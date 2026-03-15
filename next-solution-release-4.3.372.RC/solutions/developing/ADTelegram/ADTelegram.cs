using System;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using ADPluginBase;
using DevExpress.Xpo;
using Opc.Ua;
using Utilities.Logger;
using System.Windows.Forms;
using Telegram.Bot;
using Telegram.Bot.Args;
using Telegram.Bot.Exceptions;
using System.Collections.Generic;

namespace ADTelegram
{
    #region Data
    public enum ADTelegramError : int
    {
        ADTelegramErrorNoError = 0,
        ADTelegramErrorNoConnection = -1,
        ADTelegramErrorWrongNumber = -2,
        ADTelegramConnectionError = -3,
        ADTelegramErrorNoWriteMessage = -8,
        ADTelegramErrorNoMessage = -101
    }
    #endregion


    public class ADTelegramMessageSender : PluginBase
    {
        #region Ctor
        public ADTelegramMessageSender()
        { }
        #endregion

        #region Data and Methods
        private TelegramBotClient NewClient(string Api_Hash)
        {
            return new TelegramBotClient(Api_Hash);
        }
        #endregion

        #region Methods Override
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

        internal int sendResult;
        private async void SendTextMessage(TelegramBotClient bot,string ChatID,string yourMessage)
        {
            try
            {
                await bot.SendTextMessageAsync(ChatID, yourMessage);
                sendResult = (int)ADTelegramError.ADTelegramErrorNoError;
            }
            catch(Exception ex)
            {
                sendResult = (int)ADTelegramError.ADTelegramConnectionError;
                LastError = ex.Message;
            }
        }
        public override bool SendMultiple() { return true; }
        public override int SendMessage(object msg)
        {
            int ReturnValue = (int)ADTelegramError.ADTelegramErrorNoError;
            var list = msg as List<ADPluginBase.Message>;
            var messaggio = msg as ADPluginBase.Message;
            
            if(messaggio != null)
            {
                //send single
                ReturnValue = SendTelegram(messaggio);
                LogSendResult(ReturnValue, messaggio);
            }
            else if (list != null && list.Count > 0)
            {
                //send multiple: group messages by chatID and send one for every chatID.
                var groupedList = (from m in list group m by m.ChatID into newList orderby newList.Key select newList);
                string lastmessage = string.Empty;
                foreach(var groupedUser in groupedList)
                {
                    var p = groupedUser.ToList();
                    foreach(var msge in p)
                    {
                        string myMsg = getMessageToSend(msge);
                        if(myMsg != lastmessage)
                        {
                            lastmessage = myMsg;
                            int nResult = SendTelegram(msge);
                            LogSendResult(nResult, msge);
                            if (nResult != (int)ADTelegramError.ADTelegramErrorNoError)
                                ReturnValue = nResult;
                        }
                    }
                    lastmessage = string.Empty;
                }
            }
            else
            {
                //no messages, error
                OnSystemEvent(null, Properties.Resources.NoMessage, EventSeverity.High, null, null, null,
                        (int)System.Diagnostics.EventLogEntryType.Error, (int)LoggerDestination.AlarmDispatcher);
                LastError = Properties.Resources.NoMessage;
                ReturnValue = (int)ADTelegramError.ADTelegramErrorNoMessage;
            }

            return ReturnValue;
        }
        string getMessageToSend(ADPluginBase.Message messaggio)
        {
            return (messaggio.CustomMessage ? messaggio.Textmessage : $"{messaggio.Name} {messaggio.Reason} - {messaggio.Textmessage} - {messaggio.Alarmmessage}");
        }
        void LogSendResult(int error, ADPluginBase.Message messaggio)
        {
            if (error == (int)ADTelegramError.ADTelegramErrorNoError)
            {
                OnSystemEvent(null, string.Format(Properties.Resources.ADTelegramErrorNoError, messaggio.Textmessage),
                EventSeverity.Min, null, string.Format(Properties.Resources.DetailsFormat, messaggio.Name, messaggio.TagName), messaggio.Reason,
                        (int)System.Diagnostics.EventLogEntryType.Information, (int)LoggerDestination.AlarmDispatcher);
            }
            else if (error == (int)ADTelegramError.ADTelegramConnectionError)
            {
                OnSystemEvent(null, string.Format(Properties.Resources.ADTelegramConnectionError, GetPluginName(), LastError), EventSeverity.High, null,
                    null, null, (int)System.Diagnostics.EventLogEntryType.Error, (int)LoggerDestination.AlarmDispatcher);
            }
        }
        int SendTelegram(ADPluginBase.Message messaggio)
        {
            LastError = string.Empty;
            try
            {
                TelegramBotClient bot = new TelegramBotClient(Token);
                string yourMessage = getMessageToSend(messaggio);
                SendTextMessage(bot, messaggio.ChatID, yourMessage);
                return sendResult;
            }
            catch (System.Exception ex)
            {
                OnSystemEvent(null, string.Format(Properties.Resources.ADTelegramConnectionError, GetPluginName(), ex.Message), EventSeverity.High, null,
                        null, null, (int)System.Diagnostics.EventLogEntryType.Error, (int)LoggerDestination.AlarmDispatcher);
                return (int)ADTelegramError.ADTelegramConnectionError;
            }
        }
        #endregion

        #region Methods
        private bool LoadDriverSettings(PluginSettings configuration)
        {
            if (configuration == null)
                return false;
            _Token= configuration.Token;
            _ChatID = configuration.ChatID;
            return true;
        }
        public void LoadDefaultSettings()
        {
            _Token = string.Empty;
            _ChatID = string.Empty;
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
            settings.ChatID = ChatID;
            settings.Token = Token;

        }
        #endregion

        #region Properties 

        private string _ChatID;
        public string ChatID
        {
            get { return _ChatID; }
            set { _ChatID = value; }
        }

        private string _Token;
        public string Token
        {
            get { return _Token; }
            set { _Token = value; }
        }

        #endregion
    }
}