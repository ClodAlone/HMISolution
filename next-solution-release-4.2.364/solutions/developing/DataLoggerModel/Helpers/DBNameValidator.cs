using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace DataLoggerModel.Helpers
{
    public static class DBNameValidator
    {
        #region Declarations

        const String NameMacth = @"^[a-zA-Z][a-zA-Z0-9_\s*]*$";
        const String TableNameMacth = @"^[\p{L}0-9_\s]+$";
        const String NameReplace = @"[^a-zA-Z0-9_\s*]";

        #endregion

        #region Public Methods

        public static string EnsureValidName(string name)
        {
            int index = 0;
            for (index = 0; index < name.Length; index++)
            {
                // search for any non letter character from input string
                if (Char.IsLetter(name[index]))
                    break;
            }

            // remove any non letter character from input string
            name = name.Substring(index);

            // replace any invalid character from input string
            if (!Regex.IsMatch(name, NameMacth))
                return Regex.Replace(name, NameReplace, "_");

            return name;
        }

        public static bool IsValidName(string name)
        {
            return Regex.IsMatch(name, NameMacth);
        }

        public static bool IsValidTableName(string name)
        {
            if (String.IsNullOrEmpty(name))
                return false;

            return Regex.IsMatch(name, TableNameMacth);
        }

        #endregion
    }
}
