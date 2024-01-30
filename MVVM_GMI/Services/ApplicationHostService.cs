using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using MVVM_GMI.Services.Database;
using MVVM_GMI.Views.Pages;
using MVVM_GMI.Views.Windows;
using System.ComponentModel;
using Wpf.Ui;

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

            var navigationWindow = _serviceProvider.GetRequiredService<MainWindow>();
            var authWindow = _serviceProvider.GetRequiredService<AuthWindow>();

            navigationWindow.Closing += ShutDown;
            authWindow.Closing += ShutDown;

            void ShutDown(object? sender, CancelEventArgs e)
            {
                Application.Current.Shutdown();
            }

            if (await API.Auth.LoginAsync())
            {
                // check membership
                (bool success, bool isQualified, Membership? membership) = await API.Auth.Membership();

                if (success && isQualified && membership.UserMembership.isWelcomed)
                {
                    navigationWindow.Loaded += OnNavigationWindowLoaded;
                    navigationWindow.Show();
                }

                authWindow.Show();
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
