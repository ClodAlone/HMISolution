using System;
using System.Collections.Generic;
using System.Linq;
using DriverCodeBaseEx;
using DriverBaseInterfaces;
using DriverCodeBaseEx.Helpers;
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
                bool bCopy = false;
                object val = new object();
                uint nType = (uint)TagNode.DataType.Identifier;
                if (forceUpdate)
                    ReadValue = null;
                uint size = 0;
                switch (nType)
                {
                    case (uint)Opc.Ua.DataTypes.String:
                        {
                            byte[] tbuf;
                            uint iCheckCharacter = 0;
                            if (TagNode.ArrayDimension == 0)
                            {
                                if (Size > buffer.Length)
                                    size = (uint)buffer.Length;
                                else
                                    size = Size;
                                for (iCheckCharacter = 0; iCheckCharacter < size; iCheckCharacter++)
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

                                if (Size > buffer.Length)
                                    size = (uint)buffer.Length;
                                else
                                    size = Size;
                                int elemlen = (int)(size / TagNode.ArrayDimension);

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
