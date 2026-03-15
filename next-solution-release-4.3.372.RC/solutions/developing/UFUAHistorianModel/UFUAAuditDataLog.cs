using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DevExpress.Xpo;
using Opc.Ua;

namespace UFUAHistorianModel
{

    [DeferredDeletion(false)]
    public class UFUAAuditDataLog : XPObject
    {
        #region Constructors
        public UFUAAuditDataLog(Session session)
            : base(session)
        { }
        #endregion

        #region Properties
        private string _Name;
        [Size(2048)]
        public string Name
        {
            get
            {
                return _Name;
            }
            set
            {
                if (value != null && value.Length > 2048)
                    value = value.Substring(0, 2048);

                SetPropertyValue("Name", ref _Name, value);
            }
        }

        private String _NodeId;
        [Size(2048)]
        public String NodeId
        {
            get
            {
                return _NodeId;
            }
            set
            {
                if (value != null && value.Length > 2048)
                    value = value.Substring(0, 2048);

                SetPropertyValue("NodeId", ref _NodeId, value);
            }
        }

        private string _HistoricalName;
        public string HistoricalName
        {
            get
            {
                return _HistoricalName;
            }
            set
            {
                if (value != null && value.Length > SizeAttribute.DefaultStringMappingFieldSize)
                    value = value.Substring(0, SizeAttribute.DefaultStringMappingFieldSize);

                SetPropertyValue("HistoricalName", ref _HistoricalName, value);
            }
        }

        private string _Description;
        [Size(SizeAttribute.Unlimited)]
        public string Description
        {
            get
            {
                return _Description;
            }
            set
            {
                SetPropertyValue("Description", ref _Description, value);
            }
        }
        #endregion
    }
}
