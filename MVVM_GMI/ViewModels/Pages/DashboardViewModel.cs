// This Source Code Form is subject to the terms of the MIT License.
// If a copy of the MIT was not distributed with this file, You can obtain one at https://opensource.org/licenses/MIT.
// Copyright (C) Leszek Pomianowski and WPF UI Contributors.
// All Rights Reserved.

using MVVM_GMI.Services;

namespace MVVM_GMI.ViewModels.Pages
{
    public partial class DashboardViewModel : ObservableObject
    {
        [ObservableProperty]
        private int _counter = 0;

        [ObservableProperty]
        private string _whatsNewText = "New Things\n Old things\n More new things\nsome bugfixes\n uhuh\noooooo\nnoooooooooooo";

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



        [RelayCommand]
        private void OnCounterIncrement()
        {
            Counter++;
        }


        int x = 0;
        [RelayCommand]
        void PressedPlay()
        {
            PlayButtonState(x);
            x++;

            MinecraftService mc = new MinecraftService();
            
        }

        void PlayButtonState(int state)
        {
            switch (state)
            {
                case 0: // Default
                    PlayButtonText = "PLAY ";
                    PlayButtonColor = "Orange";
                    PlayButtonHoverColor = "Yellow";
                    PlayButtonPressedColor = "White";
                    PlayButtonPressedTextColor = "Gray";
                    break;
                case 1: // Loading
                    PlayButtonText = "LOADING ";
                    PlayButtonColor = "Gray";
                    PlayButtonHoverColor = "Gray";
                    PlayButtonPressedColor = "Gray";
                    PlayButtonPressedTextColor = "White";
                    break;
                case 2: // Playing
                    PlayButtonText = "PLAYING ";
                    PlayButtonColor = "Green";
                    PlayButtonHoverColor = "Green";
                    PlayButtonPressedColor = "Green";
                    PlayButtonPressedTextColor = "White";
                    break;
                case 3: // Error
                    PlayButtonText = "ERROR ";
                    PlayButtonColor = "Red";
                    PlayButtonHoverColor = "Red";
                    PlayButtonPressedColor = "Red";
                    PlayButtonPressedTextColor = "White";
                    break;
                case 4: // No Connection
                    PlayButtonText = "CONNECTING ";
                    PlayButtonColor = "Blue";
                    PlayButtonHoverColor = "Blue";
                    PlayButtonPressedColor = "Blue";
                    PlayButtonPressedTextColor = "White";
                    break;

            }

        }
    }
}
