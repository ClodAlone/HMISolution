#region Copyright Syncfusion Inc. 2001 - 2014
// Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Any infringement will be prosecuted under
// applicable laws. 
#endregion
using System;
using System.Windows;
using System.Linq;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Controls.Primitives;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Windows.Threading;
using System.Threading;

#if !WPF
using System.Windows.Printing;
#else
using System.Drawing.Printing;
using System.Printing;
using System.Windows.Documents;
#endif

using Syncfusion.Windows.Shared;
using System.Windows.Shapes;

namespace Syncfusion.Windows.Tools.Controls
{
    /// <summary>
    /// 
    /// </summary>
    /// <param name="imageResizer"></param>
    /// <param name="imageContainer"></param>
    internal delegate void DragCompletedCallBack(ImageResizer imageResizer, ImageContainerAdv imageContainer);

    internal class Worker
    {
        ParagraphAdv paragraph;
        ManualResetEvent evt;
        internal Worker(ParagraphAdv para, ManualResetEvent e)
        {
            paragraph = para;
            evt = e;
        }

        internal void RunTask(object state)
        {
            try
            {
                if (paragraph != null)
                {
                    paragraph.MeasureElements();
                    paragraph.LinkElementBoxes();
                }
            }
            finally
            {
                evt.Set();
            }
        }
    }

#if WPF
    public class PaginatorAdv : DocumentPaginator
    {

        LayoutViewer m_Viewer;
        Size m_PageSize;

        public PaginatorAdv(LayoutViewer viewer,Size pagesize)
        {
            m_Viewer = viewer;
            m_PageSize = pagesize;
        }

        public override DocumentPage GetPage(int pageNumber)
        {
            if (m_Viewer != null)
            {
                if (m_Viewer.Pages.Count > 0)
                {
                    PageAdv page = m_Viewer.Pages[pageNumber];
                    page.Measure(m_PageSize);
                    page.Arrange(new Rect(new Point(0.0, 0.0), m_PageSize));
                    return new DocumentPage(page);
                }
            }
            return null;
        }

        public override bool IsPageCountValid
        {
            get { return true; }
        }

        public override int PageCount
        {
            get 
            {
                return m_Viewer != null ? m_Viewer.Pages.Count : 0;
            }
        }

        public override Size PageSize
        {
            get
            {
                return m_PageSize;
            }
            set
            {
                m_PageSize = value;
            }
        }

        public override IDocumentPaginatorSource Source
        {
            get { return null; }
        }
    }
#endif

    public abstract class LayoutViewer : ContentControl
    {
        internal bool NeedRecreateLayout = true;
        internal CaretAdv Caret;
        internal ParagraphAdv CurrentParagraph;
        internal LineInfo CurrentLine;
        private PageAdv currentPage;
        internal DocumentAdv Document;
        internal Size AvailableSize;
        internal RichTextBoxAdv OwnerControl;
        internal bool OnLoading = false;
        internal double UpDownSelectionWidth = double.NaN;
        internal bool UseUpDownSelection = false;
        private List<PageAdv> pages;
        internal ImageResizer ImageResizer;
        internal bool isMousedown = false;
        internal bool isPageActive = false;
        bool isDoubleClick = false;
        private bool isMouseInsideSelectedLines = false;
        bool isTripleClick = false;
        bool isSingleClick = false;
        private DispatcherTimer dragTimer = null;
        internal bool isDragging = false;
        private bool isDropping = false;
        private DispatcherTimer doubleClickTimer = null;
        private DispatcherTimer scrollingTimer = null;
        private ObservableCollection<LineInfo> lineInfos;
        private int printedPagesCount = 0;
        internal double transX = 1;
        internal double transY = 1;
        internal Size OriginalSize = Size.Empty;
        protected ScaleTransform scaleTransform = null;
        private ImageContainerAdv selectedImage = null;
        LayoutViewer layoutViewer = null;
        internal bool IsPrinting = false;
        internal TableCellElementBox m_parentcell = null;
        internal int startPage = 0;
        internal int endPage = 0;
        private bool imageselected=false;
        private bool m_Printflag = false;
#if WPF
        private PrintDialog m_printDialog;
#endif
#if WPF && (SyncfusionFramework4_0 || SyncfusionFramework4_5 || SyncfusionFramework4_5_1)
        internal bool isTouchDownOnSelectionMark = false;
        internal bool isTouchInsideSelection = false;
#endif

        /// <summary>
        /// Gets or Sets the visible bounds
        /// </summary>
        internal Rect Visiblebounds;

        internal ImageContainerAdv SelectedImage
        {
            get
            {
                return selectedImage;
            }
            set
            {
                selectedImage = value;
            }
        }

        internal bool IsImageResizerSelected
        {
            get
            {
                return imageselected;
            }
            set
            {
                imageselected = value;
                if (!value)
                {
                    SelectedImage = null;
                    ImageResizer.Visibility = Visibility.Collapsed;
                }
            }
        }
              
        internal ObservableCollection<LineInfo> LineInfos
        {
            get
            {
                return lineInfos;
            }
            set
            {
                lineInfos = value;
            }
        }

        internal InlineStyle CurrentInlineStyle
        {
            get
            {
                if (OwnerControl != null)
                {
                    return OwnerControl.CurrentInlineStyle;
                }

                return null;
            }
        }

        internal ParagraphStyle CurrentParagraphStyle
        {
            get
            {
                if (OwnerControl != null)
                {
                    return OwnerControl.CurrentParagraphStyle;
                }

                return null;
            }
        }

        /// <summary>
        /// Gets the No of Pages in the LayoutViewer.
        /// </summary>
        public List<PageAdv> Pages
        {
            get
            {
                return pages;
            }
            internal set
            {
                pages = value;
            }
        }

        public bool IsSelected
        {
            get
            {
                if (Pages.Count > 0)
                    return Pages.Where(page => page.IsSelected).Count() > 0 || SelectedImage != null;
                return false;
            }
            internal set
            {
                if (value)
                {
                    AddSelections();
#if !WPF 
                    OwnerControl.CanExecuteCommands();
#endif               
                }
                else
                {
                    OwnerControl.Selection.Start = TextPosition.Copy();
                    OwnerControl.Selection.End = null;
                    RemoveSelections();
                    SelectedImage = null;
                    OwnerControl.Viewer.IsImageResizerSelected = false;
                    ImageResizer.Visibility = Visibility.Collapsed;
                    OwnerControl.Selection.IsCellSelected = false;
                }
            }
        }

        /// <summary>
        /// Gets the CurrentPage where Cursor available.
        /// </summary>
        public PageAdv CurrentPage
        {
            get
            {
                return currentPage;
            }
            internal set
            {
                if (currentPage != null && value != currentPage)
                    currentPage.HideCaret(false);
                currentPage = value;
                OwnerControl.CurrentPage = currentPage;
            }
        }
        
        public bool IsReadOnly
        {
            get
            {
                return OwnerControl.IsReadOnly;
            }
        }

        /// <summary>
        /// Gets or Sets the TextPosition
        /// </summary>
        public TextPosition TextPosition
        {
            get
            {
                if (OwnerControl != null && OwnerControl.PositionHandler != null)
                    return OwnerControl.PositionHandler.TextPosition;
                return null;
            }
            internal set
            {
                if (OwnerControl != null && OwnerControl.PositionHandler != null)
                    OwnerControl.PositionHandler.TextPosition = value;
            }
        }

        internal ScrollBar HorizontalScrollBar
        {
            get;
            set;
        }

        internal ScrollBar VerticalScrollBar
        {
            get;
            set;
        }

        internal RenderingManager RenderingManager
        {
            get
            {
                return OwnerControl.RenderingManager;
            }
        }

        /// <summary>
        /// Initializes the new instance of Layout viewer
        /// </summary>
        public LayoutViewer()
        {

        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        internal abstract void FindFocusedPage(MouseEventArgs e);
#if WPF && (SyncfusionFramework4_0 || SyncfusionFramework4_5 || SyncfusionFramework4_5_1)
        /// <summary>
        /// Finds the focused page.
        /// </summary>
        /// <param name="e">The <see cref="TouchEventArgs" /> instance containing the event data.</param>
        internal abstract void FindFocusedPage(TouchEventArgs e);
#endif

        public bool IsZoomEnabled
        {
            get
            {
                return OwnerControl.IsZoomEnabled;
            }
        }

        public int? CurrentPageNumber
        {
            get
            {
                return (int?)GetValue(CurrentPageNumberProperty);
            }
            internal set
            {
                SetValue(CurrentPageNumberProperty, value);
            }
        }

        // Using a DependencyProperty as the backing store for CurrentPageNumber.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty CurrentPageNumberProperty =
            DependencyProperty.Register("CurrentPageNumber", typeof(int?), typeof(LayoutViewer), new PropertyMetadata(null));
        
        /// <summary>
        /// 
        /// </summary>
        /// <param name="richTextBox"></param>
        public LayoutViewer(RichTextBoxAdv richTextBox)
        {
            OwnerControl = richTextBox;
#if WPF
            //Handled specifically to avoid focus, on Tab navigation.
            Focusable = false;
#endif
            scaleTransform = new ScaleTransform();
            Pages = new List<PageAdv>();
            doubleClickTimer = new DispatcherTimer();
            doubleClickTimer.Interval = new TimeSpan(0, 0, 0, 0, 200);
            doubleClickTimer.Tick += new EventHandler(doubleClickTimer_Tick);
            dragTimer = new DispatcherTimer();
            dragTimer.Interval = new TimeSpan(0, 0, 0, 0, 500);
            dragTimer.Tick += new EventHandler(dragTimer_Tick);
            MouseLeftButtonDown += new MouseButtonEventHandler(LayoutViewer_MouseLeftButtonDown);
            MouseRightButtonDown += new MouseButtonEventHandler(LayoutViewer_MouseRightButtonDown);
            MouseLeftButtonUp += new MouseButtonEventHandler(LayoutViewer_MouseLeftButtonUp);
            MouseMove += new MouseEventHandler(LayoutViewer_MouseMove);
#if WPF && (SyncfusionFramework4_0 || SyncfusionFramework4_5 || SyncfusionFramework4_5_1)
            TouchDown += new EventHandler<TouchEventArgs>(LayoutViewer_TouchDown);
            TouchMove += new EventHandler<TouchEventArgs>(LayoutViewer_TouchMove);
#endif
            scrollingTimer = new DispatcherTimer();
            scrollingTimer.Interval = new TimeSpan(0, 0, 0, 0, 100);
            scrollingTimer.Tick += new EventHandler(scrollingTimer_Tick);
            ImageResizer = new ImageResizer() { DragCompletedCallBack = new DragCompletedCallBack(DragCompleted), OwnerControl = richTextBox };
        }

        public void ResetZooming()
        {
            transX = 1;
            transY = 1;
            Zoom();
        }

        internal virtual void Zoom()
        {

        }

        internal void UpdateCurrentPageNumber()
        {
            if (this != null && this.Pages !=null)
            {
                for (int p = 0; p < this.Pages.Count; p++)
                {
                    PageAdv page = this.Pages[p];
                    if (page != null && this.CurrentPage != null && page == this.CurrentPage)
                    {
                        CurrentPageNumber = p + 1;
                        break;
                    }
                }
            }
        }


        public void PrintDocument()
        {
#if !WPF
            PrintDocument document = new PrintDocument();
            document.PrintPage += new EventHandler<PrintPageEventArgs>(document_PrintPage);
            document.EndPrint += new EventHandler<EndPrintEventArgs>(document_EndPrint);
            document.BeginPrint += new EventHandler<BeginPrintEventArgs>(document_BeginPrint);
            document.Print("Print Document");
#else
            m_printDialog = new PrintDialog();
            if ((bool)m_printDialog.ShowDialog())
            {
                PreparePrinting();
                RoutedEventArgs args = new RoutedEventArgs();
                OwnerControl.FirePrinting(args);
                if (!args.Handled && m_Printflag)
                {
                    if (layoutViewer.Pages.Count != 0)
                    {
                        if (this is FlowLayoutViewer && layoutViewer is PageLayoutViewer)
                        {
                            PageAdv page = layoutViewer.Pages[printedPagesCount];
                            if (page.Parent != null && page.Parent is Canvas)
                            {
                                (page.Parent as Canvas).Children.Remove(page);
                            }
                        }

                        PrintCapabilities printcablities = m_printDialog.PrintQueue.GetPrintCapabilities(m_printDialog.PrintTicket);

                        Size size = new Size(printcablities.PageImageableArea.ExtentWidth, printcablities.PageImageableArea.ExtentHeight);
                        var paginator = new PaginatorAdv(layoutViewer, size);
                        m_printDialog.PrintDocument(paginator, "Print Document");
                        m_Printflag = false;
                    }
                    if (!m_Printflag)
                    {
                        CompletePrinting();
                    }
                }
            }
#endif
        }
        
        internal void PreparePrinting()
        {
            layoutViewer = this;
            if (this is FlowLayoutViewer)
            {
                layoutViewer = OwnerControl.ChangePageLayoutForPrinting(true);
            }


            PageAdv current = layoutViewer.Pages[printedPagesCount];

            AddAllLinesToPage(true);
            CollapseSelectionsForPrinting();
            m_Printflag = true;
        }

        internal void CompletePrinting()
        {
            printedPagesCount = 0;
#if !WPF
            RemoveAllLinesFromPage();
#endif
            AddSelectionsForPrinting();
            if (this is PageLayoutViewer)
            {
                SetVisibleLinesToPage();
                PositionCursor();
            }
            else
            {
                OwnerControl.ChangePageLayoutForPrinting(false);
            }

            if (layoutViewer != null)
            {
                layoutViewer.IsPrinting = false;
                layoutViewer = null;
            }
            if (!m_Printflag)
            {
                OwnerControl.FirePrinted();
            }
        }
        
#if !WPF
        void document_BeginPrint(object sender, BeginPrintEventArgs e)
        {
            layoutViewer = this;
            if (this is FlowLayoutViewer)
            {
                layoutViewer = OwnerControl.ChangePageLayoutForPrinting(true);
            }

            AddAllLinesToPage(true);
            CollapseSelectionsForPrinting();
        }
               
        void document_EndPrint(object sender, EndPrintEventArgs e)
        {
            printedPagesCount = 0;
            (sender as PrintDocument).PrintPage -= new EventHandler<PrintPageEventArgs>(document_PrintPage);
            (sender as PrintDocument).EndPrint -= new EventHandler<EndPrintEventArgs>(document_EndPrint);
            (sender as PrintDocument).BeginPrint -= new EventHandler<BeginPrintEventArgs>(document_BeginPrint);
            RemoveAllLinesFromPage();
            AddSelectionsForPrinting();
            if (this is PageLayoutViewer)
            {
                SetVisibleLinesToPage();
                PositionCursor();
            }
            else
            {
                OwnerControl.ChangePageLayoutForPrinting(false);
            }

            if (layoutViewer != null)
            {
                layoutViewer.IsPrinting = false;
                layoutViewer = null;
            }

            if (e.Error == null)
                MessageBox.Show("Document is Printed");
        }
        void document_PrintPage(object sender, PrintPageEventArgs e)
        {
            if (layoutViewer.Pages.Count != 0)
            {
                if (this is FlowLayoutViewer && layoutViewer is PageLayoutViewer)
                {
                    PageAdv page = layoutViewer.Pages[printedPagesCount];
                    if (page.Parent != null && page.Parent is Canvas)
                    {
                        (page.Parent as Canvas).Children.Remove(page);
                    }
                }
                e.PageVisual = layoutViewer.Pages[printedPagesCount];  
                printedPagesCount++;
            }
            if (layoutViewer.Pages.Count > printedPagesCount)
                e.HasMorePages = true;
            else
                e.HasMorePages = false;
        }
        
#endif

        public PageAdv SkipPage(int nextPageindex)
        {
            PageAdv page = null;
            if (nextPageindex < Pages.Count)
            {
                layoutViewer = this;
                if (this is FlowLayoutViewer)
                {
                    layoutViewer = OwnerControl.ChangePageLayoutForPrinting(true);
                }

                PageAdv next = Pages[nextPageindex];
                page = new PageAdv();
                page.Height = next.Height;
                page.Width = next.Width;

                foreach (LineInfo line in next.LineInfos)
                {
                    AddLineToPage(line, page);
                }

                CollapseSelectionForPage(page);

                return page;
            }
            return page;
        }

        private void CollapseSelectionForPage(PageAdv page)
        {
            if (SelectedImage != null)
                ImageResizer.Visibility = Visibility.Collapsed;

            if (page.SelectionPath != null)
                page.SelectionPath.Visibility = Visibility.Collapsed;

            page.DecreaseThickness();

            page.CollapseBorder(true);
        }

        internal void CollapseSelectionsForPrinting()
        {
            if (SelectedImage != null)
                ImageResizer.Visibility = Visibility.Collapsed;

            foreach (PageAdv page in Pages)
            {
                CollapseSelectionForPage(page);
            }
        }

        internal void AddSelectionsForPrinting()
        {
            if (layoutViewer != null)
            {
                if (layoutViewer.SelectedImage != null)
                    layoutViewer.ImageResizer.Visibility = Visibility.Visible;

                foreach (PageAdv page in layoutViewer.Pages)
                {
                    if (page.SelectionPath != null)
                        page.SelectionPath.Visibility = Visibility.Visible;
                    page.IncreaseThickness();

                    page.CollapseBorder(false);
                }
            }
        }

        internal void AddAllLinesToPage(bool collapseCursor)
        {
            if (layoutViewer != null)
            {
                foreach (PageAdv page in layoutViewer.Pages)
                {
                    if (collapseCursor)
                        page.Caret.Visibility = Visibility.Collapsed;

                    foreach (LineInfo line in page.LineInfos)
                    {
#if !WPF
                        if (line.TextRenderers.Count == 0 && line.DecoratingElements.Count == 0)
#endif
                            AddLineToPage(line, page);
                    }

#if WPF
                    Canvas canvas = (Canvas)Content;
                    if (canvas != null)
                    {
                        if (!canvas.Children.Contains(page))
                        {
                            canvas.Children.Add(page);
                        }
                    }
#endif
                }
            }
        }

        internal void RemoveAllLinesFromPage()
        {
            if (layoutViewer != null)
            {
                foreach (PageAdv page in layoutViewer.Pages)
                {
                    foreach (LineInfo line in page.LineInfos)
                    {
                        RemoveLineFromPage(line, page);
                    }
                }
            }
        }

        /// <summary>
        /// Measures the child elements
        /// </summary>
        /// <param name="availableSize"></param>
        /// <returns></returns>
        protected override Size MeasureOverride(Size availableSize)
        {
            AvailableSize = availableSize;
            OriginalSize = availableSize;

#if !WPF
            if (VerticalScrollBar != null)
                VerticalScrollBar.Value = 0;
#endif

            if (Document == null)
            {
                OwnerControl.Document = new DocumentAdv();
            }

            if (Document.Sections.Count == 0)
            {
                SectionAdv section = new SectionAdv();
                section.Document = OwnerControl.Document;
                OwnerControl.Document.Sections.Add(section);
            }

            if (Document.Sections[0].Blocks.Count == 0)
            {
                ParagraphAdv paragraph = new ParagraphAdv();
                Document.Sections[0].Blocks.Add(paragraph);
            }
            if (NeedRecreateLayout)
            {
                foreach (SectionAdv section in Document.Sections)
                {
                    section.Document = Document;
                    foreach (BlockAdv block in section.Blocks)
                    {
                        block.Section = section;
                        block.LayoutViewer = this;
                        block.Margin = section.PageContentMargin;
                        block.MeasureElements();
                        block.LinkElementBoxes();
                    }
                }
                OnLoading = true;

                NeedRecreateLayout = false;
            }

            SetPreviousBlocks();

            ArrangeElements();

            if (double.IsInfinity(availableSize.Height))
            {
                double height = this.LineInfos.Last().BoundingRectangle.Bottom;
                height = this.OwnerControl.MinHeight > height ? this.OwnerControl.MinHeight : height;
                AvailableSize = new Size(availableSize.Width, height);
                OriginalSize = AvailableSize;
            }


            if (!IsPrinting)
                SetVisibleLinesToPage();

            OnLoading = false;

            if (!IsPrinting)
            {

                if (OwnerControl != null && OwnerControl.PositionHandler == null)
                {
                    OwnerControl.PositionHandler = new DocumentPositionHandler(Document);
                }

                OwnerControl.PositionHandler.PositionCursor();

                OwnerControl.Selection.UpdateSelection();
            }

            return base.MeasureOverride(AvailableSize);
        }

        /// <summary>
        /// Removes the selection from each page
        /// </summary>
        public void RemoveSelections()
        {
            if (Pages.Count > 0)
            {
                Pages.ForEach(page =>
                {
                    if (page.IsSelected)
                    {
                        page.IsSelected = false;
                    }
                });
            }
        }

        /// <summary>
        /// Create paragraph when there the control is empty
        /// </summary>
        internal void CreateBlockOnEmpty()
        {
            OwnerControl.CurrentParagraphStyle.SetDefaultStyle();
            OwnerControl.CurrentInlineStyle.SetDefaultStyle();
            if (Document == null)
            {
                OwnerControl.Document = new DocumentAdv();
            }

            if (Document.Sections.Count == 0)
            {
                SectionAdv section = new SectionAdv();
                section.Document = OwnerControl.Document;
                OwnerControl.Document.Sections.Add(section);
            }

            if (Document.Sections[0].Blocks.Count == 0)
            {
                BlockAdv block = new ParagraphAdv();
                block.LayoutViewer = this;
                block.Section = Document.Sections[0];
                Document.Sections[0].Blocks.Add(block);
            }
        }

        /// <summary>
        /// Returns the first paragraph in the Document
        /// </summary>
        /// <returns></returns>
        internal BlockAdv GetFirstBlock()
        {
            if (Document != null && Document.Sections.Count != 0
                && Document.Sections[0].Blocks.Count != 0)
            {
                return Document.Sections[0].Blocks[0];
            }

            return null;
        }

        /// <summary>
        /// Adds the selection to the page
        /// </summary>
        public void AddSelections()
        {
            if (Pages.Count > 0)
            {
                Pages.ForEach(page =>
                {
                    page.IsSelected = true;
                    if (page.Caret != null && page.IsSelected)
                    {
                        page.HideCaret(false);
                    }
                });
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="line"></param>
        /// <returns></returns>
        internal PageAdv GetPageFromLine(LineInfo line)
        {
            PageAdv page = null;
            LineInfo parentline = null;
            if (line != null)
            {
                foreach (PageAdv pg in Pages)
                {
                    if (line.Block != null)
                    {
                        if (!line.Block.IsInsideTable)
                        {
                            if (pg.LineInfos.Contains(line))
                            {
                                page = pg;
                                break;
                            }
                        }
                        else
                        {
                            parentline = line.Block.AssociatedCell.CellElementBox.LineInfo;
                            //if (line.Block.AssociatedCell.HasRowSpan())
                            //{
                            //    TableCellElementBox eBox = line.Block.AssociatedCell.CellElementBox.BottomCellBox;
                            //    while (eBox != null)
                            //    {
                            //        if (eBox.LineInfos.Contains(line))
                            //        {
                            //            parentline = eBox.LineInfo;
                            //            break;
                            //        }
                            //        else
                            //            eBox = eBox.BottomCellBox;
                            //    }
                            //}
                            TableCellElementBox cellBox = line.Block.AssociatedCell.CellElementBox;

                            if (parentline.IsSplitted)
                            {
                                cellBox = line.Block.AssociatedCell.CellElementBox.ChildBox;

                                while (cellBox != null)
                                {
                                    if (cellBox.LineInfos.Contains(TextPosition.LineInfo))
                                    {
                                        parentline = cellBox.LineInfo;
                                        break;
                                    }
                                    cellBox = cellBox.ChildBox;
                                }
                            }
                            else if (line.Block.AssociatedCell.HasRowSpan())
                            {
                                cellBox = line.Block.AssociatedCell.CellElementBox.BottomCellBox;

                                while (cellBox != null)
                                {
                                    if (cellBox.LineInfo.IsSplitted)
                                    {
                                        cellBox = cellBox.ChildBox;

                                        while (cellBox != null)
                                        {
                                            if (cellBox.LineInfos.Contains(TextPosition.LineInfo))
                                            {
                                                parentline = cellBox.LineInfo;
                                                break;
                                            }
                                            cellBox = cellBox.ChildBox;
                                        }
                                    }
                                    if (cellBox != null)
                                    {
                                        if (cellBox.LineInfos.Contains(TextPosition.LineInfo))
                                        {
                                            parentline = cellBox.LineInfo;
                                            break;
                                        }
                                        cellBox = cellBox.BottomCellBox;
                                    }
                                }
                            }

                            if (parentline != null)
                            {
                                while (parentline.Block.AssociatedCell != null)
                                {
                                    parentline = parentline.Block.AssociatedCell.CellElementBox.LineInfo;
                                }
                                if (pg.LineInfos.Contains(parentline))
                                {
                                    page = pg;
                                    break;
                                }
                            }
                        }
                    }
                }

            }

            return page;
        }

        internal PageAdv GetNextPage(PageAdv page)
        {
            if (page != null && Pages.Contains(page))
            {
                int index = Pages.IndexOf(page) + 1;
                if (index != Pages.Count)
                    return Pages[index];
            }
            return null;
        }

        internal PageAdv GetPreviousPage(PageAdv page)
        {
            if (page != null && Pages.Contains(page))
            {
                int index = Pages.IndexOf(page) - 1;
                if (index >= 0)
                    return Pages[index];
            }
            return null;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void dragTimer_Tick(object sender, EventArgs e)
        {
            isDragging = true;
        }

#if WPF && (SyncfusionFramework4_0 || SyncfusionFramework4_5 || SyncfusionFramework4_5_1)
        /// <summary>
        /// Handles the TouchDown event of the LayoutViewer control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="TouchEventArgs" /> instance containing the event data.</param>
        void LayoutViewer_TouchDown(object sender, TouchEventArgs e)
        {
            if (ImageResizer != null)
            {
                ImageResizer.Visibility = Visibility.Collapsed;
                SelectedImage = null;
                IsImageResizerSelected = false;
            }
            UseUpDownSelection = false;
            UpDownSelectionWidth = double.NaN;
            CaptureMouse();
            isMousedown = true;
            isPageActive = true;
            FindFocusedPage(e);
            OwnerControl.Focus();
            isTouchDownOnSelectionMark = false;
            isTouchInsideSelection = false;

            if (!IsSelected && CurrentPage != null && CurrentPage.Caret != null)
            {
                Point point = e.GetTouchPoint(CurrentPage.ForegroundContainer).Position;
                ElementBox box = CurrentPage.GetElementBox(point);
                if (box is TableCellElementBox)
                {
                    LineInfo line = (box as TableCellElementBox).GetLineFromPoint(point);
                    if (line != null)
                    {
                        box = line.GetElementBoxFromPoint(point);
                    }
                }
                if (box != null && box.IsImageBox)
                    (box.Inline as ImageContainerAdv).SelectElement();
            }

            bool isInside = false;
            if (IsSelected)
            {
                isInside = IsMouseInsideSelectedLines(e);

                if (isInside && !isTripleClick && !isDoubleClick && CurrentPage != null)
                {
                    isDragging = true;
                    isMouseInsideSelectedLines = true;
                    isTouchInsideSelection = true;
                    CurrentPage.Cursor = Cursors.Arrow;
                }
                else if (CurrentPage != null)
                {
                    isMouseInsideSelectedLines = false;
                    IsSelected = false;
                    CurrentPage.Cursor = Cursors.IBeam;
                }
            }

            if (CurrentPage != null)
            {
                Rect startRect = new Rect(Canvas.GetLeft(CurrentPage.TouchStart), Canvas.GetTop(CurrentPage.TouchStart), 15, 15);
                Rect endRect = new Rect(Canvas.GetLeft(CurrentPage.TouchEnd), Canvas.GetTop(CurrentPage.TouchEnd), 15, 15);
                Point touchPoint = e.GetTouchPoint(CurrentPage.ForegroundContainer).Position;
                if (e.OriginalSource == CurrentPage.TouchStart || e.OriginalSource == CurrentPage.TouchEnd
                    || startRect.Contains(touchPoint) || endRect.Contains(touchPoint))
                    isTouchDownOnSelectionMark = true;
                else if(!isInside)
                {
                    //Collapses the visibility of Touch start and end marks.
                    CurrentPage.TouchStart.Visibility = Visibility.Collapsed;
                    CurrentPage.TouchEnd.Visibility = Visibility.Collapsed;
                }
                OwnerControl.Selection.StartingPage = Pages.IndexOf(CurrentPage);
                CurrentPage.UpdateCaretPosition(touchPoint);
                OwnerControl.Selection.StartPoint = new Point(CurrentPage.Caret.Location.X, CurrentPage.Caret.Location.Y + CurrentPage.Caret.Height / 2);
                if (!isInside)
                    OwnerControl.Selection.Start = OwnerControl.PositionHandler.TextPosition.Copy();
            }
            isSingleClick = true;
            isDragging = false;
            isDropping = false;            
            CheckForCursorVisibility(true);
            if (CurrentPage != null && !isTouchInsideSelection)
                CurrentPage.ShowTouchMark();
        }

        /// <summary>
        /// Handles the TouchMove event of the LayoutViewer control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="TouchEventArgs" /> instance containing the event data.</param>
        void LayoutViewer_TouchMove(object sender, TouchEventArgs e)
        {
            if (isTouchDownOnSelectionMark && !isDragging)
            {
                if (CurrentPage != null)
                {
                    CurrentPage.TouchStart.Visibility = Visibility.Collapsed;
                    CurrentPage.TouchEnd.Visibility = Visibility.Collapsed;
                }
                FindFocusedPage(e);
                if (CurrentPage != null)
                {
                    CurrentPage.UpdateCaretPosition(e.GetTouchPoint(CurrentPage.ForegroundContainer).Position);
                    OwnerControl.Selection.EndingPage = Pages.IndexOf(CurrentPage);
                    OwnerControl.Selection.EndPoint = new Point(CurrentPage.Caret.Location.X, CurrentPage.Caret.Location.Y + CurrentPage.Caret.Height / 2);
                    OwnerControl.Selection.End = OwnerControl.PositionHandler.TextPosition.Copy();
                    startPage = OwnerControl.Selection.StartingPage;
                    endPage = OwnerControl.Selection.EndingPage;
                    for (int i = 0; i < Pages.Count; i++)
                    {
                        if (i == startPage || i == endPage || (startPage < i && endPage > i) || (startPage > i && endPage < i))
                        {
                            Pages[i].SelectedLines = Pages[i].GenerateLineInfoForSelection();
                        }
                        else
                        {
                            Pages[i].IsSelected = false;
                        }
                    }
                    Point startPoint = OwnerControl.Selection.StartPoint;
                    Point endPoint = OwnerControl.Selection.EndPoint;
                    if (OwnerControl.Selection.StartingPage == OwnerControl.Selection.EndingPage)
                    {
                        if ((OwnerControl.Selection.StartPoint.Y > OwnerControl.Selection.EndPoint.Y && CurrentPage.SelectedLines.Count != 1)
                            || ((OwnerControl.Selection.StartPoint.Y == OwnerControl.Selection.EndPoint.Y || CurrentPage.SelectedLines.Count == 1)
                            && OwnerControl.Selection.StartPoint.X > OwnerControl.Selection.EndPoint.X))
                        {
                            startPoint = OwnerControl.Selection.EndPoint;
                            endPoint = OwnerControl.Selection.StartPoint;
                        }
                    }
                    else if (OwnerControl.Selection.StartingPage > OwnerControl.Selection.EndingPage)
                    {
                        startPoint = OwnerControl.Selection.EndPoint;
                        endPoint = OwnerControl.Selection.StartPoint;
                        startPage = OwnerControl.Selection.EndingPage;
                        endPage = OwnerControl.Selection.StartingPage;
                    }
                    if (Pages.Count < endPage)
                    {
                        OwnerControl.Selection.StartingInline = Pages[startPage].GetInlineFromPoint(startPoint);
                        OwnerControl.Selection.StartingIndex = Pages[startPage].GetIndexFromPoint(startPoint);
                    }
                    if (Pages.Count < startPage)
                    {
                        OwnerControl.Selection.EndingInline = Pages[endPage].GetInlineFromPoint(endPoint);
                        OwnerControl.Selection.EndingIndex = Pages[endPage].GetIndexFromPoint(endPoint);
                    }
                    foreach (PageAdv page in Pages)
                    {
                        page.AddSelectionPath();
                    }

                    if (Pages.Where(page => page.IsSelected).Count() > 0 || SelectedImage != null)
                    {
                        SelectionChangedEventArgs args = new SelectionChangedEventArgs();
                        args.StartPosition = OwnerControl.Selection.Start;
                        args.EndPosition = OwnerControl.Selection.End;
                        args.SelectedText = OwnerControl.Selection.Text;
                        OwnerControl.FireSelectionChanged(args);
                    }
                    OwnerControl.Selection.ExtractSelectedText();
                    if (OwnerControl.Selection.StartPoint != OwnerControl.Selection.EndPoint
                        || OwnerControl.Selection.StartingPage != OwnerControl.Selection.EndingPage)
                    {
                        Pages[OwnerControl.Selection.StartingPage].SetTouchMarkAtSelectionStart();
                        Pages[OwnerControl.Selection.EndingPage].SetTouchMarkAtSelectionEnd();
                    }
                }
            }
            if (OwnerControl.Selection.Start != null && OwnerControl.Selection.Start.IsEqual(OwnerControl.Selection.End) && !OwnerControl.Selection.IsCellSelected)
            {
                IsSelected = false;
            }
        }
#endif

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void LayoutViewer_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            if (ImageResizer != null)
            {
                ImageResizer.Visibility = Visibility.Collapsed;
                SelectedImage = null;
                IsImageResizerSelected = false;
            }
            UseUpDownSelection = false;
            UpDownSelectionWidth = double.NaN;
#if WPF
            CaptureMouse();
#endif
            isMousedown = true;
            isPageActive = true;
#if SILVERLIGHT
            CaptureMouse();
#endif
            FindFocusedPage(e);
            OwnerControl.Focus();
            bool isInside = false;

            if (!IsSelected && CurrentPage != null && CurrentPage.Caret != null)
            {
                Point point = e.GetPosition(CurrentPage.ForegroundContainer);
                ElementBox box = CurrentPage.GetElementBox(point);
                if (box is TableCellElementBox)
                {
                    LineInfo line = (box as TableCellElementBox).GetLineFromPoint(point);
                    if (line != null)
                    {
                        box = line.GetElementBoxFromPoint(point);
                    }
                }
                if (box != null && box.IsImageBox)
                    (box.Inline as ImageContainerAdv).SelectElement();
            }

            if (IsSelected)
            {
                isInside = IsMouseInsideSelectedLines(e);

                if (isInside && !isTripleClick && !isDoubleClick && CurrentPage != null)
                {
                    isDragging = true;
                    isMouseInsideSelectedLines = true;
                    CurrentPage.Cursor = Cursors.Arrow;
                }
                else if (CurrentPage != null)
                {
                    isMouseInsideSelectedLines = false;
                    IsSelected = false;
                    CurrentPage.Cursor = Cursors.IBeam;
                }
            }

            if (CurrentPage != null)
            {
                OwnerControl.Selection.StartingPage = Pages.IndexOf(CurrentPage);
                CurrentPage.UpdateCaretPosition(e);
                OwnerControl.Selection.StartPoint = new Point(CurrentPage.Caret.Location.X, CurrentPage.Caret.Location.Y + CurrentPage.Caret.Height / 2);
                if (!isInside)
                    OwnerControl.Selection.Start = OwnerControl.PositionHandler.TextPosition.Copy();
            }

#if WPF
            if (e.ClickCount > 2)
            {
                IsSelected = false;
                bool canexecute = false;

                if (TextPosition.IsInsideTable && TextPosition.Paragraph != null && TextPosition.Paragraph.AssociatedCell != null)
                {
                    TableCellAdv cell = TextPosition.Paragraph.AssociatedCell;
                    if (TextPosition.Paragraph == (ParagraphAdv)cell.Blocks.Last())
                    {
                        OwnerControl.PositionHandler.SelectCell(cell);
                    }
                    else
                        canexecute = true;
                }
                else
                    canexecute = true;

                if (canexecute)
                {
                    TextPosition startpos = new TextPosition(Document);
                    startpos.Paragraph = CurrentParagraph;
                    startpos.SetIndex("0");
                    OwnerControl.Selection.Start = startpos;

                    TextPosition endpos = new TextPosition(Document);
                    endpos.Paragraph = CurrentParagraph;
                    endpos.SetIndex(CurrentParagraph.Length());
                    OwnerControl.Selection.End = endpos;

                    OwnerControl.Selection.Select();
                }
                PositionCursor();
                isSingleClick = false;
            }
            else if (e.ClickCount == 2)
            {
                IsSelected = false;
                isSingleClick = false;
                OwnerControl.PositionHandler.SelectWord();
            }
            if (e.ClickCount == 1)
            {
                isSingleClick = true;
                isDragging = false;
                isDropping = false;
            }
#else
            if (CurrentParagraph != null && doubleClickTimer.IsEnabled)
            {
                if (isTripleClick)
                {
                    IsSelected = false;
                    bool canexecute = false;

                    if (TextPosition.IsInsideTable && TextPosition.Paragraph != null && TextPosition.Paragraph.AssociatedCell != null)
                    {
                        TableCellAdv cell = TextPosition.Paragraph.AssociatedCell;
                        if (TextPosition.Paragraph == (ParagraphAdv)cell.Blocks.Last())
                        {
                            OwnerControl.PositionHandler.SelectCell(cell);
                        }
                        else
                            canexecute = true;
                    }
                    else
                        canexecute = true;

                    if (canexecute)
                    {
                        TextPosition startpos = new TextPosition(Document);
                        startpos.Paragraph = CurrentParagraph;
                        startpos.SetIndex("0");
                        OwnerControl.Selection.Start = startpos;

                        TextPosition endpos = new TextPosition(Document);
                        endpos.Paragraph = CurrentParagraph;
                        endpos.SetIndex(CurrentParagraph.Length());
                        OwnerControl.Selection.End = endpos;

                        OwnerControl.Selection.Select();
                    }
                    PositionCursor();
                }
                if (isDoubleClick)
                {
                    IsSelected = false;
                    OwnerControl.PositionHandler.SelectWord();
                }

                isTripleClick = isDoubleClick;
                isDoubleClick = !isTripleClick;
                doubleClickTimer.Stop();
                doubleClickTimer.Start();
            }
            else
            {
                
                doubleClickTimer.Start();
                isDoubleClick = true;
            }
#endif
            CheckForCursorVisibility(false);
        }

        private bool IsMouseInsideSelectedLines(MouseButtonEventArgs e)
        {
            foreach (PageAdv page in Pages)
            {
                if (page.IsSelected)
                {
                    Point pt = e.GetPosition(page.ForegroundContainer);
                    if (page.SelectedLines.Where(line => line.BoundingRectangle.Contains(pt)).Count() > 0)
                    {
                        return true;
                    }
                    //if (page.SelectedBoxes.Where(b => b.BoundingRectangle.Contains(pt)).Count() > 0)
                    //{
                    //    return true;
                    //}

                }
                else if (SelectedImage != null && page.SelectedLines.Count == 1)
                {
                    Point pt = e.GetPosition(page.ForegroundContainer);
                    if (page.SelectedLines.Where(line => line.BoundingRectangle.Contains(pt)).Count() > 0)
                    {
                        return true;
                    }
                    //if (page.SelectedBoxes.Where(b => b.BoundingRectangle.Contains(pt)).Count() > 0)
                    //{
                    //    return true;
                    //}
                }
            }

            return false;
        }

#if WPF && (SyncfusionFramework4_0 || SyncfusionFramework4_5 || SyncfusionFramework4_5_1)
        /// <summary>
        /// Determines whether the mouse is inside selected lines.
        /// </summary>
        /// <param name="e">The <see cref="TouchEventArgs" /> instance containing the event data.</param>
        /// <returns><c>true</c> if the mouse is inside selected lines; otherwise, <c>false</c>.</returns>
        private bool IsMouseInsideSelectedLines(TouchEventArgs e)
        {
            foreach (PageAdv page in Pages)
            {
                if (page.IsSelected)
                {
                    Point pt = e.GetTouchPoint(page.ForegroundContainer).Position;
                    if (page.SelectedLines.Where(line => line.BoundingRectangle.Contains(pt)).Count() > 0)
                        return true;
                }
                else if (SelectedImage != null && page.SelectedLines.Count == 1)
                {
                    Point pt = e.GetTouchPoint(page.ForegroundContainer).Position;
                    if (page.SelectedLines.Where(line => line.BoundingRectangle.Contains(pt)).Count() > 0)
                        return true;
                }
            }
            return false;
        }
#endif

        internal void CheckForCursorVisibility(bool isTouch)
        {
            if (CurrentPage != null)
            {
                bool isChecked = false;
                if (IsSelected)
                {
                    Point pt = new Point(Math.Ceiling(CurrentPage.Caret.Location.X), Math.Ceiling(CurrentPage.Caret.Location.Y));
                    foreach (PageAdv page in Pages)
                    {
                        if (page.IsSelected)
                        {
                            if (page.SelectedLines.Where(line => Ceil(line.BoundingRectangle).Contains(pt)).Count() > 0)
                            {
                                if (page == CurrentPage)
                                {
                                    CurrentPage.HideCaret(isTouch);
                                    isChecked = true;
                                }
                            }
                            if (page.SelectedLines.Where(line => line.IsTableLine).Count() > 0)
                            {
                                if(page==CurrentPage)
                                {
                                    CurrentPage.HideCaret(isTouch);
                                    isChecked=true;
                                }
                            }
                        }
                    }

                    if (!isChecked)
                        CurrentPage.ShowCaret(isTouch);
                }
                else if (isPageActive)
                {
                    CurrentPage.ShowCaret(isTouch);
                }
                else
                {
                    CurrentPage.HideCaret(isTouch);
                }
            }

        }

        protected override void OnLostFocus(RoutedEventArgs e)
        {
            if (OwnerControl.HideCursorOnLostFocus)
            {
                CurrentPage.HideCaret(false);
            }
            base.OnLostFocus(e);
        }

        private Rect Ceil(Rect rect)
        {
            return new Rect(Math.Ceiling(rect.X), Math.Ceiling(rect.Y), Math.Ceiling(rect.Width), Math.Ceiling(rect.Height));
        }

        void LayoutViewer_MouseRightButtonDown(object sender, MouseButtonEventArgs e)
        {
            if (ImageResizer != null)
            {
                ImageResizer.Visibility = Visibility.Collapsed;
                SelectedImage = null;
                IsImageResizerSelected = false;
            }            
            UseUpDownSelection = false;
            UpDownSelectionWidth = double.NaN;
            this.CaptureMouse();
            FindFocusedPage(e);
            OwnerControl.Focus();
            if (!IsSelected && CurrentPage != null && CurrentPage.Caret != null)
            {
                Point point = e.GetPosition(CurrentPage.ForegroundContainer);
                ElementBox box = CurrentPage.GetElementBox(point);
                if (box is TableCellElementBox)
                {
                    LineInfo line = (box as TableCellElementBox).GetLineFromPoint(point);
                    if (line != null)
                    {
                        box = line.GetElementBoxFromPoint(point);
                    }
                }
                if (box != null && box.IsImageBox)
                    (box.Inline as ImageContainerAdv).SelectElement();
            }
            if (CurrentPage != null)
            {
                CurrentPage.UpdateCaretPosition(e);
                if (!IsMouseInsideSelectedLines(e))
                    IsSelected = false;
            }
            OwnerControl.ChangeContextMenuOnReadOnly();
            CheckForCursorVisibility(false);
        }

        public bool GetIsStyleChanged()
        {
            return false;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void LayoutViewer_MouseMove(object sender, MouseEventArgs e)
        {
            if (!isDragging && isMousedown)
            {
                dragTimer.Stop();
            }
            isDropping = false;

#if WPF
            if (e.LeftButton == MouseButtonState.Pressed && isMouseInsideSelectedLines)
                isDragging = true;

            if(isMousedown && e.LeftButton==MouseButtonState.Pressed && !isDragging)
#else
            if (isMousedown && !isDragging)
#endif
            {
                FindFocusedPage(e);
                if (CurrentPage != null)
                {
                    CurrentPage.UpdateCaretPosition(e);
                    OwnerControl.Selection.EndingPage = Pages.IndexOf(CurrentPage);
                    OwnerControl.Selection.EndPoint = new Point(CurrentPage.Caret.Location.X, CurrentPage.Caret.Location.Y + CurrentPage.Caret.Height / 2);
                    OwnerControl.Selection.End = OwnerControl.PositionHandler.TextPosition.Copy();
                    startPage = OwnerControl.Selection.StartingPage;
                    endPage = OwnerControl.Selection.EndingPage;
                    for (int i = 0; i < Pages.Count; i++)
                    {
                        if (i == startPage || i == endPage || (startPage < i && endPage > i) || (startPage > i && endPage < i))
                        {
                            Pages[i].SelectedLines = Pages[i].GenerateLineInfoForSelection();
                        }
                        else
                        {
                            Pages[i].IsSelected = false;
                        }
                    }
                    Point startPoint = OwnerControl.Selection.StartPoint;
                    Point endPoint = OwnerControl.Selection.EndPoint;
                    if (OwnerControl.Selection.StartingPage == OwnerControl.Selection.EndingPage)
                    {
                        if ((OwnerControl.Selection.StartPoint.Y > OwnerControl.Selection.EndPoint.Y && CurrentPage.SelectedLines.Count != 1)
                            || ((OwnerControl.Selection.StartPoint.Y == OwnerControl.Selection.EndPoint.Y || CurrentPage.SelectedLines.Count == 1)
                            && OwnerControl.Selection.StartPoint.X > OwnerControl.Selection.EndPoint.X))
                        {
                            startPoint = OwnerControl.Selection.EndPoint;
                            endPoint = OwnerControl.Selection.StartPoint;
                        }
                    }
                    else if (OwnerControl.Selection.StartingPage > OwnerControl.Selection.EndingPage)
                    {
                        startPoint = OwnerControl.Selection.EndPoint;
                        endPoint = OwnerControl.Selection.StartPoint;
                        startPage = OwnerControl.Selection.EndingPage;
                        endPage = OwnerControl.Selection.StartingPage;
                    }
                    if (Pages.Count < endPage)
                    {
                        OwnerControl.Selection.StartingInline = Pages[startPage].GetInlineFromPoint(startPoint);
                        OwnerControl.Selection.StartingIndex = Pages[startPage].GetIndexFromPoint(startPoint);
                    }
                    if (Pages.Count < startPage)
                    {
                        OwnerControl.Selection.EndingInline = Pages[endPage].GetInlineFromPoint(endPoint);
                        OwnerControl.Selection.EndingIndex = Pages[endPage].GetIndexFromPoint(endPoint);
                    }
                    //if (!string.IsNullOrEmpty(OwnerControl.Selection.Text))
                    //{

                    //}
                    foreach (PageAdv page in Pages)
                    {
                        page.AddSelectionPath();
                    }

                    if (Pages.Where(page => page.IsSelected).Count() > 0 || SelectedImage != null)
                    {
                        SelectionChangedEventArgs args = new SelectionChangedEventArgs();
                        args.StartPosition = OwnerControl.Selection.Start;
                        args.EndPosition = OwnerControl.Selection.End;
                        args.SelectedText = OwnerControl.Selection.Text;
                        OwnerControl.FireSelectionChanged(args);
                    }
                    OwnerControl.Selection.ExtractSelectedText();
                }
            }
            else if (isDragging && isMouseInsideSelectedLines)
            {
                bool isInside = false;
                foreach (PageAdv page in Pages)
                {
                    if (page.IsSelected)
                    {
                        Point pt = e.GetPosition(page.ForegroundContainer);
                        if (page.SelectedLines.Where(line => line.BoundingRectangle.Contains(pt)).Count() > 0)
                        {
                            isInside = true;
                        }
                    }
                }
                FindFocusedPage(e);
                if (CurrentPage != null)
                {
                    CurrentPage.UpdateCaretPosition(e);
                }
                if (!isInside)
                {
                    isDropping = true;
                }
            }
            else if (!isDragging)
            {
                if (IsSelected)
                {
                    foreach (PageAdv page in Pages)
                    {
                        if (page.IsSelected)
                        {
                            Point pt = e.GetPosition(page.ForegroundContainer);
                            if (page.SelectedLines.Where(line => line.BoundingRectangle.Contains(pt)).Count() > 0)
                            {
                                page.Cursor = Cursors.Arrow;
                            }
                            else
                            {
                                page.Cursor = Cursors.IBeam;
                            }
                        }
                    }
                }
                else if (CurrentPage != null)
                {
                    CurrentPage.Cursor = Cursors.IBeam;
                }
            }
            else if (CurrentPage != null)
            {
                CurrentPage.Cursor = Cursors.IBeam;
            }
            if (isMousedown)
            {
                scrollingTimer.Stop();
                scrollingTimer.Start();
            }

            if (OwnerControl.Selection.Start != null && OwnerControl.Selection.Start.IsEqual(OwnerControl.Selection.End) && !OwnerControl.Selection.IsCellSelected)
            {
                IsSelected = false;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="startPage"></param>
        /// <param name="endPage"></param>
        internal void SelectPages(int startPage, int endPage)
        {
            for (int i = 0; i < Pages.Count; i++)
            {
                if (i == startPage || i == endPage || (startPage < i && endPage > i) || (startPage > i && endPage < i))
                {
                    Pages[i].SelectedLines = Pages[i].GenerateLineInfoForSelection();
                }
                else
                {
                    Pages[i].IsSelected = false;
                }
            }

            if (!string.IsNullOrEmpty(OwnerControl.Selection.Text) || OwnerControl.Selection.Blocks.Count > 0)
            {
                foreach (PageAdv page in Pages)
                {
                    page.AddSelectionPath();
                }
            }

            if (Pages.Where(page => page.IsSelected).Count() > 0 || SelectedImage != null)
            {
                SelectionChangedEventArgs args = new SelectionChangedEventArgs();
                args.StartPosition = OwnerControl.Selection.Start;
                args.EndPosition = OwnerControl.Selection.End;
                args.SelectedText = OwnerControl.Selection.Text;
                OwnerControl.FireSelectionChanged(args);
            }

        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void LayoutViewer_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        { 
            isMousedown = false;
            //isDragging = false;
            dragTimer.Stop();
            ReleaseMouseCapture();

            if (OwnerControl.Selection.Start != null && OwnerControl.Selection.Start.IsGreaterThan(OwnerControl.Selection.End))
            {
                TextPosition pos = OwnerControl.Selection.Start;
                OwnerControl.Selection.Start = OwnerControl.Selection.End;
                OwnerControl.Selection.End = pos;
            }
            bool flag = false;

#if WPF
            if(e.LeftButton==MouseButtonState.Pressed && isDropping && !IsReadOnly)
            {
#else
            if (isDropping && !IsReadOnly)
            {
#endif
                if (OwnerControl.Selection.Blocks.Count > 0)
                {
                    HistoryInfo history = new HistoryInfo();
                    history.Action = Actions.DragDrop;
                    OwnerControl.Selection.GetHistoryInfo(history);
                    OwnerControl.Selection.RemoveSelection(false);
                    history.NextIsTable = IsTableAtNext();

                    if (OwnerControl.Selection.End != null && !OwnerControl.Selection.End.IsGreaterThan(TextPosition) && OwnerControl.Selection.End.Paragraph == TextPosition.Paragraph)
                    {
                        int index = 0;
                        if (OwnerControl.Selection.Start.Paragraph == OwnerControl.Selection.End.Paragraph)
                        {
                            index = OwnerControl.Selection.Start.ParseIndex();
                        }
                        TextPosition.Index = TextPosition.StepDown(OwnerControl.Selection.End.ParseIndex(OwnerControl.Selection.End.StepDown(index)));
                    }

                    bool needToInsert = false;

                    if (OwnerControl.Selection.Start.IsPositionAtParagraphStart && OwnerControl.Selection.End.IsPositionAtParagraphEnd && TextPosition.Paragraph.Inlines.Count == 0)
                    {
                        needToInsert = true;
                    }
                    UpdateTextPosition();
                    history.TempPositionStart = TextPosition.CopyForHistory();
                    bool needImageToSelect = false;
                    if (SelectedImage != null)
                        needImageToSelect = true;
                    IsSelected = false;
                    flag = true;
                    OwnerControl.Selection.Start = TextPosition.Copy();
                    if (needToInsert)
                    {
                        OwnerControl.PositionHandler.InsertBlocks(OwnerControl.Selection.Blocks);
                        TextPosition pos = OwnerControl.Selection.Start.GetPositionUsingIndex();
                        if (pos != null)
                            OwnerControl.Selection.Start = pos;
                    }
                    else
                        OwnerControl.Selection.PasteSelection(OwnerControl.Selection.Blocks);

                    history.TempPositionEnd = TextPosition.CopyForHistory();
                    OwnerControl.Selection.End = TextPosition.Copy();
                    if (needImageToSelect)
                    {
                        int indexInInline = 0;
                        SelectedImage = OwnerControl.PositionHandler.GetInlineFromIndex(OwnerControl.Selection.End.Paragraph, OwnerControl.Selection.End.Index, ref indexInInline) as ImageContainerAdv;
                        IsImageResizerSelected = true;
                        history.IsImageResizerSelected = IsImageResizerSelected;
                    }
                    OwnerControl.Selection.Select();
                    OwnerControl.History.RecordUndo(history);
                    OwnerControl.History.ClearRedo();
                }
            }
            else if (isDropping)
            {
                IsSelected = false;
            }
#if !WPF
            if (isMouseInsideSelectedLines)
            {
                isMouseInsideSelectedLines = false;
                if (!flag && SelectedImage == null)
                    IsSelected = false;
            }            
#else
            if (isSingleClick && isMouseInsideSelectedLines)
            {
                isMouseInsideSelectedLines = false;
                if (!flag && SelectedImage==null)
                { IsSelected = false; }
            }
#endif

            if (isDragging)
            {
                isDragging = false;
                CurrentPage.ResumeAnimation();
                CurrentPage.Cursor = Cursors.IBeam;
                isDropping = false;
            }

            CheckForCursorVisibility(false);
        }

        internal bool IsTableAtNext()
        {
            if (OwnerControl.PositionHandler.TextPosition.Paragraph.NextBlock == null)
                return false;

            return OwnerControl.PositionHandler.TextPosition.Paragraph.Inlines.Count == 0
                && OwnerControl.PositionHandler.TextPosition.IsZeroIndex()
                && OwnerControl.PositionHandler.TextPosition.Paragraph.NextBlock.IsTable;
        }


        internal void UpdateTextPosition()
        {
            if (TextPosition.Paragraph != null)
            {
                int value = TextPosition.ParseIndex();
                TextPosition.SetIndex(value.ToString());
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        internal abstract bool CheckForScrollBarVisibility();

        /// <summary>
        /// Sets the IsArranged property of the paragraph to false
        /// </summary>
        internal void SetIsArrangedToFalse()
        {
            foreach (SectionAdv section in Document.Sections)
            {
                IterateBlocksToResetIsArranged(section.Blocks);
            }
        }

        internal void IterateBlocksToResetIsArranged(BlockCollection<BlockAdv> Blocks)
        {
            IEnumerable<BlockAdv> paragraphs = Blocks.Where<BlockAdv>(para => para.IsArranged);
            foreach (BlockAdv para in paragraphs)
            {
                para.IsArranged = false;
            }
            foreach (BlockAdv b in Blocks)
            {
                if (b is TableAdv)
                {
                    foreach (TableRowAdv row in (b as TableAdv).Rows)
                    {
                        foreach (TableCellAdv c in row.Cells)
                        {
                            IterateBlocksToResetIsArranged(c.Blocks);
                        }
                    }
                }
            }
        }

        /// <summary>
        /// Moves the cursor to the specified position
        /// </summary>
        /// <param name="pos"></param>
        public void MoveToPosition(TextPosition pos)
        {
            if (pos != null)
            {
                TextPosition = pos;
                PositionCursor();
            }
        }

        /// <summary>
        /// Position the cursor to the current TextPosition
        /// </summary>
        private void PositionCursor()
        {
            if (OwnerControl != null && OwnerControl.PositionHandler != null)
                OwnerControl.PositionHandler.PositionCursor();
        }

        /// <summary>
        /// Returns the next position in the document
        /// </summary>
        /// <returns></returns>
        public TextPosition GetNextPosition()
        {
            if (TextPosition != null)
            {
                if (!TextPosition.IsPositionAtParagraphEnd)
                {
                    TextPosition pos = TextPosition.Copy();
                    pos.Index = TextPosition.StepUp(1);
                    return pos;
                }
                else
                {
                    if (TextPosition.Paragraph != null)
                    {
                        if (TextPosition.Paragraph.NextBlock != null)
                        {
                            TextPosition pos = new TextPosition(Document);
                            if (TextPosition.Paragraph.NextBlock.IsParagraph)
                            {
                                pos.Paragraph = TextPosition.Paragraph.NextBlock as ParagraphAdv;
                            }
                            else
                            {
                                BlockAdv block = TextPosition.Paragraph.NextBlock;
                                while (block.IsTable)
                                {
                                    block = (block as TableAdv).GetFirstBlockInFirstCell();
                                }
                                pos.Paragraph = block as ParagraphAdv;
                            }
                            pos.SetIndex("0");
                            return pos;
                        }
                        else if (TextPosition.Paragraph.NextBlock == null)
                        {
                            TextPosition pos = new TextPosition(Document);
                            if (TextPosition.Paragraph.AssociatedCell != null)
                            {
                                TableCellAdv nextcell = TextPosition.Paragraph.AssociatedCell.GetNextCell();
                                if (nextcell != null)
                                {
                                    BlockAdv block = nextcell.Blocks[0];
                                    while (block.IsTable)
                                    {
                                        block = (block as TableAdv).GetFirstBlockInFirstCell();
                                    }
                                    pos.Paragraph = block as ParagraphAdv;
                                    pos.SetIndex("0");
                                    return pos;
                                }
                                else
                                {
                                    BlockAdv nextblock = TextPosition.Paragraph.AssociatedCell.OwnerTable.NextBlock;
                                    if (nextblock == null)
                                    {
                                        nextblock = TextPosition.Paragraph.AssociatedCell.OwnerTable;
                                        while (nextblock.AssociatedCell != null)
                                        {
                                            nextblock = nextblock.AssociatedCell.OwnerTable;
                                            if (nextblock.NextBlock != null)
                                            {
                                                break;
                                            }
                                        }
                                        nextblock = nextblock.NextBlock;
                                    }
                                    if (nextblock != null)
                                    {
                                        if (nextblock.IsParagraph)
                                        {
                                            pos.Paragraph = nextblock as ParagraphAdv;
                                        }
                                        else
                                        {
                                            while (nextblock.IsTable)
                                            {
                                                nextblock = (nextblock as TableAdv).GetFirstBlockInFirstCell();
                                            }
                                            pos.Paragraph = nextblock as ParagraphAdv;
                                        }

                                        pos.SetIndex("0");
                                        return pos;
                                    }
                                }
                            }
                        }
                        else if (TextPosition.Paragraph.Section != null)
                        {
                            SectionAdv section = TextPosition.Paragraph.Section.GetNextSection();
                            if (section != null && section.Blocks.Count > 0)
                            {
                                TextPosition pos = new TextPosition(Document);
                                BlockAdv block = section.Blocks[0];
                                while (block.IsTable)
                                {
                                    block = (block as TableAdv).GetFirstBlockInFirstCell();
                                }
                                pos.Paragraph = block as ParagraphAdv;
                                pos.SetIndex("0");
                                return pos;
                            }
                        }
                    }
                }

            }

            return null;
        }

        /// <summary>
        /// Returns the previous position in the document
        /// </summary>
        /// <returns></returns>
        public TextPosition GetPreviousPosition()
        {
            if (TextPosition != null)
            {
                if (!TextPosition.IsPositionAtParagraphStart)
                {
                    TextPosition pos = TextPosition.Copy();
                    pos.Index = TextPosition.StepDown(1);
                    return pos;
                }
                else
                {
                    if (TextPosition.Paragraph != null)
                    {
                        if (TextPosition.Paragraph.PreviousBlock != null)
                        {
                            TextPosition pos = new TextPosition(Document);
                            if (TextPosition.Paragraph.PreviousBlock.IsParagraph)
                            {
                                pos.Paragraph = TextPosition.Paragraph.PreviousBlock as ParagraphAdv;
                            }
                            else
                            {
                                BlockAdv block = TextPosition.Paragraph.PreviousBlock;
                                while (block.IsTable)
                                {
                                    block = (block as TableAdv).GetLastBlockInLastCell();
                                }
                                pos.Paragraph = block as ParagraphAdv;
                            }
                            pos.SetIndex(pos.Paragraph.Length());
                            return pos;
                        }
                        else if (TextPosition.Paragraph.PreviousBlock == null)
                        {
                            if (TextPosition.Paragraph.IsInsideTable)
                            {
                                TextPosition pos = new Controls.TextPosition(Document);
                                TableCellAdv previouscell = TextPosition.Paragraph.AssociatedCell.GetPreviousCell();
                                if (previouscell != null)
                                {
                                    BlockAdv block = previouscell.Blocks[previouscell.Blocks.Count - 1];
                                    if (block is ParagraphAdv)
                                    {
                                        pos.Paragraph = block as ParagraphAdv;
                                    }
                                    else if (block is TableAdv)
                                    {
                                        while (block.IsTable)
                                        {
                                            block = (block as TableAdv).GetLastBlockInLastCell();
                                        }
                                        pos.Paragraph = block as ParagraphAdv;
                                    }

                                    pos.SetIndex(pos.Paragraph.Length());
                                    return pos;
                                }
                                else
                                {
                                    BlockAdv blk = TextPosition.Paragraph.AssociatedCell.OwnerTable.PreviousBlock;
                                    if (blk == null)
                                    {
                                        blk = TextPosition.Paragraph.AssociatedCell.OwnerTable;
                                        while (blk.AssociatedCell != null)
                                        {
                                            blk = blk.AssociatedCell.OwnerTable;
                                            if (blk.PreviousBlock != null)
                                            {
                                                break;
                                            }
                                        }
                                        blk = blk.PreviousBlock;
                                    }
                                    if (blk != null)
                                    {
                                        if (blk.IsParagraph)
                                        {
                                            pos.Paragraph = blk as ParagraphAdv;
                                        }
                                        else
                                        {
                                            while (blk.IsTable)
                                            {
                                                blk = (blk as TableAdv).GetLastBlockInLastCell();
                                            }
                                            pos.Paragraph = blk as ParagraphAdv;
                                        }

                                        pos.SetIndex(pos.Paragraph.Length());
                                        return pos;
                                    }
                                }
                            }
                        }
                        else if (TextPosition.Paragraph.Section != null)
                        {
                            SectionAdv section = TextPosition.Paragraph.Section.GetPreviousSection();
                            if (section != null && section.Blocks.Count > 0)
                            {
                                TextPosition pos = new TextPosition(Document);
                                BlockAdv lastblk = section.Blocks.Last();
                                if (lastblk.IsParagraph)
                                    pos.Paragraph = lastblk as ParagraphAdv;
                                else
                                {
                                    while (lastblk.IsTable)
                                    {
                                        lastblk = (lastblk as TableAdv).GetLastBlockInLastCell();
                                    }
                                    pos.Paragraph = lastblk as ParagraphAdv;
                                }
                                pos.SetIndex(pos.Paragraph.Length());
                                return pos;
                            }
                        }
                    }
                }

            }

            return null;
        }

        /// <summary>
        /// Called when ImageResizer dragging is completed
        /// </summary>
        /// <param name="imageResizer"></param>
        /// <param name="imageContainer"></param>
        internal void DragCompleted(ImageResizer imageResizer, ImageContainerAdv imageContainer)
        {
            bool canArrange = true;
            ParagraphAdv paragraph = CreateBlock(imageContainer);
            canArrange = imageContainer.Height == ImageResizer.Height && imageContainer.Width == ImageResizer.Width ? false : true;
            imageContainer.Height = ImageResizer.Height;
            imageContainer.Width = ImageResizer.Width;

            if (canArrange)
            {
                HistoryInfo history = new HistoryInfo();
                history.Action = Actions.ImageReszing;
                history.StartPosition = TextPosition.CopyForHistory();
                history.StartPosition.Paragraph = imageContainer.Paragraph;
                history.StartPosition.SetIndex(imageContainer.Paragraph.GetIndexFromInlineAndSpanIndex(imageContainer, 1).ToString());
                history.UndoType = UndoType.SelectionBased;
                history.InlineStyle = imageContainer.CreatInline();
                history.Blocks.Add(paragraph);
                OwnerControl.History.RecordUndo(history);
                OwnerControl.History.ClearRedo();
                imageContainer.Paragraph.ArrangeElements();
                SetIsArrangedToFalse();
                SetVisibleLinesToPage();
                OwnerControl.PositionHandler.PositionCursor();
                if (imageResizer.Parent == null)
                {
                    Canvas canvs = imageContainer.ElementBox.Element.Parent as Canvas;
                    if (canvs != null)
                    {
                        canvs.Children.Add(imageResizer);
                    }
                }

                OwnerControl.Selection.ExtractSelectedText();

                //imageContainer.Paragraph.ArrangeElements(new Size(650, 800));
            }

            ImageResizer.Left = Canvas.GetLeft(imageContainer.ElementBox.Element);
            ImageResizer.Top = Canvas.GetTop(imageContainer.ElementBox.Element);
        }

        public ParagraphAdv CreateBlock(Inline inline)
        {
            ParagraphAdv para = new ParagraphAdv();
            if (inline != null)
            {
                para.Inlines.Add(inline.CreatInline());
            }

            return para;
        }

        /// <summary>
        /// Handles double click
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void doubleClickTimer_Tick(object sender, EventArgs e)
        {
            isDoubleClick = false;
            isTripleClick = false;
            doubleClickTimer.Stop();
        }


        void scrollingTimer_Tick(object sender, EventArgs e)
        {
            if (isMousedown)
            {
                //VerticalScrollBar.Value = VerticalScrollBar.Value - VerticalScrollBar.LargeChange;
            }
        }

        /// <summary>
        /// Arranges the element box
        /// </summary>
        /// <param name="document"></param>
        public abstract void ArrangeElements();

        /// <summary>
        /// 
        /// </summary>
        public abstract void SetPreviousBlocks();

        /// <summary>
        /// 
        /// </summary>
        public abstract void SetVisibleLinesToPage();

        /// <summary>
        /// 
        /// </summary>
        /// <param name="page"></param>
        /// <param name="line"></param>
        public abstract void BringLineToView(LineInfo line);

        /// <summary>
        /// 
        /// </summary>
        public abstract void UpdateVerticalScrollBar();

        /// <summary>
        /// 
        /// </summary>
        public abstract void UpdateHorizontalScrollBar();

        /// <summary>
        /// 
        /// </summary>
        /// <param name="args"></param>
        /// <param name="canvas"></param>
        internal void HandleTextInput(string text)
        {
            UseUpDownSelection = false;
            UpDownSelectionWidth = double.NaN;
            HistoryInfo history = new HistoryInfo();
            history.Action = Actions.Insert;
            history.StartPosition = TextPosition.CopyForHistory();
            history.Text = text;
            if (IsSelected)
            {
                OwnerControl.Selection.GetHistoryInfo(history);

                List<TableCellAdv> selectedcells = OwnerControl.Selection.SelectedCellsInTable();
                List<PreservedCellsInfo> copiedcells = new List<PreservedCellsInfo>();
                if (selectedcells.Count > 0)
                {
                    foreach (TableCellAdv cell in selectedcells)
                    {
                        PreservedCellsInfo cleanedcell = new PreservedCellsInfo();
                        cleanedcell.RowIndex = cell.RowIndex;
                        cleanedcell.ColumnIndex = cell.ColumnIndex;
                        foreach (BlockAdv b in cell.Blocks)
                        {
                            cleanedcell.Blocks.Add(b.CopyBlock());
                        }
                        copiedcells.Add(cleanedcell);
                    }
                    history.PreservedCells = copiedcells;
                }
                history.IsImageResizerSelected = IsImageResizerSelected;
                OwnerControl.Selection.RemoveSelection(true);
            }

            history.InlineStyle = CurrentInlineStyle.GetSpan();
            int indexInInline = 0;
            Inline inline = OwnerControl.PositionHandler.GetInlineFromTextPosition(ref indexInInline);
            history.IsStyleChanged = CurrentInlineStyle.IsEqualInStyle(inline);
            history.NextIsTable = IsTableAtNext();
            OwnerControl.History.RecordUndo(history);
            OwnerControl.PositionHandler.InsertText(text);

            OwnerControl.History.CheckForClearingRedo();
        }

        internal double GetZoomedValue(double value, bool useTranx)
        {
            return value * (useTranx ? transX : transY);
        }

        /// <summary>
        /// Handles the End key
        /// </summary>
        internal void HandleEndKey()
        {
            UseUpDownSelection = false;
            UpDownSelectionWidth = double.NaN;
            TextPosition pos = TextPosition;

            if (IsSelected)
                pos = OwnerControl.Selection.End;
            IsSelected = false;

            if (pos != null && pos.LineInfo != null)
            {
                PageAdv page = Pages[pos.LineInfo.PageIndex];
                if (page != null)
                {
                    Point point = new Point(pos.LineInfo.BoundingRectangle.Right, pos.LineInfo.BoundingRectangle.Top + pos.LineInfo.BoundingRectangle.Height / 2);
                    page.UpdateCaretPosition(point);
                }
            }
        }

        internal void HandleTabKey()
        {
            UseUpDownSelection = false;
            UpDownSelectionWidth = double.NaN;
            TextPosition pos = TextPosition;

            if (pos.Paragraph.IsInsideTable)
            {
                if (IsSelected)
                    IsSelected = false;
                if (pos != null)
                {
                    TableCellAdv nextcell = pos.Paragraph.AssociatedCell.GetNextCell();
                    if (nextcell != null)
                    {
                        TextPosition startPos = new TextPosition(Document);
                        TextPosition endPos = new TextPosition(Document);

                        if (nextcell.Blocks.Count > 0)
                        {
                            if (nextcell.Blocks[0].IsParagraph)
                            {
                                pos.Paragraph = nextcell.Blocks[0] as ParagraphAdv;
                            }
                            else
                            {
                                BlockAdv next = nextcell.Blocks[0];
                                while (next.IsTable)
                                {
                                    next = (next as TableAdv).GetFirstBlockInFirstCell();
                                }
                                pos.Paragraph = next as ParagraphAdv;
                            }
                            pos.SetIndex("0");
                            startPos = pos;

                            if (nextcell.Blocks.Count == 1 && nextcell.Blocks[0].IsParagraph && nextcell.Blocks[0].Inlines.Count == 0)
                            {
                                MoveToPosition(startPos);
                                CheckForCursorVisibility(false);
                                return;
                            }

                            BlockAdv finalblk = nextcell.Blocks[nextcell.Blocks.Count - 1];
                            if (finalblk.IsParagraph)
                            {
                                endPos.Paragraph = finalblk as ParagraphAdv;
                            }
                            else
                            {
                                BlockAdv next = finalblk;
                                while (next.IsTable)
                                {
                                    next = (next as TableAdv).GetLastBlockInLastCell();
                                }
                                endPos.Paragraph = next as ParagraphAdv;
                            }
                            endPos.SetIndex(endPos.Paragraph.Length());

                            MoveToPosition(endPos);
                            CheckForCursorVisibility(false);

                            OwnerControl.Selection.Select(startPos, endPos);
                        }
                    }
                    else
                    {
                        TableCellAdv cell = pos.Paragraph.AssociatedCell;
                        if (cell.IsLastCell)
                        {
                            OwnerControl.Viewer.InsertRow(RowPlacement.Below);
                        }
                        HandleTabKey();
                    }
                }

            }
        }

        /// <summary>
        /// Handles the End key
        /// </summary>
        internal void HandleHomeKey()
        {
            UseUpDownSelection = false;
            UpDownSelectionWidth = double.NaN;
            TextPosition pos = TextPosition;

            if (IsSelected)
                pos = OwnerControl.Selection.Start;
            IsSelected = false;
            if (pos != null && pos.LineInfo != null)
            {
                PageAdv page = Pages[pos.LineInfo.PageIndex];
                if (page != null)
                {
                    Point point = new Point(pos.LineInfo.BoundingRectangle.Left, pos.LineInfo.BoundingRectangle.Top + pos.LineInfo.BoundingRectangle.Height / 2);
                    page.UpdateCaretPosition(point);
                }
            }
        }

        /// <summary>
        /// Handles the Shift End key
        /// </summary>
        internal void HandleShiftEndKey()
        {
            UseUpDownSelection = false;
            UpDownSelectionWidth = double.NaN;
            if (TextPosition != null && TextPosition.LineInfo != null)
            {
                PageAdv page = Pages[TextPosition.LineInfo.PageIndex];
                if (page != null)
                {
                    if (!IsSelected)
                    {
                        OwnerControl.Selection.Start = TextPosition.Copy();
                        Point point = new Point(TextPosition.LineInfo.BoundingRectangle.Right, TextPosition.LineInfo.BoundingRectangle.Top + TextPosition.LineInfo.BoundingRectangle.Height / 2);
                        page.UpdateCaretPosition(point);
                        OwnerControl.Selection.End = TextPosition;
                        OwnerControl.Selection.Select();
                        CheckForCursorVisibility(false);
                    }
                    else
                    {
                        bool start = false;
                        if (OwnerControl.Selection.Start.IsEqual(TextPosition))
                            start = true;
                        Point point = new Point(TextPosition.LineInfo.BoundingRectangle.Right, TextPosition.LineInfo.BoundingRectangle.Top + TextPosition.LineInfo.BoundingRectangle.Height / 2);
                        page.UpdateCaretPosition(point);
                        if (start)
                            OwnerControl.Selection.Start = TextPosition.Copy();
                        else
                            OwnerControl.Selection.End = TextPosition.Copy();

                        OwnerControl.Selection.Select();
                        CheckForCursorVisibility(false);
                    }
                }
            }
        }

        /// <summary>
        /// Handles the shift End key
        /// </summary>
        internal void HandleShiftHomeKey()
        {
            UseUpDownSelection = false;
            UpDownSelectionWidth = double.NaN;
            if (TextPosition != null && TextPosition.LineInfo != null)
            {
                PageAdv page = Pages[TextPosition.LineInfo.PageIndex];
                if (page != null)
                {
                    if (!IsSelected)
                    {
                        OwnerControl.Selection.End = TextPosition.Copy();
                        Point point = new Point(TextPosition.LineInfo.BoundingRectangle.Left, TextPosition.LineInfo.BoundingRectangle.Top + TextPosition.LineInfo.BoundingRectangle.Height / 2);
                        page.UpdateCaretPosition(point);
                        OwnerControl.Selection.Start = TextPosition;
                        OwnerControl.Selection.Select();
                        CheckForCursorVisibility(false);
                    }
                    else
                    {
                        bool start = false;
                        if (OwnerControl.Selection.Start.IsEqual(TextPosition))
                            start = true;
                        Point point = new Point(TextPosition.LineInfo.BoundingRectangle.Left, TextPosition.LineInfo.BoundingRectangle.Top + TextPosition.LineInfo.BoundingRectangle.Height / 2);
                        page.UpdateCaretPosition(point);
                        if (start)
                            OwnerControl.Selection.Start = TextPosition.Copy();
                        else
                            OwnerControl.Selection.End = TextPosition.Copy();

                        OwnerControl.Selection.Select();
                        CheckForCursorVisibility(false);
                    }
                }
            }
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="e"></param>
        /// <param name="canvas"></param>
        internal void HandleDeleteKey()
        {
            List<BlockAdv> block = new List<BlockAdv>();
            HistoryInfo history = new HistoryInfo();
            history.Action = Actions.Delete;
            history.StartPosition = TextPosition.CopyForHistory();

            if (IsSelected)
            {
                OwnerControl.Selection.GetHistoryInfo(history);
                List<TableCellAdv> selectedcells = OwnerControl.Selection.SelectedCellsInTable();
                List<PreservedCellsInfo> copiedcells = new List<PreservedCellsInfo>();
                if (selectedcells.Count > 0)
                {
                    foreach (TableCellAdv cell in selectedcells)
                    {
                        PreservedCellsInfo cleanedcell = new PreservedCellsInfo();
                        cleanedcell.RowIndex = cell.RowIndex;
                        cleanedcell.ColumnIndex = cell.ColumnIndex;
                        foreach (BlockAdv b in cell.Blocks)
                        {
                            cleanedcell.Blocks.Add(b.CopyBlock());
                        }
                        copiedcells.Add(cleanedcell);
                    }
                    history.PreservedCells = copiedcells;
                }
                history.IsImageResizerSelected = IsImageResizerSelected;
                OwnerControl.Selection.RemoveSelection(true);
            }
            else
            {
                OwnerControl.PositionHandler.Delete();
            }
            history.NextIsTable = IsTableAtNext();
            history.InlineStyle = OwnerControl.PositionHandler.DeletedInline;
            history.TempPositionStart = TextPosition.CopyForHistory();
            OwnerControl.History.RecordUndo(history);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="args"></param>
        /// <param name="canvas"></param>
        internal void HandleBackKey()
        {
            HistoryInfo history = new HistoryInfo();
            history.Action = Actions.BackSpace;
            history.StartPosition = TextPosition.CopyForHistory();
            bool record = true;
            if (IsSelected)
            {
                OwnerControl.Selection.GetHistoryInfo(history);
                List<TableCellAdv> selectedcells = OwnerControl.Selection.SelectedCellsInTable();
                List<PreservedCellsInfo> copiedcells = new List<PreservedCellsInfo>();
                if (selectedcells.Count > 0)
                {
                    foreach (TableCellAdv cell in selectedcells)
                    {
                        PreservedCellsInfo cleanedcell = new PreservedCellsInfo();
                        cleanedcell.RowIndex = cell.RowIndex;
                        cleanedcell.ColumnIndex = cell.ColumnIndex;
                        foreach (BlockAdv b in cell.Blocks)
                        {
                            cleanedcell.Blocks.Add(b.CopyBlock());
                        }
                        copiedcells.Add(cleanedcell);
                    }
                    history.PreservedCells = copiedcells;
                }
                history.IsImageResizerSelected = IsImageResizerSelected;
                OwnerControl.Selection.RemoveSelection(true);
            }
            else
            {
                record = OwnerControl.PositionHandler.BackSpace();
            }
            history.NextIsTable = IsTableAtNext();
            history.InlineStyle = OwnerControl.PositionHandler.DeletedInline;
            history.TempPositionStart = TextPosition.CopyForHistory();
            if (record)
                OwnerControl.History.RecordUndo(history);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="e"></param>
        /// <param name="canvas"></param>
        internal void HandleUpKey()
        {
            if (IsSelected)
            {
                if (TextPosition.IsEqual(OwnerControl.Selection.End))
                    MoveToPosition(OwnerControl.Selection.Start);
                IsSelected = false;
            }
            else
            {
                LineInfo line = GetPreviousLineInfo();
                if (line != null)
                {
                    bool isBegin = false;
                    TextPosition pos = GetTextPosition(line, UpDownSelectionWidth, ref isBegin);
                    if (isBegin)
                    {
                        //PageAdv page = GetPageFromLine(line);
                        PageAdv page = Pages[line.PageIndex];

                        if (page != null)
                            page.UpdateCaretPosition(new Point(line.BoundingRectangle.Left, line.BoundingRectangle.Top + line.BoundingRectangle.Height / 2));
                    }
                    else
                    {
                        MoveToPosition(pos);
                    }
                }
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="e"></param>
        /// <param name="canvas"></param>
        internal void HandleDownKey()
        {
            if (IsSelected)
            {
                if (TextPosition.IsEqual(OwnerControl.Selection.Start))
                    MoveToPosition(OwnerControl.Selection.End);
                IsSelected = false;
            }
            else
            {
                LineInfo line = GetNextLineInfo();
                if (line != null)
                {
                    bool isBegin = false;
                    TextPosition pos = GetTextPosition(line, UpDownSelectionWidth, ref isBegin);
                    if (isBegin)
                    {
                        //PageAdv page = GetPageFromLine(line);
                        PageAdv page = Pages[line.PageIndex];

                        if (page != null)
                            page.UpdateCaretPosition(new Point(line.BoundingRectangle.Left, line.BoundingRectangle.Top + line.BoundingRectangle.Height / 2));
                    }
                    else
                    {
                        MoveToPosition(pos);
                    }
                }
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="e"></param>
        /// <param name="canvas"></param>
        internal void HandleEnterKey()
        {
            HistoryInfo history = new HistoryInfo();
            history.Action = Actions.Enter;
            history.StartPosition = TextPosition.CopyForHistory();
            if (IsSelected)
            {
                OwnerControl.Selection.GetHistoryInfo(history);
                 List<TableCellAdv> selectedcells = OwnerControl.Selection.SelectedCellsInTable();
                List<PreservedCellsInfo> copiedcells = new List<PreservedCellsInfo>();
                if (selectedcells.Count > 0)
                {
                    foreach (TableCellAdv cell in selectedcells)
                    {
                        PreservedCellsInfo cleanedcell = new PreservedCellsInfo();
                        cleanedcell.RowIndex = cell.RowIndex;
                        cleanedcell.ColumnIndex = cell.ColumnIndex;
                        foreach (BlockAdv b in cell.Blocks)
                        {
                            cleanedcell.Blocks.Add(b.CopyBlock());
                        }
                        copiedcells.Add(cleanedcell);
                    }
                    history.PreservedCells = copiedcells;
                }
                history.IsImageResizerSelected = IsImageResizerSelected;
                OwnerControl.Selection.RemoveSelection(true);
            }
            history.NextIsTable = IsTableAtNext();
            OwnerControl.History.RecordUndo(history);
            OwnerControl.PositionHandler.Enter();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="inline"></param>
        internal void InsertInline(Inline inline)
        {
            if (OwnerControl.IsReadOnly || inline == null)
                return;

            HistoryInfo history = new HistoryInfo();
            history.Action = Actions.InsertInline;
            history.InlineStyle = inline.CreatInline();
            history.InlineStyle.InternalText = inline.InternalText;
            if (IsSelected)
            {
                OwnerControl.Selection.GetHistoryInfo(history);
                OwnerControl.Selection.RemoveSelection(true);
            }

            history.StartPosition = TextPosition.CopyForHistory();
            history.EndPosition = TextPosition.CopyForHistory();
            history.EndPosition.Index=history.EndPosition.StepUp((int)inline.GetLength());

            OwnerControl.History.RecordUndo(history);

            OwnerControl.PositionHandler.InsertInline(inline);
        }

        internal void DeleteTable()
        {
            if (OwnerControl.IsReadOnly)
                return;

            if (!TextPosition.Paragraph.IsInsideTable)
                return;

            TableAdv tableparent = TextPosition.Paragraph.AssociatedCell.OwnerTable;

            DeletedTableHistory history = new DeletedTableHistory();
            history.Action = Actions.DeleteTable;
            history.StartPosition = TextPosition.CopyForHistory();
            
            SelectedCellsInfo selectedCells = GetSelectedCellsInfo();

            if (IsSelected && selectedCells.Owner != null)
            {
                tableparent = selectedCells.Owner;
                history.DeletedTableIndex = selectedCells.Owner.IsInsideTable ? selectedCells.Owner.AssociatedCell.Blocks.IndexOf(selectedCells.Owner) 
                    : Document.Sections[0].Blocks.IndexOf(selectedCells.Owner);
                history.PreservedBlock = selectedCells.Owner.CopyBlock();

                if (OwnerControl.Selection.Start != null && OwnerControl.Selection.End != null)
                {
                    history.StartPosition = OwnerControl.Selection.Start.CopyForHistory();
                    history.EndPosition = OwnerControl.Selection.End.CopyForHistory();
                    history.IsCellSelected = OwnerControl.Selection.IsCellSelected;
                    history.UndoType = UndoType.SelectionBased;
                }
                history.DeletedTable = (TableAdv)selectedCells.Owner.CopyBlock();
                //history.RowsCount = selectedCells.Owner.Rows.Count;
                //history.ColumnCount = selectedCells.Owner.TableHolder.Columns.Count;
            }
            else
            {
                TableAdv table = TextPosition.Paragraph.AssociatedCell.OwnerTable;
                history.DeletedTableIndex = table.IsInsideTable ? table.AssociatedCell.Blocks.IndexOf(table) 
                    : Document.Sections[0].Blocks.IndexOf(table);
                history.PreservedBlock = TextPosition.Paragraph.AssociatedCell.OwnerTable.CopyBlock();
                history.DeletedTable = (TableAdv)TextPosition.Paragraph.AssociatedCell.OwnerTable.CopyBlock();
                //history.RowsCount = tableparent.Rows.Count;
                //history.ColumnCount = tableparent.TableHolder.Columns.Count;
            }

            //foreach (TableRowAdv row in tableparent.Rows)
            //{
            //    foreach (TableCellAdv cell in row.Cells)
            //    {
            //        PreservedCellsInfo preservedcell = new PreservedCellsInfo();
            //        preservedcell.RowIndex = cell.RowIndex;
            //        preservedcell.ColumnIndex = cell.ColumnIndex;
            //        preservedcell.Background = cell.Background;
            //        preservedcell.ColumnSpan = cell.ColumnSpan;
            //        preservedcell.RowSpan = cell.RowSpan;
                    
            //        foreach (BlockAdv b in cell.Blocks)
            //        {
            //            preservedcell.Blocks.Add(b.CopyBlock());
            //        }
            //        history.DeletedCellsInfo.Add(preservedcell);
            //    }
            //}
            
            OwnerControl.PositionHandler.DeleteTable();

            IsSelected = false;

            OwnerControl.History.RecordUndo(history);
        }

        internal void DeleteRow()
        {
            if (OwnerControl.IsReadOnly)
                return;

            if (!TextPosition.Paragraph.IsInsideTable)
                return;

            TableRowAdv currentrow = TextPosition.Paragraph.AssociatedCell.OwnerRow;
            TableAdv parent = currentrow.Owner;

            DeletedRowHistory history = new DeletedRowHistory();
            history.Action = Actions.DeleteRow;
            TextPosition.VirtualPosition = TextPosition.Index;            
            history.StartPosition = TextPosition.CopyForHistory();
            
            SelectedCellsInfo selectedcells = GetSelectedCellsInfo();

            if (IsSelected)
            {
                if (OwnerControl.Selection.Start != null && OwnerControl.Selection.End != null)
                {
                    history.UndoType = UndoType.SelectionBased;
                    history.StartPosition = OwnerControl.Selection.Start.CopyForHistory();
                    history.EndPosition = OwnerControl.Selection.End.CopyForHistory();
                    history.IsCellSelected = OwnerControl.Selection.IsCellSelected;
                }
            }

            if (parent.Rows.Count == 1 || selectedcells.RowsCount==parent.Rows.Count)
            {
                history.CanDeleteTable = true;
                history.DeletedTable = parent.CopyBlock() as TableAdv;
                if (parent.IsInsideTable)
                {
                    history.DeletedTableIndex = parent.AssociatedCell.Blocks.IndexOf(parent);
                }
                else
                {
                    history.DeletedTableIndex = Document.Sections[0].Blocks.IndexOf(parent);
                }
            }

            if (IsSelected && selectedcells.Owner != null)
            {
                TableAdv table = selectedcells.Owner;
                List<TableCellAdv> cells = new List<TableCellAdv>();
                int i = 0;
                while (i < selectedcells.RowsCount)
                {
                    currentrow = table.Rows[selectedcells.StartRowIndex];
                    parent.GetRowSpannedCellsIntersectingWithGivenRow(currentrow,ref cells);
                    history.DeletedRows.Add(currentrow);
                    OwnerControl.PositionHandler.DeleteRow(currentrow);
                    i++;
                }
                history.RowSpanAffectedCells = cells;
                history.RowIndex = selectedcells.StartRowIndex;
                history.RowsCount = selectedcells.RowsCount;
                IsSelected = false;
            }
            else
            {
                List<TableCellAdv> cells = new List<TableCellAdv>();
                history.RowIndex = parent.Rows.IndexOf(currentrow);
                history.RowsCount = 1;
                history.DeletedRows.Add(currentrow.CopyFromGivenRow(false, false));
                parent.GetRowSpannedCellsIntersectingWithGivenRow(currentrow, ref cells);
                history.RowSpanAffectedCells = cells;
                OwnerControl.PositionHandler.DeleteRow(currentrow);
            }

            OwnerControl.History.RecordUndo(history);
        }

        internal void DeleteColumn()
        {
            if (OwnerControl.IsReadOnly)
                return;

            if (!TextPosition.Paragraph.IsInsideTable)
                return;

            TableAdv parent = TextPosition.Paragraph.AssociatedCell.OwnerTable;

            int mainIndex = TextPosition.Paragraph.AssociatedCell.ColumnIndex;
            
            SelectedCellsInfo cellsInfo = GetSelectedCellsInfo();

            List<TableRowAdv> deletedrows = new List<TableRowAdv>();

            DeletedColumnHistory history = new DeletedColumnHistory();
            history.StartPosition = TextPosition.CopyForHistory();
            history.Action = Actions.DeleteColumn;
            bool canarrange = false;
            BlockAdv currentblock = null;

            if (IsSelected)
            {
                if (OwnerControl.Selection.Start != null && OwnerControl.Selection.End != null)
                {
                    history.StartPosition = OwnerControl.Selection.Start.CopyForHistory();
                    history.EndPosition = OwnerControl.Selection.End.CopyForHistory();
                    history.UndoType = UndoType.SelectionBased;
                    history.IsCellSelected = OwnerControl.Selection.IsCellSelected;
                }
            }

            if (parent != null)
            {
                bool deletedtable = false;
                if (IsSelected)
                {
                    deletedtable = cellsInfo.ColumnsCount == cellsInfo.Owner.TableHolder.Columns.Count;
                }
                else
                {
                    deletedtable = parent.TableHolder.Columns.Count == 1;
                }
                if (deletedtable)
                {
                    history.CanDeleteTable = true;
                    if (parent.NextBlock != null)
                    {
                        currentblock = parent.NextBlock;
                    }
                    parent.ClearLines();
                    history.DeletedTable = parent.CopyBlock() as TableAdv;

                    PreservedCellsInfo preserved = new PreservedCellsInfo();

                    foreach (TableRowAdv row2 in parent.Rows)
                    {
                        foreach (TableCellAdv cell2 in row2.Cells)
                        {
                            PreservedCellsInfo pre = new PreservedCellsInfo();
                            pre.RowIndex = cell2.RowIndex;
                            pre.ColumnIndex = cell2.ColumnIndex;
                            pre.TableCell = cell2.CreateNewCell(false, false);
                            foreach (BlockAdv b in cell2.Blocks)
                            {
                                pre.Blocks.Add(b.CopyBlock());
                            }
                            history.DeletedCellsInfo.Add(pre);
                        }
                    }
                    int index = 0;
                    if (parent.IsInsideTable)
                    {
                        index = parent.AssociatedCell.Blocks.IndexOf(parent);
                        parent.AssociatedCell.Blocks.Remove(parent);
                    }
                    else
                    {
                        index = Document.Sections[0].Blocks.IndexOf(parent);
                        Document.Sections[0].Blocks.Remove(parent);
                    }
                    history.DeletedTableIndex = index;

                    OwnerControl.Viewer.SetPreviousBlocks();

                    OwnerControl.Viewer.SetIsArrangedToFalse();

                    parent.PreviousBlock.ArrangeElements();

                    OwnerControl.Viewer.SetIsArrangedToFalse();

                    if (currentblock != null)
                    {
                        BlockAdv tempblk = currentblock;
                        while (currentblock.IsTable)
                        {
                            tempblk = (tempblk as TableAdv).GetFirstBlockInFirstCell();
                        }
                        TextPosition.Paragraph = tempblk as ParagraphAdv;
                    }

                    TextPosition.SetIndex("0");

                    canarrange = true;
                }
                else
                {
                    if (!deletedtable)
                    {
                        List<TableCellAdv> templist = new List<TableCellAdv>();

                        if (IsSelected)
                        {
                            mainIndex = cellsInfo.StartColumnIndex;
                        }
                        else
                        {
                            mainIndex = TextPosition.Paragraph.AssociatedCell.ColumnIndex;
                            cellsInfo.ColumnsCount = 1;
                        }

                        for (int j = 0; j < cellsInfo.ColumnsCount; j++)
                        {
                            if (j > 0)
                            {
                                mainIndex++;
                                if (mainIndex > cellsInfo.EndColumnIndex)
                                {
                                    break;
                                }
                            }
                            for (int r = 0; r < parent.Rows.Count; r++)
                            {
                                TableRowAdv row = parent.Rows[r];

                                for (int i = 0; i < row.Cells.Count; i++)
                                {
                                    TableCellAdv cell = row.Cells[i];

                                    if ((cell.ColumnIndex > mainIndex) || ((cell.ColumnIndex + cell.ColumnSpan) <= mainIndex))
                                        continue;

                                    templist.Add(cell);
                                    break;
                                }
                            }

                            foreach (TableCellAdv c in templist)
                            {
                                PreservedCellsInfo preserved = new PreservedCellsInfo();
                                preserved.RowIndex = c.RowIndex;
                                preserved.ColumnIndex = c.ColumnIndex;

                                foreach (BlockAdv b in c.Blocks)
                                {
                                    preserved.Blocks.Add(b.CopyBlock());
                                }
                                preserved.TableCell = c.CreateNewCell(false, false);

                                if (c.ColumnSpan > 1)
                                {
                                    preserved.IsColumnSpanChanged = true;
                                    c.ColumnSpan--;
                                }
                                else
                                {
                                    c.OwnerRow.Cells.Remove(c);
                                }
                                history.DeletedCellsInfo.Add(preserved);
                            }
                            //parent.MeasureElements();
                            templist.Clear();
                        }
                        List<TableCellAdv> intersected = new List<TableCellAdv>();
                        foreach (TableRowAdv row3 in parent.Rows)
                        {
                            if (row3.Cells.Count == 0)
                            {
                                parent.GetRowSpannedCellsIntersectingWithGivenRow(row3,ref intersected);
                                List<TableRowAdv> rows = new List<TableRowAdv>();
                                rows.Add(row3.CopyFromGivenRow(false, false));
                                DeletedRowHistory deleted = new DeletedRowHistory(parent.Rows.IndexOf(row3), rows);
                                deleted.RowSpanAffectedCells = intersected;
                                history.DeletedRows.Add(deleted);
                                deletedrows.Add(row3);
                            }
                        }

                        foreach (TableRowAdv row4 in deletedrows)
                        {
                            if (row4.Cells.Count == 0)
                            {
                                OwnerControl.PositionHandler.DeleteRow(row4);
                            }
                        }
                    }

                    parent.MeasureElements();

                    OwnerControl.Viewer.SetPreviousBlocks();

                    OwnerControl.Viewer.SetIsArrangedToFalse();

                    parent.ArrangeElements();

                    OwnerControl.Viewer.SetIsArrangedToFalse();
                    BlockAdv firstblock =null;

                    if (parent.Rows.First().Cells.Count > 0)
                    {
                        if (parent.Rows.First().Cells.Count - 1 < mainIndex)
                        {
                            firstblock = parent.Rows.First().Cells.Last().Blocks.First();
                        }
                        else
                        {
                            firstblock = parent.Rows.First().Cells[mainIndex].Blocks.First();
                        }

                        if (firstblock != null)
                        {
                            if (firstblock.IsParagraph)
                            {
                                TextPosition.Paragraph = (ParagraphAdv)firstblock;
                                TextPosition.SetIndex("0");
                            }
                            else if (firstblock.IsTable)
                            {
                                while (firstblock.IsTable)
                                {
                                    firstblock = ((TableAdv)firstblock).GetFirstBlockInFirstCell();
                                }
                                TextPosition.Paragraph = firstblock as ParagraphAdv;
                                TextPosition.SetIndex("0");
                            }
                        }
                    }
                    canarrange = true;
                }
                IsSelected = false;
                OwnerControl.History.RecordUndo(history);

                if (canarrange)
                    OwnerControl.PositionHandler.InvalidateVisibleRegion();
            }
        }

        internal void InsertTable(int rowcount, int colcount)
        {
            if (OwnerControl.IsReadOnly || rowcount == 0 || colcount == 0)
                return;

            OwnerControl.PositionHandler.InsertTable(rowcount, colcount);
        }

        internal void InsertRow(RowPlacement rowplace)
        {
            bool canarrange = false;
            int index = 0;
            TextPosition startPos = null;
            TextPosition endPos = null;

            if (OwnerControl.IsReadOnly)
                return;

            InsertedRowHistory history = new InsertedRowHistory();
            history.Action = Actions.InsertRow;
            TextPosition.VirtualPosition = TextPosition.Index;
            history.StartPosition = TextPosition.CopyForHistory();
            history.RowPlacement = rowplace;
            
            TableRowAdv currentrow = null;

            if (!TextPosition.Paragraph.IsInsideTable)
                return;

            SelectedCellsInfo cellsInfo = GetSelectedCellsInfo();

            if (IsSelected && cellsInfo.Owner !=null)
            {
                if (OwnerControl.Selection.Start != null && OwnerControl.Selection.End != null)
                {
                    history.UndoType = UndoType.SelectionBased;
                    history.StartPosition = OwnerControl.Selection.Start;
                    history.EndPosition = OwnerControl.Selection.End;
                    history.IsCellSelected = OwnerControl.Selection.IsCellSelected;
                }
                for (int i = 0; i < cellsInfo.Owner.Rows.Count; i++)
                {
                    if (rowplace == RowPlacement.Below)
                    {
                        if (i == cellsInfo.EndRowIndex)
                        {
                            currentrow = cellsInfo.Owner.Rows[i];
                        }
                    }
                    else if (rowplace == RowPlacement.Above)
                    {
                        if (i == cellsInfo.StartRowIndex)
                        {
                            currentrow = cellsInfo.Owner.Rows[i];
                        }
                    }
                }

                for (int j = 0; j < cellsInfo.RowsCount; j++)
                {
                    InsertRow(rowplace, currentrow, ref index);
                }

                int start = index;
                int end = index + cellsInfo.RowsCount - 1;

                if (rowplace == RowPlacement.Above)
                {
                    start = cellsInfo.StartRowIndex;
                    end = cellsInfo.StartRowIndex + cellsInfo.RowsCount - 1;
                    index = cellsInfo.StartRowIndex;
                }

                startPos = new TextPosition(Document);
                TableRowAdv row5 = currentrow.Owner.Rows[start];
                startPos.Paragraph = row5.Cells[0].Blocks[0] as ParagraphAdv;
                startPos.SetIndex("0");

                endPos = new TextPosition(Document);
                TableRowAdv row6 = currentrow.Owner.Rows[end];
                endPos.Paragraph = row6.Cells.Last().Blocks.Last() as ParagraphAdv;
                endPos.SetIndex(endPos.Paragraph.Length());

                history.RowsCount = cellsInfo.RowsCount;
                canarrange = true;
            }
            else
            {
                currentrow = TextPosition.Paragraph.AssociatedCell.OwnerRow;
                InsertRow(rowplace, currentrow, ref index);
                startPos = new Controls.TextPosition(Document);
                startPos.Paragraph = currentrow.Owner.Rows[index].Cells.First().Blocks.First() as ParagraphAdv;
                startPos.SetIndex("0");

                endPos = new Controls.TextPosition(Document);
                endPos.Paragraph = currentrow.Owner.Rows[index].Cells.Last().Blocks.Last() as ParagraphAdv;
                endPos.SetIndex(endPos.Paragraph.Length());
                history.RowsCount = 1;
                canarrange = true;
            }
            history.RowIndex = index;
            OwnerControl.History.RecordUndo(history);

            if (canarrange)
            {
                SetIsArrangedToFalse();
                SetVisibleLinesToPage();
                SetIsArrangedToFalse();
                if (startPos != null && endPos != null)
                {
                    OwnerControl.Selection.Select(startPos, endPos);
                }
            }

            OwnerControl.History.CheckForClearingRedo();
        }

        internal void InsertRow(RowPlacement rowplace,TableRowAdv currentrow,ref int index)
        {
            TableRowAdv newrow = null;
            if (rowplace == RowPlacement.Below)
            {
                TableAdv tableparent = currentrow.Owner;

                //newrow = currentrow.CopyFromGivenRow(true, true);
                newrow = currentrow.CreateNewRow();

                foreach (TableCellAdv cell3 in currentrow.Cells)
                {
                    if (cell3.HasRowSpan())
                    {
                        cell3.RowSpan++;
                        continue;
                    }
                    TableCellAdv newcell = cell3.CreateNewCell(true, false);
                    newrow.Cells.Add(newcell);
                }

                List<TableCellAdv> rowspannedcells = new List<TableCellAdv>();
                tableparent.GetRowSpannedCellsIntersectingWithGivenRow(currentrow,ref rowspannedcells);

                foreach (TableCellAdv cell2 in rowspannedcells)
                {
                    if (cell2.HasRowSpan())
                    {
                        cell2.RowSpan++;
                    }
                }
                newrow.Owner = tableparent;
                index = tableparent.Rows.IndexOf(currentrow);
                index++;
                tableparent.InsertRowAtIndex(newrow, index);
            }
            else if (rowplace == RowPlacement.Above)
            {
                TableAdv tableparent = currentrow.Owner;

                newrow = currentrow.CreateNewRow();

                foreach (TableCellAdv cell in currentrow.Cells)
                {
                    //if (cell.HasRowSpan())
                    //{
                    //    cell.RowSpan++;
                    //    continue;
                    //}
                    TableCellAdv newcell = cell.CreateNewCell(true, false);
                    newrow.Cells.Add(newcell);
                }

                List<TableCellAdv> rowspannedcells = new List<TableCellAdv>();
                
                tableparent.GetRowSpannedCellsIntersectingWithGivenRow(currentrow,ref rowspannedcells);

                foreach (TableCellAdv cell2 in rowspannedcells)
                {
                    if (cell2.HasRowSpan())
                    {
                        cell2.RowSpan++;
                    }
                }
                index = tableparent.Rows.IndexOf(currentrow);
                newrow.Owner = tableparent;
                tableparent.InsertRowAtIndex(newrow, index);
            }
        }

        internal void InsertColumn(ColumnPlacement columnplacement)
        {
            TextPosition startPos = null;
            TextPosition endPos = null;

            if (OwnerControl.IsReadOnly)
                return;

            bool arrange = false;
            int index = 0;

            if (!TextPosition.Paragraph.IsInsideTable)
                return;

            List<TableCellAdv> inserted = new List<TableCellAdv>();

            InsertedColumnHistory history = new InsertedColumnHistory();
            history.Action = Actions.InsertColumn;
            TextPosition.VirtualPosition = TextPosition.Index;
            history.StartPosition = TextPosition.CopyForHistory();
            history.ColumnPlace = columnplacement;
            SelectedCellsInfo cellsInfo = GetSelectedCellsInfo();

            if (IsSelected && cellsInfo.Owner !=null)
            {
                if (OwnerControl.Selection.Start != null && OwnerControl.Selection.End != null)
                {
                    history.StartPosition = OwnerControl.Selection.Start.CopyForHistory();
                    history.EndPosition = OwnerControl.Selection.End.CopyForHistory();
                    history.UndoType = UndoType.SelectionBased;
                    history.IsCellSelected = OwnerControl.Selection.IsCellSelected;
                }
                if (columnplacement == ColumnPlacement.Left)
                {
                    index = cellsInfo.StartColumnIndex;
                }
                else if (columnplacement == ColumnPlacement.Right)
                {
                    index = cellsInfo.EndColumnIndex +1;
                }

                for (int i = 0; i < cellsInfo.ColumnsCount; i++)
                {
                    InsertColumn(columnplacement, cellsInfo.Owner, ref index, ref inserted);
                }

                List<TableCellAdv> cells1 = GetSelectedColumnCells(cellsInfo.Owner, index);

                startPos = new TextPosition(Document);
                //TableCellAdv tablecell = cellsInfo.Owner.Rows.First().Cells[index];
                TableCellAdv tablecell = cells1.First();
                startPos.Paragraph = tablecell.Blocks.First() as ParagraphAdv;
                startPos.SetIndex("0");

                List<TableCellAdv> cells2=GetSelectedColumnCells(cellsInfo.Owner,index+cellsInfo.ColumnsCount-1);

                endPos = new TextPosition(Document);
                //TableCellAdv tablecell2 = cellsInfo.Owner.Rows.Last().Cells[index + cellsInfo.ColumnsCount - 1];
                TableCellAdv tablecell2 = cells2.Last();
                endPos.Paragraph = tablecell2.Blocks.Last() as ParagraphAdv;
                endPos.SetIndex(endPos.Paragraph.Length());

                arrange = true;
            }
            else
            {
                TableAdv table = TextPosition.Paragraph.AssociatedCell.OwnerTable;
                if (columnplacement == ColumnPlacement.Left)
                {
                    index = TextPosition.Paragraph.AssociatedCell.ColumnIndex;
                }
                else if (columnplacement == ColumnPlacement.Right)
                {
                    index = TextPosition.Paragraph.AssociatedCell.ColumnIndex + TextPosition.Paragraph.AssociatedCell.ColumnSpan;
                }
                InsertColumn(columnplacement, table, ref index, ref inserted);

                TableCellAdv startcell = null;
                TableCellAdv endcell = null;
                int minvalue = 0;
                int maxvalue = 0;

                foreach (TableCellAdv cell3 in inserted)
                {
                    minvalue = Math.Min(minvalue, cell3.RowIndex);
                    maxvalue = Math.Max(maxvalue, cell3.RowIndex);
                }

                foreach (TableCellAdv cell4 in inserted)
                {
                    if (cell4.RowIndex == minvalue)
                    {
                        startcell = cell4;
                    }
                    if (cell4.RowIndex == maxvalue)
                    {
                        endcell = cell4;
                    }
                }

                //startPos = new Controls.TextPosition(Document);
                //startPos.Paragraph = startcell.Blocks.First() as ParagraphAdv;
                //startPos.SetIndex("0");

                //endPos = new Controls.TextPosition(Document);
                //endPos.Paragraph = endcell.Blocks.Last() as ParagraphAdv;
                //endPos.SetIndex(endPos.Paragraph.Length());

                OwnerControl.PositionHandler.SelectColumn(index);

                arrange = true;
            }

            foreach (TableCellAdv cell in inserted)
            {
                history.InsertedCellsInfo.Add(new PreservedCellsInfo { RowIndex = cell.RowIndex, ColumnIndex = cell.ColumnIndex });
            }
            OwnerControl.History.RecordUndo(history);

            if (arrange)
            {   
                OwnerControl.Viewer.SetIsArrangedToFalse();
                OwnerControl.Viewer.SetVisibleLinesToPage();
                OwnerControl.Viewer.SetIsArrangedToFalse();
                if (startPos != null && endPos != null)
                {
                    OwnerControl.Selection.Select(startPos, endPos);
                }
            }

            OwnerControl.History.CheckForClearingRedo();
        }

        internal List<TableCellAdv> GetSelectedColumnCells(TableAdv table,int index)
        {
            List<TableCellAdv> cellsToSelect = new List<TableCellAdv>();

            foreach (TableRowAdv row in table.Rows)
            {
                TableCellAdv cellAdv = null;
                foreach (TableCellAdv cell in row.Cells)
                {
                    if ((index < (cell.ColumnIndex + cell.ColumnSpan)) && (index >= cell.ColumnIndex))
                    {
                        cellAdv = cell;
                        break;
                    }
                }
                if (cellAdv != null)
                {
                    cellsToSelect.Add(cellAdv);
                }
            }
            return cellsToSelect;
        }

        internal void InsertColumn(ColumnPlacement columnplace, TableAdv table, ref int index,ref List<TableCellAdv> inserted)
        {
            if (columnplace == ColumnPlacement.Left)
            {
                table.InsertColumnAtIndex(index,ref inserted);
            }
            if (columnplace == ColumnPlacement.Right)
            {
                //index = TextPosition.Paragraph.AssociatedCell.ColumnIndex + TextPosition.Paragraph.AssociatedCell.ColumnSpan;
                table.InsertColumnAtIndex(index,ref inserted);
            }
        }


        internal SelectedCellsInfo GetSelectedCellsInfo()
        {
            List<TableCellAdv> selectedcells = OwnerControl.Selection.SelectedCellsInTable();
            SelectedCellsInfo cellsInfo = null;
            List<int> rownos = new List<int>();
            List<int> colnos = new List<int>();

            if (TextPosition.Paragraph.IsInsideTable)
            {
                cellsInfo = new SelectedCellsInfo();
                BlockAdv selected = IsSelected && OwnerControl.Selection.Blocks.Count > 0 ? OwnerControl.Selection.Blocks.First() : null;
                if(selectedcells.Count==1)
                {
                    cellsInfo.Owner = selectedcells[0].OwnerTable;
                    cellsInfo.ColumnsCount = selectedcells[0].ColumnSpan;
                    cellsInfo.StartColumnIndex = selectedcells[0].ColumnIndex;
                    cellsInfo.EndColumnIndex = selectedcells[0].ColumnIndex + selectedcells[0].ColumnSpan - 1;
                    cellsInfo.StartRowIndex = selectedcells[0].RowIndex;
                    cellsInfo.EndRowIndex = selectedcells[0].RowIndex;
                    cellsInfo.RowsCount = selectedcells[0].RowSpan;
                }
                else if (selectedcells.Count > 0)
                {
                    cellsInfo.Owner = selectedcells[0].OwnerTable;
                    foreach (TableCellAdv cell in selectedcells)
                    {
                        if (selectedcells.First() == cell)
                        {
                            cellsInfo.StartRowIndex = cell.OwnerTable.Rows.IndexOf(cell.OwnerRow);
                            cellsInfo.StartColumnIndex = cell.ColumnIndex;
                        }
                        if (selectedcells.Last() == cell)
                        {
                            cellsInfo.EndRowIndex = cell.OwnerTable.Rows.IndexOf(cell.OwnerRow);
                            cellsInfo.EndColumnIndex = cell.ColumnIndex + cell.ColumnSpan - 1;
                        }
                        if (!rownos.Contains(cell.OwnerTable.Rows.IndexOf(cell.OwnerRow)))
                        {
                            rownos.Add(cell.OwnerTable.Rows.IndexOf(cell.OwnerRow));
                        }
                        if (!colnos.Contains(cell.ColumnIndex))
                        {
                            for (int i = 0; i < cell.ColumnSpan; i++)
                            {
                                colnos.Add(cell.ColumnIndex);
                            }
                        }
                    }

                    cellsInfo.RowsCount = rownos.Count;
                    cellsInfo.ColumnsCount = colnos.Count;
                }
                else
                {
                    if (selected != null && selected.AssociatedCell !=null)
                    {
                        cellsInfo.StartRowIndex = selected.AssociatedCell.OwnerTable.Rows.IndexOf(selected.AssociatedCell.OwnerRow);
                        cellsInfo.EndRowIndex = selected.AssociatedCell.OwnerTable.Rows.IndexOf(selected.AssociatedCell.OwnerRow);
                        cellsInfo.StartColumnIndex = selected.AssociatedCell.ColumnIndex;
                        cellsInfo.EndColumnIndex = selected.AssociatedCell.ColumnIndex;
                        cellsInfo.Owner = selected.AssociatedCell.OwnerTable;
                        cellsInfo.RowsCount = 1;
                        cellsInfo.ColumnsCount = 1;
                    }
                }
            }
            return cellsInfo;
        }

        internal void ChangeTableCellBackground(Color color)
        {
            TableCellStyleHistoryInfo cellstyleInfo = new TableCellStyleHistoryInfo();
            cellstyleInfo.Action = Actions.TableCellBackground;
            if (IsSelected)
            {
                cellstyleInfo.UndoType = UndoType.SelectionBased;
                if (OwnerControl.Selection.Start != null && OwnerControl.Selection.End != null && OwnerControl.Selection.Start.IsInSameTable(OwnerControl.Selection.End))
                {
                    cellstyleInfo.StartPosition = OwnerControl.Selection.Start.CopyForHistory();
                    cellstyleInfo.EndPosition = OwnerControl.Selection.End.CopyForHistory();
                    List<TableCellAdv> selectedcells = OwnerControl.Selection.SelectedCellsInTable();
                    cellstyleInfo.Table = selectedcells[0].OwnerTable;
                    foreach (TableCellAdv cell in selectedcells)
                    {
                        PreservedCellsInfo preserved = new PreservedCellsInfo();
                        preserved.RowIndex = cell.RowIndex;
                        preserved.ColumnIndex = cell.ColumnIndex;
                        Path path=cell.CellElementBox.AssociatedPath;
                        preserved.Background = (path.Fill as SolidColorBrush).Color;
                        cell.Background = color;
                        path.Fill = new SolidColorBrush(color);
                        cellstyleInfo.PreservedCells.Add(preserved);
                    }
                }
                else
                    return;
            }
            else
            {
                TableCellAdv cell = TextPosition.Paragraph.AssociatedCell;

                if (cell != null)
                {
                    cellstyleInfo.Table = cell.OwnerTable;
                    cellstyleInfo.StartPosition = TextPosition.CopyForHistory();
                    cell.Background = color;

                    Path path = cell.CellElementBox.AssociatedPath;
                    cellstyleInfo.TableCellBackground = (path.Fill as SolidColorBrush).Color;
                    path.Fill = new SolidColorBrush(color);
                }
            }
            OwnerControl.History.RecordUndo(cellstyleInfo);
        }

        internal void ChangeTableBorderColor(Color color)
        {
            TableCellStyleHistoryInfo cellstyleInfo = new TableCellStyleHistoryInfo();
            cellstyleInfo.Action = Actions.TableBorderColor;
            if (IsSelected)
            {
                if (OwnerControl.Selection.Start != null && OwnerControl.Selection.End != null && OwnerControl.Selection.Start.IsInSameTable(OwnerControl.Selection.End))
                {
                    cellstyleInfo.StartPosition = OwnerControl.Selection.Start.CopyForHistory();
                    cellstyleInfo.EndPosition = OwnerControl.Selection.End.CopyForHistory();
                    List<TableCellAdv> selectedcells = OwnerControl.Selection.SelectedCellsInTable();
                    TableAdv table = selectedcells[0].OwnerTable;
                    cellstyleInfo.Table = table;
                    table.BorderBrush = color;
                    cellstyleInfo.BorderColor = (selectedcells[0].CellElementBox.AssociatedPath.Fill as SolidColorBrush).Color;
                    foreach (TableRowAdv row in table.Rows)
                    {
                        foreach (TableCellAdv cell in row.Cells)
                        {
                            Path path = cell.CellElementBox.AssociatedPath;
                            if(path !=null)
                            {                            
                                path.Stroke = new SolidColorBrush(color);
                            }
                        }
                    }
                }
                else
                    return;
            }
            else
            {
                TableCellAdv cell = TextPosition.Paragraph.AssociatedCell;
                if (cell != null)
                {
                    TableAdv table = cell.OwnerTable;
                    cellstyleInfo.Table = table;
                    table.BorderBrush = color;
                    cellstyleInfo.BorderColor = (cell.CellElementBox.AssociatedPath.Fill as SolidColorBrush).Color;
                    foreach (TableRowAdv row in table.Rows)
                    {
                        foreach (TableCellAdv cell2 in row.Cells)
                        {
                            Path path = cell.CellElementBox.AssociatedPath;
                            if (path != null)
                            {
                                path.Stroke = new SolidColorBrush(color);
                            }
                        }
                    }
                }
            }
            OwnerControl.History.RecordUndo(cellstyleInfo);
        }

        internal void ChangeBorderThickness(double value)
        {
            TableCellStyleHistoryInfo cellstyleInfo = new TableCellStyleHistoryInfo();
            cellstyleInfo.Action = Actions.TableBorderThickness;
            if (IsSelected)
            {
                if (OwnerControl.Selection.Start != null && OwnerControl.Selection.End != null && OwnerControl.Selection.Start.IsInSameTable(OwnerControl.Selection.End))
                {
                    cellstyleInfo.StartPosition = OwnerControl.Selection.Start.CopyForHistory();
                    cellstyleInfo.EndPosition = OwnerControl.Selection.End.CopyForHistory();
                    List<TableCellAdv> selectedcells = OwnerControl.Selection.SelectedCellsInTable();
                    TableAdv table = selectedcells[0].OwnerTable;
                    cellstyleInfo.Table = table;
                    table.BorderThickness = value;
                    cellstyleInfo.BorderThickness = selectedcells[0].CellElementBox.AssociatedPath.StrokeThickness;

                    foreach (TableRowAdv row in table.Rows)
                    {
                        foreach (TableCellAdv cell in row.Cells)
                        {
                            cell.CellElementBox.AssociatedPath.StrokeThickness = value;
                        }
                    }
                }
                else
                    return;
            }
            else
            {
                TableCellAdv cell = TextPosition.Paragraph.AssociatedCell;
                if (cell != null)
                {
                    TableAdv table = cell.OwnerTable;
                    cellstyleInfo.Table = table;
                    cellstyleInfo.BorderThickness = cell.CellElementBox.AssociatedPath.StrokeThickness;
                    table.BorderThickness = value;
                    foreach (TableRowAdv row in table.Rows)
                    {
                        foreach (TableCellAdv cell2 in row.Cells)
                        {
                            cell2.CellElementBox.AssociatedPath.StrokeThickness = value;
                        }
                    }
                }
            }
            OwnerControl.History.RecordUndo(cellstyleInfo);
        }

        internal void SelectCell()
        {
            if (OwnerControl.IsReadOnly)
                return;

            OwnerControl.PositionHandler.SelectCell();
        }

        internal void SelectRow()
        {
            if (OwnerControl.IsReadOnly)
                return;

            OwnerControl.PositionHandler.SelectRow();
        }

        internal void SelectColumn()
        {
            if (OwnerControl.IsReadOnly)
                return;

            OwnerControl.PositionHandler.SelectColumn();
        }

        internal void SelectTable()
        {
            if (OwnerControl.IsReadOnly)
                return;

            OwnerControl.PositionHandler.SelectTable();
        }

        internal void MergeSelectedCells()
        {
            if (OwnerControl.IsReadOnly)
                return;

            OwnerControl.PositionHandler.MergeSelectedCells();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="e"></param>
        /// <param name="canvas"></param>
        internal void HandleControlDownKey()
        {
            if (TextPosition != null && TextPosition.Paragraph != null
                && TextPosition.Paragraph.NextBlock != null)
            {
                TextPosition.Paragraph = TextPosition.Paragraph.NextBlock as ParagraphAdv;
                TextPosition.Index = "0";
                PositionCursor();
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="e"></param>
        /// <param name="canvas"></param>
        internal void HandleControlUpKey()
        {
            if (TextPosition != null && TextPosition.Paragraph != null
                && TextPosition.Paragraph.PreviousBlock != null)
            {
                TextPosition.Paragraph = TextPosition.Paragraph.PreviousBlock as ParagraphAdv;
                TextPosition.Index = "0";
                PositionCursor();
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="e"></param>
        /// <param name="canvas"></param>
        internal void HandleControlHomeKey()
        {
            if (Document != null)
            {
                MoveToPosition(Document.DocumentStart());
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="e"></param>
        /// <param name="canvas"></param>
        internal void HandleControlEndKey()
        {
            if (Document != null)
            {
                MoveToPosition(Document.DocumentEnd());
            }
        }

        /// <summary>
        /// 
        /// </summary>
        internal void HandleShiftUpArrow()
        {
            LineInfo line = GetPreviousLineInfo();
            if (line != null)
            {
                bool isBegin = false;
                TextPosition pos = GetTextPosition(line, UpDownSelectionWidth, ref isBegin);
                if (OwnerControl != null && OwnerControl.Selection != null && pos != null)
                {
                    if (IsSelected)
                    {
                        if (!OwnerControl.Selection.Start.IsGreaterThan(pos) && !OwnerControl.Selection.Start.IsEqual(pos))
                            OwnerControl.Selection.End = pos;
                        else
                            OwnerControl.Selection.Start = pos;
                    }
                    else
                    {
                        OwnerControl.Selection.Start = pos;
                        OwnerControl.Selection.End = TextPosition;
                    }
                    if (isBegin)
                    {
                        PageAdv page = GetPageFromLine(line);
                        if (page != null)
                            page.UpdateCaretPosition(new Point(line.BoundingRectangle.Left, line.BoundingRectangle.Top + line.BoundingRectangle.Height / 2));
                    }
                    else
                    {
                        MoveToPosition(pos);
                    }
                    OwnerControl.Selection.Select();
                    CheckForCursorVisibility(false);
                }
            }
        }

        /// <summary>
        /// 
        /// </summary>
        internal void HandleShiftDownArrow()
        {
            LineInfo line = GetNextLineInfo();
            if (line != null)
            {
                bool isBegin = false;
                TextPosition pos = GetTextPosition(line, UpDownSelectionWidth, ref isBegin);
                if (OwnerControl != null && OwnerControl.Selection != null && pos != null)
                {
                    if (IsSelected)
                    {
                        if (!OwnerControl.Selection.End.IsGreaterThan(pos) && !OwnerControl.Selection.End.IsEqual(pos))
                            OwnerControl.Selection.End = pos;
                        else
                            OwnerControl.Selection.Start = pos;
                    }
                    else
                    {
                        OwnerControl.Selection.Start = TextPosition.Copy();
                        OwnerControl.Selection.End = pos;
                    }

                    if (isBegin)
                    {
                        PageAdv page = GetPageFromLine(line);
                        if (page != null)
                            page.UpdateCaretPosition(new Point(line.BoundingRectangle.Left, line.BoundingRectangle.Top + line.BoundingRectangle.Height / 2));
                    }
                    else
                    {
                        MoveToPosition(pos);
                    }
                    OwnerControl.Selection.Select();
                    CheckForCursorVisibility(false);
                }
            }
        }


        /// <summary>
        /// 
        /// </summary>
        internal void HandleShiftTabKey()
        {
            UseUpDownSelection = false;
            UpDownSelectionWidth = double.NaN;
            TextPosition pos = TextPosition;

            if (pos.Paragraph.IsInsideTable)
            {
                if (IsSelected)
                    IsSelected = false;

                if (pos != null)
                {
                    TableCellAdv prevCell = pos.Paragraph.AssociatedCell.GetPreviousCell();
                    if (prevCell != null)
                    {
                        TextPosition startPos = new TextPosition(Document);
                        TextPosition endPos = new TextPosition(Document);

                        if (prevCell.Blocks.Count > 0)
                        {
                            if (prevCell.Blocks[0].IsParagraph)
                            {
                                pos.Paragraph = prevCell.Blocks[0] as ParagraphAdv;
                            }
                            else
                            {
                                BlockAdv next = prevCell.Blocks[0];
                                while (next.IsTable)
                                {
                                    next = (next as TableAdv).GetFirstBlockInFirstCell();
                                }
                                pos.Paragraph = next as ParagraphAdv;
                            }
                            pos.SetIndex("0");
                            startPos = pos;

                            if (prevCell.Blocks.Count == 1 && prevCell.Blocks[0].IsParagraph && prevCell.Blocks[0].Inlines.Count == 0)
                            {
                                MoveToPosition(startPos);
                                CheckForCursorVisibility(false);
                                return;
                            }

                            BlockAdv finalblk = prevCell.Blocks.Last();
                            if (finalblk.IsParagraph)
                            {
                                endPos.Paragraph = finalblk as ParagraphAdv;
                            }
                            else
                            {
                                BlockAdv next = finalblk;
                                while (next.IsTable)
                                {
                                    next = (next as TableAdv).GetLastBlockInLastCell();
                                }
                                endPos.Paragraph = next as ParagraphAdv;
                            }
                            endPos.SetIndex(endPos.Paragraph.Length());

                            MoveToPosition(endPos);
                            CheckForCursorVisibility(false);

                            OwnerControl.Selection.Select(startPos, endPos);
                        }
                    }
                }

            }
        }

        internal void HandlePageDown()
        {
            IsSelected = false;

            if (this is PageLayoutViewer)
            {
                PageAdv page = (this as PageLayoutViewer).GetPageDownTo(CurrentPage);
                if (page != null && page.LineInfos.Count > 0)
                {
                    //bool isBegin = false;
                    //TextPosition pos = GetTextPosition(page.LineInfos[0], 0, ref isBegin);
                    //MoveToPosition(pos);
                    page.UpdateCaretPosition(new Point(page.LineInfos[0].BoundingRectangle.Left, page.LineInfos[0].BoundingRectangle.Top + page.LineInfos[0].BoundingRectangle.Height / 2));
                }
            }
        }

        internal void HandlePageUp()
        {
            IsSelected = false;

            if (this is PageLayoutViewer)
            {
                PageAdv page = (this as PageLayoutViewer).GetPageTopOf(CurrentPage);
                if (page != null && page.LineInfos.Count > 0)
                {
                    //bool isBegin = false;
                    //TextPosition pos = GetTextPosition(page.LineInfos[0], 0, ref isBegin);
                    //MoveToPosition(pos);
                    page.UpdateCaretPosition(new Point(page.LineInfos[0].BoundingRectangle.Left, page.LineInfos[0].BoundingRectangle.Top + page.LineInfos[0].BoundingRectangle.Height / 2));
                }
            }
        }

        internal void HandleControlPageUp()
        {
            IsSelected = false;

            PageAdv page = GetPreviousPage(CurrentPage);
            if (page != null && page.LineInfos.Count > 0)
            {
                bool isBegin = false;
                TextPosition pos = GetTextPosition(page.LineInfos[0], 0, ref isBegin);
                MoveToPosition(pos);
            }
        }

        internal void HandleControlPageDown()
        {
            IsSelected = false;

            PageAdv page = GetNextPage(CurrentPage);
            if (page != null && page.LineInfos.Count > 0)
            {
                bool isBegin = false;
                TextPosition pos = GetTextPosition(page.LineInfos[0], 0, ref isBegin);
                MoveToPosition(pos);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        internal void HandleShiftLeftArrow()
        {
            UseUpDownSelection = false;
            UpDownSelectionWidth = double.NaN;
            TextPosition prePos = GetPreviousPosition();
            if (OwnerControl != null && OwnerControl.Selection != null && prePos != null)
            {
                if (IsSelected)
                {
                    if (!OwnerControl.Selection.Start.IsGreaterThan(prePos))
                        OwnerControl.Selection.End = prePos;
                    else
                        OwnerControl.Selection.Start = prePos;
                }
                else
                {
                    OwnerControl.Selection.Start = prePos;
                    OwnerControl.Selection.End = TextPosition;
                }
                MoveToPosition(prePos);
                OwnerControl.Selection.Select();
                CheckForCursorVisibility(false);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        internal void HandleShiftRightKey()
        {
            UseUpDownSelection = false;
            UpDownSelectionWidth = double.NaN;
            TextPosition nextPos = GetNextPosition();
            if (OwnerControl != null && OwnerControl.Selection != null && nextPos != null)
            {
                if (IsSelected)
                {
                    if (!OwnerControl.Selection.End.IsGreaterThan(nextPos) && !OwnerControl.Selection.End.IsEqual(nextPos))
                        OwnerControl.Selection.End = nextPos;
                    else
                        OwnerControl.Selection.Start = nextPos;
                }
                else
                {
                    OwnerControl.Selection.Start = TextPosition;
                    OwnerControl.Selection.End = nextPos;
                }
                MoveToPosition(nextPos);
                OwnerControl.Selection.Select();
                CheckForCursorVisibility(false);
            }
        }

        internal void HandleCtrlShiftLeft()
        {
            if (IsSelected)
            {
                TextPosition startPos = OwnerControl.Selection.Start;
                TextPosition endPos = OwnerControl.Selection.End;
                IsSelected = false;
                HandleControlLeft();
                if (!startPos.IsGreaterThan(TextPosition) && !startPos.IsEqual(TextPosition))
                    endPos = TextPosition.Copy();
                else
                    startPos = TextPosition.Copy();

                OwnerControl.Selection.Start = startPos;
                OwnerControl.Selection.End = endPos;
            }
            else
            {
                OwnerControl.Selection.End = TextPosition.Copy();
                HandleControlLeft();
                OwnerControl.Selection.Start = TextPosition.Copy();
            }

            OwnerControl.Selection.Select();
            CheckForCursorVisibility(false);
        }

        internal void HandleCtrlShiftRight()
        {
            if (IsSelected)
            {
                TextPosition startPos = OwnerControl.Selection.Start;
                TextPosition endPos = OwnerControl.Selection.End;
                IsSelected = false;
                HandleControlRight();
                if (!endPos.IsGreaterThan(TextPosition) && !endPos.IsEqual(TextPosition))
                    endPos = TextPosition.Copy();
                else if (!endPos.IsEqual(TextPosition))
                {
                    startPos = TextPosition.Copy();
                    OwnerControl.Selection.End = endPos;
                }

                OwnerControl.Selection.Start = startPos;
                OwnerControl.Selection.End = endPos;
            }
            else
            {
                OwnerControl.Selection.Start = TextPosition.Copy();
                HandleControlRight();
                OwnerControl.Selection.End = TextPosition.Copy();
            }

            OwnerControl.Selection.Select();
            CheckForCursorVisibility(false);
        }

        internal void HandleControlLeft()
        {
            if (IsSelected)
            {
                if (TextPosition.IsEqual(OwnerControl.Selection.End))
                    MoveToPosition(OwnerControl.Selection.Start);
                IsSelected = false;
            }
            else if (TextPosition != null && TextPosition.Paragraph != null)
            {
                if (TextPosition.IsPositionAtParagraphStart && TextPosition.Paragraph.PreviousBlock != null)
                {
                    TextPosition.Index = TextPosition.Paragraph.PreviousBlock.Length();
                    TextPosition.Paragraph = TextPosition.Paragraph.PreviousBlock as ParagraphAdv;
                    MoveToPosition(TextPosition);
                }
                else
                {
                    int indexInInline = 0;
                    int indexInElementBox = 0;
                    Inline inline = OwnerControl.PositionHandler.GetInlineFromTextPosition(ref indexInInline);
                    ElementBox elementBox = OwnerControl.PositionHandler.GetElemenBoxFromIndexInInline(inline, indexInInline, ref indexInElementBox);
                    if (elementBox != null)
                    {
                        TextPosition pos = TextPosition.Paragraph.GetPreviousWordStart(elementBox);
                        if (pos != null)
                        {
                            MoveToPosition(pos);
                        }
                    }
                }
            }
        }

        internal void HandleControlRight()
        {
            if (IsSelected)
            {
                if (TextPosition.IsEqual(OwnerControl.Selection.Start))
                    MoveToPosition(OwnerControl.Selection.End);
                IsSelected = false;
            }
            else if (TextPosition != null && TextPosition.Paragraph != null)
            {
                if (TextPosition.IsPositionAtParagraphEnd && TextPosition.Paragraph.NextBlock != null)
                {
                    TextPosition.Index = "0";
                    TextPosition.Paragraph = TextPosition.Paragraph.NextBlock as ParagraphAdv;
                    MoveToPosition(TextPosition);
                }
                else
                {
                    int indexInInline = 0;
                    int indexInElementBox = 0;
                    Inline inline = OwnerControl.PositionHandler.GetInlineFromTextPosition(ref indexInInline);
                    ElementBox elementBox = OwnerControl.PositionHandler.GetElemenBoxFromIndexInInline(inline, indexInInline, ref indexInElementBox);
                    if (elementBox != null)
                    {
                        TextPosition pos = TextPosition.Paragraph.GetNextWordStart(elementBox);
                        if (pos != null)
                        {
                            MoveToPosition(pos);
                        }
                    }
                }
            }
        }

        /// <summary>
        /// 
        /// </summary>
        internal void HandleLeftKey()
        {
            UseUpDownSelection = false;
            UpDownSelectionWidth = double.NaN;
            if (IsSelected)
            {
                if (TextPosition.IsEqual(OwnerControl.Selection.End))
                    MoveToPosition(OwnerControl.Selection.Start);
                IsSelected = false;
            }
            else
            {
                MoveToPosition(GetPreviousPosition());
            }
        }

        /// <summary>
        /// 
        /// </summary>
        internal void HandleRightKey()
        {
            UseUpDownSelection = false;
            UpDownSelectionWidth = double.NaN;
            if (IsSelected)
            {
                if (TextPosition.IsEqual(OwnerControl.Selection.Start))
                    MoveToPosition(OwnerControl.Selection.End);
                IsSelected = false;
            }
            else
            {
                MoveToPosition(GetNextPosition());
            }
        }

        /// <summary>
        /// Returns the next line to the current line
        /// </summary>
        /// <returns></returns>
        internal LineInfo GetNextLineInfo()
        {
            if (TextPosition != null && TextPosition.LineInfo != null && LineInfos.Count > 0)
            {
                if (!TextPosition.Paragraph.IsInsideTable)
                {
                    if (LineInfos.Last() != TextPosition.LineInfo && LineInfos.Contains(TextPosition.LineInfo))
                    {
                        LineInfo next = LineInfos[LineInfos.IndexOf(TextPosition.LineInfo) + 1];
                        if (next != null)
                        {
                            if (next.IsTableLine)
                            {
                                ElementBox box = next.GetElementBoxFromPoint(new Point(UpDownSelectionWidth, next.Location.Y));
                                TableCellElementBox celbox = box as TableCellElementBox;
                                if (celbox != null)
                                {
                                    return celbox.BaseCell.LineInfos.First();
                                }
                            }
                            else
                            {
                                return next;
                            }
                        }
                    }
                }
                else if (TextPosition.Paragraph.IsInsideTable)
                {
                    if (TextPosition.Paragraph.LineInfo.Last() != TextPosition.LineInfo && TextPosition.Paragraph.LineInfo.Contains(TextPosition.LineInfo))
                    {
                        int index = TextPosition.Paragraph.LineInfo.IndexOf(TextPosition.LineInfo) + 1;
                        return TextPosition.Paragraph.LineInfo[index];
                    }
                    else if (TextPosition.Paragraph.LineInfo.Last() == TextPosition.LineInfo && TextPosition.Paragraph.NextBlock != null)
                    {
                        BlockAdv next = null;
                        next = TextPosition.Paragraph.NextBlock;
                        while (next.IsTable)
                        {
                            next = (next as TableAdv).GetFirstBlockInFirstCell();
                        }
                        return next.LineInfo[0];
                    }
                    else if (TextPosition.Paragraph.LineInfo.First() == TextPosition.LineInfo && TextPosition.Paragraph.NextBlock == null)
                    {
                        if (TextPosition.Paragraph.AssociatedCell.OwnerTable.NextBlock != null)
                        {
                            TableCellElementBox cellBox =null;
                            LineInfo lne = TextPosition.Paragraph.AssociatedCell.CellElementBox.LineInfo;
                            if (lne.IsSplitted)
                            {
                                cellBox = TextPosition.Paragraph.AssociatedCell.CellElementBox.ChildBox;
                                
                                while (cellBox != null)
                                {
                                    if (cellBox.LineInfos.Contains(TextPosition.LineInfo))
                                    {
                                        lne = cellBox.LineInfo;
                                        break;
                                    }
                                    cellBox = cellBox.ChildBox;
                                }    
                            }
                            else if(TextPosition.Paragraph.AssociatedCell.HasRowSpan())
                            {
                                cellBox = TextPosition.Paragraph.AssociatedCell.CellElementBox.BottomCellBox;

                                while (cellBox != null)
                                {
                                    if (cellBox.LineInfo.IsSplitted)
                                    {
                                        cellBox = cellBox.ChildBox;

                                        while (cellBox != null)
                                        {
                                            if (cellBox.LineInfos.Contains(TextPosition.LineInfo))
                                            {
                                                lne = cellBox.LineInfo;
                                                break;
                                            }
                                            cellBox = cellBox.ChildBox;
                                        }
                                    }
                                    if (cellBox.LineInfos.Contains(TextPosition.LineInfo))
                                    {
                                        lne = cellBox.LineInfo;
                                        break;
                                    }
                                    cellBox = cellBox.BottomCellBox;
                                }
                            }
                            
                            if (TextPosition.Paragraph.AssociatedCell.OwnerTable.LineInfo.Last() == lne)
                            {
                                return TextPosition.Paragraph.AssociatedCell.OwnerTable.NextBlock.LineInfo.First();
                            }
                            else
                            {
                                LineInfo nextLine = TextPosition.Paragraph.AssociatedCell.OwnerTable.LineInfo[TextPosition.Paragraph.AssociatedCell.OwnerTable.LineInfo.IndexOf(lne) + 1];
                                ElementBox box = nextLine.GetElementBoxFromPoint(new Point(UpDownSelectionWidth, nextLine.Location.Y));
                                TableCellElementBox celbox = box as TableCellElementBox;
                                if (celbox != null)
                                {
                                    return celbox.BaseCell.LineInfos.First();
                                }
                            }
                        }
                    }

                }
            }

            return null;
        }

        /// <summary>
        /// Returns the previous line to the current line
        /// </summary>
        /// <returns></returns>
        internal LineInfo GetPreviousLineInfo()
        {
            if (!TextPosition.Paragraph.IsInsideTable)
            {
                if (LineInfos.First() != TextPosition.LineInfo && LineInfos.Contains(TextPosition.LineInfo))
                {
                    LineInfo previous = LineInfos[LineInfos.IndexOf(TextPosition.LineInfo) - 1];
                    if (previous != null)
                    {
                        if (previous.IsTableLine)
                        {
                            ElementBox box = previous.GetElementBoxFromPoint(new Point(UpDownSelectionWidth, previous.Location.Y));
                            TableCellElementBox celbox = box as TableCellElementBox;
                            if (celbox != null)
                            {
                                return celbox.BaseCell.LineInfos.Last();
                            }
                        }
                        else
                        {
                            return previous;
                        }
                    }
                }
            }
            else
            {
                if (TextPosition.Paragraph.LineInfo.First() != TextPosition.LineInfo && TextPosition.Paragraph.LineInfo.Contains(TextPosition.LineInfo))
                {
                    int index = TextPosition.Paragraph.LineInfo.IndexOf(TextPosition.LineInfo) - 1;
                    return TextPosition.Paragraph.LineInfo[index];
                }
                else if (TextPosition.Paragraph.LineInfo.First() == TextPosition.LineInfo && TextPosition.Paragraph.PreviousBlock != null)
                {
                    BlockAdv previous = null;
                    previous = TextPosition.Paragraph.PreviousBlock;
                    while (previous.IsTable)
                    {
                        previous = (previous as TableAdv).GetLastBlockInLastCell();
                    }
                    return previous.LineInfo.Count > 0 ? previous.LineInfo[previous.LineInfo.Count - 1] : null;
                }
                else if (TextPosition.Paragraph.LineInfo.First() == TextPosition.LineInfo && TextPosition.Paragraph.PreviousBlock == null)
                {
                    if (TextPosition.Paragraph.AssociatedCell.OwnerTable.PreviousBlock != null)
                    {
                        LineInfo lne = TextPosition.Paragraph.AssociatedCell.CellElementBox.LineInfo;
                        if (TextPosition.Paragraph.AssociatedCell.OwnerTable.LineInfo.First() == lne)
                        {
                            return TextPosition.Paragraph.AssociatedCell.OwnerTable.PreviousBlock.LineInfo.Last();
                        }
                        else
                        {
                            LineInfo prevLine= TextPosition.Paragraph.AssociatedCell.OwnerTable.LineInfo[TextPosition.Paragraph.AssociatedCell.OwnerTable.LineInfo.IndexOf(lne) - 1];
                            ElementBox box = prevLine.GetElementBoxFromPoint(new Point(UpDownSelectionWidth, prevLine.Location.Y));
                            TableCellElementBox celbox = box as TableCellElementBox;
                            if (celbox != null)
                            {
                                return celbox.BaseCell.LineInfos.Last();
                            }
                        }

                    }
                }
            }

            return null;
        }

        /// <summary>
        /// Gets the text position using line and width
        /// </summary>
        /// <param name="line"></param>
        /// <param name="width"></param>
        /// <returns></returns>
        internal TextPosition GetTextPosition(LineInfo line, double width, ref bool isBegin)
        {
            TextPosition pos = null;
            if (line != null && !double.IsNaN(width))
            {
                pos = new TextPosition(Document);
                Point point = new Point(width, line.Location.Y);
                ElementBox element = line.GetElementBoxFromPoint(point);
                pos.Paragraph = line.Block as ParagraphAdv;
                pos.LineInfo = line;
                if ((line.Block as ParagraphAdv).Inlines.Count == 0)
                {
                    pos.SetIndex("0");
                }
                else if (element != null)
                {
                    if (element.IsImageBox || element.IsUIBox)
                    {
                        pos.SetIndex((line.Block as ParagraphAdv).GetIndexFromInlineAndSpanIndex(element.Inline, 1).ToString());
                    }
                    else
                    {
                        double boxWidth = 0;
                        double indexInInline = 0;
                        string text = element.InternalText;
                        if (element.ElementLocation.X + element.ElementSize.Width > width)
                        {
                            boxWidth = width - element.ElementLocation.X;
                            text = GetText(element.InternalText, boxWidth, element);
                            if (string.IsNullOrEmpty(text))
                                isBegin = true;
                        }
                        indexInInline = OwnerControl.PositionHandler.GetSpanPositionToInsertText(element.Inline, element, text);
                        pos.SetIndex(pos.Paragraph.GetIndexFromInlineAndSpanIndex(element.Inline, indexInInline).ToString());
                    }
                }
            }
            return pos;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="Text"></param>
        /// <param name="width"></param>
        /// <returns></returns>
        public string GetText(string Text, double width, ElementBox box)
        {
            int i = 0;
            string text = string.Empty;
            while (i < Text.Length)
            {
                text = Text.Substring(0, i + 1);
                if (Math.Floor(TextHelper.MeasureText(text, box).Width) >= Math.Floor(width))
                {
                    if (Math.Floor(TextHelper.MeasureText(text, box).Width) > Math.Floor(width))
                    {
                        text = Text.Substring(0, i);
                    }
                    break;
                }
                i++;
            }
            return text;
        }

        /// <summary>
        /// Removes the line from page
        /// </summary>
        /// <param name="lineInfo"></param>
        /// <param name="page"></param>
        internal void RemoveLineFromPage(LineInfo lineInfo, PageAdv page)
        {
            RenderingManager.Remove(lineInfo, page);

            lineInfo.RenderingOption = RenderingOptions.Render;

            foreach (UIElement element in lineInfo.Elements)
            {
                if (page.ForegroundContainer.Children.Contains(element))
                {
                    page.ForegroundContainer.Children.Remove(element);
                }
            }

            if (lineInfo.Container != null && OwnerControl.DefferedScrolling)
            {
                lineInfo.Elements.Remove(lineInfo.Container);
                lineInfo.Container = null;
            }
        }

        internal virtual void RemoveViewer()
        {
            OwnerControl = null;
            Pages.Clear();
            doubleClickTimer.Tick -= new EventHandler(doubleClickTimer_Tick);
            doubleClickTimer = null;
            dragTimer.Tick -= new EventHandler(dragTimer_Tick);
            dragTimer = null;
            MouseLeftButtonDown -= new MouseButtonEventHandler(LayoutViewer_MouseLeftButtonDown);
            MouseLeftButtonUp -= new MouseButtonEventHandler(LayoutViewer_MouseLeftButtonUp);
            MouseMove -= new MouseEventHandler(LayoutViewer_MouseMove);
            MouseRightButtonDown -= new MouseButtonEventHandler(LayoutViewer_MouseRightButtonDown);
            scrollingTimer.Tick -= new EventHandler(scrollingTimer_Tick);
#if WPF && (SyncfusionFramework4_0 || SyncfusionFramework4_5 || SyncfusionFramework4_5_1)
            TouchDown -= new EventHandler<TouchEventArgs>(LayoutViewer_TouchDown);
            TouchMove -= new EventHandler<TouchEventArgs>(LayoutViewer_TouchMove);
#endif
            scrollingTimer = null;
            ImageResizer = null;
        }

        internal void SelectImage(ImageContainerAdv image)
        {
            if (image != null && image.Paragraph != null)
            {
                OwnerControl.Selection.Start = new TextPosition(Document) { Paragraph = image.Paragraph };
                OwnerControl.Selection.Start.SetIndex(image.Paragraph.GetIndexFromInlineAndSpanIndex(image, 0).ToString());
                OwnerControl.Selection.End = new TextPosition(Document) { Paragraph = image.Paragraph };
                OwnerControl.Selection.End.SetIndex(image.Paragraph.GetIndexFromInlineAndSpanIndex(image, 1).ToString());
                OwnerControl.Selection.SetVirtualPositionsForStartAndEnd(OwnerControl.Selection.Start,OwnerControl.Selection.End);
                OwnerControl.Selection.ExtractSelectedText();
                MoveToPosition(OwnerControl.Selection.End.Copy());
                PageAdv page = GetPageFromLine(image.ElementBox.LineInfo);
                if (page != null)
                {
                    page.SelectedLines.Clear();
                    page.SelectedLines.Add(new LineInfo()
                    {
                        BoundingRectangle = new Rect(image.ElementBox.BoundingRectangle.Left,
                            image.ElementBox.BoundingRectangle.Top, image.ElementBox.Width, image.ElementBox.LineInfo.Height)
                    });
                }
            }
        }

        /// <summary>
        /// Adds the line to page
        /// </summary>
        /// <param name="lineInfo"></param>
        /// <param name="page"></param>
        internal void AddLineToPage(LineInfo lineInfo, PageAdv page)
        {
            /******Uncomment the below line to render the text in image******/

            //lineInfo.CreateDrawingContext();

            /******Uncomment the below line to render the text in TextBlock******/

            RenderingManager.Render(lineInfo, page);

            //foreach (UIElement element in lineInfo.Elements)
            //{
            //    if (!page.ForegroundContainer.Children.Contains(element))
            //    {
            //        page.ForegroundContainer.Children.Add(element);
            //    }
            //}
        }

    }
}
