﻿using CommunityToolkit.Mvvm.Messaging;
using Microsoft.Web.WebView2.Core;
using MVVM_GMI.Messages;
using MVVM_GMI.ViewModels.Pages;
using System.Windows.Data;
using Wpf.Ui.Controls;
using Microsoft.Web.WebView2.Wpf;

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

            try
            {
                WebView2 webview = new WebView2()
                {
                    Source = Source,
                    ZoomFactor = 0.9,
                    Name = "Docs_WebView",
                    DefaultBackgroundColor = System.Drawing.Color.Transparent

                };

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

            }
            catch
            {
                CenterText.Text = "An Error occurred, you have to restart the launcher to fix it.";
            }


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