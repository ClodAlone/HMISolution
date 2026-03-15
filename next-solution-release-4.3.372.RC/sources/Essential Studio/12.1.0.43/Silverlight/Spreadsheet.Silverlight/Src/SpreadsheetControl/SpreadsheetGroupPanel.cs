#region Copyright Syncfusion Inc. 2001 - 2014
// Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Any infringement will be prosecuted under
// applicable laws. 
#endregion
using Syncfusion.Windows.Controls.Grid;
using Syncfusion.Windows.Controls.Scroll;
using Syncfusion.Windows.Controls.Spreadsheet.Commands;
using Syncfusion.XlsIO;
using Syncfusion.XlsIO.Implementation;
using Syncfusion.XlsIO.Parser.Biff_Records;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Media;
using System.ComponentModel;

#if SILVERLIGHT
using System.Windows.Shapes;
#endif

namespace Syncfusion.Windows.Controls.Spreadsheet
{
   [DesignTimeVisible(false)]
   public class SpreadsheetGroupPanel:Panel
    {
        private SpreadsheetControl control;
        private bool isSummaryRowBelow;
        private bool isSummaryColumnRight;
        private string orientation;
        double headerRowHeight;
        double headerColumnWidth;

        #region Constructor   
     
        public SpreadsheetGroupPanel()
        { 
        }

        public SpreadsheetGroupPanel(string panelOrientation)
        {
            this.Orientation = panelOrientation;
        }

        #endregion

        public SpreadsheetControl Control
        {
            get{return this.control;}
            set
            {
                this.control = value;
                this.Control.GridProperties.PropertyChanged += GridProperties_PropertyChanged;               
            }
        }

        void GridProperties_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            if (e.PropertyName == "SpreadsheetGrid")
            {
                if (this.Control.GridProperties.ActiveSpreadsheetGrid != null)
                {
                    this.Control.GridProperties.ActiveSpreadsheetGrid.ScrollRows.Changed += ScrollRows_Changed;
                    this.Control.GridProperties.ActiveSpreadsheetGrid.ScrollColumns.Changed += ScrollColumns_Changed;
                }
            }
        }

       /// <summary>
       /// A boolean property that determines whether the SummaryRow (Expander) is above the group or below group.
       /// </summary>
        bool IsSummaryRowBelow
        {
            get{ return this.isSummaryRowBelow; }
            set{ this.isSummaryRowBelow = value; }
        }

       /// <summary>
       /// Boolean property that determines whether the SummaryColumn (i.e Expander) is Right of the Details or Left of the Details.
       /// </summary>
        bool IsSummaryColumnRight
        {
            get{ return this.isSummaryColumnRight; }
            set{ this.isSummaryColumnRight = value; }
        }

       /// <summary>
       /// Boolean property that determines whether the panel is RowGroupPanel or ColumnGroupPanel.
       /// </summary>
        public string Orientation
        {
            get{ return this.orientation; }
            set{ this.orientation = value; }
        }      

       //Invalidating the ColumnGroupPanel when there is a change in ScrollColumns.
        void ScrollColumns_Changed(object sender, EventArgs e)
        {
            if (this.Orientation == "Horizontal")
            {
                this.InvalidateMeasure();
#if !SILVERLIGHT
                this.InvalidateVisual();                
#endif
            }
        }

       //Invalidating the RowGroupPanel when there is a change in ScrollRows.
        void ScrollRows_Changed(object sender, EventArgs e)
        {
            if (this.Orientation == "Vertical")
            {
                this.InvalidateMeasure();
#if !SILVERLIGHT
                this.InvalidateVisual();                
#endif
            }
        }

       //Collections used for Recycling.
        List<UIElement>UnloadUiElements;
        Dictionary<int, UIElement> UnloadHeaderElements;

       //List used to store all OutlineWrappers from WorkSheet.
        List<IOutlineWrapper> OutlineWrappers = new List<IOutlineWrapper>();

       //Dictionary which holds the information about grid RangeRect for the corresponding group with the outlinelevel.
       //Horizontal and Vertical plain lines are drawn based on this.
        Dictionary<Rect,int> RenderLineInfo = new Dictionary<Rect,int>();

       //Dictionary which holds the GridRangeInfo and Outline level of all visible groups.
       //used to specify where the dots should be placed in the panel.
        Dictionary<GridRangeInfo, int>GroupLineInfo = new Dictionary<GridRangeInfo, int>();

       //Dictionary which holds the information about row index of the dot along with the outline level.
        Dictionary<int, int> DotInfo = new Dictionary<int, int>();

        List<int> RowButtonsList=new List<int>();
                
       /// <summary>
       /// In this method we are adding the SpreadsheetGroup Buttons for the Visible OutlineWrappers. 
       /// </summary>
       /// <param name="availableSize">Available size of panel </param>
       /// <returns>availableSize</returns>
        protected override Size MeasureOverride(Size availableSize)
        {
            this.PreArrange();
#if SILVERLIGHT
            this.Children.Clear();
#endif            
            if ((this.Control as SpreadsheetControl).ExcelProperties.WorkBook.ActiveSheet!=null)
                OutlineWrappers = ((this.Control as SpreadsheetControl).ExcelProperties.WorkBook.ActiveSheet as WorksheetImpl).OutlineWrappers;

            if (OutlineWrappers != null && OutlineWrappers.Count>0)
            {
                headerRowHeight = this.Control.GridProperties.CurrentExcelGridModel.RowHeights[0];
                headerColumnWidth = this.Control.GridProperties.CurrentExcelGridModel.ColumnWidths[0];
                IsSummaryRowBelow = this.Control.ExcelProperties.WorkBook.ActiveSheet.PageSetup.IsSummaryRowBelow;
                IsSummaryColumnRight = this.Control.ExcelProperties.WorkBook.ActiveSheet.PageSetup.IsSummaryColumnRight;

                RowButtonsList.Clear();
                RenderLineInfo.Clear();
                GroupLineInfo.Clear(); 

                if (this.Orientation == "Vertical")
                {
                    foreach (OutlineWrapper outlineWrapper in OutlineWrappers)
                    {
                        if (outlineWrapper.GroupBy == ExcelGroupBy.ByRows)
                        {
                            GridRangeInfo groupRange = outlineWrapper.OutlineRange.ConvertExcelRangeToGridRange();
                            groupRange = GridRangeInfo.Rows(groupRange.Top, groupRange.Bottom);
                            double AdjacentRowHeight;
                            double buttonHeight;
                            bool isButtonVisible = false;

                            //Calculating the height of the button based on the adjacent row height of the grouped range.
                            if (IsSummaryRowBelow)
                            {
                                AdjacentRowHeight = this.Control.GridProperties.ActiveSpreadsheetGrid.RowHeights[groupRange.Bottom + 1];
                                buttonHeight = AdjacentRowHeight > 20 ? 20 : AdjacentRowHeight;
                            }
                            else
                            {
                                if (groupRange.Top == 1)
                                {
                                    buttonHeight = 0;
                                }
                                else
                                {
                                    AdjacentRowHeight = this.Control.GridProperties.ActiveSpreadsheetGrid.RowHeights[groupRange.Top - 1];
                                    buttonHeight = AdjacentRowHeight > 20 ? 20 : AdjacentRowHeight;
                                }
                            }

                            Rect rangeRect = this.Control.GridProperties.ActiveSpreadsheetGrid.RangeToRect(ScrollAxisRegion.Body, ScrollAxisRegion.Body, groupRange, true, true);

                            double x = outlineWrapper.OutlineLevel * 20 - 20;
                            double y = IsSummaryRowBelow ? rangeRect.Bottom : rangeRect.Top;

                            //calculating if the button is visible or not, if visible the row index is added to the RowButtonList.
                            if (IsSummaryRowBelow)
                            {
                                //Here am Subtracting 40 from FinalSize , because The size of the TabItem, Scrollbar is 40.
                                if (y >= headerRowHeight && !RowButtonsList.Contains(groupRange.Bottom + 1))
                                {
                                    if (y < (availableSize.Height - buttonHeight - 40))
                                    {
                                        RowButtonsList.Add(groupRange.Bottom + 1);
                                        isButtonVisible = true;
                                    }

                                    //below code adds RangeRect along with outline level to the RenderLineInfo dictionary.
                                    if (!outlineWrapper.Outline.IsHidden && rangeRect.Top < availableSize.Height - 40 && rangeRect.Bottom > headerRowHeight && !RenderLineInfo.ContainsKey(rangeRect))
                                        RenderLineInfo.Add(rangeRect, outlineWrapper.OutlineLevel);
                                }
                            }
                            else
                            {
                                if (y <= availableSize.Height - 40 && !RowButtonsList.Contains(groupRange.Top - 1))
                                {
                                    if (y >= (headerRowHeight + buttonHeight))
                                    {
                                        RowButtonsList.Add(groupRange.Top - 1);
                                        isButtonVisible = true;
                                    }

                                    if (!outlineWrapper.Outline.IsHidden && rangeRect.Top < availableSize.Height - 40 && rangeRect.Bottom > headerRowHeight && !RenderLineInfo.ContainsKey(rangeRect))
                                        RenderLineInfo.Add(rangeRect, outlineWrapper.OutlineLevel);
                                }
                            }

                            //Below code adds the GridRangeInfo to the GroupLineInfo dictionary along with the outline level.
                            //Which is used in OnRender Method to decide where the dots should be placed .
                            if (!outlineWrapper.Outline.IsHidden && rangeRect.Top < availableSize.Height - 40 && rangeRect.Bottom > headerRowHeight && !GroupLineInfo.ContainsKey(groupRange))
                                GroupLineInfo.Add(groupRange, outlineWrapper.OutlineLevel);

                            if (isButtonVisible)
                            {
                                if (UnloadUiElements.Count>0)
                                {
                                    SpreadsheetGroupButton button = UnloadUiElements[0] as SpreadsheetGroupButton;
                                    button.Range = groupRange;
                                    button.IsCollapsed = outlineWrapper.Outline.IsHidden;
                                    button.OutlineLevel = outlineWrapper.OutlineLevel;
                                    button.IsChecked = !outlineWrapper.Outline.IsHidden;
                                    button.Command = new GroupRowExpandCommand(button, this.Control.GridProperties.CurrentExcelGridModel);
#if SILVERLIGHT
                                    this.Children.Add(button);
#endif
                                    UnloadUiElements.RemoveAt(0);
                                }
                                else
                                {
                                    SpreadsheetGroupButton button = new SpreadsheetGroupButton(groupRange, outlineWrapper.OutlineLevel,outlineWrapper.Outline.IsHidden);
                                    button.Margin = new Thickness(1, 0, 0, 0);
                                    button.IsChecked = !outlineWrapper.Outline.IsHidden;
                                    button.Command = new GroupRowExpandCommand(button, this.Control.GridProperties.CurrentExcelGridModel);
                                    this.Children.Add(button);                                    
                                }
                            }                                                        
                        }
                    }

#if SILVERLIGHT
                    this.AddGroupLines();
#endif

                    if (this.Control.OutlineRowCount > 0)
                    {
                        //Adding the outline label buttons for the panel.
                        for (int i = 1; i <= this.Control.OutlineRowCount + 1; i++)
                        {
                            if (UnloadHeaderElements.ContainsKey(i))
                            {
#if SILVERLIGHT
                            this.Children.Add(UnloadHeaderElements[i] as Button);
#endif
                                UnloadHeaderElements.Remove(i);
                            }
                            else
                            {
                                Button btn = new Button();
                                btn.Width = 20;
                                btn.Height = 20;
                                btn.Content = i.ToString();
                                btn.Tag = i;
                                btn.Command = new CollapseAllGroupCommand(i, ExcelGroupBy.ByRows, this.Control.GridProperties.CurrentExcelGridModel,
                                    (this.Control as SpreadsheetControl).ExcelProperties.WorkBook.ActiveSheet);
                                this.Children.Add(btn);
                            }
                        }
                    }
                }
                else
                {
                    foreach (OutlineWrapper outlineWrapper in OutlineWrappers)
                    {
                        if (outlineWrapper.GroupBy == ExcelGroupBy.ByColumns)
                        {
                            GridRangeInfo groupRange = outlineWrapper.OutlineRange.ConvertExcelRangeToGridRange();
                            groupRange = GridRangeInfo.Cols(groupRange.Left, groupRange.Right);
                            double AdjacentColumnWidth;
                            double buttonWidth;
                            bool isButtonVisible = false;

                            if (isSummaryColumnRight)
                            {
                                AdjacentColumnWidth = this.Control.GridProperties.ActiveSpreadsheetGrid.ColumnWidths[groupRange.Right + 1];
                                buttonWidth = AdjacentColumnWidth > 20 ? 20 : AdjacentColumnWidth;
                            }
                            else
                            {
                                if (groupRange.Left == 1)
                                {
                                    buttonWidth = 0;
                                }
                                else
                                {
                                    AdjacentColumnWidth = this.Control.GridProperties.ActiveSpreadsheetGrid.ColumnWidths[groupRange.Left - 1];
                                    buttonWidth = AdjacentColumnWidth > 20 ? 20 : AdjacentColumnWidth;
                                }
                            }

                            Rect rangeRect = this.Control.GridProperties.ActiveSpreadsheetGrid.RangeToRect(ScrollAxisRegion.Body, ScrollAxisRegion.Body, groupRange, true, true);

                            double y = outlineWrapper.OutlineLevel * 20 - 20;
                            double x = IsSummaryColumnRight ? rangeRect.Right : rangeRect.Left;

                            if (IsSummaryColumnRight)
                            {
                                //Here am Subtracting 20 from FinalSize , because The size of the TabItem, Scrollbar is 20.
                                if (x >= headerColumnWidth && !RowButtonsList.Contains(groupRange.Right + 1))
                                {
                                    if (x < (availableSize.Width - buttonWidth - 20))
                                    {
                                        RowButtonsList.Add(groupRange.Right + 1);
                                        isButtonVisible = true;
                                    }

                                    if (!outlineWrapper.Outline.IsHidden && rangeRect.Left < availableSize.Width - 20 && rangeRect.Right > headerColumnWidth && !RenderLineInfo.ContainsKey(rangeRect))
                                        RenderLineInfo.Add(rangeRect, outlineWrapper.OutlineLevel);
                                }
                            }
                            else
                            {
                                if (x <= availableSize.Width - 20 && !RowButtonsList.Contains(groupRange.Left - 1))
                                {
                                    if (x >= (headerColumnWidth + buttonWidth))
                                    {
                                        RowButtonsList.Add(groupRange.Left - 1);
                                        isButtonVisible = true;
                                    }

                                    if (!outlineWrapper.Outline.IsHidden && rangeRect.Left < availableSize.Width - 20 && rangeRect.Right > headerColumnWidth && !RenderLineInfo.ContainsKey(rangeRect))
                                        RenderLineInfo.Add(rangeRect, outlineWrapper.OutlineLevel);
                                }
                            }

                            if (!outlineWrapper.Outline.IsHidden && rangeRect.Left < availableSize.Width - 20 && rangeRect.Right > headerColumnWidth && !GroupLineInfo.ContainsKey(groupRange))
                                GroupLineInfo.Add(groupRange, outlineWrapper.Outline.OutlineLevel);

                            if (isButtonVisible)
                            {
                                if (UnloadUiElements.Count > 0)
                                {
                                    SpreadsheetGroupButton button = UnloadUiElements[0] as SpreadsheetGroupButton;
                                    button.Range = groupRange;
                                    button.IsCollapsed = outlineWrapper.Outline.IsHidden;
                                    button.OutlineLevel = outlineWrapper.OutlineLevel;
                                    button.IsChecked = !outlineWrapper.Outline.IsHidden;
                                    button.Command = new GroupColumnExpandCommand(button, this.Control.GridProperties.CurrentExcelGridModel);
#if SILVERLIGHT
                                    this.Children.Add(button);
#endif
                                    UnloadUiElements.RemoveAt(0);
                                }
                                else
                                {
                                    SpreadsheetGroupButton button = new SpreadsheetGroupButton(groupRange, outlineWrapper.OutlineLevel,outlineWrapper.Outline.IsHidden);
                                    button.IsChecked = !outlineWrapper.Outline.IsHidden;
                                    button.Command = new GroupColumnExpandCommand(button, this.Control.GridProperties.CurrentExcelGridModel);
                                    this.Children.Add(button);
                                }
                            }                            
                        }
                    }

#if SILVERLIGHT
                    this.AddGroupLines();
#endif

                    if (this.Control.OutlineColumnCount > 0)
                    {
                        //Adding the outline label buttons for the panel.
                        for (int i = 1; i <= this.Control.OutlineColumnCount + 1; i++)
                        {
                            if (UnloadHeaderElements.ContainsKey(i))
                            {
#if SILVERLIGHT
                            this.Children.Add(UnloadHeaderElements[i] as Button);
#endif
                                UnloadHeaderElements.Remove(i);
                            }
                            else
                            {
                                Button btn = new Button();
                                btn.Width = 20;
                                btn.Height = 20;
                                btn.Content = i.ToString();
                                btn.Tag = i;
                                btn.Command = new CollapseAllGroupCommand(i, ExcelGroupBy.ByColumns, this.Control.GridProperties.CurrentExcelGridModel,
                                    (this.Control as SpreadsheetControl).ExcelProperties.WorkBook.ActiveSheet);
                                this.Children.Add(btn);
                            }
                        }
                    }
                }
            }

            this.PostArrange();
            return base.MeasureOverride(availableSize);
        }

        protected override Size ArrangeOverride(Size finalSize)
        {                                               
            if (this.Control.ExcelProperties.WorkBook.ActiveSheet != null)
            {                         
                UIElementCollection elements = base.Children;                               
                
                if (this.Orientation == "Vertical")
                {
                    foreach (UIElement el in elements)
                    {
                        if (el is SpreadsheetGroupButton)
                        {
                            SpreadsheetGroupButton button = el as SpreadsheetGroupButton;                                                                                  
                            Rect rangeRect = this.Control.GridProperties.ActiveSpreadsheetGrid.RangeToRect(ScrollAxisRegion.Body, ScrollAxisRegion.Body, button.Range, true, true);

                            double AdjacentRowHeight;  
                            double x = button.OutlineLevel * 20 - 20;
                            double y = IsSummaryRowBelow ? rangeRect.Bottom : rangeRect.Top;

                            if (IsSummaryRowBelow)
                            {
                                AdjacentRowHeight = this.Control.GridProperties.ActiveSpreadsheetGrid.RowHeights[button.Range.Bottom + 1];
                                button.Height = AdjacentRowHeight > 20 ? 20 : AdjacentRowHeight;
                            }
                            else
                            {
                                if (button.Range.Top == 1)
                                {
                                    button.Height = 0;
                                }
                                else
                                {
                                    AdjacentRowHeight = this.Control.GridProperties.ActiveSpreadsheetGrid.RowHeights[button.Range.Top - 1];
                                    button.Height = AdjacentRowHeight > 20 ? 20 : AdjacentRowHeight;
                                }
                            }

                            if (IsSummaryRowBelow)                                                                                               
                                el.Arrange(new Rect(x, y, 21, button.Height));                                                                                                                                
                            else                                                            
                                el.Arrange(new Rect(x, y - button.Height, 21, button.Height));                                                                                                                                                                
                        }
#if SILVERLIGHT
                        else if (el is Line)
                        {
                            Line line = el as Line;
                            el.Arrange(new Rect(new Point(0,0),new Point(finalSize.Width,finalSize.Height)));
                        }
#endif
                        else if(el is Button)
                        {
                            //ToDo : Need to calculate the Width and Height of the Rect.
                            Button btn = el as Button;
                            double horX = (int)btn.Tag * 20 - 20;
                            double horY = headerRowHeight > 20 ? (headerRowHeight / 2) - 10 : 0;
                            btn.Arrange(new Rect(horX, horY, btn.Width,btn.Height));                            
                        }                                               
                    }                    
                }
                else
                {
                    foreach (UIElement el in elements)
                    {
                        if (el is SpreadsheetGroupButton)
                        {
                            SpreadsheetGroupButton button = el as SpreadsheetGroupButton;                            
                            double AdjacentColumnWidth;                           
                            Rect rangeRect = this.Control.GridProperties.ActiveSpreadsheetGrid.RangeToRect(ScrollAxisRegion.Body, ScrollAxisRegion.Body, button.Range, true, true);

                            double y = button.OutlineLevel * 20 - 20;
                            double x = IsSummaryColumnRight ? rangeRect.Right : rangeRect.Left;

                            if (isSummaryColumnRight)
                            {
                                AdjacentColumnWidth = this.Control.GridProperties.ActiveSpreadsheetGrid.ColumnWidths[button.Range.Right + 1];
                                button.Width = AdjacentColumnWidth > 20 ? 20 : AdjacentColumnWidth;
                            }
                            else
                            {
                                if (button.Range.Left == 1)
                                {
                                    button.Width = 0;
                                }
                                else
                                {
                                    AdjacentColumnWidth = this.Control.GridProperties.ActiveSpreadsheetGrid.ColumnWidths[button.Range.Left - 1];
                                    button.Width = AdjacentColumnWidth > 20 ? 20 : AdjacentColumnWidth;
                                }
                            }

                            if (IsSummaryColumnRight)                                                                                                    
                                el.Arrange(new Rect(x, y, button.Width, 20));                          
                            else                            
                                el.Arrange(new Rect(x - button.Width, y, button.Width, 20));                                                        
                        }
#if SILVERLIGHT
                        else if (el is Line)
                        {
                            Line line = el as Line;
                            el.Arrange(new Rect(new Point(0, 0), new Point(finalSize.Width, finalSize.Height)));
                        }
#endif
                        else if(el is Button)
                        {
                            //ToDo : Need to calculate the Width and Height of the Rect.
                            Button btn = el as Button;
                            double horX = headerColumnWidth > 20 ? (headerColumnWidth / 2) - 10 : 0;
                            double horY = (int)btn.Tag * 20 - 20;
                            btn.Arrange(new Rect(horX, horY, btn.Width,btn.Height));
                        }
                    }
                }
            }
            return base.ArrangeOverride(finalSize);
        }

#if SILVERLIGHT
        internal void AddGroupLines()
        {
            DotInfo.Clear();

            if (this.Orientation == "Vertical")
            {
                foreach (var rangeItem in GroupLineInfo)
                {
                    for (int i = rangeItem.Key.Top; i <= rangeItem.Key.Bottom; i++)
                    {
                        if (!DotInfo.ContainsKey(i))
                            DotInfo.Add(i, rangeItem.Value);
                        else
                            DotInfo[i] = DotInfo[i] > rangeItem.Value ? DotInfo[i] : rangeItem.Value;
                    }
                }

                //Drawing the GroupLines.
                foreach (var item in RenderLineInfo)
                {
                    double x = item.Value * 20 - 10;
                    double startPoint = item.Key.Top < headerRowHeight ? headerRowHeight : (item.Key.Top > this.ActualHeight - 40 ? this.ActualHeight - 40 : item.Key.Top);
                    double endPoint = item.Key.Bottom < headerRowHeight ? headerRowHeight : (item.Key.Bottom > this.ActualHeight - 40 ? this.ActualHeight - 40 : item.Key.Bottom);

                    startPoint = IsSummaryRowBelow ? startPoint + 2 : startPoint;
                    endPoint = IsSummaryRowBelow ? endPoint : endPoint - 2;

                    Line groupLine = new Line();
                    groupLine.Stroke = new SolidColorBrush(Colors.Black);
                    groupLine.StrokeThickness = 2;                    
                    groupLine.X1 = x;
                    groupLine.Y1 = startPoint;
                    groupLine.X2 = x;
                    groupLine.Y2 = endPoint;

                    this.Children.Add(groupLine);                    

                    if (IsSummaryRowBelow && item.Key.Top >= headerRowHeight)
                    {
                        Line smallLine = new Line();
                        smallLine.Stroke = new SolidColorBrush(Colors.Black);
                        smallLine.StrokeThickness = 2; 
                        smallLine.X1 = x;
                        smallLine.Y1 = startPoint;
                        smallLine.X2 = x + 8;
                        smallLine.Y2 = startPoint;

                        this.Children.Add(smallLine);                        
                    }
                    else if (!IsSummaryRowBelow && item.Key.Bottom <= this.ActualHeight - 40)
                    {
                        Line smallLine = new Line();
                        smallLine.Stroke = new SolidColorBrush(Colors.Black);
                        smallLine.StrokeThickness = 2; 
                        smallLine.X1 = x;
                        smallLine.Y1 = endPoint;
                        smallLine.X2 = x + 8;
                        smallLine.Y2 = endPoint;

                        this.Children.Add(smallLine);                        
                    }

                }

                // Drawing the dots between the Groups.
                foreach (var rowItem in DotInfo)
                {
                    double x = rowItem.Value * 20 + 10;

                    Rect rowRect = this.Control.GridProperties.ActiveSpreadsheetGrid.RangeToRect(ScrollAxisRegion.Body, ScrollAxisRegion.Body, GridRangeInfo.Row(rowItem.Key), true, true);

                    double y = rowRect.Top + (rowRect.Height / 2);

                    if (y > headerRowHeight && y < this.ActualHeight - 40 && rowRect.Height > 0)
                    {

                        Line dotLine = new Line();
                        dotLine.Stroke = new SolidColorBrush(Colors.Black);
                        dotLine.StrokeThickness = 2; 
                        dotLine.X1 = x;
                        dotLine.Y1 = y;
                        dotLine.X2 = x;
                        dotLine.Y2 = y + 2;

                        this.Children.Add(dotLine);                        
                    }
                }
            }
            else
            {                
                foreach (var rangeItem in GroupLineInfo)
                {
                    for (int i = rangeItem.Key.Left; i <= rangeItem.Key.Right; i++)
                    {
                        if (!DotInfo.ContainsKey(i))
                            DotInfo.Add(i, rangeItem.Value);
                        else
                            DotInfo[i] = DotInfo[i] > rangeItem.Value ? DotInfo[i] : rangeItem.Value;
                    }
                }

                //Drawing the GroupLines.
                foreach (var item in RenderLineInfo)
                {
                    double y = item.Value * 20 - 10;
                    double startPoint = item.Key.Left < headerColumnWidth ? headerColumnWidth : (item.Key.Left > this.ActualWidth - 20 ? this.ActualWidth - 20 : item.Key.Left);
                    double endPoint = item.Key.Right < headerColumnWidth ? headerColumnWidth : (item.Key.Right > this.ActualWidth - 20 ? this.ActualWidth - 20 : item.Key.Right);

                    startPoint = IsSummaryColumnRight ? startPoint + 2 : startPoint;
                    endPoint = IsSummaryColumnRight ? endPoint : endPoint - 2;

                    Line groupLine = new Line();
                    groupLine.Stroke = new SolidColorBrush(Colors.Black);
                    groupLine.StrokeThickness = 2;
                    groupLine.X1 = startPoint;
                    groupLine.Y1 = y;
                    groupLine.X2 = endPoint;
                    groupLine.Y2 = y;

                    this.Children.Add(groupLine);                    

                    if (IsSummaryColumnRight && item.Key.Left >= headerColumnWidth)
                    {
                        Line smallLine = new Line();
                        smallLine.StrokeThickness = 2;
                        smallLine.Stroke = new SolidColorBrush(Colors.Black);
                        smallLine.X1 = startPoint;
                        smallLine.Y1 = y;
                        smallLine.X2 = startPoint;
                        smallLine.Y2 = y+8;

                        this.Children.Add(smallLine);                        
                    }
                    else if (!IsSummaryColumnRight && item.Key.Right <= this.ActualWidth - 20)
                    {
                        Line smallLine = new Line();
                        smallLine.Stroke = new SolidColorBrush(Colors.Black);
                        smallLine.StrokeThickness = 2;
                        smallLine.X1 = endPoint;
                        smallLine.Y1 = y;
                        smallLine.X2 = endPoint;
                        smallLine.Y2 = y + 8;

                        this.Children.Add(smallLine);                        
                    }

                    // Drawing the dots between the Groups.
                    foreach (var rowItem in DotInfo)
                    {
                        double y1 = rowItem.Value * 20 + 10;

                        Rect rowRect = this.Control.GridProperties.ActiveSpreadsheetGrid.RangeToRect(ScrollAxisRegion.Body, ScrollAxisRegion.Body, GridRangeInfo.Col(rowItem.Key), true, true);

                        double x = rowRect.Left + (rowRect.Width / 2);

                        if (x > headerColumnWidth && x < this.ActualWidth - 20 && rowRect.Width > 0)
                        {
                            Line dotLine = new Line();
                            dotLine.StrokeThickness = 2;
                            dotLine.Stroke = new SolidColorBrush(Colors.Black);
                            dotLine.X1 = x;
                            dotLine.Y1 = y1;
                            dotLine.X2 = x+2;
                            dotLine.Y2 = y1;

                            this.Children.Add(dotLine);                            
                        }
                    }
                }
            }
        }
#endif

#if !SILVERLIGHT
       protected override void OnRender(DrawingContext dc)
        {            
            DotInfo.Clear();

            if (this.Orientation == "Vertical")
            {
                //Below code computes where the dots should be placed using GroupLineInfo.
                //Each row contains only one dot at the inner most outlinelevel.
                foreach (var rangeItem in GroupLineInfo)
                {
                    for (int i = rangeItem.Key.Top; i <= rangeItem.Key.Bottom; i++)
                    {
                        if (!DotInfo.ContainsKey(i))
                            DotInfo.Add(i, rangeItem.Value);
                        else
                            DotInfo[i] = DotInfo[i] > rangeItem.Value ? DotInfo[i] : rangeItem.Value;
                    }
                }                

                //Drawing the vertical GroupLines.
                foreach (var item in RenderLineInfo)
                {
                    double x = item.Value * 20 - 10;
                    double startPoint = item.Key.Top < headerRowHeight ? headerRowHeight : (item.Key.Top > this.ActualHeight - 40 ? this.ActualHeight - 40 : item.Key.Top);
                    double endPoint = item.Key.Bottom < headerRowHeight ? headerRowHeight : (item.Key.Bottom > this.ActualHeight - 40 ? this.ActualHeight - 40 : item.Key.Bottom);

                    startPoint = IsSummaryRowBelow ? startPoint + 2 : startPoint;
                    endPoint = IsSummaryRowBelow ? endPoint : endPoint - 2;

                    dc.DrawLine(new Pen(Brushes.Black, 0.5), new Point(x, startPoint), new Point(x, endPoint));

                    //Below code draws a small horizontal line to the end of the group.
                    if (IsSummaryRowBelow && item.Key.Top >= headerRowHeight)
                    {                        
                        dc.DrawLine(new Pen(Brushes.Black, 0.5), new Point(x, startPoint), new Point(x + 8, startPoint));
                    }
                    else if (!IsSummaryRowBelow && item.Key.Bottom <= this.ActualHeight - 40)
                    {                        
                        dc.DrawLine(new Pen(Brushes.Black, 0.5), new Point(x, endPoint), new Point(x + 8, endPoint));
                    }

                }

                // Drawing the dots between the Groups.
                foreach (var rowItem in DotInfo)
                {
                    double x = rowItem.Value * 20 + 10;

                    Rect rowRect = this.Control.GridProperties.ActiveSpreadsheetGrid.RangeToRect(ScrollAxisRegion.Body, ScrollAxisRegion.Body, GridRangeInfo.Row(rowItem.Key), true, true);

                    double y = rowRect.Top + (rowRect.Height / 2);

                    if (y > headerRowHeight && y < this.ActualHeight - 40 && rowRect.Height > 0)
                    {
                        dc.DrawLine(new Pen(Brushes.Black, 2), new Point(x, y), new Point(x, y + 2));
                    }
                }
            }
            else
            {
                foreach (var rangeItem in GroupLineInfo)
                {
                    for (int i = rangeItem.Key.Left; i <= rangeItem.Key.Right; i++)
                    {
                        if (!DotInfo.ContainsKey(i))
                            DotInfo.Add(i, rangeItem.Value);
                        else
                            DotInfo[i] = DotInfo[i] > rangeItem.Value ? DotInfo[i] : rangeItem.Value;
                    }
                }

                //Drawing the GroupLines.
                foreach (var item in RenderLineInfo)
                {
                    double y = item.Value * 20 - 10;
                    double startPoint = item.Key.Left < headerColumnWidth ? headerColumnWidth : (item.Key.Left > this.ActualWidth - 20 ? this.ActualWidth - 20 : item.Key.Left);
                    double endPoint = item.Key.Right < headerColumnWidth ? headerColumnWidth : (item.Key.Right > this.ActualWidth - 20 ? this.ActualWidth - 20 : item.Key.Right);

                    startPoint = IsSummaryColumnRight ? startPoint + 2 : startPoint;
                    endPoint = IsSummaryColumnRight ? endPoint : endPoint - 2;

                    dc.DrawLine(new Pen(Brushes.Black, 0.5), new Point(startPoint, y), new Point(endPoint, y));

                    if (IsSummaryColumnRight && item.Key.Left >= headerColumnWidth)
                    {                        
                        dc.DrawLine(new Pen(Brushes.Black, 0.5), new Point(startPoint, y), new Point(startPoint, y+8));
                    }
                    else if (!IsSummaryColumnRight && item.Key.Right <= this.ActualWidth - 20)
                    {                        
                        dc.DrawLine(new Pen(Brushes.Black, 0.5), new Point(endPoint, y), new Point(endPoint,y+8));
                    }
                }

                // Drawing the dots between the Groups.
                foreach (var rowItem in DotInfo)
                {
                    double y = rowItem.Value * 20 + 10;

                    Rect rowRect = this.Control.GridProperties.ActiveSpreadsheetGrid.RangeToRect(ScrollAxisRegion.Body, ScrollAxisRegion.Body, GridRangeInfo.Col (rowItem.Key), true, true);

                    double x = rowRect.Left + (rowRect.Width / 2);

                    if (x > headerColumnWidth && x < this.ActualWidth - 20 && rowRect.Width > 0)
                    {
                        dc.DrawLine(new Pen(Brushes.Black, 2), new Point(x, y), new Point(x+2, y));
                    }
                }
            }
        }
#endif


       /// <summary>
       /// Copies the Childrens of the panel to Collections for recycling.
       /// </summary>
        internal void PreArrange()
        {
            UnloadUiElements = new List<UIElement>();
            UnloadHeaderElements = new Dictionary<int, UIElement>();

            foreach (UIElement el in this.Children)
            {
                if (el is SpreadsheetGroupButton)
                    UnloadUiElements.Add(el);
                else if(el is Button)
                    UnloadHeaderElements.Add((int)((el as Button).Tag), el);                
            }
        }

       /// <summary>
       /// removes the unwanted childrens from the panel and clears the Unload Dictionaries.
       /// </summary>
        internal void PostArrange()
        {
#if !SILVERLIGHT
            foreach (UIElement el in UnloadUiElements)
            {
                this.Children.Remove(el);
            }
            foreach (UIElement el1 in UnloadHeaderElements.Values)
                this.Children.Remove(el1);
#endif
            UnloadHeaderElements.Clear();
            UnloadUiElements.Clear();
        }

    }

    /// <summary>
    /// Expander/Collapse buttons placed in the SpreasheetGroupPanel.
    /// </summary>
   [DesignTimeVisible(false)]
    public class SpreadsheetGroupButton : ToggleButton
    {
        public SpreadsheetGroupButton()
        {
            base.DefaultStyleKey = typeof(SpreadsheetGroupButton);
        }

        public SpreadsheetGroupButton(GridRangeInfo Range, int OutlineLevel,bool isColapsed)
        {
            base.DefaultStyleKey = typeof(SpreadsheetGroupButton);

            this.Range = Range;
            this.OutlineLevel = OutlineLevel;
            this.IsCollapsed = isColapsed;                     
        }

        private GridRangeInfo range;
        private int outlineLevel;
        private bool isCollapsed;

        public GridRangeInfo Range
        {
            get{ return this.range; }
            set{ this.range = value; }
        }

        public int OutlineLevel
        {
            get{ return this.outlineLevel; }
            set{ this.outlineLevel = value; }
        }

        public bool IsCollapsed
        {
            get{ return this.isCollapsed; }
            set{ this.isCollapsed = value; }
        }
        
    }
}
