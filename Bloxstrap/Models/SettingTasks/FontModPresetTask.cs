using Bloxstrap.Models.Entities;
using Bloxstrap.Models.SettingTasks.Base;

namespace Bloxstrap.Models.SettingTasks
{
    public class FontModPresetTask : EnumBaseTask<Enums.FontType>
    {
        private readonly Dictionary<Enums.FontType, string> _fontResourceMap = new()
        {
            { Enums.FontType.NotoSansThai, "Fonts.NotoSansThai-VariableFont_wdth,wght.ttf" },
            { Enums.FontType.Rubik, "Fonts.Rubik-VariableFont_wght.ttf" },
            { Enums.FontType.Accanthis, "Fonts.AccanthisADFStd.ttf" },
            { Enums.FontType.ArialBold, "Fonts.ArialBold.ttf" },
            { Enums.FontType.ComicSans, "Fonts.ComicSans.ttf" },
            { Enums.FontType.Gotham, "Fonts.Gotham.ttf" },
            { Enums.FontType.GothamBold, "Fonts.GothamBold.ttf" },
            { Enums.FontType.LegacyArial, "Fonts.LegacyArial.ttf" },
            { Enums.FontType.Roboto, "Fonts.Roboto.ttf" },
            { Enums.FontType.RobotoMono, "Fonts.RobotoMono.ttf" },
            { Enums.FontType.SourceSansPro, "Fonts.SourceSansPro.ttf" },
            { Enums.FontType.Arial, "Fonts.Arial.ttf" }
        };

        public string CustomFontPath { get; set; } = "";

        public string? GetFileHash()
        {
            if (!File.Exists(Paths.CustomFont))
                return null;

            using var fileStream = File.OpenRead(Paths.CustomFont);
            return MD5Hash.Stringify(App.MD5Provider.ComputeHash(fileStream));
        }

        public FontModPresetTask() : base("ModPreset", "TextFont")
        {
            // Check if a custom font exists
            if (File.Exists(Paths.CustomFont))
            {
                // Try to determine if it's a preset or custom font
                var currentHash = GetFileHash();
                bool isPreset = false;

                foreach (var preset in _fontResourceMap)
                {
                    var data = new ModPresetFileData(@"content\fonts\families\SourceSansPro.json", preset.Value);
                    
                    if (currentHash == MD5Hash.Stringify(data.ResourceHash))
                    {
                        OriginalState = preset.Key;
                        isPreset = true;
                        break;
                    }
                }

                if (!isPreset)
                {
                    OriginalState = Enums.FontType.Custom;
                    CustomFontPath = Paths.CustomFont;
                }
            }
        }

        public override void Execute()
        {
            if (NewState == Enums.FontType.Default)
            {
                // Remove custom font
                if (File.Exists(Paths.CustomFont))
                {
                    Filesystem.AssertReadOnly(Paths.CustomFont);
                    File.Delete(Paths.CustomFont);
                }
            }
            else if (NewState == Enums.FontType.Custom)
            {
                // Apply custom font
                if (!String.IsNullOrEmpty(CustomFontPath) && File.Exists(CustomFontPath))
                {
                    if (String.Compare(CustomFontPath, Paths.CustomFont, StringComparison.InvariantCultureIgnoreCase) != 0)
                    {
                        Directory.CreateDirectory(Path.GetDirectoryName(Paths.CustomFont)!);

                        Filesystem.AssertReadOnly(Paths.CustomFont);
                        File.Copy(CustomFontPath, Paths.CustomFont, true);
                    }
                }
            }
            else
            {
                // Apply preset font
                if (_fontResourceMap.TryGetValue(NewState, out string? resourceId))
                {
                    var data = new ModPresetFileData(@"content\fonts\families\SourceSansPro.json", resourceId);

                    Directory.CreateDirectory(Path.GetDirectoryName(Paths.CustomFont)!);

                    using var resourceStream = data.ResourceStream;
                    using var memoryStream = new MemoryStream();
                    resourceStream.CopyTo(memoryStream);

                    Filesystem.AssertReadOnly(Paths.CustomFont);
                    File.WriteAllBytes(Paths.CustomFont, memoryStream.ToArray());
                }
            }

            OriginalState = NewState;
        }
    }
}
