//-------------------------------------------------------------------------------------------------
// <copyright file="GridDropDownMonthCalendarCellRenderer.cs" company="syncfusion">
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
using System.Text;

using Syncfusion.Diagnostics;
using Syncfusion.Windows.Forms;

namespace Syncfusion.Windows.Forms.Grid
{
    /// <summary>
    /// Defines the renderer part of a month calendar cell that lets users drop-down a calendar
    /// and select a date or range of dates.
    /// </summary>
    /// <remarks>
    /// <para/>
    /// A renderer is created for each <see cref="GridCellModelBase"/>
    /// and <see cref="GridControlBase"/>. There can be several renderers
    /// associated with one <see cref="GridDropDownMonthCalendarCellModel"/> if several views display the same
    /// <see cref="GridModel"/>.
    ///  <para/>
    /// <para/>
    /// The following table lists some characteristics about the DropDownMonthCalendar cell type:
    /// <para/>
    /// <list type="table">
    ///     <listheader>
    ///         <term>Item</term>
    ///         <description>Description</description>
    ///     </listheader>
    ///     <item>
    ///         <term>CellType</term>
    ///         <description>MonthCalendar</description>
    ///     </item>
    ///     <item>
    ///         <term>Renderer</term>
    ///         <description><see cref="GridDropDownMonthCalendarCellRenderer"/></description>
    ///     </item>
    ///     <item>
    ///         <term>Model</term>
    ///         <description><see cref="GridDropDownMonthCalendarCellModel"/></description>
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
    ///         <description><see cref="MonthCalendar"/></description>
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
    /// The cells behavior and appearance can be customized with the following properties of the <see cref="GridStyleInfo"/> class:
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
    ///         <description>MonthCalendar (Default: Text Box)</description>
    ///     </item>
    ///     <item>
    ///         <term><see cref="GridStyleInfo.CellValue"/> (<see cref="System.Object"/>)</term>
    ///         <description>This property holds the cell value. Although the cell value is typically a string, it can also be any other primitive type such as int, byte, enum, or any custom type that is derived from <see cref="System.Object"/>. (Default: String.Empty)</description>
    ///     </item>
    ///     <item>
    ///         <term><see cref="GridStyleInfo.CellValueType"/> (<see cref="System.Type"/>)</term>
    ///         <description>Specifies the preferred <see cref="System.Type"/> for cell values. When you assign a value
    /// to the <see cref="GridStyleInfo"/> object, the value will be converted to this type. If the
    /// value cannot be converted, <see cref="GridStyleInfo.Error"/> will contain error information. Recommended types for a DropDownMonthCalendar are System.DateTime, System.String, or none. (Default: NULL)</description>
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
    ///         <term><see cref="GridStyleInfo.Enabled"/> (<see cref="System.Boolean"/>)</term>
    ///         <description>Specifies if the cell can be activated as current cell or if cell should be skipped when moving the current cell. When disabled, the drop-down button can still be clicked. You should also disable <see cref="GridStyleInfo.Clickable"/> if you do not want the user to click the drop-down button. (Default: True)</description>
    ///     </item>
    ///     <item>
    ///         <term><see cref="GridStyleInfo.Error"/> (<see cref="System.String"/>)</term>
    ///         <description>Holds error information if a value could not be converted to the <see cref="System.Type"/> specified with <see cref="GridStyleInfo.CellValueType"/>. (Default: String.Empty)</description>
    ///     </item>
    ///     <item>
    ///         <term><see cref="GridStyleInfo.Font"/> (<see cref="GridFontInfo"/>)</term>
    ///         <description>The font for drawing text. (Default: GridFontInfo.Default)</description>
    ///     </item>
    ///     <item>
    ///         <term><see cref="GridStyleInfo.Format"/> (<see cref="System.String"/>)</term>
    ///         <description>Lets you specify a DateTime format when  <see cref="GridStyleInfo.CellValueType"/> has been initialized to "System.DateTime". (Default: String.Empty)</description>
    ///     </item>
    ///     <item>
    ///         <term><see cref="GridStyleInfo.HorizontalAlignment"/> (<see cref="GridHorizontalAlignment"/>)</term>
    ///         <description>Specifies the horizontal alignment of text in the cell. This does not affect the position of the drop-down button. (Default: GridHorizontalAlignment.Left)</description>
    ///     </item>
    ///     <item>
    ///         <term><see cref="GridStyleInfo.ImageIndex"/> (<see cref="System.Int32"/>)</term>
    ///         <description>Specifies an index for an image in the <see cref="GridStyleInfo.ImageList"/> of a <see cref="GridStyleInfo"/>
    /// instance. The image is only shown in the text field, not in the drop-down list. (Default: -1)</description>
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
    ///         <description>Specifies if cell contents can be modified by the user. The user can still drop-down the MonthCalendar panel but changes will not be saved back into the text field. (Default: False)</description>
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
    ///         <description>Specifies if cells should be drawn using Windows XP themes when <see cref="GridControlBase.ThemesEnabled"/> has been set. (Default: True)</description>
    ///     </item>
    ///     <item>
    ///         <term><see cref="GridStyleInfo.Trimming"/> (<see cref="System.Drawing.StringTrimming"/>)</term>
    ///         <description>Indicates how text is trimmed when it exceeds the edges of the cell text rectangle. (Default: StringTrimming.Character)</description>
    ///     </item>
    ///     <item>
    ///         <term><see cref="GridStyleInfo.ValidateValue"/> (<see cref="GridCellValidateValueInfo"/>)</term>
    ///         <description>Holds validation rules for the cell values that are being checked before any user changes are committed to the grid cell's style object. (Default: null)</description>
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
    public class GridDropDownMonthCalendarCellRenderer : GridDropDownCellRenderer
    {
        System.Windows.Forms.MonthCalendar calendar;

        /// <summary>
        /// Initializes a new GridDropDownMonthCalendarCellRenderer object for the given GridControlBase
        /// and GridCellModelBase.
        /// </summary>
        /// <param name="grid">The <see cref="GridControlBase"/> that displays this cell renderer.</param>
        /// <param name="cellModel">The <see cref="GridCellModelBase"/> that holds data for this cell renderer that should
        /// be shared among views.</param>
        /// <remarks>References to GridControlBase, 
        /// and GridCellModelBase will be saved.</remarks>
        public GridDropDownMonthCalendarCellRenderer(GridControlBase grid, GridCellModelBase cellModel)
            : base(grid, cellModel)
        {
            DropDownButton = new GridCellComboBoxButton(this);
            DropDownImp.InitFocusEditPart = true;
        }

#if obs
        protected override GridCellButton CreateChildButton()
        {
            return new GridCellComboBoxButton(this);
        }
#endif

        static Size calendarControlSize = Size.Empty;

        /// <internalonly/>
        /// <summary>Gets CalendarControlSize. Used internally.</summary>
        [Syncfusion.Documentation.DocumentationExclude()]
        public static Size CalendarControlSize
        {
            get
            {
                if (calendarControlSize.IsEmpty)
                {
                    // Determine default size of MonthCalendar when shown in a form.
                    TopLevelWindow f = new TopLevelWindow();
                    f.Location = new Point(10000, 10000);
                    f.Size = new Size(500, 500);
                    f.ShowWindowTopMost();
                    MonthCalendar drawCalendar = new MonthCalendar();
                    f.Controls.Add(drawCalendar);
                    f.Visible = true;
                    calendarControlSize = drawCalendar.Size;
                    f.Dispose();
                }

                return calendarControlSize;
            }
        }

        /// <override/>
        protected /*internal*/ override void InitializeDropDownContainer()
        {
            base.InitializeDropDownContainer();

            calendar = new MonthCalendar();
            calendar.Dock = DockStyle.Fill;
            calendar.Visible = true;
            calendar.DateSelected += new DateRangeEventHandler(CalendarDateSelected);
            calendar.MaxSelectionCount = 1;

            this.DropDownContainer.Controls.Add(calendar);
        }

        // TODO: DropDownClosingDropDown

        /// <override/>
        /// <summary>Called to indicate that the popup child was closed in the specified mode.</summary>
        /// <param name="childUI">Popup child.</param>
        /// <param name="popupCloseType">The way in which the popup was closed.</param>
        public override void ChildClosing(IPopupChild childUI, PopupCloseType popupCloseType)
        {
            if (popupCloseType == PopupCloseType.Done && !IsReadOnly())
            {
               if (!this.NotifyCurrentCellChanging())
                {
                    return;
                }

                DateTime date = calendar.SelectionStart;
                if (StyleInfo.CellValueType == typeof(DateTime))
                {
                    ControlValue = date;
                }
                else
                {
                    ControlValue = date.ToShortDateString();
                }

                TextBox.Select(0, 0);
                TextBox.Modified = true;
                TextBox.Focus();
                // ControlValue already did call this.NotifyCurrentCellChanged();
            }

            base.ChildClosing(childUI, popupCloseType);
        }

        /// <override/>
        protected /*internal*/ override void OnCloseDropDown(PopupCloseType reason)
        {
            base.OnCloseDropDown(reason);
        }

        void CalendarDateSelected(object sender, DateRangeEventArgs date)
        {
            CurrentCell.CloseDropDown(PopupCloseType.Done);
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
        /// <summary>Occurs when the drop down container is about to be shown.</summary>
        /// <param name="sender">Event source.</param>
        /// <param name="e">Event data.</param>
        public override void DropDownContainerShowingDropDown(object sender, CancelEventArgs e)
        {
            Size size = new Size(CalendarControlSize.Width + 2, CalendarControlSize.Height + 2);
            GridCurrentCellShowingDropDownEventArgs ce = new GridCurrentCellShowingDropDownEventArgs(size);
            Grid.RaiseCurrentCellShowingDropDown(ce);
            if (ce.Cancel)
            {
                e.Cancel = true;
                return;
            }

            this.DropDownContainer.Size = ce.Size;

            try
            {
                ////Fix for incident 48853. Format gets reversed when shown in DropDown monthcalendar
                ////UpdateControlValue();

                object cvalue = ControlValue;
                if (cvalue is DateTime)
                {
                    calendar.SetDate((DateTime)cvalue);
                }
                else if (TextBox.Text.Length > 0)
                {
                    calendar.SetDate(DateTime.Parse(TextBox.Text));
                }
                else
                {
                    calendar.SetDate(DateTime.Now);
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
    }
}
