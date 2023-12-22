using MVVM_GMI.ViewModels.Pages;
using Wpf.Ui.Controls;

namespace MVVM_GMI.Views.Pages
{
    /// <summary>
    /// Interaction logic for AuthPage.xaml
    /// </summary>
    public partial class AuthPage : INavigableView<AuthViewModel>
    {
        public AuthViewModel ViewModel { get; }

        public AuthPage(AuthViewModel viewModel)
        {
            ViewModel = viewModel;
            DataContext = this;

            InitializeComponent();
        }
    }
}
