#region Copyright Syncfusion Inc. 2001 - 2014
// Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Any infringement will be prosecuted under
// applicable laws. 
#endregion
using System.Windows;
using System;

#if !WinRT
namespace Syncfusion.Windows.Controls.Scroll
#else
using Windows.Foundation;

namespace Syncfusion.WinRT.Controls.Scroll
#endif
{
    /// <summary>
    /// This interface is used by <see cref="MouseControllerDispatcher"/>
    /// to support interaction with UIElements that grab mouse capture.
    /// When a mouse was pressed and a control grabs the mouse capture the
    /// MouseControllerDispatcher creates a capture context to allow
    /// the mouse to interact with the UIElement while inside the context
    /// but also to switch back to another context when the user
    /// drags the mouse out of the UIElement. 
    /// <para/> 
    /// An example scenerio is
    /// selection of cells inside a grid. While inside a textbox you can
    /// select text but when dragging the mouse outside the grid switches
    /// to selecting cells. When dragging the mouse back inside the textbox,
    /// the textbox gets the mouse capture again and you switch back
    /// to selecting text inside the cell.
    /// </summary>
#if WinRT
    [ClassReference(IsReviewed = false)]
#endif
    public interface ICaptureContext
    {
        /// <summary>
        /// Determines if the points belongs to the same element that this context was crreated for.
        /// </summary>
        /// <param name="mousePosition">The mouse position.</param>
        /// <returns></returns>
        bool PointInContext(Point mousePosition);

        /// <summary>
        /// Recaptures the mouse if the capture was taken away earlier because the user
        /// moved the mouse away from the UIElement and now has moved the mouse back into
        /// the UIElement.
        /// </summary>
        /// <returns></returns>
        Point RecaptureMouse();


        /// <summary>
        /// Cancel the mouse capture. 
        /// </summary>
        /// <returns>true if capture can be cancelled; false if context should not be changed.</returns>
        bool CancelMouseCapture();
    }
}
