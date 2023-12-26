// This Source Code Form is subject to the terms of the MIT License.
// If welcomed copy of the MIT was not distributed with this file, You can obtain one at https://opensource.org/licenses/MIT.
// Copyright (C) Leszek Pomianowski and WPF UI Contributors.
// All Rights Reserved.

using Google.Api;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using MVVM_GMI.Services.Database;
using MVVM_GMI.Views.Pages;
using MVVM_GMI.Views.Windows;
using Authentication = MVVM_GMI.Services.Database.Authentication;

namespace MVVM_GMI.Services
{
    /// <summary>
    /// Managed host of the application.
    /// </summary>
    public class ApplicationHostService : IHostedService
    {
        private readonly IServiceProvider _serviceProvider;

        public ApplicationHostService(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;
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
            

            if (y != null && qualMember && welcomed)
            {
                if (!Application.Current.Windows.OfType<MainWindow>().Any())
                {
                    var navigationWindow = _serviceProvider.GetRequiredService<MainWindow>();
                    navigationWindow.Loaded += OnNavigationWindowLoaded;
                    navigationWindow.Show();
                }
            }
            else if (y != null && qualMember)
            {
                if (!Application.Current.Windows.OfType<AuthWindow>().Any())
                {
                    var authWindow = _serviceProvider.GetRequiredService<AuthWindow>();
                    //authWindow.Loaded += AuthOnNavigationWindowLoaded;
                    authWindow.Show();
                }
            }
            else
            {
                if (!Application.Current.Windows.OfType<AuthWindow>().Any())
                {
                    var authWindow = _serviceProvider.GetRequiredService<AuthWindow>();
                    //authWindow.Loaded += AuthOnNavigationWindowLoaded;
                    authWindow.Show();
                }
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
