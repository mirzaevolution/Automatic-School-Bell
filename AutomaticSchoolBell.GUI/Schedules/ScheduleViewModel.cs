using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using CoreLib.DataAccess;
using CoreLib.Models;

namespace AutomaticSchoolBell.GUI.Schedules
{
    public class ScheduleViewModel:BindableBase
    {
        
        private ScheduleModel _selectedSchedule;
        private ObservableCollection<ScheduleModel> _scheduleCollection;
        private bool _addMode, _editMode, _removeMode, _canEditFields, _canUseDatagrid = true;
        public ScheduleModel SelectedSchedule
        {
            get
            {
                return _selectedSchedule;
            }
            set
            {
                OnPropertyChanged(ref _selectedSchedule, value, nameof(SelectedSchedule));
            }
        }
        public ObservableCollection<ScheduleModel> ScheduleCollection
        {
            get
            {
                return _scheduleCollection;
            }
            set
            {
                OnPropertyChanged(ref _scheduleCollection, value, nameof(ScheduleCollection));
            }
        }
        public RelayCommand AddCommand { get; private set; }
        public RelayCommand EditCommand { get; private set; }
        public RelayCommand RemoveCommand { get; private set; }
        public RelayCommand SaveCommand { get; private set; }
        public RelayCommand CancelCommand { get; private set; }
        public bool AddMode
        {
            get
            {
                return _addMode;
            }
            set
            {
                OnPropertyChanged(ref _addMode, value, nameof(AddMode));
            }
        }
        public bool EditMode
        {
            get
            {
                return _editMode;
            }
            set
            {
                OnPropertyChanged(ref _editMode, value, nameof(_editMode));
            }
        }
        public bool RemoveMode
        {
            get
            {
                return _removeMode;
            }
            set
            {
                OnPropertyChanged(ref _removeMode, value, nameof(RemoveMode));
            }
        }
        public bool CanEditFields
        {
            get
            {
                return _canEditFields;
            }
            set
            {
                OnPropertyChanged(ref _canEditFields, value, nameof(CanEditFields));
            }
        }
        public bool CanUseDatagrid
        {
            get
            {
                return _canUseDatagrid;
            }
            set
            {
                OnPropertyChanged(ref _canUseDatagrid, value, nameof(CanUseDatagrid));
            }
        }
        public event EventHandler<string> ErrorOccured;
        public event EventHandler<string> Information;
        public ScheduleViewModel()
        {
            InitializeComponents();
        }
        public async void Load()
        {
            await Task.Run(() =>
            {
                try
                {
                    using (Repository<Schedule> repo = new Repository<Schedule>())
                    {
                        var result = repo.GetAll();
                        if (result.Status.Success)
                        {
                            if (result.Data.Count > 0)
                            {
                                var convertedList = result.Data.Select(x => Converter.ConvertToScheduleModel(x)).ToList();
                                ScheduleCollection = new ObservableCollection<ScheduleModel>(convertedList);
                            }
                            else
                                ScheduleCollection = new ObservableCollection<ScheduleModel>();
                        }
                        else
                            OnErrorOccured(result.Status.ErrorMessage);
                    }
                }
                catch (Exception ex)
                {
                    OnErrorOccured($"An error occured.\nMessage: {ex.Message}");
                }
            });
        }

        private void InitializeComponents()
        {
            ScheduleCollection = new ObservableCollection<ScheduleModel>();
            AddCommand = new RelayCommand(OnAdd, CanAdd);
            EditCommand = new RelayCommand(OnEdit, CanEdit);
            RemoveCommand = new RelayCommand(OnRemove, CanRemove);
            SaveCommand = new RelayCommand(OnSave, CanSave);
            CancelCommand = new RelayCommand(OnCancel, CanCancel);
        }
        
        private void OnAdd()
        {
            AddMode = true;
            CanEditFields = true;
            CanUseDatagrid = false;
            SelectedSchedule = new ScheduleModel
            {
                Day = DateTime.Now.DayOfWeek,
                Description = "",
                AudioLocation = "",
                PlayerLocation = "",
                StartTime = new TimeSpan(0, 0, 0),
                IsChecked = false
            };
            ScheduleCollection.Add(SelectedSchedule);
        }

        private bool CanAdd()
        {
            if (!_addMode && !_editMode && !_removeMode)
                return true;
            return false;
        }

        private void OnEdit()
        {
            EditMode = true;
            CanEditFields = true;
            CanUseDatagrid = false;
            SelectedSchedule.BeginEdit();
        }

        private bool CanEdit()
        {
            if ((SelectedSchedule == null) || (_addMode || _editMode || _removeMode))
                return false;
            return true;
        }

        private void OnRemove()
        {
            RemoveMode = true;
            CanEditFields = false;
            CanUseDatagrid = true;

            
        }

        private bool CanRemove()
        {
            if ((ScheduleCollection.Count == 0) || (_addMode || _editMode || _removeMode))
                return false;
            return true;
        }

        private void OnSave()
        {
            if(AddMode)
            {
                var schedule = Converter.ConvertFromScheduleModel(SelectedSchedule);
                try
                {
                    using (Repository<Schedule> repo = new Repository<Schedule>())
                    {
                        var result = repo.InsertWithResult(schedule);
                        if(result.Status.Success)
                        {
                            SelectedSchedule.Id = result.Data.ID;
                            OnInformationRequested("Data added successfully");
                        }
                        else
                        {
                            OnErrorOccured(result.Status.ErrorMessage);
                        }
                    }
                }
                catch (Exception ex)
                {
                    OnErrorOccured($"An error occured.\nMessage: {ex.Message}");
                }
                AddMode = false;
                
            }
            else if(EditMode)
            {
                SelectedSchedule.EndEdit();
                var schedule = Converter.ConvertFromScheduleModel(SelectedSchedule);
                try
                {
                    using (Repository<Schedule> repo = new Repository<Schedule>())
                    {
                        var result = repo.Update(schedule);
                        if (result.Success)
                        {
                            OnInformationRequested("Data updated successfully");
                        }
                        else
                        {
                            OnErrorOccured(result.ErrorMessage);
                        }
                    }
                }
                catch (Exception ex)
                {
                    OnErrorOccured($"An error occured.\nMessage: {ex.Message}");
                }
                EditMode = false;
            }
            else if(RemoveMode)
            {
                var temp = from item in ScheduleCollection
                           where item.IsChecked
                           select item;
                var schedulesToRemove = temp.Select(x => Converter.ConvertFromScheduleModel(x));
                try
                {
                    using (Repository<Schedule> repo = new Repository<Schedule>())
                    {
                        var result = repo.Delete(schedulesToRemove);
                        if (result.Success)
                        {
                            foreach (var item in temp)
                                ScheduleCollection.Remove(item);
                            OnInformationRequested("Data removed successfully");
                        }
                        else
                        {
                            OnErrorOccured(result.ErrorMessage);
                        }
                    }
                }
                catch (Exception ex)
                {
                    OnErrorOccured($"An error occured.\nMessage: {ex.Message}");
                }
                RemoveMode = true;
            }
            CanUseDatagrid = true;
            CanEditFields = false;
            
        }

        private bool CanSave()
        {
            if(RemoveMode)
            {
                if (ScheduleCollection.Any(x => x.IsChecked))
                    return true;
                return false;
            }
            return (AddMode || EditMode);
                
        }

        private void OnCancel()
        {
            if(AddMode)
            {
                ScheduleCollection.Remove(SelectedSchedule);
                SelectedSchedule = null;
                AddMode = false;
            }
            else if(EditMode)
            {
                SelectedSchedule.CancelEdit();
                EditMode = false;
            }
            else if(RemoveMode)
            {
                foreach(var item in ScheduleCollection)
                {
                    if (item.IsChecked)
                        item.IsChecked = false;
                }
                RemoveMode = false;
            }
            CanUseDatagrid = true;
            CanEditFields = false;
        }

        private bool CanCancel()
        {
            if (RemoveMode)
                return true;
            if(AddMode || EditMode)
            {
                if (SelectedSchedule.HasErrors)
                    return false;
                return true;
            }
            return true;
        }

        protected virtual void OnErrorOccured(string message)
        {
            ErrorOccured?.Invoke(this, message);
        }
        protected virtual void OnInformationRequested(string message)
        {
            Information?.Invoke(this, message);
        }
    }
}
