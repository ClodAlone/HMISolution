using System;
using System.Collections.Generic;
using System.Linq;
using DriverCodeBase;
using DevExpress.Xpo;
using System.ComponentModel;
using Opc.Ua;
namespace OpcClientDriver
{
    [MapInheritance(MapInheritanceType.ParentTable)]
    public class OpcClientDriverCommJobSettings : CommJobSettings
    {
        #region Constructors

        public OpcClientDriverCommJobSettings(Session session, OpcClientDriverCommJob job)
            : base(session, job)
        {
            if (job.OPCItem.HostName != null)
                _HostName = job.OPCItem.HostName;
            if (job.OPCItem.AppName != null)
                _AppName = job.OPCItem.AppName;
            if (job.OPCItem.EndpointUrl != null)
                _EndpointUrl = job.OPCItem.EndpointUrl;
            if (job.OPCItem.RelativePath != null)
                _RelativePath = job.OPCItem.RelativePath;
            if (job.OPCItem.ResolvedNodeId != null)
                _ResolvedNodeId = job.OPCItem.ResolvedNodeId;
            if (job.OPCItem.HumanReadable != null)
                _ItemName = job.OPCItem.HumanReadable;
            if (job.OPCItem.TypeDefinitionNodeId != null)
                _TypeDefinitionNodeId = job.OPCItem.TypeDefinitionNodeId;
        }

        public OpcClientDriverCommJobSettings(Session session)
            : base(session)
        {
            // This constructor is used when an object is loaded from a persistent storage.
            // Do not place any code here.
        }

        protected OpcClientDriverCommJobSettings()
        {
            // This constructor is used when an object is loaded from a persistent storage.
            // Do not place any code here.
        }

        #endregion

        public void DefaultSettings()
        {
            base.DefaultSettings();
            
        }

        #region Properties

        #region OpcUaEntityReference
        private string _ItemName;
        [Size(SizeAttribute.Unlimited)]
        public string ItemName
        {
            get
            {
                return _ItemName;
            }
            set
            {
                SetPropertyValue("ItemName", ref _ItemName, value);
            }
        }

        private string _HostName;
        public string HostName
        {
            get
            {
                return _HostName;
            }
            set
            {
                SetPropertyValue("HostName", ref _HostName, value);
            }
        }

        private string _AppName;
        [Size(SizeAttribute.Unlimited)]
        public string AppName
        {
            get
            {
                return _AppName;
            }
            set
            {
                SetPropertyValue("AppName", ref _AppName, value);
            }
        }
        private string _RelativePath;
        [Size(SizeAttribute.Unlimited)]
        public string RelativePath
        {
            get
            {
                return _RelativePath;
            }
            set
            {
                SetPropertyValue("RelativePath", ref _RelativePath, value);
            }
        }
        private string _EndpointUrl;
        [Size(SizeAttribute.Unlimited)]
        public string EndpointUrl
        {
            get
            {
                return _EndpointUrl;
            }
            set
            {
                SetPropertyValue("EndpointUrl", ref _EndpointUrl, value);
            }
        }
        private string _StartingAddress;
        [Size(SizeAttribute.Unlimited)]
        public string StartingAddress
        {
            get
            {
                return _StartingAddress;
            }
            set
            {
                SetPropertyValue("StartingAddress", ref _StartingAddress, value);
            }
        }

        private string _TypeDefinitionName;
        [Size(SizeAttribute.Unlimited)]
        public string TypeDefinitionName
        {
            get
            {
                return _TypeDefinitionName;
            }
            set
            {
                SetPropertyValue("TypeDefinitionName", ref _TypeDefinitionName, value);
            }
        }
        private NodeId _ResolvedNodeId;
        public NodeId ResolvedNodeId
        {
            get
            {
                return _ResolvedNodeId;
            }
            set
            {
                SetPropertyValue("ResolvedNodeId", ref _ResolvedNodeId, value);
            }
        }
        private NodeId _ResolvedStartingNodeId;
        public NodeId ResolvedStartingNodeId
        {
            get
            {
                return _ResolvedStartingNodeId;
            }
            set
            {
                SetPropertyValue("ResolvedStartingNodeId", ref _ResolvedStartingNodeId, value);
            }
        }
        private ExpandedNodeId _TypeDefinitionNodeId;
        public ExpandedNodeId TypeDefinitionNodeId
        {
            get
            {
                return _TypeDefinitionNodeId;
            }
            set
            {
                SetPropertyValue("TypeDefinitionNodeId", ref _TypeDefinitionNodeId, value);
            }
        }

        #endregion
        #endregion


        #region IDataErrorInfo Members
       
        #endregion

    }
}
