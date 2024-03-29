﻿using System;
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
            bool parsed = DateTime.TryParse(value.ToString(), out dt);
            if (parsed)
            {
                if(DateTime.Now.ToLocalTime().Year - dt.ToLocalTime().Year < 18 || DateTime.Now.ToLocalTime().Year - dt.ToLocalTime().Year > 80)
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
