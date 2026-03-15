//-------------------------------------------------------------------------------------------------
// <copyright file="GridDropDownGridListControlCellRenderer.cs" company="syncfusion">
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
using System.Windows.Forms;
using System.Text;

using Syncfusion.Diagnostics;
using Syncfusion.Windows.Forms;
using System.Data;
using System.Collections.Generic;

namespace Syncfusion.Windows.Forms.Grid
{
    /// <summary>
    /// Defines the renderer part of a drop-down ListControl-like grid that lets users drop-down a grid
    /// that can be bound to a <see cref="GridStyleInfo.DataSource"/> of a <see cref="GridStyleInfo"/>
    /// instance and supports auto-complete. Display and value members can be specified with <see cref="GridStyleInfo.ValueMember"/>
    /// and <see cref="GridStyleInfo.DisplayMember"/>.
    /// </summary>
    /// <remarks>
    /// The <see cref="GridDropDownGridListControlCellRenderer"/> supports an autocomplete feature that
    /// will fill the text with possible matches from the drop-down list while the user is entering text.
    /// <para/>
    /// A renderer is created for each <see cref="GridCellModelBase"/>
    /// and <see cref="GridControlBase"/>. There can be several renderers
    /// associated with one <see cref="GridDropDownGridListControlCellModel"/> if several views display the same
    /// <see cref="GridModel"/>.
    /// <para/>
    /// The following table lists some characteristics about the DropDownGridListControl cell type.
    /// <para/>
    /// <list type="table">
    ///     <listheader>
    ///         <term>Item</term>
    ///         <description>Description</description>
    ///     </listheader>
    ///     <item>
    ///         <term>CellType</term>
    ///         <description>GridListControl</description>
    ///     </item>
    ///     <item>
    ///         <term>Renderer</term>
    ///         <description><see cref="GridDropDownGridListControlCellRenderer"/></description>
    ///     </item>
    ///     <item>
    ///         <term>Model</term>
    ///         <description><see cref="GridDropDownGridListControlCellModel"/></description>
    ///     </item>
    ///     <item>
    ///         <term>XP Themes Support</term>
    ///         <description>Yes</description>
    ///     </item>
    ///     <item>
    ///         <term>Cell Button</term>
    ///         <description><see cref="GridCellComboBoxButton"/></description>
    ///     </item>
    ///     <item>
    ///         <term>Interactive</term>
    ///         <description>Edit with Text Input or DropDown</description>
    ///     </item>
    ///     <item>
    ///         <term>Control</term>
    ///         <description><see cref="GridDropDownEditPartControl"/></description>
    ///     </item>
    ///     <item>
    ///         <term>DropDown Control</term>
    ///         <description><see cref="GridListControl"/></description>
    ///     </item>
    ///     <item>
    ///         <term>Floating Support</term>
    ///         <description>No</description>
    ///     </item>
    ///     <item>
    ///         <term>Base Type</term>
    ///         <description><see cref="GridDropDownCellRenderer"/></description>
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
    ///         <description>GridListControl (Default: Text Box)</description>
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
    ///         <term><see cref="GridStyleInfo.Clickable"/> (<see cref="System.Boolean"/>)</term>
    ///         <description>Specifies if the drop-down button can be clicked. If set to False, the button will be drawn grayed out. See <see cref="GridStyleInfo.Enabled"/> how to disable activating the drop-down cell. (Default: True)</description>
    ///     </item>
    ///     <item>
    ///         <term><see cref="GridStyleInfo.CultureInfo"/> (<see cref="System.Globalization.CultureInfo"/>)</term>
    ///         <description>The culture information holds rules for parsing and formatting the cell's value. (Default: NULL)</description>
    ///     </item>
    ///     <item>
    ///         <term><see cref="GridStyleInfo.DataSource"/> (<see cref="System.Object"/>)</term>
    ///         <description>Specifies a data source that holds items to be displayed in a drop-down list. A datasource can be specified instead of manually filling the choicelist with string entries. (Default: NULL)</description>
    ///     </item>
    ///     <item>
    ///         <term><see cref="GridStyleInfo.DisplayMember"/> (<see cref="System.String"/>)</term>
    ///         <description>Names the property in the <see cref="GridStyleInfo.DataSource"/> that holds the text to be displayed in a cell that depends on a <see cref="GridStyleInfo.ValueMember"/>. (Default: String.Empty)</description>
    ///     </item>
    ///     <item>
    ///         <term><see cref="GridStyleInfo.DropDownStyle"/> (<see cref="GridDropDownStyle"/>)</term>
    ///         <description>Specifies if user input is restricted to items from the <see cref="GridStyleInfo.ChoiceList"/> or <see cref="GridStyleInfo.DataSource"/>. (Default: GridDropDownStyle.Editable)</description>
    ///     </item>
    ///     <item>
    ///         <term><see cref="GridStyleInfo.Enabled"/> (<see cref="System.Boolean"/>)</term>
    ///         <description>Specifies if the cell can be activated as current cell or if the cell should be skipped when moving the current cell. When disabled, the drop-down button can still be clicked. You should also disable <see cref="GridStyleInfo.Clickable"/> if you do not want the user to click the drop-down button. (Default: True)</description>
    ///     </item>
    ///     <item>
    ///         <term><see cref="GridStyleInfo.Error"/> (<see cref="System.String"/>)</term>
    ///         <description>Holds error information if a value could not be converted to the <see cref="System.Type"/> specified with <see cref="GridStyleInfo.CellValueType"/>. (Default: String.Empty)</description>
    ///     </item>
    ///     <item>
    ///         <term><see cref="GridStyleInfo.ExclusiveChoiceList"/> (<see cref="System.Boolean"/>)</term>
    ///         <description>Specifies if user input is restricted to items from the <see cref="GridStyleInfo.ChoiceList"/> or <see cref="GridStyleInfo.DataSource"/>. (Default: False)</description>
    ///     </item>
    ///     <item>
    ///         <term><see cref="GridStyleInfo.Font"/> (<see cref="GridFontInfo"/>)</term>
    ///         <description>The font for drawing text. (Default: GridFontInfo.Default)</description>
    ///     </item>
    ///     <item>
    ///         <term><see cref="GridStyleInfo.HorizontalAlignment"/> (<see cref="GridHorizontalAlignment"/>)</term>
    ///         <description>Specifies horizontal alignment of text in the cell. This does not affect the position of the drop-down button. (Default: GridHorizontalAlignment.Left)</description>
    ///     </item>
    ///     <item>
    ///         <term><see cref="GridStyleInfo.HotkeyPrefix"/> (<see cref="System.Drawing.Text.HotkeyPrefix"/>)</term>
    ///         <description>Specifies how hot-key prefixes should be displayed. Hot-keys are indicated in text with an '&amp;' (ampersand). When you enable the hot-key prefix, the specific characters can be displayed underlined or regular. The '&amp;' will not be displayed. (Default: HotkeyPrefix.Show)</description>
    ///     </item>
    ///     <item>
    ///         <term><see cref="GridStyleInfo.ImageIndex"/> (<see cref="System.Int32"/>)</term>
    ///         <description>Specifies an index for an image in the <see cref="GridStyleInfo.ImageList"/> of a <see cref="GridStyleInfo"/>
    /// instance. The image is only shown in the text field, not in the drop-down list.  You have to add custom programming logic in order to set the ImageIndex based on a selection in the drop-down list. (Default: -1)</description>
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
    ///         <term><see cref="GridStyleInfo.ReadOnly"/> (<see cref="System.Boolean"/>)</term>
    ///         <description>Specifies if cell contents can be modified by the user. The user can still drop-down the grid panel but changes will not be saved back into the text field. (Default: False)</description>
    ///     </item>
    ///     <item>
    ///         <term><see cref="GridStyleInfo.ShowButtons"/> (<see cref="GridShowButtons"/>)</term>
    ///         <description>Specifies when to show or display the drop-down button. Possible choices are: show the button only for the current cell, always show buttons, or never show buttons. (Default: GridShowButtons.Show)</description>
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
    ///         <term><see cref="GridStyleInfo.TextAlign"/> (<see cref="GridTextAlign"/>)</term>
    ///         <description>Align text left of button elements (which is typical for combo boxes). Or align text right of button elements. (Default: GridTextAlign.Default)</description>
    ///     </item>
    ///     <item>
    ///         <term><see cref="GridStyleInfo.TextColor"/> (<see cref="System.Drawing.Color"/>)</term>
    ///         <description>Lets you specify the color for drawing the cell text. (Default: SystemColors.WindowText)</description>
    ///     </item>
    ///     <item>
    ///         <term><see cref="GridStyleInfo.TextMargins"/> (<see cref="GridMarginsInfo"/>)</term>
    ///         <description>Holds text margins in pixels. When drawing a cell, this specifies the empty area between the
    /// text rectangle and the borders of the client rectangle of the cell. The client rectangle is the cell rectangle without buttons and borders. (Default: GridMarginsInfo.Default)</description>
    ///     </item>
    ///     <item>
    ///         <term><see cref="GridStyleInfo.Themed"/> (<see cref="System.Boolean"/>)</term>
    ///         <description>Specifies if cell should be drawn using Windows XP themes when <see cref="GridControlBase.ThemesEnabled"/> has been set. (Default: True)</description>
    ///     </item>
    ///     <item>
    ///         <term><see cref="GridStyleInfo.Trimming"/> (<see cref="System.Drawing.StringTrimming"/>)</term>
    ///         <description>Indicates how text is trimmed when it exceeds the edges of the cell text rectangle. (Default: StringTrimming.Character)</description>
    ///     </item>
    ///     <item>
    ///         <term><see cref="GridStyleInfo.ValidateValue"/> (<see cref="GridCellValidateValueInfo"/>)</term>
    ///         <description>Holds validation rules for the cell value that are being checked before any user changes are committed to the grid cells style object. (Default: NULL)</description>
    ///     </item>
    ///     <item>
    ///         <term><see cref="GridStyleInfo.ValueMember"/> (<see cref="System.String"/>)</term>
    ///         <description>Names the property in the datasource that holds the key to be saved in a cell. (Default: String.Empty)</description>
    ///     </item>
    ///     <item>
    ///         <term><see cref="GridStyleInfo.VerticalAlignment"/> (<see cref="GridVerticalAlignment"/>)</term>
    ///         <description>Specifies the vertical alignment of text and the drop-down button in the cell. (Default: GridVerticalAlignment.Top)</description>
    ///     </item>
    ///     <item>
    ///         <term><see cref="GridStyleInfo.WrapText"/> (<see cref="System.Boolean"/>)</term>
    ///         <description>Specifies if text should be wrapped when it does not fit into a single line. (Default: True)</description>
    ///     </item>
    /// </list>
    /// <para/>
    /// </remarks>
    public class GridDropDownGridListControlCellRenderer : GridDropDownCellRenderer
    {
        /// <internalonly/>
        private GridListControl listBoxPart = null;

        /// <internalonly/>
        private string findString = null;

        private bool setupEditPartListeners = false;
        private bool inShowDropDown = false;
        internal bool allowNewEntries = true;

        /// <summary>
        /// Initializes a new GridDropDownGridListControlCellRenderer object for the given GridControlBase
        /// and GridCellModelBase.
        /// </summary>
        /// <param name="grid">The <see cref="GridControlBase"/> that displays this cell renderer.</param>
        /// <param name="cellModel">The <see cref="GridCellModelBase"/> that holds data for this cell renderer that should
        /// be shared among views.</param>
        /// <remarks>References to GridControlBase,
        /// and GridCellModelBase will be saved.</remarks>
        public GridDropDownGridListControlCellRenderer(GridControlBase grid, GridCellModelBase cellModel)
            : base(grid, cellModel)
        {
            DropDownImp.InitFocusEditPart = true;
            DropDownButton = new GridCellComboBoxButton(this);
        }

        /// <summary>
        /// Gets the <see cref="GridDropDownGridListControlCellModel"/> that this cell renderer belongs to.
        /// </summary>
        public new GridDropDownGridListControlCellModel Model
        {
            get
            {
                return (GridDropDownGridListControlCellModel)base.Model;
            }
        }

        /// <summary>
        /// Creates the grid that is displayed in the drop-down window.
        /// </summary>
        /// <returns>A <see cref="GridListControl"/> to be placed in the dropdown container.</returns>
        protected virtual GridListControl CreateListControlPart()
        {
            return new GridDropDownGridListControlPart();
        }

        /// <override/>
        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                if (EditPart != null)
                {
                    EditPart.MouseDown -= new MouseEventHandler(this.OnEditPartMouseDown);
                    EditPart.KeyDown -= new KeyEventHandler(EditPart_KeyDown);
                }

                ResetListControlPart();
            }

            base.Dispose(disposing);
        }

        /// <summary>
        /// Called after <see cref="OnInitialize"/> created <see cref="ListControlPart"/>
        /// </summary>
        protected virtual void OnEnsureListControlPart()
        {
        }

        /// <summary>
        /// Called from <see cref="Dispose(bool)"/> to destroy <see cref="ListControlPart"/>
        /// </summary>
        protected virtual void ResetListControlPart()
        {
            if (this.listBoxPart != null)
            {
                this.listBoxPart.grid.MouseUp -= new MouseEventHandler(this.ListControlMouseUp);
                this.listBoxPart.grid.CurrentCellMoving -= new GridCurrentCellMovingEventHandler(this.ListControlCurrentCellMoving);
                this.listBoxPart.grid.PrepareViewStyleInfo -= new GridPrepareViewStyleInfoEventHandler(ListControlGridPrepareViewStyleInfo);
                this.listBoxPart.grid.CellDrawn -= new GridDrawCellEventHandler(ListControlGridCellDrawn);
                this.listBoxPart.GotFocus -= new EventHandler(this.ListControlGotFocus);
                this.listBoxPart.grid.GotFocus -= new EventHandler(this.ListControlGotFocus);
                this.listBoxPart.grid.VScrollBar.Scroll -= new ScrollEventHandler(this.ListControlScroll);
                this.listBoxPart.Dispose();
                this.listBoxPart = null;
            }
        }

        private void EnsureListControlPart()
        {
            if (this.listBoxPart == null)
            {
                this.listBoxPart = CreateListControlPart();
                this.listBoxPart.BindingContext = new BindingContext();
                this.listBoxPart.grid.BorderStyle = BorderStyle.FixedSingle;
                this.listBoxPart.grid.MouseUp += new MouseEventHandler(this.ListControlMouseUp);
                this.listBoxPart.grid.CurrentCellMoving += new GridCurrentCellMovingEventHandler(this.ListControlCurrentCellMoving);
                this.listBoxPart.GotFocus += new EventHandler(this.ListControlGotFocus);
                this.listBoxPart.grid.GotFocus += new EventHandler(this.ListControlGotFocus);
                this.listBoxPart.grid.VScrollBar.Scroll += new ScrollEventHandler(this.ListControlScroll);
                this.listBoxPart.grid.PrepareViewStyleInfo += new GridPrepareViewStyleInfoEventHandler(ListControlGridPrepareViewStyleInfo);
                this.listBoxPart.grid.CellDrawn += new GridDrawCellEventHandler(ListControlGridCellDrawn);
            }

            if (!this.setupEditPartListeners /*&& !this.DisableTextBox*/ && EditPart != null)
            {
                EditPart.MouseDown += new MouseEventHandler(this.OnEditPartMouseDown);
                EditPart.KeyDown += new KeyEventHandler(EditPart_KeyDown);
                this.setupEditPartListeners = true;
            }

            OnEnsureListControlPart();
        }

        /// <summary>
        /// To return a datatable from an IEnumerable source incase the datasource bound to the dropdown is IEnumerable with no DisplayMember
        /// </summary>
        /// <param name="datasource">datasource bound to the DropDown</param>
        /// <param name="type">Type of datasource items</param>
        /// <returns>datatable</returns>
        private DataTable GetTableFromList(object datasource, Type type)
        {
            DataTable table = new DataTable();
            table.Columns.Add(type.Name, type);
            IEnumerable iEnumList = datasource as IEnumerable;
            foreach (object item in iEnumList)
            {
                DataRow row = table.NewRow();
                row[type.Name] = item;
                table.Rows.Add(row);
            }
            return table;
        }

        /// <summary>
        /// Event handler for PrepareViewStyleInfo event of dropdown table.
        /// </summary>
        /// <param name="sender">Event source.</param>
        /// <param name="e">Event data.</param>
        protected virtual void ListControlGridPrepareViewStyleInfo(object sender, GridPrepareViewStyleInfoEventArgs e)
        {
            if (this.Model.isCombobox)
                e.Style.Borders.Bottom = GridBorder.Empty;
        }

        /// <summary>
        /// Event handler for CellDrawn event of dropdown table.
        /// </summary>
        /// <param name="sender">Event source.</param>
        /// <param name="e">Event data.</param>
        protected virtual void ListControlGridCellDrawn(object sender, GridDrawCellEventArgs e)
        {
        }

        /// <override/>
        protected /*internal*/ override void OnRejectChanges()
        {
            base.OnRejectChanges();
            GridStyleInfo style = Grid.Model[RowIndex, ColIndex];
            this.listBoxPart.Text = style.GetFormattedText(style.CellValue, GridCellBaseTextInfo.CurrentText);
        }

        void ListControlCurrentCellMoving(object sender, GridCurrentCellMovingEventArgs e)
        {
            if (Control.MouseButtons == MouseButtons.Left)
            {
                e.Options |= GridSetCurrentCellOptions.NoSetFocus;
            }
        }

        /// <override/>
        /// <summary>Allows custom formatting of a cell by changing its style object.</summary>
        /// <param name="e">Event data.</param>
        public override void OnPrepareViewStyleInfo(GridPrepareViewStyleInfoEventArgs e)
        {
            if (e.Style.CellValueType == typeof(byte[]))
            {
                // Avoid an exception when setting ListControl.DisplayMember to an Image property.
                e.Style.DropDownStyle = GridDropDownStyle.Exclusive;
                e.Style.DisplayMember = e.Style.ValueMember;
            }

            base.OnPrepareViewStyleInfo(e);
        }

        /// <override/>
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
            lastSetControlText = null;
            this.savedText = string.Empty;

            // check if we need to fill combobox with the choice list
            if (((GridDropDownCellModel)Model).SupportsChoiceList)
            {
                GridStyleInfo style = StyleInfo;
                bool exclusive = style.ExclusiveChoiceList;
                this.DisableTextBox = exclusive && !style.IsAutoComplete();
                this.allowNewEntries = !exclusive;

                EnsureListControlPart();

                // This ensures that the text box does not get focus and user is not able
                // to type text into it.
                if (this.DisableTextBox)
                {
                    this.DropDownContainer.ParentControl = this.Grid;
                }
                else
                {
                    this.DropDownContainer.ParentControl = this.TextBox;
                }

                object dataSource = Model.GetDataSource(style);

                if (dataSource is IEnumerable)
                {
                    TypeConverter tc = TypeDescriptor.GetConverter(TypeDescriptor.GetReflectionType(dataSource));
                    if (tc != null && (tc is EnumConverter || tc is CollectionConverter || tc is TypeConverter) && !(tc is ComponentConverter) && !(style.DataSource is ArrayConverter) 
                        && !(style.DataSource is ArrayList) && string.IsNullOrEmpty(style.DisplayMember) && string.IsNullOrEmpty(style.ValueMember))
                    {
                        DataTable dt = GetTableFromList(dataSource, typeof(object));
                        dataSource = dt;
                        style.DisplayMember = dt.Columns[0].Caption;
                        style.ValueMember = dt.Columns[0].Caption;
                    }
                }

               if (this.listBoxPart.DataSource != null && this.listBoxPart.DataSource != dataSource)
                {
                    this.listBoxPart.DataBindings.Clear();
                    this.listBoxPart.DataSource = null;
                }

                this.listBoxPart.grid.BeginUpdate();
                this.listBoxPart.grid.TableStyle.Font = style.Font;
                this.listBoxPart.grid.RightToLeft = Grid.RightToLeft;
                this.listBoxPart.grid.TableStyle.Interior = style.Interior;
                this.listBoxPart.grid.TableStyle.TextColor = style.TextColor;
                this.listBoxPart.ImageList = style.ImageList;
                this.listBoxPart.AllowResizeColumns = false;
                this.listBoxPart.grid.WantKeys = false;
                this.listBoxPart.grid.ThemesEnabled = Grid.ThemesEnabled && style.Themed;
                listBoxPart.MultiColumn = true;

                this.listBoxPart.grid.Model.Options.GridVisualStylesDrawing = Grid.GetGridWindow().GetGridVisualStylesDrawing();
                this.listBoxPart.grid.Model.Options.GridVisualStyles = Grid.GetGridWindow().GetGridVisualStyles();
                // Fill with Choices.
                this.listBoxPart.SetDataBinding(this.listBoxPart.BindingContext, dataSource, style.DisplayMember, style.ValueMember);

                this.ControlValue = style.CellValue;  // raises Grid.CurrentCellInitializeControlText

                if (!this.Grid.Model.EnableLegacyStyle && this.Grid.Model.EnableGridListControlInComboBox)
                {
                    this.ListControlPart.ShowColumnHeader = false;
                    this.ListControlPart.MultiColumn = false;
                    this.listBoxPart.GridVisualStyles = this.Grid.GetGridVisualStyles();
                    this.listBoxPart.Grid.Properties.DisplayHorzLines = false;
                }

                this.listBoxPart.grid.EndUpdate();
            }

            if (!this.DisableTextBox)
            {
                TextBox.Select(0, 0);
                TextBox.ReadOnly = Grid.Model[rowIndex, colIndex].ReadOnly;
            }
        }

        ////        protected override void OnDeactived(int rowIndex, int colIndex)
        ////        {
        ////            if (this.listBoxPart != null && this.listBoxPart.grid != null)
        ////                this.listBoxPart.grid.CurrentCell.ResetCurrentCellWithoutDeactivate();
        ////
        ////            base.OnDeactived (rowIndex, colIndex);
        ////        }
        ////

        string lastSetControlText = null;

        /// <override/>
        protected override void OnSetControlText(string text)
        {
#if DEBUG
            if (Switches.CellRenderer.TraceVerbose)
            {
                TraceUtil.TraceCurrentMethodInfo(text);
            }
#else
            ;
#endif
            try
            {
                if (!this.InNotifyCurrentCellChangedException
                    && !inTextBoxChanged
                    && this.listBoxPart != null)
                {
                    // Update current selected index in ListBoxPart.
                    if (this.InSetControlValue)
                    {
                        if (listBoxPart.SelectedValue == null || !listBoxPart.SelectedValue.Equals(ControlValue))
                        {
                            listBoxPart.SelectedIndex = Model.FindValue(StyleInfo, ControlValue);
                        }
                    }
                    else
                    {
                        if (lastSetControlText != text)
                        {
                            if (listBoxPart.Text != text)
                            {
                                FindItemExact(text, true, -1, false);
                            }

                            // might be obsolete - review again: this.listBoxPart.Text = text;
                            this.listBoxPart.Text = text;
                        }
                    }
                }

                base.OnSetControlText(text); // Sets TextBoxText
                lastSetControlText = text;
            }
            catch (Exception ex)
            {
                TraceUtil.TraceExceptionCatched(ex);
                if (!ExceptionManager.RaiseExceptionCatched(this, ex))
                {
                    throw;
                }
            }
        }

        ////        ///// <override/>
        ////        public override string ControlText
        ////        {
        ////            get
        ////            {
        ////                return TextBoxText;
        ////            }
        ////            set
        ////            {
        ////                TraceUtil.TraceCurrentMethodInfoIf(Switches.CellRenderer.TraceVerbose, value);
        ////                if (!TextBox.ReadOnly)
        ////                    TextBoxText = value;
        ////                try
        ////                {
        ////                    if (this.listBoxPart != null)
        ////                    {
        ////                        if (value != null && value.Length > 0)
        ////                            this.listBoxPart.Text = value;
        ////                        else if (this.listBoxPart.Items != null && this.listBoxPart.Items.Count > 0)
        ////                            this.listBoxPart.SelectedIndex = 0;
        ////                        SetControlValueSelectedItem(this.listBoxPart.SelectedItem);
        ////                    }
        ////                }
        ////                catch (Exception ex)
        ////                {
        ////                    TraceUtil.TraceExceptionCatched(ex);
        ////                    if (!ExceptionManager.RaiseExceptionCatched(this, ex))
        ////                        throw ex;
        ////                }
        ////            }
        ////        }
        ////
        ////
        ////        void SyncControlValue(int index)
        ////        {
        ////            if (index != -1)
        ////            {
        ////                GridStyleInfo style = StyleInfo;
        ////                if (style.ChoiceList == null || style.ChoiceList.Count == 0)
        ////                {
        ////                    GridDropDownGridListControlCellModel model = (GridDropDownGridListControlCellModel) Model;
        ////                    bool exclusive;
        ////                    model.FillWithChoices(model.ListBox, this.StyleInfo, out exclusive);
        ////                    object value = (index >= 0 && index < model.ListBox.GetItemCount())
        ////                        ? model.ListBox.GetItemValue(index)
        ////                        : null;
        ////                    SetControlValue(value, false);
        ////                }
        ////                else
        ////                    SetControlValue(style.ChoiceList[index], false);
        ////            }
        ////            else
        ////                SetControlValue(ControlText, false);
        ////        }
        ////
        ////        bool raiseCurrentCellChanged = true;
        ////
        ////        ///// <override/>
        ////        protected override void NotifyCurrentCellChanged()
        ////        {
        ////            TraceUtil.TraceCurrentMethodInfoIf(Switches.CellRenderer.TraceVerbose);
        ////            if (raiseCurrentCellChanged)
        ////                CurrentCell.NotifyChanged();
        ////        }
        ////
        ////        internal bool SetComboBoxText(string s, bool validate, int index)
        ////        {
        ////            TextBoxText = s;
        ////            if (validate)
        ////            {
        ////                if (index >= 0 && ListControlPart.Items != null && index < ListControlPart.Items.Count)
        ////                    SetControlValueSelectedItem(this.ListControlPart.Items[index]);
        ////                else
        ////                    SetControlValueSelectedItem(null);
        ////            }
        ////
        ////            return false;
        ////        }
        ////
        ////        internal void SetControlValueSelectedItem(object item)
        ////        {
        ////            object value = item != null ? ListControlPart.GetItemValue(item) : null;
        ////            SetControlValue(value, false);
        ////        }

        /// <summary>Creates a list from DataTable and recreate a table using the list</summary>
        /// <param name="listBoxPart">listcontrolpart</param>
        /// <param name="searchText">The specified text to search from the list.</param>
        /// <param name="dataSource">dataSource of listcontrolpart</param>
        private void GetlistFromTable(GridListControl listBoxPart, string searchText, object dataSource)
        {
            DataTable table = (DataTable)dataSource;
            List<string> list = new List<string>();
            List<string> choiceList = new List<string>();
            string colName = string.Empty;
            if (string.IsNullOrEmpty(listBoxPart.DisplayMember))
            {
                colName = table.Columns[0].Caption;
            }
            else
            {
                 colName = listBoxPart.DisplayMember;
            }

            foreach (DataRow row in table.Rows)
                list.Add(row[colName].ToString());

            foreach (string s in list)
            {
                if (s.ToLowerInvariant().Contains(searchText.ToLowerInvariant()))
                    choiceList.Add(s);
            }

            DataTable Dt = new DataTable();
            DataRow Dr = null;

            Dt.Columns.Add(colName);

            for (int i = 0; i < choiceList.Count; i++)
            {
                Dr = Dt.NewRow();
                Dr[colName] = choiceList[i];
                Dt.Rows.Add(Dr);
            }
            

            listBoxPart.DataSource = null;
            listBoxPart.DataSource = Dt;
            listBoxPart.DisplayMember = colName;
            listBoxPart.Refresh();
        }

        /// <summary>Refresh the Choicelist/</summary>
        /// <param name="searchText">The specified text to search from the choicelist.</param>
        /// <param name="listBoxPart">ListBox which is contain the string collection.</param>
        /// <returns>ListBox part with refreshed values</returns>
        private void RefreshChoices(GridListControl listBoxPart, string searchText)
        {
            if (ListControlPart != null)
            {
                if (this.StyleInfo.ChoiceList != null)
                {
                    List<string> list = new List<string>();
                    string colName = listBoxPart.DisplayMember;

                    foreach (object s1 in listBoxPart.Items)
                        list.Add(s1.ToString());

                    listBoxPart.DataSource = null;
                    listBoxPart.DataSource = list;
                    listBoxPart.DisplayMember = colName;
                }
                listBoxPart.Refresh();
                SetSuggestedChoices(listBoxPart, searchText);
            }
        }
        /// <summary>
        /// Generates the possible choices of the choice list based on the searchText.
        /// </summary>
        /// <param name="listBoxPart">Choice list.</param>
        /// <param name="searchText">Text to be searched.</param>
        internal void SetSuggestedChoices(GridListControl listBoxPart, string searchText)
        {
            if (ListControlPart != null)
            {
                if (StyleInfo.ChoiceList != null)
                {
                    listBoxPart.Items.Clear();
                    foreach (string s in this.StyleInfo.ChoiceList)
                    {
                        if (s.Contains(searchText))
                            listBoxPart.Items.Add(s);
                    }
                }
                else
                {
                    object ds = Model.GetDataSource(this.StyleInfo);
                    if (ds is DataTable)
                    {
                        GetlistFromTable(listBoxPart, searchText, ds);
                    }
                }

                listBoxPart.Height = listBoxPart.Items.Count * listBoxPart.ItemHeight;
                this.DropDownContainer.Size = listBoxPart.Size;
                this.DropDownContainer.PopupHost.Size = this.DropDownContainer.Size;
                listBoxPart.Refresh();
            }
        }


        /// <override/>
        protected override void OnKeyPress(KeyPressEventArgs e)
        {
#if DEBUG
            if (Switches.KeyboardEvents.TraceVerbose)
            {
                TraceUtil.TraceCurrentMethodInfo(e.Handled, e.KeyChar, e.Handled);
            }
#else
            ;
#endif

            GridDropDownGridListControlCellModel model = Model as GridDropDownGridListControlCellModel;

            if (!e.Handled)
            {
                if (!this.HasFocusControl)
                {
                    // Key pressed for the first time
                    if (!Char.IsControl(e.KeyChar) && !IsReadOnly())
                    {
                        if (ListControlPart.Items == null)
                        {
                            object ds = model.GetDataSource(this.StyleInfo);
                            if (ds == null)
                            {
                                base.OnKeyPress(e);
                                return;
                            }

                            ListControlPart.DataSource = ds;
                        }

                        int index;
                        if (this.DisableTextBox)
                        {
                            //// DropdownList mode without editing.
                            string itemText;
                            if (ListControlPart.Items == null)
                            {
                                //// Find entry in DataSource.
                                IList list = Model.GetDataSource(StyleInfo) as IList;
                                ////IList list = (IList) Grid.Model[RowIndex, ColIndex].DataSource;
                                if (list != null)
                                {
                                    int start = this.ListControlPart.FindItemInternal(list, TextBoxText, -1, true, out itemText);
                                    index = this.ListControlPart.FindItemInternal(list, e.KeyChar.ToString(), start, true, out itemText);
                                    if (index > -1)
                                    {
                                        if (this.NotifyCurrentCellChanging())
                                        {
                                            this.SynchronizeDisplayText(index);
                                            this.NotifyCurrentCellChanged();
                                        }
                                    }
                                }
                            }
                            else
                            {
                                // Find entry in ListControlPart.Items.
                                index = FindItem(e.KeyChar.ToString(), true, ListControlPart.SelectedIndex, true);
                                if (index != -1)
                                {
                                    if (this.NotifyCurrentCellChanging())
                                    {
                                        this.SynchronizeDisplayText(index);
                                        this.NotifyCurrentCellChanged();
                                    }
                                }
                            }
                        }
                        else 
                        {
                            // Combo box mode with editing.
                            CurrentCell.BeginEdit();
                            if (ListControlPart.Items == null)
                            {
                                // Find entry in DataSource.
                                string itemText;
                                IList list = Model.GetDataSource(StyleInfo) as IList;
                                if (list != null)
                                {
                                    index = this.ListControlPart.FindItemInternal(list, e.KeyChar.ToString(), ListControlPart.SelectedIndex, true, out itemText);
                                    if (index != -1)
                                    {
                                        this.SynchronizeDisplayText(index);
                                    }
                                    else
                                    {
                                        if (!allowNewEntries)
                                        {
                                            e.Handled = true;
                                            return;
                                        }

                                        SetTextBoxText(e.KeyChar.ToString(), true);
                                    }
                                }
                            }
                            else
                            {
                                // Find entry in ListControlPart.Items.
                                index = FindItem(e.KeyChar.ToString(), false, Math.Max(0, ListControlPart.SelectedIndex) - 1, true);
                                if (index == -1)
                                {
                                    index = FindItem(e.KeyChar.ToString(), false, -1, true);
                                }

                                if (index != -1 && StyleInfo.DropDownStyle != GridDropDownStyle.Editable)
                                {
                                    if (ListControlPart.SelectedIndex != index)
                                    {
                                        this.SynchronizeDisplayText(index);
                                    }
                                }
                                else
                                {
                                    if (!allowNewEntries)
                                    {
                                        e.Handled = true;
                                        return;
                                    }

                                    SetTextBoxText(e.KeyChar.ToString(), true);
                                }
                            }
                            // Select text right after the first character.
                            this.TextBoxControl.Select(1, Math.Max(0, this.TextBoxControl.Text.Length - 1));
                        }

                        e.Handled = true;
                    }
                }
                else
                {
                    // Key pressed after cell has been switched into edit mode.
                    char charCode = (char)(e.KeyChar & 255);
                    int indexFound = 0;
                    int selStart = 0;
                    int selLength = 0;

                    selStart = TextBox.SelectionStart;
                    selLength = TextBox.SelectionLength;

                    if (listBoxPart != null && listBoxPart.SelectedIndex != -1 && this.StyleInfo.DropDownStyle == GridDropDownStyle.Editable
                       && this.StyleInfo.AutoCompleteInEditMode == GridComboSelectionOptions.Both)
                    {
                        listBoxPart.SelectedIndex = -1;
                    }

                    if (listBoxPart != null && IsDroppedDown && this.StyleInfo.DropDownStyle == GridDropDownStyle.Editable
                        && this.StyleInfo.AutoCompleteInEditMode == GridComboSelectionOptions.AutoSuggest)
                    {
                        SetSuggestedChoices(listBoxPart, this.ControlText + e.KeyChar.ToString());
                    }

                    // Pass the e.KeyChar to the IsControl method, not the charCode.
                    if ((!Char.IsControl(e.KeyChar) || charCode == 8) && !IsReadOnly()
                        && (this.StyleInfo.DropDownStyle != GridDropDownStyle.Editable || this.StyleInfo.AutoCompleteInEditMode == GridComboSelectionOptions.AutoComplete))
                    {
                        if (ListControlPart.Items == null)
                        {
                            object ds = model.GetDataSource(this.StyleInfo);
                            if (ds == null)
                            {
                                base.OnKeyPress(e);
                                return;
                            }

                            ListControlPart.DataSource = ds;
                        }

                       

                        if (selStart + selLength == TextBox.Text.Length)
                        {
                            selLength = 0;
                            this.findString = TextBox.Text.Substring(0, selStart);
                            if (charCode == 8)
                            {
                                this.savedText = TextBox.Text;
                                if (this.HasControlValue)
                                {
                                    this.savedValue = ControlValue;
                                }
                                else
                                {
                                    savedValue = null;
                                }

                                // special case when caret is at end of text and text itself can't be edited.
                                bool shorten = true;
                                if (TextBox.SelectionLength == 0 && selStart > 0 && !this.allowNewEntries)
                                {
                                    shorten = false;
                                    selStart--;
                                    selLength++;
                                    this.findString = TextBox.Text.Substring(0, selStart);
                                }

                                if (findString == string.Empty)
                                {
                                    TextBoxText = string.Empty;
                                    e.Handled = true;
                                }
                                else
                                {
                                    if (!this.ValidateString(findString))
                                    {
                                        if (shorten && this.findString.Length > 0)
                                        {
                                            this.findString = this.findString.Substring(0, this.findString.Length - 1);
                                        }

                                        if (this.savedValue != null)
                                        {
                                            this.SetControlValue(savedValue, false);
                                        }

                                        TextBoxText = this.savedText;
                                        selStart = this.findString.Length;
                                        selLength = Math.Max(0, TextBoxText.Length - this.findString.Length);
                                        e.Handled = true;
                                    }
                                    else if (this.Grid.IsWindowless)
                                    {
                                        return;
                                    }
                                }
                            }
                            else
                            {
                                this.findString = this.findString + e.KeyChar;
                                int start = -1;
                                string textBoxText = string.Empty;
                                if (ListControlPart.Items == null)
                                {
                                    IList list = Model.GetDataSource(StyleInfo) as IList;
                                    ////IList list = (IList) Grid.Model[RowIndex, ColIndex].DataSource;

                                    if (list != null)
                                    {
                                        string itemText;
                                        if (TextBoxText.ToLower().StartsWith(findString.ToLower()))
                                        {
                                            textBoxText = TextBoxText;
                                            selLength = Math.Max(0, textBoxText.Length - this.findString.Length);
                                        }
                                        else
                                        {
                                            start = this.ListControlPart.FindItemInternal(list, TextBoxText, -1, true, out itemText);
                                            indexFound = this.ListControlPart.FindItemInternal(list, this.findString, Math.Max(start - 1, -1), false, out itemText);
                                            if (indexFound == -1)
                                            {
                                                indexFound = this.ListControlPart.FindItemInternal(list, this.findString, Math.Max(start - 1, -1), true, out itemText);
                                            }

                                            if (indexFound > -1)
                                            {
                                                textBoxText = this.GetItemText(list[indexFound]);
                                                selLength = Math.Max(0, textBoxText.Length - this.findString.Length);
                                            }
                                            else if (!this.DisableTextBox)
                                            {
                                                if (!allowNewEntries)
                                                {
                                                    e.Handled = true;
                                                    return;
                                                }

                                                if (StyleInfo.AutoCompleteInEditMode != GridComboSelectionOptions.AutoComplete)
                                                    textBoxText = this.findString;
                                            }
                                            // Will set TextBox.SelectedText below - just setting ControlValue now - optimizes TextBox.SelectedText speed.
                                            if (indexFound >= 0)
                                            {
                                                object value = model.GetInternalListBox().GetItemValue(list[indexFound]);
                                                SetControlValue(value, false);
                                            }
                                            else
                                            {
                                                ResetControlValue();
                                            }
                                        }
                                    }
                                }
                                else
                                {
                                    if (TextBoxText.ToLower().StartsWith(findString.ToLower()))
                                    {
                                        textBoxText = TextBoxText;
                                        selLength = Math.Max(0, textBoxText.Length - this.findString.Length);
                                    }
                                    else
                                    {
                                        if (TextBoxText.Length > 0)
                                        {
                                            start = FindItem(TextBoxText, true, -1, false);
                                        }

                                        indexFound = this.FindItem(this.findString, true, start, false);
                                        if (indexFound == -1 && StyleInfo.DropDownStyle != GridDropDownStyle.Editable)
                                        {
                                            indexFound = this.FindItem(this.findString, true, start, true);
                                        }
                                        if ((indexFound >= -1 )&& StyleInfo.DropDownStyle == GridDropDownStyle.Editable && StyleInfo.AutoCompleteInEditMode == GridComboSelectionOptions.AutoComplete)
                                        {
                                            indexFound = this.FindItem(this.findString, true, start, true);
                                            if (indexFound > -1)
                                            {
                                                textBoxText = this.GetItemText(this.ListControlPart.Items[indexFound]);
                                                selLength = Math.Max(0, textBoxText.Length - this.findString.Length);
                                            }
                                        }
                                        if (indexFound > -1 && StyleInfo.DropDownStyle != GridDropDownStyle.Editable)
                                        {
                                            textBoxText = this.GetItemText(this.ListControlPart.Items[indexFound]);
                                            selLength = Math.Max(0, textBoxText.Length - this.findString.Length);
                                        }
                                        else if (!this.DisableTextBox)
                                        {
                                            if (!allowNewEntries)
                                            {
                                                e.Handled = true;
                                                return;
                                            }
                                            if (indexFound == -1)
                                                textBoxText = this.findString;
                                        }
                                        // Will set TextBox.SelectedText below - just setting ControlValue now - optimizes TextBox.SelectedText speed.
                                        if (indexFound >= 0 && StyleInfo.DropDownStyle != GridDropDownStyle.Editable)
                                        {
                                            object value = ListControlPart.GetItemValue(ListControlPart.Items[indexFound]);
                                            SetControlValue(value, false);
                                        }
                                        else
                                        {
                                            ResetControlValue();
                                        }
                                    }
                                }

                                if (this.DisableTextBox)
                                {
                                    throw new InvalidOperationException();
                                }

                                if (textBoxText == this.savedText)
                                {
                                    if (this.HasControlText && ControlText != textBoxText)
                                    {
                                        this.ResetControlText();
                                    }

                                    selStart = this.findString.Length;
                                }
                                else
                                {
                                    if (this.NotifyCurrentCellChanging())
                                    {
                                        this.IgnoreTextBoxChanged = true;
                                        TextBox.SelectedText = textBoxText.Substring(findString.Length - 1);
                                        if (!this.HasControlValue)
                                        {
                                            this.ControlText = textBoxText;
                                        }

                                        if (this.HasControlText && ControlText != textBoxText)
                                        {
                                            this.ResetControlText();
                                        }

                                        selStart = this.findString.Length;
                                        this.IgnoreTextBoxChanged = false;
                                        this.NotifyCurrentCellChanged();
                                    }
                                    else
                                    {
                                        this.ResetControlValue();
                                    }
                                }
                            }

                            e.Handled = true;

                            TextBox.SelectionStart = selStart;
                            TextBox.SelectionLength = selLength;
                        }
                        else
                        {
                            if (charCode != 8)
                            {
                                SetSelectedText(e.KeyChar.ToString(), true);
                            }
                            else
                            {
                                TextBox.SelectedText = string.Empty;
                            }

                            e.Handled = true;
                        }
                    }
                }
            }
            // Don't call base: base.OnKeyPress(e).
        }

        bool inSetTextBoxText = false;
        bool inTextBoxChanged = false;
        int selStart = 0;
        int selLength = -1;
        string savedText = string.Empty;
        object savedValue = null;

        /// <override/>
        protected override void TextBoxChanged(object sender, EventArgs e)
        {
            if (inTextBoxChanged || TextBox.Text == savedText)
            {
                return;
            }

            inTextBoxChanged = true;
            try
            {
                if (!this.ValidateString(TextBox.Text))
                {
                    if (this.savedValue != null)
                    {
                        this.SetControlValue(savedValue, false);
                    }

                    TextBox.Text = savedText;
                    TextBox.Select(selStart, selLength);
                    return;
                }
                else if (!inSetTextBoxText)
                {
                    this.FindItemExact(TextBox.Text, true, 0, false);
                }

                base.TextBoxChanged(sender, e);
            }
            finally
            {
                inTextBoxChanged = false;
            }
        }

        ////        ///// <override/>
        ////        protected /*internal*/ override bool OnValidate()
        ////        {
        ////            TraceUtil.TraceCurrentMethodInfoIf(Switches.CellRenderer.TraceVerbose);
        ////
        ////            if (CurrentCell.IsEditing && CurrentCell.IsModified)
        ////            {
        ////                if (!ValidateString(ControlText))
        ////                    return false;
        ////            }
        ////
        ////            return true; 
        ////        }

        /// <override/>
        /// <summary>
        /// Checks whether the specified text is valid.
        /// </summary>
        /// <param name="text">Text to be validated.</param>
        /// <returns>True if the text is valid; False otherwise.</returns>
        public override bool ValidateString(string text)
        {
#if DEBUG
            if (Switches.KeyboardEvents.TraceVerbose)
            {
                TraceUtil.TraceCurrentMethodInfo(text);
            }
#else
            ;
#endif
            if (text == string.Empty)
            {
                return true;
            }

            if (!this.allowNewEntries && this.ListControlPart.FindStringExact(text) == -1)
            {
                return false;
            }

            ////            try
            ////            {
            ////                if (!CurrentStyle.ApplyFormattedText(text, GridCellBaseTextInfo.Validate))
            ////                {
            ////                    CurrentCell.ErrorMessage = text + " is not a valid entry.";
            ////                    return false;
            ////                }
            ////            }
            ////            catch (Exception ex)
            ////            {
            ////                CurrentCell.ErrorMessage = text + ": " + ex.Message;
            ////                return false;
            ////            }

            //// check if base.ValidateString calls ApplyFormattedText.
            return base.ValidateString(text);
        }

        /// <override/>
        /// <summary>
        /// Lets you customize and redirect the mouse wheel behavior to a cell renderer.
        /// </summary>
        /// <param name="e">The event arguments.</param>
        /// <returns>True if the parent grid should not be scrolled; False if the parent grid should scroll.</returns>
        public override bool ProcessMouseWheel(MouseEventArgs e)
        {
            this.listBoxPart.Grid.MouseControllerDispatcher.ProcessCancelMode();  // Cancel Mouse tracking
            this.listBoxPart.Grid.RaiseMouseWheel(e);
            return this.IsDroppedDown;
        }

        /// <override/>
        /// <summary>
        /// Occurs when the drop down container is about to be shown.
        /// </summary>
        /// <param name="sender">Evnet source.</param>
        /// <param name="e">Event data.</param>
        public override void DropDownContainerShowingDropDown(object sender, CancelEventArgs e)
        {
            inShowDropDown = true;
            GridListControl listBoxPart = ListControlPart;
            if (!this.DisableTextBox)
            {
                listBoxPart.BackColor = this.TextBoxControl.BackColor;
                listBoxPart.ForeColor = this.TextBoxControl.ForeColor;
                listBoxPart.Font = this.TextBoxControl.Font;
                listBoxPart.RightToLeft = Grid.RightToLeft;
            }
            else
            {
                listBoxPart.BackColor = Color.FromArgb(255, StyleInfo.Interior.BackColor);
                listBoxPart.ForeColor = StyleInfo.TextColor;
                listBoxPart.Font = StyleInfo.GdipFont;
                listBoxPart.RightToLeft = Grid.RightToLeft;
            }

            if (this.StyleInfo.DropDownStyle == GridDropDownStyle.Editable && this.StyleInfo.AutoCompleteInEditMode == GridComboSelectionOptions.AutoSuggest)
            {
                SetSuggestedChoices(listBoxPart, this.ControlText);
            }
           
            Size size = new Size(Grid.GetColWidth(ColIndex), 75);

            if (listBoxPart.DataSource == null)
            {
                GridStyleInfo style = StyleInfo;
                object ds = Model.GetDataSource(style);
                if (ds != null)
                {
                    this.listBoxPart.SetDataBinding(this.listBoxPart.BindingContext, ds, style.DisplayMember, style.ValueMember);
                }
            }

            if (listBoxPart.DataSource != null)
            {
                if (string.IsNullOrEmpty(listBoxPart.DisplayMember))
                {
                    GridStyleInfo style = StyleInfo;
                    object ds = Model.GetDataSource(style);
                    if (ds is DataTable)
                    {
                        DataTable table = (DataTable)ds;
                        listBoxPart.DisplayMember = table.Columns[0].Caption;
                    }
                }
                if (listBoxPart.Dock != DockStyle.Fill)
                {
                    listBoxPart.FillLastColumn = false;
                    listBoxPart.Grid.VScroll = false;
                    listBoxPart.Grid.HScroll = false;
                    listBoxPart.Grid.VScrollPixel = listBoxPart.Grid.RowCount < 26;
                    listBoxPart.Grid.HScrollPixel = true;
                    listBoxPart.SetBounds(0, 0, size.Width, size.Height);
                    size = listBoxPart.Size;
                    if (!this.Grid.Model.EnableGridListControlInComboBox && !this.ListControlPart.MultiColumn)
                    {
                        listBoxPart.Size = size;
                    }
                    else
                    {
                        listBoxPart.SetBounds(0, 0, size.Width + SystemInformation.VerticalScrollBarWidth, size.Height + SystemInformation.HorizontalScrollBarHeight);
                    }
                    if (!listBoxPart.Grid.VScroll)
                    {
                        listBoxPart.SetBounds(0, 0, size.Width, size.Height + SystemInformation.HorizontalScrollBarHeight);
                    }

                    listBoxPart.FillLastColumn = true;
                    size = listBoxPart.Size;
                }
                else
                {
                    size = listBoxPart.Size;
                }
            }
            else
            {
                size = listBoxPart.Size;
            }

            GridCurrentCellShowingDropDownEventArgs ce = new GridCurrentCellShowingDropDownEventArgs(size);
            Grid.RaiseCurrentCellShowingDropDown(ce);
            if (ce.Cancel || listBoxPart.DataSource == null)
            {
                e.Cancel = true;
                return;
            }

            if (ce.Size != listBoxPart.Size)
            {
                GridDropDownGridListControlPart lp = listBoxPart as GridDropDownGridListControlPart;
                if (lp != null)
                {
                    bool b = lp.AllowModifySetBoundsCore;
                    lp.AllowModifySetBoundsCore = false;
                    listBoxPart.Size = ce.Size;
                    lp.AllowModifySetBoundsCore = b;
                }
                else
                {
                    listBoxPart.Size = ce.Size;
                }
            }
            if (this.StyleInfo.DropDownStyle == GridDropDownStyle.Editable)
            {

                if (this.StyleInfo.AutoCompleteInEditMode == GridComboSelectionOptions.Both)
                {
                    if (FindItem(TextBoxText, true, -1, true) != -1)
                    {
                        TextBoxText = this.listBoxPart.Text;
                        TextBox.SelectAll();
                    }
                }
                else if (this.StyleInfo.AutoCompleteInEditMode == GridComboSelectionOptions.AutoComplete)
                    listBoxPart.Text = TextBoxText;
                else
                {
                    listBoxPart.Text = TextBoxText;
                    this.DropDownContainer.Size = listBoxPart.Size;
                    return;
                }
            }
            else
                listBoxPart.Text = TextBoxText;

            listBoxPart.Name = "GridListControl";
            listBoxPart.Grid.UpdateStyles(); // Fixes issue with Vista that unwanted scrollbars show up.
            this.DropDownContainer.Size = listBoxPart.Size;
            if (DisableTextBox)
            {
                Hide();
            }
        }

        /// <override/>
        /// <summary>Occurs after the popup child was dropped down and made visible.</summary>
        /// <param name="sender">The event source.</param>
        /// <param name="e">The event data.</param>
        public override void DropDownContainerShowedDropDown(object sender, EventArgs e)
        {
            DropDownContainer.FocusParent();
            GridControl grid = this.ListControlPart.grid;
            grid.CurrentCell.Deactivate(true);
            grid.MouseControllerDispatcher.TrackMouse = grid.RangeInfoToRectangle(GridRangeInfo.Rows(grid.TopRowIndex, grid.RowCount));
            if (listBoxPart.Dock != DockStyle.Fill)
            {
                int sbWidth = grid.VScroll ? SystemInformation.VerticalScrollBarWidth : 0;
                int width = grid.ColWidths.GetTotal(0, grid.ColCount) + 2;
                if (width < grid.Width - sbWidth)
                {
                    grid.ColWidths[grid.ColCount] += grid.Width - sbWidth - width;
                }
            }

            inShowDropDown = false;
            NotifyShowedDropDown();
        }

        /// <override/>
        protected /*internal*/ override void InitializeDropDownContainer()
        {
            if (this.DropDownContainer != null)
            {
                this.DropDownContainer.Controls.Add(this.ListControlPart);
            }
        }

        /// <summary>
        /// Handles the <see cref="Control.MouseUp"/> event for the list box, closes the drop-down.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">Event data.</param>
        public virtual void ListControlMouseUp(object sender, MouseEventArgs e)
        {
            if (this.listBoxPart.SelectedIndex != -1)
            {
                // QA issue 23 fix
                currentCellChangedCalled = false;
                CurrentCell.notifyChangingCalled = false;
                SynchronizeDisplayText(this.listBoxPart.SelectedIndex);
                if (CurrentCell.notifyChangingCalled && !this.currentCellChangedCalled)
                {
                    this.NotifyCurrentCellChanged();
                }
            }

            CurrentCell.CloseDropDown(PopupCloseType.Done);
        }

        bool currentCellChangedCalled = false;  // Helps working around issue that CurrentCellChanged is called twice (see qa defect 23) 

        /// <override/>
        protected override void NotifyCurrentCellChanged()
        {
            currentCellChangedCalled = true;
            base.NotifyCurrentCellChanged();
        }

        /// <summary>
        /// Handles the vertical Scroll event for the dropped-down grid and sets focus to the drop-down.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">Event data.</param>
        public virtual void ListControlScroll(object sender, ScrollEventArgs e)
        {
            DropDownContainer.FocusParent();
        }

        /// <summary>
        /// Handles the <see cref="Control.GotFocus"/> event for the list box, resets mouse tracking.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">Event data.</param>
        public virtual void ListControlGotFocus(object sender, EventArgs e)
        {
            if (!inShowDropDown)
            {
                listBoxPart.grid.MouseControllerDispatcher.ResetTrackMouse();
            }
        }

        /// <override/>
        protected override void OnKeyDown(KeyEventArgs e)
        {
#if DEBUG
            if (Switches.KeyboardEvents.TraceVerbose)
            {
                TraceUtil.TraceCurrentMethodInfo(e.Handled, e.KeyCode, e.Handled);
            }
#else
            ;
#endif
            this.selStart = TextBox.SelectionStart;
            this.selLength = TextBox.SelectionLength;
            this.savedText = TextBox.Text;
            if (this.HasControlValue)
            {
                savedValue = ControlValue;
            }
            else
            {
                savedValue = null;
            }

            base.OnKeyDown(e);

            if (!e.Handled && IsDroppedDown && Control.ModifierKeys == Keys.None)
            {
                int curSel = this.ListControlPart.SelectedIndex;
                int linesVisible = this.ListControlPart.Items != null && this.ListControlPart.Items.Count > 0 ? (this.ListControlPart.Height / this.ListControlPart.GetItemHeight(0)) - 1 : 0;
                switch (e.KeyCode)
                {
                    case Keys.Down:
                        curSel++;
                        e.Handled = true;
                        break;
                    case Keys.Up:
                        curSel = curSel - 1;
                        e.Handled = true;
                        break;
                    case Keys.PageDown:
                        curSel += linesVisible;
                        e.Handled = true;
                        break;
                    case Keys.PageUp:
                        curSel = curSel - linesVisible;
                        e.Handled = true;
                        break;
                    case Keys.End:
                        curSel = this.ListControlPart.Items.Count - 1;
                        e.Handled = true;
                        break;
                    case Keys.Home:
                        curSel = 0;
                        e.Handled = true;
                        break;
                }

                if (curSel != this.ListControlPart.SelectedIndex)
                {
                    if (curSel > this.ListControlPart.Items.Count - 1)
                    {
                        curSel = this.ListControlPart.Items.Count - 1;
                    }

                    if (curSel < 0)
                    {
                        curSel = 0;
                    }

                    this.ListControlPart.SetSelected(curSel, true);
                    SynchronizeDisplayText(curSel);
                }
            }
            else if(IsDroppedDown && Control.ModifierKeys == Keys.None)
            {
                int curSel = this.ListControlPart.SelectedIndex;
                int linesVisible = this.ListControlPart.Items != null && this.ListControlPart.Items.Count > 0 ? (this.ListControlPart.Height / this.ListControlPart.GetItemHeight(0)) - 1 : 0;
                if (this.IsEditing && this.StyleInfo.DropDownStyle == GridDropDownStyle.Editable)
                {
                    if (this.StyleInfo.AutoCompleteInEditMode == GridComboSelectionOptions.Both)
                    {
                        if (!string.IsNullOrEmpty(TextBoxText))
                        {
                            curSel = FindItem(TextBoxText, true, -1, true);
                            TextBoxText = listBoxPart.Items[curSel].ToString();
                            TextBox.SelectAll();
                            e.Handled = true;
                        }
                    }
                    else if (this.StyleInfo.AutoCompleteInEditMode == GridComboSelectionOptions.AutoSuggest)
                    {
                        if (TextBox.SelectionLength > 0)
                        {
                            TextBox.SelectedText = string.Empty;
                        }
                        else if (TextBox.SelectionStart >= 0)
                        {
                            switch (e.KeyCode)
                            {

                                case Keys.Back:
                                    if (this.StyleInfo.AutoCompleteInEditMode != GridComboSelectionOptions.AutoSuggest)
                                    {
                                        if (TextBox.SelectionStart > 0)
                                        {
                                            TextBox.Select(TextBox.SelectionStart - 1, 1);
                                        }
                                    }
                                    TextBox.SelectedText = string.Empty;
                                    break;
                                case Keys.Home:
                                    TextBox.SelectionStart = 0;
                                    TextBox.SelectionLength = 0;
                                    CurrentCell.ScrollInView(GridScrollCurrentCellReason.KeyPress);
                                    break;
                                case Keys.End:
                                    TextBox.SelectionStart = TextBox.Text.Length;
                                    TextBox.SelectionLength = 0;
                                    CurrentCell.ScrollInView(GridScrollCurrentCellReason.KeyPress);
                                    break;
                                case Keys.Left:
                                    TextBox.SelectionStart = TextBox.SelectionStart - 1;
                                    CurrentCell.ScrollInView(GridScrollCurrentCellReason.KeyPress);
                                    break;
                                case Keys.Right:
                                    TextBox.SelectionStart = TextBox.SelectionStart + 1;
                                    CurrentCell.ScrollInView(GridScrollCurrentCellReason.KeyPress);
                                    break;
                                case Keys.Delete:
                                    TextBox.Select(TextBox.SelectionStart, 1);
                                    TextBox.SelectedText = string.Empty;
                                    CurrentCell.ScrollInView(GridScrollCurrentCellReason.KeyPress);
                                    break;
                            }
                        }
                        SetSuggestedChoices(listBoxPart, TextBox.Text);
                        e.Handled = true;
                        this.ignoreWmChar = true;
                    }
                }
            }
            ////            if (!this.allowNewEntries && TextBox is GridOriginalTextBoxControl)
            ////            {
            ////                this.ignoreWmChar = false;////((GridOriginalTextBoxControl) TextBox).ignoreNextBackspace = false;
            ////           }
        }

        /// <summary>
        /// Retrieves the text from the list box index and sets the TextBoxText. Called
        /// when the user presses arrow keys to move selection in dropped list box.
        /// </summary>
        /// <param name="index">List box index.</param>
        protected virtual void SynchronizeDisplayText(int index)
        {
            object item = ListControlPart.Items[index];
            ControlValue = this.listBoxPart.GetItemValue(item);
        }

        /// <summary>
        /// Searches for a given prefix at a starting index in the drop-down list.
        /// </summary>
        /// <param name="prefix">The prefix to search for.</param>
        /// <param name="selectItem">True if found entry should be selected in list box.</param>
        /// <param name="start">The index where to start the search.</param>
        /// <param name="ignoreCase">True if case can be ignored; False if case sensitive.</param>
        /// <returns>The index of the entry that matches the prefix.</returns>
        public virtual int FindItem(string prefix, bool selectItem, int start, bool ignoreCase)
        {
            if (this.ListControlPart.Items == null)
            {
                return -1;
            }

            CultureInfo culture = StyleInfo.GetCulture(true);
            if (ignoreCase)
            {
                prefix = prefix.ToUpper(culture);
            }

            int count = this.ListControlPart.Items.Count;
            for (int i = 0; i < count; i++)
            {
                int index = (i + start + 1) % count;
                string itemText = this.GetItemText(ListControlPart.Items[index]);
                if (itemText.Length >= prefix.Length)
                {
                    itemText = itemText.Substring(0, prefix.Length);
                    if (ignoreCase)
                    {
                        itemText = itemText.ToUpper(culture);
                    }

                    if (prefix == itemText)
                    {
                        if (selectItem && index != this.ListControlPart.SelectedIndex)
                        {
                            this.ListControlPart.SetSelected(index, true);
                        }

                        return index;
                    }
                }
            }

            if (selectItem && this.ListControlPart.SelectedIndex > 0)
            {
                this.ListControlPart.SetSelected(this.ListControlPart.SelectedIndex, false);
            }

            return -1;
        }

        string GetItemText(object item)
        {
            //// I tried to make this virtual since I add problem with Color.ToString in
            //// GridDropDownStandardValuesCellRenderer. However, the Model does also call
            //// ListBox.GetItemText.
            //// Better soultion is to provide a property descriptor and call Converter.ConvertTo
            //// as in GridDropDownStandardValuesCellRenderer.

            return ListControlPart.GetItemText(item);
        }

        /// <summary>
        /// Finds text in the list box.
        /// </summary>
        /// <param name="text">The text (or prefix) to find.</param>
        /// <param name="selectItem">True if you want to select the text in the list box.</param>
        /// <param name="start">The first index to start searching.</param>
        /// <param name="ignoreCase">True if case can be ignored; False if case sensitive.</param>
        /// <returns>The index of the entry that starts with the text; -1 if
        /// no entry could be found.</returns>
        public virtual int FindItemExact(string text, bool selectItem, int start, bool ignoreCase)
        {
            if (this.ListControlPart.Items == null)
            {
                return -1;
            }

            CultureInfo culture = StyleInfo.GetCulture(true);
            if (ignoreCase)
            {
                text = text.ToUpper(culture);
            }

            if (text == string.Empty)
            {
                if (selectItem && -1 != this.ListControlPart.SelectedIndex)
                {
                    this.ListControlPart.SelectedIndex = -1;
                }

                return -1;
            }

            int count = this.ListControlPart.Items.Count;
            for (int i = -1; i < count; i++)
            {
                // Minor optimization to look first at the Selected index since this is likely to be hit a couple of times.
                int index = i == -1 ? this.ListControlPart.SelectedIndex : (i + start + 1) % count;
                if (index != -1)
                {
                    object item = this.ListControlPart.Items[index];
                    string itemText = GetItemText(item);
                    if (itemText.Length == text.Length)
                    {
                        if (ignoreCase)
                        {
                            itemText = itemText.ToUpper(culture);
                        }

                        if (text == itemText)
                        {
                            if (selectItem && index != this.ListControlPart.SelectedIndex)
                            {
                                this.ListControlPart.SetSelected(index, true);
                            }

                            return index;
                        }
                    }
                }
            }

            return -1;
        }

        /// <summary>
        /// Handles the <see cref="Control.MouseDown"/> event for the text box. Selects the next item in the list if the user double-clicks.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">Event data.</param>
        protected virtual void OnEditPartMouseDown(object sender, MouseEventArgs e)
        {
        }

        /// <override/>
        protected override void OnClick(int rowIndex, int colIndex, MouseEventArgs e)
        {
            base.OnClick(rowIndex, colIndex, e);
            if (Environment.TickCount > this.DropDownImp.IgnoreDoubleClickTicks)
            {
                if (this.Grid.CurrentCell.HasCurrentCellAt(rowIndex, colIndex))
                {
                    if (!SupportsFocusControl)
                    {
                        CurrentCell.ShowDropDown();
                    }
                }
            }
        }

        /// <summary>
        /// Closes the DropDown if the CurrentCell is deactivated along with BrowseOnly property
        /// </summary>
        protected override bool OnDeactivating()
        {
            if (this.CurrentCell.IsDroppedDown && this.Grid.Model.BrowseOnly)
                this.CurrentCell.CloseDropDown(PopupCloseType.Deactivated);
            return base.OnDeactivating();
        }

        /// <override/>
        protected override void OnControlDoubleClick(Control control)
        {
            if (!Model.AllowDoubleClickChangeSelectedIndex)
            {
                base.OnControlDoubleClick(control);
                return;
            }

            if (!this.DisableTextBox && this.ListControlPart.Items != null)
            {
                int n = (this.ListControlPart.SelectedIndex + 1) % this.ListControlPart.Items.Count;
                this.ListControlPart.SetSelected(n, true);
                SynchronizeDisplayText(n);
                CurrentCell.BeginEdit();
            }

            base.OnControlDoubleClick(control);
        }

        /// <override/>
        protected override void OnDoubleClick(int rowIndex, int colIndex, MouseEventArgs e)
        {
            if (!Model.AllowDoubleClickChangeSelectedIndex)
            {
                base.OnDoubleClick(rowIndex, colIndex, e);
                return;
            }

            if (this.DisableTextBox && this.ListControlPart.Items != null && this.ListControlPart.Items.Count > 0)
            {
                int n = (this.ListControlPart.SelectedIndex + 1) % this.ListControlPart.Items.Count;
                this.ListControlPart.SetSelected(n, true);
                SynchronizeDisplayText(n);
                CurrentCell.BeginEdit();
            }
        }

        ////        bool inSetTextBoxText = false;
        ////        bool inGlcTextBoxChanged = false;
        ////        int selStart = 0;
        ////        int selLength = -1;
        ////        string savedText = string.Empty;

        ////        ///// <override/>
        ////        protected override void TextBoxChanged(object sender, EventArgs e)
        ////        {
        ////            if (inGlcTextBoxChanged)
        ////                return;
        ////
        ////            inGlcTextBoxChanged = true;
        ////            try
        ////            {
        ////                if (!this.ValidateString(TextBox.Text))
        ////                {
        ////                    TextBox.Text = savedText;
        ////                    TextBox.Select(selStart, selLength);
        ////                }
        ////                else if (!inSetTextBoxText)
        ////                {
        ////                    int index = this.FindItemExact(TextBox.Text, true, 0, false);
        ////                    if (index != -1)
        ////                        SyncControlValue(index);
        ////                    else
        ////                        ControlText = TextBox.Text;
        ////                }
        ////
        ////                base.TextBoxChanged (sender, e);
        ////            }
        ////            finally
        ////            {
        ////                inGlcTextBoxChanged = false;
        ////            }
        ////        }
        ////
        ////        ///// <override/>
        ////        protected override string TextBoxText
        ////        {
        ////            get
        ////            {
        ////                return base.TextBoxText;
        ////            }
        ////            set
        ////            {
        ////                inSetTextBoxText = true;
        ////                try
        ////                {
        ////                    base.TextBoxText = value;
        ////                    int index = this.FindItemExact(value, true, 0, false);
        ////                    SyncControlValue(index);
        ////                }
        ////                finally
        ////                {
        ////                    inSetTextBoxText = false;
        ////                }
        ////            }
        ////        }
        ////
        ////
        ////        ///// <internalonly/>
        ////        [Syncfusion.Documentation.DocumentationExclude()]
        ////        protected string BaseTextBoxText
        ////        {
        ////            get
        ////            {
        ////                return base.TextBoxText;
        ////            }
        ////            set
        ////            {
        ////                base.TextBoxText = value;
        ////            }
        ////        }
        ////
        ////        ///// <override/>
        ////        protected /*internal*/ override bool OnValidate()
        ////        {
        ////            TraceUtil.TraceCurrentMethodInfoIf(Switches.CellRenderer.TraceVerbose);
        ////
        ////            if (CurrentCell.IsEditing && CurrentCell.IsModified)
        ////            {
        ////                if (!ValidateString(ControlText))
        ////                    return false;
        ////            }
        ////
        ////            return true;
        ////        }
        ////
        ////        ///// <override/>
        ////        public override bool ValidateString(string text)
        ////        {
        ////            TraceUtil.TraceCurrentMethodInfoIf(Switches.KeyboardEvents.TraceVerbose, text);
        ////            if (text == string.Empty)
        ////                return true;
        ////
        ////            if (!this.allowNewEntries)
        ////            {
        ////                int indexFound = this.FindItemExact(text, true, 0, false);
        ////                if (indexFound == -1)
        ////                    return false;
        ////            }
        ////            return base.ValidateString(text);
        ////        }

        /// <summary>
        /// Gets the grid that is displayed in the drop-down window.
        /// </summary>
        public GridListControl ListControlPart
        {
            get
            {
                EnsureListControlPart();
                return this.listBoxPart;
            }
        }

        private void EditPart_KeyDown(object sender, KeyEventArgs e)
        {
            this.savedText = TextBox.Text;
        }
    }
}
