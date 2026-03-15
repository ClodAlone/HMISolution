// <copyright file="ChildDataParser.cs" company="Syncfusion">
// Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Any infringement will be prosecuted under
// applicable laws. 
// </copyright>

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.ComponentModel;
using System.Windows.Data;
using System.Windows.Controls.Primitives;

namespace Syncfusion.Windows.Diagram
{
    /// <summary>
    /// Converts collection of objects to Hierarchical Data format.
    /// </summary>
    internal class ChildDataParser : TreeViewItem
    {
        internal DiagramModel dm;
        internal Node node;

        #region Initialization

        /// <summary>
        /// Initializes a new instance of the <see cref="ChildDataParser"/> class.
        /// </summary>
        public ChildDataParser()
        {
            this.ChildItems = new List<ChildDataParser>();
            this.IsExpanded = true;
        }


        /// <summary>
        /// Initializes a new instance of the <see cref="ChildDataParser"/> class.
        /// </summary>
        public ChildDataParser(DiagramModel model)
        {
            this.ChildItems = new List<ChildDataParser>();
            this.IsExpanded = true;
            this.dm = model;
        }

        #endregion

        #region  Properties

        /// <summary>
        /// Gets or sets the child items.
        /// </summary>
        /// <value>The child items.</value>
        public List<ChildDataParser> ChildItems { get; set; }

        #endregion

        #region Properties

        /// <summary>
        /// Creates or identifies the element that is used to display the given item.
        /// </summary>
        /// <returns>The element that is used to display the given item.</returns>
        protected override DependencyObject GetContainerForItemOverride()
        {
            base.GetContainerForItemOverride();
            ChildDataParser newItem = new ChildDataParser(dm);
            this.ChildItems.Add(newItem);
                newItem.PropertyChanged -= new PropertyChangedEventHandler(ch_PropertyChanged);
                newItem.PropertyChanged += new PropertyChangedEventHandler(ch_PropertyChanged);
            return newItem;
        }

        void ch_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs("ChildChanged"));
            }
        }

        protected override void OnItemsChanged(System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
        {
            base.OnItemsChanged(e);
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs("ChildChanged"));
            }
        }

        private bool m_Replace = false;

        protected override void OnItemsSourceChanged(System.Collections.IEnumerable oldValue, System.Collections.IEnumerable newValue)
        {
            if (oldValue != null && newValue != null)
            {
                m_Replace = true;
            }
            base.OnItemsSourceChanged(oldValue, newValue);
        }

        protected override void ClearContainerForItemOverride(DependencyObject element, object item)
        {
            base.ClearContainerForItemOverride(element, item);
            if (element is ChildDataParser)
            {
                ChildDataParser cdp = element as ChildDataParser;
                RemoveChildItems(cdp);
                dm.Nodes.Remove(cdp.node);
                ChildItems.Remove(cdp);
            }
        }

        protected override void PrepareContainerForItemOverride(DependencyObject element, object item)
        {
            //if (!m_Replace)
            {
                Node cnode = new Node();
                if (node == null && (dm.LayoutType == LayoutType.DirectedTreeLayout || dm.LayoutType == LayoutType.BowtieLayout))
                {
                    if (dm.Nodes.Contains(dm.LayoutRoot))
                    {
                        node = dm.LayoutRoot as Node;
                    }
                                
                }
                if (element is ChildDataParser)
                {
                    ChildDataParser child = element as ChildDataParser;
                    if (!child.m_Replace)
                    {
                        cnode.Content = item;
                        if (ItemTemplate != null && ItemTemplate.HasContent)
                        {
                            cnode.ContentTemplate = this.ItemTemplate;
                        }
                        child.node = cnode;
                        cnode.Model = dm;
                        dm.Nodes.Add(cnode);

                        Node.setINodeBinding(cnode, cnode.Content as INode);

                        if (node != null)
                        {
                            LineConnector ortho;
                            if (dm != null && dm.dc != null && dm.dc.View != null)
                            {
                                ortho = new LineConnector(dm.dc.View);
                            }
                            else
                            {
                                ortho = new LineConnector(null);
                            }
                            ortho.HeadNode = this.node;
                            ortho.TailNode = cnode;
                            if (this.node != null)
                            {
                                cnode.PxOffsetX = this.node.PxOffsetX;
                                cnode.PxOffsetY = this.node.PxOffsetY;
                            }
                            ortho.Content = node.Content;
                            dm.Connections.Add(ortho);
                        }
                    }
                    else
                    {
                        cnode = child.node;
                        cnode.Content = item;
                        if (ItemTemplate != null && ItemTemplate.HasContent)
                        {
                            cnode.ContentTemplate = this.ItemTemplate;
                        }
                        Node.setINodeBinding(cnode, cnode.Content as INode);
                        child.m_Replace = false;
                    }
                }
            }
            base.PrepareContainerForItemOverride(element, item);
        }

        private void RemoveChildItems(ChildDataParser childDataParser)
        {
            foreach (ChildDataParser child in childDataParser.ChildItems)
            {
                RemoveChildItems(child);
                if (child.node != null)
                {
                    dm.Nodes.Remove(child.node);
                }
            }
            childDataParser.ChildItems.Clear();
        }        

        internal event PropertyChangedEventHandler PropertyChanged;

        #endregion

    }
}
