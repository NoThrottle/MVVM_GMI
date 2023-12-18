// This Source Code Form is subject to the terms of the MIT License.
// If a copy of the MIT was not distributed with this file, You can obtain one at https://opensource.org/licenses/MIT.
// Copyright (C) Leszek Pomianowski and WPF UI Contributors.
// All Rights Reserved.

namespace MVVM_GMI.ViewModels.Pages
{
    public partial class DashboardViewModel : ObservableObject
    {
        [ObservableProperty]
        private int _counter = 0;

        [ObservableProperty]
        private string _whatsNewText = "New Things\n Old things\n More new things\nsome bugfixes\n uhuh\noooooo\nnoooooooooooo";

        [RelayCommand]
        private void OnCounterIncrement()
        {
            Counter++;
        }
    }
}
