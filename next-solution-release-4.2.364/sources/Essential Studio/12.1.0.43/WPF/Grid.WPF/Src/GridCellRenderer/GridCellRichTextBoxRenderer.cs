#region Copyright Syncfusion Inc. 2001 - 2014
// Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Any infringement will be prosecuted under
// applicable laws. 
#endregion
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Controls;
using System.Windows;
using System.Windows.Documents;
using System.Windows.Media;
using Syncfusion.Windows.Controls.Cells;
using System.Windows.Input;
using Syncfusion.Windows.GridCommon;

namespace Syncfusion.Windows.Controls.Grid.GridCellRenderer
{

    public class GridCellRichTextBoxCellModel : GridCellModel<GridCellRichTextBoxCellRenderer>
    {
        public GridCellRichTextBoxCellModel()
        {

        }
        public override Size CalculatePreferredCellSize(int rowIndex, int colIndex, GridStyleInfo style, GridQueryBounds queryBounds)
        {
            Size clientSize = OnQueryPrefferedClientSize(rowIndex, colIndex, style, queryBounds);
            if (clientSize.IsEmpty)
                return Size.Empty;

            Thickness margins = style.TextMargins.ToThickness();
            if (style.HasImageIndex)
            {
                margins = style.AdjustImageWidthAndHeightToMargin(margins, clientSize);
            }
            else
            {
                margins = style.ErrorInfo.AdjustErrorInfoMargin(margins, clientSize);
            }           
            margins.Left = Math.Max(margins.Left, 2);
            margins.Right = Math.Max(margins.Right, 2);

            Size size = AddBorderMargins(clientSize, margins);
            size = AddBorderMargins(size, style.BorderMargins.ToThickness());
            return size;
        }

        public override string GetFormattedText(GridStyleInfo style, object value, int textInfo)
        {
            FlowDocument document = value as FlowDocument;

            StringBuilder FormatText = new StringBuilder();
            foreach (var block in document.Blocks)
                {
                Paragraph para = block as Paragraph;
                foreach (var run in para.Inlines)
                    {
                    if (run is Run)
                        {
                        Run rn = run as Run;
                        FormatText.Append(rn.Text);
                        }

                    }
                }
            return FormatText.ToString();
            /*
                        string text = ""; Unreachable code
                        if (document != null)
                        {
                            var CompleteTextRange = new TextRange(document.ContentStart, document.ContentEnd);
                           text = CompleteTextRange.Text;   // Text in the RTE. 
                        }
                        return text;
            */
            // return base.GetFormattedText(style, value, textInfo);
        }
    }

    public class GridCellRichTextBoxCellRenderer : GridVirtualizingCellRenderer<RichTextBox>
    {

        private RichTextPaint _rtp = new RichTextPaint();
        public GridCellRichTextBoxCellRenderer()
        {
            IsControlTextShown = false; // CellValue must be FlowDocument
            IsFocusable = true;
            SupportsRenderOptimization = true;
            AllowTransparentBackground = true;
            AllowKeepAliveOnlyCurrentCell = true;
            AllowRecycle = true;
        }

        protected override void OnActivated()
        {
            GridControl.InvalidateCell(CellRowColumnIndex);
        }
        protected override void OnDeactivated()
        {
            GridControl.InvalidateCell(CellRowColumnIndex);
        }
        protected override void OnRender(System.Windows.Media.DrawingContext dc, RenderCellArgs rca, GridRenderStyleInfo style)
        {
            if (rca.CellUIElements != null)
            {
                ((RichTextBox)rca.CellUIElements.UIElements[0]).FlowDirection = style.FlowDirection;
                return;
            }

            // Will only get hit if SupportsRenderOptimization is true, otherwise rca.CellUIElements is never null.
            Thickness margins = style.TextMargins.ToThickness();
            if (style.HasImageIndex)
            {
                margins = style.AdjustImageWidthAndHeightToMargin(margins, rca.CellRect.Size);
            }
            else
            {
                margins = style.ErrorInfo.AdjustErrorInfoMargin(margins, rca.CellRect.Size);
            }
            Rect textRectangle = rca.SubtractBorderMargins(rca.CellRect, margins);

            // TextBoxView always seems to have this margin and I am not able to reset the margin.
            // Therefore I am also hard-codeing it here so that TextBox behavior is properly
            // emulated.
            //margins.Left = Math.Max(margins.Left, 2);
            //margins.Right = Math.Max(margins.Right, 2);

            /// If we resize the column means Text will displace from its origional.
            /// Because While increasing the column size margin.left also increase.
            /// So I have set the Constant value for margin.Left
            if (style.HorizontalAlignment == HorizontalAlignment.Left && style.ErrorInfo.HasErrorMessage && style.ErrorInfo.ErrorContentAlignment == ImageContentAlignment.Left)
            {
                margins.Left = 20;
            }

            textRectangle = rca.SubtractBorderMargins(rca.CellRect, margins);

            if (textRectangle.IsEmpty)
                return;

            //if (style.FlowDirection == FlowDirection.RightToLeft)
            //{
            //    textBox.FlowDirection = style.FlowDirection;
            //    double m11 = -1;
            //    double m22 = 1;
            //    double offsetX = textBox.Width;
            //    double offsetY = 0;
            //    textBox.LayoutTransform = new MatrixTransform(m11, 0, 0, m22, offsetX, offsetY); //make sure this does not leak...
            //}
            //else
            //{
            //    textBox.LayoutTransform = MatrixTransform.Identity;
            //}
            this._rtp.DrawRichText(dc, textRectangle, style);
            base.OnRender(dc, rca, style);
        }

        /// <summary>
        /// Called to initialize the content of the cell
        /// using the information from the cell style (value, text,
        /// behavior etc.). You must override this method in your
        /// derived class.
        /// </summary>
        /// <param name="textBox">The text box.</param>
        /// <param name="style">The cell style info.</param>
        public override void OnInitializeContent(RichTextBox textBox, GridRenderStyleInfo style)
        {
            textBox.Padding = new Thickness(0);
            textBox.IsReadOnly = style.ReadOnly;
            textBox.Background = Brushes.White;
            textBox.Foreground = Brushes.Black;
            FlowDocument document = GetControlValue(style) as FlowDocument;
            if (document == null)
            {

                // when the CellValue is empty we just set a empty PAragraph with Empty Run. 
                if (style.CellValue != null)
                    textBox.Document = new FlowDocument(new Paragraph(new Run(style.CellValue.ToString())));
            }
            if (document != null)
            {
                if (document.Parent != null)
                {
                    // since the FlowDocument will have parent as RichTextBox when Previously InitilizeContent() Invoked. We Need to Clear its Parent when we set again. 
                    var parentTextBox = document.Parent as RichTextBox;
                    parentTextBox.Document = new FlowDocument();
                }
                textBox.Document = document;
            }
            // We Trun the RichTextBox as the Mirror View to support RightToLeft .
            if (style.FlowDirection == FlowDirection.RightToLeft)
            {
                textBox.FlowDirection = style.FlowDirection;
                double m11 = -1;
                double m22 = 1;
                double offsetX = textBox.Width;
                double offsetY = 0;
                textBox.LayoutTransform = new MatrixTransform(m11, 0, 0, m22, offsetX, offsetY); //make sure this does not leak...
            }

            VirtualizingCellsControl.SetWantsMouseInput(textBox, true);
        }

        protected override void ArrangeUIElement(ArrangeCellArgs aca, RichTextBox uiElement, GridRenderStyleInfo style)
        {
            Thickness margins = style.TextMargins.ToThickness();
            margins = style.ErrorInfo.AdjustErrorInfoMarginOnEditing(margins, style.GridControl, style.CellRowColumnIndex);
            uiElement.Padding = margins;

            base.ArrangeUIElement(aca, uiElement, style);
        }

        public override void CreateRendererElement(RichTextBox textBox, GridRenderStyleInfo style)
        {
            base.CreateRendererElement(textBox, style);
            textBox.Padding = new Thickness(0);
            textBox.IsReadOnly = style.ReadOnly;
            FlowDocument document = GetControlValue(style) as FlowDocument;
            if (document == null)
            {
                // when the CellValue is empty we just set a empty PAragraph with Empty Run. 
                if (style.CellValue != null)
                    textBox.Document = new FlowDocument(new Paragraph(new Run(style.CellValue.ToString())));
            }
            else
            {
                if (document.Parent != null)
                {
                    // since the FlowDocument will have parent as RichTextBox when Previously InitilizeContent() Invoked. We Need to Clear its Parent when we set again. 
                    var parentTextBox = document.Parent as RichTextBox;
                    parentTextBox.Document = new FlowDocument();
                }
                textBox.Document = document;
            }
            // We Trun the RichTextBox as the Mirror View to support RightToLeft .
            if (style.FlowDirection == FlowDirection.RightToLeft)
            {
                textBox.FlowDirection = style.FlowDirection;
                double m11 = -1;
                double m22 = 1;
                double offsetX = textBox.Width;
                double offsetY = 0;
                textBox.LayoutTransform = new MatrixTransform(m11, 0, 0, m22, offsetX, offsetY); //make sure this does not leak...
            }

            VirtualizingCellsControl.SetWantsMouseInput(textBox, true);
        }

        protected override void OnWireUIElement(RichTextBox uiElement)
        {
            uiElement.TextChanged += new TextChangedEventHandler(uiElement_TextChanged);
            uiElement.LostFocus += new RoutedEventHandler(uiElement_LostFocus);
            base.OnWireUIElement(uiElement);
        }

        void uiElement_LostFocus(object sender, RoutedEventArgs e)
            {
            var Style = this.GridControl.Model[this.CurrentCell.RowIndex, this.CurrentCell.ColumnIndex] as GridStyleInfo;          
            Style.CellValue = (sender as RichTextBox).Document as FlowDocument;
           
            }

        public override void MouseDown(FrameworkElement owner, Scroll.MouseControllerEventArgs e)
        {
            RichTextSelection selection = new RichTextSelection(this.GridControl);
            selection.MouseDown(e);
        }

        void uiElement_TextChanged(object sender, TextChangedEventArgs e)
        {
           
        }

        protected override void OnUnwireUIElement(RichTextBox uiElement)
        {
            // need to reset Document when scrolled out of view. Otherwise an exception is
            // thrown next time the assigned document is assigned to another new RichTextBoxCell
            uiElement.Document = new FlowDocument();
            uiElement.TextChanged -= new TextChangedEventHandler(uiElement_TextChanged);
            base.OnUnwireUIElement(uiElement);
        }

        protected override object GetControlValueFromEditorCore(RichTextBox uiElement)
        {
            var CompleteTextRange = new TextRange(uiElement.Document.ContentStart, uiElement.Document.ContentEnd);
            string text = CompleteTextRange.Text;   // Text in the RTE. 
            return text;
        }

        protected override void OnGridPreviewTextInput(TextCompositionEventArgs e)
        {
            //TraceUtil.TraceCurrentMethodInfo(e.ControlText);
            CurrentCell.ScrollInView();
            CurrentCell.BeginEdit(true);
        }

        /// <summary>
        /// Key Navigation Specification for Rich Text Eidtior.             
        ///                  Key                                   	                                       Action to be Processed 
        ///  Left Arrow is pressed while the cursor is not at 0 position	                     Move the cursor to before Character index position. 
        ///  Right Arrow is Pressed while cursor is not at last character of the Editor.	     Move the cursor to next Character index position.
        ///  UP Arrow is Pressed when the Cursor is not in First Row of RTE	                     Move the cursor to above line with the same character  index or last character index if above line if the length of the line is less than current caret position.
        ///  Down  Arrow is Pressed when the cursor is not in Last Row of RTE	                 Move the cursor to below line with the same character  index or last character index if next line if the length of the line is less than current caret position 
        ///  Left Arrow is pressed while the cursor is at 0 position	                         The current cell should be moved to Previous valid cell.  
        ///  Right Arrow is Pressed while cursor is at last character of the Editor.	         The current Cell should be moved to Next valid cell. 
        ///  UP Arrow is Pressed when the Cursor is in First Row of RTE	                         The current Cell should be moved to above Valid cell. 
        ///  Down  Arrow is Pressed when the cursor is in Last Row of RTE	                     The current cell should be moved to below valid cell. 
        ///  Tab  Key is pressed.  Never mind where the caret index is placed. 	                 The focus  should be moved to next cell.  
        ///  Delete Key is pressed.  Never mind where the caret index is placed.	             Should delete the next character from the current caret position in Editor. 
        ///  Backspace Key is pressed. Never mind where the caret index is placed.	             Should delete the previous character from the current caret position in Editor.
        ///  Esc Key is pressed.  Never mind where the caret index is placed.                    The Current cell must come out of Edit mode and focus should be in the Current Cell. 
        ///  CTRL + Left Key	                                                                 Move the caret symbol position to the starting character of previous WORD.
        ///  CTRL + Right Key	                                                                 Move the caret symbol position to the starting Character of next WORD.
        ///  CTRL + UP Key	                                                                     Move the caret symbol position to the starting Character of Previous  Paragraph.
        ///  CTRL + Down Key	                                                                 Move the caret symbol position to the starting Character of next Paragraph.
        ///  Shift + Left Key	                                                                 Should add the previous character from the current caret position in Selection.
        ///  Shift + Right Key	                                                                 Should add the next character from the current caret position in Selection.
        ///  Shift + UP Key	                                                                     Should add the previous line from the current caret position in Selection.(Selection will be added till the caret index above the current caret index) 
        ///  Shift + Down Key                                                                    Should add the next line from the current caret position in Selection. (Selection will be added till the caret index below the current caret index)
        ///  Home 	                                                                             Mover the cursor to the starting character index of the current line.
        ///  End		                                                                         Move the cursor to the last character index(end of the line) of current line. 
        ///  CTRL + Home 	                                                                     Mover the cursor to the starting character index of the Entire text in the  Cell(RTE).
        ///  CTRL + End		                                                                     Move the cursor to the last character index of the  Entire text in the  Cell(RTE).
        ///  Shift + Home 	                                                                     Mover the cursor to the starting character index of the current line. Also the Current Line will be added in selection.
        ///  Shift + End		                                                                 Move the cursor to the last character index(end of the line) of current line. Also the Current Line will be added in selection.
        ///  Enter(Return) Key is pressed. 	                                                     Cursor should move to next line. (new line) 
        ///  CRTL + Enter Key.                                                                   Current cell should be moved to next valid cell.
        /// </summary>
        /// <param name="KeyEventArgs"></param>
        protected override bool ShouldGridTryToHandlePreviewKeyDown(KeyEventArgs e)
        {
            bool isControlKey = (e.KeyboardDevice.Modifiers & ModifierKeys.Control) != ModifierKeys.None;
            bool isShiftKey = (e.KeyboardDevice.Modifiers & ModifierKeys.Shift) != ModifierKeys.None;
            if (!this.CurrentCell.IsEditing)
                return true;
            switch (e.Key)
            {
                case Key.Tab:
                    return true;
                case Key.Escape:
                    return true;
                case Key.Left:
                    {
                        RichTextBox Rtb = this.CurrentCellUIElement as RichTextBox;
                        var textRange = new TextRange(Rtb.Document.ContentStart, Rtb.CaretPosition);
                        string text = textRange.Text;
                        text = text.Replace("\n", "").Replace("\r", "").Replace("\t", "");
                        if (text.Length == 0)
                            return true;

                        return false;
                    }
                case Key.Right:
                    {
                        RichTextBox Rtb = this.CurrentCellUIElement as RichTextBox;
                        var textRange = new TextRange(Rtb.CaretPosition, Rtb.Document.ContentEnd);
                        string text = textRange.Text;
                        text = text.Replace("\n", "").Replace("\r", "").Replace("\t", "");
                        if (text.Length == 0)
                            return true;
                        return false;
                    }
                case Key.Enter:
                    {
                        if (isControlKey)
                        {
                            this.CurrentCell.MoveRight();
                            return true;
                        }

                        return false;
                    }

                case Key.Up:
                    {
                        RichTextBox Rtb = this.CurrentCellUIElement as RichTextBox;
                        var textRange = new TextRange(Rtb.Document.ContentStart, Rtb.CaretPosition);
                        if (!textRange.Text.Contains("\r\n"))
                        {
                            this.CurrentCell.MoveUp();
                            e.Handled = true;
                            return false;
                        }
                        return false;
                    }
                case Key.Down:
                    {
                        RichTextBox Rtb = this.CurrentCellUIElement as RichTextBox;
                            string myText = new TextRange(Rtb.Document.ContentStart, Rtb.Document.ContentEnd).Text;
                            var end_pointer = Rtb.Document.ContentStart.GetNextInsertionPosition(LogicalDirection.Forward).GetPositionAtOffset(myText.Length, LogicalDirection.Forward);
                            var textRange = new TextRange(Rtb.CaretPosition, end_pointer);
                            if (!textRange.Text.Contains("\r\n"))
                            {
                                this.CurrentCell.MoveDown();
                                e.Handled = true;
                                return false;
                            }
                        return false;
                    }
            }

            return false;
        }
    }


    public class RichTextPaint
    {
        private Dictionary<Size, VisualBrush> brushes = new Dictionary<Size, VisualBrush>();

        private Thickness margin = new Thickness(0);

        /// <summary>
        /// Default constructor.
        /// </summary>
        public RichTextPaint()
        {
        }

        /// <summary>
        /// Gets or sets the margins.
        /// </summary>
        public Thickness Margin
        {
            get { return this.margin; }
            set { this.margin = value; }
        }

        /// <summary>
        /// Draws the UpDown Control in the cell rectangle.
        /// </summary>
        /// <param name="dc">The drawing context.</param>
        /// <param name="rc">Cell rectangle.</param>
        /// <param name="text">Text to be drawn over the UpDown Control.</param>
        /// <param name="style">Cell style information.</param>
        /// <returns>Cell margins.</returns>
        public Thickness DrawRichText(DrawingContext dc, Rect rc, GridStyleInfo style)
        {
            VisualBrush vb = this.GetVisualBrush(rc.Size, style);
            dc.DrawRectangle(vb, null, rc);
            rc = this.SubtractBorderMargins(rc, this.margin);
            return this.margin;
            }

        /// <summary>
        /// Removes border margins to get cells client area.
        /// </summary>
        /// <param name="cellRect">Cell rectangle.</param>
        /// <param name="mi">The border margins.</param>
        /// <returns>Returns the cells client area.</returns>
        public Rect SubtractBorderMargins(Rect cellRect, Thickness mi)
        {
            if (cellRect.IsEmpty || cellRect.Width <= mi.Left + mi.Right || cellRect.Height <= mi.Top + mi.Bottom)
            {
                return new Rect(0, 0, 0, 0);
            }
            cellRect.Height -= mi.Bottom + mi.Top;
            cellRect.Y += mi.Top;
            cellRect.Width -= mi.Right + mi.Left;
            cellRect.X += mi.Left;
            return cellRect;
        }


        private VisualBrush GetVisualBrush(Size size, GridStyleInfo style)
        {
            bool wasAnimated = GridUtil.IsAnimated;
            try
            {
                RichTextBox parentTextBox = null;
                GridUtil.IsAnimated = false;
                FlowDocument fd = style.CellValue as FlowDocument;
                VisualBrush visualBrush;
                RichTextBox tempRichTextbox = new RichTextBox();
                if (fd!=null && fd.Parent != null)
                {
                    parentTextBox = fd.Parent as RichTextBox;
                    tempRichTextbox = parentTextBox;
                    //parentTextBox.Document = new FlowDocument();
                }
                else
                {
                    //the below null check is added to avoid the exception while rendering addnewrow cell
                    if (fd == null)
                        fd = new FlowDocument();

                    tempRichTextbox.Document = fd;
                }
                tempRichTextbox.Background = Brushes.Transparent;
                tempRichTextbox.BorderThickness = new Thickness(0);
                tempRichTextbox.Padding = new Thickness(0);
                tempRichTextbox.BeginInit();
                tempRichTextbox.Width = size.Width;
                tempRichTextbox.Height = size.Height;
                if (style.FlowDirection == FlowDirection.RightToLeft)
                {
                    // We Transform the Control to get its mirror View. 
                    tempRichTextbox.LayoutTransform = new ScaleTransform(-1, 1);
                }
                tempRichTextbox.EndInit();
                tempRichTextbox.Measure(size);
                tempRichTextbox.Arrange(new Rect(size));
                visualBrush = new VisualBrush();
                visualBrush.Visual = tempRichTextbox;
                this.brushes[size] = visualBrush;
                return visualBrush;
            }
            finally
            {
                GridUtil.IsAnimated = wasAnimated;
            }
        }
    }

    public class RichTextSelection : GridSelectCellsMouseController
    {
        public RichTextSelection(GridControlBase grid)
            : base(grid)
        {
        }

        public override void MouseDown(Scroll.MouseControllerEventArgs e)
        {
            base.MouseDown(e);
        }
    }


}