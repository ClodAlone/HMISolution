using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Opc.Ua;
using DevExpress.Xpo;

namespace ADPluginBase
{
    public class Message
    {
        public NodeId NodeId { get; set; }
        public string Recipient { get; set; }
        public string Name { get; set; }
        public string Textmessage { get; set; }
        public string Alarmmessage { get; set; }
        public string Reason { get; set; }
        public string Email { get; set; }
        public string PhoneNumber { get; set; }
        public string MobilePhoneNumber { get; set; }
        public string ChatID { get; set; }
        public string FCMTokenPath { get; set; }
        public DateTime TimeStamp { get; set; }
        public NodeId PluginID { get; set; }
        public string PluginName { get; set; }
        public bool ADGroupMessage { get; set; }
        public int ErrorCount { get; set; }
        public bool CustomMessage { get; set; }
        public string TagName { get; set; }
        public string Attachments { get; set; }

        public string GroupId { get; set; }
        public string ServerAlarmStringId { get; set; }//in order to ACK the server alarm to the server...

        public string CultureName { get; set; }
        public Guid UserId { get; set; }
    }
}
