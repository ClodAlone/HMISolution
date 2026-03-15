using System;
using System.Reflection;
using Utilities.ApplicationSettingsHelper;

namespace MelsecQEth.Properties
{
    partial class Settings
    {
        Settings()
            : base(new ConfigurationFileApplicationSettings(Assembly.GetExecutingAssembly(), typeof(Settings))) { }
    }
}
