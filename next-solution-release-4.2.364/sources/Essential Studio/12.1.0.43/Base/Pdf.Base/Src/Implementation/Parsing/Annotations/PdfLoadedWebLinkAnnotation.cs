#region Copyright Syncfusion Inc. 2001 - 2014
// Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Any infringement will be prosecuted under
// applicable laws. 
#endregion

using System;
using System.Collections.Generic;
using System.Drawing;
using System.Text;
using Syncfusion.Pdf.Graphics;
using Syncfusion.Pdf.IO;
using Syncfusion.Pdf.Primitives;

/// <summary>
/// The Syncfusion.Pdf.Interactive namespace contains classes used to create interactive elements.
/// </summary>
namespace Syncfusion.Pdf.Interactive
{
    /// <summary>
    /// Represents the loaded web link annotation class.
    /// </summary>
    public class PdfLoadedWebLinkAnnotation : PdfLoadedUriAnnotation
    {
        #region Fields
        /// <summary>
        /// CrossTable
        /// </summary>
        private PdfCrossTable m_crossTable;
        #endregion

        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="PdfLoadedLineAnnotation"/> class.
        /// </summary>
        /// <param name="dictionary">The dictionary.</param>
        /// <param name="crossTable">The cross table.</param>
        /// <param name="rectangle">The rectangle</param>
        /// <param name="text">The text</param>
        internal PdfLoadedWebLinkAnnotation(PdfDictionary dictionary, PdfCrossTable crossTable, RectangleF rectangle, string text)
            : base(dictionary, crossTable, rectangle, text)
        {
            if (text == null)
                throw new ArgumentNullException("text");

            Dictionary = dictionary;
            m_crossTable = crossTable;
            Text = text;
        }
        #endregion

    }
}
