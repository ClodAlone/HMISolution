//-------------------------------------------------------------------------------------------------
// <copyright file="GridTextBoxCellRenderer.cs" company="syncfusion">//
//  Copyright (c) Syncfusion Inc. 2001 - 2014. All rights reserved.
//  Use of this code is subject to the terms of our license.
//  A copy of the current license can be obtained at any time by e-mailing
//  licensing@syncfusion.com. Re-distribution in any form is strictly
//  prohibited. Any infringement will be prosecuted under applicable laws. 
// </copyright>
//-------------------------------------------------------------------------------------------------

using System;
using System.Collections;
using System.Diagnostics;
using System.Drawing;
using System.Windows.Forms;
using System.Text;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;

using Syncfusion.Diagnostics;
using Syncfusion.Drawing;
using Syncfusion.Windows.Forms;

namespace Syncfusion.Windows.Forms.Grid
{
    /// <summary>
    /// Implements the renderer part of a text box cell.
    /// </summary>
    /// <remarks>
    /// Use "text box" as identifier in <see cref="GridStyleInfo.CellType"/> of a cells <see cref="GridStyleInfo"/>
    /// to associate this cell type with a cell.
    /// <para/>
    /// A renderer is created for each <see cref="GridCellModelBase"/>
    /// and <see cref="GridControlBase"/>. There can be several renderers
    /// associated with one <see cref="GridTextBoxCellModel"/> if several views display the same
    /// <see cref="GridModel"/>.
    /// <para/>
    /// <para/>
    /// The following table lists some characteristics about the text box cell type.
    /// <para/>
    /// <list type="table">
    ///     <listheader>
    ///         <term>Item</term>
    ///         <description>Description</description>
    ///     </listheader>
    ///     <item>
    ///         <term>CellType</term>
    ///         <description>Text Box</description>
    ///     </item>
    ///     <item>
    ///         <term>Renderer</term>
    ///         <description><see cref="GridTextBoxCellRenderer"/></description>
    ///     </item>
    ///     <item>
    ///         <term>Model</term>
    ///         <description><see cref="GridTextBoxCellModel"/></description>
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
    ///         <description><see cref="GridTextBoxControl"/></description>
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
    /// The cell's behavior and appearance can be customized with the following properties of the <see cref="GridStyleInfo"/> class.
    /// <para/>
    /// <list type="table">
    ///     <listheader>
    ///         <term>PropertyName</term>
    ///         <description>Description</description>
    ///     </listheader>
    ///     <item>
    ///         <term><see cref="GridStyleInfo.AllowEnter"/> (<see cref="System.Boolean"/>)</term>
    ///         <description>Gets / sets if pressing the &lt;Enter&gt;-Key should insert a new line into the edited text. (Default: False)</description>
    ///     </item>
    ///     <item>
    ///         <term><see cref="GridStyleInfo.AutoSize"/> (<see cref="System.Boolean"/>)</term>
    ///         <description>Gets / sets if the cell height should automatically increase when the edited text does not fit into the cell and <see cref="GridStyleInfo.WrapText"/> is True. If <see cref="GridStyleInfo.WrapText"/> is False, <see cref="GridStyleInfo.AutoSize"/> will affect the column width. (Default: False)</description>
    ///     </item>
    ///     <item>
    ///         <term><see cref="GridStyleInfo.BackgroundImage"/> (<see cref="System.Drawing.Image"/>)</term>
    ///         <description>Gets / sets the image that the cell displays as background. (Default: null)</description>
    ///     </item>
    ///     <item>
    ///         <term><see cref="GridStyleInfo.BackgroundImageMode"/> (<see cref="GridBackgroundImageMode"/>)</term>
    ///         <description>Indicates how the background image is displayed. (Default: GridBackgroundImageMode)</description>
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
    ///         <description>Text Box (Default: Text Box)</description>
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
    ///         <description>The culture information holds rules for parsing and formatting the cells value. (Default: NULL)</description>
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
    ///         <term><see cref="GridStyleInfo.HorizontalAlignment"/> (<see cref="GridHorizontalAlignment"/>)</term>
    ///         <description>Specifies horizontal alignment of text in the cell. (Default: GridHorizontalAlignment.Left)</description>
    ///     </item>
    ///     <item>
    ///         <term><see cref="GridStyleInfo.HotkeyPrefix"/> (<see cref="System.Drawing.Text.HotkeyPrefix"/>)</term>
    ///         <description>Specifies how hot-key prefixes should be displayed. Hot-keys are indicated in text with an '&amp;' (ampersand). When you enable hot-key prefix, the specific characters can be displayed underlined or regular. The '&amp;' will not be displayed. (Default: HotkeyPrefix.Show)</description>
    ///     </item>
    ///     <item>
    ///         <term><see cref="GridStyleInfo.ImageIndex"/> (<see cref="System.Int32"/>)</term>
    ///         <description>Specifies an index for an image in the <see cref="GridStyleInfo.ImageList"/> of a <see cref="GridStyleInfo"/>
    /// instance. (Default: -1)</description>
    ///     </item>
    ///     <item>
    ///         <term><see cref="GridStyleInfo.ImageList"/> (<see cref="System.Windows.Forms.ImageList"/>)</term>
    ///         <description>The <see cref="GridStyleInfo.ImageList"/> that holds a collection of images. Cells can choose images with the <see cref="GridStyleInfo.ImageIndex"/> property in a <see cref="GridStyleInfo"/>
    /// instance. (Default: NULL)</description>
    ///     </item>
    ///     <item>
    ///         <term><see cref="GridStyleInfo.Interior"/> (<see cref="Syncfusion.Drawing.BrushInfo"/>)</term>
    ///         <description>Lets you specify a solid backcolor, gradient, or pattern style with both back and forecolor for a cell's
    ///  background. (Default: SystemColors.Window)</description>
    ///     </item>
    ///     <item>
    ///         <term><see cref="GridStyleInfo.MaxLength"/> (<see cref="System.Int32"/>)</term>
    ///         <description>Limits the number of characters the user can type into the cell. Note: When selecting text from list or when pasting text, the text can be longer. Additional validation is necessary on your side. (Default: 0)</description>
    ///     </item>
    ///     <item>
    ///         <term><see cref="GridStyleInfo.MergeCell"/> (<see cref="GridMergeCellDirection"/>)</term>
    ///         <description>Specifies merge behavior for an individual cell when merging cells feature has been enabled in a <see cref="GridModel"/> with <see cref="GridModelOptions.MergeCellsMode"/>. (Default: GridMergeCellDirection.None)</description>
    ///     </item>
    ///     <item>
    ///         <term><see cref="GridStyleInfo.ReadOnly"/> (<see cref="System.Boolean"/>)</term>
    ///         <description>Specifies if cell contents can be modified by the user. You can programmatically change Read-only cells by setting <see cref="GridModel.DiscardReadOnly"/> to True. (Default: False)</description>
    ///     </item>
    ///     <item>
    ///         <term><see cref="GridStyleInfo.StrictValueType"/> (<see cref="System.Boolean"/>)</term>
    ///         <description>Indicates whether an exception should be thrown in the <see cref="GridStyleInfo.ApplyFormattedText(string)"/> method if the formatted text can not be parsed or converted to the type specified with <see cref="GridStyleInfo.CellValueType"/> (Default: true)</description>
    ///     </item>
    ///     <item>
    ///         <term><see cref="GridStyleInfo.Text"/> (<see cref="System.String"/>)</term>
    ///         <description>Gets / sets the value as a string. If a <see cref="GridStyleInfo.CellValueType"/>
    /// is specified, the text will be parsed or converted to the type specified with
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
    ///         <description>Holds text margins in pixels. When drawing a cell this specifies the empty area between the
    /// text rectangle and the client rectangle of the cell without borders and cell buttons. (Default: GridMarginsInfo.Default)</description>
    ///     </item>
    ///     <item>
    ///         <term><see cref="GridStyleInfo.Trimming"/> (<see cref="System.Drawing.StringTrimming"/>)</term>
    ///         <description>Indicates how text is trimmed when it exceeds the edges of the cell text rectangle. (Default: StringTrimming.Character)</description>
    ///     </item>
    ///     <item>
    ///         <term><see cref="GridStyleInfo.ValidateValue"/> (<see cref="GridCellValidateValueInfo"/>)</term>
    ///         <description>Holds validation rules for the cell value that are being checked before any user changes are committed to the grid cell's style object. (Default: NULL)</description>
    ///     </item>
    ///     <item>
    ///         <term><see cref="GridStyleInfo.VerticalAlignment"/> (<see cref="GridVerticalAlignment"/>)</term>
    ///         <description>Specifies vertical alignment of text in the cell. (Default: GridVerticalAlignment.Top)</description>
    ///     </item>
    ///     <item>
    ///         <term><see cref="GridStyleInfo.VerticalScrollbar"/> (<see cref="System.Boolean"/>)</term>
    ///         <description>Specifies if text box should show a vertical scrollbar when text is being edited and does not fit in cell. WrapText must be initialized to true. (Default: false)</description>
    ///     </item>
    ///     <item>
    ///         <term><see cref="GridStyleInfo.WrapText"/> (<see cref="System.Boolean"/>)</term>
    ///         <description>Specifies if text should be wrapped when it does not fit into a single line. (Default: True)</description>
    ///     </item>
    /// </list>
    /// <para/>
    /// <para/>
    /// <para/>
    /// </remarks>
    public class GridTextBoxCellRenderer : GridStaticCellRenderer
    {
        /// <internalonly/>
        private bool wantsAutoSize = false;

        /// <internalonly/>
        private int limitTextLength = 0;

        /// <internalonly/>
        private TextBoxBase textBoxControl = null;

        /// <internalonly/>
        private bool inTextBoxChanged = false;

        private bool disableTextBox = false;        //// set this TRUE if text shall not be editable
        ////private bool modified = false;
        private string disabledTextBoxText = String.Empty;

        /// <override/>
        /// <summary>Returns the state information that lets you restore the current edit state.</summary>
        /// <returns>State information.</returns>
        public override object GetEditState()
        {
            if (IsEditing)
            {
                return new int[] { this.TextBox.SelectionStart, this.TextBox.SelectionLength };
            }

            return null;
        }

        /// <override/>
        /// <summary>
        /// Restores previously retrieved editing state information from a GetEditState call.
        /// </summary>
        /// <param name="state">The cell-type specific object with the edit state information.</param>
        public override void SetEditState(object state)
        {
            if (IsEditing && state is int[])
            {
                int[] array = state as int[];
                if (array.Length == 2)
                {
                    this.TextBox.SelectionStart = array[0];
                    this.TextBox.SelectionLength = array[1];
                }
            }

            base.SetEditState(state);
        }
        
        /// <internalonly/>
        /// <summary>
        /// Used internally.
        /// </summary>
        [Syncfusion.Documentation.DocumentationExclude()]
        protected bool WantsAutoSize
        {
            get
            {
                return wantsAutoSize;
            }

            set
            {
                wantsAutoSize = value;
            }
        }

        /// <internalonly/>
        /// <summary>
        /// Used internally.
        /// </summary>
        [Syncfusion.Documentation.DocumentationExclude()]
        protected int LimitTextLength
        {
            get
            {
                return limitTextLength;
            }

            set
            {
                limitTextLength = value;
            }
        }

        /// <internalonly/>
        /// <summary>
        /// Used internally.
        /// </summary>
        [Syncfusion.Documentation.DocumentationExclude()]
        protected TextBoxBase TextBoxControl
        {
            get
            {
                return textBoxControl;
            }

            set
            {
                textBoxControl = value;
            }
        }

        /// <internalonly/>
        /// <summary>
        /// Used internally.
        /// </summary>
        [Syncfusion.Documentation.DocumentationExclude()]
        protected bool InTextBoxChanged
        {
            get
            {
                return inTextBoxChanged;
            }

            set
            {
                inTextBoxChanged = value;
            }
        }

        /// <summary>
        /// Initializes a new GridTextBoxCellRenderer object for the given GridControlBase
        /// and GridCellModelBase.
        /// </summary>
        /// <param name="grid">The <see cref="GridControlBase"/> that displays this cell renderer.</param>
        /// <param name="cellModel">The <see cref="GridCellModelBase"/> that holds data for this cell renderer that should
        /// be shared among views.</param>
        /// <remarks>References to GridControlBase,
        /// and GridCellModelBase will be saved.</remarks>
        public GridTextBoxCellRenderer(GridControlBase grid, GridCellModelBase cellModel)
            : base(grid, cellModel)
        {
            SupportsFocusControl = true;
        }

        /// <override/>
        protected override void WireModel(GridCellModelBase cellModel)
        {
            cellModel.ActiveTextChanged += new GridCellEventHandler(ModelActiveTextChanged);
            base.WireModel(cellModel);
        }

        /// <override/>
        protected /*internal*/ override void UnwireModel(GridCellModelBase cellModel)
        {
            cellModel.ActiveTextChanged -= new GridCellEventHandler(ModelActiveTextChanged);
            base.UnwireModel(cellModel);
        }

        void ModelActiveTextChanged(object sender, GridCellEventArgs e)
        {
            if (!inModelUpdateActiveText && !inTextBoxChanged && !InBeginEdit)
            {
#if DEBUG
                if (Switches.CellRenderer.TraceVerbose)
                {
                    TraceUtil.TraceCurrentMethodInfo(e, inTextBoxChanged);
                }
#else
                    ;
#endif
                Grid.InvalidateRange(GridRangeInfo.Cell(e.RowIndex, e.ColIndex), GridRangeOptions.MergeAllSpannedCells);
                if (CurrentCell.HasCurrentCellAt(e.RowIndex, e.ColIndex))
                {
                    string s = Model.GetActiveText(e.RowIndex, e.ColIndex);
                    if (s != null)
                    {
                        CurrentCell.Renderer.ControlText = s;
                    }
                    else
                    {
                        CurrentCell.Refresh();
                    }

                    CurrentCell.IsModified = false;
                }
            }
        }

        /// <override/>
        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                if (this.textBoxControl != null)
                {
                    this.textBoxControl.TextChanged -= new EventHandler(TextBox_Changed);
                    this.textBoxControl.LostFocus -= new EventHandler(TextBoxLostFocus);
                    this.textBoxControl.GotFocus -= new EventHandler(TextBoxGotFocus);
                    this.textBoxControl.Dispose();
                    this.textBoxControl = null;
                }
            }

            base.Dispose(disposing);
        }

        bool savedUseRightToLeftCompatibleTextBoxOption = false;

        /// <summary>
        /// Gets the text box that is shown in-place in the cell when the
        /// user starts editing the cell.
        /// </summary>
        public TextBoxBase TextBox
        {
            get
            {
                if (/*!this.disableTextBox && */this.textBoxControl == null
                     || savedUseRightToLeftCompatibleTextBoxOption != Grid.Model.Options.UseRightToLeftCompatibleTextBox)
                {
                    Control parent = Grid.GetWindow();
                    parent.SuspendLayout();
                    if (this.textBoxControl != null)
                    {
                        textBoxControl.Dispose();
                    }

                    GridQueryCreateCellTextBoxEventArgs e = new GridQueryCreateCellTextBoxEventArgs(this);
                    Grid.RaiseQueryCreateCellTextBox(e);
                    if (e.TextBox != null)
                    {
                        this.textBoxControl = e.TextBox;
                    }
                    else
                    {
                        this.textBoxControl = CreateTextBox();
                    }

                    this.textBoxControl.TabStop = false;
                    this.textBoxControl.TextChanged += new EventHandler(TextBox_Changed);
                    this.textBoxControl.LostFocus += new EventHandler(TextBoxLostFocus);
                    this.textBoxControl.GotFocus += new EventHandler(TextBoxGotFocus);
                    textBoxControl.Visible = false;
                    parent.Controls.Add(textBoxControl);
                    SetControl(textBoxControl);
                    parent.ResumeLayout(false);
                    savedUseRightToLeftCompatibleTextBoxOption = Grid.Model.Options.UseRightToLeftCompatibleTextBox;
                }

                return this.textBoxControl;
            }
        }

        /// <summary>
        /// Creates the text box that is shown in-place in the cell when the
        /// user starts editing the cell.
        /// </summary>
        /// <returns>
        /// Returns the new instance of the <see cref="GridTextBoxControl"/>.
        /// </returns>
        protected virtual TextBoxBase CreateTextBox()
        {
#if DEBUG
            if (Switches.CellRenderer.TraceVerbose)
            {
                TraceUtil.TraceCurrentMethodInfo();
            }
#endif

            if (Grid.Model.Options.UseRightToLeftCompatibleTextBox)
            {
                return new GridOriginalTextBoxControl(this);
            }

            return new GridTextBoxControl(this);
        }
        
        bool ignoreTextBoxChanged = false;

        /// <summary>
        /// Gets or sets a value indicating whether TextBoxChanged events should be ignored.
        /// </summary>
        public bool IgnoreTextBoxChanged
        {
            get
            {
                return ignoreTextBoxChanged;
            }

            set
            {
                ignoreTextBoxChanged = value;
            }
        }

        void TextBox_Changed(object sender, EventArgs e)
        {
            if (this.InSetControlText || this.IgnoreTextBoxChanged)
            {
                return;
            }

            inTextBoxChanged = true;
            try
            {
                TextBoxChanged(sender, e);
            }
            finally
            {
                inTextBoxChanged = false;
            }
        }

        bool inTextBoxChangedRollback = false;

        /// <summary>
        /// Occurs when the <see cref="Control.TextChanged"/> event of the <see cref="TextBox"/> is raised.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">Event data.</param>
        protected virtual void TextBoxChanged(object sender, EventArgs e)
        {
#if DEBUG
            if (Switches.CellRenderer.TraceVerbose)
            {
                TraceUtil.TraceCurrentMethodInfo();
            }
#endif
            if (InInitialize || ((IGridTextBoxControl)textBoxControl).Initializing)
            {
                return;
            }

            if (!this.NotifyCurrentCellChanging())
            {
                CurrentCell.SuspendEvents();
                object controlValue = StyleInfo.CellValue;
                this.inTextBoxChangedRollback = true;
                this.InitializeControlText(controlValue);
                ////TextBoxText = ControlText;
                //// InitializeControlText will call
                ////TextBoxText = controlValue != null ? controlValue.ToString() : string.Empty;
                this.inTextBoxChangedRollback = false;
                CurrentCell.IsModified = false;
                TextBox.ReadOnly = true;
                if (savedEditState != null)
                {
                    this.SetEditState(savedEditState);
                }

                CurrentCell.ResumeEvents();
                return;
            }

            this.ResetControlText();
            this.ResetControlValue();

            this.NotifyCurrentCellChanged();
            //// Happened already in InitControlText: ControlText = textBoxControl.Text;.
            CurrentCell.IsModified = true;

            if (Grid.AllowTextBoxAutoSize && StyleInfo.AutoSize && StyleInfo.WrapText)
            {
                inResizeToFit = true;
                Grid.Model.RowHeights.ResizeToFit(GridRangeInfo.Cell(RowIndex, ColIndex), GridResizeToFitOptions.NoShrinkSize | GridResizeToFitOptions.ResizeCoveredCells);
                inResizeToFit = false;
            }
        }

        /// <override/>
        protected override void OnSetControlText(string text)
        {
            if (!this.InTextBoxChanged || this.inTextBoxChangedRollback)
            {
                TextBoxText = text;
            }
        }

        bool inResizeToFit = false;

        /// <override/>
        protected sealed override void ControlLostFocus(object sender, EventArgs e)
        {
        }

        /// <override/>
        protected sealed override void ControlGotFocus(object sender, EventArgs e)
        {
        }

        /// <summary>
        /// Handles the <see cref="Control.LostFocus"/> event of the text box and raise a
        /// <see cref="GridControlBase.CurrentCellControlLostFocus"/> event.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">Event data.</param>
        protected virtual void TextBoxLostFocus(object sender, EventArgs e)
        {
#if DEBUG
            if (Switches.CellRenderer.TraceVerbose)
            {
                TraceUtil.TraceCurrentMethodInfo();
            }
#endif
            this.NotifyCurrentCellControlLostFocus(this.Control);
        }

        /// <summary>
        /// Handles the <see cref="Control.GotFocus"/> event of the text box and raises a
        /// <see cref="GridControlBase.CurrentCellControlGotFocus"/> event.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">Event data.</param>
        protected virtual void TextBoxGotFocus(object sender, EventArgs e)
        {
#if DEBUG
            if (Switches.CellRenderer.TraceVerbose)
            {
                TraceUtil.TraceCurrentMethodInfo();
            }
#endif
            if (Grid.Model.Options.ActivateCurrentCellBehavior == GridCellActivateAction.SelectAll)
            {
                TextBox.SelectAll();
            }

            this.NotifyCurrentCellControlGotFocus(this.Control);
        }

        /// <summary>
        /// Called from Initialize after currentRowIndex, inInitialize are set and PerformLayout finished.
        /// </summary>
        /// <param name="rowIndex">Specifies the row id.</param>
        /// <param name="colIndex">Specifies the column id.</param>
        protected override void OnInitialize(int rowIndex, int colIndex)
        {
#if DEBUG
            if (Switches.CellRenderer.TraceVerbose)
            {
                TraceUtil.TraceCurrentMethodInfo(rowIndex, colIndex);
            }
#else
               ;
#endif
            //// Stores the cell coordinates, resets the style and sets the window text.

            try
            {
                GridStyleInfo style = Grid.Model[rowIndex, colIndex];
                this.ControlValue = style.CellValue;
                //// Will raise Grid.CurrentCellInitializeControlText.
                bool isTextRightToLeft = (style.RightToLeft == RightToLeft.Inherit && Grid.IsRightToLeft()) || style.RightToLeft == RightToLeft.Yes;
                TextBox.RightToLeft = isTextRightToLeft ? RightToLeft.Yes : RightToLeft.No;

                if (!this.disableTextBox)
                {
                    TextBox.Select(0, 0);
                    TextBox.ReadOnly = Grid.Model[rowIndex, colIndex].ReadOnly;
                }
            }
            finally
            {
                base.OnInitialize(rowIndex, colIndex);
            }
        }

        bool forceControlValueInNotifyCurrentCellChanged = true;

        /// <summary>
        /// Gets or sets a value indicating whether a call to SetControlValue should be
        /// made when <see cref="NotifyCurrentCellChanged"/> is called.
        /// </summary>
        bool ForceControlValueInNotifyCurrentCellChanged
        {
            get
            {
                return forceControlValueInNotifyCurrentCellChanged;
            }

            set
            {
                forceControlValueInNotifyCurrentCellChanged = value;
            }
        }

        bool inNotifyCurrentCellChangedException = false;

        /// <summary>
        /// Gets a value indicating whether InNotifyCurrentCellChangedException. Internal only.
        /// </summary>
        /// <value>
        /// <c>true</c> if [in notify current cell changed exception]; otherwise, <c>false</c>.
        /// </value>
        /// <internalonly/>
        [Syncfusion.Documentation.DocumentationExclude()]
        public bool InNotifyCurrentCellChangedException
        {
            get
            {
                return inNotifyCurrentCellChangedException;
            }
        }

        bool inModelUpdateActiveText = false;

        /// <summary>
        /// Calls <see cref="GridCellModelBase.SetActiveText"/> on the <see cref="GridCellModelBase"/>
        /// </summary>
        protected void ModelUpdateActiveText()
        {
            inModelUpdateActiveText = true;
            Model.SetActiveText(currentRowIndex, currentColIndex, TextBoxText);
            inModelUpdateActiveText = false;
        }

        /// <override/>
        protected override void NotifyCurrentCellChanged()
        {
            if (forceControlValueInNotifyCurrentCellChanged && !this.InSetSetControlValue)
            {
                //// CurrentStyle is a view style - it is detached, changes won't go to cell.
                try
                {
                    GridStyleInfo style = StyleInfo;
                    if (this.HasControlValue)
                    {
                        style.CellValue = ControlValue;
                    }
                    else
                    {
                        ControlText = TextBoxText;
                    }

                    ////if (style.FormattedText != TextBoxText)
                    ////     Model.ApplyFormattedText(style, TextBoxText, GridCellBaseTextInfo.TextBox);
                    ////SetControlValue(style.CellValue, true);
                    ////if (!this.HasControlValue)
                    ////     ControlText = TextBoxText;
                    ModelUpdateActiveText();
                }
                catch
                {
                    inNotifyCurrentCellChangedException = true;
                    try
                    {
                        ControlText = TextBoxText;
                    }
                    finally
                    {
                        inNotifyCurrentCellChangedException = true;
                    }
                }
            }

            base.NotifyCurrentCellChanged();
        }

        bool inSetTextBoxTextCore = false;

        /// <summary>
        /// Gets a value indicating whether InSetTextBoxTextCore. Internal only.
        /// </summary>
        /// <value>
        /// <c>true</c> if [in set text box text core]; otherwise, <c>false</c>.
        /// </value>
        /// <internalonly/>
        [Syncfusion.Documentation.DocumentationExclude()]
        public bool InSetTextBoxTextCore
        {
            get
            {
                return inSetTextBoxTextCore;
            }
        }

        /// <summary>
        /// Gets or sets the TextBoxTextCore. Internal only.
        /// </summary>
        /// <value>The text box text core.</value>
        /// <internalonly/>
        [Syncfusion.Documentation.DocumentationExclude()]
        public string TextBoxTextCore
        {
            get
            {
                if (!this.disableTextBox)
                {
                    return TextBox.Text;
                }
                else
                {
                    return disabledTextBoxText;
                }
            }

            set
            {
                inSetTextBoxTextCore = true;
                try
                {
                    RichTextBox rtb = ((TextBoxBase)TextBox) as RichTextBox;
#if DEBUG
                    if (Switches.CellRenderer.TraceVerbose)
                    {
                        TraceUtil.TraceCurrentMethodInfo(value);
                    }
#else
                         ;
#endif

                    if (!this.disableTextBox)
                    {
                        HorizontalAlignment savedAlign = HorizontalAlignment.Left;
                        if (rtb != null)
                        {
                            bool savedIgnoreUICues = Grid.IgnoreUICues;
                            Grid.IgnoreUICues = true;
                            if (!rtb.Multiline && value.IndexOfAny(new char[] { '\r', '\n' }) != -1)
                            {
                                rtb.Multiline = true;
                            }

                            Grid.IgnoreUICues = savedIgnoreUICues;
                            savedAlign = rtb.SelectionAlignment;
                        }
                        else
                        {
                            //// original textbox
                            GridStyleInfo style = StyleInfo;
                            char pwc = style.PasswordChar;
                            if (pwc != ' ')
                            {
                                ((TextBox)TextBox).PasswordChar = style.PasswordChar;
                            }
                            else
                            {
                                ((TextBox)TextBox).PasswordChar = '\0';
                            }

                            ((TextBox)TextBox).CharacterCasing = style.CharacterCasing;
                        }

                        if (InInitialize)
                        {
                            ((IGridTextBoxControl)TextBox).SuspendEvents();
                            TextBox.Text = value;
                            if (rtb != null)
                            {
                                rtb.SelectAll();
                                rtb.SelectionAlignment = savedAlign;
                            }

                            ((IGridTextBoxControl)textBoxControl).ResumeEvents();
                        }
                        else
                        {
                            if (this.TextBoxText == value && !(this is GridDropDownCellRenderer))
                            {
                                return;
                            }

                            //// SH 8/3/04 - commented these lines out because TextBoxText
                            //// is called when NotifyCurrentCellChanging returned false in
                            //// TextBoxChanged. In that case there must be a way to reset
                            //// the text back to its old value.
                            ////if (this.IsReadOnly() || !this.NotifyCurrentCellChanging())
                            ////     return;

                            //// QA issue 23 fix
                            if (!this.InInitialize && !this.inTextBoxChangedRollback)
                            {
                             if (this.IsReadOnly()  || !NotifyCurrentCellChanging())
                                {
                                    return;
                                }
                            }

                            ((IGridTextBoxControl)TextBox).SuspendEvents();
                            TextBox.Text = value;
                            if (rtb != null)
                            {
                                rtb.SelectAll();
                                rtb.SelectionAlignment = savedAlign;
                            }

                            ((IGridTextBoxControl)textBoxControl).ResumeEvents();
                        }
                    }
                    else
                    {
                        if (InInitialize)
                        {
                            disabledTextBoxText = value;
                        }
                        else
                        {
                            if (this.TextBoxText == value)
                            {
                                return;
                            }

                            if (this.IsReadOnly() || !this.NotifyCurrentCellChanging())
                            {
                                return;
                            }

                            disabledTextBoxText = value;
                        }

                        GridCellLayout layout = GetCellLayout(RowIndex, ColIndex, StyleInfo);
                        Grid.InternalInvalidate(layout.TextRectangle);
                    }

                    if (Grid.Model.ActiveGridView == Grid)
                    {
                        if (CurrentCell.IsEditing)
                        {
                            SetModelActiveText(value);
                        }

                        Grid.InvalidateRange(GridRangeInfo.Cell(RowIndex, ColIndex), GridRangeOptions.MergeAllSpannedCells);
                    }

                    ControlText = value;

                    if (!this.InInitialize)
                    {
                        NotifyCurrentCellChanged();
                    }
                }
                finally
                {
                    inSetTextBoxTextCore = false;
                }
            }
        }

        /// <summary>
        /// Gets or sets the current text in the <see cref="TextBox"/>.
        /// </summary>
        /// <remarks>
        /// When you change this text, the setter of this property will ensure
        /// the cell is not Read-only, raise a <see cref="GridControlBase.CurrentCellChanging"/>
        /// change the text and after successful change, raise a <see cref="GridControlBase.CurrentCellChanged"/>
        /// event and force a refresh of the cell.</remarks>
        protected virtual string TextBoxText
        {
            get
            {
                return TextBoxTextCore;
            }

            set
            {
                if (!this.InSetTextBoxTextCore)
                {
                    TextBoxTextCore = value;
                }
            }
        }

        /// <summary>
        /// Calls <see cref="GridCellModelBase.SetActiveText"/> and ensures that <see cref="GridCellModelBase.ActiveTextChanged"/>
        /// event is ignored for the current renderer. Only other renderers in other grid views will handle the
        /// <see cref="GridCellModelBase.ActiveTextChanged"/> event.
        /// </summary>
        /// <param name="value">The text to be set.</param>
        protected void SetModelActiveText(string value)
        {
            bool itb = this.InTextBoxChanged;
            InTextBoxChanged = true;
            try
            {
                Model.SetActiveText(RowIndex, ColIndex, value);
            }
            finally
            {
                this.InTextBoxChanged = itb;
            }
        }

        /// <override/>
        /// <summary>
        /// Gets the selected text in the current cell.
        /// </summary>
        /// <param name="strResult">A string with the selected text.</param>
        /// <returns>True if the operation is successful.</returns>
        public override bool GetSelectedText(out string strResult)
        {
            if (this.Initalized)
            {
                if (!CurrentCell.HasControlFocus)
                {
                    strResult = ControlText;
                }
                else
                {
                    strResult = TextBox.SelectedText;
                }

                return true;
            }

            strResult = string.Empty;
            return false;
        }

        /// <override/>
        /// <summary>Replaces the selected text in the current cell.</summary>
        /// <param name="replacement">String to replace the current selected text.</param>
        public override void ReplaceSel(string replacement)
        {
            if (Initalized && Grid.CurrentCell.HasCurrentCellAt(RowIndex, ColIndex)
                 && CurrentCell.HasControlFocus)
            {
                TextBox.SelectedText = replacement;
            }
            else
            {
                base.ReplaceSel(replacement);
            }
        }

        ////          /// <override/>
        ////          protected /*internal*/ override bool OnSaveChanges()
        ////          {
        ////               TraceUtil.TraceCurrentMethodInfoIf(Switches.CellRenderer.TraceVerbose, this.TextBoxText);
        ////               if (CurrentCell.IsModified)
        ////               {
        ////                    GridStyleInfo style = Grid.Model[this.RowIndex, this.ColIndex];
        ////                    style.FormattedText = this.TextBoxText;
        ////                    return true;
        ////               }
        ////               return false;
        ////          }
        ////
        ////SS          public static int drawCount = 0;
        ////SS          public static bool inMouseDown = false;

        /// <override/>
        protected override void OnDraw(Graphics g, Rectangle clientRectangle, int rowIndex, int colIndex, GridStyleInfo style)
        {
            bool hasPaswordChar = false;
            if (this.TextBox is TextBox && this.ShouldDrawFocused(rowIndex, colIndex))
            {
                TextBox tb = (TextBox)this.TextBox;
                char pwc = style.PasswordChar;
                if (pwc != ' ')
                {
                    tb.PasswordChar = style.PasswordChar;
                    hasPaswordChar = true;
                }
                else
                {
                    tb.PasswordChar = '\0';
                }

                tb.CharacterCasing = style.CharacterCasing;
            }

            Rectangle textRectangle;
            ////SS               Trace.Write(String.Format("DrawCell({0},{1},{2}) ", rowIndex, colIndex, style.FormattedText));

            ////SS               if (Control.ModifierKeys == Keys.Control)
            ////SSDebugger.Break();

            if (Grid.CurrentCell.HasCurrentCellAt(rowIndex, colIndex) && CurrentCell.IsEditing && !Grid.PrintingMode)
            {
                ////SSTrace.WriteLine(++drawCount);
                ////SSTrace.WriteLine("\n+++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++");
                ////SSif (GridTextBoxCellRenderer.inMouseDown)
                ////SS                         Trace.//SSWriteLine("+++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++");

                if (!HasFocusControl)
                {
                    //// Disabled textbox control
#if DEBUG
                    Trace.WriteLineIf(Switches.TextBoxCellEvents.TraceVerbose, String.Format("Draw({0},{1}) : {2}", rowIndex, colIndex, TextBoxText));
#endif

                    Hide();
                    style.CellValue = TextBoxText;
                    base.OnDraw(g, clientRectangle, rowIndex, colIndex, style);
                    return;
                }
                else
                {
                    this.RowIndex = rowIndex;
                    this.ColIndex = colIndex;
#if DEBUG

                    if (Switches.CellRenderer.TraceVerbose)
                    {
                        TraceUtil.TraceCurrentMethodInfo(clientRectangle, rowIndex, colIndex);
                    }
#else

                         ;
#endif

                    int imageIndex = style.ImageIndex;
                    ImageList imageList = style.ImageList;
                    if (imageIndex != -1 && imageList != null && imageIndex < imageList.Images.Count)
                    {
                        Rectangle imageRectangle = this.GetCellLayout(rowIndex, colIndex, style).CellRectangle;
                        DrawImage(g, imageList, imageIndex, imageRectangle, Grid.IsRightToLeft());
                    }

                    //// Active textbox control
                    IGridTextBoxControl itextBox = TextBox as IGridTextBoxControl;
                    TextBoxBase textBox = TextBox;
                    itextBox.BeginInit();

                    if (itextBox.FloatDone && textBox.Visible)
                    {
                        textRectangle = RemoveMargins(clientRectangle, style);
                        textBox.Bounds = textRectangle;
                        itextBox.FloatDone = false;
                    }
                    else
                    {
                        bool textBoxModified = textBox.Modified;
                        ////Hide();
                        ////GridFontInfo ogfont = style.ReadOnlyFont;
                        ////ogfont.Size = font.Size*Grid.GetZoom()/100;
                        ////ogfont.Orientation = 0;
                        //// no vertical font for editing the cell
                        Font font = style.GdipFont; ////ogfont.GdipFont;

                        //// Cell-Color
                        if (Grid.PrintingMode && Grid.Model.Properties.BlackWhite)
                        {
                            textBox.ForeColor = Color.Black;
                            textBox.BackColor = Color.White;
                        }
                        else
                        {
                            textBox.ForeColor = Color.FromArgb(255, Grid.GetForeColor(style.TextColor));
                            textBox.BackColor = Color.FromArgb(255, Grid.GetBackColor(style.Interior.BackColor));
                        }

                        textRectangle = RemoveMargins(clientRectangle, style);
                        GridMargins tbmargins = itextBox.TextBoxMargins;
                        if (Grid.IsRightToLeft())
                        {
                            tbmargins = tbmargins.SwapRightToLeft();
                        }
                        
                        bool isTextRightToLeft = (style.RightToLeft == RightToLeft.Inherit && Grid.IsRightToLeft()) || style.RightToLeft == RightToLeft.Yes;

                        textBox.RightToLeft = isTextRightToLeft ? RightToLeft.Yes : RightToLeft.No;

                        Rectangle textBoxBounds = GridMargins.RemoveMargins(textRectangle, tbmargins);

                        if (inResizeToFit && textBox.Size == textBoxBounds.Size)
                        {
                            textBox.Location = textBoxBounds.Location;
                        }
                        else
                        {
                            Size sizeWg = WinFormsUtils.MeasureSampleWString(g, font);
                            ////sizeWg.Height += 2;
                            if (!style.WrapText && !style.AllowEnter)
                            {
                                //// Vertical alignment for single line text.
                                switch (style.VerticalAlignment)
                                {
                                    case GridVerticalAlignment.Middle:
                                        GridUtil.OffsetTop(ref textRectangle, Math.Max(0, (textRectangle.Height - sizeWg.Height) / 2));
                                        break;
                                    case GridVerticalAlignment.Bottom:
                                        GridUtil.OffsetTop(ref textRectangle, Math.Max(0, textRectangle.Bottom - sizeWg.Height));
                                        break;
                                }

                                textRectangle.Height = Math.Min(textRectangle.Height, sizeWg.Height); ////+1);
                            }

                            textBox.Font = font;
                            bool wrapText = style.WrapText;

                            RichTextBox rtb = textBox as RichTextBox;
                            TextBox tb = textBox as TextBox;

                            if (rtb != null)
                            {
                                if (style.VerticalScrollbar && wrapText)
                                {
                                    rtb.ScrollBars = RichTextBoxScrollBars.Vertical;
                                }
                                else
                                {
                                    rtb.ScrollBars = RichTextBoxScrollBars.None;
                                }
                            }
                            else
                            {
                                if (style.VerticalScrollbar && wrapText)
                                {
                                    tb.ScrollBars = ScrollBars.Vertical;
                                }
                                else
                                {
                                    tb.ScrollBars = ScrollBars.None;
                                }
                            }

                            /*if (!(textRectangle.Height > sizeWg.Height * 3/2 && wrapText))
                            {
                                 GridUtil.OffsetLeft(ref textRectangle, 1);
                                 GridUtil.OffsetTop(ref textRectangle, 1);
                            }
                            else
                            {
                                 textRectangle.Width -= 2;
                                 GridUtil.OffsetTop(ref textRectangle, 1);
                            }*/

                            textBox.Bounds = textBoxBounds;
                            this.wantsAutoSize = style.AutoSize && Grid.AllowTextBoxAutoSize;
                            ////itextBox.AcceptsReturn = style.AllowEnter;
                            textBox.WordWrap = wrapText;
                            if (hasPaswordChar)
                            {
                                textBox.Multiline = false;
                            }
                            else
                            {
                                textBox.Multiline = textRectangle.Height > sizeWg.Height * 3 / 2 && wrapText || style.AllowEnter;
                            }

                            this.limitTextLength = style.MaxLength;
                            textBox.MaxLength = this.limitTextLength;
                            textBox.ReadOnly = IsReadOnly();
                            int selectionStart = textBox.SelectionStart;
                            int selectionLength = textBox.SelectionLength;
                            ////itextBox.SelectAll();

                            if (rtb != null)
                            {
                                HorizontalAlignment selectionAlignment = HorizontalAlignment.Left;
                                switch (style.HorizontalAlignment)
                                {
                                    case GridHorizontalAlignment.Left:
                                        selectionAlignment = isTextRightToLeft ? HorizontalAlignment.Right : HorizontalAlignment.Left;
                                        break;

                                    case GridHorizontalAlignment.Center:
                                        selectionAlignment = HorizontalAlignment.Center;
                                        break;

                                    case GridHorizontalAlignment.Right:
                                        selectionAlignment = isTextRightToLeft ? HorizontalAlignment.Left : HorizontalAlignment.Right;
                                        break;
                                }

                                if (rtb.SelectionAlignment != selectionAlignment)
                                {
                                    rtb.SelectAll();
                                    rtb.SelectionAlignment = selectionAlignment;
                                }
                            }
                            else
                            {
                                switch (style.HorizontalAlignment)
                                {
                                    case GridHorizontalAlignment.Left:
                                        tb.TextAlign = isTextRightToLeft ? HorizontalAlignment.Right : HorizontalAlignment.Left;
                                        break;

                                    case GridHorizontalAlignment.Center:
                                        tb.TextAlign = HorizontalAlignment.Center;
                                        break;

                                    case GridHorizontalAlignment.Right:
                                        tb.TextAlign = isTextRightToLeft ? HorizontalAlignment.Left : HorizontalAlignment.Right;
                                        break;
                                }
                            }

                            if (selectionStart >= 0)
                            {
                                textBox.SelectionStart = selectionStart;
                                textBox.SelectionLength = selectionLength;
                            }

                            textBox.Visible = true;
                            textBox.Update();

                            if (Grid.Focused)
                            {
#if DEBUG
                                Trace.WriteLineIf(Switches.GridFocus.TraceVerbose, "--- Set TextBox.Focus in OnDraw");
#endif
                                textBox.RightToLeft = isTextRightToLeft ? RightToLeft.Yes: RightToLeft.No;
                                textBox.Focus();
                            }

                            textBox.Modified = textBoxModified;
                        }
                    }

                    itextBox.EndInit();
                }
            }
            else
            {
                //// static cell
                base.OnDraw(g, clientRectangle, rowIndex, colIndex, style);
            }
        }

        const int MK_LBUTTON = 1; /*0x0001*/
        const int MK_RBUTTON = 2; /*0x0002*/
        const int MK_SHIFT = 4; /*0x0004*/
        const int MK_CONTROL = 8; /*0x0008*/
        const int MK_MBUTTON = 16; /*0x0010*/

        //// Mouse hit

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
                    CurrentCell.BeginEdit();
                    if ((Grid.Model.Options.ActivateCurrentCellBehavior & GridCellActivateAction.SelectAll) == 0)
                    {
                        if (CurrentCell.HasControlFocus && e.Button == MouseButtons.Left
                             && (Grid.Model.Options.ActivateCurrentCellBehavior & GridCellActivateAction.PositionCaret) != 0)
                        {
                            Grid.Update();
                            Point loc = layout.TextRectangle.Location;
                            Point p = new Point(e.X - loc.X, e.Y - loc.Y);
                            NativeMethods.FakeLeftMouseClick(TextBox, p);
                        }
                        else
                        {
                            Point loc = layout.TextRectangle.Location;
                            Point p = new Point(e.X - loc.X, e.Y - loc.Y);
                            ControlMouseDown(this, new MouseEventArgs(e.Button, e.Clicks, p.X, p.Y, 0));
                        }
                    }
                    else
                    {
                        Point loc = layout.TextRectangle.Location;
                        Point p = new Point(e.X - loc.X, e.Y - loc.Y);
                        ControlMouseDown(this, new MouseEventArgs(e.Button, e.Clicks, p.X, p.Y, 0));
                    }
                }
                ////System.Threading.Thread.Sleep(1000);
                ////SSTraceUtil.TraceCurrentMethodInfoIf(Switches.CellRenderer.TraceVerbose, rowIndex, colIndex, new Point(e.X, e.Y), clickOnCell, layout.ClientRectangle);
            }

            base.OnClick(rowIndex, colIndex, e);
        }

        /// <override/>
        protected override void OnDoubleClick(int rowIndex, int colIndex, MouseEventArgs e)
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
            if ((clickOnCell && (Grid.Model.Options.ActivateCurrentCellBehavior & GridCellActivateAction.DblClickOnCell) != 0)
                 || (Grid.Model.Options.ActivateCurrentCellBehavior & GridCellActivateAction.SetCurrent) != 0)
            {
                beginEdit = true;
            }

            if (beginEdit)
            {
                CurrentCell.BeginEdit();
            }

            base.OnDoubleClick(rowIndex, colIndex, e);
        }

        /// <override/>
        protected override void OnHasFocusControlChanged()
        {
            if (HasFocusControl)
            {
                ignoreWmChar = true;
                bool visible = !this.DisableTextBox && TextBox.Visible && Grid.GridBounds.Contains(TextBox.Location);

                if (!this.DisableTextBox && TextBox.Visible && CurrentCell.IsVisible && Grid.Model.Options.ActivateCurrentCellBehavior == GridCellActivateAction.SelectAll)
                {
                    TextBox.SelectAll();
                }

                if (CurrentCell.IsModified)
                {
                    TextBox.Modified = true;
                }

                if (visible && !TextBox.Focused)
                {
#if DEBUG
                    Trace.WriteLineIf(Switches.GridFocus.TraceVerbose, "--- Set TextBox.Focus in OnHasFocusControlChanged");
#endif

                    TextBox.Focus();
                }
            }

            base.OnHasFocusControlChanged();
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
                savedEditState = this.GetEditState();

                if (!Char.IsControl(e.KeyChar)
                     && !IsReadOnly()
                     && SupportsFocusControl)
                {
                    if (!this.DisableTextBox)
                    {
                        //// TraceUtil.TraceCurrentMethodInfo(Grid.inImeComposition, e.Handled, e.KeyChar);

                        if (!CurrentCell.HasControlFocus)
                        {
                            if (this.ValidateString(e.KeyChar.ToString())
                                 && CurrentCell.BeginEdit())
                            {
                                CurrentCell.HasControlFocus = true;
                                if (this.NotifyCurrentCellChanging())
                                {
                                    ControlText = e.KeyChar.ToString();
                                    CurrentCell.IsModified = true;
                                    if (!this.disableTextBox)
                                    {
                                        TextBox.SelectionStart = TextBox.Text.Length;
                                        TextBox.SelectionLength = 0;
                                    }
                                }
                            }

                            e.Handled = true;
                        }
                        else if (Grid.inImeComposition)
                        {
                            string s = TextBoxText + e.KeyChar.ToString();
                            if (this.ValidateString(s))
                            {
                                if (this.NotifyCurrentCellChanging())
                                {
                                    CurrentCell.IsModified = true;
                                    TextBoxText = s;
                                }
                            }

                            e.Handled = true;
                        }
                    }
                }
            }

            base.OnKeyPress(e);
        }

        /// <summary>
        /// Sets the TextBox.Text of the text box and optionally validates the string with a <see cref="GridCellRendererBase.ValidateString"/> call.
        /// </summary>
        /// <param name="s">The text to be displayed.</param>
        /// <param name="validate">Indicates if <see cref="GridCellRendererBase.ValidateString"/> should be called.</param>
        /// <returns>True if text was valid and could be copied to the text box.</returns>
        /// <remarks>
        /// The method will also raises a cancelable <see cref="GridControlBase.CurrentCellChanging"/> event if the text
        /// was not modified.
        /// </remarks>
        public bool SetTextBoxText(string s, bool validate)
        {
            // REVIEW: What about CurrentCellChanged event afterwards?
            if (InInitialize)
            {
                TextBoxText = s;
                return true;
            }

            if (!IsReadOnly())
            {
                bool valid = !validate || this.ValidateString(s);

                if (valid)
                {
                    if (CurrentCell.BeginEdit())
                    {
                        if (this.NotifyCurrentCellChanging())
                        {
                            CurrentCell.IsModified = true;
                            TextBoxTextCore = s;
                            return true;
                        }
                    }
                }
            }

            return false;
        }

        /// <summary>
        /// Sets the TextBox.SelectedText of the text box and optionally validates the string with a <see cref="GridCellRendererBase.ValidateString"/> call.
        /// </summary>
        /// <param name="text">The text that should replace the current selected text in the text box.</param>
        /// <param name="validate">Indicates if <see cref="GridCellRendererBase.ValidateString"/> should be called.</param>
        /// <returns>True if text was valid and could be copied to the text box.</returns>
        /// <remarks>
        /// The method raises a <see cref="GridControlBase.CurrentCellChanged"/> event after the text
        /// was modified.
        /// <para/>
        /// Note: This is a low-level method. The method does not raise <see cref="GridControlBase.CurrentCellChanging"/> and also does not
        /// check if the current cell is already in editing mode.
        /// </remarks>
        public bool SetSelectedText(string text, bool validate)
        {
            //// REVIEW: CurrentCell.BeginEdit() and NotifyCurrentCellChanging
            string saved = TextBox.Text;
            int selStart = TextBox.SelectionStart;
            int selLength = TextBox.SelectionLength;
            TextBox.SuspendLayout();
            IGridTextBoxControl itextBox = TextBox as IGridTextBoxControl;
            itextBox.SuspendEvents();
            try
            {
                TextBox.SelectedText = text;
                bool valid = !validate || this.ValidateString(TextBox.Text);
                if (!valid)
                {
                    TextBox.Text = saved;
                    TextBox.SelectionStart = selStart;
                    TextBox.SelectionLength = selLength;
                    return false;
                }

                this.TextBox_Changed(TextBox, EventArgs.Empty);
                return true;
            }
            finally
            {
                TextBox.ResumeLayout();
                itextBox.ResumeEvents();
            }
        }

        object savedEditState;

        /// <override/>
        protected override void OnKeyDown(KeyEventArgs e)
        {
            savedEditState = this.GetEditState();
            ignoreWmChar = false;
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
                                    ////!IsReadOnly() - cell is not readonly, Grid.RaiseCurrentCellDeleting() - OnDeleteCell notification returns true
                                    if (!IsReadOnly()        
                                         && OnDeleting()
                                         && Grid.RaiseCurrentCellDeleting())
                                    {
                                        //// Delete text, SetTextBoxText call BeginEdit()
                                        SetTextBoxText(string.Empty, true);
                                        CurrentCell.IsModified = true;
                                        CurrentCell.ScrollInView(GridScrollCurrentCellReason.KeyPress);
                                    }

                                    e.Handled = true;
                                }
                                ////                                        else if (Control.ModifierKeys == Keys.Shift)
                                ////                                        {
                                ////                                             Grid.Model.CutPaste.Cut();
                                ////                                        }
                                ////                                        else
                                ////                                        {
                                ////                                             Grid.Model.Clear(Control.ModifierKeys == Keys.Control);
                                ////                                        }
                            }
                        }
                        else if (Grid.IsWindowless || TextBox is TextBox)  // TextBox is "Original TextBox", not RichTextbox ...
                        {
                            if (TextBox.SelectionLength > 0)
                            {
                                TextBox.SelectedText = string.Empty;
                            }
                            else if (TextBox.SelectionStart >= 0)
                            {
                                if (e.KeyCode == Keys.Back)
                                {
                                    if (TextBox.SelectionStart > 0)
                                    {
                                        TextBox.Select(TextBox.SelectionStart - 1, 1);
                                    }
                                }
                                else
                                {
                                    TextBox.Select(TextBox.SelectionStart, 1);
                                }

                                TextBox.SelectedText = string.Empty;
                            }

                            e.Handled = true;
                            this.ignoreWmChar = TextBox is GridOriginalTextBoxControl && TextBox.SelectionLength == 0;
                        }

                        break;

                    case Keys.Escape:
                        if (CurrentCell.IsDroppedDown)
                        {
                            CurrentCell.CloseDropDown(PopupCloseType.Canceled);
                            e.Handled = true;
                        }
                        else if (CurrentCell.IsModified)
                        {
                            CurrentCell.RejectChanges();
                            CurrentCell.CancelEdit();
                            CurrentCell.Refresh();
                            CurrentCell.ScrollInView(GridScrollCurrentCellReason.KeyPress);
                            e.Handled = true;
                        }
                        else
                        {
                            CurrentCell.CancelEdit();
                        }

                        if ((Grid.Model.Options.ActivateCurrentCellBehavior & Syncfusion.Windows.Forms.Grid.GridCellActivateAction.SetCurrent) != 0)
                        {
                            CurrentCell.BeginEdit();
                        }

                        return;

                    case Keys.End:
                        if (Control.ModifierKeys == Keys.None && !CurrentCell.HasControlFocus && !this.DisableTextBox)
                        {
                            if (CurrentCell.BeginEdit())
                            {
                                //// Position caret.
                                CurrentCell.HasControlFocus = true;
                                TextBox.SelectionStart = TextBox.Text.Length;
                                TextBox.SelectionLength = 0;
                                CurrentCell.ScrollInView(GridScrollCurrentCellReason.KeyPress);
                                e.Handled = true;
                            }
                        }

                        break;

                    case Keys.Home:
                        if (Control.ModifierKeys == Keys.None && !CurrentCell.HasControlFocus && !this.DisableTextBox)
                        {
                            if (CurrentCell.BeginEdit())
                            {
                                //// Position caret.
                                CurrentCell.HasControlFocus = true;
                                TextBox.SelectionStart = 0;
                                TextBox.SelectionLength = 0;
                                CurrentCell.ScrollInView(GridScrollCurrentCellReason.KeyPress);
                                Grid.Update();
                                e.Handled = true;
                            }
                        }

                        break;

                    case Keys.Enter:
                        if (Control.ModifierKeys == Keys.None && CurrentCell.IsEditing && this.StyleInfo.AllowEnter && this.NotifyCurrentCellChanging())
                        {
                            SetSelectedText("\n", true);
                            CurrentCell.ScrollInView(GridScrollCurrentCellReason.KeyPress);
                            e.Handled = true;
                        }

                        break;

                    case Keys.Tab:
                        if (Grid.WantTabKey)
                        {
                            ignoreWmChar = true;
                        }

                        break;
                    case Keys.V:
                        if (e.Control && this.HasFocusControl && CurrentCell.IsEditing && this.NotifyCurrentCellChanging() && !(this is GridFormulaCellRenderer))
                        {
                            if (this.CanPaste())
                            {
                                this.Paste();
                                e.Handled = true;
                            }
                        }
                        break;
                }
            }

            base.OnKeyDown(e);
        }

        /// <override/>
        /// <summary>
        /// Determines whether current cell can be copied to clipboard.
        /// </summary>
        /// <returns>returs True.</returns>
        /// <remarks></remarks>
        public override bool CanCopy()
        {
            return true;
        }

        /// <override/>
        /// <summary>Called when user initiates a clipboard copy and works with a single cell only.</summary>
        /// <returns>True if the operation is successful.</returns>
        public override bool Copy()
        {
            if (this.HasFocusControl && TextBox.SelectionLength == 0)
            {
                return false;
            }

            DataObject dataObject = new DataObject();
            if (this.HasFocusControl && !this.DisableTextBox && TextBox.SelectionLength > 0)
            {
                dataObject.SetData(DataFormats.UnicodeText, TextBox.SelectedText);
            }
            else
            {
                dataObject.SetData(DataFormats.UnicodeText, TextBoxText);
            }

            Clipboard.SetDataObject(dataObject);
            return true;
        }

        /// <override/>
        /// <summary>Called when user initiates a clipboard paste and works only with a single cell.</summary>
        /// <returns>True if the operation is successful.</returns>
        public override bool Paste()
        {
            if (TextBox.ReadOnly)
            {
                return false;
            }

            IDataObject iData = Clipboard.GetDataObject();

            if (iData != null)
            {
                string buffer = null;
                if (iData.GetDataPresent(DataFormats.UnicodeText))
                {
                    buffer = iData.GetData(DataFormats.UnicodeText) as string;
                }
                else if (iData.GetDataPresent(DataFormats.Text))
                {
                    buffer = iData.GetData(DataFormats.Text) as string;
                }
                string UNIQUESTRINGMARKER = ((char)127).ToString();
                if (buffer.IndexOf(UNIQUESTRINGMARKER) != -1)
                {
                    buffer = buffer.Replace(UNIQUESTRINGMARKER, Environment.NewLine);
                }

                if (buffer != null)
                {
                    if (!CurrentCell.IsEditing)
                    {
                        CurrentCell.BeginEdit();
                        if (TextBox != null)
                        {
                            TextBox.SelectAll();
                        }
                    }

                    if (this.HasFocusControl && !DisableTextBox && buffer != null)
                    {
                        SetSelectedText(buffer, true);
                    }
                    else
                    {
                        SetTextBoxText(buffer, true);
                    }

                    return true;
                }
            }

            return false;
        }

        /// <override/>
        /// <summary>Determines whether the current cell can be pasted from clipboard.</summary>
        /// <returns>True if it is allowed.</returns>
        public override bool CanPaste()
        {
            if (TextBox.ReadOnly)
            {
                return false;
            }

            DateTime start = DateTime.Now;
            if (DateTime.Now.Subtract(start).TotalSeconds > 5)
            {
                Application.DoEvents();
                start = DateTime.Now;
            } 
            IDataObject iData = Clipboard.GetDataObject();

            if (iData != null)
            {
                string buffer = null;
                if (iData.GetDataPresent(DataFormats.UnicodeText))
                {
                    buffer = iData.GetData(DataFormats.UnicodeText) as string;
                }
                else if (iData.GetDataPresent(DataFormats.Text))
                {
                    buffer = iData.GetData(DataFormats.Text) as string;
                }

                if (buffer != null)
                {
                    return buffer.IndexOf("\t") == -1 && buffer.IndexOf("\r") == -1;
                }
            }

            return false;
        }

        /// <override/>
        /// <summary>Determines whether the current cell can be cut to clipboard.</summary>
        /// <returns>True if this operation is allowed.</returns>
        public override bool CanCut()
        {
            if (TextBox.ReadOnly)
            {
                return false;
            }

            return true;
        }

        /// <override/>
        /// <summary>Called when user initiates a clipboard cut and works with a single cell only.</summary>
        /// <returns>True if the operation is successful.</returns>
        public override bool Cut()
        {
            if (Copy())
            {
                if (!CurrentCell.IsEditing)
                {
                    CurrentCell.BeginEdit();
                    if (TextBox != null)
                    {
                        TextBox.SelectAll();
                    }
                }

                if (this.HasFocusControl && !DisableTextBox)
                {
                    SetSelectedText(string.Empty, true);
                }
                else
                {
                    SetTextBoxText(string.Empty, true);
                }

                return true;
            }

            return false;
        }

        /// <summary>
        /// Gets or sets a value indicating whether the text box should be hidden. This is useful for derived
        /// cell renderers that want to be able to switch between a edit-mode and
        /// static mode, e.g. a drop-down list does not need to have text input
        /// capabilities.
        /// </summary>
        /// <remarks>
        /// When disabled, you can still call <see cref="TextBoxText"/>
        /// to change the text for the active cell. In this case TextBoxText
        /// will save the text in a member variable and simply draw the text
        /// by itself.
        /// </remarks>
        public bool DisableTextBox
        {
            get
            {
                return disableTextBox;
            }

            set
            {
                if (DisableTextBox == value)
                {
                    return;
                }
#if DEBUG
                if (Switches.CellRenderer.TraceVerbose)
                {
                    TraceUtil.TraceCurrentMethodInfo(value);
                }
#else

                    ;
#endif
                if (this.Initalized)
                {
                    string text = TextBoxText;
                    this.SupportsFocusControl = !value;
                    disableTextBox = value;
                    if (!HasFocusControl)
                    {
                        OnHasFocusControlChanged();
                        this.disabledTextBoxText = text;
                    }
                    else
                    {
                        IGridTextBoxControl itextBox = TextBox as IGridTextBoxControl;
                        itextBox.BeginInit();
                        textBoxControl.Text = text;
                        itextBox.EndInit();
                    }
                }
                else
                {
                    this.SupportsFocusControl = !value;
                    disableTextBox = value;
                }
            }
        }
    }
}