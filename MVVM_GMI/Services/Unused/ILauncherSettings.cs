//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Text;
//using System.Threading.Tasks;

//namespace MVVM_GMI.Services
//{
//    public interface ILauncherSettings
//    {

//        /// <summary>
//        /// Path for the Launcher, defaults to UserCredential Roaming Appdata .gmi
//        /// </summary>
//        String LauncherPath { get; set; }

//        /// <summary>
//        /// Path for Minecraft, defaults to LauncherPath .../mc/
//        /// </summary>
//        String MinecraftPath { get; set; }

//        /// <summary>
//        /// Defaults to Dark Mode (0). Light Mode is = 1
//        /// </summary>
//        bool AppTheme { get; set; }

//        /// <summary>
//        /// Default to True
//        /// </summary>
//        bool AutoDownloadUpdates { get; set; }

//        /// <summary>
//        /// Defaults to True
//        /// </summary>
//        bool AutoInstallUpdates { get; set; }

//        /// <summary>
//        /// True if the settings file has been constructed.
//        /// </summary>
//        bool WrittenToFile { get; set; }
//    }

//    public class LauncherSettingsService : ILauncherSettings
//    {
//        public string LauncherPath { get; set; }
//        public string MinecraftPath { get; set; }
//        public bool AppTheme { get; set; }
//        public bool AutoDownloadUpdates { get; set; }
//        public bool AutoInstallUpdates { get; set; }
//        public bool WrittenToFile { get; set; }
//    }
//}
