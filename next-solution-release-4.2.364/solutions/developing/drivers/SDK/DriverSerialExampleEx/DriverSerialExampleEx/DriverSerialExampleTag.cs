using System;
using System.Collections.Generic;
using System.Linq;
using DriverCodeBaseEx;
using DriverBaseInterfaces;

namespace DriverSerialExample
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

    public sealed class DriverSerialExampleTag : Tag
    {
        #region Constructors

        public DriverSerialExampleTag(TagDefinition tag, uint byteoffset, uint bitoffset)
            : base(tag, byteoffset, bitoffset)
        {

        }

        public DriverSerialExampleTag(TagDefinition tag)
            : base(tag)
        {

        }

        #endregion

        #region Override Properties/Functions

        public override bool BuildDynamicSettings(String dynamicSettings)
        {
            return DriverSerialExampleDynSettings.TryParse(dynamicSettings);
        }

        public override DynTagSettings DynSettings
        {
            get { return (DynTagSettings)DriverSerialExampleDynSettings; }
        }

        #endregion
        
        #region Properties

        readonly DriverSerialExampleDynTagSettings _DriverSerialExampleDynSettings = new DriverSerialExampleDynTagSettings();
        public DriverSerialExampleDynTagSettings DriverSerialExampleDynSettings
        {
            get { return _DriverSerialExampleDynSettings; }
        }

        #endregion        

    }
}
