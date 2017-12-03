using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using CoreLib.DataAccess;
using CoreLib.Models;
using Microsoft.Win32;
using AutomaticSchoolBell.GUI.WindowsService;
namespace AutomaticSchoolBell.GUI.Schedules
{
    public class ScheduleViewModel:BindableBase
    {
        
        private ScheduleModel _selectedSchedule;
        private ObservableCollection<ScheduleModel> _scheduleCollection;
        private bool _addMode, _editMode, _removeMode, _canEditFields=false, _canUseDatagrid = true;
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
        public RelayCommand AddCommand { get; private set; } //ctrl + shift + a
        public RelayCommand EditCommand { get; private set; } //ctrl + shift + e
        public RelayCommand RemoveCommand { get; private set; } //ctrl + delete
        public RelayCommand SaveCommand { get; private set; } //ctrl + s
        public RelayCommand BrowseCommand { get; private set; } //ctrl + b
        public RelayCommand CancelCommand { get; private set; } //ctrl + q
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
            OnInformationRequested("Fetching data...");
            bool success = true;
            string error = "";
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
                        {
                            error = result.Status.ErrorMessage;
                            success = false;
                        }
                    }
                }
                catch (Exception ex)
                {
                    success = false;
                    error = $"An error occured. Message: {ex.Message}";
                }
            });

            //to avoid cross thread exception
            if (success)
                OnInformationRequested("Data loaded successfully");
            else
                OnErrorOccured(error);
        }
        private void InitializeComponents()
        {
            ScheduleCollection = new ObservableCollection<ScheduleModel>();
            AddCommand = new RelayCommand(OnAdd, CanAdd);
            EditCommand = new RelayCommand(OnEdit, CanEdit);
            RemoveCommand = new RelayCommand(OnRemove, CanRemove);
            SaveCommand = new RelayCommand(OnSave, CanSave);
            CancelCommand = new RelayCommand(OnCancel, CanCancel);
            BrowseCommand = new RelayCommand(OnBrowse, CanBrowse);
        }
        private void OnBrowse()
        {
            try
            {
                OpenFileDialog openFileDialog = new OpenFileDialog
                {
                    CheckFileExists = true,
                    CheckPathExists = true,
                    FileName = "",
                    Filter = "Mp3 File (*.mp3)|*.mp3"
                };
                if (openFileDialog.ShowDialog().Value)
                {
                    SelectedSchedule.AudioLocation= openFileDialog.FileName;
                }
            }
            catch (Exception ex)
            {
                OnErrorOccured(ex.Message);
            }
        }
        private bool CanBrowse()
        {
            return (AddMode || EditMode);
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
                PlayerLocation = Global.PlayerLocation,
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

        private async void OnSave()
        {
            if(AddMode)
            {
                var schedule = Converter.ConvertFromScheduleModel(SelectedSchedule);
                int id = 0;
                bool success = true;
                string error = "";
                OnInformationRequested("Loading...");
                await Task.Run(() =>
                {
                    try
                    {
                        using (Repository<Schedule> repo = new Repository<Schedule>())
                        {
                            var result = repo.InsertWithResult(schedule);
                            if (result.Status.Success)
                            {
                                id = result.Data.ID;
                            }
                            else
                            {
                                success = false;
                                error = result.Status.ErrorMessage;
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        success = false;
                        error = $"An error occured.Message: {ex.Message}";
                    }
                });
                if (success)
                {
                    SelectedSchedule.Id = id;
                    OnInformationRequested("Data added successfully, refreshing service in the background...");
                    await Task.Run(() =>
                    {
                        var result = Controller.RefreshService();
                        if (!result.Success)
                        {
                            success = false;
                            error = result.ErrorMessage;
                        }
                    });
                    if (success)
                    {
                        OnInformationRequested("Service refreshed successfully");
                    }
                    else
                    {
                        OnErrorOccured(error);
                    }
                }
                else
                {
                    ScheduleCollection.Remove(SelectedSchedule);
                    SelectedSchedule = null;
                    OnErrorOccured(error);
                }
                AddMode = false;
                
            }
            else if(EditMode)
            {
                
                var schedule = Converter.ConvertFromScheduleModel(SelectedSchedule);

                bool success = true;
                string error = "";
                OnInformationRequested("Loading...");
                await Task.Run(() =>
                {
                    try
                    {
                        using (Repository<Schedule> repo = new Repository<Schedule>())
                        {
                            var result = repo.Update(schedule);
                            if (!result.Success)
                            {
                                success = false;
                                error = result.ErrorMessage;

                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        success = false;
                        error = $"An error occured.Message: {ex.Message}";

                    }
                });
                if(success)
                {

                    SelectedSchedule.EndEdit();
                    OnInformationRequested("Data updated successfully, refreshing service in the background...");
                    await Task.Run(() =>
                    {
                        var result = Controller.RefreshService();
                        if (!result.Success)
                        {
                            success = false;
                            error = result.ErrorMessage;
                        }
                    });
                    if (success)
                    {
                        OnInformationRequested("Service refreshed successfully");
                    }
                    else
                    {
                        OnErrorOccured(error);
                    }
                }
                else
                {
                    SelectedSchedule.CancelEdit();
                    OnErrorOccured(error);
                }
                EditMode = false;
            }
            else if(RemoveMode)
            {
                var temp = from item in ScheduleCollection
                           where item.IsChecked
                           select item;
                var schedulesToRemove = temp.Select(x => Converter.ConvertFromScheduleModel(x));
                bool success = true;
                string error = "";
                OnInformationRequested("Loading...");
                await Task.Run(() =>
                {
                    try
                    {
                        using (Repository<Schedule> repo = new Repository<Schedule>())
                        {
                            var result = repo.Delete(schedulesToRemove);
                            if (!result.Success)
                            {
                                success = false;
                                error = result.ErrorMessage;
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        success = false;
                        error = $"An error occured.Message: {ex.Message}";
                    }
                });
                if(success)
                {
                    //to avoid Invalid Operation Exception
                    for (int i = 0; i < ScheduleCollection.Count; i++)
                    {
                        if (ScheduleCollection[i].IsChecked)
                        {
                            ScheduleCollection.RemoveAt(i);
                            i--;
                        }

                    }
                    OnInformationRequested("Data removed successfully, refreshing service in the background...");
                    await Task.Run(() =>
                    {
                        var result = Controller.RefreshService();
                        if (!result.Success)
                        {
                            success = false;
                            error = result.ErrorMessage;
                        }
                    });
                    if (success)
                    {
                        OnInformationRequested("Service refreshed successfully");
                    }
                    else
                    {
                        OnErrorOccured(error);
                    }
                }
                else
                {
                    OnErrorOccured(error);
                }
                RemoveMode = false;
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
            if (AddMode || EditMode)
            {
                if (SelectedSchedule.HasErrors)
                    return false;
                return true;
            }
            return false;
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
            return (AddMode || EditMode);
            
           
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
