#region Copyright Syncfusion Inc. 2001 - 2014
// Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Any infringement will be prosecuted under
// applicable laws. 
#endregion

#if !SILVERLIGHT && !NETFX_CORE && !WP

using System;
using System.Collections;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Collections.Generic;

using Syncfusion.Pdf.Graphics;
using Syncfusion.Pdf.Parsing;
using Syncfusion.Pdf.Interactive;

namespace Syncfusion.Pdf.HtmlToPdf
{
    /// <summary>
    /// Represents the result of html to pdf conversion.
    /// </summary>
    public class HtmlToPdfResult : IDisposable
    {
#region Fields
        /// <summary>
        /// Holds the list of Page-Breaks.
        /// </summary>
        private ArrayList m_pageBreakCollection;

        /// <summary>
        /// Holds the list of hyperlinks.
        /// </summary>
        private ArrayList m_anchorsCollection;
        /// <summary>
        /// Holds the list of document links.
        /// </summary>
        private ArrayList m_documentLinkCollection;

        /// <summary>
        /// Holds the resultant images.
        /// </summary>
        private Image[] m_images;

        private PointF m_location;

        private float m_metafileTransparency = 0f;

        /// Holds the Quality of image.
        /// </summary>
        private long m_quality = 100;

        private Stream m_docStream;

        /// <summary>
        /// Internal variable to hold if conversion is completed.
        /// </summary>
        private bool m_Completed = true;

        /// <summary>
        /// Internal variable to store the scroll position.
        /// </summary>
        private float m_height;

        /// <summary>
        /// Internal variable to store the height yet to be converted.
        /// </summary>
        private float m_remHeight;

        /// <summary>
        /// Internal variable to store layout result of HTML to PDF.
        /// </summary>
        private PdfLayoutResult m_layoutResult;
        private bool m_isImagePath;
        private const int m_splitOffset = 4;

        private float m_scrollBarWidth;
        private float m_scrollBarHeight;
        #endregion

#region Constructor
        /// <summary>
        /// Initializes a new instance of the <see cref="HtmlToPdfResult"/> class.
        /// </summary>
        /// <param name="image">The image.</param>
        /// <param name="pageBreaks">The page breaks.</param>
        /// <param name="anchors">The anchors.</param>
        /// <param name="anchors">The document links.</param>
        public HtmlToPdfResult(Image[] image, ArrayList pageBreaks, ArrayList anchors, ArrayList documentLinks)
        {
            m_images = image;
            m_anchorsCollection = anchors;
            m_pageBreakCollection = pageBreaks;
            m_documentLinkCollection = documentLinks;
        }

        public HtmlToPdfResult(Stream docStream)
        {
            m_docStream = docStream;
        }

        /// <summary>
        /// Initializes new instance of the <see cref="HtmlToPdfResult"/> class.
        /// </summary>
        /// <param name="image"></param>
        /// <param name="pageBreaks"></param>
        /// <param name="anchors"></param>
        /// <param name="document links"></param>
        /// <param name="remHeight"></param>
        internal HtmlToPdfResult(Image[] image, ArrayList pageBreaks, ArrayList anchors, ArrayList documentLinks, float remHeight)
            : this(image, pageBreaks, anchors, documentLinks)
        {
            m_remHeight = remHeight;
        }
        #endregion

#region Properties
        internal bool IsImagePath
        {
            get
            {
                return m_isImagePath;
            }
            set
            {
                m_isImagePath = value;
            }
        }
        /// <summary>
        /// Gets the Page-Break Collection.
        /// </summary>
        internal ArrayList PageBreakCollection
        {
            get
            {
                return m_pageBreakCollection;
            }
        }
        
        /// <summary>
        /// Gets the hyper-links Collection.
        /// </summary>
        internal ArrayList AnchorsCollection
        {
            get
            {
                return m_anchorsCollection;
            }
        }
        
        /// <summary>
        /// Gets if conversion is complete.
        /// </summary>
        internal bool Completed
        {
            get
            {
                return m_Completed;
            }
        }

        /// <summary>
        /// Gets the next scroll height.
        /// </summary>
        internal float Height
        {
            get
            {
                return m_height;
            }
        }

        /// <summary>
        /// Gets the rendered image.
        /// </summary>
        /// <value>The rendered image.</value>
        public Image RenderedImage
        {
            get
            {
                return m_images[0];
            }
        }

        /// <summary>
        /// Returns the image array after the conversion.
        /// </summary>
        /// <value>The images.</value>
        public Image[] Images
        {
            get
            {
                return m_images;
            }
        }

        /// <summary>
        /// Handles the quality of Bitmap images in HTML.
        /// </summary>
        public long Quality
        {
            set
            {
                m_quality = value;
            }
        }

        public PointF Location
        {
            get
            {
                return m_location;
            }
            set
            {
                m_location = value;
            }
        }

        public float MetafileTransparency
        {
            get
            {
                return m_metafileTransparency;
            }

            set
            {
                if (value > 0 && value <= 1)
                    m_metafileTransparency = value;
                else
                    throw new PdfException("Value can only be greater than 0 and less than or equal to 1");
            }

        }

        internal PdfLayoutResult LayoutResult
        {
            get
            {
                return m_layoutResult;
            }
        }


        internal float ScrollBarWidth
        {
            get
            {
                return m_scrollBarWidth;
            }
            set
            {
                m_scrollBarWidth = value;
            }
        }

        internal float ScrollBarHeight
        {
            get
            {
                return m_scrollBarHeight;
            }
            set
            {
                m_scrollBarHeight = value;
            }
        }
        #endregion

#region Methods
        public void Render(PdfDocument document)
        {
            PdfLoadedDocument loadedDocument = new PdfLoadedDocument(m_docStream);
            document.ImportPageRange(loadedDocument, 0, loadedDocument.Pages.Count - 1);
            document.Pages.Remove(document.Pages[0]);
        }
        /// <summary>
        /// Draws the HtmlToPdfResults on to the document.
        /// </summary>
        /// <param name="page">The Pdf Page.</param>
        /// <param name="format">The Metafile layout format.</param>
        public void Render(PdfPageBase page, PdfLayoutFormat format)
        {
            if (page == null)
            {
                throw new PdfException("Page cannot be null.");
            }

            if (m_images == null)
            {
                throw new PdfException("Image cannot be null.");
            }

            format = (format == null) ? new PdfLayoutFormat() : format;

            if (m_pageBreakCollection != null)
            {
                //if (m_pageBreakCollection.Count > 1)
                //{
                //    ArrayList pageBreakCollection = new ArrayList();
                //    float pageHeight = page.Size.Height;
                   
                //    for (int i = 0; i < m_pageBreakCollection.Count; i++)
                //    {
                //       //m_pageBreakCollection[i] = (float) m_pageBreakCollection[i]*0.75f;
                //    }

                //    for (int i = 0; i < m_pageBreakCollection.Count; i++)
                //    {
                //        if ((float)m_pageBreakCollection[i] <= pageHeight)
                //        {
                //            pageBreakCollection.Add(m_pageBreakCollection[i]);
                //        }
                //        else if ((float)m_pageBreakCollection[i] > pageHeight)
                //        {
                //            pageBreakCollection.Add(pageHeight);
                //            float value = (float)m_pageBreakCollection[i] - pageHeight;
                //            do
                //            {
                //                if (value > pageHeight)
                //                {
                //                    pageBreakCollection.Add(pageHeight);
                //                }
                //                else
                //                {
                //                    pageBreakCollection.Add(value);
                //                }
                                
                //                value -= pageHeight;
                //            } while(value>0);
                //        }
                //    }
                  
                //    m_pageBreakCollection = pageBreakCollection;
                //}
            }

            PdfLayoutResult result = null;

            foreach (Image image in m_images)
            {
                if (result != null && result.Bounds.Size.Height <= page.Size.Height)
                {
                    page = result.Page;

                    result = image is Metafile ?
                        DrawMetaFile((Metafile)image, page, new RectangleF(0, result.Bounds.Size.Height-m_splitOffset, page.Size.Width, 0), format, m_quality) : 
                        DrawBitmap((Bitmap)image, page, new RectangleF(0, result.Bounds.Size.Height-m_splitOffset, page.Size.Width, 0), format);
                }
                else if (image is Metafile && result == null)
                {
                    PdfGraphicsState state = null;

                    // Workaround for Pixel to Point conversion.
                    if (page is PdfPage && (page as PdfPage).Document.FileStructure.TaggedPdf)
                    {
                        state = page.Graphics.Save();
                        page.Graphics.ScaleTransform(.75f, .75f);
                    }

                    result = DrawMetaFile((Metafile)image, page, RectangleF.Empty, format, m_quality);

                    // Workaround for Pixel to Point conversion.
                    if (page is PdfPage && (page as PdfPage).Document.FileStructure.TaggedPdf && state != null)
                    {
                        page.Graphics.ScaleTransform(1f, 1f);
                        page.Graphics.Restore(state);
                    }
                }
                else
                {
                    result = DrawBitmap((Bitmap)image, page, RectangleF.Empty, format);
                }
            }

            // Checks whether the conversion is complete.
            if (page is PdfPage && (page as PdfPage).Section.ParentDocument is PdfDocument && (page as PdfPage).Section.ParentDocument.FileStructure.TaggedPdf)
            {
                if (m_remHeight > 0)
                {
                    m_height = result.Bounds.Height;
                    m_Completed = false;
                }
                else
                    m_Completed = true;

                m_layoutResult = result;
            }

            # region Match DocumentLinkAnnotations

            Dictionary<int, List<PdfUriAnnotation>> repeatedList = new Dictionary<int, List<PdfUriAnnotation>>();

            PdfDocument doc = (page as PdfPage).Document;
            if (doc != null && m_documentLinkCollection != null && m_documentLinkCollection.Count > 0)
            {
                foreach (PdfPage p in doc.Pages)
                {
                    for (int j = 0; j < p.Annotations.Count; j++)
                    {
                        if (p.Annotations[j] is PdfDocumentLinkAnnotation)
                        {
                            PdfDocumentLinkAnnotation annot = p.Annotations[j] as PdfDocumentLinkAnnotation;
                            if (annot.Destination == null)
                            {
                                float pHeight = 0;
                                for (int k = 0; k < doc.Pages.Count; k++)
                                {
                                    if (annot.Destination != null)
                                        break;
                                    PdfPage pdfPage = doc.Pages[k];
                                    for (int i = pdfPage.Annotations.Count - 1; i >= 0; i--)
                                    {
                                        foreach (KeyValuePair<int, List<PdfUriAnnotation>> item in repeatedList)
                                        {
                                            bool found = false;
                                            if (item.Key == i)
                                            {
                                                foreach (PdfUriAnnotation tempAnnot in item.Value)
                                                {
                                                    if (tempAnnot.Text == annot.Text)
                                                    {
                                                        PdfPageBase tempPage = tempAnnot.Page;
                                                        PdfDestination dest = new PdfDestination(tempPage, tempAnnot.Location);
                                                        annot.Destination = dest;
                                                        found = true;
                                                        break;
                                                    }
                                                }
                                                if (found)
                                                    break;
                                            }
                                            else if (found)
                                                break;
                                        }

                                        if (pdfPage.Annotations[i] is PdfUriAnnotation)
                                        {
                                            PdfUriAnnotation uri = (pdfPage.Annotations[i] as PdfUriAnnotation);
                                            if (uri.Text == annot.Text)
                                            {
                                                PointF loc = uri.Location;
                                                if (loc.Y > pHeight)
                                                    loc.Y -= pHeight;

                                                PdfDestination dest = new PdfDestination(pdfPage, loc);
                                                annot.Destination = dest;

                                                if (!repeatedList.ContainsKey(k))
                                                    repeatedList.Add(k, new List<PdfUriAnnotation>());
                                                (repeatedList[k] as List<PdfUriAnnotation>).Add(uri);

                                                pdfPage.Annotations.RemoveAt(i);
                                                break;
                                            }
                                        }
                                    }
                                    pHeight += pdfPage.Graphics.ClientSize.Height;
                                }
                            }
                        }
                    }
                }
            }
            repeatedList.Clear();
            repeatedList = null;

            # endregion
        }

        /// <summary>
        /// Renders the HTMl conversion and returns the layout result
        /// </summary>
        /// <param name="page"></param>
        /// <param name="format"></param>
        /// <param name="result"></param>
        public void Render(PdfPageBase page, PdfLayoutFormat format, out PdfLayoutResult result)
        {
            if (page == null)
            {
                throw new PdfException("Page cannot be null.");
            }

            if (m_images == null)
            {
                throw new PdfException("Image cannot be null.");
            }

            format = (format == null) ? new PdfLayoutFormat() : format;

            if (m_pageBreakCollection != null)
            {
                //if (m_pageBreakCollection.Count > 1)
                //{
                //    ArrayList pageBreakCollection = new ArrayList();
                //    float pageHeight = page.Size.Height;

                //    for (int i = 0; i < m_pageBreakCollection.Count; i++)
                //    {
                //        //m_pageBreakCollection[i] = (float) m_pageBreakCollection[i]*0.75f;
                //    }

                //    for (int i = 0; i < m_pageBreakCollection.Count; i++)
                //    {
                //        if ((float)m_pageBreakCollection[i] <= pageHeight)
                //        {
                //            pageBreakCollection.Add(m_pageBreakCollection[i]);
                //        }
                //        else if ((float)m_pageBreakCollection[i] > pageHeight)
                //        {
                //            pageBreakCollection.Add(pageHeight);
                //            float value = (float)m_pageBreakCollection[i] - pageHeight;
                //            do
                //            {
                //                if (value > pageHeight)
                //                {
                //                    pageBreakCollection.Add(pageHeight);
                //                }
                //                else
                //                {
                //                    pageBreakCollection.Add(value);
                //                }

                //                value -= pageHeight;
                //            } while (value > 0);
                //        }
                //    }

                //    m_pageBreakCollection = pageBreakCollection;
                //}
            }

           
            result = null;
           
            foreach (Image image in m_images)
            {
                if (result != null && result.Bounds.Size.Height <= page.Size.Height)
                {
                    page = result.Page;

                    result = image is Metafile ?
                        DrawMetaFile((Metafile)image, page, new RectangleF(0, result.Bounds.Size.Height, page.Size.Width, 0), format, m_quality) :
                        DrawBitmap((Bitmap)image, page, new RectangleF(0, result.Bounds.Size.Height, page.Size.Width, 0), format);
                }
                else if (result == null && Location != null && image is Metafile)
                {
                    result = DrawMetaFile((Metafile)image, page, new RectangleF(Location.X, Location.Y, page.Size.Width, 0), format, m_quality);
                    if (page == result.Page)
                    {
                        result = new PdfLayoutResult(result.Page, new RectangleF(result.Bounds.X, result.Bounds.Y, result.Bounds.Width, result.Bounds.Height + Location.Y));
                    }
                }               
                else if (image is Metafile && result == null)
                {
                    result = DrawMetaFile((Metafile)image, page, RectangleF.Empty, format, m_quality);
                }
                else
                {
                    result = DrawBitmap((Bitmap)image, page, RectangleF.Empty, format);
                }
            }
        }

        /// <summary>
        /// Draws the meta file.
        /// </summary>
        /// <param name="page">The page.</param>
        /// <param name="format">The format.</param>
        private PdfLayoutResult DrawMetaFile(Metafile metafile, PdfPageBase page, RectangleF bounds, PdfLayoutFormat format, long quality)
        {
            PdfMetafile mf = new PdfMetafile(metafile);
            mf.ScrollBarWidth = ScrollBarWidth;
            mf.ScrollBarHeight = ScrollBarHeight;
            mf.Quality = quality;

            if (m_metafileTransparency > 0)
                mf.SetTransparency(m_metafileTransparency, m_metafileTransparency, PdfBlendMode.Normal, true);

            PdfMetafileLayoutFormat metaFormat = (format is PdfMetafileLayoutFormat) ?
                (format as PdfMetafileLayoutFormat) : new PdfMetafileLayoutFormat();

            float[] pageOffsets = new float[m_pageBreakCollection.Count];

            format = (format == null) ? new PdfMetafileLayoutFormat() : format;

            m_pageBreakCollection.CopyTo(pageOffsets);
            mf.HtmlHyperlinksCollection = m_anchorsCollection;
            mf.DocumentLinksCollection = m_documentLinkCollection;
            mf.IsImagePath = IsImagePath;
            return mf.Draw((PdfPage)page, bounds, pageOffsets, metaFormat);
        }

        /// <summary>
        /// Draws the bitmap.
        /// </summary>
        /// <param name="bitmap">The bitmap.</param>
        /// <param name="page">The page.</param>
        /// <param name="bounds">The bounds.</param>
        /// <param name="format">The format.</param>
        /// <returns></returns>
        private PdfLayoutResult DrawBitmap(Bitmap bitmap, PdfPageBase page, RectangleF bounds, PdfLayoutFormat format)
        {
            PdfBitmap bmp = new PdfBitmap(bitmap);
            bmp.ScrollBarWidth = ScrollBarWidth;
            bmp.ScrollBarHeight = ScrollBarHeight;
            format = (format == null) ? new PdfLayoutFormat() : format;

            return bmp.Draw((PdfPage)page, bounds.Location, format);
        }
        #endregion

#region IDisposable
        /// <summary>
        /// Performs application-defined tasks associated with releasing, or resetting unmanaged resources.
        /// </summary>
        void IDisposable.Dispose()
        {
            if (m_images != null)
            {
                for (int i = 0; i < m_images.Length; i++)
                    m_images[i].Dispose();
            }
            m_images = null;
            m_pageBreakCollection = null;
        }
        #endregion
    }
}
#endif