using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Utilities
{
    public static class NamespaceTableConverter
    {
        public static string WholeFolderWildChar = "\\*";

        static Opc.Ua.NamespaceTable n = new Opc.Ua.NamespaceTable();
        static UInt16 ns = (UInt16)(n.Count + 2 - 1);
        static string tagsRootName = $"Tags";
        static string formattedNsChars = $"{ns}:";
        static string formattedRootTags = $"{ns}:{tagsRootName}/";
        public static string FormattedRootTags { get { return formattedRootTags; } }
        public static string FormattedNsChars { get { return formattedNsChars; } }
        public static string GetSanitizedValue(string value)
        {
            return value?.Replace(FormattedNsChars, string.Empty);
        }
        public static string GetSanitizedReadableValue(string value)
        {
            var sanitized = GetSanitizedValue(value);
            var s = sanitized.Replace('/', '\\').Replace('&', '\\');
            var tags = "Tags\\";
            if (s.StartsWith(tags, StringComparison.OrdinalIgnoreCase))
                s = s.Substring(tags.Length);
            return s;
        }
        public static string GetRelativePathValue(string value)
        {
            var relativePath = value;
            if (relativePath.EndsWith(NamespaceTableConverter.WholeFolderWildChar))
            {
                relativePath = relativePath.Substring(0, relativePath.Length - NamespaceTableConverter.WholeFolderWildChar.Length);
                relativePath = relativePath.Replace("\\", "/");
                relativePath = relativePath.Replace("/", String.Format("/{0}", NamespaceTableConverter.FormattedNsChars));
                relativePath = String.Format("{0}Tags/{0}{1}/{0}", NamespaceTableConverter.FormattedNsChars, relativePath);
            }
            return relativePath;
        }
        public static string GetRelativePath(string value)
        {
            if (!string.IsNullOrEmpty(value))
            {
                Opc.Ua.NamespaceTable n = new Opc.Ua.NamespaceTable();
                UInt16 ns = (UInt16)(n.Count + 2 - 1);

                var find = String.Format("{0}:{1}\\", ns, tagsRootName);
                var ret = value;
                if (ret.StartsWith(find, true, System.Globalization.CultureInfo.CurrentCulture))
                    ret = ret.Substring(find.Length);
                find = String.Format("{0}:{1}/", ns, tagsRootName);
                if (ret.StartsWith(find, true, System.Globalization.CultureInfo.CurrentCulture))
                    ret = ret.Substring(find.Length);

                string oldChars = string.Format("{0}:", ns);
                ret = ret.Replace(oldChars, "");
                ret = ret.Replace("/", "\\");
                return ret;
            }

            return String.Empty;
        }
    }
}
