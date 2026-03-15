#region Copyright Syncfusion Inc. 2001 - 2014
// Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Any infringement will be prosecuted under
// applicable laws. 
#endregion

using System;
using System.Drawing;
using System.IO;
using Syncfusion.Pdf.Graphics;
using Syncfusion.Pdf.IO;
using Syncfusion.Pdf.Primitives;

namespace Syncfusion.Pdf.Graphics
{
    /// <summary>
    /// Represents Pdf Template object.
    /// </summary>
    public class PdfTemplate :
        PdfShapeElement,
        IPdfWrapper
    {
        #region Fields
        /// <summary>
        /// Graphics context of the template.
        /// </summary>
        private PdfGraphics m_graphics;

        /// <summary>
        /// Content of the object.
        /// </summary>
        internal PdfStream m_content;

        /// <summary>
        /// Resources of the template.
        /// </summary>
        private PdfResources m_resources;

        /// <summary>
        /// Size of the template.
        /// </summary>
        private SizeF m_size;

        /// <summary>
        /// Indicates if the template is read-only.
        /// </summary>
        private bool m_bIsReadonly;

        private bool m_writeTransformation = true;
        #endregion

        #region Constructors
        /// <summary>
        /// Initializes a new instance of the <see cref="PdfTemplate"/> class.
        /// </summary>
        /// <param name="size">The size.</param>
        public PdfTemplate(SizeF size)
            : this(size.Width, size.Height)
        {
        }

        internal PdfTemplate(SizeF size, bool writeTransformation)
            : this(size.Width, size.Height)
        {
            m_writeTransformation = writeTransformation;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="PdfTemplate"/> class.
        /// </summary>
        /// <param name="width">The width.</param>
        /// <param name="height">The height.</param>
        public PdfTemplate(float width, float height)
            : base()
        {
            m_content = new PdfStream();
            SetSize(new SizeF(width, height));

            Initialize();
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="PdfTemplate"/> class.
        /// </summary>
        /// <param name="origin">The origin points of the Pdf Page</param>
        /// <param name="size">The size of the new template.</param>
        /// <param name="stream">The data stream of the new template.</param>
        /// <param name="resources">The resources of the new template.</param>
        /// <remarks>The resulting template is read-only in order to avoid unexpected side effects
        /// caused by non-restored graphics state.</remarks>
        internal PdfTemplate(PointF origin, SizeF size, MemoryStream stream, PdfDictionary resources)
            : base()
        {
            if (size == SizeF.Empty)
            {
                throw new ArgumentException("The size of the new PdfTemplate can't be empty.");
            }

            if (stream == null)
            {
                throw new ArgumentNullException("stream");
            }

            m_content = new PdfStream();
            if (origin.X < 0 || origin.Y < 0)
                SetSize(origin, size);
            else
                SetSize(size);

            Initialize();

            stream.WriteTo(m_content.InternalStream);

            if (resources != null)
            {
                m_content[DictionaryProperties.Resources] = new PdfDictionary(resources);
                m_resources = new PdfResources(resources);
            }

            m_bIsReadonly = true;
        }


        /// <summary>
        /// Initializes a new instance of the <see cref="PdfTemplate"/> class.
        /// </summary>
        /// <param name="size">The size of the new template.</param>
        /// <param name="stream">The data stream of the new template.</param>
        /// <param name="resources">The resources of the new template.</param>
        /// <remarks>The resulting template is read-only in order to avoid unexpected side effects
        /// caused by non-restored graphics state.</remarks>
        internal PdfTemplate(SizeF size, MemoryStream stream, PdfDictionary resources)
            : base()
        {
            if (size == SizeF.Empty)
            {
                throw new ArgumentException("The size of the new PdfTemplate can't be empty.");
            }

            if (stream == null)
            {
                throw new ArgumentNullException("stream");
            }

            m_content = new PdfStream();
            SetSize(size);
            Initialize();

            stream.WriteTo(m_content.InternalStream);

            if (resources != null)
            {
                m_content[DictionaryProperties.Resources] = new PdfDictionary(resources);
                m_resources = new PdfResources(resources);
            }

            m_bIsReadonly = true;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="PdfTemplate"/> class.
        /// </summary>
        /// <param name="template">The template.</param>
        internal PdfTemplate(PdfStream template)
        {
            if (template == null)
            {
                throw new ArgumentNullException("template");
            }

            m_content = template;

            IPdfPrimitive obj = PdfCrossTable.Dereference(m_content[DictionaryProperties.BBox]);
            RectangleF rect = (obj as PdfArray).ToRectangle();

            m_size = rect.Size;

            m_bIsReadonly = true;
        }
        #endregion

        #region Properties
        /// <summary>
        /// Gets graphics context of the template.
        /// </summary>
        /// <remarks>It will return null, if the template is read-only.</remarks>
        public PdfGraphics Graphics
        {
            get
            {
                if (m_bIsReadonly)
                {
                    m_graphics = null;
                }
                else if (m_graphics == null)
                {
                    PdfGraphics.GetResources gr = new PdfGraphics.GetResources(GetResources);

                    m_graphics = new PdfGraphics(Size, gr, m_content);

                    if(m_writeTransformation)
                    // Transform co-ordinates to Top/Left.
                    m_graphics.InitializeCoordinates();
                }

                return m_graphics;
            }
        }

        /// <summary>
        /// Gets the size of the template.
        /// </summary>
        public SizeF Size
        {
            get
            {
                return m_size;
            }
        }

        /// <summary>
        /// Gets the width of the template.
        /// </summary>
        public float Width
        {
            get
            {
                return Size.Width;
            }
        }

        /// <summary>
        /// Gets the height of the template.
        /// </summary>
        public float Height
        {
            get
            {
                return Size.Height;
            }
        }

        /// <summary>
        /// Gets a value indicating whether the template is read-only.
        /// </summary>
        /// <value><c>true</c> if the template is read-only; otherwise, <c>false</c>.</value>
        /// <remarks>Read-only templates does not expose graphics. They just return null.</remarks>
        public bool ReadOnly
        {
            get
            {
                return m_bIsReadonly;
            }
        }

        #endregion

        #region Public methods
        /// <summary>
        /// Resets the template and sets the specified size.
        /// </summary>
        /// <param name="size">The size.</param>
        public void Reset(SizeF size)
        {
            SetSize(size);
            Reset();
        }

        /// <summary>
        /// Resets an instance.
        /// </summary>
        public void Reset()
        {
            if (m_resources != null)
            {
                m_resources = null;
                m_content.Remove(DictionaryProperties.Resources);
            }

            if (m_graphics != null)
            {
                m_graphics.Reset(Size);
            }
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

        #region Implementation
        /// <summary>
        /// Returns a rectangle that bounds this element.
        /// </summary>
        /// <returns>Returns a rectangle that bounds this element.</returns>
        /// <remarks>This method doesn't take into consideration a rotation of the element.</remarks>
        #if !NETFX_CORE  && !WP
        [Syncfusion.Documentation.DocumentationExclude()] 
        #endif
        protected override RectangleF GetBoundsInternal()
        {
            return new RectangleF(PointF.Empty, Size);
        }

        /// <summary>
        /// Draws an element on the Graphics.
        /// </summary>
        /// <param name="graphics">Graphics context where the element should be printed.</param>
        #if !NETFX_CORE  && !WP
        [Syncfusion.Documentation.DocumentationExclude()] 
        #endif
        protected override void DrawInternal(PdfGraphics graphics)
        {
            if (graphics == null)
            {
                throw new ArgumentNullException("graphics");
            }

            graphics.DrawPdfTemplate(this, PointF.Empty);
        }

        /// <summary>
        /// Initializes object.
        /// </summary>
        private void Initialize()
        {
            AddType();
            AddSubType();
        }

        /// <summary>
        /// Gets the resources and modifies the template dictionary.
        /// </summary>
        /// <returns>Pdf resources.</returns>
        private PdfResources GetResources()
        {
            if (m_resources == null)
            {
                m_resources = new PdfResources();
                m_content[DictionaryProperties.Resources] = m_resources;
            }

            return m_resources;
        }

        /// <property name="flag" value="Finished" />
        /// <summary>
        /// Adds type key.
        /// </summary>
        private void AddType()
        {
            PdfName value = m_content.GetName(DictionaryProperties.XObject);

            m_content[DictionaryProperties.Type] = value;
        }

        /// <property name="flag" value="Finished" />
        /// <summary>
        /// Adds SubType key.
        /// </summary>
        private void AddSubType()
        {
            PdfName value = m_content.GetName(DictionaryProperties.Form);

            m_content[DictionaryProperties.Subtype] = value;
        }

        /// <summary>
        /// Sets the size of the template.
        /// </summary>
        /// <param name="size">The size.</param>
        private void SetSize(SizeF size)
        {
            RectangleF rect = new RectangleF(PointF.Empty, size);
            PdfArray val = PdfArray.FromRectangle(rect);

            m_content[DictionaryProperties.BBox] = val;
            m_size = size;
        }

        /// <summary>
        /// Sets the size of the template.
        /// </summary>
        /// <param name="size">The size.</param>
        private void SetSize(PointF origin, SizeF size)
        {         
            PdfArray array = new PdfArray(new float[] { origin.X, origin.Y, size.Width, size.Height });

            m_content[DictionaryProperties.BBox] = array;
            m_size = size;

        }

        /// <summary>
        /// Copies the resources from the template.
        /// </summary>
        internal void CloneResources(PdfCrossTable crossTable)
        {
            if (m_resources != null)
            {
                PdfDictionary resourceDict = m_resources.Clone(crossTable) as PdfDictionary;
                m_resources = new PdfResources(resourceDict);
                m_content[DictionaryProperties.Resources] = resourceDict;
            }
        }
        #endregion
    }
}
