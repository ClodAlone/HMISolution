using DevExpress.Utils.About;
using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace Utilities
{
    public static class NewNameHelper
    {
        #region Declarations
        public const String NewNameRegExpr = @"\d+?$";
        #endregion

        #region Public Methods
        public static String FindNewName(String name, IList<String> listname, IDictionary<string, ulong> mapcounter = null, String format = "{0}{1}", bool bRemoveEndsNumbers = true)
        {
            ulong counter = 1;
            string fmtzero = "0";
            string baseName = name;
            if (bRemoveEndsNumbers)
                ParseName(name, mapcounter, out baseName, out fmtzero, out counter);

            while (listname.Contains(name))
                name = String.Format(format, baseName, (counter++).ToString(fmtzero));
            listname.Add(name);

            if (mapcounter != null)
                mapcounter[baseName] = counter;

            return name;
        }

        public static void ParseName(String name, out String baseName, out String fmtzero, out ulong counter)
        {
            ParseName(name, null, out baseName, out fmtzero, out counter);
        }
        #endregion

        #region Private Methods
        static void ParseName(String name, IDictionary<string, ulong> mapcounter, out String baseName, out String fmtzero,out  ulong counter)
        {
            counter = 1;
            fmtzero = "0";
            baseName = Regex.Replace(name, NewNameRegExpr, "");
            var match = Regex.Match(name, NewNameRegExpr);
            if (match.Success && !String.IsNullOrEmpty(match.Value))
            {
                fmtzero = new String('0', match.Value.Length);
                if (mapcounter == null || !mapcounter.TryGetValue(baseName, out counter))
                {
                    if (!ulong.TryParse(match.Value, out counter))
                        counter = 1;
                }
            }
        }
        #endregion
    }
}
