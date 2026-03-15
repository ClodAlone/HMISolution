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

namespace Syncfusion.Windows.Tools.Controls
{
    public class InsertedTableHistory :HistoryInfo
    {
        int index = 0;
        int row = 0;
        int column = 0;

        public InsertedTableHistory()
        {
            IsInsertedTableHistory = true;
        }

        internal int TableIndex
        {
            get
            {
                return index;
            }
            set
            {
                index = value;
            }
        }

        internal int RowsCount
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

        internal int ColumnsCount
        {
            get
            {
                return column;
            }
            set
            {
                column = value;
            }
        }
    }
}
