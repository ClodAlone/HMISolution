using System;
using DriverCodeBaseEx;
using DriverBaseInterfaces;

namespace Databoom
{
    public sealed class DataboomTag : Tag
    {
        #region Constructors

        public DataboomTag(TagDefinition tag, uint byteoffset, uint bitoffset)
            : base(tag, byteoffset, bitoffset)
        {
        }

        public DataboomTag(TagDefinition tag)
            : base(tag)
        {
        }

        #endregion

        #region Override Properties/Functions
        
        public override bool BuildDynamicSettings(String dynamicSettings)
        {
            return DataboomDynSettings.TryParse(dynamicSettings);
        }

        public override DynTagSettings DynSettings
        {
            get { return (DynTagSettings)DataboomDynSettings; }
        }
        #endregion

        #region Properties
        readonly DataboomDynTagSettings _DataboomDynSettings = new DataboomDynTagSettings();
        public DataboomDynTagSettings DataboomDynSettings
        {
            get { return _DataboomDynSettings; }
        }
        #endregion
    }
}
