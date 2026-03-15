using System;
using DriverCodeBaseEx;
using DriverBaseInterfaces;

namespace MTConnect
{
    public sealed class MTConnectTag : Tag
    {
        #region Constructors

        public MTConnectTag(TagDefinition tag, uint byteoffset, uint bitoffset)
            : base(tag, byteoffset, bitoffset)
        {
        }

        public MTConnectTag(TagDefinition tag)
            : base(tag)
        {
        }

        #endregion
        //public override bool SetTagValue(ref byte[] buffer, int index, bool forceUpdate, uint elemsize = 0, uint buffertype = 0)
        //{
        //    return base.SetTagValue(ref buffer, index, forceUpdate, elemsize, buffertype);
        //}

        #region Override Properties/Functions

        public override bool BuildDynamicSettings(String dynamicSettings)
        {
            return MTConnectDynSettings.TryParse(dynamicSettings);
        }

        public override DynTagSettings DynSettings
        {
            get { return (DynTagSettings)MTConnectDynSettings; }
        }
        #endregion

        #region Properties
        readonly MTConnectDynTagSettings _MTConnectDynSettings = new MTConnectDynTagSettings();
        public MTConnectDynTagSettings MTConnectDynSettings
        {
            get { return _MTConnectDynSettings; }
        }
        #endregion
    }
}
