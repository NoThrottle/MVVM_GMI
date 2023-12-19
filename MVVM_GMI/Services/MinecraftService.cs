using CmlLib.Core.Auth;
using CmlLib.Core.Installer.FabricMC;
using CmlLib.Core;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using MVVM_GMI.Views.Pages;
using MVVM_GMI.Views.Windows;
using System.Diagnostics;
using System.Drawing.Imaging;
using System.IO;
using MVVM_GMI.ViewModels.Pages;

namespace MVVM_GMI.Services
{
    public class MinecraftService : IMinecraftSettings
    {
        public event Action? TaskCompleted;
        public event Action<int>? ProgressUpdated;

        public bool MinecraftRunning = false;

        static MinecraftPath path = new MinecraftPath(Path.Combine(Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), ".gmi")));

        public async Task QuickLaunch()
        {

            

            //UpdateStatus("Initializing", "Checking Minecraft Installation", 0, 100);

            Console.WriteLine("Check Everything (Quick Launch)");

            //Application.Current.Dispatcher.Invoke(new Action(() => {
            //    ProgressBar_Large.Visibility = Visibility.Visible;
            //    ProgressBar_Large.IsIndeterminate = true;
            //    Button_Join_States("Loading");
           // }));

            var launcher = new CMLauncher(path);
            launcher.GameFileCheckers.JavaFileChecker.CheckHash = false;
            launcher.GameFileCheckers.LibraryFileChecker.CheckHash = false;
            launcher.GameFileCheckers.LogFileChecker.CheckHash = false;
            launcher.GameFileCheckers.ClientFileChecker.CheckHash = false;
            launcher.GameFileCheckers.AssetFileChecker.CheckHash = false;

            InitializeAsync(launcher);


        }

        public async Task CheckEverything()
        {

            //UpdateStatus("Initializing", "Checking Minecraft Installation", 0, 100);

            Console.WriteLine("Check Everything");

            //Application.Current.Dispatcher.Invoke(new Action(() => {
            //    ProgressBar_Large.Visibility = Visibility.Visible;
            //    ProgressBar_Large.IsIndeterminate = true;
            //    Button_Join_States("Loading");
            //}));

            var launcher = new CMLauncher(path);
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

        private CMLauncher SetSettings(CMLauncher launcher)
        {


            return launcher;
        }

        public void InitializeAsync(CMLauncher launcher)
        {

            System.Net.ServicePointManager.DefaultConnectionLimit = 256;

            launcher.FileChanged += (e) =>
            {
                Application.Current.Dispatcher.Invoke(new Action(() =>
                {
                    //UpdateStatus("Initializing Minecraft: ", String.Format("[{0}] {1} - {2}/{3}", e.FileKind.ToString(), e.FileName, e.ProgressedFileCount, e.TotalFileCount), e.ProgressedFileCount, e.TotalFileCount);

                }));

                if (e.FileName == "release")
                {
                    //Application.Current.Dispatcher.Invoke(new Action(() => {
                    //    SetUpdateText("", "");
                    //    ProgressBar_Large.IsIndeterminate = true;
                    //}));
                }
            };

            Process process = null;

            //process = launcher.CreateProcess("fabric-loader-" + FabricLoaderVersion + "-" + MinecraftVersion, new MLaunchOption
            //{

            //    //JavaPath = Path.Combine(toUse_JavaPath,"bin"),
            //    ServerIp = serverAddress,

            //    MaximumRamMb = set.Minecraft.ramAllocation,

            //    ScreenWidth = 1280,
            //    ScreenHeight = 720,

            //    Session = MSession.GetOfflineSession(@set.Minecraft.username),
            //});

            process.StartInfo.RedirectStandardError = true;
            process.StartInfo.RedirectStandardOutput = true;
            process.EnableRaisingEvents = true;
            process.ErrorDataReceived += (s, e) => Console.WriteLine(e.Data);

            //UpdateStatus("Starting Minecraft:", "Waiting for game...", true);

            bool check = true;

            Console.WriteLine("Starting Minecraft Process");

            Console.WriteLine(process.StartInfo.FileName);
            Console.WriteLine(process.StartInfo.Arguments);
            var processUtil = new CmlLib.Utils.ProcessUtil(process);

            processUtil.OutputReceived += (s, e) =>
            {
                Console.WriteLine(e);

                //if (check && e != null && e.Contains(@"Connecting to"))
                //{
                //    check = false;
                //    Application.Current.Dispatcher.Invoke(new Action(() =>
                //    {

                //        SetUpdateText("hide", "");

                //        Button_Join_States("Running");

                //        ProgressBar_Large.IsIndeterminate = false;
                //        ProgressBar_Large.Visibility = Visibility.Hidden;

                //        if (this.WindowState != WindowState.Minimized)
                //        {
                //            this.WindowState = WindowState.Minimized;
                //        }
                //    }));
                //}

                if (e != null && e.Contains(@"Game Crashed! Crash report saved to:"))
                {
                    Application.Current.Dispatcher.Invoke(new Action(() =>
                    {
                        //SetDefaultState();
                    }));

                    MessageBox.Show("Game Crashed", "Error");
                }
            };

            processUtil.StartWithEvents();

            MinecraftRunning = true;

            Console.ReadLine();

            //process.Exited += (s, a) => SetDefaultState();

            //processUtil.Exited += (sender, args) =>
            //{
            //    Application.Current.Dispatcher.Invoke(new Action(() =>
            //    {
            //        UpdateStatus("", "", false);
            //        Button_Join_States("Join");
            //        SetDefaultState();
            //    }));

            //    MinecraftRunning = false;
            //};

            processUtil.Process.WaitForExit();


        }

        public async Task InstallFabric()
        {

            Console.WriteLine("Installing Fabric");

            var launcher = new CMLauncher(path);

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
    class MinecraftLoadingUpdate
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
        public String? Process { get; set; }

        /// <summary>
        /// Current process' description
        /// </summary>
        public String? ProcessDescription {  get; set; }

    }

    
}
