#region Copyright Syncfusion Inc. 2001 - 2014
// Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Any infringement will be prosecuted under
// applicable laws. 
#endregion

/// <summary>
/// The Syncfusion.Pdf.ColorSpace namespace contains classes for enhanced printing support with various Color channels.
/// </summary>
namespace Syncfusion.Pdf.ColorSpace
{
    /// <summary>
    /// Represents an indexed color, based on an indexed colorspace. 
    /// </summary>
    /// <example>
    /// <code lang="C#">
    /// // Creates a new document
    /// PdfDocument doc = new PdfDocument();
    /// //Creates a new page and adds it as the last page of the document
    /// PdfPage page = doc.Pages.Add();
    /// // Creates indexed color space
    /// PdfIndexedColorSpace colorspace = new PdfIndexedColorSpace();
    /// colorspace.BaseColorSpace = new PdfDeviceColorSpace(PdfColorSpace.RGB);
    /// colorspace.MaxColorIndex = 3;
    /// colorspace.IndexedColorTable = new byte[] { 150, 0, 222, 255, 0, 0, 0, 255, 0, 0, 0, 255 };
    /// // Creates index color
    /// PdfIndexedColor color = new PdfIndexedColor(colorspace);
    /// color.SelectColorIndex = 3;
    /// RectangleF rect = new RectangleF(20, 70, 200, 100);
    /// PdfPen pen = new PdfPen(color);
    /// page.Graphics.DrawRectangle(pen, rect);
    /// doc.Save("IndexedColor.pdf");
    /// </code>
    /// <code lang="VB">
   /// ' Creates a new document
   /// Dim doc As PdfDocument = New PdfDocument()
   /// ' Create a page
   /// Dim page As PdfPage = doc.Pages.Add()
   /// ' Creates indexed color space
   /// Dim colorspace As PdfIndexedColorSpace = New PdfIndexedColorSpace()
   /// colorspace.BaseColorSpace = New PdfDeviceColorSpace(PdfColorSpace.RGB)
   /// colorspace.MaxColorIndex = 3
   /// colorspace.IndexedColorTable = New Byte() { 150, 0, 222, 255, 0, 0, 0, 255, 0, 0, 0, 255 }
   /// ' Creates index color
   /// Dim color As PdfIndexedColor = New PdfIndexedColor(colorspace)
   /// color.SelectColorIndex = 3
   /// Dim rect As RectangleF = New RectangleF(20, 70, 200, 100)
   /// Dim pen As PdfPen = New PdfPen(color)
   /// page.Graphics.DrawRectangle(pen, rect)
   /// doc.Save("IndexedColor.pdf")
    /// </code>
    /// </example>
    /// <seealso cref="PdfExtendedColor"/> Class      
    /// <seealso cref="PdfIndexedColorSpace"/> Class    
    /// <seealso cref="PdfDeviceColorSpace"/> Class      
    /// <seealso cref="Syncfusion.Pdf.Graphics.PdfPen"/> Class
    /// <seealso cref="PdfColorSpaces"/> Class
    public class PdfIndexedColor : PdfExtendedColor
    {
        #region Fields
        /// <summary>
        /// Local variable to store the color index.
        /// </summary>
        private int m_colorIndex;
        #endregion

        #region Constructors
        /// <summary>
        /// Initializes a new instance of the <see cref="PdfIndexedColor"/> class.
        /// </summary>
        /// <param name="colorspace">The colorspace.</param>
        /// <example>
        /// <code lang="C#">
        /// // Creates a new document
        /// PdfDocument doc = new PdfDocument();
        /// //Creates a new page and adds it as the last page of the document
        /// PdfPage page = doc.Pages.Add();
        /// // Creates indexed color space
        /// PdfIndexedColorSpace colorspace = new PdfIndexedColorSpace();
        /// colorspace.BaseColorSpace = new PdfDeviceColorSpace(PdfColorSpace.RGB);
        /// colorspace.MaxColorIndex = 3;
        /// colorspace.IndexedColorTable = new byte[] { 150, 0, 222, 255, 0, 0, 0, 255, 0, 0, 0, 255 };
        /// // Creates index color
        /// PdfIndexedColor color = new PdfIndexedColor(colorspace);
        /// color.SelectColorIndex = 3;
        /// RectangleF rect = new RectangleF(20, 70, 200, 100);
        /// PdfPen pen = new PdfPen(color);
        /// page.Graphics.DrawRectangle(pen, rect);
        /// doc.Save("IndexedColor.pdf");
        /// </code>
        /// <code lang="VB">
        /// ' Creates a new document
        /// Dim doc As PdfDocument = New PdfDocument()
        /// ' Create a page
        /// Dim page As PdfPage = doc.Pages.Add()
        /// ' Creates indexed color space
        /// Dim colorspace As PdfIndexedColorSpace = New PdfIndexedColorSpace()
        /// colorspace.BaseColorSpace = New PdfDeviceColorSpace(PdfColorSpace.RGB)
        /// colorspace.MaxColorIndex = 3
        /// colorspace.IndexedColorTable = New Byte() { 150, 0, 222, 255, 0, 0, 0, 255, 0, 0, 0, 255 }
        /// ' Creates index color
        /// Dim color As PdfIndexedColor = New PdfIndexedColor(colorspace)
        /// color.SelectColorIndex = 3
        /// Dim rect As RectangleF = New RectangleF(20, 70, 200, 100)
        /// Dim pen As PdfPen = New PdfPen(color)
        /// page.Graphics.DrawRectangle(pen, rect)
        /// doc.Save("IndexedColor.pdf")
        /// </code>
        /// </example>
        /// <seealso cref="PdfExtendedColor"/> Class      
        /// <seealso cref="PdfIndexedColorSpace"/> Class    
        /// <seealso cref="PdfDeviceColorSpace"/> Class      
        /// <seealso cref="Syncfusion.Pdf.Graphics.PdfPen"/> Class
        /// <seealso cref="PdfColorSpaces"/> Class
        public PdfIndexedColor(PdfIndexedColorSpace colorspace)
            : base(colorspace)
        {
        }
        #endregion

        #region Properties
        /// <summary>
        /// Gets or sets the color index
        /// </summary>
        /// <value>The index of the select color.</value>
        /// <remarks>The acceptable range for this value is 0 - MaxColorIndex.</remarks>
        /// <example>        
        /// <code lang="C#">
        /// // Creates a new document
        /// PdfDocument doc = new PdfDocument();
        /// //Creates a new page and adds it as the last page of the document
        /// PdfPage page = doc.Pages.Add();
        /// // Creates indexed color space
        /// PdfIndexedColorSpace colorspace = new PdfIndexedColorSpace();
        /// colorspace.BaseColorSpace = new PdfDeviceColorSpace(PdfColorSpace.RGB);
        /// colorspace.MaxColorIndex = 3;
        /// colorspace.IndexedColorTable = new byte[] { 150, 0, 222, 255, 0, 0, 0, 255, 0, 0, 0, 255 };
        /// // Creates index color
        /// PdfIndexedColor color = new PdfIndexedColor(colorspace);
        /// color.SelectColorIndex = 3;
        /// RectangleF rect = new RectangleF(20, 70, 200, 100);
        /// PdfPen pen = new PdfPen(color);
        /// page.Graphics.DrawRectangle(pen, rect);
        /// doc.Save("IndexedColor.pdf");
        /// </code>
        /// <code lang="VB">
        /// ' Creates a new document
        /// Dim doc As PdfDocument = New PdfDocument()
        /// ' Create a page
        /// Dim page As PdfPage = doc.Pages.Add()
        /// ' Creates indexed color space
        /// Dim colorspace As PdfIndexedColorSpace = New PdfIndexedColorSpace()
        /// colorspace.BaseColorSpace = New PdfDeviceColorSpace(PdfColorSpace.RGB)
        /// colorspace.MaxColorIndex = 3
        /// colorspace.IndexedColorTable = New Byte() { 150, 0, 222, 255, 0, 0, 0, 255, 0, 0, 0, 255 }
        /// ' Creates index color
        /// Dim color As PdfIndexedColor = New PdfIndexedColor(colorspace)
        /// color.SelectColorIndex = 3
        /// Dim rect As RectangleF = New RectangleF(20, 70, 200, 100)
        /// Dim pen As PdfPen = New PdfPen(color)
        /// page.Graphics.DrawRectangle(pen, rect)
        /// doc.Save("IndexedColor.pdf")
        /// </code>
        /// </example>
        /// <seealso cref="PdfExtendedColor"/> Class      
        /// <seealso cref="PdfIndexedColorSpace"/> Class    
        /// <seealso cref="PdfDeviceColorSpace"/> Class      
        /// <seealso cref="Syncfusion.Pdf.Graphics.PdfPen"/> Class
        /// <seealso cref="PdfColorSpaces"/> Class
        public int SelectColorIndex
        {
            get
            {
                return m_colorIndex;
            }

            set
            {
                m_colorIndex = value;
            }
        }
        #endregion
    }
}
