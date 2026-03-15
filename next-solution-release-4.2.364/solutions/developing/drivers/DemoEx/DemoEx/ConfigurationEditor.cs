using DriverCodeBaseEx;
using DriverBaseInterfaces;

namespace Demo
{
    public class ConfigurationEditor : BaseConfigurationEditor, IConfigurationEditor
    {
        public ConfigurationEditor() :
            base()
        { }

        #region Virtual
        public override ChannelSettings NewChannel()
        {
            if (IsValid)
            {
                var ch = new DemoChannelSettings(ufw);
                ch.DriverSettings = ConfigurationSettings;
                ch.DefaultSettings();
                return ch;
            }
            return null;
        }
        public override StationSettings NewStation()
        {
            if (IsValid)
            {
                var st = new DemoStationSettings(ufw);
                st.DriverSettings = ConfigurationSettings;
                st.DefaultSettings();
                return st;
            }
            return null;
        }
        protected override DriverSettings NewDriver()
        {
            if (IsValid)
                return new DemoDriverSettings(ufw);
            return null;
        }
        #endregion

    }
}
