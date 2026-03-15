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
using System.Windows.Media.Imaging;

namespace Syncfusion.Windows.PdfViewer
{
    interface ItemsProvider<T>
    {
        /// <summary>
        /// Returns the total number of items available.
        /// </summary>
        int Count();

        /// <summary>
        /// 
        /// </summary>
        void Unload();

        /// <summary>
        /// 
        /// </summary>
        int CurrentPage { get; set; }

        /// <summary>
        /// Returns a range of items.
        /// </summary>
        /// <param name="startIndex">The start index.</param>
        /// <param name="count">The number of items to fetch.</param>
        IList<T> GetRange(int startIndex, int count);
        /// <summary>
        /// Returns a range of items.
        /// </summary>
        /// <param name="startIndex">The start index.</param>
        /// <param name="count">The number of items to fetch.</param>
        IList<T> GetBlankPage(int startIndex);
    }
}
