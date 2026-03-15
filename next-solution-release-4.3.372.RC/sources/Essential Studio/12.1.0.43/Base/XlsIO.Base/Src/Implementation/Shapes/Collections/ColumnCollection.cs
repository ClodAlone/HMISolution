#region Copyright Syncfusion Inc. 2001 - 2014
// Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Any infringement will be prosecuted under
// applicable laws. 
#endregion
using System;
using System.Collections.Generic;
using System.Text;
using System.Collections;

namespace Syncfusion.XlsIO.Implementation
{
    internal class ColumnCollection : List<Column>
    {
        internal Column column;
        private double defaultWidth;
        private WorksheetImpl workSheet;

        internal double Width
        {
            get
            {
                return this.defaultWidth;
            }
            set
            {
                this.defaultWidth = value;
            }
        }

        internal ColumnCollection(WorksheetImpl workSheet, double defaultWidth)
        {
            this.workSheet = workSheet;
            this.defaultWidth = defaultWidth;
        }

        public Column GetColumnByIndex(int index)
        {
            return (Column)this[index];
        }
        internal Column GetOrCreateColumn()
        {
            if (this.column == null)
            {
                this.column = new Column(0, this.workSheet, this.defaultWidth);
            }
            return this.column;
        }
        public double GetWidth(int colIndex, bool isDefaultWidth)
        {
            if ((this.column == null) || (this.column.Index > colIndex))
            {
                return this.defaultWidth;
            }
            if (isDefaultWidth)
            {
                return this.column.defaultWidth;
            }
            if (!this.column.IsHidden)
            {
                return this.column.defaultWidth;
            }
            return 0.0;
        }
        public int GetWidth(int minCol, int maxCol, bool isDefaultWidth, bool isLayout)
        {
            bool flag = this.workSheet.View == Syncfusion.XlsIO.SheetView.PageLayout;
            double num = (!isLayout || !flag) ? 1.0 : 1.05;
            int num2 = 0;
            for (int i = minCol; i <= maxCol; i++)
            {
                double num4 = this.defaultWidth;
                if ((this.column != null) && (this.column.Index <= i))
                {
                    if (isDefaultWidth)
                    {
                        num4 = this.column.defaultWidth;
                    }
                    else
                    {
                        num4 = this.column.IsHidden ? 0.0 : this.column.defaultWidth;
                    }
                }
                num2 += WorksheetImpl.CharacterWidth(num4, this.workSheet.GetAppImpl());
            }
            return (int)((num2 * num) + 0.5);
        }
        internal Column AddColumn(int index)
        {
            Column column = new Column((short)index, this.workSheet, this.defaultWidth);
            this.Add(column);
            return column;
        }
        public bool GetColumnIndex(int columnIndex, out int arrIndex)
        {
            if (base.Count == 0)
            {
                arrIndex = 0;
                return false;
            }
            int num = 0;
            int num2 = base.Count - 1;
            int num3 = 0;
            Column column = null;
            while (num <= num2)
            {
                num3 = (num + num2) / 2;
                column = (Column)this[num3];
                if (column.Index == columnIndex)
                {
                    arrIndex = num3;
                    return true;
                }
                if (column.Index < columnIndex)
                {
                    num = num3 + 1;
                }
                else
                {
                    num2 = num3 - 1;
                }
            }
            if (column.Index < columnIndex)
            {
                arrIndex = num3 + 1;
            }
            else
            {
                arrIndex = num3;
            }
            return false;
        }
        
    }
}
