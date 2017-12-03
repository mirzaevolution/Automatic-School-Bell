using MahApps.Metro.Controls;
using System.Windows;
using AutomaticSchoolBell.GUI.About;
namespace AutomaticSchoolBell.GUI
{

    public partial class MainWindow : MetroWindow
    {
       
        public MainWindow()
        {
            InitializeComponent();
            

        }

        private void HamburgerMenuControl_ItemClick(object sender, ItemClickEventArgs e)
        {
            this.HamburgerMenuControl.Content = e.ClickedItem;
        }

        private void ButtonAbout_Click(object sender, RoutedEventArgs e)
        {
            new AboutView().ShowDialog();
        }
    }
}
