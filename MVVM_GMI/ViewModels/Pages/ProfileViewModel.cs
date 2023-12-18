using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Wpf.Ui.Controls;

namespace MVVM_GMI.ViewModels.Pages
{
    public partial class ProfileViewModel : ObservableObject, INavigationAware
    {
        private bool _isInitialized = false;

        [ObservableProperty]
        private int _counter = 0;

        [ObservableProperty]
        private string _whatsNewText = "New Things\n Old things\n More new things\nsome bugfixes\n uhuh\noooooo\nnoooooooooooo";

        [RelayCommand]
        private void OnCounterIncrement()
        {
            Counter++;
        }

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
