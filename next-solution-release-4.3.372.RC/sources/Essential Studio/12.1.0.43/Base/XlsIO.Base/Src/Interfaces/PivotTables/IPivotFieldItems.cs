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

namespace Syncfusion.XlsIO
{
    public interface IPivotFieldItems
    {
        /// <summary>
        /// get the filter item from items collections based on index
        /// </summary>
        /// <param name="index"></param>
        /// <returns></returns>
        IPivotFieldItem this[int index] { get; }

        /// <summary>
        /// get the filter item from items collections based on filter text
        /// </summary>
        /// <param name="FilterText"></param>
        /// <returns></returns>
        IPivotFieldItem this[string FilterText] { get; }

        /// <summary>
        /// get the count field items.
        /// </summary>
        int Count { get; }
    }
}
