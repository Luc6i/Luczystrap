using System.Windows.Controls;

namespace Bloxstrap.UI.Elements.Settings.Pages
{
    /// <summary>
    /// Interaction logic for AccountsPage.xaml
    /// </summary>
    public partial class AccountsPage
    {
        public AccountsPage()
        {
            DataContext = new ViewModels.Settings.AccountsViewModel();
            InitializeComponent();
        }
    }
}