using MVVM_GMI.ViewModels.Pages;
using Wpf.Ui.Controls;

namespace MVVM_GMI.Views.Pages
{
    /// <summary>
    /// Interaction logic for ProfilePage.xaml
    /// </summary>
    public partial class ADMINModManagerPage : INavigableView<ADMINModManagerViewModel>
    {

        public ADMINModManagerViewModel ViewModel { get; }

        public ADMINModManagerPage(ADMINModManagerViewModel viewModel)
        {
            ViewModel = viewModel;
            DataContext = this;

            InitializeComponent();
        }
    }
}