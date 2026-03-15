using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;
using UFUAModel.Extensions;

namespace UFRecipeLayout.LayoutItemControls
{
    internal class CheckBoxValueConverter : IValueConverter
    {
        #region Declarations
        readonly UFUAModel.DataType valueType;
        #endregion

        #region Constructors
        public CheckBoxValueConverter(UFUAModel.DataType valueType)
        {
            this.valueType = valueType;
        }
        #endregion

        #region IValueConverter Members

        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            if (targetType != typeof(Nullable<bool>))
                throw new InvalidOperationException("The target must be a Nullable<bool>");

            if (value != null)
            {
                bool isChecked;
                double dValue;

                if (bool.TryParse(value.ToString(), out isChecked))
                    return isChecked;
                else if (double.TryParse(value.ToString(), out dValue))
                    return dValue > 0.0;
            }

            return false;
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            return DataReader.Extensions.TypeExtensions.ChangeType(value, valueType.ToNetType(), force: true);
        }

        #endregion

    }
}
