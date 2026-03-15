#region Copyright Syncfusion Inc. 2001 - 2014
// Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Any infringement will be prosecuted under
// applicable laws. 
#endregion

using System;

using Syncfusion.Pdf.IO;
using Syncfusion.Pdf.Primitives;

namespace Syncfusion.Pdf.Graphics
{
    /// <summary>
    /// This class helps to fill correctly
    /// External graphics state dictionary.
    /// </summary>
    internal sealed class PdfExternalGraphicsState : IPdfWrapper
    {
        #region Fields
        private PdfDictionary m_stateDictionary = new PdfDictionary();
        #endregion

        #region IPdfWrapper Members
        /// <summary>
        /// Gets the element.
        /// </summary>
        IPdfPrimitive IPdfWrapper.Element
        {
            get
            {
                return m_stateDictionary;
            }
        }
        #endregion
    }
}
