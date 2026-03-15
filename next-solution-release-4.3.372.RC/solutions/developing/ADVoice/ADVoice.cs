using ADPluginBase;
using DevExpress.Xpo;
using Opc.Ua;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Utilities.Logger;
using Ozeki.Media;
using Ozeki.VoIP;

namespace ADVoice
{
    
    public enum SendErrors : int
    {
        OkOnCharge = PluginBase.OkOnCharge,
        NoError = 0,
        NoMessage = -101,
        NoNumber = -102
    }

    public enum VoiceStates : int
    {
        StateInit,
        StateCall,
        StateInitVoIP,
        StateWaitRegistration,
        StateIntro,
        StateWaitIntro,
        StateMessage,
        StateWaitMessage,
        StateGetNext,
        StateWaitNext,
        StateWaitCall,
        StateBye,
        StateWaitBye,
        StateCallFailed,
        StateAckMessages,
        StateNextAddress,
        StateListMessagesToAck,
        StateWaitIntroPause,
        StateMessagePause,
        StateIntroMessagePause
    }
    public class ADVoice : PluginBase
    {
        #region Ctor
        public ADVoice()
        {
            GroupRecipients = true;
        }
        #endregion Ctor


        #region VoIP
        static ISoftPhone _softphone;   // softphone object
        static IPhoneLine _phoneLine;   // phoneline object
        static IPhoneCall _call;
        static MediaConnector _connector;
        static PhoneCallAudioSender _mediaSender;
        static TextToSpeech _textToSpeech;

        static protected ManualResetEvent RegistrationEvent;
        static protected ManualResetEvent CallStateChangeEvent;

        static bool VoIPOnLine = false;
        int DTMFReceived;
        #endregion VoIP

        #region Const
        public const int DTMFNONE = -1;
        public const int DTMF0 = 0;
        public const int DTMF1 = 1;
        public const int DTMF2 = 2;
        public const int DTMF3 = 3;
        public const int DTMF4 = 4;
        public const int DTMF5 = 5;
        public const int DTMF6 = 6;
        public const int DTMF7 = 7;
        public const int DTMF8 = 8;
        public const int DTMF9 = 9;
        public const int DTMFSTAR = 10;
        public const int DTMFHASHMARK = 11;
        public const int DTMFA = 12;
        public const int DTMFB = 13;
        public const int DTMFC = 14;
        public const int DTMFD = 15;
        #endregion Const

        #region Data
        private object lockListObj = new object();
        List<VoiceMessage> ListVoiceMessagesWaiting = new List<VoiceMessage>();
        List<VoiceMessage> ListCircularVoiceMessages = new List<VoiceMessage>();

        DateTime lastOperationTime;
        DateTime lastPauseTime;
        int currAddress;

        private Thread VoiceSendThread;
        protected ManualResetEvent StopVoiceSendThread;
        protected ManualResetEvent ttsSpeechStoppedEvent;
        protected ManualResetEvent callDTMFReceivedEvent;
        protected readonly object lockThreadObject = new object();
        VoiceStates currentState;
        List<VoiceMessage> currentList = new List<VoiceMessage>();
        VoiceMessage currentMessage = new VoiceMessage();
        VoiceMessage nextMessage = new VoiceMessage();
        List<VoiceMessage> listToAck = new List<VoiceMessage>();
        #endregion Data

        #region methods override
        public override bool Init(string strSettingPath)
        {
            try
            {
                Ozeki.Common.LicenseManager.Instance.SetLicense("OZSDK-PRO1CALL-160930-986F6C06", "TUNDOjEsTVBMOjEsRzcyOTp0cnVlLE1TTEM6MSxNRkM6MSxVUDoyMDE3LjA5LjMwLFA6MjE5OS4wMS4wMXxndkxueXRVamdOTjJxK084N01WU1dCL1dpRFZjYk4yeWxqa2VkMXRyZDZCdjBJOUFnTnVPNDdJcTBvSG1VZHpjalJ3TzZIWGxiUGY1UXNnVzVCNWVLUT09");
            }
            catch(Exception e)
            { }
            
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
                    var conf = (from tag in new XPQuery<ADVoicePluginSettings>(ufw).AsParallel() select tag).ToList();
                    if (conf.Count > 0)
                    {
                        LoadDriverSettings(conf[0]);
                        bRet = true;
                    }
                    else
                    {
                        OnSystemEvent(null, String.Format(Properties.Resources.ErrorLoadingPluginSettings, Properties.Resources.NoConfiguration), EventSeverity.Min, null, null, null,
                        (int)System.Diagnostics.EventLogEntryType.Error, (int)LoggerDestination.AlarmDispatcher);
                        LoadDefaultSettings();
                    }
                }
                catch (InvalidOperationException ex)
                {
                    OnSystemEvent(null, String.Format(Properties.Resources.ErrorLoadingPluginSettings, ex.Message), EventSeverity.Min, null, null, null,
                        (int)System.Diagnostics.EventLogEntryType.Error, (int)LoggerDestination.AlarmDispatcher);
                    LoadDefaultSettings();
                }
            }

            return bRet;
        }

        public override bool SendMultiple() { return true; }
        public override int SendMessage(object msg)
        {
            LastError = string.Empty;
            var messaggio = msg as ADPluginBase.Message;
            var list = msg as List<ADPluginBase.Message>;
            List<ADPluginBase.Message> msgList = new List<ADPluginBase.Message>();

            if ((list == null || list.Count == 0) && messaggio == null)
            {
                OnSystemEvent(null, Properties.Resources.NoMessage, EventSeverity.High, null, null, null,
                        (int)System.Diagnostics.EventLogEntryType.Error, (int)LoggerDestination.AlarmDispatcher);
                LastError = Properties.Resources.NoMessage;
                return (int)SendErrors.NoMessage;
            }

            //check the presence of message phone number
            // for a single recipient, return error, in order to let the pluginThread retry.
            // for list of recipient, check that not all the numbers are void. Signal the user with void number and post.
            // if all the recipients have a void number, return error, in order to let the pluginThread retry.
            if(messaggio != null)
            {
                if (string.IsNullOrEmpty(messaggio.PhoneNumber))
                {
                    LastError = string.Format(Properties.Resources.NoNumber, messaggio.Name, messaggio.Recipient);
                    OnSystemEvent(null, LastError, EventSeverity.High, null, null, null,
                        (int)System.Diagnostics.EventLogEntryType.Error, (int)LoggerDestination.AlarmDispatcher);
                    
                    return (int)SendErrors.NoNumber;
                }
            }
            else
            {
                //list
                var invalid = (from m in list where string.IsNullOrEmpty(m.PhoneNumber) select m).ToList();
                if(invalid.Count > 0)
                {
                    foreach(var m in invalid)
                    {
                        LastError = string.Format(Properties.Resources.NoNumber, m.Name, m.Recipient);
                        OnSystemEvent(null, LastError, EventSeverity.High, null, null, null,
                        (int)System.Diagnostics.EventLogEntryType.Error, (int)LoggerDestination.AlarmDispatcher);
                    }
                    if(invalid.Count == list.Count)
                    {
                        //no valid recipients, return error
                        return (int)SendErrors.NoNumber;
                    }
                    else
                    {
                        //take out invalid recipients from list, signal 
                        foreach (var m in invalid)
                            list.Remove(m);
                    }
                }

            }

            /**/
            // Consistency check on messages, if appropriate...
            /**/
            lock(lockListObj)
            {
                if (messaggio != null)
                    ListVoiceMessagesWaiting.Add(new VoiceMessage(messaggio));
                else if (list != null && list.Count > 0)
                    ListVoiceMessagesWaiting.Add(new VoiceMessage(list));
            }
            

            if (VoiceSendThread == null)
                Startup();

            if (VoiceRunEvent != null)
                VoiceRunEvent.Set();
            
            return (int)SendErrors.OkOnCharge;
        }

        protected override void Dispose(bool disposing)
        {
            StopAll();



            lock (lockThreadObject)
            {
                if (StopVoiceSendThread != null)
                    StopVoiceSendThread.Dispose();
                if (VoiceRunEvent != null)
                    VoiceRunEvent.Dispose();
            }

            base.Dispose(disposing);
        }

        public override void StopPlugin()
        {
            StopAll();

            base.StopPlugin();
        }
        #endregion methods override

        #region Methods

        void StopAll()
        {
            StopThread();

            CloseCallResources();

            ListVoiceMessagesWaiting.Clear();
            ListCircularVoiceMessages.Clear();
    }
        private void CloseCallResources()
        {
            if (CallStateChangeEvent != null)
                CallStateChangeEvent.Dispose();
            CallStateChangeEvent = null;
            if (RegistrationEvent != null)
                RegistrationEvent.Dispose();
            RegistrationEvent = null;

            if (_connector != null)
            {
                _connector.Disconnect(_textToSpeech, _mediaSender);
                _connector.Dispose();
                _connector = null;
            }

            if (_mediaSender != null)
            {
                _mediaSender.Detach();
                _mediaSender.Dispose();
                _mediaSender = null;
            }

            if (_textToSpeech != null)
            {
                _textToSpeech.Stopped -= _textToSpeech_Stopped;
                _textToSpeech.Dispose();
                _textToSpeech = null;
            }

            if (_call != null)
            {
                if (_call.CallState != CallState.Completed)
                    _call.HangUp();

                _call.CallStateChanged -= call_CallStateChanged;
                _call.DtmfReceived -= _call_DtmfReceived;
            }

            if (callDTMFReceivedEvent != null)
                callDTMFReceivedEvent.Dispose();
            callDTMFReceivedEvent = null;


            if (_phoneLine != null)
            {
                _phoneLine.RegistrationStateChanged -= line_RegStateChanged;
                _softphone.UnregisterPhoneLine(_phoneLine);
                _phoneLine.Dispose();
                _phoneLine = null;
            }

            if (_softphone != null)
            {
                _softphone.Close();
                _softphone = null;
            }


            VoIPOnLine = false;
            if (ttsSpeechStoppedEvent != null)
                ttsSpeechStoppedEvent.Dispose();
            ttsSpeechStoppedEvent = null;
        }
        private bool LoadDriverSettings(ADVoicePluginSettings configuration)
        {
            if (configuration == null)
                return false;
            WelcomeMsg = configuration.WelcomeMsg;
            FarewellMsg = configuration.FarewellMsg;
            NextMsg = configuration.NextMsg;
            TimeOut = configuration.TimeOut;
            MaxRetry = configuration.MaxRetry;
            ForceSharpACK = configuration.ForceSharpACK;
            EnableSIPServer = configuration.EnableSIPServer;
            RegistrationRequired = configuration.RegistrationRequired;
            DisplayName = configuration.DisplayName;
            UserName = configuration.UserName;
            RegisterName = configuration.RegisterName;
            RegisterPassword = configuration.RegisterPassword;
            Host = configuration.Host;
            Port = configuration.Port;
            SelectedVoice = configuration.SelectedVoice;
            Volume = configuration.Volume;
            Rate = configuration.Rate;
            Pitch = configuration.Pitch;
            ACKServerAlarm = configuration.ACKServerAlarm;
            Multiplex = configuration.Multiplex;
            RegistrationTimeOut = configuration.RegistrationTimeOut;
            return true;
        }
        public void LoadDefaultSettings()
        {
            WelcomeMsg = "";
            FarewellMsg = "";
            NextMsg = "";
            TimeOut = 60;
            MaxRetry = 3;
            ForceSharpACK = false;
            EnableSIPServer = false;
            RegistrationRequired = true;
            DisplayName = "";
            UserName = "";
            RegisterName = "";
            RegisterPassword = "";
            Host = "";
            Port = 5060;
            SelectedVoice = "";
            Volume = 50;
            Rate = 0;
            Pitch = 0;
            ACKServerAlarm = false;
            Multiplex = false;
            RegistrationTimeOut = 120;
        }
        public void SavePluginSettings(IDataLayer idl)
        {
            using (UnitOfWork ufw = new UnitOfWork(idl))
            {
                var configuration = (from tag in new XPQuery<ADVoicePluginSettings>(ufw).AsParallel() select tag).ToList();

                if (configuration.Count == 0)
                    configuration.Add(new ADVoicePluginSettings(ufw));
                SavePluginSettings(configuration[0]);
                ufw.CommitChanges();
            }
        }
        private void SavePluginSettings(ADVoicePluginSettings settings)
        {
            settings.WelcomeMsg = WelcomeMsg;
            settings.FarewellMsg = FarewellMsg;
            settings.NextMsg = NextMsg;
            settings.TimeOut = TimeOut;
            settings.MaxRetry = MaxRetry;
            settings.ForceSharpACK = ForceSharpACK;
            settings.EnableSIPServer = EnableSIPServer;
            settings.RegistrationRequired = RegistrationRequired;
            settings.DisplayName = DisplayName;
            settings.UserName = UserName;
            settings.RegisterName = RegisterName;
            settings.RegisterPassword = RegisterPassword;
            settings.Host = Host;
            settings.Port = Port;
            settings.SelectedVoice = SelectedVoice;
            settings.Volume = Volume;
            settings.Rate = Rate;
            settings.Pitch = Pitch;
            settings.ACKServerAlarm = ACKServerAlarm;
            settings.Multiplex = Multiplex;
            settings.RegistrationTimeOut = RegistrationTimeOut;
        }

        public void StopThread()
        {
            Thread thread = VoiceSendThread;
            lock (lockThreadObject)
            {
                if (VoiceRunEvent != null)
                    VoiceRunEvent.Set();

                if (StopVoiceSendThread != null)
                    StopVoiceSendThread.Set();
            }
            
            if (thread != null)
                thread.Join();

            //disconnect media
            

        }

        public bool Startup()
        {
            lock (lockThreadObject)
            {
                if (VoiceRunEvent == null)
                    VoiceRunEvent = new ManualResetEvent(false);
                else
                    VoiceRunEvent.Reset();

                if (VoiceSendThread == null)
                    VoiceSendThread = new Thread(VoiceSendThreadProc);

                if (StopVoiceSendThread == null)
                    StopVoiceSendThread = new ManualResetEvent(false);
                else
                    StopVoiceSendThread.Reset();

                if (!VoiceSendThread.IsAlive)
                    VoiceSendThread.Start();

                return VoiceSendThread != null && (VoiceSendThread.ThreadState & (ThreadState.Stopped | ThreadState.Unstarted)) == 0;
            }
        }

        bool GetCurrentMessage()
        {
            currentMessage.Reset();
            lock (lockListObj)
            {
                if (ListVoiceMessagesWaiting.Count > 0)
                {
                    var currMsg = ListVoiceMessagesWaiting[0];
                    currentMessage.Copy(currMsg);
                    ListVoiceMessagesWaiting.RemoveAt(0);
                }
            }
            return currentMessage.IsValid();
        }

        List<VoiceMessage> GetAllMessagesToAck()
        {
            string address = currentMessage.Addresses[currAddress].Address;
            lock (lockListObj)
            {
                if (ListVoiceMessagesWaiting.Count > 0)
                {
                    var nextlist = (from m in ListVoiceMessagesWaiting
                                    where (from a in m.Addresses where a.Address == address select a).ToList().Count > 0
                                    select m).ToList();
                    return nextlist;
                }
            }
            return null;
        }
        bool GetNextMessage()
        {
            string address = currentMessage.Addresses[currAddress].Address;
            lock (lockListObj)
            {
                if (ListVoiceMessagesWaiting.Count > 0)
                {
                    var nextlist = (from m in ListVoiceMessagesWaiting
                                where (from a in m.Addresses where a.Address == address select a).ToList().Count > 0
                                select m).ToList();
                    if(nextlist.Count > 0)
                    {
                        nextMessage.Reset();
                        var msg = nextlist[0];
                        nextMessage.Copy(msg);
                        ListVoiceMessagesWaiting.Remove(msg);
                        return nextMessage.IsValid();
                    }
                }
            }
            return false;
        }

        void AcknowledgeMessage(VoiceMessage msg, int allforAddress = -1)
        {
            if(allforAddress != -1 && msg.Addresses.Count > allforAddress)
            {
                //do for all the message to sent to the address msg.Addresses[allforAddress]
                var nextlist = (from m in ListVoiceMessagesWaiting
                                where (from a in m.Addresses where a.Address == msg.Addresses[allforAddress].Address
                                       select a).ToList().Count > 0
                                select m).ToList();
                foreach (var m in nextlist)
                {
                    foreach (var n in m.NodeIds)
                    {
                        OnSystemEvent(null, String.Format(Properties.Resources.MessageSent, m.Readeable(n)),
                                EventSeverity.Min, null, null, null, (int)System.Diagnostics.EventLogEntryType.Information,
                                (int)LoggerDestination.AlarmDispatcher);
                        OnSendResultEvent(n, (ACKServerAlarm ? PluginBase.OkAckServer : PluginBase.OkSendResult));
                    }
                        
                }
                foreach (var n in msg.NodeIds)
                {
                    OnSystemEvent(null, String.Format(Properties.Resources.MessageSent, msg.Readeable(n)),
                                EventSeverity.Min, null, null, null, (int)System.Diagnostics.EventLogEntryType.Information,
                                (int)LoggerDestination.AlarmDispatcher);
                    OnSendResultEvent(n, (ACKServerAlarm ? PluginBase.OkAckServer : PluginBase.OkSendResult));
                }
                    
            }
            else
            {
                //do for msg only
                foreach (var n in msg.NodeIds)
                {
                    OnSystemEvent(null, String.Format(Properties.Resources.MessageSent, msg.Readeable()),
                                EventSeverity.Min, null, null, null, (int)System.Diagnostics.EventLogEntryType.Information,
                                (int)LoggerDestination.AlarmDispatcher);
                    OnSendResultEvent(n, (ACKServerAlarm ? PluginBase.OkAckServer : PluginBase.OkSendResult));
                }
                    
                
            }
        }

        ManualResetEvent VoiceRunEvent;
        protected virtual void VoiceSendThreadProc(object data)
        {
            int sleepCycle = 10;

            currentState = VoiceStates.StateInitVoIP;
            if (VoiceRunEvent != null)
                VoiceRunEvent.Set();
            lastOperationTime = DateTime.Now;
            System.Diagnostics.Trace.TraceInformation(string.Format("currentState => {0}", currentState.ToString()));
            while (true)
            {
                if(VoiceRunEvent != null)
                    VoiceRunEvent.WaitOne();
                
                switch(currentState)
                {
                    case VoiceStates.StateInitVoIP:
                        
                        if (InitVoIP())
                        {
                            currentState = VoiceStates.StateInit;
                            System.Diagnostics.Trace.TraceInformation(string.Format("VoiceStates.StateInitVoIP => {0}", currentState.ToString()));
                        }
                        else
                        {
                            currentState = VoiceStates.StateWaitRegistration;
                            System.Diagnostics.Trace.TraceInformation(string.Format("VoiceStates.StateInitVoIP => {0}", currentState.ToString()));
                            lastOperationTime = DateTime.Now;
                        }
                        break;
                    case VoiceStates.StateWaitRegistration:
                        if (((DateTime.Now - lastOperationTime).TotalMilliseconds / 1000) > RegistrationTimeOut)
                        {
                            currentState = VoiceStates.StateInitVoIP;
                            System.Diagnostics.Trace.TraceInformation(string.Format("VoiceStates.StateWaitRegistration => {0}", currentState.ToString()));
                            CloseCallResources();
                        }
                        break;
                    case VoiceStates.StateInit:
                        if(GetCurrentMessage())
                        {
                            //got a message to send!!
                            currentState = VoiceStates.StateCall;
                            System.Diagnostics.Trace.TraceInformation(string.Format("VoiceStates.StateInit => {0}", currentState.ToString()));
                            currAddress = 0;
                            OnSystemEvent(null, String.Format(Properties.Resources.SendMessage, currentMessage.Readeable(), currentMessage.Addresses[currAddress].Address), 
                                EventSeverity.Min, null, null, null, (int)System.Diagnostics.EventLogEntryType.Information, 
                                (int)LoggerDestination.AlarmDispatcher);
                            if(ForceSharpACK)
                            {
                                ListCircularVoiceMessages.Clear();
                                ListCircularVoiceMessages.Add(currentMessage);
                            }
                            
                        }
                        else
                        {
                            //currentMessage is invalid, if present
                            if(!currentMessage.IsEmpty())
                            {
                                OnSystemEvent(null, String.Format(Properties.Resources.MessageInvalid, currentMessage.Readeable()),
                                EventSeverity.Min, null, null, null, (int)System.Diagnostics.EventLogEntryType.Information,
                                (int)LoggerDestination.AlarmDispatcher);
                            }
                            if (VoiceRunEvent != null && !StopVoiceSendThread.WaitOne(0))
                            {
                                VoiceRunEvent.Reset();
                                continue;
                            }
                                
                        }
                        break;

                    case VoiceStates.StateCall:
                        currentMessage.Done = false;

                        DialCall();
                        if (GetCallState() == CallState.Answered || GetCallState() == CallState.InCall)
                        {
                            //call answered, speech message
                            currentState = VoiceStates.StateIntro;
                            lastOperationTime = DateTime.Now;//will set timeout check...
                            System.Diagnostics.Trace.TraceInformation(string.Format("VoiceStates.StateCall => {0}", currentState.ToString()));
                        }
                        else
                        //call error, go to next address, if any. Otherwise log error and go to next message.
                        {
                            currentState = VoiceStates.StateCallFailed;
                        }
                        break;
                    case VoiceStates.StateCallFailed:
                        OnSystemEvent(null, String.Format(Properties.Resources.CallFailed, currentMessage.Addresses[currAddress].Address, GetCallState()),
                               EventSeverity.Min, null, null, null, (int)System.Diagnostics.EventLogEntryType.Error,
                               (int)LoggerDestination.AlarmDispatcher);
                        currentMessage.Addresses[currAddress].Error++;
                        currentState = VoiceStates.StateNextAddress;
                        CloseCall();
                        break;
                    case VoiceStates.StateNextAddress:
                        {
                            int last = (currentMessage.Done ? nextMessage.GetLastIndex() : currentMessage.GetLastIndex());
                            if (currAddress == last)
                            {
                                //reached the last address, test retry
                                int rtr;
                                if (currentMessage.Done)
                                {
                                    rtr = ++nextMessage.Retry;
                                }
                                else
                                {
                                    rtr = ++currentMessage.Retry;
                                }
                                
                                if (rtr >= MaxRetry)
                                {
                                    //if retry exceeded, error, delete message
                                    OnSystemEvent(null, String.Format(Properties.Resources.MessageRetryExceeded, (currentMessage.Done ? nextMessage.Readeable() : currentMessage.Readeable())),
                                        EventSeverity.Min, null, null, null, (int)System.Diagnostics.EventLogEntryType.Error,
                                        (int)LoggerDestination.AlarmDispatcher);
                                    
                                    if (currentMessage.Done)
                                    {
                                        foreach (var n in nextMessage.NodeIds)
                                            OnSendResultEvent(n, -1);
                                        nextMessage.Reset();
                                    }
                                    else
                                    {
                                        foreach (var n in currentMessage.NodeIds)
                                            OnSendResultEvent(n, -1);
                                        currentMessage.Reset();
                                    }
                                }
                                else
                                {
                                    VoiceMessage m = new VoiceMessage();
                                    if (currentMessage.Done)
                                        m.Copy(nextMessage);
                                    else
                                        m.Copy(currentMessage);
                                    ListVoiceMessagesWaiting.Add(m);
                                }
                                currentState = VoiceStates.StateInit;
                                break;
                            }
                            currAddress = (currentMessage.Done ? nextMessage.GetNextCurrentAddress(currAddress) : currentMessage.GetNextCurrentAddress(currAddress));
                            currentState = VoiceStates.StateWaitCall;
                        }
                        break;
                    case VoiceStates.StateWaitCall:
                        //add a delay, is appropriated...
                        currentState = VoiceStates.StateCall;
                        System.Diagnostics.Trace.TraceInformation(string.Format("VoiceStates.StateWaitCall => {0}", currentState.ToString()));
                        break;
                    case VoiceStates.StateIntro:
                        SpeakText(WelcomeMsg);
                        currentState = VoiceStates.StateWaitIntro;
                        System.Diagnostics.Trace.TraceInformation(string.Format("VoiceStates.StateIntro => {0}", currentState.ToString()));
                        
                        DTMFReceived = DTMFNONE;
                        break;
                    case VoiceStates.StateWaitIntro:
                        if (callDTMFReceivedEvent.WaitOne(0))
                        {
                            callDTMFReceivedEvent.Reset();
                            StopSpeak();
                            currentState = VoiceStates.StateIntroMessagePause;
                            lastPauseTime = DateTime.Now;
                            System.Diagnostics.Trace.TraceInformation(string.Format("VoiceStates.StateWaitIntro => {0}", currentState.ToString()));
                            break;
                        }
                        if(ttsSpeechStoppedEvent.WaitOne(0))
                        {
                            StopSpeak();
                            currentState = VoiceStates.StateWaitIntroPause/*StateIntro*/;
                            lastPauseTime = DateTime.Now;
                        }
                        //timeout...
                        break;
                    case VoiceStates.StateWaitIntroPause:
                        if (((DateTime.Now - lastPauseTime).TotalMilliseconds > 1500))
                        {
                            currentState = VoiceStates.StateIntro;
                        }
                        if(((DateTime.Now - lastPauseTime).TotalMilliseconds > 500) && callDTMFReceivedEvent.WaitOne(0))
                        {
                            callDTMFReceivedEvent.Reset();
                            currentState = VoiceStates.StateIntroMessagePause;
                            lastPauseTime = DateTime.Now;
                        }
                        break;
                    case VoiceStates.StateIntroMessagePause:
                        //from Intro to Message
                        if ((DateTime.Now - lastPauseTime).TotalMilliseconds > 1500 )
                        {
                            currentState = VoiceStates.StateMessage;
                            lastOperationTime = DateTime.Now;//will set timeout check...
                        }
                        break;
                    case VoiceStates.StateMessagePause:
                        //before the repeat of a message, ack and  so on is legal...
                        if ((DateTime.Now - lastPauseTime).TotalMilliseconds > 1500)
                        {
                            currentState = VoiceStates.StateMessage;
                        }
                        CheckDTMFWaitMessage();
                        break;
                    case VoiceStates.StateMessage:
                        SpeakText((currentMessage.Done ? nextMessage.Text : currentMessage.Text));
                        if (nextMessage.IsValid())
                            nextMessage.IncrementCall(currentMessage.Addresses[currAddress].Address);
                        currentState = VoiceStates.StateWaitMessage;
                        System.Diagnostics.Trace.TraceInformation(string.Format("VoiceStates.StateMessage => {0}", currentState.ToString()));
                        DTMFReceived = DTMFNONE;
                        break;
                    case VoiceStates.StateWaitMessage:
                        if (CheckDTMFWaitMessage() > 0)
                            break;
                        
                        if (ttsSpeechStoppedEvent.WaitOne(0))
                        {
                            StopSpeak();
                            currentState = VoiceStates.StateMessagePause;
                            lastPauseTime = DateTime.Now;
                        }
                        break;
                    case VoiceStates.StateGetNext:
                        if(GetNextMessage())
                        {
                            //currentMessage updated to the next, play new text
                            currentMessage.Done = true;
                            System.Threading.Thread.Sleep(500);
                            SpeakText(NextMsg);
                            currentState = VoiceStates.StateWaitNext;
                            OnSystemEvent(null, String.Format(Properties.Resources.SendMessage, nextMessage.Readeable(), nextMessage.Addresses[currAddress].Address),
                               EventSeverity.Min, null, null, null, (int)System.Diagnostics.EventLogEntryType.Information,
                               (int)LoggerDestination.AlarmDispatcher);
                            System.Diagnostics.Trace.TraceInformation(string.Format("VoiceStates.StateGetNext 0 => {0}", currentState.ToString()));
                            lastOperationTime = DateTime.Now;//will set timeout check...
                            if (ForceSharpACK)
                                ListCircularVoiceMessages.Add(nextMessage);
                        }
                        else
                        {
                            //No next message...
                            if(ForceSharpACK)
                            {
                                if(ListCircularVoiceMessages.Count > 0)
                                {
                                    nextMessage = ListCircularVoiceMessages[0];
                                    ListCircularVoiceMessages.RemoveAt(0);
                                    ListCircularVoiceMessages.Add(nextMessage);
                                }
                                //continue playing the current message
                                if(currentMessage.IsValid() || nextMessage.IsValid())
                                {
                                    SpeakText(NextMsg);
                                    currentState = VoiceStates.StateWaitNext;
                                    System.Diagnostics.Trace.TraceInformation(string.Format("VoiceStates.StateGetNext 1 => {0}", currentState.ToString()));
                                }
                                else
                                {
                                    //close call, invalid next message...
                                    StopSpeak();
                                    currentState = VoiceStates.StateBye;
                                    System.Diagnostics.Trace.TraceInformation(string.Format("VoiceStates.StateGetNext 2 => {0}", currentState.ToString()));
                                }
                            }
                            else
                            {
                                //close call, no next message
                                StopSpeak();
                                currentState = VoiceStates.StateBye;
                                System.Diagnostics.Trace.TraceInformation(string.Format("VoiceStates.StateGetNext => {0}", currentState.ToString()));
                            }
                        }
                        break;
                    case VoiceStates.StateWaitNext:
                        if (ttsSpeechStoppedEvent.WaitOne(0))
                        {
                            StopSpeak();
                            currentState = VoiceStates.StateMessage;
                            lastOperationTime = DateTime.Now;
                            System.Diagnostics.Trace.TraceInformation(string.Format("VoiceStates.StateWaitNext => {0}", currentState.ToString()));
                        }
                        break;
                    case VoiceStates.StateBye:
                        System.Threading.Thread.Sleep(500);
                        SpeakText(FarewellMsg);
                        currentState = VoiceStates.StateWaitBye;
                        System.Diagnostics.Trace.TraceInformation(string.Format("VoiceStates.StateBye => {0}", currentState.ToString()));
                        lastOperationTime = DateTime.Now;//will set timeout check...
                        break;
                    case VoiceStates.StateWaitBye:
                        if (ttsSpeechStoppedEvent.WaitOne(0))
                        {
                            System.Threading.Thread.Sleep(500);
                            StopSpeak();
                            CloseCall();
                            currentState = VoiceStates.StateAckMessages;
                            System.Diagnostics.Trace.TraceInformation(string.Format("VoiceStates.StateWaitBye => {0}", currentState.ToString()));
                        }
                        break;
                    case VoiceStates.StateListMessagesToAck:
                        //add all the eligible messages to the list of being acknowledged
                        string address = currentMessage.Addresses[currAddress].Address;
                        var nextlist = (from m in ListVoiceMessagesWaiting
                                        where (from a in m.Addresses where a.Address == address select a).ToList().Count > 0
                                        select m).ToList();
                        if (nextlist.Count > 0)
                            listToAck.AddRange(nextlist);
                        foreach (var m in nextlist)
                        {
                            if (ListVoiceMessagesWaiting.Contains(m))
                                ListVoiceMessagesWaiting.Remove(m);
                        }
                        //Acknowledge all the messages for the address and close call
                        currentState = VoiceStates.StateBye;
                        break;
                    case VoiceStates.StateAckMessages:
                        if(listToAck.Count > 0)
                        {
                            foreach(var m in listToAck)
                            {
                                if(Multiplex && !m.SentToAll())
                                {
                                    //back in queue again, to  send to remaining recipients
                                    VoiceMessage callagain = new VoiceMessage();
                                    callagain.Copy(m);
                                    ListVoiceMessagesWaiting.Add(callagain);
                                }
                                else
                                {
                                    AcknowledgeMessage(m);
                                }
                            }
                        }
                        //do it also for currentMessage
                        if (Multiplex && !currentMessage.SentToAll())
                        {
                            //not sent to all, test for next address...
                            VoiceMessage callagain = new VoiceMessage();
                            callagain.Copy(currentMessage);
                            ListVoiceMessagesWaiting.Add(callagain);
                        }
                        else
                        {
                            AcknowledgeMessage(currentMessage);
                        }
                        currentState = VoiceStates.StateInit;
                        break;
                }
                

                //timeout
                if(currentState == VoiceStates.StateWaitIntro || 
                    currentState == VoiceStates.StateWaitMessage || 
                    currentState == VoiceStates.StateWaitNext ||
                    currentState == VoiceStates.StateWaitBye)
                {
                    if (((DateTime.Now - lastOperationTime).TotalMilliseconds / 1000) > TimeOut)
                    {
                        //elapsed, abort this send operation
                        StopSpeak();
                        if (currentState == VoiceStates.StateWaitIntro)
                        {
                            currentState = VoiceStates.StateCallFailed;
                        }
                        else if (currentState == VoiceStates.StateWaitMessage)
                        {
                            if(nextMessage.IsValid())
                            {
                                StopSpeak();
                                CloseCall();
                                currentState = VoiceStates.StateAckMessages;
                            }
                            else
                            {
                                currentState = VoiceStates.StateCallFailed;
                            }
                        }
                        else if (currentState == VoiceStates.StateWaitNext)
                        {
                            currentState = VoiceStates.StateMessage;
                            lastOperationTime = DateTime.Now;
                            System.Diagnostics.Trace.TraceInformation(string.Format("timeout => {0}", currentState.ToString()));
                        }
                        else if (currentState == VoiceStates.StateWaitBye)
                        {
                            CloseCall();
                            currentState = VoiceStates.StateAckMessages;
                            System.Diagnostics.Trace.TraceInformation(string.Format("timeout => {0}", currentState.ToString()));
                        }
                        else
                        {
                            //never reach this code...
                            //call error, go to next address, if any. Otherwise log error and go to next message.
                            
                            CloseCall();
                            if (++currAddress == currentMessage.Addresses.Count)
                            {
                                //error, test retry.
                                if (++currentMessage.Retry > MaxRetry)
                                {
                                    //if retry exceeded, error, delete message
                                    OnSystemEvent(null, String.Format(Properties.Resources.ErrorSendingMessageRetry, currentMessage.Text),
                                        EventSeverity.Min, null, null, null, (int)System.Diagnostics.EventLogEntryType.Error,
                                        (int)LoggerDestination.AlarmDispatcher);

                                    foreach (var n in currentMessage.NodeIds)
                                        OnSendResultEvent(n, -1);

                                    currentMessage.Reset();
                                    currentState = VoiceStates.StateInit;
                                    System.Diagnostics.Trace.TraceInformation(string.Format("timeout 1 => {0}", currentState.ToString()));
                                }
                                else
                                {
                                    //if not, make another call for the message
                                    OnSystemEvent(null, String.Format(Properties.Resources.ErrorSendingMessage, currentMessage.Text, currentMessage.Retry),
                                        EventSeverity.Min, null, null, null, (int)System.Diagnostics.EventLogEntryType.Error,
                                        (int)LoggerDestination.AlarmDispatcher);
                                    currAddress = 0;
                                    currentState = VoiceStates.StateWaitCall;
                                    System.Diagnostics.Trace.TraceInformation(string.Format("timeout 2 => {0}", currentState.ToString()));
                                }
                            }
                            else
                            {
                                //call the next address
                                currentState = VoiceStates.StateWaitCall;
                                System.Diagnostics.Trace.TraceInformation(string.Format("timeout 3 => {0}", currentState.ToString()));
                            }
                            
                        }

                    }
                }


                if (StopVoiceSendThread.WaitOne(sleepCycle, false))
                    break;
            }

            System.Diagnostics.Trace.TraceInformation("Terminating Voice Working thread!");
        }

        public int CheckDTMFWaitMessage()
        {
            if (callDTMFReceivedEvent.WaitOne(0))
            {
                callDTMFReceivedEvent.Reset();
                if (DTMFReceived == DTMFHASHMARK)
                {
                    StopSpeak();
                    currentState = VoiceStates.StateListMessagesToAck;
                    currentMessage.Done = true;
                    if (currentMessage.Done && nextMessage.IsValid())
                        listToAck.Add(nextMessage);
                    var others = GetAllMessagesToAck();
                    if (others != null && others.Count > 0)
                        listToAck.AddRange(others);

                    System.Diagnostics.Trace.TraceInformation(string.Format("VoiceStates.StateWaitMessage => {0}", currentState.ToString()));
                    return 1;
                }
                else if (DTMFReceived != DTMFNONE)
                {
                    StopSpeak();

                    if (ForceSharpACK)
                    {
                        //look for next
                        currentState = VoiceStates.StateGetNext;
                        System.Diagnostics.Trace.TraceInformation(string.Format("VoiceStates.StateWaitMessage 0 => {0}", currentState.ToString()));
                    }
                    else
                    {
                        currentMessage.Done = true;
                        //Acknowledge message
                        if (currentMessage.Done && nextMessage.IsValid())
                            listToAck.Add(nextMessage);

                        //look for next
                        currentState = VoiceStates.StateGetNext;
                        System.Diagnostics.Trace.TraceInformation(string.Format("VoiceStates.StateWaitMessage 1 => {0}", currentState.ToString()));
                    }
                    return 2;
                }
            }
            return 0;
        }
        #endregion Methods


        #region VoIP methods
        bool InitVoIP()
        {
            
            if(!VoIPOnLine)
            {
                if (RegistrationEvent == null)
                    RegistrationEvent = new ManualResetEvent(false);
                else
                    RegistrationEvent.Reset();

                if (CallStateChangeEvent == null)
                    CallStateChangeEvent = new ManualResetEvent(false);
                else
                    CallStateChangeEvent.Reset();

                if (callDTMFReceivedEvent == null)
                    callDTMFReceivedEvent = new ManualResetEvent(false);
                else
                    callDTMFReceivedEvent.Reset();

                if (ttsSpeechStoppedEvent == null)
                    ttsSpeechStoppedEvent = new ManualResetEvent(false);
                else
                    ttsSpeechStoppedEvent.Reset();

                // Create a softphone object with RTP port range 5000-10000
                if (_softphone == null)
                {
                    _softphone = SoftPhoneFactory.CreateSoftPhone(5000, 10000);

                    //Enable only voice and DTFM events
                    foreach(var c in _softphone.Codecs)
                        _softphone.DisableCodec(c.CodecType);
                    _softphone.EnableCodec(CodecPayloadType.PCMA);
                    _softphone.EnableCodec(CodecPayloadType.telephone_event);
                }
                try
                {
                    if(EnableSIPServer)
                    {
                        var registrationRequired = RegistrationRequired;
                        var userName = UserName;
                        var displayName = (string.IsNullOrEmpty(DisplayName) ? UserName : DisplayName);
                        var authenticationId = (string.IsNullOrEmpty(RegisterName) ? UserName : RegisterName);
                        var registerPassword = RegisterPassword;
                        var domainHost = Host;
                        var domainPort = Port;
                        var account = new SIPAccount(registrationRequired, displayName, userName, authenticationId, registerPassword, domainHost, domainPort);

                        _phoneLine = _softphone.CreatePhoneLine(account);
                    }
                    else
                    {
                        var config = new DirectIPPhoneLineConfig(SoftPhoneFactory.GetLocalIP().ToString(), Port);
                        _phoneLine = _softphone.CreateDirectIPPhoneLine(config);
                    }
                        
                    _phoneLine.RegistrationStateChanged += line_RegStateChanged;
                    _softphone.RegisterPhoneLine(_phoneLine);

                    var sub = _phoneLine.Subscription.Create(SIPEventType.MessageSummary);
                    _phoneLine.Subscription.Subscribe(sub);
                }
                catch (Exception ex)
                {
                    //log error Console.WriteLine("Error during SIP registration: " + ex);
                    VoIPOnLine = false;
                    return VoIPOnLine;
                }

                RegistrationEvent.WaitOne((int)(TimeOut * 1000), false);
                if(_mediaSender == null)
                    _mediaSender = new PhoneCallAudioSender();
                if(_connector == null)
                    _connector = new MediaConnector();

                if (_textToSpeech == null)
                {
                    _textToSpeech = new TextToSpeech();
                    _textToSpeech.Stopped += _textToSpeech_Stopped;
                }
                    
                var listvoices = _textToSpeech.GetAvailableVoices();
                //error, no voices!!!
                if (listvoices.Count > 0)
                {
                    var setupvoice = (from v in listvoices where v.Name == SelectedVoice select v).ToList();
                    if (setupvoice.Count > 0)
                    {
                        _textToSpeech.ChangeVoice(SelectedVoice);
                        _textToSpeech.Rate = Rate;
                    }
                    else
                        VoIPOnLine = false;
                }
                else
                    VoIPOnLine = false;

            }
                
            
            return VoIPOnLine;
        }

        private void _textToSpeech_Stopped(object sender, EventArgs e)
        {
            System.Diagnostics.Trace.TraceInformation(string.Format("|-->_textToSpeech_Stopped {0}", DateTime.Now.ToLongTimeString()));
            if(ttsSpeechStoppedEvent != null)
                ttsSpeechStoppedEvent.Set();
        }

        static void line_RegStateChanged(object sender, RegistrationStateChangedArgs e)
        {
            if (e.State == RegState.NotRegistered || e.State == RegState.Error)
            {
                // log error Console.WriteLine("Registration failed!");
                VoIPOnLine = false;
                if(RegistrationEvent != null)
                    RegistrationEvent.Set();
            }
                

            if (e.State == RegState.RegistrationSucceeded)
            {
                Console.WriteLine("Registration succeeded - Online!");
                VoIPOnLine = true;
                if (RegistrationEvent != null)
                    RegistrationEvent.Set();
            }
        }

        void DialCall()
        {
            CallStateChangeEvent.Reset();
            var numberToDial = currentMessage.Addresses[currAddress].Address;
            currentMessage.Addresses[currAddress].Call++;
            try
            {
                if (EnableSIPServer)
                    _call = _softphone.CreateCallObject(_phoneLine, numberToDial);
                else
                    _call = _softphone.CreateDirectIPCallObject(_phoneLine, new DirectIPDialParameters(Port.ToString()), numberToDial);
                _call.CallStateChanged += call_CallStateChanged;
                _call.DtmfReceived += _call_DtmfReceived;
                _call.Start();
                CallStateChangeEvent.WaitOne((int)(TimeOut * 1000), false);
            }
            catch(Exception ex)
            {
                //trace error...
                OnSystemEvent(null, String.Format(Properties.Resources.ErrorDialCall, currentMessage.Readeable(), currentMessage.Addresses[currAddress].Address, ex.Message),
                                EventSeverity.High, null, null, null, (int)System.Diagnostics.EventLogEntryType.Error,
                                (int)LoggerDestination.AlarmDispatcher);
            }
        }


        void CloseCall()
        {
            if(_call != null)
            {
                _call.HangUp();
                _call.CallStateChanged -= call_CallStateChanged;
                _call.DtmfReceived -= _call_DtmfReceived;
            }
        }
        CallState GetCallState()
        {
            if (_call != null)
                return _call.CallState;
            return CallState.Error;
        }
        static void call_CallStateChanged(object sender, CallStateChangedArgs e)
        {
            if (e.State == CallState.Answered)
            {
                CallStateChangeEvent.Set();
                _mediaSender.AttachToCall(_call);
                
            }
                
        }

        private void _call_DtmfReceived(object sender, VoIPEventArgs<DtmfInfo> e)
        {
            DTMFReceived = e.Item.Signal.Signal;
            if(callDTMFReceivedEvent != null)
                callDTMFReceivedEvent.Set();
        }

        void SpeakText(string text)
        {
            try
            {
                if (_textToSpeech == null)
                {
                    _textToSpeech = new TextToSpeech();
                    _textToSpeech.Stopped += _textToSpeech_Stopped;
                    _textToSpeech.ChangeVoice(SelectedVoice);
                    _textToSpeech.Rate = Rate;
                }

                callDTMFReceivedEvent.Reset();
                ttsSpeechStoppedEvent.Reset();
                if (!_connector.IsConnected(_textToSpeech, _mediaSender))
                    _connector.Connect(_textToSpeech, _mediaSender);

                _textToSpeech.AddAndStartText(text);

                System.Diagnostics.Trace.TraceInformation(string.Format("|-->SpeakText {0}", DateTime.Now.ToLongTimeString()));
            }
            catch (Exception ex)
            {
                //trace error...
                OnSystemEvent(null, String.Format(Properties.Resources.ErrorSendingMessage, currentMessage.Readeable(), currentMessage.Addresses[currAddress].Address, ex.Message),
                                EventSeverity.High, null, null, null, (int)System.Diagnostics.EventLogEntryType.Error,
                                (int)LoggerDestination.AlarmDispatcher);
            }
        }
        
        void StopSpeak()
        {
            

            if (_textToSpeech != null)
            {
                _textToSpeech.Stop();
                _textToSpeech.Clear();
                _connector.Disconnect(_textToSpeech, _mediaSender);
            }
                
            
        }

        #endregion VoIP methods

        #region Properties

        private string _WelcomeMsg;
        public string WelcomeMsg
        {
            get { return _WelcomeMsg; }
            set { _WelcomeMsg = value; }
        }

        private string _FarewellMsg;
        public string FarewellMsg
        {
            get { return _FarewellMsg; }
            set { _FarewellMsg = value; }
        }
        private string _NextMsg;
        public string NextMsg
        {
            get { return _NextMsg; }
            set { _NextMsg = value; }
        }
        private UInt32 _TimeOut;
        public UInt32 TimeOut
        {
            get { return _TimeOut; }
            set { _TimeOut = value; }
        }
        private uint _MaxRetry;
        public uint MaxRetry
        {
            get { return _MaxRetry; }
            set { _MaxRetry = value; }
        }
        private bool _ForceSharpACK;
        public bool ForceSharpACK
        {
            get { return _ForceSharpACK; }
            set { _ForceSharpACK = value; }
        }
        /*-- SIP SERVER --*/
        private bool _EnableSIPServer;//if false, make direct call: requires Host.
        public bool EnableSIPServer
        {
            get { return _EnableSIPServer; }
            set { _EnableSIPServer = value; }
        }
        private bool _RegistrationRequired;//Registration required for this account.
        public bool RegistrationRequired
        {
            get { return _RegistrationRequired; }
            set { _RegistrationRequired = value; }
        }
        private string _DisplayName;//Display name for the account.
        public string DisplayName
        {
            get { return _DisplayName; }
            set { _DisplayName = value; }
        }
        private string _UserName;//The username for the SIP account.
        public string UserName
        {
            get { return _UserName; }
            set { _UserName = value; }
        }
        private string _RegisterName;//The authorization name for the SIP account.
        public string RegisterName
        {
            get { return _RegisterName; }
            set { _RegisterName = value; }
        }
        private string _RegisterPassword;//The password for the SIP account.
        public string RegisterPassword
        {
            get { return _RegisterPassword; }
            set { _RegisterPassword = value; }
        }
        private string _Host;//The domain host for SIP registration.
        public string Host
        {
            get { return _Host; }
            set { _Host = value; }
        }
        private Int32 _Port;//The domain host for SIP registration.
        public Int32 Port
        {
            get { return _Port; }
            set { _Port =  value; }
        }

        private UInt32 _RegistrationTimeOut;
        public UInt32 RegistrationTimeOut
        {
            get { return _RegistrationTimeOut; }
            set { _RegistrationTimeOut = value; }
        }

        private string _SelectedVoice;
        public string SelectedVoice
        {
            get { return _SelectedVoice; }
            set { _SelectedVoice = value; }
        }
        private int _Volume;
        public int Volume
        {
            get { return _Volume; }
            set { _Volume = value; }
        }
        private int _Rate;
        public int Rate
        {
            get { return _Rate; }
            set { _Rate =  value; }
        }
        private int _Pitch;
        public int Pitch
        {
            get { return _Pitch; }
            set { _Pitch = value; }
        }
        private bool _ACKServerAlarm;
        public bool ACKServerAlarm
        {
            get { return _ACKServerAlarm; }
            set { _ACKServerAlarm = value; }
        }
        private bool _Multiplex;
        public bool Multiplex
        {
            get { return _Multiplex; }
            set { _Multiplex = value; }
        }
        #endregion Properties
    }
}
