using MVVM_GMI.Helpers;
using Newtonsoft.Json.Linq;
using System.IO;

namespace MVVM_GMI.Services
{
    public class ConfigurationService : ILauncherProperties
    {
        private static string pathLauncherJSON = Path.Combine(Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), ".gmi"), "config.json");
        private static string pathLauncher = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), ".gmi");

        #region Boiler plate

        private static ConfigurationService instance;
        private static readonly object lockObject = new object();

        public Launcher fromLauncher { get; set; }
        public Minecraft fromMinecraft { get; set; }

        private ConfigurationService()
        {
            JSONPropsExist();
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
                            instance.fromLauncher = new Launcher();
                            instance.fromMinecraft = new Minecraft();
                        }
                    }
                }
                return instance;
            }
        }


        #endregion

        public class Minecraft
        {

            static int defaultRAM()
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

            private int _MaxRamAllocation = int.Parse((string)instance.GetValue("MaxRamAllocation", defaultRAM()));
            /// <summary>
            /// Maximum RAM allocated for Minecraft (not the JVM)
            /// </summary>
            public int MaxRamAllocation
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

            private int _MinRamAllocation = int.Parse((string)instance.GetValue("MinRamAllocation", 128));
            /// <summary>
            /// Minimum RAM allocated for Minecraft (not the JVM) if it is not capped
            /// </summary>
            public int MinRamAllocation
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

            private bool _CapRamAllocation = bool.Parse((string)instance.GetValue("CapRamAllocation", false));
            /// <summary>
            /// Makes the minimum ram allocation the same as the maximum. May help with lag
            /// </summary>
            public bool CapRamAllocation
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

            private bool _StartFullscreen = bool.Parse((string)instance.GetValue("StartFullscreen", false));
            /// <summary>
            /// Starts Minecraft in fullscreen in the current active screen
            /// </summary>
            public bool StartFullscreen
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


            private int _StartingWidth = int.Parse((string)instance.GetValue("StartingWidth", 1280));
            /// <summary>
            /// Starts Minecraft in this Width if not in fullscreen
            /// </summary>
            public int StartingWidth
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

            private int _StartingHeight = int.Parse((string)instance.GetValue("StartingHeight", 720));
            /// <summary>
            /// Starts Minecraft in this Height if not in fullscreen
            /// </summary>
            public int StartingHeight
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

            private bool _EnableLogging = bool.Parse((string)instance.GetValue("EnableLogging", false));
            /// <summary>
            /// Enables log output in Minecraft, may be laggy
            /// </summary>
            public bool EnableLogging
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

            private string? _JVMArguments = (string)instance.GetValue("JVMArguments", "");
            /// <summary>
            /// JVM Arguments to start Minecraft with
            /// </summary>
            public string? JVMArguments
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
            public bool WrittenToFile { get; set; }
        }

        public class Launcher
        {

            private string _LauncherPath = (string)instance.GetValue("LauncherPath", pathLauncher);
            /// <summary>
            /// Path for the Launcher, defaults to UserCredential Roaming Appdata .gmi
            /// </summary>
            public string LauncherPath
            {
                get
                {
                    return _LauncherPath;
                    //return "";
                }
                set
                {
                    _LauncherPath = value;
                    Instance.setProperty("LauncherPath", value);

                }
            }

            private string _MinecraftPath = (string)instance.GetValue("MinecraftPath", Path.Combine(pathLauncher, "mc"));
            /// <summary>
            /// Path for Minecraft, defaults to LauncherPath .../mc/
            /// </summary>
            public String MinecraftPath
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

            private bool _AppTheme = bool.Parse((string)instance.GetValue("AppTheme", false));
            /// <summary>
            /// Defaults to Dark Mode (0). Light Mode is = 1
            /// </summary>
            public bool AppTheme
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

            private bool _AutoDownloadUpdates = bool.Parse((string)instance.GetValue("AutoDownloadUpdates", false));
            /// <summary>
            /// Default to True
            /// </summary>
            public bool AutoDownloadUpdates
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

            private bool _AutoInstallUpdates = bool.Parse((string)instance.GetValue("AutoInstallUpdates", false));

            /// <summary>
            /// Defaults to True
            /// </summary>
            public bool AutoInstallUpdates
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


            private int _ModUpdateIndex = int.Parse((string)instance.GetValue("ModUpdateIndex", 0));
            public int ModUpdateIndex
            {
                get
                {
                    return _ModUpdateIndex;
                }
                set
                {
                    _ModUpdateIndex = value;
                    Instance.setProperty("ModUpdateIndex", value);

                }
            }


            /// <summary>
            /// UNUSED: True if the settings file has been constructed.
            /// </summary>
            public bool WrittenToFile { get; set; }
        }

        //--------------------------------------------------//


        void setProperty<T>(string key, T value)
        {
            Update<T>(key, value);
        }

        /// <summary>
        /// Gets the Value from the key if it exists, otherwise returns the default value.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="key"></param>
        /// <param name="defaultValue"></param>
        /// <returns></returns>
        internal object GetValue<T>(string key, T defaultValue)
        {

            

            JSONPropsExist();

            using (Stream stream = File.Open(pathLauncherJSON, FileMode.Open, FileAccess.Read, FileShare.Read))
            using (StreamReader streamReader = new StreamReader(stream))
            {
                JObject obj = null;

                try
                {
                    obj = JObject.Parse(streamReader.ReadToEnd());

                    if (obj.ContainsKey(key))
                    {

                        JToken val = obj[key];

                        if (val is JValue jval)
                        {
                            return val.ToString();
                        }
                        else if (val is JArray jarr)
                        {
                            List<string> stringArray = jarr.ToObject<List<string>>();
                            return stringArray.ToString();

                        }
                    }
                }
                catch
                {

                }


            }

            Update<T>(key, defaultValue);
            return defaultValue.ToString();


        }

        //to do, remove default value and fix logic

        /// <summary>
        /// Writes the value to the JSON file. Creates the key if it doesn't exist.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="key"></param>
        /// <param name="value"></param>
        /// <param name="defaultValue"></param>
        /// <returns></returns>
        private void Update<T>(string key, T value)
        {
            JSONPropsExist();

            string jsonString = File.ReadAllText(pathLauncherJSON);

            if (jsonString.Length == 0)
            {
                JObject sobj = new JObject();

                if (value is IEnumerable<T>)
                {
                    var arr = new JArray();

                    foreach (var t in ((IEnumerable<T>)value))
                    {
                        arr.Add(t);
                    }

                    sobj.Add(key, arr);
                }
                else
                {
                    sobj.Add(key, value.ToString());
                }

                File.WriteAllText(pathLauncherJSON, sobj.ToString());
                return;
            }

            JObject obj = JObject.Parse(jsonString);

            if (obj.ContainsKey(key))
            {

                if (obj[key] is JValue && value is not IEnumerable<T>)
                {
                    obj[key] = value.ToString();
                }
                else if (obj[key] is JArray && value is IEnumerable<T>)
                {
                    ((JArray)obj[key]).Add((IEnumerable<T>)value);
                }

                string edited = obj.ToString();
                File.WriteAllText(pathLauncherJSON, edited);
                return;

            }
            else
            {
                if (value is IEnumerable<T>)
                {
                    var arr = new JArray();

                    foreach (var t in ((IEnumerable<T>)value))
                    {
                        arr.Add(t);
                    }

                    obj.Add(key, arr);
                }
                else
                {
                    obj.Add(key, value.ToString());
                }

                string edited = obj.ToString();
                File.WriteAllText(pathLauncherJSON, edited);
                return;

            }
        }

        /// <summary>
        /// Create the JSON File. Returns true if it exists, false if it doesnt but creates it anyway.
        /// Throws error if something went wrong in creating the JSON file
        /// </summary>
        /// <returns></returns>
        private bool JSONPropsExist()
        {

            Directory.CreateDirectory(pathLauncher);


            if (File.Exists(pathLauncherJSON))
            {
                return true;
               
            }

            using (FileStream fileStream = File.Create(pathLauncherJSON))
            {
                
            }

            var x = 0;
            var hasError = true;

            while(hasError)
            {
                try
                {
                    using(Stream stream = File.OpenRead(pathLauncherJSON))
                    {
                        hasError = false;
                        break;
                    }

                }
                catch
                {
                    x++;
                    Thread.Sleep(1000);
                }

                if (x >= 10)
                {
                    throw new Exception("Something Went Wrong in creating the JSON FIle");
                }
            }

            return false;
        }
    }
}
