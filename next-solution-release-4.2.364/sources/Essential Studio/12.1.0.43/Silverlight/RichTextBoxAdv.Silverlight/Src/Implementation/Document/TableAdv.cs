#region Copyright Syncfusion Inc. 2001 - 2014
// Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Any infringement will be prosecuted under
// applicable laws. 
#endregion
using System;
using System.Net;
using System.Linq;
using System.Windows;
using System.Collections;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Ink;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;
using System.Windows.Markup;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Text;

namespace Syncfusion.Windows.Tools.Controls
{
    [ContentProperty("Rows")]
    public class TableAdv : BlockAdv
    {
        #region Private members

        TableRowAdvCollection rows = new TableRowAdvCollection();
        TableHolder tablehold = null;
        ObservableCollection<List<ElementBox>> tableboxes = new ObservableCollection<List<ElementBox>>();

        #endregion

        public override bool IsEmpty
        {
            get { return Rows.Count ==0; }
        }


        public Color Background
        {
            get { return (Color)GetValue(BackgroundProperty); }
            set { SetValue(BackgroundProperty, value); }
        }

        // Using a DependencyProperty as the backing store for Background.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty BackgroundProperty =
            DependencyProperty.Register("Background", typeof(Color), typeof(TableAdv), new PropertyMetadata(Color.FromArgb(0,0,0,0)));



        public Color BorderBrush
        {
            get 
            { 
                return (Color)GetValue(BorderBrushProperty); 
            }
            set 
            { 
                SetValue(BorderBrushProperty, value); 
            }
        }

        // Using a DependencyProperty as the backing store for BorderBrush.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty BorderBrushProperty =DependencyProperty.Register("BorderBrush", typeof(Color), typeof(TableAdv), new PropertyMetadata(Colors.Black));

        public double BorderThickness
        {
            get 
            { 
                return (double)GetValue(BorderThicknessProperty); 
            }
            set 
            { 
                SetValue(BorderThicknessProperty, value); 
            }
        }

        // Using a DependencyProperty as the backing store for BorderThickness.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty BorderThicknessProperty = DependencyProperty.Register("BorderThickness", typeof(double), typeof(TableAdv), new PropertyMetadata(1.15));

        public DoubleCollection StrokeDashArray
        {
            get 
            { 
                return (DoubleCollection)GetValue(StrokeDashArrayProperty); 
            }
            set 
            { 
                SetValue(StrokeDashArrayProperty, value); 
            }
        }

        // Using a DependencyProperty as the backing store for StrokeDashArray.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty StrokeDashArrayProperty =
            DependencyProperty.Register("StrokeDashArray", typeof(DoubleCollection), typeof(TableAdv), new PropertyMetadata(new DoubleCollection()));
        
        public PenLineCap StrokeDashCap
        {
            get 
            { 
                return (PenLineCap)GetValue(StrokeDashCapProperty); 
            }
            set 
            { 
                SetValue(StrokeDashCapProperty, value); 
            }
        }

        // Using a DependencyProperty as the backing store for StrokeDashCap.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty StrokeDashCapProperty =
            DependencyProperty.Register("StrokeDashCap", typeof(PenLineCap), typeof(TableAdv), new PropertyMetadata(PenLineCap.Flat));

        public double StrokeDashOffset
        {
            get 
            { 
                return (double)GetValue(StrokeDashOffsetProperty); 
            }
            set 
            { 
                SetValue(StrokeDashOffsetProperty, value); 
            }
        }

        // Using a DependencyProperty as the backing store for StrokeDashOffset.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty StrokeDashOffsetProperty =
            DependencyProperty.Register("StrokeDashOffset", typeof(double), typeof(TableAdv), new PropertyMetadata(0.0));

        public PenLineJoin StrokeLineJoin
        {
            get 
            { 
                return (PenLineJoin)GetValue(StrokeLineJoinProperty); 
            }
            set 
            { 
                SetValue(StrokeLineJoinProperty, value); 
            }
        }

        // Using a DependencyProperty as the backing store for StrokeLineJoin.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty StrokeLineJoinProperty =
            DependencyProperty.Register("StrokeLineJoin", typeof(PenLineJoin), typeof(TableAdv), new PropertyMetadata(PenLineJoin.Miter));

        public double StrokeMiterLimit
        {
            get 
            { 
                return (double)GetValue(StrokeMiterLimitProperty); 
            }
            set 
            { 
                SetValue(StrokeMiterLimitProperty, value); 
            }
        }

        // Using a DependencyProperty as the backing store for StrokeMiterLimit.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty StrokeMiterLimitProperty =
            DependencyProperty.Register("StrokeMiterLimit", typeof(double), typeof(TableAdv), new PropertyMetadata(0.0));


        public PenLineCap StrokeStartLineCap
        {
            get 
            { 
                return (PenLineCap)GetValue(StrokeStartLineCapProperty); 
            }
            set 
            { 
                SetValue(StrokeStartLineCapProperty, value); 
            }
        }

        // Using a DependencyProperty as the backing store for StrokeStartLineCap.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty StrokeStartLineCapProperty =
            DependencyProperty.Register("StrokeStartLineCap", typeof(PenLineCap), typeof(TableAdv), new PropertyMetadata(PenLineCap.Flat));


        public PenLineCap StrokeEndLineCap
        {
            get 
            { 
                return (PenLineCap)GetValue(StrokeEndLineCapProperty); 
            }
            set 
            { 
                SetValue(StrokeEndLineCapProperty, value); 
            }
        }

        // Using a DependencyProperty as the backing store for StrokeEndLineCap.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty StrokeEndLineCapProperty =
            DependencyProperty.Register("StrokeEndLineCap", typeof(PenLineCap), typeof(TableAdv), new PropertyMetadata(PenLineCap.Flat));

        
        public Style BorderStyle
        {
            get { return (Style)GetValue(BorderStyleProperty); }
            set { SetValue(BorderStyleProperty, value); }
        }

        // Using a DependencyProperty as the backing store for BorderStyle.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty BorderStyleProperty =
            DependencyProperty.Register("BorderStyle", typeof(Style), typeof(TableAdv), null);

        /// <summary>
        /// Row collection  
        /// </summary>
        public TableRowAdvCollection Rows
        {
            get
            {
                return rows;
            }
            set
            {
                rows = value;
            }
        }

        /// <summary>
        /// Table Holder to hold columns
        /// </summary>
        public TableHolder TableHolder
        {
            get
            {
                return tablehold;
            }
            internal set
            {
                tablehold = value;
            }
        }
        
        /// <summary>
        /// It Measures the TableWidth
        /// </summary>
        public double TableWidth
        {
            get
            {
                if (!IsInsideTable)
                {
                    if (this.LayoutViewer is PageLayoutViewer)
                        return Section.PageSize.Width;
                    else
                        return this.LayoutViewer.AvailableSize.Width;
                }
                else if (IsInsideTable)
                    return AssociatedCell.DesiredWidth -(AssociatedCell.CellMargin.Left + AssociatedCell.CellMargin.Right);
                return 0.0;
            }
        }

        /// <summary>
        /// It gets/sets the ElementBoxes of the Table 
        /// </summary>
        internal ObservableCollection<List<ElementBox>> TableElementBoxes
        {
            get
            {
                return tableboxes;
            }
            set
            {
                tableboxes = value;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        public TableAdv()
        {
            IsTable = true;
            Rows = new TableRowAdvCollection();
            this.LineInfo = new ObservableCollection<LineInfo>();
            this.TableHolder = new TableHolder();
            TableElementBoxes = new ObservableCollection<List<ElementBox>>();
        }
        
        /// <summary>
        /// It creates the new table with given rows and columns count
        /// </summary>
        /// <param name="rowcount"></param>
        /// <param name="columncount"></param>
        public TableAdv(int rowcount, int columncount):this()
        {
            TableRowAdv row = null;
            TableCellAdv cell = null;
            for (int i = 0; i < rowcount; i++)
            {
                row = new TableRowAdv();
                for (int j = 0; j < columncount; j++)
                {
                    cell = new TableCellAdv();
                    row.Cells.Add(cell);
                }
                this.Rows.Add(row);
            }
        }

        /// <summary>
        /// It measures the Table instance
        /// </summary>
        internal override void MeasureElements()
        {
            TableElementBoxes.Clear();
            
            Rows.ToList().ForEach(row =>
            {
                row.Owner = this;
                row.MeasureElements();
            });

            SetCellColumnIndexs();
            AddColumns();
            RecalculateTableElementBoxes();
        }

        /// <summary>
        /// It adds the columns to the Table.
        /// </summary>
        internal void AddColumns()
        {
            TableHolder.Columns.Clear();
            int toremovelast = 0;

            foreach (TableRowAdv r in Rows)
            {
                foreach (TableCellAdv c in r.Cells)
                {
                    toremovelast = Math.Max(toremovelast, (c.ColumnSpan + c.ColumnIndex) - 1);
                    while (TableHolder.Columns.Count < (c.ColumnIndex+c.ColumnSpan))
                    {
                        TableHolder.Columns.Add(new TableColumnAdv());
                    }
                }
            }
            while ((toremovelast+1) < TableHolder.Columns.Count)
            {
                TableHolder.Columns.RemoveAt(TableHolder.Columns.Count - 1);
            }
        }

        /// <summary>
        /// It sets the Column indexs to the TableCell
        /// </summary>
        internal void SetCellColumnIndexs()
        {
            int maxcolumnspan = 0;
            int maxrowspan = 0;
            int tempcolumnindex = 0;
            int temprowindex = 0;

            try
            {
                foreach (TableRowAdv row in Rows)
                {
                    foreach (TableCellAdv cell in row.Cells)
                    {
                        maxcolumnspan = maxcolumnspan + cell.ColumnSpan;
                        maxrowspan = maxrowspan + cell.RowSpan;
                    }
                }

                bool[,] matrix = new bool[maxrowspan, maxcolumnspan];

                foreach (TableRowAdv r in this.Rows)
                {
                    tempcolumnindex = 0;
                    foreach (TableCellAdv c in r.Cells)
                    {
                        while (matrix[temprowindex, tempcolumnindex])
                        {
                            tempcolumnindex++;
                        }
                        for (int i = 0; i < c.ColumnSpan; i++)
                        {
                            for (int j = 0; j < c.RowSpan; j++)
                            {
                                matrix[temprowindex + j, tempcolumnindex + i] = true;
                            }
                        }
                        c.RowIndex = temprowindex;
                        if(c.CellElementBox !=null)
                            c.CellElementBox.RowIndex = temprowindex;
                        c.ColumnIndex = tempcolumnindex;
                        if(c.CellElementBox !=null)
                            c.CellElementBox.ColumnIndex = tempcolumnindex;
                        tempcolumnindex++;
                    }
                    temprowindex++;
                }
            }
            catch { }
        }

        /// <summary>
        /// 
        /// </summary>
        internal override void ArrangeElements()
        {
            CreateNewLines = true;
            AddColumns();
            if (IsInsideTable)
            {
                TableAdv owner = AssociatedCell.OwnerTable;
                //double preWidth = AssociatedCell.DesiredWidth;
                //double height = AssociatedCell.CellHeight;

                //double subracted = owner.TableWidth - owner.Section.PageContentMargin.Left - owner.Section.PageContentMargin.Right;

                //TableLayoutCalculator.MeasureTableLayout(owner, subracted);
                //bool isSplitted = AssociatedCell.CellElementBox.LineInfo.IsSplitted;

                //if (preWidth == AssociatedCell.DesiredWidth)
                //{
                //    ArrangeTable();
                //    AssociatedCell.CellElementBox.SetElementPosition();
                //    if (Math.Floor(AssociatedCell.MeasureCellHeight()) != Math.Floor(height) || isSplitted)
                //    {
                //        int index = owner.LineInfo.IndexOf(AssociatedCell.CellElementBox.LineInfo);
                //        index = index == 0 ? 0 : index - 1;
                //        LineInfo lineInfo = owner.LineInfo[index];
                //        while (lineInfo.HasChildBoxes)
                //        {
                //            index--;
                //            lineInfo = owner.LineInfo[index];
                //        }
                //        owner.ArrangeElements(index);
                //    }
                //}
                //else
                //{
                while (owner != null)
                {
                    owner.ArrangeElements();
                    if (owner.AssociatedCell == null)
                        break;
                    owner = owner.AssociatedCell.OwnerTable;
                }
                //}
            }
            else
            {
                ArrangeTable();
            }
        }

        internal void ArrangeTable()
        {
            if (Section == null)
                Section = base.GetLayoutViewer().Document.Sections[0];

            if (!IsInsideTable)
            {
                if (LayoutViewer is PageLayoutViewer)
                {
                    ArrangeElements(Section.PageSize);
                }
                else
                {
                    ArrangeElements(LayoutViewer.AvailableSize);
                }
            }
            else
            {
                ArrangeElements(new Size(AssociatedCell.DesiredWidth, 0));
            }
        }

        /// <summary>
        /// It recalculate the Tableboxes withe RowSpan and ColumnSpan
        /// </summary>
        internal void RecalculateTableElementBoxes()
        {
            TableCellElementBox cellbox = null;

            foreach (List<ElementBox> rowelementboxes in TableElementBoxes)
            {
                for (int i = 0; i < rowelementboxes.Count; i++)
                {
                    ElementBox box = rowelementboxes[i];
                    if (box is TableCellElementBox)
                    {
                        cellbox = box as TableCellElementBox;
                        //if (cellbox.ElementSize.Width > 0.0)
                        //{
                            if (cellbox.RowSpan > 1)
                            {
                                TableCellElementBox rowspanbox = new TableCellElementBox();
                                rowspanbox.IsTopHide = true;
                                TableCellElementBox bottomcell = null;
                                int colindex = cellbox.ColumnIndex;
                                int cellindex = 0;
                                int index = TableElementBoxes.IndexOf(rowelementboxes) + 1;
                                
                                if (index < TableElementBoxes.Count)
                                {
                                    for (int j = 0; j < TableElementBoxes[index].Count; j++)
                                    {
                                        TableCellElementBox cellbox2 = TableElementBoxes[index][j] as TableCellElementBox;
                                        if (cellbox2.ColumnIndex >= colindex)
                                        {
                                            cellindex = j;
                                            break;
                                        }
                                        else if (j == TableElementBoxes[index].Count - 1)
                                        {
                                            if (cellbox2.ColumnIndex < colindex)
                                            {
                                                cellindex = j + 1;
                                                break;
                                            }
                                        }
                                    }
                                    if (TableElementBoxes[index].Count == cellindex)
                                    {
                                        bottomcell = cellbox;
                                    }
                                    else if (cellindex < TableElementBoxes[index].Count)
                                    {
                                        bottomcell = TableElementBoxes[index][cellindex] as TableCellElementBox;
                                    }
                                    else
                                        break;
                                    rowspanbox.ElementSize = bottomcell.ElementSize;
                                    int rowspan = cellbox.RowSpan;
                                    rowspanbox.RowSpan = rowspan - 1;
                                    rowspanbox.ColumnIndex = cellbox.ColumnIndex;
                                    rowspanbox.ColumnSpan = cellbox.ColumnSpan;
                                    rowspanbox.BaseCell = cellbox.BaseCell;
                                    cellbox.BaseCell.RowSpannedBoxes.Add(rowspanbox);
                                    cellbox.BottomCellBox = rowspanbox;
                                    if (!TableElementBoxes[index].Contains(rowspanbox))
                                    {
                                        TableElementBoxes[index].Insert(cellindex, rowspanbox);
                                    }
                                    cellbox.IsBottomHide = true;
                                    if (TableElementBoxes[index].Count > TableHolder.Columns.Count)
                                    {
                                        TableHolder.Columns.Add(new TableColumnAdv());
                                    }
                                }
                            }
                            if (cellbox.ColumnSpan > 1)
                            {
                                int total = rowelementboxes.Count + cellbox.ColumnSpan - 1;
                                if (total > TableHolder.Columns.Count)
                                {
                                    TableHolder.Columns.Add(new TableColumnAdv());
                                }
                            }
                        //}
                    }
                }
                   
            }
        }

        /// <summary>
        /// It assigns the Desired width to the TableCells
        /// </summary>
        internal void SetDesiredWidthToCells()
        {
            foreach (TableRowAdv rw in Rows)
            {
                foreach (TableCellAdv cell in rw.Cells)
                {
                    if (cell.HasColumnSpan())
                    {
                        double tempwidth = 0.0;
                        for (int i = cell.ColumnIndex; i < (cell.ColumnIndex + cell.ColumnSpan); i++)
                        {
                            tempwidth += TableHolder.Columns[i].PreferredWidth;
                        }
                        cell.DesiredWidth = tempwidth;
                    }
                    else
                    {
                        cell.DesiredWidth = TableHolder.Columns[cell.ColumnIndex].PreferredWidth;
                    }
                }
            }
        }
        
        /// <summary>
        /// 
        /// </summary>
        /// <param name="size"></param>
        internal override void ArrangeElements(Size size)
        {
            BlockAdv block = Arrange(size);
            while (block != null)
            {
                block.LayoutViewer = LayoutViewer;
                block=block.Arrange(size);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="size"></param>
        /// <returns></returns>
        internal override BlockAdv Arrange(Size size)
        {
            double computedWidth = 0.0;
            double comX = 0.0;
            double startPoint = 0.0;
            bool containsLine = false;
            int lineIndex = 0;
            bool flag = false;
            LineInfo lineInfo = new LineInfo();

            double previouspoint = EndPoint;
            this.Width = size.Width;
            lineInfo.Block = this;
            lineInfo.IsTableLine = true;

            if (CreateNewLines)
            {
                if (!IsInsideTable)
                {
                    computedWidth = Width - (Margin.Left + Margin.Right + LeftIndent + RightIndent);
                    comX = Margin.Left + LeftIndent;
                    startPoint = PreviousBlock != null ? PreviousBlock.EndPoint : Margin.Top;
                }
                else if (IsInsideTable)
                {
                    computedWidth = Width;
                    comX = AssociatedCell.CellMargin.Left;
                    startPoint = PreviousBlock != null ? PreviousBlock.EndPoint : AssociatedCell.CellElementBox.BoundingRectangle.Y;
                }

                if (LayoutViewer is FlowLayoutViewer)
                {
                    computedWidth = Width;
                    comX = LeftIndent;
                    startPoint = PreviousBlock != null ? PreviousBlock.EndPoint : 0;
                }

                TableLayoutCalculator.MeasureTableOnAutoMode(this, computedWidth);
                
                if (Section != null)
                {
                    if (Section.Document != null)
                    {
                        if (Section.Document.OwnerControl != null)
                        {
                            lineInfo.CalEmptyLineHeight(Section.Document.OwnerControl.CurrentInlineStyle);
                        }
                    }
                }

                foreach (LineInfo line in LineInfo)
                {
                    if (!IsInsideTable)
                    {
                        if (LayoutViewer.LineInfos.Contains(line))
                        {
                            if (LineInfo.First() == line)
                            {
                                containsLine = true;
                                lineIndex = LayoutViewer.LineInfos.IndexOf(line);
                            }
                            LayoutViewer.LineInfos.Remove(line);
                        }
                    }
                    else if (IsInsideTable)
                    {
                        if (AssociatedCell.LineInfos.Contains(line))
                        {
                            if (LineInfo.First() == line)
                            {
                                containsLine = true;
                                lineIndex = AssociatedCell.LineInfos.IndexOf(line);
                            }
                            AssociatedCell.LineInfos.Remove(line);
                        }
                    }
                    line.ClearElementBoxes();
                }

                LineInfo.Clear();

                lineInfo.BoundingRectangle = new Rect(comX, startPoint, lineInfo.Width, lineInfo.Height);
                lineInfo.Width = lineInfo.BoundingRectangle.Width;

                //LineInfo.Add(lineInfo);

                double xPos = 0.0;
                foreach (List<ElementBox> elementbox in this.TableElementBoxes)
                {
                    xPos = comX;
                    foreach (ElementBox box in elementbox)
                    {
                        box.LineInfo = lineInfo;
                        lineInfo.Width = xPos + box.ElementSize.Width;
                        lineInfo.Add(box);
                        lineInfo.Block = this;
                        xPos = xPos + box.ElementSize.Width;
                    }
                    LineInfo.Add(lineInfo);
                    lineInfo = new LineInfo();
                    lineInfo.IsTableLine = true;
                }

                if (!containsLine && PreviousBlock != null && PreviousBlock.LineInfo.Count > 0)
                {
                    containsLine = true;
                    lineIndex = IsInsideTable ? AssociatedCell.LineInfos.IndexOf(PreviousBlock.LineInfo.Last()) + 1 : GetLayoutViewer().LineInfos.IndexOf(PreviousBlock.LineInfo.Last()) + 1;
                }
                if (!containsLine && NextBlock != null && NextBlock.LineInfo.Count > 0)
                {
                    containsLine = true;
                    lineIndex = IsInsideTable ? AssociatedCell.LineInfos.IndexOf(NextBlock.LineInfo.First()) : GetLayoutViewer().LineInfos.IndexOf(NextBlock.LineInfo.First());
                }

                double xposition = 0.0;

                if (!IsInsideTable)
                    xposition = comX;
                else if (IsInsideTable)
                    xposition = AssociatedCell.CellMargin.Left + AssociatedCell.CellElementBox.BoundingRectangle.Left;

                for (int i = 0; i < LineInfo.Count; i++)
                {
                    LineInfo line = LineInfo[i];
                    line.IsFirstLine = false;
                    line.CalculateMaxHeight();
                    if (!IsInsideTable)
                    {
                        if (containsLine && lineIndex >-1)
                        {
                            this.LayoutViewer.LineInfos.Insert(lineIndex, line);
                        }
                        else
                        {
                            this.LayoutViewer.LineInfos.Add(line);
                        }
                    }
                    else
                    {
                        if (containsLine && lineIndex > -1)
                        {
                            AssociatedCell.LineInfos.Insert(lineIndex, line);
                        }
                        else
                        {
                            AssociatedCell.LineInfos.Add(line);
                        }
                    }
                    lineIndex++;
                    if (line.Width > computedWidth)
                        line.Width = computedWidth;
                    if (line.IsFirstLine)
                    {
                        startPoint = line.BoundingRectangle.Top;
                    }
                    line.BoundingRectangle = new Rect(xposition, startPoint, line.Width, line.Height);
                    line.ArrangeTableElementBoxes();
                    startPoint = line.BoundingRectangle.Bottom;
                }
            }
            else
            {
                UpdateBounds(size);
            }
            IsArranged = true;
            WrapToNextParagraphs(previouspoint,size, ref flag);
            CreateNewLines = false;

            if (flag)
                return NextBlock;
            return null;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="size"></param>
        internal void UpdateBounds(Size size)
        {
            double end = EndPoint;
            Width = size.Width;
            int lineIndex = 0;
            bool containsLine = false;

            double startPoint = PreviousBlock != null ? PreviousBlock.EndPoint : Margin.Top;
            double computedWidth = Width - (Margin.Left + Margin.Right + LeftIndent + RightIndent);
            double comX = Margin.Left + LeftIndent;

            if (IsInsideTable)
            {
                startPoint = PreviousBlock != null ? PreviousBlock.EndPoint : AssociatedCell.CellElementBox.BoundingRectangle.Y;
                comX = AssociatedCell.CellElementBox.BoundingRectangle.X;
                computedWidth = Width;
            }

            if (LayoutViewer is FlowLayoutViewer)
            {
                computedWidth = Width - (LeftIndent + RightIndent);
                comX = LeftIndent;
                startPoint = PreviousBlock != null ? PreviousBlock.EndPoint : 0;
            }

            if (LayoutViewer is PageLayoutViewer)
            {
                foreach (LineInfo line in LineInfo)
                {
                    if(line.RenderingOption==RenderingOptions.None)
                        line.RenderingOption = RenderingOptions.RemoveAndAdd;

                    if (IsInsideTable)
                    {
                        if (AssociatedCell.LineInfos.Contains(line))
                        {
                            if (LineInfo.First() == line)
                            {
                                containsLine = true;
                                lineIndex = AssociatedCell.LineInfos.IndexOf(line);
                            }
                            AssociatedCell.LineInfos.Remove(line);
                        }
                    }
                    else
                    {
                        if (LayoutViewer.LineInfos.Contains(line))
                        {
                            if (LineInfo.First() == line)
                            {
                                containsLine = true;
                                lineIndex = LayoutViewer.LineInfos.IndexOf(line);
                            }
                            LayoutViewer.LineInfos.Remove(line);
                        }
                    }
                }

                RemoveSplittedLines(LineInfo);
                
                for (int i = 0; i < LineInfo.Count; i++)
                {
                    LineInfo line = LineInfo[i];
                    line.IsFirstLine = false;
                    line.CalculateMaxHeight();
                    if (!IsInsideTable)
                    {
                        if (containsLine && lineIndex > -1)
                        {
                            LayoutViewer.LineInfos.Insert(lineIndex, line);
                        }
                        else
                        {
                            LayoutViewer.LineInfos.Add(line);
                        }
                    }
                    else
                    {
                        if (AssociatedCell != null)
                        {
                            if (containsLine && lineIndex > -1)
                            {
                                AssociatedCell.LineInfos.Insert(lineIndex, line);
                            }
                            else
                            {
                                AssociatedCell.LineInfos.Add(line);
                            }
                        }
                    }
                    lineIndex++;
                    if (line.IsFirstLine)
                    {
                        startPoint = line.BoundingRectangle.Top;
                    }
                    line.BoundingRectangle = new Rect(comX, startPoint, line.Width, line.Height);
                    line.ArrangeTableElementBoxes();
                    startPoint = line.BoundingRectangle.Bottom;
                }
            }
            else
            {
                foreach (LineInfo line in LineInfo)
                {
                    if(line.RenderingOption==RenderingOptions.None)
                        line.RenderingOption = RenderingOptions.RemoveAndAdd;

                    if (!IsInsideTable)
                    {
                        if (!LayoutViewer.LineInfos.Contains(line))
                        {
                            LayoutViewer.LineInfos.Add(line);
                        }
                    }
                    else
                    {
                        if (!AssociatedCell.LineInfos.Contains(line))
                        {
                            AssociatedCell.LineInfos.Add(line);
                        }
                    }
                    line.BoundingRectangle = new Rect(comX, startPoint, line.Width, line.Height);
                    line.ArrangeTableElementBoxes();
                    startPoint = line.BoundingRectangle.Bottom;
                }
            }
        }

        /// <summary>
        /// It removes the Splitted lines from the Page in UpdateBounds
        /// </summary>
        /// <param name="lineinfos"></param>
        internal void RemoveSplittedLines(ObservableCollection<LineInfo> lineinfos)
        {
            int k = 0;
            while (k < lineinfos.Count)
            {
                LineInfo line = lineinfos[k];
                if (line.HasChildBoxes)
                {
                    lineinfos.Remove(line);
                    continue;
                }
                foreach (ElementBox e in line.ElementBoxes)
                {
                    TableCellElementBox cellbox = e as TableCellElementBox;
                    if (cellbox != null)
                    {
                        foreach (LineInfo l in cellbox.LineInfos)
                        {
                            if (l.IsTableLine)
                                RemoveSplittedLines(l.Block.LineInfo);
                        }
                    }
                }
                k++;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="index"></param>
        internal override void ArrangeElements(int index)
        {
            if (index <= 0)
                ArrangeElements();
            else
            {
                double end = EndPoint;
                Size size = new Size(0.0, 0.0);
                if (LayoutViewer is PageLayoutViewer)
                {
                    Width = Section.PageSize.Width;
                    size = Section.PageSize;
                }
                else
                {
                    Width = LayoutViewer.AvailableSize.Width;
                    size = LayoutViewer.AvailableSize;
                }
                int lineIndex = 0;
                bool containsLine = false;

                if (IsInsideTable)
                {
                    TableAdv owner = AssociatedCell.OwnerTable;
                    double preWidth = AssociatedCell.DesiredWidth;

                    double subracted = owner.TableWidth - owner.Section.PageContentMargin.Left - owner.Section.PageContentMargin.Right;

                    TableLayoutCalculator.MeasureTableLayout(owner, subracted);

                    if (preWidth != AssociatedCell.DesiredWidth)
                    {
                        while (owner.AssociatedCell != null)
                        {
                            owner = owner.AssociatedCell.OwnerTable;
                        }
                        owner.ArrangeElements();
                        return;
                    }
                }

                int? startedindex = null;

                for (int i = index; i < LineInfo.Count; i = index)
                {
                    if (IsInsideTable)
                    {
                        if (AssociatedCell.LineInfos.Contains(LineInfo[i]))
                        {
                            if (startedindex == null)
                            {
                                startedindex = i;
                            }
                            containsLine = true;
                            if (i == index)
                                lineIndex = AssociatedCell.LineInfos.IndexOf(LineInfo[i]);
                            AssociatedCell.LineInfos.Remove(LineInfo[i]);
                            LineInfo[i].ClearElementBoxes();
                            LineInfo.Remove(LineInfo[i]);
                        }
                    }
                    else
                    {
                        if (LayoutViewer.LineInfos.Contains(LineInfo[i]))
                        {
                            if (startedindex == null)
                            {
                                startedindex = i;
                            }
                            containsLine = true;
                            if (i == index)
                                lineIndex = LayoutViewer.LineInfos.IndexOf(LineInfo[i]);
                            LayoutViewer.LineInfos.Remove(LineInfo[i]);
                            LineInfo[i].ClearElementBoxes();
                            LineInfo.Remove(LineInfo[i]);
                        }
                    }
                }

                double computedWidth = Width - (Margin.Left + Margin.Right + LeftIndent + RightIndent);
                double comX = Margin.Left + LeftIndent;
                double startPoint = LineInfo.Count > 0 && LineInfo[LineInfo.Count - 1] != null ? LineInfo[LineInfo.Count - 1].BoundingRectangle.Bottom : PreviousBlock != null ? PreviousBlock.EndPoint : Margin.Top;

                if (IsInsideTable)
                {
                    startPoint = LineInfo.Count > 0 && LineInfo[LineInfo.Count - 1] != null ? LineInfo[LineInfo.Count - 1].BoundingRectangle.Bottom : PreviousBlock != null ? PreviousBlock.EndPoint : AssociatedCell.CellElementBox.BoundingRectangle.Y;
                    comX = AssociatedCell.CellElementBox.BoundingRectangle.X + AssociatedCell.CellMargin.Left;
                    computedWidth = Width - AssociatedCell.CellMargin.Left - AssociatedCell.CellMargin.Right;
                }

                if (LayoutViewer is FlowLayoutViewer)
                {
                    computedWidth = Width - (LeftIndent + RightIndent);
                    comX = LeftIndent;
                    startPoint = LineInfo.Count > 0 && LineInfo[LineInfo.Count - 1] != null ? LineInfo[LineInfo.Count - 1].BoundingRectangle.Bottom : PreviousBlock != null ? PreviousBlock.EndPoint : 0;
                }

                TableLayoutCalculator.MeasureTableOnAutoMode(this, index, computedWidth);

                double xPos = 0.0;
                LineInfo lineInfo = new LineInfo();
                lineInfo.IsTableLine = true;

                for (int k = (int)startedindex; k < TableElementBoxes.Count; k++)
                {
                    List<ElementBox> elementbox = TableElementBoxes[k];
                    xPos = comX;
                    foreach (ElementBox box in elementbox)
                    {
                        box.LineInfo = lineInfo;
                        lineInfo.Width = xPos + box.ElementSize.Width;
                        lineInfo.Add(box);
                        lineInfo.Block = this;
                        xPos = xPos + box.ElementSize.Width;
                    }
                    LineInfo.Add(lineInfo);
                    lineInfo = new LineInfo();
                    lineInfo.IsTableLine = true;
                }

                double xposition = 0.0;
                if (!IsInsideTable)
                    xposition = comX;
                else if (IsInsideTable)
                    xposition = AssociatedCell.CellMargin.Left + AssociatedCell.CellElementBox.BoundingRectangle.Left;

                for (int i = index; i < LineInfo.Count; i++)
                {
                    LineInfo[i].IsFirstLine = false;
                    LineInfo[i].CalculateMaxHeight();
                    if (!IsInsideTable)
                    {
                        if (containsLine && lineIndex > -1)
                        {
                            LayoutViewer.LineInfos.Insert(lineIndex, LineInfo[i]);
                        }
                        else
                        {
                            LayoutViewer.LineInfos.Add(LineInfo[i]);
                        }
                    }
                    lineIndex++;
                    if (LineInfo[i].IsFirstLine)
                    {
                        startPoint = LineInfo[i].BoundingRectangle.Top;
                    }
                    LineInfo[i].BoundingRectangle = new Rect(xposition, startPoint, LineInfo[i].Width, LineInfo[i].Height);
                    //double comWidth = ListType != ListType.None ? computedWidth - Hanging : computedWidth;
                    LineInfo[i].ArrangeTableElementBoxes();
                    startPoint = LineInfo[i].BoundingRectangle.Bottom;
                }
                if (LayoutViewer != null)
                {
                    LayoutViewer.ImageResizer.UpdateResizerLocation();
                }
                IsArranged = true;
                bool flag = false;
                if (LineInfo.Count != 0)
                {
                    EndPoint = LineInfo.Last().BoundingRectangle.Bottom;

                    if (Math.Round(end) != Math.Round(EndPoint) && !IsArrangingParagraph)
                    {
                        if (NextBlock != null)
                        {
                            NextBlock.CreateNewLines = false;
                            if (NextBlock.LineInfo.Count == 0)
                                NextBlock.CreateNewLines = true;
                            NextBlock.ArrangeElements(size);
                        }
                    }
                    else if ((Math.Round(end) == Math.Round(EndPoint) && !IsArrangingParagraph))
                    {
                        flag = false;
                        if (NextBlock != null)
                        {
                            if (NextBlock.LineInfo.Count == 0)
                            {
                                NextBlock.CreateNewLines = true;
                                flag = true;
                            }
                            else
                            {
                                PageAdv page1 = LayoutViewer.GetPageFromLine(LineInfo.Last());
                                PageAdv page2 = LayoutViewer.GetPageFromLine(NextBlock.LineInfo.Last());
                                flag = page1 != page2;
                            }

                            if (flag)
                                NextBlock.ArrangeElements(size);
                        }
                    }

                    IsArrangingParagraph = false;
                }
                else
                {
                    EndPoint = 0;
                }
            }
        }

        /// <summary>
        /// It gets the Intersecting cells with the Row.
        /// </summary>
        /// <param name="givenrow"></param>
        /// <returns></returns>
        internal void GetRowSpannedCellsIntersectingWithGivenRow(TableRowAdv givenrow,ref List<TableCellAdv> cells)
        {
            int index = Rows.IndexOf(givenrow);

            foreach (TableRowAdv row in Rows)
            {
                if (row == givenrow)
                {
                    break;
                }
                foreach (TableCellAdv cell in row.Cells)
                {
                    if ((cell.RowIndex + cell.RowSpan) - 1 >= index)
                    {
                        cells.Add(cell);            
                    }
                }
            }
        }

        /// <summary>
        /// It updates the Column indexs.
        /// </summary>
        internal void UpdateColumnIndexes()
        {
            foreach (List<ElementBox> collection in TableElementBoxes)
            {
                int index = 0;
                foreach (ElementBox e in collection)
                {
                    index=collection.IndexOf(e);
                    TableCellElementBox cbox = e as TableCellElementBox;
                    if (cbox != null)
                    {
                        cbox.ColumnIndex = index;
                    }
                    if ((e as TableCellElementBox).ColumnSpan > 1)
                    {
                        index = index + (e as TableCellElementBox).ColumnSpan - 1;
                    }
                }
            }
        }

        /// <summary>
        /// It extracts the Text from the TableCell's Blocks
        /// </summary>
        /// <param name="start"></param>
        /// <param name="end"></param>
        /// <param name="startcell"></param>
        /// <param name="endcell"></param>
        /// <param name="blocks"></param>
        /// <param name="IsStarted"></param>
        /// <param name="stringbuilder"></param>
        /// <returns></returns>
        internal override int Search(TextPosition start, TextPosition end, TableCellAdv startcell, TableCellAdv endcell,ref BlockCollection<BlockAdv> blocks ,ref bool IsStarted,ref StringBuilder stringbuilder)
        {
            bool started = false;
            BlockAdv temptable = null;
            TableRowAdv temprow = null;
            TableCellAdv newcell = null;
            started = IsStarted;

            if (Rows.Count > 0)
            {
                if (started)
                    temptable = CreateBlock();
                foreach (TableRowAdv row in Rows)
                {
                    temprow = row.CreateNewRow();
                    foreach (TableCellAdv cell in row.Cells)
                    {
                        newcell = cell.CreateNewCell(false, false);
                        if (started)
                        {
                            stringbuilder.Append("\r\n");
                        }
                        if (cell == startcell)
                        {
                            foreach (BlockAdv b in cell.Blocks)
                            {
                                newcell.Blocks.Add(b.CopyBlock());
                            }
                            temprow.Cells.Add(newcell);
                            if (temptable == null)
                            {
                                temptable = CreateBlock();
                            }
                            if (!(temptable as TableAdv).Rows.Contains(temprow))
                            {
                                (temptable as TableAdv).Rows.Add(temprow);
                            }
                            if ((temptable as TableAdv).Rows.Count > 0 && !blocks.Contains(temptable))
                            {
                                blocks.Add(temptable);
                            }
                            started = true;
                        }
                        else if (cell == endcell && started)
                        {
                            foreach (BlockAdv b in cell.Blocks)
                            {
                                newcell.Blocks.Add(b.CopyBlock());
                            }
                            temprow.Cells.Add(newcell);
                            if (temptable == null)
                            {
                                temptable = CreateBlock();
                            }
                            if (!(temptable as TableAdv).Rows.Contains(temprow))
                            {
                                (temptable as TableAdv).Rows.Add(temprow);
                            }
                            if ((temptable as TableAdv).Rows.Count > 0 && !blocks.Contains(temptable))
                            {
                                blocks.Add(temptable);
                            }
                            started = false;
                            return 0;
                        }
                        else
                        {
                            if (started)
                            {
                                foreach (BlockAdv b in cell.Blocks)
                                {
                                    newcell.Blocks.Add(b.CopyBlock());
                                }
                                temprow.Cells.Add(newcell);
                                if (temptable == null)
                                {
                                    temptable = CreateBlock();
                                }
                                if (!(temptable as TableAdv).Rows.Contains(temprow))
                                {
                                    (temptable as TableAdv).Rows.Add(temprow);
                                }
                                if ((temptable as TableAdv).Rows.Count > 0 && !blocks.Contains(temptable))
                                {
                                    blocks.Add(temptable);
                                }
                            }
                        }
                    }
                }
            }
            if (started)
                return 1;

            if (IsStarted && !started)
                return 0;

            return -1;
        }

        /// <summary>
        /// It gets the text to cut.
        /// </summary>
        /// <param name="start"></param>
        /// <param name="end"></param>
        /// <param name="startcell"></param>
        /// <param name="endcell"></param>
        /// <param name="IsStarted"></param>
        /// <param name="removedblocks"></param>
        /// <param name="affectedblocks"></param>
        /// <returns></returns>
        internal override int SearchToCut(TextPosition start, TextPosition end, TableCellAdv startcell, TableCellAdv endcell, ref bool IsStarted, ref BlockCollection<BlockAdv> removedblocks, ref BlockCollection<BlockAdv> affectedblocks)
        {
            List<TableRowAdv> temprows = new List<TableRowAdv>();
            bool started = IsStarted;

            foreach (TableRowAdv row in Rows)
            {
                foreach (TableCellAdv c in row.Cells)
                {
                    if (c == startcell)
                    {
                        started = true;
                        if (!temprows.Contains(row))
                        {
                            temprows.Add(row);
                        }
                        if (!affectedblocks.Contains(this))
                        {
                            this.CreateNewLines = true;
                            affectedblocks.Add(this);
                        }
                    }
                    else if (c == endcell)
                    {
                        if (!temprows.Contains(row))
                        {
                            temprows.Add(row);
                        }
                        if (!affectedblocks.Contains(this))
                        {
                            this.CreateNewLines = true;
                            affectedblocks.Add(this);
                        }
                        started = false;
                    }
                    else
                    {
                        if (started)
                        {
                            if (!temprows.Contains(row))
                            {
                                temprows.Add(row);
                            }
                        }
                    }
                }

            }

            foreach (TableRowAdv r in temprows)
            {
                Rows.Remove(r);
                if (Rows.Count == 0)
                {
                    if (!removedblocks.Contains(this))
                    {
                        removedblocks.Add(this);
                        if (affectedblocks.Contains(this))
                        {
                            affectedblocks.Remove(this);
                        }
                    }
                }
                else
                {
                    if (!affectedblocks.Contains(this))
                    {
                        this.CreateNewLines = true;
                        affectedblocks.Add(this);
                    }
                }
            }

            temprows.Clear();

            if (started)
                return 1;

            if (IsStarted && !started)
                return 0;

            return -1;
        }

        /// <summary>
        /// It extracts the Inlines from the TableCells.
        /// </summary>
        /// <param name="start"></param>
        /// <param name="end"></param>
        /// <param name="started"></param>
        /// <param name="affectedblocks"></param>
        /// <param name="inlines"></param>
        /// <returns></returns>
        internal override int ExtractInlines(TextPosition start, TextPosition end, ref bool started, ref BlockCollection<BlockAdv> affectedblocks,ref List<Inline> inlines)
        {
            TableRowAdv firstrow = null;
            TableCellAdv firstcell = null;
            TableRowAdv secondrow = null;
            TableCellAdv secondcell = null;
            BlockAdv firstblk = null;
            BlockAdv secondblk = null;
            List<TableCellAdv> selectedcells = null;
            List<PreservedCellsInfo> preservedcells = null;

            if (start.IsInSameTable(end))
            {
                firstblk = BaseParent.Document.GetBlockFromVirtualPosition(start.VirtualPosition, ref firstrow, ref firstcell);
                secondblk = BaseParent.Document.GetBlockFromVirtualPosition(end.VirtualPosition, ref secondrow, ref secondcell);

                if (firstblk == secondblk && firstblk ==this && secondblk ==this)
                {
                    selectedcells = BaseParent.Selection.SelectedCellsInTable();
                    preservedcells = new List<PreservedCellsInfo>();

                    foreach (TableCellAdv celladv in selectedcells)
                    {
                        PreservedCellsInfo precell = new PreservedCellsInfo();
                        precell.RowIndex = celladv.RowIndex;
                        precell.ColumnIndex = celladv.ColumnIndex;
                        preservedcells.Add(precell);
                    }
                }
            }

            foreach (TableRowAdv row in Rows)
            {
                foreach (TableCellAdv cell in row.Cells)
                {
                    if (firstblk == secondblk && firstblk == this && secondblk == this)
                    {
                        foreach (PreservedCellsInfo presercell in preservedcells)
                        {
                            if (cell.RowIndex == presercell.RowIndex && cell.ColumnIndex == presercell.ColumnIndex)
                            {
                                foreach (BlockAdv b in cell.Blocks)
                                {
                                    int value = b.ExtractInlines(start, end, ref started, ref affectedblocks, ref inlines);
                                    if (value == 0)
                                    {
                                        started = false;
                                        return 0;
                                    }
                                    else if (value == 1)
                                    {
                                        started = true;
                                    }
                                    else if (value == -1)
                                        continue;

                                }
                            }
                        }
                    }
                    else
                    {
                        foreach (BlockAdv b in cell.Blocks)
                        {
                            int value = b.ExtractInlines(start, end, ref started, ref affectedblocks, ref inlines);
                            if (value == 0)
                            {
                                started = false;
                                return 0;
                            }
                            else if (value == 1)
                            {
                                started = true;
                            }
                            else if (value == -1)
                                continue;

                        }
                    }
                }
            }
            return -1;
        }

        /// <summary>
        /// It searches to select the Table.
        /// </summary>
        /// <param name="start"></param>
        /// <param name="end"></param>
        /// <param name="IsStarted"></param>
        /// <param name="blocks"></param>
        /// <returns></returns>
        internal override int SearchToSelect(TextPosition start, TextPosition end,ref bool IsStarted, ref BlockCollection<BlockAdv> blocks)
        {
            bool started = IsStarted;
            TableRowAdv startrow = null;
            TableRowAdv endrow = null;
            TableCellAdv startcell = null;
            TableCellAdv endcell = null;
            List<TableCellAdv> selectedcells = null;
            List<PreservedCellsInfo> preservedcells = null;
            BlockAdv startblk = BaseParent.Document.GetBlockFromVirtualPosition(start.VirtualPosition, ref startrow, ref startcell);
            BlockAdv endblk = BaseParent.Document.GetBlockFromVirtualPosition(end.VirtualPosition, ref endrow, ref endcell);

            if (start.IsInSameTable(end))
            {
                if (startblk == endblk && startblk == this && endblk == this)
                {
                    selectedcells = BaseParent.Selection.SelectedCellsInTable();
                    preservedcells = new List<PreservedCellsInfo>();

                    foreach (TableCellAdv celladv in selectedcells)
                    {
                        PreservedCellsInfo precell = new PreservedCellsInfo();
                        precell.RowIndex = celladv.RowIndex;
                        precell.ColumnIndex = celladv.ColumnIndex;
                        preservedcells.Add(precell);
                    }
                }
            }

            foreach (TableRowAdv row in Rows)
            {
                foreach (TableCellAdv cell in row.Cells)
                {
                    if (startblk == endblk && startblk == this && endblk == this)
                    {
                        foreach (PreservedCellsInfo presercell in preservedcells)
                        {
                            if (cell.RowIndex == presercell.RowIndex && cell.ColumnIndex == presercell.ColumnIndex)
                            {
                                foreach (BlockAdv b in cell.Blocks)
                                {
                                    int value = b.SearchToSelect(start, end, ref started, ref blocks);
                                    if (value == 0)
                                    {
                                        started = false;
                                        return 0;
                                    }
                                    else if (value == 1)
                                    {
                                        started = true;
                                    }
                                    else if (value == -1)
                                        continue;

                                }
                            }
                        }
                    }
                    else
                    {
                        foreach (BlockAdv b in cell.Blocks)
                        {
                            int value = b.SearchToSelect(start, end, ref started, ref blocks);
                            if (value == 0)
                            {
                                started = false;
                                return 0;
                            }
                            else if (value == 1)
                            {
                                started = true;
                            }
                            else if (value == -1)
                                continue;

                        }
                    }
                }
            }
            if (started)
                return 1;

            if (IsStarted && !started)
                return 0;

            return -1;
        }

        /// <summary>
        /// It returns the Blocks from the Table
        /// </summary>
        /// <returns></returns>
        internal BlockCollection<BlockAdv> GetBlocksFromTable()
        {
            BlockCollection<BlockAdv> tempblocks = new BlockCollection<BlockAdv>();

            foreach (TableRowAdv row in Rows)
            {
                foreach (TableCellAdv cell in row.Cells)
                {
                    foreach (BlockAdv b in cell.Blocks)
                    {
                        if (b.IsParagraph)
                        {
                            tempblocks.Add(b);
                        }
                        else if (b.IsTable)
                        {
                            foreach (BlockAdv b2 in (b as TableAdv).GetBlocksFromTable())
                            {
                                tempblocks.Add(b2);
                            }
                        }
                    }
                }
            }
            return tempblocks;
        }

        internal void AssignStyleForTable(BlockAdv block,HistoryInfo history)
        {
            foreach (TableRowAdv row2 in Rows)
            {
                foreach (TableCellAdv cell2 in row2.Cells)
                {
                    foreach (BlockAdv blk in cell2.Blocks)
                    {
                        if (blk.IsParagraph)
                        {
                            if (blk == block)
                            {
                                var style = this.GetType().GetProperty(history.Action.ToString()).GetValue(this, null);
                                block.GetType().GetProperty(history.Action.ToString()).SetValue(block, style, null);
                            }
                        }
                        else if (blk.IsTable)
                        {
                            AssignStyleForTable(block, history);
                        }
                    }
                }
            }
        }

        /// <summary>
        /// It copies the Table instance.
        /// </summary>
        /// <returns></returns>
        public override BlockAdv CopyBlock()
        {
            TableRowAdv newrow = null;
            TableCellAdv newcell = null;
            TableAdv table = CreateNewTable();
            foreach (TableRowAdv row in Rows)
            {
                newrow = row.CreateNewRow();
                foreach (TableCellAdv c in row.Cells)
                {
                    newcell=c.CreateNewCell(false,false);
                    foreach (BlockAdv b in c.Blocks)
                    {
                        newcell.Blocks.Add(b.CopyBlock());
                    }
                    newrow.Cells.Add(newcell);
                }
                table.Rows.Add(newrow);
            }
            return table;
        }

        /// <summary>
        /// It inserts the Row at the specified index.
        /// </summary>
        /// <param name="given"></param>
        /// <param name="index"></param>
        public void InsertRowAtIndex(TableRowAdv given,int index)
        {
            try
            {
                if (index == Rows.Count)
                {
                    this.Rows.Add(given);
                }
                else
                {
                    this.Rows.Insert(index, given);
                }

                MeasureElements();

                LayoutViewer.SetIsArrangedToFalse();

                LayoutViewer.SetPreviousBlocks();

                LayoutViewer.SetIsArrangedToFalse();

                ArrangeElements();
            }
            catch { }
        }

        /// <summary>
        /// It inserts the Column at the given index.
        /// </summary>
        /// <param name="index"></param>
        /// <returns></returns>
        public void InsertColumnAtIndex(int index,ref List<TableCellAdv> insertedcells)
        {
            foreach (TableRowAdv row in Rows)
            {
                bool isSet = false;
                if (index < TableHolder.Columns.Count)
                {
                    for (int i = 0; i < row.Cells.Count; i++)
                    {
                        TableCellAdv cell = row.Cells[i];
                        if (cell.ColumnIndex >= index)
                        {
                            TableCellAdv newcell = cell.CreateNewCell(true, true);
                            //if (index >= row.Cells.Count)
                            //{
                            //    row.Cells.Add(newcell);
                            //}
                            //else
                            //{
                            //    row.Cells.Insert(index, newcell);
                            //}

                            row.Cells.Insert(i, newcell);
                            insertedcells.Add(newcell);
                            isSet = true;
                            break;
                        }
                    }
                }
                if (!isSet)
                {
                    TableCellAdv lastcell = row.Cells.Last().CreateNewCell(true,true);
                    row.Cells.Add(lastcell);
                    insertedcells.Add(lastcell);
                }
            }
            MeasureElements();

            LayoutViewer.SetIsArrangedToFalse();

            LayoutViewer.SetPreviousBlocks();

            LayoutViewer.SetIsArrangedToFalse();

            ArrangeElements();
        }

        /// <summary>
        /// It updates the TableCells.
        /// </summary>
        internal void UpdateTableCells()
        {
            foreach (TableRowAdv row in this.Rows)
            {
                row.Cells.ToList().ForEach(c =>
                {
                    c.Arrange();
                    c.UpdateCellHeight();
                });
            }
        }
       

        internal void UpdateTableCells(int index)
        {
            foreach (TableRowAdv row2 in Rows)
            {
                if (Rows.IndexOf(row2) >= index)
                {
                    row2.Cells.ToList().ForEach(c =>
                        {
                            c.Arrange();
                            c.UpdateCellHeight();
                        });
                }
            }
        }

        /// <summary>
        /// 
        /// </summary>
        internal void PrepareBoxLines()
        {
            double maxheight = 0.0;
            foreach (List<ElementBox> boxes in TableElementBoxes)
            {
                foreach (ElementBox eBox in boxes)
                {
                    TableCellElementBox eCellBox = eBox as TableCellElementBox;
                    if (eCellBox != null)
                    {
                        if (eCellBox.HasRowSpan() && eCellBox.BottomCellBox != null)
                        {
                            int k = 0;
                            while (k < eCellBox.BottomCellBox.LineInfos.Count)
                            {
                                eCellBox.BottomCellBox.LineInfos.RemoveAt(k);
                            }
                        }
                    }
                }

                foreach (ElementBox e in boxes)
                {
                    TableCellElementBox cellbox = e as TableCellElementBox;
                    if (cellbox != null)
                    {
                        if (!cellbox.HasRowSpan())
                        {
                            maxheight = Math.Max(maxheight, cellbox.LineInfos.Sum<LineInfo>(l => l.Height));
                        }
                    }
                }

                foreach (ElementBox e2 in boxes)
                {
                    int i = 0;
                    double num = 0.0;
                    TableCellElementBox cellbox2 = e2 as TableCellElementBox;

                    if (cellbox2.HasRowSpan() && cellbox2.BottomCellBox != null)
                    {
                        while (i < cellbox2.LineInfos.Count)
                        {
                            LineInfo line = cellbox2.LineInfos[i];
                            num += line.Height;
                            if (num > Math.Ceiling(maxheight))
                            {
                                cellbox2.LineInfos.Remove(line);
                                cellbox2.BottomCellBox.LineInfos.Add(line);
                                continue;
                            }
                            i++;
                        }
                    }
                }
            }
        }

        internal void PrepareBoxLines(int index)
        {
            double maxheight = 0.0;
            foreach (List<ElementBox> boxes in TableElementBoxes)
            {
                if (TableElementBoxes.IndexOf(boxes) >= index)
                {
                    foreach (ElementBox eBox in boxes)
                    {
                        TableCellElementBox eCellBox = eBox as TableCellElementBox;
                        if (eCellBox != null)
                        {
                            if (eCellBox.HasRowSpan() && eCellBox.BottomCellBox != null)
                            {
                                int k = 0;
                                while (k < eCellBox.BottomCellBox.LineInfos.Count)
                                {
                                    eCellBox.BottomCellBox.LineInfos.RemoveAt(k);
                                }
                            }
                        }
                    }

                    foreach (ElementBox e in boxes)
                    {
                        TableCellElementBox cellbox = e as TableCellElementBox;
                        if (cellbox != null)
                        {
                            if (!cellbox.HasRowSpan())
                            {
                                maxheight = Math.Max(maxheight, cellbox.LineInfos.Sum<LineInfo>(l => l.Height));
                            }
                        }
                    }

                    foreach (ElementBox e2 in boxes)
                    {
                        int i = 0;
                        double num = 0.0;
                        TableCellElementBox cellbox2 = e2 as TableCellElementBox;

                        if (cellbox2.HasRowSpan() && cellbox2.BottomCellBox != null)
                        {
                            while (i < cellbox2.LineInfos.Count)
                            {
                                LineInfo line = cellbox2.LineInfos[i];
                                num += line.Height;
                                if (num > Math.Ceiling(maxheight))
                                {
                                    cellbox2.LineInfos.Remove(line);
                                    cellbox2.BottomCellBox.LineInfos.Add(line);
                                    continue;
                                }
                                i++;
                            }
                        }
                    }
                }
            }
        }

        /// <summary>
        /// It sets the ElementSize from the calculated size.
        /// </summary>
        internal void SetElementSize()
        {
            foreach (List<ElementBox> boxes in TableElementBoxes)
            {
                foreach (ElementBox box in boxes)
                {
                    if (box is TableCellElementBox)
                    {
                        box.ElementSize = new Size((box as TableCellElementBox).BaseCell.DesiredWidth, (box as TableCellElementBox).LineInfos.Sum<LineInfo>(l => l.Height) == 0.0 ?
                            (box as TableCellElementBox).BaseCell.CalculateEmptyLineHeight(BaseParent.CurrentInlineStyle) : (box as TableCellElementBox).LineInfos.Sum<LineInfo>(l => l.Height));
                    }
                    else if (box is ChildTableCellElementBox)
                    {
                        box.ElementSize = new Size(((box as ChildTableCellElementBox).GetParentCellBox() as TableCellElementBox).BaseCell.DesiredWidth, ((box as ChildTableCellElementBox).GetParentCellBox() as TableCellElementBox).BaseCell.CellHeight);
                    }
                }
            }
        }

        internal void SetElementSize(int index)
        {
            foreach (List<ElementBox> boxes in TableElementBoxes)
            {
                if (TableElementBoxes.IndexOf(boxes) >= index)
                {
                    foreach (ElementBox box in boxes)
                    {
                        if (box is TableCellElementBox)
                        {
                            box.ElementSize = new Size((box as TableCellElementBox).BaseCell.DesiredWidth, (box as TableCellElementBox).LineInfos.Sum<LineInfo>(l => l.Height) == 0.0 ?
                                (box as TableCellElementBox).BaseCell.CalculateEmptyLineHeight(BaseParent.CurrentInlineStyle) : (box as TableCellElementBox).LineInfos.Sum<LineInfo>(l => l.Height));
                        }
                        else if (box is ChildTableCellElementBox)
                        {
                            box.ElementSize = new Size(((box as ChildTableCellElementBox).GetParentCellBox() as TableCellElementBox).BaseCell.DesiredWidth, ((box as ChildTableCellElementBox).GetParentCellBox() as TableCellElementBox).BaseCell.CellHeight);
                        }
                    }

                }
            }
        }

        /// <summary>
        /// It copies from the Table's Properties.
        /// </summary>
        /// <returns></returns>
        internal TableAdv CreateNewTable()
        {
            TableAdv newtable = new TableAdv();
            newtable.Section = Section;
            newtable.BorderBrush = BorderBrush;
            newtable.BorderThickness = BorderThickness;
            newtable.LayoutViewer = LayoutViewer;
            newtable.IsInsideTable = IsInsideTable;
            newtable.AssociatedCell = AssociatedCell;
            newtable.LeftIndent = LeftIndent;
            newtable.Margin = Margin;
            newtable.RightIndent = RightIndent;
            return newtable;
        }
        
        /// <summary>
        /// It creates the new Table instance.
        /// </summary>
        /// <returns></returns>
        internal override BlockAdv CreateBlock()
        {
            TableAdv newtable = new TableAdv();
            newtable.BorderBrush = BorderBrush;
            newtable.BorderThickness = BorderThickness;
            newtable.IsInsideTable = IsInsideTable;
            newtable.AssociatedCell = AssociatedCell;
            newtable.LeftIndent = LeftIndent;
            newtable.Margin = Margin;
            newtable.RightIndent = RightIndent;
            return newtable;
        }

        /// <summary>
        /// It returns the Last Block in the Table.
        /// </summary>
        /// <returns></returns>
        internal BlockAdv GetLastBlockInLastCell()
        {
            if (Rows.Count > 0)
            {
                TableRowAdv lastrow = Rows[Rows.Count - 1];
                TableCellAdv lastcell = lastrow.Cells[lastrow.Cells.Count - 1];
                return lastcell.Blocks[lastcell.Blocks.Count - 1];
            }
            return null;
        }

        /// <summary>
        /// It returns the first block in the table.
        /// </summary>
        /// <returns></returns>
        internal BlockAdv GetFirstBlockInFirstCell()
        {
            if (Rows.Count > 0)
            {
                TableRowAdv firstrow = Rows[0];
                if (firstrow.Cells.Count > 0)
                {
                    TableCellAdv firstcell = firstrow.Cells[0];
                    if (firstcell.Blocks.Count == 0)
                        return null;
                    return firstcell.Blocks[0];
                }
            }
            return null;
        }
                       
    }

}
