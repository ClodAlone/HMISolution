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
using System.Drawing;
using System.Drawing.Drawing2D;
using Syncfusion.Documentation;
using Syncfusion.Windows.Forms.Chart.PostScript;

namespace Syncfusion.Windows.Forms.Chart
{
    /// <summary>
    /// Provides the methods to draw into post script image.
    /// </summary>
    /// <internalonly/>
    [Syncfusion.Documentation.DocumentationExclude()]
    public class ToPostScript
    {
        #region Constructor
        /// <summary>
        /// Initializes a new instance of the <see cref="ToPostScript"/> class.
        /// </summary>
        public ToPostScript()
        {
        }
        #endregion

        #region Constants
        private const string Uri = "__Uri";
        private const string MethodName = "__MethodName";
        private const string MethodSignature = "__MethodSignature";
        private const string TypeName = "__TypeName";
        private const string Args = "__Args";
        private const string CallContext = "__CallContext";
        #endregion

        #region Members
        private PostScriptGraphics m_graph;
        private GraphProxy m_proxy;
        private Image m_buff;
        private PostScriptImage postScript;
		private bool m_editableText = false;
        #endregion

        #region Public methods
        /// <summary>
        /// Gets the real graphics.
        /// </summary>
        /// <param name="size">The size.</param>
        /// <returns>Return real graphics.</returns>
        public Graphics GetRealGraphics(Size size)
        {
            m_buff = new Bitmap(size.Width, size.Height);
            Graphics g = GraphProxy.Create(m_buff, out m_proxy);
            m_proxy.GraphInvoke += new GraphProxyEventHandler(CallBackMethod);
            postScript = new PostScriptImage(size.Width, size.Height);
            m_graph = postScript.GetGraphics();
            //// m_graph.SetGraph( g );
            return g;
        }

        /// <summary>
        /// Gets the post script image.
        /// </summary>
        /// <returns>Returns PostScriptImage object. </returns>
        public PostScriptImage GetPostScriptImage()
        {
            return postScript;
        }

        /// <summary>
        /// Saves the specified name.
        /// </summary>
        /// <param name="name">The name.</param>
        public void Save(string name)
        {
            postScript.Save(name);
        }
		/// <summary>
        /// Enable or Disable the editable text for chart.
        /// </summary>
        public bool EditableText
        {
            get
            {
                return m_editableText;
            }
            set
            {
                if (m_editableText != value)
                    m_editableText = value;
            }
        }
		#endregion
        #region Helper methods
        /// <summary>
        /// Calls the back method.
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
                case "DrawPolygon":
                    PRXDrawPolygon((object[])e.Properties[Args]);
                    break;
                case "FillPolygon":
                    PRXFillPolygon((object[])e.Properties[Args]);
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
                case "DrawBezier":
                    PRXDrawBezier((object[])e.Properties[Args]);
                    break;
                case "TranslateTransform":
                    PRXTranslateTransform((object[])e.Properties[Args]);
                    break;
                case "Clear":
                    PRXClear((object[])e.Properties[Args]);
                    break;
                case "DrawString":
                    PRXDrawString((object[])e.Properties[Args]);
                    break;
                case "IntersectClip":
                    PRXIntersectClip((object[])e.Properties[Args]);
                    break;
                case "SetClip":
                    PRXSetClip((object[])e.Properties[Args]);
                    break;
                case "ResetClip":
                    PRXResetClip();
                    break;
                case "set_Transform":
                    PRXset_Transform((object[])e.Properties[Args]);
                    break;
                case "BeginContainer":
                    PRXBeginContainer();
                    break;
                case "EndContainer":
                    PRXEndContainer();
                    break;
                case "RotateTransform":
                    PRXRotateTransform((object[])e.Properties[Args]);
                    break;

            }
        }

        /// <summary>
        /// PRXs the draw path.
        /// </summary>
        /// <param name="prms">The PRMS.</param>
        private void PRXDrawPath(object[] prms)
        {
            m_graph.DrawPath((Pen)prms[0], (GraphicsPath)prms[1]);
        }
        /// <summary>
        /// PRXs the rotate transform.
        /// </summary>
        /// <param name="prms">The PRMS.</param>
        private void PRXRotateTransform(object[] prms)
        {
            m_graph.RotateTransform((Single)prms[0]);
        }
        /// <summary>
        /// PRXs the fill path.
        /// </summary>
        /// <param name="prms">The PRMS.</param>
        private void PRXFillPath(object[] prms)
        {
            m_graph.FillPath((Brush)prms[0], (GraphicsPath)prms[1]);
        }

        /// <summary>
        /// PRXs the draw polygon.
        /// </summary>
        /// <param name="prms">The PRMS.</param>
        private void PRXDrawPolygon(object[] prms)
        {
            m_graph.DrawPolygon((Pen)prms[0], (PointF[])prms[1]);
        }

        /// <summary>
        /// PRXs the fill polygon.
        /// </summary>
        /// <param name="prms">The PRMS.</param>
        private void PRXFillPolygon(object[] prms)
        {
            m_graph.FillPolygon((Brush)prms[0], (PointF[])prms[1]);
        }

        /// <summary>
        /// PRXs the draw rectangle.
        /// </summary>
        /// <param name="prms">The PRMS.</param>
        private void PRXDrawRectangle(object[] prms)
        {
            if ((prms[1] is float) || (prms[1] is int))
            {
                float x = (float)prms[1];
                float y = (float)prms[2];
                float w = (float)prms[3];
                float h = (float)prms[4];

                m_graph.DrawRectangle((Pen)prms[0], x, y, w, h);
            }
            else
            {
                m_graph.DrawRectangle((Pen)prms[0], (Rectangle)prms[1]);
            }
        }

        /// <summary>
        /// PRXs the fill rectangle.
        /// </summary>
        /// <param name="prms">The PRMS.</param>
        private void PRXFillRectangle(object[] prms)
        {
            if ((prms[1] is float) || (prms[1] is int))
            {
                float x, y, w, h;
                if ((prms[1] is float))
                {
                    x = (float)prms[1];
                    y = (float)prms[2];
                    w = (float)prms[3];
                    h = (float)prms[4];
                }
                else
                {
                    x = (int)prms[1];
                    y = (int)prms[2];
                    w = (int)prms[3];
                    h = (int)prms[4];
                }

                m_graph.FillRectangle((Brush)prms[0], x, y, w, h);
            }
            else if (prms[1] is Rectangle)
            {
                m_graph.FillRectangle((Brush)prms[0], (Rectangle)prms[1]);
            }
            else
            {
                m_graph.FillRectangle((Brush)prms[0], (RectangleF)prms[1]);
            }
        }

        /// <summary>
        /// PRXs the translate transform.
        /// </summary>
        /// <param name="prms">The PRMS.</param>
        private void PRXTranslateTransform(object[] prms)
        {
            m_graph.TranslateTransform((Single)prms[0], (Single)prms[1]);
        }

        /// <summary>
        /// PRXs the draw line.
        /// </summary>
        /// <param name="prms">The PRMS.</param>
        private void PRXDrawLine(object[] prms)
        {
            float x1 = 0, y1 = 0, x2 = 0, y2 = 0;

            if (prms[1] is Point)
            {
                x1 = ((Point)prms[1]).X;
                y1 = ((Point)prms[1]).Y;
                x2 = ((Point)prms[2]).X;
                y2 = ((Point)prms[2]).Y;
            }

            if (prms[1] is PointF)
            {
                x1 = ((PointF)prms[1]).X;
                y1 = ((PointF)prms[1]).Y;
                x2 = ((PointF)prms[2]).X;
                y2 = ((PointF)prms[2]).Y;
            }

            if ((prms[1] is float) || (prms[1] is int))
            {
                x1 = (float)prms[1];
                y1 = (float)prms[2];
                x2 = (float)prms[3];
                y2 = (float)prms[4];
            }

            m_graph.DrawLine((Pen)prms[0], x1, y1, x2, y2);
        }

        /// <summary>
        /// PRXs the draw bezier.
        /// </summary>
        /// <param name="prms">The PRMS.</param>
        private void PRXDrawBezier(object[] prms)
        {
            float x1 = 0, y1 = 0, x2 = 0, y2 = 0, x3 = 0, y3 = 0, x4 = 0, y4 = 0;

            if (prms[1] is Point)
            {
                x1 = ((Point)prms[1]).X;
                y1 = ((Point)prms[1]).Y;
                x2 = ((Point)prms[2]).X;
                y2 = ((Point)prms[2]).Y;
                x3 = ((Point)prms[3]).X;
                y3 = ((Point)prms[3]).Y;
                x4 = ((Point)prms[4]).X;
                y4 = ((Point)prms[4]).Y;
            }

            if (prms[1] is PointF)
            {
                x1 = ((PointF)prms[1]).X;
                y1 = ((PointF)prms[1]).Y;
                x2 = ((PointF)prms[2]).X;
                y2 = ((PointF)prms[2]).Y;
                x3 = ((PointF)prms[3]).X;
                y3 = ((PointF)prms[3]).Y;
                x4 = ((PointF)prms[4]).X;
                y4 = ((PointF)prms[4]).Y;
            }

            if ((prms[1] is float) || (prms[1] is int))
            {
                x1 = (float)prms[1];
                y1 = (float)prms[2];
                x2 = (float)prms[3];
                y2 = (float)prms[4];
                x3 = (float)prms[5];
                y3 = (float)prms[6];
                x4 = (float)prms[7];
                y4 = (float)prms[8];
            }

            m_graph.DrawBezier((Pen)prms[0], x1, y1, x2, y2, x3, y3, x4, y4);
        }

        /// <summary>
        /// PRXs the clear.
        /// </summary>
        /// <param name="prms">The PRMS.</param>
        private void PRXClear(object[] prms)
        {
            m_graph.Clear((Color)prms[0]);
        }

        /// <summary>
        /// PRXs the draw string.
        /// </summary>
        /// <param name="prms">The PRMS.</param>
        private void PRXDrawString(object[] prms)
        {
            float x = 0, y = 0,width=0,height=0;
			StringFormat stringformat = new StringFormat();
			
            if (prms[3] is RectangleF)
            {
                x = ((RectangleF)prms[3]).X;
                y = ((RectangleF)prms[3]).Y;
                width = ((RectangleF)prms[3]).Width;
                height = ((RectangleF)prms[3]).Height;
                if(prms.Length > 4)
				stringformat = ((StringFormat)prms[4]);
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

            m_graph.DrawString((string)prms[0], (Font)prms[1], (Brush)prms[2], x, y, width, height,stringformat,this.EditableText);
        }

        /// <summary>
        /// PRXs the set clip.
        /// </summary>
        /// <param name="prms">The PRMS.</param>
        private void PRXSetClip(object[] prms)
        {
            RectangleF rect;

            if (prms[0] is RectangleF)
            {
                rect = new RectangleF(((RectangleF)prms[0]).X, ((RectangleF)prms[0]).Y, ((RectangleF)prms[0]).Width, ((RectangleF)prms[0]).Height);

                m_graph.SetClip(rect);
            }

            if (prms[0] is Rectangle)
            {
                rect = new RectangleF(((Rectangle)prms[0]).X, ((Rectangle)prms[0]).Y, ((Rectangle)prms[0]).Width, ((Rectangle)prms[0]).Height);

                m_graph.SetClip(rect);
            }
        }

        /// <summary>
        /// PRXs the intersect clip.
        /// </summary>
        /// <param name="prms">The PRMS.</param>
        private void PRXIntersectClip(object[] prms)
        {
            RectangleF rect;

            if (prms[0] is RectangleF)
            {
                rect = new RectangleF(((RectangleF)prms[0]).X, ((RectangleF)prms[0]).Y, ((RectangleF)prms[0]).Width, ((RectangleF)prms[0]).Height);

                m_graph.IntersectClip(rect);
            }

            if (prms[0] is Rectangle)
            {
                rect = new RectangleF(((Rectangle)prms[0]).X, ((Rectangle)prms[0]).Y, ((Rectangle)prms[0]).Width, ((Rectangle)prms[0]).Height);

                m_graph.IntersectClip(rect);
            }
        }

        /// <summary>
        /// PRXs the reset clip.
        /// </summary>
        private void PRXResetClip()
        {
            m_graph.ResetClip();
        }

        /// <summary>
        /// PRXs the begin container.
        /// </summary>
        private void PRXBeginContainer()
        {
            m_graph.BeginContainer();
        }

        /// <summary>
        /// PRXs the end container.
        /// </summary>
        private void PRXEndContainer()
        {
           m_graph.EndContainer();
        }

        /// <summary>
        /// PRs the xset_ transform.
        /// </summary>
        /// <param name="prms">The PRMS.</param>
        private void PRXset_Transform(object[] prms)
        {
            if (prms[0] is Matrix)
            {
                m_graph.SetTransform(((Matrix)prms[0]));
            }
        }
        #endregion
    }
}
