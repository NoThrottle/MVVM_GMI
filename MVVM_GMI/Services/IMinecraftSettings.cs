using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MVVM_GMI.Services
{
    public interface IMinecraftSettings
    {
        /// <summary>
        /// Maximum RAM allocated for Minecraft (not the JVM)
        /// </summary>
        int MaxRamAllocation { get; set; }

        /// <summary>
        /// Minimum RAM allocated for Minecraft (not the JVM) if it is not capped
        /// </summary>
        int MinRamAllocation { get; set; }

        /// <summary>
        /// Makes the minimum ram allocation the same as the maximum. May help with lag
        /// </summary>
        bool CapRamAllocation { get; set; }

        /// <summary>
        /// Starts Minecraft in fullscreen in the current active screen
        /// </summary>
        bool StartFullscreen { get; set; }

        /// <summary>
        /// Starts Minecraft in this Width if not in fullscreen
        /// </summary>
        int StartingWidth { get; set; }

        /// <summary>
        /// Starts Minecraft in this Height if not in fullscreen
        /// </summary>
        int StartingHeight {  get; set; }

        /// <summary>
        /// Enables log output in Minecraft, may be laggy
        /// </summary>
        bool EnableLogging { get; set; }

        /// <summary>
        /// JVM Arguments to start Minecraft with
        /// </summary>
        string[]? JVMArguments { get; set; }

    }

    public class MinecraftSetting : IMinecraftSettings
    {
        int IMinecraftSettings.MaxRamAllocation { get; set; }
        int IMinecraftSettings.MinRamAllocation { get; set; }
        bool IMinecraftSettings.CapRamAllocation { get; set; }
        bool IMinecraftSettings.StartFullscreen { get; set; }
        int IMinecraftSettings.StartingWidth { get; set; }
        int IMinecraftSettings.StartingHeight { get; set; }
        bool IMinecraftSettings.EnableLogging { get; set; }
        string[]? IMinecraftSettings.JVMArguments { get; set; }




    }
}
