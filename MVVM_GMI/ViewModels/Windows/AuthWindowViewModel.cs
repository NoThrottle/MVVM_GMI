using Microsoft.Extensions.DependencyInjection;
using MVVM_GMI.Services.Database;
using MVVM_GMI.Views.Pages;
using Wpf.Ui;

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
            try
            {
                var x = new Authentication();
                await x.SubmitMembershipRequestAsync(Services.UserProfileService.AuthorizedUsername, ToSubmitReferenceCode, ToSubmitEmail);
            }
            catch
            {
                await ShowDialogAsync("Error","Unable to connect to the internet. Ensure you have a proper connection","","","Okay");
                
            }
            
        }

        [RelayCommand]
        void LogOut()
        {
            var x = new Authentication();
            x.LogOut();
            ClearFields();
            SwitchToLogin();
            AuthVisible = "Visible";
            MembershipProcessVisible = "Collapsed";           
        }


        [RelayCommand]
        async Task RefreshMembershipAsync()
        {
            ClearFields();

            MembershipProcessVisible = "Collapsed";
            AuthVisible = "Collapsed";
            WelcomeScreenVisible = "Collapsed";
            AwaitingResponseVisible = "Collapsed";
            OneMoreThingVisible = "Collapsed";
            RejectedVisible = "Collapsed";

            SubmittedReferenceCode = "";
            SubmittedEmail = "";

            ToSubmitEmail = "";
            ToSubmitReferenceCode = "";

            try
            {
                ProcessUserMembership(MVVM_GMI.Services.UserProfileService.AuthorizedUsername);
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
            var x = new Authentication();
            x.UpdateMembership(Services.UserProfileService.AuthorizedUsername, true, false, true, true);
            GoToMainWindowPage<DashboardPage>();
        }

        void ProcessUserMembership(string Username)
        {
            var x = new Authentication();
            var mem = x.GetMembership(Username);


            if (mem.QualifiedMember && mem.isWelcomed)
            {
                GoToMainWindowPage<DashboardPage>();
                return;
            }

            if (mem.QualifiedMember && !mem.isWelcomed) 
            {

                WelcomeScreenVisible = "Visible";
                return;
            
            }

            MembershipProcessVisible = "Visible";


            try
            {
                var memReq = x.GetMembershipRequest(Username);
                SubmittedReferenceCode = memReq.ReferenceCode;
                SubmittedEmail = memReq.Email;
            }
            catch
            {

            }

            if (mem.hasSubmitted && mem.hasErrorResponse)
            {
                RejectedVisible = "Visible";
            }

            if (mem.hasSubmitted && !mem.hasErrorResponse)
            {
                AwaitingResponseVisible = "Visible";
            }

            if (!mem.hasSubmitted)
            {
                OneMoreThingVisible = "Visible";
            }


        }


        [RelayCommand]
        async Task LoginAsync()
        {
            
            var x = new Authentication();

            try
            {
                bool y = x.Login(LoginUsername, LoginPassword);

                if (!y)
                {
                    await ShowDialogAsync("Login Error:", "Incorrect Username or Password", "", "", "Okay");
                    return;
                }

                RefreshMembershipAsync();
                LoginVisible = "False";
            }
            catch
            {
                await ShowDialogAsync("Error", "Unable to connect to the internet. Ensure you have a proper connection", "", "", "Okay");
            }

        }

        [RelayCommand]
        async Task CreateAccountAsync()
        {
            var x = new Authentication();

            List<String>? y = null;

            try
            {
                y = x.SignUp(SignupUsername, SignupPassword, SignupCode);

                if (y == null)
                {

                    RefreshMembershipAsync();
                    SignupVisible = "False";
                }
                else
                {
                    await ShowDialogAsync("Sign-up Error:", String.Join(Environment.NewLine, y), "", "", "Okay");
                    return;
                }

            }
            catch
            {
                await ShowDialogAsync("Error", "Unable to connect to the internet. Ensure you have a proper connection", "", "", "Okay");
            }


        }

        [RelayCommand]
        void SwitchToCreating()
        {
            ClearFields();
            SignupVisible = "Visible";
            LoginVisible = "Collapsed";
        }

        [RelayCommand]
        void SwitchToLogin()
        {
            ClearFields();
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
            if (!Application.Current.Windows.OfType<MVVM_GMI.Views.Windows.MainWindow>().Any())
            {
                var navigationWindow = _serviceProvider.GetRequiredService<MVVM_GMI.Views.Windows.MainWindow>();
                navigationWindow.Loaded += (sender, e) =>
                {

                    if (sender is not MVVM_GMI.Views.Windows.MainWindow navigationWindow)
                    {
                        return;
                    }

                    navigationWindow.NavigationView.Navigate(typeof(Page));

                };
                navigationWindow.Show();

                _serviceProvider.GetRequiredService<MVVM_GMI.Views.Windows.AuthWindow>().Close();
            }
        }

    }
}
