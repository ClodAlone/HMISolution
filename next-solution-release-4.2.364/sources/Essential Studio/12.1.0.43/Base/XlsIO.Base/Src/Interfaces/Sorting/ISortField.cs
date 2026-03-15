#region Copyright Syncfusion Inc. 2001 - 2014
// Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Any infringement will be prosecuted under
// applicable laws. 
#endregion
#if ( WINRT )
using Windows.UI;

#endif

#if  (SILVERLIGHT || WP)
using System.Windows.Media;
#elif ( WINRT )
using Syncfusion.XlsIO;
#else
using System.Drawing;

#endif 

namespace Syncfusion.XlsIO
{
    public interface ISortField
    {
        #region Properties
        /// <summary>
        /// Represents the column to be sorted on.
        /// </summary>
         int Key { get; set; }
        /// <summary>
        /// Represents the sort by in the range.
        /// </summary>
        SortOn SortOn { get; set; }
        /// <summary>
        /// Represents the sort order.
        /// </summary>
        OrderBy Order { get; set; }
        /// <summary>
        /// Represents the color to sort. Throws exception when SortOn type is Values.
        /// </summary>
        Color Color { get; set; }
        #endregion

        #region Methods
        /// <summary>
        /// Sets sorting priority.
        /// </summary>
        /// <param name="priority">integer priority value. 0 represents high priority.</param>
        void SetPriority(int priority);
        #endregion
    }
}
