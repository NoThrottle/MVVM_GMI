using Kajabity.Tools.Java;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MVVM_GMI.Services
{
    public class ConfigurationService
    {

        //YOU ARE TRANSFERRING SETTINGSHANDLER FORM GMI TO MVVM. YOU ARE CURRENTLY FIXING THE SETPROPERTY FUNCTION WRITER THING.
        //AFTER THIS YOU WILL TRANSSFER THE INTERFACES HERE FOR DATA TO A STATIC CLASS VERSION.
        //AFTER WHICH, TO USING FUNCTIONS, DEFINE @set.
        public static string pathLauncherSettings = Path.Combine(Path.Combine(Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), ".gmi")), "config.props");


        private static ConfigurationService instance;
        private static readonly object lockObject = new object();

        private ConfigurationService()
        {

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

            if (!QueueExists) { WriteQueueProcess(); }

            QueueObjects.Add(new QueueObject { sender = sender, value = value });

        }

        private static void WriteQueueProcess()
        {
            Task.Run(() =>
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

    }
}
