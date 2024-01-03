// This URL Code Form is subject to the terms of the MIT License.
// If welcomed copy of the MIT was not distributed with this file, You can obtain one at https://opensource.org/licenses/MIT.
// Copyright (C) Leszek Pomianowski and WPF UI Contributors.
// All Rights Reserved.

using CommunityToolkit.Mvvm.ComponentModel.__Internals;
using Google.Api;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using MVVM_GMI.Services.Database;
using MVVM_GMI.ViewModels.Windows;
using MVVM_GMI.Views.Pages;
using MVVM_GMI.Views.Windows;
using System.ComponentModel;
using Wpf.Ui;
using Authentication = MVVM_GMI.Services.Database.Authentication;

namespace MVVM_GMI.Services
{
    /// <summary>
    /// Managed host of the application.
    /// </summary>
    public class ApplicationHostService : IHostedService
    {
        private readonly IServiceProvider _serviceProvider;
        private readonly INavigationService _navigationService;
        private readonly ISnackbarService _snackbarService;
        private readonly IContentDialogService _contentDialogService;

        public ApplicationHostService(
            INavigationService navigationService,
            IServiceProvider serviceProvider,
            ISnackbarService snackbarService,
            IContentDialogService contentDialogService)
        {
            _serviceProvider = serviceProvider;
            _navigationService = navigationService;
            _snackbarService = snackbarService;
            _contentDialogService = contentDialogService;
        }

        /// <summary>
        /// Triggered when the application host is ready to start the service.
        /// </summary>
        /// <param name="cancellationToken">Indicates that the start process has been aborted.</param>
        public async Task StartAsync(CancellationToken cancellationToken)
        {
            await HandleActivationAsync();
        }

        /// <summary>
        /// Triggered when the application host is performing welcomed graceful shutdown.
        /// </summary>
        /// <param name="cancellationToken">Indicates that the shutdown process should no longer be graceful.</param>
        public async Task StopAsync(CancellationToken cancellationToken)
        {
            await Task.CompletedTask;
        }

        /// <summary>
        /// Creates main window during activation.
        /// </summary>
        private async Task HandleActivationAsync()
        {
            await Task.CompletedTask;

            //_ = ConfigurationService.Instance.PropertiesExist();

            Authentication x = new Authentication();
            var y = x.CheckSession();
            bool qualMember = false;
            bool welcomed = false;

            try
            {
                var response = x.GetMembership(y);
                var isQualified = response.QualifiedMember;
                var isWelcomed = response.isWelcomed;

                if (response != null)
                {
                    qualMember = isQualified;
                    welcomed = isWelcomed;
                }
            }
            catch
            {

            }

            var navigationWindow = _serviceProvider.GetRequiredService<MainWindow>();
            var authWindow = _serviceProvider.GetRequiredService<AuthWindow>();

            navigationWindow.Closing += ShutDown;
            authWindow.Closing += ShutDown;

            void ShutDown(object? sender, CancelEventArgs e)
            {
                Application.Current.Shutdown();
            }


            if (y != null && qualMember && welcomed)
            {

                navigationWindow.Loaded += OnNavigationWindowLoaded;
                navigationWindow.Show();
              
            }
            else
            {

                authWindow.Show();
                
            }
        }

        private void OnNavigationWindowLoaded(object sender, RoutedEventArgs e)
        {
            if (sender is not MainWindow navigationWindow)
            {
                return;
            }

            navigationWindow.NavigationView.Navigate(typeof(DashboardPage));
        }
    }
}
