using System.ComponentModel;

namespace AutomaticSchoolBell.GUI
{
    public class BindableBase : INotifyPropertyChanged
    {
        protected virtual void OnPropertyChanged<PropType>(ref PropType member, PropType value, string propertyName )
        {
            if (Equals(member, value))
                return;
            member = value;
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
        public event PropertyChangedEventHandler PropertyChanged;
    }
}
