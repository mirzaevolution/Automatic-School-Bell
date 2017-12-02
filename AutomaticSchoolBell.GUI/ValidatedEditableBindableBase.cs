using System;
using System.Collections.Generic;
using System.Linq;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Collections;

namespace AutomaticSchoolBell.GUI
{
    public class ValidatedEditableBindableBase<T> : EditableBindableBase<T>, INotifyDataErrorInfo
    {
        private Dictionary<string, List<string>> _errors = new Dictionary<string, List<string>>();
        public bool HasErrors => _errors.Count > 0;

        public event EventHandler<DataErrorsChangedEventArgs> ErrorsChanged = delegate { };

        public IEnumerable GetErrors(string propertyName)
        {
            if (String.IsNullOrEmpty(propertyName))
                return null;
            if (_errors.ContainsKey(propertyName))
                return _errors[propertyName];
            return null;
        }
        protected override void OnPropertyChanged<PropType>(ref PropType member, PropType value, string propertyName)
        {
            base.OnPropertyChanged(ref member, value, propertyName);
            Validate(propertyName, value);
        }
        private void Validate<PropType>(string propertyName, PropType value)
        {
            List<ValidationResult> result = new List<ValidationResult>();

            Validator.TryValidateProperty(value, new ValidationContext(this)
            {
                MemberName = propertyName
            }, result);
            if (result.Any())
                _errors[propertyName] = result.Select(x => x.ErrorMessage).ToList();
            else
                _errors.Remove(propertyName);
            ErrorsChanged?.Invoke(this, new DataErrorsChangedEventArgs(propertyName));
        }
    }
}
