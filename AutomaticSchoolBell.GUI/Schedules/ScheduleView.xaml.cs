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

namespace AutomaticSchoolBell.GUI.Schedules
{
    /// <summary>
    /// Interaction logic for ScheduleView.xaml
    /// </summary>
    public partial class ScheduleView : UserControl
    {
        public ScheduleView()
        {
            InitializeComponent();
        }

        private void ScheduleViewModel_ErrorOccured(object sender, string e)
        {
            BitmapImage image = new BitmapImage(new Uri("/Content/ico-notif-error.png", UriKind.Relative));
            ImageNotification.Source = image;
            TextStatus.Text = e;
        }

        private void ScheduleViewModel_Information(object sender, string e)
        {
            BitmapImage image = new BitmapImage(new Uri("/Content/ico-notif-info.png", UriKind.Relative));
            ImageNotification.Source = image;
            TextStatus.Text = e;
        }

   
    }
}
