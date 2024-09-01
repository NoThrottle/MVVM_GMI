using CommunityToolkit.Mvvm.Messaging;
using Google.Cloud.Firestore;
using Microsoft.Web.WebView2.Core;
using MVVM_GMI.Messages;
using MVVM_GMI.Models;
using MVVM_GMI.Services;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.IO;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using Wpf.Ui;
using Wpf.Ui.Controls;
using @online = MVVM_GMI.Helpers.OnlineRequest;

namespace MVVM_GMI.ViewModels.Pages
{
    public partial class ScreenshotGalleryViewModel : ObservableObject, INavigationAware
    {

        IContentDialogService _contentDialogService;
        INavigationService _navigationService;
        public ScreenshotGalleryViewModel(INavigationService navigationService, IContentDialogService contentDialogService)
        {
            _contentDialogService = contentDialogService;
            _navigationService = navigationService;
        }

        private bool _isInitialized = false;

        public void OnNavigatedTo()
        {
            if (!_isInitialized)
                InitializeViewModel();

            cancelSearch = false;
            LoadImages();
        }

        public void OnNavigatedFrom() 
        {
            cancelSearch = true;
            Images1.Clear();
        }

        private void InitializeViewModel()
        {
            _isInitialized = true;
            
        }

        [ObservableProperty]
        ObservableCollection<ScreenshotImage> _images1 = new ObservableCollection<ScreenshotImage>();

        [RelayCommand]
        private void GoBack()
        {
            _navigationService.GoBack();
        }


        private bool cancelSearch = false;

        private void LoadImages()
        {
            string folderPath = System.IO.Path.Combine(ConfigurationService.Instance.fromLauncher.MinecraftPath, "screenshots"); // Replace with your folder path
            if (!Directory.Exists(folderPath))
                return;

            Task.Run(() => {

                foreach (var filePath in Directory.GetFiles(folderPath, "*.png"))
                {

                    if (cancelSearch)
                    {
                        cancelSearch = false;
                        Images1.Clear();
                        break;
                    }

                    ScreenshotImage x = new ScreenshotImage() { 
                        Image = CreateThumbnail(filePath),
                        FilePath = filePath,
                        Filename = System.IO.Path.GetFileName(filePath),
                        Date = File.GetCreationTime(filePath).ToString("dd/MM/yyyy HH:mm:ss"),
                        SizeInMB = (new FileInfo(filePath).Length / 1024 / 1024).ToString("0.00") + " MB"
                    };

                    Application.Current.Dispatcher.BeginInvoke(() =>
                    {
                        _images1.Add(x);
                    });

                    Thread.Sleep(5); // Add a delay to prevent UI from freezing
                }

                
            });

        }
        public static BitmapSource CreateThumbnail(string imagePath, int thumbWidth = 256, int thumbHeight = 144)
        {
            using (System.Drawing.Image image = System.Drawing.Image.FromFile(imagePath))
            {
                // Calculate the new size while maintaining aspect ratio
                var ratioX = (double)thumbWidth / image.Width;
                var ratioY = (double)thumbHeight / image.Height;
                var ratio = Math.Min(ratioX, ratioY);

                int newWidth = (int)(image.Width * ratio);
                int newHeight = (int)(image.Height * ratio);

                // Create a new Bitmap object with the desired dimensions
                var thumbnail = new Bitmap(newWidth, newHeight);

                // Draw the resized image to the new Bitmap
                using (var graphics = Graphics.FromImage(thumbnail))
                {
                    graphics.CompositingQuality = CompositingQuality.AssumeLinear;
                    graphics.InterpolationMode = InterpolationMode.Low;
                    graphics.SmoothingMode = SmoothingMode.None;
                    graphics.DrawImage(image, 0, 0, newWidth, newHeight);
                }

                using (MemoryStream memory = new MemoryStream())
                {
                    thumbnail.Save(memory, System.Drawing.Imaging.ImageFormat.Bmp);
                    memory.Position = 0;
                    BitmapImage bitmapImage = new BitmapImage();
                    bitmapImage.BeginInit();
                    bitmapImage.StreamSource = memory;
                    bitmapImage.CacheOption = BitmapCacheOption.OnLoad;
                    bitmapImage.EndInit();
                    bitmapImage.Freeze();

                    return bitmapImage;
                }

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

}
