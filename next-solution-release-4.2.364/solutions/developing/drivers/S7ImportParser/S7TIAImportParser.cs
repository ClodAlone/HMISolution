using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Accon.AGLink;
using Accon.Symbolik;
using System.Windows.Forms;
using UFUAModel;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Text.RegularExpressions;
using static System.Windows.Forms.VisualStyles.VisualStyleElement.TextBox;

namespace S7ImportParser
{
    public class S7TIAImportParser : IDisposable
    {
        public enum ProtocolType
        {
            S7Tcp,
            TiaSymbolic
        }

        IntPtr rootSchemaNodeHandle = IntPtr.Zero;
        Dictionary<string, IntPtr> m_MapProgramsProgetTiaPortal;
        Dictionary<string, string> m_MapValueDepthStructToNumStruct;
        Dictionary<int, string> programList;
        Dictionary<string, List<stNode>> m_PrototipeVariableStruct;
        Dictionary<string, stPrototype> m_mapUDTBaseS7P;
        Dictionary<long, ImportVariable> m_MapImportVariable;
        List<stNode> m_ListOfAllChilds;
        private int m_currentCulture;
        public ProtocolType Protocol = ProtocolType.TiaSymbolic;
        private int currentDBNr = 0;

        public const char FILE_FIELD_SEPARATOR = ':';
        public const string TIA_PROJECT_TAG_TABLE = "Default tag table";
        public const string TIA_PROJECT_TAG_TABLE_TYPE = "Tag_Table";
        public const string TIA_PROJECT_DATABLOCK_TYPE = "Datablock";

        private ImportSourceManagement m_ImportSource = ImportSourceManagement.None;

        // used only to mark db variabile with special identifier for tag table (on PLC DB il always >=0)
        private const int S7TCP_TAG_TABLE_DB = -1;

        public enum ImportTypes
        {
            Unknown,
            Standard,
            Array,
            Struct,
        }

        static bool IsAStructure(AGL4.SystemType struct_system_type)
        {
            bool isAStruct = false;
            switch (struct_system_type)
            {
                case AGL4.SystemType.S7_DTL:
                case AGL4.SystemType.S7_IEC_Counter:
                case AGL4.SystemType.S7_IEC_DCounter:
                case AGL4.SystemType.S7_IEC_LCounter:
                case AGL4.SystemType.S7_IEC_SCounter:
                case AGL4.SystemType.S7_IEC_UCounter:
                case AGL4.SystemType.S7_IEC_UDCounter:
                case AGL4.SystemType.S7_IEC_ULCounter:
                case AGL4.SystemType.S7_IEC_USCounter:
                case AGL4.SystemType.S7_IEC_Timer:
                case AGL4.SystemType.S7_Struct:
                case AGL4.SystemType.S7_UDT_Instance:
                case AGL4.SystemType.S7_FB_Instance:
                    isAStruct = true;
                    break;
                default:
                    break;

            }
            return (isAStruct);
        }

        public struct ImportVariable
        {
            public String szName;
            public String szDescription;
            public DataType nType;
            public String szType;
            public String szAddressImport;
            public String szAddress;
            public uint nSize;
            public uint ArrayDimension;
            public long lID;
            public long lParent;
            public DataType nElemType;
            public String szPreName;
            public uint valMinAbsOp;
            public uint valMaxAbsOp;
            public ImportTypes ImportType;
            public AGL4.SystemType S7DataFormat;
            public uint StringLength;
            public UInt32 depth;
            public string path;
            public bool ToDelete;

            //public bool IsMovedFrom()
            //{
            //    return (S7DataFormat == AGL4.SystemType.S7_UDT_Instance || S7DataFormat == AGL4.SystemType.S7_Struct || ArrayDimension != 0);
            //}
        };

        public struct stPrototype
        {
            public string udtName;
            public List<String> Values;
            public int numRealChildren;
        };
        
        class stNode : ICloneable
        {
            public IntPtr node;
            //public uint depthStruct;
            public int LastChild;
            public int NumChild;
            public UInt32 depth;
            public string path;
            public AGL4.HierarchyType hierarchy_type;
            public AGL4.SystemType system_type;
            public string name;
            public string nameOrig;
            public AGL4.ValueType value_type;
            public AGL4.PermissionType permissionType;
            public string comment;
            public uint dimension;
            public string matrix;
            public int string_size;
            public string udtName;
            public uint ArrayDimension;
            //public bool childOfStruct;
            public string objectName;
            public stNode ParentNode;
            
            public ProtocolType Protocol;
            public int S7Tcp_DBNr;
            public int S7Tcp_Offset;
            public int S7Tcp_BitNr;

            public AGL4.SystemType S7Tcp_StructInfo_1stMemberType;

            public int S7Tcp_StructInfo_TotalSize;
            public int S7Tcp_StructInfo_LevelSize;

            public int S7Tcp_StructInfo_ArraySize;
            public int S7Tcp_StructInfo_ArrayOffset;
            public int S7Tcp_ArrayFirstIndex;

            public AGL4.SystemType S7Tcp_TagTableSystemType;

            public stNode(ProtocolType protocol)
            {
                Protocol = protocol;
                S7Tcp_StructInfo_1stMemberType = AGL4.SystemType.S7_UNKNOWN;
                S7Tcp_TagTableSystemType = AGL4.SystemType.UNKNOWN;
            }

            public void stNodeInit(IntPtr nodeInfo)
            {
                //depthStruct = 0;
                LastChild = 0;
                NumChild = 0;
                depth = 1;
                path = string.Empty;
                name = string.Empty;
                hierarchy_type = AGL4.HierarchyType.UNDEFINED;
                value_type = AGL4.ValueType.UNDEFINED;
                system_type = AGL4.SystemType.UNDEFINED;
                permissionType = AGL4.PermissionType.NONE;
                dimension = 0;
                matrix = string.Empty;
                string_size = 0;
                udtName = string.Empty;
                ArrayDimension = 0;
                //childOfStruct = false;
                objectName = string.Empty;
                node = nodeInfo;
                ParentNode = null;
                hierarchy_type = AGL4.HierarchyType.UNDEFINED;
                S7Tcp_DBNr = 0;
                S7Tcp_Offset = 0;
                S7Tcp_BitNr = 0;

                S7Tcp_StructInfo_TotalSize = 0;
                S7Tcp_StructInfo_LevelSize = 0;

                S7Tcp_StructInfo_ArraySize = 0;
                S7Tcp_StructInfo_ArrayOffset = 0;
                S7Tcp_ArrayFirstIndex = 0;
            }

            public bool IsInvalidMoviconName()
            {
                return (UFUAModel.Helpers.NameValidator.EnsureValidName(objectName) != objectName);
            }

            // check if element have to be moved from orginal position for driver requirement
            public bool MovedToRoot()
            {
                switch (Protocol)
                {
                    case ProtocolType.TiaSymbolic:
                        // move to root string and array of struct
                        return ((ArrayDimension != 0 && (system_type == AGL4.SystemType.S7_UDT || IsAStructure(system_type) ))
                            || ((system_type == AGL4.SystemType.S7_String) || (system_type == AGL4.SystemType.S7_WString) && IsParentNodeStructOrUdt()) || IsInvalidMoviconName());
                    case ProtocolType.S7Tcp:
                        // move to root array of struct (string are leaved into struct)
                        return ((ArrayDimension != 0 && (system_type == AGL4.SystemType.S7_UDT || IsAStructure(system_type)))
                            || IsInvalidMoviconName());
                    default:
                        return false;
                }
            }

            internal void UpdateAllPath(string sourcePath, string targetPath)
            {
                if (path.IndexOf(sourcePath, 0) >= 0)
                {
                    path = path.Replace(sourcePath, targetPath);
                }
                if (name.IndexOf(sourcePath, 0) >= 0)
                {
                    name = name.Replace(sourcePath, targetPath);
                }
            }

            private void CalculateVarAddress(stNode node, ref string address)
            {

                if (string.IsNullOrWhiteSpace(address))
                {
                    address = node.objectName;
                }
                else
                {
                    // 1st node is always PLC
                    if (node.ParentNode == null) { 
                        address = string.Format("{0}.{1}", "PLC", address);
                    }else{                         
                        switch (node.system_type)
                        {                            
                            case AGL4.SystemType.Group:
                                // discard logical name
                                break;
                            case AGL4.SystemType.S7_Tag_Table:
                                // replace logical name with "plc" name
                                address = string.Format("{0}.{1}", "Table", address);
                                break;
                            default:
                                address = string.Format("{0}.{1}", node.objectName, address);
                                break;
                        }
                    }
                }                

                if (node.ParentNode != null)
                    CalculateVarAddress((stNode)node.ParentNode, ref address);
            }

            /// <summary>
            /// Calculate the "real" plc address removing from address group\folder
            /// </summary>
            /// <returns></returns>
            public void GetAddress(ImportSourceManagement importSource, out string address, out string addressImport)
            {
                address = string.Empty;
                addressImport = this.name;

                switch (importSource)
                {
                    case ImportSourceManagement.Plc:
                        address = this.name;
                        break;
                    case ImportSourceManagement.Project:
                        // try to recalculate tag address 
                        CalculateVarAddress(this, ref address);
                        break;
                }
            }

            public bool IsParentNodeStructOrUdt()
            {
                return (ParentNode != null && (ParentNode.system_type == AGL4.SystemType.S7_UDT || IsAStructure(ParentNode.system_type)));
            }

            #region S7Tcp
            public void UpdateStructArraySize(int arraySize, int arrayOffset)
            {
                S7Tcp_StructInfo_ArraySize = arraySize;
                S7Tcp_StructInfo_ArrayOffset = arrayOffset;
            }

            public void updateStructSize(ref int totalSize, ref int level, AGL4.SystemType varType, int offset, int bit, int size, int string_size)
            {
                level++;
                if (ParentNode != null)
                {
                    if (ParentNode.system_type == AGL4.SystemType.S7_Datablock || ParentNode.system_type == AGL4.SystemType.S7_Tag_Table)
                        return;

                    if (IsAStructure(ParentNode.system_type))
                    { 
                        if (level == 1)
                        {
                            ParentNode.S7Tcp_StructInfo_LevelSize = offset;

                            switch (varType)
                            {
                                case AGL4.SystemType.S7_Bool:
                                    ParentNode.S7Tcp_StructInfo_LevelSize += 1; // 1 = 1 byte --> to contain a bit 1 byte is required
                                    break;
                                case AGL4.SystemType.S7_String:
                                    // assign the string size only if not assigned (otherwise last string size will be assigned)
                                    if (ParentNode.string_size == 0)
                                        ParentNode.string_size = string_size; // real strig size
                                    ParentNode.S7Tcp_StructInfo_LevelSize += size;  //((real)string_size + (string header)2)
                                    break;
                                default:
                                    ParentNode.S7Tcp_StructInfo_LevelSize += size;
                                    break;
                            }

                            if (ParentNode.S7Tcp_StructInfo_LevelSize % 2 != 0)
                                ParentNode.S7Tcp_StructInfo_LevelSize++;
                        }

                        if (ParentNode.S7Tcp_StructInfo_1stMemberType == AGL4.SystemType.S7_UNKNOWN)
                            ParentNode.S7Tcp_StructInfo_1stMemberType = varType;

                        ParentNode.S7Tcp_StructInfo_TotalSize = ParentNode.S7Tcp_StructInfo_LevelSize + totalSize;

                        totalSize += ParentNode.S7Tcp_StructInfo_LevelSize;

                        // recursive call
                        ParentNode.updateStructSize(ref totalSize, ref level, varType, offset, bit, size, string_size);
                    }
                }
            }

            public void UpdateStructSize(AGL4.SystemType varType, int offset, int bit, int size, int string_size)
            {
                int totalSize = 0;
                int level = 0;

                updateStructSize(ref totalSize, ref level, varType, offset, bit, size, string_size);
            }

            private int CalculateArrayStructSize(stNode node, int arrayElement)
            {
                return ((arrayElement - node.S7Tcp_ArrayFirstIndex) - node.S7Tcp_StructInfo_ArrayOffset) * (node.S7Tcp_StructInfo_TotalSize);
            }

            private bool GetArrayIndexFromName(string field, out int arrayIndex)
            {
                arrayIndex = -1;

                int start = field.IndexOf("[");
                if (start >= 0)
                {
                    int end = field.IndexOf("]", start);
                    if (end > start)
                    {
                        if (!int.TryParse(field.Substring(start + 1, end - start - 1), out arrayIndex))
                            arrayIndex = -1;
                    }
                }

                return (arrayIndex != -1);
            }

            public void getElementOffset(stNode node, ref int size)
            {
                if (node != null)
                {
                    if (node.system_type == AGL4.SystemType.S7_Datablock || node.system_type == AGL4.SystemType.S7_Tag_Table)
                        return;

                    size += node.S7Tcp_Offset;
                    
                    if (IsAStructure(node.system_type))
                    { 
                        if (GetArrayIndexFromName(node.objectName, out int arrayIndex))                        
                            size += (ushort)CalculateArrayStructSize(node, arrayIndex);
                    }

                    if (node.ParentNode != null)
                        getElementOffset(node.ParentNode, ref size);
                }
            }

            public void GetAddressS7Tcp(S7TcpAddress s7TCPOffset, out string address)
            {
                int offset = 0;
                
                getElementOffset(this, ref offset);

                address = CalculateAddressS7Tcp(s7TCPOffset.DBNr, offset, S7Tcp_BitNr);
            }


            /// <summary>
            /// Get address type (letter M,I or Q) of tag table's var; if not defined (struct's member), try to get data from parent
            /// </summary>
            /// <param name="tagTableSystemType"></param>
            /// <returns></returns>
            private string GetTagTableAddressType(AGL4.SystemType tagTableSystemType)
            {
                if (!(tagTableSystemType == AGL4.SystemType.S7_Memory || tagTableSystemType == AGL4.SystemType.S7_Input || tagTableSystemType == AGL4.SystemType.S7_Output))
                    // recursive call
                    S7TcpGetSystemTypeTableUdtSubStruct(ref this.ParentNode, ref tagTableSystemType);

                switch (tagTableSystemType) {
                    case AGL4.SystemType.S7_Memory:
                        return "M";
                    case AGL4.SystemType.S7_Input:
                        return "I";
                    case AGL4.SystemType.S7_Output:
                        return "Q";
                    default:
                        return string.Empty;
                }
            }

            private string CalculateAddressS7Tcp(int dbNr, int offset, int bitNr)
            {
                string szAddress = string.Empty;
                AGL4.SystemType vartype;

                if (IsAStructure(system_type))
                    vartype = S7Tcp_StructInfo_1stMemberType;
                else
                    vartype = system_type;

                switch (vartype)
                {
                    case AGL4.SystemType.S7_Bool:
                        if (dbNr == S7TCP_TAG_TABLE_DB)
                            szAddress = string.Format("{0}{1}.{2}", GetTagTableAddressType(S7Tcp_TagTableSystemType), offset, bitNr);
                        else
                            szAddress = string.Format("DB{0}.DBX{1}.{2}", dbNr, offset, bitNr);
                        break;
                    case AGL4.SystemType.S7_Byte:
                    case AGL4.SystemType.S7_USInt:
                    case AGL4.SystemType.S7_SInt:
                    case AGL4.SystemType.S7_Char:
                        if (dbNr == S7TCP_TAG_TABLE_DB)
                            szAddress = string.Format("{0}B{1}", GetTagTableAddressType(S7Tcp_TagTableSystemType), offset);
                        else
                            szAddress = string.Format("DB{0}.DBB{1}", dbNr, offset);
                        break;
                    case AGL4.SystemType.S7_Word:
                    case AGL4.SystemType.S7_Int:
                    case AGL4.SystemType.S7_UInt:
                    case AGL4.SystemType.S7_Date:
                        if (dbNr == S7TCP_TAG_TABLE_DB)
                            szAddress = string.Format("{0}W{1}", GetTagTableAddressType(S7Tcp_TagTableSystemType), offset);
                        else
                            szAddress = string.Format("DB{0}.DBW{1}", dbNr, offset);
                        break;
                    case AGL4.SystemType.S7_S5Time:
                        if (IsAStructure(system_type))
                        {
                            if (dbNr == S7TCP_TAG_TABLE_DB)
                                szAddress = string.Format("{0}B{1}", GetTagTableAddressType(S7Tcp_TagTableSystemType), offset);
                            else
                                szAddress = string.Format("DB{0}.DBB{1}", dbNr, offset);
                        }
                        else
                        {
                            if (dbNr == S7TCP_TAG_TABLE_DB)
                                szAddress = string.Format("{0}W{1},T", GetTagTableAddressType(S7Tcp_TagTableSystemType), offset);
                            else
                                szAddress = string.Format("DB{0}.DBW{1},T", dbNr, offset);
                        }
                        break;

                    case AGL4.SystemType.S7_DInt:
                    case AGL4.SystemType.S7_UDInt:
                    case AGL4.SystemType.S7_DWord:
                    case AGL4.SystemType.S7_Real:
                    case AGL4.SystemType.S7_Time:
                    case AGL4.SystemType.S7_Time_Of_Day:
                        if (dbNr == S7TCP_TAG_TABLE_DB)
                            szAddress = string.Format("{0}D{1}", GetTagTableAddressType(S7Tcp_TagTableSystemType), offset);
                        else
                            szAddress = string.Format("DB{0}.DBD{1}", dbNr, offset);
                        break;

                    case AGL4.SystemType.S7_String:
                    case AGL4.SystemType.S7_WString:
                        //ushort nLengthString = (ushort)string_size; // DataRW.OpAnz;
                        //nLengthString -= 2;
                        if (dbNr == S7TCP_TAG_TABLE_DB)
                            szAddress = string.Format("{0}{1}:{2}", GetTagTableAddressType(S7Tcp_TagTableSystemType), (offset + 2), string_size);
                        else
                            szAddress = string.Format("DB{0}.DBB{1}:{2}", dbNr, (offset + 2), string_size);
                        break;

                    case AGL4.SystemType.S7_ULInt:
                    case AGL4.SystemType.S7_LInt:
                    case AGL4.SystemType.S7_LWord:
                    case AGL4.SystemType.S7_LReal:
                        // address Int64 using DBB format
                        if (dbNr == S7TCP_TAG_TABLE_DB)
                            szAddress = string.Format("{0}{1}", GetTagTableAddressType(S7Tcp_TagTableSystemType), offset);
                        else
                            szAddress = string.Format("DB{0}.DBB{1}", dbNr, offset);
                        break;                    
                }

                return szAddress;
            }

            #endregion

            #region ICloneable Methods
            /// <remarks>
            /// Creates a deep copy of the collection.
            /// </remarks>
            public object Clone()
            {
                return this.MemberwiseClone();
            }
#endregion
        }
        struct stOffset
        {
            public int nStartArray;
            public int nHigt;
            public int nSizeArray;
        };

        public class DBsNameAndTables
        {
            public string ObjectType { get; set; }
            public string ObjectName { get; set; }

            public DBsNameAndTables(string objectType, string objectName)
            {
                ObjectType = objectType;
                ObjectName = objectName;
            }
        }

        class S7TcpAddress
        {
            public class Level
            {
                public int Offset;
                public string NodeName;
                public AGL4.SystemType System_type;

                public Level(int offset, string nodeName, AGL4.SystemType system_type)
                {                    
                    Offset = offset;
                    NodeName = nodeName;
                    System_type = system_type;
                }
            }

            public int DBNr;
            public S7TcpAddress()
            {
                DBNr = 0;
            }

            public void Reset()
            {
                DBNr = 0;
            }

            public void AddLevel(stNode stnode)
            {             
                switch (stnode.system_type)
                {
                    case AGL4.SystemType.S7_Tag_Table:
                    case AGL4.SystemType.S7_Datablock:
                        DBNr = stnode.S7Tcp_DBNr;
                        break;
                }             
            }
        };

        //define how to manage imported data (from PLC or TIA project/file) 
        public enum ImportSourceManagement
        {
            ProjectAndPlc = 0,
            Project = 1,
            Plc = 2,    // defined but not used
            None = 99,
        }

        public S7TIAImportParser()
        {                        
            try
            {
                // Set the library license code
                AGL4.Activate("00BEF5-E1F5-260260");
            }
            catch
            {
                MessageBox.Show(Properties.Resources.AGLink40NotFound,
                                Properties.Resources.ImportMsgBoxTitle);
                return;
            }
        }
        public bool IsTheFileTiaImportedOfPLC(IntPtr pHandleFile)
        {
            int ret = AGL4.Symbolic_GetProjectReferenceCulture(pHandleFile, ref m_currentCulture);

            //Get number of child nodes for this node
            int child_count = 0;
            ret = AGL4.Symbolic_GetChildCount(pHandleFile, ref child_count);
            if (ret != AGL4Sym.AGLSYM_SUCCESS)
            {
                //TRACE(_T("m_pAGL_Symbolic_GetChildCount Error number =  %d  line = %d\n"), nError, __LINE__);
                return (false);
            }

            if (child_count > 0)
            {
                programList = new Dictionary<int, string>();
                //Iterate through all childs
                for (int child_index = 0; child_index < child_count; ++child_index)
                {
                    IntPtr child = new IntPtr();
                    ret = AGL4.Symbolic_GetChild(pHandleFile, child_index, ref child);
                    if (ret != AGL4Sym.AGLSYM_SUCCESS)
                    {
                        continue;
                    }
                    AGL4.SegmentType segmentType = AGL4.SegmentType.UNDEFINED;
                    //Differentiate the type of a node. Is it a normal field node, an index or the root node.
                    ret = AGL4.Symbolic_GetSegmentType(child, ref segmentType);
                    if (ret != AGL4Sym.AGLSYM_SUCCESS)
                    {
                        //TRACE(_T("m_pAGL_Symbolic_GetSegmentType Error number =  %d  line = %d\n"), nError, __LINE__);
                        return (false);
                    }

                    if (segmentType == AGL4.SegmentType.FIELD)
                    {
                        string node_name = String.Empty;
                        ret = ParseNameInfo(ref child, ref node_name);
                        if (ret != AGL4Sym.AGLSYM_SUCCESS)
                        {
                            //TRACE(_T("ParseNameInfo Error number =  %d  line = %d\n"), nError, __LINE__);
                            return (false);
                        }
                        if (node_name == "PLC")
                        {
                            return (true);
                        }
                    }
                }
            }

            return (false);
        }


        public void GetListDataBlocksAndTables(IntPtr Handle, ref List<S7TIAImportParser.DBsNameAndTables> listDataBlocksAndTables)
        {
            stNode stnode = new stNode(Protocol);
            stnode.stNodeInit(Handle);

            int nrElementsTagTables = 0;
            int nrDataBlocks = 0;
            string symbol_path = string.Empty;

            ParseDataBlocksAndTables(ref stnode, ref listDataBlocksAndTables, ref symbol_path, ref nrElementsTagTables, ref nrDataBlocks);

            //hasElements = (nrElementsTagTables > 1 || nrDataBlocks > 0);

            LoadDefaultListTable(ref listDataBlocksAndTables);            
        }

        public void LoadDefaultListTable(ref List<S7TIAImportParser.DBsNameAndTables> listDataBlocksAndTables)
        {
            if (listDataBlocksAndTables == null)
                listDataBlocksAndTables = new List<S7TIAImportParser.DBsNameAndTables>();

            var match = listDataBlocksAndTables.FirstOrDefault(o => o.ObjectName == TIA_PROJECT_TAG_TABLE && o.ObjectType == TIA_PROJECT_TAG_TABLE_TYPE);
            if (match == null)
                listDataBlocksAndTables.Add(new DBsNameAndTables(TIA_PROJECT_TAG_TABLE_TYPE, TIA_PROJECT_TAG_TABLE));
        }

        void ParseDataBlocksAndTables(ref stNode stnode, ref List<S7TIAImportParser.DBsNameAndTables> listDataBlocksAndTables, ref string symbol_path, ref int nrElementsTagTables, ref int nrDataBlocks)
        {
            int ret = AGL4.AGL40_SUCCESS;
            stnode.hierarchy_type = AGL4.HierarchyType.UNDEFINED;
            ret = ParseNodeDataBlocksAndTable(ref stnode, ref listDataBlocksAndTables, ref nrElementsTagTables, ref nrDataBlocks);
            if (ret != AGL4.AGL40_SUCCESS)
            {
                return;
            }

            if (stnode.hierarchy_type == AGL4.HierarchyType.STRUCTURE || stnode.hierarchy_type == AGL4.HierarchyType.ARRAY)
            {
                //Get number of child nodes for this node
                int child_count = 0;
                //int child_count_struct = 0;

                //get nr of elements to cicle and nr of valid elements for Movicon prototype
                //ret = GetChildCountMoviconTiaPortal(stnode, symbol_path, ref child_count, ref child_count_struct);
                ret = AGL4.Symbolic_GetChildCount(stnode.node, ref child_count);
                if (ret != AGL4.AGL40_SUCCESS)
                {
                    return;
                }

                //Iterate through all childs
                for (int child_index = 0; child_index < child_count; ++child_index)
                {
                    stNode stNodeIdent = new stNode(Protocol);
                    stNodeIdent.depth = stnode.depth;
                    stNodeIdent.ParentNode = stnode;
                    //stNodeIdent.depthStruct = stnode.depthStruct;
                    //struct of struct only for user's define data type
                    //if (stnode.system_type == AGL4.SystemType.S7_UDT_Instance)
                    //    stNodeIdent.childOfStruct = true;

                    stNodeIdent.LastChild = child_index;

                    ret = AGL4.Symbolic_GetChild(stnode.node, child_index, ref stNodeIdent.node);
                    if (ret != AGL4.AGL40_SUCCESS)
                    {
                        continue;
                    }

                    ParseDataBlocksAndTables(ref stNodeIdent, ref listDataBlocksAndTables, ref symbol_path, ref nrElementsTagTables, ref nrDataBlocks);
                }
            }

            //if (stnode.depthStruct > 0)
            //{
            //    stnode.depthStruct--;
            //}

            //Collapse childs to unload the childs in memory
            if (stnode.hierarchy_type == AGL4.HierarchyType.ARRAY)
            {
                ret = AGL4.Symbolic_Collapse(stnode.node);
                if (ret != AGL4.AGL40_SUCCESS)
                {
                    return;
                }
            }

        }
    
        /// Get all Informations for a specific node and read the value from the PLC
        int ParseNodeDataBlocksAndTable(ref stNode stnode, ref List<S7TIAImportParser.DBsNameAndTables> listDataBlocksAndTables, ref int nrElementsTagTables, ref int nrDataBlocks)
        {
            AGL4.SegmentType segment_type = AGL4.SegmentType.UNDEFINED;
            
            //Differentiate the type of a node. Is it a normal field node, an index or the root node.
            int ret = AGL4.Symbolic_GetSegmentType(stnode.node, ref segment_type);
            if (ret != AGL4Sym.AGLSYM_SUCCESS)
            {
                return (ret);
            }

            String name = String.Empty;
            if (segment_type == AGL4.SegmentType.FIELD)
            {
                ret = ParseNameInfo(ref stnode.node, ref name);
                if (ret != AGL4Sym.AGLSYM_SUCCESS)
                {
                    return (ret);
                }               
            }
            else if (segment_type == AGL4.SegmentType.ROOT)
            {
                ret = AGL4.Symbolic_GetProjectReferenceCulture(stnode.node, ref m_currentCulture);
            }
            else
            {
                return (AGL4Sym.AGLSYM_INVALID_ELEM_TYPE);
            }

            AGL4.SystemType system_scope_type = AGL4.SystemType.UNDEFINED;
            AGL4.SystemTypeState system_scope_type_state = AGL4.SystemTypeState.UNDEFINED;
            AGL4.TypeState type_state = AGL4.TypeState.UNDEFINED;
            ret = ParseNodeSchemaInfos(ref stnode.node, ref system_scope_type, ref system_scope_type_state, ref type_state, ref stnode.hierarchy_type);
            if (ret != AGL4Sym.AGLSYM_SUCCESS)
            {
                return (ret);
            }

            //Obtains the plc system data type
            ret = AGL4.Symbolic_GetSystemType(stnode.node, ref stnode.system_type);
            if (ret != AGL4Sym.AGLSYM_SUCCESS)
            {
                return (ret);
            }

            //If the node is inside a Tag table get extra informations about this Tag
            ret = AGL4.Symbolic_GetSystemType(stnode.node, ref stnode.system_type);
            if (ret != AGL4Sym.AGLSYM_SUCCESS)
            {
                return (ret);
            }
            if (stnode.system_type == AGL4.SystemType.S7_Datablock) 
            {
                //string nameAndType = string.Format("{0}{1}Datablock", name, FILE_FIELD_SEPARATOR);
                listDataBlocksAndTables.Add(new DBsNameAndTables(S7TIAImportParser.TIA_PROJECT_DATABLOCK_TYPE, name));
                nrDataBlocks++;
            }
            else if (stnode.system_type == AGL4.SystemType.S7_Tag_Table)
            {
                //string nameAndType = string.Format("{0}{1}Tag_Table", name, FILE_FIELD_SEPARATOR);
                //listDataBlocksAndTables.Add(nameAndType);
                listDataBlocksAndTables.Add(new DBsNameAndTables(S7TIAImportParser.TIA_PROJECT_TAG_TABLE_TYPE, name));
                nrElementsTagTables++;
            }
            return (ret);
        }
                
        public S7TIAImportParser(ImportSourceManagement importSource, string file, ProtocolType protocol = ProtocolType.TiaSymbolic)
        {
            Protocol = protocol;

            int ret;                        
            try
            {
                // Set the library license code
                AGL4.Activate("00BEF5-E1F5-260260");
            }
            catch
            {
                MessageBox.Show(Properties.Resources.AGLink40NotFound,
                                Properties.Resources.ImportMsgBoxTitle);
                return;
            }

            m_MapProgramsProgetTiaPortal = new Dictionary<string, IntPtr>();
            m_MapValueDepthStructToNumStruct = new Dictionary<string, string>();
            m_ListOfAllChilds = new List<stNode>();
            m_mapUDTBaseS7P = new Dictionary<string, stPrototype>();
            m_ImportSource = importSource;

            if (rootSchemaNodeHandle != IntPtr.Zero)
            {
                AGL4.Symbolic_FreeHandle(rootSchemaNodeHandle);
                rootSchemaNodeHandle = new IntPtr();
            }

            if(string.IsNullOrEmpty(file))
            {
                return;
            }

            ret = AGL4.Symbolic_LoadTIAProjectSymbols(file, ref rootSchemaNodeHandle, true);
            if (ret == AGL4Sym.AGLSYM_SUCCESS)
            {
                ret = AGL4.Symbolic_GetProjectReferenceCulture(rootSchemaNodeHandle, ref m_currentCulture);

                //Get number of child nodes for this node
                int child_count = 0;
                ret = AGL4.Symbolic_GetChildCount(rootSchemaNodeHandle, ref child_count);
                if (ret != AGL4Sym.AGLSYM_SUCCESS)
                {
                    //TRACE(_T("m_pAGL_Symbolic_GetChildCount Error number =  %d  line = %d\n"), nError, __LINE__);
                    return;
                }

                if (child_count > 0)
                {
                    programList = new Dictionary<int, string>();
                    //Iterate through all childs
                    for (int child_index = 0; child_index < child_count; ++child_index)
                    {
                        IntPtr child = new IntPtr();
                        ret = AGL4.Symbolic_GetChild(rootSchemaNodeHandle, child_index, ref child);
                        if (ret != AGL4Sym.AGLSYM_SUCCESS)
                        {
                            continue;
                        }
                        AGL4.SegmentType segmentType = AGL4.SegmentType.UNDEFINED;
                        //Differentiate the type of a node. Is it a normal field node, an index or the root node.
                        ret = AGL4.Symbolic_GetSegmentType(child, ref segmentType);
                        if (ret != AGL4Sym.AGLSYM_SUCCESS)
                        {
                            //TRACE(_T("m_pAGL_Symbolic_GetSegmentType Error number =  %d  line = %d\n"), nError, __LINE__);
                            return;
                        }

                        if (segmentType == AGL4.SegmentType.FIELD)
                        {
                            string node_name = String.Empty;
                            ret = ParseNameInfo(ref child, ref node_name);
                            if (ret != AGL4Sym.AGLSYM_SUCCESS)
                            {
                                //TRACE(_T("ParseNameInfo Error number =  %d  line = %d\n"), nError, __LINE__);
                                return;
                            }
                            programList.Add(programList.Count, node_name);
                            m_MapProgramsProgetTiaPortal[node_name] = child;
                        }
                    }
                }                
            } else
            {
                MessageBox.Show(string.Format("{0} - {1} {2}={3}", file, Properties.Resources.ImportFileFormatInvalid, Properties.Resources.ImportFileErrorCode,string.Format("0x{0:X}",ret)), Properties.Resources.ImportMsgBoxTitle);
                return;
            }
        }//End costructor

        public virtual void Dispose()
        {
            Clean();
        }

        void Clean()
        {
            if (rootSchemaNodeHandle != IntPtr.Zero)
            {
                AGL4.Symbolic_FreeHandle(rootSchemaNodeHandle);
                rootSchemaNodeHandle = new IntPtr();
            }

            m_MapProgramsProgetTiaPortal.Clear();
            m_MapValueDepthStructToNumStruct.Clear();
            if(programList != null)
            { 
                programList.Clear();
            }

            if(m_PrototipeVariableStruct != null)
            {
                foreach (var item in m_PrototipeVariableStruct)
                {
                    item.Value.Clear();
                }
                m_PrototipeVariableStruct.Clear();
            }

            if (m_mapUDTBaseS7P != null)
            {
                foreach (var item in m_mapUDTBaseS7P)
                {
                    item.Value.Values.Clear();
                }
                m_mapUDTBaseS7P.Clear();
            }

            if (m_MapImportVariable != null)
            {
                m_MapImportVariable.Clear();
            }
        }

        /// Get the name of the symbol and escape the name
        int ParseNameInfo(ref IntPtr node, ref string szName)
        {
            int ret = AGL4.AGL40_PARAMETER_ERROR;

            if (node == IntPtr.Zero)
            {
                return ret;
            }

            ret = AGL4.Symbolic_GetName(node, ref szName);

            if (ret != AGL4.AGL40_SUCCESS)
            {
                return ret;
            }

            String escapedString = String.Empty;
            Int32 errorPosition = 0;
            ret = AGL4.Symbolic_EscapeString(szName, ref escapedString, ref errorPosition);
            if (ret == AGL4Sym.AGLSYM_SUCCESS)
            {
                if (!string.IsNullOrEmpty(escapedString))
                    szName = escapedString;
            }           

            return (ret);
        }

        public Dictionary<int, string> GetListOfProgramPLC()
        {
            return (programList);
        }

        public IntPtr GetHandler()
        {
            return (rootSchemaNodeHandle);
        }

        public void ParsingSelectProgram(int selectProgra)
        {
            stNode stnode = new stNode(Protocol);
            stnode.stNodeInit(m_MapProgramsProgetTiaPortal[programList[selectProgra]]);

            ParseNodeTiaPortal(ref stnode);

            PopulationMapsForImport();
        }

        public void ParsingHandlerPointer(IntPtr _rootSchemaNodeHandle)
        {
            rootSchemaNodeHandle = _rootSchemaNodeHandle;
            stNode stnode = new stNode(Protocol);
            stnode.stNodeInit(_rootSchemaNodeHandle);

            ParseNodeTiaPortal(ref stnode);

            PopulationMapsForImport();
        }

        void TiaMemoryClear()
        {
            m_MapValueDepthStructToNumStruct.Clear();
            m_MapProgramsProgetTiaPortal.Clear();
            m_ListOfAllChilds.Clear();
        }

        void ParseNodeTiaPortal(ref stNode stnode)
        {
            string symbol_path = String.Empty;
            int ret = AGL4.AGL40_SUCCESS;
            stnode.hierarchy_type = AGL4.HierarchyType.UNDEFINED;
            ret = ParseNodeInfoTiaPortal(ref stnode, ref symbol_path);
            if (ret != AGL4.AGL40_SUCCESS)
            {
                return;
            }

            //If HierarchyType_t of this node is STRUCTURE or ARRAY parse the child nodes
            if (stnode.hierarchy_type == AGL4.HierarchyType.STRUCTURE || stnode.hierarchy_type == AGL4.HierarchyType.ARRAY)
            {
                //Get number of child nodes for this node
                int child_count = 0;
                //int child_count_struct = 0;

                if (IsAStructure(stnode.system_type))
                {
                    stnode.depth++;
                    //if (stnode.childOfStruct && stnode.system_type == AGL4.SystemType.S7_UDT_Instance)
                    //    stnode.depthStruct++;
                }

                if ((stnode.hierarchy_type == AGL4.HierarchyType.ARRAY) || ((stnode.matrix != null) && (stnode.matrix.Contains("]"))))
                {
                    stnode.hierarchy_type = AGL4.HierarchyType.ARRAY;
                    stnode.depth = 1;
                }

                ////get nr of elements to cicle and nr of valid elements for Movicon prototype
                //ret = GetChildCountMoviconTiaPortal(stnode, symbol_path,ref child_count, ref child_count_struct);
                ret = AGL4.Symbolic_GetChildCount(stnode.node, ref child_count);
                if (ret != AGL4.AGL40_SUCCESS)
                {
                    return;
                }


                //add structure's node and array's node
                stnode.NumChild = child_count; // child_count_struct;
                //m_ListOfAllChilds.Add(stnode);
                stNode tmp = stnode;
                bool checkstNorePresent = false;
                foreach(stNode currentStnode in m_ListOfAllChilds)
                {
                    if (currentStnode.name.Equals(tmp.name) && currentStnode.path.Equals(tmp.path))
                    {
                        checkstNorePresent = true;
                    }
                }

                if (!checkstNorePresent)
                {
                    m_ListOfAllChilds.Add(stnode);
                }
                else
                {
                    return;
                }

#if DEBUG
                //System.Diagnostics.Debug.WriteLine("CHECK {0} - {1} - {2} - {3} - {4} - {5} - {6} - {7} - {8} - {9} - {10} - {11} - {12} - {13} - {14}", stnode.path, stnode.name, stnode.objectName,
                //                                                                                                 stnode.NumChild, stnode.LastChild,
                //                                                                                                 stnode.system_type, stnode.hierarchy_type,
                //                                                                                                 stnode.value_type, stnode.permissionType,
                //                                                                                                 stnode.dimension, stnode.matrix,
                //                                                                                                 stnode.comment, stnode.string_size, stnode.udtName,
                //                                                                                                 child_count);
#endif
                //Iterate through all childs
                for (int child_index = 0; child_index < child_count; ++child_index)
                {
                    stNode stNodeIdent = new stNode(Protocol);
                    stNodeIdent.depth = stnode.depth;
                    stNodeIdent.ParentNode = stnode;
                    //stNodeIdent.depthStruct = stnode.depthStruct;
                    //struct of struct only for user's define data type
                    //if (stnode.system_type == AGL4.SystemType.S7_UDT_Instance)
                    //    stNodeIdent.childOfStruct = true;

                    stNodeIdent.LastChild = child_index;
                    stNodeIdent.path = symbol_path;

                    ret = AGL4.Symbolic_GetChild(stnode.node, child_index, ref stNodeIdent.node);
                    if (ret != AGL4.AGL40_SUCCESS)
                    {
                        continue;
                    }

                    ParseNodeTiaPortal(ref stNodeIdent);
                }

                //if (stnode.depthStruct > 0)
                //{
                //    stnode.depthStruct--;
                //}

                //Collapse childs to unload the childs in memory
                if (stnode.hierarchy_type == AGL4.HierarchyType.ARRAY)
                {
                    ret = AGL4.Symbolic_Collapse(stnode.node);
                    if (ret != AGL4.AGL40_SUCCESS)
                    {
                        return;
                    }
                }
            }
            else
            {
                switch (Protocol)
                {
                    case ProtocolType.TiaSymbolic:
                        if (stnode.system_type == AGL4.SystemType.S7_String)
                            stnode.depth = 1;
                        break;
                }

                stNode tmp = stnode;
                bool checkstNorePresent = false;
                Parallel.ForEach(m_ListOfAllChilds, (currentStnode) =>
                {
                    if (currentStnode.name.Equals(tmp.name))
                    {
                        checkstNorePresent = true;
                    }
                });

                if (!checkstNorePresent)
                {
                    m_ListOfAllChilds.Add(stnode);
                }
                else
                {
                    return;
                }


#if DEBUG
                //System.Diagnostics.Debug.WriteLine("CHECK {0} - {1} - {2} - {3} - {4} - {5} - {6} - {7} - {8} - {9} - {10} - {11} - {12}", stnode.path, stnode.name,
                //                                                                                                    stnode.NumChild, stnode.LastChild,
                //                                                                                                    stnode.system_type, stnode.hierarchy_type,
                //                                                                                                    stnode.value_type, stnode.permissionType,
                //                                                                                                    stnode.dimension, stnode.matrix,
                //                                                                                                    stnode.comment, stnode.string_size, stnode.udtName);
#endif
            }
        }


        /// <summary>
        /// Get address type (letter M,I or Q) of struct table's var; if not defined (sub struct), try to get data from parent
        /// </summary>
        /// <param name="stnode"></param>
        /// <param name="system_type"></param>
        static void S7TcpGetSystemTypeTableUdtSubStruct(ref stNode stnode, ref AGL4.SystemType system_type)
        {
            if (stnode != null)
            {
                if (!(stnode.S7Tcp_TagTableSystemType == AGL4.SystemType.S7_Memory || stnode.S7Tcp_TagTableSystemType == AGL4.SystemType.S7_Input || stnode.S7Tcp_TagTableSystemType == AGL4.SystemType.S7_Output))
                    S7TcpGetSystemTypeTableUdtSubStruct(ref stnode.ParentNode, ref system_type);
                else
                    system_type = stnode.S7Tcp_TagTableSystemType;
            }
        }

        /// <summary>
        /// Get tag table
        /// </summary>
        /// <param name="stnode"></param>
        /// <param name="system_type"></param>
        /// <returns></returns>
        private int S7TcpGetSystemTypeTableUdtStruct(ref stNode stnode, ref AGL4.SystemType system_type)
        {
            int ret = AGL4.Symbolic_GetSystemType(stnode.node, ref system_type);
            if (!(system_type == AGL4.SystemType.S7_Memory || system_type == AGL4.SystemType.S7_Input || system_type == AGL4.SystemType.S7_Output))
                S7TcpGetSystemTypeTableUdtSubStruct(ref stnode.ParentNode, ref system_type);

            return ret;
        }

        /// Get all Informations for a specific node and read the value from the PLC
        int ParseNodeInfoTiaPortal(ref stNode stnode, ref string symbol_path)
        {
            AGL4.SegmentType segment_type = AGL4.SegmentType.UNDEFINED;

            //Differentiate the type of a node. Is it a normal field node, an index or the root node.
            int ret = AGL4.Symbolic_GetSegmentType(stnode.node, ref segment_type);
            if (ret != AGL4Sym.AGLSYM_SUCCESS)
            {
                return (ret);
            }

            String path_extend = String.Empty;
            if (segment_type == AGL4.SegmentType.FIELD)
            {
                if (stnode.depth > 1)
                {
                    path_extend += ".";
                }
                string node_name = String.Empty;
                ret = ParseNameInfo(ref stnode.node, ref node_name);
                if (ret != AGL4Sym.AGLSYM_SUCCESS)
                {
                    return (ret);
                }
                path_extend += node_name;
                stnode.objectName = node_name;
            }
            else if (segment_type == AGL4.SegmentType.INDEX)
            {
                string szIndex = String.Empty;
                ret = ParseIndexArray(ref stnode.node, ref szIndex);
                path_extend += szIndex;
            }
            else if (segment_type == AGL4.SegmentType.ROOT)
            {
                ret = AGL4.Symbolic_GetProjectReferenceCulture(stnode.node, ref m_currentCulture);
            }
            else
            {
                return (AGL4Sym.AGLSYM_INVALID_ELEM_TYPE);
            }

            if (stnode.depth > 0)
            {
                string stNameEacape = String.Empty;
                ret = AGL4.Symbolic_GetEscapedPath(stnode.node, ref stNameEacape);
                if (ret == AGL4.AGL40_SUCCESS)
                {

                    if((!string.IsNullOrEmpty(stNameEacape)) && (stNameEacape[stNameEacape.Length - 1] == '.'))
                    {
                        stNameEacape = stNameEacape.Substring(0, stNameEacape.Length - 1);
                    }

                    symbol_path = stNameEacape;
                    stnode.name = stNameEacape;
                }
            }

            AGL4.SystemType system_scope_type = AGL4.SystemType.UNDEFINED;
            AGL4.SystemTypeState system_scope_type_state = AGL4.SystemTypeState.UNDEFINED;
            AGL4.TypeState type_state = AGL4.TypeState.UNDEFINED;
            ret = ParseNodeSchemaInfos(ref stnode.node, ref system_scope_type, ref system_scope_type_state, ref type_state, ref stnode.hierarchy_type);
            if (ret != AGL4Sym.AGLSYM_SUCCESS)
            {
                return (ret);
            }

            //Obtains the plc system data type
            ret = AGL4.Symbolic_GetSystemType(stnode.node, ref stnode.system_type);
            if (ret != AGL4Sym.AGLSYM_SUCCESS)
            {
                return (ret);
            }

            //If the node is inside a Tag table get extra informations about this Tag
            if (system_scope_type == AGL4.SystemType.S7_Tag_Table)
            {
                ret = ParseTagInfosTagTable(ref stnode.node, ref stnode.system_type);
                if (ret != AGL4Sym.AGLSYM_SUCCESS)
                {
                    return (ret);
                }

                if (stnode.system_type == AGL4.SystemType.S7_UDT || IsAStructure(stnode.system_type))
                {
                    ret = S7TcpGetSystemTypeTableUdtStruct(ref stnode, ref stnode.S7Tcp_TagTableSystemType);
                }
                else
                //if (stnode.system_type != AGL4.SystemType.S7_UDT && stnode.system_type != AGL4.SystemType.S7_UDT_Instance && stnode.system_type != AGL4.SystemType.S7_Struct)
                {
                    ret = ParseValueType(ref stnode, ref stnode.permissionType, ref stnode.value_type);
                    if (ret != AGL4Sym.AGLSYM_SUCCESS)
                    {
                        return (ret);
                    }
                    else if (ret >= 0)
                    {
                        if (stnode.value_type != AGL4.ValueType.UNDEFINED)
                        {

                            if (!GetTheDisplayAttribute(ref stnode.node, segment_type, stnode.system_type))
                            {
                                return (AGL4.AGL40_SUCCESS);
                            }

                            string szComment = " ";
                            ret = GetComment(ref stnode.node, ref szComment);
                        }
                    }
                }                
            }

            //If the node is a string the extra informations about this string
            if (stnode.system_type == AGL4.SystemType.S7_String || stnode.system_type == AGL4.SystemType.S7_WString)
            {
                ret = ParseStringInfos(ref stnode.node, ref stnode.string_size);
                if (ret != AGL4Sym.AGLSYM_SUCCESS)
                {
                    return (ret);
                }
            }

            switch (stnode.system_type)
            {
                case AGL4.SystemType.S7_Datablock:
                    switch (Protocol)
                    {
                        case ProtocolType.S7Tcp:
                            // don't process optimized data block 
                            ret = AGL4.Symbolic_DatablockIsSymbolic(stnode.node, out int booleanValue);
                            if (ret != AGL4Sym.AGLSYM_SUCCESS || booleanValue == 1)
                                return AGL4Sym.AGLSYM_INVALID_BLKTYPE;

                            ret = AGL4.Symbolic_DatablockGetNumber(stnode.node, out currentDBNr);
                            if (ret == AGL4Sym.AGLSYM_SUCCESS)
                                stnode.S7Tcp_DBNr = currentDBNr;
                            else
                                currentDBNr = 0;
                            break;
                    }
                    break;
                case AGL4.SystemType.S7_Tag_Table:
                    switch (Protocol)
                    {
                        case ProtocolType.S7Tcp:
                            currentDBNr = S7TCP_TAG_TABLE_DB;
                            stnode.S7Tcp_DBNr = currentDBNr;
                            break;
                    }
                    break;
            }

            //if (system_scope_type == AGL4.SystemType.S7_Datablock)
            if (system_scope_type == AGL4.SystemType.S7_Datablock || system_scope_type == AGL4.SystemType.S7_Tag_Table) {                
                if ((stnode.hierarchy_type == AGL4.HierarchyType.STRUCTURE))
                {
                    if (system_scope_type == AGL4.SystemType.S7_Datablock)
                    {
                        //Obtains the plc system data type
                        ret = AGL4.Symbolic_GetSystemType(stnode.node, ref stnode.system_type);
                        if (ret != AGL4Sym.AGLSYM_SUCCESS)
                        {
                            return (ret);
                        }
                        switch (Protocol)
                        {
                            case ProtocolType.S7Tcp:
                                uint byteOffset = 0;
                                uint bitOffset = 0;
                                ret = AGL4.Symbolic_GetLocalOffset(stnode.node, ref byteOffset, ref bitOffset);
                                if (ret == AGL4Sym.AGLSYM_SUCCESS)
                                {
                                    //stnode.DataRW.DBNr = (ushort)currentDBNr;
                                    stnode.S7Tcp_Offset = (ushort)byteOffset;
                                    //System.Diagnostics.Debug.WriteLine(string.Format("Symbolic_GetLocalOffset{0} Offset {1}", stnode.name, stnode.S7Tcp_Offset));
                                }
                                break;
                        }

                    }

                    if (stnode.system_type == AGL4.SystemType.S7_UDT_Instance)
                    {
                        GetUDTName(stnode.node, ref stnode);
                    }
                    else if (stnode.system_type == AGL4.SystemType.S7_UDT)
                    {
                        GetUDTName(stnode.node, ref stnode);
                    }
                    if (ret == AGL4Sym.AGLSYM_SUCCESS)
                    {
                        return (ret);
                    }
                    //if(stnode.name[stnode.name.Length -1] =='.')
                    //{
                    //    stnode.name = stnode.name.Substring(0, stnode.name.Length - 1);
                    //}
                }
                if (stnode.hierarchy_type == AGL4.HierarchyType.ARRAY)
                {
                    switch (Protocol)
                    {
                        case ProtocolType.S7Tcp:
                            uint byteOffset = 0;
                            uint bitOffset = 0;
                            ret = AGL4.Symbolic_GetLocalOffset(stnode.node, ref byteOffset, ref bitOffset);
                            if (ret == AGL4Sym.AGLSYM_SUCCESS)
                            {                                
                                stnode.S7Tcp_Offset = (ushort)byteOffset;
                                //System.Diagnostics.Debug.WriteLine(string.Format("Symbolic_GetLocalOffset {0} Offset {1}", stnode.name, stnode.S7Tcp_Offset));
                            }
                            break;
                    }

                    string szArrayDimensions = string.Empty;
                    stOffset[] OffsetArray = new stOffset[4];
                    uint Dimension = 0;
                    uint ArrayDimension = 1;
                    ret = ParseArrayDimensions(ref stnode, ref szArrayDimensions, ref Dimension, ref OffsetArray, ref ArrayDimension);
                    if (ret != AGL4Sym.AGLSYM_SUCCESS)
                    {
                        return (ret);
                    }

                    stnode.dimension = Dimension;
                    stnode.matrix = szArrayDimensions;
                    IntPtr array_type_node = new IntPtr();
                    //Determine the node to the base type for complex nodes like Array or UDT.
                    //I.e. for an array of struct the struct node. For an UDT-Instance the UDT node. 
                    //For a S7-Memory Tag of bool the bool.
                    ret = AGL4.Symbolic_GetType(stnode.node, ref array_type_node);
                    if (ret != AGL4Sym.AGLSYM_SUCCESS)
                    {
                        return (ret);
                    }

                    //Obtains the plc system data type
                    ret = AGL4.Symbolic_GetSystemType(array_type_node, ref stnode.system_type);
                    if (ret != AGL4Sym.AGLSYM_SUCCESS)
                    {
                        return (ret);
                    }

                    if ((stnode.system_type == AGL4.SystemType.S7_String) ||
                    (stnode.system_type == AGL4.SystemType.S7_WString))
                    {
                        ret = ParseStringInfos(ref array_type_node, ref stnode.string_size);
                        if (ret != AGL4Sym.AGLSYM_SUCCESS)
                        {
                            return (ret);
                        }
                    }

                    switch (Protocol)
                    {
                        case ProtocolType.S7Tcp:
                            //int arraySizeByte = 0;
                            //if (stnode.system_type == AGL4.SystemType.S7_Bool)
                            //{
                            //    arraySizeByte = Math.DivRem((int)ArrayDimension, 8, out int bitRest);
                            //    if (bitRest > 0)
                            //        arraySizeByte++;
                            //}
                            //else
                            //{
                            //    arraySizeByte = (int)(GetStandardDataTypeSize(stnode.system_type) * ArrayDimension);
                            //}

                            //// array's byte address are always even 
                            //if (arraySizeByte % 2 != 0)
                            //    arraySizeByte++;

                            //int arraySizeByte = 0;
                            //if (stnode.system_type == AGL4.SystemType.S7_Bool)
                            //{
                            //    arraySizeByte = (int)ArrayDimension;
                            //}
                            //else
                            //{
                            //    arraySizeByte = (int)(GetStandardDataTypeSize(stnode.system_type) * ArrayDimension);
                            //}

                            stnode.UpdateStructSize(stnode.system_type, OffsetArray[0].nStartArray, stnode.S7Tcp_BitNr, (int)ArrayDimension, stnode.string_size);
                            break;
                    }

                    if (stnode.system_type == AGL4.SystemType.S7_UDT_Instance)
                    {
                        GetUDTName(array_type_node, ref stnode);
                    }
                    if ((IsAStructure(stnode.system_type)))
                    {
                        stNode starray_type_node = stnode;
                        starray_type_node.node = array_type_node;
                        ParseNodeTiaPortal(ref starray_type_node);
                    }
                    //if (ret == -1)
                    //{
                    //    stnode.depthStruct++;
                    //}
                }
                else
                {
                    //stnode.depthStruct = 0;
                    ret = ParseValueType(ref stnode, ref stnode.permissionType, ref stnode.value_type);
                    if (ret != AGL4Sym.AGLSYM_SUCCESS)
                    {
                    //    stnode.depthStruct = 0;
                        return (ret);
                    }

                    if (!GetTheDisplayAttribute(ref stnode.node, segment_type, stnode.system_type))
                    {
                        return (AGL4.AGL40_SUCCESS);
                    }

                    string szComment = " ";
                    ret = GetComment(ref stnode.node, ref szComment);
                    if (ret == AGL4Sym.AGLSYM_SUCCESS)
                    {
                        stnode.comment = szComment;
                    }

                    if (path_extend.IndexOf("[") == 0)
                    {
                        ret = AGL4Sym.AGLSYM_SUCCESS;
                    }
                    else if (stnode.value_type != AGL4.ValueType.UNDEFINED)
                    {
                        if (ret == AGL4Sym.AGLSYM_SUCCESS)
                        {
                            return (ret);
                        }
                    }
                }
            }
            return (ret);
        }

        /// Get the SystemType of the variable the Tag is pointing to
        int ParseTagInfos(ref IntPtr node, ref AGL4.SystemType tag_system_type)
        {
            IntPtr tag_type = new IntPtr();
            //Determine the node to the base type for complex nodes like Array or UDT.
            //I.e. for an array of struct the struct node. For an UDT-Instance the UDT node. 
            //For a S7-Memory Tag of bool the bool.
            int ret = AGL4.Symbolic_GetType(node, ref tag_type);
            if (ret != AGL4Sym.AGLSYM_SUCCESS)
            {
                return (ret);
            }

            tag_system_type = AGL4.SystemType.UNDEFINED;
            //Obtains the plc system data type
            ret = AGL4.Symbolic_GetSystemType(tag_type, ref tag_system_type);
            if (ret != AGL4Sym.AGLSYM_SUCCESS)
            {
                return (ret);
            }
            return (ret);
        }

        /// Get the SystemType of the variable the Tag is pointing to
        int ParseTagInfosTagTable(ref IntPtr node, ref AGL4.SystemType tag_system_type)
        {
            IntPtr tag_type = new IntPtr();
            //Determine the node to the base type for complex nodes like Array or UDT.
            //I.e. for an array of struct the struct node. For an UDT-Instance the UDT node. 
            //For a S7-Memory Tag of bool the bool.
            int ret = AGL4.Symbolic_GetType(node, ref tag_type);
            if (ret == AGL4Sym.AGLSYM_SUCCESS)
            {
                tag_system_type = AGL4.SystemType.UNDEFINED;
                //Obtains the plc system data type
                ret = AGL4.Symbolic_GetSystemType(tag_type, ref tag_system_type);
            } 
            if (ret != AGL4Sym.AGLSYM_SUCCESS)
            {
                ret = AGL4.Symbolic_GetSystemType(node, ref tag_system_type);
            }

            //AGlink library sometimes mismatch UDT data type inside tag tables
            if (tag_system_type == AGL4.SystemType.S7_UDT)
                tag_system_type = AGL4.SystemType.S7_UDT_Instance;

            return (ret);
        }

        void AddPrototypeStructure(stNode node, bool isChildren = false)
        {
            string nameElement;
            string nameStructure;
            if (IsAStructure(node.system_type))
            {
                nameStructure = node.udtName;
                if (!m_mapUDTBaseS7P.ContainsKey(nameStructure))
                {
                    //List<string> StringArray = new List<string>();
                    stPrototype Proto = new stPrototype();
                    Proto.udtName = nameStructure;
                    Proto.Values = new List<string>();
                    Proto.numRealChildren = 0;                    
                    m_mapUDTBaseS7P.Add(nameStructure, Proto);
                }
                if (isChildren == true)
                {
                    nameElement = node.udtName;
                    nameStructure = node.path;
                    string szType = string.Empty;
                    string szTypeStruct = string.Empty;
                    uint nVarSize = 0;
                    UFUAModel.DataType nType = GetMoviconTypeIdFromTiaPortalType(node.system_type, ref nVarSize, ref szType, ref szTypeStruct);
                    if (m_mapUDTBaseS7P.ContainsKey(nameStructure))
                    {
                        stPrototype Proto = m_mapUDTBaseS7P[nameStructure];
                        string value = string.Format("{0} : {1} ;{2}", nameElement, szTypeStruct, 0);
                        if (Proto.Values.Contains(value))
                        {
                            Proto.Values = new List<string>();                            
                            Proto.numRealChildren = 0;
                        }

                        if (!Proto.Values.Contains(value) && !node.MovedToRoot())
                        {
                            Proto.numRealChildren++;
                            Proto.Values.Add(value);
                        }
                                                
                        m_mapUDTBaseS7P[nameStructure] = Proto;
                    }
                }
            }
            else if (node.hierarchy_type == AGL4.HierarchyType.ITEM)
            {                
                nameElement = node.name.Substring(node.path.Length + 1);
                nameStructure = node.udtName;
                string szType = string.Empty;
                string szTypeStruct = string.Empty;
                uint nVarSize = 0;
                UFUAModel.DataType nType = GetMoviconTypeIdFromTiaPortalType(node.system_type, ref nVarSize, ref szType, ref szTypeStruct);
                if (m_mapUDTBaseS7P.ContainsKey(nameStructure))
                {
                    stPrototype Proto = m_mapUDTBaseS7P[nameStructure];                    
                    string value = string.Format("{0} : {1} ;{2}", nameElement, szTypeStruct, 0);                    
                    if (Proto.Values.Contains(value))
                    {
                        Proto.Values = new List<string>();
                        Proto.numRealChildren = 0;
                    }

                    // add non existing element and not moved to root element (like string, ecc)
                    if (!Proto.Values.Contains(value) && !node.MovedToRoot()) {
                        Proto.numRealChildren++;
                        Proto.Values.Add(value);
                    }                    
                                        
                    m_mapUDTBaseS7P[nameStructure] = Proto;
                }                
            }
        }

        /// Get string length
        int ParseStringInfos(ref IntPtr node, ref int string_size)
        {
            int res = AGL4.Symbolic_GetMaxStringSize(node, ref string_size);
            return (res);
        }

        /// Get Meta Infos about the Node

        int ParseNodeSchemaInfos(ref IntPtr node, ref AGL4.SystemType system_scope_type, ref AGL4.SystemTypeState system_scope_type_state,
            ref AGL4.TypeState type_state, ref AGL4.HierarchyType hierarchy_type)
        {
            //Get the area where the node is located. I.e. S7_Datablock, S7_Tag_Table or S7_UDT.
            int ret = AGL4.Symbolic_GetSystemScope(node, ref system_scope_type);
            if (ret != AGL4.AGL40_SUCCESS)
            {
                return (ret);
            }

            //Get the system state of the symbol. Means VALID, INVALID or S7_DATABLOCK_NOT_COMPILED
            ret = AGL4.Symbolic_GetSystemTypeState(node, ref system_scope_type_state);
            if (ret != AGL4.AGL40_SUCCESS)
            {
                return (ret);
            }

            //Get the state of a symbol. Means valid, invalid or not supported.
            ret = AGL4.Symbolic_GetTypeState(node, ref type_state);
            if (ret != AGL4.AGL40_SUCCESS)
            {
                return (ret);
            }

            //Defines whether it's a structure, array or a single item
            AGL4.HierarchyType nTmphierarchy_type = hierarchy_type;
            ret = AGL4.Symbolic_GetHierarchyType(node, ref nTmphierarchy_type);
            if (ret != AGL4.AGL40_SUCCESS)
            {
                return (ret);
            }
            hierarchy_type = nTmphierarchy_type;
            return (ret);
        }        /// Get Array Index informations and format it like  '[1,2,3]'

        int ParseIndexArray(ref IntPtr node, ref string szIndex)
        {
            uint index_size = 0;
            StringBuilder stream = new StringBuilder(string.Empty);

            int ret = AGL4.Symbolic_GetIndexSize(node, ref index_size);
            if (ret != AGL4.AGL40_SUCCESS)
            {
                return (ret);
            }

            stream.Append('[');
            for (int i = 0; i < index_size; ++i)
            {
                int value = 0;
                //Get the system specific value of an index component. I.e. for [1,2,4] the single value 1, 2 or 4.
                ret = AGL4.Symbolic_GetIndex(node, i, ref value);
                if (ret != AGL4.AGL40_SUCCESS)
                {
                    return (ret);
                }                
                stream.Append(value);
                if (i < index_size - 1)
                {
                    stream.Append(',');
                }
            }
            stream.Append(']');
            szIndex = stream.ToString();
            return (ret);
        }

        /// The ValueType of the node
        int ParseValueType(ref stNode stnode, ref AGL4.PermissionType permissionType, ref AGL4.ValueType value_type)
        {
            value_type = AGL4.ValueType.UNDEFINED;

            /*Determines the data type that is needed to map the system type to a value on a pc. I.e. z.b S7-Bool => UInt8, S7-Int => Int16 und ULInt => UInt64.
              Types that cannot be mapped directly will be "SystemSpecific" i.e. S7-DTL. For those types there are special converter.*/
            int ret = AGL4.Symbolic_GetValueType(stnode.node, ref value_type);
            if (ret != AGL4Sym.AGLSYM_SUCCESS)
            {
                return ret;
            }

            switch (value_type)
            {
                case AGL4.ValueType.UINT8:
                case AGL4.ValueType.UINT16:
                case AGL4.ValueType.UINT32:
                case AGL4.ValueType.UINT64:

                case AGL4.ValueType.INT8:
                case AGL4.ValueType.INT16:
                case AGL4.ValueType.INT32:
                case AGL4.ValueType.INT64:
                case AGL4.ValueType.SYSTEM_SPECIFIC:

                case AGL4.ValueType.CHAR8:
                case AGL4.ValueType.CHAR16:
                case AGL4.ValueType.CHAR32:

                case AGL4.ValueType.STRING8:
                case AGL4.ValueType.STRING16:
                case AGL4.ValueType.STRING32:

                case AGL4.ValueType.FLOAT32:
                case AGL4.ValueType.FLOAT64:
                    {
                        permissionType = AGL4.PermissionType.UNDEFINED;
                        //Get information about read or write permissions for a node.
                        ret = AGL4.Symbolic_GetPermissionType(stnode.node, ref permissionType);

                        if ((permissionType != AGL4.PermissionType.NONE) && (permissionType != AGL4.PermissionType.UNDEFINED))
                        {
                            switch (Protocol) {
                                case ProtocolType.TiaSymbolic:
                                    return (ret);
                                case ProtocolType.S7Tcp:
                                    // Get address type (letter M, I or Q) of tag table's var
                                    if (currentDBNr == S7TCP_TAG_TABLE_DB)
                                        ret = AGL4.Symbolic_GetSystemType(stnode.node, ref stnode.S7Tcp_TagTableSystemType);

                                    int size = 0;
                                    AGL4.DATA_RW40 TmpDataRW = new AGL4.DATA_RW40();
                                    ret = AGL4.Symbolic_Get_DATA_RW40(stnode.node, ref TmpDataRW, ref size);
                                    if (ret != AGL4Sym.AGLSYM_SUCCESS)
                                    {
                                        return (ret);
                                    }

                                    if (IsAStructure(stnode.system_type) || (stnode.ParentNode != null && IsAStructure(stnode.ParentNode.system_type)))
                                    {                                       
                                        uint byteOffset = 0;
                                        uint bitOffset = 0;
                                        ret = AGL4.Symbolic_GetLocalOffset(stnode.node, ref byteOffset, ref bitOffset);
                                        if (ret != AGL4Sym.AGLSYM_SUCCESS)
                                        {
                                            return (ret);
                                        }
                                        stnode.S7Tcp_Offset = (int)byteOffset;
                                        stnode.S7Tcp_BitNr = (int)bitOffset;
                                        //System.Diagnostics.Debug.WriteLine(string.Format("Symbolic_GetLocalOffset {0} Offset {1}", stnode.name, stnode.S7Tcp_Offset));
                                    }
                                    else
                                    {                                        
                                        stnode.S7Tcp_Offset = TmpDataRW.Offset;
                                        stnode.S7Tcp_BitNr = TmpDataRW.BitNr;
                                        //System.Diagnostics.Debug.WriteLine(string.Format("Symbolic_Get_DATA_RW40 {0} Offset {1}", stnode.name, stnode.S7Tcp_Offset));
                                    }

                                    stnode.UpdateStructSize(stnode.system_type, stnode.S7Tcp_Offset, stnode.S7Tcp_BitNr, size, stnode.string_size);

                                    return (ret);                                    
                            }
                        }
                    }
                    break;
                default:
                    break;
            }
            return (-1);
        }

        bool GetTheDisplayAttribute(ref IntPtr itemHandle, AGL4.SegmentType segment_type, AGL4.SystemType system_type)
        {

            bool has_node_hmi_attributes = segment_type == AGL4.SegmentType.FIELD &&
                                            system_type != AGL4.SystemType.S7_Datablock &&
                                            system_type != AGL4.SystemType.S7_Datablock_Group &&
                                            system_type != AGL4.SystemType.S7_UDT_Group &&
                                            system_type != AGL4.SystemType.S7_Tag_Table &&
                                            system_type != AGL4.SystemType.S7_Tag_Group &&
                                            system_type != AGL4.SystemType.Group &&
                                            system_type != AGL4.SystemType.S7_PLC;
            if (!has_node_hmi_attributes)
            {
                return (true);
            }

            bool hmi_accessible = false;
            int res = AGL4.Symbolic_GetAttributeHMIAccessible(itemHandle, out hmi_accessible);
            if (res < AGL4.AGL40_SUCCESS)
            {
                return (true);
            }
            return (hmi_accessible);
        }

        int GetComment(ref IntPtr itemHandle, ref string commentString)
        {
            int cultureCount = 0;
            int res = AGL4.Symbolic_GetCommentCultureCount(itemHandle, ref cultureCount);
            if (res != AGL4.AGL40_SUCCESS)
            {
                return res;
            }
            if (cultureCount > 0)
            {

                res = AGL4.Symbolic_GetComment(itemHandle, m_currentCulture, ref commentString);
                if (res != AGL4.AGL40_SUCCESS)
                {
                    if (res == AGL4.AGL40_SYMBOLIC_NOT_APPLICABLE)
                    {
                        res = AGL4.AGL40_SUCCESS;
                    }
                    else
                    {
                        return res;
                    }
                }
            }
            return res;
        }
                
        int ParseArrayDimensions(ref stNode stnode, ref string arrayDimensions, ref uint Dimension,
                                 ref stOffset[] st_OffsetArray, ref uint ArrayDimension)
        {
            StringBuilder stream = new StringBuilder(string.Empty);

            int dimension_count = 0;
            //Determines the count of dimensions of an array. I.e. Array[1..2] of int => 1. Array[1..2, 1..2] of int => 2.
            int ret = AGL4.Symbolic_GetArrayDimensionCount(stnode.node, ref dimension_count);
            if (ret != AGL4.AGL40_SUCCESS)
            {
                return (ret);
            }
            Dimension = (uint)dimension_count;
            if (Dimension > 0)
                st_OffsetArray = new stOffset[Dimension];

            stream.Append('[');
            ArrayDimension = 1;
            for (int i = 0; i < dimension_count; ++i)
            {
                int lower = 0;
                int upper = 0;
                //Determine the lower and upper index value of an array. Array[1..20] of int => {1, 20}. 
                //Array[-1..20, 1..10] of int => for dimension 0 = {-1, 20}, or for dimension 1 = {1, 10}
                ret = AGL4.Symbolic_GetArrayDimension(stnode.node, i, ref lower, ref upper);
                if (ret != AGL4.AGL40_SUCCESS)
                {
                    return (ret);
                }

                switch (Protocol)
                {
                    case ProtocolType.S7Tcp:
                        uint byteOffset = 0;
                        uint bitOffset = 0;
                        ret = AGL4.Symbolic_GetLocalOffset(stnode.node, ref byteOffset, ref bitOffset);
                        if (ret != AGL4Sym.AGLSYM_SUCCESS)
                        {
                            stnode.S7Tcp_DBNr = (ushort)currentDBNr;
                            stnode.S7Tcp_Offset = (ushort)byteOffset;
                            //System.Diagnostics.Debug.WriteLine(string.Format("Symbolic_GetLocalOffset{0} Offset {1}", stnode.name, stnode.S7Tcp_Offset));
                        }
                        break;
                }

                st_OffsetArray[i].nStartArray = lower;
                st_OffsetArray[i].nHigt = upper;
                st_OffsetArray[i].nSizeArray = (upper - lower) + 1;
                ArrayDimension = (uint)(ArrayDimension * st_OffsetArray[i].nSizeArray);
                stream.Append(string.Format("{0}..{1}",lower,upper));
                if (i < dimension_count - 1)
                {
                    stream.Append(", ");
                }
            }
            stream.Append(']');
            arrayDimensions = stream.ToString();

            return (ret);
        }

        bool GetUDTName(IntPtr node, ref stNode stnode)
        {
            IntPtr udtHandle = new IntPtr();
            int ret = AGL4.Symbolic_GetType(node, ref udtHandle);
            if (ret != AGL4Sym.AGLSYM_SUCCESS)
            {
                return (false);
            }

            string udt_name = string.Empty;
            ret = AGL4.Symbolic_GetName(udtHandle, ref udt_name);
            if (ret != AGL4Sym.AGLSYM_SUCCESS)
            {
                return (false);
            }
            stnode.udtName = udt_name;

            switch (Protocol)
            {
                case ProtocolType.S7Tcp:                            
                    uint byteOffset = 0;
                    uint bitOffset = 0;
                    ret = AGL4.Symbolic_GetLocalOffset(stnode.node, ref byteOffset, ref bitOffset);
                    if (ret == AGL4Sym.AGLSYM_SUCCESS)
                    {
                        //stnode.DataRW.DBNr = (ushort)currentDBNr;
                        stnode.S7Tcp_Offset = (ushort)byteOffset;
                        //System.Diagnostics.Debug.WriteLine(string.Format("Symbolic_GetLocalOffset{0} Offset {1}", udt_name, stnode.S7Tcp_Offset));
                    }
                    break;

            }            
            return true;
        }

        public struct StructParameter
        {
            public int parent;
            public int prew_parent;
            public int numRealChildren;
            public int numChildren;     // nr processed elements of struct
            public string nameUdt;      // nr elements realy present if the struct (array, string, strange name was moved for driver's requirement)
            public string prew_ParentName;
            public bool IsPrototype;
            public uint depth;
        }
        
        private void ExpandInMap(List<stNode> listOfAllChilds, ref int globalIID, ref int parent, ref int element_index, ref Dictionary<string, StructParameter> map_ListChildrenStructure, ref S7TcpAddress s7TCPOffset)
        {
            bool udtGroup = false;

            while (element_index < listOfAllChilds.Count) {

                stNode child = listOfAllChilds.ElementAt(element_index);

                s7TCPOffset.AddLevel(child);

                //Condition for not inserting the ptototypes in the list of tags.
                if ((child.hierarchy_type == AGL4.HierarchyType.STRUCTURE) && (child.system_type == AGL4.SystemType.S7_UDT))
                {
                    element_index++;
                    udtGroup = true;
                    continue;
                }
                if ((child.hierarchy_type == AGL4.HierarchyType.STRUCTURE) && (child.system_type == AGL4.SystemType.S7_Tag_Table))
                {
                    udtGroup = false;
                }
                if (udtGroup)
                {
                    element_index++;
                    continue;
                }

                if ((child.hierarchy_type == AGL4.HierarchyType.STRUCTURE) && (IsAStructure(child.system_type)))
                {
                    if ((child.matrix != null) && (child.matrix.IndexOf("]")) == child.matrix.Length - 1)
                        AddPrototype(child, ref parent, globalIID, ref map_ListChildrenStructure);
                    else
                        AddStructure(ref element_index, listOfAllChilds, child, ref parent, ref globalIID, ref map_ListChildrenStructure, ref s7TCPOffset);
                }

                if (child.hierarchy_type == AGL4.HierarchyType.ITEM)
                {                    
                    AddItemMain(globalIID, ref parent, child, ref map_ListChildrenStructure, ref s7TCPOffset);
                }

                if ((child.hierarchy_type == AGL4.HierarchyType.ARRAY && (IsAStructure(child.system_type)))
                    || (child.hierarchy_type == AGL4.HierarchyType.STRUCTURE && (!string.IsNullOrEmpty(child.matrix))))
                {                    
                    AddArrayOfStructure( listOfAllChilds, ref globalIID, ref parent, child, ref element_index, ref map_ListChildrenStructure, ref s7TCPOffset);
                }
                else if (child.hierarchy_type == AGL4.HierarchyType.ARRAY)
                {                    
                    AddArray(ref globalIID, ref parent, child, ref element_index, ref map_ListChildrenStructure, ref s7TCPOffset);
                }

                globalIID++;
                element_index++;
            }
        }

        void RemoveMarkedElement()
        {
            //remove unncessary tag; remove later to avoid dictionary unsorted key insert
            foreach (var tag in m_MapImportVariable.Where(a => a.Value.ToDelete).ToList().AsParallel())
                m_MapImportVariable.Remove(tag.Key);
        }

        void PopulationMapsForImport()
        {
            if ((m_ListOfAllChilds == null) && (m_ListOfAllChilds.Count == 0))
            {
                return;
            }

            if (m_MapImportVariable == null)
            {
                m_MapImportVariable = new Dictionary<long, ImportVariable>();
            }
            else
            {
                m_MapImportVariable.Clear();
            }

            if (m_mapUDTBaseS7P == null)
            {
                m_mapUDTBaseS7P = new Dictionary<string, stPrototype>();
            }
            else
            {
                m_mapUDTBaseS7P.Clear();
            }

            if (m_PrototipeVariableStruct == null)
            {
                m_PrototipeVariableStruct = new Dictionary<string, List<stNode>>();
            }
            else
            {
                m_PrototipeVariableStruct.Clear();
            }


            //foreach (var stnode in m_ListOfAllChilds)
            //{
            //    System.Diagnostics.Debug.WriteLine("CHECK {0} - {1} - {2} - {3} - {4} - {5} - {6} - {7} - {8} - {9} - {10} - {11} - {12} - {13} - {14}", stnode.path, stnode.name, stnode.ojectName,
            //                                                                                     stnode.NumChild, stnode.LastChild,
            //                                                                                     stnode.system_type, stnode.hierarchy_type,
            //                                                                                     stnode.value_type, stnode.permissionType,
            //                                                                                     stnode.dimension, stnode.matrix,
            //                                                                                     stnode.comment, stnode.string_size, stnode.udtName, stnode.depth); //, stnode.depthStruct);
            //}

            int parent = -1;
            int globalIID = 0;
            int index = 0;
            S7TcpAddress offset = new S7TcpAddress();
            Dictionary<string, StructParameter> map_ListChildrenStructure = new Dictionary<string, StructParameter>();
            ExpandInMap(m_ListOfAllChilds, ref globalIID, ref parent, ref index, ref map_ListChildrenStructure, ref offset);

            RemoveMarkedElement();            
        }

        bool StandarStruttureTia(AGL4.SystemType struct_system_type, ref string Struct)
        {
            bool IsAStruct = false;
            if (struct_system_type == AGL4.SystemType.S7_DTL)
            {
                IsAStruct = true;
                Struct = string.Format("S7_DTL");
            }
            else if (struct_system_type == AGL4.SystemType.S7_IEC_Counter)
            {
                IsAStruct = true;
                Struct = "S7_IEC_Counter";
            }
            else if (struct_system_type == AGL4.SystemType.S7_IEC_DCounter)
            {
                IsAStruct = true;
                Struct = "S7_IEC_DCounter";
            }
            else if (struct_system_type == AGL4.SystemType.S7_IEC_LCounter)
            {
                IsAStruct = true;
                Struct = "S7_IEC_LCounter";
            }
            else if (struct_system_type == AGL4.SystemType.S7_IEC_SCounter)
            {
                IsAStruct = true;
                Struct = "S7_IEC_SCounter";
            }
            else if (struct_system_type == AGL4.SystemType.S7_IEC_UCounter)
            {
                IsAStruct = true;
                Struct = "S7_IEC_UCounter";
            }
            else if (struct_system_type == AGL4.SystemType.S7_IEC_UDCounter)
            {
                IsAStruct = true;
                Struct = "S7_IEC_UDCounter";
            }
            else if (struct_system_type == AGL4.SystemType.S7_IEC_ULCounter)
            {
                IsAStruct = true;
                Struct = "S7_IEC_ULCounter";
            }
            else if (struct_system_type == AGL4.SystemType.S7_IEC_USCounter)
            {
                IsAStruct = true;
                Struct = "S7_IEC_USCounter";
            }
            else if (struct_system_type == AGL4.SystemType.S7_IEC_Timer)
            {
                IsAStruct = true;
                Struct = "S7_IEC_Timer";
            }

            return IsAStruct;
        }

        void AddPrototype(stNode child, ref int Parent, int globalIID, ref Dictionary<string, StructParameter> map_ListChildrenStructure)
        {
            if (child.NumChild == 0)
            {
                return;
            }

            string oldname = child.name;
            StructParameter par = new StructParameter();
            par.prew_parent = Parent;
            par.numChildren = child.NumChild;
            par.numRealChildren = 0;
            par.prew_ParentName = child.name;
            par.depth = child.depth;
            par.IsPrototype = true;

            if (child.system_type == AGL4.SystemType.S7_UDT_Instance)
            {
                par.nameUdt = child.udtName;
            }
            else
            {
                par.nameUdt = child.name;
            }
            child.udtName = par.nameUdt;

            if (map_ListChildrenStructure.ContainsKey(oldname))
            {
                StructParameter parLoc = map_ListChildrenStructure[child.name];
                parLoc.numChildren--;
                parLoc.numRealChildren++;
                map_ListChildrenStructure[oldname] = parLoc;
                AddPrototypeStructure(/*ref */child, true);
            }
            else
            {
                AddPrototypeStructure(child);
                map_ListChildrenStructure.Add(oldname, par); 
            }
            if (!m_PrototipeVariableStruct.ContainsKey(oldname)) 
            {
                List<stNode> stNodeArray = new List<stNode>();
                m_PrototipeVariableStruct.Add(oldname, stNodeArray);
            }
            
        }
                      
        void AddStructure(ref int element_index,List<stNode> listOfAllChilds, stNode child,ref int parent, ref int globalIID, ref Dictionary<string, StructParameter> map_ListChildrenStructure, ref S7TcpAddress s7TCPOffset, int index = -1)
        {
            int InitialParent;
                        
            InitialParent = parent;

            if (child.NumChild == 0)
            {
                return;
            }
                        
            StructParameter par = new StructParameter();
            par.prew_parent = parent; // prew_parent;
            par.numChildren = child.NumChild;
            par.numRealChildren = 0;
            par.prew_ParentName = child.path;
            string standarNameStructure = string.Empty;
            if(StandarStruttureTia(child.system_type,ref standarNameStructure))
            {                
                par.nameUdt = standarNameStructure;
                //To avoid inserting many conditions to if, it is  change the type of system_type variable.
                child.system_type = AGL4.SystemType.S7_UDT_Instance;
            }
            else if (child.system_type == AGL4.SystemType.S7_UDT_Instance)
            {
                par.nameUdt = child.udtName;
            }
            else
            {
                // S7_Struct
                if (child.nameOrig != null)
                {
                    par.nameUdt = child.nameOrig;
                } 
                else
                {
                    par.nameUdt = child.name;
                    switch (Protocol)
                    {
                        case ProtocolType.S7Tcp:
                            RemovePlcNameFromS7TcpVar(ref par.nameUdt);
                            //child.S7Tcp_Offset += child.GetParentStructOffset();
                            //s7TCPOffset.SetLastLevelOffset(child.S7Tcp_Offset);
                            break;
                    }
                }                
            }
            child.udtName = par.nameUdt;

            if (map_ListChildrenStructure.ContainsKey(child.path))
            {
                StructParameter parLoc = map_ListChildrenStructure[child.path];
                parLoc.numChildren--;
                parLoc.numRealChildren++;
                map_ListChildrenStructure[child.path] = parLoc;
                //Adder 
                if (parLoc.IsPrototype)
                {
                    //child.nameOrig = child.name;
                    AddPrototype( child, ref parent, globalIID, ref map_ListChildrenStructure); //prew_parent
                    if (m_PrototipeVariableStruct.ContainsKey(child.path))
                    {
                        List<stNode> stNodeArray = new List<stNode>();
                        m_PrototipeVariableStruct[child.path].Add(child);
                        return;
                    }
                }

                //structure invalid name --> move to root
                if (child.IsInvalidMoviconName())
                {
                    parLoc.numRealChildren--;
                    map_ListChildrenStructure[child.path] = parLoc;
                    AddItem(globalIID, -1, child, ref map_ListChildrenStructure, ref s7TCPOffset);
                }
                else
                    AddItemStruct(globalIID, parent, child, ref map_ListChildrenStructure, ref s7TCPOffset); //prew_parent
                AddPrototypeStructure(child,true);
            }
            else
            {
                if (child.IsInvalidMoviconName())
                {
                    AddItem(globalIID, -1, child, ref map_ListChildrenStructure, ref s7TCPOffset); //prew_parent
                }
                else
                {
                    AddItem(globalIID, parent, child, ref map_ListChildrenStructure, ref s7TCPOffset); //prew_parent
                }
                AddPrototypeStructure(child);
            }

            parent = globalIID;
            par.parent = parent;
            map_ListChildrenStructure.Add(child.name, par);
            globalIID++;

            //The reference to the parent is not the correct one, because it does not include an array index, 
            //so a new list is created where the parent of the member has address correct, 
            //so that it is possible to reconstruct the correct address of the members of the structure.
            List<stNode> listOfChilds = GetAllChilds(element_index, listOfAllChilds, child);
            
            int start_process_index = 0;
            ExpandInMap(listOfChilds, ref globalIID, ref parent, ref start_process_index, ref map_ListChildrenStructure, ref s7TCPOffset);

            if (map_ListChildrenStructure.ContainsKey(child.name))
            {
                par = map_ListChildrenStructure[child.name];                
                map_ListChildrenStructure.Remove(child.name);
                //if struct is empty decrease counter's element of previous struct (if exist)
                if (par.numRealChildren == 0)
                {
                    if (!string.IsNullOrEmpty(par.prew_ParentName))
                    {
                        if (map_ListChildrenStructure.ContainsKey(par.prew_ParentName))
                        {
                            StructParameter prew_ParentName = map_ListChildrenStructure[par.prew_ParentName];
                            prew_ParentName.numRealChildren--;
                            map_ListChildrenStructure[par.prew_ParentName] = prew_ParentName;
                        }
                        parent = par.prew_parent;
                    }
                    if (par.parent > 0)
                    {
                        //mark element to be delete later
                        ImportVariable IVar = m_MapImportVariable[par.parent];
                        IVar.ToDelete = true;
                        m_MapImportVariable[par.parent] = IVar;
                    }
                }
            }

            element_index += listOfChilds.Count;

            parent = InitialParent;
        }

        void AddItemMain(int globalIID, ref int Parent, stNode child, ref Dictionary<string, StructParameter> map_ListChildrenStructure, ref S7TcpAddress s7TCPOffset)
        {
            if (!map_ListChildrenStructure.ContainsKey(child.path))
            {
                Parent = -1;
                AddItem(globalIID, Parent, child, ref map_ListChildrenStructure, ref s7TCPOffset);
            }
            else
            {
                StructParameter par = map_ListChildrenStructure[child.path];
                par.numChildren--;
                par.numRealChildren++;
                if (child.MovedToRoot())
                {
                    par.numRealChildren--;
                    par.numChildren++; //
                }
                map_ListChildrenStructure[child.path] = par;
                child.udtName = par.nameUdt;
                AddPrototypeStructure(child);
                
                if (child.IsInvalidMoviconName())
                    AddItem(globalIID, -1, child, ref map_ListChildrenStructure, ref s7TCPOffset); 
                else
                    AddItemStruct(globalIID, Parent, child, ref map_ListChildrenStructure, ref s7TCPOffset);
            }
        }

        bool AddItem(int globalIID, int Parent, stNode node, ref Dictionary<string, StructParameter> map_ListChildrenStructure, ref S7TcpAddress s7TCPOffset, bool isArrayElement = false)
        {
            bool ret = true;
            uint nVarSize = 0;            
            string szType = string.Empty;
            string szTypeStruct = string.Empty;
            UFUAModel.DataType nType = GetMoviconTypeIdFromTiaPortalType(node.system_type, ref nVarSize, ref szType, ref szTypeStruct);
            if (nVarSize == 0)
            {
                return false;
            }

            ImportVariable IVar = new ImportVariable();            
            IVar.szDescription = node.comment;            
            IVar.szName = node.name.Trim('\"');
            switch (Protocol)
            {
                case ProtocolType.TiaSymbolic:
                    node.GetAddress(m_ImportSource, out IVar.szAddress, out IVar.szAddressImport);
                    break;

                case ProtocolType.S7Tcp:
                    node.GetAddressS7Tcp(s7TCPOffset, out IVar.szAddress);
                    break;
            }

            if (node.depth >= 3 && !node.MovedToRoot() || isArrayElement)
                IVar.szName = node.objectName;

            IVar.lID = globalIID;
            IVar.lParent = Parent;
            IVar.nType = nType;
            IVar.szType = szType;
            IVar.nSize = nVarSize;
            IVar.nElemType = nType;
            IVar.ArrayDimension = node.ArrayDimension;
            IVar.S7DataFormat = node.system_type;

            if (IsAStructure(node.system_type))
            {
                if (node.udtName == "S7_DTL")
                {
                    IVar.S7DataFormat = AGL4.SystemType.S7_DTL;
                }
                else
                {
                    IVar.S7DataFormat = AGL4.SystemType.S7_Struct;
                }

                IVar.szType = node.udtName;
                IVar.ImportType = ImportTypes.Struct;
                //if (node.IsInvalidMoviconName())
                //{
                //    IVar.lParent = -1;
                //}
            }

            switch (Protocol) {
                case ProtocolType.TiaSymbolic:
                    if ((node.system_type == AGL4.SystemType.S7_String) || (node.system_type == AGL4.SystemType.S7_WString))
                    {
                        IVar.StringLength = (uint)node.string_size;
                        IVar.nSize = (uint)node.string_size;
                        // don't move to root array's element
                        if (!isArrayElement)
                            IVar.lParent = -1;
                    }
                    break;
                case ProtocolType.S7Tcp:
                    if ((node.system_type == AGL4.SystemType.S7_String) || (node.system_type == AGL4.SystemType.S7_WString))
                    {
                        IVar.StringLength = (uint)node.string_size;
                        IVar.nSize = (uint)node.string_size;                        
                    }
                    break;
            }
            IVar.depth = node.depth;

            switch (Protocol)
            {
                case ProtocolType.S7Tcp:
                    RemovePlcNameFromS7TcpVar(ref IVar.szName);
                    break;
            }

            m_MapImportVariable[IVar.lID] = IVar;
            return (ret);
        }

        bool AddItemStruct(int globalIID, int Parent, stNode node, ref Dictionary<string, StructParameter> map_ListChildrenStructure, ref S7TcpAddress s7TCPOffset)
        {
            bool ret = true;
            uint nVarSize = 0;            
            string szType = string.Empty;
            string szTypeStruct = string.Empty;
            UFUAModel.DataType nType = GetMoviconTypeIdFromTiaPortalType(node.system_type, ref nVarSize, ref szType, ref szTypeStruct);
            if (nVarSize == 0)
            {
                return false;
            }

            ImportVariable IVar = new ImportVariable();
            IVar.szDescription = node.comment;
            IVar.szName = node.name.Substring(node.path.Length + 1).Trim('\"');
            
            switch (Protocol)
            {
                case ProtocolType.TiaSymbolic:
                    node.GetAddress(m_ImportSource, out IVar.szAddress, out IVar.szAddressImport);
                    break;
                case ProtocolType.S7Tcp:
                    node.GetAddressS7Tcp(s7TCPOffset, out IVar.szAddress);
                    break;
            }

            IVar.lID = globalIID;
            IVar.lParent = Parent;
            IVar.nType = nType;
            IVar.szType = szType;
            IVar.nSize = nVarSize;
            IVar.nElemType = nType;
            IVar.ArrayDimension = node.ArrayDimension;
            IVar.S7DataFormat = node.system_type;

            if (IsAStructure(node.system_type))
            {
                if (node.udtName == "S7_DTL")
                {
                    IVar.S7DataFormat = AGL4.SystemType.S7_DTL;
                    //IVar.lParent = -1;
                    //IVar.szName = node.name.Trim('\"');
                }
                else
                {
                    IVar.S7DataFormat = AGL4.SystemType.S7_Struct;
                }
                IVar.szType = node.udtName;
                IVar.ImportType = ImportTypes.Struct;
                IVar.szType = node.udtName;
            }

            switch (Protocol)
            {
                case ProtocolType.TiaSymbolic:
                    if ((node.system_type == AGL4.SystemType.S7_String) || (node.system_type == AGL4.SystemType.S7_WString))
                    {
                        IVar.StringLength = (uint)node.string_size;
                        IVar.nSize = (uint)node.string_size;
                        IVar.lParent = -1;
                        IVar.szName = node.name.Trim('\"');
                        IVar.S7DataFormat = AGL4.SystemType.S7_String;
                        IVar.nType = nType;
                        IVar.szType = szType;
                    }
                    break;
                case ProtocolType.S7Tcp:
                    if ((node.system_type == AGL4.SystemType.S7_String) || (node.system_type == AGL4.SystemType.S7_WString))
                    {
                        IVar.StringLength = (uint)node.string_size;
                        IVar.nSize = (uint)node.string_size;
                        //IVar.lParent = -1;
                        //IVar.szName = node.name.Trim('\"');
                        IVar.szName = node.objectName.Trim('\"');
                        IVar.S7DataFormat = AGL4.SystemType.S7_String;
                        IVar.nType = nType;
                        IVar.szType = szType;
                    }
                    break;
            }
                        
            IVar.depth = node.depth;
            IVar.path = node.path;

            m_MapImportVariable[IVar.lID] = IVar;
            return (ret);
        }

        bool AddArray(ref int globalIID, ref int Parent, stNode node, ref int child_index, ref Dictionary<string, StructParameter> map_ListChildrenStructure, ref S7TcpAddress s7TCPOffset)
        {
            bool ret = false;
            uint numberItem;
            stNode local;
            int parent;
            UInt16[] stdim;

            if (map_ListChildrenStructure.ContainsKey(node.path))
            {
                StructParameter parLoc = map_ListChildrenStructure[node.path];
                parLoc.numChildren--;
                parLoc.numRealChildren++;
                map_ListChildrenStructure[node.path] = parLoc;
            }

            stdim = new UInt16[node.dimension * 2];
            ret = ParseArrayDimensions(node, ref stdim);
            if (ret == false)
            {
                return (ret);
            }

            int arrayElementIndex = 0;
            uint varSize = GetStandardDataTypeSize(node.system_type);
            uint IndexOfItteration = 1;
            int LastlParent = -1;
            //Metodo utilizzato per creare elementi di matrici e matrici multilivello
            RecursiveInteractions(IndexOfItteration, stdim, ref Parent, ref LastlParent, node,  ref map_ListChildrenStructure, ref s7TCPOffset,ref arrayElementIndex, "", ref globalIID);
            return (true);
        }
        /// <summary>
        /// Method used for creating elements of multi-level arrays and arrays
        /// </summary>
        /// <param name="IndexOfItteration"></param>
        /// <param name="stdim"></param>
        /// <param name="Parent"></param>
        /// <param name="LastlParent"></param>
        /// <param name="node"></param>
        /// <param name="map_ListChildrenStructure"></param>
        /// <param name="s7TCPOffset"></param>
        /// <param name="arrayElementIndex"></param>
        /// <param name="indexmatrix"></param>
        /// <param name="globalIID"></param>
        /// <returns></returns>
        bool RecursiveInteractions(uint IndexOfItteration, UInt16[] stdim, ref int Parent, ref int LastlParent, stNode node, ref Dictionary<string, StructParameter> map_ListChildrenStructure, ref S7TcpAddress s7TCPOffset,ref int arrayElementIndex, string indexmatrix, ref int globalIID)
        {
            
            //Conditin that identifie the one dimension array
            if (node.dimension == IndexOfItteration)
            {
                stNode local = (stNode)node.Clone();
                if (IndexOfItteration == node.dimension)
                    LastIntteration(IndexOfItteration, stdim, ref Parent, ref LastlParent,  node,  ref map_ListChildrenStructure, ref s7TCPOffset,ref arrayElementIndex, indexmatrix, ref globalIID);
            }
            else//Conditin that identifie the multy dimension array
            {
                uint selectElemetStarArray1 = (IndexOfItteration - 1) * 2;
                for (int index = stdim[selectElemetStarArray1]; index <= stdim[selectElemetStarArray1 + 1]; index++)
                {
                    //Conditin that identifie the multy dimension array
                    if ((IndexOfItteration + 1)!= node.dimension)
                    {

                        string localaindexmatrix = indexmatrix;
                        if (string.IsNullOrWhiteSpace(indexmatrix))
                            localaindexmatrix = string.Format("{0},", index);
                        else
                            localaindexmatrix = string.Format("{0}{1},", indexmatrix, index);

                        RecursiveInteractions(IndexOfItteration + 1, stdim, ref Parent, ref LastlParent,  node,  ref map_ListChildrenStructure, ref s7TCPOffset,ref arrayElementIndex, localaindexmatrix, ref globalIID);
                    }
                    else//Conditin that identifie the last dimension of multy dimension array
                    {
                        string localaindexmatrix = indexmatrix;
                        if (string.IsNullOrWhiteSpace(indexmatrix))
                            localaindexmatrix = string.Format("{0},", index);
                        else
                            localaindexmatrix = string.Format("{0}{1},", indexmatrix, index);

                        LastIntteration(IndexOfItteration + 1, stdim, ref Parent, ref LastlParent,  node,  ref map_ListChildrenStructure, ref s7TCPOffset,ref arrayElementIndex, localaindexmatrix, ref globalIID);
                    }
                }                             
            }
            return true;
        }
        /// <summary>
        /// Method used to create elements of the last level
        /// </summary>
        /// <param name="IndexOfItteration"></param>
        /// <param name="stdim"></param>
        /// <param name="Parent"></param>
        /// <param name="LastlParent"></param>
        /// <param name="node"></param>
        /// <param name="map_ListChildrenStructure"></param>
        /// <param name="s7TCPOffset"></param>
        /// <param name="arrayElementIndex"></param>
        /// <param name="indexmatrix"></param>
        /// <param name="globalIID"></param>
        void LastIntteration(uint IndexOfItteration, UInt16[] stdim, ref int Parent, ref int LastlParent, stNode node, ref Dictionary<string, StructParameter> map_ListChildrenStructure, ref S7TcpAddress s7TCPOffset,ref int arrayElementIndex, string indexmatrix,ref int globalIID)
        {            
            string szType = string.Empty;
            string szTypeStruct = string.Empty;
            uint nVarSize = 0;
            uint numberItem = 0;
            UFUAModel.DataType nType = GetMoviconTypeIdFromTiaPortalType(node.system_type, ref nVarSize, ref szType, ref szTypeStruct);

            //Insert the array elements container only once or split the string array
            if ((node.system_type == AGL4.SystemType.S7_String) || (LastlParent == -1))
            {
                //Calculate number of elements of array
                for(int index = 0; index < stdim.Length;)
                {
                    //A necesary condition for avoid of multiply value for zero that result always zero
                    if (numberItem != 0)
                        numberItem = numberItem * (uint)(stdim[index + 1] - stdim[index] + 1);
                    else
                        numberItem = (uint)(stdim[index + 1] - stdim[index] + 1);
                    index += 2;
                }
                //
                LastlParent = Parent;
                stNode local = (stNode)node.Clone();
                local.ArrayDimension = numberItem;
                // in case of not member of struct
                if (Parent == -1 || node.IsInvalidMoviconName())
                {
                    // move to root 
                    if (node.IsInvalidMoviconName() && map_ListChildrenStructure.ContainsKey(node.path))
                    {
                        StructParameter parLoc = map_ListChildrenStructure[node.path];
                        parLoc.numRealChildren--;
                        map_ListChildrenStructure[node.path] = parLoc;
                    }
                    AddItem(globalIID, -1, local, ref map_ListChildrenStructure, ref s7TCPOffset);
                }
                else
                {
                    AddItemStruct(globalIID, LastlParent, local, ref map_ListChildrenStructure, ref s7TCPOffset);
                    
                }
                LastlParent = globalIID;
                globalIID++;
            }
            uint selectElemetStarArray = (IndexOfItteration - 1) * 2;
            for (int index = stdim[selectElemetStarArray]; index <= stdim[selectElemetStarArray + 1]; index++)
            {
                stNode local = (stNode)node.Clone();
                local.name = string.Format("{0}[{1}{2}]", local.name, indexmatrix, index);
                local.objectName = string.Format("{0}[{1}{2}]", local.objectName, indexmatrix, index);
                local.ArrayDimension = 0;

                switch (Protocol)
                {
                    case ProtocolType.S7Tcp:
                        GetArrayNextElementAddressS7Tcp(ref local, ref arrayElementIndex, (int)nVarSize, (index == stdim[0]));
                        break;
                }

                AddItem(globalIID, LastlParent, local, ref map_ListChildrenStructure, ref s7TCPOffset, true);
                globalIID++;
                arrayElementIndex++;
            }
        }
        private List<stNode> GetAllChilds(int start_element_index, List<stNode> sourceList, stNode parentArray)
        {
            List<stNode> Result = new List<stNode>();
            string sourcePath = (parentArray.nameOrig == null ? parentArray.name : parentArray.nameOrig);
            start_element_index++;
            if (sourceList.Count < start_element_index)
                return Result;

            for (int index = start_element_index; index < sourceList.Count; index++)
            {
                stNode node = sourceList[index];
                if (node.path.IndexOf(sourcePath, 0) >= 0)
                {
                    stNode NewNode = (stNode)node.Clone();
                    NewNode.ParentNode = parentArray;
                    //stNode NewNode = node;
                    NewNode.UpdateAllPath(sourcePath, parentArray.name);
                    Result.Add(NewNode);
                }
                else
                {
                    break;
                }
            }

            return Result;
        }
                
        bool AddArrayOfStructure(List<stNode> listOfAllChilds,ref int globalIID, ref int parent,stNode node, ref int element_index, ref Dictionary<string, StructParameter> map_ListChildrenStructure, ref S7TcpAddress s7TCPOffset)
        {
            bool ret = false;
            UInt16[] stdim;
            
            if (map_ListChildrenStructure.ContainsKey(node.path))
            {
                StructParameter parLoc = map_ListChildrenStructure[node.path];
                parLoc.numChildren--;
                if (parLoc.prew_parent != -1)
                    parLoc.numRealChildren++;
                map_ListChildrenStructure[node.path] = parLoc;
            }

            stdim = new UInt16[node.dimension * 2];
            ret = ParseArrayDimensions(node, ref stdim);
            if (ret == false)
            {
                return (ret);
            }

            if (node.dimension == 1)
            {                
                //calculate the number of elements                
                uint numberItem = (uint)(stdim[1] - stdim[0] + 1);                
                if (node.dimension == 1)
                {
                    //node.S7Tcp_StructSize_Array = (int)numberItem;
                    int start_process_index = element_index;
                    for (int index = stdim[0]; index <= stdim[1]; index++)
                    {
                        s7TCPOffset.AddLevel(node);

                        stNode local = (stNode) node.Clone();
                        local.nameOrig = local.name;
                        local.name = string.Format("{0}[{1}]",local.name,index);
                        local.objectName = string.Format("{0}[{1}]", local.objectName,index);
                        local.path = local.name; // node.name;
                        local.udtName = node.udtName;
                        local.matrix = null;

                        switch (Protocol)
                        {
                            case ProtocolType.S7Tcp:                                
                                local.S7Tcp_DBNr = 0;
                                local.S7Tcp_Offset = node.S7Tcp_Offset;
                                local.S7Tcp_BitNr = 0;
                                local.S7Tcp_ArrayFirstIndex = stdim[0];
                                break;
                        }                        

                        local.hierarchy_type = AGL4.HierarchyType.STRUCTURE;
                        //use source structure type
                        //local.system_type = AGL4.SystemType.S7_Struct;

                        element_index = start_process_index;
                        AddStructure(ref element_index, listOfAllChilds, local, ref parent, ref globalIID, ref map_ListChildrenStructure, ref s7TCPOffset, index);
                    }
                }
            }
            if (node.dimension == 2)
            {
                //calculate the number of elements                
                uint numberItem = (uint)((stdim[3] - stdim[2] + 1) * (stdim[1] - stdim[0] + 1));
                stNode local;                
                for (int index2 = stdim[0]; index2 <= stdim[1]; index2++)
                {
                    for (int index = stdim[2]; index <= stdim[3]; index++)
                    {
                        local = node;
                        local.nameOrig = local.name;
                        local.name = string.Format("{0}[{1},{2}]", local.name ,index2,index);
                        local.path = node.name;
                        local.udtName = node.name;

                        local.hierarchy_type = AGL4.HierarchyType.STRUCTURE;
                        local.system_type = AGL4.SystemType.S7_Struct;

                        AddElementOfStructure(local, ref globalIID, ref map_ListChildrenStructure, ref s7TCPOffset);
                    }
                }
            }
            if (node.dimension == 3)
            {
                //calculate the number of elements                
                uint numberItem = (uint)((stdim[5] - stdim[4] + 1) * (stdim[3] - stdim[2] + 1) * (stdim[1] - stdim[0] + 1));
                stNode local;
                for (int index3 = stdim[0]; index3 <= stdim[1]; index3++)
                {
                    for (int index2 = stdim[2]; index2 <= stdim[3]; index2++)
                    {

                        for (int index = stdim[4]; index <= stdim[5]; index++)
                        {
                            local = node;
                            local.nameOrig = local.name;
                            local.name = string.Format("{0}[{1},{2},{2}]", local.name, index3,index2,index);
                            local.path = node.name;
                            local.udtName = node.name;

                            local.hierarchy_type = AGL4.HierarchyType.STRUCTURE;
                            local.system_type = AGL4.SystemType.S7_Struct;

                            AddElementOfStructure(local, ref globalIID, ref map_ListChildrenStructure, ref s7TCPOffset);
                        }
                    }
                }
            }

            return (ret);
        }

        void AddElementOfStructure(stNode local, ref int globalIID, ref Dictionary<string, StructParameter> map_ListChildrenStructure, ref S7TcpAddress s7TCPOffset, bool isNested = false)
        {
            List<stNode> items;
            if (m_PrototipeVariableStruct.ContainsKey(local.udtName))
            {
                items = m_PrototipeVariableStruct[local.udtName];
            }
            else if (m_PrototipeVariableStruct.ContainsKey(local.nameOrig))
            {
                items = m_PrototipeVariableStruct[local.nameOrig];
            }
            else
            {
                return;
            }

            int parent = -1;
            if (!isNested)
            {                 
                AddItem(globalIID, parent, local, ref map_ListChildrenStructure, ref s7TCPOffset);
            }

            parent = globalIID;
            globalIID++;
            for(int index = 0; index < items.Count; index++) //foreach(stNode item in items)
            {
                stNode item = items[index];
                string baseArrayName = local.name;
                string tempName = item.name.Substring(item.path.Length +1);
                item.path = baseArrayName;
                baseArrayName = string.Format("{0}.{1}",baseArrayName ,tempName);
                item.name = baseArrayName;
                AddItemStruct(globalIID, parent, item, ref map_ListChildrenStructure, ref s7TCPOffset);
                if (item.hierarchy_type == AGL4.HierarchyType.STRUCTURE)
                {
                    AddElementOfStructure(item, ref globalIID, ref map_ListChildrenStructure, ref s7TCPOffset, true);
                }
                globalIID++;
            }
        }

        bool ParseArrayDimensions(stNode node, ref UInt16[] stdim)
        {
            System.Text.StringBuilder sb = new System.Text.StringBuilder();
            if (node.matrix.Length == 0)
            {
                return (false);
            }
            char ch;
            int indexBuffer = 0;
            for (int index = 0; index < node.matrix.Length; index++)
            {
                ch = node.matrix[index];
                if (Char.IsNumber(ch))
                {
                    sb.Append(ch);
                }
                else
                {
                    if (sb.Length != 0)
                    {
                        stdim[indexBuffer] = Convert.ToUInt16(sb.ToString());
                        indexBuffer++;
                        sb.Clear();
                    }
                }
            }
            return (true);
        }

        UFUAModel.DataType GetMoviconTypeIdFromTiaPortalType(AGL4.SystemType system_type, ref uint VarSize, ref string szS7Type, ref string szTypeMov)
        {
            UFUAModel.DataType nType = UFUAModel.DataType.Boolean;


            if (system_type == AGL4.SystemType.S7_Bool)
            {
                VarSize = 1;
                szS7Type = "S7_Bool";
                szTypeMov = "BOOL";
            }
            else if (system_type == AGL4.SystemType.S7_USInt)
            {
                nType = UFUAModel.DataType.Byte;
                VarSize = 1;
                szS7Type = "S7_USInt";
                szTypeMov = "BYTE";
            }
            else if (system_type == AGL4.SystemType.S7_Byte)
            {
                nType = UFUAModel.DataType.Byte;
                VarSize = 1;
                szS7Type = "S7_Byte";
                szTypeMov = "BYTE";
            }
            else if (system_type == AGL4.SystemType.S7_Char)
            {
                nType = UFUAModel.DataType.SByte;
                VarSize = 1;
                szS7Type = "S7_Char";
                szTypeMov = "CHAR";
            }
            else if (system_type == AGL4.SystemType.S7_SInt)
            {
                nType = UFUAModel.DataType.SByte;
                VarSize = 1;
                szS7Type = "S7_SInt";
                szTypeMov = "BYTE";
            }
            else if (system_type == AGL4.SystemType.S7_Int)
            {
                nType = UFUAModel.DataType.Int16;
                VarSize = 2;
                szS7Type = "S7_Int";
                szTypeMov = "INT";
            }
            else if (system_type == AGL4.SystemType.S7_UInt)
            {
                nType = UFUAModel.DataType.UInt16;
                VarSize = 2;
                szS7Type = "S7_UInt";
                szTypeMov = "WORD";
            }
            else if (system_type == AGL4.SystemType.S7_Date)
            {
                nType = UFUAModel.DataType.UInt16;
                VarSize = 2;
                szS7Type = "S7_Date";
                szTypeMov = "WORD";
            }
            else if (system_type == AGL4.SystemType.S7_Word)
            {
                nType = UFUAModel.DataType.UInt16;
                VarSize = 2;
                szS7Type = "S7_Word";
                szTypeMov = "WORD";
            }
            else if (system_type == AGL4.SystemType.S7_S5Time)
            {
                nType = UFUAModel.DataType.UInt32;
                VarSize = 4;
                szS7Type = "S7_S5Time";
                szTypeMov = "DWORD";
            }
            else if (system_type == AGL4.SystemType.S7_DInt)
            {
                nType = UFUAModel.DataType.Int32;
                VarSize = 4;
                szS7Type = "S7_DInt";
                szTypeMov = "DINT";
            }
            else if (system_type == AGL4.SystemType.S7_UDInt)
            {
                nType = UFUAModel.DataType.UInt32;
                VarSize = 4;
                szS7Type = "S7_UDInt";
                szTypeMov = "DWORD";
            }
            else if (system_type == AGL4.SystemType.S7_DWord)
            {
                nType = UFUAModel.DataType.UInt32;
                VarSize = 4;
                szS7Type = "S7_DWord";
                szTypeMov = "DWORD";
            }
            else if (system_type == AGL4.SystemType.S7_Real)
            {
                nType = UFUAModel.DataType.Float;
                VarSize = 4;
                szS7Type = "S7_Real";
                szTypeMov = "REAL";
            }
            else if (system_type == AGL4.SystemType.S7_Time)
            {
                nType = UFUAModel.DataType.UInt32;
                VarSize = 4;
                szS7Type = "S7_Time";
                szTypeMov = "DWORD";
            }
            else if (system_type == AGL4.SystemType.S7_Time_Of_Day)
            {
                nType = UFUAModel.DataType.UInt32;
                VarSize = 4;
                szS7Type = "S7_Time_Of_Day";
                szTypeMov = "DWORD";
            }
            else if (system_type == AGL4.SystemType.S7_LWord)
            {
                nType = UFUAModel.DataType.Int64;
                VarSize = 8;
                szS7Type = "S7_LWord";
                szTypeMov = "S7_LWord";
            }
            else if (system_type == AGL4.SystemType.S7_LReal)
            {
                nType = UFUAModel.DataType.Double;
                VarSize = 8;
                szS7Type = "S7_LReal";
                szTypeMov = "S7_LReal";
            }
            else if (system_type == AGL4.SystemType.S7_ULInt)
            {
                nType = UFUAModel.DataType.UInt64;
                VarSize = 8;
                szS7Type = "S7_ULInt";
                szTypeMov = "S7_ULInt";
            }
            else if (system_type == AGL4.SystemType.S7_LInt)
            {
                nType = UFUAModel.DataType.Int64;
                VarSize = 8;
                szS7Type = "S7_LInt";
                szTypeMov = "S7_LInt";
            }
            else if (system_type == AGL4.SystemType.S7_String)
            {
                nType = UFUAModel.DataType.String;
                VarSize = 1;
                szS7Type = "S7_String";
                szTypeMov = "STRING";
            }
            else if (system_type == AGL4.SystemType.S7_WString)
            {
                switch (Protocol)
                {
                    case ProtocolType.TiaSymbolic:
                        nType = UFUAModel.DataType.String;
                        VarSize = 1;
                        szS7Type = "S7_WString ";
                        szTypeMov = "STRING";
                        break;

                    // type not supported
                    case ProtocolType.S7Tcp:
                        nType = UFUAModel.DataType.Boolean;
                        VarSize = 0;
                        break;
                }
            }
            else if (system_type == AGL4.SystemType.S7_Struct)
            {
                nType = UFUAModel.DataType.String;
                VarSize = 1;
                szS7Type = "S7_Struct";
                szTypeMov = "STRUCT";
            }
            else if (system_type == AGL4.SystemType.S7_UDT_Instance)
            {
                nType = UFUAModel.DataType.String;
                VarSize = 1;
                szS7Type = "S7_Struct";
                szTypeMov = "STRUCT";
            }
            else if(system_type == AGL4.SystemType.S7_FB_Instance)
            {
                nType = UFUAModel.DataType.String;
                VarSize = 1;
                szS7Type = "S7_Struct";
                szTypeMov = "STRUCT";
            }
            else
            {
                nType = UFUAModel.DataType.Boolean;
                VarSize = 0;
            }

            return (nType);
        }

        uint GetStandardDataTypeSize(AGL4.SystemType system_type)
        {
            uint varSize = 0;            

            if (!(IsAStructure(system_type))) {
                string szS7Type = string.Empty;
                string szTypeMov = string.Empty;
                UFUAModel.DataType dt = GetMoviconTypeIdFromTiaPortalType(system_type, ref varSize, ref szS7Type, ref szTypeMov);
            }

            return varSize;
        }

        void GetArrayNextElementAddressS7Tcp(ref stNode stnode, ref int offset, int varSize, bool firstSubArrayElement)
        {
            if (stnode.system_type == AGL4.SystemType.S7_Bool)
            {
                int bytePart = Math.DivRem(offset, 8, out int bitRest);
                stnode.S7Tcp_Offset += bytePart;
                stnode.S7Tcp_BitNr = bitRest;

                // sub array start alway at new byte address
                if (firstSubArrayElement)
                {
                    if (stnode.S7Tcp_BitNr % 8 != 0)
                    {
                        offset += (8 - stnode.S7Tcp_BitNr);
                        stnode.S7Tcp_Offset++;
                        stnode.S7Tcp_BitNr = 0;
                    }
                }
            }
            else
            {
                if (stnode.system_type == AGL4.SystemType.S7_String ||
                    stnode.system_type == AGL4.SystemType.S7_WString)
                {
                    int localVasSize = stnode.string_size + 2;
                    if ((localVasSize % 2) != 0)
                    {
                        localVasSize++;
                    }
                    stnode.S7Tcp_Offset += (offset * localVasSize);
                }
                else
                    stnode.S7Tcp_Offset += (offset * varSize);
                stnode.S7Tcp_BitNr = 0;

               
            }
        }

        private void RemovePlcNameFromS7TcpVar(ref string varName)
        {
            int pos = varName.IndexOf('.');
            if (pos > 0)
                varName =  varName.Substring(pos + 1);
        }

        public Dictionary<long, ImportVariable> GetMapImportVariable()
        {
            return (m_MapImportVariable);
        }
        public Dictionary<string, stPrototype> GetMapUDTBaseS7P()
        {
            return (m_mapUDTBaseS7P);
        }        
    }
}
