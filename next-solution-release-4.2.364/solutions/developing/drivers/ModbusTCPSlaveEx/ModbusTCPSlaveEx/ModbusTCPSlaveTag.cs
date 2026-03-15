using System;
using DriverCodeBaseEx;
using DriverBaseInterfaces;

namespace ModbusTCPSlave
{
    public sealed class ModbusTCPSlaveTag : Tag
    {
        #region Constructors

        public ModbusTCPSlaveTag(TagDefinition tag, uint byteoffset, uint bitoffset)
            : base(tag, byteoffset, bitoffset)
        {

        }

        public ModbusTCPSlaveTag(TagDefinition tag)
            : base(tag)
        {

        }

        #endregion

        #region Override Properties/Functions

        public override bool BuildDynamicSettings(String dynamicSettings)
        {
            return ModbusTCPSlaveDynSettings.TryParse(dynamicSettings);
        }

        public override DynTagSettings DynSettings
        {
            get { return (DynTagSettings)ModbusTCPSlaveDynSettings; }
        }

        #endregion

        #region Properties

        readonly ModbusTCPSlaveDynTagSettings _ModbusTCPSlaveDynSettings = new ModbusTCPSlaveDynTagSettings();
        public ModbusTCPSlaveDynTagSettings ModbusTCPSlaveDynSettings
        {
            get { return _ModbusTCPSlaveDynSettings; }
        }

        #endregion        

    }
}
