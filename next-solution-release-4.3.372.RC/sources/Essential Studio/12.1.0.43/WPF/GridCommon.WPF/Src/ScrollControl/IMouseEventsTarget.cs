#region Copyright Syncfusion Inc. 2001 - 2014
// Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Any infringement will be prosecuted under
// applicable laws. 
#endregion
using System.Windows;
using System.Windows.Input;
using Syncfusion.Windows.GridCommon;

namespace Syncfusion.Windows.Controls.Scroll
{
    /// <summary>
    /// This interface provides methods that the <see cref="ScrollControl"/> class can call
    /// to forward mouse events in the <see cref="ScrollControl.MouseEventListeners"/> collection.
    /// </summary>
    /// <value>The mouse event listeners.</value>
    public interface IMouseEventsTarget
    {
        /// <summary>
        /// Sets the host.
        /// </summary>
        /// <param name="host">The host.</param>
        void SetHost(FrameworkElement host);
       
        /// <summary>
        /// Invoked when an unhandled <see cref="System.Windows.Input.Mouse.MouseEnterEvent"/> attached event is raised on the host element. 
        /// </summary>
        /// <param name="e">The <see cref="System.Windows.Input.MouseEventArgs"/> instance containing the event data.</param>
        void OnMouseEnter(MouseEventArgs e);

        /// <summary>
        /// Invoked when an unhandled <see cref="System.Windows.Input.Mouse.MouseLeaveEvent"/> attached event is raised on the host element. 
        /// </summary>
        /// <param name="e">The <see cref="System.Windows.Input.MouseEventArgs"/> instance containing the event data.</param>
        void OnMouseLeave(MouseEventArgs e);
    
        /// <summary>
        /// Invoked when an unhandled <see cref="System.Windows.Input.Mouse.MouseDownEvent"/> attached event is raised on the host element. 
        /// </summary>
        /// <param name="e">The <see cref="System.Windows.Input.MouseButtonEventArgs"/> instance containing the event data.</param>
        void OnMouseDown(MouseButtonEventArgs e);
      
        /// <summary>
        /// Invoked when an unhandled <see cref="System.Windows.Input.Mouse.MouseMoveEvent"/> attached event is raised on the host element. 
        /// </summary>
        /// <param name="e">The <see cref="System.Windows.Input.MouseEventArgs"/> instance containing the event data.</param>
        void OnMouseMove(MouseEventArgs e);
     
        /// <summary>
        /// Invoked when an unhandled <see cref="System.Windows.Input.Mouse.MouseUpEvent"/> attached event is raised on the host element. 
        /// </summary>
        /// <param name="e">The <see cref="System.Windows.Input.MouseButtonEventArgs"/> instance containing the event data.</param>
        void OnMouseUp(MouseButtonEventArgs e);
        
        /// <summary>
        /// Invoked when an unhandled <see cref="System.Windows.Input.Mouse.MouseWheelEvent"/> attached event is raised on the host element. 
        /// </summary>
        /// <param name="e">The <see cref="System.Windows.Input.MouseWheelEventArgs"/> instance containing the event data.</param>
        void OnMouseWheel(MouseWheelEventArgs e);
       
        /// <summary>
        /// Invoked when an unhandled <see cref="System.Windows.Input.Mouse.PreviewMouseDownEvent"/> attached event is raised on the host element. 
        /// </summary>
        /// <param name="e">The <see cref="System.Windows.Input.MouseButtonEventArgs"/> instance containing the event data.</param>
        void OnPreviewMouseDown(MouseButtonEventArgs e);

        /// <summary>
        /// Invoked when an unhandled <see cref="System.Windows.Input.Mouse.PreviewMouseMoveEvent"/> attached event is raised on the host element. 
        /// </summary>
        /// <param name="e">The <see cref="System.Windows.Input.MouseEventArgs"/> instance containing the event data.</param>
        void OnPreviewMouseMove(MouseEventArgs e);

        /// <summary>
        /// Invoked when an unhandled <see cref="System.Windows.Input.Mouse.PreviewMouseUpEvent"/> attached event is raised on the host element. 
        /// </summary>
        /// <param name="e">The <see cref="System.Windows.Input.MouseButtonEventArgs"/> instance containing the event data.</param>
        void OnPreviewMouseUp(MouseButtonEventArgs e);

        /// <summary>
        /// Invoked when an unhandled <see cref="System.Windows.Input.Mouse.PreviewMouseWheelEvent"/> attached event is raised on the host element. 
        /// </summary>
        /// <param name="e">The <see cref="System.Windows.Input.MouseWheelEventArgs"/> instance containing the event data.</param>
        void OnPreviewMouseWheel(MouseWheelEventArgs e);

        /// <summary>
        /// Invoked when an unhandled <see cref="System.Windows.Input.Mouse.OnDrop"/> attached event is raised on the host element.
        /// </summary>
        /// <param name="e">The <see cref="System.Windows.DragEventArgs"/> instance containing the event data.</param>
        void OnDrop(DragEventArgs e);
    }


}
