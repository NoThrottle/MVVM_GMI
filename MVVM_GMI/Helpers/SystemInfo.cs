using System;
using System.Collections.Generic;
using System.Linq;
using System.Management;
using System.Text;
using System.Threading.Tasks;

namespace MVVM_GMI.Helpers
{
    public class SystemInfo
    {
        public static int SystemRam()
        {

            string ram = "0";
            ObjectQuery winQuery = new ObjectQuery("SELECT * FROM CIM_OperatingSystem");

            ManagementObjectSearcher searcher = new ManagementObjectSearcher(winQuery);

            foreach (ManagementObject item in searcher.Get())
            {
                ram = item["TotalVisibleMemorySize"].ToString();
            }

            return (int)Math.Floor(decimal.Parse(ram) / 1024);
        }


    }
}
