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
    /// Provides the interface to the page.
    /// </summary>
    public interface IDiagramPage
    {
        #region Properties

        /// <summary>
        /// Gets the selection list.
        /// </summary>
        /// <value>The selection list.</value>
        NodeCollection SelectionList
        {
            get;
        }
        
        /// <summary>
        /// Gets the actual width.
        /// </summary>
        /// <value>The actual width.</value>
        double ActualWidth
        {
            get;
        }

        /// <summary>
        /// Gets the actual height.
        /// </summary>
        /// <value>The actual height.</value>
        double ActualHeight
        {
            get;
        }

        /// <summary>
        /// Invalidates the measure.
        /// </summary>
        void InvalidateMeasure();

        #endregion
    }
}
