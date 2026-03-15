using System;
using System.Collections.Generic;
using System.Globalization;
using UFUAModel.Helpers;
using System.Windows.Data;

namespace UFUAEditor.Converters
{
    /// <summary>
    /// This class simply converts a Boolean to a Visibility
    /// This class is kind of obsolete as there is a Standard 
    /// BooleanToVisibilityConverter within the System.Windows.Controls 
    /// namespace provided with the .NET framework, but you can not 
    /// debug that code. So this ValueConverter
    /// was provided in order that it could be debugger
    /// </summary>
    [ValueConversion(typeof(UFUAModel.UFUATag), typeof(String))]
    public class TagPathConverter : IValueConverter
    {
        public Document.UFUAServerDocument Document { get; set; }
        #region IValueConverter implementation
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var ufuatag = value as UFUAModel.UFUATag;
            if (ufuatag == null || Document == null)
                return String.Empty;

            String fullTagPath = ufuatag.FolderPath;
            if (ufuatag.PrototypeReference != null)
            {
                if (ufuatag.PrototypeReference.UFUATagOwner != null)
                {
                    var c = TagComponentsHelper.GetUFUATagComponents(Document.GetAplicationName(), ufuatag, new List<UFUAModel.UFUATag>() { ufuatag.PrototypeReference.UFUATagOwner });
                    fullTagPath = TagComponentsHelper.GetTagPath(c);
                    try
                    {
                        fullTagPath = System.IO.Path.GetDirectoryName(fullTagPath);
                        if (String.IsNullOrEmpty(ufuatag.FolderPath) && !String.IsNullOrEmpty(c.TagParent?.Name))
                            fullTagPath = String.Format("{0}\\{1}", fullTagPath, c.TagParent.Name);
                    }
                    catch { }
                }
                else if (String.IsNullOrEmpty(ufuatag.PrototypeModel))
                {
                    fullTagPath = ufuatag.PrototypeReference.Name;
                    if (!String.IsNullOrEmpty(ufuatag.FolderPath))
                        fullTagPath = String.Format("{0}\\{1}", fullTagPath, ufuatag.FolderPath);
                }
            }
            return fullTagPath;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException("This method is intentionally not implemented");
        }
        #endregion
    }
}
