#region Copyright Syncfusion Inc. 2001 - 2014
// Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Any infringement will be prosecuted under
// applicable laws. 
#endregion
using System.Windows;
using System.Xml;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.Serialization;
using System.Text;
using System.Xml.Serialization;

#if WINRT_USING
using Windows.Foundation;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
#endif
using Syncfusion.UI.Xaml.Diagram.Controller;
using Syncfusion.UI.Xaml.Diagram.Controls;
using Syncfusion.UI.Xaml.Diagram.Utility;

namespace Syncfusion.UI.Xaml.Diagram.Serializer
{
    public class Group : Node
    {
        public List<Node> NodeIds { get; set; }

        public List<Connector> ConnectorIds { get; set; }

        public List<Group> GroupIds { get; set; }

        //private static IEnumerable<Type> GetKnownType()
        //{
        //    foreach (var type in GraphSerializerController.GetKnownType())
        //    {
        //        yield return type;
        //    }
        //}

        internal void Serializer(IInternalGroup wrapper)
        {
            base.Serializer(wrapper);
            NodeIds = new List<Node>();
            ConnectorIds = new List<Connector>();
            GroupIds = new List<Group>();
            UpdateGroup(this, wrapper);
        }

        private static void UpdateGroup(Group g, IInternalGroup wrapper)
        {
            foreach (IInternalNode node in wrapper.InternalNodes)
            {
                g.NodeIds.Add(node.Serialize());
            }
            foreach (IInternalConnector con in wrapper.InternalConnectors)
            {
                g.ConnectorIds.Add(con.Serialize());
            }

            if (wrapper.InternalGroups != null)
            {
                foreach (IInternalGroup group in wrapper.InternalGroups)
                {
                    g.GroupIds.Add(group.Serialize());
                }
            }
        }

        internal IInternalGroup DeSerializeGroup(IGraphInternal diagram)
        {
            IInternalGroup wrapper = base.DeSerializeNode(diagram) as IInternalGroup;
            DeSerializeAnnotationsandPorts(wrapper, diagram);

            IInternalGroup internalNode = wrapper;
            //List<IInternalNode> Nodes = new List<IInternalNode>();
            //List<IInternalConnector> Connectors = new List<IInternalConnector>();
            //List<IInternalGroup> Groups = new List<IInternalGroup>();
            //internalNode.InternalNodes = new ObservableElements<object, IInternalNode>(Nodes, ElementType.Node, SourceType.Group, _mSharedData.EventAggregator, _mSharedData.Graph.GetNodeWrapper);
            //internalNode.InternalConnectors = new ObservableElements<object, IInternalConnector>(Connectors, ElementType.Connector, SourceType.Group, _mSharedData.EventAggregator, _mSharedData.Graph.GetConnectorWrapper);
            //internalNode.InternalGroups = new ObservableElements<object, IInternalGroup>(Groups, ElementType.Group, SourceType.Group, _mSharedData.EventAggregator, _mSharedData.Graph.GetGroupWrapper);
            //List<IAnnotation> Annotation = new List<IAnnotation>();
            //internalNode.InternalAnnotations = new ObservableElements<IAnnotation, AnnotationEditorWrapper>(Annotation, ElementType.Annotation, SourceType.Node, _mSharedData.EventAggregator, _mSharedData.Graph.GetAnnotationWrapper);
            //List<INodePort> Port = new List<INodePort>();
            //internalNode.Ports = new ObservableElements<INodePort, IInternalNodePort>(Port, ElementType.Port, SourceType.Node, _mSharedData.EventAggregator, _mSharedData.Graph.GetNodePortWrapper);

            //if (!Serialization)
            //{
            //    if (!_misCutEnabled)
            //    {
            //        internalNode.OffsetX = internalNode.OffsetX + (25 * (_mPasteCount + 1));
            //        internalNode.OffsetY = internalNode.OffsetY + (25 * (_mPasteCount + 1));
            //    }
            //}


            foreach (Node node in NodeIds)
            {
                IInternalNode node1 = node.DeSerializeNode(diagram);
                node1.KnownParentGroup = internalNode;
            }
            foreach (Connector con in ConnectorIds)
            {
                IInternalConnector con1 = con.DeSerialize(diagram);
                con1.KnownParentGroup = internalNode;
            }
            foreach (Group grp in GroupIds)
            {
                IInternalGroup internalgroup = grp.DeSerializeGroup(diagram);
                diagram.InternalGroups.Add(internalgroup, ItemSource.UnKnown);
            }

            return wrapper;
        }
    }
}
