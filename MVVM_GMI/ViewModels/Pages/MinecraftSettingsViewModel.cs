using MVVM_GMI.Helpers;
using MVVM_GMI.Services;
using MVVM_GMI.Views.Pages;
using System.Diagnostics;
using System.IO;
using System.Security.Policy;
using System.Windows.Navigation;
using Wpf.Ui;
using Wpf.Ui.Controls;
using @from = MVVM_GMI.Services.ConfigurationService;

namespace MVVM_GMI.ViewModels.Pages
{
    public partial class MinecraftSettingsViewModel : ObservableObject, INavigationAware
    {

        IContentDialogService _contentDialogService;
        INavigationService _navigationService;
        public MinecraftSettingsViewModel(INavigationService navigationService, IContentDialogService contentDialogService)
        {
            _contentDialogService = contentDialogService;
            _navigationService = navigationService;
        }

        private bool _isInitialized = false;

        public void OnNavigatedTo()
        {
            if (!_isInitialized)
                InitializeViewModel();
        }

        public void OnNavigatedFrom() { }

        private void InitializeViewModel()
        {
            InitializeValues();
            _isInitialized = true;
        }



        [ObservableProperty]
        private string _display_isFullscreen;

        [ObservableProperty]
        private int _display_width;

        [ObservableProperty]
        private int _display_height;


        [ObservableProperty]
        private int _ram_maximum;

        [ObservableProperty]
        private int _ram_current;

        [ObservableProperty]
        private string _ram_capped;

        [ObservableProperty]
        private string _jvmArguments;


        [RelayCommand]
        private void OpenGraphicsSettings()
        {
            Process.Start("explorer", "ms-settings:display-advancedgraphics");
        }

        [RelayCommand]
        private void GoBack()
        {
            SaveValues();
            _navigationService.GoBack();
        }

        [RelayCommand]
        async Task ResetMod()
        {

            var x = await ShowDialogAsync("Are you sure?","This action will delete your entire mods folder, forcing a reinstall when you next play. Do you want to proceed?","Yes","","No");
            
            if (x == ContentDialogResult.Primary)
            {
                Directory.Delete(Path.Combine(from.Instance.fromLauncher.MinecraftPath,"mods"),true);
                from.Instance.fromLauncher.DidStarterAction = false;
                from.Instance.fromLauncher.ModUpdateIndex = 0;
            }

        }

        [RelayCommand]
        async Task ResetMinecraft()
        {
            var x = await ShowDialogAsync("Are you sure?", "This action will delete your entire minecraft folder, forcing a reinstall when you next play. Do you want to proceed?", "Yes", "", "No");

            if (x == ContentDialogResult.Primary)
            {
                Directory.Delete(from.Instance.fromLauncher.MinecraftPath, true);
                from.Instance.fromLauncher.DidStarterAction = false;
                from.Instance.fromLauncher.ModUpdateIndex = 0;
            }
        }

        private void InitializeValues()
        {
            Display_isFullscreen = from.Instance.fromMinecraft.StartFullscreen.ToString();
            Display_width = ConfigurationService.Instance.fromMinecraft.StartingWidth;
            Display_height = ConfigurationService.Instance.fromMinecraft.StartingHeight;

            Ram_maximum = SystemInfo.SystemRam();
            Ram_current = ConfigurationService.Instance.fromMinecraft.MaxRamAllocation;
            Ram_capped = from.Instance.fromMinecraft.CapRamAllocation.ToString();
            JvmArguments = String.Join(", ", from.Instance.fromMinecraft.JVMArguments);
        }

        private void SaveValues()
        {
            from.Instance.fromMinecraft.StartFullscreen = bool.Parse(Display_isFullscreen);
            from.Instance.fromMinecraft.StartingWidth = Display_width;
            from.Instance.fromMinecraft.StartingHeight = Display_height;

            from.Instance.fromMinecraft.MaxRamAllocation = Ram_current;
            from.Instance.fromMinecraft.CapRamAllocation = bool.Parse(Ram_capped);
            //from.WriteProperties();
            from.Instance.fromMinecraft.JVMArguments = JvmArguments;
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
