#region Copyright Syncfusion Inc. 2001 - 2014
// Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Any infringement will be prosecuted under
// applicable laws. 
#endregion

using System;

/// <summary>
/// The Syncfusion.Pdf.ColorSpace namespace contains classes for enhanced printing support with various Color channels.
/// </summary>
namespace Syncfusion.Pdf.ColorSpace
{
    /// <summary>
    /// Represents a calibrated Lab color, based on a Lab colorspace. 
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
    /// <seealso cref="PdfLabColorSpace"/> Class            
    /// <seealso cref="Syncfusion.Pdf.Graphics.PdfPen"/> Class
    /// <seealso cref="PdfColorSpaces"/> Class
    public class PdfLabColor : PdfExtendedColor
    {
        #region Fields
        /// <summary>
        /// Local variable to store the A value.
        /// </summary>
        private double m_a;

        /// <summary>
        /// Local variable to store the B value.
        /// </summary>
        private double m_b;

        /// <summary>
        /// Local variable to store the L value.
        /// </summary>
        private double m_l;
        #endregion

        #region Constructors
        /// <summary>
        /// Initializes a new instance of the <see cref="T:PdfLabColor"/> class.
        /// </summary>
        /// <param name="colorspace">The ColorSpace.</param>
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
        /// <seealso cref="PdfLabColorSpace"/> Class            
        /// <seealso cref="Syncfusion.Pdf.Graphics.PdfPen"/> Class
        /// <seealso cref="PdfColorSpaces"/> Class
        public PdfLabColor(PdfColorSpaces colorspace)
            : base(colorspace)
        {
        }
        #endregion

        #region Properties
        /// <summary>
        /// Gets or sets the a* component for this color. 
        /// </summary>
        /// <value>The a* component of this color.</value>
        /// <remarks>The range for this value is defined by the Range property of the underlying Lab colorspace. </remarks>
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
        /// <seealso cref="PdfLabColorSpace"/> Class            
        /// <seealso cref="Syncfusion.Pdf.Graphics.PdfPen"/> Class
        /// <seealso cref="PdfColorSpaces"/> Class
        public double A
        {
            get
            {
                return m_a;
            }

            set
            {
                PdfLabColorSpace colorSpace = base.ColorSpace as PdfLabColorSpace;
                if ((colorSpace.Range != null) && ((value < colorSpace.Range[0]) || (value > colorSpace.Range[1])))
                {
                    throw new ArgumentOutOfRangeException("A", "a* component must be in the range defined by the Lab colorspace.");
                }

                m_a = value;
            }
        }

        /// <summary>
        /// Gets or sets the b* component for this color. 
        /// </summary>
        /// <value>The b* component of this color.</value>
        /// <remarks>The range for this value is defined by the Range property of the underlying Lab colorspace. </remarks>
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
        /// <seealso cref="PdfLabColorSpace"/> Class            
        /// <seealso cref="Syncfusion.Pdf.Graphics.PdfPen"/> Class
        /// <seealso cref="PdfColorSpaces"/> Class
        public double B
        {
            get
            {
                return m_b;
            }

            set
            {
                PdfLabColorSpace colorSpace = base.ColorSpace as PdfLabColorSpace;
                if ((colorSpace.Range != null) && ((value < colorSpace.Range[2]) || (value > colorSpace.Range[3])))
                {
                    throw new ArgumentOutOfRangeException("B", "b* component must be in the range defined by the Lab colorspace.");
                }

                m_b = value;
            }
        }

        /// <summary>
        /// Gets or sets the l component for this color. 
        /// </summary>
        /// <value>The l component of this color. </value>
        /// <remarks>The acceptable range for this value is [0.0 100.0]. 0.0 means the darkest color that can be achieved, and 100.0 means the lightest color. </remarks>
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
        /// <seealso cref="PdfLabColorSpace"/> Class            
        /// <seealso cref="Syncfusion.Pdf.Graphics.PdfPen"/> Class
        /// <seealso cref="PdfColorSpaces"/> Class
        public double L
        {
            get
            {
                return m_l;
            }

            set
            {
                if ((value < 0.0) || (value > 100.0))
                {
                    throw new ArgumentOutOfRangeException("L", "L must be between 0 and 100");
                }

                m_l = value;
            }
        }
        #endregion
    }
}
