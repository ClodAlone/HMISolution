using ScreenSettings.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace ScreenSettings.Documents
{
    static public class Alias
    {
        static public String ReplaceAlias(String val, Dictionary<string, string> mapAlias, ScreenDocument Document, ScreenEntity entity = null)
        {
            var ret = val;

            var matches = Regex.Matches(ret, Utilities.AliasHelper.regex);
            if (matches.Count > 0)
            {
                foreach (var match in matches)
                {
                    var m = match.ToString();
                    var key = m.Replace(Utilities.Properties.Settings.Default.OpenTagAlias, "");
                    key = key.Replace(Utilities.Properties.Settings.Default.CloseTagAlias, "");
                    if (mapAlias != null && mapAlias.ContainsKey(key) && !String.IsNullOrEmpty(mapAlias[key]))
                        ret = ret.Replace(m, mapAlias[key]);
                    else if (Document != null)
                    {
                        var aliasFound = Document.FindAlias(key, entity);
                        if (!String.IsNullOrEmpty(aliasFound))
                            ret = ret.Replace(m, aliasFound);
                    }
                }
            }

            return ret;
        }
    }
}
