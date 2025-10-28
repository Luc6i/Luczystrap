# LuczyStrap Plugin SDK

Welcome to the **LuczyStrap Modding Wiki** — your all-in-one guide to creating plugins for **LuczyStrap** without needing access to the source code!

---

## 📦 What's Included

* 7 interface files (`.cs`) that define the plugin system
* Example plugin for quick setup
* Full setup & usage guide

---

## 🚀 Quick Start (5 Steps)

### **Step 1: Install .NET 8**

Download and install the latest version of .NET 8:
👉 [https://dotnet.microsoft.com/download/dotnet/8.0](https://dotnet.microsoft.com/download/dotnet/8.0)

---

### **Step 2: Create a New Plugin Project**

Open **Command Prompt** and run:

```bash
dotnet new classlib -n MyPlugin -f net8.0
cd MyPlugin
```

---

### **Step 3: Add SDK Files**

Copy all **7 `.cs` files** from the SDK folder into your new `MyPlugin` project folder.

---

### **Step 4: Create Your Plugin Class**

Create a new file named `MyPlugin.cs` and paste the following code:

```csharp
using System.Threading.Tasks;
using Bloxstrap.Plugins.PluginSDK;

namespace MyPlugin
{
    public class MyPlugin : ILuczyStrapPlugin
    {
        public PluginMetadata Metadata => new PluginMetadata
        {
            Id = "my-plugin",
            Name = "My First Plugin",
            Description = "A test plugin",
            Author = "Your Name",
            Version = "1.0.0",
            Type = PluginType.Utility
        };

        private IPluginLogger? _logger;

        public Task<bool> OnLoadAsync(PluginContext context)
        {
            _logger = context.Logger;
            _logger.Log("✅ Plugin loaded!");
            return Task.FromResult(true);
        }

        public Task OnUnloadAsync()
        {
            _logger?.Log("👋 Plugin unloaded");
            return Task.CompletedTask;
        }

        public Task OnRobloxLaunchAsync()
        {
            _logger?.Log("🚀 Roblox launching!");
            return Task.CompletedTask;
        }

        public Task OnRobloxCloseAsync()
        {
            _logger?.Log("❌ Roblox closed");
            return Task.CompletedTask;
        }
    }
}
```

---

### **Step 5: Create `plugin.json`**

In your project folder, create a `plugin.json` file with this content:

```json
{
  "id": "my-plugin",
  "name": "My First Plugin",
  "description": "A test plugin",
  "author": "Your Name",
  "version": "1.0.0",
  "type": "Utility",
  "tags": [],
  "requiresRestart": false,
  "isTrusted": false
}
```

---

### **Step 6: Build Your Plugin**

Compile your plugin using:

```bash
dotnet build -c Release
```

The resulting DLL will appear in:

```
bin\Release\net8.0\MyPlugin.dll
```

---

### **Step 7: Install Plugin**

1. Copy these two files to:

   ```
   %LocalAppData%\Luczystrap\Plugins\my-plugin\
   ```

   * `MyPlugin.dll` *(rename to `my-plugin.dll`)*
   * `plugin.json`

2. Restart **LuczyStrap**

3. Open: **Mods → Miscellaneous → Plugin Manager**

4. Find your plugin and click **Enable!** ✅

---

## ⚙️ Plugin Features

### **Logging**

Use the built-in logger to print messages to the console or debug output.

```csharp
_logger.Log("Plugin started successfully!");
```

### **Settings (Save Data)**

Save and load persistent plugin settings easily:

```csharp
context.Settings.Set("myKey", "myValue");
string value = context.Settings.Get<string>("myKey");
context.Settings.Save();
```

### **Mod Installation (File Overrides)**

Replace Roblox content dynamically:

```csharp
// Install a custom cursor
context.ModManager.InstallMod(
    "C:\\path\\to\\cursor.png",
    @"content\\textures\\Cursors\\KeyboardMouse\\ArrowCursor.png"
);

// Remove the mod
context.ModManager.RemoveMod(@"content\\textures\\Cursors\\KeyboardMouse\\ArrowCursor.png");
```

---

## 🎨 Common Roblox File Paths

| File                                                         | Description   |
| ------------------------------------------------------------ | ------------- |
| `content\\textures\\Cursors\\KeyboardMouse\\ArrowCursor.png` | Mouse cursor  |
| `content\\sounds\\action_jump.mp3`                           | Jump sound    |
| `content\\textures\\ui\\Logo.png`                            | Roblox logo   |
| `content\\sounds\\action_footsteps_plastic.mp3`              | Walking sound |

---

## 📤 Share Your Plugin

1. Build your plugin
2. Create a `.zip` containing:

   * `your-plugin.dll`
   * `plugin.json`
   * `README.md` *(optional)*
3. Share it on our Discord:
   👉 [https://discord.gg/C2rkCMkc9c](https://discord.gg/C2rkCMkc9c)
4. Post it in the **#mods** channel:
   [MODS Channel](https://discord.com/channels/1429853228673007747/1432746269146878044)

---

## 🧩 Need Help?

* 💬 Discord: [https://discord.gg/C2rkCMkc9c](https://discord.gg/C2rkCMkc9c)
* 🐞 Report Issues: [https://github.com/Luc6i/Luczystrap_asset/issues](https://github.com/Luc6i/Luczystrap_asset/issues)

---

**Made with ❤️ by the LuczyStrap Community**
