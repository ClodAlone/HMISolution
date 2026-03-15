#region Copyright Syncfusion Inc. 2001 - 2014
// Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Any infringement will be prosecuted under
// applicable laws. 
#endregion

using Syncfusion.Pdf.Graphics;
using Syncfusion.Pdf.Primitives;

/// <summary>
/// The Syncfusion.Pdf.ColorSpace namespace contains classes for enhanced printing support with various Color channels.
/// </summary>
namespace Syncfusion.Pdf.ColorSpace
{
    /// <summary>
    /// Represents an indexed colorspace.
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
    /// <seealso cref="PdfColorSpaces"/> Class      
    /// <seealso cref="PdfIndexedColor"/> Class    
    /// <seealso cref="PdfDeviceColorSpace"/> Class      
    /// <seealso cref="Syncfusion.Pdf.Graphics.PdfPen"/> Class  
    public class PdfIndexedColorSpace : PdfColorSpaces
    {
        #region Fields
        /// <summary>
        /// Local variable to store the base colorspace.
        /// </summary>
        private PdfColorSpaces m_basecolorspace = new PdfDeviceColorSpace(PdfColorSpace.RGB);

        /// <summary>
        /// Local variable to store the maximum Color Index.
        /// </summary>
        private int m_maxColorIndex = 0;

        /// <summary>
        /// Local variable to store the indexed Color Table.
        /// </summary>
        private byte[] m_indexedColorTable;

        /// <summary>
        /// Local variable to store the stream.
        /// </summary>
        private PdfStream m_stream = new PdfStream();
        #endregion

        #region Constructor
        /// <summary>
        /// Initializes a new instance of the <see cref="PdfIndexedColorSpace"/> class.
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
        /// <seealso cref="PdfIndexedColor"/> Class    
        /// <seealso cref="PdfDeviceColorSpace"/> Class      
        /// <seealso cref="Syncfusion.Pdf.Graphics.PdfPen"/> Class
        /// <seealso cref="PdfColorSpaces"/> Class
        public PdfIndexedColorSpace()
            : base()
        {
            m_stream.BeginSave += new SavePdfPrimitiveEventHandler(Stream_BeginSave);
            Initialize();
        }
        #endregion

        #region Properties
        /// <summary>
        /// Gets or sets the base colorspace. 
        /// </summary>
        /// <value>The color space in which the values in the color table are to be interpreted.</value>
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
        /// <seealso cref="PdfIndexedColor"/> Class    
        /// <seealso cref="PdfDeviceColorSpace"/> Class      
        /// <seealso cref="Syncfusion.Pdf.Graphics.PdfPen"/> Class
        /// <seealso cref="PdfColorSpaces"/> Class
        public PdfColorSpaces BaseColorSpace
        {
            get
            {
                return m_basecolorspace;
            }

            set
            {
                m_basecolorspace = value;
                Initialize();
            }
        }

        /// <summary>
        /// Gets or sets the index of the max color.
        /// </summary>
        /// <value>The maximum index that can be used to access the values in the color table.</value>
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
        /// <seealso cref="PdfIndexedColor"/> Class    
        /// <seealso cref="PdfDeviceColorSpace"/> Class      
        /// <seealso cref="Syncfusion.Pdf.Graphics.PdfPen"/> Class
        /// <seealso cref="PdfColorSpaces"/> Class
        public int MaxColorIndex
        {
            get
            {
                return m_maxColorIndex;
            }

            set
            {
                m_maxColorIndex = value;
                Initialize();
            }
        }

        /// <summary>
        /// Gets or sets the color table. 
        /// </summary>
        /// <value>The table of color components.</value>
        /// <remarks>The color table data must be m * (maxIndex + 1) bytes long, where m is the number of color components in the base color space. Each byte is an unsigned integer in the range 0 to 255 that is scaled to the range of the corresponding color component in the base color space; that is, 0 corresponds to the minimum value in the range for that component, and 255 corresponds to the maximum.</remarks>
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
        /// <seealso cref="PdfIndexedColor"/> Class    
        /// <seealso cref="PdfDeviceColorSpace"/> Class      
        /// <seealso cref="Syncfusion.Pdf.Graphics.PdfPen"/> Class
        /// <seealso cref="PdfColorSpaces"/> Class
        public byte[] IndexedColorTable
        {
            get
            {
                return m_indexedColorTable;
            }

            set
            {
                m_indexedColorTable = value;
                Initialize();
            }
        }
        #endregion

        #region Methods
        /// <summary>
        /// Gets the profile data.
        /// </summary>
        /// <returns>The profile data.</returns>
        public byte[] GetProfileData()
        {
            byte[] data = new byte[1000];
            data = m_indexedColorTable;
            return data;
        }

        /// <summary>
        /// Saves an instance.
        /// </summary>
        protected void Save()
        {
            byte[] profileData = null;
            if (m_indexedColorTable == null)
            {
                profileData = this.GetProfileData();
            }
            else
            {
                profileData = m_indexedColorTable;
            }

            m_stream.Clear();
            m_stream.InternalStream.Write(profileData, 0, profileData.Length);
        }

        /// <summary>
        /// Initializes the PdfICCBased Colorspace.
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
                PdfName name = new PdfName("Indexed");
                colorspace.Add(name);
                PdfReferenceHolder refHolder = new PdfReferenceHolder(m_stream);
                if (m_basecolorspace != null)
                {
                    if (m_basecolorspace is PdfCalGrayColorSpace)
                    {
                        PdfReferenceHolder refhold = new PdfReferenceHolder(m_basecolorspace);
                        colorspace.Add(refhold);
                    }
                    else if (m_basecolorspace is PdfCalRGBColorSpace)
                    {
                        PdfReferenceHolder refhold = new PdfReferenceHolder(m_basecolorspace);
                        colorspace.Add(refhold);
                    }
                    else if (m_basecolorspace is PdfLabColorSpace)
                    {
                        PdfReferenceHolder refhold = new PdfReferenceHolder(m_basecolorspace);
                        colorspace.Add(refhold);
                    }
                    else if (m_basecolorspace is PdfDeviceColorSpace)
                    {
                        PdfDeviceColorSpace temp = m_basecolorspace as PdfDeviceColorSpace;
                        string type = temp.DeviceColorSpaceType.ToString();
                        if (type == "RGB")
                        {
                            PdfName alternate = new PdfName("DeviceRGB");
                            colorspace.Add(alternate);
                        }
                        else if (type == "CMYK")
                        {
                            PdfName alternate = new PdfName("DeviceCMYK");
                            colorspace.Add(alternate);
                        }
                        else if (type == "GrayScale")
                        {
                            PdfName alternate = new PdfName("DeviceGray");
                            colorspace.Add(alternate);
                        }
                    }

                    colorspace.Add(new PdfNumber(m_maxColorIndex));
                }

                colorspace.Add(refHolder);
            }

            return colorspace;
        }

        /// <summary>
        /// Handles the BeginSave event of the Stream control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="ars">The <see cref="Syncfusion.Pdf.Primitives.SavePdfPrimitiveEventArgs"/> instance containing the event data.</param>
        private void Stream_BeginSave(object sender, SavePdfPrimitiveEventArgs ars)
        {
            Save();
        }
        #endregion
    }
}
