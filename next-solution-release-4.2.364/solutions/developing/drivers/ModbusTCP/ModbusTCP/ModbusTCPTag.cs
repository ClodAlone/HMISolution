using System;
using System.Collections.Generic;
using System.Linq;
using DriverCodeBase;
using DriverBaseInterfaces;
using DriverCodeBase.Helpers;
using Opc.Ua;

namespace ModbusTCP
{


    public sealed class ModbusTCPTag : Tag
    {
        #region Constructors

        public ModbusTCPTag(TagDefinition tag, uint byteoffset, uint bitoffset)
            : base(tag, byteoffset, bitoffset)
        {

        }

        public ModbusTCPTag(TagDefinition tag)
            : base(tag)
        {

        }

        #endregion

        #region Override Properties/Functions
        // Removed to solve FOGBUZ 11694
        //public override uint GetTagBuffer(ref byte[] buffer, bool read = true, int index = 0)
        //{
        //    if (TagNode.DataType.IdType == IdType.Numeric && ((uint)TagNode.DataType.Identifier == Opc.Ua.DataTypes.Boolean))
        //    {
        //        object curVal = Value.Value;
        //        uint size = (uint)(TagNode.ArrayDimension == 0 ? 1 : TagNode.ArrayDimension / 8 + (TagNode.ArrayDimension % 8 > 0 ? 1 : 0));
        //        if (buffer != null && buffer.Length >= size)
        //        {
        //            if (TagNode.ArrayDimension == 0)
        //            {
        //                //BitConverter.GetBytes((bool)(curVal)).CopyTo(buffer, index);
        //                if ((bool)curVal)
        //                buffer[index] += (byte)(1 << (int)(ByteOffset % 8));
        //            }
        //            else
        //            {
        //                Array b = curVal as Array;
        //                if (b != null && b.GetLength(0) == TagNode.ArrayDimension)
        //                {
        //                    int j = 0;
        //                    int k = index/*0*/;
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
            bool res = ModbusTCPDynSettings.TryParse(dynamicSettings);
            if (TagNode.DataType.IdType == IdType.Numeric && (uint)TagNode.DataType.Identifier == Opc.Ua.DataTypes.String)
                Size = (uint)((ModbusTCPDynSettings.StringLength + 1) / 2) * 2;

            return res;
        }

        public override DynTagSettings DynSettings
        {
            get { return (DynTagSettings)ModbusTCPDynSettings; }
        }

        #endregion
        #region Methods
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
                                Array.Copy(buffer, index, tbuf, 0, iCheckCharacter);
                                return base.SetTagValue(ref tbuf, 0, forceUpdate, elemsize, buffertype);
                            }
                            else
                            {
                                System.Text.UTF8Encoding enc = new System.Text.UTF8Encoding();
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
                                        if (buffer[(nOffset + iCheckCharacter + index)] == 0x00)
                                        {
                                            break;
                                        }
                                    }
                                    tbuf = new byte[iCheckCharacter];
                                    Array.Copy(buffer, index + nOffset, tbuf, 0, iCheckCharacter);
                                    a[i] = enc.GetString(tbuf);
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

        #endregion

        #region Properties

        readonly ModbusTCPDynTagSettings _ModbusTCPDynSettings = new ModbusTCPDynTagSettings();
        public ModbusTCPDynTagSettings ModbusTCPDynSettings
        {
            get { return _ModbusTCPDynSettings; }
        }

        #endregion        

    }
}
