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
    internal sealed class PreservedCellsInfo
    {
        private int rowindex = 0;
        private int columnindex = 0;
        private BlockCollection<BlockAdv> blocks = new BlockCollection<BlockAdv>();
        private bool isColumnSpanChanged = false;
        TableCellAdv cell = null;
        Color background = Colors.White;

        /// <summary>
        /// 
        /// </summary>
        public PreservedCellsInfo()
        {

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
        internal int ColumnIndex
        {
            get
            {
                return columnindex;
            }
            set
            {
                columnindex = value;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        internal BlockCollection<BlockAdv> Blocks
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
        /// 
        /// </summary>
        internal bool IsColumnSpanChanged
        {
            get
            {
                return isColumnSpanChanged;
            }
            set
            {
                isColumnSpanChanged = value;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        internal TableCellAdv TableCell
        {
            get
            {
                return cell;
            }
            set
            {
                cell = value;
            }
        }

        internal Color Background
        {
            get
            {
                return background; 
            }
            set
            {
                background = value;
            }
        }

        internal int ColumnSpan
        {
            get;
            set;
        }

        internal int RowSpan
        {
            get;
            set;
        }
    }
}
