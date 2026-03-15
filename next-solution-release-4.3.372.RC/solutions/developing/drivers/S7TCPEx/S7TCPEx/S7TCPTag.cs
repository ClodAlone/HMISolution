using System;
using System.Collections.Generic;
using System.Linq;
using DriverCodeBaseEx;
using DriverBaseInterfaces;
using Opc.Ua;
using UFUAModel.Extensions;
using System.Text;

namespace S7TCP
{
    public sealed class S7TCPTag : Tag
    {
        
        public struct ChangedBit
        {
            public int BitNr { get; set; }
            public byte BitValue { get; set; }

            public ChangedBit (int bitNr, byte bitValue)
            {
                BitNr = bitNr;
                BitValue = bitValue;
            }
        }
        
        #region Constructors

        public S7TCPTag(TagDefinition tag, uint byteoffset, uint bitoffset)
            : base(tag, byteoffset, bitoffset)
        {
            _IsMemberOfStruct = S7TCPDynSettings.IsMemberOfStruct;
        }

        public S7TCPTag(TagDefinition tag)
            : base(tag)
        {
            _IsMemberOfStruct = S7TCPDynSettings.IsMemberOfStruct;
        }

        #endregion

        #region Override Properties/Functions

        public override bool BuildDynamicSettings(String dynamicSettings)
        {
            bool res = S7TCPDynSettings.TryParse(dynamicSettings);
            if (TagNode.DataType.IdType == IdType.Numeric && (uint)TagNode.DataType.Identifier == Opc.Ua.DataTypes.String)
                Size = (uint)S7TCPDynSettings.Length;

            return res;
        }

        public override DynTagSettings DynSettings
        {
            get { return (DynTagSettings)S7TCPDynSettings; }
        }

        public override uint GetTagBuffer(ref byte[] buffer, bool read = true, int index = 0, uint elemsize = 0, uint buffertype = 0)
        {
            uint size = 0;            
            if (TagNode.DataType.IdType == IdType.Numeric)
            { 
                switch ((uint)TagNode.DataType.Identifier)
                {
                    case (uint)Opc.Ua.DataTypes.UInt64:
                    case (uint)Opc.Ua.DataTypes.Int64:
                    case (uint)Opc.Ua.DataTypes.Double:
                        size = base.GetTagBuffer(ref buffer, read, index, elemsize, buffertype);

                        for (int i = 0; i < ((buffer.Length)/8); i++)
                            Array.Reverse(buffer, i * 8, 8);
                        break;
                    default:
                        size = base.GetTagBuffer(ref buffer, read, index, elemsize, buffertype);
                        break;
                }
            }

            return size;

        }

        public List<ChangedBit> GetTagBufferChangedBits()
        {
            object curVal = Utils.Clone(WriteVal);

            List<ChangedBit> changesBits = new List<ChangedBit>();

            Array b = curVal as Array;
            if (b != null && b.GetLength(0) == TagNode.ArrayDimension)
            {                
                for (int i = 0; i < TagNode.ArrayDimension; i++)
                {
                    //write one bit at a time
                    if (Value.Value == null || (bool)(DataTypeExtensions.ChangeType(b.GetValue(i), UFUAModel.DataType.Boolean)) != (bool)((Value.Value as Array).GetValue(i)))
                        changesBits.Add(new ChangedBit(i, (bool)(b.GetValue(i)) ? (byte)1 : (byte)0));
                }
            }

            return changesBits;
        }

        /// <summary>   The read value. </summary>
        public override bool SetTagValue(ref byte[] buffer, int index, uint elemsize = 0, uint buffertype = 0, bool forceUpdate = false)
        {
            if (TagNode.DataType.IdType == IdType.Numeric)
            {                
                switch ((uint)TagNode.DataType.Identifier)
                {
                    case (uint)Opc.Ua.DataTypes.UInt64:
                    case (uint)Opc.Ua.DataTypes.Int64:
                    case (uint)Opc.Ua.DataTypes.Double:                        
                        for (int i = 0; i < (Size / 8); i++)
                            Array.Reverse(buffer, index + (i * 8), 8);
                        break;
                }

                return base.SetTagValue(ref buffer, index, forceUpdate, elemsize, buffertype);

            }
            return (false);
        }


        #endregion

        #region Methods        
        #endregion

        #region Properties

        private readonly S7TCPDynTagSettings _S7TCPDynSettings = new S7TCPDynTagSettings();
        public S7TCPDynTagSettings S7TCPDynSettings
        {
            get { return _S7TCPDynSettings; }
        }
        private bool _IsMemberOfStruct;
        public bool IsMemberOfStruct
        {
            get { return _IsMemberOfStruct; }
        }
        #endregion      
    }
}
