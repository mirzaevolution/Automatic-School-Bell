using System;
using System.IO;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace AudioPlayer
{
    public partial class MainWindow : Window
    {
        private MediaPlayer player = new MediaPlayer()
        {
            Volume = 1.0
        };
        public MainWindow()
        {
            InitializeComponent();

        }
        public MainWindow(string audioLocation)
        {
            InitializeComponent();
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
