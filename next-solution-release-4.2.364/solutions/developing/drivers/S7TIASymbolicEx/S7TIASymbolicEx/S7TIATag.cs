using System;
using System.Collections.Generic;
using System.Linq;
using DriverCodeBaseEx;
using DriverBaseInterfaces;
using Opc.Ua;
using UFUAModel.Extensions;

namespace S7TIASymbolic
{
    public sealed class S7TIATag : Tag
    {
        #region Constructors

        public S7TIATag(TagDefinition tag, uint byteoffset, uint bitoffset)
            : base(tag, byteoffset, bitoffset)
        {

        }

        public S7TIATag(TagDefinition tag)
            : base(tag)
        {
            
        }

        #endregion

        #region Override Properties/Functions

        public override bool BuildDynamicSettings(String dynamicSettings)
        {
            bool res = S7TIADynTagSettings.TryParse(dynamicSettings);
            if (TagNode.DataType.IdType == IdType.Numeric && (uint)TagNode.DataType.Identifier == Opc.Ua.DataTypes.String)
            {
                Size = (uint)S7TIADynTagSettings.StringLength;
            }
            return res;
        }

        public override DynTagSettings DynSettings
        {
            get { return (DynTagSettings)S7TIADynTagSettings; }
        }

        public override uint GetTagBuffer(ref byte[] buffer, bool read = true, int index = 0, uint elemsize = 0, uint buffertype = 0)
        {
            object curVal = Value.Value;
            uint size = 0;
            if (TagNode.DataType.IdType == IdType.Numeric)
            {
                switch ((uint)TagNode.DataType.Identifier)
                {
                    case (uint)Opc.Ua.DataTypes.Boolean:
                        size = 1;
                        if (buffer != null && buffer.Length >= size)
                        {
                            if (TagNode.ArrayDimension == 0)
                            {
                                BitConverter.GetBytes((bool)DataTypeExtensions.ChangeType(curVal, UFUAModel.DataType.Boolean)).CopyTo(buffer, index);
                            }
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
                                        //write one bit at a time
                                        buffer[i] = ((bool)(b.GetValue(i)) ? (byte)1 : (byte)0);
                                    }
                                }
                                size = TagNode.ArrayDimension;
                            }
                        }
                        break;
                    case (uint)Opc.Ua.DataTypes.String:
                        {
                            size = (TagNode.ArrayDimension == 0 ? 1 : TagNode.ArrayDimension);
                            if (buffer != null && buffer.Length >= size)
                            {
                                System.Text.UTF8Encoding enc = new System.Text.UTF8Encoding();
                                if (TagNode.ArrayDimension == 0)
                                {
                                    size = base.GetTagBuffer(ref buffer, read, index, elemsize, buffertype);
                                }
                                else
                                {
                                    Array b = curVal as Array;
                                    if (b != null && b.GetLength(0) == TagNode.ArrayDimension)
                                    {
                                        int elemlen = (int)(Size / TagNode.ArrayDimension);
                                        for (int i = 0; i < TagNode.ArrayDimension; i++)
                                        {
                                            int count = 0;
                                            if (b.GetValue(i) != null)
                                            {
                                                count = enc.GetByteCount((string)b.GetValue(i));
                                            }
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
                        }
                        break;
                    default:
                        size = base.GetTagBuffer(ref buffer, read, index, elemsize, buffertype);
                        break;
                }
            }
            else
            {
                size = base.GetTagBuffer(ref buffer, read, index, elemsize, buffertype);
            }

            return size;
        }

        public void GetTagBufferWString(ref object buffer)
        {
            object curVal = null;

            if (TagNode.DataType.IdType == IdType.Numeric)
            {
                switch ((uint)TagNode.DataType.Identifier)
                {
                    case (uint)Opc.Ua.DataTypes.String:
                        {
                            if (TagNode.ArrayDimension == 0)
                            {
                                curVal = Value.Value;
                            }
                            else
                            {
                                Array b = Value.Value as Array;
                                if (b != null && b.GetLength(0) == TagNode.ArrayDimension)
                                {
                                    curVal = Value.Value;
                                }
                            }
                        }
                        break;
                }
            }

            buffer = curVal;
        }
        #endregion

        #region Methods
        /// <summary>   The read value. </summary>
        public override bool SetTagValue(ref byte[] buffer, int index, bool forceUpdate, uint elemsize = 0, uint buffertype = 0)
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
                           System.Text.UTF8Encoding enc = new System.Text.UTF8Encoding();
                            if (TagNode.ArrayDimension == 0)
                            {
                                uint ssize = 0;
                                switch (_S7StringDataFormat)
                                {
                                    case S7DataFormats.String:
                                        {
                                            if (Size > buffer.Length)
                                                ssize = (uint)buffer.Length;
                                            else
                                                ssize = Size;
                                            byte[] tbuf = new byte[ssize];
                                            Array.Copy(buffer, index, tbuf, 0, ssize);
                                            val = new System.Text.UTF8Encoding().GetString(tbuf);
                                            break;
                                        }
                                    case S7DataFormats.WString:
                                        {
                                            if (Size * 2 > buffer.Length)
                                                ssize = (uint)buffer.Length;
                                            else
                                                ssize = Size * 2;
                                            byte[] tbuf = new byte[ssize];
                                            Array.Copy(buffer, index, tbuf, 0, ssize);
                                            val = new System.Text.UTF8Encoding().GetString(System.Text.Encoding.Convert(System.Text.Encoding.Unicode, System.Text.Encoding.UTF8, tbuf));
                                            break;
                                        }
                                }

                                bCopy = (ReadValue == null || (string)ReadValue != (string)val) || (LastValue != null && (string)LastValue != (string)val);
                            }
                            else
                            {
                                string[] a = new string[TagNode.ArrayDimension];
                                Array b = ReadValue as Array;
                                bool check = (b != null && b.GetLength(0) == TagNode.ArrayDimension);
                                bCopy = !check;

                                int ssize = (int)(Size / TagNode.ArrayDimension);

                                for (int i = 0; i < TagNode.ArrayDimension; i++)
                                {
                                    switch (_S7StringDataFormat)
                                    {
                                        case S7DataFormats.String:
                                            {
                                                a[i] = new System.Text.UTF8Encoding().GetString(buffer, (i * ssize), ssize).Trim('\0');
                                                break;
                                            }
                                        case S7DataFormats.WString:
                                            {
                                                string uni = new System.Text.UnicodeEncoding().GetString(buffer, (i * (ssize * 2)), ssize * 2).Trim('\0');
                                                a[i] = new System.Text.UTF8Encoding().GetString(System.Text.Encoding.Convert(System.Text.Encoding.Unicode, System.Text.Encoding.UTF8, new System.Text.UnicodeEncoding().GetBytes(uni)));
                                                break;
                                            }
                                    }
                                    if (check && a[i] != (string)b.GetValue(i))
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
                    case (uint)Opc.Ua.DataTypes.Float:
                        {
                            uint size = 0;
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
                                        val = (Single)nbuf[index];
                                    else if (elemsize == 2)
                                        val = (Single)BitConverter.ToInt16(nbuf, index);
                                    else if (elemsize == 4)
                                        val = (Single)BitConverter.ToInt32(nbuf, index);
                                    else if (elemsize == 8)
                                        val = (Single)BitConverter.ToInt64(nbuf, index);
                                    else
                                    {                                                                 
                                        if(nbuf.Length == 8)
                                        {
                                            Double tmpDouble = 0.0;
                                            tmpDouble = BitConverter.ToDouble(nbuf, index);
                                            val = Convert.ToSingle(tmpDouble);
                                        }
                                        else
                                        {
                                            val = BitConverter.ToSingle(nbuf, index);
                                        }                                        
                                    }
                                    bCopy = (ReadValue == null || (float)ReadValue != (float)val);
                                }
                                else
                                {
                                    uint ncheckLengthBffer = TagNode.ArrayDimension * 8;
                                    bool bIsArrayOfDoouble = (ncheckLengthBffer == nbuf.Length);
                                    Double tmpDouble = 0.0;
                                    float[] a = new float[TagNode.ArrayDimension];
                                    Array b = ReadValue/*Value.Value*/ as Array;
                                    bool check = (b != null && b.GetLength(0) == TagNode.ArrayDimension);
                                    bCopy = !check;
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
                                        {
                                            if(bIsArrayOfDoouble)
                                            {
                                                tmpDouble = BitConverter.ToDouble(nbuf, index + 8 * i);
                                                a[i] = Convert.ToSingle(tmpDouble);
                                            }
                                            else
                                            {
                                                a[i] = BitConverter.ToSingle(nbuf, index + 4 * i);
                                            }                                            
                                        }
                                        if (check && a[i] != (float)b.GetValue(i))
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
                        }
                        break;
                    default:
                        {
                            return base.SetTagValue(ref buffer, index, forceUpdate, elemsize, buffertype);
                        }
                        break;
                }
               
            }
            return (false);
        }

        #endregion

        #region Properties

        private readonly S7TIADynTagSettings _S7TIADynTagSettings = new S7TIADynTagSettings();
        public S7TIADynTagSettings S7TIADynTagSettings
        {
            get { return _S7TIADynTagSettings; }
        }

        private S7DataFormats _S7StringDataFormat;
        public S7DataFormats S7StringDataFormat
        {
            get { return _S7StringDataFormat; }
            set { _S7StringDataFormat = value; }
        }
        #endregion      
    }
}
