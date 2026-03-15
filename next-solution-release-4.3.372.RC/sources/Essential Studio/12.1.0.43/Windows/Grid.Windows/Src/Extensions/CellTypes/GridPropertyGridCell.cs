//-------------------------------------------------------------------------------------------------
// <copyright file="GridPropertyGridCell.cs" company="syncfusion">
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
using System.Globalization;
using System.Runtime.Serialization;
using System.Text;
using System.Windows.Forms;

using System.ComponentModel.Design;
using System.Drawing.Design;
using System.Windows.Forms.Design;
using System.Reflection;

using System.Runtime.InteropServices;
using System.Security;

using Syncfusion.Diagnostics;
using Syncfusion.Windows.Forms;
using Syncfusion.Windows.Forms.Grid;

namespace Syncfusion.Windows.Forms.Grid
{
    /// <summary>
    /// Defines the model / data part of a dropdown with an embedded <see cref="PropertyGrid"/>.
    /// </summary>
    /// <remarks>
    /// You typically access cell models through the <see cref="GridModel.CellModels"/>
    /// property of the <see cref="GridModel"/> class.<para/>
    /// A <see cref="GridPropertyGridCellModel"/> can serve as model for several <see cref="GridPropertyGridCellRenderer"/>
    /// instances if there are several <see cref="GridControlBase"/> views for a <see cref="GridModel"/>.
    /// <para/>
    /// See <see cref="GridPropertyGridCellRenderer"/> for more detailed information about this cell type.
    /// </remarks>
    [Serializable]
    public class GridPropertyGridCellModel : GridDropDownCellModel
    {
        /// <overload>
        /// Initializes a new <see cref="GridPropertyGridCellModel"/> object.
        /// </overload>
        /// <summary>
        /// Initializes a new <see cref="GridPropertyGridCellModel"/> object 
        /// and stores a reference to the <see cref="GridModel"/> this cell belongs to.
        /// </summary>
        /// <param name="grid">The <see cref="GridModel"/> for this cell model.</param>    
        /// <remarks>
        /// You typically access cell models through the <see cref="GridModel.CellModels"/>
        /// property of the <see cref="GridModel"/> class.
        /// </remarks>
        public GridPropertyGridCellModel(GridModel grid)
            : base(grid)
        {
            AllowFloating = false;
            ButtonBarSize = new Size(SystemInformation.VerticalScrollBarWidth, 0);
            SupportsChoiceList = false;
        }

        /// <summary>
        /// Initializes a new <see cref="GridPropertyGridCellModel"/> from a serialization stream.
        /// </summary>
        /// <param name="info">An object that holds all the data needed to serialize or deserialize this instance.</param>
        /// <param name="context">Describes the source and destination of the serialized stream specified by info. </param>
        protected GridPropertyGridCellModel(SerializationInfo info, StreamingContext context)
            : base(info, context)
        {
            ////TraceUtil.TraceCurrentMethodInfoIf(Switches.Serialization.TraceVerbose, info.FullTypeName, info.MemberCount);
        }

        /// <override/>
        /// <summary>Creates a renderer for this cell model.</summary>
        /// <param name="control">The grid control.</param>
        /// <returns>Cell renderer.</returns>
        public override GridCellRendererBase CreateRenderer(GridControlBase control)
        {
            return new GridPropertyGridCellRenderer(control, this);
        }
    }

    /// <summary>
    /// Defines the renderer part of a dropdown with an embedded <see cref="PropertyGrid"/> that lets users modify
    /// the properties of a complex cell value with nested properties.
    /// </summary>
    /// <remarks>
    /// A renderer is created for each <see cref="GridCellModelBase"/>
    /// and <see cref="GridControlBase"/>. There can be several renderers
    /// associated with one <see cref="GridPropertyGridCellModel"/> if several views display the same
    /// <see cref="GridModel"/>.
    /// </remarks>
    public class GridPropertyGridCellRenderer : GridDropDownCellRenderer
    {
        GridStyleInfo currentStyle;
        PropertyDescriptor currentPropertyDescriptor;
        PropertyGrid propertyGrid;
        Size savedSize = new Size(300, 300);
        object oldControlValue;
        object currentValue;

        /// <summary>
        /// Initializes a new GridPropertyGridCellRenderer object for the given GridControlBase
        /// and GridCellModelBase.
        /// </summary>
        /// <param name="grid">The <see cref="GridControlBase"/> that displays this cell renderer.</param>
        /// <param name="cellModel">The <see cref="GridCellModelBase"/> that holds data for this cell renderer that should
        /// be shared among views.</param>
        /// <remarks>References to GridControlBase,
        /// and GridCellModelBase will be saved.</remarks>
        public GridPropertyGridCellRenderer(GridControlBase grid, GridCellModelBase cellModel)
            : base(grid, cellModel)
        {
            DropDownImp.InitFocusEditPart = false;
            DropDownButton = new GridCellComboBoxButton(this);
        }

        /// <override/>
        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                if (propertyGrid != null)
                {
                    propertyGrid.SelectedGridItemChanged -= new SelectedGridItemChangedEventHandler(propertyGrid_SelectedGridItemChanged);
                    propertyGrid.Dispose();
                    propertyGrid = null;
                }

                oldControlValue = null;
                currentPropertyDescriptor = null;
                currentStyle = null;
                currentValue = null;
            }

            base.Dispose(disposing);
        }

        /// <override/>
        protected override void OnInitialize(int rowIndex, int colIndex)
        {
            this.currentStyle = CurrentStyle;
            TypeConverter currentTypeConverter = Model.GetTypeConverter(this.currentStyle);
            currentPropertyDescriptor = Model.GetPropertyDescriptor(currentStyle);

            this.DisableTextBox = currentTypeConverter == null
                || !currentTypeConverter.CanConvertFrom(typeof(string));

            //// No need to worry about size here, just for anchoring purposes.
            //// AutoSizeHeightOfGrid will be called afterward.

            if (propertyGrid == null)
            {
                GridDropDownContainer dropDownContainer = DropDownContainer;
                dropDownContainer.CausesValidation = false;

                dropDownContainer.Bounds = new Rectangle(-10000, -1000, 5, 5);
                dropDownContainer.BorderStyle = BorderStyle.None;

                dropDownContainer.Controls.Clear();
                propertyGrid = new PropertyGrid();
                propertyGrid.Dock = DockStyle.Fill;
                dropDownContainer.Controls.Add(propertyGrid);
                dropDownContainer.Dock = DockStyle.Fill;
                propertyGrid.SelectedGridItemChanged += new SelectedGridItemChangedEventHandler(propertyGrid_SelectedGridItemChanged);
            }

            base.OnInitialize(rowIndex, colIndex);
        }

        /// <override/>
        /// <summary>Occurs when the drop down is about to be shown.</summary>
        /// <param name="sender">Event source.</param>
        /// <param name="e">Event data.</param>
        public override void DropDownContainerShowingDropDown(object sender, CancelEventArgs e)
        {
            DropDownContainer.PopupHost.FormBorderStyle = FormBorderStyle.SizableToolWindow;
            DropDownContainer.PopupHost.ClientSize = savedSize;

            this.propertyGrid.SelectedObject = currentValue;
        }

        /// <override/>
        /// <summary>Occurs after the popup child was dropped down and made visible.</summary>
        /// <param name="sender">Event source.</param>
        /// <param name="e">Event data.</param>
        public override void DropDownContainerShowedDropDown(object sender, EventArgs e)
        {
            base.DropDownContainerShowedDropDown(sender, e);
            propertyGrid.Focus();
        }

        /// <override/>
        /// <summary>Called to indicate that the popup child was closed.</summary>
        /// <param name="sender">Source of the event.</param>
        /// <param name="e">Event data.</param>
        public override void DropDownContainerCloseDropDown(object sender, PopupClosedEventArgs e)
        {
            if (IsChanged() && NotifyCurrentCellChanging())
            {
                this.ControlValue = this.currentValue;
                NotifyCurrentCellChanged();
            }

            this.propertyGrid.SelectedObject = null;
            base.DropDownContainerCloseDropDown(sender, e);
        }

        /// <summary>
        /// This method is called from GridCurrentCell.ConfirmChanges when the current cell
        /// was marked as modified. Any drop-downs have been closed at this time. It saves changes for the current cell.
        /// </summary>
        /// <returns>
        /// True if changes were saved successfully; False if no changes were saved.
        /// </returns>
        /// <override/>
        protected override bool OnSaveChanges()
        {
            if (CurrentCell.IsModified)
            {
                // Save Control Value directly - do not call base (which ends up calling style.FormattedText = this.TextBoxText;)
                GridStyleInfo style = Grid.Model[this.RowIndex, this.ColIndex];
                if (!this.DisableTextBox && !this.HasControlValue && this.HasControlText)
                {
                    style.ApplyFormattedText(ControlText);
                }
                else
                {
                    style.CellValue = this.ControlValue;
                }

                return true;
            }

            return false;
        }

        /// <summary>
        /// Occurs before the current cell switches into editing mode.
        /// </summary>
        /// <returns>
        /// True if editing the cell is allowed; false otherwise.
        /// </returns>
        /// <override/>
        protected override bool OnStartEditing()
        {
            currentValue = currentStyle.CellValue;

            if (currentValue is ICloneable && (currentPropertyDescriptor == null || !this.currentPropertyDescriptor.IsReadOnly))
            {
                oldControlValue = currentValue;
                currentValue = ((ICloneable)currentValue).Clone();
            }

            if (currentValue == null)
            {
                currentValue = Activator.CreateInstance(currentStyle.CellValueType);
                oldControlValue = Activator.CreateInstance(currentStyle.CellValueType);
            }

            return base.OnStartEditing();
        }

        /// <override/>
        protected override void OnEditingComplete()
        {
            currentValue = null;
            base.OnEditingComplete();
        }
        
        /// <override/>
        protected override void OnEndEdit()
        {
            if (this.currentValue != null)
            {
                CurrentCell.CloseDropDown(PopupCloseType.Deactivated);
            }

            base.OnEndEdit();
        }

        /// <summary>
        /// Called from GridCurrentCell.Deactivate after GridCurrentCell.Deactivating event
        /// and before the current cell is deactivated.
        /// </summary>
        /// <returns>
        /// True if renderer can be deactivated; False if deactivation should be aborted.
        /// </returns>
        /// <override/>
        protected override bool OnDeactivating()
        {
            if (this.currentValue != null)
            {
                CurrentCell.CloseDropDown(PopupCloseType.Deactivated);
            }

            return base.OnDeactivating();
        }

        /// <override/>
        protected override void OnDeactived(int rowIndex, int colIndex)
        {
            propertyGrid.SelectedGridItemChanged -= new SelectedGridItemChangedEventHandler(propertyGrid_SelectedGridItemChanged);

            this.oldControlValue = null;
            this.currentValue = null;
            this.TextBoxText = string.Empty;

            base.OnDeactived(rowIndex, colIndex);
        }

        /// <override/>
        /// <summary>Checks if the given text is valid.</summary>
        /// <param name="text">Input text.</param>
        /// <returns>returns True.</returns>
        public override bool ValidateString(string text)
        {
            return true;
        }

        /// <summary>
        /// This method is called from GridCurrentCell.Validate after GridCurrentCell.Validating event has been
        /// fired. The default version checks if the active text fits any criteria as specified
        /// in the style object: It can be parsed into a cell value and meets GridCellValidateValueInfo criteria.
        /// </summary>
        /// <returns>
        /// True if the modified text is valid;
        /// </returns>
        /// <override/>
        protected override bool OnValidate()
        {
            return true;
        }

        /// <override/>
        protected override void OnMouseDown(int rowIndex, int colIndex, MouseEventArgs e)
        {
            if (this.currentValue != null)
            {
                CurrentCell.CloseDropDown(PopupCloseType.Deactivated);
            }

            base.OnMouseDown(rowIndex, colIndex, e);
        }

        /// <override/>
        /// <summary>
        /// Indicates whether a specified control is part of the popup hierarchy.
        /// </summary>
        /// <param name="control">A control instance.</param>
        /// <param name="askPopupParent">True if the query should be passed to the popup parent; False if you should not query the popup parent.</param>
        /// <returns>returns True.</returns>
        public override bool IsRelatedControl(Control control, bool askPopupParent)
        {
            this.savedSize = this.DropDownContainer.PopupHost.Size;
            return true;
        }

        bool IsChanged()
        {
            if (CurrentCell.IsModified)
            {
                return true;
            }

            bool changed = true;
            if (currentValue is ICloneable && (currentPropertyDescriptor == null || !this.currentPropertyDescriptor.IsReadOnly))
            {
                changed = !currentValue.Equals(oldControlValue);
            }

            return changed;
        }

        private void propertyGrid_SelectedGridItemChanged(object sender, SelectedGridItemChangedEventArgs e)
        {
            if (IsChanged())
            {
                this.NotifyCurrentCellChanged();
            }
        }
    }
}
