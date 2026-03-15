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
using Syncfusion.Windows.PdfViewer;
using System.Drawing.Drawing2D;
using Syncfusion.PdfViewer.Base;

namespace Syncfusion.Windows.Forms.PdfViewer
{
    [ToolboxItem(false)]
    internal partial class VirtualizingPagePanel : UserControl
    {
        #region Constants
        const int c_gapBetweenPages = 8;
        #endregion

        #region Members
        List<Page> m_pages;
        float m_zoomFactor = 1;
        int m_actualHeight;
        int m_actualWidth;
        internal int m_windowHeight;
        PdfViewerExceptions exceptions = new PdfViewerExceptions();
        NotificationBar m_notificationBar;
        private bool m_isLeftSet = true;
        #endregion

        #region Constructor
        public VirtualizingPagePanel()
        {
            InitializeComponent();

            this.DoubleBuffered = true;
        }
        #endregion

        #region Properties
        public int ActualWidth
        {
            get
            {
                return m_actualWidth;
            }
        }
        public int ActualHeight
        {
            get
            {
                return m_actualHeight;
            }
        }
        public List<Page> Pages
        {
            get
            {
                if (m_pages == null)
                    m_pages = new List<Page>();

                return m_pages;
            }
        }
        #endregion

        #region Implementation
        public void Initialize()
        {
            //For the first time, we compute and set each page bounds (i.e., it's draw location).
            ComputePageBounds();

            Width = ComputeWidth();
            Height = m_pages[m_pages.Count - 1].Bounds.Bottom + c_gapBetweenPages;
            m_windowHeight = Height;
            if (Height > 32767)
                Height = m_pages[0].Height;
            m_actualHeight = Height;
            m_actualWidth = Width;
        }

        public void Draw(PaintEventArgs e, float vScrollPosition, float hscrollPosition, float zoomFactor)
        {
            bool requiresRedraw = (m_zoomFactor != zoomFactor);

            int i = GetPageIndexAtPosition(vScrollPosition + c_gapBetweenPages);
            Console.WriteLine("Paint Called : " + i.ToString());

            if (this.Parent.Width > this.Width)
            {
                this.Left = (this.Parent.Width - this.Width) / 2;
                m_isLeftSet = true;
            }
            else if (m_isLeftSet)
            {
                this.Left = 0;
                m_isLeftSet = false;
            }
            Graphics g = e.Graphics;
            GraphicsState gs = g.Save();

            m_zoomFactor = zoomFactor;
            g.ScaleTransform(zoomFactor, zoomFactor);



            //Previous Page (if exists).
            Rectangle clipRect = Rectangle.Empty;
            if (i > 0)
            {
                m_pages[i - 1].Draw(e.Graphics, requiresRedraw);
            }

            //Current Page.
            m_pages[i].Draw(e.Graphics, requiresRedraw);

            //Next Page.
            if (i + 1 < m_pages.Count)
            {
                m_pages[i + 1].Draw(e.Graphics, requiresRedraw);
            }

            g.Restore(gs);
            if (exceptions.Exceptions.Length != 0)
            {
                m_notificationBar = new NotificationBar("Essential PDF Viewer could not load parts of the document", exceptions.Exceptions.ToString());
            }
        }

        #endregion

        #region Helper Methods
        internal int GetPageIndexAtPosition(float offset)
        {
            if (offset <= c_gapBetweenPages)
                return 0;

            List<int> indexes = new List<int>();
            //while (true)
            //{
                for (int i = 0; i < m_pages.Count; i++)
                {
                    int top = (int)(m_pages[i].Bounds.Top * m_zoomFactor);
                    int bottom = (int)(m_pages[i].Bounds.Bottom * m_zoomFactor);
                    
                    //if (m_pages[i].Bounds.Contains(new Point(m_pages[i].Bounds.Left, (int)offset)))
                    if(offset >= top && offset <= bottom)
                    {
                        //System.Diagnostics.Debug.WriteLine("Page Index : " + i);
                        return i;
                    }
                }
            //    offset += c_gapBetweenPages;
            //}
//#if DEBUG
            //System.Diagnostics.Debug.WriteLine(
            //    string.Format("Unable to find any visible pages at the offset {0}", offset));

            return 0;
//#endif
        }



        void ComputePageBounds()
        {
            //Note: Zoom factor will affect the page size.
            int top = this.Top + c_gapBetweenPages;
            for (int i = 0; i < m_pages.Count; i++)
            {
                Rectangle pageBounds = new Rectangle(0, top, m_pages[i].Width, m_pages[i].Height);

                top += (m_pages[i].Bounds.Height + c_gapBetweenPages);
                m_pages[i].Bounds = pageBounds;
            }
        }

        int ComputeWidth()
        {
            int width = 0;
            for (int i = 0; i < m_pages.Count; i++)
            {
                width = Math.Max(m_pages[i].Bounds.Width, width);
            }
            return width;
        }
        public void SetZoom(double sx)
        {
            m_zoomFactor = (float)sx;
        }
        #endregion
    }
}
