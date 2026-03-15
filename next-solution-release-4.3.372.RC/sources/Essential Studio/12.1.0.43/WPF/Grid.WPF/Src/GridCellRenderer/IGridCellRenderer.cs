#region Copyright Syncfusion Inc. 2001 - 2014
// Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Any infringement will be prosecuted under
// applicable laws. 
#endregion
using System.Windows;
using System.Windows.Input;
using Syncfusion.Windows.Controls.Cells;
using Syncfusion.Windows.GridCommon;
using System;
using System.Windows.Automation.Peers;
using System.Windows.Media;

namespace Syncfusion.Windows.Controls.Grid
{
    /// <summary>
    /// Provides an interface for <see cref="GridCellRendererBase"/> for the cell renderers.
    /// </summary>
    public interface IGridCellRenderer : ICellRenderer, IDisposable
    {
        /// <summary>
        /// Gets the cell model.
        /// </summary>
        GridCellModelBase CellModel { get; }

        /// <summary>
        /// Gets or sets a value indicating whether [allow grid to focus].
        /// </summary>
        /// <value><c>true</c> if [allow grid to focus]; otherwise, <c>false</c>.</value>
        bool AllowGridToFocus { get; set; }

        // Features
        /// <summary>
        /// Gets or sets a value that indicates whether the ControlText is shown in the cell.
        /// </summary>
        bool IsControlTextShown { get; set; }

        /// <summary>
        /// Gets or sets a value that indicates whether the cell is focusable.
        /// </summary>
        bool IsFocusable { get; set; }

        /// <summary>
        /// Gets or sets a value that indicates whether the cell is editable.
        /// </summary>
        bool IsEditable { get; set; }

        /// <summary>
        /// Gets or sets a value that indicates whether the cell is modifiable.
        /// </summary>
        bool IsModifiable { get; set; }

        /// <summary>
        /// Gets or sets a value that indicates whether the cell can be dropped down.
        /// </summary>
        bool IsDropDownable { get; set; }

        /// <summary>
        /// Gets or sets whether the renderer supports rendering itsself directly to the drawing context.
        /// </summary>
        bool SupportsRenderOptimization{get ;set;}

        // Created
        /// <summary>
        /// Occurs when the cell model is created.
        /// </summary>
        /// <param name="cellModel">The cell model.</param>
        void RaiseCreated(GridCellModelBase cellModel);

        // Current Cell Workflow
        /// <summary>
        /// Occurs when activation of cell has been failed.
        /// </summary>
        void RaiseActivateFailed();
        
        /// <summary>
        /// Occurs when the cell is being activated.
        /// </summary>
        /// <param name="gridControlBase">The grid.</param>
        /// <param name="rowColumnIndex">Cell row column index.</param>
        /// <param name="options">Activation options for current cell.</param>
        /// <returns>True if the cell has been activated successfully; false otherwise.</returns>
        bool RaiseActivating(GridControlBase gridControlBase, RowColumnIndex rowColumnIndex, GridActivateCurrentCellOptions options);

        /// <summary>
        /// Occurs when the cell has been activated.
        /// </summary>
        void RaiseActivated();

        /// <summary>
        /// Occurs when the cell deactivation is failed.
        /// </summary>
        void RaiseDeactivateFailed();

        /// <summary>
        /// Occurs when the cell is being deactivated.
        /// </summary>
        /// <returns>True if the cell has been deactivated successfully; false otherwise.</returns>
        bool RaiseDeactivating();

        /// <summary>
        /// Occurs when the cell has been deactivated successfully.
        /// </summary>
        void RaiseDeactivated();

        /// <summary>
        /// Occurs when the cell is getting initialized.
        /// </summary>
        /// <param name="options">Activation options for the current cell.</param>
        void RaiseInitialize(GridActivateCurrentCellOptions options);

        /// <summary>
        /// Occurs when the changes that are done in the cell are rejected.
        /// </summary>
        void RaiseRejectChanges();

        /// <summary>
        /// Occurs when the changes that are done in the cell are saved.
        /// </summary>
        /// <returns>True if the changed content are saved successfully; false otherwise.</returns>
        bool RaiseSaveChanges();

        /// <summary>
        /// Occurs when the cell is validated.
        /// </summary>
        /// <returns>True if the cell has been validated successfully.</returns>
        bool RaiseValidate();

        /// <summary>
        /// Occurs when the cell has been validated.
        /// </summary>
        void RaiseValidated();

        /// <summary>
        /// Occurs when Clipboard Paste.
        /// </summary>
        void RaiseClipboardPaste(GridCutPasteEventArgs args);
        /// <summary>
        /// Occurs After Clipboard Paste.
        /// </summary>
        void RaiseClipboardPasted(GridCutPasteEventArgs args);

        // Current Cell State
        /// <summary>
        /// Provides various activation options for the cell.
        /// </summary>
        GridActivateCurrentCellOptions ActivateOptions { get; }

        /// <summary>
        /// Returns the current cell.
        /// </summary>
        GridCurrentCell CurrentCell { get; }

        /// <summary>
        /// Returns the UI element of current cell.
        /// </summary>
        UIElement CurrentCellUIElement { get; }

        /// <summary>
        /// Returns current cell style.
        /// </summary>
        GridRenderStyleInfo CurrentStyle { get; }

        /// <summary>
        /// Gets or sets the parent grid.
        /// </summary>
        GridControlBase GridControl { get; set; }

        /// <summary>
        /// Returns the cell's row column index.
        /// </summary>
        RowColumnIndex CellRowColumnIndex { get; }

        /// <summary>
        /// Returns the row index of the cell.
        /// </summary>
        int RowIndex { get; }

        /// <summary>
        /// Returns the column index of the cell.
        /// </summary>
        int ColumnIndex { get; }

        /// <summary>
        /// Specifies whether the cell state for the current cell has been set. 
        /// </summary>
        bool HasCurrentCellState { get; }

        /// <summary>
        /// Gets or sets a value that indicates whether the cell has the focus.
        /// </summary>
        bool IsFocused { get; set; }

        /// <summary>
        /// Specifies wheter the cell has been modified.
        /// </summary>
        bool IsModified { get; }

        /// <summary>
        /// Specifies whether the current cell is dropped down.
        /// </summary>
        bool IsDroppedDown { get; set; }

        // Control Text
        /// <summary>
        /// Gets or sets the active text that is displayed for the current cell, e.g. TextBox.Text.
        /// </summary>
        string ControlText { get; set; }

        /// <summary>
        /// Checks if ControlText for the current cell has been set.
        /// </summary>
        bool HasControlText { get; }

        /// <summary>
        /// Resets the ControlText to its original state.
        /// </summary>
        void ResetControlText();

        /// <summary>
        /// Sets the given value as control text for the cell.
        /// </summary>
        /// <param name="value">The value.</param>
        /// <returns>True if the control text has been set successfully; false otherwise.</returns>
        bool SetControlText(string value);

        // Control Value
        /// <summary>
        /// Gets or sets the cell value for the current cell.
        /// </summary>
        object ControlValue { get; set; }

        /// <summary>
        /// Returns whether the cell value for the current cell has been changed.
        /// </summary>
        bool HasControlValue { get; }

        /// <summary>
        /// Resets the cell value of the current cell to its original state.
        /// </summary>
        void ResetControlValue();

        /// <summary>
        /// Sets the given value as control text for the cell.
        /// </summary>
        /// <param name="value">The value.</param>
        /// <param name="enforceApplyControlText">When true, enforces applying control text.</param>
        /// <returns>True if the control text has been set successfully; false otherwise.</returns>
        bool SetControlText(string value, bool enforceApplyControlText);
        //bool SetControlValue(object value);

        // Current Cell Methods
        /// <summary>
        /// Refreshes the current cell.
        /// </summary>
        void RefreshContent();

        /// <summary>
        /// Determines whether the given cell coordinates represent the current cell.
        /// </summary>
        /// <param name="gridControl">The grid.</param>
        /// <param name="cellRowColumnIndex">The cell row column index.</param>
        /// <returns>True if it is current cell; false otherwise.</returns>
        bool IsCurrentCell(GridControlBase grid, RowColumnIndex pos);

        /// <summary>
        /// Determines whether the given cell style represents the current cell.
        /// </summary>
        /// <param name="style">Cell style information.</param>
        /// <returns>True if it is current cell; false otherwise.</returns>
        bool IsCurrentCell(GridRenderStyleInfo style);

        /// <summary>
        /// Determines whether the given cell element corresponds to the current cell.
        /// </summary>
        /// <param name="uiElement">The cell control.</param>
        /// <returns>True if it is current cell; false otherwise.</returns>
        bool IsCurrentCell(UIElement uiElement);

        /// <summary>
        /// Let Renderer decide whether the parent grid should be allowed to handle keys and prevent
        /// the key event from being handled by the visual UIElement for this renderer. If this method
        /// returns true the parent grid will handle arrow keys and set the Handled flag in the event
        /// data. Keys that the grid does not handle will be ignored and be routed to the UIElement 
        /// for this renderer.
        /// </summary>
        /// <param name="e">A <see cref="KeyEventArgs"/> object.</param>
        /// <returns>True if the parent grid should be allowed to handle keys; false otherwise.</returns>
        bool ShouldGridTryToHandlePreviewKeyDown(KeyEventArgs e);

        /// <summary>
        /// Occurs before the cell shifts into editing mode.
        /// </summary>
        /// <param name="e">A <see cref="TextCompositionEventArgs"/> object.</param>
        void RaiseGridPreviewTextInput(TextCompositionEventArgs e);

        /// <summary>
        /// Raises <see cref="CurrentCellConfirmChangesFailed"/> event.
        /// </summary>
        void RaiseConfirmChangesFailed();

        /// <summary>
        /// Occurs when the cell gets started editing.
        /// </summary>
        /// <returns>True</returns>
        bool RaiseStartEditing();

        /// <summary>
        /// Occurs when the cell switches to editing mode.
        /// </summary>
        void RaiseBeginEdit();

        /// <summary>
        /// Occurs when the editing mode ends for the cell.
        /// </summary>
        void RaiseEndEdit();

        /// <summary>
        /// Occurs once the grid completes the editing mode for the cell.
        /// </summary>
        void RaiseEditingComplete();

        /// <summary>
        /// Updates the cell row column index with the given value
        /// </summary>
        /// <param name="cellRowColumnIndex">The cell row column index.</param>
        void UpdateCellRowColumnIndex(RowColumnIndex cellRowColumnIndex);

        void UpdateCurrentStyle();

        //void Invalidate();

        /// <summary>
        /// Raises PreviewMouseMove event.
        /// </summary>
        /// <param name="rci">The cell row column index.</param>
        /// <param name="e">The <see cref="System.Windows.Input.MouseEventArgs"/> that contains the event data.</param>
        void RaiseGridPreviewMouseMove(RowColumnIndex rci, MouseEventArgs e);

        /// <summary>
        /// Raises CellClick event.
        /// </summary>
        /// <param name="rowIndex">Row index.</param>
        /// <param name="colIndex">Column index.</param>
        /// <param name="e">The <see cref="System.Windows.Input.MouseControllerEventArgs"/> that contains the event data.</param>
        void RaiseGridCellClick(int rowIndex, int colIndex, Syncfusion.Windows.Controls.Scroll.MouseControllerEventArgs e);

        /// <summary>
        /// Empties the recyclebin.
        /// </summary>
        void EmptyRecycleBin();

        AutomationPeer OnCreateAutomationPeer();

#if !SILVERLIGHT
        /// <summary>
        /// force to render graphics while printing that do not belong to live controls and the cells which is not view (e.g. static text).
        /// <see cref="GridCellRendererBase{S}"/> implements this method and calls the 
        /// virtual <see cref="GridCellRendererBase{S}.RenderForPrinting"/> method.
        /// </summary>
        /// <param name="dc">The drawing context.</param>
        /// <param name="rca">The render cell layout information.</param>
        /// <param name="style">The cell style info.</param>
        void RenderForPrinting(DrawingContext dc, RenderCellArgs rca,GridRenderStyleInfo style);
#endif
    }

    //public interface ICellModelBound
    //{
    //    GridCellModelBase CellModel { get; set; }
    //}

}
