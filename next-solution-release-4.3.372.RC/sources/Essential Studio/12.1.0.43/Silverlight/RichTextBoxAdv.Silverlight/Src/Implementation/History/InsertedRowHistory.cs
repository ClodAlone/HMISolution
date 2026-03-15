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
    internal class InsertedRowHistory :HistoryInfo
    {
        int rows = 0;
        RowPlacement rowplacement;
        int rowindex = 0;

        public InsertedRowHistory()
        {
            IsInsertedRowHistory = true;
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

        internal RowPlacement RowPlacement
        {
            get
            {
                return rowplacement;
            }
            set
            {
                rowplacement = value;
            }
        }
    }
}
