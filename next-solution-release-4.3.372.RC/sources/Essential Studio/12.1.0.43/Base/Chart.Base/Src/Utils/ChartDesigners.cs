#region Copyright Syncfusion Inc. 2001 - 2014
// Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Any infringement will be prosecuted under
// applicable laws. 
#endregion


#region File using derectives

using System;
using System.Collections;
using System.ComponentModel;
using System.ComponentModel.Design;
using System.Drawing;
using System.Drawing.Design;
using System.Drawing.Drawing2D;
using System.Resources;
using System.Text;
using System.Windows.Forms;
using System.Windows.Forms.Design;
using Syncfusion.Documentation;
using Syncfusion.Drawing;

#endregion

namespace Syncfusion.Windows.Forms.Chart
{
    /// <summary>
    /// Provides a base class that can be used to design value editors with the drop-down list.
    /// </summary>
    public abstract class ChartDropDownUIEditor : UITypeEditor
    {
        #region Members
        private DropDownListBox m_dropDownListBox = null;
        #endregion

        #region Implementation
        /// <summary>
        /// Gets the values.
        /// </summary>
        /// <returns>Returns IList.</returns>
        protected abstract IList GetValues();

        /// <summary>
        /// Edits the specified object's value using the editor style indicated by the <see cref="M:System.Drawing.Design.UITypeEditor.GetEditStyle"></see> method.
        /// </summary>
        /// <param name="context">An <see cref="T:System.ComponentModel.ITypeDescriptorContext"></see> that can be used to gain additional context information.</param>
        /// <param name="provider">An <see cref="T:System.IServiceProvider"></see> that this editor can use to obtain services.</param>
        /// <param name="value">The object to edit.</param>
        /// <returns>
        /// The new value of the object. If the value of the object has not changed, this should return the same object it was passed.
        /// </returns>
        public override object EditValue(ITypeDescriptorContext context, IServiceProvider provider, object value)
        {
            if (provider != null)
            {
                IWindowsFormsEditorService edSvc = (IWindowsFormsEditorService)provider.GetService(typeof(IWindowsFormsEditorService));

                if (edSvc == null)
                {
                    return value;
                }

                if (m_dropDownListBox == null)
                {
                    m_dropDownListBox = new DropDownListBox(this);
                }

                m_dropDownListBox.Start(edSvc, context, value);

                edSvc.DropDownControl(m_dropDownListBox);
                value = m_dropDownListBox.Value;

                m_dropDownListBox.End();
            }

            return value;
        }

        /// <summary>
        /// Gets the editor style used by the <see cref="M:System.Drawing.Design.UITypeEditor.EditValue(System.IServiceProvider,System.Object)"></see> method.
        /// </summary>
        /// <param name="context">An <see cref="T:System.ComponentModel.ITypeDescriptorContext"></see> that can be used to gain additional context information.</param>
        /// <returns>
        /// A <see cref="T:System.Drawing.Design.UITypeEditorEditStyle"></see> value that indicates the style of editor used by the <see cref="M:System.Drawing.Design.UITypeEditor.EditValue(System.IServiceProvider,System.Object)"></see> method. If the <see cref="T:System.Drawing.Design.UITypeEditor"></see> does not support this method, then <see cref="M:System.Drawing.Design.UITypeEditor.GetEditStyle"></see> will return <see cref="F:System.Drawing.Design.UITypeEditorEditStyle.None"></see>.
        /// </returns>
        public override UITypeEditorEditStyle GetEditStyle(ITypeDescriptorContext context)
        {
            return UITypeEditorEditStyle.DropDown;
        }

        /// <summary>
        /// Gets the item text.
        /// </summary>
        /// <param name="item">The item.</param>
        /// <returns></returns>
        protected virtual string GetItemText(object item)
        {
            return item.ToString();
        }
        #endregion

        #region Class internal declarations
        /// <summary>
        /// Class represent palette drop down user interface.
        /// </summary>
        class DropDownListBox : ListBox
        {
            #region Constants
            private const int c_iconWidth = 16;
            #endregion

            #region Class members
            private IWindowsFormsEditorService m_edSvc = null;
            private ChartDropDownUIEditor m_editor = null;
            private ITypeDescriptorContext m_context = null;

            private object m_value = null;
            private IList m_values = null;
            #endregion

            #region Properties
            /// <summary>
            /// Gets the value.
            /// </summary>
            /// <value>The value.</value>
            public object Value
            {
                get
                {
                    return m_value;
                }
            }
            #endregion

            #region Class initialize methods
            /// <summary>
            /// Initializes a new instance of the <see cref="DropDownListBox"/> class.
            /// </summary>
            /// <param name="editor">The owner.</param>
            public DropDownListBox(ChartDropDownUIEditor editor)
            {
                m_editor = editor;

                this.ItemHeight += 2;
                this.BorderStyle = BorderStyle.None;
                this.DrawMode = DrawMode.OwnerDrawFixed;
            }
            #endregion

            #region Class public methods
            /// <summary>
            /// Initializes the temporary variables.
            /// </summary>
            /// <param name="edSvc">The <see cref="IWindowsFormsEditorService"/>.</param>
            /// <param name="context">The <see cref="ITypeDescriptorContext"/>.</param>
            /// <param name="value">The value.</param>
            public void Start(IWindowsFormsEditorService edSvc, ITypeDescriptorContext context, object value)
            {
                m_edSvc = edSvc;
                m_context = context;

                m_value = value;
                m_values = m_editor.GetValues();

                foreach (object item in m_values)
                {
                    this.Items.Add(m_editor.GetItemText(item));
                }

                this.Height = this.ItemHeight * m_values.Count;
                this.SelectedIndex = m_values.IndexOf(m_value);
            }

            /// <summary>
            /// Removes the temporary variables.
            /// </summary>
            public void End()
            {
                m_edSvc = null;
                m_context = null;
                m_value = null;
                m_values = null;

                this.Items.Clear();
                this.SelectedItem = null;
            }
            #endregion

            #region Class overrides

            /// <summary>
            /// The OnSelectedIndexChanged method.
            /// </summary>
            /// <param name="e">Event object with the details</param>
            protected override void OnSelectedIndexChanged(EventArgs e)
            {
                if (this.SelectedIndex >= 0)
                {
                    m_value = m_values[this.SelectedIndex];
                }
                else
                {
                    m_value = null;
                }

                base.OnSelectedIndexChanged(e);
            }

            /// <summary>
            /// Raises the <see cref="E:System.Windows.Forms.ListBox.DrawItem"></see> event.
            /// </summary>
            /// <param name="e">A <see cref="T:System.Windows.Forms.DrawItemEventArgs"></see> that contains the event data.</param>
            protected override void OnDrawItem(DrawItemEventArgs e)
            {
                e.DrawBackground();

                if (m_editor.GetPaintValueSupported(m_context))
                {
                    m_editor.PaintValue(new PaintValueEventArgs(m_context, m_values[e.Index],
                        e.Graphics, new Rectangle(e.Bounds.X + 2, e.Bounds.Y + 2, c_iconWidth - 1, e.Bounds.Height - 3)));

                    e.Graphics.DrawRectangle(SystemPens.ControlDarkDark, e.Bounds.X + 1, e.Bounds.Y + 1, c_iconWidth, e.Bounds.Height - 2);                        

                    using (SolidBrush brush = new SolidBrush(this.ForeColor))
                    {
                        e.Graphics.DrawString(Items[e.Index].ToString(), e.Font, brush, new Rectangle(e.Bounds.X + c_iconWidth + 2, e.Bounds.Y, e.Bounds.Width, e.Bounds.Height));
                    }
                }
                else
                {
                    using (SolidBrush brush = new SolidBrush(this.ForeColor))
                    {
                        e.Graphics.DrawString(Items[e.Index].ToString(), e.Font, brush, e.Bounds);
                    }
                }

                e.DrawFocusRectangle();
            }

            /// <summary>
            /// Raises the <see cref="E:System.Windows.Forms.ListBox.MeasureItem"></see> event.
            /// </summary>
            /// <param name="e">A <see cref="T:System.Windows.Forms.MeasureItemEventArgs"></see> that contains the event data.</param>
            protected override void OnMeasureItem(MeasureItemEventArgs e)
            {
                base.OnMeasureItem(e);

                if (m_editor.GetPaintValueSupported(m_context))
                {
                    e.ItemWidth += c_iconWidth + 2;
                }
            }

            /// <summary>
            /// Raises the <see cref="E:System.Windows.Forms.Control.MouseUp"/> event.
            /// </summary>
            /// <param name="e">A <see cref="T:System.Windows.Forms.MouseEventArgs"/> that contains the event data.</param>
            protected override void OnMouseUp(MouseEventArgs e)
            {
                base.OnMouseUp(e);
                m_edSvc.CloseDropDown();
            }

            /// <summary>
            /// Raises the <see cref="E:System.Windows.Forms.Control.KeyUp"></see> event.
            /// </summary>
            /// <param name="e">A <see cref="T:System.Windows.Forms.KeyEventArgs"></see> that contains the event data.</param>
            protected override void OnKeyUp(KeyEventArgs e)
            {
                base.OnKeyUp(e);

                if (e.KeyCode == Keys.Enter)
                {
                    m_edSvc.CloseDropDown();
                }
            }
            #endregion
        }
        #endregion
    }

    /// <internalonly/>
    /// <summary>
    /// Custom <see cref="UITypeEditor"/> for arrays of <see cref="Color"/> struct.
    /// </summary>
    [DocumentationExclude()]
    public class ColorsUIEditor : ArrayEditor
    {
        #region Constructors
        /// <summary>
        /// Initializes a new instance of the <see cref="ColorsUIEditor"/> class.
        /// </summary>
        /// <param name="type">The data type of the items in the array.</param>
        public ColorsUIEditor(Type type)
            : base(type)
        {
        }
        #endregion

        #region Imnplementation
        /// <summary>
        /// Paints a representation of the value of an object using the specified <see cref="T:System.Drawing.Design.PaintValueEventArgs"></see>.
        /// </summary>
        /// <param name="e">A <see cref="T:System.Drawing.Design.PaintValueEventArgs"></see> that indicates what to paint and where to paint it.</param>
        public override void PaintValue(PaintValueEventArgs e)
        {
            Color[] colors = e.Value as Color[];

            if (colors != null)
            {
                float xOffset = (float)e.Bounds.Width / colors.Length;

                for (int i = 0; i < colors.Length; i++)
                {
                    using (SolidBrush sb = new SolidBrush(colors[i]))
                    {
                        e.Graphics.FillRectangle(sb, e.Bounds.X + i * xOffset, e.Bounds.Y, xOffset, e.Bounds.Height);
                    }
                }
            }

            base.PaintValue(e);
        }

        /// <summary>
        /// Indicates whether the specified context supports painting a representation of an object's value within the specified context.
        /// </summary>
        /// <param name="context">An <see cref="T:System.ComponentModel.ITypeDescriptorContext"></see> that can be used to gain additional context information.</param>
        /// <returns>
        /// true if <see cref="M:System.Drawing.Design.UITypeEditor.PaintValue(System.Object,System.Drawing.Graphics,System.Drawing.Rectangle)"></see> is implemented; otherwise, false.
        /// </returns>
        public override bool GetPaintValueSupported(ITypeDescriptorContext context)
        {
            return true;
        }
        #endregion
    }

    /// <summary>
    /// Custom <see cref="UITypeEditor"/> for <see cref="ChartSeriesType"/> enumeration.
    /// </summary>
    public class ChartSeriesTypeEditor : UITypeEditor
    {
        #region Public methods
        /// <summary>
        /// Paints a representation of the value of an object using the specified <see cref="T:System.Drawing.Design.PaintValueEventArgs"></see>.
        /// </summary>
        /// <param name="e">A <see cref="T:System.Drawing.Design.PaintValueEventArgs"></see> that indicates what to paint and where to paint it.</param>
        public override void PaintValue(PaintValueEventArgs e)
        {
            e.Graphics.DrawImage(Utils.ChartSeriesTypeImages.GetImage((ChartSeriesType)e.Value), e.Bounds);
            base.PaintValue(e);
        }

        /// <summary>
        /// Indicates whether the specified context supports painting a representation of an object's value within the specified context.
        /// </summary>
        /// <param name="context">An <see cref="T:System.ComponentModel.ITypeDescriptorContext"></see> that can be used to gain additional context information.</param>
        /// <returns>
        /// true if <see cref="M:System.Drawing.Design.UITypeEditor.PaintValue(System.Object,System.Drawing.Graphics,System.Drawing.Rectangle)"></see> is implemented; otherwise, false.
        /// </returns>
        public override bool GetPaintValueSupported(ITypeDescriptorContext context)
        {
            return true;
        }
        #endregion
    }

    /// <summary>
    /// Custom <see cref="UITypeEditor"/> for <see cref="ChartColorPalette"/> enum.
    /// </summary>
    /// <internalonly/>
    [DocumentationExclude()]
    public class ChartColorPaletteEditor : ChartDropDownUIEditor
    {
        #region Class overrides
        /// <summary>
        /// Paints a representation of the value of an object using the specified <see cref="T:System.Drawing.Design.PaintValueEventArgs"></see>.
        /// </summary>
        /// <param name="e">A <see cref="T:System.Drawing.Design.PaintValueEventArgs"></see> that indicates what to paint and where to paint it.</param>
        public override void PaintValue(PaintValueEventArgs e)
        {
            float colCount = ChartColorModel.NumColorsInPalette;
            float xOffset = e.Bounds.Width / colCount;

            ChartColorModel colorModel = new ChartColorModel();
            colorModel.Palette = (ChartColorPalette)e.Value;

            if (colorModel.Palette != ChartColorPalette.Custom)
            {
                for (int i = 0; i < colCount; i++)
                {
                    using (SolidBrush sb = new SolidBrush(colorModel.GetColor(i)))
                    {
                        e.Graphics.FillRectangle(sb, e.Bounds.X + i * xOffset, e.Bounds.Y, xOffset, e.Bounds.Height);
                    }
                }
            }

            base.PaintValue(e);
        }

        /// <summary>
        /// Indicates whether the specified context supports painting a representation of an object's value within the specified context.
        /// </summary>
        /// <param name="context">An <see cref="T:System.ComponentModel.ITypeDescriptorContext"></see> that can be used to gain additional context information.</param>
        /// <returns>
        /// true if <see cref="M:System.Drawing.Design.UITypeEditor.PaintValue(System.Object,System.Drawing.Graphics,System.Drawing.Rectangle)"></see> is implemented; otherwise, false.
        /// </returns>
        public override bool GetPaintValueSupported(ITypeDescriptorContext context)
        {
            return true;
        }

        /// <summary>
        /// Gets the values.
        /// </summary>
        /// <returns>Returns IList.</returns>
        protected override IList GetValues()
        {
            return Enum.GetValues(typeof(ChartColorPalette));
        }
        #endregion
    }

    /// <summary>
    /// Provides a <see cref="UITypeEditor"/> for visually picking a <see cref="BrushInfo"/>.
    /// </summary>
    /// <internalonly/>
    [DocumentationExclude()]
    public class BrushInfoEditor : UITypeEditor
    {
        #region Class members
        /// <summary>
        /// Store UI for edit <see cref="BrushInfo"/> value.
        /// </summary>
        private BrushInfoUI m_brushInfoUI;
        #endregion

        #region Class overrides
        /// <summary>
        /// Edits the specified object's value using the editor style indicated by the <see cref="M:System.Drawing.Design.UITypeEditor.GetEditStyle"></see> method.
        /// </summary>
        /// <param name="context">An <see cref="T:System.ComponentModel.ITypeDescriptorContext"></see> that can be used to gain additional context information.</param>
        /// <param name="provider">An <see cref="T:System.IServiceProvider"></see> that this editor can use to obtain services.</param>
        /// <param name="value">The object to edit.</param>
        /// <returns>
        /// The new value of the object. If the value of the object has not changed, this should return the same object it was passed.
        /// </returns>
        public override object EditValue(System.ComponentModel.ITypeDescriptorContext context, IServiceProvider provider, object value)
        {
            if (provider != null)
            {
                IWindowsFormsEditorService edSvc = (IWindowsFormsEditorService)provider.GetService(typeof(IWindowsFormsEditorService));
                if (edSvc == null)
                {
                    return value;
                }

                if (m_brushInfoUI == null)
                {
                    m_brushInfoUI = new BrushInfoUI(this);
                }

                m_brushInfoUI.Start(edSvc, value);
                edSvc.DropDownControl(m_brushInfoUI);
                value = m_brushInfoUI.Value;
                m_brushInfoUI.End();
            }
            return value;
        }

        /// <summary>
        /// Gets the editor style used by the <see cref="M:System.Drawing.Design.UITypeEditor.EditValue(System.IServiceProvider,System.Object)"></see> method.
        /// </summary>
        /// <param name="context">An <see cref="T:System.ComponentModel.ITypeDescriptorContext"></see> that can be used to gain additional context information.</param>
        /// <returns>
        /// A <see cref="T:System.Drawing.Design.UITypeEditorEditStyle"></see> value that indicates the style of editor used by the <see cref="M:System.Drawing.Design.UITypeEditor.EditValue(System.IServiceProvider,System.Object)"></see> method. If the <see cref="T:System.Drawing.Design.UITypeEditor"></see> does not support this method, then <see cref="M:System.Drawing.Design.UITypeEditor.GetEditStyle"></see> will return <see cref="F:System.Drawing.Design.UITypeEditorEditStyle.None"></see>.
        /// </returns>
        public override UITypeEditorEditStyle GetEditStyle(System.ComponentModel.ITypeDescriptorContext context)
        {
            return UITypeEditorEditStyle.DropDown;
        }
        #endregion

        #region Class internal declaration
        /// <summary>
        /// Represent user interface for edit <see cref="BrushInfo"/> class.
        /// </summary>
        public class BrushInfoUI : UserControl
        {
            #region Form controls
            private Panel pnlCommands;
            private Button btnCancel;
            private Button btnOk;
            private PropertyGrid propertyGrid;

            /// <summary>
            /// Required designer variable.
            /// </summary>
            private System.ComponentModel.IContainer components = null;
            #endregion

            #region Class members
            /// <summary>
            /// Store class for edit brush in property grid.
            /// </summary>
            private Interior m_value;

            /// <summary>
            /// Store default brush.
            /// </summary>
            private BrushInfo m_defValue;

            /// <summary>
            /// Store <see cref="IWindowsFormsEditorService"/>
            /// </summary>
            private IWindowsFormsEditorService m_edSvc;
            #endregion

            #region Class properties
            /// <summary>
            /// Gets the value.
            /// </summary>
            public object Value
            {
                get
                {
                    return m_value.Brush;
                }
            }
            #endregion

            #region Class initialize/finalize methods
            /// <summary>
            /// Initializes a new instance of the <see cref="BrushInfoUI"/> class.
            /// </summary>
            public BrushInfoUI(BrushInfoEditor editor)
            {
                InitializeComponent();
            }

            /// <summary>
            /// Clean up any resources being used.
            /// </summary>
            /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
            protected override void Dispose(bool disposing)
            {
                if (disposing && (components != null))
                {
                    components.Dispose();
                }
                base.Dispose(disposing);
            }
            #endregion

            #region Windows Form Designer generated code

            /// <summary>
            /// Required method for Designer support - do not modify
            /// the contents of this method with the code editor.
            /// </summary>
            private void InitializeComponent()
            {
                this.pnlCommands = new System.Windows.Forms.Panel();
                this.btnOk = new System.Windows.Forms.Button();
                this.btnCancel = new System.Windows.Forms.Button();
                this.propertyGrid = new System.Windows.Forms.PropertyGrid();
                this.pnlCommands.SuspendLayout();
                this.SuspendLayout();
                // 
                // pnlCommands
                // 
                this.pnlCommands.Controls.Add(this.btnCancel);
                this.pnlCommands.Controls.Add(this.btnOk);
                this.pnlCommands.Dock = System.Windows.Forms.DockStyle.Bottom;
                this.pnlCommands.Location = new System.Drawing.Point(0, 138);
                this.pnlCommands.Name = "pnlCommands";
                this.pnlCommands.Size = new System.Drawing.Size(216, 46);
                this.pnlCommands.TabIndex = 0;
                // 
                // btnOk
                // 
                this.btnOk.Location = new System.Drawing.Point(24, 12);
                this.btnOk.Name = "btnOk";
                this.btnOk.Size = new System.Drawing.Size(75, 23);
                this.btnOk.TabIndex = 0;
                this.btnOk.Text = "Ok";
#if SyncfusionFramework2_0
                this.btnOk.UseVisualStyleBackColor = true;
#endif
                this.btnOk.Click += new EventHandler(btnOk_Click);
                // 
                // btnCancel
                // 
                this.btnCancel.Location = new System.Drawing.Point(118, 12);
                this.btnCancel.Name = "btnCancel";
                this.btnCancel.Size = new System.Drawing.Size(75, 23);
                this.btnCancel.TabIndex = 0;
                this.btnCancel.Text = "Cancel";
#if SyncfusionFramework2_0
                this.btnCancel.UseVisualStyleBackColor = true;
#endif
                this.btnCancel.Click += new EventHandler(btnCancel_Click);
                // 
                // propertyGrid
                // 
                this.propertyGrid.Dock = System.Windows.Forms.DockStyle.Fill;
                this.propertyGrid.HelpVisible = false;
                this.propertyGrid.Location = new System.Drawing.Point(0, 0);
                this.propertyGrid.Name = "propertyGrid";
                this.propertyGrid.Size = new System.Drawing.Size(216, 138);
                this.propertyGrid.TabIndex = 1;
                this.propertyGrid.ToolbarVisible = false;
                this.propertyGrid.PropertySort = PropertySort.Alphabetical;
                // 
                // BrushInfoUI
                // 
#if SyncfusionFramework2_0
                this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
                this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
#endif
                this.ClientSize = new System.Drawing.Size(216, 184);
                this.Controls.Add(this.propertyGrid);
                this.Controls.Add(this.pnlCommands);
                this.Name = "BrushInfoUI";
                this.pnlCommands.ResumeLayout(false);
                this.ResumeLayout(false);

            }
            #endregion

            #region Class public methods
            /// <summary>
            /// Starts the specified <see cref="IWindowsFormsEditorService"/>.
            /// </summary>
            /// <param name="edSvc">The <see cref="IWindowsFormsEditorService"/>.</param>
            /// <param name="value">The value to edit.</param>
            public void Start(IWindowsFormsEditorService edSvc, object value)
            {
                m_edSvc = edSvc;
                m_value = new Interior();
                m_defValue = value as BrushInfo;
                m_value.Brush = (value as ICloneable).Clone() as BrushInfo;
                this.propertyGrid.SelectedObject = m_value;
                this.propertyGrid.ExpandAllGridItems();
            }

            /// <summary>
            /// Ends this instance.
            /// </summary>
            public void End()
            {
                m_edSvc = null;
                m_value = null;
            }
            #endregion

            #region Class overrides
            /// <summary>
            /// Raises the <see cref="E:System.Windows.Forms.Control.KeyDown"></see> event.
            /// </summary>
            /// <param name="e">A <see cref="T:System.Windows.Forms.KeyEventArgs"></see> that contains the event data.</param>
            protected override void OnKeyDown(KeyEventArgs e)
            {
            }

            /// <summary>
            /// Raises the <see cref="E:System.Windows.Forms.Control.KeyUp"></see> event.
            /// </summary>
            /// <param name="e">A <see cref="T:System.Windows.Forms.KeyEventArgs"></see> that contains the event data.</param>
            protected override void OnKeyUp(KeyEventArgs e)
            {
                if (e.KeyData == Keys.Escape)
                {
                    m_value.Brush = m_defValue;
                }
            }
            #endregion

            #region Class event handlers
            /// <summary>
            /// Handles the Click event of the btnOk control.
            /// </summary>
            /// <param name="sender">The source of the event.</param>
            /// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
            private void btnOk_Click(object sender, EventArgs e)
            {
                m_edSvc.CloseDropDown();
            }

            /// <summary>
            /// Handles the Click event of the btnCancel control.
            /// </summary>
            /// <param name="sender">The source of the event.</param>
            /// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
            private void btnCancel_Click(object sender, EventArgs e)
            {
                m_value.Brush = m_defValue;
                m_edSvc.CloseDropDown();
            }
            #endregion

            #region Class internal declaration
            /// <summary>
            /// Class for edit BrushInfo in property grid.
            /// </summary>
            class Interior
            {
                #region Class members
                /// <summary>
                /// Store brush.
                /// </summary>
                private BrushInfo m_brush;
                #endregion

                #region Class properties
                /// <summary>
                /// Gets or sets brush.
                /// </summary>
                public BrushInfo Brush
                {
                    get
                    {
                        return m_brush;
                    }

                    set
                    {
                        m_brush = value;
                    }
                }
                #endregion
            }
            #endregion
        }
        #endregion
    }

    /// <summary>
    /// Custom <see cref="UITypeEditor"/> for arrays of <see cref="ChartSymbolShape"/> enumeration.
    /// </summary>
    /// <internalonly/>
    [DocumentationExclude()]
    public class ChartSymbolShapeEditor : UITypeEditor
    {
        #region Implementation
        /// <summary>
        /// Paints a representation of the value of an object using the specified <see cref="T:System.Drawing.Design.PaintValueEventArgs"></see>.
        /// </summary>
        /// <param name="e">A <see cref="T:System.Drawing.Design.PaintValueEventArgs"></see> that indicates what to paint and where to paint it.</param>
        public override void PaintValue(PaintValueEventArgs e)
        {
            ChartSymbolShape shape = (ChartSymbolShape)e.Value;
            SmoothingMode smooth = e.Graphics.SmoothingMode;

            int wh = Math.Min(e.Bounds.Width, e.Bounds.Height) - 3;
            Point loc = new Point(e.Bounds.X + (e.Bounds.Width - wh) / 2, e.Bounds.Y + (e.Bounds.Height - wh) / 2);

            e.Graphics.SmoothingMode = SmoothingMode.AntiAlias;

            if (shape == ChartSymbolShape.Image)
            {
                e.Graphics.DrawIcon(SystemIcons.Question, e.Bounds);
            }
            else
            {
                ChartSymbolHelper.FillAndDrawSymbol(e.Graphics, shape, new Rectangle(loc, new Size(wh, wh)), Pens.Black, Brushes.Gray);
            }

            e.Graphics.SmoothingMode = smooth;

            base.PaintValue(e);
        }

        /// <summary>
        /// Indicates whether the specified context supports painting a representation of an object's value within the specified context.
        /// </summary>
        /// <param name="context">An <see cref="T:System.ComponentModel.ITypeDescriptorContext"></see> that can be used to gain additional context information.</param>
        /// <returns>
        /// true if <see cref="M:System.Drawing.Design.UITypeEditor.PaintValue(System.Object,System.Drawing.Graphics,System.Drawing.Rectangle)"></see> is implemented; otherwise, false.
        /// </returns>
        public override bool GetPaintValueSupported(ITypeDescriptorContext context)
        {
            return true;
        }
        #endregion
    }
}
