#region Copyright Syncfusion Inc. 2001 - 2014
// Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Any infringement will be prosecuted under
// applicable laws. 
#endregion
#if !WinRT
using System.Windows;
using System.Windows.Input;
using Syncfusion.Windows.Controls.Cells;
using Syncfusion.Windows.GridCommon;
using System;
using Syncfusion.Windows.Controls.Scroll;

namespace Syncfusion.Windows.Controls.Grid
#else
using Syncfusion.WinRT.Controls.Cells;
using Syncfusion.WinRT.Controls.Grid;
using Syncfusion.WinRT.Controls.Scroll;
using System;
using Windows.Devices.Input;
using Windows.UI.Core;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Input;

namespace Syncfusion.WinRT.Controls.Grid
#endif
{
#if WinRT
    [ClassReference(IsReviewed = false)]
#endif
    public interface IGridCellRenderer : ICellRenderer, IDisposable
    {
        GridCellModelBase CellModel { get; }

        // Features
        bool IsControlTextShown { get; set; }
        bool IsFocusable { get; set; }
        bool IsEditable { get; set; }
        bool IsModifiable { get; set; }
        bool IsDropDownable { get; set; }

        // Created
        void RaiseCreated(GridCellModelBase cellModel);

        // Current Cell Workflow
        void RaiseActivateFailed();
        bool RaiseActivating(GridControlBase gridControlBase, RowColumnIndex rowColumnIndex, GridActivateCurrentCellOptions options);
        void RaiseActivated();
        void RaiseDeactivateFailed();
        bool RaiseDeactivating();
        void RaiseDeactivated();
        void RaiseInitialize(GridActivateCurrentCellOptions options);
        void CopyCurrentStyle();
        void RaiseRejectChanges();
        bool RaiseSaveChanges();
        bool RaiseValidate();
        void RaiseValidated();

        // Current Cell State
        GridActivateCurrentCellOptions ActivateOptions { get; }
        GridCurrentCell CurrentCell { get; }
        UIElement CurrentCellUIElement { get; }
        GridRenderStyleInfo CurrentStyle { get; }
        GridControlBase GridControl { get; set; }
        RowColumnIndex CellRowColumnIndex { get; }
        int RowIndex { get; }
        int ColumnIndex { get; }
        bool HasCurrentCellState { get; }

        bool IsFocused { get; set; }
        bool IsModified { get; }
        bool IsDroppedDown { get; set; }

        // Control Text
        string ControlText { get; set; }
        bool HasControlText { get; }
        void ResetControlText();
        bool SetControlText(string value);

        // Control Value
        object ControlValue { get; set; }
        bool HasControlValue { get; }
        void ResetControlValue();
        bool SetControlText(string value, bool enforceApplyControlText);
        //bool SetControlValue(object value);

        // Current Cell Methods
        void RefreshContent();

        bool IsCurrentCell(GridControlBase grid, RowColumnIndex pos);
        bool IsCurrentCell(GridRenderStyleInfo style);
        bool IsCurrentCell(UIElement uiElement);

        /// <summary>
        /// Let Renderer decide whether the parent grid should be allowed to handle keys and prevent
        /// the key event from being handled by the visual UIElement for this renderer. If this method
        /// returns true the parent grid will handle arrow keys and set the Handled flag in the event
        /// data. Keys that the grid does not handle will be ignored and be routed to the UIElement 
        /// for this renderer.
        /// </summary>
        /// <param name="e"></param>
        /// <returns></returns>
        /// 
#if !WinRT
        bool ShouldGridTryToHandlePreviewKeyDown(KeyEventArgs e);
#else
        bool ShouldGridTryToHandlePreviewKeyDown(KeyRoutedEventArgs e);
#endif


#if SyncfusionFramework4_0
        void RaiseGridPreviewTextInput(TextCompositionEventArgs e);
#endif

        void RaiseConfirmChangesFailed();

        bool RaiseStartEditing();

        void RaiseBeginEdit();

        void RaiseEndEdit();

        void RaiseEditingComplete();

        void UpdateCellRowColumnIndex(RowColumnIndex cellRowColumnIndex);
        void UpdateCurrentStyle();

        //void Invalidate();

        void RaiseGridPreviewMouseMove(RowColumnIndex rci, MouseEventArgs e);

        void RaiseGridCellClick(int rowIndex, int colIndex, MouseControllerEventArgs e);
    }

    //public interface ICellModelBound
    //{
    //    GridCellModelBase CellModel { get; set; }
    //}

}
