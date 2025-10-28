using System;
using System.Threading.Tasks;

namespace Bloxstrap.Plugins.PluginSDK
{
    /// <summary>
    /// Main interface that all LuczyStrap plugins must implement
    /// </summary>
    public interface ILuczyStrapPlugin
    {
        /// <summary>
        /// Plugin metadata (name, version, description, etc.)
        /// </summary>
        PluginMetadata Metadata { get; }

        /// <summary>
        /// Called when plugin is loaded. Return true if successful.
        /// </summary>
        Task<bool> OnLoadAsync(PluginContext context);

        /// <summary>
        /// Called when plugin is unloaded or disabled
        /// </summary>
        Task OnUnloadAsync();

        /// <summary>
        /// Called when Roblox is about to launch
        /// </summary>
        Task OnRobloxLaunchAsync();

        /// <summary>
        /// Called when Roblox has closed
        /// </summary>
        Task OnRobloxCloseAsync();
    }

    /// <summary>
    /// Plugin metadata
    /// </summary>
    public class PluginMetadata
    {
        public string Id { get; set; } = "";
        public string Name { get; set; } = "";
        public string Description { get; set; } = "";
        public string Author { get; set; } = "";
        public string Version { get; set; } = "1.0.0";
        public PluginType Type { get; set; }
        public string[] Dependencies { get; set; } = Array.Empty<string>();
        public string IconUrl { get; set; } = "";
        public string DownloadUrl { get; set; } = "";
        public DateTime CreatedDate { get; set; }
        public DateTime LastUpdated { get; set; }
        public int Downloads { get; set; }
        public double Rating { get; set; }
        public string[] Tags { get; set; } = Array.Empty<string>();
        public bool RequiresRestart { get; set; }
        public bool IsTrusted { get; set; }
    }

    /// <summary>
    /// Types of plugins
    /// </summary>
    public enum PluginType
    {
        Theme,
        Script,
        UIModule,
        Integration,
        Mod,
        Utility
    }

    /// <summary>
    /// Context provided to plugins
    /// </summary>
    public class PluginContext
    {
        public string PluginDataDirectory { get; set; } = "";
        public IPluginLogger Logger { get; set; } = null!;
        public IPluginSettingsManager Settings { get; set; } = null!;
        public IPluginModManager ModManager { get; set; } = null!;
        public IPluginThemeManager ThemeManager { get; set; } = null!;
        public string LuczyStrapVersion { get; set; } = "";
        public string RobloxVersion { get; set; } = "";
    }

    public interface IPluginLogger
    {
        void Log(string message);
        void LogWarning(string message);
        void LogError(string message);
        void LogException(Exception ex);
    }

    public interface IPluginSettingsManager
    {
        T? Get<T>(string key);
        void Set<T>(string key, T value);
        void Save();
    }

    public interface IPluginModManager
    {
        bool InstallMod(string modPath, string targetPath);
        bool RemoveMod(string targetPath);
        string GetModsDirectory();
    }

    public interface IPluginThemeManager
    {
        void ApplyCustomTheme(string themePath);
        void RegisterTheme(string themeId, string themePath);
        string GetCurrentTheme();
    }
}