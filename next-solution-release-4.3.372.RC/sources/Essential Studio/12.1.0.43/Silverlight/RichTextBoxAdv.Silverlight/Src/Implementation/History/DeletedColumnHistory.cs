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
    internal class DeletedColumnHistory :HistoryInfo
    {
        bool candeltetable = false;
        List<PreservedCellsInfo> deletedcells = new List<PreservedCellsInfo>();
        List<DeletedRowHistory> deletedrows=new List<DeletedRowHistory>();
        TableAdv table = null;
        int index = 0;

        public DeletedColumnHistory()
        {
            IsDeletedColumnHistory = true;
        }

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
        internal List<DeletedRowHistory> DeletedRows
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
                return index;
            }
            set
            {
                index = value;
            }
        }
    }
}
