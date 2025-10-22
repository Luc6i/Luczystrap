using System.ComponentModel;
using System.Windows;

namespace Bloxstrap.UI.Elements.Dialogs
{
    public partial class AddAccountDialog : INotifyPropertyChanged
    {
        private string _securityCookie = "";

        public AddAccountDialog()
        {
            DataContext = this;
            InitializeComponent();
        }

        public string SecurityCookie
        {
            get => _securityCookie;
            set
            {
                _securityCookie = value?.Trim() ?? "";
                OnPropertyChanged(nameof(SecurityCookie));
                OnPropertyChanged(nameof(IsValid));
            }
        }

        public bool IsValid => !string.IsNullOrWhiteSpace(SecurityCookie) && SecurityCookie.Length > 50;

        private void AddButton_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = true;
            Close();
        }

        private void CancelButton_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = false;
            Close();
        }

        public event PropertyChangedEventHandler? PropertyChanged;

        protected void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}