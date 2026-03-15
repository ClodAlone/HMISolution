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
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Ink;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;
using System.Windows.Markup;
using System.Collections.Generic;
using Syncfusion.Windows.Shared;
using System.ComponentModel;

namespace Syncfusion.Windows.Tools.Controls
{
    [ContentProperty("Cells")]
    public class TableRowAdv :INotifyPropertyChanged
    {
        double rowheight = 20.0;
        TableCellAdvCollection cells = new TableCellAdvCollection();
        List<ElementBox> rowelementboxes = new List<ElementBox>();
        Color bgcolor = Color.FromArgb(0, 0, 0, 0);
        TextAlignment align = TextAlignment.Left;

        public Color Background
        {
            get
            {
                return bgcolor;
            }
            set
            {
                bgcolor = value;
                OnPropertyChanged("Background");
            }
        }
        
        public TextAlignment TextAlignment
        {
            get
            {
                return align;
            }
            set
            {
                align = value;
                OnPropertyChanged("TextAlignment");
            }
        }

        internal List<ElementBox> RowElementBoxes
        {
            get
            {
                return rowelementboxes;
            }
            set
            {
                rowelementboxes = value;
            }
        }

        public TableCellAdvCollection Cells
        {
            get
            {
                return cells;
            }
            set
            {
                cells = value;
            }
        }

        public TableAdv Owner
        {
            get;
            set;
        }

        internal double RowHeight
        {
            get
            {
                return Cells[0].CellHeight;
            }
            set
            {
                rowheight = value;
            }
        }

        public TableRowAdv()
        {
            Cells = new TableCellAdvCollection();
            RowElementBoxes = new List<ElementBox>();
        }

        internal void MeasureElements()
        {
            RowElementBoxes.Clear();

            int columnindx = 0;

            foreach (TableCellAdv cell in Cells)
            {
                cell.OwnerRow = this;
                cell.ColumnIndex = columnindx;
                cell.RowIndex = Owner.Rows.IndexOf(this);
                cell.MeasureElements();
                columnindx++;
            }
            if(Owner !=null)
                this.Owner.TableElementBoxes.Add(this.RowElementBoxes);
        }
        
        internal void ValidateTableCellElementBox()
        {
            foreach (TableCellElementBox box in this.RowElementBoxes)
            {
                box.ElementSize = new Size(box.BaseCell.DesiredWidth, box.BaseCell.CellHeight);
            }
        }

        internal void ArrangeElements()
        {
            for (int i = 0; i < Cells.Count; i++)
            {
                Cells[i].ArrangeElements();
            }
        }

        internal void UpdateRowHeight()
        {
            Cells.ToList().ForEach(cell =>
                {
                    cell.MeasureCellHeight();
                    RowHeight = Math.Max(RowHeight, cell.CellHeight);
                });
        }

        internal TableRowAdv CreateNewRow()
        {
            TableRowAdv newrow = new TableRowAdv();
            //newrow.Cells = Cells;
            return newrow;
        }
        
        internal TableRowAdv CopyFromGivenRow(bool clearRowSpans,bool emptyblocks)
        {
            TableRowAdv newrow = this.CreateNewRow();
            //newrow.Owner = (TableAdv)Owner.CopyBlock();
            newrow.Owner = Owner;
            foreach (TableCellAdv cell in this.Cells)
            {
                TableCellAdv newcell = cell.CreateNewCell(clearRowSpans, false);
                if (!emptyblocks)
                {
                    foreach (BlockAdv b in cell.Blocks)
                    {
                        newcell.Blocks.Add(b.CopyBlock());
                    }
                }
                newrow.Cells.Add(newcell);
            }
            return newrow;
        }

        public event PropertyChangedEventHandler PropertyChanged;

        private void OnPropertyChanged(string propname)
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(propname));
            }
        }
    }
}
