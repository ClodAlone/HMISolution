#region Copyright Syncfusion Inc. 2001 - 2014
// Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Any infringement will be prosecuted under
// applicable laws. 
#endregion
using System;
using System.Net;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Ink;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;
using System.Text;
using System.Collections.Generic;

namespace Syncfusion.Windows.Tools.Controls
{
    public class RenderingManager
    {
        internal RichTextBoxAdv OwnerControl;
        StringBuilder builder;

        TextElementBox box;
        internal const double C_Thickness = 0.9;

        public RenderingManager(RichTextBoxAdv richTextBox)
        {
            OwnerControl = richTextBox;
            builder = new StringBuilder();
        }

        internal LayoutViewer LayoutViewer
        {
            get
            {
                return OwnerControl.Viewer;
            }
        }

        public void Render(LineInfo line, PageAdv page)
        {
            //if (line.RenderingOption != RenderingOptions.None)
            //{
                foreach (UIElement element in line.Elements)
                {
                    if (!page.ForegroundContainer.Children.Contains(element))
                    {
                        if ((element as FrameworkElement).Parent != null && (element as FrameworkElement).Parent is Canvas)
                            ((element as FrameworkElement).Parent as Canvas).Children.Remove(element);
                        page.ForegroundContainer.Children.Add(element);
                    }
                }
                if (line.TextRenderers.Count > 0)
                {
                    RemoveElementFromPage(line.TextRenderers);
                }
                if (line.RenderingOption == RenderingOptions.Render)
                {
                    line.TextRenderers.Clear();
                }
                if (line.DecoratingElements.Count > 0)
                {
                    RemoveElementFromPage(line.DecoratingElements);
                }
                line.DecoratingElements.Clear();
                RenderHighlight(line, page);
                RenderStrikeThrough(line, page);
                RenderUnderline(line, page);
                RenderText(line, page);
            //}
            if (line.IsTableLine)
            {
                foreach (ElementBox elementbox in line.ElementBoxes)
                {
                    if (elementbox is ChildTableCellElementBox)
                    {
                        foreach (LineInfo l in (elementbox as ChildTableCellElementBox).LineInfos)
                        {
                            if (line.PageIndex == l.PageIndex)
                            {
                                Render(l, page);
                            }
                        }
                    }
                    else if (elementbox is TableCellElementBox)
                    {
                        foreach (LineInfo templine in (elementbox as TableCellElementBox).LineInfos)
                        {
                            if (line.PageIndex == templine.PageIndex)
                            {
                                Render(templine, page);
                            }
                        }
                    }
                }
                RenderTable(line, page);
            }
        }

        internal void RenderText(LineInfo line, PageAdv page)
        {
            builder.Clear();
            bool cancheck = false;
            bool canContinuouslyRender = line.Block != null && line.Block is ParagraphAdv ? (line.Block as ParagraphAdv).TextAlignment != TextAlignment.Justify : true;

            if (line.TextRenderers.Count > 0)
            {
                RemoveElementFromPage(line.TextRenderers);
            }

            if (line.RenderingOption == RenderingOptions.RemoveAndAdd)
            {
                if (line.TextRenderers.Count > 0)
                {
                    RenderOnAddAndRemove(line, page, canContinuouslyRender);
                }
            }
            else
            {
                line.TextRenderers.Clear();

                foreach (ElementBox elementBox in line.ElementBoxes)
                {
                    if (elementBox is TextElementBox)
                    {
                        if (box == null)
                        {
                            builder.Append(elementBox.InternalText);
                            box = elementBox as TextElementBox;
                        }
                        else if (box.Inline.HasEqualTextStyle(elementBox.Inline) && canContinuouslyRender)
                        {
                            builder.Append(elementBox.InternalText);
                        }
                        else
                        {
                            TextBlock textBlock = CreateBlock(box, builder.ToString());
                            line.TextRenderers.Add(textBlock);
                            page.ForegroundContainer.Children.Add(textBlock);
                            SetPosition(box.ElementLocation, textBlock);
                            box = elementBox as TextElementBox;
                            builder.Clear();
                            builder.Append(elementBox.InternalText);
                        }
                        cancheck = true;
                    }
                    else if (box != null)
                    {
                        TextBlock textBlock = CreateBlock(box, builder.ToString());
                        line.TextRenderers.Add(textBlock);
                        page.ForegroundContainer.Children.Add(textBlock);
                        SetPosition(box.ElementLocation, textBlock);
                        box = null;
                        builder.Clear();
                    }
                }

                if (box != null)
                {
                    TextBlock textBlock = CreateBlock(box, builder.ToString());
                    line.TextRenderers.Add(textBlock);
                    page.ForegroundContainer.Children.Add(textBlock);
                    SetPosition(box.ElementLocation, textBlock);
                    box = null;
                    builder.Clear();
                }
                if (cancheck && line !=null)
                {
                    line.RenderingOption = RenderingOptions.None;
                }
            }
        }

        internal void RenderOnAddAndRemove(LineInfo line,PageAdv page,bool continous)
        {
            int i = 0;
            bool cancheck = false;

            foreach (ElementBox elementBox in line.ElementBoxes)
            {
                if (elementBox is TextElementBox)
                {
                    if (box == null)
                    {
                        builder.Append(elementBox.InternalText);
                        box = elementBox as TextElementBox;
                    }
                    else if (box.Inline.HasEqualTextStyle(elementBox.Inline) && continous)
                    {
                        builder.Append(elementBox.InternalText);
                    }
                    else
                    {
                        TextBlock textBlock = line.TextRenderers[i];
                        page.ForegroundContainer.Children.Add(textBlock);
                        SetPosition(box.ElementLocation, textBlock);
                        box = elementBox as TextElementBox;
                        builder.Clear();
                        builder.Append(elementBox.InternalText);
                        i++;
                    }
                    cancheck = true;
                }
                else if (box != null)
                {
                    TextBlock textBlock = line.TextRenderers[i];
                    page.ForegroundContainer.Children.Add(textBlock);
                    SetPosition(box.ElementLocation, textBlock);
                    box = null;
                    builder.Clear();
                    i++;
                }
            }

            if (box != null)
            {
                TextBlock textBlock = line.TextRenderers[i];
                page.ForegroundContainer.Children.Add(textBlock);
                SetPosition(box.ElementLocation, textBlock);
                box = null;
                builder.Clear();
                i++;
            }
            if (cancheck && line != null)
            {
                line.RenderingOption = RenderingOptions.None;
            }
        }


        internal void RenderHighlight(LineInfo line, PageAdv page)
        {
            double width = 0;
            double height = line.Height;

            bool hasHighlight = false;
            ElementBox element = null;

            foreach (ElementBox elementBox in line.ElementBoxes)
            {
                if (elementBox is TextElementBox || elementBox is HyperlinkElementBox)
                {
                    if (element == null)
                    {
                        element = elementBox;
                        hasHighlight = elementBox.Inline is SpanAdv ? (elementBox.Inline as SpanAdv).HighlightColor != null && (elementBox.Inline as SpanAdv).HighlightColor.ToString() != "#00000000" :
                            (elementBox.Inline as HyperlinkAdv).HighlightColor != null && (elementBox.Inline as HyperlinkAdv).HighlightColor.ToString() != "#00000000";
                        width = element.BoundingRectangle.Width;
                    }
                    else if (element.Inline.HasEqualHighlightStyle(elementBox.Inline) && hasHighlight)
                    {
                        width += elementBox.BoundingRectangle.Width;
                    }
                    else if (hasHighlight)
                    {
                        Color color = element.Inline is SpanAdv ? (element.Inline as SpanAdv).HighlightColor : (element.Inline as HyperlinkAdv).HighlightColor;
                        Path path = CreatePath(CreateRect(0, 0, width, height), color);
                        line.DecoratingElements.Add(path);
                        page.DecorationContainer.Children.Add(path);
                        SetPosition(element.Location, path);
                        element = elementBox;
                        hasHighlight = elementBox.Inline is SpanAdv ? (elementBox.Inline as SpanAdv).HighlightColor != null && (elementBox.Inline as SpanAdv).HighlightColor.ToString() != "#00000000" :
                            (elementBox.Inline as HyperlinkAdv).HighlightColor != null && (elementBox.Inline as HyperlinkAdv).HighlightColor.ToString() != "#00000000";
                        width = element.BoundingRectangle.Width;
                    }
                    else
                    {
                        element = elementBox;
                        hasHighlight = elementBox.Inline is SpanAdv ? (elementBox.Inline as SpanAdv).HighlightColor != null && (elementBox.Inline as SpanAdv).HighlightColor.ToString() != "#00000000" :
                            (elementBox.Inline as HyperlinkAdv).HighlightColor != null && (elementBox.Inline as HyperlinkAdv).HighlightColor.ToString() != "#00000000";
                        width = element.BoundingRectangle.Width;
                    }
                }
                else if (element != null && hasHighlight)
                {
                    Color color = element.Inline is SpanAdv ? (element.Inline as SpanAdv).HighlightColor : (element.Inline as HyperlinkAdv).HighlightColor;
                    Path path = CreatePath(CreateRect(0, 0, width, height), color);
                    line.DecoratingElements.Add(path);
                    page.DecorationContainer.Children.Add(path);
                    SetPosition(element.Location, path);
                    element = null;
                    hasHighlight = false;
                    width = 0;
                }
            }

            if (element != null && hasHighlight)
            {
                Color color = element.Inline is SpanAdv ? (element.Inline as SpanAdv).HighlightColor : (element.Inline as HyperlinkAdv).HighlightColor;
                Path path = CreatePath(CreateRect(0, 0, width, height), color);
                line.DecoratingElements.Add(path);
                page.DecorationContainer.Children.Add(path);
                SetPosition(element.Location, path);
                element = null;
                hasHighlight = false;
                width = 0;
            }
        }

        private Rect CreateRect(double x, double y, double width, double height)
        {
            return new Rect(x, y, width, height);
        }

        internal void RenderUnderline(LineInfo line, PageAdv page)
        {
            double width = 0;
            double height = line.Height;
            bool hasUnderline = false;
            ElementBox element = null;

            foreach (ElementBox elementBox in line.ElementBoxes)
            {
                if (elementBox is TextElementBox || elementBox is HyperlinkElementBox)
                {
                    if (element == null)
                    {
                        element = elementBox;
                        hasUnderline = elementBox.Inline is SpanAdv ? (elementBox.Inline as SpanAdv).Underline : (elementBox.Inline as HyperlinkAdv).Underline;
                        width = element.BoundingRectangle.Width;
                    }
                    else if (element.Inline.HasEqualUnderlineStyle(elementBox.Inline) && hasUnderline)
                    {
                        width += elementBox.BoundingRectangle.Width;
                    }
                    else if (hasUnderline)
                    {
                        Color color = element.Inline is SpanAdv ? (element.Inline as SpanAdv).Foreground : (element.Inline as HyperlinkAdv).Foreground;
                        Line lineElement = CreateLine(CreateRect(0, 0, width, height), color);
                        line.DecoratingElements.Add(lineElement);
                        page.DecorationContainer.Children.Add(lineElement);
                        SetPosition(CalculateUnderlineLocation(line, element), lineElement);
                        element = elementBox;
                        hasUnderline = elementBox.Inline is SpanAdv ? (elementBox.Inline as SpanAdv).Underline : (elementBox.Inline as HyperlinkAdv).Underline;
                        width = element.BoundingRectangle.Width;
                    }
                    else
                    {
                        element = elementBox;
                        hasUnderline = elementBox.Inline is SpanAdv ? (elementBox.Inline as SpanAdv).Underline : (elementBox.Inline as HyperlinkAdv).Underline;
                        width = element.BoundingRectangle.Width;
                    }
                }
                else if (element != null && hasUnderline)
                {
                    Color color = element.Inline is SpanAdv ? (element.Inline as SpanAdv).Foreground : (element.Inline as HyperlinkAdv).Foreground;
                    Line lineElement = CreateLine(CreateRect(0, 0, width, height), color);
                    line.DecoratingElements.Add(lineElement);
                    page.DecorationContainer.Children.Add(lineElement);
                    SetPosition(CalculateUnderlineLocation(line, element), lineElement);
                    element = null;
                    hasUnderline = false;
                    width = 0;
                }
            }

            if (element != null && hasUnderline)
            {
                Color color = element.Inline is SpanAdv ? (element.Inline as SpanAdv).Foreground : (element.Inline as HyperlinkAdv).Foreground;
                Line lineElement = CreateLine(CreateRect(0, 0, width, height), color);
                line.DecoratingElements.Add(lineElement);
                page.DecorationContainer.Children.Add(lineElement);
                SetPosition(CalculateUnderlineLocation(line, element), lineElement);
                hasUnderline = false;
                element = null;
                width = 0;
            }
        }

        private Point CalculateUnderlineLocation(LineInfo line, ElementBox elementBox)
        {
            Baseline baseline = elementBox.Inline is SpanAdv ? (elementBox.Inline as SpanAdv).Baseline : (elementBox.Inline as HyperlinkAdv).Baseline;
            FontStyle fontStyle = elementBox.Inline is SpanAdv ? (elementBox.Inline as SpanAdv).FontStyle : (elementBox.Inline as HyperlinkAdv).FontStyle;
            double fontSize = elementBox.Inline is SpanAdv ? (elementBox.Inline as SpanAdv).FontSize : (elementBox.Inline as HyperlinkAdv).FontSize;
            FontWeight fontWeight = elementBox.Inline is SpanAdv ? (elementBox.Inline as SpanAdv).FontWeight : (elementBox.Inline as HyperlinkAdv).FontWeight;
            FontFamily fontFamily = elementBox.Inline is SpanAdv ? (elementBox.Inline as SpanAdv).FontFamily : (elementBox.Inline as HyperlinkAdv).FontFamily;

            double remainingSpace = line.CalculatedLineSpace + line.AfterSpacing + line.Offset;
            if (baseline == Baseline.Subscript)
            {
                Size size = TextHelper.MeasureText(elementBox.InternalText, fontStyle, fontWeight, fontFamily, (60 * fontSize) / 100);
                if (remainingSpace - (TextHelper.TextMeasurer.BaselineOffset / 2) > 0)
                    remainingSpace -= (TextHelper.TextMeasurer.BaselineOffset / 2);
            }
            double start = elementBox.Size.Height - remainingSpace;
            double end = elementBox.Size.Height;
            double imaginaryPoint = 2;
            if (start + 2 >= end)
            {
                imaginaryPoint = end - start;
            }

            int x = (int)elementBox.Location.X;
            int y = (int)((elementBox.Size.Height - line.CalculatedLineSpace - line.AfterSpacing - line.Offset) + imaginaryPoint);

            return new Point(x, y + elementBox.Location.Y);
        }

        private Point CalculateSingleStrikeLocation(LineInfo line, ElementBox elementBox)
        {
            Baseline baseline = elementBox.Inline is SpanAdv ? (elementBox.Inline as SpanAdv).Baseline : (elementBox.Inline as HyperlinkAdv).Baseline;
            FontStyle fontStyle = elementBox.Inline is SpanAdv ? (elementBox.Inline as SpanAdv).FontStyle : (elementBox.Inline as HyperlinkAdv).FontStyle;
            double fontSize = elementBox.Inline is SpanAdv ? (elementBox.Inline as SpanAdv).FontSize : (elementBox.Inline as HyperlinkAdv).FontSize;
            FontWeight fontWeight = elementBox.Inline is SpanAdv ? (elementBox.Inline as SpanAdv).FontWeight : (elementBox.Inline as HyperlinkAdv).FontWeight;
            FontFamily fontFamily = elementBox.Inline is SpanAdv ? (elementBox.Inline as SpanAdv).FontFamily : (elementBox.Inline as HyperlinkAdv).FontFamily;

            double remainingSpace = line.CalculatedLineSpace + line.AfterSpacing + line.Offset;
            if (baseline == Baseline.Subscript)
            {
                Size size = TextHelper.MeasureText(elementBox.InternalText, fontStyle, fontWeight, fontFamily, (60 * fontSize) / 100);
                if (remainingSpace - (TextHelper.TextMeasurer.BaselineOffset / 2) > 0)
                    remainingSpace -= (TextHelper.TextMeasurer.BaselineOffset / 2);
            }
            double start = elementBox.Size.Height - remainingSpace;
            double end = elementBox.Size.Height;
            double imaginaryPoint = 2;
            if (start + 2 >= end)
            {
                imaginaryPoint = end - start;
            }

            int x = (int)elementBox.Location.X;
            int y = (int)((elementBox.Size.Height - line.CalculatedLineSpace - line.AfterSpacing - line.Offset) + imaginaryPoint);

            return new Point(x, y);
        }

        internal void RenderStrikeThrough(LineInfo line, PageAdv page)
        {
            double width = 0;
            double height = line.Height;
            StrikeThrough strikeThro = StrikeThrough.None;
            bool hasStrikeThro = false;
            ElementBox element = null;

            foreach (ElementBox elementBox in line.ElementBoxes)
            {
                if (elementBox is TextElementBox || elementBox is HyperlinkElementBox)
                {
                    if (element == null)
                    {
                        element = elementBox;
                        strikeThro = elementBox.Inline is SpanAdv ? (elementBox.Inline as SpanAdv).StrikeThrough : (elementBox.Inline as HyperlinkAdv).StrikeThrough;
                        hasStrikeThro = strikeThro != StrikeThrough.None;
                        width = element.BoundingRectangle.Width;
                    }
                    else if (element.Inline.HasEqualStrikeThroStyle(elementBox.Inline) && hasStrikeThro)
                    {
                        width += elementBox.BoundingRectangle.Width;
                    }
                    else if (hasStrikeThro)
                    {
                        Color color = element.Inline is SpanAdv ? (element.Inline as SpanAdv).Foreground : (element.Inline as HyperlinkAdv).Foreground;
                        Line lineElement1 = CreateLine(CreateRect(0, 0, width, height), color);
                        Line lineElement2 = null;
                        if (strikeThro == StrikeThrough.DoubleStrike)
                        {
                            lineElement2 = CreateLine(CreateRect(0, 0, width, height), color);
                            line.DecoratingElements.Add(lineElement2);
                            page.DecorationContainer.Children.Add(lineElement2);
                        }
                        line.DecoratingElements.Add(lineElement1);
                        page.DecorationContainer.Children.Add(lineElement1);
                        SetPosition(strikeThro, element, lineElement1, lineElement2);
                        element = elementBox;
                        strikeThro = elementBox.Inline is SpanAdv ? (elementBox.Inline as SpanAdv).StrikeThrough : (elementBox.Inline as HyperlinkAdv).StrikeThrough;
                        hasStrikeThro = strikeThro != StrikeThrough.None;
                        width = element.BoundingRectangle.Width;
                    }
                    else
                    {
                        element = elementBox;
                        strikeThro = elementBox.Inline is SpanAdv ? (elementBox.Inline as SpanAdv).StrikeThrough : (elementBox.Inline as HyperlinkAdv).StrikeThrough;
                        hasStrikeThro = strikeThro != StrikeThrough.None;
                        width = element.BoundingRectangle.Width;
                    }
                }
                else if (element != null && hasStrikeThro)
                {
                    Color color = element.Inline is SpanAdv ? (element.Inline as SpanAdv).Foreground : (element.Inline as HyperlinkAdv).Foreground;
                    Line lineElement1 = CreateLine(CreateRect(0, 0, width, height), color);
                    Line lineElement2 = null;
                    if (strikeThro == StrikeThrough.DoubleStrike)
                    {
                        lineElement2 = CreateLine(CreateRect(0, 0, width, height), color);
                        line.DecoratingElements.Add(lineElement2);
                        page.DecorationContainer.Children.Add(lineElement2);
                    }
                    line.DecoratingElements.Add(lineElement1);
                    page.DecorationContainer.Children.Add(lineElement1);
                    SetPosition(strikeThro, element, lineElement1, lineElement2);
                    element = null;
                    hasStrikeThro = false;
                    width = 0;
                }
            }

            if (element != null && hasStrikeThro)
            {
                Color color = element.Inline is SpanAdv ? (element.Inline as SpanAdv).Foreground : (element.Inline as HyperlinkAdv).Foreground;
                Line lineElement1 = CreateLine(CreateRect(0, 0, width, height), color);
                Line lineElement2 = null;
                if (strikeThro == StrikeThrough.DoubleStrike)
                {
                    lineElement2 = CreateLine(CreateRect(0, 0, width, height), color);
                    line.DecoratingElements.Add(lineElement2);
                    page.DecorationContainer.Children.Add(lineElement2);
                }

                line.DecoratingElements.Add(lineElement1);
                page.DecorationContainer.Children.Add(lineElement1);
                SetPosition(strikeThro, element, lineElement1, lineElement2);
                hasStrikeThro = false;
                element = null;
                width = 0;
            }
        }

        internal void RenderTable(LineInfo line, PageAdv page)
        {
            TableCellElementBox cbox = null;
            bool cancheck = false;
            foreach (ElementBox e in line.ElementBoxes)
            {
                if (e is TableCellElementBox || e is ChildTableCellElementBox)
                {
                    cbox = e as TableCellElementBox;
                    if ((cbox.IsBottomHide && !cbox.IsTopHide) || (!cbox.IsTopHide && !cbox.IsTopHide) || line.IsFirstLine)
                    {
                        Path tableoutline = CreateCell(new Rect(0, 0, cbox.BoundingRectangle.Width, cbox.BoundingRectangle.Height), cbox is TableCellElementBox ? (cbox as TableCellElementBox).Background : ((cbox as ChildTableCellElementBox).GetParentCellBox() as TableCellElementBox).Background, cbox);
                        cbox.AssociatedPath = tableoutline;
                        line.DecoratingElements.Add(tableoutline);
                        page.DecorationContainer.Children.Add(tableoutline);
                        SetPosition(cbox.Location, tableoutline);
                        cancheck = true;
                    }
                }
            }
            if (cancheck && line != null)
            {
                line.RenderingOption = RenderingOptions.None;
            }
        }

        private bool CheckRightHide(TableCellElementBox tablebox)
        {
            LineInfo line = tablebox.LineInfo;
            int index = line.ElementBoxes.IndexOf(tablebox);

            if (index == line.ElementBoxes.Count - 1)
                return false;

            return line.ElementBoxes[index + 1] != null;
        }

        private bool CheckBottomHide(TableCellElementBox tablebox)
        {
            LineInfo line = tablebox.LineInfo;
            int columnindex = tablebox.ColumnIndex;

            if (line.IsLastLine && tablebox.HasRowSpan() && !line.HasChildBoxes)
                return false;
            else if (line.IsLastLine)
                return false;

            if (tablebox.RowSpan > 1)
                return tablebox.IsBottomHide;

            return false;
        }

        private bool CheckTopHide(TableCellElementBox tablebox)
        {
            LineInfo line = tablebox.LineInfo;
            if (line.IsFirstLine)
                return false;
            else
                return tablebox.IsTopHide;
        }

        private Path CreateCell(Rect rect, Color color, TableCellElementBox tablebox)
        {
            double yValue = 0.0;
            double boxHeight = tablebox.BoundingRectangle.Height;

            TableCellElementBox bottom = tablebox.BottomCellBox;
            TableAdv table = (TableAdv)tablebox.LineInfo.Block;

            Path path = new Path();
            path.Stroke = new SolidColorBrush(table.BorderBrush);
            path.StrokeThickness = table.BorderThickness;
            path.Fill = new SolidColorBrush(CheckBackgroundColor(tablebox.BaseCell));
            //path.StrokeDashArray = table.StrokeDashArray;
            path.StrokeDashCap = table.StrokeDashCap;
            path.StrokeDashOffset = table.StrokeDashOffset;
            path.StrokeLineJoin = table.StrokeLineJoin;
            path.StrokeMiterLimit = table.StrokeMiterLimit;
            path.StrokeStartLineCap = table.StrokeStartLineCap;
            path.StrokeEndLineCap = table.StrokeEndLineCap;
            if (table.BorderStyle != null)
                path.Style = table.BorderStyle;

            PathGeometry pathGeo = new PathGeometry();
            PathFigure pathFig = new PathFigure();
            LineSegment lineSeg = new LineSegment();

            if (tablebox.HasRowSpan())
            {
                while (bottom !=null)
                {
                    boxHeight += bottom.BoundingRectangle.Height;
                    //yValue = bottom.BoundingRectangle.Y;
                    if (bottom.BottomCellBox != null && !bottom.LineInfo.IsLastLine)
                    {
                        bottom = bottom.BottomCellBox;
                    }
                    else
                        break;
                }
            }
            else
                yValue = rect.Y;

            lineSeg.Point = new Point(rect.X, yValue);

            pathFig.StartPoint = new Point(rect.X, yValue);
            pathFig.Segments.Add(lineSeg);

            LineSegment lineSeg1 = new LineSegment();
            lineSeg1.Point = new Point(rect.Right, rect.Top);
            pathFig.Segments.Add(lineSeg1);

            LineSegment lineSeg2 = new LineSegment();

            if (bottom != null)
            {
                lineSeg2.Point = new Point(rect.Right, boxHeight);
            }
            else
            {
                lineSeg2.Point = new Point(rect.Right, rect.Height);
            }
            pathFig.Segments.Add(lineSeg2);
            
            LineSegment lineSeg3 = new LineSegment();

            if (bottom != null)
            {
                lineSeg3.Point = new Point(rect.Left, boxHeight);
            }
            else
            {
                lineSeg3.Point = new Point(rect.Left, rect.Bottom);
            }
            pathFig.Segments.Add(lineSeg3);

            pathFig.IsClosed = true;
            pathGeo.Figures.Add(pathFig);
            path.Data = pathGeo;
            return path;
        }

        private Color CheckBackgroundColor(TableCellAdv cell)
        {
            Color color = cell.Background;

            if (IsEmpty(color))
            {
                if (IsEmpty(cell.OwnerRow.Background))
                {
                    if (IsEmpty(cell.ColumnBackground))
                    {
                        if (IsEmpty(cell.OwnerTable.Background))
                        {
                            return Colors.Transparent;
                        }
                        else
                        {
                            return cell.OwnerTable.Background;
                        }
                    }
                    else
                    {
                        return cell.ColumnBackground;
                    }
                }
                else
                {
                    return cell.OwnerRow.Background;
                }
            }
            else
            {
                return cell.Background;
            }
        }

        private bool IsEmpty(Color color)
        {
            return Math.Abs(color.R) == 0 && Math.Abs(color.G) == 0 && Math.Abs(color.B) == 0
                && Math.Abs(color.A) == 0;
        }

        private void SetPosition(StrikeThrough strikeThrough, ElementBox elementBox, Line line1, Line line2)
        {
            int x = (int)elementBox.Location.X;
            int y = (int)Math.Ceiling((elementBox.ElementLocation.Y + elementBox.ElementSize.Height / 2));
            SetPosition(new Point(x, y), line1);
            if (strikeThrough == StrikeThrough.DoubleStrike)
            {
                y = y + 2;
                SetPosition(new Point(x, y), line2);
            }
        }

        private void RemoveElementFromPage(List<TextBlock> blocks)
        {
            foreach (TextBlock block in blocks)
            {
                if (block.Parent != null)
                    (block.Parent as Canvas).Children.Remove(block);
            }
        }

        private void RemoveElementFromPage(List<UIElement> blocks)
        {
            foreach (UIElement block in blocks)
            {
                if ((block as FrameworkElement).Parent != null)
                    ((block as FrameworkElement).Parent as Canvas).Children.Remove(block);
            }
        }

        public void Remove(LineInfo line, PageAdv page)
        {
            foreach (UIElement element in line.Elements)
            {
                if (page.ForegroundContainer.Children.Contains(element))
                {
                    page.ForegroundContainer.Children.Remove(element);
                }
            }

            //if (!line.IsTableLine)
            //{
            foreach (TextBlock renderer in line.TextRenderers)
            {
                if (page.ForegroundContainer.Children.Contains(renderer))
                {
                    page.ForegroundContainer.Children.Remove(renderer);
                }
                else
                {
                    if (renderer.Parent != null)
                    {
                        (renderer.Parent as Canvas).Children.Remove(renderer);
                    }
                }
            }

            foreach (UIElement renderer in line.DecoratingElements)
            {
                if (page.DecorationContainer.Children.Contains(renderer))
                {
                    page.DecorationContainer.Children.Remove(renderer);
                }
            }
            if (line.RenderingOption==RenderingOptions.Render)
            {
                line.TextRenderers.Clear();
                line.DecoratingElements.Clear();
            }
            //}
            if (line.IsTableLine)
            {
                foreach (ElementBox box in line.ElementBoxes)
                {
                    if (box is ChildTableCellElementBox)
                    {
                        foreach (LineInfo ln in (box as ChildTableCellElementBox).LineInfos)
                        {
                            Remove(ln, page);
                        }
                    }
                    else if (box is TableCellElementBox)
                    {
                        TableCellElementBox b = box as TableCellElementBox;
                        int i = 0;
                        while (i < b.LineInfos.Count)
                        {
                            LineInfo l = b.LineInfos[i];
                            if (l.HasChildBoxes)
                            {
                                b.LineInfos.Remove(l);
                                continue;
                            }
                            Remove(l, page);
                            i++;
                        }
                    }
                }
            }
        }

        private TextBlock CreateBlock(TextElementBox elementBox, string str)
        {
            TextBlock block = new TextBlock();
            block.Foreground = new SolidColorBrush(elementBox.Foreground);
            block.FontWeight = elementBox.FontWeight;
            block.FontStyle = elementBox.FontStyle;
            block.FontFamily = elementBox.FontFamily;
            block.FontSize = elementBox.FontSize;
            if (elementBox.Baseline != Baseline.Normal)
                block.FontSize = elementBox.BaselineFontSize;
            block.Text = str;

            return block;
        }
        internal Path CreatePath(Rect rect, Color color)
        {
            Path path = new Path();
            PathGeometry pathGeo = new PathGeometry();
            PathFigure pathFig = new PathFigure();
            LineSegment lineSeg = new LineSegment();
            lineSeg.Point = new Point(rect.X - 1, rect.Bottom);
            pathFig.StartPoint = new Point(rect.X, rect.Bottom);
            pathFig.Segments.Add(lineSeg);
            LineSegment lineSeg1 = new LineSegment();
            lineSeg1.Point = new Point(rect.X - 1, rect.Top);
            pathFig.Segments.Add(lineSeg1);
            LineSegment lineSeg2 = new LineSegment();
            lineSeg2.Point = new Point(rect.Right, rect.Y);
            pathFig.Segments.Add(lineSeg2);
            LineSegment lineSeg3 = new LineSegment();
            lineSeg3.Point = new Point(rect.Right, rect.Bottom);
            pathFig.Segments.Add(lineSeg3);
            pathFig.IsClosed = true;
            pathGeo.Figures.Add(pathFig);
            path.Data = pathGeo;
            path.Fill = new SolidColorBrush(color);
            return path;
        }

        private Line CreateLine(Rect rect, Color color)
        {
            Line line = new Line();
            line.X1 = rect.X;
            line.X2 = rect.X + rect.Width;
            line.Y1 = rect.Y;
            line.Y2 = rect.Y;
            line.Stroke = new SolidColorBrush(color);
            return line;
        }

        private Line CreateLine(double x1, double x2, double y1, double y2, Color color)
        {
            Line line = new Line();
            line.X1 = x1;
            line.X2 = x2;
            line.Y1 = y1;
            line.Y2 = y2;
            line.Stroke = new SolidColorBrush(color);
            return line;
        }

        private void SetPosition(Point point, UIElement element)
        {
            Canvas.SetLeft(element, point.X);
            Canvas.SetTop(element, point.Y);
        }
    }
}
