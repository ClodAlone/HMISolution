using System;
using System.Linq;
using System.Windows.Data;
using DevExpress.Xpo;
using UFUAEditor.Document;

namespace UFUAEditor.Converters
{
    public class EUTagsListConverter : IValueConverter
    {
        public UFUAServerDocument Document { get; set; }
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            var EUName = value as string;

            if (Document == null || String.IsNullOrEmpty(EUName))
                return null;

            return (from tag in new XPQuery<UFUAModel.UFUATag>(Document.GetSession(), true)/*.AsParallel()*/ where tag.UFUAEngineeringUnit == EUName select tag).ToList();
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
