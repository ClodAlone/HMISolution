using System;
using System.Reflection;
using Utilities.ApplicationSettingsHelper;

namespace ADFirebase
{
    // Replace "Settings" with the name of your settings file.
    partial class Settings
    {
        Settings()
            : base(new ConfigurationFileApplicationSettings(Assembly.GetExecutingAssembly(), typeof(Settings))) { }
    }
}
