using System.Windows.Input;

using CommunityToolkit.Mvvm.Input;

using Bloxstrap.Enums.FlagPresets;
using System.Windows;
using Bloxstrap.UI.Elements.Settings.Pages;
using Wpf.Ui.Mvvm.Contracts;
using System.Windows.Documents;
using System.Windows.Controls;
using System.Windows.Navigation;

namespace Bloxstrap.UI.ViewModels.Settings
{
    public class FastFlagsViewModel : NotifyPropertyChangedViewModel
    {
        private Dictionary<string, object>? _preResetFlags;
        private Page? _page;

        public event EventHandler? RequestPageReloadEvent;
        
        public event EventHandler? OpenFlagEditorEvent;

        public FastFlagsViewModel()
        {
        }

        public FastFlagsViewModel(Page page)
        {
            _page = page;
        }

        private void OpenFastFlagEditor() => OpenFlagEditorEvent?.Invoke(this, EventArgs.Empty);

        public ICommand OpenFastFlagEditorCommand => new RelayCommand(OpenFastFlagEditor);

        public bool UseFastFlagManager
        {
            get => App.Settings.Prop.UseFastFlagManager;
            set => App.Settings.Prop.UseFastFlagManager = value;
        }

        public IReadOnlyDictionary<MSAAMode, string?> MSAALevels => FastFlagManager.MSAAModes;

        public MSAAMode SelectedMSAALevel
        {
            get => MSAALevels.FirstOrDefault(x => x.Value == App.FastFlags.GetPreset("Rendering.MSAA")).Key;
            set => App.FastFlags.SetPreset("Rendering.MSAA", MSAALevels[value]);
        }

        public IReadOnlyDictionary<RenderingMode, string> RenderingModes => FastFlagManager.RenderingModes;

        public RenderingMode SelectedRenderingMode
        {
            get => App.FastFlags.GetPresetEnum(RenderingModes, "Rendering.Mode", "True");
            set
            {
                RenderingMode[] DisableD3D11 = new RenderingMode[]
                {
                    RenderingMode.Vulkan,
                    RenderingMode.OpenGL
                };

                App.FastFlags.SetPresetEnum("Rendering.Mode", value.ToString(), "True");
                App.FastFlags.SetPreset("Rendering.Mode.DisableD3D11", DisableD3D11.Contains(value) ? "True" : null);
            }
        }

        public bool FixDisplayScaling
        {
            get => App.FastFlags.GetPreset("Rendering.DisableScaling") == "True";
            set => App.FastFlags.SetPreset("Rendering.DisableScaling", value ? "True" : null);
        }

        public IReadOnlyDictionary<TextureQuality, string?> TextureQualities => FastFlagManager.TextureQualityLevels;

        public TextureQuality SelectedTextureQuality
        {
            get => TextureQualities.Where(x => x.Value == App.FastFlags.GetPreset("Rendering.TextureQuality.Level")).FirstOrDefault().Key;
            set
            {
                if (value == TextureQuality.Default)
                {
                    App.FastFlags.SetPreset("Rendering.TextureQuality", null);
                }
                else
                {
                    App.FastFlags.SetPreset("Rendering.TextureQuality.OverrideEnabled", "True");
                    App.FastFlags.SetPreset("Rendering.TextureQuality.Level", TextureQualities[value]);
                }
            }
        }

        private static readonly string[] LODLevels = { "L0", "L12", "L23", "L34" };

        public bool FRMQualityOverrideEnabled
        {
            get => App.FastFlags.GetPreset("Rendering.FRMQualityOverride") != null;
            set
            {
                if (value)
                    FRMQualityOverride = 21;
                else
                    App.FastFlags.SetPreset("Rendering.FRMQualityOverride", null);

                OnPropertyChanged(nameof(FRMQualityOverride));
                OnPropertyChanged(nameof(FRMQualityOverrideEnabled));
            }
        }

        public int FRMQualityOverride
        {
            get => int.TryParse(App.FastFlags.GetPreset("Rendering.FRMQualityOverride"), out var x) ? x : 21;
            set
            {
                App.FastFlags.SetPreset("Rendering.FRMQualityOverride", value);

                OnPropertyChanged(nameof(FRMQualityOverride));
            }
        }

        public bool MeshQualityEnabled
        {
            get => App.FastFlags.GetPreset("Geometry.MeshLOD.Static") != null;
            set
            {
                if (value)
                {
                    // we enable level 3 by default
                    MeshQuality = 3;
                }
                else
                {
                    foreach (string level in LODLevels)
                        App.FastFlags.SetPreset($"Geometry.MeshLOD.{level}", null);

                    App.FastFlags.SetPreset("Geometry.MeshLOD.Static", null);
                }

                OnPropertyChanged(nameof(MeshQualityEnabled));
            }
        }

        public bool RemoveGrass
        {
            get => App.FastFlags?.GetPreset("Rendering.RemoveGrass1") == "0";
            set
            {
                App.FastFlags.SetPreset("Rendering.RemoveGrass1", value ? "0" : null);
                App.FastFlags.SetPreset("Rendering.RemoveGrass2", value ? "0" : null);
                App.FastFlags.SetPreset("Rendering.RemoveGrass3", value ? "0" : null);
            }
        }

        public int MeshQuality
        {
            get => int.TryParse(App.FastFlags.GetPreset("Geometry.MeshLOD.Static"), out var x) ? x : 0;
            set
            {
                int clamped = Math.Clamp(value, 0, LODLevels.Length - 1);

                for (int i = 0; i < LODLevels.Length; i++)
                {
                    int lodValue = Math.Clamp(clamped - i, 0, 3);
                    string lodLevel = LODLevels[i];

                    App.FastFlags.SetPreset($"Geometry.MeshLOD.{lodLevel}", lodValue);
                }

                App.FastFlags.SetPreset("Geometry.MeshLOD.Static", clamped);
                OnPropertyChanged(nameof(MeshQuality));
                OnPropertyChanged(nameof(MeshQualityEnabled));
            }
        }

        public IReadOnlyDictionary<MeshDistanceLevel, (string L0, string L12, string L23, string L34)> MeshDistanceLevels => FastFlagManager.MeshDistanceLevels;

        public MeshDistanceLevel SelectedMeshDistanceLevel
        {
            get
            {
                string? l0 = App.FastFlags.GetPreset("Geometry.MeshDistance.L0");
                string? l12 = App.FastFlags.GetPreset("Geometry.MeshDistance.L12");
                string? l23 = App.FastFlags.GetPreset("Geometry.MeshDistance.L23");
                string? l34 = App.FastFlags.GetPreset("Geometry.MeshDistance.L34");

                // If any value is null, return Default
                if (l0 == null || l12 == null || l23 == null || l34 == null)
                    return MeshDistanceLevel.Default;

                // Find matching level
                foreach (var level in MeshDistanceLevels)
                {
                    if (level.Key == MeshDistanceLevel.Default)
                        continue;

                    if (level.Value.L0 == l0 && level.Value.L12 == l12 && 
                        level.Value.L23 == l23 && level.Value.L34 == l34)
                        return level.Key;
                }

                return MeshDistanceLevel.Default;
            }
            set
            {
                if (value == MeshDistanceLevel.Default)
                {
                    App.FastFlags.SetPreset("Geometry.MeshDistance.L0", null);
                    App.FastFlags.SetPreset("Geometry.MeshDistance.L12", null);
                    App.FastFlags.SetPreset("Geometry.MeshDistance.L23", null);
                    App.FastFlags.SetPreset("Geometry.MeshDistance.L34", null);
                }
                else
                {
                    var distances = MeshDistanceLevels[value];
                    App.FastFlags.SetPreset("Geometry.MeshDistance.L0", distances.L0);
                    App.FastFlags.SetPreset("Geometry.MeshDistance.L12", distances.L12);
                    App.FastFlags.SetPreset("Geometry.MeshDistance.L23", distances.L23);
                    App.FastFlags.SetPreset("Geometry.MeshDistance.L34", distances.L34);
                }

                OnPropertyChanged(nameof(SelectedMeshDistanceLevel));
            }
        }

        public IReadOnlyDictionary<LuciPreset, Dictionary<string, object>> LuciPresets => FastFlagManager.LuciPresets;

        public ICommand ApplyLuciPresetCommand => new RelayCommand<LuciPreset>(ApplyLuciPreset);

        private void ApplyLuciPreset(LuciPreset preset)
        {
            if (preset == LuciPreset.None)
                return;

            var presetFlags = LuciPresets[preset];

            foreach (var flag in presetFlags)
            {
                App.FastFlags.SetValue(flag.Key, flag.Value);
            }

            // Navigate to the FastFlag editor
            if (_page != null && Window.GetWindow(_page) is INavigationWindow window)
            {
                window.Navigate(typeof(FastFlagEditorPage));
            }
        }

        public ICommand RunBenchmarkCommand => new RelayCommand(RunBenchmark);

        private async void RunBenchmark()
        {
            // Run benchmark on a background thread to avoid blocking UI
            await Task.Run(() =>
            {
                System.Threading.Thread.Sleep(500); // Small delay to show the process
            });

            var (score, tier, preset) = Bloxstrap.Utility.PerformanceBenchmark.GetDetailedBenchmark();

            var presetName = preset switch
            {
                LuciPreset.Potato => "Potato (Low-End)",
                LuciPreset.Low => "Low (Mid-End)",
                LuciPreset.Ultra => "Ultra (High-End)",
                _ => "Unknown"
            };

            var message = $"Benchmark Complete!\n\n" +
                         $"Performance Score: {score}/100\n" +
                         $"System Tier: {tier}\n" +
                         $"Recommended Preset: {presetName}\n\n" +
                         $"Would you like to apply the recommended preset?";

            var result = Frontend.ShowMessageBox(
                message,
                MessageBoxImage.Information,
                MessageBoxButton.YesNo
            );

            if (result == MessageBoxResult.Yes)
            {
                ApplyLuciPreset(preset);
            }
        }

        public bool ResetConfiguration
        {
            get => _preResetFlags is not null;

            set
            {
                if (value)
                {
                    _preResetFlags = new(App.FastFlags.Prop);
                    App.FastFlags.Prop.Clear();
                }
                else
                {
                    App.FastFlags.Prop = _preResetFlags!;
                    _preResetFlags = null;
                }

                RequestPageReloadEvent?.Invoke(this, EventArgs.Empty);
            }
        }
    }
}
