#region Copyright Syncfusion Inc. 2001 - 2014
// Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Any infringement will be prosecuted under
// applicable laws. 
#endregion

using System;
using Syncfusion.Pdf.Graphics;
using Syncfusion.Pdf.Primitives;

/// <summary>
/// The Syncfusion.Pdf.ColorSpace namespace contains classes for enhanced printing support with various Color channels.
/// </summary>
namespace Syncfusion.Pdf.ColorSpace
{
    /// <summary>
    /// Represents the base class for all colorspaces. 
    /// </summary>
    /// <example>
    /// <code lang="C#">
    /// // Create a new PDF document.
    /// PdfDocument doc = new PdfDocument();
    /// //Creates a new page and adds it as the last page of the document
    /// PdfPage page = doc.Pages.Add();
    /// //  Set the document`s color spaces as GrayScale 
    /// doc.ColorSpace = PdfColorSpace.GrayScale;
    /// PdfPen pen = new PdfPen(PdfBrushes.Red);          
    /// // Draws the rectangle
    /// page.Graphics.DrawRectangle(pen, new RectangleF(0,0,100,200));
    /// doc.Save("ColorSpace.pdf");
    /// </code>
    /// <code lang="VB">
    /// ' Create a new PDF document.
    /// Dim doc As PdfDocument = New PdfDocument()
    /// ' Create a page
    /// Dim page As PdfPage = doc.Pages.Add()
    /// '  Set the document`s color spaces as GrayScale 
    /// doc.ColorSpace = PdfColorSpace.GrayScale
    /// Dim pen As PdfPen = New PdfPen(PdfBrushes.Red)
    /// ' Draws the rectangle
    /// page.Graphics.DrawRectangle(pen, New RectangleF(0,0,100,200))
    /// doc.Save("ColorSpace.pdf")
    /// </code>
    /// </example>           
    /// <seealso cref="PdfPen"/> Class
    /// <seealso cref="IPdfWrapper"/> Interface
    public abstract class PdfColorSpaces : IPdfWrapper, IPdfCache
    {
        #region Fields

        /// <summary>
        /// Internal variable to store the resources.
        /// </summary>
        internal PdfResources resources = null;

        /// <summary>
        /// Internal variable to store the internal object.
        /// </summary>
        protected static object s_syncObject = new object();

        /// <summary>
        /// Internal variable to store the color Internals.
        /// </summary>
        private IPdfPrimitive m_colorInternals;

        /// <summary>
        /// Internal variable to store the dictionary.
        /// </summary>
        private PdfDictionary m_dictionary = new PdfDictionary();

        /// <summary>
        /// Internal variable to store the colorspace.
        /// </summary>
        private PdfArray colorspace = new PdfArray();

        #endregion

        #region IPdfWrapper Members
        /// <summary>
        /// Gets Pdf primitive representing the font.
        /// </summary>
        IPdfPrimitive IPdfWrapper.Element
        {
            get
            {
                return m_colorInternals;
            }
        }
        #endregion

        #region IPdfCache Members
        /// <summary>
        /// Checks whether the object is similar to another object.
        /// </summary>
        /// <param name="obj">The object to compare witht ehcurrent object.</param>
        /// <returns>True - if the objects have equal internals and can share them, False otherwise.</returns>
        bool IPdfCache.EqualsTo(IPdfCache obj)
        {
            bool result = false;
            return result;
        }

        /// <summary>
        /// Returns internals of the object.
        /// </summary>
        /// <returns>Returns internals of the object.</returns>
        IPdfPrimitive IPdfCache.GetInternals()
        {
            return m_colorInternals;
        }

        /// <summary>
        /// Sets internals to the object.
        /// </summary>
        /// <param name="internals">Internals of the object.</param>
        void IPdfCache.SetInternals(IPdfPrimitive internals)
        {
            if (internals == null)
            {
                throw new ArgumentNullException("internals");
            }

            m_colorInternals = internals;
        }
        #endregion
    }
}

