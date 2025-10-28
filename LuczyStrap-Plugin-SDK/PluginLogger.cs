using System;
using Bloxstrap.Plugins.PluginSDK;

namespace Bloxstrap.Plugins
{
    internal class PluginLogger : IPluginLogger
    {
        private readonly string _pluginId;

        public PluginLogger(string pluginId)
        {
            _pluginId = pluginId;
        }

        public void Log(string message)
        {
            App.Logger.WriteLine($"Plugin:{_pluginId}", message);
        }

        public void LogWarning(string message)
        {
            App.Logger.WriteLine($"Plugin:{_pluginId}", $"[WARNING] {message}");
        }

        public void LogError(string message)
        {
            App.Logger.WriteLine($"Plugin:{_pluginId}", $"[ERROR] {message}");
        }

        public void LogException(Exception ex)
        {
            App.Logger.WriteException($"Plugin:{_pluginId}", ex);
        }
    }
}