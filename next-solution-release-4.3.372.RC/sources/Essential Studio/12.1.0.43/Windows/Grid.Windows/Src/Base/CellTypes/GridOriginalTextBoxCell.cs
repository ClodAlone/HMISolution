//-------------------------------------------------------------------------------------------------
// <copyright file="GridOriginalTextBoxCell.cs" company="syncfusion">
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
using System.Windows.Forms;
using System.Runtime.Serialization;
using System.Text;
using System.Security;
using System.Security.Permissions;

using Syncfusion.Diagnostics;
using Syncfusion.Windows.Forms;

namespace Syncfusion.Windows.Forms.Grid
{
    /// <summary>
    /// Implements the data / model part for an original text box entry cell.
    /// </summary>
    /// <remarks>
    /// You typically access cell models through the <see cref="GridModel.CellModels"/>
    /// property of the <see cref="GridModel"/> class.<para/>
    /// A <see cref="GridOriginalTextBoxCellModel"/> can serve as model for several <see cref="GridOriginalTextBoxCellRenderer"/>
    /// instances if there are several <see cref="GridControlBase"/> views for a <see cref="GridModel"/>.
    /// <para/>
    /// See <see cref="GridOriginalTextBoxCellRenderer"/> for more detailed information about this cell type.
    /// </remarks>
    [Serializable]
    public class GridOriginalTextBoxCellModel : GridTextBoxCellModel
    {
        /// <overload>
        /// Initializes a new <see cref="GridOriginalTextBoxCellModel"/> object.
        /// </overload>
        /// <summary>
        /// Initializes a new <see cref="GridOriginalTextBoxCellModel"/> object
        /// and stores a reference to the <see cref="GridModel"/> this cell belongs to.
        /// </summary>
        /// <param name="grid">The <see cref="GridModel"/> for this cell model.</param>
        /// <remarks>
        /// You typically access cell models through the <see cref="GridModel.CellModels"/>
        /// property of the <see cref="GridModel"/> class.
        /// </remarks>
        public GridOriginalTextBoxCellModel(GridModel grid)
            : base(grid)
        {
        }

        /// <summary>
        /// Implements the ISerializable interface and returns the data needed to serialize the cell model.
        /// </summary>
        /// <param name="info">A SerializationInfo object containing the information required to serialize the cell model.</param>
        /// <param name="context">A StreamingContext object containing the source and destination of the serialized stream.</param>
        [SecurityPermissionAttribute(SecurityAction.Demand, SerializationFormatter = true)]
        [SecurityPermissionAttribute(SecurityAction.LinkDemand, SerializationFormatter = true)]
        public override void GetObjectData(SerializationInfo info, StreamingContext context)
        {
#if DEBUG
            if (Switches.Serialization.TraceVerbose)
            {
                TraceUtil.TraceCurrentMethodInfo(info.FullTypeName, info.MemberCount);
            }
#else
            ;
#endif
            base.GetObjectData(info, context);
        }

        /// <summary>
        /// Initializes a new <see cref="GridOriginalTextBoxCellModel"/> from a serialization stream.
        /// </summary>
        /// <param name="info">An object that holds all the data needed to serialize or deserialize this instance.</param>
        /// <param name="context">Describes the source and destination of the serialized stream specified by info. </param>
        protected GridOriginalTextBoxCellModel(SerializationInfo info, StreamingContext context)
            : base(info, context)
        {
#if DEBUG
            if (Switches.Serialization.TraceVerbose)
            {
                TraceUtil.TraceCurrentMethodInfo(info.FullTypeName, info.MemberCount);
            }
#else
            ;
#endif
        }

        /// <override/>
        /// <summary>Creates a renderer for this cell model.</summary>
        /// <param name="grid">The grid control.</param>
        /// <returns>Cell renderer.</returns>
        public override GridCellRendererBase CreateRenderer(GridControlBase grid)
        {
            return new GridOriginalTextBoxCellRenderer(grid, this);
        }

        // GetFormattedText moved to base class for 3.0.0.20
    }

    /// <summary>
    /// Implements the renderer part of a text box cell.
    /// </summary>
    /// <remarks>
    /// Use "OriginalTextBox" as identifier in <see cref="GridStyleInfo.CellType"/> of a cells <see cref="GridStyleInfo"/>
    /// to associate this cell type with a cell.
    /// <para/>
    /// The "OriginalTextBox" cell type supports password entry and upper/lower case data entry. See the
    /// <see cref="GridStyleInfo.PasswordChar"/> and <see cref="GridStyleInfo.CharacterCasing"/> properties
    /// of the <see cref="GridStyleInfo"/> class.
    /// <para/>
    /// <para/>
    /// The following table lists some characteristics about the OriginalTextBox cell type:
    /// <para/>
    /// <list type="table">
    ///     <listheader>
    ///         <term>Item</term>
    ///         <description>Description</description>
    ///     </listheader>
    ///     <item>
    ///         <term>CellType</term>
    ///         <description>OriginalTextBox</description>
    ///     </item>
    ///     <item>
    ///         <term>Renderer</term>
    ///         <description><see cref="GridOriginalTextBoxCellRenderer"/></description>
    ///     </item>
    ///     <item>
    ///         <term>Model</term>
    ///         <description><see cref="GridOriginalTextBoxCellModel"/></description>
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
    ///         <description><see cref="TextBox"/></description>
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
    ///         <term><see cref="GridStyleInfo.AllowEnter"/> (<see cref="System.Boolean"/>)</term>
    ///         <description>Gets / sets if pressing the &lt;Enter&gt;-Key should insert a new line into the edited text. (Default: False)</description>
    ///     </item>
    ///     <item>
    ///         <term><see cref="GridStyleInfo.AutoSize"/> (<see cref="System.Boolean"/>)</term>
    ///         <description>Gets / sets if the cell height should automatically increase when the edited text does not fit into the cell and <see cref="GridStyleInfo.WrapText"/> is True. If <see cref="GridStyleInfo.WrapText"/> is False, <see cref="GridStyleInfo.AutoSize"/> will affect the column width. (Default: False)</description>
    ///     </item>
    ///     <item>
    ///         <term><see cref="GridStyleInfo.BackgroundImage"/> (<see cref="System.Drawing.Image"/>)</term>
    ///         <description>Gets / sets the image that the cells display as background. (Default: NULL)</description>
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
    ///         <description>OriginalTextBox (Default: Text Box)</description>
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
    ///         <term><see cref="GridStyleInfo.CharacterCasing"/> (<see cref="System.Windows.Forms.CharacterCasing"/>)</term>
    ///         <description>Specifies if cell control modifies the case of characters as they are typed when the cell's <see cref="GridStyleInfo.CellType"/> is "OriginalTextBox". (Default: CharacterCasing.Normal)</description>
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
    ///         <description> Lets you specify a solid backcolor, gradient, or pattern style with both back and forecolor for a cell's
    ///  background. (Default: SystemColors.Window)</description>
    ///     </item>
    ///     <item>
    ///         <term><see cref="GridStyleInfo.MaxLength"/> (<see cref="System.Int32"/>)</term>
    ///         <description>Limits the number of characters the user can type into the cell. Note: When selecting a text from a choice list or when pasting text, the text can be longer. Additional validation is necessary on your side. (Default: 0)</description>
    ///     </item>
    ///     <item>
    ///         <term><see cref="GridStyleInfo.MergeCell"/> (<see cref="GridMergeCellDirection"/>)</term>
    ///         <description>Specifies merge behavior for an individual cell when merging cells features have been enabled in a <see cref="GridModel"/> with <see cref="GridModelOptions.MergeCellsMode"/>. (Default: GridMergeCellDirection.None)</description>
    ///     </item>
    ///     <item>
    ///         <term><see cref="GridStyleInfo.PasswordChar"/> (<see cref="System.Char"/>)</term>
    ///         <description>The character used to mask characters of a password in a password-entry cell. The cells <see cref="GridStyleInfo.CellType"/> must be "OriginalTextBox". (Default: Blank)</description>
    ///     </item>
    ///     <item>
    ///         <term><see cref="GridStyleInfo.ReadOnly"/> (<see cref="System.Boolean"/>)</term>
    ///         <description>Specifies if cell contents can be modified by the user. You can programmatically change Read-only cells by setting <see cref="GridModel.DiscardReadOnly"/> to True. (Default: False)</description>
    ///     </item>
    ///     <item>
    ///         <term><see cref="GridStyleInfo.StrictValueType"/> (<see cref="System.Boolean"/>)</term>
    ///         <description>Indicates whether an exception should be thrown in the <see cref="GridStyleInfo.ApplyFormattedText(string)"/> method if the formatted text can not be parsed and converted to the type specified with <see cref="GridStyleInfo.CellValueType"/>. (Default: True)</description>
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
    ///     <item>
    ///         <term><see cref="GridStyleInfo.Trimming"/> (<see cref="System.Drawing.StringTrimming"/>)</term>
    ///         <description>Indicates how text is trimmed when it exceeds the edges of the cell text rectangle. (Default: StringTrimming.Character)</description>
    ///     </item>
    ///     <item>
    ///         <term><see cref="GridStyleInfo.ValidateValue"/> (<see cref="GridCellValidateValueInfo"/>)</term>
    ///         <description>Holds validation rules for the cell values that are being checked before any user changes are committed to the grid cell's style object. (Default: NULL)</description>
    ///     </item>
    ///     <item>
    ///         <term><see cref="GridStyleInfo.VerticalAlignment"/> (<see cref="GridVerticalAlignment"/>)</term>
    ///         <description>Specifies vertical alignment of text in the cell. (Default: GridVerticalAlignment.Top)</description>
    ///     </item>
    ///     <item>
    ///         <term><see cref="GridStyleInfo.VerticalScrollbar"/> (<see cref="System.Boolean"/>)</term>
    ///         <description>Specifies if the text box should show a vertical scrollbar when text is being edited and does not fit in cell. WrapText must be initialized to True. (Default: False)</description>
    ///     </item>
    ///     <item>
    ///         <term><see cref="GridStyleInfo.WrapText"/> (<see cref="System.Boolean"/>)</term>
    ///         <description>Specifies if text should be wrapped when it does not fit into a single line. (Default: True)</description>
    ///     </item>
    /// </list>
    /// <para/>
    /// </remarks>
    /// <example>
    /// The following code samples show how to apply PasswordChar and CharacterCasing
    /// <coderef file="d:\syncfusion\essential suite\Grid\Samples\CellTypes\PasswordCells\CS\MainForm.cs" name="Password Cells" lang="C#"><code lang="C#">
    ///             form.Text = "Password Cells";
    /// <para/>
    ///             gridControl1.BeginUpdate();
    ///             gridControl1.TableStyle.FloatCell = true;
    ///             gridControl1.FloatCellsMode = GridFloatCellsMode.OnDemandCalculation;
    ///             gridControl1.TableStyle.BackColor = Color.FromArgb(204, 212, 230);
    ///             gridControl1.TableStyle.CellType = "OriginalTextBox";
    /// <para/>
    ///             GridStyleInfo style;
    /// <para/>
    ///             for (int row = 2; row != 10; row++)
    ///             {
    ///                 style = gridControl1[row, 2];
    ///                 style.PasswordChar = '*';
    ///                 style.Text = new string((char) (65 + row), row);
    /// <para/>
    ///                 style = gridControl1[row, 3];
    ///                 style.CharacterCasing = CharacterCasing.Lower;
    ///                 style.Text = new string((char) (65 + row), row);
    /// <para/>
    ///                 style = gridControl1[row, 4];
    ///                 style.CharacterCasing = CharacterCasing.Upper;
    ///                 style.Text = new string((char) (65 + row), row);
    ///             }
    /// <para/>
    ///             gridControl1.EndUpdate(true);</code></coderef>
    /// <coderef file="d:\syncfusion\essential suite\Grid\Samples\CellTypes\PasswordCells\VB\MainForm.vb" name="Password Cells" lang="VB"><code lang="VB">
    ///        form.Text = "Password Cells"
    /// <para/>
    ///        gridControl1.BeginUpdate()
    ///        gridControl1.TableStyle.FloatCell = True
    ///        gridControl1.FloatCellsMode = GridFloatCellsMode.OnDemandCalculation
    ///        gridControl1.TableStyle.BackColor = Color.FromArgb(204, 212, 230)
    ///        gridControl1.TableStyle.CellType = "OriginalTextBox"
    /// <para/>
    ///        Dim style As GridStyleInfo
    /// <para/>
    ///        Dim row As Integer
    ///        For row = 2 To 10
    ///            style = gridControl1(row, 2)
    ///            style.PasswordChar = "*"c
    ///            style.Text = New String(ChrW(65 + row), row)
    /// <para/>
    ///            style = gridControl1(row, 3)
    ///            style.CharacterCasing = CharacterCasing.Lower
    ///            style.Text = New String(ChrW(65 + row), row)
    /// <para/>
    ///            style = gridControl1(row, 4)
    ///            style.CharacterCasing = CharacterCasing.Upper
    ///            style.Text = New String(ChrW(65 + row), row)
    ///        Next row
    /// <para/>
    ///        gridControl1.EndUpdate(True)</code></coderef>
    /// </example>
    public class GridOriginalTextBoxCellRenderer : GridTextBoxCellRenderer
    {
        GridOriginalTextBoxCellModel pwcModel;
        TextBox originalTextBox;

        /// <summary>
        /// Initializes a new GridOriginalTextBoxCellRenderer object for the given GridControlBase
        /// and GridCellModelBase.
        /// </summary>
        /// <param name="grid">The <see cref="GridControlBase"/> that display this cell renderer.</param>
        /// <param name="cellModel">The <see cref="GridCellModelBase"/> that holds data for this cell renderer that should
        /// be shared among views.</param>
        /// <remarks>References to GridControlBase
        /// and GridCellModelBase will be saved.</remarks>
        public GridOriginalTextBoxCellRenderer(GridControlBase grid, GridCellModelBase cellModel)
            : base(grid, cellModel)
        {
            this.pwcModel = cellModel as GridOriginalTextBoxCellModel;
        }

        /// <summary>
        /// Creates the text box that is shown in-place in the cell when the
        /// user starts editing the cell.
        /// </summary>
        /// <returns>
        /// Returns the new instance of the <see cref="GridTextBoxControl"/>.
        /// </returns>
        /// <override/>
        protected override TextBoxBase CreateTextBox()
        {
            this.originalTextBox = new GridOriginalTextBoxControl(this);
            return this.originalTextBox;
        }

        /// <override/>
        /// <summary>
        /// Gets or sets the active text that is displyed for the current cell.
        /// </summary>
        public override string ControlText
        {
            get
            {
                return base.ControlText;
            }

            set
            {
                switch (this.CurrentStyle.CharacterCasing)
                {
                    case CharacterCasing.Lower:
                        value = value.ToLower();
                        break;
                    case CharacterCasing.Upper:
                        value = value.ToUpper();
                        break;
                }

                base.ControlText = value;
            }
        }

        /// <override/>
        protected override void OnDraw(Graphics g, Rectangle clientRectangle, int rowIndex, int colIndex, GridStyleInfo style)
        {
            // moved to base class for 3.0.0.20
            /*
            if (this.ShouldDrawFocused(rowIndex, colIndex))
            {
                char pwc = style.PasswordChar;
                if (pwc != ' ')
                    this.originalTextBox.PasswordChar = style.PasswordChar;
                else
                    this.originalTextBox.PasswordChar = '\0';
                this.originalTextBox.CharacterCasing = style.CharacterCasing;
            }
            */
            base.OnDraw(g, clientRectangle, rowIndex, colIndex, style);
        }

        ////        /// <override/>
        ////        protected override void InitializeControlText(object controlValue)
        ////        {
        ////            TextBoxText = controlValue != null ? controlValue.ToString() : "";
        ////        }
        ////
        ////        /// <override/>
        ////        protected override void NotifyCurrentCellChanged()
        ////        {
        ////            SetControlValue(TextBoxText, false);
        ////            base.NotifyCurrentCellChanged();
        ////        }
    }
}
