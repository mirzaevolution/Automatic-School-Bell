using System;

namespace AutomaticSchoolBell.GUI
{
    public class ScheduleModel:ValidatedEditableBindableBase<ScheduleModel>
    {
        protected override ScheduleModel Property
        {
            get
            {
                return new ScheduleModel
                {
                    Id = _id,
                    Day = _day,
                    StartTime = _startTime,
                    Description = _description,
                    AudioLocation = _audioLocation,
                    PlayerLocation = _playerLocation
                };
            }
            set
            {
                ScheduleModel model = value;
                if(model!=null)
                {
                    Id = model.Id;
                    Day = model.Day;
                    StartTime = model.StartTime;
                    Description = model.Description;
                    AudioLocation = model.AudioLocation;
                    PlayerLocation = model.PlayerLocation;
                }
            }
        }
        private int _id;
        private DayOfWeek _day;
        private TimeSpan _startTime;
        private string _description, _audioLocation, _playerLocation;
        private bool _isChecked;

        public int Id
        {
            get
            {
                return _id;
            }
            set
            {
                OnPropertyChanged(ref _id, value, nameof(Id));
            }
        }
        public DayOfWeek Day
        {
            get
            {
                return _day;
            }
            set
            {
                OnPropertyChanged(ref _day, value, nameof(Day));
            }
        }
        public TimeSpan StartTime
        {
            get
            {
                return _startTime;
            }
            set
            {
                OnPropertyChanged(ref _startTime, value, nameof(StartTime));
            }
        }
        public string Description
        {
            get
            {
                return _description;
            }
            set
            {
                OnPropertyChanged(ref _description, value, nameof(Description));
            }
        }
        public string AudioLocation
        {
            get
            {
                return _audioLocation;
            }
            set
            {
                OnPropertyChanged(ref _audioLocation, value, nameof(AudioLocation));
            }
        }
        public string PlayerLocation
        {
            get
            {
                return _playerLocation;
            }
            set
            {
                OnPropertyChanged(ref _playerLocation, value, nameof(PlayerLocation));
            }
        }
        public bool IsChecked
        {
            get
            {
                return _isChecked;
            }
            set
            {
                OnPropertyChanged(ref _isChecked, value, nameof(IsChecked));
            }
        }
    }
}
