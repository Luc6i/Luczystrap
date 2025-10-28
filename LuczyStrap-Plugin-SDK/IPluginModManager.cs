namespace Bloxstrap.Plugins.PluginSDK
{
    /// <summary>
    /// Manager for installing and removing Roblox mods
    /// </summary>
    public interface IPluginModManager
    {
        /// <summary>
        /// Install a mod file to Roblox modifications directory
        /// </summary>
        /// <param name="modPath">Full path to your mod file</param>
        /// <param name="targetPath">Relative path in Roblox (e.g., "content\textures\cursor.png")</param>
        /// <returns>True if successful</returns>
        bool InstallMod(string modPath, string targetPath);

        /// <summary>
        /// Remove a previously installed mod
        /// </summary>
        /// <param name="targetPath">Relative path in Roblox</param>
        /// <returns>True if successful</returns>
        bool RemoveMod(string targetPath);

        /// <summary>
        /// Get the Roblox modifications directory path
        /// </summary>
        string GetModsDirectory();
    }
}