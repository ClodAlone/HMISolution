using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Reflection;
using UFUAModel;
using Utilities;
using DriverCodeBase.Enumerators;
using DriverCodeBase.UI;
using DriverCodeBase.UI.Controls;

namespace LacbusPC.UI
{
    public enum LacbusPcImportFileTypes : byte
    {
        unsupportedFileType = 0,
        SoftoolsV1_11 = 1,
        SoftoolS98 = 2
    }

    /// <summary>
    /// Interaction logic for ImportTagsEditorTree.xaml
    /// </summary>
    public partial class ImportTagsEditorTree : UserControl, IDisposable
    {
        bool alreadyLoaded = false;
        Dictionary<string, byte> underlyingProtocolList;
        Dictionary<string, UInt16> rtuNumberList;
        Dictionary<string, UInt16> fieldIndexList;
        ImportDataModelLacbus importDataModel;
        byte stationUnderlyingProtocol = (byte)LacbusPcUnderlyingProtocols.LacbusPC;
        UInt16 stationRtuNumber = 0;        
        public GetStationName readStationName;
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
                underlyingProtocolList = new Dictionary<string, byte>();
                rtuNumberList = new Dictionary<string, UInt16>();
                fieldIndexList = new Dictionary<string, UInt16>();
                
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
                    colWidth = 200
                });
                columns.Add(new BaseImportTree.GridColData()
                {
                    colName = "Type",
                    bindingName = "TagType",
                    colWidth = 200
                });
                columns.Add(new BaseImportTree.GridColData()
                {
                    colName = "Category",
                    bindingName = "TagCategory",
                    colWidth = 200
                });

                baseImportTree = new BaseImportTree(DriverName, lista[0],
                    (lista[1].ToLower().IndexOf("true") != -1), columns);
                baseImportTree.SetVisibleButtons();
                baseImportTree.SetFileFilter("txt files|*.txt|csv files|*.csv");
                baseImportTree.LoadImportFile = LoadFile;
                baseImportTree.CreateItemControl = CreateImportDataTreeItemControl;
                MainStack.Children.Add(baseImportTree);

                readStationName = baseImportTree.funcGetStationName();

                // use to retrive some station's data use during import
                FillStationList();

                DataContext = this;
            };
        }

        private ImportDataTreeItemControl CreateImportDataTreeItemControl(object tag)
        {
            return ((ImportDataTreeItemControl)new ImportDataTreeItemControlLacbus(tag));
        }

        private void FillStationList()
        {
            byte underlyingProtocol = (byte)LacbusPcUnderlyingProtocols.LacbusPC;
            UInt16 rtuNumber = 0;

            foreach (var settingsBase in baseImportTree.GetStationSettingsList().Values)
            {
                LacbusPCStationSettings settings = settingsBase as LacbusPCStationSettings;
                underlyingProtocol = (byte)settings.LacbusPCProtocolType;
                rtuNumber = settings.LacbusPCRTUNumber;                
                underlyingProtocolList.Add(settings.Name, underlyingProtocol);
                rtuNumberList.Add(settings.Name, rtuNumber);
            }            
        }

        private static ImportPrototype addOutputPrototype(Dictionary<string, ImportPrototype> protoMap, ImportData elem)
        {
            ImportPrototype proto = null;

            if (protoMap.ContainsKey(((ImportDataLacbus)elem).ElemType))
            {
                proto = protoMap[((ImportDataLacbus)elem).ElemType];
            }
            else
            {
                // New prototype
                proto = new ImportPrototype();
                proto.Name = ((ImportDataLacbus)elem).ElemType;
                proto.Elements = new List<ImportTag>();
                // Value Field
                ImportTag valueField = new ImportTag();
                valueField.ModelType = ModelType.Variable;
                valueField.ArrayDimension = 0;
                valueField.Name = "Value";
                if(proto.Name == "Prototype_LacbusRTU_OutputLogical")
                {
                    valueField.DataType = UFUAModel.DataType.Boolean;
                }
                else
                {
                    valueField.DataType = UFUAModel.DataType.Double;
                }
                proto.Elements.Add(valueField);
                // Lock Field
                ImportTag lockField = new ImportTag();
                lockField.ModelType = ModelType.Variable;
                lockField.ArrayDimension = 0;
                lockField.Name = "Lock";
                lockField.DataType = UFUAModel.DataType.Boolean;
                proto.Elements.Add(lockField);

                protoMap.Add(((ImportDataLacbus)elem).ElemType, proto);
            }

            return proto;
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
                // Build the dictionary of the repeated names
                Dictionary<string, byte> repeatedNamesMap = new Dictionary<string, byte>();
                Dictionary<string, byte> namesMap = new Dictionary<string, byte>();
                foreach (var elemList in list)
                {
                    ImportDataLacbus elem = elemList as ImportDataLacbus;
                    if (elem != null)
                    {
                        if(!namesMap.ContainsKey(elem.Name))
                        {
                            namesMap.Add(elem.Name, 0);
                        }
                        else if(!repeatedNamesMap.ContainsKey(elem.Name))
                        {
                            repeatedNamesMap.Add(elem.Name, 0);
                        }
                    }
                }
                
                List<ImportTag> taglist = new List<ImportTag>();
                Dictionary<string, ImportPrototype> protoMap = new Dictionary<string, ImportPrototype>();
                foreach (var elemList in list)
                {
                    ImportDataLacbus single = elemList as ImportDataLacbus;
                    if (single != null)
                    {                        
                        // Check if the variable is of type Digital Output or Analog Output
                        bool isStructure = false;
                        if (((single.szType == "Output Logical") ||
                            (single.szType == "Output Measure")) &&
                            (single.Category != "Historical"))
                        {
                            isStructure = true;
                        }

                        ImportPrototype proto = null;
                        if (isStructure)
                        {
                            proto = addOutputPrototype(protoMap, single);
                            if (proto == null)
                            {
                                continue;
                            }
                        }

                        string tagUniqueName = single.Name;
                        if(repeatedNamesMap.ContainsKey(tagUniqueName))
                        {
                            tagUniqueName = single.UniqueName;
                        }

                        LacbusPCDynTagSettings sp = new LacbusPCDynTagSettings();
                        if (!sp.TryParse(single.DynAddress))
                            continue;
                        sp.StationName = stationName;
                        single.DynAddress = sp.ToString();

                        ImportTag tagtoimport = new ImportTag()
                        {
                            Name = single.Name,
                            DataType = (UFUAModel.DataType)((ImportDataLacbus)single).TagTypeInt,
                            DynSettings = single.DynAddress,
                            Folder = importfolder,
                            ModelType = (isStructure ? UFUAModel.ModelType.ObjectType : UFUAModel.ModelType.Variable),
                            Description = single.Description,
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
                }
                DataContext = new ImportObject() { PrototypesToImport = protoMap.Values.ToList(), TagsToImport = taglist };
            }
            else
            {
                DataContext = this;
            }
        }

        private LacbusPcImportFileTypes CheckTheImportFileType(string fileHeader)
        {
            LacbusPcImportFileTypes fileType = LacbusPcImportFileTypes.unsupportedFileType;
            if(!String.IsNullOrWhiteSpace(fileHeader))
            {
                int tabIndex = fileHeader.IndexOf('\t');
                string[] row;
                if (tabIndex >= 0)
                {
                    int rtuAddressFieldIndex = fileHeader.IndexOf("60002");
                    if(rtuAddressFieldIndex >= 0)
                    {
                        fileType = LacbusPcImportFileTypes.SoftoolsV1_11;
                    }
                    else
                    {
                        rtuAddressFieldIndex = fileHeader.IndexOf("65");
                        if(rtuAddressFieldIndex >= 0)
                        {
                            fileType = LacbusPcImportFileTypes.SoftoolS98;
                        }
                    }

                    if(fileType != LacbusPcImportFileTypes.unsupportedFileType)
                    {
                        // Now that we have recognized the file type we can fill the dictionary of the fields
                        fieldIndexList.Clear();
                        row = fileHeader.Split('\t');
                        for(int i=0; i<row.GetLength(0); i++)
                        {
                            fieldIndexList.Add(row[i], (ushort)i);
                        }
                    }
                }
            }

            return(fileType);
        }

        int SoftoolsV1_11_GetFieldIndex(string fieldName)
        {
            int fieldIndex = -1;
            UInt16 fieldInd = 0;
            if (fieldIndexList.TryGetValue(fieldName, out fieldInd) == true)
            {
                fieldIndex = (int)fieldInd;
            }

            return (fieldIndex);
        }

        private bool SoftoolS98_GetDatumNumberType(UInt16 infoNumber, UInt16 infoCharacteristic1, UInt16 infoCharacteristic2, ref string datumNumber, ref string datumType)
        {
            bool returnValue = false;
            datumNumber = String.Empty;
            datumType = String.Empty;
            if (infoCharacteristic1 == 0) // Input
            {
                if (infoCharacteristic2 == 0) // Digital
                {
                    datumType = "Digital Input";
                    datumNumber = infoNumber.ToString();
                    returnValue = true;
                }
                else if (infoCharacteristic2 == 1) // Analog
                {
                    datumType = "Analog Input";
                    datumNumber = infoNumber.ToString();
                    returnValue = true;
                }
                else if (infoCharacteristic2 == 3) // Count
                {
                    datumType = "Count Input";
                    datumNumber = infoNumber.ToString();
                    returnValue = true;
                }
            }
            else if (infoCharacteristic1 == 1) // Output
            {
                if (infoCharacteristic2 == 0) // Digital
                {
                    datumType = "Digital Output";
                    datumNumber = infoNumber.ToString();
                    returnValue = true;
                }
                else if (infoCharacteristic2 == 1) // Analog
                {
                    datumType = "Analog Output";
                    datumNumber = infoNumber.ToString();
                    returnValue = true;
                }
            }

            return (returnValue);
        }

        private bool SoftoolsV1_11_GetDatumNumberType(UInt16 infoNumber, UInt16 infoCharacteristic1, UInt16 infoCharacteristic2, UInt16 infoConversionType, ref string datumNumber, ref string datumType)
        {
            bool returnValue = false;
            datumNumber = String.Empty;
            datumType = String.Empty;
            if (infoCharacteristic1 == 0) // Input
            {
                if(infoCharacteristic2 == 0) // Logical
                {
                    datumType = "Input Logical";
                    datumNumber = infoNumber.ToString();
                    returnValue = true;
                }
                else if (infoCharacteristic2 == 1) // Numerical
                {
                    if(infoConversionType == 2)
                    {
                        datumType = "Input Counter";
                        datumNumber = infoNumber.ToString();
                        returnValue = true;
                    }
                    else
                    {
                        datumType = "Input Measure";
                        datumNumber = infoNumber.ToString();
                        returnValue = true;
                    }
                }
            }
            else if(infoCharacteristic1 == 1) // Output
            {
                if (infoCharacteristic2 == 0) // Logical
                {
                    datumType = "Output Logical";
                    datumNumber = infoNumber.ToString();
                    returnValue = true;
                }
                else if (infoCharacteristic2 == 1) // Numerical
                {
                    if (infoConversionType == 2)
                    {
                        datumType = "Output Counter";
                        datumNumber = infoNumber.ToString();
                        returnValue = true;
                    }
                    else
                    {
                        datumType = "Output Measure";
                        datumNumber = infoNumber.ToString();
                        returnValue = true;
                    }
                }
            }

            return (returnValue);
        }

        private DatumTypes SoftoolS98_GetDatumType(string datumTypeString)
        {
            DatumTypes returnValue = DatumTypes.DigitalInput;

            if (datumTypeString == "Digital Input")
            {
                returnValue = DatumTypes.DigitalInput;
            }
            else if (datumTypeString == "Digital Output")
            {
                returnValue = DatumTypes.DigitalOutput;
            }
            else if (datumTypeString == "Count Input")
            {
                returnValue = DatumTypes.CountInput;
            }
            else if (datumTypeString == "Analog Input")
            {
                returnValue = DatumTypes.AnalogInput;
            }
            else if (datumTypeString == "Analog Output")
            {
                returnValue = DatumTypes.AnalogOutput;
            }

            return (returnValue);
        }

        private DatumTypes SoftoolsV1_11_GetDatumType(string datumTypeString)
        {
            DatumTypes returnValue = DatumTypes.DigitalInput;

            if (datumTypeString == "Input Logical")
            {
                returnValue = DatumTypes.DigitalInput;
            }
            else if (datumTypeString == "Output Logical")
            {
                returnValue = DatumTypes.DigitalOutput;
            }
            else if (datumTypeString == "Input Counter")
            {
                returnValue = DatumTypes.CountInput;
            }
            else if (datumTypeString == "Output Counter")
            {
                returnValue = DatumTypes.CountInput;
            }
            else if (datumTypeString == "Input Measure")
            {
                returnValue = DatumTypes.AnalogInput;
            }
            else if (datumTypeString == "Output Measure")
            {
                returnValue = DatumTypes.AnalogOutput;
            }

            return (returnValue);
        }

        private uint SoftoolS98ParseData(List<string[]> parsedData, byte stationUnderlyingProtocol)
        {
            LacbusPCDynTagSettings p = new LacbusPCDynTagSettings();
            string dynamicaddress;
            string varName;
            int MovType;
            uint varsize;
            uint parsedVariables = 0;
            foreach (var s in parsedData)
            {
                // Check the RTU number: it must match the one of the selected station
                int fieldIndex = SoftoolsV1_11_GetFieldIndex("65");
                if (fieldIndex == -1)
                {
                    continue;
                }
                if (s.GetLength(0) <= fieldIndex)
                {
                    continue;
                }
                string rtuNumberString = s[fieldIndex];
                UInt16 rtuNumber = 0;
                if (UInt16.TryParse(rtuNumberString, out rtuNumber) == false)
                {
                    continue;
                }
                if (rtuNumber != stationRtuNumber)
                {
                    continue;
                }

                // Used communication protocol (Supported protocols: SOFBUS PL = 1, SOFBUS SMS = 0)
                UInt16 infoCommunicationProtocol = 1;
                fieldIndex = SoftoolsV1_11_GetFieldIndex("1301");
                if ((fieldIndex >= 0) && (s.GetLength(0) > fieldIndex))
                {
                    string infoString = s[fieldIndex];
                    if (UInt16.TryParse(infoString, out infoCommunicationProtocol) == false)
                    {
                        infoCommunicationProtocol = 1;
                    }
                }
                if ((infoCommunicationProtocol != 1) && (infoCommunicationProtocol != 0))
                {
                    continue;
                }
                if ((infoCommunicationProtocol == 1) && (stationUnderlyingProtocol != (byte)LacbusPcUnderlyingProtocols.SofbusPL))
                {
                    continue;
                }
                else if ((infoCommunicationProtocol == 0) && (stationUnderlyingProtocol != (byte)LacbusPcUnderlyingProtocols.LacbusSofbusSMS))
                {
                    continue;
                }

                // Information label
                string infoLabel = String.Empty;
                fieldIndex = SoftoolsV1_11_GetFieldIndex("3");
                if (fieldIndex >= 0)
                {
                    infoLabel = s[fieldIndex];
                }

                // Information number
                UInt16 infoNumber = 0;
                fieldIndex = SoftoolsV1_11_GetFieldIndex("23");
                if ((fieldIndex >= 0) && (s.GetLength(0) > fieldIndex))
                {
                    string infoString = s[fieldIndex];
                    if (UInt16.TryParse(infoString, out infoNumber) == false)
                    {
                        continue;
                    }
                }
                else
                {
                    continue;
                }

                // Input or Output 
                UInt16 infoCharacteristic1 = 0;
                fieldIndex = SoftoolsV1_11_GetFieldIndex("6");
                if ((fieldIndex >= 0) && (s.GetLength(0) > fieldIndex))
                {
                    string infoString = s[fieldIndex];
                    if (UInt16.TryParse(infoString, out infoCharacteristic1) == false)
                    {
                        continue;
                    }
                }
                else
                {
                    continue;
                }

                // Data type
                UInt16 infoCharacteristic2 = 0;
                fieldIndex = SoftoolsV1_11_GetFieldIndex("7");
                if ((fieldIndex >= 0) && (s.GetLength(0) > fieldIndex))
                {
                    string infoString = s[fieldIndex];
                    if (UInt16.TryParse(infoString, out infoCharacteristic2) == false)
                    {
                        continue;
                    }
                }
                else
                {
                    continue;
                }

                // Data conversion parameters
                double rawMinValue = 0.0;
                double rawMaxValue = 0.0;
                double convMinValue = 0.0;
                double convMaxValue = 0.0;
                bool convParamValid = true;
                fieldIndex = SoftoolsV1_11_GetFieldIndex("58");
                if ((fieldIndex >= 0) && (s.GetLength(0) > fieldIndex))
                {
                    string infoString = s[fieldIndex];
                    if(double.TryParse(infoString, out rawMinValue) == false)
                    {
                        convParamValid = false;
                    }
                }
                fieldIndex = SoftoolsV1_11_GetFieldIndex("59");
                if ((fieldIndex >= 0) && (s.GetLength(0) > fieldIndex))
                {
                    string infoString = s[fieldIndex];
                    if (double.TryParse(infoString, out rawMaxValue) == false)
                    {
                        convParamValid = false;
                    }
                }
                fieldIndex = SoftoolsV1_11_GetFieldIndex("60");
                if ((fieldIndex >= 0) && (s.GetLength(0) > fieldIndex))
                {
                    string infoString = s[fieldIndex];
                    if (double.TryParse(infoString, out convMinValue) == false)
                    {
                        convParamValid = false;
                    }
                }
                fieldIndex = SoftoolsV1_11_GetFieldIndex("61");
                if ((fieldIndex >= 0) && (s.GetLength(0) > fieldIndex))
                {
                    string infoString = s[fieldIndex];
                    if (double.TryParse(infoString, out convMaxValue) == false)
                    {
                        convParamValid = false;
                    }
                }
                if(convParamValid == false)
                {
                    rawMinValue = 0.0;
                    rawMaxValue = 0.0;
                    convMinValue = 0.0;
                    convMaxValue = 0.0;
                }

                // Set the strings of the Datum Number and of the Datum Type    
                String datumNumber = String.Empty;
                String datumType = String.Empty;
                if (SoftoolS98_GetDatumNumberType(infoNumber, infoCharacteristic1, infoCharacteristic2, ref datumNumber, ref datumType) == false)
                {
                    continue;
                }

                // Add to the tree the tag for instantaneous data

                // Set the dynamic address
                dynamicaddress = string.Empty;
                LacbusPCDynTagSettings dynTagSettings = new LacbusPCDynTagSettings();
                dynTagSettings.StationName = baseImportTree.ReadStationName();// CmbStation.Text;
                dynTagSettings.LacbusPCDatumNumber = infoNumber;
                dynTagSettings.LacbusPCDatumType = SoftoolS98_GetDatumType(datumType);
                dynTagSettings.LacbusPCDatumCategory = DatumCategories.Instantaneous;
                if((dynTagSettings.LacbusPCDatumType == DatumTypes.AnalogInput) ||
                   (dynTagSettings.LacbusPCDatumType == DatumTypes.AnalogOutput) ||
                   (dynTagSettings.LacbusPCDatumType == DatumTypes.CountInput))
                {
                    dynTagSettings.LacbusPCConversionMinRawValue = rawMinValue;
                    dynTagSettings.LacbusPCConversionMaxRawValue = rawMaxValue;
                    dynTagSettings.LacbusPCConversionMinValue = convMinValue;
                    dynTagSettings.LacbusPCConversionMaxValue = convMaxValue;
                }
                if ((dynTagSettings.LacbusPCDatumType == DatumTypes.DigitalInput) || (dynTagSettings.LacbusPCDatumType == DatumTypes.AnalogInput)) // Read-only
                {
                    dynTagSettings.TagLinkType = (int)LinkType.Input;
                }
                dynamicaddress = dynTagSettings.ToString();

                // Set the variable name
                varName = String.Empty;
                if (String.IsNullOrWhiteSpace(infoLabel) == false)
                {
                    varName = infoLabel;
                }
                else
                {
                    varName = datumType + " " + datumNumber;
                }

                // Get MOVICON type and size
                MovType = -1;
                varsize = 0;
                MovType = SoftoolsV1_11GetMoviconTypeId(dynTagSettings.LacbusPCDatumType, dynTagSettings.LacbusPCDatumCategory, ref varsize);
                if (MovType == -1)
                {
                    continue;
                }

                // Add the tag to the tree
                var IVar = importDataModel.addImportData();
                IVar.Name = varName;
                ((ImportDataLacbus)IVar).UniqueName = varName + "_" + datumType + "_" + datumNumber;
                IVar.Address = datumNumber;
                IVar.DynAddress = dynamicaddress;
                ((ImportDataLacbus)IVar).TagTypeInt = MovType;
                IVar.Select = false;
                IVar.szType = datumType;
                ((ImportDataLacbus)IVar).Category = "Instantaneous";
                AddTreeItem(IVar);
                parsedVariables++;

                // Add by default a tag for historical data
                LacbusPCDynTagSettings historicalDynTagSettings = new LacbusPCDynTagSettings();
                historicalDynTagSettings.Parse(dynamicaddress);
                historicalDynTagSettings.TagLinkType = (int)LinkType.Input;
                historicalDynTagSettings.LacbusPCDatumCategory = DatumCategories.Historical;
                string historicalDynamicAddress = historicalDynTagSettings.ToString();
                var IVarh = importDataModel.addImportData();
                IVarh.Name = varName;
                IVarh.Name += " Historical";
                ((ImportDataLacbus)IVarh).UniqueName = varName + "_Historical" + "_" + datumType + "_" + datumNumber;
                IVarh.Address = datumNumber;
                IVarh.DynAddress = historicalDynamicAddress;
                ((ImportDataLacbus)IVarh).TagTypeInt = MovType;
                IVarh.Select = false;
                IVarh.szType = datumType;
                ((ImportDataLacbus)IVarh).Category = "Historical";
                AddTreeItem(IVarh);
            }
            return (parsedVariables);
        }

        private uint SoftoolsV1_11ParseData(List<string[]> parsedData, byte stationUnderlyingProtocol)
        {
            LacbusPCDynTagSettings p = new LacbusPCDynTagSettings();
            string dynamicaddress;
            string varName;
            int MovType;
            uint varsize;
            uint parsedVariables = 0;
            foreach (var s in parsedData)
            {
                // Check the RTU number: it must match the one of the selected station
                int fieldIndex = SoftoolsV1_11_GetFieldIndex("60002");
                if(fieldIndex == -1)
                {
                    continue;
                }
                if(s.GetLength(0) <= fieldIndex)
                {
                    continue;
                }
                string rtuNumberString = s[fieldIndex];
                UInt16 rtuNumber = 0;
                if(UInt16.TryParse(rtuNumberString, out rtuNumber) == false)
                {
                    continue;
                }
                if(rtuNumber != stationRtuNumber)
                {
                    continue;
                }

                // Used communication protocol (supported protocols: LACBUS RTU = 3, LACBUS SMS = 2)
                UInt16 infoCommunicationProtocol = 3;
                fieldIndex = SoftoolsV1_11_GetFieldIndex("60119");
                if ((fieldIndex >= 0) && (s.GetLength(0) > fieldIndex))
                {
                    string infoString = s[fieldIndex];
                    if (UInt16.TryParse(infoString, out infoCommunicationProtocol) == false)
                    {
                        infoCommunicationProtocol = 3;
                    }
                }
                if((infoCommunicationProtocol != 3) && (infoCommunicationProtocol != 2))
                {
                    continue;
                }
                if((infoCommunicationProtocol == 3) && (stationUnderlyingProtocol != (byte)LacbusPcUnderlyingProtocols.LacbusRTU))
                {
                    continue;
                }
                else if ((infoCommunicationProtocol == 2) && (stationUnderlyingProtocol != (byte)LacbusPcUnderlyingProtocols.LacbusSofbusSMS))
                {
                    continue;
                }

                // Information label
                string infoLabel = String.Empty;
                fieldIndex = SoftoolsV1_11_GetFieldIndex("60101");
                if (fieldIndex >= 0)
                {
                    infoLabel = s[fieldIndex];
                }

                // Information number
                UInt16 infoNumber = 0;
                fieldIndex = SoftoolsV1_11_GetFieldIndex("60102");
                if ((fieldIndex >= 0) && (s.GetLength(0) > fieldIndex))
                {
                    string infoString = s[fieldIndex];
                    if(UInt16.TryParse(infoString, out infoNumber) == false)
                    {
                        continue;
                    }
                }
                else
                {
                    continue;
                }

                // Information characteristic 1
                UInt16 infoCharacteristic1 = 0;
                fieldIndex = SoftoolsV1_11_GetFieldIndex("60103");
                if ((fieldIndex >= 0) && (s.GetLength(0) > fieldIndex))
                {
                    string infoString = s[fieldIndex];
                    if (UInt16.TryParse(infoString, out infoCharacteristic1) == false)
                    {
                        continue;
                    }
                }
                else
                {
                    continue;
                }

                // Information characteristic 2
                UInt16 infoCharacteristic2 = 0;
                fieldIndex = SoftoolsV1_11_GetFieldIndex("60104");
                if ((fieldIndex >= 0) && (s.GetLength(0) > fieldIndex))
                {
                    string infoString = s[fieldIndex];
                    if (UInt16.TryParse(infoString, out infoCharacteristic2) == false)
                    {
                        continue;
                    }
                }
                else
                {
                    continue;
                }

                // Information characteristic 3
                UInt16 infoCharacteristic3 = 0;
                fieldIndex = SoftoolsV1_11_GetFieldIndex("60105");
                if ((fieldIndex >= 0) && (s.GetLength(0) > fieldIndex))
                {
                    string infoString = s[fieldIndex];
                    if (UInt16.TryParse(infoString, out infoCharacteristic3) == false)
                    {
                        infoCharacteristic3 = 0;
                    }
                }

                // Conversion type for transmission
                UInt16 infoConversionType = 0;
                fieldIndex = SoftoolsV1_11_GetFieldIndex("60109");
                if ((fieldIndex >= 0) && (s.GetLength(0) > fieldIndex))
                {
                    string infoString = s[fieldIndex];
                    if (UInt16.TryParse(infoString, out infoConversionType) == false)
                    {
                        infoConversionType = 0;
                    }
                }

                // Archiving indication
                UInt16 infoArchiving = 0;
                fieldIndex = SoftoolsV1_11_GetFieldIndex("60117");
                if ((fieldIndex >= 0) && (s.GetLength(0) > fieldIndex))
                {
                    string infoString = s[fieldIndex];
                    if (UInt16.TryParse(infoString, out infoArchiving) == false)
                    {
                        infoArchiving = 0;
                    }
                }

                // Alarm indication
                UInt16 infoAlarm = 0;
                fieldIndex = SoftoolsV1_11_GetFieldIndex("60118");
                if ((fieldIndex >= 0) && (s.GetLength(0) > fieldIndex))
                {
                    string infoString = s[fieldIndex];
                    if (UInt16.TryParse(infoString, out infoAlarm) == false)
                    {
                        infoAlarm = 0;
                    }
                }

                // Set the strings of the Datum Number and of the Datum Type    
                String datumNumber = String.Empty;
                String datumType = String.Empty;
                if(SoftoolsV1_11_GetDatumNumberType(infoNumber, infoCharacteristic1, infoCharacteristic2, infoConversionType, ref datumNumber, ref datumType) == false)
                {
                    continue;
                }

                // Add to the tree the tag for instantaneous data

                // Set the dynamic address
                dynamicaddress = string.Empty;
                LacbusPCDynTagSettings dynTagSettings = new LacbusPCDynTagSettings();
                dynTagSettings.StationName = baseImportTree.ReadStationName();// CmbStation.Text;
                dynTagSettings.LacbusPCDatumNumber = infoNumber;
                dynTagSettings.LacbusPCDatumType = SoftoolsV1_11_GetDatumType(datumType);
                dynTagSettings.LacbusPCDatumCategory = DatumCategories.Instantaneous;
                if(infoCharacteristic3 == 0) // Read-only
                {
                    dynTagSettings.TagLinkType = (int)LinkType.Input;
                }
                else if ((dynTagSettings.LacbusPCDatumType == DatumTypes.DigitalInput) || (dynTagSettings.LacbusPCDatumType == DatumTypes.AnalogInput)) // Read-only
                {
                    dynTagSettings.TagLinkType = (int)LinkType.Input;
                }
                dynamicaddress = dynTagSettings.ToString();

                // Set the variable name
                varName = String.Empty;
                if(String.IsNullOrWhiteSpace(infoLabel) == false)
                {
                    varName = infoLabel;
                }
                else
                {
                    varName = datumType + " " + datumNumber;
                }

                // Get MOVICON type and size
                MovType = -1;
                varsize = 0;
                MovType = SoftoolsV1_11GetMoviconTypeId(dynTagSettings.LacbusPCDatumType, dynTagSettings.LacbusPCDatumCategory, ref varsize);
                if(MovType == -1)
                {
                    continue;
                }

                // Add the tag to the tree
                var IVar = importDataModel.addImportData();
                IVar.Name = varName;
                ((ImportDataLacbus)IVar).UniqueName = varName + "_" + datumType + "_" + datumNumber;
                IVar.Address = datumNumber;
                IVar.DynAddress = dynamicaddress;
                ((ImportDataLacbus)IVar).TagTypeInt = MovType;
                IVar.Select = false;
                IVar.szType = datumType;
                ((ImportDataLacbus)IVar).Category = "Instantaneous";
                if(dynTagSettings.LacbusPCDatumType == DatumTypes.DigitalOutput)
                {
                    ((ImportDataLacbus)IVar).ElemType = "Prototype_LacbusRTU_OutputLogical";
                }
                else if (dynTagSettings.LacbusPCDatumType == DatumTypes.AnalogOutput)
                {
                    ((ImportDataLacbus)IVar).ElemType = "Prototype_LacbusRTU_OutputMeasure";
                }
                AddTreeItem(IVar);
                parsedVariables++;

                // If necessary, add a tag for historical data
                if ((infoArchiving != 0) || (infoAlarm != 0))
                {
                    LacbusPCDynTagSettings historicalDynTagSettings = new LacbusPCDynTagSettings();
                    historicalDynTagSettings.Parse(dynamicaddress);
                    historicalDynTagSettings.TagLinkType = (int)LinkType.Input;
                    historicalDynTagSettings.LacbusPCDatumCategory = DatumCategories.Historical;
                    string historicalDynamicAddress = historicalDynTagSettings.ToString();
                    var IVarh = importDataModel.addImportData();
                    IVarh.Name = varName;
                    IVarh.Name += " Historical";
                    ((ImportDataLacbus)IVarh).UniqueName = varName + "_Historical" + "_" + datumType + "_" + datumNumber;
                    IVarh.Address = datumNumber;
                    IVarh.DynAddress = historicalDynamicAddress;
                    ((ImportDataLacbus)IVarh).TagTypeInt = MovType;
                    IVarh.Select = false;
                    IVarh.szType = datumType;
                    ((ImportDataLacbus)IVarh).Category = "Historical";
                    AddTreeItem(IVarh);
                }
            }
            return (parsedVariables);
        }

        private ImportDataModel LoadFile(string file)
        {          
            stationUnderlyingProtocol = (byte)LacbusPcUnderlyingProtocols.LacbusPC;
            if(!underlyingProtocolList.TryGetValue(baseImportTree.ReadStationName(), out stationUnderlyingProtocol))
            {
                stationUnderlyingProtocol = (byte)LacbusPcUnderlyingProtocols.LacbusPC;
            }
            stationRtuNumber = 0;
            if (!rtuNumberList.TryGetValue(baseImportTree.ReadStationName(), out stationRtuNumber))
            {
                stationRtuNumber = 0;
            }

            //importDataModel = new ImportDataModel(readStationName);

            file = file.ToLower();
            if (file.Contains(".txt") || file.Contains(".csv"))
            {
                //stationList.Clear();
                //underlyingProtocolList.Clear();
                //rtuNumberList.Clear();
                fieldIndexList.Clear();
  
                try
                {
                    using (new WaitCursor())
                    {
                        //ImportTree.Model = null;
                        if (importDataModel != null)
                            importDataModel.Dispose();
                        importDataModel = new ImportDataModelLacbus(readStationName);

                        List<string[]> parsedData = new List<string[]>();
                        LacbusPcImportFileTypes fileType = LacbusPcImportFileTypes.unsupportedFileType;
                        using (System.IO.StreamReader readFile = new System.IO.StreamReader(file))
                        {
                            string line;
                            string[] row;

                            // Read the first line of the file, containing the list of fields,
                            // check the file type, and fill appropriately the dictionary of the record fields
                            if((line = readFile.ReadLine()) == null)
                            {
                                MessageBox.Show(DriverCodeBase.UI.Properties.Resources.ImportErrorEmptyFile,
                                                Properties.Resources.ImportMsgBoxTitle);
                                return null;
                            }
                            fileType = CheckTheImportFileType(line);
                            if (fileType == LacbusPcImportFileTypes.unsupportedFileType)
                            {
                                MessageBox.Show(DriverCodeBase.UI.Properties.Resources.ImportErrorEmptyFile,
                                                Properties.Resources.ImportMsgBoxTitle);
                                return null;
                            }

                            // Check if the file type is coherent with the protocol supported by the selected station
                            if(((fileType == LacbusPcImportFileTypes.SoftoolsV1_11) && (stationUnderlyingProtocol != (byte)LacbusPcUnderlyingProtocols.LacbusRTU) && (stationUnderlyingProtocol != (byte)LacbusPcUnderlyingProtocols.LacbusSofbusSMS)) ||
                               ((fileType == LacbusPcImportFileTypes.SoftoolS98) && (stationUnderlyingProtocol != (byte)LacbusPcUnderlyingProtocols.SofbusPL) && (stationUnderlyingProtocol != (byte)LacbusPcUnderlyingProtocols.LacbusSofbusSMS)))
                            {
                                MessageBox.Show(Properties.Resources.ImportErrorInconsistentProtocol,
                                                Properties.Resources.ImportMsgBoxTitle);
                                return null;
                            }

                            // Get the data records
                            while ((line = readFile.ReadLine()) != null)
                            {
                                row = line.Split('\t');
                                parsedData.Add(row);
                            }
                        }

                        // Parse the data records
                        uint parsedVariables = 0;
                        if (fileType == LacbusPcImportFileTypes.SoftoolsV1_11)
                        {
                            parsedVariables = SoftoolsV1_11ParseData(parsedData, stationUnderlyingProtocol);
                        }
                        else if (fileType == LacbusPcImportFileTypes.SoftoolS98)
                        {
                            parsedVariables = SoftoolS98ParseData(parsedData, stationUnderlyingProtocol);
                        }
                        if(parsedVariables == 0)
                        {
                            MessageBox.Show(Properties.Resources.ImportErrorDataNotFoundForStation,
                                            Properties.Resources.ImportMsgBoxTitle);
                            return null;
                        }
                    }
                }
                catch (Exception ex)
                {
                    string errorMessage = Properties.Resources.ImportErrorOpeningFile;
                    errorMessage += ": ";
                    errorMessage += file;
                    MessageBox.Show(errorMessage,
                                    Properties.Resources.ImportMsgBoxTitle);
                    importDataModel = null;
                }
            }

            return importDataModel;
        }

        public int SoftoolsV1_11GetMoviconTypeId(DatumTypes type, DatumCategories category, ref uint varSize)
        {
            int nType = -1;
            varSize = 0;
            if(type == DatumTypes.DigitalInput)
            {
                if(category == DatumCategories.Instantaneous)
                {
                    nType = (int)UFUAModel.DataType.Boolean;
                    varSize = 1;
                }
                else
                {
                    nType = (int)UFUAModel.DataType.Boolean;
                    varSize = 1;
                }
            }
            else if(type == DatumTypes.DigitalOutput)
            {
                if (category == DatumCategories.Instantaneous)
                {
                    nType = (int)UFUAModel.DataType.Boolean;
                    varSize = 1;
                }
                else
                {
                    nType = (int)UFUAModel.DataType.Boolean;
                    varSize = 1;
                }
            }
            else if(type == DatumTypes.CountInput)
            {
                if (category == DatumCategories.Instantaneous)
                {
                    nType = (int)UFUAModel.DataType.Double;
                    varSize = 8;
                }
                else
                {
                    nType = (int)UFUAModel.DataType.Double;
                    varSize = 8;
                }
            }
            else if(type == DatumTypes.AnalogInput)
            {
                if (category == DatumCategories.Instantaneous)
                {
                    nType = (int)UFUAModel.DataType.Double;
                    varSize = 8;
                }
                else
                {
                    nType = (int)UFUAModel.DataType.Double;
                    varSize = 8;
                }
            }
            else if(type == DatumTypes.AnalogOutput)
            {
                if (category == DatumCategories.Instantaneous)
                {
                    nType = (int)UFUAModel.DataType.Double;
                    varSize = 8;
                }
                else
                {
                    nType = (int)UFUAModel.DataType.Double;
                    varSize = 8;
                }
            }

            return (nType);
        }

        internal void AddTreeItem(ImportData tag, ImportData parent = null)
        {
            tag.Parent = parent;
            ((ImportDataLacbus)tag).strId = tag.Id.ToString("X6");
            if (parent == null)
            {
                tag.TreeLevel = 0xFF;
                tag.parentId = -1;
                importDataModel.Children.Add(tag);
            }
            else
            {
                tag.TreeLevel = parent.TreeLevel - 1;
                tag.parentId = parent.Id;
                parent.Children.Add(tag);
            }
            ((ImportDataLacbus)tag).strLevel = tag.TreeLevel.ToString("X2");
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

    public class ImportDataLacbus : ImportData, IDisposable
    {
        public ImportDataLacbus(ImportDataModel inDataModel) :
            base(inDataModel)
        {
            dataModelLacbus = inDataModel as ImportDataModelLacbus;
        }
        private ImportDataModelLacbus dataModelLacbus { get; set; }

        //private int _Id;
        //public int Id
        //{
        //    get { return _Id; }
        //    set { _Id = value; }
        //}
        private string _PreName;
        public string PreName
        {
            get { return _PreName; }
            set { _PreName = value; }
        }
        private string _Name;
        public override string Name
        {
            get { return ImportTagName; }
            set { _Name = value; }
        }

        private string _UniqueName;
        public string UniqueName
        {
            get { return _UniqueName; }
            set { _UniqueName = value; }
        }

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
        //N.B.: In Movicon 3.4 property TagType was defined DataType type, so we defined a new TagTypeInt (int) and update import
        private int _TagTypeInt;
        public int TagTypeInt
        {
            get { return _TagTypeInt; }
            set { _TagTypeInt = value; }
        }
        //private string _szType;
        //public string szType
        //{
        //    get { return _szType; }
        //    set { _szType = value; }
        //}
        //private string _Description;
        //public string Description
        //{
        //    get { return _Description; }
        //    set { _Description = value; }
        //}
        private string _Category;
        public string Category
        {
            get { return _Category; }
            set { _Category = value; }
        }
        private string _ElemType;
        public string ElemType
        {
            get { return _ElemType; }
            set { _ElemType = value; }
        }
        //private UFUAModel.DataType _Type;
        //public UFUAModel.DataType Type
        //{
        //    get { return _Type; }
        //    set { _Type = value; }
        //}
        //private ImportTypes _ImportDataType;
        //public ImportTypes ImportDataType
        //{
        //    get { return _ImportDataType; }
        //    set { _ImportDataType = value; }
        //}

        private uint _Size;
        public uint Size
        {
            get { return _Size; }
            set { _Size = value; }
        }
        //private int _parentId;
        //public int parentId
        //{
        //    get { return _parentId; }
        //    set { _parentId = value; }
        //}

        //private uint _ArrayDimension;
        //public uint ArrayDimension
        //{
        //    get { return _ArrayDimension; }
        //    set { _ArrayDimension = value; }
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

        private string _strLevel;
        public string strLevel
        {
            get { return _strLevel; }
            set { _strLevel = value; }
        }

        private string _strId;
        public string strId
        {
            get { return _strId; }
            set { _strId = value; }
        }

        public string ImportTagName
        {
            get
            {
                return dataModel.getStationName() != "_" ?
                       dataModel.getStationName() + _PreName + _Name : _PreName + _Name;
            }
        }

        public override UFUAModel.DataType IconTagType
        {
            get {
                if (_TagTypeInt == -1)
                    return UFUAModel.DataType.Boolean;
                else
                    return (UFUAModel.DataType)_TagTypeInt; 
            }
        }

        #region IDisposable Members
        public void Dispose()
        {
            if (Children != null && Children.Count > 0)
            {
                foreach (ImportData el in Children)
                {
                    var sl = el as ImportDataLacbus;
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

    public class ImportDataModelLacbus : ImportDataModel, IDisposable
    {
        public ImportDataModelLacbus(GetStationName inGetStationName) :
            base(inGetStationName)
        {            
        }
        public override ImportData addImportData()
        {
            ImportDataLacbus importData = new ImportDataLacbus(this);
            return importData;
        }

        #region IDisposable Members
        public void Dispose()
        {
            foreach (ImportData el in Children)
            {
                ImportDataLacbus els7 = el as ImportDataLacbus;
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
}
