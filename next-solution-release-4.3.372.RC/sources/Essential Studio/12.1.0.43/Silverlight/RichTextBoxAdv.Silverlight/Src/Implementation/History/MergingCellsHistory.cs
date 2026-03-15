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
    public class MergedCellsHistory :HistoryInfo
    {
        int rowspan = 0;
        int columnspan = 0;
        List<PreservedCellsInfo> mergedcells = new List<PreservedCellsInfo>();
        List<DeletedRowHistory> deletedrows = new List<DeletedRowHistory>();

        public MergedCellsHistory()
        {
            IsMergedCellsHistory = true;
        }

        internal int AffectedRowSpan
        {
            get
            {
                return rowspan;
            }
            set
            {
                rowspan = value;
            }
        }

        internal int AffectedColumnSpan
        {
            get
            {
                return columnspan;
            }
            set
            {
                columnspan = value;
            }
        }

        internal List<PreservedCellsInfo> MergedCellsInfo
        {
            get
            {
                return mergedcells;
            }
            set
            {
                mergedcells = value;
            }
        }

        internal List<DeletedRowHistory> DeletedRowsHistory
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
    }
}
