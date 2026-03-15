#region Copyright Syncfusion Inc. 2001 - 2014
// Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Any infringement will be prosecuted under
// applicable laws. 
#endregion

using System;

using Syncfusion.Pdf.Primitives;

namespace Syncfusion.Pdf.Functions
{
    /// <summary>
    /// Implements PDF Stitching Function.
    /// </summary>
    internal class PdfStitchingFunction : PdfFunction
    {
        #region Constructors
        /// <summary>
        /// Initializes a new instance of the <see cref="T:PdfStitchingFunction"/> class.
        /// </summary>
        internal PdfStitchingFunction()
            : base(new PdfDictionary())
        {
        }
        #endregion
    }
}
