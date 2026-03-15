// <copyright file="DataParser.cs" company="Syncfusion">
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

namespace Syncfusion.Windows.Diagram
{
    /// <summary>
    /// Converts collection of objects to Hierarchical Data format.
    /// </summary>
    internal class DataParser : TreeView
    {
        internal DiagramModel dm;

        #region Initialization

        /// <summary>
        /// Initializes a new instance of the <see cref="DataParser"/> class.
        /// </summary>
        public DataParser()
        {
            this.ChildItems = new List<ChildDataParser>();
        }

        public DataParser(DiagramModel model)
        {
            this.ChildItems = new List<ChildDataParser>();
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

        #region Implementation
        
        /// <summary>
        /// Creates or identifies the element that is used to display the given item.
        /// </summary>
        /// <returns>The element that is used to display the given item.</returns>
        protected override DependencyObject GetContainerForItemOverride()
        {
            base.GetContainerForItemOverride();
            ChildDataParser newItem = new ChildDataParser(dm);
            this.ChildItems.Add(newItem);
            newItem.PropertyChanged -= new System.ComponentModel.PropertyChangedEventHandler(c_PropertyChanged);
            newItem.PropertyChanged += new System.ComponentModel.PropertyChangedEventHandler(c_PropertyChanged);
            return newItem;
        }

        void c_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            if (dm != null && dm.dc != null && dm.dc.View != null && dm.init)
            {
                if (PropertyChanged != null)
                {
                    PropertyChanged(sender, e);
                }
            }
        }

        protected override void OnItemsChanged(System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
        {
            base.OnItemsChanged(e);
           
            if (dm != null && dm.dc != null && dm.dc.View != null && dm.init)
            {
                if (PropertyChanged != null)
                {
                    PropertyChanged(this, new PropertyChangedEventArgs("ChildChanged"));
                }
            }
        }

        internal event PropertyChangedEventHandler PropertyChanged;

        #endregion
    }
}
