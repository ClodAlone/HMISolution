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
using Syncfusion.DocIO.DLS;
#endregion

namespace Syncfusion.DocIO.DLS
{
    /// <summary>
    /// Represents a collection of <see cref="Syncfusion.DocIO.DLS.IWSection"/>.
    /// </summary>
    public interface IWSectionCollection : IEntityCollectionBase
    {
        /// <summary>
        /// Gets the <see cref="Syncfusion.DocIO.DLS.IWSection"/> at the specified index.
        /// </summary>
        /// <value></value>
        new WSection this[int index]
        {
            get;
        }
        /// <summary>
        /// Adds a section to end of document.
        /// </summary>
        /// <param name="section">The section.</param>
        /// <returns></returns>
        int Add(IWSection section);
        /// <summary>
        /// Returns the zero-based index of the specified section.
        /// </summary>
        /// <param name="section">The section.</param>
        /// <returns></returns>
        int IndexOf(IWSection section);
    }
}