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
    /// Represents an ICC color, based on an ICC colorspace.
    /// </summary>
    /// <example>
    /// <code lang="C#">
    /// // Creates a new document
    /// PdfDocument doc = new PdfDocument();
    /// //Creates a new page and adds it as the last page of the document
    /// PdfPage page = doc.Pages.Add();
    /// // Read the ICC profile from disk.
    /// FileStream fs = new FileStream("rgb.icc", FileMode.Open, FileAccess.Read);
    /// byte[] profileData = new byte[fs.Length];
    /// fs.Read(profileData, 0, profileData.Length);
    /// fs.Close();
    /// PdfCalRGBColorSpace calRgbCS = new PdfCalRGBColorSpace();
    /// calRgbCS.Gamma = new double[] { 7.6, 5.1, 8.5 };      
    /// // Creating instance for ICCColorSpace      
    /// PdfICCColorSpace IccBasedCS = new PdfICCColorSpace();
    /// IccBasedCS.ProfileData = profileData;
    /// IccBasedCS.AlternateColorSpace = calRgbCS;
    /// IccBasedCS.ColorComponents = 3;
    /// IccBasedCS.Range = new double[] { 0.0, 1.0, 0.0, 1.0, 0.0, 1.0 };
    /// PdfICCColor iccColorSpace = new PdfICCColor(IccBasedCS);
    /// iccColorSpace.ColorComponents = new double[] { 1, 0, 1 };
    /// PdfPen pen = new PdfPen(iccColorSpace);
    /// RectangleF rect = new RectangleF(20, 70, 200, 100);
    /// page.Graphics.DrawRectangle(pen, rect);
    /// doc.Save("ICCColorCS.pdf");
    /// </code>
    /// <code lang="VB">
    /// ' Creates a new document
    /// Dim doc As PdfDocument = New PdfDocument()
    /// ' Create a page
    /// Dim page As PdfPage = doc.Pages.Add()
    /// ' Read the ICC profile from disk.
    /// Dim fs As FileStream = New FileStream("rgb.icc", FileMode.Open, FileAccess.Read)
    /// Dim profileData() As Byte = New Byte(fs.Length - 1){}
    /// fs.Read(profileData, 0, profileData.Length)
    /// fs.Close()
    /// Dim calRgbCS As PdfCalRGBColorSpace = New PdfCalRGBColorSpace()
    /// calRgbCS.Gamma = New Double() { 7.6, 5.1, 8.5 }
    /// ' Creating instance for ICCColorSpace      
    /// Dim IccBasedCS As PdfICCColorSpace = New PdfICCColorSpace()
    /// IccBasedCS.ProfileData = profileData
    /// IccBasedCS.AlternateColorSpace = calRgbCS
    /// IccBasedCS.ColorComponents = 3
    /// IccBasedCS.Range = New Double() { 0.0, 1.0, 0.0, 1.0, 0.0, 1.0 }
    /// Dim iccColorSpace As PdfICCColor = New PdfICCColor(IccBasedCS)
    /// iccColorSpace.ColorComponents = New Double() { 1, 0, 1 }
    /// Dim pen As PdfPen = New PdfPen(iccColorSpace)
    /// Dim rect As RectangleF = New RectangleF(20, 70, 200, 100)
    /// page.Graphics.DrawRectangle(pen, rect)
    /// doc.Save("ICCColorCS.pdf")
    /// </code>
    /// </example>
    /// <seealso cref="PdfExtendedColor"/> Class    
    /// <seealso cref="PdfCalRGBColorSpace"/> Class    
    /// <seealso cref="PdfICCColorSpace"/> Class       
    /// <seealso cref="Syncfusion.Pdf.Graphics.PdfPen"/> Class
    /// <seealso cref="PdfColorSpaces"/> Class
    public class PdfICCColor : PdfExtendedColor
    {
        #region Fields
        /// <summary>
        /// Local variable to store the color components.
        /// </summary>
        private double[] m_components;

        /// <summary>
        /// Local variable to store the Alternatic Colorspace of this Color.
        /// </summary>
        private PdfICCColorSpace m_colorspaces;
        #endregion

        #region Constructors
        /// <summary>
        /// Initializes a new instance of the <see cref="PdfICCColor"/> class.
        /// </summary>
        /// <param name="colorspace">The colorspace.</param>
        /// <example>
        /// <code lang="C#">
        /// // Creates a new document
        /// PdfDocument doc = new PdfDocument();
        /// //Creates a new page and adds it as the last page of the document
        /// PdfPage page = doc.Pages.Add();
        /// // Read the ICC profile from disk.
        /// FileStream fs = new FileStream("rgb.icc", FileMode.Open, FileAccess.Read);
        /// byte[] profileData = new byte[fs.Length];
        /// fs.Read(profileData, 0, profileData.Length);
        /// fs.Close();
        /// PdfCalRGBColorSpace calRgbCS = new PdfCalRGBColorSpace();
        /// calRgbCS.Gamma = new double[] { 7.6, 5.1, 8.5 };      
        /// // Creating instance for ICCColorSpace      
        /// PdfICCColorSpace IccBasedCS = new PdfICCColorSpace();
        /// IccBasedCS.ProfileData = profileData;
        /// IccBasedCS.AlternateColorSpace = calRgbCS;
        /// IccBasedCS.ColorComponents = 3;
        /// IccBasedCS.Range = new double[] { 0.0, 1.0, 0.0, 1.0, 0.0, 1.0 };
        /// PdfICCColor iccColorSpace = new PdfICCColor(IccBasedCS);
        /// iccColorSpace.ColorComponents = new double[] { 1, 0, 1 };
        /// PdfPen pen = new PdfPen(iccColorSpace);
        /// RectangleF rect = new RectangleF(20, 70, 200, 100);
        /// page.Graphics.DrawRectangle(pen, rect);
        /// doc.Save("ICCColorCS.pdf");
        /// </code>
        /// <code lang="VB">
        /// ' Creates a new document
        /// Dim doc As PdfDocument = New PdfDocument()
        /// ' Create a page
        /// Dim page As PdfPage = doc.Pages.Add()
        /// ' Read the ICC profile from disk.
        /// Dim fs As FileStream = New FileStream("rgb.icc", FileMode.Open, FileAccess.Read)
        /// Dim profileData() As Byte = New Byte(fs.Length - 1){}
        /// fs.Read(profileData, 0, profileData.Length)
        /// fs.Close()
        /// Dim calRgbCS As PdfCalRGBColorSpace = New PdfCalRGBColorSpace()
        /// calRgbCS.Gamma = New Double() { 7.6, 5.1, 8.5 }
        /// ' Creating instance for ICCColorSpace      
        /// Dim IccBasedCS As PdfICCColorSpace = New PdfICCColorSpace()
        /// IccBasedCS.ProfileData = profileData
        /// IccBasedCS.AlternateColorSpace = calRgbCS
        /// IccBasedCS.ColorComponents = 3
        /// IccBasedCS.Range = New Double() { 0.0, 1.0, 0.0, 1.0, 0.0, 1.0 }
        /// Dim iccColorSpace As PdfICCColor = New PdfICCColor(IccBasedCS)
        /// iccColorSpace.ColorComponents = New Double() { 1, 0, 1 }
        /// Dim pen As PdfPen = New PdfPen(iccColorSpace)
        /// Dim rect As RectangleF = New RectangleF(20, 70, 200, 100)
        /// page.Graphics.DrawRectangle(pen, rect)
        /// doc.Save("ICCColorCS.pdf")
        /// </code>
        /// </example>
        /// <seealso cref="PdfExtendedColor"/> Class    
        /// <seealso cref="PdfCalRGBColorSpace"/> Class    
        /// <seealso cref="PdfICCColorSpace"/> Class       
        /// <seealso cref="Syncfusion.Pdf.Graphics.PdfPen"/> Class
        /// <seealso cref="PdfColorSpaces"/> Class
        public PdfICCColor(PdfColorSpaces colorspace)
            : base(colorspace)
        {
            m_colorspaces = colorspace as PdfICCColorSpace;
        }
        #endregion

        #region Properties
        /// <summary>
        /// Gets or sets the color components. 
        /// </summary>
        /// <value>An array of values that describe the color in the ICC colorspace. </value>
        /// <remarks>The length of this array must match the value of ColorComponents property on the underlying ICC colorspace. </remarks>
        /// <example>
        /// <code lang="C#">
        /// // Creates a new document
        /// PdfDocument doc = new PdfDocument();
        /// //Creates a new page and adds it as the last page of the document
        /// PdfPage page = doc.Pages.Add();
        /// // Read the ICC profile from disk.
        /// FileStream fs = new FileStream("rgb.icc", FileMode.Open, FileAccess.Read);
        /// byte[] profileData = new byte[fs.Length];
        /// fs.Read(profileData, 0, profileData.Length);
        /// fs.Close();
        /// PdfCalRGBColorSpace calRgbCS = new PdfCalRGBColorSpace();
        /// calRgbCS.Gamma = new double[] { 7.6, 5.1, 8.5 };      
        /// // Creating instance for ICCColorSpace      
        /// PdfICCColorSpace IccBasedCS = new PdfICCColorSpace();
        /// IccBasedCS.ProfileData = profileData;
        /// IccBasedCS.AlternateColorSpace = calRgbCS;
        /// IccBasedCS.ColorComponents = 3;
        /// IccBasedCS.Range = new double[] { 0.0, 1.0, 0.0, 1.0, 0.0, 1.0 };
        /// PdfICCColor iccColorSpace = new PdfICCColor(IccBasedCS);
        /// iccColorSpace.ColorComponents = new double[] { 1, 0, 1 };
        /// PdfPen pen = new PdfPen(iccColorSpace);
        /// RectangleF rect = new RectangleF(20, 70, 200, 100);
        /// page.Graphics.DrawRectangle(pen, rect);
        /// doc.Save("ICCColorCS.pdf");
        /// </code>
        /// <code lang="VB">
        /// ' Creates a new document
        /// Dim doc As PdfDocument = New PdfDocument()
        /// ' Create a page
        /// Dim page As PdfPage = doc.Pages.Add()
        /// ' Read the ICC profile from disk.
        /// Dim fs As FileStream = New FileStream("rgb.icc", FileMode.Open, FileAccess.Read)
        /// Dim profileData() As Byte = New Byte(fs.Length - 1){}
        /// fs.Read(profileData, 0, profileData.Length)
        /// fs.Close()
        /// Dim calRgbCS As PdfCalRGBColorSpace = New PdfCalRGBColorSpace()
        /// calRgbCS.Gamma = New Double() { 7.6, 5.1, 8.5 }
        /// ' Creating instance for ICCColorSpace      
        /// Dim IccBasedCS As PdfICCColorSpace = New PdfICCColorSpace()
        /// IccBasedCS.ProfileData = profileData
        /// IccBasedCS.AlternateColorSpace = calRgbCS
        /// IccBasedCS.ColorComponents = 3
        /// IccBasedCS.Range = New Double() { 0.0, 1.0, 0.0, 1.0, 0.0, 1.0 }
        /// Dim iccColorSpace As PdfICCColor = New PdfICCColor(IccBasedCS)
        /// iccColorSpace.ColorComponents = New Double() { 1, 0, 1 }
        /// Dim pen As PdfPen = New PdfPen(iccColorSpace)
        /// Dim rect As RectangleF = New RectangleF(20, 70, 200, 100)
        /// page.Graphics.DrawRectangle(pen, rect)
        /// doc.Save("ICCColorCS.pdf")
        /// </code>
        /// </example>
        /// <seealso cref="PdfExtendedColor"/> Class    
        /// <seealso cref="PdfCalRGBColorSpace"/> Class    
        /// <seealso cref="PdfICCColorSpace"/> Class       
        /// <seealso cref="Syncfusion.Pdf.Graphics.PdfPen"/> Class
        /// <seealso cref="PdfColorSpaces"/> Class
        public double[] ColorComponents
        {
            get
            {
                return m_components;
            }

            set
            {
                if (value == null)
                {
                    throw new ArgumentNullException("ColorComponents", "ColorComponents array cannot be null.");
                }

                PdfICCColorSpace colorSpace = base.ColorSpace as PdfICCColorSpace;
                if (value.Length != colorSpace.ColorComponents)
                {
                    throw new ArgumentOutOfRangeException("ColorComponents", "Array length must match the number of color components defined on the underlying ICC colorspace.");
                }

                m_components = value;
            }
        }

        /// <summary>
        /// Gets the Colorspace.
        /// </summary>
        internal PdfICCColorSpace ColorSpaces
        {
            get
            {
                return m_colorspaces;
            }
        }
        #endregion
    }
}
