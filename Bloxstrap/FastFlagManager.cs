using Bloxstrap.Enums.FlagPresets;
using System.Security.Policy;
using System.Windows;
using System.Windows.Media.Animation;

namespace Bloxstrap
{
    public class FastFlagManager : JsonManager<Dictionary<string, object>>
    {
        public override string ClassName => nameof(FastFlagManager);

        public override string LOG_IDENT_CLASS => ClassName;

        public override string ProfilesLocation => Path.Combine(Paths.Base, "Profiles");

        public override string FileLocation => Path.Combine(Paths.Modifications, "ClientSettings\\ClientAppSettings.json");

        public bool Changed => !OriginalProp.SequenceEqual(Prop);

        public static IReadOnlyDictionary<string, string> PresetFlags = new Dictionary<string, string>
        {

            // Presets and stuff
            { "Rendering.ManualFullscreen", "FFlagHandleAltEnterFullscreenManually" },
            { "Rendering.DisableScaling", "DFFlagDisableDPIScale" },
            { "Rendering.MSAA", "FIntDebugForceMSAASamples" },
            { "Rendering.FRMQualityOverride", "DFIntDebugFRMQualityLevelOverride" },

            // Rendering engines
            { "Rendering.Mode.DisableD3D11", "FFlagDebugGraphicsDisableDirect3D11" },
            { "Rendering.Mode.D3D11", "FFlagDebugGraphicsPreferD3D11" },
            { "Rendering.Mode.Vulkan", "FFlagDebugGraphicsPreferVulkan" },
            { "Rendering.Mode.OpenGL", "FFlagDebugGraphicsPreferOpenGL" },

            // Geometry
            { "Geometry.MeshLOD.Static", "DFIntCSGLevelOfDetailSwitchingDistanceStatic" }, // this isnt actually a flag, we use it to determine current value, not the best way of doing that :sob:
            { "Geometry.MeshLOD.L0", "DFIntCSGLevelOfDetailSwitchingDistance" },
            { "Geometry.MeshLOD.L12", "DFIntCSGLevelOfDetailSwitchingDistanceL12" },
            { "Geometry.MeshLOD.L23", "DFIntCSGLevelOfDetailSwitchingDistanceL23" },
            { "Geometry.MeshLOD.L34", "DFIntCSGLevelOfDetailSwitchingDistanceL34" },

            // Mesh Distance (Render Distance)
            { "Geometry.MeshDistance.L0", "DFIntCSGLevelOfDetailSwitchingDistance" },
            { "Geometry.MeshDistance.L12", "DFIntCSGLevelOfDetailSwitchingDistanceL12" },
            { "Geometry.MeshDistance.L23", "DFIntCSGLevelOfDetailSwitchingDistanceL23" },
            { "Geometry.MeshDistance.L34", "DFIntCSGLevelOfDetailSwitchingDistanceL34" },

            // Texture quality
            { "Rendering.TextureQuality.OverrideEnabled", "DFFlagTextureQualityOverrideEnabled" },
            { "Rendering.TextureQuality.Level", "DFIntTextureQualityOverride" },
            
            // Remove Grass - ADD THESE THREE LINES
            { "Rendering.RemoveGrass1", "FIntFRMMinGrassDistance" },
            { "Rendering.RemoveGrass2", "FIntFRMMaxGrassDistance" },
            { "Rendering.RemoveGrass3", "FIntGrassMovementReducedMotionFactor" },
        };

        public static IReadOnlyDictionary<RenderingMode, string> RenderingModes => new Dictionary<RenderingMode, string>
        {
            { RenderingMode.Default, "None" },
            { RenderingMode.Vulkan, "Vulkan" },
            { RenderingMode.OpenGL, "OpenGL" },
            { RenderingMode.D3D11, "D3D11" },
        };

        public static IReadOnlyDictionary<MSAAMode, string?> MSAAModes => new Dictionary<MSAAMode, string?>
        {
            { MSAAMode.Default, null },
            { MSAAMode.x1, "1" },
            { MSAAMode.x2, "2" },
            { MSAAMode.x4, "4" }
        };

        public static IReadOnlyDictionary<TextureQuality, string?> TextureQualityLevels => new Dictionary<TextureQuality, string?>
        {
            { TextureQuality.Default, null },
            { TextureQuality.Level0, "0" },
            { TextureQuality.Level1, "1" },
            { TextureQuality.Level2, "2" },
            { TextureQuality.Level3, "3" },
        };

        public static IReadOnlyDictionary<MeshDistanceLevel, (string L0, string L12, string L23, string L34)> MeshDistanceLevels => new Dictionary<MeshDistanceLevel, (string, string, string, string)>
        {
            { MeshDistanceLevel.Default, (null!, null!, null!, null!) },
            { MeshDistanceLevel.Lowest, ("0", "0", "0", "0") },
            { MeshDistanceLevel.Low, ("2500", "1875", "1250", "625") },
            { MeshDistanceLevel.Medium, ("5000", "3750", "2500", "1250") },
            { MeshDistanceLevel.High, ("7500", "5625", "3750", "1875") },
            { MeshDistanceLevel.Ultra, ("10000", "7500", "5000", "2500") }
        };

        public static IReadOnlyDictionary<LuciPreset, Dictionary<string, object>> LuciPresets => new Dictionary<LuciPreset, Dictionary<string, object>>
        {
            { 
                LuciPreset.None, 
                new Dictionary<string, object>() 
            },
            { 
                LuciPreset.Potato, 
                new Dictionary<string, object>
                {
                    { "DFFlagDebugPauseVoxelizer", "True" },
                    { "DFFlagTextureQualityOverrideEnabled", "True" },
                    { "DFIntCSGLevelOfDetailSwitchingDistance", "0" },
                    { "DFIntCSGLevelOfDetailSwitchingDistanceL12", "0" },
                    { "DFIntCSGLevelOfDetailSwitchingDistanceL23", "0" },
                    { "DFIntCSGLevelOfDetailSwitchingDistanceL34", "0" },
                    { "DFIntDebugFRMQualityLevelOverride", "1" },
                    { "DFIntTextureQualityOverride", "0" },
                    { "FFlagDebugGraphicsPreferD3D11", "True" },
                    { "FFlagDebugSkyGray", "True" },
                    { "FFlagHandleAltEnterFullscreenManually", "False" },
                    { "FIntDebugForceMSAASamples", "1" },
                    { "FIntFRMMaxGrassDistance", "0" },
                    { "FIntFRMMinGrassDistance", "0" },
                    { "FIntGrassMovementReducedMotionFactor", "0" }
                }
            },
            { 
                LuciPreset.Low, 
                new Dictionary<string, object>
                {
                    { "DFFlagTextureQualityOverrideEnabled", "True" },
                    { "DFIntCSGLevelOfDetailSwitchingDistance", "2500" },
                    { "DFIntCSGLevelOfDetailSwitchingDistanceL12", "0" },
                    { "DFIntCSGLevelOfDetailSwitchingDistanceL23", "0" },
                    { "DFIntCSGLevelOfDetailSwitchingDistanceL34", "0" },
                    { "DFIntDebugFRMQualityLevelOverride", "1" },
                    { "DFIntTextureQualityOverride", "1" },
                    { "FFlagDebugGraphicsPreferD3D11", "True" },
                    { "FFlagDebugSkyGray", "True" },
                    { "FFlagHandleAltEnterFullscreenManually", "False" },
                    { "FIntDebugForceMSAASamples", "1" },
                    { "FIntFRMMaxGrassDistance", "0" },
                    { "FIntFRMMinGrassDistance", "0" },
                    { "FIntGrassMovementReducedMotionFactor", "0" }
                }
            },
            { 
                LuciPreset.Ultra, 
                new Dictionary<string, object>
                {
                    { "DFFlagDisableDPIScale", "True" },
                    { "DFFlagTextureQualityOverrideEnabled", "True" },
                    { "DFIntCSGLevelOfDetailSwitchingDistance", "10000" },
                    { "DFIntCSGLevelOfDetailSwitchingDistanceL12", "7500" },
                    { "DFIntCSGLevelOfDetailSwitchingDistanceL23", "5000" },
                    { "DFIntCSGLevelOfDetailSwitchingDistanceL34", "2500" },
                    { "DFIntDebugFRMQualityLevelOverride", "21" },
                    { "FFlagDebugGraphicsPreferD3D11", "True" },
                    { "FFlagHandleAltEnterFullscreenManually", "False" },
                    { "FIntDebugForceMSAASamples", "4" }
                }
            }
        };

        // all fflags are stored as strings
        // to delete a flag, set the value as null
        public void SetValue(string key, object? value)
        {
            const string LOG_IDENT = "FastFlagManager::SetValue";

            if (value is null)
            {
                if (Prop.ContainsKey(key))
                    App.Logger.WriteLine(LOG_IDENT, $"Deletion of '{key}' is pending");

                Prop.Remove(key);
            }
            else
            {
                if (Prop.ContainsKey(key))
                {
                    if (key == Prop[key].ToString())
                        return;

                    App.Logger.WriteLine(LOG_IDENT, $"Changing of '{key}' from '{Prop[key]}' to '{value}' is pending");
                }
                else
                {
                    App.Logger.WriteLine(LOG_IDENT, $"Setting of '{key}' to '{value}' is pending");
                }

                Prop[key] = value.ToString()!;
            }
        }

        // this returns null if the fflag doesn't exist
        public string? GetValue(string key)
        {
            // check if we have an updated change for it pushed first
            if (Prop.TryGetValue(key, out object? value) && value is not null)
                return value.ToString();

            return null;
        }

        public void SetPreset(string prefix, object? value)
        {
            foreach (var pair in PresetFlags.Where(x => x.Key.StartsWith(prefix)))
                SetValue(pair.Value, value);
        }

        public void SetPresetEnum(string prefix, string target, object? value)
        {
            foreach (var pair in PresetFlags.Where(x => x.Key.StartsWith(prefix)))
            {
                if (pair.Key.StartsWith($"{prefix}.{target}"))
                    SetValue(pair.Value, value);
                else
                    SetValue(pair.Value, null);
            }
        }

        public string? GetPreset(string name)
        {
            if (!PresetFlags.ContainsKey(name))
            {
                App.Logger.WriteLine("FastFlagManager::GetPreset", $"Could not find preset {name}");
                Debug.Assert(false, $"Could not find preset {name}");
                return null;
            }

            return GetValue(PresetFlags[name]);
        }

        public T GetPresetEnum<T>(IReadOnlyDictionary<T, string> mapping, string prefix, string value) where T : Enum
        {
            foreach (var pair in mapping)
            {
                if (pair.Value == "None")
                    continue;

                if (GetPreset($"{prefix}.{pair.Value}") == value)
                    return pair.Key;
            }

            return mapping.First().Key;
        }

        public bool IsPreset(string Flag) => PresetFlags.Values.Any(v => v.ToLower() == Flag.ToLower());

        public override void Save()
        {
            // convert all flag values to strings before saving

            foreach (var pair in Prop)
                Prop[pair.Key] = pair.Value.ToString()!;

            base.Save();

            // clone the dictionary
            OriginalProp = new(Prop);
        }

        public override void Load(bool alertFailure = true)
        {
            base.Load(alertFailure);

            // clone the dictionary
            OriginalProp = new(Prop);

            if (GetPreset("Rendering.ManualFullscreen") != "False")
                SetPreset("Rendering.ManualFullscreen", "False");
        }
    }
}
