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

namespace AutomaticSchoolBell.GUI.RepeatedSchedules
{
    /// <summary>
    /// Interaction logic for RepeatedScheduleView.xaml
    /// </summary>
    public partial class RepeatedScheduleView : UserControl
    {
        public RepeatedScheduleView()
        {
            InitializeComponent();
        }

        private void RepeatedScheduleViewModel_ErrorOccured(object sender, string e)
        {
            BitmapImage image = new BitmapImage(new Uri("/Content/ico-notif-error.png", UriKind.Relative));
            ImageNotification.Source = image;
            TextStatus.Text = e;
        }

        private void RepeatedScheduleViewModel_Information(object sender, string e)
        {
            BitmapImage image = new BitmapImage(new Uri("/Content/ico-notif-info.png", UriKind.Relative));
            ImageNotification.Source = image;
            TextStatus.Text = e;
        }
    }
}
