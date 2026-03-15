using DocumentManager.ComponentService;
using StringManager.ComponentService;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;

namespace UFUAEditor.Converters
{
    public class TranslateConverter : IValueConverter
    {
        IDictionary<String, String> map;
        public void SetConverterDocument(IDocument doc)
        {
            var stringManager = doc.GetService(typeof(IStringEditorManager)) as IStringEditorManager;
            if (stringManager != null)
            {
                map = stringManager.GetListStringForCulture(doc, stringManager.GetActiveCulture(doc));
            }
        }

        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            if (map != null && value is String && map.ContainsKey(value as String))
                return map[value as String];
            return value;
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
