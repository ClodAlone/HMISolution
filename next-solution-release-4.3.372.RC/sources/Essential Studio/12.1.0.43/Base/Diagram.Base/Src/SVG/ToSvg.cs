#region Copyright Syncfusion Inc. 2001 - 2014
// Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
//
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Re-distribution in any form is strictly
// prohibited. Any infringement will be prosecuted under applicable laws. 
#endregion

using System.Collections;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.IO;
using System.Text;
using System.Xml;

namespace Syncfusion.SVG.IO
{
    /// <summary>
    /// Convert the diagram to SVG format.
    /// </summary>
    public class ToSvg
    {
        #region Constants
        private const string Uri = "__Uri";
        private const string MethodName = "__MethodName";
        private const string MethodSignature = "__MethodSignature";
        private const string TypeName = "__TypeName";
        private const string Args = "__Args";
        private const string CallContext = "__CallContext";
        #endregion

        #region Members
        private SvgGraph m_svgGraphics;
        private Graphics m_graphics;
        private GraphProxy m_proxy;
        private Image m_buff;
        #endregion

        #region Proprties
        /// <summary>
        /// Gets the buffer.
        /// </summary>
        /// <value>The buffer.</value>
        public Image Buffer
        {
            get
            {
                return m_buff;
            }
        }
        #endregion

        #region Constructor
        /// <summary>
        /// Initializes a new instance of the <see cref="ToSvg"/> class.
        /// </summary>
        public ToSvg()
        {
        }
        #endregion

        #region Public methods
        /// <summary>
        /// Gets the real graphics.
        /// </summary>
        /// <param name="size">The size.</param>
        /// <returns>The graphics.</returns>
        public Graphics GetRealGraphics(Size size)
        {
            m_buff = new Bitmap(size.Width, size.Height);

            m_graphics = GraphProxy.Create(m_buff, out m_proxy);
            m_proxy.GraphInvoke += new GraphProxyEventHandler(CallBackMethod);

            m_svgGraphics = new SvgGraph();

            return m_graphics;
        }

        /// <summary>
        /// Saves the specified name.
        /// </summary>
        /// <param name="name">The name.</param>
        public void Save(string name)
        {
            m_svgGraphics.SaveDocument(name);
        }
        #endregion

        #region Helper methods
        /// <summary>
        /// Calls the back method.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The <see cref="Syncfusion.SVG.IO.GraphProxyEventArgs"/> instance containing the event data.</param>
        private void CallBackMethod(object sender, GraphProxyEventArgs e)
        {
            string name = (string)e.Properties[MethodName];

            switch (name)
            {
                case "DrawPath":
                    PRXDrawPath((object[])e.Properties[Args]);
                    break;
                case "FillPath":
                    PRXFillPath((object[])e.Properties[Args]);
                    break;
                case "DrawRectangle":
                    PRXDrawRectangle((object[])e.Properties[Args]);
                    break;
                case "FillRectangle":
                    PRXFillRectangle((object[])e.Properties[Args]);
                    break;
                case "DrawLine":
                    PRXDrawLine((object[])e.Properties[Args]);
                    break;
                case "Clear":
                    PRXClear((object[])e.Properties[Args]);
                    break;
                case "DrawString":
                    PRXDrawString((object[])e.Properties[Args]);
                    break;
                case "DrawEllipse":
                    PRXDrawEllipse((object[])e.Properties[Args]);
                    break;
                case "FillEllipse":
                    PRXFillEllipse((object[])e.Properties[Args]);
                    break;
                case "DrawPolygon":
                    PRXDrawPolygon((object[])e.Properties[Args]);
                    break;
                case "FillPolygon":
                    PRXFillPolygon((object[])e.Properties[Args]);
                    break;
                case "DrawImage":
                    PRXDrawImage((object[])e.Properties[Args]);
                    break;
                case "SetClip":
                    PRXSetClip((object[])e.Properties[Args]);
                    break;
                case "ResetClip":
                    PRXResetClip((object[])e.Properties[Args]);
                    break;
            }
        }

        /// <summary>
        /// The draw path.
        /// </summary>
        /// <param name="prms">The objects.</param>
        private void PRXDrawPath(object[] prms)
        {
            Pen pn = prms[0] as Pen;
            GraphicsPath gp = prms[1] as GraphicsPath;

            m_svgGraphics.Transform = m_graphics.Transform;
            m_svgGraphics.DrawPath(pn, gp);
        }

        /// <summary>
        /// The fill path.
        /// </summary>
        /// <param name="prms">The objects.</param>
        private void PRXFillPath(object[] prms)
        {
            Brush br = prms[0] as Brush;
            GraphicsPath gp = prms[1] as GraphicsPath;

            m_svgGraphics.Transform = m_graphics.Transform;
            m_svgGraphics.FillPath(br, gp);
        }

        private void PRXDrawRectangle(object[] prms)
        {
            Pen pn = (Pen)prms[0];
            float x = 0, y = 0, w = 0, h = 0;

            if (prms[1] is Rectangle)
            {
                Rectangle rect = (Rectangle)prms[1];

                x = rect.X;
                y = rect.Y;
                w = rect.Width;
                h = rect.Height;
            }
            else
            {
                x = (float)prms[1];
                y = (float)prms[2];
                w = (float)prms[3];
                h = (float)prms[4];
            }

            m_svgGraphics.Transform = m_graphics.Transform;
            m_svgGraphics.DrawRectangle(pn, x, y, w, h);
        }
        
        private void PRXFillRectangle(object[] prms)
        {
            Brush br = (Brush)prms[0];
            float x = 0, y = 0, w = 0, h = 0;

            if (prms[1] is Rectangle)
            {
                Rectangle rect = (Rectangle)prms[1];

                x = rect.X;
                y = rect.Y;
                w = rect.Width;
                h = rect.Height;
            }
            else if (prms[1] is RectangleF)
            {
                RectangleF rect = (RectangleF)prms[1];

                x = rect.X;
                y = rect.Y;
                w = rect.Width;
                h = rect.Height;
            }
            else
            {
                x = (float)prms[1];
                y = (float)prms[2];
                w = (float)prms[3];
                h = (float)prms[4];
            }

            m_svgGraphics.Transform = m_graphics.Transform;
            m_svgGraphics.FillRectangle(br, x, y, w, h);
        }
        
        private void PRXDrawLine(object[] prms)
        {
            Pen pn = prms[0] as Pen;
            float x1 = 0, x2 = 0, y1 = 0, y2 = 0;

            if (prms[1] is Point)
            {
                x1 = ((Point)prms[1]).X;
                y1 = ((Point)prms[1]).Y;
                x2 = ((Point)prms[2]).X;
                y2 = ((Point)prms[2]).Y;
            }
            else if (prms[1] is PointF)
            {
                x1 = ((PointF)prms[1]).X;
                y1 = ((PointF)prms[1]).Y;
                x2 = ((PointF)prms[2]).X;
                y2 = ((PointF)prms[2]).Y;
            }
            else
            {
                x1 = (float)prms[1];
                y1 = (float)prms[2];
                x2 = (float)prms[3];
                y2 = (float)prms[4];
            }

            m_svgGraphics.Transform = m_graphics.Transform;
            m_svgGraphics.DrawLine(pn, x1, y1, x2, y2);
        }
        
        private void PRXClear(object[] prms)
        {
            m_svgGraphics.Clear((Color)prms[0]);
        }
        
        private void PRXDrawString(object[] prms)
        {
            float x = 0, y = 0;
            string text = (string)prms[0];
            Font font = (Font)prms[1];
            Brush br = (Brush)prms[2];

            if (prms[3] is RectangleF)
            {
                x = ((RectangleF)prms[3]).X;
                y = ((RectangleF)prms[3]).Y;
            }
            if (prms[3] is PointF)
            {
                x = ((PointF)prms[3]).X;
                y = ((PointF)prms[3]).Y;
            }
            if (prms[3] is float)
            {
                x = (float)prms[3];
                y = (float)prms[4];
            }

            m_svgGraphics.Transform = m_graphics.Transform;
            m_svgGraphics.DrawString(text, font, br, x, y);
        }
        
        private void PRXDrawEllipse(object[] prms)
        {
            Pen pn = (Pen)prms[0];
            float x = 0, y = 0, w = 0, h = 0;

            if (prms[1] is Rectangle)
            {
                Rectangle rect = (Rectangle)prms[1];

                x = rect.X;
                y = rect.Y;
                w = rect.Width;
                h = rect.Height;
            }
            else if (prms[1] is RectangleF)
            {
                RectangleF rect = (RectangleF)prms[1];

                x = rect.X;
                y = rect.Y;
                w = rect.Width;
                h = rect.Height;
            }
            else
            {
                x = (float)prms[1];
                y = (float)prms[2];
                w = (float)prms[3];
                h = (float)prms[4];
            }

            m_svgGraphics.Transform = m_graphics.Transform;
            m_svgGraphics.DrawEllipse(pn, x, y, w, h);
        }
        
        private void PRXFillEllipse(object[] prms)
        {
            Brush br = (Brush)prms[0];
            float x = 0, y = 0, w = 0, h = 0;

            if (prms[1] is Rectangle)
            {
                Rectangle rect = (Rectangle)prms[1];

                x = rect.X;
                y = rect.Y;
                w = rect.Width;
                h = rect.Height;
            }
            else
            {
                x = (float)prms[1];
                y = (float)prms[2];
                w = (float)prms[3];
                h = (float)prms[4];
            }

            m_svgGraphics.Transform = m_graphics.Transform;
            m_svgGraphics.FillEllipse(br, x, y, w, h);
        }
        private void PRXDrawPolygon(object[] prms)
        {
            Pen pen = (Pen)prms[0];
            PointF[] points;

            if (prms[1] is Point)
            {
                Point[] ps = (Point[])prms[1];
                points = new PointF[ps.Length];

                for (int i = 0; i < ps.Length; i++)
                {
                    points[i] = new PointF(ps[i].X, ps[i].Y);
                }
            }
            else
            {
                points = (PointF[])prms[1];
            }

            m_svgGraphics.Transform = m_graphics.Transform;
            m_svgGraphics.DrawPolygon(pen, points);
        }
        
        private void PRXFillPolygon(object[] prms)
        {
            PointF[] points;
            Brush br = (Brush)prms[0];

            if (prms[1] is Point)
            {
                Point[] ps = (Point[])prms[1];
                points = new PointF[ps.Length];

                for (int i = 0; i < ps.Length; i++)
                {
                    points[i] = new PointF(ps[i].X, ps[i].Y);
                }
            }
            else
            {
                points = (PointF[])prms[1];
            }

            m_svgGraphics.Transform = m_graphics.Transform;
            m_svgGraphics.FillPolygon(br, points);
        }
        
        private void PRXDrawImage(object[] prms)
        {
            Image img = (Image)prms[0];
            RectangleF rect = RectangleF.Empty;

            if (prms[1] is Point)
            {
                rect = new RectangleF(((Point)prms[1]).X, ((Point)prms[1]).Y, img.Width, img.Height);
            }
            else if (prms[1] is PointF)
            {
                rect = new RectangleF(((PointF)prms[1]).X, ((PointF)prms[1]).Y, img.Width, img.Height);
            }
            else if (prms[1] is float)
            {
                rect = new RectangleF((float)prms[1], (float)prms[2], img.Width, img.Height);
            }
            else if (prms[1] is int)
            {
                float x = (int)prms[1];
                float y = (int)prms[2];

                rect = new RectangleF(x, y, img.Width, img.Height);
            }

            m_svgGraphics.Transform = m_graphics.Transform;
            m_svgGraphics.DrawImage(img, rect.X, rect.Y, rect.Width, rect.Height);
        }
        
        private void PRXSetClip(object[] prms)
        {
            if (prms[0] is Rectangle)
            {
                Rectangle rc = (Rectangle)prms[0];
                GraphicsPath gp = new GraphicsPath();

                gp.AddRectangle(rc);
                m_svgGraphics.SetClip(gp);
            }
            else if (prms[0] is RectangleF)
            {
                RectangleF rc = (RectangleF)prms[0];
                GraphicsPath gp = new GraphicsPath();

                gp.AddRectangle(rc);
                m_svgGraphics.SetClip(gp);
            }
            else if (prms[0] is GraphicsPath)
            {
                m_svgGraphics.SetClip((GraphicsPath)prms[0]);
            }
        }
        
        private void PRXResetClip(object[] prms)
        {
            m_svgGraphics.ResetClip();
        }
        #endregion
    }
}
