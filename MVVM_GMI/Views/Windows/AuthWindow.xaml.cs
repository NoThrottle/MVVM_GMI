using MVVM_GMI.ViewModels.Windows;
using Wpf.Ui;
using Wpf.Ui.Controls;

namespace MVVM_GMI.Views.Windows
{
    public partial class AuthWindow
    {
        public AuthWindowViewModel ViewModel { get; }

        public AuthWindow(
            AuthWindowViewModel viewModel,
            INavigationService navigationService,
            IServiceProvider serviceProvider,
            ISnackbarService snackbarService,
            IContentDialogService contentDialogService
        )
        {
            Wpf.Ui.Appearance.SystemThemeWatcher.Watch(this);

            ViewModel = viewModel;
            DataContext = this;

            InitializeComponent();

            contentDialogService.SetContentPresenter(AuthRootContentDialog);
            

        }

        private void Hyperlink_RequestNavigate(object sender, System.Windows.Navigation.RequestNavigateEventArgs e)
        {
            System.Diagnostics.Process.Start("explorer", e.Uri.ToString());
        }

    }
}
