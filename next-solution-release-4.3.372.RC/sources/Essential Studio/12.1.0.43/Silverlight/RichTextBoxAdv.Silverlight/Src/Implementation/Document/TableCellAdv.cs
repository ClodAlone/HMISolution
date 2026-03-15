#region Copyright Syncfusion Inc. 2001 - 2014
// Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Any infringement will be prosecuted under
// applicable laws. 
#endregion
using System;
using System.Net;
using System.Windows;
using System.Linq;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Ink;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;
using System.Windows.Markup;
using System.Collections.Generic;
using System.ComponentModel;
using Syncfusion.Windows.Tools.Controls;
using System.Collections.ObjectModel;
using System.Collections;
using System.Collections.Specialized;

namespace Syncfusion.Windows.Tools.Controls
{
    [ContentProperty("Blocks")]
    public class TableCellAdv :DependencyObject
    {
        #region Members

        private BlockCollection<BlockAdv> blocks;
        private double desiredwidth = 0.0;
        private double height = 0.0;
        private int rowspan = 1;
        private int columnspan = 1;
        private TableAdv ownertable = null;
        private TableCellElementBox cellbox = null;
        private ObservableCollection<LineInfo> lineinfos = new ObservableCollection<LineInfo>();
        private List<ElementBox> rowspanboxes = new List<ElementBox>();
        private TableRowAdv row = null;
        private bool isSelected = false;
        private Color color = Color.FromArgb(0, 0, 0, 0);

        #endregion

        /// <summary>
        /// It gets/sets the OwnerRow
        /// </summary>
        internal TableRowAdv OwnerRow
        {
            get
            {
                return row;
            }
            set
            {
                row = value;
            }
        }

        /// <summary>
        /// It gets/sets the Table instance.
        /// </summary>
        internal TableAdv OwnerTable
        {
            get
            {
                if (OwnerRow != null)
                    return this.OwnerRow.Owner;
                else
                    return ownertable;
            }
            set
            {
                ownertable = value;
            }
        }

        /// <summary>
        /// Gets the Block Collection
        /// </summary>
        public BlockCollection<BlockAdv> Blocks
        {
            get
            {
                return blocks;
            }
            set
            {
                blocks = value;
            }
        }

        /// <summary>
        /// It gets/sets Height of the Cell
        /// </summary>
        [BrowsableAttribute(false)]
        internal double CellHeight
        {
            get
            {
                return height;
            }
            set
            {
                height = value;
            }
        }

        /// <summary>
        /// It gets/sets the Width of the Cell
        /// </summary>
        public double DesiredWidth
        {
            get
            {
                return desiredwidth;
            }
            set
            {
                desiredwidth = value;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        internal List<ElementBox> RowSpannedBoxes
        {
            get
            {
                return rowspanboxes;
            }
            set
            {
                rowspanboxes = value;
            }
        }

        /// <summary>
        /// It gets /sets the ParentCellBox
        /// </summary>
        internal TableCellElementBox CellElementBox
        {
            get
            {
                return cellbox;
            }
            set
            {
                cellbox = value;
                cellbox.RowSpan = RowSpan;
                cellbox.ColumnSpan = ColumnSpan;
                cellbox.RowIndex = RowIndex;
                cellbox.ColumnIndex = ColumnIndex;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        internal bool IsLastCell
        {
            get
            {
                TableRowAdv row=null;
                TableCellAdv cell = null;
                if (OwnerTable.Rows.Count > 0)
                {
                    row = OwnerTable.Rows[OwnerTable.Rows.Count - 1];
                    if (row == OwnerRow)
                    {
                        if (row.Cells.Count > 0)
                        {
                            cell = row.Cells[row.Cells.Count - 1];
                            if (cell != null)
                                return true;
                        }
                    }
                }
                return false;
            }
        }

        /// <summary>
        /// It gets/sets the TableCell selection
        /// </summary>
        internal bool IsSelected
        {
            get
            {
                return isSelected;
            }
            set
            {
                isSelected = value;
            }
        }

        /// <summary>
        /// It gets the RowIndex of the Cell
        /// </summary>
        public int RowIndex
        {
            get;
            internal set;
        }

        /// <summary>
        /// It gets ColumnIndex of the Cell.
        /// </summary>
        public int ColumnIndex
        {
            get;
            internal set;
        }

        /// <summary>
        /// It gets/sets the RowSpan of the Cell.
        /// </summary>
        public int RowSpan
        {
            get
            {
                return rowspan;
            }
            set
            {
                if (value < 1)
                {
                    throw new InvalidOperationException("RowSpan value must be greater than 1");
                }
                rowspan = value;
            }
        }

        /// <summary>
        /// It gets/sets the ColumnSpan of the Cell.
        /// </summary>
        public int ColumnSpan
        {
            get
            {
                return columnspan;
            }
            set
            {
                if (value < 1)
                {
                    throw new InvalidOperationException("ColumnSpan value must be greater than 1");
                }
                columnspan = value;
            }
        }
        
        /// <summary>
        /// 
        /// </summary>
        internal TableCellMode TableCellMode
        {
            get;
            set;
        }
        
        /// <summary>
        /// It gets/sets the Lines in the TableCell.
        /// </summary>
        internal ObservableCollection<LineInfo> LineInfos
        {
            get
            {
                return lineinfos;
            }
            set
            {
                lineinfos = value;
            }

        }

        /// <summary>
        /// It gets or sets the Background color of the Cell
        /// </summary>
        public Color Background
        {
            get
            {
                return color;
            }
            set
            {
                color = value;
            }
        }

        Color _columnbg = Color.FromArgb(0, 0, 0, 0);
        internal Color ColumnBackground
        {
            get
            {
                return _columnbg;
            }
            set
            {
                _columnbg = value;
            }
        }

        TextAlignment _columnAlign = TextAlignment.Left;

        internal TextAlignment ColumnAlignment
        {
            get
            {
                return _columnAlign;
            }
            set
            {
                _columnAlign = value;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        internal Thickness CellMargin
        {
            get { return (Thickness)GetValue(CellMarginProperty); }
            set { SetValue(CellMarginProperty, value); }
        }

        // Using a DependencyProperty as the backing store for CellMargin.  This enables animation, styling, binding, etc...
        internal static readonly DependencyProperty CellMarginProperty =
            DependencyProperty.Register("CellMargin", typeof(Thickness), typeof(TableCellAdv), new PropertyMetadata(new Thickness(5)));

        public TextAlignment TextAlignment
        {
            get { return (TextAlignment)GetValue(TextAlignmentProperty); }
            set { SetValue(TextAlignmentProperty, value); }
        }

        // Using a DependencyProperty as the backing store for TextAlignment.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty TextAlignmentProperty =
            DependencyProperty.Register("TextAlignment", typeof(TextAlignment), typeof(TableCellAdv), new PropertyMetadata(TextAlignment.Left));


        internal TableColumnAdv OwnerColumn
        {
            get
            {
                return OwnerTable.TableHolder.Columns[ColumnIndex];
            }
        }

        /// <summary>
        /// 
        /// </summary>
        public TableCellAdv()
        {
            blocks = new BlockCollection<BlockAdv>();
            LineInfos.CollectionChanged += new System.Collections.Specialized.NotifyCollectionChangedEventHandler(LineInfos_CollectionChanged);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void LineInfos_CollectionChanged(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
        {
            IList lineinfos = e.NewItems as IList;
            int lineindex=e.NewStartingIndex;
            LineInfo previousline=null;
            if (e.Action == NotifyCollectionChangedAction.Remove || e.Action == NotifyCollectionChangedAction.Reset)
            {
                lineinfos = e.OldItems;
                lineindex = e.OldStartingIndex;
            }
            if (lineinfos != null)
            {
                if (lineindex > 0)
                {
                    previousline = LineInfos[lineindex - 1];
                }

                foreach (LineInfo ln in lineinfos)
                {
                    if (e.Action == NotifyCollectionChangedAction.Add)
                    {
                        if (lineindex < CellElementBox.LineInfos.Count - 1 && CellElementBox.LineInfos.Count > 0)
                        {
                            CellElementBox.LineInfos.Insert(lineindex, ln);
                        }
                        else
                        {
                            CellElementBox.LineInfos.Add(ln);
                        }
                    }
                    else if (e.Action == NotifyCollectionChangedAction.Remove || e.Action == NotifyCollectionChangedAction.Reset)
                    {
                        if (CellElementBox.LineInfos.Count > 0)
                        {
                            if (CellElementBox.LineInfos.Contains(ln))
                            {
                                CellElementBox.LineInfos.Remove(ln);
                            }
                            else
                            {
                                TableCellElementBox cellbox=CellElementBox;

                                while (cellbox.BottomCellBox !=null)
                                {
                                    if (cellbox.BottomCellBox.LineInfos.Contains(ln))
                                    {
                                        cellbox.BottomCellBox.LineInfos.Remove(ln);
                                    }
                                    cellbox = cellbox.BottomCellBox;
                                }
                            }
                        }
                        for (int i = 0; i < CellElementBox.LineInfos.Count; i++)
                        {
                            LineInfo line=CellElementBox.LineInfos[i];
                            if (line.HasChildBoxes)
                                CellElementBox.LineInfos.Remove(line);
                        }
                    }
                }
            }
        }

        /// <summary>
        /// It measures the TableCell.
        /// </summary>
        internal void MeasureElements()
        {
            TableCellElementBox cellelementbox = new TableCellElementBox();
            cellelementbox.Background = Background;
            this.CellElementBox = cellelementbox;
            
            if (this.Blocks.Count == 0)
            {
                this.Blocks.Add(CreateEmptyBlock());
            }
            foreach (var block in Blocks)
            {
                block.IsInsideTable = true;
                block.AssociatedCell = this;
                block.MeasureElements();
                block.LinkElementBoxes();
            }

            UpdateCellHeight();
            cellelementbox.BaseCell = this;
            cellelementbox.CellBlocks = Blocks;
            cellelementbox.DesiredWidth = DesiredWidth;
            cellelementbox.ElementSize = new Size(this.DesiredWidth, this.CellHeight);            
            this.OwnerRow.RowElementBoxes.Add(cellelementbox);
        }

        /// <summary>
        /// It arranges the TableCell.
        /// </summary>
        internal void ArrangeElements()
        {
            if (this.Blocks.Count > 0)
            {
                foreach (BlockAdv b in Blocks)
                {
                    b.ArrangeElements();
                }
            }
        }

        /// <summary>
        /// 
        /// </summary>
        internal void ResetBoxLineInfos()
        {
            double lineheight = 0.0;
            List<LineInfo> boxlines = CellElementBox.LineInfos.ToList();
            List<LineInfo> temp = new List<LineInfo>();
            if (boxlines.Count > 0 && HasRowSpan())
            {
                foreach (LineInfo line in boxlines)
                {
                    lineheight += Math.Ceiling(line.Height);
                    if (lineheight > Math.Ceiling(CellElementBox.ElementSize.Height))
                    {
                        CellElementBox.LineInfos.Remove(line);
                        temp.Add(line);
                    }
                }
                foreach (LineInfo lin in temp)
                {
                    CellElementBox.BottomCellBox.LineInfos.Add(lin);
                }
            }
        }
        
        /// <summary>
        /// It measures the CellHeight.
        /// </summary>
        /// <returns></returns>
        internal double MeasureCellHeight()
        {
            if (Blocks.Count > 0)
            {
                foreach (BlockAdv b in Blocks)
                {
                    if (b is ParagraphAdv)
                    {
                        if (b.Inlines == null)
                        {
                            return CalculateEmptyLineHeight(OwnerTable.BaseParent.CurrentInlineStyle);
                        }
                    }
                }
                if (Blocks.Count == 1)
                {
                    if (Blocks[0].Inlines != null && Blocks[0].Inlines.Count == 0)
                        return CalculateEmptyLineHeight(OwnerTable.BaseParent.CurrentInlineStyle) ;
                }
                if (CellElementBox != null)
                    return CellElementBox.LineInfos.Sum<LineInfo>(l => l.Height);
            }
            return 0.0;
        }

        /// <summary>
        /// It calculates the Empty line height.
        /// </summary>
        /// <param name="style"></param>
        /// <returns></returns>
        internal double CalculateEmptyLineHeight(InlineStyle style)
        {
            double Height=0.0;
            if (style != null)
            {
                Size size = TextHelper.MeasureText(" ", style);
                Height = size.Height;

                double Offset = (Height * 20) / 100;
                Height = Height + Offset;
            }
            return Height;
        }

        /// <summary>
        /// It updates the Height of the Cell.
        /// </summary>
        internal void UpdateCellHeight()
        {
            CellHeight = MeasureCellHeight();
        }

        /// <summary>
        /// 
        /// </summary>
        internal void Arrange()
        {
            Blocks.ToList().ForEach(block =>
                {
                    block.CreateNewLines = true;
                    if (DesiredWidth != 0.0)
                    {
                        block.Arrange(new Size(DesiredWidth - 10d, CellHeight));
                    }
                });
        }
        
        /// <summary>
        /// 
        /// </summary>
        /// <param name="clearRowspan"></param>
        /// <param name="clearcolumnspan"></param>
        /// <returns></returns>
        internal TableCellAdv CreateNewCell(bool clearRowspan,bool clearcolumnspan)
        {
            TableCellAdv newcell = new TableCellAdv();
            newcell.Background = Background;
            newcell.CellElementBox = CellElementBox;
            newcell.RowIndex = RowIndex;
            newcell.ColumnIndex = ColumnIndex;
            newcell.CellHeight = CellHeight;
            newcell.ColumnSpan = ColumnSpan;
            newcell.DesiredWidth = DesiredWidth;
            //newcell.OwnerRow = OwnerRow;
            //newcell.OwnerTable = (TableAdv)OwnerTable.CopyBlock();
            if (clearRowspan)
            {
                newcell.RowSpan = 1;
            }
            else
            {
                newcell.RowSpan = RowSpan;
            }

            if (clearcolumnspan)
            {
                newcell.ColumnSpan = 1;
            }
            else
            {
                newcell.ColumnSpan = ColumnSpan;
            }
            newcell.TableCellMode = TableCellMode;
            return newcell;
        }
        
        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        internal bool HasColumnSpan()
        {
            if (this.ColumnSpan > 1)
                return true;
            return false;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        internal bool HasRowSpan()
        {
            if (this.RowSpan > 1)
            {
                return true;
            }
            return false;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        internal TableCellAdv GetNextCell()
        {
            TableRowAdv nextrow=null;
            if (OwnerRow.Cells.IndexOf(this) != OwnerRow.Cells.Count - 1)
            {
                return OwnerRow.Cells[OwnerRow.Cells.IndexOf(this) + 1];
            }
            else
            {
                int index = OwnerTable.Rows.IndexOf(OwnerRow);
                if (index != OwnerTable.Rows.Count - 1)
                {
                    nextrow= OwnerTable.Rows[index + 1];
                    if (nextrow != null)
                    {
                        return nextrow.Cells[0];
                    }
                }
            }
            return null;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        internal TableCellAdv GetPreviousCell()
        {
            TableRowAdv previousrow = null;
            if (OwnerRow.Cells.IndexOf(this) != 0)
            {
                return OwnerRow.Cells[OwnerRow.Cells.IndexOf(this) - 1];
            }
            else
            {
                int Index = OwnerTable.Rows.IndexOf(OwnerRow);
                if (Index != 0)
                {
                    previousrow = OwnerTable.Rows[Index - 1];
                    if (previousrow != null)
                    {
                        return previousrow.Cells[previousrow.Cells.Count - 1];
                    }
                }
            }
            return null;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        internal bool HasEmptyBlocks()
        {
            foreach (var block in Blocks)
            {
                if (block.Inlines.Count == 0)
                    return true;
            }
            return false;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        internal BlockAdv CreateEmptyBlock()
        {
            ParagraphAdv currentparagraph = new ParagraphAdv();
            currentparagraph.IsInsideTable = true;
            currentparagraph.AssociatedCell = this;
            return currentparagraph;
        }
        
    }
}

