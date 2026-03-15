using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace UnitConverterModel.Helpers
{
    public static class NameValidator
    {
        #region Declarations

        const String NameMacth = @"^[a-zA-Z][a-zA-Z0-9_]*$";
        const String NameReplace = @"[^a-zA-Z0-9_]";

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
                    return Regex.Replace(name, NameReplace, "_");
            }

            return name;
        }

        public static bool IsValidName(string name)
        {
            if (String.IsNullOrEmpty(name))
                return false;

            return Regex.IsMatch(name, NameMacth);
        }

        #endregion
    }
}
