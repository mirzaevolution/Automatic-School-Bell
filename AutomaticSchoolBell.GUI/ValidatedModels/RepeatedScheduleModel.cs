using System;
using CoreLib.Models;
using System.ComponentModel.DataAnnotations;

namespace AutomaticSchoolBell.GUI
{
    public class RepeatedScheduleModel:ValidatedEditableBindableBase<RepeatedScheduleModel>
    {

        protected override RepeatedScheduleModel Property
        {
            get
            {
                return new RepeatedScheduleModel
                {
                    Id = _id,
                    StartDateTime = _startDateTime,
                    Repetition = _repetition,
                    Description = _description,
                    AudioLocation = _audioLocation,
                    PlayerLocation = _playerLocation
                };
            }
            set
            {
                RepeatedScheduleModel model = value;
                if(model!=null)
                {
                    Id = model.Id;
                    StartDateTime = model.StartDateTime;
                    Repetition = model.Repetition;
                    Description = model.Description;
                    AudioLocation = model.AudioLocation;
                    PlayerLocation = model.PlayerLocation;
                }
            }
        }

        private int _id;
        private DateTime? _startDateTime;
        private Repetition _repetition;
        private string _description;
        private string _audioLocation;
        private string _playerLocation;
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

        [Required]
        public DateTime? StartDateTime
        {
            get
            {
                return _startDateTime;
            }
            set
            {
                OnPropertyChanged(ref _startDateTime, value, nameof(StartDateTime));
            }
        }

        [Required]
        public Repetition Repetition
        {
            get
            {
                return _repetition;
            }
            set
            {
                OnPropertyChanged(ref _repetition, value, nameof(Repetition));
            }
        }

        [Required]
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

        [Required]
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
