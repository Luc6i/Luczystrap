using System.Windows.Input;
using CommunityToolkit.Mvvm.Input;
using Bloxstrap.Utility;

namespace Bloxstrap.UI.ViewModels.Settings
{
    public class WindowsTweaksViewModel : NotifyPropertyChangedViewModel
    {
        // Performance Tweaks
        public bool ForceHighPriority
        {
            get => App.Settings.Prop.ForceHighPriority;
            set
            {
                App.Settings.Prop.ForceHighPriority = value;
                // This will be applied when Roblox launches
            }
        }

        public bool SetUltimatePerformance
        {
            get => App.Settings.Prop.SetUltimatePerformance;
            set
            {
                App.Settings.Prop.SetUltimatePerformance = value;
                
                if (value)
                    WindowsTweaks.SetUltimatePerformancePlan();
            }
        }

        // System Tweaks
        public bool DisableCoreParking
        {
            get => App.Settings.Prop.DisableCoreParking;
            set
            {
                App.Settings.Prop.DisableCoreParking = value;
                
                if (value)
                    WindowsTweaks.DisableCoreParking();
                else
                    WindowsTweaks.EnableCoreParking();
            }
        }

        public bool DisableGpuTelemetry
        {
            get => App.Settings.Prop.DisableGpuTelemetry;
            set
            {
                App.Settings.Prop.DisableGpuTelemetry = value;
                
                if (value)
                    WindowsTweaks.DisableGpuTelemetry();
                else
                    WindowsTweaks.EnableGpuTelemetry();
            }
        }

        public bool OptimizeNetworkTraffic
        {
            get => App.Settings.Prop.OptimizeNetworkTraffic;
            set
            {
                App.Settings.Prop.OptimizeNetworkTraffic = value;
                
                if (value)
                    WindowsTweaks.OptimizeNetworkTraffic();
            }
        }

        public bool SetCloudflareDNS
        {
            get => App.Settings.Prop.SetCloudflareDNS;
            set
            {
                App.Settings.Prop.SetCloudflareDNS = value;
                
                if (value)
                    WindowsTweaks.SetCloudflareDNS();
                else
                    WindowsTweaks.ResetDNS();
            }
        }
    }
}