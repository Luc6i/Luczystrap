namespace Bloxstrap.Plugins.PluginSDK
{
    /// <summary>
    /// Context provided to plugins with access to LuczyStrap features
    /// </summary>
    public class PluginContext
    {
        /// <summary>
        /// Directory where plugin can store data
        /// </summary>
        public string PluginDataDirectory { get; set; } = "";

        /// <summary>
        /// Logger for writing to LuczyStrap logs
        /// </summary>
        public IPluginLogger Logger { get; set; } = null!;

        /// <summary>
        /// Settings manager for persistent configuration
        /// </summary>
        public IPluginSettingsManager Settings { get; set; } = null!;

        /// <summary>
        /// Manager for installing/removing Roblox mods
        /// </summary>
        public IPluginModManager ModManager { get; set; } = null!;

        /// <summary>
        /// Manager for theme customization
        /// </summary>
        public IPluginThemeManager ThemeManager { get; set; } = null!;

        /// <summary>
        /// Current LuczyStrap version
        /// </summary>
        public string LuczyStrapVersion { get; set; } = "";

        /// <summary>
        /// Current Roblox version
        /// </summary>
        public string RobloxVersion { get; set; } = "";
    }
}