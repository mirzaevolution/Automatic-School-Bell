using System;
using System.Globalization;
using System.Windows.Data;
using CoreLib.Models;
namespace AutomaticSchoolBell.GUI.EnumConverter
{
    public class RepetitionConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            Repetition repetition;
            if(Enum.TryParse<Repetition>(value.ToString(),out repetition))
            {
                switch(repetition)
                {
                    case Repetition.Hourly:
                        return "Hourly";
                    case Repetition.Daily:
                        return "Daily";
                    case Repetition.Monthly:
                        return "Monthly";
                }
            }
            return null;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            Repetition repetition = Repetition.Hourly;
            switch(value.ToString())
            {
                case "Daily":
                    repetition = Repetition.Daily;
                    break;
                case "Monthly":
                    repetition = Repetition.Monthly;
                    break;
            }
            return repetition;
        }
    }
}
