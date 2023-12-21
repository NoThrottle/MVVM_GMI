using Kajabity.Tools.Java;
using MVVM_GMI.Helpers;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms.VisualStyles;

namespace MVVM_GMI.Services
{
    public class ConfigurationService : ILauncherProperties
    {
        public static string pathLauncherSettings = Path.Combine(Path.Combine(Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), ".gmi")), "config.props");

        private static ConfigurationService instance;
        private static readonly object lockObject = new object();

        public ConfigurationService()
        {
            PropertiesExist();
        }

        public static ConfigurationService Instance
        {
            get
            {
                if (instance == null)
                {
                    lock (lockObject)
                    {
                        if (instance == null)
                        {
                            instance = new ConfigurationService();
                        }
                    }
                }
                return instance;
            }
        }

        public static class Minecraft
        {

            private static int _MaxRamAllocation;
            /// <summary>
            /// Maximum RAM allocated for Minecraft (not the JVM)
            /// </summary>
            public static int MaxRamAllocation
            {
                get
                {
                    return _MaxRamAllocation;
                }
                set
                {
                    _MaxRamAllocation = value;
                    Instance.setProperty("MaxRamAllocation", value);

                }
            }

            private static int _MinRamAllocation { get; set; }
            /// <summary>
            /// Minimum RAM allocated for Minecraft (not the JVM) if it is not capped
            /// </summary>
            public static int MinRamAllocation
            {
                get
                {
                    return _MinRamAllocation;
                }
                set
                {
                    _MinRamAllocation = value;
                    Instance.setProperty("MinRamAllocation", value);

                }
            }

            private static bool _CapRamAllocation { get; set; }
            /// <summary>
            /// Makes the minimum ram allocation the same as the maximum. May help with lag
            /// </summary>
            public static bool CapRamAllocation
            {
                get
                {
                    return _CapRamAllocation;
                }
                set
                {
                    _CapRamAllocation = value;
                    Instance.setProperty("CapRamAllocation", value);

                }
            }

            private static bool _StartFullscreen { get; set; }
            /// <summary>
            /// Starts Minecraft in fullscreen in the current active screen
            /// </summary>
            public static bool StartFullscreen
            {
                get
                {
                    return _StartFullscreen;
                }
                set
                {
                    _StartFullscreen = value;
                    Instance.setProperty("StartFullscreen", value);

                }
            }


            private static int _StartingWidth { get; set; }
            /// <summary>
            /// Starts Minecraft in this Width if not in fullscreen
            /// </summary>
            public static int StartingWidth
            {
                get
                {
                    return _StartingWidth;
                }
                set
                {
                    _StartingWidth = value;
                    Instance.setProperty("StartingWidth", value);

                }
            }

            private static int _StartingHeight { get; set; }
            /// <summary>
            /// Starts Minecraft in this Height if not in fullscreen
            /// </summary>
            public static int StartingHeight
            {
                get
                {
                    return _StartingHeight;
                }
                set
                {
                    _StartingHeight = value;
                    Instance.setProperty("StartingHeight", value);

                }
            }

            private static bool _EnableLogging { get; set; }
            /// <summary>
            /// Enables log output in Minecraft, may be laggy
            /// </summary>
            public static bool EnableLogging
            {
                get
                {
                    return _EnableLogging;
                }
                set
                {
                    _EnableLogging = value;
                    Instance.setProperty("EnableLogging", value);

                }
            }

            private static string[]? _JVMArguments { get; set; }
            /// <summary>
            /// JVM Arguments to start Minecraft with
            /// </summary>
            public static string[]? JVMArguments
            {
                get
                {
                    return _JVMArguments;
                }
                set
                {
                    _JVMArguments = value;
                    Instance.setProperty("JVMArguments", value);

                }
            }


            /// <summary>
            /// UNUSED: True if the settings file has been constructed.
            /// </summary>
            public static bool WrittenToFile { get; set; }

            /// <summary>
            /// Sets the initial value of the variables.
            /// Bypasses a stack overflow when editing the value of the public variables
            /// </summary>
            /// <param name="MaxRamAllocation"></param>
            /// <param name="MinRamAllocation"></param>
            /// <param name="CapRamAllocation"></param>
            /// <param name="StartFullscreen"></param>
            /// <param name="StartingWidth"></param>
            /// <param name="StartingHeight"></param>
            /// <param name="EnableLogging"></param>
            /// <param name="JVMArguments"></param>
            public static void SetInitialValues(int MaxRamAllocation,
                                                int MinRamAllocation,
                                                bool CapRamAllocation,
                                                bool StartFullscreen,
                                                int StartingWidth,
                                                int StartingHeight,
                                                bool EnableLogging,
                                                string[] JVMArguments)
            {
                _MaxRamAllocation = MaxRamAllocation;
                _MinRamAllocation = MinRamAllocation;
                _CapRamAllocation = CapRamAllocation;
                _StartFullscreen = StartFullscreen;
                _StartingWidth = StartingWidth;
                _StartingHeight = StartingHeight;
                _EnableLogging = EnableLogging;
                _JVMArguments = JVMArguments;
            }
        }

        public static class Launcher
        {

            private static string _LauncherPath { get; set; }
            /// <summary>
            /// Path for the Launcher, defaults to User Roaming Appdata .gmi
            /// </summary>
            public static String LauncherPath
            {
                get
                {
                    return _LauncherPath;
                }
                set
                {
                    _LauncherPath = value;
                    Instance.setProperty("LauncherPath", value);

                }
            }

            private static string _MinecraftPath { get; set; }
            /// <summary>
            /// Path for Minecraft, defaults to LauncherPath .../mc/
            /// </summary>
            public static String MinecraftPath
            {
                get
                {
                    return _MinecraftPath;
                }
                set
                {
                    _MinecraftPath = value;
                    Instance.setProperty("MinecraftPath", value);

                }
            }

            private static bool _AppTheme { get; set; }
            /// <summary>
            /// Defaults to Dark Mode (0). Light Mode is = 1
            /// </summary>
            public static bool AppTheme
            {
                get
                {
                    return _AppTheme;
                }
                set
                {
                    _AppTheme = value;
                    Instance.setProperty("AppTheme", value);

                }
            }

            private static bool _AutoDownloadUpdates { get; set; }
            /// <summary>
            /// Default to True
            /// </summary>
            public static bool AutoDownloadUpdates
            {
                get
                {
                    return _AutoDownloadUpdates;
                }
                set
                {
                    _AutoDownloadUpdates = value;
                    Instance.setProperty("AutoDownloadUpdates", value);

                }
            }

            private static bool _AutoInstallUpdates { get; set; }

            /// <summary>
            /// Defaults to True
            /// </summary>
            public static bool AutoInstallUpdates
            {
                get
                {
                    return _AutoInstallUpdates;
                }
                set
                {
                    _AutoInstallUpdates = value;
                    Instance.setProperty("AutoInstallUpdates", value);

                }
            }

            /// <summary>
            /// UNUSED: True if the settings file has been constructed.
            /// </summary>
            public static bool WrittenToFile { get; set; }

            /// <summary>
            /// Sets the initial value of the variables.
            /// Bypasses a stack overflow when editing the value of the public variables
            /// </summary>
            /// <param name="LauncherPath"></param>
            /// <param name="MinecraftPath"></param>
            /// <param name="AppTheme"></param>
            /// <param name="AutoDownloadUpdates"></param>
            /// <param name="AutoInstallUpdates"></param>
            public static void SetInitialValues(string LauncherPath,
                                                string MinecraftPath,
                                                bool AppTheme,
                                                bool AutoDownloadUpdates,
                                                bool AutoInstallUpdates)
            {
                _LauncherPath = LauncherPath;
                _MinecraftPath = MinecraftPath;
                _AppTheme = AppTheme;
                _AutoDownloadUpdates = AutoDownloadUpdates;
                _AutoInstallUpdates = AutoInstallUpdates;
            }
        }

        //--------------------------------------------------//

        static List<QueueObject> QueueObjects = new List<QueueObject>();
        static bool QueueExists = false;
        static bool QueueWriting = false;

        private class QueueObject
        {
            public string sender { get; set; }
            public object value { get; set; }
        }

        private void setProperty(string sender, object value)
        {

            WriteProperties();
            //if (!QueueExists) { WriteQueueProcess(); }

            //QueueObjects.Add(new QueueObject { sender = sender, value = value });

        }

        public static void WriteProperties()
        {
            using (FileStream stream = new FileStream(pathLauncherSettings, FileMode.Create))
            {
                var p = new JavaProperties();

                p.SetProperty("configVersion", ILauncherProperties.ConfigVersion.ToString());

                p.SetProperty("MaxRamAllocation", Minecraft.MaxRamAllocation.ToString());
                p.SetProperty("MinRamAllocation", Minecraft.MinRamAllocation.ToString());
                p.SetProperty("CapRamAllocation", Minecraft.CapRamAllocation.ToString());
                p.SetProperty("StartFullscreen", Minecraft.StartFullscreen.ToString());
                p.SetProperty("StartingWidth", Minecraft.StartingWidth.ToString());
                p.SetProperty("StartingHeight", Minecraft.StartingHeight.ToString());
                p.SetProperty("EnableLogging", Minecraft.EnableLogging.ToString());
                p.SetProperty("JVMArguments", Minecraft.JVMArguments.ToString());

                p.SetProperty("LauncherPath", Launcher.LauncherPath);
                p.SetProperty("MinecraftPath", Launcher.MinecraftPath);
                p.SetProperty("AppTheme", Launcher.AppTheme.ToString());
                p.SetProperty("AutoDownloadUpdates", Launcher.AutoDownloadUpdates.ToString());
                p.SetProperty("AutoInstallUpdates", Launcher.AutoInstallUpdates.ToString());

                p.Store(stream, "DO NOT TOUCH");

            }

        }

        private static void WriteQueueProcess()
        {
            Task.Run(async () =>
            {

                QueueExists = true;
                while (true)
                {
                    Thread.Sleep(100);

                    if (QueueObjects.Count != 0)
                    {
                        QueueWriting = true;
                        var x = QueueObjects[0];
                        try
                        {
                            using (FileStream stream = new FileStream(pathLauncherSettings, FileMode.Create))
                            {
                                var prop = new JavaProperties();

                                prop.SetProperty(x.sender, x.value.ToString());
                                

                                prop.Store(stream, "DO NOT TOUCH");
                            }
                        }
                        catch (Exception e)
                        {
                            MessageBox.Show(e.StackTrace, "Error");
                        }
                        QueueObjects.RemoveAt(0);
                        QueueWriting = false;
                    }
                }
            });
        }


        /// <summary>
        /// Reads the properties files and updates the values in memory
        /// </summary>
        private void ReadProperties()
        {

            Console.WriteLine("Reading Properties");

            var p = new JavaProperties();

            Object[] x = new object[8];
            Object[] y = new object[5];

            using (Stream stream = File.OpenRead(pathLauncherSettings))
            {
                p.Load(stream);

                if (p.ContainsKey("MaxRamAllocation"))
                {
                    x[0] = int.Parse(p.GetProperty("MaxRamAllocation"));
                }
                if (p.ContainsKey("MinRamAllocation"))
                {
                    x[1] = int.Parse(p.GetProperty("MinRamAllocation"));
                }
                if (p.ContainsKey("CapRamAllocation"))
                {
                    x[2] = bool.Parse(p.GetProperty("CapRamAllocation"));
                }
                if (p.ContainsKey("StartFullscreen"))
                {
                    x[3] = bool.Parse(p.GetProperty("StartFullscreen"));
                }
                if (p.ContainsKey("StartingWidth"))
                {
                    x[4] = int.Parse(p.GetProperty("StartingWidth"));
                }
                if (p.ContainsKey("StartingHeight"))
                {
                    x[5] = int.Parse(p.GetProperty("StartingHeight"));
                }
                if (p.ContainsKey("EnableLogging"))
                {
                    x[6] = bool.Parse(p.GetProperty("EnableLogging"));
                }
                if (p.ContainsKey("JVMArguments"))
                {
                    x[7] = p.GetProperty("JVMArguments").Split(',');
                }

                //---------------//

                if (p.ContainsKey("LauncherPath"))
                {
                    y[0] = p.GetProperty("LauncherPath");
                }
                if (p.ContainsKey("MinecraftPath"))
                {
                    y[1] = p.GetProperty("MinecraftPath");
                }
                if (p.ContainsKey("AppTheme"))
                {
                    y[2] = bool.Parse(p.GetProperty("AppTheme"));
                }
                if (p.ContainsKey("AutoDownloadUpdates"))
                {
                    y[3] = bool.Parse(p.GetProperty("AutoDownloadUpdates"));
                }
                if (p.ContainsKey("AutoInstallUpdates"))
                {
                    y[4] = bool.Parse(p.GetProperty("AutoInstallUpdates"));
                }

            }

            Minecraft.SetInitialValues((int)x[0], (int)x[1], (bool)x[2], (bool)x[3], (int)x[4], (int)x[5], (bool)x[6], (string[])x[7]);
            Launcher.SetInitialValues((string)y[0], (string)y[1], (bool)y[2], (bool)y[3], (bool)y[4]);
            
        }


        /// <summary>
        /// Writes the default values for all the settings
        /// </summary>
        public void WriteDefault()
        {
            Console.WriteLine("Writing Default Properties");

            int defaultRAM()
            {
                if (SystemInfo.SystemRam() > 6000)
                {
                    return 2048;
                }
                else
                {
                    return 1560;
                }
            }

            try
            {

                using(FileStream stream = new FileStream(pathLauncherSettings, FileMode.Create))
                {
                    var p = new JavaProperties();

                    p.SetProperty("configVersion", ILauncherProperties.ConfigVersion.ToString());

                    p.SetProperty("MaxRamAllocation",defaultRAM().ToString());
                    p.SetProperty("MinRamAllocation","128");
                    p.SetProperty("CapRamAllocation", "false");
                    p.SetProperty("StartFullscreen", "false");
                    p.SetProperty("StartingWidth", "1280");
                    p.SetProperty("StartingHeight", "720");
                    p.SetProperty("EnableLogging", "false");
                    p.SetProperty("JVMArguments", "");

                    p.SetProperty("LauncherPath", Path.Combine(Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), ".gmi")));
                    p.SetProperty("MinecraftPath", Path.Combine(Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), ".gmi"), "mc"));
                    p.SetProperty("AppTheme", "false");
                    p.SetProperty("AutoDownloadUpdates", "true");
                    p.SetProperty("AutoInstallUpdates", "true");

                    p.Store(stream, "DO NOT TOUCH");

                }

            }
            catch (Exception e)
            {
                MessageBox.Show(e.StackTrace, "Error"); // REPLACE THIS WiTH NOTIFICATION SERVICE

            }
        }

        bool PropertiesExist()
        {
            if (File.Exists(pathLauncherSettings))
            {
                try
                {
                    ReadProperties();
                }
                catch (Exception e)
                {
                    WriteDefault();
                }
                return true;
            }
            else
            {
                WriteDefault();
                return true;
            }

        }
    }
}
