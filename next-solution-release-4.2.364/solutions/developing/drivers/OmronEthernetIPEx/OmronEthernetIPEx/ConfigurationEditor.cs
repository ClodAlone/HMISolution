using DevExpress.Xpo;
using DriverCodeBaseEx;
using DriverBaseInterfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OmronEthernetIP
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
                var ch = new OmronEthernetIPChannelSettings(ufw);
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
                var st = new OmronEthernetIPStationSettings(ufw);
                st.DriverSettings = ConfigurationSettings;
                st.DefaultSettings();
                return st;
            }
            return null;
        }
        protected override DriverSettings NewDriver()
        {
            if (IsValid)
                return new OmronEthernetIPDriverSettings(ufw);
            return null;
        }
        #endregion

    }
}
