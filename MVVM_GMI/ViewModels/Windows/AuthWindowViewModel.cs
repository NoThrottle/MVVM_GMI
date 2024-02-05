using MVVM_GMI.Services.Database;
using MVVM_GMI.Views.Pages;
using Wpf.Ui;
using Wpf.Ui.Extensions;
using static MVVM_GMI.Services.Database.API;

namespace MVVM_GMI.ViewModels.Windows
{
    public partial class AuthWindowViewModel : ObservableObject
    {

        INavigationService _navigationService;
        IServiceProvider _serviceProvider;
        IContentDialogService _contentDialogService;

        public AuthWindowViewModel(INavigationService navigationService, IServiceProvider serviceProvider, IContentDialogService contentDialogService)
        {
            _navigationService = navigationService;
            _serviceProvider = serviceProvider;
            _contentDialogService = contentDialogService;
        }


        [ObservableProperty]
        private string _applicationTitle = "HighSkyMC Launcher";

        //------------

        [ObservableProperty]
        private string _loginVisible = "Visible";

        [ObservableProperty]
        private string _signupVisible = "Collapsed";

        [ObservableProperty]
        private string _authVisible = "Visible";

        //Login

        [ObservableProperty]
        private string _loginUsername = "";

        [ObservableProperty]
        private string _loginPassword = "";

        //Signup

        [ObservableProperty]
        private string _signupUsername = "";

        [ObservableProperty]
        private string _signupPassword = "";

        [ObservableProperty]
        private string _signupCode = "";

        [ObservableProperty]
        private string _signupTickRules = "False";

        [ObservableProperty]
        private string _signupTickTerms = "False";

        [ObservableProperty]
        private string _signupEnabled = "False";

        //Process

        [ObservableProperty]
        private string _membershipProcessVisible = "Collapsed";

        [ObservableProperty]
        private string _awaitingResponseVisible = "Collapsed";

        [ObservableProperty]
        private string _oneMoreThingVisible = "Collapsed";

        [ObservableProperty]
        private string _rejectedVisible = "Collapsed";

        [ObservableProperty]
        private string _submittedReferenceCode = "";

        [ObservableProperty]
        private string _submittedEmail = "";

        [ObservableProperty]
        private string _toSubmitReferenceCode = "False";

        [ObservableProperty]
        private string _toSubmitEmail = "False";

        //WelcomeScreen

        [ObservableProperty]
        private string _welcomeScreenVisible = "Collapsed";


        [RelayCommand]
        async Task SubmitMembershipRequestAsync()
        {

            var payment = new Payment()
            {
                ReferenceCode = ToSubmitReferenceCode,
                Email = ToSubmitEmail
            };

            (bool sent, bool success, string error, Membership membership) = await Auth.SubmitPayment(payment);

            if (!sent)
            {
                await ShowDialogAsync(
                    "Error",
                    "Unable to connect to the internet. Ensure you have a proper connection",
                    "",
                    "",
                    "Okay"
                );
                return;
            }

            if (!success)
            {
                await ShowDialogAsync(
                    "Error",
                    "Unable to connect to send request. \n Error: " + error ?? "",
                    "",
                    "",
                    "Okay"
                );
                return;
            }
            else
            {

                await RefreshMembershipAsync();

            }

            //IF SUCCESS OPEN MEMBERSHIP WAITING RESPONSE PAGE
            
        }

        [RelayCommand]
        void LogOut()
        {
            API.LogOut();

            ClearFields();
            SwitchToLogin();
            AuthVisible = "Visible";
            MembershipProcessVisible = "Collapsed";           
        }


        [RelayCommand]
        async Task RefreshMembershipAsync()
        {
            ClearFields();

            try
            {
                await RefreshMembership(MVVM_GMI.Services.UserProfileService.AuthorizedUsername);
            }
            catch
            {
                await ShowDialogAsync("Error", "Unable to connect to the internet. Ensure you have a proper connection", "", "", "Okay");
                LogOut();
                SwitchToCreating();
            }
        }

        [RelayCommand]
        async Task WelcomeAsync()
        {
            await Auth.Welcome();
            GoToMainWindowPage<DashboardPage>();
        }

        async Task RefreshMembership(string Username)
        {

            APIResponse response = await Auth.Membership();

            if (!response.Sent)
            {
                await ShowDialogAsync("Error", "Unable to connect to the internet. Ensure you have a proper connection", "", "", "Okay");
                return;
            }

            if (!response.Success)
            {
                await ShowDialogAsync("Error", response.Error, "", "", "Okay");
                return;
            }

            (bool isMember, Membership mem) = (Tuple<bool, Membership>) response.Content;

            if (isMember)
            {
                GoToMainWindowPage<DashboardPage>();
                return;
            }

            if (mem.UserMembership.QualifiedMember && !mem.UserMembership.isWelcomed) 
            {

                WelcomeScreenVisible = "Visible";
                return;
            
            }

            MembershipProcessVisible = "Visible";
            var x = mem.UserMembership;

            if (mem.UserMembershipRequest == null)
            {
                OneMoreThingVisible = "Visible";
                return;
            }
            else
            {
                SubmittedEmail = mem.UserMembershipRequest.Email;
                SubmittedReferenceCode = mem.UserMembershipRequest.ReferenceCode;
            }

            if (x.hasSubmitted && x.hasErrorResponse)
            {
                RejectedVisible = "Visible";
            }

            if (x.hasSubmitted && !x.hasErrorResponse)
            {
                AwaitingResponseVisible = "Visible";
            }

            System.Windows.MessageBox.Show("idiot");
        }


        [RelayCommand]
        async Task LoginAsync()
        {

            if (String.IsNullOrEmpty(LoginUsername) || 
                LoginUsername.Trim().Length < 6 ||  
                !Helpers.Extensions.IsAlphanumericUnderscore(LoginUsername) || 
                String.IsNullOrEmpty(LoginPassword) || 
                LoginPassword.Trim().Length < 8)
            {
                await ShowDialogAsync(
                    "Login Error:", 
                    "Username or Password too short or empty.", 
                    "", 
                    "", 
                    "Okay");
                return;
            }

            var loginCredentials = new LoginCredentials()
            {
                username = LoginUsername,
                password = LoginPassword
            };

            APIResponse response = await Auth.LoginNormal(loginCredentials);

            if (!response.Sent)
            {
                await ShowDialogAsync("Error", "Unable to connect to the internet. Ensure you have a proper connection", "", "", "Okay");
                return;
            }

            if (!response.Success)
            {
                await ShowDialogAsync("Login Error:", response.Error, "", "", "Okay");
                return;
            }

            await RefreshMembershipAsync();
            LoginVisible = "Collapsed";

        }

        [RelayCommand]
        async Task CreateAccountAsync()
        {
            var x = ValidateRegistration(SignupUsername, SignupPassword);
            if (x.Count > 0)
            {
                await ShowDialogAsync("Sign-up Error:", String.Join(Environment.NewLine, x), "", "", "Okay");
                return;
            }

            var registration = new RegisterCredentials()
            {
                username = SignupUsername,
                password = SignupPassword,
                inviteCode = SignupCode
            };

            APIResponse response = await Auth.Register(registration);

            if (response.Sent && response.Success)
            {

                await RefreshMembershipAsync();
                SignupVisible = "Collapsed";
            }
            else
            {
                await ShowDialogAsync(
                    "Sign-up Error:", 
                    response.Error , 
                    "", 
                    "",
                    "Okay");
                return;
            }

        }

        [RelayCommand]
        void SwitchToCreating()
        {
            ClearFields();
            AuthVisible = "Visible";
            SignupVisible = "Visible";
            LoginVisible = "Collapsed";
        }

        [RelayCommand]
        void SwitchToLogin()
        {
            ClearFields();
            AuthVisible = "Visible";
            SignupVisible = "Collapsed";
            LoginVisible = "Visible";
        }

        [RelayCommand]
        void UpdateSignUpButton()
        {
            int x = 0;
            if (SignupTickRules.ToLower() == "true")
            {
                x++;
            }

            if (SignupTickTerms.ToLower() == "true")
            {
                x++;
            }

            if (x == 2)
            {
                SignupEnabled = "true";
            }
            else
            {
                SignupEnabled = "false";
            }
        }

        void ClearFields()
        {

            AuthVisible = "Collapsed";

            MembershipProcessVisible = "Collapsed";
                AwaitingResponseVisible = "Collapsed";
                OneMoreThingVisible = "Collapsed";
                RejectedVisible = "Collapsed";

            WelcomeScreenVisible = "Collapsed";

            SubmittedReferenceCode = "";
            SubmittedEmail = "";

            ToSubmitEmail = "";
            ToSubmitReferenceCode = "";

            SignupUsername = "";
            SignupPassword = "";
            SignupCode = "";
            SignupTickRules = "False";
            SignupTickTerms = "False";

            LoginUsername = "";
            LoginPassword = "";

            ToSubmitEmail = "";
            ToSubmitReferenceCode = "";
        }

        async Task<Wpf.Ui.Controls.ContentDialogResult> ShowDialogAsync(string Title, string Content, string PrimaryButtonText, string SecondaryButtonText, string CloseButtonText)
        {
            var x = await _contentDialogService.ShowSimpleDialogAsync(
                    new SimpleContentDialogCreateOptions()
                    {
                        Title = Title,
                        Content = Content,
                        PrimaryButtonText = PrimaryButtonText,
                        SecondaryButtonText = SecondaryButtonText,
                        CloseButtonText = CloseButtonText
                    }
                    );

            return x;
        }

        void GoToMainWindowPage<Page>()
        {


            Helpers.Extensions.RestartApplication();

        }

        List<string> ValidateRegistration(string Username, string Password)
        {
            List<string> result = new List<string>();

            if (string.IsNullOrEmpty(Username))
            {
                result.Add("Enter a username.");
            }

            if (string.IsNullOrEmpty(Password))
            {
                result.Add("Enter a password.");
            }

            if (Username.Length < 6)
            {
                result.Add("Username must be longer than 6 characters.");
            }

            if (Username.Length > 16)
            {
                result.Add("Username must be shorter than 17 characters.");
            }
            
            if (!MVVM_GMI.Helpers.Extensions.IsAlphanumericUnderscore(Username))
            {
                result.Add("Username must only contain alphanumerics and underscores.");
            }

            if (Password.Length < 8)
            {
                result.Add("Password must be at least 8 characters.");
            }

            if (Password.Length > 16)
            {
                result.Add("Password must be shorter than 17 characters.");
            }

            return result;
        }
    }
}
