using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;

namespace WPF_Helpers.ValidationRules
{
    public class StringToNumberValidationRule : ValidationRule
    {
        public override ValidationResult Validate(object value, CultureInfo cultureInfo)
        {
            if (value == null || string.IsNullOrEmpty(value.ToString()))
            {
                return new ValidationResult(false, "Поле не должно быть пустым");
            }

            if (int.TryParse(value.ToString(), out int numberInt))
            {
                return new ValidationResult(true, null);
            }
            else if (float.TryParse(value.ToString(), out float numberFloat))
            {
                return new ValidationResult(true, null);
            }
            else if (double.TryParse(value.ToString(), out double numberDouble))
            {
                return new ValidationResult(true, null);
            }
            else if (decimal.TryParse(value.ToString(), out decimal numberDecimal))
            {
                return new ValidationResult(true, null);
            }
            else
            {
                return new ValidationResult(false, "В поле можно вводить только числа");
            }
        }
    }
}
