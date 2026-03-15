/* ========================================================================
 * Copyright (c) 2005-2009 The OPC Foundation, Inc. All rights reserved.
 *
 * OPC Foundation MIT License 1.00
 * 
 * Permission is hereby granted, free of charge, to any person
 * obtaining a copy of this software and associated documentation
 * files (the "Software"), to deal in the Software without
 * restriction, including without limitation the rights to use,
 * copy, modify, merge, publish, distribute, sublicense, and/or sell
 * copies of the Software, and to permit persons to whom the
 * Software is furnished to do so, subject to the following
 * conditions:
 * 
 * The above copyright notice and this permission notice shall be
 * included in all copies or substantial portions of the Software.
 * THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND,
 * EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES
 * OF MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND
 * NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT
 * HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY,
 * WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING
 * FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR
 * OTHER DEALINGS IN THE SOFTWARE.
 *
 * The complete license agreement can be found here:
 * http://opcfoundation.org/License/MIT/1.00/
 * ======================================================================*/

using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using System.Xml;
using System.Xml.Serialization;
using System.IO;
using System.Reflection;

using Opc.Ua.CodeGenerator;

namespace Opc.Ua.ModelCompiler
{
    /// <summary>
    /// Generates code based on a UA Type Dictionary.
    /// </summary>
    public partial class ModelGenerator : Opc.Ua.CodeGenerator.CodeGenerator
    {       
        #region Constructors
		/// <summary>
		/// Loads the model design from the specified file and validates it.
		/// </summary>
		public ModelGenerator()
        {
		}
        #endregion      
        
        #region Public Properties
        const string TemplatePath = "Opc.Ua.ModelCompiler.Templates.";
        const string DefaultNamespace = "http://opcfoundation.org/UA/";
        #endregion
        
        /// <summary>
        /// Generates the source code files.
        /// </summary>        
        public virtual void ValidateAndUpdateIds(string filePath, string identifierFilePath, uint startId)
        {
            m_validator = new ModelCompilerValidator(startId);
            m_validator.Validate(filePath, identifierFilePath, false);
            m_model = m_validator.Dictionary;
        }

        /// <summary>
        /// Generates the source code files.
        /// </summary>        
        public virtual void GenerateIdentifiers(string filePath, string prefix)
        {         
            WriteTemplate_Identifiers(filePath, prefix, true);
            WriteTemplate_Names(filePath, prefix, true);
        }
        
        /// <summary>
        /// Generates the source code files.
        /// </summary>        
        public virtual void GenerateIdentifiersAnsiC(string filePath, string prefix)
        {         
            WriteTemplate_IdentifiersAnsiC(filePath, prefix, true);
            WriteTemplate_NamesAnsiC(filePath, prefix, true);
        }
        
        /// <summary>
        /// Generates the source code files.
        /// </summary>        
        public virtual void GenerateSchema(string filePath, string prefix)
        {         
            List<NodeDesign> nodes = new List<NodeDesign>();

            foreach (NodeDesign node in m_validator.Nodes)
            {
                if (node is DataTypeDesign)
                {
                    nodes.Add(node);
                }
            }

            WriteTemplate_XmlSchema(filePath, prefix, true);
            WriteTemplate_BinarySchema(filePath, prefix, true);
        }
        
        #region Types Class Generation
        /// <summary>
        /// Adds the supertypes to the list.
        /// </summary>
        private void AddSuperTypes(TypeDesign type, List<NodeDesign> sortedTypes)
        {
            if (type.BaseType != null)
            {
                TypeDesign baseType = m_validator.FindType(type.BaseType) as TypeDesign;
                
                if (baseType != null)
                {
                    AddSuperTypes(baseType, sortedTypes);
                }
            }

            if (!type.IsDeclaration)
            {
                if (!sortedTypes.Contains(type))
                {
                    sortedTypes.Add(type);
                }
            }
        }

        /// <summary>
        /// Returns a list of types sorted by their dependencies.
        /// </summary>
        private List<NodeDesign> SortTypeByDependencies(List<NodeDesign> nodes)
        {
            List<NodeDesign> sortedTypes = new List<NodeDesign>();

            // add reference types.
            foreach (NodeDesign node in nodes)
            {
                if (!node.IsDeclaration && node is ReferenceTypeDesign)
                {
                    AddSuperTypes((ReferenceTypeDesign)node, sortedTypes);
                }
            }
            
            // add datatypes
            foreach (NodeDesign node in nodes)
            {
                DataTypeDesign datatype = node as DataTypeDesign;

                if (!node.IsDeclaration && datatype != null)
                {
                    AddSuperTypes(datatype, sortedTypes);

                    // add datatype encodings.
                    if (datatype.HasEncodings)
                    {
                        foreach (ObjectDesign encoding in datatype.Encodings)
                        {
                            if (!sortedTypes.Contains(encoding))
                            {
                                sortedTypes.Add(encoding);
                            }
                        }
                    }
                }
            }
                        
            // add varible types
            foreach (NodeDesign node in nodes)
            {
                if (!node.IsDeclaration && node is VariableTypeDesign)
                {
                    AddSuperTypes((VariableTypeDesign)node, sortedTypes);
                }
            }
            
            // add object types
            foreach (NodeDesign node in nodes)
            {
                if (!node.IsDeclaration && node is ObjectTypeDesign)
                {
                    AddSuperTypes((ObjectTypeDesign)node, sortedTypes);
                }
            }

            return sortedTypes;
        }
        
        /// <summary>
        /// Creates a class that defines all types in the namespace.
        /// </summary>
        private void WriteTemplate_Identifiers(string filePath, string prefix, bool exportAll)
        {          
			StreamWriter writer = new StreamWriter(String.Format(@"{0}\{1}Identifiers.cs", filePath, prefix), false);

            try
            {
                string templateName = "Types.Identifiers.cs";
                
                Template template = new Template(writer, TemplatePath + templateName, Assembly.GetExecutingAssembly());
                
                template.AddReplacement("_Namespace_", GetNamespaceCodePath(m_model.TargetNamespace, false));
                template.AddReplacement("_NamespaceUri_", GetConstantForNamespace(m_model.TargetNamespace));
                template.AddReplacement("_NamespaceName_", GetNamespaceCodePath(m_model.TargetNamespace, false));
                
                AddTemplate(
                    template,
                    "// ListOfImports",
                    null,
                    m_model.Namespaces,
                    new LoadTemplateEventHandler(LoadTemplate_NamespaceImports),
                    null);
                
                AddTemplate(
                    template,
                    "// ListOfReferenceTypes",
                    TemplatePath + "Types.TypeId.cs",
                    GetNodes("ReferenceType", exportAll),
                    null,
                    new WriteTemplateEventHandler(WriteTemplate_CodeTypeId));
                
                AddTemplate(
                    template,
                    "// ListOfDataTypes",
                    TemplatePath + "Types.TypeId.cs",
                    GetNodes("DataType", exportAll),
                    null,
                    new WriteTemplateEventHandler(WriteTemplate_CodeTypeId));
                
                AddTemplate(
                    template,
                    "// ListOfObjectTypes",
                    TemplatePath + "Types.TypeId.cs",
                    GetNodes("ObjectType", exportAll),
                    null,
                    new WriteTemplateEventHandler(WriteTemplate_CodeTypeId));
                
                AddTemplate(
                    template,
                    "// ListOfEventTypes",
                    TemplatePath + "Types.TypeId.cs",
                    GetNodes("EventType", exportAll),
                    null,
                    new WriteTemplateEventHandler(WriteTemplate_CodeTypeId));
                
                AddTemplate(
                    template,
                    "// ListOfVariableTypes",
                    TemplatePath + "Types.TypeId.cs",
                    GetNodes("VariableType", exportAll),
                    null,
                    new WriteTemplateEventHandler(WriteTemplate_CodeTypeId));
                
                AddTemplate(
                    template,
                    "// ListOfObjects",
                    TemplatePath + "Types.TypeId.cs",
                    GetNodes("Object", exportAll),
                    null,
                    new WriteTemplateEventHandler(WriteTemplate_CodeTypeId));
                
                AddTemplate(
                    template,
                    "// ListOfVariables",
                    TemplatePath + "Types.TypeId.cs",
                    GetNodes("Variable", exportAll),
                    null,
                    new WriteTemplateEventHandler(WriteTemplate_CodeTypeId));

                template.WriteTemplate(null);       
            }
            finally
            {
                writer.Close();
            }
        }
                
        /// <summary>
        /// Creates a class that defines all types in the namespace.
        /// </summary>
        private void WriteTemplate_Names(string filePath, string prefix, bool exportAll)
        {          
			StreamWriter writer = new StreamWriter(String.Format(@"{0}\{1}Names.cs", filePath, prefix), false);

            try
            {
                string templateName = "Types.Names.cs";
                
                Template template = new Template(writer, TemplatePath + templateName, Assembly.GetExecutingAssembly());
                
                template.AddReplacement("_Namespace_", GetNamespaceCodePath(m_model.TargetNamespace, false));
                template.AddReplacement("_NamespaceUri_", GetConstantForNamespace(m_model.TargetNamespace));
                template.AddReplacement("_NamespaceName_", GetNamespaceCodePath(m_model.TargetNamespace, false));
                
                AddTemplate(
                    template,
                    "// ListOfImports",
                    null,
                    m_model.Namespaces,
                    new LoadTemplateEventHandler(LoadTemplate_NamespaceImports),
                    null);
                
                AddTemplate(
                    template,
                    "// ListOfNames",
                    TemplatePath + "Types.Name.cs",
                    GetNodes("", exportAll),
                    new LoadTemplateEventHandler(LoadTemplate_CodeTypeId),
                    new WriteTemplateEventHandler(WriteTemplate_CodeTypeId));
                
                template.WriteTemplate(null);       
            }
            finally
            {
                writer.Close();
            }
        }

        /// <summary>
        /// Creates a class that defines all types in the namespace.
        /// </summary>
        private void WriteTemplate_IdentifiersAnsiC(string filePath, string prefix, bool exportAll)
        {          
			StreamWriter writer = new StreamWriter(String.Format(@"{0}\{1}_identifiers.h", filePath, prefix.ToLower()), false);

            try
            {
                string templateName = "Types.Identifiers.h";
                
                Template template = new Template(writer, TemplatePath + templateName, Assembly.GetExecutingAssembly());
                
                template.AddReplacement("_Date_", DateTime.Now.ToShortDateString());
                template.AddReplacement("_FileName_", String.Format("{0}_Identifiers", prefix));
                                
                AddTemplate(
                    template,
                    "// ListOfIdentifiers",
                    null,
                    GetNodes("", exportAll),
                    new LoadTemplateEventHandler(LoadTemplate_TypeIdAnsiC),
                    null);
                
                Context context = new Context();
                context.BlankLine = true;               
                template.WriteTemplate(context);       
            }
            finally
            {
                writer.Close();
            }
        }
               
        /// <summary>
        /// Writes the code to defined a identifier for a type.
        /// </summary>
        private string LoadTemplate_TypeIdAnsiC(Template template, Context context)
        {
            NodeDesign node = context.Target as NodeDesign;

            if (node == null)
            {
                return null;
            }
            
            template.WriteNextLine(context.Prefix);            
            template.Write("#define OpcUaId_{0}", node.SymbolicId.Name);
            template.Write(" {0}", node.NumericId);

            return null;
        }    
               
        /// <summary>
        /// Creates a class that defines all types in the namespace.
        /// </summary>
        private void WriteTemplate_NamesAnsiC(string filePath, string prefix, bool exportAll)
        {          
			StreamWriter writer = new StreamWriter(String.Format(@"{0}\{1}_browsenames.h", filePath, prefix.ToLower()), false);

            try
            {
                string templateName = "Types.Identifiers.h";
                
                Template template = new Template(writer, TemplatePath + templateName, Assembly.GetExecutingAssembly());
                
                template.AddReplacement("_Date_", DateTime.Now.ToShortDateString());
                template.AddReplacement("_FileName_", String.Format("{0}_BrowseNames", prefix));
                                
                List<NodeDesign> nodes = GetNodeList();
                SortedDictionary<string,string> browseNames = GetBrowseNames(nodes);

                AddTemplate(
                    template,
                    "// ListOfIdentifiers",
                    null,
                    browseNames,
                    new LoadTemplateEventHandler(LoadTemplate_NameAnsiC),
                    null);
                
                template.WriteTemplate(null);       
            }
            finally
            {
                writer.Close();
            }
        }

        /// <summary>
        /// Writes the code to defined a identifier for a type.
        /// </summary>
        private string LoadTemplate_NameAnsiC(Template template, Context context)
        {
            KeyValuePair<string,string>? browseName = context.Target as KeyValuePair<string,string>?;

            if (browseName == null)
            {
                return null;
            }

            template.WriteNextLine(context.Prefix);            
            template.Write("#define OpcUa_BrowseName_{0}", browseName.Value.Key);
            template.Write(" \"{0}\"", browseName.Value.Value);

            return null;
        }    

        /// <summary>
        /// Creates a class that defines all types in the namespace.
        /// </summary>
        private void WriteTemplate_Namespaces(string filePath, string prefix)
        {          
			StreamWriter writer = new StreamWriter(String.Format(@"{0}\{1}Namespaces.cs", filePath, prefix), false);

            try
            {
                string templateName = "Types.Namespaces.cs";
                
                Template template = new Template(writer, TemplatePath + templateName, Assembly.GetExecutingAssembly());
                
                template.AddReplacement("_Namespace_", GetNamespaceCodePath(m_model.TargetNamespace, false));
                template.AddReplacement("_NamespaceUri_", GetConstantForNamespace(m_model.TargetNamespace));
                template.AddReplacement("_NamespaceName_", GetNamespaceCodePath(m_model.TargetNamespace, false));
                                
                AddTemplate(
                    template,
                    "// ListOfImports",
                    null,
                    m_model.Namespaces,
                    new LoadTemplateEventHandler(LoadTemplate_NamespaceImports),
                    null);

                AddTemplate(
                    template,
                    "// ListOfNamespaceUris",
                    TemplatePath + "Types.NamespaceUri.cs",
                    m_model.Namespaces,
                    null,
                    new WriteTemplateEventHandler(WriteTemplate_CodeNamespaceUri));

                template.WriteTemplate(null);       
            }
            finally
            {
                writer.Close();
            }
        }
 
        /// <summary>
        /// Writes the code to defined a identifier for a type.
        /// </summary>
        private string LoadTemplate_CodeTypeId(Template template, Context context)
        {
            NodeDesign node = context.Target as NodeDesign;

            if (node == null)
            {
                return null;
            }

            if (node is TypeDesign || node.Parent is InstanceDesign || node is EncodingDesign)
            {
                return null;
            }

            return context.TemplatePath;
        }

        /// <summary>
        /// Writes the code to defined a identifier for a type.
        /// </summary>
        private bool WriteTemplate_CodeTypeId(Template template, Context context)
        {
            NodeDesign node = context.Target as NodeDesign;

            if (node == null)
            {
                return false;
            }
            
            template.AddReplacement("_Identifier_", node.NumericId.ToString()); 
            template.AddReplacement("_SymbolicId_", node.SymbolicId.Name); 
            template.AddReplacement("_BrowseName_", node.BrowseName); 
            template.AddReplacement("_NodeClass_", GetNodeClass(node)); 
            template.AddReplacement("_ClassName_", "Id"); 

            return template.WriteTemplate(context);
        }        

        /// <summary>
        /// Writes the code define a constant for a namespace uri.
        /// </summary>
        private bool WriteTemplate_CodeNamespaceUri(Template template, Context context)
        {
            Namespace ns = context.Target as Namespace;

            if (ns == null)
            {
                return false;
            }
            
            template.AddReplacement("_Name_", ns.Name); 
            template.AddReplacement("_CodeName_", ns.Prefix); 
            
            if (ns.Value == DefaultNamespace)
            {
                template.AddReplacement("_NamespaceUri_", Namespaces.OpcUa); 
            }
            else
            {
                template.AddReplacement("_NamespaceUri_", ns.Value); 
            }

            return template.WriteTemplate(context);
        }
        
        /// <summary>
        /// Writes the code to defined a identifier for a type.
        /// </summary>
        private string LoadTemplate_NamespaceImports(Template template, Context context)
        {
            Namespace ns = context.Target as Namespace;

            if (ns == null)
            {
                return null;
            }

            if (ns.Value == m_model.TargetNamespace)
            {
                return null;
            }
            
            string internalPrefix = GetNamespaceCodePath(ns.Value, true);
            string externalPrefix = GetNamespaceCodePath(ns.Value, false);
            
            template.WriteNextLine(context.Prefix);            
            template.Write("using {0};", externalPrefix);

            if (externalPrefix != internalPrefix)
            {
                template.WriteNextLine(context.Prefix);            
                template.Write("using {0};", internalPrefix);
            }
                        
            return null;
        }       

        
        /// <summary>
        /// Returns the list of nodes.
        /// </summary>
        /// <returns></returns>
        private IEnumerable<NodeDesign> GetNodes(string nodeClass)
        {
            return GetNodes(nodeClass, false);
        }

        /// <summary>
        /// Returns the list of nodes.
        /// </summary>
        private IList<NodeDesign> GetNodes(string nodeClass, bool exportAll)
        {
            SortedList<string,NodeDesign> nodes = new SortedList<string,NodeDesign>();
            
            foreach (NodeDesign node in m_validator.Nodes)
            {
                if (node.IsDeclaration && !exportAll)
                {
                    continue;
                }
                
                bool nestedDeclaration = false;

                InstanceDesign instance = node as InstanceDesign;

                while (instance != null && instance.Parent != null)
                {
                    if (instance.Parent.IsDeclaration && !exportAll)
                    {
                        nestedDeclaration = true;
                        break;
                    }

                    instance = instance.Parent as InstanceDesign;
                }

                if (nestedDeclaration)
                {
                    continue;
                }

                if (String.IsNullOrEmpty(nodeClass))
                {    
                    nodes.Add(node.SymbolicId.Name, node);
                    continue;
                }

                if (nodeClass == "Object")
                {        
                    if (node is ObjectDesign)
                    {
                        nodes.Add(node.SymbolicId.Name, node);
                    }

                    continue;
                }
                
                if (nodeClass == "Variable")
                {          
                    if (node is VariableDesign)
                    {
                        nodes.Add(node.SymbolicId.Name, node);
                    }

                    continue;
                }

                if (nodeClass == "ReferenceType")
                {
                    if (node is ReferenceTypeDesign)
                    {
                        nodes.Add(node.SymbolicId.Name, node);
                    }

                    continue;
                }

                if (nodeClass == "DataType")
                {
                    DataTypeDesign datatype = node as DataTypeDesign;

                    if (datatype != null)
                    {
                        nodes.Add(node.SymbolicId.Name, datatype);
                    }

                    continue;
                }

                if (nodeClass == "ObjectType")
                {
                    if (node is ObjectTypeDesign)
                    {
                        nodes.Add(node.SymbolicId.Name, node);
                    }

                    continue;
                }

                if (nodeClass == "VariableType")
                {
                    if (node is VariableTypeDesign)
                    {
                        nodes.Add(node.SymbolicId.Name, node);
                    }

                    continue;
                }
            }

            return nodes.Values;
        }
        #endregion
        
        #region XmlSchema Class Generation
        /// <summary>
        /// Creates a class that defines all types in the namespace.
        /// </summary>
        private void WriteTemplate_XmlSchema(string filePath, string prefix, bool exportAll)
        {            
			StreamWriter writer = new StreamWriter(String.Format(@"{0}\{1}Types.xsd", filePath, prefix), false);

            try
            {
                Template template = new Template(writer, TemplatePath + "XmlSchema.File.xml", Assembly.GetExecutingAssembly());
                
                template.AddReplacement("_Namespace_", m_model.TargetXmlNamespace);
                                                           
                AddTemplate(
                    template,
                    "xmlns:s0=\"ListOfNamespaces\"",
                    null,
                    m_model.Namespaces,
                    new LoadTemplateEventHandler(LoadTemplate_XmlNamespaceImports),
                    null);

                AddTemplate(
                    template,
                    "<!-- Imports -->",
                    null,
                    m_model.Namespaces,
                    new LoadTemplateEventHandler(LoadTemplate_XmlNamespaceImports),
                    null);
                
                AddTemplate(
                    template,
                    "<!-- BuiltInTypes -->",
                    "Opc.Ua.ModelCompiler.StackGenerator.DataTypes.Templates.XmlSchema.BuiltInTypes.xsd",
                    new ModelDesign[] { m_model },
                    new LoadTemplateEventHandler(LoadTemplate_XmlType),
                    new WriteTemplateEventHandler(WriteTemplate_XmlType));

                AddTemplate(
                    template,
                    "<!-- ListOfTypes -->",
                    null,
                    GetNodes("DataType", exportAll),
                    new LoadTemplateEventHandler(LoadTemplate_XmlType),
                    new WriteTemplateEventHandler(WriteTemplate_XmlType));
                
                template.WriteTemplate(null);       
            }
            finally
            {
                writer.Close();
            }
        }
        
        /// <summary>
        /// Writes the code to defined a identifier for a type.
        /// </summary>
        private string LoadTemplate_XmlNamespaceImports(Template template, Context context)
        {
            Namespace ns = context.Target as Namespace;

            if (ns == null)
            {
                return null;
            }

            if (ns.Value == m_model.TargetNamespace)
            {
                return null;
            }

            string uri = ns.Value;

            if (!String.IsNullOrEmpty(ns.XmlNamespace))
            {
                uri = ns.XmlNamespace;
            }
            
            if (context.Token.Contains("xmlns:s0"))
            {
                if (ns.Value == DefaultNamespace)
                {
                    return null;
                }                               

                template.WriteNextLine(context.Prefix);
                template.Write("xmlns:s{0}=\"{1}\"", GetNamespaceIndex(ns.Value), uri);
                return null;
            }

            template.WriteNextLine(context.Prefix);            
            template.Write("<xs:import namespace=\"{0}\" />", uri);
                        
            return null;
        }        
        
        /// <summary>
        /// Writes the code to defined a identifier for a type.
        /// </summary>
        private string LoadTemplate_XmlType(Template template, Context context)
        {
            DataTypeDesign dataType = context.Target as DataTypeDesign;

            if (dataType == null)
            {
                if (context.Token == "<!-- BuiltInTypes -->" && m_model.TargetNamespace == DefaultNamespace)
                {
                    return context.TemplatePath;
                }

                return null;
            }

            BasicDataType basicType = GetBasicDataType(dataType);
            
            if (basicType == BasicDataType.Enumeration)
            {
                if (dataType.SymbolicId == new XmlQualifiedName("Enumeration", DefaultNamespace))
                {
                    return null;
                }

                return TemplatePath + "XmlSchema.EnumeratedType.xml";
            }
            
            if (basicType == BasicDataType.Structure)
            {
                if (dataType.SymbolicId == new XmlQualifiedName("Structure", DefaultNamespace))
                {
                    return null;
                }

                if (dataType.BaseTypeNode.SymbolicName.Name == "Structure")
                {
                    return TemplatePath + "XmlSchema.ComplexType.xml";
                }
                else
                {
                    return TemplatePath + "XmlSchema.DerivedType.xml";
                }
            }
            
            return null;
        }   

        /// <summary>
        /// Writes the code to defined a identifier for a type.
        /// </summary>
        private bool WriteTemplate_XmlType(Template template, Context context)
        {
            DataTypeDesign dataType = context.Target as DataTypeDesign;

            if (dataType == null)
            {
                if (context.Token == "<!-- BuiltInTypes -->" && m_model.TargetNamespace == DefaultNamespace)
                {
                    template.WriteNextLine(context.Prefix);  
                    return template.WriteTemplate(context);
                }

                return false;
            }   

            if (context.FirstInList)
            {
                template.WriteNextLine(context.Prefix);            
            }

            DataTypeDesign baseType = dataType.BaseTypeNode as DataTypeDesign;

            if (baseType != null)
            {
                template.AddReplacement("_BaseType_", GetXmlDataType(baseType, ValueRank.Scalar)); 
            }
            
            template.AddReplacement("_TypeName_", dataType.SymbolicName.Name); 
            template.AddReplacement("_Description_", dataType.Description.Value); 

            AddTemplate(
                template,
                "<!-- ListOfFields -->",
                null,
                dataType.Fields,
                new LoadTemplateEventHandler(LoadTemplate_XmlTypeFields),
                null);
                              
            return template.WriteTemplate(context);
        }     
 
        /// <summary>
        /// Writes the code to defined a identifier for a type.
        /// </summary>
        private string LoadTemplate_XmlTypeFields(Template template, Context context)
        {
            Parameter field = context.Target as Parameter;

            if (field == null)
            {
                return null;
            }
            
            BasicDataType basicType = GetBasicDataType(field.Parent as DataTypeDesign);
            
            if (basicType == BasicDataType.Enumeration)
            {   
                template.WriteNextLine(context.Prefix);            
                template.Write("<xs:enumeration value=\"{0}_{1}\" />", field.Name, field.Identifier);
                return null;
            }
            
            basicType = GetBasicDataType(field.DataTypeNode);

            if (basicType == BasicDataType.XmlElement)
            {   
                template.WriteNextLine(context.Prefix);   
                template.Write("<xs:element name=\"{0}\" >", field.Name);            
                template.WriteNextLine(context.Prefix);
                template.Write("  <xs:complexType>");                
                template.WriteNextLine(context.Prefix);
                template.Write("    <xs:sequence>");                
                template.WriteNextLine(context.Prefix);
                template.Write("      <xs:any minOccurs=\"0\" processContents=\"lax\" />");        
                template.WriteNextLine(context.Prefix);
                template.Write("    </xs:sequence>");          
                template.WriteNextLine(context.Prefix);
                template.Write("  </xs:complexType>");       
                template.WriteNextLine(context.Prefix);   
                template.Write("</xs:element>");     
                return null;
            }
    
            template.WriteNextLine(context.Prefix);

            switch (basicType)
            {
                case BasicDataType.String:
                case BasicDataType.ByteString:
                case BasicDataType.DiagnosticInfo:
                case BasicDataType.ExpandedNodeId:
                case BasicDataType.LocalizedText:
                case BasicDataType.NodeId:
                case BasicDataType.QualifiedName:
                {
                    template.Write("<xs:element name=\"{0}\" type=\"{1}\" minOccurs=\"0\" nillable=\"true\" />", field.Name, GetXmlDataType(field.DataTypeNode, field.ValueRank));
                    break;
                }
                    
                case BasicDataType.Guid:
                case BasicDataType.StatusCode:
                {
                    template.Write("<xs:element name=\"{0}\" type=\"{1}\" minOccurs=\"0\" />", field.Name, GetXmlDataType(field.DataTypeNode, field.ValueRank));
                    break;
                }

                default:
                {   
                    template.Write("<xs:element name=\"{0}\" type=\"{1}\" minOccurs=\"1\" />", field.Name, GetXmlDataType(field.DataTypeNode, field.ValueRank));
                    break;
                }
            }
      
            return null;
        }       

        /// <summary>
        /// Returns the data type to use for the value of a variable or the argument of a method.
        /// </summary>
        private string GetXmlDataType(DataTypeDesign dataType, ValueRank valueRank)
        {
            if (valueRank != ValueRank.Scalar)
            {
                switch (GetBasicDataType(dataType))
                {
                    case BasicDataType.Boolean: { return "ua:ListOfBoolean"; }
                    case BasicDataType.SByte: { return "ua:ListOfSByte"; }
                    case BasicDataType.Int16: { return "ua:ListOfInt16"; }
                    case BasicDataType.UInt16: { return "ua:ListOfUInt16"; }
                    case BasicDataType.Int32: { return "ua:ListOfInt32"; }
                    case BasicDataType.UInt32: { return "ua:ListOfUInt32"; }
                    case BasicDataType.Int64: { return "ua:ListOfInt64"; }
                    case BasicDataType.UInt64: { return "ua:ListOfUInt64"; }
                    case BasicDataType.Float: { return "ua:ListOfFloat"; }
                    case BasicDataType.Double: { return "ua:ListOfDouble"; }
                    case BasicDataType.String: { return "ua:ListOfString"; }
                    case BasicDataType.DateTime: { return "ua:ListOfDateTime"; }
                    case BasicDataType.Guid: { return "ua:ListOfGuid"; }
                    case BasicDataType.ByteString: { return "ua:ListOfByteString"; }
                    case BasicDataType.XmlElement: { return "ua:ListOfXmlElement"; }
                    case BasicDataType.NodeId: { return "ua:ListOfNodeId"; }
                    case BasicDataType.ExpandedNodeId: { return "ua:ListOfExpandedNodeId"; }
                    case BasicDataType.StatusCode: { return "ua:ListOfStatusCode"; }
                    case BasicDataType.DiagnosticInfo: { return "ua:ListOfDiagnosticInfo"; }
                    case BasicDataType.QualifiedName: { return "ua:ListOfQualifiedName"; }
                    case BasicDataType.LocalizedText: { return "ua:ListOfLocalizedText"; }
                    case BasicDataType.DataValue: { return "ua:ListOfDataValue"; }
                    case BasicDataType.Number: { return "ua:ListOfDouble"; }
                    case BasicDataType.Integer: { return "ua:ListOfInt64"; }
                    case BasicDataType.UInteger: { return "ua:ListOfUInt64"; }
                    case BasicDataType.BaseDataType:  { return "ua:ListOfVariant"; }
                        
                    default:
                    case BasicDataType.Enumeration:
                    case BasicDataType.Structure:
                    {
                        if (dataType.SymbolicName == new XmlQualifiedName("Structure", DefaultNamespace))
                        {
                            return String.Format("ua:ListOfExtensionObject");
                        }

                        string prefix = "tns";

                        if (dataType.SymbolicName.Namespace != m_model.TargetNamespace)
                        {
                            if (dataType.SymbolicName.Namespace == DefaultNamespace)
                            {
                                if (dataType.SymbolicName.Name == "Enumeration")
                                {
                                    return String.Format("ua:ListOfInt32");
                                }

                                prefix = "ua";
                            }
                            else
                            {
                                prefix = String.Format("s{0}", GetNamespaceIndex(dataType.SymbolicName.Namespace));
                            }
                        }

                        return String.Format("{0}:ListOf{1}", prefix, dataType.SymbolicName.Name);
                    }
                }
            }

            switch (GetBasicDataType(dataType))
            {
                case BasicDataType.Boolean: { return "xs:boolean"; }
                case BasicDataType.SByte: { return "xs:byte"; }
                case BasicDataType.Byte: { return "xs:unsignedByte"; }
                case BasicDataType.Int16: { return "xs:short"; }
                case BasicDataType.UInt16: { return "xs:unsignedShort"; }
                case BasicDataType.Int32: { return "xs:int"; }
                case BasicDataType.UInt32: { return "xs:unsignedInt"; }
                case BasicDataType.Int64: { return "xs:long"; }
                case BasicDataType.UInt64: { return "xs:unsignedLong"; }
                case BasicDataType.Float: { return "xs:float"; }
                case BasicDataType.Double: { return "xs:double"; }
                case BasicDataType.String: { return "xs:string"; }
                case BasicDataType.DateTime: { return "xs:dateTime"; }
                case BasicDataType.Guid: { return "ua:Guid"; }
                case BasicDataType.ByteString: { return "xs:base64Binary"; }
                case BasicDataType.XmlElement: { return "ua:XmlElement"; }
                case BasicDataType.NodeId: { return "ua:NodeId"; }
                case BasicDataType.ExpandedNodeId: { return "ua:ExpandedNodeId"; }
                case BasicDataType.StatusCode: { return "ua:StatusCode"; }
                case BasicDataType.DiagnosticInfo: { return "ua:DiagnosticInfo"; }
                case BasicDataType.QualifiedName: { return "ua:QualifiedName"; }
                case BasicDataType.LocalizedText: { return "ua:LocalizedText"; }
                case BasicDataType.DataValue: { return "ua:DataValue"; }
                case BasicDataType.Number: { return "xs:double"; }
                case BasicDataType.Integer: { return "xs:long"; }
                case BasicDataType.UInteger: { return "xs:ulong"; }
                case BasicDataType.BaseDataType:  { return "ua:Variant"; }
                    
                default:
                case BasicDataType.Enumeration:
                case BasicDataType.Structure:
                {
                    if (dataType.SymbolicName == new XmlQualifiedName("Structure", DefaultNamespace))
                    {
                        return String.Format("ua:ExtensionObject");
                    }

                    string prefix = "tns";

                    if (dataType.SymbolicName.Namespace != m_model.TargetNamespace)
                    {
                        if (dataType.SymbolicName.Namespace == DefaultNamespace)
                        {
                            if (dataType.SymbolicName.Name == "Enumeration")
                            {
                                return String.Format("ua:Int32");
                            }

                            prefix = "ua";
                        }
                        else
                        {
                            prefix = String.Format("s{0}", GetNamespaceIndex(dataType.SymbolicName.Namespace));
                        }
                    }

                    return String.Format("{0}:{1}", prefix, dataType.SymbolicName.Name);
                }
            }
        }
        #endregion
        
        #region BinarySchema Class Generation
        /// <summary>
        /// Creates a class that defines all types in the namespace.
        /// </summary>
        private void WriteTemplate_BinarySchema(string filePath, string prefix, bool exportAll)
        {            
			StreamWriter writer = new StreamWriter(String.Format(@"{0}\{1}Types.bsd", filePath, prefix), false);

            try
            {
                Template template = new Template(writer, TemplatePath + "BinarySchema.File.xml", Assembly.GetExecutingAssembly());
                
                template.AddReplacement("_DictionaryUri_", m_model.TargetNamespace);
                                                           
                AddTemplate(
                    template,
                    "xmlns:s0=\"ListOfNamespaces\"",
                    null,
                    m_model.Namespaces,
                    new LoadTemplateEventHandler(LoadTemplate_BinaryNamespaceImports),
                    null);

                AddTemplate(
                    template,
                    "<!-- Imports -->",
                    null,
                    m_model.Namespaces,
                    new LoadTemplateEventHandler(LoadTemplate_BinaryNamespaceImports),
                    null);
                
                AddTemplate(
                    template,
                    "<!-- BuiltInTypes -->",
                    TemplatePath + "BinarySchema.BuiltInTypes.bsd",
                    new ModelDesign[] { m_model },
                    new LoadTemplateEventHandler(LoadTemplate_BinaryType),
                    new WriteTemplateEventHandler(WriteTemplate_BinaryType));

                AddTemplate(
                    template,
                    "<!-- ListOfTypes -->",
                    null,
                    GetNodes("DataType", exportAll),
                    new LoadTemplateEventHandler(LoadTemplate_BinaryType),
                    new WriteTemplateEventHandler(WriteTemplate_BinaryType));

                template.WriteTemplate(null);       
            }
            finally
            {
                writer.Close();
            }
        }
        
        /// <summary>
        /// Writes the code to defined a identifier for a type.
        /// </summary>
        private string LoadTemplate_BinaryNamespaceImports(Template template, Context context)
        {
            Namespace ns = context.Target as Namespace;

            if (ns == null)
            {
                return null;
            }

            if (ns.Value == m_model.TargetNamespace)
            {
                return null;
            }
            
            if (context.Token.Contains("xmlns:s0"))
            {
                if (ns.Value == DefaultNamespace)
                {
                    return null;
                }                               

                template.WriteNextLine(context.Prefix);            
                template.Write("xmlns:s{0}=\"{1}\"", GetNamespaceIndex(ns.Value), ns.Value);
                return null;
            }

            template.WriteNextLine(context.Prefix);            
            template.Write("<opc:Import Namespace=\"{0}\" Location=\"{1}.BinarySchema.bsd\"/>", ns.Value, GetNamespaceCodePath(ns.Value, false));
                        
            return null;
        }        
        
        /// <summary>
        /// Writes the code to defined a identifier for a type.
        /// </summary>
        private string LoadTemplate_BinaryType(Template template, Context context)
        {
            DataTypeDesign dataType = context.Target as DataTypeDesign;

            if (dataType == null)
            {
                if (context.Token == "<!-- BuiltInTypes -->" && m_model.TargetNamespace == DefaultNamespace)
                {
                    return context.TemplatePath;
                }

                return null;
            }

            BasicDataType basicType = GetBasicDataType(dataType);
            
            if (basicType == BasicDataType.Enumeration)
            {
                if (!dataType.HasFields)
                {
                    return null;
                }

                return TemplatePath + "BinarySchema.EnumeratedType.xml";
            }
            
            if (basicType == BasicDataType.Structure)
            {
                if (!dataType.HasFields)
                {
                    return null;
                }

                return TemplatePath + "BinarySchema.ComplexType.xml";
            }
            
            return null;
        }   

        /// <summary>
        /// Writes the code to defined a identifier for a type.
        /// </summary>
        private bool WriteTemplate_BinaryType(Template template, Context context)
        {
            DataTypeDesign dataType = context.Target as DataTypeDesign;

            if (dataType == null)
            {
                if (context.Token == "<!-- BuiltInTypes -->" && m_model.TargetNamespace == DefaultNamespace)
                {
                    template.WriteNextLine(context.Prefix);  
                    return template.WriteTemplate(context);
                }

                return false;
            }   
            
            if (context.FirstInList)
            {
                template.WriteNextLine(context.Prefix);            
            }

            template.AddReplacement("_TypeName_", dataType.SymbolicName.Name); 
            template.AddReplacement("_Description_", dataType.Description.Value); 

            List<Parameter> fields = new List<Parameter>();

            DataTypeDesign baseType = dataType;

            while (baseType != null)
            {
                if (baseType.Fields != null)
                {
                    fields.InsertRange(0, baseType.Fields);
                }

                baseType = baseType.BaseTypeNode as DataTypeDesign;
            }

            AddTemplate(
                template,
                "<!-- ListOfFields -->",
                null,
                fields,
                new LoadTemplateEventHandler(LoadTemplate_BinaryTypeFields),
                null);
                              
            return template.WriteTemplate(context);
        }     
 
        /// <summary>
        /// Writes the code to defined a identifier for a type.
        /// </summary>
        private string LoadTemplate_BinaryTypeFields(Template template, Context context)
        {
            Parameter field = context.Target as Parameter;

            if (field == null)
            {
                return null;
            }
            
            BasicDataType basicType = GetBasicDataType(field.Parent as DataTypeDesign);
            
            if (basicType == BasicDataType.Enumeration)
            {   
                template.WriteNextLine(context.Prefix);            
                template.Write("<opc:EnumeratedValue Name=\"{0}\" Value=\"{1}\" />", field.Name, field.Identifier);
                return null;
            }

            if (field.ValueRank != ValueRank.Scalar)
            {
                template.WriteNextLine(context.Prefix);            
                template.Write("<opc:Field Name=\"NoOf{0}\" TypeName=\"opc:Int32\" />", field.Name);
                template.WriteNextLine(context.Prefix);            
                template.Write("<opc:Field Name=\"{0}\" TypeName=\"{1}\" LengthField=\"NoOf{0}\" />", field.Name, GetBinaryDataType(field.DataTypeNode));
                return null;    
            }
   
            template.WriteNextLine(context.Prefix);            
            template.Write("<opc:Field Name=\"{0}\" TypeName=\"{1}\" />", field.Name, GetBinaryDataType(field.DataTypeNode));
            return null;
        }       

        /// <summary>
        /// Returns the data type to use for the value of a variable or the argument of a method.
        /// </summary>
        private string GetBinaryDataType(DataTypeDesign dataType)
        {
            switch (GetBasicDataType(dataType))
            {
                case BasicDataType.Boolean: { return "opc:Boolean"; }
                case BasicDataType.SByte: { return "opc:SByte"; }
                case BasicDataType.Byte: { return "opc:Byte"; }
                case BasicDataType.Int16: { return "opc:Int16"; }
                case BasicDataType.UInt16: { return "opc:UInt16"; }
                case BasicDataType.Int32: { return "opc:Int32"; }
                case BasicDataType.UInt32: { return "opc:UInt32"; }
                case BasicDataType.Int64: { return "opc:Int64"; }
                case BasicDataType.UInt64: { return "opc:UInt64"; }
                case BasicDataType.Float: { return "opc:Float"; }
                case BasicDataType.Double: { return "opc:Double"; }
                case BasicDataType.String: { return "opc:String"; }
                case BasicDataType.DateTime: { return "opc:DateTime"; }
                case BasicDataType.Guid: { return "opc:Guid"; }
                case BasicDataType.ByteString: { return "opc:ByteString"; }
                case BasicDataType.XmlElement: { return "ua:XmlElement"; }
                case BasicDataType.NodeId: { return "ua:NodeId"; }
                case BasicDataType.ExpandedNodeId: { return "ua:ExpandedNodeId"; }
                case BasicDataType.StatusCode: { return "ua:StatusCode"; }
                case BasicDataType.DiagnosticInfo: { return "ua:DiagnosticInfo"; }
                case BasicDataType.QualifiedName: { return "ua:QualifiedName"; }
                case BasicDataType.LocalizedText: { return "ua:LocalizedText"; }
                case BasicDataType.DataValue: { return "ua:DataValue"; }
                case BasicDataType.Number: { return "ua:Double"; }
                case BasicDataType.Integer: { return "ua:Int64"; }
                case BasicDataType.UInteger: { return "ua:UInt64"; }
                case BasicDataType.BaseDataType:  { return "ua:Variant"; }
                    
                default:
                case BasicDataType.Enumeration:
                case BasicDataType.Structure:
                {
                    if (dataType.SymbolicName == new XmlQualifiedName("Structure", DefaultNamespace))
                    {
                        return String.Format("ua:ExtensionObject");
                    }

                    string prefix = "tns";

                    if (dataType.SymbolicName.Namespace != m_model.TargetNamespace)
                    {
                        if (dataType.SymbolicName.Namespace == DefaultNamespace)
                        {
                            prefix = "ua";
                        }
                        else
                        {
                            prefix = String.Format("s{0}", GetNamespaceIndex(dataType.SymbolicName.Namespace));
                        }
                    }
                                            
                    return String.Format("{0}:{1}", prefix, dataType.SymbolicName.Name);
                }
            }
        }
        #endregion

        #region DataType Class Generation
        /// <summary>
        /// Creates a class that defines all types in the namespace.
        /// </summary>
        private void WriteTemplate_DataTypes(string filePath, List<NodeDesign> nodes)
        {            
			StreamWriter writer = new StreamWriter(String.Format(@"{0}\DataTypes.cs", filePath), false);

            try
            {
                Template template = new Template(writer, TemplatePath + "DataTypes.File.cs", Assembly.GetExecutingAssembly());
                
                template.AddReplacement("_Namespace_", GetNamespaceCodePath(m_model.TargetNamespace, false));
                template.AddReplacement("_NamespaceUri_", GetConstantForNamespace(m_model.TargetNamespace));
                           
                nodes = SortTypeByDependencies(nodes);
                                
                AddTemplate(
                    template,
                    "// ListOfImports",
                    null,
                    m_model.Namespaces,
                    new LoadTemplateEventHandler(LoadTemplate_NamespaceImports),
                    null);

                AddTemplate(
                    template,
                    "// ListOfDataTypes",
                    TemplatePath + "DataTypes.Class.cs",
                    nodes,
                    new LoadTemplateEventHandler(LoadTemplate_DataTypeClass),
                    new WriteTemplateEventHandler(WriteTemplate_DataTypeClass));
                
                template.WriteTemplate(null);       
            }
            finally
            {
                writer.Close();
            }
        }
        
        /// <summary>
        /// Loads the template 
        /// </summary>
        private string LoadTemplate_DataTypeClass(Template template, Context context)
        {
            DataTypeDesign dataType = context.Target as DataTypeDesign;

            if (dataType == null)
            {
                return null;
            }

            BasicDataType basicType = GetBasicDataType(dataType);

            if (basicType == BasicDataType.Enumeration)
            {
                if (dataType.Fields == null)
                {
                    return null;
                }
                
                return TemplatePath + "DataTypes.Enumeration.cs";
            }

            if (basicType != BasicDataType.Structure)
            {
                return null;
            }

            return context.TemplatePath;
        }

        /// <summary>
        /// Writes the code to defined a identifier for a type.
        /// </summary>
        private bool WriteTemplate_DataTypeClass(Template template, Context context)
        {
            DataTypeDesign dataType = context.Target as DataTypeDesign;

            if (dataType == null)
            {
                return false;
            }
            
            BasicDataType basicType = GetBasicDataType(dataType);
            
            template.AddReplacement("_BrowseName_", dataType.SymbolicName.Name); 
            template.AddReplacement("_BaseType_", dataType.BaseTypeNode.ClassName); 
            template.AddReplacement("_BaseTypeNodeId_", GetBaseTypeNodeId(dataType)); 
            template.AddReplacement("_DisplayName_", dataType.DisplayName.Value);   
            template.AddReplacement("_DisplayNameKey_", dataType.DisplayName.Key);   
            template.AddReplacement("_Description_", dataType.Description.Value);    
            template.AddReplacement("_DescriptionKey_", dataType.Description.Key);   
            template.AddReplacement("_DefaultLocale_", m_model.DefaultLocale);      
            template.AddReplacement("_NamespaceUri_", GetConstantForNamespace(dataType.SymbolicId.Namespace)); 
            template.AddReplacement("_IsAbstract_", GetBooleanValue(dataType.IsAbstract)); 
            template.AddReplacement("_WriteAccess_", dataType.WriteAccess);        

            if (basicType == BasicDataType.Structure)
            {
                template.AddReplacement("_Encodings_", "encodings");
            }
            else
            {
                template.AddReplacement("_Encodings_", "null");
            }

            template.AddReplacement("// _SerializationAttribute_", GetSerializationAttribute(dataType, false));
            
            // add new qualifier to static methods in dervived classes.
            if (dataType.BaseTypeNode.SymbolicName.Name != "Structure")
            {
                template.AddReplacement("_Qualifier_", "new ");
            }
            else
            {
                template.AddReplacement("_Qualifier_", "");
            }

            AddTemplate(
                template,
                "// ListOfFieldInitializers",
                null,
                dataType.Fields,
                new LoadTemplateEventHandler(LoadTemplate_DataTypeInitializers),
                null);
            
            AddTemplate(
                template,
                "// ListOfFields",
                null,
                dataType.Fields,
                new LoadTemplateEventHandler(LoadTemplate_DataTypeFields),
                null);
            
            AddTemplate(
                template,
                "// ListOfEncoding",
                null,
                dataType.Fields,
                new LoadTemplateEventHandler(LoadTemplate_DataTypeFieldEncoding),
                null);
            
            AddTemplate(
                template,
                "// ListOfDecoding",
                null,
                dataType.Fields,
                new LoadTemplateEventHandler(LoadTemplate_DataTypeFieldDecoding),
                null);
            
            AddTemplate(
                template,
                "// ListOfComparisons",
                null,
                dataType.Fields,
                new LoadTemplateEventHandler(LoadTemplate_DataTypeComparisons),
                null);
            
            AddTemplate(
                template,
                "// ListOfCopies",
                null,
                dataType.Fields,
                new LoadTemplateEventHandler(LoadTemplate_DataTypeCopies),
                null);
                        
            AddTemplate(
                template,
                "// ListOfProperties",
                TemplatePath + "DataTypes.Property.cs",
                dataType.Fields,
                new LoadTemplateEventHandler(LoadTemplate_DataTypeProperties),
                new WriteTemplateEventHandler(WriteTemplate_DataTypeProperties));
            
            AddTemplate(
                template,
                "// ListOfValues",
                TemplatePath + "DataTypes.EnumerationValue.cs",
                dataType.Fields,
                null,
                new WriteTemplateEventHandler(WriteTemplate_DataTypeProperties));

            AddTemplate(
                template,
                "// CollectionClass",
                TemplatePath + "DataTypes.Collection.cs",
                new DataTypeDesign[] { dataType },
                new LoadTemplateEventHandler(LoadTemplate_DataTypeCollection),
                new WriteTemplateEventHandler(WriteTemplate_DataTypeCollection));
            
            return template.WriteTemplate(context);
        }      
        
        /// <summary>
        /// Loads the template 
        /// </summary>
        private string LoadTemplate_DataTypeCollection(Template template, Context context)
        {
            DataTypeDesign dataType = context.Target as DataTypeDesign;

            if (dataType == null)
            {
                return null;
            }
            
            if (dataType.NoArraysAllowed)
            {
                return null;
            }
                
            template.WriteNextLine(context.Prefix);    

            return context.TemplatePath;
        }

        /// <summary>
        /// Writes the code to defined a identifier for a type.
        /// </summary>
        private bool WriteTemplate_DataTypeCollection(Template template, Context context)
        {
            DataTypeDesign dataType = context.Target as DataTypeDesign;

            if (dataType == null)
            {
                return false;
            }
            
            template.AddReplacement("_BrowseName_", dataType.SymbolicName.Name);
            template.AddReplacement("// _CollectionSerializationAttribute_", GetSerializationAttribute(dataType, true));

            return template.WriteTemplate(context);
        }      

        /// <summary>
        /// Loads the template 
        /// </summary>
        private string LoadTemplate_DataTypeEncoding(Template template, Context context)
        {
            ObjectDesign encoding = context.Target as ObjectDesign;

            if (encoding == null)
            {
                return null;
            }

            if (context.FirstInList)
            {  
                template.WriteNextLine(context.Prefix);            
                template.Write("Dictionary<QualifiedName,NodeId> encodings = new Dictionary<QualifiedName,NodeId>();");
                template.WriteNextLine(context.Prefix);
            }

            return context.TemplatePath;
        }

        /// <summary>
        /// Writes the code to defined a identifier for a type.
        /// </summary>
        private bool WriteTemplate_DataTypeEncoding(Template template, Context context)
        {
            EncodingDesign encoding = context.Target as EncodingDesign;

            if (encoding == null)
            {
                return false;
            }

            DataTypeDesign datatype = encoding.Parent as DataTypeDesign;

            if (datatype == null)
            {
                return false;
            }

            template.AddReplacement("_BrowseName_", encoding.SymbolicName.Name); 
            template.AddReplacement("_TypeName_", datatype.SymbolicName.Name);    
            template.AddReplacement("_NamespaceUri_", GetConstantForNamespace(encoding.SymbolicName.Namespace)); 
            
            return template.WriteTemplate(context);
        }      

        /// <summary>
        /// Writes the field of a class.
        /// </summary>
        private string LoadTemplate_DataTypeInitializers(Template template, Context context)
        {
            Parameter field = context.Target as Parameter;

            if (field == null)
            {
                return null;
            }

            template.WriteNextLine(context.Prefix);            
            template.Write("{0} = {1};", GetFieldName(field), GetDataTypeFieldInitializer(field.DataTypeNode, field.ValueRank));
            
            return null;
        }

        /// <summary>
        /// Writes the field of a class.
        /// </summary>
        private string LoadTemplate_DataTypeFields(Template template, Context context)
        {
            Parameter field = context.Target as Parameter;

            if (field == null)
            {
                return null;
            }

            template.WriteNextLine(context.Prefix);            
            template.Write("private {0} {1};", GetDataTypeFieldDataType(field.DataTypeNode, field.ValueRank), GetFieldName(field));
            
            return null;
        }

        /// <summary>
        /// Writes the field of a class.
        /// </summary>
        private string LoadTemplate_DataTypeFieldEncoding(Template template, Context context)
        {
            Parameter field = context.Target as Parameter;

            if (field == null)
            {
                return null;
            }
            
            BasicDataType basicType = GetBasicDataType(field.DataTypeNode);
                        
            template.WriteNextLine(context.Prefix); 
                  
            string elementName = GetDataTypeFieldDataType(field.DataTypeNode, ValueRank.Scalar);

            // fixed structure.
            if (basicType == BasicDataType.Structure && field.DataTypeNode.SymbolicId != new XmlQualifiedName("Structure", DefaultNamespace))
            {
                if (field.ValueRank == ValueRank.Scalar)
                {
		            template.Write("encoder.WriteEncodeable(\"{0}\", {0}, typeof({1}));", field.Name, elementName);
                    return null;
                }

	            template.Write("encoder.WriteEncodeableArray(\"{0}\", ({1}[]){0}, typeof({1}));", field.Name, elementName);
                return null;                
            }

            // enumeration.
            if (basicType == BasicDataType.Enumeration)
            {
                if (field.ValueRank == ValueRank.Scalar)
                {
		            template.Write("encoder.WriteEnumerated(\"{0}\", {0});", field.Name);
                    return null;
                }
               
			
	            template.Write("encoder.WriteEnumeratedArray(\"{0}\", ({1}[]){0}, typeof({1}));", field.Name, elementName);
                return null;                
            }

            // built-in type.            
            template.Write("encoder.Write{1}(\"{0}\", {0});", field.Name, GetFunctionNameForField(field.DataTypeNode, field.ValueRank));            
            return null;
        }
            
        /// <summary>
        /// Writes the field of a class.
        /// </summary>
        private string LoadTemplate_DataTypeFieldDecoding(Template template, Context context)
        {
            Parameter field = context.Target as Parameter;

            if (field == null)
            {
                return null;
            }
            
            BasicDataType basicType = GetBasicDataType(field.DataTypeNode);
                        
            template.WriteNextLine(context.Prefix); 
                  
            string elementName = GetDataTypeFieldDataType(field.DataTypeNode, ValueRank.Scalar);

            // fixed structure.
            if (basicType == BasicDataType.Structure && field.DataTypeNode.SymbolicId != new XmlQualifiedName("Structure", DefaultNamespace))
            {
                if (field.ValueRank == ValueRank.Scalar)
                {
		            template.Write("{0} = ({1})decoder.ReadEncodeable(\"{0}\", typeof({1}));", field.Name, elementName);
                    return null;
                }
                
	            template.Write("{0} = ({1}Collection)decoder.ReadEncodeableArray(\"{0}\", typeof({1}));", field.Name, elementName);
                return null;                
            }

            // enumeration.
            if (basicType == BasicDataType.Enumeration)
            {
                if (field.ValueRank == ValueRank.Scalar)
                {
		            template.Write("{0} = ({1})decoder.ReadEnumerated(\"{0}\", typeof({1}));", field.Name, elementName);
                    return null;
                }
               
			
	            template.Write("{0} = ({1}[])decoder.ReadEnumeratedArray(\"{0}\", typeof({1}));", field.Name, elementName);
                return null;                
            }

            // built-in type.            
            template.Write("{0} = decoder.Read{1}(\"{0}\");", field.Name, GetFunctionNameForField(field.DataTypeNode, field.ValueRank));            
            return null;
        }
            
        /// <summary>
        /// Writes the field of a class.
        /// </summary>
        private string LoadTemplate_DataTypeComparisons(Template template, Context context)
        {
            Parameter field = context.Target as Parameter;

            if (field == null)
            {
                return null;
            }

            template.WriteNextLine(context.Prefix);            
            template.Write("if (!Utils.IsEqual({0}, value.{0})) return false;", GetFieldName(field));
            
            return null;
        }
        
        /// <summary>
        /// Writes the field of a class.
        /// </summary>
        private string LoadTemplate_DataTypeCopies(Template template, Context context)
        {
            Parameter field = context.Target as Parameter;

            if (field == null)
            {
                return null;
            }

            template.WriteNextLine(context.Prefix);            
            template.Write("clone.{0} = ({1})Utils.Clone(this.{0});", GetFieldName(field), GetDataTypeFieldDataType(field.DataTypeNode, field.ValueRank));
            
            return null;
        }

        /// <summary>
        /// Writes the field of a class.
        /// </summary>
        private string LoadTemplate_DataTypeWriteSchema(Template template, Context context)
        {
            Parameter field = context.Target as Parameter;

            if (field == null)
            {
                return null;
            }
            
            template.WriteNextLine(context.Prefix); 
                  
            DataTypeDesign parent = field.Parent as DataTypeDesign;

            if (parent != null && parent.IsEnumeration)
            {
	            template.Write("writer.WriteEnumerationValue(\"{0}\", {1});", field.Name, field.Identifier);
                return null;
            }

            string functionName = GetFunctionNameForField(field.DataTypeNode, field.ValueRank);
            string elementName = GetDataTypeFieldDataType(field.DataTypeNode, ValueRank.Scalar);
            string elementNamespace = GetConstantForNamespace(field.DataTypeNode.SymbolicId.Namespace);

            if (!field.DataTypeNode.IsStructure && !field.DataTypeNode.IsEnumeration)
            {                
	            template.Write("writer.Write{1}(\"{0}\");", field.Name, functionName);    
                return null;                
            }

            if (field.DataType == new XmlQualifiedName("Structure", DefaultNamespace))
            {
	            template.Write("writer.Write{1}(\"{0}\");", field.Name, functionName);    
                return null;       
            }
        
            template.Write("writer.Write{1}(\"{0}\", ", field.Name, functionName);
            template.Write("\"{0}\", {1});", elementName, elementNamespace);
            return null;
        }

        /// <summary>
        /// Writes the field of a class.
        /// </summary>
        private string LoadTemplate_DataTypeProperties(Template template, Context context)
        {
            Parameter field = context.Target as Parameter;

            if (field == null)
            {
                return null;
            }
            
            if (field.ValueRank != ValueRank.Scalar)
            {
                return TemplatePath + "DataTypes.ArrayProperty.cs";
            }
            
            return context.TemplatePath;
        }
        
        /// <summary>
        /// Creates classes that implement the model.
        /// </summary>
        private bool WriteTemplate_DataTypeProperties(Template template, Context context)
        {
            Parameter field = context.Target as Parameter;

            if (field == null)
            {
                return false;
            }
              
            template.AddReplacement("_Description_", field.Description.Value);    
            template.AddReplacement("_BrowseName_", field.Name);     
            template.AddReplacement("_FieldName_", GetFieldName(field));   
            template.AddReplacement("_TypeName_", GetDataTypeFieldDataType(field.DataTypeNode, field.ValueRank));     
            template.AddReplacement("_DefaultValue_", GetDataTypeFieldInitializer(field.DataTypeNode, field.ValueRank));  
            template.AddReplacement("_Identifier_", field.Identifier.ToString()); 
            
            string attribute = null;
            bool isEnum = context.TemplatePath.EndsWith("EnumerationValue.cs");
                        
            if (isEnum)
            {
		        attribute = String.Format("[EnumMember(Value = \"{0}_{1}\")]", field.Name, field.Identifier); 
            }     
            else
            {
                attribute = String.Format("[DataMember(Name = \"{0}\", Order = {1})]", field.Name, context.Index + 1);
            }
            
            template.AddReplacement("// _SerializationAttribute_", attribute);     
            		
            return template.WriteTemplate(context);
        }
        
        /// <summary>
        /// Returns the builtin datatype for the datatype.
        /// </summary>
        private string GetSerializationAttribute(DataTypeDesign datatype, bool isCollection)
        {
            string ns = GetConstantForNamespace(datatype.SymbolicId.Namespace);

            if (isCollection)
            {            
	            return String.Format("[CollectionDataContract(Name = \"ListOf{0}\", Namespace = {1}, ItemName=\"{0}\")]", datatype.SymbolicName.Name, ns);
            }

            return String.Format("[DataContract(Name = \"{0}\", Namespace = {1})]", datatype.SymbolicName.Name, ns);            
        }
                
        /// <summary>
        /// Returns the system type represented by the datatype.
        /// </summary>
        private string GetFunctionNameForField(DataTypeDesign datatype, ValueRank valueRank)
        {
            // ignore known types.
            if (datatype == null)
            {
                return "Variant";
            }
            
            // get the builtin type.
            BasicDataType basicType = GetBasicDataType(datatype);

            string functionName = null;

            switch (basicType)
            {               
                case BasicDataType.Boolean:         { functionName = "Boolean"; break; }
                case BasicDataType.SByte:           { functionName = "SByte"; break; }
                case BasicDataType.Byte:            { functionName = "Byte"; break; }
                case BasicDataType.Int16:           { functionName = "Int16"; break; }
                case BasicDataType.UInt16:          { functionName = "UInt16"; break; }
                case BasicDataType.Int32:           { functionName = "Int32"; break; }
                case BasicDataType.UInt32:          { functionName = "UInt32"; break; }
                case BasicDataType.Int64:           { functionName = "Int64"; break; }
                case BasicDataType.UInt64:          { functionName = "UInt64"; break; }
                case BasicDataType.Float:           { functionName = "Float"; break; }
                case BasicDataType.Double:          { functionName = "Double"; break; }
                case BasicDataType.String:          { functionName = "String"; break; }
                case BasicDataType.DateTime:        { functionName = "DateTime"; break; }
                case BasicDataType.Guid:            { functionName = "Guid"; break; }
                case BasicDataType.ByteString:      { functionName = "ByteString"; break; }
                case BasicDataType.XmlElement:      { functionName = "XmlElement"; break; }
                case BasicDataType.NodeId:          { functionName = "NodeId"; break; }
                case BasicDataType.ExpandedNodeId:  { functionName = "ExpandedNodeId"; break; }
                case BasicDataType.StatusCode:      { functionName = "StatusCode"; break; }
                case BasicDataType.DiagnosticInfo:  { functionName = "DiagnosticInfo"; break; }
                case BasicDataType.QualifiedName:   { functionName = "QualifiedName"; break; }
                case BasicDataType.LocalizedText:   { functionName = "LocalizedText"; break; }
                case BasicDataType.DataValue:       { functionName = "DataValue"; break; }
                case BasicDataType.Enumeration:     { functionName = "Enumerated"; break; }
                
                case BasicDataType.Structure:       
                { 
                    if (datatype.SymbolicId != new XmlQualifiedName("Structure", DefaultNamespace))
                    {
                        functionName = "Encodeable";
                        break;
                    }

                    functionName = "ExtensionObject"; 
                    break;
                }
                        
                default: 
                { 
                    functionName = "Variant"; 
                    break;
                }
            }

            if (valueRank != ValueRank.Scalar)
            {
                functionName += "Array";
            }

            return functionName;
        }   
        #endregion
        

        /// <summary>
        /// Writes the the function prototype for the call delegare.
        /// </summary>
        private string LoadTemplate_DeclareMethodDelegate(Template template, Context context)
        {
            MethodDesign method = context.Target as MethodDesign;

            if (method == null)
            {
                return null;
            }    
            
            int inputCount = (method.InputArguments != null)?method.InputArguments.Length:0;
            int outputCount = (method.OutputArguments != null)?method.OutputArguments.Length:0;
            
            // get the return type.
            string returnType = "void";

            if (outputCount > 0)
            {
                Parameter argument = method.OutputArguments[0];
                returnType = GetVariableValueDataType(argument.DataTypeNode, argument.ValueRank);
            }

            // treat methods with few arguments as a special case.
            if (inputCount <= 1 && outputCount <= 1)
            {
                template.WriteNextLine(context.Prefix);
                template.Write("public delegate {1} {0}MethodHandler(OperationContext context, NodeSource target", method.SymbolicName.Name, returnType);

                if (inputCount > 0)
                {                    
                    Parameter argument = method.InputArguments[0];
                    template.Write(", ");
                    template.Write(GetVariableValueDataType(argument.DataTypeNode, argument.ValueRank));
                    template.Write(" ");
                    template.Write(ToLowerCamelCase(argument.Name));
                }
                    
                template.Write(");");                    
                return null;
            }
                                    
            template.WriteNextLine(context.Prefix);
            template.Write("public delegate {1} {0}MethodHandler(", method.SymbolicName.Name, returnType);
            
            List<string> types = new List<string>();
            List<string> names = new List<string>();
            
            types.Add("OperationContext");
            names.Add("context");

            types.Add("NodeSource");
            names.Add("target");
    
            if (method.InputArguments != null)
            {
                foreach (Parameter argument in method.InputArguments)
                {
                    types.Add(GetVariableValueDataType(argument.DataTypeNode, argument.ValueRank));
                    names.Add(ToLowerCamelCase(argument.Name));
                }
            }
            
            if (method.OutputArguments != null)
            {              
                for (int ii = 1; ii < method.OutputArguments.Length; ii++)
                {
                    Parameter argument = method.OutputArguments[ii];
                    types.Add(String.Format("out {0}", GetVariableValueDataType(argument.DataTypeNode, argument.ValueRank)));
                    names.Add(ToLowerCamelCase(argument.Name));
                }
            }
            
            // calculate field length.
            int length = 0;

            for (int ii = 0; ii < types.Count; ii++)
            {
                if (length < types[ii].Length)
                {
                    length = types[ii].Length;
                }
            }

            length++;
           
            // write parameters.
            for (int ii = 0; ii < types.Count; ii++)
            {
                if (ii > 0)
                {
                    template.Write(",");
                }

                template.WriteNextLine(context.Prefix);
                template.Write("    {0}", types[ii]);
                template.Write("{0}", new string(' ', length - types[ii].Length));
                template.Write("{0}", names[ii]);
            }
            
            template.Write(");");
            
            return null;
        }

        /// <summary>
        /// Writes the the function prototype for the call method.
        /// </summary>
        private string LoadTemplate_DeclareCallInvoke(Template template, Context context)
        {
            MethodDesign method = context.Target as MethodDesign;

            if (method == null)
            {
                return null;
            }               
            
            bool includeTarget = context.Token.Contains("NodeSource target");

            string methodName = method.SymbolicName.Name;

            if (includeTarget)
            {
                methodName = "Call";
            }            
            
            int inputCount = (method.InputArguments != null)?method.InputArguments.Length:0;
            int outputCount = (method.OutputArguments != null)?method.OutputArguments.Length:0;
            
            // get the return type.
            string returnType = "void";

            if (outputCount > 0)
            {
                Parameter argument = method.OutputArguments[0];
                returnType = GetVariableValueDataType(argument.DataTypeNode, argument.ValueRank);
            }

            // treat methods with few arguments as a special case.
            if (inputCount <= 1 && outputCount <= 1)
            {
                template.WriteNextLine(context.Prefix);
                template.Write("public {1} {0}(OperationContext context", methodName, returnType);
                
                if (includeTarget)
                {
                    template.Write(", NodeSource target");
                }

                if (inputCount > 0)
                {                    
                    Parameter argument = method.InputArguments[0];
                    template.Write(", ");
                    template.Write(GetVariableValueDataType(argument.DataTypeNode, argument.ValueRank));
                    template.Write(" ");
                    template.Write(ToLowerCamelCase(argument.Name));
                }
                    
                template.Write(")");                    
                return null;
            }
                        
            template.WriteNextLine(context.Prefix);
            template.Write("public {1} {0}(", methodName, returnType);
            
            List<string> types = new List<string>();
            List<string> names = new List<string>();
            
            types.Add("OperationContext");
            names.Add("context");

            if (includeTarget)
            {
                types.Add("NodeSource");
                names.Add("target");
            }

            if (method.InputArguments != null)
            {    
                foreach (Parameter argument in method.InputArguments)
                {
                    types.Add(GetVariableValueDataType(argument.DataTypeNode, argument.ValueRank));
                    names.Add(ToLowerCamelCase(argument.Name));
                }
            }
            
            if (method.OutputArguments != null)
            {
                for (int ii = 1; ii < method.OutputArguments.Length; ii++)
                {
                    Parameter argument = method.OutputArguments[ii];
                    types.Add(String.Format("out {0}", GetVariableValueDataType(argument.DataTypeNode, argument.ValueRank)));
                    names.Add(ToLowerCamelCase(argument.Name));
                }
            }
            
            // calculate field length.
            int length = 0;

            for (int ii = 0; ii < types.Count; ii++)
            {
                if (length < types[ii].Length)
                {
                    length = types[ii].Length;
                }
            }

            length++;
           
            // write parameters.
            for (int ii = 0; ii < types.Count; ii++)
            {
                if (ii > 0)
                {
                    template.Write(",");
                }

                template.WriteNextLine(context.Prefix);
                template.Write("    {0}", types[ii]);
                template.Write("{0}", new string(' ', length - types[ii].Length));
                template.Write("{0}", names[ii]);
            }
            
            template.Write(")");
            
            return null;
        }

        /// <summary>
        /// Writes the code to copy the input arguments.
        /// </summary>
        private string LoadTemplate_CopyInputArguments(Template template, Context context)
        {
            Parameter argument = context.Target as Parameter;

            if (argument == null)
            {
                return null;
            }                       

            template.WriteNextLine(context.Prefix);
            template.Write("inputArguments.Add({0});", ToLowerCamelCase(argument.Name));
    
            return null;
        }
        
        /// <summary>
        /// Writes the code to copy the output arguments.
        /// </summary>
        private string LoadTemplate_CopyOutputArguments(Template template, Context context)
        {
            MethodDesign method = context.Target as MethodDesign;

            if (method == null)
            {
                return null;
            }                       

            if (method.OutputArguments != null)
            {
                for (int ii = 1; ii < method.OutputArguments.Length; ii++)
                {
                    Parameter argument = method.OutputArguments[ii];
                    string typeName = GetVariableValueDataType(argument.DataTypeNode, argument.ValueRank);

                    template.WriteNextLine(context.Prefix);
                    template.Write("{2} = ({0})outputArguments[{1}];", typeName, context.Index, ToLowerCamelCase(argument.Name));
                }

                if (method.OutputArguments.Length > 1)
                {
                    template.WriteNextLine(context.Prefix);
                }

                template.WriteNextLine(context.Prefix);
                template.Write("return ({0})outputArguments[0];", GetVariableValueDataType(method.OutputArguments[0].DataTypeNode, method.OutputArguments[0].ValueRank));
            }
    
            return null;
        }
            
        /// <summary>
        /// Writes the code to copy the output arguments.
        /// </summary>
        private string LoadTemplate_AssignInputArguments(Template template, Context context)
        {
            Parameter argument = context.Target as Parameter;

            if (argument == null)
            {
                return null;
            }                       
            
            string typeName = GetVariableValueDataType(argument.DataTypeNode, argument.ValueRank);
            
            template.WriteNextLine(context.Prefix);
            template.Write("{0} ", typeName);
            template.Write("{0} = ", ToLowerCamelCase(argument.Name));
            template.Write("({0})inputArguments[{1}];", typeName, context.Index);
    
            return null;
        }

        /// <summary>
        /// Writes the code to declare the output arguments.
        /// </summary>
        private string LoadTemplate_DeclareOutputArguments(Template template, Context context)
        {
            Parameter argument = context.Target as Parameter;

            if (argument == null)
            {
                return null;
            }                           
                        
            template.WriteNextLine(context.Prefix);
            template.Write("{0} ", GetVariableValueDataType(argument.DataTypeNode, argument.ValueRank));
            template.Write("{0} = ", ToLowerCamelCase(argument.Name));
            template.Write("{0};", GetVariableValueInitializer(argument.DataTypeNode, argument.ValueRank));
                        
            return null;
        }
        
        /// <summary>
        /// Writes the code to invoke a method callback.
        /// </summary>
        private string LoadTemplate_InvokeMethodCallback(Template template, Context context)
        {
            MethodDesign method = context.Target as MethodDesign;

            if (method == null)
            {
                return null;
            }                           
            
            string methodName = String.Format("{0}Method.Call", method.SymbolicName.Name);
            string targetName = "this";            
            bool returnRequired = false;
            
            if (context.Token.EndsWith("InvokeCallback"))
            {
                methodName = "Callback";
                targetName = "target";
            }
            else
            {
                returnRequired = true;
                
                if (context.Token.Contains("m_TypeDefinition"))
                {
                    methodName = String.Format("m_TypeDefinition.{0}", methodName);
                }
            }
            
            int inputCount = (method.InputArguments != null)?method.InputArguments.Length:0;
            int outputCount = (method.OutputArguments != null)?method.OutputArguments.Length:0;
            
            // handle the return type.        
            template.WriteNextLine(context.Prefix);

            if (outputCount > 0)
            {
                if (returnRequired)
                {
                    template.Write("return ");
                }
                else
                {
                    Parameter argument = method.OutputArguments[0];
                    template.Write("{0} = ", ToLowerCamelCase(argument.Name));
                }
            }

            // treat methods with few arguments as a special case.
            if (inputCount <= 1 && outputCount <= 1)
            {
                template.Write("{0}(context, {1}", methodName, targetName);
                
                if (inputCount > 0)
                {                    
                    Parameter argument = method.InputArguments[0];
                    template.Write(", ");
                    template.Write(ToLowerCamelCase(argument.Name));
                }
                
                template.Write(");");                    
                return null;
            }
            
            template.Write("{0}(", methodName);
            template.WriteNextLine(context.Prefix);
            template.Write("    context,");
            template.WriteNextLine(context.Prefix);
            template.Write("    {0}", targetName);
    
            if (method.InputArguments != null)
            {
                foreach (Parameter argument in method.InputArguments)
                {
                    template.Write(",");
                    template.WriteNextLine(context.Prefix);
                    template.Write("    {0}", ToLowerCamelCase(argument.Name));
                }
            }

            if (method.OutputArguments != null)
            {
                for (int ii = 1; ii < method.OutputArguments.Length; ii++)
                {
                    Parameter argument = method.OutputArguments[ii];
                    template.Write(",");
                    template.WriteNextLine(context.Prefix);
                    template.Write("    out {0}", ToLowerCamelCase(argument.Name));
                }
            }
            
            template.Write(");");
           
            return null;
        }

        /// <summary>
        /// Adds the output arguments to the list to return.
        /// </summary>
        private string LoadTemplate_AssignOutputArguments(Template template, Context context)
        {
            Parameter argument = context.Target as Parameter;

            if (argument == null)
            {
                return null;
            }                     
      
            if (context.FirstInList)
            {
                template.WriteNextLine(context.Prefix);
            }
            
            template.WriteNextLine(context.Prefix);
            template.Write("outputArguments.Add({0});", ToLowerCamelCase(argument.Name));
    
            return null;
        }

        /// <summary>
        /// Writes the code to create a child.
        /// </summary>
        private bool WriteTemplate_CodeMethodArgument(Template template, Context context)
        {
            Parameter argument = context.Target as Parameter;

            if (argument == null)
            {
                return false;
            }
                           
            template.AddReplacement("_BrowseName_", argument.Name);  
            template.AddReplacement("_DataType_", GetDataType(argument.DataTypeNode));    
            template.AddReplacement("_ValueRank_", GetValueRank(argument.ValueRank));   
            template.AddReplacement("_Description_", argument.Description.Value);    
            template.AddReplacement("_DescriptionKey_", argument.Description.Key);   
            template.AddReplacement("_DefaultLocale_", m_model.DefaultLocale);  
            template.AddReplacement("_Direction_", (context.Token.Contains("Input"))?"Input":"Output");  
    
            return template.WriteTemplate(context);
        }
        
        #region Common Functions
        #region ChildMasks Enumeration
        /// <summary>
        /// Masks used to search for children.
        /// </summary>
        [Flags]
        private enum ChildMasks
        {
            Private = 0x001,
            Required = 0x002,
            Optional = 0x004,
            Shared = 0x008,
            Replaceable = 0x010,
            Objects = 0x020,
            Variables = 0x040,
            Properties = 0x080,
            Methods = 0x100,
            Redefinition = 0x200,
            NoRedefinition = 0x400,
            AllTypes = ChildMasks.Objects | ChildMasks.Variables | ChildMasks.Properties | ChildMasks.Methods,
            NoPrivate = ChildMasks.Required | ChildMasks.Optional | ChildMasks.Shared | ChildMasks.Replaceable,
            InstanceOnly = ChildMasks.Required | ChildMasks.Optional |ChildMasks.Replaceable, 
            AllRules = ChildMasks.NoPrivate | ChildMasks.Private
        }
        #endregion
                
        /// <summary>
        /// Returns all nested children of a node.
        /// </summary>
        private IList<NodeDesign> GetNestedChildren(NodeDesign node, ChildMasks masks)
        {
            List<NodeDesign> compositeList = new List<NodeDesign>();

            IList<NodeDesign> children = GetChildren(node, masks);
            compositeList.AddRange(children);

            foreach (NodeDesign child in children)
            {
                IList<NodeDesign> nestedChildren = GetNestedChildren(child, masks);
                compositeList.AddRange(nestedChildren);
            }

            return compositeList;
        }

        /// <summary>
        /// Returns the children for an instance.
        /// </summary>
        private IList<NodeDesign> GetChildren(NodeDesign node, ChildMasks masks)
        {
            List<NodeDesign> children = new List<NodeDesign>();

            if (node.Children != null && node.Children.Items != null)
            {
                foreach (InstanceDesign child in node.Children.Items)
                {
                    if (((masks & ChildMasks.Objects) == 0) && child is ObjectDesign)
                    {
                        continue;
                    }
                    
                    if (((masks & ChildMasks.Variables) == 0) && child is VariableDesign)
                    {
                        if (((masks & ChildMasks.Properties) == 0))
                        {
                            continue;
                        }
                    }

                    if (((masks & ChildMasks.Properties) == 0) && child is PropertyDesign)
                    {
                        if (((masks & ChildMasks.Variables) == 0))
                        {
                            continue;
                        }
                    }
                    
                    if (((masks & ChildMasks.Methods) == 0) && child is MethodDesign)
                    {
                        continue;
                    }

                    
                    ModellingRule modellingRule = child.ModellingRule;

                    if (((masks & ChildMasks.Required) == 0) && modellingRule == ModellingRule.Mandatory)
                    {
                        continue;
                    }

                    if (((masks & ChildMasks.Optional) == 0) && modellingRule == ModellingRule.Optional)
                    {
                        continue;
                    }

                    if (((masks & ChildMasks.Shared) == 0) && modellingRule == ModellingRule.MandatoryShared)
                    {
                        continue;
                    }                    

                    if (((masks & ChildMasks.Private) == 0) && modellingRule == ModellingRule.None)
                    {
                        continue;
                    }
                    
                    if ((masks & ChildMasks.Redefinition) != 0 && child.InstanceDeclarationNode == null)
                    {
                        continue;
                    }
                    
                    if ((masks & ChildMasks.NoRedefinition) != 0 && child.InstanceDeclarationNode != null)
                    {
                        continue;
                    }
                    
                    children.Add(child);
                }
            }

            return children;
        }
              
        /// <summary>
        /// Returns the references for the children for an instance.
        /// </summary>
        private IList<Reference> GetReferences(NodeDesign node, ChildMasks masks)
        {
            List<Reference> references = new List<Reference>(); 
            
            if (node.References != null)
            {
                references.AddRange(node.References);
            }

            foreach (InstanceDesign child in GetChildren(node, masks))
            {
                if (child.References != null)
                {
                    references.AddRange(child.References);
                }
            }

            return references;
        }
        
        /// <summary>
        /// Returns true if an ObjectType class definition is required.
        /// </summary>
        private bool TypeDefinitionRequired(NodeDesign node)
        {
            TypeDesign type = node as TypeDesign;

            if (type != null)
            {
                if ((type is ObjectTypeDesign || type is VariableTypeDesign) && !type.HasChildren)
                {
                    return false;
                }

                return true;
            }

            InstanceDesign instance = node as InstanceDesign;

            if (instance != null)
            {
                IList<NodeDesign> children = GetChildren(
                    instance, 
                    ChildMasks.NoRedefinition | ChildMasks.AllTypes | ChildMasks.InstanceOnly);

                if (children.Count == 0)
                {
                    return false;
                }               

                return true;
            }
                
            return false;
        }

        /// <summary>
        /// Returns the browse names of all children defined for the node (includes children from base types).
        /// </summary>
        private List<NodeDesign> GetChildrenFromType(TypeDesign type, ChildMasks masks)
        {
            List<NodeDesign> children = new List<NodeDesign>();

            while (type != null)
            {
                children.AddRange(GetChildren(type, masks));
                type = type.BaseTypeNode;
            }

            return children;
        }

        /// <summary>
        /// Returns the NodeId for a child.
        /// </summary>
        private string GetNodeId(NodeDesign child)
        {            
            if (child is TypeDesign)
            {
                string nodeClass = "ObjectTypes";

                if (child is VariableTypeDesign)
                {
                    nodeClass = "VariableTypes";
                }

                return String.Format(
                    "Opc.Ua.NodeId.Create({1}.{3}.{0}, {2}, Server.NamespaceUris)", 
                    child.SymbolicName.Name, 
                    GetNamespaceCodePath(child.SymbolicId.Namespace, false), 
                    GetConstantForNamespace(child.SymbolicId.Namespace),
                    nodeClass);
            }

            return GetNode(child) + ".NodeId";
        }               

        /// <summary>
        /// Returns the Node for a child.
        /// </summary>
        private string GetNode(NodeDesign child)
        {            
            InstanceDesign instance = child as InstanceDesign;

            if (instance != null)
            {
                string path = GetPropertyName(instance);

                while (child.Parent is InstanceDesign)
                {
                    path = String.Format("{0}.{1}", GetPropertyName((InstanceDesign)child.Parent), path);
                    child = child.Parent;
                }

                return path;
            }
            
            if (child is ObjectTypeDesign || child is VariableTypeDesign)
            {
                return "this";
            }

            return child.SymbolicId.Name;
        }
               
        /// <summary>
        /// Returns the relative path.
        /// </summary>
        private string GetRelativePath(string browsePath)
        {
            if (String.IsNullOrEmpty(browsePath))
            {
                return "null";
            }

            return String.Format("RelativePath.Parse(\"{0}\", namespaceUris, Server.NamespaceUris)", browsePath);
        }

        /// <summary>
        /// Returns a list of nodes to process.
        /// </summary>
        private List<NodeDesign> GetNodeList()
        {
            List<NodeDesign> nodes = new List<NodeDesign>();

            foreach (NodeDesign node in m_model.Items)
            {
                if (!node.IsDeclaration)
                {
                    nodes.Add(node);
                }
            }

            return nodes;
        }

        /// <summary>
        /// Returns a constant for the namespace uri.
        /// </summary>
        private string GetConstantForNamespace(string namespaceUri)
        {
            if (m_model.Namespaces != null)
            {
                foreach (Namespace ns in m_model.Namespaces)
                {
                    if (ns.Value == namespaceUri)
                    {
                        return String.Format("{1}.Namespaces.{0}", ns.Name, ns.Prefix);
                    }
                }
            }

            return null;            
        }

        /// <summary>
        /// Returns a constant for the namespace uri.
        /// </summary>
        private string GetNameForNamespace(string namespaceUri)
        {
            if (m_model.Namespaces != null)
            {
                foreach (Namespace ns in m_model.Namespaces)
                {
                    if (ns.Value == namespaceUri)
                    {
                        return String.Format("{0}", ns.Name);
                    }
                }
            }

            return null;            
        }

        /// <summary>
        /// Returns a qualifier for the namespace to use in code.
        /// </summary>
        private int GetNamespaceIndex(string namespaceUri)
        {
            if (m_model.Namespaces != null)
            {
                for (int ii = 0; ii < m_model.Namespaces.Length; ii++)
                {
                    if (m_model.Namespaces[ii].Value == namespaceUri)
                    {
                        return ii;
                    }
                }
            }

            return 0;            
        }

        /// <summary>
        /// Returns a qualifier for the namespace to use in code.
        /// </summary>
        private string GetNamespaceCodePath(string namespaceUri, bool useInternal)
        {
            if (m_model.Namespaces != null)
            {
                foreach (Namespace ns in m_model.Namespaces)
                {
                    if (ns.Value == namespaceUri)
                    {
                        if (useInternal && !String.IsNullOrEmpty(ns.InternalPrefix))
                        {
                            return String.Format("{0}", ns.InternalPrefix);
                        }
                        else
                        {
                            return String.Format("{0}", ns.Prefix);
                        }
                    }
                }
            }

            return null;            
        }

        /// <summary>
        /// Returns the name of a property.
        /// </summary>
        private string GetPropertyName(InstanceDesign child)
        {
            string propertyName = child.SymbolicName.Name;

            if (child is MethodDesign)
            {
                propertyName += "Method";
            }

            return propertyName;
        }        

        /// <summary>
        /// Returns the name of a field.
        /// </summary>
        private string GetFieldName(Parameter field)
        {
            return String.Format("m_{0}", ToLowerCamelCase(field.Name));
        }        

        /// <summary>
        /// Returns the name of a field.
        /// </summary>
        private string GetFieldName(InstanceDesign child)
        {
            string fieldName = String.Format("m_{0}", ToLowerCamelCase(child.SymbolicName.Name));

            if (child is MethodDesign)
            {
                fieldName += "Method";
            }

            return fieldName;
        }        
        
        /// <summary>
        /// Returns the NodeClass of a Node
        /// </summary>
        private string GetNodeClass(NodeDesign node)
        {
            if (node is VariableDesign)
            {
                return "Variable";
            }

            if (node is VariableTypeDesign)
            {
                return "VariableType";
            }

            if (node is ObjectDesign)
            {
                return "Object";
            }

            if (node is ObjectTypeDesign)
            {
                return "ObjectType";
            }

            if (node is ReferenceTypeDesign)
            {
                return "ReferenceType";
            }

            if (node is DataTypeDesign)
            {
                return "DataType";
            }

            if (node is MethodDesign)
            {
                return "Method";
            }

            return "Node";
        }        
        
        /// <summary>
        /// Recursively search for the containing class name.
        /// </summary>
        private string GetContainingClassName(NodeDesign child)
        {
            TypeDesign type = child.Parent as TypeDesign;

            if (type != null)
            {
                return GetStaticClassName(type);
            }

            string className = GetChildClassName(child.Parent as InstanceDesign);

            if (className.EndsWith("<T>"))
            {
                return String.Format("{0}<object>", className.Substring(0, className.Length-"<T>".Length));
            }

            return className;
        }
        
        /// <summary>
        /// Returns a name qualified with a namespace prefix.
        /// </summary>
        protected string GetChildClassName(InstanceDesign child)
        {
            // no type name for methods
            if (child is MethodDesign)
            {
                if (child.TypeDefinition != null)
                {
                    MethodDesign methodType = m_validator.FindType(child.TypeDefinition) as MethodDesign;

                    if (methodType != null)
                    {
                        return methodType.SymbolicName.Name + "MethodSource";
                    }
                }

                return child.SymbolicName.Name + "MethodSource";
            }

            string typeName = child.TypeDefinition.Name;
            
            VariableDesign variable = child as VariableDesign;

            if (variable != null)
            {
                VariableTypeDesign variableType = variable.TypeDefinitionNode as VariableTypeDesign;

                while (variableType != null && variableType.NoClassGeneration)
                {
                    variableType = variableType.BaseTypeNode as VariableTypeDesign;
                }

                if (variableType != null)
                {
                    string templateType = GetTemplateParameter(variableType);

                    if (templateType == "T")
                    {
                        return String.Format("{0}<{1}>", variableType.ClassName, GetVariableValueDataType(variable.DataTypeNode, variable.ValueRank)); 
                    }

                    return variableType.ClassName;
                }
            }
            
            ObjectDesign objectd = child as ObjectDesign;

            if (objectd != null)
            {
                ObjectTypeDesign objectType = objectd.TypeDefinitionNode as ObjectTypeDesign;
                
                while (objectType != null && objectType.NoClassGeneration)
                {
                    objectType = objectType.BaseTypeNode as ObjectTypeDesign;
                }

                if (objectType != null)
                {
                    return objectType.ClassName;
                }
            }
            
            if (typeName.EndsWith("Type"))
            {
                typeName = typeName.Substring(0, typeName.Length-4);
            }

            return typeName;
        }

        /// <summary>
        /// Returns a symbol for a modelling rule.
        /// </summary>
        private string GetModellingRule(ModellingRule modellingRule)
        {
            if (modellingRule == ModellingRule.None)
            {
                return "null";
            }
            
            return String.Format("Opc.Ua.Objects.ModellingRule_{0}", modellingRule);
        }        

        /// <summary>
        /// Returns a symbol for a reference type.
        /// </summary>
        private string GetReferenceType(XmlQualifiedName referenceType)
        {
            if (referenceType.Namespace == DefaultNamespace)
            {
                return String.Format("Opc.Ua.ReferenceTypes.{0}", referenceType.Name);
            }
            
            return String.Format(
                "Opc.Ua.NodeId.Create({1}.ReferenceTypes.{0}, {2}, Server.NamespaceUris)", 
                referenceType.Name, 
                GetNamespaceCodePath(referenceType.Namespace, false),
                GetConstantForNamespace(referenceType.Namespace));
        }        

        /// <summary>
        /// Returns the template parameter to use with a variable type.
        /// </summary>
        private string GetTemplateParameter(VariableTypeDesign variableType)
        {
            string typeName = "T";

            if (variableType == null)
            {
                return typeName;
            }
            
            // check if type defines a specific datatype.
            if (variableType.DataType != new XmlQualifiedName("BaseDataType", DefaultNamespace))
            {
                typeName = GetVariableValueDataType(variableType.DataTypeNode, variableType.ValueRank);
            }
                        
            // nothing more to do if base type is BaseDataVariable
            if (variableType.BaseTypeNode == null || variableType.BaseTypeNode.ClassName == "VariableSource")
            {
                return typeName;
            }
            
            VariableTypeDesign baseType = variableType.BaseTypeNode as VariableTypeDesign;
            
            // cannot redefine datatype if base type defines one explicitly 
            if (baseType.DataType != new XmlQualifiedName("BaseDataType", DefaultNamespace))
            {
                return null;
            }
                
            return typeName;
        }
        
        /// <summary>
        /// Returns a symbol for a object type.
        /// </summary>
        private string GetBaseTypeNodeId(NodeDesign node)
        {
            TypeDesign type = node as TypeDesign;

            if (type != null)
            {
                return GetTypeNodeId(type.BaseTypeNode);
            }

            return null;
        }
        
        
        /// <summary>
        /// Returns a symbol for a object type.
        /// </summary>
        private string GetTypeNodeId(NodeDesign node)
        {
            TypeDesign type = node as TypeDesign;

            if (type == null)
            {
                return "Opc.Ua.ObjectTypes.BaseObjectType";
            }
            
            string nodeType = null;

            if (node is ObjectTypeDesign) nodeType = "ObjectTypes";
            else if (node is VariableTypeDesign) nodeType = "VariableTypes";
            else if (node is DataTypeDesign) nodeType = "DataTypes"; 
            else if (node is ReferenceTypeDesign) nodeType = "ReferenceTypes"; 

            if (type.SymbolicId.Namespace == DefaultNamespace)
            {                                
                return String.Format("Opc.Ua.{0}.{1}", nodeType, type.SymbolicName.Name);
            }

            return String.Format(
                "Opc.Ua.NodeId.Create({1}.{3}.{0}, {2}, Server.NamespaceUris)", 
                type.SymbolicName.Name, 
                GetNamespaceCodePath(type.SymbolicName.Namespace, false), 
                GetConstantForNamespace(type.SymbolicName.Namespace),
                nodeType);
        }

        /// <summary>
        /// Returns a symbol for a type definition.
        /// </summary>
        private string GetTypeDefinition(NodeDesign node)
        {
            XmlQualifiedName typeDefinition = null;
            string nodeType = null;

            if (node is ObjectDesign)
            {
                typeDefinition = ((ObjectDesign)node).TypeDefinition;
                nodeType = "ObjectTypes";
            }
            
            else if (node is VariableDesign)
            {
                typeDefinition = ((VariableDesign)node).TypeDefinition;
                nodeType = "VariableTypes";
            }

            else
            {
                return null;
            }
                        
            if (typeDefinition.Namespace == DefaultNamespace)
            {
                return String.Format("Opc.Ua.{0}.{1}", nodeType, typeDefinition.Name);
            }
            
            return String.Format(
                "Opc.Ua.NodeId.Create({1}.{3}.{0}, {2}, Server.NamespaceUris)", 
                typeDefinition.Name, 
                GetNamespaceCodePath(typeDefinition.Namespace, false), 
                GetConstantForNamespace(typeDefinition.Namespace),
                nodeType);
        }

        /// <summary>
        /// Returns a symbol for a data type.
        /// </summary>
        private string GetDataType(DataTypeDesign datatype)
        {                        
            if (datatype.SymbolicId.Namespace == DefaultNamespace)
            {
                return String.Format("Opc.Ua.DataTypes.{0}", datatype.SymbolicName.Name);
            }
            
            return String.Format(
                "Opc.Ua.NodeId.Create({1}.DataTypes.{0}, {2}, Server.NamespaceUris)", 
                datatype.SymbolicName.Name, 
                GetNamespaceCodePath(datatype.SymbolicId.Namespace, false), 
                GetConstantForNamespace(datatype.SymbolicId.Namespace));
        }
        
        /// <summary>
        /// Maps the array size enumeration onto a string.
        /// </summary>
        private string GetValueRank(ValueRank valueRank)
        {
            switch (valueRank)
            {
                case ValueRank.Scalar:        return "ValueRanks.Scalar";
                case ValueRank.Array:         return "ValueRanks.OneDimension";
                case ValueRank.ScalarOrArray: return "ValueRanks.ScalarOrArray";
            }

            return "ValueRanks.Scalar";
        }

        /// <summary>
        /// Maps the access level enumeration onto a string.
        /// </summary>
        private string GetAccessLevel(AccessLevel accessLevel)
        {
            switch (accessLevel)
            {
                case AccessLevel.Read:      return "AccessLevels.CurrentRead";
                case AccessLevel.Write:     return "AccessLevels.CurrentWrite";
                case AccessLevel.ReadWrite: return "AccessLevels.CurrentReadOrWrite";
            }

            return "AccessLevels.None";
        }

        /// <summary>
        /// Maps the MinimumSamplingInterval onto a constant.
        /// </summary>
        private string GetMinimumSamplingInterval(int minimumSamplingInterval)
        {
            switch (minimumSamplingInterval)
            {
                case -1: return "MinimumSamplingIntervals.Indeterminate";
                case 0:  return "MinimumSamplingIntervals.Continuous";
            }

            return minimumSamplingInterval.ToString();
        }
                
        /// <summary>
        /// Returns the class name for an instance type.
        /// </summary>
        private string GetClassName(TypeDesign type)
        {
            VariableTypeDesign variableType = type as VariableTypeDesign;

            if (variableType != null)
            {
                string templateType = GetTemplateParameter(variableType);

                if (templateType == "T")
                {
                    return String.Format("{0}<{1}>", variableType.ClassName, templateType); 
                }
            }

            if (type != null)
            {
                return type.ClassName;
            }

            return null;
        }       
        
        /// <summary>
        /// Returns the class name for an instance type.
        /// </summary>
        private string GetStaticClassName(TypeDesign type)
        {
            VariableTypeDesign variableType = type as VariableTypeDesign;

            if (variableType != null)
            {
                string templateType = GetTemplateParameter(variableType);

                if (templateType == "T")
                {
                    return String.Format("{0}<object>", variableType.ClassName); 
                }
            }

            if (type != null)
            {
                return type.ClassName;
            }

            return null;
        }       
        
        /// <summary>
        /// Returns the class name for a type.
        /// </summary>
        private string GetClassTypeName(TypeDesign type)
        {
            VariableTypeDesign variableType = type as VariableTypeDesign;

            if (variableType != null)
            {
                string templateType = GetTemplateParameter(variableType);

                if (templateType == "T")
                {
                    return String.Format("{0}<{1}>", variableType.SymbolicName.Name, templateType); 
                }
            }

            return type.SymbolicName.Name;
        }       

        /// <summary>
        /// Returns the the name of the base type for an instance of a type.
        /// </summary>
        private string GetBaseClassName(TypeDesign type)
        {
            if (type.BaseTypeNode.NoClassGeneration)
            {
                return GetBaseClassName(type.BaseTypeNode);
            }

            VariableTypeDesign variableType = type as VariableTypeDesign;

            if (variableType != null)
            {
                string templateType = GetTemplateParameter(variableType);

                if (templateType != null)
                {
                    return String.Format("{0}<{1}>", variableType.BaseTypeNode.ClassName, templateType); 
                }
            }
            
            if (type.BaseTypeNode != null)
            {
                if (type.BaseTypeNode.ClassName == "BaseEvent")
                {
                    return "ObjectSource";
                }

                return type.BaseTypeNode.ClassName;
            }

            return null;
        }
        
        /// <summary>
        /// Returns a boolean value as text.
        /// </summary>
        private string GetBooleanValue(bool value)
        {
            return (value)?"true":"false";
        }
        
        /// <summary>
        /// Returns the default value for a variable type.
        /// </summary>
        private string GetVariableValueDefaultValue(VariableTypeDesign variableType)
        {
            return GetVariableValueDefaultValue(variableType.DataTypeNode, variableType.ValueRank, variableType.DecodedValue);
        }       
        
        /// <summary>
        /// Returns the default value for a variable.
        /// </summary>
        private string GetVariableValueDefaultValue(VariableDesign variable)
        {
            return GetVariableValueDefaultValue(variable.DataTypeNode, variable.ValueRank, variable.DecodedValue);
        }       

        /// <summary>
        /// Returns the default value for a variable.
        /// </summary>
        private string GetVariableValueDefaultValue(DataTypeDesign dataType, ValueRank valueRank, object defaultValue)
        {
            // defaults only supported for scalar values at this time.
            if (defaultValue == null)
            {
                return GetVariableValueInitializer(dataType, valueRank);
            }

            if (valueRank != ValueRank.Scalar)
            {
                throw new InvalidOperationException("Default values for arrays not supported.");
            }

            BasicDataType basicType = GetBasicDataType(dataType);

            if (!GetSystemType(basicType).IsInstanceOfType(defaultValue))
            {
                throw new InvalidOperationException("Default value does not match the data type. Expected: " + basicType.ToString());
            }
            
            switch (basicType)
            {
                case BasicDataType.Boolean: { return ((bool)defaultValue)?"true":"false"; }
                case BasicDataType.SByte: { return String.Format("(sbyte){0}", (sbyte)defaultValue); }
                case BasicDataType.Byte: { return String.Format("(byte){0}", (byte)defaultValue); }
                case BasicDataType.Int16: { return String.Format("(short){0}", (short)defaultValue); }
                case BasicDataType.UInt16: { return String.Format("(ushort){0}", (ushort)defaultValue); }
                case BasicDataType.Int32: { return String.Format("(int){0}", (int)defaultValue); }
                case BasicDataType.UInt32: { return String.Format("(uint){0}", (uint)defaultValue); }
                case BasicDataType.Int64: { return String.Format("(long){0}", (long)defaultValue); }
                case BasicDataType.UInt64: { return String.Format("(ulong){0}", (ulong)defaultValue); }
                case BasicDataType.Float: { return String.Format("(float){0}", (float)defaultValue); }
                case BasicDataType.Double: { return String.Format("(double){0}", (double)defaultValue); }
                
                case BasicDataType.String:            
                {
                    string s = (string)defaultValue;

                    if (String.IsNullOrEmpty(s))
                    {
                        return "String.Empty";
                    }
 
                    return String.Format("\"{0}\"", defaultValue); 
                }

                case BasicDataType.DateTime:             
                { 
                    DateTime dateTime = (DateTime)defaultValue;

                    if (dateTime == DateTime.MinValue)
                    {
                        return "DateTime.MinValue";
                    }

                    return String.Format("System.Xml.XmlConvert.ToDateTime(\"{0}\", XmlDateTimeSerializationMode.Utc)", XmlConvert.ToString(dateTime, XmlDateTimeSerializationMode.Utc)); 
                }

                case BasicDataType.Guid:                 
                { 
                    Uuid guid = (Uuid)defaultValue;

                    if (guid == Uuid.Empty)
                    {
                        return "Uuid.Empty";
                    }

                    return String.Format("new Uuid(\"{0}\")", guid); 
                }
		        
                case BasicDataType.ByteString:               
                { 
                    byte[] bytes = (byte[])defaultValue;

                    if (bytes == null)
                    {
                        return "(byte[])null";
                    }

                    return String.Format("Convert.FromBase64String(\"{0}\")", Convert.ToBase64String((byte[])defaultValue)); 
                }

                case BasicDataType.XmlElement: { return "(XmlElement)null"; }
                case BasicDataType.NodeId: { return "NodeId.Null"; }
                case BasicDataType.ExpandedNodeId: { return "ExpandedNodeId.Null"; }

                case BasicDataType.StatusCode:           
                { 
                    StatusCode code = (StatusCode)defaultValue;

                    string browseName = StatusCodes.GetBrowseName((uint)code);

                    if (String.IsNullOrEmpty(browseName))
                    {                        
                        return String.Format("(StatusCode){0}", code.Code);
                    }

                    return String.Format("(StatusCode)StatusCodes.{0}", browseName);
                }

                case BasicDataType.DiagnosticInfo: { return "(DiagnosticInfo)null"; }
                
                case BasicDataType.QualifiedName:        
                { 
                    QualifiedName qname = (QualifiedName)defaultValue;

                    if (QualifiedName.IsNull(qname))
                    {
                        return "QualifiedName.Null";
                    }

                    return String.Format("new QualifiedName(\"{0}\", {1})", qname.Name, qname.NamespaceIndex); 
                }

                case BasicDataType.LocalizedText: 
                { 
                    Opc.Ua.LocalizedText text = (Opc.Ua.LocalizedText)defaultValue;

                    if (Opc.Ua.LocalizedText.IsNullOrEmpty(text))
                    {
                        return "LocalizedText.Null";
                    }

                    return String.Format("new LocalizedText(\"{0}\", \"{1}\")", text.Locale, text.Text); 
                }
                    
                case BasicDataType.Number: { return String.Format("(double){0}", defaultValue); }
                case BasicDataType.Integer: { return String.Format("(long){0}", defaultValue); }
                case BasicDataType.UInteger: { return String.Format("(ulong){0}", defaultValue); }
 
                default:
                case BasicDataType.DataValue:
                case BasicDataType.BaseDataType:
                case BasicDataType.Enumeration:
                case BasicDataType.Structure:
                {
                    throw new InvalidOperationException("Default values for type not supported: " + basicType.ToString());
                }
            }
        }

        /// <summary>
        /// Returns the initializer to use for the value of a variable or the argument of a method.
        /// </summary>
        private string GetVariableValueInitializer(DataTypeDesign dataType, ValueRank valueRank)
        {
            if (valueRank != ValueRank.Scalar)
            {
                return String.Format("({0}[])null", GetVariableValueDataType(dataType, ValueRank.Scalar));
            }

            switch (GetBasicDataType(dataType))
            {
                case BasicDataType.Boolean: { return "false"; }
                case BasicDataType.SByte: { return "(sbyte)0"; }
                case BasicDataType.Byte: { return "(byte)0"; }
                case BasicDataType.Int16: { return "(short)0"; }
                case BasicDataType.UInt16: { return "(ushort)0"; }
                case BasicDataType.Int32: { return "(int)0"; }
                case BasicDataType.UInt32: { return "(uint)0"; }
                case BasicDataType.Int64: { return "(long)0"; }
                case BasicDataType.UInt64: { return "(ulong)0"; }
                case BasicDataType.Float: { return "(float)0"; }
                case BasicDataType.Double: { return "(double)0"; }
                case BasicDataType.String: { return "(string)null"; }
                case BasicDataType.DateTime: { return "DateTime.MinValue"; }
                case BasicDataType.Guid: { return "Guid.Empty"; }
                case BasicDataType.ByteString: { return "(byte[])null"; }
                case BasicDataType.XmlElement: { return "(XmlElement)null"; }
                case BasicDataType.NodeId: { return "NodeId.Null"; }
                case BasicDataType.ExpandedNodeId: { return "ExpandedNodeId.Null"; }
                case BasicDataType.StatusCode: { return "(StatusCode)StatusCodes.Good"; }
                case BasicDataType.DiagnosticInfo: { return "(DiagnosticInfo)null"; }
                case BasicDataType.QualifiedName: { return "QualifiedName.Null"; }
                case BasicDataType.LocalizedText: { return "LocalizedText.Null"; }
                case BasicDataType.Number: { return "(double)0"; }
                case BasicDataType.Integer: { return "(long)0"; }
                case BasicDataType.UInteger: { return "(ulong)0"; }
                case BasicDataType.DataValue: { return "null"; }
                case BasicDataType.BaseDataType: { return "null"; }
                    
                case BasicDataType.Enumeration:
                {
                    if (!dataType.HasFields)
                    {
                        throw new InvalidOperationException("Must define a default value for all imported enumerations: " + dataType.SymbolicName.Name);
                    }

                    return String.Format(
                        "{2}.{0}.{1}", 
                        dataType.SymbolicName.Name, 
                        dataType.Fields[0].Name, 
                        GetNamespaceCodePath(dataType.SymbolicName.Namespace, false));
                }

                case BasicDataType.Structure:
                {
                    if (dataType.SymbolicName == new XmlQualifiedName("Structure", DefaultNamespace))
                    {
                        return "(IEncodeable)null";
                    }

                    return String.Format("({0})null", dataType.SymbolicName.Name);
                }
            }

            return "null";
        }
         
        /// <summary>
        /// Returns the initializer to use for the value of a data type field.
        /// </summary>
        private string GetDataTypeFieldInitializer(DataTypeDesign dataType, ValueRank valueRank)
        {
            if (valueRank != ValueRank.Scalar)
            {
                return String.Format("new {0}()", GetDataTypeFieldDataType(dataType, valueRank));
            }

            switch (GetBasicDataType(dataType))
            {
                case BasicDataType.Boolean: { return "false"; }
                case BasicDataType.SByte: { return "(sbyte)0"; }
                case BasicDataType.Byte: { return "(byte)0"; }
                case BasicDataType.Int16: { return "(short)0"; }
                case BasicDataType.UInt16: { return "(ushort)0"; }
                case BasicDataType.Int32: { return "(int)0"; }
                case BasicDataType.UInt32: { return "(uint)0"; }
                case BasicDataType.Int64: { return "(long)0"; }
                case BasicDataType.UInt64: { return "(ulong)0"; }
                case BasicDataType.Float: { return "(float)0"; }
                case BasicDataType.Double: { return "(double)0"; }
                case BasicDataType.String: { return "(string)null"; }
                case BasicDataType.DateTime: { return "DateTime.MinValue"; }
                case BasicDataType.Guid: { return "Uuid.Empty"; }
                case BasicDataType.ByteString: { return "(byte[])null"; }
                case BasicDataType.XmlElement: { return "(XmlElement)null"; }
                case BasicDataType.NodeId: { return "NodeId.Null"; }
                case BasicDataType.ExpandedNodeId: { return "ExpandedNodeId.Null"; }
                case BasicDataType.StatusCode: { return "(StatusCode)StatusCodes.Good"; }
                case BasicDataType.DiagnosticInfo: { return "new DiagnosticInfo()"; }
                case BasicDataType.QualifiedName: { return "QualifiedName.Null"; }
                case BasicDataType.LocalizedText: { return "LocalizedText.Null"; }
                case BasicDataType.DataValue: { return "(DataValue)null"; }
                case BasicDataType.Number: { return "(double)0"; }
                case BasicDataType.Integer: { return "(long)0"; }
                case BasicDataType.UInteger: { return "(ulong)0"; }
                case BasicDataType.BaseDataType: { return "Variant.Null"; }
                    
                case BasicDataType.Enumeration:
                {
                    return String.Format(
                        "{2}.{0}.{1}", 
                        dataType.SymbolicName.Name, 
                        dataType.Fields[0].Name, 
                        GetNamespaceCodePath(dataType.SymbolicName.Namespace, false));
                }

                case BasicDataType.Structure:
                {
                    if (dataType.SymbolicName == new XmlQualifiedName("Structure", DefaultNamespace))
                    {
                        return "(ExtensionObject)null";
                    }

                    return String.Format("new {0}()", dataType.SymbolicName.Name);
                }
            }

            return "Variant.Null";
        }

        /// <summary>
        /// Returns the data type to use for the value of a variable or the argument of a method.
        /// </summary>
        private string GetVariableValueDataType(DataTypeDesign dataType, ValueRank valueRank)
        {
            if (valueRank != ValueRank.Scalar)
            {
                return String.Format("{0}[]", GetVariableValueDataType(dataType, ValueRank.Scalar));
            }

            switch (GetBasicDataType(dataType))
            {
                case BasicDataType.Boolean: { return "bool"; }
                case BasicDataType.SByte: { return "sbyte"; }
                case BasicDataType.Byte: { return "byte"; }
                case BasicDataType.Int16: { return "short"; }
                case BasicDataType.UInt16: { return "ushort"; }
                case BasicDataType.Int32: { return "int"; }
                case BasicDataType.UInt32: { return "uint"; }
                case BasicDataType.Int64: { return "long"; }
                case BasicDataType.UInt64: { return "ulong"; }
                case BasicDataType.Float: { return "float"; }
                case BasicDataType.Double: { return "double"; }
                case BasicDataType.String: { return "string"; }
                case BasicDataType.DateTime: { return "DateTime"; }
                case BasicDataType.Guid: { return "Guid"; }
                case BasicDataType.ByteString: { return "byte[]"; }
                case BasicDataType.XmlElement: { return "XmlElement"; }
                case BasicDataType.NodeId: { return "NodeId"; }
                case BasicDataType.ExpandedNodeId: { return "ExpandedNodeId"; }
                case BasicDataType.StatusCode: { return "StatusCode"; }
                case BasicDataType.DiagnosticInfo: { return "DiagnosticInfo"; }
                case BasicDataType.QualifiedName: { return "QualifiedName"; }
                case BasicDataType.LocalizedText: { return "LocalizedText"; }
                case BasicDataType.DataValue: { return "DataValue"; }
                case BasicDataType.Number: { return "double"; }
                case BasicDataType.Integer: { return "long"; }
                case BasicDataType.UInteger: { return "ulong"; }
                case BasicDataType.BaseDataType: { return "object"; }
                    
                case BasicDataType.Enumeration:
                case BasicDataType.Structure:
                {
                    if (dataType.SymbolicName == new XmlQualifiedName("Structure", DefaultNamespace))
                    {
                        return "ExtensionObject";
                    }

                    return dataType.SymbolicName.Name;
                }
            }

            return "object";
        }

        /// <summary>
        /// Returns the data type to use for the value of a datatype field.
        /// </summary>
        private string GetDataTypeFieldDataType(DataTypeDesign dataType, ValueRank valueRank)
        {
            BasicDataType basicType = GetBasicDataType(dataType);

            if (valueRank != ValueRank.Scalar)
            {
                if (dataType.SymbolicName == new XmlQualifiedName("Structure", DefaultNamespace))
                {
                    return "ExtensionObjectCollection";
                }
                
                switch (basicType)
                {
                    case BasicDataType.Guid: { return "UuidCollection"; }
                    case BasicDataType.Number: { return "VariantCollection"; }
                    case BasicDataType.Integer: { return "VariantCollection"; }
                    case BasicDataType.UInteger: { return "VariantCollection"; }
                    case BasicDataType.BaseDataType: { return "VariantCollection"; } 

                    case BasicDataType.Enumeration:
                    case BasicDataType.Structure:
                    {
                        return String.Format("{0}Collection", dataType.SymbolicName.Name);
                    }

                    default:
                    {
                        return String.Format("{0}Collection", basicType);
                    }
                }

            }

            switch (basicType)
            {
                case BasicDataType.Boolean: { return "bool"; }
                case BasicDataType.SByte: { return "sbyte"; }
                case BasicDataType.Byte: { return "byte"; }
                case BasicDataType.Int16: { return "short"; }
                case BasicDataType.UInt16: { return "ushort"; }
                case BasicDataType.Int32: { return "int"; }
                case BasicDataType.UInt32: { return "uint"; }
                case BasicDataType.Int64: { return "long"; }
                case BasicDataType.UInt64: { return "ulong"; }
                case BasicDataType.Float: { return "float"; }
                case BasicDataType.Double: { return "double"; }
                case BasicDataType.String: { return "string"; }
                case BasicDataType.DateTime: { return "DateTime"; }
                case BasicDataType.Guid: { return "Uuid"; }
                case BasicDataType.ByteString: { return "byte[]"; }
                case BasicDataType.XmlElement: { return "XmlElement"; }
                case BasicDataType.NodeId: { return "NodeId"; }
                case BasicDataType.ExpandedNodeId: { return "ExpandedNodeId"; }
                case BasicDataType.StatusCode: { return "StatusCode"; }
                case BasicDataType.DiagnosticInfo: { return "DiagnosticInfo"; }
                case BasicDataType.QualifiedName: { return "QualifiedName"; }
                case BasicDataType.LocalizedText: { return "LocalizedText"; }
                case BasicDataType.DataValue: { return "DataValue"; }
                case BasicDataType.Number: { return "Variant"; }
                case BasicDataType.Integer: { return "Variant"; }
                case BasicDataType.UInteger: { return "Variant"; }
                case BasicDataType.BaseDataType: { return "Variant"; }
                    
                case BasicDataType.Enumeration:
                case BasicDataType.Structure:
                {
                    if (dataType.SymbolicName == new XmlQualifiedName("Structure", DefaultNamespace))
                    {
                        return "ExtensionObject";
                    }

                    return dataType.SymbolicName.Name;
                }
            }

            return "Variant";
        }
        
        /// <summary>
        /// Returns system type for a basic data type.
        /// </summary>
        private Type GetSystemType(BasicDataType basicType)
        {
            switch (basicType)
            {
                case BasicDataType.Boolean: { return typeof(bool); }
                case BasicDataType.SByte: { return typeof(sbyte); }
                case BasicDataType.Byte: { return typeof(byte); }
                case BasicDataType.Int16: { return typeof(short); }
                case BasicDataType.UInt16: { return typeof(ushort); }
                case BasicDataType.Int32: { return typeof(int); }
                case BasicDataType.UInt32: { return typeof(uint); }
                case BasicDataType.Int64: { return typeof(long); }
                case BasicDataType.UInt64: { return typeof(ulong); }
                case BasicDataType.Float: { return typeof(float); }
                case BasicDataType.Double: { return typeof(double); }
                case BasicDataType.String: { return typeof(string); }
                case BasicDataType.DateTime: { return typeof(DateTime); }
                case BasicDataType.Guid: { return typeof(Guid); }
                case BasicDataType.ByteString: { return typeof(byte[]); }
                case BasicDataType.XmlElement: { return typeof(XmlElement); }
                case BasicDataType.NodeId: { return typeof(NodeId); }
                case BasicDataType.ExpandedNodeId: { return typeof(ExpandedNodeId); }
                case BasicDataType.StatusCode: { return typeof(StatusCode); }
                case BasicDataType.DiagnosticInfo: { return typeof(DiagnosticInfo); }
                case BasicDataType.QualifiedName: { return typeof(QualifiedName); }
                case BasicDataType.LocalizedText: { return typeof(Opc.Ua.LocalizedText); }
                case BasicDataType.DataValue: { return typeof(DataValue); }
                case BasicDataType.Number: { return typeof(double); }
                case BasicDataType.Integer: { return typeof(long); }
                case BasicDataType.UInteger: { return typeof(ulong); }
                case BasicDataType.BaseDataType: { return typeof(object); }                    
                case BasicDataType.Enumeration: { return typeof(int); }
                case BasicDataType.Structure: { return typeof(ExtensionObject); }
            }

            return typeof(object);
        }
        
        /// <summary>
        /// Returns system type for a basic data type.
        /// </summary>
        private string GetSystemTypeName(DataTypeDesign datatype)
        {
            switch (datatype.BasicDataType)
            {
                case BasicDataType.Boolean: { return "bool"; }
                case BasicDataType.SByte: { return "sbyte"; }
                case BasicDataType.Byte: { return "byte"; }
                case BasicDataType.Int16: { return "short"; }
                case BasicDataType.UInt16: { return "ushort"; }
                case BasicDataType.Int32: { return "int"; }
                case BasicDataType.UInt32: { return "uint"; }
                case BasicDataType.Int64: { return "long"; }
                case BasicDataType.UInt64: { return "ulong"; }
                case BasicDataType.Float: { return "float"; }
                case BasicDataType.Double: { return "double"; }
                case BasicDataType.String: { return "string"; }
                case BasicDataType.DateTime: { return "DateTime"; }
                case BasicDataType.Guid: { return "Guid"; }
                case BasicDataType.ByteString: { return "byte[]"; }
                case BasicDataType.XmlElement: { return "XmlElement"; }
                case BasicDataType.NodeId: { return "NodeId"; }
                case BasicDataType.ExpandedNodeId: { return "ExpandedNodeId"; }
                case BasicDataType.StatusCode: { return "StatusCode"; }
                case BasicDataType.DiagnosticInfo: { return "DiagnosticInfo"; }
                case BasicDataType.QualifiedName: { return "QualifiedName"; }
                case BasicDataType.LocalizedText: { return "LocalizedText"; }
                case BasicDataType.DataValue: { return "DataValue"; }
                case BasicDataType.Number: { return "double"; }
                case BasicDataType.Integer: { return "long"; }
                case BasicDataType.UInteger: { return "ulong"; }
                case BasicDataType.BaseDataType: 
                { 
                    return "object";
                }
                    
                case BasicDataType.Structure:
                {
                    return "IEncodeable";
                }
                    
                case BasicDataType.UserDefined:
                case BasicDataType.Enumeration:
                {
                    return datatype.SymbolicName.Name;
                }
            }

            return "object";
        }

        /// <summary>
        /// Returns the basic type for a datatype.
        /// </summary>
        private BasicDataType GetBasicDataType(DataTypeDesign dataType)
        {
            if (dataType == null)
            {
                return BasicDataType.BaseDataType;
            }

            // recursively search hierarchy to find basic type.
            if (dataType.SymbolicName.Namespace != DefaultNamespace)
            {
                return GetBasicDataType(dataType.BaseTypeNode as DataTypeDesign);
            }

            // recursively search hierarchy if conversion to enum fails.
            foreach (string name in Enum.GetNames(typeof(BasicDataType)))
            {
                if (name == dataType.SymbolicName.Name)
                {
                    return (BasicDataType)Enum.Parse(typeof(BasicDataType), dataType.SymbolicName.Name);
                }
            }

            return GetBasicDataType(dataType.BaseTypeNode as DataTypeDesign);
        }
        #endregion        

        #region Private Fields
        private ModelCompilerValidator m_validator;
        private ModelDesign m_model;
        #endregion
    }
}
