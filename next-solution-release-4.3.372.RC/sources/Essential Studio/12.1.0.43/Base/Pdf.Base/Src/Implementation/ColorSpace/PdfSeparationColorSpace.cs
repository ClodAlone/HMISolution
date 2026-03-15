#region Copyright Syncfusion Inc. 2001 - 2014
// Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Any infringement will be prosecuted under
// applicable laws. 
#endregion

using System;
using Syncfusion.Pdf.Functions;
using Syncfusion.Pdf.Graphics;
using Syncfusion.Pdf.IO;
using Syncfusion.Pdf.Primitives;

/// <summary>
/// The Syncfusion.Pdf.ColorSpace namespace contains classes for enhanced printing support with various Color channels.
/// </summary>
namespace Syncfusion.Pdf.ColorSpace
{
    /// <summary>
    /// Represents a separation colorspace
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
    /// <seealso cref="PdfColorSpaces"/> Class      
    /// <seealso cref="PdfSeparationColor"/> Class            
    /// <seealso cref="Syncfusion.Pdf.Graphics.PdfPen"/> Class   
    public class PdfSeparationColorSpace : PdfColorSpaces, IPdfWrapper
    {
        #region Fields
        /// <summary>
        /// Local variable to store the Coloring.
        /// </summary>
        private string m_colorant;

        /// <summary>
        /// Local variable to store teh Pdffuncion.
        /// </summary>
        private PdfFunction m_function;

        /// <summary>
        /// Local variable to store the internal stream.
        /// </summary>
        private PdfStream m_stream = new PdfStream();

        /// <summary>
        /// Local variable to store the Alternative Colorspaces.
        /// </summary>
        private PdfColorSpaces m_alterantecolorspaces = new PdfDeviceColorSpace(PdfColorSpace.CMYK);
        #endregion
        
        #region Constructors
        /// <summary>
        /// Initializes a new instance of the <see cref="PdfSeparationColorSpace"/> class.
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
        /// <seealso cref="PdfSeparationColor"/> Class            
        /// <seealso cref="Syncfusion.Pdf.Graphics.PdfPen"/> Class
        /// <seealso cref="PdfColorSpaces"/> Class 
        public PdfSeparationColorSpace()
            : base()
        {
            m_stream.Compress = true;
            m_stream.SetProperty(DictionaryProperties.Filter, new PdfName(DictionaryProperties.FlateDecode));
            m_stream.BeginSave += new SavePdfPrimitiveEventHandler(Stream_BeginSave);
            Initialize();
        }
        #endregion

        #region Properties
        /// <summary>
        /// Gets or sets the alternate color spaces.
        /// </summary>
        /// <value>The alternate color space to be used when the destination device does not support separation colorspace.</value>
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
        /// colorspace.AlternateColorSpaces = new PdfDeviceColorSpace(PdfColorSpace.GrayScale);          
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
        /// colorspace.AlternateColorSpaces = New PdfDeviceColorSpace(PdfColorSpace.GrayScale)
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
        /// <seealso cref="PdfSeparationColor"/> Class            
        /// <seealso cref="Syncfusion.Pdf.Graphics.PdfPen"/> Class
        /// <seealso cref="PdfColorSpaces"/> Class 
        public PdfColorSpaces AlternateColorSpaces
        {
            get
            {
                return m_alterantecolorspaces;
            }

            set
            {
                m_alterantecolorspaces = value;
                Initialize();
            }
        }

        /// <summary>
        /// Gets or sets the colorant represented by this separation colorspace. 
        /// </summary>
        /// <value>The name of the colorant.</value>
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
        /// colorspace.AlternateColorSpaces = new PdfDeviceColorSpace(PdfColorSpace.GrayScale);          
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
        /// colorspace.AlternateColorSpaces = New PdfDeviceColorSpace(PdfColorSpace.GrayScale)
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
        /// <seealso cref="PdfSeparationColor"/> Class            
        /// <seealso cref="Syncfusion.Pdf.Graphics.PdfPen"/> Class
        /// <seealso cref="PdfColorSpaces"/> Class 
        public string Colorant
        {
            get
            {
                return m_colorant;
            }

            set
            {
                m_colorant = value;
                Initialize();
            }
        }

        /// <summary>
        /// Gets or sets the tint transform function for the this colorspace. 
        /// </summary>
        /// <value>Tint transform function for the colorspace.</value>
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
        /// colorspace.AlternateColorSpaces = new PdfDeviceColorSpace(PdfColorSpace.GrayScale);          
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
        /// colorspace.AlternateColorSpaces = New PdfDeviceColorSpace(PdfColorSpace.GrayScale)
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
        /// <seealso cref="PdfSeparationColor"/> Class            
        /// <seealso cref="Syncfusion.Pdf.Graphics.PdfPen"/> Class
        /// <seealso cref="PdfColorSpaces"/> Class 
        public PdfFunction TintTransform
        {
            get
            {
                return m_function;
            }

            set
            {
                m_function = value;
                Initialize();
            }
        }
        #endregion

        #region Methods
        /// <summary>
        /// Get the profile data.
        /// </summary>
        /// <returns>The profile data</returns>
        public byte[] GetProfileData()
        {
            byte[] data = new byte[] { };
            return data;
        }

        /// <summary>
        /// Saves an instance.
        /// </summary>
        protected void Save()
        {
            byte[] profileData = null;
            profileData = this.GetProfileData();
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
                PdfName name = new PdfName("Separation");
                colorspace.Add(name);
                if (m_colorant != null)
                {
                    PdfName colorant = new PdfName(m_colorant);
                    colorspace.Add(colorant);
                }
                else
                {
                    PdfName colorant = new PdfName("All");
                    colorspace.Add(colorant);
                }

                if (m_alterantecolorspaces != null)
                {
                    if (m_alterantecolorspaces is PdfCalGrayColorSpace)
                    {
                        PdfName alternate = new PdfName("CalGray");
                        PdfReferenceHolder refhold = new PdfReferenceHolder(m_alterantecolorspaces);
                        colorspace.Add(refhold);
                    }
                    else if (m_alterantecolorspaces is PdfCalRGBColorSpace)
                    {
                        PdfName alternate = new PdfName("CalRGB");
                        PdfReferenceHolder refhold = new PdfReferenceHolder(m_alterantecolorspaces);
                        colorspace.Add(refhold);
                    }
                    else if (m_alterantecolorspaces is PdfLabColorSpace)
                    {
                        PdfName alternate = new PdfName("Lab");
                        PdfReferenceHolder refhold = new PdfReferenceHolder(m_alterantecolorspaces);
                        colorspace.Add(refhold);
                    }
                    else if (m_alterantecolorspaces is PdfDeviceColorSpace)
                    {
                        PdfDeviceColorSpace temp = m_alterantecolorspaces as PdfDeviceColorSpace;
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
                }
                else
                {
                    PdfName alternate = new PdfName("DeviceCMYK");
                    colorspace.Add(alternate);
                }

                if (m_function != null)
                {
                    if (m_alterantecolorspaces is PdfCalGrayColorSpace)
                    {
                        PdfExponentialInterpolationFunction function = m_function as PdfExponentialInterpolationFunction;
                        function.Dictionary.SetProperty(DictionaryProperties.FunctionType, new PdfNumber(2));
                        function.Dictionary.SetProperty(DictionaryProperties.Domain, new PdfArray(new double[] { 0.0, 1.0 }));
                        function.Dictionary.SetProperty(DictionaryProperties.Range, new PdfArray(new double[] { 0.0, 1.0 }));
                        function.Dictionary.SetProperty(DictionaryProperties.C0, new PdfArray(new double[] { 0.0 }));
                        if (function.C1.Length == 1)
                        {
                            function.Dictionary.SetProperty(DictionaryProperties.C1, new PdfArray(function.C1));
                        }
                        else
                        {
                            throw new ArgumentOutOfRangeException();
                        }

                        function.Dictionary.SetProperty(DictionaryProperties.N, new PdfNumber(1));
                        PdfReferenceHolder refHolder = new PdfReferenceHolder(function);
                        colorspace.Add(refHolder);
                    }
                    else if (m_alterantecolorspaces is PdfCalRGBColorSpace)
                    {
                        PdfExponentialInterpolationFunction function = m_function as PdfExponentialInterpolationFunction;
                        function.Dictionary.SetProperty(DictionaryProperties.FunctionType, new PdfNumber(2));
                        function.Dictionary.SetProperty(DictionaryProperties.Domain, new PdfArray(new double[] { 0.0, 1.0 }));
                        function.Dictionary.SetProperty(DictionaryProperties.Range, new PdfArray(new double[] { 0.0, 1.0, 0.0, 1.0, 0.0, 1.0 }));
                        function.Dictionary.SetProperty(DictionaryProperties.C0, new PdfArray(new double[] { 0.0, 0.0, 0.0 }));
                        if (function.C1.Length == 3)
                        {
                            function.Dictionary.SetProperty(DictionaryProperties.C1, new PdfArray(function.C1));
                        }
                        else
                        {
                            throw new ArgumentOutOfRangeException();
                        }

                        function.Dictionary.SetProperty(DictionaryProperties.N, new PdfNumber(1));
                        PdfReferenceHolder refHolder = new PdfReferenceHolder(function);
                        colorspace.Add(refHolder);
                    }
                    else if (m_alterantecolorspaces is PdfLabColorSpace)
                    {
                        PdfExponentialInterpolationFunction function = m_function as PdfExponentialInterpolationFunction;
                        function.Dictionary.SetProperty(DictionaryProperties.FunctionType, new PdfNumber(2));
                        function.Dictionary.SetProperty(DictionaryProperties.Domain, new PdfArray(new double[] { 0.0, 1.0 }));
                        function.Dictionary.SetProperty(DictionaryProperties.Range, new PdfArray(new double[] { 0.0, 100.0, 0.0, 100.0, 0.0, 100.0 }));
                        function.Dictionary.SetProperty(DictionaryProperties.C0, new PdfArray(new double[] { 0.0, 0.0, 0.0 }));
                        if (function.C1.Length == 3)
                        {
                            function.Dictionary.SetProperty(DictionaryProperties.C1, new PdfArray(function.C1));
                        }
                        else
                        {
                            throw new ArgumentOutOfRangeException();
                        }

                        function.Dictionary.SetProperty(DictionaryProperties.N, new PdfNumber(1));
                        PdfReferenceHolder refHolder = new PdfReferenceHolder(function);
                        colorspace.Add(refHolder);
                    }
                    else if (m_alterantecolorspaces is PdfDeviceColorSpace)
                    {
                        PdfDeviceColorSpace temp = m_alterantecolorspaces as PdfDeviceColorSpace;
                        string type = temp.DeviceColorSpaceType.ToString();
                        if (type == "RGB")
                        {
                            PdfExponentialInterpolationFunction function = m_function as PdfExponentialInterpolationFunction;
                            function.Dictionary.SetProperty(DictionaryProperties.FunctionType, new PdfNumber(2));
                            function.Dictionary.SetProperty(DictionaryProperties.Domain, new PdfArray(new double[] { 0.0, 1.0 }));
                            function.Dictionary.SetProperty(DictionaryProperties.Range, new PdfArray(new double[] { 0.0, 1.0, 0.0, 1.0, 0.0, 1.0 }));
                            function.Dictionary.SetProperty(DictionaryProperties.C0, new PdfArray(new double[] { 0.0, 0.0, 0.0 }));
                            if (function.C1.Length == 3)
                            {
                                function.Dictionary.SetProperty(DictionaryProperties.C1, new PdfArray(function.C1));
                            }
                            else
                            {
                                throw new ArgumentOutOfRangeException();
                            }

                            function.Dictionary.SetProperty(DictionaryProperties.N, new PdfNumber(1));
                            PdfReferenceHolder refHolder = new PdfReferenceHolder(function);
                            colorspace.Add(refHolder);
                        }
                        else if (type == "CMYK")
                        {
                            PdfExponentialInterpolationFunction function = m_function as PdfExponentialInterpolationFunction;
                            function.Dictionary.SetProperty(DictionaryProperties.FunctionType, new PdfNumber(2));
                            function.Dictionary.SetProperty(DictionaryProperties.C0, new PdfArray(function.C0));
                            if (function.C1.Length == 4)
                            {
                                function.Dictionary.SetProperty(DictionaryProperties.C1, new PdfArray(function.C1));
                            }
                            else
                            {
                                throw new ArgumentOutOfRangeException();
                            }

                            function.Dictionary.SetProperty(DictionaryProperties.N, new PdfNumber(1));
                            PdfReferenceHolder refHolder = new PdfReferenceHolder(function);
                            colorspace.Add(refHolder);
                        }
                        else if (type == "GrayScale")
                        {
                            PdfExponentialInterpolationFunction function = m_function as PdfExponentialInterpolationFunction;
                            function.Dictionary.SetProperty(DictionaryProperties.FunctionType, new PdfNumber(2));
                            function.Dictionary.SetProperty(DictionaryProperties.Domain, new PdfArray(new double[] { 0.0, 1.0 }));
                            function.Dictionary.SetProperty(DictionaryProperties.Range, new PdfArray(new double[] { 0.0, 1.0 }));
                            function.Dictionary.SetProperty(DictionaryProperties.C0, new PdfArray(new double[] { 0.0 }));
                            if (function.C1.Length == 1)
                            {
                                function.Dictionary.SetProperty(DictionaryProperties.C1, new PdfArray(function.C1));
                            }
                            else
                            {
                                throw new ArgumentOutOfRangeException();
                            }

                            function.Dictionary.SetProperty(DictionaryProperties.N, new PdfNumber(1));
                            PdfReferenceHolder refHolder = new PdfReferenceHolder(function);
                            colorspace.Add(refHolder);
                        }
                    }
                }
                else
                {
                    float[] domain = new float[] { 0, 1 };
                    float[] range = new float[] { 0, 1, 0, 1, 0, 1, 0, 1 };
                    m_stream.SetProperty(DictionaryProperties.FunctionType, new PdfNumber(4));
                    m_stream.SetProperty(DictionaryProperties.Domain, new PdfArray(domain));
                    m_stream.SetProperty(DictionaryProperties.Range, new PdfArray(range));
                }
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
