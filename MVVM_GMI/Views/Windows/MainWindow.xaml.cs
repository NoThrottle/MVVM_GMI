using MVVM_GMI.ViewModels.Windows;
using Wpf.Ui;
using Wpf.Ui.Controls;

namespace MVVM_GMI.Views.Windows
{
    public partial class MainWindow
    {
        public MainWindowViewModel ViewModel { get; set; }

        public MainWindow(
            MainWindowViewModel viewModel,
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

            navigationService.SetNavigationControl(NavigationView);
            snackbarService.SetSnackbarPresenter(SnackbarPresenter);
            contentDialogService.SetContentPresenter(RootContentDialog);

            NavigationView.SetServiceProvider(serviceProvider);

            
        }
    }
}
