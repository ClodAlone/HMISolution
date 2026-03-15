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

#if !SILVERLIGHT

using System.Collections;
using System.Collections.Generic;

namespace Syncfusion.DocIO.DLS.Rendering
{
    /// <summary>
    /// Represents a collection of pages
    /// </summary>
    [Syncfusion.Documentation.DocumentationExclude()]
    internal class PageCollection : List<Page>
    {
        #region Constructors
        /// <summary>
        /// Initializes a new instance of the <see cref="PageCollection"/> class.
        /// </summary>
        public PageCollection()
        {
        }
        #endregion
    }
}

#endif