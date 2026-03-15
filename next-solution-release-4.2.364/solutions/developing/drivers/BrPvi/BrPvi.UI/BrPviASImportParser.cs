using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Xml.Linq;
using UFUAModel;
using DriverCodeBase.UI;
using DriverCodeBase.UI.Controls;


namespace BrPvi.UI
{
    public class BrPviASImportParser : BrPviImportBase, IDisposable
    {
        #region data member
        private string _ProjectFile;
        public string ProjectFile {
            set {
                _ProjectFile = value;
                _ProjectPath = Path.GetDirectoryName(value);
                _ProjectName = Path.GetFileName(value);
            }
            get { return _ProjectFile; }
        }

        private string _ProjectName = string.Empty;
        private string _ProjectPath = string.Empty;
        private string _ProjectVersion = string.Empty;
        
        public string ProjectVersion { get { return _ProjectVersion; } }
        private List<ASImportedVariable> _ParsedVars = null;        
        //private Dictionary<string, string[]> _mapFunctionBlock;
        private Dictionary<string, List<ASImportedVariable>> _mapSTRUCT;
        private Dictionary<string, string[]> _mapUDT;
        private Dictionary<string, int> _mapConstants;

        private Dictionary<string, ASProgram> _mapPrograms;

        private enum Parse
        {
            Variable,
            MemberOfDataType
        }
        #endregion

        /// <summary>
        /// Object using to rapresent elements of Automation Studio Projects (file, directory, ecc)
        /// </summary>
        private class ASNode {
            public enum AsType
            {
                Unknown,
                Package_SubDirectory,    // subdirectory
                Program_SubDirectory,    // subdirectory
                Library_SubDirectory,    // subdirectory
                VariableDefinition,
                VariableDataTypeDefinition,
                Package_Physical,    // subdirectory
                Config_Physical,    // subdirectory
                SWMap
            }

            public string Name { get; set; }
            public string Path { get; set; }
            public string FileName { get; set; }
            public AsType Type { get; set; }
            // higher number, low priority
            public int Priority { get; set; }
            public ASNode ParentNode { get; set; }
            //relative taskname
            public string ProgramPath { get; set; }
            private void DefaultConstructor()
            {
                Name = string.Empty;
                Path = string.Empty;
                FileName = string.Empty;
                Type = AsType.Unknown;
                Priority = 9999;
                ParentNode = null;
                ProgramPath = string.Empty;
            }
            public ASNode()
            {
                DefaultConstructor();
            }

            public ASNode(ASNode parentNode, string nodePath, string fileInfo, string name, AsType type, int priority)
            {
                DefaultConstructor();
                Name = name;
                Type = type;
                Path = nodePath;
                FileName = fileInfo;
                Priority = priority;
                ParentNode = parentNode;
                if (parentNode != null)
                    ProgramPath = parentNode.ProgramPath;
            }

            public bool IsRootNode()
            {
                return (ParentNode.ParentNode == null);
            }

            public bool IsParentNodeLibrary()
            {
                if (ParentNode == null)
                    return false;
                else
                    return (ParentNode.Type == AsType.Library_SubDirectory);
            }

            public void CombineProgramPath(string path,string program)
            {
                if (String.IsNullOrEmpty(path))
                    ProgramPath = program;
                else
                    ProgramPath = string.Format("{0}.{1}",path, program);
            }
        }

        public class ASImportedVariable : ICloneable
        {
            public string VariableName { get; set; }
            public string TaskName { get; set; }
            public string Description { get; set; }
            public string VarDeclaration { get; set; }
            public string VarType { get; set; }
            public ImportUDTType Category { get; set; }
            public string VarTypeOfStruct { get; set; }
            public uint VarSize { get; set; }
            public uint ElementsNumber { get; set; }            
            public uint ArrayDimCount { get; set; }
            public uint[] ArrayStartIndexes { get; set; }
            public uint[] ArrayIndexLimits { get; set; }

            public ASImportedVariable()
            {
                VariableName = string.Empty;
                TaskName = string.Empty;
                Description = string.Empty;
                VarDeclaration = string.Empty;
                VarType = string.Empty;
                Category = ImportUDTType.Unknown;
                VarTypeOfStruct = string.Empty;
                VarSize = 0;
                ElementsNumber = 0;
                ArrayDimCount = 0;
                ArrayStartIndexes = null;
                ArrayIndexLimits = null;
            }

            #region ICloneable Members
            public object Clone()
            {
                return MemberwiseClone();
            }

            public ASImportedVariable CastedClone()
            {
                return (ASImportedVariable)MemberwiseClone();
            }
            #endregion
        }

        /// <summary>
        /// Contain information about program running on PLC
        /// </summary>
        private class ASProgram
        {
            public string Name { get; set; }
            public string Source { get; set; }

            public ASProgram()
            {
                Name = string.Empty;
                Source = string.Empty;
            }

            public ASProgram(string name, string source)
            {
                Name = name;
                Source = source;
            }
        }

        public BrPviASImportParser() : base()
        {            
        }


        #region Automation studio Project Parser
        private List<ASNode> GetASNodeSWMapInfoFile(ASNode node)
        {
            List<ASNode> Elements = new List<ASNode>();
            XDocument xdoc;
            string NodeFile = string.Empty;

            NodeFile = Path.Combine(node.Path, node.FileName);

            try
            {
                // Load the file
                xdoc = XDocument.Load(NodeFile);
                if (xdoc != null)
                {
                    //query elements
                    switch (node.Type)
                    {
                        case ASNode.AsType.Package_Physical:
                            foreach (XElement Element in xdoc.Elements().Where(p => p.Name.LocalName == "Physical").Elements().Where(q => q.Name.LocalName == "Objects").Nodes())
                            {
                                if (Element.NodeType == System.Xml.XmlNodeType.Element)
                                {
                                    switch (Element.FirstAttribute.Value)
                                    {
                                        case "Configuration": //subdirectory has only one attribute
                                            if (Element.Attributes().ToList().Count == 1)
                                                Elements.Add(new ASNode(node, Path.Combine(node.Path, Element.Value), "Config.pkg", Element.Value, ASNode.AsType.Config_Physical, 0));
                                            break;
                                    }
                                }
                            }
                            break;
                        case ASNode.AsType.Config_Physical:
                            foreach (XElement Element in xdoc.Elements().Where(p => p.Name.LocalName == "Configuration").Elements().Where(q => q.Name.LocalName == "Objects").Nodes())
                            {
                                if (Element.NodeType == System.Xml.XmlNodeType.Element)
                                {
                                    switch (Element.FirstAttribute.Value)
                                    {
                                        case "Cpu": //subdirectory has only one attribute
                                            if (Element.Attributes().ToList().Count == 1)
                                                Elements.Add(new ASNode(node, Path.Combine(node.Path, Element.Value), "Cpu.sw", Element.Value, ASNode.AsType.SWMap,0));
                                            break;
                                    }
                                }
                            }
                            break;
                    }
                }
            }
            catch (Exception ex)
            {
                base.LastError = ex.Message;
                Elements = new List<ASNode>();
            }
            finally
            {
                xdoc = null;
            }

            // resort imported elements --> first if all, import data types
            return Elements.OrderBy(s => s.Priority).ToList();
        }

        /// <summary>
        /// Retrive information about directory's contents --> a list of file with scope
        // </summary>        
        private List<ASNode> GetASNodeInfoFile(ASNode node)
        {
            List<ASNode> Elements = new List<ASNode>();
            XAttribute Attr = null;
            XDocument xdoc;
            ASNode NewNode;
            string NodeFile = string.Empty;

            NodeFile = Path.Combine(node.Path, node.FileName);

            try
            {
                // Load the file
                xdoc = XDocument.Load(NodeFile);
                if (xdoc != null)
                {
                    //query elements
                    switch (node.Type)
                    {
                        case ASNode.AsType.Package_SubDirectory:
                            foreach (XElement Element in xdoc.Elements().Where(p => p.Name.LocalName == "Package").Elements().Where(q => q.Name.LocalName == "Objects").Nodes()) {
                                if (Element.NodeType == System.Xml.XmlNodeType.Element) {
                                    switch (Element.FirstAttribute.Value) {
                                        case "File":
                                            Attr = Element.Attributes().ToList().Find(p => p.Name == "Description");
                                            if (Attr != null)
                                            {
                                                switch (Attr.Value)
                                                {
                                                    case "Global data types":
                                                        Elements.Add(new ASNode(node, node.Path, Element.Value, Path.GetFileNameWithoutExtension(Element.Value), ASNode.AsType.VariableDataTypeDefinition, 1));
                                                        break;
                                                    case "Global variables":
                                                        Elements.Add(new ASNode(node, node.Path, Element.Value, Path.GetFileNameWithoutExtension(Element.Value), ASNode.AsType.VariableDefinition, 2));
                                                        break;
                                                }
                                            }
                                            break;
                                        case "Package": //subdirectory has only one attribute
                                            if (Element.Attributes().ToList().Count == 1)
                                            {
                                                NewNode = new ASNode(node, Path.Combine(node.Path, Element.Value), "Package.pkg", Element.Value, ASNode.AsType.Package_SubDirectory, 20);
                                                NewNode.CombineProgramPath(node.ProgramPath, Element.Value);
                                                Elements.Add(NewNode);
                                            }
                                            break;
                                        case "Program": // program subdirectory contain other variables declaration
                                            NewNode = new ASNode(node, Path.Combine(node.Path, Element.Value), "IEC.prg", Element.Value, ASNode.AsType.Program_SubDirectory, 10);
                                            NewNode.CombineProgramPath(node.ProgramPath, Element.Value);
                                            Elements.Add(NewNode);
                                            break;
                                    }
                                }
                            }
                            break;
                        case ASNode.AsType.Program_SubDirectory:
                            //if program (with path) is not contained into this list means that program was not downloaded to Plc --> no needs to parse 
                            if (_mapPrograms.ContainsKey(node.ProgramPath))
                            {
                                foreach (XElement Element in xdoc.Elements().Where(p => p.Name.LocalName == "Program").Elements().Where(q => q.Name.LocalName == "Files").Nodes())
                                {
                                    if (Element.NodeType == System.Xml.XmlNodeType.Element)
                                    {
                                        Attr = Element.Attributes().ToList().Find(p => p.Name == "Description");
                                        if (Attr != null)
                                        {
                                            switch (Attr.Value)
                                            {
                                                case "Local data types":
                                                    Elements.Add(new ASNode(node, node.Path, Element.Value, Path.GetFileNameWithoutExtension(Element.Value), ASNode.AsType.VariableDataTypeDefinition, 1));
                                                    break;
                                                case "Local variables":
                                                    Elements.Add(new ASNode(node, node.Path, Element.Value, Path.GetFileNameWithoutExtension(Element.Value), ASNode.AsType.VariableDefinition, 2));
                                                    break;
                                            }
                                        }
                                    }

                                }
                            }
                            break;
                    }
                }
            }
            catch (Exception ex) {
                base.LastError = ex.Message;
                Elements = new List<ASNode>();
            }
            finally
            {
                xdoc = null;
            }

            // resort imported elements --> first if all, import data types
            return Elements.OrderBy(s => s.Priority).ToList();
        }

        /// <summary>
        /// Retrive information about directory's contents --> a list of file with scope
        // </summary>        
        private List<ASNode> GetASNodeLibraryInfoFile(ASNode node)
        {
            List<ASNode> Elements = new List<ASNode>();
            XAttribute Attr = null;
            XDocument xdoc;
            ASNode NewNode;
            string NodeFile = string.Empty;

            NodeFile = Path.Combine(node.Path, node.FileName);

            try
            {
                // Load the file
                xdoc = XDocument.Load(NodeFile);
                if (xdoc != null)
                {
                    //query elements
                    switch (node.Type)
                    {
                        case ASNode.AsType.Package_SubDirectory:
                            foreach (XElement Element in xdoc.Elements().Where(p => p.Name.LocalName == "Package").Elements().Where(q => q.Name.LocalName == "Objects").Nodes())
                            {
                                if (Element.NodeType == System.Xml.XmlNodeType.Element)
                                {
                                    switch (Element.FirstAttribute.Value)
                                    {
                                        case "Package": //subdirectory has only one attribute
                                            if (Element.Attributes().ToList().Count > 1)
                                            {
                                                Attr = Element.Attributes().ToList().Find(p => p.Name == "Description" && p.Value == "Global libraries");
                                                if (Attr != null)
                                                {
                                                    NewNode = new ASNode(node, Path.Combine(node.Path, Element.Value), "Package.pkg", Element.Value, ASNode.AsType.Library_SubDirectory, 20);
                                                    NewNode.CombineProgramPath(node.ProgramPath, Element.Value);
                                                    Elements.Add(NewNode);
                                                }
                                            }
                                            break;
                                    }
                                }
                            }
                            break;
                        case ASNode.AsType.Library_SubDirectory:
                            //inside system library import only data type definitions --> developer can use inside plc program
                            foreach (XElement Element in xdoc.Elements().Where(p => p.Name.LocalName == "Package").Elements().Where(q => q.Name.LocalName == "Objects").Nodes())
                            {
                                if (Element.NodeType == System.Xml.XmlNodeType.Element)
                                {
                                    switch (Element.FirstAttribute.Value)
                                    {
                                        case "Library":
                                            Attr = Element.Attributes().ToList().Find(p => p.Name == "Language");
                                            if (Attr != null)
                                            {
                                                switch (Attr.Value)
                                                {
                                                    case "Binary":
                                                        Elements.Add(new ASNode(node, Path.Combine(node.Path, Element.Value), string.Format("{0}{1}", Element.Value, ".typ"), Element.Value, ASNode.AsType.VariableDataTypeDefinition, 0));
                                                        break;
                                                }
                                            }
                                            break;
                                    }
                                }
                            }
                            break;
                    }
                }
            }
            catch (Exception ex)
            {
                base.LastError = ex.Message;
                Elements = new List<ASNode>();
            }
            finally
            {
                xdoc = null;
            }

            // resort imported elements --> first if all, import data types
            return Elements.OrderBy(s => s.Priority).ToList();
        }

        private bool UdtIsEnum(string[] udtDefinition)
        {
            if (udtDefinition.Length != 1)
            {
                return false;
            }

            string fieldLine = udtDefinition[0];
            int index = fieldLine.IndexOf(':');
            if (index <= 0)
            {
                return false;
            }

            string fieldName = fieldLine.Substring(0, index);
            fieldName = fieldName.Trim();
            if (fieldName == String.Empty)
            {
                return (false);
            }

            if (fieldLine.Length < (index + 2))
            {
                return (false);
            }

            string fieldType = fieldLine.Substring(index + 1);
            fieldType = fieldType.Trim();
            if (fieldType == String.Empty)
            {
                return (false);
            }
            if (String.Compare(fieldName, "___Movicon_Import_Enum_Alias", true) != 0)
            {
                return (false);
            }

            return true;
        }

        private int GetVarTypeAndSizeFileExp(ref string FieldType, ref UInt32 StandardSize, out uint arrayDimCount, out uint elementsNumber, out UInt32[] arrayStartIndexes, out UInt32[] arrayIndexLimits)
        {
            int returnValue = (int)ImportTypes.Unknown;
            int MovType = (int)DataType.Boolean;
            StandardSize = 0;
            arrayDimCount = 0;
            elementsNumber = 0;
            arrayStartIndexes = null;
            arrayIndexLimits = null;
            FieldType = FieldType.Trim();
            if (FieldType == String.Empty)
            {
                return returnValue;
            }

            // Array?
            int searchIndex1 = FieldType.IndexOf("ARRAY");
            if (searchIndex1 >= 0)
            {
                if (FieldType.Length < (searchIndex1 + 6))
                {
                    return returnValue;
                }

                string szAux = FieldType.Substring(searchIndex1 + 5);
                szAux = szAux.Trim();
                searchIndex1 = szAux.IndexOf('[');
                if (searchIndex1 < 0)
                {
                    return returnValue;
                }

                int searchIndex2 = szAux.IndexOf("..");
                if (searchIndex2 < searchIndex1)
                {
                    return returnValue;
                }
                string szAux2 = szAux.Substring(searchIndex1 + 1,
                                    searchIndex2 - searchIndex1 - 1);
                int nStart0 = 0;
                if (Int32.TryParse(szAux2, out nStart0) == false)
                {
                    if (_mapConstants.ContainsKey(szAux2) == false)
                    {
                        return returnValue;
                    }
                    nStart0 = _mapConstants[szAux2];
                }
                if (nStart0 < 0)
                {
                    return returnValue;
                }

                int nStart1 = 0;
                int nStart2 = 0;
                int nEnd0 = 0;
                int nEnd1 = 0;
                int nEnd2 = 0;
                int searchIndex3 = szAux.IndexOf(',');

                // Unidimensional array?
                if (searchIndex3 < searchIndex2)
                {
                    searchIndex3 = szAux.IndexOf(']');
                    if (searchIndex3 < searchIndex2)
                    {
                        return returnValue;
                    }
                    szAux2 = String.Empty;
                    szAux2 = szAux.Substring(searchIndex2 + 2,
                                        searchIndex3 - searchIndex2 - 2);
                    if (Int32.TryParse(szAux2, out nEnd0) == false)
                    {
                        if (_mapConstants.ContainsKey(szAux2) == false)
                        {
                            return returnValue;
                        }
                        nEnd0 = _mapConstants[szAux2];
                    }
                    if (nEnd0 <= nStart0)
                    {
                        return returnValue;
                    }

                    searchIndex1 = szAux.IndexOf("OF");
                    if (searchIndex1 < searchIndex3)
                    {
                        return returnValue;
                    }
                    if (szAux.Length < (searchIndex1 + 3))
                    {
                        return returnValue;
                    }
                    szAux2 = String.Empty;
                    szAux2 = szAux.Substring(searchIndex1 + 2);
                    szAux2 = szAux2.Trim();

                    //ignore control --> datatype could be defined later
                    //if (GetVarTypeAndSize(ref szAux2, out StandardSize) < 0)
                    //{
                    //    if ((_mapUDT.ContainsKey(szAux2) == false) || ((_mapUDT[szAux2].Length) <= 0))
                    //    {
                    //        if ((_mapSTRUCT.ContainsKey(szAux2) == false) || ((_mapSTRUCT[szAux2].Count) <= 0))
                    //        {
                    //            //if ((_mapFunctionBlock.ContainsKey(szAux2) == false) || ((_mapFunctionBlock[szAux2].Length) <= 0))
                    //            //{
                    //                return returnValue;
                    //            //}
                    //        }
                    //    }
                    //}

                    arrayDimCount = 1;
                    Array.Resize(ref arrayStartIndexes, 1);
                    Array.Resize(ref arrayIndexLimits, 1);
                    arrayStartIndexes[0] = (UInt32)nStart0;
                    //arrayIndexLimits[0] = (UInt32)(nEnd0 - nStart0 + 1);
                    arrayIndexLimits[0] = (UInt32)(nEnd0 + 1);
                    //elementsNumber = arrayIndexLimits[0];
                    elementsNumber = (uint)(nEnd0 - nStart0 + 1);
                    FieldType = String.Empty;
                    FieldType = szAux2;
                    returnValue = (int)ImportTypes.Array;
                }

                // Multidimensional array
                else
                {
                    szAux2 = String.Empty;
                    szAux2 = szAux.Substring(searchIndex2 + 2,
                        searchIndex3 - searchIndex2 - 2);
                    if (Int32.TryParse(szAux2, out nEnd0) == false)
                    {
                        if (_mapConstants.ContainsKey(szAux2) == false)
                        {
                            return returnValue;
                        }
                        nEnd0 = _mapConstants[szAux2];
                    }
                    if (nEnd0 <= nStart0)
                    {
                        return returnValue;
                    }
                    if (szAux.Length < (searchIndex3 + 11))
                    {
                        return returnValue;
                    }

                    szAux2 = String.Empty;
                    szAux2 = szAux.Substring(searchIndex3 + 1);
                    szAux2 = szAux2.Trim();
                    szAux = String.Empty;
                    szAux = szAux2;
                    searchIndex2 = szAux.IndexOf("..");
                    if (searchIndex2 < 1)
                    {
                        return (returnValue);
                    }
                    szAux2 = String.Empty;
                    szAux2 = szAux.Substring(0, searchIndex2);
                    if (Int32.TryParse(szAux2, out nStart1) == false)
                    {
                        if (_mapConstants.ContainsKey(szAux2) == false)
                        {
                            return returnValue;
                        }
                        nStart1 = _mapConstants[szAux2];
                    }
                    if (nStart1 < 0)
                    {
                        return returnValue;
                    }
                    searchIndex3 = szAux.IndexOf(",");

                    // Bi-dimensional array?
                    if (searchIndex3 < searchIndex2)
                    {
                        searchIndex3 = szAux.IndexOf(']');
                        if (searchIndex3 < searchIndex2)
                        {
                            return returnValue;
                        }
                        szAux2 = String.Empty;
                        szAux2 = szAux.Substring(searchIndex2 + 2,
                            searchIndex3 - searchIndex2 - 2);
                        if (Int32.TryParse(szAux2, out nEnd1) == false)
                        {
                            if (_mapConstants.ContainsKey(szAux2) == false)
                            {
                                return returnValue;
                            }
                            nEnd1 = _mapConstants[szAux2];
                        }
                        if (nEnd1 <= nStart1)
                        {
                            return returnValue;
                        }

                        searchIndex1 = szAux.IndexOf("OF");
                        if (searchIndex1 < searchIndex3)
                        {
                            return returnValue;
                        }
                        if (szAux.Length < (searchIndex1 + 3))
                        {
                            return returnValue;
                        }
                        szAux2 = String.Empty;
                        szAux2 = szAux.Substring(searchIndex1 + 2);
                        szAux2 = szAux2.Trim();
                        //ignore control --> datatype could be defined later
                        //if (GetVarTypeAndSize(ref szAux2, out StandardSize) < 0)
                        //{
                        //    if ((_mapUDT.ContainsKey(szAux2) == false) || ((_mapUDT[szAux2].Length) <= 0))
                        //    {
                        //        if ((_mapSTRUCT.ContainsKey(szAux2) == false) || ((_mapSTRUCT[szAux2].Count) <= 0))
                        //        {
                        //            //if ((_mapFunctionBlock.ContainsKey(szAux2) == false) || ((_mapFunctionBlock[szAux2].Length) <= 0))
                        //            //{
                        //                return returnValue;
                        //            //}
                        //        }
                        //    }
                        //}

                        arrayDimCount = 2;
                        Array.Resize(ref arrayStartIndexes, 2);
                        Array.Resize(ref arrayIndexLimits, 2);
                        arrayStartIndexes[0] = (UInt32)nStart0;
                        //arrayIndexLimits[0] = (UInt32)(nEnd0 - nStart0 + 1);
                        arrayIndexLimits[0] = (UInt32)(nEnd0 + 1);
                        arrayStartIndexes[1] = (UInt32)nStart1;
                        //arrayIndexLimits[1] = (UInt32)(nEnd1 - nStart1 + 1);
                        arrayIndexLimits[1] = (UInt32)(nEnd1 + 1);
                        elementsNumber = (uint)((nEnd0 - nStart0 + 1) * (nEnd1 - nStart1 + 1));
                        FieldType = String.Empty;
                        FieldType = szAux2;
                        returnValue = (int)ImportTypes.Array;
                    }

                    // 3-dimensional array
                    else
                    {
                        szAux2 = String.Empty;
                        szAux2 = szAux.Substring(searchIndex2 + 2,
                            searchIndex3 - searchIndex2 - 2);
                        if (Int32.TryParse(szAux2, out nEnd1) == false)
                        {
                            if (_mapConstants.ContainsKey(szAux2) == false)
                            {
                                return returnValue;
                            }
                            nEnd1 = _mapConstants[szAux2];
                        }
                        if (nEnd1 <= nStart1)
                        {
                            return returnValue;
                        }
                        if (szAux.Length < (searchIndex3 + 11))
                        {
                            return returnValue;
                        }
                        szAux2 = String.Empty;
                        szAux2 = szAux.Substring(searchIndex3 + 1);
                        szAux2.Trim();
                        szAux = String.Empty;
                        szAux = szAux2;
                        searchIndex2 = szAux.IndexOf("..");
                        if (searchIndex2 < 1)
                        {
                            return returnValue;
                        }
                        szAux2 = String.Empty;
                        szAux2 = szAux.Substring(0, searchIndex2);
                        if (Int32.TryParse(szAux2, out nStart2) == false)
                        {
                            if (_mapConstants.ContainsKey(szAux2) == false)
                            {
                                return returnValue;
                            }
                            nStart2 = _mapConstants[szAux2];
                        }
                        if (nStart2 < 0)
                        {
                            return returnValue;
                        }
                        searchIndex3 = szAux.IndexOf(']');
                        if (searchIndex3 < searchIndex2)
                        {
                            return returnValue;
                        }
                        szAux2 = String.Empty;
                        szAux2 = szAux.Substring(searchIndex2 + 2,
                                searchIndex3 - searchIndex2 - 2);
                        if (Int32.TryParse(szAux2, out nEnd2) == false)
                        {
                            if (_mapConstants.ContainsKey(szAux2) == false)
                            {
                                return returnValue;
                            }
                            nEnd2 = _mapConstants[szAux2];
                        }
                        if (nEnd2 <= nStart2)
                        {
                            return returnValue;
                        }

                        searchIndex1 = szAux.IndexOf("OF");
                        if (searchIndex1 < searchIndex3)
                        {
                            return returnValue;
                        }
                        if (szAux.Length < (searchIndex1 + 3))
                        {
                            return returnValue;
                        }
                        szAux2 = String.Empty;
                        szAux2 = szAux.Substring(searchIndex1 + 2);
                        szAux2 = szAux2.Trim();
                        //ignore control --> datatype could be defined later
                        //if (GetVarTypeAndSize(ref szAux2, out StandardSize) < 0)
                        //{
                        //    if ((_mapUDT.ContainsKey(szAux2) == false) || ((_mapUDT[szAux2].Length) <= 0))
                        //    {
                        //        if ((_mapSTRUCT.ContainsKey(szAux2) == false) || ((_mapSTRUCT[szAux2].Count) <= 0))
                        //        {
                        //            //if ((_mapFunctionBlock.ContainsKey(szAux2) == false) || ((_mapFunctionBlock[szAux2].Length) <= 0))
                        //            //{
                        //                return returnValue;
                        //            //}
                        //        }
                        //    }
                        //}

                        arrayDimCount = 3;
                        Array.Resize(ref arrayStartIndexes, 3);
                        Array.Resize(ref arrayIndexLimits, 3);
                        arrayStartIndexes[0] = (UInt32)nStart0;
                        //arrayIndexLimits[0] = (UInt32)(nEnd0 - nStart0 + 1);
                        arrayIndexLimits[0] = (UInt32)(nEnd0 + 1);
                        arrayStartIndexes[1] = (UInt32)nStart1;
                        //arrayIndexLimits[1] = (UInt32)(nEnd1 - nStart1 + 1);
                        arrayIndexLimits[1] = (UInt32)(nEnd1 + 1);
                        arrayStartIndexes[2] = (UInt32)nStart2;
                        //arrayIndexLimits[2] = (UInt32)(nEnd2 - nStart2 + 1);
                        arrayIndexLimits[2] = (UInt32)(nEnd2 + 1);
                        elementsNumber = (uint)((nEnd0 - nStart0 + 1) * (nEnd1 - nStart1 + 1) * (nEnd2 - nStart2 + 1));
                        FieldType = String.Empty;
                        FieldType = szAux2;
                        returnValue = (int)ImportTypes.Array;
                    }
                }

                return returnValue;
            }

            if (_mapUDT.ContainsKey(FieldType) == true)
            {
                string[] udtDefinition = _mapUDT[FieldType];
                if (udtDefinition.Length > 0)
                {
                    if (UdtIsEnum(udtDefinition) == false)
                    {
                        returnValue = (int)ImportTypes.StructOrEnum;
                    }
                    else
                    {
                        string stdType = "INT";
                        MovType = GetVarTypeAndSize(ref stdType, out StandardSize);
                        if (MovType >= 0)
                        {
                            returnValue = (int)ImportTypes.Standard;
                        }
                    }
                }

                return returnValue;
            }

            if (_mapSTRUCT.ContainsKey(FieldType) == true)
            {
                if ((_mapSTRUCT[FieldType].Count) > 0)
                {
                    returnValue = (int)ImportTypes.StructOrEnum;
                }
                return returnValue;
            }

            //if (_mapFunctionBlock.ContainsKey(FieldType) == true)
            //{
            //    if ((_mapFunctionBlock[FieldType].Length) <= 0)
            //    {
            //        returnValue = (int)ImportTypes.StructOrEnum;
            //    }
            //    return returnValue;
            //}

            // Standard type?
            MovType = GetVarTypeAndSize(ref FieldType, out StandardSize);
            if (MovType >= 0)
            {
                returnValue = (int)ImportTypes.Standard;
            }

            return returnValue;
        }

        private int GetVarTypeAndSize(ref string Type, out uint VarSize)
        {
            int nType = -1;
            VarSize = 0;
            Type = Type.Trim();
            if (Type.Length == 0)
                return (nType);

            if (String.Compare(Type, "BOOL", true) == 0)
            {
                nType = (int)UFUAModel.DataType.Boolean;
                VarSize = 1;
            }
            else if (String.Compare(Type, "BIT", true) == 0)
            {
                nType = (int)UFUAModel.DataType.Boolean;
                VarSize = 1;
            }
            else if (String.Compare(Type, "BYTE", true) == 0)
            {
                nType = (int)UFUAModel.DataType.Byte;
                VarSize = 1;
            }
            else if (String.Compare(Type, "WORD", true) == 0)
            {
                nType = (int)UFUAModel.DataType.UInt16;
                VarSize = 2;
            }
            else if (String.Compare(Type, "DWORD", true) == 0)
            {
                nType = (int)UFUAModel.DataType.UInt32;
                VarSize = 4;
            }
            else if (String.Compare(Type, "SINT", true) == 0)
            {
                nType = (int)UFUAModel.DataType.SByte;
                VarSize = 1;
            }
            else if (String.Compare(Type, "USINT", true) == 0)
            {
                nType = (int)UFUAModel.DataType.Byte;
                VarSize = 1;
            }
            else if (String.Compare(Type, "INT", true) == 0)
            {
                nType = (int)UFUAModel.DataType.Int16;
                VarSize = 2;
            }
            else if (String.Compare(Type, "UINT", true) == 0)
            {
                nType = (int)UFUAModel.DataType.UInt16;
                VarSize = 2;
            }
            else if (String.Compare(Type, "INT16", true) == 0)
            {
                nType = (int)UFUAModel.DataType.Int16;
                VarSize = 2;
            }
            else if (String.Compare(Type, "UINT16", true) == 0)
            {
                nType = (int)UFUAModel.DataType.UInt16;
                VarSize = 2;
            }
            else if (String.Compare(Type, "DINT", true) == 0)
            {
                nType = (int)UFUAModel.DataType.Int32;
                VarSize = 2;
            }
            else if (String.Compare(Type, "UDINT", true) == 0)
            {
                nType = (int)UFUAModel.DataType.UInt32;
                VarSize = 4;
            }
            else if (String.Compare(Type, "REAL", true) == 0)
            {
                nType = (int)UFUAModel.DataType.Float;
                VarSize = 4;
            }
            else if (String.Compare(Type, "LREAL", true) == 0)
            {
                nType = (int)UFUAModel.DataType.Double;
                VarSize = 8;
            }
            else if (Type.IndexOf("STRING", StringComparison.OrdinalIgnoreCase) == 0 || Type.IndexOf("WSTRING", StringComparison.OrdinalIgnoreCase) == 0)
            {
                nType = (int)UFUAModel.DataType.String;
                VarSize = 81;
                int nIndex = Type.IndexOf('[');
                int nIndex2 = Type.IndexOf(']');
                string szSize = String.Empty;
                if ((nIndex > 0) && (nIndex2 > (nIndex + 1)))
                {
                    szSize = Type.Substring(nIndex + 1, nIndex2 - nIndex - 1);
                    szSize = szSize.Trim();
                    if (szSize != String.Empty)
                    {
                        int stringSize = 0;
                        if (Int32.TryParse(szSize, out stringSize) == true)
                        {
                            if (stringSize > 0)
                            {
                                Type = Type.Substring(0, nIndex).ToUpper();
                                VarSize = (uint)stringSize + 1;
                                if (Type.IndexOf("WSTRING", StringComparison.OrdinalIgnoreCase) == 0)
                                    VarSize *= 2;
                            }
                        }
                    }
                }
                else
                {
                    nIndex = Type.IndexOf('[');
                    nIndex2 = Type.IndexOf(']');
                    if ((nIndex > 0) && (nIndex2 > (nIndex + 1)))
                    {
                        szSize = Type.Substring(nIndex + 1, nIndex2 - nIndex - 1);
                        szSize = szSize.Trim();
                        if (szSize != String.Empty)
                        {
                            int stringSize = 0;
                            if (Int32.TryParse(szSize, out stringSize) == true)
                            {
                                if (stringSize > 0)
                                {
                                    Type = Type.Substring(0, nIndex).ToUpper();
                                    VarSize = (uint)stringSize + 1;
                                    if (Type.IndexOf("WSTRING", StringComparison.OrdinalIgnoreCase) == 0)
                                        VarSize *= 2;
                                }
                            }
                        }
                    }
                }
            }
            //else if (String.Compare(Type, "TIME", true) == 0)
            //{
            //    nType = (int)UFUAModel.DataType.UInt32;
            //    VarSize = 4;
            //}
            //else if (String.Compare(Type, "TIME_OF_DAY", true) == 0)
            //{
            //    nType = (int)UFUAModel.DataType.UInt32;
            //    VarSize = 4;
            //}
            //else if (String.Compare(Type, "TOD", true) == 0)
            //{
            //    nType = (int)UFUAModel.DataType.UInt32;
            //    VarSize = 4;
            //}
            //else if (String.Compare(Type, "DATE", true) == 0)
            //{
            //    nType = (int)UFUAModel.DataType.UInt32;
            //    VarSize = 4;
            //}
            //else if (String.Compare(Type, "DT", true) == 0)
            //{
            //    nType = (int)UFUAModel.DataType.UInt32;
            //    VarSize = 4;
            //}
            //else if (String.Compare(Type, "DATE_AND_TIME", true) == 0)
            //{
            //    nType = (int)UFUAModel.DataType.UInt32;
            //    VarSize = 4;
            //}

            return (nType);
        }

        private bool IsReferenceVariable(string dataType)
        {
            return (dataType.Trim().Contains("REFERENCE TO"));
        }

        
        private void ParseDataTypeKeyword(ref string dataType, out Boolean referenceTo,out Boolean redundUnreplicable)
        {            
            referenceTo = false;
            redundUnreplicable = false;

            dataType = dataType.Trim();

            // remove useless keywords
            if (dataType.Contains("REFERENCE TO")) {
                dataType = dataType.Replace("REFERENCE TO", string.Empty).Trim();
                referenceTo = true;
            }

            // remove useless keywords
            if (dataType.Contains("{REDUND_UNREPLICABLE}"))
            {
                dataType = dataType.Replace("{REDUND_UNREPLICABLE}", string.Empty).Trim();
                redundUnreplicable = true;
            }
        }

        private void RemoveBadCharacters(ref string row, out string comment)
        {
            comment = string.Empty;

            row = row.Trim();

            // find comment inside string (* comment *)
            Regex regex = new Regex(@"\(\*.*?\*\)");
            MatchCollection matches = regex.Matches(row);
            if (matches.Count > 0)
            {
                comment = matches[0].ToString().Substring(2, matches[0].ToString().Length - 2);
                //remove comment from text
                row = row.Replace(matches[0].ToString(), string.Empty);

                row = row.Trim();
            }

            //remove tabulator characters
            row.Replace("\t", string.Empty).Trim();

            // remove variable declaration line terminator's character
            if (row.Length > 0 && row.Substring(row.Length - 1, 1) == ";")
                row = row.Substring(0, row.Length - 1);

            // remove variable default value assignement
            if (row.LastIndexOf(":=")>=0)
                row = row.Substring(0, row.LastIndexOf(":=")).Trim();
        }

        private void ParseDataTypeUndefineTPY(ASNode node, string dataType, string typeName, string[] rows, ref int rowIndex)
        {
            int StartIndex = rowIndex;
            string Comment = string.Empty;

            // start to cicle from 1st member of datatype
            for (rowIndex = StartIndex + 1; rowIndex < rows.Length; rowIndex++)
            {
                string row = rows[rowIndex];

                RemoveBadCharacters(ref row, out Comment);

                if (!string.IsNullOrEmpty(row))
                {
                    //until end of structure cicle members
                    if (row.IndexOf("END_", 0) >= 0 || row.Substring(row.Length - 1, 1) == ")")
                        break;
                }
            }

        }

        private void ParseDataTypeStructTPY(ASNode node, string dataType, string typeName, string[] rows, ref int rowIndex)
        {
            int StartIndex = rowIndex;
            string Comment = string.Empty;
            List<ASImportedVariable> listOfElements = new List<ASImportedVariable>();

            // start to cicle from 1st member of datatype
            for (rowIndex = StartIndex + 1; rowIndex < rows.Length; rowIndex++)
            {
                string row = rows[rowIndex];

                RemoveBadCharacters(ref row, out Comment);

                //until end of structure cicle members
                if (row.IndexOf("END_", 0) < 0)
                {
                    var RowData = row.Split(':');

                    if (RowData.Length >= 2)
                    {
                        ASImportedVariable NewVar = ParseASVariablesDefinition(Parse.MemberOfDataType, node, row, true);
                        if (NewVar != null)
                            listOfElements.Add(NewVar);
                    }
                } else {
                    // end of list of members of structure
                    break;
                }
            }

            if (listOfElements != null && listOfElements.Count > 0)
            {
                if (!_mapSTRUCT.ContainsKey(typeName))
                    _mapSTRUCT.Add(typeName, listOfElements);
            }
        }

        private void ParseDataTypeArrayTPY(ASNode node, string dataType, string typeName)
        {
            string[] arrayOfStrings = { String.Format("___Movicon_Import_Array : {0}", dataType) };

            if (!_mapUDT.ContainsKey(typeName))
                _mapUDT.Add(typeName, arrayOfStrings);
        }

        void ParseDataTypeEnumConstantTPY(ASNode node, string dataType, string typeName, string[] rows, ref int rowIndex)
        {
            int StartIndex = rowIndex;
            string Comment = string.Empty;

            dataType = "Enumeration";

            // start to cicle from 1st member of datatype
            for (rowIndex = StartIndex + 1; rowIndex < rows.Length; rowIndex++)
            {
                string row = rows[rowIndex];

                RemoveBadCharacters(ref row, out Comment);

                //until end of structure cicle members
                if (row.IndexOf(")", 0) >= 0)
                {
                    // end of list of members
                    break;
                }
            }

            string[] arrayOfStrings = { String.Format("___Movicon_Import_Enum : {0}", dataType) };
            if (!_mapUDT.ContainsKey(typeName))
                _mapUDT.Add(typeName, arrayOfStrings);
        }

        private void ParseASVariablesDataType(ASNode node, string[] rows, ref int rowIndex)
        {
            string Comment = string.Empty;
            string row = rows[rowIndex];


            RemoveBadCharacters(ref row, out Comment);

            var RowData = row.Split(':');

            if (RowData.Length >= 2)
            {
                string TypeName = RowData[0].Trim();
                string TypeDeclaration = RowData[1].Trim();

                string VarType;
                string VarTypeArrOfStruct;
                ImportUDTType category = ImportUDTType.Unknown;

                //get data type
                GetUdtTypeTPY(Parse.MemberOfDataType, ref TypeDeclaration, out category, out VarType, out VarTypeArrOfStruct);

                switch (category)
                {
                    case ImportUDTType.IgnoreVar:
                        break;
                    case ImportUDTType.Alias:
                        break;
                    case ImportUDTType.Array:
                        ParseDataTypeArrayTPY(node, TypeDeclaration, TypeName);
                        break;
                    case ImportUDTType.Struct:
                        // continue the parse inside the function
                        ParseDataTypeStructTPY(node, TypeDeclaration, TypeName, rows, ref rowIndex);
                        break;
                    case ImportUDTType.Enum:
                        // continue the parse inside the function
                        ParseDataTypeEnumConstantTPY(node, TypeDeclaration, TypeName, rows, ref rowIndex);
                        break;
                    default: // unmanged data type --> find the end on the element
                        ParseDataTypeUndefineTPY(node, TypeDeclaration, TypeName, rows, ref rowIndex);
                        break;
                }
            }
        }

        /// <summary>
        /// Remove elements of struct without mapped data type; if struct have no elements remove struct 
        /// </summary>
        private void RemoveUnMappedDataTypes()
        {
            foreach (var MemberOfStruct in _mapSTRUCT)
            {
                int MemberNr = 0;
                while (MemberNr < MemberOfStruct.Value.Count)
                {
                    ASImportedVariable Member = MemberOfStruct.Value[MemberNr];
                    if (Member.Category == ImportUDTType.ArrayOfStruct || Member.Category == ImportUDTType.Struct)
                    {
                        if (!_mapSTRUCT.ContainsKey(Member.VarTypeOfStruct))
                        {
                            MemberOfStruct.Value.RemoveAt(MemberNr);
                            MemberNr--;
                        }
                    }
                    MemberNr++;
                }                
            }
            foreach (var tag in _mapSTRUCT.Where(a => a.Value.Count==0).ToList().AsParallel())
                _mapSTRUCT.Remove(tag.Key);
        }

        private void ParseASVariablesDataTypeFile(ASNode node)
        {
            string VariablesFile = Path.Combine(node.Path, node.FileName);
            bool ContentParsing = false;
            string Comment = string.Empty;
            bool ContinueParse = true;

            //parent of library node could not contain any datatype file (.typ) --> no error 
            if (node.IsParentNodeLibrary())
            {
                try
                {
                    ContinueParse = File.Exists(VariablesFile);
                }
                catch (Exception ex)
                {
                    ContinueParse = false;
                }
            }
            else
            {
                ContinueParse = true;
            }

            if (ContinueParse)
            {
                try
                {
                    // Load the file
                    var Rows = File.ReadAllLines(VariablesFile);

                    for (int RowIndex = 0; RowIndex < Rows.Length; RowIndex++)
                    {
                        string row = Rows[RowIndex];

                        switch (row)
                        {
                            case "TYPE":
                                ContentParsing = true;
                                break;

                            case "END_TYPE":
                                ContentParsing = false;
                                break;
                        }

                        if (ContentParsing)
                            ParseASVariablesDataType(node, Rows, ref RowIndex);
                    }
                }
                catch (Exception ex)
                {
                    base.LastError = ex.Message;
                }
            }

            // try to remove all structure elements not mapped
            RemoveUnMappedDataTypes();
        }

        private void ParseSWMapFile(ASNode node)
        {
            List<ASNode> Elements = new List<ASNode>();
            XDocument xdoc;
            string NodeFile = string.Empty;

            NodeFile = Path.Combine(node.Path, node.FileName);

            try
            {
                // Load the file
                xdoc = XDocument.Load(NodeFile);
                if (xdoc != null)
                {
                    foreach (XElement TaskElement in xdoc.Elements().Where(p => p.Name.LocalName == "SwConfiguration").Elements().Where(q => q.Name.LocalName == "TaskClass").Nodes())
                    {
                        string Name = null;
                        string Source = null;
                        foreach (XAttribute Attr in TaskElement.Attributes())
                        {
                            switch (Attr.Name.LocalName)
                            {
                                case "Name":
                                    Name = Attr.Value;
                                    break;
                                case "Source":
                                    Source = Path.GetFileNameWithoutExtension(Attr.Value);
                                    break;
                            }
                        }
                        if (!String.IsNullOrEmpty(Name))
                            _mapPrograms.Add(Source, new ASProgram(Name, Source));
                    }
                }
            }
            catch (Exception ex)
            {
                base.LastError = ex.Message;
                Elements = new List<ASNode>();
            }
            finally
            {
                xdoc = null;
            }
        }

        private bool IsEnumDataType(string varType)
        {
            return (_mapUDT.ContainsKey(varType));
        }

        private void TraverseStructAndGetArrayOfStruct(string sourcePath, string structName, ref List<ASImportedVariable> estractedArrayOfStructIsStruct)
        {
            if (!_mapSTRUCT.ContainsKey(structName))
                return;

            // get members of struct
            List<ASImportedVariable> MembersOfStruct = _mapSTRUCT[structName];
            foreach (ASImportedVariable Member in MembersOfStruct)
            {
                if (Member.Category == ImportUDTType.Struct || Member.Category == ImportUDTType.ArrayOfStruct)
                {
                    string ChildPath = string.Format("{0}.{1}", sourcePath, Member.VariableName);
                    if (Member.Category == ImportUDTType.ArrayOfStruct)
                    {
                        ASImportedVariable ArrayOfStruct = Member.CastedClone();
                        ArrayOfStruct.VariableName = ChildPath;
                        estractedArrayOfStructIsStruct.Add(ArrayOfStruct);
                    }
                    TraverseStructAndGetArrayOfStruct(ChildPath, Member.VariableName, ref estractedArrayOfStructIsStruct);
                }
            }
        }

        private void GetUdtTypeTPY(Parse parsingOf, ref string varDeclaration, out ImportUDTType udtType, out string varType, out string VarTypeOfStruct)
        {
            string FirstVarType = null;
            udtType = ImportUDTType.Unknown;
            varType = string.Empty;
            VarTypeOfStruct = string.Empty;

            Boolean referenceTo;
            Boolean redundUnreplicable;
            ParseDataTypeKeyword(ref varDeclaration, out referenceTo, out redundUnreplicable);

            //discard variable declared as reference (--> pointer)
            if (referenceTo)
            {
                udtType = ImportUDTType.IgnoreVar;
                return;
            }

            //match data type string with only continuos letters,numeber and <_>
            var Matches = Regex.Matches(varDeclaration, MATCH_DATATYPE_VAR_DECLARATION);
            if (Matches.Count == 0)
            {
                FirstVarType = varDeclaration.Trim();
                varType = varDeclaration.Trim();                
            }
            else
            {
                FirstVarType = Matches[0].ToString();
                varType = Matches[Matches.Count - 1].ToString();                
            }

            switch (FirstVarType)
            {
                case "STRUCT":
                    udtType = ImportUDTType.Struct;
                    break;
                case "ARRAY":
                    udtType = ImportUDTType.Array;
                    //array of structur is managed later as Struct
                    if (!IsStandardASDataType(varType))
                    {
                        VarTypeOfStruct = ConvertVarTypeFileFormatToDirectImportTypeFormat(varType);
                        udtType = ImportUDTType.ArrayOfStruct;
                        varType = "STRUCT";
                    } //else  {                        
                        // pointer to array of string in member of struct are not managed properly by pvu
                        //if (referenceTo && (varType == "STRING" || varType == "WSTRING"))
                        //    udtType = ImportUDTType.IgnoreVar;
                    //}
                    break;
                case "":    //enum are converted as Int32
                    udtType = ImportUDTType.Enum;
                    varType = "ENUM";
                    break;
                default:
                    //array of structur is managed later as Struct
                    if (IsStandardASDataType(varType))
                    {
                        udtType = ImportUDTType.Alias;
                        // pointer to array of string in member of struct are not managed properly by pvi
                        //if (referenceto && (vartype == "string" || vartype == "wstring"))
                        //    udttype = importudttype.ignorevar;
                    } else {
                        if (IsEnumDataType(varType)) {
                            udtType = ImportUDTType.Enum;                            
                        } else {
                            VarTypeOfStruct = ConvertVarTypeFileFormatToDirectImportTypeFormat(varType);
                            udtType = ImportUDTType.Struct;
                            varType = "STRUCT";
                        }
                    }
                    break;
            }

            varType = ConvertVarTypeFileFormatToDirectImportTypeFormat(varType);
        }

        private string GetTaskNameFromNode(ASNode node) {
            if (_mapPrograms.ContainsKey(node.ProgramPath))
                return _mapPrograms[node.ProgramPath].Name;
            else
                return node.ProgramPath;
        }

        /// <summary>
        /// Check if data type is not a user datatype
        /// </summary>
        /// <returns></returns>
        private bool IsStandardASDataType(string dataType)
        {
            return (ConvertVarTypeFileFormatToDirectImportTypeFormat(dataType) != dataType);
        }
        
        private string ConvertVarTypeFileFormatToDirectImportTypeFormat(string dataType)
        {
            string ResultFormat = dataType;

            switch (dataType.ToUpper())
            {
                case "SINT": //sbyte
                    ResultFormat = "i8";
                    break;
                case "USINT":
                case "BYTE":
                    ResultFormat = "u8";
                    break;
                case "INT":
                    ResultFormat = "i16";
                    break;
                case "DINT":
                    ResultFormat = "i32";
                    break;
                case "LINT": // to test
                    ResultFormat = "i64";
                  break;
                case "UINT":
                    ResultFormat = "u16";
                    break;
                case "UDINT":
                    ResultFormat = "u32";
                    break;
                case "ULINT":   // to test
                    ResultFormat = "u64";
                    break;
                case "REAL":
                    ResultFormat = "f32";
                    break;
                case "LREAL":
                    ResultFormat = "f64";
                    break;
                case "BOOL":
                    ResultFormat = "boolean";
                    break;
                case "STRING":
                    ResultFormat = "string";
                    break;
                case "WSTRING":
                    ResultFormat = "wstring";
                    break;
                //case "-1":
                //    ResultFormat = "struct";
                //    break;
                //SupportedPlcVariableTypes["time"] = (int)DataType.UInt32;
                //SupportedPlcVariableTypes["dt"] = (int)DataType.UInt32;
                //SupportedPlcVariableTypes["date"] = (int)DataType.UInt32;
                //SupportedPlcVariableTypes["tod"] = (int)DataType.UInt32;
                //case "date": //data --> unsuported data type
                //    ResultFormat= "data";
                //    break;
                case "DWORD":
                    ResultFormat = "u32";
                    break;
            }

            return ResultFormat;
        }

        private void ParseASVariablesDefinitionFile(ASNode node)
        {
            string VariablesFile = Path.Combine(node.Path, node.FileName);
            bool ContentParsing = false;
        
            try
            {
                // Load the file
                var Rows = File.ReadAllLines(VariablesFile);

                for (int RowIndex = 0; RowIndex < Rows.Length; RowIndex++)
                {
                    string row = Rows[RowIndex];
                    switch (row)
                    {
                        case "VAR":
                        //case "VAR CONSTANT":
                        case "VAR RETAIN":
                            ContentParsing = true;
                            break;

                        case "END_VAR":
                            ContentParsing = false;
                            break;
                    }

                    if (ContentParsing)
                    {
                        List<ASImportedVariable> ArrayOfStructInStruct = new List<ASImportedVariable>();
                        ASImportedVariable NewVar = ParseASVariablesDefinition(Parse.Variable, node, row, false, ref ArrayOfStructInStruct);
                        if (NewVar != null) { 
                            _ParsedVars.Add(NewVar);
                            if (ArrayOfStructInStruct.Count > 0)
                                _ParsedVars.AddRange(ArrayOfStructInStruct);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                base.LastError = ex.Message;
            }            
        }

        private ASImportedVariable ParseASVariablesDefinition(Parse parsingFrom, ASNode node, string row, Boolean ignoreStructPresence)
        {
            List<ASImportedVariable> AstractedArrayOfStructIsStruct = null;
            return ParseASVariablesDefinition(parsingFrom, node, row, ignoreStructPresence, ref AstractedArrayOfStructIsStruct);
        }

        private ASImportedVariable ParseASVariablesDefinition(Parse parsingFrom, ASNode node, string row, Boolean ignoreStructPresence, ref List<ASImportedVariable> estractedArrayOfStructIsStruct)
        {            
            string Comment = string.Empty;
            string TaskName = GetTaskNameFromNode(node);
            ASImportedVariable NewElement = null;

            RemoveBadCharacters(ref row, out Comment);
                        
            var RowData = row.Split(':');

            if (RowData.Length >= 2)
            {
                string VariableName = RowData[0].Trim();
                string VarDeclaration = RowData[1].Trim();
                uint VarSize = 0;
                string VarType;
                string VarTypeOfStruct;
                ImportUDTType Category = ImportUDTType.Unknown;

                //get data type
                GetUdtTypeTPY(parsingFrom, ref VarDeclaration, out Category, out VarType, out VarTypeOfStruct);

                NewElement = new ASImportedVariable();
                NewElement.VariableName = VariableName;
                NewElement.TaskName = TaskName;
                NewElement.Description = Comment;
                NewElement.VarType = VarType;
                NewElement.Category = Category;

                switch (Category)
                {
                    case ImportUDTType.IgnoreVar:
                        NewElement = null;
                        break;

                    case ImportUDTType.Alias:
                        
                        if (GetVarTypeAndSize(ref VarDeclaration, out VarSize) >= 0)
                        {
                            NewElement.VarDeclaration = VarDeclaration;
                            NewElement.VarSize = VarSize;
                            NewElement.ElementsNumber = 1;
                        } else {
                            NewElement = null;
                        }
                        break;

                    case ImportUDTType.Enum:    // inside PLC this type is managed as i32
                        NewElement.VarDeclaration = VarDeclaration;
                        NewElement.VarSize = 0;
                        NewElement.ElementsNumber = 1;
                        // for area of struct store datatype
                        break;

                    // Structure or data type not supported (DATE, DATE_AND_TIME --> not supported and so discard it)
                    case ImportUDTType.Struct:
                        NewElement.VarDeclaration = VarDeclaration;
                        NewElement.VarSize = VarSize;                                    
                        NewElement.ElementsNumber = 1;
                        // for area of struct store datatype
                        NewElement.VarTypeOfStruct = VarTypeOfStruct;
                        //don't add variable without mapped structure data type
                        if (!ignoreStructPresence && !_mapSTRUCT.ContainsKey(VarTypeOfStruct))
                            NewElement = null;
                        else
                            if (estractedArrayOfStructIsStruct != null)
                                TraverseStructAndGetArrayOfStruct(VariableName, NewElement.VarTypeOfStruct, ref estractedArrayOfStructIsStruct);
                        break;

                    case ImportUDTType.Array:
                    case ImportUDTType.ArrayOfStruct:
                        uint ArrayDimCount;
                        uint ElementsNumber;
                        uint[] ArrayStartIndexes;
                        uint[] ArrayIndexLimits;

                        // Check the variable type and get the variable size
                        GetVarTypeAndSizeFileExp(ref VarDeclaration, ref VarSize, out ArrayDimCount,out ElementsNumber, out ArrayStartIndexes, out ArrayIndexLimits);

                        NewElement.VarDeclaration = VarDeclaration;                                    
                        NewElement.VarSize = VarSize;
                        NewElement.ElementsNumber = ElementsNumber;
                        NewElement.ArrayDimCount = ArrayDimCount;
                        NewElement.ArrayStartIndexes = ArrayStartIndexes;
                        NewElement.ArrayIndexLimits = ArrayIndexLimits;

                        if (Category == ImportUDTType.ArrayOfStruct)
                        {
                            NewElement.VarTypeOfStruct = VarTypeOfStruct;
                            //don't add variable without mapped structure data type
                            if (!ignoreStructPresence && !_mapSTRUCT.ContainsKey(VarTypeOfStruct))
                                NewElement = null;
                            else
                                if (estractedArrayOfStructIsStruct != null)
                                    TraverseStructAndGetArrayOfStruct(VariableName, NewElement.VarTypeOfStruct, ref estractedArrayOfStructIsStruct);
                        }

                        break;
                }
            }

            return NewElement;
        }

        private void ParseASNode(ASNode node)
        {
            switch (node.Type)
            {
                case ASNode.AsType.Package_SubDirectory:                    
                case ASNode.AsType.Program_SubDirectory:
                case ASNode.AsType.Library_SubDirectory:
                    List<ASNode> Nodes = GetASNodeInfoFile(node);
                    if (Nodes != null && Nodes.Count>0)
                    {
                        foreach (ASNode Node in Nodes)
                            ParseASNode(Node);
                    }
                    return;

                case ASNode.AsType.VariableDefinition:
                    ParseASVariablesDefinitionFile(node);
                    break;

                case ASNode.AsType.VariableDataTypeDefinition:
                    ParseASVariablesDataTypeFile(node);
                    break;
            }
        }

        private void ParseASNodeSWMap(ASNode node)
        {
            switch (node.Type)
            {
                case ASNode.AsType.Package_Physical:
                case ASNode.AsType.Config_Physical:
                    List<ASNode> Nodes = GetASNodeSWMapInfoFile(node);
                    if (Nodes != null && Nodes.Count > 0)
                    {
                        foreach (ASNode Node in Nodes)
                            ParseASNodeSWMap(Node);
                    }
                    return;
                case ASNode.AsType.SWMap:
                    ParseSWMapFile(node);
                    return;
            }
        }

        private void ParseASNodeLibrary(ASNode node)
        {
            switch (node.Type)
            {
                case ASNode.AsType.Package_SubDirectory:
                case ASNode.AsType.Program_SubDirectory:
                case ASNode.AsType.Library_SubDirectory:
                    List<ASNode> Nodes = GetASNodeLibraryInfoFile(node);
                    if (Nodes != null && Nodes.Count > 0)
                    {
                        foreach (ASNode Node in Nodes)
                            ParseASNodeLibrary(Node);
                    }
                    return;

                case ASNode.AsType.VariableDataTypeDefinition:
                    ParseASVariablesDataTypeFile(node);
                    break;
            }
        }

        private void RemoveUnMappedVariableDataTypes()
        {
            int VarNr = 0;
            while (VarNr < _ParsedVars.Count)
            {
                ASImportedVariable Var = _ParsedVars[VarNr];
                if (Var.Category == ImportUDTType.ArrayOfStruct || Var.Category == ImportUDTType.Struct)
                {
                    if (!_mapSTRUCT.ContainsKey(Var.VarTypeOfStruct))
                    {
                        _ParsedVars.RemoveAt(VarNr);
                        VarNr--;
                    }
                }
                VarNr++;
            }
        }

        private void ConvertConstantDataTypeToInt32()
        {
            string EnumDataType = "i32";
            uint EnumVarSize = 0;

            GetVarTypeAndSize(ref EnumDataType, out EnumVarSize);

            foreach (var DataType in _mapSTRUCT.Values)
            {
                foreach (ASImportedVariable Member in DataType.FindAll(m => m.Category == ImportUDTType.Enum)) {                     
                    Member.VarType = EnumDataType;
                    Member.VarSize = EnumVarSize;
                }
            }

            foreach (ASImportedVariable Var in _ParsedVars.FindAll(m => m.Category == ImportUDTType.Enum))
            {
                Var.VarType = EnumDataType;
                Var.VarSize = EnumVarSize;
            }
        }

        protected void RemoveArrayOfStructInSubStruct()
        {
            foreach (var MembersOfStruct in _mapSTRUCT.Values)
                MembersOfStruct.RemoveAll(a => a.Category == ImportUDTType.ArrayOfStruct);

            // try to remove struct with no members
            foreach (var tag in _mapSTRUCT.Where(a => a.Value.Count == 0).ToList().AsParallel())
            {
                _mapSTRUCT.Remove(tag.Key);
                // remove alla vars with this data type
                _ParsedVars.RemoveAll(a => a.Category == ImportUDTType.ArrayOfStruct && a.VarTypeOfStruct == tag.Key);
            }
        }

        public override ImportDataModel Import(GetStationName readStationName, string station)
        {            
            _ParsedVars = new List<ASImportedVariable>();
            _mapSTRUCT = new Dictionary<string, List<ASImportedVariable>>();
            //_mapFunctionBlock = new Dictionary<string, string[]>();
            _mapUDT = new Dictionary<string, string[]>();
            _mapConstants = new Dictionary<string, int>();
            _mapPrograms = new Dictionary<string, ASProgram>();
                        
            // retrive the list of programm (vars definition, data type definition, ecc) really used into projects
            ASNode SwMapNode = new ASNode(null, Path.Combine(_ProjectPath, "Physical"), "Physical.pkg", "RootNode", ASNode.AsType.Package_Physical, 0);
            ParseASNodeSWMap(SwMapNode);

            // retrive first all datatype into plc's system librarys
            ASNode LibraryNode = new ASNode(null, Path.Combine(_ProjectPath, "Logical"), "Package.pkg", "RootNode", ASNode.AsType.Package_SubDirectory, 0); 
            ParseASNodeLibrary(LibraryNode);

            //retrive the list of vars/programs to import
            ASNode RootNode = new ASNode(null,Path.Combine(_ProjectPath, "Logical"), "Package.pkg", "RootNode", ASNode.AsType.Package_SubDirectory,0);
            ParseASNode(RootNode);

            ConvertConstantDataTypeToInt32();

            // remove array of struct from struct's members --> not supported from Movicon prototype (remove empty struct)
            RemoveArrayOfStructInSubStruct();

            importDataModel = new ImportDataModelBrPVI(readStationName);

            ParsePlcDataFromASProject();

            return importDataModel;
        }

        /// <summary>
        /// Test if selected file is an Automation Studio Project
        /// </summary>
        /// <returns></returns>
        public bool IsASProject()
        {
            bool IsProject = false;
            XDocument xdoc = null;

            try
            {
                // Load the file
                xdoc = XDocument.Load(ProjectFile);
                if (xdoc != null)
                {
                    if (xdoc.FirstNode != null)
                    {
                        if (xdoc.FirstNode.NodeType == System.Xml.XmlNodeType.ProcessingInstruction)
                        {
                            if ((xdoc.FirstNode as XProcessingInstruction).Target == "AutomationStudio")
                            {
                                IsProject = true;
                                // get Automation Studio version inside quotes
                                var reg = new Regex("\".*?\"");
                                var matches = reg.Matches((xdoc.FirstNode as XProcessingInstruction).Data);
                                if (matches.Count > 0)
                                    _ProjectVersion = matches[0].ToString();
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

            return (IsProject);
        }        
        #endregion

        #region Import from Automation Studio Project
        void ParsePlcDataFromASProject()
        {
            //mapSTRUCT = importFromFile.mapSTRUCT;
            //mapFunctionBlock = importFromFile.mapFunctionBlock;
            //mapUDT = importFromFile.mapUDT;
            //mapConstants = importFromFile.mapConstants;

            FillSupportedPlcTypeDictionary();

            foreach (var pviObject in _ParsedVars)
                AddPlcVariableFromASProject(pviObject);
        }

        void AddPlcVariableFromASProject(BrPviASImportParser.ASImportedVariable pviObject)
        {
            int moviconType;

            DriverCodeBase.Enumerators.LinkType jobType = DriverCodeBase.Enumerators.LinkType.InputOutput;

            // Set the first part of the variable name
            string namePrefix = String.Empty;
            if (!String.IsNullOrWhiteSpace(pviObject.TaskName))
            {
                namePrefix = pviObject.TaskName + "_";
            }

            // Structure?
            if (IsPlcStructureType(pviObject.VarType))
            {
                // Add the structure variable and its elements to the tree
                AddPviStructureVariableFromAs(namePrefix,
                                        pviObject.TaskName,
                                        pviObject.VariableName,
                                        pviObject.VarTypeOfStruct,
                                        pviObject.VarSize,
                                        pviObject, //string.Empty,
                                        jobType,
                                        pviObject.ElementsNumber,
                                        -1,
                                        null);
                return;
            }

            // Array
            if (pviObject.ElementsNumber > 1)
            {
                // Add the array variable and its elements to the tree
                AddPviArrayVariableFromAS(namePrefix,
                                    String.Empty,
                                    pviObject.TaskName,
                                    pviObject.VariableName,
                                    pviObject.VarType,
                                    pviObject,
                                    jobType,
                                    pviObject.ElementsNumber,
                                    pviObject.VarSize,
                                    -1,
                                    null);
                return;
            }

            // Standard variable
            // Get the corresponding MOVICON type
            moviconType = GetMoviconType(pviObject.VarType);
            // Invalid Type?
            if (moviconType == -1)
            {
                return;
            }

            // Add the variable to the tree
            AddPviStandardVar(namePrefix,
                              pviObject.VariableName,
                              pviObject.VariableName,
                              pviObject.TaskName,
                              String.Empty,
                              pviObject.VarType,
                              moviconType,
                              pviObject.VarSize,
                              jobType,
                              -1,
                              null);
        }

        void AddPviStructureVariableFromAs(string namePrefix,
                                     string taskName,
                                     string variableName,
                                     string variableType,
                                     uint nVarSize,
                                     BrPviASImportParser.ASImportedVariable typeDefinition, // string typeDefinition,
                                     DriverCodeBase.Enumerators.LinkType jobType,
                                     uint elementsNumber,
                                     int parentId = -1,
                                     ImportData inRootItem = null)
        {

            //// Parse the definitions of the structure elements
            //int searchIndex = typeDefinition.IndexOf('{');
            //if (searchIndex < 0)
            //{
            //    return;
            //}

            //string auxString = typeDefinition.Substring(searchIndex);
            //char[] splitParameters = new char[1];
            //splitParameters[0] = '{';
            //string[] structureElements = typeDefinition.Split(splitParameters);
            //if (structureElements.Count() < 1)
            //{
            //    return;
            //}

            // If needed, add the task name to the type name
            string completeTypeName = String.Empty;
            if (!String.IsNullOrWhiteSpace(taskName))
            {
                completeTypeName = taskName + "_" + variableType;
            }
            else
            {
                completeTypeName = variableType;
            }

            // Simple structure?
            if (elementsNumber == 1)
            {
                List<BrPviASImportParser.ASImportedVariable> structureElements = GetStructElements(typeDefinition.VarTypeOfStruct);
                if (structureElements == null || structureElements.Count == 0)
                    return;

                // Build the structure variable to be added to the tree
                ImportData v = importDataModel.addImportData();
                ((ImportDataBrPvi)v).PreName = namePrefix;
                ((ImportDataBrPvi)v).Name = variableName;
                ((ImportDataBrPvi)v).Size = nVarSize;
                v.szType = completeTypeName;
                ((ImportDataBrPvi)v).szTypeView = GetMoviconTypeFromPlcType(v.szType);
                ((ImportDataBrPvi)v).Task = taskName;
                ((ImportDataBrPvi)v).ElemType = completeTypeName;
                v.Address = variableName;
                v.Description = String.Empty;
                v.parentId = parentId;
                v.Id = m_lGlobalID++;
                v.ArrayDimension = 0;
                ((ImportDataBrPvi)v).ImportDataType = ImportTypes.StructOrEnum;
                ((ImportDataBrPvi)v).JobType = jobType;

                // Add the structure elements to the tree
                string addressPrefix = variableName;
                AddPviStructureElementsFromAS(namePrefix,
                                        taskName,
                                        addressPrefix,
                                        structureElements,
                                        jobType,
                                        v.Id,
                                        inRootItem,
                                        v);
            }
            // Array of Structures
            else
            {
                // Add the array elements to the tree
                AddPviStructureArrayFromAS(namePrefix,
                                     variableName,
                                     taskName,
                                     completeTypeName,
                                     new List<BrPviASImportParser.ASImportedVariable>() { typeDefinition },
                                     jobType,
                                     elementsNumber,
                                     parentId,
                                     inRootItem);
            }
        }

        void AddPviStructureElementsFromAS(string namePrefix,
                                     string taskName,
                                     string addressPrefix,
                                     List<BrPviASImportParser.ASImportedVariable> structureElements, //string[] structureElements,
                                     DriverCodeBase.Enumerators.LinkType jobType,
                                     int parentId,
                                     ImportData inParentRootItem,
                                     ImportData inRootItem)
        {
            bool parentStructHasBeenAddedToTheTree = false;

            for (int i = 0; i < structureElements.Count(); i++)
            {
                BrPviASImportParser.ASImportedVariable structElement = structureElements[i];

                string auxString = String.Format(".{0}", structElement.VariableName);
                string elementName = structElement.VariableName;
                string elementAddress = addressPrefix + auxString;
                string varPreName = namePrefix + addressPrefix + ".";

                string varType = structElement.VarType;

                uint variableLength = structElement.VarSize;

                uint elementsNumber = structElement.ElementsNumber;

                // Structure?
                if (IsPlcStructureType(varType))
                {
                    varType = structElement.VarTypeOfStruct;

                    string completeTypeName = String.Empty;
                    if (!String.IsNullOrWhiteSpace(taskName))
                    {
                        completeTypeName = taskName + "_" + varType;
                    }
                    else
                    {
                        completeTypeName = varType;
                    }

                    // Array of structures?
                    if (elementsNumber > 1)
                    {
                        List<BrPviASImportParser.ASImportedVariable> substructureDefinition = new List<BrPviASImportParser.ASImportedVariable>();
                        substructureDefinition.Add(structElement);
                        substructureDefinition.AddRange(GetStructElements(structElement.VarTypeOfStruct));
                        // no elements
                        if (substructureDefinition.Count == 1)
                            return;
                        AddPviStructureArrayFromAS(varPreName,
                                             elementName,
                                             taskName,
                                             completeTypeName,
                                             substructureDefinition,
                                             jobType,
                                             elementsNumber,
                                             -1,
                                             null);
                    }
                    else  // singol structure element
                    {

                        List<BrPviASImportParser.ASImportedVariable> substructureElements = GetStructElements(structElement.VarTypeOfStruct);
                        if (substructureElements == null || substructureElements.Count == 0)
                            return;

                        // Add the parent structure variable to the tree
                        if ((inRootItem != null) && !parentStructHasBeenAddedToTheTree)
                        {
                            AddTreeItem(inRootItem, inParentRootItem);
                            parentStructHasBeenAddedToTheTree = true;
                        }

                        // Build the substructure variable to be added to the tree
                        ImportData v = importDataModel.addImportData();
                        ((ImportDataBrPvi)v).PreName = varPreName;
                        v.Name = elementName;
                        ((ImportDataBrPvi)v).Size = variableLength;
                        v.szType = completeTypeName;
                        ((ImportDataBrPvi)v).szTypeView = GetMoviconTypeFromPlcType(v.szType);
                        ((ImportDataBrPvi)v).Task = taskName;
                        ((ImportDataBrPvi)v).ElemType = completeTypeName;
                        v.Address = elementAddress;
                        v.Description = String.Empty;
                        v.parentId = parentId;
                        v.Id = m_lGlobalID++;
                        v.ArrayDimension = 0;
                        ((ImportDataBrPvi)v).ImportDataType = ImportTypes.StructOrEnum;
                        ((ImportDataBrPvi)v).JobType = jobType;

                        // Add the substructure elements to the tree
                        AddPviStructureElementsFromAS(namePrefix,
                                                taskName,
                                                elementAddress,
                                                substructureElements,
                                                jobType,
                                                v.Id,
                                                inRootItem,
                                                v);
                    }
                }
                else
                {

                    // Get the corresponding MOVICON type
                    int moviconType = GetMoviconType(varType);
                    // Invalid Type?
                    if (moviconType == -1)
                    {
                        i++;
                        continue;
                    }

                    // Add the parent structure variable to the tree
                    if ((inRootItem != null) && !parentStructHasBeenAddedToTheTree)
                    {
                        AddTreeItem(inRootItem, inParentRootItem);
                        parentStructHasBeenAddedToTheTree = true;
                    }

                    // Array of non complex elements ?
                    if (elementsNumber > 1)
                    {
                        string addrPrefix = addressPrefix + ".";
                        // Add the array variable and its elements to the tree
                        AddPviArrayVariableFromAS(varPreName,
                                            addrPrefix,
                                            taskName,
                                            elementName,
                                            varType,
                                            structElement,
                                            jobType,
                                            elementsNumber,
                                            variableLength,
                                            parentId,
                                            inRootItem);
                    }
                    else // Standard variable
                    {
                        // Add the standard variable to the tree
                        AddPviStandardVar(varPreName,
                                          elementName,
                                          elementAddress,
                                          taskName,
                                          String.Empty, // No description
                                          varType,
                                          moviconType,
                                          variableLength,
                                          jobType,
                                          parentId,
                                          inRootItem);
                    }
                }
            }
        }


        void AddPviArrayVariableFromAS(string namePrefix,
                                 string addressPrefix,
                                 string taskName,
                                 string variableName,
                                 string elementType,
                                 BrPviASImportParser.ASImportedVariable typeDefinition, //string typeDefinition,
                                 DriverCodeBase.Enumerators.LinkType jobType,
                                 uint elementsNumber,
                                 uint elementSize,
                                 int parentId = -1,
                                 ImportData inRootItem = null)
        {
            // Get the MOVICON type corresponding to the element type
            int moviconType = GetMoviconType(elementType);
            // Invalid Type?
            if (moviconType == -1)
            {
                return;
            }

            // Add the variable to the tree
            AddPviArray(namePrefix,
                        addressPrefix,
                        taskName,
                        variableName,
                        elementType,
                        moviconType,
                        elementSize,
                        jobType,
                        elementsNumber,
                        typeDefinition.ArrayDimCount,
                        typeDefinition.ArrayStartIndexes,
                        typeDefinition.ArrayIndexLimits,
                        parentId,
                        inRootItem);
        }

        void AddPviStructureArrayFromAS(string namePrefix,
                                  string variableName,
                                  string taskName,
                                  string completeTypeName,
                                  List<BrPviASImportParser.ASImportedVariable> structureElements, //string[] structureElements,
                                  DriverCodeBase.Enumerators.LinkType jobType,
                                  uint elementsNumber,
                                  int parentId,
                                  ImportData inRootItem)
        {

            string varNamePrefix = String.Format("{0}[", variableName);
            string addrPrefix = String.Format("{0}[", namePrefix + variableName);

            AddPviStructureArrayElementsFromAS(namePrefix,
                                         varNamePrefix,
                                         addrPrefix,
                                         taskName,
                                         completeTypeName,
                                         structureElements,
                                         jobType,
                                         elementsNumber,
                                         structureElements[0].ArrayDimCount,
                                         0,
                                         structureElements[0].ArrayStartIndexes,
                                         structureElements[0].ArrayIndexLimits,
                                         parentId,
                                         inRootItem);//,
                                                     //pviObject);
        }

        void AddPviStructureArrayElementsFromAS(string namePrefix,
                                  string varNamePrefix,
                                  string addrPrefix,
                                  string taskName,
                                  string completeTypeName,
                                  List<BrPviASImportParser.ASImportedVariable> structureElements, //string[] structureElements,
                                  DriverCodeBase.Enumerators.LinkType jobType,
                                  uint elementsNumber,
                                  uint arrayDimCount,
                                  uint currentDim,
                                  uint[] arrayStartIndexes,
                                  uint[] arrayIndexLimits,
                                  int parentId,
                                  ImportData inRootItem) //,
                                                         //BrPviASImportParser.ASImportedVariable pviObject)
        {
            for (uint i = arrayStartIndexes[currentDim]; i < arrayIndexLimits[currentDim]; i++)
            {
                uint RealArrayIndex = i - arrayStartIndexes[currentDim];
                //in multidimension array, from 2nd dimension, use start index (standard array, always start from 0)
                if (currentDim > 0)
                    RealArrayIndex = i;
                string szAddress = String.Format("{0}{1}", addrPrefix, RealArrayIndex);
                string varName = String.Format("{0}{1}", varNamePrefix, RealArrayIndex);                
                if (currentDim == (arrayDimCount - 1))
                {
                    varName += "]";
                    szAddress += "]";
                    AddPviStructureArraySingleElementFromAS(namePrefix,
                                                      varName,
                                                      szAddress,
                                                      taskName,
                                                      completeTypeName,
                                                      structureElements,
                                                      jobType,
                                                      parentId,
                                                      inRootItem);
                }
                else
                {
                    varName += ",";
                    szAddress += ",";
                    AddPviStructureArrayElementsFromAS(namePrefix,
                                                 varName,
                                                 szAddress,
                                                 taskName,
                                                 completeTypeName,
                                                 structureElements,
                                                 jobType,
                                                 elementsNumber,
                                                 arrayDimCount,
                                                 currentDim + 1,
                                                 arrayStartIndexes,
                                                 arrayIndexLimits,
                                                 parentId,
                                                 inRootItem);
                }
            }
        }

        void AddPviStructureArraySingleElementFromAS(string namePrefix,
                                               string varName,
                                               string szAddress,
                                               string taskName,
                                               string completeTypeName,
                                               List<BrPviASImportParser.ASImportedVariable> structureElements, //string[] structureElements,
                                               DriverCodeBase.Enumerators.LinkType jobType,
                                               int parentId,
                                               ImportData inRootItem)
        {

            uint variableLength = structureElements[0].VarSize;

            // Build the structure variable to be added to the tree
            ImportData v = importDataModel.addImportData();
            ((ImportDataBrPvi)v).PreName = namePrefix;
            v.Name = varName;
            ((ImportDataBrPvi)v).Size = variableLength;
            v.szType = completeTypeName;
            ((ImportDataBrPvi)v).szTypeView = GetMoviconTypeFromPlcType(v.szType);
            ((ImportDataBrPvi)v).Task = taskName;
            ((ImportDataBrPvi)v).ElemType = completeTypeName;
            v.Address = szAddress;
            v.Description = String.Empty;
            v.parentId = parentId;
            v.Id = m_lGlobalID++;
            v.ArrayDimension = 0;
            ((ImportDataBrPvi)v).ImportDataType = ImportTypes.StructOrEnum;
            ((ImportDataBrPvi)v).JobType = jobType;

            // Add the structure elements to the tree
            if (structureElements.Count() < 1) // davide 2)
            {
                return;
            }

            List<BrPviASImportParser.ASImportedVariable> elementDefinitions = GetStructElements(structureElements[0].VarTypeOfStruct);
            if (elementDefinitions == null || elementDefinitions.Count == 0)
                return;

            string addressPrefix = szAddress;
            AddPviStructureElementsFromAS(namePrefix,
                                    taskName,
                                    addressPrefix,
                                    elementDefinitions,
                                    jobType,
                                    v.Id,
                                    inRootItem,
                                    v);
        }

        //private void AddPviStandardVar(string szNamePrefix,
        //                               string szVarName,
        //                               string szAddress,
        //                               string taskName,
        //                               string description,
        //                               string szVarType,
        //                               int moviconType,
        //                               uint nVarSize,
        //                               DriverCodeBase.Enumerators.LinkType jobType,
        //                               int parentId = -1,
        //                               ImportData inRootItem = null)
        //{
        //    ImportData v = importDataModel.addImportData();
        //    v.PreName = szNamePrefix;
        //    v.Name = szVarName;
        //    v.Size = nVarSize;
        //    v.szType = szVarType;
        //    v.szTypeView = GetMoviconTypeFromPlcType(v.szType);
        //    v.Task = taskName;
        //    v.ElemType = szVarType;
        //    v.Type = (DataType)moviconType;
        //    v.Address = szAddress;
        //    v.Description = description;
        //    v.parentId = parentId;
        //    v.Id = m_lGlobalID++;
        //    v.ArrayDimension = 0;
        //    v.ImportDataType = ImportTypes.Standard;
        //    v.JobType = jobType;
        //    AddTreeItem(v, inRootItem);
        //}
        #endregion

        #region Utility
        public List<ASImportedVariable> GetStructElements(string structName)
        {
            if (_mapSTRUCT.ContainsKey(structName))
                return _mapSTRUCT[structName];
            else
                return new List<ASImportedVariable>();
        }
        #endregion

        public virtual void Dispose()
        {
            Clean();
        }

        void Clean()
        {
            
        }
    }
}
