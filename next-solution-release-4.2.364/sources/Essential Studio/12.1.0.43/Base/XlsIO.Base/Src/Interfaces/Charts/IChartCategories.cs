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
using System;
using System.Collections;
using System.Collections.Generic;
#endregion

namespace Syncfusion.XlsIO
{
    public interface IChartCategories : IParentApplication,
    ICollection<IChartCategory>
    {
        /// <summary>
        /// Returns the number of objects in the collection. Read-only Long.
        /// </summary>
        int Count { get; }
        /// <summary>
        /// Returns a single Name object from a Names collection.
        /// </summary>
        IChartCategory this[int index] { get; }
        /// <summary>
        /// Returns a single Name object from a Names collection.
        /// </summary>
        IChartCategory this[string name] { get; }        
    }
}

