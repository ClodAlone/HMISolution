////////////////////////////////////////////////////////////////////////////////////////////////////
// file:	Persistence\DriverSerialExampleChannelSettings.cs
//
// summary:	Implements the driver serial example channel settings class
////////////////////////////////////////////////////////////////////////////////////////////////////

using System;
using System.Collections.Generic;
using System.Linq;
using SerialDriverCodeBase;
using DevExpress.Xpo;
using System.ComponentModel;

namespace DriverSerialExample
{
    /// <summary>   Settings for the drivers's channel(DriverSerialExampleChannel). </summary>
    [MapInheritance(MapInheritanceType.ParentTable)]
    public class DriverSerialExampleChannelSettings : SerialChannelSettings, IDataErrorInfo
    {
        #region Constructors

        ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>
        /// Constructor that prevents a default instance of this class from being created.
        /// </summary>
        ///
        /// <param name="session" type="Session">   The session. </param>
        ////////////////////////////////////////////////////////////////////////////////////////////////////
        public DriverSerialExampleChannelSettings(Session session)
            : base(session)
        {
            // This constructor is used when an object is loaded from a persistent storage.
            // Do not place any code here.
            session.UpdateSchema(typeof(SerialChannelSettings));
        }

        ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>
        /// Constructor that prevents a default instance of this class from being created.
        /// </summary>
        ////////////////////////////////////////////////////////////////////////////////////////////////////
        private DriverSerialExampleChannelSettings()
        {
            // This constructor is used when an object is loaded from a persistent storage.
            // Do not place any code here.
        }
        
        
        #endregion

        /// <summary>   Set propertys to default value. </summary>
        public void DefaultSettings()
        {
            base.DefaultSettings();
            CommPortName = "Com1";
            CommPortBaudRate = 19200;
            CommPortDataBits = 8;
            CommPortParity = (int)System.IO.Ports.Parity.None;
            CommPortStopBits = (int)System.IO.Ports.StopBits.One;
            CommPortHandshake = (int)System.IO.Ports.Handshake.None;
            CommPortRtsEnable = false;
            CommPortDtrEnable = false;
            CommPortReadTimeout = 5000;
            CommPortWriteTimeout = 5000;
            _FrameType = 0;
        }

        ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>   Set propertys from DriverSerialExampleChannelSettings "ch". </summary>
        ///
        /// <param name="ch">   . </param>
        ////////////////////////////////////////////////////////////////////////////////////////////////////
        public void CopyProperties(DriverSerialExampleChannelSettings ch)
        {
            base.CopyProperties(ch);
            FrameType = ch.FrameType;
        }
        /////////////////////////////

        #region Properties

        /// <summary>   Type of the frame. </summary>
        private byte _FrameType;
        ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>   Enter the frame type of the data exchange telegrams. </summary>
        ///
        /// <value> The type of the frame. </value>
        ////////////////////////////////////////////////////////////////////////////////////////////////////
        public byte FrameType
        {
            get
            {
                return _FrameType;
            }
            set
            {
                SetPropertyValue("FrameType", ref _FrameType, value);
            }
        }


        #endregion


        #region IDataErrorInfo Members

        ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>   Gets an error message indicating what is wrong with this object. </summary>
        ///
        /// <value>
        /// An error message indicating what is wrong with this object. The default is an empty string
        /// ("").
        /// </value>
        ////////////////////////////////////////////////////////////////////////////////////////////////////
        public new string Error
        {
            get
            {
                var context = new System.ComponentModel.DataAnnotations.ValidationContext(this, null, null);
                var results = new List<System.ComponentModel.DataAnnotations.ValidationResult>();

                return !System.ComponentModel.DataAnnotations.Validator.TryValidateObject(this, context, results)
                    ? string.Join(Environment.NewLine, results.Select(x => x.ErrorMessage))
                    : null;
            }
        }

        ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>   Indexer to get items within this collection using array index syntax. </summary>
        ///
        /// <param name="propertyName" type="string">   Name of the property. </param>
        ///
        /// <returns>   The indexed item. </returns>
        ////////////////////////////////////////////////////////////////////////////////////////////////////
        public new string this[string propertyName]
        {
            get
            {
                String s = PerformValidation(propertyName);
                if (!String.IsNullOrEmpty(s))
                    return s;
                var context = new System.ComponentModel.DataAnnotations.ValidationContext(this, null, null)
                {
                    MemberName = propertyName
                };

                var results = new List<System.ComponentModel.DataAnnotations.ValidationResult>();
                var value = GetType().GetProperty(propertyName).GetValue(this, null);

                return !System.ComponentModel.DataAnnotations.Validator.TryValidateProperty(value, context, results)
                    ? string.Join(Environment.NewLine, results.Select(x => x.ErrorMessage))
                    : null;
            }
        }

        ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>   DriverSerialExampleChannelSettings property validation. </summary>
        ///
        /// <param name="propertyName"> . </param>
        ///
        /// <returns>   A String. </returns>
        ////////////////////////////////////////////////////////////////////////////////////////////////////
        protected override String PerformValidation(String propertyName)
        {
            string sBase = base.PerformValidation(propertyName);
            if (sBase != null)
                return sBase;
            
            if (propertyName == "FrameType")
            {
                //FrameType
            }
       

            return null;
        }

        #endregion
    }
}
