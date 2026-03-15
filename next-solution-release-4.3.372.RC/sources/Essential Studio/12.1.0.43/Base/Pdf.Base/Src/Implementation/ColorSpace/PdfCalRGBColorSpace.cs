#region Copyright Syncfusion Inc. 2001 - 2014
// Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Any infringement will be prosecuted under
// applicable laws. 
#endregion

using System;
using Syncfusion.Pdf.Graphics;
using Syncfusion.Pdf.IO;
using Syncfusion.Pdf.Primitives;

/// <summary>
/// The Syncfusion.Pdf.ColorSpace namespace contains classes for enhanced printing support with various Color channels.
/// </summary>
namespace Syncfusion.Pdf.ColorSpace
{
    /// <summary>
    /// Representing a CalRGB colorspace. 
    /// </summary>
    /// <example>
    /// <code lang="C#">
    /// // Create a new PDF document
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
    /// ' Create a new PDF document
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
    /// <seealso cref="PdfColorSpaces"/> Class
    /// <seealso cref="PdfCalRGBColor"/> Class    
    /// <seealso cref="PdfPen"/> Class   
    public class PdfCalRGBColorSpace : PdfColorSpaces, IPdfWrapper
    {
        #region Fields
        /// <summary>
        /// Local variable to store the Whitepoint of this colorspace.
        /// </summary>
        private double[] m_whitePoint = new double[] { 0.9505, 1.0, 1.089 };

        /// <summary>
        /// Local variable to store the BlackPoint of this colorspace.
        /// </summary>
        private double[] m_blackPoint;

        /// <summary>
        /// Local variable to store the Gama of this colorspace.
        /// </summary>
        private double[] m_gama;

        /// <summary>
        /// Local variable to store the Matrix value of this colorspace.
        /// </summary>
        private double[] m_matrix;
        #endregion

        #region Constructors
        /// <summary>
        /// Initializes a new instance of the <see cref="T:PdfCalRGBColorSpace"/> class.
        /// </summary>
        /// <example>
        /// <code lang="C#">
        /// // Create a new PDF document
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
        /// ' Create a new PDF document
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
        /// <seealso cref="PdfColorSpaces"/> Class
        /// <seealso cref="PdfCalRGBColor"/> Class    
        /// <seealso cref="Syncfusion.Pdf.Graphics.PdfPen"/> Class       
        public PdfCalRGBColorSpace()
        {
            Initialize();
        }
        #endregion

        #region Properties
        /// <summary>
        /// Gets or sets the black point. 
        /// </summary>
        /// <value>An array of three numbers [XB YB ZB] specifying the tristimulus value, in the CIE 1931 XYZ space, of the diffuse black point. </value>
        /// <example>
        /// <code lang="C#">
        /// // Create a new PDF document
        /// PdfDocument doc = new PdfDocument();
        /// //Creates a new page and adds it as the last page of the document
        /// PdfPage page = doc.Pages.Add();  
        /// RectangleF rect = new RectangleF(20, 70, 200, 100);
        /// // Creates redColorSpace
        /// PdfCalRGBColorSpace calRgbCS = new PdfCalRGBColorSpace();
        /// calRgbCS.BlackPoint = new double[] { 0.5, 1, 0.8 };   
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
        /// ' Create a new PDF document
        /// Dim doc As PdfDocument = New PdfDocument()
        /// ' Create a page
        /// Dim page As PdfPage = doc.Pages.Add()
        /// ' Gets the graphics object.
        /// Dim g As PdfGraphics = page.Graphics
        /// Dim font As PdfFont = New PdfStandardFont(PdfFontFamily.Helvetica, 14, PdfFontStyle.Bold)
        /// Dim rect As RectangleF = New RectangleF(20, 70, 200, 100)
        /// ' Creates redColorSpace
        /// Dim calRgbCS As PdfCalRGBColorSpace = New PdfCalRGBColorSpace()
        /// calRgbCS.BlackPoint = New Double() { 0.5, 1, 0.8 }
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
        /// <seealso cref="PdfCalRGBColor"/> Class    
        /// <seealso cref="Syncfusion.Pdf.Graphics.PdfPen"/> Class
        /// <seealso cref="PdfColorSpaces"/> Class
        public double[] BlackPoint
        {
            get
            {
                return m_blackPoint;
            }

            set
            {
                if ((value != null) && (value.Length != 3))
                {
                    throw new ArgumentOutOfRangeException("BlackPoint", "BlackPoint array must have 3 values.");
                }

                m_blackPoint = value;
                Initialize();
            }
        }

        /// <summary>
        /// Gets or sets the gamma. 
        /// </summary>
        /// <value>An array of three numbers [GR GG GB] specifying the gamma for the red, green, and blue components of the color space. </value>
        /// <example>
        /// <code lang="C#">
        /// // Create a new PDF document
        /// PdfDocument doc = new PdfDocument();
        /// //Creates a new page and adds it as the last page of the document
        /// PdfPage page = doc.Pages.Add();  
        /// RectangleF rect = new RectangleF(20, 70, 200, 100);
        /// // Creates redColorSpace
        /// PdfCalRGBColorSpace calRgbCS = new PdfCalRGBColorSpace();
        /// calRgbCS.Gamma = new double[] { 1.6, 1.1, 2.5 };        
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
        /// ' Create a new PDF document
        /// Dim doc As PdfDocument = New PdfDocument()
        /// ' Create a page
        /// Dim page As PdfPage = doc.Pages.Add()
        /// ' Gets the graphics object.
        /// Dim g As PdfGraphics = page.Graphics
        /// Dim font As PdfFont = New PdfStandardFont(PdfFontFamily.Helvetica, 14, PdfFontStyle.Bold)
        /// Dim rect As RectangleF = New RectangleF(20, 70, 200, 100)
        /// ' Creates redColorSpace
        /// Dim calRgbCS As PdfCalRGBColorSpace = New PdfCalRGBColorSpace()
        /// calRgbCS.Gamma = New Double() { 1.6, 1.1, 2.5 }
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
        /// <seealso cref="PdfCalRGBColor"/> Class    
        /// <seealso cref="Syncfusion.Pdf.Graphics.PdfPen"/> Class
        /// <seealso cref="PdfColorSpaces"/> Class
        public double[] Gamma
        {
            get
            {
                return m_gama;
            }

            set
            {
                if ((value != null) && (value.Length != 3))
                {
                    throw new ArgumentOutOfRangeException("Gamma", "Gamma array must have 3 values.");
                }

                m_gama = value;
                Initialize();
            }
        }

        /// <summary>
        /// Gets or sets the colorspace transformation matrix. 
        /// </summary>
        /// <value>An array of nine numbers [XA YA ZA XB YB ZB XC YC ZC] specifying the linear interpretation of the decoded A, B, and C components of the color space with respect to the final XYZ representation.</value>
        /// <example>
        /// <code lang="C#">
        /// // Create a new PDF document.
        /// PdfDocument doc = new PdfDocument();
        /// //Creates a new page and adds it as the last page of the document
        /// PdfPage page = doc.Pages.Add();                  
        /// RectangleF rect = new RectangleF(20, 70, 200, 100);
        /// // Creates redColorSpace
        /// PdfCalRGBColorSpace calRgbCS = new PdfCalRGBColorSpace();
        /// calRgbCS.Matrix = new double[] { 1, 0, 0, 0, 1, 0, 0, 0, 1 };   
        /// PdfCalRGBColor red = new PdfCalRGBColor(calRgbCS);
        /// red.Red = 0;
        /// PdfPen pen = new PdfPen(red);
        /// PdfBrush brush = new PdfSolidBrush(red);
        /// // Draws the rectangle
        /// page.Graphics.DrawRectangle(pen, rect);
        /// doc.Save("CalRedColorSpace.pdf");            
        /// </code>
        /// <code lang="VB">
        /// ' Create a new PDF document.
        /// Dim doc As PdfDocument = New PdfDocument()
        /// ' Create a page
        /// Dim page As PdfPage = doc.Pages.Add()    
        /// Dim rect As RectangleF = New RectangleF(20, 70, 200, 100)
        /// ' Creates redColorSpace
        /// Dim calRgbCS As PdfCalRGBColorSpace = New PdfCalRGBColorSpace()
        /// calRgbCS.Matrix = New Double() { 1, 0, 0, 0, 1, 0, 0, 0, 1 }
        /// Dim red As PdfCalRGBColor = New PdfCalRGBColor(calRgbCS)
        /// red.Red = 0
        /// Dim pen As PdfPen = New PdfPen(red)
        /// Dim brush As PdfBrush = New PdfSolidBrush(red)
        /// ' Draws the rectangle
        /// page.Graphics.DrawRectangle(pen, rect)
        /// doc.Save("CalRedColorSpace.pdf")
        /// </code>
        /// </example>      
        /// <seealso cref="PdfCalRGBColor"/> Class    
        /// <seealso cref="Syncfusion.Pdf.Graphics.PdfPen"/> Class
        /// <seealso cref="PdfColorSpaces"/> Class
        public double[] Matrix
        {
            get
            {
                return m_matrix;
            }

            set
            {
                if ((value != null) && (value.Length != 9))
                {
                    throw new ArgumentOutOfRangeException("Matrix", "Matrix array must have 9 values.");
                }

                m_matrix = value;
                Initialize();
            }
        }

        /// <summary>
        /// Gets or sets the white point.
        /// </summary>
        /// <value>An array of three numbers [XW YW ZW] specifying the tristimulus value, in the CIE 1931 XYZ space, of the diffuse white point.</value>
        /// <example>
        /// <code lang="C#">
        /// // Create a new PDF document.
        /// PdfDocument doc = new PdfDocument();
        /// //Creates a new page and adds it as the last page of the document
        /// PdfPage page = doc.Pages.Add();                    
        /// RectangleF rect = new RectangleF(20, 70, 200, 100);
        /// // Creates redColorSpace
        /// PdfCalRGBColorSpace calRgbCS = new PdfCalRGBColorSpace();
        /// calRgbCS.WhitePoint = new double[] { 0.2, 1, 0.8 };
        /// PdfCalRGBColor red = new PdfCalRGBColor(calRgbCS);
        /// red.Red = 0;
        /// PdfPen pen = new PdfPen(red);
        /// PdfBrush brush = new PdfSolidBrush(red);
        /// // Draws the rectangle
        /// page.Graphics.DrawRectangle(pen, rect);
        /// doc.Save("CalRedColorSpace.pdf");
        /// </code>
        /// <code lang="VB">
        /// ' Create a new PDF document.
        /// Dim doc As PdfDocument = New PdfDocument()
        /// ' Create a page
        /// Dim page As PdfPage = doc.Pages.Add()        
        /// Dim rect As RectangleF = New RectangleF(20, 70, 200, 100)
        /// ' Creates redColorSpace
        /// Dim calRgbCS As PdfCalRGBColorSpace = New PdfCalRGBColorSpace()
        /// calRgbCS.WhitePoint = New Double() { 0.2, 1, 0.8 }
        /// Dim red As PdfCalRGBColor = New PdfCalRGBColor(calRgbCS)
        /// red.Red = 0
        /// Dim pen As PdfPen = New PdfPen(red)
        /// Dim brush As PdfBrush = New PdfSolidBrush(red)
        /// ' Draws the rectangle
        /// page.Graphics.DrawRectangle(pen, rect)
        /// doc.Save("CalRedColorSpace.pdf")
        /// </code>
        /// </example>       
        /// <seealso cref="PdfCalRGBColor"/> Class    
        /// <seealso cref="Syncfusion.Pdf.Graphics.PdfPen"/> Class
        /// <seealso cref="PdfColorSpaces"/> Class
        public double[] WhitePoint
        {
            get
            {
                return m_whitePoint;
            }

            set
            {
                if (value == null)
                {
                    throw new ArgumentNullException("WhitePoint", "WhitePoint array cannot be null.");
                }

                if (value.Length != 3)
                {
                    throw new ArgumentOutOfRangeException("WhitePoint", "WhitePoint array must have 3 values.");
                }

                m_whitePoint = value;
                Initialize();
            }
        }
        #endregion

        #region Methods
        /// <summary>
        /// Initializes the PdfCalRGB ColorSpace
        /// </summary>
        private void Initialize()
        {
            lock (s_syncObject)
            {
                IPdfCache equalColorSpace = PdfDocument.Cache.Search(this);

                IPdfPrimitive internals = null;

                if (equalColorSpace == null)
                {
                    internals = CreateInternals();
                }
                else
                {
                    internals = equalColorSpace.GetInternals();
                }

                ((IPdfCache)this).SetInternals(internals);
            }
        }

        /// <summary>
        /// Creates PdfCalRGB ColorSpace Array
        /// </summary>
        /// <returns>PdfCalRGB's ColorSpace Array.</returns>
        private PdfArray CreateInternals()
        {
            PdfArray colorspace = new PdfArray();
            if (colorspace != null)
            {
                PdfName name = new PdfName("CalRGB");
                colorspace.Add(name);
                PdfDictionary color = new PdfDictionary();
                color.SetProperty(DictionaryProperties.WhitePoint, new PdfArray(m_whitePoint));
                if (m_gama != null)
                {
                    color.SetProperty(DictionaryProperties.Gamma, new PdfArray(m_gama));
                }

                if (m_blackPoint != null)
                {
                    color.SetProperty(DictionaryProperties.BlackPoint, new PdfArray(m_blackPoint));
                }

                if (m_matrix != null)
                {
                    color.SetProperty(DictionaryProperties.Matrix, new PdfArray(m_matrix));
                }

                colorspace.Add(color);
            }

            return colorspace;
        }
        #endregion
    }
}
