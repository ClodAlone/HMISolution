////////////////////////////////////////////////////////////////////////////////////////////////////
// file:	Tag.cs
//
// summary:	Implements the tag class
////////////////////////////////////////////////////////////////////////////////////////////////////

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Opc.Ua;
using DriverBaseInterfaces;
using DriverCodeBaseEx.Helpers;
using DriverCodeBaseEx.Enumerators;
using UFUAModel.Extensions;
using System.Text.RegularExpressions;

namespace DriverCodeBaseEx
{
    /// <summary>   variable of the base communication driver. </summary>
    public abstract class Tag
    {
        #region Constructors

        ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>   Specialised constructor for use only by derived class. </summary>
        ///
        /// <param name="tag" type="TagDefinition"> The tag. </param>
        /// <param name="byteoffset">               The byte offset. </param>
        /// <param name="bitoffset">                The bit offset. </param>
        ////////////////////////////////////////////////////////////////////////////////////////////////////
        protected Tag(TagDefinition tag, uint byteoffset, uint bitoffset)
            : this(tag)
        {
            _ByteOffset = byteoffset;
            _BitOffset = bitoffset;
        }

        ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>   Specialised constructor for use only by derived class. </summary>
        ///
        /// <param name="tag" type="TagDefinition"> The tag. </param>
        ////////////////////////////////////////////////////////////////////////////////////////////////////
        protected Tag(TagDefinition tag)
        {
            TagNode = tag;
            bIsValid = BuildDynamicSettings(TagNode.DynamicSettings);
            if (!bIsValid)
                InvalidReason = Properties.Resources.ErrorDynamicSettingsInvalidFormat;
            if (tag.DataType.IdType == IdType.Numeric)
            {
                 uint nType = (uint)tag.DataType.Identifier;
                 uint arraysize = tag.ArrayDimension;

                switch(nType)
                {
                    case (uint)Opc.Ua.DataTypes.Boolean:
                        _Size = 1;
                        break;
                    case (uint)Opc.Ua.DataTypes.Byte:
                        _Size = 1;
                        break;
                    case (uint)Opc.Ua.DataTypes.Double:
                        _Size = 8;
                        break;
                    case (uint)Opc.Ua.DataTypes.Float:
                        _Size = 4;
                        break;
                    case (uint)Opc.Ua.DataTypes.Int16:
                        _Size = 2;
                        break;
                    case (uint)Opc.Ua.DataTypes.Int32:
                        _Size = 4;
                        break;
                    case (uint)Opc.Ua.DataTypes.Int64:
                        _Size = 8;
                        break;
                    case (uint)Opc.Ua.DataTypes.SByte:
                        _Size = 1;
                        break;
                    case (uint)Opc.Ua.DataTypes.UInt16:
                        _Size = 2;
                        break;
                    case (uint)Opc.Ua.DataTypes.UInt32:
                        _Size = 4;
                        break;
                    case (uint)Opc.Ua.DataTypes.UInt64:
                        _Size = 8;
                        break;
                }
                if(arraysize > 0)
                    _Size *= arraysize;
            }

            SetInitialValue(tag.InitialValue);            
        }
        
        #endregion

        #region Data Members

        /// <summary>   The tag node. </summary>
        public readonly TagDefinition TagNode;
        /// <summary>   true if this object is valid. </summary>
        public bool bIsValid;
        public string InvalidReason;

        /// <summary>   The write value. </summary>
        public object WriteVal;

        /// <summary>   The buffer of protocol read . </summary>
        public byte[] MemRW;

        #endregion

        #region Abstract Properties/Functions

        ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>   Builds dynamic settings. </summary>
        ///
        /// <param name="dynamicSettings" type="String">    The dynamic settings. </param>
        ///
        /// <returns>   true if it succeeds, false if it fails. </returns>
        ////////////////////////////////////////////////////////////////////////////////////////////////////
        public abstract bool BuildDynamicSettings(String dynamicSettings);
        ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>   Gets the dynamic settings. </summary>
        ///
        /// <value> The dynamic settings. </value>
        ////////////////////////////////////////////////////////////////////////////////////////////////////
        public abstract DynTagSettings DynSettings { get; }

        #endregion
        
        #region Internal Methods

        public bool setMemRW(byte[] buffer, int index , int Length)
        {
            if(MemRW == null)
                MemRW = new byte[Length];
            if (Length > MemRW.Length)
                return false;
            Array.Copy(buffer, index, MemRW, 0, Length);
            return true;
        }
        public bool getWriteValueFromMemRW(ref byte[] buffer, UInt16 DataByteSize, uint ProtocolDataByteSize, int ElementNumber, int ArrayIndex)
        {
            byte[] jobdata = new byte[ProtocolDataByteSize];
            byte[] tmpMemRw = new byte[ProtocolDataByteSize];
            int memRwArrayOffset = (int)(ArrayIndex * ProtocolDataByteSize);
            if (MemRW != null)
                if (MemRW.Length >= ProtocolDataByteSize + memRwArrayOffset)
                    for (int i = 0; i < ProtocolDataByteSize; i++)
                        tmpMemRw[i] = MemRW[i + memRwArrayOffset];

            if (TagNode.DataType == Opc.Ua.DataTypes.Boolean)
            {
                for (int i = 0; i < ProtocolDataByteSize; i++)
                {
                    if (ElementNumber / 8 == i)
                    {
                        byte mask = (byte)(1 << (ElementNumber % 8));
                        jobdata[i] = (byte)((tmpMemRw[i] & (mask ^ 0xff)) | (byte)(buffer[0] != 0 ? mask : 0));
                    }
                    else
                        jobdata[i] = tmpMemRw[i];
                }
            }
            else
            {
                int ByteNumber = (int)(ElementNumber * GetDataTypeByteSize((uint)TagNode.DataType.Identifier));
                int ii = 0;
                for (int i = 0; i < ProtocolDataByteSize; i++)
                    jobdata[i] = (i < ByteNumber || i >= ByteNumber + DataByteSize) ? tmpMemRw[i] : buffer[ii++];
            }
            buffer = jobdata;
            return true;
        }
        public bool getBoolValueFromMemRW(int indexData, int ElementNumber)
        {
            return (MemRW[ElementNumber / 8 + indexData] & (1 << (ElementNumber % 8))) != 0;
        }
        public uint GetDataTypeByteSize(uint DataType)
        {
            return (GetDataTypeBitSize(DataType) + 7) / 8;
        }
        public static uint GetDataTypeBitSize(uint DataType)
        {
            switch (DataType)
            {
                case (uint)BuiltInType.Boolean:
                    return 1;
                case (uint)BuiltInType.SByte:
                case (uint)BuiltInType.Byte:
                    return 8;
                case (uint)BuiltInType.Int16:
                case (uint)BuiltInType.UInt16:
                    return 16;
                case (uint)BuiltInType.Float:
                case (uint)BuiltInType.UInt32:
                case (uint)BuiltInType.Int32:
                    return 32;
                case (uint)BuiltInType.UInt64:
                case (uint)BuiltInType.Int64:
                case (uint)BuiltInType.Double:
                    return 64;
                default:
                    return 8;
            }
        }

        ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>   Sets tag value. </summary>
        ///
        /// <param name="buffer" type="ref byte[]"> [in,out] The buffer. </param>
        /// <param name="index" type="int">         zero-based index of the. </param>
        /// <param name="elemsize" type="uint">     (Optional) size of the protocol element. </param>
        /// <param name="buffertype" type="uint">     (Optional) DataTypes of the destination buffer. </param>
        /// <param name="forceUpdate" type="bool">     (Optional) force value update. </param>
        /// 
        /// <returns>   true if it succeeds, false if it fails. </returns>
        ////////////////////////////////////////////////////////////////////////////////////////////////////
        public virtual bool SetTagValue(ref byte[] buffer, int index, uint elemsize = 0, uint buffertype = 0, bool forceUpdate = false)
        {
            return (SetTagValue(ref buffer, index, forceUpdate, elemsize, buffertype));
        }

        ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>   Sets tag value. </summary>
        ///
        /// <param name="buffer" type="ref byte[]"> [in,out] The buffer. </param>
        /// <param name="index" type="int">         zero-based index of the. </param>
        /// <param name="elemsize" type="uint">     (Optional) size of the protocol element. </param>
        /// <param name="buffertype" type="uint">     (Optional) DataTypes of the destination buffer. </param>
        /// <param name="forceUpdate" type="bool">     (Optional) force value update. </param>
        /// 
        /// <returns>   true if it succeeds, false if it fails. </returns>
        ////////////////////////////////////////////////////////////////////////////////////////////////////
        public virtual bool SetTagValue(ref byte[] buffer, int index, bool forceUpdate, uint elemsize = 0, uint buffertype = 0)
        {
            if (TagNode.DataType.IdType == IdType.Numeric)
            {
                if (!IsQualityGood())
                    forceUpdate = true;

                bool bCopy = false;
                object val = new object();
                uint size = 0;
                uint nType = (uint)TagNode.DataType.Identifier;
                object readValue = (forceUpdate ? null : Value.Value);
                switch (nType)
                {
                    case (uint)Opc.Ua.DataTypes.String:
                        {
                            System.Text.UTF8Encoding enc = new UTF8Encoding();
                            if (TagNode.ArrayDimension == 0)
                            {
                                if (Size > buffer.Length)
                                    size = (uint)buffer.Length;
                                else
                                    size = Size;
                                byte[] tbuf = new byte[size];
                                Array.Copy(buffer, index, tbuf, 0, size);
                                val = enc.GetString(tbuf);
                                bCopy = (readValue == null || Convert.ToString(readValue) != (string)val);
                            }
                            else
                            {
                                string[] a = new string[TagNode.ArrayDimension];
                                Array b = readValue as Array;
                                bool check = (b != null && b.GetLength(0) == TagNode.ArrayDimension);
                                bCopy = (!check);
                                int elemlen = (int)(Size / TagNode.ArrayDimension);
                                byte[] tbuf = new byte[elemlen];
                                for (int i = 0; i < TagNode.ArrayDimension; i++)
                                {
                                    Array.Copy(buffer, index + (i * elemlen), tbuf, 0, elemlen);
                                    a[i] = enc.GetString(tbuf);
                                    if (check && a[i] != Convert.ToString(b.GetValue(i)))
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
                    case (uint)Opc.Ua.DataTypes.Boolean:
                        size = (TagNode.ArrayDimension == 0 ? 1 : TagNode.ArrayDimension);
                        if (buffer != null && buffer.Length >= (index + size))
                        {
                            if (TagNode.ArrayDimension == 0)
                            {
                                val = BitConverter.ToBoolean(buffer, index);
                                bCopy = (readValue == null || Convert.ToBoolean(readValue) != (bool)val);
                            }
                            else
                            {
                                bool[] a = new bool[TagNode.ArrayDimension];
                                Array b = readValue as Array;
                                bool check = (b != null && b.GetLength(0) == TagNode.ArrayDimension);
                                bCopy = (!check);
                                for (int i = 0; i < TagNode.ArrayDimension; i++)
                                {
                                    a[i] = BitConverter.ToBoolean(buffer, index + i);
                                    if (check && a[i] != Convert.ToBoolean(b.GetValue(i)))
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
                    case (uint)Opc.Ua.DataTypes.SByte:
                        size = (TagNode.ArrayDimension == 0 ? 1 : TagNode.ArrayDimension);
                        if (buffer != null && buffer.Length >= (index + size))
                        {
                            if (TagNode.ArrayDimension == 0)
                            {
                                val = (sbyte)buffer[index];
                                bCopy = (readValue == null || Convert.ToSByte(readValue) != (sbyte)val);
                            }
                            else
                            {
                                sbyte[] a = new sbyte[TagNode.ArrayDimension];
                                Array b = readValue as Array;
                                bool check = (b != null && b.GetLength(0) == TagNode.ArrayDimension);
                                bCopy = (!check);
                                for (int i = 0; i < TagNode.ArrayDimension; i++)
                                {
                                    a[i] = (sbyte)buffer[index + i];
                                    if (check && a[i] != Convert.ToSByte(b.GetValue(i)))
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
                    case (uint)Opc.Ua.DataTypes.Byte:
                        size = (TagNode.ArrayDimension == 0 ? 1 : TagNode.ArrayDimension);
                        if (buffer != null && buffer.Length >= (index + size))
                        {
                            if (TagNode.ArrayDimension == 0)
                            {
                                val = (byte)buffer[index];
                                bCopy = (readValue == null || Convert.ToByte(readValue) != (byte)val);
                            }
                            else
                            {
                                byte[] a = new byte[TagNode.ArrayDimension];
                                Array b = readValue as Array;
                                bool check = (b != null && b.GetLength(0) == TagNode.ArrayDimension);
                                bCopy = (!check);
                                for (int i = 0; i < TagNode.ArrayDimension; i++)
                                {
                                    a[i] = (byte)buffer[index + i];
                                    if (check && a[i] != Convert.ToByte(b.GetValue(i)))
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
                    case (uint)Opc.Ua.DataTypes.UInt16:
                        {
                            size = (TagNode.ArrayDimension == 0 ? 2 : TagNode.ArrayDimension * 2);
                            byte[] nbuf = null;
                            if (elemsize == 0)
                            {
                                nbuf = new byte[buffer.Length];
                                buffer.CopyTo(nbuf, 0);
                            }
                            else
                            {
                                uint newsize = (TagNode.ArrayDimension == 0 ? elemsize : TagNode.ArrayDimension * elemsize);
                                nbuf = new byte[size];
                                if (buffer == null || buffer.Length < (index + newsize))
                                    break;
                                if (TagNode.ArrayDimension == 0)
                                {
                                    Array.Copy(buffer, index, nbuf, 0, elemsize);
                                    index = 0;
                                }
                                else
                                {
                                    for (int i = 0; i < TagNode.ArrayDimension; i++)
                                    {
                                        Array.Copy(buffer, index + (elemsize * i), nbuf, i * 2, elemsize);
                                    }

                                    index = 0;
                                }

                            }
                            if (nbuf != null && nbuf.Length >= (index + size))
                            {
                                if (TagNode.ArrayDimension == 0)
                                {
                                    val = BitConverter.ToUInt16(nbuf, index);
                                    bCopy = (readValue == null || Convert.ToUInt16(readValue) != (UInt16)val);                                    
                                }
                                else
                                {
                                    UInt16[] a = new UInt16[TagNode.ArrayDimension];
                                    Array b = readValue as Array;
                                    bool check = (b != null && b.GetLength(0) == TagNode.ArrayDimension);
                                    bCopy = (!check);
                                    for (int i = 0; i < TagNode.ArrayDimension; i++)
                                    {
                                        a[i] = BitConverter.ToUInt16(nbuf, index + 2 * i);
                                        if (check && a[i] != Convert.ToUInt16(b.GetValue(i)))
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
                        }
                        break;
                    case (uint)Opc.Ua.DataTypes.Double:
                        {
                            size = (TagNode.ArrayDimension == 0 ? 8 : TagNode.ArrayDimension * 8);
                            byte[] nbuf = null;
                            if (elemsize == 0)
                            {
                                nbuf = new byte[buffer.Length];
                                buffer.CopyTo(nbuf, 0);
                            }
                            else
                            {
                                uint newsize = (TagNode.ArrayDimension == 0 ? elemsize : TagNode.ArrayDimension * elemsize);
                                nbuf = new byte[size];
                                if (buffer == null || buffer.Length < (index + newsize))
                                    break;
                                if (TagNode.ArrayDimension == 0)
                                {
                                    Array.Copy(buffer, index, nbuf, 0, elemsize);
                                    index = 0;
                                }
                                else
                                {
                                    for (int i = 0; i < TagNode.ArrayDimension; i++)
                                    {
                                        Array.Copy(buffer, index + (elemsize * i), nbuf, i * 8, elemsize);
                                    }

                                    index = 0;
                                }

                            }
                            if (nbuf != null && nbuf.Length >= (index + size))
                            {
                                if (TagNode.ArrayDimension == 0)
                                {
                                    if (elemsize == 1)
                                        val = (Double)nbuf[index];
                                    else if (elemsize == 2)
                                        val = (Double)BitConverter.ToInt16(nbuf, index);
                                    else if (elemsize == 4)
                                        val = (Double)BitConverter.ToInt32(nbuf, index);
                                    else if (elemsize == 8)
                                        val = (Double)BitConverter.ToInt64(nbuf, index);
                                    else
                                        val = BitConverter.ToDouble(nbuf, index);
                                    bCopy = (readValue == null || Convert.ToDouble(readValue) != (double)val);
                                }
                                else
                                {
                                    double[] a = new double[TagNode.ArrayDimension];
                                    Array b = readValue as Array;
                                    bool check = (b != null && b.GetLength(0) == TagNode.ArrayDimension);
                                    bCopy = (!check);
                                    for (int i = 0; i < TagNode.ArrayDimension; i++)
                                    {
                                        if (elemsize == 1)
                                            a[i] = (Double)nbuf[index + 8 * i];
                                        else if (elemsize == 2)
                                            a[i] = (Double)BitConverter.ToInt16(nbuf, (int)(index + 8 * i));
                                        else if (elemsize == 4)
                                            a[i] = (Double)BitConverter.ToInt32(nbuf, (int)(index + 8 * i));
                                        else if (elemsize == 8)
                                            a[i] = (Double)BitConverter.ToInt64(nbuf, (int)(index + 8 * i));
                                        else
                                            a[i] = BitConverter.ToDouble(nbuf, index + 8 * i);
                                        if (check && a[i] != Convert.ToDouble(b.GetValue(i)))
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
                        }
                        break;
                    case (uint)Opc.Ua.DataTypes.Int16:
                        {
                            size = (TagNode.ArrayDimension == 0 ? 2 : TagNode.ArrayDimension * 2);
                            byte[] nbuf = null;
                            if (elemsize == 0)
                            {
                                nbuf = new byte[buffer.Length];
                                buffer.CopyTo(nbuf, 0);
                            }
                            else
                            {
                                uint newsize = (TagNode.ArrayDimension == 0 ? elemsize : TagNode.ArrayDimension * elemsize);
                                nbuf = new byte[size];
                                if (buffer == null || buffer.Length < (index + newsize))
                                    break;
                                if (TagNode.ArrayDimension == 0)
                                {
                                    Array.Copy(buffer, index, nbuf, 0, elemsize);

                                    index = 0;
                                }
                                else
                                {
                                    for (int i = 0; i < TagNode.ArrayDimension; i++)
                                    {
                                        Array.Copy(buffer, index + (elemsize * i), nbuf, i * 2, elemsize);
                                    }

                                    index = 0;
                                }

                            }
                            if (nbuf != null && nbuf.Length >= (index + size))
                            {
                                if (TagNode.ArrayDimension == 0)
                                {
                                    val = BitConverter.ToInt16(nbuf, index);
                                    bCopy = (readValue == null || Convert.ToInt16(readValue) != (Int16)val);
                                }
                                else
                                {
                                    Int16[] a = new Int16[TagNode.ArrayDimension];
                                    Array b = readValue as Array;
                                    bool check = (b != null && b.GetLength(0) == TagNode.ArrayDimension);
                                    bCopy = (!check);
                                    for (int i = 0; i < TagNode.ArrayDimension; i++)
                                    {
                                        a[i] = BitConverter.ToInt16(nbuf, index + 2 * i);
                                        if (check && a[i] != Convert.ToInt16(b.GetValue(i)))
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
                        }
                        break;
                    case (uint)Opc.Ua.DataTypes.Int32:
                        {
                            size = (TagNode.ArrayDimension == 0 ? 4 : TagNode.ArrayDimension * 4);
                            byte[] nbuf = null;
                            if (elemsize == 0)
                            {
                                nbuf = new byte[buffer.Length];
                                buffer.CopyTo(nbuf, 0);
                            }
                            else
                            {
                                uint newsize = (TagNode.ArrayDimension == 0 ? elemsize : TagNode.ArrayDimension * elemsize);
                                nbuf = new byte[size];
                                if (buffer == null || buffer.Length < (index + newsize))
                                    break;
                                if (TagNode.ArrayDimension == 0)
                                {
                                    Array.Copy(buffer, index, nbuf, 0, elemsize);
                                    index = 0;
                                }
                                else
                                {
                                    for (int i = 0; i < TagNode.ArrayDimension; i++)
                                    {
                                        Array.Copy(buffer, index + (elemsize * i), nbuf, i * 4, elemsize);
                                    }

                                    index = 0;
                                }

                            }
                            if (nbuf != null && nbuf.Length >= (index + size))
                            {
                                if (TagNode.ArrayDimension == 0)
                                {
                                    if (elemsize == 1)
                                        val = (Int32)nbuf[index];
                                    else if (elemsize == 2)
                                        val = (Int32)BitConverter.ToInt16(nbuf, index);
                                    else
                                        val = BitConverter.ToInt32(nbuf, index);
                                    bCopy = (readValue == null || Convert.ToInt32(readValue) != (Int32)val);
                                }
                                else
                                {
                                    Int32[] a = new Int32[TagNode.ArrayDimension];
                                    Array b = readValue as Array;
                                    bool check = (b != null && b.GetLength(0) == TagNode.ArrayDimension);
                                    bCopy = (!check);
                                    for (int i = 0; i < TagNode.ArrayDimension; i++)
                                    {
                                        a[i] = BitConverter.ToInt32(nbuf, index + 4 * i);
                                        if (check && a[i] != Convert.ToInt32(b.GetValue(i)))
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
                        }
                        break;
                    case (uint)Opc.Ua.DataTypes.Int64:
                        {
                            size = (TagNode.ArrayDimension == 0 ? 8 : TagNode.ArrayDimension * 8);
                            byte[] nbuf = null;
                            if (elemsize == 0)
                            {
                                nbuf = new byte[buffer.Length];
                                buffer.CopyTo(nbuf, 0);
                            }
                            else
                            {
                                uint newsize = (TagNode.ArrayDimension == 0 ? elemsize : TagNode.ArrayDimension * elemsize);
                                nbuf = new byte[size];
                                if (buffer == null || buffer.Length < (index + newsize))
                                    break;
                                if (TagNode.ArrayDimension == 0)
                                {
                                    Array.Copy(buffer, index, nbuf, 0, elemsize);
                                    index = 0;
                                }
                                else
                                {
                                    for (int i = 0; i < TagNode.ArrayDimension; i++)
                                    {
                                        Array.Copy(buffer, index + (elemsize * i), nbuf, i * 8, elemsize);
                                    }

                                    index = 0;
                                }

                            }
                            if (nbuf != null && nbuf.Length >= (index + size))
                            {
                                if (TagNode.ArrayDimension == 0)
                                {
                                    if (elemsize == 1)
                                        val = (Int64)nbuf[index];
                                    else if (elemsize == 2)
                                        val = (Int64)BitConverter.ToInt16(nbuf, index);
                                    else if (elemsize == 4)
                                        val = (Int64)BitConverter.ToInt32(nbuf, index);
                                    else
                                        val = BitConverter.ToInt64(nbuf, index);
                                    bCopy = (readValue == null || Convert.ToInt64(readValue) != (Int64)val);
                                }
                                else
                                {
                                    Int64[] a = new Int64[TagNode.ArrayDimension];
                                    Array b = readValue as Array;
                                    bool check = (b != null && b.GetLength(0) == TagNode.ArrayDimension);
                                    bCopy = (!check);
                                    for (int i = 0; i < TagNode.ArrayDimension; i++)
                                    {
                                        a[i] = BitConverter.ToInt64(nbuf, index + 8 * i);
                                        if (check && a[i] != Convert.ToInt64(b.GetValue(i)))
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
                        }
                        break;
                    case (uint)Opc.Ua.DataTypes.Float:
                        {
                            size = (TagNode.ArrayDimension == 0 ? 4 : TagNode.ArrayDimension * 4);
                            byte[] nbuf = null;
                            if (elemsize == 0)
                            {
                                nbuf = new byte[buffer.Length];
                                buffer.CopyTo(nbuf, 0);
                            }
                            else
                            {
                                uint newsize = (TagNode.ArrayDimension == 0 ? elemsize : TagNode.ArrayDimension * elemsize);
                                if(size >= newsize)
                                {
                                    nbuf = new byte[size];
                                }
                                else
                                { 
                                    nbuf = new byte[newsize];
                                }
                                if (buffer == null || buffer.Length < (index + newsize))
                                    break;
                                if (TagNode.ArrayDimension == 0)
                                {
                                    Array.Copy(buffer, index, nbuf, 0, elemsize);
                                    index = 0;
                                }
                                else
                                {
                                    for (int i = 0; i < TagNode.ArrayDimension; i++)
                                    {
                                        Array.Copy(buffer, index + (elemsize * i), nbuf, i * 4, elemsize);
                                    }

                                    index = 0;
                                }

                            }
                            if (nbuf != null && nbuf.Length >= (index + size))
                            {
                                if (TagNode.ArrayDimension == 0)
                                {
                                    if (elemsize == 1)
                                        val = (Single)nbuf[index];
                                    else if (elemsize == 2)
                                        val = (Single)BitConverter.ToInt16(nbuf, index);
                                    else if (elemsize == 4)
                                        val = (Single)BitConverter.ToInt32(nbuf, index);
                                    else if (elemsize == 8)
                                        val = (Single)BitConverter.ToInt64(nbuf, index);
                                    else
                                        val = BitConverter.ToSingle(nbuf, index);
                                    bCopy = (readValue == null || Convert.ToSingle(readValue) != (float)val);
                                }
                                else
                                {
                                    float[] a = new float[TagNode.ArrayDimension];
                                    Array b = readValue as Array;
                                    bool check = (b != null && b.GetLength(0) == TagNode.ArrayDimension);
                                    bCopy = (!check);
                                    for (int i = 0; i < TagNode.ArrayDimension; i++)
                                    {
                                        if (elemsize == 1)
                                            a[i] = (Single)nbuf[(int)(index + 4 * i)];
                                        else if (elemsize == 2)
                                            a[i] = (Single)BitConverter.ToInt16(nbuf, (int)(index + 4 * i));
                                        else if (elemsize == 4)
                                            a[i] = (Single)BitConverter.ToInt32(nbuf, (int)(index + 4 * i));
                                        else if (elemsize == 8)
                                            a[i] = (Single)BitConverter.ToInt64(nbuf, (int)(index + 4 * i));
                                        else
                                            a[i] = BitConverter.ToSingle(nbuf, index + 4 * i);
                                        if (check && a[i] != Convert.ToSingle(b.GetValue(i)))
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
                        }
                        break;
                    case (uint)Opc.Ua.DataTypes.UInt32:
                        {
                            size = (TagNode.ArrayDimension == 0 ? 4 : TagNode.ArrayDimension * 4);

                            byte[] nbuf = null;
                            if (elemsize == 0)
                            {
                                nbuf = new byte[buffer.Length];
                                buffer.CopyTo(nbuf, 0);
                            }
                            else
                            {
                                uint newsize = (TagNode.ArrayDimension == 0 ? elemsize : TagNode.ArrayDimension * elemsize);
                                nbuf = new byte[size];
                                if (buffer == null || buffer.Length < (index + newsize))
                                    break;
                                if (TagNode.ArrayDimension == 0)
                                {
                                    Array.Copy(buffer, index, nbuf, 0, elemsize);
                                    index = 0;
                                }
                                else
                                {
                                    for (int i = 0; i < TagNode.ArrayDimension; i++)
                                    {
                                        Array.Copy(buffer, index + (elemsize * i), nbuf, i * 4, elemsize);
                                    }

                                    index = 0;
                                }

                            }

                            if (nbuf != null && nbuf.Length >= (index + size))
                            {
                                if (TagNode.ArrayDimension == 0)
                                {
                                    val = BitConverter.ToUInt32(nbuf, index);
                                    bCopy = (readValue == null || Convert.ToUInt32(readValue) != (UInt32)val);
                                }
                                else
                                {
                                    UInt32[] a = new UInt32[TagNode.ArrayDimension];
                                    Array b = readValue as Array;
                                    bool check = (b != null && b.GetLength(0) == TagNode.ArrayDimension);
                                    bCopy = (!check);
                                    for (int i = 0; i < TagNode.ArrayDimension; i++)
                                    {
                                        a[i] = BitConverter.ToUInt32(nbuf, index + 4 * i);
                                        if (check && a[i] != Convert.ToUInt32(b.GetValue(i)))
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
                        }
                        break;
                    case (uint)Opc.Ua.DataTypes.UInt64:
                        {
                            size = (TagNode.ArrayDimension == 0 ? 8 : TagNode.ArrayDimension * 8);
                            byte[] nbuf = null;
                            if (elemsize == 0)
                            {
                                nbuf = new byte[buffer.Length];
                                buffer.CopyTo(nbuf, 0);
                            }
                            else
                            {
                                uint newsize = (TagNode.ArrayDimension == 0 ? elemsize : TagNode.ArrayDimension * elemsize);
                                nbuf = new byte[size];
                                if (buffer == null || buffer.Length < (index + newsize))
                                    break;
                                if (TagNode.ArrayDimension == 0)
                                {
                                    Array.Copy(buffer, index, nbuf, 0, elemsize);
                                    index = 0;
                                }
                                else
                                {
                                    for (int i = 0; i < TagNode.ArrayDimension; i++)
                                    {
                                        Array.Copy(buffer, index + (elemsize * i), nbuf, i * 8, elemsize);
                                    }

                                    index = 0;
                                }

                            }
                            if (nbuf != null && nbuf.Length >= (index + size))
                            {
                                if (TagNode.ArrayDimension == 0)
                                {
                                    val = BitConverter.ToUInt64(nbuf, index);
                                    bCopy = (readValue == null || Convert.ToUInt64(readValue) != (UInt64)val);
                                }
                                else
                                {
                                    UInt64[] a = new UInt64[TagNode.ArrayDimension];
                                    Array b = readValue as Array;
                                    bool check = (b != null && b.GetLength(0) == TagNode.ArrayDimension);
                                    bCopy = (!check);
                                    for (int i = 0; i < TagNode.ArrayDimension; i++)
                                    {
                                        a[i] = BitConverter.ToUInt64(nbuf, index + 8 * i);
                                        if (check && a[i] != Convert.ToUInt64(b.GetValue(i)))
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
                        }
                        break;
                }
            }
            return false;
        }

        ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>   Sets read value. </summary>
        ///
        /// <param name="val" type="object">    The value. </param>
        ////////////////////////////////////////////////////////////////////////////////////////////////////
        public void SetValue(object val)
        {
#if DEBUG
            System.Diagnostics.Trace.TraceInformation("SetReadValue {0} {1}", val, TagNode.Name);
#endif
            Value.Value = Utils.Clone(val);
        }

        ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>   Sets write value. </summary>
        ///
        /// <param name="val" type="object">    The value. </param>
        ////////////////////////////////////////////////////////////////////////////////////////////////////
        public void SetWriteVal(object val)
        {
            WriteVal = Utils.Clone(val);
        }

        ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>   Gets read value. </summary>
        ///
        /// <returns>   The read value. </returns>
        ////////////////////////////////////////////////////////////////////////////////////////////////////
        public object GetValue()
        {
            return Utils.Clone(Value.Value);
        }


        ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>   Gets read value. </summary>
        ///
        /// <returns>   The read value. </returns>
        ////////////////////////////////////////////////////////////////////////////////////////////////////
        public static bool IsReadValueEqualsTo(Tag tag, object value1, object value2)
        {
            if (tag.TagNode.ArrayDimension == 0)
            {
                return value1.Equals(value2);
            }
            else
            {
                Type readValueArrayType = value1.GetType();
                Type valueArrayType = value2.GetType();
                if ((readValueArrayType != valueArrayType ) || (!readValueArrayType.IsArray || !valueArrayType.IsArray))
                    return false;

                Array a1 = value1 as Array;
                Array a2 = value2 as Array;

                if (a1.Length != a2.Length) 
                    return false;

                bool equal = true;
                try
                {
                    for (int i = 0; i < a1.Length; i++)
                    {
                        equal &= a1.GetValue(i).Equals(a2.GetValue(i));
                        if (!equal)
                            break;
                    }
                } catch (Exception ex)
                {
                    equal = false;
                }

                return equal;
            }
        }

        public static object ArraySumTagValue(Tag tag, object value, object targetArray)
        {
            if (tag.TagNode.ArrayDimension > 0 && tag.TagNode.DataType.IdType == IdType.Numeric)
            {
                if (targetArray == null)
                {
                    return Utils.Clone(value);
                }
                else
                {
                    switch ((uint)tag.TagNode.DataType.Identifier)
                    {
                        case Opc.Ua.DataTypes.Boolean:
                            {
                                Boolean[] source = (Boolean[])value;
                                Boolean[] target = (Boolean[])targetArray;

                                return target.Concat(source).ToArray();
                            }
                        case Opc.Ua.DataTypes.Byte:
                            {
                                byte[] source = (byte[])value;
                                byte[] target = (byte[])targetArray;

                                return target.Concat(source).ToArray();
                            }
                        case Opc.Ua.DataTypes.SByte:
                            {
                                SByte[] source = (SByte[])value;
                                SByte[] target = (SByte[])targetArray;

                                return target.Concat(source).ToArray();
                            }
                        case Opc.Ua.DataTypes.Int16:
                            {
                                Int16[] source = (Int16[])value;
                                Int16[] target = (Int16[])targetArray;

                                return target.Concat(source).ToArray();
                            }
                        case Opc.Ua.DataTypes.UInt16:
                            {
                                UInt16[] source = (UInt16[])value;
                                UInt16[] target = (UInt16[])targetArray;

                                return target.Concat(source).ToArray();
                            }
                        case Opc.Ua.DataTypes.Int32:
                            {
                                Int32[] source = (Int32[])value;
                                Int32[] target = (Int32[])targetArray;

                                return target.Concat(source).ToArray();
                            }
                        case Opc.Ua.DataTypes.UInt32:
                            {
                                UInt32[] source = (UInt32[])value;
                                UInt32[] target = (UInt32[])targetArray;

                                return target.Concat(source).ToArray();
                            }
                        case Opc.Ua.DataTypes.Int64:
                            {
                                Int64[] source = (Int64[])value;
                                Int64[] target = (Int64[])targetArray;

                                return target.Concat(source).ToArray();
                            }
                        case Opc.Ua.DataTypes.UInt64:
                            {
                                UInt64[] source = (UInt64[])value;
                                UInt64[] target = (UInt64[])targetArray;

                                return target.Concat(source).ToArray();
                            }
                        case Opc.Ua.DataTypes.Float:
                            {
                                Single[] source = (Single[])value;
                                Single[] target = (Single[])targetArray;

                                return target.Concat(source).ToArray();
                            }
                        case Opc.Ua.DataTypes.Double:
                            {
                                Double[] source = (Double[])value;
                                Double[] target = (Double[])targetArray;

                                return target.Concat(source).ToArray();
                            }
                    }
                }
            }

            return targetArray;
        }

        public static object ArrayGetTagValue(Tag tag, object value, int startIndex, int length)
        {
            if (tag.TagNode.ArrayDimension > 0 && tag.TagNode.DataType.IdType == IdType.Numeric)
            {
                if (value != null)
                {                    
                    switch ((uint)tag.TagNode.DataType.Identifier)
                    {
                        case Opc.Ua.DataTypes.Boolean:
                            {
                                Boolean[] source = (Boolean[])value;
                                return source.Skip(startIndex).Take(length).ToArray();
                            }
                        case Opc.Ua.DataTypes.Byte:
                            {
                                byte[] source = (byte[])value;
                                return source.Skip(startIndex).Take(length).ToArray();
                            }
                        case Opc.Ua.DataTypes.SByte:
                            {
                                SByte[] source = (SByte[])value;
                                return source.Skip(startIndex).Take(length).ToArray();
                            }
                        case Opc.Ua.DataTypes.Int16:
                            {
                                Int16[] source = (Int16[])value;
                                return source.Skip(startIndex).Take(length).ToArray();
                            }
                        case Opc.Ua.DataTypes.UInt16:
                            {
                                UInt16[] source = (UInt16[])value;
                                return source.Skip(startIndex).Take(length).ToArray();
                            }
                        case Opc.Ua.DataTypes.Int32:
                            {
                                Int32[] source = (Int32[])value;
                                return source.Skip(startIndex).Take(length).ToArray();
                            }
                        case Opc.Ua.DataTypes.UInt32:
                            {
                                UInt32[] source = (UInt32[])value;
                                return source.Skip(startIndex).Take(length).ToArray();
                            }
                        case Opc.Ua.DataTypes.Int64:
                            {
                                Int64[] source = (Int64[])value;
                                return source.Skip(startIndex).Take(length).ToArray();
                            }
                        case Opc.Ua.DataTypes.UInt64:
                            {
                                UInt64[] source = (UInt64[])value;
                                return source.Skip(startIndex).Take(length).ToArray();
                            }
                        case Opc.Ua.DataTypes.Float:
                            {
                                Single[] source = (Single[])value;
                                return source.Skip(startIndex).Take(length).ToArray();
                            }
                        case Opc.Ua.DataTypes.Double:
                            {
                                Double[] source = (Double[])value;
                                return source.Skip(startIndex).Take(length).ToArray();
                            }
                    }
                }
            }

            return null;
        }

        ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>   Sets internal values. </summary>
        ///
        /// <param name="val" type="object">    The value. </param>
        ////////////////////////////////////////////////////////////////////////////////////////////////////
        //public void SetInternalValues(object val)
        //{
        //    WriteVal = val;
        //}

        ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>   Gets tag buffer. </summary>
        ///
        /// <param name="buffer" type="ref byte []">    [in,out] The buffer. </param>
        /// <param name="read" type="bool">             (Optional) true if the data was read. </param>
        /// <param name="index" type="int">             (Optional) zero-based index of the. </param>
        /// <param name="elemsize" type="uint">             (Optional) size of the protocol element. </param>
        /// <param name="buffertype" type="uint">             (Optional) DataTypes of the destination buffer. </param>
        /// <returns>   The tag buffer. </returns>
        ////////////////////////////////////////////////////////////////////////////////////////////////////
        public virtual uint GetTagBuffer(ref byte [] buffer, bool read = true, int index = 0, uint elemsize = 0, uint buffertype = 0)
        {
            object curVal = WriteVal;
            uint size = 0;
            if (TagNode.DataType.IdType == IdType.Numeric)
            {
                uint nType = (uint)TagNode.DataType.Identifier;
                switch (nType)
                {
                    case (uint)Opc.Ua.DataTypes.String:
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
                                                Array.Copy(enc.GetBytes((string)b.GetValue(i)), 0, buffer, index + (i*elemlen), min);
                                                if (min < elemlen)
                                                {
                                                    for(int k = 0; k < (elemlen-min); k++)
                                                    Array.Copy(enc.GetBytes((string)" "), 0, buffer, index + (i * elemlen)+min+k, 1);
                                                }
                                                size += (uint)elemlen;
                                            }
                                        }
                                    }
                                }
                            }
                        }
                        break;
                    case (uint)Opc.Ua.DataTypes.Boolean:
                        // Modified to solve FOGBUGZ 11044
                        size = (uint)(TagNode.ArrayDimension == 0 ? 1 : TagNode.ArrayDimension / 8 + (TagNode.ArrayDimension % 8 > 0 ? 1 : 0));
                        if (buffer != null && buffer.Length >= size)
                        {
                            if (TagNode.ArrayDimension == 0)
                            {
                                if (curVal != null)
                                {
                                    BitConverter.GetBytes((bool)DataTypeExtensions.ChangeType(curVal, UFUAModel.DataType.Boolean)).CopyTo(buffer, index);
                                }
                            }
                            else
                            {
                                Array b = curVal as Array;
                                if (b != null && b.GetLength(0) == TagNode.ArrayDimension)
                                {
                                    int j = 0;
                                    int k = 0;
                                    for (int i = 0; i < TagNode.ArrayDimension; i++)
                                    {
                                        if ((bool)DataTypeExtensions.ChangeType(b.GetValue(i), UFUAModel.DataType.Boolean))
                                        {
                                            buffer[index + k] += (byte)(1 << j);
                                        }
                                        if (++j > 7)
                                        {
                                            j = 0;
                                            k++;
                                        }
                                    }
                                }
                            }
                        }
                        break;
                    case (uint)Opc.Ua.DataTypes.SByte:
                        size = (TagNode.ArrayDimension == 0 ? 1 : TagNode.ArrayDimension);
                        if (buffer != null && buffer.Length >= size)
                        {
                            SByte charAux;
                            if (TagNode.ArrayDimension == 0)
                            {
                                if (curVal != null)
                                {
                                    // Modified  to solve FOGBUGZ 11044
                                    charAux = (SByte)DataTypeExtensions.ChangeType(curVal, UFUAModel.DataType.SByte);
                                    buffer[index] = (byte)charAux;
                                }
                            }
                            else
                            {
                                Array b = curVal as Array;
                                if (b != null && b.GetLength(0) == TagNode.ArrayDimension)
                                {
                                    for (int i = 0; i < TagNode.ArrayDimension; i++)
                                    {
                                        // Modified  to solve FOGBUGZ 11044
                                        charAux = (SByte)DataTypeExtensions.ChangeType(b.GetValue(i), UFUAModel.DataType.SByte);
                                        buffer[index + i] = (byte)charAux;
                                    }
                                }
                            }
                        }
                        break;
                    case (uint)Opc.Ua.DataTypes.Byte:
                        size = (TagNode.ArrayDimension == 0 ? 1 : TagNode.ArrayDimension);
                        if (buffer != null && buffer.Length >= size)
                        {
                            if (TagNode.ArrayDimension == 0)
                            {
                                if (curVal != null)
                                {
                                    // Modified  to solve FOGBUGZ 11044
                                    buffer[index] = (byte)DataTypeExtensions.ChangeType(curVal, UFUAModel.DataType.Byte);
                                }
                            }
                            else
                            {
                                Array b = curVal as Array;
                                if (b != null && b.GetLength(0) == TagNode.ArrayDimension)
                                {
                                    for (int i = 0; i < TagNode.ArrayDimension; i++)
                                    {
                                        // Modified  to solve FOGBUGZ 11044
                                        buffer[index + i] = (byte)(DataTypeExtensions.ChangeType(b.GetValue(i), UFUAModel.DataType.Byte));
                                    }
                                }
                            }
                        }
                        break;
                    case (uint)Opc.Ua.DataTypes.UInt16:
                        size = (TagNode.ArrayDimension == 0 ? 2 : TagNode.ArrayDimension * 2);
                        if (elemsize > 0)
                        {
                            size = (TagNode.ArrayDimension == 0 ? elemsize : TagNode.ArrayDimension * elemsize);
                        }
                        else
                            elemsize = 2;
                        if (buffer != null && buffer.Length >= size)
                        {
                            if (TagNode.ArrayDimension == 0)
                            {
                                if (curVal != null)
                                {
                                    Array.Copy(BitConverter.GetBytes((UInt16)DataTypeExtensions.ChangeType(curVal, UFUAModel.DataType.UInt16)), 0, buffer, index, size);
                                }
                            }
                            else
                            {
                                Array b = curVal as Array;
                                if (b != null && b.GetLength(0) == TagNode.ArrayDimension)
                                {
                                    for (int i = 0; i < TagNode.ArrayDimension; i++)
                                    {
                                        Array.Copy(BitConverter.GetBytes((UInt16)DataTypeExtensions.ChangeType(b.GetValue(i), UFUAModel.DataType.UInt16)), 0, buffer, index + elemsize * i, elemsize);
                                    }
                                }
                            }
                        }
                        break;
                    case (uint)Opc.Ua.DataTypes.Double:
                        if (elemsize > 0)
                        {
                            size = (TagNode.ArrayDimension == 0 ? elemsize : TagNode.ArrayDimension * elemsize);
                        }
                        else
                            size = (TagNode.ArrayDimension == 0 ? 8 : TagNode.ArrayDimension * 8);
                        if (buffer != null && buffer.Length >= size)
                        {
                            if (TagNode.ArrayDimension == 0)
                            {
                                if (curVal != null)
                                {
                                    if (elemsize == 0)
                                        Array.Copy(BitConverter.GetBytes((double)DataTypeExtensions.ChangeType(curVal, UFUAModel.DataType.Double)), 0, buffer, index, size);
                                    else
                                        CopyToIntElement(buffer, index, elemsize, DataTypeExtensions.ChangeType(curVal, UFUAModel.DataType.Double));
                                }
                            }
                            else
                            {
                                Array b = curVal as Array;
                                if (b != null && b.GetLength(0) == TagNode.ArrayDimension)
                                {
                                    for (int i = 0; i < TagNode.ArrayDimension; i++)
                                    {
                                        if (elemsize == 0)
                                            Array.Copy(BitConverter.GetBytes((double)DataTypeExtensions.ChangeType(b.GetValue(i), UFUAModel.DataType.Double)), 0, buffer, index + 8 * i, 8);
                                        else
                                            CopyToIntElement(buffer, (int)(index + elemsize * i), elemsize, (double)DataTypeExtensions.ChangeType(b.GetValue(i), UFUAModel.DataType.Double));
                                    }
                                }
                            }
                        }
                        break;
                    case (uint)Opc.Ua.DataTypes.Int16:
                        size = (TagNode.ArrayDimension == 0 ? 2 : TagNode.ArrayDimension * 2);
                        if (elemsize > 0)
                        {
                            size = (TagNode.ArrayDimension == 0 ? elemsize : TagNode.ArrayDimension * elemsize);
                        }
                        else
                            elemsize = 2;
                        if (buffer != null && buffer.Length >= size)
                        {
                            if (TagNode.ArrayDimension == 0)
                            {
                                if (curVal != null)
                                {
                                    Array.Copy(BitConverter.GetBytes((Int16)DataTypeExtensions.ChangeType(curVal, UFUAModel.DataType.Int16)), 0, buffer, index, size);
                                }
                            }
                            else
                            {
                                Array b = curVal as Array;
                                if (b != null && b.GetLength(0) == TagNode.ArrayDimension)
                                {
                                    for (int i = 0; i < TagNode.ArrayDimension; i++)
                                    {
                                        Array.Copy(BitConverter.GetBytes((Int16)DataTypeExtensions.ChangeType(b.GetValue(i), UFUAModel.DataType.Int16)), 0, buffer, index + elemsize * i, elemsize);
                                    }
                                }
                            }
                        }
                        break;
                    case (uint)Opc.Ua.DataTypes.Int32:
                        size = (TagNode.ArrayDimension == 0 ? 4 : TagNode.ArrayDimension * 4);
                        if (elemsize > 0)
                        {
                            size = (TagNode.ArrayDimension == 0 ? elemsize : TagNode.ArrayDimension * elemsize);
                        }
                        else
                            elemsize = 4;
                        if (buffer != null && buffer.Length >= size)
                        {
                            if (TagNode.ArrayDimension == 0)
                            {
                                if (curVal != null)
                                {
                                    Array.Copy(BitConverter.GetBytes((Int32)DataTypeExtensions.ChangeType(curVal, UFUAModel.DataType.Int32)), 0, buffer, index, size);
                                }
                            }
                            else
                            {
                                Array b = curVal as Array;
                                if (b != null && b.GetLength(0) == TagNode.ArrayDimension)
                                {
                                    for (int i = 0; i < TagNode.ArrayDimension; i++)
                                    {
                                        Array.Copy(BitConverter.GetBytes((Int32)DataTypeExtensions.ChangeType(b.GetValue(i), UFUAModel.DataType.Int32)), 0, buffer, index + elemsize * i, elemsize);
                                    }
                                }
                            }
                        }
                        break;
                    case (uint)Opc.Ua.DataTypes.Int64:
                        size = (TagNode.ArrayDimension == 0 ? 8 : TagNode.ArrayDimension * 8);
                        if (elemsize > 0)
                        {
                            size = (TagNode.ArrayDimension == 0 ? elemsize : TagNode.ArrayDimension * elemsize);
                        }
                        else
                            elemsize = 8;
                        if (buffer != null && buffer.Length >= size)
                        {
                            if (TagNode.ArrayDimension == 0)
                            {
                                if (curVal != null)
                                {
                                    Array.Copy(BitConverter.GetBytes((Int64)DataTypeExtensions.ChangeType(curVal, UFUAModel.DataType.Int64)), 0, buffer, index, size);
                                }
                            }
                            else
                            {
                                Array b = curVal as Array;
                                if (b != null && b.GetLength(0) == TagNode.ArrayDimension)
                                {
                                    for (int i = 0; i < TagNode.ArrayDimension; i++)
                                    {
                                        Array.Copy(BitConverter.GetBytes((Int64)DataTypeExtensions.ChangeType(b.GetValue(i), UFUAModel.DataType.Int64)), 0, buffer, index + elemsize * i, elemsize);
                                    }
                                }
                            }
                        }
                        break;
                    case (uint)Opc.Ua.DataTypes.Float:
                        if (elemsize > 0)
                        {
                            size = (TagNode.ArrayDimension == 0 ? elemsize : TagNode.ArrayDimension * elemsize);
                        }
                        else
                            size = (TagNode.ArrayDimension == 0 ? 4 : TagNode.ArrayDimension * 4);

                        if (buffer != null && buffer.Length >= size)
                        {
                            if (TagNode.ArrayDimension == 0)
                            {
                                if (curVal != null)
                                {
                                    if (elemsize == 0)
                                        Array.Copy(BitConverter.GetBytes((float)DataTypeExtensions.ChangeType(curVal, UFUAModel.DataType.Float)), 0, buffer, index, size);
                                    else
                                        CopyToIntElement(buffer, index, elemsize, DataTypeExtensions.ChangeType(curVal, UFUAModel.DataType.Float));
                                }
                            }
                            else
                            {
                                Array b = curVal as Array;
                                if (b != null && b.GetLength(0) == TagNode.ArrayDimension)
                                {
                                    for (int i = 0; i < TagNode.ArrayDimension; i++)
                                    {
                                        if (elemsize == 0)
                                        {
                                            // Modifier in version 2.2.30.0 (FOGBUGZ 15461)
                                            Array.Copy(BitConverter.GetBytes((float)DataTypeExtensions.ChangeType(b.GetValue(i), UFUAModel.DataType.Float)), 0, buffer, index + 4 * i, 4);
                                        }                                            
                                        else
                                            CopyToIntElement(buffer, (int)(index + elemsize * i), elemsize, (float)DataTypeExtensions.ChangeType(b.GetValue(i), UFUAModel.DataType.Float));
                                    }
                                }
                            }
                        }
                        break;
                    case (uint)Opc.Ua.DataTypes.UInt32:
                        size = (TagNode.ArrayDimension == 0 ? 4 : TagNode.ArrayDimension * 4);

                        if (elemsize > 0)
                        {
                            size = (TagNode.ArrayDimension == 0 ? elemsize : TagNode.ArrayDimension * elemsize);
                        }
                        else
                            elemsize = 4;

                        if (buffer != null && buffer.Length >= size)
                        {
                            if (TagNode.ArrayDimension == 0)
                            {
                                if (curVal != null)
                                {
                                    Array.Copy(BitConverter.GetBytes((UInt32)DataTypeExtensions.ChangeType(curVal, UFUAModel.DataType.UInt32)), 0, buffer, index, size);
                                }
                            }
                            else
                            {
                                Array b = curVal as Array;
                                if (b != null && b.GetLength(0) == TagNode.ArrayDimension)
                                {
                                    for (int i = 0; i < TagNode.ArrayDimension; i++)
                                    {
                                        Array.Copy(BitConverter.GetBytes((UInt32)DataTypeExtensions.ChangeType(b.GetValue(i), UFUAModel.DataType.UInt32)), 0, buffer, index + elemsize * i, elemsize);
                                    }
                                }
                            }
                        }

                        break;
                    case (uint)Opc.Ua.DataTypes.UInt64:
                        size = (TagNode.ArrayDimension == 0 ? 8 : TagNode.ArrayDimension * 8);
                        if (elemsize > 0)
                        {
                            size = (TagNode.ArrayDimension == 0 ? elemsize : TagNode.ArrayDimension * elemsize);
                        }
                        else
                            elemsize = 8;
                        if (buffer != null && buffer.Length >= size)
                        {
                            if (TagNode.ArrayDimension == 0)
                            {
                                if (curVal != null)
                                {
                                    Array.Copy(BitConverter.GetBytes((UInt64)DataTypeExtensions.ChangeType(curVal, UFUAModel.DataType.UInt64)), 0, buffer, index, size);
                                }
                            }
                            else
                            {
                                Array b = curVal as Array;
                                if (b != null && b.GetLength(0) == TagNode.ArrayDimension)
                                {
                                    for (int i = 0; i < TagNode.ArrayDimension; i++)
                                    {
                                        Array.Copy(BitConverter.GetBytes((UInt64)DataTypeExtensions.ChangeType(b.GetValue(i), UFUAModel.DataType.UInt64)), 0, buffer, index + elemsize * i, elemsize);
                                    }
                                }
                            }
                        }
                        break;
                }
                if (!(buffer != null && buffer.Length >= size))
                    size = 0;
            }
            return size;
        }

        private static void CopyToIntElement(byte[] buffer, int index, uint elemsize, object curVal)
        {
            Int64 tmp = Convert.ToInt64(curVal);
            
            if (elemsize == 1)
                Array.Copy(BitConverter.GetBytes(tmp % 0x100), 0, buffer, index, elemsize);
            else if (elemsize == 2)
                Array.Copy(BitConverter.GetBytes(tmp % 0x10000), 0, buffer, index, elemsize);
            else if (elemsize == 4)
                Array.Copy(BitConverter.GetBytes(tmp % 0x100000000), 0, buffer, index, elemsize);
            else if (elemsize == 8)
                Array.Copy(BitConverter.GetBytes(tmp), 0, buffer, index, elemsize);
        }

        /// <summary>
        /// Check if text match with filter pattern
        /// </summary>
        /// <param name="text"></param>
        /// <param name="pattern"></param>
        /// <param name="indexPattern"></param>
        /// <param name="totalChar"></param>
        /// <returns></returns>
        public static bool ImportTagMatchPattern(string text, string pattern, int indexPattern, int totalChar)
        {
            // is a syncfusion pattern ?
            switch (pattern.Substring(0, 1))
            {
                case "#":   // match all text conatin pattern chararcters 
                    pattern = pattern.Substring(1, pattern.Length - 1);
                    if (pattern.Length == 0)
                        return false;
                    else
                        return text.ToUpper().Contains(pattern.ToUpper());
                case "%":   // match all text with final chararcters equal to pattern
                    pattern = pattern.Substring(1, pattern.Length - 1);
                    if (pattern.Length == 0 || text.Length < pattern.Length)
                        return false;
                    else
                        return (text.ToUpper().Substring(text.Length - pattern.Length, pattern.Length).ToUpper() == pattern.ToUpper());
                default:    //
                    //--> not contain special chacter --> is not Movicon 11/Windows search patter
                    if (!(pattern.Contains("{") || pattern.Contains("}") || pattern.Contains("?") || pattern.Contains("*")))
                    {
                        // match all text with initial chararcters equal to pattern
                        if (text.Length < pattern.Length)
                            return false;
                        else
                            return (text.Substring(0, pattern.Length).ToUpper() == pattern.ToUpper());
                    }
                    break;
            }

            string C = null;
            string P = null;

            // Movicon 11 / Windows default search patter/jolly charaters
            while (true)
            {
                if (indexPattern < pattern.Length)
                    P = pattern.Substring(indexPattern++, 1).ToUpper();
                else
                    return (totalChar == text.Length);

                // char of pattern
                switch (P)
                {
                    case "*":
                        // find next strinng element match next pattern char
                        while (totalChar < text.Length)
                        {
                            if (ImportTagMatchPattern(text, pattern, indexPattern, totalChar++))
                                return true;
                        }

                        return ImportTagMatchPattern(text, pattern, indexPattern, totalChar);

                        break;
                    case "?":
                        if (totalChar >= text.Length)
                            return false;

                        // go ahead
                        totalChar++;
                        break;
                    // match set of chars {A,B,C}  {A-Z} format
                    case "{":
                        //if (totalChar >= text.Length)
                        //    return false;
                        //C = text.Substring(totalChar, 1).ToUpper();

                        int EndOfSubPattern = pattern.IndexOf("}", indexPattern);
                        // wrong expression
                        if (EndOfSubPattern < 0)
                            return false;

                        string SubPattern = pattern.Substring(indexPattern, EndOfSubPattern - indexPattern).Trim().ToUpper();
                        if (string.IsNullOrEmpty(SubPattern))
                            return false;

                        // parse subexpression as regular expression
                        return Regex.IsMatch(text.ToUpper(), string.Format("[{0}]", SubPattern));

                        break;

                    // mtach exact char
                    default:
                        if (totalChar >= text.Length)
                            return false;
                        C = text.Substring(totalChar, 1).ToUpper();

                        if (C != P)
                            return false;

                        totalChar++;
                        break;
                }
            }
        }

        #endregion      
    
        #region Properties
        /// <summary>   The sampling interval. </summary>
        private double _SamplingInterval;
        ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>   Gets or sets the sampling interval. </summary>
        ///
        /// <value> The sampling interval. </value>
        ////////////////////////////////////////////////////////////////////////////////////////////////////
        public double SamplingInterval
        {
            get { return _SamplingInterval; }
            set
            {
                _SamplingInterval = value;
            }
        }

        /// <summary>   The byte offset. </summary>
        private uint _ByteOffset;
        ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>   Gets or sets the byte offset. </summary>
        ///
        /// <value> The byte offset. </value>
        ////////////////////////////////////////////////////////////////////////////////////////////////////
        public uint ByteOffset
        {
            get { return _ByteOffset; }
            set
            {
                _ByteOffset = value;
            }
        }

        /// <summary>   The bit offset. </summary>
        private uint _BitOffset;
        ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>   Gets or sets the bit offset. </summary>
        ///
        /// <value> The bit offset. </value>
        ////////////////////////////////////////////////////////////////////////////////////////////////////
        public uint BitOffset
        {
            get { return _BitOffset; }
            set
            {
                _BitOffset = value;
            }
        }

        /// <summary>   The size. </summary>
        private uint _Size;
        ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>   Gets or sets the size. </summary>
        ///
        /// <value> The size. </value>
        ////////////////////////////////////////////////////////////////////////////////////////////////////
        public uint Size
        {
            get { return _Size; }
            set
            {
                _Size = value;
            }
        }

        /// <summary>   true to in use. </summary>
        private bool _InUse;
        ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>   Gets or sets a value indicating whether the in use. </summary>
        ///
        /// <value> true if in use, false if not. </value>
        ////////////////////////////////////////////////////////////////////////////////////////////////////
        public bool InUse
        {
            get { return _InUse; }
            set
            {
                _InUse = value;
            }
        }

        /// <summary>   The value. </summary>
        private Opc.Ua.DataValue _Value;
        ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>   Gets the value. </summary>
        ///
        /// <value> The value. </value>
        ////////////////////////////////////////////////////////////////////////////////////////////////////
        public Opc.Ua.DataValue Value
        {
            get 
            { 
                if (_Value == null)
                    _Value = new DataValue();

                return _Value; 
            
            }
        }

        private bool _IsValueChangedByChild = false;
        ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary> Set tag value change state if child job with the same tag value is changed </summary>
        ///
        /// <value> true if child job value if changed, false if not. </value>
        ////////////////////////////////////////////////////////////////////////////////////////////////////
        public bool IsValueChangedByChild
        {
            get { return _IsValueChangedByChild; }
            set { _IsValueChangedByChild = value; }
        }
        #endregion

        #region Public Methods
        ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>   Set tag initial value </summary>
        ///
        /// <param name="initiaValue" type="object">    Initial value </param>
        ////////////////////////////////////////////////////////////////////////////////////////////////////
        private void SetInitialValue(object initiaValue)
        {
            SetValue(initiaValue);
            SetWriteVal(initiaValue);
        }
        
        ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary> Value is set by child job</summary>
        ///
        /// <param name="value" type="object"> new value </param>
        ////////////////////////////////////////////////////////////////////////////////////////////////////
        public void SetValueFromChild(object value)
        {                       
            _IsValueChangedByChild = true;
            SetValue(value);            
        }

        /// <summary>
        /// <summary> Init tag internal value in of atomic struct child before job exectuion</summary>
        /// </summary>
        public virtual void SetInitialValueChild(object value)
        {
            _IsValueChangedByChild = false;
            SetValue(value);
        }

        ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary> Gets the good quality state of tag . </summary>
        ///
        /// <value> The goo quality state of tag. </value>
        ////////////////////////////////////////////////////////////////////////////////////////////////////
        public bool IsQualityGood()
        {
            return (Value != null && Value.StatusCode == StatusCodes.Good);
        }
        #endregion
    }
}
