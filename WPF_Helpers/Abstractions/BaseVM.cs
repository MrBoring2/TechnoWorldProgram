using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace WPF_Helpers.Abstractions
{
    public class BaseVM : NotifyPropertyChangedBase, IDataErrorInfo
    {
        protected void ValidationMessageSetter(object value, [CallerMemberName] string propertyName = "")
        {
            OnPropertyChanged(propertyName);
            string validationresult = ValidateProperty(propertyName, value);
            if (!string.IsNullOrEmpty(validationresult))
            {
                if (_dataErrors.ContainsKey(propertyName))
                {
                    _dataErrors.Remove(propertyName);
                }
                _dataErrors.Add(propertyName, validationresult);
            }

            else if (_dataErrors.ContainsKey(propertyName))
                _dataErrors.Remove(propertyName);
        }

        protected void ValidationNotRequiredMessageSetter(bool isRequired, object value, [CallerMemberName] string propertyName = "")
        {
            if (isRequired)
            {
                ValidationMessageSetter(value, propertyName);
            }
            else
            {
                if (_dataErrors.ContainsKey(propertyName))
                {
                    _dataErrors.Remove(propertyName);
                }
            }
        }

        private string ValidateProperty(string propertyName, object value)
        {
            var results = new List<ValidationResult>(2);
            string error = string.Empty;

            bool result = Validator.TryValidateProperty(
                 value,
                 new ValidationContext(this, null, null)
                 {
                     MemberName = propertyName
                 },
                 results);

            //if (!result && (value == null || ((value is int || value is long) && (int)value == 0) || (value is decimal && (decimal)value == 0)))
            //    return null;

            //if(value is int || value is long || value is decimal || value is double || value is float)
            //{

            //}

            if (!result)
            {
                ValidationResult validationResult = results.First();
                error = validationResult.ErrorMessage;
            }

            return error;
        }

        private Dictionary<string, string> _dataErrors = new Dictionary<string, string>();

        public int GetErrorsCount => _dataErrors.Count;

        public string Error
        {
            get { return null; }
        }

        public string this[string columnName]
        {
            get
            {
                if (_dataErrors.ContainsKey(columnName))
                    return _dataErrors[columnName];
                else
                    return null;
            }
        }
    }
}
