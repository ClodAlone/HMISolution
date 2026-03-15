#region Copyright Syncfusion Inc. 2001 - 2014
// Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Any infringement will be prosecuted under
// applicable laws. 
#endregion
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using System.Windows;
using System.Windows.Media;
using System.Collections;
using System.Windows.Forms;
using System.Drawing;


namespace Syncfusion.Windows.Forms.PivotAnalysis

{

    public class PivotGridConstants
    {
        public const string AllString = "(All)";
        public const string TotalString = "Total";
    }
    public class Common
    {
    }

    public enum GridAutoSizeOption
    {
        /// <summary>
        /// All Rows will be resized
        /// </summary>
        All,
        /// <summary>
        /// None of the rows will be resized
        /// </summary>
        None,
        /// <summary>
        /// The value specified in AutoSizeRowCount will be resized
        /// </summary>
        FixedCount,
        /// <summary>
        /// The Grand Total rows will be used to determine column size.
        /// </summary>
        TotalRows
    }


    internal class ReverseCustomComparer : IComparer
    {
        IComparer comparer = null;
        public ReverseCustomComparer(IComparer comparer)
        {
            this.comparer = comparer;
        }
        public int Compare(object x, object y)
        {
            return -comparer.Compare(x, y);
        }
    }


    public class ReverseOrderComparer : IComparer
    {
        #region IComparer Members

        public int Compare(object x, object y)
        {
            if (x == null && y == null)
                return 0;
            else if (y == null)
                return 1;
            else if (x == null)
                return -1;
            else
                return -x.ToString().CompareTo(y.ToString());
        }

        #endregion
    }
}
