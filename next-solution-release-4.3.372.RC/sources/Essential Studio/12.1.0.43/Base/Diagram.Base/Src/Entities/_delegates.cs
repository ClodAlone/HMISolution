#region Copyright Syncfusion Inc. 2001 - 2014
// Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
//
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Re-distribution in any form is strictly
// prohibited. Any infringement will be prosecuted under applicable laws. 
#endregion

using System.Drawing;

namespace Syncfusion.Windows.Forms.Diagram
{
    /// <summary>
    /// Delegate used by PropertyContainers to update container node's info.
    /// </summary>
    /// <remarks>
    /// Used by Decorator to update container node's hittesting region.
    /// Used by AnchiringPrimitives to update their container node's refresh rect.
    /// </remarks>
    public delegate void UpdateCallback();

    /// <summary>
    /// Delegate used by handles to update container node
    /// with info of what handle moved and move offset.
    /// </summary>
    /// <param name="handleMoved">The handle.</param>
    /// <param name="szOffset">The offset size.</param>
    internal delegate void HandleMoved(IHandle handleMoved, SizeF szOffset);
}
