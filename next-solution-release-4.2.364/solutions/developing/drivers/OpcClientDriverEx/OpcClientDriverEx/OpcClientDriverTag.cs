using System;
using System.Collections.Generic;
using System.Linq;
using DriverCodeBaseEx;
using DriverBaseInterfaces;

namespace OpcClientDriver
{


    public sealed class OpcClientDriverTag : Tag
    {
        #region Constructors

        public OpcClientDriverTag(TagDefinition tag, uint byteoffset, uint bitoffset)
            : base(tag, byteoffset, bitoffset)
        {

        }

        public OpcClientDriverTag(TagDefinition tag)
            : base(tag)
        {

        }

        #endregion

        #region Override Properties/Functions

        public override bool BuildDynamicSettings(String dynamicSettings)
        {
            return OpcClientDynSettings.TryParse(dynamicSettings);
        }

        public override DynTagSettings DynSettings
        {
            get { return (DynTagSettings)OpcClientDynSettings; }
        }

        #endregion

        #region Properties

        private readonly OpcClientDriverDynTagSettings _OpcClientDynSettings = new OpcClientDriverDynTagSettings();
        public OpcClientDriverDynTagSettings OpcClientDynSettings
        {
            get { return _OpcClientDynSettings; }
        }

        #endregion

    }
}
