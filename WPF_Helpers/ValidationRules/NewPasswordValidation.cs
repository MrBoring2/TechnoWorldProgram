using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WPF_Helpers.ValidationRules
{
    public class NewPasswordValidation : ValidationAttribute
    {
        private bool _isEnabled;
        private int _minLength;
        private int _maxLength;
        public NewPasswordValidation(int minLength, int maxLength) : base()
        {
            _maxLength = maxLength;
            _minLength = minLength;
        }
        public override bool IsValid(object value)
        {
            if (!_isEnabled)
            {
                return true;
            }
            return base.IsValid(value);
        }
    }
}
