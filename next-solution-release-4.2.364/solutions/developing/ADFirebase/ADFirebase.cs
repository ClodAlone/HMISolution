using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using ADPluginBase;
using DevExpress.Xpo;
using Google.Apis.Auth.OAuth2.Responses;
using Opc.Ua;
using Utilities.Logger;

namespace ADFirebase
{
    #region Data
    public enum ADFirebaseError : int
    {
        ADFirebaseErrorNoError = 0,
        ADFirebaseConnectionError = 1,
        ADFirebaseErrorNoMessage = 2,
        ADFirebaseErrorNoDBURL = 3,
        ADFirebaseErrorSAKFile = 4,
        ADFirebaseErrorNoValidToken = 5,
        ADFirebaseErrorMessagingException = 6,
        ADFirebaseErrorRelativePathOnDbProject = 7,
    }
    #endregion


    public class ADFirebaseNotificationSender : PluginBase, IDisposable
    {
        internal HttpClient httpClient;
        internal TokenResponse TokenResponse;
        internal string alarmDispatcherFolder;
        private bool isDbProject;
        internal string serviceAccountKeyFilePath;
        internal bool InitOk;
        internal bool RTDBUrlOk;
        internal bool ServiceAccountKeyFileOk;
        private bool relativeSAKpathOnDBProject;
        internal IReadOnlyDictionary<string, string> credentialParameters;
        internal string ProjectID;
        internal string conn;
        string jsonFileOnDB = string.Empty;

        #region Ctor
        public ADFirebaseNotificationSender()
        { }
        #endregion

        #region Methods Override
        public override bool Init(string strSettingPath)
        {
            PluginInfo.Initialize(this);
            httpClient = new HttpClient();
            isDbProject = XpoHelpers.XpoHelper.IsSQlDataProvider(strSettingPath);
            if (isDbProject)
                conn = strSettingPath;
            else
                alarmDispatcherFolder = Path.GetDirectoryName(GetFileBase(strSettingPath, GetPluginName()));

            var baseInit = base.Init(strSettingPath);

            return baseInit;
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
                    bRet = LoadDriverSettings(configuration);
                }
                catch (InvalidOperationException ex)
                {
                    OnSystemEvent(null, String.Format(Properties.Resources.ErrorLoadingPluginSettings, ex.Message), EventSeverity.Min,
                        null, null, null, (int)System.Diagnostics.EventLogEntryType.Error, (int)LoggerDestination.AlarmDispatcher);
                    LoadDefaultSettings();
                    bRet = true;
                }
            }

            return bRet;
        }

        internal int sendResult;
        private async void SendTextMessage(FCMClient client, List<string> fcmTokens, string yourMessage, Dictionary<string, string> data)
        {
            try
            {
                if (fcmTokens.Count == 1)
                    await client.SendTextMessageAsync(fcmTokens.Single(), yourMessage, data);
                else
                    await client.SendTextMulticastMessageAsync(fcmTokens, yourMessage, data);
                sendResult = (int)ADFirebaseError.ADFirebaseErrorNoError;
            }
            catch (TokenException ex)
            {
                sendResult = (int)ADFirebaseError.ADFirebaseErrorNoValidToken;
                LastError = string.Format(Properties.Resources.DetailsFormat, "NoValidToken", ex.Message);
            }
            catch (FirebaseAdmin.Messaging.FirebaseMessagingException ex)
            {
                sendResult = (int)ADFirebaseError.ADFirebaseErrorMessagingException;
                LastError = string.Format(Properties.Resources.ADFirebaseErrorMessagingException, ex.Message, Enum.GetName(typeof(FirebaseAdmin.Messaging.MessagingErrorCode), ex.MessagingErrorCode));
            }
            catch (Exception ex)
            {
                sendResult = (int)ADFirebaseError.ADFirebaseConnectionError;
                LastError = ex.Message;
            }
        }

        public override bool SendMultiple() { return true; }
        public override int SendMessage(object msg)
        {
            int ReturnValue = (int)ADFirebaseError.ADFirebaseErrorNoError;
            var list = msg as List<Message>;
            var messaggio = msg as Message;

            if (!InitOk)
            {
                if (relativeSAKpathOnDBProject)
                {
                    ReturnValue = (int)ADFirebaseError.ADFirebaseErrorRelativePathOnDbProject;
                    LastError = Properties.Resources.ADFirebaseErrorRelativePathOnDbProject;
                }
                else if (!ServiceAccountKeyFileOk)
                {
                    ReturnValue = (int)ADFirebaseError.ADFirebaseErrorSAKFile;
                    LastError = Properties.Resources.ADFirebaseErrorSAKFile;
                }
                else if (!RTDBUrlOk)
                {
                    ReturnValue = (int)ADFirebaseError.ADFirebaseErrorNoDBURL;
                    LastError = Properties.Resources.ADFirebaseErrorNoDBURL;
                }
                LogSendResult(ReturnValue, null);
                return ReturnValue;
            }

            if (messaggio != null)
            {
                //send single
                ReturnValue = SendFCM(getMessageToSend(messaggio), new List<string>() { messaggio.FCMTokenPath }, GetDataFromMessage(messaggio));
                LogSendResult(ReturnValue, messaggio);
            }
            else if (list != null && list.Count > 0)
            {
                foreach (var groupByGroupId in list.GroupBy(m => m.GroupId))
                {
                    foreach (var groupByText in groupByGroupId.GroupBy(m => getMessageToSend(m)))
                    {
                        string myMsg = groupByText.Key;
                        int nResult = SendFCM(myMsg, groupByText.Select(x => x.FCMTokenPath).ToList(), GetDataFromMessage(groupByText.First()));
                        LogSendResult(nResult, groupByText.First());
                        if (nResult != (int)ADFirebaseError.ADFirebaseErrorNoError)
                            ReturnValue = nResult;
                    }
                }
            }
            else
            {
                //no messages, error
                OnSystemEvent(null, Properties.Resources.NoMessage, EventSeverity.High, null, null, null,
                        (int)System.Diagnostics.EventLogEntryType.Error, (int)LoggerDestination.AlarmDispatcher);
                LastError = Properties.Resources.NoMessage;
                ReturnValue = (int)ADFirebaseError.ADFirebaseErrorNoMessage;
            }

            return ReturnValue;
        }

        string getMessageToSend(ADPluginBase.Message messaggio)
        {
            return messaggio.CustomMessage ? messaggio.Textmessage : $"{messaggio.Name} {messaggio.Reason} - {messaggio.Textmessage} - {messaggio.Alarmmessage}";
        }

        void LogSendResult(int error, Message messaggio)
        {
            void LogError(string text)
            {
                OnSystemEvent(null, text, EventSeverity.High, null, null, null, (int)System.Diagnostics.EventLogEntryType.Error, (int)LoggerDestination.AlarmDispatcher);
            }

            switch ((ADFirebaseError)error)
            {
                case ADFirebaseError.ADFirebaseErrorNoError:
                    OnSystemEvent(null, string.Format(Properties.Resources.ADFirebaseErrorNoError, messaggio.Textmessage),
                    EventSeverity.Min, null, string.Format(Properties.Resources.DetailsFormat, messaggio.Name, messaggio.TagName), messaggio.Reason,
                        (int)System.Diagnostics.EventLogEntryType.Information, (int)LoggerDestination.AlarmDispatcher);
                    break;

                case ADFirebaseError.ADFirebaseConnectionError:
                    LogError(string.Format(Properties.Resources.ADFirebaseConnectionError, GetPluginName(), LastError));
                    break;

                case ADFirebaseError.ADFirebaseErrorSAKFile: 
                case ADFirebaseError.ADFirebaseErrorNoDBURL:
                case ADFirebaseError.ADFirebaseErrorRelativePathOnDbProject:
                    LogError(LastError);
                    break;

                case ADFirebaseError.ADFirebaseErrorNoValidToken:
                    LogError(string.Format(Properties.Resources.ADFirebaseErrorNoValidToken, messaggio.Recipient, messaggio.FCMTokenPath));
                    break;
            }
        }
        

        int SendFCM(string text, List<string> tokens, Dictionary<string, string> data)
        {
            LastError = string.Empty;
            try
            {
                FCMClient client = new FCMClient(this,jsonFileOnDB);
                SendTextMessage(client, tokens, text, data);
                return sendResult;
            }
            catch (Exception ex)
            {
                OnSystemEvent(null, string.Format(Properties.Resources.ADFirebaseConnectionError, GetPluginName(), ex.Message), EventSeverity.High, null,
                        null, null, (int)System.Diagnostics.EventLogEntryType.Error, (int)LoggerDestination.AlarmDispatcher);
                LastError = ex.Message;
                return (int)ADFirebaseError.ADFirebaseConnectionError;
            }
        }

        private Dictionary<string, string> GetDataFromMessage(Message message)
        {
            var tmp =  new Dictionary<string, string>()
            {
                { "Name", message.Name },
                { "Reason", message.Reason },
            };

            if (!string.IsNullOrEmpty(message.Textmessage)) { tmp.Add("Textmessage", message.Textmessage); }
            if (!string.IsNullOrEmpty(message.Alarmmessage)) { tmp.Add("Alarmmessage", message.Alarmmessage); }
            return tmp;
        }

        protected override void Dispose(bool disposing)
        {
            httpClient.Dispose();
            base.Dispose(disposing);
        }

        #endregion

        #region Methods
        private bool LoadDriverSettings(PluginSettings configuration)
        {
            if (configuration == null)
                return false;

            _serveiceAccountKeyFile = configuration.ServeiceAccountKeyFile;
            _realtimeDatabaseUrl = configuration.RealtimeDatabaseUrl;
            _useAccessToken = configuration.UseAccessToken;
            _notificationsTitle = configuration.NotificationsTitle;

            return InitPluginStates();
        }

        bool InitPluginStates()
        {
            RTDBUrlOk = !string.IsNullOrEmpty(_realtimeDatabaseUrl);
            ServiceAccountKeyFileOk = !string.IsNullOrEmpty(_serveiceAccountKeyFile);
            InitOk = RTDBUrlOk && ServiceAccountKeyFileOk;
            if (!InitOk) return false;

            string credentialString = string.Empty;
            if (isDbProject)
            {
                ServiceAccountKeyFileOk = TryToGetJsonFileFromDatabase(out jsonFileOnDB);
                if (ServiceAccountKeyFileOk)
                    credentialString = jsonFileOnDB;
            }
            else
            {
                serviceAccountKeyFilePath = Path.IsPathRooted(_serveiceAccountKeyFile)
                    ? _serveiceAccountKeyFile
                    : Path.Combine(alarmDispatcherFolder, _serveiceAccountKeyFile);
                ServiceAccountKeyFileOk = File.Exists(serviceAccountKeyFilePath);
                if (ServiceAccountKeyFileOk)
                    credentialString = File.ReadAllText(serviceAccountKeyFilePath);
            }
            if(!string.IsNullOrEmpty(credentialString))
            {
                credentialParameters = Newtonsoft.Json.JsonConvert.DeserializeObject<Dictionary<string, string>>(credentialString);
                credentialParameters.TryGetValue("project_id", out ProjectID);
            }
            

            InitOk = InitOk && ServiceAccountKeyFileOk;
            return InitOk;
        }
        private bool TryToGetJsonFileFromDatabase(out string jsonString)
        {
            string sJson = string.Empty;
            bool result = false;
            JSONFile file = null;
            using (var idl = XpoDefault.GetDataLayer(conn, DevExpress.Xpo.DB.AutoCreateOption.DatabaseAndSchema))
            {
                using (UnitOfWork ufw = new UnitOfWork(idl))
                {
                    file = (from S in new XPQuery<JSONFile>(ufw).AsParallel() select S).ToList().FirstOrDefault();
                    if (file != null)
                    {
                        try
                        {
                            using (MemoryStream ms = new MemoryStream(file.FileBody))
                            using (StreamReader sr = new StreamReader(ms))
                                sJson = sr.ReadToEnd();
                            result = true;
                        }
                        catch
                        {
                            result = false;
                            sJson = String.Empty;
                        }
                    }
                }
                jsonString = sJson;
            }
            return result;
        }

            public void LoadDefaultSettings()
        {
            _serveiceAccountKeyFile = string.Empty;
            _realtimeDatabaseUrl = string.Empty;
            _useAccessToken = true;
            _notificationsTitle = string.Empty;
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
            settings.ServeiceAccountKeyFile = ServeiceAccountKeyFile;
            settings.RealtimeDatabaseUrl = RealtimeDatabaseUrl;
            settings.UseAccessToken = UseAccessToken;
            settings.NotificationsTitle = NotificationsTitle;
        }
        #endregion

        #region Properties 
        private string _serveiceAccountKeyFile;
        public string ServeiceAccountKeyFile
        {
            get => _serveiceAccountKeyFile;
            set => _serveiceAccountKeyFile = value;
        }

        private string _realtimeDatabaseUrl;
        public string RealtimeDatabaseUrl
        {
            get => _realtimeDatabaseUrl;
            set => _realtimeDatabaseUrl = value;
        }

        private bool _useAccessToken;
        public bool UseAccessToken 
        {
            get => _useAccessToken;
            set => _useAccessToken = value;
        }

        private string _notificationsTitle;
        public string NotificationsTitle
        {
            get => _notificationsTitle;
            set => _notificationsTitle = value;
        }
        #endregion
    }
}
