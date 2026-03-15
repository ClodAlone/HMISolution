using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;
using DevExpress.Xpf.Gauges;

namespace Meters.Converters
{
    class PredefinedElementKindToTickmarksModel : IValueConverter
    {
        #region IValueConverter implementation
        public object Convert(object value, System.Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            PredefinedElementKind gaugeModelKind = (LinearScale.PredefinedTickmarksPresentations as IEnumerable<PredefinedElementKind>).ElementAt((int)value) as PredefinedElementKind;

            if (gaugeModelKind != null)
                return Activator.CreateInstance(gaugeModelKind.Type);
            return value;
        }
        public object ConvertBack(object value, System.Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            return null;
        }
        #endregion
    }
}
