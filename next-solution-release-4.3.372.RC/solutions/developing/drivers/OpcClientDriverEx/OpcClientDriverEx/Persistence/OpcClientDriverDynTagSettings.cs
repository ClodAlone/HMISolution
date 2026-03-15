using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DriverCodeBaseEx;
using DriverCodeBaseEx.Helpers;
using System.ComponentModel;
using Opc.Ua;
using DriverBaseInterfaces;

namespace OpcClientDriver
{
    public sealed class OpcClientDriverDynTagSettings : DynTagSettings
    {
        #region Constructors
        protected readonly List<string> validParameterNames;
        public OpcClientDriverDynTagSettings()
            : base()
        {
            validParameterNames = GetParamNames();
        }
        bool Valid = false;
        #endregion

        #region Static Members

        private static readonly String ItemNameParameter = "IP";
        private static readonly String HostNameParameter = "HN";
        private static readonly String AppNameParameter = "AN";
        private static readonly String RelativePathParameter = "RP";
        private static readonly String EndpointUrlParameter = "EU";
        private static readonly String StartingAddressParameter = "SA";
        private static readonly String AliasParameter = "AP";
        private static readonly String TypeDefinitionNameParameter = "TD";
        private static readonly String ResolvedNodeIdParameter = "RN";
        private static readonly String ResolvedStartingNodeIdParameter = "RS";
        private static readonly String TypeDefinitionNodeIdParameter = "TN";
        private static readonly String Struct_IEC61131_3Parameter = "IE";

        public override List<string> GetParamNames()
        {
            var l = base.GetParamNames();
            l.Add(ItemNameParameter);
            l.Add(HostNameParameter);
            l.Add(AppNameParameter);
            l.Add(RelativePathParameter);
            l.Add(EndpointUrlParameter);
            l.Add(StartingAddressParameter);
            l.Add(AliasParameter);
            l.Add(TypeDefinitionNameParameter);
            l.Add(ResolvedNodeIdParameter);
            l.Add(ResolvedStartingNodeIdParameter);
            l.Add(TypeDefinitionNodeIdParameter);
            l.Add(Struct_IEC61131_3Parameter);
            return l;
        }
        #endregion

        #region Override Functions

        public override void Parse(String dynamicSettings)
        {
            base.Parse(dynamicSettings);

            DynamicStringParser helper = new DynamicStringParser(dynamicSettings, validParameterNames);

            if (String.IsNullOrEmpty(helper.GetPartByName(AppNameParameter))
                || String.IsNullOrEmpty(helper.GetPartByName(ItemNameParameter))
                || String.IsNullOrEmpty(helper.GetPartByName(EndpointUrlParameter))
                || String.IsNullOrEmpty(helper.GetPartByName(RelativePathParameter)))
                Valid = false;

            ItemName = helper.GetPartByName(ItemNameParameter);
            AppName = helper.GetPartByName(AppNameParameter);
            RelativePath = helper.GetPartByName(RelativePathParameter);
            EndpointUrl = helper.GetPartByName(EndpointUrlParameter);
            if (helper.GetPartByName(StartingAddressParameter).Length > 0)
                StartingAddress = helper.GetPartByName(StartingAddressParameter);
            if (helper.GetPartByName(TypeDefinitionNameParameter).Length > 0)
                TypeDefinitionName = helper.GetPartByName(TypeDefinitionNameParameter);
            if (helper.GetPartByName(HostNameParameter).Length > 0)
                HostName = helper.GetPartByName(HostNameParameter);
            if (helper.GetPartByName(ResolvedNodeIdParameter).Length > 0)
            {
                try
                {
                    ResolvedNodeId = new NodeId(helper.GetPartByName(ResolvedNodeIdParameter));
                }
                catch
                {
                    // don't set any value -> the driver will request it at RunTime
                }
            }
            if (helper.GetPartByName(ResolvedStartingNodeIdParameter).Length > 0)
            {
                try
                {             
                    ResolvedStartingNodeId = new NodeId(helper.GetPartByName(ResolvedStartingNodeIdParameter));
                }
                catch
                {
                    // don't set any value -> the driver will request it at RunTime
                }
            }
            if (helper.GetPartByName(TypeDefinitionNodeIdParameter).Length > 0)
                TypeDefinitionNodeId = new ExpandedNodeId(helper.GetPartByName(TypeDefinitionNodeIdParameter));
            Struct_IEC61131_3 = helper.GetPartByName(Struct_IEC61131_3Parameter, false);

            Valid = true;
        }

        public override bool TryParse(String dynamicSettings)
        {
            Valid = false;
            if (!base.TryParse(dynamicSettings))
                return false;

            DynamicStringParser helper = new DynamicStringParser(dynamicSettings, validParameterNames);

            // required parameter
            if (String.IsNullOrEmpty(helper.GetPartByName(ItemNameParameter))
                || String.IsNullOrEmpty(helper.GetPartByName(AppNameParameter))
                || (String.IsNullOrEmpty(helper.GetPartByName(RelativePathParameter)) && String.IsNullOrEmpty(helper.GetPartByName(ResolvedNodeIdParameter)))
                || String.IsNullOrEmpty(helper.GetPartByName(EndpointUrlParameter)))
                return false;

            ItemName = helper.GetPartByName(ItemNameParameter);
            AppName = helper.GetPartByName(AppNameParameter);
            EndpointUrl = helper.GetPartByName(EndpointUrlParameter);
            if (helper.GetPartByName(RelativePathParameter).Length > 0)
                RelativePath = helper.GetPartByName(RelativePathParameter);
            if (helper.GetPartByName(StartingAddressParameter).Length > 0)
                StartingAddress = helper.GetPartByName(StartingAddressParameter);
            if (helper.GetPartByName(TypeDefinitionNameParameter).Length > 0)
                TypeDefinitionName = helper.GetPartByName(TypeDefinitionNameParameter);
            if (helper.GetPartByName(HostNameParameter).Length > 0)
                HostName = helper.GetPartByName(HostNameParameter);
            if (helper.GetPartByName(ResolvedNodeIdParameter).Length > 0)
            {
                try
                {
                    ResolvedNodeId = new NodeId(helper.GetPartByName(ResolvedNodeIdParameter));
                }
                catch
                {
                    // don't set any value -> the driver will request it at RunTime
                }
            }
            if (helper.GetPartByName(ResolvedStartingNodeIdParameter).Length > 0)
            {
                try
                {
                    ResolvedStartingNodeId = new NodeId(helper.GetPartByName(ResolvedStartingNodeIdParameter));
                }
                catch
                {
                    // don't set any value -> the driver will request it at RunTime
                }
            }
            if (helper.GetPartByName(TypeDefinitionNodeIdParameter).Length > 0)
                TypeDefinitionNodeId = new ExpandedNodeId(helper.GetPartByName(TypeDefinitionNodeIdParameter));

            Struct_IEC61131_3 = helper.GetPartByName(Struct_IEC61131_3Parameter, false);

            Valid = true;
            return true;
        }

        public override string ToString()
        {
            
            var dynamicstring = new StringBuilder(base.ToString());
            dynamicstring.Append(DynamicStringParser.CharSep);
            dynamicstring.AppendFormat("{0}{1}{2}", AppNameParameter, DynamicStringParser.CharAssign, AppName);
            dynamicstring.Append(DynamicStringParser.CharSep);
            dynamicstring.AppendFormat("{0}{1}{2}", ItemNameParameter, DynamicStringParser.CharAssign, ItemName);
            dynamicstring.Append(DynamicStringParser.CharSep);
            dynamicstring.AppendFormat("{0}{1}{2}", EndpointUrlParameter, DynamicStringParser.CharAssign, EndpointUrl);
            dynamicstring.Append(DynamicStringParser.CharSep);
            dynamicstring.AppendFormat("{0}{1}{2}", RelativePathParameter, DynamicStringParser.CharAssign, RelativePath);
            if (ResolvedNodeId != null && ResolvedNodeId.ToString().Length > 0)
            {
                dynamicstring.Append(DynamicStringParser.CharSep);
                dynamicstring.AppendFormat("{0}{1}{2}", ResolvedNodeIdParameter, DynamicStringParser.CharAssign, ResolvedNodeId.ToString());
            }
            if (HostName != null && HostName.Length > 0)
            {
                dynamicstring.Append(DynamicStringParser.CharSep);
                dynamicstring.AppendFormat("{0}{1}{2}", HostNameParameter, DynamicStringParser.CharAssign, HostName);
            }
            if (StartingAddress != null && StartingAddress.Length > 0)
            {
                dynamicstring.Append(DynamicStringParser.CharSep);
                dynamicstring.AppendFormat("{0}{1}{2}", StartingAddressParameter, DynamicStringParser.CharAssign, StartingAddress);
            }
            if (TypeDefinitionName != null && TypeDefinitionName.Length > 0)
            {
                dynamicstring.Append(DynamicStringParser.CharSep);
                dynamicstring.AppendFormat("{0}{1}{2}", TypeDefinitionNameParameter, DynamicStringParser.CharAssign, TypeDefinitionName);
            }

            if (ResolvedStartingNodeId != null && ResolvedStartingNodeId.ToString().Length > 0)
            {
                dynamicstring.Append(DynamicStringParser.CharSep);
                dynamicstring.AppendFormat("{0}{1}{2}", ResolvedStartingNodeIdParameter, DynamicStringParser.CharAssign, ResolvedStartingNodeId.ToString());
            }

            if (TypeDefinitionNodeId != null && TypeDefinitionNodeId.ToString().Length > 0)
            {
                dynamicstring.Append(DynamicStringParser.CharSep);
                dynamicstring.AppendFormat("{0}{1}{2}", TypeDefinitionNodeIdParameter, DynamicStringParser.CharAssign, TypeDefinitionNodeId.ToString());
            }

            dynamicstring.Append(DynamicStringParser.CharSep);
            dynamicstring.AppendFormat("{0}{1}{2}", Struct_IEC61131_3Parameter, DynamicStringParser.CharAssign, Struct_IEC61131_3);

            return dynamicstring.ToString();
        }

        public override string GetFirstDynSetting(Tag tag, TagDefinition thistagdefinition)
        {
            TryParse(tag.TagNode.DynamicSettings);
            var relativeName = thistagdefinition.Name;
            var localNamespaceIndex = FindNamespaceIndexFromRelativeName(relativeName);
            var serverNamespaceIndex = FindNamespaceIndexFromRelativeName(RelativePath);
            if (localNamespaceIndex != -1 && serverNamespaceIndex != -1 && localNamespaceIndex != serverNamespaceIndex)
            {
                relativeName = relativeName.Replace(String.Format("{0}:", localNamespaceIndex), String.Format("{0}:", serverNamespaceIndex));
            }
            DynamicStringParser helper = new DynamicStringParser(tag.TagNode.DynamicSettings, validParameterNames);
            var h = helper.GetPartByName(ResolvedNodeIdParameter);
            var lastIndex = h.LastIndexOf("=");
            var name = h.Substring(lastIndex + 1);
            var index = RelativePath.LastIndexOf(":");
            var cfrName = RelativePath.Substring(index + 1);
            string previousRelativePath = RelativePath;
            if (name != cfrName)
                RelativePath = string.Format("{0}/{1}", RelativePath, relativeName);
            ResolvedNodeId = null;
            string newDynSettings = ToString();
            RelativePath = previousRelativePath;
            return (newDynSettings);
        }
        #endregion

        #region Methods
        int FindNamespaceIndexFromRelativeName(string relativeName)
        {
            int namepaceIndex = -1;
            if (!String.IsNullOrEmpty(relativeName))
            {
                var index = relativeName.LastIndexOf(':');
                if (index > 0)
                {
                    int namespaceIndexFound;
                    int counter = 0;
                    while (--index >= 0)
                    {
                        var namespaceIndexText = relativeName.Substring(index, ++counter);
                        if (!int.TryParse(namespaceIndexText, out namespaceIndexFound))
                            break;

                        namepaceIndex = namespaceIndexFound;
                    }
                }
            }

            return namepaceIndex;
        }
        #endregion

        #region Properties
        private string _Item;
        public string ItemString
        {
            get
            {
                return _Item;
            }
            set
            {
                _Item = value;
            }
        }
        private string _ItemName;
        public string ItemName
        {
            get
            {
                return _ItemName;
            }
            set
            {
                _ItemName = value;
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
                _HostName = value;
            }
        }

        private string _AppName;
        public string AppName
        {
            get
            {
                return _AppName;
            }
            set
            {
                _AppName = value;
            }
        }
        private string _RelativePath;
        public string RelativePath
        {
            get
            {
                return _RelativePath;
            }
            set
            {
                _RelativePath = value;
            }
        }
        private string _EndpointUrl;
        public string EndpointUrl
        {
            get
            {
                return _EndpointUrl;
            }
            set
            {
                _EndpointUrl = value;
            }
        }
        private string _StartingAddress;
        public string StartingAddress
        {
            get
            {
                return _StartingAddress;
            }
            set
            {
                _StartingAddress = value;
            }
        }
        private string _TypeDefinitionName;
        public string TypeDefinitionName
        {
            get
            {
                return _TypeDefinitionName;
            }
            set
            {
                _TypeDefinitionName = value;
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
                _ResolvedNodeId = value;
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
                _ResolvedStartingNodeId = value;
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
                _TypeDefinitionNodeId = value;
            }
        }

        private bool _Struct_IEC61131_3;
        public bool Struct_IEC61131_3
        {
            get
            {
                return _Struct_IEC61131_3;
            }
            set
            {
                _Struct_IEC61131_3 = value;
            }
        }
        #endregion

        #region IDataErrorInfo Members

        protected override String PerformValidation(String propertyName)
        {
            string sBase = base.PerformValidation(propertyName);

            if (sBase != null)
                return sBase;
            if (propertyName == "AppName")
                if(String.IsNullOrEmpty(AppName) && (String.IsNullOrEmpty(EndpointUrl) || String.IsNullOrEmpty(RelativePath) || NodeId.IsNull(ResolvedNodeId) || String.IsNullOrEmpty(ItemName)))
                    return Properties.Resources.ErrorAppName;
            if (propertyName == "EndpointUrl")
            {
                if (String.IsNullOrEmpty(EndpointUrl) && (String.IsNullOrEmpty(AppName) || String.IsNullOrEmpty(RelativePath) || NodeId.IsNull(ResolvedNodeId) || String.IsNullOrEmpty(ItemName)))
                    return Properties.Resources.ErrorEndpointUrl;
                if (!Uri.IsWellFormedUriString(EndpointUrl, UriKind.Absolute))
                    return Properties.Resources.EndpointDescription_InvalidUri;
            }
            if (propertyName == "ResolvedNodeId" || propertyName == "RelativePath")
                if (String.IsNullOrEmpty(RelativePath) && NodeId.IsNull(ResolvedNodeId) && (String.IsNullOrEmpty(AppName) || String.IsNullOrEmpty(EndpointUrl) || String.IsNullOrEmpty(ItemName)))
                    return Properties.Resources.ErrorNodeId;
            if (propertyName == "ItemName")
                if (String.IsNullOrEmpty(ItemName) && (String.IsNullOrEmpty(AppName) || String.IsNullOrEmpty(RelativePath) || NodeId.IsNull(ResolvedNodeId) || String.IsNullOrEmpty(EndpointUrl)))
                    return Properties.Resources.ErrorItemName;

            /*if(!Valid)
                return UFUAModel.Properties.Resources.InvalidDynamcSettings;*/
            return null;
        }

    #endregion
    }
}
