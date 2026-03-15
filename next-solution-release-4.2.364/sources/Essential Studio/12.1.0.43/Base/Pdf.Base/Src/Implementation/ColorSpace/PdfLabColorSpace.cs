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
    /// Represents a Lab colorspace
    /// </summary>
    /// <example>
    /// <code lang="C#">
    /// // Creates a new document
    /// PdfDocument doc = new PdfDocument();
    /// //Creates a new page and adds it as the last page of the document
    /// PdfPage page = doc.Pages.Add();
    /// // Create lab color space
    /// PdfLabColorSpace calGrayCS = new PdfLabColorSpace();
    /// calGrayCS.Range = new double[] { 0.2, 1, 0.8, 23.5 };
    /// calGrayCS.WhitePoint = new double[] { 0.2, 1, 0.8 };
    /// // Create lab color
    /// PdfLabColor labColor = new PdfLabColor(calGrayCS);
    /// labColor.L = 90;
    /// labColor.A = 0.5;
    /// labColor.B = 20;
    /// PdfPen pen = new PdfPen(labColor);
    /// RectangleF rect = new RectangleF(20, 70, 200, 100);
    /// page.Graphics.DrawRectangle(pen, rect);
    /// doc.Save("LabColor.pdf");
    /// </code>
    /// <code lang="VB">
    /// ' Create a new document
    /// Dim doc As PdfDocument = New PdfDocument()
    /// ' Create a page
    /// Dim page As PdfPage = doc.Pages.Add()
    /// ' Creates lab color space
    /// Dim calGrayCS As PdfLabColorSpace = New PdfLabColorSpace()
    /// calGrayCS.Range = New Double() { 0.2, 1, 0.8, 23.5 }
    /// calGrayCS.WhitePoint = New Double() { 0.2, 1, 0.8 }
    /// ' Create lab color
    /// Dim labColor As PdfLabColor = New PdfLabColor(calGrayCS)
    /// labColor.L = 90
    /// labColor.A = 0.5
    /// labColor.B = 20
    /// Dim pen As PdfPen = New PdfPen(labColor)
    /// Dim rect As RectangleF = New RectangleF(20, 70, 200, 100)
    /// page.Graphics.DrawRectangle(pen, rect)
    /// doc.Save("LabColor.pdf")
    /// </code>
    /// </example>       
    /// <seealso cref="PdfLabColor"/> Class            
    /// <seealso cref="Syncfusion.Pdf.Graphics.PdfPen"/> Class
    /// <seealso cref="PdfColorSpaces"/> Class
    public class PdfLabColorSpace : PdfColorSpaces, IPdfWrapper
    {
        #region Fields
        /// <summary>
        /// Local variable to store the WhitePoint.
        /// </summary>
        private double[] m_whitePoint = new double[] { 0.9505, 1.0, 1.089 };

        /// <summary>
        /// Local variable to store the BlackPoint
        /// </summary>
        private double[] m_blackPoint;

        /// <summary>
        /// Local variable to store the Range
        /// </summary>
        private double[] m_range;
        #endregion

        #region Constructos
        /// <summary>
        /// Initializes a new instance of the <see cref="PdfLabColorSpace"/> class.
        /// </summary>
        /// <example>
        /// <code lang="C#">
        /// // Creates a new document
        /// PdfDocument doc = new PdfDocument();
        /// //Creates a new page and adds it as the last page of the document
        /// PdfPage page = doc.Pages.Add();
        /// // Create lab color space
        /// PdfLabColorSpace calGrayCS = new PdfLabColorSpace();
        /// calGrayCS.Range = new double[] { 0.2, 1, 0.8, 23.5 };
        /// calGrayCS.WhitePoint = new double[] { 0.2, 1, 0.8 };
        /// // Create lab color
        /// PdfLabColor labColor = new PdfLabColor(calGrayCS);
        /// labColor.L = 90;
        /// labColor.A = 0.5;
        /// labColor.B = 20;
        /// PdfPen pen = new PdfPen(labColor);
        /// RectangleF rect = new RectangleF(20, 70, 200, 100);
        /// page.Graphics.DrawRectangle(pen, rect);
        /// doc.Save("LabColor.pdf");
        /// </code>
        /// <code lang="VB">
        /// ' Create a new document
        /// Dim doc As PdfDocument = New PdfDocument()
        /// ' Create a page
        /// Dim page As PdfPage = doc.Pages.Add()
        /// ' Creates lab color space
        /// Dim calGrayCS As PdfLabColorSpace = New PdfLabColorSpace()
        /// calGrayCS.Range = New Double() { 0.2, 1, 0.8, 23.5 }
        /// calGrayCS.WhitePoint = New Double() { 0.2, 1, 0.8 }
        /// ' Create lab color
        /// Dim labColor As PdfLabColor = New PdfLabColor(calGrayCS)
        /// labColor.L = 90
        /// labColor.A = 0.5
        /// labColor.B = 20
        /// Dim pen As PdfPen = New PdfPen(labColor)
        /// Dim rect As RectangleF = New RectangleF(20, 70, 200, 100)
        /// page.Graphics.DrawRectangle(pen, rect)
        /// doc.Save("LabColor.pdf")
        /// </code>
        /// </example>       
        /// <seealso cref="PdfLabColor"/> Class            
        /// <seealso cref="Syncfusion.Pdf.Graphics.PdfPen"/> Class
        /// <seealso cref="PdfColorSpaces"/> Class
        public PdfLabColorSpace()
            : base()
        {
            Initialize();
        }
        #endregion

        #region Properties
        /// <summary>
        /// Gets or sets BlackPoint
        /// </summary>
        /// <value>An array of three numbers [XB YB ZB] specifying the tristimulus value, in the CIE 1931 XYZ space, of the diffuse black point.</value>
        /// <example>
        /// <code lang="C#">
        /// // Creates a new document
        /// PdfDocument doc = new PdfDocument();
        /// //Creates a new page and adds it as the last page of the document
        /// PdfPage page = doc.Pages.Add();
        /// // Create lab color space
        /// PdfLabColorSpace calGrayCS = new PdfLabColorSpace();
        /// calGrayCS.Range = new double[] { 0.2, 1, 0.8, 23.5 };
        /// calGrayCS.BlackPoint = new double[] { 0.2, 1, 0.8 };
        /// // Create lab color
        /// PdfLabColor labColor = new PdfLabColor(calGrayCS);
        /// labColor.L = 90;
        /// labColor.A = 0.5;
        /// labColor.B = 20;
        /// PdfPen pen = new PdfPen(labColor);
        /// RectangleF rect = new RectangleF(20, 70, 200, 100);
        /// page.Graphics.DrawRectangle(pen, rect);
        /// doc.Save("LabColor.pdf");
        /// </code>
        /// <code lang="VB">
        /// ' Create a new document
        /// Dim doc As PdfDocument = New PdfDocument()
        /// ' Create a page
        /// Dim page As PdfPage = doc.Pages.Add()
        /// ' Creates lab color space
        /// Dim calGrayCS As PdfLabColorSpace = New PdfLabColorSpace()
        /// calGrayCS.Range = New Double() { 0.2, 1, 0.8, 23.5 }
        /// calGrayCS.BlackPoint = New Double() { 0.2, 1, 0.8 }
        /// ' Create lab color
        /// Dim labColor As PdfLabColor = New PdfLabColor(calGrayCS)
        /// labColor.L = 90
        /// labColor.A = 0.5
        /// labColor.B = 20
        /// Dim pen As PdfPen = New PdfPen(labColor)
        /// Dim rect As RectangleF = New RectangleF(20, 70, 200, 100)
        /// page.Graphics.DrawRectangle(pen, rect)
        /// doc.Save("LabColor.pdf")
        /// </code>
        /// </example>              
        /// <seealso cref="PdfLabColor"/> Class            
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
        /// Gets or sets the Range
        /// </summary>
        /// <value>An array of three numbers [XB YB ZB] specifying the tristimulus value, in the CIE 1931 XYZ space, of the diffuse black point.</value>
        /// <example>
        /// <code lang="C#">
        /// // Creates a new document
        /// PdfDocument doc = new PdfDocument();
        /// //Creates a new page and adds it as the last page of the document
        /// PdfPage page = doc.Pages.Add();
        /// // Create lab color space
        /// PdfLabColorSpace calGrayCS = new PdfLabColorSpace();
        /// calGrayCS.Range = new double[] { 0.2, 1, 0.8, 23.5 };
        /// calGrayCS.WhitePoint = new double[] { 0.2, 1, 0.8 };
        /// // Create lab color
        /// PdfLabColor labColor = new PdfLabColor(calGrayCS);
        /// labColor.L = 90;
        /// labColor.A = 0.5;
        /// labColor.B = 20;
        /// PdfPen pen = new PdfPen(labColor);
        /// RectangleF rect = new RectangleF(20, 70, 200, 100);
        /// page.Graphics.DrawRectangle(pen, rect);
        /// doc.Save("LabColor.pdf");
        /// </code>
        /// <code lang="VB">
        /// ' Create a new document
        /// Dim doc As PdfDocument = New PdfDocument()
        /// ' Create a page
        /// Dim page As PdfPage = doc.Pages.Add()
        /// ' Creates lab color space
        /// Dim calGrayCS As PdfLabColorSpace = New PdfLabColorSpace()
        /// calGrayCS.Range = New Double() { 0.2, 1, 0.8, 23.5 }
        /// calGrayCS.WhitePoint = New Double() { 0.2, 1, 0.8 }
        /// ' Create lab color
        /// Dim labColor As PdfLabColor = New PdfLabColor(calGrayCS)
        /// labColor.L = 90
        /// labColor.A = 0.5
        /// labColor.B = 20
        /// Dim pen As PdfPen = New PdfPen(labColor)
        /// Dim rect As RectangleF = New RectangleF(20, 70, 200, 100)
        /// page.Graphics.DrawRectangle(pen, rect)
        /// doc.Save("LabColor.pdf")
        /// </code>
        /// </example>         
        /// <seealso cref="PdfLabColor"/> Class            
        /// <seealso cref="Syncfusion.Pdf.Graphics.PdfPen"/> Class
        /// <seealso cref="PdfColorSpaces"/> Class
        public double[] Range
        {
            get
            {
                return m_range;
            }

            set
            {
                if ((value != null) && (value.Length != 4))
                {
                    throw new ArgumentOutOfRangeException("Range", "Range array must have 3 values.");
                }

                m_range = value;
                Initialize();
            }
        }

        /// <summary>
        /// Gets or sets the white point
        /// </summary>
        /// <value>An array of three numbers [XW YW ZW] specifying the tristimulus value, in the CIE 1931 XYZ space, of the diffuse white point. </value>
        /// <example>
        /// <code lang="C#">
        /// // Creates a new document
        /// PdfDocument doc = new PdfDocument();
        /// //Creates a new page and adds it as the last page of the document
        /// PdfPage page = doc.Pages.Add();
        /// // Create lab color space
        /// PdfLabColorSpace calGrayCS = new PdfLabColorSpace();
        /// calGrayCS.Range = new double[] { 0.2, 1, 0.8, 23.5 };
        /// calGrayCS.WhitePoint = new double[] { 0.2, 1, 0.8 };
        /// // Create lab color
        /// PdfLabColor labColor = new PdfLabColor(calGrayCS);
        /// labColor.L = 90;
        /// labColor.A = 0.5;
        /// labColor.B = 20;
        /// PdfPen pen = new PdfPen(labColor);
        /// RectangleF rect = new RectangleF(20, 70, 200, 100);
        /// page.Graphics.DrawRectangle(pen, rect);
        /// doc.Save("LabColor.pdf");
        /// </code>
        /// <code lang="VB">
        /// ' Create a new document
        /// Dim doc As PdfDocument = New PdfDocument()
        /// ' Create a page
        /// Dim page As PdfPage = doc.Pages.Add()
        /// ' Creates lab color space
        /// Dim calGrayCS As PdfLabColorSpace = New PdfLabColorSpace()
        /// calGrayCS.Range = New Double() { 0.2, 1, 0.8, 23.5 }
        /// calGrayCS.WhitePoint = New Double() { 0.2, 1, 0.8 }
        /// ' Create lab color
        /// Dim labColor As PdfLabColor = New PdfLabColor(calGrayCS)
        /// labColor.L = 90
        /// labColor.A = 0.5
        /// labColor.B = 20
        /// Dim pen As PdfPen = New PdfPen(labColor)
        /// Dim rect As RectangleF = New RectangleF(20, 70, 200, 100)
        /// page.Graphics.DrawRectangle(pen, rect)
        /// doc.Save("LabColor.pdf")
        /// </code>
        /// </example>            
        /// <seealso cref="PdfLabColor"/> Class            
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
        /// Creates PdfLab colorspace Array
        /// </summary>
        /// <returns>PdfLab's colorspace Array.</returns>
        private PdfArray CreateInternals()
        {
            PdfArray colorspace = new PdfArray();
            if (colorspace != null)
            {
                PdfName name = new PdfName("Lab");
                colorspace.Add(name);
                PdfDictionary color = new PdfDictionary();
                color.SetProperty(DictionaryProperties.WhitePoint, new PdfArray(m_whitePoint));
                if (m_blackPoint != null)
                {
                    color.SetProperty(DictionaryProperties.BlackPoint, new PdfArray(m_blackPoint));
                }

                if (m_range != null)
                {
                    color.SetProperty(DictionaryProperties.Range, new PdfArray(m_range));
                }

                colorspace.Add(color);
            }

            return colorspace;
        }
        #endregion
    }
}
