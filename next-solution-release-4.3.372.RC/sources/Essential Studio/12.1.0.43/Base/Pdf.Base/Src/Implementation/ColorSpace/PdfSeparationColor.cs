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
    /// Represents a separation color, based on a separation colorspace. 
    /// </summary>
    /// <example>
    /// <code lang="C#">
    /// // Creates a new document
    /// PdfDocument doc = new PdfDocument();
    /// //Creates a new page and adds it as the last page of the document
    /// PdfPage page = doc.Pages.Add();
    /// // Creates ExponentialInterpolationFunction function
    /// PdfExponentialInterpolationFunction function = new PdfExponentialInterpolationFunction(true);
    /// float[] numArray = new float[4];
    /// numArray[0] = 0.38f;
    /// numArray[1] = 0.88f;
    /// function.C1 = numArray;
    /// // Creates SeparationColorSpace
    /// PdfSeparationColorSpace colorspace = new PdfSeparationColorSpace();
    /// colorspace.TintTransform = function;
    /// colorspace.Colorant = "PANTONE Orange 021 C";
    /// PdfSeparationColor color = new PdfSeparationColor(colorspace);
    /// color.Tint = 0.7;
    /// RectangleF rect = new RectangleF(20, 70, 200, 100);
    /// PdfPen pen = new PdfPen(color);
    /// page.Graphics.DrawRectangle(pen, rect);    
    /// doc.Save("SeparationColor.pdf");
    /// </code>
    /// <code lang="VB">
    /// ' Creates a new document
    /// Dim doc As PdfDocument = New PdfDocument()
    /// ' Create a page
    /// Dim page As PdfPage = doc.Pages.Add()
    /// ' Creates ExponentialInterpolationFunction function
    /// Dim [function] As PdfExponentialInterpolationFunction = New PdfExponentialInterpolationFunction(True)
    /// Dim numArray() As Single = New Single(3){}
    /// numArray(0) = 0.38f
    /// numArray(1) = 0.88f
    /// [function].C1 = numArray
    /// ' Creates SeparationColorSpace
    /// Dim colorspace As PdfSeparationColorSpace = New PdfSeparationColorSpace()
    /// colorspace.TintTransform = [function]
    /// colorspace.Colorant = "PANTONE Orange 021 C"
    /// Dim color As PdfSeparationColor = New PdfSeparationColor(colorspace)
    /// color.Tint = 0.7
    /// Dim rect As RectangleF = New RectangleF(20, 70, 200, 100)
    /// Dim pen As PdfPen = New PdfPen(color)
    /// page.Graphics.DrawRectangle(pen, rect)
    /// doc.Save("SeparationColor.pdf")
    /// </code>
    /// </example>
    /// <seealso cref="PdfExtendedColor"/> Class      
    /// <seealso cref="PdfSeparationColorSpace"/> Class            
    /// <seealso cref="Syncfusion.Pdf.Graphics.PdfPen"/> Class
    /// <seealso cref="PdfColorSpaces"/> Class
    public class PdfSeparationColor : PdfExtendedColor
    {
        #region Fileds
        /// <summary>
        /// Local variable to store the tint value.
        /// </summary>
        private double m_tint = 1;

        #endregion

        #region Constructor
        /// <summary>
        /// Initializes a new instance of the <see cref="PdfSeparationColor"/> class.
        /// </summary>
        /// <param name="colorspace">The colorspace.</param>
        /// <example>
        /// <code lang="C#">
        /// // Creates a new document
        /// PdfDocument doc = new PdfDocument();
        /// //Creates a new page and adds it as the last page of the document
        /// PdfPage page = doc.Pages.Add();
        /// // Creates ExponentialInterpolationFunction function
        /// PdfExponentialInterpolationFunction function = new PdfExponentialInterpolationFunction(true);
        /// float[] numArray = new float[4];
        /// numArray[0] = 0.38f;
        /// numArray[1] = 0.88f;
        /// function.C1 = numArray;
        /// // Creates SeparationColorSpace
        /// PdfSeparationColorSpace colorspace = new PdfSeparationColorSpace();
        /// colorspace.TintTransform = function;
        /// colorspace.Colorant = "PANTONE Orange 021 C";
        /// PdfSeparationColor color = new PdfSeparationColor(colorspace);
        /// color.Tint = 0.7;
        /// RectangleF rect = new RectangleF(20, 70, 200, 100);
        /// PdfPen pen = new PdfPen(color);
        /// page.Graphics.DrawRectangle(pen, rect);    
        /// doc.Save("SeparationColor.pdf");
        /// </code>
        /// <code lang="VB">
        /// ' Creates a new document
        /// Dim doc As PdfDocument = New PdfDocument()
        /// ' Create a page
        /// Dim page As PdfPage = doc.Pages.Add()
        /// ' Creates ExponentialInterpolationFunction function
        /// Dim [function] As PdfExponentialInterpolationFunction = New PdfExponentialInterpolationFunction(True)
        /// Dim numArray() As Single = New Single(3){}
        /// numArray(0) = 0.38f
        /// numArray(1) = 0.88f
        /// [function].C1 = numArray
        /// ' Creates SeparationColorSpace
        /// Dim colorspace As PdfSeparationColorSpace = New PdfSeparationColorSpace()
        /// colorspace.TintTransform = [function]
        /// colorspace.Colorant = "PANTONE Orange 021 C"
        /// Dim color As PdfSeparationColor = New PdfSeparationColor(colorspace)
        /// color.Tint = 0.7
        /// Dim rect As RectangleF = New RectangleF(20, 70, 200, 100)
        /// Dim pen As PdfPen = New PdfPen(color)
        /// page.Graphics.DrawRectangle(pen, rect)
        /// doc.Save("SeparationColor.pdf")
        /// </code>
        /// </example>
        /// <seealso cref="PdfExtendedColor"/> Class      
        /// <seealso cref="PdfSeparationColorSpace"/> Class            
        /// <seealso cref="Syncfusion.Pdf.Graphics.PdfPen"/> Class
        /// <seealso cref="PdfColorSpaces"/> Class
        public PdfSeparationColor(PdfColorSpaces colorspace)
            : base(colorspace)
        {
        }
        #endregion

        #region Properties
        /// <summary>
        /// Gets or sets the Tint Value.
        /// </summary>
        /// <value>A float value specifying the tint of this color.</value>
        /// <remarks>The acceptable range for this value is [0.0 1.0]. 0.0 means the lightest color that can be achieved, and 1.0 means the darkest color.</remarks>
        /// <example>
        /// <code lang="C#">
        /// // Creates a new document
        /// PdfDocument doc = new PdfDocument();
        /// //Creates a new page and adds it as the last page of the document
        /// PdfPage page = doc.Pages.Add();
        /// // Creates ExponentialInterpolationFunction function
        /// PdfExponentialInterpolationFunction function = new PdfExponentialInterpolationFunction(true);
        /// float[] numArray = new float[4];
        /// numArray[0] = 0.38f;
        /// numArray[1] = 0.88f;
        /// function.C1 = numArray;
        /// // Creates SeparationColorSpace
        /// PdfSeparationColorSpace colorspace = new PdfSeparationColorSpace();
        /// colorspace.TintTransform = function;
        /// colorspace.Colorant = "PANTONE Orange 021 C";
        /// PdfSeparationColor color = new PdfSeparationColor(colorspace);
        /// color.Tint = 0.7;
        /// RectangleF rect = new RectangleF(20, 70, 200, 100);
        /// PdfPen pen = new PdfPen(color);
        /// page.Graphics.DrawRectangle(pen, rect);    
        /// doc.Save("SeparationColor.pdf");
        /// </code>
        /// <code lang="VB">
        /// ' Creates a new document
        /// Dim doc As PdfDocument = New PdfDocument()
        /// ' Create a page
        /// Dim page As PdfPage = doc.Pages.Add()
        /// ' Creates ExponentialInterpolationFunction function
        /// Dim [function] As PdfExponentialInterpolationFunction = New PdfExponentialInterpolationFunction(True)
        /// Dim numArray() As Single = New Single(3){}
        /// numArray(0) = 0.38f
        /// numArray(1) = 0.88f
        /// [function].C1 = numArray
        /// ' Creates SeparationColorSpace
        /// Dim colorspace As PdfSeparationColorSpace = New PdfSeparationColorSpace()
        /// colorspace.TintTransform = [function]
        /// colorspace.Colorant = "PANTONE Orange 021 C"
        /// Dim color As PdfSeparationColor = New PdfSeparationColor(colorspace)
        /// color.Tint = 0.7
        /// Dim rect As RectangleF = New RectangleF(20, 70, 200, 100)
        /// Dim pen As PdfPen = New PdfPen(color)
        /// page.Graphics.DrawRectangle(pen, rect)
        /// doc.Save("SeparationColor.pdf")
        /// </code>
        /// </example>
        /// <seealso cref="PdfExtendedColor"/> Class      
        /// <seealso cref="PdfSeparationColorSpace"/> Class            
        /// <seealso cref="Syncfusion.Pdf.Graphics.PdfPen"/> Class
        /// <seealso cref="PdfColorSpaces"/> Class
        public double Tint
        {
            get
            {
                return m_tint;
            }

            set
            {
                m_tint = value;
            }
        }
        #endregion
    }
}
