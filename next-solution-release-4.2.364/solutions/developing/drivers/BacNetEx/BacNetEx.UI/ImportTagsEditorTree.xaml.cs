////////////////////////////////////////////////////////////////////////////////////////////////////
// file:	ImportTagsEditorTree.xaml.cs
//
// summary:	Implements the import tags editor tree.xaml class
////////////////////////////////////////////////////////////////////////////////////////////////////

using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using Utilities;
using System.Reflection;
using UFUAModel;
using System.Collections.ObjectModel;
using System.ComponentModel;
using DriverCodeBaseEx.UI;
using DriverCodeBaseEx.UI.Controls;

namespace BACnet.UI
{

    /// <summary>   Interaction logic for ImportTagsEditor.xaml. </summary>
    public partial class ImportTagsEditorTree : UserControl, IDisposable, IDataErrorInfo
    {
        enum RecordData : byte
        {
            keyname,
            device_obj_instance,
            object_name,
            object_type,
            object_instance,
            description,
        }

        public class ImportDataModelBACnet : ImportDataModel
        {
            public ImportDataModelBACnet(GetStationName inGetStationName) 
                : base(inGetStationName)
            { }

            public override ImportData addImportData()
            {
                ImportDataBACnet importData = new ImportDataBACnet(this);
                return importData;
            }
        }

        BaseImportTree baseImportTree;
        /// <summary>   The import data model. </summary>
        ImportDataModelBACnet importDataModel;

        

        /// <summary>   true if already loaded. </summary>
        bool alreadyLoaded = false;
        
        /// <summary>   Name of the read station. </summary>
        public GetStationName readStationName;

        int m_lGlobalID = 0;

        BACnetEnums.ImportVarNameFormat importVarNameFormat = BACnetEnums.ImportVarNameFormat.Keyname;

        /// <summary>   Default constructor. </summary>
        public ImportTagsEditorTree()
        {
            InitializeComponent();

            //executed at loaded
            Loaded += (o, e) =>
            {
                if (alreadyLoaded)
                    return;
                
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
                    colName = "Dynamic",
                    bindingName = "TagDynAddress", // BACnet driver's customization
                    colWidth = 350
                });
                columns.Add(new BaseImportTree.GridColData()
                {
                    colName = "Type",                    
                    bindingName = "TagType",
                    colWidth = 250
                });

                baseImportTree = new BaseImportTree(DriverName, lista[0],
                    (lista[1].ToLower().IndexOf("true") != -1), columns);
                baseImportTree.SetVisibleButtons();
                baseImportTree.SetFileFilter("csv files|*.csv");
                baseImportTree.LoadImportFile = LoadFile;
                baseImportTree.CreateItemControl = CreateImportDataTreeItemControl;
                MainStack.Children.Add(baseImportTree);

                readStationName = baseImportTree.funcGetStationName();
                
                DependencyPropertyDescriptor descriptor =
                DependencyPropertyDescriptor.FromProperty(CheckBox.IsCheckedProperty, typeof(CheckBox));
                descriptor.AddValueChanged(CkAllPropertyEnable, LoadFileCkAllPropertyEnable);
                descriptor =
                   DependencyPropertyDescriptor.FromProperty(CheckBox.IsCheckedProperty, typeof(CheckBox));
                descriptor.AddValueChanged(CkCOVEnable, LoadFileCkAllPropertyEnable);

                cboImportVarNameFormat.ItemsSource = new List<BACnetEnums.ImportVarNameFormat>()
                {
                    BACnetEnums.ImportVarNameFormat.Keyname,
                    BACnetEnums.ImportVarNameFormat.ObjectName,
                    BACnetEnums.ImportVarNameFormat.ObjectName_PropertyName_KeyName,
                };
                cboImportVarNameFormat.SelectedValue = BACnetEnums.ImportVarNameFormat.Keyname;

                DataContext = this;
            };

        }

        private ImportDataTreeItemControl CreateImportDataTreeItemControl(object tag)
        {
            return ((ImportDataTreeItemControl)new ImportDataTreeItemControlBACnet(tag));
        }

        /// <summary>   Imports all selected tags. </summary>
        public void ImportSelectedTags()
        {
            string importfolder = string.Empty;
            List<ImportData> list = new List<ImportData>();
            importfolder = baseImportTree.ReadFolderName();
            list = baseImportTree.GetSelectedTags();

            if (list.Count > 0)
            {
                List<ImportTag> taglist = new List<ImportTag>();
                Dictionary<string, ImportPrototype> protoMap = new Dictionary<string, ImportPrototype>();
                ObservableCollection<object> notToBeImported = new ObservableCollection<object>();
                List<BACnetEnums.PropertyIdentifier> propertyList = new List<BACnetEnums.PropertyIdentifier>();
                foreach (var elem in list)
                {
                    if (elem != null && (notToBeImported.IndexOf(elem) < 0))
                    {
                        ImportDataBACnet single = (ImportDataBACnet) elem;
                        if (single != null)
                        {
                            bool isArray = single.ArrayDimension > 0 ? true : false;

                            ImportTag tagtoimport = new ImportTag()
                            {
                                Name = single.Name,
                                DataType = single.TagType,
                                DynSettings = single.DynAddress,
                                Folder = importfolder,
                                ModelType = UFUAModel.ModelType.Variable,
                                Description = single.Description,
                                ArrayDimension = single.ArrayDimension,
                            };

                            adjustType(tagtoimport, single);
                            taglist.Add(tagtoimport);

                            if (isArray)
                            {
                                foreach (ImportData el in elem.Children)
                                {
                                    if (el != null)
                                    {
                                        notToBeImported.Add(el);
                                    }
                                }
                            }
                            if (!propertyList.Contains(single.ObjectProperty))
                                propertyList.Add(single.ObjectProperty);
                        }
                    }
                }
                createTypeList(propertyList, protoMap);

                DataContext = new ImportObject() { PrototypesToImport = protoMap.Values.ToList(), TagsToImport = taglist };
            }
            else
                DataContext = this;
        }
        private void adjustType(ImportTag tagtoimport, ImportDataBACnet single)
        {
            switch (single.ObjectProperty)
            {
                case BACnetEnums.PropertyIdentifier.EFFECTIVE_PERIOD:
                    tagtoimport.PrototypeModel = BACnetEnums.prototipe.DataRange.ToString();
                    tagtoimport.ModelType = UFUAModel.ModelType.ObjectType;
                    break;
                case BACnetEnums.PropertyIdentifier.EXCEPTION_SCHEDULE:
                    tagtoimport.PrototypeModel = BACnetEnums.prototipe.ScheduleSpecialEvent.ToString();
                    tagtoimport.ModelType = UFUAModel.ModelType.ObjectType;
                    break;
                case BACnetEnums.PropertyIdentifier.LIST_OF_OBJECT_PROPERTY_REFERENCES:
                    tagtoimport.PrototypeModel = BACnetEnums.prototipe.ListOfObjectPropertyReference.ToString();
                    tagtoimport.ModelType = UFUAModel.ModelType.ObjectType;
                    break;
                case BACnetEnums.PropertyIdentifier.WEEKLY_SCHEDULE:
                    tagtoimport.PrototypeModel = BACnetEnums.prototipe.ScheduleDay.ToString();
                    tagtoimport.ModelType = UFUAModel.ModelType.ObjectType;
                    break;
                case BACnetEnums.PropertyIdentifier.DATE_LIST:
                    tagtoimport.PrototypeModel = BACnetEnums.prototipe.DateList.ToString();
                    tagtoimport.ModelType = UFUAModel.ModelType.ObjectType;
                    break;
            }

        }
        private void createTypeList(List<BACnetEnums.PropertyIdentifier> propertyList, Dictionary<string, ImportPrototype> protoMap)
        {
            foreach (BACnetEnums.PropertyIdentifier property in propertyList)
            {
                switch (property)
                {
                    case BACnetEnums.PropertyIdentifier.EFFECTIVE_PERIOD:
                        createType("DataRange", "STRUCT", BACnetEnums.prototipe.DataRange,  protoMap);
                        break;
                    case BACnetEnums.PropertyIdentifier.EXCEPTION_SCHEDULE:
                        createType("ScheduleSpecialEvent", "STRUCT", BACnetEnums.prototipe.ScheduleSpecialEvent, protoMap);
                        break;
                    case BACnetEnums.PropertyIdentifier.LIST_OF_OBJECT_PROPERTY_REFERENCES:
                        createType("ListOfObjectPropertyReference", "STRUCT", BACnetEnums.prototipe.ListOfObjectPropertyReference,  protoMap);
                        break;
                    case BACnetEnums.PropertyIdentifier.WEEKLY_SCHEDULE:
                        createType("ScheduleDay", "STRUCT", BACnetEnums.prototipe.ScheduleDay,  protoMap);
                        break;
                    case BACnetEnums.PropertyIdentifier.DATE_LIST:
                        createType("DateList", "STRUCT", BACnetEnums.prototipe.DateList,  protoMap);
                        break;
                }
            }
        }

        private ImportData createType(string Name, string szType, BACnetEnums.prototipe Prototipe,  Dictionary<string, ImportPrototype> protoMap, DataType Type = DataType.Byte)
        {
            ImportDataBACnet newMember = importDataModel.addImportData() as ImportDataBACnet;
            if (newMember == null)
                return null;
            newMember.ArrayDimension = 0;
            newMember.Name = Name;
            newMember.ElemType = Prototipe.ToString();
            newMember.szType = szType;
            newMember.TagType = Type;
            if (szType == "STRUCT")
            {
                ImportDataBACnet rootItem = null;
                if (!protoMap.ContainsKey(Prototipe.ToString()))
                {
                    rootItem = importDataModel.addImportData() as ImportDataBACnet;
                    rootItem.Name = Prototipe.ToString();
                    rootItem.ElemType = Prototipe.ToString();
                    rootItem.szType = "STRUCT";
                    switch (Prototipe)
                    {
                        case BACnetEnums.prototipe.Date:
                            rootItem.Children.Add(createType("dayOfWeek", "Byte", BACnetEnums.prototipe.noPrototipe, protoMap, DataType.Byte));
                            rootItem.Children.Add(createType("day", "Byte", BACnetEnums.prototipe.noPrototipe, protoMap, DataType.Byte));
                            rootItem.Children.Add(createType("month", "Byte", BACnetEnums.prototipe.noPrototipe, protoMap, DataType.Byte));
                            rootItem.Children.Add(createType("year", "Byte", BACnetEnums.prototipe.noPrototipe, protoMap, DataType.Byte));
                            break;
                        case BACnetEnums.prototipe.Event:
                            rootItem.Children.Add(createType("hours", "Byte", BACnetEnums.prototipe.noPrototipe, protoMap, DataType.Byte));
                            rootItem.Children.Add(createType("min", "Byte", BACnetEnums.prototipe.noPrototipe, protoMap, DataType.Byte));
                            rootItem.Children.Add(createType("sec", "Byte", BACnetEnums.prototipe.noPrototipe, protoMap, DataType.Byte));
                            rootItem.Children.Add(createType("hundredths", "Byte", BACnetEnums.prototipe.noPrototipe, protoMap, DataType.Byte));
                            rootItem.Children.Add(createType("type", "Byte", BACnetEnums.prototipe.noPrototipe, protoMap, DataType.Byte));
                            rootItem.Children.Add(createType("value", "Double", BACnetEnums.prototipe.noPrototipe, protoMap, DataType.Double));
                            break;
                        case BACnetEnums.prototipe.DeviceObjectPropertyReference:
                            rootItem.Children.Add(createType("objectIdentifier", "UInt32", BACnetEnums.prototipe.noPrototipe, protoMap, DataType.UInt32));
                            rootItem.Children.Add(createType("propertyIdentifier", "UInt16", BACnetEnums.prototipe.noPrototipe, protoMap, DataType.UInt16));
                            rootItem.Children.Add(createType("propertyArrayIndex", "UInt16", BACnetEnums.prototipe.noPrototipe, protoMap, DataType.UInt16));
                            rootItem.Children.Add(createType("deviceIdentifier", "UInt32", BACnetEnums.prototipe.noPrototipe, protoMap, DataType.UInt32));
                            break;
                        case BACnetEnums.prototipe.DataRange:
                            rootItem.Children.Add(createType("startDate", "STRUCT", BACnetEnums.prototipe.Date, protoMap));
                            rootItem.Children.Add(createType("endDate", "STRUCT", BACnetEnums.prototipe.Date, protoMap));
                            break;
                        case BACnetEnums.prototipe.ScheduleSpecialEvent:
                            rootItem.Children.Add(createType("eventType", "Byte", BACnetEnums.prototipe.noPrototipe, protoMap, DataType.Byte));
                            rootItem.Children.Add(createType("period", "STRUCT", BACnetEnums.prototipe.CalendarEntry, protoMap));
                            rootItem.Children.Add(createType("Priority", "Byte", BACnetEnums.prototipe.noPrototipe, protoMap, DataType.Byte));
                            createElementArray(protoMap, rootItem, BACnetEnums.prototipe.Event);
                            break;
                        case BACnetEnums.prototipe.ListOfObjectPropertyReference:
                            createElementArray(protoMap, rootItem, BACnetEnums.prototipe.DeviceObjectPropertyReference);
                            break;
                        case BACnetEnums.prototipe.ScheduleDay:
                            createElementArray(protoMap, rootItem, BACnetEnums.prototipe.Event);
                            break;
                        case BACnetEnums.prototipe.DateList:
                            createElementArray(protoMap, rootItem, BACnetEnums.prototipe.CalendarEntry);
                            break;
                        case BACnetEnums.prototipe.CalendarEntry:
                            rootItem.Children.Add(createType("calendarType", "Byte", BACnetEnums.prototipe.noPrototipe, protoMap, DataType.Byte));
                            rootItem.Children.Add(createType("dataRange", "STRUCT", BACnetEnums.prototipe.DataRange, protoMap));
                            break;
                    }
                    addPrototype(protoMap, rootItem);
                }
            }
            return newMember;
        }

        private void createElementArray(Dictionary<string, ImportPrototype> protoMap, ImportData rootItem, BACnetEnums.prototipe Prototipe)
        {
            string arrayElem = Prototipe.ToString().ToLower();
            for (int index = 0; index < SizeOfArray; index++)
            {
                rootItem.Children.Add(createType(arrayElem + string.Format("_{0:d3}", index), "STRUCT", Prototipe, protoMap));
            }
        }

        private static ImportPrototype addPrototype(Dictionary<string, ImportPrototype> protoMap, ImportData elem)
        {
            ImportPrototype proto = null;
            ImportDataBACnet bacElem = elem as ImportDataBACnet;
            if (bacElem == null)
                return null;

            if (!protoMap.ContainsKey(bacElem.ElemType))
            {
                if (bacElem.Children.Count != 0)
                {
                    proto = new ImportPrototype();
                    proto.Name = bacElem.ElemType;
                    proto.Elements = new List<ImportTag>();

                    foreach (var t in bacElem.Children)
                    {
                        ImportDataBACnet el = t as ImportDataBACnet;
                        if (el != null)
                        {
                            ImportTag a = new ImportTag();

                            if (el.szType == "STRUCT")
                            {
                                ImportPrototype protoElem = addPrototype(protoMap, el);
                                if (protoElem == null)
                                    continue;

                                a.ModelType = UFUAModel.ModelType.ObjectType;
                                a.PrototypeModel = el.ElemType;
                            }
                            else
                            {
                                a.ModelType = ModelType.Variable;
                            }
                            a.ArrayDimension = el.ArrayDimension;
                            a.Name = el.Name;
                            a.DataType = el.TagType;
                            a.Description = el.Description;
                            proto.Elements.Add(a);
                        }
                    }
                    protoMap.Add(bacElem.ElemType, proto);
                }
            }
            else
                proto = protoMap[bacElem.ElemType];
            return proto;
        }

        private void LoadFileCkAllPropertyEnable(object sender, EventArgs e)
        {
            enableMessage = false;
            baseImportTree.ReloadInputFile();
            enableMessage = true;
        }

        bool enableMessage = true;
        private ImportDataModel LoadFile(string file)
        {
            importDataModel = new ImportDataModelBACnet(readStationName);
            m_lGlobalID = 0;
            List<string> dayOfWeek = new List<string>()
            {
                "Sunday",
                "Monday",
                "Tuesday",
                "Wednesday",
                "Thursday",
                "Friday",
                "Saturday",
            };
            file = file.ToLower();
            if (file.Contains(".csv"))
            {
                try
                {
                    using (new WaitCursor())
                    {
                        bool ThereIsObjectTypeNotManaged = false;
                        List<string[]> parsedData = new List<string[]>();
                        using (System.IO.StreamReader readFile = new System.IO.StreamReader(file))
                        {
                            string line;
                            string[] row;

                            bool bHeader = true;
                            while ((line = readFile.ReadLine()) != null)
                            {
                                if (bHeader)
                                {
                                    bHeader = !(line.Substring(0, 1) == "#" && line.IndexOf("keyname") >= 0);
                                    continue;
                                }
                                row = line.Split(';');
                                if (row.Length < (int)RecordData.description)
                                    continue;
                                if ((!string.IsNullOrEmpty(row[(int)RecordData.keyname].Trim())) &&
                                    (!string.IsNullOrEmpty(row[(int)RecordData.object_name].Trim())) &&
                                    (!string.IsNullOrEmpty(row[(int)RecordData.object_type].Trim())))
                                    parsedData.Add(row);
                            }
                        }
                        BACnetDynTagSettings p = new BACnetDynTagSettings();
                        var Parent = importDataModel.addImportData();

                        BACnetEnums.ObjectTypes BACnetObjectType;

                        p.StationName = baseImportTree.ReadStationName();
                        
                        foreach (var s in parsedData)
                        {

                            try
                            {
                                BACnetObjectType = (BACnetEnums.ObjectTypes)Convert.ToInt32(s[(int)RecordData.object_type]);
                            }
                            catch (FormatException ex)
                            {
                                continue;
                            }
                            p.BACnetObjectType = BACnetObjectType;
                            p.ObjectName = s[(int)RecordData.object_name];
                            p.InstanceNumber = -1;

                            int InstanceNumber;
                            //Instance Number must be numeric >=0; if value is non present or invalid, set -1 (Movicon try to get this value on Run-Time whith who-has
                            if (Int32.TryParse(s[(int)RecordData.object_instance], out InstanceNumber))
                            {
                                if (InstanceNumber >= 0)
                                    p.InstanceNumber = InstanceNumber;
                            }

                            Dictionary<string, List<BACnetEnums.PropertyIdentifier>> ObjectPropertyDictionary;
                            if (CkAllPropertyEnable.IsChecked == true)
                                ObjectPropertyDictionary = BACnetEnums.ObjectPropertyDictionaryAll;
                            else
                                ObjectPropertyDictionary = BACnetEnums.ObjectPropertyDictionaryRequired;

                            string BACnetObjectTypeKey = BACnetObjectType.ToString();
                            if (!ObjectPropertyDictionary.ContainsKey(BACnetObjectTypeKey))
                            {
                                ThereIsObjectTypeNotManaged = true;
                                continue;
                            }

                            foreach (BACnetEnums.PropertyIdentifier Property in ObjectPropertyDictionary[BACnetObjectTypeKey])
                            {
                                p.PropertyIdentifier = Property;
                                p.TagLinkType = (int)BACnetProtocol.AreaLinkType(BACnetObjectTypeKey, Property);
                                if (Property != BACnetEnums.PropertyIdentifier.WEEKLY_SCHEDULE)
                                {
                                    string szProperty = "." + Property.ToString();
                                    try
                                    {
                                        addImportData(p, BACnetObjectType, s, BACnetObjectTypeKey, Property, szProperty);
                                    }
                                    catch(Exception e)
                                    {
                                        continue;
                                    }
                                }
                                else
                                {
                                    ushort ArrayIndex = 1;
                                    foreach (string day in dayOfWeek)
                                    {
                                        string szProperty = "." + Property.ToString() + "_" + day;
                                        try
                                        {
                                            addImportData(p, BACnetObjectType, s, BACnetObjectTypeKey, Property, szProperty, ArrayIndex++);
                                        }
                                        catch
                                        {
                                            continue;
                                        }
                                    }
                                }
                            }

                        }
                        if (ThereIsObjectTypeNotManaged && enableMessage)
                        {
                            MessageBox.Show(Properties.Resources.ObjectTypeNotManaged);
                        }
                    }
                }
                catch (Exception ex)
                {
                    if (Environment.UserInteractive)
                        MessageBox.Show(ex.Message);
                    return null;
                }
            }

            if (importDataModel == null || importDataModel.Children.Count == 0)
            {
                MessageBox.Show(DriverCodeBaseEx.UI.Properties.Resources.ImportErrorEmptyFile,
                                Properties.Resources.ImportMsgBoxTitle);
            }

            return importDataModel;
        }

        private void addImportData(BACnetDynTagSettings p, BACnetEnums.ObjectTypes BACnetObjectType, string[] s, string BACnetObjectTypeKey, BACnetEnums.PropertyIdentifier Property, string szProperty, ushort ArrayIndex = 1)
        {
            ImportDataBACnet IVar = (ImportDataBACnet)importDataModel.addImportData();
            switch (importVarNameFormat)
            {
                case BACnetEnums.ImportVarNameFormat.Keyname:
                    IVar.Name = string.Format("{0}{1}", s[(int)RecordData.keyname], szProperty);
                    break;
                case BACnetEnums.ImportVarNameFormat.ObjectName:
                    IVar.Name = string.Format("{0}{1}",s[(int)RecordData.object_name], szProperty);
                    break;
                case BACnetEnums.ImportVarNameFormat.ObjectName_PropertyName_KeyName:
                    IVar.Name = string.Format("{0}{1}.{2}",s[(int)RecordData.object_name], szProperty, s[(int)RecordData.keyname]);
                    break;
            }            
            IVar.ObjectProperty = Property;
            IVar.TagType = BACnetEnums.ApplicationTagDataType[BACnetEnums.ObjectPropertyTypeDictionary[BACnetObjectTypeKey][Property]];
 
            if( (BACnetObjectTypeKey.Equals(BACnetEnums.ObjectTypes.BINARY_INPUT.ToString()) ||
                 BACnetObjectTypeKey.Equals(BACnetEnums.ObjectTypes.BINARY_OUTPUT.ToString()) ||
                 BACnetObjectTypeKey.Equals(BACnetEnums.ObjectTypes.BINARY_VALUE.ToString())) &&
                (Property == BACnetEnums.PropertyIdentifier.PRESENT_VALUE))
            {
                IVar.TagType = DataType.Boolean;
            }
            if (IVar.TagType == DataType.String)
                p.DataSize = 80;
            else
                p.DataSize = 0;

            if (BACnetEnums.isCovSupported(BACnetObjectTypeKey, Property.ToString()) &&
                CkCOVEnable.IsChecked == true)
                p.COVEnable = true;
            else
                p.COVEnable = false;
            p.ArrayIndex = ArrayIndex;

            IVar.DynAddress = p.ToString();
            if (s.Count() > (int)RecordData.description)
            {
                IVar.Description = s[(int)RecordData.description];
            }
            else
            {
                IVar.Description = String.Empty;
            }
            IVar.Address = s[(int)RecordData.object_name];
            IVar.BACnetObjectType = BACnetObjectType;
            IVar.Id = m_lGlobalID++;
            IVar.szType = BACnetObjectTypeKey + szProperty;
            IVar.ArrayDimension = 0;

            if (IVar.ObjectProperty == BACnetEnums.PropertyIdentifier.PRIORITY_ARRAY)
                IVar.ArrayDimension = 16;

            AddTreeItem(IVar);
        }



        ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>   Gets movicon type identifier. </summary>
        ///
        /// <param name="Type" type="string">       The type. </param>
        /// <param name="VarSize" type="ref uint">  [in,out] Size of the variable. </param>
        ///
        /// <returns>   The movicon type identifier. </returns>
        ////////////////////////////////////////////////////////////////////////////////////////////////////
        public DataType GetMoviconTypeId(string Type, ref uint VarSize)
        {
            DataType nType = unchecked((DataType)(-1));
            VarSize = 0;
            Type.Trim();
            if (Type.Length == 0)
                return (nType);
            Type = Type.ToUpper();
            if (Type.Contains("BIT"))
            {
                nType = DataType.Boolean;
                VarSize = 1;
            }
            else if (Type.Contains("BOOL"))
            {
                nType = DataType.Boolean;
                VarSize = 1;
            }
            else if (Type.Contains("BYTE"))
            {
                nType = DataType.Byte;
                VarSize = 1;
            }
            else if (Type.Contains("WORD"))
            {
                nType = DataType.UInt16;
                VarSize = 2;
            }
            else if (Type.Contains("DWORD"))
            {
                nType = DataType.UInt32;
                VarSize = 4;
            }
            else if (Type.Contains("INT"))
            {
                nType = DataType.Int16;
                VarSize = 2;
            }
            else if (Type.Contains("DINT"))
            {
                nType = DataType.Int32;
                VarSize = 4;
            }
            else if (Type.Contains("REAL"))
            {
                nType = DataType.Float;
                VarSize = 4;
            }
            else if (Type.Contains("CHAR"))
            {
                nType = DataType.SByte;
                VarSize = 1;
            }
            return (nType);

        }
  
        ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>   Add item to importDataModel. </summary>
        ///
        /// <param name="tag">      . </param>
        /// <param name="parent">   (Optional) </param>
        ////////////////////////////////////////////////////////////////////////////////////////////////////
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
                parent.Children.Add(tag);
            }
        }
        
        //public override baseImportTree.ImportDataTreeItemControl CreateImportDataTreeItemControl(object tag)
        //{
        //    return new ImportDataTreeItemControl(tag);
        //}

        #region IDisposable Members

        ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>
        /// Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged
        /// resources.
        /// </summary>
        ////////////////////////////////////////////////////////////////////////////////////////////////////
        public void Dispose()
        {
            using (new WaitCursor())
            {
                if (baseImportTree != null)
                {
                    baseImportTree.Dispose();
                    baseImportTree = null;
                }
            }
        }

        #endregion

        #region Properties

        private int _SizeOfArray = 1;

        public int SizeOfArray
        {
            get { return _SizeOfArray; }
            set
            {
                _SizeOfArray = value;
            }
        }

        #endregion

        #region IDataErrorInfo Members

        ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>   Gets an error message indicating what is wrong with this object. </summary>
        ///
        /// <value>
        /// An error message indicating what is wrong with this object. The default is an empty string
        /// ("").
        /// </value>
        ////////////////////////////////////////////////////////////////////////////////////////////////////
        public string Error
        {
            get
            {
                var context = new System.ComponentModel.DataAnnotations.ValidationContext(this, null, null);
                var results = new List<System.ComponentModel.DataAnnotations.ValidationResult>();

                return !System.ComponentModel.DataAnnotations.Validator.TryValidateObject(this, context, results)
                    ? string.Join(Environment.NewLine, results.Select(x => x.ErrorMessage))
                    : null;
            }
        }

        ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>   Indexer to get items within this collection using array index syntax. </summary>
        ///
        /// <param name="propertyName" type="string">   Name of the property. </param>
        ///
        /// <returns>   The indexed item. </returns>
        ////////////////////////////////////////////////////////////////////////////////////////////////////
        public string this[string propertyName]
        {
            get
            {
                String s = PerformValidation(propertyName);
                if (!String.IsNullOrEmpty(s))
                    return s;
                var context = new System.ComponentModel.DataAnnotations.ValidationContext(this, null, null)
                {
                    MemberName = propertyName
                };

                var results = new List<System.ComponentModel.DataAnnotations.ValidationResult>();
                var propertyInfo = GetType().GetProperty(propertyName);
                if (propertyInfo != null)
                {
                    var value = propertyInfo.GetValue(this, null);

                    return !System.ComponentModel.DataAnnotations.Validator.TryValidateProperty(value, context, results)
                        ? string.Join(Environment.NewLine, results.Select(x => x.ErrorMessage))
                        : null;
                }

                return null;
            }
        }

        ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>   Performs the validation action. </summary>
        ///
        /// <param name="propertyName" type="String">   Name of the property. </param>
        ///
        /// <returns>   A String. </returns>
        ////////////////////////////////////////////////////////////////////////////////////////////////////
        protected virtual String PerformValidation(String propertyName)
        {

            if (propertyName == "SizeOfArray")
            {
                if (SizeOfArray < 1 )
                    return Properties.Resources.ZeroIsNotAccepted;
            }

            return null;
        }

        #endregion

        private void cboImportVarNameFormat_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            importVarNameFormat = (BACnetEnums.ImportVarNameFormat)(sender as ComboBox).SelectedValue;
            enableMessage = false;
            baseImportTree.ReloadInputFile();
            enableMessage = true;
        }
    }



    ///// <summary>   Import data. </summary>
    public class ImportDataBACnet : ImportData
    {
        //    ////////////////////////////////////////////////////////////////////////////////////////////////////
        //    /// <summary>   Constructor. </summary>
        //    ///
        //    /// <param name="inDataModel" type="ImportDataModel">   The in data model. </param>
        //    ////////////////////////////////////////////////////////////////////////////////////////////////////
        public ImportDataBACnet(ImportDataModel inDataModel) 
            : base(inDataModel)
        {
        }

        private string _ElemType;
        public string ElemType
        {
            get { return _ElemType; }
            set { _ElemType = value; }
        }

        

        private BACnetEnums.ObjectTypes _BACnetObjectType;
        public BACnetEnums.ObjectTypes BACnetObjectType
        {
            get { return _BACnetObjectType; }
            set { _BACnetObjectType = value; }
        }
        private BACnetEnums.PropertyIdentifier _ObjectProperty;
        public BACnetEnums.PropertyIdentifier ObjectProperty
        {
            get { return _ObjectProperty; }
            set { _ObjectProperty = value; }
        }
    };

    

}
