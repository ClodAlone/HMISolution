using System;
using System.Reflection;
using Utilities.ApplicationSettingsHelper;

namespace SchedulerRTControl.Properties
{
   partial  class Settings
    {
        Settings()
            : base(new ConfigurationFileApplicationSettings(Assembly.GetExecutingAssembly(), typeof(Settings))) { }
    }
}
