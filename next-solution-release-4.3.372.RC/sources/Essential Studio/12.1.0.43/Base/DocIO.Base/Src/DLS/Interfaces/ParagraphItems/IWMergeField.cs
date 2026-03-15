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

using Syncfusion.DocIO.DLS;
#endregion

namespace Syncfusion.DocIO.DLS
{
    /// <summary>
    /// Interface publishes merge field functionality
    /// </summary>
    public interface IWMergeField : IWField
    {
        /// <summary>
        /// Gets / sets field name
        /// </summary>
        string FieldName { get; set; }
        /// <summary>
        /// Gets/sets "text before" switching value
        /// </summary>
        string TextBefore { get; set; }
        /// <summary>
        /// Gets/sets "text after" switching value
        /// </summary>
        string TextAfter { get; set; }
    }
}