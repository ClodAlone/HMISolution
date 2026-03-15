using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DriverCodeBase;
using DriverBaseInterfaces;
using DriverCodeBase.Helpers;
using Opc.Ua;

namespace Databoom
{


    public sealed class DataboomTag : Tag
    {
        #region Constructors

        public DataboomTag(TagDefinition tag, uint byteoffset, uint bitoffset)
            : base(tag, byteoffset, bitoffset)
        {

        }

        public DataboomTag(TagDefinition tag)
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
            return DataboomDynSettings.TryParse(dynamicSettings);
        }

        public override DynTagSettings DynSettings
        {
            get { return (DynTagSettings)DataboomDynSettings; }
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
                uint size = 0;
                uint nType = (uint)TagNode.DataType.Identifier;
                if (forceUpdate)
                {
                    SetReadValue(null);
                }
                switch (nType)
                {
                    case (uint)Opc.Ua.DataTypes.String:
                        {
                            System.Text.UTF8Encoding enc = new UTF8Encoding();
                            if (TagNode.ArrayDimension == 0)
                            {
                                size = (uint)buffer.Length;
                                byte[] tbuf = new byte[size];
                                Array.Copy(buffer, index, tbuf, 0, size);
                                val = enc.GetString(tbuf);
                                bCopy = (GetReadValue() == null || (string)GetReadValue() != (string)val || (LastValue != null && (string)LastValue != (string)val));
                            }
                            else
                            {
                                return(base.SetTagValue(ref buffer, index, forceUpdate, elemsize, buffertype));
                            }
                            if (GetReadValue() == null || bCopy)
                            {
                                SetReadValue(val);
                                WriteVal = val;
                                Value.Value = val;
                                LastValue = val;
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

        readonly DataboomDynTagSettings _DataboomDynSettings = new DataboomDynTagSettings();
        public DataboomDynTagSettings DataboomDynSettings
        {
            get { return _DataboomDynSettings; }
        }

        #endregion        

    }
}
