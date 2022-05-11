using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WPF_Helpers.ValidationRules
{
    public class DateOfBirthValidation : ValidationAttribute
    {
        public override bool IsValid(object value)
        {
            DateTime dt;
            bool parsed = DateTime.TryParse((string)value, out dt);
            if (parsed)
            {
                if(dt.Year - DateTime.Now.ToLocalTime().Year < 18)
                {
                    return false;
                }
                else
                {
                    return true;
                }
            }
            return true;
        }
    }
}
