namespace MVVM_GMI.Services
{
    public class ILauncherProperties
    {

        /// <summary>
        /// Current version of the installed Launcher
        /// </summary>
        public static int LauncherVersion { get; } = 0;


        /// <summary>
        /// Current version of the installed Launcher in a readable format
        /// </summary>
        public static String LauncherVersionReadable { get; } = "0.1.0";

        /// <summary>
        /// Current version of the config file. 
        /// Changes in version may mean translation.s
        /// </summary>
        public static int ConfigVersion { get; } = 1;



    }
}
