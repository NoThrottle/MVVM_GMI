using Microsoft.Extensions.DependencyInjection;
using MineStatLib;
using MVVM_GMI.Helpers;
using MVVM_GMI.Services;
using MVVM_GMI.Views.Pages;
using MVVM_GMI.Views.Windows;
using System.Collections.ObjectModel;
using System.Windows.Threading;
using Wpf.Ui;
using @online = MVVM_GMI.Helpers.OnlineRequest;

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
        private string _changelog = "";

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

            StartPlayerTimerAsync();

            try
            {
                CheckForUpdatesAsync();

                Changelog = Task.Run(async () => {

                    return await OnlineRequest.GetJsonAsync("https://pastebin.com/raw/jGgj4Mk5");


                }).Result;
            }
            catch (Exception e)
            {
                ShowDialogAsync("Error","Unable to connect to the internet. Some functions may be unavailable.", "","","Ok");
                Changelog = "No Internet";
            }


            

            
        }

        async Task CheckForUpdatesAsync()
        {
            var x = new UpdateService();
            if (x.CheckForUpdates())
            {
                await ShowDialogAsync("New Version Available", "A new version of the launcher is available and updating is required.","","","Okay");

                if (x.StartUpdate())
                {
                    Application.Current.Shutdown();
                }
            }
        }

        [RelayCommand]
        private void OpenMinecraftSettings()
        {
            _navigationService.Navigate(typeof(MinecraftSettingsPage));
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
            var x = await _dialogService.ShowAsync(
                    new Wpf.Ui.Controls.ContentDialog()
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


        //-----UI------

        [ObservableProperty]
        ObservableCollection<string> _playersOnline = new ObservableCollection<string>();

        [ObservableProperty]
        string _playersOnlineText = "PLAYERS ONLINE: 0/100";

        [ObservableProperty]
        string _onlineStatusLoadingVisibility = "Visible";

        [ObservableProperty]
        string _serverHealthText = "Unknown";

        [ObservableProperty]
        string _serverStatusText = "Unknown";

        [ObservableProperty]
        string _serverHealthColor = "#9e9e9e";

        [ObservableProperty]
        string _serverStatusColor = "#9e9e9e";

        private DispatcherTimer playersOnlineTimer;

     
        async Task StartPlayerTimerAsync()
        {

            playersOnlineTimer = new DispatcherTimer();
            playersOnlineTimer.Interval = TimeSpan.FromSeconds(5);

            try
            {
                playersOnlineTimer.Tick += RefreshMinestat;
                OnlineStatusLoadingVisibility = "Collapsed";
            }
            catch
            {
                OnlineStatusLoadingVisibility = "Visible";
            }


            playersOnlineTimer.Start();
        }

        async void RefreshMinestat(object sender, EventArgs e)
        {
            
            try
            {

                ServerInfoProperties serverProps = null;
                MineStat stat = null;

                await Task.Run(async () => {
                    serverProps = online.GetFromDatabaseAsync<ServerInfoProperties>("ServerProperties", "serverInfo").Result;
                    stat = new MineStat(serverProps.ipAddress, ushort.Parse(serverProps.port));
                });

                GetServerStatus(stat);
                GetOnlinePlayers(stat);

            }
            catch
            {
                ServerHealthText = "Unknown";
                ServerHealthColor = "#9e9e9e";

                ServerStatusText = "Unknown";
                ServerStatusColor = "#9e9e9e";
                return;
            }
         
        }

        async void GetServerStatus(MineStat ms)
        {

            switch (ms.ServerUp)
            {
                case false:
                    ServerStatusText = "Offline";
                    ServerStatusColor = "#f44336";
                    break;
                case true:
                    ServerStatusColor = "#4caf50";
                    ServerStatusText = "Online";
                    break;
            }

            var ping = ms.Latency;

            if (ms.ServerUp == false)
            { 
                ServerHealthText = "Unknown";
                ServerHealthColor = "#9e9e9e";
            }
            else if (ping <= 100)
            {
                ServerHealthText = "Fast";
                ServerHealthColor = "#4caf50";
            }
            else if (ping <= 200)
            {
                ServerHealthText = "Fair";
                ServerHealthColor = "#ffeb3b";
            }
            else if (ping <= 250)
            {
                ServerHealthText = "Slow";
                ServerHealthColor = "#ff9800";
            }
            else if (ping > 250)
            {
                ServerHealthText = "Laggy";
                ServerHealthColor = "#f44336";
            }
        }

        async void GetOnlinePlayers(MineStat ms)
        {

            PlayersOnlineText = "PLAYERS ONLINE: " + ms.CurrentPlayers + "/" + ms.MaximumPlayers;

            PlayersOnline.Clear();

            if (ms.CurrentPlayersInt == 0)
            {
                PlayersOnline.Add("Be the first one online!");
                return;
            }

            foreach (var player in ms.PlayerList)
            {
                PlayersOnline.Add(player);
            }
        }
    }
}
