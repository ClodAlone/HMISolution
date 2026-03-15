// <copyright file="DataParser.cs" company="Syncfusion">
// Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Any infringement will be prosecuted under
// applicable laws. 
// </copyright>

namespace Syncfusion.Windows.Diagram
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Windows;
    using System.Windows.Controls;

    /// <summary>
    /// Converts collection of objects to Hierarchical Data format.
    /// </summary>
    internal class DataParser : TreeView
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="DataParser"/> class.
        /// </summary>
        public DataParser()
        {
            this.ChildItems = new List<ChildDataParser>();
        }

        /// <summary>
        /// Gets or sets the child items.
        /// </summary>
        /// <value>The child items.</value>
        public List<ChildDataParser> ChildItems { get; set; }

        /// <summary>
        /// Creates or identifies the element that is used to display the given item.
        /// </summary>
        /// <returns>The element that is used to display the given item.</returns>
        protected override DependencyObject GetContainerForItemOverride()
        {
            ChildDataParser newItem = new ChildDataParser();
            this.ChildItems.Add(newItem);
            return newItem;
        }
        #region Initialization

        #endregion

        #region  Properties

        #endregion

        #region Implementation

        #endregion
    }
}
