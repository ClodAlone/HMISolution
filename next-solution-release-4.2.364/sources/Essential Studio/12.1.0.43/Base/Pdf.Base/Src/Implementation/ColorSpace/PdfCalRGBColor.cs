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
    /// Represents a calibrated RGB color, based on a CalRGB colorspace. 
    /// </summary>
    /// <example>
    /// <code lang="C#">
    /// // Create a new PDF document.
    /// PdfDocument doc = new PdfDocument();
    /// //Creates a new page and adds it as the last page of the document
    /// PdfPage page = doc.Pages.Add();  
    /// RectangleF rect = new RectangleF(20, 70, 200, 100);
    /// // Creates RedColorSpace
    /// PdfCalRGBColorSpace calRgbCS = new PdfCalRGBColorSpace();
    /// calRgbCS.Gamma = new double[] { 1.6, 1.1, 2.5 };
    /// calRgbCS.Matrix = new double[] { 1, 0, 0, 0, 1, 0, 0, 0, 1 };
    /// calRgbCS.WhitePoint = new double[] { 0.2, 1, 0.8 };
    /// PdfCalRGBColor red = new PdfCalRGBColor(calRgbCS);
    /// red.Red = 0;
    /// red.Green = 1;
    /// red.Blue = 0;
    /// PdfPen pen = new PdfPen(red);
    /// PdfBrush brush = new PdfSolidBrush(red);
    /// // Draw the rectangle
    /// page.Graphics.DrawRectangle(pen, rect);
    /// doc.Save("CalRedColorSpace.pdf");
    /// </code>
    /// <code lang="VB">
    /// ' Create a new PDF document.
    /// Dim doc As PdfDocument = New PdfDocument()
    /// ' Create a page
    /// Dim page As PdfPage = doc.Pages.Add()
    /// ' Gets the graphics object.
    /// Dim g As PdfGraphics = page.Graphics
    /// Dim font As PdfFont = New PdfStandardFont(PdfFontFamily.Helvetica, 14, PdfFontStyle.Bold)
    /// Dim rect As RectangleF = New RectangleF(20, 70, 200, 100)
    /// ' Creates RedColorSpace
    /// Dim calRgbCS As PdfCalRGBColorSpace = New PdfCalRGBColorSpace()
    /// calRgbCS.Gamma = New Double() { 1.6, 1.1, 2.5 }
    /// calRgbCS.Matrix = New Double() { 1, 0, 0, 0, 1, 0, 0, 0, 1 }
    /// calRgbCS.WhitePoint = New Double() { 0.2, 1, 0.8 }
    /// Dim red As PdfCalRGBColor = New PdfCalRGBColor(calRgbCS)
    /// red.Red = 0
    /// red.Green = 1
    /// red.Blue = 0
    /// Dim pen As PdfPen = New PdfPen(red)
    /// Dim brush As PdfBrush = New PdfSolidBrush(red)
    /// ' Draw the rectangle
    /// page.Graphics.DrawRectangle(pen, rect)
    /// doc.Save("CalRedColorSpace.pdf")
    /// </code>
    /// </example>
    /// <seealso cref="PdfExtendedColor"/> Class
    /// <seealso cref="PdfCalRGBColorSpace"/> Class    
    /// <seealso cref="Syncfusion.Pdf.Graphics.PdfPen"/> Class
    /// <seealso cref="PdfColorSpaces"/> Class
    public class PdfCalRGBColor : PdfExtendedColor
    {
        #region Filelds
        /// <summary>
        /// Local variable to store the Red Color.
        /// </summary>
        private double m_red;

        /// <summary>
        /// Local variable to store the Green Color.
        /// </summary>
        private double m_green;

        /// <summary>
        /// Local variable to store the Blue Color.
        /// </summary>
        private double m_blue;
        #endregion

        #region Constructos
        /// <summary>
        /// Initializes a new instance of the <see cref="PdfCalRGBColor"/> class.
        /// </summary>
        /// <param name="colorspace">The colorspace</param>
        /// <example>
        /// <code lang="C#">
        /// // Create a new PDF document.
        /// PdfDocument doc = new PdfDocument();
        /// //Creates a new page and adds it as the last page of the document
        /// PdfPage page = doc.Pages.Add();  
        /// RectangleF rect = new RectangleF(20, 70, 200, 100);
        /// // Creates RedColorSpace
        /// PdfCalRGBColorSpace calRgbCS = new PdfCalRGBColorSpace();
        /// calRgbCS.Gamma = new double[] { 1.6, 1.1, 2.5 };
        /// calRgbCS.Matrix = new double[] { 1, 0, 0, 0, 1, 0, 0, 0, 1 };
        /// calRgbCS.WhitePoint = new double[] { 0.2, 1, 0.8 };
        /// PdfCalRGBColor red = new PdfCalRGBColor(calRgbCS);
        /// red.Red = 0;
        /// red.Green = 1;
        /// red.Blue = 0;
        /// PdfPen pen = new PdfPen(red);
        /// PdfBrush brush = new PdfSolidBrush(red);
        /// // Draw the rectangle
        /// page.Graphics.DrawRectangle(pen, rect);
        /// doc.Save("CalRedColorSpace.pdf");
        /// </code>
        /// <code lang="VB">
        /// ' Create a new PDF document.
        /// Dim doc As PdfDocument = New PdfDocument()
        /// ' Create a page
        /// Dim page As PdfPage = doc.Pages.Add()
        /// ' Gets the graphics object.
        /// Dim g As PdfGraphics = page.Graphics
        /// Dim font As PdfFont = New PdfStandardFont(PdfFontFamily.Helvetica, 14, PdfFontStyle.Bold)
        /// Dim rect As RectangleF = New RectangleF(20, 70, 200, 100)
        /// ' Creates RedColorSpace
        /// Dim calRgbCS As PdfCalRGBColorSpace = New PdfCalRGBColorSpace()
        /// calRgbCS.Gamma = New Double() { 1.6, 1.1, 2.5 }
        /// calRgbCS.Matrix = New Double() { 1, 0, 0, 0, 1, 0, 0, 0, 1 }
        /// calRgbCS.WhitePoint = New Double() { 0.2, 1, 0.8 }
        /// Dim red As PdfCalRGBColor = New PdfCalRGBColor(calRgbCS)
        /// red.Red = 0
        /// red.Green = 1
        /// red.Blue = 0
        /// Dim pen As PdfPen = New PdfPen(red)
        /// Dim brush As PdfBrush = New PdfSolidBrush(red)
        /// ' Draw the rectangle
        /// page.Graphics.DrawRectangle(pen, rect)
        /// doc.Save("CalRedColorSpace.pdf")
        /// </code>
        /// </example>
        /// <seealso cref="PdfExtendedColor"/> Class
        /// <seealso cref="PdfCalRGBColorSpace"/> Class    
        /// <seealso cref="Syncfusion.Pdf.Graphics.PdfPen"/> Class
        /// <seealso cref="PdfColorSpaces"/> Class
        public PdfCalRGBColor(PdfColorSpaces colorspace)
            : base(colorspace)
        {
        }
        #endregion

        #region Properties
        /// <summary>
        /// Gets or sets the Blue value.
        /// </summary>
        /// <value>The blue level of this color.</value>
        /// <remarks>The acceptable range for this value is [0.0 1.0]. 0.0 means the darkest color that can be achieved, and 1.0 means the lightest. </remarks>
        /// <example>
        /// <code lang="C#">
        /// // Create a new PDF document.
        /// PdfDocument doc = new PdfDocument();
        /// //Creates a new page and adds it as the last page of the document
        /// PdfPage page = doc.Pages.Add();  
        /// RectangleF rect = new RectangleF(20, 70, 200, 100);
        /// // Creates RedColorSpace
        /// PdfCalRGBColorSpace calRgbCS = new PdfCalRGBColorSpace();
        /// calRgbCS.Gamma = new double[] { 1.6, 1.1, 2.5 };
        /// calRgbCS.Matrix = new double[] { 1, 0, 0, 0, 1, 0, 0, 0, 1 };
        /// calRgbCS.WhitePoint = new double[] { 0.2, 1, 0.8 };
        /// PdfCalRGBColor red = new PdfCalRGBColor(calRgbCS);          
        /// red.Blue = 0;
        /// PdfPen pen = new PdfPen(red);
        /// PdfBrush brush = new PdfSolidBrush(red);
        /// // Draw the rectangle
        /// page.Graphics.DrawRectangle(pen, rect);
        /// doc.Save("CalRedColorSpace.pdf");
        /// </code>
        /// <code lang="VB">
        /// ' Create a new PDF document.
        /// Dim doc As PdfDocument = New PdfDocument()
        /// ' Create a page
        /// Dim page As PdfPage = doc.Pages.Add()
        /// ' Gets the graphics object.
        /// Dim g As PdfGraphics = page.Graphics
        /// Dim font As PdfFont = New PdfStandardFont(PdfFontFamily.Helvetica, 14, PdfFontStyle.Bold)
        /// Dim rect As RectangleF = New RectangleF(20, 70, 200, 100)
        /// ' Creates RedColorSpace
        /// Dim calRgbCS As PdfCalRGBColorSpace = New PdfCalRGBColorSpace()
        /// calRgbCS.Gamma = New Double() { 1.6, 1.1, 2.5 }
        /// calRgbCS.Matrix = New Double() { 1, 0, 0, 0, 1, 0, 0, 0, 1 }
        /// calRgbCS.WhitePoint = New Double() { 0.2, 1, 0.8 }
        /// Dim red As PdfCalRGBColor = New PdfCalRGBColor(calRgbCS)
        /// red.Green = 1
        /// Dim pen As PdfPen = New PdfPen(red)
        /// Dim brush As PdfBrush = New PdfSolidBrush(red)
        /// ' Draw the rectangle
        /// page.Graphics.DrawRectangle(pen, rect)
        /// doc.Save("CalRedColorSpace.pdf")
        /// </code>
        /// </example>
        /// <seealso cref="PdfExtendedColor"/> Class
        /// <seealso cref="PdfCalRGBColorSpace"/> Class    
        /// <seealso cref="Syncfusion.Pdf.Graphics.PdfPen"/> Class
        /// <seealso cref="PdfColorSpaces"/> Class
        public double Blue
        {
            get
            {
                return m_blue;
            }

            set
            {
                if ((value < 0.0) || (value > 1.0))
                {
                    throw new ArgumentOutOfRangeException("Blue", "Blue level must be between 0 and 1");
                }

                m_blue = value;
            }
        }

        /// <summary>
        /// Gets or sets the green level for this color. 
        /// </summary>
        /// <value>The green level of this color. </value>
        /// <remarks>The acceptable range for this value is [0.0 1.0]. 0.0 means the darkest color that can be achieved, and 1.0 means the lightest color. </remarks>
        /// <example>
        /// <code lang="C#">
        /// // Create a new PDF document.
        /// PdfDocument doc = new PdfDocument();
        /// //Creates a new page and adds it as the last page of the document
        /// PdfPage page = doc.Pages.Add();  
        /// RectangleF rect = new RectangleF(20, 70, 200, 100);
        /// // Creates RedColorSpace
        /// PdfCalRGBColorSpace calRgbCS = new PdfCalRGBColorSpace();
        /// calRgbCS.Gamma = new double[] { 1.6, 1.1, 2.5 };
        /// calRgbCS.Matrix = new double[] { 1, 0, 0, 0, 1, 0, 0, 0, 1 };
        /// calRgbCS.WhitePoint = new double[] { 0.2, 1, 0.8 };
        /// PdfCalRGBColor red = new PdfCalRGBColor(calRgbCS);     
        /// red.Green = 1;        
        /// PdfPen pen = new PdfPen(red);
        /// PdfBrush brush = new PdfSolidBrush(red);
        /// // Draw the rectangle
        /// page.Graphics.DrawRectangle(pen, rect);
        /// doc.Save("CalRedColorSpace.pdf");
        /// </code>
        /// <code lang="VB">
        /// ' Create a new PDF document.
        /// Dim doc As PdfDocument = New PdfDocument()
        /// ' Create a page
        /// Dim page As PdfPage = doc.Pages.Add()
        /// ' Gets the graphics object.
        /// Dim g As PdfGraphics = page.Graphics
        /// Dim font As PdfFont = New PdfStandardFont(PdfFontFamily.Helvetica, 14, PdfFontStyle.Bold)
        /// Dim rect As RectangleF = New RectangleF(20, 70, 200, 100)
        /// ' Creates RedColorSpace
        /// Dim calRgbCS As PdfCalRGBColorSpace = New PdfCalRGBColorSpace()
        /// calRgbCS.Gamma = New Double() { 1.6, 1.1, 2.5 }
        /// calRgbCS.Matrix = New Double() { 1, 0, 0, 0, 1, 0, 0, 0, 1 }
        /// calRgbCS.WhitePoint = New Double() { 0.2, 1, 0.8 }
        /// Dim red As PdfCalRGBColor = New PdfCalRGBColor(calRgbCS)        
        /// red.Green = 1        
        /// Dim pen As PdfPen = New PdfPen(red)
        /// Dim brush As PdfBrush = New PdfSolidBrush(red)
        /// ' Draw the rectangle
        /// page.Graphics.DrawRectangle(pen, rect)
        /// doc.Save("CalRedColorSpace.pdf")
        /// </code>
        /// </example>
        /// <seealso cref="PdfExtendedColor"/> Class
        /// <seealso cref="PdfCalRGBColorSpace"/> Class    
        /// <seealso cref="Syncfusion.Pdf.Graphics.PdfPen"/> Class
        /// <seealso cref="PdfColorSpaces"/> Class
        public double Green
        {
            get
            {
                return m_green;
            }

            set
            {
                if ((value < 0.0) || (value > 1.0))
                {
                    throw new ArgumentOutOfRangeException("Green", "Green level must be between 0 and 1");
                }

                m_green = value;
            }
        }

        /// <summary>
        /// Gets or sets the red level for this color.
        /// </summary>
        /// <value>The red level of this color.</value>
        /// <remarks>The acceptable range for this value is [0.0 1.0]. 0.0 means the darkest color that can be achieved, and 1.0 means the lightest color. </remarks>
        /// <example>
        /// <code lang="C#">
        /// // Create a new PDF document.
        /// PdfDocument doc = new PdfDocument();
        /// //Creates a new page and adds it as the last page of the document
        /// PdfPage page = doc.Pages.Add();  
        /// RectangleF rect = new RectangleF(20, 70, 200, 100);
        /// // Creates RedColorSpace
        /// PdfCalRGBColorSpace calRgbCS = new PdfCalRGBColorSpace();
        /// calRgbCS.Gamma = new double[] { 1.6, 1.1, 2.5 };
        /// calRgbCS.Matrix = new double[] { 1, 0, 0, 0, 1, 0, 0, 0, 1 };
        /// calRgbCS.WhitePoint = new double[] { 0.2, 1, 0.8 };
        /// PdfCalRGBColor red = new PdfCalRGBColor(calRgbCS);
        /// red.Red = 0;      
        /// PdfPen pen = new PdfPen(red);
        /// PdfBrush brush = new PdfSolidBrush(red);
        /// // Draw the rectangle
        /// page.Graphics.DrawRectangle(pen, rect);
        /// doc.Save("CalRedColorSpace.pdf");
        /// </code>
        /// <code lang="VB">
        /// ' Create a new PDF document.
        /// Dim doc As PdfDocument = New PdfDocument()
        /// ' Create a page
        /// Dim page As PdfPage = doc.Pages.Add()
        /// ' Gets the graphics object.
        /// Dim g As PdfGraphics = page.Graphics
        /// Dim font As PdfFont = New PdfStandardFont(PdfFontFamily.Helvetica, 14, PdfFontStyle.Bold)
        /// Dim rect As RectangleF = New RectangleF(20, 70, 200, 100)
        /// ' Creates RedColorSpace
        /// Dim calRgbCS As PdfCalRGBColorSpace = New PdfCalRGBColorSpace()
        /// calRgbCS.Gamma = New Double() { 1.6, 1.1, 2.5 }
        /// calRgbCS.Matrix = New Double() { 1, 0, 0, 0, 1, 0, 0, 0, 1 }
        /// calRgbCS.WhitePoint = New Double() { 0.2, 1, 0.8 }
        /// Dim red As PdfCalRGBColor = New PdfCalRGBColor(calRgbCS)
        /// red.Red = 0     
        /// Dim pen As PdfPen = New PdfPen(red)
        /// Dim brush As PdfBrush = New PdfSolidBrush(red)
        /// ' Draw the rectangle
        /// page.Graphics.DrawRectangle(pen, rect)
        /// doc.Save("CalRedColorSpace.pdf")
        /// </code>
        /// </example>
        /// <seealso cref="PdfExtendedColor"/> Class
        /// <seealso cref="PdfCalRGBColorSpace"/> Class    
        /// <seealso cref="Syncfusion.Pdf.Graphics.PdfPen"/> Class
        /// <seealso cref="PdfColorSpaces"/> Class
        public double Red
        {
            get
            {
                return m_red;
            }

            set
            {
                if ((value < 0.0) || (value > 1.0))
                {
                    throw new ArgumentOutOfRangeException("Red", "Red level must be between 0 and 1");
                }

                m_red = value;
            }
        }
        #endregion
    }
}
