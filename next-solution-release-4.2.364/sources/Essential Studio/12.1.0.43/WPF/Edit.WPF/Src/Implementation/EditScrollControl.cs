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
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using Syncfusion.Windows.Controls.Scroll;

namespace Syncfusion.Windows.Edit
{
    /// <summary>
    ///
    /// </summary>
#if SyncfusionFramework4_0

    using System.ComponentModel;

    [DesignTimeVisible(false)]
#endif
    public class EditScrollControl : ScrollAxisControl
    {
        #region Local Variables

        /// <summary>
        /// Internal property to store row details
        /// </summary>
        internal IEditableLineSizeHost rowheights;

        /// <summary>
        /// Internal property to store Column details
        /// </summary>
        internal IEditableLineSizeHost columnwidths;

        /// <summary>
        /// instance for LineNumber property.
        /// </summary>
        private int m_linenumber = 0;

        /// <summary>
        /// bool variable IsDragging for check text dragging or not.
        /// </summary>
        private bool IsDragging = false;

        /// <summary>
        /// instance for CaretIndex property.
        /// </summary>
        private int m_index = 0;

        /// <summary>
        /// instance for Caret property.
        /// </summary>
        private LineItem m_currentitem = null;

        /// <summary>
        /// internal variable to hold EditControl reference.
        /// </summary>
        internal EditControl parentControl;

        /// <summary>
        /// bool variable isShiftPressed for check Shift pressed or not.
        /// </summary>
        internal bool isShiftPressed;

        /// <summary>
        /// int variable to hold the Selection start line.
        /// </summary>
        internal int selectionStartLine;

        /// <summary>
        /// int variable to hold the selection start index.
        /// </summary>
        internal int selectionStartIndex;

        /// <summary>
        /// bool variable indicating if the selection has started.
        /// </summary>
        internal bool isSelectionstarted;

        /// <summary>
        /// bool variable indicating if all the text in the control is selected.
        /// </summary>
        internal bool isSelectedAll;

        /// <summary>
        /// bool variable indicating if Ellipsis for collapsed item is selected.
        /// </summary>
        internal bool isEllipsisSelected = false;

        #endregion Local Variables

        #region Constructor

        /// <summary>
        ///
        /// </summary>
        public EditScrollControl()
        {
            isSelectedAll = false;
        }

        #endregion Constructor

        #region Properties

        /// <summary>
        /// Gets or sets current linenumber
        /// </summary>
        /// <value>
        /// Type: System.Int32
        /// </value>
        public int LineNumber
        {
            get
            {
                return m_linenumber;
            }

            set
            {
                m_linenumber = value;
            }
        }

        /// <summary>
        /// Gets or sets the index of the caret.
        /// </summary>
        /// <value>
        /// Type: System.Int32
        /// </value>
        public int CaretIndex
        {
            get
            {
                return m_index;
            }

            set
            {
                m_index = value;
            }
        }

        /// <summary>
        /// Gets Cursor object
        /// </summary>
        /// <value>
        /// Type: Syncfusion.Windows.Edit.CursorLayer
        /// </value>
        internal CursorLayer Caret
        {
            get
            {
                return GetCursor(m_currentitem);
            }
        }

        /// <summary>
        /// Gets or sets the current line item.
        /// </summary>
        /// <value>
        /// Type: Syncfusion.Windows.Edit.LineItem
        /// </value>
        internal LineItem CurrentLineItem
        {
            get
            {
                return m_currentitem;
            }

            set
            {
                m_currentitem = value;
            }
        }

        /// <summary>
        /// Gets the parent edit control.
        /// </summary>
        /// <value>
        /// Type: Syncfusion.Windows.Edit.EditControl
        /// </value>
        internal EditControl ParentEditControl
        {
            get
            {
                return parentControl;
            }
        }

        /// <summary>
        /// Gets a value indicating the total width of fixed columns (LineNumber and Expand
        /// Collapse Column) in the EditScrollControl.
        /// </summary>
        internal double FixedWidth
        {
            get
            {
                if (columnwidths.LineCount > 2)
                {
                    return columnwidths[0] + columnwidths[1] + columnwidths[2] + columnwidths[3];
                }

                return 0;
            }
        }

        /// <summary>
        /// Gets or sets the TextSelectionPointer
        /// </summary>
        /// <value>
        /// Type: <see
        /// cref="T:Syncfusion.Windows.Edit.SelectionPointer">Syncfusion.Windows.Edit.SelectionPointer</see>
        /// </value>
        internal SelectionPointer TextSelectionPointer
        {
            get;
            set;
        }

        #endregion Properties

        #region Implementation

        /// <summary>
        /// Internal helper method to initialize the values for RowHeightsProvider and ColumnWidthsProvider.
        /// </summary>
        internal void InitializeValues(EditControl parent)
        {
            rowheights = new LineSizeCollection();
            columnwidths = new LineSizeCollection();
            RowHeightsProvider = new VariableLineWidthProvider(this, (LineSizeCollection)rowheights, true);
            ColumnWidthsProvider = new VariableLineWidthProvider(this, (LineSizeCollection)columnwidths, false);
            //VerticalPixelScroll = false;
            rowheights.DefaultLineSize = 100;
            columnwidths.DefaultLineSize = 10;
            columnwidths.LineCount = 5;
            columnwidths.HeaderLineCount = 4;
            parentControl = parent;
            if (this.ParentEditControl.AllowDragDrop)
                this.AllowDrop = true;
            if (parentControl != null && parentControl.Lines != null)
            {
                rowheights.LineCount = parentControl.Lines.Count;
                parentControl.Lines.CollectionChanged += new System.Collections.Specialized.NotifyCollectionChangedEventHandler(Lines_CollectionChanged);
                if (parentControl.Lines.Count > 0)
                {
                    parentControl.PreferredWidth = Math.Max(parentControl.Lines.Max(line => line.TextWidth) + 50, parentControl.PreferredWidth);
                }

                columnwidths[0] = CalculateLineNumberColumnWidth();
                if (this.ParentEditControl.IsTrackChangesEnabled)
                {
                    columnwidths[1] = this.ParentEditControl.ChangesIndicatorWidth;
                }
                else
                {
                    columnwidths[1] = 0;
                }

                if (this.ParentEditControl.EnableOutlining)
                {
                    columnwidths[2] = this.ParentEditControl.OutliningAreaWidth;
                }
                else
                {
                    columnwidths[2] = 0;
                }
                columnwidths[3] = 5;
                columnwidths[4] = parentControl.PreferredWidth;
            }

            Binding bindingRow = new Binding("RowHeightsProvider.DefaultLineSize");
            bindingRow.Source = this;
            bindingRow.Mode = BindingMode.OneWayToSource;
            parentControl.SetBinding(EditControl.LineHeightProperty, bindingRow);
            this.ParentEditControl.Drop += new DragEventHandler(ParentEditControl_Drop);
        }

        private void ParentEditControl_Drop(object sender, DragEventArgs e)
        {
            Point mousehit = e.GetPosition(this);
            if (((Keyboard.Modifiers & ModifierKeys.Control) != 0) && e.KeyStates == DragDropKeyStates.ControlKey && this.ParentEditControl.SelectedText != "" && this.ParentEditControl.AllowDragDrop)
            {
                LineItem item = this.GetLineItem(mousehit);
                this.ParentEditControl.Lines[item.LineNumber - 1].Text = InsertionManager.InsertText(this.ParentEditControl.Lines[item.LineNumber - 1].Text, this.parentControl.SelectedText, CaretIndex);
                int count = this.parentControl.SelectedText.Count();
                ClearSelection();
                AddSelectionLayer(item.LineNumber - 1, CaretIndex, CaretIndex + count);
            }
            else if (this.ParentEditControl.SelectedText != "" && this.ParentEditControl.AllowDragDrop && IsDragging)
            {
                IsDragging = false;
                LineItem item = this.GetLineItem(mousehit);
                int count = this.parentControl.SelectedText.Count();
                this.ParentEditControl.Lines[item.LineNumber - 1].Text = InsertionManager.InsertText(this.ParentEditControl.Lines[item.LineNumber - 1].Text, this.parentControl.SelectedText, CaretIndex);
                RemoveSelectedText(TextSelectionPointer);
                ClearSelection();
                AddSelectionLayer(item.LineNumber - 1, CaretIndex, CaretIndex + count);
                this.MoveCursorToLineItem(item.LineNumber - 1);
                this.CaretIndex = this.Caret.MoveToLocation(CaretIndex);
            }
        }

        /// <summary>
        /// Overriden method of ScrollAxisControl. It is used in the control to arrange the
        /// items based on the VisibleLinesCollection.
        /// </summary>
        /// <param name="arrangeSize">Specifies the size available for arrange the
        /// items.</param>
        protected override void OnArrangeContent(Size arrangeSize)
        {
            Point corner = new Point(ScrollColumns.ViewCorner, ScrollRows.ViewCorner);
            VisibleLinesCollection visibleRows = ScrollRows.GetVisibleLines();
            foreach (VisibleLineInfo visibleLine in visibleRows)
            {
                ArrangeRow(visibleLine, corner);
            }
        }

        /// <summary>
        /// Helper method used to Arrange items in the control.
        /// </summary>
        /// <param name="visibleRow">represents VisibleLineInfo of current item</param>
        /// <param name="corner">represents the point value</param>
        private void ArrangeRow(VisibleLineInfo visibleRow, Point corner)
        {
            int rowIndex = visibleRow.LineIndex;
            Rect cellRect = new Rect(0, visibleRow.Origin, ScrollColumns.ViewSize, visibleRow.Size);
            VisibleLinesCollection visibleColumns = ScrollColumns.GetVisibleLines();

            foreach (VisibleLineInfo visibleColumn in visibleColumns)
            {
                switch (visibleColumn.LineIndex)
                {
                    case 0:
                        if (this.ParentEditControl.ShowLineNumber)
                        {
                            ArrangeLineNumberElement(rowIndex, visibleColumn, visibleRow, cellRect);
                        }
                        break;

                    case 1:
                        if (this.ParentEditControl.IsTrackChangesEnabled)
                        {
                            ArrangeLineStateIndicatorElement(rowIndex, visibleColumn, visibleRow, cellRect);
                        }
                        break;

                    case 2:
                        if (this.ParentEditControl.EnableOutlining && this.ParentEditControl.CurrentLanguage.SupportsOutlining)
                        {
                            ArrangeExpandCollapseElement(rowIndex, visibleColumn, visibleRow, cellRect);
                        }
                        break;

                    case 4:
                        ArrangeLineItemElement(rowIndex, visibleColumn, visibleRow, cellRect);
                        break;
                }
            }
        }

        /// <summary>
        /// Helper method used to Arrange LineItem objects
        /// </summary>
        /// <param name="rowIndex">represents the index of the row</param>
        /// <param name="visibleColumn">represents the column where it has to be placed</param>
        /// <param name="visibleRow">represents the visibleRow object containing the size of visible row</param>
        /// <param name="cellRect">represents the Rect object</param>
        private void ArrangeLineItemElement(int rowIndex, VisibleLineInfo visibleColumn, VisibleLineInfo visibleRow, Rect cellRect)
        {
            if (rowIndex >= ParentEditControl.Lines.Count)
            {
                return;
            }

            LineItem element = ParentEditControl.Lines[rowIndex];
            ScrollControlChildFrame canvas = GetChildFrame(visibleColumn.IsHeader, visibleRow.IsHeader, visibleColumn.IsFooter, visibleRow.IsFooter, this.InnerFrame);
            if (canvas != null)
            {
                if (rowIndex == this.ScrollRows.ScrollLineIndex)
                {
                    ClearCanvasChildren(canvas, typeof(LineItem));
                    Canvas.SetZIndex(canvas, 0);
                }

                var item = canvas.Children.Where(textBlck => textBlck is TextBlock && (textBlck as TextBlock).Text == element.Text);
                if (item.Count() == 0)
                {
                    if (element.Parent != null && element.Parent is ScrollControlChildFrame)
                    {
                        ((ScrollControlChildFrame)element.Parent).Children.Remove(element);
                    }
                    canvas.Children.Add(element);
                }
            }

            if (double.IsNaN(element.Height))
            {
                element.Measure(new Size(double.MaxValue, double.MaxValue));
            }

            cellRect.X = visibleColumn.Origin;
            cellRect.Height = parentControl.LineHeight;
            element.Width = parentControl.PreferredWidth;
            cellRect.Width = parentControl.PreferredWidth;
            element.Arrange(cellRect);
        }

        /// <summary>
        /// Helper method used to Arrange Line number textblocks
        /// </summary>
        /// <param name="rowIndex">represents the index of the row</param>
        /// <param name="visibleColumn">represents the column where it has to be placed</param>
        /// <param name="visibleRow">represents the visibleRow object containing the size of visible row</param>
        /// <param name="arrangeRect">represents the Rect object</param>
        private void ArrangeLineNumberElement(int rowIndex, VisibleLineInfo visibleColumn, VisibleLineInfo visibleRow, Rect arrangeRect)
        {
            //Border element = new Border();

            TextBlock element = new TextBlock()
            {
                Text = (visibleRow.LineIndex + 1).ToString(),
                FontFamily = ParentEditControl.FontFamily,
                FontSize = ParentEditControl.FontSize,
                TextAlignment = TextAlignment.Right,
                Padding = new Thickness(2, 0, 5, 0)
            };
            Utils.BindObjectsToControl(element, TextBlock.ForegroundProperty, "LineNumberTextForeground");
            Utils.BindObjectsToControl(element, TextBlock.FontFamilyProperty, "FontFamily");
            Utils.BindObjectsToControl(element, TextBlock.FontSizeProperty, "FontSize");

            //element.Child = content;
            ScrollControlChildFrame canvas = GetChildFrame(visibleColumn.IsHeader, visibleRow.IsHeader, visibleColumn.IsFooter, visibleRow.IsFooter, this.InnerFrame);
            if (canvas != null)
            {
                if (rowIndex == this.ScrollRows.ScrollLineIndex)
                {
                    ClearCanvasChildren(canvas, typeof(TextBlock));
                    Canvas.SetZIndex(canvas, 10);
                }

                //var item = canvas.Children.Where(textBlck => textBlck is TextBlock && (textBlck as TextBlock).Text == element.Text);
                //if (item.Count() == 0)
                //{
                canvas.Children.Add(element);
                //}
            }

            arrangeRect.X = visibleColumn.Origin;
            arrangeRect.Width = visibleColumn.Size;
            arrangeRect.Height = parentControl.LineHeight;
            element.Height = parentControl.LineHeight;
            element.Width = arrangeRect.Width;
            element.Arrange(arrangeRect);
        }

        /// <summary>
        /// Helper method used to Arrange line state indicator elements
        /// </summary>
        /// <param name="rowIndex">represents the index of the row</param>
        /// <param name="visibleColumn">represents the column where it has to be placed</param>
        /// <param name="visibleRow">represents the visibleRow object containing the size of visible row</param>
        /// <param name="arrangeRect">represents the Rect object</param>
        private void ArrangeLineStateIndicatorElement(int rowIndex, VisibleLineInfo visibleColumn, VisibleLineInfo visibleRow, Rect arrangeRect)
        {
            Grid content = new Grid();
            content.SnapsToDevicePixels = true;
            content.HorizontalAlignment = HorizontalAlignment.Center;
            content.VerticalAlignment = VerticalAlignment.Center;
            content.Cursor = Cursors.Arrow;
            content.Tag = this.parentControl.Lines[rowIndex];
            MultiBinding multiBinding = new MultiBinding();
            Binding binding = new Binding("LineState");
            binding.Source = this.ParentEditControl.Lines[rowIndex];
            multiBinding.Bindings.Add(binding);
            Binding binding1 = new Binding("");
            binding1.Source = this.ParentEditControl.Lines[rowIndex];
            multiBinding.Bindings.Add(binding1);
            Binding binding2 = new Binding("SavedLineIndicatorBrush");
            binding2.Source = this.ParentEditControl;
            multiBinding.Bindings.Add(binding2);
            Binding binding3 = new Binding("ModifiedLineIndicatorBrush");
            binding3.Source = this.ParentEditControl;
            multiBinding.Bindings.Add(binding3);
            multiBinding.Converter = new LineStateToBackgroundConverter();
            content.SetBinding(Grid.BackgroundProperty, multiBinding);

            ScrollControlChildFrame canvas = GetChildFrame(visibleColumn.IsHeader, visibleRow.IsHeader, visibleColumn.IsFooter, visibleRow.IsFooter, this.InnerFrame);
            if (canvas != null)
            {
                canvas.IsHitTestVisible = true;
                if (rowIndex == this.ScrollRows.ScrollLineIndex)
                {
                    ClearCanvasChildren(canvas, typeof(Grid));
                    Canvas.SetZIndex(canvas, 10);
                }

                canvas.Children.Add(content);
            }

            arrangeRect.X = visibleColumn.Origin;
            arrangeRect.Width = visibleColumn.Size;
            arrangeRect.Height = parentControl.LineHeight;
            content.Height = parentControl.LineHeight;
            content.Width = arrangeRect.Width;
            content.Arrange(arrangeRect);
        }

        /// <summary>
        /// Helper method used to Arrange Expand collapse buttons
        /// </summary>
        /// <param name="rowIndex">represents the index of the row</param>
        /// <param name="visibleColumn">represents the column where it has to be placed</param>
        /// <param name="visibleRow">represents the visibleRow object containing the size of visible row</param>
        /// <param name="arrangeRect">represents the Rect object</param>
        private void ArrangeExpandCollapseElement(int rowIndex, VisibleLineInfo visibleColumn, VisibleLineInfo visibleRow, Rect arrangeRect)
        {
            Border element = new Border();
            element.SnapsToDevicePixels = true;
            ToggleButton content = new ToggleButton();
            content.SnapsToDevicePixels = true;
            content.HorizontalAlignment = HorizontalAlignment.Center;
            content.VerticalAlignment = VerticalAlignment.Center;
            content.HorizontalContentAlignment = HorizontalAlignment.Center;
            content.VerticalContentAlignment = VerticalAlignment.Center;
            content.Cursor = Cursors.Arrow;
            Utils.BindObjects(content, ToggleButton.IsCheckedProperty, "IsExpanded", this.ParentEditControl.Lines[rowIndex]);
            content.Tag = this.parentControl.Lines[rowIndex];
            MultiBinding multiBinding = new MultiBinding();
            Binding binding = new Binding("ContainsLines");
            binding.Source = this.ParentEditControl.Lines[rowIndex];
            multiBinding.Bindings.Add(binding);
            Binding binding1 = new Binding("");
            binding1.Source = this.ParentEditControl.Lines[rowIndex];
            multiBinding.Bindings.Add(binding1);
            Binding binding2 = new Binding("ParentLineNumber");
            binding2.Source = this.ParentEditControl.Lines[rowIndex];
            multiBinding.Bindings.Add(binding2);
            Binding binding3 = new Binding("IsEndLine");
            binding3.Source = this.ParentEditControl.Lines[rowIndex];
            binding3.UpdateSourceTrigger = UpdateSourceTrigger.PropertyChanged;
            binding3.Mode = BindingMode.TwoWay;
            multiBinding.Bindings.Add(binding3);
            Binding binding4 = new Binding("ShowBlockIndicatorLine");
            binding4.Source = this.ParentEditControl;
            binding4.Mode = BindingMode.TwoWay;
            multiBinding.Bindings.Add(binding4);
            multiBinding.Converter = new LinePropertiesToStyleConverter();
            multiBinding.ConverterParameter = this.ParentEditControl;
            content.SetBinding(ToggleButton.StyleProperty, multiBinding);

            element.Child = content;
            content.ClickMode = ClickMode.Press;
            content.Click += new RoutedEventHandler(content_Click);
            ScrollControlChildFrame canvas = GetChildFrame(visibleColumn.IsHeader, visibleRow.IsHeader, visibleColumn.IsFooter, visibleRow.IsFooter, this.InnerFrame);
            if (canvas != null)
            {
                canvas.IsHitTestVisible = true;
                if (rowIndex == this.ScrollRows.ScrollLineIndex)
                {
                    ClearCanvasChildren(canvas, typeof(Border));
                    Canvas.SetZIndex(canvas, 10);
                }

                canvas.Children.Add(element);
            }

            arrangeRect.X = visibleColumn.Origin;
            arrangeRect.Width = visibleColumn.Size;
            arrangeRect.Height = parentControl.LineHeight;
            element.Height = parentControl.LineHeight;
            element.Width = arrangeRect.Width;
            element.Arrange(arrangeRect);
        }

        /// <summary>
        /// Helper method to update the Hidden state of the VisibleLineInfo between a range
        /// </summary>
        /// <param name="start">represents the start index from where the hidden state has to be updated</param>
        /// <param name="end">represents the end index from where the hidden state has to be updated</param>
        /// <param name="hidden">represents the hidden state to be applied for the VisibleLineInfo object</param>
        internal void UpdateExpandStatus(int start, int end, bool hidden)
        {
            for (int i = start; i >= 0 && i <= end && i < ScrollRows.LineCount; i++)
            {
                this.rowheights.SetHidden(i, i, hidden);
                if (!hidden && !this.ParentEditControl.Lines[i].IsExpanded)
                {
                    i = this.ParentEditControl.Lines[i].EndLine - 1;
                }
            }
        }

        /// <summary>
        /// Helper method to check if the parent line item is expanded
        /// </summary>
        /// <param name="item">represents the lineitem to which this condition has to be checked</param>
        /// <returns>a value indicating whether the item's parent is expanded or not</returns>
        private bool GetParentExpanded(LineItem item)
        {
            if (item.ParentLineNumber > 0)
            {
                LineItem parentLineItem = this.ParentEditControl.Lines[item.ParentLineNumber - 1];
                return parentLineItem.IsExpanded;
            }
            return true;
        }

        /// <summary>
        /// Helper method used to clear all children in Canvas.
        /// </summary>
        /// <param name="canvas">represents the canvas object</param>
        /// <param name="objectType"> represents the Type</param>
        private void ClearCanvasChildren(ScrollControlChildFrame canvas, Type objectType)
        {
            for (int i = canvas.Children.Count; i > 0; i--)
            {
                if (canvas.Children[i - 1].GetType().Equals(objectType))
                {
                    canvas.Children.RemoveAt(i - 1);
                }
            }
        }

        /// <summary>
        /// Internal helper method to reset the internal collections and line sizes.
        /// </summary>
        internal void ResetItems()
        {
            VisibleLinesCollection columncoll = ScrollColumns.GetVisibleLines();
            VisibleLineInfo column = null;
            VisibleLinesCollection rowscoll = ScrollRows.GetVisibleLines();
            VisibleLineInfo row = null;

            if (columncoll == null)
            {
                return;
            }

            if (rowscoll == null)
            {
                return;
            }

            if (columncoll.Count > 0)
            {
                column = columncoll[0];
            }

            if (rowscoll.Count > 0)
            {
                row = rowscoll[0];
            }

            if (row != null && column != null)
            {
                ScrollControlChildFrame canvas = GetChildFrame(column.IsHeader, row.IsHeader, column.IsFooter, row.IsFooter, this.InnerFrame);
                if (canvas != null)
                {
                    canvas.Children.Clear();
                }
            }

            ScrollRows.ResetLineResize();
            ScrollRows.ResetVisibleLines();
            ScrollColumns.ResetVisibleLines();
        }

        /// <summary>
        /// Helper method to calculate the width of the line number column in
        /// EditScrollControl. Width of the column is calculated based on the number of
        /// lines in the EditControl.Lines collection
        /// </summary>
        /// <returns>
        /// returns desired width of the line number column
        /// </returns>
        internal double CalculateLineNumberColumnWidth()
        {
            if (!parentControl.ShowLineNumber)
            {
                return 0;
            }

            double returnValue = 0d;
            if (parentControl.IsAutoLineNumberAreaWidthEnabled)
            {
                if (parentControl.Lines.Count > 0)
                {
                    if (parentControl.Lines.Count < 1000)
                    {
                        returnValue = (Utils.GetWidth("999", parentControl.FontFamily, parentControl.FontSize, parentControl.Foreground)) + 15;
                    }
                    else
                    {
                        returnValue = (Utils.GetWidth(parentControl.Lines.Count.ToString(), parentControl.FontFamily, parentControl.FontSize, parentControl.Foreground)) + 15;
                    }
                }
            }
            else
            {
                returnValue = this.ParentEditControl.LineNumberAreaWidth;
            }
            this.ParentEditControl.ActualLineNumberAreaWidth = returnValue;
            return returnValue;
        }

        /// <summary>
        /// Returns lineitem on a given location. Used to get the lineitem on which the click was made
        /// </summary>
        /// <param name="point">The point of the click position.</param>
        /// <returns>Returns the Line item based on the point position.</returns>
        internal LineItem GetLineItem(Point point)
        {
            int itemcnt = 0;
            itemcnt = (int)(point.Y / parentControl.LineHeight);
            var lineInfo = this.ScrollRows.GetVisibleLineAtPoint(point.Y);
            if (lineInfo != null)
            {
                if (lineInfo.VisibleIndex > itemcnt)
                {
                    itemcnt = lineInfo.VisibleIndex;
                }
            }

            VisibleLinesCollection visibleLines = this.ScrollRows.GetVisibleLines();
            if (itemcnt < visibleLines.Count && itemcnt >= 0)
            {
                int itemIndex = visibleLines[itemcnt].LineIndex;
                if (itemIndex >= ParentEditControl.Lines.Count)
                {
                    return null;
                }

                return this.ParentEditControl.Lines[itemIndex];
            }
            else if (itemcnt < 0)
            {
                if (this.ScrollRows.ScrollLineIndex > 0)
                {
                    return this.ParentEditControl.Lines[this.ScrollRows.ScrollLineIndex - 1];
                }
                else
                {
                    return this.ParentEditControl.Lines[0];
                }
            }

            if (visibleLines[visibleLines.Count - 1].LineIndex + 1 < this.ParentEditControl.Lines.Count)
            {
                return this.ParentEditControl.Lines[visibleLines[visibleLines.Count - 1].LineIndex + 1];
            }
            else
            {
                return this.ParentEditControl.Lines[this.ParentEditControl.Lines.Count - 1];
            }
        }

        /// <summary>
        /// Helper method to update the cursor location
        /// </summary>
        /// <param name="lineitem">The lineitem to update the cursor location.</param>
        /// <param name="pt">The point object used to update the cursor point.</param>
        internal void SetCursorLocation(LineItem lineitem, Point pt)
        {
            LineItem previousitem = CurrentLineItem;
            CursorLayer cursorlayer = null;

            if (lineitem == null)
            {
                return;
            }

            if (previousitem != lineitem)
            {
                if (previousitem != null)
                {
                    RemoveCursorLayer(previousitem);
                    previousitem.SetCursorOnLoad = false;
                    previousitem.SetCursorIndex = 0;
                }

                cursorlayer = new CursorLayer(lineitem);
                AdornerLayer layer = AdornerLayer.GetAdornerLayer(lineitem);
                if (layer != null)
                {
                    layer.Add(cursorlayer);
                }
            }
            else
            {
                cursorlayer = GetCursor(lineitem);
            }

            CurrentLineItem = lineitem;

            if (cursorlayer == null && CurrentLineItem.LineNumber > 0)
            {
                MoveCursorToLineItem(this.ParentEditControl.Lines.IndexOf(ParentEditControl.Lines[CurrentLineItem.LineNumber - 1]));
                cursorlayer = GetCursor(CurrentLineItem);
            }

            m_index = FindPosition(CurrentLineItem, pt);
            if (cursorlayer == null)
                return;

            string text = CurrentLineItem.Text;

            if (!CurrentLineItem.IsExpanded)
            {
                text = this.ParentEditControl.CurrentLanguage.GetCollapsedItemText(CurrentLineItem);
            }

            if (m_index < text.Length)
            {
                cursorlayer.CaretPosition = new Point(Utils.GetWidth(text.Substring(0, m_index), ParentEditControl.FontFamily, ParentEditControl.FontSize, ParentEditControl.Foreground), 0);
            }
            else
            {
                if (m_index == text.Length)
                {
                    cursorlayer.CaretPosition = new Point(Utils.GetWidth(text, ParentEditControl.FontFamily, ParentEditControl.FontSize, ParentEditControl.Foreground), 0);
                }
                else
                {
                    m_index = cursorlayer.MoveToBegin();
                }
            }

            LineNumber = CurrentLineItem.LineNumber - 1;
            cursorlayer.CursorIndex = m_index;
            cursorlayer.LineNumber = LineNumber;
            cursorlayer.LineItemHost = CurrentLineItem;
            if (Keyboard.FocusedElement != this.ScrollOwner)
            {
                FocusManager.SetFocusedElement(this.ParentEditControl, this.ScrollOwner);
                Keyboard.Focus(this.ParentEditControl);
            }
            previousitem = null;
        }

        /// <summary>
        /// Helper method to remove cursor from LineItem
        /// </summary>
        /// <param name="item">The Line item for remove the cursor.</param>
        internal void RemoveCursorLayer(LineItem item)
        {
            if (item == null)
            {
                return;
            }

            AdornerLayer layer = AdornerLayer.GetAdornerLayer(item);
            if (layer != null)
            {
                Adorner[] adorners = layer.GetAdorners(item);

                if (adorners == null)
                {
                    return;
                }

                foreach (Adorner adorner in adorners)
                {
                    if (adorner is CursorLayer)
                    {
                        layer.Remove(adorner);
                    }
                }
            }
        }

        /// <summary>
        /// Helper method that returns Cursor object in the lineitem
        /// </summary>
        /// <param name="item">The Line item for get the cursor.</param>
        /// <returns>Returns the Cursor object based on the specified line item.</returns>
        private CursorLayer GetCursor(LineItem item)
        {
            if (item == null)
            {
                return null;
            }

            AdornerLayer layer = AdornerLayer.GetAdornerLayer(item);
            if (layer != null)
            {
                Adorner[] adorners = layer.GetAdorners(item);
                if (adorners == null)
                {
                    return null;
                }

                foreach (Adorner adorner in adorners)
                {
                    if (adorner is CursorLayer)
                    {
                        return (CursorLayer)adorner;
                    }
                }
            }

            return null;
        }

        /// <summary>
        /// Returns Index of the character in the text based on the point value
        /// </summary>
        /// <param name="currentitem">The current line item.</param>
        /// <param name="point">The point to get the cursor index.</param>
        /// <returns>Returns the current item Index value.</returns>
        private int FindPosition(LineItem currentitem, Point point)
        {
            double headerWidth = this.ScrollColumns.GetLineSize(1) + this.ScrollColumns.GetLineSize(3);
            if (this.ParentEditControl.EnableOutlining)
            {
                headerWidth += this.ScrollColumns.GetLineSize(2);
            }

            if (this.ParentEditControl.ShowLineNumber)
            {
                headerWidth += this.ScrollColumns.GetLineSize(0);
            }

            double pointValue = Math.Max(point.X - headerWidth + this.HorizontalOffset, 0);

            string currentLineItemText = currentitem.IsExpanded ? currentitem.Text : this.ParentEditControl.CurrentLanguage.GetCollapsedItemText(currentitem);
            double textwidth = Utils.GetWidth(currentLineItemText, ParentEditControl.FontFamily, ParentEditControl.FontSize, ParentEditControl.Foreground);
            List<char> charList = null;
            int ellipsisIndex = -1;
            if (currentitem.IsExpanded)
            {
                charList = new List<char>(currentLineItemText.ToCharArray());
            }
            else
            {
                if (currentitem.ContainsPreprocessor)
                {
                    charList = new List<char>(currentLineItemText.ToCharArray(0, currentLineItemText.IndexOf(currentitem.PreprocessorText)));
                    ellipsisIndex = currentLineItemText.IndexOf(currentitem.PreprocessorText);
                }
                else
                {
                    ellipsisIndex = currentLineItemText.IndexOf(this.ParentEditControl.CurrentLanguage.EllipsisText);
                    charList = new List<char>(currentLineItemText.ToCharArray(0, ellipsisIndex));
                    charList.AddRange(currentLineItemText.Substring(ellipsisIndex + this.ParentEditControl.CurrentLanguage.EllipsisText.Length).ToCharArray());
                }
            }

            double width = 0;
            double charwidth = 0;
            for (int i = 0; i < charList.Count; i++)
            {
                charwidth = Utils.GetWidth(charList[i].ToString(), ParentEditControl.FontFamily, ParentEditControl.FontSize, ParentEditControl.Foreground);
                if (pointValue < (width + charwidth))
                {
                    if (currentitem.IsExpanded)
                    {
                        if (pointValue > (width + Math.Round(charwidth * 0.85, 2)))
                        {
                            return i + 1;
                        }
                        return i;
                    }
                    int val = i;

                    if (pointValue > (width + Math.Round(charwidth * 0.85, 2)))
                    {
                        val = i + 1;
                    }

                    if (val > ellipsisIndex && val < this.ParentEditControl.CurrentLanguage.EllipsisText.Length + ellipsisIndex)
                    {
                        return ellipsisIndex + this.ParentEditControl.CurrentLanguage.EllipsisText.Length;
                    }
                    else
                    {
                        return val;
                    }
                }
                width += charwidth;
            }
            return currentLineItemText.Length;
        }

        /// <summary>
        /// Helper method to move cursor to specified lineitem
        /// </summary>
        /// <param name="index">The index to move the cursor position.</param>
        internal void MoveCursorToLineItem(int index)
        {
            if (((index == this.ParentEditControl.Lines.IndexOf(CurrentLineItem)) && Caret != null && CurrentLineItem.LineNumber == ((LineItem)this.ParentEditControl.Lines[index]).LineNumber) || index >= this.ParentEditControl.Lines.Count || index < 0)
            {
                return;
            }

            if (CurrentLineItem == null)
            {
                CurrentLineItem = this.ParentEditControl.Lines[index] as LineItem;
            }

            LineItem item = CurrentLineItem;
            CursorLayer cursorlayer = GetCursor(item);

            if (cursorlayer != null)
            {
                AdornerLayer.GetAdornerLayer(item).Remove(cursorlayer);
            }

            if (index >= this.ParentEditControl.Lines.Count)
            {
                return;
            }

            var loadLines = this.ParentEditControl.Lines.Where(line => line.SetCursorOnLoad);
            foreach (LineItem lineItem in loadLines)
            {
                lineItem.SetCursorOnLoad = false;
            }

            CurrentLineItem = this.ParentEditControl.Lines[index] as LineItem;

            CurrentLineItem.SetCursorOnLoad = true;

            if (CurrentLineItem == null)
            {
                ((LineItem)this.ParentEditControl.Lines[index]).SetCursorOnLoad = true;
                return;
            }

            AdornerLayer layer = AdornerLayer.GetAdornerLayer(CurrentLineItem);
            cursorlayer = new CursorLayer(CurrentLineItem);

            if (layer != null)
            {
                layer.Add(cursorlayer);
                LineNumber = CurrentLineItem.LineNumber - 1;
                cursorlayer.CursorIndex = m_index;
                cursorlayer.LineNumber = LineNumber;
                cursorlayer.LineItemHost = CurrentLineItem;
                CurrentLineItem.SetCursorOnLoad = false;
            }
        }

        /// <summary>
        /// Helper method that fetches immediate next word
        /// </summary>
        /// <param name="index">The index of the word.</param>
        /// <returns>Returns the index of the next word.</returns>
        /// <remarks>GetNextWord used to fetch the next word, based on the index value.</remarks>
        private int GetNextWord(int index)
        {
            if (this.CurrentLineItem != null && this.CurrentLineItem.WordsCollection != null)
            {
                List<WordDetails> wordsCollection = null;
                if (CurrentLineItem.IsExpanded)
                {
                    wordsCollection = new List<WordDetails>(this.parentControl.CurrentLanguage.SplitTextToWords(CurrentLineItem.Text));
                }
                else
                {
                    var strText = this.ParentEditControl.CurrentLanguage.GetCollapsedItemText(this.CurrentLineItem);
                    wordsCollection = this.GenerateCollapsedItemWordsCollection(strText, CurrentLineItem);
                }

                var worditems = from wd in wordsCollection
                                where wd.StartIndex > index
                                select wd;

                foreach (WordDetails words in worditems)
                {
                    if (words != null)
                    {
                        if (words.Text == " ")
                        {
                            return words.EndIndex;
                        }
                        else
                        {
                            return words.StartIndex;
                        }
                    }
                }
            }

            return this.CurrentLineItem.IsExpanded ? this.CurrentLineItem.Text.Length : this.ParentEditControl.CurrentLanguage.GetCollapsedItemText(this.CurrentLineItem).Length;
        }

        /// <summary>
        /// Helper method to generate words for iterating when control + arrow key is used for collapsed lines
        /// </summary>
        /// <param name="strText">represents the collapsed item text</param>
        /// <param name="currentLineItem"> represents the currentLine</param>
        /// <returns>returns a list of WordDetails based on the text</returns>
        private List<WordDetails> GenerateCollapsedItemWordsCollection(string strText, LineItem currentLineItem)
        {
            string ellipsistext = currentLineItem.ContainsPreprocessor ? currentLineItem.PreprocessorText : this.ParentEditControl.CurrentLanguage.EllipsisText;
            int ellipsisindex = strText.IndexOf(ellipsistext);
            List<WordDetails> wordsCollection = new List<WordDetails>(this.ParentEditControl.CurrentLanguage.SplitTextToWords(strText.Substring(0, ellipsisindex)));
            Point start = wordsCollection.Count > 0 ? wordsCollection[wordsCollection.Count - 1].EndPosition : new Point();

            WordDetails word = new WordDetails();
            word.Text = ellipsistext;
            word.StartIndex = ellipsisindex;
            word.EndIndex = ellipsisindex + word.Text.Length;
            double width = Utils.GetWidth(word.Text, this.ParentEditControl.FontFamily, ParentEditControl.FontSize, ParentEditControl.Foreground);
            word.StartPosition = start;
            start = new Point(start.X + width, start.Y);
            word.EndPosition = start;
            wordsCollection.Add(word);

            int wordIndex = ellipsisindex + word.Text.Length;
            if (strText.Length > wordIndex)
            {
                var remaingWords = this.ParentEditControl.CurrentLanguage.SplitTextToWords(strText.Substring(wordIndex));
                foreach (WordDetails words in remaingWords)
                {
                    words.StartIndex = wordIndex;
                    words.EndIndex = wordIndex + words.Text.Length;
                    wordIndex += words.Text.Length;
                }
                wordsCollection.AddRange(remaingWords);
            }

            return wordsCollection;
        }

        /// <summary>
        /// Gets the current word.
        /// </summary>
        /// <param name="index">The index of the word.</param>
        /// <returns>Returns the current word.</returns>
        /// <remarks>GetCurrentWord used to fetch the current word, based on the index value.</remarks>
        internal WordDetails GetCurrentWord(int index)
        {
            if (this.CurrentLineItem != null && this.CurrentLineItem.WordsCollection != null)
            {
                var worditems = from wd in this.CurrentLineItem.WordsCollection
                                where wd.StartIndex <= (index - 1)
                                orderby wd.StartIndex descending
                                select wd;

                foreach (WordDetails words in worditems)
                {
                    if (words != null)
                    {
                        return words;
                    }
                }
            }

            return null;
        }

        /// <summary>
        /// Fetches the previous word boundary based on the index
        /// </summary>
        /// <param name="index">The index of the current word.</param>
        /// <returns>Returns the Previous word's start index.</returns>
        /// <remarks>GetPreviousWord used to fetch the previous word's start index, based on the index value.</remarks>
        internal int GetPreviousWordIndex(int index)
        {
            if (this.CurrentLineItem != null && this.CurrentLineItem.WordsCollection != null)
            {
                List<WordDetails> wordsCollection = null;
                if (CurrentLineItem.IsExpanded)
                {
                    wordsCollection = new List<WordDetails>(this.parentControl.CurrentLanguage.SplitTextToWords(CurrentLineItem.Text));
                }
                else
                {
                    var strText = this.ParentEditControl.CurrentLanguage.GetCollapsedItemText(this.CurrentLineItem);
                    wordsCollection = this.GenerateCollapsedItemWordsCollection(strText, CurrentLineItem);
                }

                var worditems = from wd in wordsCollection
                                where wd.StartIndex < (index)
                                orderby wd.StartIndex descending
                                select wd;
                foreach (WordDetails words in worditems)
                {
                    if (words != null && words.Text.Trim() != string.Empty)
                    {
                        return words.StartIndex;
                    }
                }
            }

            return 0;
        }

        /// <summary>
        /// Fetches the previous word boundary based on the index
        /// </summary>
        /// <param name="index">The index of the current word.</param>
        /// <returns>Returns the Previous word.</returns>
        /// <remarks>GetPreviousWord used to fetch the previous word's start index, based on the index value.</remarks>
        internal WordDetails GetPreviousWord(int index)
        {
            if (this.CurrentLineItem != null && this.CurrentLineItem.WordsCollection != null)
            {
                List<WordDetails> wordsCollection = null;
                if (CurrentLineItem.IsExpanded)
                {
                    wordsCollection = new List<WordDetails>(this.parentControl.CurrentLanguage.SplitTextToWords(CurrentLineItem.Text));
                }
                else
                {
                    var strText = this.ParentEditControl.CurrentLanguage.GetCollapsedItemText(this.CurrentLineItem);
                    wordsCollection = this.GenerateCollapsedItemWordsCollection(strText, CurrentLineItem);
                }

                WordDetails currentWord = this.GetCurrentWord(index);

                var worditems = from wd in wordsCollection
                                where wd.StartIndex < (index)
                                orderby wd.StartIndex descending
                                select wd;
                foreach (WordDetails words in worditems)
                {
                    if (words != null && words.Text.Trim() != string.Empty && (currentWord == null || (currentWord != null && words.StartIndex != currentWord.StartIndex && words.Text != currentWord.Text)))
                    {
                        return words;
                    }
                }
            }

            return null;
        }

        /// <summary>
        /// Helper method to get the Selected text based on the SelectionPointer object
        /// </summary>
        /// <param name="pointer">The pointer of the text.</param>
        /// <returns>Returns the selected text based on the selection pointer.</returns>
        internal string GetSelectedText(SelectionPointer pointer)
        {
            if (pointer == null)
            {
                return string.Empty;
            }

            LineItem selitem;
            StringBuilder str = new StringBuilder(string.Empty);
            for (int i = pointer.StartLine; i <= pointer.EndLine; i++)
            {
                if (i >= ParentEditControl.Lines.Count)
                {
                    return str.ToString();
                }

                selitem = ParentEditControl.Lines[i] as LineItem;

                if (i == pointer.StartLine)
                {
                    if (pointer.StartLine == pointer.EndLine)
                    {
                        if (selitem.IsExpanded)
                        {
                            try
                            {
                                str.Append(selitem.Text.Substring(pointer.StartIndex, Math.Min(pointer.EndIndex - pointer.StartIndex, selitem.Text.Length)));
                            }
                            catch { }
                        }
                        else
                        {
                            string collapsedText = this.ParentEditControl.CurrentLanguage.GetCollapsedItemText(selitem);
                            int collapsedTextLength = collapsedText.Length;
                            int ellipsisIndex = 0;
                            if (selitem.ContainsPreprocessor)
                            {
                                var listener = selitem.GetPreprocessorType();
                                if (listener != null)
                                {
                                    ellipsisIndex = selitem.Text.IndexOf(listener.BlockStart);
                                }
                            }
                            else
                            {
                                ellipsisIndex = collapsedText.IndexOf(this.ParentEditControl.CurrentLanguage.EllipsisText);
                            }

                            if (pointer.EndIndex <= ellipsisIndex)
                            {
                                str.Append(selitem.Text.Substring(pointer.StartIndex, pointer.EndIndex - pointer.StartIndex));
                            }
                            else
                            {
                                if (pointer.StartIndex < selitem.Text.Length)
                                {
                                    str.Append(selitem.Text.Substring(pointer.StartIndex));
                                }
                                pointer.EndLine = selitem.EndLine - 1;
                                int plannedEndIndex = this.ParentEditControl.CurrentLanguage.GetCollapsedItemSelectionEndIndex(selitem, selitem.Text.Length);

                                int ellipsisEndind = ellipsisIndex + this.parentControl.CurrentLanguage.EllipsisText.Length;
                                if (pointer.EndIndex == ellipsisIndex + this.parentControl.CurrentLanguage.EllipsisText.Length)
                                {
                                    pointer.EndIndex = plannedEndIndex;
                                    //pointer.SelectionPointerChanged += new SelectionPointerChangedEventHandler(TextSelectionPointer_SelectionPointerChanged);
                                }
                                else
                                {
                                    int tempEndIndex = pointer.EndIndex - plannedEndIndex;
                                    pointer.EndIndex = plannedEndIndex + tempEndIndex;
                                }
                            }
                        }
                    }
                    else
                    {
                        str.Append(selitem.Text.Substring(Math.Min(pointer.StartIndex, selitem.Text.Length)));
                    }
                }
                else
                {
                    if (i == pointer.EndLine)
                    {
                        str.Append("\r\n" + selitem.Text.Substring(0, Math.Min(selitem.Text.Length, pointer.EndIndex)));
                    }
                    else
                    {
                        str.Append("\r\n" + selitem.Text);
                    }
                }
            }

            return str.ToString();
        }

        /// <summary>
        /// Helper method to update the selection pointer. In this method, values are StartLine and EndLine are validated and updated accordingly.
        /// </summary>
        /// <param name="startline">The startline of the selection pointer.</param>
        /// <param name="endline">The endline of the selection pointer.</param>
        /// <param name="startIndex">The start index of the selection pointer.</param>
        /// <param name="endindex">The endindex of the selection pointer.</param>
        internal void UpdateSelectionPointer(int startline, int endline, int startIndex, int endindex)
        {
            if (TextSelectionPointer == null)
            {
                TextSelectionPointer = new SelectionPointer();
                TextSelectionPointer.SelectionPointerChanged += new SelectionPointerChangedEventHandler(TextSelectionPointer_SelectionPointerChanged);
            }

            if (startline < endline)
            {
                TextSelectionPointer.StartIndex = startIndex;
                TextSelectionPointer.EndIndex = endindex;
                TextSelectionPointer.StartLine = startline;
                TextSelectionPointer.EndLine = endline;
            }
            else
            {
                TextSelectionPointer.StartLine = endline;
                TextSelectionPointer.EndLine = startline;
                if (startline == endline)
                {
                    if (startIndex < endindex)
                    {
                        TextSelectionPointer.StartIndex = startIndex;
                        TextSelectionPointer.EndIndex = endindex;
                    }
                    else
                    {
                        TextSelectionPointer.StartIndex = endindex;
                        TextSelectionPointer.EndIndex = startIndex;
                    }
                }
                else
                {
                    TextSelectionPointer.StartIndex = endindex;
                    TextSelectionPointer.EndIndex = startIndex;
                }
            }

            if (TextSelectionPointer.StartLine == 0 && TextSelectionPointer.EndLine == (ParentEditControl.Lines.Count - 1) && TextSelectionPointer.StartIndex == 0)
            {
                if (((LineItem)ParentEditControl.Lines[ParentEditControl.Lines.Count - 1]).Text.Length == TextSelectionPointer.EndIndex)
                {
                    isSelectedAll = true;
                }
                else
                {
                    isSelectedAll = false;
                }
            }
            else
            {
                isSelectedAll = false;
            }
        }

        /// <summary>
        /// Updates Selection in a LineItem based on SelectionPointer and index
        /// </summary>
        /// <param name="pointer">The selection pointer to update the selection.</param>
        /// <param name="i">The Line number of the selection pointer.</param>
        internal void UpdateSelection(SelectionPointer pointer, int i)
        {
            if (i >= 0 && ParentEditControl.Lines.Count > i)
            {
                if (ParentEditControl.Lines.Count > i)
                {
                    LineItem item = ParentEditControl.Lines[i];
                    string text = item.IsExpanded ? item.Text : this.ParentEditControl.CurrentLanguage.GetCollapsedItemText(item);
                    if (i == pointer.StartLine)
                    {
                        if (pointer.StartLine == pointer.EndLine)
                        {
                            AddSelectionLayer(pointer.StartLine, pointer.StartIndex, pointer.EndIndex);
                        }
                        else
                        {
                            if (this.parentControl.Lines[i].IsExpanded)
                            {
                                AddSelectionLayer(i, pointer.StartIndex, text.Length);
                            }
                            else
                            {
                                LineItem lineitem = this.ParentEditControl.Lines[i];
                                string collapsedText = this.ParentEditControl.CurrentLanguage.GetCollapsedItemText(lineitem);
                                int ellipsisIndex = collapsedText.IndexOf(this.ParentEditControl.CurrentLanguage.EllipsisText);
                                int index = this.ParentEditControl.CurrentLanguage.GetCollapsedItemSelectionEndIndex(lineitem, lineitem.Text.Length);
                                int endIndex = this.ParentEditControl.CurrentLanguage.GetSelectionEndIndex(lineitem);
                                if (pointer.EndLine == lineitem.EndLine - 1 && pointer.EndIndex == endIndex)
                                {
                                    AddSelectionLayer(i, pointer.StartIndex, index);
                                }
                                else if (pointer.EndLine > 1 || (pointer.EndLine == i && pointer.EndIndex > ellipsisIndex + this.ParentEditControl.CurrentLanguage.EllipsisText.Length))
                                {
                                    AddSelectionLayer(i, pointer.StartIndex, text.Length);
                                }
                                else
                                {
                                    AddSelectionLayer(i, pointer.StartIndex, ellipsisIndex + this.ParentEditControl.CurrentLanguage.EllipsisText.Length);
                                }
                            }
                        }
                    }
                    else
                    {
                        if (i == pointer.EndLine)
                        {
                            AddSelectionLayer(i, 0, pointer.EndIndex);
                        }
                        else
                        {
                            if (i > pointer.StartLine && i < pointer.EndLine)
                            {
                                AddSelectionLayer(i, 0, text.Length);
                            }
                            else
                            {
                                RemoveSelectionLayer(i);
                            }
                        }
                    }
                }
            }
        }

        /// <summary>
        /// Helper method that generates Selection rectangle geometry based on the selected text width and adds SelectionLayer adorner to the LineItem.
        /// </summary>
        /// <param name="line">The line to generate selection rectangle.</param>
        /// <param name="startIndex">The start index of the line.</param>
        /// <param name="endindex">The end index of the line.</param>
        internal void AddSelectionLayer(int line, int startIndex, int endindex)
        {
            if (line >= ParentEditControl.Lines.Count)
            {
                return;
            }
            LineItem lineitem = null;
            if (line >= 0)
                lineitem = ParentEditControl.Lines[line] as LineItem;

            int stindex;
            if (lineitem != null)
            {
                if (endindex < startIndex)
                {
                    stindex = endindex;
                    endindex = startIndex;
                    startIndex = stindex;
                }

                if (lineitem.ParentControl != null)
                {
                    bool isEndIndexChanged = false;
                    string text = lineitem.Text;
                    if (!lineitem.IsExpanded)
                    {
                        text = this.ParentEditControl.CurrentLanguage.GetCollapsedItemText(lineitem);
                        if (endindex >= text.Length)
                        {
                            isEndIndexChanged = true;
                        }
                    }
                    startIndex = startIndex < 0 ? 0 : startIndex;
                    double startx = Utils.GetWidth(text.Substring(0, Math.Min(startIndex, text.Length)), lineitem.ParentControl.FontFamily, lineitem.ParentControl.FontSize, lineitem.ParentControl.Foreground);
                    Point startpoint = new Point(startx, 0);

                    double endx = Utils.GetWidth(text.Substring(0, Math.Max(Math.Min(endindex, text.Length), 0)), lineitem.ParentControl.FontFamily, lineitem.ParentControl.FontSize, lineitem.ParentControl.Foreground);
                    Point endpoint = new Point(endx, 0);

                    Rect rect = new Rect(startpoint.X, 0, Math.Round(Math.Max((endpoint.X - startpoint.X), 0), 0), lineitem.ParentControl.LineHeight);
                    RectangleGeometry rectget = new RectangleGeometry(rect);

                    lineitem.SelectionStartIndex = startIndex;
                    if (!lineitem.IsExpanded && isEndIndexChanged)
                    {
                        lineitem.SelectionEndIndex = text.Length;
                    }
                    else
                    {
                        lineitem.SelectionEndIndex = Math.Min(endindex, lineitem.Text.Length);
                    }
                    lineitem.IsSelected = true;
                    lineitem.SelectionPath = rectget;
                }
                else
                {
                    lineitem.IsSelected = true;
                    lineitem.SelectionStartIndex = startIndex;
                    lineitem.SelectionEndIndex = endindex;
                }
            }
            else
            {
                lineitem.IsSelected = true;
                lineitem.SelectionStartIndex = startIndex;
                lineitem.SelectionEndIndex = endindex;
            }
        }

        /// <summary>
        /// Helper method to remove selection layer from the Lineitem
        /// </summary>
        /// <param name="line">The line number to remove selection layer.</param>
        internal void RemoveSelectionLayer(int line)
        {
            ParentEditControl.Lines[line].IsSelected = false;
        }

        /// <summary>
        /// Helper method to clear selection
        /// </summary>
        internal void ClearSelection()
        {
            var selitems = from item in ParentEditControl.Lines
                           where (item as LineItem).IsSelected == true
                           select item;
            foreach (LineItem selitem in selitems)
            {
                if (selitem.LineNumber < 0)
                {
                    selitem.IsSelected = false;
                }
                else
                {
                    RemoveSelectionLayer(selitem.LineNumber - 1);
                }
            }

            this.ParentEditControl.SelectedText = string.Empty;
            TextSelectionPointer = null;
            isSelectedAll = false;
            isEllipsisSelected = false;
            this.ParentEditControl.isIndentSelected = false;
            if (this.ParentEditControl.SearchResults != null)
            {
                this.ParentEditControl.FindOptions.IsSelectionSelected = false;
                this.ParentEditControl.SearchResults.EditTextSelection = null;
                this.ParentEditControl.SearchResults.IsSelectionChanged = false;
            }
        }

        /// <summary>
        /// Helper method used for moving cursor position based on key pressed and updates selection when shift is pressed
        /// </summary>
        /// <param name="e">The <see cref="System.Windows.Input.KeyEventArgs"/> instance containing the event data.</param>
        internal void KeyDownPreview(KeyEventArgs e)
        {
            KeyboardDevice device = e.KeyboardDevice;
            if (((e.KeyboardDevice.Modifiers == ModifierKeys.Shift) || (e.KeyboardDevice.Modifiers == (ModifierKeys.Shift | ModifierKeys.Control))) && (!isShiftPressed))
            {
                isShiftPressed = true;
                isSelectionstarted = true;
                if (TextSelectionPointer == null)
                {
                    selectionStartLine = LineNumber;
                    selectionStartIndex = CaretIndex;
                }
            }

            string currentLineItemText = CurrentLineItem.IsExpanded ? CurrentLineItem.Text : this.ParentEditControl.CurrentLanguage.GetCollapsedItemText(CurrentLineItem);
            switch (e.Key)
            {
                #region Right Arrow Navigation

                case Key.Right:
                    if (this.ParentEditControl.isIntellisenseBoxOpen)
                    {
                        var prevWord = this.GetCurrentWord(this.CaretIndex);
                        if (prevWord != null)
                        {
                            if (prevWord.Text == " " || prevWord.Text == this.ParentEditControl.CurrentLanguage.IntellisenseDrillDownChar.ToString() || this.CaretIndex == CurrentLineItem.Text.Length)
                            {
                                this.ParentEditControl.CurrentLanguage.HideIntellisensePopup();
                            }
                        }
                    }

                    if (this.Caret.CursorIndex >= currentLineItemText.Length)
                    {
                        if (this.ParentEditControl.SelectedText != string.Empty && this.TextSelectionPointer != null)
                        {
                            this.ScrollRows.ScrollInView(this.TextSelectionPointer.EndLine);
                            this.MoveCursorToLineItem(this.TextSelectionPointer.EndLine);
                            if (this.Caret != null)
                            {
                                this.CaretIndex = this.Caret.MoveToLocation(this.TextSelectionPointer.EndIndex);
                            }
                            else
                            {
                                this.ParentEditControl.Lines[this.TextSelectionPointer.EndLine].SetCursorOnLoad = true;
                                this.ParentEditControl.Lines[this.TextSelectionPointer.EndLine].SetCursorIndex = this.TextSelectionPointer.EndIndex;
                            }
                            ClearSelection();
                        }
                        else
                        {
                            if (this.CurrentLineItem.IsExpanded)
                            {
                                this.ScrollRows.ScrollInView(this.parentControl.LineNumber);
                                this.MoveCursorToLineItem(this.parentControl.LineNumber);
                            }
                            else
                            {
                                this.ScrollRows.ScrollInView(this.CurrentLineItem.EndLine);
                                this.MoveCursorToLineItem(this.CurrentLineItem.EndLine);
                            }

                            if (this.Caret != null)
                            {
                                this.CaretIndex = this.Caret.MoveToBegin();
                            }
                            else
                            {
                                this.parentControl.Lines[this.parentControl.LineNumber - 1].SetCursorIndex = 0;
                            }

                            if (device.Modifiers == ModifierKeys.Shift || device.Modifiers == (ModifierKeys.Shift | ModifierKeys.Control))
                            {
                                UpdateSelectionPointer(this.selectionStartLine, this.LineNumber, this.selectionStartIndex, this.CaretIndex);
                            }
                            else
                            {
                                ClearSelection();
                            }
                        }
                    }
                    else
                    {
                        if (device.Modifiers == ModifierKeys.None)
                        {
                            if (this.ParentEditControl.SelectedText != string.Empty && this.TextSelectionPointer != null)
                            {
                                this.ScrollRows.ScrollInView(this.TextSelectionPointer.EndLine);
                                this.MoveCursorToLineItem(this.TextSelectionPointer.EndLine);
                                if (this.Caret != null)
                                {
                                    this.CaretIndex = this.Caret.MoveToLocation(this.TextSelectionPointer.EndIndex);
                                }
                                else
                                {
                                    this.ParentEditControl.Lines[this.TextSelectionPointer.EndLine].SetCursorOnLoad = true;
                                    this.ParentEditControl.Lines[this.TextSelectionPointer.EndLine].SetCursorIndex = this.TextSelectionPointer.EndIndex;
                                }
                                ClearSelection();
                            }
                            else
                            {
                                this.CaretIndex = this.Caret.MoveTo(1);
                            }
                            ClearSelection();
                        }
                        else if (device.Modifiers == ModifierKeys.Control)
                        {
                            this.CaretIndex = this.Caret.MoveToLocation(this.GetNextWord(this.Caret.CursorIndex));
                            ClearSelection();
                        }
                        else if (device.Modifiers == ModifierKeys.Shift)
                        {
                            this.CaretIndex = this.Caret.MoveTo(1);
                            UpdateSelectionPointer(selectionStartLine, this.LineNumber, selectionStartIndex, this.CaretIndex);
                        }
                        else if (device.Modifiers == (ModifierKeys.Control | ModifierKeys.Shift))
                        {
                            this.CaretIndex = this.Caret.MoveToLocation(this.GetNextWord(this.CaretIndex));
                            UpdateSelectionPointer(selectionStartLine, this.LineNumber, selectionStartIndex, this.CaretIndex);
                        }
                    }

                    e.Handled = true;
                    break;

                #endregion Right Arrow Navigation

                #region Left Arrow Navigation

                case Key.Left:
                    if (this.ParentEditControl.isIntellisenseBoxOpen)
                    {
                        var prevWord = this.GetCurrentWord(this.CaretIndex);
                        if (prevWord != null)
                        {
                            if (prevWord.Text == " " || prevWord.Text == this.ParentEditControl.CurrentLanguage.IntellisenseDrillDownChar.ToString() || this.CaretIndex == 0)
                            {
                                this.ParentEditControl.CurrentLanguage.HideIntellisensePopup();
                            }
                        }
                    }
                    if (this.CaretIndex == 0)
                    {
                        if (this.parentControl.LineNumber > 1)
                        {
                            if (this.ParentEditControl.SelectedText != string.Empty && this.TextSelectionPointer != null)
                            {
                                this.ScrollRows.ScrollInView(this.TextSelectionPointer.StartLine);
                                this.MoveCursorToLineItem(this.TextSelectionPointer.StartLine);
                                this.CaretIndex = this.Caret.MoveToLocation(this.TextSelectionPointer.StartIndex);
                                ClearSelection();
                            }
                            else
                            {
                                var lineItem = this.ParentEditControl.Lines[this.parentControl.LineNumber - 2];
                                if (GetParentExpanded(lineItem))
                                {
                                    this.ScrollRows.ScrollInView(this.parentControl.LineNumber - 2);
                                    this.MoveCursorToLineItem(this.parentControl.LineNumber - 2);
                                }
                                else
                                {
                                    this.ScrollRows.ScrollInView(lineItem.ParentLineNumber - 1);
                                    this.MoveCursorToLineItem(lineItem.ParentLineNumber - 1);
                                }

                                if (this.Caret != null)
                                {
                                    this.CaretIndex = this.Caret.MoveToEnd();
                                }
                                else
                                {
                                    this.parentControl.Lines[this.parentControl.LineNumber - 1].SetCursorIndex = this.parentControl.Lines[this.parentControl.LineNumber - 1].Text.Length;
                                }

                                if (device.Modifiers == ModifierKeys.Shift || device.Modifiers == (ModifierKeys.Shift | ModifierKeys.Control))
                                {
                                    UpdateSelectionPointer(this.selectionStartLine, this.LineNumber, this.selectionStartIndex, this.CaretIndex);
                                }
                                else
                                {
                                    ClearSelection();
                                }
                            }
                        }
                    }
                    else
                    {
                        if (device.Modifiers == ModifierKeys.None)
                        {
                            if (this.ParentEditControl.SelectedText != string.Empty && this.TextSelectionPointer != null)
                            {
                                this.ScrollRows.ScrollInView(this.TextSelectionPointer.StartLine);
                                this.MoveCursorToLineItem(this.TextSelectionPointer.StartLine);
                                if (this.Caret != null)
                                {
                                    this.CaretIndex = this.Caret.MoveToLocation(this.TextSelectionPointer.StartIndex);
                                }
                                else
                                {
                                    this.ParentEditControl.Lines[this.TextSelectionPointer.StartLine].SetCursorIndex = this.TextSelectionPointer.StartIndex;
                                    this.ParentEditControl.Lines[this.TextSelectionPointer.StartLine].SetCursorOnLoad = true;
                                }
                            }
                            else
                            {
                                if (this.Caret != null)
                                    this.CaretIndex = this.Caret.MoveTo(-1);
                            }
                            ClearSelection();
                        }
                        else if (device.Modifiers == ModifierKeys.Control)
                        {
                            this.CaretIndex = this.Caret.MoveToLocation(this.GetPreviousWordIndex(this.Caret.CursorIndex));
                            ClearSelection();
                        }
                        else if (device.Modifiers == ModifierKeys.Shift)
                        {
                            this.CaretIndex = this.Caret.MoveTo(-1);
                            UpdateSelectionPointer(selectionStartLine, this.LineNumber, selectionStartIndex, this.CaretIndex);
                        }
                        else if (device.Modifiers == (ModifierKeys.Control | ModifierKeys.Shift))
                        {
                            this.CaretIndex = this.Caret.MoveToLocation(this.GetPreviousWordIndex(this.CaretIndex));
                            UpdateSelectionPointer(selectionStartLine, this.LineNumber, selectionStartIndex, this.CaretIndex);
                        }
                    }

                    e.Handled = true;
                    break;

                #endregion Left Arrow Navigation

                #region Up Arrow Navigation

                case Key.Up:
                    if (this.ParentEditControl.isIntellisenseBoxOpen)
                    {
                        if (device.Modifiers != ModifierKeys.None)
                        {
                            this.ParentEditControl.CurrentLanguage.HideIntellisensePopup();
                        }
                        else
                        {
                            if (this.ParentEditControl.intellisenseBox.SelectedItem == null)
                            {
                                this.ParentEditControl.intellisenseBox.SelectedIndex = 0;
                            }
                            else if (this.ParentEditControl.intellisenseBox.SelectedIndex > 0)
                            {
                                this.ParentEditControl.intellisenseBox.SelectedIndex -= 1;
                            }
                            this.ParentEditControl.intellisenseBox.ScrollIntoView(this.ParentEditControl.intellisenseBox.SelectedItem);
                            e.Handled = true;
                            return;
                        }
                    }

                    if (device.Modifiers == ModifierKeys.Control || device.Modifiers == (ModifierKeys.Control | ModifierKeys.Shift))
                    {
                        this.LineUp();
                        var lineItem = this.ParentEditControl.Lines[this.parentControl.LineNumber - 2];
                        if (this.ScrollRows.LastBodyVisibleLineIndex == this.parentControl.LineNumber - 1)
                        {
                            if (GetParentExpanded(lineItem))
                            {
                                this.MoveCursorToLineItem(this.parentControl.LineNumber - 2);
                            }
                            else
                            {
                                this.MoveCursorToLineItem(lineItem.ParentLineNumber - 1);
                            }
                        }
                    }
                    else
                    {
                        bool isCaretVisible = false;
                        if (this.parentControl.LineNumber > 1)
                        {
                            if (this.TextSelectionPointer != null && this.ParentEditControl.SelectedText != string.Empty && device.Modifiers != ModifierKeys.Shift)
                            {
                                int startLine = this.TextSelectionPointer.StartLine > 0 ? this.TextSelectionPointer.StartLine - 1 : 0;
                                var lineItem = this.ParentEditControl.Lines[startLine];
                                if (GetParentExpanded(lineItem))
                                {
                                    this.ScrollRows.ScrollInView(startLine);
                                    this.MoveCursorToLineItem(startLine);
                                }
                                else
                                {
                                    startLine = lineItem.ParentLineNumber - 1;
                                    this.ScrollRows.ScrollInView(lineItem.ParentLineNumber - 1);
                                    this.MoveCursorToLineItem(lineItem.ParentLineNumber - 1);
                                }
                                if (this.Caret != null)
                                {
                                    this.CaretIndex = this.Caret.MoveToLocation(this.TextSelectionPointer.StartIndex);
                                }
                                else
                                {
                                    this.ParentEditControl.Lines[startLine].SetCursorIndex = this.TextSelectionPointer.StartIndex;
                                    this.ParentEditControl.Lines[startLine].SetCursorOnLoad = true;
                                }
                            }
                            else
                            {
                                var lineItem = this.ParentEditControl.Lines[this.parentControl.LineNumber - 2];
                                if (GetParentExpanded(lineItem))
                                {
                                    this.ScrollRows.ScrollInView(this.parentControl.LineNumber - 2);
                                    this.MoveCursorToLineItem(this.parentControl.LineNumber - 2);
                                }
                                else
                                {
                                    this.ScrollRows.ScrollInView(lineItem.ParentLineNumber - 1);
                                    this.MoveCursorToLineItem(lineItem.ParentLineNumber - 1);
                                }
                                if (this.Caret != null)
                                {
                                    this.CaretIndex = this.Caret.MoveTo(0);
                                    isCaretVisible = true;
                                }
                                else
                                {
                                    isCaretVisible = false;
                                }
                            }
                        }

                        if (device.Modifiers == ModifierKeys.Shift)
                        {
                            if (!isCaretVisible && this.LineNumber > 0)
                            {
                                this.UpdateSelectionPointer(this.selectionStartLine, this.LineNumber - 1, this.selectionStartIndex, this.CaretIndex);
                            }
                            else
                            {
                                this.UpdateSelectionPointer(this.selectionStartLine, this.LineNumber, this.selectionStartIndex, this.CaretIndex);
                            }
                        }
                        else
                        {
                            ClearSelection();
                        }
                    }

                    e.Handled = true;
                    break;

                #endregion Up Arrow Navigation

                #region Down Arrow Navigation

                case Key.Down:
                    if (this.ParentEditControl.isIntellisenseBoxOpen)
                    {
                        if (device.Modifiers != ModifierKeys.None)
                        {
                            this.ParentEditControl.CurrentLanguage.HideIntellisensePopup();
                        }
                        else
                        {
                            if (this.ParentEditControl.intellisenseBox.SelectedItem == null)
                            {
                                this.ParentEditControl.intellisenseBox.SelectedIndex = 0;
                            }
                            else if (this.ParentEditControl.intellisenseBox.SelectedIndex >= 0)
                            {
                                this.ParentEditControl.intellisenseBox.SelectedIndex += 1;
                            }
                            this.ParentEditControl.intellisenseBox.ScrollIntoView(this.ParentEditControl.intellisenseBox.SelectedItem);
                            e.Handled = true;
                            return;
                        }
                    }
                    if (device.Modifiers == ModifierKeys.Control || device.Modifiers == (ModifierKeys.Control | ModifierKeys.Shift))
                    {
                        if (this.ScrollRows.ScrollLineIndex == this.parentControl.LineNumber - 1)
                        {
                            if (this.CurrentLineItem.IsExpanded)
                            {
                                this.MoveCursorToLineItem(this.parentControl.LineNumber);
                            }
                            else
                            {
                                this.MoveCursorToLineItem(this.CurrentLineItem.EndLine);
                            }
                        }

                        this.LineDown();
                    }
                    else
                    {
                        if (this.TextSelectionPointer != null && this.ParentEditControl.SelectedText != string.Empty && device.Modifiers != ModifierKeys.Shift)
                        {
                            var lineItem = this.ParentEditControl.Lines[this.TextSelectionPointer.EndLine];
                            int endLine = this.TextSelectionPointer.EndLine;
                            int endIndex = this.TextSelectionPointer.EndIndex;
                            if (lineItem.IsExpanded && this.TextSelectionPointer.EndLine + 1 < this.ParentEditControl.Lines.Count)
                            {
                                endLine = endLine + 1;
                            }
                            else if (lineItem.ContainsLines && !lineItem.IsExpanded && lineItem.EndLine < this.ParentEditControl.Lines.Count)
                            {
                                endLine = lineItem.EndLine;
                            }
                            else if (this.TextSelectionPointer.EndLine + 1 == this.ParentEditControl.Lines.Count || (lineItem.ContainsLines && lineItem.EndLine == this.ParentEditControl.Lines.Count))
                            {
                                endLine = this.ParentEditControl.Lines.Count - 1;
                                endIndex = this.ParentEditControl.Lines[endLine].Text.Length;
                            }
                            this.ScrollRows.ScrollInView(endLine);
                            this.MoveCursorToLineItem(endLine);
                            if (this.Caret != null)
                            {
                                this.CaretIndex = this.Caret.MoveToLocation(endIndex);
                            }
                            else
                            {
                                this.ParentEditControl.Lines[endLine].SetCursorOnLoad = true;
                                this.ParentEditControl.Lines[endLine].SetCursorIndex = endIndex;
                            }
                            ClearSelection();
                        }
                        else
                        {
                            if (this.parentControl.LineNumber < this.parentControl.Lines.Count)
                            {
                                if (this.CurrentLineItem.IsExpanded)
                                {
                                    this.ScrollRows.ScrollInView(this.parentControl.LineNumber);
                                    this.MoveCursorToLineItem(this.parentControl.LineNumber);
                                }
                                else
                                {
                                    this.ScrollRows.ScrollInView(this.parentControl.LineNumber);
                                    this.MoveCursorToLineItem(this.CurrentLineItem.EndLine);
                                }

                                if (this.Caret != null)
                                {
                                    this.CaretIndex = this.Caret.MoveTo(0);
                                    if (device.Modifiers == ModifierKeys.Shift)
                                    {
                                        this.UpdateSelectionPointer(this.selectionStartLine, this.LineNumber, this.selectionStartIndex, this.CaretIndex);
                                    }
                                    else
                                    {
                                        ClearSelection();
                                    }
                                }
                                else
                                {
                                    if (device.Modifiers == ModifierKeys.Shift)
                                    {
                                        this.UpdateSelectionPointer(this.selectionStartLine, this.parentControl.LineNumber, this.selectionStartIndex, this.CaretIndex);
                                    }
                                    else
                                    {
                                        ClearSelection();
                                    }
                                }
                            }
                            else if (this.ParentEditControl.LineNumber == this.ParentEditControl.Lines.Count)
                            {
                                if (this.Caret != null)
                                {
                                    this.CaretIndex = this.Caret.MoveToEnd();
                                    if (device.Modifiers == ModifierKeys.Shift)
                                    {
                                        this.UpdateSelectionPointer(this.selectionStartLine, this.LineNumber, this.selectionStartIndex, this.CaretIndex);
                                    }
                                    else
                                    {
                                        ClearSelection();
                                    }
                                }
                                else
                                {
                                    if (device.Modifiers == ModifierKeys.Shift)
                                    {
                                        var item = this.ParentEditControl.Lines[this.ParentEditControl.LineNumber - 1];
                                        this.UpdateSelectionPointer(this.selectionStartLine, this.parentControl.LineNumber, this.selectionStartIndex, item.Text.Length);
                                    }
                                    else
                                    {
                                        ClearSelection();
                                    }
                                }
                            }
                        }
                    }

                    e.Handled = true;
                    break;

                #endregion Down Arrow Navigation

                #region Home Key

                case Key.Home:
                    if (this.ParentEditControl.isIntellisenseBoxOpen)
                    {
                        this.ParentEditControl.CurrentLanguage.HideIntellisensePopup();
                    }
                    if (device.Modifiers == ModifierKeys.None || device.Modifiers == ModifierKeys.Shift)
                    {
                        if (device.Modifiers == ModifierKeys.None && this.ParentEditControl.SelectedText != string.Empty && this.TextSelectionPointer != null)
                        {
                            int startLine = this.TextSelectionPointer.StartLine;
                            int startIndex = this.TextSelectionPointer.StartIndex;
                            this.ScrollRows.ScrollInView(startLine);
                            this.MoveCursorToLineItem(startLine);
                            if (this.Caret != null)
                            {
                                this.CaretIndex = this.Caret.MoveToLocation(startIndex);
                            }
                            else
                            {
                                this.ParentEditControl.Lines[startLine].SetCursorIndex = startIndex;
                                this.ParentEditControl.Lines[startLine].SetCursorOnLoad = true;
                            }
                            ClearSelection();
                        }
                        else
                        {
                            if (this.Caret != null)
                            {
                                this.CaretIndex = this.Caret.MoveToBegin();
                                this.ScrollToLeftEnd();
                            }

                            if (device.Modifiers == ModifierKeys.Shift)
                            {
                                UpdateSelectionPointer(this.selectionStartLine, this.LineNumber, this.selectionStartIndex, 0);
                            }
                            else
                            {
                                ClearSelection();
                            }
                        }
                    }

                    if (device.Modifiers == ModifierKeys.Control || device.Modifiers == (ModifierKeys.Control | ModifierKeys.Shift))
                    {
                        this.ScrollRows.ScrollInView(0);
                        this.MoveCursorToLineItem(0);
                        if (this.Caret != null)
                        {
                            this.CaretIndex = this.Caret.MoveToBegin();
                        }
                        else
                        {
                            this.parentControl.Lines[0].SetCursorIndex = 0;
                        }

                        if (device.Modifiers == (ModifierKeys.Control | ModifierKeys.Shift))
                        {
                            UpdateSelectionPointer(this.selectionStartLine, 0, this.selectionStartIndex, 0);
                        }
                        else
                        {
                            ClearSelection();
                        }
                    }

                    e.Handled = true;
                    break;

                #endregion Home Key

                #region End Key

                case Key.End:
                    if (this.ParentEditControl.isIntellisenseBoxOpen)
                    {
                        this.ParentEditControl.CurrentLanguage.HideIntellisensePopup();
                    }
                    if (device.Modifiers == ModifierKeys.None || device.Modifiers == ModifierKeys.Shift)
                    {
                        if (device.Modifiers == ModifierKeys.None && this.ParentEditControl.SelectedText != string.Empty && this.TextSelectionPointer != null)
                        {
                            int endLine = this.TextSelectionPointer.EndLine;
                            int endIndex = this.TextSelectionPointer.EndIndex;

                            this.ScrollRows.ScrollInView(endLine);
                            this.MoveCursorToLineItem(endLine);
                            if (this.Caret != null)
                            {
                                this.CaretIndex = this.Caret.MoveToLocation(endIndex);
                            }
                            else
                            {
                                this.ParentEditControl.Lines[endLine].SetCursorIndex = endIndex;
                                this.ParentEditControl.Lines[endLine].SetCursorOnLoad = true;
                            }
                            ClearSelection();
                        }
                        else
                        {
                            if (this.Caret != null)
                            {
                                this.CaretIndex = this.Caret.MoveToEnd();
                                if (this.Caret.CaretPosition.X > this.ViewportWidth)
                                {
                                    this.ScrollToRightEnd();
                                }
                            }

                            if (device.Modifiers == ModifierKeys.Shift)
                            {
                                UpdateSelectionPointer(this.selectionStartLine, this.LineNumber, this.selectionStartIndex, this.CaretIndex);
                            }
                            else
                            {
                                ClearSelection();
                            }
                        }
                    }

                    if (device.Modifiers == ModifierKeys.Control || device.Modifiers == (ModifierKeys.Control | ModifierKeys.Shift))
                    {
                        this.ScrollRows.ScrollInView(this.ScrollRows.LineCount - 1);
                        this.MoveCursorToLineItem(this.ScrollRows.LineCount - 1);
                        if (this.Caret != null)
                        {
                            this.CaretIndex = this.Caret.MoveToEnd();
                            if (this.Caret.CaretPosition.X > this.ViewportWidth)
                            {
                                this.ScrollToRightEnd();
                            }
                        }
                        else
                        {
                            this.parentControl.Lines[this.parentControl.Lines.Count - 1].SetCursorIndex = this.parentControl.Lines[this.parentControl.Lines.Count - 1].Text.Length;
                        }

                        if (device.Modifiers == (ModifierKeys.Control | ModifierKeys.Shift))
                        {
                            UpdateSelectionPointer(this.selectionStartLine, this.ScrollRows.LineCount - 1, this.selectionStartIndex, this.CaretIndex);
                        }
                        else
                        {
                            ClearSelection();
                        }
                    }

                    e.Handled = true;
                    break;

                #endregion End Key

                #region Page Up

                case Key.PageUp:
                    if (this.ParentEditControl.isIntellisenseBoxOpen)
                    {
                        this.ParentEditControl.CurrentLanguage.HideIntellisensePopup();
                    }
                    if (device.Modifiers == ModifierKeys.None || device.Modifiers == ModifierKeys.Shift)
                    {
                        var visibleLines = this.ScrollRows.GetVisibleLines();
                        int itemNumber = (int)(this.parentControl.LineNumber - this.ScrollRows.ScrollLineIndex - 1);
                        if (this.parentControl.LineNumber > visibleLines.Count)
                        {
                            this.ScrollRows.ScrollToPreviousPage();

                            if (this.TextSelectionPointer != null)
                            {
                                if ((this.ScrollRows.ScrollLineIndex + itemNumber) < this.TextSelectionPointer.StartLine && this.LineNumber > this.TextSelectionPointer.StartLine)
                                {
                                    int startLine = this.TextSelectionPointer.StartLine;
                                    this.MoveCursorToLineItem(startLine);
                                    if (this.Caret != null)
                                    {
                                        this.CaretIndex = this.Caret.MoveToLocation(this.TextSelectionPointer.StartIndex);
                                    }
                                    else
                                    {
                                        this.ParentEditControl.Lines[startLine].SetCursorOnLoad = true;
                                        this.ParentEditControl.Lines[startLine].SetCursorIndex = this.TextSelectionPointer.StartIndex;
                                    }
                                    this.ClearSelection();
                                    return;
                                }
                            }

                            if (itemNumber > 0)
                            {
                                this.MoveCursorToLineItem(this.ScrollRows.ScrollLineIndex + itemNumber);
                                if (this.Caret != null)
                                {
                                    this.CaretIndex = this.Caret.MoveTo(0);
                                }
                                else
                                {
                                    this.ParentEditControl.Lines[this.ScrollRows.ScrollLineIndex + itemNumber].SetCursorOnLoad = true;
                                    this.ParentEditControl.Lines[this.ScrollRows.ScrollLineIndex + itemNumber].SetCursorIndex = this.CaretIndex;
                                }
                            }
                            else
                            {
                                this.MoveCursorToLineItem(0);
                                if (this.Caret != null)
                                {
                                    this.CaretIndex = this.Caret.MoveTo(0);
                                }
                                else
                                {
                                    this.ParentEditControl.Lines[0].SetCursorOnLoad = true;
                                    this.ParentEditControl.Lines[0].SetCursorIndex = this.CaretIndex;
                                }
                            }

                            if (device.Modifiers == ModifierKeys.Shift)
                            {
                                this.UpdateSelectionPointer(this.selectionStartLine, this.ScrollRows.ScrollLineIndex + itemNumber, this.selectionStartIndex, this.CaretIndex);
                            }
                            else
                            {
                                this.ClearSelection();
                            }
                        }
                        else
                        {
                            this.ScrollToTop();
                            if (this.TextSelectionPointer != null)
                            {
                                if ((this.ScrollRows.ScrollLineIndex) < this.TextSelectionPointer.StartLine && this.LineNumber > this.TextSelectionPointer.StartLine)
                                {
                                    int startLine = this.TextSelectionPointer.StartLine;
                                    this.MoveCursorToLineItem(startLine);
                                    if (this.Caret != null)
                                    {
                                        this.CaretIndex = this.Caret.MoveToLocation(this.TextSelectionPointer.StartIndex);
                                    }
                                    else
                                    {
                                        this.ParentEditControl.Lines[startLine].SetCursorOnLoad = true;
                                        this.ParentEditControl.Lines[startLine].SetCursorIndex = this.TextSelectionPointer.StartIndex;
                                    }
                                    this.ClearSelection();
                                    return;
                                }
                            }
                            this.MoveCursorToLineItem(0);
                            if (this.Caret != null)
                            {
                                this.CaretIndex = this.Caret.MoveTo(0);
                            }
                            else
                            {
                                this.ParentEditControl.Lines[0].SetCursorOnLoad = true;
                                this.ParentEditControl.Lines[0].SetCursorIndex = this.CaretIndex;
                            }
                            if (device.Modifiers == ModifierKeys.Shift)
                            {
                                this.UpdateSelectionPointer(this.selectionStartLine, 0, this.selectionStartIndex, this.CaretIndex);
                            }
                            else
                            {
                                this.ClearSelection();
                            }
                        }
                    }

                    if (device.Modifiers == ModifierKeys.Control || device.Modifiers == (ModifierKeys.Control | ModifierKeys.Shift))
                    {
                        this.MoveCursorToLineItem(this.ScrollRows.ScrollLineIndex);
                        if (device.Modifiers == (ModifierKeys.Control | ModifierKeys.Shift))
                        {
                            this.UpdateSelectionPointer(this.selectionStartLine, this.ScrollRows.ScrollLineIndex, this.selectionStartIndex, this.CaretIndex);
                        }
                        else
                        {
                            this.ClearSelection();
                        }
                    }

                    e.Handled = true;
                    break;

                #endregion Page Up

                #region Page Down

                case Key.PageDown:
                    if (this.ParentEditControl.isIntellisenseBoxOpen)
                    {
                        this.ParentEditControl.CurrentLanguage.HideIntellisensePopup();
                    }
                    if (device.Modifiers == ModifierKeys.None || device.Modifiers == ModifierKeys.Shift)
                    {
                        var visibleLines = this.ScrollRows.GetVisibleLines();
                        if (this.ScrollRows.LastBodyVisibleLineIndex < this.parentControl.Lines.Count - 1)
                        {
                            int itemNumber = (int)(this.parentControl.LineNumber - this.ScrollRows.ScrollLineIndex - 1);
                            this.ScrollRows.ScrollToNextPage();

                            if (this.TextSelectionPointer != null)
                            {
                                if (this.ScrollRows.LastBodyVisibleLineIndex < this.TextSelectionPointer.EndLine && this.LineNumber < this.TextSelectionPointer.EndLine)
                                {
                                    int endLine = this.TextSelectionPointer.EndLine;
                                    this.MoveCursorToLineItem(endLine);
                                    if (this.Caret != null)
                                    {
                                        this.CaretIndex = this.Caret.MoveToLocation(this.TextSelectionPointer.EndIndex);
                                    }
                                    else
                                    {
                                        this.ParentEditControl.Lines[endLine].SetCursorOnLoad = true;
                                        this.ParentEditControl.Lines[endLine].SetCursorIndex = this.TextSelectionPointer.EndIndex;
                                    }
                                    this.ClearSelection();
                                    return;
                                }
                            }

                            if (itemNumber >= 0)
                            {
                                this.MoveCursorToLineItem(this.ScrollRows.ScrollLineIndex + itemNumber);
                                if (this.Caret != null)
                                {
                                    this.CaretIndex = this.Caret.MoveTo(0);
                                }
                                else
                                {
                                    this.ParentEditControl.Lines[this.ScrollRows.ScrollLineIndex + itemNumber].SetCursorOnLoad = true;
                                    this.ParentEditControl.Lines[this.ScrollRows.ScrollLineIndex + itemNumber].SetCursorIndex = this.CaretIndex;
                                }
                            }
                            else
                            {
                                this.MoveCursorToLineItem(0);
                                if (this.TextSelectionPointer != null)
                                {
                                    if ((this.ScrollRows.LastBodyVisibleLineIndex) > this.TextSelectionPointer.EndLine && this.LineNumber < this.TextSelectionPointer.EndLine)
                                    {
                                        int endLine = this.TextSelectionPointer.EndLine;
                                        this.MoveCursorToLineItem(endLine);
                                        if (this.Caret != null)
                                        {
                                            this.CaretIndex = this.Caret.MoveToLocation(this.TextSelectionPointer.EndIndex);
                                        }
                                        else
                                        {
                                            this.ParentEditControl.Lines[endLine].SetCursorOnLoad = true;
                                            this.ParentEditControl.Lines[endLine].SetCursorIndex = this.TextSelectionPointer.EndIndex;
                                        }
                                        this.ClearSelection();
                                        return;
                                    }
                                }
                                if (this.Caret != null)
                                {
                                    this.CaretIndex = this.Caret.MoveTo(0);
                                }
                                else
                                {
                                    this.ParentEditControl.Lines[0].SetCursorOnLoad = true;
                                    this.ParentEditControl.Lines[0].SetCursorIndex = this.CaretIndex;
                                }
                            }

                            if (device.Modifiers == ModifierKeys.Shift)
                            {
                                this.UpdateSelectionPointer(this.selectionStartLine, this.ScrollRows.ScrollLineIndex + itemNumber, this.selectionStartIndex, this.CaretIndex);
                            }
                            else
                            {
                                this.ClearSelection();
                            }
                        }
                        else
                        {
                            this.ScrollToBottom();

                            if (this.TextSelectionPointer != null)
                            {
                                if (this.ScrollRows.LastBodyVisibleLineIndex >= this.TextSelectionPointer.EndLine && this.LineNumber < this.TextSelectionPointer.EndLine)
                                {
                                    int endLine = this.TextSelectionPointer.EndLine;
                                    this.MoveCursorToLineItem(endLine);
                                    if (this.Caret != null)
                                    {
                                        this.CaretIndex = this.Caret.MoveToLocation(this.TextSelectionPointer.EndIndex);
                                    }
                                    else
                                    {
                                        this.ParentEditControl.Lines[endLine].SetCursorOnLoad = true;
                                        this.ParentEditControl.Lines[endLine].SetCursorIndex = this.TextSelectionPointer.EndIndex;
                                    }
                                    this.ClearSelection();
                                    return;
                                }
                            }

                            this.MoveCursorToLineItem(this.parentControl.Lines.Count - 1);
                            if (this.Caret != null)
                            {
                                this.CaretIndex = this.Caret.MoveTo(0);
                            }
                            else
                            {
                                this.ParentEditControl.Lines[this.parentControl.Lines.Count - 1].SetCursorOnLoad = true;
                                this.ParentEditControl.Lines[this.parentControl.Lines.Count - 1].SetCursorIndex = this.CaretIndex;
                            }
                            if (device.Modifiers == ModifierKeys.Shift)
                            {
                                this.UpdateSelectionPointer(this.selectionStartLine, this.parentControl.Lines.Count - 1, this.selectionStartIndex, this.CaretIndex);
                            }
                            else
                            {
                                this.ClearSelection();
                            }
                        }
                    }

                    if (device.Modifiers == ModifierKeys.Control || device.Modifiers == (ModifierKeys.Control | ModifierKeys.Shift))
                    {
                        this.MoveCursorToLineItem(this.ScrollRows.LastBodyVisibleLineIndex - 1);

                        if (device.Modifiers == (ModifierKeys.Control | ModifierKeys.Shift))
                        {
                            this.UpdateSelectionPointer(this.selectionStartLine, this.ScrollRows.LastBodyVisibleLineIndex - 1, this.selectionStartIndex, this.CaretIndex);
                        }
                        else
                        {
                            this.ClearSelection();
                        }
                    }

                    e.Handled = true;
                    break;

                #endregion Page Down
            }
        }

        /// <summary>
        /// Releases the text selection and sets SelectedText property
        /// </summary>
        /// <param name="e">The <see cref="System.Windows.Input.KeyEventArgs"/> instance containing the event data.</param>
        internal void KeyUpPreview(KeyEventArgs e)
        {
            if (e.KeyboardDevice.Modifiers != ModifierKeys.Shift && isSelectionstarted)
            {
                isShiftPressed = false;
                this.ParentEditControl.SelectedText = GetSelectedText(TextSelectionPointer);
            }
        }

        /// <summary>
        /// Helper method to remove the text with in the selected area.
        /// </summary>
        /// <param name="selectionPointer">represents the text selection pointer</param>
        /// <returns>a value indicating the cursorindex</returns>
        internal int RemoveSelectedText(SelectionPointer selectionPointer)
        {
            int i = selectionPointer.EndLine;
            int startLine = selectionPointer.StartLine;
            string removeText = this.RemoveTextRange(selectionPointer);
            this.ParentEditControl.AddUndoManager = false;
            this.ParentEditControl.Lines[selectionPointer.StartLine].Text = removeText;
            this.ParentEditControl.Lines[selectionPointer.StartLine].SetCursorIndex = selectionPointer.StartIndex;
            this.ParentEditControl.Lines[selectionPointer.StartLine].SetCursorOnLoad = true;
            this.ClearSelection();
            this.ScrollRows.ScrollInView(startLine);
            this.MoveCursorToLineItem(startLine);
            this.ParentEditControl.isReinitializeLines = false;
            this.ParentEditControl.Text = this.ParentEditControl.GetText();
            this.ParentEditControl.isReinitializeLines = true;
            this.ParentEditControl.AddUndoManager = true;
            return this.ParentEditControl.Lines[startLine].SetCursorIndex;
        }

        /// <summary>
        /// Helper method to remove selected text
        /// </summary>
        /// <param name="pointer">The Selection pointer of Line.</param>
        /// <returns>Returns the text</returns>
        private string RemoveTextRange(SelectionPointer pointer)
        {
            int i = pointer.StartLine;
            int j = pointer.StartLine;
            string temp = string.Empty;
            bool removedCollapsedLines = false;
            var collapsedLines = this.ParentEditControl.Lines.Where(line => this.ParentEditControl.Lines.IndexOf(line) >= pointer.StartLine && this.ParentEditControl.Lines.IndexOf(line) <= pointer.EndLine && !line.IsExpanded);
            foreach (LineItem item in collapsedLines)
            {
                item.IsExpanded = true;
                UpdateExpandStatus(item.LineNumber, item.EndLine - 1, false);
                removedCollapsedLines = true;
                item.ContainsLines = false;
                item.EndLine = -1;
                item.StartLine = -1;
            }

            while (i <= pointer.EndLine)
            {
                if (i == pointer.StartLine)
                {
                    if (pointer.StartLine == pointer.EndLine)
                    {
                        if (pointer.EndIndex <= this.ParentEditControl.Lines[i].Text.Length)
                            temp = this.ParentEditControl.Lines[i].Text.Substring(0, pointer.StartIndex) + this.ParentEditControl.Lines[i].Text.Substring(pointer.EndIndex);
                        this.ParentEditControl.Lines[i].IsSelected = false;
                    }
                    else
                    {
                        this.parentControl.isAddingLinesCompleted = false;
                        this.ParentEditControl.Lines.CollectionChanged -= new System.Collections.Specialized.NotifyCollectionChangedEventHandler(Lines_CollectionChanged);
                        temp = this.ParentEditControl.Lines[i].Text.Substring(0, Math.Min(pointer.StartIndex, this.ParentEditControl.Lines[i].Text.Length));
                        this.ParentEditControl.Lines[i].IsSelected = false;
                    }

                    j++;
                }
                else
                {
                    if (i == pointer.EndLine)
                    {
                        this.ParentEditControl.isAddingLinesCompleted = true;
                        this.ParentEditControl.Lines.CollectionChanged += new System.Collections.Specialized.NotifyCollectionChangedEventHandler(Lines_CollectionChanged);
                        if (this.ParentEditControl.Lines.Count > j)
                            temp = temp + this.ParentEditControl.Lines[j].Text.Substring(Math.Min(pointer.EndIndex, this.ParentEditControl.Lines[j].Text.Length));
                        this.rowheights.RemoveLines(j, 1, null);
                        if (this.ParentEditControl.Lines.Count > j)
                            this.ParentEditControl.Lines.RemoveAt(j);
                    }
                    else
                    {
                        this.rowheights.RemoveLines(pointer.StartLine + 1, 1, null);
                        if (this.ParentEditControl.Lines.Count > pointer.StartLine + 1)
                            this.ParentEditControl.Lines.RemoveAt(pointer.StartLine + 1);
                    }
                }

                i++;
            }

            if (removedCollapsedLines)
            {
                this.ParentEditControl.Lines[pointer.StartLine].Text = temp;
                InvalidateVisual(true);
            }
            this.ParentEditControl.CurrentLanguage.ApplyExpandItems();
            return temp;
        }

        /// <summary>
        /// Helper method to Select all the lineitems in the panel.
        /// </summary>
        internal void ExecuteSelectAll()
        {
            if (this.ParentEditControl.Lines.Count > 0)
            {
                LineItem lastitem = ParentEditControl.Lines[ParentEditControl.Lines.Count - 1];
                UpdateSelectionPointer(0, ParentEditControl.Lines.Count - 1, 0, lastitem.Text.Length);
                this.ScrollToBottom();
                MoveCursorToLineItem(ParentEditControl.Lines.Count - 1);
                if (this.Caret != null)
                {
                    this.CaretIndex = this.Caret.MoveToEnd();
                }
                else
                {
                    lastitem.SetCursorOnLoad = true;
                    lastitem.SetCursorIndex = lastitem.Text.Length;
                }

                this.parentControl.SelectedText = GetSelectedText(this.TextSelectionPointer);
            }
        }

        /// <summary>
        /// Helper method to show tooltip when mouse is hovered on the collapsed area.
        /// </summary>
        /// <param name="element">represents on DependencyObject to which tooltip to be set.</param>
        /// <param name="pt">represents mouse' current position</param>
        internal void ShowToolTipWhenOverEllipsis(DependencyObject element, Point pt)
        {
            int index = this.ScrollRows.VisiblePointToLineIndex(pt.Y);
            var item = this.ParentEditControl.Lines[index];
            ToolTip tooltip = ToolTipService.GetToolTip(element) as ToolTip;
            if (!item.IsExpanded && CheckMouseOverEllipsis(pt) && tooltip == null)
            {
                this.Cursor = Cursors.Arrow;
                item.Cursor = Cursors.Arrow;
                TextBlock block = new TextBlock();
                block.TextWrapping = TextWrapping.Wrap;
                block.TextTrimming = TextTrimming.CharacterEllipsis;
                block.Text = this.ParentEditControl.GetTextRange(index, item.EndLine);
                tooltip = new ToolTip();
                tooltip.Content = block;
                ToolTipService.SetToolTip(element, tooltip);
                ToolTipService.SetIsEnabled(tooltip, true);
                ToolTipService.SetShowDuration(tooltip, 3);
                tooltip.IsOpen = true;
                item.MouseLeave += new MouseEventHandler(item_MouseLeave);
            }
            else if (!CheckMouseOverEllipsis(pt))
            {
                if (tooltip != null)
                {
                    tooltip.IsOpen = false;
                    ToolTipService.SetToolTip(element, null);
                    ToolTipService.SetIsEnabled(element, false);
                    this.Cursor = Cursors.IBeam;
                    item.Cursor = Cursors.IBeam;
                }
            }
        }

        private void item_MouseLeave(object sender, MouseEventArgs e)
        {
            LineItem element = sender as LineItem;
            ToolTip tooltip = ToolTipService.GetToolTip(element) as ToolTip;
            if (tooltip != null)
            {
                tooltip.IsOpen = false;
                ToolTipService.SetToolTip(element, null);
                ToolTipService.SetIsEnabled(element, false);
                this.Cursor = Cursors.IBeam;
                element.Cursor = Cursors.IBeam;
                element.MouseLeave -= new MouseEventHandler(item_MouseLeave);
            }
        }

        /// <summary>
        /// Helper method to check if the mouse position is over the Ellipsis.
        /// </summary>
        /// <param name="pt">represents the mouse position</param>
        /// <returns>whether the mouse position is inside or outside of the ellipsis area</returns>
        private bool CheckMouseOverEllipsis(Point pt)
        {
            int index = this.ScrollRows.VisiblePointToLineIndex(pt.Y);
            if (index < this.ParentEditControl.Lines.Count)
            {
                var item = this.ParentEditControl.Lines[index];
                double pointValue = pt.X - this.FixedWidth;
                string ellipsisText = this.ParentEditControl.CurrentLanguage.EllipsisText;
                if (item.ContainsPreprocessor)
                {
                    ellipsisText = item.PreprocessorText;
                }

                var ellipsiswidth = Utils.GetWidth(ellipsisText, this.ParentEditControl.FontFamily, this.ParentEditControl.FontSize, this.ParentEditControl.Foreground);
                Rect rect = new Rect(item.EllipsisPosition, new Size(ellipsiswidth, this.ParentEditControl.LineHeight));
                return rect.Contains(new Point(pointValue, 0));
            }
            return false;
        }

        /// <summary>
        /// Helper method to handle double click operation on a word.
        /// </summary>
        /// <param name="clickitem">represents the lineitem on which the double click was made</param>
        /// <param name="e">represents MouseEventArgs</param>
        private void HandleDoubleClick(LineItem clickitem, MouseButtonEventArgs e)
        {
            this.ClearSelection();
            int index = this.FindPosition(clickitem, e.GetPosition(this));
            WordDetails item = GetCurrentWord(index);
            if (item != null)
            {
                if (item != null && item.Text == " ")
                {
                    item = GetCurrentWord(index + 1);
                }
                if (item != null)
                {
                    if (this.Caret != null)
                        this.CaretIndex = this.Caret.MoveToLocation(item.StartIndex + item.Text.Length);
                    isSelectionstarted = true;
                    this.UpdateSelectionPointer(clickitem.LineNumber - 1, clickitem.LineNumber - 1, item.StartIndex, item.StartIndex + item.Text.Length);
                    this.ParentEditControl.SelectedText = item.Text;
                }
            }
        }

        internal void ExpandAllItems()
        {
            var items = from line in this.ParentEditControl.Lines
                        where line.ContainsLines && !line.IsExpanded
                        select line;

            foreach (LineItem item in items)
            {
                item.IsExpanded = true;
                this.UpdateExpandStatus(this.ParentEditControl.Lines.IndexOf(item), item.EndLine - 1, false);
                item.IsChildrenExpanded = true;
            }
            this.InvalidateVisual(true);
        }

        #endregion Implementation

        #region Overrides

        /// <summary>
        /// MouseLeftButtonDown preview event
        /// Sets cursor location after calculating the index and current lineitem
        /// Initiates selection
        /// </summary>
        /// <param name="e">The <see cref="T:System.Windows.Input.MouseButtonEventArgs"/> that contains the event data. The event data reports that the left mouse button was pressed.</param>
        /// <remarks>OnPreviewMouseLeftButtonDown fires when the Mouseleft button down. It update the selection pointer.</remarks>
        protected override void OnPreviewMouseLeftButtonDown(System.Windows.Input.MouseButtonEventArgs e)
        {
            if (this.Caret != null && this.Caret.Visibility == Visibility.Collapsed)
            {
                this.Caret.Visibility = Visibility.Visible;
            }
            if (this.ParentEditControl.isIntellisenseBoxOpen)
            {
                this.ParentEditControl.CurrentLanguage.HideIntellisensePopup();
            }
            Point mousehit = e.GetPosition(this);
            int index = this.ScrollColumns.VisiblePointToLineIndex(mousehit.X);
            if (index < 3)
            {
                Mouse.Capture(null);
                return;
            }

            LineItem clickitem = this.GetLineItem(mousehit);
            if (e.ClickCount == 2)
            {
                this.HandleDoubleClick(clickitem, e);
                return;
            }

            if (!clickitem.IsExpanded)
            {
                if (CheckMouseOverEllipsis(mousehit))
                {
                    if (!isEllipsisSelected)
                    {
                        this.MoveCursorToLineItem(index);
                        this.isSelectionstarted = true;
                        this.ParentEditControl.CurrentLanguage.UpdateCollapsedItemSelectionPointer(clickitem);
                        this.ParentEditControl.SelectedText = this.GetSelectedText(this.TextSelectionPointer);
                        this.MoveCursorToLineItem(clickitem.LineNumber - 1);
                        if (this.Caret != null)
                        {
                            this.CaretIndex = this.Caret.MoveToLocation(this.parentControl.CurrentLanguage.GetCollapsedItemSelectionEndIndex(clickitem, clickitem.Text.Length));
                        }
                        else
                        {
                            clickitem.SetCursorOnLoad = true;
                            clickitem.SetCursorIndex = this.parentControl.CurrentLanguage.GetCollapsedItemSelectionEndIndex(clickitem, clickitem.Text.Length);
                        }
                        isEllipsisSelected = true;
                        Mouse.Capture(null);
                        return;
                    }
                    else
                    {
                        isEllipsisSelected = false;
                        SetCursorLocation(clickitem, mousehit);
                    }
                }
                else
                {
                    SetCursorLocation(clickitem, mousehit);
                }
            }
            else
            {
                SetCursorLocation(clickitem, mousehit);
            }

            if (isShiftPressed)
            {
                UpdateSelectionPointer(this.selectionStartLine, this.LineNumber, this.selectionStartIndex, this.CaretIndex);
            }
            else
            {
                if (isSelectionstarted || this.ParentEditControl.SelectedText != string.Empty || (!isSelectionstarted && this.TextSelectionPointer != null))
                {
                    if (!this.ParentEditControl.AllowDragDrop)
                        ClearSelection();
                    isShiftPressed = false;
                    Mouse.Capture(null);
                    isSelectionstarted = true;
                    selectionStartLine = LineNumber;
                    selectionStartIndex = CaretIndex;
                }
                else
                {
                    isSelectionstarted = true;
                    selectionStartLine = LineNumber;
                    selectionStartIndex = CaretIndex;
                }
            }
            if (this.ParentEditControl.AllowDragDrop)
            {
                if (this.CaretIndex < clickitem.SelectionStartIndex || this.CaretIndex > clickitem.SelectionEndIndex)
                    ClearSelection();
            }
            if (this.ParentEditControl.AllowDragDrop && e.LeftButton == MouseButtonState.Pressed && this.ParentEditControl.SelectedText != "")
            {
                DragDrop.DoDragDrop(this.ParentEditControl, this.ParentEditControl.SelectedText, DragDropEffects.Copy | DragDropEffects.Move);
            }
            base.OnPreviewMouseLeftButtonDown(e);
        }

        /// <summary>
        /// MouseLeftButtonUp Preview event override, updates selection and selected text when the mouse is released.
        /// </summary>
        /// <param name="e">The <see cref="T:System.Windows.Input.MouseButtonEventArgs"/> that contains the event data. The event data reports that the left mouse button was released.</param>
        protected override void OnPreviewMouseLeftButtonUp(MouseButtonEventArgs e)
        {
            Point mousehit = e.GetPosition(this);
            int index = this.ScrollColumns.VisiblePointToLineIndex(mousehit.X);
            if (index < 3)
            {
                base.OnPreviewMouseLeftButtonUp(e);
                this.ParentEditControl.SelectedText = GetSelectedText(TextSelectionPointer);
                isSelectionstarted = false;
                isShiftPressed = false;
                Mouse.Capture(null);
                return;
            }

            if (!isShiftPressed && !isSelectionstarted)
            {
                if (TextSelectionPointer != null)
                {
                    ClearSelection();
                }
            }
            else
            {
                this.ParentEditControl.SelectedText = GetSelectedText(TextSelectionPointer);
                isSelectionstarted = false;
                isShiftPressed = false;
            }

            Mouse.Capture(null);
            base.OnPreviewMouseLeftButtonUp(e);
        }

        /// <summary>
        ///
        /// </summary>
        /// <param name="e"></param>
        protected override void OnDragOver(DragEventArgs e)
        {
            IsDragging = true;
            Point pt = e.GetPosition(this);
            LineItem item = this.GetLineItem(pt);
            pt = e.GetPosition(this);
            SetCursorLocation(item, pt);
            base.OnDragOver(e);
        }

        /// <summary>
        /// MouseMove event override - updates selection, updates scrollbar offset when the mouse is moved out of the viewing area
        /// </summary>
        /// <param name="e">The <see cref="T:System.Windows.Input.MouseEventArgs"/> that contains the event data.</param>
        /// <remarks>Each mouse move event the control update the selection pointer.</remarks>
        protected override void OnMouseMove(MouseEventArgs e)
        {
            Point pt = e.GetPosition(this);
            var index = this.ScrollColumns.VisiblePointToLineIndex(pt.X);
            if (index < 3 && !(isSelectionstarted && e.LeftButton == MouseButtonState.Pressed))
            {
                this.Cursor = Cursors.Arrow;
                return;
            }
            else if (this.ParentEditControl.SelectedText != "" && this.ParentEditControl.AllowDragDrop)
            {
                this.Cursor = Cursors.Arrow;
            }
            else
            {
                this.Cursor = Cursors.IBeam;
            }

            if (isSelectionstarted && e.LeftButton == MouseButtonState.Pressed && this.ParentEditControl.SelectedText == "")
            {
                LineItem item = this.GetLineItem(pt);
                if (item != null)
                {
                    var lineNumber = item.LineNumber - 1;
                    if (lineNumber < 0)
                    {
                        lineNumber = this.ParentEditControl.Lines.IndexOf(item);
                    }

                    this.ScrollRows.ScrollInView(lineNumber);

                    if (lineNumber > this.ScrollRows.LastBodyVisibleLineIndex)
                    {
                        this.LineDown();
                    }
                    else if (lineNumber < this.ScrollRows.StartLineIndex)
                    {
                        this.LineUp();
                    }

                    pt = e.GetPosition(this);

                    SetCursorLocation(item, pt);
                    if (!item.IsExpanded)
                    {
                        if (this.ParentEditControl.CursorIndex == this.ParentEditControl.CurrentLanguage.GetCollapsedItemText(item).Length)
                        {
                            UpdateSelectionPointer(selectionStartLine, item.EndLine - 1, selectionStartIndex, CaretIndex);
                        }
                        else
                        {
                            UpdateSelectionPointer(selectionStartLine, lineNumber, selectionStartIndex, CaretIndex);
                        }
                    }
                    else
                    {
                        UpdateSelectionPointer(selectionStartLine, lineNumber, selectionStartIndex, CaretIndex);
                    }

                    Mouse.Capture(this, CaptureMode.Element);
                }
                else
                {
                    if (TextSelectionPointer != null)
                    {
                        int endline = TextSelectionPointer.EndLine;
                        int endindex = TextSelectionPointer.EndIndex;
                        if (ParentEditControl.Lines[endline].Text.Length - 1 > endindex)
                        {
                            if (Caret != null)
                            {
                                Caret.MoveToEnd();
                            }
                            else
                            {
                                MoveCursorToLineItem(endline - 1);
                            }
                            UpdateSelectionPointer(selectionStartLine, LineNumber, selectionStartIndex, ParentEditControl.Lines[endline].Text.Length);
                        }
                    }
                }
            }
            else
            {
                index = this.ScrollColumns.VisiblePointToLineIndex(pt.X);
                if (index < 3)
                {
                    this.Cursor = Cursors.Arrow;
                }
                else
                {
                    index = this.ScrollRows.VisiblePointToLineIndex(pt.Y);
                    if (index < this.ParentEditControl.Lines.Count)
                    {
                        this.ShowToolTipWhenOverEllipsis(this.ParentEditControl.Lines[index], e.GetPosition(this));
                    }
                }
            }
        }

        /// <summary>
        /// OnGotFocus - updates the visibility of the cursor to visible.
        /// </summary>
        /// <param name="e">represents the RoutedEventArgs</param>
        protected override void OnGotFocus(RoutedEventArgs e)
        {
            base.OnGotFocus(e);
        }

        /// <summary>
        /// OnGotFocus - updates the visibility of the cursor to visible.
        /// </summary>
        /// <param name="e">represents the RoutedEventArgs</param>
        protected override void OnLostFocus(RoutedEventArgs e)
        {
            base.OnLostFocus(e);
        }

        #endregion Overrides

        #region Events

        /// <summary>
        /// Updates the EditScrollControl values when an item is added or removed from the Lines Collection.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.Collections.Specialized.NotifyCollectionChangedEventArgs"/> instance containing the event data.</param>
        private void Lines_CollectionChanged(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
        {
            if (this.parentControl.isAddingLinesCompleted)
            {
                try
                {
                    rowheights.LineCount = parentControl.Lines.Count;
                    if (parentControl.Lines.Count > 0)
                    {
                        parentControl.CurrentLanguage.CalculatePreferredWidth();// Math.Max(parentControl.Lines.Max(line => line.TextWidth) + 10, parentControl.PreferredWidth);
                    }
                }
                catch
                {
                }
                columnwidths[4] = parentControl.PreferredWidth;
            }
            //if (this.parentControl.EnableOutlining)
            //{
            //    this.UpdateExpandStatus(this.ParentEditControl.LineNumber, rowheights.LineCount-1, false);
            //}
        }

        /// <summary>
        /// Event gets fired when the SelectionPointer property gets updated and updates selection accordingly
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="args">The <see cref="Syncfusion.Windows.Edit.SelectionPointerChangedEventArgs"/> instance containing the event data.</param>
        /// <remarks>TextSelectionPointer_SelectionPointerChanged fires when the Selection Pointer changed in Edit control.</remarks>
        private void TextSelectionPointer_SelectionPointerChanged(object sender, SelectionPointerChangedEventArgs args)
        {
            SelectionPointer ptr = (SelectionPointer)sender;

            if (ptr == null)
            {
                return;
            }

            //if ((args.OldValue == args.NewValue) && (args.PropertyName == "StartLine" || args.PropertyName == "EndLine"))
            //{
            //    return;
            //}

            switch (args.PropertyName)
            {
                case "StartLine":
                    if (args.NewValue > args.OldValue)
                    {
                        if (args.NewValue >= 0)
                        {
                            for (int i = args.OldValue; i < args.NewValue; i++)
                            {
                                RemoveSelectionLayer(i);
                            }
                        }
                        UpdateSelection(ptr, args.NewValue);
                    }
                    else
                    {
                        for (int i = args.NewValue; i <= args.OldValue; i++)
                        {
                            UpdateSelection(ptr, i);
                        }
                    }

                    break;

                case "EndLine":
                    if (args.NewValue < args.OldValue)
                    {
                        if (args.OldValue >= 0)
                        {
                            for (int i = args.OldValue; i > args.NewValue; i--)
                            {
                                if (i < ParentEditControl.Lines.Count)
                                {
                                    RemoveSelectionLayer(i);
                                }
                            }
                        }

                        UpdateSelection(ptr, args.NewValue);
                    }
                    else
                    {
                        for (int i = args.OldValue; i <= args.NewValue; i++)
                        {
                            UpdateSelection(ptr, i);
                        }
                    }

                    break;

                case "StartIndex":

                    if (ptr.EndIndex > args.NewValue)
                    {
                        UpdateSelection(ptr, ptr.StartLine);
                    }

                    break;

                case "EndIndex":
                    UpdateSelection(ptr, ptr.EndLine);
                    break;
            }

            if (this.ParentEditControl.SearchResults != null && !this.ParentEditControl.SearchResults.IsSelectionChanged)
            {
                if (this.TextSelectionPointer.StartIndex >= 0 && this.TextSelectionPointer.StartLine >= 0 && this.TextSelectionPointer.EndLine >= 0 && this.TextSelectionPointer.EndIndex >= 0)
                {
                    this.ParentEditControl.SearchResults.EditTextSelection = new SelectionPointer()
                    {
                        StartIndex = this.TextSelectionPointer.StartIndex,
                        StartLine = this.TextSelectionPointer.StartLine,
                        EndLine = this.TextSelectionPointer.EndLine,
                        EndIndex = this.TextSelectionPointer.EndIndex
                    };
                }
            }
        }

        /// <summary>
        /// Click Event handler for Toggle button to expand and collapse the item.
        /// </summary>
        /// <param name="sender">represents the toggle button initiating the click the event</param>
        /// <param name="e">represents the EventArgs</param>
        private void content_Click(object sender, RoutedEventArgs e)
        {
            ToggleButton button = sender as ToggleButton;
            LineItem item = button.Tag as LineItem;
            int temp = this.ScrollRows.ScrollLineIndex;

            if (button.IsChecked == true)
            {
                item.IsChildrenExpanded = true;
                this.UpdateExpandStatus(item.LineNumber, item.EndLine - 1, false);
            }
            else
            {
                this.UpdateExpandStatus(item.LineNumber, item.EndLine - 1, true);
                item.IsChildrenExpanded = false;
            }

            this.InvalidateVisual(true);
            item.UpdateCursorPosition();
        }

        /// <summary>
        /// SelectionLayar mousemove event to show tooltip when hovered on a collapsed area ellipsis
        /// </summary>
        /// <param name="sender">represents the Selection Layer</param>
        /// <param name="e">represents the MouseEventArgs</param>
        private void sellayer_MouseMove(object sender, MouseEventArgs e)
        {
            //ShowToolTipWhenOverEllipsis(sender as SelectionLayer, e.GetPosition(this));
        }

        #endregion Events
    }
}