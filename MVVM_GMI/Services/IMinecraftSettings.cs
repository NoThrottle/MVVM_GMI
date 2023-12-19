using Config.Net;

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

        /// <summary>
        /// True if the settings file has been constructed.
        /// </summary>
        bool WrittenToFile { get; set; }

    }

    public class MinecraftSettingsService : IMinecraftSettings
    {
        public int MaxRamAllocation { get; set; }
        public int MinRamAllocation { get; set; } = 128;
        public bool CapRamAllocation { get; set; } = false;
        public bool StartFullscreen { get; set; } = false;
        public int StartingWidth { get; set; } = 1280;
        public int StartingHeight { get; set; } = 720;
        public bool EnableLogging { get; set; } = false;
        public string[]? JVMArguments { get; set; }
        public bool WrittenToFile { get; set; } = false;
    }

}
