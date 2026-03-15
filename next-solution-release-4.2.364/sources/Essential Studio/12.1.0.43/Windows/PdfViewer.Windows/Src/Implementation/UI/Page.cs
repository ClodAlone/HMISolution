#region Copyright Syncfusion Inc. 2001 - 2014
// Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Any infringement will be prosecuted under
// applicable laws. 
#endregion
using System;
using System.Collections.Generic;
using System.Text;
using System.Drawing;
using Syncfusion.PdfViewer.Base;
using System.IO;
using Syncfusion.Pdf.Graphics;
using Syncfusion.Pdf;
using System.Drawing.Drawing2D;
using System.Globalization;
using System.Threading;
using Syncfusion.Pdf.Primitives;
using Syncfusion.Pdf.IO;

namespace Syncfusion.Windows.Forms.PdfViewer
{
    internal class Page
    {
        #region Constants
        const int c_ShadowWidth = 4;
        const int c_ShadowHeight = 4;
        #endregion

        #region Static Members
        private static Pen s_borderPen;
        #endregion

        #region Members
        internal List<RectangleF> matchTextPositions = new List<RectangleF>();
        internal int pageId = -1;
        private String m_webLink = String.Empty;
        private String m_annotType = String.Empty;
        private float m_annotX, m_annotY, m_annotRectHeight, m_annotWidth, m_annotHeight;
        private float m_annotBorderWidth = 0;
        private RectangleF m_tempRect = new RectangleF();
        internal Dictionary<RectangleF, string> pageAnnotations = new Dictionary<RectangleF, string>();
        internal List<PageAnnotation> pageAnnotList = new List<PageAnnotation>();
        internal Dictionary<RectangleF, string> pageURLs = new Dictionary<RectangleF, string>();
        internal List<PageAnnotation> pageURLList = new List<PageAnnotation>();
        internal Dictionary<RectangleF, string> pageTextsDict = new Dictionary<RectangleF, string>();
        internal List<TextMatchRectangle> pagetxtList = new List<TextMatchRectangle>();
        PdfUnitConvertor m_unitConverter = new PdfUnitConvertor();
        PdfPageResources m_resources;
        PdfRecordCollection m_recordCollection;
        Rectangle m_bounds;
        Bitmap m_pageImage;
        Graphics m_graphics;
        float m_zoomFactor = 1;
        int m_actualWidth;
        int m_actualHeight;
        int m_currentLocation;
        PdfPageBase m_page;
        string errorText;
        DeviceCMYK m_cmyk = new DeviceCMYK();
        #endregion

        #region Constructors
        static Page()
        {
            s_borderPen = new Pen(Color.Black);
        }

        public Page(PdfPageBase page)
        {
            this.m_page = page;
        }
        #endregion

        #region Properties
        public int ActualWidth
        {
            get
            {
                return m_actualWidth;
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

        public int CurrentLeftLocation;

        public int ActualHeight
        {
            get
            {
                return m_actualHeight;
            }
        }
        public Graphics Graphics
        {
            get
            {
                //if (m_graphics == null)
                //{
                    m_pageImage = new Bitmap(Width, Height);
                    m_graphics = Graphics.FromImage(m_pageImage);
                //}

                return m_graphics;
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

        #region Implementation
        internal void Initialize(PdfPageBase page, bool needParsing)
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
                    RectangleF rectbound = this.Bounds;
                    float[] crop = this.CropBox;
                    RectangleF rectcrop = new RectangleF(crop[0], crop[1], crop[2], crop[3]);
                    RectangleF rect = m_unitConverter.ConvertToPixels(rectcrop, PdfGraphicsUnit.Point);
                    if (rectcrop != rectbound && rect != rectbound && (rectcrop.X != 0 && rectcrop.Y != 0 && rectcrop.Width != page.Size.Width && rectcrop.Height != page.Size.Height))
                    {
                        this.Bounds = new Rectangle((int)rect.X, (int)rect.Y, (int)rect.Width - (int)rect.X, (int)rect.Height - (int)rect.Y);
                    }
                    else if (rectcrop != rectbound && rect != rectbound && (rectcrop.X != 0 && rectcrop.Y != 0 && rectcrop.Width == page.Size.Width && rectcrop.Height == page.Size.Height))
                    {
                        this.Bounds = new Rectangle((int)rect.X, (int)rect.Y, (int)rect.Width - (int)rect.X, (int)rect.Height - (int)rect.Y);
                    }
                    else if (rectcrop != rectbound && rect != rectbound && (rectcrop.X == 0 && rectcrop.Y == 0 && rectcrop.Width != page.Size.Width && rectcrop.Height == page.Size.Height))
                    {
                        this.Bounds = new Rectangle((int)rect.X, (int)rect.Y, (int)rect.Width - (int)rect.X, (int)rect.Height);
                    }
                    else if (rectcrop != rectbound && rect != rectbound && (rectcrop.X != 0 && rectcrop.Y == 0 && rectcrop.Width == page.Size.Width && rectcrop.Height == page.Size.Height))
                    {
                        this.Bounds = new Rectangle((int)rect.X, (int)rect.Y, (int)rect.Width - (int)rect.X, (int)rect.Height);
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
                errorText = msg.StackTrace;
            }
#endif
        }


        internal void Initialize(PdfPageBase page, bool needParsing,float zoomFactor)
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

            if (m_resources.ContainsKey("Annotations"))
            {
                PdfArray annots = m_resources["Annotations"] as PdfArray;
                if (annots != null)
                {

                    GetCurrentPageAnnotations(annots, page,zoomFactor);
                }
            }
            
            Size clientRectangleSize = new Size((int)m_unitConverter.ConvertToPixels(page.Size.Width, PdfGraphicsUnit.Point),
                (int)m_unitConverter.ConvertToPixels(page.Size.Height, PdfGraphicsUnit.Point));

            Width = clientRectangleSize.Width;
            Height = clientRectangleSize.Height;

            m_actualWidth = Width;
            m_actualHeight = Height;
#if !DEBUG
            }
            catch (Exception msg)
            {
                errorText = msg.StackTrace;
            }
#endif
        }
        /// <summary>
        /// Parse the annotation properties within a page
        /// </summary>
        /// <param name="annots">Annotation properties</param>
        /// <param name="page">The specific page in which annotations needs to be added</param>
        /// <param name="zoomFactor">Zoom factor need to be considered while drawing the annotation rectangle</param>
        private void GetCurrentPageAnnotations(PdfArray annots,PdfPageBase page,float zoomFactor)
        {
            RectangleF rect = new RectangleF();
            PdfArray annotDest = new PdfArray();
            pageAnnotations.Clear();
            pageAnnotList.Clear();
            for (int i = 0; i < annots.Count; i++)
            {
                PdfDictionary annotElements = (annots[i] as PdfReferenceHolder).Object as PdfDictionary;

                if (annotElements.ContainsKey("Subtype"))
                {
                    PdfName annotType = annotElements["Subtype"] as PdfName;
                    if (annotType == null)
                    {
                        annotType = (annotElements["Subtype"] as PdfReferenceHolder).Object as PdfName;
                    }
                    m_annotType = annotType.Value;
                }

                if (annotElements.ContainsKey("A"))
                {
                    PdfDictionary urlDict = annotElements["A"] as PdfDictionary;
                    if (urlDict == null)
                    {
                        urlDict = (annotElements["A"] as PdfReferenceHolder).Object as PdfDictionary;
                    }
                    if (urlDict.ContainsKey("URI"))
                    {
                        m_webLink = (urlDict["URI"] as PdfString).Value;
                    }
                }

                if (annotElements.ContainsKey("Rect"))
                {
                    PdfArray rec = annotElements["Rect"] as PdfArray;
                    rect = rec.ToRectangle();
                }

                if (annotElements.ContainsKey("Dest"))
                {
                    annotDest = annotElements["Dest"] as PdfArray;
                    if (annotDest == null)
                    {
                        annotDest = (annotElements["Dest"] as PdfReferenceHolder).Object as PdfArray;
                    }
                    
                }

                if (annotElements.ContainsKey("Border"))
                {
                    PdfArray border = annotElements["Border"] as PdfArray;
                    this.m_annotBorderWidth = border[2].ObjectCollectionIndex;
                    
                }
                if (annotElements.ContainsKey("BS"))
                {
                    PdfDictionary border = annotElements["BS"] as PdfDictionary;
                    if (border.ContainsKey("W"))
                    {
                        this.m_annotBorderWidth = (border["W"] as PdfNumber).FloatValue;
                    }
                    
                }
                if (!pageAnnotations.ContainsKey(rect))
                {
                    GetAnnotRectProperties(page, rect, m_webLink, zoomFactor, this.m_annotBorderWidth,m_annotType,annotDest);
                }
            }
        }

        /// <summary>
        /// Calculates the annotation rectangle according to the zoom values.
        /// </summary>
        /// <param name="page">The specific page in which annotations needs to be added</param>
        /// <param name="annotRect">Represents the annotation rectangle</param>
        /// <param name="uri"></param>
        /// <param name="zoomFactor"></param>
        /// <param name="border"></param>
        /// <param name="annotType"></param>
        /// <param name="pageAnnotDestinations"></param>
        private void GetAnnotRectProperties(PdfPageBase page, RectangleF annotRect,string uri,float zoomFactor,float border,string annotType,PdfArray pageAnnotDestinations)
        {
            //m_zoomFactor = zoomFactor;
            float actualHeight = m_unitConverter.ConvertFromPixels(m_actualHeight * zoomFactor, PdfGraphicsUnit.Point);
            m_annotX = (annotRect.X * zoomFactor);
            m_annotRectHeight = annotRect.Height;
            m_annotY = actualHeight - (annotRect.Y * zoomFactor) - (m_annotRectHeight * zoomFactor);
            m_annotWidth = annotRect.Width * zoomFactor;
            m_annotHeight = annotRect.Height * zoomFactor;
            m_tempRect = new RectangleF(m_annotX, m_annotY, m_annotWidth, m_annotHeight);
            PageAnnotation annotObject = new PageAnnotation(m_tempRect, uri, border,annotType,pageAnnotDestinations);
            pageAnnotList.Add(annotObject);
            if (!pageAnnotations.ContainsKey(m_tempRect))
            {
                pageAnnotations.Add(m_tempRect, uri);
            }
        }

        /// <summary>
        /// Include the URL annotation rectangles with the specific page properties
        /// </summary>
        /// <param name="page">The specific page in which annotations needs to be added</param>
        /// <param name="annotProperties">Represents the annotation properties such as rectangle,location and URI</param>
        internal void GetURLProperties(Page page, PageAnnotation annotProperties)
        {
            pageURLList.Add(annotProperties);
            if (!pageURLs.ContainsKey(annotProperties.Rect))
            {
                pageURLs.Add(annotProperties.Rect, annotProperties.URI);
            }
        }

        /// <summary>
        /// Include the text rectangles with the specific page properties
        /// </summary>
        /// <param name="page">The specific page in which text search needs to be performed</param>
        /// <param name="annotProperties">Represents the annotation properties such as rectangle,location and URI</param>
        internal void GetTextProperties(Page page, TextMatchRectangle annotProperties)
        {
            pagetxtList.Add(annotProperties);
            if (!pageTextsDict.ContainsKey(annotProperties.Rect))
            {
                pageTextsDict.Add(annotProperties.Rect, annotProperties.Text);
            }
        }

         /// <summary>
        /// Calculates the annotation rectangle according to the zoom values.
        /// </summary>
        /// <param name="page">The specific page in which annotations needs to be added</param>
        /// <param name="annotRect">Represents the annotation rectangle</param>
        /// <param name="zoomFactor">Zoom factor need to be considered</param>
        internal RectangleF GetTextRectProperties(Page page, RectangleF annotRect, float zoomFactor, bool isDrawingPanel)
        {
            float actualHeight = m_unitConverter.ConvertFromPixels(m_actualHeight * zoomFactor, PdfGraphicsUnit.Point);
            m_annotX = (annotRect.X * zoomFactor);// +page.CurrentLeftLocation;
            m_annotRectHeight = annotRect.Height;
            m_annotWidth = annotRect.Width * zoomFactor;
            m_annotHeight = annotRect.Height * zoomFactor;
            if (!isDrawingPanel)
            {
                m_annotY = actualHeight + (annotRect.Y * zoomFactor);
            }
            else
            {
                m_annotX += page.CurrentLeftLocation;
                m_annotY = actualHeight + (annotRect.Y * zoomFactor) + page.CurrentLocation;
            }
            m_tempRect = new RectangleF(m_annotX, m_annotY, m_annotWidth, m_annotHeight);
            return m_tempRect;
        }

        public void Draw(Graphics g, bool printing)
        {
#if !DEBUG
            try
            {                
#endif
            CultureInfo current = Thread.CurrentThread.CurrentCulture;
            Thread.CurrentThread.CurrentCulture = CultureInfo.InvariantCulture;
            ImageRenderer renderer = null;
                if (printing)
                {

                    g.TranslateTransform(1, 1);
                    //g.SetClip(g.VisibleClipBounds);
                    //renderer = new ImageRenderer(m_recordCollection, m_resources, g, true);
                    //renderer.RenderAsImage();


                }
                //else
                //{
                GraphicsState gs = g.Save();
                g.TranslateTransform(this.Bounds.Left, 0);
                DrawPageBorders(g);

                PdfUnitConvertor converter = new PdfUnitConvertor(g);


                g.SetClip(this.Bounds, CombineMode.Replace);

                if (m_recordCollection == null)
                {
                    this.Initialize(this.m_page, true);
                }
                renderer = new ImageRenderer(m_recordCollection, m_resources, g, true, m_cmyk);
                renderer.RenderAsImage();

                g.Restore(gs);

                Thread.CurrentThread.CurrentCulture = current;
#if !DEBUG
                //}
            }
            catch (Exception msg)
            {
                errorText = msg.StackTrace;
            }

#endif
        }

        void DrawToImage()
        {
            DrawPageBorders(this.Graphics);
            ImageRenderer renderer = new ImageRenderer(m_recordCollection, m_resources, this.Graphics, true, m_cmyk);
            CultureInfo current = Thread.CurrentThread.CurrentCulture;
            Thread.CurrentThread.CurrentCulture = CultureInfo.InvariantCulture;
            renderer.RenderAsImage();
            Thread.CurrentThread.CurrentCulture = current;
        }
        #endregion

        #region Helper Methods
        void DrawPageBorders(Graphics g)
        {
            Rectangle r = this.Bounds;

            //Exclude border pen width;
            r.Width -= ((int)s_borderPen.Width + r.Left);
            r.Height -= ((int)s_borderPen.Width);

            r.Width = (int)(r.Width * m_zoomFactor);

            g.FillRectangle(Brushes.White, r);
            g.DrawRectangle(s_borderPen, r);

        }

        public void SetLeft(int left)
        {
            this.Bounds = new Rectangle(left, this.Bounds.Top, this.Bounds.Width, this.Bounds.Height);
        }

        internal void Clear()
        {
            if (m_graphics != null)
                m_graphics.Dispose();
            matchTextPositions.Clear();
            Resources.Resources.Clear();
            pageAnnotations.Clear();
            pageAnnotList.Clear();
            pageURLs.Clear();
            pageURLList.Clear();
            pageTextsDict.Clear();
            pagetxtList.Clear();
        }
        #endregion

    }
}
