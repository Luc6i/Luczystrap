using System;

namespace Bloxstrap.Plugins.PluginSDK
{
    /// <summary>
    /// Plugin metadata displayed in Plugin Manager
    /// </summary>
    public class PluginMetadata
    {
        /// <summary>
        /// Unique plugin ID (e.g., "my-cursor-plugin")
        /// Must match folder name and DLL name
        /// </summary>
        public string Id { get; set; } = "";

        /// <summary>
        /// Display name shown in Plugin Manager
        /// </summary>
        public string Name { get; set; } = "";

        /// <summary>
        /// Brief description of plugin functionality
        /// </summary>
        public string Description { get; set; } = "";

        /// <summary>
        /// Plugin author name
        /// </summary>
        public string Author { get; set; } = "";

        /// <summary>
        /// Plugin version (e.g., "1.0.0")
        /// </summary>
        public string Version { get; set; } = "1.0.0";

        /// <summary>
        /// Plugin category
        /// </summary>
        public PluginType Type { get; set; }

        /// <summary>
        /// Plugin IDs this plugin depends on
        /// </summary>
        public string[] Dependencies { get; set; } = Array.Empty<string>();

        /// <summary>
        /// URL to plugin icon image
        /// </summary>
        public string IconUrl { get; set; } = "";

        /// <summary>
        /// URL where users can download this plugin
        /// </summary>
        public string DownloadUrl { get; set; } = "";

        /// <summary>
        /// Plugin creation date
        /// </summary>
        public DateTime CreatedDate { get; set; }

        /// <summary>
        /// Last update date
        /// </summary>
        public DateTime LastUpdated { get; set; }

        /// <summary>
        /// Download count (populated by marketplace)
        /// </summary>
        public int Downloads { get; set; }

        /// <summary>
        /// User rating (0-5 stars)
        /// </summary>
        public double Rating { get; set; }

        /// <summary>
        /// Tags for categorization (e.g., "cursor", "theme")
        /// </summary>
        public string[] Tags { get; set; } = Array.Empty<string>();

        /// <summary>
        /// Whether LuczyStrap needs to restart after enabling
        /// </summary>
        public bool RequiresRestart { get; set; }

        /// <summary>
        /// Whether plugin is verified by LuczyStrap team
        /// </summary>
        public bool IsTrusted { get; set; }
    }

    /// <summary>
    /// Plugin type categories
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
}