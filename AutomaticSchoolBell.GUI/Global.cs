using System;
using System.IO;

namespace AutomaticSchoolBell.GUI
{
    public class Global
    {
        public static string PlayerLocation
        {
            get
            {
                return Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "AlphaPlayer.exe");
            }
        }
    }
}
