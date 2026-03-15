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
using Syncfusion.PdfViewer.Base;
using System.IO;
using System.Windows.Xps.Packaging;
using System.Windows.Xps;
using System.ComponentModel;

namespace Syncfusion.Windows.PdfViewer
{
    public class PdfViewerControl : Control
    {
        #region Members
        internal TextSearchBar m_textSearchBar;
        internal DocumentToolbar m_documentToolbar;
        PdfDocumentView m_documentView;
        internal NotificationBar m_notificationBar;
        internal string m_notificationMessage = "Essential PDF could not open the PDF document";
        private PdfViewerExceptions m_exceptions = new PdfViewerExceptions();
        private bool m_enableNotificationBar = true;

        internal bool IslinkClicked = false;
        internal bool IslinkMouseOver = false;
        private static bool m_showToolbar=true;
        private static int m_pageCount;
        private static int m_currentPageIndex;
        private static FixedDocument m_printDocument;
        private double m_touchFitPageTransition;
        private double m_touchTransition;
        private double m_touchZoom;
        internal bool m_isNotifying = false;
        string documentPath = string.Empty;
        string password = string.Empty;
        Stream documentStream;
        PdfLoadedDocument loadedDoc;
        internal int GoToPageIndexBinded = 0;
        public static readonly DependencyProperty ShowToolbarProperty;
        public static readonly DependencyProperty PageCountProperty;
        public static readonly DependencyProperty PrintDocumentProperty;
        public static readonly DependencyProperty CurrentPageIndexProperty;
        public static readonly DependencyProperty ItemSourceProperty;
        public static readonly DependencyProperty ZoomModeProperty;
        public static readonly DependencyProperty CurrentPageProperty;
        #endregion
        #region Events
        public delegate void DocumentLoadedEventHandler(object sender, EventArgs args);
        public delegate void HyperLinkClickedEventHandler(object sender, AnnotEventArgs args);
        public delegate void HyperLinkMouseOverEventHandler(object sender, EventArgs args);
        /// <summary>
        /// Occurs when the Pdf document is loaded
        /// </summary>
        public event DocumentLoadedEventHandler DocumentLoaded;
        public event HyperLinkClickedEventHandler HyplerLinkClicked;
        public event HyperLinkMouseOverEventHandler HyperLinkMouseOver;
        #endregion Events

        #region Properties
        /// <summary>
        /// Gets the page count
        /// </summary>
        public int PageCount
        {
            get
            {
                if (m_documentView == null)
                    return 0;
                else
                    return (m_pageCount = m_documentView.PageCount);
            }
        }
        /// <summary>
        /// Gets the Print document
        /// </summary>
        public FixedDocument PrintDocument
        {
            get
            {
                if (m_documentView != null)
                    return (m_printDocument = m_documentView.PdfDocumentViewer.PrintDocument);
                else
                    return null;
            }
        }

        /// <summary>
        /// Silent Prints the document to the Default Printer
        /// </summary>
        public void Print()
        {
            if (m_documentView != null)
                m_documentView.PdfDocumentViewer.Print();
        }

        public bool ShowToolbar
        {
            get
            {
                return (bool)GetValue(ShowToolbarProperty);
            }
            
            set
            {
                SetValue(PdfViewerControl.ShowToolbarProperty, value);
                m_showToolbar = value;
                OnApplyTemplate();
            }
        }
        public int CurrentPageIndex
        {
            get
            {
                return (m_currentPageIndex = m_documentView.PdfDocumentViewer.CurrentPageIndex + 1);
            }
        }
        /// <summary>
        /// Enables the display of Notification bar on setting true.
        /// </summary>
        public bool EnableNotificationBar
        {
            get
            {
                return m_enableNotificationBar;
            }
            set
            {
                m_enableNotificationBar = value;
            }
        }
        /// <summary>
        /// Enables the PdfViewerControl to load a document from XAML as string or stream.
        /// </summary>
        public object ItemSource
        {
            get
            {
                return (object)this.GetValue(ItemSourceProperty);
            }
            set
            {
                this.SetValue(ItemSourceProperty, value);
            }
        }

        public ZoomMode ZoomMode
        {
            get
            {
                return (ZoomMode)this.GetValue(ZoomModeProperty);
            }
            set
            {
                this.SetValue(ZoomModeProperty, value);
            }
        }

        public int CurrentPage
        {
            get
            {
                return (m_currentPageIndex = m_documentView.CurrentPageIndex);
            }
            set
            {
                this.SetValue(CurrentPageProperty, value);
            }
        }

        #endregion Properties
        
		 # region Commands

        public ICommand FirstPageCommand
        {
            get { return (ICommand)GetValue(FirstPageCommandProperty); }
            set { SetValue(FirstPageCommandProperty, value); }
        }

        // Using a DependencyProperty as the backing store for FirstPageCommand.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty FirstPageCommandProperty =
            DependencyProperty.Register("FirstPageCommand", typeof(ICommand), typeof(PdfViewerControl), new PropertyMetadata(null));

        public object FirstPageCommandParameter
        {
            get { return (object)GetValue(FirstPageCommandParameterProperty); }
            set { SetValue(FirstPageCommandParameterProperty, value); }
        }

        // Using a DependencyProperty as the backing store for FirstPageCommandParameter.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty FirstPageCommandParameterProperty =
            DependencyProperty.Register("FirstPageCommandParameter", typeof(object), typeof(PdfViewerControl), new PropertyMetadata(null));

        public ICommand LastPageCommand
        {
            get { return (ICommand)GetValue(LastPageCommandProperty); }
            set { SetValue(LastPageCommandProperty, value); }
        }

        // Using a DependencyProperty as the backing store for LastPageCommand.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty LastPageCommandProperty =
            DependencyProperty.Register("LastPageCommand", typeof(ICommand), typeof(PdfViewerControl), new PropertyMetadata(null));

        public object LastPageCommandParameter
        {
            get { return (object)GetValue(LastPageCommandParameterProperty); }
            set { SetValue(LastPageCommandParameterProperty, value); }
        }

        // Using a DependencyProperty as the backing store for LastPageCommandParameter.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty LastPageCommandParameterProperty =
            DependencyProperty.Register("LastPageCommandParameter", typeof(object), typeof(PdfViewerControl), new PropertyMetadata(null));

        public ICommand NextPageCommand
        {
            get { return (ICommand)GetValue(NextPageCommandProperty); }
            set { SetValue(NextPageCommandProperty, value); }
        }

        // Using a DependencyProperty as the backing store for NextPageCommand.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty NextPageCommandProperty =
            DependencyProperty.Register("NextPageCommand", typeof(ICommand), typeof(PdfViewerControl), new PropertyMetadata(null));

        public object NextPageCommandParameter
        {
            get { return (object)GetValue(NextPageCommandParameterProperty); }
            set { SetValue(NextPageCommandParameterProperty, value); }
        }

        // Using a DependencyProperty as the backing store for NextPageCommandParameter.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty NextPageCommandParameterProperty =
            DependencyProperty.Register("NextPageCommandParameter", typeof(object), typeof(PdfViewerControl), new PropertyMetadata(null));

        public ICommand PreviousPageCommand
        {
            get { return (ICommand)GetValue(PreviousPageCommandProperty); }
            set { SetValue(PreviousPageCommandProperty, value); }
        }

        // Using a DependencyProperty as the backing store for PreviousPageCommand.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty PreviousPageCommandProperty =
            DependencyProperty.Register("PreviousPageCommand", typeof(ICommand), typeof(PdfViewerControl), new PropertyMetadata(null));

        public object PreviousPageCommandParameter
        {
            get { return (object)GetValue(PreviousPageCommandParameterProperty); }
            set { SetValue(PreviousPageCommandParameterProperty, value); }
        }

        // Using a DependencyProperty as the backing store for PreviousPageCommandParameter.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty PreviousPageCommandParameterProperty =
            DependencyProperty.Register("PreviousPageCommandParameter", typeof(object), typeof(PdfViewerControl), new PropertyMetadata(null));

        public ICommand IncreaseZoomCommand
        {
            get { return (ICommand)GetValue(IncreaseZoomCommandProperty); }
            set { SetValue(IncreaseZoomCommandProperty, value); }
        }

        // Using a DependencyProperty as the backing store for IncreaseZoomCommand.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty IncreaseZoomCommandProperty =
            DependencyProperty.Register("IncreaseZoomCommand", typeof(ICommand), typeof(PdfViewerControl), new PropertyMetadata(null));

        public object IncreaseZoomCommandParameter
        {
            get { return (object)GetValue(IncreaseZoomCommandParameterProperty); }
            set { SetValue(IncreaseZoomCommandParameterProperty, value); }
        }

        // Using a DependencyProperty as the backing store for IncreaseZoomCommandParameter.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty IncreaseZoomCommandParameterProperty =
            DependencyProperty.Register("IncreaseZoomCommandParameter", typeof(object), typeof(PdfViewerControl), new PropertyMetadata(null));

        public ICommand DecreaseZoomCommand
        {
            get { return (ICommand)GetValue(DecreaseZoomCommandProperty); }
            set { SetValue(DecreaseZoomCommandProperty, value); }
        }

        // Using a DependencyProperty as the backing store for DecreaseZoomCommand.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty DecreaseZoomCommandProperty =
            DependencyProperty.Register("DecreaseZoomCommand", typeof(ICommand), typeof(PdfViewerControl), new PropertyMetadata(null));

        public object DecreaseZoomCommandParameter
        {
            get { return (object)GetValue(DecreaseZoomCommandParameterProperty); }
            set { SetValue(DecreaseZoomCommandParameterProperty, value); }
        }

        // Using a DependencyProperty as the backing store for DecreaseZoomCommandParameter.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty DecreaseZoomCommandParameterProperty =
            DependencyProperty.Register("DecreaseZoomCommandParameter", typeof(object), typeof(PdfViewerControl), new PropertyMetadata(null));

        //GoToPage
        public ICommand GoToPageCommand
        {
            get { return (ICommand)GetValue(GoToPageCommandProperty); }
            set { SetValue(GoToPageCommandProperty, value); }
        }
        // Using a DependencyProperty as the backing store for DecreaseZoomCommand.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty GoToPageCommandProperty =
            DependencyProperty.Register("GoToPageCommand", typeof(ICommand), typeof(PdfViewerControl), new PropertyMetadata(null));

        public object GoToPageCommandParameter
        {
            get { return (object)GetValue(GoToPageCommandParameterProperty); }
            set { SetValue(GoToPageCommandParameterProperty, value); }
        }

        // Using a DependencyProperty as the backing store for DecreaseZoomCommandParameter.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty GoToPageCommandParameterProperty =
            DependencyProperty.Register("GoToPageCommandParameter", typeof(object), typeof(PdfViewerControl), new PropertyMetadata(null));

        public ICommand ViewModeCommand
        {
            get { return (ICommand)GetValue(ViewModeCommandProperty); }
            set { SetValue(ViewModeCommandProperty, value); }
        }

        // Using a DependencyProperty as the backing store for ViewModeCommand.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty ViewModeCommandProperty =
            DependencyProperty.Register("ViewModeCommand", typeof(ICommand), typeof(PdfViewerControl), new PropertyMetadata(null));

        public object ViewModeCommandParameter
        {
            get { return (object)GetValue(ViewModeCommandParameterProperty); }
            set { SetValue(ViewModeCommandParameterProperty, value); }
        }

        // Using a DependencyProperty as the backing store for ViewModeCommandParameter.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty ViewModeCommandParameterProperty =
            DependencyProperty.Register("ViewModeCommandParameter", typeof(object), typeof(PdfViewerControl), new PropertyMetadata(null));

        #region SwitchModeCommand
        public ICommand SwitchToThumbnailModeCommand
        {
            get { return (ICommand)GetValue(SwitchToThumbNailModeCommandProperty); }
            set { SetValue(SwitchToThumbNailModeCommandProperty, value); }
        }

        // Using a DependencyProperty as the backing store for ViewModeCommand.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty SwitchToThumbNailModeCommandProperty =
            DependencyProperty.Register("SwitchModeCommand", typeof(ICommand), typeof(PdfViewerControl), new PropertyMetadata(null));

        public object SwitchToThumbnailModeCommandParameter
        {
            get { return (object)GetValue(SwitchToThumnailModeCommandParameterProperty); }
            set { SetValue(SwitchToThumnailModeCommandParameterProperty, value); }
        }

        // Using a DependencyProperty as the backing store for ViewModeCommandParameter.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty SwitchToThumnailModeCommandParameterProperty =
            DependencyProperty.Register("SwitchModeCommandParameter", typeof(object), typeof(PdfViewerControl), new PropertyMetadata(null));
        #endregion

        #region TextSearchCommands
        public ICommand SearchNextCommand
        {
            get { return (ICommand)GetValue(SearchNextCommandProperty); }
            set { SetValue(SearchNextCommandProperty, value); }
        }

        // Using a DependencyProperty as the backing store for ViewModeCommand.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty SearchNextCommandProperty =
            DependencyProperty.Register("SearchNextCommand", typeof(ICommand), typeof(PdfViewerControl), new PropertyMetadata(null));

        public object SearchNextCommandParameter
        {
            get { return (object)GetValue(SearchNextCommandParameterProperty); }
            set { SetValue(SearchNextCommandParameterProperty, value); }
        }

        // Using a DependencyProperty as the backing store for ViewModeCommandParameter.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty SearchNextCommandParameterProperty =
            DependencyProperty.Register("SearchNextCommandParameter", typeof(object), typeof(PdfViewerControl), new PropertyMetadata(null));

        //Saerch Previous

        public ICommand SearchPreviousCommand
        {
            get { return (ICommand)GetValue(SearchPreviousCommandProperty); }
            set { SetValue(SearchPreviousCommandProperty, value); }
        }

        // Using a DependencyProperty as the backing store for ViewModeCommand.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty SearchPreviousCommandProperty =
            DependencyProperty.Register("SearchPreviousCommand", typeof(ICommand), typeof(PdfViewerControl), new PropertyMetadata(null));

        public object SearchPreviousCommandParameter
        {
            get { return (object)GetValue(SearchPreviousCommandParameterProperty); }
            set { SetValue(SearchPreviousCommandParameterProperty, value); }
        }

        // Using a DependencyProperty as the backing store for ViewModeCommandParameter.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty SearchPreviousCommandParameterProperty =
            DependencyProperty.Register("SearchPreviousCommandParameter", typeof(object), typeof(PdfViewerControl), new PropertyMetadata(null));
        #endregion

        //Printing
        public ICommand PrintCommand
        {
            get { return (ICommand)GetValue(PrintCommandProperty); }
            set { SetValue(PrintCommandProperty, value); }
        }

        // Using a DependencyProperty as the backing store for IncreaseZoomCommand.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty PrintCommandProperty =
            DependencyProperty.Register("PrintCommand", typeof(ICommand), typeof(PdfViewerControl), new PropertyMetadata(null));

        public object PrintCommandParameter
        {
            get { return (object)GetValue(PrintCommandParameterProperty); }
            set { SetValue(PrintCommandParameterProperty, value); }
        }

        // Using a DependencyProperty as the backing store for IncreaseZoomCommandParameter.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty PrintCommandParameterProperty =
            DependencyProperty.Register("PrintCommandParameter", typeof(object), typeof(PdfViewerControl), new PropertyMetadata(null));

        #endregion
		
		static PdfViewerControl()
        {
            FrameworkPropertyMetadata md = new FrameworkPropertyMetadata(ShowToolbarPropertyChanged);
            md.DefaultValue = true;
            PdfViewerControl.ShowToolbarProperty = DependencyProperty.Register("ShowToolbar", typeof(bool), typeof(PdfViewerControl), md);
            DefaultStyleKeyProperty.OverrideMetadata(typeof(PdfViewerControl), new FrameworkPropertyMetadata(typeof(PdfViewerControl)));
            PageCountProperty = DependencyProperty.Register("PageCount", typeof(int), typeof(PdfViewerControl), new UIPropertyMetadata(0));
            CurrentPageIndexProperty = DependencyProperty.Register("CurrentPageIndex", typeof(int), typeof(PdfViewerControl), new UIPropertyMetadata(0));
            PrintDocumentProperty = DependencyProperty.Register("PrintDocument", typeof(System.Drawing.Printing.PrintDocument), typeof(PdfViewerControl), new UIPropertyMetadata(null));
            ItemSourceProperty = DependencyProperty.Register("ItemSource", typeof(object), typeof(PdfViewerControl), new FrameworkPropertyMetadata(ItemSourcePropertyChanged));
            ZoomModeProperty = DependencyProperty.Register("ZoomMode", typeof(ZoomMode), typeof(PdfViewerControl), new FrameworkPropertyMetadata(ZoomModePropertyChanged));
            CurrentPageProperty = DependencyProperty.Register("CurrentPage", typeof(int), typeof(PdfViewerControl), new FrameworkPropertyMetadata(CurrentPagePropertyChanged));
        }
        public static void ZoomModePropertyChanged(DependencyObject obj, DependencyPropertyChangedEventArgs args)
        {
            PdfViewerControl viewercontrol = obj as PdfViewerControl;
            if (args.NewValue is ZoomMode)
            {
                if (viewercontrol.m_documentView != null)
                    viewercontrol.m_documentView.ZoomMode = (ZoomMode)args.NewValue;
                else
                    viewercontrol.ZoomMode = (ZoomMode)args.NewValue;
            }
        }


        public static void CurrentPagePropertyChanged(DependencyObject obj, DependencyPropertyChangedEventArgs args)
        {
            PdfViewerControl viewercontrol = obj as PdfViewerControl;
            if (args.NewValue is int)
            {
                viewercontrol.GoToPageAtIndex((int)args.NewValue);
            }
        }

        public static void ItemSourcePropertyChanged(DependencyObject obj, DependencyPropertyChangedEventArgs args)
        {
            PdfViewerControl viewercontrol = obj as PdfViewerControl;
            if (args.NewValue is string)
            {
                viewercontrol.documentPath = args.NewValue.ToString();
                viewercontrol.Load(viewercontrol.documentPath);
            }
            else if (args.NewValue is Stream)
            {
                viewercontrol.Load(args.NewValue as Stream);
            }
        }

#if SyncfusionFramework4_5||SyncfusionFramework4_0
        public PdfViewerControl()
        {
            this.IsManipulationEnabled = true;
            this.ManipulationDelta += new EventHandler<ManipulationDeltaEventArgs>(PdfViewerControl_ManipulationDelta);
            this.ManipulationCompleted += PdfViewerControl_ManipulationCompleted;
        }
        bool isTouchZoom;
        void PdfViewerControl_ManipulationCompleted(object sender, ManipulationCompletedEventArgs e)
        {
            if (m_documentView.ZoomMode == ZoomMode.FitPage)
            {
                if (m_touchFitPageTransition < 0)
                {
                    m_documentView.GoToNextPage();
                }
                else if (m_touchFitPageTransition > 0)
                {
                    m_documentView.GoToPreviousPage();
                }
                m_touchFitPageTransition = 0;
            }
            else
            {
                if (m_touchTransition > 0)
                {
                    m_documentView.UpdateOffset(-m_touchTransition);
                }
                else if (m_touchTransition < 0)
                {
                    m_documentView.UpdateOffset(-m_touchTransition);
                }
                m_touchTransition = 0;
            }

            if (m_touchZoom != 0 && isTouchZoom)
            {
                int percentage = (int)((m_documentView.ZoomFactor * 100) + (m_touchZoom * 2));
                ZoomTo(percentage);
            }
        }

        void PdfViewerControl_ManipulationDelta(object sender, ManipulationDeltaEventArgs e)
        {
            isTouchZoom = false;
            m_touchZoom = 0;
            var element = e.Source as FrameworkElement;
            if (element != null)
            {
                var deltaManipulation = e.DeltaManipulation;
                int currentZoom = 0;
                int.TryParse(m_documentToolbar.cmbCurrentZoomLevel.Text, out currentZoom);
                if (deltaManipulation.Expansion.X > 0 && deltaManipulation.Expansion.Y > 0)
                {
                    m_touchZoom += (int)deltaManipulation.Expansion.X;                    
                    isTouchZoom = true;
                }
                else if(deltaManipulation.Expansion.X < 0 && deltaManipulation.Expansion.Y < 0)
                {
                    m_touchZoom += (int)deltaManipulation.Expansion.X;                    
                    isTouchZoom = true;
                }
                
                if (e.DeltaManipulation.Translation.Y != 0)
                {
                    if (m_documentView.ZoomMode == ZoomMode.FitPage)
                    {
                        if (e.DeltaManipulation.Translation.Y > 0)
                        {
                            m_touchFitPageTransition += e.DeltaManipulation.Translation.Y;                                                    
                        }
                        else
                        {
                            m_touchFitPageTransition += e.DeltaManipulation.Translation.Y;                            
                        }
                    }
                    else
                    {
                        if (e.DeltaManipulation.Translation.Y > 0)
                        {
                            m_touchTransition += e.DeltaManipulation.Translation.Y;
                        }
                        else
                        {
                            m_touchTransition += e.DeltaManipulation.Translation.Y;
                        }
                    }
                }
            }
        }

#endif
        void notificationVisibilityChange(bool value)
        {
            m_isNotifying = true;
            OnApplyTemplate();
        }
        public static void ShowToolbarPropertyChanged(DependencyObject o, DependencyPropertyChangedEventArgs e)
        {
            if (e.NewValue.ToString() == "False")
            {
                m_showToolbar = false;
            }
            else
            {
                m_showToolbar = true;
            }
        }
        /// <summary>
        /// Loads a Pdf document in the Pdf viewer
        /// </summary>
        /// <param name="filePath">The path for the Pdf document to display in the pdf viewer</param>
        public void Load(string filePath)
        {
            try
            {
                if (!File.Exists(filePath))
                    throw new FileNotFoundException("Cannot able to locate the specified file");
                if (m_documentView != null)
                {
                    if (m_documentView.LoadedDocument == null)
                    {
                        if (this.HyplerLinkClicked != null)
                        {
                            this.IslinkClicked = true;
                        }
                        else
                        {
                            this.IslinkClicked = false;
                        }
                        if (this.HyperLinkMouseOver != null)
                        {
                            this.IslinkMouseOver = true;
                        }
                        else
                        {
                            this.IslinkMouseOver = false;
                        }
                        m_documentView.Load(filePath);
                        m_documentToolbar.ActiveView = m_documentView;
                        m_documentToolbar.Initialize(m_documentView);
                    }
                    else
                    {
                        m_documentView.Unload();
                        m_documentView.Load(filePath);
                        m_documentToolbar.Initialize(m_documentView);
                    }
                }
                else
                {
                    if (this.HyplerLinkClicked != null)
                    {
                        this.IslinkClicked = true;
                    }
                    else
                    {
                        this.IslinkClicked = false;
                    }
                    if (this.HyperLinkMouseOver != null)
                    {
                        this.IslinkMouseOver = true;
                    }
                    else
                    {
                        this.IslinkMouseOver = false;
                    }
                    documentPath = filePath;
                }
            }
            catch (Exception ex)
            {
                m_notificationMessage = "Essential PDF Viewer could not open the PDF document";
                m_exceptions.Exceptions.Append("Error in loading the PDF document \r\nThe complete stack is as given below\r\n" + ex.StackTrace.ToString() + "\r\n\r\n");
                notificationVisibilityChange(true);
                return;
            }
        }

        public void Load(Stream stream)
        {
            try
            {
                if (stream == null || stream.CanRead == false || stream.Length == 0)
                    throw new Exception("\r\n\r\nStream cannot be read");
                stream.Position = 0;
                if (m_documentView != null)
                {
                    if (m_documentView.LoadedDocument == null)
                    {
                        m_documentView.Load(stream);
                        m_documentToolbar.Initialize(m_documentView);
                    }
                    else
                    {
                        m_documentView.Unload();
                        m_documentView.Load(stream);
                        m_documentToolbar.Initialize(m_documentView);
                    }
                }
                else
                {
                    documentStream = stream;
                }
            }
            catch (Exception ex)
            {
                m_notificationMessage = "Essential PDF Viewer could not open the PDF document";
                m_exceptions.Exceptions.Append("Error while loading the stream\nThe complete stack is as given below" + ex.StackTrace.ToString() + "\r\n\r\n");
                notificationVisibilityChange(true);
                return;
            }
        }
        /// <summary>
        /// Loads a Pdf document in the Pdf viewer
        /// </summary>
        /// <param name="filePath">The path for the Pdf document to display in the pdf viewer</param>
        /// <param name="password">The password for opening the document.</param>
        public void Load(String filePath, string password)
        {
            try
            {
                if (!File.Exists(filePath))
                    throw new FileNotFoundException("Cannot able to locate the specified file");

                if (m_documentView != null)
                {
                    if (m_documentView.LoadedDocument == null)
                    {
                        m_documentView.Load(filePath, password);
                        m_documentToolbar.Initialize(m_documentView);
                    }
                    else
                    {
                        m_documentView.Unload();
                        m_documentView.Load(filePath, password);
                        m_documentToolbar.Initialize(m_documentView);
                    }
                }
                else
                {
                    documentPath = filePath;
                    this.password = password;
                }

            }
            catch (Syncfusion.Pdf.PdfException exp)
            {
                m_notificationMessage = "Essential PDF Viewer could not open the PDF document with the given password";
                m_exceptions.Exceptions.Append("Error while loading the document with the password provided\nThe complete stack is as given below" + exp.StackTrace.ToString() + "\r\n\r\n");
                notificationVisibilityChange(true);
                return;
            }
            catch (Exception ex)
            {
                m_notificationMessage = "Essential PDF Viewer could not open the PDF document";
                m_exceptions.Exceptions.Append("Error while loading the document with the password provided\nThe complete stack is as given below" + ex.StackTrace.ToString() + "\r\n\r\n");
                notificationVisibilityChange(true);
                m_exceptions.Exceptions.Append(ex.Message);
                return;
            }
        }
        /// <summary>
        /// Loads a pdf document in the Pdf viewer from the specified PdfLoadedDocuemnt.
        /// </summary>
        /// <param name="loadedDocument">The PdfLoadedDocument to be viewed in the PdfViewer</param>
        public void Load(PdfLoadedDocument loadedDocument)
        {
            try
            {
                if (m_documentView != null)
                {
                    if (m_documentView.LoadedDocument == null)
                    {
                        m_documentView.LoadedDocument = loadedDocument;
                        m_documentToolbar.Initialize(m_documentView);
                    }
                    else
                    {
                        m_documentView.Unload();
                        m_documentView.LoadedDocument = loadedDocument;
                        m_documentToolbar.Initialize(m_documentView);
                    }
                }
                else
                {
                    loadedDoc = loadedDocument;
                }
            }
            catch (Exception ex)
            {
                m_notificationMessage = "Essential PDF Viewer could not open the PDF document";
                m_exceptions.Exceptions.Append("Error while loading the document with the password provided\nThe complete stack is as given below" + ex.StackTrace.ToString() + "\r\n\r\n");
                notificationVisibilityChange(true);
                m_exceptions.Exceptions.Append(ex.Message);
                return;
            }
        }
        /// <summary>
        /// Unloads the Pdf document
        /// </summary>
        public void Unload()
        {
            if (m_documentView != null && m_documentView.LoadedDocument != null)
            {
                m_documentView.Unload();
            }
            this.m_documentToolbar.Reset();
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="eventArgs"></param>
        internal void OnLoaded(EventArgs eventArgs)
        {
            if (DocumentLoaded != null)
                DocumentLoaded(this, eventArgs);
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="eventArgs"></param>
        internal void OnClicked(AnnotEventArgs eventArgs)
        {
            if (HyplerLinkClicked != null)
                HyplerLinkClicked(this, eventArgs);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="eventArgs"></param>
        internal void OnMouseOver(EventArgs eventArgs)
        {
            if (HyperLinkMouseOver != null)
                HyperLinkMouseOver(this, eventArgs);
        }
        /// <summary>
        /// Navigates to the specified page.
        /// </summary>
        /// <param name="index">The page index</param>
        public void GoToPageAtIndex(int index)
        {
            if (m_documentView != null)
                m_documentView.PdfDocumentViewer.GoToPageAtIndex(index - 1);
            else
                GoToPageIndexBinded = index;
        }

        public override void OnApplyTemplate()
        {
			FirstPageCommand = new DelegateCommand(OnFirstPageCommand, CanExecute);
            LastPageCommand = new DelegateCommand(OnLastPageCommand, CanExecute);
            PreviousPageCommand = new DelegateCommand(OnPreviousPageCommand, CanExecute);
            NextPageCommand = new DelegateCommand(OnNextPageCommand, CanExecute);
            IncreaseZoomCommand = new DelegateCommand(OnIncreaseZoomCommand, CanExecute);
            DecreaseZoomCommand = new DelegateCommand(OnDecreaseZoomCommand, CanExecute);
            PrintCommand = new DelegateCommand(OnPrintCommand, CanExecute);
            GoToPageCommand = new DelegateCommand(OnGoToPageCommand, CanExecute);

            this.KeyDown += new KeyEventHandler(PdfViewerControl_KeyDown);
            this.Focus();
            m_documentView = (PdfDocumentView)GetTemplateChild("PART_DocumentView");
            m_notificationBar = (NotificationBar)GetTemplateChild("PART_NotificationBar");
            m_documentToolbar = (DocumentToolbar)GetTemplateChild("PART_Toolbar");
            m_textSearchBar = (TextSearchBar)GetTemplateChild("PART_TextSearchBar");            
            m_documentView.KeyDown += new KeyEventHandler(m_documentView_KeyDown);
            m_textSearchBar.btnNext.Click += new RoutedEventHandler(btnNext_Click);
            m_textSearchBar.btnPrev.Click += new RoutedEventHandler(btnPrev_Click);
            m_textSearchBar.btnClose.Click += new RoutedEventHandler(btnClose_Click);
            m_textSearchBar.txtSearch.GotFocus += new RoutedEventHandler(txtSearch_GotFocus);
            m_documentView.KeyDown += new KeyEventHandler(m_documentView_KeyDown);
            m_textSearchBar.txtSearch.TextChanged += new TextChangedEventHandler(txtSearch_TextChanged);
            if (m_documentView != null)
            {
                m_documentToolbar.ActiveView = m_documentView;
                if (m_showToolbar)
                    m_documentToolbar.Visibility = Visibility.Visible;
                else
                    m_documentToolbar.Visibility = Visibility.Collapsed;
                if (!m_isNotifying && m_notificationBar != null)
                {
                    m_notificationBar.Visibility = Visibility.Collapsed;
                }
                else if (m_notificationBar != null && EnableNotificationBar == true)
                {
                    m_notificationBar.label1.Content = m_notificationMessage;
                    m_notificationBar.Visibility = Visibility.Visible;
                }
                this.SizeChanged += new SizeChangedEventHandler(PdfViewer_SizeChanged);
                if (!string.IsNullOrEmpty(ItemSource as string))
                {                    
                    if (DesignerProperties.GetIsInDesignMode(this))
                    {
                        ItemSource = string.Empty;
                    }
                    else
                    {
                        documentPath = ItemSource as string;
                    }
                }
                m_textSearchBar.Visibility = Visibility.Collapsed;
                if (documentPath != string.Empty || password != string.Empty)
                {
                    if (password != string.Empty)
                    {
                        Load(documentPath);
                    }
                    else
                    {
                        Load(documentPath, password);
                    }
                }
                else if (documentStream != null)
                {
                    Load(documentStream);
                }
                else if (loadedDoc != null)
                {
                    Load(loadedDoc);
                }
            }
            if (GoToPageIndexBinded > 0)
                m_documentView.CurrentPageIndex = GoToPageIndexBinded;
            AddHandler(PreviewMouseDownEvent, new MouseButtonEventHandler(SelectivelyIgnoreMouseButton), true);
            m_documentView.ZoomMode = ZoomMode;
        }

        void btnClose_Click(object sender, RoutedEventArgs e)
        {
            if (m_textSearchBar.Visibility == Visibility.Visible)
            {
                m_textSearchBar.Visibility = Visibility.Collapsed;
            }
            m_documentView.PdfDocumentViewer.ClearSearch();
        }
        void txtSearch_TextChanged(object sender, TextChangedEventArgs e)
        {
            m_documentView.PdfDocumentViewer.m_nextMatch = -1;
        }
        void btnNext_Click(object sender, RoutedEventArgs e)
        {
            m_documentView.PdfDocumentViewer.TextSearch(m_textSearchBar.txtSearch.Text, true);
            m_documentView.PdfDocumentViewer.DrawTextSearch(m_documentView.PdfDocumentViewer.CurrentPageIndex);
        }

        void btnPrev_Click(object sender, RoutedEventArgs e)
        {
            m_documentView.PdfDocumentViewer.TextSearch(m_textSearchBar.txtSearch.Text, false);
            m_documentView.PdfDocumentViewer.DrawTextSearch(m_documentView.PdfDocumentViewer.CurrentPageIndex);
        }

        void txtSearch_GotFocus(object sender, RoutedEventArgs e)
        {
            m_documentView.PdfDocumentViewer.m_nextMatch = -1;
            m_textSearchBar.txtSearch.SelectAll();
            Keyboard.Focus(m_textSearchBar.txtSearch);
            m_textSearchBar.Visibility = Visibility.Visible;
            m_textSearchBar.UpdateLayout();
            m_textSearchBar.InvalidateVisual();
            m_textSearchBar.txtSearch.Focusable = true;
            Keyboard.Focus(m_textSearchBar.txtSearch);
        }

        void PdfViewerControl_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.F && Keyboard.Modifiers == ModifierKeys.Control)
            {
                m_textSearchBar.Visibility = Visibility.Visible;
                m_textSearchBar.UpdateLayout();
                m_textSearchBar.InvalidateVisual();
                m_textSearchBar.txtSearch.Focusable = true;
                Keyboard.Focus(m_textSearchBar.txtSearch);                
            }
            if (m_textSearchBar.txtSearch.IsFocused == true && e.Key == Key.Enter)
            {
                RoutedEventArgs args = new RoutedEventArgs();
                btnNext_Click(this, args);
            }
        } 

        void m_documentView_KeyDown(object sender, KeyEventArgs e)
        {
            this.Focus();
            if (e.Key == Key.F && Keyboard.Modifiers == ModifierKeys.Control)
            {
                if (m_textSearchBar.Visibility == Visibility.Visible)
                {
                    m_textSearchBar.Visibility = Visibility.Collapsed;
                }
                else
                {
                    m_textSearchBar.Visibility = Visibility.Visible; 
                }
            }

        }

        void PdfViewer_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            if (m_documentView != null)
            {
                m_documentView.Width = e.NewSize.Width;
                m_documentView.Height = e.NewSize.Height - m_documentToolbar.ActualHeight;
                m_textSearchBar.txtSearch.MinWidth = e.NewSize.Width / 7;
                m_textSearchBar.txtSearch.MinWidth = e.NewSize.Width / 7;
                m_textSearchBar.txtSearch.InvalidateMeasure();
                m_textSearchBar.InvalidateVisual();
            }
            m_documentView.ZoomMode = m_documentView.ZoomMode;
        }

        private static void SelectivelyIgnoreMouseButton(object sender, MouseButtonEventArgs e)
        {
            bool invalidClick = false;
            DependencyObject parent = e.OriginalSource as UIElement;
            while (parent != null)
            {
                Type parentType = parent.GetType();
                if (parentType.Name == "ScrollBar" || parentType.Name == "ScrollChrome" || parentType.Name == "DocumentToolbar" || parentType.Name == "ScrollViewer")
                {
                    invalidClick = true;
                    if (e.ChangedButton == MouseButton.Right)
                    {
                        e.Handled = true;
                    }
                }
                parent = VisualTreeHelper.GetParent(parent);
            }
            if (invalidClick == false)
            {
                PdfViewerControl sndr = sender as PdfViewerControl;
                if (e.ChangedButton == MouseButton.Left)
                {
                    sndr.RaiseEvent(new MouseButtonEventArgs(Mouse.PrimaryDevice, Environment.TickCount, MouseButton.Left) { RoutedEvent = Mouse.MouseDownEvent });
                }
                else if (e.ChangedButton == MouseButton.Right)
                {
                    sndr.RaiseEvent(new MouseButtonEventArgs(Mouse.PrimaryDevice, Environment.TickCount, MouseButton.Right) { RoutedEvent = Mouse.MouseDownEvent });
                }
            }
        }

        /// <summary>
        /// Returns the page number and rectangle postions of the text matchs found in the page
        /// </summary>
        /// <param name="text">The text to be searched</param>
        /// <param name="matchRect">Holds the page number and rectangle positions of the text matches</param> 
        public bool FindText(String text, out Dictionary<int, List<System.Drawing.RectangleF>> matchRect)
        {
            matchRect = new Dictionary<int, List<System.Drawing.RectangleF>>();
            bool matchFound=m_documentView.FindText(text, out matchRect);
            return matchFound;
        }
        public void InsertNotificationBar(string message)
        {
            if (m_notificationBar != null)
            {
                m_notificationBar.label1.Content = message;
                m_notificationBar.Visibility = Visibility.Visible;
                this.SizeChanged += new SizeChangedEventHandler(PdfViewer_SizeChanged);
            }
        }
        /// <summary>
        /// Exports the specified page as Image
        /// </summary>
        /// <param name="pageIndex">The page index to be converted into image</param>
        /// <returns>Returns the specified page as BitmapSource</returns>
        public BitmapSource ExportAsImage(int pageIndex)
        {
            return m_documentView.ExportAsImage(pageIndex);
        }

        /// <summary>
        /// Exports the specified pages as Images
        /// </summary>
        /// <param name="startIndex">The starting page index</param>
        /// <param name="endIndex">The ending page index</param>
        /// <returns>Returns the specified pages as Images</returns>
        public BitmapSource[] ExportAsImage(int startIndex, int endIndex)
        {
            return m_documentView.ExportAsImage(startIndex, endIndex);
        }

        /// <summary>
        /// Magnifies the page of the document to the provided zoom percentage.
        /// </summary>
        /// <param name="percentage">Zoom percentage</param>
        public void ZoomTo(int percentage)
        {
            if (m_documentView != null)
            {
                m_documentView.ZoomTo(percentage);
                m_documentToolbar.cmbCurrentZoomLevel.Text = percentage.ToString();
            }
        }
		 #region Command methods
        public void GotoPage(int pNumber)
        {
            if (m_documentView != null && m_documentView.LoadedDocument != null && pNumber <= PageCount)
            {
                GoToPageAtIndex(pNumber);
            }
        }

        private bool CanExecute(object parameter)
        {
            return true;
        }

        internal void OnFirstPageCommand(object sender)
        {
            if (m_documentView != null && PageCount > 0)
            {
                GotoPage(1);
            }
        }

        internal void OnLastPageCommand(object sender)
        {
            if (m_documentView != null && PageCount > 0)
            {
                GotoPage(PageCount);
            }
        }

        internal void OnPreviousPageCommand(object sender)
        {
            int pageIndex = m_documentView.CurrentPageIndex;
            if (m_documentView != null && PageCount > 0)
            {
                if (pageIndex > 0)
                    GotoPage(pageIndex);
            }
        }

        internal void OnNextPageCommand(object sender)
        {
            int pageIndex = m_documentView.CurrentPageIndex;
            if (m_documentView != null && PageCount > 0)
            {
                if (pageIndex < (PageCount - 1))
                {
                    GotoPage(pageIndex + 2);
                }
            }
        }

        internal void OnIncreaseZoomCommand(object sender)
        {
            if (m_documentView != null && PageCount > 0 && m_documentView.ZoomFactor < 3.0f)
            {
                ZoomTo((int)((m_documentView.ZoomFactor * 100) + 25));
            }
        }

        internal void OnDecreaseZoomCommand(object sender)
        {
            if (m_documentView != null && PageCount > 0 && m_documentView.ZoomFactor > 1.0f)
            {
                ZoomTo((int)((m_documentView.ZoomFactor * 100) - 25));
            }
        }

        internal void OnPrintCommand(object sender)
        {
            if (m_documentView != null && PageCount > 0)
            {
                m_documentView.Print();
            }
        }

        internal void OnGoToPageCommand(object destPageParm)
        {
            string destPageStr = destPageParm as String;
            int destPage = -1;
            bool result = Int32.TryParse(destPageStr, out destPage);
            if (result && destPage > 0 && destPage <= PageCount)
            {
                GotoPage(destPage);
            }
        }
        # endregion
    }
	 public class DelegateCommand : ICommand
    {
        /// <summary>
        /// Occurs when changes occur that affect whether the command should execute.
        /// </summary>
        public event EventHandler CanExecuteChanged;

        Func<object, bool> canExecute;
        Action<object> executeAction;
        bool canExecuteCache;

        /// <summary>
        /// Initializes a new instance of the <see cref="DelegateCommand"/> class.
        /// </summary>
        /// <param name="executeAction">The execute action.</param>
        /// <param name="canExecute">The can execute.</param>
        public DelegateCommand(Action<object> executeAction,
                               Func<object, bool> canExecute)
        {
            this.executeAction = executeAction;
            this.canExecute = canExecute;
        }

        #region ICommand Members
        /// <summary>
        /// Defines the method that determines whether the command 
        /// can execute in its current state.
        /// </summary>
        /// <param name="parameter">
        /// Data used by the command. 
        /// If the command does not require data to be passed,
        /// this object can be set to null.
        /// </param>
        /// <returns>
        /// true if this command can be executed; otherwise, false.
        /// </returns>
        public bool CanExecute(object parameter)
        {
            bool tempCanExecute = canExecute(parameter);

            if (canExecuteCache != tempCanExecute)
            {
                canExecuteCache = tempCanExecute;
                if (CanExecuteChanged != null)
                {
                    CanExecuteChanged(this, new EventArgs());
                }
            }

            return canExecuteCache;
        }

        /// <summary>
        /// Defines the method to be called when the command is invoked.
        /// </summary>
        /// <param name="parameter">
        /// Data used by the command. 
        /// If the command does not require data to be passed, 
        /// this object can be set to null.
        /// </param>
        public void Execute(object parameter)
        {
            executeAction(parameter);
        }
        #endregion
    }
}
