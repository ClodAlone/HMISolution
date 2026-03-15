using System;
using System.Windows.Controls;
using System.Xml;
using System.Windows.Markup;
using System.IO;
using Utilities;
using System.Windows;
using System.Collections.Generic;
using UFInterfaces;
using System.Collections.ObjectModel;
using System.Windows.Media;
using System.Windows.Shapes;
using System.Windows.Data;

namespace SymbolGallery
{
    [ValueConversion(typeof(Boolean), typeof(string))]
    public class GridViewToContentConverter : IValueConverter
    {
        #region IValueConverter Members
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            if (parameter != null && parameter is String)
            {
                bool bvalue;
                if(Boolean.TryParse(value.ToString(), out bvalue))
                {
                    switch (parameter.ToString())
                    {
                        case "0":
                            if (bvalue)
                                return Properties.Resources.VisualizationMode_Carousel;
                            else
                                return Properties.Resources.VisualizationMode_Grid;
                            break;
                        case "1":
                            if (bvalue)
                                return Properties.Resources.VisualizationModeTooltip_Carousel;
                            else
                                return Properties.Resources.VisualizationModeTooltip_Grid;
                            break;
                        default:
                            return Properties.Resources.VisualizationMode_Grid;
                            break;
                    }
                }
            }
            return Properties.Resources.VisualizationMode_Grid;
        }
        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            throw new NotImplementedException();
        }
        #endregion
    }
}
