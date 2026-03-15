//-------------------------------------------------------------------------------------------------
// <copyright file="GridControlBase.cs" company="syncfusion">
//  Copyright (c) Syncfusion Inc. 2001 - 2014. All rights reserved.
//  Use of this code is subject to the terms of our license.
//  A copy of the current license can be obtained at any time by e-mailing
//  licensing@syncfusion.com. Re-distribution in any form is strictly
//  prohibited. Any infringement will be prosecuted under applicable laws. 
// </copyright>
//-------------------------------------------------------------------------------------------------

using System;
using System.Collections;
using System.ComponentModel;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Windows.Forms;
using System.Runtime.InteropServices;
using System.Text;
using System.Security;
using System.Security.Permissions;
using System.Collections.Generic;
using Syncfusion.ComponentModel;
using Syncfusion.Drawing;
using Syncfusion.Diagnostics;
using Syncfusion.Styles;
using Syncfusion.Windows.Forms;
////using Syncfusion.Windows.Forms.Grid.GridInternal;

namespace Syncfusion.Windows.Forms.Grid
{
    /// <summary>
    /// Defines an interface for an object that handles Ole Drag Drop events raised by <see cref="GridControlBase"/> objects.
    /// </summary>
    public interface IGridOleDragDropEventsTarget
    {
        /// <summary>
        /// Occurs when a drag-and-drop operation is completed and before <see cref="Control.DragDrop"/> event is raised.
        /// </summary>
        /// <param name="e"><see cref="DragEventArgs"/> containing data related to this event</param>
        void OnDragDrop(DragEventArgs e);

        /// <summary>
        /// Occurs when an object is dragged into the control's bounds and before <see cref="Control.DragEnter"/> event is raised.
        /// </summary>
        /// <param name="e"><see cref="DragEventArgs"/> containing data related to this event</param>
        void OnDragEnter(DragEventArgs e);

        /// <summary>
        /// Occurs when an object is dragged out of the control's bounds and before <see cref="Control.DragLeave"/> event is raised.
        /// </summary>
        /// <param name="e"><see cref="DragEventArgs"/> containing data related to this event</param>
        void OnDragLeave(EventArgs e);

        /// <summary>
        /// Occurs when an object is dragged over the control's bounds and before <see cref="Control.DragOver"/> event is raised.
        /// </summary>
        /// <param name="e"><see cref="DragEventArgs"/> containing data related to this event</param>
        void OnDragOver(DragEventArgs e);
    }

    /// <summary>
    /// Defines an interface for an object that handles events raised by <see cref="GridControlBase"/> objects.
    /// </summary>
    public interface IGridControlBaseEventsTarget : IDisposable
    {
        /// <copyfrom cref="GridControlBase.CellButtonClicked"/>
        /// <summary>See <see cref="GridControlBase.CellButtonClicked"/> in the <see cref="GridControlBase"/> class for information.</summary>
        void OnCellButtonClicked(GridCellButtonClickedEventArgs e);

        /// <copyfrom cref="GridControlBase.CellCancelMode"/>
        /// <summary>See <see cref="GridControlBase.CellCancelMode"/> in the <see cref="GridControlBase"/> class for information.</summary>
        void OnCellCancelMode(GridCellMouseEventArgs e);

        /// <copyfrom cref="GridControlBase.CellClick"/>
        /// <summary>See <see cref="GridControlBase.CellClick"/> in the <see cref="GridControlBase"/> class for information.</summary>
        void OnCellClick(GridCellClickEventArgs e);

        /// <copyfrom cref="GridControlBase.CellCursor"/>
        /// <summary>See <see cref="GridControlBase.CellCursor"/> in the <see cref="GridControlBase"/> class for information.</summary>
        void OnCellCursor(GridCellCursorEventArgs e);

        /// <copyfrom cref="GridControlBase.QueryScrollCellInView"/>
        /// <summary>See <see cref="GridControlBase.QueryScrollCellInView"/> in the <see cref="GridControlBase"/> class for information.</summary>
        void OnQueryScrollCellInView(GridQueryScrollCellInViewEventArgs e);

        /// <copyfrom cref="GridControlBase.CellDoubleClick"/>
        /// <summary>See <see cref="GridControlBase.CellDoubleClick"/> in the <see cref="GridControlBase"/> class for information.</summary>
        void OnCellDoubleClick(GridCellClickEventArgs e);

        /// <copyfrom cref="GridControlBase.CellDrawn"/>
        /// <summary>See <see cref="GridControlBase.CellDrawn"/> in the <see cref="GridControlBase"/> class for information.</summary>
        void OnCellDrawn(GridDrawCellEventArgs e);

        /// <copyfrom cref="GridControlBase.CellHitTest"/>
        /// <summary>See <see cref="GridControlBase.CellHitTest"/> in the <see cref="GridControlBase"/> class for information.</summary>
        void OnCellHitTest(GridCellHitTestEventArgs e);

        /// <copyfrom cref="GridControlBase.CellMouseDown"/>
        /// <summary>See <see cref="GridControlBase.CellMouseDown"/> in the <see cref="GridControlBase"/> class for information.</summary>
        void OnCellMouseDown(GridCellMouseEventArgs e);

        /// <copyfrom cref="GridControlBase.CellMouseHover"/>
        /// <summary>See <see cref="GridControlBase.CellMouseHover"/> in the <see cref="GridControlBase"/> class for information.</summary>
        void OnCellMouseHover(GridCellMouseEventArgs e);

        /// <copyfrom cref="GridControlBase.CellMouseHoverEnter"/>
        /// <summary>See <see cref="GridControlBase.CellMouseHoverEnter"/> in the <see cref="GridControlBase"/> class for information.</summary>
        void OnCellMouseHoverEnter(GridCellMouseEventArgs e);

        /// <copyfrom cref="GridControlBase.CellMouseHoverLeave"/>
        /// <summary>See <see cref="GridControlBase.CellMouseHoverLeave"/> in the <see cref="GridControlBase"/> class for information.</summary>
        void OnCellMouseHoverLeave(GridCellMouseEventArgs e);

        /// <copyfrom cref="GridControlBase.CellMouseMove"/>
        /// <summary>See <see cref="GridControlBase.CellMouseMove"/> in the <see cref="GridControlBase"/> class for information.</summary>
        void OnCellMouseMove(GridCellMouseEventArgs e);

        /// <copyfrom cref="GridControlBase.CellMouseUp"/>
        /// <summary>See <see cref="GridControlBase.CellMouseUp"/> in the <see cref="GridControlBase"/> class for information.</summary>
        void OnCellMouseUp(GridCellMouseEventArgs e);

        /// <copyfrom cref="GridControlBase.CheckBoxClick"/>
        /// <summary>See <see cref="GridControlBase.CheckBoxClick"/> in the <see cref="GridControlBase"/> class for information.</summary>
        void OnCheckBoxClick(GridCellClickEventArgs e);

        /// <copyfrom cref="GridControlBase.CurrentCellAcceptedChanges"/>
        /// <summary>See <see cref="GridControlBase.CurrentCellAcceptedChanges"/> in the <see cref="GridControlBase"/> class for information.</summary>
        void OnCurrentCellAcceptedChanges(CancelEventArgs e);

        /// <copyfrom cref="GridControlBase.CurrentCellActivated"/>
        /// <summary>See <see cref="GridControlBase.CurrentCellActivated"/> in the <see cref="GridControlBase"/> class for information.</summary>
        void OnCurrentCellActivated(EventArgs e);

        /// <copyfrom cref="GridControlBase.CurrentCellActivateFailed"/>
        /// <summary>See <see cref="GridControlBase.CurrentCellActivateFailed"/> in the <see cref="GridControlBase"/> class for information.</summary>
        void OnCurrentCellActivateFailed(GridCurrentCellActivateFailedEventArgs e);

        /// <copyfrom cref="GridControlBase.CurrentCellActivating"/>
        /// <summary>See <see cref="GridControlBase.CurrentCellActivating"/> in the <see cref="GridControlBase"/> class for information.</summary>
        void OnCurrentCellActivating(GridCurrentCellActivatingEventArgs e);

        /// <copyfrom cref="GridControlBase.CurrentCellChanged"/>
        /// <summary>See <see cref="GridControlBase.CurrentCellChanged"/> in the <see cref="GridControlBase"/> class for information.</summary>
        void OnCurrentCellChanged(EventArgs e);

        /// <copyfrom cref="GridControlBase.CurrentCellChanging"/>
        /// <summary>See <see cref="GridControlBase.CurrentCellChanging"/> in the <see cref="GridControlBase"/> class for information.</summary>
        void OnCurrentCellChanging(CancelEventArgs e);

        /// <copyfrom cref="GridControlBase.CurrentCellCloseDropDown"/>
        /// <summary>See <see cref="GridControlBase.CurrentCellCloseDropDown"/> in the <see cref="GridControlBase"/> class for information.</summary>
        void OnCurrentCellCloseDropDown(PopupClosedEventArgs e);

        /// <copyfrom cref="GridControlBase.CurrentCellConfirmChangesFailed"/>
        /// <summary>See <see cref="GridControlBase.CurrentCellConfirmChangesFailed"/> in the <see cref="GridControlBase"/> class for information.</summary>
        void OnCurrentCellConfirmChangesFailed(EventArgs e);

        /// <copyfrom cref="GridControlBase.CurrentCellControlDoubleClick"/>
        /// <summary>See <see cref="GridControlBase.CurrentCellControlDoubleClick"/> in the <see cref="GridControlBase"/> class for information.</summary>
        void OnCurrentCellControlDoubleClick(ControlEventArgs e);

        /// <copyfrom cref="GridControlBase.CurrentCellControlGotFocus"/>
        /// <summary>See <see cref="GridControlBase.CurrentCellControlGotFocus"/> in the <see cref="GridControlBase"/> class for information.</summary>
        void OnCurrentCellControlGotFocus(ControlEventArgs e);

        /// <copyfrom cref="GridControlBase.CurrentCellControlKeyMessage"/>
        /// <summary>See <see cref="GridControlBase.CurrentCellControlKeyMessage"/> in the <see cref="GridControlBase"/> class for information.</summary>
        void OnCurrentCellControlKeyMessage(GridCurrentCellControlKeyMessageEventArgs e);

        /// <copyfrom cref="GridControlBase.CurrentCellControlLostFocus"/>
        /// <summary>See <see cref="GridControlBase.CurrentCellControlLostFocus"/> in the <see cref="GridControlBase"/> class for information.</summary>
        void OnCurrentCellControlLostFocus(ControlEventArgs e);

        /// <copyfrom cref="GridControlBase.CurrentCellDeactivated"/>
        /// <summary>See <see cref="GridControlBase.CurrentCellDeactivated"/> in the <see cref="GridControlBase"/> class for information.</summary>
        void OnCurrentCellDeactivated(GridCurrentCellDeactivatedEventArgs e);

        /// <copyfrom cref="GridControlBase.CurrentCellDeactivateFailed"/>
        /// <summary>See <see cref="GridControlBase.CurrentCellDeactivateFailed"/> in the <see cref="GridControlBase"/> class for information.</summary>
        void OnCurrentCellDeactivateFailed(EventArgs e);

        /// <copyfrom cref="GridControlBase.CurrentCellDeactivating"/>
        /// <summary>See <see cref="GridControlBase.CurrentCellDeactivating"/> in the <see cref="GridControlBase"/> class for information.</summary>
        void OnCurrentCellDeactivating(CancelEventArgs e);

        /// <copyfrom cref="GridControlBase.CurrentCellDeleting"/>
        /// <summary>See <see cref="GridControlBase.CurrentCellDeleting"/> in the <see cref="GridControlBase"/> class for information.</summary>
        void OnCurrentCellDeleting(CancelEventArgs e);

        /// <copyfrom cref="GridControlBase.CurrentCellEditingComplete"/>
        /// <summary>See <see cref="GridControlBase.CurrentCellEditingComplete"/> in the <see cref="GridControlBase"/> class for information.</summary>
        void OnCurrentCellEditingComplete(EventArgs e);

        /// <copyfrom cref="GridControlBase.CurrentCellInitializeControlText"/>
        /// <summary>See <see cref="GridControlBase.CurrentCellInitializeControlText"/> in the <see cref="GridControlBase"/> class for information.</summary>
        void OnCurrentCellInitializeControlText(GridCurrentCellInitializeControlTextEventArgs e);

        /// <copyfrom cref="GridControlBase.CurrentCellErrorMessage"/>
        /// <summary>See <see cref="GridControlBase.CurrentCellErrorMessage"/> in the <see cref="GridControlBase"/> class for information.</summary>
        void OnCurrentCellErrorMessage(GridCurrentCellErrorMessageEventArgs e);

        /// <copyfrom cref="GridControlBase.CurrentCellKeyDown"/>
        /// <summary>See <see cref="GridControlBase.CurrentCellKeyDown"/> in the <see cref="GridControlBase"/> class for information.</summary>
        void OnCurrentCellKeyDown(KeyEventArgs e);

        /// <copyfrom cref="GridControlBase.CurrentCellKeyPress"/>
        /// <summary>See <see cref="GridControlBase.CurrentCellKeyPress"/> in the <see cref="GridControlBase"/> class for information.</summary>
        void OnCurrentCellKeyPress(KeyPressEventArgs e);

        /// <copyfrom cref="GridControlBase.CurrentCellKeyUp"/>
        /// <summary>See <see cref="GridControlBase.CurrentCellKeyUp"/> in the <see cref="GridControlBase"/> class for information.</summary>
        void OnCurrentCellKeyUp(KeyEventArgs e);

        /// <copyfrom cref="GridControlBase.CurrentCellMoved"/>
        /// <summary>See <see cref="GridControlBase.CurrentCellMoved"/> in the <see cref="GridControlBase"/> class for information.</summary>
        void OnCurrentCellMoved(GridCurrentCellMovedEventArgs e);

        /// <copyfrom cref="GridControlBase.CurrentCellMoveFailed"/>
        /// <summary>See <see cref="GridControlBase.CurrentCellMoveFailed"/> in the <see cref="GridControlBase"/> class for information.</summary>
        void OnCurrentCellMoveFailed(GridCurrentCellMoveFailedEventArgs e);

        /// <copyfrom cref="GridControlBase.CurrentCellMoving"/>
        /// <summary>See <see cref="GridControlBase.CurrentCellMoving"/> in the <see cref="GridControlBase"/> class for information.</summary>
        void OnCurrentCellMoving(GridCurrentCellMovingEventArgs e);

        /// <copyfrom cref="GridControlBase.CurrentCellRejectedChanges"/>
        /// <summary>See <see cref="GridControlBase.CurrentCellRejectedChanges"/> in the <see cref="GridControlBase"/> class for information.</summary>
        void OnCurrentCellRejectedChanges(EventArgs e);

        /// <copyfrom cref="GridControlBase.CurrentCellShowedDropDown"/>
        /// <summary>See <see cref="GridControlBase.CurrentCellShowedDropDown"/> in the <see cref="GridControlBase"/> class for information.</summary>
        void OnCurrentCellShowedDropDown(EventArgs e);

        /// <copyfrom cref="GridControlBase.CurrentCellShowingDropDown"/>
        /// <summary>See <see cref="GridControlBase.CurrentCellShowingDropDown"/> in the <see cref="GridControlBase"/> class for information.</summary>
        void OnCurrentCellShowingDropDown(GridCurrentCellShowingDropDownEventArgs e);

        /// <copyfrom cref="GridControlBase.CurrentCellStartEditing"/>
        /// <summary>See <see cref="GridControlBase.CurrentCellStartEditing"/> in the <see cref="GridControlBase"/> class for information.</summary>
        void OnCurrentCellStartEditing(CancelEventArgs e);

        /// <copyfrom cref="GridControlBase.CurrentCellValidated"/>
        /// <summary>See <see cref="GridControlBase.CurrentCellValidated"/> in the <see cref="GridControlBase"/> class for information.</summary>
        void OnCurrentCellValidated(EventArgs e);

        /// <copyfrom cref="GridControlBase.CurrentCellValidateString"/>
        /// <summary>See <see cref="GridControlBase.CurrentCellValidateString"/> in the <see cref="GridControlBase"/> class for information.</summary>
        void OnCurrentCellValidateString(GridCurrentCellValidateStringEventArgs e);

        /// <copyfrom cref="GridControlBase.CurrentCellValidating"/>
        /// <summary>See <see cref="GridControlBase.CurrentCellValidating"/> in the <see cref="GridControlBase"/> class for information.</summary>
        void OnCurrentCellValidating(CancelEventArgs e);

        /// <copyfrom cref="GridControlBase.PushButtonClick"/>
        /// <summary>See <see cref="GridControlBase.PushButtonClick"/> in the <see cref="GridControlBase"/> class for information.</summary>
        void OnPushButtonClick(GridCellPushButtonClickEventArgs e);

        /// <copyfrom cref="GridControlBase.DrawCell"/>
        /// <summary>See <see cref="GridControlBase.DrawCell"/> in the <see cref="GridControlBase"/> class for information.</summary>
        void OnDrawCell(GridDrawCellEventArgs e);

        /// <copyfrom cref="GridControlBase.DrawCellDisplayText"/>
        /// <summary>See <see cref="GridControlBase.DrawCellDisplayText"/> in the <see cref="GridControlBase"/> class for information.</summary>
        void OnDrawCellDisplayText(GridDrawCellDisplayTextEventArgs e);

        /// <copyfrom cref="GridControlBase.DrawCellBackground"/>
        /// <summary>See <see cref="GridControlBase.DrawCellBackground"/> in the <see cref="GridControlBase"/> class for information.</summary>
        void OnDrawCellBackground(GridDrawCellBackgroundEventArgs e);

        /// <copyfrom cref="GridControlBase.DrawCellButton"/>
        /// <summary>See <see cref="GridControlBase.DrawCellButton"/> in the <see cref="GridControlBase"/> class for information.</summary>
        void OnDrawCellButton(GridDrawCellButtonEventArgs e);

        /// <copyfrom cref="GridControlBase.DrawCellButtonBackground"/>
        /// <summary>See <see cref="GridControlBase.DrawCellButtonBackground"/> in the <see cref="GridControlBase"/> class for information.</summary>
        void OnDrawCellButtonBackground(GridDrawCellButtonBackgroundEventArgs e);

        /// <copyfrom cref="GridControlBase.DrawCellFrameAppearance"/>
        /// <summary>See <see cref="GridControlBase.DrawCellFrameAppearance"/> in the <see cref="GridControlBase"/> class for information.</summary>
        void OnDrawCellFrameAppearance(GridDrawCellBackgroundEventArgs e);

        /// <copyfrom cref="GridControlBase.DrawCurrentCellBorder"/>
        /// <summary>See <see cref="GridControlBase.DrawCurrentCellBorder"/> in the <see cref="GridControlBase"/> class for information.</summary>
        void OnDrawCurrentCellBorder(GridDrawCurrentCellBorderEventArgs e);

        /// <copyfrom cref="GridControlBase.PrepareViewStyleInfo"/>
        /// <summary>See <see cref="GridControlBase.PrepareViewStyleInfo"/> in the <see cref="GridControlBase"/> class for information.</summary>
        void OnPrepareViewStyleInfo(GridPrepareViewStyleInfoEventArgs e);

        /// <copyfrom cref="GridControlBase.QueryNextCurrentCellPosition"/>
        /// <summary>See <see cref="GridControlBase.QueryNextCurrentCellPosition"/> in the <see cref="GridControlBase"/> class for information.</summary>
        void OnQueryNextCurrentCellPosition(GridQueryNextCurrentCellPositionEventArgs e);

        /// <copyfrom cref="GridControlBase.MoveCurrentCellDirection"/>
        /// <summary>See <see cref="GridControlBase.MoveCurrentCellDirection"/> in the <see cref="GridControlBase"/> class for information.</summary>
        void OnMoveCurrentCellDirection(GridMoveCurrentCellDirectionEventArgs e);

        /// <copyfrom cref="GridControlBase.ResizingColumns"/>
        /// <summary>See <see cref="GridControlBase.ResizingColumns"/> in the <see cref="GridControlBase"/> class for information.</summary>
        void OnResizingColumns(GridResizingColumnsEventArgs e);

        /// <copyfrom cref="GridControlBase.ResizingRows"/>
        /// <summary>See <see cref="GridControlBase.ResizingRows"/> in the <see cref="GridControlBase"/> class for information.</summary>
        void OnResizingRows(GridResizingRowsEventArgs e);

        /// <copyfrom cref="GridControlBase.WrapCellNextControlInForm"/>
        /// <summary>See <see cref="GridControlBase.WrapCellNextControlInForm"/> in the <see cref="GridControlBase"/> class for information.</summary>
        void OnWrapCellNextControlInForm(GridWrapCellNextControlInFormEventArgs e);

        /// <copyfrom cref="GridControlBase.HScrollPixelPosChanged"/>
        /// <summary>See <see cref="GridControlBase.HScrollPixelPosChanged"/> in the <see cref="GridControlBase"/> class for information.</summary>
        void OnHScrollPixelPosChanged(GridScrollPositionChangedEventArgs e);

        /// <copyfrom cref="GridControlBase.HScrollPixelPosChanging"/>
        /// <summary>See <see cref="GridControlBase.HScrollPixelPosChanging"/> in the <see cref="GridControlBase"/> class for information.</summary>
        void OnHScrollPixelPosChanging(GridScrollPositionChangingEventArgs e);

        /// <copyfrom cref="GridControlBase.LeftColChanged"/>
        /// <summary>See <see cref="GridControlBase.LeftColChanged"/> in the <see cref="GridControlBase"/> class for information.</summary>
        void OnLeftColChanged(GridRowColIndexChangedEventArgs e);

        /// <copyfrom cref="GridControlBase.LeftColChanging"/>
        /// <summary>See <see cref="GridControlBase.LeftColChanging"/> in the <see cref="GridControlBase"/> class for information.</summary>
        void OnLeftColChanging(GridRowColIndexChangingEventArgs e);

        /// <copyfrom cref="GridControlBase.MouseActivating"/>
        /// <summary>See <see cref="GridControlBase.MouseActivating"/> in the <see cref="GridControlBase"/> class for information.</summary>
        void OnMouseActivating(CancelEventArgs e);

        /// <copyfrom cref="GridControlBase.QueryCanOleDragRange"/>
        /// <summary>See <see cref="GridControlBase.QueryCanOleDragRange"/> in the <see cref="GridControlBase"/> class for information.</summary>
        void OnQueryCanOleDragRange(GridQueryCanOleDragRangeEventArgs e);

        /// <copyfrom cref="GridControlBase.ScrollInfoChanged"/>
        /// <summary>See <see cref="GridControlBase.ScrollInfoChanged"/> in the <see cref="GridControlBase"/> class for information.</summary>
        void OnScrollInfoChanged(EventArgs e);

        /// <copyfrom cref="GridControlBase.SelectionDragged"/>
        /// <summary>See <see cref="GridControlBase.SelectionDragged"/> in the <see cref="GridControlBase"/> class for information.</summary>
        void OnSelectionDragged(GridSelectionDragEventArgs e);

        /// <copyfrom cref="GridControlBase.SelectionDragging"/>
        /// <summary>See <see cref="GridControlBase.SelectionDragging"/> in the <see cref="GridControlBase"/> class for information.</summary>
        void OnSelectionDragging(GridSelectionDragEventArgs e);

        /// <copyfrom cref="GridControlBase.SelectionFrameChanged"/>
        /// <summary>See <see cref="GridControlBase.SelectionFrameChanged"/> in the <see cref="GridControlBase"/> class for information.</summary>
        void OnSelectionFrameChanged(GraphicsEventArgs e);

        /// <copyfrom cref="GridControlBase.SelectionFrameChanging"/>
        /// <summary>See <see cref="GridControlBase.SelectionFrameChanging"/> in the <see cref="GridControlBase"/> class for information.</summary>
        void OnSelectionFrameChanging(GraphicsEventArgs e);

        /// <copyfrom cref="GridControlBase.TopRowChanged"/>
        /// <summary>See <see cref="GridControlBase.TopRowChanged"/> in the <see cref="GridControlBase"/> class for information.</summary>
        void OnTopRowChanged(GridRowColIndexChangedEventArgs e);

        /// <copyfrom cref="GridControlBase.TopRowChanging"/>
        /// <summary>See <see cref="GridControlBase.TopRowChanging"/> in the <see cref="GridControlBase"/> class for information.</summary>
        void OnTopRowChanging(GridRowColIndexChangingEventArgs e);

        /// <copyfrom cref="GridControlBase.VScrollPixelPosChanged"/>
        /// <summary>See <see cref="GridControlBase.VScrollPixelPosChanged"/> in the <see cref="GridControlBase"/> class for information.</summary>
        void OnVScrollPixelPosChanged(GridScrollPositionChangedEventArgs e);

        /// <copyfrom cref="GridControlBase.VScrollPixelPosChanging"/>
        /// <summary>See <see cref="GridControlBase.VScrollPixelPosChanging"/> in the <see cref="GridControlBase"/> class for information.</summary>
        void OnVScrollPixelPosChanging(GridScrollPositionChangingEventArgs e);

        /// <summary>Occurs when <see cref="GridControlBase.OnDeactivated"/> is called on <see cref="GridControlBase"/>.</summary>
        void OnDeactivated(EventArgs e);

        /// <summary>Occurs when <see cref="GridControlBase.OnEnter"/> is called on <see cref="GridControlBase"/>.</summary>
        void OnEnter(EventArgs e);

        /// <summary>Occurs when <see cref="GridControlBase.OnInvalidated"/> is called on <see cref="GridControlBase"/>.</summary>
        void OnInvalidated(InvalidateEventArgs e);

        /// <summary>Occurs when <see cref="GridControlBase.OnKeyDown"/> is called on <see cref="GridControlBase"/>.</summary>
        void OnKeyDown(KeyEventArgs e);

        /// <summary>Occurs when <see cref="GridControlBase.OnKeyPress"/> is called on <see cref="GridControlBase"/>.</summary>
        void OnKeyPress(KeyPressEventArgs e);

        /// <summary>Occurs when <see cref="GridControlBase.OnKeyUp"/> is called on <see cref="GridControlBase"/>.</summary>
        void OnKeyUp(KeyEventArgs e);

        /// <summary>Occurs when <see cref="GridControlBase.OnLayout"/> is called on <see cref="GridControlBase"/>.</summary>
        void OnLayout(LayoutEventArgs le);

        /// <summary>Occurs when <see cref="GridControlBase.OnMouseDown"/> is called on <see cref="GridControlBase"/>.</summary>
        void OnMouseDown(MouseEventArgs e);

        /// <summary>Occurs when <see cref="GridControlBase.OnMouseMove"/> is called on <see cref="GridControlBase"/>.</summary>
        void OnMouseMove(MouseEventArgs e);

        /// <summary>Occurs when <see cref="GridControlBase.OnMouseUp"/> is called on <see cref="GridControlBase"/>.</summary>
        void OnMouseUp(MouseEventArgs e);

        /// <summary>Occurs when <see cref="GridControlBase.OnEnter"/> is called on <see cref="GridControlBase"/>.</summary>
        void OnMouseWheel(MouseEventArgs e);

        /// <summary>Occurs when <see cref="GridControlBase.OnPaint"/> is called on <see cref="GridControlBase"/>.</summary>
        void OnPaint(PaintEventArgs pe);

        /// <summary>Occurs when <see cref="GridControlBase.OnScrollControlMouseDown"/> is called on <see cref="GridControlBase"/>.</summary>
        void OnScrollControlMouseDown(CancelMouseEventArgs e);

        /// <summary>Occurs when <see cref="GridControlBase.OnScrollTipFeedback"/> is called on <see cref="GridControlBase"/>.</summary>
        void OnScrollTipFeedback(ScrollTipFeedbackEventArgs e);

        /// <summary>Occurs when <see cref="GridControlBase.OnSizeChanged"/> is called on <see cref="GridControlBase"/>.</summary>
        void OnSizeChanged(EventArgs e);

        /// <summary>Occurs when <see cref="GridControlBase.OnSplitterPaneClosing"/> is called on <see cref="GridControlBase"/>.</summary>
        void OnSplitterPaneClosing(EventArgs e);

        /// <summary>Occurs when <see cref="GridControlBase.OnValidating"/> is called on <see cref="GridControlBase"/>.</summary>
        void OnValidating(CancelEventArgs e);

        /// <summary>Occurs when <see cref="GridControlBase.OnVisibleChanged"/> is called on <see cref="GridControlBase"/>.</summary>
        void OnVisibleChanged(EventArgs e);

        /// <summary>Occurs when <see cref="GridControlBase.OnWindowScrolled"/> is called on <see cref="GridControlBase"/>.</summary>
        void OnWindowScrolled(ScrollWindowEventArgs e);

        /// <copyfrom cref="GridControlBase.SupportsTransparentBackColorChanged"/>
        /// <summary>See <see cref="GridControlBase.SupportsTransparentBackColorChanged"/> in the <see cref="GridControlBase"/> class for information.</summary>
        void OnSupportsTransparentBackColorChanged(EventArgs e);

        /// <copyfrom cref="GridControlBase.PrintingModeChanged"/>
        /// <summary>See <see cref="GridControlBase.PrintingModeChanged"/> in the <see cref="GridControlBase"/> class for information.</summary>
        void OnPrintingModeChanged(EventArgs e);

        /// <copyfrom cref="GridControlBase.ModelChanged"/>
        /// <summary>See <see cref="GridControlBase.ModelChanged"/> in the <see cref="GridControlBase"/> class for information.</summary>
        void OnModelChanged(EventArgs e);

        /// <copyfrom cref="GridControlBase.GridControlMouseDown"/>
        /// <summary>See <see cref="GridControlBase.GridControlMouseDown"/> in the <see cref="GridControlBase"/> class for information.</summary>
        void OnGridControlMouseDown(CancelMouseEventArgs e);

        /// <copyfrom cref="GridControlBase.GridControlMouseMove"/>
        /// <summary>See <see cref="GridControlBase.GridControlMouseMove"/> in the <see cref="GridControlBase"/> class for information.</summary>
        void OnGridControlMouseMove(CancelMouseEventArgs e);

        /// <copyfrom cref="GridControlBase.GridControlMouseUp"/>
        /// <summary>See <see cref="GridControlBase.GridControlMouseUp"/> in the <see cref="GridControlBase"/> class for information.</summary>
        void OnGridControlMouseUp(CancelMouseEventArgs e);

        /// <copyfrom cref="GridControlBase.OnGridValidating"/>
        /// <summary>See <see cref="GridControlBase.OnGridValidating"/> in the <see cref="GridControlBase"/> class for information.</summary>
        void OnGridValidating(CancelEventArgs e);

        /// <copyfrom cref="GridControlBase.GridBoundsChanged"/>
        /// <summary>See <see cref="GridControlBase.GridBoundsChanged"/> in the <see cref="GridControlBase"/> class for information.</summary>
        void OnGridBoundsChanged(EventArgs e);

        /// <copyfrom cref="GridControlBase.ThemeChanged"/>
        /// <summary>See <see cref="GridControlBase.ThemeChanged"/> in the <see cref="GridControlBase"/> class for information.</summary>
        void OnThemeChanged(EventArgs e);
    }

    /// <summary>
    /// Implements a grid control that displays a grid model.
    /// </summary>
    /// <remarks>
    /// <see cref="GridControlBase"/> implements a view on a <see cref="GridModel"/>. Several views can be opened for the same model. Changes
    /// in <see cref="GridModel"/> are reflected immediately among all views.<para/>
    /// The <see cref="GridModel"/> provides storage for all data and settings associated with the grid. The <see cref="GridControlBase"/>
    /// implements user interaction and display of the data.<para/>
    /// <see cref="GridControlBase"/> is a user control that is derived from <see cref="ScrollControl"/>. It lets the user scrolls through grid data
    /// with mouse or keyboard. The grid displays a large number of cells where each cell can have its own unique formatting and cell type.<para/>
    /// <see cref="GridControlBase"/> also offers a wide range of events that let you customize the default behavior of the grid at run-time.
    /// </remarks>
    public class GridControlBase : ScrollControl,
        IThemedControl,
        ICreateNewWindow,
        IGridModelSource,
        IGridWindowlessSite,
        IGridWindowlessObject
    {
        private GridModel model;
        private Rectangle bounds = Rectangle.Empty;
        private bool inPrinting = false;
        private bool themesEnabled = false;
        private bool handleWMSYSCHAR = false;

        private bool unHideColsOnDblClick = true;
        private string backgroundImageID = string.Empty;

        internal Rectangle printBounds = Rectangle.Empty;
        internal GridCurrentCellMoveDelegateHandler externalMove = null; //// GridSelectCellsMouseController will assign this.

        static bool useOldHiddenScrollLogic = false;

        static bool allowViewStyleCacheBaseStyleValues = false;

        static bool useImageListDrawing = false;

        private bool showRowHeaderErroricon = false;
        private SortIconPlacement sortIconPlacement = SortIconPlacement.Right;
        private bool allowProportionalColumnSizing = false;
        public bool enableRTLMk = false;
        int unicodeRLM = 8206;
        Dictionary<int, int> sizedColumns = null;
        /// <summary>
        /// Gets or sets a value indicating whether the grid can enable caching style property values
        /// of GetViewStyleInfo styles when accessed the first time. This will make accessing
        /// the same property repeatedly faster (e.g. style.CellValue or style.ReadOnly might be 
        /// accessed multiple times from different routines for the same style object).
        /// </summary>
        public static bool AllowViewStyleCacheBaseStyleValues
        {
            get { return allowViewStyleCacheBaseStyleValues; }
            set { allowViewStyleCacheBaseStyleValues = value; }
        }

        /// <summary>
        /// Gets or sets a value indicating whether to switch back to old logic used for hiding rows or columns
        /// in case of any compatibility problems (logic was changed after 4.2)
        /// </summary>
        [Browsable(false)]
        [DesignerSerializationVisibilityAttribute(DesignerSerializationVisibility.Hidden)]
        [Syncfusion.Documentation.DocumentationExclude()]
        [System.Xml.Serialization.XmlIgnore]
        public static bool UseOldHiddenScrollLogic
        {
            get
            {
                return useOldHiddenScrollLogic;
            }

            set
            {
                useOldHiddenScrollLogic = value;
            }
        }
        /// <summary>       
        /// Gets or sets a value to assign the placement of the SortIcon
        /// </summary>
        [DesignerSerializationVisibilityAttribute(DesignerSerializationVisibility.Hidden), Category("Appearance")]
        [Syncfusion.Documentation.DocumentationExclude()]
        [Description("Gets or sets a value to assign the placement of the SortIcon.")]
        [System.Xml.Serialization.XmlIgnore]
        public SortIconPlacement SortIconPlacement
        {
            get
            {
                return sortIconPlacement;
            }
            set
            {
                if (sortIconPlacement != value)
                    sortIconPlacement = value;
            }
        }

        /// <summary>
        /// Grid uses ImageList_DrawEx to draw cell images. Lets you switch to default ImageList_Draw instead. 
        /// Added for PrintColumnsToFit helper class which uses Metafile drawings internally
        /// </summary>
        [Browsable(false)]
        [DesignerSerializationVisibilityAttribute(DesignerSerializationVisibility.Hidden)]
        [Syncfusion.Documentation.DocumentationExclude()]
        [System.Xml.Serialization.XmlIgnore]
        public static bool UseImageListDrawing
        {
            get
            {
                return useImageListDrawing;
            }
            set
            {
                useImageListDrawing = value;
            }
        }

        /// <summary>
        /// Gets or sets the ExternalMove. Used by GridSelectCellsMouseController.
        /// </summary>
        [Browsable(false)]
        [DesignerSerializationVisibilityAttribute(DesignerSerializationVisibility.Hidden)]
        [Syncfusion.Documentation.DocumentationExclude()]
        [System.Xml.Serialization.XmlIgnore]
        public GridCurrentCellMoveDelegateHandler ExternalMove
        {
            get
            {
                return externalMove;
            }

            set
            {
                externalMove = value;
            }
        }

        /// <summary>
        /// Occurs before TextBox of a TextBox, OriginalTextBox or derived cell type is created.
        /// </summary>
        [Browsable(false)]
        [Syncfusion.Documentation.DocumentationExclude()]
        public event GridQueryCreateCellTextBoxEventHandler QueryCreateCellTextBox;

        /// <summary>
        /// Raises the <see cref="QueryCreateCellTextBox"/> event.
        /// </summary>
        /// <param name="e">A <see cref="GridQueryCreateCellTextBoxEventArgs" /> that contains the event data.</param>
        [Syncfusion.Documentation.DocumentationExclude()]
        protected virtual void OnQueryCreateCellTextBox(GridQueryCreateCellTextBoxEventArgs e)
        {
            if (QueryCreateCellTextBox != null)
            {
                QueryCreateCellTextBox(this, e);
            }
        }

        internal void RaiseQueryCreateCellTextBox(GridQueryCreateCellTextBoxEventArgs e)
        {
            ////SSTraceUtil.TraceCurrentMethodInfo(this.ToString(), e);
            OnQueryCreateCellTextBox(e);
        }

        /// <summary>
        /// Occurs before BrushPaint.FillRectangle is called.
        /// </summary>
        [Browsable(false)]
        public event GridFillRectangleHookEventHandler FillRectangleHook;

        /// <summary>
        /// Checks for this.RightToLeft == RightToLeft.Yes;
        /// </summary>
        /// <returns>True if text is drawn from right to left.</returns>
        [Syncfusion.Documentation.DocumentationExclude()]
        public bool IsRightToLeft()
        {
            return this.RightToLeft == RightToLeft.Yes;
        }

        /// <summary>
        /// Raises the  <see cref="FillRectangleHook"/> event.
        /// </summary>
        /// <param name="e">A <see cref="GridFillRectangleHookEventArgs" /> that contains the event data.</param>
        protected virtual void OnFillRectangleHook(GridFillRectangleHookEventArgs e)
        {
            if (FillRectangleHook != null)
            {
                FillRectangleHook(this, e);
            }
        }

        internal void RaiseFillRectangleHook(GridFillRectangleHookEventArgs e)
        {
            ////SSTraceUtil.TraceCurrentMethodInfo(this.ToString(), e);
            OnFillRectangleHook(e);
        }

        bool allowTextBoxAutoSize = true;

        /// <summary>
        /// Gets or sets a value indicating whether to force cell renderers to ignore the GridStyleInfo.AutoSize property and
        /// do not automatically resize cells while typing.
        /// </summary>
        [DesignerSerializationVisibilityAttribute(DesignerSerializationVisibility.Hidden)]
        [Browsable(false)]
        [Syncfusion.Documentation.DocumentationExclude()]
        public bool AllowTextBoxAutoSize
        {
            get
            {
                return allowTextBoxAutoSize;
            }

            set
            {
                allowTextBoxAutoSize = value;
            }
        }

        bool allowRowResizeUsingCellBoundaries = false;

        /// <summary>
        /// Gets or sets a value indicating whether to allow resizing of rows through standard cell boundaries. No support for GridListControl.
        /// </summary>
        [DefaultValue(false)]
        [Description("Enables resizing of rows through standard cell boundaries.")]
        [Category("Grid")]
        public virtual bool AllowRowResizeUsingCellBoundaries
        {
            get
            {
                return allowRowResizeUsingCellBoundaries;
            }
            set
            {
                if(allowRowResizeUsingCellBoundaries != value)
                    allowRowResizeUsingCellBoundaries = value;
            }
        }

        bool allowColumnResizeUsingCellBoundaries = false;

        /// <summary>
        /// Gets or sets a value indicating whether to allow resizing of columns through standard cell boundaries. For GridListControl, it is true by default.
        /// </summary>
        [DefaultValue(false)]
        [Description("Enables resizing of columns through standard cell boundaries.")]
        [Category("Grid")]
        public virtual bool AllowColumnResizeUsingCellBoundaries
        {
            get
            {
                return allowColumnResizeUsingCellBoundaries;
            }
            set
            {
                if (allowColumnResizeUsingCellBoundaries != value)
                    allowColumnResizeUsingCellBoundaries = value;
            }
        }

        private bool showMessageBoxOnDrop = false;

        /// <summary>
        /// Gets or sets a value indicating whether to display messagebox before dropping cell contents from source to the destination.
        /// </summary>
        [DefaultValue(false)]
        [Description("Allows us to display messagebox before dropping cell contents from source to the destination.")]
        [Category("Grid")]
        public bool ShowMessageBoxOnDrop
        {
            get
            {
                return showMessageBoxOnDrop;
            }

            set
            {
                if (showMessageBoxOnDrop != value)
                {
                    showMessageBoxOnDrop = value;
                }
            }
        }

        [ThreadStaticAttribute]
        static bool useGdiPlusRightAlignedTextWorkaround = false;

        private bool optimizeDrawBackground = true;

        /// <summary>
        /// Gets or sets a value indicating whether the grid's painting routines have a built-in optimization where it combines
        /// cells that have the same background to be drawn with one paint operation instead
        /// of drawing the background for each cell individually.
        /// </summary>
        /// <remarks>
        /// The property is true by default. The optimization causes the <see cref="DrawCellBackground"/>
        /// event to be hit for every cell and also change the background color of a style object
        /// within a <see cref="GridCellRendererBase.OnDraw"/> operation to be ignored. <para/>
        /// You should set it False if this is an issue for you. Or you could also try setting <see cref="GridStyleInfo.Interior"/>
        /// be set to BrushInfo.Empty to force DrawCellBackground be called for cells.
        /// </remarks>
        [DefaultValue(true)]
        [Description("Enables built-in optimization that allows grid to combine background drawing for cells that have the same background.")]
        [Category("Grid")]
        public bool OptimizeDrawBackground
        {
            get
            {
                return this.optimizeDrawBackground;
            }

            set
            {
                this.optimizeDrawBackground = value;
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether UseGdiPlusRightAlignedTextWorkaround implements a work-around for a GDI+ known issue with
        /// right-aligned text and DrawString.
        /// </summary>
        /// <remarks>
        /// When you have cells with right-aligned text some words are not aligned at the right border as
        /// you would expect. This is because of GDI+ designed behavior, and the degree of the problem varies
        /// from font to font. You can see this behavior for example with regular System.Windows.Forms.Label
        /// controls. Since Essential Grid relies on GDI+, it exhibits the same behavior. Besides using a
        /// Monospaced Font, some include using either antialiased string drawing or
        /// explicitly measuring the string width and not relying on DrawString to draw the text right-aligned.
        /// <para/>
        /// When you enable UseGdiPlusRightAlignedTextWorkaround then the static cell renderer will
        /// use the measure string width workaround. However, this will slow down drawing of right-aligned text.
        /// </remarks>
        public static bool UseGdiPlusRightAlignedTextWorkaround
        {
            get
            {
                return useGdiPlusRightAlignedTextWorkaround;
            }

            set
            {
                useGdiPlusRightAlignedTextWorkaround = value;
            }
        }

        private IGridControlBaseEventsTarget eventsTarget;

        /// <summary>
        /// Gets or sets the GridControlBaseEventsTarget. Redirects events defined in <see cref="IGridControlBaseEventsTarget"/> to the specified object.
        /// Each event will first be called on <see cref="GridControlBaseEventsTarget"/> before the actual
        /// event handler in this control is called.
        /// </summary>
        [DesignerSerializationVisibilityAttribute(DesignerSerializationVisibility.Hidden)]
        [Browsable(false)]
        public IGridControlBaseEventsTarget GridControlBaseEventsTarget
        {
            get
            {
                return eventsTarget;
            }

            set
            {
                eventsTarget = value;
            }
        }

        #region Horizontal Pixel Scrolling
        int _hScrollPixelDelta = 0;
        internal bool hPixelScroll = false;
        internal int currentHScrollPixelPosSaved = -1;
        internal int hScrollPixelWidthSaved = -1;

        /// <summary>
        /// Gets or sets a value indicating whether it enables horizontal pixel scrolling for the grid.
        /// </summary>
        [Category("Scrolling"),
        Browsable(true),
        Description("Defines whether the horizontal pixel scrolling is enabled."),
        DefaultValue(false)]
        public bool HScrollPixel
        {
            get
            {
                return hPixelScroll;
            }

            set
            {
                if (hPixelScroll != value)
                {
                    hPixelScroll = value;
                    hScrollPixelDelta = 0;
                    Invalidate();
                    UpdateScrollBars();
                }
            }
        }

        internal int hScrollPixelDelta
        {
            get
            {
                return this.PrintingMode ? 0 : _hScrollPixelDelta;
            }

            set
            {
                _hScrollPixelDelta = value;
            }
        }

        /// <summary>
        /// The current difference between the left column's pixel offset and the current scroll position.
        /// </summary>
        /// <returns>Difference between left column's pixel offset and current scroll position.</returns>
        public int GetCurrentHScrollPixelDelta()
        {
            return hScrollPixelDelta;
        }

        /// <summary>
        /// The current scroll position for pixel scrolling
        /// </summary>
        /// <returns>Current scroll position.</returns>
        public int GetCurrentHScrollPixelPos()
        {
            if (currentHScrollPixelPosSaved != -1)
            {
                return currentHScrollPixelPosSaved;
            }

            currentHScrollPixelPosSaved = ColIndexToHScrollPixelPos(this.LeftColIndex) + this.hScrollPixelDelta;
            return currentHScrollPixelPosSaved;
        }

        /// <summary>
        /// Sets the current scroll position for pixel scrolling and scrolls the grid
        /// </summary>
        /// <param name="pixelPos">The new horizontal pixel scroll position.</param>
        public void SetCurrentHScrollPixelPos(int pixelPos)
        {
            int hScrollPixelMinimum = GetHScrollPixelMinimum();
            pixelPos = Math.Max(Math.Min(pixelPos, GetHScrollPixelWidth() - HScrollBar.LargeChange), hScrollPixelMinimum);

            int hPixelScrollPos = this.GetCurrentHScrollPixelPos();
            if (pixelPos < hPixelScrollPos)
            {
                ScrollGrid.DoPixelScroll(GridDirectionType.Left, hPixelScrollPos - pixelPos);
            }
            else if (pixelPos > hPixelScrollPos)
            {
                ScrollGrid.DoPixelScroll(GridDirectionType.Right, pixelPos - hPixelScrollPos);
            }

            currentHScrollPixelPosSaved = -1;
            ////Update();
        }

        /// <summary>
        /// The left most scroll position for pixel scrolling.
        /// </summary>
        /// <returns>Left most scroll position.</returns>
        public int GetHScrollPixelMinimum()
        {
            return ColIndexToHScrollPixelPos(this.InternalGetFrozenCols() + 1);
        }

        /// <summary>
        /// Gets the scroll position for pixel scrolling for a column.
        /// </summary>
        /// <param name="colIndex">Column index.</param>
        /// <returns>Horizontal scroll position in pixel.</returns>
        public virtual int ColIndexToHScrollPixelPos(int colIndex)
        {
            return ViewLayout.GetColRangeWidth(0, colIndex - 1, GridCellSizeKind.ActualSize);
        }

        //// event ScrollPositionChanged HScrollPixelPosChanged

        /// <summary>
        /// Occurs after the horizontal pixel scroll position was changed.
        /// </summary>
        [Description("Occurs after the horizontal pixel scroll position was changed.")]
        [Category("Scrolling")]
        public event GridScrollPositionChangedEventHandler HScrollPixelPosChanged;

        /// <summary>
        /// Raises the <see cref="HScrollPixelPosChanged"/> event.
        /// </summary>
        /// <param name="e">An EventArgs that contains the event data.</param>
        protected virtual void OnHScrollPixelPosChanged(GridScrollPositionChangedEventArgs e)
        {
            if (this.eventsTarget != null)
            {
                this.eventsTarget.OnHScrollPixelPosChanged(e);
            }

            if (HScrollPixelPosChanged != null)
            {
                HScrollPixelPosChanged(this, e);
            }
        }

        internal void RaiseHScrollPixelPosChanged(bool success)
        {
            GridScrollPositionChangedEventArgs e = new GridScrollPositionChangedEventArgs(success);
            OnHScrollPixelPosChanged(e);
        }

        //// event ScrollPositionChanged VScrollPixelPosChanged

        /// <summary>
        /// Occurs after the vertical pixel scroll position was changed.
        /// </summary>
        [Description("Occurs after the horizontal pixel scroll position was changed.")]
        [Category("Scrolling")]
        public event GridScrollPositionChangedEventHandler VScrollPixelPosChanged;

        /// <summary>
        /// Raises the <see cref="VScrollPixelPosChanged"/> event.
        /// </summary>
        /// <param name="e">An EventArgs that contains the event data.</param>
        protected virtual void OnVScrollPixelPosChanged(GridScrollPositionChangedEventArgs e)
        {
            if (this.eventsTarget != null)
            {
                this.eventsTarget.OnVScrollPixelPosChanged(e);
            }

            if (VScrollPixelPosChanged != null)
            {
                VScrollPixelPosChanged(this, e);
            }
        }

        internal void RaiseVScrollPixelPosChanged(bool success)
        {
            GridScrollPositionChangedEventArgs e = new GridScrollPositionChangedEventArgs(success);
            OnVScrollPixelPosChanged(e);
        }

        //// event ScrollPositionChanging VScrollPixelPosChanging

        /// <summary>
        /// Occurs before the vertical pixel scroll position is changed.
        /// </summary>
        [Description("Occurs before the vertical pixel scroll position is changed.")]
        [Category("Scrolling")]
        public event GridScrollPositionChangingEventHandler VScrollPixelPosChanging;

        /// <summary>
        /// Raises the <see cref="VScrollPixelPosChanging"/> event.
        /// </summary>
        /// <param name="e">An EventArgs that contains the event data.</param>
        protected virtual void OnVScrollPixelPosChanging(GridScrollPositionChangingEventArgs e)
        {
            if (this.eventsTarget != null)
            {
                this.eventsTarget.OnVScrollPixelPosChanging(e);
            }

            if (VScrollPixelPosChanging != null)
            {
                VScrollPixelPosChanging(this, e);
            }
        }

        internal bool RaiseVScrollPixelPosChanging(int position)
        {
            GridScrollPositionChangingEventArgs e = new GridScrollPositionChangingEventArgs(position);
            OnVScrollPixelPosChanging(e);
            return !e.Cancel;
        }

        //// event ScrollPositionChanging HScrollPixelPosChanging

        /// <summary>
        /// Occurs before the horizontal pixel scroll position is changed.
        /// </summary>
        [Description("Occurs before the horizontal pixel scroll position is changed.")]
        [Category("Scrolling")]
        public event GridScrollPositionChangingEventHandler HScrollPixelPosChanging;

        /// <summary>
        /// Raises the <see cref="HScrollPixelPosChanging"/> event.
        /// </summary>
        /// <param name="e">An EventArgs that contains the event data.</param>
        protected virtual void OnHScrollPixelPosChanging(GridScrollPositionChangingEventArgs e)
        {
            if (this.eventsTarget != null)
            {
                this.eventsTarget.OnHScrollPixelPosChanging(e);
            }

            if (HScrollPixelPosChanging != null)
            {
                HScrollPixelPosChanging(this, e);
            }
        }

        internal void RaiseHScrollPixelPosChanging(GridScrollPositionChangingEventArgs e)
        {
            OnHScrollPixelPosChanging(e);
        }

        internal bool RaiseHScrollPixelPosChanging(int position)
        {
            GridScrollPositionChangingEventArgs e = new GridScrollPositionChangingEventArgs(position);
            OnHScrollPixelPosChanging(e);
            return !e.Cancel;
        }

        /// <summary>
        /// Gets the total width of the grid for pixel scrolling.
        /// </summary>
        /// <returns>Total grid width.</returns>
        public virtual int GetHScrollPixelWidth()
        {
            if (hScrollPixelWidthSaved == -1)
            {
                int pixelIndex = 0;
                int n = 0;
                do
                {
                    pixelIndex += GetColWidth(n);
                }
                while (ScrollGrid.GetNextColIndex(ref n));
                hScrollPixelWidthSaved = pixelIndex;
            }

            return hScrollPixelWidthSaved;
        }

        /// <summary>
        /// Gets the column and the pixel delta to the scroll position of the column for a specified scroll position.
        /// </summary>
        /// <param name="pixelPos">Scroll position.</param>
        /// <param name="colIndex">Column index.</param>
        /// <param name="pixelDelta">Pixel delta.</param>
        public virtual void HScrollPixelPosToColIndex(int pixelPos, out int colIndex, out int pixelDelta)
        {
            int frozenCount = this.InternalGetFrozenCols();
            int pixelIndex = 0;
            colIndex = 0;
            pixelDelta = 0;
            int n = 0;
            do
            {
                pixelDelta = pixelPos - pixelIndex;
                pixelIndex += GetColWidth(n);
                if (pixelIndex > pixelPos && n > frozenCount)
                {
                    colIndex = n;
                    break;
                }
            }
            while (ScrollGrid.GetNextColIndex(ref n));
            if (pixelDelta < 0)
            {
                pixelDelta = 0;
                colIndex = frozenCount + 1;
            }
        }

        /// <summary>
        /// Scrolls the left and / or right bounds of the rectangle into view.
        /// </summary>
        /// <param name="r">Bounded rectangle.</param>
        /// <returns>True if scrolling is successful; False otherwise.</returns>
        public virtual bool HScrollPixelScrollInView(Rectangle r)
        {
            int currentHScrollPixelPos = GetCurrentHScrollPixelPos();
            if (this.IsRightToLeft())
            {
                if (r.Left < ViewLayout.ScrollAreaBounds.Left && r.Width < ViewLayout.ScrollAreaBounds.Width)
                {
                    SetCurrentHScrollPixelPos(currentHScrollPixelPos - r.Left + ViewLayout.ScrollAreaBounds.Left);
                    return true;
                }
                else if (r.Right > ViewLayout.ScrollAreaBounds.Right)
                {
                    SetCurrentHScrollPixelPos(currentHScrollPixelPos - r.Right + ViewLayout.ScrollAreaBounds.Right);
                    return true;
                }
                else if (r.Right > ViewLayout.ScrollAreaBounds.Right)
                {
                    // && r.Width >= ViewLayout.ScrollAreaBounds.Width))
                    if (r.Width < ViewLayout.ScrollAreaBounds.Width || r.Right < ViewLayout.ScrollAreaBounds.Left)
                    {
                        SetCurrentHScrollPixelPos(currentHScrollPixelPos - r.Right + ViewLayout.ScrollAreaBounds.Right);
                    }

                    return true;
                }
            }
            else
            {
                if (r.Left < ViewLayout.ScrollAreaBounds.Left)
                {
                    SetCurrentHScrollPixelPos(currentHScrollPixelPos + r.Left - ViewLayout.ScrollAreaBounds.Left);
                    return true;
                }
                else if (r.Right > ViewLayout.ScrollAreaBounds.Right && r.Width < ViewLayout.ScrollAreaBounds.Width)
                {
                    SetCurrentHScrollPixelPos(currentHScrollPixelPos + r.Right - ViewLayout.ScrollAreaBounds.Right);
                    return true;
                }
                else if (r.Right > ViewLayout.ScrollAreaBounds.Right)
                {
                    // && r.Width >= ViewLayout.ScrollAreaBounds.Width))
                    if (r.Width < ViewLayout.ScrollAreaBounds.Width || r.Left > ViewLayout.ScrollAreaBounds.Right)
                    {
                        SetCurrentHScrollPixelPos(currentHScrollPixelPos + r.Left - ViewLayout.ScrollAreaBounds.Left);
                    }

                    return true;
                }
            }

            return false;
        }

        #endregion

        #region Vertical Pixel Scrolling
        int _vScrollPixelDelta = 0;
        internal bool vPixelScroll = false;
        internal int currentVScrollPixelPosSaved = -1;
        internal int vScrollPixelHeightSaved = -1;

        /// <summary>
        /// Gets or sets a value indicating whether it enables vertical pixel scrolling for the grid.
        /// </summary>
        [Category("Scrolling"),
        Browsable(true),
        Description("Defines whether the vertical pixel scrolling is enabled."),
        DefaultValue(false)] 
        public bool VScrollPixel
        {
            get
            {
                return vPixelScroll;
            }

            set
            {
                if (vPixelScroll != value)
                {
                    vPixelScroll = value;
                    vScrollPixelDelta = 0;
                    Invalidate();
                    UpdateScrollBars();
                }
            }
        }

        /// <exclude/>
        protected void InternalSetVScrollPixel(bool value)
        {
            vPixelScroll = value;
        }

        internal int vScrollPixelDelta
        {
            get
            {
                return this.PrintingMode ? 0 : _vScrollPixelDelta;
            }

            set
            {
                _vScrollPixelDelta = value;
            }
        }

        /// <summary>
        /// Gets the current offset in pixel for the top most row.
        /// </summary>
        /// <returns>The number of pixels the top most row is above the view area.</returns>
        public int GetCurrentVScrollPixelDelta()
        {
            return this.PrintingMode ? 0 : vScrollPixelDelta;
        }

        /// <summary>
        /// Gets the current absolute vertical pixel position of the top most row.
        /// </summary>
        /// <returns>The vertical pixel scroll position.</returns>
        public int GetCurrentVScrollPixelPos()
        {
            if (currentVScrollPixelPosSaved != -1)
            {
                return currentVScrollPixelPosSaved;
            }

            currentVScrollPixelPosSaved = RowIndexToVScrollPixelPos(this.TopRowIndex) + this.vScrollPixelDelta;
            return currentVScrollPixelPosSaved;
        }

        /// <summary>
        /// Sets the current absolute vertical pixel scroll position without
        /// raising events and without updating the display.
        /// </summary>
        /// <param name="pixelPos">The new vertical pixel scroll position.</param>
        public void InternalSetCurrentVScrollPixelPos(int pixelPos)
        {
            int rowIndex;
            int pixelDelta;
            VScrollPixelPosToRowIndex(pixelPos, out rowIndex, out pixelDelta);
            this.InternalSetTopRow(rowIndex);
            this.vScrollPixelDelta = pixelDelta;
        }

        /// <summary>
        /// Sets the current absolute horizontal pixel position without
        /// raising events and without updating the display.
        /// </summary>
        /// <param name="pixelPos">The new horizontal pixel scroll position.</param>
        public void InternalSetCurrentHScrollPixelPos(int pixelPos)
        {
            int colIndex;
            int pixelDelta;
            HScrollPixelPosToColIndex(pixelPos, out colIndex, out pixelDelta);
            this.InternalSetLeftCol(colIndex);
            this.hScrollPixelDelta = pixelDelta;
        }

        /// <summary>
        /// Sets the current absolute vertical pixel scroll position
        /// raising events and scrolling the display.
        /// </summary>
        /// <param name="pixelPos">The new vertical pixel scroll position.</param>
        public void SetCurrentVScrollPixelPos(int pixelPos)
        {
            int vScrollPixelMinimum = GetVScrollPixelMinimum();
            pixelPos = Math.Max(Math.Min(pixelPos, GetVScrollPixelHeight() - VScrollBar.LargeChange), vScrollPixelMinimum);

            try
            {
                int vPixelScrollPos = this.GetCurrentVScrollPixelPos();
                if (pixelPos < vPixelScrollPos)
                {
                    ScrollGrid.DoPixelScroll(GridDirectionType.Up, vPixelScrollPos - pixelPos);
                }
                else if (pixelPos > vPixelScrollPos)
                {
                    ScrollGrid.DoPixelScroll(GridDirectionType.Down, pixelPos - vPixelScrollPos);
                }
            }
            catch (Exception)
            {
            }

            currentVScrollPixelPosSaved = -1;
            ////Update();
        }

        /// <summary>
        /// Gets the smallest value possible for vertical pixel scroll position (will be assigned to VScrollBar.Minimum).
        /// </summary>
        /// <returns>The smallest value possible for vertical pixel scroll position.</returns>
        public int GetVScrollPixelMinimum()
        {
            return RowIndexToVScrollPixelPos(this.InternalGetFrozenRows() + 1);
        }

        /// <summary>
        /// Returns the absolute vertical pixel scroll position for a specific row.
        /// </summary>
        /// <param name="rowIndex">The row index.</param>
        /// <returns>The  absolute vertical pixel scroll position of the row.</returns>
        public virtual int RowIndexToVScrollPixelPos(int rowIndex)
        {
            return ViewLayout.GetRowRangeHeight(0, rowIndex - 1, GridCellSizeKind.ActualSize);
        }

        /// <summary>
        /// Gets the total height of all rows in the grid.
        /// </summary>
        /// <returns>The total height of all rows in the grid.</returns>
        public virtual int GetVScrollPixelHeight()
        {
            if (vScrollPixelHeightSaved == -1)
            {
                int pixelIndex = 0;
                int n = 0;
                do
                {
                    pixelIndex += GetRowHeight(n);
                }
                while (ScrollGrid.GetNextRowIndex(ref n));
                vScrollPixelHeightSaved = pixelIndex;
            }

            return vScrollPixelHeightSaved;
        }

        /// <summary>
        /// Determines the row index that is located at a specified vertical pixel scroll position.
        /// </summary>
        /// <param name="pixelPos">The absolute vertical pixel scroll position.</param>
        /// <param name="rowIndex">Returns the resulting rowIndex.</param>
        /// <param name="pixelDelta">Returns the number of pixels the top most row is avove the view area for the pixelPos scroll position.</param>
        public virtual void VScrollPixelPosToRowIndex(int pixelPos, out int rowIndex, out int pixelDelta)
        {
            int frozenCount = this.InternalGetFrozenRows();
            int pixelIndex = 0;
            rowIndex = 0;
            pixelDelta = 0;
            int n = 0;
            do
            {
                pixelDelta = pixelPos - pixelIndex;
                pixelIndex += GetRowHeight(n);
                if (pixelIndex > pixelPos && n > frozenCount)
                {
                    rowIndex = n;
                    return;
                }
            }
            while (ScrollGrid.GetNextRowIndex(ref n));

            if (pixelDelta < 0)
            {
                pixelDelta = 0;
                rowIndex = frozenCount + 1;
            }
        }

        /// <summary>
        /// Scrolls the specified rectangle into view.
        /// </summary>
        /// <param name="r">The coordinates to be scrolled into view. The rectangle coordinates are client coordinates of the window. The
        /// rectangle coordinates can be negative if the coordinates are above the current viewing area.</param>
        /// <returns>True if grid was scrolled; False if rectangle bounds were already inside visible area.</returns>
        public virtual bool VScrollPixelScrollInView(Rectangle r)
        {
            int currentVScrollPixelPos = GetCurrentVScrollPixelPos();
            if (r.Top < ViewLayout.ScrollAreaBounds.Top)
            {
                SetCurrentVScrollPixelPos(currentVScrollPixelPos + r.Top - ViewLayout.ScrollAreaBounds.Top);
                return true;
            }
            else if (r.Bottom > ViewLayout.ScrollAreaBounds.Bottom && r.Height < ViewLayout.ScrollAreaBounds.Height)
            {
                SetCurrentVScrollPixelPos(currentVScrollPixelPos + r.Bottom - ViewLayout.ScrollAreaBounds.Bottom + 1);
                return true;
            }
            else if (r.Bottom > ViewLayout.ScrollAreaBounds.Bottom) 
            {
                //// && r.Height >= ViewLayout.ScrollAreaBounds.Height))
                SetCurrentVScrollPixelPos(currentVScrollPixelPos + r.Top - ViewLayout.ScrollAreaBounds.Top);
                return true;
            }

            return false;
        }

        #endregion

        /// <summary>
        /// Gets or sets the grid bounds while the grid is in printing mode.
        /// </summary>
        [DesignerSerializationVisibilityAttribute(DesignerSerializationVisibility.Hidden)]
        [Browsable(false)]
        public Rectangle PrintBounds
        {
            get
            {
                return printBounds;
            }

            set
            {
                printBounds = value;
            }
        }

        GridRangeInfo _updateSelectRange_Range = null;
        GridRangeInfoList _updateSelectRange_OldRange = null;

        /// <summary>
        /// Gets or sets UpdateSelectRange_Range. Used internally.
        /// </summary>
        [Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        [Syncfusion.Documentation.DocumentationExclude()]
        public GridRangeInfo UpdateSelectRange_Range
        {
            get
            {
                return _updateSelectRange_Range;
            }

            set
            {
                _updateSelectRange_Range = value;
            }
        }

        /// <summary>
        /// Gets or sets UpdateSelectRange_OldRange. Used internally.
        /// </summary>
        [Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        [Syncfusion.Documentation.DocumentationExclude()]
        public GridRangeInfoList UpdateSelectRange_OldRange
        {
            get
            {
                return _updateSelectRange_OldRange;
            }

            set
            {
                _updateSelectRange_OldRange = value;
            }
        }

         /// <summary>
        /// Gets or sets a value indicating whether to allow resizing of hidden columns when double click. Default value is true.
        /// </summary>
        [DefaultValue(true)]
        [Description("Enables resizing of hidden columns like Excel.")]
        [Category("Behavior")]
        public bool UnHideColsOnDblClick
        {
            get
            {
                return unHideColsOnDblClick;
            }
            set
            {
                if (unHideColsOnDblClick != value)
                    unHideColsOnDblClick = value;
            }
        }


        /// <overload>
        /// Initializes a new <see cref="GridControlBase"/>.
        /// </overload>
        /// <summary>
        /// Initializes a new <see cref="GridControlBase"/>.
        /// </summary>
        public GridControlBase()
            : this(null)
        {
        }

        /// <summary>
        /// Initializes a new <see cref="GridControlBase"/> and attaches it to a <see cref="GridModel"/>.
        /// </summary>
        /// <param name="model">The <see cref="GridModel"/> this control is associated with.</param>
        public GridControlBase(GridModel model)
        {
            ////[EvalPlaceholder]//
            SetStyle(ControlStyles.ResizeRedraw, false);
            SetStyle(ControlStyles.AllPaintingInWmPaint | ControlStyles.UserPaint | Syncfusion.Windows.Forms.WhidbeyCompatibleControlStyles.DoubleBuffer, true);
            SetStyle(ControlStyles.UserMouse, false);

            this.model = model;
            m_gridPaint = new GridPaint(this);
            this.BackColor = SystemColors.Window;
            sizedColumns = new Dictionary<int, int>();
        }

        /// <summary>
        /// Lets you change <see cref="Control.SetStyle"/>.
        /// </summary>
        /// <param name="style">The window style.</param>
        /// <param name="value">The new value.</param>
        public void SetWindowStyle(ControlStyles style, bool value)
        {
            SetStyle(style, value);
        }

        /// <override/>
        /// <summary>Returns a string holding the current object details.</summary>
        /// <returns>String representation of the current object.</returns>
        public override string ToString()
        {
            return PaneDesc;
        }

        /// <summary>
        /// Gets or sets the <see cref="GridModel"/> that manages data to be displayed in the grid.
        /// </summary>
        /// <remarks>
        /// You can replace the <see cref="GridModel"/> at run-time. The <see cref="GridControlBase"/>
        /// will release and establish links to the previous model and establish new relationship
        /// with the new model, then redraw itself.
        /// </remarks>
        [Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        [EditorBrowsable(EditorBrowsableState.Always)]
        public GridModel Model
        {
            get
            {
                if (model == null)
                {
                    if (this.IsDisposed || this.IsSplitterPaneClosing)
                    {
                        model = null;
                    }
                    else
                    {
                        model = new GridModel();
                    }
                }

                return model;
            }

            set
            {
                if (model != value)
                {
                    if (model != null)
                    {
                        UnwireModel();
                    }

                    model = value;
                    if (model != null)
                    {
                        WireModel();
                    }

                    OnModelChanged(EventArgs.Empty);
                }
            }
        }

        /// <summary>
        /// Returns the Background color to be drawn after the last cell (default is Model.Properties.BackgroundColor)
        /// </summary>
        /// <returns>A Background Color</returns>
        public virtual Color GetBackgroundColor()
        {
            return Model.Properties.BackgroundColor;
        }

        /// <summary>
        /// Returns the VisualStyles (Model.Options.GridVisualStyles)
        /// </summary>
        /// <returns>Returns GridVisualStyles</returns>
        public virtual GridVisualStyles GetGridVisualStyles()
        {
            return Model.Options.GridVisualStyles;
        }

        /// <summary>
        /// Returns the IVisualStyleDrawing (Model.Options.GridVisualStylesDrawing)
        /// </summary>
        /// <returns>IVisualStyleDrawing object.</returns>
        public virtual IVisualStylesDrawing GetGridVisualStylesDrawing()
        {
            return Model.Options.GridVisualStylesDrawing;
        }

        ////        internal void SetModelInt(GridModel model)
        ////        {
        ////            this.Model = model;
        ////        }

        /// <override/>
        /// <summary>Specfies the cursor to be displayed when the mouse pointer is over the control.</summary>
        public override Cursor Cursor
        {
            get
            {
                return base.Cursor;
            }

            set
            {
                base.Cursor = value;
            }
        }

        bool ShouldSerializeCursor()
        {
            return false;
        }

        /// <summary>
        /// Gets a back color, replaces SystemColors.Window with the <see cref="Control.BackColor"/> of this control.
        /// </summary>
        /// <param name="c">The original color.</param>
        /// <returns>The resulting color.</returns>
        public virtual Color GetBackColor(Color c)
        {
            if (c == SystemColors.Window)
            {
                c = this.BackColor;
            }

            if (c.A == 255 && Model.Options.AlphaBlendValue != -1)
            {
                return Color.FromArgb(Model.Options.AlphaBlendValue, c);
            }

            return c;
        }

        /// <summary>
        /// Gets a <see cref="Syncfusion.Drawing.BrushInfo"/>, replaces SystemColors.Window with the <see cref="Control.BackColor"/> of this control.
        /// </summary>
        /// <param name="br">The original <see cref="Syncfusion.Drawing.BrushInfo"/>.</param>
        /// <returns>The resulting <see cref="Syncfusion.Drawing.BrushInfo"/>.</returns>
        public virtual BrushInfo GetInterior(BrushInfo br)
        {
            if ((br.Style == BrushStyle.Solid && br.BackColor == SystemColors.Window) || br.Style == BrushStyle.None)
            {
                return new BrushInfo(this.BackColor);
            }

            if (br.BackColor.A == 255 && br.ForeColor.A == 255 && Model.Options.AlphaBlendValue != -1)
            {
                return new BrushInfo(Model.Options.AlphaBlendValue, br);
            }

            return br;
        }

        /// <summary>
        /// Gets a forecolor, replaces SystemColors.WindowText with the <see cref="Control.ForeColor"/> of this control.
        /// </summary>
        /// <param name="c">The original color.</param>
        /// <returns>The resulting color.</returns>
        public virtual Color GetForeColor(Color c)
        {
            if (c == SystemColors.WindowText)
            {
                return this.ForeColor;
            }

            return c;
        }

        /// <summary>
        /// Gets or sets a value indicating whether it enables or disables vertical scrollbar. Overriden. With <see cref="GridControlBase"/>, use
        /// <see cref="VScrollBehavior"/> instead.
        /// </summary>
        [Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public new bool VScroll
        {
            get
            {
                return base.VScroll;
            }

            set
            {
                base.VScroll = value;
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether it enables or disables horizontal scrollbar. Overriden. With <see cref="GridControlBase"/>, use
        /// <see cref="HScrollBehavior"/> instead.
        /// </summary>
        [Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public new bool HScroll
        {
            get
            {
                return base.HScroll;
            }

            set
            {
                base.HScroll = value;
            }
        }
        /// <summary>
        /// Boolean variable setting whether the alignment in grid should be similar to Excel
        /// </summary>
        bool excelLikeAlignment = false;
        /// <summary>
        /// property which enables to set or get the alignement in Grid as in Excel.
        /// </summary>
        [DefaultValue(false)]
        [Description("Enables Grid to have alignment like as in Excel.")]
        [Category("Behavior")]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public bool ExcelLikeAlignment
        {
            get
            {
                return excelLikeAlignment;
            }
            set
            {
               excelLikeAlignment=value;
            }
        }

        /// <summary>
        /// This event will be fired when the ThemesEnabled property is changed.
        /// </summary>
        [Description("Occurs when the ThemesEnabled property is changed."),
        Category("Behavior")]
        public event EventHandler ThemeChanged;

        /// <summary>
        /// Raises the <see cref="ThemeChanged"/> event.
        /// </summary>
        /// <param name="e">An EventArgs that contains the event data.</param>
        /// <remarks>
        /// <para>The OnThemeChanged method also allows derived classes to handle the event
        /// without attaching a delegate. This is the preferred technique for
        /// handling the event in a derived class.</para>
        /// <para>Notes to Inheritors: When overriding OnThemeChanged in a derived
        /// class, be sure to call the base class's OnThemeChanged method so that
        /// registered delegates receive the event.</para>
        /// </remarks>
        protected virtual void OnThemeChanged(EventArgs e)
        {
            if (this.eventsTarget != null)
            {
                this.eventsTarget.OnThemeChanged(e);
            }
#if DEBUG

            if (Switches.GridControlBaseEvents.TraceVerbose)
            {
                TraceUtil.TraceCurrentMethodInfo(PaneDesc, this.ThemesEnabled);
            }
#else

            ;
#endif

            if (this.ThemeChanged != null)
            {
                try
                {
                    this.ThemeChanged(this, e);
                }
                catch (Exception ex)
                {
                    TraceUtil.TraceExceptionCatched(ex);
                    if (!ExceptionManager.RaiseExceptionCatched(this, ex))
                    {
                        throw;
                    }
                }
            }
        }

        /// <summary>
        /// Occurs when the <see cref="GridControlBase.Model"/> reference to <see cref="GridModel"/> is changed.
        /// </summary>
        [Description("Occurs when the model is replaced."),
        Category("Behavior")]
        public event EventHandler ModelChanged;

        /// <summary>
        /// Raises the <see cref="GridControlBase.ModelChanged"/> event.
        /// </summary>
        /// <param name="e">A <see cref="EventArgs" /> that contains the event data.</param>
        protected virtual void OnModelChanged(EventArgs e)
        {
            if (this.eventsTarget != null)
            {
                this.eventsTarget.OnModelChanged(e);
            }

            if (this.model != null)
            {
#if DEBUG
                if (Switches.GridControlBaseEvents.TraceVerbose)
                {
                    TraceUtil.TraceCurrentMethodInfo(PaneDesc, Model.FileName);
                }
#else
                ;
#endif
                this.VerticalScrollTips = Model.Options.VerticalScrollTips;
                this.HorizontalScrollTips = Model.Options.HorizontalScrollTips;
                this.VerticalThumbTrack = Model.Options.VerticalThumbTrack;
                this.VerticalScrollTips = Model.Options.VerticalScrollTips;

                this.Office2007ScrollBars = Model.Options.Office2007ScrollBars;
                this.Office2007ScrollBarsColorScheme = Model.Options.Office2007ScrollBarsColorScheme;

                this.GridOfficeScrollBars = Model.Options.GridOfficeScrollBars;
                this.Office2010ScrollBarsColorScheme = Model.Options.Office2010ScrollBarsColorScheme;
            }

            CurrentCell.GridModelChanged(this, e);

            if (ModelChanged != null)
            {
                ModelChanged(this, e);
            }
        }

        /// <summary>
        /// Gets or sets the bounds of the grid. This is normally the <see cref="Control.ClientRectangle"/> of a control but
        /// you change <see cref="GridBounds"/> and by doing this instruct the grid that only parts of the current
        /// should be used to display the grid.
        /// </summary>
        /// <remarks>
        /// While in printing mode, <see cref="GridBounds"/> will return printer dimensions.
        /// </remarks>
        [Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public Rectangle GridBounds
        {
            get
            {
                if (PrintingMode)
                {
                    return printBounds;
                }
                else if (bounds.IsEmpty)
                {
                    return this.ClientRectangle;
                }

                return bounds;
            }

            set
            {
                if (bounds != value)
                {
                    bounds = value;
                    ViewLayout.Reset();
                    int dummy = ViewLayout.LastVisibleRow;
                    Model.FloatingCells.EvaluateFloatingCells(ViewLayout.VisibleCellsRange);
                    GridRangeInfo mergeRange = Model.Options.MergeCellsLayout == GridMergeCellsLayout.VisibleRange ? ViewLayout.VisibleCellsRange : GridCellsRange;
                    Model.MergeCells.EvaluateMergeCells(mergeRange);
                    OnGridBoundsChanged(EventArgs.Empty);
                }
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether WM_SYSCHAR need to be handled in TextBox and OriginalTextBox Cell types
        /// to handle universal Keyboard Mappings.
        /// </summary>
        /// <remarks>
        /// This property is mainly added to handle the scenario where Input characters from a universal keyBoard Mapping will be
        /// WM_SYSCHAR (0x104), instead of WM_CHAR (0x102), which needs the TextBox and OriginalTextBox CellTypes to be forced to read 
        /// the Key Messages.
        /// </remarks>
        [Browsable(false), DefaultValue(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public bool HandleWMSYSCHAR
        {
            get
            {
                return this.handleWMSYSCHAR;
            }
            set
            {
                if (this.handleWMSYSCHAR != value)
                    this.handleWMSYSCHAR = value;
            }
        }


        /// <summary>
        /// Specifies if the error icon should be displayed in the header cell if the validation is failed.
        /// </summary>
        [Description("Specifies if the error icon should be displayed in the header cell if the validation is failed.")]
        [Browsable(true), DefaultValue(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Visible),Category("Grid")]
        public bool ShowRowHeaderErroricon
        {
            get
            {
                return showRowHeaderErroricon;
            }

            set
            {
                showRowHeaderErroricon = value;
            }
        }
        /// <summary>
        /// Gets or sets a value to resize the columns proportionally fit its content
        /// </summary>
        [DefaultValue(false)]
        [Browsable(false)]
        [Category("Appearance")]
        [Description("Resizes the columns to proportionally fit its content")]
        public bool AllowProportionalColumnSizing
        {
            get
            {
                return allowProportionalColumnSizing;
            }

            set
            {
                allowProportionalColumnSizing = value;
            }
        }
        /// <summary>
        /// Gets or sets a value to include unicode character in text.
        /// </summary>
        [DefaultValue(false)]
        [Browsable(false)]
        [Category("Appearance")]
        [Description("Includes the RTL Mark in text for displaying the text properly")]
        public bool EnableRTLMark
        {
            get
            {
                return enableRTLMk;
            }

            set
            {
                enableRTLMk = value;
            }
        }

        /// <summary>
        /// Gets a value indicating whether <see cref="GridBounds"/> was initialized. If not, <see cref="GridBounds"/> returns the same
        /// value as <see cref="Control.ClientRectangle"/>.
        /// </summary>
        [Browsable(false)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public bool HasGridBounds
        {
            get
            {
                return !bounds.IsEmpty;
            }
        }

        /// <summary>
        /// Occurs when the <see cref="GridBounds"/> property has been changed.
        /// </summary>
        [Category("Property Changed")]
        [Description("Occurs when the GridBounds property has been changed.")]
        public event EventHandler GridBoundsChanged;

        /// <summary>
        /// Resets the <see cref="GridBounds"/> property.
        /// </summary>
        public void ResetGridBounds()
        {
            GridBounds = Rectangle.Empty;
        }

        /// <summary>
        /// Raises the <see cref="GridControlBase.GridBoundsChanged"/> event.
        /// </summary>
        /// <param name="e">A <see cref="EventArgs" /> that contains the event data.</param>
        protected virtual void OnGridBoundsChanged(EventArgs e)
        {
            if (this.eventsTarget != null)
            {
                this.eventsTarget.OnGridBoundsChanged(e);             
            }
#if DEBUG

            if (Switches.GridControlBaseEvents.TraceVerbose)
            {
                TraceUtil.TraceCurrentMethodInfo(PaneDesc, this.GridBounds);
            }
#else

            ;
#endif
            ViewLayout.Reset();
            if (GridBoundsChanged != null)
            {
                GridBoundsChanged(this, e);
            }
        }

        /// <summary>
        /// Gets the default size of the control.
        /// </summary>
        protected override Size DefaultSize
        {
            get
            {
                return new Size(130, 80);
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether it toggles printing mode for the grid. While in printing mode <see cref="GridBounds"/> will return
        /// the print rectangle of the page being printed and not the client rectangle of the control on
        /// the screen.
        /// </summary>
        /// <remarks>
        /// When printing the grid, the GridPrintDocument class will toggle the
        /// printing mode for the grid. At that time, all drawing related code will use the
        /// printing page as canvas and <see cref="GridBounds"/>.<para/>
        /// Also, <see cref="TopRowIndex"/> and <see cref="LeftColIndex"/> will return values
        /// with top row and left column for the page that is currently being printed.
        /// </remarks>
        [Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public bool PrintingMode
        {
            get
            {
                if (this.IsWindowless)
                {
                    return this.GetGridWindow().PrintingMode;
                }

                return inPrinting;
            }

            set
            {
                if (inPrinting != value)
                {
                    inPrinting = value;
                    Model.Properties.Printing = value;
                    OnPrintingModeChanged(EventArgs.Empty);
                }
            }
        }

        /// <summary>
        /// Method used internally by nested table control to copy LeftColIndex, TopRowIndex and Bounds
        /// to PrintInfo.
        /// </summary>
        [Syncfusion.Documentation.DocumentationExclude()]
        public void InitPrintInfo()
        {
            PrintInfo.m_nPrintLeftCol = this.gridScroll.m_nLeftCol;
            PrintInfo.m_nPrintTopRow = this.gridScroll.m_nTopRow;
            this.printBounds = this.bounds;
        }

        /// <summary>
        /// Method used internally by nested table control to restore GridBounds after printing.
        /// </summary>
        [Syncfusion.Documentation.DocumentationExclude()]
        public void RestoreBoundsAfterPrint()
        {
            this.bounds = this.printBounds;
        }

        /// <summary>
        /// Occurs when the <see cref="PrintingMode"/> has been changed for the grid object.
        /// </summary>
        [Description("Occurs for every cell that is about be redrawn."),
        Category("Behavior")]
        public event EventHandler PrintingModeChanged;

        /// <summary>
        /// Raises the <see cref="GridControlBase.PrintingModeChanged"/> event.
        /// </summary>
        /// <param name="e">A <see cref="GridPrepareViewStyleInfoEventArgs" /> that contains the event data.</param>
        protected virtual void OnPrintingModeChanged(EventArgs e)
        {
            if (this.eventsTarget != null)
            {
                this.eventsTarget.OnPrintingModeChanged(e);
            }
#if DEBUG

            if (Switches.GridControlBaseEvents.TraceVerbose)
            {
                TraceUtil.TraceCurrentMethodInfo(PaneDesc, this.PrintingMode);
            }
#else

            ;
#endif
            ViewLayout.Reset();
            if (PrintingModeChanged != null)
            {
                PrintingModeChanged(this, e);
            }
        }

        /// <summary>
        /// Gets or sets the behavior of the horizontal scrollbar.
        /// </summary>
        /// <value>
        /// A <see cref="GridScrollbarMode"/> enumeration that offers various options for the scrollbar behavior.
        /// </value>
        [Category("Scrolling"),
        Browsable(true),
        Description("Defines the scroll behavior for horizontal scrolling.")]
        public GridScrollbarMode HScrollBehavior
        {
            get
            {
                return this.ScrollGrid.hScrollSetting;
            }

            set
            {
                this.ScrollGrid.hScrollSetting = value;
            }
        }

        void ResetHScrollBehavior()
        {
            this.HScrollBehavior = GridScrollbarMode.DetectIfShared;
        }

        /// <summary>
        /// Gets or sets the behavior of the vertical scrollbar.
        /// </summary>
        /// <value>
        /// A <see cref="GridScrollbarMode"/> enumeration that offers various options for the scrollbar behavior.
        /// </value>
        [Category("Scrolling"),
        Browsable(true),
        Description("Defines the scroll behavior for vertical scrolling.")]
        public GridScrollbarMode VScrollBehavior
        {
            get
            {
                return ScrollGrid.vScrollSetting;
            }

            set
            {
                ScrollGrid.vScrollSetting = value;
            }
        }

        void ResetVScrollBehavior()
        {
            this.VScrollBehavior = GridScrollbarMode.DetectIfShared;
        }

        /// <summary>
        /// Used internally.
        /// </summary>
        /// <returns>returns boolean value to indicate Serialize VScroll Behavior</returns>
        /// <internalonly/>
        [Syncfusion.Documentation.DocumentationExclude()]
        public bool ShouldSerializeVScrollBehavior()
        {
            return ScrollGrid.vScrollSetting != (GridScrollbarMode.Automatic | GridScrollbarMode.AutoScroll)
                && ScrollGrid.vScrollSetting != GridScrollbarMode.DetectIfShared
                && (!this.FillSplitterPane || this.splitterControl == null);
        }

        /// <summary>
        /// Used internally.
        /// </summary>
        /// <returns>returns boolean value to indicate Serialize HScroll Behavior.</returns>
        /// <internalonly/>
        [Syncfusion.Documentation.DocumentationExclude()]
        public bool ShouldSerializeHScrollBehavior()
        {
            return ScrollGrid.hScrollSetting != (GridScrollbarMode.Automatic | GridScrollbarMode.AutoScroll)
                && ScrollGrid.hScrollSetting != GridScrollbarMode.DetectIfShared
                && (!this.FillSplitterPane || this.splitterControl == null);
        }
        private bool disableFormattedTextInEditMode = false;
        /// <summary>
        /// Gets or sets a value indicating whether the formatting of the cell text can be disabled in edit mode.
        /// </summary>
        [DefaultValue(false), Category("Behavior"),
        Description("Specifies whether the formatting of the cell text can be disabled in edit mode.")]
        public bool DisableFormattedTextInEditMode
        {
            get
            {
                return disableFormattedTextInEditMode;
            }
            set
            {
                if (disableFormattedTextInEditMode != value)
                {
                    disableFormattedTextInEditMode = value;
                }
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether XP Themes (visual styles) should be used for this control when
        /// available.
        /// </summary>
        [DefaultValue(false),
        Description("Specifies whether XP Themes should be used for this control when available."),
        Category(@"Appearance")]
        public bool ThemesEnabled
        {
            get 
            { 
                return this.themesEnabled; 
            }

            set
            {
                if (this.themesEnabled != value)
                {
                    this.themesEnabled = value;
                    this.OnThemeChanged(EventArgs.Empty);
                    this.Invalidate();
                }
            }
        }

        /// <summary>
        /// Toggles between standard and Office2007 scrollbars.
        /// </summary>
        [Category("Appearance"),
        Description("Toggle between standard and Office2007 scrollbars."),
        DefaultValue(false)]
        public override bool Office2007ScrollBars
        {
            get
            {
                return base.Office2007ScrollBars;
            }

            set
            {
                base.Office2007ScrollBars = value;
                Model.Options.Office2007ScrollBars = value;
            }
        }

        /// <summary>
        /// Gets / sets the style of Office2007 scroll bars
        /// </summary>
        [Category("Appearance"),
        Browsable(true),
        Description("Office 2007 style scrollbars."),
        DefaultValue(Office2007ColorScheme.Blue)]
        public override Office2007ColorScheme Office2007ScrollBarsColorScheme
        {
            get
            {
                return base.Office2007ScrollBarsColorScheme;
            }

            set
            {
                base.Office2007ScrollBarsColorScheme = value;
                Model.Options.Office2007ScrollBarsColorScheme = value;
            }
        }   
        /// <summary>
        /// Gets or sets MS Office-like scrollbars.
        /// </summary>
        [Category("Appearance"),
        Description("Gets or sets MS Office-like scrollbars."),
        DefaultValue(OfficeScrollBars.None)]
        public override OfficeScrollBars GridOfficeScrollBars
        {
            get
            {
                return base.GridOfficeScrollBars;
            }
            set
            {
                if (value == OfficeScrollBars.Metro)
                    this.MetroScrollBars = true;
                else
                    this.MetroScrollBars = false;
                base.GridOfficeScrollBars = value;
                Model.Options.GridOfficeScrollBars = value;
            }
        }

        /// <summary>
        /// Gets the ScrollControl's MetroScrollBars
        /// </summary>
        [Browsable(false)]
        public override bool MetroScrollBars
        {
            get
            {
                return base.MetroScrollBars;
            }
            set
            {
                base.MetroScrollBars = value;
            }
        }

        /// <summary>
        /// Gets the ScrollControl's MetroColorTable
        /// </summary>
        [Browsable(false)]
        public override MetroColorTable MetroColorTable
        {
            get
            {
                return base.MetroColorTable;
            }
            set
            {
                base.MetroColorTable = value;
            }
        }
        /// <summary>
        /// Gets / sets the style of Office2010 scroll bars
        /// </summary>
        [Category("Appearance"),
        Browsable(true),
        Description("MS-Office 2010 style scrollbars."),
        DefaultValue(Office2010ColorScheme.Blue)]
        public override Office2010ColorScheme Office2010ScrollBarsColorScheme
        {
            get
            {
                return base.Office2010ScrollBarsColorScheme;
            }
            set
            {
                base.Office2010ScrollBarsColorScheme = value;
                Model.Options.Office2010ScrollBarsColorScheme = value;
            }
        }

        #region InitializeGrid
        /// <summary>
        /// Gets or sets a value indicating whether it toggles support for Windows 2000 and Windows XP transparency. Set this true
        /// if you want the grid to draw transparent over a background bitmap.
        /// </summary>
        [Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public bool SupportsTransparentBackColor
        {
            get
            {
                return GetStyle(ControlStyles.SupportsTransparentBackColor);
            }

            set
            {
                if (this.SupportsTransparentBackColor != value)
                {
                    SetStyle(ControlStyles.SupportsTransparentBackColor, value);
                    foreach (Control c in this.Controls)
                    {
                        MakeTransparent(c, this.SupportsTransparentBackColor);
                    }

                    this.OnSupportsTransparentBackColorChanged(EventArgs.Empty);
                }
            }
        }

        /// <summary>
        /// Occurs when the <see cref="SupportsTransparentBackColor"/> has changed.
        /// </summary>
        [Category("Appearance")]
        [Description("Occurs when the SupportsTransparentBackColor property has been changed.")]
        public event EventHandler SupportsTransparentBackColorChanged;

        /// <override/>
        protected override void OnControlAdded(ControlEventArgs e)
        {
            base.OnControlAdded(e);
            MakeTransparent(e.Control, this.SupportsTransparentBackColor);
        }

        /*
                /// <override/>
                protected override void OnControlRemoved(ControlEventArgs e)
                {

                    foreach (DictionaryEntry entry in this.cellRenderers.content)
                    {
                        GridCellRendererBase renderer = (GridCellRendererBase)entry.Value;
                        if (!renderer.IsDisposing && renderer.Control == e.Control)
                        {
                            cellRenderers.content.Remove(entry.Key);
                            renderer.Dispose();
                            break;
                        }
                    }
                    base.OnControlRemoved(e);
                }
        */
        private int padding = 5;
        /// <summary>
        /// When DPI is greater than 100 then the DefaultRowHeight will be set based on the font size.
        /// </summary>
        /// <returns>The Height Value</returns>
        internal int RowHeightOnScaling()
        {
            if (this.Model.ActiveGridView != null)
                using (Graphics graph = this.Model.ActiveGridView.CreateGraphics())
                {
                    Font s = this.Font;
                    float heights = padding + s.GetHeight(graph.DpiY);
                    if (this.DefaultRowHeight > (int)heights)
                        return this.DefaultRowHeight;
                    return (int)Math.Round(heights, 0);
                }
            return this.DefaultRowHeight;
        }
        private bool persistentSettings = false;
        /// <summary>
        /// Gets or sets if the metro theme settings for grid should differ from other themes for look and feel.
        /// </summary>
        [Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public bool PersistAppearanceSettings 
        {
            get
            {
                return persistentSettings;
            }
            set
            {
                persistentSettings = value;
            }
        }
        private void MakeTransparent(Control control, bool value)
        {
            System.Reflection.MethodInfo mInfo = typeof(Control).GetMethod(
                "SetStyle",
                System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.InvokeMethod | System.Reflection.BindingFlags.NonPublic);
            if (mInfo != null)
            {
                mInfo.Invoke(control, new object[] { ControlStyles.SupportsTransparentBackColor, value });
            }
        }

        /// <summary>
        /// Raises the <see cref="SupportsTransparentBackColorChanged"/> event.
        /// </summary>
        /// <param name="e">A <see cref="EventArgs" /> that contains the event data.</param>
        protected virtual void OnSupportsTransparentBackColorChanged(EventArgs e)
        {
            if (this.eventsTarget != null)
            {
                this.eventsTarget.OnSupportsTransparentBackColorChanged(e);
            }

            if (SupportsTransparentBackColorChanged != null)
            {
                SupportsTransparentBackColorChanged(this, e);
            }
        }

        #region BackgroundImageID
        /// <summary>
        /// Gets or sets the Namespace ID that contains the grids's background image information id.
        /// </summary>
        /// <remarks>
        /// <para/>
        /// The default value for the BackgroundImageID property is "".<para/>
        /// <para/>
        /// </remarks>
        [Description("Gets / sets the namespace id for the image to be displayed."),
        Browsable(false),
        Category("Image"),
        DefaultValue("")]
        [NotifyParentProperty(true)]
        public string BackgroundImageID
        {
            get
            {
                return backgroundImageID;
            }

            set
            {
                backgroundImageID = value;
            }
        }

        /// <summary>
        /// Resets BackgroundImageID state.
        /// </summary>
        public void ResetBackgroundImageID()
        {
            backgroundImageID = string.Empty;
        }

        [EditorBrowsableAttribute(EditorBrowsableState.Never)]
        private bool ShouldSerializeBackgroundImageID()
        {
            return backgroundImageID != string.Empty;
        }

        /// <summary>
        /// Gets a value indicating whether BackgroundImage state has been initialized for the current object.
        /// </summary>
        [Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public bool HasBackgroundImageID
        {
            get
            {
                return backgroundImageID != string.Empty;
            }
        }
        #endregion

        /// <override/>
        /// <summary>Specifies the background color for the grid.</summary>
        public override Color BackColor
        {
            get { return base.BackColor; }
            set { base.BackColor = value; }
        }

        bool ShouldSerializeBackColor()
        {
            return BackColor != SystemColors.Window;
        }

        /// <summary>
        /// Initializes all mouse controllers for this grid. See <see cref="GridControllerOptions"/> for
        /// default mouse controllers that you can enable and disable through the <see cref="GridModelOptions.ControllerOptions"/>
        /// of the <see cref="GridModel.Options"/> property.
        /// </summary>
        /// <remarks>
        /// Controllers will be added and removed from <see cref="ScrollControl.MouseControllerDispatcher"/>.
        /// </remarks>
        protected virtual void InitializeMouseControllers()
        {
        }

        /// <summary>
        /// Resets all mouse controllers and removes them from the <see cref="ScrollControl.MouseControllerDispatcher"/>.
        /// </summary>
        protected virtual void ResetMouseControllers()
        {
        }

        /// <summary>
        /// Initializes all data object consumers for this grid. See <see cref="GridDataObjectConsumerOptions"/> for
        /// default consumers that you can enable and disable through the <see cref="GridModelOptions.DataObjectConsumerOptions"/> property
        /// of the <see cref="GridModel.Options"/> property.
        /// </summary>
        /// <remarks>
        /// Controllers will be registered with by <see cref="GridControlBaseImp.RegisterDataObjectConsumer"/>.
        /// </remarks>
        protected virtual void InitializeDataObjectConsumerOptions()
        {
        }

        /// <summary>
        /// Overriden. Calls <see cref="GridControlBase.Initialize"/> when the control is added to the parent forms
        /// <see cref="Control.Controls"/> collection.
        /// </summary>
        protected override void InitLayout()
        {
            if (Model.ActiveGridView == null)
            {
                Model.ActiveGridView = this;
            }

            if (!initialized)
            {
                Initialize();
            }

            base.InitLayout();
        }

        /// <summary>
        /// Sets up listeners for the <see cref="GridModel"/> and initializes mouse controllers and data object consumers.
        /// </summary>
        protected virtual void WireModel()
        {
            InitializeMouseControllers();
            InitializeDataObjectConsumerOptions();

            Model.NotifyResetVolatileData += new EventHandler(ModelNotifyResetVolatileData);
            Model.FrozenRowCountChanged += new GridCountChangedEventHandler(ModelFixedRowChanged);
            Model.FrozenColCountChanged += new GridCountChangedEventHandler(ModelFixedColChanged);
            Model.HeaderRowCountChanged += new GridCountChangedEventHandler(ModelHeaderRowChanged);
            Model.HeaderColCountChanged += new GridCountChangedEventHandler(ModelHeaderColChanged);
            Model.CellsChanged += new GridCellsChangedEventHandler(ModelCellsChanged);
            Model.RowHeightsChanging += new GridRowColSizeChangingEventHandler(ModelRowHeightsChanging);
            Model.RowHeightsChanged += new GridRowColSizeChangedEventHandler(ModelRowHeightsChanged);
            Model.ColWidthsChanging += new GridRowColSizeChangingEventHandler(ModelColWidthsChanging);
            Model.ColWidthsChanged += new GridRowColSizeChangedEventHandler(ModelColWidthsChanged);
            Model.RowsHidden += new GridRowColHiddenEventHandler(ModelRowsHidden);
            Model.ColsHidden += new GridRowColHiddenEventHandler(ModelColsHidden);
            Model.DefaultRowHeightChanged += new GridDefaultSizeChangedEventHandler(ModelDefaultRowHeightChanged);
            Model.DefaultColWidthChanged += new GridDefaultSizeChangedEventHandler(ModelDefaultColWidthChanged);
            Model.ColsMoved += new GridRangeMovedEventHandler(ModelColsRangeMoved);
            Model.ColsMoving += new GridRangeMovingEventHandler(ModelColsRangeMoving);
            Model.RowsMoved += new GridRangeMovedEventHandler(ModelRowsRangeMoved);
            Model.RowsMoving += new GridRangeMovingEventHandler(ModelRowsRangeMoving);
            Model.ColsRemoved += new GridRangeRemovedEventHandler(ModelColsRangeRemoved);
            Model.ColsRemoving += new GridRangeRemovingEventHandler(ModelColsRangeRemoving);
            Model.RowsRemoved += new GridRangeRemovedEventHandler(ModelRowsRangeRemoved);
            Model.RowsRemoving += new GridRangeRemovingEventHandler(ModelRowsRangeRemoving);
            Model.ColsInserted += new GridRangeInsertedEventHandler(ModelColsRangeInserted);
            Model.ColsInserting += new GridRangeInsertingEventHandler(ModelColsRangeInserting);
            Model.RowsInserted += new GridRangeInsertedEventHandler(ModelRowsRangeInserted);
            Model.RowsInserting += new GridRangeInsertingEventHandler(ModelRowsRangeInserting);
            Model.CoveredRangesChanging += new GridCoveredRangesChangingEventHandler(ModelCoveredRangesChanging);
            Model.CoveredRangesChanged += new GridCoveredRangesChangedEventHandler(ModelCoveredRangesChanged);
            Model.BanneredRangesChanging += new GridBanneredRangesChangingEventHandler(ModelBanneredRangesChanging);
            Model.BanneredRangesChanged += new GridBanneredRangesChangedEventHandler(ModelBanneredRangesChanged);
            Model.MergeCellsChanged += new GridMergeCellsChangedEventHandler(ModelMergeCellsChanged);
            ////            Model.SelectionChanged += new GridSelectionChangedEventHandler(ModelSelectionChanged);
            ////            Model.PrepareClearSelection += new EventHandler(ModelPrepareClearSelection);
            ////            Model.PrepareChangeSelection += new GridPrepareChangeSelectionEventHandler(ModelPrepareChangeSelection);
            ////            Model.FloatingCellsChanging += new GridFloatingCellsChangingEventHandler(ModelFloatingCellsChanging);
            Model.FloatingCellsChanged += new GridFloatingCellsChangedEventHandler(ModelFloatingCellsChanged);
            Model.RefreshRequest += new EventHandler(ModelRefreshRequest);
            Model.InvalidateRangeRequest += new GridInvalidateRangeRequestEventHandler(ModelInvalidateRangeRequest);
            Model.BeginUpdateRequest += new EventHandler(ModelBeginUpdateRequest);
            Model.EndUpdateRequest += new GridEndUpdateRequestEventHandler(ModelEndUpdateRequest);
            ////            Model.ChangingLayoutCells += new GridChangeLayoutCellsEventHandler(ModelChangingLayoutCells);
            ////Model.ChangedLayoutCells += new GridChangeLayoutCellsEventHandler(ModelChangedLayoutCells);
            Model.Options.OptionsChanged += new EventHandler(ModelOptionsChanged);
            Model.Options.ControllerOptionsChanged += new EventHandler(ModelControllerOptionsChanged);
            Model.Options.DataObjectConsumerOptionsChanged += new EventHandler(ModelDataObjectConsumerOptionsChanged);
            ////Model.CurrentCellMoved += new GridChangeLayoutCellsEventHandler(ModelCurrentCellMoved);

            Model.Properties.Changed += new EventHandler(PropertiesChanged);

            foreach (string key in CellRenderers.Keys)
            {
                GridCellRendererBase cellRenderer = CellRenderers[key];
                cellRenderer.Model = Model.CellModels[key];
            }
        }

        void PropertiesChanged(object sender, EventArgs e)
        {
            Refresh();
        }

        void ModelOptionsChanged(object sender, EventArgs e)
        {
            base.VerticalScrollTips = Model.Options.VerticalScrollTips;
            base.HorizontalScrollTips = Model.Options.HorizontalScrollTips;
            base.VerticalThumbTrack = Model.Options.VerticalThumbTrack;
            base.HorizontalThumbTrack = Model.Options.HorizontalThumbTrack;

            base.Office2007ScrollBars = Model.Options.Office2007ScrollBars;
            base.Office2007ScrollBarsColorScheme = Model.Options.Office2007ScrollBarsColorScheme;
            base.GridOfficeScrollBars = Model.Options.GridOfficeScrollBars;
            base.Office2010ScrollBarsColorScheme = Model.Options.Office2010ScrollBarsColorScheme;
            Refresh();
        }

        /// <summary>
        /// Specifies if the control should scroll while the user is dragging a vertical scrollbar thumb.
        /// </summary>
        [Browsable(true),
        Category("Scrolling"),
        Description("Specifies if the control should scroll while the user is dragging a vertical scrollbar thumb."),
        DefaultValue(false)]
        public override bool VerticalThumbTrack
        {
            get
            {
                return base.VerticalThumbTrack;
            }

            set
            {
                base.VerticalThumbTrack = value;
                Model.Options.VerticalThumbTrack = value;
            }
        }

        /// <summary>
        /// Specifies if the control should scroll while the user is dragging a horizontal scrollbars thumb.
        /// </summary>
        [Browsable(true),
        Category("Scrolling"),
        Description("Specifies if the control should scroll while the user is dragging a horizontal scrollbar thumb."),
        DefaultValue(false)]
        public override bool HorizontalThumbTrack
        {
            get
            {
                return base.HorizontalThumbTrack;
            }

            set
            {
                base.HorizontalThumbTrack = value;
                Model.Options.HorizontalThumbTrack = value;
            }
        }

        /// <summary>
        /// Specifies if the control should show Scroll Tips while the user is dragging a vertical scrollbar thumb.
        /// </summary>
        [Browsable(true),
        Category("Scrolling"),
        Description("Specifies if the control should show Scroll Tips while the user is dragging a vertical scrollbar thumb."),
        DefaultValue(false)]
        public override bool VerticalScrollTips
        {
            get
            {
                return base.VerticalScrollTips;
            }

            set
            {
                base.VerticalScrollTips = value;
                Model.Options.VerticalScrollTips = value;
            }
        }

        /// <summary>
        /// Specifies if the control should show Scroll Tips while the user is dragging a horizontal scrollbar thumb.
        /// </summary>
        [Browsable(true),
        Category("Scrolling"),
        Description("Specifies if the control should show Scroll Tips while the user is dragging a horizontal scrollbar thumb."),
        DefaultValue(false)]
        public override bool HorizontalScrollTips
        {
            get
            {
                return base.HorizontalScrollTips;
            }

            set
            {
                base.HorizontalScrollTips = value;
                Model.Options.HorizontalScrollTips = value;
            }
        }

        void ModelControllerOptionsChanged(object sender, EventArgs e)
        {
            this.InitializeMouseControllers();
        }

        void ModelDataObjectConsumerOptionsChanged(object sender, EventArgs e)
        {
            this.InitializeDataObjectConsumerOptions();
        }

        /// <summary>
        /// Releases listeners for the <see cref="GridModel"/>.
        /// </summary>
        protected virtual void UnwireModel()
        {
            if (Model != null)
            {
                Model.NotifyResetVolatileData -= new EventHandler(ModelNotifyResetVolatileData);
                Model.FrozenRowCountChanged -= new GridCountChangedEventHandler(ModelFixedRowChanged);
                Model.FrozenColCountChanged -= new GridCountChangedEventHandler(ModelFixedColChanged);
                Model.HeaderRowCountChanged -= new GridCountChangedEventHandler(ModelHeaderRowChanged);
                Model.HeaderColCountChanged -= new GridCountChangedEventHandler(ModelHeaderColChanged);
                Model.CellsChanged -= new GridCellsChangedEventHandler(ModelCellsChanged);
                Model.RowHeightsChanging -= new GridRowColSizeChangingEventHandler(ModelRowHeightsChanging);
                Model.RowHeightsChanged -= new GridRowColSizeChangedEventHandler(ModelRowHeightsChanged);
                Model.ColWidthsChanging -= new GridRowColSizeChangingEventHandler(ModelColWidthsChanging);
                Model.ColWidthsChanged -= new GridRowColSizeChangedEventHandler(ModelColWidthsChanged);
                Model.RowsHidden -= new GridRowColHiddenEventHandler(ModelRowsHidden);
                Model.ColsHidden -= new GridRowColHiddenEventHandler(ModelColsHidden);
                Model.DefaultRowHeightChanged -= new GridDefaultSizeChangedEventHandler(ModelDefaultRowHeightChanged);
                Model.DefaultColWidthChanged -= new GridDefaultSizeChangedEventHandler(ModelDefaultColWidthChanged);
                Model.ColsMoved -= new GridRangeMovedEventHandler(ModelColsRangeMoved);
                Model.ColsMoving -= new GridRangeMovingEventHandler(ModelColsRangeMoving);
                Model.RowsMoved -= new GridRangeMovedEventHandler(ModelRowsRangeMoved);
                Model.RowsMoving -= new GridRangeMovingEventHandler(ModelRowsRangeMoving);
                Model.ColsRemoved -= new GridRangeRemovedEventHandler(ModelColsRangeRemoved);
                Model.ColsRemoving -= new GridRangeRemovingEventHandler(ModelColsRangeRemoving);
                Model.RowsRemoved -= new GridRangeRemovedEventHandler(ModelRowsRangeRemoved);
                Model.RowsRemoving -= new GridRangeRemovingEventHandler(ModelRowsRangeRemoving);
                Model.ColsInserted -= new GridRangeInsertedEventHandler(ModelColsRangeInserted);
                Model.ColsInserting -= new GridRangeInsertingEventHandler(ModelColsRangeInserting);
                Model.RowsInserted -= new GridRangeInsertedEventHandler(ModelRowsRangeInserted);
                Model.RowsInserting -= new GridRangeInsertingEventHandler(ModelRowsRangeInserting);
                Model.CoveredRangesChanging -= new GridCoveredRangesChangingEventHandler(ModelCoveredRangesChanging);
                Model.CoveredRangesChanged -= new GridCoveredRangesChangedEventHandler(ModelCoveredRangesChanged);
                Model.MergeCellsChanged -= new GridMergeCellsChangedEventHandler(ModelMergeCellsChanged);
                Model.BanneredRangesChanging -= new GridBanneredRangesChangingEventHandler(ModelBanneredRangesChanging);
                Model.BanneredRangesChanged -= new GridBanneredRangesChangedEventHandler(ModelBanneredRangesChanged);
                ////                Model.SelectionChanged -= new GridSelectionChangedEventHandler(ModelSelectionChanged);
                ////                Model.PrepareClearSelection -= new EventHandler(ModelPrepareClearSelection);
                ////                Model.PrepareChangeSelection -= new GridPrepareChangeSelectionEventHandler(ModelPrepareChangeSelection);
                ////                Model.FloatingCellsChanging -= new GridFloatingCellsChangingEventHandler(ModelFloatingCellsChanging);
                Model.FloatingCellsChanged -= new GridFloatingCellsChangedEventHandler(ModelFloatingCellsChanged);
                Model.RefreshRequest -= new EventHandler(ModelRefreshRequest);
                Model.InvalidateRangeRequest -= new GridInvalidateRangeRequestEventHandler(ModelInvalidateRangeRequest);
                Model.BeginUpdateRequest -= new EventHandler(ModelBeginUpdateRequest);
                Model.EndUpdateRequest -= new GridEndUpdateRequestEventHandler(ModelEndUpdateRequest);
                ////                    Model.ChangingLayoutCells -= new GridChangeLayoutCellsEventHandler(ModelChangingLayoutCells);
                ////Model.ChangedLayoutCells -= new GridChangeLayoutCellsEventHandler(ModelChangedLayoutCells);
                ////Model.CurrentCellMoved -= new GridChangeLayoutCellsEventHandler(ModelCurrentCellMoved);
                Model.Options.OptionsChanged -= new EventHandler(ModelOptionsChanged);
                Model.Options.ControllerOptionsChanged -= new EventHandler(ModelControllerOptionsChanged);
                Model.Options.DataObjectConsumerOptionsChanged -= new EventHandler(ModelDataObjectConsumerOptionsChanged);
                Model.Properties.Changed -= new EventHandler(PropertiesChanged);
            }

            if (this.cellRenderers != null)
            {
                foreach (GridCellRendererBase cellRenderer in this.cellRenderers.Values)
                {
                    cellRenderer.IntUnwireModel(cellRenderer.cellModel);
                }
            }
        }

        bool initialized = false;
        
        void ModelNotifyResetVolatileData(object sender, EventArgs e)
        {
            ViewLayout.Reset();
        }

        /// <override/>
        protected override void OnVisibleChanged(EventArgs e)
        {
            if (this.eventsTarget != null)
            {
                this.eventsTarget.OnVisibleChanged(e);
            }

            if (this.IsSplitterPaneClosing)
            {
                return;
            }

#if DEBUG
            if (!Disposing)
            {
                if (Switches.Development.TraceVerbose)
                {
                    TraceUtil.TraceCurrentMethodInfo(PaneDesc, "C", Created, "U", Updating, "V", Visible, Size, PaneDesc);
                }
            }
#endif

            if (Visible && !Disposing && initialized && Visible)
            {
                ViewLayout.Reset();
                UpdateScrollBars();
            }
#if DEBUG
            if (Switches.GridControlBaseEvents.TraceVerbose)
            {
                TraceUtil.TraceCurrentMethodInfo(PaneDesc, this.Visible);
            }
#else
            ;
#endif
            if (AllowProportionalColumnSizing)
            {
                SetColumnWidths();
            }
            base.OnVisibleChanged(e);
        }

        private ArrayList subComponents = new ArrayList();

        /// <override/>
        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                SuspendLayout();
                UnwireModel();

                foreach (object obj in this.subComponents)
                {
                    if (obj is IDisposable)
                    {
                        ((IDisposable)obj).Dispose();
                    }
                }

                if (this.cellRenderers != null)
                {
                    this.cellRenderers.Dispose();
                }

                cellRenderers = null;
                ScrollGrid.Dispose();
                this.scrollersFrame1.Dispose();
                this.eventsTarget = null;

                splitterControl = null;
            }

            base.Dispose(disposing);
        }

        /// <override/>
        protected override void OnSplitterPaneClosing(EventArgs e)
        {
            if (this.eventsTarget != null)
            {
                this.eventsTarget.OnSplitterPaneClosing(e);
            }

            base.OnSplitterPaneClosing(e);
            ScrollGrid.Dispose();
            CurrentCell.Deactivate(true);
            this.UnwireModel();
            ////this.Model = null;
        }

        /// <summary>
        /// Creates a new <see cref="GridControlBase"/> and attaches it to the same <see cref="GridModel"/>.
        /// </summary>
        /// <param name="parent">A parent control. Can be a <see cref="SplitterControl"/>.</param>
        /// <param name="row">The row in a <see cref="SplitterControl"/>.</param>
        /// <param name="column">The column in a <see cref="SplitterControl"/>.</param>
        /// <returns>A new instance of <see cref="GridControlBase"/>.</returns>
        public virtual Control CreateNewControl(Control parent, int row, int column)
        {
            try
            {
                GridControlBase grid1 = (GridControlBase)Activator.CreateInstance(this.GetType(), new object[] { this.Model });
                OnCreatedNewControl(grid1, row, column);
                return grid1;
            }
            catch (Exception ex)
            {
                TraceUtil.TraceExceptionCatched(ex);
                if (!ExceptionManager.RaiseExceptionCatched(this, ex))
                {
                    throw;
                }
            }

            GridControlBase grid2 = (GridControlBase)Activator.CreateInstance(this.GetType());
            grid2.Model = this.Model;
            OnCreatedNewControl(grid2, row, column);
            return grid2;
        }

        /// <summary>
        ///     This virtual method is called after <see cref="CreateNewControl"/> successfully created a new control
        ///     and lets you initialize properties for the new control before it is displayed.
        /// </summary>
        /// <param name="grid">The new grid control.</param>
        /// <param name="row">The splitter row.</param>
        /// <param name="column">The splitter column.</param>
        protected virtual void OnCreatedNewControl(GridControlBase grid, int row, int column)
        {
            grid.vPixelScroll = this.vPixelScroll;
            grid.hPixelScroll = this.hPixelScroll;
            ////grid.hScrollPixelDelta = this.hScrollPixelDelta;
            ////grid.vScrollPixelDelta = this.vScrollPixelDelta;
            grid.HScrollIncrement = this.HScrollIncrement;
            grid.VScrollIncrement = this.VScrollIncrement;
            grid.optimizeDrawBackground = this.optimizeDrawBackground;
            grid.optimizeInsertRemoveCells = this.optimizeInsertRemoveCells;
            grid.wantsEnterKey = this.wantsEnterKey;
            grid.wantsEscapeKey = this.wantsEscapeKey;
            grid.wantKeys = this.wantKeys;
        }

        void InitSplitter()
        {
            splitterControl = (IDynamicSplitterFrame)GridUtil.GetParentControl(this, typeof(IDynamicSplitterFrame));

            int row = m_nSplitRow;
            int col = m_nSplitCol;

            if (splitterControl != null && splitterControl.FindPane(this, out m_nSplitRow, out m_nSplitCol))
            {
                //// Display row headers only in upper pane.
                displayHeaderRow = m_nSplitRow == 0;

                //// Display column headers only in left pane.
                displayHeaderCol = m_nSplitCol == 0;

                if (m_nSplitRow != row)
                {
                    ScrollGrid.RecalcHiddenRowState(0, Model.Rows.HeaderCount + 1);
                }

                if (m_nSplitCol != col)
                {
                    ScrollGrid.RecalcHiddenColState(0, Model.Cols.HeaderCount + 1);
                }

                GridControlBase otherGrid;
                if (m_nSplitRow == 1)
                {
                    otherGrid = splitterControl.GetPane(0, m_nSplitCol) as GridControlBase;
                    if (otherGrid != null)
                    {
                        ScrollGrid.m_nLeftCol = otherGrid.LeftColIndex;
                        ViewLayout.Reset();
                    }
                }

                if (m_nSplitCol == 1)
                {
                    otherGrid = splitterControl.GetPane(m_nSplitRow, 0) as GridControlBase;
                    if (otherGrid != null)
                    {
                        ScrollGrid.m_nTopRow = otherGrid.TopRowIndex;
                        ViewLayout.Reset();
                    }
                }

                if (m_nSplitRow == 1 || m_nSplitCol == 1)
                {
                    otherGrid = splitterControl.GetPane(0, 0) as GridControlBase;
                    if (otherGrid != null)
                    {
                        ScrollGrid.RecalcHiddenRowColState(otherGrid);
                    }
                }

                this.VScrollBehavior = GridScrollbarMode.Shared | GridScrollbarMode.AutoScroll;
                this.HScrollBehavior = GridScrollbarMode.Shared | GridScrollbarMode.AutoScroll;

                if (row != m_nSplitRow || col != m_nSplitCol)
                {
                    Invalidate();
                }
            }
            else
            {
                m_nSplitRow = m_nSplitCol = 0;
            }
        }

        /// <summary>
        /// Initializes the control after it has been added to the parent's <see cref="Control.Controls"/> collection.
        /// </summary>
        public virtual void Initialize()
        {
            InitSplitter();
            if (!initialized)
            {
                WireModel();
            }

            ScrollGrid.m_nLeftCol = Math.Max(GetFirstScrollableCol(), ScrollGrid.m_nLeftCol);
            ScrollGrid.m_nTopRow = Math.Max(GetFirstScrollableRow(), ScrollGrid.m_nTopRow);
            ViewLayout.Reset();

            initialized = true;

            UpdateScrollBars();
        }

        #endregion
        #region Misc

        /// <summary>
        /// Creates a <see cref="Graphics"/> object for this control and raises a <see cref="GridModel.PrepareGraphics"/> event.
        /// The graphics object must be disposed after usage.
        /// </summary>
        /// <returns>A <see cref="Graphics"/> object.</returns>
        /// <remarks>
        /// Raises a <see cref="GridModel.PrepareGraphics"/> event.
        /// </remarks>
        public virtual Graphics CreateGridGraphics()
        {
            GridControlBase control = GetGridWindow();
            Graphics g;
            if (this.HasDoubleBufferSurface)
            {
                g = DoubleBufferSurface.Graphics;
            }
            else
            {
                g = control.CreateGraphics();
            }

            control.Model.DoPrepareGraphics(g);
            return g;
        }

        ////protected override void OnPrepareDoubleBufferSurfaceGraphics(Graphics gr)
        ////{
        ////    GridControlBase control = GetGridWindow();
        ////    control.Model.DoPrepareGraphics(gr);
        ////}

        /// <summary>
        /// Scrolls the contents of the control.
        /// </summary>
        /// <param name="xAmount">Horizontal scroll offset in pixels..</param>
        /// <param name="yAmount">Vertical scroll offset in pixels.</param>
        /// <param name="rect">Scroll bounds.</param>
        /// <param name="clipRect">Clipping rectangle.</param>
        /// <param name="allowUpdate">If true, redraws the invalidated regions within its client area.</param>
        /// <returns>Resultant rectangle that was scrolled into view.</returns>
        /// <remarks></remarks>
        public override Rectangle ScrollWindow(int xAmount, int yAmount, Rectangle rect, Rectangle clipRect, bool allowUpdate)
        {
            if (!this.HasDoubleBufferSurface)
            {
                return base.ScrollWindow(xAmount, yAmount, rect, clipRect, allowUpdate);
            }

            OnWindowScrolling(new ScrollWindowEventArgs(xAmount, yAmount, rect, clipRect, Rectangle.Empty));

            // Note: there might be a problem when changing column widths in a grid and
            // using ScrollWindow because renderOriginPoint should not be changed then.
            OffsetRenderOriginPoint(xAmount, yAmount);

            Rectangle updateRect = DoubleBufferSurface.ScrollWindow(xAmount, yAmount, rect, clipRect);

            if (allowUpdate)
            {
                Graphics g = DoubleBufferSurface.Graphics;

                Rectangle invalidBounds = InvalidBounds;

                if (invalidBounds.IntersectsWith(rect))
                {
                    invalidBounds.Offset(xAmount, yAmount);
                    invalidBounds = Rectangle.Union(InvalidBounds, invalidBounds);
                    InvalidBounds = invalidBounds;
                }

                ////doubleBufferSurface.Graphics.FillRectangle(new SolidBrush(Color.Red), updateRect);
                ////doubleBufferSurface.Graphics.FillRectangle(new SolidBrush(Color.Blue), invalidBounds);
                Rectangle r = updateRect;
                if (updateRect.IntersectsWith(invalidBounds))
                {
                    r = Rectangle.Union(updateRect, invalidBounds);
                    DrawClippedGrid(g, r, true);
                }
                else
                {
                    DrawClippedGrid(g, r, true);
                    if (!invalidBounds.IsEmpty)
                    {
                        DrawClippedGrid(g, invalidBounds, true);
                    }
                }

                InvalidBounds = Rectangle.Empty;
                DoubleBufferSurface.Dirty = true;
                DoubleBufferSurface.Render();
            }

            OnWindowScrolled(new ScrollWindowEventArgs(xAmount, yAmount, rect, clipRect, updateRect));

            return updateRect;
        }

        /// <summary>
        /// Handles a <see cref="GridModel.RefreshRequest"/> event from the <see cref="GridModel"/> and
        /// redraws the current view.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">An <see cref="EventArgs"/> with event data.</param>
        virtual protected void ModelRefreshRequest(object sender, EventArgs e)
        {
#if DEBUG
            if (Switches.General.TraceVerbose)
            {
                TraceUtil.TraceCurrentMethodInfo(PaneDesc, e);
            }
#else
            ;
#endif

            if (!Model.Initializing)
            {
                Refresh(true);
            }
        }

        /// <summary>
        /// Handles a <see cref="GridModel.InvalidateRangeRequest"/> event from the <see cref="GridModel"/> and
        /// invalidates the range of cells in the current view.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">An <see cref="GridInvalidateRangeRequestEventArgs"/> with event data.</param>
        virtual protected void ModelInvalidateRangeRequest(object sender, GridInvalidateRangeRequestEventArgs e)
        {
#if DEBUG
            if (Switches.General.TraceVerbose)
            {
                TraceUtil.TraceCurrentMethodInfo(PaneDesc, e);
            }
#else
            ;
#endif

            if (!Model.Initializing)
            {
                InvalidateRange(e.Range, e.Options);
                ////                if (e.Range.Contains(CurrentCell.RangeInfo))
                ////                    CurrentCell.Refresh();
            }
        }

        /// <summary>
        /// Converts the given range from absolute row and column indexes to client
        /// row and column indexes.
        /// </summary>
        /// <param name="range">The <see cref="GridRangeInfo"/> to be converted.</param>
        /// <returns>A <see cref="GridRangeInfo"/> with client row and column indexes.</returns>
        public GridRangeInfo MakeClientRange(GridRangeInfo range)
        {
            if (range.IsCells)
            {
                return GridRangeInfo.Cells(GetClientRow(range.Top), GetClientCol(range.Left), GetClientRow(range.Bottom), GetClientCol(range.Right));
            }
            else if (range.IsCols)
            {
                return GridRangeInfo.Cols(GetClientCol(range.Left), GetClientCol(range.Right));
            }
            else if (range.IsRows)
            {
                return GridRangeInfo.Rows(GetClientRow(range.Top), GetClientRow(range.Bottom));
            }
            else
            {
                return range;
            }
        }

        internal bool IsPrinting()
        {
            return PrintingMode;
        }

        internal IntPtr SendMessage(int msg, IntPtr wparam, IntPtr lparam)
        {
            return NativeMethods.SendMessage(GetWindow().Handle, msg, wparam, lparam);
        }

        internal IntPtr SendMessage(int msg, IntPtr wparam, int lparam)
        {
            return NativeMethods.SendMessage(GetWindow().Handle, msg, wparam, (IntPtr)lparam);
        }

        internal IntPtr SendMessage(int msg, int wparam, IntPtr lparam)
        {
            return NativeMethods.SendMessage(GetWindow().Handle, msg, (IntPtr)wparam, lparam);
        }

        internal IntPtr SendMessage(int msg, int wparam, int lparam)
        {
            return NativeMethods.SendMessage(GetWindow().Handle, msg, (IntPtr)wparam, (IntPtr)lparam);
        }
        #endregion
        #region DragSelectUI
        /// <summary>
        /// Occurs when the user is about to drag or is in the process of dragging a selected range of columns or rows.
        /// </summary>
        /// <remarks>
        /// Raised after marker is drawn to give visual feedback about new position.
        /// <para/>
        ///  See <see cref="GridSelectionDragEventArgs"/> for further discussion.
        /// </remarks>
        [Description("Occurs when the user is about to drag or is in the process of dragging a selected range of columns or rows."),
        Category("Behavior")]
        public event GridSelectionDragEventHandler SelectionDragged;

        /// <summary>
        /// Occurs when the user is about to drag or is in the process of dragging a selected range of columns or rows.
        /// </summary>
        /// <remarks>
        /// Raised before new marker is drawn.
        /// <para/>
        /// See <see cref="GridSelectionDragEventArgs"/> for further discussion.
        /// </remarks>
        [Description("Occurs when the user is in the process of dragging a selected range of columns or rows."),
        Category("Behavior")]
        public event GridSelectionDragEventHandler SelectionDragging;

        /// <summary>
        /// Initiates call to <see cref="OnSelectionDragging"/>.
        /// </summary>
        /// <param name="e">The event arguments.</param>
        public void RaiseSelectionDragging(GridSelectionDragEventArgs e)
        {
            OnSelectionDragging(e);
        }

        /// <summary>
        /// Initiates call to <see cref="OnSelectionDragged"/>.
        /// </summary>
        /// <param name="e">The event arguments.</param>
        public void RaiseSelectionDragged(GridSelectionDragEventArgs e)
        {
            OnSelectionDragged(e);
        }

        /// <summary>
        /// Raises the <see cref="GridControlBase.SelectionDragging" /> event.
        /// </summary>
        /// <param name="e">An <see cref="GridSelectionDragEventArgs" /> that contains the event data.</param>
        protected virtual void OnSelectionDragging(GridSelectionDragEventArgs e)
        {
            if (this.eventsTarget != null)
            {
                this.eventsTarget.OnSelectionDragging(e);
            }
#if DEBUG

            if (Switches.GridControlBaseEvents.TraceVerbose)
            {
                TraceUtil.TraceCurrentMethodInfo(PaneDesc, e);
            }
#else

            ;
#endif

            if (SelectionDragging != null)
            {
                try
                {
                    SelectionDragging(this, e);
                }
                catch (Exception ex)
                {
                    TraceUtil.TraceExceptionCatched(ex);
                    if (!ExceptionManager.RaiseExceptionCatched(this, ex))
                    {
                        throw;
                    }

                    e.Cancel = true;
                }
            }
        }

        /// <summary>
        /// Raises the <see cref="GridControlBase.SelectionDragged" /> event.
        /// </summary>
        /// <param name="e">An <see cref="GridSelectionDragEventArgs" /> that contains the event data.</param>
        protected virtual void OnSelectionDragged(GridSelectionDragEventArgs e)
        {
            if (this.eventsTarget != null)
            {
                this.eventsTarget.OnSelectionDragged(e);
            }
#if DEBUG

            if (Switches.GridControlBaseEvents.TraceVerbose)
            {
                TraceUtil.TraceCurrentMethodInfo(PaneDesc, e);
            }
#else

            ;
#endif

            if (SelectionDragged != null)
            {
                try
                {
                    SelectionDragged(this, e);
                }
                catch (Exception ex)
                {
                    TraceUtil.TraceExceptionCatched(ex);
                    if (!ExceptionManager.RaiseExceptionCatched(this, ex))
                    {
                        throw;
                    }

                    e.Cancel = true;
                }
            }
        }
        
        #endregion
        #region ResizeCellsUI
        /// <summary>
        /// Occurs when the user is resizing a selected range of columns.
        /// </summary>
        /// <remarks>
        ///  See <see cref="GridResizingColumnsEventArgs"/> for further discussion.
        /// </remarks>
        [Description("Occurs when the user is resizing a selected range of columns."),
        Category("Behavior")]
        public event GridResizingColumnsEventHandler ResizingColumns;

        /// <summary>
        /// Occurs when the user is resizing a selected range of rows.
        /// </summary>
        /// <remarks>
        ///  See <see cref="GridResizingRowsEventArgs"/> for further discussion.
        /// </remarks>
        [Description("Occurs when the user is resizing a selected range of rows."),
        Category("Behavior")]
        public event GridResizingRowsEventHandler ResizingRows;

        /// <summary>
        /// Initiates call to <see cref="OnResizingColumns"/>.
        /// </summary>
        /// <param name="e">The event arguments.</param>
        public void RaiseResizingColumns(GridResizingColumnsEventArgs e)
        {
            try
            {
                OnResizingColumns(e);
            }
            catch (Exception ex)
            {
                TraceUtil.TraceExceptionCatched(ex);
                if (!ExceptionManager.RaiseExceptionCatched(this, ex))
                {
                    throw;
                }

                e.Cancel = true;
            }
        }

        /// <summary>
        /// Initiates call to <see cref="OnResizingRows"/>.
        /// </summary>
        /// <param name="e">The event arguments.</param>
        public void RaiseResizingRows(GridResizingRowsEventArgs e)
        {
            try
            {
                OnResizingRows(e);
            }
            catch (Exception ex)
            {
                TraceUtil.TraceExceptionCatched(ex);
                if (!ExceptionManager.RaiseExceptionCatched(this, ex))
                {
                    throw;
                }

                e.Cancel = true;
            }
        }

        /// <summary>
        /// Raises the <see cref="GridControlBase.ResizingColumns" /> event.
        /// </summary>
        /// <param name="e">An <see cref="GridResizingColumnsEventArgs" /> that contains the event data.</param>
        protected virtual void OnResizingColumns(GridResizingColumnsEventArgs e)
        {
            if (AllowProportionalColumnSizing)
            {
                if (e.Reason == Syncfusion.Windows.Forms.Grid.GridResizeCellsReason.MouseUp)
                {
                    if (comparingClientSize(e.Columns.Left, e.Width, this.Model.ColWidths[e.Columns.Left]))
                        e.Cancel = true;
                }
                else if (e.Reason == Syncfusion.Windows.Forms.Grid.GridResizeCellsReason.DoubleClick)
                {
                    this.Model.ColWidths.ResizeToFit(GridRangeInfo.Col(e.Columns.Left));
                    int size = this.Model.ColWidths[e.Columns.Left];
                    e.Cancel = true; //we handled it...
                    if (comparingClientSize(e.Columns.Left, size, this.Model.ColWidths[e.Columns.Left]))
                        e.Cancel = true;
                }
                this.SetColumnWidths();
            }
            else
            {
                if (e.Reason == GridResizeCellsReason.DoubleClick)
                {
                    if (!this.UnHideColsOnDblClick && (this.Model.HideCols[e.Columns.Left + 1] || this.Model.ColWidths[e.Columns.Left + 1] == 0))
                    {
                        e.Cancel = true;
                        return;
                    }
                }
            }
            if (this.eventsTarget != null)
            {
                this.eventsTarget.OnResizingColumns(e);
            }
#if DEBUG

            if (Switches.GridControlBaseEvents.TraceVerbose)
            {
                TraceUtil.TraceCurrentMethodInfo(PaneDesc, e);
            }
#else

            ;
#endif

            if (ResizingColumns != null)
            {
                ResizingColumns(this, e);
            }
        }
        /// <override/>
        protected override void OnClientSizeChanged(System.EventArgs e)
        {
            if (this.AllowProportionalColumnSizing)
                this.SetColumnWidths();
            base.OnClientSizeChanged(e);
        }
        /// <summary>
        /// compare the grid client size to change the column width with new column width allow resizing to fit.
        /// </summary>
        /// <param name="columnIndex">Column Index </param>
        /// <param name="newColWidth"> new column with</param>
        /// <param name="oldColWidth">column width before change the size of column</param>
        /// <returns>returns true value.</returns>
        private bool comparingClientSize(int columnIndex, int newColWidth, int oldColWidth)
        {
            int clientWidth = this.ClientSize.Width;
            int vColCount = this.Model.ColCount;
            int frozenCount = this.InternalGetFrozenCols();
            int frozen = this.Model.ColWidths.GetTotal(0, frozenCount);
            int resizedRightColWidth = 0;
            int rightColWidth = 0;
            int resizedLeftColWidth = 0;
            int leftColWidth = 0;
            int rightColCount = 0;
            int rightResizedColCount = 0;
            int leftColCount = 0;
            for (int i = columnIndex + 1  ; i <= vColCount; i++)
            {
                if (sizedColumns.ContainsKey(i))
                {
                    resizedRightColWidth += sizedColumns[i];
                    rightResizedColCount++;
                }
                else
                    rightColWidth += this.model.ColWidths[i];
                rightColCount++;
            }

            for (int i = 1; i < columnIndex; i++)
            {
                if (sizedColumns.ContainsKey(i))
                {
                    resizedLeftColWidth += sizedColumns[i];
                }
                else
                    leftColWidth += this.model.ColWidths[i];
                leftColCount++;
            }
            int tot = clientWidth - leftColWidth - resizedLeftColWidth - newColWidth - frozen - resizedRightColWidth;
            if (tot > 0 && rightColCount > 0)
            {
                if (sizedColumns.ContainsKey(columnIndex))
                {
                    sizedColumns.Remove(columnIndex);
                }
                sizedColumns.Add(columnIndex, newColWidth);

                int dx = tot / (rightColCount - rightResizedColCount);

                for (int i = columnIndex; i <= vColCount; i++)
                {
                    if (!sizedColumns.ContainsKey(i))
                    {
                        this.model.ColWidths[i] = dx;
                    }
                    else
                        this.model.ColWidths[i] = sizedColumns[i];
                }
                return false;
            }
            else
                return true;
        }
        List<int> hiddencol = new List<int>();
        /// <summary>
        /// set the column width when AllowProportionalColumnSizing  is enable
        /// </summary>
        public void SetColumnWidths()
        {
            int width = this.ClientSize.Width;
            int count = this.Model.ColCount;
            if (count > 0 && count > sizedColumns.Count)
            {
                int rowin = this.InternalGetHeaderCols(); 
                int frozenCount = this.InternalGetFrozenCols();
                int frozen = this.Model.ColWidths.GetTotal(0, frozenCount);
                int fixedSize = 0;
                int hiddenColCount = this.Model.ColHiddenEntries.Count;
                GridColHiddenCollection hiddenColumnCollection = this.Model.ColHiddenEntries;
                foreach (GridColHidden hiddenCol in hiddenColumnCollection)
                {
                    if (!hiddencol.Contains(hiddenCol.ColIndex))
                        hiddencol.Add(hiddenCol.ColIndex);
                }
                foreach (int col in sizedColumns.Keys)
                {
                    fixedSize += sizedColumns[col];
                }
                int dx = (width - frozen - fixedSize) / (count - sizedColumns.Count - this.Model.ColHiddenEntries.Count);
                int addedWidth = 0; 
                for (int i = 1; i < count ; ++i)
                {
                    if (!sizedColumns.ContainsKey(i))
                    {
                        if (hiddencol.Contains(i))
                        {
                            this.model.ColWidths[i] = 0;
                        }
                        else
                        {
                            this.model.ColWidths[i] = dx;
                            addedWidth += dx;
                        }
                    }
                }
                this.model.ColWidths[count] = width - frozen - fixedSize - addedWidth; //add all the roundoff pixels to the last column
            }
        }
        /// <summary>
        /// Raises the <see cref="GridControlBase.ResizingRows" /> event.
        /// </summary>
        /// <param name="e">An <see cref="GridResizingRowsEventArgs" /> that contains the event data.</param>
        protected virtual void OnResizingRows(GridResizingRowsEventArgs e)
        {
            if (this.eventsTarget != null)
            {
                this.eventsTarget.OnResizingRows(e);
            }
#if DEBUG

            if (Switches.GridControlBaseEvents.TraceVerbose)
            {
                TraceUtil.TraceCurrentMethodInfo(PaneDesc, e);
            }
#else

            ;
#endif

            if (ResizingRows != null)
            {
                ResizingRows(this, e);
            }
        }
        #endregion

        #region OleDataSource

        /// <summary>
        /// Occurs when the user hovers the mouse over the edge of a selected range.
        /// In your event handler, you can determine if the selected range can serve as OLE Data Source.
        /// </summary>
        /// <remarks>
        /// You can disallow the specified range to be used as OLE Data Source when
        /// you assign true to <see cref="CancelEventArgs.Cancel"/>.
        /// </remarks>
        /// Also see <seealso cref="GridQueryCanOleDragRangeEventArgs"/>
        /// Also see <seealso cref="GridControlBase.QueryCanOleDragRange"/>
        [Category("Drag Drop")]
        [Description("Occurs when the user hovers the mouse over the edge of a selected range.")]
        public event GridQueryCanOleDragRangeEventHandler QueryCanOleDragRange;

        /// <summary>
        /// Initiates call to <see cref="OnQueryCanOleDragRange"/>.
        /// </summary>
        /// <param name="e">The event arguments.</param>
        public void RaiseQueryCanOleDragRange(GridQueryCanOleDragRangeEventArgs e)
        {
            OnQueryCanOleDragRange(e);
        }

        /// <summary>
        /// Raises the <see cref="GridControlBase.QueryCanOleDragRange" /> event.
        /// </summary>
        /// <param name="e">An <see cref="GridQueryCanOleDragRangeEventArgs" /> that contains the event data.</param>
        protected virtual void OnQueryCanOleDragRange(GridQueryCanOleDragRangeEventArgs e)
        {
            if (this.eventsTarget != null)
            {
                this.eventsTarget.OnQueryCanOleDragRange(e);
            }
#if DEBUG

            if (Switches.GridControlBaseEvents.TraceVerbose)
            {
                TraceUtil.TraceCurrentMethodInfo(PaneDesc, e);
            }
#else

            ;
#endif

            if (QueryCanOleDragRange != null)
            {
                try
                {
                    QueryCanOleDragRange(this, e);
                }
                catch (Exception ex)
                {
                    TraceUtil.TraceExceptionCatched(ex);
                    if (!ExceptionManager.RaiseExceptionCatched(this, ex))
                    {
                        throw;
                    }

                    e.Cancel = true;
                }
            }
        }
        #endregion

        #region OleDropTarget

        IGridOleDragDropEventsTarget oleDragDropEventsTarget;

        /// <summary>
        /// Gets or sets the OleDragDropEventsTarget. Redirects events defined in <see cref="IGridOleDragDropEventsTarget"/> to the specified object.
        /// Each event will first be called on <see cref="IGridOleDragDropEventsTarget"/> before the actual
        /// event handler in this control is called.
        /// </summary>
        [DesignerSerializationVisibilityAttribute(DesignerSerializationVisibility.Hidden)]
        [Browsable(false)]
        public IGridOleDragDropEventsTarget OleDragDropEventsTarget
        {
            get
            {
                return oleDragDropEventsTarget;
            }

            set
            {
                oleDragDropEventsTarget = value;
            }
        }

        /// <override/>
        protected override void OnDragDrop(DragEventArgs e)
        {
            if (oleDragDropEventsTarget != null)
            {
                oleDragDropEventsTarget.OnDragDrop(e);
            }

            base.OnDragDrop(e);
        }

        /// <override/>
        protected override void OnDragEnter(DragEventArgs e)
        {
            if (oleDragDropEventsTarget != null)
            {
                oleDragDropEventsTarget.OnDragEnter(e);
            }

            base.OnDragEnter(e);
        }

        /// <override/>
        protected override void OnDragLeave(EventArgs e)
        {
            if (oleDragDropEventsTarget != null)
            {
                oleDragDropEventsTarget.OnDragLeave(e);
            }

            base.OnDragLeave(e);
        }

        /// <override/>
        protected override void OnDragOver(DragEventArgs e)
        {
            if (oleDragDropEventsTarget != null)
            {
                oleDragDropEventsTarget.OnDragOver(e);
            }

            base.OnDragOver(e);
        }

        #endregion
        #region PaintSelectCells
        internal IGridDrawSelectionFrame excelLikeFrameSelections = null;

        /// <internalonly/>
        /// <summary>Gets or sets ExcelLikeFrameSelections. Used internally.</summary>
        [Syncfusion.Documentation.DocumentationExclude()]
        [Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public IGridDrawSelectionFrame ExcelLikeFrameSelections
        {
            get
            {
                return excelLikeFrameSelections;
            }

            set
            {
                excelLikeFrameSelections = value;
            }
        }

        /// <summary>
        /// Gets selected ranges in the grid. Allows you to add and remove selections, determines
        /// selection state of a specific cell and more.
        /// </summary>
        [Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public GridModelSelections Selections
        {
            get
            {
                return Model.Selections;
            }
        }

        #endregion
        #region ExcelLikeSelectionFrame

        /// <internalonly/>
        /// <summary>Used internally.</summary>
        [Syncfusion.Documentation.DocumentationExclude()]
        public void NotifySelectionFrameChanging(Graphics g)
        {
            OnSelectionFrameChanging(new GraphicsEventArgs(g));
        }

        /// <internalonly/>
        /// <summary>Used internally.</summary>
        [Syncfusion.Documentation.DocumentationExclude()]
        public void NotifySelectionFrameChanged(Graphics g)
        {
            OnSelectionFrameChanged(new GraphicsEventArgs(g));
        }

        /// <summary>
        /// Occurs before the Excel-like selection frame is changing.
        /// </summary>
        /// <remarks>
        /// Before internal data are changed. Grid will hide selection frame.
        /// </remarks>
        [Browsable(false)]
        public event GraphicsEventHandler SelectionFrameChanging;

        /// <summary>
        /// Occurs after the Excel-like selection frame has changed.
        /// </summary>
        /// <remarks>
        /// After internal data were changed. Grid will show selection frame.
        /// </remarks>
        [Browsable(false)]
        public event GraphicsEventHandler SelectionFrameChanged;

        /// <summary>
        /// Raises the <see cref="GridControlBase.SelectionFrameChanging"/> event.
        /// </summary>
        /// <param name="e">A <see cref="GraphicsEventArgs" /> that contains the event data.</param>
        protected virtual void OnSelectionFrameChanging(GraphicsEventArgs e)
        {
            if (this.eventsTarget != null)
            {
                this.eventsTarget.OnSelectionFrameChanging(e);
            }
#if DEBUG

            if (Switches.GridControlBaseEvents.TraceVerbose)
            {
                TraceUtil.TraceCurrentMethodInfo(PaneDesc);
            }
#else

            ;
#endif

            if (SelectionFrameChanging != null)
            {
                SelectionFrameChanging(this, e);
            }
        }

        /// <summary>
        /// Raises the <see cref="GridControlBase.SelectionFrameChanged"/> event.
        /// </summary>
        /// <param name="e">A <see cref="GraphicsEventArgs" /> that contains the event data.</param>
        protected virtual void OnSelectionFrameChanged(GraphicsEventArgs e)
        {
            if (this.eventsTarget != null)
            {
                this.eventsTarget.OnSelectionFrameChanged(e);
            }
#if DEBUG

            if (Switches.GridControlBaseEvents.TraceVerbose)
            {
                TraceUtil.TraceCurrentMethodInfo(PaneDesc);
            }
#else

            ;
#endif

            if (SelectionFrameChanged != null)
            {
                SelectionFrameChanged(this, e);
            }
        }

        #endregion
        #region DrawGrid

        /// <summary>
        /// Updates scrollbars with current scroll position and scroll range. Hides
        /// or enables scrollbars as specified with <see cref="HScrollBehavior"/>
        /// and <see cref="VScrollBehavior"/>.
        /// </summary>
        /// <remarks>
        /// <see cref="UpdateScrollBars"/> might also scroll your grid if <see cref="GridScrollbarMode.AutoScroll"/>
        /// has been specified for the <see cref="HScrollBehavior"/>
        /// or <see cref="VScrollBehavior"/>.
        /// </remarks>
        public override void UpdateScrollBars()
        {
            ScrollGrid.UpdateScrollbars();
            base.UpdateScrollBars();

            //// Call it twice - base.UpdateScrollBars could have replace inner scrollbars,
            //// or hiding/showing vertical or horizontal scrollbar could affect necessity of
            //// other scrollbar.
            ScrollGrid.UpdateScrollbars();
            base.UpdateScrollBars();
        }

        private GridPaint m_gridPaint;
        internal bool m_bDrawBannerCell = false;
        internal bool m_bForceDrawBackground = false;
        internal bool m_bDrawCoveredCell = false;

        /// <internalonly/>
        /// <summary>
        /// Used internally.
        /// </summary>
        [Syncfusion.Documentation.DocumentationExclude()]
        protected internal bool m_bInvertRect = false;

        /// <override/>
        protected override void OnLayout(LayoutEventArgs le)
        {
            if (this.eventsTarget != null)
            {
                this.eventsTarget.OnLayout(le);
            }

            if (this.IsSplitterPaneClosing)
            {
                return;
            }

            if (!this.ClientRectangle.IsEmpty)
            {
#if DEBUG
                if (Switches.Development.TraceVerbose)
                {
                    TraceUtil.TraceCurrentMethodInfo(PaneDesc, "C", Created, "U", Updating, "V", Visible, Size, PaneDesc);
                }
#else
                ;
#endif

                InitSplitter();
                ViewLayout.Reset();

                // Check if grid is pane in a dynamic splitter control. // || this.CurrentCell.m_bIgnoreFocus)
                if (le.AffectedControl != this)
                {
                    return;
                }
#if DEBUG

                if (Switches.GridControlBaseEvents.TraceVerbose)
                {
                    TraceUtil.TraceCurrentMethodInfo(PaneDesc, le.AffectedProperty, Bounds);
                }
#else

                ;
#endif
            }

            UpdateScrollBars();

            base.OnLayout(le);
        }

        /// <override/>
        protected override void OnSizeChanged(EventArgs e)
        {
            if (this.eventsTarget != null)
            {
                this.eventsTarget.OnSizeChanged(e);
            }

            if (this.IsSplitterPaneClosing)
            {
                return;
            }

            if (!ClientRectangle.IsEmpty)
            {
#if DEBUG
                if (Switches.Development.TraceVerbose)
                {
                    TraceUtil.TraceCurrentMethodInfo(PaneDesc, "C", Created, "U", Updating, "V", Visible, Size, PaneDesc);
                }
#else
                ;
#endif

                if (!Model.Initializing)
                {
                    UpdateScrollBars();
                }

                if (!Model.Options.SmoothControlResize || this.IsRightToLeft())
                {
                    Invalidate();
                }
#if DEBUG
                if (Switches.GridControlBaseEvents.TraceVerbose)
                {
                    TraceUtil.TraceCurrentMethodInfo(PaneDesc, Size);
                }
#else
                ;
#endif
            }

            base.OnSizeChanged(e);
        }

        /// <internalonly/>
        [Syncfusion.Documentation.DocumentationExclude()]
        protected bool firstPaint = true;

        internal bool inOnPaint = false;

        /// <override/>
        protected override void OnPaint(PaintEventArgs pe)
        {
            //// Special handling when UpdateWithCustomPaint was called.
            if (this.customPaintDelegate != null)
            {
                pe = new PaintEventArgs(pe.Graphics, customPaintRectangle);
                customPaintDelegate(this, pe);
                return;
            }

            //// With Windows Vista when changing TextBox.MultiLine or other properties
            //// in OnDraw routine it will immeditately force a repaint of the grid area.
            if (inOnPaint && !this.Model.Properties.ForceImmediateRepaint)
            {
                return;
            }

            if (this.eventsTarget != null)
            {
                this.eventsTarget.OnPaint(pe);
            }
#if DEBUG

            if (Switches.Development.TraceVerbose)
            {
                TraceUtil.TraceCurrentMethodInfo(PaneDesc, "C", Created, "U", Updating, "V", Visible, Size, PaneDesc);
            }
#else

            ;
#endif
#if DEBUG
            if (Switches.GridPaint.TraceVerbose)
            {
                TraceUtil.TraceCurrentMethodInfo(this.PaneDesc, this.Bounds, ClientRectangle);
            }
#else
            ;
#endif

            if (Updating)
            {
                throw new InvalidProgramException("Grid is in updating mode");
            }

            if (DesignMode)
            {
                Model.ResetVolatileData();
                ViewLayout.Reset();
            }

            inOnPaint = true;
            bool savedIgnoreUICues = IgnoreUICues;
            IgnoreUICues = true;

            if (firstPaint)
            {
                UpdateScrollBars();
                firstPaint = false;
                EvaluateVisibleFloatingCells(ViewLayout.VisibleCellsRange);
                EvaluateVisibleMergeCells(ViewLayout.VisibleCellsRange);
            }

            Graphics g = pe.Graphics;
            FixRenderOrigin(g);
            Model.DoPrepareGraphics(g);
            SuspendLayout();

            try
            {
                //// Refresh last row and column.
                Rectangle rect = GridBounds;

                //// Drawing rectangle.
                Rectangle rcClip = pe.ClipRectangle;

#if DEBUG
                Trace.WriteLineIf(Switches.GridPaint.TraceVerbose, "rcClip = " + rcClip.ToString());
#endif

                if (rcClip.IsEmpty)
                {
                    return;
                }

                //// Don't draw to the screen while the grid is printing.
                if (this.PrintingMode)
                {
                    //// OnGridPrint will check m_bPrintPaintMsg and redraw the
                    //// whole grid later.
                    PrintInfo.m_bPrintPaintMsg = true;
                    return;
                }

                m_gridPaint.DrawGrid(pe.Graphics, HasGridBounds, rcClip);
            }
            catch (Exception ex)
            {
                TraceUtil.TraceExceptionCatched(ex);
                if (!ExceptionManager.RaiseExceptionCatched(this, ex))
                {
                    throw;
                }
            }
            finally
            {
#if DEBUG
                if (Switches.GridControlBaseEvents.TraceVerbose)
                {
                    TraceUtil.TraceCurrentMethodInfo(PaneDesc, pe.ClipRectangle, ClientRectangle);
                }
#else
                ;
#endif

                base.OnPaint(pe);
                ResumeLayout(false);
                IgnoreUICues = savedIgnoreUICues;
                inOnPaint = false;
            }
        }

        /// <summary>
        /// Raises the <see cref="GridControlBase.PrepareViewStyleInfo"/> event which allows changing view-specific settings for
        /// the cells style object before the cell is displayed in the grid (except cell type and
        /// base style). These changes will not be cached and saved in the grid.
        /// </summary>
        /// <param name="rowIndex">The row index.</param>
        /// <param name="colIndex">The column index.</param>
        /// <param name="style">The <see cref="GridStyleInfo"/> object that holds cell information.</param>
        public void RaisePrepareViewStyleInfo(int rowIndex, int colIndex, GridStyleInfo style)
        {
            GridPrepareViewStyleInfoEventArgs e = new GridPrepareViewStyleInfoEventArgs(rowIndex, colIndex, style);
            OnPrepareViewStyleInfo(e);
            if (!e.Cancel)
            {
                GridCellRendererBase cellRenderer = CellRenderers[style.CellType];
                if (cellRenderer != null)
                {
                    cellRenderer.OnPrepareViewStyleInfo(e);
                }
            }
        }

        /// <overload>
        /// Queries cell information that includes custom formatting based on
        /// the current view state. The custom formatting is determined by raising
        ///  <see cref="GridControlBase.PrepareViewStyleInfo"/> event.
        /// </overload>
        /// <summary>
        /// Queries cell information that includes custom formatting based on
        /// the current view state. The custom formatting is determined by raising
        ///  the <see cref="GridControlBase.PrepareViewStyleInfo"/> event.
        /// </summary>
        /// <param name="rowIndex">The row index.</param>
        /// <param name="colIndex">The column index.</param>
        /// <returns>The <see cref="GridStyleInfo"/> object that holds cell information.</returns>
        public GridStyleInfo GetViewStyleInfo(int rowIndex, int colIndex)
        {
            return GetViewStyleInfo(rowIndex, colIndex, false);
        }

        /// <summary>
        /// By default, the grid will make a copy of any style object before painting and call the
        /// PrepareViewStyleInfo event. Changes made to the style object will then be discarded after
        /// the painting. Same with DrawCell and other events. Having a copy of the style allows you to change
        /// the style object just for drawing purposes. If you do not have any need for this and want to
        /// increase scrolling performance of the grid, try setting this property false. But be aware this is
        /// only experimental for now ...
        /// </summary>
        public bool SupportsPrepareViewStyleInfo = true;

        /// <summary>
        /// Just experimental, don't use this .... Optionally don't use GetViewStyleInfo if SupportsPrepareViewStyleInfo is not set.
        /// </summary>
        /// <param name="rowIndex">Index of the row.</param>
        /// <param name="colIndex">Index of the col.</param>
        /// <param name="forceQueryCellInfo">if set to <c>true</c> [force query cell info].</param>
        /// <returns>returns GridStyleInfo </returns>
        internal GridStyleInfo GetPaintStyleInfo(int rowIndex, int colIndex, bool forceQueryCellInfo)
        {
            GridStyleInfo style;
            if (SupportsPrepareViewStyleInfo)
            {
                style = GetViewStyleInfo(rowIndex, colIndex, forceQueryCellInfo);
            }
            else
            {
                style = Model[rowIndex, colIndex];
                style.Locked = true;
            }

            return style;
        }

        internal void DisposePaintStyle(GridStyleInfo style)
        {
            if (SupportsPrepareViewStyleInfo)
            {
                style.Dispose();
            }
            else
            {
                style.Locked = false;
            }
        }

        /// <override/>
        protected override void OnEnsurePaintCodeJitted()
        {
            Bitmap bmp = new Bitmap(2, 2);
            Graphics g = Graphics.FromImage(bmp);
            this.DrawGrid(g, new Rectangle(Point.Empty, bmp.Size), false, false);
            g.Dispose();
            bmp.Dispose();
        }

        /// <summary>
        /// Queries cell information that includes custom formatting based on
        /// the current view state. The custom formatting is determined by raising
        ///  <see cref="GridControlBase.PrepareViewStyleInfo"/> event.
        /// </summary>
        /// <param name="rowIndex">The row index.</param>
        /// <param name="colIndex">The column index.</param>
        /// <param name="forceQueryCellInfo">For the QueryCellInfo to be called and not cache style objects.</param>
        /// <returns>The <see cref="GridStyleInfo"/> object that holds cell information.</returns>
        public virtual GridStyleInfo GetViewStyleInfo(int rowIndex, int colIndex, bool forceQueryCellInfo)
        {
            GridStyleInfo modelStyle;
#if DEBUG
            /*int r = rowIndex;
            int c = colIndex;
            CurrentCell.AdjustRowColIfCoveredCell(ref rowIndex, ref colIndex);
            if (rowIndex != r || colIndex != c)
            {
                r = rowIndex;
                c = colIndex;
                Debugger.Break();
                CurrentCell.AdjustRowColIfCoveredCell(ref rowIndex, ref colIndex);
                throw new InvalidOperationException("You queried the cell info of a cell that is covered. CurrentCell out of sync after collapsing maybe?");
            }  */
#endif
            GridViewStyleInfoIdentity viewStyleIdentity = new GridViewStyleInfoIdentity(this, Model.volatileData, rowIndex, colIndex/*, true*/);
            if (forceQueryCellInfo && Model.VolatileData is GridVolatileData)
            {
                modelStyle = new GridStyleInfo(viewStyleIdentity, new GridStyleInfoStore(), AllowViewStyleCacheBaseStyleValues);
                Model.GetCellInfo(rowIndex, colIndex, modelStyle);
                viewStyleIdentity.InnerIdentity = modelStyle.Identity.InnerIdentity;
            }
            else
            {
                modelStyle = Model[rowIndex, colIndex];
                viewStyleIdentity.InnerIdentity = modelStyle.Identity.InnerIdentity;
                modelStyle = new GridStyleInfo(
                    viewStyleIdentity,
                    (GridStyleInfoStore)modelStyle.Store.Clone(), 
                    AllowViewStyleCacheBaseStyleValues);
            }

            RaisePrepareViewStyleInfo(rowIndex, colIndex, modelStyle);
            return modelStyle;
        }

        /// <summary>
        /// This event is raised to allow custom formatting of
        /// a cell by changing its style object just before it is drawn.
        /// </summary>
        /// <remarks>
        /// This allows formatting based on the current view state, e.g. current cell context,
        /// focused control etc.<para/>
        /// See <see cref="GridPrepareViewStyleInfoEventArgs"/> for further discussion.
        /// </remarks>
        [Description("Occurs for every cell that is about be redrawn."),
        Category("Behavior")]
        public event GridPrepareViewStyleInfoEventHandler PrepareViewStyleInfo;

        /// <summary>
        /// Raises the <see cref="GridControlBase.PrepareViewStyleInfo"/> event.
        /// </summary>
        /// <param name="e">A <see cref="GridPrepareViewStyleInfoEventArgs" /> that contains the event data.</param>
        protected virtual void OnPrepareViewStyleInfo(GridPrepareViewStyleInfoEventArgs e)
        {
            if (this.eventsTarget != null)
            {
                this.eventsTarget.OnPrepareViewStyleInfo(e);
            }
#if DEBUG

            if (Switches.GridControlBaseEvents.TraceVerbose)
            {
                TraceUtil.TraceCurrentMethodInfo(PaneDesc, e);
            }
#else

            ;
#endif
            int Frozen_RowNumber = this.Model.Rows.FrozenCount;
            int Frozen_ColNumber=this.Model.Cols.FrozenCount;
                       
            if (!PersistAppearanceSettings && this.Model.Options.HighlightFrozenLine)
            {
                if (e.RowIndex == Frozen_RowNumber && this.Model.Properties.DisplayHorzLines && Frozen_RowNumber > 0 && e.Style.Borders.Bottom.Style != GridBorderStyle.None)
                    e.Style.Borders.Bottom = new GridBorder(e.Style.Borders.Bottom.Style, this.Model.Properties.FixedLinesColor, GridBorderWeight.ExtraThin);

                if (e.ColIndex == Frozen_ColNumber && this.Model.Properties.DisplayVertLines && Frozen_ColNumber > 0 && e.Style.Borders.Right.Style != GridBorderStyle.None)
                    e.Style.Borders.Right = new GridBorder(e.Style.Borders.Right.Style, this.Model.Properties.FixedLinesColor, GridBorderWeight.ExtraThin);
            }

            if (PrepareViewStyleInfo != null)
            {
                PrepareViewStyleInfo(this, e);
            }

            if (this.CurrentCell != null && e.RowIndex == this.CurrentCell.RowIndex && e.ColIndex == this.CurrentCell.ColIndex && this.CurrentCell.ErrorMessage != string.Empty)
            {
                e.Style.CellTipText = this.CurrentCell.ErrorMessage;              
            }
        }

        /// <summary>
        /// Occurs for every cell before the grid draws the specified cell.
        /// </summary>
        /// <remarks>
        /// See <see cref="GridDrawCellEventArgs"/> for more detailed discussion.
        /// </remarks>
        /// <seealso cref="GridControlBase.OnDrawItem"/>
        /// <seealso cref="GridCellRendererBase.OnDraw"/>
        /// <seealso cref="GridCellRendererBase.Draw"/>
        [Description("Occurs for every cell that is being drawn."),
        Category("Behavior")]
        public event GridDrawCellEventHandler DrawCell;

        static GridIconPaint iconPainter;
        /// <summary>
        /// Raises the <see cref="GridControlBase.DrawCell"/> event.
        /// </summary>
        /// <param name="e">A <see cref="GridDrawCellEventArgs" /> that contains the event data.</param>
        protected virtual void OnDrawCell(GridDrawCellEventArgs e)
        {
            if (this.eventsTarget != null)
            {
                this.eventsTarget.OnDrawCell(e);
            }
#if DEBUG

            if (Switches.GridControlBaseEvents.TraceVerbose)
            {
                TraceUtil.TraceCurrentMethodInfo(PaneDesc, e);
            }
#else

            ;
#endif
            if (e.Renderer.CurrentCell.ErrorMessage != string.Empty)
            {
                if (iconPainter == null)
                {
                    iconPainter = GridIconPaint.GridPainter;
                }
                int textMargin = 15;
                string bitmapName = "SFERROR.BMP"; 
                if (this.CurrentCell.HasCurrentCellAt(e.RowIndex, e.ColIndex) && e.Renderer.CurrentCell.ShowErrorIcon)
                {
                    e.Cancel = true;
                    e.Style.TextMargins.Right = textMargin;                    
                    e.Renderer.Draw(e.Graphics, e.Bounds, CurrentCell.RowIndex, CurrentCell.ColIndex, e.Style);
                    Brush br = new SolidBrush(Color.FromArgb(64, Color.Red));
                    e.Graphics.FillRectangle(br, e.Bounds);
                    br.Dispose();
                    Rectangle iconBounds = Rectangle.FromLTRB(e.Bounds.Right - textMargin, e.Bounds.Top, e.Bounds.Right, e.Bounds.Bottom);
                    iconBounds.Offset(-2, 0);                    
                    iconPainter.PaintIcon(e.Graphics, iconBounds, Point.Empty, bitmapName, Color.Black);
                }
            }
            if (DrawCell != null)
            {
                DrawCell(this, e);
            }
        }
        
        /// <summary>
        /// Occurs for every cell before the grid draws the display text for the specified cell.
        /// </summary>
        /// <remarks>
        /// See <see cref="GridDrawCellDisplayTextEventArgs"/> for more detailed discussion.
        /// </remarks>
        /// <seealso cref="GridControlBase.OnDrawItem"/>
        /// <seealso cref="GridCellRendererBase.OnDraw"/>
        /// <seealso cref="GridCellRendererBase.Draw"/>
        [Description("Occurs for every cell that is being drawn."),
        Category("Behavior")]
        public event GridDrawCellDisplayTextEventHandler DrawCellDisplayText;

        /// <summary>
        /// Raises the <see cref="GridControlBase.DrawCellDisplayText"/> event.
        /// </summary>
        /// <param name="e">A <see cref="GridDrawCellDisplayTextEventArgs" /> that contains the event data.</param>
        protected virtual void OnDrawCellDisplayText(GridDrawCellDisplayTextEventArgs e)
        {
            int int_result;
            double double_result;
            decimal decimal_result;
            string text = e.DisplayText;
            bool isNumber = ((((e.Style.CellValueType == null)) && (int.TryParse(text, System.Globalization.NumberStyles.Any, null, out int_result)
                    || double.TryParse(text, System.Globalization.NumberStyles.Any, null, out double_result)
                    || decimal.TryParse(text, System.Globalization.NumberStyles.Any, null, out decimal_result))
                    && (!e.Style.HasHorizontalAlignment)) || ((e.Style.CellValueType == typeof(int) || e.Style.CellValueType == typeof(double) || e.Style.CellValueType == typeof(decimal)) && (!e.Style.HasHorizontalAlignment)));
            if (e.DisplayText.Contains(System.Globalization.CultureInfo.CurrentCulture.NumberFormat.PercentSymbol))
                text = e.DisplayText.Replace(System.Globalization.CultureInfo.CurrentCulture.NumberFormat.PercentSymbol, "");
            if (excelLikeAlignment)
            {
                if(isNumber)
                    e.Style.HorizontalAlignment = GridHorizontalAlignment.Right;
            }
            if (e.Style.AutoFit != AutoFitOptions.None)
            {
                float width = e.Graphics.MeasureString(e.DisplayText, e.Style.GdipFont).Width;
                if (width > e.TextRectangle.Width)
                {
                    string displayText = e.Style.AutoFitChar.ToString();
                    for (int i = 0; i < e.TextRectangle.Width; i++)
                        displayText += e.Style.AutoFitChar.ToString();
                    if (!isNumber && e.Style.CellValueType == typeof(int))
                    {
                        System.Globalization.NumberStyles style = System.Globalization.NumberStyles.Number | System.Globalization.NumberStyles.AllowCurrencySymbol;
                        Double value;
                        isNumber = Double.TryParse(text, style, null, out value);
                    }
                    if ((e.Style.AutoFit == AutoFitOptions.Both)
                        || (e.Style.AutoFit == AutoFitOptions.Numeric && isNumber)
                        || (e.Style.AutoFit == AutoFitOptions.Alphabet && !isNumber))
                        e.DisplayText = displayText;
                }
            }

            if ((this.RightToLeft == System.Windows.Forms.RightToLeft.Yes) && EnableRTLMark)
            {
                e.DisplayText = (char)unicodeRLM + e.DisplayText + (char)unicodeRLM;
            }
            if (this.eventsTarget != null)
            {
                this.eventsTarget.OnDrawCellDisplayText(e);
            }
#if DEBUG

            if (Switches.GridControlBaseEvents.TraceVerbose)
            {
                TraceUtil.TraceCurrentMethodInfo(PaneDesc, e);
            }
#else

            ;
#endif

            if (DrawCellDisplayText != null)
            {
                DrawCellDisplayText(this, e);
            }
        }

        /// <summary>
        /// Occurs for every cell before the grid draws the background of a specified cell.
        /// </summary>
        /// <remarks>
        /// See <see cref="GridDrawCellBackgroundEventArgs"/> for more detailed discussion.
        /// </remarks>
        /// <seealso cref="GridControlBase.OnDrawItem"/>
        /// <seealso cref="GridCellRendererBase.OnDraw"/>
        [Description("Occurs for every cell that is being drawn."),
        Category("Behavior")]
        public event GridDrawCellBackgroundEventHandler DrawCellBackground;

        /// <summary>
        /// Raises the <see cref="GridControlBase.DrawCellBackground"/> event.
        /// </summary>
        /// <param name="e">A <see cref="GridDrawCellBackgroundEventArgs" /> that contains the event data.</param>
        protected virtual void OnDrawCellBackground(GridDrawCellBackgroundEventArgs e)
        {
            if (this.eventsTarget != null)
            {
                this.eventsTarget.OnDrawCellBackground(e);
            }
#if DEBUG

            if (Switches.GridControlBaseEvents.TraceVerbose)
            {
                TraceUtil.TraceCurrentMethodInfo(PaneDesc, e);
            }
#else

            ;
#endif
            if (DrawCellBackground != null)
            {
                DrawCellBackground(this, e);
            }
        }

        /// <summary>
        /// Initiates call to <see cref="OnDrawCellBackground"/>.
        /// </summary>
        /// <param name="e">The event arguments</param>
        public void RaiseDrawCellBackground(GridDrawCellBackgroundEventArgs e)
        {
            OnDrawCellBackground(e);
        }

        /// <summary>
        /// Initiates call to <see cref="OnDrawCellDisplayText"/>.
        /// </summary>
        /// <param name="e">The event arguments.</param>
        public void RaiseDrawCellDisplayText(GridDrawCellDisplayTextEventArgs e)
        {
            OnDrawCellDisplayText(e);
        }

        /// <summary>
        /// Occurs for every cell before the grid draws the frame (sunken or raised) of a specified cell
        /// and after the cell's background was drawn.
        /// </summary>
        /// <seealso cref="GridControlBase.OnDrawItem"/>
        /// <seealso cref="GridCellRendererBase.OnDraw"/>
        /// <remarks>
        /// Set e.Cancel = True if you want to customize the drawing of the frame
        /// and do not want the grid with its default drawing for the cell's frame.
        /// <para/>
        /// If you want to draw custom borders around a cell, be sure to reserve space
        /// for the extra area occupied by your borders. See <see cref="GridStyleInfo.BorderMargins"/>.
        /// </remarks>
        [Description("Occurs for every cell that is being drawn before the grid draws the frame (sunken or raised)."),
        Category("Behavior")]
        public event GridDrawCellBackgroundEventHandler DrawCellFrameAppearance;

        /// <summary>
        /// Raises the <see cref="GridControlBase.DrawCellFrameAppearance"/> event.
        /// </summary>
        /// <param name="e">A <see cref="GridDrawCellBackgroundEventArgs" /> that contains the event data.</param>
        protected virtual void OnDrawCellFrameAppearance(GridDrawCellBackgroundEventArgs e)
        {
            if (this.eventsTarget != null)
            {
                this.eventsTarget.OnDrawCellFrameAppearance(e);
            }
#if DEBUG

            if (Switches.GridControlBaseEvents.TraceVerbose)
            {
                TraceUtil.TraceCurrentMethodInfo(PaneDesc, e);
            }
#else

            ;
#endif
            if (DrawCellFrameAppearance != null)
            {
                DrawCellFrameAppearance(this, e);
            }
        }

        /// <summary>
        /// Initiates call to <see cref="OnDrawCellFrameAppearance"/>.
        /// </summary>
        /// <param name="e">The event arguments</param>
        public void RaiseDrawCellFrameAppearance(GridDrawCellBackgroundEventArgs e)
        {
            OnDrawCellFrameAppearance(e);
        }

        /// <summary>
        /// Occurs for every button in every cell before the grid draws a cell button.
        /// </summary>
        /// <remarks>
        /// See <see cref="GridDrawCellButtonEventArgs"/> for more detailed discussion.
        /// See <see cref="GridDrawCellButtonBackgroundEventArgs"/> for an example.
        /// </remarks>
        /// <seealso cref="GridControlBase.OnDrawItem"/>
        /// <seealso cref="GridCellRendererBase.OnDraw"/>
        [Description("Occurs for every cell that is being drawn."),
        Category("Behavior")]
        public event GridDrawCellButtonEventHandler DrawCellButton;

        /// <summary>
        /// Raises the <see cref="GridControlBase.DrawCellButton"/> event.
        /// </summary>
        /// <param name="e">A <see cref="GridDrawCellButtonEventArgs" /> that contains the event data.</param>
        protected virtual void OnDrawCellButton(GridDrawCellButtonEventArgs e)
        {
            if (this.eventsTarget != null)
            {
                this.eventsTarget.OnDrawCellButton(e);
            }
#if DEBUG

            if (Switches.GridControlBaseEvents.TraceVerbose)
            {
                TraceUtil.TraceCurrentMethodInfo(PaneDesc, e);
            }
#else

            ;
#endif

            if (DrawCellButton != null)
            {
                DrawCellButton(this, e);
            }
        }

        /// <summary>
        /// Initiates call to <see cref="OnDrawCellButton"/>.
        /// </summary>
        /// <param name="e">The event arguments.</param>
        public void RaiseDrawCellButton(GridDrawCellButtonEventArgs e)
        {
            OnDrawCellButton(e);
        }

        /// <summary>
        /// Occurs for every button in every cell before the grid draws the background of a cell button.
        /// </summary>
        /// <remarks>
        /// See <see cref="GridDrawCellButtonBackgroundEventArgs"/> for more detailed discussion and also an example.
        /// </remarks>
        /// <seealso cref="GridControlBase.OnDrawItem"/>
        /// <seealso cref="GridCellRendererBase.OnDraw"/>
        [Description("Occurs for every cell that is being drawn."),
        Category("Behavior")]
        public event GridDrawCellButtonBackgroundEventHandler DrawCellButtonBackground;

        /// <summary>
        /// Raises the <see cref="GridControlBase.DrawCellButtonBackground"/> event.
        /// </summary>
        /// <param name="e">A <see cref="GridDrawCellButtonBackgroundEventArgs" /> that contains the event data.</param>
        protected virtual void OnDrawCellButtonBackground(GridDrawCellButtonBackgroundEventArgs e)
        {
            if (this.eventsTarget != null)
            {
                this.eventsTarget.OnDrawCellButtonBackground(e);
            }
#if DEBUG

            if (Switches.GridControlBaseEvents.TraceVerbose)
            {
                TraceUtil.TraceCurrentMethodInfo(PaneDesc, e);
            }
#else

            ;
#endif

            if (DrawCellButtonBackground != null)
            {
                DrawCellButtonBackground(this, e);
            }
        }

        /// <summary>
        /// Initiates call to <see cref="OnDrawCellButtonBackground"/>.
        /// </summary>
        /// <param name="e">The event arguments</param>
        public void RaiseDrawCellButtonBackground(GridDrawCellButtonBackgroundEventArgs e)
        {
            OnDrawCellButtonBackground(e);
        }

        /// <summary>
        /// Occurs for every cell after the grid has drawn the specified cell.
        /// </summary>
        /// <remarks>
        /// See <see cref="GridDrawCellEventArgs"/> for more detailed discussion.
        /// </remarks>
        /// <seealso cref="GridControlBase.OnDrawItem"/>
        /// <seealso cref="GridCellRendererBase.OnDraw"/>
        /// <seealso cref="GridCellRendererBase.Draw"/>
        [Description("Occurs for every cell that is being drawn."),
        Category("Behavior")]
        public event GridDrawCellEventHandler CellDrawn;

        private bool showDisabledGridAsGray = false;

        /// <summary>
        /// Gets or sets whether the grid is shown as gray if it is disabled. The default value is false.
        /// </summary>
        [Category("Appearance"),
        Browsable(true),
        Description("Show disabled grid as gray."),
        DefaultValue(false)]
        public bool ShowDisabledGridAsGray
        {
            get { return showDisabledGridAsGray; }
            set { showDisabledGridAsGray = value;
           scrollersFrame1.SizeGripperVisibility = SizeGripperVisibility.Hidden;           
            }
        }

        [DllImport("user32.dll", CharSet = CharSet.Auto, ExactSpelling = true, CallingConvention = CallingConvention.Winapi)]
        internal static extern bool LockWindowUpdate(IntPtr hWndLock);
        /// <override/>
        protected override void OnEnabledChanged(EventArgs e)
        {
            IntPtr handle = this.TopLevelControl != null ? this.TopLevelControl.Handle : this.Handle ;
            
            //Freeze Painting
            LockWindowUpdate(handle);

            if (this.showDisabledGridAsGray && !this.Enabled)        
                scrollersFrame1.AttachedTo = this;         
            else
                scrollersFrame1.DetachFrame();
            base.OnEnabledChanged(e);

            //Unlock painting
            LockWindowUpdate(IntPtr.Zero);
        }
        bool intialized = false;
        Syncfusion.Windows.Forms.ScrollersFrame scrollersFrame1 = new Syncfusion.Windows.Forms.ScrollersFrame();         
        /// <summary>
        /// Raises the <see cref="GridControlBase.CellDrawn"/> event.
        /// </summary>
        /// <param name="e">A <see cref="GridDrawCellEventArgs" /> that contains the event data.</param>
       protected virtual void OnCellDrawn(GridDrawCellEventArgs e)
        {
            if (!this.Enabled && this.showDisabledGridAsGray)
            {
                using (Brush b = new SolidBrush(Color.FromArgb(90, Color.LightGray)))
                {                                  
                    e.Graphics.FillRectangle(b, e.Bounds);
                    e.Style.TextColor = Color.DarkGray;
                }
            }
            if (AllowProportionalColumnSizing && !intialized)
            {
                SetColumnWidths();
                intialized = true;
            }
            if (this.eventsTarget != null)
            {
                this.eventsTarget.OnCellDrawn(e);
            }
#if DEBUG

            if (Switches.GridControlBaseEvents.TraceVerbose)
            {
                TraceUtil.TraceCurrentMethodInfo(PaneDesc, e);
            }
#else

            ;
#endif
            if (CellDrawn != null)
            {
                CellDrawn(this, e);
            }
        }

        /// <summary>
        /// Occurs when the grid draws a border around the current cell.
        /// </summary>
        [Category("Behavior")]
        [Description("Occurs when the grid draws a border around the current cell.")]
        public event GridDrawCurrentCellBorderEventHandler DrawCurrentCellBorder;

        /// <summary>
        /// Raises the <see cref="DrawCurrentCellBorder"/> event.
        /// </summary>
        /// <param name="e">A <see cref="GridDrawCurrentCellBorderEventArgs" /> that contains the event data.</param>
        protected virtual void OnDrawCurrentCellBorder(GridDrawCurrentCellBorderEventArgs e)
        {
            if (this.eventsTarget != null)
            {
                this.eventsTarget.OnDrawCurrentCellBorder(e);
            }

            if (DrawCurrentCellBorder != null)
            {
                DrawCurrentCellBorder(this, e);
            }
        }

        /// <summary>
        /// Initiates call to <see cref="OnDrawCurrentCellBorder"/>.
        /// </summary>
        /// <param name="e">The event arguments</param>
        public void RaiseDrawCurrentCellBorder(GridDrawCurrentCellBorderEventArgs e)
        {
            OnDrawCurrentCellBorder(e);
        }

#if DEBUG
        int lastRowTrace = 0;
#endif

        /// <summary>
        /// Call this method to draw a single cell to a graphics object at the specified rectangle. The method
        /// does not clip the output. <para/>
        /// Be aware that if
        /// pixel scrolling is enabled the caller needs to use clipping if this cell is at the top row or 
        /// left column and only partially visible.
        /// </summary>
        /// <param name="g">The <see cref="System.Drawing.Graphics"/> context of the canvas.</param>
        /// <param name="rowIndex">The row index.</param>
        /// <param name="colIndex">The column index.</param>
        /// <param name="cellRectangle">Specifies the cell rectangle. Please note that rectItem only is the visible bounds of cell.</param>
        /// <param name="style">The <see cref="GridStyleInfo"/> object that holds cell information.</param>
        /// <param name="forceDrawBackground">Forces the background should be drawn.</param>
        /// <param name="drawBorders">Specifies whether the borders should also be drawn or excluded.</param>
        public virtual void DrawSingleCell(Graphics g, int rowIndex, int colIndex, Rectangle cellRectangle, GridStyleInfo style, bool forceDrawBackground, bool drawBorders)
        {
            try
            {
                GridCellRendererBase renderer = CellRenderers[style.CellType];
                GridDrawCellEventArgs e = new GridDrawCellEventArgs(g, renderer, cellRectangle, rowIndex, colIndex, style, false);
                OnDrawCell(e);
                if (!e.Cancel)
                {
                    renderer.DrawSingleCell(g, cellRectangle, rowIndex, colIndex, style, drawBorders);
                }

                OnCellDrawn(e);
            }
            catch (Exception ex)
            {
                Trace.WriteLineIf(Switches.GridPaint.TraceWarning, String.Format("Exception in {0} Cell({1},{2}): {3}", new object[] { style.CellType, rowIndex, colIndex, ex.ToString() }));

                TraceUtil.TraceExceptionCatched(ex);

                if (AllowDrawItemRaiseExceptionCatched)
                {
                    if (!ExceptionManager.RaiseExceptionCatched(this, ex)
                        && !(ex is FormatException || ex.InnerException is FormatException))
                    {
                        throw;
                    }
                }

                g.FillRectangle(new SolidBrush(Color.Red), cellRectangle);
                g.DrawString("Exception", Font, SystemBrushes.WindowText, cellRectangle);
            }
            finally
            {
            }
        }

        /// <summary>
        /// You should override <see cref="GridControlBase.OnDrawCell"/> instead.
        /// </summary>
        /// <param name="g">The <see cref="System.Drawing.Graphics"/> context of the canvas.</param>
        /// <param name="rowIndex">The row index.</param>
        /// <param name="colIndex">The column index.</param>
        /// <param name="rectItem">The <see cref="System.Drawing.Rectangle"/> with the bounds.</param>
        /// <param name="style">The <see cref="GridStyleInfo"/> object that holds cell information.</param>
        [Syncfusion.Documentation.DocumentationExclude()]
        virtual public void OnDrawItem(Graphics g, int rowIndex, int colIndex, Rectangle rectItem, GridStyleInfo style)
        {
#if DEBUG
            if (Switches.GridPaint.TraceVerbose)
            {
                if (rowIndex != lastRowTrace)
                {
                    Trace.WriteLine(string.Empty);
                }

                lastRowTrace = rowIndex;
                Trace.WriteIf(Switches.GridPaint.TraceVerbose, String.Format("({0},{1},{2}:{3})", new object[] { rectItem.ToString(), rowIndex, colIndex, style.Text }));
            }
#endif

            ////Debug.Assert(rowIndex <= Model.RowCount && colIndex <= Model.ColCount, "Cell coordinates out of range");

            GridCellRendererBase pControl = CellRenderers[style.CellType];
            Debug.Assert(pControl != null, "CellRenderers is null.");

            try
            {
                bool isBackgroundErased =
                    !Model.Options.TransparentBackground
                    && !m_bDrawCoveredCell
                    && !m_bForceDrawBackground;
                GridDrawCellEventArgs e = new GridDrawCellEventArgs(g, pControl, rectItem, rowIndex, colIndex, style, isBackgroundErased);
                OnDrawCell(e);
                if (!e.Cancel)
                {
                    //// If this is the current cell, is control initialized correctly?
                    if (rowIndex != 0 && colIndex != 0
                        && CurrentCell.HasCurrentCellAt(rowIndex, colIndex)
                        && !pControl.InitalizedAt(rowIndex, colIndex))
                    {
                        pControl.Initialize(rowIndex, colIndex);
                    }

                    pControl.Draw(g, rectItem, rowIndex, colIndex, style);
                }

                OnCellDrawn(e);
            }
            catch (Exception ex)
            {
                Trace.WriteLineIf(Switches.GridPaint.TraceWarning, String.Format("Exception in {0} Cell({1},{2}): {3}", new object[] { style.CellType, rowIndex, colIndex, ex.ToString() }));

                TraceUtil.TraceExceptionCatched(ex);

                if (AllowDrawItemRaiseExceptionCatched)
                {
                    if (!ExceptionManager.RaiseExceptionCatched(this, ex)
                        && !(ex is FormatException || ex.InnerException is FormatException))
                    {
                        throw;
                    }
                }

                g.FillRectangle(new SolidBrush(Color.Red), rectItem);
                g.DrawString("Exception", Font, SystemBrushes.WindowText, rectItem);
            }
        }

        private bool allowDrawItemRaiseExceptionCatched = false;

        /// <internalonly/>
        /// <summary>Gets or sets a value indicating whether to AllowDrawItemRaiseExceptionCatched. Used internally.</summary>
        [Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public bool AllowDrawItemRaiseExceptionCatched
        {
            get { return allowDrawItemRaiseExceptionCatched; }
            set { allowDrawItemRaiseExceptionCatched = value; }
        }

        /// <summary>
        /// Occurs when the grid drawing engine wants to invert a cell when it belongs
        /// to a selected range.
        /// </summary>
        /// <param name="g">The <see cref="System.Drawing.Graphics"/> context of the canvas.</param>
        /// <param name="rowIndex">The row index.</param>
        /// <param name="colIndex">The column index.</param>
        /// <param name="rectItem">Specifies the cell rectangle. Please note that rectItem only is the visible bounds of cell.</param>
        /// <param name="inPaint">True if method was called from within <see cref="Control.OnPaint"/> for this control.</param>
        protected virtual void DrawInvertCell(Graphics g, int rowIndex, int colIndex, Rectangle rectItem, bool inPaint)
        {
            //// Note: rectItem only is the visible bounds of cell.

            if (!CurrentCell.HasCurrentCellAt(rowIndex, colIndex))
            {
                m_gridPaint.DrawInvertCell(g, rowIndex, colIndex, rectItem, inPaint); ////, this.RangeInfoToRectangle(GridRangeInfo.Cell(rowIndex, colIndex), GridRangeOptions.CalculateNonClientArea));
            }

            m_bInvertRect = false;
        }

        /// <internalonly/>
        /// <summary>Used internally.</summary>
        [Syncfusion.Documentation.DocumentationExclude()]
        public void IntDrawInvertCell(Graphics g, int rowIndex, int colIndex, Rectangle rectItem, bool inPaint)
        {
            // Note: rectItem only is the visible bounds of cell.
            DrawInvertCell(g, rowIndex, colIndex, rectItem, inPaint);
        }

        /// <overload>
        /// Draws the grid to the specified <see cref="Graphics"/> canvas and using the grid boundaries
        /// specified with <see cref="GridBounds"/>.
        /// </overload>
        /// <summary>
        /// Draws the grid to the specified <see cref="Graphics"/> canvas and using the grid boundaries
        /// specified with <see cref="GridBounds"/>.
        /// </summary>
        /// <param name="g">The <see cref="System.Drawing.Graphics"/> context of the canvas.</param>
        public void DrawGrid(Graphics g)
        {
            m_gridPaint.DrawGrid(g, true);
        }

        /// <summary>
        /// Draws the grid to the specified <see cref="Graphics"/> canvas and using the grid boundaries
        /// specified with <see cref="GridBounds"/>.
        /// </summary>
        /// <param name="g">The <see cref="System.Drawing.Graphics"/> context of the canvas.</param>
        /// <param name="shouldClip">Specifies if clipping region should be saved and restored after the grid is drawn.</param>
        public virtual void DrawGrid(Graphics g, bool shouldClip)
        {
            m_gridPaint.DrawGrid(g, shouldClip);
        }

        /// <summary>
        /// Draws the grid to the specified <see cref="Graphics"/> canvas and using the grid boundaries
        /// specified with <see cref="GridBounds"/>.
        /// </summary>
        /// <param name="g">The <see cref="System.Drawing.Graphics"/> context of the canvas.</param>
        /// <param name="bounds">The target bounds (client coordinates of the control) where the grid should be drawn.</param>
        /// <param name="drawCurrentCell">Specifies whether the current cell should be drawn and outlined; if False current cell is drawn just
        /// like any other regular cell.</param>
        public void DrawGrid(Graphics g, Rectangle bounds, bool drawCurrentCell)
        {
            DrawGrid(g, bounds, drawCurrentCell, true);
        }

        /// <summary>
        /// Draws the grid to the specified <see cref="Graphics"/> canvas and specified boundaries.
        /// </summary>
        /// <param name="g">The <see cref="System.Drawing.Graphics"/> context of the canvas.</param>
        /// <param name="bounds">The target area the grid should be drawn to.</param>
        /// <param name="drawCurrentCell">Indicates if current cell shoud be outlined.</param>
        /// <param name="shouldClip">Specifies if clipping region should be saved and restored after the grid was drawn.</param>
        public void DrawGrid(Graphics g, Rectangle bounds, bool drawCurrentCell, bool shouldClip)
        {
            // Draw a grid at specified bounds.
            Rectangle saveRect = this.HasGridBounds ? GridBounds : Rectangle.Empty;
            GridBounds = bounds;
            Region r = null;
            if (shouldClip)
            {
                r = g.Clip;
            }

            bool ccHidden = CurrentCell.InternalHide;
            bool staticDrawing = CurrentCell.StaticDrawing;
            if (!drawCurrentCell)
            {
                CurrentCell.InternalHide = true;
            }

            CurrentCell.StaticDrawing = true;
            int dummy = ViewLayout.LastVisibleRow;
            Model.FloatingCells.EvaluateFloatingCells(ViewLayout.VisibleCellsRange);
            Model.MergeCells.EvaluateMergeCells(ViewLayout.VisibleCellsRange);
            DrawGrid(g);
            CurrentCell.InternalHide = ccHidden;
            CurrentCell.StaticDrawing = staticDrawing;
            if (shouldClip)
            {
                g.Clip = r;
            }

            GridBounds = saveRect;
        }

        /// <overload>
        /// Draws the portion of the grid within the clipBounds to the specified <see cref="Graphics"/> canvas.
        /// </overload>
        /// <summary>
        /// Draws the portion of the grid within the clipBounds to the specified <see cref="Graphics"/> canvas. Clipping will 
        /// automatically occur when the grid was horizontally scrolled and the first column is only partially or when
        /// the grid was vertically scrolled and the first row is only partially visible.
        /// </summary>
        /// <param name="g">The <see cref="System.Drawing.Graphics"/> context of the canvas.</param>
        /// <param name="clipBounds">The target area the grid should be drawn to.</param>
        /// <remarks>
        /// Use this method if you want to increase performance when you have frequent Invalidate(Rectangle) and Update() calls.
        /// </remarks>
        /// <para/>
        /// <example>
        /// This sample below lets you draw changes cells directly to graphics context instead of using
        /// the conventional Invalidate / Update approach.
        /// <code lang="C#">
        /// bool drawDirectToDC = true;
        /// Graphics g = null;
        /// <para/>
        /// if (drawDirectToDC)
        ///     g = m_syncGrid.CreateGridGraphics();
        /// <para/>
        /// // Clear our volatile cache
        /// m_syncGrid.ResetVolatileData();
        /// <para/>
        /// // Handle case when values is change for current cell.
        /// if (g1 == m_syncGrid.CurrentCell.RangeInfo)
        /// {
        ///     m_syncGrid.CurrentCell.Model.ResetActiveText(row, col);
        ///     continue;
        /// }
        /// <para/>
        /// // Draw direct to dc
        /// if (drawDirectToDC)
        /// {
        ///     Rectangle bounds = m_syncGrid.RangeInfoToRectangle(g1);
        ///     if (!bounds.IsEmpty)
        ///     {
        ///     // Instead of
        ///     if (false)
        ///     {
        ///             // Draw each cell individually to graphics context
        /// <para/>
        ///         GridStyleInfo style = this.m_syncGrid.Model[row, col];
        ///         GridCellRendererBase renderer = this.m_syncGrid.CellRenderers[style.CellType];
        /// <para/>
        ///         // Get client rectangle
        ///         bounds = style.CellModel.SubtractBorders(bounds, style, this.m_syncGrid.IsRightToLeft());
        /// <para/>
        ///         // Draw cell Background
        ///         Syncfusion.Drawing.BrushPaint.FillRectangle(g, bounds, style.Interior);
        /// <para/>
        ///         // Draw cell text
        ///         renderer.Draw(g, bounds, row, col, style);
        ///     }
        ///     else
        ///     {
        ///         // DrawClippedGrid method lets you simply draw the cells at the specified bounds directly to the graphics context.
        ///         // less code than drawing each cell individually ....
        /// <para/>
        ///         m_syncGrid.DrawClippedGrid(g, bounds);
        ///     }
        /// }
        /// else
        /// {
        ///     // Use more conventional Invalidate / Update mechanism.
        ///         m_syncGrid.RefreshRange(GridRangeInfo.Cell(row, col);
        /// }
        /// <para/>
        /// if (g != null)
        ///     g.Dispose();
        /// </code>
        /// </example>
        public void DrawClippedGrid(Graphics g, Rectangle clipBounds)
        {
            ViewLayout.Lock();
            int dummy = ViewLayout.LastVisibleRow;
            Model.FloatingCells.EvaluateFloatingCells(ViewLayout.VisibleCellsRange);
            Model.MergeCells.EvaluateMergeCells(ViewLayout.VisibleCellsRange);

            bool shouldClip = false;
            if (this.hScrollPixelDelta > 0
                || this.vScrollPixelDelta > 0)
            {
                shouldClip = true;
            }

            //// Note: Could possibly also check the following:
            //// && clipBounds.Left == Model.ColWidths.GetTotal(0, Model.Cols.FrozenCount)
            //// && clipBounds.Top == Model.RowHeights.GetTotal(0, Model.Rows.FrozenCount)
            //// But previous code is safer

            m_gridPaint.DrawGrid(g, shouldClip, clipBounds);
            ViewLayout.Unlock();
        }

        /// <summary>
        /// Draws the portion of the grid within the clipBounds to the specified <see cref="Graphics"/> canvas.
        /// </summary>
        /// <param name="g">The <see cref="System.Drawing.Graphics"/> context of the canvas.</param>
        /// <param name="clipBounds">The target area the grid should be drawn to.</param>
        /// <param name="shouldClip">Whether or not the clipBounds is taken into account.</param>
        /// <genoverload/>
        public void DrawClippedGrid(Graphics g, Rectangle clipBounds, bool shouldClip)
        {
            ViewLayout.Lock();
            int dummy = ViewLayout.LastVisibleRow;
            Model.FloatingCells.EvaluateFloatingCells(ViewLayout.VisibleCellsRange);
            Model.MergeCells.EvaluateMergeCells(ViewLayout.VisibleCellsRange);

            m_gridPaint.DrawGrid(g, shouldClip, clipBounds);
            ViewLayout.Unlock();
        }

        Rectangle saveRect = Rectangle.Empty;
        bool ccHidden = false;
        bool staticDrawing = false;
        Control renderControl = null;

        /// <summary>
        /// Switches the grid into a special mode in which you can call its <see cref="DrawGrid(System.Drawing.Graphics)"/>
        /// method to draw its contents at a different screen location and with different size than
        /// the current visible grid. You must call <see cref="ResetWindowlessBounds"/> to switch
        /// the grid back to normal operational mode.
        /// </summary>
        /// <param name="renderControl">The parent control with a window handle.</param>
        /// <param name="bounds">The new location and bounds of the grid.</param>
        /// <param name="drawCurrentCell">True if current cell should be outlined in a subsequent DrawGrid call;
        /// False if it should not be drawn.</param>
        public void SetWindowlessBounds(Control renderControl, Rectangle bounds, bool drawCurrentCell)
        {
            Rectangle saveRect = this.HasGridBounds ? GridBounds : Rectangle.Empty;
            GridBounds = bounds;
            bool ccHidden = CurrentCell.InternalHide;
            this.staticDrawing = CurrentCell.StaticDrawing;
            this.renderControl = CurrentCell.StaticRenderControl;
            if (!drawCurrentCell)
            {
                CurrentCell.InternalHide = true;
            }

            CurrentCell.StaticDrawing = true;
            CurrentCell.StaticRenderControl = renderControl;
            ////            ViewLayout.Reset();
            ////            int dummy = ViewLayout.LastVisibleRow;
            ////            Model.FloatingCells.EvaluateFloatingCells(ViewLayout.VisibleCellsRange);
            ////            Model.MergeCells.EvaluateMergeCells(ViewLayout.VisibleCellsRange);
            BeginUpdate(BeginUpdateOptions.Invalidate);
        }

        /// <summary>
        /// Switches the grid back into a normal operation mode after a <see cref="SetWindowlessBounds"/> call.
        /// </summary>
        public void ResetWindowlessBounds()
        {
            CurrentCell.InternalHide = ccHidden;
            CurrentCell.StaticDrawing = staticDrawing;
            CurrentCell.StaticRenderControl = renderControl;
            GridBounds = saveRect;
            EndUpdate(false);
        }

        /// <summary>
        /// Occurs when the grid drawing engine wants to draw borders for a covered cell.
        /// </summary>
        /// <param name="g">The <see cref="System.Drawing.Graphics"/> context of the canvas.</param>
        /// <param name="rectItem">Specifies the cell rectangle.</param>
        /// <param name="style">The <see cref="GridStyleInfo"/> object that holds cell information.</param>
        internal void OnDrawBorders(Graphics g, Rectangle rectItem, GridStyleInfo style)
        {
            m_gridPaint.OnDrawBorders(g, rectItem, style);
        }

        /// <summary>
        /// Inverts a given area on the specified <see cref="Graphics"/> canvas.
        /// </summary>
        /// <param name="g">The <see cref="System.Drawing.Graphics"/> context of the canvas.</param>
        /// <param name="r">A <see cref="Rectangle"/> to invert.</param>
        public void InvertRect(Graphics g, Rectangle r)
        {
            m_gridPaint.InvertRect(g, r);
        }

        /// <summary>
        /// Outlines the row and column header for the current cell.
        /// </summary>
        /// <param name="nEditRow">The row index of the current cell.</param>
        /// <param name="nEditCol">The column index of the current cell.</param>
        /// <param name="direction">The direction the current cell moved.</param>
        public void OutlineCurrentCellHeader(int nEditRow, int nEditCol, ScrollBars direction)
        {
            if (this.OutlineCurrentCellHeaderManager != null)
            {
                this.OutlineCurrentCellHeaderManager.OutlineCurrentCellHeader(nEditRow, nEditCol, direction);
            }
        }

        /// <summary>
        /// Gets a value indicating whether the grid is inside a <see cref="Control.OnPaint"/> method call.
        /// </summary>
        [Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public bool IsDrawing
        {
            get
            {
                return m_gridPaint.m_nNestedDraw > 0;
            }
        }

        /// <summary>
        /// Occurs when the grid drawing engine wants to draw the specified range of visible cells
        /// that need repainting.
        /// </summary>
        /// <param name="topRow">The top client row.</param>
        /// <param name="leftCol">The left client column.</param>
        /// <param name="bottomRow">The bottom client row.</param>
        /// <param name="rightCol">The right client column.</param>
        /// <param name="g">The <see cref="System.Drawing.Graphics"/> context of the canvas.</param>
        /// <param name="rectClip">The client rectangle with the clipping bounds.</param>
        /// <remarks>
        /// The row and index positions are relative to the top and left column.
        /// <para/>
        /// <see cref="GridControlBase.GetRow"/> and <see cref="GridControlBase.GetCol"/> let you convert
        /// client row and column positions into absolute row and column indexes.
        /// </remarks>
        public virtual void OnDrawClientRowCol(int topRow, int leftCol, int bottomRow, int rightCol, Graphics g, Rectangle rectClip)
        {
            ////TraceUtil.TraceCurrentMethodInfo(topRow, leftCol, bottomRow, rightCol);
            m_gridPaint.DrawClientRowCol(topRow, leftCol, bottomRow, rightCol, g, rectClip);
        }

        /// <override/>
        protected override void UpdateScrollTips(ScrollEventArgs se)
        {
            if (this.IsRightToLeft())
            {
                se = new ScrollEventArgs(se.Type, gridScroll.ReverseHScrollValueRTL(se.NewValue));
            }

            base.UpdateScrollTips(se);
        }

        /// <override/>
        protected override void OnScrollTipFeedback(ScrollTipFeedbackEventArgs e)
        {
            if (this.eventsTarget != null)
            {
                this.eventsTarget.OnScrollTipFeedback(e);
            }

            if (e.Text == string.Empty)
            {
                if (e.Action == ScrollTipActions.ThumbTrack)
                {
                    string text;
                    if (e.ScrollBar == ScrollBars.Horizontal)
                    {
                        text = "Column AAAA";
                    }
                    else
                    {
                        text = "Row 999999";
                    }

                    e.Size = ScrollTip.GetPreferredSize(text);
                }
                else if (e.Action == ScrollTipActions.Scroll)
                {
                    string text;
                    if (e.ScrollBar == ScrollBars.Horizontal)
                    {
                        text = String.Format("Column {0}", GridRangeInfo.GetAlphaLabel(e.Value));
                    }
                    else
                    {
                        text = String.Format("Row {0}", GridRangeInfo.GetNumericLabel(e.Value));
                    }

                    e.Text = text;
                }
            }
#if DEBUG
            if (Switches.GridControlBaseEvents.TraceVerbose)
            {
                TraceUtil.TraceCurrentMethodInfo(PaneDesc, e);
            }
#else
            ;
#endif
            base.OnScrollTipFeedback(e);
        }

        #endregion
        #region Fields
        internal IDynamicSplitterFrame splitterControl;
        internal IGridDropDownContainer popupParent = null;

        /// <internalonly/>
        /// <summary>Used internally.</summary>
        [Syncfusion.Documentation.DocumentationExclude()]
        public void InitSplitterControl()
        {
            if (this.FillSplitterPane)
            {
                this.splitterControl = (IDynamicSplitterFrame)GridUtil.GetParentControl(this, typeof(IDynamicSplitterFrame));
                if (splitterControl != null)
                {
                    this.VScrollBehavior = GridScrollbarMode.Shared | GridScrollbarMode.AutoScroll;
                    this.HScrollBehavior = GridScrollbarMode.Shared | GridScrollbarMode.AutoScroll;
                }
            }
            else
            {
                this.splitterControl = null;
            }
        }

        /// <summary>
        /// Gets or sets to allow you to specify a <see cref="GridDropDownContainer"/> as parent of the
        /// grid when used inside a drop-down. This is necessary if nested popups are needed.
        /// </summary>
        [Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public IGridDropDownContainer DropDownContainerParent
        {
            get
            {
                return popupParent;
            }

            set
            {
                popupParent = value;
            }
        }

        ////        int m_nHitTestFrame = 8;

        /// <internalonly/>
        bool m_bHitTestSelEdge = false; // will be enabled from EnableOleDataSource
        internal bool m_bInitDone = true;

        /// <internalonly/>
        /// <summary>Gets or sets a value indicating whether HitTestSelectionEdge. Used internally.</summary>
        [Syncfusion.Documentation.DocumentationExclude()]
        [Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public bool HitTestSelectionEdge
        {
            get
            {
                return m_bHitTestSelEdge;
            }

            set
            {
                m_bHitTestSelEdge = value;
            }
        }
        
        GridPrintInfo printInfo = null;

        /// <summary>
        /// Gets temporary information related to printing. This class will change in future versions.
        /// </summary>
        [DesignerSerializationVisibilityAttribute(DesignerSerializationVisibility.Hidden)]
        [Browsable(false)]
        [Syncfusion.Documentation.DocumentationExclude()]
        public GridPrintInfo PrintInfo
        {
            get
            {
                if (printInfo == null)
                {
                    printInfo = new GridPrintInfo();
                }

                return printInfo;
            }
        }

        internal int m_dxWidth = 8;
        internal int m_dyHeight = 13;

        const int Grid_NXYFACTOR = 1000;

        const int Grid_NXAVGWIDTH = 1000;       //// logical char width (average)
        const int Grid_NYHEIGHT = 1000;         //// logical char height

        #endregion
        #region TabRecordSplitter
        /// <summary>
        /// Gets a value indicating whether the grid is currently handling a <see cref="Control.Validating"/> event.
        /// </summary>
        [Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public bool IsInLeaveOrValidate
        {
            get
            {
                return this.IsValidating;
            }
        }

        bool isMouseDownCalled = false;
        
        /// <summary>
        /// Shoulds the activate current cell in enter.
        /// </summary>
        /// <returns>returns boolean value to indicate Should ActivateCurrentCell InEnter</returns>
        /// <internalonly/>
        [Syncfusion.Documentation.DocumentationExclude()]
        protected virtual bool ShouldActivateCurrentCellInEnter()
        {
            return !IsMousePressed || (isMouseDownCalled && ShouldActivateCurrentCell());
        }

        /// <summary>
        /// Used internally.
        /// </summary>
        /// <param name="rowIndex">Index of the row.</param>
        /// <param name="colIndex">Index of the col.</param>
        /// <returns>
        /// <c>true</c> if [is visible cell] [the specified row index]; otherwise, <c>false</c>.
        /// </returns>
        /// <internalonly/>
        [Syncfusion.Documentation.DocumentationExclude()]
        public bool IsVisibleCell(int rowIndex, int colIndex)
        {
            if (rowIndex <= ViewLayout.LastVisibleRow && colIndex <= ViewLayout.LastVisibleCol)
            {
                GridRangeInfo rgCell = ViewLayout.CombineSpannedRanges(GridRangeInfo.Cell(rowIndex, colIndex));
                return ViewLayout.IsRangeVisible(rgCell);
            }

            return false;
        }

        /// <summary>
        /// Used internally.
        /// </summary>
        [Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        [Syncfusion.Documentation.DocumentationExclude()]
        public void FixCurrentCellGotFocus()
        {
            FixCurrentCellGotFocus(false);
        }

        /// <summary>
        /// Used internally.
        /// </summary>
        [Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        [Syncfusion.Documentation.DocumentationExclude()]
        public void FixCurrentCellGotFocus(bool noScroll)
        {
            if (this.DesignMode)
            {
                return;
            }

            if (CurrentCell.HasCurrentCell &&
                (IsVisibleCell(CurrentCell.RowIndex, CurrentCell.ColIndex) || !IsMousePressed))
            {
                if (ViewLayout.ScrollAreaBounds.Contains(ViewLayout.RangeInfoToRectangle(CurrentCell.RangeInfo, GridCellSizeKind.ActualSize)))
                {
                    if ((CurrentCell.IsEditing && !CurrentCell.HasControlFocus)
                        || (Model.Options.ActivateCurrentCellBehavior & GridCellActivateAction.SetCurrent) != 0)
                    {
#if DEBUG
                        if (Switches.GridFocus.TraceVerbose)
                        {
                            TraceUtil.TraceCurrentMethodInfo(CurrentCell);
                        }
#else
                        ;
#endif

                        bool inScroll = ScrollGrid.m_bInDoScroll;
                        ScrollGrid.m_bInDoScroll |= noScroll;
                        if (CurrentCell.BeginEdit())
                            CurrentCell.Renderer.SetHasFocusControl(true);
                        if (CurrentCell.HasControlFocus
                            || (Model.Options.ShowCurrentCellBorderBehavior & GridShowCurrentCellBorder.GrayWhenLostFocus) != 0)
                        {
                            CurrentCell.Invalidate();
                        }

                        ScrollGrid.m_bInDoScroll = inScroll;
                    }
                    else if ((Model.Options.ShowCurrentCellBorderBehavior & GridShowCurrentCellBorder.GrayWhenLostFocus) != 0)
                    {
                        CurrentCell.Invalidate();
                    }
                }
            }

            if ((Model.Options.ActivateCurrentCellBehavior & Syncfusion.Windows.Forms.Grid.GridCellActivateAction.SetCurrent) != 0)
            {
                CurrentCell.BeginEdit();
            }

            if (!CurrentCell.HasControlFocus && this.CanSelect && FocusOnMouseDown && WantKeys && !this.QueryFocusInside())
            {
                Focus();
            }
            else
            {
                CurrentCell.Invalidate();
            }
        }

        /// <summary>
        /// Used internally.
        /// </summary>
        [Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        [Syncfusion.Documentation.DocumentationExclude()]
        public void FixCurrentCellLostFocus()
        {
            if (this.DesignMode)
            {
                return;
            }

            if (CurrentCell.HasCurrentCell && IsVisibleCell(CurrentCell.RowIndex, CurrentCell.ColIndex))
            {
                if (CurrentCell.IsEditing && CurrentCell.HasControlFocus)
                {
#if DEBUG
                    if (Switches.GridFocus.TraceVerbose)
                    {
                        TraceUtil.TraceCurrentMethodInfo(CurrentCell);
                    }
#else
                    ;
#endif
                    CurrentCell.Renderer.SetHasFocusControl(false);
                    CurrentCell.Invalidate();
                }
                else
                {
                    CurrentCell.Invalidate();
                }
            }
        }

        /// <summary>
        /// Used internally.
        /// </summary>
        [Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        [Syncfusion.Documentation.DocumentationExclude()]
        void FixDelayedCurrentCellActivate()
        {
            if (this.delayCurrentCellActivateArgs != null)
            {
                int rowIndex = Math.Max(delayCurrentCellActivateArgs.RowIndex, Model.Rows.HeaderCount + 1);
                int colIndex = Math.Max(delayCurrentCellActivateArgs.ColIndex, Model.Cols.HeaderCount + 1);
                if (CurrentCell.Activate(rowIndex, colIndex, delayCurrentCellActivateArgs.Options))
                {
                    if (!IsMousePressed)
                    {
                        CurrentCell.ScrollInView(GridScrollCurrentCellReason.Activate);
                    }

                    delayCurrentCellActivateArgs = null;
                    forceCurrentCellMoveTo = true;
                }
            }
        }

        /// <override/>
        protected override void OnControlGotFocus()
        {
            base.OnControlGotFocus();
            if ((this.IsMousePressed && !this.isMouseDownCalled) || cancelMode)
            {
                return;
            }

            FixCurrentCellGotFocus(true);
        }

        /// <override/>
        protected override void OnControlLostFocus()
        {
            base.OnControlLostFocus();
            FixCurrentCellLostFocus();
        }

        /// <override/>
        protected override/*Control*/ void OnEnter(EventArgs e)
        {
            if (this.eventsTarget != null)
            {
                this.eventsTarget.OnEnter(e);
            }

            if (this.IsSplitterPaneClosing)
            {
                return;
            }
#if DEBUG

            if (Switches.GridControlBaseEvents.TraceVerbose)
            {
                TraceUtil.TraceCurrentMethodInfo(PaneDesc);
            }
#else

            ;
#endif

            base.OnEnter(e);

            if ((this.IsMousePressed && !this.isMouseDownCalled) || cancelMode)
            {
                cancelMode = false;
                return;
            }

            if (ShouldActivateCurrentCellInEnter())
            {
                FixDelayedCurrentCellActivate();
            }
            else if ((Model.Options.ShowCurrentCellBorderBehavior & GridShowCurrentCellBorder.GrayWhenLostFocus) != 0)
            {
                CurrentCell.Invalidate();
            }

            if (!this.IsMousePressed && CurrentCell.HasCurrentCell)
            {
                CurrentCell.ScrollInView(GridScrollCurrentCellReason.GridFocus);
            }

            this.Model.ActiveGridView = this;
        }

        /// <override/>
        protected override/*Control*/ void OnDeactivated(EventArgs e)
        {
            if (this.eventsTarget != null)
            {
                this.eventsTarget.OnDeactivated(e);
            }

            // This is probably because of an OnValidated event raised after setting focus to a child cell
            // (e.g. change "CellType" in PropertyGrid ind CellTypes / CurrencyCells). If we don't return
            // this causes a "Deactivate called while Activating cell" exception.
            if (this.inOnPaint)
            {
                return;
            }
#if DEBUG

            if (Switches.GridControlBaseEvents.TraceVerbose)
            {
                TraceUtil.TraceCurrentMethodInfo(PaneDesc);
            }
#else

            ;
#endif

            base.OnDeactivated(e);
#if DEBUG

            if (Switches.Development.TraceVerbose)
            {
                TraceUtil.TraceCurrentMethodInfo(PaneDesc);
            }
#else

            ;
#endif

            ////if (this.isLostFocus && Model.Options.ShowCurrentCellBorderBehavior != GridShowCurrentCellBorder.AlwaysVisible)

            //// Only deactivate cell if this control has been validated or cell has been saved.
            if (ShouldDeactivateCurrentCell())
            {
                ////CurrentCell.ActivateOnGotFocus |= CurrentCell.HasCurrentCell;
                delayCurrentCellActivateArgs = new GridCurrentCellActivatingEventArgs(CurrentCell.RowIndex, CurrentCell.ColIndex, GridSetCurrentCellOptions.None);
                CurrentCell.Deactivate(true);
            }

            isMouseDownCalled = false;
        }

        /// <summary>
        /// Shoulds the deactivate current cell.
        /// </summary>
        /// <returns>returns boolean value</returns>
        /// <internalonly/>
        [Syncfusion.Documentation.DocumentationExclude()]
        protected virtual bool ShouldDeactivateCurrentCell()
        {
            return CurrentCell.HasCurrentCell && !CurrentCell.IsModified
                && (Model.Options.ShowCurrentCellBorderBehavior & GridShowCurrentCellBorder.WhenGridActive) != 0;
        }

        /// <summary>
        /// Shoulds the activate current cell.
        /// </summary>
        /// <returns>returns boolean value</returns>
        /// <internalonly/>
        [Syncfusion.Documentation.DocumentationExclude()]
        protected virtual bool ShouldActivateCurrentCell()
        {
            return true;
        }

        /// <override/>
        /// <summary>
        /// Indicates whether this control contains focus.
        /// </summary>
        /// <returns>True if it contains the focus.</returns>
        public override bool QueryFocusInside()
        {
            if (!Disposing && !IsDisposed)
            {
                if (this.DesignMode || base.QueryFocusInside())
                {
                    return true;
                }
                else if (CurrentCell.HasCurrentCell)
                {
                    IQueryFocusInside qfi = CurrentCell.Renderer as IQueryFocusInside;
                    if (qfi != null)
                    {
                        return qfi.QueryFocusInside();
                    }
                }
            }

            return false;
        }

        internal bool validatingFailed = false;
        bool ignoreNextValidating = false;

        /// <override/>
        protected override void OnValidating(CancelEventArgs e)
        {
            if (this.eventsTarget != null)
            {
                this.eventsTarget.OnValidating(e);
            }
#if DEBUG

            if (Switches.GridControlBaseEvents.TraceVerbose)
            {
                TraceUtil.TraceCurrentMethodInfo(e.Cancel, PaneDesc);
            }
#else

            ;
#endif

            if (ignoreNextValidating)
            {
                ignoreNextValidating = false;
                return;
            }

            base.OnValidating(e);

            if (this.IsSplitterPaneClosing || e.Cancel)
            {
                return;
            }

            OnGridValidating(e);
        }

        /// <summary>
        /// This is called from <see cref="OnValidating"/> after the grid has checked that Validating events
        /// should be passed on and not ignored.
        /// </summary>
        /// <param name="e">The event data.</param>
        protected virtual void OnGridValidating(CancelEventArgs e)
        {
            if (this.eventsTarget != null)
            {
                this.eventsTarget.OnGridValidating(e);
            }
#if DEBUG

            if (Switches.Development.TraceVerbose)
            {
                TraceUtil.TraceCurrentMethodInfo("----BEGIN----", this);
            }
#else

            ;
#endif

            try
            {
                if (!e.Cancel && CurrentCell.HasCurrentCell && CurrentCell.IsModified)
                {
                    try
                    {
                        e.Cancel = !CurrentCell.ConfirmChanges() || !CurrentCell.IsValid;
                        if (e.Cancel)
                        {
                            validatingFailed = true;
                            GridCurrentCell cc = CurrentCell;
#if DEBUG
                            if (Switches.Development.TraceVerbose)
                            {
                                TraceUtil.TraceCurrentMethodInfo("Cancel: ", cc.ErrorMessage, cc);
                            }
#else
                            ;
#endif

                            if (cc.ErrorMessage.Length > 0)
                            {
                                cc.DisplayWarningText(cc.ErrorMessage);
                                cc.ResetError();
                            }
                        }

                        ScrollGrid.m_cxOld = 0;
                        ScrollGrid.m_cyOld = 0;
                    }
                    catch (Exception ex)
                    {
                        TraceUtil.TraceExceptionCatched(ex);
                        if (!ExceptionManager.RaiseExceptionCatched(this, ex))
                        {
                            throw;
                        }

                        validatingFailed = true;
                        e.Cancel = true;
                        Model.ActiveGridView = this;
                        CurrentCell.Activate(CurrentCell.RowIndex, CurrentCell.ColIndex);
                        CurrentCell.ErrorMessage = ex.Message;
                        CurrentCell.Exception = ex;
                        return;
                    }
                }
            }
            finally
            {
                this.validatingFailed = e.Cancel;
#if DEBUG
                if (Switches.Development.TraceVerbose)
                {
                    TraceUtil.TraceCurrentMethodInfo("----END----", this);
                }
#else
                ;
#endif
            }
        }

        internal void NotifyDeactivate()
        {
            if (this.IsSplitterPaneClosing)
            {
                return;
            }

            if (CurrentCell.InShowDropDown || CurrentCell.IsInActiveOrDeactivate)
            {
                return;
            }
#if DEBUG

            if (Switches.Development.TraceVerbose)
            {
                TraceUtil.TraceCurrentMethodInfo(this);
            }
#else

            ;
#endif

            ScrollGrid.m_cxOld = 0;
            ScrollGrid.m_cyOld = 0;
        }

        internal void NotifyActivate()
        {
#if DEBUG
            if (Switches.Development.TraceVerbose)
            {
                TraceUtil.TraceCurrentMethodInfo(this);
            }
#else
            ;
#endif

            ScrollGrid.m_cxOld = 0;
            ScrollGrid.m_cyOld = 0;
        }

        /// <summary>
        /// Occurs before the grid is about to be left because the user is at the top-left or bottom-right
        /// cell and about to tab out of the grid.
        /// This event is only raised if the <see cref="GridWrapCellBehavior.NextControlInForm"/>
        /// has been specified for <see cref="GridModelOptions.WrapCell"/>. 
        /// </summary> 
        /// <remarks><see cref="GridWrapCellNextControlInFormEventHandler"/></remarks>
        [Category("Behavior")]
        [Description("Occurs before the grid is about to be left because the user is at the top-left or bottom-right cell and about to tab out of the grid.")]
        public event GridWrapCellNextControlInFormEventHandler WrapCellNextControlInForm;

        /// <summary>
        /// Raises the <see cref="WrapCellNextControlInForm"/> event.
        /// </summary>
        /// <param name="e">A <see cref="GridWrapCellNextControlInFormEventArgs" /> that contains the event data.</param>
        protected virtual void OnWrapCellNextControlInForm(GridWrapCellNextControlInFormEventArgs e)
        {
            if (this.eventsTarget != null)
            {
                this.eventsTarget.OnWrapCellNextControlInForm(e);
            }

            if (WrapCellNextControlInForm != null)
            {
                WrapCellNextControlInForm(this, e);
            }
        }

        /// <summary>
        /// Initiates call to <see cref="OnWrapCellNextControlInForm"/>.
        /// </summary>
        /// <param name="e">The event arguments.</param>
        public void RaiseWrapCellNextControlInForm(GridWrapCellNextControlInFormEventArgs e)
        {
            OnWrapCellNextControlInForm(e);
        }

        //// Splitter support.

        int m_nSplitRow;
        int m_nSplitCol;

        /// <summary>
        /// Gets the row in the splitter where this control is displayed, if the grid is used inside a dynamic splitter control.
        /// </summary>
        [Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public int SplitRow
        {
            get
            {
                return m_nSplitRow;
            }
        }

        /// <summary>
        /// Gets the column in the splitter where this control is displayed, if the grid is used inside a dynamic splitter control.
        /// </summary>
        [Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public int SplitCol
        {
            get
            {
                return m_nSplitCol;
            }
        }

        /// <summary>
        /// If the grid is used inside a dynamic splitter control, this method gives you
        /// access to the sibling grid control in another pane of the same splitter control.
        /// </summary>
        /// <param name="nSplitRow">The row of the current control in the splitter control.</param>
        /// <param name="nSplitCol">The column of the current control in the splitter control.</param>
        /// <returns>A reference to the <see cref="GridControlBase"/> in the sibling pane.</returns>
        public GridControlBase GetOtherSplitRowChildPane(int nSplitRow, int nSplitCol)
        {
            if (splitterControl != null && splitterControl.ColumnCount > 1)
            {
                return splitterControl.GetPane(nSplitRow, 1 - nSplitCol) as GridControlBase;
            }

            return null;
        }

        /// <summary>
        /// If the grid is used inside a dynamic splitter control, this method gives you
        /// access to the sibling grid control in another pane of the same splitter control.
        /// </summary>
        /// <param name="nSplitRow">The row of the current control in the splitter control.</param>
        /// <param name="nSplitCol">The column of the current control in the splitter control.</param>
        /// <returns>A reference to the <see cref="GridControlBase"/> in the sibling pane.</returns>
        public GridControlBase GetOtherSplitColumnChildPane(int nSplitRow, int nSplitCol)
        {
            if (splitterControl != null && splitterControl.RowCount > 1)
            {
                return splitterControl.GetPane(1 - nSplitRow, nSplitCol) as GridControlBase;
            }

            return null;
        }

        #endregion
        #region CurrentCell
        internal bool RaiseCurrentCellMoving(ref int rowIndex, ref int colIndex, ref GridSetCurrentCellOptions options)
        {
            if (CurrentCell.IsSuspendEvents || IsDisposed)
            {
                return true;
            }
#if DEBUG

            if (Switches.CurrentCell.TraceVerbose)
            {
                TraceUtil.TraceCurrentMethodInfo(PaneDesc, rowIndex, colIndex, options);
            }
#else

            ;
#endif

            GridCurrentCellMovingEventArgs e = new GridCurrentCellMovingEventArgs(rowIndex, colIndex, options);

            if (!CurrentCell.StaticDrawing && !CheckDelayInitialActivateCC(e))
            {
                return false;
            }

            this.OnCurrentCellMoving(e);
            options = e.Options;
            rowIndex = e.RowIndex;
            colIndex = e.ColIndex;
            return !e.Cancel;
        }

        internal void RaiseCurrentCellMoved(GridSetCurrentCellOptions options)
        {
            if (CurrentCell.IsSuspendEvents || IsDisposed)
            {
                return;
            }
#if DEBUG

            if (Switches.CurrentCell.TraceVerbose)
            {
                TraceUtil.TraceCurrentMethodInfo(PaneDesc, options);
            }
#else

            ;
#endif

            GridCurrentCellMovedEventArgs e = new GridCurrentCellMovedEventArgs(options);
            this.OnCurrentCellMoved(e);
        }

        internal void RaiseCurrentCellMoveFailed(int rowIndex, int colIndex, GridSetCurrentCellOptions options)
        {
            if (CurrentCell.IsSuspendEvents || IsDisposed)
            {
                return;
            }

            if (delayCurrentCellActivateArgs != null)
            {
                return;
            }
#if DEBUG

            if (Switches.CurrentCell.TraceVerbose)
            {
                TraceUtil.TraceCurrentMethodInfo(PaneDesc, rowIndex, colIndex, options);
            }
#else

            ;
#endif

            GridCurrentCellMoveFailedEventArgs e = new GridCurrentCellMoveFailedEventArgs(rowIndex, colIndex, options);
            this.OnCurrentCellMoveFailed(e);
        }

        internal bool RaiseCurrentCellActivating(ref int rowIndex, ref int colIndex, ref GridSetCurrentCellOptions options)
        {
            if (CurrentCell.IsSuspendEvents || IsDisposed)
            {
                return true;
            }
#if DEBUG

            if (Switches.CurrentCell.TraceVerbose)
            {
                TraceUtil.TraceCurrentMethodInfo(PaneDesc, rowIndex, colIndex, options);
            }
#else

            ;
#endif

            GridCurrentCellActivatingEventArgs e = new GridCurrentCellActivatingEventArgs(rowIndex, colIndex, options);

            if (!CurrentCell.StaticDrawing && !CheckDelayInitialActivateCC(e))
            {
                return false;
            }

            this.OnCurrentCellActivating(e);
            rowIndex = e.RowIndex;
            colIndex = e.ColIndex;
            options = e.Options;
            return !e.Cancel;
        }

        internal void RaiseCurrentCellActivated()
        {
            if (CurrentCell.IsSuspendEvents || IsDisposed)
            {
                return;
            }
#if DEBUG

            if (Switches.CurrentCell.TraceVerbose)
            {
                TraceUtil.TraceCurrentMethodInfo(PaneDesc);
            }
#else

            ;
#endif
            this.OnCurrentCellActivated(EventArgs.Empty);
        }

        internal void RaiseCurrentCellActivateFailed(int rowIndex, int colIndex)
        {
            if (CurrentCell.IsSuspendEvents || IsDisposed)
            {
                return;
            }

            if (delayCurrentCellActivateArgs != null)
            {
                return;
            }
#if DEBUG

            if (Switches.CurrentCell.TraceVerbose)
            {
                TraceUtil.TraceCurrentMethodInfo(PaneDesc, rowIndex, colIndex);
            }
#else

            ;
#endif

            GridCurrentCellActivateFailedEventArgs e = new GridCurrentCellActivateFailedEventArgs(rowIndex, colIndex);
            this.OnCurrentCellActivateFailed(e);
        }

        /// <summary>
        /// Initiates call to <see cref="OnCurrentCellDeleting"/>.
        /// </summary>
        /// <returns>True if the call is initiated.</returns>
        public bool RaiseCurrentCellDeleting()
        {
            if (CurrentCell.IsSuspendEvents || IsDisposed)
            {
                return true;
            }
#if DEBUG

            if (Switches.CurrentCell.TraceVerbose)
            {
                TraceUtil.TraceCurrentMethodInfo(PaneDesc);
            }
#else

            ;
#endif

            CancelEventArgs e = new CancelEventArgs();
            this.OnCurrentCellDeleting(e);
            return !e.Cancel;
        }

        internal bool RaiseCurrentCellChanging()
        {
            if (CurrentCell.IsSuspendEvents || IsDisposed)
            {
                return true;
            }
#if DEBUG

            if (Switches.CurrentCell.TraceVerbose)
            {
                TraceUtil.TraceCurrentMethodInfo(PaneDesc);
            }
#else

            ;
#endif

            CancelEventArgs e = new CancelEventArgs();
            this.OnCurrentCellChanging(e);
            return !e.Cancel;
        }

        internal void RaiseCurrentCellChanged()
        {
            if (CurrentCell.IsSuspendEvents || IsDisposed)
            {
                return;
            }
#if DEBUG
            if (Switches.CurrentCell.TraceVerbose)
            {
                TraceUtil.TraceCurrentMethodInfo(PaneDesc);
            }
#else

            ;
#endif

            this.OnCurrentCellChanged(EventArgs.Empty);
        }

        internal bool RaiseCurrentCellDeactivating()
        {
            if (CurrentCell.IsSuspendEvents || IsDisposed)
            {
                return true;
            }
#if DEBUG

            if (Switches.CurrentCell.TraceVerbose)
            {
                TraceUtil.TraceCurrentMethodInfo(PaneDesc);
            }
#else

            ;
#endif
            CancelEventArgs e = new CancelEventArgs();
            this.OnCurrentCellDeactivating(e);
            return !e.Cancel;
        }

        internal void RaiseCurrentCellDeactivateFailed()
        {
            if (CurrentCell.IsSuspendEvents || IsDisposed)
            {
                return;
            }
#if DEBUG

            if (Switches.CurrentCell.TraceVerbose)
            {
                TraceUtil.TraceCurrentMethodInfo(PaneDesc);
            }
#else

            ;
#endif

            this.OnCurrentCellDeactivateFailed(EventArgs.Empty);
        }

        internal bool RaiseCurrentCellValidating()
        {
            if (CurrentCell.IsSuspendEvents || IsDisposed)
            {
                return true;
            }
#if DEBUG

            if (Switches.CurrentCell.TraceVerbose)
            {
                TraceUtil.TraceCurrentMethodInfo(PaneDesc);
            }
#else

            ;
#endif

            CancelEventArgs e = new CancelEventArgs();
            this.OnCurrentCellValidating(e);
            return !e.Cancel;
        }

        /// <summary>
        /// Initiates call to <see cref="OnCurrentCellValidated"/>.
        /// </summary>
        public void RaiseCurrentCellValidated()
        {
            if (CurrentCell.IsSuspendEvents || IsDisposed)
            {
                return;
            }
#if DEBUG

            if (Switches.CurrentCell.TraceVerbose)
            {
                TraceUtil.TraceCurrentMethodInfo(PaneDesc);
            }
#else

            ;
#endif

            this.OnCurrentCellValidated(EventArgs.Empty);
        }

        /// <summary>
        /// Initiates call to <see cref="OnCurrentCellInitializeControlText"/>.
        /// </summary>
        /// <param name="e">The event arguments.</param>
        /// <returns>True if the method call is initiated.</returns>
        public bool RaiseCurrentCellInitializeControlText(GridCurrentCellInitializeControlTextEventArgs e)
        {
            if (CurrentCell.IsSuspendEvents || IsDisposed)
            {
                return true;
            }
#if DEBUG

            if (Switches.CurrentCell.TraceVerbose)
            {
                TraceUtil.TraceCurrentMethodInfo(PaneDesc);
            }
#else

            ;
#endif
            this.OnCurrentCellInitializeControlText(e);
            return !e.Cancel;
        }

        /// <summary>
        /// The CurrentCellInitializeControlText notifies you
        /// that the current cell is initialized with text to be displayed in
        /// the associated control, e.g. a text box control.
        /// </summary>
        /// <example>
        /// <code lang="C#">
        ///         public Form1()
        ///         {
        ///             InitializeComponent();
        /// <para/>
        ///             this.gridDataBoundGrid2.CurrentCellInitializeControlText += new GridCurrentCellInitializeControlTextEventHandler(gridDataBoundGrid2_CurrentCellInitializeControlText);
        ///             this.sqlDataAdapter1.Fill(this.dataSet11);
        ///         }
        /// <para/>
        ///         void gridDataBoundGrid2_CurrentCellInitializeControlText(object sender, GridCurrentCellInitializeControlTextEventArgs e)
        ///         {
        ///             if (e.CellValue != null)
        ///             {
        ///                 e.ControlText = e.CellValue.ToString();
        ///             }
        ///         }
        /// </code>
        /// </example>
        [Category("Behavior")]
        [Description("Occurs when the current cell is initialized with text to be displayed in the associated control.")]
        public event GridCurrentCellInitializeControlTextEventHandler CurrentCellInitializeControlText;

        /// <summary>
        /// Raises the <see cref="CurrentCellInitializeControlText"/> event.
        /// </summary>
        /// <param name="e">A <see cref="GridCurrentCellInitializeControlTextEventArgs" /> that contains the event data.</param>
        protected virtual void OnCurrentCellInitializeControlText(GridCurrentCellInitializeControlTextEventArgs e)
        {
            if (this.eventsTarget != null)
            {
                this.eventsTarget.OnCurrentCellInitializeControlText(e);
            }
#if DEBUG

            if (Switches.GridControlBaseEvents.TraceVerbose)
            {
                TraceUtil.TraceCurrentMethodInfo(PaneDesc, e);
            }
#else

            ;
#endif

            if (CurrentCellInitializeControlText != null)
            {
                CurrentCellInitializeControlText(this, e);
            }
        }

        /// <summary>
        /// Initiates call to <see cref="OnCurrentCellErrorMessage"/>.
        /// </summary>
        /// <param name="e">The event arguments.</param>
        /// <returns>True if the method call is initiated.</returns>
        public bool RaiseCurrentCellErrorMessage(GridCurrentCellErrorMessageEventArgs e)
        {
            if (CurrentCell.IsSuspendEvents || IsDisposed)
            {
                return true;
            }
#if DEBUG
            if (Switches.CurrentCell.TraceVerbose)
            {
                TraceUtil.TraceCurrentMethodInfo(PaneDesc);
            }
#else

            ;
#endif
            this.OnCurrentCellErrorMessage(e);
            return !e.Cancel;
        }

        /// <summary>
        /// The CurrentCellErrorMessage notifies you
        /// that the current cell validation failed and a message is displayed. You can cancel
        /// the event and display your own custom messagebox.
        /// </summary>
        /// <example>
        /// <code lang="C#">
        ///         public Form1()
        ///         {
        ///             InitializeComponent();
        /// <para/>
        ///             this.gridDataBoundGrid2.CurrentCellErrorMessage += new GridCurrentCellErrorMessageEventHandler(gridDataBoundGrid2_CurrentCellErrorMessage);
        ///             this.sqlDataAdapter1.Fill(this.dataSet11);
        ///         }
        /// <para/>
        ///         void gridDataBoundGrid2_CurrentCellErrorMessage(object sender, GridCurrentCellErrorMessageEventArgs e)
        ///         {
        ///             MessageBox.Show(e.Owner, e.Text);
        ///             e.Cancel = true;
        ///         }
        /// </code>
        /// </example>
        [Category("Behavior")]
        [Description("Occurs when the current cell validation fails.")]
        public event GridCurrentCellErrorMessageEventHandler CurrentCellErrorMessage;

        /// <summary>
        /// Raises the <see cref="CurrentCellErrorMessage"/> event.
        /// </summary>
        /// <param name="e">A <see cref="GridCurrentCellErrorMessageEventArgs" /> that contains the event data.</param>
        protected virtual void OnCurrentCellErrorMessage(GridCurrentCellErrorMessageEventArgs e)
        {
            if (this.eventsTarget != null)
            {
                this.eventsTarget.OnCurrentCellErrorMessage(e);
            }
#if DEBUG

            if (Switches.GridControlBaseEvents.TraceVerbose)
            {
                TraceUtil.TraceCurrentMethodInfo(PaneDesc, e);
            }
#else

            ;
#endif
            if (CurrentCellErrorMessage != null)
            {
                CurrentCellErrorMessage(this, e);
            }
        }

        internal bool RaiseCurrentCellAcceptedChanges()
        {
            if (CurrentCell.IsSuspendEvents || IsDisposed)
            {
                return true;
            }
#if DEBUG

            if (Switches.CurrentCell.TraceVerbose)
            {
                TraceUtil.TraceCurrentMethodInfo(PaneDesc);
            }
#else

            ;
#endif

            CancelEventArgs e = new CancelEventArgs();
            this.OnCurrentCellAcceptedChanges(e);
            return !e.Cancel;
        }

        internal void RaiseCurrentCellConfirmChangesFailed()
        {
            if (CurrentCell.IsSuspendEvents || IsDisposed)
            {
                return;
            }
#if DEBUG

            if (Switches.CurrentCell.TraceVerbose)
            {
                TraceUtil.TraceCurrentMethodInfo(PaneDesc);
            }
#else

            ;
#endif

            this.OnCurrentCellConfirmChangesFailed(EventArgs.Empty);
        }

        internal void RaiseCurrentCellRejectedChanges()
        {
            if (CurrentCell.IsSuspendEvents || IsDisposed)
            {
                return;
            }
#if DEBUG

            if (Switches.CurrentCell.TraceVerbose)
            {
                TraceUtil.TraceCurrentMethodInfo(PaneDesc);
            }
#else

            ;
#endif

            this.OnCurrentCellRejectedChanges(EventArgs.Empty);
        }

        internal void RaiseCurrentCellEditingComplete()
        {
            if (CurrentCell.IsSuspendEvents || IsDisposed)
            {
                return;
            }
#if DEBUG

            if (Switches.CurrentCell.TraceVerbose)
            {
                TraceUtil.TraceCurrentMethodInfo(PaneDesc);
            }
#else

            ;
#endif

            this.OnCurrentCellEditingComplete(EventArgs.Empty);
        }

        internal bool RaiseCurrentCellStartEditing()
        {
            if (CurrentCell.IsSuspendEvents || IsDisposed)
            {
                return true;
            }
#if DEBUG

            if (Switches.CurrentCell.TraceVerbose)
            {
                TraceUtil.TraceCurrentMethodInfo(PaneDesc);
            }
#else

            ;
#endif
            if (this.Model.BrowseOnly)
                return false;

            CancelEventArgs e = new CancelEventArgs();
            this.OnCurrentCellStartEditing(e);
            return !e.Cancel;
        }

        internal void RaiseCurrentCellDeactivated(int rowIndex, int colIndex)
        {
            if (CurrentCell.IsSuspendEvents || IsDisposed)
            {
                return;
            }
#if DEBUG

            if (Switches.CurrentCell.TraceVerbose)
            {
                TraceUtil.TraceCurrentMethodInfo(PaneDesc);
            }
#else

            ;
#endif
            GridCurrentCellDeactivatedEventArgs e = new GridCurrentCellDeactivatedEventArgs(rowIndex, colIndex);
            this.OnCurrentCellDeactivated(e);
        }

        internal void RaiseCurrentCellControlDoubleClick(Control control)
        {
            if (CurrentCell.IsSuspendEvents || IsDisposed)
            {
                return;
            }
#if DEBUG

            if (Switches.CurrentCell.TraceVerbose)
            {
                TraceUtil.TraceCurrentMethodInfo(PaneDesc, control);
            }
#else

            ;
#endif

            ControlEventArgs e = new ControlEventArgs(control);
            this.OnCurrentCellControlDoubleClick(e);
        }

        internal void RaiseCurrentCellControlGotFocus(Control control)
        {
            if (CurrentCell.IsSuspendEvents || IsDisposed)
            {
                return;
            }
#if DEBUG

            if (Switches.CurrentCell.TraceVerbose)
            {
                TraceUtil.TraceCurrentMethodInfo(PaneDesc, control);
            }
#else

            ;
#endif

            ControlEventArgs e = new ControlEventArgs(control);
            this.OnCurrentCellControlGotFocus(e);
        }

        internal void RaiseCurrentCellControlLostFocus(Control control)
        {
            if (CurrentCell.IsSuspendEvents || IsDisposed)
            {
                return;
            }
#if DEBUG

            if (Switches.CurrentCell.TraceVerbose)
            {
                TraceUtil.TraceCurrentMethodInfo(PaneDesc);
            }
#else

            ;
#endif

            ControlEventArgs e = new ControlEventArgs(control);
            this.OnCurrentCellControlLostFocus(e);
        }

        /// <summary>
        /// Initiates call to <see cref="OnCellButtonClicked"/>.
        /// </summary>
        /// <param name="rowIndex">The row index.</param>
        /// <param name="colIndex">The column index.</param>
        /// <param name="index">The button index.</param>
        /// <param name="button">The button.</param>
        /// <returns>True if operation should continue; False if it should be canceled.</returns>
        public bool RaiseCellButtonClicked(int rowIndex, int colIndex, int index, GridCellButton button)
        {
#if DEBUG
            if (Switches.CurrentCell.TraceVerbose)
            {
                TraceUtil.TraceCurrentMethodInfo(PaneDesc, rowIndex, colIndex, index, button);
            }
#else
            ;
#endif

            GridCellButtonClickedEventArgs e = new GridCellButtonClickedEventArgs(rowIndex, colIndex, index, button);
            try
            {
                OnCellButtonClicked(e);
            }
            catch (Exception ex)
            {
                TraceUtil.TraceExceptionCatched(ex);
                if (!ExceptionManager.RaiseExceptionCatched(this, ex))
                {
                    throw;
                }

                e.Cancel = true;
            }

            return !e.Cancel;
        }
        
        /// <summary>
        /// Occurs when the user has clicked on a child button element inside a cell renderer.
        /// </summary>
        [Description("Occurs when the user has clicked on a child button element inside a cell renderer."),
        Category("Behavior")]
        public event GridCellButtonClickedEventHandler CellButtonClicked;

        /// <summary>
        /// Raises the <see cref="GridControlBase.CellButtonClicked"/> event.
        /// </summary>
        /// <param name="e">A <see cref="GridCellButtonClickedEventArgs" /> that contains the event data.</param>
        protected virtual void OnCellButtonClicked(GridCellButtonClickedEventArgs e)
        {
            if (this.eventsTarget != null)
            {
                this.eventsTarget.OnCellButtonClicked(e);
            }
#if DEBUG

            if (Switches.GridControlBaseEvents.TraceVerbose)
            {
                TraceUtil.TraceCurrentMethodInfo(PaneDesc, e);
            }
#else

            ;
#endif

            if (CellButtonClicked != null)
            {
                CellButtonClicked(this, e);
            }
        }

        /// <summary>
        /// Occurs when the current cell is about to be moved to a new position.
        /// </summary>
        /// <remarks>
        /// You can disallow the activation of specific cells at run-time when
        /// you assign True to <see cref="CancelEventArgs.Cancel"/>.
        /// <para/>
        /// You can modify the <see cref="GridCurrentCellActivatingEventArgs.RowIndex"/>
        /// and <see cref="GridCurrentCellActivatingEventArgs.ColIndex"/> to activate
        /// a different cell.
        /// <para/>
        /// You can also modify the <see cref="GridCurrentCellActivatingEventArgs.Options"/>.
        /// <para/>
        /// Once the current cell has been moved, a <see cref="GridControlBase.CurrentCellMoved"/> event
        /// is raised or a <see cref="GridControlBase.CurrentCellMoveFailed"/> if moving to the specified
        /// target cell failed.
        /// <para/>
        /// See <see cref="GridCurrentCell.MoveTo(int,int,Syncfusion.Windows.Forms.Grid.GridSetCurrentCellOptions,bool)"/> for a discussion about the
        /// order of events that you receive when the current cell is moved.
        /// </remarks>
        /// <seealso cref="GridCurrentCellMovingEventArgs"/>
        /// <seealso cref="GridCurrentCell.MoveTo(int,int,Syncfusion.Windows.Forms.Grid.GridSetCurrentCellOptions,bool)"/>
        [Description("Occurs when the current cell is about to be moved to a new position."),
        Category("Behavior")]
        public event GridCurrentCellMovingEventHandler CurrentCellMoving;

        /// <summary>
        /// Occurs when the current cell has been successfully moved to a new position.
        /// </summary>
        /// <remarks>
        /// See <see cref="GridCurrentCell.MoveTo(int,int,Syncfusion.Windows.Forms.Grid.GridSetCurrentCellOptions,bool)"/> for a discussion about the
        /// order of events that you receive when the current cell is moved.
        /// </remarks>
        /// <seealso cref="GridCurrentCellMovedEventArgs"/>
        /// <seealso cref="GridCurrentCell.MoveTo(int,int,Syncfusion.Windows.Forms.Grid.GridSetCurrentCellOptions,bool)"/>
        [Description("Occurs when the current cell has been successfully moved to a new position."),
        Category("Behavior")]
        public event GridCurrentCellMovedEventHandler CurrentCellMoved;

        /// <summary>
        /// Occurs when the current cell fails to be moved to a new position.
        /// </summary>
        /// <remarks>
        /// See <see cref="GridCurrentCell.MoveTo(int,int,Syncfusion.Windows.Forms.Grid.GridSetCurrentCellOptions,bool)"/> for a discussion about the
        /// order of events that you receive when the current cell is moved.
        /// <para/>
        /// <see cref="GridCurrentCell.ErrorMessage"/> might hold an error message
        /// why the operation failed.
        /// </remarks>
        /// <seealso cref="GridCurrentCellMoveFailedEventArgs"/>
        /// <seealso cref="GridCurrentCell.MoveTo(int,int,Syncfusion.Windows.Forms.Grid.GridSetCurrentCellOptions,bool)"/>
        [Description("Occurs when the current cell fails to be moved to a new position."),
        Category("Behavior")]
        public event GridCurrentCellMoveFailedEventHandler CurrentCellMoveFailed;

        /// <summary>
        /// Occurs before the grid activates the specified cell as current cell.
        /// </summary>
        /// <remarks>
        /// You can disallow the activation of specific cells at run-time when
        /// you assign True to <see cref="CancelEventArgs.Cancel"/>.<para/>
        /// You can modify the <see cref="GridCurrentCellActivatingEventArgs.RowIndex"/>
        /// and <see cref="GridCurrentCellActivatingEventArgs.ColIndex"/> to activate
        /// a different cell.
        /// <para/>
        /// You can determine if <see cref="GridCurrentCell.Activate(int, int)"/>
        /// was called stand-alone or as result of a <see cref="GridCurrentCell.MoveTo(int,int,Syncfusion.Windows.Forms.Grid.GridSetCurrentCellOptions,bool)"/>
        /// call by checking the <see cref="GridCurrentCell.IsInMoveTo"/> property.
        /// <para/>
        /// Once the current cell has been activated, a <see cref="GridControlBase.CurrentCellActivated"/> event
        /// is raised or a <see cref="GridControlBase.CurrentCellActivateFailed"/> if activating the specified
        /// cell failed.
        /// <para/>
        /// See <see cref="GridCurrentCell.MoveTo(int,int,Syncfusion.Windows.Forms.Grid.GridSetCurrentCellOptions,bool)"/> for a discussion about the
        /// order of events that you receive when the current cell is moved.
        /// </remarks>
        /// <seealso cref="GridCurrentCellActivatingEventArgs"/>
        /// <seealso cref="GridCurrentCell.Activate(int, int)"/>
        /// <seealso cref="GridCurrentCell.IsInMoveTo"/>
        [Description("Occurs before the grid activates the specified cell as current cell."),
        Category("Behavior")]
        public event GridCurrentCellActivatingEventHandler CurrentCellActivating;

        /// <summary>
        /// Occurs after the grid activates the specified cell as current cell.
        /// </summary>
        /// <remarks>
        /// You can determine if <see cref="GridCurrentCell.Activate(int, int)"/>
        /// was called stand-alone or as result of a <see cref="GridCurrentCell.MoveTo(int,int,Syncfusion.Windows.Forms.Grid.GridSetCurrentCellOptions,bool)"/>
        /// call by checking the <see cref="GridCurrentCell.IsInMoveTo"/> property.
        /// <para/>
        /// See <see cref="GridCurrentCell.MoveTo(int,int,Syncfusion.Windows.Forms.Grid.GridSetCurrentCellOptions,bool)"/> for a discussion about the
        /// order of events that you receive when the current cell is moved.
        /// <para/>
        /// You can find out about the current cell's position by querying the <see cref="GridCurrentCell.RowIndex"/>
        /// and <see cref="GridCurrentCell.ColIndex"/> properties of the <see cref="GridControlBase.CurrentCell"/> object
        /// in <see cref="GridControlBase"/>.
        /// </remarks>
        /// <seealso cref="GridCurrentCellActivatingEventArgs"/>
        /// <seealso cref="GridCurrentCell.Activate(int, int)"/>
        /// <seealso cref="GridCurrentCell.IsInMoveTo"/>
        [Description("Occurs after the grid activates the specified cell as current cell."),
        Category("Behavior")]
        public event EventHandler CurrentCellActivated;

        /// <summary>
        /// Occurs after the grid fails to activate a specific cell as current cell.
        /// </summary>
        /// <remarks>
        /// You can determine if <see cref="GridCurrentCell.Activate(int, int)"/>
        /// was called stand-alone or as result of a <see cref="GridCurrentCell.MoveTo(int,int,Syncfusion.Windows.Forms.Grid.GridSetCurrentCellOptions,bool)"/>
        /// call by checking the <see cref="GridCurrentCell.IsInMoveTo"/> property.
        /// <para/>
        /// See <see cref="GridCurrentCell.MoveTo(int,int,Syncfusion.Windows.Forms.Grid.GridSetCurrentCellOptions,bool)"/> for a discussion about the
        /// order of events that you receive when the current cell is moved.
        /// </remarks>
        /// <seealso cref="GridCurrentCellActivateFailedEventArgs"/>
        /// <seealso cref="GridCurrentCell.Activate(int, int)"/>
        /// <seealso cref="GridCurrentCell.IsInMoveTo"/>
        [Description("Occurs after the grid fails to activate a specific cell as current cell."),
        Category("Behavior")]
        public event GridCurrentCellActivateFailedEventHandler CurrentCellActivateFailed;

        /// <summary>
        /// Occurs when the user presses the Delete key on an active current cell.
        /// </summary>
        /// <remarks>
        /// The grid will delete contents of the current cell. You can cancel the operation
        /// by setting <see cref="CancelEventArgs.Cancel"/> to True.
        /// <para/>
        /// You can find out about the current cell's position by querying the <see cref="GridCurrentCell.RowIndex"/>
        /// and <see cref="GridCurrentCell.ColIndex"/> properties of the <see cref="GridControlBase.CurrentCell"/> object
        /// in <see cref="GridControlBase"/>.
        /// <para/>
        /// See <see cref="GridCurrentCell.MoveTo(int,int,Syncfusion.Windows.Forms.Grid.GridSetCurrentCellOptions,bool)"/> for a discussion about the
        /// order of events that you receive when the current cell is moved.
        /// </remarks>
        [Description("Occurs when the user presses the Delete key on an active current cell."),
        Category("Behavior")]
        public event CancelEventHandler CurrentCellDeleting;

        /// <summary>
        /// Occurs before the current cell switches into editing mode.
        /// </summary>
        /// <remarks>
        /// The grid will switch into editing mode when the user presses a key while the cell
        /// is not in editing mode or when you call <see cref="GridCurrentCell.BeginEdit()"/>.
        /// You can cancel the operation
        /// by setting <see cref="CancelEventArgs.Cancel"/> to True.
        /// <para/>
        /// You can find out about the current cell's position by querying the <see cref="GridCurrentCell.RowIndex"/>
        /// and <see cref="GridCurrentCell.ColIndex"/> properties of the <see cref="GridControlBase.CurrentCell"/> object
        /// in <see cref="GridControlBase"/>.
        /// <para/>
        /// See <see cref="GridCurrentCell.MoveTo(int,int,Syncfusion.Windows.Forms.Grid.GridSetCurrentCellOptions,bool)"/> for a discussion about the
        /// order of events that you receive when the current cell is moved.
        /// </remarks>
        [Description("Occurs before the current cell switches into editing mode."),
        Category("Behavior")]
        public event CancelEventHandler CurrentCellStartEditing;

        /// <summary>
        /// Occurs when the user wants to modify contents of the current cell.
        /// </summary>
        /// <remarks>
        /// The grid sends this event before the changes are applied to the active cell. You can cancel the operation
        /// by setting <see cref="CancelEventArgs.Cancel"/> to True.
        /// <para/>
        /// You can find out about the current cell's position by querying the <see cref="GridCurrentCell.RowIndex"/>
        /// and <see cref="GridCurrentCell.ColIndex"/> properties of the <see cref="GridControlBase.CurrentCell"/> object
        /// in <see cref="GridControlBase"/>.
        /// <para/>
        /// See <see cref="GridCurrentCell.MoveTo(int,int,Syncfusion.Windows.Forms.Grid.GridSetCurrentCellOptions,bool)"/> for a discussion about the
        /// order of events that you receive when the current cell is moved.
        /// </remarks>
        [Description("Occurs when the user wants to modify contents of the current cell."),
        Category("Behavior")]
        public event CancelEventHandler CurrentCellChanging;

        /// <summary>
        /// Occurs when the user changes contents of the current cell.
        /// </summary>
        /// <remarks>
        /// The grid sends this event whenever changes occur, similar to a <see cref="TextBoxBase.ModifiedChanged"/> event.
        /// <para/>
        /// You can find out about the current cell's position by querying the <see cref="GridCurrentCell.RowIndex"/>
        /// and <see cref="GridCurrentCell.ColIndex"/> properties of the <see cref="GridControlBase.CurrentCell"/> object
        /// in <see cref="GridControlBase"/>
        /// <para/>
        /// See <see cref="GridCurrentCell.MoveTo(int,int,Syncfusion.Windows.Forms.Grid.GridSetCurrentCellOptions,bool)"/> for a discussion about the
        /// order of events that you receive when the current cell is moved.
        /// </remarks>
        [Description("Occurs when the user changes contents of the current cell."),
        Category("Behavior")]
        public event EventHandler CurrentCellChanged;

        /// <summary>
        /// Occurs before the grid the deactivates the current cell.
        /// </summary>
        /// <remarks>
        /// You can cancel the operation
        /// by setting <see cref="CancelEventArgs.Cancel"/> to True.
        /// <para/>
        /// You can determine if <see cref="GridCurrentCell.Deactivate"/>
        /// was called stand-alone or as result of a <see cref="GridCurrentCell.MoveTo(int,int,Syncfusion.Windows.Forms.Grid.GridSetCurrentCellOptions,bool)"/>
        /// call by checking the <see cref="GridCurrentCell.IsInMoveTo"/> property.
        /// <para/>
        /// You can find out about the current cell's position by querying the <see cref="GridCurrentCell.RowIndex"/>
        /// and <see cref="GridCurrentCell.ColIndex"/> properties of the <see cref="GridControlBase.CurrentCell"/> object
        /// in <see cref="GridControlBase"/>.
        /// <para/>
        /// See <see cref="GridCurrentCell.MoveTo(int,int,Syncfusion.Windows.Forms.Grid.GridSetCurrentCellOptions,bool)"/> for a discussion about the
        /// order of events that you receive when the current cell is moved.
        /// </remarks>
        /// <seealso cref="GridCurrentCell.Deactivate"/>
        /// <seealso cref="GridCurrentCell.IsInMoveTo"/>
        [Description("Occurs before the grid the deactivates the current cell."),
        Category("Behavior")]
        public event CancelEventHandler CurrentCellDeactivating;

        /// <summary>
        /// Occurs after the drop-down part has been dropped-down and made visible.
        /// </summary>
        [Description("Occurs after the drop-down part has been dropped-down and made visible."),
        Category("Behavior")]
        public event EventHandler CurrentCellShowedDropDown;

        /// <summary>
        /// Raises the <see cref="CurrentCellShowedDropDown"/> event.
        /// </summary>
        /// <param name="e">An <see cref="EventArgs" /> that contains the event data.</param>
        protected virtual void OnCurrentCellShowedDropDown(EventArgs e)
        {
            if (this.eventsTarget != null)
            {
                this.eventsTarget.OnCurrentCellShowedDropDown(e);
            }

            if (CurrentCellShowedDropDown != null)
            {
                CurrentCellShowedDropDown(this, e);
            }
        }

        /// <summary>
        /// Initiates call to <see cref="OnCurrentCellShowedDropDown"/>.
        /// </summary>
        /// <param name="e">The event arguments.</param>
        public void RaiseCurrentCellShowedDropDown(EventArgs e)
        {
            OnCurrentCellShowedDropDown(e);
        }

        /// <summary>
        /// Occurs when the drop-down part of the current cell was / is closed.
        /// </summary>
        [Description("Occurs when the drop-down part of the current cell was / is closed."),
        Category("Behavior")]
        public event PopupClosedEventHandler CurrentCellCloseDropDown;

        /// <summary>
        /// Raises the <see cref="CurrentCellCloseDropDown"/> event.
        /// </summary>
        /// <param name="e">A <see cref="PopupClosedEventArgs" /> that contains the event data.</param>
        protected virtual void OnCurrentCellCloseDropDown(PopupClosedEventArgs e)
        {
            if (this.eventsTarget != null)
            {
                this.eventsTarget.OnCurrentCellCloseDropDown(e);
            }

            if (CurrentCellCloseDropDown != null)
            {
                CurrentCellCloseDropDown(this, e);
            }
        }

        /// <summary>
        /// Initiates call to <see cref="OnCurrentCellCloseDropDown"/>.
        /// </summary>
        /// <param name="e">The event arguments.</param>
        public void RaiseCurrentCellCloseDropDown(PopupClosedEventArgs e)
        {
            OnCurrentCellCloseDropDown(e);
        }

        /// <summary>
        /// Occurs when the drop-down part is about to be shown.
        /// </summary>
        /// <remarks>
        /// The event will provide you with a suggested size of the drop-down control. You can change
        /// the default size in your event handler by changing the <see cref="GridCurrentCellShowingDropDownEventArgs.Size"/>
        /// property.
        /// <para/>
        /// Please note however that some drop-down controls might override the suggested height with their own
        /// preferred height. The <see cref="GridDropDownGridListControlPart"/> and <see cref="GridComboBoxListBoxPart"/>
        /// methods both provide a <see cref="GridComboBoxListBoxPart.DropDownRows"/> property that defines the
        /// number of visible rows.
        /// <para/>
        /// To abort the drop-down operation, you should set <see cref="CancelEventArgs.Cancel"/> to True.
        /// <para/>
        /// If you need to get access to the cell renderer, you can use the <see cref="GridCurrentCell.Renderer"/>
        /// property of the <see cref="GridControlBase.CurrentCell"/> object. The <see cref="GridControlBase.CurrentCell"/> object
        /// also holds style information and row and column index. See the cell renderer for properties to access
        /// the drop-down container and drop-down part.
        /// </remarks>
        [Description("Occurs when the drop-down part is about to be shown."),
        Category("Behavior")]
        public event GridCurrentCellShowingDropDownEventHandler CurrentCellShowingDropDown;

        /// <summary>
        /// Raises the <see cref="CurrentCellShowingDropDown"/> event.
        /// </summary>
        /// <param name="e">A <see cref="GridCurrentCellShowingDropDownEventArgs" /> that contains the event data.</param>
        protected virtual void OnCurrentCellShowingDropDown(GridCurrentCellShowingDropDownEventArgs e)
        {
            if (this.eventsTarget != null)
            {
                this.eventsTarget.OnCurrentCellShowingDropDown(e);
            }

            if (CurrentCellShowingDropDown != null)
            {
                CurrentCellShowingDropDown(this, e);
            }
        }

        /// <summary>
        /// Initiates call to <see cref="OnCurrentCellShowingDropDown"/>.
        /// </summary>
        /// <param name="e">The event arguments.</param>
        public void RaiseCurrentCellShowingDropDown(GridCurrentCellShowingDropDownEventArgs e)
        {
            OnCurrentCellShowingDropDown(e);
        }

        /// <summary>
        /// Occurs when the grid validates contents of the active current cell.
        /// </summary>
        /// <remarks>
        /// You can mark the contents as invalid by by setting <see cref="CancelEventArgs.Cancel"/> to True.<para/>
        /// The grid raises this event when the <see cref="GridControlBase.CurrentCell"/> object's <see cref="GridCurrentCell.Validate"/>
        /// method is called. <see cref="GridCurrentCell.Deactivate"/> calls this method when the current cell was in editing mode
        /// and its contents were modified.
        /// <para/>
        /// You can determine if <see cref="GridCurrentCell.Deactivate"/>
        /// was called stand-alone or as result of a <see cref="GridCurrentCell.MoveTo(int,int,Syncfusion.Windows.Forms.Grid.GridSetCurrentCellOptions,bool)"/>
        /// call by checking the <see cref="GridCurrentCell.IsInMoveTo"/> property.
        /// <para/>
        /// You can find out about the current cell's position by querying the <see cref="GridCurrentCell.RowIndex"/>
        /// and <see cref="GridCurrentCell.ColIndex"/> properties of the <see cref="GridControlBase.CurrentCell"/> object
        /// in <see cref="GridControlBase"/>.
        /// <para/>
        /// See <see cref="GridCurrentCell.MoveTo(int,int,Syncfusion.Windows.Forms.Grid.GridSetCurrentCellOptions,bool)"/> for a discussion about the
        /// order of events that you receive when the current cell is moved.
        /// </remarks>
        [Description("Occurs when the grid validates contents of the active current cell."),
        Category("Behavior")]
        public event CancelEventHandler CurrentCellValidating;

        /// <summary>
        /// Occurs when the grid has successfully validated the contents of the active current cell.
        /// </summary>
        /// <remarks>
        /// You can determine if <see cref="GridCurrentCell.Deactivate"/>
        /// was called stand-alone or as result of a <see cref="GridCurrentCell.MoveTo(int,int,Syncfusion.Windows.Forms.Grid.GridSetCurrentCellOptions,bool)"/>
        /// call by checking the <see cref="GridCurrentCell.IsInMoveTo"/> property.
        /// <para/>
        /// You can find out about the current cell's position by querying the <see cref="GridCurrentCell.RowIndex"/>
        /// and <see cref="GridCurrentCell.ColIndex"/> properties of the <see cref="GridControlBase.CurrentCell"/> object
        /// in <see cref="GridControlBase"/>.
        /// <para/>
        /// See <see cref="GridCurrentCell.MoveTo(int,int,Syncfusion.Windows.Forms.Grid.GridSetCurrentCellOptions,bool)"/> for a discussion about the
        /// order of events that you receive when the current cell is moved.
        /// </remarks>
        [Description("Occurs when the grid validates contents of the active current cell."),
        Category("Behavior")]
        public event EventHandler CurrentCellValidated;

        /// <summary>
        /// Occurs when the grid accepts changes made to the active current cell.
        /// </summary>
        /// <remarks>
        /// The grid raises this cancelable event when the <see cref="GridControlBase.CurrentCell"/> object's <see cref="GridCurrentCell.ConfirmChanges()"/>
        /// method is called. <see cref="GridCurrentCell.Deactivate"/> and <see cref="GridCurrentCell.EndEdit"/> call this method when the current cell was in editing mode
        /// and its contents were modified and validated.
        /// <para/>
        /// You can determine if <see cref="GridCurrentCell.Deactivate"/>
        /// was called stand-alone or as result of a <see cref="GridCurrentCell.MoveTo(int,int,Syncfusion.Windows.Forms.Grid.GridSetCurrentCellOptions,bool)"/>
        /// call by checking the <see cref="GridCurrentCell.IsInMoveTo"/> property.
        /// <para/>
        /// If you assign true to <see cref="CancelEventArgs.Cancel"/>, the grid will not deactivate the current
        /// cell.
        /// <para/>
        /// You can find out about the current cell's position by querying the <see cref="GridCurrentCell.RowIndex"/>
        /// and <see cref="GridCurrentCell.ColIndex"/> properties of the <see cref="GridControlBase.CurrentCell"/> object
        /// in <see cref="GridControlBase"/>.
        /// <para/>
        /// See <see cref="GridCurrentCell.MoveTo(int,int,Syncfusion.Windows.Forms.Grid.GridSetCurrentCellOptions,bool)"/> for a discussion about the
        /// order of events that you receive when the current cell is moved.
        /// <para/>
        /// </remarks>
        [Description("Occurs when the grid accepted changes made to the active current cell."),
        Category("Behavior")]
        public event CancelEventHandler CurrentCellAcceptedChanges;

        /// <summary>
        /// Occurs when the grid could not save changes made to the active current cell.
        /// </summary>
        /// <remarks>
        /// The grid raises this event when the <see cref="GridControlBase.CurrentCell"/> object's <see cref="GridCurrentCell.ConfirmChanges()"/>
        /// method is called and its contents were modified and could not be successfully validated
        /// or saved back to the data source.
        /// <para/>
        /// The <see cref="GridCurrentCell.Exception"/> and <see cref="GridCurrentCell.ErrorMessage"/>
        /// properties provide details why the operation failed. If you want to display a message box
        /// be sure to reset the the error state with <see cref="GridCurrentCell.ResetError"/>.
        /// <para/>
        /// You can determine if <see cref="GridCurrentCell.Deactivate"/>
        /// was called stand-alone or as result of a <see cref="GridCurrentCell.MoveTo(int,int,Syncfusion.Windows.Forms.Grid.GridSetCurrentCellOptions,bool)"/>
        /// call by checking the <see cref="GridCurrentCell.IsInMoveTo"/> property.
        /// <para/>
        /// You can find out about the current cell's position by querying the <see cref="GridCurrentCell.RowIndex"/>
        /// and <see cref="GridCurrentCell.ColIndex"/> properties of the <see cref="GridControlBase.CurrentCell"/> object
        /// in <see cref="GridControlBase"/>
        /// <para/>
        /// See <see cref="GridCurrentCell.MoveTo(int,int,Syncfusion.Windows.Forms.Grid.GridSetCurrentCellOptions,bool)"/> for a discussion about the
        /// order of events that you receive when the current cell is moved.
        /// <para/>
        /// </remarks>
        [Description("Occurs when the grid accepted changes made to the active current cell."),
        Category("Behavior")]
        public event EventHandler CurrentCellConfirmChangesFailed;

        /// <summary>
        /// Occurs when the grid rejects changes made to the active current cell.
        /// </summary>
        /// <remarks>
        /// The grid raises this event when the <see cref="GridControlBase.CurrentCell"/> object's <see cref="GridCurrentCell.RejectChanges"/>
        /// method is called. <see cref="GridCurrentCell.Deactivate"/> and <see cref="GridCurrentCell.CancelEdit"/> call this method when the current cell was in editing mode
        /// and its contents were modified.
        /// <para/>
        /// You can determine if <see cref="GridCurrentCell.Deactivate"/>
        /// was called stand-alone or as result of a <see cref="GridCurrentCell.MoveTo(int,int,Syncfusion.Windows.Forms.Grid.GridSetCurrentCellOptions,bool)"/>
        /// call by checking the <see cref="GridCurrentCell.IsInMoveTo"/> property.
        /// <para/>
        /// You can find out about the current cell's position by querying the <see cref="GridCurrentCell.RowIndex"/>
        /// and <see cref="GridCurrentCell.ColIndex"/> properties of the <see cref="GridControlBase.CurrentCell"/> object
        /// in <see cref="GridControlBase"/>.
        /// <para/>
        /// See <see cref="GridCurrentCell.MoveTo(int,int,Syncfusion.Windows.Forms.Grid.GridSetCurrentCellOptions,bool)"/> for a discussion about the
        /// order of events that you receive when the current cell is moved.
        /// <para/>
        /// </remarks>
        [Description("Occurs when the grid rejects changes made to the active current cell."),
        Category("Behavior")]
        public event EventHandler CurrentCellRejectedChanges;

        /// <summary>
        /// Occurs when the grid completes editing mode for the active current cell.
        /// </summary>
        /// <remarks>
        /// The grid raises this event when the <see cref="GridControlBase.CurrentCell"/> object's <see cref="GridCurrentCell.EndEdit"/>
        /// or <see cref="GridCurrentCell.CancelEdit"/> method is called. The event occurs after <see cref="GridControlBase.CurrentCellRejectedChanges"/>
        /// or <see cref="GridControlBase.CurrentCellAcceptedChanges"/> were raised.
        /// <para/>
        /// You can determine if <see cref="GridCurrentCell.Deactivate"/>
        /// was called stand-alone or as result of a <see cref="GridCurrentCell.MoveTo(int,int,Syncfusion.Windows.Forms.Grid.GridSetCurrentCellOptions,bool)"/>
        /// call by checking the <see cref="GridCurrentCell.IsInMoveTo"/> property.
        /// <para/>
        /// You can find out about the current cell's position by querying the <see cref="GridCurrentCell.RowIndex"/>
        /// and <see cref="GridCurrentCell.ColIndex"/> properties of the <see cref="GridControlBase.CurrentCell"/> object
        /// in <see cref="GridControlBase"/>.
        /// <para/>
        /// See <see cref="GridCurrentCell.MoveTo(int,int,Syncfusion.Windows.Forms.Grid.GridSetCurrentCellOptions,bool)"/> for a discussion about the
        /// order of events that you receive when the current cell is moved.
        /// </remarks>
        [Description("Occurs when the grid completes editing mode for the active current cell."),
        Category("Behavior")]
        public event EventHandler CurrentCellEditingComplete;

        /// <summary>
        /// Occurs after the grid deactivates current cell.
        /// </summary>
        /// <remarks>
        /// The grid raises this event when the <see cref="GridControlBase.CurrentCell"/> object's <see cref="GridCurrentCell.Deactivate"/>
        /// method is called. The event occurs after any <see cref="GridControlBase.CurrentCellRejectedChanges"/>,
        ///  <see cref="GridControlBase.CurrentCellAcceptedChanges"/>, <see cref="GridControlBase.CurrentCellRejectedChanges"/>, or
        /// <see cref="GridControlBase.CurrentCellAcceptedChanges"/> are raised.
        /// <para/>
        /// You can determine if <see cref="GridCurrentCell.Deactivate"/>
        /// was called stand-alone or as result of a <see cref="GridCurrentCell.MoveTo(int,int,Syncfusion.Windows.Forms.Grid.GridSetCurrentCellOptions,bool)"/>
        /// call by checking the <see cref="GridCurrentCell.IsInMoveTo"/> property.
        /// <para/>
        /// See <see cref="GridCurrentCell.MoveTo(int,int,Syncfusion.Windows.Forms.Grid.GridSetCurrentCellOptions,bool)"/> for a discussion about the
        /// order of events that you receive when the current cell is moved.
        /// </remarks>
        [Description("Occurs after the grid deactivates current cell."),
        Category("Behavior")]
        public event GridCurrentCellDeactivatedEventHandler CurrentCellDeactivated;

        /// <summary>
        /// Occurs after the grid fails to deactivate the current cell.
        /// </summary>
        /// <remarks>
        /// The grid raises this event when the <see cref="GridControlBase.CurrentCell"/> object's <see cref="GridCurrentCell.Deactivate"/>
        /// method is called and can not deactivate the current cell. The reason deactivation may fail could be
        /// that the cell's contents were invalid or any of the event handlers associated with deactivating the current cell
        /// signaled to abort this operation.
        /// <para/>
        /// You can determine if <see cref="GridCurrentCell.Deactivate"/>
        /// was called stand-alone or as result of a <see cref="GridCurrentCell.MoveTo(int,int,Syncfusion.Windows.Forms.Grid.GridSetCurrentCellOptions,bool)"/>
        /// call by checking the <see cref="GridCurrentCell.IsInMoveTo"/> property.
        /// <para/>
        /// See <see cref="GridCurrentCell.MoveTo(int,int,Syncfusion.Windows.Forms.Grid.GridSetCurrentCellOptions,bool)"/> for a discussion about the
        /// order of events that you receive when the current cell is moved.
        /// </remarks>
        [Description("Occurs after the grid fails to deactivate the current cell."),
        Category("Behavior")]
        public event EventHandler CurrentCellDeactivateFailed;

        /// <summary>
        /// Occurs when the current cell has switched to in-place editing and the control
        /// associated with the current cell has received the focus.
        /// </summary>
        /// <remarks>
        /// Check the associated <see cref="GridCurrentCell.Renderer"/> for state of the cell renderer.<para/>
        /// Raising this event is only optional for the cell renderer that manages the active cell.<para/>
        /// A text box will usually send this event when the associated <see cref="TextBox"/> control has received
        /// the focus after the cell was switched into edit mode with <see cref="GridCurrentCell.BeginEdit()"/>. Other cell renderers
        /// may or may not send this event.
        /// </remarks>
        [Description("Occurs when the current cell has switched to in-place editing and the control associated with the current cell has received the focus."),
        Category("Behavior")]
        public event ControlEventHandler CurrentCellControlGotFocus;

        /// <summary>
        /// This is called from the current cell control's ProcessKeyMessage method and gives
        /// you a chance to modify the default behavior of this method. Be aware that this
        /// is a very implementation-specific method and you should only handle this event
        /// if KeyDown, KeyUp, CurrentCellKeyDown, or CurrentCellKeyUp events are
        /// not good enough.
        /// </summary>
        [Browsable(false)]
        public event GridCurrentCellControlKeyMessageEventHandler CurrentCellControlKeyMessage;

        /// <summary>
        /// Raises the <see cref="CurrentCellControlKeyMessage"/> event.
        /// </summary>
        /// <param name="e">A <see cref="GridCurrentCellControlKeyMessageEventArgs" /> that contains the event data.</param>
        protected virtual void OnCurrentCellControlKeyMessage(GridCurrentCellControlKeyMessageEventArgs e)
        {
            if (this.eventsTarget != null)
            {
                this.eventsTarget.OnCurrentCellControlKeyMessage(e);
            }

            if (CurrentCellControlKeyMessage != null)
            {
                CurrentCellControlKeyMessage(this, e);
            }
        }

        /// <summary>
        /// Initiates call to <see cref="OnCurrentCellControlKeyMessage"/>.
        /// </summary>
        /// <param name="e">The event arguments.</param>
        public void RaiseCurrentCellControlKeyMessage(GridCurrentCellControlKeyMessageEventArgs e)
        {
            OnCurrentCellControlKeyMessage(e);
        }

        /// <summary>
        /// Called from the PreProcessMessage method of the control associated with the current cell. Returns true
        /// if the message was handled and the control should not process the message any further.
        /// </summary>
        /// <param name="msg">The message.</param>
        /// <returns>True if the message was handled and should not be processed by the control.</returns>
        public virtual bool NotifyCurrentCellControlPreProcessMessage(ref Message msg)
        {
            if (tabKeyNavigationInPreProcessMessage && HandleTabKeyPreProcessMessage(ref msg))
            {
                return true;
            }

            return false;
        }

        bool HandleTabKeyPreProcessMessage(ref Message msg)
        {
            if (msg.Msg >= 256 && msg.Msg <= 264 && (((int)msg.WParam & 0xff) == 0x9))
            {
                if (((int)msg.WParam & 0xff) == 0x9)
                {
                    bool shiftKeyDown = (Control.ModifierKeys & Keys.Shift) != Keys.None;

                    if (shiftKeyDown)
                    {
                        this.CurrentCell.MoveLeft();
                    }
                    else
                    {
                        this.CurrentCell.MoveRight();
                    }
                }

                return true;
            }

            return false;
        }

        /// <summary>
        /// Called from the WndProc method of the control associated with the current cell. Returns true
        /// if the message was handled and the control should not process the message any further.
        /// </summary>
        /// <param name="msg">The message.</param>
        /// <returns>True if the message was handled and should not be processed by the control.</returns>
        public bool NotifyCurrentCellControlWndProc(ref Message msg)
        {
            if (msg.Msg == WM_GETDLGCODE && wmGetDlgCodeValue != -1)
            {
                msg.Result = (IntPtr)wmGetDlgCodeValue;
                return true;
            }

            return false;
        }

        private int wmGetDlgCodeValue = -1;
        const int WM_GETDLGCODE = 135; //// 0x0087

        /// <summary>
        /// Enables support for MFC and ActiveX Containers and lets you specify the return value 
        /// when the grid or any of its child
        /// controls receive a WM_GETDLGCODE message. When using the grid in MFC
        /// applications or in an ActiveX host this value lets you change the
        /// default behavior of Tab and Arrows keys. In this scenarios it is recommended
        /// to set the value to be DLGC_WANTALLKEYS (= 4). Set this value to -1
        /// if the WM_GETDLGCODE message should not be handled. -1 is the default setting
        /// for this property.
        /// </summary>
        /// <param name="value">The specified return value when the grid receives a WM_GETDLGCODE message.</param>
        public void SetWmGetDlgCodeValue(int value)
        {
            wmGetDlgCodeValue = value;
        }

        /// <summary>
        /// Occurs when the current cell
        /// is in-place editing mode and the user double-clicks inside the control
        /// associated with the current cell.
        /// </summary>
        /// <remarks>
        /// GridCurrentCell.ControlDoubleClick lets you detect a double click inside
        /// a cell for any CurrentCellActivateBehavior. If for example the focus is set
        /// to the renderer's control after the first click, the grid will listen for a
        /// MouseDown on the newly focused control and raise this event on a second click.
        /// <para/>
        /// Check the associated <see cref="GridCurrentCell.Renderer"/> for state of the cell renderer.<para/>
        /// <para/>
        /// Raising this event is only optional for the cell renderer that manages the active cell.<para/>
        /// <para/>
        /// A text box will usually send this event when the associated <see cref="TextBox"/> control has received
        /// the focus after the cell is switched into edit mode and the user double-clicks. Other cell renderers
        /// may or may not send this event.
        /// </remarks>
        [Description("Occurs when the current cell is in-place editing mode and the user double-clicks inside the control associated with the current cell."),
        Category("Behavior")]
        public event ControlEventHandler CurrentCellControlDoubleClick;

        /// <summary>
        /// Occurs when the current cell is in in-place editing mode and the control
        /// associated with the current cell has lost the focus.
        /// </summary>
        /// <remarks>
        /// Check the associated <see cref="GridCurrentCell.Renderer"/> for state of the cell renderer.<para/>
        /// Raising this event is only optional for the cell renderer that manages the active cell.<para/>
        /// A text box will usually send this event when the associated <see cref="TextBox"/> control has lost
        /// the focus after the cell is switched into edit mode with <see cref="GridCurrentCell.BeginEdit()"/>. Other cell renderers
        /// may or may not send this event.
        /// </remarks>
        [Description("Occurs when the current cell has switched to in-place editing and the control associated with the current cell has received the focus."),
        Category("Behavior")]
        public event ControlEventHandler CurrentCellControlLostFocus;

        /// <summary>
        /// Occurs when the user clicks a push button.
        /// </summary>
        [Description("Occurs when the user clicks a push button."),
        Category("Behavior")]
        public event GridCellPushButtonClickEventHandler PushButtonClick;

        /// <summary>
        /// The user clicks inside the checker box of a check box.
        /// </summary>
        [Description("Occurs when the user clicks inside the checker box of a check box cell."),
        Category("Behavior")]
        public event GridCellClickEventHandler CheckBoxClick;

        /// <summary>
        /// The user clicks inside a cell.
        /// </summary>
        [Description("Occurs when the user clicks inside a cell."),
        Category("Behavior")]
        public event GridCellClickEventHandler CellClick;

        /// <summary>
        /// The user double-clicks inside a cell.
        /// </summary>
        [Description("Occurs when the user double-clicks inside a cell."),
        Category("Behavior")]
        public event GridCellClickEventHandler CellDoubleClick;

        /// <summary>
        /// Grid performs hit-test for a cell.
        /// </summary>
        [Description("Occurs when the grid performs hit-test for a cell."),
        Category("Behavior")]
        public event GridCellHitTestEventHandler CellHitTest;

        /// <summary>
        /// Grid queries for the cursor to display for a specific cell when the cell indicated
        /// previously with a non-zero hit-test value that it wants the mouse operation.
        /// </summary>
        [Description("Occurs when the grid queries for the cursor to display for a specific cell when the cell."),
        Category("Behavior")]
        public event GridCellCursorEventHandler CellCursor;

        /// <summary>
        /// Occurs before the <see cref="GridCellRendererBase.OnMouseHoverEnter"/> method of a cell's <see cref="GridCellRendererBase"/> is called.
        /// </summary>
        /// <remarks>
        /// Event is raised both when the cell's HitTest method indicated
        /// previously with a non-zero hit-test value that it wants the mouse operation or when
        /// the user is hovering the mouse over cells and the "SelectCells" mouse controller
        /// is about to handle the mouse operation.
        /// </remarks>
        [Description("Occurs before the OnMouseHoverEnter method of a cell's renderer is called."),
        Category("Behavior")]
        public event GridCellMouseEventHandler CellMouseHoverEnter;

        /// <summary>
        /// Occurs before the <see cref="GridCellRendererBase.OnMouseHover"/> method of a cell's <see cref="GridCellRendererBase"/> is called.
        /// </summary>
        /// <remarks>
        /// Event is raised both when the cell's HitTest method indicated
        /// previously with a non-zero hit-test value that it wants the mouse operation or when
        /// the user is hovering the mouse over cells and the "SelectCells" mouse controller
        /// is about to handle the mouse operation.
        /// </remarks>
        [Description("Occurs before the OnMouseHover method of a cell's renderer is called."),
        Category("Behavior")]
        public event GridCellMouseEventHandler CellMouseHover;

        /// <summary>
        /// Occurs before the <see cref="GridCellRendererBase.OnMouseHoverLeave"/> method of a cell's <see cref="GridCellRendererBase"/> is called.
        /// </summary>
        /// <remarks>
        /// Event is raised both when the cell's HitTest method indicated
        /// previously with a non-zero hit-test value that it wants the mouse operation or when
        /// the user is hovering the mouse over cells and the "SelectCells" mouse controller
        /// is about to handle the mouse operation.
        /// </remarks>
        [Description("Occurs before the OnMouseHoverLeave method of a cell's renderer is called."),
        Category("Behavior")]
        public event GridCellMouseEventHandler CellMouseHoverLeave;

        /// <summary>
        /// Occurs before the <see cref="GridCellRendererBase.OnMouseDown"/> method of a cell's <see cref="GridCellRendererBase"/> is called.
        /// </summary>
        /// <remarks>
        /// Event is raised only when the cell's HitTest method indicated
        /// previously with a non-zero hit-test value that it wants the mouse operation.
        /// </remarks>
        [Description("Occurs before the OnMouseDown method of a cell's renderer is called."),
        Category("Behavior")]
        public event GridCellMouseEventHandler CellMouseDown;

        /// <summary>
        /// Occurs before the <see cref="GridCellRendererBase.OnMouseMove"/> method of a cell's <see cref="GridCellRendererBase"/> is called.
        /// </summary>
        /// <remarks>
        /// Event is raised only when the cell's HitTest method indicated
        /// previously with a non-zero hit-test value that it wants the mouse operation.
        /// </remarks>
        [Description("Occurs before the OnMouseMove method of a cell's renderer is called."),
        Category("Behavior")]
        public event GridCellMouseEventHandler CellMouseMove;

        /// <summary>
        /// Occurs before the <see cref="GridCellRendererBase.OnMouseUp"/> method of a cell's <see cref="GridCellRendererBase"/> is called.
        /// </summary>
        /// <remarks>
        /// Event is raised only when the cell's HitTest method indicated
        /// previously with a non-zero hit-test value that it wants the mouse operation.
        /// </remarks>
        [Description("Occurs before the OnMouseUp method of a cell's renderer is called."),
        Category("Behavior")]
        public event GridCellMouseEventHandler CellMouseUp;

        /// <summary>
        /// Occurs before the <see cref="GridCellRendererBase.OnCancelMode"/> method of a cell's <see cref="GridCellRendererBase"/> is called.
        /// </summary>
        /// <remarks>
        /// Event is raised only when the cell's HitTest method indicated
        /// previously with a non-zero hit-test value that it wants the mouse operation.
        /// </remarks>
        [Description("Occurs before the OnCancelMode method of a cell's renderer is called."),
        Category("Behavior")]
        public event GridCellMouseEventHandler CellCancelMode;

        /// <summary>
        /// Raises the <see cref="CellHitTest"/> event.
        /// </summary>
        /// <param name="e">A <see cref="GridCellHitTestEventArgs" /> that contains the event data.</param>
        protected virtual void OnCellHitTest(GridCellHitTestEventArgs e)
        {
            if (this.eventsTarget != null)
            {
                this.eventsTarget.OnCellHitTest(e);
            }
#if DEBUG

            if (Switches.GridControlBaseEvents.TraceVerbose)
            {
                TraceUtil.TraceCurrentMethodInfo(PaneDesc, e);
            }
#else

            ;
#endif

            if (CellHitTest != null)
            {
                CellHitTest(this, e);
            }
        }

        /// <summary>
        /// Initiates call to <see cref="OnCellHitTest"/>.
        /// </summary>
        /// <param name="e">The event arguments.</param>
        public void RaiseCellHitTest(GridCellHitTestEventArgs e)
        {
            OnCellHitTest(e);
        }

        /// <summary>
        /// Raises the <see cref="CellCursor"/> event.
        /// </summary>
        /// <param name="e">A <see cref="GridCellCursorEventArgs" /> that contains the event data.</param>
        protected virtual void OnCellCursor(GridCellCursorEventArgs e)
        {
            if (this.eventsTarget != null)
            {
                this.eventsTarget.OnCellCursor(e);
            }
#if DEBUG

            if (Switches.GridControlBaseEvents.TraceVerbose)
            {
                TraceUtil.TraceCurrentMethodInfo(PaneDesc, e);
            }
#else

            ;
#endif

            if (CellCursor != null)
            {
                CellCursor(this, e);
            }
        }

        /// <summary>
        /// Initiates call to <see cref="OnCellCursor"/>.
        /// </summary>
        /// <param name="e">The event arguments.</param>
        public void RaiseCellCursor(GridCellCursorEventArgs e)
        {
            OnCellCursor(e);
        }

        /// <summary>
        /// Raises the <see cref="CellMouseHoverEnter"/> event.
        /// </summary>
        /// <param name="e">A <see cref="GridCellMouseEventArgs" /> that contains the event data.</param>
        protected virtual void OnCellMouseHoverEnter(GridCellMouseEventArgs e)
        {
            if (this.eventsTarget != null)
            {
                this.eventsTarget.OnCellMouseHoverEnter(e);
            }
#if DEBUG

            if (Switches.GridControlBaseEvents.TraceVerbose)
            {
                TraceUtil.TraceCurrentMethodInfo(PaneDesc, e);
            }
#else

            ;
#endif

            if (CellMouseHoverEnter != null)
            {
                CellMouseHoverEnter(this, e);
            }
        }

        /// <summary>
        /// Initiates call to <see cref="OnCellMouseHoverEnter"/>.
        /// </summary>
        /// <param name="e">The event arguments.</param>
        public void RaiseCellMouseHoverEnter(GridCellMouseEventArgs e)
        {
            OnCellMouseHoverEnter(e);
        }

        /// <summary>
        /// Raises the <see cref="CellMouseHover"/> event.
        /// </summary>
        /// <param name="e">A <see cref="GridCellMouseEventArgs" /> that contains the event data.</param>
        protected virtual void OnCellMouseHover(GridCellMouseEventArgs e)
        {
            if (this.eventsTarget != null)
            {
                this.eventsTarget.OnCellMouseHover(e);
            }
#if DEBUG

            if (Switches.GridControlBaseEvents.TraceVerbose)
            {
                TraceUtil.TraceCurrentMethodInfo(PaneDesc, e);
            }
#else

            ;
#endif

            if (CellMouseHover != null)
            {
                CellMouseHover(this, e);
            }
        }

        /// <summary>
        /// Initiates call to <see cref="OnCellMouseHover"/>.
        /// </summary>
        /// <param name="e">The event arguments.</param>
        public void RaiseCellMouseHover(GridCellMouseEventArgs e)
        {
            OnCellMouseHover(e);
        }

        /// <summary>
        /// Raises the <see cref="CellMouseHoverLeave"/> event.
        /// </summary>
        /// <param name="e">A <see cref="GridCellMouseEventArgs" /> that contains the event data.</param>
        protected virtual void OnCellMouseHoverLeave(GridCellMouseEventArgs e)
        {
            if (this.eventsTarget != null)
            {
                this.eventsTarget.OnCellMouseHoverLeave(e);
            }
#if DEBUG

            if (Switches.GridControlBaseEvents.TraceVerbose)
            {
                TraceUtil.TraceCurrentMethodInfo(PaneDesc, e);
            }
#else

            ;
#endif

            if (CellMouseHoverLeave != null)
            {
                CellMouseHoverLeave(this, e);
            }
        }

        /// <summary>
        /// Initiates call to <see cref="OnCellMouseHoverLeave"/>.
        /// </summary>
        /// <param name="e">The event arguments.</param>
        public void RaiseCellMouseHoverLeave(GridCellMouseEventArgs e)
        {
            OnCellMouseHoverLeave(e);
        }

        /// <summary>
        /// Raises the <see cref="CellMouseDown"/> event.
        /// </summary>
        /// <param name="e">A <see cref="GridCellMouseEventArgs" /> that contains the event data.</param>
        protected virtual void OnCellMouseDown(GridCellMouseEventArgs e)
        {
            if (this.eventsTarget != null)
            {
                this.eventsTarget.OnCellMouseDown(e);
            }
#if DEBUG

            if (Switches.GridControlBaseEvents.TraceVerbose)
            {
                TraceUtil.TraceCurrentMethodInfo(PaneDesc, e);
            }
#else

            ;
#endif

            if (CellMouseDown != null)
            {
                CellMouseDown(this, e);
            }
        }

        /// <summary>
        /// Initiates call to <see cref="OnCellMouseDown"/>.
        /// </summary>
        /// <param name="e">The event arguments.</param>
        public void RaiseCellMouseDown(GridCellMouseEventArgs e)
        {
            OnCellMouseDown(e);
        }

        /// <summary>
        /// Raises the <see cref="CellMouseMove"/> event.
        /// </summary>
        /// <param name="e">A <see cref="GridCellMouseEventArgs" /> that contains the event data.</param>
        protected virtual void OnCellMouseMove(GridCellMouseEventArgs e)
        {
            if (this.eventsTarget != null)
            {
                this.eventsTarget.OnCellMouseMove(e);
            }
#if DEBUG

            if (Switches.GridControlBaseEvents.TraceVerbose)
            {
                TraceUtil.TraceCurrentMethodInfo(PaneDesc, e);
            }
#else

            ;
#endif

            if (CellMouseMove != null)
            {
                CellMouseMove(this, e);
            }
        }

        /// <summary>
        /// Initiates call to <see cref="OnCellMouseMove"/>.
        /// </summary>
        /// <param name="e">The event arguments.</param>
        public void RaiseCellMouseMove(GridCellMouseEventArgs e)
        {
            OnCellMouseMove(e);
        }

        /// <summary>
        /// Raises the <see cref="CellMouseUp"/> event.
        /// </summary>
        /// <param name="e">A <see cref="GridCellMouseEventArgs" /> that contains the event data.</param>
        protected virtual void OnCellMouseUp(GridCellMouseEventArgs e)
        {
            if (this.eventsTarget != null)
            {
                this.eventsTarget.OnCellMouseUp(e);
            }
#if DEBUG

            if (Switches.GridControlBaseEvents.TraceVerbose)
            {
                TraceUtil.TraceCurrentMethodInfo(PaneDesc, e);
            }
#else

            ;
#endif

            if (CellMouseUp != null)
            {
                CellMouseUp(this, e);
            }
        }

        /// <summary>
        /// Initiates call to <see cref="OnCellMouseUp"/>.
        /// </summary>
        /// <param name="e">The event arguments.</param>
        public void RaiseCellMouseUp(GridCellMouseEventArgs e)
        {
            OnCellMouseUp(e);
        }

        /// <summary>
        /// Raises the <see cref="CellCancelMode"/> event.
        /// </summary>
        /// <param name="e">A <see cref="GridCellMouseEventArgs" /> that contains the event data.</param>
        protected virtual void OnCellCancelMode(GridCellMouseEventArgs e)
        {
            if (this.eventsTarget != null)
            {
                this.eventsTarget.OnCellCancelMode(e);
            }
#if DEBUG

            if (Switches.GridControlBaseEvents.TraceVerbose)
            {
                TraceUtil.TraceCurrentMethodInfo(PaneDesc, e);
            }
#else

            ;
#endif

            if (CellCancelMode != null)
            {
                CellCancelMode(this, e);
            }
        }

        /// <summary>
        /// Initiates call to <see cref="OnCellCancelMode"/>.
        /// </summary>
        /// <param name="e">The event arguments.</param>
        public void RaiseCellCancelMode(GridCellMouseEventArgs e)
        {
            OnCellCancelMode(e);
        }

        /// <summary>
        /// Indicates that scrollbar information such as Minimum or Maximum has changed.
        /// </summary>
        /// <remarks>
        /// If you want to be updated about changes in the Minimum or Maximum scroll position or page size,
        /// you should handle this event.
        /// </remarks>
        [Category("Scrolling")]
        [Description("Occurs when changes occurred in the Minimum or Maximum scroll position or page size.")]
        public event EventHandler ScrollInfoChanged;

        internal void RaiseScrollInfoChanged()
        {
            OnScrollInfoChanged(EventArgs.Empty);
        }

        /// <summary>
        /// Raises the <see cref="GridControlBase.CurrentCellMoving"/> event.
        /// </summary>
        /// <param name="e">A <see cref="GridCurrentCellMovingEventArgs" /> that contains the event data.</param>
        protected virtual void OnScrollInfoChanged(EventArgs e)
        {
            if (this.eventsTarget != null)
            {
                this.eventsTarget.OnScrollInfoChanged(e);
            }
#if DEBUG

            if (Switches.GridControlBaseEvents.TraceVerbose)
            {
                TraceUtil.TraceCurrentMethodInfo(PaneDesc);
            }
#else

            ;
#endif

            if (ScrollInfoChanged != null)
            {
                ScrollInfoChanged(this, EventArgs.Empty);
            }
        }

        /// <summary>
        /// Raises the <see cref="GridControlBase.CurrentCellMoving"/> event.
        /// </summary>
        /// <param name="e">A <see cref="GridCurrentCellMovingEventArgs" /> that contains the event data.</param>
        protected virtual void OnCurrentCellMoving(GridCurrentCellMovingEventArgs e)
        {
            if (this.eventsTarget != null)
            {
                this.eventsTarget.OnCurrentCellMoving(e);
            }
#if DEBUG

            if (Switches.Development.TraceVerbose)
            {
                TraceUtil.TraceCurrentMethodInfo(IsHandleCreated, Visible, PaneDesc, e);
            }
#else

            ;
#endif
#if DEBUG
            if (Switches.GridControlBaseEvents.TraceVerbose)
            {
                TraceUtil.TraceCurrentMethodInfo(PaneDesc, e);
            }
#else
            ;
#endif

            if (CurrentCellMoving != null)
            {
                CurrentCellMoving(this, e);
            }
        }

        /// <summary>
        /// Raises the <see cref="GridControlBase.CurrentCellMoved"/> event.
        /// </summary>
        /// <param name="e">A <see cref="GridCurrentCellMovedEventArgs" /> that contains the event data.</param>
        protected virtual void OnCurrentCellMoved(GridCurrentCellMovedEventArgs e)
        {
            if (this.eventsTarget != null)
            {
                this.eventsTarget.OnCurrentCellMoved(e);
            }
#if DEBUG
            if (Switches.GridControlBaseEvents.TraceVerbose)
            {
                TraceUtil.TraceCurrentMethodInfo(PaneDesc, e);
            }
#else
            ;
#endif

            if (CurrentCellMoved != null)
            {
                CurrentCellMoved(this, e);
            }
        }

        /// <summary>
        /// Raises the <see cref="GridControlBase.CurrentCellMoveFailed"/> event.
        /// </summary>
        /// <param name="e">A <see cref="GridCurrentCellMoveFailedEventArgs"/> that contains the event data.</param>
        protected virtual void OnCurrentCellMoveFailed(GridCurrentCellMoveFailedEventArgs e)
        {
            if (this.eventsTarget != null)
            {
                this.eventsTarget.OnCurrentCellMoveFailed(e);
            }
#if DEBUG
            if (Switches.GridControlBaseEvents.TraceVerbose)
            {
                TraceUtil.TraceCurrentMethodInfo(PaneDesc, e);
            }
#else

            ;
#endif

            if (CurrentCellMoveFailed != null)
            {
                CurrentCellMoveFailed(this, e);
            }
        }

        GridCurrentCellActivatingEventArgs delayCurrentCellActivateArgs = null;
        bool forceCurrentCellMoveTo = true;

        /// <summary>
        /// Gets or sets a value indicating whether False, any calls to <see cref="GridCurrentCell.MoveTo(int,int,Syncfusion.Windows.Forms.Grid.GridSetCurrentCellOptions,bool)"/> will be deferred
        /// until the grid sets the active control inside a parent container
        /// and its <see cref="Control.OnEnter"/> method is called.
        /// </summary>
        [Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public bool ForceCurrentCellMoveTo
        {
            get
            {
                return forceCurrentCellMoveTo;
            }

            set
            {
                forceCurrentCellMoveTo = value;
            }
        }

        internal bool CheckDelayInitialActivateCC(GridCurrentCellActivatingEventArgs e)
        {
            if (this.GetType().ToString().Equals("Syncfusion.Windows.Forms.Tools.XPMenus.MenuGrid"))
            {
                this.WantKeys = this.GetStyle(ControlStyles.Selectable);
                if (!this.WantKeys)
                {
                    return true;
                }
            }
            else
            {
                if(! this.GetStyle(ControlStyles.Selectable))
                {
                    return true;
                }
            }
            if (!forceCurrentCellMoveTo && !CurrentCell.HasCurrentCell && !this.IsActiveControl)
            {
                delayCurrentCellActivateArgs = e;
                e.Cancel = true;
                return false;
            }

            ////delayCurrentCellActivateArgs = null;
            return true;
        }

        /// <summary>
        /// Raises the <see cref="GridControlBase.CurrentCellActivating"/> event.
        /// </summary>
        /// <param name="e">A <see cref="GridCurrentCellActivatingEventArgs" /> that contains the event data.</param>
        protected virtual void OnCurrentCellActivating(GridCurrentCellActivatingEventArgs e)
        {
            if (this.eventsTarget != null)
            {
                this.eventsTarget.OnCurrentCellActivating(e);
            }
#if DEBUG
            if (Switches.Development.TraceVerbose)
            {
                TraceUtil.TraceCurrentMethodInfo(IsHandleCreated, Visible, PaneDesc, e);
            }
#else
            ;
#endif
#if DEBUG
            if (Switches.GridControlBaseEvents.TraceVerbose)
            {
                TraceUtil.TraceCurrentMethodInfo(PaneDesc, e);
            }
#else
            ;
#endif

            if (CurrentCellActivating != null)
            {
                CurrentCellActivating(this, e);
            }

            delayCurrentCellActivateArgs = null;
            if ((this.RightToLeft == System.Windows.Forms.RightToLeft.Yes) && EnableRTLMark)
            {
                this.gridCurrentCell.Grid.Model[e.RowIndex, e.ColIndex].Text = (char)unicodeRLM + this.gridCurrentCell.Grid.Model[e.RowIndex, e.ColIndex].Text + (char)unicodeRLM;
            }
          
        }

        /// <summary>
        /// Raises the <see cref="GridControlBase.PushButtonClick"/> event.
        /// </summary>
        /// <param name="e">A <see cref="GridCellPushButtonClickEventArgs" /> that contains the event data.</param>
        protected virtual void OnPushButtonClick(GridCellPushButtonClickEventArgs e)
        {
            if (this.eventsTarget != null)
            {
                this.eventsTarget.OnPushButtonClick(e);
            }
#if DEBUG

            if (Switches.GridControlBaseEvents.TraceVerbose)
            {
                TraceUtil.TraceCurrentMethodInfo(PaneDesc, e);
            }
#else

            ;
#endif

            if (PushButtonClick != null)
            {
                PushButtonClick(this, e);
            }
        }

        /// <summary>
        /// Triggers a call to <see cref="GridControlBase.OnPushButtonClick"/> and
        /// thus raises the <see cref="GridControlBase.PushButtonClick"/> event.
        /// </summary>
        /// <param name="rowIndex">The row index.</param>
        /// <param name="colIndex">The column index.</param>
        public void RaisePushButtonClick(int rowIndex, int colIndex)
        {
#if DEBUG
            if (Switches.CurrentCell.TraceVerbose)
            {
                TraceUtil.TraceCurrentMethodInfo(PaneDesc, rowIndex, colIndex);
            }
#else
            ;
#endif
            GridCellPushButtonClickEventArgs e = new GridCellPushButtonClickEventArgs(rowIndex, colIndex);
            OnPushButtonClick(e);
        }

        /// <summary>
        /// Raises the cancelable <see cref="GridControlBase.CellClick"/> event.
        /// </summary>
        /// <param name="e">A <see cref="GridCellClickEventArgs" /> that contains the event data.</param>
        protected virtual void OnCellClick(GridCellClickEventArgs e)
        {
            if (this.eventsTarget != null)
            {
                this.eventsTarget.OnCellClick(e);
            }
#if DEBUG

            if (Switches.GridControlBaseEvents.TraceVerbose)
            {
                TraceUtil.TraceCurrentMethodInfo(PaneDesc, e);
            }
#else

            ;
#endif

            if (CellClick != null)
            {
                CellClick(this, e);
            }
        }

        /// <overload>
        /// Triggers a call to <see cref="GridControlBase.OnCellClick"/> and
        /// thus raises the <see cref="GridControlBase.CellClick"/> event.
        /// </overload>
        /// <summary>
        /// Triggers a call to <see cref="GridControlBase.OnCellClick"/> and
        /// thus raises the <see cref="GridControlBase.CellClick"/> event.
        /// </summary>
        /// <param name="rowIndex">The row index.</param>
        /// <param name="colIndex">The column index.</param>
        /// <param name="mouseEventArgs">The MouseEventArgs for this event.</param>
        /// <param name="overImage">Indicates if the mouse was over an image (see <see cref="GridStyleInfo.ImageIndex"/>)
        /// in static cell when the mouse was released.</param>
        /// <returns>True if operation should continue with default behavior; False if not.</returns>
        public bool RaiseCellClick(int rowIndex, int colIndex, MouseEventArgs mouseEventArgs, bool overImage)
        {
#if DEBUG
            if (Switches.CurrentCell.TraceVerbose)
            {
                TraceUtil.TraceCurrentMethodInfo(PaneDesc, rowIndex, colIndex, overImage);
            }
#else
            ;
#endif
            GridCellClickEventArgs e = new GridCellClickEventArgs(rowIndex, colIndex, mouseEventArgs, overImage);
            OnCellClick(e);
            return !e.Cancel;
        }

        /// <summary>
        /// Triggers a call to <see cref="GridControlBase.OnCellClick"/> and
        /// thus raises the <see cref="GridControlBase.CellClick"/> event.
        /// </summary>
        /// <param name="rowIndex">The row index.</param>
        /// <param name="colIndex">The column index.</param>
        /// <param name="mouseEventArgs">The MouseEventArgs for this event.</param>
        /// <returns>True if operation should continue with default behavior; False if not.</returns>
        public bool RaiseCellClick(int rowIndex, int colIndex, MouseEventArgs mouseEventArgs)
        {
#if DEBUG
            if (Switches.CurrentCell.TraceVerbose)
            {
                TraceUtil.TraceCurrentMethodInfo(PaneDesc, rowIndex, colIndex);
            }
#else
            ;
#endif
            GridCellClickEventArgs e = new GridCellClickEventArgs(rowIndex, colIndex, mouseEventArgs, false);
            OnCellClick(e);
            return !e.Cancel;
        }

        /// <summary>
        /// Raises the cancelable <see cref="GridControlBase.CheckBoxClick"/> event.
        /// </summary>
        /// <param name="e">A <see cref="GridCellClickEventArgs" /> that contains the event data.</param>
        protected virtual void OnCheckBoxClick(GridCellClickEventArgs e)
        {
            if (this.eventsTarget != null)
            {
                this.eventsTarget.OnCheckBoxClick(e);
            }
#if DEBUG

            if (Switches.GridControlBaseEvents.TraceVerbose)
            {
                TraceUtil.TraceCurrentMethodInfo(PaneDesc, e);
            }
#else

            ;
#endif

            if (CheckBoxClick != null)
            {
                CheckBoxClick(this, e);
            }
        }

        /// <summary>
        /// Triggers a call to <see cref="GridControlBase.OnCheckBoxClick"/> and
        /// thus raises the <see cref="GridControlBase.CheckBoxClick"/> event.
        /// </summary>
        /// <param name="rowIndex">The row index.</param>
        /// <param name="colIndex">The column index.</param>
        /// <param name="mouseEventArgs">The MouseEventArgs for this event.</param>
        /// <returns>True if operation should continue with default behavior; False if not.</returns>
        public bool RaiseCheckBoxClick(int rowIndex, int colIndex, MouseEventArgs mouseEventArgs)
        {
#if DEBUG
            if (Switches.CurrentCell.TraceVerbose)
            {
                TraceUtil.TraceCurrentMethodInfo(PaneDesc, rowIndex, colIndex);
            }
#else
            ;
#endif
            GridCellClickEventArgs e = new GridCellClickEventArgs(rowIndex, colIndex, mouseEventArgs, false);
            OnCheckBoxClick(e);
            return !e.Cancel;
        }

        /// <summary>
        /// Raises the cancelable <see cref="GridControlBase.CellClick"/> event.
        /// </summary>
        /// <param name="e">A <see cref="GridCellClickEventArgs" /> that contains the event data.</param>
        protected virtual void OnCellDoubleClick(GridCellClickEventArgs e)
        {
            if (this.eventsTarget != null)
            {
                this.eventsTarget.OnCellDoubleClick(e);
            }
#if DEBUG

            if (Switches.GridControlBaseEvents.TraceVerbose)
            {
                TraceUtil.TraceCurrentMethodInfo(PaneDesc, e);
            }
#else

            ;
#endif

            if (CellDoubleClick != null)
            {
                CellDoubleClick(this, e);
            }
        }

        /// <overload>
        /// Triggers a call to <see cref="GridControlBase.OnCellDoubleClick"/> and
        /// thus raises the <see cref="GridControlBase.CellDoubleClick"/> event.
        /// </overload>
        /// <summary>
        /// Triggers a call to <see cref="GridControlBase.OnCellDoubleClick"/> and
        /// thus raises the <see cref="GridControlBase.CellDoubleClick"/> event.
        /// </summary>
        /// <param name="rowIndex">The row index.</param>
        /// <param name="colIndex">The column index.</param>
        /// <param name="mouseEventArgs">The MouseEventArgs for this event.</param>
        /// <param name="overImage">Indicates if the mouse was over an image (see <see cref="GridStyleInfo.ImageIndex"/>)
        /// in a static cell when the mouse was released.</param>
        /// <returns>True if operation should continue with default behavior; False if not.</returns>
        public bool RaiseCellDoubleClick(int rowIndex, int colIndex, MouseEventArgs mouseEventArgs, bool overImage)
        {
#if DEBUG
            if (Switches.CurrentCell.TraceVerbose)
            {
                TraceUtil.TraceCurrentMethodInfo(PaneDesc, rowIndex, colIndex, overImage);
            }
#else
            ;
#endif
            GridCellClickEventArgs e = new GridCellClickEventArgs(rowIndex, colIndex, mouseEventArgs, overImage);
            OnCellDoubleClick(e);
            return !e.Cancel;
        }

        /// <summary>
        /// Triggers a call to <see cref="GridControlBase.OnCellDoubleClick"/> and
        /// thus raises the <see cref="GridControlBase.CellDoubleClick"/> event.
        /// </summary>
        /// <param name="rowIndex">The row index.</param>
        /// <param name="colIndex">The column index.</param>
        /// <param name="mouseEventArgs">The MouseEventArgs for this event.</param>
        /// <returns>True if operation should continue with default behavior; False if not.</returns>
        public bool RaiseCellDoubleClick(int rowIndex, int colIndex, MouseEventArgs mouseEventArgs)
        {
#if DEBUG
            if (Switches.CurrentCell.TraceVerbose)
            {
                TraceUtil.TraceCurrentMethodInfo(PaneDesc, rowIndex, colIndex);
            }
#else
            ;
#endif
            GridCellClickEventArgs e = new GridCellClickEventArgs(rowIndex, colIndex, mouseEventArgs, false);
            OnCellDoubleClick(e);
            return !e.Cancel;
        }

        /// <summary>
        /// Raises the <see cref="GridControlBase.CurrentCellActivated"/> event.
        /// </summary>
        /// <param name="e">A <see cref="EventArgs" /> that contains the event data.</param>
        protected virtual void OnCurrentCellActivated(EventArgs e)
        {
            if (this.eventsTarget != null)
            {
                this.eventsTarget.OnCurrentCellActivated(e);
            }
#if DEBUG

            if (Switches.GridControlBaseEvents.TraceVerbose)
            {
                TraceUtil.TraceCurrentMethodInfo(PaneDesc, e);
            }
#else

            ;
#endif

            if (CurrentCellActivated != null)
            {
                CurrentCellActivated(this, e);
            }
        }

        /// <summary>
        /// Raises the <see cref="GridControlBase.CurrentCellActivateFailed"/> event.
        /// </summary>
        /// <param name="e">A <see cref="GridCurrentCellActivateFailedEventArgs" /> that contains the event data.</param>
        protected virtual void OnCurrentCellActivateFailed(GridCurrentCellActivateFailedEventArgs e)
        {
            if (this.eventsTarget != null)
            {
                this.eventsTarget.OnCurrentCellActivateFailed(e);
            }
#if DEBUG

            if (Switches.GridControlBaseEvents.TraceVerbose)
            {
                TraceUtil.TraceCurrentMethodInfo(PaneDesc, e);
            }
#else

            ;
#endif

            if (CurrentCellActivateFailed != null)
            {
                CurrentCellActivateFailed(this, e);
            }
        }

        /// <summary>
        /// Raises the <see cref="GridControlBase.CurrentCellDeleting"/> event.
        /// </summary>
        /// <param name="e">A <see cref="CancelEventArgs" /> that contains the event data.</param>
        protected virtual void OnCurrentCellDeleting(CancelEventArgs e)
        {
            if (this.eventsTarget != null)
            {
                this.eventsTarget.OnCurrentCellDeleting(e);
            }
#if DEBUG

            if (Switches.GridControlBaseEvents.TraceVerbose)
            {
                TraceUtil.TraceCurrentMethodInfo(PaneDesc, e);
            }
#else

            ;
#endif

            if (CurrentCellDeleting != null)
            {
                CurrentCellDeleting(this, e);
            }
        }

        /// <summary>
        /// Raises the <see cref="GridControlBase.CurrentCellStartEditing"/> event.
        /// </summary>
        /// <param name="e">A <see cref="CancelEventArgs" /> that contains the event data.</param>
        protected virtual void OnCurrentCellStartEditing(CancelEventArgs e)
        {
            if (this.eventsTarget != null)
            {
                this.eventsTarget.OnCurrentCellStartEditing(e);
            }
#if DEBUG

            if (Switches.GridControlBaseEvents.TraceVerbose)
            {
                TraceUtil.TraceCurrentMethodInfo(PaneDesc, e);
            }
#else

            ;
#endif

            if (CurrentCellStartEditing != null)
            {
                CurrentCellStartEditing(this, e);
            }
        }

        /// <summary>
        /// User pressed key down. (similar to Control.OnKeyDown)
        /// </summary>
        /// <param name="e">Event data.</param>
        public virtual void OnCurrentCellKeyDown(KeyEventArgs e)
        {
            if (this.eventsTarget != null)
            {
                this.eventsTarget.OnCurrentCellKeyDown(e);
            }
#if DEBUG
            if (Switches.KeyboardEvents.TraceVerbose || Switches.GridControlBaseEvents.TraceVerbose)
            {
                TraceUtil.TraceCurrentMethodInfo(PaneDesc, e.Handled, e.KeyCode, Control.ModifierKeys);
            }
#endif
            if (CurrentCellKeyDown != null)
            {
                CurrentCellKeyDown(this, e);
            }

            DefaultCurrentCellKeyDown(e);
        }

        void DefaultCurrentCellKeyDown(KeyEventArgs e)
        {
            if (!e.Handled)
            {
                bool bCtl = (Control.ModifierKeys & Keys.Control) != Keys.None;
                bool bShift = (Control.ModifierKeys & Keys.Shift) != Keys.None;
                bool bAlt = (Control.ModifierKeys & Keys.Alt) != Keys.None;

                switch (e.KeyCode)
                {
                    case Keys.Escape:
                        if (WantEscapeKey)
                        {
                            CancelUpdate();
                            if (CurrentCell.IsDroppedDown)
                            {
                                CurrentCell.CloseDropDown(PopupCloseType.Canceled);
                                e.Handled = true;
                            }
                            else if (CurrentCell.IsModified)
                            {
                                CurrentCell.RejectChanges();
                                CurrentCell.CancelEdit();
                                CurrentCell.Refresh();
                                Focus();
                                e.Handled = true;
                            }
                            else
                            {
                                CurrentCell.CancelEdit();
                                CurrentCell.Refresh();
                                Focus();
                            }

                            Update();
                            FixCurrentCellGotFocus();
                        }

                        return;

                    ////                    case Keys.Delete:
                    ////                        if (bShift)
                    ////                        {
                    ////                            Model.CutPaste.Cut();
                    ////                            e.Handled = true;
                    ////                        }
                    ////                        break;

                    ////                    case Keys.Insert:
                    ////                        if (bCtl)
                    ////                        {
                    ////                            Model.CutPaste.Copy();
                    ////                            e.Handled = true;
                    ////                        }
                    ////                        else if (bShift)
                    ////                        {
                    ////                            Model.CutPaste.Paste();
                    ////                            e.Handled = true;
                    ////                        }
                    ////                        break;

                    ////                    case Keys.C:
                    ////                    case Keys.V:
                    ////                    case Keys.X:
                    ////                      //// will be handled in OnKeyPress
                    ////                        break;
                }
            }
        }

        /// <summary>
        /// User released key. (similar to Control.OnKeyUp)
        /// </summary>
        /// <param name="e">A <see cref="KeyEventArgs"/> with data of the keyboard event.</param>
        public virtual void OnCurrentCellKeyUp(KeyEventArgs e)
        {
            if (this.eventsTarget != null)
            {
                this.eventsTarget.OnCurrentCellKeyUp(e);
            }

#if DEBUG
            if (Switches.KeyboardEvents.TraceVerbose || Switches.GridControlBaseEvents.TraceVerbose)
            {
                TraceUtil.TraceCurrentMethodInfo(PaneDesc, e.Handled, e.KeyCode, Control.ModifierKeys);
            }
#endif
            if (CurrentCellKeyUp != null)
            {
                CurrentCellKeyUp(this, e);
            }
        }

        /// <summary>
        /// User pressed a key. (similar to Control.OnKeyPress)
        /// </summary>
        /// <param name="e">A <see cref="KeyEventArgs"/> with data of the keyboard event.</param>
        public virtual void OnCurrentCellKeyPress(KeyPressEventArgs e)
        {
            if (this.eventsTarget != null)
            {
                this.eventsTarget.OnCurrentCellKeyPress(e);
            }

#if DEBUG
            if (Switches.KeyboardEvents.TraceVerbose || Switches.GridControlBaseEvents.TraceVerbose)
            {
                TraceUtil.TraceCurrentMethodInfo(PaneDesc, e.Handled, e.KeyChar);
            }
#endif
            if (CurrentCellKeyPress != null)
            {
                CurrentCellKeyPress(this, e);
            }
        }

        /// <summary>
        /// Called from OnValidate and checks whether the specified text is valid and raises the <see cref="CurrentCellValidateString"/> event.
        /// </summary>
        /// <param name="e">The event data with text to be validated.</param>        ///
        /// <remarks>
        ///  True if text if valid; False otherwise.
        /// This also works for limiting the keyboard input, e.g. only digits.
        /// Called after the user presses a key and before it is accepted.
        /// </remarks>
        /// <example>
        /// Don't allow "-" to be typed.
        /// <code lang="C#">
        ///         public override void OnCurrentCellValidateString(GridCurrentCellValidateStringEventArgs e)
        ///         {
        ///             TraceUtil.TraceCurrentMethodInfoIf(Switches.CellRenderer.TraceVerbose, e);
        /// <para/>
        ///             if (e.Text.IndexOf("-") != -1)
        ///                 e.Cancel = true;
        /// <para/>
        ///             base.OnCurrentCellValidateString(e);
        ///         }
        /// </code>
        /// </example>
        public virtual void OnCurrentCellValidateString(GridCurrentCellValidateStringEventArgs e)
        {
            if (this.eventsTarget != null)
            {
                this.eventsTarget.OnCurrentCellValidateString(e);
            }

#if DEBUG
            if (Switches.KeyboardEvents.TraceVerbose || Switches.GridControlBaseEvents.TraceVerbose)
            {
                TraceUtil.TraceCurrentMethodInfo(PaneDesc, e);
            }
#endif
            if (CurrentCellValidateString != null)
            {
                CurrentCellValidateString(this, e);
            }
        }

        /// <summary>
        /// Occurs after the user presses a key in the current cell and before it is accepted. Allows you to limit
        /// the keys that are accepted for the current cell while the user is typing text.
        /// </summary>
        [Description("Occurs after the user presses a key in the current cell and before it is accepted."),
        Category("Behavior")]
        public event GridCurrentCellValidateStringEventHandler CurrentCellValidateString;

        /// <summary>
        /// Occurs before <see cref="GridCellRendererBase.OnKeyPress"/> is called.
        /// </summary>
        [Category("Behavior")]
        [Description("Occurs when the current cell state tends to change on key press")]
        public event KeyPressEventHandler CurrentCellKeyPress;

        /// <summary>
        /// Occurs before <see cref="GridCellRendererBase.OnKeyDown"/> is called.
        /// </summary>
        [Category("Behavior")]
        [Description("Occurs when the current cell state tends to change on key down.")]
        public event KeyEventHandler CurrentCellKeyDown;

        /// <summary>
        /// Occurs before <see cref="GridCellRendererBase.OnKeyUp"/> is called.
        /// </summary>
        [Category("Behavior")]
        [Description("Occurs when the current cell state tends to change on key up.")]
        public event KeyEventHandler CurrentCellKeyUp;

        /// <summary>
        /// Raises the <see cref="GridControlBase.CurrentCellChanging"/> event.
        /// </summary>
        /// <param name="e">A <see cref="CancelEventArgs" /> that contains the event data.</param>
        protected virtual void OnCurrentCellChanging(CancelEventArgs e)
        {
            if (this.eventsTarget != null)
            {
                this.eventsTarget.OnCurrentCellChanging(e);
            }
#if DEBUG

            if (Switches.GridControlBaseEvents.TraceVerbose)
            {
                TraceUtil.TraceCurrentMethodInfo(PaneDesc, e);
            }
#else

            ;
#endif

            if (CurrentCellChanging != null)
            {
                CurrentCellChanging(this, e);
            }
        }

        /// <summary>
        /// This method is called from GridCellRendererBase.NotifyCurrentCellChanged. The
        /// default implementation of this virtual method raises the GridCurrentCell.CellChanged
        /// event indicating the contents of the current cell have been changed
        /// (e.g. in response to a TextBox.Changed event).
        /// </summary>
        /// <remarks>If you have implemented custom behavior which alters the content of the
        /// grid and current cell after a CurrentCellChanging event and you do not want a
        /// CurrentCellChanged event to be raised you can override this method and thereby
        /// avoid the event being raised.
        /// </remarks>
        protected virtual void NotifyCurrentCellChanged()
        {
#if DEBUG
            if (Switches.GridControlBaseEvents.TraceVerbose)
            {
                TraceUtil.TraceCurrentMethodInfo();
            }
#endif
            CurrentCell.NotifyChanged();
        }

        internal void RaiseNotifyCurrentCellChanged()
        {
            NotifyCurrentCellChanged();
        }

        /// <summary>
        /// Raises the <see cref="GridControlBase.CurrentCellChanged"/> event.
        /// </summary>
        /// <param name="e">A <see cref="EventArgs" /> that contains the event data.</param>
        protected virtual void OnCurrentCellChanged(EventArgs e)
        {
            if (this.eventsTarget != null)
            {
                this.eventsTarget.OnCurrentCellChanged(e);
            }
#if DEBUG

            if (Switches.GridControlBaseEvents.TraceVerbose)
            {
                TraceUtil.TraceCurrentMethodInfo(PaneDesc, e);
            }
#else

            ;
#endif

            if (CurrentCellChanged != null)
            {
                CurrentCellChanged(this, e);
            }
        }

        /// <summary>
        /// Raises the <see cref="GridControlBase.CurrentCellDeactivating"/> event.
        /// </summary>
        /// <param name="e">An <see cref="EventArgs" /> that contains the event data.</param>
        protected virtual void OnCurrentCellDeactivating(CancelEventArgs e)
        {
            if (this.eventsTarget != null)
            {
                this.eventsTarget.OnCurrentCellDeactivating(e);
            }
#if DEBUG

            if (Switches.GridControlBaseEvents.TraceVerbose)
            {
                TraceUtil.TraceCurrentMethodInfo(PaneDesc, e);
            }
#else

            ;
#endif

            if (CurrentCellDeactivating != null)
            {
                CurrentCellDeactivating(this, e);
            }
        }

        /// <summary>
        /// Raises the cancelable <see cref="GridControlBase.CurrentCellValidating"/> event.
        /// </summary>
        /// <param name="e">An <see cref="CancelEventArgs" /> that contains the event data.</param>
        protected virtual void OnCurrentCellValidating(CancelEventArgs e)
        {
            if (this.eventsTarget != null)
            {
                this.eventsTarget.OnCurrentCellValidating(e);
            }
#if DEBUG

            if (Switches.GridControlBaseEvents.TraceVerbose)
            {
                TraceUtil.TraceCurrentMethodInfo(PaneDesc, e);
            }
#else

            ;
#endif

            if (CurrentCellValidating != null)
            {
                CurrentCellValidating(this, e);
            }
        }

        /// <summary>
        /// Raises the <see cref="GridControlBase.CurrentCellValidated"/> event.
        /// </summary>
        /// <param name="e">An <see cref="EventArgs" /> that contains the event data.</param>
        protected virtual void OnCurrentCellValidated(EventArgs e)
        {
            if (this.eventsTarget != null)
            {
                this.eventsTarget.OnCurrentCellValidated(e);
            }
#if DEBUG
            if (Switches.GridControlBaseEvents.TraceVerbose)
            {
                TraceUtil.TraceCurrentMethodInfo(PaneDesc, e);
            }
#else

            ;
#endif

            if (CurrentCellValidated != null)
            {
                CurrentCellValidated(this, e);
            }
        }

        /// <summary>
        /// Raises the cancelable <see cref="GridControlBase.CurrentCellAcceptedChanges"/> event.
        /// </summary>
        /// <param name="e">An <see cref="EventArgs" /> that contains the event data.</param>
        protected virtual void OnCurrentCellAcceptedChanges(CancelEventArgs e)
        {
            if (this.eventsTarget != null)
            {
                this.eventsTarget.OnCurrentCellAcceptedChanges(e);
            }
#if DEBUG

            if (Switches.GridControlBaseEvents.TraceVerbose)
            {
                TraceUtil.TraceCurrentMethodInfo(PaneDesc, e);
            }
#else

            ;
#endif

            if (CurrentCellAcceptedChanges != null)
            {
                CurrentCellAcceptedChanges(this, e);
            }
        }

        /// <summary>
        /// Raises the <see cref="CurrentCellConfirmChangesFailed"/> event.
        /// </summary>
        /// <param name="e">An <see cref="EventArgs" /> that contains the event data.</param>
        protected virtual void OnCurrentCellConfirmChangesFailed(EventArgs e)
        {
            if (this.eventsTarget != null)
            {
                this.eventsTarget.OnCurrentCellConfirmChangesFailed(e);
            }
#if DEBUG

            if (Switches.GridControlBaseEvents.TraceVerbose)
            {
                TraceUtil.TraceCurrentMethodInfo(PaneDesc, e);
            }
#else

            ;
#endif

            if (CurrentCellConfirmChangesFailed != null)
            {
                CurrentCellConfirmChangesFailed(this, e);
            }
        }
        
        /// <summary>
        /// Raises the <see cref="GridControlBase.CurrentCellRejectedChanges"/> event.
        /// </summary>
        /// <param name="e">An <see cref="EventArgs" /> that contains the event data.</param>
        protected virtual void OnCurrentCellRejectedChanges(EventArgs e)
        {
            if (this.eventsTarget != null)
            {
                this.eventsTarget.OnCurrentCellRejectedChanges(e);
            }
#if DEBUG

            if (Switches.GridControlBaseEvents.TraceVerbose)
            {
                TraceUtil.TraceCurrentMethodInfo(PaneDesc, e);
            }
#else

            ;
#endif

            if (CurrentCellRejectedChanges != null)
            {
                CurrentCellRejectedChanges(this, e);
            }
        }

        /// <summary>
        /// Raises the <see cref="GridControlBase.CurrentCellEditingComplete"/> event.
        /// </summary>
        /// <param name="e">An <see cref="EventArgs" /> that contains the event data.</param>
        protected virtual void OnCurrentCellEditingComplete(EventArgs e)
        {
            if (this.eventsTarget != null)
            {
                this.eventsTarget.OnCurrentCellEditingComplete(e);
            }
#if DEBUG

            if (Switches.GridControlBaseEvents.TraceVerbose)
            {
                TraceUtil.TraceCurrentMethodInfo(PaneDesc, e);
            }
#else

            ;
#endif

            if (CurrentCellEditingComplete != null)
            {
                CurrentCellEditingComplete(this, e);
            }
        }

        /// <summary>
        /// Raises the <see cref="GridControlBase.CurrentCellDeactivated"/> event.
        /// </summary>
        /// <param name="e">A <see cref="GridCurrentCellDeactivatedEventArgs" /> that contains the event data.</param>
        protected virtual void OnCurrentCellDeactivated(GridCurrentCellDeactivatedEventArgs e)
        {
            if (this.eventsTarget != null)
            {
                this.eventsTarget.OnCurrentCellDeactivated(e);
            }
#if DEBUG

            if (Switches.GridControlBaseEvents.TraceVerbose)
            {
                TraceUtil.TraceCurrentMethodInfo(PaneDesc, e);
            }
#else

            ;
#endif

            if (CurrentCellDeactivated != null)
            {
                CurrentCellDeactivated(this, e);
            }
        }

        /// <summary>
        /// Raises the <see cref="GridControlBase.CurrentCellDeactivateFailed"/> event.
        /// </summary>
        /// <param name="e">An <see cref="EventArgs" /> that contains the event data.</param>
        protected virtual void OnCurrentCellDeactivateFailed(EventArgs e)
        {
            if (this.eventsTarget != null)
            {
                this.eventsTarget.OnCurrentCellDeactivateFailed(e);
            }
#if DEBUG

            if (Switches.GridControlBaseEvents.TraceVerbose)
            {
                TraceUtil.TraceCurrentMethodInfo(PaneDesc, e);
            }
#else

            ;
#endif

            if (CurrentCellDeactivateFailed != null)
            {
                CurrentCellDeactivateFailed(this, e);
            }
        }

        /// <summary>
        /// Raises the <see cref="GridControlBase.CurrentCellControlDoubleClick"/> event.
        /// </summary>
        /// <param name="e">An <see cref="ControlEventArgs" /> that contains the event data.</param>
        protected virtual void OnCurrentCellControlDoubleClick(ControlEventArgs e)
        {
            if (this.eventsTarget != null)
            {
                this.eventsTarget.OnCurrentCellControlDoubleClick(e);
            }

#if DEBUG
            if (Switches.KeyboardEvents.TraceVerbose || Switches.GridControlBaseEvents.TraceVerbose)
            {
                TraceUtil.TraceCurrentMethodInfo(PaneDesc, CurrentCell.ToString());
            }
#endif
            if (CurrentCellControlDoubleClick != null)
            {
                CurrentCellControlDoubleClick(this, e);
            }
        }

        /// <summary>
        /// Raises the <see cref="GridControlBase.CurrentCellControlGotFocus"/> event.
        /// </summary>
        /// <param name="e">A <see cref="ControlEventArgs" /> that contains the event data.</param>
        protected virtual void OnCurrentCellControlGotFocus(ControlEventArgs e)
        {
            if (this.eventsTarget != null)
            {
                this.eventsTarget.OnCurrentCellControlGotFocus(e);
            }

#if DEBUG
            if (Switches.KeyboardEvents.TraceVerbose || Switches.GridControlBaseEvents.TraceVerbose)
            {
                TraceUtil.TraceCurrentMethodInfo(PaneDesc, CurrentCell.ToString());
            }
#endif
            if (CurrentCellControlGotFocus != null)
            {
                CurrentCellControlGotFocus(this, e);
            }
        }

        /// <summary>
        /// Raises the <see cref="GridControlBase.CurrentCellControlLostFocus"/> event.
        /// </summary>
        /// <param name="e">A <see cref="ControlEventArgs" /> that contains the event data.</param>
        protected virtual void OnCurrentCellControlLostFocus(ControlEventArgs e)
        {
            if (this.eventsTarget != null)
            {
                this.eventsTarget.OnCurrentCellControlLostFocus(e);
            }

#if DEBUG
            if (Switches.KeyboardEvents.TraceVerbose || Switches.GridControlBaseEvents.TraceVerbose)
            {
                TraceUtil.TraceCurrentMethodInfo(PaneDesc);
            }
#endif
            if (CurrentCellControlLostFocus != null)
            {
                CurrentCellControlLostFocus(this, e);
            }

            ////            if (e.Control != null && QueryFocusInside())
            ////            {
            ////                Trace.WriteLine("Set Visble= false for " + e.Control.ToString());
            ////                if (!CurrentCell.HasControlFocus || CurrentCell.Renderer != null && CurrentCell.Renderer.Control != e.Control)
            ////                {
            ////                    //e.Control.Visible = false;
            ////                }
            ////            }
        }

        /// <summary>
        /// Determines the next position for the current cell for a given direction. Normally, cells that are not
        /// marked as enabled with <see cref="GridStyleInfo.Enabled"/> will be skipped but you can hook into this
        /// mechanism by implementing an event handler for <see cref="QueryNextCurrentCellPosition"/>.
        /// </summary>
        /// <param name="direction">The <see cref="GridDirectionType"/> that specifies the direction of the current cell movement.</param>
        /// <param name="rowIndex">The row index.</param>
        /// <param name="colIndex">The column index.</param>
        /// <returns>True if an enabled cell was found; False otherwise.</returns>
        /// <remarks>
        /// This method will raise the <see cref="GridControlBase.QueryNextCurrentCellPosition"/> event.
        /// In your event handler, you can call <see cref="GridCurrentCell.QueryNextEnabledCell"/> from your QueryNextMoveCell
        /// event handler to find out about the next enabled cell and then decide on further criteria
        /// if the suggested cell is good.
        /// <para/>
        /// See the SampleGrid class in the gridpad sample for an example.
        /// </remarks>
        public bool GetNextCurrentCellPosition(GridDirectionType direction, ref int rowIndex, ref int colIndex)
        {
            GridQueryNextCurrentCellPositionEventArgs e = new GridQueryNextCurrentCellPositionEventArgs(direction, rowIndex, colIndex);
            OnQueryNextCurrentCellPosition(e);
            if (e.Handled)
            {
                rowIndex = e.RowIndex;
                colIndex = e.ColIndex;
                return e.Result;
            }
            else
            {
                return CurrentCell.QueryNextEnabledCell(direction, ref rowIndex, ref colIndex);
            }
        }

        /// <summary>
        /// Occurs before the current cell is moved when the user navigates through the grid with arrow keys.
        /// </summary>
        /// <remarks>
        /// If you want to customize the behavior, you should manually call CurrentCell.MoveTo from within the
        /// event handler and set e.Handled = True.
        /// </remarks>
        /// <example>The following example implements wrapping the current cell to the next row at the end of a row.
        /// <code lang="C#">
        /// <para/>
        ///         private void gridControl1_MoveCurrentCellDirection(object sender, GridMoveCurrentCellDirectionEventArgs e)
        ///         {
        ///             GridControlBase grid = sender as GridControlBase;
        ///             GridModel gridModel = grid.Model;
        ///             int row = e.RowIndex;
        ///             int col = e.ColIndex;
        ///             switch (e.Direction)
        ///             {
        ///                 case GridDirectionType.Right:
        ///                 {
        ///                     col++;
        ///                     if (col > gridModel.ColCount)
        ///                     {
        ///                         row++;
        ///                         col = grid.LeftColIndex;
        ///                     }
        /// <para/>
        ///                     while (row &lt; gridModel.RowCount)
        ///                     {
        ///                         using (GridStyleInfo style = grid.GetViewStyleInfo(row, col))
        ///                         {
        ///                             if (style.Enabled)
        ///                             {
        ///                                 e.Result = grid.CurrentCell.MoveTo(row, col);
        ///                                 e.Handled = true;
        ///                                 return;
        ///                             }
        /// <para/>
        ///                             col++;
        ///                             if (col > gridModel.ColCount)
        ///                             {
        ///                                 row++;
        ///                                 col = grid.LeftColIndex;
        ///                             }
        ///                         }
        ///                     }
        ///                     e.Handled = true;
        ///                     e.Result = false;
        ///                     break;
        ///                 }
        ///                 case GridDirectionType.Left:
        ///                 {
        ///                     col--;
        ///                     if (col == gridModel.Cols.HeaderCount)
        ///                     {
        ///                         row--;
        ///                         col = gridModel.ColCount;
        ///                     }
        /// <para/>
        ///                     while (row > gridModel.Rows.HeaderCount)
        ///                     {
        ///                         using (GridStyleInfo style = grid.GetViewStyleInfo(row, col))
        ///                         {
        ///                             if (style.Enabled)
        ///                             {
        ///                                 e.Result = grid.CurrentCell.MoveTo(row, col);
        ///                                 e.Handled = true;
        ///                                 return;
        ///                             }
        /// <para/>
        ///                             col--;
        ///                             if (col == gridModel.Cols.HeaderCount)
        ///                             {
        ///                                 row--;
        ///                                 col = gridModel.ColCount;
        ///                             }
        ///                         }
        ///                     }
        ///                     e.Handled = true;
        ///                     e.Result = false;
        ///                     break;
        ///                 }
        ///             }
        /// <para/>
        ///         }
        ///     }
        /// <para/>
        /// </code>
        /// </example>
        [Category("Behavior")]
        [Description("Occurs before the current cell is moved when the user navigates through the grid with arrow keys.")]
        public event GridMoveCurrentCellDirectionEventHandler MoveCurrentCellDirection;

        /// <summary>
        /// Raises the  <see cref="MoveCurrentCellDirection"/> event.
        /// </summary>
        /// <param name="e">A <see cref="GridMoveCurrentCellDirectionEventArgs" /> that contains the event data.</param>
        protected virtual void OnMoveCurrentCellDirection(GridMoveCurrentCellDirectionEventArgs e)
        {
            if (this.eventsTarget != null)
            {
                this.eventsTarget.OnMoveCurrentCellDirection(e);
            }

            ////TraceUtil.TraceCurrentMethodInfoIf(Switches.GridControlBaseEvents.TraceVerbose, Name, e);
            if (MoveCurrentCellDirection != null)
            {
                MoveCurrentCellDirection(this, e);
            }
        }

        internal void RaiseMoveCurrentCellDirection(GridMoveCurrentCellDirectionEventArgs e)
        {
            OnMoveCurrentCellDirection(e);
        }

        /// <summary>
        /// Occurs before the the current cell is moved into a specific direction. Normally, cells that are not
        /// marked as enabled with <see cref="GridStyleInfo.Enabled"/> will be skipped but you can hook into this
        /// mechanism by implementing an event handler for <see cref="QueryNextCurrentCellPosition"/>. You should set
        /// <see cref="SyncfusionHandledEventArgs.Handled"/> to True if you handled this event.
        /// </summary>
        /// <remarks>
        /// See the SampleGrid class in the gridpad sample for an example.
        /// </remarks>
        [Category("Grid")]
        [Description("Occurs before the the current cell is moved into a specific direction.")]
        public event GridQueryNextCurrentCellPositionEventHandler QueryNextCurrentCellPosition;

        /// <summary>
        /// Raises the <see cref="QueryNextCurrentCellPosition"/> event.
        /// </summary>
        /// <param name="e">A <see cref="GridQueryNextCurrentCellPositionEventArgs" /> that contains the event data.</param>
        protected virtual void OnQueryNextCurrentCellPosition(GridQueryNextCurrentCellPositionEventArgs e)
        {
            if (this.eventsTarget != null)
            {
                this.eventsTarget.OnQueryNextCurrentCellPosition(e);
            }

            if (QueryNextCurrentCellPosition != null)
            {
                QueryNextCurrentCellPosition(this, e);
            }
        }

        GridCurrentCell gridCurrentCell;  //// No direct reference to this other than from CurrentCell and InternalSetCurrentCellObject.

        /// <summary>
        /// Gets <see cref="GridCurrentCell"/> object that provides storage for current cell information
        /// and manages all current cell operation such as activating, deactivating, saving, editing,
        /// and moving the current cell.
        /// </summary>
        /// <remarks>
        /// <see cref="CurrentCell"/> gives you a one stop interface for all current cell related operations. This is
        /// useful for an Intelli-sense-based programming environment since you do not have to dig through many unrelated
        /// functions.<para/>
        /// Events for the current cell will be raised by the <see cref="GridControlBase"/> itself. The grid offers many
        /// events for the current cell that let you customize the current cell's behavior at any stage of operation.
        /// <para/>
        /// You can find out about the current cell's position by querying the <see cref="GridCurrentCell.RowIndex"/>
        /// and <see cref="GridCurrentCell.ColIndex"/> properties of the <see cref="GridControlBase.CurrentCell"/> object
        /// in <see cref="GridControlBase"/>. The <see cref="GridCurrentCell.HasCurrentCell"/> property tells you
        /// if the grid has an active current cell. The <see cref="GridCurrentCell.HasCurrentCellAt(int)"/> method lets you
        /// find out if the current cell is at a specific row and column index.
        /// <para/>
        /// See <see cref="GridCurrentCell.MoveTo(int,int,Syncfusion.Windows.Forms.Grid.GridSetCurrentCellOptions,bool)"/> for a discussion about the
        /// order of events that you receive when the current cell is moved.
        /// </remarks>
        /// <example>
        /// The following example shows how you can customize the behavior of the current cell
        /// and highlight the whole row of the current cell instead of just the current cell itself.
        /// <code lang="C#">
        ///         /// Current cell will be moving from one position to another.
        ///         protected override void OnCurrentCellMoving(GridCurrentCellMovingEventArgs e)
        ///         {
        ///             e.Options |= GridSetCurrentCellOptions.BeginEndUpdate;
        ///             // Instead of GridSetCurrentCellOptions.BeginEndUpdate we could also
        ///             // sandwich the call in a Begin/EndUpdate pair ourselves ...
        ///             //BeginUpdate(BeginUpdateOptions.InvalidateAndScroll);
        ///         }
        /// <para/>
        ///         /// Completes a current cell's MoveTo operation indicating success.
        ///         protected override void OnCurrentCellMoved(GridCurrentCellMovedEventArgs e)
        ///         {
        ///             base.OnCurrentCellMoved(e);
        ///             //EndUpdate();
        ///         }
        /// <para/>
        ///         /// Completes a current cell's MoveTo operation indicating failure.
        ///         protected override void OnCurrentCellMoveFailed(GridCurrentCellMoveFailedEventArgs e)
        ///         {
        ///             base.OnCurrentCellMoveFailed(e);
        ///             //EndUpdate();
        ///         }
        /// <para/>
        ///         /// Highlight the current row.
        ///         protected override void OnPrepareViewStyleInfo(GridPrepareViewStyleInfoEventArgs e)
        ///         {
        ///             if (e.RowIndex > this.Model.Rows.HeaderCount &amp;&amp; e.ColIndex > this.Model.Cols.HeaderCount
        ///                 &amp;&amp; CurrentCell.HasCurrentCellAt(e.RowIndex))
        ///             {
        ///                 e.Style.Interior = new BrushInfo(SystemColors.Highlight);
        ///                 e.Style.TextColor = SystemColors.HighlightText;
        ///                 e.Style.Font.Bold = true;
        ///             }
        ///             base.OnPrepareViewStyleInfo(e);
        ///         }
        /// <para/>
        ///         /// Refresh the whole row for the old position of the current cell when it is moved to
        ///         /// a new row or when current cell is deactivated stand-alone.
        ///         protected override void OnCurrentCellDeactivated(GridCurrentCellDeactivatedEventArgs e)
        ///         {
        ///             // Check if Deactivate is called stand-alone or called from MoveTo and row is moving.
        ///             if (!CurrentCell.IsInMoveTo || CurrentCell.MoveToRowIndex != CurrentCell.MoveFromRowIndex)
        ///             {
        ///                 RefreshRange(GridRangeInfo.Row(e.RowIndex), GridRangeOptions.MergeAllSpannedCells);
        ///             }
        ///             base.OnCurrentCellDeactivated(e);
        ///         }
        /// <para/>
        ///         /// Refresh the whole row for the new current cell position when the current cell is moved
        ///         /// to a new row or when current cell is activated stand-alone (and there was no activated current cell).
        ///         protected override void OnCurrentCellActivated(EventArgs e)
        ///         {
        ///             // Check if Activate is called stand-alone or called from MoveTo and row is moving
        ///             base.OnCurrentCellActivated(e);
        ///             if (!CurrentCell.IsInMoveTo || CurrentCell.MoveToRowIndex != CurrentCell.MoveFromRowIndex
        ///                 || !CurrentCell.MoveFromActiveState)
        ///             {
        ///                 RefreshRange(GridRangeInfo.Row(CurrentCell.RowIndex), GridRangeOptions.MergeAllSpannedCells);
        ///             }
        ///         }
        /// </code>
        /// </example>
        [Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public virtual GridCurrentCell CurrentCell
        {
            get
            {
                if (gridCurrentCell == null)
                {
                    gridCurrentCell = new GridCurrentCell(this);
                }

                return gridCurrentCell;
            }
        }

        /// <summary>
        /// Replaces the internal GridCurrentCell object. This method is used by GridGroupingControl's
        /// GridNestedTableControl where one grid control is shared among multiple nested tables and
        /// these tables need to maintain their own current cell state.
        /// </summary>
        /// <param name="c">The current cell object.</param>
        public void InternalSetCurrentCellObject(GridCurrentCell c)
        {
            if (this.gridCurrentCell != c)
            {
                ////if (this.gridCurrentCell.IsInMoveTo)
                ////    throw new Exception();
#if DEBUG
                if (Switches.GridControlBaseEvents.TraceVerbose)
                {
                    TraceUtil.TraceCurrentMethodInfo();
                }
#endif
                this.gridCurrentCell = c;
            }
        }

        /// <summary>
        /// Determines if the current cell is shown at the specified row. Cell renderers call this virtual function
        /// to determine if a cell button should be shown when <see cref="GridShowButtons.ShowCurrentRow"/> is specified.
        /// </summary>
        /// <param name="rowIndex">The row index to be checked.</param>
        /// <returns>True if this row belongs the current row.</returns>
        /// <remarks>
        /// The GridDataBoundGrid overrides this method and checks whether the specified row
        /// belongs to the current record. This is not necessarily the same row since with a databound grid,
        /// a record can show values in several rows (Quicken-like display).
        /// </remarks>
        public virtual bool IsShowCurrentRow(int rowIndex)
        {
            return CurrentCell.HasCurrentCellAt(rowIndex);
        }

        ////        void HideCurrentCellIfInRange(GridRangeInfo range)
        ////        {
        ////            if (CurrentCell.HasControlFocus && range.Contains(CurrentCell.RangeInfo))
        ////            {
        ////                GridCellRendererBase pControl = CurrentCell.Renderer;
        ////                if (CurrentCell.IsEditing)
        ////                    pControl.Hide();
        ////            }
        ////        }

        ////        void ModelCurrentCellMoved(object sender, GridChangeLayoutCellsEventArgs e)
        ////        {
        ////            if (Model.Options.ShouldSynchronizeCurrentCell && !CurrentCell.HasCurrentCellAt(e.Range.Top, e.Range.Left))
        ////                CurrentCell.MoveTo(e.Range.Top, e.Range.Left, GridSetCurrentCellOptions.NoSelectRange|GridSetCurrentCellOptions.NoSetFocus|GridSetCurrentCellOptions.NoSyncCurrentCell);
        ////        }

        #endregion
        #region RepeatKey
        int m_nRepeatKeyCounter = 0;
        void ResetRepeatKeyCounter()
        {
            m_nRepeatKeyCounter = 0;
        }

        [Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        int RepeatKeyCounter
        {
            get
            {
                return m_nRepeatKeyCounter;
            }

            set
            {
                m_nRepeatKeyCounter = value;
            }
        }

        [Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        int ArrowKeyLineCount
        {
            get
            {
                if (!this.AllowIncreaseSmallChange || this.HasDoubleBufferSurface)
                {
                    return 1;
                }

                return Math.Max(1, Math.Min(4, m_nRepeatKeyCounter / 25));
            }
        }
        #endregion

        #region KeyPress
        bool wantKeys = true;

        /// <summary>
        /// Gets or sets a value indicating whether if false if you want to suppress key events for the grid. This is useful if you want to
        /// implement your own keyboard handling and not have any default key handlers in the grid.
        /// </summary>
        [Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public bool WantKeys
        {
            get
            {
                return wantKeys;
            }

            set
            {
                wantKeys = value;
            }
        }

        bool focusOnMouseDown = true;

        /// <summary>
        /// Gets or sets a value indicating whether if set to False if you want to suppress setting focus during a MouseDown event.
        /// This is useful if you have a MouseController that does not need focus.
        /// </summary>
        [Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public bool FocusOnMouseDown
        {
            get
            {
                return focusOnMouseDown;
            }

            set
            {
                focusOnMouseDown = value;
            }
        }

        /// <summary>
        /// Processes a dialog character.
        /// </summary>
        /// <param name="charCode">The character to process.</param>
        /// <returns>
        /// true if the character was processed by the control; otherwise, false.
        /// </returns>
        /// <override/>
        protected override bool ProcessDialogChar(char charCode)
        {
            if (wantKeys)
            {
#if DEBUG
                if (Switches.KeyboardEvents.TraceVerbose)
                {
                    TraceUtil.TraceCurrentMethodInfo(PaneDesc, (int)charCode);
                }
#else
                ;
#endif
                bool menuKeyDown = (Control.ModifierKeys & Keys.Alt) != Keys.None;
                switch (charCode)
                {
                    case '\b':
                        if (menuKeyDown)
                        {
                            _Undo();
                        }

                        return true;
                }
            }

            return base.ProcessDialogChar(charCode);
        }

        void _Undo()
        {
            Model.CommandStack.Undo();
        }

        bool wantsTabKey = true;

        /// <summary>
        /// Gets or sets a value indicating whether the grid control should handle tab keys to move between cells. Set this to False if focus
        /// should move to the next control in the form instead.
        /// </summary>
        [Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        [DefaultValue(true)]
        public virtual bool WantTabKey
        {
            get
            {
                return wantsTabKey;
            }

            set
            {
                wantsTabKey = value;
            }
        }

        bool wantsEnterKey = true;

        bool tabKeyNavigationInPreProcessMessage = false;

        /// <summary>
        /// Gets or sets a value indicating whether the grid control should handle tab keys to move between cells right away in the 
        /// PreProcessMessage method to make sure no other control on a form can override the tab key behavior 
        /// of this control. This is useful when the grid is hosted in an ActiveX host and the ActiveX 
        /// container does not honor the grids ProcessDialogKey override. The default value for this property is false.
        /// </summary>
        [Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        [DefaultValue(false)]
        public bool WantTabKeyInPreProcessMessage
        {
            get
            {
                return tabKeyNavigationInPreProcessMessage;
            }

            set
            {
                tabKeyNavigationInPreProcessMessage = value;
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether the grid control should handle Enter key to move between cells. Set this to False if Enter
        /// should be ignored instead.
        /// </summary>
        [Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        [DefaultValue(true)]
        public bool WantEnterKey
        {
            get
            {
                return wantsEnterKey;
            }

            set
            {
                wantsEnterKey = value;
            }
        }

        bool wantsEscapeKey = true;

        /// <summary>
        /// Gets or sets a value indicating whether the grid control should handle Escape to reset cell contents or reset
        /// cell selections. Set this to False if the Escape key should be ignored instead.
        /// </summary>
        [Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        [DefaultValue(true)]
        public bool WantEscapeKey
        {
            get
            {
                return wantsEscapeKey;
            }

            set
            {
                wantsEscapeKey = value;
            }
        }

        /// <summary>
        /// Returns the GridCurrentCell object.
        /// </summary>
        /// <returns>The <see cref="CurrentCell"/>.</returns>
        protected virtual GridCurrentCell GetCurrentCell()
        {
            return CurrentCell;
        }

        bool allowSelectNextControlinProcessDialogKey = true;

        /// <summary>Gets or sets a value indicating whether AllowSelectNextControlinProcessDialogKey. Used internally.</summary>
        /// <exclude/>
        [Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public bool AllowSelectNextControlinProcessDialogKey
        {
            get
            {
                return allowSelectNextControlinProcessDialogKey;
            }

            set
            {
                allowSelectNextControlinProcessDialogKey = value;
            }
        }

        /// <summary>
        /// Override this method to customize the processing of a tab key to move control focus.
        /// </summary>
        /// <param name="forward">True if the next Control in the Tab Order is to get focus. 
        /// False if the previous Control should receive focus.</param>
        /// <returns>True if Tabkey has been handled.</returns>
        public virtual bool ProcessTabKeyMovingFocus(bool forward)
        {
            Keys key = Keys.Tab | (forward ? Keys.None : Keys.Shift);
            return base.ProcessDialogKey(key);
        }

        /// <summary>
        /// Processes the dialog key.
        /// </summary>
        /// <param name="key">The key code.</param>
        /// <returns>return boolean value</returns>
        /// <override/>
        protected override bool ProcessDialogKey(Keys key)
        {
            if (wantKeys)
            {
                if (IsDisposed)
                {
                    return true;
                }
#if DEBUG

                if (Switches.KeyboardEvents.TraceVerbose)
                {
                    TraceUtil.TraceCurrentMethodInfo(PaneDesc, key);
                }
#else

                ;
#endif
                //// TODO: What about using DLGC_WANTSTAB instead of handling key here?
                Keys keyCode = key & Keys.KeyCode;
                bool shiftKeyDown = (Control.ModifierKeys & Keys.Shift) != Keys.None;
                bool menuKeyDown = (Control.ModifierKeys & Keys.Menu) != Keys.None;

                if (wantsTabKey && !this.tabKeyNavigationInPreProcessMessage)
                {
                    switch (keyCode)
                    {
                        case Keys.Tab:
                            if (shiftKeyDown)
                            {
                                GetCurrentCell().MoveLeft();
                            }
                            else
                            {
                                GetCurrentCell().MoveRight();
                            }

                            return true;
                    }
                }

                if (!wantsTabKey && keyCode == Keys.Tab && allowSelectNextControlinProcessDialogKey)
                {
                    ContainerControl container = (ContainerControl)GridUtil.GetParentControl(this, typeof(ContainerControl));

                    if (container != null && container.GetType().ToString() != "System.Windows.Forms.Integration.WinFormsAdapter")
                    {
                        Control active = GridUtil.GetParentControl(container.ActiveControl, typeof(GridControlBase));

                        if (active is GridControlBase)
                        {
                            if (shiftKeyDown)
                            {
                                container.TopLevelControl.SelectNextControl(active, false, true, true, true);
                                return true;
                            }
                            else
                            {
                                if (container == container.TopLevelControl && this.GridOfficeScrollBars != OfficeScrollBars.None)
                                {
                                    container.TopLevelControl.SelectNextControl(active, true, true, false, false);
                                }
                                else
                                {
                                    container.TopLevelControl.SelectNextControl(active, true, true, true, true);
                                    if (container.ActiveControl is ScrollBarCustomDraw)
                                    {
                                        container.TopLevelControl.SelectNextControl(container.ActiveControl, true, true, true, true);
                                    }
                                }
                                return true;
                            }
                        }
                    }
                }
            }

            return base.ProcessDialogKey(key);
        }

        /// <summary>
        /// Determines whether the specified key is a regular input key or a special key that requires preprocessing.
        /// </summary>
        /// <param name="keyData">One of the <see cref="T:System.Windows.Forms.Keys"/> values.</param>
        /// <returns>
        /// true if the specified key is a regular input key; otherwise, false.
        /// </returns>
        /// <override/>
        protected override bool IsInputKey(Keys keyData)
        {
            if (wantKeys)
            {
#if DEBUG
                if (Switches.KeyboardEvents.TraceVerbose)
                {
                    TraceUtil.TraceCurrentMethodInfo(PaneDesc, keyData);
                }
#else
                ;
#endif

                switch (keyData & Keys.KeyCode)
                {
                    case Keys.Tab:
                        if (wantsTabKey)
                        {
                            return (Control.ModifierKeys & (Keys.Alt | Keys.Control)) == Keys.None;
                        }

                        break;
                    case Keys.Escape:
                        return this.wantsEscapeKey;
                    case Keys.Enter:
                        return this.wantsEnterKey;
                    case Keys.Next:
                    case Keys.Prior:
                    case Keys.F2:
                    case Keys.Up:
                    case Keys.Down:
                    case Keys.Left:
                    case Keys.Right:
                    case Keys.Home:
                    case Keys.End:
                    case Keys.Insert:
                    case Keys.Delete:
                        return true; ////(Control.ModifierKeys & (Keys.Alt)) == Keys.None;
                    case Keys.Back:
                        return (Control.ModifierKeys & Keys.Control) == Keys.None;
                    //// REVIEW: Should I maybe return True here also for Enter and Escape.
                }
            }

            return base.IsInputKey(keyData);  //// Forces call to  bool ProcessKeyMessage(ref Message m)
            //// in WM_KEY/WM_CHAR window message. ProcessKeyMessage dispatches to
            //// ProcessKeyPreview(ref m) and ProcessKeyEventArgs(ref m);
            //// At last OnKeyDown, OnChar etc will be called.
        }

        /// <summary>
        /// Determines if a character is an input character that the control recognizes.
        /// </summary>
        /// <param name="charCode">The character to test.</param>
        /// <returns>
        /// true if the character should be sent directly to the control and not preprocessed; otherwise, false.
        /// </returns>
        /// <override/>
        protected override bool IsInputChar(char charCode)
        {
            if (wantKeys)
            {
#if DEBUG
                if (Switches.KeyboardEvents.TraceVerbose)
                {
                    TraceUtil.TraceCurrentMethodInfo(PaneDesc, charCode);
                }
#else
                ;
#endif
                //// Want all chars.
                return true;

                ////                bool bAlt = (Control.ModifierKeys & Keys.Alt) != Keys.None;
                ////                switch ((int) charCode)
                ////                {
                ////                    case 26:    // <CTRL>+Z
                ////                    case 18:    // <CTRL>+R
                ////                    case 3:     // <CTRL>+C
                ////                    case 22:    // <CTRL>+V
                ////                    case 24:    // <CTRL>+X
                ////                        return true;
                ////                }
            }

            return base.IsInputChar(charCode);
        }

        /// <summary>
        /// Processes a key message and generates the appropriate control events.
        /// </summary>
        /// <param name="m">A <see cref="T:System.Windows.Forms.Message"/>, passed by reference, that represents the window message to process.</param>
        /// <returns>
        /// true if the message was processed by the control; otherwise, false.
        /// </returns>
        /// <override/>
        [SecurityPermission(SecurityAction.LinkDemand, UnmanagedCode = true)]
        protected override bool ProcessKeyEventArgs(ref Message m)
        {
            if (wantKeys)
            {
                if (IsDisposed)
                {
                    return true;
                }
#if DEBUG

                if (Switches.KeyboardEvents.TraceVerbose)
                {
                    TraceUtil.TraceCurrentMethodInfo(PaneDesc, m.ToString());
                }
#else

                ;
#endif

                GridCellRendererBase renderer = CurrentCell.Renderer;

                //// If ProcessKeys was called from the grid object,
                //// give active control a chance to interpret and
                //// process the key first.

                if (renderer != null/* && this.Focused*/)
                {
                    //// KeyPressed will return true if it processed
                    //// the message.
                    if (renderer.RaiseProcessKeyEventArgs(ref m))
                    {
                        //// Make sure that cell is in visible area.
                        ////GetCurrentCell().ScrollInView();
                        ////Update();
                        return true;
                    }
                }
            }

            return base.ProcessKeyEventArgs(ref m);
        }
        /// <override/>
        protected override bool ProcessCmdKey(ref Message msg, Keys keyData)
        {
            Keys keyCode = keyData & Keys.KeyCode;
            bool controlKeyDown = (Control.ModifierKeys & Keys.Control) != Keys.None;
            if (controlKeyDown && (keyCode == Keys.C || keyCode == Keys.X))
            {

                Form frm = this.FindParentForm();
                if (frm != null && frm.IsMdiChild)
                {
                    const int WM_KEYDOWN = 0x100;
                    const int WM_SYSKEYDOWN = 0x104;

                    if ((msg.Msg == WM_KEYDOWN) || (msg.Msg == WM_SYSKEYDOWN))
                    {
                        OnCurrentCellKeyDown(new KeyEventArgs(keyData));
                        OnKeyDown(new KeyEventArgs(keyData));
                    }
                }
            }
            return base.ProcessCmdKey(ref msg, keyData);
        }

        /// <summary>
        /// When the user presses Delete the current cell renderer checks this method
        /// whether it should handle the Delete key or if Delete key should get passed
        /// onto grid. Method returns true if CurrentCell Renderer should handle Delete key,
        /// optionally raising a CurrentCellDeleting event. It returns false if grid should
        /// handle the key, possibly raising a ClearingCells event
        /// </summary>
        /// <returns>True if CurrentCell Renderer should handle Delete key, optionally raising a CurrentCellDeleting event; false if grid should handle the key, possibly raising a ClearingCells event.</returns>
        public virtual bool ShouldDeleteKeyClearCurrentCellContentsOnly()
        {
            GridRangeInfoList rl = Model.Selections.Ranges;
            /////rl.Count == 0 - no ranges of cells selected
            if (Control.ModifierKeys == Keys.None
                && (rl.Count == 0
                || (rl.Count == 1 && rl[0] == GridRangeInfo.Cell(CurrentCell.RowIndex, CurrentCell.ColIndex))))
            {
                return true;
            }

            return false;
        }

        /// <override/>
        protected override void OnKeyDown(KeyEventArgs e)
        {
            if (this.eventsTarget != null)
            {
                this.eventsTarget.OnKeyDown(e);
            }

#if DEBUG
            if (Switches.KeyboardEvents.TraceVerbose || Switches.GridControlBaseEvents.TraceVerbose)
            {
                TraceUtil.TraceCurrentMethodInfo(PaneDesc, e.Handled, e.KeyCode, e.Modifiers);
            }
#endif
            base.OnKeyDown(e);
            bool isHandled = false;
            if (wantKeys && !e.Handled && !IsDisposed)
            {
                bool isSpeedKey = false;
                Keys modifierKeys = Control.ModifierKeys;
                Keys keyCode = e.KeyCode & Keys.KeyCode;
                bool controlKeyDown = (e.Modifiers & Keys.Control) != Keys.None;
                bool shiftKeyDown = (e.Modifiers & Keys.Shift) != Keys.None;
                bool extendSelection = (modifierKeys & Keys.Shift) != Keys.None;
                bool menuKeyDown = (e.Modifiers & Keys.Menu) != Keys.None;
                bool success = false;
                int repeatCount = ArrowKeyLineCount;

                //// Is the grid in a special mode (tracking, dragging, selecting)?
                if (Capture)
                {
                    //// ... Give user a chance to abort this mode.
                    if (keyCode == Keys.Escape)
                    {
                        this.NotifyCancelMode();
                    }

                    //// Otherwise, keys cannot be processed.
                    isHandled = true;
                }
                else
                {
                    GridCellRendererBase renderer = CurrentCell.Renderer;
                    this.cancelMode = false;

                    switch (keyCode)
                    {
                        case Keys.Tab:
                            if (!controlKeyDown && !menuKeyDown && !tabKeyNavigationInPreProcessMessage)
                            {
                                if (shiftKeyDown)
                                {
                                    GetCurrentCell().MoveLeft();
                                }
                                else
                                {
                                    GetCurrentCell().MoveRight();
                                }

                                isHandled = true;
                            }

                            break;

                        case Keys.Next:
                            if (!controlKeyDown && !menuKeyDown)
                            {
                                success = CurrentCell.Move(GridDirectionType.PageDown, Math.Max(1, repeatCount), extendSelection);
                                isHandled = true;
                                isSpeedKey = Control.ModifierKeys == Keys.None && success && !cancelMode;
                            }

                            break;

                        case Keys.Prior:
                            if (!controlKeyDown && !menuKeyDown)
                            {
                                success = CurrentCell.Move(GridDirectionType.PageUp, Math.Max(1, repeatCount), extendSelection);
                                isHandled = true;
                                isSpeedKey = Control.ModifierKeys == Keys.None && success && !cancelMode;
                            }

                            break;

                        case Keys.Up:
                            if (!menuKeyDown)
                            {
                                success = CurrentCell.Move(controlKeyDown ? GridDirectionType.Top : GridDirectionType.Up, Math.Max(1, repeatCount), extendSelection);
                                isHandled = true;
                                isSpeedKey = Control.ModifierKeys == Keys.None && success && !cancelMode;
                            }

                            break;

                        case Keys.Down:
                            if (!menuKeyDown)
                            {
                                success = CurrentCell.Move(controlKeyDown ? GridDirectionType.Bottom : GridDirectionType.Down, Math.Max(1, repeatCount), extendSelection);
                                isHandled = true;
                                isSpeedKey = Control.ModifierKeys == Keys.None && success && !cancelMode;
                            }

                            break;

                        case Keys.Left:
                            if (!menuKeyDown)
                            {
                                if (this.IsRightToLeft())
                                {
                                    success = CurrentCell.Move(controlKeyDown ? GridDirectionType.MostRight : GridDirectionType.Right, Math.Max(1, repeatCount), extendSelection);
                                }
                                else
                                {
                                    success = CurrentCell.Move(controlKeyDown ? GridDirectionType.MostLeft : GridDirectionType.Left, Math.Max(1, repeatCount), extendSelection);
                                }

                                isHandled = true;
                                isSpeedKey = Control.ModifierKeys == Keys.None && success && !cancelMode;
                            }

                            break;

                        case Keys.Right:
                            if (!menuKeyDown)
                            {
                                if (this.IsRightToLeft())
                                {
                                    success = CurrentCell.Move(controlKeyDown ? GridDirectionType.MostLeft : GridDirectionType.Left, Math.Max(1, repeatCount), extendSelection);
                                }
                                else
                                {
                                    success = CurrentCell.Move(controlKeyDown ? GridDirectionType.MostRight : GridDirectionType.Right, Math.Max(1, repeatCount), extendSelection);
                                }

                                isHandled = true;
                                isSpeedKey = Control.ModifierKeys == Keys.None && success && !cancelMode;
                            }

                            break;

                        case Keys.F2:
                            if (!controlKeyDown && !menuKeyDown && !shiftKeyDown)
                            {
                                if (!CurrentCell.IsEditing)
                                {
                                    CurrentCell.BeginEdit();
                                    isHandled = true;
                                }
                                else
                                {
                                    try
                                    {
                                        CurrentCell.EndEdit();
                                        CurrentCell.Refresh();
                                        Focus();
                                        FixCurrentCellGotFocus();
                                    }
                                    catch (Exception ex)
                                    {
                                        TraceUtil.TraceExceptionCatched(ex);
                                        if (!ExceptionManager.RaiseExceptionCatched(this, ex))
                                        {
                                            throw;
                                        }

                                        bool hasFocus = this.HasControlFocus;
                                        if (ex.Message.Length > 0)
                                        {
                                            MessageBoxAdv.Show(FindFormHelper.FindForm(this), ex.Message);
                                        }

                                        if (hasFocus && !this.ContainsFocus)
                                        {
                                            Focus();
                                        }
                                    }

                                    isHandled = true;
                                }
                            }

                            break;

                        case Keys.Return:
                            if (!menuKeyDown && !controlKeyDown && !shiftKeyDown)
                            {
                                GridDirectionType nAction = Model.Options.EnterKeyBehavior;
                                if (CurrentCell.ConfirmChanges() && nAction != GridDirectionType.None)
                                {
                                    CurrentCell.Move(nAction, 1, extendSelection);
                                }
                                else if (CurrentCell.ErrorMessage.Length > 0)
                                {
                                    CurrentCell.DisplayWarningText(CurrentCell.ErrorMessage);
                                    CurrentCell.ResetError();
                                }

                                isHandled = true;
                            }

                            break;

                        case Keys.Home:
                            if (controlKeyDown && !menuKeyDown)
                            {
                                CurrentCell.Move(GridDirectionType.TopLeft, 1, extendSelection);
                                isHandled = true;
                            }

                            break;

                        case Keys.End:
                            if (controlKeyDown && !menuKeyDown)
                            {
                                CurrentCell.Move(GridDirectionType.BottomRight, 1, extendSelection);
                                isHandled = true;
                            }

                            break;

                        case Keys.Z:    //// <CTRL>+Z
                            if (controlKeyDown && !menuKeyDown)
                            {
                                _Undo();
                                isHandled = true;
                            }

                            break;

                        case Keys.Y:    //// <CTRL>+Y
                            if (controlKeyDown && !menuKeyDown)
                            {
                                Model.CommandStack.Redo();
                                isHandled = true;
                            }

                            break;

                        case Keys.R:    //// <CTRL>+R
                            if (controlKeyDown && !menuKeyDown)
                            {
                                Model.CommandStack.Redo();
                                isHandled = true;
                            }

                            break;

                        case Keys.C:     //// <CTRL>+C
                            if (controlKeyDown && !menuKeyDown)
                            {
                                if (Model.CutPaste.CanCopy())
                                {
                                    Model.CutPaste.Copy();
                                }

                                isHandled = true;
                            }

                            break;

                        case Keys.V:    //// <CTRL>+V
                            if (controlKeyDown && !menuKeyDown)
                            {
                                if (Model.CutPaste.CanPaste())
                                {
                                    if (!CurrentCell.HasCurrentCell || CurrentCell.NotifyChanging())
                                    {
                                        Model.CutPaste.Paste();
                                    }
                                }

                                isHandled = true;
                            }

                            break;

                        case Keys.X:    //// <CTRL>+X
                            if (controlKeyDown && !menuKeyDown)
                            {
                                if (Model.CutPaste.CanCut())
                                {
                                    if (!CurrentCell.HasCurrentCell || CurrentCell.NotifyChanging())
                                    {
                                        Model.CutPaste.Cut();
                                    }
                                }

                                isHandled = true;
                            }

                            break;
                        ////                    case Keys.C:
                        ////                    case Keys.V:
                        ////                    case Keys.X:
                        ////                        will be handled in OnKeyPress
                        ////                        break;

                        case Keys.Insert:
                            if (!menuKeyDown)
                            {
                                if (controlKeyDown)
                                {
                                    if (Model.CutPaste.CanCopy())
                                    {
                                        Model.CutPaste.Copy();
                                    }

                                    isHandled = true;
                                }
                                else if (shiftKeyDown)
                                {
                                    if (Model.CutPaste.CanPaste())
                                    {
                                        Model.CutPaste.Paste();
                                    }

                                    isHandled = true;
                                }
                            }

                            break;

                        case Keys.Delete:
                            if (!menuKeyDown)
                            {
                                if (shiftKeyDown)
                                {
                                    if (Model.CutPaste.CanCut())
                                    {
                                        Model.CutPaste.Cut();
                                    }

                                    isHandled = true;
                                }
                            }

                            if (!isHandled)
                            {
                                Model.Clear(controlKeyDown);
                                isHandled = true;
                            }
                            ////                                else
                            ////                                {
                            ////                                    if (CurrentCell.GetCurrentCell(out rowIndex, out colIndex) && !CurrentCell.IsActiveCurrentCell() && Model.CanClearSelection() && CurrentCell.OnDeleteCell(rowIndex, colIndex))
                            ////                                    Model.Clear(controlKeyDown);
                            ////                                    isHandled = true;
                            ////                                }
                            ////                            }
                            break;

                        case Keys.Back:
                            if (!shiftKeyDown)
                            {
                                if (controlKeyDown)
                                {
                                    Model.CommandStack.Redo();
                                    Focus();
                                    isHandled = true;
                                }
                                else if (menuKeyDown)
                                {
                                    _Undo();
                                    isHandled = true;
                                }
                            }

                            break;

                        case Keys.Escape:
                            if (WantEscapeKey && !menuKeyDown && !controlKeyDown && !shiftKeyDown)
                            {
                                if (Model.CutPaste.m_bCut)
                                {
                                    Model.CutPaste.m_bCut = false;
                                    Model.Refresh();
                                }

                                if (CurrentCell.HasCurrentCell)
                                {
                                    if (CurrentCell.IsDroppedDown)
                                    {
                                        CurrentCell.CloseDropDown(PopupCloseType.Canceled);
                                    }
                                    else
                                    {
                                        CurrentCell.RejectChanges();
                                        CurrentCell.Refresh();
                                        Focus();
                                    }

                                    CurrentCell.ScrollInView(GridScrollCurrentCellReason.KeyPress);
                                }

                                //// EXCELCURCELL
                                if (!Model.Options.ExcelLikeCurrentCell)
                                {
                                    Selections.Clear(true);
                                }

                                isHandled = true;
                            }

                            break;
                    }

                    if (isHandled)
                    {
                        Update();
                    }

                    if (isSpeedKey)
                    {
                        RepeatKeyCounter++;
                    }
                    else
                    {
                        ResetRepeatKeyCounter();
                    }
                }

                e.Handled = isHandled;
            }
        }

        /// <override/>
        protected override void OnKeyUp(KeyEventArgs e)
        {
            if (this.eventsTarget != null)
            {
                this.eventsTarget.OnKeyUp(e);
            }

#if DEBUG
            if (Switches.KeyboardEvents.TraceVerbose || Switches.GridControlBaseEvents.TraceVerbose)
            {
                TraceUtil.TraceCurrentMethodInfo(PaneDesc, e.Handled, e.KeyCode, e.Modifiers);
            }
#endif
            ResetRepeatKeyCounter();
            base.OnKeyUp(e);   //// Return True if you don't want default processing of this event.
        }

        /// <override/>
        protected override void OnKeyPress(KeyPressEventArgs e)
        {
            if (this.eventsTarget != null)
            {
                this.eventsTarget.OnKeyPress(e);
            }

#if DEBUG
            if (Switches.KeyboardEvents.TraceVerbose || Switches.GridControlBaseEvents.TraceVerbose)
            {
                TraceUtil.TraceCurrentMethodInfo(PaneDesc, e.Handled, e.KeyChar);
            }
#endif
            base.OnKeyPress(e);
            if (!e.Handled && !IsDisposed)
            {
                char charCode = e.KeyChar;

                //// Is the grid in a special mode (tracking, dragging, selecting)?
                if (Capture)
                {
                    // ... Give user a chance to abort this mode.
                    if (charCode == 27)
                    {
                        e.Handled = true;
                        this.NotifyCancelMode();
                    }

                    //// Otherwise, keys cannot be processed.

                    return;
                }

                bool bProcessed = false;

                switch ((int)charCode)
                {
                    case 9:     //// <Tab>
                    case 127:   //// <Backspace>
                    case 13:    //// <Enter>
                        bProcessed = true;
                        break;

                    ////                    case 26:    // <CTRL>+Z
                    ////                        _Undo();
                    ////                        bProcessed = true;
                    ////                        break;
                    ////
                    ////                    case 25:    // <CTRL>+Y
                    ////                        Model.CommandStack.Redo();
                    ////                        bProcessed = true;
                    ////                        break;
                    ////
                    ////                    case 18:    // <CTRL>+R
                    //////                        Model.CommandStack.Redo();
                    ////                        bProcessed = true;
                    ////                        break;
                    ////
                    ////                    case 3:     //// <CTRL>+C
                    ////                        if (Model.CutPaste.CanCopy())
                    ////                            Model.CutPaste.Copy();
                    ////                        bProcessed = true;
                    ////                        break;
                    ////
                    ////                    case 22:    //// <CTRL>+V
                    ////                        if (Model.CutPaste.CanPaste())
                    ////                        {
                    ////                            if (!CurrentCell.HasCurrentCell || CurrentCell.NotifyChanging())
                    ////                                Model.CutPaste.Paste();
                    ////                        }
                    ////                        bProcessed = true;
                    ////                        break;
                    ////
                    ////                    case 24:    //// <CTRL>+X
                    ////                        if (Model.CutPaste.CanCut())
                    ////                        {
                    ////                            if (!CurrentCell.HasCurrentCell || CurrentCell.NotifyChanging())
                    ////                                Model.CutPaste.Cut();
                    ////                        }
                    ////                        bProcessed = true;
                    ////                        break;
                }

                e.Handled = bProcessed;
            }
        }

        ////bool inPreProcessMessageTabKey = false;

        /// <override/>
        /// <summary>
        /// Preprocesses keyboard or input messages within the message loop before they are dispatched.
        /// </summary>
        /// <param name="msg">A <see cref="Message"/> to process.</param>
        /// <returns>True if preprocessed.</returns>
        public override bool PreProcessMessage(ref Message msg)
        {
            if (tabKeyNavigationInPreProcessMessage && HandleTabKeyPreProcessMessage(ref msg))
            {
                return true;
            }

            if (msg.Msg == NativeMethods.WM_KEYDOWN)
            {
                //// Avoid Control.ProcessUICues being called from Control.PreProcessMessage if grid handles TAB key itself. This is only needed
                //// if TAB is forwarded to form
                if ((int)msg.WParam == 0x09 && this.WantTabKey)
                {
                    ////inPreProcessMessageTabKey = true;
                    try
                    {
                        return base.PreProcessMessage(ref msg);
                    }
                    finally
                    {
                        ////inPreProcessMessageTabKey = false;
                    }
                }
            }

            return base.PreProcessMessage(ref msg);
        }

        /// <summary>
        /// Previews a keyboard message.
        /// </summary>
        /// <param name="m">A <see cref="T:System.Windows.Forms.Message"/>, passed by reference, that represents the window message to process.</param>
        /// <returns>
        /// true if the message was processed by the control; otherwise, false.
        /// </returns>
        /// <override/>
        [SecurityPermission(SecurityAction.LinkDemand, UnmanagedCode = true)]
        protected override bool ProcessKeyPreview(ref Message m)
        {
            if (IsDisposed)
            {
                return true;
            }
#if DEBUG

            if (Switches.KeyboardEvents.TraceVerbose)
            {
                TraceUtil.TraceCurrentMethodInfo(PaneDesc, m.ToString());
            }
#else

            ;
#endif
            if (((Keys)((int)m.WParam & (int)Control.ModifierKeys) == Keys.Enter && this.WantEnterKey)
                || ((Keys)((int)m.WParam & (int)Control.ModifierKeys) == Keys.Escape && this.WantEscapeKey))
            {
                if (m.Msg == 0x102/*NativeMethods.WM_CHAR*/)
                {
                    return true;
                }
                else
                {
                    return this.ProcessKeyEventArgs(ref m);
                }
            }

            if (m.Msg == 0x102/*NativeMethods.WM_CHAR*/)
            {
                return this.ProcessKeyEventArgs(ref m);
            }

            if ((Control.ModifierKeys & Keys.Control) != Keys.None)
            {
                return this.ProcessKeyEventArgs(ref m);
            }

            Keys keyCode = (Keys)((int)m.WParam & (int)Keys.KeyCode);
            if (IsInputKey(keyCode))
            {
                return this.ProcessKeyEventArgs(ref m);
            }

            return base.ProcessKeyPreview(ref m);
        }

        /// <summary>
        /// Processes a keyboard message.
        /// </summary>
        /// <param name="m">A <see cref="T:System.Windows.Forms.Message"/>, passed by reference, that represents the window message to process.</param>
        /// <returns>
        /// true if the message was processed by the control; otherwise, false.
        /// </returns>
        /// <override/>
        protected override bool ProcessKeyMessage(ref Message m)
        {
#if DEBUG
            if (Switches.KeyboardEvents.TraceVerbose)
            {
                TraceUtil.TraceCurrentMethodInfo(PaneDesc, m.ToString());
            }
#else
            ;
#endif
            return base.ProcessKeyMessage(ref m);
        }
        #endregion

        #region ControlPool
        /// <summary>
        /// Gets the cell renderer at the specified row and column.
        /// </summary>
        /// <param name="rowIndex">The row index.</param>
        /// <param name="colIndex">The column index.</param>
        /// <returns>The cell renderer for that cell.</returns>
        public GridCellRendererBase GetCellRenderer(int rowIndex, int colIndex)
        {
            GridStyleInfo style = GetViewStyleInfo(rowIndex, colIndex);
            GridCellRendererBase rb = CellRenderers[style.CellType];
            style.Dispose();
            return rb;
        }

        GridCellRendererCollection cellRenderers = null;

        /// <summary>
        /// Gets the collection of <see cref="GridCellRendererBase"/> objects for the current grid view
        /// method.
        /// </summary>
        /// <remarks>
        /// Cell renderers will be created on demand by calling the <see cref="GridCellModelBase.CreateRenderer"/>.
        /// Each renderer is associated with a <see cref="GridCellModelBase"/> object that holds its data and has
        /// knowledge how to instantiate a renderer and associates it with a grid view.
        /// <para/>
        /// A renderer is created for each grid view but
        /// renderers (of the same cell type) share the same <see cref="GridCellModelBase"/> instance even though they belong
        /// to different grid views.
        /// </remarks>
        /// <example>
        /// The following examples show how to get a reference to the renderer for a specific cell.
        /// <code lang="C#">
        ///             GridStyleInfo style = Model[rowIndex, colIndex];
        ///             GridCellRendererBase renderer = CellRenderers[style.CellType];
        /// </code>
        /// </example>
        [Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public GridCellRendererCollection CellRenderers
        {
            get
            {
                if (cellRenderers == null)
                {
                    cellRenderers = new GridCellRendererCollection(this);
                }

                return cellRenderers;
            }
        }
        
        #endregion
        #region Updating
        ////        GridRangeInfo combinedChangeLayoutCells = GridRangeInfo.Empty;
        ////        int notifyChangeLayoutCells = 0;
        ////        public event GridChangeLayoutCellsEventHandler ChangingLayoutCells;
        ////        public event GridChangeLayoutCellsEventHandler ChangedLayoutCells;
        ////
        ////        void ModelChangingLayoutCells(object sender, GridChangeLayoutCellsEventArgs e)
        ////        {
        ////            TraceUtil.TraceCurrentMethodInfo(e);
        ////            NotifyChangingLayoutCells(e.Range);
        ////        }
        ////
        void ModelChangedLayoutCells(object sender, GridChangeLayoutCellsEventArgs e)
        {
#if DEBUG
            if (Switches.CurrentCell.TraceVerbose)
            {
                TraceUtil.TraceCurrentMethodInfo(PaneDesc, e);
            }
#else
            ;
#endif
            ////NotifyChangedLayoutCells();
            UpdateScrollBars();
        }
        ////
        ////        public void NotifyChangingLayoutCells(GridRangeInfo range)
        ////        {
        ////            if (notifyChangeLayoutCells++ == 0)
        ////                combinedChangeLayoutCells = range;
        ////            else
        ////                combinedChangeLayoutCells = combinedChangeLayoutCells.UnionRange(range);
        ////            OnChangingLayoutCells(new GridChangeLayoutCellsEventArgs(combinedChangeLayoutCells));
        ////        }
        ////
        ////        protected virtual void OnChangingLayoutCells(GridChangeLayoutCellsEventArgs e)
        ////        {
        ////            Trace.WriteLine("ScrollContol.OnChangingLayoutCells(" + e.ToString() + ")");
        ////            if (ChangingLayoutCells != null)
        ////            {
        ////                try
        ////                {
        ////                    ChangingLayoutCells(this, e);
        ////                }
        ////                catch (Exception ex)
        ////                {
        ////                    TraceUtil.TraceExceptionCatched(ex);
        ////                }
        ////            }
        ////        }
        ////
        ////        public void NotifyChangedLayoutCells()
        ////        {
        ////            if (--notifyChangeLayoutCells == 0)
        ////            {
        ////                OnChangedLayoutCells(new GridChangeLayoutCellsEventArgs(combinedChangeLayoutCells));
        ////                combinedChangeLayoutCells = GridRangeInfo.Empty;
        ////            }
        ////        }
        ////
        ////        protected virtual void OnChangedLayoutCells(GridChangeLayoutCellsEventArgs e)
        ////        {
        ////            Trace.WriteLine("ScrollContol.OnChangedLayoutCells(" + e.ToString() + ")");
        ////            if (ChangingLayoutCells != null)
        ////            {
        ////                try
        ////                {
        ////                    ChangedLayoutCells(this, e);
        ////                }
        ////                catch (Exception ex)
        ////                {
        ////                    TraceUtil.TraceExceptionCatched(ex);
        ////                }
        ////            }
        ////        }

        //// update
        void ModelBeginUpdateRequest(object sender, EventArgs e)
        {
#if DEBUG
            if (Switches.BeginEndUpdate.TraceVerbose)
            {
                TraceUtil.TraceCurrentMethodInfo(PaneDesc, e);
            }
#else
            ;
#endif
            BeginUpdateModel(Model.UpdateOptions, true);
        }

        void ModelEndUpdateRequest(object sender, GridEndUpdateRequestEventArgs e)
        {
#if DEBUG
            if (Switches.BeginEndUpdate.TraceVerbose)
            {
                TraceUtil.TraceCurrentMethodInfo(PaneDesc, e);
            }
#else
            ;
#endif
            EndUpdateModel(e.Value, true);
        }

        private Rectangle invalidRect;
        ////        private bool savedDisableScrollWindow = false;

        /// <summary>
        /// Gets or sets the outer rectangle after a batch of Invalid calls after a BeginUpdate call. This
        /// is for internal use only.
        /// </summary>
        [Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public Rectangle InvalidBounds
        {
            get
            {
                return Updating ? invalidRect : Rectangle.Empty;
            }

            set
            {
                invalidRect = value;
            }
        }

        /// <overload>
        /// Suspends the painting of the control until the <see cref="ScrollControl.EndUpdate()"/> method is called.
        /// </overload>
        /// <summary>
        /// Suspends the painting of the control until the <see cref="ScrollControl.EndUpdate()"/> method is called.
        /// </summary>
        /// <param name="options">Specifies the painting support during the BeginUpdate, EndUpdate batch.</param>
        /// <remarks>
        /// <para>When many paint are made to the appearance of a control, you should invoke the
        /// BeginUpdate method to temporarily freeze the drawing of the control. This results
        /// in less distraction to the user, and a performance gain. After all updates have
        /// been made, invoke the EndUpdate method to resume drawing of the control.</para>
        /// <para>
        /// Pass BeginUpdateOptions if you do not want to do a complete Refresh of the control and instead
        /// want to have certain regions of your control be invalidated or scroll the contents of control.</para>
        /// If you call BeginUpdate() and then later EndUpdate(), the control will know if a paint is pending and only
        /// refresh the control if a paint is pending. Either call to ShouldPrepareUpdate, Invalidate or a WM_PAINT message during
        /// the BeginUpdate EndUpdate block will signal the control that a paint is pending.
        /// </remarks>
        /// <seealso cref="ScrollControl.ShouldPrepareUpdate()"/>
        /// <seealso cref="ScrollControl.EndUpdate()"/>
        [EditorBrowsable(EditorBrowsableState.Advanced)]
        public override sealed void BeginUpdate(BeginUpdateOptions options)
        {
            if (this.IsWindowless)
            {
                ((GridControlBase)ParentSite).BeginUpdate(options);
                return;
            }

            BeginUpdateModel(options, false);
        }

        /// <summary>
        /// Suspends the painting of the control until the <see cref="EndUpdate"/> method is called.
        /// </summary>
        /// <param name="options">Specifies the painting support during the BeginUpdate, EndUpdate batch.</param>
        /// <param name="fromModel">Specifies if this BeginUpdate call was triggered by a call to the <see cref="GridModel.BeginUpdate()"/>
        /// of the <see cref="GridModel"/>.</param>
        [EditorBrowsable(EditorBrowsableState.Advanced)]
        public virtual void BeginUpdateModel(BeginUpdateOptions options, bool fromModel)
        {
            //// TODO: Would it make sense to work on Excel Frame here?
            ////            if (!Updating)
            ////                ResetSelectionFrame(null);
            //// and repaint it in EndUpdate() or WmPaint() ?
#if DEBUG

            if (Switches.BeginEndUpdate.TraceVerbose)
            {
                TraceUtil.TraceCurrentMethodInfo(PaneDesc, options, fromModel);
            }
#else

            ;
#endif
            if (this.IsWindowless)
            {
                ((GridControlBase)ParentSite).BeginUpdateModel(options, fromModel);
                return;
            }

            if (!PaintPending)
            {
                invalidRect = Rectangle.Empty;
            }

            base.BeginUpdate(options);
        }

        /// <overload>
        /// Resumes the painting of the control suspended by calling the BeginUpdate method.
        /// </overload>
        /// <summary>
        /// Resumes the painting of the control suspended by calling the BeginUpdate method.
        /// </summary>
        /// <param name="update">True if pending paint operations should be executed immediately; False if they should be discarded.</param>
        /// <remarks>
        /// When many paint are made to the appearance of a control, you should invoke the
        /// BeginUpdate method to temporarily freeze the drawing of the control. This results
        /// in less distraction to the user, and a performance gain. After all updates have
        /// been made, invoke the EndUpdate method to resume drawing of the control.
        /// </remarks>
        /// <seealso cref="ScrollControl.BeginUpdate()"/>
        public override sealed void EndUpdate(bool update)
        {
            if (this.IsWindowless)
            {
                ((GridControlBase)ParentSite).EndUpdate(update);
                return;
            }

            EndUpdateModel(update, false);
        }

        ////        protected override void OnEndUpdateScrollBars()
        ////        {
        ////            base.OnEndUpdateScrollBars();
        ////            UpdateScrollBars();
        ////        }

        /// <override/>
        /// <summary>
        /// Causes the control to redraw the invalidated regions within its client area.
        /// </summary>
        public new virtual void Update()
        {
            if (firstPaint || inOnPaint || Updating)
            {
                return;
            }

            if (this.IsWindowless)
            {
                GetGridWindow().Update();
            }
            else
            {
                base.Update();
            }
        }

        /// <summary>
        /// Resumes the painting of the control suspended by calling the BeginUpdate method.
        /// </summary>
        /// <param name="update">True if pending paint operations should be executed immediately; False if they should be discarded.</param>
        /// <param name="fromModel">Specified if this EndUpdate call was triggered by a call to the <see cref="GridModel.EndUpdate()"/>
        /// of the <see cref="GridModel"/></param>
        public virtual void EndUpdateModel(bool update, bool fromModel)
        {
#if DEBUG
            if (Switches.BeginEndUpdate.TraceVerbose)
            {
                TraceUtil.TraceCurrentMethodInfo(PaneDesc, update, fromModel);
            }
#else
            ;
#endif

            bool paintPend = PaintPending || !invalidRect.IsEmpty;

            if (this.IsWindowless)
            {
                ((GridControlBase)ParentSite).EndUpdateModel(update, fromModel);
                return;
            }

            base.EndUpdate(false);

            if (CurrentCell.StaticDrawing)
            {
                return;
            }

            if (!Updating && paintPend)
            {
                Rectangle rect = GridBounds;

                int rowGuess = rect.Height;
                if (DefaultRowHeight > 0)
                {
                    rowGuess = rect.Height / DefaultRowHeight;
                }

                int colGuess = rect.Width;
                if (DefaultColWidth > 0)
                {
                    colGuess = rect.Width / DefaultColWidth;
                }

                Model.RaiseQueryMaximumRowCol(
                    ViewLayout.LastVisibleRow + rowGuess,
                    ViewLayout.LastVisibleCol + colGuess);

                this.DisableScrollWindow = false;
                //// Do more stuff with InvalidBounds, e.g. EvaluateFloatingCells etc.

                int nfrow = GetFirstScrollableRow();
                if (this.TopRowIndex < nfrow)
                {
                    ScrollGrid.m_nTopRow = nfrow;
                    ViewLayout.Reset();
                }

                int nfcol = GetFirstScrollableCol();
                if (this.LeftColIndex < nfcol)
                {
                    ScrollGrid.m_nLeftCol = nfcol;
                    ViewLayout.Reset();
                }

                OnBeforePaint(EventArgs.Empty);

                //// Now, I am sure that GetRowCount and GetColCount does not return
                //// too large values.
                if (update)
                {
                    Update();
                }

                if (this.recalcScrollBars != ScrollBars.None)
                {
                    UpdateScrollBars();
                }

                recalcScrollBars = ScrollBars.None;
            }
        }

        /// <internalonly/>
        [Syncfusion.Documentation.DocumentationExclude()]
        protected virtual void IntUpdateSelectRange()
        {
        }

        /// <internalonly/>
        /// <summary>Gets PaintSelectCells. Used internally.</summary>
        [Syncfusion.Documentation.DocumentationExclude()]
        [Browsable(false)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public virtual IGridPaintSelectCells PaintSelectCells
        {
            get
            {
                return null;
            }
        }

        /// <summary>
        ///   <para>Raises the <see cref="Control.Invalidated" /> event.</para>
        /// </summary>
        /// <param name="e">An <see cref="InvalidateEventArgs" /> that contains the event data.</param>
        protected override void OnInvalidated(InvalidateEventArgs e)
        {
            if (this.eventsTarget != null)
            {
                this.eventsTarget.OnInvalidated(e);
            }

            if (this.IsSplitterPaneClosing)
            {
                return;
            }
#if DEBUG

            if (Switches.GridControlBaseEvents.TraceVerbose)
            {
                TraceUtil.TraceCurrentMethodInfo(PaneDesc, e.InvalidRect);
            }
#else

            ;
#endif
            ////            Trace.WriteLine("GridControlBase.OnInvalidated(" + e.InvalidRect.ToString() + ")");

            if (CurrentCell.StaticDrawing)
            {
                Rectangle r = e.InvalidRect;
                if (CurrentCell.StaticRenderControl != null)
                {
                    CurrentCell.StaticRenderControl.Invalidate(r);
                }
            }

            ////if (true)
            {
                if (invalidRect.IsEmpty)
                {
                    invalidRect = e.InvalidRect;
                }
                else
                {
                    invalidRect = Rectangle.Union(invalidRect, e.InvalidRect);
                }

                //// TODO: Could also do EvalDelayedFloatCells here.
            }

            if (this.IsWindowless)
            {
                ////TraceUtil.TraceCurrentMethodInfo("Windowless ...", e.InvalidRect);
                GetGridWindow().Invalidate(e.InvalidRect);
            }

            if (traceInvalidate)
            {
                TraceUtil.TraceCurrentMethodInfo(e.InvalidRect);
                TraceUtil.TraceCalledFrom(10);
            }

            base.OnInvalidated(e);
        }

        private GridViewLayout viewLayout = null;

        /// <summary>
        /// Gets layout information about the grid such as cell positions, number of visible rows,
        /// and more. Provides functions that let you find a cell under a given point.
        /// </summary>
        [Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public GridViewLayout ViewLayout
        {
            get
            {
                if (viewLayout == null)
                {
                    viewLayout = new GridViewLayout(this);
                    this.subComponents.Add(viewLayout);
                }

                return viewLayout;
            }
        }

        /// <internalonly/>
        [Syncfusion.Documentation.DocumentationExclude()]
        protected internal ScrollBars recalcScrollBars = ScrollBars.None;

        bool inBeforePaint = false;

        /// <overload>
        /// Calculates a range of cells that is displayed at the specified area.
        /// </overload>
        /// <summary>
        /// Calculates a range of cells that is displayed at the specified area.
        /// </summary>
        /// <param name="rect">A <see cref="Rectangle"/> that specifies the area.</param>
        /// <returns>A <see cref="GridRangeInfo"/> with cells that are displayed at the given area.
        /// If no cells are found, <see cref="GridRangeInfo.Empty"/> is returned.</returns>
        /// <remarks>
        /// The <see cref="Rectangle"/> is given in client coordinates.
        /// <para/>
        /// Points that fall below the last visible row or column will return the index of the last visible row or column.
        /// </remarks>
        public GridRangeInfo RectangleToRangeInfo(Rectangle rect)
        {
            return RectangleToRangeInfo(rect, 0);
        }

        /// <summary>
        /// Calculates a range of cells that is displayed at the specified area.
        /// </summary>
        /// <param name="rect">A <see cref="Rectangle"/> that specifies the area.</param>
        /// <param name="fixOutOfRange">Defines out of range behavior.
        /// <para/>
        /// </param>
        /// <returns>A <see cref="GridRangeInfo"/> with cells that are displayed at the given area.
        /// If no cells are found, <see cref="GridRangeInfo.Empty"/> is returned.</returns>
        /// <remarks>
        /// The <see cref="Rectangle"/> is given in client coordinates.
        /// <para/>
        /// Points that fall below the last visible row or right of the last column will be adjusted as defined in the
        /// fixOutOfRange parameter.
        /// <list type="bullet">
        /// <item><term>Use -1 if <see cref="GridRangeInfo.Empty"/> should be returned.</term></item>
        /// <item><term>Use 0 if the index of the last visible row or column should be used.</term></item>
        /// <item><term>Use 1 if the index of the last visible row or column should be used but 1 should be added.</term></item>
        /// </list>
        /// </remarks>
        public GridRangeInfo RectangleToRangeInfo(Rectangle rect, int fixOutOfRange)
        {
            GridRangeInfo tl = PointToRangeInfo(rect.Location, fixOutOfRange);
            if (tl.IsEmpty)
            {
                return tl;
            }

            GridRangeInfo br = PointToRangeInfo(new Point(rect.Right - 1, rect.Bottom - 1), Math.Max(0, fixOutOfRange));
            return GridRangeInfo.UnionRange(tl, br);
        }

        /// <overload>
        /// Calculates the cell that is displayed at a specific point.
        /// </overload>
        /// <summary>
        /// Calculates the cell that is displayed at a specific point.
        /// </summary>
        /// <param name="point">The <see cref="Point"/> in client coordinates.</param>
        /// <returns>A <see cref="GridRangeInfo"/> with the cell that is displayed at the point.
        /// If no cell is found, <see cref="GridRangeInfo.Empty"/> is returned.</returns>
        /// <remarks>
        /// The <see cref="Point"/> is given in client coordinates.
        /// <para/>
        /// Points that fall below the last visible row or column will return the index of the last visible row or column.
        /// </remarks>
        public GridRangeInfo PointToRangeInfo(Point point)
        {
            return PointToRangeInfo(point, 0);
        }

        /// <overload>
        /// Calculates the row and column index for a cell that is displayed at a specific point.
        /// </overload>
        /// <summary>
        /// Calculates the row and column index for a cell that is displayed at a specific point.
        /// </summary>
        /// <param name="point">The <see cref="Point"/> in client coordinates.</param>
        /// <param name="rowIndex">The row index.</param>
        /// <param name="colIndex">The column index.</param>
        /// <returns>True if cell is found; False otherwise.</returns>
        /// <remarks>
        /// The <see cref="Point"/> is given in client coordinates.
        /// <para/>
        /// Points that fall below the last visible row or column will return the index of the last visible row or column.
        /// </remarks>
        public bool PointToRowCol(Point point, out int rowIndex, out int colIndex)
        {
            return PointToRowCol(point, out rowIndex, out colIndex, -1);
        }

        /// <summary>
        /// Calculates the row and column index for a cell that is displayed at a specific point.
        /// </summary>
        /// <param name="point">The <see cref="Point"/> in client coordinates.</param>
        /// <param name="rowIndex">The row index.</param>
        /// <param name="colIndex">The column index.</param>
        /// <param name="fixOutOfRange">Defines out of range behavior.
        /// </param>
        /// <returns>True if cell is found; False otherwise.</returns>
        /// <remarks>
        /// The <see cref="Point"/> is given in client coordinates.
        /// <para/>
        /// Points that below the last visible row or right of the last column will be adjusted as defined in the
        /// fixOutOfRange parameter.
        /// <list type="bullet">
        /// <item><term>Use -1 if <see cref="GridRangeInfo.Empty"/> should be returned.</term></item>
        /// <item><term>Use 0 if the index of the last visible row or column should be used.</term></item>
        /// <item><term>Use 1 if the index of the last visible row or column should be used but 1 should be added.</term></item>
        /// </list>
        /// </remarks>
        public bool PointToRowCol(Point point, out int rowIndex, out int colIndex, int fixOutOfRange)
        {
            GridRangeInfo range = PointToRangeInfo(point, fixOutOfRange);
            if (range.IsEmpty)
            {
                rowIndex = colIndex = -1;
                return false;
            }
            else
            {
                rowIndex = range.Top;
                colIndex = range.Left;
                return true;
            }
        }

        /// <summary>
        /// Calculates the cell that is displayed at a specific point.
        /// </summary>
        /// <param name="point">The <see cref="Point"/> in client coordinates.</param>
        /// <param name="fixOutOfRange">Defines out of range behavior.
        /// </param>
        /// <returns>A <see cref="GridRangeInfo"/> with the cell that is displayed at the point.
        /// If no cell is found, <see cref="GridRangeInfo.Empty"/> is returned.</returns>
        /// <remarks>
        /// The <see cref="Point"/> is given in client coordinates.
        /// <para/>
        /// Points that fall below the last visible row or right of the last column will be adjusted as defined in the
        /// fixOutOfRange parameter.
        /// <list type="bullet">
        /// <item><term>Use -1 if <see cref="GridRangeInfo.Empty"/> should be returned.</term></item>
        /// <item><term>Use 0 if the index of the last visible row or column should be used.</term></item>
        /// <item><term>Use 1 if the index of the last visible row or column should be used but 1 should be added.</term></item>
        /// </list>
        /// </remarks>
        public GridRangeInfo PointToRangeInfo(Point point, int fixOutOfRange)
        {
            ////int x, y;
            if (fixOutOfRange >= 0)
            {
                point.X = Math.Max(0, point.X);
                point.Y = Math.Max(0, point.Y);
            }

            int colIndex = ViewLayout.PointToClientCol(point, GridCellSizeKind.VisibleSize);
            int rowIndex = ViewLayout.PointToClientRow(point, GridCellSizeKind.VisibleSize);
            int clientRows = ViewLayout.VisibleRows;
            int clientCols = ViewLayout.VisibleCols;
            int rowCount = ViewLayout.LastVisibleRow;
            int colCount = ViewLayout.LastVisibleCol;
            if (fixOutOfRange >= 0)
            {
                rowIndex = Math.Max(0, rowIndex);
                colIndex = Math.Max(0, colIndex);
                return GridRangeInfo.Cell(
                    rowIndex >= clientRows ? rowCount + fixOutOfRange : GetRow(rowIndex),
                    colIndex >= clientCols ? colCount + fixOutOfRange : GetCol(colIndex));
            }
            else if (rowIndex < clientRows && rowIndex >= 0
                && colIndex < clientCols && colIndex >= 0)
            {
                return GridRangeInfo.Cell(GetRow(rowIndex), GetCol(colIndex));
            }

            return GridRangeInfo.Empty;
        }

        /// <overload>
        /// Calculates the display area for a given range of cells.
        /// </overload>
        /// <summary>
        /// Calculates the display area for a given range of cells.
        /// </summary>
        /// <param name="range">The <see cref="GridRangeInfo"/> with the range of cells.</param>
        /// <returns>A <see cref="Rectangle"/> that spans the range of visible cells. If no cells in the given range
        /// are visible, <see cref="Rectangle.Empty"/> is returned.</returns>
        /// <remarks>
        /// If there are covered cells or floating cells, they will treated as regular cells. The range is not enlarged
        /// to fit these spanned cells.</remarks>
        public Rectangle RangeInfoToRectangle(GridRangeInfo range)
        {
            return ViewLayout.RangeInfoToRectangle(range, GridCellSizeKind.VisibleSize);
        }

        /// <summary>
        /// Calculates the display area for a given range of cells.
        /// </summary>
        /// <param name="range">The <see cref="GridRangeInfo"/> with the range of cells.</param>
        /// <param name="options">A <see cref="GridRangeOptions"/> value specifies how to handle spanned cells and also
        /// how to handle ranges that are outside the current visible area.</param>
        /// <returns>A <see cref="Rectangle"/> spans the range of visible cells. If no cells in the given range
        /// are visible, <see cref="Rectangle.Empty"/> is returned.</returns>
        /// <remarks>
        /// If there are covered cells or floating cells, they will be treated as specified with the "options" parameter:
        /// <list type="table">
        /// <listheader><term>Items</term><description>Descriptions</description></listheader>
        /// <item><term>None </term><description>Use range as specified.</description></item>
        /// <item><term>MergeCoveredCells </term><description> Enlarge range with any covered cells that intersect with the original range.</description></item>
        /// <item><term>MergeFloatedCells </term><description>Enlarge range with any floating cells that intersect with the original range.</description></item>
        /// <item><term>MergeMergedCells </term><description>Enlarge range with any merged cells that intersect with the original range.</description></item>
        /// <item><term>MergeAllSpannedCells </term><description>Enlarge range with any merged cells (not implemented, reserved for future use) that intersect with the original range.</description></item>
        /// <item><term>CalculateNonClientArea </term><description> Included are outside of the current visible grid view. Otherwise <see cref="GridControlBase.RangeInfoToRectangle(Syncfusion.Windows.Forms.Grid.GridRangeInfo)"/> will ignore cells that are not visible.</description></item>
        /// </list>
        /// </remarks>
        public Rectangle RangeInfoToRectangle(GridRangeInfo range, GridRangeOptions options)
        {
            if (range.IsEmpty)
            {
                return Rectangle.Empty;
            }
            else
            {
                GridRowColRangeInfoHandler[] handler = new GridRowColRangeInfoHandler[3];
                if ((options & GridRangeOptions.MergeCoveredCells) != 0)
                {
                    handler[0] = new GridRowColRangeInfoHandler(Model.CoveredRanges.FindRange);
                }

                if ((options & GridRangeOptions.MergeFloatedCells) != 0)
                {
                    handler[1] = new GridRowColRangeInfoHandler(Model.FloatingCells.FindRange);
                }

                if ((options & GridRangeOptions.MergeMergedCells) != 0)
                {
                    handler[2] = new GridRowColRangeInfoHandler(Model.MergeCells.FindRange);
                }

                if ((options & GridRangeOptions.MergeBanneredCells) != 0)
                {
                    handler[2] = new GridRowColRangeInfoHandler(Model.BanneredRanges.FindRange);
                }

                range = ViewLayout.VisitVisibleCells(range, handler);
            }

            if ((options & GridRangeOptions.CalculateNonClientArea) == GridRangeOptions.None)
            {
                return ViewLayout.RangeInfoToRectangle(range, true, GridCellSizeKind.VisibleSize);
            }
            else
            {
                return ViewLayout.RangeInfoToRectangle(range, false, GridCellSizeKind.ActualSize);
            }
        }

        /// <internalonly/>
        [Syncfusion.Documentation.DocumentationExclude()]
        protected internal virtual void GetScrollOutOfViewCurrentCellState(out GridCellRendererBase cellRenderer, out Control cellControl, out Rectangle currentCellBounds, out int currentCellRowIndex, out int currentCellColIndex, GridDirectionType direction)
        {
            cellRenderer = null;
            currentCellBounds = Rectangle.Empty;
            cellControl = null;

            if (CurrentCell.GetCurrentCell(out currentCellRowIndex, out currentCellColIndex))
            {
                cellRenderer = CurrentCell.Renderer;
                GridCurrentCell ncc = cellRenderer.GetNestedCurrentCell();
                if (ncc == null)
                {
                    cellRenderer = null;
                }
                else if (ncc.Renderer != null)
                {
                    cellRenderer = ncc.Renderer;

                    //// When you click inside a cell of a nested table 
                    //// and then scroll the focus is not taken away from 
                    //// the textbox and it will partly draw over the frozen 
                    //// cells and leave a trail when scrolled back.
                    //// The reason for this problem is that DoPixelScroll
                    //// checks for grid.InternalIsFrozenCol(currentCellColIndex)
                    //// and in the case for nested tables this method returns
                    //// then CurrentCell ColumnIndex of the nested table in the
                    //// parent table. Therefore we fix this problem by setting
                    //// currentCellColIndex = 0 for that specific instance
                    //// and grid.InternalIsFrozenCol(currentCellColIndex) will
                    //// then properly return false.

                    if (direction == GridDirectionType.Right || direction == GridDirectionType.Left)
                    {
                        currentCellColIndex = 0;
                    }
                    else if (direction == GridDirectionType.Up || direction == GridDirectionType.Down)
                    {
                        currentCellRowIndex = 0;
                    }
                }

                if (cellRenderer != null)
                {
                    currentCellBounds = cellRenderer.Grid.RangeInfoToRectangle(ncc.RangeInfo, GridRangeOptions.CalculateNonClientArea | GridRangeOptions.MergeCoveredCells);

                    cellControl = cellRenderer.Control;
                }
            }
        }

        /// <summary>
        /// Called before the Paint method is painting the grid.
        /// </summary>
        /// <param name="e">Empty event args.</param>
        protected virtual void OnBeforePaint(EventArgs e)
        {
#if DEBUG
            if (Switches.GridPaint.TraceVerbose)
            {
                TraceUtil.TraceCurrentMethodInfo(PaneDesc, "Begin", invalidRect, inBeforePaint);
            }
#else
            ;
#endif

            Trace.Indent();
            if (!invalidRect.IsEmpty && !inBeforePaint)
            {
                Rectangle savedRect = invalidRect;
                invalidRect.Intersect(ClientRectangle);
                inBeforePaint = true;

                base.BeginUpdate(BeginUpdateOptions.None);
                try
                {
                    int nfrow = GetFirstScrollableRow();
                    if (this.TopRowIndex < nfrow)
                    {
                        ScrollGrid.m_nTopRow = nfrow;
                        ViewLayout.Reset();
                    }

                    int nfcol = GetFirstScrollableCol();
                    if (this.LeftColIndex < nfcol)
                    {
                        ScrollGrid.m_nLeftCol = nfcol;
                        ViewLayout.Reset();
                    }

                    int rowCount = Model.RowCount;
                    int colCount = Model.ColCount;

                    if (TopRowIndex > rowCount)
                    {
                        TopRowIndex = rowCount;
                    }

                    if (LeftColIndex > colCount)
                    {
                        LeftColIndex = colCount;
                    }
                    ////
                    ////                    if (recalcScrollBars != ScrollBars.None)
                    ////                        UpdateScrollBars();
                }
                catch (Exception ex)
                {
                    TraceUtil.TraceExceptionCatched(ex);
                    if (!ExceptionManager.RaiseExceptionCatched(this, ex))
                    {
                        throw;
                    }
                }

                base.EndUpdate(false);

                ////                ViewLayout.Reset();
                ////
                string s = Model.UpdateInfo;

                GridRangeInfo rangeBounds = GridRangeInfo.Empty;
                if (!invalidRect.IsEmpty)
                {
                    savedRect = Rectangle.Union(invalidRect, savedRect);
                }

                GridRangeInfo invalidRange = RectangleToRangeInfo(savedRect, 0);
                if (this.OptimizeInsertRemoveCells)
                {
                    savedRect = ViewLayout.RangeInfoToRectangle(invalidRange, true, GridCellSizeKind.VisibleSize);
                }

                GridRangeInfo range1 = EvaluateVisibleFloatingCells(invalidRange);
                GridRangeInfo range2 = EvaluateVisibleMergeCells(invalidRange);
                GridRangeInfo range = range1.UnionRange(range2);
                range = range.UnionRange(invalidRange);
                //// REVIEW: GridRangeInfo combinedRange = ViewLayout.CombineSpannedRanges(range);
                Rectangle rect = ViewLayout.RangeInfoToRectangle(range, true, GridCellSizeKind.VisibleSize);
                ////                Trace.WriteLine("InvalidRange: " + invalidRange.ToString() + ", CombinedRange: " + combinedRange.ToString());
                ////                rect.Intersect(ClientRectangle);
                ////                invalidRange = RectangleToRangeInfo(rect);
                ////                Trace.WriteLine("InvalidRange: " + invalidRange.ToString());
                ////                HideCurrentCellIfInRange(invalidRange);
                if (!range.IsEmpty && rect != savedRect)
                {
                    Invalidate(rect);
                }

                invalidRect = Rectangle.Empty;
                inBeforePaint = false;
            }

            Trace.Unindent();
#if DEBUG
            if (Switches.GridPaint.TraceVerbose)
            {
                TraceUtil.TraceCurrentMethodInfo(PaneDesc, "End", invalidRect, inBeforePaint);
            }
#else
            ;
#endif
        }

        #endregion
        #region FloatMergeCover

        // Covered Cells
        void ModelCoveredRangesChanging(object sender, GridCoveredRangesChangingEventArgs e)
        {
        }

        void ModelCoveredRangesChanged(object sender, GridCoveredRangesChangedEventArgs e)
        {
            if (!e.Success)
            {
                return;
            }

            foreach (GridRangeInfo range in e.Ranges)
            {
                GridUpdater.UpdateCoveredCellsRange(this, range, e.SetOrReset);
            }
        }

        void ModelBanneredRangesChanging(object sender, GridBanneredRangesChangingEventArgs e)
        {
        }

        void ModelBanneredRangesChanged(object sender, GridBanneredRangesChangedEventArgs e)
        {
            if (!e.Success)
            {
                return;
            }

            foreach (GridRangeInfo range in e.Ranges)
            {
                InvalidateRange(range);
            }
        }

        void ModelMergeCellsChanged(object sender, GridMergeCellsChangedEventArgs e)
        {
            if (!e.Success)
            {
                return;
            }

            InvalidateRange(e.Range);
        }

        private GridRangeInfo EvaluateVisibleFloatingCells(GridRangeInfo range)
        {
            GridRangeInfo prgBoundary = GridRangeInfo.Empty;
            Model.FloatingCells.EvaluateFloatingCells(
                this, 
                range,
                InternalGetFrozenRows(), 
                InternalGetFrozenCols(),
                TopRowIndex, 
                LeftColIndex,
                ViewLayout.LastVisibleRow, 
                ViewLayout.LastVisibleCol,
                ref prgBoundary);
            return prgBoundary;
        }

        private GridRangeInfo EvaluateVisibleMergeCells(GridRangeInfo range)
        {
            GridRangeInfo prgBoundary = GridRangeInfo.Empty;
            Model.MergeCells.EvaluateMergeCells(
                this, 
                range,
                InternalGetFrozenRows(), 
                InternalGetFrozenCols(),
                TopRowIndex, 
                LeftColIndex,
                ViewLayout.LastVisibleRow,
                ViewLayout.LastVisibleCol, 
                ref prgBoundary);
            return prgBoundary;
        }
        ////
        ////        void ModelFloatingCellsChanging(object sender, GridFloatingCellsChangingEventArgs e)
        ////        {
        ////            TraceUtil.TraceCurrentMethodInfoIf(Switches.FloatCells.TraceVerbose, e);
        ////        }

        void ModelFloatingCellsChanged(object sender, GridFloatingCellsChangedEventArgs e)
        {
#if DEBUG
            if (Switches.FloatCells.TraceVerbose)
            {
                TraceUtil.TraceCurrentMethodInfo(PaneDesc, e);
            }
#else
            ;
#endif

            if (!e.Success)
            {
                return;
            }

            if (!inBeforePaint)
            {
                GridRangeInfo range = e.Range;
                GridModelFloatingCells.UpdateFloatedCellsRowCol(this, range.Top, range.Left, range.Bottom, range.Right);
            }
        }
        #endregion

        #region RowHeightColWidth

        /// <overload>
        /// Sets the row height a range of rows.
        /// </overload>
        /// <summary>
        /// Sets the row height a range of rows.
        /// </summary>
        /// <param name="from">First row index in range.</param>
        /// <param name="last">Last row index in range.</param>
        /// <param name="value">The value to be applied.</param>
        /// <remarks>
        /// This method calls the <see cref="GridModelRowColSizeIndexer.SetRange(int, int, int)"/>
        /// of a <see cref="GridModel.RowHeights"/> object in the <see cref="GridModel"/>.<para/>
        /// </remarks>
        public void SetRowHeight(int from, int last, int value)
        {
            Model.RowHeights.SetRange(from, last, new int[] { value });
        }

        /// <summary>
        /// Sets the row height a range of rows.
        /// </summary>
        /// <param name="from">First row index in range.</param>
        /// <param name="last">Last row index in range.</param>
        /// <param name="values">The values to be applied.</param>
        /// <genoverload/>
        public void SetRowHeight(int from, int last, int[] values)
        {
            Model.RowHeights.SetRange(from, last, values);
        }

        /// <overload>
        /// Sets values indicating if rows should be hidden.
        /// </overload>
        /// <summary>
        /// Sets values indicating if rows should be hidden.
        /// </summary>
        /// <param name="from">First row index in range.</param>
        /// <param name="last">Last row index in range.</param>
        /// <param name="value">The value to be applied.</param>
        /// <remarks>
        /// This method calls the <see cref="GridModelRowColSizeIndexer.SetRange(int, int, int)"/>
        /// of a <see cref="GridModel.HideRows"/> object in the <see cref="GridModel"/>.<para/>
        /// </remarks>
        public void SetRowHidden(int from, int last, bool value)
        {
            Model.HideRows.SetRange(from, last, new bool[] { value });
        }

        /// <summary>
        /// Sets values indicating if rows should be hidden.
        /// </summary>
        /// <param name="from">First row index in range.</param>
        /// <param name="last">Last row index in range.</param>
        /// <param name="values">An array with values to be applied.</param>
        /// <genoverload/>
        public void SetRowHidden(int from, int last, bool[] values)
        {
            Model.HideRows.SetRange(from, last, values);
        }

        /// <overload>
        /// Sets the column width for a range of columns.
        /// </overload>
        /// <summary>
        /// Sets the column width for a range of columns.
        /// </summary>
        /// <param name="from">First column index in range.</param>
        /// <param name="last">Last column index in range.</param>
        /// <param name="value">The value to be applied.</param>
        /// <remarks>
        /// This method calls the <see cref="GridModelRowColSizeIndexer.SetRange(int, int, int)"/>
        /// of a <see cref="GridModel.HideRows"/> object in the <see cref="GridModel"/>.<para/>
        /// </remarks>
        public void SetColWidth(int from, int last, int value)
        {
            Model.ColWidths.SetRange(from, last, new int[] { value });
        }

        /// <summary>
        /// Sets the column widths for a range of columns.
        /// </summary>
        /// <param name="from">First column index in range.</param>
        /// <param name="last">Last column index in range.</param>
        /// <param name="values">The values to be applied.</param>
        /// <genoverload/>
        public void SetColWidth(int from, int last, int[] values)
        {
            Model.ColWidths.SetRange(from, last, values);
        }

        /// <overload>
        /// Sets values indicating if columns should be hidden.
        /// </overload>
        /// <summary>
        /// Sets values indicating if columns should be hidden.
        /// </summary>
        /// <param name="from">First column index in range.</param>
        /// <param name="last">Last column index in range.</param>
        /// <param name="value">The value to be applied.</param>
        /// <remarks>
        /// This method calls the <see cref="GridModelHideRowColsIndexer"/>
        /// of a <see cref="GridModel.HideCols"/> object in the <see cref="GridModel"/>.<para/>
        /// </remarks>
        public void SetColHidden(int from, int last, bool value)
        {
            Model.HideCols.SetRange(from, last, new bool[] { value });
        }

        /// <summary>
        /// Sets values indicating if columns should be hidden.
        /// </summary>
        /// <param name="from">First column index in range.</param>
        /// <param name="last">Last column index in range.</param>
        /// <param name="values">An array with values to be applied.</param>
        /// <genoverload/>
        public void SetColHidden(int from, int last, bool[] values)
        {
            Model.HideCols.SetRange(from, last, values);
        }

        /// <summary>
        /// Gets or sets a value indicating whether the column header for the current cell should be highlighted.
        /// </summary>        
        [Browsable(true)]
        [DesignerSerializationVisibilityAttribute(DesignerSerializationVisibility.Hidden)]
        [Description("Specifies if the column header for the current cell should be highlighted.")]
        [Category("Grid Contents")]
        public bool MarkColHeader
        {
            get
            {
                return Model.Properties.MarkColHeader;
            }
            set
            {
                Model.Properties.MarkColHeader = value;
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether the row header for the current cell should be highlighted.
        /// </summary>       
        [Browsable(true)]
        [DesignerSerializationVisibilityAttribute(DesignerSerializationVisibility.Hidden)]
        [Description("Specifies if the row header for the current cell should be highlighted.")]
        [Category("Grid Contents")]
        public bool MarkRowHeader
        {
            get
            {
                return Model.Properties.MarkRowHeader;
            }

            set
            {
                Model.Properties.MarkRowHeader = value;
            }
        }

        /// <summary>
        /// Gets or sets <see cref="GridModelRowColOperations.DefaultSize"/> for the
        /// <see cref="GridModel.Rows"/> object in the <see cref="GridModel"/>.
        /// </summary>
        [Description("The default height used for grid rows.")]
        [Category("Grid Contents")]
        [DefaultValue(17)]
        public int DefaultRowHeight
        {
            get
            {
                return (int)Model.Rows.DefaultSize;
            }

            set
            {
                Model.Rows.DefaultSize = value;
            }
        }

        /// <summary>
        /// Gets or sets <see cref="GridModelRowColOperations.DefaultSize"/> for the
        /// <see cref="GridModel.Cols"/> object in the <see cref="GridModel"/>.
        /// </summary>
        [Description("The default width used for grid columns.")]
        [Category("Grid Contents")]
        [DefaultValue(65)]
        public int DefaultColWidth
        {
            get
            {
                return (int)Model.Cols.DefaultSize;
            }

            set
            {
                Model.Cols.DefaultSize = value;
            }
        }

        /// <overload>
        /// Returns the total row height for a range of rows.
        /// </overload>
        /// <summary>
        /// Returns the total row height for a range of rows.
        /// </summary>
        /// <param name="rowIndex1">The first row.</param>
        /// <param name="rowIndex2">The last row.</param>
        /// <returns>The total row height in pixel.</returns>
        public int GetRowRangeHeight(int rowIndex1, int rowIndex2)
        {
            return GetRowRangeHeight(rowIndex1, rowIndex2, 0);
        }

        /// <summary>
        /// Returns the total row height for a range of rows.
        /// </summary>
        /// <param name="fromRowIndex">The first row.</param>
        /// <param name="toRowIndex">The last row.</param>
        /// <param name="maxSize">Abort calculation if height is greater than this value.</param>
        /// <returns>The total row height in pixel.</returns>
        public virtual int GetRowRangeHeight(int fromRowIndex, int toRowIndex, int maxSize /* = 0 */)
        {
            int r = 0;
            for (int n = fromRowIndex; n <= toRowIndex && (maxSize == 0 || r <= maxSize);)
            {
                r += GetRowHeight(n);
                if (!ScrollGrid.GetNextRowIndex(ref n))
                {
                    break;
                }
            }

            return r;
        }

        /// <summary>
        /// Returns the row height at the <see cref="GridModelRowColSizeIndexer.this[int]"/>
        /// of a <see cref="GridModel.RowHeights"/> object in the <see cref="GridModel"/>
        /// or 0 if row is hidden.
        /// </summary>
        /// <param name="rowIndex">Row index.</param>
        /// <returns>Row height.</returns>
        /// <seealso cref="GetRowHidden"/>
        public virtual int GetRowHeight(int rowIndex)
        {
            ////                    for (int r = grid.TopRowIndex-1; r >= row; r--)
            ////                        pt.Y -= grid.GetRowHeight(r);
            if (GetRowHidden(rowIndex))
            {
                return 0;
            }

            return (int)Model.RowHeights[rowIndex]; //// + (rowIndex == 6 ? 100 : 0);
        }

        /// <summary>
        /// Returns the smallest value to be used as TopRowIndex for which the last row of the grid is visible.
        /// </summary>
        /// <param name="rectMove">Returns the rectangle with scroll bounds for the grid.</param>
        /// <returns>The calculated row index.</returns>
        public virtual int GetMaximumPossibleTopRow(out Rectangle rectMove)
        {
            int nTopRow = TopRowIndex;
            rectMove = this.ViewLayout.RectangleBottomOfRow(GetFirstScrollableRow(), GridCellSizeKind.VisibleSize);

            if (rectMove.IsEmpty)
            {
                return this.Model.RowCount;
            }

            int nLastTopRow = this.Model.RowCount;

            int y = this.GetRowHeight(nLastTopRow);
            while (y == 0)
            {
                y = this.GetRowHeight(nLastTopRow);
                if (y == 0)
                {
                    if (nLastTopRow > 1)
                    {
                        nLastTopRow--;
                    }
                    else
                    {
                        break;
                    }
                }
            }

            while (y < rectMove.Height && nLastTopRow > nTopRow)
            {
                if (!ScrollGrid.GetPrevRowIndex(ref nLastTopRow))
                {
                    break;
                }

                if (nLastTopRow > 0)
                {
                    y += this.GetRowHeight(nLastTopRow);
                }
            }

            if (y >= rectMove.Height && nLastTopRow < this.Model.RowCount)
            {
                ScrollGrid.GetNextRowIndex(ref nLastTopRow);
            }

            return nLastTopRow;
        }

        /// <summary>
        /// Returns the smallest value to be used as LeftColIndex for which the last column of the grid is visible.
        /// </summary>
        /// <param name="rectMove">Returns the rectangle with scroll bounds for the grid.</param>
        /// <returns>The calculated column index.</returns>
        public virtual int GetMaximumPossibleLeftCol(out Rectangle rectMove)
        {
            int nLeftCol = LeftColIndex;
            rectMove = this.ViewLayout.RectangleRightOfCol(GetFirstScrollableCol(), GridCellSizeKind.VisibleSize);

            if (rectMove.IsEmpty)
            {
                return this.Model.ColCount;
            }

            int nLastLeftCol = this.Model.ColCount;

            int x = this.GetColWidth(nLastLeftCol);
            while (x == 0)
            {
                x = this.GetColWidth(nLastLeftCol);
                if (x == 0)
                {
                    if (nLastLeftCol > 1)
                    {
                        nLastLeftCol--;
                    }
                    else
                    {
                        break;
                    }
                }
            }

            while (x < rectMove.Width && nLastLeftCol > nLeftCol)
            {
                if (!ScrollGrid.GetPrevColIndex(ref nLastLeftCol))
                {
                    break;
                }

                if (nLastLeftCol > 0)
                {
                    x += this.GetColWidth(nLastLeftCol);
                }
            }

            if (x >= rectMove.Width && nLastLeftCol < this.Model.ColCount)
            {
                ScrollGrid.GetNextColIndex(ref nLastLeftCol);
            }

            return nLastLeftCol;
        }

        /// <summary>
        /// Returns the column widths at the <see cref="GridModelRowColSizeIndexer.this[int]"/>
        /// of a <see cref="GridModel.ColWidths"/> object in the <see cref="GridModel"/>
        /// or 0 if column is hidden.
        /// </summary>
        /// <param name="colIndex">Column index.</param>
        /// <returns>Column width.</returns>
        /// <seealso cref="GetColHidden"/>
        public virtual int GetColWidth(int colIndex)
        {
            if (GetColHidden(colIndex))
            {
                return 0;
            }

            return (int)Model.ColWidths[colIndex];
        }

        /// <summary>
        /// Determines if a specified row is hidden in the current grid view.
        /// </summary>
        /// <param name="rowIndex">The row index.</param>
        /// <returns>True if row should be hidden; False otherwise.</returns>
        /// <remarks>
        /// You can programmatically hide rows with the <see cref="GridModelHideRowColsIndexer.this[int]"/>
        /// of a <see cref="GridModel.HideRows"/> object in the <see cref="GridModel"/>.<para/>
        /// Other criteria that will make a row be hidden in the current view are if this is the
        /// lower pane in a splitter view or if queried row is a column header row and
        /// the <see cref="GridProperties.ColHeaders"/> of <see cref="GridProperties"/> object is true.
        /// </remarks>
        public bool GetRowHidden(int rowIndex)
        {
            GridControlBase pGrid = this;

            //// Lower pane in dynamic splitter view.
            if (pGrid.splitterControl != null && !pGrid.IsPrinting() && !pGrid.displayHeaderRow && rowIndex <= pGrid.InternalGetFrozenRows())
            {
                return true;
            }

            //// Check if "Row Headers" are enabled for printing.
            if (rowIndex == 0 && !pGrid.Model.Properties.ColHeaders)
            {
                return true;
            }

            //// otherwise, let's see what is stored in the GridBitArray
            return Model.HideRows[rowIndex];
        }

        /// <summary>
        /// Determines if a specified column is hidden in the current grid view.
        /// </summary>
        /// <param name="colIndex">The column index.</param>
        /// <returns>True if column should be hidden; False otherwise.</returns>
        /// <remarks>
        /// You can programmatically hide columns with the <see cref="GridModelHideRowColsIndexer.this[int]"/>
        /// of a <see cref="GridModel.HideCols"/> object in the <see cref="GridModel"/>.<para/>
        /// Other criteria that will make a column be hidden in the current view are if this is the
        /// right pane in a splitter view or if queried row is a row header column and
        /// the <see cref="GridProperties.RowHeaders"/> of <see cref="GridProperties"/> object is true.
        /// </remarks>
        public bool GetColHidden(int colIndex)
        {
            GridControlBase pGrid = this;
            //// Right pane in dynamic splitter view.
            if (pGrid.splitterControl != null && !pGrid.IsPrinting() && !pGrid.displayHeaderCol && colIndex <= pGrid.InternalGetFrozenCols())
            {
                return true;
            }

            //// Check if "Column Headers" are enabled for printing.
            if (colIndex == 0 && !pGrid.Model.Properties.RowHeaders)
            {
                return true;
            }

            //// Otherwise, let's see what is stored in the GridBitArray.
            return Model.HideCols[colIndex];
        }

        void ModelRowHeightsChanging(object sender, GridRowColSizeChangingEventArgs e)
        {
        }

        void ModelRowHeightsChanged(object sender, GridRowColSizeChangedEventArgs e)
        {
            if (e.Success)
            {
                ViewLayout.Reset();
                Model.RowHeights.EvalRange(e.From, e.SavedValues);
                ViewLayout.Reset();
                UpdateRowHeights(e.From, e.To, e.SavedValues);
            }
        }

        void ModelColsHidden(object sender, GridRowColHiddenEventArgs e)
        {
            if (!e.Success)
            {
                return;
            }

            ViewLayout.Reset();
            int[] oldWidths = Model.ColWidths.EvalRange(e.From, Model.ColWidths.GetRange(e.From, e.To), e.SavedValues, Model.Cols.DefaultSize);
            if (Model.HideCols[e.From])
            {
                this.ScrollGrid.HideCols(e.From, e.To - e.From + 1);
            }
            else
            {
                this.ScrollGrid.ShowCols(e.From, e.To - e.From + 1);
            }

            UpdateColWidths(e.From, e.To, oldWidths);
        }

        void ModelRowsHidden(object sender, GridRowColHiddenEventArgs e)
        {
            if (!e.Success)
            {
                return;
            }

            ViewLayout.Reset();
            int[] oldHeights = Model.RowHeights.EvalRange(e.From, Model.RowHeights.GetRange(e.From, e.To), e.SavedValues, Model.Rows.DefaultSize);
            if (Model.HideRows[e.From] || Model.RowHiddenEntries.Contains(new GridRowHidden(e.From)))
            {
                this.ScrollGrid.HideRows(e.From, e.To - e.From + 1);
            }
            else
            {
                this.ScrollGrid.ShowRows(e.From, e.To - e.From + 1);
            }

            UpdateRowHeights(e.From, e.To, oldHeights);
        }

        void ModelColWidthsChanging(object sender, GridRowColSizeChangingEventArgs e)
        {
        }

        void ModelColWidthsChanged(object sender, GridRowColSizeChangedEventArgs e)
        {
            if (!e.Success)
            {
                return;
            }

            ViewLayout.Reset();
            int[] savedValues = Model.ColWidths.EvalRange(e.From, e.SavedValues);
            ViewLayout.Reset();
            UpdateColWidths(e.From, e.To, savedValues);
        }

        void ModelDefaultRowHeightChanged(object sender, GridDefaultSizeChangedEventArgs e)
        {
            if (!e.Success)
            {
                return;
            }

            ViewLayout.Reset();
            UpdateRowHeights(1, Model.RowCount, null);
        }

        void ModelDefaultColWidthChanged(object sender, GridDefaultSizeChangedEventArgs e)
        {
            if (!e.Success)
            {
                return;
            }

            ViewLayout.Reset();
            UpdateColWidths(1, Model.ColCount, null);
        }

        void UpdateRowHeights(int fromRowIndex, int toRowIndex, int[] savedHeights)
        {
            try
            {
                this.recalcScrollBars = ScrollBars.Vertical;
                Rectangle bounds = this.GridBounds;

                GridRangeInfo rowRange = GridRangeInfo.Rows(fromRowIndex, Math.Min(ViewLayout.LastVisibleRow, toRowIndex));
                GridRangeInfo range = ViewLayout.CombineSpannedRanges(rowRange);
                if (ViewLayout.IsRangeVisible(range))
                {
                    if (this.OptimizeInsertRemoveCells && savedHeights != null)
                    {
                        int oldAmount = 0;
                        int newAmount = 0;
                        for (int n = 0; n < savedHeights.Length; n++)
                        {
                            oldAmount += savedHeights[n];
                        }

                        for (int n = fromRowIndex; n <= toRowIndex; n++)
                        {
                            newAmount += Model.RowHeights[n];
                        }

                        if (oldAmount != newAmount)
                        {
                            Rectangle rc = ViewLayout.RectangleBottomOfRow(range.Top, GridCellSizeKind.VisibleSize);
                            if (oldAmount < newAmount)
                            {
                                rc.Y += oldAmount;
                                rc.Height -= oldAmount;
                            }
                            else
                            {
                                rc.Y += newAmount;
                                rc.Height -= newAmount;
                            }

                            ScrollWindow(0, newAmount - oldAmount, rc, rc, false);
                            InternalInvalidate(ViewLayout.RangeInfoToRectangle(range, GridCellSizeKind.VisibleSize));
                        }
                    }
                    else
                    {
                        InternalInvalidate(ViewLayout.RectangleBottomOfRow(range.Top, GridCellSizeKind.VisibleSize));
                    }
                }
                ////InternalInvalidate(GridBounds);
            }
            finally
            {
            }
        }

        void UpdateColWidths(int fromColIndex, int toColIndex, int[] savedWidths)
        {
            try
            {
                ViewLayout.Reset();
                this.recalcScrollBars = ScrollBars.Horizontal;
                Rectangle bounds = this.GridBounds;

                GridRangeInfo colRange = GridRangeInfo.Cols(fromColIndex, Math.Min(ViewLayout.LastVisibleCol, toColIndex));

                GridRangeInfo range = ViewLayout.CombineSpannedRanges(colRange);
                if (Model.Options.FloatCellsMode != GridFloatCellsMode.None)
                {
                    Model.FloatingCells.DelayFloatCells(range);
                    InternalInvalidate(GridBounds);
                }
                else if (ViewLayout.IsRangeVisible(range))
                {
                    if (!this.IsRightToLeft() && this.OptimizeInsertRemoveCells && savedWidths != null)
                    {
                        int oldAmount = 0;
                        int newAmount = 0;
                        for (int n = 0; n < savedWidths.Length; n++)
                        {
                            oldAmount += savedWidths[n];
                        }

                        for (int n = fromColIndex; n <= toColIndex; n++)
                        {
                            newAmount += Model.ColWidths[n];
                        }

                        if (oldAmount != newAmount)
                        {
                            Rectangle rc = ViewLayout.RectangleRightOfCol(range.Left, GridCellSizeKind.VisibleSize);
                            if (oldAmount < newAmount)
                            {
                                rc.X += oldAmount;
                                rc.Width -= oldAmount;
                            }
                            else
                            {
                                rc.X += newAmount;
                                rc.Width -= newAmount;
                            }

                            ScrollWindow(newAmount - oldAmount, 0, rc, rc, false);
                            InternalInvalidate(ViewLayout.RangeInfoToRectangle(range, GridCellSizeKind.VisibleSize));
                        }
                    }
                    else
                    {
                        InternalInvalidate(ViewLayout.RectangleRightOfCol(range.Left, GridCellSizeKind.VisibleSize));
                    }
                }
            }
            finally
            {
            }
        }

        internal bool displayHeaderCol = true;
        internal bool displayHeaderRow = true;

        /// <summary>
        /// Checks if column header should be displayed (based on splitter pane row / column).
        /// </summary>
        /// <returns>True if the column header should be displayed.</returns>
        public bool ShouldDisplayHeaderCol()
        {
            return displayHeaderCol;
        }

        /// <summary>
        /// Checks if row header should be displayed (based on splitter pane row / column).
        /// </summary>
        /// <returns>True if the row header should be displayed.</returns>
        public bool ShouldDisplayHeaderRow()
        {
            return displayHeaderRow;
        }

        #endregion
        #region WndProc
        internal const int WM_UPDATEUISTATE = 296; // 0x0128

        internal bool inImeComposition = false;

        /// <summary>
        /// Gets a value indicating whether true after WM_IME_STARTCOMPOSITION is sent and False once WM_IME_ENDCOMPOSITION is handled.
        /// </summary>
        [Browsable(false)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public bool InImeComposition
        {
            get
            {
                return inImeComposition;
            }
        }

        ////        private static void IMEPosition(IntPtr handle)
        ////        {
        ////            IntPtr himc = NativeMethods.ImmGetContext(handle);
        ////            if (himc != IntPtr.Zero)
        ////            {
        ////                NativeMethods.COMPOSITIONFORM compositionForm = new NativeMethods.COMPOSITIONFORM();
        ////                Rectangle r = new Rectangle(10, 50, 40, 40);////this.CurrentCell.
        ////                compositionForm.rcArea = NativeMethods.RECT.FromXYWH(r.X, r.Y, r.Width, r.Height);
        ////                compositionForm.dwStyle = 0x21;
        ////                NativeMethods.ImmSetCompositionWindow(handle, compositionForm);
        ////                TraceUtil.TraceCurrentMethodInfo(handle, himc, compositionForm.dwStyle, compositionForm.ptCurrentPos, compositionForm.rcArea);
        ////                NativeMethods.ImmReleaseContext(handle, himc);
        ////            }
        ////        }

        bool allowForwardCurrentCellControlKeyMessages = true;

        /// <summary>Used internally.</summary>
        /// <exclude/>
        public void EnableForwardCurrentCellControlKeyMessages(bool value)
        {
            allowForwardCurrentCellControlKeyMessages = value;
        }

        /// <summary>
        /// Gets a value indicating whether true when UpdateWithCustomPaint was called.
        /// </summary>
        [Browsable(false), System.Xml.Serialization.XmlIgnore, DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public bool InUpdateWithCustomPaint
        {
            get
            {
                return customPaintDelegate != null;
            }
        }

        /// <summary>
        /// Allows you to perform a quick Invalidate / Update pair on the grid without
        /// executing any of the grids default paint code. Instead you can provide your
        /// own routine to update invalidated areas in the grid.
        /// </summary>
        /// <param name="r">The Rectangle</param>
        /// <param name="customPaintDelegate">The customPaintDelegate</param>
        /// <example>The following example lets you draw a small portion of the grid
        /// bypassing the grids paint handler (and it trying to recalculating layout cells 
        /// and other precautions it takes when handling the Paint method).<para/>
        /// When compared to calling DrawClippedGrid this approach is a bit slower but it avoids flickering since it can 
        /// take advantage of the WinForms own double-buffering mechanism.<para/>
        /// In OnPaint the grid checks InUpdateWithCustomPaint and if it is true then the grids Paint method will only 
        /// call the delegate and immediately return.
        /// <code lang="C#">
        /// gridTableControl.UpdateWithCustomPaint(bounds, new PaintEventHandler(TableControl_CustomPaint));
        /// <para/>
        /// void TableControl_CustomPaint(object sender, PaintEventArgs e)
        /// {
        ///     Rectangle clipBounds = Rectangle.Truncate(e.Graphics.ClipBounds);
        ///     gridTableControl.DrawClippedGrid(e.Graphics, clipBounds, false);
        /// }
        /// </code>
        /// </example>
        public void UpdateWithCustomPaint(Rectangle r, PaintEventHandler customPaintDelegate)
        {
            if (r.IsEmpty)
            {
                return;
            }

            try
            {
                this.customPaintDelegate = customPaintDelegate;
                this.customPaintRectangle = r;
                base.Invalidate(r);
                base.Update();
            }
            finally
            {
                this.customPaintDelegate = null;
                this.customPaintRectangle = Rectangle.Empty;
            }
        }

        /// <summary>
        /// Allows you to perform a quick Invalidate / Update pair on the grid without
        /// executing any of the grids default paint code. Instead the grid will only
        /// call DrawClippedGrid from its Paint handler and immediately return. <para/>
        /// When compared to calling DrawClippedGrid using a cached graphics context 
        /// this method is a bit slower but it avoids flickering since it can 
        /// take advantage of the WinForms own double-buffering mechanism.
        /// </summary>
        /// <param name="clipBounds">Clipping rectangle.</param>
        public void UpdateWithDrawClippedGrid(Rectangle clipBounds)
        {
            if (clipBounds.IsEmpty)
            {
                return;
            }

            UpdateWithCustomPaint(clipBounds, new PaintEventHandler(CustomPaintHandler));
            return;
            //bool shouldClip = false;
            //if (!shouldClip)
            //{
            //    if (this.VScrollPixel && GetCurrentVScrollPixelDelta() != 0
            //        && clipBounds.Y == GetVScrollPixelMinimum())
            //    {
            //        shouldClip = true;
            //    }

            //    if (this.HScrollPixel && GetCurrentHScrollPixelDelta() != 0
            //        && clipBounds.X == GetHScrollPixelMinimum())
            //    {
            //        shouldClip = true;
            //    }
            //}

            //DrawClippedGrid(this.GetCachedGraphics(), clipBounds, shouldClip);
        }

        void CustomPaintHandler(object sender, PaintEventArgs e)
        {
            Rectangle clipBounds = Rectangle.Truncate(e.Graphics.ClipBounds);
            DrawClippedGrid(e.Graphics, clipBounds, false);
        }

        PaintEventHandler customPaintDelegate;
        Rectangle customPaintRectangle = Rectangle.Empty;

        internal void ProcessMessage(ref Message msg)
        {
            this.WndProc(ref msg);
        }
        /// <override/>
        [SecurityPermission(SecurityAction.LinkDemand, UnmanagedCode = true)]
        protected override void WndProc(ref Message msg)
        {
            // Support for being used in MFC and ActiveX Hosts.
            if (msg.Msg == WM_GETDLGCODE && wmGetDlgCodeValue != -1)
            {
                msg.Result = (IntPtr)wmGetDlgCodeValue;
                return;
            }

            if (allowForwardCurrentCellControlKeyMessages)
            {
                if (msg.Msg >= NativeMethods.WM_KEYFIRST && msg.Msg <= NativeMethods.WM_KEYLAST)
                {
                    ////TraceUtil.TraceCurrentMethodInfo(CurrentCell.ToString(), msg.ToString());
                    if (CurrentCell.HasControlFocus && CurrentCell.Renderer.Control != null
                        && CurrentCell.Renderer.Control.IsHandleCreated)
                    {
                        msg.Result = NativeMethods.SendMessage(CurrentCell.Renderer.Control.Handle, msg.Msg, msg.WParam, msg.LParam);
                        return;
                    }
                }
            }

            if (msg.Msg == 0x031A/*WM_THEMECHANGED*/)
            {
                this.Invalidate();
            }

            if (!(FocusOnMouseDown && this.WantKeys) && msg.Msg == 0x21/*WM_MOUSEACTIVATE*/)
            {
                msg.Result = (IntPtr)3; ////MA_NOACTIVATE
                return;
            }

            /*if (this.DesignMode)
                base.WndProc(ref msg);
            else */
            {
                switch (msg.Msg)
                {
                    case NativeMethods.WM_IME_COMPOSITION:
                        //// see also comments in TextBoxCellRender.FloatDone property.
                        goto default;
                    case NativeMethods.WM_IME_ENDCOMPOSITION:
                        inImeComposition = false;
                        goto default;
                    case NativeMethods.WM_IME_STARTCOMPOSITION:
                        inImeComposition = true;
                        goto default;

                    ////                    case NativeMethods.WM_IME_CONTROL:
                    ////                    case NativeMethods.WM_IME_NOTIFY:
                    ////                    case NativeMethods.WM_IME_SELECT:
                    ////                    case NativeMethods.WM_IME_SETCONTEXT:
                    ////                        TraceUtil.TraceCurrentMethodInfo(Handle.ToInt32().ToString("x"), msg.ToString());
                    ////                        //IMEPosition(msg.HWnd);
                    ////                        goto default;

                    case 0x129:
                        //// Avoid Control.ProcessUICues being called from Control.PreProcessMessage
                        ////if (inPreProcessMessageTabKey)
                        ////#define UISF_HIDEFOCUS                  0x1
                        ////#define UISF_HIDEACCEL                  0x2
                        msg.Result = (IntPtr)2;
                        return;

                    case WM_UPDATEUISTATE:
                        if (CurrentCell.IsInBeginEdit)
                        {
                            return;
                        }

                        goto default;

                    case NativeMethods.WM_PAINT:
                        if (customPaintDelegate != null)
                        {
                            goto default;
                        }

                        if (!Updating)
                        {
                            OnBeforePaint(EventArgs.Empty);
                        }

                        goto default;

                    default:
                        if (this.GridBounds.Right < 0 || this.Location.X > 9999)
                        {
                            bool c = CurrentCell.StaticDrawing;
                            CurrentCell.StaticDrawing = true;
                            base.WndProc(ref msg);
                            CurrentCell.StaticDrawing = c;
                        }
                        else
                        {
                            base.WndProc(ref msg);
                        }

                        break;
                }
            }
        }
        #endregion
        #region Windowless Support

        /// <summary>
        /// Occurs when the user is in the process of activating the control with a mouse click.
        /// </summary>
        [Description("Occurs when the user is in the process of activating the control with a mouse click."),
        Category("Behavior")]
        public event CancelEventHandler MouseActivating;

        /// <summary>
        /// Raises the <see cref="MouseActivating"/> event.
        /// </summary>
        /// <param name="e">A <see cref="CancelEventArgs" /> that contains the event data.</param>
        protected virtual void OnMouseActivating(CancelEventArgs e)
        {
            if (this.eventsTarget != null)
            {
                this.eventsTarget.OnMouseActivating(e);
            }

            if (MouseActivating != null)
            {
                MouseActivating(this, e);
            }
        }

        /// <summary>
        /// Initiates call to <see cref="OnMouseActivating"/>.
        /// </summary>
        /// <returns>True if the method call is initiated.</returns>
        public bool RaiseMouseActivating()
        {
            CancelEventArgs e = new CancelEventArgs();
            OnMouseActivating(e);
            return !e.Cancel;
        }

        /// <summary>
        /// Returns the parent control with window handle and casts it to GridControlBase.
        /// </summary>
        /// <returns>The GridControlBase with a valid window handle.</returns>
        /// <remarks>
        /// When the <see cref="IsWindowless"/> property is True, it will return the parent control; otherwise a reference to this object itself is returned.</remarks>
        public GridControlBase GetGridWindow()
        {
            return (GridControlBase)GetWindow();
        }

        /// <summary>
        /// Returns the parent control with window handle.
        /// </summary>
        /// <returns>The control with a valid window handle.</returns>
        /// <remarks>
        /// When the <see cref="IsWindowless"/> property is True, it will return the parent control; otherwise a reference to this object itself is returned.</remarks>
        public Control GetWindow()
        {
            if (this.IsWindowless)
            {
                return ParentSite.GetWindow();
            }
            else if (this.CurrentCell.StaticDrawing && this.CurrentCell.StaticRenderControl != null)
            {
                return this.CurrentCell.StaticRenderControl;
            }

            return this;
        }

        /// <summary>
        /// Returns the intersection between this control's bounds and the bounds of a parent control with window handle.
        /// </summary>
        /// <returns>The rectangle for the visible area of this control.</returns>
        /// <remarks>
        /// When the <see cref="IsWindowless"/> property is True, it will intersect the <see cref="GridBounds"/> with the parent control's <see cref="GetVisibleBounds"/>;
        /// otherwise simply the <see cref="GridBounds"/> are returned.
        /// </remarks>
        public Rectangle GetVisibleBounds()
        {
            Rectangle r = GridBounds;
            if (this.IsWindowless)
            {
                r.Intersect(ParentSite.GetVisibleBounds());
            }
            else if (this.CurrentCell.StaticDrawing)
            {
                if (CurrentCell.StaticRenderControl != null)
                {
                    r.Intersect(this.CurrentCell.StaticRenderControl.Bounds);
                }
            }

            return r;
        }

        bool isWindowless = false;

        /// <summary>
        /// Gets or sets a value indicating whether the control is used in windowless mode.
        /// </summary>
        [DesignerSerializationVisibilityAttribute(DesignerSerializationVisibility.Hidden)]
        [Browsable(false)]
        public bool IsWindowless
        {
            get { return isWindowless; }
            set { isWindowless = value; }
        }

        /// <summary>
        /// Retrieves the form that the control is on. Works also with windowless mode.
        /// </summary>
        /// <returns>The form that the control is on</returns>
        public Form FindParentForm()
        {
            if (!this.IsWindowless)
            {
                return FindForm();
            }

            return ParentSite.GetWindow().FindForm();
        }

        IGridWindowlessSite parentSite;

        /// <summary>
        /// Gets or sets the parent control that implements IGridWindowlessSite.
        /// </summary>
        [DesignerSerializationVisibilityAttribute(DesignerSerializationVisibility.Hidden)]
        [Browsable(false)]
        public IGridWindowlessSite ParentSite
        {
            get { return parentSite; }
            set { parentSite = value; }
        }

        /// <override/>
        protected override void OnCreateControl()
        {
            if (this.IsWindowless)
            {
                throw new NotSupportedException("Windowless grid controls can't have a window");
            }

            base.OnCreateControl();
        }

        /// <summary>
        /// Gets or sets a value indicating whether <see cref="Control.Capture"/> of the parent control with window handle (see <see cref="GetWindow"/>).
        /// </summary>
        [DesignerSerializationVisibilityAttribute(DesignerSerializationVisibility.Hidden)]
        [Browsable(false)]
        public bool GridCapture
        {
            get { return GetWindow().Capture; }
            set { GetWindow().Capture = value; }
        }

        /// <summary>
        /// Gets or sets a value indicating whether <see cref="Control.Capture"/> of the parent control with window handle (see <see cref="GetWindow"/>).
        /// </summary>
        [DesignerSerializationVisibilityAttribute(DesignerSerializationVisibility.Hidden)]
        [Browsable(false)]
        public new bool Capture
        {
            get { return GetWindow().Capture; }
            set { GetWindow().Capture = value; }
        }

        /// <summary>
        /// Converts a rectangle to screen coordinates.
        /// </summary>
        /// <param name="rect">The original rectangle in client coordinates.</param>
        /// <returns>The resulting rectangle in screen coordinates.</returns>
        /// <remarks>
        /// This method also supports <see cref="SetWindowlessBounds"/> mode.
        /// </remarks>
        public Rectangle GridRectangleToScreen(Rectangle rect)
        {
            Rectangle bounds = rect;
            if (CurrentCell.StaticDrawing)
            {
                if (CurrentCell.StaticRenderControl != null)
                {
                    bounds = CurrentCell.StaticRenderControl.RectangleToScreen(rect);
                }
            }
            else
            {
                bounds = GetWindow().RectangleToScreen(rect);
            }

            return bounds;
        }

        /// <summary>
        /// Converts a rectangle to client coordinates.
        /// </summary>
        /// <param name="rect">The original rectangle in screen coordinates.</param>
        /// <returns>The resulting rectangle in client coordinates.</returns>
        /// <remarks>
        /// This method also supports <see cref="SetWindowlessBounds"/> mode.
        /// </remarks>
        public Rectangle GridRectangleToClient(Rectangle rect)
        {
            Rectangle bounds;
            if (CurrentCell.StaticDrawing)
            {
                bounds = CurrentCell.StaticRenderControl.RectangleToClient(rect);
            }
            else
            {
                bounds = GetWindow().RectangleToClient(rect);
            }

            return bounds;
        }

        /// <summary>
        /// Converts a point to screen coordinates.
        /// </summary>
        /// <param name="p">>The original point in client coordinates.</param>
        /// <returns>The resulting point in screen coordinates.</returns>
        /// <remarks>
        /// This method also supports <see cref="SetWindowlessBounds"/> mode.
        /// </remarks>
        public Point GridPointToScreen(Point p)
        {
            Point pt = p;
            if (CurrentCell.StaticDrawing)
            {
                if (CurrentCell.StaticRenderControl != null)
                {
                    pt = CurrentCell.StaticRenderControl.PointToScreen(p);
                }
            }
            else
            {
                pt = GetWindow().PointToScreen(p);
            }

            return pt;
        }

        /// <summary>
        /// Converts a point to client coordinates.
        /// </summary>
        /// <param name="p">>The original point in screen coordinates.</param>
        /// <returns>The resulting point in client coordinates.</returns>
        /// <remarks>
        /// This method also supports <see cref="SetWindowlessBounds"/> mode.
        /// </remarks>
        public Point GridPointToClient(Point p)
        {
            Point pt;
            if (CurrentCell.StaticRenderControl != null && CurrentCell.StaticDrawing)
            {
                pt = CurrentCell.StaticRenderControl.PointToClient(p);
            }
            else
            {
                pt = GetWindow().PointToClient(p);
            }

            return pt;
        }

        /// <summary>
        /// Returns the result of <see cref="Control.PointToClient"/> of the parent control with window handle (see <see cref="GetWindow"/>).
        /// </summary>
        /// <param name="p">Screen point.</param>
        /// <returns>Location of specified screen point into its client co-ordinates.</returns>
        public new Point PointToClient(Point p)
        {
            return GetWindow().PointToClient(p);
        }

        /// <summary>
        /// Returns the result of <see cref="Control.PointToScreen"/> of the parent control with window handle (see <see cref="GetWindow"/>).
        /// </summary>
        /// <param name="p">Client point.</param>
        /// <returns>Point in screen co-ordinates.</returns>
        public new Point PointToScreen(Point p)
        {
            return GetWindow().PointToScreen(p);
        }

        /// <summary>
        /// Returns the result of <see cref="Control.RectangleToClient"/> of the parent control with window handle (see <see cref="GetWindow"/>).
        /// </summary>
        /// <param name="p">Rectangle to convert.</param>
        /// <returns>Rectangle in client co-ordinates.</returns>
        public new Rectangle RectangleToClient(Rectangle p)
        {
            return GetWindow().RectangleToClient(p);
        }

        /// <summary>
        /// Returns the result of <see cref="Control.RectangleToScreen"/> of the parent control with window handle (see <see cref="GetWindow"/>).
        /// </summary>
        /// /// <param name="p">Rectangle to convert.</param>
        /// <returns>Rectangle in screen co-ordinates.</returns>
        public new Rectangle RectangleToScreen(Rectangle p)
        {
            return GetWindow().RectangleToScreen(p);
        }
        
        /// <summary>
        /// Gets or sets a value indicating whether the the control is handling a <see cref="Control.MouseDown"/> event.
        /// </summary>
        [Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public new bool IsMousePressed
        {
            get
            {
                return ((ScrollControl)GetWindow()).IsMousePressed;
            }

            set
            {
                ((ScrollControl)GetWindow()).IsMousePressed = value;
            }
        }
        
        /// <summary>
        /// Gets a value indicating whether the <see cref="Control.OnValidating"/> method has been called. <see cref="Control.OnLeave"/> and <see cref="Control.OnEnter"/> reset this flag.
        /// </summary>
        [Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public new bool IsValidating
        {
            get
            {
                return ((ScrollControl)GetWindow()).IsValidating;
            }
        }

        /// <summary>
        /// Gets a value indicating whether the <see cref="Control.OnValidated"/> method has been called. <see cref="Control.OnLeave"/> and <see cref="Control.OnEnter"/> reset this flag.
        /// </summary>
        [Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public new bool IsValidated
        {
            get
            {
                return ((ScrollControl)GetWindow()).IsValidated;
            }
        }

        /// <summary>
        /// Gets a value indicating whether the <see cref="Control.OnEnter"/> has been called. <see cref="Control.OnLeave"/> resets this flag.
        /// </summary>
        [Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public new bool IsActiveControl
        {
            get
            {
                return ((ScrollControl)GetWindow()).IsActiveControl;
            }
        }

        /// <summary>
        /// Gets a value indicating whether <see cref="OnDeactivated"/> has been called. <see cref="Control.OnEnter"/> resets this flag.
        /// </summary>
        [Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public new bool IsDeactivated
        {
            get
            {
                return ((ScrollControl)GetWindow()).IsDeactivated;
            }
        }

        /// <summary>
        /// Gets a value indicating whether <see cref="OnControlGotFocus"/> has been called. <see cref="OnControlLostFocus"/> resets this flag.
        /// </summary>
        [Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public new bool HasControlFocus
        {
            get
            {
                return ((ScrollControl)GetWindow()).HasControlFocus;
            }
        }

        #endregion
        #region Frozen
        /////////////////////////////////////////////////////////////////////////////
        // Frozen Rows at the top of the Grid (before TopRow)
        // or frozen Cols at the left of the Grid (before LeftCol).

        /// <summary>
        /// Returns the number of frozen rows.
        /// </summary>
        /// <returns>Number of frozen rows.</returns>
        /// <remarks>
        /// This is usually the same value as returned by <see cref="GridModelRowColOperations.FrozenCount"/>
        /// of a <see cref="GridModel.Rows"/> object in a <see cref="GridModel"/>. <para/>
        /// When the grid is used as bottom pane in a splitter view, 0 is returned.
        /// </remarks>
        public virtual int InternalGetFrozenRows()
        {
            return (!IsPrinting() || !PrintInfo.m_bPrintCurSelOnly) ? Model.Rows.FrozenCount : 0;
        }

        /// <summary>
        /// Returns the first row that is visible after the frozen rows.
        /// </summary>
        /// <returns>First scrollable row index.</returns>
        public int GetFirstScrollableRow()
        {
            return ScrollGrid.GetNextRowIndex(InternalGetFrozenRows(), GridControlBase.UseOldHiddenScrollLogic);
        }

        /// <summary>
        /// Returns the actual number of visible frozen rows including the column header if visible. So,
        /// in case all rows are visible the value returned by this method will be Rows.FrozenCount + 1.
        /// </summary>
        /// <returns>Number of visible frozen rows.</returns>
        public int GetVisibleFrozenRows()
        {
            if (UseOldHiddenScrollLogic)
            {
                return InternalGetFrozenRows() + 1;
            }

            int nfr = InternalGetFrozenRows();
            int nnfr = ScrollGrid.RowIndexToScrollPosition(nfr);
            if (!GetRowHidden(nfr))
            {
                nnfr++;
            }

            return nnfr;
        }

        /// <summary>
        /// Returns the number of frozen columns.
        /// </summary>
        /// <returns>Number of frozen columns.</returns>
        /// <remarks>
        /// This is usually the same value as returned by <see cref="GridModelRowColOperations.FrozenCount"/>
        /// of a <see cref="GridModel.Cols"/> object in a <see cref="GridModel"/>. <para/>
        /// When the grid is used as right pane in a splitter view, 0 is returned.
        /// </remarks>
        public virtual int InternalGetFrozenCols()
        {
            return (!IsPrinting() || !PrintInfo.m_bPrintCurSelOnly) ? Model.Cols.FrozenCount : 0;
        }

        /// <summary>
        /// Returns the first column that is visible after the frozen columns.
        /// </summary>
        /// <returns>First scrollable column index.</returns>
        public int GetFirstScrollableCol()
        {
            return ScrollGrid.GetNextColIndex(InternalGetFrozenCols(), GridControlBase.UseOldHiddenScrollLogic);
        }

        /// <summary>
        /// Returns the actual number of visible frozen columns including the row header if visible. So,
        /// in case all columns are visible the value returned by this method will be Cols.FrozenCount + 1.
        /// </summary>
        /// <returns>Number of visible frozen columns.</returns>
        public int GetVisibleFrozenCols()
        {
            if (UseOldHiddenScrollLogic)
            {
                return InternalGetFrozenCols() + 1;
            }

            int nfc = InternalGetFrozenCols();
            int nnfc = ScrollGrid.ColIndexToScrollPosition(nfc);
            if (!GetColHidden(nfc))
            {
                nnfc++;
            }

            return nnfc;
        }

        /// <summary>
        /// Returns the number of header rows.
        /// </summary>
        /// <returns>Number of header rows.</returns>
        /// <remarks>
        /// This is usually the same value as returned by <see cref="GridModelRowColOperations.FrozenCount"/>
        /// of a <see cref="GridModel.Rows"/> object in a <see cref="GridModel"/>. <para/>
        /// </remarks>
        public virtual int InternalGetHeaderRows()
        {
            return Model.Rows.HeaderCount;
        }

        /// <summary>
        /// Returns the number of header columns.
        /// </summary>
        /// <returns>Number of header columns.</returns>
        /// <remarks>
        /// This is usually the same value as returned by <see cref="GridModelRowColOperations.FrozenCount"/>
        /// of a <see cref="GridModel.Cols"/> object in a <see cref="GridModel"/>. <para/>
        /// </remarks>
        public virtual int InternalGetHeaderCols()
        {
            return Model.Cols.HeaderCount;
        }

        /// <summary>
        /// Determines if the specified row is a frozen row.
        /// </summary>
        /// <param name="rowIndex">The row index.</param>
        /// <returns>True if frozen; False if otherwise.</returns>
        public bool InternalIsFrozenRow(int rowIndex)
        {
            return rowIndex > 0 && rowIndex <= InternalGetFrozenRows();
        }

        /// <summary>
        /// Determines if the specified column is a frozen column.
        /// </summary>
        /// <param name="colIndex">The column index.</param>
        /// <returns>True if frozen; False if otherwise.</returns>
        public bool InternalIsFrozenCol(int colIndex)
        {
            return colIndex > 0 && colIndex <= InternalGetFrozenCols();
        }

        /// <summary>
        /// Occurs when the fixed column count in the model has changed and the view needs to be refreshed.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">Event data.</param>
        protected virtual void ModelFixedColChanged(object sender, GridCountChangedEventArgs e)
        {
            if (!e.Success)
            {
                return;
            }

            ViewLayout.Reset();
            int headerCount = Model.Cols.HeaderCount + 1;
            int count = Model.Cols.FrozenCount;
            BeginUpdate(BeginUpdateOptions.None);
            this.InternalSetLeftCol(count + 1);
            Invalidate();
            this.recalcScrollBars = ScrollBars.Horizontal;
            EndUpdate();
        }

        /// <summary>
        /// Occurs when the fixed row count in the model has changed and the view needs to be refreshed.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">Event data.</param>
        protected virtual void ModelFixedRowChanged(object sender, GridCountChangedEventArgs e)
        {
            if (!e.Success)
            {
                return;
            }

            ViewLayout.Reset();
            int headerCount = Model.Rows.HeaderCount + 1;
            int count = Model.Rows.FrozenCount;
            BeginUpdate(BeginUpdateOptions.None);
            this.InternalSetTopRow(count + 1);
            Invalidate();
            this.recalcScrollBars = ScrollBars.Vertical;
            EndUpdate();
        }

        /// <summary>
        /// Occurs when the header column count in the model has changed and the view needs to be refreshed.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">Event data.</param>
        protected virtual void ModelHeaderColChanged(object sender, GridCountChangedEventArgs e)
        {
            if (!e.Success)
            {
                return;
            }

            ViewLayout.Reset();
            Refresh();
        }

        /// <summary>
        /// Occurs when the header row count in the model has changed and the view needs to be refreshed.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">Event data.</param>
        protected virtual void ModelHeaderRowChanged(object sender, GridCountChangedEventArgs e)
        {
            if (!e.Success)
            {
                return;
            }

            ViewLayout.Reset();
            Refresh();
        }
        #endregion
        #region RemoveCells
        bool activateCCafterRangeOp = false;

        void ModelRowsRangeRemoving(object sender, GridRangeRemovingEventArgs e)
        {
            activateCCafterRangeOp = false;

            if (!e.Cancel)
            {
                if (CurrentCell.IsLocked)
                {
                    OnSelectionFrameChanging(GraphicsEventArgs.Empty);
                }
                else if (CurrentCell.ConfirmChanges())
                {
                    activateCCafterRangeOp = CurrentCell.HasCurrentCell;
                    CurrentCell.Deactivate(true);
                    OnSelectionFrameChanging(GraphicsEventArgs.Empty);
                }
                else
                {
                    if (CurrentCell.ErrorMessage.Length > 0)
                    {
                        CurrentCell.DisplayWarningText(CurrentCell.ErrorMessage);
                        CurrentCell.ResetError();
                    }

                    e.Cancel = true;
                }
            }
        }

        void ModelRowsRangeRemoved(object sender, GridRangeRemovedEventArgs e)
        {
            try
            {
                if (!e.Success)
                {
                    return;
                }

                ViewLayout.Reset();
                if (activateCCafterRangeOp)
                {
                    CurrentCell.UpdateRemoveRows(e.From, e.To);
                }

                ScrollGrid.RecalcHiddenRowState(e.From, e.From);

                int count = e.To - e.From + 1;
                if (this.OptimizeInsertRemoveCells)
                {
                    if (!activateCCafterRangeOp && CurrentCell.RowIndex >= e.To)
                    {
                        CurrentCell.SetCurrentCellNoActivate(CurrentCell.RowIndex - e.To - e.From + 1, CurrentCell.ColIndex);
                    }

                    Rectangle rc = this.ViewLayout.RectangleBottomOfRow(e.From, GridCellSizeKind.VisibleSize);

                    if (!(e.To > InternalGetFrozenRows() && e.To < TopRowIndex) && !rc.IsEmpty)
                    {
                        int yAmount = 0;
                        int n = 0;
                        int[] rowColSizes = new int[0];
                        if (e.InsertRangeOptions != null && e.InsertRangeOptions.RowColSizes != null)
                        {
                            rowColSizes = e.InsertRangeOptions.RowColSizes;
                        }

                        for (; n < rowColSizes.Length; n++)
                        {
                            if (e.InsertRangeOptions.RowColHide == null || n >= e.InsertRangeOptions.RowColHide.Length || !e.InsertRangeOptions.RowColHide[n])
                            {
                                if (rowColSizes[n] != -1)
                                {
                                    yAmount += rowColSizes[n];
                                }
                                else
                                {
                                    yAmount += Model.Rows.DefaultSize;
                                }
                            }
                        }

                        for (; n < count; n++)
                        {
                            if (e.InsertRangeOptions == null || e.InsertRangeOptions.RowColHide == null || n >= e.InsertRangeOptions.RowColHide.Length || !e.InsertRangeOptions.RowColHide[n])
                            {
                                yAmount += Model.Rows.DefaultSize;
                            }
                        }

                        ScrollWindow(0, -yAmount, rc, rc, false);
                    }
                    else
                    {
                        if (e.From < TopRowIndex)
                        {
                            if (e.To < TopRowIndex)
                            {
                                this.InternalSetTopRow(TopRowIndex - count);
                            }
                            else
                            {
                                Invalidate();
                            }
                        }
                    }

                    this.recalcScrollBars = ScrollBars.Vertical;
                    ////Update();
                }
                else
                {
                    Refresh();
                }

                UpdateScrollBars();
            }
            finally
            {
                OnSelectionFrameChanged(GraphicsEventArgs.Empty);
            }
        }

        void ModelColsRangeRemoving(object sender, GridRangeRemovingEventArgs e)
        {
            activateCCafterRangeOp = false;

            if (!e.Cancel)
            {
                if (CurrentCell.IsLocked)
                {
                    OnSelectionFrameChanging(GraphicsEventArgs.Empty);
                }
                else if (CurrentCell.ConfirmChanges())
                {
                    activateCCafterRangeOp = CurrentCell.HasCurrentCell;
                    CurrentCell.Deactivate(true);
                    OnSelectionFrameChanging(GraphicsEventArgs.Empty);
                }
                else
                {
                    if (CurrentCell.ErrorMessage.Length > 0)
                    {
                        CurrentCell.DisplayWarningText(CurrentCell.ErrorMessage);
                        CurrentCell.ResetError();
                    }

                    e.Cancel = true;
                }
            }
        }

        void ModelColsRangeRemoved(object sender, GridRangeRemovedEventArgs e)
        {
            try
            {
                if (!e.Success)
                {
                    return;
                }

                ViewLayout.Reset();
                if (activateCCafterRangeOp)
                {
                    CurrentCell.UpdateRemoveCols(e.From, e.To);
                }

                ScrollGrid.RecalcHiddenColState(e.From, e.From);

                int count = e.To - e.From + 1;
                if (this.OptimizeInsertRemoveCells)
                {
                    if (!activateCCafterRangeOp && CurrentCell.ColIndex >= e.To)
                    {
                        CurrentCell.SetCurrentCellNoActivate(CurrentCell.RowIndex, CurrentCell.ColIndex - e.To - e.From + 1);
                    }

                    Rectangle rc = this.ViewLayout.RectangleRightOfCol(e.From, GridCellSizeKind.VisibleSize);

                    if (!(e.To > InternalGetFrozenCols() && e.To < LeftColIndex) && !rc.IsEmpty)
                    {
                        int xAmount = 0;
                        int n = 0;
                        int[] rowColSizes = new int[0];
                        if (e.InsertRangeOptions != null && e.InsertRangeOptions.RowColSizes != null)
                        {
                            rowColSizes = e.InsertRangeOptions.RowColSizes;
                        }

                        for (; n < rowColSizes.Length; n++)
                        {
                            if (e.InsertRangeOptions.RowColHide == null || n >= e.InsertRangeOptions.RowColHide.Length || !e.InsertRangeOptions.RowColHide[n])
                            {
                                if (rowColSizes[n] != -1)
                                {
                                    xAmount += rowColSizes[n];
                                }
                                else
                                {
                                    xAmount += Model.Cols.DefaultSize;
                                }
                            }
                        }

                        for (; n < count; n++)
                        {
                            if (e.InsertRangeOptions == null || e.InsertRangeOptions.RowColHide == null || n >= e.InsertRangeOptions.RowColHide.Length || !e.InsertRangeOptions.RowColHide[n])
                            {
                                xAmount += Model.Cols.DefaultSize;
                            }
                        }

                        ScrollWindow(-xAmount, 0, rc, rc, false);
                    }
                    else
                    {
                        if (e.From < LeftColIndex)
                        {
                            if (e.To < LeftColIndex)
                            {
                                this.InternalSetLeftCol(LeftColIndex - count);
                            }
                            else
                            {
                                Invalidate();
                            }
                        }
                    }

                    this.recalcScrollBars = ScrollBars.Horizontal;
                    ////Update();
                }
                else
                {
                    Refresh();
                }

                UpdateScrollBars();
            }
            finally
            {
                OnSelectionFrameChanged(GraphicsEventArgs.Empty);
            }
        }

        #endregion
        #region MoveRC
        void ModelRowsRangeMoving(object sender, GridRangeMovingEventArgs e)
        {
            activateCCafterRangeOp = false;

            if (!e.Cancel)
            {
                if (CurrentCell.IsLocked)
                {
                    OnSelectionFrameChanging(GraphicsEventArgs.Empty);
                }
                else if (CurrentCell.ConfirmChanges())
                {
                    activateCCafterRangeOp = CurrentCell.HasCurrentCell;
                    CurrentCell.Deactivate(true);
                    OnSelectionFrameChanging(GraphicsEventArgs.Empty);
                }
                else
                {
                    if (CurrentCell.ErrorMessage.Length > 0)
                    {
                        CurrentCell.DisplayWarningText(CurrentCell.ErrorMessage);
                        CurrentCell.ResetError();
                    }

                    e.Cancel = true;
                }
            }
        }

        void ModelRowsRangeMoved(object sender, GridRangeMovedEventArgs e)
        {
            try
            {
                if (!e.Success)
                {
                    return;
                }

                ViewLayout.Reset();
                UpdateRowsMoved(e.From, e.Count, e.Target);
            }
            finally
            {
                OnSelectionFrameChanged(GraphicsEventArgs.Empty);
            }
        }

        void UpdateRowsMoved(int from, int count, int target)
        {
            int to = from + count - 1;
            int dest = target > from ? target + count : target;

            if (activateCCafterRangeOp)
            {
                CurrentCell.UpdateRowsMoved(from, to, target);
            }

            ScrollGrid.RecalcHiddenRowState(Math.Min(from, target), Math.Max(dest, from) + count);

            if ((to < TopRowIndex && dest < TopRowIndex)
                || (to > ViewLayout.LastVisibleRow && target > ViewLayout.LastVisibleRow))
            {
                return; //// nothing to do
            }           
            else if (to < TopRowIndex && dest >= TopRowIndex)
            { 
                //// Rows have been moved from above the top row
            //// to below the top row.
                BeginUpdate(BeginUpdateOptions.None);
                try
                {
                    TopRowIndex -= count; //// don't update ... (could do BeginUpdate ..)
                }
                catch (Exception ex)
                {
                    TraceUtil.TraceExceptionCatched(ex);
                    if (!ExceptionManager.RaiseExceptionCatched(this, ex))
                    {
                        throw;
                    }
                }
               
                EndUpdate(false);
            }
            //// else .. more here

            this.recalcScrollBars = ScrollBars.Vertical;
            Invalidate();
        }

        void ModelColsRangeMoving(object sender, GridRangeMovingEventArgs e)
        {
            activateCCafterRangeOp = false;

            if (!e.Cancel)
            {
                if (CurrentCell.IsLocked)
                {
                    OnSelectionFrameChanging(GraphicsEventArgs.Empty);
                }
                else if (CurrentCell.ConfirmChanges())
                {
                    activateCCafterRangeOp = CurrentCell.HasCurrentCell;
                    CurrentCell.Deactivate(true);
                    OnSelectionFrameChanging(GraphicsEventArgs.Empty);
                }
                else
                {
                    if (CurrentCell.ErrorMessage.Length > 0)
                    {
                        CurrentCell.DisplayWarningText(CurrentCell.ErrorMessage);
                        CurrentCell.ResetError();
                    }

                    e.Cancel = true;
                }
            }
        }

        void ModelColsRangeMoved(object sender, GridRangeMovedEventArgs e)
        {
            try
            {
                if (!e.Success)
                {
                    return;
                }

                ViewLayout.Reset();
                UpdateColsMoved(e.From, e.Count, e.Target);
                ViewLayout.Reset();
            }
            finally
            {
                OnSelectionFrameChanged(GraphicsEventArgs.Empty);
            }
        }

        void UpdateColsMoved(int from, int count, int target)
        {
            int to = from + count - 1;
            int dest = target > from ? target + count : target;

            BeginUpdate(BeginUpdateOptions.None);
            try
            {
                if (activateCCafterRangeOp)
                {
                    CurrentCell.UpdateColsMoved(from, to, target);
                }

                ScrollGrid.RecalcHiddenColState(Math.Min(from, target), Math.Max(dest, from) + count);
                if (to < LeftColIndex && dest < LeftColIndex)
                {
                     //// nothing to do
                }                
                else if (to < LeftColIndex && dest >= LeftColIndex)
                {
                    //// Cols have been moved from above the top row
                //// to below the top row.
                    LeftColIndex -= count; //// don't update ... (could do BeginUpdate ..)
                }
                //// else .. more here
            }
            catch (Exception ex)
            {
                TraceUtil.TraceExceptionCatched(ex);
                if (!ExceptionManager.RaiseExceptionCatched(this, ex))
                {
                    throw;
                }
            }

            EndUpdate(false);
            this.recalcScrollBars = ScrollBars.Horizontal;
            Invalidate();
        }
        #endregion
        #region Insert

        void ModelRowsRangeInserting(object sender, GridRangeInsertingEventArgs e)
        {
            activateCCafterRangeOp = false;

            if (!e.Cancel)
            {
                ////NotifyBeforeUpdate();
                if (CurrentCell.IsLocked)
                {
                    OnSelectionFrameChanging(GraphicsEventArgs.Empty);
                }
                else if (CurrentCell.ConfirmChanges())
                {
                    activateCCafterRangeOp = CurrentCell.HasCurrentCell;
                    CurrentCell.Deactivate(true);
                    OnSelectionFrameChanging(GraphicsEventArgs.Empty);
                }
                else
                {
                    if (CurrentCell.ErrorMessage.Length > 0)
                    {
                        CurrentCell.DisplayWarningText(CurrentCell.ErrorMessage);
                        CurrentCell.ResetError();
                    }

                    e.Cancel = true;
                }
            }
        }

        bool optimizeInsertRemoveCells = false;

        /// <summary>
        /// Gets or sets a value indicating whether inserting and removing cells should be optimized by scrolling window contents
        /// and only invalidating new cells. Otherwise the whole area is repainted (this is the default).
        /// </summary>
        [DefaultValue(false)]
        [Description("Enables optimization for inserting and removing cells by scrolling window contents and only invalidating new cells.")]
        [Category("Grid")]
        public virtual bool OptimizeInsertRemoveCells
        {
            get
            {
                return optimizeInsertRemoveCells;
            }

            set
            {
                optimizeInsertRemoveCells = value;
            }
        }

        void ModelRowsRangeInserted(object sender, GridRangeInsertedEventArgs e)
        {
            try
            {
                if (!e.Success)
                {
                    return;
                }

                if (OptimizeInsertRemoveCells)
                {
                    ScrollGrid.RecalcHiddenRowState(e.InsertAt, e.InsertAt + e.Count);

                    //// Adjust m_nTopRow when all rows were frozen
                    if (ScrollGrid.m_nTopRow == InternalGetFrozenRows())
                    {
                        ScrollGrid.m_nTopRow++;
                        ViewLayout.Reset();
                    }

                    if (!activateCCafterRangeOp && CurrentCell.RowIndex >= e.InsertAt)
                    {
                        CurrentCell.SetCurrentCellNoActivate(CurrentCell.RowIndex + e.Count, CurrentCell.ColIndex);
                    }

                    Rectangle rc = this.ViewLayout.RectangleBottomOfRow(e.InsertAt, GridCellSizeKind.VisibleSize);
                    if (!(e.InsertAt > InternalGetFrozenRows() && e.InsertAt + e.Count <= TopRowIndex) && !rc.IsEmpty)
                    {
                        int yAmount = 0;
                        int n = 0;
                        int[] rowColSizes = new int[0];
                        if (e.InsertRangeOptions != null && e.InsertRangeOptions.RowColSizes != null)
                        {
                            rowColSizes = e.InsertRangeOptions.RowColSizes;
                        }

                        for (; n < rowColSizes.Length; n++)
                        {
                            if (e.InsertRangeOptions.RowColHide == null || n >= e.InsertRangeOptions.RowColHide.Length || !e.InsertRangeOptions.RowColHide[n])
                            {
                                yAmount += rowColSizes[n];
                            }
                        }

                        for (; n < e.Count; n++)
                        {
                            if (e.InsertRangeOptions == null || e.InsertRangeOptions.RowColHide == null || n >= e.InsertRangeOptions.RowColHide.Length || !e.InsertRangeOptions.RowColHide[n])
                            {
                                yAmount += Model.Rows.DefaultSize;
                            }
                        }

                        ScrollWindow(0, yAmount, rc, rc, false);
                    }
                    else
                    {
                        if (e.InsertAt < TopRowIndex)
                        {
                            this.InternalSetTopRow(TopRowIndex + e.Count);
                            this.ViewLayout.Reset();
                        }
                    }

                    if (activateCCafterRangeOp)
                    {
                        CurrentCell.UpdateInsertRows(e.InsertAt, e.Count);
                    }

                    this.recalcScrollBars = ScrollBars.Vertical;
                    ////Update();
                    UpdateScrollBars();
                }
                else
                {
                    ViewLayout.Reset();
                    UpdateInsertRows(e.InsertAt, e.Count);
                }
            }
            finally
            {
                OnSelectionFrameChanged(GraphicsEventArgs.Empty);
                ////                NotifyAfterUpdate();
            }
        }

        void UpdateInsertRows(int rowIndex, int nCount)
        {
            if (activateCCafterRangeOp)
            {
                CurrentCell.UpdateInsertRows(rowIndex, nCount);
            }

            ScrollGrid.RecalcHiddenRowState(rowIndex, rowIndex + nCount);

            //// Adjust m_nTopRow when all rows were frozen
            if (ScrollGrid.m_nTopRow == InternalGetFrozenRows())
            {
                ScrollGrid.m_nTopRow = ScrollGrid.GetNextRowIndex(ScrollGrid.m_nTopRow, false);
                ViewLayout.Reset();
            }

            this.recalcScrollBars = ScrollBars.Vertical;
            Invalidate();
            UpdateScrollBars();
        }

        void ModelColsRangeInserting(object sender, GridRangeInsertingEventArgs e)
        {
            activateCCafterRangeOp = false;

            if (!e.Cancel)
            {
                ////                NotifyBeforeUpdate();
                if (CurrentCell.IsLocked)
                {
                    OnSelectionFrameChanging(GraphicsEventArgs.Empty);
                }
                else if (CurrentCell.ConfirmChanges())
                {
                    activateCCafterRangeOp = CurrentCell.HasCurrentCell;
                    CurrentCell.Deactivate(true);
                    OnSelectionFrameChanging(GraphicsEventArgs.Empty);
                }
                else
                {
                    if (CurrentCell.ErrorMessage.Length > 0)
                    {
                        CurrentCell.DisplayWarningText(CurrentCell.ErrorMessage);
                        CurrentCell.ResetError();
                    }

                    e.Cancel = true;
                }
            }
        }

        void ModelColsRangeInserted(object sender, GridRangeInsertedEventArgs e)
        {
            try
            {
                if (!e.Success)
                {
                    return;
                }

                if (OptimizeInsertRemoveCells)
                {
                    if (activateCCafterRangeOp)
                    {
                        CurrentCell.UpdateInsertCols(e.InsertAt, e.Count);
                    }

                    ScrollGrid.RecalcHiddenColState(e.InsertAt, e.InsertAt + e.Count);

                    //// Adjust m_nLeftCol when all cols were frozen
                    if (ScrollGrid.m_nLeftCol == InternalGetFrozenCols())
                    {
                        ScrollGrid.m_nLeftCol++;
                        ViewLayout.Reset();
                    }

                    if (!activateCCafterRangeOp && CurrentCell.ColIndex >= e.InsertAt)
                    {
                        CurrentCell.SetCurrentCellNoActivate(CurrentCell.RowIndex, CurrentCell.ColIndex + e.Count);
                    }

                    Rectangle rc = this.ViewLayout.RectangleRightOfCol(e.InsertAt, GridCellSizeKind.VisibleSize);
                    if (!(e.InsertAt > InternalGetFrozenCols() && e.InsertAt + e.Count <= LeftColIndex) && !rc.IsEmpty)
                    {
                        int xAmount = 0;
                        int n = 0;
                        int[] rowColSizes = new int[0];
                        if (e.InsertRangeOptions != null && e.InsertRangeOptions.RowColSizes != null)
                        {
                            rowColSizes = e.InsertRangeOptions.RowColSizes;
                        }

                        for (; n < rowColSizes.Length; n++)
                        {
                            if (e.InsertRangeOptions.RowColHide == null || n >= e.InsertRangeOptions.RowColHide.Length || !e.InsertRangeOptions.RowColHide[n])
                            {
                                xAmount += rowColSizes[n];
                            }
                        }

                        for (; n < e.Count; n++)
                        {
                            if (e.InsertRangeOptions == null || e.InsertRangeOptions.RowColHide == null || n >= e.InsertRangeOptions.RowColHide.Length || !e.InsertRangeOptions.RowColHide[n])
                            {
                                xAmount += Model.Cols.DefaultSize;
                            }
                        }

                        ScrollWindow(xAmount, 0, rc, rc, false);
                    }
                    else
                    {
                        if (e.InsertAt < LeftColIndex)
                        {
                            this.InternalSetLeftCol(LeftColIndex + e.Count);
                            ViewLayout.Reset();
                        }
                    }

                    this.recalcScrollBars = ScrollBars.Horizontal;
                    ////Update();
                    UpdateScrollBars();
                }
                else
                {
                    ViewLayout.Reset();
                    UpdateInsertCols(e.InsertAt, e.Count);
                }
            }
            finally
            {
                OnSelectionFrameChanged(GraphicsEventArgs.Empty);
                ////                NotifyAfterUpdate();
            }
        }

        void UpdateInsertCols(int colIndex, int nCount)
        {
            if (activateCCafterRangeOp)
            {
                CurrentCell.UpdateInsertCols(colIndex, nCount);
            }

            ScrollGrid.RecalcHiddenColState(colIndex, colIndex + nCount);

            // Adjust m_nLeftCol when all cols were frozen
            if (ScrollGrid.m_nLeftCol == InternalGetFrozenCols())
            {
                ScrollGrid.m_nLeftCol = ScrollGrid.GetNextColIndex(ScrollGrid.m_nLeftCol, false);
                ViewLayout.Reset();
            }

            this.recalcScrollBars = ScrollBars.Horizontal;
            Invalidate();
            UpdateScrollBars();
        }

        #endregion
        #region Redraw
        ////////////////////////////////////////////////////////////////////////////
        //// Drawing cells

        ////protected override void ScrollWindowInvalidate(Rectangle r)
        ////{
        ////    DiscardPaintMessages();
        ////    this.DrawClippedGrid(this.GetCachedGraphics(), r);
        ////}

        int ignoreInvalidate;

        bool ignoreCurrentCellInvalidate = false;

        /// <summary>
        /// Gets or sets a value indicating whether it prevents any calls to <see cref="Invalidate()"/> to have any effect when current cell is being moved
        /// or current cell is activated or deactivated.
        /// </summary>
        [Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public bool IgnoreCurrentCellInvalidate
        {
            get { return ignoreCurrentCellInvalidate; }
            set { ignoreCurrentCellInvalidate = value; }
        }

        /// <summary>
        /// Prevents any subsequent calls to <see cref="Invalidate()"/> to have any effect.
        /// </summary>
        public void SuspendInvalidate()
        {
            this.GetGridWindow().ignoreInvalidate++;
        }

        /// <summary>
        /// Resumes normal operation for <see cref="Invalidate()"/> calls.
        /// </summary>
        public void ResumeInvalidate()
        {
            if (this.GetGridWindow().ignoreInvalidate > 0)
            {
                this.GetGridWindow().ignoreInvalidate--;
            }
        }

        /// <summary>
        /// Determines if calls to <see cref="Invalidate()"/> should have any effect. If true
        /// any calls to Invalidate will immediately return.
        /// </summary>
        /// <returns>True if <see cref="Invalidate()"/> should not have any effect. </returns>
        public bool ShouldIgnoreInvalidate()
        {
            return GetGridWindow().ignoreInvalidate > 0 || (IgnoreCurrentCellInvalidate && (CurrentCell.IsInActiveOrDeactivate || CurrentCell.IsInMoveTo));
        }

        /// <overload>
        /// Call <see cref="Control.Invalidate()"/> of the parent control with window handle (see <see cref="GetWindow"/>).
        /// </overload>
        /// <summary>
        /// Call <see cref="Control.Invalidate()"/> of the parent control with window handle (see <see cref="GetWindow"/>).
        /// </summary>
        public new virtual void Invalidate()
        {
            if (ShouldIgnoreInvalidate())
            {
                return;
            }

            GetWindow().Invalidate();
            if (traceInvalidate)
            {
                TraceUtil.TraceCurrentMethodInfo();
                TraceUtil.TraceCalledFrom(10);
            }
        }

        /// <summary>
        /// Invalidates the specified bounds and optionally hides the current cell renderer if it is visible inside the bounds.
        /// </summary>
        /// <param name="bounds">The area to invalidate.</param>
        public void InternalInvalidate(Rectangle bounds)
        {
            if (CurrentCell.HasCurrentCell && CurrentCell.HasControlFocus)
            {
                GridCellRendererBase cellRenderer = CurrentCell.Renderer;
                Control c = cellRenderer.Control;
                if (c != null && c.Bounds.IntersectsWith(bounds))
                {
                    cellRenderer.Hide();
                }
            }

            if (this.ShouldPrepareUpdate(true) && !bounds.IsEmpty)
            {
                Invalidate(bounds);
            }
        }

        bool traceInvalidate = false;

        /// <summary>
        /// Call <see cref="Control.Invalidate()"/> of the parent control with window handle (see <see cref="GetWindow"/>).
        /// </summary>
        /// <param name="rc">The area to invalidate.</param>
        public new virtual void Invalidate(Rectangle rc)
        {
            if (ShouldIgnoreInvalidate())
            {
                return;
            }

            GetWindow().Invalidate(rc);

            if (traceInvalidate)
            {
                ////SSif (rc.Top == 0 && Control.ModifierKeys == Keys.Control)
                ////SS    Debugger.Break();
                TraceUtil.TraceCurrentMethodInfo(rc);
                TraceUtil.TraceCalledFrom(10);
            }
        }

        /// <summary>
        /// Call <see cref="Control.Invalidate()"/> of the parent control with window handle (see <see cref="GetWindow"/>).
        /// </summary>
        /// <param name="rc">The area to invalidate.</param>
        /// <param name="invalidateChildren">Specified if child controls should also be invalidated.</param>
        public new virtual void Invalidate(Rectangle rc, bool invalidateChildren)
        {
            if (ShouldIgnoreInvalidate())
            {
                return;
            }

            GetWindow().Invalidate(rc, invalidateChildren);
            if (traceInvalidate)
            {
                TraceUtil.TraceCurrentMethodInfo(rc);
                TraceUtil.TraceCalledFrom(10);
            }
        }

        /// <summary>
        /// This is called from <see cref="GridCurrentCell.Deactivate"/> method of the <see cref="GridControlBase.CurrentCell"/>
        /// after the current cell was deactivated. The default version of this methods checks Model.Options.RefreshCurrentCellBehavior
        /// and invalidates the grid area (either the whole row, the cell or nothing) as needed.
        /// </summary>
        /// <param name="rowIndex">The row index</param>
        /// <param name="colIndex">The column index</param>
        /// <param name="savedBounds">The current cell bounds before it was deactivated.</param>
        protected internal virtual void InvalidateDeactivatedCurrentCell(int rowIndex, int colIndex, Rectangle savedBounds)
        {
            bool invalidated = false;
            GridCurrentCell gcc = CurrentCell;

            if (!Visible)
            {
                return;
            }

            if (Model.Options.RefreshCurrentCellBehavior == GridRefreshCurrentCellBehavior.RefreshRow)
            {
                if (!gcc.IsInMoveTo || gcc.MoveToRowIndex != rowIndex)
                {
                    if (IsVisibleCell(rowIndex, 0))
                    {
                        invalidated = true;
                        Rectangle bounds = RangeInfoToRectangle(GridRangeInfo.Row(rowIndex), GridRangeOptions.MergeAllSpannedCells);
                        if (bounds.IsEmpty)
                        {
                            bounds = savedBounds;
                        }
                        else if (!savedBounds.IsEmpty)
                        {
                            bounds = Rectangle.Union(bounds, savedBounds);
                        }

                        Invalidate(bounds, false);
                    }
                }
            }

            if (!invalidated && (IsVisibleCell(rowIndex, colIndex) || !savedBounds.IsEmpty))
            {
                Rectangle bounds = RangeInfoToRectangle(GridRangeInfo.Cell(rowIndex, colIndex), GridRangeOptions.MergeAllSpannedCells);
                if (bounds.IsEmpty)
                {
                    bounds = savedBounds;
                }
                else if (!savedBounds.IsEmpty)
                {
                    bounds = Rectangle.Union(bounds, savedBounds);
                }

                Invalidate(bounds, false);
            }
        }
        
        /// <overload>
        /// Invalidates the region of the grid control specified with a range of cells and
        /// causes a paint message to be sent to the control.
        /// </overload>
        /// <summary>
        /// Invalidates the region of the grid control specified with a range of cells and
        /// causes a paint message to be sent to the control.
        /// </summary>
        /// <param name="range">The range that defines the region to be invalidated.</param>
        public void InvalidateRange(GridRangeInfo range)
        {
            ////            TraceUtil.TraceCurrentMethodInfo(range);
            ////TraceUtil.TraceCurrentMethodInfoIf(Switches.BeginEndUpdate.TraceVerbose, PaneDesc, range);
            ////            if (range.IsRows && range.Top == 3 && range.Bottom == 3)
            ////                Console.WriteLine("Visible {0}", ViewLayout.VisibleCellsRange);

            if (!Visible)
            {
                return;
            }

            if (IsVisibleCell(range.Top, range.Left) || ViewLayout.VisibleCellsRange.IntersectsWith(range))
            {
                InternalInvalidate(RangeInfoToRectangle(range));
            }

            if (traceInvalidate)
            {
                TraceUtil.TraceCurrentMethodInfo(range);
                TraceUtil.TraceCalledFrom(10);
            }
        }

        /// <summary>
        /// Invalidates the region of the grid control specified with a range of cells and
        /// causes a paint message to be sent to the control.
        /// </summary>
        /// <param name="range">The range that defines the region to be invalidated.</param>
        /// <param name="options">Options that indicate if method should enlarge the affected range of cells to include covered and floating cells.</param>
        public void InvalidateRange(GridRangeInfo range, GridRangeOptions options)
        {
            ////            TraceUtil.TraceCurrentMethodInfo(range);
#if DEBUG
            if (Switches.BeginEndUpdate.TraceVerbose)
            {
                TraceUtil.TraceCurrentMethodInfo(PaneDesc, range);
            }
#else
            ;
#endif

            //// TODO: range.IsRows etc.

            if (!Visible)
            {
                return;
            }

            if (IsVisibleCell(range.Top, range.Left) || ViewLayout.VisibleCellsRange.IntersectsWith(range))
            {
                InternalInvalidate(RangeInfoToRectangle(range, options));
            }
        }

        /// <overload>
        /// Invalidates the region of the grid control specified with a range of cells and
        /// immediately causes a paint message to be sent to the control before the function returns.
        /// </overload>
        /// <summary>
        /// Invalidates the region of the grid control specified with a range of cells and
        /// immediately causes a paint message to be sent to the control before the function returns.
        /// </summary>
        /// <param name="range">The range that defines the region to be invalidated.</param>
        public void RefreshRange(GridRangeInfo range)
        {
#if DEBUG
            if (Switches.BeginEndUpdate.TraceVerbose)
            {
                TraceUtil.TraceCurrentMethodInfo(PaneDesc, range);
            }
#else
            ;
#endif

            if (this.ShouldPrepareUpdate(true))
            {
                Model.ResetVolatileData();
                InternalInvalidate(RangeInfoToRectangle(range));
                Update();
            }
        }

        /// <summary>
        /// Invalidates the region of the grid control specified with a range of cells and
        /// immediately causes a paint message to be sent to the control before the function returns.
        /// </summary>
        /// <param name="range">The range that defines the region to be invalidated.</param>
        /// <param name="options">Options that indicate if method should enlarge the affected range of cells to include covered and floating cells.</param>
        public void RefreshRange(GridRangeInfo range, GridRangeOptions options)
        {
#if DEBUG
            if (Switches.BeginEndUpdate.TraceVerbose)
            {
                TraceUtil.TraceCurrentMethodInfo(PaneDesc, range);
            }
#else
            ;
#endif

            if (this.ShouldPrepareUpdate(true))
            {
                Model.ResetVolatileData();
                InternalInvalidate(RangeInfoToRectangle(range, options));
                Update();
            }
        }

        /// <summary>
        /// Invalidates the region of the grid control specified with a range of cells and
        /// immediately causes a paint message to be sent to the control before the function returns.
        /// </summary>
        /// <param name="range">The range that defines the region to be invalidated.</param>
        /// <param name="forceRefreshCurrentCell">When a current cell is active and within the given
        /// range of cells, this option specifies if the current cell should be reinitialized
        /// with the underlying cell's value.</param>
        public void RefreshRange(GridRangeInfo range, bool forceRefreshCurrentCell)
        {
#if DEBUG
            if (Switches.BeginEndUpdate.TraceVerbose)
            {
                TraceUtil.TraceCurrentMethodInfo(PaneDesc, range);
            }
#else
            ;
#endif

            if (this.ShouldPrepareUpdate(true))
            {
                Model.ResetVolatileData();
                InternalInvalidate(RangeInfoToRectangle(range));
                if (forceRefreshCurrentCell && range.Contains(CurrentCell.RangeInfo))
                {
                    CurrentCell.Refresh();
                }

                Update();
            }
        }

        /// <summary>
        /// Invalidates the region of the grid control specified with a range of cells and
        /// immediately causes a paint message to be sent to the control before the function returns.
        /// </summary>
        /// <param name="range">The range that defines the region to be invalidated.</param>
        /// <param name="options">Options that indicate if method should enlarge the affected range of cells to include covered and floating cells.</param>
        /// <param name="forceRefreshCurrentCell">When a current cell is active and within the given
        /// range of cells, this option specifies if the current cell should be reinitialized
        /// with the underlying cell's value.</param>
        public void RefreshRange(GridRangeInfo range, GridRangeOptions options, bool forceRefreshCurrentCell)
        {
#if DEBUG
            if (Switches.BeginEndUpdate.TraceVerbose)
            {
                TraceUtil.TraceCurrentMethodInfo(PaneDesc, range);
            }
#else
            ;
#endif

            if (this.ShouldPrepareUpdate(true))
            {
                Model.ResetVolatileData();
                InternalInvalidate(RangeInfoToRectangle(range));
                if (forceRefreshCurrentCell && range.Contains(CurrentCell.RangeInfo))
                {
                    CurrentCell.Refresh();
                }

                Update();
            }
        }

        /// <overload>
        /// <summary>
        /// Forces the control to invalidate its client area and immediately redraw itself and any child controls.
        /// </summary>
        /// </overload>
        /// <param name="fromModel">True if the grid model needs to be refreshed.</param>
        /// <override/>
        protected virtual void Refresh(bool fromModel)
        {
            Model.ResetVolatileData();
            ViewLayout.Reset();
            CurrentCell.Refresh();
            UpdateScrollBars();
            base.Refresh();
        }

        /// <override/>
        /// <summary>
        /// Forces the control to invalidate its client area and immediately redraw itself
        /// and any child controls.
        /// </summary>
        public override sealed void Refresh()
        {
            Refresh(false);
        }
        #endregion

        #region Scrolling
        internal new void UpdateStyles()
        {
            base.UpdateStyles();
        }

        /// <overload>
        /// Scrolls the specified cell into view.
        /// </overload>
        /// <summary>
        /// Scrolls the specified range into view.
        /// </summary>
        /// <param name="range">The range that should be scrolled into the visible grid view area.</param>
        /// <returns>True if scrolling the grid was necessary; False if the range was already in the visible area.</returns>
        public bool ScrollCellInView(GridRangeInfo range)
        {
            int rowIndex = range.IsCols ? TopRowIndex : range.Top;
            int colIndex = range.IsRows ? LeftColIndex : range.Left;
            return ScrollCellInViewInt(rowIndex, colIndex, false, GridScrollCurrentCellReason.Any);
        }

        /// <summary>
        /// Scrolls the specified range into view.
        /// </summary>
        /// <param name="range">The range that should be scrolled into the visible grid view area.</param>
        /// <param name="reason">The reason for scrolling the current cell into view (e.g. KeyPress, GridFocus etc.)</param>
        /// <returns>True if scrolling the grid was necessary; False if the range was already in the visible area.</returns>
        public bool ScrollCellInView(GridRangeInfo range, GridScrollCurrentCellReason reason)
        {
            int rowIndex = range.IsCols ? TopRowIndex : range.Top;
            int colIndex = range.IsRows ? LeftColIndex : range.Left;
            return ScrollCellInViewInt(rowIndex, colIndex, false, reason);
        }

        internal bool ScrollCellInViewInt(GridRangeInfo range, bool scrollFloatingCell, GridScrollCurrentCellReason reason)
        {
            int rowIndex = range.IsCols ? TopRowIndex : range.Top;
            int colIndex = range.IsRows ? LeftColIndex : range.Left;
            return ScrollCellInViewInt(rowIndex, colIndex, scrollFloatingCell, reason);
        }

        /// <summary>
        /// Scrolls the specified cell into view.
        /// </summary>
        /// <param name="rowIndex">The row index. 0 if rows should not scroll.</param>
        /// <param name="colIndex">The column index. 0 if columns should not scroll.</param>
        /// <returns>True if scrolling the grid was necessary; False if the range was already in the visible area.</returns>
        public bool ScrollCellInView(int rowIndex, int colIndex)
        {
            return ScrollCellInViewInt(rowIndex, colIndex, false, GridScrollCurrentCellReason.Any);
        }

        /// <summary>
        /// Scrolls the specified cell into view.
        /// </summary>
        /// <param name="rowIndex">The row index. 0 if rows should not scroll.</param>
        /// <param name="colIndex">The column index. 0 if columns should not scroll.</param>
        /// <param name="reason">The reason for scrolling the current cell into view (e.g. KeyPress, GridFocus etc.)</param>
        /// <returns>True if scrolling the grid was necessary; False if the range was already in the visible area.</returns>
        public bool ScrollCellInView(int rowIndex, int colIndex, GridScrollCurrentCellReason reason)
        {
            return ScrollCellInViewInt(rowIndex, colIndex, false, reason);
        }

        internal bool ScrollCellInViewInt(int rowIndex, int colIndex, bool scrollFloatingCell, GridScrollCurrentCellReason reason)
        {
            return ScrollCellInViewInt(rowIndex, colIndex, scrollFloatingCell, false, reason);
        }

        internal bool ScrollCellInViewInt(int rowIndex, int colIndex, bool scrollFloatingCell, bool dontScroll, GridScrollCurrentCellReason reason)
        {
#if DEBUG
            if (Switches.GridScrolling.TraceVerbose)
            {
                Trace.WriteLine(PaneDesc, String.Format("ScrollCellInView({0},{1})", rowIndex, colIndex));
            }
#endif

            GridRangeInfo rgCovered;
            if (Model.CoveredRanges.Find(rowIndex, colIndex, out rgCovered))
            {
                //// If user clicked on a covered cell which is not
                //// completely visible and is used in the frozen
                //// cells range, scroll the grid to the most-left
                //// or most-top cell.

                if (rgCovered.Bottom > InternalGetFrozenRows())
                {
                    rowIndex = Math.Max(GetFirstScrollableRow(), rowIndex);
                }

                if (rgCovered.Right > InternalGetFrozenCols())
                {
                    colIndex = Math.Max(GetFirstScrollableCol(), colIndex);
                }
            }

            if (scrollFloatingCell)
            {
                GridRangeInfo rgFloated;
                if (Model.FloatingCells.Find(rowIndex, colIndex, out rgFloated))
                {
                    //// If user clicked on a Floated cell which is not
                    //// completely visible and is used in the frozen
                    //// cells range, scroll the grid to the most-left
                    //// or most-top cell.

                    if (rgFloated.Bottom > InternalGetFrozenRows())
                    {
                        rowIndex = Math.Max(GetFirstScrollableRow(), rowIndex);
                    }

                    if (rgFloated.Right > InternalGetFrozenCols())
                    {
                        colIndex = Math.Max(GetFirstScrollableCol(), colIndex);
                    }
                }
            }

            GridQueryScrollCellInViewEventArgs ea = new GridQueryScrollCellInViewEventArgs(rowIndex, colIndex, reason);
            OnQueryScrollCellInView(ea);
            if (ea.Handled)
            {
                return ea.Result;
            }

            GridScrollCurrentCellReason r = reason & Model.Options.AllowScrollCurrentCellInView;
            if (r == GridScrollCurrentCellReason.None)
            {
                return false;
            }

            return ProcessScrollCellInView(ea.RowIndex, ea.ColIndex, dontScroll, ea.Reason);
        }

        /// <summary>
        /// Occurs before a cell is scrolled into view by a ScrollCellInView call. Normally, the current
        /// cell is checked if it is inside the visible grid area when certain user events occur such as when a key is pressed or when the grid got focus.
        /// The event is called to check whether the specified cell is in view. If the cell is not in view, the grid will scroll the cell into view.
        /// You can hook into this
        /// mechanism by implementing an event handler for this event.
        /// </summary>
        [Category("Scrolling")]
        [Description("Occurs before a cell is scrolled into view by a ScrollCellInView call.")]
        public event GridQueryScrollCellInViewEventHandler QueryScrollCellInView;

        /// <summary>
        /// Raises the <see cref="QueryScrollCellInView"/> event.
        /// </summary>
        /// <param name="e">A <see cref="GridQueryScrollCellInViewEventArgs" /> that contains the event data.</param>
        protected virtual void OnQueryScrollCellInView(GridQueryScrollCellInViewEventArgs e)
        {
            if (this.eventsTarget != null)
            {
                this.eventsTarget.OnQueryScrollCellInView(e);
            }
#if DEBUG

            if (Switches.GridControlBaseEvents.TraceVerbose)
            {
                TraceUtil.TraceCurrentMethodInfo(PaneDesc, e);
            }
#else

            ;
#endif

            if (QueryScrollCellInView != null)
            {
                QueryScrollCellInView(this, e);
            }
        }

        /// <summary>
        /// Called to scroll the specified cell into view.
        /// </summary>
        /// <param name="rowIndex">The row index. 0 if rows should not scroll.</param>
        /// <param name="colIndex">The column index. 0 if columns should not scroll.</param>
        /// <param name="dontScroll">Specified if top row index and left col index should be changed without raising scroll events
        /// and without updating the screen (calling DoScroll). Used by GridNestedTableControl grids in GridGroupingControl.
        /// Normally this should be false for other scenarios.</param>
        /// <param name="reason">The reason for scrolling the current cell into view (e.g. KeyPress, GridFocus etc.)</param>
        /// <returns>True if scrolling the grid was necessary; False if the range was already in the visible area.</returns>      
        public virtual bool ProcessScrollCellInView(int rowIndex, int colIndex, bool dontScroll, GridScrollCurrentCellReason reason)
        {
            Rectangle rectGrid = GridBounds;

            ////            if (this.HScroll)
            ////                rectGrid.Height -= System.Windows.Forms.SystemInformation.HorizontalScrollBarHeight;
            ////            if (this.VScroll)
            ////                rectGrid.Width -= System.Windows.Forms.SystemInformation.VerticalScrollBarWidth;
            int bottomRow,
                rightCol;
            bool bScrolled = false;

            int nRowCount = Model.RowCount;
            int nColCount = Model.ColCount;

            bottomRow = ViewLayout.LastVisibleRow;
            rightCol = ViewLayout.LastVisibleCol;

            Rectangle cellBounds = Rectangle.Empty;

            if (colIndex > 0 && rowIndex > 0)
            {
                cellBounds = ViewLayout.RangeInfoToRectangle(GridRangeInfo.Cell(rowIndex, colIndex), false, GridCellSizeKind.ActualSize);
            }

            if (this.hPixelScroll)
            {
                if (!cellBounds.IsEmpty && colIndex > InternalGetFrozenCols())
                {
                    bScrolled |= this.HScrollPixelScrollInView(cellBounds);
                }
            }

            if (this.vPixelScroll)
            {
                if (!cellBounds.IsEmpty && rowIndex > InternalGetFrozenRows())
                {
                    bScrolled |= this.VScrollPixelScrollInView(cellBounds);
                }
            }

            if (cellBounds.IsEmpty || !hPixelScroll)
            {
                //// left of visible area
                if (colIndex > InternalGetFrozenCols() && (colIndex < ScrollGrid.m_nLeftCol
                    || (colIndex == ScrollGrid.m_nLeftCol && this.hScrollPixelDelta > 0))) 
                {
                    if (!dontScroll)
                    {
                        LeftColIndex = colIndex;
                    }

                    bScrolled = true;
                }
                else if (colIndex >= rightCol && ViewLayout.HasPartialVisibleCols)
                {
                    if (!dontScroll)
                    {
                        GridCellRendererBase pControl = CurrentCell.Renderer;
                        if (pControl != null)
                        {
                            Message msg = Message.Create(GetWindow().Handle, NativeMethods.WM_HSCROLL, (IntPtr)0, (IntPtr)0);
                            pControl.OnNotifyMsg(ref msg);
                            if (msg.Msg == 0)
                            {
                                return false;
                            }
                        }

                        int leftCol = colIndex;
                        int nFrozen = GetVisibleFrozenCols();
                        int iWidth = GetColWidth(leftCol);
                        for (int i = 0; i < nFrozen; i++)
                        {
                            iWidth += GetColWidth(GetCol(i));
                        }

                        while (iWidth <= rectGrid.Width && leftCol > 1)
                        {
                            if (!ScrollGrid.GetPrevColIndex(ref leftCol))
                            {
                                break;
                            }

                            if (leftCol > 0)
                            {
                                iWidth += GetColWidth(leftCol);
                            }
                        }

                        if (iWidth > rectGrid.Width && leftCol < colIndex)
                        {
                            leftCol = ScrollGrid.GetNextColIndex(leftCol, false);
                        }

                        while (GetColWidth(leftCol) == 0)
                        {
                            leftCol = ScrollGrid.GetNextColIndex(leftCol, true);
                        }

                        LeftColIndex = Math.Min(leftCol, nColCount);
                    }

                    bScrolled = true;
                }
            }

            if (cellBounds.IsEmpty || !vPixelScroll)
            {
                //// above visible area
                if (rowIndex > InternalGetFrozenRows() && (rowIndex < ScrollGrid.m_nTopRow
                    || (rowIndex == ScrollGrid.m_nTopRow && this.vScrollPixelDelta > 0))) 
                {
                    if (!dontScroll)
                    {
                        TopRowIndex = rowIndex;
                    }

                    bScrolled = true;
                }
                else if (rowIndex >= bottomRow && ViewLayout.HasPartialVisibleRows)
                {
                    if (!dontScroll)
                    {
                        GridCellRendererBase pControl = CurrentCell.Renderer;
                        //// TODO: Make this an event
                        if (pControl != null)
                        {
                            Message msg = Message.Create(GetWindow().Handle, NativeMethods.WM_VSCROLL, (IntPtr)0, (IntPtr)0);
                            pControl.OnNotifyMsg(ref msg);
                            if (msg.Msg == 0)
                            {
                                return false;
                            }
                        }

                        int topRow = rowIndex;
                        int nFrozen = GetVisibleFrozenRows();
                        int iHeight = GetRowHeight(topRow);
                        for (int i = 0; i < nFrozen; i++)
                        {
                            iHeight += GetRowHeight(GetRow(i));
                        }

                        while (iHeight <= rectGrid.Height && topRow > 1)
                        {
                            if (!ScrollGrid.GetPrevRowIndex(ref topRow))
                            {
                                break;
                            }

                            if (topRow > 0)
                            {
                                iHeight += GetRowHeight(topRow);
                            }
                        }

                        if (iHeight > rectGrid.Height && topRow < rowIndex)
                        {
                            topRow = ScrollGrid.GetNextRowIndex(topRow, false);
                        }

                        while (GetRowHeight(topRow) == 0)
                        {
                            topRow = ScrollGrid.GetNextRowIndex(topRow, true);
                        }
                        bool updateRowIndex = true;
                        if ((reason == GridScrollCurrentCellReason.BeginEdit || reason == GridScrollCurrentCellReason.Click) && rowIndex == bottomRow && this.ViewLayout.LastVisibleRow.Equals(this.Model.RowCount))
                        {
                            if (!(this.GetType().Equals(typeof(GridControlBase)) || this.GetType().Equals(typeof(GridDataBoundGrid))))
                                updateRowIndex = false;
                        }

                        if(updateRowIndex)
                            TopRowIndex = Math.Min(topRow, nRowCount);

                        Update();
                    }

                    bScrolled = true;
                }
            }

            if (!dontScroll)
            {
                if (bScrolled)
                {
                    ViewLayout.Reset();
                    Update();
                }
            }

            return bScrolled;
        }

        GridScroll gridScroll;
        [Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        internal GridScroll ScrollGrid
        {
            get
            {
                if (gridScroll == null)
                {
                    gridScroll = new GridScroll(this);
                }

                return gridScroll;
            }
        }

        private GridOutlineCurrentCellHeader outlineCurrentCellHeader = null;
        [Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        internal GridOutlineCurrentCellHeader OutlineCurrentCellHeaderManager
        {
            get
            {
                ////                if (outlineCurrentCellHeader == null)
                ////                    outlineCurrentCellHeader = new GridOutlineCurrentCellHeader(this);
                return outlineCurrentCellHeader;
            }
        }

        /////////////////////////////////////////////////////////////////////////////
        //// Cell Coordinates

        /// <summary>
        /// Changes the top row index without raising events and without updating the grid display.
        /// </summary>
        /// <param name="rowIndex">The new top row index.</param>
        /// <param name="resetvScrollPixelDelta">True if top row should be aligned; false if partial visible row should stay as is</param>
        public virtual void InternalSetTopRow(int rowIndex, bool resetvScrollPixelDelta)
        {
            ScrollGrid.m_nTopRow = rowIndex;
            if (resetvScrollPixelDelta)
            {
                this.vScrollPixelDelta = 0;
            }
        }

        /// <summary>
        /// Changes the top row index without raising events and without updating the grid display.
        /// </summary>
        /// <param name="rowIndex">The new top row index.</param>
        public void InternalSetTopRow(int rowIndex)
        {
            InternalSetTopRow(rowIndex, true);
        }

        /// <summary>
        /// Changes the left column index without raising events and without updating the grid display.
        /// </summary>
        /// <param name="colIndex">The new left column index.</param>
        /// <param name="resethScrollPixelDelta">True if left column should be aligned; false if partial visible column should stay as is</param>
        public virtual void InternalSetLeftCol(int colIndex, bool resethScrollPixelDelta)
        {
            ScrollGrid.m_nLeftCol = colIndex;
            if (resethScrollPixelDelta)
            {
                this.hScrollPixelDelta = 0;
            }
        }

        /// <summary>
        /// Changes the left column index without raising events and without updating the grid display.
        /// </summary>
        /// <param name="colIndex">The new left column index.</param>
        public void InternalSetLeftCol(int colIndex)
        {
            InternalSetLeftCol(colIndex, true);
        }

        /// <summary>
        /// Changes the top row index and scrolls the grid.
        /// </summary>
        /// <param name="rowIndex">The new top row index.</param>
        public virtual void SetTopRow(int rowIndex)
        {
            int nnfr = GetFirstScrollableRow();
            //// rowIndex must greater than 0 and not greater than GetRowCount
            rowIndex = Math.Max(nnfr, Math.Min(rowIndex, Model.RowCount));
            ////TraceUtil.TraceCurrentMethodInfo(rowIndex);

            ////            else if (DisableScrollWindow)
            ////            {
            ////                //// give the programmer the possibility to inhibit scrolling
            ////                bool success = false;
            ////                if (NotifyTopRowChanging(rowIndex))
            ////                {
            ////                    ScrollGrid.m_nTopRow = rowIndex;
            ////                    ViewLayout.Reset();
            ////                    Invalidate();
            ////                    VScrollBar.Value = rowIndex;
            ////                    success = true;
            ////                }
            ////                NotifyTopRowChanged(success);
            ////            }

            if (PrintingMode)
            {
                PrintInfo.m_nPrintTopRow = rowIndex;
            }
            else if (CurrentCell.StaticDrawing || inOnPaint || this.firstPaint || ScrollGrid.m_nTopRow < nnfr)
            {
                bool success = false;
                if (NotifyTopRowChanging(rowIndex))
                {
                    ScrollGrid.m_nTopRow = rowIndex;
                    this.vScrollPixelDelta = 0;
                }

                NotifyTopRowChanged(success);
            }           
            else
            {
                rowIndex = Math.Min(Model.RowCount, Math.Max(rowIndex, nnfr));

#if MEASURE
                using (Syncfusion.Diagnostics.MeasureTime.Measure("SetTopRow.DoScroll"))
#endif
                {
                    if (GridControlBase.UseOldHiddenScrollLogic)
                    {
                        if (rowIndex < ScrollGrid.m_nTopRow)
                        {
                            ScrollGrid.DoScroll(GridDirectionType.Up, ScrollGrid.m_nTopRow - rowIndex);
                        }
                        else if (rowIndex > ScrollGrid.m_nTopRow)
                        {
                            ScrollGrid.DoScroll(GridDirectionType.Down, rowIndex - ScrollGrid.m_nTopRow);
                        }
                        else if (this.vScrollPixelDelta != 0)
                        {
                            ScrollGrid.DoScroll(GridDirectionType.Up, 1);
                        }
                    }
                    else
                    {
                        if (rowIndex < ScrollGrid.m_nTopRow)
                        {
                            ScrollGrid.DoScroll(GridDirectionType.Up, ScrollGrid.VScrollPos - ScrollGrid.RowIndexToScrollPosition(rowIndex));
                        }
                        else if (rowIndex > ScrollGrid.m_nTopRow)
                        {
                            ScrollGrid.DoScroll(GridDirectionType.Down, ScrollGrid.RowIndexToScrollPosition(rowIndex) - ScrollGrid.VScrollPos);
                        }
                        else if (this.vScrollPixelDelta != 0)
                        {
                            ScrollGrid.DoScroll(GridDirectionType.Up, 1);
                        }
                    }
                }
#if MEASURE
                using (Syncfusion.Diagnostics.MeasureTime.Measure("SetTopRow.Update"))
#endif
                {
                    Update();
                }
            }
        }

        /// <summary>
        /// Changes the left column index and scrolls the grid.
        /// </summary>
        /// <param name="colIndex">The new left column index.</param>
        public virtual void SetLeftCol(int colIndex)
        {
            int nnfc = GetFirstScrollableCol();
            //// colIndex must greater than 0 and not greater than GetColCount
            colIndex = Math.Max(nnfc, Math.Min(colIndex, Model.ColCount));

            ////           else if (DisableScrollWindow)
            ////            {
            ////                //// give the programmer the possibility to inhibit scrolling
            ////                bool success = false;
            ////                if (NotifyLeftColChanging(colIndex))
            ////                {
            ////                    ScrollGrid.m_nLeftCol = colIndex;
            ////                    ViewLayout.Reset();
            ////                    Invalidate();
            ////                    HScrollBar.Value = colIndex;
            ////                    success = true;
            ////                }
            ////                NotifyLeftColChanged(success);
            ////            }

            if (PrintingMode)
            {
                PrintInfo.m_nPrintLeftCol = colIndex;
            }
            else if (CurrentCell.StaticDrawing || inOnPaint || this.firstPaint || ScrollGrid.m_nLeftCol < nnfc)
            {
                bool success = false;
                if (NotifyLeftColChanging(colIndex))
                {
                    ScrollGrid.m_nLeftCol = colIndex;
                    this.hScrollPixelDelta = 0;
                }

                NotifyLeftColChanged(success);
            }           
            else
            {
                colIndex = Math.Min(Model.ColCount, Math.Max(colIndex, nnfc));

                if (GridControlBase.UseOldHiddenScrollLogic)
                {
                    if (colIndex < ScrollGrid.m_nLeftCol)
                    {
                        ScrollGrid.DoScroll(GridDirectionType.Left, ScrollGrid.m_nLeftCol - colIndex);
                    }
                    else if (colIndex > ScrollGrid.m_nLeftCol)
                    {
                        ScrollGrid.DoScroll(GridDirectionType.Right, colIndex - ScrollGrid.m_nLeftCol);
                    }
                    else if (this.hScrollPixelDelta != 0)
                    {
                        ScrollGrid.DoScroll(GridDirectionType.Left, 1);
                    }
                }
                else
                {
                    if (colIndex < ScrollGrid.m_nLeftCol)
                    {
                        ScrollGrid.DoScroll(GridDirectionType.Left, ScrollGrid.HScrollPos - ScrollGrid.ColIndexToScrollPosition(colIndex));
                    }
                    else if (colIndex > ScrollGrid.m_nLeftCol)
                    {
                        ScrollGrid.DoScroll(GridDirectionType.Right, ScrollGrid.ColIndexToScrollPosition(colIndex) - ScrollGrid.HScrollPos);
                    }
                    else if (this.hScrollPixelDelta != 0)
                    {
                        ScrollGrid.DoScroll(GridDirectionType.Left, 1);
                    }
                }                ////                else
                ////                    TraceUtil.TraceCurrentMethodInfo(colIndex);

                Update();
            }
        }

        void SetTopLeftRowCol(int rowIndex, int colIndex)
        {
            TopRowIndex = rowIndex;
            LeftColIndex = colIndex;
        }

        /////////////////////////////////////////////////////////////////////////////
        //// Convert between real and client coordinates

        /// <summary>
        /// Returns the client row index for an absolute row index.
        /// </summary>
        /// <param name="rowIndex">The absolute row index.</param>
        /// <returns>The client row index relative to the top row index.</returns>
        /// <remarks>
        /// Client row indexes indicate the visible client rows in the current view. <para/>
        /// Client row indexes are numbered from 0 to the number of visible rows. <para/>
        /// Absolute row indexes are independent of the scroll position.<para/>
        /// If you have a client row index you should first convert the client row index
        /// to an absolute row index before querying information about the row, such as row height or
        /// cell contents.
        /// </remarks>
        public int GetClientRow(int rowIndex)
        {
            if (UseOldHiddenScrollLogic)
            {
                return _GetClientRow(rowIndex, TopRowIndex);
            }

            ViewLayout.DemandInitialize();

            int row1 = ScrollGrid.RowIndexToScrollPosition(rowIndex);
            if (!GridControlBase.UseOldHiddenScrollLogic && rowIndex > Model.RowCount)
            {
                row1++;
            }

            return _GetClientRow(row1, ScrollGrid.VScrollPos);
        }

        int _GetClientRow(int rowIndex, int vScrollPos)
        {
            int nnfr = GetVisibleFrozenRows();  // can be -1 if header is hidden
            if (rowIndex >= nnfr && rowIndex < vScrollPos)
            {
                return 0;
            }

            return rowIndex != GridConstants.Undefined && rowIndex > nnfr
                ? rowIndex - Math.Max(vScrollPos - nnfr, 0)
                : rowIndex;
        }

        /// <summary>
        /// Returns the client column index for an absolute column index.
        /// </summary>
        /// <param name="colIndex">The absolute column index.</param>
        /// <returns>The client column index relative to the top column index.</returns>
        /// <remarks>
        /// Client column indexes indicate the visible client columns in the current view. <para/>
        /// Client column indexes are numbered from 0 to the number of visible columns. <para/>
        /// Absolute column indexes are independent of the scroll position.<para/>
        /// If you have a client column index, you should first convert the client column index
        /// to an absolute column index before querying information about the column, such as column height or
        /// cell contents.
        /// </remarks>
        public int GetClientCol(int colIndex)
        {
            if (UseOldHiddenScrollLogic)
            {
                return _GetClientCol(colIndex, LeftColIndex);
            }

            ViewLayout.DemandInitialize();

            int col1 = ScrollGrid.ColIndexToScrollPosition(colIndex);
            if (!GridControlBase.UseOldHiddenScrollLogic && colIndex > Model.ColCount)
            {
                col1++;
            }

            return _GetClientCol(col1, ScrollGrid.HScrollPos);
        }

        int _GetClientCol(int colIndex, int hScrollPos)
        {
            int nnfc = GetVisibleFrozenCols();      // can be -1 if header is hidden
            if (colIndex >= nnfc && colIndex < hScrollPos)
            {
                return 0;
            }

            return colIndex != GridConstants.Undefined && colIndex > nnfc
                ? colIndex - Math.Max(hScrollPos - nnfc, 0)
                : colIndex;
        }

        /// <summary>
        /// Returns the absolute row index for a client row index.
        /// </summary>
        /// <param name="nClientRow">The client row index.</param>
        /// <returns>The absolute row index calculated based on the given client row index and the top row index.</returns>
        public int GetRow(int nClientRow)
        {
            if (UseOldHiddenScrollLogic || model.Options.DisplayEmptyRows)
            {
                return _GetRow(nClientRow, TopRowIndex);
            }
            ViewLayout.DemandInitialize();
            return ScrollGrid.ScrollPositionToRowIndex(_GetRow(nClientRow, ScrollGrid.VScrollPos));
        }

        int _GetRow(int nClientRow, int vScrollPos)
        {
            int nnfr = GetVisibleFrozenRows();
            return nClientRow != GridConstants.Undefined && nClientRow >= nnfr
                ? nClientRow + Math.Max(vScrollPos - nnfr, 0)
                : nClientRow;
        }

        /// <summary>
        /// Returns the absolute column index for a client column index.
        /// </summary>
        /// <param name="nClientCol">The client column index.</param>
        /// <returns>The absolute column index calculated based on the given client column index and the left column index.</returns>
        public int GetCol(int nClientCol)
        {
            if (UseOldHiddenScrollLogic || model.Options.DisplayEmptyColumns)
            {
                return _GetCol(nClientCol, LeftColIndex);
            }
            return ScrollGrid.ScrollPositionToColIndex(_GetCol(nClientCol, ScrollGrid.HScrollPos));
        }

        int _GetCol(int nClientCol, int hScrollPos)
        {
            int nnfc = GetVisibleFrozenCols();
            return nClientCol != GridConstants.Undefined && nClientCol >= nnfc
                ? nClientCol + Math.Max(hScrollPos - nnfc, 0)
                : nClientCol;
        }

        /// <summary>
        /// Gets or sets the column index of the left column. Scrolls the grid when changed.
        /// </summary>
        [Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public int TopRowIndex
        {
            get
            {
                if (PrintingMode)
                {
                    return PrintInfo.m_nPrintTopRow;
                }

                return ScrollGrid.m_nTopRow;
            }

            set
            {
                //// scroll
                SetTopRow(value);
            }
        }

        /// <summary>
        /// Returns the maximum value for <see cref="TopRowIndex"/>.
        /// </summary>
        /// <returns>Maximum value</returns>
        /// <example>The following example lets you scroll through all rows in a grid
        /// <code lang="C#">
        /// GridControlBase grid; // your grid
        /// int min = grid.GetMinimumTopRowIndex();
        /// int max = grid.GetMaximumTopRowIndex();
        /// for (int v = min; v &lt;= max; v++)
        /// {
        ///     grid.TopRowIndex = v;
        /// }
        /// </code>
        /// <code lang="VB">
        /// Dim grid As GridControlBase ' your grid
        /// int min = grid.GetMinimumTopRowIndex();
        /// int max = grid.GetMaximumTopRowIndex();
        /// for (int v = min; v &lt;= max; v++)
        /// {
        ///     grid.TopRowIndex = v;
        /// }
        /// </code>
        /// </example>
        public int GetMaximumTopRowIndex()
        {
            return ScrollGrid.GetMaxRowScrollPosition();
        }

        /// <summary>
        /// Returns the minimum value for <see cref="TopRowIndex"/>.
        /// </summary>
        /// <returns>Minimum value</returns>
        /// <example>The following example lets you scroll through all rows in a grid
        /// <code lang="C#">
        /// GridControlBase grid; // your grid
        /// int min = grid.GetMinimumTopRowIndex();
        /// int max = grid.GetMaximumTopRowIndex();
        /// for (int v = min; v &lt;= max; v++)
        /// {
        ///     grid.TopRowIndex = v;
        /// }
        /// </code>
        /// <code lang="VB">
        /// Dim grid As GridControlBase ' your grid
        /// int min = grid.GetMinimumTopRowIndex();
        /// int max = grid.GetMaximumTopRowIndex();
        /// for (int v = min; v &lt;= max; v++)
        /// {
        ///     grid.TopRowIndex = v;
        /// }
        /// </code>
        /// </example>
        public int GetMinimumTopRowIndex()
        {
            return Math.Max(1, ScrollGrid.RowIndexToScrollPosition(this.InternalGetFrozenRows()) + 1);
        }

        /// <summary>
        /// Returns the maximum value for <see cref="LeftColIndex"/>.
        /// </summary>
        /// <returns>Maximum value</returns>
        /// <example>The following example lets you scroll through all columns in a grid
        /// <code lang="C#">
        /// GridControlBase grid; // your grid
        /// int min = grid.GetMinimumLeftColIndex();
        /// int max = grid.GetMaximumLeftColIndex();
        /// for (int v = min; v &lt;= max; v++)
        /// {
        ///     grid.LeftColIndex = v;
        /// }
        /// </code>
        /// <code lang="VB">
        /// Dim grid As GridControlBase ' your grid
        /// int min = grid.GetMinimumLeftColIndex();
        /// int max = grid.GetMaximumLeftColIndex();
        /// for (int v = min; v &lt;= max; v++)
        /// {
        ///     grid.LeftColIndex = v;
        /// }
        /// </code>
        /// </example>
        public int GetMaximumLeftColIndex()
        {
            return ScrollGrid.GetMaxRowScrollPosition();
        }

        /// <summary>
        /// Returns the minimum value for <see cref="LeftColIndex"/>.
        /// </summary>
        /// <returns>Minimum value</returns>
        /// <example>The following example lets you scroll through all columns in a grid
        /// <code lang="C#">
        /// GridControlBase grid; // your grid
        /// int min = grid.GetMinimumLeftColIndex();
        /// int max = grid.GetMaximumLeftColIndex();
        /// for (int v = min; v &lt;= max; v++)
        /// {
        ///     grid.LeftColIndex = v;
        /// }
        /// </code>
        /// <code lang="VB">
        /// Dim grid As GridControlBase ' your grid
        /// int min = grid.GetMinimumLeftColIndex();
        /// int max = grid.GetMaximumLeftColIndex();
        /// for (int v = min; v &lt;= max; v++)
        /// {
        ///     grid.LeftColIndex = v;
        /// }
        /// </code>
        /// </example>
        public int GetMinimumLeftColIndex()
        {
            return Math.Max(1, ScrollGrid.RowIndexToScrollPosition(this.InternalGetFrozenRows()) + 1);
        }

        /// <summary>
        /// Gets or sets the column index of the left column. Scrolls the grid when changed.
        /// </summary>
        [Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public int LeftColIndex
        {
            get
            {
                if (PrintingMode)
                {
                    return PrintInfo.m_nPrintLeftCol;
                }

                return ScrollGrid.m_nLeftCol;
            }

            set
            {
                SetLeftCol(value);
            }
        }

        /// <summary>
        /// Occurs before the grid is scrolled when the top row index is changed.
        /// </summary>
        [Category("Scrolling")]
        [Description("Occurs before the grid is scrolled when the top row index is changed.")]
        public event GridRowColIndexChangingEventHandler TopRowChanging;

        /// <summary>
        /// Occurs before the grid is scrolled when the left column index is changed.
        /// </summary>
        [Category("Scrolling")]
        [Description("Occurs before the grid is scrolled when the left column index is changed.")]
        public event GridRowColIndexChangingEventHandler LeftColChanging;

        /// <summary>
        /// Occurs after the grid has been scrolled when the top row index is changed.
        /// </summary>
        [Category("Scrolling")]
        [Description("Occurs after the grid has been scrolled when the top row index is changed.")]
        public event GridRowColIndexChangedEventHandler TopRowChanged;

        /// <summary>
        /// Occurs after the grid has been scrolled when the left column index is changed.
        /// </summary>
        [Category("Scrolling")]
        [Description("Occurs after the grid has been scrolled when the left column index is changed.")]
        public event GridRowColIndexChangedEventHandler LeftColChanged;

        int savedLeftColIndex = 0;
        int savedTopRowIndex = 0;

        /// <override/>
        protected override void OnWindowScrolled(ScrollWindowEventArgs e)
        {
            if (this.eventsTarget != null)
            {
                this.eventsTarget.OnWindowScrolled(e);
            }

            if (this.IsSplitterPaneClosing)
            {
                return;
            }

            ViewLayout.Reset();
#if DEBUG
            if (Switches.GridControlBaseEvents.TraceVerbose)
            {
                TraceUtil.TraceCurrentMethodInfo(PaneDesc, e);
            }
#else
            ;
#endif

            base.OnWindowScrolled(e);
        }

        internal bool NotifyTopRowChanging(int topRowIndex)
        {
            savedTopRowIndex = this.TopRowIndex;
            GridRowColIndexChangingEventArgs e = new GridRowColIndexChangingEventArgs(topRowIndex);
            OnTopRowChanging(e);
            if (!e.Cancel)
            {
                ////                NotifyChangingLayoutCells(GridRangeInfo.Rows(Model.Rows.FrozenCount+1, Model.RowCount));
                ////                NotifyBeforeUpdate();
                ViewLayout.Reset();
                return true;
            }

            return false;
        }

        /// <summary>
        /// Raises the <see cref="TopRowChanging"/> event.
        /// </summary>
        /// <param name="e">Event data.</param>
        protected virtual void OnTopRowChanging(GridRowColIndexChangingEventArgs e)
        {
            if (this.eventsTarget != null)
            {
                this.eventsTarget.OnTopRowChanging(e);
            }
#if DEBUG

            if (Switches.GridControlBaseEvents.TraceVerbose)
            {
                TraceUtil.TraceCurrentMethodInfo(PaneDesc, e);
            }
#else

            ;
#endif

            if (TopRowChanging != null)
            {
                try
                {
                    TopRowChanging(this, e);
                }
                catch (Exception ex)
                {
                    TraceUtil.TraceExceptionCatched(ex);
                    if (!ExceptionManager.RaiseExceptionCatched(this, ex))
                    {
                        throw;
                    }

                    e.Cancel = true;
                }
            }
        }

        internal void NotifyTopRowChanged(bool success)
        {
            ViewLayout.Reset();
            GridRowColIndexChangedEventArgs e = new GridRowColIndexChangedEventArgs(savedTopRowIndex, success);
            OnTopRowChanged(e);
            ////            NotifyAfterUpdate();
            ////            NotifyChangedLayoutCells();
        }

        /// <summary>
        /// Raises the <see cref="TopRowChanged"/> event.
        /// </summary>
        /// <param name="e">Event data.</param>
        protected virtual void OnTopRowChanged(GridRowColIndexChangedEventArgs e)
        {
            if (this.eventsTarget != null)
            {
                this.eventsTarget.OnTopRowChanged(e);
            }
#if DEBUG

            if (Switches.GridControlBaseEvents.TraceVerbose)
            {
                TraceUtil.TraceCurrentMethodInfo(PaneDesc, e);
            }
#else

            ;
#endif

            if (TopRowChanged != null)
            {
                try
                {
                    TopRowChanged(this, e);
                }
                catch (Exception ex)
                {
                    TraceUtil.TraceExceptionCatched(ex);
                    if (!ExceptionManager.RaiseExceptionCatched(this, ex))
                    {
                        throw;
                    }
                }
            }
        }

        internal bool NotifyLeftColChanging(int leftColIndex)
        {
            savedLeftColIndex = this.LeftColIndex;
            GridRowColIndexChangingEventArgs e = new GridRowColIndexChangingEventArgs(leftColIndex);
            OnLeftColChanging(e);
            if (!e.Cancel)
            {
                ////                NotifyChangingLayoutCells(GridRangeInfo.Cols(Model.Cols.FrozenCount+1, Model.ColCount));
                ////                NotifyBeforeUpdate();
                ViewLayout.Reset();
                return true;
            }

            return false;
        }

        /// <summary>
        /// Raises the <see cref="LeftColChanging"/> event.
        /// </summary>
        /// <param name="e">Event data.</param>
        protected virtual void OnLeftColChanging(GridRowColIndexChangingEventArgs e)
        {
            if (this.eventsTarget != null)
            {
                this.eventsTarget.OnLeftColChanging(e);
            }
#if DEBUG

            if (Switches.GridControlBaseEvents.TraceVerbose)
            {
                TraceUtil.TraceCurrentMethodInfo(PaneDesc, e);
            }
#else

            ;
#endif

            if (LeftColChanging != null)
            {
                try
                {
                    LeftColChanging(this, e);
                }
                catch (Exception ex)
                {
                    TraceUtil.TraceExceptionCatched(ex);
                    if (!ExceptionManager.RaiseExceptionCatched(this, ex))
                    {
                        throw;
                    }

                    e.Cancel = true;
                }
            }
        }

        internal void NotifyLeftColChanged(bool success)
        {
            ViewLayout.Reset();
            GridRowColIndexChangedEventArgs e = new GridRowColIndexChangedEventArgs(savedLeftColIndex, success);
            OnLeftColChanged(e);
            ////        NotifyAfterUpdate();
            ////            NotifyChangedLayoutCells();
        }

        /// <summary>
        /// Raises the <see cref="LeftColChanged"/> event.
        /// </summary>
        /// <param name="e">Event data.</param>
        protected virtual void OnLeftColChanged(GridRowColIndexChangedEventArgs e)
        {
            if (this.eventsTarget != null)
            {
                this.eventsTarget.OnLeftColChanged(e);
            }
#if DEBUG

            if (Switches.GridControlBaseEvents.TraceVerbose)
            {
                TraceUtil.TraceCurrentMethodInfo(PaneDesc, e);
            }
#else

            ;
#endif

            if (LeftColChanged != null)
            {
                try
                {
                    LeftColChanged(this, e);
                }
                catch (Exception ex)
                {
                    TraceUtil.TraceExceptionCatched(ex);
                    if (!ExceptionManager.RaiseExceptionCatched(this, ex))
                    {
                        throw;
                    }
                }
            }
        }

        /// <summary>
        /// Gets the current range of the grid excluding header rows and columns.
        /// </summary>
        [Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public GridRangeInfo GridCellsRange
        {
            get
            {
                return GridRangeInfo.Cells(InternalGetHeaderRows() + 1, InternalGetHeaderCols() + 1, Model.RowCount, Model.ColCount);
            }
        }

        /// <summary>
        /// Gets the current range of cells that are scrollable (all rows and columns excluding frozen rows and columns).
        /// </summary>
        [Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public GridRangeInfo ScrollableGridRangeInfo
        {
            get
            {
                return GridRangeInfo.Cells(GetFirstScrollableRow(), GetFirstScrollableCol(), Model.RowCount, Model.ColCount);
            }
        }

        #endregion
        #region MouseMsg

        /// <override/>
        /// <summary>Returns the pane information.</summary>
        [Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public override string PaneDesc
        {
            get
            {
                string _paneDesc = base.PaneDesc;

                if (this.isMouseDownCalled)
                {
                    _paneDesc += "Mod";
                }

                if (this.PrintingMode)
                {
                    _paneDesc += "Prn";
                }

                if (this.validatingFailed)
                {
                    _paneDesc += "Vfa";
                }

                _paneDesc += " " + this.CurrentCell.ToString();

                return _paneDesc;
            }
        }

        /// <override/>
        /// <summary>Gets or sets the multiplier for mouse wheel scrolling.</summary>
        [System.Xml.Serialization.XmlIgnore]
        [DesignerSerializationVisibilityAttribute(DesignerSerializationVisibility.Hidden)]
        [Browsable(false)]
        public override int VScrollIncrement
        {
            get
            {
                if (this.VScrollPixel)
                {
                    return this.Model.Rows.DefaultSize;
                }

                return base.VScrollIncrement;
            }

            set
            {
                base.VScrollIncrement = value;
            }
        }

        /// <override/>
        /// <summary>Gets or sets the multiplier for the mouse wheel scrolling.</summary>
        [System.Xml.Serialization.XmlIgnore]
        [DesignerSerializationVisibilityAttribute(DesignerSerializationVisibility.Hidden)]
        [Browsable(false)]
        public override int HScrollIncrement
        {
            get
            {
                if (this.HScrollPixel)
                {
                    return 20;
                }

                return base.HScrollIncrement;
            }

            set
            {
                base.HScrollIncrement = value;
            }
        }
        /// <summary>
        ///       Listens for the horizontal scrollbar's scroll event.
        /// </summary>
        /// <param name="sender">
        ///    A <see cref="System.Object"/> that contains data about the control.
        /// </param>
        /// <param name="se">
        ///    A <see cref="System.Windows.Forms.ScrollEventArgs"/> that contains the event data.
        /// </param>
        protected override void OnHScroll(object sender, ScrollEventArgs se)
        {
            if (!InMouseWheel)
                base.OnHScroll(sender, se);
        }

        bool InMouseWheel = false;

        /// <override/>
        protected override void OnMouseWheel(MouseEventArgs e)
        {
            InMouseWheel = true;
            if (this.eventsTarget != null)
            {
                this.eventsTarget.OnMouseWheel(e);
            }

            if (CurrentCell.Renderer != null)
            {
                GridCurrentCell gcc = this.CurrentCell.Renderer.GetNestedCurrentCell();
                if (gcc != null && gcc.Renderer != null)
                {
                    if (gcc.Renderer.ProcessMouseWheel(e))
                    {
                        return;
                    }
                }
            }
#if DEBUG
            if (Switches.GridControlBaseEvents.TraceVerbose)
            {
                TraceUtil.TraceCurrentMethodInfo(PaneDesc, e.X, e.Y, e.Button, e.Clicks);
            }
#else
            ;
#endif

            this.MouseControllerDispatcher.ProcessMouseMove(new MouseEventArgs(MouseButtons.None, 0, -1, -1, 0));
            base.OnMouseWheel(e);
            Point pt = Control.MousePosition;
            this.MouseControllerDispatcher.ProcessMouseMove(new MouseEventArgs(MouseButtons.None, 0, pt.X, pt.Y, 0));
            InMouseWheel = false;
        } // end of method OnMouseWheel

        /// <summary>
        /// Initiates call to <see cref="OnMouseWheel"/>.
        /// </summary>
        /// <param name="e">The event arguments.</param>
        public void RaiseMouseWheel(MouseEventArgs e)
        {
            OnMouseWheel(e);
        } //// end of method OnMouseWheel

        /// <summary>
        /// Gets a value that indicates if the grid is in design mode.
        /// </summary>
        /// <returns>Returns <see cref="Component.DesignMode"/></returns>
        public bool IsDesignMode()
        {
            return DesignMode;
        }

        /// <override/>
        /// <summary>Sets input focus to the control.</summary>
        /// <returns>True if the control is focused.</returns>
        public new virtual bool Focus()
        {
            ignoreNextValidating = true;
            try
            {
                return GetWindow().Focus();
            }
            finally
            {
                ignoreNextValidating = false;
            }
        }
        
        /// <override/>
        /// <summary>Gets a value indicating whether the control has input focus.</summary>
        [Browsable(false)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public new bool Focused
        {
            get
            {
                return GetWindow().Focused;
            }
        }

        /// <override/>
        protected override void OnMouseDown(MouseEventArgs e)
        {
            ignoreMouseUp = false;
            if (this.eventsTarget != null)
            {
                this.eventsTarget.OnMouseDown(e);
            }
#if DEBUG

            if (Switches.Development.TraceVerbose)
            {
                TraceUtil.TraceCurrentMethodInfo(PaneDesc, e.Button);
            }
#else

            ;
#endif

            if (this.DesignMode && e.Button != MouseButtons.Left)
            {
                base.OnMouseDown(e);
                return;
            }

            cancelMode = false;

            if (this.CanSelect && !this.QueryFocusInside() && FocusOnMouseDown && this.WantKeys)
            {
                Focus();
                if (cancelMode || (!this.DesignMode && !this.IsActiveControl))
                {
                    cancelMode = false;
                    return;
                }
            }

            isMouseDownCalled = true;
            forceCurrentCellMoveTo = true;

            ExceptionManager.SuspendCatchExceptions();

            try
            {
                if (RaiseCancelMouseEvent(e, new CancelMouseDelegate(OnGridControlMouseDown)))
                {
#if DEBUG
                    if (Switches.GridControlBaseEvents.TraceVerbose)
                    {
                        TraceUtil.TraceCurrentMethodInfo(PaneDesc, e.X, e.Y, e.Button, e.Clicks);
                    }
#else
                    ;
#endif

                    base.OnMouseDown(e);

                    if (this.IsDisposed)
                    { 
                        return; 
                    }

                    IGridFocusHelper gf = this.ActiveController as IGridFocusHelper;
                    if (gf == null || gf.GetAllowFixFocus())
                    {
                        FixDelayedCurrentCellActivate();
                        FixCurrentCellGotFocus();
                    }
                }

                ExceptionManager.ResumeCatchExceptions();
            }
            catch (Exception ex)
            {
                ExceptionManager.ResumeCatchExceptions();

                if (!ExceptionManager.RaiseExceptionCatched(this, ex))
                {
                    throw;
                }

                ignoreMouseUp = true;

                CurrentCell.ErrorMessage = ex.Message;
                CurrentCell.Exception = ex;
                CurrentCell.DisplayWarningText(ex.ToString());
                this.CancelUpdate();
                this.Refresh();
            }
        }

        bool ignoreMouseUp = false;

        /// <summary>
        /// Raises the <see cref="GridControlBase.GridControlMouseDown"/> event.
        /// </summary>
        /// <param name="e">A <see cref="CancelMouseEventArgs" /> that contains the event data.</param>
        protected virtual void OnGridControlMouseDown(CancelMouseEventArgs e)
        {
            if (this.eventsTarget != null)
            {
                this.eventsTarget.OnGridControlMouseDown(e);
            }
#if DEBUG

            if (Switches.GridControlBaseEvents.TraceVerbose)
            {
                TraceUtil.TraceCurrentMethodInfo(PaneDesc, e);
            }
#else

            ;
#endif

            if (GridControlMouseDown != null)
            {
                GridControlMouseDown(this, e);
            }
        }

        /// <summary>
        /// Raises the <see cref="GridControlBase.GridControlMouseMove"/> event.
        /// </summary>
        /// <param name="e">A <see cref="CancelMouseEventArgs" /> that contains the event data.</param>
        protected virtual void OnGridControlMouseMove(CancelMouseEventArgs e)
        {
            if (this.eventsTarget != null)
            {
                this.eventsTarget.OnGridControlMouseMove(e);
            }
#if DEBUG

            if (Switches.GridControlBaseEvents.TraceVerbose)
            {
                TraceUtil.TraceCurrentMethodInfo(PaneDesc, e);
            }
#else

            ;
#endif

            if (GridControlMouseMove != null)
            {
                GridControlMouseMove(this, e);
            }
        }

        /// <summary>
        /// Raises the <see cref="GridControlBase.GridControlMouseUp"/> event.
        /// </summary>
        /// <param name="e">A <see cref="CancelMouseEventArgs" /> that contains the event data.</param>
        protected virtual void OnGridControlMouseUp(CancelMouseEventArgs e)
        {
            if (this.eventsTarget != null)
            {
                this.eventsTarget.OnGridControlMouseUp(e);
            }
#if DEBUG

            if (Switches.GridControlBaseEvents.TraceVerbose)
            {
                TraceUtil.TraceCurrentMethodInfo(PaneDesc, e);
            }
#else

            ;
#endif

            if (GridControlMouseUp != null)            
            {  
                GridControlMouseUp(this, e);
            }
        }

        /// <override/>
        protected override/*Control*/ void OnMouseUp(MouseEventArgs e)
        {
            if (ignoreMouseUp)
            {
                return;
            }

            if (this.eventsTarget != null)
            {
                this.eventsTarget.OnMouseUp(e);
            }

            if (!RaiseCancelMouseEvent(e, new CancelMouseDelegate(OnGridControlMouseUp)))
            {
                return;
            }
#if DEBUG

            if (Switches.GridControlBaseEvents.TraceVerbose)
            {
                TraceUtil.TraceCurrentMethodInfo(PaneDesc, e.X, e.Y, e.Button, e.Clicks);
            }
#else

            ;
#endif
            base.OnMouseUp(e);

            if (this.IsDisposed)
            {
                return;
            }

            //// REVIEW: Added the following if statement to ensure that Draw is not called twice for
            //// a text box. -
            if (CurrentCell.Renderer != null && CurrentCell.Renderer.Control != null
                && CurrentCell.Renderer.Control.Focused)
            {
                return;
            }

            //// see comments for AllowFixFocusWhenCurrentCellIsEditingInMouseUp property below.
            if (this.IsHandleCreated && (AllowFixFocusWhenCurrentCellIsEditingInMouseUp || Focused) &&
                (CurrentCell.IsEditing ||
                (Model.Options.ActivateCurrentCellBehavior & GridCellActivateAction.SetCurrent) != 0))
            {
                this.FixCurrentCellGotFocus(true);
            }
        }

        bool allowFixFocusWhenCurrentCellIsEditingInMouseUp = true;

        /// <summary>
        /// Gets or sets a value indicating whether if you press the mouse button inside the grid and release the mouse the grid checks whether 
        /// the current cell is in editing mode. If that is the case it will make sure that focus is moved
        /// to the current cell control. This ensures the grid will properly return focus to the current
        /// cell if a message box is shown and closed in a custom event handler. <para/>
        /// If this behavior causes problems you should set this property false.
        /// One known issue we found is when you open another MDI form within a CellDoubleClick event, in which
        /// case the focus would be set back to the original form. If you run into such a situation set this
        /// property false.
        /// </summary>
        [Browsable(false), DefaultValue(true)]
        public bool AllowFixFocusWhenCurrentCellIsEditingInMouseUp
        {
            get
            {
                return allowFixFocusWhenCurrentCellIsEditingInMouseUp;
            }

            set
            {
                allowFixFocusWhenCurrentCellIsEditingInMouseUp = value;
            }
        }

        /// <override/>
        protected override/*Control*/ void OnMouseMove(MouseEventArgs e)
        {
            if (this.eventsTarget != null)
            {
                this.eventsTarget.OnMouseMove(e);
            }

            if (!RaiseCancelMouseEvent(e, new CancelMouseDelegate(OnGridControlMouseMove)))
            {
                return;
            }
#if DEBUG

            if (Switches.GridControlBaseEvents.TraceVerbose)
            {
                TraceUtil.TraceCurrentMethodInfo(PaneDesc, e.X, e.Y, e.Button, e.Clicks);
            }
#else

            ;
#endif
            base.OnMouseMove(e);
        }

        /// <summary>
        /// Occurs before a <see cref="Control.MouseDown"/> is raised and allows you to cancel the mouse event.
        /// </summary>
        [Description("Occurs before a MouseDown event is raised and allows you to cancel the mouse event."),
        Category("Behavior")]
        public event CancelMouseEventHandler GridControlMouseDown;

        /// <summary>
        /// Occurs before a <see cref="Control.MouseMove"/> is raised and allows you to cancel the mouse event.
        /// </summary>
        [Description("Occurs before a MouseMove is raised and allows you to cancel the mouse event."),
        Category("Behavior")]
        public event CancelMouseEventHandler GridControlMouseMove;

        /// <summary>
        /// Occurs before a <see cref="Control.MouseUp"/> is raised and allows you to cancel the mouse event.
        /// </summary>
        [Description("Occurs before a MouseUp is raised and allows you to cancel the mouse event."),
        Category("Behavior")]
        public event CancelMouseEventHandler GridControlMouseUp;
        
        /// <summary>
        /// Raises the specified mouse event and catches any exception. If exception is 4ed NotifyCancelMode
        /// is called. Returns False if event should be ignored by the grid.
        /// </summary>
        /// <param name="e">A <see cref="MouseEventArgs" /> that contains the event data.</param>
        /// <param name="d">A delegate that handles the event.</param>
        /// <returns>False if CancelMouseEventArgs.Cancel is True; False otherwise.</returns>
        internal bool RaiseCancelMouseEvent(MouseEventArgs e, CancelMouseDelegate d)
        {
            if (Location.X > 9999 || Location.X < -1000)
            {
                return false;
            }

            CancelMouseEventArgs cmea = new CancelMouseEventArgs(e);
            try
            {
                d(cmea);
            }
            catch (Exception ex)
            {
                TraceUtil.TraceExceptionCatched(ex);
                if (!ExceptionManager.RaiseExceptionCatched(this, ex))
                {
                    throw;
                }

                cmea.Cancel = true;
                NotifyCancelMode();
            }

            return !cmea.Cancel;
        }

        /// <summary>
        /// Delegate for OnBeforeMouseDown, OnBeforeMouseMove, and OnBeforeMouseUp methods.
        /// </summary>
        internal delegate void CancelMouseDelegate(CancelMouseEventArgs e);
        
        bool cancelMode = false;

        /// <override/>
        protected override void OnCancelMode(EventArgs e)
        {
#if DEBUG
            if (Switches.GridControlBaseEvents.TraceVerbose)
            {
                TraceUtil.TraceCurrentMethodInfo(PaneDesc, "C", Created, "U", Updating, "V", Visible, Size, PaneDesc);
            }
#else
            ;
#endif

            AutoScrolling = ScrollBars.None;
            Capture = false;
            cancelMode = true;

            base.OnCancelMode(e);
        }

        /// <override/>
        protected override void OnAutoScrollingChanged(EventArgs e)
        {
            if (Control.MouseButtons == MouseButtons.None)
            {
                AutoScrolling = ScrollBars.None;
            }
            else
            {
                base.OnAutoScrollingChanged(e);
            }
        }

        /// <override/>
        protected override void OnScrollControlMouseDown(CancelMouseEventArgs e)
        {
            if (this.eventsTarget != null)
            {
                this.eventsTarget.OnScrollControlMouseDown(e);
            }

#if DEBUG
            if (Switches.MouseEvents.TraceVerbose || Switches.GridControlBaseEvents.TraceVerbose)
            {
                TraceUtil.TraceCurrentMethodInfo(PaneDesc, e);
            }
#endif
            //// TODO: Other events that should set ActiveGridView?
            this.Model.ActiveGridView = this;
            base.OnScrollControlMouseDown(e);
        }
        
        #endregion
        void ModelCellsChanged(object sender, GridCellsChangedEventArgs e)
        {
#if DEBUG
            if (Switches.General.TraceVerbose)
            {
                TraceUtil.TraceCurrentMethodInfo(e);
            }
#else
            ;
#endif

            if (!e.Success)
            {
                return;
            }

            if (CurrentCell.IsActive && e.Range.Contains(CurrentCell.RangeInfo)
                && !CurrentCell.IsInActiveOrDeactivate && !CurrentCell.IsInConfirmChanges)
            {
                try
                {
                    CurrentCell.Refresh();
                }
                catch (Exception ex)
                {
                    TraceUtil.TraceExceptionCatched(ex);
                    if (!ExceptionManager.RaiseExceptionCatched(this, ex))
                    {
                        throw;
                    }
                }
            }

            InvalidateRange(e.Range, GridRangeOptions.MergeAllSpannedCells);
        }
        
        GridModel IGridModelSource.Model
        {
            get
            {
                return this.Model;
            }
        }

        /// <summary>
        /// Returns the default maximum size for the drop-down part of a StandardValuesCell.
        /// </summary>
        /// <returns>returns the size</returns>
        protected internal virtual Size GetDefaultMaxStandardValuesSize()
        {
            return Size.Empty;
        }
        
        //// Lazy instantiation cached graphics object
        private Graphics m_cachedGraphics = null;

        //// Helpers for recreating graphics when bounds or scroll location was changed.
        int tc_yPos, tc_xPos;
        Rectangle tc_bounds;
        
        /// <summary>
        /// Returns a <see cref="Graphics"/> object that is instantiated on demand and cached. When
        /// the grid size, grid location or scroll position changes the graphics object will be 
        /// recreated. Use GetCachedGraphics when you want to optimize drawing speed of the grid
        /// and call <see cref="DrawClippedGrid(System.Drawing.Graphics,System.Drawing.Rectangle)"/> to draw cells directly with this graphics object
        /// as shown in the TraderGridTest example.
        /// </summary>
        /// <returns>A Graphics object.</returns>
        public virtual Graphics GetCachedGraphics()
        {
            if (this.HasDoubleBufferSurface)
            {
                return DoubleBufferSurface.Graphics;
            }

            return GetCachedGraphics(this);
        }

        /// <summary>
        /// Return (or initialise) the cached graphics object for this table
        /// </summary>
        /// <param name="gridTableControl">The Grid table control</param>
        /// <returns>The catched graphics object.</returns>
        private Graphics GetCachedGraphics(GridControlBase gridTableControl)
        {
            if (m_cachedGraphics != null)
            {
                if (gridTableControl.VScrollBar.Value != tc_yPos
                    || gridTableControl.HScrollBar.Value != tc_xPos
                    || gridTableControl.Bounds != tc_bounds)
                {
                    m_cachedGraphics.Dispose();
                    gridTableControl.Disposed -= new EventHandler(GridControlBase_Disposed);
                    m_cachedGraphics = null;
                }
            }

            if (m_cachedGraphics == null)
            {
                m_cachedGraphics = gridTableControl.CreateGridGraphics();
                gridTableControl.Disposed += new EventHandler(GridControlBase_Disposed);
            }

            tc_yPos = gridTableControl.VScrollBar.Value;
            tc_xPos = gridTableControl.HScrollBar.Value;
            tc_bounds = gridTableControl.Bounds;

            return m_cachedGraphics;
        }

        /// <summary>
        /// Free resources associated with this grid table and unwire delegates
        /// </summary>
        /// <param name="sender">Current object.</param>
        /// <param name="e">Event args.</param>
        private void GridControlBase_Disposed(object sender, EventArgs e)
        {
            if (m_cachedGraphics != null)
            {
                m_cachedGraphics.Dispose();
                m_cachedGraphics = null;

                GridControlBase gridTableControl = (GridControlBase)sender;
                gridTableControl.Disposed -= new EventHandler(GridControlBase_Disposed);
            }
        }

        /// <summary>
        /// When you implement a custom cell type that supports hovering or other cell highlighting
        /// features you should call this method to notify the grid about the temporary state of the cell.
        /// </summary>
        /// <param name="rowIndex">The row index.</param>
        /// <param name="colIndex">The column index.</param>
        /// <param name="style">A reference to the style of the cell.</param>
        /// <remarks>
        /// By default the method has no functionality. With a GridGroupingControl however, the
        /// grid can save the Element associated with the rowindex and check the state again at 
        /// a later time when ListChanged notifications were handled and rows possibly shifted up
        /// or down.
        /// </remarks>
        public virtual void NotifyCellHighlighted(int rowIndex, int colIndex, GridStyleInfo style)
        {
        }
    }

    /// <internalonly/>
    [Syncfusion.Documentation.DocumentationExclude()]
    internal class GridUpdater
    {
        public static void UpdateCoveredCellsRange(GridControlBase pGrid, GridRangeInfo range, bool setOrReset)
        {
            GridRangeInfo rgBoundary = GridRangeInfo.Empty;
            ////                if (pGrid.Model.FloatingCells.EvaluateFloatingCells(range, ref rgBoundary))
            ////                    range = GridRangeInfo.UnionRange(range, rgBoundary);

            if (pGrid.ShouldPrepareUpdate(true))
            {
                if (setOrReset)
                {
                    int nEditRow, nEditCol;
                    //// if current cell is hidden by covered cells range,
                    //// move current cell to the covered cell
                    if (pGrid.CurrentCell.GetCurrentCell(out nEditRow, out nEditCol)
                        && range.Contains(GridRangeInfo.Cell(nEditRow, nEditCol)))
                    {
                        pGrid.CurrentCell.MoveTo(range.Top, range.Left, GridSetCurrentCellOptions.ScrollInView);
                    }
                }

                pGrid.InvalidateRange(range);
            }
        }
    }
}