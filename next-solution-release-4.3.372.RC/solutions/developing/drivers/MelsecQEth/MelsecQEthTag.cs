using System;
using System.Collections.Generic;
using System.Linq;
using DriverCodeBase;
using DriverBaseInterfaces;
using DriverCodeBase.Helpers;
using Opc.Ua;

namespace MelsecQEth
{
    public sealed class MelsecQEthTag : Tag
    {
        public byte[] MemRWTag;
        #region Constructors

        public MelsecQEthTag(TagDefinition tag, uint byteoffset, uint bitoffset)
            : base(tag, byteoffset, bitoffset)
        {
            bool res = MelsecQEthDynSettings.TryParse(TagNode.DynamicSettings);
            if (TagNode.DataType.IdType == IdType.Numeric && (uint)TagNode.DataType.Identifier == Opc.Ua.DataTypes.String)
            {
                if (MelsecQEthDynSettings.UnicodeString == true)
                {
                    IsUnicodeString = true;
                }
                else
                {
                    IsUnicodeString = false;
                }
            }
        }

        public MelsecQEthTag(TagDefinition tag)
            : base(tag)
        {
            bool res = MelsecQEthDynSettings.TryParse(TagNode.DynamicSettings);
            if (TagNode.DataType.IdType == IdType.Numeric && (uint)TagNode.DataType.Identifier == Opc.Ua.DataTypes.String)
            {
                if (MelsecQEthDynSettings.UnicodeString == true)
                {
                    IsUnicodeString = true;
                }
                else
                {
                    IsUnicodeString = false;
                }
            }
        }

        #endregion


        private bool _IsUnicodeString;
        ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>   Gets or sets the size. </summary>
        ///
        /// <value> The size. </value>
        ////////////////////////////////////////////////////////////////////////////////////////////////////
        public bool IsUnicodeString
        {
            get { return _IsUnicodeString; }
            set
            {
                _IsUnicodeString = value;
            }
        }

        #region Override Properties/Functions
        //public override uint GetTagBuffer(ref byte[] buffer, bool read = true, int index = 0)
        //{
        //    if (TagNode.DataType.IdType == IdType.Numeric && ((uint)TagNode.DataType.Identifier == Opc.Ua.DataTypes.Boolean))
        //    {
        //        object curVal = Value.Value;
        //        uint size = (uint)(TagNode.ArrayDimension == 0 ? 1 : TagNode.ArrayDimension / 8 + (TagNode.ArrayDimension % 8 > 0 ? 1 : 0));
        //        if (buffer != null && buffer.Length >= size)
        //        {
        //            if (TagNode.ArrayDimension == 0)
        //                BitConverter.GetBytes((bool)(curVal)).CopyTo(buffer, index);
        //            else
        //            {
        //                Array b = curVal as Array;
        //                if (b != null && b.GetLength(0) == TagNode.ArrayDimension)
        //                {
        //                    int j = 0;
        //                    int k = 0;
        //                    for (int i = 0; i < TagNode.ArrayDimension; i++)
        //                    {
        //                        if ((bool)b.GetValue(i))
        //                            buffer[k] += (byte)System.Math.Pow(2.0, j);
        //                        if (++j > 7)
        //                        {
        //                            j = 0;
        //                            k++;
        //                        }
        //                    }
        //                }
        //            }
        //        }
        //        return size;
        //    }
        //    else
        //        return base.GetTagBuffer(ref buffer, read, index);
        //}

        public override bool BuildDynamicSettings(String dynamicSettings)
        {
            bool res = MelsecQEthDynSettings.TryParse(dynamicSettings);
            if (TagNode.DataType.IdType == IdType.Numeric && (uint)TagNode.DataType.Identifier == Opc.Ua.DataTypes.String)
            {
                if (MelsecQEthDynSettings.UnicodeString == true)
                {
                    Size = (uint)((MelsecQEthDynSettings.StringLength)) * 2;
                }
                else
                {
                    Size = (uint)((MelsecQEthDynSettings.StringLength));
                }
            }
            return (res);
        }

        public override DynTagSettings DynSettings
        {
            get { return (DynTagSettings)MelsecQEthDynSettings; }
        }

        public override uint GetTagBuffer(ref byte[] buffer, bool read = true, int index = 0, uint elemsize = 0, uint buffertype = 0)
        {
            object curVal = Value.Value;
            uint size = 0;
            if (TagNode.DataType.IdType == IdType.Numeric)
            {
                uint nType = (uint)TagNode.DataType.Identifier;
                switch (nType)
                {
                    case (uint)Opc.Ua.DataTypes.String:
                        {
                            // String arrays are not supported
                            //size = (TagNode.ArrayDimension == 0 ? 1 : TagNode.ArrayDimension);
                            if(TagNode.ArrayDimension == 0)
                            {
                                size = 1;
                                if (buffer != null && buffer.Length >= size)
                                {
                                    // Select the right encoding for the string
                                    System.Text.Encoding enc;
                                    if (!IsUnicodeString)
                                    {
                                        enc = new System.Text.UTF8Encoding();
                                    }
                                    else
                                    {
                                        enc = new System.Text.UnicodeEncoding();
                                    }

                                    //if (TagNode.ArrayDimension == 0)
                                    //{
                                    int count = 0;
                                    string sVal = curVal as string;
                                    if (sVal != null)
                                    {
                                        count = enc.GetByteCount(sVal);
                                        //if (buffer != null && buffer.Length > 0 && sVal != null && count > 0)
                                        if (count > 0)
                                        {
                                            int min = System.Math.Min(count, buffer.Length);
                                            // The parameter index is intentionally not used (it is always zero: there are only data bytes in buffer)
                                            Array.Copy(enc.GetBytes(sVal), 0, buffer, 0, min);
                                            size = (uint)min;
                                        }
                                    }
                                    //}
                                    //else
                                    //{
                                    //    Array b = curVal as Array;
                                    //    if (b != null && b.GetLength(0) == TagNode.ArrayDimension)
                                    //    {
                                    //        int elemlen = (int)(Size / TagNode.ArrayDimension);
                                    //        for (int i = 0; i < TagNode.ArrayDimension; i++)
                                    //        {
                                    //            int count = enc.GetByteCount((string)b.GetValue(i));
                                    //            if (buffer != null && buffer.Length > 0 && b.GetValue(i) != null && count > 0)
                                    //            {
                                    //                int min = System.Math.Min(count, elemlen);
                                    //                Array.Copy(enc.GetBytes((string)b.GetValue(i)), 0, buffer, index + (i * elemlen), min);
                                    //                if (min < elemlen)
                                    //                {
                                    //                    for (int k = 0; k < (elemlen - min); k++)
                                    //                        Array.Copy(enc.GetBytes((string)" "), 0, buffer, index + (i * elemlen) + min + k, 1);
                                    //                }
                                    //                size += (uint)elemlen;
                                    //            }
                                    //        }
                                    //    }
                                    //}
                                }
                            }
                        }
                        break;
                    default:
                        size = base.GetTagBuffer(ref buffer, read, index, elemsize, buffertype);
                        break;
                }
                if (!(buffer != null && buffer.Length >= size))
                    size = 0;
            }
            return (size);
        }

        #endregion

        #region Properties

        readonly MelsecQEthDynTagSettings _MelsecQEthDynSettings = new MelsecQEthDynTagSettings();
        public MelsecQEthDynTagSettings MelsecQEthDynSettings
        {
            get { return _MelsecQEthDynSettings; }
        }

        #endregion 
       
        # region Mothod
        public bool setMemRWTag(byte[] buffer, int index, int Length)
        {
            if (MemRWTag == null)
                MemRWTag = new byte[Length];
            if (Length > MemRWTag.Length)
                return false;
            Array.Copy(buffer, index, MemRWTag, 0, Length);
            return true;
        }
  
        /// <summary>   The read value. </summary>
        public override bool SetTagValue(ref byte[] buffer, int index, uint elemsize = 0, uint buffertype = 0, bool forceUpdate = false)
        {
            if (TagNode.DataType.IdType == IdType.Numeric)
            {
                if (Value.StatusCode.Code != StatusCodes.Good)
                    forceUpdate = true;

                bool bCopy = false;
                object val = new object();
                uint nType = (uint)TagNode.DataType.Identifier;
                if (forceUpdate)
                    ReadValue = null;
                switch (nType)
                {
                    case (uint)Opc.Ua.DataTypes.String:
                        {
                            // The parameter index is used for aggregations
                            byte[] tbuf;
                            uint stringByteLength = 0;
                            uint maxIndex = (uint)buffer.Length;
                            if(maxIndex < (Size + index))
                            {
                                return (false);
                            }
                            
                            // String arrays are not supported
                            if (TagNode.ArrayDimension == 0)
                            {
                                // Remove the ending null characters from the received string
                                if (!IsUnicodeString) // Standard string
                                {                                 
                                    for (stringByteLength = 0; (stringByteLength + index) < maxIndex; stringByteLength++)
                                    {
                                        if (buffer[(stringByteLength + index)] == 0x00)
                                        {
                                            break;
                                        }
                                    }

                                    // tbuf is a buffer with only non null characters
                                    tbuf = new byte[stringByteLength];
                                    Array.Copy(buffer, index, tbuf, 0, stringByteLength);

                                    // Call the base class method for updating the tag value
                                    return base.SetTagValue(ref tbuf, 0, forceUpdate, elemsize, buffertype);
                                }
                                else // Unicode string
                                {
                                    for (stringByteLength = 0; (stringByteLength + index + 1) < maxIndex; stringByteLength += 2)
                                    {
                                        if ((buffer[stringByteLength + index] == 0x00) && (buffer[(stringByteLength + index + 1)] == 0x00))
                                        {
                                            break;                                           
                                        }
                                    }

                                    // tbuf is a buffer with only non null characters
                                    tbuf = new byte[stringByteLength];
                                    Array.Copy(buffer, index, tbuf, 0, stringByteLength);
 
                                    // Parse the Unicode string                                  
                                    System.Text.UnicodeEncoding enc = new System.Text.UnicodeEncoding();
                                    String decodedString = enc.GetString(tbuf);

                                    // Update the tag value
                                    bCopy = (ReadValue == null || (string)ReadValue != decodedString || (LastValue != null && (string)LastValue != decodedString));
                                    if (bCopy)
                                    {
                                        WriteVal = ReadValue = decodedString;
                                        Value.Value = decodedString;
                                        LastValue = decodedString;
                                        return true;
                                    }                                  
                                }                           
                            }
                            //else
                            //{
                            //    System.Text.UTF8Encoding enc = new System.Text.UTF8Encoding();
                            //    uint nOffset = 0;
                            //    string[] a = new string[TagNode.ArrayDimension];
                            //    Array b = ReadValue as Array;
                            //    bool check = (b != null && b.GetLength(0) == TagNode.ArrayDimension);
                            //    bCopy = !check;
                            //    int elemlen = (int)(Size / TagNode.ArrayDimension);

                            //    for (int i = 0; i < TagNode.ArrayDimension; i++)
                            //    {
                            //        nOffset = (uint)(i * elemlen);
                            //        for (iCheckCharacter = 0; iCheckCharacter < elemlen; iCheckCharacter++)
                            //        {
                            //            if (buffer[(nOffset + iCheckCharacter + index)] == 0x00)
                            //            {
                            //                break;
                            //            }
                            //        }
                            //        tbuf = new byte[iCheckCharacter];
                            //        Array.Copy(buffer, index + nOffset, tbuf, 0, iCheckCharacter);
                            //        a[i] = enc.GetString(tbuf);
                            //        if (check && a[i] != (string)b.GetValue(i))
                            //            bCopy = true;
                            //    }
                            //    val = a;
                            //}
                            //if (ReadValue == null || bCopy)
                            //{
                            //    WriteVal = ReadValue = val;
                            //    Value.Value = val;
                            //    LastValue = Value.Value;
                            //    return true;
                            //}
                        }
                        break;
                    default:
                        return base.SetTagValue(ref buffer, index, forceUpdate, elemsize, buffertype);
                }
            }
            return (false);
        }

        #endregion
    }
}
