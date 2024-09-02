using CommunityToolkit.Mvvm.Messaging;
using Google.Cloud.Firestore;
using Microsoft.Web.WebView2.Core;
using MVVM_GMI.Messages;
using MVVM_GMI.Services;
using Wpf.Ui;
using Wpf.Ui.Controls;
using @online = MVVM_GMI.Helpers.OnlineRequest;

namespace MVVM_GMI.ViewModels.Pages
{
    public partial class DocumentationViewModel : ObservableObject, INavigationAware
    {

        IContentDialogService _contentDialogService;
        private bool _isInitialized = false;

        public void OnNavigatedTo()
        {
            if (!_isInitialized)
                InitializeViewModel();


            if (LastVisitedURL == "" || !LastVisitedURL.Contains("https://docs.nothrottle.com"))
            {
                Source = "https://docs.nothrottle.com/";
            }
            else 
            { 
                Source = LastVisitedURL; 
            }

            WeakReferenceMessenger.Default.Send(new WebViewMessage()
            {
                OpenOrClose = true,
                Source = Source
            });
        }

        public void OnNavigatedFrom() {

            WeakReferenceMessenger.Default.Send(new WebViewMessage()
            {
                OpenOrClose = false
            });

        }

        [RelayCommand]
        public void ChangeLastVisitedURL(string URL)
        {
            LastVisitedURL = URL;
        }


        private void InitializeViewModel()
        {
            _isInitialized = true;
        }

        public DocumentationViewModel(IContentDialogService contentDialog)
        {
            _contentDialogService = contentDialog;
        }


        [ObservableProperty]
        static string _source = "";

        [ObservableProperty]
        static string _lastVisitedURL = "";


        async Task<Wpf.Ui.Controls.ContentDialogResult> ShowDialogAsync(string Title, string Content, string PrimaryButtonText, string SecondaryButtonText, string CloseButtonText)
        {
            var x = await _contentDialogService.ShowAsync(
                    new ContentDialog()
                    {
                        Title = Title,
                        Content = Content,
                        PrimaryButtonText = PrimaryButtonText,
                        SecondaryButtonText = SecondaryButtonText,
                        CloseButtonText = CloseButtonText
                    }, new CancellationToken()
                    );

            return x;
        }

    }

}
