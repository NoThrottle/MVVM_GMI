using Microsoft.Extensions.DependencyInjection;
using MVVM_GMI.Services.Database;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using Wpf.Ui.Controls;

namespace MVVM_GMI.ViewModels.Pages
{
    public partial class ProfileViewModel : ObservableObject, INavigationAware
    {
        private bool _isInitialized = false;

        [ObservableProperty]
        private int _counter = 0;

        [ObservableProperty]
        private string _username = MVVM_GMI.Services.UserProfileService.AuthorizedUsername ?? string.Empty;

        public void OnNavigatedTo()
        {
            if (!_isInitialized)
                InitializeViewModel();
        }

        public void OnNavigatedFrom() { }

        IServiceProvider _serviceProvider;
        public ProfileViewModel(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;

        }

        private void InitializeViewModel()
        {

            _isInitialized = true;
        }

        [RelayCommand]
        void LogOut()
        {
            var x = new Authentication();
            x.LogOut();

            if (!Application.Current.Windows.OfType<MVVM_GMI.Views.Windows.AuthWindow>().Any())
            {
                var navigationWindow = _serviceProvider.GetRequiredService<MVVM_GMI.Views.Windows.AuthWindow>();
                navigationWindow.Loaded += (sender, e) =>
                {

                    if (sender is not MVVM_GMI.Views.Windows.MainWindow navigationWindow)
                    {
                        return;
                    }

                    navigationWindow.Activate();
                };
                navigationWindow.Show();

                _serviceProvider.GetRequiredService<MVVM_GMI.Views.Windows.MainWindow>().Close();
            }
        }
    }

    
}
