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
    /// Contains a list of nested records for each <see cref="Syncfusion.Windows.Data.RecordEntry"/> and nested <see cref="ICollectionViewAdv" /> instance.
    /// </summary>
    public class NestedRecordEntry : NodeEntry
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="NestedRecordEntry"/> class.
        /// </summary>
        /// <param name="parent">The parent.</param>
        /// <param name="level">The level.</param>
        public NestedRecordEntry(NodeEntry parent, int level)
            : base(parent, level)
        {
        }

        /// <summary>
        /// Gets or sets the nested level.
        /// </summary>
        /// <value>The nested level.</value>
        public int NestedLevel
        {
            get;
            set;
        }

        private ICollectionViewAdv nestedView;
        /// <summary>
        /// Gets or sets the view.
        /// </summary>
        /// <value>The view.</value>
        public ICollectionViewAdv View
        {
            get
            {
                return this.nestedView;
            }
            set
            {
                this.nestedView = value;
            }
        }

        /// <summary>
        /// Gets the nested records.
        /// </summary>
        /// <value>The nested records.</value>
        public IList<RecordEntry> NestedRecords
        {
            get
            {
                if (this.nestedView != null)
                {
                    return this.nestedView.Records;
                }
                return null;
            }
        }
    }
}
