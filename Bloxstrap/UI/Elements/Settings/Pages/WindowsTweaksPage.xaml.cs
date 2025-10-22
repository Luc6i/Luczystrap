using System.Windows;
using Bloxstrap.UI.ViewModels.Settings;

namespace Bloxstrap.UI.Elements.Settings.Pages
{
    /// <summary>
    /// Interaction logic for WindowsTweaksPage.xaml
    /// </summary>
    public partial class WindowsTweaksPage
    {
        public WindowsTweaksPage()
        {
            DataContext = new WindowsTweaksViewModel();
            InitializeComponent();
        }

        private void CardControl_Click(object sender, RoutedEventArgs e)
        {

        }
    }
}