#region Copyright Syncfusion Inc. 2001 - 2014
// Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Any infringement will be prosecuted under
// applicable laws. 
#endregion
using System.Collections.Generic;
using System.Windows;
using System.Windows.Media;
using Syncfusion.Windows.Controls.Scroll;
using Syncfusion.Windows.GridCommon;

namespace Syncfusion.Windows.Controls.Cells
{
    /// <summary>
    /// Defines the interface for a cell renderer in a <see cref="VirtualizingCellsControl"/>.
    /// A default implementation of this interface is provided by the <see cref="CellRendererBase{S}"/> class
    /// from which you should derive custom cell renderer classes. There is however no dependency
    /// on CellRendererBase inside the VirtualizingCellsControl, the VirtualizingCellsControl
    /// base class only depends on this interface.
    /// </summary>
    public interface ICellRenderer
    {
#if !SILVERLIGHT
        /// <summary>
        /// Called from <see cref="VirtualizingCellsControl.OnRenderCell"/> to manually 
        /// render graphics that do not belong to live controls (e.g. static text).
        /// <see cref="CellRendererBase{S}"/> implements this method and calls the 
        /// virtual <see cref="CellRendererBase{S}.OnRender"/> method.
        /// </summary>
        /// <param name="dc">The drawing context.</param>
        /// <param name="rca">The render cell layout information.</param>
        void Render(DrawingContext dc, RenderCellArgs rca);
#endif

        /// <summary>
        /// Called <see cref="VirtualizingCellsControl.OnArrangeCell"/> to arrange the
        /// cells UIElement children. <see cref="CellRendererBase{S}"/> implements this 
        /// method and calls the virtual <see cref="CellRendererBase{S}.OnArrange"/> method.
        /// </summary>
        /// <param name="aca">The arange cell layout information.</param>
        void Arrange(ArrangeCellArgs aca);

        /// <summary>
        /// Called from <see cref="VirtualizingCellsControl.PrepareCellUIElements"/> to
        /// prepare the cells UIElement children.
        /// <see cref="CellRendererBase{S}"/> implements this method and calls the
        /// virtual <see cref="CellRendererBase{S}.OnPrepareUIElements"/> method.
        /// <see cref="VirtualizingCellRendererBase{T}"/> overrides this method and
        /// creates new UIElements and wires them with the parent cells control.
        /// </summary>
        /// <param name="aca">The arrange cell layout information.</param>
        /// <param name="uiElements">The UI elements.</param>
        /// <param name="canvas">The canvas to which any UIElement elements should be added.</param>
        void PrepareUIElements(ArrangeCellArgs aca, List<UIElement> uiElements, ScrollControlChildFrame canvas);

        /// <summary>
        /// This method is called after a cell is scrolled out of view. 
        /// <see cref="CellRendererBase{S}"/> implements this method and calls the
        /// virtual <see cref="CellRendererBase{S}.OnUnloadUIElements"/> method.
        /// <see cref="VirtualizingCellRendererBase{T}"/> overrides this method and
        /// creates either removes the cell renderer visuals from the parent canvas 
        /// or hide them and reuse it later in same canvas depending on whether
        /// <see cref="VirtualizingCellRendererBase{T}.AllowRecycle"/> was set.
        /// </summary>
        /// <param name="host">The host.</param>
        /// <param name="cellRowColumnIndex">Index of the cell row column.</param>
        /// <param name="visuals">The visuals.</param>
        void UnloadUIElements(VirtualizingCellsControl host, RowColumnIndex cellRowColumnIndex, CellUIElements visuals);

        /// <summary>
        /// This method is called from <see cref="CellMouseCaptureInfo"/> to
        /// take away mouse capture when context of mouse operation changes (e.g. from selecting
        /// text inside one cell to selecting multiple cells in a grid)<para/>
        /// <see cref="CellRendererBase{S}"/> implements this method and calls the
        /// virtual <see cref="CellRendererBase{S}.OnCancelMouseCapture"/> method.
        /// </summary>
        /// <param name="element">The UIElement child</param>
        void CancelMouseCapture(UIElement element);

        /// <summary>
        /// This method is called from <see cref="CellMouseCaptureInfo"/> to
        /// restore mouse capture when context of mouse operation changes back to original context.
        /// <para/>
        /// <see cref="CellRendererBase{S}"/> implements this method and calls the
        /// virtual <see cref="CellRendererBase{S}.OnRecaptureMouse"/> method.
        /// </summary>
        /// <param name="element">The UIElement child</param>
        void RecaptureMouse(UIElement element);

        /// <summary>
        /// Called from <see cref="VirtualizingCellsControl.ArrangeCellUIElements"/> to
        /// determine whether the parent control should unload visuals
        /// when the cell is scrolled out of the viewable area.
        /// </summary>
        /// <value><c>true</c> if visuals should be unloaded when scrolled out of view; otherwise, <c>false</c>.</value>
        bool UnloadUIElementsWhenScrolledOutOfView { get; }

        /// <summary>
        /// Determine if cell at given cells row and column index is associated with UIElement. If yes,
        /// reintialize the cells UIElement. You should implement this method in the cell renderer
        /// and redirect the call to <see cref="VirtualizingCellsControl.RefreshCellUIElementsContent"/>
        /// on the supplied <paramref name="cellsControl"/>. If you have a complex renderer 
        /// (e.g. a nested grid) for which there is no need to reinitalize contents you can
        /// override this method and do nothing. This usually is called as a response to an 
        /// earlier <see cref="VirtualizingCellsControl.InvalidateCell"/>.
        /// call.
        /// </summary>
        /// <param name="cellsControl">The cells control.</param>
        /// <param name="cellUIElements">The cell visuals.</param>
        /// <param name="rowColumnIndex">Index of the row column.</param>
        void RefreshCellUIElementsContent(VirtualizingCellsControl cellsControl, CellUIElements cellUIElements, RowColumnIndex rowColumnIndex);

        /// <summary>
        /// Hides the specified UIElement. You should implement this method in your renderer
        /// and call e.Arrange(new Rect(0, 0, 0, 0)); If this however causes problems then
        /// you can instead call e.Visibility = Visibility.Hidden;.
        /// Using Arrange has one benefit for example that capture state of a textbox does not
        /// get affected.
        /// </summary>
        /// <param name="e">The e.</param>
        void Hide(UIElement e);

    }

    public interface IAllowInvalidateCell
    {
        bool AllowInvalidateCell { get; set; }
    }

}
