#region Copyright Syncfusion Inc. 2001 - 2014
// Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Any infringement will be prosecuted under
// applicable laws. 
#endregion
using System;
using System.Collections.Generic;
using System.Windows;

#if !WinRT
using System.Windows.Media;
using Syncfusion.Windows.Controls.Scroll;
using Syncfusion.Windows.GridCommon;
using Syncfusion.Windows.ComponentModel;

namespace Syncfusion.Windows.Controls.Cells
#else
using Syncfusion.WinRT.ComponentModel;
using Syncfusion.WinRT.Controls.Scroll;
using Windows.Foundation;
using Windows.UI.Xaml;

namespace Syncfusion.WinRT.Controls.Cells
#endif
{
    /// <summary>
    /// The <see cref="CellRendererBase{S}"/> class provides a default implementation of 
    /// the <see cref="ICellRenderer"/> interface for a cell renderer in a 
    /// <see cref="VirtualizingCellsControl"/>.<para/>
    /// You should derive from this class to implement custom cell renderer classes. 
    /// There is however no dependency on CellRendererBase inside the 
    /// VirtualizingCellsControl, the VirtualizingCellsControl
    /// base class only depends on this interface.
    /// <para/>
    /// If you want to implement a renderer with support for live UIElement visuals 
    /// inside the cell you should derive from the <see cref="VirtualizingCellRendererBase{T}"/>
    /// or grid/treeview adapted VirtualizingCellRendererBase classes.
    /// </summary>
    /// <typeparam name="S">The type for render cell styles. The type must 
    /// implement <see cref="IRenderCellInfo"/></typeparam>
#if WinRT
    [ClassReference(IsReviewed = false)]
#endif
    public class CellRendererBase<S> : Disposable, ICellRenderer 
        where S : IRenderCellInfo
    {
        bool isInArrange = false;
        bool allowCancelMouseCapture = true;
        bool allowUnloadVisuals = true;


#if (!SILVERLIGHT && !WinRT)
        /// <summary>
        /// Called from <see cref="ICellRenderer.Render"/> to manually 
        /// render graphics that do not belong to live controls (e.g. static text).
        /// </summary>
        /// <param name="dc">The drawing context.</param>
        /// <param name="rca">The render cell layout information.</param>
        /// <param name="cellInfo">The cell style info.</param>
        protected virtual void OnRender(DrawingContext dc, RenderCellArgs rca, S cellInfo)
        {
        }
#endif

        /// <summary>
        /// Gets a value indicating whether this instance is processing <see cref="OnArrange"/>.
        /// </summary>
        /// <value>
        /// 	<c>true</c> if this instance is in arrange; otherwise, <c>false</c>.
        /// </value>
        public bool IsInArrange
        {
            get
            {
                return isInArrange;
            }
        }

        /// <summary>
        /// Called from <see cref="ICellRenderer.Arrange"/> to 
        /// prepare the cells UIElement children.
        /// <see cref="VirtualizingCellRendererBase{T}"/> overrides this method and
        /// creates new UIElements and wires them with the parent cells control.
        /// </summary>
        /// <param name="aca">The arange cell layout information.</param>
        /// <param name="uiElements">The UI elements.</param>
        /// <param name="canvas">The canvas to which any UIElement elements should be added.</param>
        /// <param name="cellInfo">The cell style info.</param>
        protected virtual void OnPrepareUIElements(ArrangeCellArgs aca, List<UIElement> uiElements, ScrollControlChildFrame canvas, S cellInfo)
        {
        }

        /// <summary>
        /// Called from <see cref="ICellRenderer.Arrange"/> to
        /// arrange the cells UIElement children.
        /// </summary>
        /// <param name="aca">The arange cell layout information.</param>
        /// <param name="cellInfo">The cell style info.</param>
        protected virtual void OnArrange(ArrangeCellArgs aca, S cellInfo)
        {
        }

        void ICellRenderer.SetBounds(UIElement el, Rect rect, bool forceMeasure, bool forceArrange)
        {
            SetBounds(el, rect, forceMeasure, forceArrange);
        }

        protected virtual void SetBounds(UIElement el, Rect rect, bool forceMeasure, bool forceArrange)
        {
        }


        /// <summary>
        /// Called from <see cref="ICellRenderer.UnloadUIElements"/> after a cell is scrolled out of view. 
        /// <see cref="VirtualizingCellRendererBase{T}"/> overrides this method and
        /// creates either removes the cell renderer visuals from the parent canvas 
        /// or hide them and reuse it later in same canvas depending on whether
        /// <see cref="VirtualizingCellRendererBase{T}.AllowRecycle"/> was set.
        /// </summary>
        /// <param name="host">The host.</param>
        /// <param name="cellRowColumnIndex">Index of the cell row column.</param>
        /// <param name="visuals">The visuals.</param>
        protected virtual void OnUnloadUIElements(VirtualizingCellsControl host, RowColumnIndex cellRowColumnIndex, CellUIElements visuals)
        {
            visuals.Unload(host);
        }

        #region ICellRenderer Members

#if (!SILVERLIGHT && !WinRT)
        void ICellRenderer.Render(DrawingContext dc, RenderCellArgs rca)
        {
            S style = (S) rca.CellInfo;
            IAllowInvalidateCell aic = style as IAllowInvalidateCell;
            if (aic != null)
            {
                bool b = aic.AllowInvalidateCell;
                aic.AllowInvalidateCell = false;
                OnRender(dc, rca, style);
                aic.AllowInvalidateCell = b;
            }
            else
                OnRender(dc, rca, style);
        }
#else
        private bool supportsRenderOptimization = false;
        /// <summary>
        /// Gets or sets whether the renderer supports rendering itself directly to the
        /// drawing context. When this is possible the UIElement will only be created
        /// when the user moves the mouse over the cell or if the UIElement is needed for
        /// other reasons, e.g. animate after change. The benefit of rendering directly to the 
        /// DrawingContext instead of creating the UIElement is a much improved scrolling 
        /// performance. The default value is false.
        /// </summary>
        public bool SupportsRenderOptimization
        {
            get { return supportsRenderOptimization; }
            set { supportsRenderOptimization = value; }
        }

        bool ICellRenderer.ShouldSupportOptimization()
        {
            return this.SupportsRenderOptimization;
        }
#endif

        void ICellRenderer.Arrange(ArrangeCellArgs aca)
        {
            isInArrange = true;

            try
            {
                OnArrange(aca, (S) aca.CellInfo);
            }
            catch (Exception)// ex)
            {
            }

            finally
            {
                isInArrange = false;
            }
        }

        void ICellRenderer.PrepareUIElements(ArrangeCellArgs aca, List<UIElement> uiElements, ScrollControlChildFrame canvas)
        {
            OnPrepareUIElements(aca, uiElements, canvas, (S) aca.CellInfo);
        }

        void ICellRenderer.UnloadUIElements(VirtualizingCellsControl host, RowColumnIndex cellRowColumnIndex, CellUIElements visuals)
        {
            OnUnloadUIElements(host, cellRowColumnIndex, visuals);
        }

        /// <summary>
        /// Gets or sets a value indicating whether the control supports canceling mouse capture.
        /// </summary>
        /// <value>
        /// 	<c>true</c> if control supports canceling mouse capture; otherwise, <c>false</c>.
        /// </value>
        public bool AllowCancelMouseCapture
        {
            get { return allowCancelMouseCapture; }
            set { allowCancelMouseCapture = value; }
        }

        void ICellRenderer.CancelMouseCapture(UIElement element)
        {
            if (AllowCancelMouseCapture)
                OnCancelMouseCapture(element);
        }

        /// <summary>
        /// Called from <see cref="ICellRenderer.CancelMouseCapture"/> to take away 
        /// mouse capture when context of mouse operation changes (e.g. from selecting
        /// text inside one cell to selecting multiple cells in a grid)
        /// </summary>
        /// <param name="element">The UIElement child</param>
        protected virtual void OnCancelMouseCapture(UIElement element)
        {
#if !WinRT
            element.ReleaseMouseCapture();
#endif
        }

        void ICellRenderer.RecaptureMouse(UIElement element)
        {
            OnRecaptureMouse(element);
        }

        /// <summary>
        /// Called from from <see cref="ICellRenderer.RecaptureMouse"/> to restore mouse 
        /// capture when context of mouse operation changes back to original context
        /// </summary>
        /// <param name="element">The UIElement child</param>
        protected virtual void OnRecaptureMouse(UIElement element)
        {
#if !WinRT
            element.CaptureMouse();
#endif
        }

        bool ICellRenderer.UnloadUIElementsWhenScrolledOutOfView
        {
            get { return UnloadVisualsWhenScrolledOutOfView; }
        }

        /// <summary>
        /// Called from <see cref="VirtualizingCellsControl.ArrangeCellUIElements"/> to
        /// determine whether the parent control should unload visuals
        /// when the cell is scrolled out of the viewable area.
        /// </summary>
        /// <value><c>true</c> if visuals should be unloaded when scrolled out of view; otherwise, <c>false</c>.</value>
        public bool UnloadVisualsWhenScrolledOutOfView
        {
            get
            {
                return this.allowUnloadVisuals;
            }
            set
            {
                this.allowUnloadVisuals = value;
            }
        }

        /// <summary>
        /// Determine if cell at given cells row and column index is associated with UIElement. If yes,
        /// reinitialize the cells UIElement. You should implement this method in the cell renderer
        /// and redirect the call to <see cref="VirtualizingCellsControl.RefreshCellUIElementsContent"/>
        /// on the supplied <paramref name="cellsControl"/>. If you have a complex renderer
        /// (e.g. a nested grid) for which there is no need to reinitalize contents you can
        /// override this method and do nothing. This usually is called as a response to an 
        /// earlier <see cref="VirtualizingCellsControl.InvalidateCell"/>.
        /// </summary>
        /// <param name="cellsControl">The cells control.</param>
        /// <param name="cellUIElements">The cell visuals.</param>
        /// <param name="rowColumnIndex">Index of the row column.</param>
        public virtual void RefreshCellUIElementsContent(VirtualizingCellsControl cellsControl, CellUIElements cellUIElements, RowColumnIndex rowColumnIndex)
        {
            cellsControl.RefreshCellUIElementsContent(rowColumnIndex);
        }

        /// <summary>
        /// Hides the specified UIElement. You should implement this method in your renderer
        /// and call e.Arrange(new Rect(0, 0, 0, 0)); If this however causes problems then
        /// you can instead call e.Visibility = Visibility.Hidden;.
        /// Using Arrange has one benefit for example that capture state of a textbox does not
        /// get affected.
        /// </summary>
        /// <param name="e">The e.</param>
        public virtual void Hide(UIElement e)
        {
            // I am using Arrange (or SetRenderBounds) instead of e.Visibility = Visibility.Collapsed; 
            // The reason is to avoid focus from being taken away from the current cell textbox and 
            // also avoid that focus moves to another cell.
#if (SILVERLIGHT || WinRT)
            Rect r = VisualContainer.GetRenderBounds(e);
            r.X += 10000;
            r.Y += 10000;
            VisualContainer.SetRenderBounds(e, r);
            e.Arrange(r);
#else
            // Could also add check for !IsMouseOperationOrigin(entry.Key) here if .Visibility would
            // be more efficient for other cells.
            e.Arrange(new Rect(0, 0, 0, 0));
#endif
        }

        #endregion
    }

}
