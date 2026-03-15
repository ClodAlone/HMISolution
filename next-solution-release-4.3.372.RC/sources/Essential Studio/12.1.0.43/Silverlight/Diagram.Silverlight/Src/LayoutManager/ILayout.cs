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
    using System.Windows;

    /// <summary>
    /// Represents the layout. Provides properties for creating the layout.
    /// </summary>
    public interface ILayout
    {
        #region Properties

        /// <summary>
        /// Gets or sets the model.
        /// </summary>
        /// <value>The model.</value>
        DiagramModel Model
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the bounds.
        /// </summary>
        /// <value>The bounds.</value>
        Thickness Bounds { get; set; }

        /// <summary>
        /// Gets or sets the Layoutbounds.
        /// </summary>
        /// <value>The bounds value is   Rect  .</value>
        Rect LayoutBounds { get; set; }
        /// <summary>
        /// Gets or sets the center.
        /// </summary>
        /// <value>The center.</value>
        Point Center { get; set; }

        #endregion
    }
}
