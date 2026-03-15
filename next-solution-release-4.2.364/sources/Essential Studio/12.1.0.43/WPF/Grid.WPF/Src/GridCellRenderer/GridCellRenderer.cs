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
using Syncfusion.Windows.Controls.Cells;
using Syncfusion.Windows.Controls.Scroll;
using Syncfusion.Windows.Data;  
using Syncfusion.Windows.Diagnostics;
using Syncfusion.Windows.GridCommon;
using Syncfusion.Windows.Styles;
using Syncfusion.Windows.Shared;
using System.Windows.Automation.Peers;
using System.Windows.Controls;
using System.ComponentModel;
using Syncfusion.Linq;
using System.Linq;
using System.Collections.Specialized;
using System.Collections.Generic;
using System.Windows.Media;

namespace Syncfusion.Windows.Controls.Grid
{
	/// <summary>
	/// This is a base class for the renderer part of a cell type.
	/// </summary>
	/// <remarks>
	/// A renderer is created for each <see cref="GridCellModelBase"/>
	/// and <see cref="GridControlBase"/>.
	/// <para/>
	/// You typically access cell renderers through the <see cref="GridControlBase.CellRenderers"/>
	/// property of the <see cref="GridControlBase"/> class.<para/>
	/// </remarks>
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
			EventManager.RegisterClassHandler(typeof(UIElement), UIElement.PreviewGotKeyboardFocusEvent, new RoutedEventHandler(OnPreviewGotKeyboardFocus));
		}

		/// <summary>
		/// Initializes a new <see cref="GridCellRendererBase"/>.
		/// </summary>
		public GridCellRendererBase()
		{
			this.weakReference = new WeakReference(this);
		}

		#region Features
		/// <summary>
		/// Gets or sets a value that indicates whether the control text is displayed.
		/// </summary>
		public bool IsControlTextShown
		{
			get { return isControlTextShown; }
			set { isControlTextShown = value; }
		}

		/// <summary>
		/// Gets or sets a value that indicates whether the cell is focusable.
		/// </summary>
		public bool IsFocusable
		{
			get { return isFocusable; }
			set { isFocusable = value; }
		}

		/// <summary>
		/// Gets or sets a value that indicates whether the cell is modifiable.
		/// </summary>
		public bool IsModifiable
		{
			get { return isModifiable; }
			set { isModifiable = value; }
		}

		/// <summary>
		/// Gets or sets a value that indicates whether the cell can be dropped down.
		/// </summary>
		public bool IsDropDownable
		{
			get { return isDropDownable; }
			set { isDropDownable = value; }
		}

		Type editorType;
		/// <summary>
		/// Returns the type of the cell editor (cell control).
		/// </summary>
		public Type EditorType
		{
			get { return editorType; }
			set { editorType = value; }
		}
		#endregion

		#region Created

		/// <summary>
		/// Occurs when the current cell model is created.
		/// </summary>
		/// <param name="cellModel">The cell model.</param>
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
		/// <summary>
		/// Returns the cell model.
		/// </summary>
		public GridCellModelBase CellModel
		{
			get { return cellModel; }
		}
		#endregion

		/// <summary>
		/// Returns the parent grid.
		/// </summary>
		public GridControlBase GridControl
		{
			get
			{
				return gridControl;
			}
			set { gridControl = value; }
		}

		#region CurrentCell State - GridControl, CellRowColumnIndex, ActivateOptions, CurrentCell, CurrentStyle, CurrentCellUIElement

		/// <summary>
		/// Returns the cell row column index.
		/// </summary>
		public RowColumnIndex CellRowColumnIndex
		{
			get
			{
				if (!HasCurrentCellState)
					throw new InvalidOperationException("CellRowColumnIndex is only accesible when renderer is current cell. Check GridRenderStyleInfo.CellRowColumnIndex instead.");
				return cellRowColumnIndex;
			}
		}

		/// <summary>
		/// Returns the options to activate the current cell.
		/// </summary>
		public GridActivateCurrentCellOptions ActivateOptions
		{
			get
			{
				if (!HasCurrentCellState)
					throw new InvalidOperationException("ActivateOptions is only accesible when renderer is current cell.");
				return activateOptions;
			}
		}

		/// <summary>
		/// Returns the row index of the cell.
		/// </summary>
	public int RowIndex
		{
			get
			{
				return CellRowColumnIndex.RowIndex;
			}
		}

		/// <summary>
		/// Returns the column index of the cell.
		/// </summary>
		public int ColumnIndex
		{
			get
			{
				return CellRowColumnIndex.ColumnIndex;
			}
		}

		/// <summary>
		/// Returns the current cell.
		/// </summary>
		public GridCurrentCell CurrentCell
		{
			get
			{
				return GridControl.CurrentCell;
			}
		}

		/// <summary>
		/// Returns the current cell style.
		/// </summary>
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

		/// <summary>
		/// Returns a copy of the current cell style.
		/// </summary>
		public GridRenderStyleInfo CurrentStyleCopy
		{
			get
			{
                if (!HasCurrentCellState && currentStyleCopy ==null)
					throw new InvalidOperationException("CurrentStyle is only accesible when renderer is current cell.");
				return currentStyleCopy;
			}
		}

		/// <summary>
		/// Returns the UI element of the current cell.
		/// </summary>
		public UIElement CurrentCellUIElement
		{
			get
			{
                if (!HasCurrentCellState && currentCellUIElement ==null)
                    throw new InvalidOperationException("FocusedElement is only accesible when renderer is current cell.");
				return currentCellUIElement;
			}
			protected set
			{
                //if (EditorType != null)
                //{
                //    if (value != null && !value.GetType().IsAssignableFrom(EditorType))
                //        throw new InvalidCastException("CurrentCellUIElement should be of type " + EditorType.Name);
                //}
				currentCellUIElement = value;
			}
		}

		/// <summary>
		/// Specifies whether the cell state for the current cell has been set.
		/// </summary>
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
		}

		/// <summary>
		/// Determines whether the given cell coordinates represent the current cell.
		/// </summary>
		/// <param name="gridControl">The grid.</param>
		/// <param name="cellRowColumnIndex">The cell row column index.</param>
		/// <returns>True if it is current cell; false otherwise.</returns>
		public bool IsCurrentCell(GridControlBase gridControl, RowColumnIndex cellRowColumnIndex)
		{
			return hasCurrentCellState
				&& gridControl.CurrentCell.HasCurrentCell
				&& this.gridControl == gridControl
				&& this.cellRowColumnIndex == cellRowColumnIndex;
		}

		/// <summary>
		/// Determines whether the given cell style represents the current cell.
		/// </summary>
		/// <param name="style">Cell style information.</param>
		/// <returns>True if it is current cell; false otherwise.</returns>
		public bool IsCurrentCell(GridRenderStyleInfo style)
		{
			return style != null && IsCurrentCell(style.GridControl, style.CellRowColumnIndex);
		}

		/// <summary>
		/// Determines whether the given cell element corresponds to the current cell.
		/// </summary>
		/// <param name="uiElement">The cell control.</param>
		/// <returns>True if it is current cell; false otherwise.</returns>
		public bool IsCurrentCell(UIElement uiElement)
		{
			GridControlBase grid = VirtualizingCellsControl.GetCellRendererParentControl(uiElement) as GridControlBase;
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
		}
		#endregion

		#region CurrentCell State - IsFocused, IsModified, IsDroppedDown
		/// <summary>
		/// Gets or sets a value that indicates whether the cell has the focus.
		/// </summary>
		public bool IsFocused
		{
			get
			{
				return CurrentCellUIElement != null && CurrentCellUIElement.IsKeyboardFocusWithin;
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
							if (!CurrentCellUIElement.IsKeyboardFocusWithin && !(CurrentCell.IsInActivate && ActivateOptions.IsActivateTriggeredByGotFocus))
								CurrentCellUIElement.Focus();
							OnSetFocus();
						}
					}
					else
					{
						if (this.AllowGridToFocus)
						{
							gridControl.Focus();
						}
						OnResetFocus();
					}
				}
			}
		}

		protected virtual void OnResetFocus()
		{
		}

		protected virtual void OnSetFocus()
		{
		}

		/// <summary>
		/// Specifies wheter the cell has been modified.
		/// </summary>
		public bool IsModified
		{
			get
			{
				return GridControl.CurrentCell.IsModified;
			}
		}

		/// <summary>
		/// Specifies whether the current cell is dropped down.
		/// </summary>
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

		/// <summary>
		/// Sets the given value as control text for the cell.
		/// </summary>
		/// <param name="value">The value.</param>
		/// <returns>True if the control text has been set successfully; false otherwise.</returns>
		public bool SetControlText(string value)
		{
			return SetControlText(value, false);
		}

		/// <summary>
		/// Sets the given value as control text for the cell.
		/// </summary>
		/// <param name="value">The value.</param>
		/// <param name="enforceApplyControlText">When true, enforces applying control text.</param>
		/// <returns>True if the control text has been set successfully; false otherwise.</returns>
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
                ///Adding this code to fire NotifyPropertyChanged event if UIElementValue equals to Intial Value of the style but it was modified.
                if (!InInitialize && IsModified)
                    NotifyCurrentCellChanged();

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

                    bool bInvalidate = controlText != value;

					hasControlText = true;
					controlText = value;

					// Reinitialize editor with new text (taken from ControlText)
					if (editorText != null && editorText != controlText)
						RefreshContent();
                    else if (bInvalidate)
                        GridControl.InvalidateCell(CellRowColumnIndex);
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

		/// <summary>
		/// Validates the control text.
		/// </summary>
		/// <param name="value">Value to be validated.</param>
		/// <returns>True.</returns>
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
                if (style.CellValue == DBNull.Value)
                    style.CellValue = string.Empty;
                else
                    style.CellValue = null;
				style.Exception = ex;
				return false;
			}
			return true;
		}

		/// <summary>
		/// Retrieves the ControlText of the current cell.
		/// </summary>
		/// <param name="style">The cell style.</param>
		/// <returns>The control text.</returns>
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

		/// <summary>
		/// Specifies whether the current cell should show the cell text that is being entered.
		/// </summary>
		/// <returns>True if the text is shown; false otherwise.</returns>
		public virtual bool ShouldCurrentCellShowCellText()
		{
			return CurrentCell.IsEditing && this.IsEditable;
		}

		/// <summary>
		/// Retrieves the ControlText from the cell editor.
		/// </summary>
		/// <returns>ControlText.</returns>
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
		/// Gets or sets the cell value for the current cell.
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

		/// <summary>
		/// Sets the cell value.
		/// </summary>
		/// <param name="value">the value to be set.</param>
		/// <returns>True if the value has been set sucessfully; false otherwise.</returns>
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

		/// <summary>
		/// Validates the cell value.
		/// </summary>
		/// <param name="value">The value to be validated.</param>
		/// <returns>True.</returns>
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
			if (type == null || value == null || value is DBNull)
			{
			}
			else if (value.GetType() != type)
			{
				value = Syncfusion.Windows.Styles.ValueConvert.ChangeType(value, type, ci, returnDbNullIfNotValid);
				if (value is DBNull)
					value = null;
			}

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

		/// <summary>
		/// Gets the cell value from the given cell style.
		/// </summary>
		/// <param name="style">The cell style.</param>
		/// <returns>The cell value.</returns>
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
		/// <summary>
		/// Calls OnInitialize().
		/// </summary>
		/// <param name="options">The options to activate the current cell.</param>
		public void RaiseInitialize(GridActivateCurrentCellOptions options)
		{
			inInitialize = true;
			currentStyleCopy = CurrentStyle.Copy();

			OnInitialize();
			inInitialize = false;
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
			// when mouse down scroll tends be jumping if we scroll into view
			if (!activateOptions.IsActivateTriggeredByMouseDownIntoUIElement)
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

		/// <summary>
		/// Calls OnActivating().
		/// </summary>
		/// <param name="gridControl">The grid.</param>
		/// <param name="cellRowColumnIndex">The cell row column index.</param>
		/// <param name="options">The options to activate the current cell.</param>
		/// <returns>True.</returns>
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

		/// <summary>
		/// Calls OnActivated().
		/// </summary>
		public void RaiseActivated()
		{
			GridControl.Model.CurrentCellState = new GridModelCurrentCellState(GridControl, CellRowColumnIndex);
			OnActivated();
		}

		protected virtual void OnActivated()
		{
		}

		/// <summary>
		/// Calls OnActivateFailed().
		/// </summary>
		public void RaiseActivateFailed()
		{
			OnActivateFailed();
			CellModel.ActiveRenderer = null;
		}

		protected virtual void OnActivateFailed()
		{
		}

		/// <summary>
		/// Calls OnDeactivating().
		/// </summary>
		/// <returns>True.</returns>
		public bool RaiseDeactivating()
		{
			return OnDeactivating();
		}

		protected virtual bool OnDeactivating()
		{
			return true;
		}

		/// <summary>
		/// Calls OnDeactivated() and resets the current cell.
		/// </summary>
		public void RaiseDeactivated()
		{
			GridControlBase gridControl = GridControl;
            if (gridControl.CurrentCell != null) //&& gridControl.CurrentCell.HasCurrentCell)
            {
                // RowColumnIndex cellRowColumnIndex = CellRowColumnIndex;Unused local variable
                OnDeactivated();
                gridControl.Model.CurrentCellState = GridModelCurrentCellState.Empty;

                ResetCurrentCellState(); // Resets GridControl to null.
                CellModel.ActiveRenderer = null;

                //gridControl.InvalidateVisual(false);
            }
		}

		protected virtual void OnDeactivated()
		{
			//gridControl.InvalidateCell(cellRowColumnIndex);
		}

		/// <summary>
		/// Calls OnDeactivateFailed().
		/// </summary>
		public void RaiseDeactivateFailed()
		{
			OnDeactivateFailed();
		}

		protected virtual void OnDeactivateFailed()
		{
		}

		/// <summary>
		/// Calls OnValidate().
		/// </summary>
		/// <returns>True.</returns>
		public bool RaiseValidate()
		{
			return OnValidate();
		}

		protected virtual bool OnValidate()
		{
			return true;
		}


		/// <summary>
		/// call OnClipboardPaste(GridCutPasteEventArgs args)
		/// </summary>
		/// <returns></returns>

		public void RaiseClipboardPaste(GridCutPasteEventArgs args)
		{
			OnClipboardPaste(args);
		}

		protected virtual void OnClipboardPaste(GridCutPasteEventArgs args)
		{

		}

        /// <summary>
        /// call OnClipboardPasted(GridCutPasteEventArgs args)
        /// </summary>
        /// <returns></returns>

        public void RaiseClipboardPasted(GridCutPasteEventArgs args)
        {
            OnClipboardPasted(args);
        }

        protected virtual void OnClipboardPasted(GridCutPasteEventArgs args)
        {

        }


		/// <summary>
		/// Calls OnValidated().
		/// </summary>
		public void RaiseValidated()
		{
			OnValidated();
		}

		protected virtual void OnValidated()
		{
		}

		/// <summary>
		/// Calls OnSaveChanges().
		/// </summary>
		/// <returns>True if the changes has been saved successfully.</returns>
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

		/// <summary>
		/// Calls OnRejectChanges().
		/// </summary>
		public void RaiseRejectChanges()
		{
			OnRejectChanges();
		}

		protected virtual void OnRejectChanges()
		{
		}
		#endregion

		#region CurrentCell - Commands and Helpers

		/// <summary>
		/// Refreshes the current cell.
		/// </summary>
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
            GridDataControl gridDataControl = CurrentCell.Grid.FindParentElementOfType<GridDataControl>();
            if (gridDataControl != null)
            {
                if (gridDataControl.Model.TableProperties.UpdateMode != UpdateMode.PropertyChanged && CurrentCell.IsModified)     // In property changed mode every time when we changing the cell value NotifyCurrentCellChanging() should be fired. 
                {
                   return true;
                }
            }
            
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
			GridControlBase grid = owner as GridControlBase;
			DependencyObject dpo = Mouse.PrimaryDevice.DirectlyOver as DependencyObject;
			while (dpo != null && !(dpo is UIElement))
				dpo = GridUtil.GetParent(dpo) as DependencyObject;
			UIElement el = dpo as UIElement;
			while (el != null && VirtualizingCellsControl.GetCellRendererParentControl(el) != grid)
			{
				UIElement el2 = VirtualizingCellsControl.GetCellRendererParentControl(el);
				if (object.ReferenceEquals(el, el2))
					return;
				el = el2;
				GridControlBase innerGrid = el as GridControlBase;
				if (innerGrid == null)
					return;
			}
            var loc = e.Location;
            loc.Y -= 0.4;
            RowColumnIndex pos = grid.PointToCellRowColumnIndex(loc, true);
			IGridCellRenderer cellRenderer = GridControlBase.GetCellRenderer(el);
			if (cellRenderer == null || cellRenderer.IsCurrentCell(grid, pos) && cellRenderer.CurrentCell.IsEditing)
				return;

			GridActivateCurrentCellOptions activateOptions = new GridActivateCurrentCellOptions();
			activateOptions.IsActivateTriggeredByMouseDownIntoUIElement = true;
			activateOptions.Element = VirtualizingCellsControl.GetCellUIElement(el);
			activateOptions.ShouldBeginEdit = (grid.Model.Options.ActivateCurrentCellBehavior & (GridCellActivateAction.SetCurrent | GridCellActivateAction.ClickOnCell | GridCellActivateAction.SelectAll)) != 0;

            //The below code was added to skip the selection while moving the Current Cell to header cells.
            //Issue: Column Selected while clicking the FilterIcon, If AllowSelection is Column.
            if (pos.RowIndex < grid.Model.HeaderRows || pos.ColumnIndex < grid.Model.HeaderColumns)
                activateOptions.SetCurrentCellOptions = GridSetCurrentCellOptions.NoSelectRange;

			if (!grid.CurrentCell.MoveTo(pos.RowIndex, pos.ColumnIndex, activateOptions))
				e.Handled = true;
		}
		#endregion

		#region CurrentCell - PreviewGotKeyboardFocus

		private static void OnPreviewGotKeyboardFocus(object sender, RoutedEventArgs e)
		{
			KeyboardFocusChangedEventArgs previewGotFocus = (KeyboardFocusChangedEventArgs)e;
			DependencyObject el = previewGotFocus.NewFocus as DependencyObject;
			if (el != null)
			{
				// This event handler listens to all PreviewGotKeyboardFocus notifications
				// in the application. We need to handle it only when it is a UIElement
				// that is embedded in a cell.

				IGridCellRenderer cellRenderer = GridControlBase.GetCellRenderer(el);
				while (cellRenderer != null)
				{
					GridControlBase grid = VirtualizingCellsControl.GetCellRendererParentControl(el) as GridControlBase;
					RowColumnIndex pos = VirtualizingCellsControl.GetCellRowColumnIndex(el);

                    bool canmovecurrentcell = true;
                    if (cellRenderer is GridDataCellNestedGridRenderer)
                    {
                        var nestedcellrenderer = cellRenderer as GridDataCellNestedGridRenderer;
                        if (!nestedcellrenderer.CurrentCell.HasCurrentCell)
                            canmovecurrentcell = false;
                        else if (!nestedcellrenderer.CurrentCell.IsEditing)
                            canmovecurrentcell = false;
                    }
					// Make also sure the CellUIElement does not already belong to current
					// cell since in that case things are already just perfect.

					if (!pos.IsEmpty && grid != null && canmovecurrentcell)
					{
                        if (!grid.CurrentCell.HasCurrentCellAt(pos))
                        {
                            GridActivateCurrentCellOptions activateOptions = new GridActivateCurrentCellOptions();
                            activateOptions.IsActivateTriggeredByGotFocus = true;
                            activateOptions.Element = VirtualizingCellsControl.GetCellUIElement(el);

                            if (!grid.CurrentCell.MoveTo(pos.RowIndex, pos.ColumnIndex, activateOptions))
                            {
                                e.Handled = true;
                                break;
                            }
                        }
                        else
                        {
                            if (!grid.CurrentCell.IsEditing && grid.CurrentCell.Renderer is GridDataDataTemplateCellRenderer)
                            {
                                GridActivateCurrentCellOptions activateOptions = new GridActivateCurrentCellOptions();
                                activateOptions.IsActivateTriggeredByGotFocus = true;
                                activateOptions.Element = VirtualizingCellsControl.GetCellUIElement(el);
                                grid.CurrentCell.Activate(pos, activateOptions);
                            }
                        }
					}

					// activate also parent grids when set focus to cell inside nested grid.
					el = grid;
					cellRenderer = GridControlBase.GetCellRenderer(el);
				}
			}
		}

		#endregion

		#region IGridCellRenderer Members


		bool IGridCellRenderer.ShouldGridTryToHandlePreviewKeyDown(KeyEventArgs e)
		{
			return ShouldGridTryToHandlePreviewKeyDown(e);
		}

        protected virtual bool ShouldGridTryToHandlePreviewKeyDown(KeyEventArgs e)
        {
            return true;
        }

#if !SILVERLIGHT

        void IGridCellRenderer.RenderForPrinting(DrawingContext dc, RenderCellArgs rca, GridRenderStyleInfo cellInfo)
        {
            OnRenderForPrinting(dc, rca, cellInfo);
        }

        /// <summary>
        /// Called from <see cref="IGridCellRenderer.RenderForPrinting"/> to manually 
        /// force to render graphics while printing that do not belong to live controls and the cells which is not view (e.g. static text).
        /// </summary>
        /// <param name="dc">The drawing context.</param>
        /// <param name="rca">The render cell layout information.</param>
        /// <param name="cellInfo">The cell style info.</param>
        protected virtual void OnRenderForPrinting(DrawingContext dc, RenderCellArgs rca, GridRenderStyleInfo cellInfo)
        {
            OnRender(dc, rca, cellInfo);
        }
#endif

		void IGridCellRenderer.RaiseGridPreviewTextInput(TextCompositionEventArgs e)
		{
			OnGridPreviewTextInput(e);
		}

		protected virtual void OnGridPreviewTextInput(TextCompositionEventArgs e)
		{
		}

		void IGridCellRenderer.RaiseConfirmChangesFailed()
		{
		}

		protected virtual void OnConfirmChangesFailed()
		{
		}

		bool isEditable = true;

		/// <summary>
		/// Gets or sets a valude that indicates whether the cell is editable.
		/// </summary>
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

		/// <summary>
		/// Starts editing.
		/// </summary>
		/// <returns>True.</returns>
		public virtual bool RaiseStartEditing()
		{
			return true;
			//return OnBeforeEditing();
		}

		//protected virtual bool OnBeforeEditing()
		//{
		//    return true;
		//}

		/// <summary>
		/// Begins the editing mode for the cell.
		/// </summary>
		public void RaiseBeginEdit()
		{
			// when mouse down scroll tends be jumping if we scroll into view
			if (!activateOptions.IsActivateTriggeredByMouseDownIntoUIElement)
				ScrollInView();
			OnEnteredEditMode();
		}

		protected virtual void OnEnteredEditMode()
		{
		}

		/// <summary>
		/// Ends the editing mode for the cell.
		/// </summary>
		public void RaiseEndEdit()
		{
			ResetControlText();
			ResetControlValue();
			IsFocused = false;
			//RefreshContent();
			//OnExitEditMode();
		}

		/// <summary>
		/// Calls OnEditingComplete().
		/// </summary>
		public void RaiseEditingComplete()
		{
			OnEditingComplete();
		}

		protected virtual void OnEditingComplete()
		{
			gridControl.InvalidateCell(CellRowColumnIndex);
			gridControl.InvalidateVisual(false);
		}

		/// <summary>
		/// Updates the cell row column index with the given value.
		/// </summary>
		/// <param name="cellRowColumnIndex">The cell row column index.</param>
		public void UpdateCellRowColumnIndex(RowColumnIndex cellRowColumnIndex)
		{
			this.cellRowColumnIndex = cellRowColumnIndex;
            if (CurrentStyle.CellIdentity != null)
            {
                currentStyle.CellIdentity.UpdateCellRowColumnIndex(cellRowColumnIndex);
                if (currentStyle.ModelStyle.CellIdentity != null)
                    currentStyle.ModelStyle.CellIdentity.UpdateCellRowColumnIndex(cellRowColumnIndex);

                
            }
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

		protected override void Dispose(bool isDisposing)
		{
		}

		#endregion

		#region IHitTestSelectCells Members

		/// <summary>
		/// Occurs when the mouse is down in a cell.
		/// </summary>
		/// <param name="owner">The owner element.</param>
		/// <param name="e">The <see cref="MouseControllerEventArgs"/> object.</param>
        public virtual void MouseDown(FrameworkElement owner, MouseControllerEventArgs e)
        {
            //SD5656 has to be reviewed
            //GridControlBase grid = owner as GridControlBase;
            //var mouseControllerDispatcher = grid.MouseControllerDispatcher.Find("SelectCellsMouseController") as GridSelectCellsMouseController;
            //if (mouseControllerDispatcher != null)
            //    mouseControllerDispatcher.MouseDown(e);

            //Function invoked for FilterBar Celll and DataTemplate cell. 
            //Now handling Data template selection in SelectedCellsMouseController. 
            //So adding condition check to skip the DataTemplate cell.
            if ((!(this is GridCellDataBoundTemplateRenderer)) && this.gridControl == owner)
                MoveCurrentHelper(owner, e);
        }

		#endregion


		#region IStyleChanged Members

		/// <summary>
		/// Occurs when the current cell style is changed.
		/// </summary>
		/// <param name="e">The <see cref="StyleChangedEventArgs"/> object.</param>
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

		/// <summary>
		/// Triggers PreviewMouseMove event for the given cell coordinates.
		/// </summary>
		/// <param name="rci">The row column index.</param>
		/// <param name="e">A <see cref="MouseEventArgs"/> object.</param>
		public virtual void RaiseGridPreviewMouseMove(RowColumnIndex rci, MouseEventArgs e)
		{
		}

		#endregion

		/// <summary>
		/// Raises the GridCellClick event.
		/// </summary>
		/// <param name="rowIndex">The cell row index.</param>
		/// <param name="colIndex">The cell column index.</param>
		/// <param name="e">A reference to <see cref="MouseControllerEventArgs"/>.</param>
		public virtual void RaiseGridCellClick(int rowIndex, int colIndex, MouseControllerEventArgs e)
        {
#if !SILVERLIGHT
            var mouseButtonArgs = e.SourceEventArgs as MouseButtonEventArgs;
            RowColumnIndex cell = this.gridControl.PointToCellRowColumnIndex(mouseButtonArgs);
            GridStyleInfo style = this.gridControl.Model[cell.RowIndex, cell.ColumnIndex];

            if (mouseButtonArgs.ChangedButton == MouseButton.Right && this.gridControl.Model.EnableContextMenu)
            {
                style.BeginInit();
               GridQueryContextMenuInfoEventArgs contextMenuEventArgs = new GridQueryContextMenuInfoEventArgs(cell, style, false);
               contextMenuEventArgs.Style.ContextMenuItems = null;
                this.gridControl.Model.RaiseQueryContextMenuInfo(contextMenuEventArgs);
                style.EndInit();
            }
            if (!this.GridControl.Model.EnableContextMenu || mouseButtonArgs.ChangedButton == MouseButton.Left)
            {
                e.Handled = !GridControl.RaiseGridCellClick(rowIndex, colIndex, e.ClickCount);
            }
#else
            e.Handled = !GridControl.RaiseGridCellClick(rowIndex, colIndex, e.ClickCount);
#endif
		}

		/// <summary>
		/// Empties the recyclebin.
		/// </summary>
		public virtual void EmptyRecycleBin()
		{
		}

		#region IGridCellRenderer Members

		public virtual System.Windows.Automation.Peers.AutomationPeer OnCreateAutomationPeer()
		{
			var currentCellUIElement = this.CurrentCellUIElement;
			if (currentCellUIElement != null)
			{
				return FrameworkElementAutomationPeer.CreatePeerForElement(currentCellUIElement);
			}

			return null;
		}

		private bool allowGridToFocus = true;
		public bool AllowGridToFocus
		{
			get { return this.allowGridToFocus; }
			set { this.allowGridToFocus = value; }
		}

		bool supportsRenderOptimization = false;

		/// <summary>
		/// Gets or sets whether the renderer supports rendering itsself directly to the
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

		#endregion
	}

	/// <summary>
	/// This is a helper class that provides information about the current visual style of any grid.
	/// </summary>
	public class GridVisualStyleHelper
	{
		/// <summary>
		/// Gets the visual style of all grid types with respect to the given framework element.
		/// </summary>
		/// <param name="dpo">The framework element.</param>
		/// <returns>The name of the visual style.</returns>
		public static string GetVisualStyleOfGrid(FrameworkElement dpo)
		{
			var grid = dpo.FindParentElementOfType<GridControl>();
			if (grid != null)
			{
				return SkinStorage.GetVisualStyle(grid);
			}

			var gridDataControl = dpo.FindParentElementOfType<GridDataControl>();
			if (gridDataControl != null)
			{
				return SkinStorage.GetVisualStyle(gridDataControl);
			}

			var gridTreeControl = dpo.FindParentElementOfType<GridTreeControl>();
			if (gridTreeControl != null)
			{
				return SkinStorage.GetVisualStyle(gridTreeControl);
			}

			return string.Empty;
		}
	}
#if !SILVERLIGHT
    public static class ContextMenuCommands
    {

        private static DelegateCommand<GridDataTableProperties> sortAscending;
        private static DelegateCommand<GridDataTableProperties> sortDescending;
        private static DelegateCommand<GridDataTableProperties> clearSort;
        private static DelegateCommand<GridDataTableProperties> clearFilter;
        private static DelegateCommand<GridDataTableProperties> hideColumn;
        private static DelegateCommand<GridDataTableProperties> expandGroup;
        private static DelegateCommand<GridDataTableProperties> collapseGroup;
        private static DelegateCommand<GridDataTableProperties> delete;
        private static DelegateCommand<GridDataTableProperties> groupBy;
        private static DelegateCommand<GridDataTableProperties> hideGroupDropArea;
        private static DelegateCommand<GridDataTableProperties> bestFit;

        public static DelegateCommand<GridDataTableProperties> BestFit
        {
            get
            {
                if (bestFit == null)
                {
                    bestFit = new DelegateCommand<GridDataTableProperties>(BestFitMethod);
                }

                return bestFit;
            }
        }

        public static DelegateCommand<GridDataTableProperties> HideGroupDropArea
        {
            get
            {
                if (hideGroupDropArea == null)
                {
                    hideGroupDropArea = new DelegateCommand<GridDataTableProperties>(HideGroupDropAreaMethod);
                }

                return hideGroupDropArea;
            }
        }
       
        public static DelegateCommand<GridDataTableProperties> GroupBy
        {
            get
            {
                if (groupBy == null)
                {
                    groupBy = new DelegateCommand<GridDataTableProperties>(GroupByMethod);
                }

                return groupBy;
            }
        }
                                
        public static DelegateCommand<GridDataTableProperties> HideColumn
        {
            get
            {
                if (hideColumn == null)
                {
                    hideColumn = new DelegateCommand<GridDataTableProperties>(HideColumnMethod);
                }

                return hideColumn;
            }
        }
		
        public static DelegateCommand<GridDataTableProperties> ClearFilter
        {
            get
            {
                if (clearFilter == null)
                {
                    clearFilter = new DelegateCommand<GridDataTableProperties>(ClearFilterMethod);
                }

                return clearFilter;                
            }
        }
		
        public static DelegateCommand<GridDataTableProperties> SortAscending
        {
            get
            {
                if (sortAscending == null)
                {
                    sortAscending = new DelegateCommand<GridDataTableProperties>(SortAscendingMethod);
                }

                return sortAscending;
            }
        }
		
        public static DelegateCommand<GridDataTableProperties> SortDescending
        {
            get
            {
                if (sortDescending == null)
                {
                    sortDescending = new DelegateCommand<GridDataTableProperties>(SortDescendingMethod);
                }

                return sortDescending;
            }
        }
		
        public static DelegateCommand<GridDataTableProperties> ExpandGroup
        {
            get
            {
                if (expandGroup == null)
                {
                    expandGroup = new DelegateCommand<GridDataTableProperties>(ExpandGroupMethod);
                }

                return expandGroup;
            }
        }
		
        public static DelegateCommand<GridDataTableProperties> CollapseGroup
        {
            get
            {
                if (collapseGroup == null)
                {
                    collapseGroup = new DelegateCommand<GridDataTableProperties>(CollapseGroupMethod);
                }

                return collapseGroup;
            }
        }
		
        public static DelegateCommand<GridDataTableProperties> ClearSort
        {
            get
            {
                if (clearSort == null)
                {
                    clearSort = new DelegateCommand<GridDataTableProperties>(ClearSortMethod);
                }

                return clearSort;
            }
        }
		
        public static DelegateCommand<GridDataTableProperties> Delete
        {
            get
            {
                if (delete == null)
                {
                    delete = new DelegateCommand<GridDataTableProperties>(DeleteMethod);
                }

                return delete;
            }
        }

        private static void BestFitMethod(GridDataTableProperties tableProperties)
        {
            GridQueryContextMenuInfoEventArgs contextMenuInfoArgs = tableProperties.Model.ContextMenuEventArgs as GridQueryContextMenuInfoEventArgs;
            var visibleColumnIndex = tableProperties.Model.ResolvePositionToVisibleColumnIndex(contextMenuInfoArgs.Cell.ColumnIndex);
            if (visibleColumnIndex >= 0 && visibleColumnIndex < tableProperties.VisibleColumns.Count)
            {
                tableProperties.Model.ResizeColumnsToFit(GridRangeInfo.Col(contextMenuInfoArgs.Cell.ColumnIndex), GridResizeToFitOptions.None);
            }
        }

        private static void HideGroupDropAreaMethod(GridDataTableProperties tableProperties)
        {
            var gridDataControl = tableProperties.Model.Grid.FindParentElementOfType<GridDataControl>();
            if (gridDataControl.ShowGroupDropArea)
            {
                gridDataControl.ShowGroupDropArea = false;
            }
            else
            {
                gridDataControl.ShowGroupDropArea = true;
            }
            
            tableProperties.Model.View.Refresh();
        }

        private static void GroupByMethod(GridDataTableProperties tableProperties)
        {
            GridQueryContextMenuInfoEventArgs contextMenuInfoArgs = tableProperties.Model.ContextMenuEventArgs as GridQueryContextMenuInfoEventArgs;
            var style = new GridDataStyleInfo(contextMenuInfoArgs.Style.CellIdentity);
            var tableStyleInfoIdentity = style.CellIdentity;
            var visibleColumn = tableStyleInfoIdentity.Column;
            bool isChecked = false;
            
            foreach (var groupCol in tableProperties.GroupedColumns)
            {
                if (groupCol.ColumnName == visibleColumn.MappingName)
                {
                    isChecked = true; 
                    tableProperties.GroupedColumns.Remove(groupCol);
                    break;
                }
            }

            if (!isChecked)
            {
                GridDataGroupColumn groupColumn = new GridDataGroupColumn()
                {
                    ColumnName = visibleColumn.MappingName
                };
                tableProperties.GroupedColumns.Add(groupColumn);
            }

            tableProperties.Model.View.Refresh();
            tableProperties.Model.InvalidateVisual(true);
        }

        private static void DeleteMethod(GridDataTableProperties tableProperties)
        {
            GridQueryContextMenuInfoEventArgs contextMenuInfoArgs = tableProperties.Model.ContextMenuEventArgs as GridQueryContextMenuInfoEventArgs;
            object currentRecord = null;
            int removedRecordIndex;
            int RecordIndex;

            if (tableProperties.AllowMultipleRecordDeletion)
            {
                List<int> RangeTop = new List<int>();
                List<object> RecordsToDelete = new List<object>();
                bool IsNestedCollection = false;

                //Checking nested collection
                if (tableProperties.Relations.Count > 0)
                    IsNestedCollection = true;

                if (tableProperties.Model.SelectedRanges.Count-1 == 1)
                {
                    List<int> RowTop = new List<int>();
                    var Activerange = tableProperties.Model.SelectedRanges[0];
                    int SingleIncrement = 1;
                    int DoubleIncrement = 2;
                    // Code to get the row index when the selection is Table.
                    if (Activerange.IsTable)
                    {
                        //If the table has groups we expand all groups to calculate rowIndex Correctly.
                        if (tableProperties.Model.Table.HasGroups)
                            tableProperties.Model.Table.ExpandAllGroups();

                        for (int Row = 0; Row < tableProperties.Model.RowCount; Row = (IsNestedCollection == true) ? Row + DoubleIncrement : Row + SingleIncrement)
                            RowTop.Add(Row);
                    }
                    else
                        // Code to get the row index when the selection is other than Table.
                        for (int Top = Activerange.Top; Top <= Activerange.Bottom; Top = (IsNestedCollection == true) ? Top + DoubleIncrement : Top + SingleIncrement)
                            RowTop.Add(Top);

                    RangeTop = RowTop;
                }

                else
                {
                    List<int> RowTop = new List<int>();

                    var RowRanges = tableProperties.Model.SelectedRanges;
                    for (int index = 0; index < RowRanges.Count; index++)
                        RowTop.Add(RowRanges[index].Top);

                    RangeTop = RowTop;
                }

                if (!tableProperties.Model.Table.HasGroups)
                    for (int row = 0; row < RangeTop.Count; row++)
                        RecordsToDelete.Add(tableProperties.Model.Table.GetRecordFromRow(RangeTop[row]));

                else
                {
                    for (int row = 0; row < RangeTop.Count; row++)
                    {
                        RecordIndex = tableProperties.Model.ResolveIndexToGroupPosition(RangeTop[row]);
                        var recordEntry = tableProperties.Model.View.TopLevelGroup.DisplayElements[RecordIndex] as RecordEntry;
                        RecordsToDelete.Add(currentRecord = recordEntry != null ? recordEntry.Data : null);
                    }
                }

                //loop to itearete the selected records one by one to delete
                for (int index = 0; index < RangeTop.Count; index++)
                {
                    currentRecord = RecordsToDelete[index];
                    
                    //This Condition Check is Added to prevent the NotSupportDeletion Message While Deleting GroupCaption.
                    if (currentRecord != null)
                         tableProperties.Model.View.Remove(currentRecord);
                    
                    tableProperties.Model.RefreshDisplay(true);
                    tableProperties.Model.InvalidateDisplay();
                    tableProperties.Model.Table.RaiseGridRecordDeleted(currentRecord, RangeTop[index]);
                    tableProperties.Model.Grid.InvalidateMeasure();
                }
            }
            else
            {

                if (tableProperties.Model.Table.HasGroups)
                {
                    int position = contextMenuInfoArgs.Cell.RowIndex;
                    position = tableProperties.Model.ResolveIndexToRecordPosition(contextMenuInfoArgs.Cell.RowIndex);
                    var topLevelGroup = tableProperties.Model.View.TopLevelGroup;
                    var recordEntry = topLevelGroup.DisplayElements[position] as RecordEntry;
                    currentRecord = recordEntry != null ? recordEntry.Data : null;
                    removedRecordIndex = position;
                }
                else
                {
                    var rowIndex = tableProperties.Model.ResolveIndexToRecordPosition(contextMenuInfoArgs.Cell.RowIndex);
                    var recordEntry = tableProperties.Model.View.Records[rowIndex] as RecordEntry;
                    currentRecord = recordEntry.Data;
                    removedRecordIndex = contextMenuInfoArgs.Cell.RowIndex;
                }
                tableProperties.Model.View.Remove(currentRecord);
                tableProperties.Model.RefreshDisplay(true);
                tableProperties.Model.Grid.InvalidateCells();
                tableProperties.Model.Table.RaiseGridRecordDeleted(currentRecord, removedRecordIndex);
                tableProperties.Model.Grid.InvalidateMeasure();
            }
        }

        private static void ExpandGroupMethod(GridDataTableProperties tableProperties)
        {
            GridQueryContextMenuInfoEventArgs contextMenuInfoArgs = tableProperties.Model.ContextMenuEventArgs as GridQueryContextMenuInfoEventArgs;
            var rowIndex = tableProperties.Model.ResolveIndexToGroupPosition(contextMenuInfoArgs.Cell.RowIndex);
            var group = tableProperties.Model.Table.GroupModel.DisplayElements[rowIndex];
            var groupKey = group as Group;
            if (!groupKey.IsExpanded)
            {
                tableProperties.Model.Table.ExpandGroup(groupKey);
            }                  
        }

        private static void CollapseGroupMethod(GridDataTableProperties tableProperties)
        {
            GridQueryContextMenuInfoEventArgs contextMenuInfoArgs = tableProperties.Model.ContextMenuEventArgs as GridQueryContextMenuInfoEventArgs;
            var rowIndex = tableProperties.Model.ResolveIndexToGroupPosition(contextMenuInfoArgs.Cell.RowIndex);
            var group = tableProperties.Model.Table.GroupModel.DisplayElements[rowIndex];
            var groupKey = group as Group;
            if (groupKey.IsExpanded)
            {
                tableProperties.Model.Table.CollapseGroup(groupKey);
            }            
        }

        private static void SortAscendingMethod(GridDataTableProperties tableProperties)
        {
            GridQueryContextMenuInfoEventArgs contextMenuInfoArgs = tableProperties.Model.ContextMenuEventArgs as GridQueryContextMenuInfoEventArgs;
            var style = new GridDataStyleInfo(contextMenuInfoArgs.Style.CellIdentity);
            var tableStyleInfoIdentity = style.CellIdentity;
            var visibleColumn = tableStyleInfoIdentity.Column;
            var sortColumn = new GridDataSortColumn()
            {
                ColumnName = visibleColumn.MappingName,
                SortDirection = ListSortDirection.Descending
            };

            
                        
            if (tableProperties.SortColumns.Count != 0)
            {
                foreach (var sortCol in tableProperties.SortColumns)
                {
                    if (sortCol.ColumnName == visibleColumn.MappingName)
                    {
                        tableProperties.SortColumns.Remove(sortCol);
                        tableProperties.Model.Table.RaiseSortColumnsChanged(null, new List<GridDataSortColumn>() { sortCol }, NotifyCollectionChangedAction.Remove);
                        break;
                    }
                }
            }

            if (tableProperties.Model.Table.RaiseSortColumnsChanging(new List<GridDataSortColumn>() { sortColumn }, null, NotifyCollectionChangedAction.Add))
            {
                tableProperties.SortColumns.Add(sortColumn);
                tableProperties.Model.Table.RaiseSortColumnsChanged(new List<GridDataSortColumn>() { sortColumn }, null, NotifyCollectionChangedAction.Add);
            }


            tableProperties.Model.SortColumn(visibleColumn);
            tableProperties.Model.InvalidateCell(new RowColumnIndex(contextMenuInfoArgs.Cell.RowIndex, contextMenuInfoArgs.Cell.ColumnIndex));
        }

        private static void SortDescendingMethod(GridDataTableProperties tableProperties)
        {
            GridQueryContextMenuInfoEventArgs contextMenuInfoArgs = tableProperties.Model.ContextMenuEventArgs as GridQueryContextMenuInfoEventArgs;
            var style = new GridDataStyleInfo(contextMenuInfoArgs.Style.CellIdentity);
            var tableStyleInfoIdentity = style.CellIdentity;
            var visibleColumn = tableStyleInfoIdentity.Column;
            var sortColumn = new GridDataSortColumn()
            {
                ColumnName = visibleColumn.MappingName,
                SortDirection = ListSortDirection.Ascending
            };

            if (tableProperties.SortColumns.Count != 0)
            {
                foreach (var sortCol in tableProperties.SortColumns)
                {
                    if (sortCol.ColumnName == visibleColumn.MappingName)
                    {
                        tableProperties.SortColumns.Remove(sortCol);
                        tableProperties.Model.Table.RaiseSortColumnsChanged(null, new List<GridDataSortColumn>() { sortCol }, NotifyCollectionChangedAction.Remove);
                        break;
                    }
                }
            }

            if (visibleColumn.AllowSort)
            {
                if (tableProperties.Model.Table.RaiseSortColumnsChanging(new List<GridDataSortColumn>() { sortColumn }, null, NotifyCollectionChangedAction.Add))
                {
                    tableProperties.SortColumns.Add(sortColumn);
                    tableProperties.Model.Table.RaiseSortColumnsChanged(new List<GridDataSortColumn>() { sortColumn }, null, NotifyCollectionChangedAction.Add);
                }

                tableProperties.Model.SortColumn(visibleColumn);
            }

            tableProperties.Model.InvalidateCell(new RowColumnIndex(contextMenuInfoArgs.Cell.RowIndex, contextMenuInfoArgs.Cell.ColumnIndex));
        }

        private static void ClearSortMethod(GridDataTableProperties tableProperties)
        {
            GridQueryContextMenuInfoEventArgs contextMenuInfoArgs = tableProperties.Model.ContextMenuEventArgs as GridQueryContextMenuInfoEventArgs;
            var style = new GridDataStyleInfo(contextMenuInfoArgs.Style.CellIdentity);
            var tableStyleInfoIdentity = style.CellIdentity;
            var visibleColumn = tableStyleInfoIdentity.Column;
           
            foreach (GridDataSortColumn column in tableProperties.SortColumns)
            {
                if (column.ColumnName == visibleColumn.MappingName )
                {
                    tableProperties.SortColumns.Remove(column);   
                    break;
                }
            }
            // below code was added to update the selection while clearing the Sorting in Contextmenu.
            if (tableProperties.Model is GridDataTableModel)
                tableProperties.Model.UpdateSelectedRanges();

            tableProperties.Model.InvalidateCell(new RowColumnIndex(contextMenuInfoArgs.Cell.RowIndex, contextMenuInfoArgs.Cell.ColumnIndex));            
        }

        private static void ClearFilterMethod(GridDataTableProperties tableProperties)
        {
            GridQueryContextMenuInfoEventArgs contextMenuInfoArgs = tableProperties.Model.ContextMenuEventArgs as GridQueryContextMenuInfoEventArgs;

            if (contextMenuInfoArgs.Style.ColumnIndex > 0 && contextMenuInfoArgs.Style.ColumnIndex < tableProperties.VisibleColumns.Count)
            {
                tableProperties.Model.View.BeginInit();
                tableProperties.VisibleColumns[contextMenuInfoArgs.Style.ColumnIndex].Filters.Clear();
                /// Following codes are comments to avoid clearing the filters applied in other columns.
                //tableProperties.Model.View.FilterPredicates.Clear();
                //tableProperties.Model.View.Filter = null;
                tableProperties.Model.View.EndInit();
            }
        }

        private static void HideColumnMethod(GridDataTableProperties tableProperties)
        {
            GridQueryContextMenuInfoEventArgs contextMenuInfoArgs = tableProperties.Model.ContextMenuEventArgs as GridQueryContextMenuInfoEventArgs;
            var style = new GridDataStyleInfo(contextMenuInfoArgs.Style.CellIdentity);
            var tableStyleInfoIdentity = style.CellIdentity;
            var visibleColumn = tableStyleInfoIdentity.Column;
            visibleColumn.IsHidden = true;
            //Following codes are removed because if we hide the column from the context menu means Grouping should'nt remove.
            //tableProperties.VisibleColumns.Remove(visibleColumn);
            //if (tableProperties.Model.Table.HasGroups)
            //{
            //    foreach (var groupCol in tableProperties.GroupedColumns)
            //        if (groupCol.ColumnName == visibleColumn.MappingName)
            //        {
            //            tableProperties.GroupedColumns.Remove(groupCol);
            //            break;
            //        }
            //}
            //Till This
            tableProperties.Model.View.Refresh();    
        }
    }

#endif
}