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
    /// Represents common property for nodes.
    /// </summary>
    public interface ICommon
    {
        /// <summary>
        /// Gets or sets a value indicating whether this instance is selected.
        /// </summary>
        /// <value>
        /// <c>true</c> if this instance is selected; otherwise, <c>false</c>.
        /// </value>
        bool IsSelected
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the old ZIndex value.
        /// </summary>
        int OldZIndex { get; set; }

        /// <summary>
        /// Gets or sets the new ZIndex value.
        /// </summary>
        int NewZIndex { get; set; }
    }
}
