using System;
using System.Collections.Generic;
using System.Linq;
using Opc.Ua;
using DevExpress.Xpo;
using ADModel;

namespace ADPluginBase
{
    [DeferredDeletion(false)]
    public class MessagePersistence : XPObject
    {
        public MessagePersistence(Session session)
            : base(session)
        { }
        private NodeId _NodeId;
        [ValueConverter(typeof(ConvertNodeId))]
        public NodeId NodeId
        {
            get
            {
                return _NodeId;
            }
            set
            {
                SetPropertyValue("NodeId", ref _NodeId, value);
            }
        }
        private string _Recipient;

        public string Recipient
        {
            get
            {
                return _Recipient;
            }
            set
            {
                SetPropertyValue("Recipient", ref _Recipient, value);
            }
        }

        private string _Textmessage;
        public string Textmessage
        {
        	get
        	{
        		return _Textmessage;
        	}
        	set
        	{
        	  SetPropertyValue("Textmessage", ref _Textmessage, value);
        	}
        }

        private string _Alarmmessage;
        public string Alarmmessage
        {
            get
            {
                return _Alarmmessage;
            }
            set
            {
                SetPropertyValue("Alarmmessage", ref _Alarmmessage, value);
            }
        }

        private string _Email;
        public string Email
        {
        	get
        	{
        		return _Email;
        	}
        	set
        	{
        	  SetPropertyValue("Email", ref _Email, value);
        	}
        }
        
        private string _PhoneNumber;
        public string PhoneNumber
        {
        	get
        	{
        		return _PhoneNumber;
        	}
        	set
        	{
        	  SetPropertyValue("PhoneNumber", ref _PhoneNumber, value);
        	}
        }

        private string _MobilePhoneNumber;
        public string MobilePhoneNumber
        {
        	get
        	{
        		return _MobilePhoneNumber;
        	}
        	set
        	{
        	  SetPropertyValue("MobilePhoneNumber", ref _MobilePhoneNumber, value);
        	}
        }

        private DateTime _TimeStamp;
        public DateTime TimeStamp
        {
        	get
        	{
        		return _TimeStamp;
        	}
        	set
        	{
        	  SetPropertyValue("TimeStamp", ref _TimeStamp, value);
        	}
        }

        private NodeId _PluginID;
        public NodeId PluginID
        {
        	get
        	{
        		return _PluginID;
        	}
        	set
        	{
        	  SetPropertyValue("PluginID", ref _PluginID, value);
        	}
        }

        private string _PluginName;
        public string PluginName
        {
            get
            {
                return _PluginName;
            }
            set
            {
                SetPropertyValue("PluginName", ref _PluginName, value);
            }
        }
        private bool _ADGroupMessage;
        public bool ADGroupMessage
        {
            get
            {
                return _ADGroupMessage;
            }
            set
            {
                SetPropertyValue("ADGroupMessage", ref _ADGroupMessage, value);
            }
        }
        private int _ErrorCount;
        public int ErrorCount 
        {
            get
            {
                return _ErrorCount;
            }
            set
            {
                SetPropertyValue("ErrorCount", ref _ErrorCount, value);
            }
        }
        private string _TagName;
        public string TagName
        {
            get
            {
                return _TagName;
            }
            set
            {
                SetPropertyValue("TagName", ref _TagName, value);
            }
        }
        private string _ChatID;
        public string ChatID
        {
            get
            {
                return _ChatID;
            }
            set
            {
                SetPropertyValue("ChatID", ref _ChatID, value);
            }
        }
        private string _FCMTokenPath;
        public string FCMTokenPath
        {
            get
            {
                return _FCMTokenPath;
            }
            set
            {
                SetPropertyValue("FCMTokenPath", ref _FCMTokenPath, value);
            }
        }
        private string _Reason;
        public string Reason
        {
            get { return _Reason; }
            set { SetPropertyValue("Reason", ref _Reason, value); }
        }
        private string _Name;
        public string Name
        {
            get { return _Name; }
            set { SetPropertyValue("Name", ref _Name, value); }
        }
        private bool _CustomMessage;
        public bool CustomMessage
        {
            get { return _CustomMessage; }
            set { SetPropertyValue("CustomMessage", ref _CustomMessage, value); }
        }
        private string _Attachments;
        public string Attachments
        {
            get { return _Attachments; }
            set { SetPropertyValue("Attachments", ref _Attachments, value); }
        }
        private string _GroupId;
        public string GroupId
        {
            get { return _GroupId; }
            set { SetPropertyValue("GroupId", ref _GroupId, value); }
        }
        private string _ServerAlarmStringId;
        public string ServerAlarmStringId
        {
            get { return _ServerAlarmStringId; }
            set { SetPropertyValue("ServerAlarmStringId", ref _ServerAlarmStringId, value); }
        }

        private string _CultureName;
        public string CultureName
        {
            get { return _CultureName; }
            set { SetPropertyValue("CultureName", ref _CultureName, value); }
        }
    }
}
