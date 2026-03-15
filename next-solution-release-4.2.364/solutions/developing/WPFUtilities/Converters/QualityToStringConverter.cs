using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using System.Windows;
using System.Windows.Data;
using Opc.Ua;
using UFInterfaces.Converters;
using System.ComponentModel;
using TranslationHelpers;

namespace WPFUtilities.Converters
{
    public class QualityToStringConverter : IQualityToStringConverter
    {
        #region Converters.IQualityToStringConverter
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public IDictionary<string, string> CurrentStringList { get; set; }
        #endregion

        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            if (value == null)
                return value;
            try
            {
                string original = value as string;
                uint quality = StatusCodes.GetIdentifier(value as string);
                switch (quality)
                {
                    case StatusCodes.Good:
                        return Properties.Resources.QualityGood;
                    case StatusCodes.Bad:
                        return Properties.Resources.QualityBad;
                    case StatusCodes.Uncertain:
                        return Properties.Resources.QualityUncertain;
                    default:
                        {
                            if(!string.IsNullOrEmpty(original) && CurrentStringList != null)
                            {
                                return TranslationHelper.TranlslateText(original, CurrentStringList, original);
                            }
                        }
                        
                        break;
                }
            }
            catch (Exception)
            {
                return value;
            }

            return value;
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
