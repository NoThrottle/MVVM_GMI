using Microsoft.Extensions.DependencyInjection;
using MVVM_GMI.Helpers;
using MVVM_GMI.Services;
using MVVM_GMI.Services.Database;
using MVVM_GMI.Views.Windows;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Formats.Asn1;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using Windows.Security.Authentication.OnlineId;
using Wpf.Ui;
using Wpf.Ui.Controls;

namespace MVVM_GMI.ViewModels.Pages
{
    public partial class ProfileViewModel : ObservableObject, INavigationAware
    {
        IContentDialogService _dialogService;
        private bool _isInitialized = false;

        [ObservableProperty]
        private int _counter = 0;

        [ObservableProperty]
        private string _username = MVVM_GMI.Services.UserProfileService.AuthorizedUsername ?? string.Empty;

        public void OnNavigatedTo()
        {
            if (!_isInitialized)
            {
                InitializeViewModel();
            }

            UpdateInvitedList();
        }

        public void OnNavigatedFrom() { }

        IServiceProvider _serviceProvider;
        public ProfileViewModel(IServiceProvider serviceProvider, IContentDialogService contentDialogService)
        {
            _serviceProvider = serviceProvider;
            _dialogService = contentDialogService;
        }


        

        private void InitializeViewModel()
        {

            _isInitialized = true;
        }

        [ObservableProperty]
        string _inviteEnabled = "True";


        [ObservableProperty]
        ObservableCollection<string> _invited = new ObservableCollection<string>();

        void UpdateInvitedList()
        {

            Task.Run(() => { updateAsync(); });

            async Task updateAsync()
            {
                List<Invited> t = new List<Invited>();

                try
                {

                    t.AddRange(await OnlineRequest.GetAllFromDatabaseAsync<Invited>("UserData", UserProfileService.AuthorizedUsername, "Invited"));

                }
                catch
                {
                    System.Windows.Forms.MessageBox.Show("Cannot Fetch");
                }

                if (t.Count == 0)
                {
                    return;
                }

                Application.Current.Dispatcher.Invoke(() => 
                {
                    Invited.Clear();

                    foreach (var x in t)
                    {
                        Invited.Add(x.Username);
                    }

                });


            }

        }



        [RelayCommand]
        void CreateInviteAsync()
        {

            Task.Run(() =>
            {
 
                InviteEnabled = "False";
                bool d = action().Result;
                InviteEnabled = "True";
            });

            async Task<bool> action()
            {
                string[]? code;

                try
                {
                    code = await new API().CreateInviteCode();
                }
                catch
                {
                    await Application.Current.Dispatcher.Invoke(async () =>
                    {
                        await ShowDialogAsync("Error", "Unable to create a new invite code. Check your internet connection", "", "", "Okay");
                    });

                    return false;
                }
  
                if (code == null)
                {
                    await Application.Current.Dispatcher.Invoke(async () =>
                    {
                        await ShowDialogAsync("Error", "Unable to create a new invite code. Check your internet connection", "", "", "Okay");
                    });
                    return false;
                }

                await Application.Current.Dispatcher.Invoke(async () =>
                {

                    var dad = await ShowDialogAsync("New Code", "Here is an invite code. This can only be used once and lasts for 24 hours.\n\nCode: " + code[0].ToUpper() + "\nExpiration: " + code[1] + "\n\nMake sure to only invite people you trust.", "Copy", "", "Close");

                    if (dad == ContentDialogResult.Primary)
                    {
                        Clipboard.SetText(code[0]);
                    }

                });

                return true ;
            }

        }

        [RelayCommand]
        void LogOut()
        {

            new API().LogOut();
            Helpers.Extensions.RestartApplication();

        }



        async Task<ContentDialogResult> ShowDialogAsync(string Title, string Content, string PrimaryButtonText, string SecondaryButtonText, string CloseButtonText)
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

    }

    
}
