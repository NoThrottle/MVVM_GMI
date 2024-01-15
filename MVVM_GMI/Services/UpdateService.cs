using Google.Cloud.Firestore;
using System.Diagnostics;
using System.IO;
using System.Net.Http;
using System.Reflection;
using System.Security.Cryptography;
using online = MVVM_GMI.Helpers.OnlineRequest;
using MVVM_GMI.Helpers;

namespace MVVM_GMI.Services
{
    public class UpdateService
    {
        #region Boiler Plate
        private static UpdateService instance;
        private static readonly object lockObject = new object();

        private UpdateService()
        {
            GetInfoAsync();
        }

        public static UpdateService Instance
        {
            get
            {
                if (instance == null)
                {
                    lock (lockObject)
                    {
                        if (instance == null)
                        {
                            instance = new UpdateService();
                        }
                    }
                }
                return instance;
            }
        }
        #endregion


        LauncherVersionProperties props;
        static readonly string mainPath = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
        static readonly string DLPath = Path.Combine(mainPath, "zip", "update.zip");
        static readonly string ExtractPath = Path.Combine(mainPath, "extract");



        async Task GetInfoAsync()
        {
            try
            {
                props = await online.GetFromDatabaseAsync<LauncherVersionProperties>("ServerProperties", "launcherVersion");
            }
            catch
            {
                Debug.WriteLine("Unable to get Info");
            }
        }

        /// <summary>
        /// States: <br/>
        /// 1 - Unable to check for updates <br/>
        /// 2 - No Updates Available <br/>
        /// 3 - Non-Required Update Available <br/>
        /// 4 - Required Update Available <br/>
        /// </summary>
        internal async Task<int> CheckForUpdatesAsync()
        {

            await GetInfoAsync();

            if (props == null)
            {
                return 1;
            }

            if (props.latest != LauncherProperties.LauncherVersion)
            {
                if (LauncherProperties.LauncherVersion < props.minimum)
                {
                    return 4;
                }
                return 3;
            }
            return 2;
        }


        /// <summary>
        /// States: <br/>
        /// -3 - Retrying Download <br/>
        /// -2 - Unable to Update <br/>
        /// -1 - Doing Nothing <br/>
        /// 0 - Downloading Updates <br/>
        /// 1 - Verifying Download <br/>
        /// 2 - Extracting Files <br/>
        /// 3 - Moving Files <br/>
        /// 4 - Verifying Files <br/>
        /// 5 - Ready to Restart <br/>
        /// </summary>
        public event Action<int>? UpdateState;

        public event Action<int>? Progress;

        internal async Task<bool> StartUpdateAsync()
        {


            try
            {
                Directory.Delete(Path.Combine(mainPath, "zip"), true);
            }
            catch
            {

            }

            try
            {
                Directory.Delete(ExtractPath, true);
            }
            catch
            {

            }

            Directory.CreateDirectory(Path.Combine(mainPath, "zip"));
            Directory.CreateDirectory(ExtractPath);

            UpdateState.Invoke(0);
            await DownloadFile(props.url, DLPath);

            UpdateState.Invoke(1);
            var hash = DLPath.CalculateMD5();

            if (hash != props.hash)
            {
                UpdateState.Invoke(-3);
                Thread.Sleep(1000 * 3);
                return await StartUpdateAsync();
            }

            UpdateState.Invoke(2);
            System.IO.Compression.ZipFile.ExtractToDirectory(DLPath, ExtractPath, true);

            var files = Directory.GetFiles(mainPath);
            var newfiles = Directory.GetFiles(ExtractPath);

            int maxProgress = files.Length + newfiles.Length;
            int progress = 0;

            foreach(var file in files)
            {
                File.Move(file, file + ".bak");
                progress++;
                Progress.Invoke((int)Math.Floor((double)(progress / maxProgress * 100)));
            }

            foreach (var file in newfiles)
            {
                File.Move(file, Path.Combine(mainPath, Path.GetFileName(file)));
                progress++;
                Progress.Invoke((int)Math.Floor((double)(progress / maxProgress * 100)));
            }

            ConfigurationService.Instance.fromLauncher.JustUpdated = true;

            UpdateState.Invoke(5);
            return true;
        }

        public void VerifyAndDelete()
        {
            foreach (var file in Directory.GetFiles(mainPath))
            {
                if(Path.GetExtension(file) == ".bak")
                {
                    File.Delete(file);
                }
            }
        }

        public event Action<double>? DownloadSpeedInMB;

        async Task DownloadFile(string fileUrl, string destinationPath)
        {
            using (HttpClient client = new HttpClient())
            {
                using (HttpResponseMessage response = await client.GetAsync(fileUrl, HttpCompletionOption.ResponseHeadersRead))
                {
                    response.EnsureSuccessStatusCode();

                    using (Stream contentStream = await response.Content.ReadAsStreamAsync())
                    {
                        using (FileStream fileStream = File.Create(destinationPath))
                        {
                            var buffer = new byte[8192];
                            var stopwatch = Stopwatch.StartNew();
                            var totalBytesRead = 0L;
                            int bytesRead;

                            do
                            {
                                bytesRead = await contentStream.ReadAsync(buffer, 0, buffer.Length);
                                totalBytesRead += bytesRead;

                                if (bytesRead > 0)
                                {
                                    await fileStream.WriteAsync(buffer, 0, bytesRead);

                                    // Calculate download speed
                                    double speed = totalBytesRead / stopwatch.Elapsed.TotalSeconds;
                                    DownloadSpeedInMB.Invoke(speed / 1024 / 1024);
                                }
                            } while (bytesRead > 0);
                        }
                    }
                }
            }
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

        [FirestoreProperty]
        public string hash { get; set; }

    }

}
