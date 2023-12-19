using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net.Http.Headers;
using System.Net.Http;
using System.Net.NetworkInformation;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows;
using System.IO;
using System.Management;
using Config.Net;

namespace MVVM_GMI.Services
{
    public class SettingsStartupService : ILauncherProperties
    {

        public SettingsStartupService() 
        {

            LauncherSettings();
            MinecraftSettings();
        
        }

        /// <summary>
        /// Builds the Launcher Settings file if it does not yet exist
        /// Sets default values to settings if they do not exist.
        /// </summary>
        void LauncherSettings()
        {
            ILauncherSettings s = new ConfigurationBuilder<ILauncherSettings>().UseAppConfig().Build();

            if (s.LauncherPath == null || s.LauncherPath.Length == 0) 
            {

                s.LauncherPath = Path.Combine(Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), ".gmi"));

            }
            
            if (s.MinecraftPath == null || s.MinecraftPath.Length == 0)
            {
                s.MinecraftPath = Path.Combine(Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), ".gmi"),"mc");
            }

            if(s.WrittenToFile == false)
            {
                s.WrittenToFile = true;
                s.AppTheme = false;
                s.AutoDownloadUpdates = true;
                s.AutoInstallUpdates = true;

            }

        }

        /// <summary>
        /// Builds the Minecraft Settings file if it does not yet exist
        /// Sets default values to settings if they do not exist.
        /// </summary>
        void MinecraftSettings()
        {
            IMinecraftSettings s = new ConfigurationBuilder<IMinecraftSettings>().UseAppConfig().Build();

            if (s.WrittenToFile == false) 
            {
                s.WrittenToFile = true;
                s.StartFullscreen = false;
                s.EnableLogging = false;
                s.CapRamAllocation = false;
            }

            if (s.StartingHeight <= 0 )
            {
                s.StartingHeight = 720;
            }

            if (s.StartingWidth <= 0)
            {
                s.StartingWidth = 1280;
            }

            if (s.MinRamAllocation <= 0)
            {
                s.MinRamAllocation = 128;
            }

            if (s.MaxRamAllocation <= 0)
            {
                string ram = "0";
                ObjectQuery winQuery = new ObjectQuery("SELECT * FROM CIM_OperatingSystem");

                ManagementObjectSearcher searcher = new ManagementObjectSearcher(winQuery);

                foreach (ManagementObject item in searcher.Get())
                {
                    ram = item["TotalVisibleMemorySize"].ToString();
                }

                var x = (int)Math.Floor(decimal.Parse(ram) / 1024);

                if (x > 6000)
                {
                    s.MaxRamAllocation = 2048;
                }
                else
                {
                    s.MaxRamAllocation = 1560;
                }
            }
        
        }

    }
}
