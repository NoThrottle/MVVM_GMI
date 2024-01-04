using MVVM_GMI.ViewModels.Pages;
using Wpf.Ui.Controls;

namespace MVVM_GMI.Views.Pages
{
    public partial class MinecraftSettingsPage : INavigableView<MinecraftSettingsViewModel>
    {
        public MinecraftSettingsViewModel ViewModel { get; }

        public MinecraftSettingsPage(MinecraftSettingsViewModel viewModel)
        {
            ViewModel = viewModel;
            DataContext = this;

            InitializeComponent();
        }
    }
}
