using System;
using DriverCodeBaseEx;
using DriverBaseInterfaces;
using Opc.Ua;
using System.Text;

namespace PhoenixContactPLCI
{
    public sealed class PhoenixContactPLCITag : Tag
    {
        #region Constructors

        public PhoenixContactPLCITag(TagDefinition tag, uint byteoffset, uint bitoffset)
            : base(tag, byteoffset, bitoffset)
        {

        }

        public PhoenixContactPLCITag(TagDefinition tag)
            : base(tag)
        {

        }

        #endregion

        #region Override Properties/Functions

        public override bool BuildDynamicSettings(String dynamicSettings)
        {
            bool res = PhoenixContactPLCIDynSettings.TryParse(dynamicSettings);
            if (TagNode.DataType.IdType == IdType.Numeric && (uint)TagNode.DataType.Identifier == Opc.Ua.DataTypes.String)
                Size = (uint)PhoenixContactPLCIDynSettings.StringLength;

            return res;
        }

        public override DynTagSettings DynSettings
        {
            get { return (DynTagSettings)PhoenixContactPLCIDynSettings; }
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
        public override bool SetTagValue(ref byte[] buffer, int index, bool forceUpdate, uint elemsize = 0, uint buffertype = 0)
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
                    default:
                        return (base.SetTagValue(ref buffer, index, forceUpdate, elemsize, buffertype));
                        break;
                }                
            }
            return (false);
        }

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
        public override uint GetTagBuffer(ref byte[] buffer, bool read = true, int index = 0, uint elemsize = 0, uint buffertype = 0)
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
                                        int elemlen = (int)(buffer.Length / TagNode.ArrayDimension);                                 

                                        for (int i = 0; i < TagNode.ArrayDimension; i++)
                                        {
                                            int count = (b.GetValue(i) == null ? 0 : enc.GetByteCount((string)b.GetValue(i)));
                                            if (buffer != null && buffer.Length > 0 && count > 0)
                                            {
                                                int min = System.Math.Min(count, elemlen);
                                                Array.Copy(enc.GetBytes((string)b.GetValue(i)), 0, buffer, index + (i * elemlen), min);
                                                //size += (uint)elemlen;
                                            }
                                        }
                                        size = (uint)buffer.Length;
                                    }
                                }
                            }
                            return size;
                        }
                        break;
                    default:
                        break;
                }
            }
            return base.GetTagBuffer(ref buffer, read, index, elemsize, buffertype);
        }

        #endregion

        #region Properties

        readonly PhoenixContactPLCIDynTagSettings _PhoenixContactPLCIDynSettings = new PhoenixContactPLCIDynTagSettings();
        public PhoenixContactPLCIDynTagSettings PhoenixContactPLCIDynSettings
        {
            get { return _PhoenixContactPLCIDynSettings; }
        }

        #endregion

    }
}
