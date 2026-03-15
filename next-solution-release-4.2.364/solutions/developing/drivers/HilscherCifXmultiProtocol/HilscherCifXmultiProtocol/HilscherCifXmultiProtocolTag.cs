using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DriverCodeBase;
using DriverBaseInterfaces;
using Opc.Ua;

namespace HilscherCifXmultiProtocol
{
    public sealed class HilscherCifXmultiProtocolTag : Tag
    {
        #region Constructors

        public HilscherCifXmultiProtocolTag(TagDefinition tag, uint byteoffset, uint bitoffset)
            : base(tag, byteoffset, bitoffset)
        {

        }

        public HilscherCifXmultiProtocolTag(TagDefinition tag)
            : base(tag)
        {
            
        }

        #endregion

        #region Override Properties/Functions

        public override bool BuildDynamicSettings(String dynamicSettings)
        {
            bool res = HilscherCifXmultiProtocolDynSettings.TryParse(dynamicSettings);
            //if (TagNode.DataType.IdType == IdType.Numeric && (uint)TagNode.DataType.Identifier == Opc.Ua.DataTypes.String)
            //    Size = (uint)TwinCATDynSettings.Length;

            return res;
        }

        public override DynTagSettings DynSettings
        {
            get { return (DynTagSettings)HilscherCifXmultiProtocolDynSettings; }
        }

        #endregion

        #region Properties

        private readonly HilscherCifXmultiProtocolDynTagSettings _HilscherCifXmultiProtocolDynSettings = new HilscherCifXmultiProtocolDynTagSettings();
        public HilscherCifXmultiProtocolDynTagSettings HilscherCifXmultiProtocolDynSettings
        {
            get { return _HilscherCifXmultiProtocolDynSettings; }
        }

        #endregion
    }
}
