using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DriverCodeBaseEx;
using DriverBaseInterfaces;
using DriverCodeBaseEx.Helpers;
using Opc.Ua;

namespace PubNub
{


    public sealed class PubNubTag : Tag
    {
        #region Constructors

        public PubNubTag(TagDefinition tag, uint byteoffset, uint bitoffset)
            : base(tag, byteoffset, bitoffset)
        {

        }

        public PubNubTag(TagDefinition tag)
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
            return PubNubDynSettings.TryParse(dynamicSettings);
        }

        public override DynTagSettings DynSettings
        {
            get { return (DynTagSettings)PubNubDynSettings; }
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
                object readValue = (forceUpdate ? null : GetValue());
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
                                bCopy = (readValue == null || (string)readValue != (string)val); // || (LastValue != null && (string)LastValue != (string)val));
                            }
                            else
                            {
                                return(base.SetTagValue(ref buffer, index, forceUpdate, elemsize, buffertype));
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

        #endregion

        #region Properties

        readonly PubNubDynTagSettings _PubNubDynSettings = new PubNubDynTagSettings();
        public PubNubDynTagSettings PubNubDynSettings
        {
            get { return _PubNubDynSettings; }
        }

        #endregion        

    }
}
