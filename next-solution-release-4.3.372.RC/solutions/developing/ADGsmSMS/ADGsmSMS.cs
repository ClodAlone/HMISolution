using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ADPluginBase;
using DevExpress.Xpo;
using Opc.Ua;
using System.IO.Ports;
using System.Threading;
using UFUAServerBase;
using Utilities.Logger;
using System.Runtime.InteropServices;

namespace ADGsmSMS
{
    
    public enum GsmCommBaudRate
    { 
        GsmBaud110 = 110,
        GsmBaud300 = 300,
        GsmBaud600 = 600,
        GsmBaud1200 = 1200,
        GsmBaud2400 = 2400,
        GsmBaud4800 = 4800,
        GsmBaud9600 = 9600,
        GsmBaud14400 = 14400,
        GsmBaud19200 = 19200,
        GsmBaud38400 = 38400,
        GsmBaud56000 = 56000,
        GsmBaud57600 = 57600,
        GsmBaud115200 = 115200,
        GsmBaud128000 = 128000,
        GsmBaud256000 = 256000
    }

    public enum GsmSMSError : int
    { 
        GsmErrorNoError = 0,
        GsmErrorInitFailed = -1,
        GsmErrorNoModemInit = -2,
        GsmErrorNoPin = -3,
        GsmErrorNoService = -4,
        GsmErrorNoAsciiFormat = -5,
        GsmErrorNoWriteNumber = -6,
        GsmErrorNoAnswer =  -7,
        GsmErrorNoWriteMessage = -8,
        GsmErrorNoAnswerMessage = -9,
        GsmErrorNoPduFormat = -10,
        GsmErrorNoMessage = -101
    }
    

    public class ADGsmSMS : PluginBase
    {
        #region Ctor
        public ADGsmSMS()
        { }
        #endregion

        #region Data
        SerialPort commPort;
        
        const byte msgEnd = 13;
        const byte msgReturn = 26;
        const int MaxNumOfSetpinAttempts = 3;
        const int PinStatusOk = 0;
        const int PinStatusPin = 1;
        const int PinStatusPuk = 2;
        const int PinStatusPin2 = 3;
        const int PinStatusError = 4;

        double minCharTime = 50;
        double warmUpTime = 60000;

        SerialError lasterror = 0;
        #endregion

        #region methods override
        public override bool Init(string strSettingPath)
        {
            PluginInfo.Initialize(this);
            return base.Init(strSettingPath);
        }

        public override string GetPluginName()
        {
            return PluginInfo.GetPluginName();
        }

        public override bool LoadPluginSettings(DevExpress.Xpo.IDataLayer idl)
        {
            bool bRet = false;
            using (UnitOfWork ufw = new UnitOfWork(idl))
            {
                try
                {
                    var configuration = (from tag in new XPQuery<PluginSettings>(ufw).AsParallel() select tag).Single();
                    LoadDriverSettings(configuration);
                    bRet = true;
                }
                catch (InvalidOperationException ex)
                {
                    OnSystemEvent(null, String.Format(Properties.Resources.ErrorLoadingPluginSettings, ex.Message), EventSeverity.Min,
                        null, null, null, (int)System.Diagnostics.EventLogEntryType.Error, (int)LoggerDestination.AlarmDispatcher);
                    LoadDefaultSettings();
                }
                finally
                {
                    /*if (RasEnable)
                    {
                        DisconnectTimer = new System.Timers.Timer() { Interval = DisconnectAfter * 1000 };
                        DisconnectTimer.Elapsed += DisconnectTimer_Elapsed;
                        dialer = new RasDialer();
                        dialer.DialCompleted += dialer_DialCompleted;
                        dialer.StateChanged += dialer_StateChanged;
                        currentPhonebookPath = (PhonebookPath.Length > 0 ?
                            PhonebookPath :
                                RasPhoneBook.GetPhoneBookPath(RasPhoneBookType.AllUsers));
                    }*/
                }
            }

            return bRet;
        }

        public override int SendMessage(object msg)
        {
            LastError = string.Empty;
            var messaggio = msg as ADPluginBase.Message;
            if (messaggio == null)
            {
                OnSystemEvent(null, Properties.Resources.NoMessage, EventSeverity.High, null,
                        null, null, (int)System.Diagnostics.EventLogEntryType.Error, (int)LoggerDestination.AlarmDispatcher);
                LastError = Properties.Resources.NoMessage;
                return (int)GsmSMSError.GsmErrorNoMessage;
            }
            string address = messaggio.MobilePhoneNumber;
            string message = (messaggio.CustomMessage ? messaggio.Textmessage : $"{messaggio.Name} {messaggio.Reason} - {messaggio.Textmessage} - {messaggio.Alarmmessage}");
            /*
             * open serial port, set modem,  send message.
             */
            lasterror = 0;
            if (commPort == null)
            {
                try
                {
                    string portName = string.Empty;
                    if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
                        portName = CommPortName;
                    else
                        portName = CommPortNameLinux;

                    commPort = new SerialPort
                    { /* commPort settings */
                        PortName = portName,
                        BaudRate = CommPortBaudRate,
                        DataBits = CommPortDataBits,
                        Parity = (Parity)CommPortParity,
                        StopBits = (StopBits)CommPortStopBits,
                        Handshake = (Handshake)CommPortHandshake,
                        RtsEnable = CommPortRtsEnable,
                        DtrEnable = CommPortDtrEnable,
                        ReadTimeout = CommPortReadTimeout,
                        WriteTimeout = CommPortWriteTimeout
                    };
                }
                catch(Exception ex)
                {
                    OnSystemEvent(null, string.Format(Properties.Resources.SerialPortCreationError, GetPluginName(), ex.Message), EventSeverity.High, null,
                        null, null, (int)System.Diagnostics.EventLogEntryType.Error, (int)LoggerDestination.AlarmDispatcher);
                }
            }

            if (minCharTime == 0)
                SetMinCharTime();

            DeviceOpen();
            Thread.Sleep(300);
            //Send init string
            if (!sendInitString(InitString))
            {
                DeviceClose();
                OnSystemEvent(null, Properties.Resources.GsmErrorNoModemInit, EventSeverity.High, null, string.Format(Properties.Resources.DetailsFormat, messaggio.Name, messaggio.TagName), messaggio.Reason,
                        (int)System.Diagnostics.EventLogEntryType.Error, (int)LoggerDestination.AlarmDispatcher);
                LastError = Properties.Resources.GsmErrorNoModemInit;
                return (int)GsmSMSError.GsmErrorNoModemInit;
            }

            Thread.Sleep(300);
            //try pin
            if(!SetPinCode(Pin))
            {
                DeviceClose();
                OnSystemEvent(null, Properties.Resources.GsmErrorNoPin, EventSeverity.High, null, string.Format(Properties.Resources.DetailsFormat, messaggio.Name, messaggio.TagName), messaggio.Reason,
                        (int)System.Diagnostics.EventLogEntryType.Error, (int)LoggerDestination.AlarmDispatcher);
                LastError = Properties.Resources.GsmErrorNoPin;
                return (int)GsmSMSError.GsmErrorNoPin;
            }

            Thread.Sleep(300);
            //try service center (not PDU)
            if(!SendPDU)
            {
                if(!SetServiceNumber())
                {
                    DeviceClose();
                    OnSystemEvent(null, Properties.Resources.GsmErrorNoService, EventSeverity.High, null, string.Format(Properties.Resources.DetailsFormat, messaggio.Name, messaggio.TagName), messaggio.Reason,
                        (int)System.Diagnostics.EventLogEntryType.Error, (int)LoggerDestination.AlarmDispatcher);
                    LastError = Properties.Resources.GsmErrorNoService;
                    return (int)GsmSMSError.GsmErrorNoService;
                }
            }

            Thread.Sleep(300);
            //try setdataformat
            DateTime wutime = DateTime.Now;
            bool dfok = SetDataFormat();
            while(!dfok && (DateTime.Now - wutime).TotalMilliseconds < warmUpTime)
            {
                Thread.Sleep(500);
                dfok = SetDataFormat();
            }
            if(!dfok)
            {
                DeviceClose();
                if (SendPDU)
                {
                    OnSystemEvent(null, Properties.Resources.GsmErrorNoPduFormat, EventSeverity.High, null, string.Format(Properties.Resources.DetailsFormat, messaggio.Name, messaggio.TagName), messaggio.Reason,
                        (int)System.Diagnostics.EventLogEntryType.Error, (int)LoggerDestination.AlarmDispatcher);
                    LastError = Properties.Resources.GsmErrorNoPduFormat;
                    return (int)GsmSMSError.GsmErrorNoPduFormat;
                }
                else
                {
                    OnSystemEvent(null, Properties.Resources.GsmErrorNoAsciiFormat, EventSeverity.High, null, string.Format(Properties.Resources.DetailsFormat, messaggio.Name, messaggio.TagName), messaggio.Reason,
                        (int)System.Diagnostics.EventLogEntryType.Error, (int)LoggerDestination.AlarmDispatcher);
                    LastError = Properties.Resources.GsmErrorNoAsciiFormat;
                    return (int)GsmSMSError.GsmErrorNoAsciiFormat;
                }
            }
            
            if(!SendPDU)
            {
                Thread.Sleep(300);

                string ns = string.Format("AT+CMGS=\"{0}\"", address);
                System.Text.UTF8Encoding enc = new UTF8Encoding();
                uint len = (uint) enc.GetByteCount(ns)+1;
                byte[] ccc = new byte[len];
                enc.GetBytes(ns).CopyTo(ccc, 0);
                ccc[len - 1] = msgEnd;
                if (!DeviceWrite(ccc, len))
                {
                    DeviceClose();
                    OnSystemEvent(null, Properties.Resources.GsmErrorNoWriteNumber, EventSeverity.High, null, string.Format(Properties.Resources.DetailsFormat, messaggio.Name, messaggio.TagName), messaggio.Reason,
                        (int)System.Diagnostics.EventLogEntryType.Error, (int)LoggerDestination.AlarmDispatcher);
                    LastError = Properties.Resources.GsmErrorNoWriteNumber;
                    return (int)GsmSMSError.GsmErrorNoWriteNumber;
                }
                if(!WaitOkAnswer())
                {
                    DeviceClose();
                    OnSystemEvent(null, Properties.Resources.GsmErrorNoAnswer, EventSeverity.High, null, string.Format(Properties.Resources.DetailsFormat, messaggio.Name, messaggio.TagName), messaggio.Reason,
                        (int)System.Diagnostics.EventLogEntryType.Error, (int)LoggerDestination.AlarmDispatcher);
                    LastError = Properties.Resources.GsmErrorNoAnswer;
                    return (int)GsmSMSError.GsmErrorNoAnswer;
                }
                Thread.Sleep(300);
                ns = message;
                len = (uint) enc.GetByteCount(ns)+1;
                byte[] nc = new byte[len];
                enc.GetBytes(ns).CopyTo(nc, 0);
                nc[len - 1] = msgReturn;
                if (!DeviceWrite(nc, len))
                    {
                    DeviceClose();
                    OnSystemEvent(null, Properties.Resources.GsmErrorNoWriteMessage, EventSeverity.High, null, string.Format(Properties.Resources.DetailsFormat, messaggio.Name, messaggio.TagName), messaggio.Reason,
                        (int)System.Diagnostics.EventLogEntryType.Error, (int)LoggerDestination.AlarmDispatcher);
                    LastError = Properties.Resources.GsmErrorNoWriteMessage;
                    return (int)GsmSMSError.GsmErrorNoWriteMessage;
                }
            }
            else
            {
                string s = string.Empty;
                if(ServiceCenter.Length == 0)
                    s += "00";

                var ns = Compose(message, address);
                string l = string.Format("AT+CMGS={0}", (ns.Length - 2) / 2);
                System.Text.UTF8Encoding enc = new UTF8Encoding();
                uint len = (uint) enc.GetByteCount(l)+1;
                byte[] ccc = new byte[len];
                enc.GetBytes(l).CopyTo(ccc, 0);
                ccc[len - 1] = msgEnd;
                
                if (!DeviceWrite(ccc, len))
                {
                    DeviceClose();
                    OnSystemEvent(null, Properties.Resources.GsmErrorNoWriteNumber, EventSeverity.High, null, string.Format(Properties.Resources.DetailsFormat, messaggio.Name, messaggio.TagName), messaggio.Reason,
                        (int)System.Diagnostics.EventLogEntryType.Error, (int)LoggerDestination.AlarmDispatcher);
                    LastError = Properties.Resources.GsmErrorNoWriteNumber;
                    return (int)GsmSMSError.GsmErrorNoWriteNumber;
                }
                if(!WaitOkAnswer())
                {
                    DeviceClose();
                    OnSystemEvent(null, Properties.Resources.GsmErrorNoAnswer, EventSeverity.High, null, string.Format(Properties.Resources.DetailsFormat, messaggio.Name, messaggio.TagName), messaggio.Reason,
                        (int)System.Diagnostics.EventLogEntryType.Error, (int)LoggerDestination.AlarmDispatcher);
                    LastError = Properties.Resources.GsmErrorNoAnswer;
                    return (int)GsmSMSError.GsmErrorNoAnswer;
                }

                //dati chiusi da ctrl-z
                UnicodeEncoding uEnc = new UnicodeEncoding();
                len = (uint) ns.Length+1;
                byte[] nc = new byte[len];
                enc.GetBytes(ns).CopyTo(nc, 0); 
                nc[len - 1] = msgReturn;
                if (!DeviceWrite(nc, len))
                {
                    DeviceClose();
                    OnSystemEvent(null, Properties.Resources.GsmErrorNoWriteMessage, EventSeverity.High, null, string.Format(Properties.Resources.DetailsFormat, messaggio.Name, messaggio.TagName), messaggio.Reason,
                        (int)System.Diagnostics.EventLogEntryType.Error, (int)LoggerDestination.AlarmDispatcher);
                    LastError = Properties.Resources.GsmErrorNoWriteMessage;
                    return (int)GsmSMSError.GsmErrorNoWriteMessage;
                }

            }
            
            if(!WaitOkAnswer())
            {
                DeviceClose();
                OnSystemEvent(null, Properties.Resources.GsmErrorNoAnswerMessage, EventSeverity.High, null, string.Format(Properties.Resources.DetailsFormat, messaggio.Name, messaggio.TagName), messaggio.Reason,
                        (int)System.Diagnostics.EventLogEntryType.Error, (int)LoggerDestination.AlarmDispatcher);
                LastError = Properties.Resources.GsmErrorNoAnswerMessage;
                return (int)GsmSMSError.GsmErrorNoAnswerMessage;
            }

            DeviceClose();
            Thread.Sleep((int)Pause);
            OnSystemEvent(null, string.Format(Properties.Resources.GsmErrorNoError, messaggio.Textmessage, messaggio.Recipient, messaggio.MobilePhoneNumber),
                EventSeverity.Min, null, string.Format(Properties.Resources.DetailsFormat, messaggio.Name, messaggio.TagName), messaggio.Reason,
                        (int)System.Diagnostics.EventLogEntryType.Information, (int)LoggerDestination.AlarmDispatcher);
            return (int)GsmSMSError.GsmErrorNoError;
        }
        #endregion

        #region methods
        public virtual string Compose(string message, string phoneNumber)
        {

            string encodedData = "00"; //Length of SMSC information. Here the length is 0, which means that the SMSC stored in the phone should be used. Note: This octet is optional. On some phones this octet should be omitted! (Using the SMSC stored in phone is thus implicit)

            encodedData += "11"; //PDU type (forst octet)
            encodedData += "00";
            encodedData += EncodePhoneNumber(phoneNumber);
            encodedData += "00"; //Protocol identifier (Short Message Type 0)
            encodedData += Convert.ToString((int)0x08, 16).PadLeft(2, '0'); //Data coding scheme
            encodedData += "aa"; //Validity Period


            byte[] messageBytes = null;


            messageBytes = EncodeUCS2(message);

            //return messageBytes;

            encodedData += Convert.ToString(messageBytes.Length, 16).PadLeft(2, '0'); //Length of message

            foreach (byte b in messageBytes)
                encodedData += Convert.ToString(b, 16).PadLeft(2, '0');

            return encodedData.ToUpper();
        }
        public static byte[] EncodeUCS2(string s)
        {
            return Encoding.BigEndianUnicode.GetBytes(s);
        }
        public static string EncodePhoneNumber(string phoneNumber)
        {
            bool isInternational = phoneNumber.StartsWith("+");

            if (isInternational)
                phoneNumber = phoneNumber.Remove(0, 1);

            int header = (phoneNumber.Length << 8) + 0x81 | (isInternational ? 0x10 : 0x20);

            if (phoneNumber.Length % 2 == 1)
                phoneNumber = phoneNumber.PadRight(phoneNumber.Length + 1, 'F');

            phoneNumber = ReverseBits(phoneNumber);

            return Convert.ToString(header, 16).PadLeft(4, '0') + phoneNumber;
        }
        public static string ReverseBits(string source)
        {
            return ReverseBits(source, source.Length);
        }
        public static string ReverseBits(string source, int length)
        {
            string result = string.Empty;

            for (int i = 0; i < length; i++)
                result = result.Insert(i % 2 == 0 ? i : i - 1, source[i].ToString());

            return result;
        }
        void SetMinCharTime()
        {
            switch (CommPortBaudRate)
            {
                case 110: minCharTime = 700; break;
                case 300: minCharTime = 700; break;
                case 600: minCharTime = 350; break;
                case 1200: minCharTime = 175; break;
                case 2400: minCharTime = 84; break;
                case 4800: minCharTime = 42; break;
                case 9600: minCharTime = 21; break;
                case 14400: minCharTime = 21; break;
                case 19200: minCharTime = 14; break;
                case 38400: minCharTime = 7; break;
                case 56000: minCharTime = 7; break;
                case 57600: minCharTime = 7; break;
                case 115200: minCharTime = 7; break;
                case 128000: minCharTime = 7; break;
                case 256000: minCharTime = 7; break;
                default: minCharTime = 700; break;
            }

        }

        public void ErrorReceived(object sender, SerialErrorReceivedEventArgs e)
        {
            //SetInError(true, e.ToString());
            lasterror = e.EventType;
            
        }
        public uint GetModemAnswer(ref byte[] received)
        {
            Thread.Sleep(100);
            bool complete = false;
            uint rec = 0;
            uint lastrec = 0;
            DateTime lasttime = DateTime.Now;
            while(!complete)
            {
                rec = GetBytesToRead();
                if(rec != lastrec)
                {
                    //chars coming...
                    lastrec = rec;
                    lasttime = DateTime.Now;
                }
                else if(lastrec == 0 && ((DateTime.Now - lasttime).TotalMilliseconds > Timeout || lasterror != 0))
                {
                    //check Timeout
                    return 0;
                }
                else if((DateTime.Now - lasttime).TotalMilliseconds < minCharTime)
                {
                    Thread.Sleep((int)minCharTime);
                }
                else if(rec > 0)
                    complete = true;
            }
            if(DeviceRead(received, rec))
                return rec;

            return 0;
        }

        public bool WaitOkAnswer()
        {
            byte[] received = new byte[255];
            if (GetModemAnswer(ref received) <= 0)
                    return false;
            System.Text.UTF8Encoding enc = new UTF8Encoding();

            string result = new string(enc.GetChars(received));// received.ToString();
                if(result.ToLower().Contains("error"))
                    return false;
            return true;
        }
        public bool sendInitString(string init)
        {
            init = init.Trim();
            if (init.Length > 0)
            { 
                System.Text.UTF8Encoding enc = new UTF8Encoding();
                uint len = (uint) enc.GetByteCount(init)+1;
                byte[] bbb = new byte[len];
                enc.GetBytes(init).CopyTo(bbb,0);
                bbb[len - 1] = msgEnd;

                if (DeviceWrite(bbb, len) != true)
                    return false;

                //get modem answer
                return WaitOkAnswer();
            }
            return true;
        }
        public int GetPinStatus()
        {
            string s= "AT+CPIN?";
            System.Text.UTF8Encoding enc = new UTF8Encoding();
            uint len = (uint) enc.GetByteCount(s)+1;
            byte[] bbb = new byte[len];
            enc.GetBytes(s).CopyTo(bbb, 0);
            bbb[len - 1] = msgEnd;
            if (!DeviceWrite(bbb, len))
                return PinStatusError;

            byte[] received = new byte[255];
            if(GetModemAnswer(ref received) <= 0)
                return PinStatusError;

            string result = new string(enc.GetChars(received)).ToLower(); //received.ToString().ToLower();

            if( result.Contains("redy") || result.Contains("ready"))
		        return PinStatusOk; 
	        else if(result.Contains("sim pin2"))
		        return PinStatusPin2;
	        else if(result.Contains("sim pin"))
		        return PinStatusPin; 
	        else if(result.Contains("sim puk"))
		        return PinStatusPuk; 
	        else
		        return PinStatusError;
        }

        public bool SetPinCode(string pin)
        {
            pin = pin.Trim();
            int retries = 0;
            int pinstatus = PinStatusOk;
            string s = string.Empty;
            if(pin.Length > 0)
            {
                do
                {
                    retries++;
                    pinstatus = GetPinStatus();
                    switch(pinstatus)
                    {
                        case PinStatusOk:
                            break;
                        case PinStatusPin:
                        case PinStatusPuk:
                        case PinStatusPin2:
                            {
                                if(pinstatus == PinStatusPin)
                                    s= string.Format("AT+CPIN={0}", pin);
                                else if(pinstatus == PinStatusPuk)
                                    s= string.Format("AT+CPIN={0}", PUK);
                                else if(pinstatus == PinStatusPin2)
                                    s= string.Format("AT+CPIN2={0}", Pin2);
                                System.Text.UTF8Encoding enc = new UTF8Encoding();
                                uint len = (uint) enc.GetByteCount(s)+1;
                                byte[] bbb = new byte[len];
                                enc.GetBytes(s).CopyTo(bbb, 0);
                                bbb[len - 1] = msgEnd;
                                if (!DeviceWrite(bbb, len) || !WaitOkAnswer())
                                    pinstatus = PinStatusError;
                            }
                            break;
                    }
                }
                while(retries <= MaxNumOfSetpinAttempts && 
                    (pinstatus == PinStatusPin || pinstatus == PinStatusPuk || pinstatus == PinStatusPin2));
            }
            return true;
        }

        public bool SetServiceNumber()
        {
            string service = ServiceCenter.Trim();
            if (service.Length > 0)
            {
                string s= "AT+CSCA?";
                System.Text.UTF8Encoding enc = new UTF8Encoding();
                uint len = (uint) enc.GetByteCount(s)+1;
                byte[] bbb = new byte[len];
                enc.GetBytes(s).CopyTo(bbb, 0);
                bbb[len - 1] = msgEnd;
                if (!DeviceWrite(bbb, len))
                    return false;

                byte[] received = new byte[255];
                if(GetModemAnswer(ref received) <= 0)
                    return false;

                string result = new string(enc.GetChars(received)).ToLower();//received.ToString().ToLower();
                if (result.Contains(service.ToLower()))
                    return true;
                //set the service center number

                string ns = string.Format("AT+CSCA={0}", service);
                len = (uint) enc.GetByteCount(ns)+1;
                byte[] ccc = new byte[len];
                enc.GetBytes(ns).CopyTo(ccc, 0);
                ccc[len - 1] = msgEnd;
                if (!DeviceWrite(ccc, len) || !WaitOkAnswer())
                    return false;
            }
            return true;
        }

        public bool SetDataFormat()
        {
            string s= "AT+CMGF?";
            System.Text.UTF8Encoding enc = new UTF8Encoding();
            uint len = (uint) enc.GetByteCount(s)+1;
            byte[] bbb = new byte[len];
            enc.GetBytes(s).CopyTo(bbb, 0);
            bbb[len - 1] = msgEnd;
            if (!DeviceWrite(bbb, len))
                return false;
            byte[] received = new byte[255];
            if(GetModemAnswer(ref received) <= 0)
                return false;

            string result = new string(enc.GetChars(received)).ToLower();//received.ToString().ToLower();
            if (SendPDU)
            {
                if (result.Contains("0"))
                    return true;
            }
            else
            {
                if (result.Contains("1"))
                    return true;
            }

            if (SendPDU)
                s = "AT+CMGF=0";
            else
                s = "AT+CMGF=1";
            len = (uint)enc.GetByteCount(s) + 1;
            byte[] ccc = new byte[len];
            enc.GetBytes(s).CopyTo(ccc, 0);
            ccc[len - 1] = msgEnd;
            if (!DeviceWrite(ccc, len))
                return false;

            return WaitOkAnswer();

        }

        public bool DeviceOpen()
        {
            if (commPort != null)
            {
                if (commPort.IsOpen)
                    return true;

                commPort.ReceivedBytesThreshold = 1;
                //commPort.DataReceived += DataReceived;
                lasterror = 0;
                commPort.ErrorReceived += ErrorReceived;
                try
                {
                    commPort.Open();
                }
                catch (Exception e)
                {
                    return false;
                }
                return true;
            }
            return false;
        }

        public bool DeviceClose()
        {
            if (commPort != null)
                commPort.Close();
            return true;
        }

        public bool DeviceRead(byte[] buffer, uint count)
        {
            if (commPort == null || !commPort.IsOpen)
                return false;

            Int32 byteReads = 0;
            bool bRet = false;
            try
            {
                byteReads = commPort.Read(buffer, 0, (int)count);
                bRet = true;
            }
            catch (Exception ex)
            {
                //SetInError(true, ex.Message);
            }

            //if (bRet)
            //    TotalRxBytes += (uint)byteReads;

            return bRet;
        }

        public bool DeviceWrite(byte[] buffer, uint count)
        {
            if (commPort == null || !commPort.IsOpen)
                return false;

            bool bRet = false;
            try
            {
                commPort.Write(buffer, 0, (int)count);
                bRet = true;
            }
            catch (Exception ex)
            {
                //SetInError(true, ex.Message);
            }

            //if (bRet)
            //    TotalTxBytes += (uint)count;

            return bRet;
        }

        public uint GetBytesToRead()
        {
            uint count = 0;
            if (commPort == null || !commPort.IsOpen)
                return count;

            try
            {
                count = (uint)commPort.BytesToRead;
            }
            catch (Exception ex)
            {
                //SetInError(true, ex.Message);
            }

            return count;
        }

        public uint GetBytesToWrite()
        {
            uint count = 0;
            if (commPort == null || !commPort.IsOpen)
                return count;

            try
            {
                count = (uint)commPort.BytesToWrite;
            }
            catch (Exception ex)
            {
                //SetInError(true, ex.Message);
            }

            return count;
        }
        private bool LoadDriverSettings(PluginSettings configuration)
        {
            if (configuration == null)
                return false;
            Timeout = configuration.Timeout;
            Pause = configuration.Pause;
            InitString = configuration.InitString;
            ServiceCenter = configuration.ServiceCenter;
            Pin = configuration.Pin;
            PUK = configuration.PUK;
            Pin2 = configuration.Pin2;
            SendPDU = configuration.SendPDU;
            CommPortName = configuration.CommPortName;
            CommPortNameLinux = configuration.CommPortNameLinux;
            CommPortBaudRate = configuration.CommPortBaudRate;
            CommPortDataBits = configuration.CommPortDataBits;
            CommPortParity = configuration.CommPortParity;
            CommPortStopBits = configuration.CommPortStopBits;
            CommPortHandshake = configuration.CommPortHandshake;
            CommPortRtsEnable = configuration.CommPortRtsEnable;
            CommPortDtrEnable = configuration.CommPortDtrEnable;
            CommPortReadTimeout = configuration.CommPortReadTimeout;
            CommPortWriteTimeout = configuration.CommPortWriteTimeout;
            return true;
        }
        public void LoadDefaultSettings()
        {
            Timeout = 5000;
            Pause = 1000;
            InitString = string.Empty;
            ServiceCenter = string.Empty;
            Pin = string.Empty;
            PUK = string.Empty;
            Pin2 = string.Empty;
            SendPDU = false;   

            CommPortName = "Com1";
            CommPortNameLinux = "/dev/ttyS0";
            CommPortBaudRate = 9600;
            CommPortDataBits = 8;
            CommPortParity = (int)Parity.None;
            CommPortStopBits = (int)StopBits.One;
            CommPortHandshake = (int)Handshake.None;
            CommPortRtsEnable = false;
            CommPortDtrEnable = false;
            CommPortReadTimeout = 5000;
            CommPortWriteTimeout = 5000;
        }

        public void SavePluginSettings(IDataLayer idl)
        {
            using (UnitOfWork ufw = new UnitOfWork(idl))
            {
                var configuration = (from tag in new XPQuery<PluginSettings>(ufw).AsParallel() select tag).ToList();

                if (configuration.Count == 0)
                    configuration.Add(new PluginSettings(ufw));
                SavePluginSettings(configuration[0]);
                ufw.CommitChanges();
            }
        }
        private void SavePluginSettings(PluginSettings settings)
        {
            settings.Timeout = Timeout;
            settings.Pause = Pause;
            settings.InitString = InitString;
            settings.ServiceCenter = ServiceCenter;
            settings.Pin = Pin;
            settings.PUK = PUK;
            settings.Pin2 = Pin2;
            settings.SendPDU = SendPDU;
            settings.CommPortName = CommPortName;
            settings.CommPortNameLinux = CommPortNameLinux;
            settings.CommPortBaudRate = CommPortBaudRate;
            settings.CommPortDataBits = CommPortDataBits;
            settings.CommPortParity = CommPortParity;
            settings.CommPortStopBits = CommPortStopBits;
            settings.CommPortHandshake = CommPortHandshake;
            settings.CommPortRtsEnable = CommPortRtsEnable;
            settings.CommPortDtrEnable = CommPortDtrEnable;
            settings.CommPortReadTimeout = CommPortReadTimeout;
            settings.CommPortWriteTimeout = CommPortWriteTimeout;
        }

        #endregion

        #region Properties
        /// <summary>
        /// Port Name
        /// </summary>
        private string _CommPortName;
        public string CommPortName
        {
            get { return _CommPortName; }
            set
            {
                if (String.IsNullOrEmpty(value))
                {
                    throw new ArgumentException("Port name cannot be null");
                }

                _CommPortName = value;
            }
        }

        private string _CommPortNameLinux;
        public string CommPortNameLinux
        {
            get { return _CommPortNameLinux; }
            set
            {
                if (String.IsNullOrEmpty(value))
                {
                    throw new ArgumentException("Port name cannot be null");
                }

                _CommPortNameLinux = value;
            }
        }

        /// <summary>
        /// Baud rate
        /// </summary>
        private int _CommPortBaudRate;
        public int CommPortBaudRate
        {
            get { return _CommPortBaudRate; }
            set { _CommPortBaudRate = value; }
        }

        /// <summary>
        /// Data bits
        /// </summary>
        private int _CommPortDataBits;
        public int CommPortDataBits
        {
            get { return _CommPortDataBits; }
            set { _CommPortDataBits = value; }
        }

        /// <summary>
        /// Parity
        /// </summary>
        private int _CommPortParity;
        public int CommPortParity
        {
            get { return _CommPortParity; }
            set { _CommPortParity = value; }
        }

        /// <summary>
        /// Stop bits
        /// </summary>
        private int _CommPortStopBits;
        public int CommPortStopBits
        {
            get { return _CommPortStopBits; }
            set { _CommPortStopBits = value; }
        }

        /// <summary>
        /// Handshake
        /// </summary>
        private int _CommPortHandshake;
        public int CommPortHandshake
        {
            get { return _CommPortHandshake; }
            set { _CommPortHandshake = value; }
        }

        /// <summary>
        /// RTS enabled
        /// </summary>
        private bool _CommPortRtsEnable;
        public bool CommPortRtsEnable
        {
            get { return _CommPortRtsEnable; }
            set { _CommPortRtsEnable = value; }
        }

        /// <summary>
        /// DTR enabled
        /// </summary>
        private bool _CommPortDtrEnable;
        public bool CommPortDtrEnable
        {
            get { return _CommPortDtrEnable; }
            set { _CommPortDtrEnable = value; }
        }

        /// <summary>
        /// Read timeout
        /// </summary>
        private int _CommPortReadTimeout;
        public int CommPortReadTimeout
        {
            get { return _CommPortReadTimeout; }
            set { _CommPortReadTimeout = value; }
        }

        /// <summary>
        /// Write timeout
        /// </summary>
        private int _CommPortWriteTimeout;
        public int CommPortWriteTimeout
        {
            get { return _CommPortWriteTimeout; }
            set { _CommPortWriteTimeout = value; }
        }
        private uint _Timeout;
        public uint Timeout
        {
            get { return _Timeout; }
            set { _Timeout = value;}
        }

        private uint _Pause;
        public uint Pause
        {
            get { return _Pause; }
            set { _Pause = value; }
        }
        private string _InitString;
        public string InitString
        {
            get { return _InitString; }
            set { _InitString = value; }
        }
        private string _ServiceCenter;
        public string ServiceCenter
        {
            get { return _ServiceCenter; }
            set { _ServiceCenter = value; }
        }
        private string _Pin;
        public string Pin
        {
            get { return _Pin; }
            set { _Pin = value; }
        }
        private string _PUK;
        public string PUK
        {
            get { return _PUK; }
            set { _PUK = value; }
        }
        private string _Pin2;
        public string Pin2
        {
            get { return _Pin2; }
            set { _Pin2 = value; }
        }
        private bool _SendPDU;
        public bool SendPDU
        {
            get { return _SendPDU; }
            set { _SendPDU = value; }
        }
        #endregion

        #region IDisposable

        protected override void Dispose(bool disposing)
        {
            DeviceClose();
            if (commPort != null)
            {
                commPort.Dispose();
            }

            base.Dispose(disposing);
        }

        #endregion
    }
}
