using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DriverCodeBaseEx;
using DriverBaseInterfaces;
using Opc.Ua;

namespace ROCDriver
{


    public sealed class ROCDriverTag : Tag
    {
        #region Constructors

        public ROCDriverTag(TagDefinition tag, uint byteoffset, uint bitoffset)
            : base(tag, byteoffset, bitoffset)
        {

        }

        public ROCDriverTag(TagDefinition tag)
            : base(tag)
        {

        }

        #endregion

        #region Override Properties/Functions

        public override bool BuildDynamicSettings(String dynamicSettings)
        {
            bool res = ROCDynSettings.TryParse(dynamicSettings);
            if (TagNode.DataType.IdType == IdType.Numeric && (uint)TagNode.DataType.Identifier == Opc.Ua.DataTypes.String)
                Size = (uint)((ROCDynSettings.StringLength + 1) / 2) * 2;
            return res;
        }

        public override DynTagSettings DynSettings
        {
            get { return (DynTagSettings)ROCDynSettings; }
        }

        public override bool SetTagValue(ref byte[] buffer, int index, uint elemsize = 0, uint buffertype = 0, bool forceUpdate = false)
        {
            if (TagNode.DataType.IdType == IdType.Numeric)
            {
                if (Value.StatusCode.Code != StatusCodes.Good)
                    forceUpdate = true;

                if (forceUpdate)
                {
                    SetReadValue(null);
                }

                bool bCopy = false;
                object val = new object();
                uint size = 0;
                uint nType = (uint)TagNode.DataType.Identifier;
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
                                return (base.SetTagValue(ref buffer, index, forceUpdate, elemsize, buffertype));
                            }
                            if (GetReadValue() == null || bCopy)
                            {
                                SetReadValue(val);
                                WriteVal = val;
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

        private readonly ROCDriverDynTagSettings _ROCDynSettings = new ROCDriverDynTagSettings();
        public ROCDriverDynTagSettings ROCDynSettings
        {
            get { return _ROCDynSettings; }
        }

        #endregion        

    }
}
