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
using System.Collections.Generic;
using System.Text;
using System.IO;
using System.Reflection;
using System.Xml;

using Opc.Ua.CodeGenerator;

namespace Opc.Ua.ModelCompiler
{
    public partial class ModelGenerator
    {
        /// <summary>
        /// Generates a single file containing all of the classes.
        /// </summary>        
        public virtual void GenerateInternalSingleFile(string filePath)
        {
            // generate XML file with default values and references.
            WriteTemplate_XmlExport(filePath, GetNamespaceCodePath(m_model.TargetNamespace, false) + ".");

            // write type and object definitions.
            List<NodeDesign> nodes = GetNodeList();
            WriteTemplate_InternalSingleFile(filePath, nodes);
        }
        
        /// <summary>
        /// Generates a single file containing all of the classes.
        /// </summary>        
        public virtual void GenerateExternalSingleFile(string filePath, bool exportAll)
        {
            // generate XML file with default values and references.
            WriteTemplate_XmlSchema(filePath, GetNamespaceCodePath(m_model.TargetNamespace, false) + ".", exportAll);
            WriteTemplate_BinarySchema(filePath, GetNamespaceCodePath(m_model.TargetNamespace, false) + ".", exportAll);

            // write type and object definitions.
            IList<NodeDesign> nodes = GetNodes(String.Empty, m_model.TargetNamespace == DefaultNamespace);
            WriteTemplate_Documentation(filePath, nodes);
            WriteTemplate_ExternalSingleFile(filePath, nodes);
        }
        
        /// <summary>
        /// Creates a class that defines all types in the namespace.
        /// </summary>
        private void WriteTemplate_Documentation(string filePath, IList<NodeDesign> nodes)
        {            
			StreamWriter writer = new StreamWriter(String.Format(@"{0}\ModelDocumentation.html", filePath), false);

            try
            {
                Template template = new Template(writer, TemplatePath + "SingleFile.Documentation.html", Assembly.GetExecutingAssembly());
                
                template.AddReplacement("_Namespace_", GetNamespaceCodePath(m_model.TargetNamespace, true));
                template.AddReplacement("_ExternalNamespace_", GetNamespaceCodePath(m_model.TargetNamespace, false));
                template.AddReplacement("_NamespaceUri_", GetConstantForNamespace(m_model.TargetNamespace));                 
                template.AddReplacement("_DesignName_", GetNameForNamespace(m_model.TargetNamespace));                             
                
                SortedDictionary<string,List<NodeDesign>> identifiers = GetIdentifiers(nodes);

                AddTemplate(
                    template,
                    "// ListOfIdentifiers",
                    TemplatePath + "SingleFile.IdClass.html",
                    identifiers,
                    new LoadTemplateEventHandler(LoadTemplate_SingleFile_IdClass),
                    new WriteTemplateEventHandler(WriteTemplate_SingleFile_IdClass));
                                
                Context context = new Context();
                context.Target = nodes;
                template.WriteTemplate(context);       
            }
            finally
            {
                writer.Close();
            }
        }

        /// <summary>
        /// Creates a class that defines all types in the namespace.
        /// </summary>
        private void WriteTemplate_ExternalSingleFile(string filePath, IList<NodeDesign> nodes)
        {            
			StreamWriter writer = new StreamWriter(String.Format(@"{0}\ModelDeclarations.cs", filePath), false);

            try
            {
                Template template = new Template(writer, TemplatePath + "SingleFile.ExternalFile.cs", Assembly.GetExecutingAssembly());
                
                template.AddReplacement("_Namespace_", GetNamespaceCodePath(m_model.TargetNamespace, true));
                template.AddReplacement("_ExternalNamespace_", GetNamespaceCodePath(m_model.TargetNamespace, false));
                template.AddReplacement("_NamespaceUri_", GetConstantForNamespace(m_model.TargetNamespace));             
                template.AddReplacement("_DesignName_", GetNameForNamespace(m_model.TargetNamespace));         

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

                SortedDictionary<string,string> browseNames = GetBrowseNames(nodes);

                AddTemplate(
                    template,
                    "// ListOfBrowseNames",
                    TemplatePath + "SingleFile.BrowseName.cs",
                    browseNames,
                    new LoadTemplateEventHandler(LoadTemplate_SingleFile_BrowseNames),
                    new WriteTemplateEventHandler(WriteTemplate_SingleFile_BrowseNames));
                
                SortedDictionary<string,List<NodeDesign>> identifiers = GetIdentifiers(nodes);

                AddTemplate(
                    template,
                    "// ListOfIdentifiers",
                    TemplatePath + "SingleFile.IdClass.cs",
                    identifiers,
                    new LoadTemplateEventHandler(LoadTemplate_SingleFile_IdClass),
                    new WriteTemplateEventHandler(WriteTemplate_SingleFile_IdClass));

                // only include datatypes for the current dictionary.
                nodes = GetNodes(String.Empty, false);

                AddTemplate(
                    template,
                    "// ListOfDataTypes",
                    TemplatePath + "SingleFile.Type.cs",
                    nodes,
                    new LoadTemplateEventHandler(LoadTemplate_SingleFile_ListOfTypes),
                    new WriteTemplateEventHandler(WriteTemplate_SingleFile_ListOfTypes));
                
                Context context = new Context();
                context.Target = nodes;
                template.WriteTemplate(context);       
            }
            finally
            {
                writer.Close();
            }
        }
        
        /// <summary>
        /// Creates a class that defines all types in the namespace.
        /// </summary>
        private void WriteTemplate_InternalSingleFile(string filePath, List<NodeDesign> nodes)
        {            
			StreamWriter writer = new StreamWriter(String.Format(@"{0}\ModelImplementation.cs", filePath), false);

            try
            {
                Template template = new Template(writer, TemplatePath + "SingleFile.InternalFile.cs", Assembly.GetExecutingAssembly());
                
                template.AddReplacement("_Namespace_", GetNamespaceCodePath(m_model.TargetNamespace, true));
                template.AddReplacement("_ExternalNamespace_", GetNamespaceCodePath(m_model.TargetNamespace, false));
                template.AddReplacement("_NamespaceUri_", GetConstantForNamespace(m_model.TargetNamespace));               
                template.AddReplacement("_DesignName_", GetNameForNamespace(m_model.TargetNamespace));                       
                
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

                AddTemplate(
                    template,
                    "// ListOfTypes",
                    TemplatePath + "SingleFile.Type.cs",
                    nodes,
                    new LoadTemplateEventHandler(LoadTemplate_SingleFile_ListOfTypes),
                    new WriteTemplateEventHandler(WriteTemplate_SingleFile_ListOfTypes));
                
                Context context = new Context();
                context.Target = nodes;
                template.WriteTemplate(context);       
            }
            finally
            {
                writer.Close();
            }
        }

        #region "// ListOfBrowseNames"
        /// <summary>
        /// Loads the template for a C# class that delclares the browse names.
        /// </summary>
        private string LoadTemplate_SingleFile_BrowseNames(Template template, Context context)
        {
            KeyValuePair<string,string>? browseName = context.Target as KeyValuePair<string,string>?;

            if (browseName == null)
            {
                return null;
            }

            return context.TemplatePath;
        }

        /// <summary>
        /// Writes the code for a C# class that declares the browse names.
        /// </summary>
        private bool WriteTemplate_SingleFile_BrowseNames(Template template, Context context)
        {
            KeyValuePair<string,string>? browseName = context.Target as KeyValuePair<string,string>?;

            if (browseName == null)
            {
                return false;
            }
            
            template.AddReplacement("_SymbolicName_", browseName.Value.Key); 
            template.AddReplacement("_BrowseName_", browseName.Value.Value); 
            
            return template.WriteTemplate(context);
        }
        #endregion
        
        #region "// ListOfIdentifiers"
        /// <summary>
        /// Loads the template for a C# implementation of a type.
        /// </summary>
        private string LoadTemplate_SingleFile_IdClass(Template template, Context context)
        {
            if (context.TemplatePath.EndsWith("NodeIdClass.cs"))
            {
                if (this.m_model.TargetNamespace != DefaultNamespace)
                {
                    return null;
                }
            }

            KeyValuePair<string,List<NodeDesign>>? nodes = context.Target as KeyValuePair<string,List<NodeDesign>>?;

            if (nodes == null)
            {
                return null;
            }

            if (nodes.Value.Value.Count == 0)
            {
                return null;
            }

            return context.TemplatePath;
        }

        /// <summary>
        /// Writes the code for a C# implementation of a type.
        /// </summary>
        private bool WriteTemplate_SingleFile_IdClass(Template template, Context context)
        {
            KeyValuePair<string,List<NodeDesign>>? nodes = context.Target as KeyValuePair<string,List<NodeDesign>>?;

            if (nodes == null)
            {
                return false;
            }
         
            template.AddReplacement("_NodeClass_", nodes.Value.Key); 

            string templatePath = TemplatePath + "SingleFile.IdDeclaration.cs";

            if (context.TemplatePath.EndsWith("NodeIdClass.cs"))
            {
                templatePath = TemplatePath + "SingleFile.NodeIdDeclaration.cs";
            }
            else if (context.TemplatePath.EndsWith(".html"))
            {
                templatePath = TemplatePath + "SingleFile.IdDeclaration.html";
            }

            AddTemplate(
                template,
                "// ListOfIdentifiers",
                templatePath,
                nodes.Value.Value,
                null,
                new WriteTemplateEventHandler(WriteTemplate_SingleFile_IdDeclaration));

            return template.WriteTemplate(context);
        }

        /// <summary>
        /// Writes the code for the child of a type.
        /// </summary>
        private bool WriteTemplate_SingleFile_IdDeclaration(Template template, Context context)
        {
            NodeDesign node = context.Target as NodeDesign;

            if (node == null)
            {
                return false;
            }
            
            template.AddReplacement("_NodeClass_", GetNodeClass(node)); 
            template.AddReplacement("_SymbolicName_", node.SymbolicId.Name); 
            template.AddReplacement("_Identifier_", node.NumericId); 
                        
            return template.WriteTemplate(context);
        }      
        #endregion
        
        #region "// ListOfStateClasses"
        /// <summary>
        /// Loads the template for a C# implementation of a type.
        /// </summary>
        private string LoadTemplate_SingleFile_ListOfStateClasses(Template template, Context context)
        {
            ObjectTypeDesign objectType = context.Target as ObjectTypeDesign;

            if (objectType != null)
            {
                return TemplatePath + "States.Class.cs";
            }
            
            VariableTypeDesign variableType = context.Target as VariableTypeDesign;

            if (variableType != null)
            {
                return TemplatePath + "States.Class.cs";
            }
            
            return null;
        }

        /// <summary>
        /// Writes the code for a C# implementation of a type.
        /// </summary>
        private bool WriteTemplate_SingleFile_ListOfStateClasses(Template template, Context context)
        {
            // handle object or variable type.
            TypeDesign type = context.Target as TypeDesign;

            if (type == null || !(typeof(ObjectTypeDesign).IsInstanceOfType(type) || typeof(VariableTypeDesign).IsInstanceOfType(type)))
            {
                return false;
            }
            
            string className = GetStateClassName(type, false);
            string genericType = null;

            int index = className.IndexOf('<');

            if (index != -1)
            {
                genericType = className.Substring(index);
                className = className.Substring(0, index);
            }

            template.AddReplacement("_Description_", type.Description.Value); 
            template.AddReplacement("_ClassName_", className); 
            template.AddReplacement("<T>", genericType); 
            template.AddReplacement("_BaseType_", GetStateClassName(type.BaseTypeNode, false)); 
            
            string ns = GetConstantForNamespace(type.SymbolicId.Namespace);
            string attribute = String.Format("[DataContract(Name = \"{0}\", Namespace = {1})]", className, ns);
            template.AddReplacement("// _SerializationAttribute_", attribute);     

            Array children = GetChildren1(type.Children);

            AddTemplate(
                template,
                "// ListOfFieldInitializers",
                null,
                children,
                new LoadTemplateEventHandler(LoadTemplate_SingleFile_ListOfFieldInitializersForStateClass),
                null);
            
            AddTemplate(
                template,
                "// ListOfProperties",
                TemplatePath + "States.Property.cs",
                children,
                new LoadTemplateEventHandler(LoadTemplate_SingleFile_ListOfPropertiesForStateClass),
                new WriteTemplateEventHandler(WriteTemplate_SingleFile_ListOfPropertiesForStateClass));

            AddTemplate(
                template,
                "// ListOfFields",
                null,
                children,
                new LoadTemplateEventHandler(LoadTemplate_SingleFile_ListOfFieldsForStateClass),
                null);

            return template.WriteTemplate(context);
        }
        

        /// <summary>
        /// Loads the template for a C# field declaration.
        /// </summary>
        private string LoadTemplate_SingleFile_ListOfFieldInitializersForStateClass(Template template, Context context)
        {
            InstanceDesign instance = context.Target as InstanceDesign;

            if (instance == null)
            {
                return null;
            }
            
            if (instance.ModellingRule != ModellingRule.Mandatory)
            {
                return null;
            }

            if (instance.OveriddenNode != null)
            {
                return null;
            }

            if (instance is MethodDesign)
            {
                return null;
            }

            template.WriteNextLine(context.Prefix);
            template.Write("{1} = {0};", GetStateDefaultValue(instance), GetChildFieldName(instance));

            return context.TemplatePath;
        }
        
        /// <summary>
        /// Writes the field of a class.
        /// </summary>
        private string LoadTemplate_SingleFile_ListOfPropertiesForStateClass(Template template, Context context)
        {
            InstanceDesign instance = context.Target as InstanceDesign;

            if (instance == null)
            {
                return null;
            }
            
            if (instance.ModellingRule == ModellingRule.None || instance.ModellingRule == ModellingRule.MandatoryShared)
            {
                return null;
            }

            if (instance.OveriddenNode != null)
            {
                return null;
            }

            if (instance is MethodDesign)
            {
                return null;
            }

            VariableDesign variable = instance as VariableDesign;

            if (variable != null)
            {
                if (variable.ValueRank != ValueRank.Scalar)
                {
                    return TemplatePath + "States.ArrayProperty.cs";
                }
            }
            
            return context.TemplatePath;
        }
        
        /// <summary>
        /// Creates classes that implement the model.
        /// </summary>
        private bool WriteTemplate_SingleFile_ListOfPropertiesForStateClass(Template template, Context context)
        {
            InstanceDesign instance = context.Target as InstanceDesign;

            if (instance == null)
            {
                return false;
            }

            template.AddReplacement("_Description_", instance.Description.Value);    
            template.AddReplacement("_BrowseName_", instance.BrowseName);     
            template.AddReplacement("_FieldName_", GetChildFieldName(instance));   
            template.AddReplacement("_TypeName_", GetStateClassName(instance));    
            template.AddReplacement("_DefaultValue_", GetStateDefaultValue(instance));

            string attribute = String.Format("[DataMember(Name = \"{0}\", Order = {1})]", instance.BrowseName, context.Index + 1);
            template.AddReplacement("// _SerializationAttribute_", attribute);     
            		
            return template.WriteTemplate(context);
        }
        
        /// <summary>
        /// Loads the template for a C# field declaration.
        /// </summary>
        private string LoadTemplate_SingleFile_ListOfFieldsForStateClass(Template template, Context context)
        {
            InstanceDesign instance = context.Target as InstanceDesign;

            if (instance == null)
            {
                return null;
            }
            
            if (instance.ModellingRule == ModellingRule.None || instance.ModellingRule == ModellingRule.MandatoryShared)
            {
                return null;
            }

            if (instance.OveriddenNode != null)
            {
                return null;
            }

            if (instance is MethodDesign)
            {
                return null;
            }

            template.WriteNextLine(context.Prefix);
            template.Write("private {0} {1};", GetStateClassName(instance), GetChildFieldName(instance));

            return context.TemplatePath;
        }
        
        /// <summary>
        /// Returns the name of the class that stores the state for the node.
        /// </summary>
        private string GetStateDefaultValue(InstanceDesign instance)
        {
            if (instance == null)
            {
                return "null";
            }

            VariableDesign variable = instance as VariableDesign;

            if (variable != null)
            {
                string dataType = GetStateClassFieldDataType(variable.DataTypeNode, variable.ValueRank);
                
                VariableTypeDesign variableType = variable.TypeDefinitionNode as VariableTypeDesign;

                if (variableType.ClassName == "Property")
                {
                    if (variable.DataTypeNode.BasicDataType == BasicDataType.BaseDataType)
                    {
                        if (variable.ValueRank != ValueRank.Scalar)
                        {
                            return "new VariantCollection()";
                        }
                        else
                        {
                            return "Variant.Null";
                        }
                    }

                    if (variable.ValueRank == ValueRank.Scalar)
                    {
                        return GetVariableValueInitializer(variable.DataTypeNode, ValueRank.Scalar);
                    }

                    return String.Format("new {0}()", GetStateClassFieldDataType(variable.DataTypeNode, variable.ValueRank));
                }
                
                if (variableType.ClassName == "DataVariable")
                {
                    return String.Format("new VariableState<{0}>()", dataType);
                }

                if (variableType.DataTypeNode.BasicDataType == BasicDataType.BaseDataType)
                {
                    return String.Format("{0}State<{1}>", variableType.ClassName, dataType);
                }
            }

            return String.Format("new {0}()", GetStateClassName(instance));
        }

        /// <summary>
        /// Returns the name of the class that stores the state for the node.
        /// </summary>
        private string GetStateClassName(InstanceDesign instance)
        {
            if (instance == null)
            {
                return "object";
            }

            VariableDesign variable = instance as VariableDesign;

            if (variable != null)
            {
                string dataType = GetStateClassFieldDataType(variable.DataTypeNode, variable.ValueRank);
                
                VariableTypeDesign variableType = variable.TypeDefinitionNode as VariableTypeDesign;

                if (variableType.ClassName == "Property")
                {
                    return dataType;
                }
                
                if (variableType.ClassName == "DataVariable")
                {
                    return String.Format("VariableState<{0}>", dataType);
                }

                if (variableType.DataTypeNode.BasicDataType == BasicDataType.BaseDataType)
                {
                    return String.Format("{0}State<{1}>", variableType.ClassName, dataType);
                }
            }

            return GetStateClassName(instance.TypeDefinitionNode, false);
        }

        /// <summary>
        /// Returns the class name to use when creating an instance of the type. 
        /// </summary>
        private string GetStateClassName(TypeDesign type, bool isConstructor)
        {
            if (type == null)
            {
                return "object";
            }

            if (type.ClassName == "ObjectSource")
            {
                return "ObjectState";
            }
            
            VariableTypeDesign variableType = type as VariableTypeDesign;

            if (variableType != null)
            {            
                string dataType = GetStateClassFieldDataType(variableType.DataTypeNode, variableType.ValueRank);
                
                if (isConstructor)
                {
                    if (type.ClassName == "DataVariable")
                    {
                        return String.Format("VariableState");
                    }

                    if (variableType.DataTypeNode.BasicDataType == BasicDataType.BaseDataType)
                    {
                        return String.Format("{0}State", type.ClassName);
                    }
                }
                else
                {
                    if (type.ClassName == "DataVariable")
                    {
                        return String.Format("VariableState<{0}>", dataType);
                    }

                    if (variableType.DataTypeNode.BasicDataType == BasicDataType.BaseDataType)
                    {
                        return String.Format("{0}State<T>", type.ClassName);
                    }
                }
            }

            return type.ClassName + "State";
        }

        /// <summary>
        /// Returns the template parameter to use with the type.
        /// </summary>
        private string GetStateClassFieldDataType(DataTypeDesign dataType, ValueRank valueRank)
        {
            if (dataType == null)
            {
                if (valueRank != ValueRank.Scalar)
                {
                    return "VariantCollection";
                }

                return "Variant";
            }

            string scalarName = null;
            
            switch (dataType.BasicDataType)
            {
                case BasicDataType.UserDefined:
                {
                    scalarName = FixClassName(dataType);

                    if (valueRank != ValueRank.Scalar)
                    {
                        return String.Format("{0}Collection", scalarName);
                    }

                    break;
                }

                case BasicDataType.Structure:
                {
                    if (valueRank != ValueRank.Scalar)
                    {
                        return "IEncodeableCollection";
                    }

                    scalarName = "IEncodeable";
                    break;
                }

                case BasicDataType.BaseDataType:
                {
                    if (valueRank != ValueRank.Scalar)
                    {
                        return "VariantCollection";
                    }

                    scalarName = "Variant";
                    break;
                }

                default:
                { 
                    if (valueRank != ValueRank.Scalar)
                    {
                        return String.Format("{0}Collection", dataType.BasicDataType);
                    }

                    scalarName = GetSystemTypeName(dataType);
                    break;
                }
            }
            
            return scalarName;
        }
        #endregion

        #region "// ListOfTypes"
        /// <summary>
        /// Loads the template for a C# implementation of a type.
        /// </summary>
        private string LoadTemplate_SingleFile_ListOfTypes(Template template, Context context)
        {
            if (context.TemplatePath.EndsWith(".html"))
            {
                return null;
            }

            if (context.Token.EndsWith("DataTypes"))
            {
                DataTypeDesign datatype = context.Target as DataTypeDesign;

                if (datatype != null)
                {
                    switch (datatype.BasicDataType)
                    {
                        case BasicDataType.Structure:
                        case BasicDataType.UserDefined:
                        {
                            return TemplatePath + "DataTypes.Class.cs";
                        }

                        case BasicDataType.Enumeration:
                        {
                            return TemplatePath + "DataTypes.Enumeration.cs";
                        }
                    }
                }
                
                return null;
            }
            else
            {
                ObjectTypeDesign objectType = context.Target as ObjectTypeDesign;

                if (objectType != null)
                {
                    return TemplatePath + "SingleFile.Type.cs";
                }
                
                VariableTypeDesign variableType = context.Target as VariableTypeDesign;

                if (variableType != null)
                {
                    return TemplatePath + "SingleFile.Type.cs";
                }
                
                MethodDesign method = context.Target as MethodDesign;

                if (method != null)
                {
                    return TemplatePath + "SingleFile.Method.cs";
                }
            }
            
            return null;
        }
        
        /// <summary>
        /// Writes the code for a C# implementation of a type.
        /// </summary>
        private bool WriteTemplate_SingleFile_ListOfTypes(Template template, Context context)
        {
            // check for method.
            if (context.Target is MethodDesign)
            {
                return WriteTemplate_SingleFile_ListOfMethodsForType(template, context);
            }

            // check for datatype.
            if (context.Target is DataTypeDesign)
            {
                return WriteTemplate_DataTypeClass(template, context);
            }

            // handle object or variable type.
            TypeDesign type = context.Target as TypeDesign;

            if (type == null || !(typeof(ObjectTypeDesign).IsInstanceOfType(type) || typeof(VariableTypeDesign).IsInstanceOfType(type)))
            {
                return false;
            }

            template.AddReplacement("_Description_", type.Description.Value);

            template.AddReplacement("_ClassName_", FixClassName(type));
            template.AddReplacement("_TypeName_", type.SymbolicName.Name);
            template.AddReplacement("<NewT>", (GetTemplateParameter1(type) == "<T>")?"<T>":"");
            template.AddReplacement("_BaseClass_", GetClassNameForInstance(type, true));
            template.AddReplacement("<BaseT>", GetTypeSourceTemplateParameter(type));
            template.AddReplacement("_NodeClass_", GetNodeClass(type));         
                        
            template.AddReplacement("_NamespaceCodePath_", GetNamespaceCodePath(type.SymbolicId.Namespace, false));
            template.AddReplacement("_NamespaceUri_", GetConstantForNamespace(type.SymbolicName.Namespace));
            
            template.AddReplacement("_BrowseName_", type.SymbolicName.Name);                   
            template.AddReplacement("_BrowseNameNamespaceCodePath_", GetNamespaceCodePath(type.SymbolicName.Namespace, false));
            template.AddReplacement("_BrowseNameNamespaceUri_", GetConstantForNamespace(type.SymbolicName.Namespace));

            template.AddReplacement("_BaseType_", type.BaseTypeNode.SymbolicName.Name);
            template.AddReplacement("_BaseTypeNamespaceCodePath_", GetNamespaceCodePath(type.BaseTypeNode.SymbolicId.Namespace, false));
            template.AddReplacement("_BaseTypeNamespaceUri_", GetConstantForNamespace(type.BaseTypeNode.SymbolicId.Namespace));     
            
            Array children = GetChildren1(type.Children);

            AddTemplate(
                template,
                "// ListOfFieldInitializersForType",
                TemplatePath + "SingleFile.FieldInitializerForType.cs",
                children,
                new LoadTemplateEventHandler(LoadTemplate_SingleFile_ListOfFieldInitializersForType),
                new WriteTemplateEventHandler(WriteTemplate_SingleFile_ListOfFieldInitializersForType));
            
            AddTemplate(
                template,
                "// ListOfFieldsForType",
                null,
                children,
                new LoadTemplateEventHandler(LoadTemplate_SingleFile_ListOfFieldsForType),
                null);
            
            AddTemplate(
                template,
                "// DeclareMethods",
                TemplatePath + "SingleFile.DeclareMethods.cs",
                new object[] { children },
                new LoadTemplateEventHandler(LoadTemplate_SingleFile_DeclareMethods),
                new WriteTemplateEventHandler(WriteTemplate_SingleFile_DeclareMethods));
            
            AddTemplate(
                template,
                "// ListOfChildrenForType",
                TemplatePath + "SingleFile.Property.cs",
                children,
                new LoadTemplateEventHandler(LoadTemplate_SingleFile_ListOfChildrenForType),
                new WriteTemplateEventHandler(WriteTemplate_SingleFile_ListOfChildrenForType));
            
            AddTemplate(
                template,
                "// ListOfFieldInitializers",
                TemplatePath + "SingleFile.FieldInitializer.cs",
                children,
                new LoadTemplateEventHandler(LoadTemplate_SingleFile_ListOfFieldInitializers),
                new WriteTemplateEventHandler(WriteTemplate_SingleFile_ListOfFieldInitializersForType));
            
            AddTemplate(
                template,
                "// ListOfFields",
                null,
                children,
                new LoadTemplateEventHandler(LoadTemplate_SingleFile_ListOfFieldsForType),
                null);

            AddTemplate(
                template,
                "// ListOfChildren",
                TemplatePath + "SingleFile.Property.cs",
                children,
                new LoadTemplateEventHandler(LoadTemplate_SingleFile_ListOfChildren),
                new WriteTemplateEventHandler(WriteTemplate_SingleFile_ListOfChildrenForType));

            AddTemplate(
                template,
                "// CreateChildren",
                TemplatePath + "SingleFile.CreateChildren.cs",
                new object[] { children },
                new LoadTemplateEventHandler(LoadTemplate_SingleFile_CreateChildren),
                new WriteTemplateEventHandler(WriteTemplate_SingleFile_CreateChildren));
            
            AddTemplate(
                template,
                "// UpdateChildren",
                TemplatePath + "SingleFile.VariableToDataType.cs",
                new object[] { type },
                new LoadTemplateEventHandler(LoadTemplate_SingleFile_UpdateChildren),
                new WriteTemplateEventHandler(WriteTemplate_SingleFile_UpdateChildren));
                        
            AddTemplate(
                template,
                "// ListOfCloneChildForType",
                TemplatePath + "SingleFile.CloneChildForType.cs",
                children,
                new LoadTemplateEventHandler(LoadTemplate_SingleFile_ListOfCloneChildForType),
                new WriteTemplateEventHandler(WriteTemplate_SingleFile_ListOfCloneChild));

            AddTemplate(
                template,
                "// ListOfCloneChild",
                TemplatePath + "SingleFile.CloneChild.cs",
                children,
                new LoadTemplateEventHandler(LoadTemplate_SingleFile_ListOfCloneChild),
                new WriteTemplateEventHandler(WriteTemplate_SingleFile_ListOfCloneChild));

            return template.WriteTemplate(context);
        }
        
        /// <summary>
        /// Loads the template for a C# field declaration.
        /// </summary>
        private string LoadTemplate_SingleFile_ListOfCloneChildForType(Template template, Context context)
        {
            InstanceDesign instance = context.Target as InstanceDesign;

            if (instance == null)
            {
                return null;
            }

            MethodDesign method = instance as MethodDesign;

            if (method != null)
            {
                if (method.ModellingRule != ModellingRule.None)
                {
                    return null;
                }
            }

            return context.TemplatePath;
        }

        /// <summary>
        /// Loads the template for a C# field declaration.
        /// </summary>
        private string LoadTemplate_SingleFile_ListOfCloneChild(Template template, Context context)
        {
            InstanceDesign instance = context.Target as InstanceDesign;

            if (instance == null)
            {
                return null;
            }

            if (instance.OveriddenNode != null)
            {
                return null;
            }
            
            MethodDesign method = instance as MethodDesign;

            if (method != null)
            {
                if (method.ModellingRule != ModellingRule.Mandatory)
                {
                    return null;
                }
            }

            switch (instance.ModellingRule)
            {
                case ModellingRule.None:
                {
                    return null;
                }
            }

            return context.TemplatePath;
        }

        /// <summary>
        /// Loads the template for a C# field initializer.
        /// </summary>
        private bool WriteTemplate_SingleFile_ListOfCloneChild(Template template, Context context)
        {
            InstanceDesign instance = context.Target as InstanceDesign;

            if (instance == null)
            {
                return false;
            }
            
            template.AddReplacement("_FieldName_", GetChildFieldName(instance)); 
            template.AddReplacement("_ClassName_", GetChildClassName1(instance));
            template.AddReplacement("_ChildName_", GetChildName(instance));
           
            return template.WriteTemplate(context);
        }

        /// <summary>
        /// Loads the template for a C# field declaration.
        /// </summary>
        private string LoadTemplate_SingleFile_ListOfFieldInitializersForType(Template template, Context context)
        {
            InstanceDesign instance = context.Target as InstanceDesign;

            if (instance == null)
            {
                return null;
            }
            
            MethodDesign method = instance as MethodDesign;

            if (method != null)
            {
                return TemplatePath + "SingleFile.FieldInitializerForMethodForType.cs";
            }

            return context.TemplatePath;
        }

        /// <summary>
        /// Loads the template for a C# field declaration.
        /// </summary>
        private string LoadTemplate_SingleFile_ListOfFieldInitializers(Template template, Context context)
        {
            InstanceDesign instance = context.Target as InstanceDesign;

            if (instance == null)
            {
                return null;
            }

            if (instance.OveriddenNode != null)
            {
                if (instance.ModellingRule != ModellingRule.Mandatory)
                {
                    return null;
                }

                return TemplatePath + "SingleFile.FieldInitializerOverride.cs";
            }
            
            MethodDesign method = instance as MethodDesign;

            if (method != null)
            {
                if (method.ModellingRule != ModellingRule.Mandatory)
                {
                    return null;
                }
                    
                return TemplatePath + "SingleFile.FieldInitializerForMethod.cs";
            }

            switch (instance.ModellingRule)
            {
                case ModellingRule.Mandatory:
                {
                    break;
                }

                default:
                {
                    return null;
                }
            }

            return context.TemplatePath;
        }

        /// <summary>
        /// Loads the template for a C# field initializer.
        /// </summary>
        private bool WriteTemplate_SingleFile_ListOfFieldInitializersForType(Template template, Context context)
        {
            InstanceDesign instance = context.Target as InstanceDesign;

            if (instance == null)
            {
                return false;
            }

            template.AddReplacement("_FieldName_", GetChildFieldName(instance)); 
            template.AddReplacement("_ClassName_", GetChildClassName1(instance));
            template.AddReplacement("_NodeClass_", GetNodeClass(instance)); 
            template.AddReplacement("_ChildName_", GetChildName(instance));
                        
            if (instance.OveriddenNode != null)
            {
                instance = instance.OveriddenNode; 
            }

            template.AddReplacement("_SymbolicId_", instance.SymbolicId.Name); 
            template.AddReplacement("_NamespaceCodePath_", GetNamespaceCodePath(instance.SymbolicId.Namespace, false)); 
            template.AddReplacement("_NamespaceUri_", GetConstantForNamespace(instance.SymbolicId.Namespace)); 
            
            template.AddReplacement("_ReferenceType_", instance.ReferenceType.Name); 
            template.AddReplacement("_ReferenceTypeNamespaceCodePath_", GetNamespaceCodePath(instance.ReferenceType.Namespace, false)); 
            template.AddReplacement("_ReferenceTypeNamespaceUri_", GetConstantForNamespace(instance.ReferenceType.Namespace));

            template.AddReplacement("_BrowseName_", instance.SymbolicName.Name); 
            template.AddReplacement("_BrowseNameNamespaceCodePath_", GetNamespaceCodePath(instance.SymbolicName.Namespace, false)); 
            template.AddReplacement("_BrowseNameNamespaceUri_", GetConstantForNamespace(instance.SymbolicName.Namespace)); 
            
            if (instance.TypeDefinitionNode != null)
            {
                template.AddReplacement("_TypeName_", instance.TypeDefinitionNode.SymbolicId.Name); 
                template.AddReplacement("_TypeClass_", GetNodeClass(instance.TypeDefinitionNode)); 
                template.AddReplacement("_TypeNamespaceCodePath_", GetNamespaceCodePath(instance.TypeDefinitionNode.SymbolicId.Namespace, false)); 
                template.AddReplacement("_TypeNamespaceUri_", GetConstantForNamespace(instance.TypeDefinitionNode.SymbolicId.Namespace)); 
            }
            
            TypeDesign type = instance.Parent as TypeDesign;

            if (type != null)
            {
                template.AddReplacement("_ParentTypeName_", type.SymbolicId.Name); 
            }
           
            return template.WriteTemplate(context);
        }

        /// <summary>
        /// Loads the template for a C# field declaration.
        /// </summary>
        private string LoadTemplate_SingleFile_ListOfFieldsForType(Template template, Context context)
        {
            InstanceDesign instance = context.Target as InstanceDesign;

            if (instance == null)
            {
                return null;
            }

            MethodDesign method = instance as MethodDesign;

            if (!context.Token.EndsWith("ForType"))
            {
                if (instance.ModellingRule == ModellingRule.None)
                {
                    return null;
                }

                if (instance.OveriddenNode != null)
                {
                    return null;
                }

                if (method != null && method.ModellingRule != ModellingRule.Mandatory && method.ModellingRule != ModellingRule.Optional)
                {
                    return null;
                }
            }

            template.WriteNextLine(context.Prefix);
            template.Write("{0} {1};", GetChildClassName1(instance), GetChildFieldName(instance));

            return context.TemplatePath;
        }

        /// <summary>
        /// Loads the template for a C# field declaration.
        /// </summary>
        private string LoadTemplate_SingleFile_ListOfChildren(Template template, Context context)
        {
            InstanceDesign instance = context.Target as InstanceDesign;

            if (instance == null)
            {
                return null;
            }

            if (instance.OveriddenNode != null)
            {
                if (instance.ModellingRule != ModellingRule.Mandatory)
                {
                    return null;
                }

                return TemplatePath + "SingleFile.OverriddenProperty.cs";
            }
                        
            MethodDesign method = instance as MethodDesign;

            if (method != null)
            {
                if (method.ModellingRule != ModellingRule.Mandatory && method.ModellingRule != ModellingRule.Optional)
                {
                    return null;
                }

                return TemplatePath + "SingleFile.MethodProperty.cs";
            }

            switch (instance.ModellingRule)
            {
                case ModellingRule.Mandatory:
                {
                    break;
                }

                case ModellingRule.Optional:
                {
                    return TemplatePath + "SingleFile.OptionalProperty.cs";
                }
                
                case ModellingRule.MandatoryShared:
                {
                    return TemplatePath + "SingleFile.SharedProperty.cs";
                }
                
                case ModellingRule.CardinalityRestriction:
                case ModellingRule.ExposesItsArray:
                {
                    return TemplatePath + "SingleFile.ArrayProperty.cs";
                }

                default:
                {
                    return null;
                }
            }

            return context.TemplatePath;
        }

        /// <summary>
        /// Loads the template for a C# field declaration.
        /// </summary>
        private string LoadTemplate_SingleFile_ListOfChildrenForType(Template template, Context context)
        {
            InstanceDesign instance = context.Target as InstanceDesign;

            if (instance == null)
            {
                return null;
            }

            MethodDesign method = instance as MethodDesign;

            if (method != null)
            {
                return TemplatePath + "SingleFile.MethodProperty.cs";
            }
                        
            return context.TemplatePath;
        }

        /// <summary>
        /// Writes the code for the child of a type.
        /// </summary>
        private bool WriteTemplate_SingleFile_ListOfChildrenForType(Template template, Context context)
        {
            InstanceDesign instance = context.Target as InstanceDesign;

            if (instance == null)
            {
                return false;
            }
            
            template.AddReplacement("_Description_", instance.Description.Value); 
            template.AddReplacement("_ChildName_", GetChildName(instance)); 
            template.AddReplacement("_ClassName_", GetChildClassName1(instance)); 
            template.AddReplacement("_FieldName_", GetChildFieldName(instance)); 
            template.AddReplacement("_NodeClass_", GetNodeClass(instance));

            template.AddReplacement("_ReferenceType_", instance.ReferenceType.Name); 
            template.AddReplacement("_ReferenceTypeNamespaceCodePath_", GetNamespaceCodePath(instance.ReferenceType.Namespace, false)); 
            template.AddReplacement("_ReferenceTypeNamespaceUri_", GetConstantForNamespace(instance.ReferenceType.Namespace));

            template.AddReplacement("_BrowseName_", instance.SymbolicName.Name); 
            template.AddReplacement("_BrowseNameNamespaceCodePath_", GetNamespaceCodePath(instance.SymbolicName.Namespace, false)); 
            template.AddReplacement("_BrowseNameNamespaceUri_", GetConstantForNamespace(instance.SymbolicName.Namespace)); 
            
            if (instance.TypeDefinitionNode != null)
            {
                template.AddReplacement("_TypeName_", instance.TypeDefinitionNode.SymbolicId.Name); 
                template.AddReplacement("_TypeClass_", GetNodeClass(instance.TypeDefinitionNode)); 
                template.AddReplacement("_TypeNamespaceCodePath_", GetNamespaceCodePath(instance.TypeDefinitionNode.SymbolicId.Namespace, false)); 
                template.AddReplacement("_TypeNamespaceUri_", GetConstantForNamespace(instance.TypeDefinitionNode.SymbolicId.Namespace)); 
            }
            
            TypeDesign type = instance.Parent as TypeDesign;

            if (type != null)
            {
                template.AddReplacement("_ParentTypeName_", type.SymbolicId.Name); 
            }

            MethodDesign method = instance as MethodDesign;

            if (method != null)
            {  
                string methodName = GetMethodClassPrefix(method);

                if (!method.HasArguments)
                {
                    methodName = "NoArguments";
                }

                template.AddReplacement("_MethodName_", methodName);            
                                        
                AddTemplate(
                    template,
                    "public void _BrowseName_(OperationContext context)",
                    null,
                    new MethodDesign[] { method },
                    new LoadTemplateEventHandler(LoadTemplate_DeclareCallInvoke),
                    null);
                
                AddTemplate(
                    template,
                    "Call(context, this);",
                    null,
                    new MethodDesign[] { method },
                    new LoadTemplateEventHandler(LoadTemplate_InvokeMethodCallback),
                    null);
            }              

            return template.WriteTemplate(context);
        }                  
        #endregion

        #region "// CreateChildren"
        /// <summary>
        /// Loads the template for a C# field declaration.
        /// </summary>
        private string LoadTemplate_SingleFile_CreateChildren(Template template, Context context)
        {
            Array children = context.Target as Array;

            if (children == null)
            {
                return null;
            }

            foreach (InstanceDesign instance in children)
            {
                switch (instance.ModellingRule)
                {
                    case ModellingRule.Optional:
                    case ModellingRule.MandatoryShared:
                    {
                        template.WriteNextLine(context.Prefix);    
                        return context.TemplatePath;
                    }
                }                
            }

            return null;
        }

        /// <summary>
        /// Writes the code for a C# implementation of a type.
        /// </summary>
        private bool WriteTemplate_SingleFile_CreateChildren(Template template, Context context)
        {
            Array children = context.Target as Array;

            if (children == null)
            {
                return false;
            }

            AddTemplate(
                template,
                "// ListOfCreateChildren",
                TemplatePath + "SingleFile.CreateOptionalChild.cs",
                children,
                new LoadTemplateEventHandler(LoadTemplate_SingleFile_ListOfCreateChild),
                new WriteTemplateEventHandler(WriteTemplate_SingleFile_ListOfCreateChild));
                        
            return template.WriteTemplate(context);
        }
        
        /// <summary>
        /// Loads the template for a C# field declaration.
        /// </summary>
        private string LoadTemplate_SingleFile_ListOfCreateChild(Template template, Context context)
        {
            InstanceDesign instance = context.Target as InstanceDesign;

            if (instance == null)
            {
                return null;
            }
            
            if (instance.OveriddenNode != null)
            {
                return null;
            }

            switch (instance.ModellingRule)
            {
                case ModellingRule.Optional:
                {
                    return TemplatePath + "SingleFile.CreateOptionalChild.cs";
                }
                
                case ModellingRule.MandatoryShared:
                {
                    return TemplatePath + "SingleFile.CreateSharedChild.cs";
                }

                default:
                {
                    return null;
                }
            }
        }

        /// <summary>
        /// Writes the code for the child of a type.
        /// </summary>
        private bool WriteTemplate_SingleFile_ListOfCreateChild(Template template, Context context)
        {
            InstanceDesign instance = context.Target as InstanceDesign;

            if (instance == null)
            {
                return false;
            }
            
            template.AddReplacement("_Description_", instance.Description.Value); 
            template.AddReplacement("_ChildName_", GetChildName(instance)); 
            template.AddReplacement("_ClassName_", GetChildClassName1(instance)); 
            template.AddReplacement("_FieldName_", GetChildFieldName(instance)); 
            template.AddReplacement("_NodeClass_", GetNodeClass(instance)); 

            template.AddReplacement("_ReferenceType_", instance.ReferenceType.Name); 
            template.AddReplacement("_ReferenceTypeNamespaceCodePath_", GetNamespaceCodePath(instance.ReferenceType.Namespace, false)); 
            template.AddReplacement("_ReferenceTypeNamespaceUri_", GetConstantForNamespace(instance.ReferenceType.Namespace));

            template.AddReplacement("_BrowseName_", instance.SymbolicName.Name); 
            template.AddReplacement("_BrowseNameNamespaceCodePath_", GetNamespaceCodePath(instance.SymbolicName.Namespace, false)); 
            template.AddReplacement("_BrowseNameNamespaceUri_", GetConstantForNamespace(instance.SymbolicName.Namespace)); 
            
            TypeDesign type = instance.Parent as TypeDesign;

            if (type != null)
            {
                template.AddReplacement("_ParentTypeName_", type.SymbolicId.Name); 
            }

            return template.WriteTemplate(context);
        }          
        #endregion
 
        #region "// UpdateChildren"
        /// <summary>
        /// Loads the template for a C# field declaration.
        /// </summary>
        private string LoadTemplate_SingleFile_UpdateChildren(Template template, Context context)
        {
            VariableTypeDesign variableType = context.Target as VariableTypeDesign;

            if (variableType == null)
            {
                return null;
            }

            if (variableType.ExposesItsChildren && variableType.DataTypeNode.IsStructure)
            { 
                template.WriteNextLine(context.Prefix);
                return context.TemplatePath;
            }

            return null;
        }

        /// <summary>
        /// Writes the code for a C# implementation of a type.
        /// </summary>
        private bool WriteTemplate_SingleFile_UpdateChildren(Template template, Context context)
        {
            VariableTypeDesign variableType = context.Target as VariableTypeDesign;

            if (variableType == null || !variableType.ExposesItsChildren || !variableType.DataTypeNode.IsStructure)
            {                
                return false;
            }

            template.AddReplacement("_DataType_", variableType.DataTypeNode.SymbolicName.Name); 
                        
            AddTemplate(
                template,
                "// ListOfChildrenToRead",
                TemplatePath + "SingleFile.UpdateParentSwitch.cs",
                variableType.DataTypeNode.Fields,
                null,
                new WriteTemplateEventHandler(WriteTemplate_SingleFile_UpdateFromChild));

            AddTemplate(
                template,
                "// ListOfChildrenToWrite",
                null,
                variableType.DataTypeNode.Fields,
                new LoadTemplateEventHandler(LoadTemplate_SingleFile_UpdateFromValue),
                null);
                        
            return template.WriteTemplate(context);
        }
        
        /// <summary>
        /// Loads the template for a C# field declaration.
        /// </summary>
        private bool WriteTemplate_SingleFile_UpdateFromChild(Template template, Context context)
        {
            Parameter field = context.Target as Parameter;

            if (field == null)
            {
                return false;
            }

            VariableTypeDesign variableType = context.Container as VariableTypeDesign;

            if (variableType == null)
            {
                return false;
            }
             
            template.AddReplacement("_ChildName_", field.Name);
            template.AddReplacement("_ParentTypeName_", variableType.SymbolicId.Name);
            template.AddReplacement("_BrowseNameNamespaceCodePath_", GetNamespaceCodePath(variableType.SymbolicName.Namespace, false)); 

            AddTemplate(
                template,
                "// ListOfChildrenToRead",
                null,
                new Parameter[] { field },
                new LoadTemplateEventHandler(LoadTemplate_SingleFile_UpdateParentField),
                null);
                        
            return template.WriteTemplate(context);
        }
            
        /// <summary>
        /// Loads the template for a C# field declaration.
        /// </summary>
        private string LoadTemplate_SingleFile_UpdateParentField(Template template, Context context)
        {
            Parameter field = context.Target as Parameter;

            if (field == null)
            {
                return null;
            }

            BasicDataType basicType = field.DataTypeNode.BasicDataType;
            string elementName = GetDataTypeFieldDataType(field.DataTypeNode, field.ValueRank);
                                   
            template.WriteNextLine(context.Prefix); 

            if (field.ValueRank == ValueRank.Scalar)
            {
                switch (basicType)
                {
                    case BasicDataType.Structure:
                    {                    
		                template.Write("value.{0} = new ExtensionObject({0}.Value);", field.Name);
                        break;
                    }
                        
                    default:
                    case BasicDataType.UserDefined:
                    {
		                template.Write("value.{0} = {0}.Value;", field.Name);
                        break;
                    }
                        
                    case BasicDataType.Guid:
                    {
		                template.Write("value.{0} = new Uuid({0}.Value);", field.Name);
                        break;
                    }
                }
            }
            else
            {
                switch (basicType)
                {
                    case BasicDataType.Structure:
                    {                     
		                template.Write("value.{0} = ExtensionObjectCollection.ToExtensionObjects(({0}.Value);", field.Name);
                        break;
                    }

                    case BasicDataType.UserDefined:
                    default:
                    {
		                template.Write("value.{0} = new {1}({0}.Value);", field.Name, elementName);
                        break;
                    }
                }
            }

            return null;
        }
        
        /// <summary>
        /// Loads the template for a C# field declaration.
        /// </summary>
        private string LoadTemplate_SingleFile_UpdateFromValue(Template template, Context context)
        {
            Parameter field = context.Target as Parameter;

            if (field == null)
            {
                return null;
            }
            
            BasicDataType basicType = field.DataTypeNode.BasicDataType;
            string elementName = GetDataTypeFieldDataType(field.DataTypeNode, field.ValueRank);
                                   
            template.WriteNextLine(context.Prefix); 

            if (field.ValueRank == ValueRank.Scalar)
            {
                switch (basicType)
                {
                    case BasicDataType.Structure:
                    {                    
		                template.Write("{0}.Value = ExtensionObject.ToEncodeable(value.{0});", field.Name);
                        break;
                    }

                    case BasicDataType.UserDefined:
                    default:
                    {
		                template.Write("{0}.Value = value.{0};", field.Name);
                        break;
                    }
                }
            }
            else
            {
                switch (basicType)
                {
                    case BasicDataType.Structure:
                    {                    
		                template.Write("{0}.Value = new ExtensionObjectCollection(value.{0});", field.Name);
                        break;
                    }

                    case BasicDataType.UserDefined:
                    default:
                    {
		                template.Write("{0}.Value = new {1}(value.{0});", field.Name, elementName);
                        break;
                    }
                }
            }

            return null;
        }
        #endregion

        #region "// DeclareMethods"
        /// <summary>
        /// Loads the template for a C# field declaration.
        /// </summary>
        private string LoadTemplate_SingleFile_DeclareMethods(Template template, Context context)
        {
            Array children = context.Target as Array;

            if (children == null)
            {
                return null;
            }

            foreach (InstanceDesign instance in children)
            {
                MethodDesign method = instance as MethodDesign;

                if (method != null && method.MethodType == null && method.HasArguments)
                {
                    template.WriteNextLine(context.Prefix);    
                    return context.TemplatePath;
                }             
            }

            return null;
        }

        /// <summary>
        /// Writes the code for a C# implementation of a type.
        /// </summary>
        private bool WriteTemplate_SingleFile_DeclareMethods(Template template, Context context)
        {
            Array children = context.Target as Array;

            if (children == null)
            {
                return false;
            }

            AddTemplate(
                template,
                "// ListOfMethods",
                TemplatePath + "SingleFile.Method.cs",
                children,
                new LoadTemplateEventHandler(LoadTemplate_SingleFile_ListOfMethodsForType),
                new WriteTemplateEventHandler(WriteTemplate_SingleFile_ListOfMethodsForType));
                        
            return template.WriteTemplate(context);
        }
        
        /// <summary>
        /// Loads the template for a C# field declaration.
        /// </summary>
        private string LoadTemplate_SingleFile_ListOfMethodsForType(Template template, Context context)
        {
            MethodDesign method = context.Target as MethodDesign;

            if (method == null)
            {
                return null;
            }            

            if (method.MethodType != null || !method.HasArguments)
            {
                return null;
            }

            return context.TemplatePath;
        }

        /// <summary>
        /// Writes the code for the child of a type.
        /// </summary>
        private bool WriteTemplate_SingleFile_ListOfMethodsForType(Template template, Context context)
        {
            MethodDesign method = context.Target as MethodDesign;

            if (method == null)
            {
                return false;
            }
            
            template.AddReplacement("_Description_", method.Description.Value); 
            template.AddReplacement("_TypeName_", method.SymbolicName.Name); 
            template.AddReplacement("_ClassName_", GetChildClassName1(method)); 
            template.AddReplacement("_FieldName_", GetChildFieldName(method)); 

            AddTemplate(
                template,
                "public delegate void _TypeName_MethodHandler();",
                null,
                new MethodDesign[] { method },
                new LoadTemplateEventHandler(LoadTemplate_DeclareMethodDelegate),
                null);
            
            AddTemplate(
                template,
                "public void _TypeName_(OperationContext context, NodeSource target)",
                null,
                new MethodDesign[] { method },
                new LoadTemplateEventHandler(LoadTemplate_DeclareCallInvoke),
                null);
            
            AddTemplate(
                template,
                "// CopyInputArguments",
                null,
                method.InputArguments,
                new LoadTemplateEventHandler(LoadTemplate_CopyInputArguments),
                null);
            
            AddTemplate(
                template,
                "// CopyOutputArguments",
                null,
                new MethodDesign[] { method },
                new LoadTemplateEventHandler(LoadTemplate_CopyOutputArguments),
                null);
                                    
            AddTemplate(
                template,
                "// AssignInputArguments",
                null,
                method.InputArguments,
                new LoadTemplateEventHandler(LoadTemplate_AssignInputArguments),
                null);
            
            AddTemplate(
                template,
                "// DeclareOutputArguments",
                null,
                method.OutputArguments,
                new LoadTemplateEventHandler(LoadTemplate_DeclareOutputArguments),
                null);
            
            AddTemplate(
                template,
                "// InvokeCallback",
                null,
                new MethodDesign[] { method },
                new LoadTemplateEventHandler(LoadTemplate_InvokeMethodCallback),
                null);
            
            AddTemplate(
                template,
                "// AssignOutputArguments",
                null,
                method.OutputArguments,
                new LoadTemplateEventHandler(LoadTemplate_AssignOutputArguments),
                null);
                 
            AddTemplate(
                template,
                "// InputArgumentList",
                TemplatePath + "Code.MethodArgument.cs",
                method.InputArguments,
                null,
                new WriteTemplateEventHandler(WriteTemplate_CodeMethodArgument));

            AddTemplate(
                template,
                "// OutputArgumentList",
                TemplatePath + "Code.MethodArgument.cs",
                method.OutputArguments,
                null,
                new WriteTemplateEventHandler(WriteTemplate_CodeMethodArgument));

            return template.WriteTemplate(context);
        }          
        #endregion

        /// <summary>
        /// Returns the children for the type.
        /// </summary>
        private Array GetChildren1(ListOfChildren children)
        {
            List<InstanceDesign> selectedChildren = new List<InstanceDesign>();

            if (children == null)
            {
                return selectedChildren.ToArray();
            }
            
            foreach (InstanceDesign child in children.Items)
            {
                selectedChildren.Add(child);
            }

            return selectedChildren.ToArray();
        }

        /// <summary>
        /// Returns the class name to use when creating an instance of the type. 
        /// </summary>
        private string GetClassNameForInstance(TypeDesign type, bool isDerivation)
        {
            if (type == null)
            {
                return "object";
            }

            VariableTypeDesign variableType = type as VariableTypeDesign;

            if (variableType == null)
            {
                if (isDerivation)
                {
                    return FixClassName(type.BaseTypeNode);
                }

                return FixClassName(type);
            }

            string dataType = GetTemplateParameter1(type);

            if (dataType != "<T>")
            {
                if (isDerivation)
                {
                    return String.Format("{0}{1}", FixClassName(type.BaseTypeNode), dataType);
                }
                    
                return FixClassName(type);
            }
       
            if (isDerivation)
            {
                return String.Format("{0}<T>", FixClassName(type.BaseTypeNode));
            }
            
            return String.Format("{0}<object>", FixClassName(type));
        }
        
        /// <summary>
        /// Returns the field name of a child node.
        /// </summary>
        private string GetChildName(InstanceDesign instance)
        {
            if (instance == null)
            {
                return String.Empty;
            }

            MethodDesign method = instance as MethodDesign;

            if (method != null)
            {
                return String.Format("{0}Method", instance.SymbolicName.Name);
            }

            return instance.SymbolicName.Name;
        }

        /// <summary>
        /// Returns the field name of a child node.
        /// </summary>
        private string GetChildFieldName(InstanceDesign instance)
        {
            if (instance == null)
            {
                return String.Empty;
            }
            
            string name = String.Format("m_{0}{1}", instance.SymbolicName.Name.Substring(0,1).ToLower(), instance.SymbolicName.Name.Substring(1));
            
            MethodDesign method = instance as MethodDesign;

            if (method != null)
            {
                return String.Format("{0}Method", name);
            }

            return name;
        }

        /// <summary>
        /// Returns the template parameter for the type source declaration.
        /// </summary>
        private string GetTypeSourceTemplateParameter(TypeDesign type)
        {
            VariableTypeDesign variableType = type as VariableTypeDesign;

            if (variableType == null)
            {
                return String.Empty;
            }

            string parameter = GetTemplateParameter1(type);

            if (String.IsNullOrEmpty(parameter))
            {
                parameter = GetTemplateParameter1(type.BaseTypeNode); 
            }

            return parameter;
        }
        
        /// <summary>
        /// Returns the template parameter to use with the type.
        /// </summary>
        private string GetTemplateParameter1(TypeDesign type)
        {
            VariableTypeDesign variableType = type as VariableTypeDesign;

            if (variableType == null)
            {
                return String.Empty;
            }

            if (type.BaseTypeNode == null)
            {
                return String.Format("<T>");
            }

            if (GetTemplateParameter1(type.BaseTypeNode) != "<T>")
            {
                return String.Empty;
            }
            
            BasicDataType basicType = variableType.DataTypeNode.BasicDataType;    
            
            if (basicType == BasicDataType.BaseDataType)
            {
                return String.Format("<T>");
            }
                       
            string scalarName = null;

            switch (basicType)
            {
                case BasicDataType.UserDefined:
                {
                    scalarName = FixClassName(variableType.DataTypeNode);
                    break;
                }

                case BasicDataType.Structure:
                {
                    scalarName = "IEncodeable";
                    break;
                }

                default:
                { 
                    scalarName = GetSystemTypeName(variableType.DataTypeNode);
                    break;
                }
            }
            
            if (variableType.ValueRank != ValueRank.Scalar)
            {
                return String.Format("<IList<{0}>>", scalarName);
            }

            return String.Format("<{0}>", scalarName);
        }

        /// <summary>
        /// Returns the prefix for the declaration of a method.
        /// </summary>
        private string GetMethodClassPrefix(MethodDesign method)
        {
            NodeDesign parent = method.MethodType;

            if (parent != null)
            {                    
                return String.Format("{0}", parent.SymbolicName.Name);
            }

            parent = method.Parent;

            while (parent is InstanceDesign)
            {
                parent = parent.Parent;
            }

            TypeDesign parentType = parent as TypeDesign;

            if (parentType != null)
            {
                return String.Format("{0}.{1}", parentType.SymbolicName.Name, method.SymbolicName.Name);
            }
          
            return String.Format("{0}", method.SymbolicName.Name);
        }

        /// <summary>
        /// Returns the default value for a child node.
        /// </summary>
        private string GetChildClassName1(InstanceDesign instance)
        {
            if (instance == null)
            {
                return "NodeSource";
            }

            MethodDesign method = instance as MethodDesign;

            if (method != null)
            {  
                if (!method.HasArguments)
                {
                    return String.Format("MethodSource");
                }
                                
                return String.Format("{0}MethodSource", GetMethodClassPrefix(method));
            }

            string className = GetClassNameForInstance(instance.TypeDefinitionNode, false);

            VariableDesign variable = instance as VariableDesign;
            
            if (variable != null)
            {
                if (className.EndsWith("<object>"))
                {
                    string systemType = GetSystemTypeName(variable.DataTypeNode);

                    if (systemType != null)
                    {                
                        if (variable.ValueRank == ValueRank.Scalar)
                        {
                            return String.Format("{0}<{1}>", FixClassName(variable.TypeDefinitionNode), systemType);
                        }
                        else
                        {
                            return String.Format("{0}<IList<{1}>>", FixClassName(variable.TypeDefinitionNode), systemType);
                        }
                    }
                }
            }

            return className;
        }

        /// <summary>
        /// Returns the default value for a child node.
        /// </summary>
        private string GetTypeClassName1(TypeDesign type)
        {
            if (type == null)
            {
                return "NodeSource";
            }

            VariableTypeDesign variableType = type as VariableTypeDesign;
       
            if (variableType != null)
            {                
                VariableTypeDesign baseType = variableType.BaseTypeNode as VariableTypeDesign;

                // check if a template parameter is required.
                if (baseType.DataTypeNode.BasicDataType == BasicDataType.BaseDataType)
                {
                    return String.Format("{0}{1}", FixClassName(variableType), GetTemplateParameter1(type));                   
                }
            }

            return String.Format("{0}", FixClassName(type));
        }
        
        /// <summary>
        /// Returns the field initializer for a child node.
        /// </summary>
        private string GetChildFieldInitializer(InstanceDesign instance)
        {
            if (instance == null)
            {
                return "null";
            }

            return String.Format("new {0}(Server, this, true)", GetChildClassName1(instance));
        }

        /// <summary>
        /// Returns the browse names for the nodes defined.
        /// </summary>
        private SortedDictionary<string,string> GetBrowseNames(IList<NodeDesign> nodes)
        {
            SortedDictionary<string,string> browseNames = new SortedDictionary<string,string>();

            // add browse names for built nodes (encodings and method arguments).
            if (m_model.TargetNamespace == DefaultNamespace)
            {
                browseNames.Add("DefaultBinary", "Default Binary");                
                browseNames.Add("DefaultXml", "Default XML");
                browseNames.Add("InputArguments", "InputArguments");
                browseNames.Add("OutputArguments", "OutputArguments");
            }

            foreach (NodeDesign node in nodes)
            {
                if (node.SymbolicName.Namespace == m_model.TargetNamespace)
                {
                    browseNames[node.SymbolicName.Name] = node.BrowseName;
                }

                if (!node.HasChildren)
                {
                    continue;
                }
               
                foreach (NodeDesign child in node.Children.Items)
                {
                    if (child.SymbolicName.Namespace == m_model.TargetNamespace)
                    {
                        string browseName = null;

                        if (browseNames.TryGetValue(child.SymbolicName.Name, out browseName))
                        {
                            if (browseName != child.BrowseName)
                            {
                                throw ServiceResultException.Create(
                                    StatusCodes.BadTypeMismatch,
                                    "Two nodes with the same symbolic name have different browse names: {0} != {1}.",
                                    browseName,
                                    child.BrowseName);
                            }

                            continue;
                        }

                        browseNames[child.SymbolicName.Name] = child.BrowseName;
                    }
                }
            }

            return browseNames;
        }

        /// <summary>
        /// Returns the identifiers for the nodes defined.
        /// </summary>
        private SortedDictionary<string,List<NodeDesign>> GetIdentifiers(IList<NodeDesign> nodes)
        {
            SortedDictionary<string,List<NodeDesign>> identifiers = new SortedDictionary<string,List<NodeDesign>>();

            foreach (NodeDesign node in nodes)
            {
                if (node.NumericId == 0)
                {
                    continue;
                }

                string nodeClass = GetNodeClass(node);

                if (nodeClass == "EventType")
                {
                    nodeClass = "ObjectType";
                }

                List<NodeDesign> nodesWithinClass = null;

                if (!identifiers.TryGetValue(nodeClass, out nodesWithinClass))
                {
                    identifiers[nodeClass] = nodesWithinClass = new List<NodeDesign>();
                }

                if (!nodesWithinClass.Contains(node))
                {
                    nodesWithinClass.Add(node);
                }

                // add any data type encodings.
                DataTypeDesign datatype = node as DataTypeDesign;

                if (datatype != null)
                {
                    if (datatype.HasEncodings)
                    {
                        foreach (EncodingDesign encoding in datatype.Encodings)
                        {
                            nodeClass = GetNodeClass(encoding);

                            if (!identifiers.TryGetValue(nodeClass, out nodesWithinClass))
                            {
                                identifiers[nodeClass] = nodesWithinClass = new List<NodeDesign>();
                            }
                            
                            nodesWithinClass.Add(encoding);
                        }
                    }
                }

                if (!node.HasChildren)
                {
                    continue;
                }

                foreach (NodeDesign child in node.Children.Items)
                {
                    nodeClass = GetNodeClass(child);

                    if (!identifiers.TryGetValue(nodeClass, out nodesWithinClass))
                    {
                        identifiers[nodeClass] = nodesWithinClass = new List<NodeDesign>();
                    }

                    nodesWithinClass.Add(child);
                }
            }

            return identifiers;
        }

        /// <summary>
        /// Fixes class names for nodes.
        /// </summary>
        private string FixClassName(TypeDesign node)
        {
            if (node is DataTypeDesign)
            {
                return node.SymbolicId.Name;
            }

            return node.ClassName;
        }
    }
}
