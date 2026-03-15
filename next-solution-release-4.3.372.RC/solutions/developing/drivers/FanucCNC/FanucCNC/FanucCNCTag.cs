using System;
using DriverCodeBase;
using DriverBaseInterfaces;
using Opc.Ua;


namespace FanucCNC
{
    public sealed class FanucCNCTag : Tag
    {
        #region Constructors

        public FanucCNCTag(TagDefinition tag, uint byteoffset, uint bitoffset)
            : base(tag, byteoffset, bitoffset)
        {
            _RequestSize = 0;
        }

        public FanucCNCTag(TagDefinition tag)
            : base(tag)
        {
            _RequestSize = 0;
        }

        #endregion

        #region Override Properties/Functions

        public override bool BuildDynamicSettings(String dynamicSettings)
        {
            bool res = FanucCNCDynSettings.TryParse(dynamicSettings);
            if (TagNode.DataType.IdType == IdType.Numeric && (uint)TagNode.DataType.Identifier == Opc.Ua.DataTypes.String)
            {
                if (_FanucCNCDynSettings.FunctionSettings != null)
                    Size = _FanucCNCDynSettings.FunctionSettings.StringLength;
            }

            return res;
        }

        public override DynTagSettings DynSettings
        {
            get { return (DynTagSettings)FanucCNCDynSettings; }
        }
        #endregion

        #region Properties

        readonly FanucCNCDynTagSettings _FanucCNCDynSettings = new FanucCNCDynTagSettings();
        public FanucCNCDynTagSettings FanucCNCDynSettings
        {
            get { return _FanucCNCDynSettings; }
        }

        private int _RequestSize;        
        public int RequestSize
        {
            get { return _RequestSize; }

            set { _RequestSize = value; }
        }

        #endregion

    }
}
