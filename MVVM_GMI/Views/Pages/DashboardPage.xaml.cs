// This Source Code Form is subject to the terms of the MIT License.
// If a copy of the MIT was not distributed with this file, You can obtain one at https://opensource.org/licenses/MIT.
// Copyright (C) Leszek Pomianowski and WPF UI Contributors.
// All Rights Reserved.

using MVVM_GMI.ViewModels.Pages;
using System.Windows.Controls;
using Wpf.Ui;
using Wpf.Ui.Controls;

namespace MVVM_GMI.Views.Pages
{
    public partial class DashboardPage : INavigableView<DashboardViewModel>
    {

        private readonly IContentDialogService _dialogControl;

        public DashboardViewModel ViewModel { get; }

        public DashboardPage(DashboardViewModel viewModel, IContentDialogService contentDialogService)
        {
            ViewModel = viewModel;
            DataContext = this;

            _dialogControl = contentDialogService;

            InitializeComponent();


            //var content = new ContentDialog(_dialogControl.GetContentPresenter());
            //content.Content = "hi";
            //content.CloseButtonText = "OK";
            //content.ShowAsync();
        }
    }
}
