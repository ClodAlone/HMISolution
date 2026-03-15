#region Copyright Syncfusion Inc. 2001 - 2014
// Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Any infringement will be prosecuted under
// applicable laws. 
#endregion

#if !SILVERLIGHT &&!NETFX_CORE && !WP
using System;
using System.Collections;
using System.Drawing;

using Syncfusion.Pdf.Graphics.Images.Metafiles;
using Syncfusion.Pdf.HtmlToPdf;
using Syncfusion.Pdf.Interactive;

namespace Syncfusion.Pdf.Graphics
{
    /// <summary>
    /// Layouts the metafiles.
    /// </summary>
    internal class MetafileLayouter : ShapeLayouter
    {
        #region Constructors
        /// <summary>
        /// Initializes a new instance of the <see cref="MetafileLayouter"/> class.
        /// </summary>
        /// <param name="element">The element.</param>
        public MetafileLayouter(PdfMetafile element)
            : base(element)
        {
        }
        #endregion

        #region Properties
        /// <summary>
        /// Gets shape element.
        /// </summary>
        /// <value></value>
        new public PdfMetafile Element
        {
            get
            {
                return (base.Element as PdfMetafile);
            }
        }

        /// <summary>
        /// Gets the Text regions manager.
        /// </summary>
        private TextRegionManager TextRegions
        {
            get
            {
                return Element.TextRegions;
            }
        }

        /// <summary>
        /// Gets the Image regions manager.
        /// </summary>
        private ImageRegionManager ImageRegions
        {
            get
            {
                return Element.ImageRegions;
            }
        }
        #endregion

        #region Implementation

        /// <summary>
        /// Repositions the links.
        /// </summary>
        /// <param name="list">The list.</param>
        /// <param name="height">The height.</param>
        internal void RepositionLinks(ArrayList list, float height)
        {
            foreach (HtmlHyperLink hyperlink in list)
            {
                Element.HtmlHyperlinksCollection.Remove(hyperlink);
            }

            list.Clear();
            list = Element.HtmlHyperlinksCollection.Clone() as ArrayList;
            Element.HtmlHyperlinksCollection.Clear();

            foreach (HtmlHyperLink hyperlink in list)
            {
                float y = hyperlink.Bounds.Y - height;
                hyperlink.Bounds = new RectangleF(hyperlink.Bounds.X, y, hyperlink.Bounds.Width, hyperlink.Bounds.Height);
                Element.HtmlHyperlinksCollection.Add(hyperlink);
            }
        }

        /// <summary>
        /// Corrects current bounds on the page.
        /// </summary>
        /// <param name="currentPage">Current page.</param>
        /// <param name="currentBounds">Current lay outing bounds.</param>
        /// <param name="shapeLayoutBounds">The current active shape bounds.</param>
        /// <param name="param">Lay outing parameters.</param>
        /// <returns>Corrected lay outing bounds.</returns>
        protected override RectangleF CheckCorrectCurrentBounds(PdfPage currentPage,
            RectangleF currentBounds, RectangleF shapeLayoutBounds, PdfLayoutParams param)
        {
            if (param == null)
            {
                throw new ArgumentNullException("param");
            }

            RectangleF result = base.CheckCorrectCurrentBounds(currentPage, currentBounds,
                shapeLayoutBounds, param);

            // Calculate correct co-ordinates to prevent text line cutting among the pages.
            PdfLayoutFormat format = param.Format;
            PdfMetafileLayoutFormat formatEx = format as PdfMetafileLayoutFormat;
            bool splitLines = (formatEx != null) ? formatEx.SplitTextLines : true;
            bool splitImages = (formatEx != null) ? formatEx.SplitImages : false;
            bool htmlbreak = (formatEx != null) ? formatEx.IsHTMLPageBreak : false;

            if (!this.IsImagePath)
            {
                if (TextRegions != null && !splitLines && !htmlbreak)
                {
                    float y = shapeLayoutBounds.Y + result.Height;
                    PdfUnitConvertor convertor = new PdfUnitConvertor(Element.VerticalResolution);
                    y = convertor.ConvertToPixels(y, PdfGraphicsUnit.Point);

                    y = TextRegions.GetTopCoordinate(y);

                    if (!(result.Height <= currentPage.GetClientSize().Height))
                    {
                        y -= 2f;
                        y = TextRegions.GetTopCoordinate(y);
                    }
                    y = convertor.ConvertFromPixels(y, PdfGraphicsUnit.Point);
                    float height = 0;
                    if (y > shapeLayoutBounds.Y)
                        height = y - shapeLayoutBounds.Y;

                    result.Height = (currentPage != null && currentPage.GetClientSize().Height < height)
                        ? currentPage.GetClientSize().Height : height;
                    if (result.Y != 0)
                    {
                        float totHeight = result.Y + height;

                        if (totHeight > currentPage.GetClientSize().Height)
                        {
                            float finHeight = currentPage.GetClientSize().Height;
                            float diffHeight = totHeight - finHeight;
                            result.Height = height - diffHeight;

                            y = shapeLayoutBounds.Y + result.Height;

                            y = convertor.ConvertToPixels(y, PdfGraphicsUnit.Point);

                            y = TextRegions.GetTopCoordinate(y);
                            y = convertor.ConvertFromPixels(y, PdfGraphicsUnit.Point);

                            if (y > shapeLayoutBounds.Y)
                                height = y - shapeLayoutBounds.Y;

                            result.Height = (currentPage != null && currentPage.GetClientSize().Height < height)
                                ? currentPage.GetClientSize().Height : height;
                        }
                    }
                }

                if (ImageRegions != null && !splitImages && !htmlbreak)
                {
                    float textHeight = result.Height;

                    float y = shapeLayoutBounds.Y + result.Height;
                    PdfUnitConvertor convertor = new PdfUnitConvertor(Element.VerticalResolution);
                    y = convertor.ConvertToPixels(y, PdfGraphicsUnit.Point);

                    y = ImageRegions.GetTopCoordinate(y);

                    y = convertor.ConvertFromPixels(y, PdfGraphicsUnit.Point);
                    if (!(Math.Round(y) == Math.Round(shapeLayoutBounds.Y + result.Height)))
                        y = (float)Math.Floor(y);
                    float height = 0;
                    if (y > shapeLayoutBounds.Y)
                        height = y - shapeLayoutBounds.Y;
                    if (height == 0 || TextRegions.Count == 0)
                    {
                        result.Height = textHeight;
                    }
                    else
                    {
                        PdfPage page = param.Page;
                        if (shapeLayoutBounds.Height > page.Size.Height)
                            result.Height = height;
                        if (TextRegions != null && !splitLines)
                        {
                            y = shapeLayoutBounds.Y + result.Height;
                            y = convertor.ConvertToPixels(y, PdfGraphicsUnit.Point);

                            y = TextRegions.GetTopCoordinate(y);
                            y = convertor.ConvertFromPixels(y, PdfGraphicsUnit.Point);

                            if (y > shapeLayoutBounds.Y)
                                height = y - shapeLayoutBounds.Y;

                            result.Height = (currentPage != null && currentPage.GetClientSize().Height < height)
                                ? currentPage.GetClientSize().Height : height;
                            if (result.Height == 0)
                            {
                                if (currentPage.GetClientSize().Height > height)
                                {
                                    result.Height = textHeight;
                                }
                            }
                        }
                        else
                            result.Height = height;
                    }
                }
            }
            ArrayList list = new ArrayList();
            foreach (HtmlHyperLink hyperlink in Element.HtmlHyperlinksCollection)
            {
                float pageHeight = result.Height;
                if (pageHeight > hyperlink.Bounds.Y)
                {
                    if (String.IsNullOrEmpty(hyperlink.Hash))
                    {
                        PdfUriAnnotation annot = new PdfUriAnnotation(hyperlink.Bounds, hyperlink.Href);
                        annot.Border.Width = 0;
                        currentPage.Annotations.Add(annot);
                    }
                    else
                    {
                        PdfDocumentLinkAnnotation dAnnot = new PdfDocumentLinkAnnotation(hyperlink.Bounds);
                        dAnnot.Border.Width = 0;
                        dAnnot.ApplyText(hyperlink.Hash);
                        currentPage.Annotations.Add(dAnnot);
                    }
                    list.Add(hyperlink);
                }
            }

            foreach (HtmlHyperLink dLink in Element.DocumentLinksCollection)
            {
                 float pageHeight = result.Height;
                 if (shapeLayoutBounds.Y < dLink.Bounds.Y && (pageHeight + shapeLayoutBounds.Y) > dLink.Bounds.Y)
                 {
                     RectangleF bounds = dLink.Bounds;
                     bounds.Y -= shapeLayoutBounds.Y;
                     PdfUriAnnotation dAnnot = new PdfUriAnnotation(bounds);
                     dAnnot.ApplyText(dLink.Name);
                     currentPage.Annotations.Add(dAnnot);
                     list.Add(dLink);
                 }
            }

            RepositionLinks(list, result.Height);
            return result;
        }

        /// <summary>
        /// Corrects current bounds on the page to restrict blank pages.
        /// </summary>
        /// <param name="currentPage">Current page.</param>
        /// <param name="currentBounds">Current lay outing bounds.</param>
        /// <param name="shapeLayoutBounds">The current active shape bounds.</param>        
        /// <returns>Corrected lay outing bounds.</returns>
        protected override float ToCorrectBounds(RectangleF currentBounds, RectangleF shapeLayoutBounds, PdfPage currentPage)
        {
            RectangleF result1 = currentBounds;
            int j = 0;
            bool final = false;
            float y = 0;
            do
            {
                for (int i = (int)result1.Height; i > 0; i--)
                {
                    y = shapeLayoutBounds.Y + i;
                    PdfUnitConvertor convertor = new PdfUnitConvertor(Element.VerticalResolution);
                    y = convertor.ConvertToPixels(y, PdfGraphicsUnit.Point);
                    //Search for text regions.
                    y = TextRegions.GetCoordinate(y);
                    y = convertor.ConvertFromPixels(y, PdfGraphicsUnit.Point);
                    bool text = (y != 0);                  

                    y = convertor.ConvertToPixels(y, PdfGraphicsUnit.Point);
                    //Search for image regions.
                    y = ImageRegions.GetCoordinate(y);
                    y = convertor.ConvertFromPixels(y, PdfGraphicsUnit.Point);
                    bool image = (y != 0);

                    final = (text || image);
                    if (final)
                    {
                        y = y - shapeLayoutBounds.Y;
                        result1.Height = y - 1f;
                        j++;
                        break;
                    }
                }
            } while (final && j < 2);

            return result1.Height;
        }

        #endregion
    }

    /// <summary>
    /// Represents the Class which defines metafile lay outing settings.
    /// </summary>
    public class PdfMetafileLayoutFormat : PdfLayoutFormat
    {
        #region Fields
        /// <summary>
        /// Indicates whether text line can be split among the pages.
        /// </summary>
        private bool m_splitLines;

        /// <summary>
        /// Indicates whether the images can be split among the pages.
        /// </summary>
        private bool m_splitImages;

        private bool m_htmlPageBreak;
        private float m_trackHeight;
        #endregion

        #region Properties
        /// <summary>
        /// Gets or sets a value indicating whether [split text lines].
        /// </summary>
        /// <value><c>true</c> if [split text lines]; otherwise, <c>false</c>.</value>
        public bool SplitTextLines
        {
            get
            {
                return m_splitLines;
            }

            set
            {
                m_splitLines = value;
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether [split images].
        /// </summary>
        /// <value><c>true</c> if [split images]; otherwise, <c>false</c>.</value>
        public bool SplitImages
        {
            get
            {
                return m_splitImages;
            }

            set
            {
                m_splitImages = value;
            }
        }


        internal bool IsHTMLPageBreak
        {
            get
            {
                return m_htmlPageBreak;
            }

            set
            {
                m_htmlPageBreak = value;
            }
        }

        internal float TrackHeight
        {
            get
            {
                return m_trackHeight;
            }

            set
            {
                m_trackHeight = value;
            }
        }
        #endregion
    }
}
#endif