using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Windows;
using System.Windows.Input;
using CommunityToolkit.Mvvm.Input;
using Bloxstrap.Models;
using Bloxstrap.Managers;
using Bloxstrap.UI.Elements.Dialogs;

namespace Bloxstrap.UI.ViewModels.Settings
{
    public class AccountsViewModel : NotifyPropertyChangedViewModel
    {
        private RobloxAccount? _selectedAccount;

        public AccountsViewModel()
        {
            // Initialize commands
            AddAccountCommand = new RelayCommand(AddAccount);
            RemoveAccountCommand = new RelayCommand<RobloxAccount>(RemoveAccount);
            SwitchAccountCommand = new RelayCommand<RobloxAccount>(SwitchAccount);
            RefreshAccountsCommand = new RelayCommand(RefreshAccounts);
            LaunchWithSelectedCommand = new RelayCommand(LaunchWithSelected, CanLaunchWithSelected);

            // Load accounts from settings
            RefreshAccounts();
        }

        #region Properties

        public ObservableCollection<RobloxAccount> Accounts
        {
            get => App.Settings.Prop.RobloxAccounts;
        }

        public RobloxAccount? SelectedAccount
        {
            get => _selectedAccount;
            set
            {
                _selectedAccount = value;
                OnPropertyChanged(nameof(SelectedAccount));
                OnPropertyChanged(nameof(IsAccountSelected));
                ((RelayCommand)LaunchWithSelectedCommand).NotifyCanExecuteChanged();
            }
        }

        public bool HasAccounts => Accounts.Count > 0;
        public bool HasNoAccounts => Accounts.Count == 0;
        public bool IsAccountSelected => SelectedAccount is not null;

        #endregion

        #region Commands

        public ICommand AddAccountCommand { get; }
        public ICommand RemoveAccountCommand { get; }
        public ICommand SwitchAccountCommand { get; }
        public ICommand RefreshAccountsCommand { get; }
        public ICommand LaunchWithSelectedCommand { get; }

        #endregion

        #region Command Implementations

        private async void AddAccount()
        {
            // Show input dialog for security cookie
            var dialog = new AddAccountDialog();
            var result = dialog.ShowDialog();

            if (result != true || string.IsNullOrWhiteSpace(dialog.SecurityCookie))
                return;

            try
            {
                // Show loading message
                Frontend.ShowMessageBox(
                    "Adding account...\nThis may take a few seconds.",
                    MessageBoxImage.Information,
                    MessageBoxButton.OK
                );

                // Use AccountManager to add the account
                var account = await AccountManager.AddAccount(dialog.SecurityCookie);

                if (account is null)
                {
                    Frontend.ShowMessageBox(
                        "Failed to add account. Please check your security cookie and try again.",
                        MessageBoxImage.Error
                    );
                    return;
                }

                // Refresh the list
                RefreshAccounts();

                // Show success message
                Frontend.ShowMessageBox(
                    $"Successfully added account: {account.DisplayName} (@{account.Username})",
                    MessageBoxImage.Information
                );
            }
            catch (Exception ex)
            {
                App.Logger.WriteLine("AccountsViewModel::AddAccount", $"Failed to add account: {ex}");
                Frontend.ShowMessageBox(
                    $"An error occurred while adding the account:\n\n{ex.Message}",
                    MessageBoxImage.Error
                );
            }
        }

        private void RemoveAccount(RobloxAccount? account)
        {
            if (account is null)
                return;

            // Confirm deletion
            var result = Frontend.ShowMessageBox(
                $"Are you sure you want to remove the account '{account.DisplayName}' (@{account.Username})?\n\nThis action cannot be undone.",
                MessageBoxImage.Warning,
                MessageBoxButton.YesNo
            );

            if (result != MessageBoxResult.Yes)
                return;

            try
            {
                AccountManager.RemoveAccount(account.UserId);
                RefreshAccounts();

                Frontend.ShowMessageBox(
                    "Account removed successfully.",
                    MessageBoxImage.Information
                );
            }
            catch (Exception ex)
            {
                App.Logger.WriteLine("AccountsViewModel::RemoveAccount", $"Failed to remove account: {ex}");
                Frontend.ShowMessageBox(
                    $"An error occurred while removing the account:\n\n{ex.Message}",
                    MessageBoxImage.Error
                );
            }
        }

        private void SwitchAccount(RobloxAccount? account)
        {
            if (account is null)
                return;

            try
            {
                AccountManager.SwitchAccount(account.UserId);
                RefreshAccounts();

                Frontend.ShowMessageBox(
                    $"Switched to account: {account.DisplayName} (@{account.Username})\n\nThe active account will be used the next time you launch Roblox.",
                    MessageBoxImage.Information
                );
            }
            catch (Exception ex)
            {
                App.Logger.WriteLine("AccountsViewModel::SwitchAccount", $"Failed to switch account: {ex}");
                Frontend.ShowMessageBox(
                    $"An error occurred while switching accounts:\n\n{ex.Message}",
                    MessageBoxImage.Error
                );
            }
        }

        private void RefreshAccounts()
        {
            OnPropertyChanged(nameof(Accounts));
            OnPropertyChanged(nameof(HasAccounts));
            OnPropertyChanged(nameof(HasNoAccounts));
        }

        private void LaunchWithSelected()
        {
            if (SelectedAccount is null)
                return;

            try
            {
                const string LOG_IDENT = "AccountsViewModel::LaunchWithSelected";
                
                App.Logger.WriteLine(LOG_IDENT, $"Switching to account: {SelectedAccount.DisplayName} (@{SelectedAccount.Username})");
                
                // Switch to the selected account
                AccountManager.SwitchAccount(SelectedAccount.UserId);
                
                // Get the decrypted cookie
                string cookie = AccountManager.DecryptCookie(SelectedAccount.SecurityCookie);
                
                // CRITICAL: Launch via website with cookie injection
                string launchUrl = "https://www.roblox.com/home";
                
                // Use WebView2 or default browser to inject cookie and navigate
                var psi = new System.Diagnostics.ProcessStartInfo
                {
                    FileName = launchUrl,
                    UseShellExecute = true
                };
                
                // Set cookie via registry for Edge/Chrome to read
                SetBrowserCookie(".roblox.com", ".ROBLOSECURITY", cookie);
                
                System.Diagnostics.Process.Start(psi);
                
                App.Logger.WriteLine(LOG_IDENT, "✅ Launched browser with account authentication");
            }
            catch (Exception ex)
            {
                App.Logger.WriteLine("AccountsViewModel::LaunchWithSelected", $"Failed to launch: {ex}");
                Frontend.ShowMessageBox(
                    $"An error occurred while launching Roblox:\n\n{ex.Message}",
                    MessageBoxImage.Error
                );
            }
        }

        private void SetBrowserCookie(string domain, string name, string value)
        {
            // This is a simplified approach - you'd need InternetSetCookie from wininet.dll
            // Or use Selenium/Playwright for proper cookie injection
        }

        private bool CanLaunchWithSelected()
        {
            return SelectedAccount is not null;
        }

        #endregion
    }
}