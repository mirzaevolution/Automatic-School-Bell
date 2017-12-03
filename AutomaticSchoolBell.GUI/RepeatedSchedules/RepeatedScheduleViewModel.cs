using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using CoreLib.DataAccess;
using CoreLib.Models;
using Microsoft.Win32;
using AutomaticSchoolBell.GUI.WindowsService;

namespace AutomaticSchoolBell.GUI.RepeatedSchedules
{
    public class RepeatedScheduleViewModel:BindableBase
    {
        private RepeatedScheduleModel _selectedRepeatedSchedule;
        private ObservableCollection<RepeatedScheduleModel> _repeatedScheduleCollection;
        private bool _addMode, _editMode, _removeMode, _finishLoading, _canEditFields, _canUseDatagrid = true;
        public RepeatedScheduleModel SelectedRepeatedSchedule
        {
            get
            {
                return _selectedRepeatedSchedule;
            }
            set
            {
                OnPropertyChanged(ref _selectedRepeatedSchedule, value, nameof(SelectedRepeatedSchedule));
            }
        }
        public ObservableCollection<RepeatedScheduleModel> RepeatedScheduleCollection
        {
            get
            {
                return _repeatedScheduleCollection;
            }
            set
            {
                OnPropertyChanged(ref _repeatedScheduleCollection, value, nameof(RepeatedScheduleCollection));
            }
        }
        public RelayCommand AddCommand { get; private set; }
        public RelayCommand EditCommand { get; private set; }
        public RelayCommand RemoveCommand { get; private set; }
        public RelayCommand SaveCommand { get; private set; }

        public RelayCommand BrowseCommand { get; private set; }
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

        public RepeatedScheduleViewModel()
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
                    using (Repository<RepeatedSchedule> repo = new Repository<RepeatedSchedule>())
                    {
                        var result = repo.GetAll();
                        if (result.Status.Success)
                        {
                            if (result.Data.Count > 0)
                            {
                                var convertedList = result.Data.Select(x => Converter.ConvertToRepeatedScheduleModel(x)).ToList();
                                RepeatedScheduleCollection = new ObservableCollection<RepeatedScheduleModel>(convertedList);
                            }
                            else
                                RepeatedScheduleCollection  = new ObservableCollection<RepeatedScheduleModel>();
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
            RepeatedScheduleCollection = new ObservableCollection<RepeatedScheduleModel>();
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
                    SelectedRepeatedSchedule.AudioLocation = openFileDialog.FileName;
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
            SelectedRepeatedSchedule = new RepeatedScheduleModel
            {
                Description = "",
                AudioLocation = "",
                PlayerLocation = Global.PlayerLocation,
                IsChecked = false,
                StartDateTime = DateTime.Now,
                Repetition = Repetition.Daily
            };
            RepeatedScheduleCollection.Add(SelectedRepeatedSchedule);
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
            SelectedRepeatedSchedule.BeginEdit();
        }

        private bool CanEdit()
        {
            if ((SelectedRepeatedSchedule == null) || (_addMode || _editMode || _removeMode))
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
            if ((RepeatedScheduleCollection.Count == 0) || (_addMode || _editMode || _removeMode))
                return false;
            return true;
        }

        private async void OnSave()
        {
            if (AddMode)
            {
                var repeatedSchedule = Converter.ConvertFromRepeatedScheduleModel(SelectedRepeatedSchedule);
                bool success = true;
                string error = "";
                int id = 0;
                OnInformationRequested("Loading...");
                await Task.Run(() =>
                {
                    try
                    {
                        using (Repository<RepeatedSchedule> repo = new Repository<RepeatedSchedule>())
                        {
                            var result = repo.InsertWithResult(repeatedSchedule);
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
                    SelectedRepeatedSchedule.Id = id;
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
                    RepeatedScheduleCollection.Remove(SelectedRepeatedSchedule);
                    SelectedRepeatedSchedule = null;
                    OnErrorOccured(error);
                }
                AddMode = false;

            }
            else if (EditMode)
            {

                var repeatedSchedule = Converter.ConvertFromRepeatedScheduleModel(SelectedRepeatedSchedule);
                bool success = true;
                string error = "";
                OnInformationRequested("Loading...");
                await Task.Run(() =>
                {
                    try
                    {
                        using (Repository<RepeatedSchedule> repo = new Repository<RepeatedSchedule>())
                        {
                            var result = repo.Update(repeatedSchedule);
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
                if (success)
                {
                    SelectedRepeatedSchedule.EndEdit();
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
                    SelectedRepeatedSchedule.CancelEdit();
                    OnErrorOccured(error);
                }
                EditMode = false;
            }
            else if (RemoveMode)
            {
                var temp = from item in RepeatedScheduleCollection
                           where item.IsChecked
                           select item;
                var repeatedSchedulesToRemove = temp.Select(x => Converter.ConvertFromRepeatedScheduleModel(x));
                bool success = true;
                string error = "";
                OnInformationRequested("Loading...");
                await Task.Run(() =>
                {
                    try
                    {
                        using (Repository<RepeatedSchedule> repo = new Repository<RepeatedSchedule>())
                        {
                            var result = repo.Delete(repeatedSchedulesToRemove);
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
                if (success)
                {
                    //to avoid Invalid Operation Exception
                    for (int i = 0; i < RepeatedScheduleCollection.Count; i++)
                    {
                        if (RepeatedScheduleCollection[i].IsChecked)
                        {
                            RepeatedScheduleCollection.RemoveAt(i);
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
                if (RepeatedScheduleCollection.Any(x => x.IsChecked))
                    return true;
                return false;
            }
            return (AddMode || EditMode);
        }

        private void OnCancel()
        {
            if (AddMode)
            {
                RepeatedScheduleCollection.Remove(SelectedRepeatedSchedule);
                SelectedRepeatedSchedule = null;
                AddMode = false;
            }
            else if (EditMode)
            {
                SelectedRepeatedSchedule.CancelEdit();
                EditMode = false;
            }
            else if (RemoveMode)
            {
                foreach (var item in RepeatedScheduleCollection)
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
                if (SelectedRepeatedSchedule.HasErrors)
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
