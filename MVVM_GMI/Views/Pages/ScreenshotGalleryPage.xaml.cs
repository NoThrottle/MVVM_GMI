using CommunityToolkit.Mvvm.Messaging;
using Microsoft.Web.WebView2.Core;
using Microsoft.Web.WebView2.Wpf;
using MVVM_GMI.Messages;
using MVVM_GMI.Services;
using MVVM_GMI.ViewModels.Pages;
using System.Diagnostics;
using System.IO;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Input;
using System.Windows.Media.Imaging;
using Wpf.Ui.Controls;

namespace MVVM_GMI.Views.Pages
{

    public partial class ScreenshotGalleryPage : INavigableView<ScreenshotGalleryViewModel>
    {
        public ScreenshotGalleryViewModel ViewModel { get; }

        public ScreenshotGalleryPage(ScreenshotGalleryViewModel viewModel)
        {
            ViewModel = viewModel;
            DataContext = this;

            InitializeComponent();
           

        }

        private void MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (e.ClickCount == 2)
            {
                var path = (sender as Grid).Tag;
                Process.Start("explorer", "\"" + path + "\"");
            }
        }

    }
}
