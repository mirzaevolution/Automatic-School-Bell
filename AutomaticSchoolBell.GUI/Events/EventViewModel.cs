using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using CoreLib.DataAccess;
using CoreLib.Models;
using Microsoft.Win32;
using AutomaticSchoolBell.GUI.WindowsService;

namespace AutomaticSchoolBell.GUI.Events
{
    public class EventViewModel:BindableBase
    {
        private EventModel _selectedEvent;
        private bool _addMode, _editMode, _removeMode, _finishLoading, _canEditFields, _canUseDatagrid = true;
        private ObservableCollection<EventModel> _eventCollection;
        public EventModel SelectedEvent
        {
            get
            {
                return _selectedEvent;
            }
            set
            {
                OnPropertyChanged(ref _selectedEvent, value, nameof(SelectedEvent));
            }
        }
        public ObservableCollection<EventModel> EventCollection
        {
            get
            {
                return _eventCollection;
            }
            set
            {
                OnPropertyChanged(ref _eventCollection, value, nameof(EventCollection));
            }
        }
        public RelayCommand AddCommand { get; private set; }
        public RelayCommand EditCommand { get; private set; }
        public RelayCommand RemoveCommand { get; private set; }
        public RelayCommand SaveCommand { get; private set; }
        public RelayCommand CancelCommand { get; private set; }
        public RelayCommand BrowseCommand { get; private set; }
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
        public bool FinishLoading
        {
            get
            {
                return _finishLoading;
            }
            set
            {
                OnPropertyChanged(ref _finishLoading, value, nameof(FinishLoading));
            }
        }
        public event EventHandler<string> ErrorOccured;
        public event EventHandler<string> Information;
        public EventViewModel()
        {
            InitializeComponents();
        }

        public async void Load()
        {
            OnInformationRequested("Fetching data...");
            bool success = true;
            string error = "";
            FinishLoading = false;
            await Task.Run(() =>
            {
                try
                {
                    using (Repository<Event> repo = new Repository<Event>())
                    {
                        var result = repo.GetAll();
                        if (result.Status.Success)
                        {
                            if (result.Data.Count > 0)
                            {
                                var convertedList = result.Data.Select(x => Converter.ConvertToEventModel(x)).ToList();
                                EventCollection = new ObservableCollection<EventModel>(convertedList);
                            }
                            else
                                EventCollection = new ObservableCollection<EventModel>();
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
            {
                OnInformationRequested("Data loaded successfully");
                FinishLoading = true;
            }
            else
                OnErrorOccured(error);
        }
        
        private void InitializeComponents()
        {
            EventCollection = new ObservableCollection<EventModel>();
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
                    SelectedEvent.AudioLocation = openFileDialog.FileName;
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
            SelectedEvent = new EventModel
            {
                 Description = "",
                 AudioLocation = "",
                 PlayerLocation = Global.PlayerLocation,
                 Date = DateTime.Now.Date,
                 IsChecked = false,
                 StartTime = new TimeSpan(0,0,0)
            };
            EventCollection.Add(SelectedEvent);
        }

        private bool CanAdd()
        {
            if (!_addMode && !_editMode && !_removeMode && FinishLoading)
                return true;
            return false;
        }

        private void OnEdit()
        {
            EditMode = true;
            CanEditFields = true;
            CanUseDatagrid = false;
            SelectedEvent.BeginEdit();
        }

        private bool CanEdit()
        {
            if ((SelectedEvent == null) || (_addMode || _editMode || _removeMode))
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
            if ((EventCollection.Count == 0) || (_addMode || _editMode || _removeMode))
                return false;
            return true;
        }

        private async void OnSave()
        {
            if (AddMode)
            {
                var _event = Converter.ConvertFromEventModel(SelectedEvent);
                int id = 0;
                bool success = true;
                string error = "";
                OnInformationRequested("Loading...");
                await Task.Run(() =>
                {
                    try
                    {
                        using (Repository<Event> repo = new Repository<Event>())
                        {
                            var result = repo.InsertWithResult(_event);
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
                        error = $"An error occured. Message: {ex.Message}";
                    }
                });
                if(success)
                {
                    SelectedEvent.Id = id;
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
                    EventCollection.Remove(SelectedEvent);
                    SelectedEvent = null;
                    OnErrorOccured(error);
                }
                AddMode = false;

            }
            else if (EditMode)
            {
                
                var _event = Converter.ConvertFromEventModel(SelectedEvent);
                bool success = true;
                string error = "";
                OnInformationRequested("Loading...");
                await Task.Run(() =>
                {
                    try
                    {
                        using (Repository<Event> repo = new Repository<Event>())
                        {
                            var result = repo.Update(_event);
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
                        error = $"An error occured. Message: {ex.Message}";
                    }
                });
                if(success)
                {

                    SelectedEvent.EndEdit();
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

                    SelectedEvent.CancelEdit();
                    OnErrorOccured(error);
                }
                EditMode = false;
            }
            else if (RemoveMode)
            {
                var temp = from item in EventCollection
                           where item.IsChecked
                           select item;
                var eventsToRemove = temp.Select(x => Converter.ConvertFromEventModel(x));
                bool success = true;
                string error = "";
                OnInformationRequested("Loading...");
                await Task.Run(() =>
                {
                    try
                    {
                        using (Repository<Event> repo = new Repository<Event>())
                        {
                            var result = repo.Delete(eventsToRemove);
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
                        error = $"An error occured. Message: {ex.Message}";
                    }
                });
                if(success)
                {
                    //to avoid Invalid Operation Exception
                    for (int i = 0; i < EventCollection.Count; i++)
                    {
                        if (EventCollection[i].IsChecked)
                        {
                            EventCollection.RemoveAt(i);
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
            if (RemoveMode)
            {
                if (EventCollection.Any(x => x.IsChecked))
                    return true;
                return false;
            }
            return (AddMode || EditMode);
        }

        private void OnCancel()
        {
            if (AddMode)
            {
                EventCollection.Remove(SelectedEvent);
                SelectedEvent = null;
                AddMode = false;
            }
            else if (EditMode)
            {
                SelectedEvent.CancelEdit();
                EditMode = false;
            }
            else if (RemoveMode)
            {
                foreach (var item in EventCollection)
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
            if (AddMode || EditMode)
            {
                if (SelectedEvent.HasErrors)
                    return false;
                return true;
            }
            return false;
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
