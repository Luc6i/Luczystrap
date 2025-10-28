namespace Bloxstrap.Plugins.PluginSDK
{
    /// <summary>
    /// Settings manager for persistent plugin configuration
    /// </summary>
    public interface IPluginSettingsManager
    {
        /// <summary>
        /// Get a setting value by key
        /// </summary>
        T? Get<T>(string key);

        /// <summary>
        /// Set a setting value by key
        /// </summary>
        void Set<T>(string key, T value);

        /// <summary>
        /// Save all settings to disk
        /// </summary>
        void Save();
    }
}