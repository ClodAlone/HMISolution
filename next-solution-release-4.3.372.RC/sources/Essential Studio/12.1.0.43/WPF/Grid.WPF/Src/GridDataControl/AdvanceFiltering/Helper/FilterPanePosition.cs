#region Copyright Syncfusion Inc. 2001 - 2014
// Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Any infringement will be prosecuted under
// applicable laws. 
#endregion
namespace Syncfusion.Windows.Controls.Grid
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Windows;
    using Syncfusion.Linq;
    using System.Windows.Data;
    using System.Windows.Controls;
    using System.ComponentModel;
    using System.Windows.Controls.Primitives;
    using Syncfusion.Windows.Data;
    using Syncfusion.Windows.Shared;
    using System.Collections;
    using System.Collections.ObjectModel;

    public class FilterPanePosition
    {

        #region ctor

        /// <summary>
        /// Initializes a new instance of the <see cref="FilterPanePosition"/> class.
        /// </summary>
        public FilterPanePosition()
        {

        }

        #endregion

        #region Variables

        CustomPopupPlacementCallback _customPlacementCallback;

        #endregion

        #region Properties

        /// <summary>
        /// Gets or sets a value indicating whether this instance has callback.
        /// </summary>
        /// <value>
        /// 	<c>true</c> if this instance has callback; otherwise, <c>false</c>.
        /// </value>
        internal bool HasCallback { get; set; }

        /// <summary>
        /// Gets or sets the filter pane placement.
        /// </summary>
        /// <value>The filter pane placement.</value>
        public PlacementMode FilterPanePlacement { get; set; }

        /// <summary>
        /// Gets or sets the horizontal offset.
        /// </summary>
        /// <value>The horizontal offset.</value>
        public double HorizontalOffset { get; set; }

        /// <summary>
        /// Gets or sets the vertical offset.
        /// </summary>
        /// <value>The vertical offset.</value>
        public double VerticalOffset { get; set; }

        /// <summary>
        /// Gets or sets the custom placement callback.
        /// </summary>
        /// <value>The custom placement callback.</value>
        public CustomPopupPlacementCallback CustomPlacementCallback
        {
            get
            {
                return _customPlacementCallback;
            }
            set
            {
                _customPlacementCallback = value;
                HasCallback = true;
            }
        }

        #endregion

    }
}
