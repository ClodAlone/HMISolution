using System;
using System.Collections.Generic;
using System.Linq;
using DriverCodeBaseEx;
using DriverBaseInterfaces;

namespace ModBus
{
    public enum FunctionCodes
    {
        Coils,
        DiscreteInputs,
        MultipleRegisters,
        InputRegisters,
        SingleCoil,
        SingleRegister,
        FileRecord,
        ExceptionStatus,
    }

    public sealed class ModbusTag : Tag
    {
        #region Constructors

        public ModbusTag(TagDefinition tag, uint byteoffset, uint bitoffset)
            : base(tag, byteoffset, bitoffset)
        {

        }

        public ModbusTag(TagDefinition tag)
            : base(tag)
        {

        }

        #endregion

        #region Override Properties/Functions

        public override bool BuildDynamicSettings(String dynamicSettings)
        {
            return ModbusDynSettings.TryParse(dynamicSettings);
        }

        public override DynTagSettings DynSettings
        {
            get { return (DynTagSettings)ModbusDynSettings; }
        }

        #endregion
        
        #region Properties

        readonly ModbusDynTagSettings _ModbusDynSettings = new ModbusDynTagSettings();
        public ModbusDynTagSettings ModbusDynSettings
        {
            get { return _ModbusDynSettings; }
        }

        #endregion        

    }
}
