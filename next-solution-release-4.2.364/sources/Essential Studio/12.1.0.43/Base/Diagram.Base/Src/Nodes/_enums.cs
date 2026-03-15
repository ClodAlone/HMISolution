#region Copyright Syncfusion Inc. 2001 - 2014
// Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
//
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Re-distribution in any form is strictly
// prohibited. Any infringement will be prosecuted under applicable laws. 
#endregion

namespace Syncfusion.Windows.Forms.Diagram
{
    /// <summary>
    /// Specifies an activation style for the <see cref="Syncfusion.Windows.Forms.Diagram.ControlNode"/> node type.
    /// </summary>
    public enum ActivateStyle
    {
        /// <summary>
        /// Specifies that the Control will not be activated.
        /// </summary>
        None,

        /// <summary>
        /// Specifies that clicking a ControlNode will activate the Control hosted in it.
        /// </summary>
        Click,

        /// <summary>
        /// Specifies that clicking a selected ControlNode will activate the Control hosted in it.
        /// </summary>
        SelectedClick,

        /// <summary>
        /// Specifies that double clicking a ControlNode will activate the Control hosted in it.
        /// </summary>
        DoubleClick,

        /// <summary>
        /// Specifies that clicking a ControlNode will activate the child Control and pass the click event to it. 
        /// </summary>
        ClickPassThrough
    }
}
