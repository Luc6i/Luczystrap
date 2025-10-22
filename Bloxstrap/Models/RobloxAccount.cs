using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace Bloxstrap.Models
{
    public class RobloxAccount : INotifyPropertyChanged
    {
        private long _userId;
        private string _username = "";
        private string _displayName = "";
        private string? _avatarThumbnailUrl;
        private string _securityCookie = "";
        private bool _isActive;
        private DateTime _lastUsed;

        public long UserId
        {
            get => _userId;
            set
            {
                _userId = value;
                OnPropertyChanged();
            }
        }

        public string Username
        {
            get => _username;
            set
            {
                _username = value;
                OnPropertyChanged();
            }
        }

        public string DisplayName
        {
            get => _displayName;
            set
            {
                _displayName = value;
                OnPropertyChanged();
            }
        }

        public string? AvatarThumbnailUrl
        {
            get => _avatarThumbnailUrl;
            set
            {
                _avatarThumbnailUrl = value;
                OnPropertyChanged();
            }
        }

        public string SecurityCookie
        {
            get => _securityCookie;
            set
            {
                _securityCookie = value;
                OnPropertyChanged();
            }
        }

        public bool IsActive
        {
            get => _isActive;
            set
            {
                _isActive = value;
                OnPropertyChanged();
            }
        }

        public DateTime LastUsed
        {
            get => _lastUsed;
            set
            {
                _lastUsed = value;
                OnPropertyChanged();
            }
        }

        public event PropertyChangedEventHandler? PropertyChanged;

        protected void OnPropertyChanged([CallerMemberName] string? propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}