// This Source Code Form is subject to the terms of the MIT License.
// If a copy of the MIT was not distributed with this file, You can obtain one at https://opensource.org/licenses/MIT.
// Copyright (C) Leszek Pomianowski and WPF UI Contributors.
// All Rights Reserved.

using CmlLib.Utils;
using Config.Net;
using MVVM_GMI.Services;

namespace MVVM_GMI.ViewModels.Pages
{
    public partial class DashboardViewModel : ObservableObject
    {
        [ObservableProperty]
        private int _counter = 0;

        [ObservableProperty]
        private string _whatsNewText = "";
        
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

        public DashboardViewModel()
        {
            WhatsNewText = Task.Run(async () => {

                Changelogs x = await Changelogs.GetChangelogs();
                var y = await x.GetChangelogHtml("1.20.4");
                return y;
            
            
            }).Result;
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
            new SettingsStartupService();

            ILauncherSettings s = new ConfigurationBuilder<ILauncherSettings>().UseAppConfig().Build();
            IMinecraftSettings r = new ConfigurationBuilder<IMinecraftSettings>().UseAppConfig().Build();

            Locked = true;
            var x = r;
            var y = s;


            var z = new MinecraftService(x, y);

            z.ProgressUpdated += (s) => UpdateUI(s);
            z.QuickLaunch();

            z.TaskCompleted += () => 
            {

                PlayButtonState(4);
                Locked = false;

            };


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
