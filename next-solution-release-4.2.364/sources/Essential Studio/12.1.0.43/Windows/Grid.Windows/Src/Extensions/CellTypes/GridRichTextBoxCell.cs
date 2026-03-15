//-------------------------------------------------------------------------------------------------
// <copyright file="GridRichTextBoxCell.cs" company="syncfusion">
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
using System.Runtime.InteropServices;
using System.Runtime.Serialization;

using Syncfusion.Diagnostics;
using Syncfusion.Windows.Forms;
using Syncfusion.ComponentModel;
using Syncfusion.Drawing;

namespace Syncfusion.Windows.Forms.Grid
{
    /// <summary>
    /// Implements the data / model part for a rich text cell.
    /// </summary>
    /// <remarks>
    /// You typically access cell models through the <see cref="GridModel.CellModels"/>
    /// property of the <see cref="GridModel"/> class.<para/>
    /// A <see cref="GridRichTextBoxCellModel"/> can serve as model for several <see cref="GridRichTextBoxCellRenderer"/>
    /// instances if there are several <see cref="GridControlBase"/> views for a <see cref="GridModel"/>.
    /// <para/>
    /// See <see cref="GridRichTextBoxCellRenderer"/> for more detailed information about this cell type.
    /// </remarks>
    [Serializable]
    public class GridRichTextBoxCellModel : GridCellModelBase
    {
        /// <overload>
        /// Initializes a new <see cref="GridRichTextBoxCellModel"/> object.
        /// </overload>
        /// <summary>
        /// Initializes a new <see cref="GridRichTextBoxCellModel"/> object 
        /// and stores a reference to the <see cref="GridModel"/> this cell belongs to.
        /// </summary>
        /// <param name="grid">The <see cref="GridModel"/> for this cell model.</param>    
        /// <remarks>
        /// You typically access cell models through the <see cref="GridModel.CellModels"/>
        /// property of the <see cref="GridModel"/> class.
        /// </remarks>
        public GridRichTextBoxCellModel(GridModel grid)
            : base(grid)
        {
            ButtonBarSize = new Size(21, 0);
        }

        /// <summary>
        /// Initializes a new <see cref="GridRichTextBoxCellModel"/> from a serialization stream.
        /// </summary>
        /// <param name="info">An object that holds all the data needed to serialize or deserialize this instance.</param>
        /// <param name="context">Describes the source and destination of the serialized stream specified by info. </param>
        protected GridRichTextBoxCellModel(SerializationInfo info, StreamingContext context)
            : base(info, context)
        {
            ////            TraceUtil.TraceCurrentMethodInfoIf(true, info.FullTypeName, info.MemberCount);
            ButtonBarSize = new Size(21, 0);
        }

        /// <override/>
        /// <summary>
        /// Creates a renderer for this cell model that is specific to the GridControlBase.
        /// </summary>
        /// <param name="control">The <see cref="GridControlBase"/> the cell renderer is created for.</param>
        /// <returns>A new <see cref="GridRichTextBoxCellRenderer"/> specific for a <see cref="GridControlBase"/>.</returns>
        /// <remarks>You must override this method in your implementation of GridCellModelBase.</remarks>
        public override GridCellRendererBase CreateRenderer(GridControlBase control)
        {
            return new GridRichTextBoxCellRenderer(control, this);
        }

        RichTextBox rtb = new RichTextBox();

        /// <override/>
        /// <summary>
        /// This is called from GridStyleInfo.GetFormattedText.
        /// GridStyleInfo.CultureInfo is used for conversion to string.
        /// </summary>
        /// <param name="style">The <see cref="GridStyleInfo"/> object that holds cell information.</param>
        /// <param name="value">The value to format.</param>
        /// <param name="textInfo">TextInfo is a hint of who is calling, default is GridCellBaseTextInfo.DisplayText.</param>
        /// <returns>The formatted text for the given value.</returns>
        public override string GetFormattedText(GridStyleInfo style, object value, int textInfo)
        {
            string rtf = base.GetFormattedText(style, value, textInfo);
            if (RichTextPaint.IsValidRtf(rtf))
            {
                rtb.CreateControl();
                rtb.Rtf = rtf;
                return rtb.Text;
            }

            return rtf;
        }

        /// <override/>
        /// <summary>
        /// Parses the display text and converts it into a cell value to be stored in the style object.
        /// GridStyleInfo.CultureInfo is used for parsing the string.
        /// </summary>
        /// <param name="style">The <see cref="GridStyleInfo"/> object that holds cell information.</param>
        /// <param name="text">The input text to be parsed.</param>
        /// <param name="textInfo">TextInfo is a hint of who is calling, default is GridCellBaseTextInfo.DisplayText</param>
        /// <returns>True if value was parsed correctly and saved in style object as <see cref="GridStyleInfo.CellValue"/>; False otherwise.</returns>
        public override bool ApplyFormattedText(GridStyleInfo style, string text, int textInfo)
        {
            return base.ApplyFormattedText(style, text, textInfo);
        }
    }

    /// <summary>
    /// Implements the renderer part of a rich text cell.
    /// </summary>
    /// <remarks>
    /// Use "RichText" as identifier in CellType of a cells <see cref="GridStyleInfo"/>
    ///  to associate this cell type with a cell.
    /// <para/>
    /// This renderer supports editing the contents of the rich text with a drop-down
    /// panel. When the user drops the panel, a <see cref="GridRichTextEntryPanel"/> is shown
    /// and the user can format the text and then accept changes by pressing "Save" button.
    /// <para/>
    /// The following table lists some characteristics about the RichText cell type.
    /// <para/>
    /// <list type="table">
    ///     <listheader>
    ///         <term>Item</term>
    ///         <description>Description</description>
    ///     </listheader>
    ///     <item>
    ///         <term>CellType</term>
    ///         <description>RichText</description>
    ///     </item>
    ///     <item>
    ///         <term>Renderer</term>
    ///         <description><see cref="GridRichTextBoxCellRenderer"/></description>
    ///     </item>
    ///     <item>
    ///         <term>Model</term>
    ///         <description><see cref="GridRichTextBoxCellModel"/></description>
    ///     </item>
    ///     <item>
    ///         <term>XP Themes Support</term>
    ///         <description>Yes</description>
    ///     </item>
    ///     <item>
    ///         <term>Interactive</term>
    ///         <description>Edit with DropDown Panel</description>
    ///     </item>
    ///     <item>
    ///         <term>DropDown Control</term>
    ///         <description><see cref="GridRichTextEntryPanel"/></description>
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
    /// The cell's behavior and appearance can be customized with the following properties of the <see cref="GridStyleInfo"/> class.
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
    ///         <description>RichText (Default: TextBox)</description>
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
    ///         <description>Specifies if the user can click on any cell button elements in this renderer. (Default: True)</description>
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
    ///         <term><see cref="GridStyleInfo.Interior"/> (<see cref="Syncfusion.Drawing.BrushInfo"/>)</term>
    ///         <description> Lets you specify a solid backcolor, gradient, or pattern style with both back and forecolor for a cell's
    ///  background. (Default: SystemColors.Window)</description>
    ///     </item>
    ///     <item>
    ///         <term><see cref="GridStyleInfo.ReadOnly"/> (<see cref="System.Boolean"/>)</term>
    ///         <description>Specifies if cell contents can be modified by the user. You can programmatically change Read-only cells by setting <see cref="GridModel.DiscardReadOnly"/> to True. (Default: False)</description>
    ///     </item>
    ///     <item>
    ///         <term><see cref="GridStyleInfo.ShowButtons"/> (<see cref="GridShowButtons"/>)</term>
    ///         <description>Specifies when to show or display the cell buttons. Possible choices are: show the button only for the current cell, always show buttons, or never show buttons. (Default: GridShowButtons.Show)</description>
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
    ///         <term><see cref="GridStyleInfo.Themed"/> (<see cref="System.Boolean"/>)</term>
    ///         <description>Specifies if cell should be drawn using Windows XP themes when <see cref="GridControlBase.ThemesEnabled"/> has been set. (Default: True)</description>
    ///     </item>
    ///     <item>
    ///         <term><see cref="GridStyleInfo.ValidateValue"/> (<see cref="GridCellValidateValueInfo"/>)</term>
    ///         <description>Holds validation rules for the cell value that are being checked before any user changes are committed to the grid cell's style object. (Default: NULL)</description>
    ///     </item>
    /// </list>
    /// <para/>   
    /// </remarks>
    public class GridRichTextBoxCellRenderer : GridCellRendererBase
    {
        GridRichTextEntryPanel panel = null;
        RichTextBox richTextBox = new RichTextBox();

        /// <summary>
        /// Initializes a new GridRichTextBoxCellRenderer object for the given GridControlBase
        /// and GridCellModelBase.
        /// </summary>
        /// <param name="grid">The <see cref="GridControlBase"/> that displays this cell renderer.</param>
        /// <param name="cellModel">The <see cref="GridCellModelBase"/> that holds data for this cell renderer that should
        /// be shared among views.</param>
        /// <remarks>References to GridControlBase 
        /// and GridCellModelBase will be saved.</remarks>
        public GridRichTextBoxCellRenderer(GridControlBase grid, GridCellModelBase cellModel)
            : base(grid, cellModel)
        {
            DropDownPart = new GridDropDownCellImp(this);
            DropDownButton = new GridCellComboBoxButton(this);
        }

        /// <override/>
        protected /*internal*/ override void InitializeDropDownContainer()
        {
            base.InitializeDropDownContainer();

            panel = new GridRichTextEntryPanel();
            panel.RightToLeft = Grid.RightToLeft;
            panel.KeyDown += new KeyEventHandler(PanelKeyDown);
            AssignRtf(panel.RichTextBox);
            this.DropDownContainer.Controls.Add(panel);
        }

        void clonedRichText_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                this.CurrentCell.EndEdit();
            }
        }

        /// <summary>
        /// Gets the container where you can insert child controls to be displayed as drop-down part for your cell.
        /// </summary>
        public new GridDropDownContainer DropDownContainer
        {
            get
            {
                return (GridDropDownContainer)base.DropDownContainer;
            }
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
        /// <override/>
        protected override void OnInitialize(int rowIndex, int colIndex)
        {
            GridStyleInfo style = Grid.Model[RowIndex, ColIndex];
            ControlValue = style.CellValue;
        }
        RichTextBox clonedRichText = new RichTextBox();
        protected override void OnClick(int rowIndex, int colIndex, MouseEventArgs e)
        {
            GridStyleInfo style = Grid.Model[RowIndex, ColIndex];
            if (Grid.Model.InRichTextEditMode)
            {
                for (int i = 0; i < 2; i++)
                {
                    string text = (string)Syncfusion.Styles.ValueConvert.ChangeType(this.ControlValue, typeof(string), StyleInfo.GetCulture(true));
                    AssignRtf(this.clonedRichText, text);
                }
                GridRichControlEditCellModel newModel = new GridRichControlEditCellModel(this.Grid.Model);
                if (!Grid.Model.CellModels.ContainsKey("RichTextExt"))
                    this.Grid.Model.CellModels.Add("RichTextExt", new GridRichControlEditCellModel(this.Grid.Model));
                
                style.CellType = "RichTextExt";
                style.Control = this.clonedRichText;
                Grid.Model.RichTextControl = this.clonedRichText;
            }
            this.Grid.CurrentCell.MoveRight();

           
            this.Grid.CurrentCell.MoveLeft();
            if (Grid.Model.InRichTextEditMode)
            {
                for (int i = 0; i < 2; i++)
                {
                    string text = (string)Syncfusion.Styles.ValueConvert.ChangeType(this.ControlValue, typeof(string), StyleInfo.GetCulture(true));
                    AssignRtf(this.clonedRichText, text);
                }                

                GridRichControlEditCellModel newModel = new GridRichControlEditCellModel(this.Grid.Model);
                if (!Grid.Model.CellModels.ContainsKey("RichTextExt"))
                    this.Grid.Model.CellModels.Add("RichTextExt", new GridRichControlEditCellModel(this.Grid.Model));

                style.CellType = "RichTextExt";
                style.Control = this.clonedRichText;
                Grid.Model.RichTextControl = this.clonedRichText;
            }
            base.OnClick(rowIndex, colIndex, e);
        }
        protected override void OnEndEdit()
        {
            base.OnEndEdit();
        }
        protected override void OnBeginEdit()
        {
            base.OnBeginEdit();
        }

        protected override void OnEditingComplete()
        {
            base.OnEditingComplete();
        }
        /// <summary>
        /// Event handler for the KeyDown event of the <see cref="GridRichTextEntryPanel"/>.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">A KeyEventArgs that contains the event data. </param>
        protected virtual void PanelKeyDown(object sender, KeyEventArgs e)
        {
            if ((e.KeyCode == Keys.Enter && Control.ModifierKeys == Keys.None)
                || (e.KeyCode == Keys.Down && Control.ModifierKeys == Keys.Alt)
                || (e.KeyCode == Keys.F2 && Control.ModifierKeys == Keys.None)
                || (e.KeyCode == Keys.F4 && Control.ModifierKeys == Keys.None))
            {
                CurrentCell.CloseDropDown(PopupCloseType.Done);
            }
        }

        /// <summary>
        /// Event handler for the <see cref="GridRichTextEntryPanel.Save"/> event of the <see cref="GridRichTextEntryPanel"/>.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">Event data. </param>
        protected virtual void PanelSave(object sender, EventArgs e)
        {
            CurrentCell.CloseDropDown(PopupCloseType.Done);
        }

        /// <summary>
        /// Event handler for the <see cref="GridRichTextEntryPanel.Cancel"/> event of the <see cref="GridRichTextEntryPanel"/>.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">Event data. </param>
        protected virtual void PanelCancel(object sender, EventArgs e)
        {
            CurrentCell.CloseDropDown(PopupCloseType.Canceled);
        }

        /// <override/>
        /// <summary>
        /// Occurs when the drop-down container is about to be shown.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">Event data.</param>
        public override void DropDownContainerShowingDropDown(object sender, CancelEventArgs e)
        {
            this.DropDownContainer.PopupHost.FormBorderStyle = FormBorderStyle.Sizable;
            this.DropDownContainer.PopupHost.Size = new Size(370, 240);
            this.DropDownContainer.Dock = DockStyle.Fill;
            GridStyleInfo style = Grid.Model[RowIndex, ColIndex];
            if (panel == null)
                panel = new GridRichTextEntryPanel();
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
            base.DropDownContainerShowingDropDown(sender, e);
        }

        /// <override/>
        /// <summary>
        /// Occurs after the popup has been dropped-down and made visible.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">Event data.</param>
        public override void DropDownContainerShowedDropDown(object sender, EventArgs e)
        {
            panel.RichTextBox.Focus();
        }

        /// <override/>
        /// <summary>
        /// Will be called to indicate that the popup child was closed.
        /// </summary>
        /// <param name="sender">The child that was closed.</param>
        /// <param name="e">The event data with a <see cref="PopupClosedEventArgs.PopupCloseType"/> value.</param>
        public override void DropDownContainerCloseDropDown(object sender, PopupClosedEventArgs e)
        {
            if (e.PopupCloseType == PopupCloseType.Done)
            {
                if (this.NotifyCurrentCellChanging())
                {
                    ControlValue = panel.RichTextBox.Rtf;
                    this.NotifyCurrentCellChanged();
                }
            }

            Grid.InvalidateRange(GridRangeInfo.Cell(RowIndex, ColIndex), GridRangeOptions.MergeCoveredCells); // Merge all cells
            base.DropDownContainerCloseDropDown(sender, e);
        }
        protected override bool OnDeactivating()
        {
            return base.OnDeactivating();
        }

        /// <summary>
        /// This method is called from GridCurrentCell.ConfirmChanges when the current cell
        /// was marked as modified. Any drop-downs have been closed at this time. It saves changes for the current cell.
        /// </summary>
        /// <returns>
        /// True if changes were saved successfully.
        /// </returns>
        /// <override/>
        protected /*internal*/ override bool OnSaveChanges()
        {
            if (HasControlValue)
            {
                Grid.Model[RowIndex, ColIndex].CellValue = ControlValue;
            }

            return true;
        }
        Rectangle celltypeBounds = new Rectangle();
        /// <override/>
        protected override void OnDraw(Graphics g, Rectangle clientRectangle, int rowIndex, int colIndex, GridStyleInfo style)
        {
            celltypeBounds = clientRectangle;
            ////TraceUtil.TraceCurrentMethodInfoIf(true, clientRectangle, rowIndex, colIndex);
            Rectangle textRectangle;

            // inactive cell
            textRectangle = RemoveMargins(clientRectangle, style);
            string rtf;

            if (CurrentCell.HasCurrentCellAt(rowIndex, colIndex) && CurrentCell.IsModified && HasControlValue)
            {
                string text = (string)Syncfusion.Styles.ValueConvert.ChangeType(this.ControlValue, typeof(string), StyleInfo.GetCulture(true));
                rtf = ControlValue.ToString();
            }
            else
            {
                rtf = style.Text;
            }

            //Remove the formfeed if any before rendering the RTF text
            rtf = rtf.Replace('\f', ' ');

            Rectangle clipBounds = GetCellBoundsCore(rowIndex, colIndex, true);
            Color backColor = Color.FromArgb(255, style.Interior.BackColor);
            if (!richTextBox.ForeColor.Equals(style.TextColor))
                richTextBox.ForeColor = style.TextColor;
            if (Grid.Model.InRichTextEditMode && Grid.Model.RichTextControl != null && Grid.Model.RichTextStyleRow == style.CellIdentity.RowIndex && Grid.Model.RichTextStyleCol == style.CellIdentity.ColIndex)
            {
                string textModified = (string)Syncfusion.Styles.ValueConvert.ChangeType(this.Grid.Model.RichTextControl, typeof(string), StyleInfo.GetCulture(true));
                string rtf2 = this.Grid.Model.RichTextControl.Rtf;
                richTextBox = this.Grid.Model.RichTextControl;
                AssignRtf(richTextBox, rtf2);
                rtf = rtf2;
                this.ControlText = rtf;
            }

            RichTextPaint.DrawRichText(g, richTextBox, rtf, Grid.PrintingMode, Grid.GridBounds, textRectangle, clipBounds, backColor, style.WrapText, 100, Grid.IsRightToLeft());
        }
    }
}
