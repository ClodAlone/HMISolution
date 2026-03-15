using DevExpress.Xpo;
using DriverCodeBaseEx;
using DriverBaseInterfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IEC60870_5_104
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
                var ch = new IEC60870_5_104ChannelSettings(ufw);
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
                var st = new IEC60870_5_104StationSettings(ufw);
                st.DriverSettings = ConfigurationSettings;
                st.DefaultSettings();
                return st;
            }
            return null;
        }
        protected override DriverSettings NewDriver()
        {
            if (IsValid)
                return new IEC60870_5_104DriverSettings(ufw);
            return null;
        }
        #endregion
    }
}
