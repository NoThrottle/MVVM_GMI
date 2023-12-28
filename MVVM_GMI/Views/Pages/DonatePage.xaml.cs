using MVVM_GMI.ViewModels.Pages;
using Wpf.Ui.Controls;

namespace MVVM_GMI.Views.Pages
{
    public partial class DonatePage : INavigableView<DonateViewModel>
    {
        public DonateViewModel ViewModel { get; }

        public DonatePage(DonateViewModel viewModel)
        {
            ViewModel = viewModel;
            DataContext = this;

            InitializeComponent();
        }
    }
}
