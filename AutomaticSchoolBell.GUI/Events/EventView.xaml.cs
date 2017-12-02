using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace AutomaticSchoolBell.GUI.Events
{
    /// <summary>
    /// Interaction logic for EventView.xaml
    /// </summary>
    public partial class EventView : UserControl
    {
        public EventView()
        {
            InitializeComponent();
        }

        private void EventViewModel_ErrorOccured(object sender, string e)
        {
            BitmapImage image = new BitmapImage(new Uri("/Content/ico-notif-error.png", UriKind.Relative));
            ImageNotification.Source = image;
            TextStatus.Text = e;
        }

        private void EventViewModel_Information(object sender, string e)
        {
            BitmapImage image = new BitmapImage(new Uri("/Content/ico-notif-info.png", UriKind.Relative));
            ImageNotification.Source = image;
            TextStatus.Text = e;
        }
       
    }
}
