#region Copyright Syncfusion Inc. 2001 - 2014
// Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Any infringement will be prosecuted under
// applicable laws. 
#endregion
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Syncfusion.PdfViewer.Base;
using System.Windows;
using System.Windows.Media;
using Syncfusion.Pdf;
using System.IO;
using Syncfusion.Pdf.Graphics;
using System.Windows.Controls;
using System.Globalization;
using System.Threading;
using Syncfusion.Pdf.IO;
using Syncfusion.Pdf.Primitives;

namespace Syncfusion.Windows.PdfViewer
{
    internal class Page : Canvas
    {
        #region Constants
        const int c_ShadowWidth = 4;
        const int c_ShadowHeight = 4;
        #endregion

        #region Static Members
        private static Pen s_borderPen;
        #endregion

        #region Members

        float locationX ;
        float locationY ;
        float width ;
        float height ;
        int c_gapBetweenPages=8;
        internal List<TextSearch> textMatchRectList = new List<TextSearch>();
        internal String targetTextHighlight = String.Empty;
        internal List<PageAnnotation> pageURLList = new List<PageAnnotation>();
        internal Dictionary<System.Drawing.RectangleF, string> txtMatchHighLights = new Dictionary<System.Drawing.RectangleF, string>();
        internal Dictionary<System.Drawing.RectangleF, string> pageAnnotations = new Dictionary<System.Drawing.RectangleF, string>();
        internal Dictionary<Rect, string> m_currentPageDest = new Dictionary<Rect, string>();
        internal List<PageAnnotation> pageAnnotList = new List<PageAnnotation>();
        String uri = String.Empty;
        Dictionary<System.Drawing.RectangleF, string> tempPageAnnotations = new Dictionary<System.Drawing.RectangleF, string>();
        internal Dictionary<Rect, PdfArray> m_PageAnnotDest = new Dictionary<Rect, PdfArray>();
        float annotX, annotY, annotRectHeight, annotWidth, annotHeight;
        internal List<TextMatchRectangle> pageTxtMatchRectList = new List<TextMatchRectangle>();
        internal float annotBorderWidth = 1;
        System.Drawing.RectangleF temprect = new System.Drawing.RectangleF();
        PdfUnitConvertor m_unitConvertor = new PdfUnitConvertor();
        PdfPageResources m_resources;
        PdfRecordCollection m_recordCollection;
        Rect m_bounds;
        internal float m_zoomFactor = 1;
        double m_actualWidth;
        double m_actualHeight;
        PdfPageBase m_page;
        bool m_bInitialized;
        WPFGraphics m_graphics;
        string m_errorText;
        PdfViewerExceptions exception = new PdfViewerExceptions();
        double m_rotation;
        #endregion

        #region Constructors
        static Page()
        {
            s_borderPen = new Pen(
                new SolidColorBrush(Colors.Black), 1);
        }

        public Page(PdfPageBase page)
        {
            m_page = page;
            m_bInitialized = false;
            m_resources = PageResourceLoader.Instance.GetPageResources(page);
            if (m_resources.ContainsKey("Annotations"))
            {
                PdfArray annots = m_resources["Annotations"] as PdfArray;
                if (annots != null)
                {
                    GetCurrentPageAnnotations(annots, page);
                }
            }

            Size clientRectangleSize = new Size((int)m_unitConvertor.ConvertToPixels(m_page.Size.Width, PdfGraphicsUnit.Point),
                (int)m_unitConvertor.ConvertToPixels(m_page.Size.Height, PdfGraphicsUnit.Point));

            Width = clientRectangleSize.Width;
            Height = clientRectangleSize.Height;

            m_actualWidth = m_page.Size.Width;
            m_actualHeight = m_page.Size.Height;

            if (m_page.Rotation == PdfPageRotateAngle.RotateAngle90 || m_page.Rotation == PdfPageRotateAngle.RotateAngle270)
            {
                Width = clientRectangleSize.Height;
                Height = clientRectangleSize.Width;

                m_actualWidth = m_page.Size.Height;
                m_actualHeight = m_page.Size.Width;
            }
        }

        /// <summary>
        /// Parse the annotation properties
        /// </summary>
        private void GetCurrentPageAnnotations(PdfArray annots, PdfPageBase page)
        {
            System.Drawing.RectangleF rect = new System.Drawing.RectangleF();
            PdfArray annotDest = new PdfArray();
            float annotBorder=0;
            pageAnnotations.Clear();
            pageAnnotList.Clear();
            for (int i = 0; i < annots.Count; i++)
            {
                PdfDictionary annotElements = (annots[i] as PdfReferenceHolder).Object as PdfDictionary;

                string subType = (annotElements[DictionaryProperties.Subtype] as PdfName).Value;
                // Only Link or Field parsing is supported. Improves speed.
                if ((subType == DictionaryProperties.Link) || (subType == DictionaryProperties.Widget))
                {
                    if (annotElements.ContainsKey("A"))
                    {
                        PdfDictionary urlDict = annotElements["A"] as PdfDictionary;
                        if (annotElements["A"] is PdfReferenceHolder)
                        {
                            urlDict = (annotElements["A"] as PdfReferenceHolder).Object as PdfDictionary;
                        }
                        if (urlDict.ContainsKey("URI"))
                        {
                            if (urlDict["URI"] is PdfString)
                            {
                                uri = (urlDict["URI"] as PdfString).Value;
                            }
                        }
                    }

                    if (annotElements.ContainsKey("Rect"))
                    {
                        if (annotElements["Rect"] is PdfArray)
                        {
                            PdfArray rec = annotElements["Rect"] as PdfArray;
                            rect = rec.ToRectangle();
                        }
                    }

                    if (annotElements.ContainsKey("Border"))
                    {
                        if (annotElements["Border"] is PdfArray)
                        {
                            PdfArray border = annotElements["Border"] as PdfArray;
                            this.annotBorderWidth = border[2].ObjectCollectionIndex;
                            annotBorder = this.annotBorderWidth;
                        }
                    }
                    if (annotElements.ContainsKey("Dest"))
                    {
                        if (annotElements["Dest"] is PdfArray)
                        {
                            annotDest = annotElements["Dest"] as PdfArray;
                        }
                        else if (annotElements["Dest"] is PdfReferenceHolder)
                        {
                            annotDest = (annotElements["Dest"] as PdfReferenceHolder).Object as PdfArray;
                        }
                    }
                    if (annotElements.ContainsKey("BS"))
                    {
                        PdfDictionary border = annotElements["BS"] as PdfDictionary;

                        if (border == null && annotElements["BS"] is PdfReferenceHolder)
                            border = (annotElements["BS"] as PdfReferenceHolder).Object as PdfDictionary;

                        if (border != null && border.ContainsKey("W") && border["W"] is PdfNumber)
                        {
                            this.annotBorderWidth = (border["W"] as PdfNumber).FloatValue;
                        }
                        annotBorder = this.annotBorderWidth;
                    }

                    if (!pageAnnotations.ContainsKey(rect))
                    {
                        GetAnnotRectProperties(page, rect, uri, m_zoomFactor, annotBorder, annotDest);
                    }
                }
            }
        }

        /// <summary>
        /// Calculates the annotation rectangle according to the zoom values.
        /// </summary>
        private void GetAnnotRectProperties(PdfPageBase page, System.Drawing.RectangleF annotRect, string uri, float zoomFactor, float border, PdfArray pageAnnotDestinations)
        {
            float actualHeight = (float)page.Size.Height;
            annotX = (annotRect.X * zoomFactor);
            annotRectHeight = annotRect.Height;
            annotY = actualHeight - (annotRect.Y * zoomFactor) - (annotRectHeight * zoomFactor);
            annotWidth = annotRect.Width * zoomFactor;
            annotHeight = annotRect.Height * zoomFactor;
            temprect = new System.Drawing.RectangleF(annotX, annotY, annotWidth, annotHeight);
            PageAnnotation annotObject = new PageAnnotation(temprect, uri, border, pageAnnotDestinations);
            pageAnnotList.Add(annotObject);
            if(!pageAnnotations.ContainsKey(temprect))
                pageAnnotations.Add(temprect, uri);
        }

        #endregion

        #region Properties
        public double Rotation
        {
            get
            {
                if (m_page != null)
                    if (m_page.Rotation == PdfPageRotateAngle.RotateAngle90)
                        m_rotation = 90;
                    else if (m_page.Rotation == PdfPageRotateAngle.RotateAngle180)
                        m_rotation = 180;
                    else if (m_page.Rotation == PdfPageRotateAngle.RotateAngle270)
                        m_rotation = 270;
                return m_rotation;
            }
            set
            {
                m_rotation = value;
            }
        }
        public double ActualWidth
        {
            get
            {
                return m_actualWidth;
            }
        }

        public double ActualHeight
        {
            get
            {
                return m_actualHeight;
            }
        }

        public Rect Bounds
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

        public double Width
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

        public double Height
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

        public WPFGraphics Graphics
        {
            get
            {
                if (m_graphics == null)
                {
                    DrawForPrinting();
                }
                return m_graphics;
            }
        }
        #endregion

        #region Implementation
        void Initialize()
        {
            m_bInitialized = true;

            try
            {

                m_resources = PageResourceLoader.Instance.GetPageResources(m_page);

                using (MemoryStream stream = new MemoryStream())
                {
                    m_page.Layers.CombineContent(stream);
                    stream.Position = 0;

                    ContentParser parser = new ContentParser(stream.ToArray());
                    m_recordCollection = parser.ReadContent();
                }
            }
            catch(Exception msg)
            {
                exception.Exceptions.Append("Error occured while loading the pages of the PDF document \r\nThe complete stack trace is as follows\r\n" + msg.StackTrace.ToString() + "\r\n");
            }
            if (m_errorText != null)
            {
                PrintErrorMessage();
            }
        }

        void DrawPageBorder(DrawingContext dc)
        {

            if (Width <= 0 || Height <= 0)
                return;

            double widthExcludingShadow = Width;
            double heightExcludingShadow = Height;

            dc.DrawRectangle(Brushes.White, null, new Rect(0, 0, widthExcludingShadow, heightExcludingShadow));

            dc.DrawLine(s_borderPen, new Point(0, 0), new Point(0, heightExcludingShadow));
            dc.DrawLine(s_borderPen, new Point(0, 0), new Point(widthExcludingShadow, 0));
            dc.DrawLine(s_borderPen, new Point(0, heightExcludingShadow), new Point(widthExcludingShadow, heightExcludingShadow));
            dc.DrawLine(s_borderPen, new Point(widthExcludingShadow, heightExcludingShadow), new Point(widthExcludingShadow, 0));
        }

		public void DrawForPrinting()
        {
            if (!m_bInitialized)
                Initialize();
            
            Thread.CurrentThread.CurrentCulture = CultureInfo.InvariantCulture;

            m_graphics = new WPFGraphics();
            m_graphics.CreateGraphics(Width, Height);

            double sx = Width / m_actualWidth;
            double sy = Height / m_actualHeight;

            if (m_page.Rotation == PdfPageRotateAngle.RotateAngle90)
            {
                m_graphics.PushRotateTransform(90);
                m_graphics.PushTranslateTransform(0, -Height, true);
            }
            else if (m_page.Rotation == PdfPageRotateAngle.RotateAngle270)
            {
                m_graphics.PushTranslateTransform(-(Height - Width), Height, true);
                m_graphics.PushRotateTransform(-90);
            }

            m_graphics.PushScaleTransform(sx, sy);

            WPFRenderer renderer = new WPFRenderer(m_recordCollection, m_resources, m_graphics,
               new Rect(0, 0, Width, Height), true);
            renderer.Render();

            m_graphics.PopTransform();

            m_graphics.FlushDrawing();
        }

        /// <summary>
        /// Serch the text in the document
        /// </summary>
        /// <param name="text">Return the matching texts</param>
        public bool SearchText(string searchText, out List<TextSearch> text)
        {
            text = new List<TextSearch>();
            bool IsMatchFound = false;
            if (!m_bInitialized)
                Initialize();

            m_graphics = new WPFGraphics();
            m_graphics.CreateGraphics(Width, Height);

            double sx = Width / m_actualWidth;
            double sy = Height / m_actualHeight;
            m_graphics.PushScaleTransform(sx, sy);
            WPFRenderer renderer = new WPFRenderer(m_recordCollection, m_resources, m_graphics,
               new Rect(0, 0, Width, Height), true);
            WPFRenderer.txtDictonary.Clear();
            renderer.Render();

            Dictionary<System.Drawing.PointF, TextSearch> mergeTextHelperDict = new Dictionary<System.Drawing.PointF, TextSearch>();
            foreach (TextSearch txt in WPFRenderer.txtDictonary)
            {
                if (!mergeTextHelperDict.ContainsKey(txt.CurrentLocation))
                {
                    mergeTextHelperDict.Add(txt.CurrentLocation, txt);
                }
            }
            MergeTextWithYaxis(mergeTextHelperDict, searchText);
            foreach (TextSearch txt in mergeTextHelperDict.Values)
            {
                if (((txt.Text.IndexOf(searchText, StringComparison.InvariantCultureIgnoreCase)) >= 0))
                {
                    text.Add(txt);
                    IsMatchFound = true;
                }
            }
            m_graphics.PopTransform();
            m_graphics.FlushDrawing();
            return IsMatchFound;
        }
        /// <summary>
        /// Merge the texts with same Y axis
        /// </summary>
        /// <param name="mergeTextHelperDict">Dictonary that holds the text values to be sorted</param>
        private void MergeTextWithYaxis(Dictionary<System.Drawing.PointF, TextSearch> mergeTextHelperDict, string searchText)
        {
            List<KeyValuePair<System.Drawing.PointF, TextSearch>> tempList = new List<KeyValuePair<System.Drawing.PointF, TextSearch>>(mergeTextHelperDict);
            try
            {
                foreach (KeyValuePair<System.Drawing.PointF, TextSearch> dictEntry in tempList)
                {
                    foreach (KeyValuePair<System.Drawing.PointF, TextSearch> nextDictEntry in tempList)
                    {
                        if (dictEntry.Key.Y == nextDictEntry.Key.Y && mergeTextHelperDict.ContainsKey(dictEntry.Key) && mergeTextHelperDict.ContainsKey(nextDictEntry.Key) && dictEntry.Value != nextDictEntry.Value)
                        {
                            string combine = dictEntry.Value.Text + nextDictEntry.Value.Text;
                            if (dictEntry.Key.Y == nextDictEntry.Key.Y && !dictEntry.Value.Text.Contains(searchText) && !nextDictEntry.Value.Text.Contains(searchText) && combine.Contains(searchText))
                            {
                                mergeTextHelperDict.Remove(dictEntry.Key);
                                mergeTextHelperDict.Remove(nextDictEntry.Key);
                                FormattedText newFtext = new FormattedText(dictEntry.Value.Text + nextDictEntry.Value.Text, CultureInfo.InvariantCulture, FlowDirection.LeftToRight, dictEntry.Value.TFace, dictEntry.Value.FontSize, Brushes.Black);
                                TextSearch newText = new TextSearch(newFtext.Text, dictEntry.Key, (float)newFtext.WidthIncludingTrailingWhitespace, dictEntry.Value.FontSize, dictEntry.Value.ScalingFactor, dictEntry.Value.TextFont, newFtext, dictEntry.Value.TFace);
                                mergeTextHelperDict.Add(dictEntry.Key, newText);
                            }
                        }
                    }
                }

            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        /// <summary>
        /// Renders the pdf pages in documentview
        /// </summary>
        public void Draw(DrawingContext dc,List<Page> pages,int currentPage)
        {
            try
            {
                CultureInfo current = Thread.CurrentThread.CurrentCulture;
                Thread.CurrentThread.CurrentCulture = CultureInfo.InvariantCulture;

                if (!m_bInitialized)
                    Initialize();
                #region TextSearch Forcing Redraw
                if (pages[currentPage].textMatchRectList.Count > 0)
                {
                    m_graphics = null;
                }
                #endregion
                float rotate = 0;
                if (m_resources != null)
                    if (m_resources.ContainsKey(DictionaryProperties.Rotate))
                    {
                        rotate = (float)m_resources[DictionaryProperties.Rotate];
                    }
                
                if (m_graphics == null)
                {
                    m_graphics = new WPFGraphics();
                    m_graphics.CreateGraphics(Width, Height);
                    DrawPageBorder(m_graphics.Context);

                    if (rotate == 90)
                    {
                        m_graphics.PushRotateTransform((double)rotate);
                        m_graphics.PushTranslateTransform(0, -Height, true);
                    }
                    else if (rotate == 180)
                    {
                        m_graphics.PushRotateTransform((double)rotate);
                        m_graphics.PushTranslateTransform(-m_graphics.Width, -m_graphics.Height, true);
                    }
                    else if (rotate == 270)
                    {
                        m_graphics.PushRotateTransform((double)rotate);
                        m_graphics.PushTranslateTransform(-Height, m_graphics.Width - m_graphics.Height, true);
                    }
                    double sx = Width / m_actualWidth;
                    double sy = Height / m_actualHeight;

                    m_graphics.PushScaleTransform(sx, sy);
                    #region Annotations
                    foreach (PageAnnotation annots in pageAnnotList)
                    {
                        Rect temprect = new Rect(annots.Rect.X, annots.Rect.Y, annots.Rect.Width, annots.Rect.Height);
                        m_graphics.DrawRectangle(new Pen(Brushes.Black, (int)annots.Border), temprect);
                        PdfUnitConvertor m_unitConvertor=new PdfUnitConvertor();
                        float currentPageYLocation=m_unitConvertor.ConvertFromPixels( (float)pages[currentPage].m_bounds.Y,PdfGraphicsUnit.Point);
                        Rect destRect = new Rect(annotX * m_zoomFactor, (annotY + currentPageYLocation) * m_zoomFactor, annotWidth * m_zoomFactor, annotHeight*m_zoomFactor);
                        if (!m_currentPageDest.ContainsKey(destRect))
                        {
                            m_currentPageDest.Add(destRect, annots.URI);
                            
                        }
                        float pageGapY=m_unitConvertor.ConvertFromPixels(c_gapBetweenPages, PdfGraphicsUnit.Point);
                        temprect = new Rect(annots.Rect.X, annots.Rect.Y + pageGapY, annots.Rect.Width, annots.Rect.Height);
                        if (!m_PageAnnotDest.ContainsKey(temprect))
                        {
                            m_PageAnnotDest.Add(temprect, annots.PageAnnotDestinations);
                        }
                    }
                    #endregion

                    WPFRenderer renderer = new WPFRenderer(m_recordCollection, m_resources, m_graphics,
                       new Rect(0, 0, Width, Height), true);
                    renderer.Render();
                    #region URLRecoganization

                    if (renderer.URLDictonary.Count > 0)
                    {
                        foreach (PageURL url in renderer.URLDictonary)
                        {
                            float scaleX = -1, scaleY = -1;
                            System.Drawing.PointF transformLocation = url.CurrentLocation;

                            if (url.ScalingFactor.X != 0)
                            {
                                scaleX = url.ScalingFactor.X;
                            }
                            if (url.ScalingFactor.Y != 0)
                            {
                                scaleY = url.ScalingFactor.Y;
                            }

                            locationX = transformLocation.X;
                            locationY = transformLocation.Y;

                            if (scaleX > 0)
                            {
                                width = url.TextElementWidth * scaleX;
                            }
                            else
                            {
                                width = url.TextElementWidth;
                            }

                            if (scaleY > 0)
                            {
                                height = url.FontSize * scaleX;
                            }
                            else
                            {
                                height = url.FontSize;
                            }

                            this.pageURLList.Clear();

                            PageAnnotation tempAnnots = new PageAnnotation(new System.Drawing.RectangleF(transformLocation.X, transformLocation.Y, width, height), url.URI, 1);
                            pageAnnotList.Add(tempAnnots);
                            temprect = new System.Drawing.RectangleF(locationX, locationY, width, height);
                            if (!pageAnnotations.ContainsKey(temprect))
                            {
                                pageAnnotations.Add(temprect, url.URI);                                
                            }
                        }
                    }                    
                    #endregion

                    m_graphics.PopTransform();
                    #region TextSearch

                    if (pages[currentPage].textMatchRectList.Count > 0)
                    {
                        txtMatchHighLights.Clear();
                        foreach (TextSearch url in pages[currentPage].textMatchRectList)
                        {
                            float scaleX = -1, scaleY = -1;
                            System.Drawing.PointF transformLocation = url.CurrentLocation;

                            if (url.ScalingFactor.X != 0)
                            {
                                scaleX = url.ScalingFactor.X;
                            }
                            if (url.ScalingFactor.Y != 0)
                            {
                                scaleY = url.ScalingFactor.Y;
                            }
                            locationX = transformLocation.X;
                            locationY = transformLocation.Y;
                            if (scaleX > 0)
                            {
                                width = url.TextElementWidth * scaleX;
                            }
                            else
                            {
                                width = url.TextElementWidth;
                            }
                            if (scaleY > 0)
                            {
                                height = url.FontSize * scaleX;
                            }
                            else
                            {
                                height = url.FontSize;
                            }
                            this.pageURLList.Clear();
                            TextMatchRectangle tempAnnots = new TextMatchRectangle(new System.Drawing.RectangleF(transformLocation.X, transformLocation.Y, width, height), url.Text);
                            pageTxtMatchRectList.Add(tempAnnots);
                            temprect = new System.Drawing.RectangleF(locationX, locationY, width, height);
                            if (!txtMatchHighLights.ContainsKey(temprect))
                            {
                                int startCharLoc = url.Text.IndexOf(targetTextHighlight, StringComparison.InvariantCultureIgnoreCase);
                                if (startCharLoc >= 0)
                                {
                                    String targetString = url.Text.Substring(startCharLoc, targetTextHighlight.Length);
                                    String previousString = url.Text.Substring(0, startCharLoc);

                                    FormattedText ftTargetString = new FormattedText(targetString, CultureInfo.InvariantCulture, FlowDirection.LeftToRight, url.TFace, url.TextFont.Size, Brushes.Black);
                                    FormattedText ftPrevString = new FormattedText(previousString, CultureInfo.InvariantCulture, FlowDirection.LeftToRight, url.TFace, url.TextFont.Size, Brushes.Black);
                                    if (scaleX > 0)
                                    {
                                        temprect = new System.Drawing.RectangleF((float)(locationX + (ftPrevString.WidthIncludingTrailingWhitespace * scaleX)), locationY, (float)ftTargetString.Width * scaleX, height);
                                    }
                                    else
                                    {
                                        temprect = new System.Drawing.RectangleF((float)(locationX + ftPrevString.WidthIncludingTrailingWhitespace), locationY, (float)ftTargetString.Width, height);
                                    }
                                    txtMatchHighLights.Add(temprect, url.Text);
                                }
                            }
                        }
                    }
                    #endregion
                    foreach (KeyValuePair<System.Drawing.RectangleF, string> item in txtMatchHighLights)
                    {
                        SolidColorBrush florocentBrush = new SolidColorBrush();
                        florocentBrush.Color = Colors.Yellow;
                        florocentBrush.Opacity = 0.3;
                        m_graphics.FillRectangle(florocentBrush, new Rect(item.Key.X, item.Key.Y, item.Key.Width, item.Key.Height));
                    }
                    foreach (KeyValuePair<System.Drawing.RectangleF, string> item in pageAnnotations)
                    {
                        m_graphics.DrawRectangle(new Pen(Brushes.Transparent, 1), new Rect(item.Key.X, item.Key.Y, item.Key.Width, item.Key.Height));
                    }
                    m_graphics.FlushDrawing();
                }

                //DrawPageInternal(dc);
                m_currentPageDest.Clear();
                foreach (PageAnnotation annots in pageAnnotList)
                {
                    Rect temprect = new Rect(annots.Rect.X, annots.Rect.Y, annots.Rect.Width, annots.Rect.Height);                    
                    float currentPageYLocation = m_unitConvertor.ConvertFromPixels((float)pages[currentPage].m_bounds.Y, PdfGraphicsUnit.Point);
                    Rect destRect = new Rect(annots.Rect.X * m_zoomFactor, (annots.Rect.Y + currentPageYLocation) * m_zoomFactor, annots.Rect.Width * m_zoomFactor, annots.Rect.Height * m_zoomFactor);
                    if (!m_currentPageDest.ContainsKey(destRect))
                    {
                        m_currentPageDest.Add(destRect, annots.URI);
                    }
                }
                Thread.CurrentThread.CurrentCulture = current;
            }
            catch(Exception msg)
            {
                exception.Exceptions.Append("Error occured while rendering the content of the page \r\nThe complete stack trace is as follows\r\n" + msg.StackTrace.ToString() + "\r\n");
            }
            if (m_errorText != null)
            {
                PrintErrorMessage();
            }
        }

        void DrawPageInternal(DrawingContext dc)
        {
            DrawingGroup dg = m_graphics.Visual.Drawing;
            dg.Transform = new TranslateTransform(Bounds.X, Bounds.Y);
            dc.DrawDrawing(dg);
        }

        void PrintErrorMessage()
        {
            if (!m_bInitialized)
                Initialize();

            m_graphics = new WPFGraphics();
            m_graphics.CreateGraphics(Width, Height);

            double sx = Width / m_actualWidth;
            double sy = Height / m_actualHeight;

            m_graphics.PushScaleTransform(sx, sy);

            //m_graphics.BringIntoView
            m_graphics.RenderTransform = new TranslateTransform(0, 0);

            m_graphics.PopTransform();

            m_graphics.FlushDrawing();

            
            m_errorText = null;
        }
        #endregion
    }
}
