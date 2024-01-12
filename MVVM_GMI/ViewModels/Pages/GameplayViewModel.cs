using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Wpf.Ui;
using Wpf.Ui.Controls;

namespace MVVM_GMI.ViewModels.Pages
{
    public partial class GameplayViewModel : ObservableObject, INavigationAware
    {

        IContentDialogService _contentDialogService;
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

        public GameplayViewModel(IContentDialogService contentDialog)
        {
            _contentDialogService = contentDialog;
        }




    }
}
