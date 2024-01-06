using Google.Cloud.Firestore;
using MVVM_GMI.Helpers;
using MVVM_GMI.Models;
using System.IO;
using @from = MVVM_GMI.Services.ConfigurationService;
using @online = MVVM_GMI.Helpers.OnlineRequest;

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

        async Task ProcessActionsAsync()
        {
            if (from.Instance.fromLauncher.DidStarterAction)
            {
                return;
            }

            var t = await online.GetFromDatabaseAsync<JSONActionDocument>("JsonActions","InitialSetup");
            var x = new JSONActions();
            await x.DoActionAsync(t.JSONString);

            from.Instance.fromLauncher.DidStarterAction = true;
        }


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

            if (x == 0)
            {
                await VerifyInstallationAsync(true);
                from.Instance.fromLauncher.ModUpdateIndex = z;

            }

            await VerifyInstallationAsync(false);
            from.Instance.fromLauncher.ModUpdateIndex = z;            

            try
            {
                await ProcessActionsAsync();
            }
            catch (Exception e)
            {
                MessageBox.Show(e.Message + e.StackTrace + e.Source);
                //Internet or Rate Limit failure
            }

            return true;
            //TaskCompleted?.Invoke(true);
            
        }


        async Task<bool> DownloadModsAsync(List<ModEntry> mods)
        {
            Directory.CreateDirectory(Path.Combine(from.Instance.fromLauncher.MinecraftPath, "mods"));

            int MaxQueue = from.Instance.fromLauncher.SimultaneousDownloads;
            int inQueue = 0;

            string entryName = "";
            int max = mods.Count();
            int prog = 0;

            _ = Task.Run(() => 
            {
                foreach (var entry in mods)
                {

                    while(inQueue > MaxQueue)
                    {
                        Thread.Sleep(250);
                    }

                    Task.Run(async () => 
                    {

                        inQueue += 1;

                        entryName = entry.Name;
                        var o = Path.Combine(from.Instance.fromLauncher.MinecraftPath, "mods", entry.projectID + entry.versionID + ".jar");
                        try 
                        {
                            await online.DownloadFileAsync(entry.DownloadURL, o);
                        }
                        catch
                        {

                        }
                        prog++;

                        inQueue--;

                    });

                }

            });

            while (prog != max)
            {
                Application.Current.Dispatcher.Invoke((Action) delegate {

                    UpdateStatus("Downloading", 0, false, mods.Count(), prog, "Downloading Mods: ", prog + "/" + mods.Count() + " - Mod: " + entryName);

                });
                
                Thread.Sleep(100);
            }

            await VerifyInstallationAsync(false);
            return true;
        }

        async Task VerifyInstallationAsync(bool Reset)
        {

            var modsPath = Path.Combine(from.Instance.fromLauncher.MinecraftPath, "mods");
            var mods = await online.GetAllFromDatabaseAsync<ModEntry>("Mods", "ClientSide", true);
            var modsCopy = new List<ModEntry>(mods);


            if (Reset)
            {

                try
                {

                    Directory.Delete(modsPath, true);
                }
                catch
                {

                }

                await DownloadModsAsync(mods);
                return;

            }

            if (!Directory.Exists(modsPath))
            {
                await DownloadModsAsync(mods);
                return;

            }

            DirectoryInfo d = new DirectoryInfo(modsPath);
            FileInfo[] files = d.GetFiles();
            var filesCopy = files.ToList();
            int prog = 0;

            foreach (var mod in mods)
            {
                Application.Current.Dispatcher.Invoke((Action)delegate {
                    UpdateStatus("Verifying Installation", 0, false, mods.Count(), prog, "Verifying Installation: ", prog + "/" + mods.Count() + " - Mod: " + mod.Name);
                });

                foreach (var file in files)
                {
                    var filename = mod.projectID + mod.versionID + ".jar";
                    if (file.Name == filename && file.Length == mod.Size)
                    {
                        modsCopy.Remove(mod);
                        filesCopy.Remove(file);
                        break;
                    }
                }

                prog++;
            }

            foreach (var file in filesCopy)
            {
                try
                {
                    File.Delete(file.FullName);
                }
                catch
                {
                    MessageBox.Show("Unable to delete: " + file.FullName, "Error");
                }
            }

            if (modsCopy.Count == 0)
            {
                return;
            }

            await DownloadModsAsync(modsCopy);

            return;
        }

        void UpdateStatus(string textProgress, int intProgress, bool isIntermediate, int maxProgress, int currentProgress, string process, string processDescription)
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

        void UpdateStatus(MinecraftLoadingUpdate update)
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
