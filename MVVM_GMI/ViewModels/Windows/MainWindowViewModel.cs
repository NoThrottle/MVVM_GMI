using MVVM_GMI.Views.Pages;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Security.Policy;
using Wpf.Ui.Controls;

namespace MVVM_GMI.ViewModels.Windows
{
    public partial class MainWindowViewModel : ObservableObject
    {

        [ObservableProperty]
        private string _applicationTitle = "HighSkyMC Launcher";

        [RelayCommand]
        void OpenWebsite()
        {
            Process.Start("explorer", "https://docs.nothrottle.com");
        }

        [RelayCommand]
        void OpenDiscord()
        {
            Process.Start("explorer", "https://discord.gg/EFKhZASA2X");
        }

        [ObservableProperty]
        private ObservableCollection<object> _menuItems = new()
        {
            new NavigationViewItem()
            {
                Content = "Play",
                Icon = new SymbolIcon { Symbol = SymbolRegular.XboxController24 },
                TargetPageType = typeof(Views.Pages.DashboardPage)
            },
            //new NavigationViewItem()
            //{
            //    Content = "Info",
            //    Icon = new SymbolIcon { Symbol = SymbolRegular.DataHistogram24 },
            //    TargetPageType = typeof(Views.Pages.DonatePage)
            //},

#if DEBUG
            new NavigationViewItem()
            {
                Content = "Mods",
                Icon = new SymbolIcon { Symbol = SymbolRegular.Book24 },
                TargetPageType = typeof(Views.Pages.ADMINModManagerPage)
            },

            new NavigationViewItem()
            {
                Content = "Actions",
                Icon = new SymbolIcon { Symbol = SymbolRegular.Run24 },
                TargetPageType = typeof(Views.Pages.JSONActionPage)
            },
#endif
            new NavigationViewItem()
            {
                Content = "Donate",
                Icon = new SymbolIcon { Symbol = SymbolRegular.Money24 },
                TargetPageType = typeof(Views.Pages.DonatePage)
            }
        };

        [ObservableProperty]
        private ObservableCollection<object> _footerMenuItems = new()
        {
            new NavigationViewItem()
            {
                Content = "Profile",
                Icon = new SymbolIcon { Symbol =  SymbolRegular.Person24},
                TargetPageType = typeof(Views.Pages.ProfilePage)
            },

            new NavigationViewItem()
            {
                Content = "Settings",
                Icon = new SymbolIcon { Symbol = SymbolRegular.Settings24 },
                TargetPageType = typeof(Views.Pages.SettingsPage)
            }
        };

        [ObservableProperty]
        private ObservableCollection<MenuItem> _trayMenuItems = new()
        {
            new MenuItem { Header = "Home", Tag = "tray_home" }
        };
    }
}
