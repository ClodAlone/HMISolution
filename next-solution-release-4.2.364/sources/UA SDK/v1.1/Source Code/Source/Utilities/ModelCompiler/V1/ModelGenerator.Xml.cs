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
        /// Writes the schema information to a static XML export file.
        /// </summary>
        private void WriteTemplate_XmlExport(string filePath, string prefix)
        {          
			Stream ostrm = File.Open(String.Format(@"{0}\{1}NodeSet.xml", filePath, prefix), FileMode.Create);

            try
            {
                // build the table of namespaces.
                NamespaceTable namespaceUris = new NamespaceTable();

                foreach (Namespace ns in m_model.Namespaces)
                {
                    if (ns.Value != DefaultNamespace)
                    {
                        namespaceUris.Append(ns.Value);
                    }
                }

                // declare an empty server uri table.
                StringTable serverUris = new StringTable();

                // process all ndoes.
                NodeSet nodeset = new NodeSet();

                foreach (NodeDesign node in CollectNodesForXmlExport(m_validator.Nodes, m_model.TargetNamespace == DefaultNamespace))
                {
                    Node nodeToExport = null;

                    ObjectTypeDesign objectType = node as ObjectTypeDesign;

                    if (objectType != null)
                    {
                        ObjectTypeNode objectTypeToExport = new ObjectTypeNode();

                        objectTypeToExport.NodeClass = NodeClass.ObjectType;
                        objectTypeToExport.IsAbstract = objectType.IsAbstract;
                        
                        nodeToExport = objectTypeToExport;
                
                        ExportReferences(objectTypeToExport, objectType);
                    }

                    VariableTypeDesign variableType = node as VariableTypeDesign;

                    if (variableType != null)
                    {
                        VariableTypeNode variableTypeToExport = new VariableTypeNode();
                        
                        variableTypeToExport.NodeClass = NodeClass.VariableType;
                        variableTypeToExport.DataType = GetExportedNodeId(variableType.DataTypeNode);
                        variableTypeToExport.ValueRank = GetExportedValueRank(variableType.ValueRank);
                        variableTypeToExport.IsAbstract = variableType.IsAbstract;
                        
                        if (variableType.DataTypeNode.IsEnumeration && variableType.DecodedValue == null)
                        {
                            variableTypeToExport.Value = new Variant(variableType.DataTypeNode.Fields[0].Identifier);
                        }
                        else
                        {
                            variableTypeToExport.Value = new Variant(variableType.DecodedValue);
                        }

                        nodeToExport = variableTypeToExport;      
                
                        ExportReferences(variableTypeToExport, variableType);                  
                    }
                    
                    ObjectDesign objectd = node as ObjectDesign;

                    if (objectd != null)
                    {
                        ObjectNode objectToExport = new ObjectNode();
                        
                        objectToExport.NodeClass = NodeClass.Object;
                        objectToExport.EventNotifier = GetExportedEventNotifier(objectd.SupportsEvents);

                        nodeToExport = objectToExport;                        
                
                        ExportReferences(objectToExport, objectd);   
                    }

                    VariableDesign variable = node as VariableDesign;

                    if (variable != null)
                    {
                        VariableNode variableToExport = new VariableNode();

                        variableToExport.NodeClass = NodeClass.Variable;
                        variableToExport.DataType = GetExportedNodeId(variable.DataTypeNode);
                        variableToExport.ValueRank = GetExportedValueRank(variable.ValueRank);
                        variableToExport.AccessLevel = GetExportedAccessLevel(variable.AccessLevel);
                        variableToExport.UserAccessLevel = variableToExport.AccessLevel;
                        variableToExport.MinimumSamplingInterval = variable.MinimumSamplingInterval;
                        variableToExport.Historizing = variable.Historizing;

                        if (variable.DataTypeNode.IsEnumeration && variable.DecodedValue == null)
                        {
                            variableToExport.Value = new Variant(variable.DataTypeNode.Fields[0].Identifier);
                        }
                        else
                        {
                            variableToExport.Value = new Variant(variable.DecodedValue);
                        }
                        
                        if (variable.Parent is MethodDesign)
                        {
                            ExportMethodArguments((MethodDesign)variable.Parent, variable, variableToExport);
                        }
                        
                        if (variable.Parent is DictionaryDesign)
                        {
                            ExportDataTypeDescription((DictionaryDesign)variable.Parent, variable, variableToExport);
                        }

                        nodeToExport = variableToExport; 
          
                        ExportReferences(variableToExport, variable);                
                    }

                    MethodDesign method = node as MethodDesign;

                    if (method != null)
                    {
                        if (method.Parent == null)
                        {
                            continue;
                        }

                        MethodNode methodToExport = new MethodNode();
                        
                        methodToExport.NodeClass = NodeClass.Method;
                        methodToExport.Executable = true;
                        methodToExport.UserExecutable = true;

                        nodeToExport = methodToExport;               

                        ExportReferences(methodToExport, method);
                    }

                    DataTypeDesign datatype = node as DataTypeDesign;

                    if (datatype != null)
                    {
                        if (datatype.NotInAddressSpace)
                        {
                            continue;
                        }

                        DataTypeNode datatypeToExport = new DataTypeNode();    
                        
                        datatypeToExport.NodeClass = NodeClass.DataType;
                        datatypeToExport.IsAbstract = datatype.IsAbstract;
                        
                        nodeToExport = datatypeToExport;           

                        ExportReferences(datatypeToExport, datatype);                
                    }
                    
                    ReferenceTypeDesign referenceType = node as ReferenceTypeDesign;

                    if (referenceType != null)
                    {
                        ReferenceTypeNode referenceTypeToExport = new ReferenceTypeNode();                        
                        
                        referenceTypeToExport.NodeClass = NodeClass.ReferenceType;
                        referenceTypeToExport.IsAbstract = referenceType.IsAbstract;
                        referenceTypeToExport.Symmetric = referenceType.Symmetric;
                        referenceTypeToExport.InverseName = new Opc.Ua.LocalizedText(referenceType.InverseName.Value);

                        nodeToExport = referenceTypeToExport;   
  
                        ExportReferences(referenceTypeToExport, referenceType);                       
                    }
                    
                    ViewDesign view = node as ViewDesign;

                    if (view != null)
                    {
                        ViewNode viewToExport = new ViewNode();                        
                        
                        viewToExport.NodeClass = NodeClass.View;
                        viewToExport.EventNotifier = GetExportedEventNotifier(view.SupportsEvents);
                        viewToExport.ContainsNoLoops = view.ContainsNoLoops;

                        nodeToExport = viewToExport;    

                        ExportReferences(viewToExport, view);                             
                    }

                    nodeToExport.NodeId = GetExportedNodeId(node);
                    nodeToExport.BrowseName = GetExportedBrowseName(node);
                    nodeToExport.DisplayName = new Opc.Ua.LocalizedText(node.DisplayName.Value);
                    nodeToExport.Description = new Opc.Ua.LocalizedText(node.Description.Value);
                    nodeToExport.WriteMask = node.WriteAccess;
                    nodeToExport.UserWriteMask = node.WriteAccess;
                                
                    if (node.HasReferences)
                    {
                        foreach (Reference reference in node.References)
                        {
                            ExportReference(nodeToExport, reference.ReferenceType, reference.IsInverse, reference.TargetNode);
                        }
                    }
                    
                    // export the node.
                    if (NodeId.IsNull(nodeToExport.NodeId))
                    {
                        throw new ArgumentNullException(node.SymbolicId.Name);
                    }

                    Node exportedNode = nodeset.Add(nodeToExport, namespaceUris, serverUris);

                    foreach (ReferenceNode reference in nodeToExport.References)
                    {
                        nodeset.AddReference(exportedNode, reference, namespaceUris, serverUris);
                    }
                }

                nodeset.Write(ostrm);
            }
            finally
            {
                ostrm.Close();
            }
        }
                        
        /// <summary>
        /// Exports the arguments for a method.
        /// </summary>
        private void ExportMethodArguments(MethodDesign method, VariableDesign variable, VariableNode variableToExport)
        {
            Parameter[] parameters = method.InputArguments;

            if (variable.BrowseName == "OutputArguments")
            {
                parameters = method.OutputArguments;
            }

            Argument[] arguments = new Argument[parameters.Length];

            for (int ii = 0; ii < parameters.Length; ii++)
            {
                arguments[ii] = new Argument();
                arguments[ii].Name = parameters[ii].Name;
                arguments[ii].Description = new Opc.Ua.LocalizedText(parameters[ii].Description.Value);
                arguments[ii].DataType = GetExportedNodeId(parameters[ii].DataTypeNode);
                arguments[ii].ValueRank = GetExportedValueRank(parameters[ii].ValueRank);
            }

            variableToExport.Value = new Variant((object)arguments);
        }
                        
        /// <summary>
        /// Exports the data type description for a data type.
        /// </summary>
        private void ExportDataTypeDescription(DictionaryDesign dictionary, VariableDesign variable, VariableNode variableToExport)
        {
            if (dictionary.EncodingName == new XmlQualifiedName("DefaultXml", DefaultNamespace))
            {
                variableToExport.Value = new Variant(String.Format("//xs:element[@name='{0}']", variable.BrowseName));
            }
            else
            {
                variableToExport.Value = new Variant(variable.BrowseName);
            }
        }

        /// <summary>
        /// Recusively returns the list of children to export.
        /// </summary>
        private void CollectNodesChildrenXmlExport(NodeDesign parent, List<NodeDesign> nodesToExport)
        {
            if (parent.Children != null && parent.Children.Items != null)
            {
                foreach (NodeDesign child in parent.Children.Items)
                {
                    nodesToExport.Add(child);
                    CollectNodesChildrenXmlExport(child, nodesToExport);
                }
            }
        }

        /// <summary>
        /// Returns the list of nodes to export.
        /// </summary>>
        private List<NodeDesign> CollectNodesForXmlExport(IEnumerable<NodeDesign> nodes, bool exportAll)
        {
            List<NodeDesign> nodesToExport = new List<NodeDesign>();

            foreach (NodeDesign node in nodes)
            {
                if (!exportAll)
                {
                    NodeDesign parent = node;

                    while (parent != null)
                    {
                        if (parent.IsDeclaration)
                        {
                            break;
                        }

                        parent = parent.Parent;
                    }

                    if (parent != null)
                    {
                        continue;
                    }
                }

                nodesToExport.Add(node);
            }

            return nodesToExport;
        }

        /// <summary>
        /// Finds the specified node.
        /// </summary>
        private NodeDesign Find(XmlQualifiedName symbolicId)
        {
            foreach (NodeDesign node in m_validator.Nodes)
            {
                if (node.SymbolicId == symbolicId)
                {
                    return node;
                }
            }

            return null;
        }

        /// <summary>
        /// Returns the list of reference to export.
        /// </summary>>
        private List<ReferenceNode> CollectReferencesForXmlExport(NodeDesign node)
        {
            List<ReferenceNode> references = new List<ReferenceNode>();

            if (node.Children != null && node.Children.Items != null)
            {
                foreach (InstanceDesign child in node.Children.Items)
                {                    
                    ReferenceTypeDesign referenceType = Find(child.ReferenceType) as ReferenceTypeDesign;

                    if (referenceType == null)
                    {
                        continue;
                    }

                    ReferenceNode reference = new ReferenceNode();
                    
                    reference.ReferenceTypeId = new NodeId(referenceType.NumericId, (ushort)GetNamespaceIndex(referenceType.SymbolicId.Namespace));
                    reference.IsInverse = false;
                    reference.TargetId = new NodeId(child.NumericId, (ushort)GetNamespaceIndex(child.SymbolicId.Namespace));

                    references.Add(reference);
                } 
            }

            return references;
        }

        /// <summary>
        /// Returns the event notifier attribure for a object.
        /// </summary>
        private byte GetExportedEventNotifier(bool supportsEvents)
        {
            if (supportsEvents)
            {
                return EventNotifiers.SubscribeToEvents;
            }

            return EventNotifiers.None;
        }
        
        /// <summary>
        /// Returns the access level rank of a variable
        /// </summary>
        private byte GetExportedAccessLevel(AccessLevel accessLevel)
        {
            switch (accessLevel)
            {
                case AccessLevel.Read: { return AccessLevels.CurrentRead; }
                case AccessLevel.Write: { return AccessLevels.CurrentWrite; }
                case AccessLevel.ReadWrite: { return AccessLevels.CurrentReadOrWrite; }
            }

            return AccessLevels.None;
        }

        /// <summary>
        /// Returns the array rank of a variable or variable type.
        /// </summary>
        private int GetExportedValueRank(ValueRank valueRank)
        {
            if (valueRank == ValueRank.Array)
            {
                 return ValueRanks.OneDimension;
            }

            if (valueRank == ValueRank.Scalar)
            {
                 return ValueRanks.Scalar;
            }

            return ValueRanks.Any;
        }
        
        /// <summary>
        /// Returns the NodeId of the modelling rule.
        /// </summary>
        private NodeDesign GetExportedModellingRule(ModellingRule modellingRule)
        {
            string browseName = String.Format("ModellingRule_{0}", modellingRule);

            NodeDesign node = Find(new XmlQualifiedName(browseName, DefaultNamespace));

            if (node != null)
            {
                return node;
            }

            return null;
        }

        /// <summary>
        /// Returns the NodeId for a Node.
        /// </summary>
        private NodeId GetExportedNodeId(NodeDesign node)
        {
            return new NodeId(node.NumericId, (ushort)GetNamespaceIndex(node.SymbolicId.Namespace));
        }

        /// <summary>
        /// Returns the BrowseName for a Node.
        /// </summary>
        private QualifiedName GetExportedBrowseName(NodeDesign node)
        {
            if (node == null)
            {
                return null;
            }
          
            return new QualifiedName(node.BrowseName, (ushort)GetNamespaceIndex(node.SymbolicName.Namespace));
        }        
        
        /// <summary>
        /// Returns the NodeId for an SymbolicId.
        /// </summary>
        private NodeId GetExportedNodeId(XmlQualifiedName symbolicId)
        {
            NodeDesign node = Find(symbolicId);

            if (node == null)
            {
                return null;
            }
          
            return GetExportedNodeId(node);
        }
        
        /// <summary>
        /// Exports a reference for a Node.
        /// </summary>  
        private void ExportReference(Node nodeToExport, XmlQualifiedName referenceTypeId, bool isInverse, NodeDesign target)
        {
            ReferenceNode reference = new ReferenceNode();

            reference.ReferenceTypeId = GetExportedNodeId(referenceTypeId);
            reference.IsInverse = isInverse;
            reference.TargetId = GetExportedNodeId(target);

            if (!NodeId.IsNull(reference.ReferenceTypeId) && !NodeId.IsNull(reference.TargetId))
            {
                nodeToExport.References.Add(reference);
            }
        }
        
        /// <summary>
        /// Exports the references for a ObjectType node.
        /// </summary>        
        private void ExportReferences(ObjectTypeNode nodeToExport, ObjectTypeDesign node)
        {
            if (node.BaseTypeNode != null)
            {
                ExportReference(
                    nodeToExport, 
                    new XmlQualifiedName("HasSubtype", DefaultNamespace), 
                    true, 
                    node.BaseTypeNode);
            }
                        
            if (node.HasChildren)
            {
                foreach (InstanceDesign instance in node.Children.Items)
                {
                    ExportReference(nodeToExport, instance.ReferenceType, false, instance);
                }
            }
        }
        
        /// <summary>
        /// Exports the references for a VariableType node.
        /// </summary>
        private void ExportReferences(VariableTypeNode nodeToExport, VariableTypeDesign node)
        {
            if (node.BaseTypeNode != null)
            {
                ExportReference(
                    nodeToExport, 
                    new XmlQualifiedName("HasSubtype", DefaultNamespace), 
                    true, 
                    node.BaseTypeNode);
            }

            if (node.HasChildren)
            {
                foreach (InstanceDesign instance in node.Children.Items)
                {
                    ExportReference(nodeToExport, instance.ReferenceType, false, instance);
                }
            }
        }
        
        /// <summary>
        /// Exports the references for a Object node.
        /// </summary>
        private void ExportReferences(ObjectNode nodeToExport, ObjectDesign node)
        {
            if (node.ModellingRule != ModellingRule.None)
            {
                ExportReference(
                    nodeToExport, 
                    new XmlQualifiedName("HasModellingRule", DefaultNamespace), 
                    false, 
                    GetExportedModellingRule(node.ModellingRule));
            }

            ExportReference(
                nodeToExport, 
                new XmlQualifiedName("HasTypeDefinition", DefaultNamespace), 
                false, 
                node.TypeDefinitionNode);
            
            if (node.HasChildren)
            {
                foreach (InstanceDesign instance in node.Children.Items)
                {
                    ExportReference(nodeToExport, instance.ReferenceType, false, instance);
                }
            }
        }
        
        /// <summary>
        /// Exports the references for a Variable node.
        /// </summary>
        private void ExportReferences(VariableNode nodeToExport, VariableDesign node)
        {   
            if (node.ModellingRule != ModellingRule.None)
            {
                ExportReference(
                    nodeToExport, 
                    new XmlQualifiedName("HasModellingRule", DefaultNamespace), 
                    false, 
                    GetExportedModellingRule(node.ModellingRule));
            }
            
            ExportReference(
                nodeToExport, 
                new XmlQualifiedName("HasTypeDefinition", DefaultNamespace), 
                false, 
                node.TypeDefinitionNode);
            
            if (node.HasChildren)
            {
                foreach (InstanceDesign instance in node.Children.Items)
                {
                    ExportReference(nodeToExport, instance.ReferenceType, false, instance);
                }
            }
        }        
        
        /// <summary>
        /// Exports the references for a Method node.
        /// </summary>
        private void ExportReferences(MethodNode nodeToExport, MethodDesign node)
        {
            if (node.ModellingRule != ModellingRule.None)
            {
                ExportReference(
                    nodeToExport, 
                    new XmlQualifiedName("HasModellingRule", DefaultNamespace), 
                    false, 
                    GetExportedModellingRule(node.ModellingRule));
            }
            
            if (node.HasChildren)
            {
                foreach (InstanceDesign instance in node.Children.Items)
                {
                    ExportReference(nodeToExport, instance.ReferenceType, false, instance);
                }
            }
        }

        /// <summary>
        /// Exports the references for a DataType node.
        /// </summary>
        private void ExportReferences(DataTypeNode nodeToExport, DataTypeDesign node)
        {
            if (node.BaseTypeNode != null)
            {
                ExportReference(
                    nodeToExport, 
                    new XmlQualifiedName("HasSubtype", DefaultNamespace), 
                    true, 
                    node.BaseTypeNode);
            }
            
            if (node.HasEncodings)
            {
                foreach (EncodingDesign encoding in node.Encodings)
                {
                    ExportReference(
                        nodeToExport, 
                        new XmlQualifiedName("HasEncoding", DefaultNamespace), 
                        false, 
                        encoding);
                }
            }
            
            if (node.HasChildren)
            {
                foreach (InstanceDesign instance in node.Children.Items)
                {
                    ExportReference(nodeToExport, instance.ReferenceType, false, instance);
                }
            }
        }       
        
        /// <summary>
        /// Exports the references for a ReferenceType node.
        /// </summary>
        private void ExportReferences(ReferenceTypeNode nodeToExport, ReferenceTypeDesign node)
        {
            if (node.BaseTypeNode != null)
            {
                ExportReference(
                    nodeToExport, 
                    new XmlQualifiedName("HasSubtype", DefaultNamespace), 
                    true, 
                    node.BaseTypeNode);
            }
            
            if (node.HasChildren)
            {
                foreach (InstanceDesign instance in node.Children.Items)
                {
                    ExportReference(nodeToExport, instance.ReferenceType, false, instance);
                }
            }
        }
        
        /// <summary>
        /// Exports the references for a ReferenceType node.
        /// </summary>
        private void ExportReferences(ViewNode nodeToExport, ViewDesign node)
        {               
            if (node.HasChildren)
            {
                foreach (InstanceDesign instance in node.Children.Items)
                {
                    ExportReference(nodeToExport, instance.ReferenceType, false, instance);
                }
            }
        }
    }
}
