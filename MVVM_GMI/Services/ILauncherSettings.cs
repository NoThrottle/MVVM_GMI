using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MVVM_GMI.Services
{
    public interface ILauncherSettings
    {

        String LauncherPath { get; set; }

        String MinecraftPath { get; set; }

        String AppTheme { get; set; }

        bool AutoDownloadUpdates { get; set; }

        bool AutoInstallUpdates { get; set; }

    }
}
