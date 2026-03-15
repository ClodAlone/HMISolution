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
using Syncfusion.DocIO.ReaderWriter;
#endregion

namespace Syncfusion.DocIO.DLS
{
    /// <summary>
    /// Interface publishes merge field functionality
    /// </summary>
    public interface IWField : IWTextRange
    {
        /// <summary>
        /// Gets / sets field type
        /// </summary>
        FieldType FieldType { get; set; }
    }
}
