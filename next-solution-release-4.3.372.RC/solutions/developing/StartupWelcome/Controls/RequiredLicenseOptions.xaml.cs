using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace StartupWelcome.Controls
{
    public class OptionsModel
    {
        public String Title { get; set; }
        public IDictionary<string, string> Values { get; set; }
    }

    public class OptionTemplateSelector : DataTemplateSelector
    {

        public DataTemplate BooleanTemplate { get; set; }
        public DataTemplate NumberTemplate { get; set; }
        public DataTemplate DescriptionTemplate { get; set; }

        public override DataTemplate SelectTemplate(object item, DependencyObject container)
        {
            var value = item as String;
            if (String.IsNullOrEmpty(value))
                return null;

            bool bResult;
            int nResult;
            if (bool.TryParse(value, out bResult))
                return BooleanTemplate;
            else if (int.TryParse(value, out nResult))
                return NumberTemplate;
            else
                return DescriptionTemplate;
        }
    }

    [ValueConversion(typeof(String), typeof(Boolean))]
    public class StringToBooleanConverter : IValueConverter
    {
        #region IValueConverter Members

        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            try
            {
                return System.Convert.ToBoolean(value);
            }
            catch
            {
                return null;
            }
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            throw new NotImplementedException();
        }

        #endregion
    }

    [ValueConversion(typeof(String), typeof(Boolean))]
    public class StringToIntegerConverter : IValueConverter
    {
        #region IValueConverter Members

        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            try
            {
                return System.Convert.ToInt32(value);
            }
            catch
            {
                return null;
            }
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            throw new NotImplementedException();
        }
        #endregion
    }


    /// <summary>
    /// Interaction logic for RequiredLicenseOptions.xaml
    /// </summary>
    public partial class RequiredLicenseOptions : UserControl
    {
        public RequiredLicenseOptions()
        {
            InitializeComponent();
        }


        ObservableCollection<OptionsModel> options;
        public ObservableCollection<OptionsModel> Options
        {
            get
            {
                if (options == null)
                    options = new ObservableCollection<OptionsModel>();
                return options;
            }
        }
    }
}
