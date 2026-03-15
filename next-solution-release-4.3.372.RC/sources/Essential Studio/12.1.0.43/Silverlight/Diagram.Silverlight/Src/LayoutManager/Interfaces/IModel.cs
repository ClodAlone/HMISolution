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
    using System.Runtime.Serialization;
    using System.Text;
 
    /// <summary>
    /// Provides an interface to the model.
    /// </summary>
    public interface IModel
    {
        #region Properties
        /// <summary>
        /// Gets or sets the layout root.
        /// </summary>
        /// <value>The layout root.</value>
        IShape LayoutRoot { get; set; }

        /// <summary>
        /// Gets the connections.
        /// </summary>
        /// <value>The connections.</value>
        CollectionExt Connections { get; }

        /// <summary>
        /// Gets the shapes.
        /// </summary>
        /// <value>The shapes.</value>
        CollectionExt Nodes { get; }

        #endregion
    }
}
