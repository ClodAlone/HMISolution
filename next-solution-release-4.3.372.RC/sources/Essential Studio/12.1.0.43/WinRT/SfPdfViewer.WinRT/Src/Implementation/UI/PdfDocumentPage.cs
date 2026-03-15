#region Copyright Syncfusion Inc. 2001 - 2014
// Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Any infringement will be prosecuted under
// applicable laws. 
#endregion
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Syncfusion.Pdf;
using Syncfusion.Pdf.Graphics;
using Syncfusion.Pdf.IO;
using Syncfusion.Pdf.Primitives;
using Windows.UI.Xaml.Controls;
#if SyncfusionFramework4_5
using Syncfusion.DirectXWrapper.WinRT;
namespace Syncfusion.Windows.PdfViewer
{
    class PdfDocumentPage
    {
#region Members
        internal List<RectangleF> matchTextPositions = new List<RectangleF>();
        internal int pageId = -1;
        private Syncfusion.DirectXWrapper.WinRT.Matrix m_formMatrix = new Syncfusion.DirectXWrapper.WinRT.Matrix(1,0,0,1,0,0);
        private String m_webLink = String.Empty;
        private String m_annotType = String.Empty;

        PdfUnitConvertor m_unitConverter = new PdfUnitConvertor();
        internal PdfPageResources m_resources;
        internal PdfRecordCollection m_recordCollection;
        Rectangle m_bounds;

        int m_actualWidth;
        int m_actualHeight;
        int m_currentLocation;
        PdfPageBase m_page;

        //Image pageImage = new Image();
        #endregion


#region Properties
        public int ActualWidth
        {
            get
            {
                return m_actualWidth;
            }
        }
        internal Syncfusion.DirectXWrapper.WinRT.Matrix FormMatrix
        {
            get
            {
                return m_formMatrix;
            }
            set
            {
                m_formMatrix = value;
            }
        }
        public int CurrentLocation
        {
            get
            {
                return m_currentLocation;
            }
            set
            {
                m_currentLocation = value;
            }
        }

        public int ActualHeight
        {
            get
            {
                return m_actualHeight;
            }
        }

        public Rectangle Bounds
        {
            get
            {
                return m_bounds;
            }
            set
            {
                m_bounds = value;
            }
        }

        public int Width
        {
            get
            {
                return m_bounds.Width;
            }
            set
            {
                m_bounds.Width = value;
            }
        }

        public int Height
        {
            get
            {
                return m_bounds.Height;
            }
            set
            {
                m_bounds.Height = value;
            }
        }

        internal PdfPageResources Resources
        {
            get
            {
                return m_resources;
            }
        }

        internal PdfRecordCollection RecordCollection
        {
            get
            {
                return m_recordCollection;
            }
        }

        internal float[] CropBox
        {
            get
            {
                float[] crop = new float[] { this.Bounds.X, this.Bounds.Y, this.Bounds.Width, this.Bounds.Height };
                PdfDictionary dict = this.m_page.Dictionary;
                if (dict.ContainsKey(DictionaryProperties.CropBox))
                {
                    PdfArray cropbox = dict[DictionaryProperties.CropBox] as PdfArray;
                    for (int i = 0; i < cropbox.Count; i++)
                    {
                        crop[i] = (cropbox[i] as PdfNumber).FloatValue;
                    }
                    return crop;
                }
                return null;
            }
        }
        #endregion

#region Constructors

        public PdfDocumentPage(PdfPageBase page)
        {
            this.m_page = page;
        }
        #endregion
        private static object s_lock = new object();

        internal void Initialize(PdfPageBase page, bool needParsing)
        {
            lock (s_lock)
            {
#if !DEBUG
            try
            {
#endif
                if (needParsing && m_recordCollection == null)
                {
                    m_resources = PageResourceLoader.Instance.GetPageResources(page);

                    using (MemoryStream stream = new MemoryStream())
                    {
                        page.Layers.CombineContent(stream);
                        stream.Position = 0;

                        ContentParser parser = new ContentParser(stream.ToArray());
                        m_recordCollection = parser.ReadContent();
                    }
                }
                Size clientRectangleSize = new Size((int)m_unitConverter.ConvertToPixels(page.Size.Width, PdfGraphicsUnit.Point),
                    (int)m_unitConverter.ConvertToPixels(page.Size.Height, PdfGraphicsUnit.Point));

                if (page.Dictionary.ContainsKey(DictionaryProperties.CropBox))
                {
                    RectangleF rectbound = new RectangleF(this.Bounds.X, this.Bounds.Y, this.Bounds.Right, this.Bounds.Bottom);
                    float[] crop = this.CropBox;
                    RectangleF rectcrop = new RectangleF(crop[0], crop[1], crop[0] + crop[2], crop[1] + crop[3]);
                    RectangleF rect = m_unitConverter.ConvertToPixels(rectcrop, PdfGraphicsUnit.Point);
                    if (rectcrop != rectbound && rect != rectbound && (rectcrop.X != 0 && rectcrop.Y != 0 && rectcrop.Width != page.Size.Width && rectcrop.Height != page.Size.Height))
                    {
                        this.Bounds = new Rectangle((int)rect.X, (int)rect.Y, (int)rect.Width, (int)rect.Height);
                    }
                    else if (rectcrop != rectbound && rect != rectbound && (rectcrop.X != 0 && rectcrop.Y != 0 && rectcrop.Width == page.Size.Width && rectcrop.Height == page.Size.Height))
                    {
                        this.Bounds = new Rectangle((int)rect.X, (int)rect.Y, (int)rect.Width, (int)rect.Height);
                    }
                    else if (rectcrop != rectbound && rect != rectbound && (rectcrop.X == 0 && rectcrop.Y == 0 && rectcrop.Width != page.Size.Width && rectcrop.Height == page.Size.Height))
                    {
                        this.Bounds = new Rectangle((int)rect.X, (int)rect.Y, (int)rect.Width, (int)rect.Height + (int)rect.Y);
                    }
                    else if (rectcrop != rectbound && rect != rectbound && (rectcrop.X != 0 && rectcrop.Y == 0 && rectcrop.Width == page.Size.Width && rectcrop.Height == page.Size.Height))
                    {
                        this.Bounds = new Rectangle((int)rect.X, (int)rect.Y, (int)rect.Width, (int)rect.Height + (int)rect.Y);
                    }
                    else
                    {
                        Width = clientRectangleSize.Width;
                        Height = clientRectangleSize.Height;
                    }
                }
                else
                {
                    Width = clientRectangleSize.Width;
                    Height = clientRectangleSize.Height;
                }

                m_actualWidth = Width;
                m_actualHeight = Height;
#if !DEBUG
            }
            catch (Exception msg)
            {
                //errorText = msg.StackTrace;
            }
#endif
            }
        }
    }
}
#else
namespace Syncfusion.Windows.PdfViewer
{
    class PdfDocumentPage
    {
        #region Members
        internal List<RectangleF> matchTextPositions = new List<RectangleF>();
        internal int pageId = -1;
        private global::Windows.UI.Xaml.Media.Matrix m_formMatrix = new global::Windows.UI.Xaml.Media.Matrix(1, 0, 0, 1, 0, 0);
        private String m_webLink = String.Empty;
        private String m_annotType = String.Empty;

        PdfUnitConvertor m_unitConverter = new PdfUnitConvertor();
        internal PdfPageResources m_resources;
        internal PdfRecordCollection m_recordCollection;
        Rectangle m_bounds;

        int m_actualWidth;
        int m_actualHeight;
        int m_currentLocation;
        PdfPageBase m_page;

        //Image pageImage = new Image();
        #endregion

        #region Properties
        public int ActualWidth
        {
            get
            {
                return m_actualWidth;
            }
        }
        internal global::Windows.UI.Xaml.Media.Matrix FormMatrix
        {
            get
            {
                return m_formMatrix;
            }
            set
            {
                m_formMatrix = value;
            }
        }
        public int CurrentLocation
        {
            get
            {
                return m_currentLocation;
            }
            set
            {
                m_currentLocation = value;
            }
        }

        public int ActualHeight
        {
            get
            {
                return m_actualHeight;
            }
        }

        public Rectangle Bounds
        {
            get
            {
                return m_bounds;
            }
            set
            {
                m_bounds = value;
            }
        }

        public int Width
        {
            get
            {
                return m_bounds.Width;
            }
            set
            {
                m_bounds.Width = value;
            }
        }

        public int Height
        {
            get
            {
                return m_bounds.Height;
            }
            set
            {
                m_bounds.Height = value;
            }
        }

        internal PdfPageResources Resources
        {
            get
            {
                return m_resources;
            }
        }

        internal PdfRecordCollection RecordCollection
        {
            get
            {
                return m_recordCollection;
            }
        }

        internal float[] CropBox
        {
            get
            {
                float[] crop = new float[] { this.Bounds.X, this.Bounds.Y, this.Bounds.Width, this.Bounds.Height };
                PdfDictionary dict = this.m_page.Dictionary;
                if (dict.ContainsKey(DictionaryProperties.CropBox))
                {
                    PdfArray cropbox = dict[DictionaryProperties.CropBox] as PdfArray;
                    for (int i = 0; i < cropbox.Count; i++)
                    {
                        crop[i] = (cropbox[i] as PdfNumber).FloatValue;
                    }
                    return crop;
                }
                return null;
            }
        }
        #endregion

        #region Constructors

        public PdfDocumentPage(PdfPageBase page)
        {
            this.m_page = page;
        }
        #endregion
        private static object s_lock = new object();

        internal void Initialize(PdfPageBase page, bool needParsing)
        {
            lock (s_lock)
            {
#if !DEBUG
            try
            {
#endif
                if (needParsing && m_recordCollection == null)
                {
                    m_resources = PageResourceLoader.Instance.GetPageResources(page);

                    using (MemoryStream stream = new MemoryStream())
                    {
                        page.Layers.CombineContent(stream);
                        stream.Position = 0;

                        ContentParser parser = new ContentParser(stream.ToArray());
                        m_recordCollection = parser.ReadContent();
                    }
                }
                Size clientRectangleSize = new Size((int)m_unitConverter.ConvertToPixels(page.Size.Width, PdfGraphicsUnit.Point),
                    (int)m_unitConverter.ConvertToPixels(page.Size.Height, PdfGraphicsUnit.Point));

                if (page.Dictionary.ContainsKey(DictionaryProperties.CropBox))
                {
                    RectangleF rectbound = new RectangleF(this.Bounds.X, this.Bounds.Y, this.Bounds.Right, this.Bounds.Bottom);
                    float[] crop = this.CropBox;
                    RectangleF rectcrop = new RectangleF(crop[0], crop[1], crop[0] + crop[2], crop[1] + crop[3]);
                    RectangleF rect = m_unitConverter.ConvertToPixels(rectcrop, PdfGraphicsUnit.Point);
                    if (rectcrop != rectbound && rect != rectbound && (rectcrop.X != 0 && rectcrop.Y != 0 && rectcrop.Width != page.Size.Width && rectcrop.Height != page.Size.Height))
                    {
                        this.Bounds = new Rectangle((int)rect.X, (int)rect.Y, (int)rect.Width, (int)rect.Height);
                    }
                    else if (rectcrop != rectbound && rect != rectbound && (rectcrop.X != 0 && rectcrop.Y != 0 && rectcrop.Width == page.Size.Width && rectcrop.Height == page.Size.Height))
                    {
                        this.Bounds = new Rectangle((int)rect.X, (int)rect.Y, (int)rect.Width, (int)rect.Height);
                    }
                    else if (rectcrop != rectbound && rect != rectbound && (rectcrop.X == 0 && rectcrop.Y == 0 && rectcrop.Width != page.Size.Width && rectcrop.Height == page.Size.Height))
                    {
                        this.Bounds = new Rectangle((int)rect.X, (int)rect.Y, (int)rect.Width, (int)rect.Height + (int)rect.Y);
                    }
                    else if (rectcrop != rectbound && rect != rectbound && (rectcrop.X != 0 && rectcrop.Y == 0 && rectcrop.Width == page.Size.Width && rectcrop.Height == page.Size.Height))
                    {
                        this.Bounds = new Rectangle((int)rect.X, (int)rect.Y, (int)rect.Width, (int)rect.Height + (int)rect.Y);
                    }
                    else
                    {
                        Width = clientRectangleSize.Width;
                        Height = clientRectangleSize.Height;
                    }
                }
                else
                {
                    Width = clientRectangleSize.Width;
                    Height = clientRectangleSize.Height;
                }

                m_actualWidth = Width;
                m_actualHeight = Height;
#if !DEBUG
            }
            catch (Exception msg)
            {
                //errorText = msg.StackTrace;
            }
#endif
            }
        }
    }
}

#endif
