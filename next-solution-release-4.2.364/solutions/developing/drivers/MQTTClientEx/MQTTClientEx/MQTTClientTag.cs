using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DriverCodeBaseEx;
using DriverBaseInterfaces;
using DriverCodeBaseEx.Helpers;
using Opc.Ua;
using System.Runtime.InteropServices;

namespace MQTTClient
{


    public sealed class MQTTClientTag : Tag
    {
        #region Constructors

        public MQTTClientTag(TagDefinition tag, uint byteoffset, uint bitoffset)
            : base(tag, byteoffset, bitoffset)
        {
            _JsonMessageFormat = String.Empty;
            _JsonMessageTimestampField = String.Empty;
            _JsonTimestampFormat = JSonTimestampFormats.tf_ISO;
            LastPublishedValue = null;
        }

        public MQTTClientTag(TagDefinition tag)
            : base(tag)
        {
            _JsonMessageFormat = String.Empty;
            _JsonMessageTimestampField = String.Empty;
            _JsonTimestampFormat = JSonTimestampFormats.tf_ISO;
            LastPublishedValue = null;
        }

        #endregion

        #region specific members
        public object LastPublishedValue;
        #endregion

        #region Override Properties/Functions

        public override bool BuildDynamicSettings(String dynamicSettings)
        {
            return MQTTClientDynSettings.TryParse(dynamicSettings);
        }

        public override DynTagSettings DynSettings
        {
            get { return (DynTagSettings)MQTTClientDynSettings; }
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

        readonly MQTTClientDynTagSettings _MQTTClientDynSettings = new MQTTClientDynTagSettings();
        public MQTTClientDynTagSettings MQTTClientDynSettings
        {
            get { return _MQTTClientDynSettings; }
        }

        /// <summary>
        /// Format of Json messages
        /// </summary>
        private string _JsonMessageFormat;
        public string JsonMessageFormat
        {
            get { return _JsonMessageFormat; }
            set { _JsonMessageFormat = value; }
        }

        /// <summary>
        /// Timestamp of a Json message
        /// </summary>
        private string _JsonMessageTimestampField;
        public string JsonMessageTimestampField
        {
            get { return _JsonMessageTimestampField; }
            set { _JsonMessageTimestampField = value; }
        }

        /// <summary>
        /// Format of the Timestamp of a Json message
        /// </summary>
        private JSonTimestampFormats _JsonTimestampFormat;
        public JSonTimestampFormats JsonTimestampFormat
        {
            get { return _JsonTimestampFormat; }
            set { _JsonTimestampFormat = value; }
        }

        #endregion        

    }
}
