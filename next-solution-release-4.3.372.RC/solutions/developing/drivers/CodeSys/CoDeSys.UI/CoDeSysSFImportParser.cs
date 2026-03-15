using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Xml.Linq;
using DriverCodeBase.UI;

namespace CoDeSys.UI
{
    public class CoDeSysSFImportParser : CoDeSysImportBase, IDisposable
    {
        #region Properties
        Dictionary<string, ImportPlcSymbolDesc> _MapSTRUCTLowLevel;
        #endregion

        #region data member

        private string _SymbolFile;
        public string SymbolFile
        {
            set {
                _SymbolFile = value;
                _SymbolFilePath = Path.GetDirectoryName(value);
                _SymbolFileName = Path.GetFileName(value);
            }
            get { return _SymbolFile; }
        }

        private string _SymbolFileName;
        private string _SymbolFilePath;

        private string _SymbolFileVersion;
        public string SymbolFileVersion { get { return _SymbolFileVersion; } }

        #endregion


        public CoDeSysSFImportParser() : base()
        {
            _SymbolFileName = string.Empty;
            _SymbolFilePath = string.Empty;
            _SymbolFileVersion = string.Empty;
            _MapSTRUCTLowLevel = new Dictionary<string, ImportPlcSymbolDesc>();
        }


        #region Codesys Symbolic file
                       
        /// <summary>
        /// Browse node list to find specific data type
        /// </summary>
        /// <param name="nodes"></param>
        /// <param name="typeName"></param>
        /// <returns></returns>
        private XElement GetElementFromNodeList(List<XNode> nodes, string typeName)
        {
            XElement Result = null;

            foreach (XElement TaskElement in nodes) {
                if ( (TaskElement.Name.LocalName == "TypeUserDef" || TaskElement.Name.LocalName == "TypeArray")
                    && (TaskElement.Attributes().Where(p => p.Name.LocalName == "name").Where(p => p.Value == typeName).Count() == 1))
                {
                    Result = TaskElement;
                    break;
                }
            }

            return Result;
        }

        /// <summary>
        /// Check if element is an array type --> if so, get information about it
        /// </summary>
        /// <param name="element"></param>
        /// <param name="arrayDeclaration"></param>
        /// <param name="arrayType"></param>
        /// <returns></returns>
        private bool GetArrayElementInfo(XElement element, out string arrayDeclaration, out string arrayType)
        {
            arrayDeclaration = null;
            arrayType = null;

            if (element.Attribute("typeclass").Value == "Array")
            {
                arrayDeclaration = element.Attribute("iecname").Value;
                arrayType = element.Attribute("basetype").Value;
            }
            return (!string.IsNullOrEmpty(arrayDeclaration));
        }

        private void AddMemberOfStructInOrder(ref List<CDSImportedVariable> members, CDSImportedVariable item)
        {
            if (members.Count == 0)
            {
                members.Add(item);
                return;
            }
            if (members[members.Count - 1].CompareTo(item) <= 0)
            {
                members.Add(item);
                return;
            }
            if (members[0].CompareTo(item) >= 0)
            {
                members.Insert(0, item);
                return;
            }
            int index = members.BinarySearch(item);
            if (index < 0)
                index = ~index;
            members.Insert(index, item);
        }

        /// <summary>
        /// Convert struct definition from Symbolic file's format to PLC Import format
        /// </summary>
        /// <param name="nodes"></param>
        /// <param name="parentPrefix"></param>
        /// <param name="structElement"></param>
        /// <param name="listOfElements"></param>
        private void ParseSFDataTypeStructTPY(XElement structElement, string parentPrefix, List<XNode> nodes)
        {
            List<CDSImportedVariable> ListOfElements = new List<CDSImportedVariable>();
            string TypeName = ParseStructName(structElement.Attribute("iecname").Value);

            // browse structure member's list
            foreach (XElement Member in structElement.Nodes())
            {
                string FullTypeName = null;
                string FullElementName = null;
                ImportPlcSymbolDesc Element = new ImportPlcSymbolDesc();

                FullElementName = Member.Attribute("iecname").Value;
                Element.pszName = parentPrefix + Member.Attribute("iecname").Value;

                FullTypeName = Member.Attribute("type").Value;
                Element.pszType = RemoveTUnderscore(Member.Attribute("type").Value);

                if (!string.IsNullOrEmpty(Element.pszName) && !string.IsNullOrEmpty(Element.pszType))
                {
                    Element.ulOffset = 0;
                    // standard data type add directly to list
                    if (CoDeSysProtocol.IsStandardDataType(Element.pszType))
                    {
                        Element.ulTypeId = (uint)CoDeSysProtocol.GetVarType(Element.pszType);
                        Element.ulSize = CoDeSysProtocol.GetByteSizeOfVarType((CoDeSysProtocol.VarType)Element.ulTypeId);

                        //ListOfElements.Add(Element);
                    }
                    else // structure
                    {
                        Element.ulSize = 0;
                        Element.ulTypeId = (uint)(CoDeSysProtocol.VarType.VAR_TYPE_STRUCT);

                        // fecth structure node info from file
                        XElement NewElement = GetElementFromNodeList(nodes, FullTypeName);
                        if (NewElement != null)
                        {
                            string ArrayDeclaration;
                            string ArrayType;
                            // array type --> get declaration array [xx..xx] of type 
                            if (GetArrayElementInfo(NewElement, out ArrayDeclaration, out ArrayType))
                            {
                                //// standard array type --> add directly
                                //if (CoDeSysProtocol.IsStandardDataType(RemoveTUnderscore(ArrayType)))
                                //{
                                    Element.pszType = ArrayDeclaration;
                                    Element.ulTypeId = (uint)(CoDeSysProtocol.GetVarType(RemoveTUnderscore(ArrayType)));
                                //}
                            }
                        }
                    }
                    if (Element != null)
                    {
                        CDSImportedVariable NewVar = ParseVariablesDefinition(Element);
                        if (NewVar != null)
                            // keep sorted members of struct to allign with direct import where members of struct are sorted
                            AddMemberOfStructInOrder(ref ListOfElements, NewVar);
                            
                    }
                }
            }

            if (ListOfElements != null && ListOfElements.Count > 0)
            {
                if (!MapSTRUCT.ContainsKey(TypeName))
                    MapSTRUCT.Add(TypeName, ListOfElements);

                if (!_MapSTRUCTLowLevel.ContainsKey(TypeName))
                {
                    ImportPlcSymbolDesc Element = new ImportPlcSymbolDesc();

                    // structure always require a var with name and type declaration
                    Element = new ImportPlcSymbolDesc();
                    Element.pszName = TypeName;
                    Element.ulSize = 0;
                    Element.pszType = RemoveTUnderscore(structElement.Attribute("iecname").Value);
                    Element.ulTypeId = (uint)(CoDeSysProtocol.VarType.VAR_TYPE_STRUCT);
                    Element.ulOffset = 0;

                    _MapSTRUCTLowLevel.Add(TypeName, Element);
                }
            }
        }

        /// <summary>
        /// Array of element are definied as data type --> import separately
        /// </summary>
        /// <param name="arrayElement"></param>
        /// <param name="element"></param>
        /// <returns></returns>
        private void ParseASDataTypeArrayTPY(XElement arrayElement)
        {
            ImportPlcSymbolDesc Element = new ImportPlcSymbolDesc();

            // structure always require a var with name and type declaration
            Element = new ImportPlcSymbolDesc();
            Element.pszName = RemoveTUnderscore(arrayElement.Attribute("name").Value);            
            Element.ulSize = uint.Parse(arrayElement.Attribute("size").Value);            
            Element.pszType = RemoveTUnderscore(arrayElement.Attribute("iecname").Value);            
            Element.ulOffset = 0;
       
            if (CoDeSysProtocol.IsStandardDataType(arrayElement.Attribute("basetype").Value))
                Element.ulTypeId = (uint)(CoDeSysProtocol.GetVarType(arrayElement.Attribute("basetype").Value));
            else
                Element.ulTypeId = (uint)(CoDeSysProtocol.VarType.VAR_TYPE_STRUCT);
            
            if (!_MapSTRUCTLowLevel.ContainsKey(Element.pszName))
                _MapSTRUCTLowLevel.Add(Element.pszName, Element);
        }


        /// <summary>
        /// Retrive the list of var from symbolic file
        /// </summary>
        /// <param name="node"></param>
        /// <param name="parentPrefix"></param>
        /// <param name="mapOfDataType"></param>
        /// <param name="varList"></param>
        private void ParseASVariablesDefinition(XElement node, string parentPrefix)
        {
            parentPrefix += node.Attribute("name").Value + ".";

            foreach (XElement NodeElement in node.Nodes())
            {
                string FullTypeName;
                if (NodeElement.Attributes().Where(p => p.Name.LocalName == "type").Count() > 0)
                {
                    // assign basic var information
                    ImportPlcSymbolDesc Element = new ImportPlcSymbolDesc();
                    Element.pszName = parentPrefix + NodeElement.Attribute("name").Value;
                    FullTypeName = NodeElement.Attribute("type").Value;
                    Element.pszType = RemoveTUnderscore(NodeElement.Attribute("type").Value);

                    Element.ulOffset = 0;
                    // standard data type --> add directly to list
                    if (CoDeSysProtocol.IsStandardDataType(Element.pszType))
                    {
                        Element.ulTypeId = (uint)CoDeSysProtocol.GetVarType(Element.pszType);
                        Element.ulSize = CoDeSysProtocol.GetByteSizeOfVarType((CoDeSysProtocol.VarType)Element.ulTypeId);
                    }
                    else // structure
                    {
                        Element.ulSize = 0;
                        // array + structure
                        if (_MapSTRUCTLowLevel.ContainsKey(RemoveTUnderscore(FullTypeName)))
                        {
                            ImportPlcSymbolDesc StructDeclaration = _MapSTRUCTLowLevel[RemoveTUnderscore(FullTypeName)];
                            // assign correct name type and size 
                            Element.pszType = StructDeclaration.pszType;
                            Element.ulOffset = StructDeclaration.ulOffset;
                        } else
                        {
                            Element = null;
                        }
                    }

                    if (Element != null)
                    {
                        List<CDSImportedVariable> ArrayOfStructInStruct = new List<CDSImportedVariable>();
                        CDSImportedVariable NewVar = ParseVariablesDefinition(Element, ref ArrayOfStructInStruct);
                        if (NewVar != null)
                        {
                            ParsedVars.Add(NewVar);
                            if (ArrayOfStructInStruct.Count > 0)
                                ParsedVars.AddRange(ArrayOfStructInStruct);
                        }
                    }
                }
                else
                {   // continue to browse to next note (next level)
                    ParseASVariablesDefinition(NodeElement, parentPrefix);
                }
            }
        }

        /// <summary>
        /// Retrive the list of var and the list of struct from symbolic file
        /// </summary>
        /// <param name="node"></param>
        /// <param name="parentPrefix"></param>
        /// <param name="mapOfDataType"></param>
        /// <param name="varList"></param>
        private bool ParseSymbolicFile(out string errorMessage)
        {
            bool Result = true;
            XDocument xdoc;

            errorMessage = string.Empty;

            try
            {
                // Load the file
                xdoc = XDocument.Load(_SymbolFile);
                if (xdoc != null)
                {
                    List<XNode> nodes = xdoc.Elements().Where(p => p.Name.LocalName == "Symbolconfiguration").Elements().Where(q => q.Name.LocalName == "TypeList").Nodes().ToList();

                    // import array
                    foreach (XElement TaskElement in nodes)
                    {
                        if (TaskElement.Name.LocalName == "TypeArray")// && (TaskElement.Attributes().Where(p => p.Name.LocalName == "typeclass").Where(p => p.Value == "Array").Count() == 1))
                            ParseASDataTypeArrayTPY(TaskElement);
                    }

                    // import struct
                    foreach (XElement TaskElement in nodes)
                    {
                        if (TaskElement.Name.LocalName == "TypeUserDef" && ((TaskElement.Attributes().Where(p => p.Name.LocalName == "pouclass").Where(p => (p.Value == "STRUCTURE" || p.Value == "FUNCTION_BLOCK")).Count() == 1) ||
                            (TaskElement.Attributes().Where(p => p.Name.LocalName == "typeclass").Where(p => (p.Value == "Userdef")).Count() == 1)))
                        {
                            List <ImportPlcSymbolDesc> Members = new List<ImportPlcSymbolDesc>();
                            ParseSFDataTypeStructTPY(TaskElement, string.Empty, nodes);
                        }
                    }
                                        
                    // import var list                    
                    foreach (XElement NodeElement in xdoc.Elements().Where(p => p.Name.LocalName == "Symbolconfiguration").Elements().Where(p => p.Name.LocalName == "NodeList").Nodes())
                        ParseASVariablesDefinition(NodeElement, string.Empty);
                }
            }
            catch (Exception ex)
            {
                Result = false;
                errorMessage = ex.Message;                
            }
            finally
            {
                xdoc = null;
            }

            return Result;
        }

        //private void ConvertConstantDataTypeToInt32()
        //{
        //    string EnumDataType = "i32";
        //    uint EnumVarSize = 0;

        //    CoDeSysProtocol.GetVarTypeAndSize(ref EnumDataType, out EnumVarSize);

        //    foreach (var DataType in MapSTRUCT.Values)
        //    {
        //        foreach (CDSImportedVariable Member in DataType.FindAll(m => m.Category == ImportUDTType.Enum)) {                     
        //            //Member.VarType = EnumDataType;
        //            Member.VarSize = EnumVarSize;
        //        }
        //    }

        //    foreach (CDSImportedVariable Var in ParsedVars.FindAll(m => m.Category == ImportUDTType.Enum))
        //    {
        //        //Var.VarType = EnumDataType;
        //        Var.VarSize = EnumVarSize;
        //    }
        //}

        public ImportDataModel Import(GetStationName readStationName, string station)
        {
            _ImportDataModel = null;
                        
            string ErrorMessage;
            // retrive vars definition, data type definition, ecc from symbolic file
            if (!ParseSymbolicFile(out ErrorMessage)) {
                base.LastError = ErrorMessage;
                return _ImportDataModel;
            }

            //ConvertConstantDataTypeToInt32();
            // base method to fill list of imported var for user
            _ImportDataModel = ParsePlcData(readStationName);

            return _ImportDataModel;
        }             

        /// <summary>
        /// Test if selected file is a valid symbol file of CoDeSys PLC's project
        /// </summary>
        /// <returns></returns>
        public bool IsSymbolFile()
        {
            bool IsValid = false;
            XDocument xdoc = null;

            try
            {
                // Load the file
                xdoc = XDocument.Load(_SymbolFile);
                if (xdoc != null)
                {
                    foreach (XElement TaskElement in xdoc.Elements().Where(p => p.Name.LocalName == "Symbolconfiguration").Elements().Where(q => q.Name.LocalName == "Header").Nodes()) {
                        string Name = null;
                        string Source = null;
                        foreach (XAttribute Attr in TaskElement.Attributes())
                        {
                            switch (Attr.Name.LocalName.ToLower())
                            {
                                case "name":
                                    Name = Attr.Value;
                                    IsValid = true;
                                    break;
                                case "version":
                                    _SymbolFileVersion = Attr.Value;
                                    Source = Attr.Value;
                                    IsValid = true;
                                    break;
                            }
                        }
                    }                   
                }
            }
            catch (Exception ex) { }
            finally
            {
                xdoc = null;
            }

            return (IsValid);
        }        
        #endregion                
        
        public virtual void Dispose()
        {
        }
    }
}
