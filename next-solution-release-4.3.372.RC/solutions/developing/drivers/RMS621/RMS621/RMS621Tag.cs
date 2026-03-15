using System;
using DriverCodeBase;
using DriverBaseInterfaces;
using Opc.Ua;

namespace RMS621
{
    public sealed class RMS621Tag : Tag
    {
        #region Constructors

        public RMS621Tag(TagDefinition tag, uint byteoffset, uint bitoffset)
            : base(tag, byteoffset, bitoffset)
        {

        }

        public RMS621Tag(TagDefinition tag)
            : base(tag)
        {

        }

        #endregion

        #region Override Properties/Functions
        public override bool BuildDynamicSettings(String dynamicSettings)
        {
            return RMS621DynSettings.TryParse(dynamicSettings);
        }

        public override DynTagSettings DynSettings
        {
            get { return (DynTagSettings)RMS621DynSettings; }
        }
        #endregion

        #region Properties

        readonly RMS621DynTagSettings _RMS621DynSettings = new RMS621DynTagSettings();
        public RMS621DynTagSettings RMS621DynSettings
        {
            get { return _RMS621DynSettings; }
        }

        #endregion        
    }
}
