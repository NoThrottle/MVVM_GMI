using Microsoft.Extensions.DependencyInjection;
using MVVM_GMI.Services.Database;
using MVVM_GMI.Views.Windows;
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

        public Action isLogOut {  get; set; }

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

            new Authentication().LogOut();


            Application.Current.Shutdown();
            System.Windows.Forms.Application.Restart();



        }
    }

    
}
