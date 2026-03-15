#region Copyright Syncfusion Inc. 2001 - 2014
// Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Any infringement will be prosecuted under
// applicable laws. 
#endregion
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Controls;
using System.Windows;

namespace Syncfusion.Windows.Diagram
{
    // Partial class of ConnectorBase, used to maintain relationship properties.
    public abstract partial class ConnectorBase
    {
        /// <summary>
        /// Initialize the relationship.
        /// </summary>
        private void InitializeRelationship()
        {
            this.HeadNodeChangedEvent += new System.Windows.DependencyPropertyChangedEventHandler(ConnectorBase_HeadNodeChangedEvent);
            this.TailNodeChangedEvent += new System.Windows.DependencyPropertyChangedEventHandler(ConnectorBase_TailNodeChangedEvent);
        }

        /// <summary>
        /// When connector is deleted from the Model, this funtion is called to update the relationship properties.
        /// </summary>
        internal void DeleteConnector()
        {
            HeadNodeChanged(this.HeadNode as Node, null);
            TailNodeChanged(this.TailNode as Node, null);
        }

        /// <summary>
        /// When connected added into the the Model, this funtion is called to update the relationship properties.
        /// </summary>
        internal void Add()
        {
            HeadNodeChanged(null, this.HeadNode as Node);
            TailNodeChanged(null, this.TailNode as Node);
        }

        /// <summary>
        /// Updated relationship properties when HeadNode is changed.
        /// </summary>
        /// <param name="OldNode">OldNode</param>
        /// <param name="NewNode">NewNode</param>
        private void HeadNodeChanged(Node OldNode, Node NewNode)
        {
            if (OldNode != null && !(OldNode is Group))
            {
                OldNode.Edges.Remove(this);
                OldNode.OutEdges.Remove(this);

                if (TailNode != null)
                {
                    OldNode.Neighbors.Remove(this.TailNode);
                    //OldNode.Children.Remove(this.TailNode);

                    OldNode.OutNeighbors.Remove(this.TailNode);
                    //this.TailNode.Parents.Remove(OldNode);
                    this.TailNode.InNeighbors.Remove(OldNode);
                }
            }

            if (NewNode != null)
            {
                NewNode.Edges.AddIfDoesNotExist(this);
                NewNode.OutEdges.AddIfDoesNotExist(this);

                if (TailNode != null)
                {
                    NewNode.Neighbors.AddIfDoesNotExist(this.TailNode);
                    //NewNode.Children.AddIfDoesNotExist(this.TailNode);
                    NewNode.OutNeighbors.AddIfDoesNotExist(this.TailNode);
                    //this.TailNode.Parents.AddIfDoesNotExist(NewNode);
                    this.TailNode.InNeighbors.AddIfDoesNotExist(NewNode);
                    this.TailNode.Neighbors.AddIfDoesNotExist(NewNode);
                }
            }
        }

        /// <summary>
        /// Head of the connection is changed.
        /// </summary>
        void ConnectorBase_HeadNodeChangedEvent(object sender, System.Windows.DependencyPropertyChangedEventArgs e)
        {
            Node OldNode = e.OldValue as Node;
            Node NewNode = e.NewValue as Node;
            HeadNodeChanged(OldNode, NewNode);            
        }

        /// <summary>
        /// Update relationship properties when TailNode is changed.
        /// </summary>
        /// <param name="OldNode">OldNode</param>
        /// <param name="NewNode">NewNode</param>
        private void TailNodeChanged(Node OldNode, Node NewNode)
        {
            if (OldNode != null && !(OldNode is Group))
            {
                OldNode.Edges.Remove(this);
                OldNode.InEdges.Remove(this);

                if (HeadNode != null)
                {
                    OldNode.Neighbors.Remove(this.HeadNode);
                    //OldNode.Children.Remove(this.HeadNode);

                    //OldNode.Parents.Remove(this.HeadNode);
                    OldNode.InNeighbors.Remove(this.HeadNode);
                    this.HeadNode.OutNeighbors.Remove(OldNode);
                }
            }

            if (NewNode != null)
            {
                NewNode.Edges.AddIfDoesNotExist(this);
                NewNode.InEdges.AddIfDoesNotExist(this);

                if (HeadNode != null)
                {
                    NewNode.Neighbors.AddIfDoesNotExist(this.HeadNode);
                    //NewNode.Children.AddIfDoesNotExist(this.HeadNode);
                    //NewNode.Parents.AddIfDoesNotExist(this.HeadNode);
                    NewNode.InNeighbors.AddIfDoesNotExist(this.HeadNode);
                    this.HeadNode.OutNeighbors.AddIfDoesNotExist(NewNode);
                    this.HeadNode.Neighbors.AddIfDoesNotExist(NewNode);
                }
            }            
        }

        /// <summary>
        /// Tail of the connection is changed.
        /// </summary>
        void ConnectorBase_TailNodeChangedEvent(object sender, System.Windows.DependencyPropertyChangedEventArgs e)
        {
            Node OldNode = e.OldValue as Node;
            Node NewNode = e.NewValue as Node;
            TailNodeChanged(OldNode, NewNode);            
        }
    }
}
