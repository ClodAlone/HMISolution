using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Data;
using DevExpress.Xpf.LayoutControl;

namespace StartupWelcome.Converters
{
    public class ScalablePaddingConverter : IValueConverter
    {
        public ScalablePaddingConverter()
        {
            MinPadding = 35;
        }

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var controlHeight = (double)value;
            double desiredContentHeight = 3 * Tile.LargeHeight + 2 * TileLayoutControl.DefaultItemSpace + 40;
            double paddingY = Math.Floor(Math.Max(0, controlHeight - desiredContentHeight) / 2);
            paddingY = Math.Max(MinPadding, Math.Min(paddingY, TileLayoutControl.DefaultPadding.Top));
            double relativePadding = (paddingY - MinPadding) / (TileLayoutControl.DefaultPadding.Top - MinPadding);
            double paddingX = Math.Floor(MinPadding + relativePadding * (TileLayoutControl.DefaultPadding.Left - MinPadding));
            return new Thickness(paddingX, paddingY, paddingX, paddingY);
        }
        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }

        public double MinPadding { get; set; }
    }
}
