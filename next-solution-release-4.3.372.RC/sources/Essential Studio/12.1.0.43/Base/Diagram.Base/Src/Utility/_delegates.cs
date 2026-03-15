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
    /// Move callback
    /// </summary>
    /// <param name="fX">X value</param>
    /// <param name="fY">Y value</param>
    public delegate void MoveCallback(float fX, float fY);

    /// <summary>
    /// Size callback
    /// </summary>
    /// <param name="szOld">Old size</param>
    /// <param name="szNew">New size</param>
    public delegate void SizeCallback(SizeF szOld, SizeF szNew);

    /// <summary>
    /// Pin offset call back.
    /// </summary>
    /// <param name="szOldPinOffset">Old pin offset</param>
    /// <param name="szNewPinOffset">New pin offset</param>
    public delegate void PinOffsetCallback(SizeF szOldPinOffset, SizeF szNewPinOffset);
}
