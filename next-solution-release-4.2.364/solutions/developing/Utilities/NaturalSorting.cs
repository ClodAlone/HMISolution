using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace Utilities
{
    public static class WindowsNaturalSorting
    {
        [DllImport("shlwapi.dll", CharSet = CharSet.Unicode)]
        public static extern int StrCmpLogicalW(string psz1, string psz2);

        public static int Compare(string s1, string s2)
        {
            return StrCmpLogicalW(s1, s2);
        }
    }

    public static class NaturalSorting
    {
        public static int Compare(string s1, string s2)
        {
            decimal decA, decB;
            DateTime dtA, dtB;

            var tokenize = new Regex(@"[a-zA-Z]+|[0-9\.-]+"/*, RegexOptions.Compiled*/);

            // tokenize and sort only if both values are not numeric or valid dates
            if (!decimal.TryParse(s1, out decA) &&
                !decimal.TryParse(s2, out decB) &&
                !DateTime.TryParse(s1, out dtA) &&
                !DateTime.TryParse(s2, out dtB))
            {

                // tokenize on consecutive alphas or valid numbers
                MatchCollection aTok = tokenize.Matches(s1);
                MatchCollection bTok = tokenize.Matches(s2);

                // attempt to compare each token
                for (int tok = 0; tok < Math.Min(aTok.Count, bTok.Count); tok++)
                {
                    int iTok = CompareToken(aTok[tok].Value, bTok[tok].Value);
                    // only retun if find a sortable pair
                    if (iTok != 0)
                        return iTok;
                }

            }

            // otherwise compare if a simple value was found
            return CompareToken(s1, s2);
        }

        static int CompareToken(string a, string b)
        {

            decimal decA, decB;
            bool bDecA = decimal.TryParse(a, out decA), bDecB = decimal.TryParse(b, out decB);
            // attempt numeric comparison assuming decimal < * - ignore left 0-padded numerics
            if ((bDecA || bDecB) && a[0] != '0' && b[0] != '0')
            {
                if ((bDecA && !bDecB) || (bDecA && bDecB && decA < decB))
                    return -1;
                else if ((!bDecA && bDecB) || (bDecA && bDecB && decA > decB))
                    return 1;
                else
                    return 0;
            }

            // attempt DateTime comparison
            DateTime dtA, dtB;
            // datetime will take precedence if values are different types 
            bool bDtA = DateTime.TryParse(a, out dtA), bDtB = DateTime.TryParse(b, out dtB);
            if (bDtA || bDtB)
            {
                if ((bDtA && !bDtB) || (bDtA && bDtB && dtA < dtB))
                    return -1;
                else if ((!bDtA && bDtB) || (bDtA && bDtB && dtA > dtB))
                    return 1;
                else
                    return 0;
            }

            // otherwise, rely on the default string compare
            return (Comparer<object>.Default.Compare(a, b));
        }
    }
}
