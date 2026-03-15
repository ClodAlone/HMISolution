using ADPluginBase;
using Opc.Ua;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ADVoice
{
    public class VoiceMessage
    {
        #region Ctor
        public VoiceMessage()
        {
            Done = false;
        }

        public VoiceMessage(Message m)
        {
            Done = false;
            Add(m);
        }

        public VoiceMessage(List<Message> list)
        {
            Done = false;
            foreach (var m in list)
                Add(m);
        }
        #endregion

        public bool Done { get; set; }

        private NodeId _GroupId;
        public NodeId GroupId
        {
            get { return _GroupId; }
            set { _GroupId = value; }
        }

        private List<NodeId> _NodeIds;
        public List<NodeId> NodeIds
        {
            get
            {
                if (_NodeIds == null)
                    _NodeIds = new List<NodeId>();
                return _NodeIds;
            }
            set { _NodeIds = value; }
        }
        public string Text { get; set; }

        private List<VoiceAddress> _Addresses;
        public List<VoiceAddress> Addresses
        {
            get
            {
                if (_Addresses == null)
                    _Addresses = new List<VoiceAddress>();
                return _Addresses;
            }
            set { _Addresses = value; }
        }
        public int Retry;

        public void Reset()
        {
            Retry = 0;
            Done = false;
            if (NodeIds != null && NodeIds.Count > 0)
                NodeIds.Clear();
            if (Addresses != null && Addresses.Count > 0)
                Addresses.Clear();
            Text = string.Empty;
        }
        public void Add(Message msg)
        {
            GroupId = msg.GroupId;

            NodeIds.Add(msg.NodeId);

            Text = (msg.CustomMessage ? msg.Textmessage :
                        !string.IsNullOrEmpty(msg.Textmessage) ? string.Format("{0}   {1}", msg.Textmessage, msg.Alarmmessage) : msg.Alarmmessage
                );
                

            Addresses.Add(new VoiceAddress() { Address = msg.PhoneNumber });
        }

        public void Copy(VoiceMessage m)
        {
            Reset();
            GroupId = m.GroupId;
            NodeIds.AddRange(m.NodeIds);
            Text = m.Text;
            Addresses.AddRange(m.Addresses);
            Retry = m.Retry;
        }
        public bool IsValid()
        {
            return (Addresses.Count > 0 && NodeIds.Count > 0 && !string.IsNullOrEmpty(Text));
        }

        public bool IsEmpty()
        {
            return (Addresses.Count == 0 && NodeIds.Count == 0 && string.IsNullOrEmpty(Text));
        }

        public String Readeable(NodeId id = null)
        {
            int idx = -1;
            if (id != null)
            {
                idx = NodeIds.FindIndex(x => x.Equals(id));
                
            }

            if (idx >= 0 && idx < NodeIds.Count)
                return string.Format(Properties.Resources.MessageReadeable,
                    (Addresses.Count > 0 ? Addresses[idx].Address : Properties.Resources.MeaasgeNoAddress),
                    (NodeIds.Count > 0 ? NodeIds[idx].ToString() : Properties.Resources.MessageNoNodeId),
                    (string.IsNullOrEmpty(Text) ? Properties.Resources.MessageNoText : Text));

            return string.Format(Properties.Resources.MessageReadeable, 
                (Addresses.Count > 0 ? Addresses[0].Address : Properties.Resources.MeaasgeNoAddress),
                (NodeIds.Count > 0 ? NodeIds[0].ToString() : Properties.Resources.MessageNoNodeId),
                (string.IsNullOrEmpty(Text) ? Properties.Resources.MessageNoText : Text));
        }

        public bool SentToAddress(string address)
        {
            bool bRet = false;
            var list = (from a in Addresses where a.Address == address select a).ToList();
            if (list.Count > 0)
                bRet = (list[0].Call > 0 && list[0].Call > list[0].Error);
            return bRet;
        }
        public bool SentToAll()
        {
            bool bRet = true;
            foreach (var a in Addresses)
                bRet &= (a.Call > 0 && a.Call > a.Error);
            return bRet;
        }

        public void IncrementCall(string address)
        {
            var list = (from a in Addresses where a.Address == address select a).ToList();
            if (list.Count > 0)
                list[0].Call++;
        }
        public int GetLastIndex()
        {
            int i = Addresses.Count-1;
            for (i = Addresses.Count-1; i >= 0; i--)
            {
                if (Addresses[i].Call == Addresses[i].Error)//free to call
                {
                    break;
                }
            }
            return i;
        }
        public int GetNextCurrentAddress(int actual)
        {
            int next = actual+1;
            if (next >= Addresses.Count)
                next = 0;
            bool found = false;
            int i = Addresses.Count;
            for (i = next; i < Addresses.Count; i++)
            {
                if (Addresses[i].Call == Addresses[i].Error)//free to call
                {
                    found = true;
                    break;
                }
            }
            if (found)
                return i;
            else
                return Addresses.Count;
        }
    }
}
