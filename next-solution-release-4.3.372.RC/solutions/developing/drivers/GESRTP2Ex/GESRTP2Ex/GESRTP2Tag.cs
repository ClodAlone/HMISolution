////////////////////////////////////////////////////////////////////////////////////////////////////
// file:	GESRTP2Tag.cs
//
// summary:	Implements the driver GESRTP2 tag class
////////////////////////////////////////////////////////////////////////////////////////////////////

using System;
using System.Collections.Generic;
using System.Linq;
using DriverCodeBaseEx;
using DriverBaseInterfaces;
using DriverCodeBaseEx.Helpers;
using Opc.Ua;
using UFUAModel.Extensions;
using System.Text;

namespace GESRTP2
{

    /// <summary>   Variable of the GESRTP2 driver. </summary>
    public sealed class GESRTP2Tag : Tag
    {
        #region Constructors

        ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>   Constructor. </summary>
        ///
        /// <param name="tag" type="TagDefinition"> The tag. </param>
        /// <param name="byteoffset" type="uint">   The byteoffset. </param>
        /// <param name="bitoffset" type="uint">    The bitoffset. </param>
        ////////////////////////////////////////////////////////////////////////////////////////////////////
        public GESRTP2Tag(TagDefinition tag, uint byteoffset, uint bitoffset)
            : base(tag, byteoffset, bitoffset)
        {

        }

        ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>   Constructor. </summary>
        ///
        /// <param name="tag" type="TagDefinition"> The tag. </param>
        ////////////////////////////////////////////////////////////////////////////////////////////////////
        public GESRTP2Tag(TagDefinition tag)
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
            bool res = GESRTP2DynSettings.TryParse(dynamicSettings);
            if (TagNode.DataType.IdType == IdType.Numeric && (uint)TagNode.DataType.Identifier == Opc.Ua.DataTypes.String)
            {
                if (GESRTP2DynSettings.GetProtocolDataByteSize() == 1)
                    Size = (uint)GESRTP2DynSettings.StringLength;
                else
                    Size = (uint)((GESRTP2DynSettings.StringLength + 1) / 2) * 2;
            }

            return res;
        }

        ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>   Return dynamic settings. </summary>
        ///
        /// <value> The dynamic settings. </value>
        ////////////////////////////////////////////////////////////////////////////////////////////////////
        public override DynTagSettings DynSettings
        {
            get { return (DynTagSettings)GESRTP2DynSettings; }
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
                object curVal = WriteVal;
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
                    case (uint)Opc.Ua.DataTypes.String:
                        if (GESRTP2Protocol.IsSymbolic(this))
                        {
                            size = (TagNode.ArrayDimension == 0 ? 1 : TagNode.ArrayDimension);
                            if (buffer != null && buffer.Length >= size)
                            {
                                System.Text.UTF8Encoding enc = new UTF8Encoding();
                                if (TagNode.ArrayDimension == 0)
                                {
                                    string sVal = curVal as string;
                                    int count = enc.GetByteCount(sVal);
                                    if (buffer != null && buffer.Length > 0 && sVal != null && count > 0)
                                    {
                                        int min = System.Math.Min(count, buffer.Length);
                                        Array.Copy(enc.GetBytes(sVal), 0, buffer, index, min);
                                        size = (uint)min;
                                    }
                                }
                                else
                                {
                                    Array b = curVal as Array;
                                    if (b != null && b.GetLength(0) == TagNode.ArrayDimension)
                                    {
                                        int elemlen = (int)(Size / TagNode.ArrayDimension);
                                        for (int i = 0; i < TagNode.ArrayDimension; i++)
                                        {
                                            int count = (b.GetValue(i) == null ? 0 : enc.GetByteCount((string)b.GetValue(i)));
                                            if (buffer != null && buffer.Length > 0 && count > 0)
                                            {
                                                int min = System.Math.Min(count, elemlen);
                                                Array.Copy(enc.GetBytes((string)b.GetValue(i)), 0, buffer, index + (i * elemlen), min);
                                                //if (min < elemlen)
                                                //{
                                                //    for (int k = 0; k < (elemlen - min); k++)
                                                //        Array.Copy(enc.GetBytes((string)" "), 0, buffer, index + (i * elemlen) + min + k, 1);
                                                //}
                                                //size += (uint)elemlen;
                                            }
                                        }
                                        size = (uint)buffer.Length;
                                    }
                                }
                            }
                            return size;
                        }
                        else
                        {
                            break;
                        }
                }
            }
            return base.GetTagBuffer(ref buffer, read, index, elemsize , buffertype);
        }

        #endregion

        #region Methods
        /// <summary>   The read value. </summary>
        public override bool SetTagValue(ref byte[] buffer, int index, uint elemsize = 0, uint buffertype = 0, bool forceUpdate = false)
        {
            if (TagNode.DataType.IdType == IdType.Numeric)
            {
                bool bCopy = false;
                object val = new object();
                uint nType = (uint)TagNode.DataType.Identifier;
                object readValue = (forceUpdate ? null : GetValue());
                switch (nType)
                {
                    case (uint)Opc.Ua.DataTypes.String:
                        {
                            byte[] tbuf;
                            uint iCheckCharacter = 0;
                            if (TagNode.ArrayDimension == 0)
                            {
                                for (iCheckCharacter = 0; iCheckCharacter < Size; iCheckCharacter++)
                                {
                                    if (buffer[(iCheckCharacter + index)] == 0x00)
                                    {
                                        break;
                                    }
                                }
                                tbuf = new byte[iCheckCharacter];
                                Array.Copy(buffer, index , tbuf, 0, iCheckCharacter);
                                return base.SetTagValue(ref tbuf, 0, forceUpdate, elemsize, buffertype);
                            }
                            else
                            {
                                System.Text.UTF8Encoding enc = new System.Text.UTF8Encoding();
                                uint nOffset = 0;
                                string[] a = new string[TagNode.ArrayDimension];
                                Array b = readValue as Array;
                                bool check = (b != null && b.GetLength(0) == TagNode.ArrayDimension);
                                bCopy = (!check);
                                int elemlen = (int)(Size / TagNode.ArrayDimension);

                                for (int i = 0; i < TagNode.ArrayDimension; i++)
                                {
                                    nOffset = (uint)(i * elemlen);
                                    for (iCheckCharacter = 0; iCheckCharacter < elemlen; iCheckCharacter++)
                                    {
                                        if (buffer[(nOffset + iCheckCharacter + index)] == 0x00)
                                        {
                                            break;
                                        }
                                    }
                                    tbuf = new byte[iCheckCharacter];
                                    Array.Copy(buffer, index + nOffset, tbuf, 0, iCheckCharacter);
                                    a[i] = enc.GetString(tbuf);
                                    if (check && a[i] != (string)b.GetValue(i))
                                        bCopy = true;
                                }
                                val = a;
                            }
                            if (readValue == null || bCopy)
                            {
                                SetValue(val);
                                return true;
                            }
                        }
                        break;
                    default:
                        return base.SetTagValue(ref buffer, index, forceUpdate, elemsize, buffertype);
                }

            }
            return (false);
        }

        public void BackUpOffset()
        {
            _BackUpByteOffset = ByteOffset;
            _BackUpBitOffset = BitOffset;
        }

        public void RestoreOffset()
        {
            ByteOffset = _BackUpByteOffset;
            BitOffset = _BackUpBitOffset;
        }
        #endregion

        #region Properties

        private uint _BackUpByteOffset;
        private uint _BackUpBitOffset;

        /// <summary>   The driver GESRTP2 dynamic settings. </summary>
        readonly GESRTP2DynTagSettings _GESRTP2DynSettings = new GESRTP2DynTagSettings();
        ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>   Dynamic tag settings property. </summary>
        ///
        /// <value> The driver GESRTP2 dynamic settings. </value>
        ////////////////////////////////////////////////////////////////////////////////////////////////////
        public GESRTP2DynTagSettings GESRTP2DynSettings
        {
            get { return _GESRTP2DynSettings; }
        }

        public int _StructMixedBufferByteOffset;
        public int StructMixedBufferByteOffset
        {
            get { return _StructMixedBufferByteOffset; }
            set { _StructMixedBufferByteOffset = value; }
        }

        public int _StructMixedBufferByteSize;
        public int StructMixedBufferByteSize
        {
            get { return _StructMixedBufferByteSize; }
            set { _StructMixedBufferByteSize = value; }
        }
        #endregion
    }
}
