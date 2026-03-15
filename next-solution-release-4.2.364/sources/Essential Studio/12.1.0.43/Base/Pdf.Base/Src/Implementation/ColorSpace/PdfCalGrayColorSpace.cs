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

/// <summary>
/// The Syncfusion.Pdf.ColorSpace namespace contains classes for enhanced printing support with various Color channels.
/// </summary>
namespace Syncfusion.Pdf.ColorSpace
{
    /// <summary>
    /// Represents a CalGray colorspace.
    /// </summary>
    /// <example>
    /// <code lang="C#">
    /// // Create a new PDF document
    /// PdfDocument doc = new PdfDocument();
    /// //Creates a new page and adds it as the last page of the document
    /// PdfPage page = doc.Pages.Add();
    /// RectangleF rect = new RectangleF(20, 70, 100, 50);
    /// // Creates GrayColorSpace
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
    /// <seealso cref="PdfColorSpaces"/> Class
    /// <seealso cref="PdfCalGrayColor"/> Class    
    /// <seealso cref="Syncfusion.Pdf.Graphics.PdfPen"/> Class    
    public class PdfCalGrayColorSpace : PdfColorSpaces, IPdfWrapper
    {
        #region Filelds
        /// <summary>
        /// Local variable to store the White point of this colorspace.
        /// </summary>
        private double[] m_whitePoint = new double[] { 0.9505, 1.0, 1.089 };

        /// <summary>
        /// Local variable to store the Game value of this colorspace.
        /// </summary>
        private double m_gama = 1.0;

        /// <summary>
        /// Local variable to store the black point of this colorspace.
        /// </summary>
        private double[] m_blackPoint;
        #endregion

        #region Constructors
        /// <summary>
        /// Initializes a new instance of the <see cref="T:PdfCalGrayColorSpace"/> class.
        /// </summary>
        /// <example>
        /// <code lang="C#">
        /// // Create a new PDF document
        /// PdfDocument doc = new PdfDocument();
        /// //Creates a new page and adds it as the last page of the document
        /// PdfPage page = doc.Pages.Add();
        /// RectangleF rect = new RectangleF(20, 70, 100, 50);
        /// // Creates GrayColorSpace
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
        /// <seealso cref="PdfColorSpaces"/> Class
        /// <seealso cref="PdfCalGrayColor"/> Class    
        /// <seealso cref="Syncfusion.Pdf.Graphics.PdfPen"/> Class        
        public PdfCalGrayColorSpace()
            : base()
        {
            Initialize();
        }
        #endregion

        #region Properties
        /// <summary>
        /// Gets or sets the black point. 
        /// </summary>
        /// <value>An array of three numbers [XB YB ZB] specifying the tristimulus value, in the CIE 1931 XYZ space, of the diffuse black point. Default value: [ 0.0 0.0 0.0 ].</value>
        /// <example>
        /// <code lang="C#">
        /// // Create a new PDF document.
        /// PdfDocument doc = new PdfDocument();
        /// //Creates a new page and adds it as the last page of the document
        /// PdfPage page = doc.Pages.Add();       
        /// RectangleF rect = new RectangleF(20, 70, 100, 50);
        /// // Create CalGraycolor space
        /// PdfCalGrayColorSpace calGrayCS = new PdfCalGrayColorSpace();
        /// calGrayCS.BlackPoint = new double[] { 0.2, 0.3, 0.8 };
        /// // Create new instance for PdfCalGrayColor
        /// PdfCalGrayColor red = new PdfCalGrayColor(calGrayCS);
        /// red.Gray = 0.2;
        /// PdfPen pen = new PdfPen(red);
        /// PdfBrush brush = new PdfSolidBrush(red);
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
        /// ' Create CalGraycolor space
        /// Dim calGrayCS As PdfCalGrayColorSpace = New PdfCalGrayColorSpace()
        /// calGrayCS.BlackPoint = New Double() { 0.2, 0.3, 0.8 }
        /// ' Create new instance for PdfCalGrayColor
        /// Dim red As PdfCalGrayColor = New PdfCalGrayColor(calGrayCS)
        /// red.Gray = 0.2
        /// Dim pen As PdfPen = New PdfPen(red)
        /// Dim brush As PdfBrush = New PdfSolidBrush(red)
        /// ' Draws the rectangle
        /// page.Graphics.DrawRectangle(pen, rect)
        /// doc.Save("CalGrayColorSpace.pdf")
        /// </code>
        /// </example>
        /// <seealso cref="PdfColorSpaces"/> Class
        /// <seealso cref="PdfCalGrayColor"/> Class    
        /// <seealso cref="Syncfusion.Pdf.Graphics.PdfPen"/> Class        
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
        /// <example>
        /// <value>The gamma value for the gray component.</value>
        /// <code lang="C#">
        /// // Create a new PDF document.
        /// PdfDocument doc = new PdfDocument();
        /// //Creates a new page and adds it as the last page of the document
        /// PdfPage page = doc.Pages.Add();     
        /// RectangleF rect = new RectangleF(20, 70, 100, 50);
        /// // Creates gray color space
        /// PdfCalGrayColorSpace calGrayCS = new PdfCalGrayColorSpace();
        /// // Update color values
        /// calGrayCS.Gamma = 0.7;       
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
        /// ' Creates gray color space
        /// Dim calGrayCS As PdfCalGrayColorSpace = New PdfCalGrayColorSpace()
        /// ' Update color values
        /// calGrayCS.Gamma = 0.7
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
        /// <seealso cref="PdfColorSpaces"/> Class
        /// <seealso cref="PdfCalGrayColor"/> Class    
        /// <seealso cref="Syncfusion.Pdf.Graphics.PdfPen"/> Class        
        public double Gamma
        {
            get
            {
                return m_gama;
            }

            set
            {
                m_gama = value;
                Initialize();
            }
        }

        /// <summary>
        /// Gets or sets the white point.
        /// </summary>
        /// <value>An array of three numbers [XW YW ZW] specifying the tristimulus value, in the CIE 1931 XYZ space, of the diffuse white point. The numbers XW and ZW must be positive, and YW must be equal to 1.0.</value>
        /// <example>
        /// <code lang="C#">
        /// // Create a new PDF document.
        /// PdfDocument doc = new PdfDocument();
        /// //Creates a new page and adds it as the last page of the document
        /// PdfPage page = doc.Pages.Add();      
        /// RectangleF rect = new RectangleF(20, 70, 100, 50);
        /// // Create CalGraycolor space
        /// PdfCalGrayColorSpace calGrayCS = new PdfCalGrayColorSpace();
        /// calGrayCS.WhitePoint = new double[] { 0.2, 1, 0.8 }; 
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
        /// ' Create CalGraycolor space
        /// Dim calGrayCS As PdfCalGrayColorSpace = New PdfCalGrayColorSpace()
        /// calGrayCS.WhitePoint = New Double() { 0.2, 1, 0.8 }
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
        /// <seealso cref="PdfColorSpaces"/> Class
        /// <seealso cref="PdfCalGrayColor"/> Class    
        /// <seealso cref="Syncfusion.Pdf.Graphics.PdfPen"/> Class        
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
        /// Initializes the PdfLab Colorspace.
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
        /// Creates PdfCalGray ColorSpace Array
        /// </summary>
        /// <returns>PdfCalGray's ColorSpace Array.</returns>
        private PdfArray CreateInternals()
        {
            PdfArray colorspace = new PdfArray();
            if (colorspace != null)
            {
                PdfName name = new PdfName("CalGray");
                colorspace.Add(name);
                PdfDictionary color = new PdfDictionary();
                color.SetProperty(DictionaryProperties.WhitePoint, new PdfArray(m_whitePoint));
                color.SetProperty(DictionaryProperties.Gamma, new PdfNumber(m_gama));
                if (m_blackPoint != null)
                {
                    color.SetProperty(DictionaryProperties.BlackPoint, new PdfArray(m_blackPoint));
                }

                colorspace.Add(color);
            }

            return colorspace;
        }

        #endregion
    }
}
