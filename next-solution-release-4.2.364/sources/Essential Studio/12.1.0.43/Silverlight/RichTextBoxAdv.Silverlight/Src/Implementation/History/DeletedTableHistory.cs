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
    internal class DeletedTableHistory :HistoryInfo
    {
        List<PreservedCellsInfo> deletedcells = new List<PreservedCellsInfo>();
        TextPosition positionindelted;
        int rows = 0;
        int columns = 0;

        public DeletedTableHistory()
        {
            IsDeletedTableHistory = true;
        }

        /// <summary>
        /// 
        /// </summary>
        internal List<PreservedCellsInfo> DeletedCellsInfo
        {
            get
            {
                return deletedcells;
            }
            set
            {
                deletedcells = value;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        internal TextPosition PositionInDeletedTable
        {
            get
            {
                return positionindelted;
            }
            set
            {
                positionindelted = value;
            }

        }


        internal int RowsCount
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

        internal int ColumnCount
        {
            get
            {
                return columns;
            }
            set
            {
                columns = value;
            }
        }

        internal BlockAdv PreservedBlock
        {
            get;
            set;
        }

        internal int DeletedTableIndex
        {
            get;
            set;
        }

        internal TableAdv DeletedTable
        {
            get;
            set;
        }
    }
}
