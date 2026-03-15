////////////////////////////////////////////////////////////////////////////////////////////////////
// file:	ImportTagsEditorTree.xaml.cs
//
// summary:	Implements the import tags editor tree.xaml class
////////////////////////////////////////////////////////////////////////////////////////////////////

using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using Utilities;
using System.Reflection;
using UFUAModel;
using System.Collections.ObjectModel;
using DriverCodeBase.UI;
using DriverCodeBase.UI.Controls;

namespace GESRTP2.UI
{
    /// <summary>   Interaction logic for ImportTagsEditor.xaml. </summary>
    public partial class ImportTagsEditorTree : UserControl, IDisposable
    {
        enum RecordData : byte
        {
            Name = 0,
            Type = 1,
            Descr = 2,
            ArraySize1 = 7,
            ArraySize2 = 8,
            MaxLength = 11,
            GeAddr = 15,
            Size = 21,
        }

        /// <summary>   The import data model. </summary>
        ImportDataModelGESRTP2 importDataModel;
        /// <summary>   true if already loaded. </summary>
        bool alreadyLoaded = false;
        /// <summary>   Name of the read station. </summary>
        public GetStationName readStationName;
        int m_lGlobalID = 0;
        BaseImportTree baseImportTree;

        /// <summary>   Default constructor. </summary>
        public ImportTagsEditorTree()
        {
            InitializeComponent();

            //executed at loaded
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

                //conn = lista[0];
                //protect = (lista[1].ToLower().IndexOf("true") != -1);

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
                baseImportTree.SetFileFilter("csv files|*.csv");
                baseImportTree.LoadImportFile = LoadFile;
                baseImportTree.CreateItemControl = CreateImportDataTreeItemControl;
                MainStack.Children.Add(baseImportTree);

                readStationName = baseImportTree.funcGetStationName();

                DataContext = this;
            };

        }

        private ImportDataTreeItemControl CreateImportDataTreeItemControl(object tag)
        {
            return ((ImportDataTreeItemControl)new ImportDataTreeItemControlGESRTP2(tag));
        }

        /// <summary>   Imports all selected tags. </summary>
        public void ImportSelectedTags()
        {
            string importfolder = baseImportTree.ReadFolderName();
            List<ImportData> list = new List<ImportData>();            
            list = baseImportTree.GetSelectedTags();
            string stationName = baseImportTree.ReadStationName();

            if (list.Count > 0)
            {             
                List<ImportTag> taglist = new List<ImportTag>();
                List<ImportPrototype> protolist = new List<ImportPrototype>();
                ObservableCollection<object> notToBeImported = new ObservableCollection<object>();
                foreach (var elem in list)
                {                    
                    if (elem != null && (notToBeImported.IndexOf(elem) < 0))
                    {
                        ImportDataGESRTP2 single = elem as ImportDataGESRTP2;
                        if (single != null)
                        {
                            bool isArray = single.ArrayDimension > 0 ? true : false;
                            GESRTP2DynTagSettings sp = new GESRTP2DynTagSettings();
                            sp.StationName = stationName;
                            sp.AreaType = single.AreaType;
                            sp.StringLength = single.StringLength;
                            if (GESRTP2Protocol.DataType(single.AreaType) == UFUAModel.DataType.Byte)
                                sp.StartAddress = (ushort)((((ImportDataGESRTP2)single).AddressNum - 1) / 8 + 1);
                            else
                                sp.StartAddress = ((ImportDataGESRTP2)single).AddressNum;

                            sp.TagLinkType = (int)GESRTP2Protocol.AreaLinkType(single.AreaType);
                            sp.VarType = single.TagType;

                            ImportTag tagtoimport = new ImportTag()
                            {
                                Name = single.Name,
                                DataType = single.TagType,
                                DynSettings = sp.ToString(),
                                Folder = importfolder,
                                ModelType = UFUAModel.ModelType.Variable,
                                Description = single.Description,
                                ArrayDimension = single.ArrayDimension
                            };

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
                        }
                    }
                }
                DataContext = new ImportObject() { PrototypesToImport = protolist, TagsToImport = taglist };
            }
            else
                DataContext = this;
        }
        
        ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>   Reads the tags from an input file. </summary>
        /////
        /// <exception cref="Exception">    Thrown when an exception error condition occurs. </exception>
        ///
        /// <param name="sender">   . </param>
        /// <param name="e">        . </param>
        ////////////////////////////////////////////////////////////////////////////////////////////////////
        private ImportDataModel LoadFile(string file)
        {
            string stationName = baseImportTree.ReadStationName();

            importDataModel = new ImportDataModelGESRTP2(readStationName);
            m_lGlobalID = 0;
                        
            file = file.ToLower();
            if (file.Contains(".csv"))
            {
                try
                {
                    using (new WaitCursor())
                    {
                        List<string[]> parsedData = new List<string[]>();
                        using (System.IO.StreamReader readFile = new System.IO.StreamReader(file))
                        {
                            string line;
                            string[] row;

                            while ((line = readFile.ReadLine()) != null)
                            {
                                //remove initial value for array
                                int nPos1;
                                int nPos = line.IndexOf("\"");
                                string szAux;
                                if (nPos != -1)
                                {
                                    nPos1 = line.IndexOf("\"", nPos + 1);
                                    if (nPos1 != -1)
                                        szAux = line.Substring(0, nPos) + "0" + line.Substring(nPos1+1);
                                    else
                                        continue;
                                }
                                else
                                {
                                    szAux = line;

                                }

                                row = szAux.Split(',');
                                if (row.Length == (int)RecordData.Size)
                                    parsedData.Add(row);
                            }
                        }
                        GESRTP2DynTagSettings p = new GESRTP2DynTagSettings();
                        string dynamicaddress;
                        DataType MovType;
                        uint varsize = 0;
                        uint ArrayCnt = 0;
                        UInt32 ArraySize1;
                        UInt32 ArraySize2;
                        UInt32 MaxLength;

                        UInt32 ArraySize2Cnt = 0;
                        UInt32 ArraySize2FirstElemet = 0;
                        UInt32 ArraySize2SecondElemet = 0;
                        int nElementArrayBiFirst = 0;
                        int nElementArrayBiSecond = 0;
                        string szNameArraySize2 = string.Empty;
                        ImportData  IVarBaseArrayDue = null;

                        var Parent = importDataModel.addImportData();

                        foreach (var s in parsedData)
                        {
                            MovType = GetMoviconTypeId(s[(int)RecordData.Type], ref varsize);
                            if (MovType == unchecked((DataType)(-1)))
                                continue;
                            if (!UInt32.TryParse(s[(int)RecordData.ArraySize2], out ArraySize2))
                                ArraySize2 = 0;                            
                               
                            if (!UInt32.TryParse(s[(int)RecordData.ArraySize1], out ArraySize1))
                                ArraySize1 = 0;

                            if (!UInt32.TryParse(s[(int)RecordData.MaxLength], out MaxLength))
                                MaxLength = 32;

                            String strAreaType = String.Empty;
                            UInt16 address;
                            int GeAddrLength = s[(int)RecordData.GeAddr].Length;

                            switch (GeAddrLength)
                            {
                                case 7:
                                case 8:
                                    Int32 i = 1;
                                    for (; Char.IsLetter(s[(int)RecordData.GeAddr], i); i++ )
                                        strAreaType += s[(int)RecordData.GeAddr].Substring(i,1);
                                    if (!UInt16.TryParse(s[(int)RecordData.GeAddr].Substring(i), out address))
                                        continue;
                                    break;
                                default:
                                    continue;
                            }

                            AreaTypes AreaType = GetGeAreaType(strAreaType, MovType);
                            if (AreaType == unchecked((AreaTypes)(-1)))
                                continue;

                            dynamicaddress = string.Empty;
                            p.StationName = stationName;
                            dynamicaddress = p.ToString();

                            var IVar = importDataModel.addImportData();
                            IVar.Name = s[(int)RecordData.Name];
                            ((ImportDataGESRTP2)IVar).GeAddress = s[(int)RecordData.GeAddr];
                            IVar.Description = s[(int)RecordData.Descr];
                            ((ImportDataGESRTP2)IVar).AddressNum = address;
                            ((ImportDataGESRTP2)IVar).AreaType = AreaType;
                            IVar.Id = m_lGlobalID++;
                            IVar.szType = s[(int)RecordData.Type];
                            IVar.TagType = MovType;
                            IVar.ArrayDimension = 0;
                            IVar.ArrayDimension = 0;
                            ((ImportDataGESRTP2)IVar).StringLength = MaxLength;

                            if (ArraySize2 > 0)
                            {
                                ArraySize2Cnt = ArraySize1 * ArraySize2;
                                ArraySize2FirstElemet = ArraySize2;
                                ArraySize2SecondElemet = ArraySize1;
                                nElementArrayBiFirst = -1;
                                nElementArrayBiSecond = 0;
                                szNameArraySize2 = s[(int)RecordData.Name];
                                IVar.szType = "ARRAY";
                                IVarBaseArrayDue = IVar;
                            }
                            else if (ArraySize2Cnt > 0)
                            {
                                string nNumElemetSplit;
                                if ((ArraySize2Cnt % ArraySize2FirstElemet) == 0)
                                {
                                    var IVarArray = importDataModel.addImportData();
                                    ((ImportDataGESRTP2)IVarArray).GeAddress = ((ImportDataGESRTP2)IVar).GeAddress;
                                    ((ImportDataGESRTP2)IVarArray).StringLength = ((ImportDataGESRTP2)IVar).StringLength;
                                    ((ImportDataGESRTP2)IVarArray).AreaType = ((ImportDataGESRTP2)IVarBaseArrayDue).AreaType;
                                    IVarArray.szType = IVarBaseArrayDue.szType;
                                    IVarArray.TagType = IVarBaseArrayDue.TagType;
                                    nElementArrayBiFirst++;
                                    nNumElemetSplit= "[" +  nElementArrayBiFirst + "]" ;
                                    IVarArray.Name = szNameArraySize2 + nNumElemetSplit;
                                    IVarArray.Id = IVar.Id;
                                    ((ImportDataGESRTP2)IVarArray).AddressNum = ((ImportDataGESRTP2)IVar).AddressNum;
                                    IVarArray.ArrayDimension = ArraySize2FirstElemet;
                                    AddTreeItem(IVarArray);
                                    Parent = IVarArray;
                                    nElementArrayBiSecond = 0;
                                    IVar.Id = m_lGlobalID++;
                                }
                                
                                nNumElemetSplit = "[" + nElementArrayBiFirst + "," + nElementArrayBiSecond +"]";
                                IVar.Name = szNameArraySize2 + nNumElemetSplit;
                                IVar.TagType = Parent.TagType;
                                AddTreeItem(IVar, Parent);
                                nElementArrayBiSecond++;
                                ArraySize2Cnt--;
                            }
                            else if (ArraySize1 > 0)
                            {
                                IVar.szType = "ARRAY";
                                IVar.ArrayDimension = ArraySize1;
                                AddTreeItem(IVar);
                                ArrayCnt = IVar.ArrayDimension;
                                Parent = IVar;

                            }
                            else if (ArrayCnt > 0 && s[(int)RecordData.Name].Contains(Parent.Name))
                            {

                                IVar.TagType = Parent.TagType;
                                AddTreeItem(IVar, Parent);
                                ArrayCnt--;
                            }
                            else
                            {
                                ArrayCnt = 0;
                                AddTreeItem( IVar);
                            }
                        }
                    }
                }
                catch (Exception ex)
                {
                    if (Environment.UserInteractive)
                        MessageBox.Show(ex.Message);
                    // clear all imported tags
                    importDataModel = null;
                }
            }

            if (importDataModel == null || importDataModel.Children.Count == 0)
            {
                MessageBox.Show(DriverCodeBase.UI.Properties.Resources.ImportErrorEmptyFile,
                                Properties.Resources.ImportMsgBoxTitle);
            }

            return importDataModel;
        }


        AreaTypes GetGeAreaType(String strAreaType, DataType MovType)
        {

            switch(strAreaType)
            {
                case  "R":
                    return AreaTypes.RegisterWords_R;
                case "AI":
                    return AreaTypes.AnalogInputWords_AI;
                case "AQ":
                    return AreaTypes.AnalogOutputWords_AQ;
                case "I":
            		if(MovType ==  DataType.Boolean)
            			return  AreaTypes.DiscreteInputBits_I;
            		else
            			return AreaTypes.DiscreteInputBytes_I;
                case "Q":
            		if(MovType ==  DataType.Boolean)
            			return  AreaTypes.DiscreteOutputBits_Q;
            		else
            			return AreaTypes.DiscreteOutputBytes_Q;
                case "T":
            		if(MovType ==  DataType.Boolean)
            			return  AreaTypes.DiscreteTemporaryBits_T;
            		else
            			return AreaTypes.DiscreteTemporaryBytes_T;
                case "SA":
            		if(MovType ==  DataType.Boolean)
            			return  AreaTypes.DiscreteBits_SA;
            		else
            			return AreaTypes.DiscreteBytes_SA;
                case "SB":
            		if(MovType ==  DataType.Boolean)
            			return  AreaTypes.DiscreteBits_SB;
            		else
            			return AreaTypes.DiscreteBytes_SB;
                case "SC":
            		if(MovType ==  DataType.Boolean)
            			return  AreaTypes.DiscreteBits_SC;
            		else
            			return AreaTypes.DiscreteBytes_SC;
                case "S":
            		if(MovType ==  DataType.Boolean)
            			return  AreaTypes.DiscreteBits_S_readOnly;
            		else
            			return AreaTypes.DiscreteBytes_S_readOnly;
                case "G":
            		if(MovType ==  DataType.Boolean)
            			return  AreaTypes.GeniusGlobalDataBits_G;
            		else
            			return AreaTypes.GeniusGlobalDataBytes_G;
                case "M":
                    if (MovType == DataType.Boolean)
                        return AreaTypes.DiscreteInternalBits_M;
                    else
                        return AreaTypes.DiscreteInternalBytes_M;
                default:
                    return unchecked((AreaTypes)(-1));

            }
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
            else if (Type.Contains("DWORD"))
            {
                nType = DataType.UInt32;
                VarSize = 4;
            }
            else if (Type.Contains("WORD"))
            {
                nType = DataType.UInt16;
                VarSize = 2;
            }
            else if (Type.Contains("DINT"))
            {
                nType = DataType.Int32;
                VarSize = 4;
            }
            else if (Type.Contains("INT"))
            {
                nType = DataType.Int16;
                VarSize = 2;
            }
            else if (Type.Contains("LREAL"))
            {
                nType = DataType.Double;
                VarSize = 8;
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
            else if (Type.Contains("STRING"))
            {
                nType = DataType.String;
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
                tag.parentId = parent.Id;
                parent.Children.Add(tag);
            }
        }

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

    /// <summary>   Import data. </summary>
    public class ImportDataGESRTP2 : ImportData, IDisposable
    {        
        ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>   Constructor. </summary>
        ///
        /// <param name="inDataModel" type="ImportDataModel">   The in data model. </param>
        ////////////////////////////////////////////////////////////////////////////////////////////////////
        public ImportDataGESRTP2(ImportDataModel inDataModel) : 
            base(inDataModel)
        {        
            dataModelGESRTP2 = inDataModel as ImportDataModelGESRTP2;
        }
        ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>   Gets or sets the data model. </summary>
        ///
        /// <value> The data model. </value>
        ////////////////////////////////////////////////////////////////////////////////////////////////////
        private ImportDataModelGESRTP2 dataModelGESRTP2 { get; set; }
        
        /// <summary>   The address. </summary>
        private string _GeAddress;
        ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>   Gets or sets the GE address. </summary>
        ///
        /// <value> The address. </value>
        ////////////////////////////////////////////////////////////////////////////////////////////////////
        public string GeAddress
        {
            get { return _GeAddress; }
            set { _GeAddress = value; }
        }

        //N.B.: In Movicon 3.4 property Address was defined string type, so we defined a new AddressNum (ushort) and update import
        private ushort _AddressNum;
        ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>   Gets or sets the address. </summary>
        ///
        /// <value> The address. </value>
        ////////////////////////////////////////////////////////////////////////////////////////////////////
        public ushort AddressNum
        {
            get { return _AddressNum; }
            set { _AddressNum = value; }
        }
       
        private AreaTypes _AreaType;
        public AreaTypes AreaType
        {
            get { return _AreaType; }
            set { _AreaType = value; }
        }
        private uint _StringLength;
        public uint StringLength
        {
            get { return _StringLength; }
            set { _StringLength = value; }
        }        
        
        #region IDisposable Members
        public void Dispose()
        {
            if (Children != null && Children.Count > 0)
            {
                foreach (ImportData el in Children)
                {
                    var sl = el as ImportDataGESRTP2;
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

    public class ImportDataModelGESRTP2 : ImportDataModel, IDisposable
    {
        public ImportDataModelGESRTP2(GetStationName inGetStationName) :
            base(inGetStationName)
        {            
        }

        public override ImportData addImportData()
        {
            ImportDataGESRTP2 importData = new ImportDataGESRTP2(this);
            return importData;
        }
        #region IDisposable Members

        public void Dispose()
        {
            foreach (ImportData el in Children)
            {
                ImportDataGESRTP2 els7 = el as ImportDataGESRTP2;
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
