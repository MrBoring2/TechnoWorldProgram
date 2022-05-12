using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WPF_Helpers.ValidationRules
{
    public class PhoneNumberValidation : ValidationAttribute
    {
        public override bool IsValid(object value)
        {
            string phoneNumber = value.ToString();

            if (string.IsNullOrEmpty(phoneNumber))
            {
                return false;
            }

            if (phoneNumber.Contains("_"))
            {
                return false;
            }

            return true;
        }
    }
}
