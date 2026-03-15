using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using Utilities;
using System.Reflection;
using UFUAModel;
using System.Collections.ObjectModel;
using System.Text.RegularExpressions;
using DriverCodeBase.UI;
using DriverCodeBase.UI.Controls;
using OmronEthernetIP;
using System.Text;

namespace OmronEthernetIP.UI
{
    public enum ImportTypes
    {
        Unknown,
        Standard,
        Array,
        Struct,
    }
        
    /// <summary>
    /// Interaction logic for ImportTagsEditorTree.xaml
    /// </summary>
    public partial class ImportTagsEditorTree : UserControl, IDisposable
    {
        private const string STRUCT_MEMBER_SEPARATOR_CHAR = ".";

        ImportDataModelOmronEthernetIP importDataModel;
        bool alreadyLoaded = false;
        int m_lGlobalID = 0;
        Dictionary<string, List<string>> m_mapStruct;
        public GetStationName readStationName;
        BaseImportTree baseImportTree;

        private class ArrayInfo
        {
            public uint MinIndex = 0;
            public uint MaxIndex = 0;
            public uint Size = 0;

            public void CalculateSize()
            {
                Size = (MaxIndex - MinIndex) + 1;
            }
        }

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
                    colWidth = 300
                });

                baseImportTree = new BaseImportTree(DriverName, lista[0],
                   (lista[1].ToLower().IndexOf("true") != -1), columns);
                baseImportTree.SetVisibleButtons();
                baseImportTree.SetFileFilter("TXT files|*.txt");
                baseImportTree.LoadImportFile = LoadFile;
                baseImportTree.CreateItemControl = CreateImportDataTreeItemControl;
                MainStack.Children.Add(baseImportTree);

                readStationName = baseImportTree.funcGetStationName();

                DataContext = this;

                m_mapStruct = new Dictionary<string, List<string>>();
            };
        }

        private ImportDataTreeItemControl CreateImportDataTreeItemControl(object tag)
        {
            return ((ImportDataTreeItemControl)new ImportDataTreeItemControlOmronEthernetIP(tag));
        }

        // Used to check if all the strings of a structure have the same length
        void SetStructStringLengthsShortForm(ref string stringLengths)
        {
            string[] arrayOfStrings = stringLengths.Split(';');
            Int64 commonLength = 0;
            foreach (string elem in arrayOfStrings)
            {
                int index = elem.IndexOf(":");
                if (index > 0)
                {
                    string lengthString = elem.Substring(index + 1);
                    if (!String.IsNullOrWhiteSpace(lengthString))
                    {
                        Int64 stringSize = 0;
                        if (Int64.TryParse(lengthString, out stringSize))
                        {
                            if (stringSize > 0)
                            {
                                if (commonLength == 0)
                                {
                                    commonLength = stringSize;
                                }
                                else
                                {
                                    if (commonLength != stringSize)
                                    {
                                        commonLength = 0;
                                        break;
                                    }
                                }
                            }
                        }
                    }
                }
            }

            if (commonLength == 0)
            {
                return;
            }

            // All the strings have the same length, so set the short form of the info: ":<common string length>"
            stringLengths = String.Format(":{0}", commonLength);
        }

        public void ImportSelectedTags()
        {
            string importfolder = baseImportTree.ReadFolderName();
            List<ImportData> list = new List<ImportData>();
            list = baseImportTree.GetSelectedTags();
            string stationName = baseImportTree.ReadStationName();

            if (list.Count > 0)
            {
                List<ImportTag> taglist = new List<ImportTag>();
                Dictionary<string, ImportPrototype> protoMap = new Dictionary<string, ImportPrototype>();
                ObservableCollection<object> notToBeImported = new ObservableCollection<object>();
                foreach (var elemList in list)
                {
                    ImportData elem = elemList as ImportData;
                    if (elem != null && (notToBeImported.IndexOf(elem) < 0))
                    {
                        ImportDataOmronEthernetIP single = elem as ImportDataOmronEthernetIP;

                        bool isArray = false;
                        bool isStructure = false;

                        switch (((ImportDataOmronEthernetIP)single).ImportType)
                        {
                            case ImportTypes.Struct:
                                isStructure = true;
                                break;
                            case ImportTypes.Array:
                                isArray = true;
                                break;
                        }

                        OmronEthernetIPDynTagSettings sp = new OmronEthernetIPDynTagSettings();
                        string stringSize = String.Empty;
                        if (!isStructure)
                        {
                            if (single.ElemType.Contains("STRING("))
                            {
                                int OpenRoundBrackets = single.ElemType.IndexOf("(");
                                int CloseRoundBrackets = single.ElemType.IndexOf(")");
                                stringSize = single.ElemType.Substring(OpenRoundBrackets + 1, CloseRoundBrackets - OpenRoundBrackets - 1).Trim(new char[] { '\r', '\n', '\t', ' ' });
                                single.ElemType = single.ElemType.Substring(0, OpenRoundBrackets);
                                // if addess don't contain string's length is a struct's member--> add it
                                if (elem.Address.IndexOf(string.Format(":{0}", stringSize)) < 0)
                                    elem.Address += string.Format(":{0}", stringSize);
                            }
                            sp.TagFormat = (TagFormats)Enum.Parse(typeof(TagFormats), single.ElemType);
                        }
                        else
                        {
                            sp.TagFormat = TagFormats.STRUCTURE;
                            sp.IsObjectType = true;
                        }

                        if (!sp.ParseAddress(single.Address))
                            continue;

                        sp.StationName = stationName;

                        // FOGBUGZ 11412 and 11413
                        sp.ArrayDimension = single.ArrayDimension;

                        sp.TagLinkType = single.LinkType;

                        ImportPrototype proto = null;
                        if (isStructure)
                        {
                            // Add to the dynamic link the lengths of the structure's elements of type string
                            string stringFieldLengths = String.Empty;
                            BuildStringLengths(single, ref stringFieldLengths);
                            if (!String.IsNullOrWhiteSpace(stringFieldLengths))
                            {
                                // If all the strings of the structure have the same length, set the short form for this info
                                SetStructStringLengthsShortForm(ref stringFieldLengths);

                                sp.StructStringFieldLengths = stringFieldLengths;
                            }

                            proto = addPrototype(protoMap, single);
                            if (proto == null)
                                continue;
                        }

                        single.DynAddress = sp.ToString();

                        // FOGBUGZ 11412 and 11413
                        string importTagName = (single.PreName + single.Name).Trim(']');

                        ImportTag tagtoimport = new ImportTag()
                        {
                            // FOGBUGZ 11412 and 11413
                            Name = importTagName,

                            DataType = single.Type,
                            DynSettings = single.DynAddress,
                            Folder = importfolder,
                            ModelType = (isStructure ? UFUAModel.ModelType.ObjectType : UFUAModel.ModelType.Variable),
                            Description = single.Description,
                            ArrayDimension = single.ArrayDimension
                        };
                        if (isStructure)
                        {
                            tagtoimport.PrototypeModel = proto.Name;
                            tagtoimport.ModelType = UFUAModel.ModelType.ObjectType;
                        }

                        taglist.Add(tagtoimport);

                        // FOGBUGZ 11412 and 11413
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

                DataContext = new ImportObject() { PrototypesToImport = protoMap.Values.ToList(), TagsToImport = taglist, SearchForCompatiblePrototypes = true };
            }
            else
                DataContext = this;
        }

        private static void BuildStringLengths(ImportData elem, ref string stringLengthInfo)
        {
            if (elem.Children.Count != 0)
            {
                foreach (var t in elem.Children)
                {
                    ImportData el = t as ImportData;
                    if (el != null)
                    {
                        if (el.szType == "STRUCT")
                        {
                            BuildStringLengths(el, ref stringLengthInfo);
                        }
                        else if (((ImportDataOmronEthernetIP)el).ElemType.Contains("STRING("))
                        {
                            // Parse the string length
                            int initIndex = ((ImportDataOmronEthernetIP)el).ElemType.IndexOf("STRING(");
                            if (initIndex >= 0)
                            {
                                int endIndex = ((ImportDataOmronEthernetIP)el).ElemType.IndexOf(")");
                                if (endIndex > (initIndex + 6))
                                {
                                    initIndex += 6;
                                    string stringSize = String.Empty;
                                    stringSize = ((ImportDataOmronEthernetIP)el).ElemType.Substring(initIndex + 1, endIndex - initIndex - 1).Trim(new char[] { '\r', '\n', '\t', ' ' });
                                    if (!String.IsNullOrWhiteSpace(stringSize))
                                    {
                                        Int64 stringLength = 0;
                                        if (Int64.TryParse(stringSize, out stringLength))
                                        {
                                            // Valid length? If yes, add it to stringLengthInfo 
                                            if (stringLength > 0)
                                            {
                                                // Build the complete name of the structure element
                                                string elementName = el.Name;
                                                if (!String.IsNullOrWhiteSpace(((ImportDataOmronEthernetIP)el).PreName))
                                                {
                                                    initIndex = ((ImportDataOmronEthernetIP)el).PreName.IndexOf(".");
                                                    if (initIndex >= 0)
                                                    {
                                                        string auxString = ((ImportDataOmronEthernetIP)el).PreName.Substring(initIndex + 1);
                                                        if (!String.IsNullOrWhiteSpace(auxString))
                                                        {
                                                            elementName = auxString + el.Name;
                                                        }
                                                    }
                                                }

                                                if (!String.IsNullOrWhiteSpace(stringLengthInfo))
                                                {
                                                    stringLengthInfo += ";";
                                                }
                                                stringLengthInfo += elementName + ":" + stringSize;
                                            }
                                        }
                                    }
                                }
                            }
                        }
                    }
                }
            }
        }

        private static ImportPrototype addPrototype(Dictionary<string, ImportPrototype> protoMap, ImportData elem)
        {
            ImportPrototype proto = null;
            if (!protoMap.ContainsKey(((ImportDataOmronEthernetIP)elem).ElemType))
            {
                if (elem.Children.Count != 0)
                {
                    proto = new ImportPrototype();
                    proto.Name = ((ImportDataOmronEthernetIP)elem).ElemType;
                    proto.Elements = new List<ImportTag>();

                    foreach (var t in elem.Children)
                    {
                        ImportData el = t as ImportData;
                        if (el != null)
                        {
                            ImportTag a = new ImportTag();

                            if (el.szType == "STRUCT")
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
                            a.ArrayDimension = el.ArrayDimension;
                            a.Name = el.Name;
                            a.DataType = ((ImportDataOmronEthernetIP)el).Type;
                            a.Description = el.Description;
                            proto.Elements.Add(a);
                        }
                    }
                    protoMap.Add(((ImportDataOmronEthernetIP)elem).ElemType, proto);
                }
            }
            else
                proto = protoMap[((ImportDataOmronEthernetIP)elem).ElemType];
            return proto;
        }

        private ImportDataModel LoadFile(string file)
        {        
            importDataModel = new ImportDataModelOmronEthernetIP(readStationName);
            
            file = file.ToLower();
            if (file.Contains(".txt"))
            {
                DisplayImportFileTXT(file);
                if (importDataModel == null || importDataModel.Children.Count == 0)
                {
                    MessageBox.Show(DriverCodeBase.UI.Properties.Resources.ImportErrorEmptyFile,
                                    Properties.Resources.ImportMsgBoxTitle);
                }
            }

            return importDataModel;
        }

        private void DisplayImportFileTXT(string fileTXT)
        {
            try
            {
                using (new WaitCursor())
                {
                    OmronEthernetIPStationSettings station = (OmronEthernetIPStationSettings)baseImportTree.GetStationSettings(baseImportTree.ReadStationName());

                    // clear struct list before to import new data
                    m_mapStruct.Clear();

                    Dictionary<string, string[]> parsedData = new Dictionary<string, string[]>();
                    using (System.IO.StreamReader readFile = new System.IO.StreamReader(fileTXT))
                    {
                        string line;
                        bool firstRowSkipped = false;
                        while ((line = readFile.ReadLine()) != null)
                        {
                            if (!firstRowSkipped)
                            {
                                firstRowSkipped = true;
                                continue;
                            }                            
                            ParseVariable(line, ref parsedData, station);
                        }
                    }

                    OmronEthernetIPDynTagSettings p = new OmronEthernetIPDynTagSettings();
                    string moviconType = String.Empty;
                    string dynamicaddress = String.Empty;
                    uint varSize = 0;
                    ImportTypes importVariableType = ImportTypes.Unknown;
                    int linkType = 1;
                    foreach (var s in parsedData.Values)
                    {
                        // force vartype field to UPPER for vartype matching
                        s[2] = s[2].ToUpper();

                        moviconType = String.Empty;
                        dynamicaddress = string.Empty;
                        varSize = 0;
                        // At least 7 fields must be present in the line
                        if (s.Count() < 7)
                        {
                            continue;
                        }

                        // The 2nd field ("NAME") must not be an empty string
                        if (String.IsNullOrWhiteSpace(s[1]))
                        {
                            continue;
                        }

                        // The 6th field ("TAGLINK") must be equal to "TRUE"
                        if (s[5] == "TRUE")
                        {
                            // RW field
                            if (s[6] == "RW")
                            {
                                // Input/Output
                                linkType = 1;
                            }
                            else if (s[6] == "R")
                            {
                                // Input
                                linkType = 0;
                            }
                            else if (s[6] == "W")
                            {
                                // Output
                                linkType = 2;
                            }
                            else
                            {
                                continue;
                            }
                        }
                        else if (s[5] == "IN")
                        {
                            // Input
                            linkType = 0;
                        }
                        else if (s[5] == "OUT")
                        {
                            // Output
                            linkType = 2;
                        }
                        else
                        {
                            continue;
                        }

                        List<ArrayInfo> arrayDim = new List<ArrayInfo>() { new ArrayInfo(), new ArrayInfo(), new ArrayInfo() };                        
                        string arrayElementType = String.Empty;
                        importVariableType = GetVarTypeAndSizeTXT(s[2], ref varSize, ref moviconType,
                                                                  ref arrayDim, ref arrayElementType);
                        if (importVariableType == ImportTypes.Struct)
                        {
                            AddStructTxt(String.Empty, s[1], s[7], s[1],
                                                s[4], s[7], linkType, station);
                        }
                        else if (importVariableType == ImportTypes.Standard)
                        {
                            AddStandardVarTxt(String.Empty, s[1], String.Empty, s[1], s[4], s[2], moviconType, varSize, linkType);
                        }
                        else if (importVariableType == ImportTypes.Array)
                        {
                            AddArrayTxt(String.Empty, s[1], String.Empty, s[1],
                                        s[4], arrayElementType, varSize, arrayDim,
                                        ImportTypes.Standard, moviconType, linkType, station);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                string message;
                message = String.Format(Properties.Resources.ImportExceptionFileParsing, ex.ToString(), fileTXT);
                MessageBox.Show(message, Properties.Resources.ImportMsgBoxTitle);
            }
        }

        private MatchCollection SplitStructLevel(string varDeclaration)
        {
            //bad struct string format
            if (string.IsNullOrWhiteSpace(varDeclaration) || varDeclaration.Substring(0,1) == STRUCT_MEMBER_SEPARATOR_CHAR || varDeclaration.Substring(varDeclaration.Length -1 , 1) == STRUCT_MEMBER_SEPARATOR_CHAR)
                return null;
            else {
                // split struct declaration by <.> (member separator char); blank matches was removed
                //return Regex.Matches(varDeclaration, @"\b\w\w*");
                // split struct's elements by <.> separator char
                return Regex.Matches(varDeclaration, string.Format("([^{0}]+)", STRUCT_MEMBER_SEPARATOR_CHAR));
            }
        }

        /// <summary>
        /// Retrieve struct name (and relative father node) until selected levelNr
        /// </summary>
        /// <param name="memberName"></param>
        /// <returns></returns>
        private void GetStructLevel(MatchCollection members, string varDeclaration, int levelNr, out string structName, out string memberName, out bool lastLevel)
        {
            structName = String.Empty;
            memberName = String.Empty;
            lastLevel = false;
                        
            if (members == null || members.Count == 0)
                return;

            //// split struct declaration by <.> (member separator char)
            //arrayStructLevel = (Regex.Matches(varDeclaration, @"\[(.*?)\]")).Count;

            lastLevel = ((members.Count - 1) <= levelNr);

            if (members.Count > levelNr)
            {
                memberName = members[levelNr].Value.ToString();

                // if last element, struct name is mebmber name of previous level 
                int Index = (lastLevel ? levelNr - 1 : levelNr);
                if (Index < 0)
                    Index = 0;

                //structName = UFUAModel.Helpers.NameValidator.EnsureValidName(varDeclaration.Substring(0, members[Index].Index + members[Index].Length));
                structName = varDeclaration.Substring(0, members[Index].Index + members[Index].Length);
            }
        }

        private void AddMemberToStruct(string structName, string dataType, string memberName, string comment, ref Dictionary<string, string[]> parsedData, string[] row, OmronEthernetIPStationSettings station)
        {
            // if big array, convert into a "simple variable" --> remove from struct
            if (IsBigArrayOrMultiDimensionaArray(dataType, station))
            {
                //varmame
                row[1] = string.Format("{0}{1}{2}", structName, STRUCT_MEMBER_SEPARATOR_CHAR, memberName);
                //simple variable or array
                parsedData.Add(row[1], row);
            }
            else
            {

                string memberStructure = string.Format("{0} {1}\"{2}\"", dataType, memberName, comment);
                //it is necessary to create a second string, for comparison where comments are not inserted
                string memberStructureComp = string.Format("{0} {1}", dataType, memberName);

                //check if the name of the structure is inside the map
                if (m_mapStruct.ContainsKey(structName))
                {
                    List<string> itemList = m_mapStruct[structName];
                    //check if the member of the structure is on the list of members
                    if (!itemList.Exists(x => x.Contains(memberStructureComp)))
                        itemList.Add(memberStructure);
                }
            }
        }

        private bool IsVarDeclarationStruct(string varDeclaration)
        {
            return (varDeclaration.IndexOf(STRUCT_MEMBER_SEPARATOR_CHAR) >= 0);
        }

        private bool IsBigArrayOrMultiDimensionaArray(string dataType, OmronEthernetIPStationSettings station)
        {
            List<ArrayInfo> arraySize = new List<ArrayInfo>() { new ArrayInfo(), new ArrayInfo(), new ArrayInfo() };            
            string dummy = String.Empty;
            
            //if (IsArray(dataType, ref arraySize, ref arrayType, ref arrayDimNum))
            if (GetArrayDimL5K(ref dataType, ref dummy, ref arraySize, out uint arrayDimNum))
            {
                // multidimensional array case
                if (arrayDimNum > 1)
                    return true;

                //else if (IsBigArray(row[2])nElemSize* nArrayDim > OmronEthernetIPProtocol.MAX_SEGMENT_DATA_SIZE && arrayType == ImportTypes.Standard) {
                uint nFieldSize = 0;
                string nMovType = "";

                if (GetVarTypeAndSizeL5K(dataType, ref nFieldSize, ref nMovType) == ImportTypes.Standard)
                {

                    long nArrayDim = arraySize.Sum(a => a.Size);

                    //return (nFieldSize * nArrayDim > OmronEthernetIPProtocol.MAX_SEGMENT_DATA_SIZE);
                    // calculate data's area size inside frame 
                    return (nFieldSize * nArrayDim > OmronEthernetIPProtocol.getReadFrameDataSize(station.PlcType));
                }
            }

            return false;
        }

        /// <summary>
        /// Crate struct tree --> is father node, or father of father (and so on) node don't exist, create it. 
        /// Manage also struct var creation
        /// </summary>
        private void CreateStructTree(string[] row, ref Dictionary<string, string[]> parsedData, out string structName,out string memberName, OmronEthernetIPStationSettings station)
        {
            int LevelNr = 0;
            bool LastLevel;

            structName = string.Empty;
            memberName = string.Empty;

            // split struct
            MatchCollection StructInfo = SplitStructLevel(row[1]);            
            do
            {
                GetStructLevel(StructInfo, row[1], LevelNr, out structName, out memberName, out LastLevel);
                if (LevelNr==0)
                {
                    if (!string.IsNullOrEmpty(structName) && !m_mapStruct.ContainsKey(structName))
                    {
                        //add struct definition
                        m_mapStruct.Add(structName, new List<string> { });

                        // and new variable
                        string[] rowTemp = new string[9]; // row.Length];
                        Array.Copy(row, rowTemp, row.Length);
                        rowTemp[2] = "STRUCT";
                        rowTemp[7] = structName;
                        rowTemp[1] = structName;
                        parsedData.Add(structName, rowTemp);
                    }
                }
                else
                {
                    // last level of struct declaration contain real member declaration (ex bool, int data type) --> manage outside this function
                    if (LastLevel)
                        return;

                    if (!string.IsNullOrEmpty(structName) && !m_mapStruct.ContainsKey(structName))
                    {
                        // get struct parent's level info
                        GetStructLevel(StructInfo, row[1], LevelNr - 1, out string StructNamePreviousLevel, out string Dummy, out LastLevel);

                        //add struct definition
                        m_mapStruct.Add(structName, new List<string> { });

                        // if array of struct, move data to root level and create a new variable
                        if (structName.EndsWith("]")) {
                            // and new variable
                            string[] rowTemp = new string[9]; // row.Length];
                            Array.Copy(row, rowTemp, row.Length);
                            rowTemp[2] = "STRUCT";
                            rowTemp[7] = structName;
                            rowTemp[1] = structName;
                            parsedData.Add(structName, rowTemp);
                        }
                        else  
                        {
                            AddMemberToStruct(StructNamePreviousLevel, structName, memberName, string.Empty, ref parsedData, row, station);
                        }
                    }
                }
                LevelNr++;

            } while (!String.IsNullOrEmpty(structName));
        }

        void ParseVariable(string line, ref Dictionary<string, string[]> parsedData, OmronEthernetIPStationSettings station)
        {
            string[] row = line.Split('\t'); 

            if (row.Count() < 7)
            {
                return;
            }
            // The 2nd field ("NAME") must not be an empty string
            if (String.IsNullOrWhiteSpace(row[1]))
            {
                return;
            }

            // force var type field to UPPER for var type matching
            //Array rowLocal fields
            //[0] HOST
            //[1] NAME
            //[2] DATATYPE
            //[3] ADDRESS
            //[4] COMMENT
            //[5] TAGLINK
            //[6] RW
            //[7] POU
            //[8] Name structure
            //Composition of the structure member string

            row[1] = row[1].Trim();
            row[2] = row[2].ToUpper();
                             
            // is a struct ?
            if (IsVarDeclarationStruct(row[1]))
            {
                string StructName;
                string MemberName;

                // struct var creation is manage inside
                CreateStructTree(row, ref parsedData, out StructName, out MemberName, station);

                if (!string.IsNullOrEmpty(StructName))
                    AddMemberToStruct(StructName, row[2], MemberName, row[4], ref parsedData, row, station);
            }
            else
            {
                //simple variable or array
                parsedData.Add(row[1],row);                
            }
        }

        bool IsArray(String tagType, ref List<ArrayInfo> arrayDim, ref String arrayType, ref uint numOfDim)
        {
            numOfDim = 0;            
            bool isArray = false;
            // First dimension
            string stringToBeParsed = tagType;
            if(String.IsNullOrWhiteSpace(stringToBeParsed))
            {
                return (isArray);
            }
            int parsingIndex = 0;
            int IndexOpenSquareBracket = stringToBeParsed.IndexOf('[');
            int IndexCloseSquareBracket = stringToBeParsed.IndexOf(']');
            int IndexPoints = stringToBeParsed.IndexOf("..");
            int IndexComma = stringToBeParsed.IndexOf(',');
            if((IndexComma > 0) && (IndexComma < IndexCloseSquareBracket))
            {
                IndexCloseSquareBracket = IndexComma;
            }
            if ((IndexOpenSquareBracket > 0) && (IndexCloseSquareBracket > 0) && (IndexPoints > 0) &&
               (IndexCloseSquareBracket > (IndexPoints + 2)) && (IndexPoints > (IndexOpenSquareBracket + 1)))
            {
                string minInd = stringToBeParsed.Substring(IndexOpenSquareBracket + 1, IndexPoints - IndexOpenSquareBracket - 1);
                string maxInd = stringToBeParsed.Substring(IndexPoints + 2, IndexCloseSquareBracket - IndexPoints - 2);
                arrayDim[0].MinIndex = Convert.ToUInt32(minInd);
                arrayDim[0].MaxIndex = Convert.ToUInt32(maxInd);
                arrayDim[0].CalculateSize();
                if (arrayDim[0].MaxIndex >= arrayDim[0].MinIndex)
                {
                    isArray = true;
                    parsingIndex += IndexCloseSquareBracket + 1;
                    arrayType = stringToBeParsed.Substring(0, IndexOpenSquareBracket);
                    numOfDim++;

                    // Second dimension
                    stringToBeParsed = tagType.Substring(parsingIndex);
                    if (String.IsNullOrWhiteSpace(stringToBeParsed))
                    {
                        return (isArray);
                    }
                    IndexCloseSquareBracket = stringToBeParsed.IndexOf(']');
                    IndexPoints = stringToBeParsed.IndexOf("..");
                    IndexComma = stringToBeParsed.IndexOf(',');
                    if ((IndexComma > 0) && (IndexComma < IndexCloseSquareBracket))
                    {
                        IndexCloseSquareBracket = IndexComma;
                    }
                    if ((IndexCloseSquareBracket > 0) && (IndexPoints > 0) &&
                       (IndexCloseSquareBracket > (IndexPoints + 2)))
                    {
                        minInd = stringToBeParsed.Substring(0, IndexPoints);
                        maxInd = stringToBeParsed.Substring(IndexPoints + 2, IndexCloseSquareBracket - IndexPoints - 2);
                        arrayDim[1].MinIndex = Convert.ToUInt32(minInd);
                        arrayDim[1].MaxIndex = Convert.ToUInt32(maxInd);
                        arrayDim[1].CalculateSize();
                        if (arrayDim[1].MaxIndex >= arrayDim[1].MinIndex)
                        {
                            parsingIndex += IndexCloseSquareBracket + 1;
                            numOfDim++;

                            // Third dimension
                            stringToBeParsed = tagType.Substring(parsingIndex);
                            if (String.IsNullOrWhiteSpace(stringToBeParsed))
                            {
                                return (isArray);
                            }
                            IndexCloseSquareBracket = stringToBeParsed.IndexOf(']');
                            IndexPoints = stringToBeParsed.IndexOf("..");
                            if ((IndexCloseSquareBracket > 0) && (IndexPoints > 0) &&
                               (IndexCloseSquareBracket > (IndexPoints + 2)))
                            {
                                minInd = stringToBeParsed.Substring(0, IndexPoints);
                                maxInd = stringToBeParsed.Substring(IndexPoints + 2, IndexCloseSquareBracket - IndexPoints - 2);
                                arrayDim[2].MinIndex = Convert.ToUInt32(minInd);
                                arrayDim[2].MaxIndex = Convert.ToUInt32(maxInd);
                                arrayDim[2].CalculateSize();
                                if (arrayDim[2].MaxIndex >= arrayDim[2].MinIndex)
                                {
                                    numOfDim++;
                                }
                            }
                        }
                    }
                }
            }

            return (isArray);
        }

        ImportTypes GetVarTypeAndSizeTXT(string inSzType, ref uint nVarSize, ref String nType,
                                         ref List<ArrayInfo> arrayDim, ref string arrayType)
        {            
            ImportTypes exit = ImportTypes.Unknown;
            inSzType = inSzType.Trim();
            string szType = inSzType;
            szType = szType.ToUpper();
            nVarSize = 0;
            nType = "";
            uint arrayDimNum = 0;            
            arrayType = String.Empty;
            uint arraySize = 0;
            if (IsArray(szType, ref arrayDim, ref arrayType, ref arrayDimNum))
            {
                szType = arrayType;
                arraySize += arrayDim[0].Size;
                if(arrayDimNum > 1)
                {
                    arraySize += arrayDim[1].Size;
                    if (arrayDimNum > 2)
                    {
                        arraySize += arrayDim[2].Size;
                    }
                }
            }

            if (szType.Length == 0)
            {
                return (exit);
            }
            if (szType == "BOOL")
            {
                nType = "Boolean";
                nVarSize = 1;
                exit = ImportTypes.Standard;
            }
            else if ((szType == "BYTE") || ((szType == "USINT")))
            {
                nType = "Byte";
                nVarSize = 1;
                exit = ImportTypes.Standard;
            }
            else if (szType == "SINT")
            {
                nType = "SByte";
                nVarSize = 1;
                exit = ImportTypes.Standard;
            }
            else if ((szType == "WORD") || (szType == "UINT"))
            {
                nType = "UInt16";
                nVarSize = 2;
                exit = ImportTypes.Standard;
            }
            else if ((szType == "DWORD") || (szType == "UDINT"))
            {
                nType = "UInt32";
                nVarSize = 4;
                exit = ImportTypes.Standard;
            }
            else if (szType == "INT")
            {
                nType = "Int16";
                nVarSize = 2;
                exit = ImportTypes.Standard;
            }
            else if (szType == "DINT")
            {
                nType = "Int32";
                nVarSize = 4;
                exit = ImportTypes.Standard;
            }
            else if (szType == "REAL")
            {
                nType = "Float";
                nVarSize = 4;
                exit = ImportTypes.Standard;
            }
            else if (szType == "LREAL")
            {
                nType = "Double";
                nVarSize = 8;
                exit = ImportTypes.Standard;
            }
            else if (szType.Contains("STRING("))
            {
                nType = "String";
                nVarSize = OmronEthernetIPProtocol.MAX_STRING_LENGTH;
                int bracketOpenIndex = szType.IndexOf('(');
                int bracketCloseIndex = szType.IndexOf(')');
                if( (bracketOpenIndex >= 0) && (bracketCloseIndex >= 0) && (bracketCloseIndex > (bracketOpenIndex + 1)))
                {
                    string numOfChar = szType.Substring(bracketOpenIndex + 1, bracketCloseIndex - bracketOpenIndex - 1);
                    nVarSize = Convert.ToUInt32(numOfChar);
                    if(nVarSize > OmronEthernetIPProtocol.MAX_STRING_LENGTH)
                    {
                        nVarSize = OmronEthernetIPProtocol.MAX_STRING_LENGTH;
                    }
                }
                exit = ImportTypes.Standard;
            }
            else if ((szType == "LWORD") ||
                      (szType == "ULINT"))
            {
                nType = "UInt64";
                nVarSize = 8;
                exit = ImportTypes.Standard;
            }
            else if (szType == "LINT")
            {
                nType = "Int64";
                nVarSize = 8;
                exit = ImportTypes.Standard;
            }
            else if (szType == "STRUCT")
            {
                nType = "Struct";
                nVarSize = 1;
                exit = ImportTypes.Struct;
            }

            if (!string.IsNullOrEmpty(nType) && arraySize > 0)
            {
                exit = ImportTypes.Array;
            }

            return (exit);
        }

        void AddStandardVar(String szNamePrefix, String szVarName,
                                         String szAddressPrefix, String szAddress,
                                         String description, String szVarType, String nMovType,
                                         uint nVarSize, int linkType, int parentId = -1, ImportData inRootItem = null)
        {


            var IVar = importDataModel.addImportData();
            ((ImportDataOmronEthernetIP)IVar).PreName = szNamePrefix;
            IVar.Name = szVarName;
            ((ImportDataOmronEthernetIP)IVar).ElemType = szVarType;
            ((ImportDataOmronEthernetIP)IVar).ElemTypeView = szVarType;
            ((ImportDataOmronEthernetIP)IVar).Type = (DataType)Enum.Parse(typeof(DataType), nMovType);
            ((ImportDataOmronEthernetIP)IVar).szType = "";
            ((ImportDataOmronEthernetIP)IVar).Size = nVarSize;
            IVar.Address = szAddressPrefix + szAddress;
            IVar.Description = description;
            IVar.parentId = parentId;
            IVar.Id = m_lGlobalID++;
            IVar.ArrayDimension = 0;
            ((ImportDataOmronEthernetIP)IVar).LinkType = linkType;
            ((ImportDataOmronEthernetIP)IVar).ImportType = ImportTypes.Standard;

            AddTreeItem(IVar, inRootItem);

        }

        void AddStandardVarTxt(String szNamePrefix, String szVarName,
                               String szAddressPrefix, String szAddress,
                               String description, String szVarType, String nMovType,
                               uint nVarSize, int linkType, int parentId = -1, ImportData inRootItem = null)
        {
            var IVar = importDataModel.addImportData();
            ((ImportDataOmronEthernetIP)IVar).PreName = szNamePrefix;
            IVar.Name = szVarName;
            ((ImportDataOmronEthernetIP)IVar).ElemType = szVarType;
            ((ImportDataOmronEthernetIP)IVar).ElemTypeView = szVarType;
            int bracketIndex = ((ImportDataOmronEthernetIP)IVar).ElemType.IndexOf('(');
            if (bracketIndex > 0)
            {
                ((ImportDataOmronEthernetIP)IVar).ElemType = szVarType.Substring(0, bracketIndex);
                ((ImportDataOmronEthernetIP)IVar).ElemTypeView = szVarType;
            }
            ((ImportDataOmronEthernetIP)IVar).Type = (DataType)Enum.Parse(typeof(DataType), nMovType);
            IVar.szType = "";
            ((ImportDataOmronEthernetIP)IVar).Size = nVarSize;
            IVar.Address = szAddressPrefix + szAddress;
            if(((ImportDataOmronEthernetIP)IVar).Type == DataType.String)
            {
                IVar.Address += String.Format(":{0}", nVarSize);
            }
            IVar.Description = description;
            IVar.parentId = parentId;
            IVar.Id = m_lGlobalID++;
            IVar.ArrayDimension = 0;
            ((ImportDataOmronEthernetIP)IVar).ImportType = ImportTypes.Standard;
            ((ImportDataOmronEthernetIP)IVar).LinkType = linkType;

            AddTreeItem(IVar, inRootItem);
        }

        void AddArrayTxt(String szNamePrefix, String szArrayName, String szAddressPrefix,
                      String szArrayAddress, String description, String szElemType, uint nElemSize,
                      List<ArrayInfo> arrayDim,
                      ImportTypes arrayType,
                      String nMovType, int linkType, OmronEthernetIPStationSettings station, int parentId = -1, ImportData inRootItem = null)
        {

            if (arrayDim[0].Size == 0)
            {
                return;
            }

            String szName, szName1;
            String szAddress, szAddress1;
            if (arrayDim[1].Size == 0)
            {
                AddArray1DimTxt(ref szNamePrefix, ref szArrayName, ref szAddressPrefix, ref szArrayAddress, ref description,
                    ref szElemType, nElemSize, arrayDim[0].Size, 0, arrayDim[0].MinIndex, arrayType, nMovType, linkType, station, parentId, inRootItem);
            }
            else if (arrayDim[2].Size == 0)
            {
                for (uint nIdx = arrayDim[0].MinIndex; nIdx < (arrayDim[0].Size + arrayDim[0].MinIndex); nIdx++)
                {
                    szName = string.Format("{0}[{1}", szArrayName, nIdx);
                    szAddress = string.Format("{0}[{1}", szArrayAddress, nIdx);
                    AddArray1DimTxt(ref szNamePrefix, ref szName, ref szAddressPrefix, ref szAddress, ref description,
                        ref szElemType, nElemSize, arrayDim[1].Size, 1, arrayDim[1].MinIndex, arrayType, nMovType, linkType, station, parentId, inRootItem);
                }
            }
            else
            {
                for (uint nIdx2 = arrayDim[0].MinIndex; nIdx2 < (arrayDim[0].Size + arrayDim[0].MinIndex); nIdx2++)
                {
                    for (uint nIdx = arrayDim[1].MinIndex; nIdx < (arrayDim[1].Size + arrayDim[1].MinIndex);
                        nIdx++)
                    {
                        szName1 = string.Format("{0}[{1},{2}", szArrayName, nIdx2, nIdx);
                        szAddress1 = string.Format("{0}[{1},{2}", szArrayAddress, nIdx2, nIdx);
                        AddArray1DimTxt(ref szNamePrefix, ref szName1, ref szAddressPrefix, ref szAddress1,
                            ref description, ref szElemType, nElemSize, arrayDim[2].Size,
                             2, arrayDim[2].MinIndex, arrayType, nMovType, linkType, station, parentId, inRootItem);
                    }
                }
            }
        }
        bool AddStructTxt(String szNamePrefix, String szVarName, String szAddressPrefix, String szAddress,
            String description, String szVarType, int linkType, OmronEthernetIPStationSettings station, int parentId = -1, ImportData inRootItem = null)
        {
            if (!StructFoundL5K(szVarType))
                return false;
            if (m_mapStruct[szVarType].Count == 0)
                return false;


            String szStructNamePrefix = szNamePrefix + szVarName + ".";
            String szStructAddressPrefix = /*szAddressPrefix +*/ szAddress + ".";

            int lParentID = parentId;

            uint nFieldSize = 0;
            
            if (inRootItem == null)
                lParentID = -1;

            ImportData rootItem = importDataModel.addImportData();
            rootItem.parentId = lParentID;
            lParentID = m_lGlobalID++;
            rootItem.Id = lParentID;
            ((ImportDataOmronEthernetIP)rootItem).PreName = szNamePrefix;
            rootItem.Name = szVarName;
            rootItem.szType = "STRUCT";
            ((ImportDataOmronEthernetIP)rootItem).ElemType = szVarType;
            ((ImportDataOmronEthernetIP)rootItem).ElemTypeView = szVarType;
            ((ImportDataOmronEthernetIP)rootItem).Type = DataType.Boolean;
            rootItem.Address = /*szAddressPrefix +*/ szAddress;
            rootItem.Description = description;
            ((ImportDataOmronEthernetIP)rootItem).Size = nFieldSize;
            rootItem.ArrayDimension = 0;
            ((ImportDataOmronEthernetIP)rootItem).LinkType = linkType;

            String szFieldName = string.Empty;
            String szFieldType;
            String szFieldDescription;
            string nMovType = "";
            int nIndex = 0;
            int nIndex2 = 0;
            List<ArrayInfo> arraySize = new List<ArrayInfo>() { new ArrayInfo(), new ArrayInfo(), new ArrayInfo() };
            ImportTypes nFieldType = ImportTypes.Unknown;
            ImportTypes elementType = ImportTypes.Unknown;

            bool isFirst = true;

            foreach (string itemStructValue in m_mapStruct[szVarType])
            {

                nIndex = itemStructValue.IndexOf(' ');
                if (nIndex <= 0)
                {
                    continue;
                }
                if (itemStructValue.Length < (nIndex + 2))
                {
                    continue;
                }
                nIndex2 = itemStructValue.IndexOf("\"");
                szFieldType = itemStructValue.Substring(0, nIndex).Trim();                
                if (szFieldType.Length == 0)
                {
                    continue;
                }
                if (nIndex2 < (nIndex + 2))
                {
                    szFieldName = itemStructValue.Substring(nIndex + 1).Trim();
                    szFieldDescription = "";
                }
                else
                {
                    szFieldName = itemStructValue.Substring(nIndex + 1, nIndex2 - nIndex - 1).Trim();
                    szFieldDescription = itemStructValue.Substring(nIndex2).Trim(new char[] { '\"' });
                }
                if (szFieldName.Length == 0)
                {
                    continue;
                }

                //if (szFieldType.IndexOf("]") == szFieldType.Length - 1)
                //{
                //    int Index1 = szFieldType.IndexOf("[");
                //    int Index2 = szFieldType.IndexOf("]");
                //    string dimension = szFieldType.Substring(Index1 + 1, Index2 - Index1 - 1).Trim(new char[] { '\r', '\n', '\t', ' ' });
                //    string[] arrayValue = dimension.Split('.');
                //    int numItem = 0;
                //    int numTmp = 0;
                //    for (int index = 0; index < arrayValue.Length; index++)
                //    {
                //        if (!int.TryParse(arrayValue[index], out numTmp))
                //        {
                //            continue;
                //        }
                //        if(index == 0)
                //        {

                //            if (numTmp == 0)
                //            {
                //                //numItem = 1;
                //                numTmp = 0;
                //            }
                //            else
                //            {
                //                //numItem = numTmp * -1;
                //                numTmp = (numTmp * -1)+1;
                //            }
                //        }
                //        numItem = numTmp + numItem;
                //    }
                //    dimension = "[" + numItem.ToString() + "]";
                //    szFieldName += dimension;
                //    szFieldType = szFieldType.Substring(0, Index1);
                //}                

                //elementType = GetVarTypeAndSizeL5K(szFieldType, ref nFieldSize, ref nMovType);

                //szFieldName
                if (GetArrayDimL5K(ref szFieldType, ref szFieldName, ref arraySize, out uint arrayDimNum))
                {
                    nFieldType = ImportTypes.Array;                    

                    elementType = GetVarTypeAndSizeL5K(szFieldType, ref nFieldSize, ref nMovType);
                }
                else
                {
                    elementType = GetVarTypeAndSizeL5K(szFieldType, ref nFieldSize, ref nMovType);

                    nFieldType = elementType;
                }                

                String elemDescription = (!string.IsNullOrEmpty(description)) ? description /*+ ", " + szFieldDescription */: "";

                // Add the field to the dialog list
                switch (nFieldType)
                {
                    // Variable of standard type
                    case ImportTypes.Standard:
                        //if (nMovType != "String")
                        //{
                        AddStructRoot(inRootItem, rootItem, ref isFirst, rootItem.Address /*szAddress*/, szVarType);
                        AddStandardVar(szStructNamePrefix, szFieldName, "", szStructAddressPrefix + szFieldName,
                            elemDescription, szFieldType, nMovType,
                            nFieldSize, linkType, rootItem.Id, rootItem);
                        //}
                        //else
                        //{
                        //AddStandardVar("", szStructNamePrefix + szFieldName, "", szStructAddressPrefix + szFieldName,
                        //    elemDescription, szFieldType, nMovType, nFieldSize);
                        //}
                        break;
                    // Array
                    case ImportTypes.Array:

                        if (elementType == ImportTypes.Standard)
                        {
                            AddStructRoot(inRootItem, rootItem, ref isFirst, szAddress, szVarType);
                            AddArray(szStructNamePrefix, szFieldName, szStructAddressPrefix, szFieldName,
                                 elemDescription, szFieldType, nFieldSize,
                                 arraySize, elementType, nMovType, linkType, station, lParentID, rootItem);

                        }
                        else
                        {
                            AddArray("", szStructNamePrefix + szFieldName, "", szStructAddressPrefix + szFieldName,
                                 elemDescription, szFieldType, nFieldSize, arraySize, elementType, nMovType,linkType, station);
                        }
                        break;
                    // Structure 
                    case ImportTypes.Struct:
                        AddStructRoot(inRootItem, rootItem, ref isFirst, szAddress, szVarType);
                        AddStruct(szStructNamePrefix, szFieldName, szStructAddressPrefix, szFieldName, elemDescription,
                                     szFieldType, linkType, station, lParentID, rootItem);

                        break;
                }
            }


            return true;
        }
        bool AddStruct(String szNamePrefix, String szVarName, String szAddressPrefix, String szAddress,
            String description, String szVarType, int linkType, OmronEthernetIPStationSettings station, int parentId = -1, ImportData inRootItem = null)
        {
            if (!StructFoundL5K(szVarType))
                return false;
            if (m_mapStruct[szVarType].Count == 0)
                return false;


            String szStructNamePrefix = szNamePrefix + szVarName + ".";
            String szStructAddressPrefix = szAddressPrefix + szAddress + ".";

            int lParentID = parentId;

            uint nFieldSize = 0;
            
            if (inRootItem == null)
                lParentID = -1;

            ImportData rootItem = importDataModel.addImportData();
            rootItem.parentId = lParentID;
            lParentID = m_lGlobalID++;
            rootItem.Id = lParentID;
            ((ImportDataOmronEthernetIP)rootItem).PreName = szNamePrefix;
            rootItem.Name = szVarName;
            rootItem.szType = "STRUCT";
            ((ImportDataOmronEthernetIP)rootItem).ElemType = szVarType;
            ((ImportDataOmronEthernetIP)rootItem).ElemTypeView = szVarType;
            ((ImportDataOmronEthernetIP)rootItem).Type = DataType.Boolean;
            rootItem.Address = szAddressPrefix + szAddress;
            rootItem.Description = description;
            ((ImportDataOmronEthernetIP)rootItem).Size = nFieldSize;
            rootItem.ArrayDimension = 0;
            ((ImportDataOmronEthernetIP)rootItem).LinkType = linkType;
            
            String szFieldName = string.Empty;
            String szFieldType;            
            String szFieldDescription;
            string nMovType = "";
            int nIndex = 0;
            int nIndex2 = 0;
            List<ArrayInfo> arraySize = new List<ArrayInfo>() { new ArrayInfo(), new ArrayInfo(), new ArrayInfo() };
            ImportTypes nFieldType = ImportTypes.Unknown;
            ImportTypes elementType = ImportTypes.Unknown;

            bool isFirst = true;

            foreach (string itemStructValue in m_mapStruct[szVarType])
            {

                nIndex = itemStructValue.IndexOf(' ');
                if (nIndex <= 0)
                {
                    continue;
                }
                if (itemStructValue.Length < (nIndex + 2))
                {
                    continue;
                }
                nIndex2 = itemStructValue.IndexOf("\"");
                szFieldType = itemStructValue.Substring(0, nIndex).Trim();                
                if (szFieldType.Length == 0)
                {
                    continue;
                }
                if (nIndex2 < (nIndex + 2))
                {
                    szFieldName = itemStructValue.Substring(nIndex + 1).Trim();
                    szFieldDescription = "";
                }
                else
                {
                    szFieldName = itemStructValue.Substring(nIndex + 1, nIndex2 - nIndex - 1).Trim();
                    szFieldDescription = itemStructValue.Substring(nIndex2).Trim(new char[] { '\"' });
                }
                if (szFieldName.Length == 0)
                {
                    continue;
                }
                
                //if (szFieldType.IndexOf("]") == szFieldType.Length - 1)
                //{
                //    int Index1 = szFieldType.IndexOf("[");
                //    int Index2 = szFieldType.IndexOf("]");
                //    string dimension = szFieldType.Substring(Index1 + 1, Index2 - Index1 - 1).Trim(new char[] { '\r', '\n', '\t', ' ' });
                //    string[] arrayValue = dimension.Split('.');
                //    int numItem = 0;
                //    int numTmp = 0;
                //    for (int index = 0; index < arrayValue.Length; index++)
                //    {
                //        if (!int.TryParse(arrayValue[index], out numTmp))
                //        {
                //            continue;
                //        }
                //        if (numTmp == 0)
                //        {
                //            //numItem = 1;
                //            numTmp = 0;
                //        }
                //        else
                //        {
                //            //numItem = numTmp * -1;
                //            numTmp = (numTmp * -1) + 1;
                //        }
                //        numItem = numTmp + numItem;
                //    }
                //    dimension = "[" + numItem.ToString() + "]";
                //    szFieldName += dimension;
                //    szFieldType = szFieldType.Substring(0, Index1);
                //}

                //elementType = GetVarTypeAndSizeL5K(szFieldType, ref nFieldSize, ref nMovType);

                if (GetArrayDimL5K(ref szFieldType, ref szFieldName, ref arraySize, out uint arrayDimNum))
                {
                    nFieldType = ImportTypes.Array;                    
                    elementType = GetVarTypeAndSizeL5K(szFieldType, ref nFieldSize, ref nMovType);
                }
                else
                {
                    elementType = GetVarTypeAndSizeL5K(szFieldType, ref nFieldSize, ref nMovType);
                    nFieldType = elementType;
                }                

                String elemDescription = (!string.IsNullOrEmpty(description)) ? description /*+ ", " + szFieldDescription*/ : "";

                // Add the field to the dialog list
                switch (nFieldType)
                {
                    // Variable of standard type
                    case ImportTypes.Standard:
                        //if (nMovType != "String")
                        //{
                            AddStructRoot(inRootItem, rootItem, ref isFirst, rootItem.Address /*szAddress*/, szVarType);
                            AddStandardVar(szStructNamePrefix , szFieldName, "", szStructAddressPrefix + szFieldName,
                                elemDescription, szFieldType, nMovType,
                                nFieldSize, linkType, rootItem.Id, rootItem);
                        //}
                        //else
                        //{
                        //    AddStandardVar("", szStructNamePrefix + szFieldName, "", szStructAddressPrefix + szFieldName,
                        //        elemDescription, szFieldType, nMovType, nFieldSize);

                        //}
                        break;
                    // Array
                    case ImportTypes.Array:

                        if (elementType == ImportTypes.Standard)
                        {
                            AddStructRoot(inRootItem, rootItem, ref isFirst, szAddress, szVarType);
                            AddArray(szStructNamePrefix, szFieldName, szStructAddressPrefix, szFieldName,
                                 elemDescription, szFieldType, nFieldSize,
                                 arraySize, elementType, nMovType, linkType, station, lParentID, rootItem);

                        }
                        else
                        {
                            AddArray("", szStructNamePrefix + szFieldName, "", szStructAddressPrefix + szFieldName,
                                 elemDescription, szFieldType, nFieldSize, arraySize, elementType, nMovType, linkType, station);
                        }
                        break;
                    // Structure 
                    case ImportTypes.Struct:
                        AddStructRoot(inRootItem, rootItem, ref isFirst, szAddress, szVarType);
                        AddStruct(szStructNamePrefix, szFieldName, szStructAddressPrefix, szFieldName, elemDescription,
                                     szFieldType, linkType, station, lParentID, rootItem) ;

                        break;
                }
            }


            return true;
        }

        private void AddStructRoot(ImportData inRootItem, ImportData rootItem, ref bool isFirst, string szAddress, string szVarType)
        {
            if (isFirst)
            {
                isFirst = false;
                rootItem.Address = szAddress;
                ((ImportDataOmronEthernetIP)rootItem).ImportType = ImportTypes.Struct;
                rootItem.szType = "STRUCT";
                if (rootItem.parentId < 0)
                {
                    AddTreeItem(rootItem);
                }
                else
                {
                    AddTreeItem(rootItem, inRootItem);
                }
            }
        }

        void AddArray(String szNamePrefix, String szArrayName, String szAddressPrefix,
                      String szArrayAddress, String description, String szElemType, uint nElemSize,
                      List<ArrayInfo> arrayDim, ImportTypes arrayType,
                      String nMovType, int linkType, OmronEthernetIPStationSettings station, int parentId = -1, ImportData inRootItem = null)
        {

            if (arrayDim[0].Size == 0)
            {
                return;
            }

            String szName, szName1;
            String szAddress, szAddress1;
            if (arrayDim[1].Size == 0)
            {
                AddArray1Dim(ref szNamePrefix, ref szArrayName, ref szAddressPrefix, ref szArrayAddress, ref description,
                    ref szElemType, nElemSize, arrayDim[0], 0, arrayType, nMovType, linkType, station, parentId, inRootItem);
            }
            else if (arrayDim[2].Size == 0)
            {
                for (uint nIdx = 0; nIdx < arrayDim[0].Size; nIdx++)
                {
                    szName = string.Format("{0}[{1}", szArrayName, nIdx);
                    szAddress = string.Format("{0}[{1}", szArrayAddress, nIdx);
                    AddArray1Dim(ref szNamePrefix, ref szName, ref szAddressPrefix, ref szAddress, ref description,
                        ref szElemType, nElemSize, arrayDim[1], 1, arrayType, nMovType, linkType, station, parentId, inRootItem);
                }
            }
            else
            {
                for (uint nIdx2 = 0; nIdx2 < arrayDim[0].Size; nIdx2++)
                {
                    for (uint nIdx = 0; nIdx < arrayDim[1].Size;
                        nIdx++)
                    {
                        szName1 = string.Format("{0}[{1},{2}", szArrayName, nIdx2, nIdx);
                        szAddress1 = string.Format("{0}[{1},{2}", szArrayAddress, nIdx2, nIdx);
                        AddArray1Dim(ref szNamePrefix, ref szName1, ref szAddressPrefix, ref szAddress1,
                            ref description, ref szElemType, nElemSize, arrayDim[2],
                             2, arrayType, nMovType,linkType, station, parentId, inRootItem);
                    }
                }
            }
        }

        void AddArray1DimTxt(ref String szNamePrefix, ref String szArrayName,
                             ref String szAddressPrefix, ref String szAddress, ref String description,
                             ref String szElemType, uint nElemSize, uint nArrayDim,
                             uint nDimIndex, uint dimStart, ImportTypes arrayType, String nMovType, int linkType,
                             OmronEthernetIPStationSettings station, int parentId = -1, ImportData inRootItem = null)
        {
            // calculate data's area size inside frame 
            ushort frameFreeSpace = OmronEthernetIPProtocol.getReadFrameDataSize(station.PlcType);

            //if (nElemSize * nArrayDim > OmronEthernetIPProtocol.MAX_SEGMENT_DATA_SIZE && arrayType == ImportTypes.Standard)
            if (nElemSize * nArrayDim > frameFreeSpace && arrayType == ImportTypes.Standard)
            {
                uint nArrayBlockSize = frameFreeSpace / nElemSize;
                uint nArrayBlocks = (nArrayDim + nArrayBlockSize - 1) / nArrayBlockSize;
                uint nArrayBlock = 1;
                uint nArrayStart = 0;
                while (nArrayStart < nArrayDim)
                {
                    if (nArrayStart + nArrayBlockSize > nArrayDim)
                        nArrayBlockSize = nArrayDim - nArrayStart;
                    AddArray1DimDisplaceTxt(ref szNamePrefix, ref szArrayName,
                        ref szAddressPrefix, ref szAddress, ref description,
                        ref szElemType, nElemSize, nArrayBlockSize,
                        nDimIndex, nArrayStart + dimStart, nArrayBlock, nArrayBlocks, arrayType, nMovType, linkType, station, parentId, inRootItem);
                    nArrayBlock++;
                    nArrayStart += nArrayBlockSize;
                }

                return;
            }
            else
            {
                AddArray1DimDisplaceTxt(ref szNamePrefix, ref szArrayName,
                    ref szAddressPrefix, ref szAddress, ref description,
                    ref szElemType, nElemSize, nArrayDim,
                    nDimIndex, dimStart, 1, 1, arrayType, nMovType, linkType, station, parentId, inRootItem);
            }
        }
      
        void AddArray1Dim(ref String szNamePrefix, ref String szArrayName,
                          ref String szAddressPrefix, ref String szAddress, ref String description,
                          ref String szElemType, uint nElemSize, ArrayInfo arrayDim,
                          uint nDimIndex, ImportTypes arrayType, String nMovType, int linkType, OmronEthernetIPStationSettings station, int parentId = -1, ImportData inRootItem = null)
        {
            // calculate data's area size inside frame 
            ushort frameFreeSpace = OmronEthernetIPProtocol.getReadFrameDataSize(station.PlcType);

            if ((nElemSize * arrayDim.Size) > frameFreeSpace && arrayType == ImportTypes.Standard)
            {
                uint nArrayBlockSize = frameFreeSpace / nElemSize;
                uint nArrayBlocks = (arrayDim.Size + nArrayBlockSize - 1) / nArrayBlockSize;
                uint nArrayBlock = 1;
                uint nArrayStart = 0;
                while (nArrayStart < arrayDim.Size)
                {
                    if (nArrayStart + nArrayBlockSize > arrayDim.Size)
                        nArrayBlockSize = arrayDim.Size - nArrayStart;
                    AddArray1DimDisplace(ref szNamePrefix, ref szArrayName,
                        ref szAddressPrefix, ref szAddress, ref description,
                        ref szElemType, nElemSize, nArrayBlockSize,
                        nDimIndex, nArrayStart, nArrayBlock, nArrayBlocks, arrayType, nMovType, linkType, station, parentId, inRootItem);
                    nArrayBlock++;
                    nArrayStart += nArrayBlockSize;
                }

                return;
            }
            else
            {
                AddArray1DimDisplace(ref szNamePrefix, ref szArrayName,
                    ref szAddressPrefix, ref szAddress, ref description,
                    ref szElemType, nElemSize, arrayDim.Size,
                    nDimIndex, arrayDim.MinIndex, 1, 1, arrayType, nMovType, linkType, station, parentId, inRootItem);
            }
        }

        void AddArray1DimDisplaceTxt(ref String szNamePrefix, ref String szArrayName,
                          ref String szAddressPrefix, ref String szAddress, ref String description,
                          ref String szElemType, uint nElemSize, uint nArrayDim,
                          uint nDimIndex, uint nArrayStart, uint nArrayBlock, uint nArrayBlocks, ImportTypes arrayType,
                          String nMovType, int linkType, OmronEthernetIPStationSettings station, int parentId = -1, ImportData inRootItem = null)
        {

            if (nArrayDim == 0)
            {
                return;
            }

            uint nElementSize = 0;


            if ((nMovType == "String") && (nElemSize > 0))
            {
                nElementSize = nElemSize;
            }

            ImportData IVar = null;
            int IVarId = -1;

            String szAux;


            if (arrayType == ImportTypes.Standard && nMovType != "String")
            {
                IVar = importDataModel.addImportData();

                String szName = szArrayName;
                if (nDimIndex > 0)
                {
                    szName += "]";
                }
                if (nArrayBlocks > 1)
                {
                    szAux = string.Format("_{0}_of_{1}", nArrayBlock, nArrayBlocks);
                    szName += szAux;
                }
                ((ImportDataOmronEthernetIP)IVar).PreName = szNamePrefix;
                IVar.Name = szName;

                ((ImportDataOmronEthernetIP)IVar).ElemType = szElemType;
                ((ImportDataOmronEthernetIP)IVar).Type = (DataType)Enum.Parse(typeof(DataType), nMovType);
                IVar.szType = "ARRAY";
                ((ImportDataOmronEthernetIP)IVar).ImportType = ImportTypes.Array;

                IVar.ArrayDimension = nArrayDim;
                ((ImportDataOmronEthernetIP)IVar).Size = nArrayDim * nElementSize;

                if (nDimIndex == 0)
                    szAux = string.Format("[{0}]", nArrayStart);
                else
                    szAux = string.Format(",{0}]", nArrayStart);
                ((ImportDataOmronEthernetIP)IVar).ElemTypeView = string.Format("ARRAY[{0}..{1}] of {2}", nArrayStart, (nArrayStart + nArrayDim)-1, szElemType);

                IVar.Address = szAddressPrefix + szAddress + szAux;
                IVar.Description = description;
                IVar.parentId = parentId;
                IVarId = IVar.Id = m_lGlobalID++;
                ((ImportDataOmronEthernetIP)IVar).LinkType = linkType;
                AddTreeItem(IVar, inRootItem);

            }

            String szFieldLine;
            uint i = 0;
            for (i = 0; i < nArrayDim; i++)
            {
                szFieldLine = szArrayName;
                if (nDimIndex == 0)
                    szAux = string.Format("[{0}]", i + nArrayStart);
                else
                    szAux = string.Format(",{0}]", i + nArrayStart);
                szFieldLine += szAux;

                if (arrayType == ImportTypes.Standard && nMovType != "String")
                {
                    AddStandardVarTxt(szNamePrefix, szFieldLine, szAddressPrefix, szAddress + szAux,
                        description, szElemType, nMovType, nElementSize, linkType, IVar.Id, IVar);

                }
                else if (arrayType == ImportTypes.Struct)
                {
                    AddStruct(szNamePrefix, szFieldLine, szAddressPrefix, szAddress + szAux,
                        description, szElemType, linkType, station, IVarId, IVar);
                }
                else if (arrayType != ImportTypes.Unknown)
                {
                    if (szNamePrefix == "")
                        AddStandardVarTxt("", szFieldLine, szAddressPrefix, szAddress + szAux,
                            description, szElemType, nMovType, nElementSize, linkType);
                    else
                        AddStandardVarTxt("", szNamePrefix + "." + szFieldLine, szAddressPrefix, szAddress + szAux,
                            description, szElemType, nMovType, nElementSize, linkType);

                }
            }
            return;
        }

        void AddArray1DimDisplace(ref String szNamePrefix, ref String szArrayName,
                          ref String szAddressPrefix, ref String szAddress, ref String description,
                          ref String szElemType, uint nElemSize, uint nArrayDim,
                          uint nDimIndex, uint nArrayStart, uint nArrayBlock, uint nArrayBlocks, ImportTypes arrayType,
                          String nMovType, int linkType, OmronEthernetIPStationSettings station, int parentId = -1, ImportData inRootItem = null)
        {

            if (nArrayDim == 0)
            {
                return;
            }

            uint nElementSize = 0;


            if ((nMovType == "String") && (nElemSize > 0))
            {
                nElementSize = nElemSize;
            }

            ImportData IVar = null;
            int IVarId = -1;

            String szAux;


            if (arrayType == ImportTypes.Standard && nMovType != "String")
            {
                IVar = importDataModel.addImportData();

                String szName = szArrayName;
                if (nDimIndex > 0)
                {
                    szName += "]";
                }
                if (nArrayBlocks > 1)
                {
                    szAux = string.Format("_{0}_of_{1}", nArrayBlock, nArrayBlocks);
                    szName += szAux;
                }
                ((ImportDataOmronEthernetIP)IVar).PreName = szNamePrefix;
                IVar.Name = szName;

                ((ImportDataOmronEthernetIP)IVar).ElemType = szElemType;
                ((ImportDataOmronEthernetIP)IVar).Type = (DataType)Enum.Parse(typeof(DataType), nMovType);
                IVar.szType = "ARRAY";
                ((ImportDataOmronEthernetIP)IVar).ImportType = ImportTypes.Array;

                IVar.ArrayDimension = nArrayDim;
                ((ImportDataOmronEthernetIP)IVar).Size = nArrayDim * nElementSize;

                if (nDimIndex == 0)
                {
                    szAux = string.Format("[{0}]", nArrayStart);
                } else {
                    szAux = string.Format(",{0}]", nArrayStart);                    
                }
                ((ImportDataOmronEthernetIP)IVar).ElemTypeView = string.Format("ARRAY[{0}..{1}] of {2}", nArrayStart, (nArrayStart + nArrayDim)-1, szElemType);

                IVar.Address = szAddressPrefix + szAddress + szAux;
                IVar.Description = description;
                IVar.parentId = parentId;
                IVarId = IVar.Id = m_lGlobalID++;
                ((ImportDataOmronEthernetIP)IVar).LinkType = linkType;
                AddTreeItem(IVar, inRootItem);

            }

            String szFieldLine;
            uint i = 0;
            for (i = 0; i < nArrayDim; i++)
            {
                szFieldLine = szArrayName;
                if (nDimIndex == 0)
                {
                    szAux = string.Format("[{0}]", i + nArrayStart);
                }
                else
                {
                    szAux = string.Format(",{0}]", i + nArrayStart);
                }
                szFieldLine += szAux;

                if (arrayType == ImportTypes.Standard && nMovType != "String")
                {
                    AddStandardVar(szNamePrefix, szFieldLine, szAddressPrefix, szAddress + szAux,
                        description, szElemType, nMovType, nElementSize, linkType, IVar.Id, IVar);

                }
                else if (arrayType == ImportTypes.Struct)
                {
                    AddStruct(szNamePrefix, szFieldLine, szAddressPrefix, szAddress + szAux,
                        description, szElemType, linkType, station, IVarId, IVar);
                }
                else if (arrayType != ImportTypes.Unknown)
                {
                    if (szNamePrefix == "")
                        AddStandardVar("", szFieldLine, szAddressPrefix, szAddress + szAux,
                            description, szElemType, nMovType, nElementSize, linkType);
                    else
                        AddStandardVar("", szNamePrefix + "." + szFieldLine, szAddressPrefix, szAddress + szAux,
                            description, szElemType, nMovType, nElementSize, linkType);

                }
            }
            return;
        }

        ImportTypes GetVarTypeAndSizeL5K(string inSzType, ref uint nVarSize, ref String nType)
        {
            ImportTypes exit = ImportTypes.Unknown;
            inSzType = inSzType.Trim();
            string szType = inSzType;
            szType = szType.ToUpper();
            nVarSize = 0;
            nType = "";
            if (szType.Length == 0)
            {
                return (exit);
            }
            if (szType == "BOOL" ||
                szType == "BIT")
            {
                nType = "Boolean";
                nVarSize = 1;
                exit = ImportTypes.Standard;
            }
            else if (szType == "BYTE")
            {
                nType = "Byte";
                nVarSize = 1;
                exit = ImportTypes.Standard;
            }
            else if (szType == "SINT")
            {
                nType = "SByte";
                nVarSize = 1;
                exit = ImportTypes.Standard;
            }
            else if (szType == "WORD"
                || szType == "UINT")
            {
                nType = "UInt16";
                nVarSize = 2;
                exit = ImportTypes.Standard;
            }
            else if (szType == "DWORD"
                || szType == "UDINT")
            {
                nType = "UInt32";
                nVarSize = 4;
                exit = ImportTypes.Standard;
            }
            else if (szType == "INT")
            {
                nType = "Int16";
                nVarSize = 2;
                exit = ImportTypes.Standard;
            }
            else if (szType == "DINT" )
            {
                nType = "Int32";
                nVarSize = 4;
                exit = ImportTypes.Standard;
            }
            else if (szType == "REAL")
            {
                nType = "Float";
                nVarSize = 4;
                exit = ImportTypes.Standard;
            }
            else if (szType == "LREAL")
            {
                nType = "Double";
                nVarSize = 8;
                exit = ImportTypes.Standard;
            }
            else if (szType.Contains("STRING("))
            {
                int Index1 = szType.IndexOf("(");
                int Index2 = szType.IndexOf(")");
                string dimension = szType.Substring(Index1 + 1, Index2 - Index1 - 1).Trim(new char[] { '\r', '\n', '\t', ' ' });
                nType = "String";
                nVarSize = 0;
                if (!uint.TryParse(dimension, out nVarSize))
                {
                    nVarSize = 0;
                }
                exit = ImportTypes.Standard;
            }
            else if ((szType == "LWORD") ||
                      (szType == "ULINT"))
            {
                nType = "UInt64";
                nVarSize = 8;
                exit = ImportTypes.Standard;
            }
            else if (szType == "LINT")
            {
                nType = "Int64";
                nVarSize = 8;
                exit = ImportTypes.Standard;
            }
            else if (szType == "STRUCT")
            {
                nType = "Struct";
                nVarSize = 1;
                exit = ImportTypes.Struct;
            }
            else if (m_mapStruct.ContainsKey(inSzType))
            {
                if (m_mapStruct[inSzType].Count != 0)
                {
                    nType = inSzType;
                    nVarSize = StructSize(inSzType);
                    exit = ImportTypes.Struct;
                }
            }

            return (exit);
        }

        uint StructSize(string szType)
        {
            uint outVal = 0;
            if (m_mapStruct.ContainsKey(szType))
            {
                if (m_mapStruct[szType].Count != 0)
                {
                    uint nFieldSize = 0;
                    String nMovType = "";
                    foreach (string itemStructValue in m_mapStruct[szType])
                    {
                        if (GetStructMemberTypeFromStringL5K(itemStructValue, ref nFieldSize, ref nMovType) != ImportTypes.Unknown)
                            outVal += nFieldSize;
                    }
                }
            }
            return (outVal);
        }

        ImportTypes GetStructMemberTypeFromStringL5K(String szFieldLine, ref uint nFieldSize, ref String nMovType)
        {
            // Field type?
            ImportTypes nFieldType = ImportTypes.Unknown;

            int nSearchIndex = szFieldLine.IndexOf(" ");
            if (nSearchIndex < 0)
            {
                return (nFieldType);
            }

            nFieldType = GetVarTypeAndSizeL5K(szFieldLine.Substring(0, nSearchIndex), ref nFieldSize,
                ref nMovType);

            return nFieldType;
        }

        string ReadLineL5K(ref System.IO.StreamReader readFile)
        {
            string outString = readFile.ReadLine();
            if (outString != null)
            {
                while (outString.EndsWith(","))
                    outString += readFile.ReadLine();
            }
            return outString;

        }

        String GetKeyL5K(String Line)
        {
            String LineOut = "";
            return GetKeyL5K(Line, ref LineOut);
        }

        String GetKeyL5K(String Line, ref String LineOut)
        {
            if (String.IsNullOrWhiteSpace(Line))
            {
                return "";
            }

            String Aux = Line.Trim(new char[] { '\r', '\n', '\t', ' ' });
            LineOut = String.Empty;

            int spaceIndex = Aux.IndexOf(" ");
            if (spaceIndex < 0)
            {
                return Aux;
            }
            else
            {
                LineOut = Aux.Substring(spaceIndex);
                return Aux.Substring(0, spaceIndex);
            }
        }

        String RemoveArgumentsL5K(String Line)
        {
            String aux = Line;
            RemoveArgumentsL5K(ref aux);
            return aux;
        }

        void RemoveArgumentsL5K(ref String Line)
        {
            if (String.IsNullOrWhiteSpace(Line))
            {
                return;
            }

            String szAuxLeft;
            String szAuxRight;
            int CommentInitIndex = Line.IndexOf("(");
            if (CommentInitIndex < 0 || CommentInitIndex >= Line.Length)
                return;
            int CommentEndIndex = Line.IndexOf(")", CommentInitIndex);

            if (CommentEndIndex < Line.Length && CommentEndIndex > CommentInitIndex)
            {
                szAuxLeft = Line.Substring(0, CommentInitIndex);
                szAuxRight = Line.Substring(CommentEndIndex + 1);
                Line = szAuxLeft + szAuxRight;
            }
        }

        bool StructFoundL5K(string szType)
        {
            string szVarType = szType.Trim(new char[] { '\r', '\n', '\t', ' ' });
            if (!String.IsNullOrEmpty(szVarType))
            {
                if (m_mapStruct.ContainsKey(szVarType))
                    return (m_mapStruct[szVarType].Count > 0);
            }
            return (false);
        }

        bool GetArrayDimL5K(ref string szFieldType, ref String szLineOut,ref List<ArrayInfo> arraySize, out uint arrayDimNum)
        {
            arrayDimNum = 0;
            string arrayType = String.Empty;
            if (IsArray(szFieldType, ref arraySize, ref arrayType, ref arrayDimNum))
            {
                // var declaration without array parts ([..])
                //szLineOut = szLineOut.Substring(0, szLineOut.LastIndexOf("[")).Trim(new char[] { '\r', '\n', '\t', ' ' });
                szFieldType = arrayType;

                return true;
            }
            else
            {
                return false;
            }
        }

        //**********************************************************************
        internal void AddTreeItem(ImportData tag, ImportData parent = null)
        {
            string strId = tag.Id.ToString("X6");
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
   
    public class ImportDataOmronEthernetIP : ImportData, IDisposable
    {
        public ImportDataOmronEthernetIP(ImportDataModel inDataModel) :
            base(inDataModel)
        {
            dataModelEthernetIP = inDataModel as ImportDataModelOmronEthernetIP;
        }
        private ImportDataModelOmronEthernetIP dataModelEthernetIP { get; set; }

        private string _PreName;
        public string PreName
        {
            get { return _PreName; }
            set { _PreName = value; }
        }

        private string _ElemType;
        public string ElemType
        {
            get { return _ElemType; }
            set { _ElemType = value; }
        }
        private string _ElemTypeView;
        public string ElemTypeView
        {
            get { return _ElemTypeView; }
            set { _ElemTypeView = value; }
        }
        private DataType _Type;
        public DataType Type
        {
            get { return _Type; }
            set { _Type = value; }
        }
        private ImportTypes _ImportType;
        public ImportTypes ImportType
        {
            get { return _ImportType; }
            set { _ImportType = value; }
        }
        private uint _Size;
        public uint Size
        {
            get { return _Size; }
            set { _Size = value; }
        }
        //private string _Name;
        //public string Name
        //{
        //    get { return dataModel.getStationName() != "_" ? 
        //                 dataModel.getStationName() + _Name : _Name; }
        //    set { _Name = value; }
        //}
        
        //public string TreeName
        //{
        //    get { return getTreeName(); }
        //}

        //private string getTreeName()
        //{
        //    string treeName = Name;
        //    if (Parent != null)
        //    {
        //        treeName = Parent.getTreeName() + "." + treeName;
        //    }
        //    return treeName;
        //}

        private int _LinkType;
        public int LinkType
        {
            get { return _LinkType; }
            set { _LinkType = value; }
        }

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
                    var sl = el as ImportDataOmronEthernetIP;
                    if (sl != null)
                    {
                        sl.Dispose();
                    }
                }
                Children.Clear();
            }
        }
        #endregion
    }

    public class ImportDataModelOmronEthernetIP : ImportDataModel, IDisposable
    {
        public ImportDataModelOmronEthernetIP(GetStationName inGetStationName) :
            base(inGetStationName)
        {
        }
        public override ImportData addImportData()
        {
            ImportDataOmronEthernetIP importData = new ImportDataOmronEthernetIP(this);
            return importData;
        }

        #region IDisposable Members
        public void Dispose()
        {
            foreach (ImportData el in Children)
            {
                ImportDataOmronEthernetIP els7 = el as ImportDataOmronEthernetIP;
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
