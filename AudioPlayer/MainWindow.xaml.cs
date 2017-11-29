using System;
using System.IO;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace AudioPlayer
{
    public partial class MainWindow : Window
    {
        private MediaPlayer player;
        public MainWindow()
        {
            InitializeComponent();
            player = new MediaPlayer
            {
                Volume = 1.0f
            };
            player.MediaEnded += Player_MediaEnded;
        }

        private void Player_MediaEnded(object sender, EventArgs e)
        {
            App.Current.Shutdown();
        }

        public MainWindow(string audioLocation)
        {
            InitializeComponent();
            player = new MediaPlayer
            {
                Volume = 1.0f
            };
            player.MediaEnded += Player_MediaEnded;
            LoadAudio(audioLocation);
        }
        private void LoadAudio(string audioLocation)
        {
            try
            {
                if (File.Exists(audioLocation))
                {

                 
                    player.Open(new Uri(audioLocation));
                    player.Play();
                    
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
            }
        }
    }
}
