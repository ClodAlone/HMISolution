#if !NET_STANDARD
using DotRas;
#endif
using DriverBaseInterfaces;
using MailKit.Net.Smtp;
using MimeKit;
using Opc.Ua;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Security;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace SmtpSender
{

    public enum SmtpErrors : int
    {
        NoError = 0,
        NoMessage = -101,
        InvalidSyntaxMessage = -5,
        RASError = -1,
        DialupCancelled = -2,
        DialupTimedout = -3,
        DialupError = -4,
        InvalidMailAddress = -6,
        ErrorSendMessage = -7,
        InvalidSendAddress = -8,
        ModuleNotInitialized = -9

    }

    public class SmtpClientModule : IDisposable
    {
        #region Ctor
        public SmtpClientModule()
        {

        }
        #endregion Ctor

        #region Ras Dialer Data
#if !NET_STANDARD
        private RasDialer dialer;
        private RasHandle handle;
        System.Timers.Timer DisconnectTimer;
        string currentPhonebookPath = string.Empty;
        ManualResetEvent ConnectionComplete = new ManualResetEvent(false);
        int dialingresult = 0;
#endif
        #endregion Ras Dialer Data

        #region Data
        private bool disposedValue;
        private ServerSettings currServerSettings;
        private bool initialized = false;
        #endregion Data

        #region Properties
        public string ErrorMessage { get; set; }
        public string ModuleName { get; set; }
        public string LoggerSource { get; set; }
        public int LoggerDestination { get; set; }
        #endregion Properties

        #region Public Methods
        public bool Init(ServerSettings serverSettings)
        {
            if (initialized)
                return true;

            //init settings for the server connection and, eventually, RAS dialer
            if (currServerSettings == null)
                currServerSettings = new ServerSettings();
            currServerSettings = serverSettings;
#if !NET_STANDARD
            if (currServerSettings.RasEnable)
            {
                DisconnectTimer = new System.Timers.Timer()
                { Interval = currServerSettings.DisconnectAfter * 1000 };
                DisconnectTimer.Elapsed += DisconnectTimer_Elapsed;
                dialer = new RasDialer();
                dialer.DialCompleted += dialer_DialCompleted;
                dialer.StateChanged += dialer_StateChanged;
                currentPhonebookPath = (currServerSettings.PhonebookPath.Length > 0 ?
                    currServerSettings.PhonebookPath :
                        RasPhoneBook.GetPhoneBookPath(RasPhoneBookType.AllUsers));
            }
#endif
            initialized = true;
            return true;
        }

        public event EventHandler<SystemEventArgs> SystemEvent;
        public virtual void OnSystemEvent(object nodeId, String errMessage, EventSeverity severity, NodeId evtype = null,
            String details = null, String state = null,
            int logtype = -1, int logdestination = -1)
        {
            var temp = SystemEvent;
            if (temp != null)
            {
                SystemEventArgs e = new SystemEventArgs();
                if (nodeId != null)
                    e.sourceNode = (NodeId)nodeId;
                else
                    e.sourceNode = new NodeId(ModuleName);

                e.sourceName = String.Format("{0} {1}", LoggerSource, ModuleName);
                e.EventName = errMessage;
                e.severity = severity;
                e.time = DateTime.UtcNow;
                e.eventtype = (evtype == null ? ObjectTypeIds.DeviceFailureEventType : evtype);
                if (details != null)
                    e.details = details;
                if (state != null)
                    e.state = state;

                e.logtype = logtype;
                e.logdestination = logdestination;

                temp(this, e);
            }
        }

        bool MovServerCertificateValidationCallback(object sender, X509Certificate certificate, X509Chain chain, SslPolicyErrors sslPolicyErrors)
        {
            if (sslPolicyErrors == SslPolicyErrors.None)
                return true;
            string s = string.Format("Error: {0}\n", sslPolicyErrors);
            for (int i = 0; i<chain.ChainElements.Count; i++)
            {
                s += string.Format("Chain element {0} {1} - {2}\n", i, chain.ChainElements[i].ToString(), chain.ChainElements[i].ChainElementStatus);
            }
            s += string.Format("Cert to:{0} issurer: {1}", certificate.Subject, certificate.Issuer);

            OnSystemEvent(null, s, EventSeverity.High, logtype: (int) System.Diagnostics.EventLogEntryType.Error,
                logdestination: LoggerDestination);

            return false;
        }

        public SmtpErrors SendMail(MailSettings mail)
        {
            ErrorMessage = string.Empty;
            if (currServerSettings == null)
            {
                ErrorMessage = Properties.Resources.ModuleUninitialized;
                return SmtpErrors.ModuleNotInitialized;
            }
                
#if !NET_STANDARD
            if (currServerSettings.RasEnable)
            {
                DisconnectTimer.Stop();
                //Ras server connected?
                RasConnectionStatus r = null;
                if (handle != null)
                    r = RasConnection.GetActiveConnectionByHandle(handle).GetConnectionStatus();
                if (r == null || (r.ConnectionState != RasConnectionState.Connected))
                {
                    if (currServerSettings.PhoneNumber.Length > 0)
                        dialer.PhoneNumber = currServerSettings.PhoneNumber;
                    else
                        dialer.EntryName = currServerSettings.DialupEntry;

                    dialer.PhoneBookPath = currentPhonebookPath;
                    dialer.Timeout = currServerSettings.RetryTime * 1000;

                    try
                    {
                        // Set the credentials the dialer should use.
                        dialer.Credentials = new System.Net.NetworkCredential(currServerSettings.RASUser, currServerSettings.RASPassword);
                        handle = dialer.DialAsync();
                    }
                    catch (Exception ex)
                    {
                        OnSystemEvent(null, string.Format(Properties.Resources.RASError, ex.Source, ex.Message), EventSeverity.High, null, mail.Name, mail.Reason,
                        (int)System.Diagnostics.EventLogEntryType.Error, LoggerDestination);
                        ErrorMessage = string.Format(Properties.Resources.RASError, ex.Source, ex.Message);
                        return SmtpErrors.RASError;
                    }

                    //wait for the connection to complete.
                    if (!ConnectionComplete.WaitOne(currServerSettings.RetryTime * 1100))
                    {
                        dialer.DialAsyncCancel();
                        ConnectionComplete.Reset();
                    }

                    if (dialingresult != 0)
                    {
                        OnSystemEvent(null, string.Format(Properties.Resources.DialRASError, dialingresult), EventSeverity.High, null, mail.Name, mail.Reason,
                        (int)System.Diagnostics.EventLogEntryType.Error, LoggerDestination);
                        ErrorMessage = string.Format(Properties.Resources.DialRASError, dialingresult);
                        return (SmtpErrors)dialingresult;
                    }
                }
            }
#endif
            MimeMessage mMsg = new MimeMessage();
            try
            {
                mMsg.From.Add(MailboxAddress.Parse(mail.From));
            }
            catch (Exception e)
            {
#if !NET_STANDARD
                if (currServerSettings.RasEnable)
                    DisconnectTimer.Start();
#endif
                OnSystemEvent(null, string.Format(Properties.Resources.InvalidMailAddress, e.Source, e.Message), EventSeverity.High, null, mail.Name, mail.Reason,
                        (int)System.Diagnostics.EventLogEntryType.Error, LoggerDestination);
                ErrorMessage = string.Format(Properties.Resources.InvalidMailAddress, e.Source, e.Message);
                return SmtpErrors.InvalidMailAddress;
            }

            try
            {
                foreach(var to in mail.Address)
                    mMsg.To.Add(MailboxAddress.Parse(to));
            }
            catch (Exception e)
            {
#if !NET_STANDARD
                if (currServerSettings.RasEnable)
                    DisconnectTimer.Start();
#endif
                OnSystemEvent(null, string.Format(Properties.Resources.InvalidSendAddress, e.Source, e.Message), EventSeverity.High, null, mail.Name, mail.Reason,
                        (int)System.Diagnostics.EventLogEntryType.Error, LoggerDestination);
                ErrorMessage = string.Format(Properties.Resources.InvalidSendAddress, e.Source, e.Message);
                return SmtpErrors.InvalidSendAddress;
            }
            mMsg.Subject = mail.Subject;

            var builder = new BodyBuilder();
            builder.TextBody = mail.Message;

            if (mail.GetAttachmentFileCount() > 0)
            {
                //attachments
                var list = mail.GetAttachmentsFile();
                foreach (var attached in list)
                {
                    if (File.Exists(attached))
                    {
                        builder.Attachments.Add(attached);
                    }
                    else
                    {
                        OnSystemEvent(null, string.Format(Properties.Resources.AttachmentFileNotFound, attached), EventSeverity.Low, null, mail.Name, mail.Reason,
                            (int)System.Diagnostics.EventLogEntryType.Warning, LoggerDestination);
                    }
                }
            }

            var atcList = mail.GetAttachments();
            if (atcList != null && atcList.Count > 0)
            {
                foreach (var item in atcList)
                    builder.Attachments.Add(item);
            }
            mMsg.Body = builder.ToMessageBody();
            using (var smtpclient = new SmtpClient())
            {
                try
                {
                    smtpclient.CheckCertificateRevocation = Properties.Settings.Default.CheckCertificateRevocation;
                    smtpclient.ServerCertificateValidationCallback = MovServerCertificateValidationCallback;

                    smtpclient.Connect(currServerSettings.ServerAddress, currServerSettings.ServerPort);
                    if (currServerSettings.User.Length > 0 && currServerSettings.Password.Length > 0)
                    {
                        smtpclient.Authenticate(currServerSettings.User, currServerSettings.Password);
                    }
                    smtpclient.Send(mMsg);

                    smtpclient.Disconnect(true);
                }
                catch (Exception ex)
                {
#if !NET_STANDARD
                    if (currServerSettings.RasEnable)
                        DisconnectTimer.Start();
#endif
                    OnSystemEvent(null, string.Format(Properties.Resources.ErrorSendMessage, ex.Source, ex.Message), EventSeverity.High, null, mail.Name, mail.Reason,
                        (int)System.Diagnostics.EventLogEntryType.Error, LoggerDestination);
                    ErrorMessage = string.Format(Properties.Resources.ErrorSendMessage, ex.Source, ex.Message);
                    return SmtpErrors.ErrorSendMessage;
                }
            }
#if !NET_STANDARD
            if (currServerSettings.RasEnable)
                DisconnectTimer.Start();
#endif
            return SmtpErrors.NoError;
        }
        #endregion Public Methods


        #region RAS dialer utils
#if !NET_STANDARD
        void DisconnectTimer_Elapsed(object sender, System.Timers.ElapsedEventArgs e)
        {
            if (dialer.IsBusy)
            {
                dialer.DialAsyncCancel();
            }
            else
            {
                RasConnection connection = RasConnection.GetActiveConnectionByHandle(handle);
                if (connection != null)
                {
                    connection.HangUp();
                }
            }
        }

        private void dialer_StateChanged(object sender, StateChangedEventArgs e)
        {

        }

        private void dialer_DialCompleted(object sender, DialCompletedEventArgs e)
        {
            ErrorMessage = Properties.Resources.DialupConnected;
            if (e.Cancelled)
            {
                dialingresult = (int)SmtpErrors.DialupCancelled;
                ErrorMessage = Properties.Resources.DialupCancelled;
            }
            else if (e.TimedOut)
            {
                dialingresult = (int)SmtpErrors.DialupTimedout;
                ErrorMessage = Properties.Resources.DialupTimedout;
            }
            else if (e.Error != null)
            {
                dialingresult = (int)SmtpErrors.DialupError;
                ErrorMessage = string.Format(Properties.Resources.DialupError, e.Error.ToString());
            }
            else if (e.Connected)
            {
                dialingresult = (int)SmtpErrors.NoError;
            }

            OnSystemEvent(null, e.Error.ToString(), EventSeverity.High, null, null, null,
                        (int)System.Diagnostics.EventLogEntryType.Error, LoggerDestination);
            ConnectionComplete.Set();
        }
#endif
        #endregion RAS dialer utils

        #region Dispose
        protected virtual void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                if (disposing)
                {
                    // TODO: dispose managed state (managed objects)
#if !NET_STANDARD
                    if (DisconnectTimer != null)
                    {
                        DisconnectTimer.Elapsed -= DisconnectTimer_Elapsed;
                        DisconnectTimer.Dispose();
                        DisconnectTimer = null;
                    }
                    if (dialer != null)
                    {
                        dialer.DialCompleted -= dialer_DialCompleted;
                        dialer.StateChanged -= dialer_StateChanged;
                        dialer.Dispose();
                    }
#endif   
                }

                // TODO: free unmanaged resources (unmanaged objects) and override finalizer
                // TODO: set large fields to null
                disposedValue = true;
            }
        }

        // // TODO: override finalizer only if 'Dispose(bool disposing)' has code to free unmanaged resources
        // ~SmtpClientModule()
        // {
        //     // Do not change this code. Put cleanup code in 'Dispose(bool disposing)' method
        //     Dispose(disposing: false);
        // }

        public void Dispose()
        {
            // Do not change this code. Put cleanup code in 'Dispose(bool disposing)' method
            Dispose(disposing: true);
            GC.SuppressFinalize(this);
        }
        #endregion Dispose
    }
}
