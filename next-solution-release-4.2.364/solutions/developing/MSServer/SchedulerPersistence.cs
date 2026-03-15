using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DevExpress.Xpo;
using DevExpress.Xpo.Metadata;

namespace MSServer
{
    [DeferredDeletion(false)]
    public class SchedulerPersistence : XPObject
    {
         #region Constructors

        public SchedulerPersistence(Session session)
            : base(session)
        {
            
        }

        #endregion

        #region Properties
        private string _NodeId;
        [Size(SizeAttribute.Unlimited)]
        public string NodeId
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
        private DateTime _LastExecutionTime;
        public DateTime LastExecutionTime
        {
            get
            {
                return _LastExecutionTime;
            }
            set
            {
                SetPropertyValue("LastExecutionTime", ref _LastExecutionTime, value);
            }
        }
        private bool _CommandExecuted;
        public bool CommandExecuted
        {
            get
            {
                return _CommandExecuted;
            }
            set
            {
                SetPropertyValue("CommandExecuted", ref _CommandExecuted, value);
            }
        }
        #endregion
    }
}
