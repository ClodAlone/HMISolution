#region Copyright Syncfusion Inc. 2001 - 2014
//
//  Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
//
//  Use of this code is subject to the terms of our license.
//  A copy of the current license can be obtained at any time by e-mailing
//  licensing@syncfusion.com. Re-distribution in any form is strictly
//  prohibited. Any infringement will be prosecuted under applicable laws. 
//
#endregion

using System;
using System.Collections;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.IO;
using System.Text;
using System.Xml;
using Syncfusion.Windows.Forms.Chart.SvgBase;

namespace Syncfusion.Windows.Forms.Chart
{
    /// <summary>
    /// Provides the methods to draw into SVG image.
    /// </summary>
    /// <internalonly/>
    [Syncfusion.Documentation.DocumentationExclude()]
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

        #region Public methods
        /// <summary>
        /// Gets the <see cref="Graphics"/> for SVG by the specified size.
        /// </summary>
        /// <param name="size">The size.</param>
        /// <returns>Returns Graphics object.</returns>
        public Graphics GetRealGraphics(Size size)
        {
            m_buff = new Bitmap(size.Width, size.Height);

            m_graphics = GraphProxy.Create(m_buff, out m_proxy);
            m_proxy.GraphInvoke += new GraphProxyEventHandler(CallBackMethod);

            m_svgGraphics = new SvgGraph();

            return m_graphics;
        }

        /// <summary>
        /// Saves SVG document the specified name.
        /// </summary>
        /// <param name="name">The name.</param>
        public void Save(string name)
        {
            m_svgGraphics.SaveDocument(name);
        }
        #endregion

        #region Helper methods
        /// <summary>
        /// Handles the <see cref="Graphics"/> methods.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The <see cref="Syncfusion.Windows.Forms.Chart.GraphProxyEventArgs"/> instance containing the event data.</param>
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
        /// Draws <see cref="GraphicsPath"/>.
        /// </summary>
        /// <param name="prms">The array of parameters.</param>
        private void PRXDrawPath(object[] prms)
        {
            Pen pn = prms[0] as Pen;
            GraphicsPath gp = null;

            if (this.TryCastToGraphPath(prms, 1, out gp))
            {
                m_svgGraphics.Transform = m_graphics.Transform;
                m_svgGraphics.DrawPath(pn, gp);
            }
        }

        /// <summary>
        /// PRXs the fill path.
        /// </summary>
        /// <param name="prms">The array of parameters.</param>
        private void PRXFillPath(object[] prms)
        {
            Brush br = prms[0] as Brush;
            GraphicsPath gp = null;

            if (this.TryCastToGraphPath(prms, 1, out gp))
            {
                m_svgGraphics.Transform = m_graphics.Transform;
                m_svgGraphics.FillPath(br, gp);
            }
        }

        /// <summary>
        /// PRXs the draw rectangle.
        /// </summary>
        /// <param name="prms">The array of parameters.</param>
        private void PRXDrawRectangle(object[] prms)
        {
            Pen pn = (Pen)prms[0];
            RectangleF rect = RectangleF.Empty;

            if (this.TryCastToRect(prms, 1, out rect))
            {
                m_svgGraphics.Transform = m_graphics.Transform;
                m_svgGraphics.DrawRectangle(pn, rect.X, rect.Y, rect.Width, rect.Height);
            }
        }

        /// <summary>
        /// PRXs the fill rectangle.
        /// </summary>
        /// <param name="prms">The array of parameters.</param>
        private void PRXFillRectangle(object[] prms)
        {
            Brush br = (Brush)prms[0];
            RectangleF rect = RectangleF.Empty;

            if (this.TryCastToRect(prms, 1, out rect))
            {
                m_svgGraphics.Transform = m_graphics.Transform;
                m_svgGraphics.FillRectangle(br, rect.X, rect.Y, rect.Width, rect.Height);
            }
        }

        /// <summary>
        /// PRXs the draw line.
        /// </summary>
        /// <param name="prms">The array of parameters.</param>
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
            else if (prms[1] is float)
            {
                x1 = (float)prms[1];
                y1 = (float)prms[2];
                x2 = (float)prms[3];
                y2 = (float)prms[4];
            }
            else
            {
                x1 = (int)prms[1];
                y1 = (int)prms[2];
                x2 = (int)prms[3];
                y2 = (int)prms[4];
            }

            m_svgGraphics.Transform = m_graphics.Transform;
            m_svgGraphics.DrawLine(pn, x1, y1, x2, y2);
        }

        /// <summary>
        /// PRXs the clear.
        /// </summary>
        /// <param name="prms">The array of parameters.</param>
        private void PRXClear(object[] prms)
        {
            m_svgGraphics.Clear((Color)prms[0]);
        }

        /// <summary>
        /// PRXs the draw string.
        /// </summary>
        /// <param name="prms">The array of parameters.</param>
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
            else if (prms[3] is Rectangle)
            {
                x = ((Rectangle)prms[3]).X;
                y = ((Rectangle)prms[3]).Y;
            }
            else if (prms[3] is PointF)
            {
                x = ((PointF)prms[3]).X;
                y = ((PointF)prms[3]).Y;
            }
            else if (prms[3] is Point)
            {
                x = ((Point)prms[3]).X;
                y = ((Point)prms[4]).Y;
            }
            else if (prms[3] is float)
            {
                x = (float)prms[3];
                y = (float)prms[4];
            }
            else if (prms[3] is int)
            {
                x = (int)prms[3];
                y = (int)prms[4];
            }

            m_svgGraphics.Transform = m_graphics.Transform;
            m_svgGraphics.DrawString(text, font, br, x, y);
        }

        /// <summary>
        /// PRXs the draw ellipse.
        /// </summary>
        /// <param name="prms">The array of parameters.</param>
        private void PRXDrawEllipse(object[] prms)
        {
            Pen pn = (Pen)prms[0];
            RectangleF rect = RectangleF.Empty;

            if (this.TryCastToRect(prms, 1, out rect))
            {
                m_svgGraphics.Transform = m_graphics.Transform;
                m_svgGraphics.DrawEllipse(pn, rect.X, rect.Y, rect.Width, rect.Height);
            }
        }

        /// <summary>
        /// PRXs the fill ellipse.
        /// </summary>
        /// <param name="prms">The array of parameters.</param>
        private void PRXFillEllipse(object[] prms)
        {
            Brush br = (Brush)prms[0];
            RectangleF rect = RectangleF.Empty;

            if (this.TryCastToRect(prms, 1, out rect))
            {
                m_svgGraphics.Transform = m_graphics.Transform;
                m_svgGraphics.FillEllipse(br, rect.X, rect.Y, rect.Width, rect.Height);
            }
        }

        /// <summary>
        /// PRXs the draw polygon.
        /// </summary>
        /// <param name="prms">The array of parameters.</param>
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

        /// <summary>
        /// PRXs the fill polygon.
        /// </summary>
        /// <param name="prms">The array of parameters.</param>
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

        /// <summary>
        /// PRXs the draw image.
        /// </summary>
        /// <param name="prms">The array of parameters.</param>
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

        /// <summary>
        /// PRXs the set clip.
        /// </summary>
        /// <param name="prms">The array of parameters.</param>
        private void PRXSetClip(object[] prms)
        {
            if (prms[0] is Rectangle)
            {
                GraphicsPath gp = new GraphicsPath();
                Rectangle rc = (Rectangle)prms[0];

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

        /// <summary>
        /// Resets clip.
        /// </summary>
        /// <param name="prms">The array of parameters.</param>
        private void PRXResetClip(object[] prms)
        {
            m_svgGraphics.ResetClip();
        }

        /// <summary>
        /// Try cast parameters to <see cref="RectangleF"/>, <see cref="Rectangle"/>, <see cref="float"/> or <see cref="int"/> type.
        /// </summary>
        /// <param name="prms">The array of parameters.</param>
        /// <param name="offset">The parameter offset.</param>
        /// <param name="outRect">The output <see cref="RectangleF"/>.</param>
        /// <returns>Returns true if casting parameters to Rectangle is possible otherwise false. </returns>
        private bool TryCastToRect(object[] prms, int offset, out RectangleF outRect)
        {
            if (prms[offset] is Rectangle)
            {
                Rectangle rect = (Rectangle)prms[offset];
                outRect = new RectangleF(rect.X, rect.Y, rect.Width, rect.Height);
                return true;
            }
            else if (prms[offset] is RectangleF)
            {
                outRect = (RectangleF)prms[offset];
                return true;
            }
            else if (prms[offset] is float)
            {
                outRect = new RectangleF((float)prms[offset], (float)prms[offset + 1], (float)prms[offset + 2], (float)prms[offset + 3]);                  
                return true;
            }
            else if (prms[offset] is int)
            {
                outRect = new RectangleF((int)prms[offset], (int)prms[offset + 1], (int)prms[offset + 2], (int)prms[offset + 3]);                  
                return true;
            }

            outRect = RectangleF.Empty;

            return false;
        }

        /// <summary>
        /// Try cast parameters to <see cref="GraphicsPath"/> type.
        /// </summary>
        /// <param name="prms">The array of parameters.</param>
        /// <param name="offset">The offset.</param>
        /// <param name="gp">The output <see cref="GraphicsPath"/>.</param>
        /// <returns>Returns true if casting parameters to GraphicsPath is possible otherwise false.</returns>
        private bool TryCastToGraphPath(object[] prms, int offset, out GraphicsPath gp)
        {
            if (prms[offset] is GraphicsPath)
            {
                gp = (GraphicsPath)prms[offset];
                return true;
            }

            gp = null;

            return false;
        }
        #endregion
    }
}
