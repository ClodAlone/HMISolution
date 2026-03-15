using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;
using Utilities;

namespace CommandManager.UserControls.Converters
{
    [ValueConversion(typeof(string), typeof(string))]
    public class CommandsValueConverter : IValueConverter
    {
        #region IValueConverter Members
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (targetType != typeof(string))
                throw new InvalidOperationException("The target must be a string");
            
            if (value is string)
            {
                var commands = value as string;
                CommandManagerList commandList;
                try
                {
                    if (!String.IsNullOrEmpty(commands))
                        commandList = commands.FromXml<CommandManagerList>();
                    else
                        commandList = new CommandManagerList();
                }
                catch
                {
                    commandList = new CommandManagerList();
                }

                if (commandList.Count > 0)
                {
                    var name = new StringBuilder(commandList[0].Name);
                    for (int ii = 1; ii < commandList.Count; ii++)
                        name.AppendFormat(";{0}", commandList[ii].Name);

                    return name.ToString();
                }
            }

            return null;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
        #endregion
    }
}
