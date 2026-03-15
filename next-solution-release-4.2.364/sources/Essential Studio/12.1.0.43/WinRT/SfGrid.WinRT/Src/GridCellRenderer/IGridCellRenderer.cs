#region Copyright Syncfusion Inc. 2001 - 2014
// Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Any infringement will be prosecuted under
// applicable laws. 
#endregion
using Syncfusion.UI.Xaml.ScrollAxis;
using System;
#if WinRT
using Windows.Foundation;
using Windows.UI.Core;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Input;

#else
using System.Windows;
using System.Windows.Input;
#endif

namespace Syncfusion.UI.Xaml.Grid.Cells
{
#if WinRT
    using KeyEventArgs = KeyRoutedEventArgs;
#endif
    /// <summary>
    /// Defines the interface for all cell renderer.
    /// A default implementation of this interface is provided by the GridCellRendererBase class
    /// from which you should derive custom cell renderer classes. There is however no dependency on GridCellRendererBase.
    /// We should access the renderer by using this interface only. Do not create any objects for renderer.
    /// </summary>
    [ClassReference(IsReviewed = false)]
    public interface IGridCellRenderer : IDisposable
    {
        /// <summary>
        /// Gets or sets a value that indicates whether the cell is editable.
        /// </summary>
        bool IsEditable { get; set; }

        /// <summary>
        /// Gets or sets a value that indicates whether the cell is focusable.
        /// </summary>
        bool IsFocusible { get; set; }

        /// <summary>
        /// Gets or sets a value that indicates whether the cell can be dropped down.
        /// </summary>
        bool IsDropDownable { get; set; }

        /// <summary>
        /// The DataGrid
        /// </summary>
        SfDataGrid DataGrid { get; set; }

        /// <summary>
        /// Get whether the current cell was activated or not
        /// </summary>
        bool HasCurrentCellState { get; }

        /// <summary>
        /// Return the current cell value, only if the renderer's HasCurrentCellState is true
        /// </summary>
        /// <returns></returns>
        object GetControlValue();

        /// <summary>
        /// Set the current cell value, only if the renderer's HasCurrentCellState is true
        /// </summary>
        /// <param name="value"></param>
        void SetControlValue(object value);
    
        /// <summary>
        /// Let Renderer decide whether the parent grid should be allowed to handle keys and prevent
        /// the key event from being handled by the visual UIElement for this renderer. If this method
        /// returns true the parent grid will handle arrow keys and set the Handled flag in the event
        /// data. Keys that the grid does not handle will be ignored and be routed to the UIElement 
        /// for this renderer.
        /// </summary>
        /// <param name="e">A <see cref="KeyEventArgs"/> object.</param>
        /// <returns>True if the parent grid should be allowed to handle keys; false otherwise.</returns>
        bool ShouldGridTryToHandleKeyDown(KeyEventArgs e);

        /// <summary>
        /// Renderer will arrange the UIElement using this method
        /// </summary>
        /// <param name="cellRowColumnIndex">Row Column Index of the cell</param>
        /// <param name="uiElement">Corresponding UiElement</param>
        /// <param name="cellRect">Cell Rect for arranging the UIElement</param>
        /// <remarks></remarks>
        void Arrange(RowColumnIndex cellRowColumnIndex,UIElement uiElement, Rect cellRect);

        /// <summary>
        /// Method which is used to Measure the Cell UIElement
        /// </summary>
        /// <param name="cellRowColumnIndex">Row Column Index of the cell</param>
        /// <param name="uiElement">Corresponding UiElement</param>
        /// <param name="availableSize">Corresponding Size for measuring the  UIElement size</param>
        /// <remarks></remarks>
        void Measure(RowColumnIndex cellRowColumnIndex,UIElement uiElement, Size availableSize);

        /// <summary>
        /// Method which is used to create and initiate the UIElement for Cell
        /// </summary>
        /// <param name="cellRowColumnIndex">Row Column Index of the cell</param>
        /// <param name="record">Corresponding Record of the Row </param>
        /// <param name="column">Corresponding Grid Column</param>
        /// <param name="cellContainer">Corresponding Cell Element</param>
        /// <param name="isInEdit">Corresponding cell is in edit mode</param>
        /// <returns>Cell UIElement</returns>
        UIElement PrepareUIElements(RowColumnIndex cellRowColumnIndex,UIElement cellContainer,GridColumn column, object record,bool isInEdit);

        /// <summary>
        /// Method which is used for Unload the UIElement of the cell.(For Recyclling Purpose)
        /// </summary>
        /// <param name="cellRowColumnIndex">Cell Row Column Index</param>
        /// <param name="uiElemnt">Corresponding Cell UIElement</param>
        /// <remarks></remarks>
        void UnloadUIElements(RowColumnIndex cellRowColumnIndex, UIElement uiElemnt);

        /// <summary>
        /// Method which is used to Update the Binding Information of the Cell UIElement
        /// </summary>
        /// <param name="cellRowColumnIndex">Row Column Index of the cell</param>
        /// <param name="uiElement">Corresponding UiElement</param>
        /// <param name="column">Corresponding column for update the binding info</param>
        /// <remarks></remarks>
        void UpdateBindingInfo(RowColumnIndex cellRowColumnIndex, UIElement uiElemnt, GridColumn column, object record, bool isInEdit);

        /// <summary>
        /// Method which is used to update the style info of the Cell
        /// </summary>
        /// <param name="cellRowColumnIndex">Row Column Index of the cell</param>
        /// <param name="uiElement">Corresponding UiElement</param>
        /// <param name="column">Corresponding column for update the style info</param>
        /// <remarks></remarks>
        void UpdateCellStyle(RowColumnIndex cellRowColumnIndex, UIElement uiElement, GridColumn column);

#if !WinRT
        /// <summary>
        /// Called when text is entered in the Data Control
        /// </summary>
        /// <param name="args">KeyRoutedEventArgs</param>
        /// <remarks></remarks>
        void PreviewTextInput(TextCompositionEventArgs args);
#else
        void PreviewTextInput(KeyEventArgs args);
#endif

        /// <summary>
        /// Update the current cell information in renderer while current cell was activated.
        /// </summary>
        /// <param name="currentCellIndex">CurrentCell Index</param>
        /// <param name="currentCellElement">CurrentCell UIElement</param>
        void SetCurrentCellState(RowColumnIndex currentCellIndex, UIElement currentCellElement, bool isInEditing, bool isFocused);

        /// <summary>
        /// Reset the current cell information in renderer while current cell was deactivated.
        /// </summary>
        void ResetCurrentCellState();

        /// <summary>
        /// Sets Focus to the Editor UI Element, When it comes to view via Scrolling.
        /// </summary>
        void SetFocus(bool setFocus);

        /// <summary>
        /// Current cell enters into edit mode 
        /// </summary>
        /// <param name="cellRowColumnIndex">Row Column Index of the cell</param>
        /// <param name="currentCellElement">Corresponding Cell Element</param>
        /// <param name="column">Corresponding Grid Column</param>
        /// <param name="record">Corresponding Record of the Row </param>
        bool BeginEdit(RowColumnIndex cellRowColumnIndex, UIElement cellElement, GridColumn column, object record);

        /// <summary>
        /// current cell Leave the edit mode. 
        /// </summary>
        /// <param name="cellRowColumnIndex">Row Column Index of the cell</param>
        /// <param name="currentCellElement">Corresponding Cell Element</param>
        /// <param name="column">Corresponding Grid Column</param>
        /// <param name="record">Corresponding Record of the Row </param>
        bool EndEdit(RowColumnIndex cellRowColumnIndex, UIElement cellElement, GridColumn column, object record);

#if !WinRT

        /// <summary>
        /// Update the Source with the Edit UIElement.
        /// </summary>
        void UpdateSource(UIElement cellElement);

#endif

        void ClearRecycleBin();

    }
}
