using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace UFUAModel.Helpers
{
    public static class NameValidator
    {
        #region Declarations

        const String NameMacth = @"^[a-zA-Z][a-zA-Z0-9_]*$";
        const String TableNameMacth = @"^[\p{L}0-9_\s]+$";
        const String NameReplace = @"[^a-zA-Z0-9_]";
        const String AlarmNameMacth = @"^[a-zA-Z][a-zA-Z0-9_ ]*$";
        const String EUNameMacth = @"^[a-zA-Z0-9_]*$";
        #endregion

        #region Public Methods

        public static string EnsureValidName(string name)
        {
            if (name != null)
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
                    return Regex.Replace(name, NameReplace, "_", RegexOptions.IgnoreCase | RegexOptions.CultureInvariant | RegexOptions.Compiled);
            }

            return name;
        }

        public static string RemoveInvalidCharacters(string name)
        {
            if (name != null)
            {
                // replace any invalid character from input string
                if (!Regex.IsMatch(name, NameMacth))
                    return Regex.Replace(name, NameReplace, String.Empty, RegexOptions.IgnoreCase | RegexOptions.CultureInvariant | RegexOptions.Compiled);
            }

            return name;
        }

        public static string EnsureValidNameWithPrefix(string name, string prefix)
        {
            string validatedName = name;
            if (!String.IsNullOrEmpty(name))
            {
                string nameToBeValidated = name;
                if (!String.IsNullOrEmpty(prefix))
                {
                    if (!Char.IsLetter(name[0]))
                    {
                        nameToBeValidated = prefix + name;
                    }
                }
                validatedName = EnsureValidName(nameToBeValidated);
            }

            return validatedName;
        }

        public static bool IsValidName(string name)
        {
            if (String.IsNullOrEmpty(name))
                return false;

            return Regex.IsMatch(name, NameMacth);
        }

        public static bool IsValidTableName(string name)
        {
            if (String.IsNullOrEmpty(name))
                return false;

            return Regex.IsMatch(name, TableNameMacth);
        }

        public static bool IsValidAlarmName(string name)
        {
            if (String.IsNullOrEmpty(name))
                return false;

            return Regex.IsMatch(name, AlarmNameMacth);
        }
        public static bool IsValidEUName(string name)
        {
            if (String.IsNullOrEmpty(name))
                return false;

            return Regex.IsMatch(name, EUNameMacth);
        }
        #endregion
    }
}
