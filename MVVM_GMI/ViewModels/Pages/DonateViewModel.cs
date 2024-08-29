using Google.Cloud.Firestore;
using MVVM_GMI.Services;
using Wpf.Ui;
using Wpf.Ui.Controls;
using @online = MVVM_GMI.Helpers.OnlineRequest;

namespace MVVM_GMI.ViewModels.Pages
{
    public partial class DonateViewModel : ObservableObject, INavigationAware
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

        public DonateViewModel(IContentDialogService contentDialog)
        {
            _contentDialogService = contentDialog;
        }






        [ObservableProperty]
        private string _referenceNumber = "";

        [RelayCommand]
        async Task SubmitDonationAsync()
        {

            if (string.IsNullOrEmpty(ReferenceNumber))
            {
                await ShowDialogAsync("Error","You must input a valid reference number.","","","Okay");
                return;
            }

            var x = new Donation()
            {
                Username = UserProfileService.AuthorizedUsername,
                Reference = ReferenceNumber,
                Date = DateTimeOffset.UtcNow.UtcDateTime.ToLongDateString(),
            };

            try
            {
                await online.WriteToDatabaseAsync("Donations", UserProfileService.AuthorizedUsername ?? "Anonymous", x);
                await ShowDialogAsync("Donated!", "Thank you for donating! Your donation will be reflected in your account soon.", "", "", "Okay");
                ReferenceNumber = string.Empty;
            }
            catch (Exception e)
            {
                await ShowDialogAsync("Error", "Something went wrong. " + e.Message, "", "", "Okay");
                return;
            }


        }


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

    [FirestoreData]
    internal class Donation
    {
        [FirestoreProperty]
        public string? Username {  get; set; }

        [FirestoreProperty]
        public string? Reference { get; set; }

        [FirestoreProperty]
        public string? Date { get; set; }
    }

}
