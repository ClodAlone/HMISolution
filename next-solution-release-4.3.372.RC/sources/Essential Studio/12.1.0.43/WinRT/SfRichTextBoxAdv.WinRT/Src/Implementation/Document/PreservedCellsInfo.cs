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
using System.Windows.Input;
#if WPF
using System.Windows.Media;
#else
using Windows.UI;
#endif

#if WPF
namespace Syncfusion.Windows.Tools.RichTextBoxAdv
#else
namespace Syncfusion.UI.Xaml.RichTextBoxAdv
#endif
{
    internal sealed class PreservedCellsInfo
    {
        private int rowindex = 0;
        private int columnindex = 0;
        private BlockAdvCollection blocks = new BlockAdvCollection(null);
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
        internal BlockAdvCollection Blocks
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
