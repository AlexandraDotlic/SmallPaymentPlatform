using System;
using System.Collections.Generic;
using System.Text;

namespace Common.Utils
{
    public static class JMBGParser
    {
        public static int CalculatePersonsYearsFromJMBG(string jmbg)
        {
            string day = jmbg.Substring(0, 2);
            string month = jmbg.Substring(2, 2);
            string year = jmbg.Substring(4, 3);
            if (year.StartsWith("0")){
                year = "2" + year;
            }
            else
            {
                year = "1" + year;
            }
            string dateOfBirthString = day + month + year;
            DateTime dateOfBirth = DateTime.ParseExact(dateOfBirthString, "ddmmyyyy", System.Globalization.CultureInfo.InvariantCulture);
            DateTime now = DateTime.Today;
            int age = now.Year - dateOfBirth.Year;
            if (dateOfBirth > now.AddYears(-age)) 
                age--;
            return age;
        }
    }
}
