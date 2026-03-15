#region Copyright Syncfusion Inc. 2001 - 2014
// Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Any infringement will be prosecuted under
// applicable laws. 
#endregion

using System;
using System.Drawing;
using Syncfusion.Pdf.Graphics.Fonts;
using Syncfusion.Pdf.Primitives;

namespace Syncfusion.Pdf.Graphics
{
    /// <summary>
    /// Base interface for true type internal fonts.
    /// </summary>
    internal interface ITrueTypeFont
    {
#if !SILVERLIGHT && !NETFX_CORE && !WP
        /// <summary>
        /// Gets system font.
        /// </summary>
        Font Font { get; }
#endif
        /// <summary>
        /// Gets size of the font.
        /// </summary>
        float Size { get; }

        /// <summary>
        /// Gets font metrics.
        /// </summary>
        PdfFontMetrics Metrics { get; }

        /// <summary>
        /// Gets the pdf primitive.
        /// </summary>
        IPdfPrimitive GetInternals();

        /// <summary>
        /// Checks whether fonts are equals.
        /// </summary>
        /// <param name="font">Font to compare.</param>
        /// <returns>True if fonts are equal, False ofhtrwise.</returns>
        bool EqualsToFont(PdfFont font);

        /// <summary>
        /// Creates font internals.
        /// </summary>
        void CreateInternals();

        /// <summary>
        /// Returns width of the char symbol.
        /// </summary>
        /// <param name="charCode">Char symbol.</param>
        /// <returns>Width of the char symbol in universal units.</returns>
        int GetCharWidth(char charCode);

        /// <summary>
        /// Returns width of the text line.
        /// </summary>
        /// <param name="line">String line.</param>
        /// <returns>Width of the char symbol in universal units.</returns>
        int GetLineWidth(string line);

        /// <summary>
        /// Releases all resources.
        /// </summary>
        void Close();
    }
}
