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
    /// Represents base interface for entities collections.
    /// </summary>
    public interface IEntityCollectionBase : ICollectionBase
    {
        /// <summary>
        /// Gets the <see cref="Syncfusion.DocIO.DLS.Entity"/> at the specified index.
        /// </summary>
        /// <value></value>
        Entity this[int index]
        {
            get;
        }
    }
}