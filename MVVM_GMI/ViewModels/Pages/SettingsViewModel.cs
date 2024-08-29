using MVVM_GMI.Services;
using Wpf.Ui;
using Wpf.Ui.Controls;

namespace MVVM_GMI.ViewModels.Pages
{
    public partial class SettingsViewModel : ObservableObject, INavigationAware
    {
        IContentDialogService _dialogService;
        private bool _isInitialized = false;

        [ObservableProperty]
        private string _appVersion = String.Empty;

        [ObservableProperty]
        private Wpf.Ui.Appearance.ApplicationTheme _currentTheme = Wpf.Ui.Appearance.ApplicationTheme.Unknown;

        public SettingsViewModel(IContentDialogService contentDialogService)
        {
            _dialogService = contentDialogService;
        }

        public void OnNavigatedTo()
        {
            if (!_isInitialized)
                InitializeViewModel();
        }

        public void OnNavigatedFrom()
        { 
        
            ConfigurationService.Instance.fromLauncher.SimultaneousDownloads = SimDownloads;
            
        }

        private void InitializeViewModel()
        {
            CurrentTheme = Wpf.Ui.Appearance.ApplicationThemeManager.GetAppTheme();
            AppVersion = $"HighSkyMC Launcher - {GetAssemblyVersion()}";

            _isInitialized = true;
        }

        private string GetAssemblyVersion()
        {
            return System.Reflection.Assembly.GetExecutingAssembly().GetName().Version?.ToString()
                ?? String.Empty;
        }


        [ObservableProperty]
        private int _simDownloads = ConfigurationService.Instance.fromLauncher.SimultaneousDownloads;

        [RelayCommand]
        async Task CheckForUpdatesAsync()
        {
            var x = new Services.UpdateService();

            if (x.CheckForUpdates())
            {
                await ShowDialogAsync("New Version Available", "A new version of the launcher is available and updating is required.", "", "", "Okay");

                if (x.StartUpdate())
                {
                    Application.Current.Shutdown();
                }
            }
            else
            {
                await ShowDialogAsync("No Updates", "You are currently using the latest version of the launcher.", "", "", "Okay");
            }
        }

        [RelayCommand]
        private void OnChangeTheme(string parameter)
        {
            switch (parameter)
            {
                case "theme_light":
                    if (CurrentTheme == Wpf.Ui.Appearance.ApplicationTheme.Light)
                        break;

                    Wpf.Ui.Appearance.ApplicationThemeManager.Apply(Wpf.Ui.Appearance.ApplicationTheme.Light);
                    CurrentTheme = Wpf.Ui.Appearance.ApplicationTheme.Light;

                    break;

                default:
                    if (CurrentTheme == Wpf.Ui.Appearance.ApplicationTheme.Dark)
                        break;

                    Wpf.Ui.Appearance.ApplicationThemeManager.Apply(Wpf.Ui.Appearance.ApplicationTheme.Dark);
                    CurrentTheme = Wpf.Ui.Appearance.ApplicationTheme.Dark;

                    break;
            }
        }

        async Task<Wpf.Ui.Controls.ContentDialogResult> ShowDialogAsync(string Title, string Content, string PrimaryButtonText, string SecondaryButtonText, string CloseButtonText)
        {
            var x = await _dialogService.ShowAsync(
                    new ContentDialog()
                    {
                        Title = Title,
                        Content = Content,
                        PrimaryButtonText = PrimaryButtonText,
                        SecondaryButtonText = SecondaryButtonText,
                        CloseButtonText = CloseButtonText
                    }, new CancellationToken()
                    );

            return x;
        }
    }
}
