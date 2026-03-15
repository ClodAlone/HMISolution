#region Copyright Syncfusion Inc. 2001 - 2014
//
//  Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
//
//  Use of this code is subject to the terms of our license.
//  A copy of the current license can be obtained at any time by e-mailing
//  licensing@syncfusion.com. Re-distribution in any form is strictly
//  prohibited. Any infringement will be prosecuted under applicable laws. 
//
#endregion

#region file using directives
using System.Collections;
#endregion

namespace Syncfusion.DocIO.DLS
{
    /// <summary>
    /// Represents base interface for dls collections.
    /// </summary>
    public interface ICollectionBase : IEnumerable
    {
        /// <summary>
        /// Gets the number of items in collection.
        /// </summary>
        /// <value>The count.</value>
        int Count
        {
            get;
        }
    }
}