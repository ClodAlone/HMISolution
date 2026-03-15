using System.Reflection;
using Utilities.ApplicationSettingsHelper;

namespace MTConnect.Properties
{
    partial class Settings
    {
        Settings()
            : base(new ConfigurationFileApplicationSettings(Assembly.GetExecutingAssembly(), typeof(Settings))) { }
    }
}
