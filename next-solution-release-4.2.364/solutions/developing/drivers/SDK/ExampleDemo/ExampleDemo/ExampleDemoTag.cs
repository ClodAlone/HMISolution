using System;
using DriverCodeBase;
using DriverBaseInterfaces;

namespace ExampleDemo
{
    internal sealed class ExampleDemoTag : Tag
    {
        #region Constructors

        public ExampleDemoTag(TagDefinition tag, uint byteoffset, uint bitoffset)
            : base(tag, byteoffset, bitoffset)
        {
        }

        public ExampleDemoTag(TagDefinition tag)
            : base(tag)
        {
        }

        #endregion

        #region Override Properties/Functions

        public override bool BuildDynamicSettings(String dynamicSettings)
        {
            return DemoDynSettings.TryParse(dynamicSettings);
        }

        public override DynTagSettings DynSettings
        {
            get { return (DynTagSettings)DemoDynSettings; }
        }

        #endregion

        #region Properties

        ExampleDemoDynTagSettings _DemoDynSettings;
        public ExampleDemoDynTagSettings DemoDynSettings
        {
            get 
            { 
                if (_DemoDynSettings == null)
                    _DemoDynSettings = new ExampleDemoDynTagSettings();

                return _DemoDynSettings; 
            }
        }
        #endregion        
    }
}
