using CmlLib.Utils;
using Google.Rpc;
using Microsoft.Extensions.DependencyInjection;
using MVVM_GMI.Helpers;
using MVVM_GMI.Services;
using MVVM_GMI.Views.Pages;
using MVVM_GMI.Views.Windows;
using Wpf.Ui;
using System.Net.Http.Headers;
using Newtonsoft.Json.Linq;
using System.Collections.ObjectModel;
using System.Windows.Threading;

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
                playersOnlineTimer.Tick += GetOnlinePlayersAsync;
                playersOnlineTimer.Tick += GetServerStatus;
                OnlineStatusLoadingVisibility = "Collapsed";
            }
            catch
            {
                OnlineStatusLoadingVisibility = "Visible";
            }


            playersOnlineTimer.Start();
        }

        async void GetServerStatus(object sender, EventArgs e)
        {
            var Auth = new List<String[]>();
            Auth.Add(["Authorization", "Bearer ptlc_Qx5oOPvfzxi6gtZZAhjBs4JDj29rpIfFAcanoR80Q45"]);

            string x = null;

            try
            {
                x = await OnlineRequest.GetJsonAsync("https://control.sparkedhost.us/api/client/servers/a7974b2a-36cc-4fb3-a801-9dccfac0e6f1/resources", Auth);
            }
            catch
            {

            }

            if (x == null) { return; }

            JObject obj = JObject.Parse(x);
            var status = obj["attributes"]["current_state"].ToString() ?? null;
            var health = obj["attributes"]["resources"]["cpu_absolute"].ToString();

            switch (status)
            {
                case "offline":
                    ServerStatusText = "Offline";
                    ServerStatusColor = "#f44336";
                    break;
                case "starting":
                    ServerStatusText = "Starting";
                    ServerStatusColor = "#ffeb3b";
                    break;
                case "running":
                    ServerStatusColor = "#4caf50";
                    ServerStatusText = "Online";
                    break;
                case "stopping":
                    ServerStatusColor = "#ffeb3b";
                    ServerStatusText = "Stopping";
                    break;
                default:
                    ServerStatusText = "Unknown";
                    ServerStatusColor = "#9e9e9e";
                    break;
            }

            var h = float.Parse(health);

            if (status == null || status == "offline")
            { 
                ServerHealthText = "Unknown";
                ServerHealthColor = "#9e9e9e";
            }
            else if (h <= 90)
            {
                ServerHealthText = "Healthy";
                ServerHealthColor = "#4caf50";
            }
            else if (h <= 99)
            {
                ServerHealthText = "Fair";
                ServerHealthColor = "#ffeb3b";
            }
            else if (h <= 150)
            {
                ServerHealthText = "Caution";
                ServerHealthColor = "#ff9800";
            }
            else if (h > 150)
            {
                ServerHealthText = "Unhealthy";
                ServerHealthColor = "#f44336";
            }
        }

        async void GetOnlinePlayersAsync(object sender, EventArgs e)
        {
            var Auth = new List<String[]>();
            Auth.Add(["Authorization", "Bearer ptlc_Qx5oOPvfzxi6gtZZAhjBs4JDj29rpIfFAcanoR80Q45"]);

            string x = null;

            try
            {
                x = await OnlineRequest.GetJsonAsync("https://control.sparkedhost.us/api/client/servers/a7974b2a-36cc-4fb3-a801-9dccfac0e6f1/minecraft-players", Auth);
            }
            catch
            {

            }
            
            if (x == null) { return; }

            JObject obj = JObject.Parse(x);
            JArray? players = (JArray?)obj["data"]["online_players"];

            int count = (int)obj["data"]["online_player_count"];
            int max = (int)obj["data"]["max_players"];

            PlayersOnlineText = "PLAYERS ONLINE: " + count + "/" + max;

            PlayersOnline.Clear();

            foreach(JObject b in players)
            {
                PlayersOnline.Add(b["name"].ToString());
            }
        }
    }
}
