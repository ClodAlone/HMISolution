//-------------------------------------------------------------------------------------------------
// <copyright file="GridCurrencyTextBoxCell.cs" company="syncfusion">
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

using Syncfusion.ComponentModel;
using Syncfusion.Diagnostics;
using Syncfusion.Windows.Forms;
using Syncfusion.Drawing;
using Syncfusion.Styles;
using Syncfusion.Windows.Forms.Tools;

namespace Syncfusion.Windows.Forms.Grid
{
    /// <summary>
    /// Adds grid cell-specific keyboard logic to a <see cref="CurrencyTextBox"/>.
    /// </summary>
    [ToolboxItem(false)]
    public class GridCurrencyTextBox : CurrencyTextBox
    {
        GridCurrencyTextBoxCellRenderer parent;

        /// <summary>
        /// Initializes a new <see cref="GridCurrencyTextBox"/> and attaches it to a <see cref="GridCurrencyTextBoxCellRenderer"/>.
        /// </summary>
        /// <param name="parent">Currency text box cell renderer.</param>
        public GridCurrencyTextBox(GridCurrencyTextBoxCellRenderer parent)
        {
            this.parent = parent;
            this.AutoSize = false;
            this.TabStop = false;
            this.AllowNull = false;
        }

        /// <summary>
        /// Gets the associated cell renderer for the text box.
        /// </summary>
        [Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public GridCurrencyTextBoxCellRenderer ParentCell
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

        /// <override/>
        /// <summary>
        /// Preprocesses keyboard or input messages within the message loop before they are displatched.
        /// </summary>
        /// <param name="msg">Message to be preprocessed.</param>
        /// <returns>True if the operation is successful.</returns>
        /// <remarks></remarks>
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

        /// <override/>
        protected override bool ProcessCmdKey(ref Message msg, Keys keyData)
        {
            bool cut = (keyData == (Keys.X | Keys.Control));
            bool copy = (keyData == (Keys.C | Keys.Control));

            if (msg.Msg == NativeMethods.WM_KEYDOWN && this.SelectionLength == 0 && (copy || cut))
            {
                bool HasShortcut = false;
                if (this.ContextMenu != null && this.ContextMenu.MenuItems.Count > 0)
                {
                    foreach (MenuItem item in this.ContextMenu.MenuItems)
                    {
                        if (item.Shortcut.Equals(Shortcut.CtrlC) || item.Shortcut.Equals(Shortcut.CtrlX))
                            HasShortcut = true;
                    }
                    if (HasShortcut)
                        return this.ProcessKeyMessage(ref msg);
                }
            }
            return base.ProcessCmdKey(ref msg, keyData);
        }        

        /// <summary>
        /// Invoked when the decimal key is pressed.
        /// </summary>
        /// <returns>True if the key is handled; false otherwise.</returns>
        /// <remarks>
        /// The defined behavior for this key is to jump to the position immediately
        /// after the decimal position.
        /// </remarks>
        [Syncfusion.Documentation.DocumentationExclude()]
        override protected bool HandleDecimalKey()
        {
            if (this.GetTextBoxText() == this.NullString)
            {
                this.DecimalValue = 0;
            }

            return base.HandleDecimalKey();
        }

        /// <summary>
        /// This method overrides the <see cref="M:System.Windows.Forms.Control.ProcessKeyMessage(System.Windows.Forms.Message@)"/> method
        /// and handles the key messages that are of interest to the NumberTextBox.
        /// </summary>
        /// <param name="m">The message that is to handled.</param>
        /// <returns>
        /// True if the key message is handled; false otherwise.
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
            GridCurrencyTextBoxCellRenderer tbr = ParentCell;

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
                    if (!parent.Grid.WantEscapeKey)
                    {
                        callBase = true;
                    }
                    else
                    {
                        forwardParent = true;
                        scrollInView = true;
                    }

                    break;

                case Keys.Enter:
                    if (!parent.Grid.WantEnterKey)
                    {
                        callBase = true;
                    }
                    else
                    {
                        forwardParent = true;
                        scrollInView = true;
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
                    this.AllowNull = true;
                    if (this.DecimalValue.ToString().IndexOf("0.").Equals(0))
                    {
                        this.SelectionStart += 1;
                    }
                    scrollInView = true;
                    forwardParent = bCtl || bAlt || (bShift && this.SelectionLength == 0);
                    ////callBase = false;
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

            // Gives programmers a chance to modify the default behavior of ProcessKeyMessage
            // without subclassing this control.
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
                // This will trigger Grid.RaiseKeyDown and Grid.RaiseKeyUp events. CurrentCellKeyDown and CurrentCellKeyPress events might also
                // be triggered from Grids ProcessKeyPreview method.
                this.ProcessKeyPreview(ref m);
                return true;
            }
            else
            {
                // Call to ParentCell.ProcessKeyEventArgs will trigger CurrentCellKeyDown and CurrentCellKeyPress events but no
                // Grid.RaiseKeyDown and Grid.RaiseKeyUp events.
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

         /// <returns>
        /// true if the key was processed by the control; otherwise, false.
        /// </returns>
        /// <override/>
        protected override bool ProcessDialogKey(Keys keyData)
        {
            // Work around a weird issue described in forum 
            // http://www.syncfusion.com/Support/Forums/message.aspx?MessageID=43759
            // The problem is that when the user assign a context menu to the numeric control
            // then a ProcessDialogKey message is raised and the grid KeyDown handler is called 
            // before the currency cell. Canceling out ClearingCells will tell the grid to ignore
            // the delete key.
            if (keyData == Keys.Delete && this.ParentCell != null && this.ParentCell.Grid != null)
            {
                // subscribe to event just for a short time and immediately unsubscribe from it.
                this.ParentCell.Grid.Model.ClearingCells += new GridClearingCellsEventHandler(Model_ClearingCells);
                Timer t = new Timer();
                t.Interval = 10;
                t.Tick += new EventHandler(t_Tick);
                t.Start();
            }

            return base.ProcessDialogKey(keyData);
        }

        void Model_ClearingCells(object sender, GridClearingCellsEventArgs e)
        {
            if (!e.Handled)
            {
                e.Handled = true;
                e.Result = false;
                this.ParentCell.Grid.Model.ClearingCells -= new GridClearingCellsEventHandler(Model_ClearingCells);
            }
        }

        void t_Tick(object sender, EventArgs e)
        {
            // Unsubscribe from event.
            Timer t = (Timer)sender;
            t.Tick -= new EventHandler(t_Tick);
            t.Stop();
            t.Dispose();

            this.ParentCell.Grid.Model.ClearingCells -= new GridClearingCellsEventHandler(Model_ClearingCells);
        }

        /// <override/>
        /// <summary>
        /// Pastes the data in the clipboard into the currency textbox cell.
        /// </summary>
        public override void Paste()
        {
            bool callBase = true;
            if (ParentCell != null && ParentCell.Model != null &&
                ((GridCurrencyTextBoxCellModel)ParentCell.Model).ValidateNumberDuringCellPaste)
            {
                string s = Clipboard.GetText();
                if (s != null && s.Length > 0)
                {
                    double d;
                    decimal dec;
                    callBase = double.TryParse(s, out d) || double.TryParse(s, NumberStyles.Any, null, out d) ||
                                decimal.TryParse(s, out dec) || decimal.TryParse(s, NumberStyles.Any, null, out dec);
                }
            }

            if (callBase)
            {
                base.Paste();
            }
        }
    }

    /// <summary>
    /// Implements the data / model part for a Currency cell.
    /// </summary>
    /// <remarks>
    /// You typically access cell models through the <see cref="GridModel.CellModels"/>
    /// property of the <see cref="GridModel"/> class.<para/>
    /// A <see cref="GridCurrencyTextBoxCellModel"/> can serve as model for several <see cref="GridCurrencyTextBoxCellRenderer"/>
    /// instances if there are several <see cref="GridControlBase"/> views for a <see cref="GridModel"/>.
    /// <para/>
    /// See <see cref="GridCurrencyTextBoxCellRenderer"/> for more detailed information about this cell type.
    /// </remarks>
    [Serializable]
    public class GridCurrencyTextBoxCellModel : GridStaticCellModel
    {
        private CurrencyTextBox currencyTextBox = new CurrencyTextBox();

        /// <overload>
        /// Initializes a new <see cref="GridCurrencyTextBoxCellModel"/> object.
        /// </overload>
        /// <summary>
        /// Initializes a new <see cref="GridCurrencyTextBoxCellModel"/> object
        /// and stores a reference to the <see cref="GridModel"/> this cell belongs to.
        /// </summary>
        /// <param name="grid">The <see cref="GridModel"/> for this cell model.</param>
        /// <remarks>
        /// You typically access cell models through the <see cref="GridModel.CellModels"/>
        /// property of the <see cref="GridModel"/> class.
        /// </remarks>
        public GridCurrencyTextBoxCellModel(GridModel grid)
            : base(grid)
        {
            AllowFloating = true;
            currencyTextBox.DefaultValue = DBNull.Value;
        }

        /// <summary>
        /// Initializes a new <see cref="GridCurrencyTextBoxCellModel"/> from a serialization stream.
        /// </summary>
        /// <param name="info">An object that holds all the data needed to serialize or deserialize this instance.</param>
        /// <param name="context">Describes the source and destination of the serialized stream specified by info. </param>
        protected GridCurrencyTextBoxCellModel(SerializationInfo info, StreamingContext context)
            : base(info, context)
        {
        }

        /// <override/>
        /// <summary>Creates cell renderer.</summary>
        /// <returns>Currency TextBox cell renderer.</returns>
        public override GridCellRendererBase CreateRenderer(GridControlBase control)
        {
            return new GridCurrencyTextBoxCellRenderer(control, this);
        }
        
        /// <override/>
        /// <summary>
        /// This is called from GridStyleInfo.GetFormattedText.
        /// </summary>
        /// <param name="style">The <see cref="GridStyleInfo"/> object that holds cell information.</param>
        /// <param name="value">The value to format.</param>
        /// <param name="textInfo">TextInfo is a hint of who is calling, default is GridCellBaseTextInfo.DisplayText.</param>
        /// <returns>The formatted text for the given value.</returns>
        public override string GetFormattedText(GridStyleInfo style, object value, int textInfo)
        {
            Color textColor;
            string text;
            ////GridCellBaseTextInfo.PasteText/GridCellBaseTextInfo.CopyText
            if (textInfo == 1 && style.CurrencyEdit.ClipMode == CurrencyClipModes.ExcludeFormatting)
            {
                text = GetText(style, value);
            }
            else
            {
                text = GridCurrencyTextBoxStaticCellAdapter.GetFormattedText(style, out textColor);
            }

            return text;
        }
         
        ///<override/>
        protected override void Dispose(bool disposing)
        {
            base.Dispose(disposing);
            if (disposing && this.currencyTextBox != null)
            {
                this.currencyTextBox.Dispose();
                this.currencyTextBox = null;
            }
        }
        /// <override/>
        /// <summary>
        /// Parses the text and converts it into a cell value to be stored in the style object (ignoring any <see cref="GridStyleInfo.Format"/> settings).
        /// CultureInfo.CurrentText is used for parsing the string.
        /// </summary>
        /// <param name="style">The <see cref="GridStyleInfo"/> object that holds cell information.</param>
        /// <param name="text">The input text to be parsed.</param>
        /// <returns>True if value was parsed correctly and saved in style object as <see cref="GridStyleInfo.CellValue"/>; False otherwise.</returns>
        public override bool ApplyText(GridStyleInfo style, string text)
        {
            GridCellTextEventArgs ea = new GridCellTextEventArgs(text, style, null, -1);
            Grid.RaiseSaveCellText(ea);
            if (!ea.Handled)
            {
                try
                {
                    style.BeginUpdate();
                    if (text.Equals(string.Empty))
                    {
                        style.CellValue = GridCurrencyTextBoxStaticCellAdapter.GetValueFromStyle(style);
                    }
                    else
                    {
                        style.CellValue = text;
                    }

                    style.ResetError();
                }
                catch (Exception ex)
                {
                    style.Error = ex.Message;
                    if (style.StrictValueType)
                    {
                        throw;
                    }
                    else if (ex is FormatException || ex.InnerException is FormatException)
                    {
                        style.CellValue = text;
                        // Possibly could also change CellValueType here based on input string.
                        // e.Style.CellValueType = typeof(string);
                    }
                    else
                    {
                        throw;
                    }
                }
                finally
                {
                    style.EndUpdate();
                }
            }

            return true;
//            return base.ApplyText(style, text);
        }

        /// <override/>
        /// <summary>
        /// Parses the display text and converts it into a cell value to be stored in the style object.
        /// GridStyleInfo.CultureInfo is used for parsing the string.
        /// </summary>
        /// <param name="style">Style information for the cell.</param>
        /// <param name="text">The input text to be parsed.</param>
        /// <param name="textInfo">TextInfo is a hint of who is calling, default is GridCellBaseTextInfo.DisplayText</param>
        /// <returns>True if value was parsed correctly and saved in style object as <see cref="GridStyleInfo.CellValue"/>; False otherwise.</returns>
        public override bool ApplyFormattedText(GridStyleInfo style, string text, int textInfo)
        {           
            GridCellTextEventArgs ea = new GridCellTextEventArgs(text, style, null, textInfo);

            // Give users a chance to do more extensive parsing, e.g. set CurrencySymbol based on input text.
            Grid.RaiseSaveCellFormattedText(ea);

            if (!ea.Handled)
            {
                NumberFormatInfo nfi = style.CurrencyEdit.NumberFormatInfoObject;
                try
                {
                    if (ea.Text.Length > 0 && !ea.Text.Equals(style.CurrencyEdit.NullString))
                    {
                        decimal dec = Decimal.Parse(ea.Text, NumberStyles.Currency, nfi);
                        if (style.CellValueType != null)
                        {
                            style.CellValue = NullableHelper.ChangeType(dec, style.CellValueType);
                        }
                        else
                        {
                            if (style.CurrencyEdit.ClipMode == CurrencyClipModes.IncludeFormatting)
                            {
                                Color textColor;
                                style.CellValue = GridCurrencyTextBoxStaticCellAdapter.GetFormattedText(style, out textColor);
                            }
                            else
                            {
                                style.CellValue = dec;
                            }
                        }
                    }
                    else
                    {
                        style.CellValue = style.CurrencyEdit.NullValue;
                    }
                }
                catch (Exception ex)
                {
                    if (ex is FormatException || ex.InnerException is FormatException)
                    {
                        style.CellValue = style.CurrencyEdit.NullValue;
                    }
                    else
                    {
                        throw;
                    }
                }
            }

            return true;
        }

        /// <summary>
        /// Returns the text as it is displayed in the cell (without '-' sign) and also the text color.
        /// </summary>
        /// <param name="style">The style information.</param>
        /// <param name="value">The value.</param>
        /// <param name="textColor">Returns the text color for the cell (depending on whether value is negative or positive).</param>
        /// <returns>The text as it is displayed in the cell.</returns>
        public string GetDisplayTextAndColor(GridStyleInfo style, object value, out Color textColor)
        {
            string text = GridCurrencyTextBoxStaticCellAdapter.GetFormattedText(style, out textColor);
            return text;
        }
        
        /// <summary>
        /// Initializes a <see cref="CurrencyTextBox"/> with information supplied by a <see cref="GridStyleInfo"/>.
        /// </summary>
        /// <param name="mb">The control to be initialized.</param>
        /// <param name="style">The style with settings to be applied.</param>
        public static void InitCurrencyEditProperties(CurrencyTextBox mb, GridStyleInfo style)
        {
            GridCurrencyEditInfo mi = style.CurrencyEdit;

            mb.BeginInit();
            mb.ReadOnly = false;
            mb.Culture = style.GetCulture(true);
            mb.CurrencyNumberDigits = mi.CurrencyNumberDigits;

            // TODO: Davis - What should I do here if user has specufied UseCultureInfo
            // mi.NumberFormatInfoObject handles UseCultureInfo case. I guess that is all I need, or?
            mb.NumberFormatInfoObject = mi.NumberFormatInfoObject;

            // setting NumberFormatInfoObject will force setting these properties:
            // -->
            //            mb.CurrencyDecimalDigits = mi.CurrencyDecimalDigits;
            //            mb.NegativeSign = mi.NegativeSign;
            //            mb.CurrencyDecimalSeparator = mi.CurrencyDecimalSeparator;
            //            mb.CurrencyGroupSeparator = mi.CurrencyGroupSeparator;
            //            mb.CurrencyGroupSizes = mi.CurrencyGroupSizes;
            //            mb.CurrencyNegativePattern = mi.CurrencyNegativePattern;
            //            mb.CurrencyPositivePattern = mi.CurrencyPositivePattern;
            //            mb.CurrencySymbol = mi.CurrencySymbol;
            // <--
            mb.PositiveColor = mi.PositiveColor;
            mb.NegativeColor = mi.NegativeColor;
            mb.ClipMode = CurrencyClipModes.IncludeFormatting;  // .Text should always return formatted text, .ClipText without literals
            mb.NullString = mi.NullString;
            mb.MaxLength = style.MaxLength;
            // Not used:
            // style.HorizontalAlignment
            // style.VerticalAlignment
            // style.TextMargins
            // style.ReadOnly
            mb.EndInit();

            // Assign text end backcolor outside BeginInit / EndInit block.
            mb.Text = style.Text;

            mb.SetControlColor();
            mb.ReadOnly = style.ReadOnly;
        }

        /// <override/>
        /// <summary>
        /// This is called from GridStyleInfo.GetText (ignoring any <see cref="GridStyleInfo.Format"/> settings).
        /// </summary>
        /// <param name="style">The <see cref="GridStyleInfo"/> object that holds cell information.</param>
        /// <param name="value">The value to convert to a string.</param>
        /// <returns>The string that represents the given value.</returns>
        public override string GetText(GridStyleInfo style, object value)
        {
            GridCellTextEventArgs ea = new GridCellTextEventArgs(string.Empty, style, value, -1);
            Grid.RaiseQueryCellText(ea);
            if (ea.Handled)
            {
                return ea.Text;
            }

            CultureInfo ci = (style != null) ? style.GetCulture(true) : CultureInfo.CurrentCulture;
            
            return value != null ? Convert.ToString(value, ci.NumberFormat) : string.Empty;
        }

        private bool validateNumberDuringCellPaste = true;

        /// <summary>
        /// Gets or sets a value indicating whether Clipboard text is validated before a paste.
        /// </summary>
        /// <remarks>
        /// If ValidateNumberDuringCellPaste is true, then when a paste operation is preformed on an active 
        /// cell, the paste will only be attempted if the text on the Clipboard is valid.
        /// </remarks>
        public bool ValidateNumberDuringCellPaste
        {
            get { return validateNumberDuringCellPaste; }
            set { validateNumberDuringCellPaste = value; }
        }
    }

    /// <summary>
    /// Implements the renderer part of a currency cell that handles currency input
    /// and validation.
    /// </summary>
    /// <remarks>
    /// The CurrencyTextBox is derived from the text box and provides all the functionality
    /// of a text box and adds additional functionality of its own.
    /// <para>
    /// Collecting currency input in a consistent format requires a alot of validation code
    /// that needs to be built into the application when using the Windows Forms text box control.
    /// The CurrencyTextBox includes all this logic into its methods and properties
    /// and makes it easy for the developer and the end user to collect and enter currency data.
    /// </para>
    /// <para>
    /// The CurrencyTextBox is also closely tied to the globalization settings of the
    /// operating system for Currency related properties. Please refer to the <see cref="System.Globalization.NumberFormatInfo"/>
    /// class for a detailed explnation of globalization and Currency related attributes.
    /// </para>
    /// Use the <see cref="GridStyleInfo.CurrencyEdit"/> (<see cref="GridCurrencyEditInfo"/>) property
    /// of a <see cref="GridStyleInfo"/> to change currency edit properties for a cell.
    /// <para/>
    /// <para/>
    /// The following table lists some characteristics about the Currency cell type:
    /// <para/>
    /// <list type="table">
    ///     <listheader>
    ///         <term>Item</term>
    ///         <description>Description</description>
    ///     </listheader>
    ///     <item>
    ///         <term>CellType</term>
    ///         <description>Currency</description>
    ///     </item>
    ///     <item>
    ///         <term>Renderer</term>
    ///         <description><see cref="GridCurrencyTextBoxCellRenderer"/></description>
    ///     </item>
    ///     <item>
    ///         <term>Model</term>
    ///         <description><see cref="GridCurrencyTextBoxCellModel"/></description>
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
    ///         <description><see cref="GridCurrencyTextBox"/></description>
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
    ///         <description>Gets / sets if the cell height should automatically increase when the edited text does not fit into the cell and <see cref="GridStyleInfo.WrapText"/> is True. If <see cref="GridStyleInfo.WrapText"/> is False, <see cref="GridStyleInfo.AutoSize"/> will affect the column width. (Default: false)</description>
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
    ///         <description>Currency (Default: Text Box)</description>
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
    ///         <term><see cref="GridStyleInfo.CurrencyEdit"/> (<see cref="GridCurrencyEditInfo"/>)</term>
    ///         <description>A nested object with currency text box properties for a cell.  (Default: GridCurrencyEditInfo.Default)</description>
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
    /// Date format strings or enumeration format strings as discussed in the section "Format Specifiers and Format Providers" of the .NET Framework Developers Guide (see ms-help://MS.VSCC/MS.MSDNVS/cpguide/html/cpconformatspecifiersformatproviders.htm). (Default: String.Empty)</description>
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
    ///         <description>Specifies merge behavior for a individual cell when merging cell's features have been enabled in a <see cref="GridModel"/> with  <see cref="GridModelOptions.MergeCellsMode"/>. (Default: GridMergeCellDirection.None)</description>
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
    public class GridCurrencyTextBoxCellRenderer : GridStaticCellRenderer
    {
        private GridCurrencyTextBox focusControl;
        GridCurrencyTextBoxCellModel currencyModel;

        /// <summary>
        /// Initializes a new GridCurrencyTextBoxCellRenderer object for the given GridControlBase
        /// and GridCellModelBase.
        /// </summary>
        /// <param name="grid">The <see cref="GridControlBase"/> that display this cell renderer.</param>
        /// <param name="cellModel">The <see cref="GridCellModelBase"/> that holds data for this cell renderer that should
        /// be shared among views.</param>
        /// <remarks>References to GridControlBase,
        /// and GridCellModelBase will be saved.</remarks>
        public GridCurrencyTextBoxCellRenderer(GridControlBase grid, GridCellModelBase cellModel)
            : base(grid, cellModel)
        {
            this.SupportsFocusControl = true;
            focusControl = new GridCurrencyTextBox(this);
            focusControl.DefaultValue = DBNull.Value;
            currencyModel = (GridCurrencyTextBoxCellModel)cellModel;
            FixControlParent(focusControl);
            ////focusControl.ValidationError += new ValidationErrorEventHandler(FocusControlValidationError);
            focusControl.TextChanged += new EventHandler(FocusControlTextChanged);
            focusControl.GotFocus += new EventHandler(focusControl_GotFocus);            
        }
        /// <override/>
        protected override void Dispose(bool disposing)
        {
            base.Dispose(disposing);
            if (disposing && this.focusControl != null)
            {
                focusControl.TextChanged -= new EventHandler(FocusControlTextChanged);
                focusControl.GotFocus -= new EventHandler(focusControl_GotFocus);

                this.focusControl.Dispose();
                this.focusControl = null;
            }
        }

        /// <override/>
        /// <summary>
        /// Determines whether current cell can be copied to clipboard.
        /// </summary>
        /// <returns>True if copy is supported; otherwise False.</returns>
        public override bool CanCopy()
        {
            return Grid.Model.SelectedRanges.Count == 0;
        }

        ////Fix for defect #12240

        /// <summary>
        /// Determines whether current cell can be pasted from clipboard.
        /// </summary>
        /// <returns>True if paste is supported; otherwise False.</returns>
        public override bool CanPaste()
        {
            if (focusControl.ReadOnly)
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

                if (buffer != null)
                {
                    return buffer.IndexOf("\t") == -1 && buffer.IndexOf("\r") == -1;
                }
            }

            return false;
        }

        ////Fix for defect #12240

        /// <summary>
        /// Called when user initiates a clipboard paste and the grid has a current cell but no range is selected.
        /// </summary>
        /// <returns>True if successful; False if failed.</returns>
        public override bool Paste()
        {
            if (focusControl.ReadOnly)
            {
                return false;
            }

                if (!CurrentCell.IsEditing)
                {
                    return CurrentCell.BeginEdit();
                }

            return base.Paste();
        }

        void FocusControlTextChanged(object sender, EventArgs e)
        {
            if (this.InInitialize)
            {
                return;
            }
            
            try
            {
                if (!this.NotifyCurrentCellChanging())
                {
                    CurrentCell.SuspendEvents();
                    focusControl.BeginInit();
                    focusControl.TextChanged -= new EventHandler(FocusControlTextChanged);
                    InitializeControlText(StyleInfo.CellValue);
                    focusControl.TextChanged += new EventHandler(FocusControlTextChanged);
                    focusControl.EndInit();
                    CurrentCell.IsModified = false;
                    CurrentCell.ResumeEvents();
                    return;
                }

                this.ResetControlText();
                SetControlValue(focusControl.DecimalValue/*GetClipText(false)*/, false);
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
                ////                inTextBoxChanged = false;
                floated = false;
            }
        }

        ////bool inTextBoxChanged = false;
        bool floated = false;

        private void Model_FloatingCellsChanged(object sender, GridFloatingCellsChangedEventArgs e)
        {
            floated = true;
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
            bounds.Inflate(-1, -1);
            return bounds;
        }

        /// <override/>
        /// <summary>Allows custom formatting of a cell by changing its style object.</summary>
        /// <param name="e">Event data.</param>
        public override void OnPrepareViewStyleInfo(GridPrepareViewStyleInfoEventArgs e)
        {
            base.OnPrepareViewStyleInfo(e);
        }

        /// <override/>
        protected override void OnDraw(System.Drawing.Graphics g, System.Drawing.Rectangle clientRectangle, int rowIndex, int colIndex, Syncfusion.Windows.Forms.Grid.GridStyleInfo style)
        {
            if (this.ShouldDrawFocused(rowIndex, colIndex))
            {
                if (this.focusControl != null)
                {
                    ////if (false && inTextBoxChanged && focusControl.Visible)
                    ////{
                    ////    Rectangle textRectangle = RemoveMargins(clientRectangle, style);
                    ////    focusControl.Bounds = textRectangle;
                    ////}
                    ////else
                    {
                        //// Set size.
                        focusControl.Size = clientRectangle.Size;
                        focusControl.Location = clientRectangle.Location;
                    }
                    if (this.ShouldDrawEditing(rowIndex, colIndex))
                    {
                        if (this.ControlValue != null && focusControl.Text != this.ControlValue.ToString())
                        {
                            if ((this.Grid.Model.Options.ActivateCurrentCellBehavior & GridCellActivateAction.SelectAll) == 0)
                            {
                                focusControl.DeselectAll();
                                focusControl.SelectionStart = ((GridCurrencyTextBox)this.focusControl).SelectionStart;                             
                            }
                        }
                    }

                    if (!focusControl.ContainsFocus)
                    {
                        focusControl.Focus();
                    }
                }
            }
            else
            {
                //// Will call GridCurrencyTextBoxCellModel.GetFormattedText.
                Color textColor = style.TextColor;
                ////Temporarily change CellValueType to avoid call to parse that fails.

                string text;
                if (this.ShouldDrawEditing(rowIndex, colIndex))
                {
                    text = this.ControlText;
                }
                else
                {
                    text = currencyModel.GetActiveText(rowIndex, colIndex);
                }

                if (text == null)
                {
                    text = currencyModel.GetDisplayTextAndColor(style, style.CellValue, out textColor);
                }

                if (Grid.PrintingMode && Grid.Model.Properties.BlackWhite)
                {
                    textColor = Color.Black;
                }
                else if (style.Interior.BackColor == SystemColors.Highlight)
                {
                    textColor = style.TextColor;
                }

                DrawCurrencyText(g, clientRectangle, rowIndex, colIndex, style, text, textColor);
            }
        }

        void DrawCurrencyText(Graphics g, Rectangle clientRectangle, int rowIndex, int colIndex, GridStyleInfo style, string displayText, Color textColor)
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
                    DrawText(g, displayText, font, textRectangle, style, textColor, drawDisabled, isTextRightToLeft);
                }
            }
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
        /// <override/>
        protected override void OnBeginEdit()
        {
            if (this.ControlValue != null && this.ControlText != this.focusControl.NullString && this.focusControl.DecimalValue.Equals(0))
            {
                this.focusControl.AllowNull = false;
            }
            else
            {
                this.focusControl.AllowNull = true;
                if (this.ControlText.Equals(this.focusControl.NullString) || this.ControlValue == null)
                {
                    this.focusControl.Delete();
                }
            }
            base.OnBeginEdit();
        }
        /// <summary>
        /// This method is called from GridCurrentCell.ConfirmChanges when the current cell
        /// was marked as modified. Any drop-downs have been closed at this time. It saves changes for the current cell.
        /// </summary>
        /// <returns>
        /// True if changes were saved successfully; False if no changes were saved.
        /// </returns>
        /// <override/>
        protected /*internal*/ override bool OnSaveChanges()
        {
#if DEBUG
            if (Switches.CellRenderer.TraceVerbose)
            {
                TraceUtil.TraceCurrentMethodInfo(this.focusControl.Text);
            }
#else
            ;
#endif
            //// Setting text will trigger grid.OnSaveCellText event. By default, text will then
            //// be convereted into whatever style.CellValueType the user has specified.

            GridStyleInfo style = this.Grid.Model[RowIndex, ColIndex];

            ////Copy/Paste with ClipModes is handled in GetFormattedText method
            ////if (style.CurrencyEdit.ClipMode == CurrencyClipModes.ExcludeFormatting)
            //  style.Text = this.focusControl.GetClipText(false);
            ////else
            ////    style.Text = this.focusControl.Text;

            if (style.CellValueType != null)
            {
                if (this.focusControl.Text.Equals(style.CurrencyEdit.NullString))
                {
                    object obj = null;
                    try
                    {
                        obj = NullableHelper.ChangeType(this.focusControl.Text, style.CellValueType,style.CultureInfo);
                    }
                    catch (FormatException)
                    {
                        obj = this.focusControl.DefaultValue;
                    }
                    finally
                    {
                        style.CellValue = obj;
                    }
                }
                else
                {
                    style.CellValue = GridCellValueConvert.ChangeType(this.focusControl.DecimalValue, style.CellValueType, null);
                }
            }
            else
            {
                decimal currencyValue;
                string currencyText;
                if (!string.IsNullOrEmpty(this.focusControl.CurrencySymbol) && this.focusControl.Text.Contains(this.focusControl.CurrencySymbol.ToString()))
                {
                    currencyText = this.focusControl.Text.Replace(this.focusControl.CurrencySymbol.ToString(), string.Empty);
                    if (decimal.TryParse(currencyText, out currencyValue))
                    {
                        style.CellValue = currencyValue;
                    }
                    else
                        style.CellValue = this.focusControl.Text;
                }
                else
                    style.CellValue = this.focusControl.Text;
            }

            return true;
        }

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
                // Immeditaly switch into editing mode when cell is initialized.
                GridStyleInfo style = Grid.GetViewStyleInfo(rowIndex, colIndex, false);

                // Get the client bounds taking floated cells into consideration.
                Rectangle bounds = GetCellClientRectangle(rowIndex, colIndex, style, true);

                // Assign size before setting text to ensure that it does not scroll.
                focusControl.Size = bounds.Size;

                GridCurrencyTextBoxCellModel.InitCurrencyEditProperties(this.focusControl, style);
                focusControl.BeginInit();
                focusControl.BorderStyle = BorderStyle.None;
                focusControl.BackColor = Color.FromArgb(255, Grid.GetBackColor(style.Interior.BackColor));
                focusControl.ForeColor = style.TextColor;
                focusControl.Font = style.GdipFont;
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

                focusControl.EndInit();
                base.OnInitialize(rowIndex, colIndex);
        }

        /// <override/>
        protected override void InitializeControlText(object controlValue)
        {
            NumberFormatInfo nfi = StyleInfo != null ? StyleInfo.CurrencyEdit.NumberFormatInfoObject : CultureInfo.CurrentCulture.NumberFormat;
            string text = string.Empty;
            if (controlValue != null)
            {
                double d;
                if (Double.TryParse(controlValue.ToString(), out d) && StyleInfo.CurrencyEdit.CurrencyNumberDigits < controlValue.ToString().IndexOf(StyleInfo.CurrencyEdit.CurrencyDecimalSeparator))
                {
                    text = d.ToString("c", nfi);
                }
                else
                {
                    text = Convert.ToString(controlValue, nfi);
                }
            }

            this.focusControl.Text = text;

            text = this.focusControl.FormattedText;
            GridCurrentCellInitializeControlTextEventArgs e = new GridCurrentCellInitializeControlTextEventArgs(this.RowIndex, this.ColIndex, this.StyleInfo, controlValue, text);
            if (Grid.RaiseCurrentCellInitializeControlText(e))
            {
                if (e.ControlText != text)
                {
                    this.ControlText = e.ControlText;
                }
            }
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
        public override bool CanCut()
        {
            if (this.focusControl != null && this.focusControl.ReadOnly)
            {
                return false;
            }

            return true;
        }

        /// <override/>
        protected override void OnHasFocusControlChanged()
        {
            if (this.HasFocusControl)
            {
                if ((Grid.Model.Options.ActivateCurrentCellBehavior & GridCellActivateAction.SelectAll) != 0)
                {
                    this.focusControl.SelectAll();
                }
            }

            base.OnHasFocusControlChanged();
        }

        int cursorPos1;
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

            if (this.focusControl != null && this.focusControl is GridCurrencyTextBox)
            {
                cursorPos1 = ((GridCurrencyTextBox)this.focusControl).SelectionStart;
            }

            if (!e.Handled && !controlKeyDown && !menuKeyDown)
            {
                if (!Char.IsControl(e.KeyChar)
                    && !IsReadOnly()
                    && SupportsFocusControl)
                {
                    if (e.KeyChar.Equals('0') && this.CurrentCell.HasControlFocus)
                    {
                        if (this.Control.Text.Equals(this.focusControl.NullString))
                        {
                            this.focusControl.AllowNull = false;
                        }
                    }
                    else
                    {
                        if (!CurrentCell.HasControlFocus)
                        {
                            char result;
                            if (this.ValidateString(e.KeyChar.ToString())
                                && CurrentCell.BeginEdit())
                            {
                                CurrentCell.HasControlFocus = true;
                                if ((char.IsDigit(e.KeyChar) || e.KeyChar == '.')
                                    && this.NotifyCurrentCellChanging())
                                {
                                    this.focusControl.AllowNull = false;
                                    CurrentCell.IsModified = true;
                                    ControlValue = e.KeyChar == '.' ? "0" : e.KeyChar.ToString();
                                    this.focusControl.SelectAllOnFocus = false;
                                    Grid.Update();

                                    int cursorPos = ((GridCurrencyTextBox)this.Control).FormattedText.IndexOf(e.KeyChar);  ////added
                                    cursorPos = (cursorPos > -1) ? cursorPos + 1 : 0;  ////added
                                    ((GridCurrencyTextBox)this.Control).SelectionStart = cursorPos; ////added
                                    ((GridCurrencyTextBox)this.Control).SelectionLength = 0; ////added
                                }
                                else if (Char.TryParse(focusControl.NegativeSign, out result) && Grid.Model.Options.ActivateSendKey)
                                {
                                    SendKeys.Send(new string(e.KeyChar, 1));
                                }
                                else
                                {
                                    CurrentCell.CancelEdit();
                                }
                            }

                            e.Handled = true;
                        }
                    }
                }
                else if (e.KeyChar == '\b')
                {
                    if (this.focusControl.DecimalValue.Equals(0))
                    {
                        this.focusControl.AllowNull = true;
                        this.focusControl.Delete();
                    }
                }
            }
            
            base.OnKeyPress(e);
        }     
        /// <override/>
        protected override void OnSetControlText(string text)
        {
            this.focusControl.Text = text;
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
                                    ////IsReadOnly() - cell is not readonly; Grid.RaiseCurrentCellDeleting() - OnDeleteCell notification returns true
                                    if (!IsReadOnly()       
                                        && OnDeleting()
                                        && Grid.RaiseCurrentCellDeleting())
                                    {
                                        // position caret
                                        focusControl.SelectAll();
                                        if (CurrentCell.BeginEdit())
                                        {
                                            CurrentCell.HasControlFocus = true;
                                            this.focusControl.AllowNull = true;
                                            focusControl.Delete();
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
                                focusControl.SelectionStart = focusControl.Text.Length;
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
                                focusControl.Update();
                                focusControl.SelectionStart = 0;
                                focusControl.SelectionLength = 0;
                                CurrentCell.ScrollInView(GridScrollCurrentCellReason.KeyPress);
                                e.Handled = true;
                            }
                        }

                        break;

                    case Keys.Enter:
                        break;
                    case Keys.V:
                        if (e.Control)
                        {
                            if (!string.IsNullOrEmpty(Clipboard.GetText()))
                                this.CanPaste();
                        }
                        break;
                }
            }

            base.OnKeyDown(e);
        }

        /// <override/>
        protected override void OnDoubleClick(int rowIndex, int colIndex, MouseEventArgs e)
        {
            if (this.Grid.CurrentCell.HasCurrentCellAt(rowIndex, colIndex)
                && (this.Grid.Model.Options.ActivateCurrentCellBehavior & GridCellActivateAction.DblClickOnCell) != 0)
            {
                // Activate textbox and show caret
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

        private void focusControl_GotFocus(object sender, EventArgs e)
        {
            if ((Grid.Model.Options.ActivateCurrentCellBehavior & GridCellActivateAction.SelectAll) != 0)
            {
                this.focusControl.SelectAll();
            }

            this.NotifyCurrentCellControlGotFocus(this.Control);
        }
    }

    /// <summary>
    /// Adapter class to get CurrencyTextBox specific information for static drawing of
    /// a GridCurrencyTextBoxCell.
    /// </summary>
    internal class GridCurrencyTextBoxStaticCellAdapter : object
    {
        /// <summary>
        /// Gets the currency formatted text for a given GridStyleInfo.
        /// </summary>
        /// <param name="info">The GridStyleInfo object that has the CurrencyEditInfo.</param>
        /// <param name="textColor">Display color for the cell.</param>
        /// <returns>The formatted currency text.</returns>
        internal static string GetFormattedText(GridStyleInfo info, out Color textColor)
        {
            decimal dValue = 0m;

            object dObject = GetValueFromStyle(info);

            string text = string.Empty;
            double d; decimal d1;
            if (dObject != null)
            {
                if (Double.TryParse(dObject.ToString(), out d) && info.CurrencyEdit.CurrencyNumberDigits < dObject.ToString().IndexOf(info.CurrencyEdit.CurrencyDecimalSeparator))
                {
                    text = d.ToString("c", info.CurrencyEdit.NumberFormatInfoObject);
                }
                else if (Decimal.TryParse(dObject.ToString(), out d1))
                {
                    text = d1.ToString("c", info.CurrencyEdit.NumberFormatInfoObject);
                }
                else
                {
                    text = Convert.ToString(dObject, info.CurrencyEdit.NumberFormatInfoObject);
                }
            }
            string styleText=string.Empty;
            if (Decimal.TryParse(text, NumberStyles.Currency, info.CurrencyEdit.NumberFormatInfoObject, out dValue))
            {
                styleText = CurrencyTextBox.CurrencyFormattedText(info.CurrencyEdit.NumberFormatInfoObject, text, info.CurrencyEdit.NullString, out dValue);
            }
            else
            {
                styleText = info.CurrencyEdit.NullString;
            }

            if (dValue < 0)
            {
                textColor = info.CurrencyEdit.NegativeColor;
            }
            else
            {
                textColor = info.CurrencyEdit.PositiveColor;
            }

            return styleText;
        }

        internal static object GetValueFromStyle(GridStyleInfo info)
        {
            double dValue = 0;

            object dObject = null;
            if (info.CellValue is string && info.CellValue.ToString() != string.Empty)
            {
                string s = info.CellValue.ToString();
                CultureInfo ci = info.GetCulture(true);
                NumberFormatInfo nfi = ci != null ? ci.NumberFormat : null;
                if (double.TryParse(s, NumberStyles.Currency, nfi, out dValue) && info.CurrencyEdit.CurrencyNumberDigits < s.IndexOf(info.CurrencyEdit.CurrencyDecimalSeparator))
                {
                    dObject = Convert.ToDecimal(dValue);
                }
                else
                {
                    dObject = info.CellValue;
                }
            }
            else
            {
                dObject = info.CellValue;
            }

            return dObject;
        }
    }
}
