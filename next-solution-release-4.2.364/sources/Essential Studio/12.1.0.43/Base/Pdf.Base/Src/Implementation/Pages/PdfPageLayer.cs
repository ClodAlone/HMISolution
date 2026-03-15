#region Copyright Syncfusion Inc. 2001 - 2014
// Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Any infringement will be prosecuted under
// applicable laws. 
#endregion

using System;
using System.Drawing;
using Syncfusion.Pdf.Graphics;
using Syncfusion.Pdf.IO;
using Syncfusion.Pdf.Primitives;
using System.Text;

namespace Syncfusion.Pdf
{
    /// <summary>
    /// Describes layer of the page.
    /// </summary>
    public class PdfPageLayer :
        IPdfWrapper
    {
        #region Fields
        /// <summary>
        /// Parent page of the layer.
        /// </summary>
        private PdfPageBase m_page;
        /// <summary>
        /// Graphics context of the layer.
        /// </summary>
        private PdfGraphics m_graphics;
        /// <summary>
        /// Content of the object.
        /// </summary>
        private PdfStream m_content;
        /// <summary>
        /// Graphics state of the Graphics.
        /// </summary>
        private PdfGraphicsState m_graphicsState;
        /// <summary>
        /// Indicates whether the layer should clip page template dimensions or not.
        /// </summary>
        private bool m_clipPageTemplates;
        /// <summary>
        /// Indicates if the graphics stream was saved.
        /// </summary>
        private bool m_bSaved;
        /// <summary>
        /// Local Variable to store the colorspace of the document.
        /// </summary>
        private PdfColorSpace m_colorspace = PdfColorSpace.RGB;
        /// <summary>
        /// Local Variable to store the layer id
        /// </summary>
        private string m_layerid;
        /// <summary>
        /// Local Variable to store the name
        /// </summary>
        private string m_name;
        /// <summary>
        /// Local Variable to set visiblity
        /// </summary>
        private bool m_visible = true;
        /// <summary>
        /// Collection of the layers of the page.
        /// </summary>
        private PdfPageLayerCollection m_layer;
        /// <summary>
        /// Indicates if Sublayer is present.
        /// </summary>
        internal bool m_sublayer = false;

        /// <summary>
        /// Local variable to store length of the graphics.
        /// </summary>
        internal long m_contentLength = 0;

        #endregion

        #region Properties
        /// <summary>
        /// Get or set the Colorspace.
        /// </summary>
        internal PdfColorSpace Colorspace
        {
            get
            {
                return m_colorspace;
            }
            set
            {
                m_colorspace = value;
            }
        }

        /// <summary>
        /// Gets parent page of the layer.
        /// </summary>
        public PdfPageBase Page
        {
            get
            {
                return m_page;
            }
        }

        internal string LayerId
        {
            get
            {
                return m_layerid;
            }
            set
            {
                this.m_layerid = value;
            }
        }

        internal String Name
        {
            get
            {
                return m_name;
            }
            set
            {
                this.m_name = value;
            }
        }

        internal bool Visible
        {
            get
            {
                return m_visible;
            }
            set
            {
                this.m_visible = value;
            }
        }
        /// <summary>
        /// Gets Graphics context of the layer.
        /// </summary>
        public PdfGraphics Graphics
        {
            get
            {
                if (m_graphics == null || m_bSaved)
                {
                    InitializeGraphics(Page);
                    //if ((this.Page is PdfPage) == true)
                    //{
                    //    if (((this.Page as PdfPage).Document) != null)
                    //    {
                    //        m_graphics.ColorSpace = (this.Page as PdfPage).Document.ColorSpace;
                    //    }
                    //}
                }
                //m_graphics.ColorSpace = (this.Page as PdfPage).Document.ColorSpace;
                return m_graphics;
            }
        }

        #endregion

        #region Constructors

        /// <summary>
        /// Adds a new layer.
        /// </summary>
        /// <returns></returns>
        public PdfPageLayer Add()
        {
            PdfPageLayer layer = new PdfPageLayer(m_page);
            layer.Name = string.Empty;
          
            return layer;
        }

        /// <summary>
        /// Gets the layer collection.
        /// </summary>
        public PdfPageLayerCollection Layers
        {
            get
            {
                if (m_layer == null)
                {
                    m_layer = new PdfPageLayerCollection(this.Page);
                }
                m_layer.m_sublayer  = true;
                return m_layer;
            }
        }
        /// <summary>
        /// Creates new layer.
        /// </summary>
        /// <param name="page">Parent page of the layer.</param>
        public PdfPageLayer(PdfPageBase page)
        {
            if (page == null)
                throw new ArgumentNullException("page");

            m_page = page;
            m_clipPageTemplates = true;
            m_content = new PdfStream();
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="T:PdfPageLayer"/> class.
        /// </summary>
        /// <param name="page">The page.</param>
        /// <param name="stream">The stream.</param>
        internal PdfPageLayer(PdfPageBase page, PdfStream stream)
        {
            if (page == null)
                throw new ArgumentNullException("page");

            if (stream == null)
                throw new ArgumentNullException("stream");

            m_page = page;
            m_content = stream;
        }

        /// <summary>
        /// Creates a new layer.
        /// </summary>
        /// <param name="page">Parent page of the layer.</param>
        /// <param name="clipPageTemplates">Indicates whether the layer should clip page template dimensions or not.</param>
        internal PdfPageLayer(PdfPageBase page, bool clipPageTemplates)
            : this(page)
        {
            m_clipPageTemplates = clipPageTemplates;
        }
        #endregion

        #region Implementation
        /// <summary>
        /// Initializes Graphics context of the layer.
        /// </summary>
        /// <param name="page">The page.</param>
        private void InitializeGraphics(PdfPageBase page)
        {
            PdfPage oPage = page as PdfPage;

            if (m_graphics == null)
            {
                PdfGraphics.GetResources gr = new PdfGraphics.GetResources(Page.GetResources);

                //  Normalize the Co-Ordinate values.
                PdfArray mBox = page.Dictionary.GetValue(DictionaryProperties.MediaBox, DictionaryProperties.Parent) as PdfArray;

                // Lower Left X co-ordinate Value.
                float llx = (mBox[0] as PdfNumber).FloatValue;
                // Lower Left Y co-ordinate value.
                float lly = (mBox[1] as PdfNumber).FloatValue;
                // Upper right X co-ordinate value.
                float urx = (mBox[2] as PdfNumber).FloatValue;
                // Upper right Y co-ordinate value.
                float ury = (mBox[3] as PdfNumber).FloatValue;

                if ((llx < 0 || lly < 0 || urx < 0 || ury < 0) && (Math.Floor(Math.Abs(lly)) == Math.Floor(Math.Abs(page.Size.Height)) && Math.Floor(Math.Abs(urx)) == Math.Floor(page.Size.Width)))
                {
                    // Normalize the co-ordinates
                    RectangleF temp = new RectangleF(Math.Min(llx, urx), Math.Min(lly, ury), Math.Max(llx, urx), Math.Max(lly, ury));
                    m_graphics = new PdfGraphics(new SizeF(temp.Width, temp.Height), gr, m_content);
                }
                else
                {
                    m_graphics = new PdfGraphics(page.Size, gr, m_content);
                }

                //PdfArray contents = page.Contents as PdfArray;
                //if (contents.Count > 0)
                //{
                //    PdfStream contentStream = new PdfStream();

                //    contentStream.Write(Operators.SaveState);
                //    contentStream.Write(Operators.NewLine);

                  

                //    if (contentStream != null)
                //    {
                //        contents.Insert(0, new PdfReferenceHolder(contentStream));
                //        contents.MarkChanged();
                //    }
                //    m_graphics.m_bStateSaved = true;
                //}

                if (oPage != null)
                {
                    PdfSectionCollection sc = oPage.Section.Parent;

                    if (sc != null)
                    {
                        m_graphics.ColorSpace = sc.Document.ColorSpace;
                        Colorspace = sc.Document.ColorSpace;
                    }
                }

                m_content.BeginSave += new SavePdfPrimitiveEventHandler(BeginSaveContent);               
            }

            // First Save of the graphics state.
            m_graphicsState = m_graphics.Save();
            if (!string.IsNullOrEmpty(m_name))
            {
                
#if SILVERLIGHT || NETFX_CORE || WP
                byte[] data = Encoding.UTF8.GetBytes("/OC /" + this.LayerId + " BDC\n");
#else
                byte[] data = Encoding.ASCII.GetBytes("/OC /" + this.LayerId + " BDC\n");
#endif
                m_content.Write(data);
            }
            // Transform coordinates to the left/top and activate margins.
            if (page.Origin.X >= 0 && page.Origin.Y >= 0)
                m_graphics.InitializeCoordinates();
            else
                m_graphics.InitializeCoordinates(page);
            if (PdfGraphics.TransparencyObject)
            {
                m_graphics.SetTransparencyGroup(page);
            }
            if (page.Dictionary.ContainsKey(DictionaryProperties.Rotate))
            {
                PdfNumber rotation = page.Dictionary[DictionaryProperties.Rotate] as PdfNumber;
                if (rotation == null)
                    rotation = PdfCrossTable.Dereference(page.Dictionary[DictionaryProperties.Rotate]) as PdfNumber;
                if (rotation.FloatValue == 90)
                {
                    page.Graphics.TranslateTransform(0, page.Size.Height);
                    page.Graphics.RotateTransform(-90);
                    page.Graphics.m_clipBounds.Size = new SizeF(page.Size.Height, page.Size.Width);
                }
                else if (rotation.FloatValue == 180)
                {
                    page.Graphics.TranslateTransform(page.Size.Width, page.Size.Height);
                    page.Graphics.RotateTransform(-180);                   
                }
                else if (rotation.FloatValue == 270)
                {
                    page.Graphics.TranslateTransform(page.Size.Width, 0);
                    page.Graphics.RotateTransform(-270);
                    page.Graphics.m_clipBounds.Size=new SizeF(page.Size.Height, page.Size.Width);
                    
                }
            }

            if (oPage != null)
            {
                RectangleF clipRect = oPage.Section.GetActualBounds(oPage, true);
                PdfMargins margins = oPage.Section.PageSettings.Margins;

                if (m_clipPageTemplates)
                {
                    if (page.Origin.X >= 0 && page.Origin.Y >= 0)
                        m_graphics.ClipTranslateMargins(clipRect);
                }
                else
                {
                    m_graphics.ClipTranslateMargins(clipRect.X, clipRect.Y, margins.Left,
                        margins.Top, margins.Right, margins.Bottom);
                }
            }

            m_graphics.SetLayer(this);
            m_bSaved = false;
        }

        /// <summary>
        /// Clears PdfPageLayer.
        /// </summary>
        internal void Clear()
        {
            if (m_graphics != null)
                m_graphics.StreamWriter.Clear();
            if (m_content != null)
                m_content = null;
            if (m_graphics != null)
                m_graphics = null;
        }

        #endregion

        #region IPdfWrapper Members
        /// <summary>
        /// Gets the wrapped element.
        /// </summary>
        IPdfPrimitive IPdfWrapper.Element
        {
            get
            {
                return m_content;
            }
        }
        #endregion

        #region Event handlers
        /// <summary>
        /// Catches BeforeSave of the content event.
        /// </summary>
        /// <param name="sender">Sender of the event.</param>
        /// <param name="e">event arguments.</param>
        private void BeginSaveContent(object sender, SavePdfPrimitiveEventArgs e)
        {
            if (m_graphicsState != null)
            {
                Graphics.Restore(m_graphicsState);
                m_graphicsState = null;
            }

            m_bSaved = true;
        }
        #endregion
    }
}
