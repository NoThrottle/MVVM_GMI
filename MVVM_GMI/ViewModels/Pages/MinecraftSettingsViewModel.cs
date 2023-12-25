using MVVM_GMI.Helpers;
using MVVM_GMI.Services;
using MVVM_GMI.Views.Pages;
using System.Diagnostics;
using System.Security.Policy;
using System.Windows.Navigation;
using Wpf.Ui;
using Wpf.Ui.Controls;
using @from = MVVM_GMI.Services.ConfigurationService;

namespace MVVM_GMI.ViewModels.Pages
{
    public partial class MinecraftSettingsViewModel : ObservableObject, INavigationAware
    {

        INavigationService _navigationService;
        public MinecraftSettingsViewModel(INavigationService navigationService)
        {
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

    }
}
