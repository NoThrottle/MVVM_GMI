namespace MVVM_GMI.Services
{
    public class ILauncherProperties
    {

        /// <summary>
        /// Current version of the installed Launcher
        /// </summary>
        internal static int LauncherVersion { get; } = 2;


        /// <summary>
        /// Current version of the installed Launcher in a readable format
        /// </summary>
        internal static String LauncherVersionReadable { get; } = "1.0.0";

        /// <summary>
        /// Current version of the config file. 
        /// Changes in version may mean translation.s
        /// </summary>
        internal static int ConfigVersion { get; } = 1;

        /// <summary>
        /// Salt used for storing passwords, not meant to be out.
        /// Password will be passwordSALT and then 
        /// a random pepper is generated, appended as PEPPERpasswordSALT
        /// and then the SHA-256 hash is taken.
        /// The pepper is stored online
        /// </summary>
        internal static string Salt { get; } = "8270ebcf-22e2-491b-84b8-d96f0f2e5edb";



    }
}
