#region Copyright Syncfusion Inc. 2001 - 2014
// Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Any infringement will be prosecuted under
// applicable laws. 
#endregion

using Syncfusion.Pdf.Graphics;

/// <summary>
/// The Syncfusion.Pdf.ColorSpace namespace contains classes for enhanced printing support with various Color channels.
/// </summary>
namespace Syncfusion.Pdf.ColorSpace
{
    /// <summary>
    /// Represents a device colorspace.
    /// </summary>
    /// <example>
    /// <code lang="C#">   
    /// //  Create a new PDF document.
    /// PdfDocument doc = new PdfDocument();
    /// //Creates a new page and adds it as the last page of the document.
    /// PdfPage page = doc.Pages.Add();          
    /// RectangleF rect = new RectangleF(20, 70, 200, 100);
    /// PdfExponentialInterpolationFunction function = new PdfExponentialInterpolationFunction(true);
    /// float[] numArray = new float[3];
    /// numArray[0] = 0.38f;
    /// numArray[1] = 0.88f;
    /// function.C1 = numArray;
    /// PdfSeparationColorSpace colorspace = new PdfSeparationColorSpace();           
    /// colorspace.AlternateColorSpaces = new PdfDeviceColorSpace(PdfColorSpace.RGB);
    /// colorspace.TintTransform = function;
    /// colorspace.Colorant = "PANTONE Orange 021 C";
    /// PdfSeparationColor color = new PdfSeparationColor(colorspace);
    /// color.Tint = 0.7;
    /// PdfBrush brush = new PdfSolidBrush(color);
    /// page.Graphics.DrawRectangle(brush, rect);
    /// doc.Save("DeviceColorSpace.pdf");
    /// </code>
    /// <code lang="VB">
    /// '  Create a new PDF document.
    /// Dim doc As PdfDocument = New PdfDocument()
    /// ' Create a page.
    /// Dim page As PdfPage = doc.Pages.Add()
    /// Dim rect As RectangleF = New RectangleF(20, 70, 200, 100)
    /// Dim [function] As PdfExponentialInterpolationFunction = New PdfExponentialInterpolationFunction(True)
    /// Dim numArray() As Single = New Single(2){}
    /// numArray(0) = 0.38f
    /// numArray(1) = 0.88f
    /// [function].C1 = numArray
    /// Dim colorspace As PdfSeparationColorSpace = New PdfSeparationColorSpace()
    /// colorspace.AlternateColorSpaces = New PdfDeviceColorSpace(PdfColorSpace.RGB)
    /// colorspace.TintTransform = [function]
    /// colorspace.Colorant = "PANTONE Orange 021 C"
    /// Dim color As PdfSeparationColor = New PdfSeparationColor(colorspace)
    /// color.Tint = 0.7
    /// Dim brush As PdfBrush = New PdfSolidBrush(color)
    /// page.Graphics.DrawRectangle(brush, rect)
    /// doc.Save("DeviceColorSpace.pdf")
    /// </code>
    ///	</example>  
    /// <seealso cref="Syncfusion.Pdf.Functions.PdfExponentialInterpolationFunction"/> Class         
    /// <seealso cref="Syncfusion.Pdf.Graphics.PdfPen"/> Class
    /// <seealso cref="PdfColorSpaces"/> Class
    public class PdfDeviceColorSpace : PdfColorSpaces
    {
        #region Fields
        /// <summary>
        /// Local variable to store the Device Colospace Type.
        /// </summary>
        private PdfColorSpace m_DeviceColorSpaceType = PdfColorSpace.RGB;
        #endregion.

        #region Constructors
        /// <summary>
        /// Initializes a new instance of the <see cref="PdfDeviceColorSpace"/> class.
        /// </summary>
        /// <param name="colorspace">The colorspace.</param>
        /// <example>
        /// <code lang="C#">   
        /// //  Create a new PDF document.
        /// PdfDocument doc = new PdfDocument();
        /// //Creates a new page and adds it as the last page of the document.
        /// PdfPage page = doc.Pages.Add();          
        /// RectangleF rect = new RectangleF(20, 70, 200, 100);
        /// PdfExponentialInterpolationFunction function = new PdfExponentialInterpolationFunction(true);
        /// float[] numArray = new float[3];
        /// numArray[0] = 0.38f;
        /// numArray[1] = 0.88f;
        /// function.C1 = numArray;
        /// PdfSeparationColorSpace colorspace = new PdfSeparationColorSpace();           
        /// colorspace.AlternateColorSpaces = new PdfDeviceColorSpace(PdfColorSpace.RGB);
        /// colorspace.TintTransform = function;
        /// colorspace.Colorant = "PANTONE Orange 021 C";
        /// PdfSeparationColor color = new PdfSeparationColor(colorspace);
        /// color.Tint = 0.7;
        /// PdfBrush brush = new PdfSolidBrush(color);
        /// page.Graphics.DrawRectangle(brush, rect);
        /// doc.Save("DeviceColorSpace.pdf");
        /// </code>
        /// <code lang="VB">
        /// '  Create a new PDF document.
        /// Dim doc As PdfDocument = New PdfDocument()
        /// ' Create a page.
        /// Dim page As PdfPage = doc.Pages.Add()
        /// Dim rect As RectangleF = New RectangleF(20, 70, 200, 100)
        /// Dim [function] As PdfExponentialInterpolationFunction = New PdfExponentialInterpolationFunction(True)
        /// Dim numArray() As Single = New Single(2){}
        /// numArray(0) = 0.38f
        /// numArray(1) = 0.88f
        /// [function].C1 = numArray
        /// Dim colorspace As PdfSeparationColorSpace = New PdfSeparationColorSpace()
        /// colorspace.AlternateColorSpaces = New PdfDeviceColorSpace(PdfColorSpace.RGB)
        /// colorspace.TintTransform = [function]
        /// colorspace.Colorant = "PANTONE Orange 021 C"
        /// Dim color As PdfSeparationColor = New PdfSeparationColor(colorspace)
        /// color.Tint = 0.7
        /// Dim brush As PdfBrush = New PdfSolidBrush(color)
        /// page.Graphics.DrawRectangle(brush, rect)
        /// doc.Save("DeviceColorSpace.pdf")
        /// </code>
        ///	</example>     
        /// <seealso cref="Syncfusion.Pdf.Functions.PdfExponentialInterpolationFunction"/> Class         
        /// <seealso cref="Syncfusion.Pdf.Graphics.PdfPen"/> Class
        /// <seealso cref="PdfColorSpaces"/> Class
        public PdfDeviceColorSpace(PdfColorSpace colorspace)
        {
            m_DeviceColorSpaceType = colorspace;
        }
        #endregion

        #region Properties
        /// <summary>
        /// Gets or sets the DeviceColorSpaceType
        /// </summary>
        /// <example>
        /// <code lang="C#">
        /// // Creates a new document
        /// PdfDocument doc = new PdfDocument();
        /// //Creates a new page and adds it as the last page of the document
        /// PdfPage page = doc.Pages.Add();
        /// PdfExponentialInterpolationFunction function = new PdfExponentialInterpolationFunction(true);
        /// float[] numArray = new float[1];
        /// numArray[0] = 0.38f;          
        /// function.C1 = numArray;
        /// PdfSeparationColorSpace colorspace = new PdfSeparationColorSpace();
        /// // Creates device color space
        /// PdfDeviceColorSpace deviceColorspace1 = new PdfDeviceColorSpace(PdfColorSpace.CMYK);
        /// deviceColorspace1.DeviceColorSpaceType = PdfColorSpace.GrayScale;
        /// // Set the device color space
        /// colorspace.AlternateColorSpaces = deviceColorspace1;
        /// colorspace.TintTransform = function;
        /// colorspace.Colorant = "PANTONE Orange 021 C";
        /// PdfSeparationColor color = new PdfSeparationColor(colorspace);
        /// color.Tint = 0.7;
        /// PdfBrush brush = new PdfSolidBrush(color);
        /// RectangleF rect = new RectangleF(20, 70, 200, 100);
        /// page.Graphics.DrawRectangle(brush, rect);
        /// doc.Save("DeviceColorSpace.pdf");
        /// </code>
        /// <code lang="VB">
        /// ' Creates a new document
        /// Dim doc As PdfDocument = New PdfDocument()
        /// ' Create a page
        /// Dim page As PdfPage = doc.Pages.Add()
        /// Dim [function] As PdfExponentialInterpolationFunction = New PdfExponentialInterpolationFunction(True)
        /// Dim numArray() As Single = New Single(0){}
        /// numArray(0) = 0.38f
        /// [function].C1 = numArray
        /// Dim colorspace As PdfSeparationColorSpace = New PdfSeparationColorSpace()
        /// ' Creates device color space
        /// Dim deviceColorspace1 As PdfDeviceColorSpace = New PdfDeviceColorSpace(PdfColorSpace.CMYK)
        /// deviceColorspace1.DeviceColorSpaceType = PdfColorSpace.GrayScale
        /// ' Set the device color space
        /// colorspace.AlternateColorSpaces = deviceColorspace1
        /// colorspace.TintTransform = [function]
        /// colorspace.Colorant = "PANTONE Orange 021 C"
        /// Dim color As PdfSeparationColor = New PdfSeparationColor(colorspace)
        /// color.Tint = 0.7
        /// Dim brush As PdfBrush = New PdfSolidBrush(color)
        /// Dim rect As RectangleF = New RectangleF(20, 70, 200, 100)
        /// page.Graphics.DrawRectangle(brush, rect)
        /// doc.Save("DeviceColorSpace.pdf")
        /// </code>
        /// </example>     
        /// <seealso cref="Syncfusion.Pdf.Functions.PdfExponentialInterpolationFunction"/> Class         
        /// <seealso cref="Syncfusion.Pdf.Graphics.PdfPen"/> Class
        /// <seealso cref="PdfColorSpaces"/> Class
        public PdfColorSpace DeviceColorSpaceType
        {
            get
            {
                return m_DeviceColorSpaceType;
            }

            set
            {
                m_DeviceColorSpaceType = value;
            }
        }
        #endregion
    }
}
