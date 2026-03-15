using System;
using DriverCodeBase;
using DriverBaseInterfaces;
using Opc.Ua;


namespace CoDeSys
{
    public sealed class CoDeSysTag : Tag
    {
        #region Constructors

        public CoDeSysTag(TagDefinition tag, uint byteoffset, uint bitoffset)
            : base(tag, byteoffset, bitoffset)
        {

        }

        public CoDeSysTag(TagDefinition tag)
            : base(tag)
        {

        }

        #endregion

        #region Override Properties/Functions

        public override bool BuildDynamicSettings(String dynamicSettings)
        {
            bool res = CoDeSysDynSettings.TryParse(dynamicSettings);
            if (TagNode.DataType.IdType == IdType.Numeric && (uint)TagNode.DataType.Identifier == Opc.Ua.DataTypes.String)
                Size = (uint)CoDeSysDynSettings.StringLength;

            return res;
        }

        public override DynTagSettings DynSettings
        {
            get { return (DynTagSettings)CoDeSysDynSettings; }
        }


        #endregion

        #region Properties

        readonly CoDeSysDynTagSettings _CoDeSysDynSettings = new CoDeSysDynTagSettings();
        public CoDeSysDynTagSettings CoDeSysDynSettings
        {
            get { return _CoDeSysDynSettings; }
        }

        #endregion

    }
}
