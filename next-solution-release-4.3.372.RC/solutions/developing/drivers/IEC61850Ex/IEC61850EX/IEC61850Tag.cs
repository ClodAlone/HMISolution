using System;
using System.Collections.Generic;
using System.Linq;
using DriverCodeBaseEx;
using DriverBaseInterfaces;
using DriverCodeBaseEx.Helpers;
using Opc.Ua;

namespace IEC61850
{
    enum StringDefaultSizes
    {
        BitStringSize = 13,
        TimeSize = 50,
        OctetStringSize = 64,
        VisibleStringSize = 255
    }

    public sealed class IEC61850Tag : Tag
    {
        #region Constructors

        public IEC61850Tag(TagDefinition tag, uint byteoffset, uint bitoffset)
            : base(tag, byteoffset, bitoffset)
        {        
        }

        public IEC61850Tag(TagDefinition tag)
            : base(tag)
        {         
        }

        #endregion

        #region Override Properties/Functions

        public override bool BuildDynamicSettings(String dynamicSettings)
        {
            bool res = IEC61850DynSettings.TryParse(dynamicSettings);
            if (IEC61850DynSettings.DataMaximumLength > 0)
            {
                Size = (uint)((IEC61850DynSettings.DataMaximumLength + 1) / 2) * 2;
            }

            return res;
        }

        public override DynTagSettings DynSettings
        {
            get { return (DynTagSettings)IEC61850DynSettings; }
        }

        #endregion

        #region Methods
        /// <summary>   The read value. </summary>

        // Set a default size for a tag of type string, depending on the MMS type passed as argument
        public void SetDefaultStringSize(MMSDataTypes mmsType)
        {
            if ((TagNode.DataType.IdType != IdType.Numeric) || ((uint)TagNode.DataType.Identifier != (uint)Opc.Ua.DataTypes.String))
            {
                return;
            }
            switch(mmsType)
            {
                case MMSDataTypes.BitString:
                    Size = (uint)StringDefaultSizes.BitStringSize;
                    break;

                case MMSDataTypes.UTCTime:
                case MMSDataTypes.BinaryTime:
                    Size = (uint)StringDefaultSizes.TimeSize;
                    break;

                case MMSDataTypes.OctetString:
                    Size = (uint)StringDefaultSizes.OctetStringSize;
                    break;

                case MMSDataTypes.VisibleString:
                    Size = (uint)StringDefaultSizes.VisibleStringSize;
                    break;
            }
        }

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
                            byte[] tbuf;
                            uint iCheckCharacter = 0;
                            if (TagNode.ArrayDimension == 0)
                            {
                                if (buffer.Length > 0) {
                                    //{
                                    //    for (iCheckCharacter = 0; iCheckCharacter < Size; iCheckCharacter++)
                                    //    {
                                    //        if (buffer[(iCheckCharacter + index)] == 0x00)
                                    //        {
                                    //            break;
                                    //        }
                                    //    }
                                    //    tbuf = new byte[iCheckCharacter];
                                    //    Array.Copy(buffer, index, tbuf, 0, iCheckCharacter);
                                    //    return base.SetTagValue(ref tbuf, 0, forceUpdate, elemsize, buffertype);
                                    tbuf = new byte[buffer.Length];
                                    Array.Copy(buffer, index, tbuf, 0, buffer.Length);
                                    return base.SetTagValue(ref tbuf, 0, forceUpdate, elemsize, buffertype);
                                }                                
                                // Empty string is an acceptable case
                                else
                                {
                                    val = String.Empty;
                                    // Check if the update of the tag value must be forced 
                                    if((readValue != null) && ((readValue as String) != String.Empty))
                                    {
                                        bCopy = true;
                                    }
                                }
                            }
                            else
                            {
                                System.Text.UTF8Encoding enc = new System.Text.UTF8Encoding();
                                uint nOffset = 0;
                                string[] a = new string[TagNode.ArrayDimension];
                                Array b = readValue as Array;
                                bool check = (b != null && b.GetLength(0) == TagNode.ArrayDimension);
                                Array bL = Value.Value as Array;
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
                            if (readValue == null || bCopy)
                            {
                                SetValue(val);
                                return true;
                            }
                        }
                        break;

                    //case (uint)Opc.Ua.DataTypes.Int16:
                    //    {
                    //        uint size = (TagNode.ArrayDimension == 0 ? 2 : TagNode.ArrayDimension * 2);
                    //        if ((size + index) <= buffer.Length)
                    //        {
                    //            return base.SetTagValue(ref buffer, index, forceUpdate, elemsize, buffertype);
                    //        }
                    //        // Special case: data length < than expected
                    //        else
                    //        {
                    //            byte[] nbuf = null;
                    //            if (elemsize == 0)
                    //            {
                    //                nbuf = new byte[buffer.Length];
                    //                buffer.CopyTo(nbuf, 0);
                    //            }
                    //        }
                    //    }
                    //    break;

                    default:
                        return base.SetTagValue(ref buffer, index, forceUpdate, elemsize, buffertype);
                }

            }
            return (false);
        }

        #endregion

        #region Properties

        readonly IEC61850DynTagSettings _IEC61850DynSettings = new IEC61850DynTagSettings();
        public IEC61850DynTagSettings IEC61850DynSettings
        {
            get { return _IEC61850DynSettings; }
        }

        #endregion        

    }
}
