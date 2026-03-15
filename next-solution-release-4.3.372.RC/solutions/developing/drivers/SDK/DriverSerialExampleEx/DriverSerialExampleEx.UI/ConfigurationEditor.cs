using DevExpress.Xpo;
using DriverCodeBaseEx;
using DriverCodeBaseEx.UI;
using DriverBaseInterfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DriverSerialExample.UI
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
                var ch = new DriverSerialExampleChannelSettings(ufw);
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
                var st = new DriverSerialExampleStationSettings(ufw);
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
                var drv = new DriverSerialExampleDriverSettings(ufw);
                drv.DefaultSettings();
                return drv;
            }
            return null;
        }
        #endregion

        #region internal
        internal UnitOfWork intUfW
        {
            get { return UfW; }
        }
        #endregion
    }
}
