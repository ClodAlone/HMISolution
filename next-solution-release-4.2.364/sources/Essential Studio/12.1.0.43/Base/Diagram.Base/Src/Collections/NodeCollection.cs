#region Copyright Syncfusion Inc. 2001 - 2014
// Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
//
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Re-distribution in any form is strictly
// prohibited. Any infringement will be prosecuted under applicable laws. 
#endregion

using System;
using System.Collections;
using System.ComponentModel;
using System.Runtime.Serialization;

namespace Syncfusion.Windows.Forms.Diagram
{
    /// <summary>
    /// A collection of <see cref="Syncfusion.Windows.Forms.Diagram.Node"/> objects.
    /// </summary>
    [Serializable]
    [Description("Collection of nodes.")]
    [DefaultProperty("Item")]
    public class NodeCollection
        : CollectionEx
    {
        #region Class contants
        /// <summary>
        /// Name of the node.
        /// </summary>
        protected const string c_strNODE = "Node";
        #endregion

        #region Class nested classes
        private sealed class NodeEnumerator
            : IEnumerator
        {
            #region Class members
            private NodeCollection m_collection;
            private int m_nIndex;
            private int m_nCollectionMembers;
            #endregion

            #region Class initilize/finalize methods
            internal NodeEnumerator(NodeCollection nodes)
            {
                if (nodes == null)
                    throw new ArgumentNullException("nodes");

                m_collection = nodes;
                m_nIndex = -1;
                m_nCollectionMembers = m_collection.Count;
            }
            #endregion

            #region IEnumerator
            public object Current
            {
                get { return m_collection[m_nIndex]; }
            }
            public bool MoveNext()
            {
                if (m_collection.Count != m_nCollectionMembers)
                    throw new InvalidOperationException("collection was modified");

                return ++m_nIndex < m_collection.Count;
            }
            public void Reset()
            {
                m_nIndex = -1;
            }
            #endregion
        }
        #endregion

        #region Class Initialize/Finalize methods
        /// <summary>
        /// Initializes a new instance of the <see cref="NodeCollection"/> class.
        /// </summary>
        public NodeCollection()
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="NodeCollection"/> class.
        /// </summary>
        /// <param name="owner">The owner.</param>
        public NodeCollection(object owner)
        {
            this.Owner = owner;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="NodeCollection"/> class.
        /// </summary>
        /// <param name="src">The node collection</param>
        public NodeCollection(NodeCollection src)
            : this(src, true)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="NodeCollection"/> class.
        /// </summary>
        /// <param name="src">The node collection</param>
        /// <param name="cloning">Clone the collection.</param>
        public NodeCollection(NodeCollection src, bool cloning)
        {
            if (!cloning)
            {
                foreach (Node curObj in src)
                {
                    Members.Add(curObj);
                }
            }
            else
            {
                foreach (object curObj in src)
                {
                    ICloneable cloneableObj = curObj as ICloneable;

                    if (cloneableObj != null)
                    {
                        Members.Add(cloneableObj.Clone());
                    }
                    else
                    {
                        Members.Add(curObj);
                    }
                }
            }

            this.QuietMode = src.QuietMode;
            this.UpdateReferences = src.UpdateReferences;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="NodeCollection"/> class.
        /// </summary>
        /// <param name="info">The info.</param>
        /// <param name="context">The context.</param>
        public NodeCollection(SerializationInfo info, StreamingContext context)
            : base(info, context)
        {
        }
        #endregion

        #region Class properties
        /// <summary>
        /// Gets or sets the container.
        /// </summary>
        /// <value>The container.</value>
        public object Container
        {
            get
            {
                return this.Owner;
            }
            set
            {
                if (this.Owner != value)
                {
                    this.Owner = value;
                }
            }
        }

        /// <summary>
        /// Gets or sets the first item in collection.
        /// </summary>
        /// <exception cref="System.ArgumentOutOfRangeException">If collection count less than 1.</exception>
        /// <value>The first.</value>
        public Node First
        {
            get { return this[0]; }
            set { this[0] = value; }
        }

        /// <summary>
        /// Gets or sets the last item in collection.
        /// </summary>
        /// <exception cref="System.ArgumentOutOfRangeException">If collection count less than 1.</exception>
        /// <value>The last.</value>
        public Node Last
        {
            get { return this[this.Members.Count - 1]; }
            set { this[this.Members.Count - 1] = value; }
        }

        /// <summary>
        /// Gets or sets the <see cref="Syncfusion.Windows.Forms.Diagram.Node"/> at the specified index.
        /// </summary>
        /// <param name="index">The index.</param>
        /// <value>The node</value>
        public Node this[int index]
        {
            get { return this.Members[index] as Node; }
            set { Set(index, value); }
        }

        /// <summary>
        /// Gets or sets the <see cref="Syncfusion.Windows.Forms.Diagram.Node"/> with the specified node name.
        /// </summary>
        /// <param name="strNodeName">Node name.</param>
        /// <value>The node with the specified name.</value>
        public Node this[string strNodeName]
        {
            get
            {
                return FindNodeByName(strNodeName);
            }
            set
            {
                int nNodeIndex = -1;
                int nCounter = 0;

                // find node to modify
                foreach (Node node in this.Members)
                {
                    if (node.Name == strNodeName)
                    {
                        nNodeIndex = nCounter;
                        break;
                    }

                    // update counter
                    nCounter++;
                }

                Set(nNodeIndex, value);
            }
        }
        #endregion

        #region Class public methods
        /// <summary>
        /// Adds an node to the end of the <see cref="Syncfusion.Windows.Forms.Diagram.NodeCollection"/>.
        /// </summary>
        /// <param name="node">The node to add.</param>
        /// <returns>The value</returns>
        public int Add(Node node)
        {
            return AddValue(node);
        }

        /// <summary>
        /// Searches for the specified node and returns the zero-based index of 
        /// the first occurrence within the entire <see cref="Syncfusion.Windows.Forms.Diagram.NodeCollection"/>.
        /// </summary>
        /// <param name="node">The node.</param>
        /// <returns>
        /// The zero-based index of the first occurrence of <paramref name="node"/> 
        /// within the entire <see cref="Syncfusion.Windows.Forms.Diagram.NodeCollection"/>, 
        /// if found; otherwise, -1.
        /// </returns>
        public int IndexOf(Node node)
        {
            return this.Members.IndexOf(node);
        }

        /// <summary>
        /// Inserts an element into the <see cref="Syncfusion.Windows.Forms.Diagram.NodeCollection"/>.
        /// </summary>
        /// <param name="index">The zero-based starting index of the search.</param>
        /// <param name="node">The node to locate in the 
        /// <see cref="Syncfusion.Windows.Forms.Diagram.NodeCollection"/>.</param>
        public void Insert(int index, Node node)
        {
            InsertValue(index, node);
        }

        /// <summary>
        /// Removes the first occurrence of a specific node from the 
        /// <see cref="Syncfusion.Windows.Forms.Diagram.NodeCollection"/>.
        /// </summary>
        /// <param name="node">The node.</param>
        /// <returns>true, if remove the node, false otherwise.</returns>
        public bool Remove(Node node)
        {
           // node.UpdateReferences(null);
            return RemoveValue(node);
        }

        /// <summary>
        /// Removes a range of elements from the 
        /// <see cref="Syncfusion.Windows.Forms.Diagram.NodeCollection"/>.
        /// </summary>
        /// <param name="nodes">The nodes.</param>
        public void Remove(NodeCollection nodes)
        {
            RemoveRange(nodes);
        }

        /// <summary>
        /// Determines whether <see cref="Syncfusion.Windows.Forms.Diagram.NodeCollection"/> 
        /// contains the specified node.
        /// </summary>
        /// <param name="node">The node.</param>
        /// <returns>
        /// <c>true</c> if NodeCollection contains the specified node; otherwise, <c>false</c>.
        /// </returns>
        public bool Contains(Node node)
        {
            return this.Members.Contains(node);
        }

        /// <summary>
        /// Copies the entire <see cref="Syncfusion.Windows.Forms.Diagram.NodeCollection"/> 
        /// to a compatible one-dimensional <see cref="T:System.Array"/>, starting at the beginning of the target array.
        /// </summary>
        /// <param name="nodes">The one-dimensional array that is the destination 
        /// of the elements copied from <see cref="Syncfusion.Windows.Forms.Diagram.NodeCollection"/>.</param>
        /// <param name="index">is equal to or greater than the length of <paramref name="nodes"/>.</param>
        public void CopyTo(Node[] nodes, int index)
        {
            this.Members.CopyTo(nodes, index);
        }

        /// <summary>
        /// Determines whether <see cref="Syncfusion.Windows.Forms.Diagram.NodeCollection"/> 
        /// contains the node with specified name.
        /// </summary>
        /// <param name="strNodeName">Name of the node.</param>
        /// <returns>
        /// <c>true</c> if NodeCollection contains the node with specified name; otherwise, <c>false</c>.
        /// </returns>
        public bool Contains(string strNodeName)
        {
            return FindNodeByName(strNodeName) != null;
        }
        #endregion

        #region Class overrides
        /// <summary>
        /// Returns an enumerator that iterates through a collection.
        /// </summary>
        /// <returns>
        /// An <see cref="T:System.Collections.IEnumerator"/> object that can be used to iterate through the collection.
        /// </returns>
        public override IEnumerator GetEnumerator()
        {
            return new NodeEnumerator(this);
        }
        /// <summary>
        /// Creates a new object that is a copy of the current instance.
        /// </summary>
        /// <returns>
        /// A new object that is a copy of this instance.
        /// </returns>
        public override object Clone()
        {
            NodeCollection nc = new NodeCollection(this);

            //Restores the Connection of Cloned Nodes.
            foreach (Node node in this)            
            {
                RestoreConnections(node, nc);
            }           
            return nc;
        }

        /// <summary>
        /// Restores the connections as original node.
        /// </summary>
        /// <param name="originalNode">The original node.</param>
        /// <param name="clonedNodes">The cloned node.</param>
        private void RestoreConnections(Node originalNode, NodeCollection clonedNodes)
        {
            int nPort;

            Node clonedNode = FindNodeByName(originalNode, clonedNodes);
            ICompositeNode originalComposite = originalNode as ICompositeNode;

            if (originalComposite != null)
            {
                for (int i = 0, length = originalComposite.ChildCount; i < length; i++)
                {
                    RestoreConnections(originalComposite.GetChild(i), clonedNodes);
                }
            }

            foreach (ConnectionPoint port in originalNode.Ports)
            {
                foreach (EndPoint connection in port.Connections)
                {
                    Node connector = connection.Container as Node;
                    Node clonedConnector = FindNodeByName(connector, clonedNodes);

                    IEndPointContainer endPointContainer = connector as IEndPointContainer;
                    IEndPointContainer endPointContainerCloned = clonedConnector as IEndPointContainer;

                    if (clonedConnector != null && endPointContainer != null)
                    {
                        if (connection is HeadEndPoint)
                        {
                            nPort = originalNode.Ports.IndexOf(endPointContainer.HeadEndPoint.Port);
                            if (clonedNode != null)
                            {
                                if (nPort >= 0 && nPort < clonedNode.Ports.Count)
                                    clonedNode.Ports[nPort].Connect(endPointContainerCloned.HeadEndPoint);
                            }
                        }
                        else if (connection is TailEndPoint)
                        {
                            nPort = originalNode.Ports.IndexOf(endPointContainer.TailEndPoint.Port);
                            if (clonedNode != null)
                            {
                                if (nPort >= 0 && nPort < clonedNode.Ports.Count)
                                    clonedNode.Ports[nPort].Connect(endPointContainerCloned.TailEndPoint);
                            }
                        }
                    }
                }
            }
        }

        /// <summary>
        /// Find node by unique name in cloned collection.
        /// </summary>
        /// <param name="node">The original node.</param>
        /// <param name="nodes">The cloned nodes collection.</param>
        /// <returns>The node having the given name.</returns>
        private Node FindNodeByName(Node node, NodeCollection nodes)
        {
            Node nodeToReturn = null;

            if (node != null)
            {
                ICompositeNode composite;
                NodeCollection children = new NodeCollection();
                string originalname = node.Name;
               
                foreach (Node clone in nodes)
                {
                    // compare names
                    if (originalname == clone.FullName)
                    {
                        nodeToReturn = clone;
                        break;
                    }

                    composite = clone as ICompositeNode;

                    // check node children
                    if (composite != null)
                    {
                        children.Clear();

                        for (int i = 0, length = composite.ChildCount; i < length; i++)
                        {
                            children.Add(composite.GetChild(i));
                        }

                        nodeToReturn = FindNodeByName(node, children);

                        if (nodeToReturn != null)
                            break;
                    }
                }
            }

            return nodeToReturn;
        }

        /// <summary>
        /// Validates given value.
        /// </summary>
        /// <param name="value">value to validate</param>
        /// <exception cref="System.InvalidCastException"/>
        protected override void OnValidate(object value)
        {
            if (!(value is Node))
                throw new InvalidCastException("value");
        }

        /// <summary>
        /// Validates given values.
        /// </summary>
        /// <param name="values">The values to validate.</param>
        /// <exception cref="System.InvalidCastException"/>
        protected override void OnValidate(ICollection values)
        {
            // Get collection enumerator
            IEnumerator enumerator = values.GetEnumerator();

            // Iterate through collection members checking their types
            while (enumerator.MoveNext())
            {
                if (!(enumerator.Current is Node))
                    throw new InvalidCastException("value");
            }
        }

        /// <summary>
        /// Raises ChangesComplete event.
        /// </summary>
        /// <param name="evtArgs">event args</param>
        protected override void RaiseChangesCompleteEvent(CollectionExEventArgs evtArgs)
        {
            if (!this.QuietMode && this.EventSink != null)
            {
                if (this.EventSink is ViewerEventSink)
                    ((ViewerEventSink)this.EventSink).RaiseSelectionChangedEvent(evtArgs);
                else
                    this.EventSink.RaiseNodesChangedEvent(evtArgs);
            }
        }

        /// <summary>
        /// Raise Changing event.
        /// </summary>
        /// <param name="evtArgs">event args</param>
        protected override void RaiseChangingEvent(CollectionExEventArgs evtArgs)
        {
            if (!this.QuietMode && this.EventSink != null)
            {
                if (this.EventSink is ViewerEventSink)
                    ((ViewerEventSink)this.EventSink).RaiseSelectionChangingEvent(evtArgs);
                else
                    this.EventSink.RaiseNodesChangingEvent(evtArgs);
            }
        }

        /// <summary>
        /// Updates the service references the collection members.
        /// </summary>
        /// <param name="evtArgs">The <see cref="Syncfusion.Windows.Forms.Diagram.CollectionExEventArgs"/> instance containing the event data.</param>
        protected override void UpdateServiceReferences(CollectionExEventArgs evtArgs)
        {
            base.UpdateServiceReferences(evtArgs);

            IEnumerator enumerator = evtArgs.Elements.GetEnumerator();
            Node nodeTemp;

            while (enumerator.MoveNext())
            {
                nodeTemp = enumerator.Current as Node;

                if (nodeTemp != null)
                {
                    if (evtArgs.ChangeType == CollectionExChangeType.Insert
                        || evtArgs.ChangeType == CollectionExChangeType.Set)
                    {
                        nodeTemp.Parent = this.Owner as ICompositeNode;
                    }
                }
            }
        }
        #endregion

        #region Class helper methods
        /// <summary>
        /// Generates the unique node name.
        /// </summary>
        /// <param name="node">The node.</param>
        /// <returns>The unique name.</returns>
        protected string GenerateUniqueName(Node node)
        {
            string strLayerNameToReturn = c_strNODE;
            uint unNameSuffix = 1;

            // iterate through layers collection 
            // looking for layer with the highest suffix
            while (Contains(strLayerNameToReturn))
            {
                strLayerNameToReturn = c_strNODE + unNameSuffix.ToString();
                unNameSuffix++;
            }

            return strLayerNameToReturn;
        }

        /// <summary>
        /// Finds the node by unique name.
        /// </summary>
        /// <param name="strNodeName">Name of the node.</param>
        /// <returns>The node.</returns>
        public Node FindNodeByName(string strNodeName)
        {
            Node nodeToReturn = null;

            // iterate through layers' collection
            // looking for layer with the name specified
            foreach (Node node in this.Members)
            {
                if (node.Name == strNodeName)
                {
                    nodeToReturn = node;
                    break;
                }
            }

            return nodeToReturn;
        }
        #endregion
    }
}
