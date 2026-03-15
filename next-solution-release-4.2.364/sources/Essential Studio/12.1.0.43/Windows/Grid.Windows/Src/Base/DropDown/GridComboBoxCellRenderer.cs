//-------------------------------------------------------------------------------------------------
// <copyright file="GridComboBoxCellRenderer.cs" company="syncfusion">
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
    /// Implements the renderer part of a combo box cell.
    /// </summary>
    /// <remarks>
    /// <see cref="GridComboBoxCellRenderer"/> can be customized with
    /// <see cref="GridStyleInfo.DataSource"/>, <see cref="GridStyleInfo.ValueMember"/>,
    /// and <see cref="GridStyleInfo.DisplayMember"/> properties of a <see cref="GridStyleInfo"/> instance.
    /// <para/>
    /// If you do not have a datasource object, you can also fill the drop-down list contents
    /// with a <see cref="GridStyleInfo.ChoiceList"/> and optionally specify <see cref="GridStyleInfo.ExclusiveChoiceList"/>.
    /// <para/>
    /// The combo box cell is XP Themes enabled. It will be drawn themed if <see cref="GridControlBase.ThemesEnabled"/> is True.
    /// <para/>
    /// There can be several renderers
    /// associated with one <see cref="GridComboBoxCellModel"/> if several views display the same
    /// <see cref="GridModel"/>.
    /// <para/>
    /// Use "ComboBox" as identifier in <see cref="GridStyleInfo.CellType"/> of a cells <see cref="GridStyleInfo"/>
    /// to associate this cell type with a cell. sa
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
    ///         <description>Specifies if cell edges should be drawn raised, sunken, or flat (default). (Default: GridCellAppearance.Flat)</description>
    ///     </item>
    ///     <item>
    ///         <term><see cref="GridStyleInfo.CellTipText"/> (<see cref="System.String"/>)</term>
    ///         <description>ToolTip text to be displayed when user hovers mouse over cell. (Default: String.Empty)</description>
    ///     </item>
    ///     <item>
    ///         <term><see cref="GridStyleInfo.CellType"/> (<see cref="System.String"/>)</term>
    ///         <description>Combo Box (Default: Text Box)</description>
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
    ///         <term><see cref="GridStyleInfo.ChoiceList"/> (<see cref="System.Collections.Specialized.StringCollection"/>)</term>
    ///         <description>Specifies items to be displayed in the drop-down list. (Default: NULL)</description>
    ///     </item>
    ///     <item>
    ///         <term><see cref="GridStyleInfo.Clickable"/> (<see cref="System.Boolean"/>)</term>
    ///         <description>Specifies if the combo box button can be clicked. If set to False, the button will be drawn grayed out. See <see cref="GridStyleInfo.Enabled"/> for information on how to disable activating the combo box cell. (Default: True)</description>
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
    ///         <description>Names the property in the datasource that holds the text to be displayed in a cell that depends on a <see cref="GridStyleInfo.ValueMember"/>. (Default: String.Empty)</description>
    ///     </item>
    ///     <item>
    ///         <term><see cref="GridStyleInfo.DropDownStyle"/> (<see cref="GridDropDownStyle"/>)</term>
    ///         <description>Specifies if user input is restricted to items from the <see cref="GridStyleInfo.ChoiceList"/> or <see cref="GridStyleInfo.DataSource"/>. (Default: GridDropDownStyle.Editable)</description>
    ///     </item>
    ///     <item>
    ///         <term><see cref="GridStyleInfo.Enabled"/> (<see cref="System.Boolean"/>)</term>
    ///         <description>Specifies if the cell can be activated as current cell or if cell should be skipped when moving the current cell. When disabled, the combo box button can still be clicked but no drop-down list is displayed. You should also disable <see cref="GridStyleInfo.Clickable"/> if you do not want the user to click the combo box button. (Default: True)</description>
    ///     </item>
    ///     <item>
    ///         <term><see cref="GridStyleInfo.Error"/> (<see cref="System.String"/>)</term>
    ///         <description>Holds error information if a value can not be converted to the <see cref="System.Type"/> specified with <see cref="GridStyleInfo.CellValueType"/>. (Default: String.Empty)</description>
    ///     </item>
    ///     <item>
    ///         <term><see cref="GridStyleInfo.ExclusiveChoiceList"/> (<see cref="System.Boolean"/>)</term>
    ///         <description>Specifies if user input is restricted to items from choice list or datasource. It is recommended to use <see cref="GridStyleInfo.DropDownStyle"/> instead. (Default: False)</description>
    ///     </item>
    ///     <item>
    ///         <term><see cref="GridStyleInfo.Font"/> (<see cref="GridFontInfo"/>)</term>
    ///         <description>The font for drawing text. (Default: GridFontInfo.Default)</description>
    ///     </item>
    ///     <item>
    ///         <term><see cref="GridStyleInfo.HorizontalAlignment"/> (<see cref="GridHorizontalAlignment"/>)</term>
    ///         <description>Specifies horizontal alignment of text in the cell. This does not affect the position of the combo box button. (Default: GridHorizontalAlignment.Left)</description>
    ///     </item>
    ///     <item>
    ///         <term><see cref="GridStyleInfo.ImageIndex"/> (<see cref="System.Int32"/>)</term>
    ///         <description>Specifies an index for a image in the <see cref="GridStyleInfo.ImageList"/> of a <see cref="GridStyleInfo"/>
    /// instance. The image is currently only shown in the text field, not in the drop-down list. You have to add custom programming logic in order to set the ImageIndex based on a selection in the drop-down list. (Default: -1)</description>
    ///     </item>
    ///     <item>
    ///         <term><see cref="GridStyleInfo.Interior"/> (<see cref="Syncfusion.Drawing.BrushInfo"/>)</term>
    ///         <description> Lets you specify a solid backcolor, gradient, or pattern style with both back and forecolor for a cell's
    ///  background. (Default: SystemColors.Window)</description>
    ///     </item>
    ///     <item>
    ///         <term><see cref="GridStyleInfo.ReadOnly"/> (<see cref="System.Boolean"/>)</term>
    ///         <description>Specifies if cell contents can be modified by the user. The user can still drop-down the combo box but changes will not be saved back into the text field. (Default: False)</description>
    ///     </item>
    ///     <item>
    ///         <term><see cref="GridStyleInfo.ShowButtons"/> (<see cref="GridShowButtons"/>)</term>
    ///         <description>Specifies when to show or display the combo box button. Possible choices are: show the button only for the current cell, always show buttons, or never show buttons. (Default: GridShowButtons.Show)</description>
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
    ///         <description>Holds validation rules for the cell value that are being checked before any user changes are committed to the grid cell's style object. (Default: NULL)</description>
    ///     </item>
    ///     <item>
    ///         <term><see cref="GridStyleInfo.ValueMember"/> (<see cref="System.String"/>)</term>
    ///         <description>Names the property in the datasource that holds the key to be saved in a cell. (Default: String.Empty)</description>
    ///     </item>
    ///     <item>
    ///         <term><see cref="GridStyleInfo.VerticalAlignment"/> (<see cref="GridVerticalAlignment"/>)</term>
    ///         <description>Specifies vertical alignment of text in the cell. (Default: GridVerticalAlignment.Top)</description>
    ///     </item>
    ///     <item>
    ///         <term><see cref="GridStyleInfo.WrapText"/> (<see cref="System.Boolean"/>)</term>
    ///         <description>Specifies if text should be wrapped when it does not fit into a single line. (Default: True)</description>
    ///     </item>
    /// </list>
    /// <para/>
    /// </remarks>
    public class GridComboBoxCellRenderer : GridDropDownCellRenderer
    {
        ListBox listBoxPart = null;
        private string findString = null;
        private bool setupEditPartListeners = false;
        bool allowNewEntries = true;

        bool validateStringOnKeyPress = true;

        /// <summary>
        /// Gets or sets a value indicating whether the combobox should raise <see cref="GridControlBase.CurrentCellValidateString"/> notifications
        /// when the user types into the combobox. Default is true.
        /// </summary>
        /// <remarks>
        /// Note: In version 3.3 and earlier the combobox did not raise these notifications. To maintain compatibility
        /// this option allows you to turn off this behavior.
        /// </remarks>
        public bool AllowValidateStringOnKeyPress
        {
            get { return validateStringOnKeyPress; }
            set { validateStringOnKeyPress = value; }
        }

        /// <summary>
        /// Initializes a new <see cref="GridComboBoxCellRenderer"/> object for the given GridControlBase
        /// and <see cref="GridCellModelBase"/>.
        /// </summary>
        /// <param name="grid">The <see cref="GridControlBase"/> that display this cell renderer.</param>
        /// <param name="cellModel">The <see cref="GridComboBoxCellModel"/> that holds data for this cell renderer that should
        /// be shared among views.</param>
        /// <remarks>References to GridControlBase,
        /// and GridCellModelBase will be saved.</remarks>
        public GridComboBoxCellRenderer(GridControlBase grid, GridCellModelBase cellModel)
            : base(grid, cellModel)
        {
            DropDownImp.InitFocusEditPart = true;
            DropDownButton = new GridCellComboBoxButton(this);
        }

        /// <summary>
        /// Gets the <see cref="GridComboBoxCellModel"/> that this cell renderer belongs to.
        /// </summary>
        public new GridComboBoxCellModel Model
        {
            get
            {
                return (GridComboBoxCellModel)base.Model;
            }
        }

        /// <summary>
        /// Creates a <see cref="GridComboBoxListBoxPart"/> that is used as list box inside the drop-down.
        /// </summary>
        /// <returns>A <see cref="GridComboBoxListBoxPart"/> that is derived from <see cref="ListBox"/></returns>
        protected virtual ListBox CreateListBoxPart()
        {
            return new GridComboBoxListBoxPart();
        }

        /// <override/>
        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                if (EditPart != null)
                {
                    EditPart.MouseDown -= new MouseEventHandler(this.OnEditPartMouseDown);
                }

                this.DetachListBoxPart();
            }

            base.Dispose(disposing);
        }

        /// <summary>
        /// Called to detach the list box part from this renderer object.
        /// </summary>
        protected virtual void DetachListBoxPart()
        {
            if (this.listBoxPart != null)
            {
                if (this.DropDownImp != null)
                {
                    this.PopupControlContainer.Controls.Remove(this.listBoxPart);
                }

                this.listBoxPart.MouseUp -= new MouseEventHandler(this.ListBoxMouseUp);
                this.listBoxPart.Click -= new EventHandler(ListBoxClick);
                this.listBoxPart.DataSource = null;
                ////this.listBoxPart.Dispose();
                this.listBoxPart = null;
            }
        }

        /// <summary>
        /// Called to attach a list box part to this renderer object.
        /// </summary>
        protected virtual void AttachListBoxPart()
        {
            if (this.listBoxPart != null)
            {
                // Adding this line b'cos in MenuGrid, we might attach custom list boxes.
                // This is where the list box gets parented and its location gets set.
                this.InitializeDropDownContainer();
                this.listBoxPart.BorderStyle = BorderStyle.FixedSingle;
                this.listBoxPart.MouseUp += new MouseEventHandler(this.ListBoxMouseUp);
                this.listBoxPart.Click += new EventHandler(ListBoxClick);
            }
        }

        private void EnsureListBoxPart()
        {
            if (this.listBoxPart == null)
            {
                this.listBoxPart = CreateListBoxPart();
                this.AttachListBoxPart();
            }

            this.SetupEditPartListeners();
        }

        private void SetupEditPartListeners()
        {
            if (!this.setupEditPartListeners && /*!this.DisableTextBox &&*/ EditPart != null)
            {
                EditPart.MouseDown += new MouseEventHandler(this.OnEditPartMouseDown);
                this.setupEditPartListeners = true;
            }
        }

        /// <override/>
        protected /*internal*/ override void OnRejectChanges()
        {
            base.OnRejectChanges();
            GridStyleInfo style = Grid.Model[RowIndex, ColIndex];
            this.listBoxPart.Text = style.GetFormattedText(style.CellValue, GridCellBaseTextInfo.CurrentText);
        }

        /// <override/>
        /// <summary>
        /// Allows custom formattting of a cell by changing its style object.
        /// </summary>
        /// <param name="e">Event args.</param>
        public override void OnPrepareViewStyleInfo(GridPrepareViewStyleInfoEventArgs e)
        {
            if (e.Style.CellValueType == typeof(byte[]))
            {
                //// Avoid an exception when setting ListControl.DisplayMember to an Image property.
                e.Style.DropDownStyle = GridDropDownStyle.Exclusive;
                e.Style.DisplayMember = e.Style.ValueMember;
            }

            base.OnPrepareViewStyleInfo(e);
        }

        /// <override/>
        /// <summary>Allows the cell renderer to process the mouse wheel.</summary>
        /// <param name="e">A <see cref="MouseEventArgs"/> holding the event data.</param>
        /// <returns>True if the popup child is dropped down and the new scroll position is valid.</returns>
        public override bool ProcessMouseWheel(MouseEventArgs e)
        {
            if (this.IsDroppedDown)
            {
                if (e.Delta < 0)
                {
                    this.ListBoxPart.TopIndex++;
                }
                else
                {
                    this.ListBoxPart.TopIndex--;
                }

                return true;
            }

            return false;
        }

        /// <summary>
        /// Closes the DropDown if the CurrentCell is deactivated along with BrowseOnly property
        /// </summary>
        protected override bool OnDeactivating()
        {
            if (this.CurrentCell.InShowDropDown && this.Grid.Model.BrowseOnly)
                this.CurrentCell.CloseDropDown(PopupCloseType.Deactivated);
            return base.OnDeactivating();
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
            this.savedText = string.Empty;

            GridStyleInfo style = CurrentStyle;

            // Check if we need to fill combo box with the choice list.
            if (((GridDropDownCellModel)Model).SupportsChoiceList)
            {
                EnsureListBoxPart();

                this.listBoxPart.BeginUpdate();

                // Fill with choices.
                bool exclusive;
                ((GridComboBoxCellModel)Model).FillWithChoices(this.ListBoxPart, style, out exclusive);
                this.DisableTextBox = exclusive && !style.IsAutoComplete();
                this.allowNewEntries = !exclusive;
                this.listBoxPart.EndUpdate();

                // This ensures that the text box does not get focus and user is unable
                // to type text into it.
                if (this.DisableTextBox)
                {
                    this.DropDownContainer.ParentControl = this.Grid;
                }
                else
                {
                    this.DropDownContainer.ParentControl = this.TextBox;
                }
            }

            this.ControlValue = style.CellValue;  // raises Grid.CurrentCellInitializeControlText

            if (!this.DisableTextBox)
            {
                TextBox.Select(0, 0);
                TextBox.ReadOnly = Grid.Model[rowIndex, colIndex].ReadOnly;
            }
        }

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
                        if ((listBoxPart.SelectedItem == null && listBoxPart.SelectedValue == null) || (listBoxPart.SelectedValue != null && !listBoxPart.SelectedValue.Equals(ControlValue)))
                        {
                            listBoxPart.SelectedIndex = Model.FindValue(StyleInfo, ControlValue);
                        }
                    }
                    else
                    {
                        if (listBoxPart.Text != text)
                        {
                            FindItemExact(text, true, -1, false);
                        }

                        this.listBoxPart.Text = text;
                    }
                }

                base.OnSetControlText(text); // Sets TextBoxText.

                if (TextBoxText != text)
                {
                    LastOnSetControlTextFailed = true;
                }
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
            GridComboBoxCellModel model = Model as GridComboBoxCellModel;

            if (!e.Handled)
            {
                if (!this.HasFocusControl)
                {
                    //// Key pressed for the first time.
                    if (!Char.IsControl(e.KeyChar) && !IsReadOnly())
                    {
                        if (listBoxPart.Items == null)
                        {
                            object ds = model.GetDataSource(this.StyleInfo);
                            if (ds == null)
                            {
                                base.OnKeyPress(e);
                                return;
                            }

                            listBoxPart.DataSource = ds;
                        }

                        int index;
                        if (this.DisableTextBox)
                        {
                            //// DropdownList mode without editing.
                            index = this.FindItemExact(ControlText, false, -1, false);

                            //// Get next one.
                            index = FindItem(e.KeyChar.ToString(), true, index, true);
                            if (index != -1)
                            {
                                if (this.NotifyCurrentCellChanging())
                                {
                                    this.SynchronizeDisplayText(index);
                                    this.NotifyCurrentCellChanged();
                                }
                            }
                        }
                        else
                        {
                            //// Combo box mode with editing.
                            index = FindItem(e.KeyChar.ToString(), false, Math.Max(0, ListBoxPart.SelectedIndex) - 1, true);
                            if (index == -1)
                            {
                                index = FindItem(e.KeyChar.ToString(), false, -1, true);
                            }

                            if (index == -1)
                            {
                                if (!allowNewEntries
                                    || (validateStringOnKeyPress && !this.ValidateString(e.KeyChar.ToString())))
                                {
                                    e.Handled = true;
                                    return;
                                }

                                CurrentCell.BeginEdit();
                                SetTextBoxText(e.KeyChar.ToString(), true);
                            }
                            else
                            {
                                CurrentCell.BeginEdit();
                                if (listBoxPart.SelectedIndex != index)
                                {
                                    this.SynchronizeDisplayText(index);
                                }
                            }

                            if (this.IsEditing)
                            {
                                this.TextBoxControl.Select(1, Math.Max(0, this.TextBoxControl.Text.Length - 1));
                            }
                        }

                        e.Handled = true;
                    }
                }
                else
                {
                    //// Key pressed after cell has been switched into edit mode.
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
                        if (listBoxPart.Items == null)
                        {
                            object ds = model.GetDataSource(this.StyleInfo);
                            if (ds == null)
                            {
                                base.OnKeyPress(e);
                                return;
                            }

                            listBoxPart.DataSource = ds;
                        }

                        if (selStart + selLength == TextBox.Text.Length)
                        {
                            selLength = 0;
                            this.findString = TextBox.Text.Substring(0, selStart);
                            if (charCode == 8)
                            {
                                if (!this.ValidateString(findString))
                                {
                                    if (this.findString.Length > 0)
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
                            else
                            {
                                this.findString = this.findString + e.KeyChar;
                                string textBoxText = string.Empty;

                                if (TextBoxText.ToLower().StartsWith(findString.ToLower()))
                                {
                                    textBoxText = TextBoxText;
                                    selLength = Math.Max(0, textBoxText.Length - this.findString.Length);
                                }
                                else
                                {
                                    int start = -1;
                                    if (TextBoxText.Length > 0)
                                    {
                                        start = FindItem(TextBoxText, true, -1, false);
                                    }

                                    indexFound = this.FindItem(this.findString, true, start, false);
                                    if (indexFound == -1)
                                    {
                                        indexFound = this.FindItem(this.findString, true, start, true);
                                    }

                                    if (indexFound > -1)
                                    {
                                        textBoxText = this.ListBoxPart.GetItemText(this.ListBoxPart.Items[indexFound]);
                                        selLength = Math.Max(0, textBoxText.Length - this.findString.Length);
                                    }
                                    else if (!this.DisableTextBox)
                                    {
                                        if (!allowNewEntries)
                                        {
                                            e.Handled = true;
                                            return;
                                        }

                                        textBoxText = this.findString;
                                    }

                                    // will set TextBox.SelectedText below - just setting ControlValue now - optimizes TextBox.SelectedText speed.
                                    if (indexFound >= 0)
                                    {
                                        object value = Model.GetItemValue(ListBoxPart.DataSource, ListBoxPart.ValueMember, ListBoxPart.Items[indexFound]);
                                        SetControlValue(value, false);
                                    }
                                    else
                                    {
                                        ResetControlValue();
                                    }
                                }

                                if (this.DisableTextBox)
                                {
                                    throw new InvalidOperationException();
                                }

                                selStart = this.findString.Length;

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
                                    if (!validateStringOnKeyPress || this.ValidateString(textBoxText))
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
            if (this.StyleInfo.DropDownStyle != Syncfusion.Windows.Forms.Grid.GridDropDownStyle.Editable)
                SetSeletionIndex();
            //// Don't call base: base.OnKeyPress(e);
        }

        bool inSetTextBoxText = false;
        bool inTextBoxChanged = false;
        int selStart = 0;
        int selLength = -1;
        string savedText = string.Empty;
        object savedValue = null;

        /// <summary>
        /// Used to add the selection index to the selectionId dictionary collection.
        /// </summary>
        private void SetSeletionIndex()
        {
            if (selectionID.TryGetValue(Grid.CurrentCell.RangeInfo.ToString(), out index))
                this.selectionID.Remove(Grid.CurrentCell.RangeInfo.ToString());
            this.selectionID.Add(Grid.CurrentCell.RangeInfo.ToString(), listBoxPart.SelectedIndex);
        }
        /// <override/>
        protected override void TextBoxChanged(object sender, EventArgs e)
        {
            if (inTextBoxChanged)
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

        /// <override/>
        /// <summary>
        /// Gets or sets the active text that is displayed on the cell.
        /// </summary>
        public override string ControlText
        {
            get
            {
                return base.ControlText;
            }

            set
            {
                base.ControlText = value;
                this.savedText = value;
            }
        }

        ////        /// <override/>
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
        /// <summary>Checks if the specified text is valid.</summary>
        /// <param name="text">The specified text.</param>
        /// <returns>True if it is valid.</returns>
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

            if (!this.validateStringOnKeyPress && text == string.Empty)
            {
                return true;
            }

            if (text != string.Empty && !this.allowNewEntries && this.ListBoxPart.FindStringExact(text) == -1)
            {
                return false;
            }
            if (text != string.Empty&& !this.allowNewEntries &&  this.ListBoxPart.DataSource != null && this.ListBoxPart.FindStringExact(text) == -1)
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

        /// <summary>Refresh the Choicelist/</summary>
        /// <param name="listBoxPart">ChoiceList which is contain the entire string</param>
        /// <param name="searchText">The specified text to search from the choicelist.</param>
        /// <returns>ListBox part with refreshed values</returns>
        private void RefreshChoices(ListBox listBoxPart, string searchText)
        {
            if (ListBoxPart != null)
            {
                if (this.StyleInfo.ChoiceList != null)
                {
                    listBoxPart.Items.Clear();
                    foreach (string item in this.StyleInfo.ChoiceList)
                        listBoxPart.Items.Add(item);
                }
                listBoxPart.Refresh();
                SetSuggestedChoices(listBoxPart, searchText);
            }
        }

        /// <summary>Creates a list from DataTable</summary>
        /// <param name="listBoxPart">listboxpart</param>
        /// <param name="searchText">The specified text to search from the list.</param>
        /// <param name="dataSource">dataSource of listboxpart</param>
        private void GetlistFromTable(ListBox listBoxPart, string searchText, object dataSource)
        {
            DataTable table = (DataTable)dataSource;
            List<string> list = new List<string>();
            string colName = listBoxPart.DisplayMember;

            listBoxPart.DataSource = null;
            listBoxPart.DisplayMember = colName;
            listBoxPart.Items.Clear();

            foreach (DataRow row in table.Rows)
                listBoxPart.Items.Add(row[listBoxPart.DisplayMember].ToString());

            foreach (string s1 in listBoxPart.Items)
                list.Add(s1);

            listBoxPart.Items.Clear();
            foreach (string s in list)
            {
                if (s.Contains(searchText))
                    listBoxPart.Items.Add(s);
            }
        }

        /// <summary>Creates a list from DataTable</summary>
        /// <param name="listBoxPart">listboxpart</param>
        /// <param name="searchText">The specified text to search from the list.</param>
        /// <param name="dataSource">dataSource of listboxpart</param>
        private void GetlistFromArray(ListBox listBoxPart, string searchText, object dataSource)
        {
            ArrayList arrayList = (ArrayList)dataSource;
            IList list = (IList)arrayList;
            List<string> intList = new List<string>();
            List<string> clist = new List<string>();
            string colName = listBoxPart.DisplayMember;

            listBoxPart.DataSource = null;
            listBoxPart.DisplayMember = colName;
            listBoxPart.Items.Clear();

            foreach (object item in list)
                listBoxPart.Items.Add(item);

            foreach (string s1 in listBoxPart.Items)
                clist.Add(s1);

            listBoxPart.Items.Clear();
            foreach (string s in clist)
            {
                if (s.ToLower().Contains(searchText.ToLower()))
                    listBoxPart.Items.Add(s);
            }
        }

        /// <summary>
        /// Generates the possible choices of the choice list based on the searchText.
        /// </summary>
        /// <param name="listBoxPart">Choice list.</param>
        /// <param name="searchText">Text to be searched.</param>
        internal void SetSuggestedChoices(ListBox listBoxPart, string searchText)
        {
            if (ListBoxPart != null)
            {
                if (StyleInfo.ChoiceList != null)
                {
                    listBoxPart.Items.Clear();
                    foreach (string s in this.StyleInfo.ChoiceList)
                    {
                        if (s.ToLowerInvariant().Contains(searchText.ToLowerInvariant()))
                            listBoxPart.Items.Add(s);
                    }
                }
                else
                {
                    object ds = Model.GetDataSource(this.StyleInfo);
                    if (ds is DataTable)
                        GetlistFromTable(listBoxPart, searchText, ds);
                    else if (ds is ArrayList)
                        GetlistFromArray(listBoxPart, searchText, ds);
                }

                listBoxPart.Height = listBoxPart.Items.Count * listBoxPart.ItemHeight;
                this.DropDownContainer.Size = listBoxPart.Size;
                this.DropDownContainer.PopupHost.Size = this.DropDownContainer.Size;
                listBoxPart.Refresh();
            }
        }

        int index;
        private Dictionary<string, int> selectionID = new Dictionary<string, int>();
        /// <summary>Occurs when the drop down is about to be shown.</summary>
        /// <param name="sender">Combobox cell renderer.</param>
        /// <param name="e">Event args.</param>
        /// <override/>
        public override void DropDownContainerShowingDropDown(object sender, CancelEventArgs e)
        {
            ListBox listBoxPart = ListBoxPart;
            if (!this.DisableTextBox && this.IsControlVisible())
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

            if (this.Grid.Model.ReadOnly)
                listBoxPart.BackColor = Color.FromArgb(255, StyleInfo.Interior.BackColor);

            if (this.StyleInfo.DropDownStyle == GridDropDownStyle.Editable && this.StyleInfo.AutoCompleteInEditMode == GridComboSelectionOptions.AutoSuggest)
            {
                SetSuggestedChoices(listBoxPart, this.ControlText);
            }

            Size size = new Size(Grid.GetColWidth(ColIndex), 75);
            GridCurrentCellShowingDropDownEventArgs ce = new GridCurrentCellShowingDropDownEventArgs(size);
            Grid.RaiseCurrentCellShowingDropDown(ce);
            e.Cancel = ce.Cancel;

            if (ce.Cancel || this.ListBoxPart.Items.Count == 0)
            {
                e.Cancel = true;
                return;
            }

            listBoxPart.Size = ce.Size;
            if (this.StyleInfo.DropDownStyle == GridDropDownStyle.Editable)
            {

                if (this.StyleInfo.AutoCompleteInEditMode == GridComboSelectionOptions.Both)
                {
                    if (!string.IsNullOrEmpty(TextBoxText) && FindItem(TextBoxText, true, 0, true) != -1)
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
            if (this.StyleInfo.DropDownStyle != Syncfusion.Windows.Forms.Grid.GridDropDownStyle.Editable)
            {
                if (selectionID.TryGetValue(Grid.CurrentCell.RangeInfo.ToString(), out index))
                    listBoxPart.SelectedIndex = index;
                else
                    this.selectionID.Add(Grid.CurrentCell.RangeInfo.ToString(), listBoxPart.SelectedIndex);
            }
            this.DropDownContainer.Size = listBoxPart.Size;
        }

        /// <override/>
        /// <summary>
        /// Occurs after the pop up has been dropped down and made visible.
        /// </summary>
        /// <param name="sender">Combobox cell renderer.</param>
        /// <param name="e">The event args.</param>
        public override void DropDownContainerShowedDropDown(object sender, EventArgs e)
        {
            DropDownContainer.FocusParent();
            NotifyShowedDropDown();
        }

        /// <override/>
        protected /*internal*/ override void InitializeDropDownContainer()
        {
            if (this.DropDownContainer != null)
            {
                this.DropDownContainer.Controls.Add(this.ListBoxPart);
                // As we keep replacing the ListBoxPart, if necessary, its location should be set.
                this.ListBoxPart.Location = new Point(0, 0);
            }
        }

        /// <summary>
        /// Handles the <see cref="Control.MouseUp"/> event of the list box.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">A <see cref="MouseEventArgs"/> with event data</param>
        protected virtual void ListBoxMouseUp(object sender, MouseEventArgs e)
        {
            if (this.listBoxPart.SelectedIndex != -1)
            {
                if (this.StyleInfo.DropDownStyle != Syncfusion.Windows.Forms.Grid.GridDropDownStyle.Editable)
                    SetSeletionIndex();
                // QA issue 23 fix, issue 383 fix.
                currentCellChangedCalled = false;
                CurrentCell.notifyChangingCalled = false;
                SynchronizeDisplayText(this.listBoxPart.SelectedIndex);
                if (CurrentCell.notifyChangingCalled && !this.currentCellChangedCalled && !this.LastOnSetControlTextFailed)
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
        /// Handles the <see cref="Control.Click"/> event of the list box.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">An <see cref="EventArgs"/> with event data.</param>
        protected virtual void ListBoxClick(object sender, EventArgs e)
        {
            DropDownContainer.FocusParent();
        }

        /// <override/>
        /// <summary>
        /// Occurs when the popup child is being closed.
        /// </summary>
        /// <param name="childUI">The pop up.</param>
        /// <param name="popupCloseType">The way in which the pop up is closed.</param>
        /// <remarks></remarks>
        public override void ChildClosing(IPopupChild childUI, PopupCloseType popupCloseType)
        {
            DropDownContainerCloseDropDown(childUI, new PopupClosedEventArgs(popupCloseType));
        }

        /// <override/>
        protected override void OnKeyDown(KeyEventArgs e)
        {
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

            if (!e.Handled && IsDroppedDown && Control.ModifierKeys == Keys.None && !IsReadOnly())
            {
                int curSel = this.ListBoxPart.SelectedIndex;
                int linesVisible = (this.ListBoxPart.Height / this.ListBoxPart.GetItemHeight(0)) - 1;
                if ((curSel == -1)&& this.IsEditing && this.StyleInfo.DropDownStyle == GridDropDownStyle.Editable)
                {
                    if (this.StyleInfo.AutoCompleteInEditMode == GridComboSelectionOptions.Both)
                    {
                        curSel = FindItem(TextBoxText, true, 0, true);
                        if (curSel == -1) curSel = 0;
                        TextBoxText = listBoxPart.Items[curSel].ToString();
                        TextBox.SelectAll();
                        e.Handled = true;
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
                                    if (TextBox.SelectionStart > 0)
                                    {
                                        TextBox.Select(TextBox.SelectionStart - 1, 1);
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
                        RefreshChoices(listBoxPart, this.ControlText);
                        e.Handled = true;
                        this.ignoreWmChar = true;
                    }
                }
                else
                {
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
                            curSel = this.ListBoxPart.Items.Count - 1;
                            e.Handled = true;
                            break;
                        case Keys.Home:
                            curSel = 0;
                            e.Handled = true;
                            break;
                    }
                }

                if (curSel != this.ListBoxPart.SelectedIndex)
                {
                    if (curSel > this.ListBoxPart.Items.Count - 1)
                    {
                        curSel = this.ListBoxPart.Items.Count - 1;
                    }

                    if (curSel < 0)
                    {
                        curSel = 0;
                    }

                    if (curSel < this.ListBoxPart.Items.Count)
                    {
                        this.ListBoxPart.SetSelected(curSel, true);
                        SynchronizeDisplayText(curSel);
                    }
                }
            }
        }

        /// <summary>
        /// Retrieves the text from the list box index and sets the TextBoxText. Called
        /// when the user presses arrow keys to move selection in dropped list box.
        /// </summary>
        /// <param name="index">The list box index</param>
        protected virtual void SynchronizeDisplayText(int index)
        {
            object item = ListBoxPart.Items[index];
            if (validateStringOnKeyPress)
            {
                object obj = Model.GetItemValue(ListBoxPart.DataSource, ListBoxPart.DisplayMember, item);
                string s = obj != null ? obj.ToString() : string.Empty;
                if (!this.ValidateString(s))
                {
                    return;
                }
            }
            ControlValue = Model.GetItemValue(ListBoxPart.DataSource, ListBoxPart.ValueMember, item);
        }

        /*
                bool SetComboBoxText(string s, bool validate, int index)
                {
                    if (validate && !ValidateString(s))
                        return false;
                    TextBoxText = s;
                    return true;
                }*/

        /// <summary>
        /// Finds text in the list box.
        /// </summary>
        /// <param name="prefix">The text (or prefix) to find.</param>
        /// <param name="selectItem">True if you want to select the text in the list box.</param>
        /// <param name="start">The first index to start searching.</param>
        /// <param name="ignoreCase">True if case can be ignored; False if case sensitive.</param>
        /// <returns>The index of the entry that starts with the text; -1 if
        /// no entry could be found.</returns>
        public virtual int FindItem(string prefix, bool selectItem, int start, bool ignoreCase)
        {
            if (this.ListBoxPart.Items == null)
            {
                return -1;
            }

            CultureInfo culture = StyleInfo.GetCulture(true);
            if (ignoreCase)
            {
                prefix = prefix.ToUpper(culture);
            }

            int count = this.ListBoxPart.Items.Count;
            for (int i = 0; i < count; i++)
            {
                int index = (i + start + 1) % count;
                string itemText = ListBoxPart.GetItemText(ListBoxPart.Items[index]);
                if (itemText.Length >= prefix.Length)
                {
                    itemText = itemText.Substring(0, prefix.Length);
                    if (ignoreCase)
                    {
                        itemText = itemText.ToUpper(culture);
                    }

                    if (prefix == itemText)
                    {
                        if (selectItem && index != this.ListBoxPart.SelectedIndex)
                        {
                            this.ListBoxPart.SetSelected(index, true);
                        }

                        return index;
                    }
                }
            }

            if (selectItem && this.ListBoxPart.SelectedIndex > 0)
            {
                this.ListBoxPart.SetSelected(this.ListBoxPart.SelectedIndex, false);
            }

            return -1;
        }

        string GetItemText(object item)
        {
            //// I tried to make this virtual since I add problem with Color.ToString in
            //// GridDropDownStandardValuesCellRenderer. However, the Model does also call
            //// ListBox.GetItemText.
            //// Better solution is to provide a property descriptor and call Converter.ConvertTo
            //// as in GridDropDownStandardValuesCellRenderer.

            return ListBoxPart.GetItemText(item);
        }

        /// <summary>
        /// Finds text in the list box.
        /// </summary>
        /// <param name="text">The text (or prefix) to find.</param>
        /// <param name="selectItem">True if you want to select the text in the list box.</param>
        /// <param name="start">The first index to start searching.</param>
        /// <param name="ignoreCase">True if case can be ignored; False if case sensitive</param>
        /// <returns>The index of the entry that starts with the text; -1 if
        /// no entry could be found.</returns>
        public virtual int FindItemExact(string text, bool selectItem, int start, bool ignoreCase)
        {
            if (this.ListBoxPart.Items == null)
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
                if (selectItem && -1 != this.ListBoxPart.SelectedIndex)
                {
                    this.ListBoxPart.SelectedIndex = -1;
                }

                return -1;
            }

            int count = this.ListBoxPart.Items.Count;
            for (int i = -1; i < count; i++)
            {
                // Minor optimization to look first at the selected index since this is likely to be hit a couple of times.
                int index = i == -1 ? this.ListBoxPart.SelectedIndex : (i + start + 1) % count;
                if (index != -1)
                {
                    object item = this.ListBoxPart.Items[index];
                    string itemText = GetItemText(item);
                    if (itemText.Length == text.Length)
                    {
                        if (ignoreCase)
                        {
                            itemText = itemText.ToUpper(culture);
                        }

                        if (text == itemText)
                        {
                            if (selectItem && index != this.ListBoxPart.SelectedIndex)
                            {
                                this.ListBoxPart.SetSelected(index, true);
                            }

                            return index;
                        }
                    }
                }
            }

            return -1;
        }

        /// <summary>
        /// Handles the <see cref="Control.MouseDown"/> event of the active text box.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">An <see cref="MouseEventArgs"/> with event data.</param>
        protected virtual void OnEditPartMouseDown(object sender, MouseEventArgs e)
        {
            if (e.Clicks == 2 && Model.AllowDoubleClickChangeSelectedIndex && this.ListBoxPart.Items.Count > 0)
            {
                int n = (this.ListBoxPart.SelectedIndex + 1) % this.ListBoxPart.Items.Count;
                this.ListBoxPart.SetSelected(n, true);
                ControlText = ListBoxPart.GetItemText(this.ListBoxPart.Items[n]);
            }
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
                        if (this.Grid.Model.Options.ActivateCurrentCellBehavior != GridCellActivateAction.DblClickOnCell)
                            CurrentCell.ShowDropDown();
                    }
                }
            }
        }

        /// <override/>
        protected override void OnDoubleClick(int rowIndex, int colIndex, MouseEventArgs e)
        {
            if (!SupportsFocusControl && !this.Grid.CurrentCell.IsEditing && this.Grid.Model.Options.ActivateCurrentCellBehavior == GridCellActivateAction.DblClickOnCell)
            {
                CurrentCell.ShowDropDown();
            }

            if (!Model.AllowDoubleClickChangeSelectedIndex)
            {
                base.OnDoubleClick(rowIndex, colIndex, e);
                return;
            }

            if (this.DisableTextBox && this.ListBoxPart.Items != null && this.ListBoxPart.Items.Count > 0)
            {
                int n = (this.ListBoxPart.SelectedIndex + 1) % this.ListBoxPart.Items.Count;
                this.ListBoxPart.SetSelected(n, true);
                ControlText = ListBoxPart.GetItemText(this.ListBoxPart.Items[n]);
                CurrentCell.BeginEdit();
            }
        }

        //// Properties

        /// <summary>
        /// Gets or sets the <see cref="ListBox"/> that is displayed when the user drops-down the combo box.
        /// </summary>
        public ListBox ListBoxPart
        {
            get
            {
                EnsureListBoxPart();
                return this.listBoxPart;
            }

            set
            {
                if (this.listBoxPart != value)
                {
                    if (this.listBoxPart != null)
                    {
                        this.DetachListBoxPart();
                    }

                    this.listBoxPart = value;

                    if (this.listBoxPart != null)
                    {
                        this.AttachListBoxPart();
                        this.SetupEditPartListeners();
                    }
                }
            }
        }
    }
}
