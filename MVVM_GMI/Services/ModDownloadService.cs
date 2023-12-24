using MVVM_GMI.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using online = MVVM_GMI.Helpers.OnlineRequest;
using from = MVVM_GMI.Services.ConfigurationService;
using System.IO;
using static Grpc.Core.Metadata;

namespace MVVM_GMI.Services
{
    internal class ModDownloadService
    {

        public event Action<bool>? TaskCompleted;
        public event Action<MinecraftLoadingUpdate>? ProgressUpdated;

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

        

        async Task DownloadModsAsync()
        {
            var x = await online.GetAllFromDatabaseAsync<ModEntry>("Mods");
            //halfLength + (length % 2)
            var x1 = x.GetRange(0, (x.Count-1)/2 + (x.Count%2));
            var x2 = x.GetRange((x.Count - 1) / 2 + (x.Count % 2), x.Count-1);

            string entryName = "";
            int max = x.Count();
            int prog = 0;

            _ = Task.Run(async () => 
            {
                foreach (var entry in x1)
                {
                    entryName = entry.Name;
                    var o = Path.Combine(from.Launcher.MinecraftPath, "mods", string.Join("_", entry.Name) + "_'-" + string.Join("", entry.DatePublished) + ".jar");
                    await online.DownloadFileAsync(entry.DownloadURL, o);
                    prog++;

                }
            });

            _ = Task.Run(async () =>
            {
                foreach (var entry in x2)
                {

                    entryName = entry.Name;
                    var o = Path.Combine(from.Launcher.MinecraftPath, "mods", string.Join("_", entry.Name) + "_'-" + string.Join("", entry.DatePublished) + ".jar");
                    await online.DownloadFileAsync(entry.DownloadURL, o);
                    prog++;

                }
            });
            while (true)
            {
                UpdateStatus("Downloading", 0, false, x.Count(), prog, "Downloading Mods: ", prog + "/" + x.Count() + " - Mod: " + entryName);
            }
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

    }
}
