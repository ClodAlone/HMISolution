#region Copyright
// Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Any infringement will be prosecuted under
// applicable laws. 
#endregion

using System.Windows;

namespace Syncfusion.Windows.Tools.Controls 
{
    /// <summary>
    /// Identifies the Interface IRibbonSelector.
    /// </summary>
	internal interface IRibbonSelector
	{
        /// <summary>
        /// Called when [item selected].
        /// </summary>
        /// <param name="item">The item.</param>
		void OnItemSelected(UIElement item);

        /// <summary>
        /// Called when [item clicked].
        /// </summary>
        /// <param name="item">The item.</param>
		void OnItemClicked(UIElement item);
	}

    /// <summary>
    /// Identifies the Interface IRibbonTabItemSelector.
    /// </summary>
	internal interface IRibbonTabItemSelector
	{
        /// <summary>
        /// Called when [double click].
        /// </summary>
        /// <param name="item">The item.</param>
		void OnDoubleClick(TabButton item);

        /// <summary>
        /// Called when [mouse down].
        /// </summary>
        /// <param name="item">The item.</param>
		void OnMouseDown(TabButton item);
	}
}
