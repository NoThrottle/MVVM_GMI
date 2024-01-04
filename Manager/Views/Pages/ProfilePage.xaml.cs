using MVVM_GMI.ViewModels.Pages;
using Wpf.Ui.Controls;

namespace MVVM_GMI.Views.Pages
{
    /// <summary>
    /// Interaction logic for ProfilePage.xaml
    /// </summary>
    public partial class ProfilePage : INavigableView<ProfileViewModel>
    {

        public ProfileViewModel ViewModel { get; }

        public ProfilePage(ProfileViewModel viewModel)
        {
            ViewModel = viewModel;
            DataContext = this;

            InitializeComponent();
        }
    }
}
