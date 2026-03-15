using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace Utilities
{
    static public class AliasHelper
    {
        static public bool ContainsAlias(String val)
        {
            return !String.IsNullOrEmpty(val) && val.IndexOf(Properties.Settings.Default.OpenTagAlias) != -1;
        }

        static public int GetAliasCount(String val)
        {
            return Regex.Matches(val, regex).Count;
        }

        static public String regex = String.Format(@"\{0}([^{1}]*)\{2}", Properties.Settings.Default.OpenTagAlias,
            Properties.Settings.Default.CloseTagAlias,
            Properties.Settings.Default.CloseTagAlias);

        static public List<String> ListAliasFromString(String val)
        {
            var ret = new List<String>();
            var matches = Regex.Matches(val, regex);
            if (matches.Count > 0)
            {
                foreach (var match in matches)
                {
                    var m = match.ToString();
                    var key = m.Replace(Properties.Settings.Default.OpenTagAlias, "");
                    key = key.Replace(Properties.Settings.Default.CloseTagAlias, "");
                    if (!ret.Contains(key))
                        ret.Add(key);
                }
            }

            return ret;
        }
    }
}
