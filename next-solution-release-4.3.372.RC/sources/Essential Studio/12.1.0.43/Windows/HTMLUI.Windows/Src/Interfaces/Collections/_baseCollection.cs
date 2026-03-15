#region Copyright Syncfusion Inc. 2001 - 2014

////  Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
////  Use of this code is subject to the terms of our license.
////  A copy of the current license can be obtained at any time by e-mailing
////  licensing@syncfusion.com. Re-distribution in any form is strictly
////  prohibited. Any infringement will be prosecuted under applicable laws. 
#endregion

#region file using directives
using System;
using System.Collections;
#endregion

namespace Syncfusion.Windows.Forms.HTMLUI
{
    /// <summary>
    /// Interface that declares the properties which are the base for all collections.
    /// </summary>
    public interface IHTMLCollection
    : ICollection, IEnumerable
    {
        /// <summary>
        /// Gets the parent element of the current collection.
        /// </summary>
        IHTMLElement Parent { get; }
    }
}
