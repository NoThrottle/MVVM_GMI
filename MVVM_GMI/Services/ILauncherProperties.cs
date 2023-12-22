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

        /// <summary>
        /// Salt used for storing passwords, not meant to be out.
        /// Password will be passwordSALT and then 
        /// a random pepper is generated, appended as PEPPERpasswordSALT
        /// and then the SHA-256 hash is taken.
        /// The pepper is stored online
        /// </summary>
        public static string Salt { get; } = "8270ebcf-22e2-491b-84b8-d96f0f2e5edb";

        /// <summary>
        /// Key used to authenticate when accessing the GoogleSheets
        /// </summary>
        public static string SheetsKey { get; } = "AIzaSyAFGJFSJfQQn0FdtTkz63_KINBCTLQ_Ikg";

        /// <summary>
        /// The ID of the database sheet
        /// </summary>
        public static string SheetID { get; } = "1TV4yOzTzN7rYjZSXlnNuhAKz8AuQKSPjG8jHJiiUk9g";
    }
}
