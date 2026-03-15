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
namespace Syncfusion.Windows.Tools.RichTextBoxAdv
#else
namespace Syncfusion.UI.Xaml.RichTextBoxAdv
#endif
{
    internal class TableHolder
    {
        #region Fields
        private TableColumnAdvCollection tablecolumns = new TableColumnAdvCollection();
        #endregion

        #region Properties
        /// <summary>
        /// Gets or sets the columns.
        /// </summary>
        /// <value>
        /// The columns.
        /// </value>
        public TableColumnAdvCollection Columns
        {
            get
            {
                return tablecolumns;
            }
            set
            {
                tablecolumns = value;
            }
        }
        #endregion

        #region Constructor
        /// <summary>
        /// Initializes a new instance of the <see cref="TableHolder"/> class.
        /// </summary>
        public TableHolder()
        {
        }
        #endregion

        #region Implementations
        /// <summary>
        /// Gets the width of the previous spanned cell.
        /// </summary>
        /// <param name="prevColumnIndex">Index of the prev column.</param>
        /// <param name="curColumnIndex">Index of the cur column.</param>
        /// <returns></returns>
        internal double GetPreviousSpannedCellWidth(int prevColumnIndex, int curColumnIndex)
        {
            double width = 0;
            for (int i = prevColumnIndex; i < curColumnIndex; i++)
            {
                width += tablecolumns[i].PreferredWidth;
            }
            return width;
        }
        #endregion
    }
}
