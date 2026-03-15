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
using System.Drawing;
using System.Data;
using System.Text;
using System.Windows.Forms;
using System.Drawing.Drawing2D;
using Syncfusion.PdfViewer.Base;

namespace Syncfusion.Windows.Forms.PdfViewer
{
    /// <summary>
    /// NotificationBar for the PdfViewerControl.
    /// </summary>
	[ToolboxItem(false)]
    public partial class NotificationBar : UserControl
    {
        internal PdfViewerExceptions exception = new PdfViewerExceptions();
        internal string exceptionDetails = string.Empty;
        #region Members
        /// <summary>
        /// Boundary of the notification bar to draw the border
        /// </summary>
        private Rectangle m_bound;
        /// <summary>
        /// Graphics of the notification bar
        /// </summary>
        private Graphics m_grapx;
        /// <summary>
        /// Instance of the PdfViewerExceptions class to collect the exceptions thrown
        /// </summary>
        private PdfViewerExceptions m_exception = new PdfViewerExceptions();
        /// <summary>
        /// Corresponding viewer to add the notification bar control
        /// </summary>
        private static PdfViewerControl m_pdfViewer;
        /// <summary>
        /// To specify the visibility of the notification bar
        /// </summary>
        private bool m_visible;
        #endregion

        #region Properties
        /// <summary>
        /// Get and set PdfViewerControl in which notification bar is to be added
        /// </summary>
        internal PdfViewerControl Viewer
        {
            get
            {
                return m_pdfViewer;
            }
            set
            {
                m_pdfViewer = value;
            }
        }
        /// <summary>
        /// Get and set the visibility of the notification bar
        /// </summary>
        internal bool Visibility
        {
            get
            {
                return m_visible;
            }
            set
            {
                m_visible = value;
            }
        }
        #endregion

        #region Constructors
        /// <summary>
        /// Initializes NotificationBar
        /// </summary>
        public NotificationBar()
        {
            InitializeComponent();
            m_bound = this.Bounds;
            m_grapx = this.CreateGraphics();
            m_exception.Exceptions.Length = 0;

            this.Paint += new PaintEventHandler(NotificationBar_Paint);
            m_grapx.DrawRectangle(new Pen(Color.FromArgb(241, 246, 252), 1), m_bound);
            button1.Text = "X";
            button1.Dock = DockStyle.Right;
            label1.ForeColor = Color.FromArgb(0, 50, 105);
            label1.TextAlign = ContentAlignment.MiddleLeft;
            button1.Size = new Size(50, 30);
            button1.BackColor = Color.Transparent;
            button1.Click += new EventHandler(button1_Click);
            linkLabel1.Click += new EventHandler(linkLabel1_Click);
            m_grapx.DrawRectangle(new Pen(Color.Red, 1), m_bound);
            if (m_pdfViewer != null && m_pdfViewer.IsNotificationBarClosed)
                m_pdfViewer.addControl(this);
            m_grapx.DrawRectangle(new Pen(Color.Red), m_bound);
        }
        /// <summary>
        /// Displays error in NotificationBar.
        /// </summary>
        /// <param name="message">The Message</param>
        /// <param name="exception">The Exception</param>
        public NotificationBar(string message,string exception)
        {
            InitializeComponent();
            m_bound = this.Bounds;
            m_grapx = this.CreateGraphics();
            m_exception.Exceptions.Length = 0;

            this.Paint += new PaintEventHandler(NotificationBar_Paint);
            button1.Text = "X";
            button1.Dock = DockStyle.Right;
            button1.BackColor = Color.Transparent;
            button1.Size = new Size(35, 30);

            label1.Text = message;
            label1.ForeColor = Color.FromArgb(0, 50, 105);

            if (m_pdfViewer != null && m_pdfViewer.IsNotificationBarClosed==false)
                m_pdfViewer.addControl(this);
            button1.Click += new EventHandler(button1_Click);
            linkLabel1.Click += new EventHandler(linkLabel1_Click);

            m_grapx.DrawRectangle(new Pen(Color.Red), m_bound);
            m_pdfViewer.m_documentView.Focus();
            exceptionDetails = exception;
            m_pdfViewer.exceptions=exceptionDetails;
        }
        /// <summary>
        /// Displays error in NotificationBar.
        /// </summary>
        /// <param name="exception">The Exception</param>
        public NotificationBar(string exception)
        {
            InitializeComponent();
            m_bound = new Rectangle(0, 0, 0, 0);
            m_pdfViewer.exceptions = exception;
            m_pdfViewer.IsNotificationBarClosed = false;
            m_pdfViewer.removeControl(this);
        }
        #endregion

        #region Events
        void button1_Paint(object sender, PaintEventArgs e)
        {
            if (e.ClipRectangle != Rectangle.Empty)
            {
                Pen pen = new Pen(Color.FromArgb(180, 203, 255), 1);
                LinearGradientBrush brush = new LinearGradientBrush(new Point(e.ClipRectangle.X, e.ClipRectangle.Y), new Point(e.ClipRectangle.X, e.ClipRectangle.Y + e.ClipRectangle.Height), Color.FromArgb(207, 223, 244), Color.FromArgb(180, 203, 232));
                e.Graphics.FillRectangle(brush, e.ClipRectangle);
                e.Graphics.DrawString("X", new Font("Arial Black", 10), Brushes.DarkBlue, new PointF(e.ClipRectangle.X + 15, e.ClipRectangle.Y + 11));
                Point first, second;
                first = new Point(e.ClipRectangle.X, 0);
                second = new Point(e.ClipRectangle.X + e.ClipRectangle.Width, 0);
                e.Graphics.DrawLine(pen, first, second);
                first = new Point(e.ClipRectangle.X, e.ClipRectangle.Height - 1);
                second = new Point(e.ClipRectangle.X + e.ClipRectangle.Width, e.ClipRectangle.Height - 1);
                e.Graphics.DrawLine(pen, first, second);
                first = new Point(e.ClipRectangle.X + e.ClipRectangle.Width - 1, 0);
                second = new Point(e.ClipRectangle.X + e.ClipRectangle.Width - 1, e.ClipRectangle.Height - 1);
                e.Graphics.DrawLine(pen, first, second);
            }
        }

        void linkLabel1_Click(object sender, EventArgs e)
        {
            try
            {
                Clipboard.Clear();
                Clipboard.SetDataObject(m_pdfViewer.exceptions, true, 5, 50);
                m_pdfViewer.m_documentView.Focus();
            }
            catch(Exception exce)
            {
                exception.Exceptions.Append(exce.Message + "\r\n" + exce.StackTrace + "\r\n");
            }
        }

        void button1_Click(object sender, EventArgs e)
        {
            m_exception.Exceptions.Length = 0;
            m_pdfViewer.m_notificationBar.Visibility = false;
            m_pdfViewer.m_documentView.Focus();
            m_visible = false;
            m_pdfViewer.IsNotificationBarClosed = true;
            this.Dispose(true);
        }

        private void NotificationBar_Paint(object sender, PaintEventArgs e)
        {
            if (e.ClipRectangle != Rectangle.Empty)
            {
                Pen pen = new Pen(Color.FromArgb(180, 203, 255), 1);
                LinearGradientBrush brush = new LinearGradientBrush(new Point(e.ClipRectangle.X, e.ClipRectangle.Y), new Point(e.ClipRectangle.X, e.ClipRectangle.Y + e.ClipRectangle.Height), Color.FromArgb(207, 223, 244), Color.FromArgb(180, 203, 232));
                e.Graphics.FillRectangle(brush, e.ClipRectangle);
                Point first, second;
                first = new Point(e.ClipRectangle.X, 0);
                second = new Point(e.ClipRectangle.X + e.ClipRectangle.Width, 0);
                e.Graphics.DrawLine(pen, first, second);
                first = new Point(e.ClipRectangle.X, e.ClipRectangle.Height - 1);
                second = new Point(e.ClipRectangle.X + e.ClipRectangle.Width, e.ClipRectangle.Height - 1);
                e.Graphics.DrawLine(pen, first, second);
                e.Graphics.DrawLine(pen, new Point(0, 0), new Point(0, e.ClipRectangle.Height));
            }
        }
        #endregion
    }
}
