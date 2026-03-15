using System;
using System.Collections.Generic;
using System.Linq;
using DriverCodeBase;
using DriverBaseInterfaces;
using DriverCodeBase.Helpers;

namespace Demo
{
    internal sealed class DemoTag : Tag
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

        DemoDynTagSettings _DemoDynSettings;
        public DemoDynTagSettings DemoDynSettings
        {
            get 
            { 
                if (_DemoDynSettings == null)
                    _DemoDynSettings = new DemoDynTagSettings();

                return _DemoDynSettings; 
            }
        }
        #endregion        
    }
}
