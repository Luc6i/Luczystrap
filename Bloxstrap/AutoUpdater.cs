using System.Diagnostics;
using System.IO;
using System.Net.Http;
using System.Windows;

namespace Bloxstrap
{
    internal static class AutoUpdater
    {
        private const string LOG_IDENT = "AutoUpdater";

        public static async Task<bool> CheckForUpdates()
        {
            if (!App.Settings.Prop.CheckForUpdates)
            {
                App.Logger.WriteLine(LOG_IDENT, "Update checking is disabled");
                return false;
            }

            App.Logger.WriteLine(LOG_IDENT, "Checking for updates...");

            try
            {
                var latestRelease = await App.GetLatestRelease();

                if (latestRelease is null)
                {
                    App.Logger.WriteLine(LOG_IDENT, "Failed to get latest release information");
                    return false;
                }

                // Remove 'v' prefix if present (e.g., "v1.2.3" -> "1.2.3")
                string latestVersion = latestRelease.TagName.TrimStart('v');
                string currentVersion = App.Version;

                App.Logger.WriteLine(LOG_IDENT, $"Current version: {currentVersion}");
                App.Logger.WriteLine(LOG_IDENT, $"Latest version: {latestVersion}");

                var comparison = Utilities.CompareVersions(latestVersion, currentVersion);

                if (comparison != VersionComparison.GreaterThan)
                {
                    App.Logger.WriteLine(LOG_IDENT, "No updates available");
                    return false;
                }

                App.Logger.WriteLine(LOG_IDENT, $"Update available: {latestVersion}");

                // Find the exe asset in the release
                var exeAsset = latestRelease.Assets?.FirstOrDefault(a => 
                    a.Name.EndsWith(".exe", StringComparison.OrdinalIgnoreCase) && 
                    !a.Name.Contains("Setup", StringComparison.OrdinalIgnoreCase));

                if (exeAsset is null)
                {
                    App.Logger.WriteLine(LOG_IDENT, "No executable found in latest release");
                    return false;
                }

                // Prompt user to update
                var result = Frontend.ShowMessageBox(
                    String.Format(Strings.AutoUpdater_UpdateAvailable, latestVersion),
                    MessageBoxImage.Information,
                    MessageBoxButton.YesNo,
                    MessageBoxResult.Yes
                );

                if (result != MessageBoxResult.Yes)
                {
                    App.Logger.WriteLine(LOG_IDENT, "User declined update");
                    return false;
                }

                // Download and install update
                await DownloadAndInstallUpdate(exeAsset.BrowserDownloadUrl, latestVersion);
                return true;
            }
            catch (Exception ex)
            {
                App.Logger.WriteLine(LOG_IDENT, "Failed to check for updates");
                App.Logger.WriteException(LOG_IDENT, ex);
                return false;
            }
        }

        private static async Task DownloadAndInstallUpdate(string downloadUrl, string version)
        {
            App.Logger.WriteLine(LOG_IDENT, $"Downloading update from {downloadUrl}");

            try
            {
                // Create Updates directory
                string updatesDir = Path.Combine(Paths.Base, "Updates");
                Directory.CreateDirectory(updatesDir);

                string downloadPath = Path.Combine(updatesDir, $"{App.ProjectName}-{version}.exe");

                // Download the update
                using (var response = await App.HttpClient.GetAsync(downloadUrl, HttpCompletionOption.ResponseHeadersRead))
                {
                    response.EnsureSuccessStatusCode();

                    using (var fileStream = new FileStream(downloadPath, FileMode.Create, FileAccess.Write, FileShare.None))
                    {
                        await response.Content.CopyToAsync(fileStream);
                    }
                }

                App.Logger.WriteLine(LOG_IDENT, $"Update downloaded to {downloadPath}");

                // Launch the downloaded executable with upgrade flag
                var startInfo = new ProcessStartInfo
                {
                    FileName = downloadPath,
                    Arguments = "-upgrade",
                    UseShellExecute = true
                };

                // Preserve current launch arguments if any
                if (App.LaunchSettings.RobloxLaunchMode != LaunchMode.None)
                {
                    startInfo.Arguments += $" {string.Join(" ", App.LaunchSettings.Args)}";
                }

                Process.Start(startInfo);

                App.Logger.WriteLine(LOG_IDENT, "Launched update installer, terminating current instance");
                App.Terminate();
            }
            catch (Exception ex)
            {
                App.Logger.WriteLine(LOG_IDENT, "Failed to download or install update");
                App.Logger.WriteException(LOG_IDENT, ex);

                Frontend.ShowMessageBox(
                    String.Format(Strings.AutoUpdater_UpdateFailed, ex.Message),
                    MessageBoxImage.Error
                );
            }
        }
    }
}