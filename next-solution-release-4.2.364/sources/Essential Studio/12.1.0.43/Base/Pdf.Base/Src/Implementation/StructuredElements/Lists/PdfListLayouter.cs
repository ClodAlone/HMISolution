#region Copyright Syncfusion Inc. 2001 - 2014
// Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Any infringement will be prosecuted under
// applicable laws. 
#endregion

using System;
using System.Collections;
using System.Drawing;
using System.Collections.Generic;
using Syncfusion.Pdf.Graphics;


/// <summary>
/// The Syncfusion.Pdf.Lists namespace contains classes for creating structure elements in PDF document.
/// </summary>
namespace Syncfusion.Pdf.Lists
{
    /// <summary>
    /// Layouts list.
    /// </summary>
    /// <seealso cref="ElementLayouter"/> Class  
    internal class PdfListLayouter : ElementLayouter
    {
        #region Fields
        /// <summary>
        /// Current graphics for lay outing.
        /// </summary>
        private PdfGraphics m_graphics;

        /// <summary>
        /// Indicates end of lay outing.
        /// </summary>
        private bool m_finish = false;

        /// <summary>
        /// List that layouts at the moment.
        /// </summary>
        private PdfList m_curList;

        /// <summary>
        /// Stack than contains ListInfo.
        /// </summary>
        private Stack<ListInfo> m_info = new Stack<ListInfo>();

        /// <summary>
        /// Index of item that lay outing.
        /// </summary>
        private int m_index = 0;

        /// <summary>
        /// The indent of current list.
        /// </summary>
        private float m_indent;

        /// <summary>
        /// Height in which it stop lay outing.
        /// </summary>
        private float m_resultHeight;

        /// <summary>
        /// Lay outing bounds.
        /// </summary>
        private RectangleF m_bounds;

        /// <summary>
        /// Current page for layout.
        /// </summary>
        private PdfPage currentPage;

        /// <summary>
        /// Size for item lay outing.
        /// </summary>
        private SizeF size;

        /// <summary>
        /// If true it use paginate bounds if it is set.
        /// </summary>
        private bool usePaginateBounds = true;

        /// <summary>
        /// Current brush for lay outing.
        /// </summary>
        private PdfBrush currentBrush;

        /// <summary>
        /// Current pen for layout.
        /// </summary>
        private PdfPen currentPen;

        /// <summary>
        /// Current font for layout.
        /// </summary>
        private PdfFont currentFont;

        /// <summary>
        /// Current string format.
        /// </summary>
        private PdfStringFormat currentFormat;

        /// <summary>
        /// Marker maximum width.
        /// </summary>
        private float markerMaxWidth;
        #endregion

        #region Properties
        /// <summary>
        /// Gets or sets element.
        /// </summary>
        public new PdfList Element
        {
            get
            {
                return (base.Element as PdfList);
            }
        }
        #endregion

        #region Constructors
        /// <summary>
        /// Initializes a new instance of the <see cref="PdfListLayouter"/> class.
        /// </summary>
        /// <param name="element">The element.</param>
        public PdfListLayouter(PdfList element)
            : base(element)
        {
        }
        #endregion

        #region Implementation
        /// <summary>
        /// Layouts on the specified Graphics.
        /// </summary>
        /// <param name="graphics">The Graphics.</param>
        /// <param name="x">The x-coordinate of element.</param>
        /// <param name="y">The y-coordinate of element.</param>
        public void Layout(PdfGraphics graphics, float x, float y)
        {
            PointF location = new PointF(x, y);
            RectangleF bounds = new RectangleF(location, SizeF.Empty);

            Layout(graphics, bounds);
        }

        /// <summary>
        /// Layouts on the specified Graphics.
        /// </summary>
        /// <param name="graphics">The graphics to draw.</param>
        /// <param name="point">The location point.</param>
        public void Layout(PdfGraphics graphics, PointF point)
        {
            RectangleF bounds = new RectangleF(point, SizeF.Empty);

            Layout(graphics, bounds);
        }

        /// <summary>
        /// Layouts on the specified Graphics.
        /// </summary>
        /// <param name="graphics">The graphics to draw.</param>
        /// <param name="boundaries">The location boundaries.</param>
        public void Layout(PdfGraphics graphics, RectangleF boundaries)
        {
            if (graphics == null)
                throw new ArgumentNullException("graphics");

            m_graphics = graphics;

            PdfLayoutParams param = new PdfLayoutParams();
            param.Bounds = boundaries;
            param.Format = new PdfLayoutFormat();
            param.Format.Layout = PdfLayoutType.OnePage;

            LayoutInternal(param);
        }

        /// <summary>
        /// Layouts the element.
        /// </summary>
        /// <param name="param">Lay outing parameters.</param>
        /// <returns>Lay outing result.</returns>
        protected override PdfLayoutResult LayoutInternal(PdfLayoutParams param)
        {
            currentPage = param.Page;
            m_bounds = param.Bounds;

            if (param.Bounds.Size == SizeF.Empty && currentPage != null)
            {
                m_bounds.Size = currentPage.GetClientSize();
                m_bounds.Width -= m_bounds.X;
                m_bounds.Height -= m_bounds.Y;
            }

            if (currentPage != null)
            {
                m_graphics = currentPage.Graphics;
            }

            PageLayoutResult pageResult = new PageLayoutResult();
            pageResult.Broken = false;
            pageResult.Y = m_bounds.Y;
            m_curList = Element;
            m_indent = Element.Indent;

            SetCurrentParameters(Element);

            if (Element.Brush == null)
            {
                currentBrush = PdfBrushes.Black;
            }

            if (Element.Font == null)
            {
                currentFont = PdfDocument.DefaultFont;
            }

            if (m_curList is PdfOrderedList)
            {
                markerMaxWidth = GetMarkerMaxWidth(m_curList as PdfOrderedList, m_info);
            }

            bool useOnePage = (param.Format.Layout == PdfLayoutType.OnePage);

            while (!m_finish)
            {
                bool cancel = BeforePageLayout(m_bounds, currentPage, m_curList);

                pageResult.Y = m_bounds.Y;

                ListEndPageLayoutEventArgs endArgs = null;

                if (!cancel)
                {

                    pageResult = LayoutOnPage(pageResult);

                    endArgs = AfterPageLayouted(m_bounds, currentPage, m_curList);

                    cancel = (endArgs == null) ? false : endArgs.Cancel;
                }

                if (useOnePage || cancel)
                {
                    break;
                }

                if (currentPage != null
                    && !m_finish)
                {
                    if ((endArgs != null)
                        && (endArgs.NextPage != null))
                    {
                        currentPage = endArgs.NextPage;
                    }
                    else
                    {
                        currentPage = GetNextPage(currentPage);
                    }

                    m_graphics = currentPage.Graphics;

                    if (param.Bounds.Size == SizeF.Empty)
                    {
                        m_bounds.Size = currentPage.GetClientSize();
                        m_bounds.Width -= m_bounds.X;
                        m_bounds.Height -= m_bounds.Y;
                    }

                    if ((param.Format != null)
                        && param.Format.UsePaginateBounds &&
                        usePaginateBounds)
                    {
                        m_bounds = param.Format.PaginateBounds;
                    }
                }
            }

            m_info.Clear();

            RectangleF finalBounds = new RectangleF(m_bounds.X, pageResult.Y, m_bounds.Width, m_resultHeight);
            PdfLayoutResult result = new PdfLayoutResult(currentPage, finalBounds);

            return result;
        }

        /// <summary>
        /// Gets the width of the marker max.
        /// </summary>
        /// <param name="list">The list.</param>
        /// <param name="info">The info.</param>
        private float GetMarkerMaxWidth(PdfOrderedList list, Stack<ListInfo> info)
        {
            float width = -1;

            for (int i = 0; i < list.Items.Count; i++)
            {
                PdfStringLayoutResult result =
                CreateOrderedMarkerResult(list, list.Items[i], i + list.Marker.StartNumber, info, true);

                if (width < result.ActualSize.Width)
                {
                    width = result.ActualSize.Width;
                }

            }

            return width;
        }

        /// <summary>
        /// Sets the current parameters.
        /// </summary>
        /// <param name="list">The list.</param>
        private void SetCurrentParameters(PdfList list)
        {
            if (list.Brush != null)
            {
                currentBrush = list.Brush;
            }

            if (list.Pen != null)
            {
                currentPen = list.Pen;
            }

            if (list.Font != null)
            {
                currentFont = list.Font;
            }

            if (list.StringFormat != null)
            {
                currentFormat = list.StringFormat;
            }
        }

        /// <summary>
        /// Sets the current parameters.
        /// </summary>
        /// <param name="item">The item.</param>
        private void SetCurrentParameters(PdfListItem item)
        {
            if (item.Brush != null)
            {
                currentBrush = item.Brush;
            }

            if (item.Pen != null)
            {
                currentPen = item.Pen;
            }

            if (item.Font != null)
            {
                currentFont = item.Font;
            }

            if (item.StringFormat != null)
            {
                currentFormat = item.StringFormat;
            }
        }

        /// <summary>
        /// Layouts the on the page.
        /// </summary>
        /// <param name="pageResult">The page layout result.</param>
        /// <returns>Returns page layout result.</returns>
        private PageLayoutResult LayoutOnPage(PageLayoutResult pageResult)
        {
            float height = 0;
            float resultantHeight = 0;
            float y = m_bounds.Y;
            float x = m_bounds.X;
            size = m_bounds.Size;
            size.Width -= m_indent;

            while (true)
            {
                for (; m_index < m_curList.Items.Count; ++m_index)
                {
                    PdfListItem item = m_curList.Items[m_index] as PdfListItem;

                    if (currentPage != null && !pageResult.Broken)
                    {
                        BeforeItemLayout(item, currentPage);
                    }

                    DrawItem(ref pageResult, x, m_curList, m_index, m_indent, m_info, item, ref height, ref y);
                    resultantHeight += height;
                    if (pageResult.Broken)
                    {
                        return pageResult;
                    }

                    if (currentPage != null)
                    {
                        AfterItemLayouted(item, currentPage);
                    }

                    pageResult.MarkerWrote = false;

                    if (item.SubList != null && item.SubList.Items.Count > 0)
                    {
                        if (m_curList is PdfOrderedList)
                        {
                            PdfOrderedList oList = m_curList as PdfOrderedList;
                            oList.Marker.CurrentIndex = m_index;
                            ListInfo info = new ListInfo(m_curList, m_index, oList.Marker.GetNumber());
                            info.Brush = currentBrush;
                            info.Font = currentFont;
                            info.Format = currentFormat;
                            info.Pen = currentPen;
                            info.MarkerWidth = markerMaxWidth;

                            m_info.Push(info);
                        }
                        else
                        {
                            ListInfo info = new ListInfo(m_curList, m_index);
                            info.Brush = currentBrush;
                            info.Font = currentFont;
                            info.Format = currentFormat;
                            info.Pen = currentPen;
                            m_info.Push(info);
                        }

                        m_curList = item.SubList;

                        if (m_curList is PdfOrderedList)
                        {
                            markerMaxWidth = GetMarkerMaxWidth(m_curList as PdfOrderedList, m_info);
                        }

                        m_index = -1;
                        m_indent += m_curList.Indent;
                        size.Width -= m_curList.Indent;

                        SetCurrentParameters(item);
                        SetCurrentParameters(m_curList);
                    }
                }

                if (m_info.Count == 0)
                {
                    m_resultHeight = resultantHeight;
                    m_finish = true;
                    break;
                }

                ListInfo listInfo = (ListInfo)m_info.Pop();

                m_index = listInfo.Index + 1;
                m_indent -= m_curList.Indent;
                size.Width += m_curList.Indent;
                markerMaxWidth = listInfo.MarkerWidth;

                currentBrush = listInfo.Brush;
                currentPen = listInfo.Pen;
                currentFont = listInfo.Font;
                currentFormat = listInfo.Format;

                m_curList = listInfo.List;
            }

            return pageResult;
        }

        /// <summary>
        /// Draws the item.
        /// </summary>
        /// <param name="pageResult">The page result.</param>
        /// <param name="x">The x position.</param>
        /// <param name="curList">The current list.</param>
        /// <param name="index">The index of the item.</param>
        /// <param name="indent">The indent of the list.</param>
        /// <param name="info">The list info.</param>
        /// <param name="item">The current item.</param>
        /// <param name="height">The current height.</param>
        /// <param name="y">The y position.</param>
        private void DrawItem(ref PageLayoutResult pageResult, float x, PdfList curList, int index, float indent,
                        Stack<ListInfo> info, PdfListItem item, ref float height, ref float y)
        {
            PdfStringLayouter layouter = new PdfStringLayouter();
            PdfStringLayoutResult markerResult = null;
            PdfStringLayoutResult result = null;

            bool wroteMaker = false;
            float textIndent = curList.TextIndent;

            float posY = height + y;
            float posX = indent + x;

            float itemHeight = 0;
            float markerHeight = 0;
            SizeF itemSize = size;

            string text = item.Text;
            string markerText = null;

            PdfBrush itemBrush = currentBrush;

            if (item.Brush != null)
            {
                itemBrush = item.Brush;
            }

            PdfPen itemPen = currentPen;

            if (item.Pen != null)
            {
                itemPen = item.Pen;
            }

            PdfFont itemFont = currentFont;

            if (item.Font != null)
            {
                itemFont = item.Font;
            }

            PdfStringFormat itemFormat = currentFormat;

            if (item.StringFormat != null)
            {
                itemFormat = item.StringFormat;
            }

            if (((size.Width <= 0) || (size.Width < itemFont.Size)) && currentPage != null)
                throw new Exception("There is not enough space to layout list.");

            size.Height -= height;

            PdfMarker marker = null;

            if (curList is PdfUnorderedList)
            {
                marker = (curList as PdfUnorderedList).Marker;
            }
            else
            {
                marker = (curList as PdfOrderedList).Marker;
            }

            if (pageResult.Broken)
            {
                text = pageResult.ItemText;
                markerText = pageResult.MarkerText;
            }

            bool canDrawMarker = true;

            if (markerText != null &&
                    ((marker is PdfUnorderedMarker) &&
                    ((marker as PdfUnorderedMarker).Style == PdfUnorderedMarkerStyle.CustomString)))
            {
                markerResult = layouter.Layout(markerText, GetMarkerFont(marker, item), GetMarkerFormat(marker, item), size);
                posX += markerResult.ActualSize.Width;
                pageResult.MarkerWidth = markerResult.ActualSize.Width;
                markerHeight = markerResult.ActualSize.Height;
                canDrawMarker = true;
            }
            else
            {
                markerResult = CreateMarkerResult(index, curList, info, item);

                if (markerResult != null)
                {
                    if (curList is PdfOrderedList)
                    {
                        posX += markerMaxWidth;
                        pageResult.MarkerWidth = markerMaxWidth;
                    }
                    else
                    {
                        posX += markerResult.ActualSize.Width;
                        pageResult.MarkerWidth = markerResult.ActualSize.Width;
                    }
                    markerHeight = markerResult.ActualSize.Height;

                    if (currentPage != null)
                    {
                        canDrawMarker = (markerHeight < size.Height);
                    }

                    if (markerResult.Empty)
                    {
                        canDrawMarker = false;
                    }

                }
                else
                {
                    posX += (marker as PdfUnorderedMarker).Size.Width;
                    pageResult.MarkerWidth = (marker as PdfUnorderedMarker).Size.Width;
                    markerHeight = (marker as PdfUnorderedMarker).Size.Height;

                    if (currentPage != null)
                    {
                        canDrawMarker = (markerHeight < size.Height);
                    }
                }
            }

            if (markerText == null || markerText == string.Empty)
            {
                canDrawMarker = true;
            }

            if ((text != null) && canDrawMarker)
            {
                itemSize = size;
                itemSize.Width -= pageResult.MarkerWidth;

                if (item.TextIndent == 0)
                {
                    itemSize.Width -= textIndent;
                }
                else
                {
                    itemSize.Width -= item.TextIndent;
                }

                if (((itemSize.Width <= 0) || (itemSize.Width < itemFont.Size)) && currentPage != null)
                    throw new Exception("There is not enough space to layout the item text. Marker is too long or there is no enough space to draw it.");

                float itemX = posX;

                if (!marker.RightToLeft)
                {
                    if (item.TextIndent == 0)
                    {
                        itemX += textIndent;
                    }
                    else
                    {
                        itemX += item.TextIndent;
                    }
                }
                else
                {
                    itemX -= pageResult.MarkerWidth;

                    if (itemFormat != null &&
                            (itemFormat.Alignment == PdfTextAlignment.Right ||
                            itemFormat.Alignment == PdfTextAlignment.Center))
                    {
                        itemX -= indent;
                    }
                }

                if (currentPage == null)
                {
                    if (itemFormat != null)
                    {
                        itemFormat = (PdfStringFormat)itemFormat.Clone();
                        itemFormat.Alignment = PdfTextAlignment.Left;
                    }
                }

                result = layouter.Layout(text, itemFont, itemFormat, itemSize);
                RectangleF rect = new RectangleF(itemX, posY, itemSize.Width, itemSize.Height);
                m_graphics.DrawStringLayoutResult(result, itemFont, itemPen, itemBrush, rect, itemFormat);

                y = posY;

                itemHeight = result.ActualSize.Height;
            }

            height = (itemHeight < markerHeight) ? markerHeight : itemHeight;

            if ((result != null) && !(IsNullOrEmpty(result.Remainder))
                                            || (markerResult != null) && !(IsNullOrEmpty(markerResult.Remainder))
                                                || !canDrawMarker)
            {
                y = 0;
                height = 0;

                if (result != null)
                {
                    pageResult.ItemText = result.Remainder;

                    if (result.Remainder == item.Text)
                    {
                        canDrawMarker = false;
                    }
                }
                else
                {
                    if (!canDrawMarker)
                    {
                        pageResult.ItemText = item.Text;
                    }
                    else
                    {

                        pageResult.ItemText = null;
                    }
                }


                if (markerResult != null)
                {
                    pageResult.MarkerText = markerResult.Remainder;
                }
                else
                {
                    pageResult.MarkerText = null;
                }

                pageResult.Broken = true;
                pageResult.Y = 0;
                m_bounds.Y = 0;
            }
            else
            {
                pageResult.Broken = false;
            }

            if (result != null)
            {
                pageResult.MarkerX = posX;

                if (itemFormat != null)
                {

                    switch (itemFormat.Alignment)
                    {
                        case PdfTextAlignment.Right:
                            pageResult.MarkerX = posX + itemSize.Width - result.ActualSize.Width;
                            break;

                        case PdfTextAlignment.Center:
                            pageResult.MarkerX = posX + (itemSize.Width / 2) - (result.ActualSize.Width / 2);
                            break;
                    }
                }

                if (marker.RightToLeft)
                {
                    pageResult.MarkerX += result.ActualSize.Width;

                    if (item.TextIndent == 0)
                    {
                        pageResult.MarkerX += textIndent;
                    }
                    else
                    {
                        pageResult.MarkerX += item.TextIndent;
                    }

                    if (itemFormat != null &&
                            (itemFormat.Alignment == PdfTextAlignment.Right ||
                            itemFormat.Alignment == PdfTextAlignment.Center))
                    {
                        pageResult.MarkerX -= indent;
                    }
                }
            }

            if (((marker is PdfUnorderedMarker) &&
                            ((marker as PdfUnorderedMarker).Style == PdfUnorderedMarkerStyle.CustomString)))
            {
                if (markerResult != null)
                {
                    wroteMaker = DrawMarker(curList, item, markerResult, posY, pageResult.MarkerX);
                    pageResult.MarkerWrote = true;
                    pageResult.MarkerWidth = markerResult.ActualSize.Width;
                }
            }
            else
            {
                if (canDrawMarker && !pageResult.MarkerWrote)
                {
                    wroteMaker = DrawMarker(curList, item, markerResult, posY, pageResult.MarkerX);
                    pageResult.MarkerWrote = wroteMaker;

                    if (curList is PdfOrderedList)
                    {
                        pageResult.MarkerWidth = markerResult.ActualSize.Width;
                    }
                    else
                    {
                        pageResult.MarkerWidth = (marker as PdfUnorderedMarker).Size.Width;
                    }
                }
            }
        }

        /// <summary>
        /// Determines whether is null or empty the specified text.
        /// </summary>
        /// <param name="text">The text.</param>
        /// <returns>
        /// 	<c>true</c> if is null or empty the specified text, otherwise, <c>false</c>.
        /// </returns>
        private static bool IsNullOrEmpty(string text)
        {
            return ((text == null) || (text == String.Empty));
        }

        /// <summary>
        /// Afters the item layouted.
        /// </summary>
        /// <param name="item">The item that layout.</param>
        /// <param name="page">The page at which item layouted.</param>
        private void AfterItemLayouted(PdfListItem item, PdfPage page)
        {
            EndItemLayoutEventArgs args = new EndItemLayoutEventArgs(item, page);

            Element.OnEndItemLayout(args);
        }

        /// <summary>
        /// Before the item layout.
        /// </summary>
        /// <param name="item">The item that layouts.</param>
        /// <param name="page">The page at which item layout.</param>
        private void BeforeItemLayout(PdfListItem item, PdfPage page)
        {
            BeginItemLayoutEventArgs args = new BeginItemLayoutEventArgs(item, page);

            Element.OnBeginItemLayout(args);
        }

        /// <summary>
        /// After the page layouted.
        /// </summary>
        /// <param name="currentBounds">The current bounds.</param>
        /// <param name="currentPage">The current page.</param>
        /// <param name="list">The current list.</param>
        private ListEndPageLayoutEventArgs AfterPageLayouted(RectangleF currentBounds, PdfPage currentPage, PdfList list)
        {
            ListEndPageLayoutEventArgs args = null;

            if (Element.RaiseEndPageLayout && currentPage != null)
            {
                PdfLayoutResult result = new PdfLayoutResult(currentPage, currentBounds);
                args = new ListEndPageLayoutEventArgs(result, list);

                Element.OnEndPageLayout(args);
            }

            return args;
        }

        /// <summary>
        /// Before the page layout.
        /// </summary>
        /// <param name="currentBounds">The current bounds.</param>
        /// <param name="currentPage">The current page.</param>
        /// <param name="list">The cuurent list.</param>
        private bool BeforePageLayout(RectangleF currentBounds, PdfPage currentPage, PdfList list)
        {
            bool cancel = false;

            if (Element.RaiseBeginPageLayout && currentPage != null)
            {
                ListBeginPageLayoutEventArgs args = new ListBeginPageLayoutEventArgs(currentBounds, currentPage, list);

                Element.OnBeginPageLayout(args);
                cancel = args.Cancel;
                m_bounds = args.Bounds;
                usePaginateBounds = false;
            }

            return cancel;
        }

        /// <summary>
        /// Creates the marker result.
        /// </summary>
        /// <param name="index">The index of the item.</param>
        /// <param name="curList">The current list.</param>
        /// <param name="info">The list info.</param>
        /// <param name="item">The current item.</param>
        /// <returns>Returns marker layout result.</returns>
        private PdfStringLayoutResult CreateMarkerResult(int index, PdfList curList, Stack<ListInfo> info, PdfListItem item)
        {
            PdfStringLayoutResult markerResult = null;

            if (curList is PdfOrderedList)
            {
                markerResult = CreateOrderedMarkerResult(curList, item, index, info, false);
            }
            else
            {
                SizeF markerSize = SizeF.Empty;
                markerResult = CreateUnorderedMarkerResult(curList, item, ref markerSize);
            }

            return markerResult;
        }


        /// <summary>
        /// Craetes the unordered marker result.
        /// </summary>
        /// <param name="curList">The current list.</param>
        /// <param name="item">The current item.</param>
        /// <param name="markerSize">Size of the marker.</param>
        /// <returns></returns>
        private PdfStringLayoutResult CreateUnorderedMarkerResult(PdfList curList, PdfListItem item, ref SizeF markerSize)
        {
            PdfUnorderedMarker marker = (curList as PdfUnorderedList).Marker;
            PdfStringLayoutResult result = null;

            PdfFont markerFont = GetMarkerFont(marker, item);
            PdfStringFormat markerFormat = GetMarkerFormat(marker, item);

            PdfStringLayouter layouter = new PdfStringLayouter();

            switch (marker.Style)
            {
                case PdfUnorderedMarkerStyle.CustomImage:
                    markerSize = new SizeF(markerFont.Size, markerFont.Size);
                    marker.Size = markerSize;
                    break;

                case PdfUnorderedMarkerStyle.CustomTemplate:
                    markerSize = new SizeF(markerFont.Size, markerFont.Size);
                    marker.Size = markerSize;
                    break;

                case PdfUnorderedMarkerStyle.CustomString:
                    result = layouter.Layout(marker.Text, markerFont, markerFormat, size);
                    break;

                default:
                    PdfStandardFont uFont = new PdfStandardFont(PdfFontFamily.ZapfDingbats, markerFont.Size);

                    result = layouter.Layout(marker.GetStyledText(), uFont, null, size);
                    marker.Size = result.ActualSize;

                    if (marker.Pen != null)
                    {
                        result.m_actualSize = new SizeF(result.ActualSize.Width + 2 * marker.Pen.Width, result.ActualSize.Height + 2 * marker.Pen.Width);
                    }
                    break;
            }

            return result;
        }

        /// <summary>
        /// Creates the ordered marker result.
        /// </summary>
        /// <param name="list">The list.</param>
        /// <param name="item">The item.</param>
        /// <param name="index">The index.</param>
        /// <param name="info">The info.</param>
        /// <param name="findMaxWidth">if it is to find max width, set to <c>true</c>.</param>
        /// <returns></returns>
        private PdfStringLayoutResult CreateOrderedMarkerResult(PdfList list, PdfListItem item, int index, Stack<ListInfo> info, bool findMaxWidth)
        {
            PdfOrderedList orderedList = list as PdfOrderedList;
            PdfOrderedMarker marker = orderedList.Marker;
            marker.CurrentIndex = index;

            string text = String.Empty;

            if (orderedList.Marker.Style != PdfNumberStyle.None)
            {
                text = orderedList.Marker.GetNumber() + orderedList.Marker.Suffix;
            }

            if (orderedList.MarkerHierarchy)
            {
                object[] listInfos = info.ToArray();

                for (int i = 0; i < listInfos.Length; i++)
                {
                    ListInfo listInfo = (ListInfo)listInfos[i];

                    orderedList = listInfo.List as PdfOrderedList;

                    if (orderedList == null || orderedList.Marker.Style == PdfNumberStyle.None)
                    {
                        break;
                    }

                    marker = orderedList.Marker;

                    text = listInfo.Number + marker.Delimiter + text;

                    if (!orderedList.MarkerHierarchy)
                    {
                        break;
                    }
                }
            }

            PdfStringLayouter layouter = new PdfStringLayouter();

            orderedList = list as PdfOrderedList;
            marker = orderedList.Marker;

            PdfFont markerFont = GetMarkerFont(marker, item);

            PdfStringFormat markerFormat = GetMarkerFormat(marker, item);
            SizeF markerSize = new SizeF(size.Width, size.Height);

            if (!findMaxWidth)
            {
                markerSize.Width = markerMaxWidth;

                markerFormat = SetMarkerStringFormat(marker, markerFormat);
            }

            PdfStringLayoutResult result = layouter.Layout(text, markerFont, markerFormat, markerSize);

            return result;
        }

        /// <summary>
        /// Sets the marker alingment.
        /// </summary>
        /// <param name="marker">The marker.</param>
        /// <param name="markerFormat">The marker format.</param>
        /// <returns>Markers string format.</returns>
        private PdfStringFormat SetMarkerStringFormat(PdfOrderedMarker marker, PdfStringFormat markerFormat)
        {
            if (markerFormat == null)
            {
                markerFormat = new PdfStringFormat();
            }
            else
            {
                markerFormat = (PdfStringFormat)markerFormat.Clone();
            }

            if (marker.StringFormat == null)
            {
                markerFormat.Alignment = PdfTextAlignment.Right;

                if (marker.RightToLeft)
                {
                    markerFormat.Alignment = PdfTextAlignment.Left;
                }
            }

            if (currentPage == null)
            {
                if (markerFormat != null)
                {
                    markerFormat = (PdfStringFormat)markerFormat.Clone();
                    markerFormat.Alignment = PdfTextAlignment.Left;
                }
            }
            return markerFormat;
        }

        /// <summary>
        /// Draws the marker.
        /// </summary>
        /// <param name="curList">The current list.</param>
        /// <param name="item">The current item.</param>
        /// <param name="markerResult">The current marker result.</param>
        /// <param name="posY">The current Y position.</param>
        /// <param name="posX">The current X position.</param>
        /// <returns>Returns true if marker have been drawn.</returns>
        private bool DrawMarker(PdfList curList, PdfListItem item, PdfStringLayoutResult markerResult,
                        float posY, float posX)
        {
            PdfStringLayoutResult result = null;

            if (curList is PdfOrderedList)
            {
                if (curList.Font != null && markerResult != null)
                {
                    if (curList.Font.Size > markerResult.m_actualSize.Height)
                    {
                        posY += (curList.Font.Size / 2) - (markerResult.m_actualSize.Height / 2);
                        markerResult.m_actualSize.Height += posY;
                    }
                }
                    result = DrawOrderedMarker(curList, markerResult, item, posX, posY);                
            }
            else

            {

                if (curList.Font != null && markerResult!=null)
                {
                    if (curList.Font.Size > markerResult.m_actualSize.Height)
                    {
                        posY += (curList.Font.Size / 2) - (markerResult.m_actualSize.Height / 2);
                        markerResult.m_actualSize.Height += posY;
                    }
                }
                    result = DrawUnorderedMarker(curList, markerResult, item, posX, posY);          
             }

            return true;
        }

        /// <summary>
        /// Draws the unordered marker.
        /// </summary>
        /// <param name="curList">The current list.</param>
        /// <param name="markerResult">The current marker result.</param>
        /// <param name="item">The current item.</param>
        /// <param name="posX">The current X position.</param>
        /// <param name="posY">The current Y position.</param>
        /// <returns></returns>
        private PdfStringLayoutResult DrawUnorderedMarker(PdfList curList, PdfStringLayoutResult markerResult, PdfListItem item,
                        float posX, float posY)
        {
            PdfUnorderedList uList = curList as PdfUnorderedList;
            PdfUnorderedMarker marker = uList.Marker;
            PdfFont markerFont = GetMarkerFont(marker, item);
            PdfPen markerPen = GetMarkerPen(marker, item);
            PdfBrush markerBrush = GetMarkerBrush(marker, item);
            PdfStringFormat markerFormat = GetMarkerFormat(marker, item);

            if (markerResult != null)
            {
                PointF location = new PointF(posX - markerResult.ActualSize.Width, posY);
                marker.Size = markerResult.ActualSize;

                if (marker.Style == PdfUnorderedMarkerStyle.CustomString)
                {
                    RectangleF rect = new RectangleF(location, markerResult.ActualSize);
                    m_graphics.DrawStringLayoutResult(markerResult, markerFont, markerPen, markerBrush, rect, markerFormat);
                }
                else
                {
                    marker.UnicodeFont = new PdfStandardFont(PdfFontFamily.ZapfDingbats, markerFont.Size);
                    marker.Draw(m_graphics, location, markerBrush, markerPen);
                }
            }
            else
            {
                marker.Size = new SizeF(markerFont.Size, markerFont.Size);
                PointF location = new PointF(posX - markerFont.Size, posY);
                marker.Draw(m_graphics, location, markerBrush, markerPen);
            }

            return null;
        }

        /// <summary>
        /// Draws the ordered marker.
        /// </summary>
        /// <param name="curList">The current list.</param>
        /// <param name="markerResult">The marker result.</param>
        /// <param name="item">The current item.</param>
        /// <param name="posX">The current X position.</param>
        /// <param name="posY">The current Y position.</param>
        /// <returns></returns>
        private PdfStringLayoutResult DrawOrderedMarker(PdfList curList, PdfStringLayoutResult markerResult, PdfListItem item,
                        float posX, float posY)
        {
            PdfOrderedList oList = curList as PdfOrderedList;
            PdfOrderedMarker marker = oList.Marker;
            PdfFont markerFont = GetMarkerFont(marker, item);
            PdfStringFormat markerFormat = GetMarkerFormat(marker, item);
            PdfPen markerPen = GetMarkerPen(marker, item);
            PdfBrush markerBrush = GetMarkerBrush(marker, item);


            PointF location = new PointF(posX - markerMaxWidth, posY);
            RectangleF rect = new RectangleF(location, markerResult.ActualSize);
            rect.Width = markerMaxWidth;

            markerFormat = SetMarkerStringFormat(marker, markerFormat);

            m_graphics.DrawStringLayoutResult(markerResult, markerFont, markerPen, markerBrush, rect, markerFormat);

            return markerResult;
        }

        /// <summary>
        /// Gets the markers font.
        /// </summary>
        /// <param name="marker">The marker.</param>
        /// <param name="item">The item.</param>
        /// <returns>Returns font of the marker</returns>
        private PdfFont GetMarkerFont(PdfMarker marker, PdfListItem item)
        {
            PdfFont markerFont = marker.Font;

            if (marker.Font == null)
            {
                markerFont = item.Font;

                if (item.Font == null)
                {
                    markerFont = currentFont;
                }
            }

            marker.Font = markerFont;

            return markerFont;
        }

        /// <summary>
        /// Gets the marker format.
        /// </summary>
        /// <param name="marker">The marker.</param>
        /// <param name="item">The item.</param>
        /// <returns>Markers format.</returns>
        private PdfStringFormat GetMarkerFormat(PdfMarker marker, PdfListItem item)
        {
            PdfStringFormat markerFormat = marker.StringFormat;
            if (marker.StringFormat == null)
            {
                markerFormat = item.StringFormat;

                if (item.StringFormat == null)
                {
                    markerFormat = currentFormat;
                }
            }

            return markerFormat;
        }

        /// <summary>
        /// Gets the marker pen.
        /// </summary>
        /// <param name="marker">The marker.</param>
        /// <param name="item">The item.</param>
        /// <returns>Markers pen.</returns>
        private PdfPen GetMarkerPen(PdfMarker marker, PdfListItem item)
        {
            PdfPen markerPen = marker.Pen;
            if (marker.Pen == null)
            {
                markerPen = item.Pen;
                if (item.Pen == null)
                {
                    markerPen = currentPen;
                }
            }

            return markerPen;
        }

        /// <summary>
        /// Gets the marker brush.
        /// </summary>
        /// <param name="marker">The marker.</param>
        /// <param name="item">The item.</param>
        /// <returns>Markers brush.</returns>
        private PdfBrush GetMarkerBrush(PdfMarker marker, PdfListItem item)
        {
            PdfBrush markerBrush = marker.Brush;
            if (marker.Brush == null)
            {
                markerBrush = item.Brush;
                if (item.Brush == null)
                {
                    markerBrush = currentBrush;
                }
            }

            return markerBrush;
        }
        #endregion
    }

    /// <summary>
    /// Represents information about list.
    /// </summary>
    internal class ListInfo
    {
        #region Fields
        /// <summary>
        /// Index of list.
        /// </summary>
        private int m_index;

        /// <summary>
        /// Represents list.
        /// </summary>
        private PdfList m_list;

        /// <summary>
        /// The number of item at specified index.
        /// </summary>
        private string m_number;

        /// <summary>
        /// Lists brush.
        /// </summary>
        private PdfBrush m_brush;

        /// <summary>
        /// Lists pen.
        /// </summary>
        private PdfPen m_pen;

        /// <summary>
        /// Lists font.
        /// </summary>
        private PdfFont m_font;

        /// <summary>
        /// Lists format.
        /// </summary>
        private PdfStringFormat m_format;
        #endregion

        #region Properties
        /// <summary>
        /// Gets or sets the index.
        /// </summary>
        /// <value>The indexof the list.</value>
        internal int Index
        {
            get
            {
                return m_index;
            }
            set
            {
                m_index = value;
            }
        }

        /// <summary>
        /// Gets or sets the list.
        /// </summary>
        /// <value>The list.</value>
        internal PdfList List
        {
            get
            {
                return m_list;
            }
            set
            {
                m_list = value;
            }
        }

        /// <summary>
        /// Gets or sets the number.
        /// </summary>
        /// <value>The number of ordered list.</value>
        internal string Number
        {
            get
            {
                return m_number;
            }
            set
            {
                m_number = value;
            }
        }

        /// <summary>
        /// Gets or sets the brush.
        /// </summary>
        internal PdfBrush Brush
        {
            get
            {
                return m_brush;
            }
            set
            {
                m_brush = value;
            }
        }

        /// <summary>
        /// Gets or sets the pen.
        /// </summary>
        internal PdfPen Pen
        {
            get
            {
                return m_pen;
            }
            set
            {
                m_pen = value;
            }
        }

        /// <summary>
        /// Gets or sets the font.
        /// </summary>
        internal PdfFont Font
        {
            get
            {
                return m_font;
            }
            set
            {
                m_font = value;
            }
        }

        /// <summary>
        /// Gets or sets the format.
        /// </summary>
        /// <value>The format.</value>
        internal PdfStringFormat Format
        {
            get
            {
                return m_format;
            }
            set
            {
                m_format = value;
            }
        }

        /// <summary>
        /// Marker width;
        /// </summary>
        internal float MarkerWidth;
        #endregion

        #region Constructors
        /// <summary>
        /// Initializes a new instance of the <see cref="ListInfo"/> class.
        /// </summary>
        /// <param name="list">The list.</param>
        /// <param name="index">The index of the list.</param>
        /// <param name="number">The number if list is ordered list otherwise null.</param>
        internal ListInfo(PdfList list, int index, string number)
        {
            m_list = list;
            m_index = index;
            m_number = number;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ListInfo"/> class.
        /// </summary>
        /// <param name="list">The list.</param>
        /// <param name="index">The index.</param>
        internal ListInfo(PdfList list, int index)
            : this(list, index, string.Empty)
        {
        }
        #endregion
    }

    /// <summary>
    /// Represents Page Layout result.
    /// </summary>
    internal class PageLayoutResult
    {
        #region Fields
        /// <summary>
        /// If true item finished layout on page.
        /// </summary>
        public bool Broken;

        /// <summary>
        /// Y-ordinate of broken item of marker.
        /// </summary>
        public float Y;

        /// <summary>
        /// Text of item that was not draw.
        /// </summary>
        public string ItemText;

        /// <summary>
        /// Text of marker that was not draw.
        /// </summary>
        public string MarkerText;

        /// <summary>
        /// If true marker start draw.
        /// </summary>
        public bool MarkerWrote;

        /// <summary>
        /// Width of marker.
        /// </summary>
        public float MarkerWidth;

        /// <summary>
        /// X-coordinate of marker.
        /// </summary>
        public float MarkerX;
        #endregion
    }

    /// <summary>
    /// Represents begin page layout event arguments.
    /// </summary>
    /// <seealso cref="BeginPageLayoutEventArgs"/> Class 
    /// <example>
    /// <code lang="C#">
    ///  //Create a new PDf document
    ///  PdfDocument document = new PdfDocument();
    ///  //Creates a new page and adds it as the last page of the document
    ///  PdfPage page = document.Pages.Add();
    ///  PdfGraphics graphics = page.Graphics;
    ///  string[] products = { "Tools", "Grid", "Chart", "Edit", "Diagram", "XlsIO", "Grouping", "Calculate", "PDF", "HTMLUI", "DocIO" };                          
    ///  // Creates an item collection
    ///  PdfListItemCollection listItemCollection = new PdfListItemCollection(products);                                                                 
    ///  //Create a unordered list
    ///  PdfUnorderedList list = new PdfUnorderedList(listItemCollection);            
    ///  //Set the marker style
    ///  list.Marker.Style = PdfUnorderedMarkerStyle.Disk;
    ///  // Event handler
    ///  list.BeginPageLayout += new BeginPageLayoutEventHandler(list_BeginPageLayout);          
    ///  list.Draw(page, new RectangleF(0, 130, page.Graphics.ClientSize.Width, page.Graphics.ClientSize.Height));
    ///  document.Save("List.pdf");
    ///  // Event handler
    ///  void list_BeginPageLayout(object sender, BeginPageLayoutEventArgs e)
    ///  {
    ///   // Set the new bounds for the list
    ///   e.Bounds = new RectangleF(0, 0, e.Page.GetClientSize().Width, e.Page.GetClientSize().Height);            
    ///  }
    /// </code>
    /// <code lang="VB">
    /// 'Create a new PDf document
    /// Private document As PdfDocument = New PdfDocument()
    /// 'Create a page
    /// Private page As PdfPage = document.Pages.Add()
    /// Private graphics As PdfGraphics = page.Graphics
    /// Private products() As String = { "Tools", "Grid", "Chart", "Edit", "Diagram", "XlsIO", "Grouping", "Calculate", "PDF", "HTMLUI", "DocIO" }
    /// ' Creates an item collection
    /// Private listItemCollection As PdfListItemCollection = New PdfListItemCollection(products)
    /// 'Create a unordered list
    /// Private list As PdfUnorderedList = New PdfUnorderedList(listItemCollection)
    /// 'Set the marker style
    /// list.Marker.Style = PdfUnorderedMarkerStyle.Disk
    /// ' Event handler
    /// AddHandler list.BeginPageLayout, AddressOf list_BeginPageLayout
    /// list.Draw(page, New RectangleF(0, 130, page.Graphics.ClientSize.Width, page.Graphics.ClientSize.Height))
    /// document.Save("List.pdf")
    /// ' Event handler
    /// Private Sub list_BeginPageLayout(ByVal sender As Object, ByVal e As BeginPageLayoutEventArgs)
    ///  ' Set the new bounds for the list
    ///  e.Bounds = New RectangleF(0, 0, e.Page.GetClientSize().Width, e.Page.GetClientSize().Height)
    /// End Sub
    /// </example>
    public class ListBeginPageLayoutEventArgs : BeginPageLayoutEventArgs
    {
        #region Fields
        /// <summary>
        /// List that that starts layout.
        /// </summary>
        private PdfList m_list;
        #endregion

        #region Properties
        /// <summary>
        /// Gets the list that starts layout.
        /// </summary>
        /// <value>The list that starts layout.</value>
        /// <example>
        /// <code lang="C#">
        /// // Event handler
        /// void list_BeginPageLayout(object sender, BeginPageLayoutEventArgs e)
        /// { 
        ///   PdfUnorderedList list = sender as PdfUnorderedList;
        ///   list.Font = new PdfStandardFont(PdfFontFamily.Helvetica, 12);
        ///   // Set the new bounds for the list
        ///   e.Bounds = new RectangleF(0, 0, e.Page.GetClientSize().Width, e.Page.GetClientSize().Height);
        ///  }
        /// </code>
        /// <code lang="VB">
        /// ' Event handler
        /// Private Sub list_BeginPageLayout(ByVal sender As Object, ByVal e As BeginPageLayoutEventArgs)
        ///  Dim list As PdfUnorderedList = TryCast(sender, PdfUnorderedList)
        ///  list.Font = New PdfStandardFont(PdfFontFamily.Helvetica, 12)
        ///  ' Set the new bounds for the list
        ///  e.Bounds = New RectangleF(0, 0, e.Page.GetClientSize().Width, e.Page.GetClientSize().Height)
        /// End Sub
        /// </code>
        /// </example>
        public PdfList List
        {
            get
            {
                return m_list;
            }
        }
        #endregion

        #region Constructors
        /// <summary>
        /// Initializes a new instance of the <see cref="ListBeginPageLayoutEventArgs"/> class.
        /// </summary>
        /// <param name="bounds">The bounds of the list.</param>
        /// <param name="page">The page in which list layouts.</param>
        /// <param name="list">The list that starts layout.</param>
        internal ListBeginPageLayoutEventArgs(RectangleF bounds, PdfPage page, PdfList list)
            : base(bounds, page)
        {
            m_list = list;
        }
        #endregion
    }

    /// <summary>
    /// Represents begin page layout event arguments.
    /// </summary>
    /// <seealso cref="EndPageLayoutEventArgs"/> Class    
    public class ListEndPageLayoutEventArgs : EndPageLayoutEventArgs
    {
        #region Fields
        /// <summary>
        /// List that ended layout.
        /// </summary>
        private PdfList m_list;
        #endregion

        #region Properties
        /// <summary>
        /// Gets the list that ended layout.
        /// </summary>
        /// <value>The list that ended layout.</value>
        public PdfList List
        {
            get
            {
                return m_list;
            }
        }
        #endregion

        #region Constructors
        /// <summary>
        /// Initializes a new instance of the <see cref="ListEndPageLayoutEventArgs"/> class.
        /// </summary>
        /// <param name="layoutResult">The layout result.</param>
        /// <param name="list">The list that ended layout.</param>
        internal ListEndPageLayoutEventArgs(PdfLayoutResult layoutResult, PdfList list)
            : base(layoutResult)
        {
            m_list = list;
        }
        #endregion
    }
}
