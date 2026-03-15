using DevExpress.Xpf.Grid;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Windows.Controls;
using System.Windows.Data;

namespace TranslationHelpers
{
    public static class TranslationHelper
    {
        static string patternOpenTag = "{";
        static string patternCloseTag = "}";
        static string idPattern = $"{patternOpenTag}(?<idName>[^{patternCloseTag}]+){patternCloseTag}";
        static IDictionary<String, String> stringList;
        public static string TranslateComposedText(string sourceKey, IDictionary<String, String> stringlist, string defaultValue)
        {
            if (String.IsNullOrEmpty(sourceKey) || !CanTranslate(stringlist))
                return defaultValue;

            if (CanBeTranslated(sourceKey, stringlist))
                return stringlist[sourceKey];
            else if(IsComposed(sourceKey))
                return ReplaceIds(sourceKey, stringlist); ;

            return defaultValue;
        }

        public static string ReplaceIds(string sourceKey, IDictionary<String, String> stringlist)
        {
            stringList = stringlist;
            if (string.IsNullOrEmpty(sourceKey))
                return string.Empty;
            Regex regex = new Regex(idPattern);
            string cleanString = regex.Replace(sourceKey, ProcessIds);
            return cleanString;
        }

        private static string ProcessIds(Match m)
        {
            string tagName = m.Groups["idName"].Value;
            if (CanBeTranslated(tagName, stringList))
                return stringList[tagName];
            else
                return $"{patternOpenTag}{tagName}{patternCloseTag}";
        }

        public static bool IsComposed(string sourceKey)
        {
            if (string.IsNullOrEmpty(sourceKey))
                return false;
            Match m = Regex.Match(sourceKey, idPattern, RegexOptions.None);
            return m.Success;
        }

        public static bool CanBeTranslatedEvenIfComposed(string sourceKey, IDictionary<String, String> stringlist)
        {
            return (CanTranslate(stringlist) && IsComposed(sourceKey)) || CanBeTranslated(sourceKey, stringlist);
        }

        public static bool CanTranslate(IDictionary<String, String> stringlist)
        {
            return stringlist != null && stringlist.Count > 0;
        }

        public static bool CanBeTranslated(string sourceKey, IDictionary<String, String> stringlist)
        {
            return CanTranslate(stringlist) && !string.IsNullOrEmpty(sourceKey) && stringlist.ContainsKey(sourceKey) && !string.IsNullOrEmpty(stringlist[sourceKey]);
        }

        public static string TranlslateText(string sourceKey, IDictionary<String, String> stringlist, string original, params object[] args)
        {
            var text = TranlslateText(sourceKey, stringlist, original);
            try
            {
                return String.Format(text, args);
            }
            catch
            {
                return text;
            }
        }

        public static string TranlslateText(string sourceKey, IDictionary<String, String> stringlist, string original)
        {
            if (stringlist != null && stringlist.Count > 0 && !string.IsNullOrEmpty(sourceKey))
            {
                string sName = $"{sourceKey}";
                if (stringlist.ContainsKey(sName) && !string.IsNullOrEmpty(stringlist[sName]))
                    return stringlist[sName];
                else
                    return original;
            }

            return original;
        }

        public static void TranlslateColumns(GridColumnCollection columns, IDictionary<String, String> stringlist, string placeolder, bool keepColumnName = false)
        {
            if (stringlist != null && stringlist.Count > 0)
            {
                columns.ToList().ForEach(c => 
                {
                    string colName = $"_{placeolder}_{c.FieldName}";
                    if (!keepColumnName && !string.IsNullOrEmpty(c.Tag as string))
                        colName = $"_{placeolder}_{c.Tag as string}";
                    if (stringlist.ContainsKey(colName) && !string.IsNullOrEmpty(stringlist[colName]))
                        c.Header = stringlist[colName];
                });
            }
        }
    
        public static void TranlslateViewColumns(GridViewColumnCollection columns, IDictionary<String, String> stringlist, string placeolder)
        {
            if (stringlist != null && stringlist.Count > 0)
            {
                columns.ToList().ForEach(c =>
                {
                    if (c.DisplayMemberBinding == null)
                        return;

                    string path = string.Empty;
                    if (c.DisplayMemberBinding is MultiBinding)
                        path = (c.DisplayMemberBinding as MultiBinding).ConverterParameter.ToString();
                    else
                        path = (c.DisplayMemberBinding as Binding).Path.Path;

                    string colName = $"_{placeolder}_{path}";
                    if (stringlist.ContainsKey(colName) && !string.IsNullOrEmpty(stringlist[colName]))
                        c.Header = stringlist[colName];
                });
            }
        }
        public static void TranlslateTreeViewColumns(TreeListColumnCollection columns, IDictionary<String, String> stringlist, string placeolder, Tuple<string, string> fieldNameConversion = null)
        {
            if (stringlist != null && stringlist.Count > 0)
            {
                columns.ToList().ForEach(c =>
                {
                    var path = c.FieldName;
                    if (path == null)
                        return;
                    if (fieldNameConversion != null && path == fieldNameConversion.Item1)
                        path = String.Format("{0}_{1}", path, fieldNameConversion.Item2);

                    string colName = $"_{placeolder}_{path}";
                    if (stringlist.ContainsKey(colName) && !string.IsNullOrEmpty(stringlist[colName]))
                        c.Header = stringlist[colName];
                });
            }
        }
        public static void TranlslateGridDataColumns(GridColumnCollection columns, IDictionary<String, String> stringlist, string placeolder)
        {
            if (stringlist != null && stringlist.Count > 0)
            {
                columns.ToList().ForEach(c =>
                {
                    var fieldName = c.FieldName;
                    if (fieldName == null)
                        return;

                    string colName = $"_{placeolder}_{fieldName}";
                    if (stringlist.ContainsKey(colName) && !string.IsNullOrEmpty(stringlist[colName]))
                        c.Header = stringlist[colName];
                });
            }
        }
    }
}
