using MahApps.Metro.Controls;
using System.IO;
namespace AutomaticSchoolBell.GUI.About
{
    /// <summary>
    /// Interaction logic for AboutView.xaml
    /// </summary>
    public partial class AboutView : MetroWindow
    {
        public AboutView()
        {
            InitializeComponent();
            Load();
        }
        private async void Load()
        {
            if(File.Exists("apache-license.txt"))
            {
                try
                {
                    using (StreamReader reader = new StreamReader("apache-license.txt"))
                    {
                        string text = await reader.ReadToEndAsync();
                        TextBoxLicense.Text = text;
                    }
                }
                catch { }
            }
        }
        private void GotoUri(object sender, System.Windows.Navigation.RequestNavigateEventArgs e)
        {
            try
            {
                System.Diagnostics.Process.Start(e.Uri.AbsoluteUri);
            }
            catch { }
        }
    }
}
