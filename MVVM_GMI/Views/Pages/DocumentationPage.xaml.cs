using CommunityToolkit.Mvvm.Messaging;
using Microsoft.Web.WebView2.Core;
using Microsoft.Web.WebView2.Wpf;
using MVVM_GMI.Messages;
using MVVM_GMI.ViewModels.Pages;
using System.Windows.Data;
using Wpf.Ui.Controls;

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


        public void CreateWebviewWithSource(Uri Source)
        {

            WebView2 webview = new WebView2()
            {
                Source = Source,
                ZoomFactor = 0.9,
                Name = "Docs_WebView", 
       
            };

            RegisterName(webview.Name, webview);
            this.WebViewContainer.Children.Add(webview);
        }

        public async void DestroyWebView()
        {
            var webView = this.WebViewContainer.FindName("Docs_WebView") as WebView2;
            var url = await webView.ExecuteScriptAsync("document.location.href;");
            this.ViewModel.ChangeLastVisitedURL(url.Replace("\"",""));
            webView.UnregisterName(webView.Name);
            this.WebViewContainer.Children.Remove(webView);
        }
    }
}
