using DocumentManager.ComponentService;
using OPCUAViewModel;
using ScreenManager.ComponentService;
using System;
using System.Globalization;
using System.Linq;
using System.Text.RegularExpressions;
using System.Windows.Data;
using UFUAEditor.ComponentService;
using Utilities;

namespace ScreenManager.Converters
{
    /// <summary>
    /// Converts the string value of an alias to a human readable rapresentation.
    /// </summary>
    [ValueConversion(typeof(string), typeof(string))]
    internal class AliasValueToHumanReadableConverter : IValueConverter
    {
        internal IUFUAEditorManager ufuaEditor;
        internal IDocument document;

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (targetType != typeof(string))
                throw new InvalidOperationException("The target must be a string");

            if (parameter is OPCUAEntityReference)
                return (parameter as OPCUAEntityReference).StringRepresentationWithProject;

            var stringValue = value as String;
            if (String.IsNullOrEmpty(stringValue))
                return stringValue;

            var entity = GetReference(stringValue);
            if (entity != null)
                return entity.StringRepresentationWithProject;

            return stringValue;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }

        OPCUAEntityReference GetReference(String name)
        {
            var names = name.Split('.');
            if (names.Length > 1)
            {
                var datasync = OPCUAEntityReference.GetDataSinkInterface(names[0]);
                if (datasync != null)
                {
                    var varName = name.Substring(names[0].Length + 1).Replace('\\', '&').Replace('/', '&');
                    var isMissing = datasync.CheckVariable(varName);
                    if (!isMissing)
                        return datasync.GetReference(varName);
                }
            }
            else if (document != null && ufuaEditor != null)
            {
                string childSeparator = @"\\";
                var childNames = name.Replace(childSeparator, "|").Split('|');
                if (childNames.Length > 1)
                {
                    String xml = null;
                    xml = ufuaEditor.GetTagEntityReference(document, childNames.Last(), null, useCachedUow: true, Project: name.Replace(childSeparator, "."));
                    if (!String.IsNullOrEmpty(xml))
                        return xml.FromXml<OPCUAEntityReference>();
                }
                else
                {
                    String xml = null;
                    names = name.Split(':');
                    if (names.Length < 2)
                        xml = ufuaEditor.GetTagEntityReference(document, name, null, useCachedUow: true);
                    else
                        xml = ufuaEditor.GetTagEntityReference(document, names[1], names[0], useCachedUow: true);

                    if (!String.IsNullOrEmpty(xml))
                        return xml.FromXml<OPCUAEntityReference>();
                }
            }

            return null;
        }
    }
}
