using System;
using System.Reflection;
using Utilities.ApplicationSettingsHelper;

namespace MSZWebService.Properties
{
    partial class Settings
    {
        Settings()
            : base(new ConfigurationFileApplicationSettings(Assembly.GetExecutingAssembly(), typeof(Settings))) { }
    }
}
