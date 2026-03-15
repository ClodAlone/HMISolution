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
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Ink;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;
using System.Collections.Generic;

namespace Syncfusion.Windows.Tools.Controls
{
    internal class DeletedRowHistory :HistoryInfo
    {
        int rowindex = 0;
        bool candeltetable = false;
        List<TableRowAdv> deletedrows = new List<TableRowAdv>();
        List<TableCellAdv> rowspanaffected = new List<TableCellAdv>();
        TableAdv table = null;
        int tableindex = 0;
        int rowscount = 0;

        public DeletedRowHistory()
        {
            IsDeletedRowHistory = true;
        }

        public DeletedRowHistory(int index, List<TableRowAdv> deleted):this()
        {
            deletedrows = deleted;
            rowindex = index;
        }

        /// <summary>
        /// 
        /// </summary>
        internal int RowIndex
        {
            get
            {
                return rowindex;
            }
            set
            {
                rowindex = value;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        internal List<TableRowAdv> DeletedRows
        {
            get
            {
                return deletedrows;
            }
            set
            {
                deletedrows = value;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        internal List<TableCellAdv> RowSpanAffectedCells
        {
            get
            {
                return rowspanaffected;
            }
            set
            {
                rowspanaffected = value;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        internal bool CanDeleteTable
        {
            get
            {
                return candeltetable;
            }
            set
            {
                candeltetable = value;
            }
        }


        /// <summary>
        /// 
        /// </summary>
        internal TableAdv DeletedTable
        {
            get
            {
                return table;
            }
            set
            {
                table = value;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        internal int DeletedTableIndex
        {
            get
            {
                return tableindex;
            }
            set
            {
                tableindex = value;
            }
        }


        internal int RowsCount
        {
            get
            {
                return rowscount;
            }
            set
            {
                rowscount = value;
            }
        }
    }
}
