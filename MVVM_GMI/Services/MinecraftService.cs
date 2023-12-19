using CmlLib.Core;
using CmlLib.Core.Auth;
using CmlLib.Core.Installer.FabricMC;
using System.Diagnostics;

namespace MVVM_GMI.Services
{
    public class MinecraftService
    {

        private readonly ILauncherSettings _launcherSettings;
        private readonly IMinecraftSettings _minecraftSettings;

        public MinecraftService(
            IMinecraftSettings minecraftSettings,
            ILauncherSettings launcherSettings)
        {
            _launcherSettings = launcherSettings;
            _minecraftSettings = minecraftSettings;
        }

        //----------------//


        public event Action? TaskCompleted;
        public event Action<MinecraftLoadingUpdate>? ProgressUpdated;

        private bool MinecraftRunning = false;


        public void QuickLaunch()
        {

            if (MinecraftRunning)
            {
                Console.WriteLine("Minecraft is already running");
                return;
            }

            UpdateStatus(new MinecraftLoadingUpdate()
            {

                TextProgress = "Loading",
                IntProgress = 0,
                Intermediate = true,
                Process = "Initializing",
                ProcessDescription = "Checking Minecraft Installation"

            });

            var launcher = new CMLauncher(_launcherSettings.MinecraftPath);
            launcher.GameFileCheckers.JavaFileChecker.CheckHash = false;
            launcher.GameFileCheckers.LibraryFileChecker.CheckHash = false;
            launcher.GameFileCheckers.LogFileChecker.CheckHash = false;
            launcher.GameFileCheckers.ClientFileChecker.CheckHash = false;
            launcher.GameFileCheckers.AssetFileChecker.CheckHash = false;

            InitializeAsync(launcher);
        }

        private void InitializeAsync(CMLauncher launcher)
        {
            System.Net.ServicePointManager.DefaultConnectionLimit = 256;

            launcher.FileChanged += (e) => 
            { 
            
                UpdateStatus(new MinecraftLoadingUpdate()
                {

                    TextProgress = "Loading",
                    IntProgress = 0,
                    Intermediate = false,
                    CurrentProgress = e.ProgressedFileCount,
                    MaxProgress = e.TotalFileCount,
                    Process = "Initializing Minecraft: ",
                    ProcessDescription = String.Format("[{0}] {1} - {2}/{3}", e.FileKind.ToString(), e.FileName, e.ProgressedFileCount, e.TotalFileCount)

                });

                if (e.FileName == "release")
                {
                    UpdateStatus("Loading",0,true,0,0,"","");
                }
            
            };

            Process process = null;

            process = launcher.CreateProcess("fabric-loader-" + "0.14.21" + "-" + "1.19.4", new MLaunchOption
            {

                //ServerIp = serverAddress,

                MaximumRamMb = _minecraftSettings.MaxRamAllocation,
                MinimumRamMb = _minecraftSettings.MinRamAllocation,

                ScreenWidth = _minecraftSettings.StartingWidth,
                ScreenHeight = _minecraftSettings.StartingHeight,
                FullScreen = _minecraftSettings.StartFullscreen,

                VersionType = "HighSkyMC",
                GameLauncherName = "HighSkyMC",
                GameLauncherVersion = "2",

                Session = MSession.CreateOfflineSession("Plenty"),//MAKE SURE TO ALLOW USERNAMES WHEN PROFILING IS ENABLED

            }, checkAndDownload: false); ;

            process.StartInfo.RedirectStandardError = true;
            process.StartInfo.RedirectStandardOutput = true;
            process.EnableRaisingEvents = true;
            process.ErrorDataReceived += (s, e) => Console.WriteLine(e.Data);


            UpdateStatus("Loading", 0, true, 0, 0, "Initializing Minecraft: ", "Starting Minecraft Process");

            bool check = true;

            Console.WriteLine(process.StartInfo.FileName);
            Console.WriteLine(process.StartInfo.Arguments);
            var processUtil = new CmlLib.Utils.ProcessUtil(process);

            processUtil.OutputReceived += (s, e) => 
            {

                Console.WriteLine(e);

                if (check && e != null && e.Contains(@"Connecting to"))
                {
                    //Minimize but not yet implemented
                    check = false;
                    
                }

                if (e != null && e.Contains(@"Game Crashed! Crash report saved to:"))
                {
                    UpdateStatus(new MinecraftLoadingUpdate()
                    {
                        TextProgress = "Error",
                        IntProgress = 3,
                    });
                }
            };

            processUtil.StartWithEvents();
            MinecraftRunning = true;

            UpdateStatus(new MinecraftLoadingUpdate()
            {
                TextProgress = "Running",
                IntProgress = 1,
            });

            process.Exited += (s, a) => SetDefaultState();
            processUtil.Exited += (sender, args) => SetDefaultState();
            processUtil.Process.WaitForExit();

            void SetDefaultState()
            {
                UpdateStatus(new MinecraftLoadingUpdate()
                {
                    TextProgress = "Idle",
                    IntProgress = 4,
                });

                MinecraftRunning = false;
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


        //------OLD-------//

        public async Task CheckEverything()
        {

            //UpdateStatus("Initializing", "Checking Minecraft Installation", 0, 100);

            Console.WriteLine("Check Everything");

            //Application.Current.Dispatcher.Invoke(new Action(() => {
            //    ProgressBar_Large.Visibility = Visibility.Visible;
            //    ProgressBar_Large.IsIndeterminate = true;
            //    Button_Join_States("Loading");
            //}));

            //var launcher = new CMLauncher(path);
            var install = true;

            //UpdateStatus("Initializing", "Checking for Fabric", 0, 100);


            //var versions = await launcher.GetAllVersionsAsync();
            //foreach (var v in versions)
            //{
            //    if (v.Name == "fabric-loader-" + FabricLoaderVersion + "-" + MinecraftVersion)
            //    {
            //        Console.WriteLine("Fabric is installed, continuing.");

            //        install = false;

            //        CheckMods();
            //        InitializeAsync(launcher);

            //        Console.WriteLine(4);

            //        break;
            //    }
            //}

            if (install)
            {
                Console.WriteLine("Fabric is not installed, will be installed first.");
                await InstallFabric();
                await CheckEverything();
            }

            Console.WriteLine("Checked Everything");

            //UpdateStatus("Initializing", "Done Checking", 0, 100);

        }

        public async Task InstallFabric()
        {

            Console.WriteLine("Installing Fabric");

            //var launcher = new CMLauncher(path);

            //launcher.FileChanged += (e) =>
            //{

            //    UpdateStatus(
            //        "DL_Fabric",
            //        String.Format("[{0}] {1} - {2}/{3}", e.FileKind.ToString(), e.FileName, e.ProgressedFileCount, e.TotalFileCount),
            //        e.ProgressedFileCount,
            //        e.TotalFileCount);

            //};

            //Application.Current.Dispatcher.Invoke(new Action(() => {
            //    SetUpdateText("", "");
            //    ProgressBar_Large.IsIndeterminate = true;
            //}));

            // initialize fabric version loader
            var fabricVersionLoader = new FabricVersionLoader();
            var fabricVersions = await fabricVersionLoader.GetVersionMetadatasAsync();

            // install
            //var fabric = fabricVersions.GetVersionMetadata("fabric-loader-" + FabricLoaderVersion + "-" + MinecraftVersion);
            //await fabric.SaveAsync(path);

        }

        public void CheckMods()
        {



            //var x = new EditMods()
            //{

            //    ModListSheetsName = ModListSheetsName,
            //    ModListRange = ModListRange,
            //    GMIType = GMIType,
            //    defaultUserAgent = defaultUserAgent,
            //    accountableUserAgent = accountableUserAgent,
            //    modsDir = GetClientPaths.mods,
            //    infoDir = GetClientPaths.gmi,
            //    SheetID = SheetID,
            //    APIKey = APIKey,
            //    cfKey = cfKey

            //};

            //x.StatusChanged += (e, s) =>
            //{

            //    Application.Current.Dispatcher.Invoke(new Action(() => {
            //        ProgressBar_Large.IsIndeterminate = s.StatusIntermediate;
            //        ProgressBar_Large.Minimum = 0;
            //        ProgressBar_Large.Value = s.StatusCurrent;
            //        ProgressBar_Large.Maximum = s.StatusMax;

            //        SetUpdateText(s.StatusName + s.StatusMessage, "");
            //    }));

            //};

            //if (ThisUpdateMods != LastUpdateMods)
            //{
            //    var xw = false;
            //    Task.Run(() =>
            //    {

            //        UpdateStatus("Mods:", "", 0, 100);


            //        while (!x.ForceUpdateMods())
            //        {

            //        }

            //        xw = true;
            //    });


            //    while (!xw)
            //    {
            //        Thread.Sleep(150);
            //    }

            //    set.Launcher.modsTimestamp = int.Parse(LastUpdateMods);
            //}

        }

    }


    /// <summary>
    /// Model for Minecraft Task Updates
    /// </summary>
    public class MinecraftLoadingUpdate
    {
        /// <summary>
        /// Word form of IntProgress
        /// </summary>
        public String TextProgress { get; set; } = "Idle";

        /// <summary>
        /// 0 - Loading
        /// 1 - Running
        /// 2 - Stopping
        /// 3 - Error
        /// 4 - Idle
        /// </summary>
        public int IntProgress { get; set; } = 4;

        /// <summary>
        /// Display an intermediate progress bar instead of a min-max one.
        /// Default is true.
        /// Only visible when Loading (State 0)
        /// </summary>
        public bool Intermediate { get; set; } = true;

        /// <summary>
        /// Returns the maximum for a loading bar
        /// </summary>
        public int MaxProgress {  get; set; } = 100;

        /// <summary>
        /// Returns the current progress for a loading bar
        /// </summary>
        public int CurrentProgress { get; set; } = 0;

        /// <summary>
        /// Current process title by the task
        /// </summary>
        public String? Process { get; set; } = "";

        /// <summary>
        /// Current process' description
        /// </summary>
        public String? ProcessDescription { get; set; } = "";

    }

    
}
