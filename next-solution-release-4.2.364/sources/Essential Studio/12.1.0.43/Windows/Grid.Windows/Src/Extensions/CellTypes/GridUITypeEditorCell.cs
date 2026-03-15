//-------------------------------------------------------------------------------------------------
// <copyright file="GridUITypeEditorCell.cs" company="syncfusion">
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

using Syncfusion.ComponentModel;
using Syncfusion.Diagnostics;
using Syncfusion.Windows.Forms;

namespace Syncfusion.Windows.Forms.Grid
{
    /// <summary>
    /// Defines the model / data part of a drop-down with a ListControl-like grid or a UITypeEditor.
    /// </summary>
    /// <remarks>
    /// You typically access cell models through the <see cref="GridModel.CellModels"/>
    /// property of the <see cref="GridModel"/> class.<para/>
    /// A <see cref="GridUITypeEditorCellModel"/> can serve as model for several <see cref="GridUITypeEditorCellRenderer"/>
    /// instances if there are several <see cref="GridControlBase"/> views for a <see cref="GridModel"/>.
    /// <para/>
    /// See <see cref="GridUITypeEditorCellRenderer"/> for more detailed information about this cell type.
    /// </remarks>
    [Serializable]
    public class GridUITypeEditorCellModel : GridDropDownStandardValuesCellModel
    {
        /// <overload>
        /// Initializes a new <see cref="GridUITypeEditorCellModel"/> object.
        /// </overload>
        /// <summary>
        /// Initializes a new <see cref="GridUITypeEditorCellModel"/> object
        /// and stores a reference to the <see cref="GridModel"/> this cell belongs to.
        /// </summary>
        /// <param name="grid">The <see cref="GridModel"/> for this cell model.</param>
        /// <remarks>
        /// You typically access cell models through the <see cref="GridModel.CellModels"/>
        /// property of the <see cref="GridModel"/> class.
        /// </remarks>
        public GridUITypeEditorCellModel(GridModel grid)
            : base(grid)
        {
        }

        /// <summary>
        /// Initializes a new <see cref="GridUITypeEditorCellModel"/> from a serialization stream.
        /// </summary>
        /// <param name="info">An object that holds all the data needed to serialize or deserialize this instance.</param>
        /// <param name="context">Describes the source and destination of the serialized stream specified by info. </param>
        protected GridUITypeEditorCellModel(SerializationInfo info, StreamingContext context)
            : base(info, context)
        {
            ////TraceUtil.TraceCurrentMethodInfoIf(Switches.Serialization.TraceVerbose, info.FullTypeName, info.MemberCount);
        }

        /// <override/>
        /// <summary>
        /// Creates a <see cref="GridDropDownStandardValuesCellRenderer"/> for this cell model that is specific to the GridControlBase.
        /// </summary>
        /// <param name="control">The grid control for which the cell renderer is created for.</param>
        /// <returns>A new <see cref="GridUITypeEditorCellRenderer"/> specific for the specified grid.</returns>
        public override GridCellRendererBase CreateRenderer(GridControlBase control)
        {
            return new GridUITypeEditorCellRenderer(control, this);
        }

        /// <summary>
        /// Determines the <see cref="UITypeEditor"/> that should be displayed in
        /// the dropdown part. If it is null the standard values collection will
        /// be displayed in a dropdown grid.
        /// </summary>
        /// <param name="style">The style object</param>
        /// <returns>A <see cref="UITypeEditor"/></returns>
        public virtual UITypeEditor GetUITypeEditor(GridStyleInfo style)
        {
            PropertyDescriptor pd = this.GetPropertyDescriptor(style);

            if (pd != null)
            {
                return (UITypeEditor)pd.GetEditor(typeof(UITypeEditor));
            }

            Type type = style.CellValueType;
            if (type != null)
            {
                return (UITypeEditor)TypeDescriptor.GetEditor(type, typeof(UITypeEditor));
            }

            return null;
        }
    }

    /// <summary>
    /// Defines the renderer part of a drop-down with a ListControl-like grid or a UITypeEditor
    /// that display choices for a cell determined through the <see cref="TypeConverter.GetStandardValues()"/>
    /// method of a <see cref="TypeConverter"/>.
    /// </summary>
    /// <remarks>
    /// The <see cref="GridUITypeEditorCellRenderer"/> supports an autocomplete feature that
    /// will fill the text with possible matches from the drop-down list while the user is entering text.
    /// <para/>
    /// A renderer is created for each <see cref="GridCellModelBase"/>
    /// and <see cref="GridControlBase"/>. There can be several renderers
    /// associated with one <see cref="GridUITypeEditorCellModel"/> if several views display the same
    /// <see cref="GridModel"/>.
    /// </remarks>
    public class GridUITypeEditorCellRenderer : GridDropDownStandardValuesCellRenderer, IWindowsFormsEditorService, IServiceProvider
    {
        Control currentControl;
        GridStyleInfo currentStyle;
        UITypeEditor currentEditor;
        PropertyDescriptor currentPropertyDescriptor;
        bool closeDropDownCalled = false;
        bool unexpectedDropDown = false;
        object currentInstance = null;

        /// <summary>
        /// Initializes a new GridUITypeEditorCellRenderer object for the given GridControlBase
        /// and GridCellModelBase.
        /// </summary>
        /// <param name="grid">The <see cref="GridControlBase"/> that displays this cell renderer.</param>
        /// <param name="cellModel">The <see cref="GridCellModelBase"/> that holds data for this cell renderer that should
        /// be shared among views.</param>
        /// <remarks>References to GridControlBase,
        /// and GridCellModelBase will be saved.</remarks>
        public GridUITypeEditorCellRenderer(GridControlBase grid, GridCellModelBase cellModel)
            : base(grid, cellModel)
        {
            DropDownImp.InitFocusEditPart = false;
        }

        /// <override/>
        /// <summary>
        /// Gets the data model for this renderer.
        /// </summary>
        public new GridUITypeEditorCellModel Model
        {
            get
            {
                return (GridUITypeEditorCellModel)base.Model;
            }
        }

        /// <override/>
        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
            }

            base.Dispose(disposing);
        }

        /// <internalonly/>
        /// <summary>Gets or sets CurrentInstance. Used internally.</summary>
        [Syncfusion.Documentation.DocumentationExclude()]
        public object CurrentInstance
        {
            get
            {
                return currentInstance;
            }

            set
            {
                currentInstance = value;
            }
        }

        /// <override/>
        protected override void OnInitialize(int rowIndex, int colIndex)
        {
            this.currentStyle = StyleInfo;
            currentPropertyDescriptor = Model.GetPropertyDescriptor(currentStyle);
            UITypeEditor editor = Model.GetUITypeEditor(currentStyle);

            base.OnInitialize(rowIndex, colIndex);

            if (currentEditor != null && editor != null && currentEditor.GetType() == editor.GetType())
            {
                return;
            }

            GridDropDownContainer dropDownContainer = DropDownContainer;

            if (editor != null && editor.GetEditStyle() == UITypeEditorEditStyle.DropDown)
            {
                // No need to worry about size here, just for anchoring purposes.
                // AutoSizeHeightOfGrid will be called afterward.
                dropDownContainer.CausesValidation = false;

                dropDownContainer.Controls.Clear();
                dropDownContainer.Bounds = new Rectangle(-10000, -1000, 5, 5);
                dropDownContainer.BorderStyle = BorderStyle.None;
            }
            else
            {
                dropDownContainer.Controls.Clear();
                dropDownContainer.Controls.Add(this.ListControlPart);
                dropDownContainer.BorderStyle = BorderStyle.None;
            }

            currentEditor = editor;
        }

        /// <override/>
        protected override void OnShowDropDown()
        {
            if (currentEditor == null || currentEditor.GetEditStyle() == UITypeEditorEditStyle.None)
            {
                base.OnShowDropDown(); // standard dropdown list.
                return;
            }
            // The grid check for IsDisposed when processing MouseUp since
            // an Application.Exit can occur while Application.DoEvents is called. It should
            // be fine to do the drop-down at this time here.

            // However, to also avoid trouble in the future, it is best to
            // delay the Application.DoEvents loop a bit until after the
            // MouseUp was completely processed.
            Timer t = new Timer();
            t.Interval = 20;

            // If the user hits the dropdown button of another cell while the current cell has the
            // the UITypeEditot show we first want the call to currentEditor.EditValue return
            // flow to the t_tick code that is currently running. Therefore we delay the dropdown a couple milliseconds.
            if (inEditValue)
            {
                t.Interval = 50;
            }

            t.Tick += new EventHandler(t_Tick);
            t.Start();
        }

        bool deactivated = false;
        bool inEditValue = false;

        /// <override/>
        protected override void OnDeactived(int rowIndex, int colIndex)
        {
            deactivated = true;
        }

        private void t_Tick(object sender, EventArgs e)
        {
            Timer t = (Timer)sender;
            t.Tick -= new EventHandler(t_Tick);
            t.Dispose();

            if (Grid == null || Grid.IsDisposed)
            {
                return;
            }

            TextBox.SelectAll();

            unexpectedDropDown = false;

            TypeDescriptorContext tdc = new TypeDescriptorContext(currentInstance, currentPropertyDescriptor);
            tdc.ServiceProvider = this;

            deactivated = false;
            inEditValue = true;
            object value = currentEditor.EditValue(tdc, this, this.ControlValue);
            inEditValue = false;

            //// Because of the specialized DoEvents loop, it can happen
            //// that the whole state of this cell has changed after
            //// EditValue returns. In that case, the method should
            //// return immediately.

            if (!deactivated && !Grid.IsDisposed && currentStyle.Store != null)
            {
                if (this.NotifyCurrentCellChanging())
                {
                    SetControlValue(value, true);
                    ////BaseTextBoxText = Model.GetFormattedText(currentStyle, value, 0);
                    this.NotifyCurrentCellChanged();
                }
            }

            if (!unexpectedDropDown)
            {
                DropDownImp.OnCloseDropDown(PopupCloseType.Deactivated);
            }
        }

        void IWindowsFormsEditorService.DropDownControl(Control control)
        {
            if (!DropDownContainer.Controls.Contains(control))
            {
                this.DropDownContainer.Controls.Add(control);
            }

            control.Visible = true;
            Size size = control.Size;
            if (size.Width == 0 || size.Height == 0)
            {
                if (size.Width == 0)
                {
                    size.Width = this.GetCellLayout(RowIndex, ColIndex, currentStyle).CellRectangle.Width;
                }

                if (size.Height == 0)
                {
                    size.Height = 100;
                }

                control.Size = size;
            }

            this.DropDownContainer.Size = size;
            this.DropDownContainer.BorderStyle = BorderStyle.FixedSingle;

            this.currentControl = control;

            DropDownImp.OnShowDropDown();

            closeDropDownCalled = false;

            // Loop until we got the ok from the UI Type Editor that it is done or until Grid is hidden.
            while (!closeDropDownCalled && currentControl.Visible && Grid.Visible)
            {
                Application.DoEvents();
            }
        }

        void IWindowsFormsEditorService.CloseDropDown()
        {
            closeDropDownCalled = true;
        }

        System.Windows.Forms.DialogResult IWindowsFormsEditorService.ShowDialog(Form dialog)
        {
            DialogResult r = dialog.ShowDialog(this.DisableTextBox ? (Control)Grid : (Control)TextBox);
            closeDropDownCalled = true;
            return r;
        }

        /// <override/>
        /// <summary>Occurs when the drop down is about to be shown.</summary>
        /// <param name="sender">Event source.</param>
        /// <param name="e">Event data.</param>
        public override void DropDownContainerShowingDropDown(object sender, CancelEventArgs e)
        {
            if (currentEditor == null || currentEditor.GetEditStyle() == UITypeEditorEditStyle.None)
            {
                base.DropDownContainerShowingDropDown(sender, e); // standard dropdown list.
                return;
            }

            Size size = this.currentControl.Size;
            size.Width += 2;  // add space for borders.
            size.Height += 2;
            DropDownContainer.Size = size;
            DropDownContainer.PopupHost.Size = size;
        }

        /// <override/>
        /// <summary>
        /// Indicates that the popup child was closed.
        /// </summary>
        /// <param name="sender">Event source.</param>
        /// <param name="e">Event data.</param>
        public override void DropDownContainerCloseDropDown(object sender, PopupClosedEventArgs e)
        {
            if (currentEditor == null || currentEditor.GetEditStyle() == UITypeEditorEditStyle.None)
            {
                base.DropDownContainerCloseDropDown(sender, e); // standard dropdown list.
                return;
            }

            if (!closeDropDownCalled)
            {
                unexpectedDropDown = true;
                closeDropDownCalled = true;
            }
        }

        /// <override/>
        /// <summary>Occurs after the popup child was dropped down and made visible.</summary>
        /// <param name="sender">Event source.</param>
        /// <param name="e">Event data.</param>
        public override void DropDownContainerShowedDropDown(object sender, EventArgs e)
        {
            if (currentEditor == null || currentEditor.GetEditStyle() == UITypeEditorEditStyle.None)
            {
                base.DropDownContainerShowedDropDown(sender, e); // standard dropdown list.
                return;
            }

            //// A bit hack, but simply calling
            currentControl.Focus();
            ////DropDownContainer.PopupHost.ActiveControl = currentControl;
            //// did not work. FakeLeftMouseClick will give focus to the UITypeEditor
            ////Syncfusion.Drawing.ActiveXSnapshot.FakeLeftMouseClick(currentControl, new Point(0, 0));
            //// Update : Actually works now because of Timer fix above.
        }

        /// <override/>
        protected override void NotifyCurrentCellChanged()
        {
            base.NotifyCurrentCellChanged();
            if (this.GetPaintValueSupported(currentStyle, currentEditor))
            {
                GridStyleInfo style = CurrentStyle;
                Rectangle r = this.GetCellLayout(RowIndex, ColIndex, style).ClientRectangle;
                r.Width = GetPaintValueWidth() + style.TextMargins.ToMargins().Width;
                Grid.Invalidate(r);
                ////CurrentCell.Invalidate();
            }
        }

        /// <override/>
        /// <summary>Allows custom formatting of a cell by changing its style object.</summary>
        /// <param name="e">Event data.</param>
        public override void OnPrepareViewStyleInfo(GridPrepareViewStyleInfoEventArgs e)
        {
            UITypeEditor editor = Model.GetUITypeEditor(e.Style); 

            //// Provide space for small glyph left of text.
            if (editor != null && GetPaintValueSupported(e.Style, editor))
            {
                //// TODO: What about RTL support?
                e.Style.TextMargins.Left += GetPaintValueWidth() + e.Style.TextMargins.ToMargins().Width;
            }

            ////e.Style.DisplayMember = "Value";
            ////e.Style.ValueMember = "";

            base.OnPrepareViewStyleInfo(e);
        }

        /// <summary>
        /// Determines if the UITypeEditor supports painting glyphs before the text.
        /// </summary>
        /// <param name="style">The style of the parent cell.</param>
        /// <param name="editor">The UITypeEditor</param>
        /// <returns>true if glyphs should be painted.</returns>
        protected virtual bool GetPaintValueSupported(GridStyleInfo style, UITypeEditor editor)
        {
            return editor != null && editor.GetPaintValueSupported();
        }

        /// <summary>
        /// Paints a glyph before the text by calling the UITypeEditor.PaintValue method
        /// </summary>   
        protected virtual void PaintValue(Graphics g, Rectangle bounds, int rowIndex, int colIndex, GridStyleInfo style, UITypeEditor editor, object value)
        {
            editor.PaintValue(new PaintValueEventArgs(null, value, g, bounds));
        }
        
        /// <summary>
        /// Returns the width of the glyph
        /// </summary>
        /// <returns>returns 20</returns>
        protected virtual int GetPaintValueWidth()
        {
            return 20;
        }

        /// <summary>
        /// Returns the width of a possible "Glyph" to be drawn before the cell value. This method
        /// is overriden by the UITypeEditorCellRenderer
        /// </summary>
        /// <returns>The width in pixel for the glyph.</returns>
        /// <override/>
        protected override int GetDropDownPaintValueWidth()
        {
            if (currentEditor != null && GetPaintValueSupported(currentStyle, currentEditor))
            {
                return GetPaintValueWidth() + currentStyle.TextMargins.ToMargins().Width;
            }

            return 0;
        }

        /// <override/>
        /// <summary>
        /// Draw the content of specified cell.
        /// </summary>
        /// <param name="g">Graphics context.</param>
        /// <param name="cellRectangle">Cell rectangle.</param>
        /// <param name="rowIndex">Row index.</param>
        /// <param name="colIndex">Column index.</param>
        /// <param name="style">Cell style information.</param>
        /// <remarks></remarks>
        public override void Draw(Graphics g, Rectangle cellRectangle, int rowIndex, int colIndex, GridStyleInfo style)
        {
            GridCellComboBoxButton button = DropDownImp.DropDownButton as GridCellComboBoxButton;

            UITypeEditor editor = Model.GetUITypeEditor(style);
            if (button != null && editor != null && editor.GetEditStyle() == UITypeEditorEditStyle.Modal)
            {
                button.DrawEllipsis = true;
            }

            base.Draw(g, cellRectangle, rowIndex, colIndex, style);

            if (button != null)           
            { 
                button.DrawEllipsis = false; 
            }
        }
        
        /// <override/>
        protected override void OnDraw(Graphics g, Rectangle clientRectangle, int rowIndex, int colIndex, GridStyleInfo style)
        {
            base.OnDraw(g, clientRectangle, rowIndex, colIndex, style);

            UITypeEditor editor = Model.GetUITypeEditor(style);
            if (editor != null && GetPaintValueSupported(style, editor))
            {
                Rectangle bounds = this.GetCellLayout(rowIndex, colIndex, style).TextRectangle;
                int w = GetPaintValueWidth();
                bounds.X -= w;
                bounds.Width = w;
                bounds.Intersect(clientRectangle);
                object value = style.CellValue;
                if (this.ShouldDrawEditing(rowIndex, colIndex))
                {
                    value = this.ControlValue;
                }

                if (value != null && value.GetType() == style.CellValueType)
                {
                    GridBorderPaint.DrawRectangle(g, new GridBorder(GridBorderStyle.Solid), bounds, SystemColors.ControlLight, GridBorderSide.All);
                    bounds.Inflate(-1, -1);
                    PaintValue(g, bounds, rowIndex, colIndex, style, editor, value);
                }
            }
        }

        /// <override/>
        protected override void ListControlGridPrepareViewStyleInfo(object sender, GridPrepareViewStyleInfoEventArgs e)
        {
            // Provide space for small glyph left of text.
            if (currentEditor != null && GetPaintValueSupported(currentStyle, currentEditor))
            {
                // TODO: What about RTL support?
                e.Style.TextMargins.Left += GetPaintValueWidth() + e.Style.TextMargins.ToMargins().Width;
            }
        }

        /// <override/>
        protected override void ListControlGridCellDrawn(object sender, GridDrawCellEventArgs e)
        {
            if (currentEditor != null && GetPaintValueSupported(currentStyle, currentEditor))
            {
                GridControlBase grid = (GridControlBase)sender;
                GridCellRendererBase renderer = grid.GetCellRenderer(e.RowIndex, e.ColIndex);
                GridCellLayout layout = renderer.GetCellLayout(e.RowIndex, e.ColIndex, e.Style);
                Rectangle bounds = layout.TextRectangle;
                int w = GetPaintValueWidth();
                bounds.X -= w;
                bounds.Width = w;
                bounds.Intersect(layout.ClientRectangle);
                object value = this.ListControlPart.Items[e.RowIndex - 1];
                if (value != null && value.GetType() == e.Style.CellValueType)
                {
                    GridBorderPaint.DrawRectangle(e.Graphics, new GridBorder(GridBorderStyle.Solid), bounds, SystemColors.ControlLight, GridBorderSide.All);
                    bounds.Inflate(-1, -1);
                    PaintValue(e.Graphics, bounds, e.RowIndex, e.ColIndex, e.Style, currentEditor, value);
                }
            }
        }

        object IServiceProvider.GetService(Type classService)
        {
            if (classService == typeof(System.Windows.Forms.Design.IWindowsFormsEditorService))
            {
                return this;
            }

            return null;
        }
    }
}
