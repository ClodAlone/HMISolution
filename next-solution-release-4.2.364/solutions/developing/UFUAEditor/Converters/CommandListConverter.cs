using CommandManager;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Data;
using Utilities;

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
    [ValueConversion(typeof(String), typeof(String))]
    public class CommandListConverter : IValueConverter
    {
        #region IValueConverter implementation
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            try
            {
                string input = value as string;
                if (input == null)
                    return String.Empty;
                string output = String.Empty;
                var commandList = input.FromXml<CommandManagerList>();
                if (commandList != null)
                    foreach (var cmd in commandList)
                        output = String.Format("{0}{1} ({2}){3}", output, cmd.Name, cmd.CommandSummary, cmd == commandList.Last() ? String.Empty : Environment.NewLine);
                return output;
            }
            catch
            {
                return String.Empty;
            }
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException("This method is intentionally not implemented");
        }
        #endregion
    }
}
