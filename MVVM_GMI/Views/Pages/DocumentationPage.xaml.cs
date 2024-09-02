using CommunityToolkit.Mvvm.Messaging;
using Microsoft.Web.WebView2.Core;
using MVVM_GMI.Messages;
using MVVM_GMI.ViewModels.Pages;
using System.Windows.Data;
using Wpf.Ui.Controls;
using Microsoft.Web.WebView2.Wpf;
using System.Security.Policy;
using MVVM_GMI.Services;
using System.IO;

namespace MVVM_GMI.Views.Pages
{

    public partial class DocumentationPage : INavigableView<DocumentationViewModel>
    {
        public DocumentationViewModel ViewModel { get; }

        public DocumentationPage(DocumentationViewModel viewModel)
        {
            ViewModel = viewModel;
            DataContext = this;

            InitializeComponent();

            WeakReferenceMessenger.Default.Register<WebViewMessage>(this, (r, m) =>
            {
                if (m.OpenOrClose)
                {
                    CreateWebviewWithSource(new Uri(m.Source));
                }
                else
                {
                    DestroyWebView();
                }; 
            });
        }


        public async Task CreateWebviewWithSource(Uri Source)
        {

            try
            {
                WebView2 webview = new WebView2()
                {
                    
                    ZoomFactor = 0.9,
                    Name = "Docs_WebView",
                    DefaultBackgroundColor = System.Drawing.Color.Transparent

                };

                var userDataFolder = Path.Combine(ConfigurationService.Instance.fromLauncher.LauncherPath, "webViewCache");
                Directory.CreateDirectory(userDataFolder);

                var env = await CoreWebView2Environment.CreateAsync(null, userDataFolder);

                



                RegisterName(webview.Name, webview);
                this.WebViewContainer.Children.Add(webview);

                webview.CoreWebView2InitializationCompleted += (sender, e) =>
                {
                    if (e.IsSuccess)
                    {
                        var coreWebView2 = webview.CoreWebView2;
                        // Configure CoreWebView2 settings
                        coreWebView2.Settings.IsStatusBarEnabled = false;
                        coreWebView2.Settings.AreDefaultContextMenusEnabled = false;
                        coreWebView2.Settings.AreBrowserAcceleratorKeysEnabled = false;
                        coreWebView2.Settings.AreDevToolsEnabled = false;
                        


                    }
                };

                await webview.EnsureCoreWebView2Async(env);


                webview.Source = new UriBuilder(Source).Uri;

            }
            catch
            {
                CenterText.Text = "An Error occurred, you have to restart the launcher to fix it.";
            }


        }   

        public async void DestroyWebView()
        {

            try
            {
                var webView = this.WebViewContainer.FindName("Docs_WebView") as WebView2;
                var url = await webView.ExecuteScriptAsync("document.location.href;");
                this.ViewModel.ChangeLastVisitedURL(url.Replace("\"", ""));
                webView.UnregisterName(webView.Name);
                this.WebViewContainer.Children.Remove(webView);
            }
            catch
            {
                CenterText.Text = "An Error occurred, you have to restart the launcher to fix it.";

            }


        }
    }
}
