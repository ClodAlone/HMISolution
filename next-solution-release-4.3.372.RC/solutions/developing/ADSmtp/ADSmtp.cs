using System;
using System.Linq;
using ADPluginBase;
using DevExpress.Xpo;
using Opc.Ua;
using Utilities.Logger;
using SmtpSender;

namespace ADSmtp
{
    public enum SendErrors : int
    {
        NoError = 0,
        NoMessage = -101,
        InvalidSyntaxMessage = -5
    }

    public class ADSmtp : PluginBase
    {
        #region Ctor
        public ADSmtp()
        {
            
        }
        #endregion

        #region Data
        ServerSettings serverSettings;
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
                    var conf = (from tag in new XPQuery<ADSmtpPluginSettings>(ufw).AsParallel() select tag).ToList();
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

        public override int SendMessage(object msg)
        {
            
            LastError = string.Empty;
            var messaggio = msg as ADPluginBase.Message;
            if (messaggio == null)
            {
                OnSystemEvent(null, Properties.Resources.NoMessage, EventSeverity.High, null, null, null,
                        (int)System.Diagnostics.EventLogEntryType.Error, (int)LoggerDestination.AlarmDispatcher);
                LastError = Properties.Resources.NoMessage;
                return (int)SendErrors.NoMessage;
            }

            if (serverSettings.From.Length == 0)
                serverSettings.From = Properties.Resources.DefaultFrom;

            string address = messaggio.Email;

            //from~subject~message~attach1~attavh2~...
            string subject = string.Format(Properties.Resources.DefaultSubject, messaggio.Name, messaggio.Reason);
            string message = (messaggio.CustomMessage ?
                $"{serverSettings.From}~{subject}~{messaggio.Textmessage}" : 
                $"{serverSettings.From}~{subject}~{messaggio.Name} {messaggio.Reason} - {messaggio.Textmessage}\n {messaggio.Alarmmessage}");

            if (messaggio.Attachments != null && messaggio.Attachments.Length > 0)
            {
                var attachments = messaggio.Attachments.Split(';');
                if(attachments.Length > 0)
                    foreach (var file in attachments)
                    {
                        message += string.Format("~{0}", file);
                    }
            }

            string[] components = message.Split('~'); //from~subject~message~attach1~attavh2~...
            if (components.Length < 3)
            {
                OnSystemEvent(null, Properties.Resources.InvalidSyntaxMessage, EventSeverity.High, null, string.Format(Properties.Resources.DetailsFormat, messaggio.Name, messaggio.TagName), messaggio.Reason,
                        (int)System.Diagnostics.EventLogEntryType.Error, (int)LoggerDestination.AlarmDispatcher);
                LastError = Properties.Resources.InvalidSyntaxMessage;
                return (int)SendErrors.InvalidSyntaxMessage;
            }
            
            if (components[0].Length == 0 && messaggio.CustomMessage)
            {
                components[0] = serverSettings.From;
            }

            MailSettings mail = new MailSettings();
            mail.From = serverSettings.From;
            mail.Address.Add(address);
            mail.Subject = components[1];
            mail.Message = components[2];

            mail.Name = string.Format(Properties.Resources.DetailsFormat, messaggio.Name, messaggio.TagName);
            mail.Reason = messaggio.Reason;
            
            if (components.Length > 3)
            {
                //attachments
                for (int i = 3; i < components.Length; i++)
                {
                    mail.AddAttachmentFile(components[i]);
                }
            }

            int nResult = (int)SendErrors.NoError;
            using (var smtpclient = new SmtpClientModule())
            {
                smtpclient.Init(serverSettings);
                smtpclient.LoggerDestination = (int)LoggerDestination.AlarmDispatcher;
                smtpclient.LoggerSource = ADPluginBase.Properties.Resources.LoggerSource;
                smtpclient.ModuleName = GetPluginName();
                smtpclient.SystemEvent += PG_SystemEvent;
                nResult = (int)smtpclient.SendMail(mail);
                LastError = smtpclient.ErrorMessage;
            }

            if(nResult == 0 && messaggio.NodeId != null)
                OnSystemEvent(messaggio.NodeId, string.Format(Properties.Resources.OkSendMessage, (string.IsNullOrEmpty(messaggio.Textmessage) ? messaggio.Alarmmessage : messaggio.Textmessage), messaggio.Recipient, messaggio.Email),EventSeverity.Min, null, string.Format(Properties.Resources.DetailsFormat, messaggio.Name, messaggio.TagName), messaggio.Reason,
                        (int)System.Diagnostics.EventLogEntryType.Information, (int)LoggerDestination.AlarmDispatcher);
            return nResult;
        }
        #endregion

        #region Methods

        


        private bool LoadDriverSettings(ADSmtpPluginSettings configuration)
        {
            if (configuration == null)
                return false;
            if(serverSettings == null)
                serverSettings = new ServerSettings();

            serverSettings.ServerAddress = configuration.ServerAddress;
            serverSettings.ServerPort = configuration.ServerPort;
            serverSettings.User = configuration.User;
            serverSettings.Password = configuration.Password;
            serverSettings.From = configuration.From;

            serverSettings.RasEnable = configuration.RasEnable;
            serverSettings.DialupEntry = configuration.DialupEntry;
            serverSettings.PhoneNumber = configuration.PhoneNumber;
            serverSettings.RASUser = configuration.RASUser;
            serverSettings.RASPassword = configuration.RASPassword;
            serverSettings.RetryTime = configuration.RetryTime;
            serverSettings.DisconnectAfter = configuration.DisconnectAfter;
            serverSettings.Retries = configuration.Retries;
            serverSettings.PhonebookPath = configuration.PhonebookPath;

            return true;
        }
        public void LoadDefaultSettings()
        {
            if (serverSettings == null)
                serverSettings = new ServerSettings();
            serverSettings.ServerAddress = string.Empty;
            serverSettings.ServerPort = 25;
            serverSettings.User = string.Empty;
            serverSettings.Password = string.Empty;
            serverSettings.From = string.Empty;

            serverSettings.RasEnable = false;
            serverSettings.DialupEntry = string.Empty;
            serverSettings.PhoneNumber = string.Empty;
            serverSettings.RASUser = string.Empty;
            serverSettings.RASPassword = string.Empty;
            serverSettings.RetryTime = 20;
            serverSettings.DisconnectAfter = 20;
            serverSettings.Retries = 3;
            serverSettings.PhonebookPath = string.Empty;

        }
        public void SavePluginSettings(IDataLayer idl)
        {
            using (UnitOfWork ufw = new UnitOfWork(idl))
            {
                var configuration = (from tag in new XPQuery<ADSmtpPluginSettings>(ufw).AsParallel() select tag).ToList();

                if (configuration.Count == 0)
                    configuration.Add(new ADSmtpPluginSettings(ufw));
                SavePluginSettings(configuration[0]);
                ufw.CommitChanges();
            }
        }
        private void SavePluginSettings(ADSmtpPluginSettings settings)
        {
            settings.ServerAddress = serverSettings.ServerAddress;
            //settings.ServerPort = ServerPort;
            //settings.User = User;
            //settings.Password = Password;
            //settings.From = From;

            //settings.RasEnable = RasEnable;
            //settings.DialupEntry = DialupEntry;
            //settings.PhoneNumber = PhoneNumber;
            //settings.RASUser = RASUser;
            //settings.RASPassword = RASPassword;
            //settings.RetryTime = RetryTime;
            //settings.DisconnectAfter = DisconnectAfter;
            //settings.Retries = Retries;
            //settings.PhonebookPath = PhonebookPath;

        }
#endregion
    }
}
