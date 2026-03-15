#region Copyright Syncfusion Inc. 2001 - 2014
// Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Any infringement will be prosecuted under
// applicable laws. 
#endregion
using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.IO;
using System.Windows.Forms;

using Syncfusion.Pdf;
using Syncfusion.Pdf.Graphics;
using Syncfusion.PdfViewer.Base;
using System.ComponentModel;

namespace Syncfusion.Windows.Forms.PdfViewer
{
    [ToolboxItem(false)]
    internal partial class PdfPageView :
                            Control,
                            IPdfPageView
    {
        #region Constants
        const int c_ShadowWidth = 4;
        const int c_ShadowHeight = 4;
        #endregion

        #region Static Members
        private static Pen s_borderPen;
        #endregion

        #region Members
        PdfPageResources m_resources;
        PdfRecordCollection m_recordCollection;
        Rectangle m_bounds;
        float m_sx = 1;
        float m_sy = 1;
        float m_dpiX;
        float m_dpiY;
        Size m_actualSize;
        Bitmap m_pageImage;
        bool m_bRequiresRedraw;
        #endregion

        #region Constructors
        static PdfPageView()
        {
            s_borderPen = new Pen(Color.Black);
        }

        public PdfPageView()
        {
            InitializeComponent();

        }
        #endregion

        #region Properties
        internal Size ActualSize
        {
            get
            {
                return m_actualSize;
            }
        }
        internal Rectangle Bounds
        {
            get
            {
                return m_bounds;
            }
            set
            {
                m_bounds = value;
            }
        }
        #endregion

        #region Implementation
        protected override void OnPaint(PaintEventArgs pe)
        {
            //base.OnPaint(pe);
            //Graphics g = pe.Graphics;

            //Draw(g, false);
        }

        void DrawPageBorders(Graphics g, Rectangle r)
        {
            if (Width <= 0 || Height <= 0)
                return;

            r.Width -= (int)s_borderPen.Width;
            r.Height -= (int)s_borderPen.Width;
            int widthExcludingShadow = r.Width - 1;
            int heightExcludingShadow = r.Height - 1;

            g.FillRectangle(Brushes.White, r);
            g.DrawRectangle(s_borderPen, r);

        }

        void DrawPageShadows(Graphics g, RectangleF r)
        {
            GraphicsState state = g.Save();

            g.InterpolationMode = InterpolationMode.Low;
            g.SmoothingMode = SmoothingMode.None;
            g.CompositingQuality = CompositingQuality.HighSpeed;

            RectangleF verticalShadow = new RectangleF(
                r.Right, r.Top + c_ShadowHeight, c_ShadowWidth, r.Height - c_ShadowHeight);
            g.FillRectangle(Brushes.Black, verticalShadow);

            RectangleF horizontalShadow = new RectangleF(r.Left + c_ShadowWidth, r.Bottom, r.Width, c_ShadowHeight);
            g.FillRectangle(Brushes.Black, horizontalShadow);

            g.Restore(state);
        }

        internal float ConvertToPixelX(float x)
        {
            return (float)(x * m_dpiX / 72.0f);
        }

        internal float ConvertToPixelY(float y)
        {
            return (float)(y * m_dpiY / 72.0f);
        }
        #endregion

        #region IPageView Members
        internal void Initialize(PdfPageBase page)
        {
            m_resources = PageResourceLoader.Instance.GetPageResources(page);

            using (MemoryStream stream = new MemoryStream())
            {
                page.Layers.CombineContent(stream);
                stream.Position = 0;

                ContentParser parser = new ContentParser(stream.ToArray());
                m_recordCollection = parser.ReadContent();
            }

            PdfUnitConvertor converter = new PdfUnitConvertor();
            Size clientRectangleSize = new Size((int)converter.ConvertToPixels(page.Size.Width, PdfGraphicsUnit.Point),
                (int)converter.ConvertToPixels(page.Size.Height, PdfGraphicsUnit.Point));

            Width = clientRectangleSize.Width;
            Height = clientRectangleSize.Height;

            m_actualSize = new Size(Width, Height);
        }

        void DrawToImage()
        {
            float width = m_actualSize.Width * m_sx;
            float height = m_actualSize.Height * m_sy;

            if (m_pageImage != null)
            {
                m_pageImage.Dispose();
                m_pageImage = null;
            }

            m_pageImage = new System.Drawing.Bitmap((int)width, (int)height);

            this.Width = (int)width;
            this.Height = (int)height;

            using (Graphics g = System.Drawing.Graphics.FromImage(m_pageImage))
            {
                RectangleF rect = new RectangleF(this.Bounds.Left, this.Bounds.Top, Width, Height);
                g.FillRectangle(Brushes.White, rect);

                //Set page alignment.
                //this.Left = Math.Max((this.Parent.Size.Width - this.Size.Width) / 2, 0);
                this.Bounds = new Rectangle(this.Bounds.Left, this.Bounds.Top, this.Width, this.Height);

                
                DrawPageBorders(g, Rectangle.Round(rect));

                //Apply scale tranform.
                g.ScaleTransform(m_sx, m_sy);
                m_dpiX = g.DpiX;
                m_dpiY = g.DpiY;

                //ImageRenderer renderer = new ImageRenderer(m_recordCollection, m_resources, g, true);
                //renderer.RenderAsImage();

                g.Dispose();
            }
        }

        internal void Draw(Graphics g)
        {

            GraphicsState gs = g.Save();
            
            g.TranslateTransform(this.Bounds.X, this.Bounds.Y);

            if (m_bRequiresRedraw || m_pageImage == null)
            {
                DrawToImage();
                m_bRequiresRedraw = false;
            }

            g.DrawImageUnscaled(m_pageImage, Point.Empty);

            g.Restore(gs);

            //g.PageUnit = GraphicsUnit.Pixel;

            //GraphicsState gs = g.Save();

            //this.Left = this.Bounds.Left;

           
            //g.TranslateTransform(this.Bounds.X, this.Bounds.Y);
            ////DrawToImage();

            ////g.DrawImageUnscaled(m_pageImage, Point.Empty);
            ////g.DrawString(this.Name, new Font(FontFamily.GenericSansSerif, 25f), Brushes.Black, PointF.Empty);

            ////Apply scale tranform.
            //g.ScaleTransform(m_sx, m_sy);
            //m_dpiX = g.DpiX;
            //m_dpiY = g.DpiY;

            //ImageRenderer renderer = new ImageRenderer(m_recordCollection, m_resources, g, true);
            //renderer.RenderAsImage();


            //g.Restore(gs);

            

        }

        internal void Draw(Graphics g, bool printing)
        {

            g.PageUnit = GraphicsUnit.Pixel;

            GraphicsState gs = g.Save();

            //this.Left = (this.Parent.Size.Width - this.Size.Width) / 2;
            g.TranslateTransform(0, 0);

            if (m_bRequiresRedraw || m_pageImage == null)
            {
                DrawToImage();
                m_bRequiresRedraw = false;
            }

            if (!printing)
            {
                g.DrawImageUnscaled(m_pageImage, Point.Empty);
            }
            else
            {
                //ImageRenderer renderer = new ImageRenderer(m_recordCollection, m_resources, g, true);
                //renderer.RenderAsImage();
            }

            g.Restore(gs);
        }

        public void ZoomTo(float zoomFactor)
        {
            m_sx = m_sy = zoomFactor;
            m_bRequiresRedraw = true;

            this.Invalidate();
        }
        #endregion
    }
}
