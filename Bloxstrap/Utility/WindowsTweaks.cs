using Microsoft.Win32;
using System.Diagnostics;
using System.Net.NetworkInformation;

namespace Bloxstrap.Utility
{
    internal static class WindowsTweaks
    {
        private const string LOG_IDENT = "WindowsTweaks";

        /// <summary>
        /// Sets RobloxPlayerBeta.exe process priority to High
        /// This will be applied when Roblox launches
        /// </summary>
        public static void SetHighPriority(Process process)
        {
            try
            {
                process.PriorityClass = ProcessPriorityClass.High;
                App.Logger.WriteLine(LOG_IDENT, "Set process priority to High");
            }
            catch (Exception ex)
            {
                App.Logger.WriteLine(LOG_IDENT, "Failed to set high priority");
                App.Logger.WriteException(LOG_IDENT, ex);
            }
        }

        /// <summary>
        /// Activates Ultimate Performance power plan
        /// </summary>
        public static void SetUltimatePerformancePlan()
        {
            const string ultimateGuid = "e9a42b02-d5df-448d-aa00-03f14749eb61";
            
            try
            {
                // First, enable the Ultimate Performance plan (it's hidden by default)
                var enableProcess = Process.Start(new ProcessStartInfo
                {
                    FileName = "powercfg",
                    Arguments = $"/duplicatescheme {ultimateGuid}",
                    UseShellExecute = false,
                    CreateNoWindow = true,
                    WindowStyle = ProcessWindowStyle.Hidden
                });
                enableProcess?.WaitForExit();

                // Then activate it
                var activateProcess = Process.Start(new ProcessStartInfo
                {
                    FileName = "powercfg",
                    Arguments = $"/setactive {ultimateGuid}",
                    UseShellExecute = false,
                    CreateNoWindow = true,
                    WindowStyle = ProcessWindowStyle.Hidden
                });
                activateProcess?.WaitForExit();

                App.Logger.WriteLine(LOG_IDENT, "Ultimate Performance power plan activated");
            }
            catch (Exception ex)
            {
                App.Logger.WriteLine(LOG_IDENT, "Failed to set Ultimate Performance plan");
                App.Logger.WriteException(LOG_IDENT, ex);
            }
        }

        /// <summary>
        /// Disables CPU core parking
        /// </summary>
        public static void DisableCoreParking()
        {
            try
            {
                using var key = Registry.LocalMachine.OpenSubKey(
                    @"SYSTEM\CurrentControlSet\Control\Power\PowerSettings\54533251-82be-4824-96c1-47b60b740d00\0cc5b647-c1df-4637-891a-dec35c318583",
                    true);
                
                if (key != null)
                {
                    key.SetValue("ValueMax", 0, RegistryValueKind.DWord);
                    App.Logger.WriteLine(LOG_IDENT, "Disabled core parking");
                }
            }
            catch (Exception ex)
            {
                App.Logger.WriteLine(LOG_IDENT, "Failed to disable core parking");
                App.Logger.WriteException(LOG_IDENT, ex);
            }
        }

        /// <summary>
        /// Re-enables CPU core parking
        /// </summary>
        public static void EnableCoreParking()
        {
            try
            {
                using var key = Registry.LocalMachine.OpenSubKey(
                    @"SYSTEM\CurrentControlSet\Control\Power\PowerSettings\54533251-82be-4824-96c1-47b60b740d00\0cc5b647-c1df-4637-891a-dec35c318583",
                    true);
                
                if (key != null)
                {
                    key.SetValue("ValueMax", 100, RegistryValueKind.DWord);
                    App.Logger.WriteLine(LOG_IDENT, "Enabled core parking");
                }
            }
            catch (Exception ex)
            {
                App.Logger.WriteLine(LOG_IDENT, "Failed to enable core parking");
                App.Logger.WriteException(LOG_IDENT, ex);
            }
        }

        /// <summary>
        /// Disables GPU telemetry
        /// </summary>
        public static void DisableGpuTelemetry()
        {
            try
            {
                // Disable NVIDIA telemetry
                using var nvidiaKey = Registry.LocalMachine.CreateSubKey(
                    @"SOFTWARE\NVIDIA Corporation\NvControlPanel2\Client");
                nvidiaKey?.SetValue("OptInOrOutPreference", 0, RegistryValueKind.DWord);

                // Disable AMD telemetry
                using var amdKey = Registry.LocalMachine.CreateSubKey(
                    @"SOFTWARE\AMD\CN");
                amdKey?.SetValue("AutoUpdateTriggered", 0, RegistryValueKind.DWord);

                App.Logger.WriteLine(LOG_IDENT, "Disabled GPU telemetry");
            }
            catch (Exception ex)
            {
                App.Logger.WriteLine(LOG_IDENT, "Failed to disable GPU telemetry");
                App.Logger.WriteException(LOG_IDENT, ex);
            }
        }

        /// <summary>
        /// Re-enables GPU telemetry
        /// </summary>
        public static void EnableGpuTelemetry()
        {
            try
            {
                // Enable NVIDIA telemetry
                using var nvidiaKey = Registry.LocalMachine.CreateSubKey(
                    @"SOFTWARE\NVIDIA Corporation\NvControlPanel2\Client");
                nvidiaKey?.SetValue("OptInOrOutPreference", 1, RegistryValueKind.DWord);

                // Enable AMD telemetry
                using var amdKey = Registry.LocalMachine.CreateSubKey(
                    @"SOFTWARE\AMD\CN");
                amdKey?.SetValue("AutoUpdateTriggered", 1, RegistryValueKind.DWord);

                App.Logger.WriteLine(LOG_IDENT, "Enabled GPU telemetry");
            }
            catch (Exception ex)
            {
                App.Logger.WriteLine(LOG_IDENT, "Failed to enable GPU telemetry");
                App.Logger.WriteException(LOG_IDENT, ex);
            }
        }

        /// <summary>
        /// Optimizes network traffic settings
        /// </summary>
        public static void OptimizeNetworkTraffic()
        {
            try
            {
                var interfaces = NetworkInterface.GetAllNetworkInterfaces()
                    .Where(i => i.OperationalStatus == OperationalStatus.Up &&
                               (i.NetworkInterfaceType == NetworkInterfaceType.Ethernet ||
                                i.NetworkInterfaceType == NetworkInterfaceType.Wireless80211));

                foreach (var iface in interfaces)
                {
                    string interfaceId = iface.Id;
                    
                    // Disable power management
                    using var powerKey = Registry.LocalMachine.OpenSubKey(
                        $@"SYSTEM\CurrentControlSet\Control\Class\{{4d36e972-e325-11ce-bfc1-08002be10318}}\{interfaceId}",
                        true);
                    
                    if (powerKey != null)
                    {
                        powerKey.SetValue("*WakeOnMagicPacket", "0", RegistryValueKind.String);
                        powerKey.SetValue("*WakeOnPattern", "0", RegistryValueKind.String);
                        powerKey.SetValue("PnPCapabilities", 24, RegistryValueKind.DWord);
                    }

                    // Enable QoS packet scheduler
                    var qosProcess = Process.Start(new ProcessStartInfo
                    {
                        FileName = "netsh",
                        Arguments = "int tcp set global autotuninglevel=normal",
                        UseShellExecute = false,
                        CreateNoWindow = true,
                        WindowStyle = ProcessWindowStyle.Hidden
                    });
                    qosProcess?.WaitForExit();
                }

                App.Logger.WriteLine(LOG_IDENT, "Optimized network traffic settings");
            }
            catch (Exception ex)
            {
                App.Logger.WriteLine(LOG_IDENT, "Failed to optimize network traffic");
                App.Logger.WriteException(LOG_IDENT, ex);
            }
        }

        /// <summary>
        /// Sets DNS to Cloudflare (1.1.1.1)
        /// </summary>
        public static void SetCloudflareDNS()
        {
            try
            {
                var interfaces = NetworkInterface.GetAllNetworkInterfaces()
                    .Where(i => i.OperationalStatus == OperationalStatus.Up &&
                               (i.NetworkInterfaceType == NetworkInterfaceType.Ethernet ||
                                i.NetworkInterfaceType == NetworkInterfaceType.Wireless80211));

                foreach (var iface in interfaces)
                {
                    string interfaceName = iface.Name;
                    
                    // Set IPv4 DNS
                    var ipv4Process = Process.Start(new ProcessStartInfo
                    {
                        FileName = "netsh",
                        Arguments = $"interface ipv4 set dns name=\"{interfaceName}\" static 1.1.1.1 primary",
                        UseShellExecute = false,
                        CreateNoWindow = true,
                        WindowStyle = ProcessWindowStyle.Hidden
                    });
                    ipv4Process?.WaitForExit();

                    // Add secondary IPv4 DNS
                    var ipv4SecondaryProcess = Process.Start(new ProcessStartInfo
                    {
                        FileName = "netsh",
                        Arguments = $"interface ipv4 add dns name=\"{interfaceName}\" 1.0.0.1 index=2",
                        UseShellExecute = false,
                        CreateNoWindow = true,
                        WindowStyle = ProcessWindowStyle.Hidden
                    });
                    ipv4SecondaryProcess?.WaitForExit();

                    // Set IPv6 DNS
                    var ipv6Process = Process.Start(new ProcessStartInfo
                    {
                        FileName = "netsh",
                        Arguments = $"interface ipv6 set dns name=\"{interfaceName}\" static 2606:4700:4700::1111",
                        UseShellExecute = false,
                        CreateNoWindow = true,
                        WindowStyle = ProcessWindowStyle.Hidden
                    });
                    ipv6Process?.WaitForExit();

                    // Add secondary IPv6 DNS
                    var ipv6SecondaryProcess = Process.Start(new ProcessStartInfo
                    {
                        FileName = "netsh",
                        Arguments = $"interface ipv6 add dns name=\"{interfaceName}\" 2606:4700:4700::1001 index=2",
                        UseShellExecute = false,
                        CreateNoWindow = true,
                        WindowStyle = ProcessWindowStyle.Hidden
                    });
                    ipv6SecondaryProcess?.WaitForExit();
                }

                App.Logger.WriteLine(LOG_IDENT, "Set Cloudflare DNS (1.1.1.1)");
            }
            catch (Exception ex)
            {
                App.Logger.WriteLine(LOG_IDENT, "Failed to set Cloudflare DNS");
                App.Logger.WriteException(LOG_IDENT, ex);
            }
        }

        /// <summary>
        /// Resets DNS to automatic (DHCP)
        /// </summary>
        public static void ResetDNS()
        {
            try
            {
                var interfaces = NetworkInterface.GetAllNetworkInterfaces()
                    .Where(i => i.OperationalStatus == OperationalStatus.Up &&
                               (i.NetworkInterfaceType == NetworkInterfaceType.Ethernet ||
                                i.NetworkInterfaceType == NetworkInterfaceType.Wireless80211));

                foreach (var iface in interfaces)
                {
                    string interfaceName = iface.Name;
                    
                    // Reset IPv4 DNS to DHCP
                    var ipv4Process = Process.Start(new ProcessStartInfo
                    {
                        FileName = "netsh",
                        Arguments = $"interface ipv4 set dns name=\"{interfaceName}\" dhcp",
                        UseShellExecute = false,
                        CreateNoWindow = true,
                        WindowStyle = ProcessWindowStyle.Hidden
                    });
                    ipv4Process?.WaitForExit();

                    // Reset IPv6 DNS to DHCP
                    var ipv6Process = Process.Start(new ProcessStartInfo
                    {
                        FileName = "netsh",
                        Arguments = $"interface ipv6 set dns name=\"{interfaceName}\" dhcp",
                        UseShellExecute = false,
                        CreateNoWindow = true,
                        WindowStyle = ProcessWindowStyle.Hidden
                    });
                    ipv6Process?.WaitForExit();
                }

                App.Logger.WriteLine(LOG_IDENT, "Reset DNS to automatic (DHCP)");
            }
            catch (Exception ex)
            {
                App.Logger.WriteLine(LOG_IDENT, "Failed to reset DNS");
                App.Logger.WriteException(LOG_IDENT, ex);
            }
        }
    }
}