#region Copyright Syncfusion Inc. 2001 - 2014
// Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Any infringement will be prosecuted under
// applicable laws. 
#endregion

using System;
using Syncfusion.Pdf.Primitives;

namespace Syncfusion.Pdf.IO
{
    /// <summary>
    /// Defines the basic interace of the various writers.
    /// </summary>
#if NETFX_CORE || WP
    public interface IPdfWriter
#else
    internal interface IPdfWriter
#endif
    {
        /// <summary>
        /// Gets or sets the current position within the stream.
        /// </summary>
        long Position { get; set; }

        /// <summary>
        /// Stream length.
        /// </summary>
        long Length { get; }

        /// <summary>
        /// The document required for saving process.
        /// </summary>
        PdfDocumentBase Document { get; set; }

        /// <summary>
        /// Writes the specified PDF object.
        /// </summary>
        /// <param name="pdfObject">The PDF object.</param>
        void Write(IPdfPrimitive pdfObject);

        /// <summary>
        /// Writes the specified number.
        /// </summary>
        /// <param name="number">The number.</param>
        void Write(long number);

        /// <summary>
        /// Writes the specified number.
        /// </summary>
        /// <param name="number">The number.</param>
        void Write(float number);

        /// <summary>
        /// Writes the specified text.
        /// </summary>
        /// <param name="text">The text.</param>
        void Write(string text);

        /// <summary>
        /// Writes the specified text.
        /// </summary>
        /// <param name="text">The text.</param>
        void Write(char[] text);

        /// <summary>
        /// Writes the specified data.
        /// </summary>
        /// <param name="data">The data.</param>
        void Write(byte[] data);
    }
}
