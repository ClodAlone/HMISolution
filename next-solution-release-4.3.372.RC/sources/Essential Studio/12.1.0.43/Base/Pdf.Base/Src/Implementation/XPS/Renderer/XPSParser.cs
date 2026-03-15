#region Copyright Syncfusion Inc. 2001 - 2014
// Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Any infringement will be prosecuted under
// applicable laws. 
#endregion
using System;
using System.Collections.Generic;
using System.Text;
using Syncfusion.Pdf;
using Syncfusion.XPS;
using Syncfusion.Pdf.Graphics;
using System.Drawing;
using System.Diagnostics;
using System.Drawing.Drawing2D;

namespace Syncfusion.XPS
{
    /// <summary>
    /// Represents the XPS Parser
    /// </summary>
    internal class XPSParser : IDisposable
    {
        #region Fields
        /// <summary>
        /// Represents the currently parsed Fixed Page
        /// </summary>
        private FixedPage m_page;
        /// <summary>
        /// Represents the XPS renderer object
        /// </summary>
        private XPSRenderer m_renderer;
        /// <summary>
        /// Represents the XPS enumerator delegate
        /// </summary>
        private EnumerateXPSFileProc m_enumerateHandler;
        /// <summary>
        /// Represents the PdfUnitconvertor
        /// </summary>
        private PdfUnitConvertor m_unitConvertor;
        #endregion

        #region Events
        /// <summary>
        /// Represents the XPS enumerator
        /// </summary>
        private delegate void EnumerateXPSFileProc();
        #endregion

        #region Properties
        /// <summary>
        /// Gets the XPS renderer
        /// </summary>
        internal XPSRenderer Renderer
        {
            get
            {
                return m_renderer;
            }
        }
        #endregion

        #region Constructors
        /// <summary>
        /// Creates an instance of the XPS parser class.
        /// </summary>
        /// <param name="page">Fixed page of the XPS</param>
        /// <param name="renderer">The XPS renderer</param>
        public XPSParser(FixedPage page, XPSRenderer renderer)
        {
            m_page = page;
            m_renderer = renderer;
            m_unitConvertor = new PdfUnitConvertor();
        }
        #endregion

        #region Implementation
        /// <summary>
        /// Enumerates the XPS file
        /// </summary>
        public void Enumerate()
        {
            if (m_enumerateHandler == null)
                m_enumerateHandler = new EnumerateXPSFileProc(EnumerateXPSPage);

            m_enumerateHandler.Invoke();
        }

        /// <summary>
        /// Reads the XPS canvas
        /// </summary>
        /// <param name="canvas">XPS canvas</param>
        private void ReadCanvas(Canvas canvas)
        {
            PdfGraphicsState gs = Renderer.Graphics.Save();
            Renderer.DrawCanvas(canvas);
            Renderer.Graphics.Restore(gs);
        }

        /// <summary>
        /// Reads the XPS glyphs
        /// </summary>
        /// <param name="glyphs">XLS Glyphs</param>
        private void ReadGlyphs(Glyphs glyphs)
        {
            Renderer.DrawGlyphs(glyphs);
        }

        /// <summary>
        /// Reads the XPS path
        /// </summary>
        /// <param name="path">XPS path</param>
        private void ReadPath(Path path)
        {
            Renderer.DrawPath(path);
        }
        #endregion

        #region Helper Methods
        /// <summary>
        /// Enumerates the XPS fixed page objects
        /// </summary>
        internal void EnumerateXPSPage()
        {
            if (m_page.Items != null)
            {
                foreach (object item in m_page.Items)
                {
                    if (item is Canvas)
                        ReadCanvas((Canvas)item);
                    else if (item is Glyphs)
                        ReadGlyphs((Glyphs)item);
                    else if (item is Path)
                        ReadPath((Path)item);
                    else
                    {
#if DEBUG
                        throw new NotImplementedException(item.GetType().ToString());
#endif
                    }
                }
            }
        }

        /// <summary>
        /// Converts the pixel value to points
        /// </summary>
        /// <param name="value">pixel value</param>
        /// <returns>value in point</returns>
        private float PixelsToPoints(double value)
        {
            return m_unitConvertor.ConvertFromPixels((float)value, PdfGraphicsUnit.Point);
        }
        #endregion

        #region IDisposable Members

        public void Dispose()
        {
            //throw new NotImplementedException();
        }

        #endregion
    }
}
