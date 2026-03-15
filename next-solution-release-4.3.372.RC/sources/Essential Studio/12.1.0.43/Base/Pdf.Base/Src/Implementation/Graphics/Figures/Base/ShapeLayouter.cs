#region Copyright Syncfusion Inc. 2001 - 2014
// Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Any infringement will be prosecuted under
// applicable laws. 
#endregion

using System;
using System.Drawing;
using Syncfusion.Pdf.IO;
using Syncfusion.Pdf.Primitives;

namespace Syncfusion.Pdf.Graphics
{
    /// <summary>
    /// ShapeLayouter class.
    /// </summary>
    internal class ShapeLayouter : ElementLayouter
    {
        #region Fields
        /// <summary>
        /// Initializes the offset index.
        /// </summary>
        private static int index = 0;

        /// <summary>
        /// Initializes the difference in page height.
        /// </summary>
        private static float splitDiff = 0;

        /// <summary>
        /// Determines the end of Vertical offset values.
        /// </summary>
        private static bool last = false;
        #endregion

        #region Constructors
        /// <summary>
        /// Initializes a new instance of the <see cref="ShapeLayouter"/> class.
        /// </summary>
        /// <param name="element">The element.</param>
        public ShapeLayouter(PdfShapeElement element)
            : base(element)
        {
        }
        #endregion

        #region Properties
        /// <summary>
        /// Gets shape element.
        /// </summary>
        new public PdfShapeElement Element
        {
            get
            {
                return (base.Element as PdfShapeElement);
            }
        }

        #endregion

        #region Implementation
        /// <summary>
        /// Layouts the element.
        /// </summary>
        /// <param name="param">Lay outing parameters.</param>
        /// <returns>Lay outing result.</returns>
        protected override PdfLayoutResult LayoutInternal(PdfLayoutParams param)
        {
            if (param == null)
            {
                throw new ArgumentNullException("param");
            }

            PdfPage currentPage = param.Page;
            RectangleF currentBounds = param.Bounds;
            RectangleF shapeLayoutBounds = Element.GetBounds();
            shapeLayoutBounds.Location = PointF.Empty;

            PdfLayoutResult result = null;
            ShapeLayoutResult pageResult = new ShapeLayoutResult();

            pageResult.Page = currentPage;

            if (Element is PdfImage && (Element as PdfImage).ScrollBarHeight > 0.0)
            {
                if (shapeLayoutBounds.Height <= (Element as PdfImage).PhysicalDimension.Height)
                    shapeLayoutBounds.Height -= (Element as PdfImage).ScrollBarHeight;
                else
                    shapeLayoutBounds.Height = (Element as PdfImage).PhysicalDimension.Width - (Element as PdfImage).ScrollBarHeight;
            }
            while (true)
            {
                // Raise event.
                bool cancel = RaiseBeforePageLayout(currentPage, ref currentBounds);
                EndPageLayoutEventArgs endArgs = null;

                if (!cancel)
                {
                    pageResult = LayoutOnPage(currentPage, currentBounds, shapeLayoutBounds, param);

                    // Raise event.
                    endArgs = RaiseEndPageLayout(pageResult);
                    cancel = (endArgs == null) ? false : endArgs.Cancel;
                }

                // Tagged PDF
                if (pageResult.Page.Document.FileStructure.TaggedPdf && !pageResult.End && !cancel)
                {
                    return new PdfLayoutResult(pageResult.Page, pageResult.Bounds);
                }

                if (!pageResult.End && !cancel)
                {
                    currentBounds = GetPaginateBounds(param);
                    shapeLayoutBounds = GetNextShapeBounds(shapeLayoutBounds, pageResult);
                    currentPage = (endArgs == null || endArgs.NextPage == null) ?
                        GetNextPage(currentPage) : endArgs.NextPage;
                }
                else
                {
                    result = GetLayoutResult(pageResult);
                    break;
                }
            }

            return result;
        }

#if !SILVERLIGHT && !NETFX_CORE && !WP
        /// <summary>
        /// Layouts the HtmlToPdf element.
        /// </summary>
        /// <param name="param">Lay outing parameters.</param>
        /// <returns>Lay outing result.</returns>
        protected override PdfLayoutResult LayoutInternal(HtmlToPdf.HtmlToPdfLayoutParams param)
        {
            if (param == null)
            {
                throw new ArgumentNullException("param");
            }

            PdfLayoutParams layoutParam = new PdfLayoutParams();
            layoutParam.Bounds = param.Bounds;
            layoutParam.Format = param.Format;
            layoutParam.Page = param.Page;

            if (param.VerticalOffsets.Length == 1)
            {
                return LayoutInternal(layoutParam);
            }

            PdfPage currentPage = param.Page;
            RectangleF currentBounds = param.Bounds;
            RectangleF shapeLayoutBounds = Element.GetBounds();
            shapeLayoutBounds.Location = PointF.Empty;

            PdfLayoutResult result = null;
            ShapeLayoutResult pageResult = new ShapeLayoutResult();

            pageResult.Page = currentPage;

            if (param.Page.Section.Count == 1)
            {
                last = false;
                index = 0;
                splitDiff = 0;
            }
            if (Element is PdfImage && (Element as PdfImage).ScrollBarHeight > 0.0)
            {
                if (shapeLayoutBounds.Height <= (Element as PdfImage).PhysicalDimension.Height)
                    shapeLayoutBounds.Height -= (Element as PdfImage).ScrollBarHeight;
                else
                    shapeLayoutBounds.Height = (Element as PdfImage).PhysicalDimension.Width - (Element as PdfImage).ScrollBarHeight;
            }
            while (true)
            {
                int i = 0;
                float diff = 0;
                float height = 0;
				float newHeight = 0;
                float nNHeight = 0;
                float min = (param.Format as PdfMetafileLayoutFormat).TrackHeight;
                float trackHeight = min;
                bool flagOut = false;
                int count = param.VerticalOffsets.Length;
                foreach (float topValue in param.VerticalOffsets)
                {
                    if (param.VerticalOffsets[index] != topValue)
                        continue;
                    flagOut = false;
                    float valueCheck = topValue;
                    while (!flagOut)
                    {
                        if (valueCheck >= 0.0)
                        {
                            if (trackHeight > 0.0 && i == 0 && valueCheck > trackHeight)
                            {
                                valueCheck = topValue - trackHeight;
                                index--;
                            }
                            else
                                valueCheck = topValue;
                            nNHeight = Math.Min(currentPage.Graphics.ClientSize.Height, valueCheck);

                            if (nNHeight == valueCheck)
                            {
                                (layoutParam.Format as PdfMetafileLayoutFormat).IsHTMLPageBreak = true;
                                index++;
                            }
                            else if ((trackHeight + nNHeight) > valueCheck)
                            {
                                nNHeight = (valueCheck - trackHeight);
                                index++;
                                (layoutParam.Format as PdfMetafileLayoutFormat).IsHTMLPageBreak = true;
                            }
                        }
                        else
                            nNHeight = Math.Min(currentPage.Graphics.ClientSize.Height, shapeLayoutBounds.Height);

                        if (index == count)
                        {
                            index -= 1;
                            last = true;
                        }
                        if (i == 0)
                        {
                            float y = currentPage.Graphics.ClientSize.Height - param.Bounds.Y;
                            y = Math.Min(y, nNHeight);
                            if (y != nNHeight && (layoutParam.Format as PdfMetafileLayoutFormat).IsHTMLPageBreak)
                            {
                                index--;
                                (layoutParam.Format as PdfMetafileLayoutFormat).IsHTMLPageBreak = false;
                            }
                            y = y < 0 ? -y : y;
                            currentBounds = new RectangleF(0, param.Bounds.Y, 0, y);
                        }
                        else if (last)
                        {
                            currentBounds = RectangleF.Empty;
                            (layoutParam.Format as PdfMetafileLayoutFormat).IsHTMLPageBreak = false;
                        }
                        else
                            currentBounds = new RectangleF(0, 0, 0, nNHeight);

                        bool cancel = RaiseBeforePageLayout(currentPage, ref currentBounds);
                        EndPageLayoutEventArgs endArgs = null;

                        if (!cancel)
                        {
                            pageResult = LayoutOnPage(currentPage, currentBounds, shapeLayoutBounds, layoutParam);

                            // Raise event.
                            endArgs = RaiseEndPageLayout(pageResult);
                            cancel = (endArgs == null) ? false : endArgs.Cancel;
                            (layoutParam.Format as PdfMetafileLayoutFormat).IsHTMLPageBreak = false;
                        }

                        trackHeight += (pageResult.Bounds.Height > 0 ? pageResult.Bounds.Height : nNHeight);
                        i++;
                        if (!pageResult.End && !cancel)
                        {
                            currentBounds = GetPaginateBounds(layoutParam);
                            shapeLayoutBounds = GetNextShapeBounds(shapeLayoutBounds, pageResult);
                            currentPage = (endArgs == null || endArgs.NextPage == null) ?
                                GetNextPage(currentPage) : endArgs.NextPage;
                        }
                        else
                        {
                            result = GetLayoutResult(pageResult);
                            flagOut = true;
                            break;
                        }
                        if (((int)(trackHeight)) == ((int)(topValue)))
                        {
                            trackHeight = 0;
                            break;
                        }
                    }
                    if (pageResult.End)                                        
                        break;                   
                }
                (param.Format as PdfMetafileLayoutFormat).TrackHeight = trackHeight;
                break;
            }           
            return result;
        }
#endif

        /// <summary>
        /// Corrects current bounds on the page.
        /// </summary>
        /// <param name="currentPage">Current page.</param>
        /// <param name="currentBounds">Current lay outing bounds.</param>
        /// <param name="shapeLayoutBounds">The current active shape bounds.</param>
        /// <param name="param">Layout parameters.</param>
        /// <returns>Corrected lay outing bounds.</returns>
        protected virtual RectangleF CheckCorrectCurrentBounds(PdfPage currentPage,
            RectangleF currentBounds, RectangleF shapeLayoutBounds, PdfLayoutParams param)
        {
            if (currentPage == null)
            {
                throw new ArgumentNullException("currentPage");
            }

            SizeF pageSize = currentPage.Graphics.ClientSize;

            currentBounds.Width = (currentBounds.Width > 0) ? currentBounds.Width :
                pageSize.Width - currentBounds.X;

            currentBounds.Height = (currentBounds.Height > 0) ? currentBounds.Height :
                pageSize.Height - currentBounds.Y;

            return currentBounds;
        }

        /// <summary>
        /// Creates layout result.
        /// </summary>
        /// <param name="pageResult">Page layout result.</param>
        /// <returns>Layout result.</returns>
        private PdfLayoutResult GetLayoutResult(ShapeLayoutResult pageResult)
        {
            PdfLayoutResult result = new PdfLayoutResult(pageResult.Page, pageResult.Bounds);
            return result;
        }

        /// <summary>
        /// Layouts the element on the current page.
        /// </summary>
        /// <param name="currentPage">The current page.</param>
        /// <param name="currentBounds">The current bounds.</param>
        /// <param name="shapeLayoutBounds">Active shape bounds that aren't layouted.</param>
        /// <param name="param">Layout parameters.</param>
        /// <returns>Page lay outing result.</returns>
        private ShapeLayoutResult LayoutOnPage(PdfPage currentPage,
            RectangleF currentBounds, RectangleF shapeLayoutBounds, PdfLayoutParams param)
        {
            if (currentPage == null)
            {
                throw new ArgumentNullException("currentPage");
            }

            if (param == null)
            {
                throw new ArgumentNullException("param");
            }

            ShapeLayoutResult result = new ShapeLayoutResult();

            currentBounds = CheckCorrectCurrentBounds(currentPage, currentBounds, shapeLayoutBounds, param);

            if (Element is PdfImage && (Element as PdfImage).ScrollBarWidth > 0.0)
            {
                if (currentBounds.Width <= (Element as PdfImage).PhysicalDimension.Width)
                    currentBounds.Width -= (Element as PdfImage).ScrollBarWidth;
                else
                    currentBounds.Width = (Element as PdfImage).PhysicalDimension.Width - (Element as PdfImage).ScrollBarWidth;
            }
            bool fitToPage = FitsToBounds(currentBounds, shapeLayoutBounds);
            bool canDraw = !(param.Format.Break == PdfLayoutBreakType.FitElement && !fitToPage && currentPage == param.Page);
            bool shapeFinished = false;

            if (canDraw)
            {
                RectangleF drawRectangle = GetDrawBounds(currentBounds, shapeLayoutBounds);

# if !SILVERLIGHT && !NETFX_CORE && !WP
                if (shapeLayoutBounds.Height <= drawRectangle.Bottom)
                {
                    if (Element is PdfImage && (Element as PdfImage).ScrollBarHeight > 0.0)
                        currentBounds.Height = shapeLayoutBounds.Height;
                }
                if (Element is PdfMetafile && currentPage is PdfPage && currentPage.Section.ParentDocument is PdfDocument
                    && currentPage.Section.ParentDocument.FileStructure.TaggedPdf)
                    DrawShape(ref currentPage, currentBounds, drawRectangle, true);
                else
# endif
                    DrawShape(currentPage.Graphics, currentBounds, drawRectangle);

                result.Bounds = GetPageResultBounds(currentBounds, shapeLayoutBounds);
                shapeFinished = ((int)(currentBounds.Height)) >= ((int)(shapeLayoutBounds.Height));

                // Not required for page based layout

                //// Add page reference for text in Metafile
                //if ((currentPage is PdfPage) && currentPage.Document.FileStructure.TaggedPdf)
                //{
                //    PdfDocument doc = currentPage.Document;
                //    PdfStructTreeRoot structTreeRoot = PdfCrossTable.Dereference(doc.Catalog[DictionaryProperties.StructTreeRoot]) as PdfStructTreeRoot;
                //    if (structTreeRoot != null)
                //    {
                //        // Section implementation which made Adobe to read in reverse.
                //        //PdfDictionary kDict = PdfCrossTable.Dereference(structTreeRoot[DictionaryProperties.K]) as PdfDictionary;
                //        //PdfArray child = kDict[DictionaryProperties.K] as PdfArray;

                //        PdfArray child = structTreeRoot[DictionaryProperties.K] as PdfArray;

                //        PdfName paraName = new PdfName("P");
                //        foreach (PdfReferenceHolder refHolder in child)
                //        {
                //            PdfDictionary kDic = PdfCrossTable.Dereference(refHolder) as PdfDictionary;

                //            if (((kDic[DictionaryProperties.S] as PdfName).Value == "P") && !kDic.ContainsKey(DictionaryProperties.Pg))
                //            {
                //                PdfArray bBoxArray = (kDic[DictionaryProperties.A] as PdfDictionary)[DictionaryProperties.BBox] as PdfArray;
                //                float textYPos = (bBoxArray[1] as PdfNumber).FloatValue;
                //                if ((textYPos > currentBounds.Y) && (textYPos < (shapeLayoutBounds.Y + currentBounds.Height)))
                //                    kDic[DictionaryProperties.Pg] = new PdfReferenceHolder(currentPage);
                //            }
                //        }
                //    }
                //}

# if !SILVERLIGHT && !NETFX_CORE && !WP
                if (Element is PdfMetafile && currentPage is PdfPage && currentPage.Section.ParentDocument is PdfDocument 
                    && currentPage.Section.ParentDocument.FileStructure.TaggedPdf)
                {
                    if (currentPage.Graphics.Split > 0.0f)
                    {
                        result.End = false;
                        shapeFinished = true;
                        result.Page = currentPage;
                        result.Bounds = currentBounds;
                        result.Bounds.Height = Math.Min(currentBounds.Height, currentPage.Graphics.Split);
                        currentPage.Graphics.Split = 0.0f;
                        return result;
                    }
                    else
                    {
                        result.End = true;
                        shapeFinished = true;
                        result.Page = currentPage;
                        result.Bounds = currentBounds;
                        return result;
                    }
                }
# endif
            }

            result.End = (shapeFinished || param.Format.Layout == PdfLayoutType.OnePage);
            result.Page = currentPage;

            return result;
        }

# if !SILVERLIGHT && !NETFX_CORE && !WP
        /// <summary>
        /// Draws shape for Tagged PDF.
        /// </summary>
        /// <param name="pdfPage"></param>
        /// <param name="currentBounds"></param>
        /// <param name="drawRectangle"></param>
        /// <param name="tagged"></param>
        private void DrawShape(ref PdfPage pdfPage, RectangleF currentBounds, RectangleF drawRectangle, bool tagged)
        {
            PdfMetafile mfFile = (Element as PdfMetafile);
            using (System.Drawing.Imaging.Metafile metafile = mfFile.InternalImage.Clone() as System.Drawing.Imaging.Metafile)
            {
                using (Syncfusion.Pdf.Graphics.Images.Metafiles.PdfEmfRenderer renderer = new Syncfusion.Pdf.Graphics.Images.Metafiles.PdfEmfRenderer(pdfPage.Graphics, currentBounds.Location, true))
                {
                    using (Syncfusion.Pdf.Graphics.Images.Metafiles.MetaRecordParser parser = new Syncfusion.Pdf.Graphics.Images.Metafiles.MetaRecordParser(renderer, metafile))
                    {
                        //if (IsTranparency)
                        //{
                        //    renderer.AlphaBrush = AlphaBrush;
                        //    renderer.AlphaPen = AlphaPen;
                        //    renderer.BlendMode = BlendMode;
                        //    renderer.IsTranparency = IsTranparency;
                        //}

                        //This is required incase we run in to resolution problem,  it happens with EmfPlus files.
                        parser.Parser.PageScale = mfFile.PageScale;
                        parser.Parser.PageUnit = mfFile.PageUnit;

                        PdfGraphicsState state = pdfPage.Graphics.Save();
                        // Enumerate metafile.
                        parser.Enumerate();

                        PdfUnitConvertor convertor = new PdfUnitConvertor();
                        if (pdfPage.Graphics.Split > 0.0f)
                        {
                            Syncfusion.Pdf.Graphics.Images.Metafiles.TextRegionManager textRegions = renderer.Context as Syncfusion.Pdf.Graphics.Images.Metafiles.TextRegionManager;
                            Syncfusion.Pdf.Graphics.Images.Metafiles.ImageRegionManager imageRegions = parser.ImageContext as Syncfusion.Pdf.Graphics.Images.Metafiles.ImageRegionManager;
                            float split = convertor.ConvertToPixels(pdfPage.Graphics.Split, PdfGraphicsUnit.Point);
                            float current = textRegions.GetTopCoordinate(split);
                            current = imageRegions.GetTopCoordinate(current);

                            float width = convertor.ConvertToPixels(pdfPage.Graphics.ClientSize.Width, PdfGraphicsUnit.Point);
                            float height = convertor.ConvertToPixels(pdfPage.Graphics.ClientSize.Height, PdfGraphicsUnit.Point);

                            pdfPage.Graphics.DrawRectangle(PdfBrushes.White, new RectangleF(0, current, width, height - current));
                            pdfPage.Graphics.SetClip(new RectangleF(0, current, width, height - current));
                            
                            pdfPage.Graphics.Split = convertor.ConvertFromPixels(current, PdfGraphicsUnit.Point);
                        }
                        pdfPage.Graphics.Restore(state);

                        pdfPage = renderer.Graphics.Page as PdfPage;
                    }
                }
            }
        }
# endif

        /// <summary>
        /// Calculates the next active shape bounds.
        /// </summary>
        /// <param name="shapeLayoutBounds">The current active shape bounds.</param>
        /// <param name="pageResult">The current page layout result.</param>
        /// <returns>The next active shape bounds.</returns>
        private RectangleF GetNextShapeBounds(RectangleF shapeLayoutBounds, ShapeLayoutResult pageResult)
        {
            RectangleF layoutedBounds = pageResult.Bounds;

            shapeLayoutBounds.Y += layoutedBounds.Height;
            shapeLayoutBounds.Height -= layoutedBounds.Height;

            return shapeLayoutBounds;
        }

        /// <summary>
        /// Checks whether shape rectangle fits to the lay outing bounds.
        /// </summary>
        /// <param name="currentBounds">Lay outing bounds.</param>
        /// <param name="shapeLayoutBounds">Shape bounds.</param>
        /// <returns>True - if the shape fits into lay outing bounds, false otherwise.</returns>
        private bool FitsToBounds(RectangleF currentBounds, RectangleF shapeLayoutBounds)
        {
            bool fits = (shapeLayoutBounds.Height <= currentBounds.Height);

            return fits;
        }

        /// <summary>
        /// Returns Rectangle for element drawing on the page.
        /// </summary>
        /// <param name="currentBounds">Lay outing bounds.</param>
        /// <param name="shapeLayoutBounds">Current shape bounds.</param>
        /// <returns>Returns Rectangle for element drawing on the page.</returns>
        private RectangleF GetDrawBounds(RectangleF currentBounds, RectangleF shapeLayoutBounds)
        {
            RectangleF result = currentBounds;

            result.Y -= shapeLayoutBounds.Y;
            result.Height += shapeLayoutBounds.Y;

            return result;
        }

        /// <summary>
        /// Calculates bounds where the shape was layout on the page.
        /// </summary>
        /// <param name="currentBounds">Current lay outing bounds.</param>
        /// <param name="shapeLayoutBounds">Shape bounds.</param>
        /// <returns>Bounds where the shape was layout on the page.</returns>
        private RectangleF GetPageResultBounds(RectangleF currentBounds, RectangleF shapeLayoutBounds)
        {
            RectangleF result = currentBounds;

            result.Height = Math.Min(result.Height, shapeLayoutBounds.Height);

            return result;
        }

        /// <summary>
        /// Draws the shape.
        /// </summary>
        /// <param name="g">Current graphics.</param>
        /// <param name="currentBounds">Current page bounds.</param>
        /// <param name="drawRectangle">Draw rectangle.</param>
        private void DrawShape(PdfGraphics g, RectangleF currentBounds, RectangleF drawRectangle)
        {
            if (g == null)
            {
                throw new ArgumentNullException("g");
            }

            PdfGraphicsState gState = g.Save();

            try
            {
                g.SetClip(currentBounds);
                Element.Draw(g, drawRectangle.Location);
            }
            finally
            {
                g.Restore(gState);
            }
        }

        /// <summary>
        /// Raises PageLayout event if needed.
        /// </summary>
        /// <param name="pageResult">Page layout result.</param>
        /// <returns>Event arguments..</returns>
        private EndPageLayoutEventArgs RaiseEndPageLayout(ShapeLayoutResult pageResult)
        {
            EndPageLayoutEventArgs args = null;

            if (Element.RaiseEndPageLayout)
            {
                PdfLayoutResult res = GetLayoutResult(pageResult);
                args = new EndPageLayoutEventArgs(res);

                Element.OnEndPageLayout(args);
            }

            return args;
        }

        /// <summary>
        /// Raises BeforePageLayout event.
        /// </summary>
        /// <param name="currentPage">The current page.</param>
        /// <param name="currentBounds">The current bounds.</param>
        /// <returns>If true, stop lay outing.</returns>
        private bool RaiseBeforePageLayout(PdfPage currentPage, ref RectangleF currentBounds)
        {
            bool cancel = false;

            if (Element.RaiseBeginPageLayout)
            {
                BeginPageLayoutEventArgs args = new BeginPageLayoutEventArgs(currentBounds, currentPage);

                Element.OnBeginPageLayout(args);

                cancel = args.Cancel;
                currentBounds = args.Bounds;
            }

            return cancel;
        }

        /// <summary>
        /// Corrects the bounds to avoid blank page.
        /// </summary>
        /// <param name="currentBounds">Current lay outing bounds.</param>
        /// <param name="shapeLayoutBounds">Shape bounds.</param>
        /// <param name="currentPage">Current Page.</param>
        /// <returns>Corrected bounds.</returns>
        protected virtual float ToCorrectBounds(RectangleF currentBounds, RectangleF shapeLayoutBounds, PdfPage currentPage)
        {
            float height = currentBounds.Height;
            return height;
        }

        #endregion

        #region Internal declaration
        /// <summary>
        /// Contains lay outing result settings.
        /// </summary>
        private struct ShapeLayoutResult
        {
            /// <summary>
            /// The last page where the element was drawn.
            /// </summary>
            public PdfPage Page;

            /// <summary>
            /// The bounds of the element on the last page where it was drawn.
            /// </summary>
            public RectangleF Bounds;

            /// <summary>
            /// Indicates whether the lay outing has been finished.
            /// </summary>
            public bool End;
        }
        #endregion
    }
}
