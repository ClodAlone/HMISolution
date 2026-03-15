#region Copyright Syncfusion Inc. 2001 - 2014
// Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Any infringement will be prosecuted under
// applicable laws. 
#endregion
using System;
using System.Collections.Generic;
using System.Text;
using System.Collections;

namespace Syncfusion.Windows.Forms.Grid
{
    /// <summary>
    /// ValueComparer is used to sort the values based on the value type of the cells.
    /// </summary>
    public class ValueComparer : IComparer
    {
        #region IComparer Members
        /// <summary>
        /// compare the value for the sorting
        /// </summary>
        /// <param name="x">first object</param>
        /// <param name="y">second object</param>
        /// <returns></returns>
        public int Compare(object x, object y)
        {
            int a, b, c;

            if (int.TryParse(x.ToString(), out a) && int.TryParse(y.ToString(), out b))
            {
                if (a != 0)
                {
                    c = a.CompareTo(b);
                }
                else
                    c = 0;
                return c;
            }
            else
            {
                if (x == null && y == null)
                    return 0;

                else
                {
                    return ((IComparable)x.ToString()).CompareTo(y.ToString());

                }
            }
        }
        #endregion
    }
}
