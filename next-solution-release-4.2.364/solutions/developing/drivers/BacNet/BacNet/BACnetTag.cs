////////////////////////////////////////////////////////////////////////////////////////////////////
// file:	BACnetTag.cs
//
// summary:	Implements the driver BACnet tag class
////////////////////////////////////////////////////////////////////////////////////////////////////

using System;
using System.Collections.Generic;
using System.Linq;
using DriverCodeBase;
using DriverBaseInterfaces;
using DriverCodeBase.Helpers;
using Opc.Ua;
using UFUAModel.Extensions;

namespace BACnet
{

    /// <summary>   Variable of the BACnet driver. </summary>
    public sealed class BACnetTag : Tag
    {
        #region Constructors

        ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>   Constructor. </summary>
        ///
        /// <param name="tag" type="TagDefinition"> The tag. </param>
        /// <param name="byteoffset" type="uint">   The byteoffset. </param>
        /// <param name="bitoffset" type="uint">    The bitoffset. </param>
        ////////////////////////////////////////////////////////////////////////////////////////////////////
        public BACnetTag(TagDefinition tag, uint byteoffset, uint bitoffset)
            : base(tag, byteoffset, bitoffset)
        {

        }

        ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>   Constructor. </summary>
        ///
        /// <param name="tag" type="TagDefinition"> The tag. </param>
        ////////////////////////////////////////////////////////////////////////////////////////////////////
        public BACnetTag(TagDefinition tag)
            : base(tag)
        {

        }

        #endregion

        #region Override Properties/Functions

        ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>   Verify if string "dynamicSettings" is correct to initialize a tag. </summary>
        ///
        /// <param name="dynamicSettings">  . </param>
        ///
        /// <returns>   true if it succeeds, false if it fails. </returns>
        ////////////////////////////////////////////////////////////////////////////////////////////////////
        public override bool BuildDynamicSettings(String dynamicSettings)
        {
            bool res = BACnetDynSettings.TryParse(dynamicSettings);
            if (TagNode.DataType.IdType == IdType.Numeric && (uint)TagNode.DataType.Identifier == Opc.Ua.DataTypes.String)
                Size = (uint)BACnetDynSettings.DataSize;

            return res;
        }

        ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>   Return dynamic settings. </summary>
        ///
        /// <value> The dynamic settings. </value>
        ////////////////////////////////////////////////////////////////////////////////////////////////////
        public override DynTagSettings DynSettings
        {
            get { return (DynTagSettings)BACnetDynSettings; }
        }

        ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>   Gets tag buffer. </summary>
        ///
        /// <param name="buffer" type="ref byte []">    [in,out] The buffer. </param>
        /// <param name="read" type="bool">             (Optional) true if the data was read. </param>
        /// <param name="index" type="int">             (Optional) zero-based index of the. </param>
        ///
        /// <returns>   The tag buffer. </returns>
        ////////////////////////////////////////////////////////////////////////////////////////////////////
        public override uint GetTagBuffer(ref byte[] buffer, bool read = true, int index = 0, uint elemsize = 0, uint buffertype = 0)
        {
            if (TagNode.DataType.IdType == IdType.Numeric)
            {
                object curVal = Value.Value;
                uint size;
                uint nType = (uint)TagNode.DataType.Identifier;
                switch (nType)
                {
                    case (uint)Opc.Ua.DataTypes.Boolean:
                        int bitOffset = index % 8;
                        byte byteOffset = (byte)((index / 8) * 8);
                        size = (uint)(TagNode.ArrayDimension == 0 ? 1 : TagNode.ArrayDimension / 8 + (TagNode.ArrayDimension % 8 > 0 ? 1 : 0));
                        if (buffer != null && buffer.Length >= size)
                        {
                            if (TagNode.ArrayDimension == 0)
                            {
                                BitConverter.GetBytes((bool)DataTypeExtensions.ChangeType(curVal, UFUAModel.DataType.Boolean)).CopyTo(buffer, byteOffset);
                                buffer[0] <<= bitOffset;
                            }
                            else
                            {
                                Array b = curVal as Array;
                                if (b != null && b.GetLength(0) == TagNode.ArrayDimension)
                                {
                                    for (int i = 0; i < TagNode.ArrayDimension ; i++)
                                    {
                                        int bitIndex = i + bitOffset;
                                        if ((bool)DataTypeExtensions.ChangeType(b.GetValue(i), UFUAModel.DataType.Boolean))
                                        {
                                            buffer[byteOffset + bitIndex / 8] += (byte)(1 << (bitIndex % 8));
                                        }
                                    }
                                }
                            }
                        }
                        return size;
                }

            }
            return base.GetTagBuffer(ref buffer, read, index, elemsize , buffertype);
        }

        #endregion

        #region Properties

        /// <summary>   The driver BACnet dynamic settings. </summary>
        readonly BACnetDynTagSettings _BACnetDynSettings = new BACnetDynTagSettings();
        ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>   Dynamic tag settings property. </summary>
        ///
        /// <value> The driver BACnet dynamic settings. </value>
        ////////////////////////////////////////////////////////////////////////////////////////////////////
        public BACnetDynTagSettings BACnetDynSettings
        {
            get { return _BACnetDynSettings; }
        }

        /// <summary>  First time ProcessJobValues cicle </summary>
        public bool _ProcessJobValuesFirstTime = true;
        ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>   Dynamic tag settings property. </summary>
        ///
        /// <value> The driver BACnet dynamic settings. </value>
        ////////////////////////////////////////////////////////////////////////////////////////////////////
        public bool ProcessJobValuesFirstTime
        {
            get { return _ProcessJobValuesFirstTime; }
            set { _ProcessJobValuesFirstTime = value; }
        }
        #endregion        
    }
}
