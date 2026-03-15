using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Utilities
{
    public static class TagPathHelper
    {
        public static string GetTagPath(String humanReadable, String relativePath, bool bFullReplace = true)
        {
            string stringRef = "";
            string oldChars = string.Format("{0}:", GetNS());
            string hr = "";
            if (!String.IsNullOrEmpty(humanReadable))
                hr = humanReadable.Split(new String[] { " (" }, StringSplitOptions.RemoveEmptyEntries)[0];
            if (!String.IsNullOrEmpty(relativePath))
            {
                var s = relativePath.Replace(oldChars, "");
                if (bFullReplace) {
                    s = s.Replace('/', '\\').Replace('&', '\\');
                    var tags = "Tags\\";
                    if (s.StartsWith(tags))
                        s = s.Substring(tags.Length);
                }
                stringRef = s;
                var index = hr.IndexOf(':');
                if (index != -1)
                {
                    var find = hr.Substring(0, index);
                    stringRef = stringRef.Replace(String.Format("{0}\\", find), String.Format("{0}:", find));
                }
            }
            return !String.IsNullOrEmpty(stringRef) ? stringRef : hr;
        }

        public static UInt16 GetNS()
        {
            var n = new Opc.Ua.NamespaceTable();
            return (UInt16)(n.Count + 2 - 1);
        }
    }
}
