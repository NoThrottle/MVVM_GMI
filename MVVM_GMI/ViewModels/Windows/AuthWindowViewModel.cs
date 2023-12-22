using Microsoft.Extensions.DependencyInjection;
using MVVM_GMI.Services.Database;
using MVVM_GMI.Views.Windows;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
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


        [RelayCommand]
        async Task LoginAsync()
        {
            var x = new Authentication();
            bool y = x.Login(LoginUsername, LoginPassword);

            if (!y)
            {
                await ShowDialogAsync("Login Error:", "Incorrect Username or Password", "", "", "Okay");
                return;
            }

            if (!Application.Current.Windows.OfType<MVVM_GMI.Views.Windows.MainWindow>().Any())
            {
                var navigationWindow = _serviceProvider.GetRequiredService<MVVM_GMI.Views.Windows.MainWindow>();
                navigationWindow.Loaded += (sender, e) =>
                {

                    if (sender is not MVVM_GMI.Views.Windows.MainWindow navigationWindow)
                    {
                        return;
                    }

                    navigationWindow.NavigationView.Navigate(typeof(MVVM_GMI.Views.Pages.DashboardPage));

                };
                navigationWindow.Show();

                _serviceProvider.GetRequiredService<MVVM_GMI.Views.Windows.AuthWindow>().Close();
            }
        }

        [RelayCommand]
        async Task CreateAccountAsync()
        {
            var x = new Authentication();
            List<String>? y = x.SignUp(SignupUsername, SignupPassword, SignupCode);

            if(y == null)
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

                        navigationWindow.NavigationView.Navigate(typeof(MVVM_GMI.Views.Pages.ProfilePage));

                    };
                    navigationWindow.Show();

                    _serviceProvider.GetRequiredService<MVVM_GMI.Views.Windows.AuthWindow>().Close();
                }
            }
            else
            {
                await ShowDialogAsync("Sign-up Error:", String.Join(Environment.NewLine,y), "", "", "Okay");
                return;
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

    }
}
