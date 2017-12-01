using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using CoreLib.DataAccess;
using CoreLib.Models;


namespace AutomaticSchoolBell.GUI.RepeatedSchedules
{
    public class RepeatedScheduleViewModel:BindableBase
    {
        private RepeatedScheduleModel _selectedRepeatedSchedule;
        private ObservableCollection<RepeatedScheduleModel> _repeatedScheduleCollection;
        private bool _addMode, _editMode, _removeMode, _canEditFields, _canUseDatagrid = true;
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

        public RepeatedScheduleViewModel()
        {
            InitializeComponents();
        }
        public async void Load()
        {
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
            RepeatedScheduleCollection = new ObservableCollection<RepeatedScheduleModel>();
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
            SelectedRepeatedSchedule = new RepeatedScheduleModel
            {
                Description = "",
                AudioLocation = "",
                PlayerLocation = "",
                IsChecked = false,
                StartDateTime = new DateTime(0, 0, 0),
                Repetition = Repetition.Daily
            };
            RepeatedScheduleCollection.Add(SelectedRepeatedSchedule);
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

        private void OnSave()
        {
            if (AddMode)
            {
                var repeatedSchedule = Converter.ConvertFromRepeatedScheduleModel(SelectedRepeatedSchedule);
                try
                {
                    using (Repository<RepeatedSchedule> repo = new Repository<RepeatedSchedule>())
                    {
                        var result = repo.InsertWithResult(repeatedSchedule);
                        if (result.Status.Success)
                        {
                            SelectedRepeatedSchedule.Id = result.Data.ID;
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
            else if (EditMode)
            {
                SelectedRepeatedSchedule.EndEdit();
                var repeatedSchedule = Converter.ConvertFromRepeatedScheduleModel(SelectedRepeatedSchedule);
                try
                {
                    using (Repository<RepeatedSchedule> repo = new Repository<RepeatedSchedule>())
                    {
                        var result = repo.Update(repeatedSchedule);
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
            else if (RemoveMode)
            {
                var temp = from item in RepeatedScheduleCollection
                           where item.IsChecked
                           select item;
                var repeatedSchedulesToRemove = temp.Select(x => Converter.ConvertFromRepeatedScheduleModel(x));
                try
                {
                    using (Repository<RepeatedSchedule> repo = new Repository<RepeatedSchedule>())
                    {
                        var result = repo.Delete(repeatedSchedulesToRemove);
                        if (result.Success)
                        {
                            foreach (var item in temp)
                                RepeatedScheduleCollection.Remove(item);
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
