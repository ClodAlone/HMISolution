#region Copyright Syncfusion Inc. 2001 - 2014
// Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Any infringement will be prosecuted under
// applicable laws. 
#endregion
namespace Syncfusion.Windows.Data
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;    
    using System.Collections.ObjectModel;
    using System.Collections;
    using Syncfusion.Linq;

    /// <summary>
    /// NodeEntry is the base class for the Grouping data structure used by <see cref="ICollectionViewAdv" /> interface. It exposes some base level
    /// details for the derived constructs to use.
    /// </summary>
    public class NodeEntry : IDisposable
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="NodeEntry"/> class.
        /// </summary>
        /// <param name="node">The node.</param>
        /// <param name="level">The level.</param>
        public NodeEntry(NodeEntry node, int level)
        {
            this.Parent = node;
            this.Level = level;
        }

        /// <summary>
        /// Releases unmanaged resources and performs other cleanup operations before the
        /// <see cref="NodeEntry"/> is reclaimed by garbage collection.
        /// </summary>
        ~NodeEntry()
        {
            this.Dispose(false);
        }

        /// <summary>
        /// Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
        /// </summary>
        public void Dispose()
        {
            this.Dispose(true);
        }

        /// <summary>
        /// Releases unmanaged and - optionally - managed resources
        /// </summary>
        /// <param name="disposing"><c>true</c> to release both managed and unmanaged resources; <c>false</c> to release only unmanaged resources.</param>
        protected virtual void Dispose(bool disposing)
        {
            if (disposing)
            {
                if (this.Parent != null)
                {
                    this.Parent = null;
                }
            }
        }

        /// <summary>
        /// Gets or sets the level.
        /// </summary>
        /// <value>The level.</value>
        public int Level
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the parent.
        /// </summary>
        /// <value>The parent.</value>
        public NodeEntry Parent
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets a value indicating whether this instance is groups.
        /// </summary>
        /// <value><c>true</c> if this instance is groups; otherwise, <c>false</c>.</value>
        public bool IsGroups
        {
            get;
            protected set;
        }

        /// <summary>
        /// Gets or sets a value indicating whether this instance is records.
        /// </summary>
        /// <value>
        /// 	<c>true</c> if this instance is records; otherwise, <c>false</c>.
        /// </value>
        public bool IsRecords
        {
            get;
            protected set;
        }
    }
}
