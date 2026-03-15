using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SmtpSender
{
    public class ServerSettings
    {

        #region Ctor
        public ServerSettings()
        {
            DefaultSettings();
        }
        #endregion Ctor

        #region Methods
        public void DefaultSettings()
        {
            ServerAddress = "";
            ServerPort = 25;
            User = "";
            Password = "";
            RasEnable = false;
            DialupEntry = string.Empty;
            PhoneNumber = string.Empty;
            RASUser = string.Empty;
            RASPassword = string.Empty;
            RetryTime = 20;
            DisconnectAfter = 20;
            Retries = 3;
            PhonebookPath = string.Empty;
            From = string.Empty;
        }
        #endregion

        #region Properties
        public string ServerAddress { get; set;}
        public int ServerPort{ get; set;}
        public string User{get;set;}
        public string Password{get;set;}
        public bool RasEnable{get;set;}
        public string DialupEntry{get;set;}
        public string PhoneNumber{get;set;}
        public string RASUser{get;set;}
        public string RASPassword{get;set;}
        public int RetryTime{get;set;}
        public int DisconnectAfter{get;set;}
        public int Retries{get;set;}
        public string PhonebookPath{get;set;}
        public string From{get;set;}
        #endregion
    }
}
