using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ADPluginSettingsInterface
{
    public interface IPluginWpfEditing
    {
        System.Windows.Controls.UserControl GeneralSettingsEditor { get; }
        bool SaveSettings(System.Windows.Controls.UserControl control);
        bool CopyFile(string sourceconn, string targetconn);

        System.Windows.Controls.UserControl PluginTestEditor { get; }
    }
}
