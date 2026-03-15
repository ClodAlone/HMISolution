using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DriverCodeBase;
using DriverBaseInterfaces;
using Opc.Ua;
using UFUAModel.Extensions;

namespace BrPvi
{
    public sealed class BrPviTag : Tag
    {
        #region Constructors

        public BrPviTag(TagDefinition tag, uint byteoffset, uint bitoffset)
            : base(tag, byteoffset, bitoffset)
        {

        }

        public BrPviTag(TagDefinition tag)
            : base(tag)
        {

        }

        #endregion

        #region Override Properties/Functions

        public override bool BuildDynamicSettings(String dynamicSettings)
        {
            bool res = BrPviDynSettings.TryParse(dynamicSettings);
            if (TagNode.DataType.IdType == IdType.Numeric && (uint)TagNode.DataType.Identifier == Opc.Ua.DataTypes.String)
            {
                Size = (uint)BrPviDynSettings.BrPviArrayLength;
            }

            return res;
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
                            Object enc;
                            if(BrPviDynSettings.BrPviStringType == (uint)BrPviDynTagSettings.EnStringType.WString)
                            {
                                enc = new System.Text.UnicodeEncoding();
                            }
                            else
                            {
                                enc = new System.Text.UTF8Encoding();
                            }
                            byte[] tbuf;
                            uint iCheckCharacter = 0;
                            if (TagNode.ArrayDimension == 0)
                            {
                                if (Size > buffer.Length)
                                { 
                                    iCheckCharacter = (uint)buffer.Length;
                                }
                                else
                                {
                                    iCheckCharacter = Size;
                                }

                                if (BrPviDynSettings.BrPviStringType == (uint)BrPviDynTagSettings.EnStringType.WString)
                                {
                                    if ((buffer[(iCheckCharacter - 1)] == 0x00) && (buffer[((iCheckCharacter - 2))] == 0x00))
                                    {
                                        //at the end of the string there must be only two null characters for unicode strings and not three,
                                        //because they appear strange characters in the visualization.
                                        if (iCheckCharacter >= 3)
                                        {
                                            if (buffer[(iCheckCharacter - 3)] == 0x00)
                                            {
                                                iCheckCharacter -= 2;
                                            }
                                            else
                                            {
                                                iCheckCharacter -= 1;
                                            }
                                        }
                                        else
                                        {
                                            iCheckCharacter = 0;
                                        }
                                    }
                                }
                                else
                                {
                                    if (iCheckCharacter > 0)
                                        iCheckCharacter -= 1;
                                     else
                                        iCheckCharacter = 0;
                                }

                                tbuf = new byte[iCheckCharacter];
                                Array.Copy(buffer, index, tbuf, 0, iCheckCharacter);
                                if (BrPviDynSettings.BrPviStringType == (uint)BrPviDynTagSettings.EnStringType.WString)
                                {
                                    val = ((System.Text.UnicodeEncoding)enc).GetString(tbuf);
                                }
                                else
                                {
                                    val = ((System.Text.UTF8Encoding)enc).GetString(tbuf);
                                }
                                bCopy = (ReadValue == null || (string)ReadValue != (string)val) || (LastValue != null && (string)LastValue != (string)val);
                                if (bCopy)
                                {
                                    ReadValue = val;
                                    WriteVal = val;
                                    Value.Value = val;
                                    LastValue = Value.Value;
                                    return true;
                                }
                                //return base.SetTagValue(ref tbuf, 0, forceUpdate, elemsize, buffertype);
                            }
                            else
                            {
                                //System.Text.UTF8Encoding enc = new System.Text.UTF8Encoding();
                                uint nOffset = 0;
                                string[] a = new string[TagNode.ArrayDimension];
                                Array b = ReadValue as Array;
                                bool check = (b != null && b.GetLength(0) == TagNode.ArrayDimension);
                                Array bL = LastValue as Array;
                                bool checkL = (bL != null && bL.GetLength(0) == TagNode.ArrayDimension);
                                bCopy = (!check || !checkL);
                                int elemlen = (int)(Size / TagNode.ArrayDimension);

                                for (int i = 0; i < TagNode.ArrayDimension; i++)
                                {
                                    nOffset = (uint)(i * elemlen);
                                    for (iCheckCharacter = 0; iCheckCharacter < elemlen; iCheckCharacter++)
                                    {
                                        if (BrPviDynSettings.BrPviStringType == (uint)BrPviDynTagSettings.EnStringType.WString)
                                        {
                                            iCheckCharacter++;
                                            if ((buffer[(iCheckCharacter + nOffset)] == 0x00) &&
                                                (buffer[(nOffset + (iCheckCharacter - 1))] == 0x00))
                                            {                                               
                                                if (iCheckCharacter > 1)
                                                { 
                                                    iCheckCharacter -= 1;
                                                }
                                                else
                                                {
                                                    iCheckCharacter = 0;
                                                }
                                                break;
                                            }
                                        }
                                        else
                                        {
                                            if (buffer[(nOffset + iCheckCharacter)] == 0x00)
                                            {
                                                break;
                                            }
                                        }
                                    }
                                    tbuf = new byte[iCheckCharacter];
                                    Array.Copy(buffer, index + nOffset, tbuf, 0, iCheckCharacter);
                                    if (BrPviDynSettings.BrPviStringType == (uint)BrPviDynTagSettings.EnStringType.WString)
                                    {
                                        a[i] = ((System.Text.UnicodeEncoding)enc).GetString(tbuf);
                                    }
                                    else
                                    {
                                        a[i] = ((System.Text.UTF8Encoding)enc).GetString(tbuf);
                                    }
                                    //a[i] = enc.GetString(tbuf);
                                    if (check && a[i] != (string)b.GetValue(i) || (checkL && a[i] != (string)bL.GetValue(i)))
                                        bCopy = true;
                                }
                                val = a;
                            }
                            if (ReadValue == null || bCopy)
                            {
                                WriteVal = ReadValue = val;
                                Value.Value = val;
                                LastValue = Value.Value;
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

        public override DynTagSettings DynSettings
        {
            get { return (DynTagSettings)BrPviDynSettings; }
        }

        public override uint GetTagBuffer(ref byte[] buffer, bool read = true, int index = 0, uint elemsize = 0, uint buffertype = 0)
        {
            object curVal = Value.Value;
            uint size = 0;
            if (TagNode.DataType.IdType == IdType.Numeric &&
                (uint)TagNode.DataType.Identifier == (uint)Opc.Ua.DataTypes.Boolean)
            {
                size = 1;
                if (buffer != null && buffer.Length >= size)
                {
                    if (TagNode.ArrayDimension == 0)
                        BitConverter.GetBytes((bool)DataTypeExtensions.ChangeType(curVal, UFUAModel.DataType.Boolean)).CopyTo(buffer, index);
                    else
                    {
                        Array b = curVal as Array;
                        if (b != null && b.GetLength(0) == TagNode.ArrayDimension)
                        {
                            if (WriteVal == null)
                            {
                                WriteVal = b.Clone();
                                for (int i = 0; i < TagNode.ArrayDimension; i++)
                                {
                                    (WriteVal as Array).SetValue(!(bool)b.GetValue(i), i);
                                }
                            }
                            for (int i = 0; i < TagNode.ArrayDimension; i++)
                            {
                                // Copy one bit per byte
                                buffer[i] = ((bool)(b.GetValue(i)) ? (byte)1 : (byte)0);
                            }
                        }
                        size = TagNode.ArrayDimension;
                    }
                }
            }
            else
            {
                switch ((uint)TagNode.DataType.Identifier)
                {
                    case (uint)Opc.Ua.DataTypes.String:
                        {
                            size = (TagNode.ArrayDimension == 0 ? 1 : TagNode.ArrayDimension);
                            if (buffer != null && buffer.Length >= size)
                            {
                                if (BrPviDynSettings.BrPviStringType == (uint)BrPviDynTagSettings.EnStringType.WString)
                                {
                                    System.Text.UnicodeEncoding enc = new System.Text.UnicodeEncoding();
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
                                                int count = enc.GetByteCount((string)b.GetValue(i));
                                                if (buffer != null && buffer.Length > 0 && b.GetValue(i) != null && count > 0)
                                                {
                                                    int min = System.Math.Min(count, elemlen);
                                                    Array.Copy(enc.GetBytes((string)b.GetValue(i)), 0, buffer, index + (i * elemlen), min);
                                                    size += (uint)elemlen;
                                                }
                                            }
                                        }
                                    }
                                }
                                else
                                {
                                    System.Text.UTF8Encoding enc = new System.Text.UTF8Encoding();
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
                                                int count = enc.GetByteCount((string)b.GetValue(i));
                                                if (buffer != null && buffer.Length > 0 && b.GetValue(i) != null && count > 0)
                                                {
                                                    int min = System.Math.Min(count, elemlen);
                                                    Array.Copy(enc.GetBytes((string)b.GetValue(i)), 0, buffer, index + (i * elemlen), min);
                                                    size += (uint)elemlen;
                                                }
                                            }
                                        }
                                    }
                                }
                                //System.Text.UTF8Encoding enc = new UTF8Encoding();
                                //if (TagNode.ArrayDimension == 0)
                                //{
                                //    string sVal = curVal as string;
                                //    int count = enc.GetByteCount(sVal);
                                //    if (buffer != null && buffer.Length > 0 && sVal != null && count > 0)
                                //    {
                                //        int min = System.Math.Min(count, buffer.Length);
                                //        Array.Copy(enc.GetBytes(sVal), 0, buffer, index, min);
                                //        size = (uint)min;
                                //    }
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
                                //                size += (uint)elemlen;
                                //            }
                                //        }
                                //    }
                                //}
                            }
                        }
                        break;
                    default:
                        size = base.GetTagBuffer(ref buffer, read, index, elemsize, buffertype);
                        break;
                }
            }

            return size;
        }

        #endregion

        #region Properties

        private readonly BrPviDynTagSettings _BrPviDynSettings = new BrPviDynTagSettings();
        public BrPviDynTagSettings BrPviDynSettings
        {
            get { return _BrPviDynSettings; }
        }

        #endregion
    }
}
