using System;
using System.Threading.Tasks;
using Bloxstrap.Plugins.PluginSDK;

namespace ExamplePlugin
{
    public class ExamplePlugin : ILuczyStrapPlugin
    {
        public PluginMetadata Metadata => new PluginMetadata
        {
            Id = "example-plugin",
            Name = "Example Plugin",
            Description = "A simple test plugin that logs messages",
            Author = "LuczyStrap Team",
            Version = "1.0.0",
            Type = PluginType.Utility,
            Tags = new[] { "example", "test" }
        };

        private IPluginLogger? _logger;

        public Task<bool> OnLoadAsync(PluginContext context)
        {
            _logger = context.Logger;
            _logger.Log("🎉 Example plugin loaded!");
            
            // Count how many times loaded
            int count = context.Settings.Get<int>("LoadCount") ?? 0;
            count++;
            context.Settings.Set("LoadCount", count);
            context.Settings.Save();
            
            _logger.Log($"This plugin has been loaded {count} time(s)!");
            
            return Task.FromResult(true);
        }

        public Task OnUnloadAsync()
        {
            _logger?.Log("👋 Plugin unloaded");
            return Task.CompletedTask;
        }

        public Task OnRobloxLaunchAsync()
        {
            _logger?.Log("🚀 Roblox is launching!");
            return Task.CompletedTask;
        }

        public Task OnRobloxCloseAsync()
        {
            _logger?.Log("❌ Roblox closed");
            return Task.CompletedTask;
        }
    }
}