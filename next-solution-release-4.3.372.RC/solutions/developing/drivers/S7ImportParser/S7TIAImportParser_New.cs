using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Accon.AGLink;
using Accon.Symbolik;
using System.Windows.Forms;
using UFUAModel;

namespace S7ImportParser
{
    public class S7TIAImportParser : IDisposable
    {

        IntPtr rootSchemaNodeHandle = IntPtr.Zero;
        Dictionary<string, IntPtr> m_MapProgramsProgetTiaPortal;
        Dictionary<string, string> m_MapValueDepthStructToNumStruct;
        Dictionary<int, string> programList;
        Dictionary<string, List<stNode>> m_PrototipeVariableStruct;
        Dictionary<string, List<string>> m_mapUDTBaseS7P;
        Dictionary<long, ImportVariable> m_MapImportVariable;
        List<stNode> m_ListOfAllChilds;
        string TiaPrj;
        private int m_currentCulture;
        public enum ImportTypes
        {
            Unknown,
            Standard,
            Array,
            Struct,
        }
        public struct ImportVariable
        {
            public String szName;
            public String szDescription;
            public DataType nType;
            public String szType;
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
        };

        struct stNode
        {
            public IntPtr node;
            public uint depthStruct;
            public int LastChild;
            public int NumChild;
            public UInt32 depth;
            public string path;
            public AGL4.HierarchyType hierarchy_type;
            public AGL4.SystemType system_type;
            public string name;
            public AGL4.ValueType value_type;
            public AGL4.PermissionType permissionType;
            public string comment;
            public uint dimension;
            public string matrix;
            public int string_size;
            public string udtName;
            public string udt;
            public string nameOrig;
            public uint ArrayDimension;
            public int numDB;
            public int address;
        }
        struct stOffset
        {
            public int nStartArray;
            public int nHigt;
            public int nSizeArray;
        };     

        public S7TIAImportParser(string file)
        {
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
            m_mapUDTBaseS7P = new Dictionary<string, List<string>>();


            if (rootSchemaNodeHandle != IntPtr.Zero)
            {
                AGL4.Symbolic_FreeHandle(rootSchemaNodeHandle);
                rootSchemaNodeHandle = new IntPtr();
            }

            if(string.IsNullOrEmpty(file))
            {
                return;
            }

            TiaPrj = string.Empty;
            ret = AGL4.Symbolic_LoadTIAProjectSymbols(file, ref rootSchemaNodeHandle, true);
            if (ret == AGL4Sym.AGLSYM_SUCCESS)
            {
                TiaPrj = file;

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

            foreach (var item in m_mapUDTBaseS7P)
            {
                item.Value.Clear();
            }
            m_mapUDTBaseS7P.Clear();

            if (m_PrototipeVariableStruct != null)
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
            stNode stnode = new stNode();
            stnode.depthStruct = 0;
            stnode.LastChild = 0;
            stnode.NumChild = 0;
            stnode.depth = 1;
            stnode.path = string.Empty;
            stnode.name = string.Empty;
            stnode.hierarchy_type = AGL4.HierarchyType.UNDEFINED;
            stnode.value_type = AGL4.ValueType.UNDEFINED;
            stnode.system_type = AGL4.SystemType.UNDEFINED;
            stnode.permissionType = AGL4.PermissionType.NONE;
            stnode.dimension = 0;
            stnode.matrix = string.Empty;
            stnode.string_size = 0;
            stnode.udtName = string.Empty;
            stnode.node = m_MapProgramsProgetTiaPortal[programList[selectProgra]];
            stnode.ArrayDimension = 0;
            ParseNodeTiaPortal(ref stnode);

            PopulationMapsForImport();
        }

        public void ParsingHandlerPointer(IntPtr _rootSchemaNodeHandle)
        {
            rootSchemaNodeHandle = _rootSchemaNodeHandle;
            stNode stnode = new stNode();
            stnode.depthStruct = 0;
            stnode.LastChild = 0;
            stnode.NumChild = 0;
            stnode.depth = 1;
            stnode.path = string.Empty;
            stnode.name = string.Empty;
            stnode.hierarchy_type = AGL4.HierarchyType.UNDEFINED;
            stnode.value_type = AGL4.ValueType.UNDEFINED;
            stnode.system_type = AGL4.SystemType.UNDEFINED;
            stnode.permissionType = AGL4.PermissionType.NONE;
            stnode.dimension = 0;
            stnode.matrix = string.Empty;
            stnode.string_size = 0;
            stnode.udtName = string.Empty;
            stnode.node = _rootSchemaNodeHandle;
            stnode.ArrayDimension = 0;
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
                ret = AGL4.Symbolic_GetChildCount(stnode.node, ref child_count);
                if (ret != AGL4.AGL40_SUCCESS)
                {
                    return;
                }

                stnode.NumChild = child_count;
                m_ListOfAllChilds.Add(stnode);

#if DEBUG
                System.Diagnostics.Debug.WriteLine("CHECK {0} - {1} - {2} - {3} - {4} - {5} - {6} - {7} - {8} - {9} - {10} - {11} - {12}", stnode.path, stnode.name,
                                                                                                                 stnode.NumChild, stnode.LastChild,
                                                                                                                 stnode.system_type, stnode.hierarchy_type,
                                                                                                                 stnode.value_type, stnode.permissionType,
                                                                                                                 stnode.dimension, stnode.matrix,
                                                                                                                 stnode.comment, stnode.string_size, stnode.udtName);
#endif
                
                //Iterate through all childs
                for (int child_index = 0; child_index < child_count; ++child_index)
                {
                    
                    stNode stNodeIdent = new stNode();
                    stNodeIdent.depth = stnode.depth;
                    stNodeIdent.depthStruct = stnode.depthStruct;
                    stNodeIdent.NumChild = child_count;
                    stNodeIdent.LastChild = child_index;
                    stNodeIdent.path = symbol_path;
                    

                    ret = AGL4.Symbolic_GetChild(stnode.node, child_index, ref stNodeIdent.node);
                    if (ret != AGL4.AGL40_SUCCESS)
                    {
                        continue;
                    }

                    UInt32 depthTmp = stnode.depth + 1;
                    ParseNodeTiaPortal(ref stNodeIdent);
                }

                if (stnode.depthStruct > 0)
                {
                    stnode.depthStruct--;
                }

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
                m_ListOfAllChilds.Add(stnode);
#if DEBUG
                System.Diagnostics.Debug.WriteLine("CHECK {0} - {1} - {2} - {3} - {4} - {5} - {6} - {7} - {8} - {9} - {10} - {11} - {12}", stnode.path, stnode.name,
                                                                                                                    stnode.NumChild, stnode.LastChild,
                                                                                                                    stnode.system_type, stnode.hierarchy_type,
                                                                                                                    stnode.value_type, stnode.permissionType,
                                                                                                                    stnode.dimension, stnode.matrix,
                                                                                                                    stnode.comment, stnode.string_size, stnode.udtName);
#endif               
            }
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

                    if((!string.IsNullOrEmpty(stNameEacape))&&(stNameEacape[stNameEacape.Length - 1]=='.'))
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

                ret = ParseTagInfos(ref stnode.node, ref stnode.system_type);
                if (ret != AGL4Sym.AGLSYM_SUCCESS)
                {
                    return (ret);
                }

                ret = ParseValueType(ref stnode.node, ref stnode.permissionType, ref stnode.value_type);
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

            //If the node is a string the extra informations about this string
            if (stnode.system_type == AGL4.SystemType.S7_String || stnode.system_type == AGL4.SystemType.S7_WString)
            {
                ret = ParseStringInfos(ref stnode.node, ref stnode.string_size);
                if (ret != AGL4Sym.AGLSYM_SUCCESS)
                {
                    return (ret);
                }
            }

            if (system_scope_type == AGL4.SystemType.S7_Datablock)
            {

                if ((stnode.hierarchy_type == AGL4.HierarchyType.STRUCTURE))
                {
                    //Obtains the plc system data type
                    ret = AGL4.Symbolic_GetSystemType(stnode.node, ref stnode.system_type);
                    if (ret != AGL4Sym.AGLSYM_SUCCESS)
                    {
                        return (ret);
                    }

                    else if (stnode.system_type == AGL4.SystemType.S7_UDT_Instance)
                    {
                        GetUDTName(stnode.node, ref stnode.udtName);
                        stnode.udt = stnode.udtName;
                    }
                    else if(stnode.system_type == AGL4.SystemType.S7_UDT)
                    {
                        GetUDTName(stnode.node, ref stnode.udtName);
                        stnode.udt = stnode.udtName;
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
                    string szArrayDimensions = string.Empty;
                    stOffset[] OffsetArray = new stOffset[4];
                    uint Dimension = 0;
                    uint ArrayDimension = 1;
                    ret = ParseArrayDimensions(ref stnode.node, ref szArrayDimensions, ref Dimension, OffsetArray, ref ArrayDimension);
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

                    if (stnode.system_type == AGL4.SystemType.S7_UDT_Instance)
                    {
                        GetUDTName(array_type_node, ref stnode.udtName);
                        stnode.udt = stnode.udtName;

                    }
                    if ((stnode.system_type == AGL4.SystemType.S7_UDT_Instance) || (stnode.system_type == AGL4.SystemType.S7_Struct))
                    {
                        stNode starray_type_node = stnode;
                        starray_type_node.node = array_type_node;
                        ParseNodeTiaPortal(ref starray_type_node);
                    }

                    if (ret == -1)
                    {
                        stnode.depthStruct++;
                    }
                }
                else
                {
                    stnode.depthStruct = 0;
                    ret = ParseValueType(ref stnode.node, ref stnode.permissionType, ref stnode.value_type);
                    if (ret != AGL4Sym.AGLSYM_SUCCESS)
                    {
                        stnode.depthStruct = 0;
                        return (ret);
                    }

                    if (!GetTheDisplayAttribute(ref stnode.node, segment_type, stnode.system_type))
                    {
                        return (AGL4.AGL40_SUCCESS);
                    }

                    string szComment = " ";
                    ret = GetComment(ref stnode.node, ref szComment);

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

        void AddPrototypeStructure(ref stNode node, bool isChildren = false, Dictionary<string, StructParamete> map_ListChildrenStructure = null)
        {
            string nameElement;
            string nameStructure;
            if((node.system_type == AGL4.SystemType.S7_UDT_Instance)||(node.system_type == AGL4.SystemType.S7_Struct))
            {
                nameStructure = node.udtName;
                if(!m_mapUDTBaseS7P.ContainsKey(nameStructure))
                {
                    List<string> StringArray = new List<string>();
                    m_mapUDTBaseS7P.Add(nameStructure, StringArray);
                }
                if(isChildren == true)
                {
                    
                    if((node.system_type == AGL4.SystemType.S7_UDT_Instance) && (map_ListChildrenStructure != null))
                    {
                        if (map_ListChildrenStructure.ContainsKey(node.path.ToString()))
                        {
                            StructParamete parLoc = map_ListChildrenStructure[node.path.ToString()];
                            nameStructure = parLoc.nameUdt;                            
                        }
                        nameElement = node.name;
                    }
                    else
                    {
                        nameElement = node.udtName;
                        nameStructure = node.path;
                    }
                    
                    string szType = string.Empty;
                    string szTypeStruct = string.Empty;
                    uint nVarSize = 0;
                    UFUAModel.DataType nType = GetMoviconTypeIdFromTiaPortalType(node.system_type, ref nVarSize, ref szType, ref szTypeStruct);
                    if (m_mapUDTBaseS7P.ContainsKey(nameStructure))
                    {
                        m_mapUDTBaseS7P[nameStructure].Add(string.Format("{0} : {1} ;{2}", nameElement, szTypeStruct, 0));
                    }
                }
            }
            else if(node.hierarchy_type == AGL4.HierarchyType.ITEM)
            {
                nameElement = node.name.Substring(node.path.Length + 1);
                nameStructure = node.udtName;
                string szType = string.Empty;
                string szTypeStruct = string.Empty;
                uint nVarSize = 0;
                UFUAModel.DataType nType = GetMoviconTypeIdFromTiaPortalType(node.system_type, ref nVarSize, ref szType, ref szTypeStruct);
                if (m_mapUDTBaseS7P.ContainsKey(nameStructure))
                {
                    m_mapUDTBaseS7P[nameStructure].Add(string.Format("{0} : {1} ;{2}", nameElement, szTypeStruct, 0));
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
            int ret = AGL4.Symbolic_GetIndexSize(node, ref index_size);
            if (ret != AGL4.AGL40_SUCCESS)
            {
                return (ret);
            }

            string stream = "[";

            for (int i = 0; i < index_size; ++i)
            {
                int value = 0;
                //Get the system specific value of an index component. I.e. for [1,2,4] the single value 1, 2 or 4.
                ret = AGL4.Symbolic_GetIndex(node, i, ref value);
                if (ret != AGL4.AGL40_SUCCESS)
                {
                    return (ret);
                }
                stream = stream + string.Format("{0}", value);
                if (i < index_size - 1)
                {
                    stream = stream + ',';
                }
            }
            stream = stream + ']';
            szIndex = stream;
            return (ret);
        }

        /// The ValueType of the node
        int ParseValueType(ref IntPtr node, ref AGL4.PermissionType permissionType, ref AGL4.ValueType value_type)
        {
            value_type = AGL4.ValueType.UNDEFINED;

            /*Determines the data type that is needed to map the system type to a value on a pc. I.e. z.b S7-Bool => UInt8, S7-Int => Int16 und ULInt => UInt64.
              Types that cannot be mapped directly will be "SystemSpecific" i.e. S7-DTL. For those types there are special converter.*/
            int ret = AGL4.Symbolic_GetValueType(node, ref value_type);
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
                        ret = AGL4.Symbolic_GetPermissionType(node, ref permissionType);

                        if ((permissionType != AGL4.PermissionType.NONE) && (permissionType != AGL4.PermissionType.UNDEFINED))
                        {
                            return (ret);
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
                if (res != AGL4.AGL40_SUCCESS && res != AGL4.AGL40_SYMBOLIC_NOT_APPLICABLE)
                {
                    return res;
                }
            }
            return res;
        }
        int ParseArrayDimensions(ref IntPtr node, ref string arrayDimensions, ref uint Dimension,
                                 stOffset[] st_OffsetArray, ref uint ArrayDimension)
        {
            string stream = string.Empty;

            int dimension_count = 0;
            //Determines the count of dimensions of an array. I.e. Array[1..2] of int => 1. Array[1..2, 1..2] of int => 2.
            int ret = AGL4.Symbolic_GetArrayDimensionCount(node, ref dimension_count);
            if (ret != AGL4.AGL40_SUCCESS)
            {
                return (ret);
            }
            Dimension = (uint)dimension_count;

            stream += '[';
            ArrayDimension = 1;
            for (int i = 0; i < dimension_count; ++i)
            {
                int lower = 0;
                int upper = 0;
                //Determine the lower and upper index value of an array. Array[1..20] of int => {1, 20}. 
                //Array[-1..20, 1..10] of int => for dimension 0 = {-1, 20}, or for dimension 1 = {1, 10}
                ret = AGL4.Symbolic_GetArrayDimension(node, i, ref lower, ref upper);
                if (ret != AGL4.AGL40_SUCCESS)
                {
                    return (ret);
                }

                st_OffsetArray[i].nStartArray = lower;
                st_OffsetArray[i].nHigt = upper;
                st_OffsetArray[i].nSizeArray = (upper - lower) + 1;
                ArrayDimension = (uint)(ArrayDimension * st_OffsetArray[i].nSizeArray);
                stream += (lower + ".." + upper);
                if (i < dimension_count - 1)
                {
                    stream += ", ";
                }
            }
            stream += ']';
            arrayDimensions = stream;

            return (ret);
        }

        bool GetUDTName(IntPtr node, ref string name)
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
            name = udt_name;
            return true;
        }
        public struct StructParamete
        {
            public int parent;
            public int prew_parent;
            public int numChildren;
            public string nameUdt;
            public string prew_ParentName;
            public bool isPrototipe;
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
                m_mapUDTBaseS7P = new Dictionary<string, List<string>>();
            }
            else
            {
                m_mapUDTBaseS7P.Clear();
            }

            if(m_PrototipeVariableStruct == null)
            {
                m_PrototipeVariableStruct = new Dictionary<string, List<stNode>>();
            }
            else
            {
                m_PrototipeVariableStruct.Clear();
            }

            bool udtGroup = false;
            int Parent = -1;
            int globalIID = 0;
            //bool changeNastedStructure = false;
            
            Dictionary<string, StructParamete> map_ListChildrenStructure = new Dictionary<string, StructParamete>();
            int oldParent = Parent;
            //foreach (stNode child in m_ListOfAllChilds)
            for (int count = 0; count < m_ListOfAllChilds.Count; count++)
            {
                stNode child = m_ListOfAllChilds.ElementAt(count);
                child.nameOrig = child.name;
                //Condition for not inserting the ptototypes in the list of tags.
                if ((child.hierarchy_type == AGL4.HierarchyType.STRUCTURE) && (child.system_type == AGL4.SystemType.S7_UDT))
                {
                    udtGroup = true;
                    continue;
                }
            
                if ((child.hierarchy_type == AGL4.HierarchyType.STRUCTURE) && (child.system_type == AGL4.SystemType.S7_Tag_Table))
                {
                    udtGroup = false;
                }
                if(udtGroup)
                {
                    continue;
                }
                
                if ((child.hierarchy_type == AGL4.HierarchyType.STRUCTURE) && (IsAStructure(child.system_type ))) 
                {
                    if ((child.matrix != null ) &&(child.matrix.IndexOf("]")) == child.matrix.Length-1)
                    {                        
                        AddPrototipe(ref child, ref Parent, globalIID, map_ListChildrenStructure);
                    }
                    else
                    {
                        AddStrutture(ref child, ref Parent, globalIID, map_ListChildrenStructure);                                    
                    }

                }

                if (child.hierarchy_type == AGL4.HierarchyType.ITEM)
                {
                    AddItem(ref child, ref Parent, globalIID, map_ListChildrenStructure);
                }
                if ((child.hierarchy_type == AGL4.HierarchyType.ARRAY) && ((child.system_type == AGL4.SystemType.S7_UDT_Instance) || 
                                                                                (child.system_type == AGL4.SystemType.S7_Struct)))
                {
                    if (map_ListChildrenStructure.ContainsKey(child.path.ToString()))
                    {
                        StructParamete parLoc = map_ListChildrenStructure[child.path.ToString()];
                        parLoc.numChildren--;
                        map_ListChildrenStructure[child.path.ToString()] = parLoc;
                    }
                    AddArrayOfStructure(ref globalIID, child);

                }
                else if (child.hierarchy_type == AGL4.HierarchyType.ARRAY)
                {
                    if (map_ListChildrenStructure.ContainsKey(child.path.ToString()))
                    {
                        StructParamete parLoc = map_ListChildrenStructure[child.path.ToString()];
                        parLoc.numChildren--;
                        map_ListChildrenStructure[child.path.ToString()] = parLoc;
                    }
                    AddArray(ref globalIID, child);
                }

                globalIID++;
            }
        }

        bool IsAStructure(AGL4.SystemType struct_system_type)
        {
            bool isAStruct = false;
            switch(struct_system_type)
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
                    isAStruct = true;
                    break;
               default:
                    break;

            }
            return (isAStruct);
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
        void AddPrototipe(ref stNode child, ref int Parent, int globalIID, Dictionary<string, StructParamete> map_ListChildrenStructure)
        {
            if (child.NumChild == 0)
            {
                return;
            }
            string oldname = child.name;
            StructParamete par = new StructParamete();
            par.prew_parent = Parent;
            par.numChildren = child.NumChild;
            par.prew_ParentName = child.name;
            par.isPrototipe = true;
            par.nameUdt= child.udtName;

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
                StructParamete parLoc = map_ListChildrenStructure[child.name.ToString()];
                parLoc.numChildren--;
                map_ListChildrenStructure[oldname] = parLoc;
                AddPrototypeStructure(ref child, true);
            }
            else
            {
                AddPrototypeStructure(ref child);
            }
            if(!m_PrototipeVariableStruct.ContainsKey(oldname))
            {
                List<stNode> stNodeArray = new List<stNode>();
                m_PrototipeVariableStruct.Add(oldname, stNodeArray);
            }
            Parent = globalIID;
            par.parent = Parent;
            map_ListChildrenStructure.Add(oldname, par);
            globalIID++;
        }
        void AddStrutture(ref stNode child, ref int Parent, int  globalIID, Dictionary<string, StructParamete> map_ListChildrenStructure)
        {
            if (child.NumChild == 0)
            {
                return;
            }
            
            StructParamete par = new StructParamete();
            par.prew_parent = Parent;
            par.numChildren = child.NumChild;
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
                par.nameUdt = child.name;
            }
            child.udtName = par.nameUdt;

            if (map_ListChildrenStructure.ContainsKey(child.path.ToString()))
            {
                StructParamete parLoc = map_ListChildrenStructure[child.path.ToString()];
                parLoc.numChildren--;
                map_ListChildrenStructure[child.path.ToString()] = parLoc;                
                AddItemStruct(globalIID, Parent, child);
                AddPrototypeStructure(ref child,true, map_ListChildrenStructure);

                if (!m_PrototipeVariableStruct.ContainsKey(child.name))
                {
                    List<stNode> stNodeArray = new List<stNode>();
                    m_PrototipeVariableStruct.Add(child.name, stNodeArray);
                }

                if (m_PrototipeVariableStruct.ContainsKey(child.path))
                {
                    List<stNode> stNodeArray = new List<stNode>();
                    m_PrototipeVariableStruct[child.path].Add(child);
                }

            }
            else
            {
                AddItem(globalIID, Parent, child);
                AddPrototypeStructure(ref child);

            }
            Parent = globalIID;
            par.parent = Parent;
            map_ListChildrenStructure.Add(child.name, par);
        }

        void AddItem(ref stNode child, ref int Parent, int globalIID, Dictionary<string, StructParamete> map_ListChildrenStructure)
        {
            if (!map_ListChildrenStructure.ContainsKey(child.path.ToString()))
            {
                Parent = -1;
                AddItem(globalIID, Parent, child);
            }
            else
            {
                StructParamete par = map_ListChildrenStructure[child.path.ToString()];
                par.numChildren--;
                map_ListChildrenStructure[child.path.ToString()] = par;
                child.udtName = par.nameUdt;
                AddPrototypeStructure(ref child);

                if (m_PrototipeVariableStruct.ContainsKey(child.path))
                {
                    m_PrototipeVariableStruct[child.path].Add(child);
                }

                if (par.isPrototipe)
                {
                    //if (m_PrototipeVariableStruct.ContainsKey(child.path))
                    //{
                    //    m_PrototipeVariableStruct[child.path].Add(child);
                    //}

                    if (par.numChildren == 0)
                    {
                        map_ListChildrenStructure.Remove(child.path.ToString());
                        return;
                    }
                    return;
                }

                AddItemStruct(globalIID, Parent, child);

                if (par.numChildren == 0)
                {
                    string path = child.path;
                    for (;;)
                    {
                        if (!map_ListChildrenStructure.ContainsKey(path))
                        {
                            break;
                        }
                        par = map_ListChildrenStructure[path];
                        Parent = par.prew_parent;
                        if (par.numChildren == 0)
                        {
                            map_ListChildrenStructure.Remove(path);
                            path = par.prew_ParentName;                            
                        }
                        else
                        {
                            Parent = par.parent;
                            break;
                        }
                    }
                }
            }
        }


        bool AddItem(int globalIID, int Parent, stNode node)
        {
            bool ret = true;
            uint nVarSize = 0;

            string szAddressBoolMaster = string.Empty;
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
            IVar.szAddress = node.name;
            IVar.lID = globalIID;
            IVar.lParent = Parent;
            IVar.nType = nType;
            IVar.szType = szType;
            IVar.nSize = nVarSize;
            IVar.nElemType = nType;
            IVar.ArrayDimension = node.ArrayDimension;
            IVar.S7DataFormat = node.system_type;
            if((node.system_type == AGL4.SystemType.S7_UDT_Instance)|| (node.system_type == AGL4.SystemType.S7_Struct))
            {
                if(node.udtName == "S7_DTL")
                {
                    IVar.S7DataFormat = AGL4.SystemType.S7_DTL;
                }
                else
                {
                  IVar.S7DataFormat = AGL4.SystemType.S7_Struct;
                }
                
                IVar.szType = node.udtName;
                IVar.ImportType = ImportTypes.Struct;
            }
            if ((node.system_type == AGL4.SystemType.S7_String) || (node.system_type == AGL4.SystemType.S7_WString))
            {
                IVar.StringLength = (uint)node.string_size;
                IVar.nSize = (uint)node.string_size;
                IVar.lParent = -1;
            }

            m_MapImportVariable[IVar.lID] = IVar;
            return (ret);
        }
        bool AddItemStruct(int globalIID, int Parent, stNode node)
        {
            bool ret = true;
            uint nVarSize = 0;

            string szAddressBoolMaster = string.Empty;
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
            IVar.szAddress = node.name;
            IVar.lID = globalIID;
            IVar.lParent = Parent;
            IVar.nType = nType;
            IVar.szType = szType;
            IVar.nSize = nVarSize;
            IVar.nElemType = nType;
            IVar.S7DataFormat = node.system_type;
            if ((node.system_type == AGL4.SystemType.S7_UDT_Instance) || (node.system_type == AGL4.SystemType.S7_Struct))
            {
                if (node.udtName == "S7_DTL")
                {
                    IVar.S7DataFormat = AGL4.SystemType.S7_DTL;
                    IVar.lParent = -1;
                    IVar.szName = node.name.Trim('\"');
                }
                else
                {
                    IVar.S7DataFormat = AGL4.SystemType.S7_Struct;
                }
                IVar.szType = node.udt;
                IVar.ImportType = ImportTypes.Struct;

            }

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

            m_MapImportVariable[IVar.lID] = IVar;
            return (ret);
        }
        bool AddArray(ref int globalIID, stNode node)
        {
            bool ret = false;
            UInt16[] stdim = new UInt16[node.dimension * 2];
            ret = ParseArrayDimensions(node, ref stdim);
            if (ret == false)
            {
                return (ret);
            }

            
            if(node.dimension == 1)
            {
                //calculate the number of elements                
                uint numberItem = (uint)(stdim[1] - stdim[0] + 1);
                    
                int parent = -1;
                if(node.system_type != AGL4.SystemType.S7_String)
                {
                    node.ArrayDimension = numberItem;
                    AddItem(globalIID, parent, node);
                    parent = globalIID;
                    globalIID++;
                }
                stNode local;
                if (node.dimension == 1)
                {
                    for (int index = stdim[0]; index <= stdim[1]; index++)
                    {
                        local = node;
                        local.name = local.name + "[" + index.ToString() + "]";
                        local.ArrayDimension = 0;
                        AddItem(globalIID, parent, local);
                        globalIID++;
                    }
                }
            }
            if (node.dimension == 2)
            {
                stNode local;
                //calculate the number of elements                
                uint numberItem = (uint)((stdim[3] - stdim[2] + 1) * (stdim[1] - stdim[0] + 1));

                int parent = -1;
                if (node.system_type != AGL4.SystemType.S7_String)
                {
                    node.ArrayDimension = numberItem;
                    AddItem(globalIID, parent, node);
                    parent = globalIID;
                    globalIID++;
                }

                for (int index2 = stdim[0]; index2 <= stdim[1]; index2++)
                {
                    for (int index = stdim[2]; index <= stdim[3]; index++)
                    {
                        local = node;
                        local.name = local.name + "[" + index2.ToString()+ "," + index.ToString() + "]";
                        local.ArrayDimension = 0;
                        AddItem(globalIID, parent, local);
                        globalIID++;
                    }
                }
            }
            if (node.dimension == 3)
            {
                //calculate the number of elements                
                uint numberItem = (uint)((stdim[5] - stdim[4] + 1) * (stdim[3] - stdim[2] + 1) * (stdim[1] - stdim[0] + 1));
                stNode local;
                int parent = -1;
                if (node.system_type != AGL4.SystemType.S7_String)
                {
                    node.ArrayDimension = numberItem;
                    AddItem(globalIID, parent, node);
                    parent = globalIID;
                    globalIID++;
                }
                for (int index3 = stdim[0]; index3 <= stdim[1]; index3++)
                {
                    for (int index2 = stdim[2]; index2 <= stdim[3]; index2++)
                    {

                        for (int index = stdim[4]; index <= stdim[5]; index++)
                        {
                            local = node;
                            local.name = local.name + "[" + index3.ToString() + ","+ index2.ToString() + "," + index.ToString() + "]";
                            local.ArrayDimension = 0;
                            AddItem(globalIID, parent, local);
                            globalIID++;
                        }
                    }
                }
            }

            return (ret);
        }

        bool AddArrayOfStructure(ref int globalIID, stNode node)
        {
            bool ret = false;
            UInt16[] stdim = new UInt16[node.dimension * 2];
            ret = ParseArrayDimensions(node, ref stdim);
            if (ret == false)
            {
                return (ret);
            }


            if (node.dimension == 1)
            {
                //calculate the number of elements                
                uint numberItem = (uint)(stdim[1] - stdim[0] + 1);
                stNode local;
                if (node.dimension == 1)
                {
                    for (int index = stdim[0]; index <= stdim[1]; index++)
                    {
                        local = node;
                        local.name = local.name + "[" + index.ToString() + "]";
                        local.path = node.name;
                        local.udtName = node.name;

                        local.hierarchy_type = AGL4.HierarchyType.STRUCTURE;
                        local.system_type = AGL4.SystemType.S7_Struct;

                        AddElementOfStructure(local, ref globalIID);
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
                        local.name = local.name + "[" + index2.ToString() + "," + index.ToString() + "]";
                        local.path = node.name;
                        local.udtName = node.name;

                        local.hierarchy_type = AGL4.HierarchyType.STRUCTURE;
                        local.system_type = AGL4.SystemType.S7_Struct;

                        AddElementOfStructure(local, ref globalIID);
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
                            local.name = local.name + "[" + index3.ToString() + "," + index2.ToString() + "," + index.ToString() + "]";
                            local.path = node.name;
                            local.udtName = node.name;

                            local.hierarchy_type = AGL4.HierarchyType.STRUCTURE;
                            local.system_type = AGL4.SystemType.S7_Struct;

                            AddElementOfStructure(local, ref globalIID);
                        }
                    }
                }
            }

            return (ret);
        }

        void AddElementOfStructure(stNode local, ref int globalIID)
        {
            if(!m_PrototipeVariableStruct.ContainsKey(local.udtName))
            {
                return;
            }

            List<stNode> items = m_PrototipeVariableStruct[local.udtName];

            int parent = -1;
            AddItem(globalIID, parent, local);
            parent = globalIID;
            globalIID++;
            for(int index = 0; index < items.Count; index++) //foreach(stNode item in items)
            {
                stNode item = items[index];
                string baseArrayName = local.name;
                string tempName = item.name.Substring(item.path.Length +1);
                item.path = baseArrayName;
                baseArrayName = baseArrayName + "." + tempName;
                item.name = baseArrayName;                               
                AddItemStruct(globalIID, parent, item);
                
                if (item.hierarchy_type == AGL4.HierarchyType.STRUCTURE)
                {
                    AddOfStructure(item, ref globalIID);
                }
                else
                {
                    globalIID++;
                }           
            }
        }
        void AddOfStructure(stNode local, ref int globalIID)
        {
            List<stNode> items;
            if (m_PrototipeVariableStruct.ContainsKey(local.udtName))
            {
                items = m_PrototipeVariableStruct[local.udtName];                
            }
            else if(m_PrototipeVariableStruct.ContainsKey(local.nameOrig))
            {
                items = m_PrototipeVariableStruct[local.nameOrig];
            }
            else if (m_PrototipeVariableStruct.ContainsKey(local.path))
            {
                items = m_PrototipeVariableStruct[local.path];
            }
            else
            {
                return;
            }            

            int parent = globalIID;
            globalIID++;
            for (int index = 0; index < items.Count; index++) //foreach(stNode item in items)
            {
                stNode item = items[index];
                string baseArrayName = local.name;
                string tempName = item.name.Substring(item.path.Length + 1);
                item.path = baseArrayName;
                baseArrayName = baseArrayName + "." + tempName;
                item.name = baseArrayName;
                AddItemStruct(globalIID, parent, item);
                if (item.hierarchy_type == AGL4.HierarchyType.STRUCTURE)
                {
                    AddOfStructure(item, ref globalIID);
                }
                else
                {
                    globalIID++;
                }
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
                nType = UFUAModel.DataType.Byte;
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
                nType = UFUAModel.DataType.Double;
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
            else if (system_type == AGL4.SystemType.S7_LInt)
            {
                nType = UFUAModel.DataType.Double;
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
            else
            {
                nType = UFUAModel.DataType.Boolean;
                VarSize = 0;
            }

            return (nType);
        }

        public Dictionary<long, ImportVariable> GetMapImportVariable()
        {
            return (m_MapImportVariable);
        }
        public Dictionary<string, List<string>> GetMapUDTBaseS7P()
        {
            return (m_mapUDTBaseS7P);
        } 

    }
}
