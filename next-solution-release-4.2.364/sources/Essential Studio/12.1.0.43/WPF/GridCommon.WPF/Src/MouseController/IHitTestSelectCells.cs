#region Copyright Syncfusion Inc. 2001 - 2014
// Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Any infringement will be prosecuted under
// applicable laws. 
#endregion
using System;
using System.Windows;

namespace Syncfusion.Windows.Controls.Scroll
{
    /// <summary>
    /// This interface can optionially be implemented by a cell renderer.
    /// When the user presses or release the mouse button <see cref="MouseControllerDispatcher"/> checks whether the mouse was pressed
    /// directly over a renderer (in such case MouseControllerEventArgs.DirectlyOverRenderer should have been set). If the interface is implemented
    /// the MouseDown and MouseUp methods are called. 
    /// <para/>
    /// The grids cell renderers
    /// implement this interface to move the current cell to the cell
    /// when the mouse is clicked inside the cells live UIElement visual.
    /// </summary>
    public interface IHitTestSelectCells
    {
        //bool AllowHitTest(MouseControllerEventArgs mouseEventArgs, IMouseController controller);

        /// <summary>
        /// MouseDown is called when MouseControllerDispatcher.HitTest did not
        /// return a controller to be set as ActiveController and the mouse
        /// was pressed over a UIElement inside a cell which hosts this
        /// renderer.
        /// </summary>
        /// <param name="owner">The owner.</param>
        /// <param name="mouseControllerEventArgs">The <see cref="MouseControllerEventArgs"/> instance containing the event data.</param>
        void MouseDown(FrameworkElement owner, MouseControllerEventArgs mouseControllerEventArgs);


        // <summary>
        // MouseUp is called when MouseControllerDispatcher.HitTest when
        // ActiveController is null and the mouse
        // was release over a UIElement inside a cell which hosts this
        // renderer.
        // </summary>
        // <param name="owner">The owner.</param>
        // <param name="mouseControllerEventArgs">The <see cref="MouseControllerEventArgs"/> instance containing the event data.</param>
        //void MouseUp(FrameworkElement owner, MouseControllerEventArgs mouseControllerEventArgs);
    }
}
