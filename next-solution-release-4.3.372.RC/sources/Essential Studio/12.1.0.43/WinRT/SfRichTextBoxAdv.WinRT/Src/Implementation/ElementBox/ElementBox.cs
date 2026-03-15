#region Copyright Syncfusion Inc. 2001 - 2014
// Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Any infringement will be prosecuted under
// applicable laws. 
#endregion
using System;
using System.Net;
using System.Windows;
using System.Linq;
using System.Windows.Input;
using System.Collections.Generic;
using System.Text;
#if WPF
using System.Windows.Controls;
using System.Windows.Shapes;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using FontStyleEnum = System.Windows.FontStyles;
#else
using Windows.Foundation;
using Windows.UI.Xaml;
using Windows.UI;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Shapes;
using Windows.UI.Xaml.Media;
using Windows.UI.Text;
using FontStyleEnum = Windows.UI.Text.FontStyle;
using Windows.UI.Xaml.Media.Imaging;
#endif

#if WPF
namespace Syncfusion.Windows.Tools.RichTextBoxAdv
#else
namespace Syncfusion.UI.Xaml.RichTextBoxAdv
#endif
{
    internal interface IWidget
    {
        #region Properties
        #endregion

        #region Implementations
        #endregion
    }
    internal abstract class Widget : IWidget
    {
        #region Fields
        internal Point Location;
        internal double Width = 0, Height = 0;
        internal List<IWidget> ChildWidgets;
        internal Widget ContainerWidget;
        #endregion

        #region Properties
        #endregion

        #region Constructor
        /// <summary>
        /// Initializes a new instance of the <see cref="Widget"/> class.
        /// </summary>
        internal Widget()
        {
            Location = new Point();
            ChildWidgets = new List<IWidget>();
        }
        #endregion

        #region Implementations
        /// <summary>
        /// Gets the line widget.
        /// </summary>
        /// <param name="point">The point.</param>
        /// <returns></returns>
        internal abstract LineWidget GetLineWidget(Point point);
        /// <summary>
        /// Removes the widget.
        /// </summary>
        internal abstract void RemoveWidget();
        /// <summary>
        /// Gets the page.
        /// </summary>
        /// <returns></returns>
        internal PageAdv GetPage()
        {
            PageAdv page = null;
            if (ContainerWidget is BodyWidget)
                page = (ContainerWidget as BodyWidget).Page;
            else if (ContainerWidget != null)
                page = ContainerWidget.GetPage();
            return page;
        }
        /// <summary>
        /// Gets the cell widget.
        /// </summary>
        /// <returns></returns>
        internal TableCellWidget GetCellWidget()
        {
            TableCellWidget cellWidget = null;
            if (ContainerWidget is TableCellWidget)
                cellWidget = ContainerWidget as TableCellWidget;
            else
                cellWidget = ContainerWidget.GetCellWidget();
            return cellWidget;
        }
        /// <summary>
        /// Updates the widget location.
        /// </summary>
        /// <param name="area">The area.</param>
        public void UpdateWidgetLocation(Rect area)
        {
            Location = new Point(area.X, area.Y);
            Width = area.Width;
        }
        /// <summary>
        /// Updates the widget location.
        /// </summary>
        /// <param name="widget">The widget.</param>
        public void UpdateWidgetLocation(Widget widget)
        {
            Location = widget.Location;
            Width = widget.Width;
        }
        /// <summary>
        /// Clears the child selection highlight.
        /// </summary>
        internal void ClearChildSelectionHighlight()
        {
            for (int i = 0; i < ChildWidgets.Count; i++)
            {
                if (ChildWidgets[i] is LineWidget)
                    (ChildWidgets[i] as LineWidget).ClearSelectionHighlight();
                else if (ChildWidgets[i] is TableCellWidget)
                    (ChildWidgets[i] as TableCellWidget).ClearSelectionHighlight();
                else if (ChildWidgets[i] is Widget)
                    (ChildWidgets[i] as Widget).ClearChildSelectionHighlight();
            }
        }
        /// <summary>
        /// Updates the container widget.
        /// </summary>
        /// <param name="cellWidget">The cell widget.</param>
        internal void UpdateContainerWidget(TableCellWidget cellWidget)
        {
            if (ContainerWidget != null)
            {
                ContainerWidget.ChildWidgets.Remove(this);
                ContainerWidget.Height -= Height;
                if (ContainerWidget.ChildWidgets.Count == 0
                    && ContainerWidget is TableCellWidget)
                    (ContainerWidget as TableCellWidget).Dispose();
            }
            cellWidget.ChildWidgets.Add(this);
            ContainerWidget = cellWidget;
            double top = cellWidget.Location.Y;
            if (cellWidget.ChildWidgets.Count > 1)
            {
                Widget prevWidget = cellWidget.ChildWidgets[cellWidget.ChildWidgets.Count - 2] as Widget;
                Location = new Point(Location.X, prevWidget.Location.Y + prevWidget.Height);
                cellWidget.Height = Location.Y + Height - top;
                top = Location.Y;
            }
            else
            {
                Location = new Point(Location.X, top);
                cellWidget.Height = Location.Y + Height - top;
            }
            if (this is TableWidget)
                (this as TableWidget).UpdateChildLocation(top);
        }
        /// <summary>
        /// Updates the container widget.
        /// </summary>
        /// <param name="bodyWidget">The body widget.</param>
        /// <param name="index">The index.</param>
        internal void UpdateContainerWidget(BodyWidget bodyWidget, int index)
        {
            if (ContainerWidget != null)
            {
                ContainerWidget.ChildWidgets.Remove(this);
                ContainerWidget.Height -= Height;
                if (ContainerWidget.ChildWidgets.Count == 0
                    && ContainerWidget is BodyWidget)
                    (ContainerWidget as BodyWidget).Dispose();
            }
            bodyWidget.ChildWidgets.Insert(index, this);
            bodyWidget.Height += Height;
            ContainerWidget = bodyWidget;
        }
        #endregion
    }
    internal sealed class ParagraphWidget : Widget
    {
        #region Fields
        internal ParagraphAdv Paragraph;
        #endregion

        #region Properties
        #endregion

        #region Constructor
        /// <summary>
        /// Initializes a new instance of the <see cref="ParagraphWidget"/> class.
        /// </summary>
        /// <param name="paragraphAdv">The paragraph adv.</param>
        internal ParagraphWidget(ParagraphAdv paragraphAdv)
        {
            Paragraph = paragraphAdv;
        }
        #endregion

        #region Implementations
        /// <summary>
        /// Shifts the childs to widget.
        /// </summary>
        /// <param name="destinationWidget">The destination widget.</param>
        internal void ShiftChildsToWidget(ParagraphWidget destinationWidget)
        {
            for (int i = 0; i < ChildWidgets.Count; i++)
            {
                LineWidget line = ChildWidgets[i] as LineWidget;
                //Moves the line widget to destination widget.
                line.UpdateParagraphWidget(destinationWidget, destinationWidget.ChildWidgets.Count);
                i--;
            }
        }
        /// <summary>
        /// Determines whether this widget fit in client area.
        /// </summary>
        /// <param name="viewer">The viewer.</param>
        /// <returns>
        ///   <c>true</c> if this widget fit in client area; otherwise, <c>false</c>.
        /// </returns>
        internal bool IsFitInClientArea(LayoutViewer viewer)
        {
            LineWidget lastLine = ChildWidgets[ChildWidgets.Count - 1] as LineWidget;
            double height = Height;
            double maxElementHeight = lastLine.GetMaxElementHeight();
            if (lastLine.Height > maxElementHeight)
                height -= lastLine.Height - maxElementHeight;
            return viewer.ClientActiveArea.Height >= height;
        }
        /// <summary>
        /// Shifts to previous widget.
        /// </summary>
        /// <param name="viewer">The viewer.</param>
        /// <param name="prevWidget">The prev widget.</param>
        internal void ShiftToPreviousWidget(LayoutViewer viewer, ParagraphWidget prevWidget)
        {
            for (int i = 0; i < ChildWidgets.Count; i++)
            {
                LineWidget line = ChildWidgets[i] as LineWidget;
                double maxElementHeight = line.GetMaxElementHeight();
                if (viewer.ClientActiveArea.Height >= maxElementHeight)
                {
                    //Moves the line widget to previous widget.
                    line.UpdateParagraphWidget(prevWidget, prevWidget.ChildWidgets.Count);
                    i--;
                    viewer.CutFromTop(viewer.ClientActiveArea.Y + line.Height);
                }
                else
                {
                    BodyWidget bodyWidget = prevWidget.ContainerWidget as BodyWidget;
                    BodyWidget nextBodyWidget = bodyWidget.CreateOrGetNextBodyWiget(viewer);
                    if (ContainerWidget != nextBodyWidget)
                        UpdateContainerWidget(nextBodyWidget, 0);
                    //Updates client area based on next page.
                    viewer.UpdateClientArea(nextBodyWidget.Section.SectionFormat);
                    break;
                }
            }
        }
        /// <summary>
        /// Splits the widget.
        /// </summary>
        /// <param name="viewer">The viewer.</param>
        /// <param name="prevBodyWidget">The prev body widget.</param>
        /// <param name="index">The index.</param>
        /// <returns></returns>
        internal bool SplitWidget(LayoutViewer viewer, BodyWidget prevBodyWidget, int index)
        {
            LineWidget firstLine = ChildWidgets[0] as LineWidget;
            double maxElementHeight = firstLine.GetMaxElementHeight();
            BodyWidget nextBodyWidget = ContainerWidget as BodyWidget;
            if (viewer.ClientActiveArea.Height >= maxElementHeight)
            {
                ParagraphWidget splittedWidget = null;
                int widgetIndex = Paragraph.ParagraphWidgets.IndexOf(this);
                if (widgetIndex < Paragraph.ParagraphWidgets.Count - 1)
                {
                    splittedWidget = Paragraph.ParagraphWidgets[widgetIndex + 1] as ParagraphWidget;
                    nextBodyWidget = splittedWidget.ContainerWidget as BodyWidget;
                }
                else
                {
                    splittedWidget = new ParagraphWidget(Paragraph);
                    splittedWidget.Width = Width;
                    splittedWidget.Location = new Point(Location.X, Location.Y);
                    Paragraph.ParagraphWidgets.Add(splittedWidget);
                }
                if (prevBodyWidget != ContainerWidget)
                    UpdateContainerWidget(prevBodyWidget, index);
                for (int i = ChildWidgets.Count - 1; i > 0; i--)
                {
                    if (IsFitInClientArea(viewer))
                        break;
                    else
                    {
                        LineWidget line = ChildWidgets[i] as LineWidget;
                        //Moves the line widget to next widget.
                        line.UpdateParagraphWidget(splittedWidget, 0);
                    }
                }
                if (splittedWidget.ContainerWidget == null)
                {
                    double y = viewer.ClientActiveArea.Y;
                    //Checks whether next node exists, else adds new page.
                    nextBodyWidget = prevBodyWidget.CreateOrGetNextBodyWiget(viewer);
                    nextBodyWidget.ChildWidgets.Insert(0, splittedWidget);
                    nextBodyWidget.Height += splittedWidget.Height;
                    splittedWidget.ContainerWidget = nextBodyWidget;
                    if (nextBodyWidget.ChildWidgets.Count == 1)
                    {
                        Location = new Point(Location.X, y);
                        return true;
                    }
                }
            }
            else
            {
                nextBodyWidget = prevBodyWidget.CreateOrGetNextBodyWiget(viewer);
                if (ContainerWidget != nextBodyWidget)
                    UpdateContainerWidget(nextBodyWidget, 0);
            }
            if (prevBodyWidget == ContainerWidget)
            {
                Location = new Point(Location.X, viewer.ClientActiveArea.Y);
                viewer.CutFromTop(viewer.ClientActiveArea.Y + Height);
            }
            else
                //Updates client area based on next body widget.
                viewer.UpdateClientArea(nextBodyWidget.Section.SectionFormat);
            return false;
        }
        /// <summary>
        /// Highlights the specified selection.
        /// </summary>
        /// <param name="selection">The selection.</param>
        /// <param name="startIndex">The start index.</param>
        /// <param name="endLine">The end line.</param>
        /// <param name="endElement">The end element.</param>
        /// <param name="endIndex">The end index.</param>
        internal void Highlight(SelectionAdv selection, int startIndex, LineWidget endLine, ElementBox endElement, int endIndex)
        {
            double top = 0;
            for (int i = startIndex; i < ChildWidgets.Count; i++)
            {
                LineWidget line = ChildWidgets[i] as LineWidget;
                if (i == startIndex)
                    top = line.GetTop();
                double left = line.GetLeft();
                if (line == endLine)
                {
                    //Selection ends in current line.
                    double right = endLine.GetLeft(endElement, endIndex);
                    selection.CreateHighlightBorder(line, right - left, left, top);
                    return;
                }
                selection.CreateHighlightBorder(line, line.GetWidth(true) - (left - Location.X), left, top);
                top += line.Height;
            }
        }
        /// <summary>
        /// Gets the line widget.
        /// </summary>
        /// <param name="point">The point.</param>
        /// <returns></returns>
        internal override LineWidget GetLineWidget(Point point)
        {
            double top = Location.Y;
            for (int i = 0; i < ChildWidgets.Count; i++)
            {
                if (top <= point.Y
                    && (top + (ChildWidgets[i] as LineWidget).Height) >= point.Y)
                {
                    return ChildWidgets[i] as LineWidget;
                }
                top += (ChildWidgets[i] as LineWidget).Height;
            }
            LineWidget lineWidget = null;
            if (ChildWidgets.Count > 0)
            {
                if (Location.Y <= point.Y)
                    lineWidget = ChildWidgets[ChildWidgets.Count - 1] as LineWidget;
                else
                    lineWidget = ChildWidgets[0] as LineWidget;
            }
            return lineWidget;
        }
        /// <summary>
        /// Removes the widget.
        /// </summary>
        internal override void RemoveWidget()
        {
            for (int i = 0; i < ChildWidgets.Count; i++)
            {
                (ChildWidgets[i] as LineWidget).RemoveWidget();
            }
        }
        /// <summary>
        /// Renders the specified page.
        /// </summary>
        /// <param name="page">The page.</param>
        internal void Render(PageAdv page)
        {
            double top = Location.Y;
            for (int i = 0; i < ChildWidgets.Count; i++)
            {
                (ChildWidgets[i] as LineWidget).Render(page, Location.X, top);
                top += (ChildWidgets[i] as LineWidget).Height;
            }
        }
        /// <summary>
        /// Renders the specified page.
        /// </summary>
        /// <param name="viewer">The viewer.</param>
        internal void Render(FlowLayoutViewer viewer)
        {
            double top = Location.Y;
            if (viewer.OwnerControl.LayoutType == LayoutType.Continuous
                && (viewer.Visiblebounds.Top > (top + Height) * viewer.ScaleFactor
                || viewer.Visiblebounds.Bottom < top * viewer.ScaleFactor))
                return;
            PageAdv page = viewer.CurrentPage;
            for (int i = 0; i < ChildWidgets.Count; i++)
            {
                if (viewer.OwnerControl.LayoutType == LayoutType.Block
                    || !(viewer.Visiblebounds.Top > (top + (ChildWidgets[i] as LineWidget).Height) * viewer.ScaleFactor
                    || viewer.Visiblebounds.Bottom < top * viewer.ScaleFactor))
                    (ChildWidgets[i] as LineWidget).Render(page, Location.X, top);
                top += (ChildWidgets[i] as LineWidget).Height;
            }
        }
        /// <summary>
        /// Determines whether the first line fit in the specified bottom.
        /// </summary>
        /// <param name="bottom">The bottom.</param>
        /// <returns>
        ///   <c>true</c> if the first line fit in the specified bottom; otherwise, <c>false</c>.
        /// </returns>
        internal bool IsFirstLineFit(double bottom)
        {
            bool isFirstLineFit = false;
            LineWidget lineWidget = ChildWidgets[0] as LineWidget;
            isFirstLineFit = (Location.Y + lineWidget.Height <= bottom);
            return isFirstLineFit;
        }
        /// <summary>
        /// Gets the splitted widget.
        /// </summary>
        /// <param name="bottom">The bottom.</param>
        /// <returns></returns>
        internal ParagraphWidget GetSplittedWidget(double bottom)
        {
            double lineBottom = Location.Y;
            ParagraphWidget splittedWidget = null;
            for (int i = 0; i < ChildWidgets.Count; i++)
            {
                LineWidget lineWidget = ChildWidgets[i] as LineWidget;
                if (bottom < lineBottom + lineWidget.Height)
                {
                    if (i == 0)
                    {
                        splittedWidget = this;
                        break;
                    }
                    if (ChildWidgets.Contains(lineWidget))
                    {
                        ChildWidgets.Remove(lineWidget);
                        i--;
                    }
                    Height -= lineWidget.Height;
                    if (splittedWidget == null)
                    {
                        //Creates new widget, to hold the splitted contents.
                        splittedWidget = new ParagraphWidget(Paragraph);
                        splittedWidget.UpdateWidgetLocation(this);
                        Paragraph.ParagraphWidgets.Add(splittedWidget);
                        splittedWidget.Height = lineWidget.Height;
                    }
                    else
                        splittedWidget.Height += lineWidget.Height;
                    splittedWidget.ChildWidgets.Add(lineWidget);
                    lineWidget.ParagraphWidget = splittedWidget;
                }
                lineBottom += lineWidget.Height;
            }
            return splittedWidget;
        }
        /// <summary>
        /// Releases unmanaged and - optionally - managed resources.
        /// </summary>
        internal void Dispose()
        {
            if (ContainerWidget != null)
            {
                ContainerWidget.ChildWidgets.Remove(this);
                ContainerWidget.Height -= Height;
                if (ContainerWidget.ChildWidgets.Count == 0
                    && ContainerWidget is BodyWidget)
                    (ContainerWidget as BodyWidget).Dispose();
                ContainerWidget = null;
            }
            for (int i = 0; i < ChildWidgets.Count; i++)
            {
                LineWidget widget = ChildWidgets[i] as LineWidget;
                widget.Dispose();
                ChildWidgets.Remove(widget);
                i--;
            }
            if (Paragraph != null)
            {
                if (Paragraph.ParagraphWidgets != null)
                    Paragraph.ParagraphWidgets.Remove(this);
                Paragraph = null;
            }
        }
        #endregion
    }
    internal sealed class HeaderFooterWidget : BodyWidget
    {
        #region Fields
        internal HeaderFooter HeaderFooter;
        #endregion

        #region Properties
        #endregion
        
        #region Constructor
        /// <summary>
        /// Initializes a new instance of the <see cref="HeaderFooterWidget"/> class.
        /// </summary>
        /// <param name="ownerHeaderFooter">The owner header footer.</param>
        /// <param name="section">The section.</param>
        internal HeaderFooterWidget(HeaderFooter ownerHeaderFooter, SectionAdv section)
            : base(section)
        {
            HeaderFooter = ownerHeaderFooter;
        }
        #endregion

        #region Implementations
        #endregion
    }

    internal class BodyWidget : Widget
    {
        #region Fields
        internal SectionAdv Section;
        internal PageAdv Page;
        #endregion

        #region Properties
        #endregion

        #region Constructor
        /// <summary>
        /// Initializes a new instance of the <see cref="BodyWidget"/> class.
        /// </summary>
        /// <param name="section">The section.</param>
        internal BodyWidget(SectionAdv section)
        {
            Section = section;
        }
        #endregion

        #region Implementations
        /// <summary>
        /// Creates the or get next body wiget.
        /// </summary>
        /// <param name="viewer">The viewer.</param>
        /// <returns></returns>
        internal BodyWidget CreateOrGetNextBodyWiget(LayoutViewer viewer)
        {
            BodyWidget nextBodyWidget = null;
            //Checks whether next node exists, else adds new page.
            int pageIndex = 0;
                pageIndex = viewer.Pages.IndexOf(Page);
            PageAdv page = null;
            if (pageIndex == viewer.Pages.Count - 1
                || viewer.Pages[pageIndex + 1].Section != Section)
            {
                page = viewer.CreateNewPage(Section);
                if (viewer.Pages[pageIndex + 1].Section != Section)
                    viewer.InsertPage(pageIndex + 1, page);
                nextBodyWidget = page.BodyWidgets[0];
            }
            else
            {
                page = viewer.Pages[pageIndex + 1];
                nextBodyWidget = page.BodyWidgets[0] as BodyWidget;
            }
            return nextBodyWidget;
        }
        /// <summary>
        /// Gets the line widget.
        /// </summary>
        /// <param name="point">The point.</param>
        /// <returns></returns>
        internal override LineWidget GetLineWidget(Point point)
        {
            for (int i = 0; i < ChildWidgets.Count; i++)
            {
                if ((ChildWidgets[i] as Widget).Location.Y <= point.Y
                    && ((ChildWidgets[i] as Widget).Location.Y + (ChildWidgets[i] as Widget).Height) >= point.Y)
                    return (ChildWidgets[i] as Widget).GetLineWidget(point);
            }
            LineWidget lineWidget = null;
            if (ChildWidgets.Count > 0)
            {
                if ((ChildWidgets[0] as Widget).Location.Y <= point.Y)
                    lineWidget = (ChildWidgets[ChildWidgets.Count - 1] as Widget).GetLineWidget(point);
                else
                    lineWidget = (ChildWidgets[0] as Widget).GetLineWidget(point);
            }
            return lineWidget;
        }
        /// <summary>
        /// Removes the widget.
        /// </summary>
        internal override void RemoveWidget()
        {
            for (int i = 0; i < ChildWidgets.Count; i++)
            {
                (ChildWidgets[i] as Widget).RemoveWidget();
            }
        }
        /// <summary>
        /// Renders the specified page.
        /// </summary>
        /// <param name="viewer">The viewer.</param>
        internal void Render(FlowLayoutViewer viewer)
        {
            for (int i = 0; i < ChildWidgets.Count; i++)
            {
                if (ChildWidgets[i] is ParagraphWidget)
                    (ChildWidgets[i] as ParagraphWidget).Render(viewer);
                else
                    (ChildWidgets[i] as TableWidget).Render(viewer);
            }
        }
        /// <summary>
        /// Renders the specified page.
        /// </summary>
        /// <param name="page">The page.</param>
        internal void Render(PageAdv page)
        {
            for (int i = 0; i < ChildWidgets.Count; i++)
            {
                if (ChildWidgets[i] is ParagraphWidget)
                    (ChildWidgets[i] as ParagraphWidget).Render(page);
                else
                    (ChildWidgets[i] as TableWidget).Render(page);
            }
        }
        /// <summary>
        /// Shifts the child location.
        /// </summary>
        /// <param name="shiftTop">The shift top.</param>
        internal void ShiftChildLocation(double shiftTop)
        {
            Location = new Point(Location.X, Location.Y + shiftTop);
            for (int i = 0; i < ChildWidgets.Count; i++)
            {
                if (ChildWidgets[i] is ParagraphWidget)
                    (ChildWidgets[i] as Widget).Location = new Point((ChildWidgets[i] as Widget).Location.X, (ChildWidgets[i] as Widget).Location.Y + shiftTop);
                else
                    (ChildWidgets[i] as TableWidget).ShiftChildLocation(shiftTop);
            }
        }
        /// <summary>
        /// Releases unmanaged and - optionally - managed resources.
        /// </summary>
        internal void Dispose()
        {
            if (ContainerWidget != null)
            {
                ContainerWidget.ChildWidgets.Remove(this);
                ContainerWidget.Height -= Height;
                ContainerWidget = null;
            }
            for (int i = 0; i < ChildWidgets.Count; i++)
            {
                Widget widget = ChildWidgets[i] as Widget;
                if (widget is ParagraphWidget)
                    (widget as ParagraphWidget).Dispose();
                else
                    (widget as TableWidget).Dispose();
                ChildWidgets.Remove(widget);
                i--;
            }
            if (Section != null)
            {
                if (Section.BodyWidgets != null)
                    Section.BodyWidgets.Remove(this);
                Section = null;
            }
            if (this is HeaderFooterWidget && (this as HeaderFooterWidget).HeaderFooter != null)
            {
                if ((this as HeaderFooterWidget).HeaderFooter.LayoutedWidgets != null)
                    (this as HeaderFooterWidget).HeaderFooter.LayoutedWidgets.Remove(this as HeaderFooterWidget);
                (this as HeaderFooterWidget).HeaderFooter = null;
            }
            if (Page != null)
            {
                if (Page.BodyWidgets.Contains(this))
                {
                    Page.BodyWidgets.Remove(this);
                    if (Page.BodyWidgets.Count == 0)
                        Page.Dispose();
                }
                else if (Page.HeaderWidget == this)
                    Page.HeaderWidget = null;
                else if (Page.FooterWidget == this)
                    Page.FooterWidget = null;
                Page = null;
            }
        }
        #endregion
    }
    internal sealed class TableWidget : Widget
    {
        #region Fields
        internal TableAdv Table;
        internal Thickness Margin;
        #endregion

        #region Properties
        #endregion

        #region Constructor
        /// <summary>
        /// Initializes a new instance of the <see cref="TableWidget"/> class.
        /// </summary>
        /// <param name="tableAdv">The table adv.</param>
        internal TableWidget(TableAdv tableAdv)
        {
            Table = tableAdv;
            Margin = new Thickness();
        }
        #endregion

        #region Implementations
        /// <summary>
        /// Gets the line widget.
        /// </summary>
        /// <param name="point">The point.</param>
        /// <returns></returns>
        internal override LineWidget GetLineWidget(Point point)
        {
            LineWidget lineWidget = null;
            for (int i = 0; i < ChildWidgets.Count; i++)
            {
                if ((ChildWidgets[i] as TableRowWidget).Location.Y <= point.Y
                    && ((ChildWidgets[i] as TableRowWidget).Location.Y + (ChildWidgets[i] as TableRowWidget).Height) >= point.Y)
                {
                    lineWidget = (ChildWidgets[i] as TableRowWidget).GetLineWidget(point);
                    break;
                }
            }
            return lineWidget;
        }
        /// <summary>
        /// Removes the widget.
        /// </summary>
        internal override void RemoveWidget()
        {
            for (int i = 0; i < ChildWidgets.Count; i++)
            {
                (ChildWidgets[i] as Widget).RemoveWidget();
            }
        }
        /// <summary>
        /// Updates the height.
        /// </summary>
        /// <param name="viewer">The viewer.</param>
        internal void UpdateHeight(LayoutViewer viewer)
        {
            for (int i = 0; i < ChildWidgets.Count; i++)
            {
                TableRowWidget rowWidget = ChildWidgets[i] as TableRowWidget;
                rowWidget.UpdateHeight(viewer);
            }
        }
        /// <summary>
        /// Renders the specified page.
        /// </summary>
        /// <param name="viewer">The viewer.</param>
        internal void Render(FlowLayoutViewer viewer)
        {
            if (viewer.OwnerControl.LayoutType == LayoutType.Continuous
                && (viewer.Visiblebounds.Top > (Location.Y + Height) * viewer.ScaleFactor
                || viewer.Visiblebounds.Bottom < Location.Y * viewer.ScaleFactor))
                return;
            for (int i = 0; i < ChildWidgets.Count; i++)
            {
                TableRowWidget rowWidget = ChildWidgets[i] as TableRowWidget;
                //Renders table cell outline rectangle - Border and background color.
                for (int j = 0; j < rowWidget.ChildWidgets.Count; j++)
                {
                    (rowWidget.ChildWidgets[j] as TableCellWidget).Render(viewer);
                }
            }
        }
        /// <summary>
        /// Renders the specified page.
        /// </summary>
        /// <param name="page">The page.</param>
        internal void Render(PageAdv page)
        {
            for (int i = 0; i < ChildWidgets.Count; i++)
            {
                TableRowWidget rowWidget = ChildWidgets[i] as TableRowWidget;
                //Renders table cell outline rectangle - Border and background color.
                for (int j = 0; j < rowWidget.ChildWidgets.Count; j++)
                {
                    (rowWidget.ChildWidgets[j] as TableCellWidget).Render(page);
                }
            }
        }
        /// <summary>
        /// Gets the splitted widget.
        /// </summary>
        /// <param name="bottom">The bottom.</param>
        /// <returns></returns>
        internal TableWidget GetSplittedWidget(double bottom)
        {
            double rowBottom = Location.Y;
            TableWidget splittedWidget = null;
            for (int i = 0; i < ChildWidgets.Count; i++)
            {
                TableRowWidget rowWidget = ChildWidgets[i] as TableRowWidget;
                double rowHeight = rowWidget.Height;
                if (bottom < rowBottom + rowHeight || splittedWidget != null)
                {
                    TableRowWidget splittedRow = (splittedWidget == null) ? rowWidget.GetSplittedWidget(bottom) : rowWidget;
                    if (splittedRow != null)
                    {
                        if (i == 0 && splittedRow == rowWidget)
                            //Returns if the whole table does not fit in current page.
                            return this;
                        if (ChildWidgets.Contains(splittedRow))
                        {
                            ChildWidgets.Remove(splittedRow);
                            i--;
                            Height -= splittedRow.Height;
                        }
                        else
                            Height -= rowHeight - rowWidget.Height;
                        if (splittedWidget == null)
                        {
                            //Creates new widget, to hold the splitted contents.
                            splittedWidget = new TableWidget(Table);
                            splittedWidget.UpdateWidgetLocation(this);
                            Table.TableWidgets.Add(splittedWidget);
                            splittedWidget.Height = splittedRow.Height;
                        }
                        else
                            splittedWidget.Height += splittedRow.Height;
                        splittedWidget.ChildWidgets.Add(splittedRow);
                        splittedRow.ContainerWidget = splittedWidget;
                    }
                }
                rowBottom += rowWidget.Height;
            }
            return splittedWidget;
        }
        /// <summary>
        /// Updates the child location.
        /// </summary>
        /// <param name="top">The top.</param>
        internal void UpdateChildLocation(double top)
        {
            for (int i = 0; i < ChildWidgets.Count; i++)
            {
                TableRowWidget rowWidget = ChildWidgets[i] as TableRowWidget;
                rowWidget.Location = new Point(rowWidget.Location.X, top);
                rowWidget.UpdateChildLocation(top);
                top += rowWidget.Height;
            }
        }
        /// <summary>
        /// Shifts the child location.
        /// </summary>
        /// <param name="shiftTop">The shift top.</param>
        internal void ShiftChildLocation(double shiftTop)
        {
            Location = new Point(Location.X, Location.Y + shiftTop);
            for (int i = 0; i < ChildWidgets.Count; i++)
            {
                (ChildWidgets[i] as TableRowWidget).ShiftChildLocation(shiftTop);
            }
        }
        /// <summary>
        /// Determines whether the first line fit in the specified bottom.
        /// </summary>
        /// <param name="bottom">The bottom.</param>
        /// <returns>
        ///   <c>true</c> if the first line fit in the specified bottom; otherwise, <c>false</c>.
        /// </returns>
        internal bool IsFirstLineFit(double bottom)
        {
            return (ChildWidgets[0] as TableRowWidget).IsFirstLineFit(bottom);
        }
        /// <summary>
        /// Releases unmanaged and - optionally - managed resources.
        /// </summary>
        internal void Dispose()
        {
            if (ContainerWidget != null)
            {
                ContainerWidget.ChildWidgets.Remove(this);
                ContainerWidget.Height -= Height;
                if (ContainerWidget.ChildWidgets.Count == 0
                    && ContainerWidget is BodyWidget)
                    (ContainerWidget as BodyWidget).Dispose();
                ContainerWidget = null;
            }
            for (int i = 0; i < ChildWidgets.Count; i++)
            {
                TableRowWidget widget = ChildWidgets[i] as TableRowWidget;
                widget.Dispose();
                ChildWidgets.Remove(widget);
                i--;
            }
            if (Table != null)
            {
                if (Table.TableWidgets != null)
                    Table.TableWidgets.Remove(this);
                Table = null;
            }
        }
        #endregion
    }
    internal sealed class TableRowWidget : Widget
    {
        #region Fields
        internal TableRowAdv TableRow;
        #endregion

        #region Properties
        #endregion

        #region Constructor
        /// <summary>
        /// Initializes a new instance of the <see cref="TableRowWidget"/> class.
        /// </summary>
        /// <param name="tableRowAdv">The table row adv.</param>
        internal TableRowWidget(TableRowAdv tableRowAdv)
        {
            TableRow = tableRowAdv;
        }
        #endregion

        #region Implementations
        /// <summary>
        /// Updates the table widget.
        /// </summary>
        /// <param name="tableWidget">The table widget.</param>
        /// <param name="index">The index.</param>
        internal void UpdateTableWidget(TableWidget tableWidget, int index)
        {
            if (ContainerWidget != null)
            {
                ContainerWidget.ChildWidgets.Remove(this);
                if (ContainerWidget.ChildWidgets.Count == 0
                    && ContainerWidget is TableWidget)
                    (ContainerWidget as TableWidget).Dispose();
                else if (ContainerWidget.ContainerWidget is BodyWidget)
                    ContainerWidget.ContainerWidget.Height -= Height;
                ContainerWidget.Height -= Height;
            }
            tableWidget.ChildWidgets.Insert(index, this);
            tableWidget.Height += Height;
            ContainerWidget = tableWidget;
        }
        /// <summary>
        /// Gets the line widget.
        /// </summary>
        /// <param name="point">The point.</param>
        /// <returns></returns>
        internal override LineWidget GetLineWidget(Point point)
        {
            for (int i = 0; i < ChildWidgets.Count; i++)
            {
                if ((ChildWidgets[i] as TableCellWidget).Location.X <= point.X
                    && ((ChildWidgets[i] as TableCellWidget).Location.X + (ChildWidgets[i] as TableCellWidget).Width) >= point.X)
                    return (ChildWidgets[i] as TableCellWidget).GetLineWidget(point);
            }
            LineWidget lineWidget = null;
            if (ChildWidgets.Count > 0)
            {
                if ((ChildWidgets[0] as Widget).Location.X <= point.X)
                    lineWidget = (ChildWidgets[ChildWidgets.Count - 1] as Widget).GetLineWidget(point);
                else
                    lineWidget = (ChildWidgets[0] as Widget).GetLineWidget(point);
            }
            return lineWidget;
        }
        /// <summary>
        /// Updates the height.
        /// </summary>
        /// <param name="viewer">The viewer.</param>
        internal void UpdateHeight(LayoutViewer viewer)
        {
            for (int i = 0; i < ChildWidgets.Count; i++)
            {
                TableCellWidget cellWidget = ChildWidgets[i] as TableCellWidget;
                int rowSpan = 1;
#if WPF
                rowSpan = cellWidget.TableCell.CellFormat.RowSpan;
#else
                UIDispatcher.Execute(() => rowSpan = cellWidget.TableCell.CellFormat.RowSpan);
#endif
                if (rowSpan > 1)
                {
                    int spanEndIndex = ContainerWidget.ChildWidgets.IndexOf(this) + rowSpan - 1 - (TableRow.RowIndex - cellWidget.TableCell.OwnerRow.RowIndex);
                    if (viewer.ClientArea.Bottom < cellWidget.Location.Y + cellWidget.Height + cellWidget.Margin.Bottom
                        || spanEndIndex >= ContainerWidget.ChildWidgets.Count)
                        SplitSpannedCellWidget(cellWidget, viewer.ClientArea.Bottom);
                    TableRowWidget spanEndRowWidget = this;
                    if (spanEndIndex > 0)
                    {
                        if (spanEndIndex < ContainerWidget.ChildWidgets.Count)
                            spanEndRowWidget = ContainerWidget.ChildWidgets[spanEndIndex] as TableRowWidget;
                        else
                            spanEndRowWidget = ContainerWidget.ChildWidgets[ContainerWidget.ChildWidgets.Count - 1] as TableRowWidget;
                    }
                    if (cellWidget.Location.Y + cellWidget.Height + cellWidget.Margin.Bottom < spanEndRowWidget.Location.Y + spanEndRowWidget.Height)
                        cellWidget.Height = spanEndRowWidget.Location.Y + spanEndRowWidget.Height - cellWidget.Location.Y - cellWidget.Margin.Bottom;
                    else if (ContainerWidget.ContainerWidget != null && cellWidget.Location.Y + cellWidget.Height + cellWidget.Margin.Bottom > spanEndRowWidget.Location.Y + spanEndRowWidget.Height)
                        spanEndRowWidget.Height = cellWidget.Location.Y + cellWidget.Height + cellWidget.Margin.Bottom - spanEndRowWidget.Location.Y;
                }
                else
                    cellWidget.Height = Height - cellWidget.Margin.Top - cellWidget.Margin.Bottom;
                cellWidget.UpdateHeight(viewer);
                
                Widget widget = ContainerWidget;
                while (widget.ContainerWidget is Widget)
                {
                    widget = widget.ContainerWidget;
                }
                PageAdv page = null;
                if (widget is BodyWidget)
                    page = (widget as BodyWidget).Page;
                //Renders the current table row contents, after relayout based on editing.
                if (viewer is PageLayoutViewer && (viewer as PageLayoutViewer).VisiblePages.Contains(page))
                    cellWidget.Render(page);
                else if (viewer is FlowLayoutViewer && (viewer as FlowLayoutViewer).CurrentPage == page)
                    cellWidget.Render(viewer as FlowLayoutViewer);
            }
        }
        /// <summary>
        /// Splits the spanned cell widget.
        /// </summary>
        /// <param name="cellWidget">The cell widget.</param>
        /// <param name="bottom">The bottom.</param>
        private void SplitSpannedCellWidget(TableCellWidget cellWidget, double bottom)
        {
            TableCellWidget splittedCell = cellWidget.GetSplittedWidget(bottom, false);
            if (splittedCell != null)
            {
                TableRowWidget lastRow = ContainerWidget.ChildWidgets[ContainerWidget.ChildWidgets.Count - 1] as TableRowWidget;
                int lastRowIndex = lastRow.TableRow.TableRowWidgets.IndexOf(lastRow);
                TableRowWidget splittedRow;
                if (lastRowIndex < lastRow.TableRow.TableRowWidgets.Count - 1)
                    splittedRow = lastRow.TableRow.TableRowWidgets[lastRow.TableRow.TableRowWidgets.Count - 1];
                else
                    splittedRow = (lastRow.TableRow.NextNode as TableRowAdv).TableRowWidgets.Last();
                for (int i = 0; i < splittedRow.ChildWidgets.Count; i++)
                {
                    if ((splittedRow.ChildWidgets[i] as Widget).Location.X > splittedCell.Location.X)
                    {
                        splittedRow.ChildWidgets.Insert(i, splittedCell);
                        break;
                    }
                }
                if (splittedRow.Height < splittedCell.Height + splittedCell.Margin.Top + splittedCell.Margin.Bottom)
                    splittedRow.Height = splittedCell.Height + splittedCell.Margin.Top + splittedCell.Margin.Bottom;
            }
        }
        /// <summary>
        /// Removes the widget.
        /// </summary>
        internal override void RemoveWidget()
        {
            for (int i = 0; i < ChildWidgets.Count; i++)
            {
                (ChildWidgets[i] as Widget).RemoveWidget();
            }
        }
        /// <summary>
        /// Updates the child location.
        /// </summary>
        /// <param name="top">The top.</param>
        internal void UpdateChildLocation(double top)
        {
            for (int i = 0; i < ChildWidgets.Count; i++)
            {
                TableCellWidget cellWidget = ChildWidgets[i] as TableCellWidget;
                cellWidget.Location = new Point(cellWidget.Location.X, top + cellWidget.Margin.Top);
                cellWidget.UpdateChildLocation(cellWidget.Location.Y);
            }
        }
        /// <summary>
        /// Shifts the child location.
        /// </summary>
        /// <param name="shiftTop">The shift top.</param>
        internal void ShiftChildLocation(double shiftTop)
        {
            Location = new Point(Location.X, Location.Y + shiftTop);
            for (int i = 0; i < ChildWidgets.Count; i++)
            {
                (ChildWidgets[i] as TableCellWidget).ShiftChildLocation(shiftTop);
            }
        }
        /// <summary>
        /// Determines whether the first line fit in the specified bottom.
        /// </summary>
        /// <param name="bottom">The bottom.</param>
        /// <returns>
        ///   <c>true</c> if the first line fit in the specified bottom; otherwise, <c>false</c>.
        /// </returns>
        internal bool IsFirstLineFit(double bottom)
        {
            for (int i = 0; i < ChildWidgets.Count; i++)
            {
                TableCellWidget cellWidget = ChildWidgets[i] as TableCellWidget;
                if (!cellWidget.IsFirstLineFit(bottom))
                    return false;
            }
            return true;
        }
        /// <summary>
        /// Gets the splitted widget.
        /// </summary>
        /// <param name="bottom">The bottom.</param>
        /// <returns></returns>
        internal TableRowWidget GetSplittedWidget(double bottom)
        {
            TableRowWidget splittedWidget = null;
            for (int i = 0; i < ChildWidgets.Count; i++)
            {
                TableCellWidget cellWidget = ChildWidgets[i] as TableCellWidget;
                TableCellWidget splittedCell = cellWidget.GetSplittedWidget(bottom, true);
                if (splittedCell != null)
                {
                    if (splittedCell == cellWidget) 
                        //Returns if the whole content of the row does not fit in current page.
                        return this;
                    if (ChildWidgets.Contains(splittedCell))
                        ChildWidgets.Remove(splittedCell);
                    if (i == 0 || Height < cellWidget.Height + cellWidget.Margin.Top + cellWidget.Margin.Bottom)
                        Height = cellWidget.Height + cellWidget.Margin.Top + cellWidget.Margin.Bottom;
                    if (splittedWidget == null)
                    {
                        //Creates new widget, to hold the splitted contents.
                        splittedWidget = new TableRowWidget(TableRow);
                        splittedWidget.UpdateWidgetLocation(this);
                        TableRow.TableRowWidgets.Add(splittedWidget);
                        splittedWidget.Height = 0;
                    }
                    if (splittedWidget.Height < splittedCell.Height + splittedCell.Margin.Top + splittedCell.Margin.Bottom)
                        splittedWidget.Height = splittedCell.Height + splittedCell.Margin.Top + splittedCell.Margin.Bottom;
                    splittedWidget.ChildWidgets.Add(splittedCell);
                    splittedCell.ContainerWidget = splittedWidget;
                }
            }
            return splittedWidget;
        }
        /// <summary>
        /// Releases unmanaged and - optionally - managed resources.
        /// </summary>
        internal void Dispose()
        {
            if (ContainerWidget != null)
            {
                ContainerWidget.ChildWidgets.Remove(this);
                if (ContainerWidget.ChildWidgets.Count == 0
                    && ContainerWidget is TableWidget)
                    (ContainerWidget as TableWidget).Dispose();
                else if (ContainerWidget.ContainerWidget is BodyWidget)
                    ContainerWidget.ContainerWidget.Height -= Height;
                ContainerWidget.Height -= Height;
                ContainerWidget = null;
            }
            for (int i = 0; i < ChildWidgets.Count; i++)
            {
                TableCellWidget widget = ChildWidgets[i] as TableCellWidget;
                widget.Dispose();
                ChildWidgets.Remove(widget);
                i--;
            }
            if (TableRow != null)
            {
                if (TableRow.TableRowWidgets != null)
                    TableRow.TableRowWidgets.Remove(this);
                TableRow = null;
            }
        }
        #endregion
    }
    internal sealed class TableCellWidget : Widget
    {
        #region Fields
        internal TableCellAdv TableCell;
        internal Rectangle CellRectangle;
        internal Thickness Margin;
        internal Border SelectionHighlight = null;
        #endregion

        #region Properties
        #endregion

        #region Constructor
        /// <summary>
        /// Initializes a new instance of the <see cref="TableCellWidget"/> class.
        /// </summary>
        /// <param name="tableCellAdv">The table cell adv.</param>
        internal TableCellWidget(TableCellAdv tableCellAdv)
        {
            Margin = new Thickness();
#if !WPF
            UIDispatcher.Execute(() =>
                {
#endif
                    TableCell = tableCellAdv;
                    CellRectangle = new Rectangle();
                    if (TableCell.CellFormat.Background != Color.FromArgb(0, 0, 0, 0))
                        CellRectangle.Fill = new SolidColorBrush(TableCell.CellFormat.Background);
                    else if (TableCell.OwnerTable.TableFormat.Background != Color.FromArgb(0, 0, 0, 0))
                        CellRectangle.Fill = new SolidColorBrush(TableCell.OwnerTable.TableFormat.Background);
                    TableAdv table = TableCell.OwnerTable;
                    CellRectangle.Stroke = new SolidColorBrush(table.BorderBrush);
                    CellRectangle.StrokeThickness = table.BorderThickness;
                    //tablePath.Fill = new SolidColorBrush(table.Background);
#if !WPF
                });
#endif
        }
        #endregion

        #region Implementations
        /// <summary>
        /// Updates the height of the widget.
        /// </summary>
        internal void UpdateWidgetHeight()
        {
            if (ChildWidgets.Count > 0)
            {
                Widget lastWidget = ChildWidgets[ChildWidgets.Count - 1] as Widget;
                Height = lastWidget.Location.Y + lastWidget.Height - Location.Y;
            }
        }
        /// <summary>
        /// Updates the table row widget.
        /// </summary>
        /// <param name="rowWidget">The row widget.</param>
        /// <param name="index">The index.</param>
        internal void UpdateTableRowWidget(TableRowWidget rowWidget, int index)
        {
            if (ContainerWidget != null)
            {
                ContainerWidget.ChildWidgets.Remove(this);
                if (ContainerWidget.ChildWidgets.Count == 0
                    && ContainerWidget is TableRowWidget)
                    (ContainerWidget as TableRowWidget).Dispose();
            }
            rowWidget.ChildWidgets.Insert(index, this);
            ContainerWidget = rowWidget;
        }
        /// <summary>
        /// Highlights the specified selection.
        /// </summary>
        /// <param name="selection">The selection.</param>
        internal void Highlight(SelectionAdv selection)
        {
            //Clears Selection highlight of the child widgets.
            ClearChildSelectionHighlight();
            //Highlights the entire cell.
            selection.CreateHighlightBorder(this);
        }
        /// <summary>
        /// Gets the first line widget.
        /// </summary>
        /// <returns></returns>
        internal LineWidget GetFirstLineWidget()
        {
            Widget widget = ChildWidgets[0] as Widget;
            if (widget is ParagraphWidget)
                return widget.ChildWidgets[0] as LineWidget;
            else
            {
                TableRowWidget firstRowWidget = widget.ChildWidgets[0] as TableRowWidget;
                TableCellWidget firstCellWidget = firstRowWidget.ChildWidgets[0] as TableCellWidget;
                return firstCellWidget.GetFirstLineWidget();
            }
        }
        /// <summary>
        /// Gets the last line widget.
        /// </summary>
        /// <returns></returns>
        private LineWidget GetLastLineWidget()
        {
            Widget widget = ChildWidgets[ChildWidgets.Count - 1] as Widget;
            if (widget is ParagraphWidget)
                return widget.ChildWidgets[widget.ChildWidgets.Count - 1] as LineWidget;
            else
            {
                TableRowWidget lastRowWidget = widget.ChildWidgets[widget.ChildWidgets.Count - 1] as TableRowWidget;
                TableCellWidget lastCellWidget = lastRowWidget.ChildWidgets[lastRowWidget.ChildWidgets.Count - 1] as TableCellWidget;
                return lastCellWidget.GetLastLineWidget();
            }
        }
        /// <summary>
        /// Gets the line widget.
        /// </summary>
        /// <param name="point">The point.</param>
        /// <returns></returns>
        internal override LineWidget GetLineWidget(Point point)
        {
            for (int i = 0; i < ChildWidgets.Count; i++)
            {
                if ((ChildWidgets[i] as Widget).Location.Y <= point.Y
                    && ((ChildWidgets[i] as Widget).Location.Y + (ChildWidgets[i] as Widget).Height) >= point.Y)
                    return (ChildWidgets[i] as Widget).GetLineWidget(point);
            }
            LineWidget lineWidget = null;
            if (ChildWidgets.Count > 0)
            {
                if ((ChildWidgets[0] as Widget).Location.Y <= point.Y)
                    lineWidget = (ChildWidgets[ChildWidgets.Count - 1] as Widget).GetLineWidget(point);
                else
                    lineWidget = (ChildWidgets[0] as Widget).GetLineWidget(point);
            }
            return lineWidget;
        }
        /// <summary>
        /// Removes the widget.
        /// </summary>
        internal override void RemoveWidget()
        {
            //Removes the selection highlight borders from the page (Canvas).
            if (SelectionHighlight != null && SelectionHighlight.Parent is Panel)
                (SelectionHighlight.Parent as Panel).Children.Remove(SelectionHighlight);
            for (int i = 0; i < ChildWidgets.Count; i++)
            {
                (ChildWidgets[i] as Widget).RemoveWidget();
            }
        }
        /// <summary>
        /// Updates the height.
        /// </summary>
        /// <param name="viewer">The viewer.</param>
        internal void UpdateHeight(LayoutViewer viewer)
        {
            for (int i = 0; i < ChildWidgets.Count; i++)
            {
                if (ChildWidgets[i] is TableWidget)
                    (ChildWidgets[i] as TableWidget).UpdateHeight(viewer);
            }
        }
        /// <summary>
        /// Renders the specified page.
        /// </summary>
        /// <param name="viewer">The viewer.</param>
        internal void Render(FlowLayoutViewer viewer)
        {
            if (viewer.OwnerControl.LayoutType == LayoutType.Continuous
                && (viewer.Visiblebounds.Top > (Location.Y + Height) * viewer.ScaleFactor
                || viewer.Visiblebounds.Bottom < Location.Y * viewer.ScaleFactor))
                return;
            for (int i = 0; i < ChildWidgets.Count; i++)
            {
                if (ChildWidgets[i] is ParagraphWidget)
                    //Adds the table widget to the page.
                    (ChildWidgets[i] as ParagraphWidget).Render(viewer);
                else
                    (ChildWidgets[i] as TableWidget).Render(viewer);
            }
#if !WPF
            UIDispatcher.Execute(() =>
            {
#endif
                PageAdv page = viewer.CurrentPage;
                if (SelectionHighlight != null)
                {
                    if (SelectionHighlight.Parent is Panel)
                        (SelectionHighlight.Parent as Panel).Children.Remove(SelectionHighlight);
                    page.ForegroundContainer.Children.Add(SelectionHighlight);
                }
                if (TableCell.CellFormat.RowSpan > 0 && TableCell.CellFormat.ColumnSpan > 0)
                    //Renders table cell outline rectangle - Border and background color.
                    RenderTableCellOutline(page);
#if !WPF
            });
#endif
        }
        /// <summary>
        /// Renders the specified page.
        /// </summary>
        /// <param name="page">The page.</param>
        internal void Render(PageAdv page)
        {
            //Todo: Cell center and bottom alignments
            //if (tableCell.TableCellWidgets.IndexOf(this) == 0)
            //{
            //}
            for (int i = 0; i < ChildWidgets.Count; i++)
            {
                if (ChildWidgets[i] is ParagraphWidget)
                    //Adds the table widget to the page.
                    (ChildWidgets[i] as ParagraphWidget).Render(page);
                else
                    (ChildWidgets[i] as TableWidget).Render(page);
            }
#if !WPF
            UIDispatcher.Execute(() =>
                {
#endif
                    if (SelectionHighlight != null)
                    {
                        if (SelectionHighlight.Parent is Panel)
                            (SelectionHighlight.Parent as Panel).Children.Remove(SelectionHighlight);
                        page.ForegroundContainer.Children.Add(SelectionHighlight);
                    }
                    if (TableCell.CellFormat.RowSpan > 0 && TableCell.CellFormat.ColumnSpan > 0)
                        //Renders table cell outline rectangle - Border and background color.
                        RenderTableCellOutline(page);
#if !WPF
                });
#endif
        }
        /// <summary>
        /// Renders the table cell outline.
        /// </summary>
        /// <param name="page">The page.</param>
        internal void RenderTableCellOutline(PageAdv page)
        {
            CellRectangle.Height = Height + Margin.Top + Margin.Bottom + 1;
            CellRectangle.Width = Width + Margin.Left + Margin.Right + 1;
            if (!page.DecorationContainer.Children.Contains(CellRectangle))
            {
                if (CellRectangle.Parent is Panel)
                    (CellRectangle.Parent as Panel).Children.Remove(CellRectangle);
                page.DecorationContainer.Children.Add(CellRectangle);
            }
            Canvas.SetLeft(CellRectangle, Location.X - Margin.Left);
            Canvas.SetTop(CellRectangle, Location.Y - Margin.Top);
        }
        /// <summary>
        /// Shifts the child location.
        /// </summary>
        /// <param name="shiftTop">The shift top.</param>
        internal void ShiftChildLocation(double shiftTop)
        {
            Location = new Point(Location.X, Location.Y + shiftTop);
            if (TableCell.CellFormat.RowSpan > 0 && TableCell.CellFormat.ColumnSpan > 0)
                //Shift table cell outline rectange - Border and background color.
                Canvas.SetTop(CellRectangle, Location.Y - Margin.Top);
            for (int i = 0; i < ChildWidgets.Count; i++)
            {
                if (ChildWidgets[i] is ParagraphWidget)
                    (ChildWidgets[i] as Widget).Location = new Point((ChildWidgets[i] as Widget).Location.X, (ChildWidgets[i] as Widget).Location.Y + shiftTop);
                else
                    (ChildWidgets[i] as TableWidget).ShiftChildLocation(shiftTop);
            }
        }
        /// <summary>
        /// Updates the child location.
        /// </summary>
        /// <param name="top">The top.</param>
        internal void UpdateChildLocation(double top)
        {
            for (int i = 0; i < ChildWidgets.Count; i++)
            {
                (ChildWidgets[i] as Widget).Location = new Point((ChildWidgets[i] as Widget).Location.X, top);
                if (ChildWidgets[i] is TableWidget)
                    (ChildWidgets[i] as TableWidget).UpdateChildLocation(top);
                top += (ChildWidgets[i] as Widget).Height;
            }
        }
        /// <summary>
        /// Determines whether the first line fit in the specified bottom.
        /// </summary>
        /// <param name="bottom">The bottom.</param>
        /// <returns>
        ///   <c>true</c> if the first line fit in the specified bottom; otherwise, <c>false</c>.
        /// </returns>
        internal bool IsFirstLineFit(double bottom)
        {
            if (ChildWidgets.Count == 0)
                return true;
            if (ChildWidgets[0] is ParagraphWidget)
            {
                ParagraphWidget paraWidget = ChildWidgets[0] as ParagraphWidget;
                return paraWidget.IsFirstLineFit(bottom - Margin.Bottom);
            }
            else
            {
                TableWidget tableWidget = ChildWidgets[0] as TableWidget;
                return tableWidget.IsFirstLineFit(bottom - Margin.Bottom);
            }
        }
        /// <summary>
        /// Gets the splitted widget.
        /// </summary>
        /// <param name="bottom">The bottom.</param>
        /// <returns></returns>
        internal TableCellWidget GetSplittedWidget(double bottom, bool splitMinimalWidget)
        {
            TableCellWidget splittedWidget = null;
            if (Location.Y + Height > bottom - Margin.Bottom)
            {
                for (int i = 0; i < ChildWidgets.Count; i++)
                {
                    if (ChildWidgets[i] is ParagraphWidget)
                    {
                        ParagraphWidget paragraphWidget = ChildWidgets[i] as ParagraphWidget;
                        ParagraphWidget splittedPara = paragraphWidget.GetSplittedWidget(bottom - Margin.Bottom);
                        if (splittedPara != null)
                        {
                            if (i == 0 && splittedPara == paragraphWidget)
                                //Returns if the whole content of the cell does not fit in current page.
                                return this;
                            if (ChildWidgets.Contains(splittedPara))
                            {
                                ChildWidgets.Remove(splittedPara);
                                i--;
                            }
                            Height -= splittedPara.Height;
                            if (splittedWidget == null)
                            {
                                //Creates new widget, to hold the splitted contents.
                                splittedWidget = CreateCellWidget();
                            }
                            splittedWidget.Height += splittedPara.Height;
                            splittedWidget.ChildWidgets.Add(splittedPara);
                            splittedPara.ContainerWidget = splittedWidget;
                        }
                    }
                    else
                    {
                        TableWidget tableWidget = ChildWidgets[i] as TableWidget;
                        //Check for nested table.
                        if (bottom - Margin.Bottom < tableWidget.Location.Y + tableWidget.Height)
                        {
                            double tableHeight = tableWidget.Height;
                            TableWidget splittedTable = tableWidget.GetSplittedWidget(bottom - Margin.Bottom);
                            if (splittedTable != null)
                            {
                                if (i == 0 && splittedTable == tableWidget)
                                    //Returns if the whole table does not fit in current page.
                                    return this;
                                if (ChildWidgets.Contains(splittedTable))
                                {
                                    ChildWidgets.Remove(splittedTable);
                                    i--;
                                    Height -= splittedTable.Height;
                                }
                                else
                                    Height -= tableHeight - tableWidget.Height;
                                if (splittedWidget == null)
                                {
                                    //Creates new widget, to hold the splitted contents.
                                    splittedWidget = CreateCellWidget();
                                }
                                splittedWidget.Height += splittedTable.Height;
                                splittedWidget.ChildWidgets.Add(splittedTable);
                                splittedTable.ContainerWidget = splittedWidget;
                            }
                        }
                    }
                }
            }
            if (splittedWidget == null && splitMinimalWidget)
            {
                if (ChildWidgets.Count == 0)
                    Height = 0;
                //Creates new widget, to hold the splitted contents.
                splittedWidget = CreateCellWidget();
            }
            return splittedWidget;
        }
        /// <summary>
        /// Creates the cell widget.
        /// </summary>
        /// <returns></returns>
        private TableCellWidget CreateCellWidget()
        {
            TableCellWidget cellWidget = new TableCellWidget(TableCell);
            cellWidget.UpdateWidgetLocation(this);
            TableCell.TableCellWidgets.Add(cellWidget);
            cellWidget.Margin = Margin;
            return cellWidget;
        }
        /// <summary>
        /// Clears the selection highlight.
        /// </summary>
        internal void ClearSelectionHighlight()
        {
            if (SelectionHighlight != null)
            {
                if (SelectionHighlight.Parent is Panel)
                    (SelectionHighlight.Parent as Panel).Children.Remove(SelectionHighlight);
                SelectionHighlight = null;
            }
        }
        /// <summary>
        /// Releases unmanaged and - optionally - managed resources.
        /// </summary>
        internal void Dispose()
        {
            if (ContainerWidget != null)
            {
                ContainerWidget.ChildWidgets.Remove(this);
                ContainerWidget = null;
            }
            for (int i = 0; i < ChildWidgets.Count; i++)
            {
                Widget widget = ChildWidgets[i] as Widget;
                if (widget is ParagraphWidget)
                    (widget as ParagraphWidget).Dispose();
                else
                    (widget as TableWidget).Dispose();
                ChildWidgets.Remove(widget);
                i--;
            }
#if !WPF
            UIDispatcher.Execute(() =>
            {
#endif
                if (CellRectangle != null)
                {
                    if (CellRectangle.Parent is Panel)
                        (CellRectangle.Parent as Panel).Children.Remove(CellRectangle);
                    CellRectangle = null;
                }
                ClearSelectionHighlight();
#if !WPF
            });
#endif
            if (TableCell != null)
            {
                if (TableCell.TableCellWidgets != null)
                    TableCell.TableCellWidgets.Remove(this);
                TableCell = null;
            }
        }
        #endregion
    }
    internal sealed class LineWidget : IWidget
    {
        #region Fields
        internal double Width = 0, Height = 0;
        internal ParagraphWidget ParagraphWidget;
        internal List<ElementBox> Children;
        internal Border SelectionHighlight = null;
        #endregion

        #region Properties
        #endregion

        #region Constructor
        /// <summary>
        /// Initializes a new instance of the <see cref="LineWidget"/> class.
        /// </summary>
        /// <param name="paragraphWidget">The paragraph widget.</param>
        internal LineWidget(ParagraphWidget paragraphWidget)
        {
            Children = new List<ElementBox>();
            ParagraphWidget = paragraphWidget;
        }
        #endregion

        #region Implementations
        /// <summary>
        /// Updates the paragraph widget.
        /// </summary>
        /// <param name="newParagraphWidget">The new paragraph widget.</param>
        /// <param name="index">The index.</param>
        internal void UpdateParagraphWidget(ParagraphWidget newParagraphWidget, int index)
        {
            if (ParagraphWidget != null)
            {
                ParagraphWidget.ChildWidgets.Remove(this);
                ParagraphWidget.Height -= Height;
                if (ParagraphWidget.ContainerWidget != null)
                    ParagraphWidget.ContainerWidget.Height -= Height;
                if (ParagraphWidget.ChildWidgets.Count == 0)
                    ParagraphWidget.Dispose();
            }
            newParagraphWidget.ChildWidgets.Insert(index, this);
            ParagraphWidget = newParagraphWidget;
            newParagraphWidget.Height += Height;
            if (newParagraphWidget.ContainerWidget != null)
                newParagraphWidget.ContainerWidget.Height += Height;
        }
        /// <summary>
        /// Gets the height of the max element.
        /// </summary>
        /// <returns></returns>
        internal double GetMaxElementHeight()
        {
            double height = 0;
            if (Children.Count == 0 || Children.Count == 1 && Children[0] is ListTextElementBox)
            {
                double topMargin = 0, bottomMargin = 0;
                height = ParagraphWidget.Paragraph.GetParagraphMarkSize(ref topMargin, ref bottomMargin).Height;
                height += topMargin;
                if (Children.Count == 1)
                {
                    ListTextElementBox element = Children[0] as ListTextElementBox;
                    if (height < element.Margin.Top + element.Height)
                        height = element.Margin.Top + element.Height;
                }
            }
            else
            {
                foreach (ElementBox element in Children)
                {
                    if (height < element.Margin.Top + element.Height)
                        height = element.Margin.Top + element.Height;
                }
            }
            return height;
        }
        /// <summary>
        /// Gets the top.
        /// </summary>
        /// <returns></returns>
        internal double GetTop()
        {
            double top = ParagraphWidget.Location.Y;
            int count = ParagraphWidget.ChildWidgets.IndexOf(this);
            for (int i = 0; i < count; i++)
            {
                top += (ParagraphWidget.ChildWidgets[i] as LineWidget).Height;
            }
            return top;
        }
        /// <summary>
        /// Gets the current line start left.
        /// </summary>
        /// <returns></returns>
        internal double GetLineStartLeft()
        {
            double left = ParagraphWidget.Location.X;
            if (IsParagraphFirstLine())
                left += ParagraphWidget.Paragraph.ParagraphFormat.FirstLineIndent;
            if (Children.Count > 0)
                left += Children[0].Margin.Left;
            return left;
        }
        /// <summary>
        /// Gets the left of text.
        /// </summary>
        /// <returns></returns>
        internal double GetLeft()
        {
            double left = ParagraphWidget.Location.X;
#if !WPF
            UIDispatcher.Execute(() =>
            {
#endif
                if (IsParagraphFirstLine())
                    left += ParagraphWidget.Paragraph.ParagraphFormat.FirstLineIndent;
#if !WPF
            });
#endif
            for (int i = 0; i < Children.Count; i++)
            {
                ElementBox element = Children[i];
                if (element is ListTextElementBox)
                    left += element.Margin.Left + element.Width;
                else
                {
                    left += element.Margin.Left;
                    break;
                }
            }
            return left;
        }
        /// <summary>
        /// Gets the left.
        /// </summary>
        /// <param name="elementBox">The element box.</param>
        /// <param name="index">The index.</param>
        /// <returns></returns>
        internal double GetLeft(ElementBox elementBox, int index)
        {
            double left = ParagraphWidget.Location.X;
#if !WPF
            UIDispatcher.Execute(() =>
            {
#endif
                if (IsParagraphFirstLine())
                    left += ParagraphWidget.Paragraph.ParagraphFormat.FirstLineIndent;
#if !WPF
            });
#endif
            int count = Children.IndexOf(elementBox);
            if (Children.Count == 1 && Children[0] is ListTextElementBox)
                count = 1;
            for (int i = 0; i < count; i++)
            {
                left += Children[i].Margin.Left + Children[i].Width;
            }
            if (elementBox != null)
                left += elementBox.Margin.Left;
            if (elementBox is TextElementBox)
            {
                if (index == (elementBox as TextElementBox).Length)
                    left += elementBox.Width;
                else if (index > (elementBox as TextElementBox).Length)
                    //Include width of Paragraph mark.
                    left += elementBox.Width + TextHelper.GetParagraphMarkSize(elementBox.Inline.OwnerParagraph.CharacterFormat).Width;
                else
                    left += TextHelper.MeasureTextWithSpace(elementBox as TextElementBox, index).Width;
            }
            else if (index > 0)
            {
                if (elementBox != null)
                {
                    left += elementBox.Width;
                    if (index == 2)
                        //Include width of Paragraph mark.
                        left += TextHelper.GetParagraphMarkSize(elementBox.Inline.OwnerParagraph.CharacterFormat).Width;
                }
                else
                    left += TextHelper.GetParagraphMarkSize(ParagraphWidget.Paragraph.CharacterFormat).Width;
            }
            return left;
        }
        /// <summary>
        /// Gets the width.
        /// </summary>
        /// <param name="includeParagraphMark">if set to <c>true</c> [include paragraph mark].</param>
        /// <returns></returns>
        internal double GetWidth(bool includeParagraphMark)
        {
            double width = 0;
#if !WPF
            UIDispatcher.Execute(() =>
            {
#endif
                if (IsParagraphFirstLine())
                    width += ParagraphWidget.Paragraph.ParagraphFormat.FirstLineIndent;
#if !WPF
            });
#endif
            for (int i = 0; i < Children.Count; i++)
            {
                width += Children[i].Margin.Left + Children[i].Width;
            }
            if (includeParagraphMark && ParagraphWidget.Paragraph.ParagraphWidgets.IndexOf(ParagraphWidget) == ParagraphWidget.Paragraph.ParagraphWidgets.Count - 1
                && ParagraphWidget.ChildWidgets.IndexOf(this) == ParagraphWidget.ChildWidgets.Count - 1)
                width += TextHelper.GetParagraphMarkSize(ParagraphWidget.Paragraph.CharacterFormat).Width;
            return width;
        }
        /// <summary>
        /// Gets the first element.
        /// </summary>
        /// <returns></returns>
        internal ElementBox GetFirstElement()
        {
            ElementBox element = null;
            for (int i = 0; i < Children.Count; i++)
            {
                element = Children[i];
                if (element is ListTextElementBox)
                    element = null;
                else
                    break;
            }
            return element;
        }
        /// <summary>
        /// Gets the first element.
        /// </summary>
        /// <param name="left">The left.</param>
        /// <returns></returns>
        internal ElementBox GetFirstElement(ref double left)
        {
            double firstLineIndent = 0;
#if !WPF
            UIDispatcher.Execute(() =>
            {
#endif
                if (IsParagraphFirstLine())
                    firstLineIndent = ParagraphWidget.Paragraph.ParagraphFormat.FirstLineIndent;
#if !WPF
            });
#endif
            left += firstLineIndent;
            ElementBox element = null;
            for (int i = 0; i < Children.Count; i++)
            {
                element = Children[i];
                if (element is ListTextElementBox)
                {
                    left += element.Margin.Left + element.Width;
                    element = null;
                }
                else
                    break;
            }
            return element;
        }
        /// <summary>
        /// Gets the hyperlink field.
        /// </summary>
        /// <param name="rte">The rte.</param>
        /// <param name="cursorPosition">The cursor position.</param>
        /// <returns></returns>
        internal FieldBeginAdv GetHyperlinkField(SfRichTextBoxAdv rte, Point cursorPosition)
        {
            Inline inline = null;
            double top = GetTop();
            double lineStartLeft = GetLineStartLeft();
            if (cursorPosition.Y < top || cursorPosition.Y > top + Height
                || cursorPosition.X < lineStartLeft || cursorPosition.X > lineStartLeft + Width)
                return null;
            double left = ParagraphWidget.Location.X;
            ElementBox element = GetFirstElement(ref left);
            if (element == null)
            {
                double width = TextHelper.GetParagraphMarkSize(ParagraphWidget.Paragraph.CharacterFormat).Width;
                if (cursorPosition.X <= lineStartLeft + width)
                {
                    //Check if paragraph is within a field result.
                    List<FieldBeginAdv> CheckedFields = new List<FieldBeginAdv>();
                    FieldBeginAdv field = ParagraphWidget.Paragraph.GetHyperlinkField(ParagraphWidget.Paragraph, CheckedFields);
                    CheckedFields.Clear();
                    CheckedFields = null;
                    return field;
                }
            }
            else
            {
                if (cursorPosition.X > left + element.Margin.Left)
                {
                    for (int i = Children.IndexOf(element); i < Children.Count; i++)
                    {
                        element = Children[i];
                        if (cursorPosition.X < left + element.Margin.Left + element.Width || i == Children.Count - 1)
                            break;
                        left += element.Margin.Left + element.Width;
                    }
                }
                inline = element.Inline;
                double width = element.Margin.Left + element.Width;
                if (inline.NextNode == null && element == inline.Elements[inline.Elements.Count - 1])
                    //Include width of Paragraph mark.
                    width += TextHelper.GetParagraphMarkSize(inline.OwnerParagraph.CharacterFormat).Width;
                if (cursorPosition.X <= left + width)
                {
                    //Check if inline is within a field result.
                    List<FieldBeginAdv> CheckedFields = new List<FieldBeginAdv>();
                    FieldBeginAdv field = inline.OwnerParagraph.GetHyperlinkField(inline, CheckedFields);
                    CheckedFields.Clear();
                    CheckedFields = null;
                    return field;
                }
            }
            return null;
        }
        /// <summary>
        /// Updates the text position.
        /// </summary>
        /// <param name="rte">The rte.</param>
        /// <param name="inline">The inline.</param>
        /// <param name="index">The index.</param>
        /// <param name="caretPosition">The caret position.</param>
        /// <param name="includeParagraphMark">if set to <c>true</c> [include paragraph mark].</param>
        /// <returns></returns>
        private bool UpdateTextPosition(SfRichTextBoxAdv rte, ref Inline inline, ref int index, ref Point caretPosition, bool includeParagraphMark)
        {
            bool isImageSelected = false;
            double top = GetTop();
            double left = ParagraphWidget.Location.X;
            ElementBox element = GetFirstElement(ref left);
            if (element == null)
            {
                double topMargin = 0, bottomMargin = 0;
                Size size = ParagraphWidget.Paragraph.GetParagraphMarkSize(ref topMargin, ref bottomMargin);
                if (includeParagraphMark && caretPosition.X > left + size.Width / 2)
                {
                    left += size.Width;
                    if (ParagraphWidget.Paragraph.Inlines.Count > 0)
                    {
                        inline = ParagraphWidget.Paragraph.Inlines[ParagraphWidget.Paragraph.Inlines.Count - 1];
                        index = inline.Length;
                    }
                    index++;
                }
                caretPosition = new Point(left, topMargin > 0 ? top + topMargin : top);
            }
            else
            {
                if (caretPosition.X > left + element.Margin.Left)
                {
                    for (int i = Children.IndexOf(element); i < Children.Count; i++)
                    {
                        element = Children[i];
                        if (caretPosition.X < left + element.Margin.Left + element.Width || i == Children.Count - 1)
                            break;
                        left += element.Margin.Left + element.Width;
                    }
                    if (caretPosition.X > left + element.Margin.Left + element.Width)
                    {
                        index = element is TextElementBox ? GetTextLength(element) + (element as TextElementBox).Length : 1;
                        left += element.Margin.Left + element.Width;
                    }
                    else if (element is TextElementBox)
                    {
                        double x = caretPosition.X - left - element.Margin.Left;
                        left += element.Margin.Left;
                        double prevWidth = 0;
                        int charIndex = 0;
                        for (int i = 1; i <= (element as TextElementBox).Length; i++)
                        {
                            double width = 0;
                            if (i == (element as TextElementBox).Length)
                                width = element.Width;
                            else
                                width = TextHelper.MeasureTextWithSpace(element as TextElementBox, i).Width;
                            if (x < width || i == (element as TextElementBox).Length)
                            {
                                //Updates exact left position of the caret.
                                double charWidth = width - prevWidth;
                                if (x - prevWidth > charWidth / 2)
                                {
                                    left += width;
                                    charIndex = i;
                                }
                                else
                                {
                                    left += prevWidth;
                                    charIndex = i - 1;
                                    if (i == 1 && element != Children[0])
                                    {
                                        int curIndex = Children.IndexOf(element);
                                        if (!(Children[curIndex - 1] is ListTextElementBox))
                                        {
                                            element = Children[curIndex - 1];
                                            charIndex = element is TextElementBox ? (element as TextElementBox).Length : 1;
                                        }
                                    }
                                }
                                break;
                            }
                            prevWidth = width;
                        }
                        index = GetTextLength(element) + charIndex;
                    }
                    else
                    {
                        if (!rte.IsReadOnlyMode)
                            isImageSelected = element is ImageElementBox;
                        if (caretPosition.X - left - element.Margin.Left > element.Width / 2)
                        {
                            index = 1;
                            left += element.Margin.Left + element.Width;
                        }
                        else if (element != Children[0] && !isImageSelected)
                        {
                            int curIndex = Children.IndexOf(element);
                            if (!(Children[curIndex - 1] is ListTextElementBox))
                            {
                                element = Children[curIndex - 1];
                                index = element is TextElementBox ? GetTextLength(element) + (element as TextElementBox).Length : 1;
                            }
                        }
                    }
                }
                else
                {
                    left += element.Margin.Left;
                    index = GetTextLength(element);
                }
                if (element is TextElementBox)
                    top += element.Margin.Top > 0 ? element.Margin.Top : 0;
                else
                {
                    double height = TextHelper.MeasureText(element.Inline.CharacterFormat).Height;
                    double baselineOffset = TextHelper.TextMeasurer.BaselineOffset;
                    top += element.Margin.Top + element.Height - baselineOffset;
                }
                inline = element.Inline;
                inline = inline.ValidateTextPosition(ref index);
                bool isParagraphEnd = inline.NextNode == null && index == inline.Length;
                if (includeParagraphMark && inline.NextNode is FieldCharacterAdv && index == inline.Length)
                    isParagraphEnd = inline.IsLastRenderedInline(index);
                if (includeParagraphMark && isParagraphEnd)
                {
                    //Include width of Paragraph mark.
                    double width = TextHelper.GetParagraphMarkSize(inline.OwnerParagraph.CharacterFormat).Width;
                    if (caretPosition.X > left + width / 2)
                    {
                        left += width;
                        index = inline.Length + 1;
                    }
                }
                caretPosition = new Point(left, top);
            }
            return isImageSelected;
        }
        /// <summary>
        /// Updates the text position.
        /// </summary>
        /// <param name="rte">The rte.</param>
        /// <param name="point">The point.</param>
        /// <param name="isDoubleTapped">if set to <c>true</c> [is double tapped].</param>
        internal void UpdateTextPosition(SfRichTextBoxAdv rte, Point point, bool isDoubleTapped)
        {
            Point caretPosition = point;
            Inline inline = null;
            int index = 0;
            bool isImageSelected = UpdateTextPosition(rte, ref inline, ref index, ref caretPosition, false);
            if (isImageSelected)
            {
                //Selects the image and adds the image resizer to viewer.
                rte.Selection.Select(ParagraphWidget.Paragraph, inline, index, caretPosition);
                if (index == 0)
                    rte.Selection.ExtendForward();
                else
                    rte.Selection.ExtendBackward();
            }
            else
            {
                if (isDoubleTapped)
                {
                    if (inline == null)
                    {
                        rte.Selection.Select(ParagraphWidget.Paragraph, inline, index, caretPosition);
                        rte.Selection.ExtendForward();
                    }
                    else
                        inline.SelectWord(rte.Selection, index, caretPosition.X > point.X);
                }
                else
                    rte.Selection.Select(ParagraphWidget.Paragraph, inline, index, caretPosition);
            }
        }
        /// <summary>
        /// Updates the text position.
        /// </summary>
        /// <param name="rte">The rte.</param>
        /// <param name="point">The point.</param>
        /// <param name="textPosition">The text position.</param>
        /// <param name="includeParagraphMark">if set to <c>true</c> [include paragraph mark].</param>
        internal void UpdateTextPosition(SfRichTextBoxAdv rte, Point point, TextPosition textPosition, bool includeParagraphMark)
        {
            Point caretPosition = point;
            Inline inline = null;
            int index = 0;
            UpdateTextPosition(rte, ref inline, ref index, ref caretPosition, includeParagraphMark);
            textPosition.SetPosition(ParagraphWidget.Paragraph, inline, index, caretPosition);
        }
        /// <summary>
        /// Gets the length of the text.
        /// </summary>
        /// <param name="element">The element.</param>
        /// <returns></returns>
        private int GetTextLength(ElementBox element)
        {
            int length = 0;
            int count = element.Inline.Elements.IndexOf(element);
            for (int i = 0; i < count; i++)
            {
                length += (element.Inline.Elements[i] as TextElementBox).Length;
            }
            return length;
        }
        /// <summary>
        /// Clears the selection highlight.
        /// </summary>
        internal void ClearSelectionHighlight()
        {
            if (SelectionHighlight != null)
            {
                if (SelectionHighlight.Parent is Panel)
                    (SelectionHighlight.Parent as Panel).Children.Remove(SelectionHighlight);
                SelectionHighlight = null;
            }
        }
        /// <summary>
        /// Removes the widget.
        /// </summary>
        internal void RemoveWidget()
        {
            //Removes the selection highlight borders from the page (Canvas).
            if (SelectionHighlight != null && SelectionHighlight.Parent is Panel)
                (SelectionHighlight.Parent as Panel).Children.Remove(SelectionHighlight);
            for (int i = 0; i < Children.Count; i++)
            {
                Children[i].RemoveRenderedElements(null);
            }
        }
        /// <summary>
        /// Renders the specified page.
        /// </summary>
        /// <param name="page">The page.</param>
        /// <param name="left">The left.</param>
        /// <param name="top">The top.</param>
        internal void Render(PageAdv page, double left, double top)
        {
#if !WPF
            UIDispatcher.Execute(() =>
            {
#endif
                if (SelectionHighlight != null)
                {
                    if (SelectionHighlight.Parent is Panel)
                        (SelectionHighlight.Parent as Panel).Children.Remove(SelectionHighlight);
                    page.ForegroundContainer.Children.Add(SelectionHighlight);
                }
                if (IsParagraphFirstLine())
                    left += ParagraphWidget.Paragraph.ParagraphFormat.FirstLineIndent;
                for (int i = 0; i < Children.Count; i++)
                {
                    Children[i].Render(page, left, top);
                    left += Children[i].Margin.Left + Children[i].Width;
                }
#if !WPF
            });
#endif
        }
        /// <summary>
        /// Releases unmanaged and - optionally - managed resources.
        /// </summary>
        internal void Dispose()
        {
            if (ParagraphWidget != null)
            {
                ParagraphWidget.ChildWidgets.Remove(this);
                ParagraphWidget.Height -= Height;
                ParagraphWidget = null;
            }
#if !WPF
            UIDispatcher.Execute(() =>
            {
#endif
                ClearSelectionHighlight();
                for (int i = 0; i < Children.Count; i++)
                {
                    ElementBox element = Children[i];
                    element.Dispose();
                    Children.Remove(element);
                    i--;
                }
#if !WPF
            });
#endif
        }
        /// <summary>
        /// Clears the child widgets.
        /// </summary>
        /// <param name="viewer">The viewer.</param>
        /// <param name="prevElement">The prev element.</param>
        /// <returns></returns>
        internal double ClearChildWidgets(LayoutViewer viewer, ElementBox prevElement)
        {
            bool startClear = false;
            double left = 0;
#if !WPF
            UIDispatcher.Execute(() =>
            {
#endif
                if (IsParagraphFirstLine())
                    left += ParagraphWidget.Paragraph.ParagraphFormat.FirstLineIndent;
                for (int i = 0; i < Children.Count; i++)
                {
                    ElementBox element = Children[i];
                    element.CurrentLineWidget = null;
                    if (startClear || prevElement == null)
                        element.Dispose();
                    else
                    {
                        viewer.LineElements.Add(element);
                        left += element.Margin.Left + element.Width;
                    }
                    if (prevElement == element)
                        startClear = true;
                    Children.Remove(element);
                    i--;
                }
                ParagraphWidget = null;
                ClearSelectionHighlight();
#if !WPF
            });
#endif    
            return left;
        }
        /// <summary>
        /// Preserves the previous elements.
        /// </summary>
        /// <param name="viewer">The viewer.</param>
        /// <returns></returns>
        internal double PreservePreviousElements(LayoutViewer viewer)
        {
            double left = 0;
#if !WPF
            UIDispatcher.Execute(() =>
            {
#endif
                if (IsParagraphFirstLine())
                    left += ParagraphWidget.Paragraph.ParagraphFormat.FirstLineIndent;
#if !WPF
            });
#endif    
            for (int i = 0; i < Children.Count; i++)
            {
                ElementBox element = Children[i];
                element.CurrentLineWidget = null;
                ParagraphAdv paragraph = element.BaseNode is Inline ? (element.BaseNode as Inline).OwnerParagraph : element.BaseNode as ParagraphAdv;
                if (paragraph == null || paragraph == ParagraphWidget.Paragraph)
                    break;
                else
                {
                    viewer.LineElements.Add(element);
                    left += element.Margin.Left + element.Width;
                }
                Children.Remove(element);
                i--;
            }
            return left;
        }
        /// <summary>
        /// Determines whether [is paragraph first line].
        /// </summary>
        /// <returns>
        ///   <c>true</c> if [is paragraph first line]; otherwise, <c>false</c>.
        /// </returns>
        internal bool IsParagraphFirstLine()
        {
            if ((this == ParagraphWidget.ChildWidgets[0]) && (ParagraphWidget == ParagraphWidget.Paragraph.ParagraphWidgets[0]))
                return true;
            return false;
        }
        #endregion
    }
    internal abstract class ElementBox
    {
        #region Fields
        internal double Width = 0, Height = 0;
        internal Thickness Margin;
        internal BaseNode BaseNode;
        internal LineWidget CurrentLineWidget;
        internal FrameworkElement RenderedElement;
        internal List<FrameworkElement> DecorationElements;
        #endregion

        #region Properties
        /// <summary>
        /// Gets the inline.
        /// </summary>
        /// <value>
        /// The inline.
        /// </value>
        internal Inline Inline
        {
            get
            {
                return BaseNode as Inline;
            }
        }
        #endregion

        #region Constructor
        /// <summary>
        /// Initializes a new instance of the <see cref="ElementBox" /> class.
        /// </summary>
        /// <param name="baseNode">The base node.</param>
        internal ElementBox(BaseNode baseNode)
        {
            BaseNode = baseNode;
            DecorationElements = new List<FrameworkElement>();
        }
        #endregion

        #region Implementations
        /// <summary>
        /// Renders the specified page.
        /// </summary>
        /// <param name="page">The page.</param>
        /// <param name="left">The left.</param>
        /// <param name="top">The top.</param>
        internal abstract void Render(PageAdv page, double left, double top);
        /// <summary>
        /// Releases unmanaged and - optionally - managed resources.
        /// </summary>
        internal abstract void Dispose();
        /// <summary>
        /// Gets the index in inline.
        /// </summary>
        /// <returns></returns>
        internal int GetIndexInInline()
        {
            int indexInInline = 0;
            if (Inline != null && this is TextElementBox)
            {
                int count = Inline.Elements.IndexOf(this);
                for (int i = 0; i < count; i++)
                {
                    indexInInline += (Inline.Elements[i] as TextElementBox).Length;
                }
            }
            return indexInInline;
        }
        /// <summary>
        /// Renders the highlight element.
        /// </summary>
        /// <param name="page">The page.</param>
        /// <param name="left">The left.</param>
        /// <param name="top">The top.</param>
        /// <param name="format">The format.</param>
        protected void RenderHighlightElement(PageAdv page, double left, double top, CharacterFormat format)
        {
            Border highlightElement = new Border();
            highlightElement.Background = new SolidColorBrush(format.GetHighlightColor());
            highlightElement.Width = Width;
            highlightElement.Height = Margin.Top < 0 ? Margin.Top + Height : Height;
            Canvas.SetLeft(highlightElement, left + Margin.Left);
            top += (Margin.Top > 0 ? Margin.Top : 0);
            Canvas.SetTop(highlightElement, top);
            page.ForegroundContainer.Children.Add(highlightElement);
            DecorationElements.Add(highlightElement);
        }
        /// <summary>
        /// Renders the strike through.
        /// </summary>
        /// <param name="page">The page.</param>
        /// <param name="left">The left.</param>
        /// <param name="top">The top.</param>
        /// <param name="strikeThrough">The strike through.</param>
        /// <param name="fontColor">The font color.</param>
        /// <param name="baselineAlignment">The baseline alignment.</param>
        protected void RenderStrikeThrough(PageAdv page, double left, double top, StrikeThrough strikeThrough, Color fontColor, BaselineAlignment baselineAlignment)
        {
            double renderedHeight = Height / (baselineAlignment == BaselineAlignment.Normal ? 1 : 1.5);
            double topMargin = Margin.Top;
            if (baselineAlignment == BaselineAlignment.Subscript)
                topMargin += Height - renderedHeight;
            top += topMargin > 0 ? topMargin : 0;
            double lineHeight = renderedHeight / 20;
            double y = renderedHeight / 2 + 0.5 * lineHeight;
            int lineCnt = 0;
            if (strikeThrough == StrikeThrough.DoubleStrike)
                y -= lineHeight;
            while (lineCnt < (strikeThrough == StrikeThrough.DoubleStrike ? 2 : 1))
            {
                lineCnt++;
                if (topMargin + y <= -lineHeight)
                    continue;
                Border strikeLine = new Border();
                strikeLine.Width = Width;
                strikeLine.Height = lineHeight;
                strikeLine.Background = new SolidColorBrush(fontColor);
                if (topMargin < 0)
                {
                    strikeLine.Clip = new RectangleGeometry();
#if WPF
                    (strikeLine.Clip as RectangleGeometry).Rect = new Rect(0, -topMargin, Width, lineHeight);
#else
                    strikeLine.Clip.Rect = new Rect(0, -topMargin, Width, lineHeight);
#endif
                }
                Canvas.SetLeft(strikeLine, left + Margin.Left);
                Canvas.SetTop(strikeLine, top + y);
                page.ForegroundContainer.Children.Add(strikeLine);
                DecorationElements.Add(strikeLine);
                y += 2 * lineHeight;
            }
        }
        /// <summary>
        /// Renders the underline.
        /// </summary>
        /// <param name="page">The page.</param>
        /// <param name="left">The left.</param>
        /// <param name="top">The top.</param>
        /// <param name="underline">The underline.</param>
        /// <param name="fontColor">The font color.</param>
        /// <param name="baselineAlignment">The baseline alignment.</param>
        protected void RenderUnderline(PageAdv page, double left, double top, Underline underline, Color fontColor, BaselineAlignment baselineAlignment)
        {
            double renderedHeight = Height / (baselineAlignment == BaselineAlignment.Normal ? 1 : 1.5);
            double topMargin = Margin.Top;
            if (baselineAlignment == BaselineAlignment.Subscript)
                topMargin += Height - renderedHeight;
            top += topMargin > 0 ? topMargin : 0;
            double lineHeight = renderedHeight / 20;
            double y = renderedHeight - 2 * lineHeight;
            Border underlineBorder = new Border();
            underlineBorder.Width = Width;
            underlineBorder.Height = lineHeight;
            underlineBorder.Background = new SolidColorBrush(fontColor);
            if (topMargin < 0)
            {
                underlineBorder.Clip = new RectangleGeometry();
#if WPF
                (underlineBorder.Clip as RectangleGeometry).Rect = new Rect(0, -topMargin, Width, lineHeight);
#else
                underlineBorder.Clip.Rect = new Rect(0, -topMargin, Width, lineHeight);
#endif
            }
            Canvas.SetLeft(underlineBorder, left + Margin.Left);
            Canvas.SetTop(underlineBorder, top + y);
            page.ForegroundContainer.Children.Add(underlineBorder);
            DecorationElements.Add(underlineBorder);
        }
        /// <summary>
        /// Removes the rendered elements.
        /// </summary>
        /// <param name="page">The page.</param>
        internal void RemoveRenderedElements(PageAdv page)
        {
            if (RenderedElement != null)
            {
                if (RenderedElement.Parent is Panel)
                    (RenderedElement.Parent as Panel).Children.Remove(RenderedElement);
                else if (page != null)
                    page.ForegroundContainer.Children.Remove(RenderedElement);
                RenderedElement = null;
            }
            for (int i = 0; i < DecorationElements.Count; i++)
            {
                FrameworkElement decorationElement = DecorationElements[i];
                if (decorationElement.Parent is Panel)
                    (decorationElement.Parent as Panel).Children.Remove(decorationElement);
                else if (page != null)
                    page.ForegroundContainer.Children.Remove(decorationElement);
                DecorationElements.Remove(decorationElement);
                i--;
            }
        }
        #endregion
    }
    internal sealed class TextElementBox : ElementBox
    {
        #region Fields
        internal double BaselineOffset = 0;
        internal int StartIndex = 0;
        internal int Length = 0;
        #endregion

        #region Properties
        /// <summary>
        /// Gets the text.
        /// </summary>
        /// <value>
        /// The text.
        /// </value>
        internal string Text
        {
            get
            {
                if (Inline is SpanAdv)
                    return (Inline as SpanAdv).Text.Substring(StartIndex, Length);
                return "";
            }
        }
        #endregion

        #region Constructor
        /// <summary>
        /// Initializes a new instance of the <see cref="TextElementBox" /> class.
        /// </summary>
        /// <param name="inline">The inline.</param>
        internal TextElementBox(Inline inline)
            : base(inline)
        {
        }
        #endregion

        #region Implementations
        /// <summary>
        /// Renders the specified page.
        /// </summary>
        /// <param name="page">The page.</param>
        /// <param name="left">The left.</param>
        /// <param name="top">The top.</param>
        internal override void Render(PageAdv page, double left, double top)
        {
            TextBlock textBlock = new TextBlock();
            CharacterFormat format = Inline.CharacterFormat;
            Color fontColor = format.FontColor;
            //Gets the font color compared to the background color, if font color is empty(auto).
            if (fontColor == Color.FromArgb(0, 0, 0, 0))
            {
                DocumentAdv document = Inline.OwnerParagraph.Document;
                fontColor = Inline.CharacterFormat.GetDefaultFontColor(document);
            }
            textBlock.Foreground = new SolidColorBrush(fontColor);
            BaselineAlignment baselineAlignment = format.BaselineAlignment;
            textBlock.FontSize = format.FontSize / (baselineAlignment == BaselineAlignment.Normal ? 1 : 1.5);
            textBlock.FontFamily = format.FontFamily;
            if (format.Bold)
                textBlock.FontWeight = FontWeights.Bold;
            if (format.Italic)
                textBlock.FontStyle = FontStyleEnum.Italic;
            textBlock.Text = Text;
            textBlock.Width = Width;
            double topMargin = Margin.Top;
            if (baselineAlignment == BaselineAlignment.Subscript)
                topMargin += Height - Height / 1.5;
            if (topMargin < 0)
            {
                textBlock.Clip = new RectangleGeometry();
#if WPF
                (textBlock.Clip as RectangleGeometry).Rect = new Rect(0, -topMargin, Width, Height);
#else
                textBlock.Clip.Rect = new Rect(0, -topMargin, Width, Height);
#endif
            }
            RemoveRenderedElements(page);
            if (format.HighlightColor != HighlightColor.NoColor)
                RenderHighlightElement(page, left, top, format);
            RenderedElement = textBlock;
            page.ForegroundContainer.Children.Add(textBlock);
            Canvas.SetLeft(textBlock, left + Margin.Left);
            Canvas.SetTop(textBlock, top + topMargin);
            StrikeThrough strikeThrough = format.StrikeThrough;
            if (strikeThrough != StrikeThrough.None)
                RenderStrikeThrough(page, left, top, strikeThrough, fontColor, baselineAlignment);
            Underline underline = format.Underline;
            if (underline != Underline.None)
                RenderUnderline(page, left, top, underline, fontColor, baselineAlignment);
        }
        /// <summary>
        /// Releases unmanaged and - optionally - managed resources.
        /// </summary>
        internal override void Dispose()
        {
            StartIndex = 0;
            Length = 0;
            if (CurrentLineWidget != null)
            {
                if (CurrentLineWidget.Children.Contains(this))
                    CurrentLineWidget.Children.Remove(this);
                CurrentLineWidget = null;
            }
            if (Inline != null)
            {
                if (Inline.Elements.Contains(this))
                    Inline.Elements.Remove(this);
                BaseNode = null;
            }
            RemoveRenderedElements(null);
        }
        #endregion
    }
    internal sealed class ImageElementBox : ElementBox
    {
        #region Properties
        /// <summary>
        /// Gets the image container.
        /// </summary>
        /// <value>
        /// The image container.
        /// </value>
        internal ImageContainerAdv ImageContainer
        {
            get
            {
                return Inline as ImageContainerAdv;
            }
        }
        #endregion

        #region Constructor
        /// <summary>
        /// Initializes a new instance of the <see cref="ImageElementBox" /> class.
        /// </summary>
        /// <param name="inline">The inline.</param>
        internal ImageElementBox(Inline inline)
            : base(inline)
        {
        }
        #endregion

        #region Implementations
        /// <summary>
        /// Renders the specified page.
        /// </summary>
        /// <param name="page">The page.</param>
        /// <param name="left">The left.</param>
        /// <param name="top">The top.</param>
        internal override void Render(PageAdv page, double left, double top)
        {
            Image image = new Image();
            BitmapImage bmp = new BitmapImage();
#if WPF
            bmp.BeginInit();
            bmp.SetSource(new System.IO.MemoryStream(ImageContainer.ImageBytes));
            bmp.EndInit();
#else
            bmp.SetSource(new System.IO.MemoryStream(ImageContainer.ImageBytes));
#endif
            image.Source = bmp;
            double actualWidth = (Inline as ImageContainerAdv).Width;
            image.Width = actualWidth;
            image.Height = Height;
            image.Stretch = Stretch.Fill;
            if (Margin.Top < 0 || Width < actualWidth)
            {
                image.Clip = new RectangleGeometry();
#if WPF
                (image.Clip as RectangleGeometry).Rect = new Rect(0, -Margin.Top, Width, Height);
#else
                image.Clip.Rect = new Rect(0, -Margin.Top, Width, Height);
#endif
            }
            RemoveRenderedElements(page);
            CharacterFormat format = ImageContainer.CharacterFormat;
            if (format.HighlightColor != HighlightColor.NoColor)
                RenderHighlightElement(page, left, top, format);
            RenderedElement = image;
            page.ForegroundContainer.Children.Add(image);
            Canvas.SetLeft(image, left + Margin.Left);
            Canvas.SetTop(image, top + Margin.Top);
        }
        /// <summary>
        /// Releases unmanaged and - optionally - managed resources.
        /// </summary>
        internal override void Dispose()
        {
            if (CurrentLineWidget != null)
            {
                if (CurrentLineWidget.Children.Contains(this))
                    CurrentLineWidget.Children.Remove(this);
                CurrentLineWidget = null;
            }
            if (Inline != null)
            {
                if (Inline.Elements.Contains(this))
                    Inline.Elements.Remove(this);
                BaseNode = null;
            }
            RemoveRenderedElements(null);
        }
        #endregion
    }
    internal sealed class UIElementBox : ElementBox
    {
        #region Properties
        /// <summary>
        /// Gets the UI container.
        /// </summary>
        /// <value>
        /// The UI container.
        /// </value>
        internal UIContainerAdv UIContainer
        {
            get
            {
                return Inline as UIContainerAdv;
            }
        }
        #endregion

        #region Constructor
        /// <summary>
        /// Initializes a new instance of the <see cref="ImageElementBox" /> class.
        /// </summary>
        /// <param name="inline">The inline.</param>
        internal UIElementBox(Inline inline)
            : base(inline)
        {
        }
        #endregion

        #region Implementations
        /// <summary>
        /// Renders the specified page.
        /// </summary>
        /// <param name="page">The page.</param>
        /// <param name="left">The left.</param>
        /// <param name="top">The top.</param>
        internal override void Render(PageAdv page, double left, double top)
        {
            RemoveRenderedElements(page);
            RenderedElement = UIContainer.UIElement as FrameworkElement;
            page.ForegroundContainer.Children.Add(UIContainer.UIElement);
            Canvas.SetLeft(UIContainer.UIElement, left + Margin.Left);
            Canvas.SetTop(UIContainer.UIElement, top + Margin.Top);
        }
        /// <summary>
        /// Releases unmanaged and - optionally - managed resources.
        /// </summary>
        internal override void Dispose()
        {
            if (CurrentLineWidget != null)
            {
                if (CurrentLineWidget.Children.Contains(this))
                    CurrentLineWidget.Children.Remove(this);
                CurrentLineWidget = null;
            }
            if (Inline != null)
            {
                if (Inline.Elements.Contains(this))
                    Inline.Elements.Remove(this);
                BaseNode = null;
            }
            RemoveRenderedElements(null);
        }
        #endregion
    }
    internal sealed class ListTextElementBox : ElementBox
    {
        #region Fields
        internal double BaselineOffset = 0;
        private string text;
        internal ListLevelAdv ListLevel;
        #endregion

        #region Properties
        /// <summary>
        /// Gets or sets the text.
        /// </summary>
        /// <value>
        /// The text.
        /// </value>
        internal string Text
        {
            get
            {
                return text;
            }
            set
            {
                text = value;
                if (RenderedElement is TextBlock)
                    (RenderedElement as TextBlock).Text = text;
            }
        }
        /// <summary>
        /// Gets the paragraph.
        /// </summary>
        /// <value>
        /// The paragraph.
        /// </value>
        internal ParagraphAdv Paragraph
        {
            get
            {
                return BaseNode as ParagraphAdv;
            }
        }
        #endregion

        #region Constructor
        /// <summary>
        /// Initializes a new instance of the <see cref="ListTextElementBox" /> class.
        /// </summary>
        /// <param name="paragraphAdv">The paragraph adv.</param>
        /// <param name="listLevel">The list level.</param>
        internal ListTextElementBox(ParagraphAdv paragraphAdv, ListLevelAdv listLevel)
            : base(paragraphAdv)
        {
            ListLevel = listLevel;
        }
        #endregion

        #region Implementations
        /// <summary>
        /// Renders the specified page.
        /// </summary>
        /// <param name="page">The page.</param>
        /// <param name="left">The left.</param>
        /// <param name="top">The top.</param>
        internal override void Render(PageAdv page, double left, double top)
        {
            TextBlock textBlock = new TextBlock();
            CharacterFormat format = ListLevel.CharacterFormat;
            double fontSize = format.FontSize;
            if (Paragraph.CharacterFormat.ReadLocalValue(CharacterFormat.FontSizeProperty) is double)
                fontSize = Paragraph.CharacterFormat.FontSize;
            FontFamily fontFamily = format.FontFamily;
            if (ListLevel.ReadLocalValue(ListLevelAdv.BulletCharacterProperty) == DependencyProperty.UnsetValue
                && Paragraph.CharacterFormat.ReadLocalValue(CharacterFormat.FontFamilyProperty) is FontFamily)
                fontFamily = Paragraph.CharacterFormat.FontFamily;
            bool bold = format.Bold;
            if (Paragraph.CharacterFormat.ReadLocalValue(CharacterFormat.BoldProperty) is bool)
                bold = Paragraph.CharacterFormat.Bold;
            bool italic = format.Italic;
            if (Paragraph.CharacterFormat.ReadLocalValue(CharacterFormat.ItalicProperty) is bool)
                italic = Paragraph.CharacterFormat.Italic;
            BaselineAlignment baselineAlignment = format.BaselineAlignment;
            if (Paragraph.CharacterFormat.ReadLocalValue(CharacterFormat.BaselineAlignmentProperty) is BaselineAlignment)
                baselineAlignment = Paragraph.CharacterFormat.BaselineAlignment;
            Color fontColor = format.FontColor;
            if (Paragraph.CharacterFormat.ReadLocalValue(CharacterFormat.FontColorProperty) is Color)
                fontColor = Paragraph.CharacterFormat.FontColor;
            //Gets the font color compared to the background color, if font color is empty(auto).
            if (fontColor == Color.FromArgb(0, 0, 0, 0))
            {
                DocumentAdv document = Paragraph.Document;
                fontColor = ListLevel.CharacterFormat.GetDefaultFontColor(document);
            }
            textBlock.Foreground = new SolidColorBrush(fontColor);
            textBlock.FontSize = fontSize / (baselineAlignment == BaselineAlignment.Normal ? 1 : 1.5);
            textBlock.FontFamily = fontFamily;
            if (bold)
                textBlock.FontWeight = FontWeights.Bold;
            if (italic)
                textBlock.FontStyle = FontStyleEnum.Italic;
            textBlock.Text = Text;
            textBlock.Width = Width;
            double topMargin = Margin.Top;
            if (baselineAlignment == BaselineAlignment.Subscript)
                topMargin += Height - Height / 1.5;
            if (topMargin < 0)
            {
                textBlock.Clip = new RectangleGeometry();
#if WPF
                (textBlock.Clip as RectangleGeometry).Rect = new Rect(0, -topMargin, Width, Height);
#else
                textBlock.Clip.Rect = new Rect(0, -topMargin, Width, Height);
#endif
            }
            RemoveRenderedElements(page);
            if (format.HighlightColor != HighlightColor.NoColor)
                RenderHighlightElement(page, left, top, format);
            RenderedElement = textBlock;
            page.ForegroundContainer.Children.Add(textBlock);
            Canvas.SetLeft(textBlock, left + Margin.Left);
            Canvas.SetTop(textBlock, top + topMargin);
            StrikeThrough strikeThrough = format.StrikeThrough;
            if (strikeThrough != StrikeThrough.None)
                RenderStrikeThrough(page, left, top, strikeThrough, fontColor, baselineAlignment);
            Underline underline = format.Underline;
            if (underline != Underline.None)
                RenderUnderline(page, left, top, underline, fontColor, baselineAlignment);
        }
        /// <summary>
        /// Releases unmanaged and - optionally - managed resources.
        /// </summary>
        internal override void Dispose()
        {
            if (CurrentLineWidget != null)
            {
                if (CurrentLineWidget.Children.Contains(this))
                    CurrentLineWidget.Children.Remove(this);
                CurrentLineWidget = null;
            }
            RemoveRenderedElements(null);
            BaseNode = null;
            ListLevel = null;
        }
        #endregion
    }
}
