#region Copyright Syncfusion Inc. 2001 - 2014
// Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Any infringement will be prosecuted under
// applicable laws. 
#endregion

using System;
using System.Collections.Generic;
using System.Text;

/// <summary>
/// The Syncfusion.Pdf.ColorSpace namespace contains classes for enhanced printing support with various Color channels.
/// </summary>
namespace Syncfusion.Pdf.ColorSpace
{
    /// <summary>
    /// Represents a calibrated gray color, based on a CalGray colorspace. 
    /// </summary>
    /// <example>
    /// <code lang="C#">
    /// // Create a new PDF document.
    /// PdfDocument doc = new PdfDocument();
    /// //Creates a new page and adds it as the last page of the document.
    /// PdfPage page = doc.Pages.Add();          
    /// RectangleF rect = new RectangleF(20, 70, 100, 50);
    /// // Create Gray ColorSpace
    /// PdfCalGrayColorSpace calGrayCS = new PdfCalGrayColorSpace();
    /// // Create new instance for PdfCalGrayColor
    /// PdfCalGrayColor gray = new PdfCalGrayColor(calGrayCS);
    /// gray.Gray = 0.2;
    /// PdfPen pen = new PdfPen(gray);
    /// PdfBrush brush = new PdfSolidBrush(gray);
    /// // Draws the rectangle
    /// page.Graphics.DrawRectangle(pen, rect);
    /// doc.Save("CalGrayColorSpace.pdf");
    /// </code>
    /// <code lang="VB">
    /// ' Create a new PDF document.
    /// Dim doc As PdfDocument = New PdfDocument()
    /// ' Create a page
    /// Dim page As PdfPage = doc.Pages.Add()  
    /// Dim rect As RectangleF = New RectangleF(20, 70, 100, 50)
    /// ' Creates GrayColorSpace
    /// Dim calGrayCS As PdfCalGrayColorSpace = New PdfCalGrayColorSpace()
    /// ' Create new instance for PdfCalGrayColor
    /// Dim gray As PdfCalGrayColor = New PdfCalGrayColor(calGrayCS)
    /// gray.Gray = 0.2
    /// Dim pen As PdfPen = New PdfPen(gray)
    /// Dim brush As PdfBrush = New PdfSolidBrush(gray)
    /// ' Draws the rectangle
    /// page.Graphics.DrawRectangle(pen, rect)
    /// doc.Save("CalGrayColorSpace.pdf")
    /// </code>
    /// </example>
    /// <seealso cref="PdfExtendedColor"/> Class
    /// <seealso cref="PdfCalGrayColorSpace"/> Class        
    /// <seealso cref="Syncfusion.Pdf.Graphics.PdfPen"/> Class  
    /// <seealso cref="PdfColorSpaces"/> Class
    public class PdfCalGrayColor : PdfExtendedColor
    {
        #region Fields
        /// <summary>
        /// Local Variable to store the Gray value.
        /// </summary>
        private double m_gray;
        #endregion

        #region Constructors
        /// <summary>
        /// Initializes a new instance of the <see cref="PdfCalGrayColor"/> class.
        /// </summary>
        /// <param name="colorspace">The color space.</param>
        /// <example>
        /// <code lang="C#">
        /// // Create a new PDF document.
        /// PdfDocument doc = new PdfDocument();
        /// //Creates a new page and adds it as the last page of the document.
        /// PdfPage page = doc.Pages.Add();      
        /// PdfFont font = new PdfStandardFont(PdfFontFamily.Helvetica, 14, PdfFontStyle.Bold);
        /// RectangleF rect = new RectangleF(20, 70, 100, 50);
        /// // Create Gray ColorSpace
        /// PdfCalGrayColorSpace calGrayCS = new PdfCalGrayColorSpace();
        /// // Create new instance for PdfCalGrayColor
        /// PdfCalGrayColor gray = new PdfCalGrayColor(calGrayCS);
        /// gray.Gray = 0.2;
        /// PdfPen pen = new PdfPen(gray);
        /// PdfBrush brush = new PdfSolidBrush(gray);
        /// // Draw the rectangle
        /// page.Graphics.DrawRectangle(pen, rect);
        /// doc.Save("CalGrayColorSpace.pdf");
        /// </code>
        /// <code lang="VB">
        /// ' Create a new PDF document.
        /// Dim doc As PdfDocument = New PdfDocument()
        /// ' Create a page.
        /// Dim page As PdfPage = doc.Pages.Add()  
        /// Dim rect As RectangleF = New RectangleF(20, 70, 100, 50)
        /// ' Create Gray ColorSpace
        /// Dim calGrayCS As PdfCalGrayColorSpace = New PdfCalGrayColorSpace()
        /// ' Create new instance for PdfCalGrayColor
        /// Dim gray As PdfCalGrayColor = New PdfCalGrayColor(calGrayCS)
        /// gray.Gray = 0.2
        /// Dim pen As PdfPen = New PdfPen(gray)
        /// Dim brush As PdfBrush = New PdfSolidBrush(gray)
        /// ' Draw the rectangle
        /// page.Graphics.DrawRectangle(pen, rect)
        /// doc.Save("CalGrayColorSpace.pdf")
        /// </code>
        /// </example>
        /// <seealso cref="PdfExtendedColor"/> Class
        /// <seealso cref="PdfCalGrayColorSpace"/> Class        
        /// <seealso cref="Syncfusion.Pdf.Graphics.PdfPen"/> Class
        /// <seealso cref="PdfColorSpaces"/> Class
        public PdfCalGrayColor(PdfColorSpaces colorspace)
            : base(colorspace)
        {
        }
        #endregion

        #region Properties
        /// <summary>
        /// Gets or sets the gray level for this color. 
        /// </summary>
        /// <value>The gray level of this color.</value>
        /// <remarks>The acceptable range for this value is [0.0 1.0]. 
        /// 0.0 means the darkest color that can be achieved, and 1.0 means the lightest color. </remarks>
        /// <example>
        /// <code lang="C#">
        /// // Create a new PDF document.
        /// PdfDocument doc = new PdfDocument();
        /// //Creates a new page and adds it as the last page of the document
        /// PdfPage page = doc.Pages.Add();
        /// // Gets the graphics object.
        /// PdfGraphics g = page.Graphics;
        /// // Created font object
        /// PdfFont font = new PdfStandardFont(PdfFontFamily.Helvetica, 14, PdfFontStyle.Bold);
        /// RectangleF rect = new RectangleF(20, 70, 100, 50);
        /// // Creates GrayColorSpace
        /// PdfCalGrayColorSpace calGrayCS = new PdfCalGrayColorSpace();
        /// // Create new instance for PdfCalGrayColor
        /// PdfCalGrayColor red = new PdfCalGrayColor(calGrayCS);
        /// red.Gray = 0.2;
        /// PdfPen pen = new PdfPen(red);
        /// PdfBrush brush = new PdfSolidBrush(red);
        /// // Draw the rectangle
        /// g.DrawRectangle(pen, rect);
        /// doc.Save("CalGrayColorSpace.pdf");
        /// </code>
        /// <code lang="VB">
        /// ' Create a new PDF document.
        /// Dim doc As PdfDocument = New PdfDocument()
        /// ' Create a page
        /// Dim page As PdfPage = doc.Pages.Add()
        /// ' Gets the graphics object.
        /// Dim g As PdfGraphics = page.Graphics
        /// ' Created font object
        /// Dim font As PdfFont = New PdfStandardFont(PdfFontFamily.Helvetica, 14, PdfFontStyle.Bold)
        /// Dim rect As RectangleF = New RectangleF(20, 70, 100, 50)
        /// ' Creates GrayColorSpace
        /// Dim calGrayCS As PdfCalGrayColorSpace = New PdfCalGrayColorSpace()
        /// ' Create new instance for PdfCalGrayColor
        /// Dim red As PdfCalGrayColor = New PdfCalGrayColor(calGrayCS)
        /// red.Gray = 0.2
        /// Dim pen As PdfPen = New PdfPen(red)
        /// Dim brush As PdfBrush = New PdfSolidBrush(red)
        /// ' Draw the rectangle
        /// g.DrawRectangle(pen, rect)
        /// doc.Save("CalGrayColorSpace.pdf")
        /// </code>
        /// </example>
        /// <seealso cref="PdfExtendedColor"/> Class
        /// <seealso cref="PdfCalGrayColorSpace"/> Class        
        /// <seealso cref="Syncfusion.Pdf.Graphics.PdfPen"/> Class
        /// <seealso cref="PdfColorSpaces"/> Class
        public double Gray
        {
            get
            {
                return m_gray;
            }

            set
            {
                if ((value < 0.0) || (value > 1.0))
                {
                    throw new ArgumentOutOfRangeException("Gray", "Gray level must be between 0 and 1");
                }

                m_gray = value;
            }
        }
        #endregion
    }
}
