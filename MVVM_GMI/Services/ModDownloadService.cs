using Google.Cloud.Firestore;
using MVVM_GMI.Models;
using System.IO;
using from = MVVM_GMI.Services.ConfigurationService;
using online = MVVM_GMI.Helpers.OnlineRequest;
using static MVVM_GMI.Helpers.Extensions;

namespace MVVM_GMI.Services
{
    internal class ModDownloadService
    {

        public event Action<bool>? TaskCompleted;
        public event Action<MinecraftLoadingUpdate>? ProgressUpdated;

        #region Boiler plate

        private static ModDownloadService instance;
        private static readonly object lockObject = new object();

        public ModDownloadService()
        {
           
        }

        public static ModDownloadService Instance
        {
            get
            {
                if (instance == null)
                {
                    lock (lockObject)
                    {
                        if (instance == null)
                        {
                            instance = new ModDownloadService();
                        }
                    }
                }
                return instance;
            }
        }

        #endregion Boiler plate

        public async Task<bool> CheckInstalledModVersion()
        {
            UpdateStatus(new MinecraftLoadingUpdate() 
            { 
                IntProgress = 0,
                TextProgress = "Loading",
                Intermediate = true,
                Process = "Getting mod version",
                ProcessDescription = ""
                       
            });

            int x = from.Instance.fromLauncher.ModUpdateIndex;
            var y = await online.GetFromDatabaseAsync<ModsProperties>("ServerProperties","mods");
            int z = y.updateIndex;

            if (x != z)
            {
                var df = await DownloadModsAsync();
                from.Instance.fromLauncher.ModUpdateIndex = z;
            }

            return true;
            //TaskCompleted?.Invoke(true);
            
        }


        async Task<bool> DownloadModsAsync()
        {
            try
            {
                Directory.Delete(Path.Combine(from.Instance.fromLauncher.MinecraftPath, "mods"),true);
            }
            catch
            {

            }

            Directory.CreateDirectory(Path.Combine(from.Instance.fromLauncher.MinecraftPath, "mods"));

            var x = await online.GetAllFromDatabaseAsync<ModEntry>("Mods","ClientSide",true);

            int MaxQueue = 10;
            int inQueue = 0;

            string entryName = "";
            int max = x.Count()-1;
            int prog = 0;

            _ = Task.Run(async () => 
            {
                foreach (var entry in x)
                {

                    while(inQueue > MaxQueue)
                    {
                        Thread.Sleep(250);
                    }

                    Task.Run(async () => 
                    {

                        inQueue += 1;

                        entryName = entry.Name;
                        var o = Path.Combine(from.Instance.fromLauncher.MinecraftPath, "mods", entry.Name + entry.projectID + entry.versionID + ".jar");
                        await online.DownloadFileAsync(entry.DownloadURL, o);
                        prog++;

                        inQueue--;

                    });

                }

            });

            while (prog != max)
            {
                Application.Current.Dispatcher.Invoke((Action)async delegate {

                    UpdateStatus("Downloading", 0, false, x.Count(), prog, "Downloading Mods: ", prog + "/" + x.Count() + " - Mod: " + entryName);

                });
                
                Thread.Sleep(100);
            }

            return true;
        }


        public void UpdateStatus(string textProgress, int intProgress, bool isIntermediate, int maxProgress, int currentProgress, string process, string processDescription)
        {
            MinecraftLoadingUpdate x = new MinecraftLoadingUpdate();
            x.TextProgress = textProgress;
            x.IntProgress = intProgress;
            x.Intermediate = isIntermediate;
            x.MaxProgress = maxProgress;
            x.CurrentProgress = currentProgress;
            x.Process = process;
            x.ProcessDescription = processDescription;


            ProgressUpdated?.Invoke(x);
        }

        public void UpdateStatus(MinecraftLoadingUpdate update)
        {
            ProgressUpdated?.Invoke(update);
        }

        [FirestoreData]
        class ModsProperties
        {

            [FirestoreProperty]
            public int updateIndex { get; set; }

            [FirestoreProperty]
            public bool forceUpdate { get; set; }
        }

    }
}
