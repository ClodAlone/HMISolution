#region Copyright Syncfusion Inc. 2001 - 2014
// Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Any infringement will be prosecuted under
// applicable laws. 
#endregion
using System;
using System.Globalization;
using System.Windows;
using System.Windows.Input;


#if !WinRT
using Syncfusion.Windows.Controls.Cells;
using Syncfusion.Windows.Controls.Scroll;
using Syncfusion.Windows.GridCommon;
using Syncfusion.Windows.Styles;
using System.Windows.Controls;
namespace Syncfusion.Windows.Controls.Grid
#else

using Syncfusion.WinRT.Controls.Cells;
using Syncfusion.WinRT.Controls.Scroll;
using Syncfusion.WinRT.GridCommon;
using Syncfusion.WinRT.Styles;
using Windows.UI.Xaml;
using Windows.UI.Core;
using Windows.Devices.Input;
using Windows.UI.Xaml.Controls;

namespace Syncfusion.WinRT.Controls.Grid
#endif
{
#if WinRT
    [ClassReference(IsReviewed = false)]
#endif
    public class GridCellRendererBase : CellRendererBase<GridRenderStyleInfo>, IGridCellRenderer, IHitTestSelectCells, IStyleChanged
    {
        #region Fields
        GridCellModelBase cellModel;
        WeakReference weakReference;

        bool isModifiable;
        bool isDropDownable;
        bool isFocusable;
        bool isControlTextShown;

        bool inSetControlText;
        bool inSetControlValue;
        bool inInitialize;

        // State for current cell only. Will be set in RaiseActivating and reset in RaiseDeactivating
        GridControlBase gridControl;
        RowColumnIndex cellRowColumnIndex;
        GridActivateCurrentCellOptions activateOptions;
        bool hasCurrentCellState;
        UIElement currentCellUIElement;
        GridRenderStyleInfo currentStyle;
        GridRenderStyleInfo currentStyleCopy;

        string controlText = "";
        bool hasControlText = false;
        object controlValue = null;
        bool hasControlValue = false;

        #endregion

        static GridCellRendererBase()
        {
        }

        public GridCellRendererBase()
        {
            this.weakReference = new WeakReference(this);
            this.UseDefaultRenderer = true;
        }

        #region Features

#if (SILVERLIGHT || WinRT)
        /// <summary>
        /// Gets or Sets the default renderer initialization. If true then the default renderer routine would be called in OnInitializeRendererElement.
        /// </summary>
        public bool UseDefaultRenderer
        {
            get;
            set;
        }
#endif

        public bool IsControlTextShown
        {
            get { return isControlTextShown; }
            set { isControlTextShown = value; }
        }

        public bool IsFocusable
        {
            get { return isFocusable; }
            set { isFocusable = value; }
        }

        public bool IsModifiable
        {
            get { return isModifiable; }
            set { isModifiable = value; }
        }

        public bool IsDropDownable
        {
            get { return isDropDownable; }
            set { isDropDownable = value; }
        }

        Type editorType;

        public Type EditorType
        {
            get { return editorType; }
            set { editorType = value; }
        }
        #endregion

        #region Created

        public void RaiseCreated(GridCellModelBase cellModel)
        {
            this.cellModel = cellModel;
            OnCreated();
        }

        protected virtual void OnCreated()
        {
        }

        #endregion

        #region CellModel
        public GridCellModelBase CellModel
        {
            get { return cellModel; }
        }
        #endregion

        public GridControlBase GridControl
        {
            get
            {
                return gridControl;
            }
            set { gridControl = value; }
        }

        #region CurrentCell State - GridControl, CellRowColumnIndex, ActivateOptions, CurrentCell, CurrentStyle, CurrentCellUIElement
        public RowColumnIndex CellRowColumnIndex
        {
            get
            {
                if (!HasCurrentCellState)
                    throw new InvalidOperationException("CellRowColumnIndex is only accessible when renderer is current cell. Check GridRenderStyleInfo.CellRowColumnIndex instead.");
                return cellRowColumnIndex;
            }
        }

        public GridActivateCurrentCellOptions ActivateOptions
        {
            get
            {
                if (!HasCurrentCellState)
                    throw new InvalidOperationException("ActivateOptions is only accesible when renderer is current cell.");
                return activateOptions;
            }
        }

        public int RowIndex
        {
            get
            {
                return CellRowColumnIndex.RowIndex;
            }
        }

        public int ColumnIndex
        {
            get
            {
                return CellRowColumnIndex.ColumnIndex;
            }
        }

        public GridCurrentCell CurrentCell
        {
            get
            {
                return GridControl.CurrentCell;
            }
        }

        public GridRenderStyleInfo CurrentStyle
        {
            get
            {
                if (!HasCurrentCellState)
                    throw new InvalidOperationException("CurrentStyle is only accesible when renderer is current cell.");
                //if (invalidateDirty)
                //{
                //    this.currentStyle = gridControl.GetRenderStyleInfo(cellRowColumnIndex);
                //    this.currentStyleCopy = currentStyle.Copy();
                //    invalidateDirty = false;
                //}
                return currentStyle;
            }
        }

        public GridRenderStyleInfo CurrentStyleCopy
        {
            get
            {
                if (!HasCurrentCellState)
                    throw new InvalidOperationException("CurrentStyle is only accesible when renderer is current cell.");
                return currentStyleCopy;
            }
        }

        public UIElement CurrentCellUIElement
        {
            get
            {
                if (!HasCurrentCellState)
                    throw new InvalidOperationException("FocusedElement is only accesible when renderer is current cell.");
                return currentCellUIElement;
            }
            protected set
            {
#warning TODO - Check EditorType for swapped uielement
                /*if (EditorType != null)
                {
                    if (value != null && !value.GetType().IsAssignableFrom(EditorType))
                        throw new InvalidCastException("CurrentCellUIElement should be of type " + EditorType.Name);
                }*/
                currentCellUIElement = value;
            }
        }

        public bool HasCurrentCellState
        {
            get { return hasCurrentCellState; }
        }

        protected void SetCurrentCellState(GridControlBase gridControl, RowColumnIndex cellRowColumnIndex, GridActivateCurrentCellOptions options)
        {
            //this.gridControl = gridControl;
            this.cellRowColumnIndex = cellRowColumnIndex;
            this.hasCurrentCellState = true;
            this.activateOptions = options;
            this.CurrentCellUIElement = options.Element;
            this.currentStyle = gridControl.GetRenderStyleInfo(cellRowColumnIndex);
            this.currentStyle.WeakReferenceChangedListeners.Add(weakReference);
            this.CopyCurrentStyle();
        }

        public bool IsCurrentCell(GridControlBase gridControl, RowColumnIndex cellRowColumnIndex)
        {
            return hasCurrentCellState
                && gridControl.CurrentCell.HasCurrentCell
                && this.gridControl == gridControl
                && this.cellRowColumnIndex == cellRowColumnIndex;
        }

        public bool IsCurrentCell(GridRenderStyleInfo style)
        {
            return style != null && IsCurrentCell(style.GridControl, style.CellRowColumnIndex);
        }

        public bool IsCurrentCell(UIElement uiElement)
        {
            GridControlBase grid = VirtualizingCellsControl.GetCellsControl(uiElement) as GridControlBase;
            RowColumnIndex cellRowColumnIndex = VirtualizingCellsControl.GetCellRowColumnIndex(uiElement);
            return IsCurrentCell(grid, cellRowColumnIndex);
        }

        protected void ResetCurrentCellState()
        {
            if (this.currentStyle != null)
                this.currentStyle.WeakReferenceChangedListeners.Remove(weakReference);
            this.hasCurrentCellState = false;
            this.hasControlText = false;
            this.hasControlValue = false;
            this.currentStyleCopy = null;
        }
        #endregion

        #region CurrentCell State - IsFocused, IsModified, IsDroppedDown
        private bool isFocused = false;
        public bool IsFocused
        {
            get
            {
                //return CurrentCellUIElement != null;
                return this.isFocused;
            }
            set
            {
                if (IsFocusable && value != IsFocused && CurrentCellUIElement != null)
                {
                    if (value)
                    {
                        if (!CurrentCell.IsEditing && !CurrentCell.IsInBeginEdit)
                            CurrentCell.BeginEdit(true);
                        else
                        {
#if (!SILVERLIGHT && !WinRT)
                            if (!CurrentCellUIElement.IsKeyboardFocusWithin && !(CurrentCell.IsInActivate && ActivateOptions.IsActivateTriggeredByGotFocus))
#else
                            if (!(CurrentCell.IsInActivate && ActivateOptions.IsActivateTriggeredByGotFocus))
#endif
                            {
                                var control = this.CurrentCellUIElement as Control;
                                if (control != null)
                                {
#if !WinRT
                                    control.Focus();
#else
                                    control.Focus(FocusState.Programmatic);
#endif 
                                }
                            }
                            OnSetFocus();
                        }
                    }
                    else
                    {
#if !WinRT
                        gridControl.Focus();
#else
                        gridControl.Focus(FocusState.Pointer);
#endif 
                        OnResetFocus();
                    }
                    this.isFocused = value;
                }
            }
        }

        protected virtual void OnResetFocus()
        {
        }

        protected virtual void OnSetFocus()
        {
        }

        public bool IsModified
        {
            get
            {
                return GridControl.CurrentCell.IsModified;
            }
        }


        public bool IsDroppedDown
        {
            get
            {
                return GridControl.CurrentCell.IsDroppedDown;
            }
            set
            {
                //GridControl.CurrentCell.IsDroppedDown = value;
            }
        }
        #endregion

        #region CurrentCell - ControlText

        /// <summary>
        /// The active text that is displayed for the current cell, e.g. TextBox.Text.
        /// </summary>
        public virtual string ControlText
        {
            get
            {
                if (!HasCurrentCellState)
                    throw new InvalidOperationException("ControlText is only accesible when renderer is current cell.");

                if (!IsControlTextShown)
                    return CurrentStyle.GetText(ControlValue);

                if (!this.HasControlText)
                {
                    if (this.HasControlValue)
                    {
                        controlText = GetControlTextCore(CurrentStyle, ControlValue);
                        hasControlText = true;
                    }
                    else if (CurrentCellUIElement == null)
                    {
                        controlText = GetControlTextCore(CurrentStyle, CurrentStyle.CellValue);
                        hasControlText = true;
                    }
                    else
                    {
                        return GetControlTextFromEditor();
                    }
                }

                return controlText;
            }
            set
            {
                if (!SetControlText(value, true))
                    throw new Exception("ControlText setter failed with value " + value);
            }
        }

        public bool SetControlText(string value)
        {
            return SetControlText(value, false);
        }

        public bool SetControlText(string value, bool enforceApplyControlText)
        {
            if (!HasCurrentCellState)
                throw new InvalidOperationException("ControlText is only accesible when renderer is current cell.");

            if (!IsControlTextShown)
                throw new InvalidOperationException("IsControlTextShown has not been set for this renderer.");

            if (inSetControlText)
                return true;

            string editorText = null;
            if (CurrentCellUIElement != null)
                editorText = GetControlTextFromEditor();
            else
                GridControl.InvalidateCell(CellRowColumnIndex);

            if (!HasControlValue &&
                (HasControlText && value == ControlText
                || !HasControlText && value == GetControlTextCore(CurrentStyle, CurrentStyle.CellValue)))
            {
                controlText = value;
                hasControlText = true;
                return true;
            }

            if (!InInitialize)
            {
                if (!ValidateControlText(value))
                    return false;

                if (!NotifyCurrentCellChanging())
                    return false;
            }

            inSetControlText = true;
            try
            {
                if (ApplyControlText(CurrentStyleCopy, value))
                {
                    ResetControlValue();

                    hasControlText = true;
                    controlText = value;

                    // Reinitialize editor with new text (taken from ControlText)
                    if (editorText != null && editorText != controlText)
                        RefreshContent();
                }
                else if (enforceApplyControlText)
                    return false;
            }
            finally
            {
                inSetControlText = false;
            }

            if (!InInitialize)
                NotifyCurrentCellChanged();

            return true;
        }

        public virtual bool ValidateControlText(string value)
        {
            return true;
        }

        /// <summary>
        /// Checks if ControlText for the current cell has been set.
        /// </summary>
        public bool HasControlText
        {
            get
            {
                return hasControlText;
            }
        }

        /// <summary>
        /// Resets the ControlText to its original state.
        /// </summary>
        public void ResetControlText()
        {
            hasControlText = false;
            controlText = "";
        }

        protected virtual bool ApplyControlText(GridRenderStyleInfo style, string controlText)
        {
            try
            {
                style.ApplyFormattedText(controlText, GridCellBaseTextInfo.CurrentText);
            }
            catch (Exception ex)
            {
                style.CellValue = null;
                style.Exception = ex;
                return false;
            }
            return true;
        }

        public string GetControlText(GridRenderStyleInfo style)
        {
            return GetControlTextCore(style, style.CellValue);
        }

        protected virtual string GetControlTextCore(GridRenderStyleInfo style, object cellValue)
        {
            try
            {
                if (CurrentCell.HasCurrentCellAt(style.CellRowColumnIndex) && ShouldCurrentCellShowCellText())
                    return style.GetText(cellValue);
                else
                    return style.GetFormattedText(cellValue, GridCellBaseTextInfo.CurrentText);
            }
            catch (Exception ex)
            {
                return ex.Message;
            }
        }

        public virtual bool ShouldCurrentCellShowCellText()
        {
            return CurrentCell.IsEditing;
        }

        public virtual string GetControlTextFromEditor()
        {
            if (CurrentCellUIElement != null)
                return CurrentCellUIElement.ToString();
            else
            {
                try
                {
                    return GetControlTextCore(CurrentStyle, CurrentStyle.CellValue);
                }
                catch (Exception ex)
                {
                    return ex.Message;
                }
            }
        }

        /// <summary>
        /// Gets a value indicating whether ControlText setter is in process.
        /// </summary>
        /// <value><c>true</c> if ControlText setter is in process; otherwise, <c>false</c>.</value>
        public bool InSetControlText
        {
            get
            {
                return inSetControlText;
            }
        }

        #endregion

        #region CurrentCell - ControlValue

        /// <summary>
        /// The cell value for the current cell.
        /// </summary>
        public virtual object ControlValue
        {
            get
            {
                if (!HasCurrentCellState)
                    throw new InvalidOperationException("ControlValue is only accesible when renderer is current cell.");

                if (!this.HasControlValue)
                {
                    if (this.HasControlText)
                    {
                        if (ApplyControlText(CurrentStyleCopy, ControlText))
                            controlValue = CurrentStyleCopy.CellValue;
                        else
                            controlValue = null;
                        hasControlValue = true;
                    }
                    else if (CurrentCellUIElement == null)
                    {
                        controlValue = CurrentStyle.CellValue;
                        controlValue = ChangeType(controlValue, CurrentStyle.CellValueType, CurrentStyle.GetCulture(true), true);
                    }
                    else
                    {
                        controlValue = GetControlValueFromEditor();
                        controlValue = ChangeType(controlValue, CurrentStyle.CellValueType, CurrentStyle.GetCulture(true), true);
                    }
                }

                return controlValue;
            }
            set
            {
                if (!SetControlValue(value))
                    throw new Exception("ControlValue setter failed with value " + value);
            }
        }

        public bool SetControlValue(object value)
        {

            if (!HasCurrentCellState)
                throw new InvalidOperationException("ControlValue is only accesible when renderer is current cell.");

            if (inSetControlValue)
                return false;

            if (InInitialize)
            {
                controlValue = value;
                hasControlValue = true;
                return true;
            }

            if (!HasControlValue && !HasControlText && ObjectEquals(value, CurrentStyle.CellValue))
                return true;

            object editorValue = GetControlValueFromEditor();

            if (!ValidateControlValue(value))
                return false;

            if (!NotifyCurrentCellChanging())
                return false;

            inSetControlValue = true;
            try
            {
                try
                {
                    object adjustedValue = ChangeType(value, CurrentStyle.CellValueType, CurrentStyle.GetCulture(true), false);

                    ResetControlText();

                    controlValue = adjustedValue;
                    hasControlValue = true;

                    // Reinitialize editor with new value 
                    if (!ObjectEquals(editorValue, value))
                        RefreshContent();
                }
                catch (Exception ex)
                {
                    //CurrentStyle.CellValue = oldValue;
                    CurrentStyle.Exception = ex;
                }
            }
            finally
            {
                inSetControlValue = false;
            }

            NotifyCurrentCellChanged();

            return true;
        }

        public virtual bool ValidateControlValue(object value)
        {
            return true;
        }

        private bool ObjectEquals(object value1, object value2)
        {
            return Object.ReferenceEquals(value1, value2)
                || value1 != null && value1.Equals(value2);
        }

        private object ChangeType(object value, Type type, CultureInfo ci, bool returnDbNullIfNotValid)
        {
#if !WinRT
            if (type == null || value == null || value is DBNull)
            {
            }
            else if (value.GetType() != type)
            {
                value = ValueConvert.ChangeType(value, type, ci, returnDbNullIfNotValid);
                if (value is DBNull)
                    value = null;
            }
#else
            if (type == null || value == null)
            {
            }
            else if (value.GetType() != type)
            {
                value = ValueConvert.ChangeType(value, type, ci, returnDbNullIfNotValid);
            }
#endif
            return value;
        }

        /// <summary>
        /// Returns whether the cell value for the current cell has been changed.
        /// </summary>
        public bool HasControlValue
        {
            get
            {
                return hasControlValue;
            }
        }

        /// <summary>
        /// Resets the cell value of the current cell to its original state.
        /// </summary>
        public void ResetControlValue()
        {
            hasControlValue = false;
            controlValue = null;
            //if (!IsModified)
            //    CurrentStyle.CellValue = CurrentStyle.ModelStyle.CellValue;
        }

        protected virtual object GetControlValueFromEditor()
        {
            return CurrentStyle.CellValue;
        }

        public object GetControlValue(GridRenderStyleInfo style)
        {
            if (IsCurrentCell(style))
                return ControlValue;
            else
                return style.CellValue;
        }

        //string GetValueTextCore(GridRenderStyleInfo style, object cellValue)
        //{
        //    try
        //    {
        //        return style.GetText(cellValue);
        //    }
        //    catch (Exception ex)
        //    {
        //        return ex.Message;
        //    }
        //}



        /// <summary>
        /// Returns true if ControlValue setter was called.
        /// </summary>
        public bool InSetControlValue
        {
            get
            {
                return inSetControlValue;
            }
        }

        #endregion

        #region CurrentCell - Workflow

        public void RaiseInitialize(GridActivateCurrentCellOptions options)
        {
            inInitialize = true;
            //CellUIElements cellUI = this.GridControl.GetCellUIElements(CellRowColumnIndex);
            //cellUI = GridControl.GetCellUIElements(CellRowColumnIndex);
            //if (cellUI != null && cellUI.UIElements.Count > 0 && !cellUI.IsDirty)
            //{
            //    activateOptions.Element = cellUI.UIElements[0];
                //if (!(activateOptions.Element is ComboBox))
                //    this.RefreshContent();
           // }
            currentStyleCopy = CurrentStyle.Copy();

            OnInitialize();
            inInitialize = false;
        }

        /// <summary>
        /// Method for copying the CurrentStyle
        /// </summary>
        public void CopyCurrentStyle()
        {
            currentStyleCopy = CurrentStyle.Copy();
        }

        protected bool InInitialize
        {
            get
            {
                return inInitialize;
            }
        }

        protected virtual void OnInitialize()
        {
            ScrollInView();

            if (activateOptions.Element == null)
            {
                CellUIElements visuals = GridControl.GetCellUIElements(cellRowColumnIndex);
                if (visuals != null && visuals.UIElements.Count > 0)
                {
                    CurrentCellUIElement = activateOptions.Element = visuals.UIElements[0];
                }
            }

        }

        protected virtual void ScrollInView()
        {
            GridControl.ScrollInView(cellRowColumnIndex);
        }

        public bool RaiseActivating(GridControlBase gridControl, RowColumnIndex cellRowColumnIndex, GridActivateCurrentCellOptions options)
        {
            SetCurrentCellState(gridControl, cellRowColumnIndex, options);
            CellModel.ActiveRenderer = this;
            return OnActivating();
        }

        protected virtual bool OnActivating()
        {
            return true;
        }

        public void RaiseActivated()
        {
            GridControl.Model.CurrentCellState = new GridModelCurrentCellState(GridControl, CellRowColumnIndex);
            OnActivated();
        }

        protected virtual void OnActivated()
        {
           // gridControl.InvalidateCell(cellRowColumnIndex);
        }

        public void RaiseActivateFailed()
        {
            OnActivateFailed();
            CellModel.ActiveRenderer = null;
        }

        protected virtual void OnActivateFailed()
        {
        }

        public bool RaiseDeactivating()
        {
            return OnDeactivating();
        }

        protected virtual bool OnDeactivating()
        {
            return true;
        }

        public void RaiseDeactivated()
        {
            GridControlBase gridControl = GridControl;
            RowColumnIndex cellRowColumnIndex = CellRowColumnIndex;

            OnDeactivated();
            gridControl.Model.CurrentCellState = GridModelCurrentCellState.Empty;

            ResetCurrentCellState(); // Resets GridControl to null.
            CellModel.ActiveRenderer = null;

            //gridControl.InvalidateVisual(false);
        }

        protected virtual void OnDeactivated()
        {
           // gridControl.InvalidateCell(cellRowColumnIndex);
        }

        public void RaiseDeactivateFailed()
        {
            OnDeactivateFailed();
        }

        protected virtual void OnDeactivateFailed()
        {
        }

        public bool RaiseValidate()
        {
            return OnValidate();
        }

        protected virtual bool OnValidate()
        {
            return true;
        }

        public void RaiseValidated()
        {
            OnValidated();
        }

        protected virtual void OnValidated()
        {
        }

        public bool RaiseSaveChanges()
        {
            return OnSaveChanges();
        }

        protected virtual bool OnSaveChanges()
        {
            try
            {
                CurrentStyle.ModelStyle.CellValue = ControlValue;
            }
            catch (Exception ex)
            {
                CurrentStyle.Exception = ex;
                return false;
            }

            return true;
        }

        public void RaiseRejectChanges()
        {
            OnRejectChanges();
        }

        protected virtual void OnRejectChanges()
        {
        }
        #endregion

        #region CurrentCell - Commands and Helpers

        public virtual void RefreshContent()
        {
            SetCurrentCellState(GridControl, CellRowColumnIndex, ActivateOptions);
            if (!HasCurrentCellState)
                throw new InvalidOperationException("RefreshContent is only accesible when renderer is current cell.");
            GridControl.InvalidateCell(CellRowColumnIndex);
        }

        protected virtual bool NotifyCurrentCellChanging()
        {
            if (IsInArrange)
                return true;
            if (!GridControl.Model.IgnoreReadOnly && CurrentStyle.ReadOnly)
                return false;

            if (CurrentCell.IsModified)
                return true;

            return CurrentCell.NotifyChanging();
        }

        protected virtual void NotifyCurrentCellChanged()
        {
            if (IsInArrange)
                return;
            CurrentCell.NotifyChanged();

            if (CurrentCell.IsModified)
                CellModel.RaiseCurrentCellContentChanged();
        }

        #endregion

        #region CurrentCell - Mouse
        protected static void MoveCurrentHelper(FrameworkElement owner, MouseControllerEventArgs e)
        {
            //GridControlBase grid = owner as GridControlBase;
            //DependencyObject dpo = Mouse.PrimaryDevice.DirectlyOver as DependencyObject;
            //while (dpo != null && !(dpo is UIElement))
            //    dpo = GridUtil.GetParent(dpo) as DependencyObject;
            //UIElement el = dpo as UIElement;
            //while (el != null && VirtualizingCellsControl.GetCellsControl(el) != grid)
            //{
            //    UIElement el2 = VirtualizingCellsControl.GetCellsControl(el);
            //    if (object.ReferenceEquals(el,el2))
            //        return;
            //    el = el2;
            //    GridControlBase innerGrid = el as GridControlBase;
            //    if (innerGrid == null)
            //        return;
            //}
            //RowColumnIndex pos = grid.PointToCellRowColumnIndex(e.Location, true);
            //IGridCellRenderer cellRenderer = GridControlBase.GetCellRenderer(el);
            //if (cellRenderer == null || cellRenderer.IsCurrentCell(grid, pos) && cellRenderer.CurrentCell.IsEditing)
            //    return;

            //GridActivateCurrentCellOptions activateOptions = new GridActivateCurrentCellOptions();
            //activateOptions.IsActivateTriggeredByMouseDownIntoUIElement = true;
            //activateOptions.Element = VirtualizingCellsControl.GetCellUIElement(el);

            //if (!grid.CurrentCell.MoveTo(pos.RowIndex, pos.ColumnIndex, activateOptions))
            //    e.Handled = true;
        }
        #endregion

        #region CurrentCell - PreviewGotKeyboardFocus

        //private static void OnPreviewGotKeyboardFocus(object sender, RoutedEventArgs e)
        //{
        //    KeyboardFocusChangedEventArgs previewGotFocus = (KeyboardFocusChangedEventArgs)e;
        //    DependencyObject el = previewGotFocus.NewFocus as DependencyObject;
        //    if (el != null)
        //    {
        //        // This event handler listens to all PreviewGotKeyboardFocus notifications
        //        // in the application. We need to handle it only when it is a UIElement
        //        // that is embedded in a cell.

        //        IGridCellRenderer cellRenderer = GridControlBase.GetCellRenderer(el);
        //        while (cellRenderer != null)
        //        {
        //            RowColumnIndex pos = VirtualizingCellsControl.GetCellRowColumnIndex(el);
        //            GridControlBase grid = VirtualizingCellsControl.GetCellsControl(el) as GridControlBase;

        //            // Make also sure the CellUIElement does not already belong to current
        //            // cell since in that case things are already just perfect.

        //            if (!pos.IsEmpty && grid != null && !grid.CurrentCell.HasCurrentCellAt(pos))
        //            {
        //                // Move current cell.
        //                GridActivateCurrentCellOptions activateOptions = new GridActivateCurrentCellOptions();
        //                activateOptions.IsActivateTriggeredByGotFocus = true;
        //                activateOptions.Element = VirtualizingCellsControl.GetCellUIElement(el);

        //                if (!grid.CurrentCell.MoveTo(pos.RowIndex, pos.ColumnIndex, activateOptions))
        //                {
        //                    e.Handled = true;
        //                    break;
        //                }
        //            }

        //            // activate also parent grids when set focus to cell inside nested grid.
        //            el = grid;
        //            cellRenderer = GridControlBase.GetCellRenderer(el);
        //        }
        //    }
        //}

        #endregion

        #region IGridCellRenderer Members

#if !WinRT
        bool IGridCellRenderer.ShouldGridTryToHandlePreviewKeyDown(KeyEventArgs e)
        {
            return ShouldGridTryToHandlePreviewKeyDown(e);
        }

        protected virtual bool ShouldGridTryToHandlePreviewKeyDown(KeyEventArgs e)
        {
            return true;
        }
        
#else
        bool IGridCellRenderer.ShouldGridTryToHandlePreviewKeyDown(Windows.UI.Xaml.Input.KeyRoutedEventArgs e)
        {
            return ShouldGridTryToHandlePreviewKeyDown(e);
        }
        public virtual bool ShouldGridTryToHandlePreviewKeyDown(Windows.UI.Xaml.Input.KeyRoutedEventArgs e)
        {
            return true;
        }
#endif
#if SyncfusionFramework4_0
        void IGridCellRenderer.RaiseGridPreviewTextInput(TextCompositionEventArgs e)
        {
            OnGridPreviewTextInput(e);
        }

        protected virtual void OnGridPreviewTextInput(TextCompositionEventArgs e)
        {
        }
#endif
        void IGridCellRenderer.RaiseConfirmChangesFailed()
        {
        }

        protected virtual void OnConfirmChangesFailed()
        {
        }

        bool isEditable = true;

        public bool IsEditable
        {
            get
            {
                return isEditable;
            }
            set
            {
                isEditable = value;
            }
        }

        public bool RaiseStartEditing()
        {
            return true;
            //return OnBeforeEditing();
        }

        //protected virtual bool OnBeforeEditing()
        //{
        //    return true;
        //}

        public void RaiseBeginEdit()
        {
            if (this.SupportsRenderOptimization)
            {
                if (this.GridControl.Model.Options.ActivateCurrentCellBehavior == GridCellActivateAction.ClickOnCell && this.CurrentStyle.IsChanged)
                    this.GridControl.InvalidateCell(this.CellRowColumnIndex);
                this.GridControl.DelayedSwapCellUIElements(cellRowColumnIndex, true);
            }
            else if (this is GridCellDataTemplateRenderer)
            {
                this.GridControl.DelayedSwapCellUIElements(cellRowColumnIndex, true);
            }
            ScrollInView();
            OnEnteredEditMode();
        }

        protected virtual void OnEnteredEditMode()
        {
        }

        public void RaiseEndEdit()
        {
            ResetControlText();
            ResetControlValue();
            IsFocused = false;
            //RefreshContent();
            //OnExitEditMode();
#if (SILVERLIGHT || WinRT)
            if (this.SupportsRenderOptimization)
            {
                this.GridControl.DelayedSwapCellUIElements(this.CellRowColumnIndex, false);
            }
#endif
        }

        public void RaiseEditingComplete()
        {
            OnEditingComplete();
        }

        protected virtual void OnEditingComplete()
        {
            gridControl.InvalidateCell(CellRowColumnIndex);
            gridControl.InvalidateVisual(false);
        }

        public void UpdateCellRowColumnIndex(RowColumnIndex cellRowColumnIndex)
        {
            this.cellRowColumnIndex = cellRowColumnIndex;
            currentStyle.CellIdentity.UpdateCellRowColumnIndex(cellRowColumnIndex);
            currentStyle.ModelStyle.CellIdentity.UpdateCellRowColumnIndex(cellRowColumnIndex);
        }

         public void UpdateCurrentStyle()
        {
            this.currentStyle = gridControl.GetRenderStyleInfo(cellRowColumnIndex);
        }

        //bool invalidateDirty = false;

        //public virtual void Invalidate()
        //{
        //    //invalidateDirty = true;
        //}

        #endregion

        #region IDisposable Members

        public void Dispose()
        {
            Dispose(true);
        }

        protected override void Dispose(bool isDisposing)
        {
        }

        #endregion

        #region IHitTestSelectCells Members

        public virtual void MouseDown(FrameworkElement owner, MouseControllerEventArgs e)
        {
            MoveCurrentHelper(owner, e);
        }

        #endregion


        #region IStyleChanged Members

        public void StyleChanged(StyleChangedEventArgs e)
        {
            if (e.Sip == GridStyleInfoStore.CellValueProperty
                || e.Sip == null && currentStyle.Store.IsValueModified(GridStyleInfoStore.CellValueProperty))
            {
                SetControlValue(currentStyle.CellValue);
            }
        }

        #endregion

        #region IGridCellRenderer Members


        public virtual void RaiseGridPreviewMouseMove(RowColumnIndex rci, MouseEventArgs e)
        {
        }

        #endregion

        public virtual void RaiseGridCellClick(int rowIndex, int colIndex, MouseControllerEventArgs e)
        {
            GridControl.RaiseGridCellClick(rowIndex, colIndex);
        }
    }
}
