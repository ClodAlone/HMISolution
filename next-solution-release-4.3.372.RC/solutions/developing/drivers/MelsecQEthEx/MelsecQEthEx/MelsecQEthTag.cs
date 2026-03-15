using System;
using DriverCodeBaseEx;
using DriverBaseInterfaces;
using Opc.Ua;
using DevExpress.CodeParser;
using System.Text;
using System.Linq;

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
            _AddressType = MelsecQEthDynSettings.AddressType;
            if (TagNode.DataType.IdType == IdType.Numeric && (uint)TagNode.DataType.Identifier == Opc.Ua.DataTypes.String)
                _UnicodeString = MelsecQEthDynSettings.UnicodeString;
        }

        public MelsecQEthTag(TagDefinition tag)
            : base(tag)
        {
            bool res = MelsecQEthDynSettings.TryParse(TagNode.DynamicSettings);
            _AddressType = MelsecQEthDynSettings.AddressType;
            if (TagNode.DataType.IdType == IdType.Numeric && (uint)TagNode.DataType.Identifier == Opc.Ua.DataTypes.String)
                _UnicodeString = MelsecQEthDynSettings.UnicodeString;
        }
        #endregion

        #region Override Properties/Functions
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
            object curVal = WriteVal;
            uint size = 0;
            System.Text.Encoding enc;
            if (TagNode.DataType.IdType == IdType.Numeric)
            {
                uint nType = (uint)TagNode.DataType.Identifier;
                switch (nType)
                {
                    case (uint)Opc.Ua.DataTypes.String:
                        {                            
                            if (TagNode.ArrayDimension == 0)
                            {
                                size = 1;
                                if (buffer != null && buffer.Length >= size)
                                {
                                    // Select the right encoding for the string                                    
                                    if (!_UnicodeString)                                    
                                        enc = new System.Text.UTF8Encoding();
                                    else
                                        enc = new System.Text.UnicodeEncoding();
                                    
                                    string sVal = curVal as string;
                                    if (sVal != null)
                                    {
                                        int count = enc.GetByteCount(sVal);
                                        //if (buffer != null && buffer.Length > 0 && sVal != null && count > 0)
                                        if (count > 0)
                                        {
                                            int min = System.Math.Min(count, buffer.Length);
                                            // The parameter index is intentionally not used (it is always zero: there are only data bytes in buffer)
                                            Array.Copy(enc.GetBytes(sVal), 0, buffer, 0, min);
                                            size = (uint)min;
                                        }
                                    }
                                }

                                // add string terminator \0
                                if (_AddressType == MelsecQEthProtocol.AddressTypes.Label)
                                    Array.Resize(ref buffer, (int)(Size + MelsecQEthProtocol.STRING_TERMINATOR_NULL_CHARS));
                            }
                            else
                            {
                                // string array is managed only for label data
                                if (_AddressType == MelsecQEthProtocol.AddressTypes.Label)
                                {
                                    Array b = curVal as Array;
                                    if (b != null && b.GetLength(0) == TagNode.ArrayDimension)
                                    {
                                        size = (Size + (TagNode.ArrayDimension * MelsecQEthProtocol.STRING_TERMINATOR_NULL_CHARS));
                                        Array.Resize(ref buffer, (int)size);

                                        if (!_UnicodeString)
                                            enc = new System.Text.UTF8Encoding();
                                        else
                                            enc = new System.Text.UnicodeEncoding();

                                        int elemlen = (int)(Size / TagNode.ArrayDimension);
                                        for (int i = 0; i < TagNode.ArrayDimension; i++)
                                        {
                                            if (b.GetValue(i) != null)
                                            {
                                                byte[] arrayElement = enc.GetBytes((string)b.GetValue(i));
                                                if (arrayElement.Length > elemlen)
                                                    Array.Resize(ref arrayElement, elemlen);
                                                Array.Copy(arrayElement, 0, buffer, (elemlen + MelsecQEthProtocol.STRING_TERMINATOR_NULL_CHARS) * i, arrayElement.Length);
                                            }
                                        }
                                    }
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

        private bool _UnicodeString;
        public bool UnicodeString
        {
            get { return _UnicodeString; }
            set { _UnicodeString = value; }
        }

        public MelsecQEthProtocol.AddressTypes _AddressType;
        #endregion

        #region Mothod
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
                object readValue = (forceUpdate ? null : Value.Value);
                switch (nType)
                {
                    case (uint)Opc.Ua.DataTypes.String:
                        {
                            // The parameter index is used for aggregations
                            byte[] tbuf;
                            int stringByteLength = 0;
                            uint maxIndex = (uint)buffer.Length;

                            // String arrays are not supported
                            if (TagNode.ArrayDimension == 0)
                            {
                                // Remove the ending null characters from the received string
                                if (!_UnicodeString) // Standard string
                                {
                                    for (stringByteLength = 0; (stringByteLength + index) < maxIndex; stringByteLength++)
                                    {
                                        if (buffer[(stringByteLength + index)] == 0x00)
                                        {
                                            break;
                                        }
                                    }

                                    if (stringByteLength > Size)
                                        stringByteLength = (int)Size;

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

                                    if (stringByteLength > Size)
                                        stringByteLength = (int)Size;

                                    // tbuf is a buffer with only non null characters
                                    tbuf = new byte[stringByteLength];
                                    Array.Copy(buffer, index, tbuf, 0, stringByteLength);

                                    // Parse the Unicode string                                  
                                    System.Text.UnicodeEncoding enc = new System.Text.UnicodeEncoding();
                                    String decodedString = enc.GetString(tbuf);

                                    // Update the tag value
                                    bCopy = (readValue == null || (string)readValue != decodedString);
                                    if (bCopy)
                                    {
                                        SetValue(decodedString);
                                        return true;
                                    }
                                }
                            }
                            else
                            {
                                string[] a = new string[TagNode.ArrayDimension];
                                Array b = readValue as Array;
                                bool check = (b != null && b.GetLength(0) == TagNode.ArrayDimension);
                                bCopy = (!check);
                                int elemlen = (int)(Size / TagNode.ArrayDimension);
                                for (int i = 0; i < TagNode.ArrayDimension; i++)
                                {
                                    byte[] tbufArray = buffer.Skip((elemlen + MelsecQEthProtocol.STRING_TERMINATOR_NULL_CHARS) * i).Take(elemlen).ToArray();
                                    if (!_UnicodeString) // Standard string
                                        a[i] = Encoding.UTF8.GetString(tbufArray).TrimEnd(new char[] { '\0' });
                                    else
                                        a[i] = Encoding.Unicode.GetString(tbufArray).TrimEnd(new char[] { '\0', '\0' });
                                    if (check && a[i] != Convert.ToString(b.GetValue(i)))
                                        bCopy = true;
                                }
                                val = a;

                                // Update the tag value
                                if (readValue == null || bCopy)
                                {
                                    SetValue(val);
                                    return true;
                                }
                            }
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
