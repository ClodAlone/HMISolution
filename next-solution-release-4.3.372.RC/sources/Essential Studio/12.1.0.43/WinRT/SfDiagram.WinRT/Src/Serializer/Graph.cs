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
    public class Graph
    {
        //public object ID { get; set; }
        public GraphConstraints Constraints { get; set; }
        public MultipleSelectionMode MultipleSelectionMode { get; set; }
        public Tool Tool { get; set; }
        public DrawingTool DrawingTool { get; set; }

        public List<Node> Nodes { get; set; }
        public List<Connector> Connectors { get; set; }
        public List<Group> Groups { get; set; }
        public Dictionary<string, object> Attributes { get; set; }
        public Version V { get; set; }

        private void SerializeProperties(IGraphInternal diagram)
        {
            Constraints = diagram.Constraints;
            MultipleSelectionMode = diagram.MultipleSelectionMode;
            Tool = diagram.Tool;
            DrawingTool = diagram.DrawingTool;
            Attributes = GraphSerializerController.GetAttributes(diagram.Source);
            Nodes = new List<Node>();
            Connectors = new List<Connector>();
            Groups = new List<Group>();
            V = this.GetVersion();
        }

        internal void Serialize(IGraphInternal diagram)
        {
            SerializeProperties(diagram);
            if (diagram.InternalNodes != null)
            {
                foreach (IInternalNode node in diagram.InternalNodes)
                {
                    Nodes.Add(node.Serialize());
                }
            }
            if (diagram.InternalConnectors != null)
            {
                foreach (IInternalConnector connector in diagram.InternalConnectors)
                {
                    Connector con = connector.Serialize();
                    Connectors.Add(con);
                }
            }
            if (diagram.InternalGroups != null)
            {
                foreach (IInternalGroup group in diagram.InternalGroups)
                {
                    Group group1 = @group.Serialize();
                    Groups.Add(group1);
                }
            }
        }

        private void Serialize(IInternalGroup selector)
        {
            foreach (IInternalNode node in selector.InternalNodes)
            {
                Nodes.Add(node.Serialize());
            }
            foreach (IInternalConnector connector in selector.InternalConnectors)
            {
                Connector con = connector.Serialize();
                Connectors.Add(con);
            }
            foreach (IInternalGroup group in selector.InternalGroups)
            {
                Group group1 = group.Serialize();
                Groups.Add(group1);
            }
        }

        internal void DeSerialize(IGraphInternal diagram)
        {
            //diagram.InternalSelectedItems.ClearSelection();
            foreach (Node node in Nodes)
            {
                IInternalNode internalNode = node.DeSerializeNode(diagram);
                (diagram.InternalNodes).Add(internalNode, ItemSource.UnKnown);
            }

            foreach (Connector connector in Connectors)
            {
                IInternalConnector internalConnector = connector.DeSerialize(diagram);
                (diagram.InternalConnectors).Add(internalConnector, ItemSource.UnKnown);
            }
            foreach (Group group in Groups)
            {
                IInternalGroup internalgroupnode = group.DeSerializeGroup(diagram);
                diagram.InternalGroups.Add(internalgroupnode, ItemSource.UnKnown);
                internalgroupnode.UpdateBoundsCorners();
            }

            //if (!Serialization)
            //{
            //    if (!_misCutEnabled)
            //    {
            //        _mPasteCount++;
            //    }
            //}
            //_misCutEnabled = false;
            GraphSerializerController.SetAttributes(diagram.Source, Attributes);
        }

        internal void Copy(IGraphInternal diagram)
        {
            //SerializeProperties(diagram);

            Nodes = new List<Node>();
            Connectors = new List<Connector>();
            Groups = new List<Group>();
            Serialize(diagram.InternalSelectedItems);
        }

        //private static IEnumerable<Type> GetKnownType()
        //{
        //    foreach (var type in GraphSerializerController.GetKnownType())
        //    {
        //        yield return type;
        //    }
        //}
    }
}
