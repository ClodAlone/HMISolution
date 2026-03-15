using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using Utilities;
using DevExpress.Xpo;
using System.Reflection;
using UFUAModel;
using System.Xml.Linq;
using DriverCodeBase.UI;
using DriverCodeBase.UI.Controls;

namespace IEC61850.UI
{
    public enum ImportTypeClass
    {
        TypeClass_Unrecognized,
        TypeClass_Standard,
        TypeClass_DO,
        TypeClass_DA,
        TypeClass_Enum,
        TypeClass_Dataset,
        TypeClass_Report
    };
    
    public partial class ImportTagsEditorTree : UserControl, IDisposable
    {
        ImportDataModelIEC61850 importDataModel;
        bool alreadyLoaded = false;
        public GetStationName readStationName;
        Dictionary<string, List<string>> lnodeTypesMap = new Dictionary<string, List<string>>();
        Dictionary<string, Dictionary<string, string>> lnodeStructuresMap = new Dictionary<string, Dictionary<string, string>>();
        Dictionary<string, List<string>> doTypesMap = new Dictionary<string, List<string>>();
        Dictionary<string, Dictionary<string, string>> doStructuresMap = new Dictionary<string, Dictionary<string, string>>();
        Dictionary<string, List<string>> daTypesMap = new Dictionary<string, List<string>>();
        Dictionary<string, Dictionary<string, string>> daStructuresMap = new Dictionary<string, Dictionary<string, string>>();
        Dictionary<string, List<string>> enumTypesMap = new Dictionary<string, List<string>>();
        Dictionary<string, int> enumMoviconTypes = new Dictionary<string, int>();
        Dictionary<string, List<string>> logicalDevicesMap = new Dictionary<string, List<string>>();
        Dictionary<string, List<string>> logicalNodesMap = new Dictionary<string, List<string>>();
        Dictionary<string, List<string>> datasetsMap = new Dictionary<string, List<string>>();
        Dictionary<string, string> reportControlsMap = new Dictionary<string, string>();
        Dictionary<string, List<DatasetVar>> importDatasetsMap = new Dictionary<string, List<DatasetVar>>();
        int globalID = 0;
        const int typeStruct = (int)DataType.String + 1;
        BaseImportTree baseImportTree;

        public ImportTagsEditorTree()
        {
            InitializeComponent();

            Loaded += (o, e) =>
            {
                if (alreadyLoaded)
                {                    
                    importDataModel = null;                 
                    return;
                }

                alreadyLoaded = true;
                List<string> lista = DataContext as List<string>;
                if (lista == null || lista.Count < 2)
                    return;

                Assembly a = Assembly.GetAssembly(this.GetType());
                string DriverName = a.GetName().Name;
                DriverName = DriverName.Replace(".UI", "");

                // Add column to the Import Grid
                List<BaseImportTree.GridColData> columns = new List<BaseImportTree.GridColData>();
                columns.Add(new BaseImportTree.GridColData()
                {
                    colName = "Name",
                    bindingName = "TagName",
                    colWidth = 300
                });
                columns.Add(new BaseImportTree.GridColData()
                {
                    colName = "Address",
                    bindingName = "TagAddress",
                    colWidth = 250
                });
                columns.Add(new BaseImportTree.GridColData()
                {
                    colName = "Type",
                    bindingName = "TagType",
                    colWidth = 200
                });
                columns.Add(new BaseImportTree.GridColData()
                {
                    colName = "Description",
                    bindingName = "TagDescription",
                    colWidth = 210
                });

                baseImportTree = new BaseImportTree(DriverName, lista[0],
                    (lista[1].ToLower().IndexOf("true") != -1), columns);
                baseImportTree.SetVisibleButtons();
                baseImportTree.SetFileFilter("SCL files|*.cid;*.scd;*.icd");
                baseImportTree.LoadImportFile = LoadFile;
                MainStack.Children.Add(baseImportTree);

                readStationName = baseImportTree.funcGetStationName();

                DataContext = this;
            };

        }

        public void ImportSelectedTags()
        {
            string importfolder = baseImportTree.ReadFolderName();
            List<ImportData> list = new List<ImportData>();
            list = baseImportTree.GetSelectedTags();
            string stationName = baseImportTree.ReadStationName();
            int behaviorExistingTags = baseImportTree.GetBehaviorForExistingTags();
            int behaviorDynamicLink = baseImportTree.GetBehaviorForDynamicLink();

            if (list.Count > 0)
            {               
                Dictionary<int, ImportData> mapVar = new Dictionary<int, ImportData>();
                Dictionary<int, ImportData> tempMapVar = new Dictionary<int, ImportData>();
                foreach (var elemList in list)
                {
                    ImportDataIEC61850 elem = elemList as ImportDataIEC61850;
                    if (elem != null)
                    {
                        if((elem.TypeClass != ImportTypeClass.TypeClass_Dataset) && (elem.TypeClass != ImportTypeClass.TypeClass_Report))
                        {
                            tempMapVar[elem.Id] = elem;
                        }
                        // Special case datasets or reports: add only the members of the
                        // dataset (report), not the dataset (report) itself
                        else
                        {
                            AddMembersOfAdataSetToTheImportMap(ref tempMapVar, elem);
                        }
                    }
                }

                // Copy the variable map, removing, eventually, undesired parents
                foreach (KeyValuePair<int, ImportData> kvp in tempMapVar)
                {
                    mapVar[kvp.Key] = kvp.Value;
                    if(mapVar.ContainsKey(kvp.Value.parentId))
                    {
                        mapVar.Remove(kvp.Value.parentId);
                    }
                }

                List<ImportTag> taglist = new List<ImportTag>();
                Dictionary<string, ImportPrototype> protoMap = new Dictionary<string, ImportPrototype>();
                foreach (KeyValuePair<int, ImportData> kvp in mapVar)
                {
                    ImportPrototype proto = null;
                    ImportDataIEC61850 elem = kvp.Value as ImportDataIEC61850;
                    if(elem == null)
                    {
                        continue;
                    }
                    // Manage structures and elements of structures
                    string parentName = String.Empty;
                    bool isStructure = false;
                    if((elem.parentId != -1) && (elem.Parent != null))
                    {
                        if(((ImportDataIEC61850)elem.Parent).TagTypeInt == typeStruct)
                        {
                            parentName = elem.Parent.Name;
                        }
                    }
                    if((elem.Children.Count() > 0) && ((ImportDataIEC61850)elem).TagTypeInt == typeStruct)
                    {
                        switch(elem.TypeClass)
                        {
                            case ImportTypeClass.TypeClass_DA:
                                if(daTypesMap.ContainsKey(elem.szType))
                                {
                                    proto = addPrototype(protoMap, elem);
                                    if(proto == null)
                                    {
                                        continue;
                                    }
                                    else
                                    {
                                        isStructure = true;
                                    }
                                }
                                break;
                        }
                    }

                    // Prepare the string of the dynamic settings 
                    IEC61850DynTagSettings sp = new IEC61850DynTagSettings();
                    if (!sp.ParseAddress(elem.Address))
                    {
                        continue;
                    }
                    sp.StationName = stationName;

                    // Set the MMS data type
                    uint stringLength = 0;
                    sp.MMSDataType = GetMMSTypeId(elem.szType, ((ImportDataIEC61850)elem).TagTypeInt, ((ImportDataIEC61850)elem).TypeClass, out stringLength);
                    if(stringLength > 0)
                    {
                        sp.DataMaximumLength = stringLength;
                    }

                    // Functional Constraint read only? --> Task type = Input
                    if(FunctionalConstraintIsReadOnly(sp.FunctionalConstraint) == true)
                    {
                        sp.TagLinkType = (int)DriverCodeBase.Enumerators.LinkType.Input;
                    }

                    // Special case reports
                    if(!String.IsNullOrWhiteSpace(elem.ReportName))
                    {
                        // Parse the RCB address
                        IEC61850DynTagSettings spReport = new IEC61850DynTagSettings();
                        if (!spReport.ParseAddress(elem.ReportName))
                        {
                            continue;
                        }

                        // Functional Constraint read only ? --> Task type = Input
                        if (FunctionalConstraintIsReadOnly(spReport.FunctionalConstraint) == true)
                            sp.TagLinkType = (int)DriverCodeBase.Enumerators.LinkType.Input;

                        // Set the report parameters
                        sp.ReportLogicalDeviceName = spReport.LogicalDeviceName;
                        sp.ReportLogicalNodeName = spReport.LogicalNodeName;
                        sp.ReportName = spReport.DataItemIdentifier;
                        switch (spReport.FunctionalConstraint) {
                            case FunctionalConstraints.BR:                       
                                sp.ReportType = ReportTypes.Buffered;
                                break;
                            case FunctionalConstraints.RP: 
                                sp.ReportType = ReportTypes.Unbuffered;
                                break;
                        }
                    }

                    if (isStructure)
                        sp.TagLinkType = (int)DriverCodeBase.Enumerators.LinkType.Input;

                    //elem.DynAddress = sp.ToString();
                    string dynAddress = sp.ToString();
                    elem.DynAddress = dynAddress.Replace('$', '.');

                    // Set the supervisor's variable name
                    string importTagName = String.Empty;
                    if (String.IsNullOrWhiteSpace(parentName))
                    {
                        importTagName = elem.Name;
                    }
                    else
                    {
                        importTagName = string.Format("{0}.{1}", parentName, elem.Name);
                    }

                    ImportTag tagtoimport = new ImportTag()
                    {
                        Name = importTagName,
                        DataType = (DataType)((ImportDataIEC61850)elem).TagTypeInt,
                        DynSettings = elem.DynAddress,
                        Folder = importfolder,
                        ModelType = UFUAModel.ModelType.Variable,
                        Description = elem.Description,
                        ArrayDimension = 0,
                        BehaviorForExistingTags = (BehaviorExistingTagsValues)behaviorExistingTags,
                        BehaviorForDynamicLink = (BehaviorDynamicLinkValues)behaviorDynamicLink,
                    };
                    if (isStructure)
                    {
                        tagtoimport.PrototypeModel = proto.Name;
                        tagtoimport.ModelType = UFUAModel.ModelType.ObjectType;                        
                    }
                    taglist.Add(tagtoimport);
                }
                DataContext = new ImportObject() { PrototypesToImport = protoMap.Values.ToList(), TagsToImport = taglist };
            }
            else
            {
                DataContext = this;
            }
        }

        private static ImportPrototype addPrototype(Dictionary<string, ImportPrototype> protoMap, ImportData elem)
        {
            ImportPrototype proto = null;
            if (!protoMap.ContainsKey(elem.szType))
            {
                if (elem.Children.Count != 0)
                {
                    proto = new ImportPrototype();
                    proto.Name = elem.szType;
                    proto.Elements = new List<ImportTag>();

                    foreach (var t in elem.Children)
                    {
                        ImportData el = t as ImportData;
                        if (el != null)
                        {
                            ImportTag a = new ImportTag();

                            if (((ImportDataIEC61850)el).TagTypeInt == typeStruct)
                            {
                                ImportPrototype protoElem = addPrototype(protoMap, el);
                                if (protoElem == null)
                                    continue;

                                a.ModelType = UFUAModel.ModelType.ObjectType;
                                a.PrototypeModel = protoElem.Name;
                            }
                            else
                            {
                                a.ModelType = ModelType.Variable;
                            }
                            a.ArrayDimension = 0;
                            a.Name = el.Name.Replace('$', '.');
                            a.DataType = (DataType)((ImportDataIEC61850)el).TagTypeInt;
                            a.Description = el.Description;
                            proto.Elements.Add(a);
                        }
                    }
                    protoMap.Add(elem.szType, proto);
                }
            }
            else
                proto = protoMap[elem.szType];
            return proto;
        }

        bool FunctionalConstraintIsReadOnly(FunctionalConstraints fc)
        {
            bool isReadOnly = false;
            switch(fc)
            {
                case FunctionalConstraints.ST:
                case FunctionalConstraints.MX:
                case FunctionalConstraints.SG:
                    isReadOnly = true;
                    break;
            }
            return (isReadOnly);
        }

        MMSDataTypes GetMMSTypeId(string typeString, int moviconType, ImportTypeClass typeClass, out uint stringLength)
        {
            stringLength = 0;
            MMSDataTypes mmsType = MMSDataTypes.Boolean;
            if(moviconType == typeStruct)
            {
                mmsType = MMSDataTypes.Structure;
            }
            else if(typeClass == ImportTypeClass.TypeClass_Enum)
            {
                switch(moviconType)
                {
                    case (int)DataType.SByte:
                        mmsType = MMSDataTypes.Integer8Bits;
                        break;
                    case (int)DataType.Byte:
                        mmsType = MMSDataTypes.UnsignedInteger8Bits;
                        break;
                    case (int)DataType.Int16:
                        mmsType = MMSDataTypes.Integer16Bits;
                        break;
                    case (int)DataType.UInt16:
                        mmsType = MMSDataTypes.UnsignedInteger16Bits;
                        break;
                    case (int)DataType.Int32:
                        mmsType = MMSDataTypes.Integer32Bits;
                        break;
                    case (int)DataType.UInt32:
                        mmsType = MMSDataTypes.UnsignedInteger32Bits;
                        break;
                }
            }
            else if (string.Compare(typeString, "BOOLEAN", true) == 0)
            {
                mmsType = MMSDataTypes.Boolean;
            }
            else if (string.Compare(typeString, "INT8", true) == 0)
            {
                mmsType = MMSDataTypes.Integer8Bits;
            }
            else if (string.Compare(typeString, "INT16", true) == 0)
            {
                mmsType = MMSDataTypes.Integer16Bits;
            }
            else if (string.Compare(typeString, "INT24", true) == 0)
            {
                mmsType = MMSDataTypes.Integer32Bits;
            }
            else if (string.Compare(typeString, "INT32", true) == 0)
            {
                mmsType = MMSDataTypes.Integer32Bits;
            }
            else if (string.Compare(typeString, "INT8U", true) == 0)
            {
                mmsType = MMSDataTypes.UnsignedInteger8Bits;
            }
            else if (string.Compare(typeString, "INT16U", true) == 0)
            {
                mmsType = MMSDataTypes.UnsignedInteger16Bits;
            }
            else if (string.Compare(typeString, "INT24U", true) == 0)
            {
                mmsType = MMSDataTypes.UnsignedInteger32Bits;
            }
            else if (string.Compare(typeString, "INT32U", true) == 0)
            {
                mmsType = MMSDataTypes.UnsignedInteger32Bits;
            }
            else if (string.Compare(typeString, "FLOAT32", true) == 0)
            {
                mmsType = MMSDataTypes.FloatingPoint32Bits;
            }
            else if (string.Compare(typeString, "FLOAT64", true) == 0)
            {
                mmsType = MMSDataTypes.FloatingPoint64Bits;
            }
            else if (string.Compare(typeString, "Quality", true) == 0)
            {
                mmsType = MMSDataTypes.BitString;
                stringLength = 13;
            }
            else if (string.Compare(typeString, "Timestamp", true) == 0)
            {
                mmsType = MMSDataTypes.UTCTime;
                stringLength = 50;
            }
            else if (string.Compare(typeString, "VisString32", true) == 0)
            {
                mmsType = MMSDataTypes.VisibleString;
                stringLength = 32;
            }
            else if (string.Compare(typeString, "VisString64", true) == 0)
            {
                mmsType = MMSDataTypes.VisibleString;
                stringLength = 64;
            }
            else if (string.Compare(typeString, "VisString129", true) == 0)
            {
                mmsType = MMSDataTypes.VisibleString;
                stringLength = 129;
            }
            else if (string.Compare(typeString, "VisString255", true) == 0)
            {
                mmsType = MMSDataTypes.VisibleString;
                stringLength = 255;
            }
            else if (string.Compare(typeString, "Octet64", true) == 0)
            {
                mmsType = MMSDataTypes.OctetString;
                stringLength = 64;
            }
            else if (string.Compare(typeString, "EntryTime", true) == 0)
            {
                mmsType = MMSDataTypes.BinaryTime;
                stringLength = 50;
            }
            else if (string.Compare(typeString, "Check", true) == 0)
            {
                mmsType = MMSDataTypes.BitString;
                stringLength = 2;
            }
            else if (string.Compare(typeString, "Dbpos", true) == 0)
            {
                mmsType = MMSDataTypes.UnsignedInteger8Bits;
            }
            else if (string.Compare(typeString, "OptFlds", true) == 0)
            {
                mmsType = MMSDataTypes.BitString;
                stringLength = 10;
            }
            else if (string.Compare(typeString, "TrgOps", true) == 0)
            {
                mmsType = MMSDataTypes.BitString;
                stringLength = 6;
            }

            return (mmsType);
        }

        void AddMembersOfAdataSetToTheImportMap(ref Dictionary<int, ImportData> mapVar, ImportData elem)
        {
            if((elem == null) || (elem.Children == null) || (elem.Children.Count < 1))
            {
                return;
            }
            foreach (ImportData child in elem.Children)
            {
                if (child != null)
                {
                    mapVar[child.Id] = child;
                }
            }
        }
        
        // Supported type of files: SCL (System Configuration description Language) files with one of the following extensions,
        // .CID = Configured IED Description;
        // .ICD = IED Capability Description;
        // .SCD = System Configuration Description.
        private ImportDataModel LoadFile(string file)
        {            
            file = file.ToLower();
            if (file.Contains(".cid") || file.Contains(".scd") || file.Contains(".icd"))
            {
                //txtTitle.Text = string.Format("{0} {1}", Properties.Resources.Import_Device_Variables, file);
                //txtTitle.Refresh();

                // Empty internal lists and dictionaries
                //stationList.Clear();
                lnodeTypesMap.Clear();
                lnodeStructuresMap.Clear();
                doTypesMap.Clear();
                doStructuresMap.Clear();
                daTypesMap.Clear();
                daStructuresMap.Clear();
                enumTypesMap.Clear();
                enumMoviconTypes.Clear();
                logicalDevicesMap.Clear();
                logicalNodesMap.Clear();
                datasetsMap.Clear();
                reportControlsMap.Clear();
                importDatasetsMap.Clear();
                globalID = 0;

                //tbReportNamePostfix.Text = tbReportNamePostfix.Text.Trim();                               
                XDocument tpyDoc = null;
                try
                {
                    using (new WaitCursor())
                    {                        
                        importDataModel = new ImportDataModelIEC61850(readStationName);

                        // Load the file
                        tpyDoc = XDocument.Load(file);
                        if (tpyDoc != null)
                        {
                            // Check if the file is a valid SCL file
                            if(IsSclFile(ref tpyDoc, file))
                            {
                                // Parse Data Templates
                                SclFileParseDataTypeTemplates(ref tpyDoc, file);

                                // Parse Logical Devices
                                if(!SclFileParseLogicalDevices(ref tpyDoc, file))
                                {
                                    string message;
                                    message = String.Format(Properties.Resources.IEC61850InvalidSclFile, file);
                                    MessageBox.Show(message, Properties.Resources.ImportMsgBoxTitle);
                                }

                                // Add Tags to the tree
                                AddTags();
                                System.Diagnostics.Debug.WriteLine("Number of tags in the tree (level 0) = {0}", importDataModel.Children.Count);
                            }
                            else
                            {
                                string message;
                                message = String.Format(Properties.Resources.IEC61850InvalidSclFile, file);
                                MessageBox.Show(message, Properties.Resources.ImportMsgBoxTitle);
                            }
                        }
                        else
                        {
                            string message;
                            message = String.Format(Properties.Resources.IEC61850InvalidXmlFile, file);
                            MessageBox.Show(message, Properties.Resources.ImportMsgBoxTitle);                          
                        }
                    }
                }
                catch (Exception ex)
                {
                    string message;
                    message = String.Format(Properties.Resources.ImportExceptionFileLoading, file, ex.Message);
                    MessageBox.Show(message, Properties.Resources.ImportMsgBoxTitle);
                    if (tpyDoc != null)
                    {
                        tpyDoc = null;
                    }
                    //return;
                    importDataModel = null;
                }
                if (tpyDoc != null)
                {
                    tpyDoc = null;
                }
            }

            return importDataModel;
        }

        // Parse the SCL file contents for data type template definitions
        void SclFileParseDataTypeTemplates(ref XDocument tpyDoc, string fileName)
        {
            try
            {
                // Check if the SCL file contains definitions of data templates
                IEnumerable<XElement> DataTypeTemplates = tpyDoc.Elements().Where(p => p.Name.LocalName == "SCL").Elements().Where(q => q.Name.LocalName == "DataTypeTemplates");
                if (DataTypeTemplates.Count() <= 0)
                {
                    return;
                }
                foreach (XElement dataTypeTemplatesElement in DataTypeTemplates)
                {
                    // Parse the SCL file contents for Logical Nodes Types
                    SclFileParseLNodeTypes(dataTypeTemplatesElement, fileName);
                    // Parse the SCL file contents for DO Types
                    SclFileParseDOTypes(dataTypeTemplatesElement, fileName);
                    // Parse the SCL file contents for DA Types
                    SclFileParseDATypes(dataTypeTemplatesElement, fileName);
                    // Parse the SCL file contents for Enumeration Types
                    SclFileParseEnumTypes(dataTypeTemplatesElement, fileName);
                }
            }
            catch (Exception ex)
            {
                string message;
                message = String.Format(Properties.Resources.ImportExceptionFileLoading, fileName, ex.Message);
                MessageBox.Show(message, Properties.Resources.ImportMsgBoxTitle);
            }
        }

        // Parse the SCL file contents for Logical Nodes Types
        void SclFileParseLNodeTypes(XElement dataTypeTemplate, string fileName)
        {
            try
            {

                IEnumerable<XElement> lnodeTypes = dataTypeTemplate.Elements().Where(p => p.Name.LocalName == "LNodeType");
                if (lnodeTypes.Count() <= 0)
                {
                    return;
                }

                foreach (XElement typeInfo in lnodeTypes)
                {
                    ParseSingleLNodeType(typeInfo, fileName);
                }
            }
            catch (Exception ex)
            {
                string message;
                message = String.Format(Properties.Resources.ImportExceptionFileLoading, fileName, ex.Message);
                MessageBox.Show(message, Properties.Resources.ImportMsgBoxTitle);
            }
        }

        // Get an attribute value in a safe way (no exceptions)
        string GetAttributeValue(XElement xElement, string attributeName)
        {
            string attributeValue = String.Empty;
            if(!String.IsNullOrWhiteSpace(attributeName))
            {
                XAttribute xAttribute = xElement.Attribute(attributeName);
                if(xAttribute != null)
                {
                    attributeValue = xAttribute.Value;
                }
            }
            return (attributeValue);
        }

        // Get an attribute Boolean value in a safe way (no exceptions)
        bool GetAttributeBoolValue(XElement xElement, string attributeName)
        {
            bool attributeBoolValue = false;
            string attributeValue = GetAttributeValue(xElement, attributeName);
            if(attributeValue == "true")
            {
                attributeBoolValue = true;
            }
            return (attributeBoolValue);
        }

        // Get an attribute Boolean value in a safe way (no exceptions). The default value is true.
        bool GetAttributeBoolValueDefaultTrue(XElement xElement, string attributeName)
        {
            bool attributeBoolValue = true;
            string attributeValue = GetAttributeValue(xElement, attributeName);
            if (attributeValue == "false")
            {
                attributeBoolValue = false;
            }
            return (attributeBoolValue);
        }

        void ParseSingleLNodeType(XElement typeInfo, string fileName)
        {
            try
            {
                //string lnodeID = typeInfo.Attribute("id").Value;
                string lnodeID = GetAttributeValue(typeInfo, "id");
                if (String.IsNullOrWhiteSpace(lnodeID) || lnodeTypesMap.ContainsKey(lnodeID))
                {
                    return;
                }
                IEnumerable<XElement> doElements = typeInfo.Elements().Where(p => p.Name.LocalName == "DO");
                if(doElements.Count() <= 0)
                {
                    return;
                }
                List<string> doData = new List<string>();
                Dictionary<string, string> infoMap = new Dictionary<string, string>();
                foreach(XElement doElement in doElements)
                {
                    //string doName = doElement.Attribute("name").Value;
                    //string doType = doElement.Attribute("type").Value;
                    string doName = GetAttributeValue(doElement, "name");
                    string doType = GetAttributeValue(doElement, "type");
                    if (!String.IsNullOrWhiteSpace(doName) && !String.IsNullOrWhiteSpace(doType))
                    {
                        doData.Add(string.Format("{0} | {1}", doName, doType));
                        infoMap[doName] = doType;
                    }
                }
                if(doData.Count > 0)
                {
                    lnodeTypesMap[lnodeID] = doData;
                    lnodeStructuresMap[lnodeID] = infoMap;
                }
            }
            catch (Exception ex)
            {
                string message;
                message = String.Format(Properties.Resources.ImportExceptionFileLoading, fileName, ex.Message);
                MessageBox.Show(message, Properties.Resources.ImportMsgBoxTitle);
            }
        }

        // Parse the SCL file contents for DO Types
        void SclFileParseDOTypes(XElement dataTypeTemplate, string fileName)
        {
            try
            {

                IEnumerable<XElement> doTypes = dataTypeTemplate.Elements().Where(p => p.Name.LocalName == "DOType");
                if (doTypes.Count() <= 0)
                {
                    return;
                }

                foreach (XElement typeInfo in doTypes)
                {
                    ParseSingleDOType(typeInfo, fileName);
                }
            }
            catch (Exception ex)
            {
                string message;
                message = String.Format(Properties.Resources.ImportExceptionFileLoading, fileName, ex.Message);
                MessageBox.Show(message, Properties.Resources.ImportMsgBoxTitle);
            }
        }

        void ParseSingleDOType(XElement typeInfo, string fileName)
        {
            try
            {
                //string doID = typeInfo.Attribute("id").Value;
                string doID = GetAttributeValue(typeInfo, "id");
                if (String.IsNullOrWhiteSpace(doID) || doTypesMap.ContainsKey(doID))
                {
                    return;
                }
                IEnumerable<XElement> daElements = typeInfo.Elements().Where(p => p.Name.LocalName == "DA");
                IEnumerable<XElement> sdoElements = typeInfo.Elements().Where(p => p.Name.LocalName == "SDO");
                if ((daElements.Count() <= 0) && (sdoElements.Count() <= 0))
                {
                    return;
                }

                List<string> daSdoData = new List<string>();
                Dictionary<string, string> infoMap = new Dictionary<string, string>();

                // Parse <DA> elements
                foreach (XElement daElement in daElements)
                {
                    //string daName = daElement.Attribute("name").Value;
                    //string daType = daElement.Attribute("bType").Value;
                    //string daFC = daElement.Attribute("fc").Value;
                    string daName = GetAttributeValue(daElement, "name");
                    string daType = GetAttributeValue(daElement, "bType");
                    string daFC = GetAttributeValue(daElement, "fc");
                    if (!String.IsNullOrWhiteSpace(daName) && !String.IsNullOrWhiteSpace(daType))
                    {
                        if ((daType == "Struct") || (daType == "Enum"))
                        {
                            //daType = daElement.Attribute("type").Value;
                            daType = GetAttributeValue(daElement, "type");
                        }
                        if (!String.IsNullOrWhiteSpace(daType))
                        {
                            if(daFC == null)
                            {
                                daFC = String.Empty;
                            }
                            daSdoData.Add(string.Format("{0} | {1} | {2}", daName, daType, daFC));
                            infoMap[daName] = string.Format("{0} | {1}", daType, daFC);
                        }
                    }
                }

                // Parse <SDO> elements
                foreach (XElement sdoElement in sdoElements)
                {
                    //string sdoName = sdoElement.Attribute("name").Value;
                    //string sdoType = sdoElement.Attribute("type").Value;
                    string sdoName = GetAttributeValue(sdoElement, "name");
                    string sdoType = GetAttributeValue(sdoElement, "type");
                    if (!String.IsNullOrWhiteSpace(sdoName) && !String.IsNullOrWhiteSpace(sdoType))
                    {
                        daSdoData.Add(string.Format("{0} | {1} | SDO", sdoName, sdoType));
                        infoMap[sdoName] = string.Format("{0} | SDO", sdoType);
                    }
                }

                if (daSdoData.Count > 0)
                {
                    doTypesMap[doID] = daSdoData;
                    doStructuresMap[doID] = infoMap;
                }
            }
            catch (Exception ex)
            {
                string message;
                message = String.Format(Properties.Resources.ImportExceptionFileLoading, fileName, ex.Message);
                MessageBox.Show(message, Properties.Resources.ImportMsgBoxTitle);
            }
        }

        // Parse the SCL file contents for DA Types
        void SclFileParseDATypes(XElement dataTypeTemplate, string fileName)
        {
            try
            {
                IEnumerable<XElement> daTypes = dataTypeTemplate.Elements().Where(p => p.Name.LocalName == "DAType");
                if (daTypes.Count() <= 0)
                {
                    return;
                }

                foreach (XElement typeInfo in daTypes)
                {
                    ParseSingleDAType(typeInfo, fileName);
                }
            }
            catch (Exception ex)
            {
                string message;
                message = String.Format(Properties.Resources.ImportExceptionFileLoading, fileName, ex.Message);
                MessageBox.Show(message, Properties.Resources.ImportMsgBoxTitle);
            }
        }

        void ParseSingleDAType(XElement typeInfo, string fileName)
        {
            try
            {
                //string daID = typeInfo.Attribute("id").Value;
                string daID = GetAttributeValue(typeInfo, "id");
                if (String.IsNullOrWhiteSpace(daID) || daTypesMap.ContainsKey(daID))
                {
                    return;
                }
                IEnumerable<XElement> bdaElements = typeInfo.Elements().Where(p => p.Name.LocalName == "BDA");
                if (bdaElements.Count() <= 0)
                {
                    return;
                }

                List<string> bdaData = new List<string>();
                Dictionary<string, string> infoMap = new Dictionary<string, string>();

                // Parse <BDA> elements
                foreach (XElement bdaElement in bdaElements)
                {
                    //string bdaName = bdaElement.Attribute("name").Value;
                    //string bdaType = bdaElement.Attribute("bType").Value;
                    string bdaName = GetAttributeValue(bdaElement, "name");
                    string bdaType = GetAttributeValue(bdaElement, "bType");
                    if (!String.IsNullOrWhiteSpace(bdaName) && !String.IsNullOrWhiteSpace(bdaType))
                    {
                        if ((bdaType == "Struct") || (bdaType == "Enum"))
                        {
                            //bdaType = bdaElement.Attribute("type").Value;
                            bdaType = GetAttributeValue(bdaElement, "type");
                        }
                        if (!String.IsNullOrWhiteSpace(bdaType))
                        {
                            bdaData.Add(string.Format("{0} | {1}", bdaName, bdaType));
                            infoMap[bdaName] = bdaType;
                        }
                    }
                }

                if (bdaData.Count > 0)
                {
                    daTypesMap[daID] = bdaData;
                    daStructuresMap[daID] = infoMap;
                }
            }
            catch (Exception ex)
            {
                string message;
                message = String.Format(Properties.Resources.ImportExceptionFileLoading, fileName, ex.Message);
                MessageBox.Show(message, Properties.Resources.ImportMsgBoxTitle);
            }
        }

        // Parse the SCL file contents for Enumeration Types
        void SclFileParseEnumTypes(XElement dataTypeTemplate, string fileName)
        {
            try
            {
                IEnumerable<XElement> enumTypes = dataTypeTemplate.Elements().Where(p => p.Name.LocalName == "EnumType");
                if (enumTypes.Count() <= 0)
                {
                    return;
                }

                foreach (XElement typeInfo in enumTypes)
                {
                    ParseSingleEnumType(typeInfo, fileName);
                }
            }
            catch (Exception ex)
            {
                string message;
                message = String.Format(Properties.Resources.ImportExceptionFileLoading, fileName, ex.Message);
                MessageBox.Show(message, Properties.Resources.ImportMsgBoxTitle);
            }
        }

        void ParseSingleEnumType(XElement typeInfo, string fileName)
        {
            try
            {
                //string enumID = typeInfo.Attribute("id").Value;
                string enumID = GetAttributeValue(typeInfo, "id");
                if (String.IsNullOrWhiteSpace(enumID) || daTypesMap.ContainsKey(enumID))
                {
                    return;
                }
                IEnumerable<XElement> enumElements = typeInfo.Elements().Where(p => p.Name.LocalName == "EnumVal");
                if (enumElements.Count() <= 0)
                {
                    return;
                }

                List<string> enumData = new List<string>();

                // Parse <EnumVal> elements
                foreach (XElement enumElement in enumElements)
                {
                    string enumName = enumElement.Value;
                    //string enumVal = enumElement.Attribute("ord").Value;
                    string enumVal = GetAttributeValue(enumElement, "ord");
                    if (!String.IsNullOrWhiteSpace(enumName) && !String.IsNullOrWhiteSpace(enumVal))
                    {
                        enumData.Add(string.Format("{0} | {1}", enumName, enumVal));
                    }
                }

                if ((enumData.Count > 0) && (SetCorrespondingMoviconType(enumID, enumData) == true))
                {
                    enumTypesMap[enumID] = enumData;
                }
            }
            catch (Exception ex)
            {
                string message;
                message = String.Format(Properties.Resources.ImportExceptionFileLoading, fileName, ex.Message);
                MessageBox.Show(message, Properties.Resources.ImportMsgBoxTitle);
            }
        }

        bool SetCorrespondingMoviconType(string enumType, List<string> enumValues)
        {
            int minValue = 2147483647;
            int maxValue = -2147483648;
            foreach(string enumValue in enumValues)
            {
                int index = enumValue.IndexOf(" | ");
                if(index > 0)
                {
                    string enumStringValue = enumValue.Substring(index + 3);
                    int value = 0;
                    if(int.TryParse(enumStringValue, out value))
                    {
                        if(value < minValue)
                        {
                            minValue = value;
                        }
                        if(value > maxValue)
                        {
                            maxValue = value;
                        }
                    }
                }
            }

            if(maxValue < minValue)
            {
                return(false);
            }

            int moviconType = (int)DataType.Byte;
            bool signedValue = false;
            if(minValue < 0)
            {
                signedValue = true;
            }

            if (signedValue == true)
            {
                moviconType = (int)DataType.SByte;
                if((minValue < -32768) || (maxValue > 32767))
                {
                    moviconType = (int)DataType.Int32;
                }
                else if ((minValue < -128) || (maxValue > 127))
                {
                    moviconType = (int)DataType.Int16;
                }
            }
            else
            {
                moviconType = (int)DataType.Byte;
                if (maxValue > 65535)
                {
                    moviconType = (int)DataType.UInt32;
                }
                else if (maxValue > 255)
                {
                    moviconType = (int)DataType.UInt16;
                }
            }

            enumMoviconTypes[enumType] = moviconType;
            return (true);
        }

        // Check if the file contains the XML element "<SCL>" 
        bool IsSclFile(ref XDocument tpyDoc, string fileName)
        {
            bool isValid = false;
            try
            {
                //isValid = tpyDoc.Elements().Where(p => p.Name.LocalName == "SCL").Elements().Count() > 0;
                isValid = tpyDoc.Elements().Where(p => p.Name.LocalName == "SCL").Count() > 0;
            }
            catch (Exception ex)
            {
                string message;
                message = String.Format(Properties.Resources.ImportExceptionFileLoading, fileName, ex.Message);
                MessageBox.Show(message, Properties.Resources.ImportMsgBoxTitle);
            }

            return (isValid);
        }

        public int GetMoviconTypeId(string Type, ref uint VarSize)
        {
            int nType = -1;
            VarSize = 0;
            Type.Trim();
            if (Type.Length == 0)
                return (nType);
            Type = Type.ToUpper();
            if (Type.Contains("BIT"))
            {
                nType = (int)DataType.Boolean;
                VarSize = 1;
            }
            else if (Type.Contains("BOOL"))
            {
                nType = (int)DataType.Boolean;
                VarSize = 1;
            }
            else if (Type.Contains("BYTE"))
            {
                nType = (int)DataType.Byte;
                VarSize = 1;
            }
            else if (Type.Contains("WORD"))
            {
                nType = (int)DataType.UInt16;
                VarSize = 2;
            }
            else if (Type.Contains("DWORD"))
            {
                nType = (int)DataType.UInt32;
                VarSize = 4;
            }
            else if (Type.Contains("INT"))
            {
                nType = (int)DataType.Int16;
                VarSize = 2;
            }
            else if (Type.Contains("DINT"))
            {
                nType = (int)DataType.Int32;
                VarSize = 4;
            }
            else if (Type.Contains("REAL"))
            {
                nType = (int)DataType.Float;
                VarSize = 4;
            }
            else if (Type.Contains("CHAR"))
            {
                nType = (int)DataType.SByte;
                VarSize = 1;
            }
            return (nType);

        }

        bool SclFileParseLogicalDevices(ref XDocument tpyDoc, string fileName)
        {
            try
            {
                // Check if the SCL file contains definitions of logical devices
                IEnumerable<XElement> logicalDevices = tpyDoc.Elements().Where(p => p.Name.LocalName == "SCL").Elements().Where(q => q.Name.LocalName == "IED");
                if (logicalDevices.Count() <= 0)
                {
                    return(false);
                }
                foreach (XElement logicalDeviceElement in logicalDevices)
                {
                    // Parse the SCL file contents for an IED settings
                    SclFileParseIED(logicalDeviceElement, fileName);
                }
            }
            catch (Exception ex)
            {
                string message;
                message = String.Format(Properties.Resources.ImportExceptionFileLoading, fileName, ex.Message);
                MessageBox.Show(message, Properties.Resources.ImportMsgBoxTitle);
                logicalDevicesMap.Clear();
                return (false);
            }

            if (logicalDevicesMap.Count() <= 0)
            {
                return (false);
            }
            else
            {
                return (true);
            }
        }

        // Parse the SCL file contents for an IED settings
        void SclFileParseIED(XElement logicalDeviceElement, string fileName)
        {
            try
            {
                // Get the the first part of the Logical Device name (the name of the IED)
                string iedName = GetAttributeValue(logicalDeviceElement, "name");
                if (String.IsNullOrWhiteSpace(iedName))
                {
                    return;
                }

                // Complete the name of the logical device and store it into the map
                // Parse the elements <AccessPoint>
                IEnumerable<XElement> accessPoints = logicalDeviceElement.Elements().Where(p => p.Name.LocalName == "AccessPoint");
                if (accessPoints.Count() <= 0)
                {
                    return;
                }
                foreach(XElement accessPointElement in accessPoints)
                {
                    // Parse the elements <Server>
                    IEnumerable<XElement> servers = accessPointElement.Elements().Where(p => p.Name.LocalName == "Server");
                    if(servers.Count() <= 0)
                    {
                        continue;
                    }
                    foreach(XElement serverElement in servers)
                    {
                        // Parse the elements <LDevice>
                        IEnumerable<XElement> ldevices = serverElement.Elements().Where(p => p.Name.LocalName == "LDevice");
                        if(ldevices.Count() <= 0)
                        {
                            continue;
                        }
                        foreach(XElement ldeviceElement in ldevices)
                        {
                            //string instance = ldeviceElement.Attribute("inst").Value;
                            string instance = GetAttributeValue(ldeviceElement, "inst");
                            if (String.IsNullOrWhiteSpace(instance))
                            {
                                continue;
                            }

                            // Complete the name of the logical device
                            string ldName = iedName + instance;

                            if (logicalDevicesMap.ContainsKey(ldName))
                            {
                                continue;
                            }

                            // Parse the element <LN0> (Logical Node 0)
                            IEnumerable<XElement> ln0List = ldeviceElement.Elements().Where(p => p.Name.LocalName == "LN0");
                            //if (ln0List.Count() != 1)
                            //{
                            //    continue;
                            //}
                            List<string> logicalNodeNames = new List<string>();
                            foreach (XElement ln0 in ln0List)
                            {
                                //string lnName = ln0.Attribute("prefix").Value;
                                //lnName += ln0.Attribute("lnClass").Value;
                                //lnName += ln0.Attribute("inst").Value;
                                string lnName = GetAttributeValue(ln0, "prefix") + GetAttributeValue(ln0, "lnClass") + GetAttributeValue(ln0, "inst");
                                if (String.IsNullOrWhiteSpace(lnName))
                                {
                                    continue;
                                }
                                logicalNodeNames.Add(lnName);
                                string lnSearchKey = String.Format("{0}/{1}", ldName, lnName);
                                if(logicalNodesMap.ContainsKey(lnSearchKey))
                                {
                                    continue;
                                }
                                //string lnType = ln0.Attribute("lnType").Value;
                                string lnType = GetAttributeValue(ln0, "lnType");
                                if (String.IsNullOrWhiteSpace(lnType))
                                {
                                    continue;
                                }
                                string nodeType = string.Format("Type {0}", lnType);
                                List<string> logicalNodeElements = new List<string>();
                                logicalNodeElements.Add(nodeType);

                                // Parse the elements <DOI>
                                IEnumerable<XElement> doiList = ln0.Elements().Where(p => p.Name.LocalName == "DOI");
                                if (doiList.Count() > 0)
                                {
                                    foreach (XElement doi in doiList)
                                    {
                                        string name = GetAttributeValue(doi, "name");
                                        if(String.IsNullOrWhiteSpace(name))
                                        {
                                            continue;
                                        }
                                        string doiName = string.Format("DOI {0}", name);
                                        logicalNodeElements.Add(doiName);

                                        // Parse the elements <DAI>
                                        IEnumerable<XElement> daiList = doi.Elements().Where(p => p.Name.LocalName == "DAI");
                                        if (daiList.Count() > 0)
                                        {
                                            foreach (XElement dai in daiList)
                                            {
                                                //string nameAttribute = dai.Attribute("name").Value;
                                                string nameAttribute = GetAttributeValue(dai, "name");
                                                if (String.IsNullOrWhiteSpace(nameAttribute))
                                                {
                                                    continue;
                                                }
                                                string daiName = string.Format("DAI {0}", name);
                                                logicalNodeElements.Add(daiName);
                                            }
                                        }
                                    }
                                }

                                if(logicalNodeElements.Count > 0)
                                {
                                    logicalNodesMap[lnSearchKey] = logicalNodeElements;
                                }

                                // Parse the list of datasets
                                SclFileParseNodeDatasets(ln0, iedName, ldName, lnName);

                                // Parse the list of Report Controls
                                SclFileParseNodeReportControls(ln0, ldName, lnName);
                            }

                            // Parse the elements <LN> (Logical Nodes)
                            IEnumerable<XElement> lnList = ldeviceElement.Elements().Where(p => p.Name.LocalName == "LN");
                            if (lnList.Count() <= 0)
                            {
                                continue;
                            }
                            foreach (XElement ln in lnList)
                            {
                                string lnName = GetAttributeValue(ln, "prefix") + GetAttributeValue(ln, "lnClass") + GetAttributeValue(ln, "inst");
                                if (String.IsNullOrWhiteSpace(lnName))
                                {
                                    continue;
                                }
                                logicalNodeNames.Add(lnName);
                                string lnSearchKey = String.Format("{0}/{1}", ldName, lnName);
                                if (logicalNodesMap.ContainsKey(lnSearchKey))
                                {
                                    continue;
                                }
                                string lnType = GetAttributeValue(ln, "lnType");
                                if (String.IsNullOrWhiteSpace(lnType))
                                {
                                    continue;
                                }
                                string nodeType = string.Format("Type {0}", lnType);
                                List<string> logicalNodeElements = new List<string>();
                                logicalNodeElements.Add(nodeType);

                                // Parse the elements <DOI>
                                IEnumerable<XElement> doiList = ln.Elements().Where(p => p.Name.LocalName == "DOI");
                                if (doiList.Count() > 0)
                                {
                                    foreach (XElement doi in doiList)
                                    {
                                        string name = GetAttributeValue(doi, "name");
                                        if (String.IsNullOrWhiteSpace(name))
                                        {
                                            continue;
                                        }
                                        string doiName = string.Format("DOI {0}", name);
                                        logicalNodeElements.Add(doiName);

                                        // Parse the elements <DAI>
                                        IEnumerable<XElement> daiList = doi.Elements().Where(p => p.Name.LocalName == "DAI");
                                        if (daiList.Count() > 0)
                                        {
                                            foreach (XElement dai in daiList)
                                            {
                                                string nameAttribute = GetAttributeValue(dai, "name");
                                                if (String.IsNullOrWhiteSpace(nameAttribute))
                                                {
                                                    continue;
                                                }
                                                string daiName = string.Format("DAI {0}", name);
                                                logicalNodeElements.Add(daiName);
                                            }
                                        }
                                    }
                                }

                                if (logicalNodeElements.Count > 0)
                                {
                                    logicalNodesMap[lnSearchKey] = logicalNodeElements;
                                }

                                // Parse the list of datasets
                                SclFileParseNodeDatasets(ln, iedName, ldName, lnName);

                                // Parse the list of Report Controls
                                SclFileParseNodeReportControls(ln, ldName, lnName);
                            }

                            if(logicalNodeNames.Count > 0)
                            {
                                logicalDevicesMap[ldName] = logicalNodeNames;
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                string message;
                message = String.Format(Properties.Resources.ImportExceptionFileLoading, fileName, ex.Message);
                MessageBox.Show(message, Properties.Resources.ImportMsgBoxTitle);
            }
        }

        // Parse the list of datasets of a Logical Node
        void SclFileParseNodeDatasets(XElement ln, string iedName, string ldName, string lnName)
        {
            if(String.IsNullOrWhiteSpace(iedName) || String.IsNullOrWhiteSpace(ldName) || String.IsNullOrWhiteSpace(lnName))
            {
                return;
            }

            string headerName = string.Format("{0}/{1}$", ldName, lnName);

            // Parse <DataSet> elements
            IEnumerable<XElement> datasetList = ln.Elements().Where(p => p.Name.LocalName == "DataSet");
            foreach(XElement datasetElement in datasetList)
            {
                string datasetName = GetAttributeValue(datasetElement, "name");
                if(String.IsNullOrWhiteSpace(datasetName))
                {
                    continue;
                }
                string datasetCompleteName = headerName + datasetName;

                List<string> datasetElements = new List<string>();

                // Parse <FCDA> elements
                IEnumerable<XElement> fcdaList = datasetElement.Elements().Where(p => p.Name.LocalName == "FCDA");
                foreach (XElement fcdaElement in fcdaList)
                {
                    // Get the name of the device
                    string elementDevice = iedName + GetAttributeValue(fcdaElement, "ldInst");

                    // Get the name of the logical node
                    string elementNode = GetAttributeValue(fcdaElement, "prefix") + GetAttributeValue(fcdaElement, "lnClass") + GetAttributeValue(fcdaElement, "lnInst");
                    if(String.IsNullOrWhiteSpace(elementNode))
                    {
                        continue;
                    }

                    // Get the Functional Constraint
                    string elementFC = GetAttributeValue(fcdaElement, "fc");
                    if (String.IsNullOrWhiteSpace(elementFC))
                    {
                        continue;
                    }

                    // Get the Data Object name
                    string elementDO = GetAttributeValue(fcdaElement, "doName");
                    if (String.IsNullOrWhiteSpace(elementDO))
                    {
                        continue;
                    }

                    // Get the Data Attribute name (optional)
                    string elementDA = GetAttributeValue(fcdaElement, "daName");

                    // Build the element string
                    string element = string.Format("{0}/{1}${2}${3}", elementDevice, elementNode, elementFC, elementDO);
                    if(!String.IsNullOrWhiteSpace(elementDA))
                    {
                        element += "$";
                        element += elementDA;
                    }

                    // Replace the character '.' with '$'
                    element.Replace('.', '$');

                    // Add the element to the list of the dataset elements
                    datasetElements.Add(element);
                }

                if(datasetElements.Count > 0)
                {
                    datasetsMap[datasetCompleteName] = datasetElements;
                }
            }
        }

        // Parse the list of Report Controls of a logical node
        void SclFileParseNodeReportControls(XElement ln, string ldName, string lnName)
        {
            if (String.IsNullOrWhiteSpace(ldName) || String.IsNullOrWhiteSpace(lnName))
            {
                return;
            }

            string headerName = string.Format("{0}/{1}$", ldName, lnName);
            bool isBuffered = false;
            bool isIndexed = true;
            uint maxNumberOfReportInstances = 1; 

            // Parse <ReportControl> elements
            IEnumerable<XElement> reportControlList = ln.Elements().Where(p => p.Name.LocalName == "ReportControl");
            foreach (XElement reportControlElement in reportControlList)
            {
                // Get the name of the RCB
                string rcbName = GetAttributeValue(reportControlElement, "name");
                if (String.IsNullOrWhiteSpace(rcbName))
                {
                    continue;
                }

                // Check if the RCB is buffered or not
                isBuffered = GetAttributeBoolValue(reportControlElement, "buffered");

                // Check if the RCB is indexed or not
                isIndexed = GetAttributeBoolValueDefaultTrue(reportControlElement, "indexed");

                // Get the name of the dataset
                string datasetName = GetAttributeValue(reportControlElement, "datSet");
                if (String.IsNullOrWhiteSpace(datasetName))
                {
                    continue;
                }

                // Build the complete name of the dataset
                string datasetCompleteName = headerName + datasetName;

                // Check the maximum number of instances for this RCB
                maxNumberOfReportInstances = 1;
                IEnumerable<XElement> rptEnabledElements = reportControlElement.Elements().Where(p => p.Name.LocalName == "RptEnabled");
                if (rptEnabledElements.Count() > 0)
                {
                    XElement rptEnabledElement = rptEnabledElements.First();
                    string maxValue = GetAttributeValue(rptEnabledElement, "max");
                    if(!String.IsNullOrWhiteSpace(maxValue))
                    {
                        if(uint.TryParse(maxValue, out maxNumberOfReportInstances) == false)
                        {
                            maxNumberOfReportInstances = 1;
                        }
                        if((maxNumberOfReportInstances < 1) || (maxNumberOfReportInstances > 99))
                        {
                            maxNumberOfReportInstances = 1;
                        }
                    }
                }

                // Add the instances of the RCB to the map of the Report Controls
                for(uint i=1; i<=maxNumberOfReportInstances; i++)
                {
                    // Build the complete name of the report
                    string rcbCompleteName;
                    if(isIndexed || (maxNumberOfReportInstances > 1))
                    {
                        if (!isBuffered)
                        {
                            rcbCompleteName = string.Format("{0}RP${1}{2}", headerName, rcbName, i.ToString("D2"));
                        }
                        else
                        {
                            rcbCompleteName = string.Format("{0}BR${1}{2}", headerName, rcbName, i.ToString("D2"));
                        }
                    }
                    else
                    {
                        if (!isBuffered)
                        {
                            rcbCompleteName = string.Format("{0}RP${1}", headerName, rcbName);
                        }
                        else
                        {
                            rcbCompleteName = string.Format("{0}BR${1}", headerName, rcbName);
                        }
                    }

                    // Add an element to the map of the Report Controls
                    reportControlsMap[rcbCompleteName] = datasetCompleteName;
                }
            }
        }

        // Add Tags to the tree
        void AddTags()
        {
            // Add to the tree the tags of the logical devices
            foreach(KeyValuePair<string, List<string>> kvp in logicalDevicesMap)
            {
                AddLogicalDeviceTags(kvp.Key, kvp.Value);
            }

            // Add to the tree the tags of the datasets
            foreach (KeyValuePair<string, List<string>> kvp in datasetsMap)
            {
                AddDatasetTags(kvp.Key, kvp.Value);
            }

            // Add to the tree the tags of the reports
            foreach (KeyValuePair<string, string> kvp in reportControlsMap)
            {
                AddReportControlTags(kvp.Key, kvp.Value);
            }
        }

        void AddLogicalDeviceTags(string logicalDeviceName, List<string> nodeList)
        {
            if(String.IsNullOrWhiteSpace(logicalDeviceName) || (nodeList == null) || (nodeList.Count <= 0))
            {
                return;
            }

            foreach(string node in nodeList)
            {
                string nodeSearchKey = string.Format("{0}/{1}", logicalDeviceName, node);
                if(!logicalNodesMap.ContainsKey(nodeSearchKey))
                {
                    continue;
                }
                AddLogicalNodeTags(nodeSearchKey, logicalNodesMap[nodeSearchKey]);
            }
        }

        void AddLogicalNodeTags(string nodeCompleteName, List<string> nodeInfo)
        {
            if(String.IsNullOrWhiteSpace(nodeCompleteName) || (nodeInfo.Count < 1))
            {
                return;
            }

            // Retrieve the name of the type of the node
            string nodeTypeInfo = nodeInfo[0];
            if(String.IsNullOrWhiteSpace(nodeTypeInfo) || !nodeTypeInfo.Contains("Type "))
            {
                return;
            }
            string nodeType = nodeTypeInfo.Substring(5);
            if (String.IsNullOrWhiteSpace(nodeType))
            {
                return;
            }

            // Retrieve the info of the node type
            if(!lnodeTypesMap.ContainsKey(nodeType))
            {
                return;
            }
            List<string> logicalNodeTypeInfo = lnodeTypesMap[nodeType];

            // Add the node tags to the tree
            AddNodeTags(nodeCompleteName, logicalNodeTypeInfo);
        }

        void AddNodeTags(string nodeCompleteName, List<string> nodeTypeInfo)
        {
            if (String.IsNullOrWhiteSpace(nodeCompleteName) || (nodeTypeInfo.Count < 1))
            {
                return;
            }

            foreach(string typeInfo in nodeTypeInfo)
            {
                string[] doInfo = typeInfo.Split(new char[] { '|' });
                if(doInfo.Length != 2 )
                {
                    continue;
                }
                string doName = doInfo[0].Trim();
                string doType = doInfo[1].Trim();

                // Add the tags of the Data Object
                AddDOTags(nodeCompleteName, doName, doType);
            }
        }

        // Add the tags of a Data Object
        void AddDOTags(string nodeCompleteName, string doName, string doType)
        {
            if(!doTypesMap.ContainsKey(doType))
            {
                return;
            }

            foreach (string typeInfo in doTypesMap[doType])
            {
                string[] daInfo = typeInfo.Split(new char[] { '|' });
                if (daInfo.Length != 3)
                {
                    continue;
                }

                string daName = daInfo[0].Trim();
                string daType = daInfo[1].Trim();
                string daFC = daInfo[2].Trim();

                if(!IsFunctionalConstraintSupported(daFC) && (string.Compare(daFC, "SDO", true) != 0))
                {
                    continue;
                }

                string daCompleteName = string.Format("{0}${1}${2}${3}", nodeCompleteName, daFC, doName, daName);
                AddDATags(daCompleteName, daName, daType, -1, null);
            }
        }

        bool IsFunctionalConstraintSupported(string functionalConstraint)
        {
            bool isSupported = false;
            if(string.Compare(functionalConstraint, "ST", true) == 0)
            {
                isSupported = true;
            }
            else if (string.Compare(functionalConstraint, "MX", true) == 0)
            {
                isSupported = true;
            }
            else if (string.Compare(functionalConstraint, "SG", true) == 0)
            {
                isSupported = true;
            }
            else if (string.Compare(functionalConstraint, "CO", true) == 0)
            {
                isSupported = true;
            }
            else if (string.Compare(functionalConstraint, "SP", true) == 0)
            {
                isSupported = true;
            }
            else if (string.Compare(functionalConstraint, "SV", true) == 0)
            {
                isSupported = true;
            }
            else if (string.Compare(functionalConstraint, "CF", true) == 0)
            {
                isSupported = true;
            }
            else if (string.Compare(functionalConstraint, "DC", true) == 0)
            {
                isSupported = true;
            }

            return (isSupported);
        }

        void AddDATags(string daCompleteName, string tagName, string daType, int parentID = -1, ImportData parent = null)
        {
            ImportTypeClass typeClass = GetImportTypeClass(daType);
            if(typeClass == ImportTypeClass.TypeClass_Unrecognized)
            {
                return;
            }

            // Enumeration or standard type
            if((typeClass == ImportTypeClass.TypeClass_Enum) || (typeClass == ImportTypeClass.TypeClass_Standard))
            {
                uint varSize = 0;
                int moviconType = GetMoviconVarTypeAndSize(daType, typeClass, out varSize);
                if((moviconType == -1) || (varSize == 0))
                {
                    return;
                }

                ImportData v = importDataModel.addImportData();
                if((parentID == -1) || String.IsNullOrWhiteSpace(tagName))
                {
                    v.Name = daCompleteName;
                }
                else
                {
                    v.Name = tagName;
                }
                v.parentId = parentID;
                v.Id = globalID++;
                v.Address = daCompleteName;
                ((ImportDataIEC61850)v).Type = (DataType)moviconType;
                ((ImportDataIEC61850)v).TagTypeInt = moviconType;
                v.szType = daType;
                if(((ImportDataIEC61850)v).Type == DataType.String)
                {
                    ((ImportDataIEC61850)v).Size = varSize;
                }
                ((ImportDataIEC61850)v).TypeClass = typeClass;
                AddTreeItem(v, parent);
            }

            // DO Type
            else if (typeClass == ImportTypeClass.TypeClass_DO)
            {
                if(!doTypesMap.ContainsKey(daType))
                {
                    return;
                }

                foreach(string daInfo in doTypesMap[daType])
                {
                    string[] daInfoElements = daInfo.Split(new char[] {'|'});
                    if(daInfoElements.Length < 2)
                    {
                        continue;
                    }
                    string daNameInfo = daInfoElements[0].Trim();
                    string daTypeInfo = daInfoElements[1].Trim();
                    string completeName = string.Format("{0}${1}", daCompleteName, daNameInfo);
                    string daFCInfo = String.Empty;
                    if (daInfoElements.Length > 2)
                    {
                        daFCInfo = daInfoElements[2].Trim();
                    }
                    if(!String.IsNullOrWhiteSpace(daFCInfo) && (string.Compare(daFCInfo, "SDO", true) != 0))
                    {
                        if(!IsFunctionalConstraintSupported(daFCInfo))
                        {
                            continue;
                        }
                        int index = completeName.IndexOf("$SDO$");
                        if(index > 0)
                        {
                            string leftString = completeName.Substring(0, index);
                            string rightString = completeName.Substring(index + 5);
                            completeName = String.Empty;
                            completeName = string.Format("{0}${1}${2}", leftString, daFCInfo, rightString);
                        }
                    }

                    // Recursive call
                    AddDATags(completeName, daNameInfo, daTypeInfo, -1, null);
                }
            }

            // DA Type
            else
            {
                if (!daTypesMap.ContainsKey(daType))
                {
                    return;
                }

                // Add a structure variable to the tree of the tags that can be imported
                int varID = -1;
                ImportData varParent = null;
                if(parentID == -1)
                {
                    ImportData v = importDataModel.addImportData();
                    uint varSize = 0;
                    int type = GetMoviconVarTypeAndSize(daType, typeClass, out varSize);
                    varID = globalID++;
                    v.Name = daCompleteName;
                    v.Address = daCompleteName;
                    v.parentId = parentID;
                    v.Id = varID;
                    ((ImportDataIEC61850)v).TypeClass = typeClass;
                    v.szType = daType;
                    ((ImportDataIEC61850)v).TagTypeInt = type;
                    ((ImportDataIEC61850)v).Type = (DataType)type;
                    ((ImportDataIEC61850)v).Size = varSize;
                    AddTreeItem(v, parent);
                    varParent = v;
                }
                else
                {                   
                    varID = parentID;
                    varParent = parent;

                    // special case --> DA object don't "manage" struct except Originator 
                    if (daType == "Originator")
                    {
                        ImportData v = importDataModel.addImportData();
                        uint varSize = 0;
                        int type = GetMoviconVarTypeAndSize(daType, typeClass, out varSize);
                        varID = globalID++;
                        v.Name = tagName;
                        v.Address = tagName;
                        v.parentId = parentID;
                        v.Id = varID;
                        ((ImportDataIEC61850)v).TypeClass = typeClass;
                        v.szType = daType;
                        ((ImportDataIEC61850)v).TagTypeInt = type;
                        ((ImportDataIEC61850)v).Type = (DataType)type;
                        ((ImportDataIEC61850)v).Size = varSize;
                        AddTreeItem(v, parent);
                        varParent = v;
                    }
                }

                foreach (string daInfo in daTypesMap[daType])
                {
                    string[] daInfoElements = daInfo.Split(new char[] { '|' });
                    if (daInfoElements.Length < 2)
                    {
                        continue;
                    }
                    string itemName = daInfoElements[0].Trim();
                    string itemType = daInfoElements[1].Trim();
                    string completeAddress = string.Format("{0}${1}", daCompleteName, itemName);

                    if (parentID != -1)
                    {
                        itemName = string.Format("{0}${1}", tagName, itemName);
                    }
                    //else
                    //{
                    //    itemName = string.Format("{0}${1}", daCompleteName, auxString);
                    //}

                    // Recursive call
                    AddDATags(completeAddress, itemName, itemType, varID, varParent);
                }
            }
        }

        int GetMoviconVarTypeAndSize(string daType, ImportTypeClass typeClass, out uint varSize)
        {
            varSize = 0;
            int moviconType = -1; // Invalid or unsupported
            switch (typeClass)
            {
                case ImportTypeClass.TypeClass_Standard:
                    moviconType = GetMoviconStandardVarTypeAndSize(daType, out varSize);
                    break;

                case ImportTypeClass.TypeClass_Enum:
                    moviconType = GetMoviconEnumVarTypeAndSize(daType, out varSize);
                    break;

                default:
                    if (daTypesMap.ContainsKey(daType))
                    {
                        uint structSize = 0;
                        moviconType = typeStruct;
                        foreach (string daInfo in daTypesMap[daType])
                        {
                            int index = daInfo.IndexOf("| ");
                            if ((index < 0) || (index >= (daInfo.Length - 2)))
                            {
                                continue;
                            }
                            string fieldType = daInfo.Substring(index + 2);
                            uint fieldSize = 0;
                            ImportTypeClass fieldTypeClass = GetImportTypeClass(fieldType);
                            if (fieldTypeClass != ImportTypeClass.TypeClass_Unrecognized)
                            {
                                if (GetMoviconVarTypeAndSize(fieldType, fieldTypeClass, out fieldSize) > -1)
                                {
                                    if (fieldSize > 0)
                                    {
                                        structSize += fieldSize;
                                    }
                                }
                            }
                        }
                        if(structSize > 0)
                        {
                            varSize = structSize;
                        }
                    }
                    break;
            }
            return (moviconType);
        }

        int GetMoviconEnumVarTypeAndSize(string daType, out uint varSize)
        {
            varSize = 0;
            int moviconType = -1; // Invalid or unsupported
            if(enumMoviconTypes.ContainsKey(daType))
            {
                moviconType = enumMoviconTypes[daType];
                switch(moviconType)
                {
                    case (int)DataType.Byte:
                    case (int)DataType.SByte:
                        varSize = 1;
                        break;

                    case (int)DataType.Int16:
                    case (int)DataType.UInt16:
                        varSize = 2;
                        break;

                    case (int)DataType.Int32:
                    case (int)DataType.UInt32:
                        varSize = 4;
                        break;
                }
            }
            return (moviconType);
        }

        int GetMoviconStandardVarTypeAndSize(string daType, out uint varSize)
        {
            varSize = 0;
            int moviconType = -1; // Invalid or unsupported

            if (string.Compare(daType, "BOOLEAN", true) == 0)
            {
                moviconType = (int)DataType.Boolean;
                varSize = 1;
            }
            else if (string.Compare(daType, "INT8", true) == 0)
            {
                moviconType = (int)DataType.SByte;
                varSize = 1;
            }
            else if (string.Compare(daType, "INT16", true) == 0)
            {
                moviconType = (int)DataType.Int16;
                varSize = 2;
            }
            else if (string.Compare(daType, "INT24", true) == 0)
            {
                moviconType = (int)DataType.Int32;
                varSize = 4;
            }
            else if (string.Compare(daType, "INT32", true) == 0)
            {
                moviconType = (int)DataType.Int32;
                varSize = 4;
            }
            else if (string.Compare(daType, "INT8U", true) == 0)
            {
                moviconType = (int)DataType.Byte;
                varSize = 1;
            }
            else if (string.Compare(daType, "INT16U", true) == 0)
            {
                moviconType = (int)DataType.UInt16;
                varSize = 2;
            }
            else if (string.Compare(daType, "INT24U", true) == 0)
            {
                moviconType = (int)DataType.UInt32;
                varSize = 4;
            }
            else if (string.Compare(daType, "INT32U", true) == 0)
            {
                moviconType = (int)DataType.UInt32;
                varSize = 4;
            }
            else if (string.Compare(daType, "FLOAT32", true) == 0)
            {
                moviconType = (int)DataType.Float;
                varSize = 4;
            }
            else if (string.Compare(daType, "FLOAT64", true) == 0)
            {
                moviconType = (int)DataType.Double;
                varSize = 8;
            }
            else if (string.Compare(daType, "Quality", true) == 0)
            {
                moviconType = (int)DataType.UInt16;
                varSize = 2;
            }
            else if (string.Compare(daType, "Timestamp", true) == 0)
            {
                moviconType = (int)DataType.String;
                varSize = 50;
            }
            else if (string.Compare(daType, "VisString32", true) == 0)
            {
                moviconType = (int)DataType.String;
                varSize = 32;
            }
            else if (string.Compare(daType, "VisString64", true) == 0)
            {
                moviconType = (int)DataType.String;
                varSize = 64;
            }
            else if (string.Compare(daType, "VisString129", true) == 0)
            {
                moviconType = (int)DataType.String;
                varSize = 129;
            }
            else if (string.Compare(daType, "VisString255", true) == 0)
            {
                moviconType = (int)DataType.String;
                varSize = 255;
            }
            else if (string.Compare(daType, "Octet64", true) == 0)
            {
                moviconType = (int)DataType.String;
                varSize = 64;
            }
            else if (string.Compare(daType, "EntryTime", true) == 0)
            {
                moviconType = (int)DataType.String;
                varSize = 1;
            }
            else if (string.Compare(daType, "Check", true) == 0)
            {
                moviconType = (int)DataType.Byte;
                varSize = 1;
            }
            else if (string.Compare(daType, "Dbpos", true) == 0)
            {
                moviconType = (int)DataType.Byte;
                varSize = 1;
            }

            return (moviconType);
        }

        ImportTypeClass GetImportTypeClass(string typeName)
        {
            ImportTypeClass typeClass = ImportTypeClass.TypeClass_Unrecognized;

            if (daTypesMap.ContainsKey(typeName))
            {
                typeClass = ImportTypeClass.TypeClass_DA;
            }
            else if (doTypesMap.ContainsKey(typeName))
            {
                typeClass = ImportTypeClass.TypeClass_DO;
            }
            else if (enumTypesMap.ContainsKey(typeName))
            {
                typeClass = ImportTypeClass.TypeClass_Enum;
            }
            else if (string.Compare(typeName, "BOOLEAN", true) == 0)
            {
                typeClass = ImportTypeClass.TypeClass_Standard;
            }
            else if (string.Compare(typeName, "INT8", true) == 0)
            {
                typeClass = ImportTypeClass.TypeClass_Standard;
            }
            else if (string.Compare(typeName, "INT16", true) == 0)
            {
                typeClass = ImportTypeClass.TypeClass_Standard;
            }
            else if (string.Compare(typeName, "INT24", true) == 0)
            {
                typeClass = ImportTypeClass.TypeClass_Standard;
            }
            else if (string.Compare(typeName, "INT32", true) == 0)
            {
                typeClass = ImportTypeClass.TypeClass_Standard;
            }
            else if (string.Compare(typeName, "INT8U", true) == 0)
            {
                typeClass = ImportTypeClass.TypeClass_Standard;
            }
            else if (string.Compare(typeName, "INT16U", true) == 0)
            {
                typeClass = ImportTypeClass.TypeClass_Standard;
            }
            else if (string.Compare(typeName, "INT24U", true) == 0)
            {
                typeClass = ImportTypeClass.TypeClass_Standard;
            }
            else if (string.Compare(typeName, "INT32U", true) == 0)
            {
                typeClass = ImportTypeClass.TypeClass_Standard;
            }
            else if (string.Compare(typeName, "FLOAT32", true) == 0)
            {
                typeClass = ImportTypeClass.TypeClass_Standard;
            }
            else if (string.Compare(typeName, "FLOAT64", true) == 0)
            {
                typeClass = ImportTypeClass.TypeClass_Standard;
            }
            else if (string.Compare(typeName, "Quality", true) == 0)
            {
                typeClass = ImportTypeClass.TypeClass_Standard;
            }
            else if (string.Compare(typeName, "Timestamp", true) == 0)
            {
                typeClass = ImportTypeClass.TypeClass_Standard;
            }
            else if (string.Compare(typeName, "VisString32", true) == 0)
            {
                typeClass = ImportTypeClass.TypeClass_Standard;
            }
            else if (string.Compare(typeName, "VisString64", true) == 0)
            {
                typeClass = ImportTypeClass.TypeClass_Standard;
            }
            else if (string.Compare(typeName, "VisString129", true) == 0)
            {
                typeClass = ImportTypeClass.TypeClass_Standard;
            }
            else if (string.Compare(typeName, "VisString255", true) == 0)
            {
                typeClass = ImportTypeClass.TypeClass_Standard;
            }
            else if (string.Compare(typeName, "Octet64", true) == 0)
            {
                typeClass = ImportTypeClass.TypeClass_Standard;
            }
            else if (string.Compare(typeName, "EntryTime", true) == 0)
            {
                typeClass = ImportTypeClass.TypeClass_Standard;
            }
            else if (string.Compare(typeName, "Check", true) == 0)
            {
                typeClass = ImportTypeClass.TypeClass_Standard;
            }
            else if (string.Compare(typeName, "EntryTime", true) == 0)
            {
                typeClass = ImportTypeClass.TypeClass_Standard;
            }
            else if (string.Compare(typeName, "Dbpos", true) == 0)
            {
                typeClass = ImportTypeClass.TypeClass_Standard;
            }

            return (typeClass);
        }

        void AddDatasetTags(string datasetName, List<string> datasetElements)
        {
            if (String.IsNullOrWhiteSpace(datasetName) || (datasetElements == null) || (datasetElements.Count < 1))
            {
                return;
            }

            foreach (string element in datasetElements)
            {
                if (string.IsNullOrWhiteSpace(element))
                {
                    continue;
                }

                // Get the components of the MMS address
                string nodeName = String.Empty;
                string functionalConstraint = String.Empty;
                string doName = String.Empty;
                string daName = String.Empty;
                if (!SplitMMSAddress(element, out nodeName, out functionalConstraint, out doName, out daName))
                {
                    continue;
                }

                // Get the properties of the node
                if (!logicalNodesMap.ContainsKey(nodeName))
                {
                    continue;
                }
                List<string> nodeInfo = logicalNodesMap[nodeName];
                if ((nodeInfo == null) || (nodeInfo.Count < 1))
                {
                    continue;
                }

                // Retrieve the name of the node type
                int index = nodeInfo[0].IndexOf("Type ");
                if ((index < 0) || (index >= (nodeInfo[0].Length - 6)))
                {
                    continue;
                }
                string nodeType = nodeInfo[0].Substring(index + 5);
                if (String.IsNullOrWhiteSpace(nodeType))
                {
                    continue;
                }

                // Retrieve the info of the Node type
                if (!lnodeStructuresMap.ContainsKey(nodeType))
                {
                    continue;
                }
                Dictionary<string, string> nodeInfoMap = lnodeStructuresMap[nodeType];
                if ((nodeInfoMap == null) || (nodeInfoMap.Count < 1))
                {
                    continue;
                }

                // Get the type of the Data Object (DO)
                if (!nodeInfoMap.ContainsKey(doName))
                {
                    continue;
                }
                string doType = nodeInfoMap[doName];
                if (String.IsNullOrWhiteSpace(doType))
                {
                    continue;
                }

                // Retrieve the info of the DO type
                if(!doStructuresMap.ContainsKey(doType) || !doTypesMap.ContainsKey(doType))
                {
                    continue;
                }
                Dictionary<string, string> doInfoMap = doStructuresMap[doType];
                List<string> doInfoList = doTypesMap[doType];
                if((doInfoMap == null) || (doInfoMap.Count < 1))
                {
                    continue;
                }
                if ((doInfoList == null) || (doInfoList.Count < 1))
                {
                    continue;
                }

                // Add DA info to the list of members of the data set
                AddTagsOfADataSet(datasetName, element, daName, doInfoMap, doInfoList, false, functionalConstraint);                
            }

            // Add tags to the tree
            if(!importDatasetsMap.ContainsKey(datasetName))
            {
                return;
            }
            List<DatasetVar> tagList = importDatasetsMap[datasetName];
            if((tagList == null) || (tagList.Count < 1))
            {
                return;
            }

            // Calculate and check the total size of the data set
            uint totalSize = 0;
            foreach(DatasetVar tag in tagList)
            {
                totalSize += tag.Size;
            }
            if(totalSize == 0)
            {
                return;
            }

            // Add to the tree a node corresponding to the data set
            int datasetID = globalID++;
            ImportData vds = importDataModel.addImportData();
            vds.Name = datasetName;
            vds.Address = datasetName;
            vds.parentId = -1;
            vds.Id = datasetID;
            vds.szType = datasetName;
            ((ImportDataIEC61850)vds).TypeClass = ImportTypeClass.TypeClass_Dataset;
            ((ImportDataIEC61850)vds).Type = (DataType)typeStruct;
            ((ImportDataIEC61850)vds).TagTypeInt = typeStruct;
            ((ImportDataIEC61850)vds).Size = totalSize;
            vds.Description = "Dataset";
            AddTreeItem(vds);

            // Add to the tree the nodes of the elements of the data set
            foreach (DatasetVar tag in tagList)
            {
                ImportData v = importDataModel.addImportData();
                v.Name = tag.Name;
                v.Address = tag.Address;
                v.parentId = datasetID;
                v.Id = globalID++;
                v.szType = tag.Type;
                ((ImportDataIEC61850)v).TypeClass = tag.TypeClass;
                ((ImportDataIEC61850)v).Type = (DataType)tag.TypeID;
                ((ImportDataIEC61850)v).TagTypeInt = tag.TypeID;
                ((ImportDataIEC61850)v).Size = tag.Size;
                ((ImportDataIEC61850)v).DatasetName = datasetName;
                AddTreeItem(v, vds);
            }
        }

        // Add DA info to the list of members of the data set
        void AddTagsOfADataSet(string datasetName, string tagName, string daName, Dictionary<string, string> doInfoMap, List<string> doInfoList, bool isDAInfo, string functionalConstraint)
        {
            if(!String.IsNullOrWhiteSpace(daName))
            {
                int index = daName.IndexOf('$');
                // Normal case
                if(index < 0)
                {
                    if(!doInfoMap.ContainsKey(daName))
                    {
                        return;
                    }
                    string daInfo = doInfoMap[daName];
                    if(String.IsNullOrWhiteSpace(daInfo))
                    {
                        return;
                    }

                    // Get the DA type
                    if(!isDAInfo)
                    {
                        index = daInfo.IndexOf(" | ");
                        if((index <= 0) || (index >= (daInfo.Length - 3)))
                        {
                            return;
                        }
                    }
                    else
                    {
                        index = daInfo.Length;
                    }
                    string daTypeInfo = daInfo.Substring(0, index);

                    string dataAttributeName = String.Empty;
                    AddSingleTagOfADataSet(datasetName, tagName, dataAttributeName, daTypeInfo, functionalConstraint);
                }

                // Special case: complex DA
                else
                {
                    if ((index == 0) || (index == (daName.Length - 1)))
                    {
                        return;
                    }
                    int index2 = 0;
                    string daNameFirstPart = daName.Substring(0, index);
                    if(String.IsNullOrWhiteSpace(daNameFirstPart) || !doInfoMap.ContainsKey(daNameFirstPart))
                    {
                        return;
                    }
                    string daInfo = doInfoMap[daNameFirstPart];

                    // Get the DA type
                    if (!isDAInfo)
                    {
                        index2 = daInfo.IndexOf(" | ");
                        if ((index2 <= 0) || (index2 >= (daInfo.Length - 3)))
                        {
                            return;
                        }
                    }
                    else
                    {
                        index2 = daInfo.Length;
                    }
                    string daTypeInfo = daInfo.Substring(0, index2);

                    // Retrieve the info of the DA type
                    if(!daStructuresMap.ContainsKey(daTypeInfo) || !daTypesMap.ContainsKey(daTypeInfo))
                    {
                        return;
                    }
                    Dictionary<string, string> daInfoMap = daStructuresMap[daTypeInfo];
                    if((daInfoMap == null) || (daInfoMap.Count < 1))
                    {
                        return;
                    }
                    List<string> daInfoList = daTypesMap[daTypeInfo];
                    if ((daInfoList == null) || (daInfoList.Count < 1))
                    {
                        return;
                    }

                    string daNameSecondPart = daName.Substring(index + 1);
                    if (String.IsNullOrWhiteSpace(daNameSecondPart))
                    {
                        return;
                    }

                    // Recursive call
                    AddTagsOfADataSet(datasetName, tagName, daNameSecondPart, daInfoMap, daInfoList, true, functionalConstraint);
                }
            }
            else
            {
                foreach(string daInfo in doInfoList)
                {
                    // Get the DA name
                    int index = daInfo.IndexOf(" | ");
                    if ((index <= 0) || (index >= (daInfo.Length - 3)))
                    {
                        continue;
                    }
                    string daNameInfo = daInfo.Substring(0, index);

                    // Get the DA type
                    int index2 = daInfo.IndexOf(" | ", index + 3);
                    if ((index2 <= 0) || (index2 >= (daInfo.Length - 3)) || (index2 == (index + 3)))
                    {
                        continue;
                    }
                    string daTypeInfo = daInfo.Substring(index + 3, index2 - index - 3);

                    // Get the DA Functional Constraint and check it
                    string daFC = daInfo.Substring(index2 + 3);
                    if(String.IsNullOrWhiteSpace(daFC))
                    {
                        continue;
                    }
                    if((string.Compare(daFC, functionalConstraint, true) != 0) && (string.Compare(daFC, "SDO", true) != 0))
                    {
                        continue;
                    }

                    AddSingleTagOfADataSet(datasetName, tagName, daNameInfo, daTypeInfo, functionalConstraint);
                }
            }
        }

        void AddSingleTagOfADataSet(string datasetName, string tagName, string daName, string daType, string functionalConstraint)
        {
            if (String.IsNullOrWhiteSpace(datasetName) || String.IsNullOrWhiteSpace(tagName) || String.IsNullOrWhiteSpace(daType))
            {
                return;
            }

            // Build the complete name of the tag
            string tagCompleteName = String.Empty;
            if (!String.IsNullOrWhiteSpace(daName))
            {
                tagCompleteName = string.Format("{0}${1}", tagName, daName);
            }
            else
            {
                tagCompleteName = tagName;
            }

            ImportTypeClass typeClass = GetImportTypeClass(daType);
            if (typeClass == ImportTypeClass.TypeClass_Unrecognized)
            {
                return;
            }

            // Enumeration or standard type
            if ((typeClass == ImportTypeClass.TypeClass_Enum) || (typeClass == ImportTypeClass.TypeClass_Standard))
            {
                uint varSize = 0;
                int moviconType = GetMoviconVarTypeAndSize(daType, typeClass, out varSize);
                if ((moviconType == -1) || (varSize == 0))
                {
                    return;
                }
                List<DatasetVar> dVarList;
                if (!importDatasetsMap.ContainsKey(datasetName))
                {
                    dVarList = new List<DatasetVar>();
                    importDatasetsMap[datasetName] = dVarList;
                }
                else
                {
                    dVarList = importDatasetsMap[datasetName];
                }
                DatasetVar newTag = new DatasetVar();
                newTag.Name = tagCompleteName;
                newTag.Address = tagCompleteName;
                newTag.Type = daType;
                newTag.Size = varSize;
                newTag.TypeID = moviconType;
                newTag.TypeClass = typeClass;
                dVarList.Add(newTag);
            }

            // DO
            else if (typeClass == ImportTypeClass.TypeClass_DO)
            {
                if (!doTypesMap.ContainsKey(daType))
                {
                    return;
                }
                List<string> daInfoList = doTypesMap[daType];
                if ((daInfoList == null) || (daInfoList.Count < 1))
                {
                    return;
                }

                foreach (string daInfo in daInfoList)
                {
                    // Get the DA name
                    int index = daInfo.IndexOf(" | ");
                    if ((index <= 0) || (index >= (daInfo.Length - 3)))
                    {
                        continue;
                    }
                    string daNameInfo = daInfo.Substring(0, index);
                    if (String.IsNullOrWhiteSpace(daNameInfo))
                    {
                        continue;
                    }

                    // Get the DA type
                    string daTypeInfo = daInfo.Substring(index + 3);

                    // Get the DA functional constraint (optional) and check it
                    string daFC = String.Empty;
                    index = daTypeInfo.IndexOf(" | ");
                    if ((index > 0) && (index < (daTypeInfo.Length - 3)))
                    {
                        daFC = daTypeInfo.Substring(index + 3);
                        string auxString = daTypeInfo.Substring(0, index);
                        daTypeInfo = auxString;
                    }
                    if (!String.IsNullOrWhiteSpace(daFC) && (string.Compare(daFC, "SDO", true) != 0))
                    {
                        if (string.Compare(daFC, functionalConstraint, true) != 0)
                        {
                            continue;
                        }
                    }

                    // Recursive call
                    AddSingleTagOfADataSet(datasetName, tagCompleteName, daNameInfo, daTypeInfo, functionalConstraint);
                }
            }

            // DA
            else
            {
                if(!daTypesMap.ContainsKey(daType))
                {
                    return;
                }
                List<string> daInfoList = daTypesMap[daType];
                if((daInfoList == null) || (daInfoList.Count < 1))
                {
                    return;
                }

                foreach (string daInfo in daInfoList)
                {
                    // Get the DA name
                    int index = daInfo.IndexOf(" | ");
                    if ((index <= 0) || (index >= (daInfo.Length - 3)))
                    {
                        continue;
                    }
                    string daNameInfo = daInfo.Substring(0, index);
                    if (String.IsNullOrWhiteSpace(daNameInfo))
                    {
                        continue;
                    }

                    // Get the DA type
                    string daTypeInfo = daInfo.Substring(index + 3);
                    if (String.IsNullOrWhiteSpace(daTypeInfo))
                    {
                        continue;
                    }

                    // Recursive call
                    AddSingleTagOfADataSet(datasetName, tagCompleteName, daNameInfo, daTypeInfo, functionalConstraint);
                }
            }
        }

        // Get the components of the MMS address
        bool SplitMMSAddress(string mmsAddress, out string nodeName, out string functionalConstraint, out string doName, out string daName)
        {
            nodeName = String.Empty;
            functionalConstraint = String.Empty;
            doName = String.Empty;
            daName = String.Empty;
            string[] addressParts = mmsAddress.Split(new char[] {'$'});
            if(addressParts.Length < 3)
            {
                return (false);
            }
            nodeName = addressParts[0];
            functionalConstraint = addressParts[1];
            doName = addressParts[2];
            // Optional part of the MMS address
            if(addressParts.Length > 3)
            {
                daName = addressParts[3];
            }

            return (true);
        }

        void AddReportControlTags(string reportControlName, string reportControlDataset)
        {
            // Get the components of the RCB address
            string nodeName = String.Empty;
            string functionalConstraint = String.Empty;
            string doName = String.Empty;
            string daName = String.Empty;

            if (!SplitMMSAddress(reportControlName, out nodeName, out functionalConstraint, out doName, out daName))
                return;
            // Only the functional constraints "BR" (Buffered Report) and "RP" (Unbuffered Report) are supported for RCB
            //if ((string.Compare(functionalConstraint, "BR") != 0) && (string.Compare(functionalConstraint, "RP") != 0))
            if (functionalConstraint != "BR" && functionalConstraint != "RP")
                return;

            // Retrieve the template of the elements of the dataset
            if(!importDatasetsMap.ContainsKey(reportControlDataset))
                return;

            List<DatasetVar> tagList = importDatasetsMap[reportControlDataset];
            if((tagList == null) || (tagList.Count < 1))
                return;

            // Calculate and check the total size of the data set
            uint totalSize = 0;
            foreach (DatasetVar tag in tagList)
                totalSize += tag.Size;
            if (totalSize == 0)
                return;
                        
            switch (functionalConstraint) {
                case "BR":
                    totalSize += 398;
                    break;
                case "RP":
                    totalSize += 279;
                    break;
            }

            // Add to the tree a node corresponding to the RCB
            int reportID = globalID++;
            ImportData vrcb = importDataModel.addImportData();
            vrcb.Name = reportControlName;
            vrcb.Address = reportControlName;
            vrcb.parentId = -1;
            vrcb.Id = reportID;
            vrcb.szType = reportControlDataset;
            ((ImportDataIEC61850)vrcb).TypeClass = ImportTypeClass.TypeClass_Report;
            ((ImportDataIEC61850)vrcb).Type = (DataType)typeStruct;
            ((ImportDataIEC61850)vrcb).TagTypeInt = typeStruct;
            ((ImportDataIEC61850)vrcb).Size = totalSize;
            vrcb.Description = "Report Control Block";
            AddTreeItem(vrcb);

            // Add to the tree the nodes corresponding to the elements of the RCB
            switch (functionalConstraint)
            {
                case "BR":
                    AddTagsOfABufferedReportControl(reportControlName, reportID, vrcb);
                    break;
                case "RP":
                    AddTagsOfAnUnbufferedReportControl(reportControlName, reportID, vrcb);
                    break;
            }

            // Add to the tree the nodes of the elements of the data set
            foreach (DatasetVar tag in tagList)
            {
                ImportData v = importDataModel.addImportData();
                v.Name = tag.Name;
                v.Address = tag.Address;
                v.parentId = reportID;
                v.Id = globalID++;
                v.szType = tag.Type;
                ((ImportDataIEC61850)v).TypeClass = tag.TypeClass;
                ((ImportDataIEC61850)v).Type = (DataType)tag.TypeID;
                ((ImportDataIEC61850)v).TagTypeInt = tag.TypeID;
                ((ImportDataIEC61850)v).Size = tag.Size;
                ((ImportDataIEC61850)v).ReportName = reportControlName;
                AddTreeItem(v, vrcb);
            }
        }

        void AddTagsOfABufferedReportControl(string rcbName, int reportID, ImportData report)
        {
            // Fixed fields of the RCB
            for (int i = 0; i < 15; i++)
            {
                ImportData v = importDataModel.addImportData();
                switch(i)
                {
                    // RptID
                    case 0:
                        {
                            v.Name = "RptID";
                            v.Address = string.Format("{0}$RptID", rcbName);
                            v.parentId = reportID;
                            v.Id = globalID++;
                            v.szType = "VisString64";
                            ((ImportDataIEC61850)v).TypeClass = ImportTypeClass.TypeClass_Standard;
                            ((ImportDataIEC61850)v).Type = DataType.String;
                            ((ImportDataIEC61850)v).TagTypeInt = (int)DataType.String;
                            ((ImportDataIEC61850)v).Size = 64;
                            AddTreeItem(v, report);
                        }
                        break;

                    // RptEna
                    case 1:
                        {
                            v.Name = "RptEna";
                            v.Address = string.Format("{0}$RptEna", rcbName);
                            v.parentId = reportID;
                            v.Id = globalID++;
                            v.szType = "BOOLEAN";
                            ((ImportDataIEC61850)v).TypeClass = ImportTypeClass.TypeClass_Standard;
                            ((ImportDataIEC61850)v).Type = DataType.Boolean;
                            ((ImportDataIEC61850)v).TagTypeInt = (int)DataType.Boolean;
                            ((ImportDataIEC61850)v).Size = 1;
                            AddTreeItem(v, report);
                        }
                        break;

                    // DatSet
                    case 2:
                        {
                            v.Name = "DatSet";
                            v.Address = string.Format("{0}$DatSet", rcbName);
                            v.parentId = reportID;
                            v.Id = globalID++;
                            v.szType = "VisString129";
                            ((ImportDataIEC61850)v).TypeClass = ImportTypeClass.TypeClass_Standard;
                            ((ImportDataIEC61850)v).Type = DataType.String;
                            ((ImportDataIEC61850)v).TagTypeInt = (int)DataType.String;
                            ((ImportDataIEC61850)v).Size = 129;
                            AddTreeItem(v, report);
                        }
                        break;

                    // ConfRev
                    case 3:
                        {
                            v.Name = "ConfRev";
                            v.Address = string.Format("{0}$ConfRev", rcbName);
                            v.parentId = reportID;
                            v.Id = globalID++;
                            v.szType = "INT32U";
                            ((ImportDataIEC61850)v).TypeClass = ImportTypeClass.TypeClass_Standard;
                            ((ImportDataIEC61850)v).Type = DataType.UInt32;
                            ((ImportDataIEC61850)v).TagTypeInt = (int)DataType.UInt32;
                            ((ImportDataIEC61850)v).Size = 4;
                            AddTreeItem(v, report);
                        }
                        break;

                    // OptFlds
                    case 4:
                        {
                            v.Name = "OptFlds";
                            v.Address = string.Format("{0}$OptFlds", rcbName);
                            v.parentId = reportID;
                            v.Id = globalID++;
                            v.szType = "OptFlds";
                            ((ImportDataIEC61850)v).TypeClass = ImportTypeClass.TypeClass_Standard;
                            ((ImportDataIEC61850)v).Type = DataType.UInt16;
                            ((ImportDataIEC61850)v).TagTypeInt = (int)DataType.UInt16;
                            ((ImportDataIEC61850)v).Size = 2;
                            AddTreeItem(v, report);
                        }
                        break;

                    // BufTm
                    case 5:
                        {
                            v.Name = "BufTm";
                            v.Address = string.Format("{0}$BufTm", rcbName);
                            v.parentId = reportID;
                            v.Id = globalID++;
                            v.szType = "INT32U";
                            ((ImportDataIEC61850)v).TypeClass = ImportTypeClass.TypeClass_Standard;
                            ((ImportDataIEC61850)v).Type = DataType.UInt32;
                            ((ImportDataIEC61850)v).TagTypeInt = (int)DataType.UInt32;
                            ((ImportDataIEC61850)v).Size = 4;
                            AddTreeItem(v, report);
                        }
                        break;

                    // SqNum
                    case 6:
                        {
                            v.Name = "SqNum";
                            v.Address = string.Format("{0}$SqNum", rcbName);
                            v.parentId = reportID;
                            v.Id = globalID++;
                            v.szType = "INT32U";
                            ((ImportDataIEC61850)v).TypeClass = ImportTypeClass.TypeClass_Standard;
                            ((ImportDataIEC61850)v).Type = DataType.UInt32;
                            ((ImportDataIEC61850)v).TagTypeInt = (int)DataType.UInt32;
                            ((ImportDataIEC61850)v).Size = 4;
                            AddTreeItem(v, report);
                        }
                        break;

                    // TrgOps
                    case 7:
                        {
                            v.Name = "TrgOps";
                            v.Address = string.Format("{0}$TrgOps", rcbName);
                            v.parentId = reportID;
                            v.Id = globalID++;
                            v.szType = "TrgOps";
                            ((ImportDataIEC61850)v).TypeClass = ImportTypeClass.TypeClass_Standard;
                            ((ImportDataIEC61850)v).Type = DataType.Byte;
                            ((ImportDataIEC61850)v).TagTypeInt = (int)DataType.Byte;
                            ((ImportDataIEC61850)v).Size = 1;
                            AddTreeItem(v, report);
                        }
                        break;

                    // IntgPd
                    case 8:
                        {
                            v.Name = "IntgPd";
                            v.Address = string.Format("{0}$IntgPd", rcbName);
                            v.parentId = reportID;
                            v.Id = globalID++;
                            v.szType = "INT32U";
                            ((ImportDataIEC61850)v).TypeClass = ImportTypeClass.TypeClass_Standard;
                            ((ImportDataIEC61850)v).Type = DataType.UInt32;
                            ((ImportDataIEC61850)v).TagTypeInt = (int)DataType.UInt32;
                            ((ImportDataIEC61850)v).Size = 4;
                            AddTreeItem(v, report);
                        }
                        break;

                    // GI
                    case 9:
                        {
                            v.Name = "GI";
                            v.Address = string.Format("{0}$GI", rcbName);
                            v.parentId = reportID;
                            v.Id = globalID++;
                            v.szType = "BOOLEAN";
                            ((ImportDataIEC61850)v).TypeClass = ImportTypeClass.TypeClass_Standard;
                            ((ImportDataIEC61850)v).Type = DataType.Boolean;
                            ((ImportDataIEC61850)v).TagTypeInt = (int)DataType.Boolean;
                            ((ImportDataIEC61850)v).Size = 1;
                            AddTreeItem(v, report);
                        }
                        break;

                    // PurgeBuf
                    case 10:
                        {
                            v.Name = "PurgeBuf";
                            v.Address = string.Format("{0}$PurgeBuf", rcbName);
                            v.parentId = reportID;
                            v.Id = globalID++;
                            v.szType = "BOOLEAN";
                            ((ImportDataIEC61850)v).TypeClass = ImportTypeClass.TypeClass_Standard;
                            ((ImportDataIEC61850)v).Type = DataType.Boolean;
                            ((ImportDataIEC61850)v).TagTypeInt = (int)DataType.Boolean;
                            ((ImportDataIEC61850)v).Size = 1;
                            AddTreeItem(v, report);
                        }
                        break;

                    // EntryID
                    case 11:
                        {
                            v.Name = "EntryID";
                            v.Address = string.Format("{0}$EntryID", rcbName);
                            v.parentId = reportID;
                            v.Id = globalID++;
                            v.szType = "Octet64";
                            ((ImportDataIEC61850)v).TypeClass = ImportTypeClass.TypeClass_Standard;
                            ((ImportDataIEC61850)v).Type = DataType.String;
                            ((ImportDataIEC61850)v).TagTypeInt = (int)DataType.String;
                            ((ImportDataIEC61850)v).Size = 64;
                            AddTreeItem(v, report);
                        }
                        break;

                    // TimeofEntry
                    case 12:
                        {
                            v.Name = "TimeofEntry";
                            v.Address = string.Format("{0}$TimeofEntry", rcbName);
                            v.parentId = reportID;
                            v.Id = globalID++;
                            v.szType = "EntryTime";
                            ((ImportDataIEC61850)v).TypeClass = ImportTypeClass.TypeClass_Standard;
                            ((ImportDataIEC61850)v).Type = DataType.String;
                            ((ImportDataIEC61850)v).TagTypeInt = (int)DataType.String;
                            ((ImportDataIEC61850)v).Size = 50;
                            AddTreeItem(v, report);
                        }
                        break;

                    // ResvTms
                    case 13:
                        {
                            v.Name = "ResvTms";
                            v.Address = string.Format("{0}$ResvTms", rcbName);
                            v.parentId = reportID;
                            v.Id = globalID++;
                            v.szType = "INT32";
                            ((ImportDataIEC61850)v).TypeClass = ImportTypeClass.TypeClass_Standard;
                            ((ImportDataIEC61850)v).Type = DataType.Int32;
                            ((ImportDataIEC61850)v).TagTypeInt = (int)DataType.Int32;
                            ((ImportDataIEC61850)v).Size = 4;
                            AddTreeItem(v, report);
                        }
                        break;

                    // Owner
                    case 14:
                        {
                            v.Name = "Owner";
                            v.Address = string.Format("{0}$Owner", rcbName);
                            v.parentId = reportID;
                            v.Id = globalID++;
                            v.szType = "Octet64";
                            ((ImportDataIEC61850)v).TypeClass = ImportTypeClass.TypeClass_Standard;
                            ((ImportDataIEC61850)v).Type = DataType.String;
                            ((ImportDataIEC61850)v).TagTypeInt = (int)DataType.String;
                            ((ImportDataIEC61850)v).Size = 64;
                            AddTreeItem(v, report);
                        }
                        break;
                }
            }
       }

        void AddTagsOfAnUnbufferedReportControl(string rcbName, int reportID, ImportData report)
        {
            // Fixed fields of the RCB
            for (int i = 0; i < 12; i++)
            {
                ImportData v = importDataModel.addImportData();
                switch (i)
                {
                    // RptID
                    case 0:
                        {
                            v.Name = "RptID";
                            v.Address = string.Format("{0}$RptID", rcbName);
                            v.parentId = reportID;
                            v.Id = globalID++;
                            v.szType = "VisString64";
                            ((ImportDataIEC61850)v).TypeClass = ImportTypeClass.TypeClass_Standard;
                            ((ImportDataIEC61850)v).Type = DataType.String;
                            ((ImportDataIEC61850)v).TagTypeInt = (int)DataType.String;
                            ((ImportDataIEC61850)v).Size = 64;
                            AddTreeItem(v, report);
                        }
                        break;

                    // RptEna
                    case 1:
                        {
                            v.Name = "RptEna";
                            v.Address = string.Format("{0}$RptEna", rcbName);
                            v.parentId = reportID;
                            v.Id = globalID++;
                            v.szType = "BOOLEAN";
                            ((ImportDataIEC61850)v).TypeClass = ImportTypeClass.TypeClass_Standard;
                            ((ImportDataIEC61850)v).Type = DataType.Boolean;
                            ((ImportDataIEC61850)v).TagTypeInt = (int)DataType.Boolean;
                            ((ImportDataIEC61850)v).Size = 1;
                            AddTreeItem(v, report);
                        }
                        break;

                    // Resv
                    case 2:
                        {
                            v.Name = "Resv";
                            v.Address = string.Format("{0}$Resv", rcbName);
                            v.parentId = reportID;
                            v.Id = globalID++;
                            v.szType = "BOOLEAN";
                            ((ImportDataIEC61850)v).TypeClass = ImportTypeClass.TypeClass_Standard;
                            ((ImportDataIEC61850)v).Type = DataType.Boolean;
                            ((ImportDataIEC61850)v).TagTypeInt = (int)DataType.Boolean;
                            ((ImportDataIEC61850)v).Size = 1;
                            AddTreeItem(v, report);
                        }
                        break;

                    // DatSet
                    case 3:
                        {
                            v.Name = "DatSet";
                            v.Address = string.Format("{0}$DatSet", rcbName);
                            v.parentId = reportID;
                            v.Id = globalID++;
                            v.szType = "VisString129";
                            ((ImportDataIEC61850)v).TypeClass = ImportTypeClass.TypeClass_Standard;
                            ((ImportDataIEC61850)v).Type = DataType.String;
                            ((ImportDataIEC61850)v).TagTypeInt = (int)DataType.String;
                            ((ImportDataIEC61850)v).Size = 129;
                            AddTreeItem(v, report);
                        }
                        break;

                    // ConfRev
                    case 4:
                        {
                            v.Name = "ConfRev";
                            v.Address = string.Format("{0}$ConfRev", rcbName);
                            v.parentId = reportID;
                            v.Id = globalID++;
                            v.szType = "INT32U";
                            ((ImportDataIEC61850)v).TypeClass = ImportTypeClass.TypeClass_Standard;
                            ((ImportDataIEC61850)v).Type = DataType.UInt32;
                            ((ImportDataIEC61850)v).TagTypeInt = (int)DataType.UInt32;
                            ((ImportDataIEC61850)v).Size = 4;
                            AddTreeItem(v, report);
                        }
                        break;

                    // OptFlds
                    case 5:
                        {
                            v.Name = "OptFlds";
                            v.Address = string.Format("{0}$OptFlds", rcbName);
                            v.parentId = reportID;
                            v.Id = globalID++;
                            v.szType = "OptFlds";
                            ((ImportDataIEC61850)v).TypeClass = ImportTypeClass.TypeClass_Standard;
                            ((ImportDataIEC61850)v).Type = DataType.UInt16;
                            ((ImportDataIEC61850)v).TagTypeInt = (int)DataType.UInt16;
                            ((ImportDataIEC61850)v).Size = 2;
                            AddTreeItem(v, report);
                        }
                        break;

                    // BufTm
                    case 6:
                        {
                            v.Name = "BufTm";
                            v.Address = string.Format("{0}$BufTm", rcbName);
                            v.parentId = reportID;
                            v.Id = globalID++;
                            v.szType = "INT32U";
                            ((ImportDataIEC61850)v).TypeClass = ImportTypeClass.TypeClass_Standard;
                            ((ImportDataIEC61850)v).Type = DataType.UInt32;
                            ((ImportDataIEC61850)v).TagTypeInt = (int)DataType.UInt32;
                            ((ImportDataIEC61850)v).Size = 4;
                            AddTreeItem(v, report);
                        }
                        break;

                    // SqNum
                    case 7:
                        {
                            v.Name = "SqNum";
                            v.Address = string.Format("{0}$SqNum", rcbName);
                            v.parentId = reportID;
                            v.Id = globalID++;
                            v.szType = "INT32U";
                            ((ImportDataIEC61850)v).TypeClass = ImportTypeClass.TypeClass_Standard;
                            ((ImportDataIEC61850)v).Type = DataType.UInt32;
                            ((ImportDataIEC61850)v).TagTypeInt = (int)DataType.UInt32;
                            ((ImportDataIEC61850)v).Size = 4;
                            AddTreeItem(v, report);
                        }
                        break;

                    // TrgOps
                    case 8:
                        {
                            v.Name = "TrgOps";
                            v.Address = string.Format("{0}$TrgOps", rcbName);
                            v.parentId = reportID;
                            v.Id = globalID++;
                            v.szType = "TrgOps";
                            ((ImportDataIEC61850)v).TypeClass = ImportTypeClass.TypeClass_Standard;
                            ((ImportDataIEC61850)v).Type = DataType.Byte;
                            ((ImportDataIEC61850)v).TagTypeInt = (int)DataType.Byte;
                            ((ImportDataIEC61850)v).Size = 1;
                            AddTreeItem(v, report);
                        }
                        break;

                    // IntgPd
                    case 9:
                        {
                            v.Name = "IntgPd";
                            v.Address = string.Format("{0}$IntgPd", rcbName);
                            v.parentId = reportID;
                            v.Id = globalID++;
                            v.szType = "INT32U";
                            ((ImportDataIEC61850)v).TypeClass = ImportTypeClass.TypeClass_Standard;
                            ((ImportDataIEC61850)v).Type = DataType.UInt32;
                            ((ImportDataIEC61850)v).TagTypeInt = (int)DataType.UInt32;
                            ((ImportDataIEC61850)v).Size = 4;
                            AddTreeItem(v, report);
                        }
                        break;

                    // GI
                    case 10:
                        {
                            v.Name = "GI";
                            v.Address = string.Format("{0}$GI", rcbName);
                            v.parentId = reportID;
                            v.Id = globalID++;
                            v.szType = "BOOLEAN";
                            ((ImportDataIEC61850)v).TypeClass = ImportTypeClass.TypeClass_Standard;
                            ((ImportDataIEC61850)v).Type = DataType.Boolean;
                            ((ImportDataIEC61850)v).TagTypeInt = (int)DataType.Boolean;
                            ((ImportDataIEC61850)v).Size = 1;
                            AddTreeItem(v, report);
                        }
                        break;

                    // Owner
                    case 11:
                        {
                            v.Name = "Owner";
                            v.Address = string.Format("{0}$Owner", rcbName);
                            v.parentId = reportID;
                            v.Id = globalID++;
                            v.szType = "Octet64";
                            ((ImportDataIEC61850)v).TypeClass = ImportTypeClass.TypeClass_Standard;
                            ((ImportDataIEC61850)v).Type = DataType.String;
                            ((ImportDataIEC61850)v).TagTypeInt = (int)DataType.String;
                            ((ImportDataIEC61850)v).Size = 64;
                            AddTreeItem(v, report);
                        }
                        break;
                }
            }
        }

        internal void AddTreeItem(ImportData tag, ImportData parent = null)
        {
            tag.Parent = parent;
            if (parent == null)
            {
                tag.TreeLevel = 0xFF;
                string strLevel = tag.TreeLevel.ToString("X2");
                tag.parentId = -1;
                importDataModel.Children.Add(tag);
            }
            else
            {
                tag.TreeLevel = parent.TreeLevel - 1;
                string strLevel = tag.TreeLevel.ToString("X2");
                tag.parentId = parent.Id;
                parent.Children.Add(tag);
            }
        }

        #region IDisposable Members

        public void Dispose()
        {
            using (new WaitCursor())
            {
                if (importDataModel != null)
                {
                    importDataModel.Dispose();
                    importDataModel = null;
                }

                if (baseImportTree != null)
                {
                    baseImportTree.LoadImportFile -= LoadFile;

                    baseImportTree.Dispose();
                    baseImportTree = null;
                }
            }
        }
        #endregion
    }

    public class ImportDataIEC61850 : ImportData, IDisposable
    {        
        public ImportDataIEC61850(ImportDataModel inDataModel):
            base(inDataModel)
        {
            dataModelIEC61850 = inDataModel as ImportDataModelIEC61850;
        }
        private ImportDataModelIEC61850 dataModelIEC61850 { get; set; }

        //private int _Id;
        //public int Id
        //{
        //    get { return _Id; }
        //    set { _Id = value; }
        //}
        //private int _parentId;
        //public int parentId
        //{
        //    get { return _parentId; }
        //    set { _parentId = value; }
        //}
        //private string _Name;
        //public string Name
        //{
        //    get
        //    {
        //        return dataModel.getStationName() != "_" ?
        //               dataModel.getStationName() + _Name : _Name;
        //    }
        //    set { _Name = value; }
        //}
        //private string _Address;
        //public string Address
        //{
        //    get { return _Address; }
        //    set { _Address = value; }
        //}
        //private bool _Select;
        //public bool Select
        //{
        //    get { return _Select; }
        //    set { _Select = value; }
        //}
        //private string _DynAddress;
        //public string DynAddress
        //{
        //    get { return _DynAddress; }
        //    set { _DynAddress = value; }
        //}
        private int _TagTypeInt;
        public int TagTypeInt
        {
            get { return _TagTypeInt; }
            set { _TagTypeInt = value; }
        }

        private DataType _Type;
        public DataType Type
        {
            get { return _Type; }
            set { _Type = value; }
        }

        private uint _Size;
        public uint Size
        {
            get { return _Size; }
            set { _Size = value; }
        }

        //private string _Description;
        //public string Description
        //{
        //    get { return _Description; }
        //    set { _Description = value; }
        //}
        //private string _szType;
        //public string szType
        //{
        //    get { return _szType; }
        //    set { _szType = value; }
        //}
        //private ImportData _Parent;
        //public ImportData Parent
        //{
        //    get { return _Parent; }
        //    set { _Parent = value; }
        //}
        //private uint _TreeLevel;
        //public uint TreeLevel
        //{
        //    get { return _TreeLevel; }
        //    set { _TreeLevel = value; }
        //}

        private ImportTypeClass _TypeClass;
        public ImportTypeClass TypeClass
        {
            get { return _TypeClass; }
            set { _TypeClass = value; }
        }

        private string _DatasetName;
        public string DatasetName
        {
            get { return _DatasetName; }
            set { _DatasetName = value; }
        }

        private string _ReportName;
        public string ReportName
        {
            get { return _ReportName; }
            set { _ReportName = value; }
        }

        //public string TreeName
        //{
        //    get { return getTreeName(); }
        //}

        //private string getTreeName()
        //{
        //    string treeName = Name;
        //    if(Parent != null)
        //    {
        //        treeName = Parent.getTreeName() + "." + treeName;
        //    }
        //    return treeName;
        //}

        //public string ImportTagName
        //{
        //    get
        //    {
        //        return dataModel.getStationName() != "_" ?
        //               dataModel.getStationName() + _Name : _Name;
        //    }
        //}

        public override DataType IconTagType
        {
            get { return _Type; }
        }

        #region IDisposable Members

        public void Dispose()
        {
            if (Children != null && Children.Count > 0)
            {
                foreach (ImportData el in Children)
                {
                    var sl = el as ImportDataIEC61850;
                    if (sl != null)
                    {
                        sl.Dispose();
                    }
                }
                Children.Clear();
            }
        }

        #endregion
    };

    public class ImportDataModelIEC61850 : ImportDataModel, IDisposable
    {
        public ImportDataModelIEC61850(GetStationName inGetStationName):
            base(inGetStationName)
        {            
        }
        public override ImportData addImportData()
        {
            ImportDataIEC61850 importData = new ImportDataIEC61850(this);
            return importData;
        }

        #region IDisposable Members

        public void Dispose()
        {
            foreach (ImportData el in Children)
            {
                ImportDataIEC61850 els7 = el as ImportDataIEC61850;
                if (els7 != null)
                {
                    els7.Dispose();
                }
            }
            Children.Clear();
            Children = null;
        }
        #endregion
    }

    public class DatasetVar
    {
        public DatasetVar()
        {
            _Size = 0;
            _TypeID = (int) DataType.Byte;
            _TypeClass = ImportTypeClass.TypeClass_Standard;
        }

        #region Properties
        private string _Name;
        public string Name
        {
            get { return _Name; }
            set { _Name = value; }
        }

        private string _Address;
        public string Address
        {
            get { return _Address; }
            set { _Address = value; }
        }

        private string _Type;
        public string Type
        {
            get { return _Type; }
            set { _Type = value; }
        }

        private uint _Size;
        public uint Size
        {
            get { return _Size; }
            set { _Size = value; }
        }

        private int _TypeID;
        public int TypeID
        {
            get { return _TypeID; }
            set { _TypeID = value; }
        }

        private ImportTypeClass _TypeClass;
        public ImportTypeClass TypeClass
        {
            get { return _TypeClass; }
            set { _TypeClass = value; }
        }
        #endregion
    }
}
