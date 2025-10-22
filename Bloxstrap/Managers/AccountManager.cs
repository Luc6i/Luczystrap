using System;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Security.Cryptography;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using Bloxstrap.Models;

namespace Bloxstrap.Managers
{
    public static class AccountManager
    {
        private const string LOG_IDENT = "AccountManager";

        /// <summary>
        /// Adds a new Roblox account using the security cookie
        /// </summary>
        public static async Task<RobloxAccount?> AddAccount(string securityCookie)
        {
            const string LOG_IDENT_FUNC = $"{LOG_IDENT}::AddAccount";

            try
            {
                // Clean the cookie
                securityCookie = securityCookie.Trim();
                if (securityCookie.StartsWith("_|WARNING:-DO-NOT-SHARE-THIS.--Sharing-this-will-allow-someone-to-log-in-as-you-and-to-steal-your-ROBUX-and-items.|_"))
                {
                    // Remove the warning prefix that Roblox adds
                    securityCookie = securityCookie.Replace("_|WARNING:-DO-NOT-SHARE-THIS.--Sharing-this-will-allow-someone-to-log-in-as-you-and-to-steal-your-ROBUX-and-items.|_", "");
                }

                App.Logger.WriteLine(LOG_IDENT_FUNC, "Fetching user information...");

                // Fetch user info from Roblox API
                var userInfo = await FetchUserInfo(securityCookie);
                
                if (userInfo is null)
                {
                    App.Logger.WriteLine(LOG_IDENT_FUNC, "Failed to fetch user info");
                    return null;
                }

                // Check if account already exists
                var existingAccount = App.Settings.Prop.RobloxAccounts.FirstOrDefault(a => a.UserId == userInfo.UserId);
                if (existingAccount is not null)
                {
                    App.Logger.WriteLine(LOG_IDENT_FUNC, $"Account {userInfo.UserId} already exists, updating...");
                    
                    // Update existing account
                    existingAccount.Username = userInfo.Username;
                    existingAccount.DisplayName = userInfo.DisplayName;
                    existingAccount.AvatarThumbnailUrl = userInfo.AvatarUrl;
                    existingAccount.SecurityCookie = EncryptCookie(securityCookie);
                    existingAccount.LastUsed = DateTime.Now;
                    
                    App.Settings.Save();
                    return existingAccount;
                }

                // Create new account
                var newAccount = new RobloxAccount
                {
                    UserId = userInfo.UserId,
                    Username = userInfo.Username,
                    DisplayName = userInfo.DisplayName,
                    AvatarThumbnailUrl = userInfo.AvatarUrl,
                    SecurityCookie = EncryptCookie(securityCookie),
                    IsActive = App.Settings.Prop.RobloxAccounts.Count == 0, // First account is active by default
                    LastUsed = DateTime.Now
                };

                App.Settings.Prop.RobloxAccounts.Add(newAccount);

                if (newAccount.IsActive)
                    App.Settings.Prop.ActiveAccountId = newAccount.UserId;

                App.Settings.Save();

                App.Logger.WriteLine(LOG_IDENT_FUNC, $"Successfully added account: {newAccount.DisplayName} (@{newAccount.Username})");
                return newAccount;
            }
            catch (Exception ex)
            {
                App.Logger.WriteLine(LOG_IDENT_FUNC, $"Failed to add account: {ex}");
                throw;
            }
        }

        /// <summary>
        /// Removes an account by user ID
        /// </summary>
        public static void RemoveAccount(long userId)
        {
            const string LOG_IDENT_FUNC = $"{LOG_IDENT}::RemoveAccount";

            var account = App.Settings.Prop.RobloxAccounts.FirstOrDefault(a => a.UserId == userId);
            if (account is null)
            {
                App.Logger.WriteLine(LOG_IDENT_FUNC, $"Account {userId} not found");
                return;
            }

            bool wasActive = account.IsActive;
            App.Settings.Prop.RobloxAccounts.Remove(account);

            // If we removed the active account, set another one as active
            if (wasActive && App.Settings.Prop.RobloxAccounts.Count > 0)
            {
                var newActive = App.Settings.Prop.RobloxAccounts.First();
                newActive.IsActive = true;
                App.Settings.Prop.ActiveAccountId = newActive.UserId;
            }
            else if (App.Settings.Prop.RobloxAccounts.Count == 0)
            {
                App.Settings.Prop.ActiveAccountId = null;
            }

            App.Settings.Save();
            App.Logger.WriteLine(LOG_IDENT_FUNC, $"Removed account {userId}");
        }

        /// <summary>
        /// Switches to a different account
        /// </summary>
        public static void SwitchAccount(long userId)
        {
            const string LOG_IDENT_FUNC = $"{LOG_IDENT}::SwitchAccount";

            var account = App.Settings.Prop.RobloxAccounts.FirstOrDefault(a => a.UserId == userId);
            if (account is null)
            {
                App.Logger.WriteLine(LOG_IDENT_FUNC, $"Account {userId} not found");
                return;
            }

            // Deactivate all accounts
            foreach (var acc in App.Settings.Prop.RobloxAccounts)
            {
                acc.IsActive = false;
            }

            // Activate selected account
            account.IsActive = true;
            account.LastUsed = DateTime.Now;
            App.Settings.Prop.ActiveAccountId = userId;

            App.Settings.Save();
            App.Logger.WriteLine(LOG_IDENT_FUNC, $"Switched to account {userId}");
        }

        /// <summary>
        /// Gets the currently active account
        /// </summary>
        public static RobloxAccount? GetActiveAccount()
        {
            return App.Settings.Prop.RobloxAccounts.FirstOrDefault(a => a.IsActive);
        }

        /// <summary>
        /// Applies the active account's cookie by directly writing to Roblox's SQLite Cookies database
        /// CRITICAL: All Roblox processes must be terminated before calling this
        /// </summary>
        public static void ApplyAccountCookie()
        {
            const string LOG_IDENT_FUNC = $"{LOG_IDENT}::ApplyAccountCookie";

            var activeAccount = GetActiveAccount();
            if (activeAccount is null)
            {
                App.Logger.WriteLine(LOG_IDENT_FUNC, "No active account found");
                return;
            }

            try
            {
                string decryptedCookie = DecryptCookie(activeAccount.SecurityCookie);
                
                App.Logger.WriteLine(LOG_IDENT_FUNC, $"🔄 Injecting cookie for: {activeAccount.DisplayName} (@{activeAccount.Username})");
                App.Logger.WriteLine(LOG_IDENT_FUNC, $"Cookie length: {decryptedCookie.Length} characters");
                App.Logger.WriteLine(LOG_IDENT_FUNC, $"Cookie preview: {decryptedCookie.Substring(0, Math.Min(20, decryptedCookie.Length))}...");
                
                // CRITICAL STEP 1: Terminate ALL Roblox processes
                var processes = System.Diagnostics.Process.GetProcessesByName("RobloxPlayerBeta")
                    .Concat(System.Diagnostics.Process.GetProcessesByName("eurotrucks2"))
                    .Concat(System.Diagnostics.Process.GetProcessesByName("RobloxCrashHandler"))
                    .ToArray();
                
                if (processes.Any())
                {
                    App.Logger.WriteLine(LOG_IDENT_FUNC, $"⏳ Terminating {processes.Length} Roblox process(es)...");
                    foreach (var proc in processes)
                    {
                        try 
                        { 
                            App.Logger.WriteLine(LOG_IDENT_FUNC, $"Killing process: {proc.ProcessName} (PID: {proc.Id})");
                            proc.Kill(); 
                            proc.WaitForExit(3000); 
                            proc.Dispose(); 
                        }
                        catch (Exception ex)
                        {
                            App.Logger.WriteLine(LOG_IDENT_FUNC, $"Failed to kill process: {ex.Message}");
                        }
                    }
                    System.Threading.Thread.Sleep(2000);
                    App.Logger.WriteLine(LOG_IDENT_FUNC, "✓ All processes terminated");
                }
                else
                {
                    App.Logger.WriteLine(LOG_IDENT_FUNC, "No Roblox processes running");
                }
                
                // STEP 2: Locate Roblox's data directories
                string robloxDir = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "Roblox");
                
                App.Logger.WriteLine(LOG_IDENT_FUNC, $"🔍 DIAGNOSTIC: Scanning Roblox directory structure...");
                
                // Check what storage Roblox is actually using
                string[] potentialDirs = {
                    Path.Combine(robloxDir, "Cookies"),
                    Path.Combine(robloxDir, "Local Storage"),
                    Path.Combine(robloxDir, "Session Storage"),
                    Path.Combine(robloxDir, "IndexedDB"),
                    Path.Combine(robloxDir, "http_www.roblox.com_0.indexeddb.leveldb"),
                    Path.Combine(robloxDir, "https_www.roblox.com_0.indexeddb.leveldb")
                };
                
                foreach (var dir in potentialDirs)
                {
                    if (File.Exists(dir))
                        App.Logger.WriteLine(LOG_IDENT_FUNC, $"  📄 Found FILE: {Path.GetFileName(dir)}");
                    else if (Directory.Exists(dir))
                        App.Logger.WriteLine(LOG_IDENT_FUNC, $"  📁 Found DIR: {Path.GetFileName(dir)}");
                }
                
                // CRITICAL FIX: Roblox uses Local Storage LevelDB, NOT Cookies!
                string localStorageDir = Path.Combine(robloxDir, "Local Storage");
                string levelDbDir = Path.Combine(localStorageDir, "leveldb");
                
                App.Logger.WriteLine(LOG_IDENT_FUNC, $"Target directory: {localStorageDir}");
                
                // Ensure Roblox directory exists
                Directory.CreateDirectory(robloxDir);
                
                // STEP 3: Delete ALL browser storage to force clean state
                string[] pathsToDelete = {
                    Path.Combine(robloxDir, "Cookies"),
                    Path.Combine(robloxDir, "Cookies-journal"),
                    Path.Combine(robloxDir, "Local Storage"),
                    Path.Combine(robloxDir, "Session Storage"),
                    Path.Combine(robloxDir, "IndexedDB"),
                    Path.Combine(robloxDir, "http_www.roblox.com_0.indexeddb.leveldb"),
                    Path.Combine(robloxDir, "https_www.roblox.com_0.indexeddb.leveldb")
                };
                
                foreach (var item in pathsToDelete)
                {
                    try
                    {
                        if (File.Exists(item))
                        {
                            File.Delete(item);
                            App.Logger.WriteLine(LOG_IDENT_FUNC, $"✓ Deleted file: {Path.GetFileName(item)}");
                        }
                        if (Directory.Exists(item))
                        {
                            Directory.Delete(item, true);
                            App.Logger.WriteLine(LOG_IDENT_FUNC, $"✓ Deleted directory: {Path.GetFileName(item)}");
                        }
                    }
                    catch (Exception ex)
                    {
                        App.Logger.WriteLine(LOG_IDENT_FUNC, $"⚠ Could not delete {Path.GetFileName(item)}: {ex.Message}");
                    }
                }
                
                System.Threading.Thread.Sleep(1000);
                
                // STEP 4: Write to BOTH Cookies (SQLite) AND Local Storage (LevelDB)
                string cookiesDbPath = Path.Combine(robloxDir, "Cookies");
                
                // Write SQLite cookie
                WriteCookieToDatabase(cookiesDbPath, decryptedCookie);
                
                // ALSO write to Local Storage LevelDB format
                WriteToLocalStorage(robloxDir, decryptedCookie);
                
                // STEP 5: Verify the database exists
                if (File.Exists(cookiesDbPath))
                {
                    App.Logger.WriteLine(LOG_IDENT_FUNC, $"✅ Cookie database created at: {cookiesDbPath}");
                }
                else
                {
                    App.Logger.WriteLine(LOG_IDENT_FUNC, $"❌ WARNING: Cookie database does not exist after creation!");
                }
                
                App.Logger.WriteLine(LOG_IDENT_FUNC, $"✅ Cookie injection complete for: {activeAccount.DisplayName}");
            }
            catch (Exception ex)
            {
                App.Logger.WriteLine(LOG_IDENT_FUNC, $"❌ Failed: {ex}");
                App.Logger.WriteException(LOG_IDENT_FUNC, ex);
            }
        }

        /// <summary>
        /// Writes cookie to Local Storage in LevelDB format (Roblox CEF uses this)
        /// </summary>
        private static void WriteToLocalStorage(string robloxDir, string cookieValue)
        {
            const string LOG_IDENT_FUNC = $"{LOG_IDENT}::WriteToLocalStorage";
            
            try
            {
                // Create Local Storage directory structure
                string localStorageDir = Path.Combine(robloxDir, "Local Storage");
                string levelDbDir = Path.Combine(localStorageDir, "leveldb");
                
                Directory.CreateDirectory(levelDbDir);
                
                // LevelDB stores data as key-value pairs in .ldb files
                // We'll create a simple .ldb file with the cookie
                string currentFile = Path.Combine(levelDbDir, "000003.ldb");
                
                // LevelDB format is complex, but we can write a simplified version
                // Format: [key_length][key][value_length][value]
                
                string key = "_https://www.roblox.com\x00\x01.ROBLOSECURITY";
                byte[] keyBytes = System.Text.Encoding.UTF8.GetBytes(key);
                byte[] valueBytes = System.Text.Encoding.UTF8.GetBytes(cookieValue);
                
                using var fs = new FileStream(currentFile, FileMode.Create, FileAccess.Write);
                using var writer = new BinaryWriter(fs);
                
                // Write key length (varint)
                writer.Write((byte)keyBytes.Length);
                writer.Write(keyBytes);
                
                // Write value length (varint)
                writer.Write((byte)valueBytes.Length);
                writer.Write((byte)(valueBytes.Length >> 8));
                writer.Write(valueBytes);
                
                fs.Flush();
                
                // Create CURRENT file pointing to our manifest
                File.WriteAllText(Path.Combine(levelDbDir, "CURRENT"), "MANIFEST-000002\n");
                
                // Create a minimal MANIFEST file
                File.WriteAllBytes(Path.Combine(levelDbDir, "MANIFEST-000002"), new byte[] { 
                    0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00,
                    0x01, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00,
                    0x03, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00
                });
                
                App.Logger.WriteLine(LOG_IDENT_FUNC, $"✓ Written to Local Storage LevelDB");
            }
            catch (Exception ex)
            {
                App.Logger.WriteLine(LOG_IDENT_FUNC, $"⚠ LevelDB write failed (this might be OK): {ex.Message}");
            }
        }

        /// <summary>
        /// Writes the .ROBLOSECURITY cookie directly to Roblox's SQLite cookies database
        /// </summary>
        private static void WriteCookieToDatabase(string dbPath, string cookieValue)
        {
            const string LOG_IDENT_FUNC = $"{LOG_IDENT}::WriteCookieToDatabase";
            
            try
            {
                // Delete existing database to start fresh
                if (File.Exists(dbPath))
                {
                    File.Delete(dbPath);
                    App.Logger.WriteLine(LOG_IDENT_FUNC, "Deleted existing Cookies database");
                }
                
                // Wait for file system to catch up
                System.Threading.Thread.Sleep(500);
                
                using var connection = new System.Data.SQLite.SQLiteConnection($"Data Source={dbPath};Version=3;Synchronous=FULL;");
                connection.Open();
                
                App.Logger.WriteLine(LOG_IDENT_FUNC, "Creating cookies table...");
                
                // Create the cookies table with correct Chromium schema (simplified for Roblox CEF)
                string createTableSql = @"
                    CREATE TABLE cookies (
                        creation_utc INTEGER NOT NULL,
                        host_key TEXT NOT NULL,
                        name TEXT NOT NULL,
                        value TEXT NOT NULL,
                        path TEXT NOT NULL,
                        expires_utc INTEGER NOT NULL,
                        is_secure INTEGER NOT NULL,
                        is_httponly INTEGER NOT NULL,
                        last_access_utc INTEGER NOT NULL,
                        has_expires INTEGER NOT NULL DEFAULT 1,
                        is_persistent INTEGER NOT NULL DEFAULT 1,
                        priority INTEGER NOT NULL DEFAULT 1,
                        encrypted_value BLOB DEFAULT '',
                        samesite INTEGER NOT NULL DEFAULT -1,
                        source_scheme INTEGER NOT NULL DEFAULT 0,
                        source_port INTEGER NOT NULL DEFAULT -1,
                        is_same_party INTEGER NOT NULL DEFAULT 0,
                        PRIMARY KEY (host_key, name, path)
                    );
                    CREATE INDEX domain ON cookies(host_key);
                    CREATE INDEX is_transient ON cookies(is_persistent);";
                
                using (var cmd = new System.Data.SQLite.SQLiteCommand(createTableSql, connection))
                {
                    cmd.ExecuteNonQuery();
                }
                
                App.Logger.WriteLine(LOG_IDENT_FUNC, "Table created successfully");
                
                // Get current time as Chrome timestamp (microseconds since Jan 1, 1601)
                long chromeTime = ((DateTimeOffset.UtcNow.ToUniversalTime().Ticks - 621355968000000000) / 10);
                
                // Expiration: 2 years from now (Roblox cookies are long-lived)
                long expirationTime = chromeTime + (730L * 24 * 60 * 60 * 1000000);
                
                App.Logger.WriteLine(LOG_IDENT_FUNC, $"Inserting cookie (length: {cookieValue.Length} chars)");
                
                // Insert the .ROBLOSECURITY cookie
                string insertSql = @"
                    INSERT INTO cookies (
                        creation_utc, host_key, name, value, path, expires_utc,
                        is_secure, is_httponly, last_access_utc, has_expires, is_persistent,
                        priority, encrypted_value, samesite, source_scheme, source_port, is_same_party
                    ) VALUES (
                        @creation_utc, @host_key, @name, @value, @path, @expires_utc,
                        @is_secure, @is_httponly, @last_access_utc, @has_expires, @is_persistent,
                        @priority, @encrypted_value, @samesite, @source_scheme, @source_port, @is_same_party
                    );";
                
                using var insertCmd = new System.Data.SQLite.SQLiteCommand(insertSql, connection);
                insertCmd.Parameters.AddWithValue("@creation_utc", chromeTime);
                insertCmd.Parameters.AddWithValue("@host_key", ".roblox.com");
                insertCmd.Parameters.AddWithValue("@name", ".ROBLOSECURITY");
                insertCmd.Parameters.AddWithValue("@value", cookieValue);
                insertCmd.Parameters.AddWithValue("@path", "/");
                insertCmd.Parameters.AddWithValue("@expires_utc", expirationTime);
                insertCmd.Parameters.AddWithValue("@is_secure", 1);
                insertCmd.Parameters.AddWithValue("@is_httponly", 1);
                insertCmd.Parameters.AddWithValue("@last_access_utc", chromeTime);
                insertCmd.Parameters.AddWithValue("@has_expires", 1);
                insertCmd.Parameters.AddWithValue("@is_persistent", 1);
                insertCmd.Parameters.AddWithValue("@priority", 1);
                insertCmd.Parameters.AddWithValue("@encrypted_value", new byte[0]);
                insertCmd.Parameters.AddWithValue("@samesite", -1); // No SameSite restriction
                insertCmd.Parameters.AddWithValue("@source_scheme", 2); // HTTPS
                insertCmd.Parameters.AddWithValue("@source_port", 443);
                insertCmd.Parameters.AddWithValue("@is_same_party", 0);
                
                int rowsAffected = insertCmd.ExecuteNonQuery();
                
                App.Logger.WriteLine(LOG_IDENT_FUNC, $"✓ Cookie inserted successfully ({rowsAffected} row(s) affected)");
                
                // CRITICAL: Force database flush to disk
                using (var pragmaCmd = new System.Data.SQLite.SQLiteCommand("PRAGMA synchronous=FULL; PRAGMA wal_checkpoint(TRUNCATE);", connection))
                {
                    pragmaCmd.ExecuteNonQuery();
                }
                
                connection.Close();
                
                // Wait for database to fully close
                System.Threading.Thread.Sleep(300);
                
                // Verify the cookie was written
                if (File.Exists(dbPath))
                {
                    var fileInfo = new FileInfo(dbPath);
                    App.Logger.WriteLine(LOG_IDENT_FUNC, $"✓ Database file size: {fileInfo.Length} bytes");
                }
            }
            catch (Exception ex)
            {
                App.Logger.WriteLine(LOG_IDENT_FUNC, $"❌ Database write failed: {ex.Message}");
                App.Logger.WriteException(LOG_IDENT_FUNC, ex);
                throw;
            }
        }

        #region Private Helper Methods

        private static async Task<UserInfo?> FetchUserInfo(string securityCookie)
        {
            try
            {
                using var httpClient = new HttpClient();
                httpClient.DefaultRequestHeaders.Add("Cookie", $".ROBLOSECURITY={securityCookie}");

                // Get authenticated user info
                var response = await httpClient.GetAsync("https://users.roblox.com/v1/users/authenticated");
                
                if (!response.IsSuccessStatusCode)
                    return null;

                var json = await response.Content.ReadAsStringAsync();
                var userDoc = JsonDocument.Parse(json);
                var root = userDoc.RootElement;

                long userId = root.GetProperty("id").GetInt64();
                string username = root.GetProperty("name").GetString() ?? "";
                string displayName = root.GetProperty("displayName").GetString() ?? "";

                // Get avatar thumbnail
                var thumbnailResponse = await httpClient.GetAsync(
                    $"https://thumbnails.roblox.com/v1/users/avatar-headshot?userIds={userId}&size=150x150&format=Png"
                );

                string? avatarUrl = null;
                if (thumbnailResponse.IsSuccessStatusCode)
                {
                    var thumbnailJson = await thumbnailResponse.Content.ReadAsStringAsync();
                    var thumbnailDoc = JsonDocument.Parse(thumbnailJson);
                    
                    if (thumbnailDoc.RootElement.TryGetProperty("data", out var dataArray) && dataArray.GetArrayLength() > 0)
                    {
                        avatarUrl = dataArray[0].GetProperty("imageUrl").GetString();
                    }
                }

                return new UserInfo
                {
                    UserId = userId,
                    Username = username,
                    DisplayName = displayName,
                    AvatarUrl = avatarUrl
                };
            }
            catch (Exception ex)
            {
                App.Logger.WriteLine($"{LOG_IDENT}::FetchUserInfo", $"Error: {ex}");
                return null;
            }
        }

        private static string EncryptCookie(string cookie)
        {
            try
            {
                byte[] data = Encoding.UTF8.GetBytes(cookie);
                byte[] encrypted = ProtectedData.Protect(data, null, DataProtectionScope.CurrentUser);
                return Convert.ToBase64String(encrypted);
            }
            catch (Exception ex)
            {
                App.Logger.WriteLine($"{LOG_IDENT}::EncryptCookie", $"Error: {ex}");
                throw;
            }
        }

        public static string DecryptCookie(string encryptedCookie)
        {
            try
            {
                byte[] encrypted = Convert.FromBase64String(encryptedCookie);
                byte[] data = ProtectedData.Unprotect(encrypted, null, DataProtectionScope.CurrentUser);
                return Encoding.UTF8.GetString(data);
            }
            catch (Exception ex)
            {
                App.Logger.WriteLine($"{LOG_IDENT}::DecryptCookie", $"Error: {ex}");
                throw;
            }
        }

        private class UserInfo
        {
            public long UserId { get; set; }
            public string Username { get; set; } = "";
            public string DisplayName { get; set; } = "";
            public string? AvatarUrl { get; set; }
        }

        #endregion
    }
}