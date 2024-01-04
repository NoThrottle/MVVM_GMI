using Google.Cloud.Firestore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using AutoUpdaterDotNET;
using System.Threading.Tasks;
using online = MVVM_GMI.Helpers.OnlineRequest;
using Wpf.Ui;

namespace MVVM_GMI.Services
{
    internal class UpdateService
    {

        LauncherVersionProperties props;


        public UpdateService()
        {

            GetInfoAsync();

        }

        async Task GetInfoAsync()
        {
            props = await online.GetFromDatabaseAsync<LauncherVersionProperties>("ServerProperties", "launcherVersion");
        }

        /// <summary>
        /// Returns true if the client needs to update. False if there is a minor update OR no update.
        /// </summary>
        /// <returns></returns>
        internal bool CheckForUpdates()
        {
            if (props.latest != ILauncherProperties.LauncherVersion)
            {
                if (ILauncherProperties.LauncherVersion < props.minimum)
                {

                    return true;
                }
            }
            return false;
        }

        internal bool StartUpdate()
        {

            UpdateInfoEventArgs args = new UpdateInfoEventArgs()
            {

                DownloadURL = props.url
            
            };

            if (AutoUpdater.DownloadUpdate(args))
            {
                return true;
            }

            return false;
        }
    }


    [FirestoreData]
    internal class LauncherVersionProperties
    {

        [FirestoreProperty]
        public int latest { get; set; }

        [FirestoreProperty]
        public int minimum { get; set; }

        [FirestoreProperty]
        public string url { get; set; }

    }

}
