#region Copyright
// Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Any infringement will be prosecuted under
// applicable laws. 
#endregion

namespace Syncfusion.Windows.Diagram
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;

    /// <summary>
    /// Represents node collection properties.
    /// </summary>
    public interface INodeGroup
    {
        #region Properties

        /// <summary>
        /// Gets the ID.
        /// </summary>
        /// <value>The ID of the node.</value>
        Guid ID
        {
            get;
        }

        /// <summary>
        /// Gets or sets the parent ID.
        /// </summary>
        /// <value>The parent ID.</value>
        Guid ParentID
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets a value indicating whether this instance is grouped.
        /// </summary>
        /// <value>
        /// <c>true</c> if this instance is grouped; otherwise, <c>false</c>.
        /// </value>
        bool IsGrouped
        {
            get;
            set;
        }

        /// <summary>
        /// Gets the groups to which the INodeGroup objects belong.
        /// </summary>
        /// <value>The groups.</value>
        CollectionExt Groups { get; }

        /// <summary>
        /// Gets or sets the reference number of the INodeGroup objects. Used for serialization purposes..
        /// </summary>
        /// <value>The reference no.</value>
        int ReferenceNo { get; set; }

        #endregion
    }
}
