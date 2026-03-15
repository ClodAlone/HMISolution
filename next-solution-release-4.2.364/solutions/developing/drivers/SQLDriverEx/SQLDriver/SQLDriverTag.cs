using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DriverCodeBaseEx;
using DriverBaseInterfaces;
using DriverCodeBaseEx.Helpers;
using Opc.Ua;

namespace SQLDriver
{


    public sealed class SQLDriverTag : Tag
    {
        #region Constructors

        public SQLDriverTag(TagDefinition tag, uint byteoffset, uint bitoffset)
            : base(tag, byteoffset, bitoffset)
        {

        }

        public SQLDriverTag(TagDefinition tag)
            : base(tag)
        {

        }

        #endregion

        #region Override Properties/Functions

        public override bool BuildDynamicSettings(String dynamicSettings)
        {
            bool res = SQLDriverDynSettings.TryParse(dynamicSettings);
            //if (TagNode.DataType.IdType == IdType.Numeric && (uint)TagNode.DataType.Identifier == Opc.Ua.DataTypes.String)
            //{
            //    Size = (uint)250;
            //}
            return res;
        }

        public override DynTagSettings DynSettings
        {
            get { return (DynTagSettings)SQLDriverDynSettings; }
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
                            byte[] tbuf;
                            uint iCheckCharacter = 0;
                            if (TagNode.ArrayDimension == 0)
                            {
                                for (iCheckCharacter = 0; iCheckCharacter < buffer.Length; iCheckCharacter++)
                                {
                                    if (buffer[(iCheckCharacter + index)] == 0x00)
                                    {
                                        break;
                                    }
                                }
                                Size = iCheckCharacter;
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
                                bCopy = !check;
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

        readonly SQLDriverDynTagSettings _SQLDriverDynSettings = new SQLDriverDynTagSettings();
        public SQLDriverDynTagSettings SQLDriverDynSettings
        {
            get { return _SQLDriverDynSettings; }
        }

        #endregion        

    }
}
