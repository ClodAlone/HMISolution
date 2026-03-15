#region Copyright Syncfusion Inc. 2001 - 2014
// Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Any infringement will be prosecuted under
// applicable laws. 
#endregion
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Globalization;
using System.Text;
using System.Threading;
using System.Windows.Forms;
using Syncfusion.Pdf.Parsing;
using Syncfusion.PdfViewer.Base;
using System.IO;
using Syncfusion.Pdf;
using Syncfusion.Windows.PdfViewer;
using System.Drawing.Printing;
using System.Drawing.Imaging;

namespace Syncfusion.Windows.Forms.PdfViewer
{
    /// <summary>
    /// PdfViewerControl helps to view and print PDF files. The DocumentToolbar helps to navigate
    /// easily anywhere inside the document and also provides direct access to open, save and print
    /// documents.
    /// </summary>
    [ToolboxItem(true)]
    [ToolboxBitmap(typeof(PdfViewerControl), "icons.pdfviewer.bmp")]
    public partial class PdfViewerControl : Control
    {
        #region Members
        internal bool IslinkCliked = false;
        internal bool IslinkHover = false;
        internal string exceptions = string.Empty;
        internal bool IsNotificationBarClosed;
        private bool m_showToolbar = true;
        internal PdfViewerExceptions m_exceptions = new PdfViewerExceptions();
        private DocumentToolbar m_documentToolbar;
        internal PdfDocumentView m_documentView;
        internal NotificationBar m_notificationBar;
        private TableLayoutPanel panel = new TableLayoutPanel();
        private bool m_enableNoticationBar = true;
        internal NotificationBar m_notify;
        #endregion

        #region Constructors
        /// <summary>
        /// Initializes PdfViewerControl.
        /// </summary>
        public PdfViewerControl()
        {
            InitializeComponent();
            m_notificationBar = new NotificationBar();
            m_notificationBar.Viewer = this;
            m_notificationBar.Visibility = false;

            panel.ColumnCount = 1;
            panel.RowCount = 2;

            panel.ColumnCount = 1;
            panel.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
            panel.Location = new System.Drawing.Point(3, 3);
            panel.Name = "tableLayoutPanel1";
            panel.RowCount = 2;
            panel.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 30F));
            panel.RowStyles.Add(new System.Windows.Forms.RowStyle());
            panel.AutoSize = true;
            panel.Dock = DockStyle.Fill;

            m_documentToolbar = new DocumentToolbar();
            m_documentToolbar.Dock = DockStyle.Fill;
            panel.Controls.Add(m_documentToolbar, 0, 0);

            m_documentView = new PdfDocumentView();
            m_documentView.Dock = DockStyle.Fill;
            //m_documentView.AutoScroll = false;
            m_documentToolbar.ActiveView = m_documentView;
            panel.Controls.Add(m_documentView, 0, 2);


            this.Controls.Add(panel);
            panel.Parent = this;
            panel.PerformLayout();

            m_documentView.SizeChanged += new EventHandler(m_documentView_SizeChanged);
        }

        void ImageStructure_ImagePreRender(object sender, ImagePreRenderEventArgs args)
        {
            if (ImagePreRender != null)
                ImagePreRender(sender, args);
        }

        void m_documentView_SizeChanged(object sender, EventArgs e)
        {
            m_documentView.Invalidate();
            if (m_documentView.DrawingPanel != null)
            {
                m_documentView.ScrollDown = null;
                m_documentView.DrawingPanel.Invalidate();
            }
        }
        #endregion

        #region Properties
        /// <summary>
        /// Gets the print document
        /// </summary>
        [Browsable(false)]
        public PrintDocument PrintDocument
        {
            get
            {
                CultureInfo current = Thread.CurrentThread.CurrentCulture;
                Thread.CurrentThread.CurrentCulture = CultureInfo.InvariantCulture;
                return m_documentView.PrintDocument;
                Thread.CurrentThread.CurrentCulture = current;
            }
        }
        /// <summary>
        /// Gets the page count
        /// </summary>
        public int PageCount
        {
            get
            {
                if (m_documentView == null)
                    return 0;

                return m_documentView.PageCount;
            }
        }
        /// <summary>
        /// Gets and sets the visibility of the toolbar
        /// </summary>
        public bool ShowToolBar
        {
            get
            {
                return m_showToolbar;
            }
            set
            {
                m_showToolbar = value;
                if (!m_showToolbar)
                {
                    this.removeControl(m_documentToolbar);
                }
                else
                {
                    this.addControl(m_documentToolbar);
                }
            }
        }
        /// <summary>
        /// Returns the index of the current page displayed in the Viewer
        /// </summary>
        public int CurrentPageIndex
        {
            get
            {
                return m_documentView.CurrentPageIndex;
            }
        }

        /// <summary>
        /// Enables the display of Notification bar on setting true.
        /// </summary>
        public bool EnableNotificationBar
        {
            get
            {
                return m_enableNoticationBar;
            }
            set
            {
                m_enableNoticationBar = value;
            }
        }

        /// <summary>
        /// Magnifies the page of the document to the provided zoom percentage.
        /// </summary>
        /// <param name="percentage">Zoom percentage</param>
        public void ZoomTo(int percentage)
        {
            m_documentView.ZoomTo(percentage);
            m_documentToolbar.cmbCurrentZoomLevel.Text = percentage.ToString();
        }

        /// <summary>
        /// Gets or sets the displacement value for scrolling.
        /// </summary>
        public int ScrollDisplacementValue
        {
            get
            {
                return m_documentView.ScrollDisplacementValue;
            }
            set
            {
                m_documentView.ScrollDisplacementValue = value;
            }
        }
        #endregion

        #region Events
        /// <summary>
        /// Delegate used for KeyPressed event.
        /// </summary>
        public delegate void KeyPressedEventHandler(object sender, KeyPressEventArgs args);
        /// <summary>
        /// Delegate used for DocumentLoaded event.
        /// </summary>
        public delegate void DocumentLoadedEventHandler(object sender, EventArgs args);
        /// <summary>
        /// Delegate used for HyperLinkClicked event.
        /// </summary>
        public delegate void HyperLinkClickedEventHandler(object sender, AnnotEventArgs args);
        /// <summary>
        /// Delegate used for HyperLinkMouseOver event.
        /// </summary>
        public delegate void HyperLinkMouseoverEventHandler(object sender, AnnotEventArgs args);
        /// <summary>
        /// Delegate used for ImagePreRender event.
        /// </summary>
        public delegate void ImagePreRenderEventHandler(object sender, ImagePreRenderEventArgs args);
        
        /// <summary>
        /// Occurs when a key is pressed
        /// </summary>
        public event KeyPressedEventHandler KeyPressed;
        /// <summary>
        /// Occurs when the pdf document is loaded
        /// </summary>
        public event DocumentLoadedEventHandler DocumentLoaded;
        /// <summary>
        /// Occurs when a hyperlink is clicked.
        /// </summary>
        public event HyperLinkClickedEventHandler HyperLinkClicked;
        /// <summary>
        /// Occurs when a hyperlink is hovered;
        /// </summary>
        public event HyperLinkMouseoverEventHandler HyperlinkHover;

        /// <summary>
        /// Occurs prior to the rendering of every image in the document
        /// </summary>
        public event ImagePreRenderEventHandler ImagePreRender;


        #endregion Events

        #region Implementation
        /// <summary>
        /// Loads a Pdf document in the Pdf viewer
        /// </summary>
        /// <param name="filePath">The path for the Pdf document to display in the pdf viewer</param>
        public void Load(string filePath)
        {
            try
            {
                if (!File.Exists(filePath))
                {
                    throw new FileNotFoundException("filePath");
                }
                if (this.HyperLinkClicked != null)
                {
                    this.IslinkCliked = true;
                }
                else
                {
                    this.IslinkCliked = false;
                }
                m_documentView.Load(filePath);
                GetDocumentOrientation();
                m_documentToolbar.Initialize(m_documentView);
            }
            catch (Exception ex)
            {
                m_notify = new NotificationBar("Essential PDF Viewer could not open the PDF document", ex.Message);
                return;
            }
        }
        /// <summary>
        /// Loads a Pdf document in the Pdf viewer
        /// </summary>
        /// <param name="filePath">The path for the Pdf document to display in the pdf viewer</param>
        /// <param name="password">The password for opening the document.</param>
        public void Load(string filePath, string password)
        {
            try
            {
                if (!File.Exists(filePath))
                {
                    throw new FileNotFoundException("filePath");
                }
                m_documentView.Load(filePath, password);
                GetDocumentOrientation();
                m_documentToolbar.Initialize(m_documentView);
            }
            catch (PdfException exp)
            {
                m_notify = new NotificationBar("Essential PDF Viewer could not open the PDF document", exp.Message);
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
                if (loadedDocument == null)
                    throw new ArgumentNullException("Loaded document should not be null");
                m_documentView.LoadedDocument = loadedDocument;
                m_documentView.Load(loadedDocument);
                GetDocumentOrientation();
                m_documentToolbar.Initialize(m_documentView);
            }
            catch (Exception ex)
            {
                m_notify = new NotificationBar("Essential PDF Viewer could not open the PDF document", ex.Message);
            }
        }
        /// <summary>
        /// Loads a Pdf document  in the Pdf viewer from the specified stream.
        /// </summary>
        /// <param name="stream">A stream that contains the data for the Pdf document</param>
        public void Load(Stream stream)
        {
            m_documentView.Load(stream);
            GetDocumentOrientation();
            m_documentToolbar.Initialize(m_documentView);
        }

        internal void OnClicked(AnnotEventArgs eventArgs)
        {
            if (HyperLinkClicked != null)
                HyperLinkClicked(this, eventArgs);
        }
        internal void OnHover(AnnotEventArgs eventArgs)
        {
            if (HyperlinkHover != null)
                HyperlinkHover(this, eventArgs);
        }
        internal void OnLoaded(EventArgs eventArgs)
        {
            m_documentView.Focus();
            if (DocumentLoaded != null)
                DocumentLoaded(this, eventArgs);
            if (ImagePreRender != null)
                ImageStructure.ImagePreRender += new ImageStructure.ImagePreRenderEventHandler(ImageStructure_ImagePreRender);
        }
        internal void OnKeyPressed(KeyPressEventArgs eventArgs)
        {
            //m_documentView.Focus();
            if (KeyPressed != null)
            {
                KeyPressed(this, eventArgs);
            }
        }
        /// <summary>
        /// Unloads the Pdf document
        /// </summary>
        public void Unload()
        {
            if (m_documentView != null)
                m_documentView.Unload();
        }
        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        public void Dispose()
        {
            Unload();
            Dispose(true);
        }
        /// <summary>
        /// Exports the specified page as Image
        /// </summary>
        /// <param name="pageIndex">The page index to be converted into image</param>
        /// <returns>Returns the specified page as Image</returns>
        public Bitmap ExportAsImage(int pageIndex)
        {
            return m_documentView.ExportAsImage(pageIndex);
        }
        /// <summary>
        /// Returns the page number and rectangle positions of the text matchs
        /// </summary>
        /// <param name="text">The text to be searched</param>
        /// <param name="matchRect">Holds the page number and rectangle positions of the text matches</param>       

        public bool FindText(String text, out Dictionary<int, List<RectangleF>> matchRect)
        {
            Thread.CurrentThread.CurrentCulture = CultureInfo.InvariantCulture;
            bool IsMatchFound=m_documentView.FindText(text,out matchRect);
            return IsMatchFound;
        }

        /// <summary>
        /// Exports the specified pages as Images
        /// </summary>
        /// <param name="startIndex">The starting page index</param>
        /// <param name="endIndex">The ending page index</param>
        /// <returns>Returns the specified pages as Images</returns>
        public Bitmap[] ExportAsImage(int startIndex, int endIndex)
        {
            return m_documentView.ExportAsImage(startIndex, endIndex);
        }
        
        /// <summary>
        /// Exports the specified pages as Images
        /// </summary>
        /// <param name="startIndex">The starting page index</param>
        /// <param name="endIndex">The ending page index</param>
        /// <param name="customSize">The custom size of the converted image</param>
        /// <param name="keepAspectRatio">Whether need to maintain the pdf page size</param>
        /// <returns>Returns the specified pages as images with custom size</returns>
        public Bitmap[] ExportAsImage(int startIndex, int endIndex,SizeF customSize,bool keepAspectRatio)
        {
            return m_documentView.ExportAsImage(startIndex, endIndex,customSize,keepAspectRatio);
        }

         /// <summary>
        /// Exports the specified pages as Images
        /// </summary>
        /// <param name="startIndex">The starting page index</param>
        /// <param name="endIndex">The ending page index</param>
        /// <param name="customSize">The custom size of the converted image</param>
        /// <param name="dpiX">The horizondal resolution of the image</param>
        /// <param name="dpiY">The vertical resolution of the image</param>
        /// <param name="keepAspectRatio">Whether need to maintain the pdf page size</param>
        /// <returns>Returns the specified pages as images with custom size and resolution</returns>
        public Bitmap[] ExportAsImage(int startIndex, int endIndex, SizeF customSize, float dpiX,float dpiY,bool keepAspectRatio)
        {
            return m_documentView.ExportAsImage(startIndex, endIndex, customSize,dpiX,dpiY,keepAspectRatio);
        }

        /// <summary>
        /// Exports the specified page as Metafile
        /// </summary>
        /// <param name="pageIndex">The page index to be converted into image</param>
        /// <returns>Metafile</returns>
        public Metafile ExportAsMetafile(int pageIndex)
        {
            return m_documentView.ExportAsMetafile(pageIndex);
        }

        /// <summary>
        /// Exports the specified pages as Metafile
        /// </summary>
        /// <param name="startIndex">The starting page index</param>
        /// <param name="endIndex">The ending page index</param>
        /// <returns>Array of Metafile</returns>
        public Metafile[] ExportAsMetafile(int startIndex, int endIndex)
        {
            return m_documentView.ExportAsMetafile(startIndex, endIndex);
        }
         /// <summary>
        /// Exports the specified page as Image
        /// </summary>
        /// <param name="pageIndex">The page index to be converted into image</param>
        /// <param name="customSize">The custom size of the converted image</param>
        /// <param name="keepAspectRatio">Whether need to maintain the pdf page size</param>
        /// Returns the specified page as image with custom size
        public Bitmap ExportAsImage(int pageIndex, SizeF customSize,bool keepAspectRatio)
        {
            return m_documentView.ExportAsImage(pageIndex, customSize,keepAspectRatio);
        }

        /// <summary>
        /// Exports the specified page as Image
        /// </summary>
        /// <param name="pageIndex">The page index to be converted into image</param>
        /// <param name="customSize">The custom size of the converted image</param>
        /// <param name="dpiX">The horizondal resolution of the converted image</param>
        /// <param name="dpiY">The vertical resolution of the converted image</param>
        /// <param name="keepAspectRatio">Whether need to maintain the pdf page size</param>
        /// Returns the specified page as image with custom size and resolution
        public Bitmap ExportAsImage(int pageIndex, SizeF customSize, float dpiX,float dpiY,bool keepAspectRatio)
        {
            Bitmap img=m_documentView.ExportAsImage(pageIndex, customSize,keepAspectRatio);
            img.SetResolution(dpiX, dpiY);
            return img;
        }
        /// <summary>
        /// Navigates to the specified page.
        /// </summary>
        /// <param name="index">The page index</param>
        public void GoToPageAtIndex(int index)
        {
            m_documentView.GoToPageAtIndex(index);
        }
        internal void removeControl(Control ctrl)
        {
            ctrl.Visible = false;
            m_notificationBar.Visibility = false;
            if (ctrl is NotificationBar && panel.Controls.Container.Controls.ContainsKey("NotificationBar"))
            {
                panel.Controls.Container.Controls[2].Dispose();
            }
            else if (ctrl is DocumentToolbar && panel.Controls.Container.Controls.ContainsKey("DocumentToolbar"))
            {
                panel.RowStyles[0].Height = 0;
            }
        }
        internal void addControl(Control ctrl)
        {
            if (ctrl is DocumentToolbar)
            {
                if (!m_documentToolbar.Visible && m_showToolbar)
                {
                    ctrl.Visible = true;
                    panel.RowStyles[0].Height = 28;
                }
            }
            if (ctrl is NotificationBar)
            {
                if (!EnableNotificationBar)
                    return;
                if (m_notificationBar.Visibility)
                {
                    return;
                }
                else
                {
                    m_notificationBar.Visibility = true;
                    ctrl.Dock = DockStyle.Fill;
                    panel.Controls.Add(ctrl);
                }
            }
        }
        /// <summary>
        /// Sets the document orienation for PrintDocument.
        /// </summary>
        private void GetDocumentOrientation()
        {
            foreach (Page pge in m_documentView.Pages)
            {
                if (pge.Height < pge.Width && m_documentView.PrintDocument != null)
                {
                    //m_documentView.PrintDocument.DefaultPageSettings.Landscape = true;
                }
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="msg"></param>
        /// <param name="keyData"></param>
        /// <returns></returns>
        protected override bool ProcessCmdKey(ref Message msg, Keys keyData)
        {
            switch (keyData)
            {
                case Keys.Control | Keys.F:
                    SearchBox searchBox = new SearchBox();
                    m_documentView.AddSearchBox(searchBox);
                    break;
            }
            KeyPressEventArgs args = new KeyPressEventArgs(msg, keyData);
            OnKeyPressed(args);
            return false;
        }
        #endregion

    }
    /// <summary>
    /// Custom event argument class used to notify when a key is pressed.
    /// </summary>
    public class KeyPressEventArgs : EventArgs
    {
        private Message m_message = new Message();
        private Keys m_keyData = new Keys();
        /// <summary>
        /// Returns the message.
        /// </summary>
        public Message msg
        {
            get
            {
                return m_message;
            }
        }
        /// <summary>
        /// Returns the key data.
        /// </summary>
        public Keys KeyData
        {
            get
            {
                return m_keyData;
            }
        }
        internal KeyPressEventArgs(Message message,Keys keyData)
        {
            m_keyData = keyData;
            m_message = message;
        }
    }
}
