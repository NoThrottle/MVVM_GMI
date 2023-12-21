using MVVM_GMI.Helpers;
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
            Process.Start("explorer", "ms-settings:display-advancedgraphics-default");
        }

        [RelayCommand]
        private void GoBack()
        {
            SaveValues();
            _navigationService.GoBack();
        }

        private void InitializeValues()
        {
            Display_isFullscreen = from.Minecraft.StartFullscreen.ToString();
            Display_width = from.Minecraft.StartingWidth;
            Display_height = from.Minecraft.StartingHeight;

            Ram_maximum = SystemInfo.SystemRam();
            Ram_current = from.Minecraft.MaxRamAllocation;
            Ram_capped = from.Minecraft.CapRamAllocation.ToString();
            //JvmArguments = String.Join(", ", from.Minecraft.JVMArguments);
        }

        private void SaveValues()
        {
            from.Minecraft.StartFullscreen = bool.Parse(Display_isFullscreen);
            from.Minecraft.StartingWidth = Display_width;
            from.Minecraft.StartingHeight = Display_height;

            from.Minecraft.MaxRamAllocation = Ram_current;
            from.Minecraft.CapRamAllocation = bool.Parse(Ram_capped);

            from.WriteProperties();
            //from.Minecraft.JVMArguments = JvmArguments.Split(", ");
        }

    }
}
