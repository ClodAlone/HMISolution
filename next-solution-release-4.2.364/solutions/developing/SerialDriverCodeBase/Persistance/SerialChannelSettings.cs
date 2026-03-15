////////////////////////////////////////////////////////////////////////////////////////////////////
// file:	Persistance\SerialChannelSettings.cs
//
// summary:	Implements the serial channel settings class
////////////////////////////////////////////////////////////////////////////////////////////////////

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO.Ports;
using DriverCodeBase;
using DevExpress.Xpo.Metadata;
using DevExpress.Xpo;

namespace SerialDriverCodeBase
{
    /// <summary>   settings for the drivers's channel(SerialChannel) </summary>
    [Persistent("SerialChannelSettings_1")]
    public class SerialChannelSettings : ChannelSettings
    {
        #region Constructors

        ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>   Specialised default constructor for use only by derived class. </summary>
        ///
        /// <param name="session" type="Session">   The session. </param>
        ////////////////////////////////////////////////////////////////////////////////////////////////////
        protected SerialChannelSettings(Session session)
            : base(session)
        {
            // This constructor is used when an object is loaded from a persistent storage.
            // Do not place any code here.
            session.UpdateSchema(typeof(SerialChannelSettings));
        }

        /// <summary>   Specialised default constructor for use only by derived class. </summary>
        protected SerialChannelSettings()
        {
            // This constructor is used when an object is loaded from a persistent storage.
            // Do not place any code here.
        }
       
        
        #endregion

        ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>   Copies the properties described by ch. </summary>
        ///
        /// <param name="ch" type="SerialChannelSettings">  The ch. </param>
        ////////////////////////////////////////////////////////////////////////////////////////////////////
        public void CopyProperties(SerialChannelSettings ch)
        {
            //steve 080711
            base.CopyProperties(ch);
            CommPortName = ch.CommPortName;
            CommPortNameLinux = ch.CommPortNameLinux;
            CommPortBaudRate = ch.CommPortBaudRate;
            CommPortDataBits = ch.CommPortDataBits;
            CommPortParity = ch.CommPortParity;
            CommPortStopBits = ch.CommPortStopBits;
            CommPortHandshake = ch.CommPortHandshake;
            CommPortRtsEnable = ch.CommPortRtsEnable;
            CommPortDtrEnable = ch.CommPortDtrEnable;
            CommPortReadTimeout = ch.CommPortReadTimeout;
            CommPortWriteTimeout = ch.CommPortWriteTimeout;
        }
        /////////////////////////////

        /// <summary>   Default settings. </summary>
        public void DefaultSettings()
        {
            base.DefaultSettings();
            _CommPortName = "Com1";
            _CommPortNameLinux = string.Empty;
            _CommPortBaudRate = 9600;
            _CommPortDataBits = 8;
            _CommPortParity = (int)Parity.None;
            _CommPortStopBits = (int)StopBits.One;
            _CommPortHandshake = (int)Handshake.None;
            _CommPortRtsEnable = false;
            _CommPortDtrEnable = false;
            _CommPortReadTimeout = 5000;
            _CommPortWriteTimeout = 5000;
        }

        #region Properties

        /// <summary>   Port Name. </summary>
        private string _CommPortName;
        ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>   Gets or sets the name of the communications port. </summary>
        ///
        /// <exception cref="ArgumentException">    Thrown when one or more arguments have unsupported or
        ///                                         illegal values. </exception>
        ///
        /// <value> The name of the communications port. </value>
        ////////////////////////////////////////////////////////////////////////////////////////////////////
        public string CommPortName
        {
            get { return _CommPortName; }
            set
            {
                //if (String.IsNullOrEmpty(value))
                //{
                //    throw new ArgumentException("Port name cannot be null");
                //}

                SetPropertyValue("CommPortName", ref _CommPortName, value);
            }
        }

        /// <summary>   Linux Port Name. </summary>
        private string _CommPortNameLinux;
        ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>   Gets or sets the name of the communications port. </summary>
        ///
        /// <exception cref="ArgumentException">    Thrown when one or more arguments have unsupported or
        ///                                         illegal values. </exception>
        ///
        /// <value> The name of the communications port. </value>
        ////////////////////////////////////////////////////////////////////////////////////////////////////
        public string CommPortNameLinux
        {
            get { return _CommPortNameLinux; }
            set
            {
                //if (String.IsNullOrEmpty(value))
                //{
                //    throw new ArgumentException("Port name cannot be null");
                //}

                SetPropertyValue("CommPortNameLinux", ref _CommPortNameLinux, value);
            }
        }

        /// <summary>   Baud rate. </summary>
        private int _CommPortBaudRate;
        ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>   Gets or sets the communications port baud rate. </summary>
        ///
        /// <value> The communications port baud rate. </value>
        ////////////////////////////////////////////////////////////////////////////////////////////////////
        public int CommPortBaudRate
        {
            get
            {
                return _CommPortBaudRate;
            }
            set
            {
                SetPropertyValue("CommPortBaudRate", ref _CommPortBaudRate, value);
            }
        }

        /// <summary>   Data bits. </summary>
        private int _CommPortDataBits;
        ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>   Gets or sets the communications port data bits. </summary>
        ///
        /// <value> The communications port data bits. </value>
        ////////////////////////////////////////////////////////////////////////////////////////////////////
        public int CommPortDataBits
        {
            get
            {
                return _CommPortDataBits;
            }
            set
            {
                SetPropertyValue("CommPortDataBits", ref _CommPortDataBits, value);
            }
        }

        /// <summary>   Parity. </summary>
        private int _CommPortParity;
        ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>   Gets or sets the communications port parity. </summary>
        ///
        /// <value> The communications port parity. </value>
        ////////////////////////////////////////////////////////////////////////////////////////////////////
        public int CommPortParity
        {
            get
            {
                return _CommPortParity;
            }
            set
            {
                SetPropertyValue("CommPortParity", ref _CommPortParity, value);
            }
        }

        /// <summary>   Stop bits. </summary>
        private int _CommPortStopBits;
        ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>   Gets or sets the communications port stop bits. </summary>
        ///
        /// <value> The communications port stop bits. </value>
        ////////////////////////////////////////////////////////////////////////////////////////////////////
        public int CommPortStopBits
        {
            get
            {
                return _CommPortStopBits;
            }
            set
            {
                SetPropertyValue("CommPortStopBits", ref _CommPortStopBits, value);
            }
        }

        /// <summary>   Handshake. </summary>
        private int _CommPortHandshake;
        ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>   Gets or sets the communications port handshake. </summary>
        ///
        /// <value> The communications port handshake. </value>
        ////////////////////////////////////////////////////////////////////////////////////////////////////
        public int CommPortHandshake
        {
            get
            {
                return _CommPortHandshake;
            }
            set
            {
                SetPropertyValue("CommPortHandshake", ref _CommPortHandshake, value);
            }
        }

        /// <summary>   RTS enabled. </summary>
        private bool _CommPortRtsEnable;
        ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>
        /// Gets or sets a value indicating whether this object is communications port RTS enable.
        /// </summary>
        ///
        /// <value> true if communications port RTS enable, false if not. </value>
        ////////////////////////////////////////////////////////////////////////////////////////////////////
        public bool CommPortRtsEnable
        {
            get
            {
                return _CommPortRtsEnable;
            }
            set
            {
                SetPropertyValue("CommPortRtsEnable", ref _CommPortRtsEnable, value);
            }
        }

        /// <summary>   DTR enabled. </summary>
        private bool _CommPortDtrEnable;
        ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>
        /// Gets or sets a value indicating whether this object is communications port dtr enable.
        /// </summary>
        ///
        /// <value> true if communications port dtr enable, false if not. </value>
        ////////////////////////////////////////////////////////////////////////////////////////////////////
        public bool CommPortDtrEnable
        {
            get
            {
                return _CommPortDtrEnable;
            }
            set
            {
                SetPropertyValue("CommPortDtrEnable", ref _CommPortDtrEnable, value);
            }
        }

        /// <summary>   Read timeout. </summary>
        private int _CommPortReadTimeout;
        ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>   Gets or sets the communications port read timeout. </summary>
        ///
        /// <value> The communications port read timeout. </value>
        ////////////////////////////////////////////////////////////////////////////////////////////////////
        public int CommPortReadTimeout
        {
            get
            {
                return _CommPortReadTimeout;
            }
            set
            {
                SetPropertyValue("CommPortReadTimeout", ref _CommPortReadTimeout, value);
            }
        }

        /// <summary>   Write timeout. </summary>
        private int _CommPortWriteTimeout;
        ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>   Gets or sets the communications port write timeout. </summary>
        ///
        /// <value> The communications port write timeout. </value>
        ////////////////////////////////////////////////////////////////////////////////////////////////////
        public int CommPortWriteTimeout
        {
            get
            {
                return _CommPortWriteTimeout;
            }
            set
            {
                SetPropertyValue("CommPortWriteTimeout", ref _CommPortWriteTimeout, value);
            }
        }

        #endregion

        ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>   Performs the validation action. </summary>
        ///
        /// <param name="propertyName" type="String">   Name of the property. </param>
        ///
        /// <returns>   A String. </returns>
        ////////////////////////////////////////////////////////////////////////////////////////////////////
        protected override String PerformValidation(String propertyName)
        {
            string sBase = base.PerformValidation(propertyName);
            if (sBase != null)
                return sBase;

            switch (propertyName)
            {
                case "CommPortName":
                    if (string.IsNullOrWhiteSpace(CommPortName))
                        return Properties.Resources.ErrorPortNameInvalid;

                    if (DriverSettings != null && ((from c in DriverSettings.ChannelSettings/*.AsParallel()*/
                                                    where c != this && (c as SerialChannelSettings).CommPortName == CommPortName
                                                    select c).ToList().Count > 0))
                    {
                        return Properties.Resources.ErrorSerialPortAlreadyUsed;
                    }
                    break;
                case "CommPortNameLinux":
                    if (!string.IsNullOrEmpty(CommPortNameLinux))
                    {
                        if (DriverSettings != null && ((from c in DriverSettings.ChannelSettings/*.AsParallel()*/
                                                        where c != this && (c as SerialChannelSettings).CommPortNameLinux == CommPortNameLinux
                                                        select c).ToList().Count > 0))
                        {
                            return Properties.Resources.ErrorSerialPortAlreadyUsed;
                        }
                    }
                    break;
                case "CommPortBaudRate":
                    break;
                case "CommPortDataBits":
                    if (CommPortDataBits != 7 && CommPortDataBits != 8)
                    {
                        return Properties.Resources.ErrorDataBits;
                    }
                    break;
                case "CommPortParity":
                    if (CommPortParity < (int)Parity.None || CommPortParity > (int)Parity.Space)
                        return Properties.Resources.ErrorParity;
                    break;
                case "CommPortStopBits":
                    if (CommPortStopBits <= (int)StopBits.None || CommPortStopBits > (int)StopBits.OnePointFive)
                        return Properties.Resources.ErrorStopBits;
                    break;
                case "CommPortHandshake":
                    if (CommPortHandshake < (int)Handshake.None || CommPortHandshake > (int)Handshake.RequestToSendXOnXOff)
                        return Properties.Resources.ErrorHandshake;
                    break;
                case "CommPortRtsEnable":
                    break;
                case "CommPortDtrEnable":
                    break;
                case "CommPortReadTimeout":
                    if (CommPortReadTimeout < SerialPort.InfiniteTimeout)
                        return Properties.Resources.ErrorReadTimeout;
                    break;
                case "CommPortWriteTimeout":
                    if (CommPortWriteTimeout < SerialPort.InfiniteTimeout)
                        return Properties.Resources.ErrorWriteTimeout;
                    break;
            }
            return null;
        }
    }
}
