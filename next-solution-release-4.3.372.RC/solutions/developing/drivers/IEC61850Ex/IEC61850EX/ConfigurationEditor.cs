using DriverCodeBaseEx;
using DriverBaseInterfaces;

namespace IEC61850
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
                var ch = new IEC61850ChannelSettings(ufw);
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
                var st = new IEC61850StationSettings(ufw);
                st.DriverSettings = ConfigurationSettings;
                st.DefaultSettings();
                return st;
            }
            return null;
        }
        protected override DriverSettings NewDriver()
        {
            if (IsValid)
            {
                var drv = new IEC61850DriverSettings(ufw);
                drv.DefaultSettings();
                return drv;
            }
            return null;
        }
        #endregion
    }
}
