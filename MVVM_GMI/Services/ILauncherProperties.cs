using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MVVM_GMI.Services
{
    public interface ILauncherProperties
    {

        int LauncherVersion { get; set; }

        String LauncherVersionReadable { get; set; }


    }
}
