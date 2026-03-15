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
using System.Text;
#endregion

namespace Syncfusion.DocIO.DLS
{
    /// <summary>
    /// Represents a collection of <see cref="Syncfusion.DocIO.DLS.IWTable"/>.
    /// </summary>
    public interface IWTableCollection : IEntityCollectionBase
    {
        /// <summary>
        /// Gets the <see cref="Syncfusion.DocIO.DLS.IWTable"/> at the specified index.
        /// </summary>
        /// <value></value>
        new IWTable this[int index]
        {
            get;
        }
        /// <summary>
        /// Adds a table to end of text body.
        /// </summary>
        /// <param name="table">The table.</param>
        /// <returns></returns>
        int Add(IWTable table);
        /// <summary>
        /// Determines the index of a specific item in the collection.
        /// </summary>
        /// <param name="table">The table.</param>
        /// <returns></returns>
        int IndexOf(IWTable table);
        /// <summary>
        /// Determines whether the <see cref="Syncfusion.DocIO.DLS.IWTableCollection"/> contains a specific value.
        /// </summary>
        /// <param name="table">The table.</param>
        /// <returns>
        /// 	if table is found, set to <c>true</c>.
        /// </returns>
        bool Contains(IWTable table);
    }
}
