namespace MVVM_GMI.Services
{
    public class ILauncherProperties
    {

        /// <summary>
        /// Current version of the installed Launcher
        /// </summary>
        int LauncherVersion { get; } = 0;


        /// <summary>
        /// Current version of the installed Launcher in a readable format
        /// </summary>
        String LauncherVersionReadable { get; } = "0.1.0";


    }
}
