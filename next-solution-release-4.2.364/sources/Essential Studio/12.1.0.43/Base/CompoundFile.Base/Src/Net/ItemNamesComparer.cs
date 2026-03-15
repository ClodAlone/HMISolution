#region Copyright Syncfusion Inc. 2001 - 2014
// Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Any infringement will be prosecuted under
// applicable laws. 
#endregion
using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;

#if DOCIO
namespace Syncfusion.CompoundFile.DocIO.Net
#else
namespace Syncfusion.CompoundFile.XlsIO.Net
#endif
{
    /// <summary>
    /// This comparer is used to compare item names inside storage.
    /// </summary>
    class ItemNamesComparer : IComparer, IComparer<string>
    {
        #region IComparer Members
        /// <summary>
        /// Compares two objects and returns a value indicating whether one is less
        /// than, equal to, or greater than the other.
        /// </summary>
        /// <param name="x">The first object to compare.</param>
        /// <param name="y">The second object to compare.</param>
        /// <returns>
        /// Less than zero if x is less than y. 
        /// Zero if x equals y. 
        /// Greater than zero if x is greater than y. 
        /// </returns>
        public int Compare(object x, object y)
        {
            if (x == null && y == null)
                return 0;

            if (y == null)
                return 1;

            if (x == null)
                return -1;

            string strX = x.ToString();
            string strY = y.ToString();

            int iXLength = strX.Length;
            int iYLength = strY.Length;

            int iResult = iXLength - iYLength;

            if (iResult == 0)
                iResult = StringComparer.Ordinal.Compare(strX, strY);

            return iResult;
        }
        /// <summary>
        /// Compares two objects and returns a value indicating whether one is less
        /// than, equal to, or greater than the other.
        /// </summary>
        /// <param name="x">The first object to compare.</param>
        /// <param name="y">The second object to compare.</param>
        /// <returns>
        /// Less than zero if x is less than y. 
        /// Zero if x equals y. 
        /// Greater than zero if x is greater than y. 
        /// </returns>
        public int Compare(string x, string y)
        {
            if (x == null && y == null)
                return 0;

            if (y == null)
                return 1;

            if (x == null)
                return -1;

            int iXLength = x.Length;
            int iYLength = y.Length;

            int iResult = iXLength - iYLength;

            if (iResult == 0)
                iResult = StringComparer.Ordinal.Compare(x.ToUpper(), y.ToUpper());

            return iResult;
        }
        #endregion
    }
}
