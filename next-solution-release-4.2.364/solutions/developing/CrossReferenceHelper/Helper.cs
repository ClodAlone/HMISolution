using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CrossReferenceHelper
{
    public class Helper
    {
        public static string GetNewPath(string shortname, string readablePath, string rootName = null, UInt16? ns = null)
        {
            if (ns == null)
                ns = GetNSNumber();
            if (rootName == null)
                rootName = UFUAServerInfo.UFUAServerInfo.GetTagRootName();
            string tagsprefix = readablePath != null && readablePath.StartsWith($"{ns}:{rootName}/") ? $"{ns}:{rootName}/" : string.Empty;
            shortname = shortname.Replace("\\", "/").Replace(":", "/");
            if (shortname.Contains('/'))
            {
                System.Text.StringBuilder rel = new System.Text.StringBuilder();
                foreach (String s in shortname.Split('/').ToList())
                {
                    rel.Append($"{ns}:{s}/");
                }
                readablePath = rel.ToString().Remove(rel.Length - 1);
            }
            else
            {
                readablePath = String.Format("{1}:{0}", shortname, ns);
            }

            readablePath = $"{tagsprefix}{readablePath}";
            return readablePath;
        }
        public static UInt16 GetNSNumber()
        {
            Opc.Ua.NamespaceTable n = new Opc.Ua.NamespaceTable();
            return (UInt16)(n.Count + 2 - 1);
        }
    }
}
