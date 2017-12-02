using System;
using System.Globalization;
using System.Windows.Data;

namespace AutomaticSchoolBell.GUI.EnumConverter
{
    public class DayOfWeekConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            DayOfWeek dayOfWeek;
            if (Enum.TryParse<DayOfWeek>(value.ToString(), out dayOfWeek))
            {
                switch (dayOfWeek)
                {
                    case DayOfWeek.Sunday:
                        return "Sunday";
                    case DayOfWeek.Monday:
                        return "Monday";
                    case DayOfWeek.Tuesday:
                        return "Tuesday";
                    case DayOfWeek.Wednesday:
                        return "Wednesday";
                    case DayOfWeek.Thursday:
                        return "Thursday";
                    case DayOfWeek.Friday:
                        return "Friday";
                    case DayOfWeek.Saturday:
                        return "Saturday";
                }
            }
            return null;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            string day = value.ToString();
            DayOfWeek dayOfWeek = DayOfWeek.Sunday;
            switch (day)
            {
                case "Monday":
                    dayOfWeek = DayOfWeek.Monday;
                    break;
                case "Tuesday":
                    dayOfWeek = DayOfWeek.Tuesday;
                    break;
                case "Wednesday":
                    dayOfWeek = DayOfWeek.Wednesday;
                    break;
                case "Thursday":
                    dayOfWeek = DayOfWeek.Thursday;
                    break;
                case "Friday":
                    dayOfWeek = DayOfWeek.Friday;
                    break;
                case "Saturday":
                    dayOfWeek = DayOfWeek.Saturday;
                    break;

            }
            return dayOfWeek;


        }
    }
}
