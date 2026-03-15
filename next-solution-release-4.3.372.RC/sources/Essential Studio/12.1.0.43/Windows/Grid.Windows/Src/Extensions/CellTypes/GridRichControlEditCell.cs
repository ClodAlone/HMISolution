#region Copyright Syncfusion Inc. 2001 - 2014
// Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Any infringement will be prosecuted under
// applicable laws. 
#endregion
using System;
using System.Drawing;
using System.Collections;
using System.ComponentModel;
using System.Windows.Forms;
using System.Data;
using System.Runtime.Serialization;

using Syncfusion.Diagnostics;
using Syncfusion.Drawing;
using Syncfusion.Windows.Forms;

namespace Syncfusion.Windows.Forms.Grid
{
    /// <summary>
    /// Implements the data / model part for a generic control cell.
    /// </summary>
    /// <remarks>
    /// You typically access cell models through the <see cref="GridModel.CellModels"/>
    /// property of the <see cref="GridModel"/> class.<para/>
    /// A <see cref="GridRichControlEditCellModel"/> can serve as model for several <see cref="GridCurrencyTextBoxCellRenderer"/>
    /// instances if a there are several <see cref="GridControlBase"/> views for a <see cref="GridModel"/>.
    /// <para/>
    /// See <see cref="GridRichControlEditCellRenderer"/> for more detailed information about this cell type.
    /// </remarks>
    [Serializable]
    public class GridRichControlEditCellModel : GridStaticCellModel
    {
        /// <overload>
        /// Initializes a new <see cref="GridRichControlEditCellModel"/> object.
        /// </overload>
        /// <summary>
        /// Initializes a new <see cref="GridRichControlEditCellModel"/> object 
        /// and stores a reference to the <see cref="GridModel"/> this cell belongs to.
        /// </summary>
        /// <param name="grid">The <see cref="GridModel"/> for this cell model.</param>    
        /// <remarks>
        /// You typically access cell models through the <see cref="GridModel.CellModels"/>
        /// property of the <see cref="GridModel"/> class.
        /// </remarks>
        public GridRichControlEditCellModel(GridModel grid)
            : base(grid)
        {
            AllowFloating = false;
            AllowMerging = false;
        }

        /// <summary>
        /// Initializes a new <see cref="GridRichControlEditCellModel"/> from a serialization stream.
        /// </summary>
        /// <param name="info">An object that holds all the data needed to serialize or deserialize this instance.</param>
        /// <param name="context">Describes the source and destination of the serialized stream specified by info. </param>
        protected GridRichControlEditCellModel(SerializationInfo info, StreamingContext context)
            : base(info, context)
        {
        }

        /// <override/>
        /// <summary>Creates a renderer for this cell model.</summary>
        /// <param name="control">The grid control.</param>
        /// <returns>Cell renderer.</returns>
        public override GridCellRendererBase CreateRenderer(GridControlBase control)
        {
            return new GridRichControlEditCellRenderer(control, this);
        }
    }

    /// <summary>
    /// Implements the renderer part of a generic control cell.
    /// </summary>
    /// <remarks>
    /// Use the <see cref="GridStyleInfo.Control"/> property to associate any windows
    /// forms control with this cell type. <para/>
    /// Be careful when sharing this cell type among several cells. In most
    /// cases, it will cause problems and you should assign a different control
    /// to each cell's <see cref="GridStyleInfo.Control"/>
    /// <para/>
    /// GridRichControlEditCellRenderer is derived from GridStaticCellRenderer. It adds support
    /// for embedding any custom .NET control inside a cell. But if you want to draw the content
    /// of the cell yourself, deriving from GridStaticCellRenderers and overriding its OnDraw
    /// method should be enough.
    /// <para/>
    /// See the SliderCells, WebBrowserCells, and PictureBoxCells for samples
    /// of this cell type.
    /// <para/>
    /// The following table lists some characteristics about the Control cell type:
    /// <para/>
    /// <list type="table">
    ///     <listheader>
    ///         <term>Item</term>
    ///         <description>Description</description>
    ///     </listheader>
    ///     <item>
    ///         <term>CellType</term>
    ///         <description>Control</description>
    ///     </item>
    ///     <item>
    ///         <term>Renderer</term>
    ///         <description><see cref="GridRichControlEditCellRenderer"/></description>
    ///     </item>
    ///     <item>
    ///         <term>Model</term>
    ///         <description><see cref="GridRichControlEditCellModel"/></description>
    ///     </item>
    ///     <item>
    ///         <term>XP Themes Support</term>
    ///         <description>NA</description>
    ///     </item>
    ///     <item>
    ///         <term>Interactive</term>
    ///         <description>Depends on embedded control</description>
    ///     </item>
    ///     <item>
    ///         <term>Control</term>
    ///         <description><see cref="Control"/></description>
    ///     </item>
    ///     <item>
    ///         <term>Floating Support</term>
    ///         <description>No</description>
    ///     </item>
    ///     <item>
    ///         <term>Base Type</term>
    ///         <description><see cref="GridStaticCellRenderer"/></description>
    ///     </item>
    /// </list>
    /// <para/>
    /// <para/>
    /// The cell's behavior and appearance can be customized with the following properties of the <see cref="GridStyleInfo"/> class:
    /// <para/>
    /// <list type="table">
    ///     <listheader>
    ///         <term>PropertyName</term>
    ///         <description>Description</description>
    ///     </listheader>
    ///     <item>
    ///         <term><see cref="GridStyleInfo.BaseStyle"/> (<see cref="System.String"/>)</term>
    ///         <description>The base style for this style instance with default values for properties that are not initialized for this style object. (Default: String.Empty)</description>
    ///     </item>
    ///     <item>
    ///         <term><see cref="GridStyleInfo.Borders"/> (<see cref="GridBordersInfo"/>)</term>
    ///         <description>Top, left, bottom, and right border settings. (Default: GridBordersInfo.Default)</description>
    ///     </item>
    ///     <item>
    ///         <term><see cref="GridStyleInfo.CellAppearance"/> (<see cref="GridCellAppearance"/>)</term>
    ///         <description>Specifies if cell edges shall be drawn raised, sunken, or flat (default). (Default: GridCellAppearance.Flat)</description>
    ///     </item>
    ///     <item>
    ///         <term><see cref="GridStyleInfo.CellTipText"/> (<see cref="System.String"/>)</term>
    ///         <description>ToolTip text to be displayed when user hovers mouse over cell. (Default: String.Empty)</description>
    ///     </item>
    ///     <item>
    ///         <term><see cref="GridStyleInfo.CellType"/> (<see cref="System.String"/>)</term>
    ///         <description>Control (Default: Text Box)</description>
    ///     </item>
    ///     <item>
    ///         <term><see cref="GridStyleInfo.CellValue"/> (<see cref="System.Object"/>)</term>
    ///         <description>This property holds the cell value. Although the cell value is typically a string, it can also be any other primitive type such as int, byte, enum, or any custom type that is derived from <see cref="System.Object"/>. (Default: String.Empty)</description>
    ///     </item>
    ///     <item>
    ///         <term><see cref="GridStyleInfo.CellValueType"/> (<see cref="System.Type"/>)</term>
    ///         <description>Specifies the preferred <see cref="System.Type"/> for cell values. When you assign a value to the <see cref="GridStyleInfo"/> object, the value will be converted to this type. If the value cannot be converted, <see cref="GridStyleInfo.Error"/> will contain error information. (Default: NULL)</description>
    ///     </item>
    ///     <item>
    ///         <term><see cref="GridStyleInfo.Control"/> (<see cref="System.Windows.Forms.Control"/>)</term>
    ///         <description>A custom control you can associate with a cell. (Default: NULL)</description>
    ///     </item>
    ///     <item>
    ///         <term><see cref="GridStyleInfo.Enabled"/> (<see cref="System.Boolean"/>)</term>
    ///         <description>Specifies if the cell can be activated as current cell or if the cell should be skipped when moving the current cell. (Default: True)</description>
    ///     </item>
    ///     <item>
    ///         <term><see cref="GridStyleInfo.Error"/> (<see cref="System.String"/>)</term>
    ///         <description>Holds error information if a value could not be converted to the <see cref="System.Type"/> specified with <see cref="GridStyleInfo.CellValueType"/>. (Default: String.Empty)</description>
    ///     </item>
    ///     <item>
    ///         <term><see cref="GridStyleInfo.Interior"/> (<see cref="Syncfusion.Drawing.BrushInfo"/>)</term>
    ///         <description> Lets you specify a solid backcolor, gradient, or pattern style with both back and forecolor for a cells
    ///  background. (Default: SystemColors.Window)</description>
    ///     </item>
    ///     <item>
    ///         <term><see cref="GridStyleInfo.ReadOnly"/> (<see cref="System.Boolean"/>)</term>
    ///         <description>Specifies if cell contents can be modified by the user. You can programmatically change Read-only cells by setting <see cref="GridModel.DiscardReadOnly"/> to True. (Default: False)</description>
    ///     </item>
    /// </list>
    /// <para/>   
    /// </remarks>
    public class GridRichControlEditCellRenderer : GridStaticCellRenderer
    {
        private Control control;

        /// <summary>
        /// Initializes a new GridRichControlEditCellRenderer object for the given GridControlBase
        /// and GridCellModelBase.
        /// </summary>
        /// <param name="grid">The <see cref="GridControlBase"/> that display this cell renderer.</param>
        /// <param name="cellModel">The <see cref="GridCellModelBase"/> that holds data for this cell renderer that should
        /// be shared among views.</param>
        /// <remarks>References to GridControlBase 
        /// and GridCellModelBase will be saved.</remarks>
        public GridRichControlEditCellRenderer(GridControlBase grid, GridCellModelBase cellModel)
            : base(grid, cellModel)
        {
            this.SupportsFocusControl = true;
            panel = new GridRichTextEntryPanel();
            GridStyleInfo style = Grid.Model[RowIndex, ColIndex];
            panel.RichTextBox.Font = style.GdipFont;
            if (this.HasControlValue)
            {
                string text = (string)Syncfusion.Styles.ValueConvert.ChangeType(this.ControlValue, typeof(string), StyleInfo.GetCulture(true));
                AssignRtf(panel.RichTextBox, text);
            }
            else
            {
                AssignRtf(panel.RichTextBox, style.Text);
            }

            Color backColor = Color.FromArgb(255, style.Interior.BackColor);
            panel.RichTextBox.BackColor = backColor;
            panel.RichTextBox.ForeColor = style.TextColor;
            panel.Dock = DockStyle.Fill;
            panel.Save += new EventHandler(PanelSave);
            panel.Cancel += new EventHandler(PanelCancel);
            control = panel;
        }

        
        public override bool OnScrollInView(GridScrollCurrentCellReason reason)
        {
            return base.OnScrollInView(reason);
        }
        /// <summary>
        /// Event handler for the <see cref="GridRichTextEntryPanel.Save"/> event of the <see cref="GridRichTextEntryPanel"/>.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">Event data. </param>
        protected virtual void PanelSave(object sender, EventArgs e)
        {
            // CurrentCell.CloseDropDown(PopupCloseType.Done);
        }

        /// <summary>
        /// Event handler for the <see cref="GridRichTextEntryPanel.Cancel"/> event of the <see cref="GridRichTextEntryPanel"/>.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">Event data. </param>
        protected virtual void PanelCancel(object sender, EventArgs e)
        {
            // CurrentCell.CloseDropDown(PopupCloseType.Canceled);
        }
        void AssignRtf(RichTextBox rtb)
        {
            if (HasControlValue)
            {
                string text = (string)Syncfusion.Styles.ValueConvert.ChangeType(this.ControlValue, typeof(string), StyleInfo.GetCulture(true));
                AssignRtf(panel.RichTextBox, text);
            }
            else if (HasControlText)
            {
                AssignRtf(panel.RichTextBox, this.ControlText);
            }
        }

        void AssignRtf(RichTextBox rtb, string rtf)
        {
            if (RichTextPaint.IsValidRtf(rtf))
            {
                rtb.Rtf = rtf;
            }
            else
            {
                rtb.Text = rtf;
            }
        }
        protected override void OnButtonClicked(int rowIndex, int colIndex, int button)
        {
            base.OnButtonClicked(rowIndex, colIndex, button);
        }
        Size EnlargeWithScrollbars(Size size)
        {
            if (Grid.PrintingMode)
            {
                size.Width += SystemInformation.VerticalScrollBarWidth + 2;
                size.Height += SystemInformation.HorizontalScrollBarHeight + 2;
            }

            return size;
        }

        Rectangle bounds = Rectangle.Empty;
        Control rControl = null;

        void BeginResizeNoPaint(Control c, Size size)
        {
            rControl = c;
            //  Syncfusion.Drawing.ActiveXSnapshot.BeginResizeNoPaint(c, size, out bounds);
        }

        void EndResizeNoPaint()
        {
            Control c = rControl;
            //   Syncfusion.Drawing.ActiveXSnapshot.EndResizeNoPaint(c, bounds);
        }

        private void MakeUnBuffered(Control control)
        {
            System.Reflection.MethodInfo mInfo = typeof(Control).GetMethod("SetStyle", System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.InvokeMethod | System.Reflection.BindingFlags.NonPublic);
            if (mInfo != null)
            {
                mInfo.Invoke(control, new object[] { Syncfusion.Windows.Forms.WhidbeyCompatibleControlStyles.DoubleBuffer, false });
            }

            foreach (Control c in control.Controls)
            {
                c.CausesValidation = false;
                MakeUnBuffered(c);
            }
        }

        /// <summary>
        /// Parents the control to the grid instance and ensures correct settings
        /// for <see cref="Control.CausesValidation"/>, <see cref="Control.Anchor"/>,
        /// and <see cref="Control.Dock"/>.
        /// </summary>
        /// <param name="control">The control that is shown in this cell.</param>
        protected void FixControlParent(Control control)
        {
            if (control.Parent != Grid || !control.Visible)
            {
                control.Location = new Point(10000, 10000);
                foreach (Control c in control.Controls)
                {
                    c.CausesValidation = false;
                    MakeUnBuffered(c);
                }

                control.CausesValidation = false;
                control.Anchor = AnchorStyles.None;
                control.Dock = DockStyle.None;
                ////control.Parent = Grid;
                Grid.GetWindow().Controls.Add(control);
                control.Visible = true;
                SetControl(control);
            }
        }

        protected override void OnCellLayoutChanged()
        {
            base.OnCellLayoutChanged();
        }

        protected override void DrawCellAppearance(Graphics g, Rectangle rect, GridStyleInfo style)
        {
            base.DrawCellAppearance(g, rect, style);
        }

        public override bool ShouldRefreshCurrentCell()
        {
            return base.ShouldRefreshCurrentCell();
        }

        Control ctrl;
        public Control PassedControl
        {
            get
            {
                return ctrl;
            }
            set
            {
                ctrl = value;
            }
        }

        protected override void OnActivated()
        {
            GridStyleInfo style = Grid.Model[RowIndex, ColIndex];
            style.Control = Grid.Model.RichTextControl;
            ControlValue = Grid.Model.RichTextControl.Rtf;
            base.OnActivated();
        }
        protected override void ControlLostFocus(object sender, EventArgs e)
        {
            GridStyleInfo style = Grid.Model[RowIndex, ColIndex];
            if (style.Control is RichTextBox)
            {
                RichTextBox rtb = (RichTextBox)style.Control;
                ControlValue = rtb.Rtf;
                Grid.Model.RichTextControl.Rtf = rtb.Rtf;
            }
            style.CellType = GridCellTypeName.RichText;
            base.ControlLostFocus(sender, e);
        }

        GridRichTextEntryPanel panel = new GridRichTextEntryPanel();
        protected override void OnEditingComplete()
        {
            GridStyleInfo style = Grid.Model[RowIndex, ColIndex];
            if (style.Control is RichTextBox)
            {
                RichTextBox rtb = (RichTextBox)style.Control;
                ControlValue = rtb.Rtf;
                Grid.Model.RichTextControl.Rtf = rtb.Rtf;
            }
            Grid.Model.RichTextStyleRow  = RowIndex;
            Grid.Model.RichTextStyleCol = ColIndex; 
            style.CellType = GridCellTypeName.RichText;
            base.OnEditingComplete();
        }
        RichTextBox richTextClone = new RichTextBox();
        /// <override/>
        protected override void OnDraw(System.Drawing.Graphics g, System.Drawing.Rectangle clientRectangle, int rowIndex, int colIndex, Syncfusion.Windows.Forms.Grid.GridStyleInfo style)
        {
            Control control = style.Control;
            if (control != null)
            {
                this.FixControlParent(control);

                if (this.ShouldDrawFocused(rowIndex, colIndex))
                {
                    //// Position current grid.
                    control.Size = clientRectangle.Size;
                    ////control.PerformLayout();
                    control.Location = clientRectangle.Location;
                    if (!control.ContainsFocus)
                    {
                        control.Focus();
                    }
                }
                else
                {
                    //// Render control to bitmap and then draw the bitmap.
                    Bitmap bmp = null;

                    Size size = EnlargeWithScrollbars(clientRectangle.Size);

                    if (control is AxHost)
                    {
                        AxHost axHost = (AxHost)control;
                        object ocx = axHost.GetOcx();
                        BeginResizeNoPaint(axHost, size);
                        FixControlParent(control);
                        bmp = ActiveXSnapshot.TakeSnapshot(ocx);
                        EndResizeNoPaint();
                    }
                    else
                    {
                        BeginResizeNoPaint(control, size);
                        bmp = ActiveXSnapshot.PrintWindow(control);
                        EndResizeNoPaint();
                    }

                    if (bmp != null)
                    {
                        Region clip = g.Clip;
                        g.IntersectClip(clientRectangle);
                        //// TODO: Is there a better way to find out if we are printing
                        if (g.DpiY > 96 || GridControlBase.UseImageListDrawing)
                        {
                            g.DrawImage(bmp, clientRectangle);  // scale image 
                        }
                        else
                        {
                            g.DrawImageUnscaled(bmp, clientRectangle); // don't scale if printed to screen.
                        }

                        g.Clip = clip;
                    }
                }
            }
        }

        /// <summary>
        /// This is called from GridCurrentCell.Activate after the activating event has been raised
        /// and allows interception of cell activation.
        /// </summary>
        /// <param name="rowIndex">The row index.</param>
        /// <param name="colIndex">The column index.</param>
        /// <returns>
        /// True is cell can be activated; False otherwise.
        /// </returns>
        /// <override/>
        protected /*internal*/ override bool OnActivating(int rowIndex, int colIndex)
        {
            // Get the CellEmbeddedGrid from style Control and assign to Renderer.Control.
            object tag = Grid.Model[rowIndex, colIndex].Control;
            if (tag is Control)
            {
                control = (Control)tag;
                SetControl(control);
            }

            return base.OnActivating(rowIndex, colIndex);
        }

        /// <override/>
        /// <summary>
        /// Hides the control.
        /// </summary>
        public override void Hide()
        {
            object tag = this.StyleInfo.Control;
            if (tag is Control)
            {
                control = (Control)tag;
                SetControl(control);
            }

            base.Hide();
        }

        /// <override/>
        protected override void OnHasFocusControlChanged()
        {
            base.OnHasFocusControlChanged();
        }

        /// <override/>
        protected override void OnInitialize(int rowIndex, int colIndex)
        {
            // Immediately switch into editing mode when cell is initialized.
            CurrentCell.BeginEdit();
            base.OnInitialize(rowIndex, colIndex);
        }

        /// <override/>
        protected override void OnClick(int rowIndex, int colIndex, MouseEventArgs e)
        {
            if (this.Grid.CurrentCell.HasCurrentCellAt(rowIndex, colIndex))
            {
                // Activate text box and show caret.
                GridCellLayout layout = GetCellLayout(rowIndex, colIndex, Grid.Model[rowIndex, colIndex]);
                bool clickOnCell = layout.ClientRectangle.Contains(new Point(e.X, e.Y));
#if DEBUG
                if (Switches.CellRenderer.TraceVerbose)
                {
                    TraceUtil.TraceCurrentMethodInfo(rowIndex, colIndex, new Point(e.X, e.Y), clickOnCell, layout.ClientRectangle);
                }
#else
                ;
#endif
                Control focusControl = this.Control;
                bool beginEdit = focusControl != null;

                if (beginEdit)
                {
                    CurrentCell.BeginEdit();
                    if (CurrentCell.HasControlFocus && e.Button == MouseButtons.Left)
                    {
                        Grid.Update();
                        Point loc = layout.ClientRectangle.Location;
                        Point p = new Point(e.X - loc.X, e.Y - loc.Y);
                        ActiveXSnapshot.FakeLeftMouseClick(focusControl, p);
                    }
                }
            }

            base.OnClick(rowIndex, colIndex, e);
        }
    }
}