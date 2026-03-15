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
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using Syncfusion.Pdf.Parsing;
using System.IO;

namespace Syncfusion.Windows.PdfViewer
{
   
    /// <summary>
    /// Interaction logic for PdfView.xaml
    /// </summary>
    public partial class PdfDocumentView : UserControl
    {
        private static FixedDocument m_printDocument;
        public PdfDocumentView()
        {
            
            InitializeComponent();
            InitializeViewer();
            //this.IsManipulationEnabled = true;
            //this.ManipulationDelta += new EventHandler<ManipulationDeltaEventArgs>(PdfViewerControl_ManipulationDelta);
            //this.ManipulationCompleted += PdfViewerControl_ManipulationCompleted;
        }
        private double m_touchZoom;
        bool isTouchZoom;
        private double m_touchFitPageTransition;
        private double m_touchTransition;
        ZoomMode m_zoomMode;

        #region Events
        public delegate void NavigationButtonStatesChangedEventHandler(object sender, EventArgs args);
        public event NavigationButtonStatesChangedEventHandler NavigationButtonStatesChanged;

        public delegate void CurrentPageChangedEventHandler(object sender, EventArgs args);
        public event CurrentPageChangedEventHandler CurrentPageChanged;

        public delegate void ZoomChangedEventHandler(object sender, ZoomEventArgs args);
        public event ZoomChangedEventHandler ZoomChanged;

        public delegate void DocumentLoadedEventHandler(object sender, EventArgs args);
        public event DocumentLoadedEventHandler DocumentLoaded;
        #endregion
        
        public ZoomMode ZoomMode
        {
            get
            {
                return m_zoomMode;
            }
            set
            {
                m_zoomMode = value;
                if (innerPanel.ScrollViewer != null)
                    innerPanel.SetZoom();
                if (ZoomChanged != null)
                {
                    ZoomEventArgs args = new ZoomEventArgs((int)(innerPanel.ZoomFactor * 100));
                    ZoomChanged(this, args);
                }
                //if (m_pagePanel != null && m_pagePanel.Pages != null)
                  //  SetZoom();
            }
        }
        public void GoToPreviousPage()
        {
            innerPanel.GotoPreviousPage();
        }
        public void GoToNextPage()
        {
            innerPanel.GotoNextPage();
        }

        public bool CanGoToFirstPage
        {
            get
            {
                return innerPanel.CanGoToFirstPage;
            }
        }

        public bool CanGoToPreviousPage
        {
            get
            {
                return innerPanel.CanGoToPreviousPage;
            }
        }

        public bool CanGoToNextPage
        {
            get
            {
                return innerPanel.CanGoToNextPage;
            }
        }

        public bool CanGoToLastPage
        {
            get
            {
                return innerPanel.CanGoToLastPage;
            }
        }

        /// <summary>
        /// Gets or sets whether to display page number when scrolling.
        /// </summary>
        public bool ShowPageNumber
        {
            get
            {
                return innerPanel.ShowPageNumber;
            }
            set
            {
                innerPanel.ShowPageNumber = value;
            }
        }

        public void GoToPageAtIndex(int pageIndex)
        {
            innerPanel.GoToPageAtIndex(pageIndex);
        }

        /// <summary>
        /// Moves the Vscroll bar to specified page and offset location
        /// </summary>
        /// <param name="pageIndex">The destination page</param>
        /// <param name="destOffset">The destination offset in the page</param>        
        public void GoToPageAtIndexAndOffset(int pageIndex, float destOffset)
        {
            (innerPanel as DocumentView).GoToPageAtIndexAndOffset(pageIndex, destOffset);
        }

        /// <summary>
        /// Gets the Print document
        /// </summary>
        public FixedDocument PrintDocument
        {
            get
            {
                if (innerPanel != null)
                    return (m_printDocument = innerPanel.PrintDocument);
                else
                    return null;
            }
        }
        /// <summary>
        /// Silent Prints the document to the Default Printer
        /// </summary>
        public void Print()
        {
            innerPanel.Print();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="eventArgs"></param>
        private void OnLoaded(EventArgs eventArgs)
        {
            if (DocumentLoaded != null)
                DocumentLoaded(this, eventArgs);
        }

        //void PdfViewerControl_ManipulationCompleted(object sender, ManipulationCompletedEventArgs e)
        //{
        //    if (ZoomMode == ZoomMode.FitPage)
        //    {
        //        //if (m_touchFitPageTransition < 0)
        //        //{
        //        //    m_documentView.GoToNextPage();
        //        //}
        //        //else if (m_touchFitPageTransition > 0)
        //        //{
        //        //    m_documentView.GoToPreviousPage();
        //        //}
        //        m_touchFitPageTransition = 0;
        //    }
        //    else
        //    {
        //        if (m_touchTransition > 0)
        //        {
        //            innerPanel.UpdateOffset(-m_touchTransition);
        //        }
        //        else if (m_touchTransition < 0)
        //        {
        //            innerPanel.UpdateOffset(-m_touchTransition);
        //        }
        //        m_touchTransition = 0;
        //    }

        //    if (m_touchZoom != 0 && isTouchZoom)
        //    {
        //        int percentage = (int)((innerPanel.ZoomFactor * 100) + (m_touchZoom * 2));
        //        float m_zoomFactor = (float)percentage / 100;
        //        innerPanel.ZoomTo(m_zoomFactor);
        //    }
        //}
        public int PageCount
        {
            get
            {
                return innerPanel.PageCount;
            }
        }
        internal IPdfDocumentView PdfDocumentViewer
        {
            get
            {
                return innerPanel;
            }
        }
        //public delegate void CurrentPageChangedEventHandler(object sender, EventArgs args);
        //public event CurrentPageChangedEventHandler CurrentPageChanged;
        public int CurrentPageIndex
        {
            get
            {
                return innerPanel.CurrentPageIndex + 1;
            }
            set
            {
                innerPanel.CurrentPageIndex = value - 1;
            }
        }
        internal double ZoomFactor
        {
            get
            {
                return  innerPanel.ZoomFactor;
            }
            //set
            //{
            //    ZoomFactor = innerPanel.ZoomFactor;
            //}
        }
        //void PdfViewerControl_ManipulationDelta(object sender, ManipulationDeltaEventArgs e)
        //{
        //    isTouchZoom = false;
        //    m_touchZoom = 0;
        //    var element = e.Source as FrameworkElement;
        //    if (element != null)
        //    {
        //        var deltaManipulation = e.DeltaManipulation;
        //        if (deltaManipulation.Expansion.X > 0 && deltaManipulation.Expansion.Y > 0)
        //        {
        //            m_touchZoom += (int)deltaManipulation.Expansion.X;
        //            isTouchZoom = true;
        //        }
        //        else if (deltaManipulation.Expansion.X < 0 && deltaManipulation.Expansion.Y < 0)
        //        {
        //            m_touchZoom += (int)deltaManipulation.Expansion.X;
        //            isTouchZoom = true;
        //        }

        //        if (e.DeltaManipulation.Translation.Y != 0)
        //        {
        //            if (ZoomMode == ZoomMode.FitPage)
        //            {
        //                if (e.DeltaManipulation.Translation.Y > 0)
        //                {
        //                    m_touchFitPageTransition += e.DeltaManipulation.Translation.Y;
        //                }
        //                else
        //                {
        //                    m_touchFitPageTransition += e.DeltaManipulation.Translation.Y;
        //                }
        //            }
        //            else
        //            {
        //                if (e.DeltaManipulation.Translation.Y > 0)
        //                {
        //                    m_touchTransition += e.DeltaManipulation.Translation.Y;
        //                }
        //                else
        //                {
        //                    m_touchTransition += e.DeltaManipulation.Translation.Y;
        //                }
        //            }
        //        }
        //    }
        //}
        public static readonly DependencyProperty ZoomStepProperty = DependencyProperty.Register("ZoomStep", typeof(double),
                                                                              typeof(PdfDocumentView), new FrameworkPropertyMetadata(0.25));

        public static readonly DependencyProperty MinZoomFactorProperty = DependencyProperty.Register("MinZoomFactor", typeof(double),
                                                                            typeof(PdfDocumentView), new FrameworkPropertyMetadata(0.15));

        public static readonly DependencyProperty MaxZoomFactorProperty = DependencyProperty.Register("MaxZoomFactor", typeof(double),
                                                                            typeof(PdfDocumentView), new FrameworkPropertyMetadata(6.0));
        public static DependencyProperty ZoomModeProperty = DependencyProperty.Register("ZoomMode", typeof(ZoomMode), typeof(PdfDocumentView), new FrameworkPropertyMetadata(ZoomMode.Default));
        internal double ZoomStep
        {
            get { return (double)GetValue(ZoomStepProperty); }
            set { SetValue(ZoomStepProperty, value); }
        }

        public void ZoomTo(int percentage)
        {
            this.innerPanel.ZoomTo(percentage);
            if (ZoomChanged != null)
            {
                ZoomEventArgs args = new ZoomEventArgs((int)(innerPanel.ZoomFactor * 100));
                ZoomChanged(this, args);
            }
        }
        public void GoToFirstPage()
        {
            //if (!CanGoToFirstPage)
            //    throw new Exception("\r\n\r\nCan't go to first page.");
            //innerPanel.go
           innerPanel.GoToPageAtIndex(0);
        }
        public void GoToLastPage()
        {
            //if (!CanGoToLastPage)
            //    throw new Exception("\r\n\r\nCan't go to last page.");

            innerPanel.GoToPageAtIndex(this.PageCount);
        }
        internal void UpdateOffset(double offset)
        {
            innerPanel.UpdateOffset(offset);
        }
        internal PdfLoadedDocument LoadedDocument
        {
            get
            {
                return innerPanel.LoadedDocument;
            }
            set
            {
                innerPanel.LoadedDocument = value;
            }
        }

        private void InitializeViewer()
        {
            this.pnlMain.Children.Clear();
            this.innerPanel = new DocumentView(this);
            this.innerPanel.CurrentPageChanged += new PdfViewer.CurrentPageChangedEventHandler(innerPanel_CurrentPageChanged);
            this.innerPanel.NavigationButtonStatesChanged += new PdfViewer.NavigationButtonStatesChangedEventHandler(innerPanel_NavigationButtonStatesChanged);
            this.pnlMain.Children.Add(this.innerPanel.Instance);
        }

        void innerPanel_NavigationButtonStatesChanged(object sender, EventArgs args)
        {
            if (NavigationButtonStatesChanged != null)
                NavigationButtonStatesChanged(this, null);
        }

        void innerPanel_CurrentPageChanged(object sender, EventArgs args)
        {
            if (CurrentPageChanged != null)
                CurrentPageChanged(this, null);
        }

        private IPdfDocumentView innerPanel;
        internal string FileName;
        internal ScrollViewer ScrollViewer
        {
            get { return this.innerPanel.ScrollViewer; }
        }
        public BitmapSource ExportAsImage(int pageIndex)
        {
            return innerPanel.ExportAsImage(pageIndex);
        }
        public BitmapSource[] ExportAsImage(int startIndex, int endIndex)
        {
            return innerPanel.ExportAsImage(startIndex, endIndex);
        }
        public void Load(string filePath)
        {
            FileName = filePath;
            innerPanel.Load(filePath);
        }
        public void Load(Stream stream)
        {
            innerPanel.Load(stream);
        }
        public void Load(String filePath, string password)
        {
            FileName = filePath;
            innerPanel.Load(filePath);
        }
        public void Load(PdfLoadedDocument loadedDocument)
        {
            innerPanel.Load(loadedDocument);
        }
        //public override void OnApplyTemplate()
        //{
        //    base.OnApplyTemplate();
        //    ScrollViewer scrollViewer = VisualTreeHelperEx.FindChild<ScrollViewer>(this);
        //}
        public void Unload()
        {
            innerPanel.Unload();
        }

        /// <summary>
        /// Returns the page number and rectangle postions of the text matchs found in the page
        /// </summary>
        /// <param name="text">The text to be searched</param>
        /// <param name="matchRect">Holds the page number and rectangle positions of the text matches</param> 
        internal bool FindText(string text, out Dictionary<int, List<System.Drawing.RectangleF>> matchRect)
        {
            return (innerPanel as DocumentView).FindTextMatches(text, out matchRect);
        }
    }
}
