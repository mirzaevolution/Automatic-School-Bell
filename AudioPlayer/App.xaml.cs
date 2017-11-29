using System;
using System.Windows;

namespace AudioPlayer
{
    public partial class App : Application
    {
        private void Application_Startup(object sender, StartupEventArgs e)
        {
            if (e.Args.Length > 0)
            {
                MainWindow window = new MainWindow(e.Args[0]);
                window.Show();
            }
        }
    }
}
