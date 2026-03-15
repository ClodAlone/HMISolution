using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Data;
using System.IO;
using System.Windows.Media.Imaging;
using System.Windows;

namespace UFProjectWizard
{
    public class ImageConverter : IValueConverter
    {

        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            return ComponentService.UFProjectWizardComponent.GetControlImage("PWEditor"); 
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }

    public class ImageTruncater : IValueConverter
    {

        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            if (value == null)
            {
                return ComponentService.UFProjectWizardComponent.GetControlImage("PWEditor");
            }
            else if (value is BitmapImage)
            {
                if (value.ToString().Contains("folder"))
                {
                    return ComponentService.UFProjectWizardComponent.GetControlImage("PWEditor");
                }
                return value;
            }
            else
            {
                return ComponentService.UFProjectWizardComponent.GetControlImage("PWEditor");
            }

        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }

    public class InfoToImageConverter : IValueConverter
    {

        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            if (value == null)
            {
                return ComponentService.UFProjectWizardComponent.GetControlImage("PWEditor");
            }
            else 
            {
                return GetImage(value as string);
            }

        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            throw new NotImplementedException();
        }
        const String imageExt = ".png|.jpg|.bmp";
        private string GetImage(string _file)
        {
            String _description = string.Empty;
            try
            {
                foreach (string s in imageExt.Split('|'))
                {
                    String fileSettings = String.Format("{0}\\{1}{2}", System.IO.Path.GetDirectoryName(_file), System.IO.Path.GetFileNameWithoutExtension(_file), s);
                    if (System.IO.File.Exists(fileSettings))
                        return fileSettings;
                }
            }
            catch (Exception ex)
            {
            }

            return ComponentService.UFProjectWizardComponent.GetControlImage("PWEditor").UriSource.OriginalString;
        }
    }

    public class BoolToVisibilityCollapseConverter : IValueConverter
    {
        #region IValueConverter implementation
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            Boolean input = (Boolean)value;

            if (parameter != null)
            {
                Boolean invert = Boolean.Parse(parameter.ToString());
                if (invert)
                    input = !input;
            }

            if (input)
                return Visibility.Visible;
            return Visibility.Collapsed;
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            throw new NotImplementedException("This method is intentionally not implemented");
        }
        #endregion
    }
}
