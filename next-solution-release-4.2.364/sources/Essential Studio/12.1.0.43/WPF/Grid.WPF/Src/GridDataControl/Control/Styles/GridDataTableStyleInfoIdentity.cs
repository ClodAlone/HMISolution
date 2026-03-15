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
    using Syncfusion.Windows.Controls.Cells;
    using System.ComponentModel;
    using Syncfusion.Windows.Data;

    public class GridDataTableStyleInfoIdentity : GridStyleInfoIdentity
    {
        public GridDataTableStyleInfoIdentity(GridDataVolatileCellStyles data, int rowIndex, int colIndex)
            : base((GridVolatileCellStyles)data, rowIndex, colIndex)
        {
            this.Data = data;
        }

        public GridDataTableStyleInfoIdentity(GridDataVolatileCellStyles data, RowColumnIndex pos)
            : base((GridVolatileCellStyles)data, pos)
        {
            this.Data = data;
        }

        public GridDataTableStyleInfoIdentity(GridDataVolatileCellStyles data, int rowIndex, int colIndex, bool offline)
            : base((GridDataVolatileCellStyles)data, rowIndex, colIndex, offline)
        {
            this.Data = data;
        }

        public GridDataTableStyleInfoIdentity(GridDataVolatileCellStyles data, RowColumnIndex pos, bool offline)
            : base((GridDataVolatileCellStyles)data, pos, offline)
        {
            this.Data = data;
        }

        public new GridDataVolatileCellStyles Data
        {
            get;
            internal set;
        }

        /// <summary>
        /// Gets the type of the table cell.
        /// </summary>
        /// <value>The type of the table cell.</value>
        public GridDataTableCellType TableCellType
        {
            get;
            internal set;
        }

        /// <summary>
        /// Gets the visible column.
        /// </summary>
        /// <value>The column.</value>
        public GridDataVisibleColumn Column
        {
            get;
            internal set;
        }

        /// <summary>
        /// Gets the record entry.
        /// </summary>
        /// <value>The record entry.</value>
        public GridDataRecord RecordEntry
        {
            get;
            internal set;
        }

        /// <summary>
        /// Gets the record.
        /// </summary>
        /// <value>The record.</value>
        public object Record
        {
            get
            {
            //    if (this.isDirty && this.RecordIndex > -1 && this.RecordIndex < this.GridModel.View.Records.Count-1 )
            //    {
            //        var rec = this.GridModel.View.Records.GetItemAt(this.RecordIndex);
            //        this.cachedRecord = rec;
            //        this.isDirty = false;
            //    }
                if (this.RecordEntry != null)
                {
                    return this.RecordEntry.Data;
                }
                return null;

                //return this.cachedRecord;
            }
        }

        private int recordIndex = -1;
        /// <summary>
        /// Gets the index of the record.
        /// </summary>
        /// <value>The index of the record.</value>
        public int RecordIndex
        {
            get
            {
                if (this.recordIndex == -1)
                {
                    this.recordIndex = this.GridModel.ResolveIndexToRecordPosition(this.RowIndex);
                }

                return this.recordIndex;
            }            
        }

        /// <summary>
        /// Gets the group.
        /// </summary>
        /// <value>The group.</value>
        public Group Group
        {
            get;
            internal set;
        }

        /// <summary>
        /// Gets the summary row.
        /// </summary>
        /// <value>The summary row.</value>
        public GridDataSummaryRow SummaryRow
        {
            get;
            internal set;
        }

        /// <summary>
        /// Gets the summary column.
        /// </summary>
        /// <value>The summary column.</value>
        public GridDataSummaryColumn SummaryColumn
        {
            get;
            internal set;
        }

        /// <summary>
        /// Gets the stack header row.
        /// </summary>
        /// <value>The stack header row.</value>
        public GridDataStackedHeaderRow StackHeaderRow
        {
            get;
            internal set;
        }

        /// <summary>
        /// Gets the stack header column.
        /// </summary>
        /// <value>The stack header column.</value>
        public GridDataStackedHeaderColumn StackHeaderColumn
        {
            get;
            internal set;
        }

        /// <summary>
        /// Gets the grid model.
        /// </summary>
        /// <value></value>
        public new GridDataTableModel GridModel
        {
            get
            {
                return (GridDataTableModel)this.Data.Host;
            }
        }

        public override void Dispose()
        {
            this.Column = null;
            this.Data = null;
            
            base.Dispose();
        }
    }
}
