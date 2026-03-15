using System;
using DriverCodeBaseEx;
using DriverBaseInterfaces;

namespace Demo
{
    public sealed class DemoTag : Tag
    {
        #region Constructors

        public DemoTag(TagDefinition tag, uint byteoffset, uint bitoffset)
            : base(tag, byteoffset, bitoffset)
        {
        }

        public DemoTag(TagDefinition tag)
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
        readonly DemoDynTagSettings _DemoDynSettings = new DemoDynTagSettings();
        public DemoDynTagSettings DemoDynSettings
        {
            get { return _DemoDynSettings; }
        }
        #endregion
    }
}
