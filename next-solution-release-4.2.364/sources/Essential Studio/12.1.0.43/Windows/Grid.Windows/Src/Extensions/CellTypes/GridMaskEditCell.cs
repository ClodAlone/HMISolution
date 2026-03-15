//-------------------------------------------------------------------------------------------------
// <copyright file="GridMaskEditCell.cs" company="syncfusion">
//  Copyright (c) Syncfusion Inc. 2001 - 2014. All rights reserved.
//  Use of this code is subject to the terms of our license.
//  A copy of the current license can be obtained at any time by e-mailing
//  licensing@syncfusion.com. Re-distribution in any form is strictly
//  prohibited. Any infringement will be prosecuted under applicable laws. 
// </copyright>
//-------------------------------------------------------------------------------------------------

using System;
using System.Drawing;
using System.Collections;
using System.ComponentModel;
using System.Windows.Forms;
using System.Data;
using System.Runtime.Serialization;
using System.Runtime.InteropServices;
using System.Security.Permissions;

using System.Diagnostics;
using System.Globalization;

using Syncfusion.Diagnostics;
using Syncfusion.Windows.Forms;
using Syncfusion.Drawing;
using Syncfusion.Styles;
using Syncfusion.Windows.Forms.Tools;

namespace Syncfusion.Windows.Forms.Grid
{
    /// <summary>
    /// Adds grid cell-specific keyboard logic to a <see cref="MaskedEditBox"/>.
    /// </summary>
    [ToolboxItem(false)]
    public class GridMaskedEditBox : MaskedEditBox
    {
        GridMaskEditCellRenderer parent;

        /// <summary>
        /// Initializes a new <see cref="GridMaskedEditBox"/> and attaches it to a <see cref="GridMaskEditCellRenderer"/>.
        /// </summary>
        /// <param name="parent">Parent cell renderer object.</param>
        public GridMaskedEditBox(GridMaskEditCellRenderer parent)
        {
            this.parent = parent;
            this.AutoSize = false;
            this.TabStop = false;
        }

        /// <summary>
        /// Gets the associated cell renderer for the text box.
        /// </summary>
        [Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public GridMaskEditCellRenderer ParentCell
        {
            get
            {
                return this.parent;
            }
        }

        /// <summary>
        /// Determines whether the specified key is an input key or a special key that requires preprocessing.
        /// </summary>
        /// <param name="keyData">One of the key's values.</param>
        /// <returns>
        /// true if the specified key is an input key; otherwise, false.
        /// </returns>
        /// <override/>
        protected override bool IsInputKey(Keys keyData)
        {
            switch (keyData & Keys.KeyCode)
            {
                case Keys.Tab:
                    return this.ParentCell.Grid.WantTabKey;
                case Keys.Escape:
                    return this.ParentCell.Grid.WantEscapeKey;
                case Keys.Enter:
                    return this.ParentCell.Grid.WantEnterKey;
            }

            return base.IsInputKey(keyData);
        }

        /// <summary>
        /// Intercepts the Key messages.
        /// </summary>
        /// <param name="m">The message data.</param>
        /// <returns>
        /// True if the key is handled; False otherwise.
        /// </returns>
        /// <override/>
        protected override bool ProcessKeyMessage(ref Message m)
        {
#if DEBUG
            if (Switches.KeyboardEvents.TraceVerbose)
            {   
                TraceUtil.TraceCurrentMethodInfo(m.ToString());
            }
#else
                
            ;
#endif
            GridMaskEditCellRenderer tbr = ParentCell;

            Rectangle intersect = Rectangle.Intersect(this.Bounds, this.parent.Grid.GridBounds);

            Keys keyCode = (Keys)((int)m.WParam) & Keys.KeyCode;
            if (m.Msg == 0x102/*WM_CHAR*/)
            {
                if ((parent.Grid.WantEnterKey && keyCode == Keys.Enter)
                    || (parent.Grid.WantEscapeKey && keyCode == Keys.Escape))
                {
                    return true;
                }

                if (intersect != this.Bounds)
                {
                    this.parent.Grid.CurrentCell.ScrollInView(GridScrollCurrentCellReason.KeyPress);
                }
                ////                bool b = ParentCell.ProcessKeyEventArgs(ref m)
                ////                    || this.ProcessKeyEventArgs(ref m);
                return base.ProcessKeyMessage(ref m);
            }

            bool bCtl = (Control.ModifierKeys & Keys.Control) != Keys.None;
            bool bAlt = (Control.ModifierKeys & Keys.Alt) != Keys.None;
            bool bShift = (Control.ModifierKeys & Keys.Shift) != Keys.None;

            bool forwardParent = false;
            bool scrollInView = false;
            bool callBase = true;

            switch (keyCode)
            {
                case Keys.Home:
                case Keys.End:
                    if (bCtl)
                    {
                        forwardParent = true;
                    }
                    else
                    {
                        scrollInView = true;
                    }

                    break;

                case Keys.Left:
                    if (bCtl || (this.SelectionStart <= 0 && (this.SelectionLength == 0 || !bShift)))
                    {
                        forwardParent = true;
                    }
                    else
                    {
                        scrollInView = true;
                    }

                    callBase = false;
                    break;
                case Keys.Right:
                    if (bCtl || this.SelectionStart + this.SelectionLength >= this.Text.Length)
                    {
                        forwardParent = true;
                    }
                    else
                    {
                        scrollInView = true;
                    }

                    callBase = false;
                    break;
                case Keys.Up:
                    forwardParent = true;
                    break;
                case Keys.Down:
                    forwardParent = !bAlt;
                    callBase = !bAlt;
                    break;
                case Keys.PageDown:
                case Keys.PageUp:
                case Keys.Tab:
                case Keys.F2:
                    forwardParent = true;
                    break;

                case Keys.Escape:
                    if (parent.Grid.WantEscapeKey)
                    {
                        forwardParent = true;
                        scrollInView = true;
                        callBase = false;
                    }

                    break;

                case Keys.Enter:
                    if (parent.Grid.WantEnterKey)
                    {
                        forwardParent = true;
                        scrollInView = true;
                        callBase = false;
                    }

                    break;

                case Keys.X:
                case Keys.V:
                case Keys.C:
                    scrollInView = bCtl;
                    forwardParent = true;
                    break;

                case Keys.Insert:
                    scrollInView = true;
                    forwardParent = ((bCtl || bShift) && this.SelectionLength == 0) || bAlt; // || bShift;
                    break;

                case Keys.Delete:
                    scrollInView = true;
                    forwardParent = (bCtl || bAlt || bShift) && this.SelectionLength == 0;
                    callBase = false;
                    break;

                case Keys.F4:
                    if (bCtl || bAlt)
                    {
                        callBase = false;
                    }

                    break;

                default:
                    forwardParent = bCtl || bAlt;
                    break;
            }

            //// Give programmers a chance to modify the default behavior of ProcessKeyMessage
            //// without subclassing this control.
            GridCurrentCellControlKeyMessageEventArgs e = new GridCurrentCellControlKeyMessageEventArgs(this, m, scrollInView, forwardParent, callBase, true);
            this.parent.Grid.RaiseCurrentCellControlKeyMessage(e);
            if (e.Handled)
            {
                return e.Result;
            }

            scrollInView = e.ScrollInView;
            forwardParent = e.CallProcessKeyPreview;
            callBase = e.CallBaseProcessKeyMessage;

            if (scrollInView)
            {
                if (intersect != this.Bounds)
                {
                    this.parent.Grid.CurrentCell.ScrollInView(GridScrollCurrentCellReason.KeyPress);
                }
            }

            if (forwardParent)
            {
                //// This will trigger Grid.RaiseKeyDown and Grid.RaiseKeyUp events. CurrentCellKeyDown and CurrentCellKeyPress events might also
                //// be triggered from Grids ProcessKeyPreview method.
                this.ProcessKeyPreview(ref m);
                return true;
            }
            else
            {
                //// Call to ParentCell.ProcessKeyEventArgs will trigger CurrentCellKeyDown and CurrentCellKeyPress events but no
                //// Grid.RaiseKeyDown and Grid.RaiseKeyUp events.
                if (callBase)
                {
                    return base.ProcessKeyMessage(ref m);
                }
                else
                {
                    bool b = ParentCell.RaiseProcessKeyEventArgs(ref m)
                        || this.ProcessKeyEventArgs(ref m);

                    return b;
                }
            }
        }

        /// <override/>
        /// <summary>
        /// Preprocesses keyboard or input messages within the message loop before they are dispatched.
        /// </summary>
        /// <param name="msg">The Message.</param>
        /// <returns>True if they are preprocessed.</returns>
        public override bool PreProcessMessage(ref System.Windows.Forms.Message msg)
        {
            if (this.parent.Grid.NotifyCurrentCellControlPreProcessMessage(ref msg))
            {
                return true;
            }

            return base.PreProcessMessage(ref msg);
        }

        /// <override/>
        [SecurityPermission(SecurityAction.LinkDemand, UnmanagedCode = true)]
        protected override void WndProc(ref Message msg)
        {
            if (this.parent.Grid.NotifyCurrentCellControlWndProc(ref msg))
            {
                return;
            }

            base.WndProc(ref msg);
        }
    }

    /// <summary>
    /// Implements the data / model part for a MaskedEdit cell.
    /// </summary>
    /// <remarks>
    /// You typically access cell models through the <see cref="GridModel.CellModels"/>
    /// property of the <see cref="GridModel"/> class.<para/>
    /// A <see cref="GridMaskEditCellModel"/> can serve as model for several <see cref="GridMaskEditCellRenderer"/>
    /// instances if a there are several <see cref="GridControlBase"/> views for a <see cref="GridModel"/>.
    /// <para/>
    /// See <see cref="GridMaskEditCellRenderer"/> for more detailed information about this cell type.
    /// </remarks>
    [Serializable]
    public class GridMaskEditCellModel : GridStaticCellModel
    {
        private MaskedEditBox maskEditBox = new MaskedEditBox();

        /// <overload>
        /// Initializes a new <see cref="GridMaskEditCellModel"/> object.
        /// </overload>
        /// <summary>
        /// Initializes a new <see cref="GridMaskEditCellModel"/> object
        /// and stores a reference to the <see cref="GridModel"/> this cell belongs to.
        /// </summary>
        /// <param name="grid">The <see cref="GridModel"/> for this cell model.</param>
        /// <remarks>
        /// You typically access cell models through the <see cref="GridModel.CellModels"/>
        /// property of the <see cref="GridModel"/> class.
        /// </remarks>
        public GridMaskEditCellModel(GridModel grid)
            : base(grid)
        {
            AllowFloating = true;
        }

        /// <summary>
        /// Initializes a new <see cref="GridMaskEditCellModel"/> from a serialization stream.
        /// </summary>
        /// <param name="info">An object that holds all the data needed to serialize or deserialize this instance.</param>
        /// <param name="context">Describes the source and destination of the serialized stream specified by info. </param>
        protected GridMaskEditCellModel(SerializationInfo info, StreamingContext context)
            : base(info, context)
        {
        }

        /// <override/>
        /// <summary>Creates a renderer for this cell model.</summary>
        /// <param name="control">The grid control.</param>
        /// <returns>Cell renderer.</returns>
        public override GridCellRendererBase CreateRenderer(GridControlBase control)
        {
            return new GridMaskEditCellRenderer(control, this);
        }

        /// <override/>
        /// <summary>
        /// Gets the text to be displayed in the cell.
        /// </summary>
        /// <param name="style">Cell style information.</param>
        /// <param name="value">Cell value.</param>
        /// <returns>Text to be displayed in the cell.</returns>
        public override string GetText(GridStyleInfo style, object value)
        {
            if (style.MaskEdit.ClipMode == ClipModes.IncludeLiterals)
            {
                this.maskEditBox.Text = (value != null && !(value is DBNull)) ? value.ToString() : string.Empty;
                return maskEditBox.Text;
            }

            return base.GetText(style, value);
        }

        /// <override/>
        /// <summary>
        /// Returns the formatted text with formatting.
        /// </summary>
        /// <param name="style">Cell style information.</param>
        /// <param name="value">Cell value.</param>
        /// <param name="textInfo">TextInfo is a hint of who is calling, default is GridCellBaseTextInfo.DisplayText.</param>
        /// <returns>Formatted text.</returns>
        public override string GetFormattedText(GridStyleInfo style, object value, int textInfo)
        {
            InitMaskedEditProperties(this.maskEditBox, style);
            this.maskEditBox.Text = (value != null && !(value is DBNull)) ? value.ToString() : string.Empty;

            if (textInfo == GridCellBaseTextInfo.CopyText && style.MaskEdit.ClipMode == ClipModes.ExcludeLiterals)
            {
                return GetText(style, value);
            }
            else
            {
                return maskEditBox.FormattedText;
            }
        }

        /// <summary>
        /// Initializes a <see cref="MaskedEditBox"/> with information supplied by a <see cref="GridStyleInfo"/>
        /// </summary>
        /// <param name="mb">The control to be initialized.</param>
        /// <param name="style">The style with settings to be applied.</param>
        public static void InitMaskedEditProperties(MaskedEditBox mb, GridStyleInfo style)
        {
            GridMaskEditInfo mi = style.MaskEdit;

            mb.BeginInit();
            mb.ReadOnly = false;
            mb.Culture = style.GetCulture(true);
            mb.AllowPrompt = mi.AllowPrompt;
            mb.ClipMode = mi.ClipMode;
            mb.DateSeparator = mi.DateSeparator;
            mb.DateTimeFormatInfoObject = mi.DateTimeFormatInfoObject;
            mb.DecimalSeparator = mi.DecimalSeparator;
            mb.Mask = mi.Mask;
            mb.MaxValue = mi.MaxValue;
            mb.MinValue = mi.MinValue;
            mb.NumberFormatInfoObject = mi.NumberFormatInfoObject;
            mb.PaddingCharacter = mi.PaddingCharacter;
            mb.PassivePromptCharacter = mi.PassivePromptCharacter;
            mb.PromptCharacter = mi.PromptCharacter;
            mb.SpecialCultureValue = mi.SpecialCultureValue;
            mb.ThousandSeparator = mi.ThousandSeparator;
            mb.TimeSeparator = mi.TimeSeparator;
            mb.UsageMode = mi.UsageMode;
            mb.UseLocaleDefault = mi.UseLocaleDefault;
            mb.UseUserOverride = mi.UseUserOverride;
            mb.MaxLength = style.MaxLength;
            // Not used:
            // style.HorizontalAlignment
            // style.VerticalAlignment
            // style.TextMargins
            // style.ReadOnly
            mb.EndInit();

            // Text must be assigned outside BeginInit / EndInit block.
            //mb.Text = style.Text;
            mb.ReadOnly = style.ReadOnly;
        }
    }

    /// <summary>
    /// Implements the renderer part of a currency cell.
    /// </summary>
    /// <remarks>
    /// Use the <see cref="GridStyleInfo.MaskEdit"/> (<see cref="GridMaskEditInfo"/>) property
    /// of a <see cref="GridStyleInfo"/> to change masked edit properties for a cell.
    /// <para/>
    /// The following table lists some characteristics about the MaskedEdit cell type:
    /// <para/>
    /// <list type="table">
    ///     <listheader>
    ///         <term>Item</term>
    ///         <description>Description</description>
    ///     </listheader>
    ///     <item>
    ///         <term>CellType</term>
    ///         <description>MaskEdit</description>
    ///     </item>
    ///     <item>
    ///         <term>Renderer</term>
    ///         <description><see cref="GridMaskEditCellRenderer"/></description>
    ///     </item>
    ///     <item>
    ///         <term>Model</term>
    ///         <description><see cref="GridMaskEditCellModel"/></description>
    ///     </item>
    ///     <item>
    ///         <term>XP Themes Support</term>
    ///         <description>NA</description>
    ///     </item>
    ///     <item>
    ///         <term>Interactive</term>
    ///         <description>Edit with Text Input</description>
    ///     </item>
    ///     <item>
    ///         <term>Control</term>
    ///         <description><see cref="GridMaskedEditBox"/></description>
    ///     </item>
    ///     <item>
    ///         <term>Floating Support</term>
    ///         <description>Both</description>
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
    ///         <term><see cref="GridStyleInfo.AutoSize"/> (<see cref="System.Boolean"/>)</term>
    ///         <description>Gets / sets if the cell height should automatically increase when the edited text does not fit into the cell and <see cref="GridStyleInfo.WrapText"/> is True. If <see cref="GridStyleInfo.WrapText"/> is False, <see cref="GridStyleInfo.AutoSize"/> will affect the column width. (Default: False)</description>
    ///     </item>
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
    ///         <description>MaskedEdit (Default: Text Box)</description>
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
    ///         <term><see cref="GridStyleInfo.CultureInfo"/> (<see cref="System.Globalization.CultureInfo"/>)</term>
    ///         <description>The culture information holds rules for parsing and formatting the cell's value. (Default: NULL)</description>
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
    ///         <term><see cref="GridStyleInfo.FloatCell"/> (<see cref="System.Boolean"/>)</term>
    ///         <description>Gets / sets if text can float into the boundaries of a neighboring cell. (Default: True)</description>
    ///     </item>
    ///     <item>
    ///         <term><see cref="GridStyleInfo.FloodCell"/> (<see cref="System.Boolean"/>)</term>
    ///         <description>Gets / sets if this cell can be flooded by a previous cell. (Default: True)</description>
    ///     </item>
    ///     <item>
    ///         <term><see cref="GridStyleInfo.Font"/> (<see cref="GridFontInfo"/>)</term>
    ///         <description>The font for drawing text. (Default: GridFontInfo.Default)</description>
    ///     </item>
    ///     <item>
    ///         <term><see cref="GridStyleInfo.Format"/> (<see cref="System.String"/>)</term>
    ///         <description>Gets / sets the format mask for formatting the cell value. You can specify numeric format strings,
    /// date format strings, or enumeration format strings as discussed in the section "Format Specifiers and Format Providers" of the .NET Framework Developers Guide (see ms-help://MS.VSCC/MS.MSDNVS/cpguide/html/cpconformatspecifiersformatproviders.htm) (Default: String.Empty)</description>
    ///     </item>
    ///     <item>
    ///         <term><see cref="GridStyleInfo.Interior"/> (<see cref="Syncfusion.Drawing.BrushInfo"/>)</term>
    ///         <description> Lets you specify a solid backcolor, gradient, or pattern style with both back and forecolor for a cell's
    ///  background. (Default: SystemColors.Window)</description>
    ///     </item>
    ///     <item>
    ///         <term><see cref="GridStyleInfo.MaskEdit"/> (<see cref="GridMaskEditInfo"/>)</term>
    ///         <description>A nested object with masked edit properties for a cell.  (Default: GridMaskEditInfo.Default)</description>
    ///     </item>
    ///     <item>
    ///         <term><see cref="GridStyleInfo.MaxLength"/> (<see cref="System.Int32"/>)</term>
    ///         <description>Limits the number of characters the user can type into the cell. Note: When selecting a text from a choice list or when pasting text, the text can be longer. Additional validation is necessary on your side. (Default: 0)</description>
    ///     </item>
    ///     <item>
    ///         <term><see cref="GridStyleInfo.MergeCell"/> (<see cref="GridMergeCellDirection"/>)</term>
    ///         <description>Specifies merge behavior for an individual cell when merging cell's features have been enabled in a <see cref="GridModel"/> with <see cref="GridModelOptions.MergeCellsMode"/>. (Default: GridMergeCellDirection.None)</description>
    ///     </item>
    ///     <item>
    ///         <term><see cref="GridStyleInfo.ReadOnly"/> (<see cref="System.Boolean"/>)</term>
    ///         <description>Specifies if cell contents can be modified by the user. You can programmatically change Read-only cells by setting <see cref="GridModel.DiscardReadOnly"/> to True. (Default: False)</description>
    ///     </item>
    ///     <item>
    ///         <term><see cref="GridStyleInfo.Text"/> (<see cref="System.String"/>)</term>
    ///         <description>Gets / sets the value as a string. If a <see cref="GridStyleInfo.CellValueType"/>
    /// is specified, the text will be parsed and converted to the type specified with
    /// <see cref="GridStyleInfo.CellValueType"/> using any <see cref="GridStyleInfo.CultureInfo"/>
    /// information.
    ///  (Default: String.Empty)</description>
    ///     </item>
    ///     <item>
    ///         <term><see cref="GridStyleInfo.TextColor"/> (<see cref="System.Drawing.Color"/>)</term>
    ///         <description>Lets you specify the color for drawing the cell text. (Default: SystemColors.WindowText)</description>
    ///     </item>
    ///     <item>
    ///         <term><see cref="GridStyleInfo.TextMargins"/> (<see cref="GridMarginsInfo"/>)</term>
    ///         <description>Holds text margins in pixels. When drawing a cell, this specifies the empty area between the
    /// text rectangle and the client rectangle of the cell without borders and cell buttons. (Default: GridMarginsInfo.Default)</description>
    ///     </item>
    /// </list>
    /// <para/>    
    /// </remarks>
    public class GridMaskEditCellRenderer : GridStaticCellRenderer
    {
        private GridMaskedEditBox focusControl;

        /// <summary>
        /// Initializes a new GridMaskEditCellRenderer object for the given GridControlBase
        /// and GridCellModelBase.
        /// </summary>
        /// <param name="grid">The <see cref="GridControlBase"/> that displays this cell renderer.</param>
        /// <param name="cellModel">The <see cref="GridCellModelBase"/> that holds data for this cell renderer that should
        /// be shared among views.</param>
        /// <remarks>References to GridControlBase
        /// and GridCellModelBase will be saved.</remarks>
        public GridMaskEditCellRenderer(GridControlBase grid, GridCellModelBase cellModel)
            : base(grid, cellModel)
        {
            this.SupportsFocusControl = true;
            focusControl = new GridMaskedEditBox(this);
            FixControlParent(focusControl);
            focusControl.ValidationError += new ValidationErrorEventHandler(FocusControlValidationError);
            focusControl.TextChanged += new EventHandler(FocusControlTextChanged);
        }

        void FixControlParent(Control control)
        {
            if (control.Parent != Grid || !control.Visible)
            {
                control.Location = new Point(-1000, -1000);
                control.CausesValidation = false;
                control.Anchor = AnchorStyles.None;
                control.Dock = DockStyle.None;
                control.Parent = Grid.GetWindow();
                control.Visible = true;
                SetControl(control);
            }
        }

        void FocusControlTextChanged(object sender, EventArgs e)
        {
#if DEBUG
            if (Switches.CellRenderer.TraceVerbose)
            {
                TraceUtil.TraceCurrentMethodInfo();
            }
#endif
            if (InInitialize || inTextBoxChanged)
            {
                return;
            }

            inTextBoxChanged = true;
            try
            {
                if (!this.NotifyCurrentCellChanging())
                {
                    CurrentCell.SuspendEvents();
                    InitializeControlText(StyleInfo.CellValue);
                    CurrentCell.IsModified = false;
                    CurrentCell.ResumeEvents();
                    return;
                }

                this.SetControlValue(focusControl.ClipText, false);
                ResetControlText();
                this.NotifyCurrentCellChanged();

                Grid.Model.FloatingCellsChanged += new GridFloatingCellsChangedEventHandler(Model_FloatingCellsChanged);
                Model.SetActiveText(RowIndex, ColIndex, focusControl.Text);
                Grid.Model.FloatingCellsChanged -= new GridFloatingCellsChangedEventHandler(Model_FloatingCellsChanged);
                CurrentCell.IsModified = true;

                if (floated)
                {
                    GridRangeInfo rangeCell = GridRangeInfo.Cell(this.RowIndex, this.ColIndex);
                    Grid.RefreshRange(rangeCell);
                }
                else if (StyleInfo.AutoSize && StyleInfo.WrapText)
                {
                    Grid.Model.ColWidths.ResizeToFit(GridRangeInfo.Cell(RowIndex, ColIndex), GridResizeToFitOptions.NoShrinkSize | GridResizeToFitOptions.ResizeCoveredCells);
                }
            }
            finally
            {
                inTextBoxChanged = false;
                floated = false;
            }
        }

        bool inTextBoxChanged = false;
        bool floated = false;

        private void Model_FloatingCellsChanged(object sender, GridFloatingCellsChangedEventArgs e)
        {
            floated = true;
        }
        string text = string.Empty;
        /// <override/>
        protected override void OnDraw(System.Drawing.Graphics g, System.Drawing.Rectangle clientRectangle, int rowIndex, int colIndex, Syncfusion.Windows.Forms.Grid.GridStyleInfo style)
        {
            if (this.ShouldDrawFocused(rowIndex, colIndex))
            {
                if (this.focusControl != null)
                {
                    ////clientRectangle.Inflate(-1, -1);

                    ////if (false && inTextBoxChanged && focusControl.Visible)
                    ////{
                    ////    Rectangle textRectangle = RemoveMargins(clientRectangle, style);
                    ////    focusControl.Bounds = textRectangle;
                    ////}
                    ////else
                    {
                        //// Set size
                        focusControl.Size = clientRectangle.Size;
                        focusControl.Location = clientRectangle.Location;
                    }

                    if (!focusControl.ContainsFocus)
                    {
                        focusControl.Focus();
                    }
                }
            }
            else
            {
                text = style.FormattedText;
                DrawCellText(g, clientRectangle, rowIndex, colIndex, style, text);
            }
        }


        void DrawCellText(Graphics g, Rectangle clientRectangle, int rowIndex, int colIndex, GridStyleInfo style, string displayText)
        {
            bool drawDisabled = false;

            if (clientRectangle.IsEmpty)
            {
                return;
            }
            int imageIndex = style.ImageIndex;
            if (imageIndex != -1)
            {
                ImageList imageList = style.ImageList;
                if (imageList != null && imageIndex < imageList.Images.Count)
                {
                    // TODO: Draw in client or cell boundaries.
                    // Using cellbounds core has advantage that image won't shift around when
                    // we draw a border.
                    Rectangle imageRectangle = this.GetCellLayout(rowIndex, colIndex, style).CellRectangle;
                    DrawImage(g, imageList, imageIndex, imageRectangle, Grid.IsRightToLeft());
                }
            }

            Rectangle textRectangle = RemoveMargins(clientRectangle, style);
            if (textRectangle.IsEmpty)
            {
                return;
            }

            if (style.HasError)
            {
                displayText = style.Error;
                drawDisabled = true;
            }

            if (displayText.Length > 0)
            {
                GridDrawCellDisplayTextEventArgs e = new GridDrawCellDisplayTextEventArgs(g, displayText, textRectangle, style);
                Grid.RaiseDrawCellDisplayText(e);
                if (!e.Cancel)
                {
                    textRectangle = e.TextRectangle;
                    displayText = e.DisplayText;
                    Font font = style.GdipFont;
                    bool isTextRightToLeft = (style.RightToLeft == RightToLeft.Inherit && Grid.IsRightToLeft()) || style.RightToLeft == RightToLeft.Yes;
                    DrawText(g, displayText, font, textRectangle, style, style.TextColor, drawDisabled, isTextRightToLeft);
                }
            }
        }
        /// <summary>
        /// This method is called from PerformLayout to calculate the client rectangle given
        /// the inner rectangle of a cell and any boundaries of cell buttons.
        /// </summary>
        /// <param name="rowIndex">The row index.</param>
        /// <param name="colIndex">The column index.</param>
        /// <param name="style">The <see cref="GridStyleInfo"/> object that holds cell information.</param>
        /// <param name="innerBounds">The <see cref="System.Drawing.Rectangle"/> with the inner bounds of a cell.</param>
        /// <param name="buttonsBounds">An array of <see cref="System.Drawing.Rectangle"/> with bounds for each cell button element.</param>
        /// <returns>
        /// A <see cref="System.Drawing.Rectangle"/> with the bounds.
        /// </returns>
        /// <override/>
        protected override Rectangle OnLayout(int rowIndex, int colIndex, GridStyleInfo style, Rectangle innerBounds, Rectangle[] buttonsBounds)
        {
            Rectangle bounds = base.OnLayout(rowIndex, colIndex, style, innerBounds, buttonsBounds);
			//SD3702 - MaskEdit Text is not showing properly in GridControl
            //bounds.Inflate(-1, -1);
            return bounds;
        }
        
        ////        /// <override/>
        ////        protected /*internal*/ override bool OnSaveChanges()
        ////        {
        ////            TraceUtil.TraceCurrentMethodInfoIf(Switches.CellRenderer.TraceVerbose, this.focusControl.Text);
        ////            this.Grid.Model[RowIndex, ColIndex].CellValue = this.focusControl.ClipText;
        ////            return true;
        ////        }

        /// <summary>
        /// This method is called from GridCurrentCell.Validate after GridCurrentCell.Validating event has been
        /// fired. The default version checks if the active text fits any criteria as specified
        /// in the style object: It can be parsed into a cell value and meets GridCellValidateValueInfo criteria.
        /// </summary>
        /// <returns>
        /// True if the modified text is valid; False otherwise.
        /// </returns>
        /// <override/>
        protected /*internal*/ override bool OnValidate()
        {
            invalidText = null;
            focusControl.Validate(true);
            if (string.IsNullOrEmpty(this.ControlText) && !focusControl.Text.Equals(focusControl.Mask.Replace('9',' ')))
               this.ControlText = focusControl.Text;

            if (invalidText != null)
            {
                CurrentCell.ErrorMessage = "Invalid Text: " + invalidText;
                return false;
            }

            return base.OnValidate();
        }

        string invalidText = null;

        void FocusControlValidationError(object sender, ValidationErrorArgs e)
        {
            invalidText = e.InvalidText;
        }

        /// <override/>
        protected override void OnInitialize(int rowIndex, int colIndex)
        {
            //// Immediately switch into editing mode when cell is initialized.
            GridStyleInfo style = Grid.GetViewStyleInfo(rowIndex, colIndex, false);

            //// Get the client bounds taking floated cells into consideration.
            Rectangle bounds = GetCellClientRectangle(rowIndex, colIndex, style, true);

            //// Assign size before setting text ensure that it does not scroll
            focusControl.Size = bounds.Size;

            //// Initialize contents.
            focusControl.BorderStyle = BorderStyle.None;
            ////focusControl.BackColor = style.Interior.BackColor;
            focusControl.ForeColor = style.TextColor;
            focusControl.Font = style.GdipFont;
            focusControl.BackColor = Color.FromArgb(255, Grid.GetBackColor(style.Interior.BackColor));

            switch (style.HorizontalAlignment)
            {
                case GridHorizontalAlignment.Left:
                    focusControl.TextAlign = HorizontalAlignment.Left;
                    break;

                case GridHorizontalAlignment.Center:
                    focusControl.TextAlign = HorizontalAlignment.Center;
                    break;

                case GridHorizontalAlignment.Right:
                    focusControl.TextAlign = HorizontalAlignment.Right;
                    break;
            }

            GridMaskEditCellModel.InitMaskedEditProperties(this.focusControl, style);
            ////focusControl.EndInit();
            base.OnInitialize(rowIndex, colIndex);
        }

        /// <override/>
        protected /*internal*/ override void OnActivated()
        {
            if ((Grid.Model.Options.ActivateCurrentCellBehavior & GridCellActivateAction.SelectAll) != 0)
            {
                this.focusControl.SelectAll();
            }

            base.OnActivated();
        }

        /// <override/>
        protected override void OnHasFocusControlChanged()
        {
            if ((Grid.Model.Options.ActivateCurrentCellBehavior & GridCellActivateAction.SelectAll) != 0)
            {
                this.focusControl.SelectAll();
            }

            base.OnHasFocusControlChanged();
        }

        /// <override/>
        protected override void OnKeyDown(KeyEventArgs e)
        {
#if DEBUG
            if (Switches.KeyboardEvents.TraceVerbose)
            {
                TraceUtil.TraceCurrentMethodInfo(e.Handled, e.KeyCode, Control.ModifierKeys);
            }
#else
            ;
#endif
            if (!e.Handled)
            {
                switch (e.KeyCode)
                {
                    case Keys.Back:
                    case Keys.Delete:
                        if (!CurrentCell.HasControlFocus || !CurrentCell.IsEditing)
                        {
                            if (Control.ModifierKeys != Keys.Alt)
                            {
                                if (Grid.ShouldDeleteKeyClearCurrentCellContentsOnly())
                                {
                                    ////IsReadOnly() - cell is not readonly;  Grid.RaiseCurrentCellDeleting() - OnDeleteCell notification returns true
                                    if (!IsReadOnly()       
                                        && OnDeleting()
                                        && Grid.RaiseCurrentCellDeleting())
                                    {
                                        //// position caret
                                        focusControl.SelectAll();
                                        if (CurrentCell.BeginEdit())
                                        {
                                            ControlText = string.Empty;
                                            CurrentCell.HasControlFocus = true;
                                            CurrentCell.ScrollInView(GridScrollCurrentCellReason.KeyPress);
                                            e.Handled = true;
                                        }
                                    }
                                }
                            }
                        }

                        break;

                    case Keys.Escape:
                        return;

                    case Keys.End:
                        if (Control.ModifierKeys == Keys.None && !CurrentCell.HasControlFocus)
                        {
                            if (CurrentCell.BeginEdit())
                            {
                                // position caret
                                CurrentCell.HasControlFocus = true;
                                Grid.Update();
                                focusControl.SelectionStart = focusControl.FormattedText.Length;
                                focusControl.SelectionLength = 0;
                                CurrentCell.ScrollInView(GridScrollCurrentCellReason.KeyPress);
                                e.Handled = true;
                            }
                        }

                        break;

                    case Keys.Home:
                        if (Control.ModifierKeys == Keys.None && !CurrentCell.HasControlFocus)
                        {
                            if (CurrentCell.BeginEdit())
                            {
                                // position caret
                                CurrentCell.HasControlFocus = true;
                                Grid.Update();
                                focusControl.SelectionStart = 0;
                                focusControl.SelectionLength = 0;
                                CurrentCell.ScrollInView(GridScrollCurrentCellReason.KeyPress);
                                e.Handled = true;
                            }
                        }

                        break;

                    case Keys.Enter:
                        break;
                }
            }

            base.OnKeyDown(e);
        }

        /// <override/>
        protected override void OnKeyPress(KeyPressEventArgs e)
        {
#if DEBUG
            if (Switches.KeyboardEvents.TraceVerbose)
            {
                TraceUtil.TraceCurrentMethodInfo(e.Handled, e.KeyChar);
            }
#else
            ;
#endif
            bool controlKeyDown = (Control.ModifierKeys & Keys.Control) != Keys.None;
            bool menuKeyDown = (Control.ModifierKeys & Keys.Alt) != Keys.None;

            if (!e.Handled && !controlKeyDown && !menuKeyDown)
            {
                if (!Char.IsControl(e.KeyChar)
                    && !IsReadOnly()
                    && SupportsFocusControl)
                {
                    if (!CurrentCell.HasControlFocus)
                    {
                        if (this.ValidateString(e.KeyChar.ToString())
                            && CurrentCell.BeginEdit())
                        {
                            CurrentCell.HasControlFocus = true;
                            if (this.NotifyCurrentCellChanging())
                            {
                                ResetControlText();
                                ResetControlValue();
                                focusControl.Text = string.Empty;
                                CurrentCell.IsModified = true;
                                Grid.Update();
                                NativeMethods.SendMessage(focusControl.Handle, NativeMethods.WM_CHAR, (int)e.KeyChar, 0);
                            }
                        }

                        e.Handled = true;
                    }
                }
            }

            base.OnKeyPress(e);
        }

        /// <override/>
        protected override void InitializeControlText(object controlValue)
        {
            //// OnSetControlText will always be called with FormattedText as argument.
            ////
            //// Handling InitializeControlText instead of OnSetControlText
            //// prevents assignment of FormattedText to this.focusControl.Text. 
            //// This fixes problems described in incident 22124 with setting
            //// focus inside cell (sample mask: ">AAA&&&-&&-#")

            string text = StyleInfo.GetFormattedText(controlValue, GridCellBaseTextInfo.CurrentText);
            GridCurrentCellInitializeControlTextEventArgs e = new GridCurrentCellInitializeControlTextEventArgs(this.RowIndex, this.ColIndex, this.StyleInfo, controlValue, text);
            if (Grid.RaiseCurrentCellInitializeControlText(e) && controlValue != null)
            {
                text = StyleInfo.GetText(controlValue);
                this.focusControl.Text = text;
            }
        }

        /// <override/>
        protected override void OnDoubleClick(int rowIndex, int colIndex, MouseEventArgs e)
        {
            if (this.Grid.CurrentCell.HasCurrentCellAt(rowIndex, colIndex)
                && (this.Grid.Model.Options.ActivateCurrentCellBehavior & GridCellActivateAction.DblClickOnCell) != 0)
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
                CurrentCell.BeginEdit();

                focusControl.SelectAll();
                Point loc = layout.TextRectangle.Location;
                Point p = new Point(e.X - loc.X, e.Y - loc.Y);
                ControlMouseDown(this, new MouseEventArgs(e.Button, e.Clicks, p.X, p.Y, 0));
            }

            base.OnDoubleClick(rowIndex, colIndex, e);
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
                bool beginEdit = false;
                if ((clickOnCell && (Grid.Model.Options.ActivateCurrentCellBehavior & GridCellActivateAction.ClickOnCell) != 0)
                    || (Grid.Model.Options.ActivateCurrentCellBehavior & GridCellActivateAction.SetCurrent) != 0)
                {
                    beginEdit = true;
                }

                if (beginEdit)
                {
                    // Fake mouse click.
                    CurrentCell.BeginEdit();

                    if ((Grid.Model.Options.ActivateCurrentCellBehavior & GridCellActivateAction.SelectAll) == 0)
                    {
                        if (CurrentCell.HasControlFocus && e.Button == MouseButtons.Left)
                        {
                            focusControl.Update();
                            Point loc = layout.ClientRectangle.Location;
                            Point p = new Point(e.X - loc.X, e.Y - loc.Y);
                            ActiveXSnapshot.FakeLeftMouseClick(focusControl, p);
                        }
                    }
                    else
                    {
                        focusControl.SelectAll();
                        Point loc = layout.TextRectangle.Location;
                        Point p = new Point(e.X - loc.X, e.Y - loc.Y);
                        ControlMouseDown(this, new MouseEventArgs(e.Button, e.Clicks, p.X, p.Y, 0));
                    }
                }
            }

            base.OnClick(rowIndex, colIndex, e);
        }

        /// <override/>
        /// <summary>
        /// Determines whether the current cell can be copied to clipboard.
        /// </summary>
        /// <returns>True if it can be copied; False otherwise.</returns>
        public override bool CanCopy()
        {
            return Grid.Model.SelectedRanges.Count == 0;
        }
    }
}
