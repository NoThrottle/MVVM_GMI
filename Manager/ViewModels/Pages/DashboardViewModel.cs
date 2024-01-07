using CmlLib.Utils;
using Microsoft.Extensions.DependencyInjection;
using MVVM_GMI.Services;
using MVVM_GMI.Views.Pages;
using MVVM_GMI.Views.Windows;
using Wpf.Ui;

namespace MVVM_GMI.ViewModels.Pages
{
    public partial class DashboardViewModel : ObservableObject
    {

        IContentDialogService _dialogService;
        INavigationService _navigationService;
        IServiceProvider _serviceProvider;

        [ObservableProperty]
        private int _counter = 0;

        [ObservableProperty]
        private string _whatsNewText = "";

        #region Play Button
        //Play Button Properties
        [ObservableProperty]
        private string _playButtonColor = "Orange";

        [ObservableProperty]
        private string _playButtonText = "PLAY ";

        [ObservableProperty]
        private string _playButtonHoverColor = "Yellow";

        [ObservableProperty]
        private string _playButtonPressedColor = "White";

        [ObservableProperty]
        private string _playButtonPressedTextColor = "White";
        #endregion

        #region Process
        //PROCESS PROGRESS

        [ObservableProperty]
        private string _processVisibility = "Collapsed";

        [ObservableProperty]
        private string _processText = "";

        [ObservableProperty]
        private string _processDescription = "";

        [ObservableProperty]
        private bool _loadingBarIntermediate = true;

        [ObservableProperty]
        private int _loadingBarMaximumValue = 100;

        [ObservableProperty]
        private int _loadingBarCurrentValue = 0;
        #endregion

        public DashboardViewModel(IContentDialogService contentDialogService, INavigationService navigationService, IServiceProvider serviceProvider)
        {
            _dialogService = contentDialogService;
            _navigationService = navigationService;
            _serviceProvider = serviceProvider;

            var x = _serviceProvider.GetRequiredService<MainWindow>();
            _dialogService.SetContentPresenter(x.RootContentDialog);
            
        }

        [RelayCommand]
        private void OpenMinecraftSettings()
        {
            _navigationService.Navigate(typeof(MinecraftSettingsPage));
        }


        [RelayCommand]
        private void OnCounterIncrement()
        {
            Counter++;
          
        }


        bool Locked = false;
        [RelayCommand]
        void PressedPlay()
        {
            if (Locked) { return; }

            Task.Run(async () => 
            {

                Locked = true;
                var z = new MinecraftService();

                z.ProgressUpdated += (s) => UpdateUI(s);
                z.TaskCompleted += (a) =>
                {
                    
                    PlayButtonState(4);
                    Locked = false;

                };

                await z.DefaultStartupAsync();

            });
        }

        [RelayCommand]
        void PressedQuickLaunch()
        {
            if (Locked) { return; }

            Task.Run(async () =>
            {

                Locked = true;
                var z = new MinecraftService();

                z.ProgressUpdated += (s) => UpdateUI(s);
                z.TaskCompleted += (a) =>
                {
                    PlayButtonState(4);
                    Locked = false;

                    if (a == false)
                    {

                        Application.Current.Dispatcher.Invoke((Action)async delegate {
                            var x = await ShowDialogAsync("Quick Launch Note:", "Unable to launch Minecraft using quick launch. Launch normally instead?", "Sure", "", "Cancel");
                            if (x == Wpf.Ui.Controls.ContentDialogResult.Primary)
                            {
                                PressedPlay();
                            }
                        });
                    }
                };

                await z.QuickLaunchAsync();

            });
        }


        async Task<Wpf.Ui.Controls.ContentDialogResult> ShowDialogAsync(string Title, string Content, string PrimaryButtonText, string SecondaryButtonText, string CloseButtonText)
        {
            var x = await _dialogService.ShowSimpleDialogAsync(
                    new SimpleContentDialogCreateOptions()
                    {
                        Title = Title,
                        Content = Content,
                        PrimaryButtonText = PrimaryButtonText,
                        SecondaryButtonText = SecondaryButtonText,
                        CloseButtonText = CloseButtonText
                    }
                    );

            return x;
        }

        void UpdateUI(MinecraftLoadingUpdate mcu)
        {
            PlayButtonState(mcu.IntProgress);
            ProcessText = mcu.Process;
            ProcessDescription = mcu.ProcessDescription;
            LoadingBarIntermediate = mcu.Intermediate;
            LoadingBarMaximumValue = mcu.MaxProgress;
            LoadingBarCurrentValue = mcu.CurrentProgress;

        }

        void PlayButtonState(int state)
        {
            switch (state)
            {
                case 4: // Default
                    PlayButtonText = "PLAY ";
                    PlayButtonColor = "Orange";
                    PlayButtonHoverColor = "Yellow";
                    PlayButtonPressedColor = "White";
                    PlayButtonPressedTextColor = "Gray";
                    ProcessVisibility = "Collapsed";
                    break;
                case 0: // Loading
                    PlayButtonText = "LOADING ";
                    PlayButtonColor = "Gray";
                    PlayButtonHoverColor = "Gray";
                    PlayButtonPressedColor = "Gray";
                    PlayButtonPressedTextColor = "White";
                    ProcessVisibility = "Visible";
                    break;
                case 1: // Playing
                    PlayButtonText = "PLAYING ";
                    PlayButtonColor = "Green";
                    PlayButtonHoverColor = "Green";
                    PlayButtonPressedColor = "Green";
                    PlayButtonPressedTextColor = "White";
                    ProcessVisibility = "Collapsed";
                    break;
                case 2: // Error
                    PlayButtonText = "ERROR ";
                    PlayButtonColor = "Red";
                    PlayButtonHoverColor = "Red";
                    PlayButtonPressedColor = "Red";
                    PlayButtonPressedTextColor = "White";
                    ProcessVisibility = "Collapsed";
                    break;
                case 5: // No Connection
                    PlayButtonText = "CONNECTING ";
                    PlayButtonColor = "Blue";
                    PlayButtonHoverColor = "Blue";
                    PlayButtonPressedColor = "Blue";
                    PlayButtonPressedTextColor = "White";
                    ProcessVisibility = "Collapsed";
                    break;

            }

        }
    }
}
