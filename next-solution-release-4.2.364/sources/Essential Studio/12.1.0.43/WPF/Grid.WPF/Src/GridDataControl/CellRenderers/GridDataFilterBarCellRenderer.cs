#region Copyright Syncfusion Inc. 2001 - 2014
// Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Any infringement will be prosecuted under
// applicable laws. 
#endregion
namespace Syncfusion.Windows.Controls.Grid
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Windows.Controls;
    using System.Windows;
    using Syncfusion.Windows.Controls.Cells;
    using System.Windows.Media;
    using System.Windows.Input;
    using Syncfusion.Windows.Controls.Scroll;
    using Syncfusion.Linq;
    using System.Windows.Threading;
    using Syncfusion.Windows.Data;
    using System.Text.RegularExpressions;
    using System.Globalization;
    using System.Windows.Data;
    using System.Collections.ObjectModel;
    using System.Xml.Serialization;
    using System.Collections;

    public class GridDataFilterBarCellModel : GridCellModel<GridDataFilterBarCellRenderer>
    {
    }

    public class GridDataFilterBarCellRenderer : GridVirtualizingCellRenderer<TextBox>
    {

        #region Private Properties
        
        int textBoxSelectionStart = -1;
        int textBoxSelectionLength = 0;
        private int ccSelectionStart = 0, ccSelectionLength = 0;
        string FilterValue = string.Empty;
        // string NumericRegexString = @"[\d.<>=!]+$";Variable is assigned but it is never used

        private const string AndStringToken = " and ";
        private const string AndSymbolToken = " && ";
        private const string OrStringToken = " or ";
        private const string OrSymbolToken = " || ";
        private const string EqualToken = "=";
        private const string StartsWithToken = "%";
        private const string ContainsToken = "#";
        private const string QuoteString = "'";
        private const string LeftBraceString = "[ ";
        private const string RightBraceString = " ]";
        private const string EmptyString = "";

        string errorToolTip = "";
        int currentRowCount = 0;
#if Silverlight 
        string NumericWithANDORString = @"[\d.<>=!-spaceSPACEandANDorOR|\s]+$";
#else
        string NumericWithANDORString = @"[\d.<>=!-andANDorOR\s]+$";
#endif
        #endregion

        #region GridDataFilterToken Properties

        private bool isTokensSetOutside = false;
        private string containsString = "#";

        public string ContainsString
        {
            get { return containsString; }
            set
            {
                containsString = value;
                isTokensSetOutside = true;
            }
        }

        private string startWithEndWithString = "%";

        public string StartWithEndWithString
        {
            get { return startWithEndWithString; }
            set
            {
                startWithEndWithString = value;
                isTokensSetOutside = true;
            }
        }

        private string lessThanString = "<";

        public string LessThanString
        {
            get { return lessThanString; }
            set
            {
                lessThanString = value;
                isTokensSetOutside = true;
            }
        }

        private string lessThanOrEqualString = "<=";

        public string LessThanOrEqualString
        {
            get { return lessThanOrEqualString; }
            set
            {
                lessThanOrEqualString = value;
                isTokensSetOutside = true;
            }
        }

        private string greaterThanString = ">";

        public string GreaterThanString
        {
            get { return greaterThanString; }
            set
            {
                greaterThanString = value;
                isTokensSetOutside = true;
            }
        }

        private string greaterThanOrEqualString = ">=";

        public string GreaterThanOrEqualString
        {
            get { return greaterThanOrEqualString; }
            set
            {
                greaterThanOrEqualString = value;
                isTokensSetOutside = true;
            }
        }

        private string equalsString = "=";

        public string EqualsString
        {
            get { return equalsString; }
            set
            {
                equalsString = value;
                isTokensSetOutside = true;
            }
        }

        private string notString = "!";

        public string NotString
        {
            get { return notString; }
            set
            {
                notString = value;
                isTokensSetOutside = true;
            }
        }

        #endregion

        #region Public Poperties

        public bool IsFilteringSuspend { get; private set; }

        string numericFollow = @"(?<Value1>([\w<>=!-\\\/\.]+))(?<path>[^\r\n]*)";
        /// <summary>
        /// Gets or Sets the Regex String for Numeric FilterBarType
        /// </summary>
        public string NumericFollow
        {
            get { return numericFollow; }
            set { numericFollow = value; }
        }
        string numericWithSeparatorRegexString = @"(?<Value1>([\w<>=-\\\/\.]+))\s(?<Key>\w{2,3})\s(?<Value2>([\w<>=-\\\/\.]+))(?<path>[^\r\n]*)";

        /// <summary>
        /// Gets or Sets the Regex String with Separator for Numeric FilterBarType
        /// </summary>
        public string NumericWithSeparatorRegexString
        {
            get { return numericWithSeparatorRegexString; }
            set { numericWithSeparatorRegexString = value; }
        }

        //  string AlphaNumericWithSeparatorRegexString = @"(?<Value1>([\w<>#%=.]+))\s(?<Key>\w{2,3})\s(?<Value2>([\w<>#%=.]+[\w<>#%=.]+))";  This is previous patten. The patten is modified to below since we can't use multiple filters in Alphanumeric Filter Type.
        string alphaNumericWithSeparatorRegexString = @"(?<Value1>([\w<>#%=.]+))\s(?<Key>\w{2,3})\s(?<Value2>([\w<>#%=.]+[\w<>#%=.]+))(?<path>[^\r\n]*)";


        /// <summary>
        /// Gets or Sets the Regex String with Separator for AlphaNumeric FilterBarType
        /// </summary>
        public string AlphaNumericWithSeparatorRegexString
        {
            get { return alphaNumericWithSeparatorRegexString; }
            set { alphaNumericWithSeparatorRegexString = value; }
        }


        string alphaNumericRegexString = @"^[a-zA-Z0-9#%.\s]+$";
        /// <summary>
        /// Gets or Sets the Regex String for AlphaNumeric FilterBarType
        /// </summary>
        public string AlphaNumericRegexString
        {
            get { return alphaNumericRegexString; }
            set { alphaNumericRegexString = value; }
        }
        


        /// <summary>
        /// Displays the Error Tooltip when the filtered value was worng
        /// </summary>
        public string ErrorToolTip
        {
            get { return errorToolTip; }
            set
            {
                errorToolTip = value;
                if (HasCurrentCellState)
                {
                    if (CurrentCellUIElement != null && errorToolTip.Trim() != EmptyString)
                    {
                        ToolTipService.SetToolTip(CurrentCellUIElement, ErrorToolTip);
                        SetStatusMessage(ErrorToolTip + " " + GetStatusMessage());
                    }
#if !SILVERLIGHT
                    else if (CurrentCellUIElement != null)
                    {
                        CurrentCellUIElement.ToolTip = null;
                    }
#endif
                }
            }
        }

        private FilterType defaultFilterType = FilterType.Undefined;

        public FilterType DefaultFilterType
        {
            get
            {
                if (defaultFilterType == Linq.FilterType.Undefined)
                {
                    return GetFilterType();
                }
                return defaultFilterType;
            }
            set { defaultFilterType = value; }
        }
        
        /// <summary>
        /// Gets the Current FilterBar Type
        /// </summary>
        public GridDataFilterBarType FilterBarType
        {
            get
            {
                return GetCurrentFilterBarType(); 
            }            
        }

        private GridDataFilterBarMode filterBarMode = GridDataFilterBarMode.Immediate;

        /// <summary>
        /// Gets or Sets the FilterBarMode for the current Visible Column
        /// </summary>
        public GridDataFilterBarMode FilterBarMode
        {
            get { return filterBarMode; }
            set { filterBarMode = value; }
        }

        #endregion
        
        #region Constructor
        /// <summary>
        /// Initializes a new <see cref="GridDataFilterBarCellRenderer"/>.
        /// </summary>
        public GridDataFilterBarCellRenderer()
        {
#if !SILVERLIGHT
            SupportsRenderOptimization = true;
#else
            SupportsRenderOptimization = true;
#endif
            AllowRecycle = false;
            IsControlTextShown = true;
            IsFocusable = true;
            AllowKeepAliveOnlyCurrentCell = true;
        }
        #endregion

        #region Base Methods


        protected override void OnGridPreviewTextInput(TextCompositionEventArgs e)
        {
            TextBox tb;
#if !SILVERLIGHT

            if (CurrentCell.IsEditing)// || (tb != null && tb.IsFocused))
                return;
#else
            if (CurrentCell.IsEditing)
                return;
            CurrentCell.BeginEdit(true);//Key press should enter into Edit Mode.
            tb = CurrentCellUIElement;
#endif

            CurrentCell.ScrollInView();
            CurrentCell.BeginEdit(true);
            tb = CurrentCellUIElement;


            if (tb != null)
            {
                //SetControlText(e.Text);
                System.Text.RegularExpressions.Regex regex = new System.Text.RegularExpressions.Regex(@"[\b]");
                string str = regex.Replace(e.Text, EmptyString);
                tb.Text += str;
#if !SILVERLIGHT
                tb.CaretIndex = tb.Text.Length;
#else
                tb.SelectionStart = tb.Text.Length;
#endif
            }
            e.Handled = true;
        }

        protected override void OnSetFocus()
        {
            if (!ActivateOptions.IsActivateTriggeredByMouseDownIntoUIElement)
                CurrentCellUIElement.SelectAll();

        }

#if !SILVERLIGHT

        protected override bool ShouldGridTryToHandlePreviewKeyDown(KeyEventArgs e)
        {
            // return false to indicate the CurrentCellUIElement should handle the key
            // and the grid should ignore it.
            bool isControlKey = (e.KeyboardDevice.Modifiers & ModifierKeys.Control) != ModifierKeys.None;
            bool isShiftKey = (e.KeyboardDevice.Modifiers & ModifierKeys.Shift) != ModifierKeys.None;
            if (isControlKey)
                return true;
            switch (e.Key)
            {
                case Key.Tab:
                    return true;

                case Key.Left:
                    {
                        //if (isShiftKey)
                        //{
                        //    return false;
                        //}
                        //TextBox tb = this.CurrentCellUIElement;
                        //if (tb != null)
                        //{
                        //    if (tb.SelectionStart == 0)
                        //        return true;
                        //    else
                        //        return false;
                        //}
                        //return false;
                        if (this.CurrentCell.IsEditing)
                        {
                            if (isShiftKey)
                            {
                                return false;
                            }
                            TextBox tb = this.CurrentCellUIElement as TextBox;
                            if (tb != null)
                            {
                                if (tb.CaretIndex == 0 && tb.SelectionLength == 0)
                                {
                                    //e.Handled = true;
                                    return true;
                                } // When the entire text is selected pressing right arrow key moves the focus to the next cell, whereas in Excel the cursor moves to the last position. The below code is added to move the cursor to the last position
                                else if (tb.SelectionLength == tb.Text.Length)
                                {
                                    tb.CaretIndex = tb.Text.Length;
                                    e.Handled = true;
                                    return false;
                                }
                                // this is removed since this will suitable for right click only.
                                //else if (tb.CaretIndex == tb.Text.Length)
                                //    return true;
                                else
                                    return false;
                            }
                        }
                        return true;   
                    }
                case Key.Right:
                    {
                        if (this.CurrentCell.IsEditing)
                        {
                            if (isShiftKey)
                            {
                                return false;
                            }
                            TextBox tb = this.CurrentCellUIElement as TextBox;
                            if (tb != null)
                            {
                                if (tb.CaretIndex == 0 && tb.Text.Length == 0)
                                {
                                    //e.Handled = true;
                                    return true;
                                }// When the entire text is selected pressing right arrow key moves the focus to the next cell, whereas in Excel the cursor moves to the last position. The below code is added to move the cursor to the last position
                                if (tb.SelectionLength == tb.Text.Length)
                                {
                                    tb.CaretIndex = tb.Text.Length;
                                    e.Handled = true;
                                    return false;
                                }
                                else if (tb.CaretIndex == tb.Text.Length)
                                    return true;
                                else
                                    return false;
                            }
                        }
                        return true;   
                    }
                case Key.Down:
                case Key.Up:
                    {
                        // Move to next cell when whole text in textbox is selected.
                        TextBox tb = CurrentCellUIElement;
                        if (tb != null && tb.SelectionStart == 0 && tb.SelectionLength == tb.Text.Length)
                            return true;

                        // otherwise, move caret within textbox when cell is not in edit-mode.
                        return !CurrentCell.IsEditing;
                    }

                case Key.End:
                case Key.Home:
                    {
                        TextBox tb = this.CurrentCellUIElement as TextBox;
                        if (tb != null && tb.IsFocused)
                            return false;
                        else
                            return true;
                    }

                case Key.Delete:
                    {
                        CurrentCell.BeginEdit(true);
                        SetControlText(EmptyString);
                        return false;
                    }

                case Key.F2:
                    {
                        var textBox = this.CurrentCellUIElement;
                        if (textBox != null && this.CurrentCell.IsEditing && textBox.SelectionLength == textBox.Text.Length)
                        {
                            textBox.CaretIndex = textBox.Text.Length;
                            e.Handled = true;
                            return false;
                        }
                    }
                    break;
                case Key.Enter:
                    //if (!HasCurrentCellState || this.IsFilteringSuspend)
                    var gdcModel = (GridDataTableModel)this.GridControl.Model;

                    var filterTextBox = CurrentCellUIElement;
                    if (gdcModel != null && filterTextBox != null && gdcModel.TableProperties.FilterBarMode == GridDataFilterBarMode.OnEnter)
                    {
                        bool skipFilter = false;

                        string[] skipInput = { this.ContainsString, this.LessThanString, this.LessThanOrEqualString, this.GreaterThanString, this.GreaterThanOrEqualString, this.EqualsString, this.NotString };
                        ErrorToolTip = EmptyString;
                        FilterValue = filterTextBox.Text;
                       
                        if (gdcModel != null && gdcModel.TableProperties != null && gdcModel.TableProperties.VisibleColumns.Count > 0)
                        {
                            var visCol = GetCurrentColumn();

                            if (visCol == null) skipFilter = true;
                            if ((gdcModel.TableProperties.FilterBarMode == GridDataFilterBarMode.Immediate && visCol.FilterBarMode == GridDataFilterBarMode.OnEnter) ||
                                (gdcModel.TableProperties.FilterBarMode == GridDataFilterBarMode.OnEnter && visCol.FilterBarMode == GridDataFilterBarMode.OnEnter)) skipFilter = true;
                        }
                        if (!skipFilter)
                            ValidataFilter();
                    }
                    this.CurrentCell.MoveRight();                        
                    e.Handled = true;
                    return true;


            }

            return base.ShouldGridTryToHandlePreviewKeyDown(e);
        }
#endif

        /// <summary>
        /// Raises GridCellClick event.
        /// </summary>
        /// <param name="rowIndex">The cell row index.</param>
        /// <param name="colIndex">The cell column index.</param>
        /// <param name="e">A <see cref="MouseControllerEventArgs"/> object.</param>
        public override void RaiseGridCellClick(int rowIndex, int colIndex, MouseControllerEventArgs e)
        {
            if (CurrentCell.HasCurrentCellAt(rowIndex, colIndex)
                && !CurrentCell.IsEditing
                && ((GridControl.Model.Options.ActivateCurrentCellBehavior & GridCellActivateAction.DblClickOnCell) == 0))
            {
                CurrentCell.BeginEdit(true);

            }
#if !SILVERLIGHT
            if (this.CurrentCellUIElement != null)
                this.CurrentCellUIElement.CaretIndex = this.CurrentCellUIElement.Text.Length;
            base.RaiseGridCellClick(rowIndex, colIndex, e);
#endif

        }

//#if !SILVERLIGHT
//        public override void CreateRendererElement(TextBox uiElement, GridRenderStyleInfo style)
//        {
//            uiElement.IsReadOnly = style.ReadOnly;
//            base.CreateRendererElement(uiElement, style);
//        }
//#endif
        /// <summary>
        /// Initializes the content of the textbox cell
        /// using the information from the cell style (value, text,
        /// behavior etc.).
        /// </summary>
        /// <param name="textBox">The text box.</param>
        /// <param name="style">The cell style info.</param
        public override void OnInitializeContent(TextBox textBox, GridRenderStyleInfo style)
        {
            base.OnInitializeContent(textBox, style);

            // TextBoxView always seems to have this margin and I am not able to reset the margin.
            // Therefore I am also hard-codeing it here so that TextBox behavior is properly
            // emulated.
            Thickness margins = style.TextMargins.ToThickness();
            margins.Left = Math.Max(0, margins.Left - 5);
            margins.Right = Math.Max(0, margins.Right - 5);
            margins.Top = Math.Max(0, margins.Top - 3);
            margins.Bottom = Math.Max(0, margins.Bottom - 3);

#if !SILVERLIGHT
            if (style.HasImageIndex)
            {
                margins = style.AdjustImageWidthAndHeightToMargin(margins, style.GridControl);
            }
            else
            {
                margins = style.ErrorInfo.AdjustErrorInfoMargin(margins, style.GridControl, style.CellRowColumnIndex);
            }
#endif
            textBox.Padding = margins;

            this.IsFilteringSuspend = true;
            textBox.Background = Brushes.White;
            textBox.Foreground = Brushes.Black;
            if (IsCurrentCell(style) && HasControlText && this.CurrentCell.IsEditing)//&& style.CellType != "FormulaCell")
            {
                textBox.Text = GetControlTextFromEditor();
            }
#if SILVERLIGHT
            /// To retain the text on entering edit mode, fix is applied based on TextBox Cell renderer
            else
            {
                textBox.Text = GetControlText(style);
            }
#endif

            this.IsFilteringSuspend = false;
        }

        protected override void ArrangeUIElement(ArrangeCellArgs aca, TextBox textBox, GridRenderStyleInfo style)
        {
            if (style.TextWrapping != TextWrapping.NoWrap)
            {
                Rect bounds = aca.CellRect;
#if !SILVERLIGHT
                if (bounds.Height < style.Font.GetLineHeightValue() * 2)
                    textBox.TextWrapping = TextWrapping.NoWrap;
#else
                if (bounds.Height < style.Font.FontSize * 2)
                    textBox.TextWrapping = TextWrapping.NoWrap;
#endif
            }
            if (style.Font.Orientation != 0)
            {
                textBox.RenderTransform = null;
            }

            if (style.Font.Orientation != 0)
            {
                textBox.RenderTransform = null;
            }
            Thickness margins = style.TextMargins.ToThickness();

            margins = style.ErrorInfo.AdjustErrorInfoMargin(margins, style.GridControl, style.CellRowColumnIndex);

            textBox.Padding = margins;
            base.ArrangeUIElement(aca, textBox, style);
        }

#if !SILVERLIGHT

        bool ignoreTextChanged = false;

        protected override void OnRender(DrawingContext dc, RenderCellArgs rca, GridRenderStyleInfo style)
        {
            if (rca.CellUIElements != null)
                return;

            // Will only get hit if SupportsRenderOptimization is true, otherwise rca.CellUIElements is never null.
            var margins = style.TextMargins.ToThickness();
            if (style.HasImageIndex)
            {
                margins = style.AdjustImageWidthAndHeightToMargin(margins, rca.CellRect.Size);
            }
            else
            {
                margins = style.ErrorInfo.AdjustErrorInfoMargin(margins, rca.CellRect.Size);
            }
            Rect textRectangle = rca.SubtractBorderMargins(rca.CellRect, margins);
            if (textRectangle.IsEmpty)
                return;

            margins.Left = Math.Max(margins.Left, 2);
            margins.Right = Math.Max(margins.Right, 2);
            string text = GetControlText(style);

            // Draw the formatted text string to the DrawingContext of the control.
            GridTextBoxPaint.DrawText(dc, textRectangle, text, style);
            //base.OnRender(dc, rca, style);
            //if (rca.CellUIElements != null && rca.CellUIElements.UIElements != null &&
            //    rca.CellUIElements.UIElements.Count > 0 && rca.CellUIElements.UIElements[0] is TextBox &&
            //    ((TextBox)rca.CellUIElements.UIElements[0]).Text == string.Empty && style.Text != string.Empty)
            //{
            //    ignoreTextChanged = true;
            //    ((TextBox)rca.CellUIElements.UIElements[0]).Text = style.Text; 
            //    ignoreTextChanged = false;
            //}
        }
#endif

        protected override void OnInitialize()
        {
            base.OnInitialize();
            textBoxSelectionStart = -1;
            ControlText = GetControlText(CurrentStyle);
        }
#if !SILVERLIGHT
        //ScrollViewer sv;
        protected override void OnActivated()
        {
#else
        protected override void OnActivated()
        {
#endif


            if (this.CurrentCellUIElement != null)
            {
                this.CurrentCellUIElement.Focus();
            }
        }

#if SILVERLIGHT
        protected override bool ShouldGridTryToHandlePreviewKeyDown(KeyEventArgs e)
        {
            bool isControlKey = (e.Key == Key.Shift);
            if (isControlKey)
                return true;
            switch (e.Key)
            {
                case Key.Enter:
                    var gdcModel = (GridDataTableModel)this.GridControl.Model;
                    if (gdcModel != null && gdcModel.TableProperties.FilterBarMode == GridDataFilterBarMode.OnEnter)
                    {
                        var filterTextBox = CurrentCellUIElement;
                        bool skipFilter = false;

                        string[] skipInput = { ">", "<", "=", "!", " " };
                        ErrorToolTip = "";
                        FilterValue = filterTextBox.Text;

                        if (gdcModel != null && gdcModel.TableProperties != null && gdcModel.TableProperties.VisibleColumns.Count > 0)
                        {
                            var visCol = GetCurrentColumn();

                            if (visCol == null) skipFilter = true;
                            if ((gdcModel.TableProperties.FilterBarMode == GridDataFilterBarMode.Immediate && visCol.FilterBarMode == GridDataFilterBarMode.OnEnter) ||
                                (gdcModel.TableProperties.FilterBarMode == GridDataFilterBarMode.OnEnter && visCol.FilterBarMode == GridDataFilterBarMode.OnEnter)) skipFilter = true;
                        }
                        if (!skipFilter)
                            ValidataFilter();
                    }
                    return false;
            }
            return base.ShouldGridTryToHandlePreviewKeyDown(e);
        }
#endif


        protected override string GetControlTextFromEditorCore(TextBox textBox)
        {
            return textBox.Text;
        }

        /// <summary>
        /// Refreshes the textbox UnwireTemplatePartscell.
        /// </summary>
        public override void RefreshContent()
        {
            base.RefreshContent();
            if (textBoxSelectionStart != -1 && CurrentCellUIElement != null)
                CurrentCellUIElement.Select(textBoxSelectionStart, textBoxSelectionLength);
        }

        /// <summary>
        /// Wire events from textBox
        /// </summary>
        /// <param name="textBox"></param>
        protected override void OnWireUIElement(TextBox textBox)
        {
            base.OnWireUIElement(textBox);
            textBox.SelectionChanged += textBox_SelectionChanged;
            textBox.TextChanged += textBox_TextChanged;
            textBox.KeyDown += textBox_KeyDown;
#if !SILVERLIGHT
            textBox.PreviewKeyDown += textBox_PreviewKeyDown;
#endif
        }

        /// <summary>
        /// Unwire previously wired events from textBox. 
        /// </summary>
        /// <param name="textBox"></param>
        protected override void OnUnwireUIElement(TextBox textBox)
        {
            base.OnUnwireUIElement(textBox);
            textBox.SelectionChanged -= textBox_SelectionChanged;
            textBox.TextChanged -= textBox_TextChanged;
            textBox.KeyDown -= textBox_KeyDown;
#if !SILVERLIGHT
            textBox.PreviewKeyDown -= textBox_PreviewKeyDown;
#endif
        }

        protected override bool OnDeactivating()
        {
            bool returnValue = base.OnDeactivating();

            if (returnValue)
            {
                var currentColumn = GetCurrentColumn();
                ObservableCollection<FilterPredicate> filtersTobeRemoved = new ObservableCollection<FilterPredicate>();

                foreach (var filter in currentColumn.Filters)
                {
                    if (filter.FilterType == FilterType.Undefined && filter.FilterValue.ToString().Length < 2)
                        filtersTobeRemoved.Add(filter);
                }

                foreach (var filter in filtersTobeRemoved)
                {
                    currentColumn.Filters.Remove(filter);
                }
            }

            return returnValue;
        }

        #endregion

        #region Private Methods

#if SILVERLIGHT
        internal static string GetStringForUnknownKey(int platformKeyCode, bool isShiftPressed)
        {
            switch (platformKeyCode)
            {
                case 0XBA:
                    if (isShiftPressed) return ":";
                    else return ";";
                case 0XBF:
                    if (isShiftPressed) return "?";
                    else return "/";
                case 0XC0:
                    if (isShiftPressed) return "~";
                    else return "`";
                case 0XDB:
                    if (isShiftPressed) return "{";
                    else return "[";
                case 0XDC:
                    if (isShiftPressed) return "|";
                    else return "\\";
                case 0XDD:
                    if (isShiftPressed) return "}";
                    else return "]";
                case 0XDE:
                    if (isShiftPressed) return "\"";
                    else return "'";
                case 0XBB:
                    if (isShiftPressed) return "+";
                    else return "=";
                case 0xBC:
                    if (isShiftPressed) return "<";
                    else return ",";
                case 0XBD:
                    if (isShiftPressed) return "-";
                    else return "_";
                case 0XBE:
                    if (isShiftPressed) return ">";
                    else return ".";
                case 0X31:
                    return "<>";
                default:
                    return "unknown";
            }
        }
#endif
        internal static string GetStringForKey(Key key, bool isShiftPressed)
        {
            switch (key)
            {
                case Key.D0:
                    if (isShiftPressed) return ")";
                    else return "0";
                case Key.D1:
                    if (isShiftPressed) return "!";
                    else return "1";
                case Key.D3:
                    if (isShiftPressed) return ContainsToken;
                    else return "3";
                case Key.D5:
                    if (isShiftPressed) return StartsWithToken;
                    else return "5";
#if !SILVERLIGHT
                case Key.Oem1:
                    return ";";
                case Key.Oem2:
                    return "/";
                case Key.Oem3:
                    return "`";
                case Key.Oem4:
                    return "[";
                case Key.Oem5:
                    return "\\";
                case Key.Oem6:
                    return "]";
                case Key.Oem7:
                    return "'";
                case Key.OemComma:
                    if (isShiftPressed) return "<";
                    else return ",";
                case Key.OemPeriod:
                    if (isShiftPressed) return ">";
                    else return ".";
                case Key.OemMinus:
                    return "-";
                case Key.OemPlus:
                    if (isShiftPressed) return "+";
                    else return "=";
                case Key.Oem102:
                    return "<>";
                case Key.Subtract:
                    return "-";
                case Key.Decimal:
                    return ".";
#endif
                default:
                    return key.ToString();
            }
        }


        public bool ValidateCurrentColumn()
        {
            var visCol = GetCurrentColumn();
            if (visCol == null)
                return false;

            var model = this.GridControl.Model as GridDataTableModel;
            if (model != null && model.TableProperties != null && model.TableProperties.VisibleColumns.Count > 0)
            {
                var currentColIndex = model.TableProperties.VisibleColumns.IndexOf(visCol);
                if (currentColIndex >= 0 && model.TableProperties.VisibleColumns.Count > currentColIndex)
                {
                    return true;
                }
            }
            return false;
        }

        private string[] GetFilterTokens()
        {
            string[] token = new string[8];
            token[0] = this.ContainsString;
            token[1] = this.EqualsString;
            token[2] = this.NotString;
            token[3] = this.StartWithEndWithString;
            token[4] = this.lessThanOrEqualString;
            token[5] = this.lessThanString;
            token[6] = this.GreaterThanOrEqualString;
            token[7] = this.GreaterThanString;
            return token;
        }

        public FilterType GetFilterType()
        {
            var model = this.GridControl.Model as GridDataTableModel;
            if (model != null)
            {
                var type = model.TableProperties.DefaultFilterOperator;
                switch (type)
                {
                    case FilterOperatorType.Contains:
                        return FilterType.Contains;
                    case FilterOperatorType.Equals:
                        return FilterType.Equals;
                    case FilterOperatorType.StartsWith:
                        return FilterType.StartsWith;
                }
            }
            return FilterType.Undefined;
        }
        
        /// <summary>
        /// Returns the Current FilterValue else returns empty String
        /// </summary>
        public string GetCurrentFilterValue()
        {
            var currVisCol = GetCurrentColumn();
            if (currVisCol != null && currVisCol.Filters.Count > 0)
            {
                return currVisCol.Filters[0].FilterValue.ToString();
            }
            return "";
        }

        /// <summary>
        /// Returns the Current GridDataVisibleColumn.
        /// </summary>
        /// <returns></returns>
        public GridDataVisibleColumn GetCurrentColumn()
        {
            GridDataVisibleColumn column = default(GridDataVisibleColumn);
            if (this.HasCurrentCellState)
            {
                var currentColIndex = this.CellRowColumnIndex.ColumnIndex;
                var gdcModel = this.GridControl.Model as GridDataTableModel;
                if (gdcModel != null && gdcModel.TableProperties != null && gdcModel.TableProperties.VisibleColumns.Count > 0 && currentColIndex >= 0)
                {
                    var colOffset = gdcModel.ResolveDefaultColumnOffset();
                    currentColIndex = currentColIndex - colOffset;
                    if (currentColIndex < 0) currentColIndex = 0;
                    if (gdcModel.TableProperties.VisibleColumns.Count > currentColIndex)
                        column = gdcModel.TableProperties.VisibleColumns[currentColIndex];
                }
            }
            return column;
        }


        /// <summary>
        /// Returns the GridDataFilterBarType based on the VisibleColumn ColumnType.
        /// </summary>
        /// <returns></returns>
        public GridDataFilterBarType GetCurrentFilterBarType()
        {
            if (this.GridControl != null && this.GridControl.Model != null)
            {
                // var gdcModel = (GridDataTableModel)this.GridControl.Model; Unused local variable
                var visCol = GetCurrentColumn();

                if (visCol != null)
                {
                    var coltype = visCol.ColumnType;
                    if (coltype == null) return GridDataFilterBarType.NotUsed;

                    switch (coltype.FullName)
                    {
                        case "System.Boolean":
                            return GridDataFilterBarType.Boolean;
                        case "System.String":
                            return GridDataFilterBarType.AlphaNumeric;
                        case "System.DateTime":
                            return GridDataFilterBarType.DateTime;
                        case "System.Int32":
                        case "System.Int16":
                        case "System.Int64":
                        case "System.SByte":
                        case "System.Byte":
                        case "System.UInt16":
                        case "System.UInt32":
                        case "System.UInt64":
                        case "System.Double":
                        case "System.Single":
                        case "System.Decimal":
                            return GridDataFilterBarType.Numeric;
                        case "System.Char":
                            return GridDataFilterBarType.AlphaNumeric;
                        default:
                            return GridDataFilterBarType.NotUsed;
                    }
                }
            }

            return GridDataFilterBarType.NotUsed;
        }

        private bool CheckFilterBarTypeValid(string filteredval, GridDataFilterBarType filteType)
        {
            switch (filteType)
            {
                case GridDataFilterBarType.AlphaNumeric:
                    string val = filteredval.Trim();
                    if (val.Equals(string.Empty) || val.Length < 1)
                        return false;
                    return true;
                case GridDataFilterBarType.Numeric:
                    //var numPattern = @"(\+|-)?[0-9]+(\.[0-9]*)?\d*$"; - RegEx allowed ?[0-9] to numeric filter.This is not correct
                    var numPattern = @"^-?\d+([\.]{1}\d*)?$";
                    if (Regex.IsMatch(filteredval, numPattern)) return true;
                    break;
                case GridDataFilterBarType.Boolean:
                    var strArrays = filteredval.ToArray();
                    {
                        string _false = "false";
                        string _true = "true";
                        if (_true.Contains(filteredval.ToString().ToLower()))
                        {
                            strArrays = "1".ToArray();
                            filteredval = strArrays.ToString();

                        }
                        else if (_false.Contains(filteredval.ToString().ToLower()))
                        {
                            strArrays = "0".ToArray();
                            filteredval = strArrays.ToString();
                        }

                    }
                    foreach (var item in strArrays)
                    {
                        if (item.ToString() == "0") continue;
                        else if (item.ToString() == "1") continue;
                        else return false;
                    }
                    return true;
                case GridDataFilterBarType.DateTime:
                    #region Date Time Validation
                    DateTime validDateTime;
                    if (!DateTime.TryParse(filteredval, CultureInfo.CurrentCulture.DateTimeFormat, DateTimeStyles.NoCurrentDateDefault, out validDateTime))
                    {
                        return false;
                    }
                    return true;
                    #endregion
                case GridDataFilterBarType.NotUsed:
                default:
                    break;
            }
            return false;
        }

        private bool ValidateNumericDateTime(string textBoxValue, string newString)
        {
            var strarrys = textBoxValue.ToArray();
            var cnt = 0;
            var prev = 'u';
            foreach (char item in strarrys)
            {
                if (item == '!')
                {
                    prev = '!';
                    cnt++;
                }
                else if (item == '=' && prev == '=') return false;
                else if (item == '=' && prev != '<' && prev != '>')
                {
                    prev = '=';
                    cnt++;
                }
                else if ((item == '<' || item == '>' || item == '!' || item == '=') && prev == '!') return false;
                else if (item == '<' || item == '>' || item == '!' || item == '=' && ((newString != "=" && cnt < 1) || (newString == "=" && cnt >= 1)))
                {
                    var cntval = from res in textBoxValue.ToArray()
                                 where res == '='
                                 select res;
                    prev = item;
                    if (cntval.Count() == 1 && newString == "=" && item == '=')
                    {
                    }
                    else cnt++;
                }
            }
            if (cnt > 1) return false;
            return true;
        }

        private string RemoveEscapeSeq(string filtervalue, bool removeSpace)
        {
            filtervalue = filtervalue.Replace("\r", EmptyString);
            filtervalue = filtervalue.Replace("\n", EmptyString);
            if (removeSpace)
            {
                filtervalue = filtervalue.Replace(" ", EmptyString);
            }
            else
            {
                filtervalue = filtervalue.Trim();
            }

            return filtervalue;
        }


        /// <summary>
        /// This method is used to update the FilterBar Status message diplayed in the bottom of the Grid
        /// </summary>
        /// <param name="filter"></param>
        /// <param name="visColumn"></param>
        /// <returns></returns>
        private string GetFilterTypeStrindEqu(FilterPredicate filter, GridDataVisibleColumn visColumn)
        {
            string filtervalue = filter.FilterValue.ToString();
            Linq.FilterType filterType = filter.FilterType;
            string colname = "";
            if (visColumn != null)
            {
                colname = (visColumn.HeaderText != "") ? visColumn.HeaderText : (visColumn.MappingName != "" ? visColumn.MappingName : "");
            }
            colname = LeftBraceString + colname + RightBraceString;
            switch (filterType)
            {
                case FilterType.LessThan:
                    return AddQuotes(colname, this.LessThanString, filtervalue);
                case FilterType.LessThanOrEqual:
                    return AddQuotes(colname, this.LessThanOrEqualString, filtervalue);
                case FilterType.Equals:
                    return AddQuotes(colname, this.EqualsString, filtervalue);
                case FilterType.NotEquals:
                    return AddQuotes(colname, this.NotString, filtervalue);
                case FilterType.GreaterThanOrEqual:
                    return AddQuotes(colname, this.GreaterThanOrEqualString, filtervalue);
                case FilterType.GreaterThan:
                    return AddQuotes(colname, this.GreaterThanString, filtervalue);
                case FilterType.EndsWith:
                    return AddQuotes(colname, this.StartWithEndWithString, filtervalue);
                case FilterType.StartsWith:
                    return AddQuotes(colname, filtervalue, this.StartWithEndWithString);
                case FilterType.Contains:
                    return AddQuotes(colname, this.ContainsString, filtervalue);
                case FilterType.Undefined:
                default:
                    return AddQuotes(colname, "", filtervalue);
            }
        }

        private string AddQuotes(string columnname, string token, string filtervalue)
        {
            return columnname + QuoteString + token + filtervalue + QuoteString;
        }


        private void ValidataFilter()
        {
            if (this.IsFilteringSuspend)
                return;
            var tokenizer = new GridDataFiltersTokenizer();
            tokenizer.CurrentFilterBarType = this.FilterBarType;
            tokenizer.ContainsString = this.ContainsString;
            tokenizer.GreaterThanOrEqualString = this.GreaterThanOrEqualString;
            tokenizer.GreaterThanString = this.GreaterThanString;
            tokenizer.LessThanOrEqualString = this.LessThanOrEqualString;
            tokenizer.LessThanString = this.LessThanString;
            tokenizer.NotString = this.NotString;
            tokenizer.StartsAndEndsWithString = this.StartWithEndWithString;
            tokenizer.isTokenSetOutside = this.isTokensSetOutside;
            tokenizer.Tokens = GetFilterTokens();
            var filterType = this.GetFilterType();
            var gdcModel = (GridDataTableModel)this.GridControl.Model;
            var visCol = GetCurrentColumn();
            if (this.ValidateCurrentColumn())
            {
                if (FilterValue == string.Empty)
                {
                    ClearCurrentColumnFilter();
                    return;
                }
                // the Following Block replaces "&&" and "||" Symbol as Predicate for the Filter Predicate. 
                FilterValue = FilterValue.Replace(AndSymbolToken, AndStringToken).Replace(OrSymbolToken, OrStringToken).ToString();
                if (this.FilterBarType == GridDataFilterBarType.AlphaNumeric)
                {
                    if (FilterValue.ToLower().Contains(AndStringToken) || FilterValue.ToLower().Contains(OrStringToken))
                    {
                        string tempfiltervalue = FilterValue.ToLower();
                        StringBuilder filterval = new StringBuilder();

                        int OrPredicateIndex = tempfiltervalue.IndexOf(OrStringToken);
                        int AndPredicateIndex = tempfiltervalue.IndexOf(AndStringToken);
                        //If the FilterValue contains both AND condition and OR condition.
                        if (AndPredicateIndex > 0 && OrPredicateIndex > 0)
                        {
                            int predicatecount = tempfiltervalue.Length;
                            for (int count = 0; count < predicatecount; count++)
                            {
                                OrPredicateIndex = tempfiltervalue.IndexOf(OrStringToken);
                                AndPredicateIndex = tempfiltervalue.IndexOf(AndStringToken);
                                if (OrPredicateIndex == -1 && AndPredicateIndex == -1)
                                {
                                    FilterValue = filterval.ToString();
                                    break;
                                }
                                if ((OrPredicateIndex < AndPredicateIndex || AndPredicateIndex == -1) && OrPredicateIndex != -1)
                                {
                                    string tempval = tempfiltervalue.Substring(0, OrPredicateIndex);
                                    if (this.DefaultFilterType == FilterType.Contains)
                                    {
                                        if (!tempval.Contains(this.ContainsString))
                                            tempval = ContainsToken + tempval + AndStringToken;
                                        else
                                            tempval = tempval + OrStringToken;
                                    }
                                    if (this.DefaultFilterType == FilterType.StartsWith)
                                    {
                                        if (!tempval.Contains(this.StartWithEndWithString))
                                            tempval = tempval + StartsWithToken + OrStringToken;
                                        else
                                            tempval = tempval + OrStringToken;
                                    }
                                    if (this.DefaultFilterType == FilterType.Equals)
                                    {
                                        if (!tempval.Contains(this.EqualsString))
                                            tempval = EqualToken + tempval + OrStringToken;
                                        else
                                            tempval = tempval + OrStringToken;
                                    }
                                    filterval.Append(tempval);
                                    tempfiltervalue = tempfiltervalue.Substring(OrPredicateIndex + 4);
                                    OrPredicateIndex = tempfiltervalue.IndexOf(OrStringToken);
                                    if (OrPredicateIndex == -1 && AndPredicateIndex == -1)
                                    {
                                        if (!tempfiltervalue.Contains(this.ContainsString))
                                            tempfiltervalue = ContainsToken + tempfiltervalue;
                                        filterval.Append(tempfiltervalue);
                                        FilterValue = filterval.ToString();
                                        break;
                                    }
                                }
                                else
                                {
                                    string tempval = tempfiltervalue.Substring(0, AndPredicateIndex);
                                    if (this.DefaultFilterType == FilterType.Contains)
                                    {
                                        if (!tempval.Contains(this.ContainsString))
                                            tempval = ContainsToken + tempval + AndStringToken;
                                        else
                                            tempval = tempval + AndStringToken;
                                    }
                                    if (this.DefaultFilterType == FilterType.StartsWith)
                                    {
                                        if (!tempval.Contains(this.StartWithEndWithString))
                                            tempval = tempval + StartsWithToken + AndStringToken;
                                        else
                                            tempval = tempval + AndStringToken;
                                    }
                                    if (this.DefaultFilterType == FilterType.Equals)
                                    {
                                        if (!tempval.Contains(this.EqualsString))
                                            tempval = EqualToken + tempval + AndStringToken;
                                        else
                                            tempval = tempval + AndStringToken;
                                    }
                                    filterval.Append(tempval);
                                    tempfiltervalue = tempfiltervalue.Substring(AndPredicateIndex + 5);
                                    AndPredicateIndex = tempfiltervalue.IndexOf(AndStringToken);
                                    if (AndPredicateIndex == -1 && OrPredicateIndex == -1)
                                    {
                                        if (this.DefaultFilterType == FilterType.Contains)
                                        {
                                            if (!tempfiltervalue.Contains(this.ContainsString))
                                                tempfiltervalue = ContainsToken + tempfiltervalue;
                                        }
                                        else if (this.DefaultFilterType == FilterType.StartsWith)
                                        {
                                            if (!tempfiltervalue.Contains(this.StartWithEndWithString))
                                                tempfiltervalue = tempfiltervalue + StartsWithToken;
                                        }
                                        else if (this.DefaultFilterType == FilterType.Equals)
                                        {
                                            if (!tempfiltervalue.Contains(this.EqualsString))
                                                tempfiltervalue = EqualToken + tempfiltervalue;
                                        }
                                        filterval.Append(tempfiltervalue);
                                        FilterValue = filterval.ToString();
                                        break;
                                    }
                                }
                            }
                        }
                        else if (AndPredicateIndex == -1)
                        {
                            int predicatecount = 5;
                            for (int count = 0; count < predicatecount; count++)
                            {
                                OrPredicateIndex = tempfiltervalue.IndexOf(OrStringToken);
                                if (OrPredicateIndex == -1)
                                {
                                    FilterValue = filterval.ToString();
                                    break;
                                }
                                string tempval = tempfiltervalue.Substring(0, OrPredicateIndex);
                                if (this.DefaultFilterType == FilterType.Contains)
                                {
                                    if (!tempval.Contains(this.ContainsString))
                                        tempval = ContainsToken + tempval + OrStringToken;
                                    else
                                        tempval = tempval + OrStringToken;
                                }
                                if (this.DefaultFilterType == FilterType.StartsWith)
                                {
                                    if (!tempval.Contains(this.StartWithEndWithString))
                                        tempval = tempval + StartsWithToken + OrStringToken;
                                    else
                                        tempval = tempval + OrStringToken;
                                }
                                if (this.DefaultFilterType == FilterType.Equals)
                                {
                                    if (!tempval.Contains(this.EqualsString))
                                        tempval = EqualToken + tempval + OrStringToken;
                                    else
                                        tempval = tempval + OrStringToken;
                                }
                                filterval.Append(tempval);
                                tempfiltervalue = tempfiltervalue.Substring(OrPredicateIndex + 4);
                                OrPredicateIndex = tempfiltervalue.IndexOf(OrStringToken);
                                if (OrPredicateIndex == -1)
                                {
                                    if (this.DefaultFilterType == FilterType.Contains)
                                    {
                                        if (!tempfiltervalue.Contains(this.ContainsString))
                                            tempfiltervalue = ContainsToken + tempfiltervalue;
                                    }
                                    else if (this.DefaultFilterType == FilterType.StartsWith)
                                    {
                                        if (!tempfiltervalue.Contains(this.StartWithEndWithString))
                                            tempfiltervalue = tempfiltervalue + StartsWithToken;
                                    }
                                    else if (this.DefaultFilterType == FilterType.Equals)
                                    {
                                        if (!tempfiltervalue.Contains(this.EqualsString))
                                            tempfiltervalue = EqualToken + tempfiltervalue;
                                    }
                                    filterval.Append(tempfiltervalue);
                                    FilterValue = filterval.ToString();
                                    break;
                                }
                            }
                        }
                        else if (OrPredicateIndex == -1)
                        {
                            int predicatecount = 5;
                            for (int count = 0; count < predicatecount; count++)
                            {
                                AndPredicateIndex = tempfiltervalue.IndexOf(AndStringToken);
                                if (AndPredicateIndex == -1)
                                {
                                    FilterValue = filterval.ToString();
                                    break;
                                }
                                string tempval = tempfiltervalue.Substring(0, AndPredicateIndex);
                                if (this.DefaultFilterType == FilterType.Contains)
                                {
                                    if (!tempval.Contains(this.ContainsString))
                                        tempval = ContainsToken + tempval + AndStringToken;
                                    else
                                        tempval = tempval + AndStringToken;
                                }
                                else if (this.DefaultFilterType == FilterType.StartsWith)
                                {
                                    if (!tempval.Contains(this.StartWithEndWithString))
                                        tempval = tempval + StartsWithToken + AndStringToken;
                                    else
                                        tempval = tempval + AndStringToken;
                                }
                                else if (this.DefaultFilterType == FilterType.Equals)
                                {
                                    if (!tempval.Contains(this.EqualsString))
                                        tempval = EqualToken + tempval + AndStringToken;
                                    else
                                        tempval = tempval + AndStringToken;
                                }
                                filterval.Append(tempval);
                                tempfiltervalue = tempfiltervalue.Substring(AndPredicateIndex + 5);
                                AndPredicateIndex = tempfiltervalue.IndexOf(AndStringToken);
                                if (AndPredicateIndex == -1)
                                {
                                    if (this.DefaultFilterType == FilterType.Contains)
                                    {
                                        if (!tempfiltervalue.Contains(this.ContainsString))
                                            tempfiltervalue = ContainsToken + tempfiltervalue;
                                    }
                                    else if (this.DefaultFilterType == FilterType.StartsWith)
                                    {
                                        if (!tempfiltervalue.Contains(this.StartWithEndWithString))
                                            tempfiltervalue = tempfiltervalue + StartsWithToken;
                                    }
                                    else if (this.DefaultFilterType == FilterType.Equals)
                                    {
                                        if (!tempfiltervalue.Contains(this.EqualsString))
                                            tempfiltervalue = EqualToken + tempfiltervalue;
                                    }
                                    filterval.Append(tempfiltervalue);
                                    FilterValue = filterval.ToString();
                                    break;
                                }
                            }
                        }
                    }
                }
                //if (gdcModel.TableProperties.FilterBehavior == FilterBehavior.StringTyped)
                //{
                //    gdcModel.FilterColumn(visCol, FilterValue, this.DefaultFilterType, gdcModel.TableProperties.FilterBarPredicateType, false, true);
                //}
                string pattern = NumericWithSeparatorRegexString;

                if (this.FilterBarType == GridDataFilterBarType.AlphaNumeric)
                    pattern = AlphaNumericWithSeparatorRegexString;
                else
                    pattern = NumericFollow;

                if (this.FilterBarType == GridDataFilterBarType.AlphaNumeric && gdcModel.TableProperties.AlphaNumericFilterType == AlphaNumericFilterType.WithoutWildcard)
                {
                    gdcModel.FilterColumn(visCol, FilterValue, this.DefaultFilterType, gdcModel.TableProperties.FilterBarPredicateType, false, true);
                    SetCurrentStatusMessage();
                    return;
                }
                //check the keyValue is right or not for alphanumeric regular expression.
                bool keyValue = true;
                if (Regex.IsMatch(FilterValue, pattern, RegexOptions.IgnoreCase))
                {
                    var coll = Regex.Match(FilterValue, pattern, RegexOptions.IgnoreCase);
                    if (coll.Groups.Count >= 4)
                    {
                        if (this.FilterBarType == GridDataFilterBarType.AlphaNumeric)
                        {
                            keyValue = coll.Groups["Key"].Value.ToLower() == "or" || coll.Groups["Key"].Value.ToLower() == "and";
                        }
                    }
                }
                if (Regex.IsMatch(FilterValue, pattern, RegexOptions.IgnoreCase) && keyValue)
                {
                    var coll = Regex.Match(FilterValue, pattern, RegexOptions.IgnoreCase);

                    if (coll.Groups.Count >= 4)
                    {
                        var tkn1 = tokenizer.GetToken(coll.Groups["Value1"].Value);
                        if (tkn1.FilterType == FilterType.Undefined)
                        {
                            //if (this.FilterBarType == GridDataFilterBarType.Boolean)
                            //    tkn1.FilterType = FilterType.Equals;
                            //else
                                tkn1.FilterType = this.DefaultFilterType;
                        }
                        if (tkn1 == null || tkn1.FilterValue == null || tkn1.FilterType == Linq.FilterType.Undefined)
                        {
                            if (FilterValue.Equals(tkn1.FilterValue) && tkn1.FilterType == Linq.FilterType.Undefined)
                            {
                                if (visCol.FilterBehavior == FilterBehavior.StronglyTyped)
                                {
                                    SetCurrentStatusMessage();
                                    SetInVaildDataError();
                                    return;
                                }
                                else
                                {
                                    gdcModel.FilterColumn(visCol, FilterValue, Linq.FilterType.StartsWith, gdcModel.TableProperties.FilterBarPredicateType, false, true);
                                }

                                SetCurrentStatusMessage();
                                return;
                            }
                        }
                        var tempFilterValue = RemoveEscapeSeq(tkn1.FilterValue, true);
                        if (tempFilterValue == "") return;
                        if (this.FilterBarType == GridDataFilterBarType.Boolean)
                        {
                            string Fls = "false";
                            string tru = "true";
#if !SILVERLIGHT

                            if (!(this.GridControl.Model as GridDataTableModel).IsLegacyDataTable)
                            {
 #endif
                                if (tru.Contains(tempFilterValue))
                                {
                                    tempFilterValue = "1";
                                }
                                else if (Fls.Contains(tempFilterValue))
                                {
                                    tempFilterValue = "0";
                                }
                            #if !SILVERLIGHT
                            }
                            else if ((this.GridControl.Model as GridDataTableModel).IsLegacyDataTable)
                            {
                                if (tempFilterValue == "1")
                                    tempFilterValue = "true";
                                else if (tempFilterValue=="0")
                                    tempFilterValue="false";

                            }
#endif

                                else if (tempFilterValue != "1" && tempFilterValue != "0")
                                return;

                            tkn1.FilterType = FilterType.Equals;
                          
                        }
                        if (!CheckFilterBarTypeValid(tempFilterValue, this.FilterBarType) && gdcModel.TableProperties.FilterBehavior == FilterBehavior.StronglyTyped)
                        {
                            SetCurrentStatusMessage();
                            SetInVaildDataError();
                            return;
                        }
                        #region Date Time Validation
                        if (visCol.ColumnType.FullName == "System.DateTime")
                        {
                            DateTime validDateTime;
                            if (!DateTime.TryParse(tempFilterValue, CultureInfo.CurrentCulture.DateTimeFormat, DateTimeStyles.NoCurrentDateDefault, out validDateTime))
                            {
                                return;
                            }
                        }
                        #endregion
                        List<FilterPredicate> filterPredicates = new List<FilterPredicate>();
                        PredicateType prdtype;
                        if (this.FilterBarType == GridDataFilterBarType.AlphaNumeric)
                        {
                            prdtype = (coll.Groups["Key"].Value.ToLower() == "or") ? PredicateType.Or : PredicateType.And; //- Previous code
                        }
                        else
                            prdtype = gdcModel.TableProperties.FilterBarPredicateType;
                        var filterBehavior = gdcModel.TableProperties.FilterBehavior;
                        filterPredicates.Add(new FilterPredicate() { FilterType = tkn1.FilterType, FilterValue = tempFilterValue, FilterBehavior = filterBehavior, IsCaseSensitive = false, PredicateType = gdcModel.TableProperties.FilterBarPredicateType });

                        if (this.FilterBarType == GridDataFilterBarType.AlphaNumeric)
                        {

                            var tkn2 = tokenizer.GetToken(coll.Groups["Value2"].Value);
                            if (tkn2 == null || tkn2.FilterValue == null || tkn2.FilterType == Linq.FilterType.Undefined)
                            {
                                SetCurrentStatusMessage();
                                SetInVaildDataError();
                                return;
                            }
                            tempFilterValue = RemoveEscapeSeq(tkn2.FilterValue, true);
                            if (tempFilterValue == "") return;
                            if (!CheckFilterBarTypeValid(tempFilterValue, this.FilterBarType))
                            {
                                SetCurrentStatusMessage();
                                SetInVaildDataError();
                                return;
                            }
                            #region Date Time Validation
                            if (visCol.ColumnType.FullName == "System.DateTime")
                            {
                                DateTime validDateTime;
                                if (!DateTime.TryParse(tempFilterValue, CultureInfo.CurrentCulture.DateTimeFormat, DateTimeStyles.NoCurrentDateDefault, out validDateTime))
                                {
                                    return;
                                }
                            }
                            #endregion
                            filterPredicates.Add(new FilterPredicate() { FilterType = tkn2.FilterType, FilterValue = tempFilterValue, IsCaseSensitive = visCol.IsCaseSensitiveFilter, PredicateType = prdtype });
                        }

                        bool shouldReturn = false;

                        var expression = coll.Groups["path"].Value;
                        if (expression != null && expression.Length > 0)
                        {
                            this.UpdateFilterPredicates(ref filterPredicates, ref shouldReturn, ref tokenizer, expression, visCol);
                            if (shouldReturn)
                            {
                                return;
                            }
                        }
                        gdcModel.FilterColumn(visCol, filterPredicates, true);
                    }
                }
                else
                {
                    var filterToken = tokenizer.GetToken(FilterValue);
                    if (filterToken == null)
                    {
                        SetCurrentStatusMessage();
                        SetInVaildDataError();
                        return;
                    }

                    var filterSpaces = this.FilterBarType == GridDataFilterBarType.AlphaNumeric ? false : true;
                    FilterValue = RemoveEscapeSeq(filterToken.FilterValue, filterSpaces);
                    if (this.FilterBarType == GridDataFilterBarType.Numeric && FilterValue.Count() == 1)
                    {
                        if (FilterValue.Equals("-"))
                        {
                            FilterValue = filterToken.FilterValue.Replace("-", "");
                        }
                    }

                    if ((!CheckFilterBarTypeValid(FilterValue, this.FilterBarType) & FilterValue.Count() > 0))
                    {
                        SetCurrentStatusMessage();
                        SetInVaildDataError();
                        return;
                    }


                    if (filterType == FilterType.StartsWith || filterType == FilterType.EndsWith)
                    {
                        if (visCol.Filters.Count > 0)
                        {
                            gdcModel.TableProperties.VisibleColumns.SuspendEvents();
                            visCol.Filters.Clear();
                            gdcModel.TableProperties.VisibleColumns.ResumeEvents();
                        }
                    }

                    filterType = filterToken.FilterType;
                    if (this.FilterBarType == GridDataFilterBarType.AlphaNumeric && filterType == FilterType.Undefined)
                    {
                        filterType = this.GetFilterType();
                    }

                    if (filterType != FilterType.Undefined)
                    {
                        if (FilterValue == string.Empty)
                            gdcModel.FilterColumn(visCol, null, filterType, gdcModel.TableProperties.FilterBarPredicateType, visCol.IsCaseSensitiveFilter, true);
                        else
                        {
                            #region Date Time Validation
                            if (visCol.ColumnType.FullName == "System.DateTime")
                            {
                                DateTime validDateTime;
                                if (!DateTime.TryParse(FilterValue, out validDateTime))
                                {
                                    return;
                                }
                            }
                            #endregion

                            gdcModel.FilterColumn(visCol, FilterValue, filterType, gdcModel.TableProperties.FilterBarPredicateType, visCol.IsCaseSensitiveFilter, true);
                        }
                    }
                    else if (visCol.Filters.Count > 0)
                    {
                        visCol.Filters[0].FilterType = FilterType.Undefined;
                    }
                    else
                        gdcModel.FilterColumn(visCol, null, filterType, gdcModel.TableProperties.FilterBarPredicateType, false, true);
                }

                #region Error Tooltip if records count is zero
                currentRowCount = gdcModel.View.Records.Count;
                if (currentRowCount == 0)
                {
                    SetCurrentStatusMessage();
                    SetNoRecordsDataError();
                    return;
                }
                #endregion

                SetCurrentStatusMessage();
                if (filterType != FilterType.Undefined)
                {
                    if (FilterValueChanged != null)
                        FilterValueChanged(this, new GridDataFilterBarValueArgs(this.FilterValue, filterType));
                }
            }
        }

        private void UpdateFilterPredicates(ref List<FilterPredicate> filterPredicates, ref bool shouldReturn, ref GridDataFiltersTokenizer tokenizer, string expression, GridDataVisibleColumn visCol)
        {
            //string NumericWithSeparatorRegexString = @"(?<Value1>([\w\s<>=-\\\/\.]+))\s(?<Key>\w{2,3})\s(?<Value2>([\w<>=-\\\/\.]+))(?<path>[^\r\n]*)";

            var newPattern = @"\s(?<Key>\w{2,3})\s(?<Value1>([\w<>!=-\\\/\.]+))(?<path>[^\r\n]*)";
            // New pattern is added for Numeric Column to support Multiple Filters. 
            if (this.FilterBarType == GridDataFilterBarType.AlphaNumeric)
                newPattern = @"\s(?<Key>\w{2,3})\s(?<Value1>([\w<>#%=.]+[\w<>#%=.]+))(?<path>[^\r\n]*)";
            //else
              //  newPattern = NumericFollow;
            var datagrid = (GridDataControl)this.GridControl.FindParentElementOfType<GridDataControl>();
            if (Regex.IsMatch(expression, newPattern, RegexOptions.IgnoreCase))
            {
                var coll = Regex.Match(expression, newPattern, RegexOptions.IgnoreCase);

                var tkn1 = tokenizer.GetToken(coll.Groups["Value1"].Value);
                if (datagrid != null && tkn1.FilterType == FilterType.Undefined)
                {
                    if (datagrid.DefaultFilterOperator == FilterOperatorType.Contains)
                    {
                        tkn1.FilterType = FilterType.Contains;
                    }
                    else if (datagrid.DefaultFilterOperator == FilterOperatorType.Equals)
                    {
                        tkn1.FilterType = FilterType.Equals;
                    }
                    else if (datagrid.DefaultFilterOperator == FilterOperatorType.StartsWith)
                    {
                        tkn1.FilterType = FilterType.StartsWith;
                    }
                }
                if (tkn1 == null || tkn1.FilterValue == null || tkn1.FilterType == Linq.FilterType.Undefined)
                {
                    SetCurrentStatusMessage();
                    SetInVaildDataError();
                    shouldReturn = true;
                    return;
                }


                var tempFilterValue = RemoveEscapeSeq(tkn1.FilterValue, true);
                if (tempFilterValue == "") return;
                if (!CheckFilterBarTypeValid(tempFilterValue, this.FilterBarType))
                {
                    SetCurrentStatusMessage();
                    SetInVaildDataError();
                    shouldReturn = true;
                    return;
                }
                #region Date Time Validation
                if (visCol.ColumnType.FullName == "System.DateTime")
                {
                    DateTime validDateTime;
                    if (!DateTime.TryParse(tempFilterValue, CultureInfo.CurrentCulture.DateTimeFormat, DateTimeStyles.NoCurrentDateDefault, out validDateTime))
                    {
                        shouldReturn = true;
                        return;
                    }
                }
                #endregion

                var prdtype = (coll.Groups["Key"].Value.ToLower() == "or") ? PredicateType.Or : PredicateType.And;
                filterPredicates.Add(new FilterPredicate() { FilterType = tkn1.FilterType, FilterValue = tempFilterValue, IsCaseSensitive = false, PredicateType = prdtype });

                var expression2 = coll.Groups["path"].Value;
                if (expression2 != null && expression2.Length > 0)
                {
                    this.UpdateFilterPredicates(ref filterPredicates, ref shouldReturn, ref tokenizer, expression2, visCol);

                    if (shouldReturn)
                    {
                        return;
                    }
                }
            }
        }

        #endregion

        #region Events

        public event EventHandler<GridDataFilterBarValueArgs> FilterValueChanged = delegate { };
#if !SILVERLIGHT
        /// <summary>
        /// Handles the PreviewKeyDown event of the textBox control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.Windows.Input.KeyEventArgs"/> instance containing the event data.</param>
        private void textBox_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            try
            {
                var gdcModel = (GridDataTableModel)this.GridControl.Model;
                var column = this.GetCurrentColumn();
                var textBox = sender as TextBox;
                if (column.ColumnType == typeof(Boolean))
                {
                    if (e.Key == Key.Back || e.Key == Key.Delete)
                    {
                        textBox.Text = "";
                    }
                }
            }
            catch
            {

            }
        }
#endif
        private void textBox_KeyDown(object sender, KeyEventArgs e)
        {
            var gdcModel = (GridDataTableModel)this.GridControl.Model;
            var column = this.GetCurrentColumn();
            var textBox = sender as TextBox;
            if (column.ColumnType == typeof(Boolean))
            {
                textBox.Text = "";
                switch (e.Key)
                {
                    case Key.F:
                        textBox.Text = "false";
                        break;
                    case Key.T:
                        textBox.Text = "true";
                        break;
                    case Key.D0:
                        textBox.Text = "0";
                        break;
                    case Key.D1:
                        textBox.Text = "1";
                        break;
                }                
                e.Handled = true;
#if !SILVERLIGHT

                textBox.CaretIndex = textBox.Text.Length;
#else
                textBox.Select(textBox.Text.Length, 0);
#endif
            }

            bool isShiftPressed = Keyboard.Modifiers == ModifierKeys.Shift ? true : false;
            var keyStr = string.Empty;
#if SILVERLIGHT

            if (e.Key.ToString().ToLower().Equals("unknown"))
            {
                keyStr = GetStringForUnknownKey(e.PlatformKeyCode, isShiftPressed);
            }
            else
#endif
            {
                keyStr = GetStringForKey(e.Key, isShiftPressed);
            }
            if (e.Key != Key.Tab && e.Key!= Key.Escape)
            {
                if (column != null && column.FilterBehavior == FilterBehavior.StringTyped)
                {
                    if (!Regex.IsMatch(keyStr, AlphaNumericRegexString))
                    {
                        if (keyStr != "/" && keyStr != "-")
                        {
                            //e.Handled = true;
                            return;
                        }
                    }
                }
                if (this.FilterBarType == GridDataFilterBarType.Numeric && !keyStr.Equals("RightShift") && !keyStr.Equals("Space"))
                {
                    if (!Regex.IsMatch(keyStr, NumericWithANDORString))
                    {
                        e.Handled = true;
                        return;
                    }
                }

                if (this.FilterBarType != GridDataFilterBarType.AlphaNumeric && Keyboard.Modifiers == ModifierKeys.Shift)
                {
                    if (keyStr == this.StartWithEndWithString || keyStr == this.ContainsString)
                    {
                        //e.Handled = true;
                        return;
                    }
                }
                else if (this.FilterBarType != GridDataFilterBarType.AlphaNumeric)
                {
                    if (textBox.Text.Contains(this.StartWithEndWithString) || textBox.Text.Contains(this.ContainsString))
                    {
                        //e.Handled = true;
                        return;
                    }
                }
                else if (this.FilterBarType == GridDataFilterBarType.AlphaNumeric && keyStr.Contains(this.lessThanString) || keyStr.Contains(this.GreaterThanString) || keyStr.Contains(this.EqualsString))
                {
                    if (keyStr != this.EqualsString)
                    {
                        e.Handled = true;
                        return;
                    }
                }
                else if (this.FilterBarType == GridDataFilterBarType.Boolean && keyStr.Contains(this.EqualsString) && keyStr.Contains(this.NotString))
                {
                    e.Handled = true;
                    return;
                }
                else if (this.FilterBarType == GridDataFilterBarType.AlphaNumeric && gdcModel.TableProperties.AlphaNumericFilterType == AlphaNumericFilterType.WithoutWildcard)
                {
                    if (keyStr.Contains(this.StartWithEndWithString) || keyStr.Contains(this.ContainsString))
                    {
                        e.Handled = true;
                        return;
                    }
                }

            }

            switch (e.Key)
            {
                case Key.Enter:
                    ValidataFilter();
                    break;
                case Key.Right:
                case Key.Left:
                case Key.Down:
                case Key.Up:
                    if (ccSelectionStart == textBox.SelectionStart
                        && ccSelectionLength == textBox.SelectionLength)
                        this.GridControl.MoveCurrentCellWithArrowKey(e);
                    break;
                case Key.Tab:
                    if (this.HasCurrentCellState)
                    {
                        this.GridControl.InvalidateCell(this.CellRowColumnIndex);
                    }

                    this.GridControl.MoveCurrentCellWithArrowKey(e);
                    break;
            }
        }

        private void textBox_SelectionChanged(object sender, RoutedEventArgs e)
        {
            TextBox textBox = (TextBox)sender;
            if (!IsInArrange)
            {
                textBoxSelectionStart = textBox.SelectionStart;
                textBoxSelectionLength = textBox.SelectionLength;

                if (HasCurrentCellState)
                    GridControl.InvalidateCell(this.CellRowColumnIndex);
            }
            
        }

        private void textBox_TextChanged(object sender, TextChangedEventArgs e)
        {
#if !SILVERLIGHT
            if (ignoreTextChanged)
                return;
#endif
            if (!HasCurrentCellState || this.IsFilteringSuspend)
            {
                return;
            }

            var textBox = (TextBox)sender;
            var gdcModel = (GridDataTableModel)this.GridControl.Model;
            if (gdcModel.TableProperties.FilterBarMode == GridDataFilterBarMode.Immediate)
            {

                bool skipFilter = false;

                // Filtering operation is suspended till the operand or filter value is 
                // entered to optimize the filtering operation.
                // Note: The "%" character is not added in the below list since it is used for both 
                // "Starts with" and "Ends with"

                //string[] skipInput = { ">", "<", "=", "!", " " };
                //string[] skipInput = { this.greaterThanString, this.lessThanString, this.EqualsString, this.NotString, " " };
                List<string> skipInput = new List<string>();
                skipInput.Add(this.GreaterThanString);
                skipInput.Add(this.LessThanString);
                skipInput.Add(this.NotString);
                skipInput.Add(this.EqualsString);
                skipInput.Add(" ");
                if (isTokensSetOutside)
                {
                    skipInput.Add(this.GreaterThanOrEqualString);
                    skipInput.Add(this.LessThanOrEqualString);
                }

#if !SILVERLIGHT

                foreach (TextChange change in e.Changes)
                {
                    if (change.AddedLength > 0)
                    {
                        string newChar = textBox.Text.Substring(textBox.Text.Length - change.AddedLength);
                        if (change.AddedLength == 1 && skipInput.Contains(newChar))
                        {
                            skipFilter = true;
                        }
                    }
                }
#else
                if (textBox.Text.Length > 1)
                {
                    var charArray = textBox.Text.ToCharArray();
                    var changedText = charArray[charArray.Length - 1].ToString();

                    if (skipInput.Contains(changedText))
                    {
                        skipFilter = true;
                    }
                }
#endif
                ErrorToolTip = EmptyString;
                FilterValue = textBox.Text;               
                if (gdcModel != null && gdcModel.TableProperties != null && gdcModel.TableProperties.VisibleColumns.Count > 0)
                {
                    var visCol = GetCurrentColumn();
                    if (visCol == null) return;
                    if ((gdcModel.TableProperties.FilterBarMode == GridDataFilterBarMode.Immediate && visCol.FilterBarMode == GridDataFilterBarMode.OnEnter) ||
                        (gdcModel.TableProperties.FilterBarMode == GridDataFilterBarMode.OnEnter && visCol.FilterBarMode == GridDataFilterBarMode.OnEnter)) return;
                }
                if (!skipFilter)
                    ValidataFilter();

            }
        }


        #endregion

        //private bool CheckFilterBarInputTextValid(string textBoxValue, string newString)
        //{
        //    var filterBartype = GetCurrentFilterBarType();
        //    switch (filterBartype)
        //    {
        //        case GridDataFilterBarType.AlphaNumeric:
        //            if (Regex.IsMatch(textBoxValue, AlphaNumericRegexString))
        //            {
        //                var strarrys = textBoxValue.ToArray();
        //                var cnt = 0;
        //                foreach (char item in strarrys)
        //                {
        //                    if (item == '%' || item == '#')
        //                    {
        //                        cnt++;
        //                    }
        //                }
        //                if (cnt > 1) return false;
        //                return true;
        //            }
        //            break;
        //        case GridDataFilterBarType.Numeric:
        //            if (Regex.IsMatch(textBoxValue, NumericRegexString))
        //            {
        //                return ValidateNumericDateTime(textBoxValue, newString);
        //            }
        //            break;
        //        case GridDataFilterBarType.Boolean:
        //            var strArrays = textBoxValue.ToArray();
        //            foreach (var item in strArrays)
        //            {
        //                if (item.ToString() != "0" && item.ToString() != "1") return false;
        //            }
        //            return true;
        //        case GridDataFilterBarType.DateTime:
        //            return ValidateNumericDateTime(textBoxValue, newString);
        //        case GridDataFilterBarType.NotUsed:
        //            break;
        //        default:
        //            break;
        //    }
        //    return false;
        //}
        
        #region Virtual methods

        /// <summary>
        /// Clears the Filter from the Current VisibleColumn..
        /// </summary>
        protected virtual void ClearCurrentColumnFilter()
        {
            var model = this.GridControl.Model as GridDataTableModel;
            if (model != null && model.TableProperties.VisibleColumns != null && model.TableProperties.VisibleColumns.Count > 0)
            {
                var col = this.GetCurrentColumn();
                var columnIndex = model.TableProperties.VisibleColumns.IndexOf(col);
                if (columnIndex >= 0 && model.TableProperties.VisibleColumns.Count > columnIndex)
                {
                    model.ClearFilterColumn(col, null, this.GetFilterType(), model.TableProperties.FilterBarPredicateType, false, true);
                    SetCurrentStatusMessage();
                }
            }
        }


        /// <summary>
        /// This method will apply Filter for the current Visible Column.
        /// </summary>
        /// <param name="visCol">Current VisibleColumn </param>
        /// <param name="filtervalue">FilterValue</param>
        /// <param name="defaultFilterType">FilterType</param>
        /// <param name="predicateType">PredicateType</param>
        /// <param name="NeedValidation">True will validate the Filtering. False will apply filter directly.</param>
        public void GenerateFilter(object filtervalue, FilterType defaultFilterType, bool NeedValidation)
        {
            var gdcModel = this.GridControl.Model as GridDataTableModel;
            if (gdcModel != null)
            {
                if (!NeedValidation)
                {
                    gdcModel.FilterColumn(GetCurrentColumn(), filtervalue, this.DefaultFilterType,  gdcModel.TableProperties.FilterBarPredicateType, false, true);
                }
                else
                {
                    this.FilterValue = filtervalue.ToString();
                    this.DefaultFilterType = defaultFilterType;
                    this.ValidataFilter();
                }
            }
        }

        private void SetCurrentStatusMessage()
        {
            var gdcModel = (GridDataTableModel)this.GridControl.Model;
            if (gdcModel != null && gdcModel.Grid != null)
            {
                string filters = "";
                var multipleFilters = 0;
                bool skipPredicateType = false;
                foreach (var item in gdcModel.TableProperties.VisibleColumns)
                {
                    if (item != null && item.Filters.Count > 0)
                    {
                        int index = gdcModel.TableProperties.VisibleColumns.IndexOf(item);
                        if (index > 0 && filters != "")
                        {
                            filters = filters + " " + gdcModel.TableProperties.FilterBarPredicateType.ToString() + " ";
                            skipPredicateType = true;
                        }
                        filters = filters + LeftBraceString;
                        foreach (FilterPredicate filitem in item.Filters.OfType<FilterPredicate>())
                        {
                            if (filitem != null)
                            {
                                if (multipleFilters <= 0 || skipPredicateType)
                                {
                                    filters += GetFilterTypeStrindEqu(filitem, item) + " ";
                                    skipPredicateType = false;
                                }
                                else
                                    filters += " " + filitem.PredicateType.ToString() + " " + GetFilterTypeStrindEqu(filitem, item);
                                multipleFilters++;
                            }
                        }
                        filters = filters + RightBraceString;
                    }
                }

                if (filters.Trim() == EmptyString)
                    SetStatusMessage(EmptyString);
                else
                    SetStatusMessage(filters);

            }
        }

        public void SetStatusMessage(string statusMessage)
        {
            var gdcModel = (GridDataTableModel)this.GridControl.Model;
            if (gdcModel == null || gdcModel.Grid == null) return;
            var gdc = gdcModel.Grid.FindParentElementOfType<GridDataControl>();
            if (gdc != null)
            {
                gdc.StatusBarMessage = statusMessage;
            }
        }

        public string GetStatusMessage()
        {
            var gdcModel = (GridDataTableModel)this.GridControl.Model;
            if (gdcModel == null || gdcModel.Grid == null) return "";
            var gdc = gdcModel.Grid.FindParentElementOfType<GridDataControl>();
            if (gdc != null)
            {
                return gdc.StatusBarMessage;
            }
            return EmptyString;
        }

        protected virtual void SetNoRecordsDataError()
        {
            ErrorToolTip = GridDataResourceWrapper.NoRecordsfound;
        }

        protected virtual void SetInValidDateTimeError()
        {
            ErrorToolTip = GridDataResourceWrapper.InvalidDataTime;
        }

        protected virtual void SetInVaildDataError()
        {
            ErrorToolTip = GridDataResourceWrapper.InvalidDataToFilter;
        }

        #endregion

        #region internal Classes
                
        internal class GridDataFilterToken
        {
            public FilterType FilterType { get; set; }
            public string FilterValue { get; set; }

            public override string ToString()
            {
                return string.Format("FilterType-> {0} \nValue-> {1}", this.FilterType, this.FilterValue);
            }
        }

        internal class GridDataFiltersTokenizer
        {
            private string[] tokens = new string[] { "%", "<", "<=", ">", ">=", "!", "=", "#" };

            public string[] Tokens
            {
                get { return tokens; }
                set { tokens = value; }
            }
            string[] escTokens = new string[] { "\t", "\r\n" };
            internal GridDataFilterBarType CurrentFilterBarType = GridDataFilterBarType.AlphaNumeric;

            public bool isTokenSetOutside = false;
            internal string ContainsString = "#";
            internal string StartsAndEndsWithString = "%";
            internal string GreaterThanString = ">";
            internal string GreaterThanOrEqualString = ">=";
            internal string LessThanString = "<";
            internal string LessThanOrEqualString = "<=";
            internal string EqualString = "=";
            internal string NotString = "!";

            public GridDataFilterToken GetToken(string filterStr)
            {
                if (filterStr == null || filterStr == string.Empty) return new GridDataFilterToken() { FilterType = FilterType.Undefined, FilterValue = "" };
                var value = string.Empty;
                var filterType = FilterType.Undefined;

                var firstBlock = filterStr.ToArray();
                this.GetToken(firstBlock, firstBlock[0], ref value, ref filterType);
                return new GridDataFilterToken() { FilterType = filterType, FilterValue = value };
            }

            internal bool IsToken(char c)
            {
                var token = Tokens.FirstOrDefault(f => f == c.ToString());
                return token != null;
            }

            private bool IsEscSequence(char c)
            {
                var escSeq = escTokens.FirstOrDefault(e => e == c.ToString());
                return escSeq != null;
            }

            private void GetToken(char[] remainingChars, char filterChar, ref string value, ref FilterType filterTokenType)
            {
                if (IsToken(filterChar))
                {
                    // we found a token
                    var tokenType = this.GetTokenType(value != string.Empty, filterChar.ToString());
                    filterTokenType = tokenType;
                    if (!this.isTokenSetOutside && (tokenType == FilterType.LessThan || tokenType == FilterType.GreaterThan))
                    {
                        if (remainingChars.Length > 1)
                        {
                            // possibility of a LessThanOrEqual / GreaterThanOrEqual
                            var nextBlock = remainingChars.Skip(1).ToArray();
                            var nextChar = nextBlock[0];
                            if (IsToken(nextChar))
                            {
                                var nextCharTokenType = this.GetTokenType(false, nextChar.ToString());
                                if (nextCharTokenType == FilterType.Equals)
                                {
                                    if (tokenType == FilterType.LessThan)
                                    {
                                        tokenType = FilterType.LessThanOrEqual;
                                        filterTokenType = tokenType;
                                    }
                                    else if (tokenType == FilterType.GreaterThan)
                                    {
                                        tokenType = FilterType.GreaterThanOrEqual;
                                        filterTokenType = tokenType;
                                    }
                                }

                                SkipAndTokenizeBlock(nextBlock, ref value, ref filterTokenType);
                            }
                            else
                            {
                                this.GetFilterValue(nextBlock, nextChar, ref value, ref filterTokenType);
                            }
                        }
                    }
                    else
                    {
                        if (remainingChars.Length > 1)
                        {
                            var nextBlock = remainingChars.Skip(1).ToArray();
                            var nextChar = nextBlock[0];
                            this.GetFilterValue(nextBlock, nextChar, ref value, ref filterTokenType);
                        }
                    }
                }
                else
                {
                    this.GetFilterValue(remainingChars, filterChar, ref value, ref filterTokenType);
                }
            }

            private void GetFilterValue(char[] remainingChars, char filterChar, ref string value, ref FilterType filterTokenType)
            {
                if (!this.IsEscSequence(filterChar))
                {
                    value += filterChar;
                    SkipAndTokenizeBlock(remainingChars, ref value, ref filterTokenType);
                }
                else
                {
                    SkipAndTokenizeBlock(remainingChars, ref value, ref filterTokenType);
                }
            }

            private void SkipAndTokenizeBlock(char[] remainingChars, ref string value, ref FilterType filterTokenType)
            {
                if (remainingChars.Length > 1)
                {
                    var nextBlock = remainingChars.Skip(1).ToArray();
                    var nextChar = nextBlock[0];
                    this.GetToken(nextBlock, nextChar, ref value, ref filterTokenType);
                }
            }

            private FilterType GetTokenType(bool isEmptyText, string token)
            {
                var filterType = FilterType.Undefined;

                if (token == this.StartsAndEndsWithString)
                {
                    //if (CurrentFilterBarType != GridDataFilterBarType.AlphaNumeric) 
                    //    return FilterType.Undefined;
                    if (isEmptyText)
                    {
                        filterType = FilterType.StartsWith;
                    }
                    else
                    {
                        filterType = FilterType.EndsWith;
                    }
                }
                if (token == this.ContainsString)
                {
                    if (CurrentFilterBarType != GridDataFilterBarType.AlphaNumeric) return FilterType.Undefined;
                    filterType = FilterType.Contains;
                }
                if (token == this.LessThanString)
                {
                    if (CurrentFilterBarType == GridDataFilterBarType.AlphaNumeric || CurrentFilterBarType == GridDataFilterBarType.Boolean) return FilterType.Undefined;
                    filterType = FilterType.LessThan;
                }
                if (token == this.LessThanOrEqualString)
                {
                    if (CurrentFilterBarType == GridDataFilterBarType.AlphaNumeric || CurrentFilterBarType == GridDataFilterBarType.Boolean) return FilterType.Undefined;
                    filterType = FilterType.LessThanOrEqual;
                }
                if (token == this.GreaterThanString)
                {
                    if (CurrentFilterBarType == GridDataFilterBarType.AlphaNumeric || CurrentFilterBarType == GridDataFilterBarType.Boolean) return FilterType.Undefined;
                    filterType = FilterType.GreaterThan;
                }
                if (token == this.GreaterThanOrEqualString)
                {
                    if (CurrentFilterBarType == GridDataFilterBarType.AlphaNumeric || CurrentFilterBarType == GridDataFilterBarType.Boolean) return FilterType.Undefined;
                    filterType = FilterType.GreaterThanOrEqual;
                }
                if (token == this.NotString)
                {
                    filterType = FilterType.NotEquals;
                }
                if (token == this.EqualString)
                {
                    filterType = FilterType.Equals;
                }
                //switch (token)
                //{
                //    case "%":
                //        if (CurrentFilterBarType != GridDataFilterBarType.AlphaNumeric) return FilterType.Undefined;
                //        if (isEmptyText)
                //        {
                //            filterType = FilterType.StartsWith;
                //        }
                //        else
                //        {
                //            filterType = FilterType.EndsWith;
                //        }
                //        break;
                //    case "#":
                //        if (CurrentFilterBarType != GridDataFilterBarType.AlphaNumeric) return FilterType.Undefined;
                //        filterType = FilterType.Contains;
                //        break;
                //    case "<":
                //        if (CurrentFilterBarType == GridDataFilterBarType.AlphaNumeric || CurrentFilterBarType == GridDataFilterBarType.Boolean) return FilterType.Undefined;
                //        filterType = FilterType.LessThan;
                //        break;
                //    case "<=":
                //        if (CurrentFilterBarType == GridDataFilterBarType.AlphaNumeric || CurrentFilterBarType == GridDataFilterBarType.Boolean) return FilterType.Undefined;
                //        filterType = FilterType.LessThanOrEqual;
                //        break;
                //    case ">":
                //        if (CurrentFilterBarType == GridDataFilterBarType.AlphaNumeric || CurrentFilterBarType == GridDataFilterBarType.Boolean) return FilterType.Undefined;
                //        filterType = FilterType.GreaterThan;
                //        break;
                //    case ">=":
                //        if (CurrentFilterBarType == GridDataFilterBarType.AlphaNumeric || CurrentFilterBarType == GridDataFilterBarType.Boolean) return FilterType.Undefined;
                //        filterType = FilterType.GreaterThanOrEqual;
                //        break;
                //    case "!":
                //        // if (CurrentFilterBarType == GridDataFilterBarType.Boolean) return FilterType.Undefined; Previous Code
                //        filterType = FilterType.NotEquals;
                //        break;
                //    case "=":
                //        filterType = FilterType.Equals;

                //        break;
                //}

                return filterType;
            }

            internal static string GetFilterTypeString(string filtervalue, FilterType filterType)
            {
                switch (filterType)
                {
                    case FilterType.LessThan:
                        return "<" + filtervalue;
                    case FilterType.LessThanOrEqual:
                        return "<=" + filtervalue;
                    case FilterType.Equals:
                        return "=" + filtervalue;
                    case FilterType.NotEquals:
                        return "!" + filtervalue;
                    case FilterType.GreaterThanOrEqual:
                        return ">=" + filtervalue;
                    case FilterType.GreaterThan:
                        return ">" + filtervalue;
                    case FilterType.EndsWith:
                        return "%" + filtervalue;
                    case FilterType.StartsWith:
                        {
                            string[] input = filtervalue.Split(' ');
                            if (input.Length > 1)
                            {
                                var value = filtervalue;

                                var first = true;
                                foreach (string str in input)
                                {
                                    if (first)
                                    {
                                        value = str + "%";
                                        first = false;
                                    }
                                    else
                                        value += " " + str;
                                }

                                return value;
                            }
                            return filtervalue + "%";
                        }
                    case FilterType.Contains:
                        return "#" + filtervalue;
                    case FilterType.Undefined:
                    default:
                        return filtervalue;
                }
            }
        }

        #endregion        
    }

    public class GridDataFilterBarValueArgs : EventArgs
    {
        public FilterType FilterType { get; set; }
        public string FilterValue { get; set; }

        public GridDataFilterBarValueArgs()
        {
        }

        public GridDataFilterBarValueArgs(string filterValue, FilterType filterType)
        {
            this.FilterValue = filterValue;
            this.FilterType = filterType;
        }
    }

    public enum GridDataFilterBarMode
    {
        Immediate,
        OnEnter
    }

    public enum GridDataFilterBarType
    {
        AlphaNumeric,
        Numeric,
        Boolean,
        DateTime,
        NotUsed
    }

    public enum AlphaNumericFilterType
    {
        WithWildcard,
        WithoutWildcard
    }

    public class GridDataFilterBarStyle : DependencyObject
    {
        /// <summary>
        /// This is an enum Property used to select the celltype of  FilterBar
        /// </summary>
        public CellType CellType
        {
            get { return (CellType)GetValue(CellTypeProperty); }
            set { SetValue(CellTypeProperty, value); }
        }


        public static readonly DependencyProperty CellTypeProperty =
            DependencyProperty.Register("CellType", typeof(CellType), typeof(GridDataFilterBarStyle),
#if !SILVERLIGHT

 new UIPropertyMetadata(CellType.TextBox));
#else
            new PropertyMetadata(CellType.TextBox));
#endif

        public String ValueMember
        {
            get { return (String)GetValue(ValueMemberProperty); }
            set { SetValue(ValueMemberProperty, value); }
        }


        public static readonly DependencyProperty ValueMemberProperty =
            DependencyProperty.Register("ValueMember", typeof(String), typeof(GridDataFilterBarStyle),
#if !SILVERLIGHT

 new UIPropertyMetadata(null));
#else
 new PropertyMetadata(null));
#endif

        public String DisplayMember
        {
            get { return (String)GetValue(DisplayMemberProperty); }
            set { SetValue(DisplayMemberProperty, value); }
        }


        public static readonly DependencyProperty DisplayMemberProperty =
            DependencyProperty.Register("DisplayMember", typeof(String), typeof(GridDataFilterBarStyle),
#if !SILVERLIGHT

 new UIPropertyMetadata(null));
#else
             new PropertyMetadata(null));
#endif


        public static readonly DependencyProperty ItemsSourceProperty = DependencyProperty.Register(
           "ItemsSource",
           typeof(object),
           typeof(GridDataFilterBarStyle),
           new PropertyMetadata(null));
        /// <summary>
        /// This Property used to set the Dropdown filterbar cell items source.
        /// </summary>
        [XmlIgnore]
        public IEnumerable ItemsSource
        {
            get
            {
                return this.GetValue(GridDataFilterBarStyle.ItemsSourceProperty) as IEnumerable;
            }

            set
            {
                this.SetValue(GridDataFilterBarStyle.ItemsSourceProperty, value);
            }
        }


        //For Editable Dropdown
        public bool IsEditable
        {
            get { return (bool)GetValue(IsEditableProperty); }
            set { SetValue(IsEditableProperty, value); }
        }


        public static readonly DependencyProperty IsEditableProperty =
            DependencyProperty.Register("IsEditable", typeof(bool), typeof(GridDataFilterBarStyle),
#if !SILVERLIGHT

 new UIPropertyMetadata(null));
#else
 new PropertyMetadata(null));
#endif
        //Till This

    }

    public enum CellType
    {
        ComboBox,
        TextBox
    }
}
