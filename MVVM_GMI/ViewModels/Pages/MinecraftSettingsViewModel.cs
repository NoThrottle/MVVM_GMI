using Wpf.Ui.Controls;

namespace MVVM_GMI.ViewModels.Pages
{
    public class MinecraftSettingsViewModel : ObservableObject, INavigationAware
    {

        private bool _isInitialized = false;

        public void OnNavigatedTo()
        {
            if (!_isInitialized)
                InitializeViewModel();
        }

        public void OnNavigatedFrom() { }

        private void InitializeViewModel()
        {
            _isInitialized = true;
        }

    }
}
