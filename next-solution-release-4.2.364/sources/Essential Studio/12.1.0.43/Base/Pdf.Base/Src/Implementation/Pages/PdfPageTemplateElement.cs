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

namespace Syncfusion.Pdf
{
    /// <summary>
    /// Describes a page template object that can be used as header/footer, watermark or stamp.
    /// </summary>
    public class PdfPageTemplateElement
    {
        #region Fields
        /// <summary>
        /// Layer type of the template.
        /// </summary>
        private bool m_foreground;
        /// <summary>
        /// Docking style.
        /// </summary>
        private PdfDockStyle m_dockStyle;
        /// <summary>
        /// Alignment style.
        /// </summary>
        private PdfAlignmentStyle m_alignmentStyle;
        /// <summary>
        /// PdfTemplate object.
        /// </summary>
        private PdfTemplate m_template;
        /// <summary>
        /// Usage type of this template.
        /// </summary>
        private TemplateType m_type;
        /// <summary>
        /// Location of the template on the page.
        /// </summary>
        private PointF m_location;
        #endregion

        #region Properties
        /// <summary>
        /// Gets or sets the dock style of the page template element.
        /// </summary>
        public PdfDockStyle Dock
        {
            get
            {
                return m_dockStyle;
            }
            set
            {
                if (m_dockStyle != value && Type == TemplateType.None)
                {
                    m_dockStyle = value;

                    // Reset alignment.
                    ResetAlignment();
                }
            }
        }

        /// <summary>
        /// Gets or sets alignment of the page template element.
        /// </summary>
        public PdfAlignmentStyle Alignment
        {
            get
            {
                return m_alignmentStyle;
            }
            set
            {
                if (m_alignmentStyle != value)
                {
                    SetAlignment(value);
                }
            }
        }

        /// <summary>
        /// Indicates whether the page template is located in front of 
        /// the page layers or behind of it.
        /// </summary>
        public bool Foreground
        {
            get
            {
                return m_foreground;
            }
            set
            {
                if (m_foreground != value)
                {
                    m_foreground = value;
                }
            }
        }

        /// <summary>
        /// Indicates whether the page template is located behind of 
        /// the page layers or in front of it.
        /// </summary>
        public bool Background
        {
            get
            {
                return !m_foreground;
            }
            set
            {
                m_foreground = !value;
            }
        }

        /// <summary>
        /// Gets or sets location of the page template element.
        /// </summary>
        public PointF Location
        {
            get
            {
                return m_location;
            }
            set
            {
                if (Type == TemplateType.None)
                {
                    m_location = value;
                }
            }
        }

        /// <summary>
        /// Gets or sets X co-ordinate of the template element on the page.
        /// </summary>
        public float X
        {
            get
            {
                return m_location.X;
            }
            set
            {
                if (Type == TemplateType.None)
                {

                    m_location.X = value;
                }
            }
        }

        /// <summary>
        /// Gets or sets Y co-ordinate of the template element on the page.
        /// </summary>
        public float Y
        {
            get
            {
                return m_location.Y;
            }
            set
            {
                if (Type == TemplateType.None)
                {
                    m_location.Y = value;
                }
            }
        }

        /// <summary>
        /// Gets or sets size of the page template element.
        /// </summary>
        public SizeF Size
        {
            get
            {
                return m_template.Size;
            }
            set
            {
                if (m_template.Size != value && Type == TemplateType.None)
                {
                    m_template.Reset(value);
                }
            }
        }

        /// <summary>
        /// Gets or sets width of the page template element.
        /// </summary>
        public float Width
        {
            get
            {
                return m_template.Width;
            }
            set
            {
                if (m_template.Width != value && Type == TemplateType.None)
                {
                    SizeF size = m_template.Size;

                    size.Width = value;
                    m_template.Reset(size);
                }
            }
        }

        /// <summary>
        /// Gets or sets height of the page template element.
        /// </summary>
        public float Height
        {
            get
            {
                return m_template.Height;
            }
            set
            {
                if (m_template.Height != value && Type == TemplateType.None)
                {
                    SizeF size = m_template.Size;

                    size.Height = value;
                    m_template.Reset(size);
                }
            }
        }

        /// <summary>
        /// Gets or sets bounds of the page template element.
        /// </summary>
        public RectangleF Bounds
        {
            get
            {
                return new RectangleF(Location, Size);
            }
            set
            {
                if (Type == TemplateType.None)
                {
                    Location = value.Location;
                    Size = value.Size;
                }
            }
        }

        /// <summary>
        /// Gets graphics context of the page template element.
        /// </summary>
        public PdfGraphics Graphics
        {
            get
            {
                return Template.Graphics;
            }
        }

        /// <summary>
        /// Gets Pdf template object.
        /// </summary>
        internal PdfTemplate Template
        {
            get
            {
                if (m_template == null)
                {
                    m_template = new PdfTemplate(Size);

                }
                return m_template;
            }
        }

        /// <summary>
        /// Gets or sets type of the usage of this page template.
        /// </summary>
        internal TemplateType Type
        {
            get
            {
                return m_type;
            }
            set
            {
                if (m_type != value)
                {
                    UpdateDocking(value);

                    m_type = value;
                }
            }
        }
        #endregion

        #region Constructors
        /// <summary>
        /// Creates a new page template.
        /// </summary>
        /// <param name="bounds">Bounds of the template.</param>
        public PdfPageTemplateElement(RectangleF bounds)
            : this(bounds.X, bounds.Y, bounds.Width, bounds.Height)
        {
        }
        /// <summary>
        /// Initializes a new instance of the <see cref="PdfPageTemplateElement"/> class.
        /// </summary>
        /// <param name="bounds">The bounds.</param>
        /// <param name="page">The page.</param>
        public PdfPageTemplateElement(RectangleF bounds, PdfPage page)
            : this(bounds.X, bounds.Y, bounds.Width, bounds.Height, page)
        {
        }

        /// <summary>
        /// Creates a new page template.
        /// </summary>
        /// <param name="location">Location of the template.</param>
        /// <param name="size">Size of the template.</param>
        public PdfPageTemplateElement(PointF location, SizeF size)
            : this(location.X, location.Y, size.Width, size.Height)
        {
        }
        /// <summary>
        /// Initializes a new instance of the <see cref="PdfPageTemplateElement"/> class.
        /// </summary>
        /// <param name="location">The location.</param>
        /// <param name="size">The size.</param>
        /// <param name="page">The page.</param>
        public PdfPageTemplateElement(PointF location, SizeF size, PdfPage page)
            : this(location.X, location.Y, size.Width, size.Height, page)
        {
        }

        /// <summary>
        /// Creates new page template object.
        /// </summary>
        /// <param name="size">Size of the template.</param>
        public PdfPageTemplateElement(SizeF size)
            : this(size.Width, size.Height)
        {
        }

        /// <summary>
        /// Creates a new page template.
        /// </summary>
        /// <param name="width">Width of the template.</param>
        /// <param name="height">Height of the template.</param>
        public PdfPageTemplateElement(float width, float height)
            : this(0, 0, width, height)
        {
        }

        /// <summary>
        /// Creates a new page template.
        /// </summary>
        /// <param name="width">Width of the template.</param>
        /// <param name="height">Height of the template.</param>
        /// <param name="page">The Current Page object.</param>
        public PdfPageTemplateElement(float width, float height, PdfPage page)
            : this(0, 0, width, height, page)
        {
        }

        /// <summary>
        /// Creates a new page template.
        /// </summary>
        /// <param name="x">X co-ordinate of the template.</param>
        /// <param name="y">Y co-ordinate of the template.</param>
        /// <param name="width">Width of the template.</param>
        /// <param name="height">Height of the template.</param>
        public PdfPageTemplateElement(float x, float y, float width, float height)
        {
            X = x;
            Y = Y;
            m_template = new PdfTemplate(width, height);
        }

        /// <summary>
        /// Creates a new page template.
        /// </summary>
        /// <param name="x">X co-ordinate of the template.</param>
        /// <param name="y">Y co-ordinate of the template.</param>
        /// <param name="width">Width of the template.</param>
        /// <param name="height">Height of the template.</param>
        /// <param name="page">The Current Page object.</param>
        public PdfPageTemplateElement(float x, float y, float width, float height, PdfPage page)
        {
            X = x;
            Y = Y;
            m_template = new PdfTemplate(width, height);
            Graphics.ColorSpace = page.Document.ColorSpace;
        }
        #endregion

        #region Helper methods
        /// <summary>
        /// Draws the template.
        /// </summary>
        /// <param name="layer">Parent layer.</param>
        /// <param name="document">Parent document.</param>
        internal void Draw(PdfPageLayer layer, PdfDocument document)
        {
            if (layer == null)
                throw new ArgumentNullException("layer");

            if (document == null)
                throw new ArgumentNullException("document");

            PdfPage page = layer.Page as PdfPage;
            RectangleF bounds = CalculateBounds(page, document);
            layer.Graphics.DrawPdfTemplate(Template, bounds.Location, bounds.Size);
        }

        /// <summary>
        /// Updates Dock property if template is used as header/footer.
        /// </summary>
        /// <param name="type">Type of the template.</param>
        private void UpdateDocking(TemplateType type)
        {
            if (type != TemplateType.None)
            {
                switch (type)
                {
                    case TemplateType.Top:
                        Dock = PdfDockStyle.Top;
                        break;

                    case TemplateType.Bottom:
                        Dock = PdfDockStyle.Bottom;
                        break;

                    case TemplateType.Left:
                        Dock = PdfDockStyle.Left;
                        break;

                    case TemplateType.Right:
                        Dock = PdfDockStyle.Right;
                        break;
                }

                ResetAlignment();
            }
        }

        /// <summary>
        /// Resets alignment of the template.
        /// </summary>
        private void ResetAlignment()
        {
            Alignment = PdfAlignmentStyle.None;
        }

        /// <summary>
        /// Sets alignment of the template.
        /// </summary>
        /// <param name="alignment">Alignment style.</param>
        private void SetAlignment(PdfAlignmentStyle alignment)
        {
            if (Dock == PdfDockStyle.None)
            {
                m_alignmentStyle = alignment;
            }
            else
            {
                // Template is docked and alignment has been changed.
                bool canBeSet = false;

                switch (Dock)
                {
                    case PdfDockStyle.Left:
                        canBeSet = (alignment == PdfAlignmentStyle.TopLeft || alignment == PdfAlignmentStyle.MiddleLeft ||
                            alignment == PdfAlignmentStyle.BottomLeft || alignment == PdfAlignmentStyle.None);
                        break;

                    case PdfDockStyle.Top:
                        canBeSet = (alignment == PdfAlignmentStyle.TopLeft || alignment == PdfAlignmentStyle.TopCenter ||
                            alignment == PdfAlignmentStyle.TopRight || alignment == PdfAlignmentStyle.None);
                        break;

                    case PdfDockStyle.Right:
                        canBeSet = (alignment == PdfAlignmentStyle.TopRight || alignment == PdfAlignmentStyle.MiddleRight ||
                            alignment == PdfAlignmentStyle.BottomRight || alignment == PdfAlignmentStyle.None);
                        break;

                    case PdfDockStyle.Bottom:
                        canBeSet = (alignment == PdfAlignmentStyle.BottomLeft || alignment == PdfAlignmentStyle.BottomCenter ||
                            alignment == PdfAlignmentStyle.BottomRight || alignment == PdfAlignmentStyle.None);
                        break;

                    case PdfDockStyle.Fill:
                        canBeSet = (alignment == PdfAlignmentStyle.MiddleCenter || alignment == PdfAlignmentStyle.None);
                        break;
                }

                if (canBeSet)
                {
                    m_alignmentStyle = alignment;
                }
            }
        }

        /// <summary>
        /// Calculates bounds of the page template.
        /// </summary>
        /// <param name="page">Parent page.</param>
        /// <param name="document">Parent document.</param>
        /// <returns>Bounds of the page template.</returns>
        private RectangleF CalculateBounds(PdfPage page, PdfDocument document)
        {
            if (page == null)
                throw new ArgumentNullException("page");

            if (document == null)
                throw new ArgumentNullException("document");

            RectangleF result = Bounds;

            if (m_alignmentStyle != PdfAlignmentStyle.None)
            {
                result = GetAlignmentBounds(page, document);
            }
            else if (m_dockStyle != PdfDockStyle.None)
            {
                result = GetDockBounds(page, document);
            }

            return result;
        }

        /// <summary>
        /// Calculates bounds according to the alignment.
        /// </summary>
        /// <param name="page">Parent page.</param>
        /// <param name="document">Parent document.</param>
        /// <returns>Bounds according to the alignment.</returns>
        private RectangleF GetAlignmentBounds(PdfPage page, PdfDocument document)
        {
            if (page == null)
                throw new ArgumentNullException("page");

            if (document == null)
                throw new ArgumentNullException("document");

            RectangleF result = Bounds;

            if (Type == TemplateType.None)
            {
                result = GetSimpleAlignmentBounds(page, document);
            }
            else
            {
                result = GetTemplateAlignmentBounds(page, document);
            }

            return result;
        }

        /// <summary>
        /// Calculates bounds according to the alignment.
        /// </summary>
        /// <param name="page">Parent page.</param>
        /// <param name="document">Parent document.</param>
        /// <returns>Bounds according to the alignment.</returns>
        private RectangleF GetSimpleAlignmentBounds(PdfPage page, PdfDocument document)
        {
            if (page == null)
                throw new ArgumentNullException("page");

            if (document == null)
                throw new ArgumentNullException("document");

            RectangleF result = Bounds;
            PdfSection section = page.Section;
            RectangleF actualBounds = section.GetActualBounds(document, page, false);
            float x = X;
            float y = Y;

            switch (m_alignmentStyle)
            {
                case PdfAlignmentStyle.TopLeft:
                    {
                        x = 0f;
                        y = 0f;
                    }
                    break;

                case PdfAlignmentStyle.TopCenter:
                    {
                        x = (actualBounds.Width - Width) / 2f;
                        y = 0f;
                    }
                    break;

                case PdfAlignmentStyle.TopRight:
                    {
                        x = actualBounds.Width - Width;
                        y = 0f;
                    }
                    break;

                case PdfAlignmentStyle.MiddleLeft:
                    {
                        x = 0f;
                        y = (actualBounds.Height - Height) / 2f;
                    }
                    break;

                case PdfAlignmentStyle.MiddleCenter:
                    {
                        x = (actualBounds.Width - Width) / 2f;
                        y = (actualBounds.Height - Height) / 2f;
                    }
                    break;

                case PdfAlignmentStyle.MiddleRight:
                    {
                        x = actualBounds.Width - Width;
                        y = (actualBounds.Height - Height) / 2f;
                    }
                    break;

                case PdfAlignmentStyle.BottomLeft:
                    {
                        x = 0f;
                        y = actualBounds.Height - Height;
                    }
                    break;

                case PdfAlignmentStyle.BottomCenter:
                    {
                        x = (actualBounds.Width - Width) / 2f;
                        y = actualBounds.Height - Height;
                    }
                    break;

                case PdfAlignmentStyle.BottomRight:
                    {
                        x = actualBounds.Width - Width;
                        y = actualBounds.Height - Height;
                    }
                    break;

            }

            result.X = x;
            result.Y = y;

            return result;
        }

        /// <summary>
        /// Calculates bounds according to the alignment.
        /// </summary>
        /// <param name="page">Parent page.</param>
        /// <param name="document">Parent document.</param>
        /// <returns>Bounds according to the alignment.</returns>
        private RectangleF GetTemplateAlignmentBounds(PdfPage page, PdfDocument document)
        {
            if (page == null)
                throw new ArgumentNullException("page");

            if (document == null)
                throw new ArgumentNullException("document");

            RectangleF result = Bounds;
            PdfSection section = page.Section;
            RectangleF actualBounds = section.GetActualBounds(document, page, false);
            float x = X;
            float y = Y;

            switch (m_alignmentStyle)
            {
                case PdfAlignmentStyle.TopLeft:
                    {
                        if (Type == TemplateType.Left)
                        {
                            x = -actualBounds.X;
                            y = 0f;
                        }
                        else if (Type == TemplateType.Top)
                        {
                            x = -actualBounds.X;
                            y = -actualBounds.Y;
                        }
                        break;
                    }

                case PdfAlignmentStyle.TopCenter:
                    {
                        x = (actualBounds.Width - Width) / 2f;
                        y = -actualBounds.Y;
                        break;
                    }

                case PdfAlignmentStyle.TopRight:
                    {
                        if (Type == TemplateType.Right)
                        {
                            x = actualBounds.Width + section.GetRightIndentWidth(document, page, false) - Width;
                            y = 0f;
                        }
                        else if (Type == TemplateType.Top)
                        {
                            x = actualBounds.Width + section.GetRightIndentWidth(document, page, false) - Width;
                            y = -actualBounds.Y;
                        }
                        break;
                    }

                case PdfAlignmentStyle.MiddleLeft:
                    {
                        x = -actualBounds.X;
                        y = (actualBounds.Height - Height) / 2f;
                        break;
                    }

                case PdfAlignmentStyle.MiddleCenter:
                    {
                        x = (actualBounds.Width - Width) / 2f;
                        y = (actualBounds.Height - Height) / 2f;
                        break;
                    }

                case PdfAlignmentStyle.MiddleRight:
                    {
                        x = actualBounds.Width + section.GetRightIndentWidth(document, page, false) - Width;
                        y = (actualBounds.Height - Height) / 2f;
                        break;
                    }

                case PdfAlignmentStyle.BottomLeft:
                    {
                        if (Type == TemplateType.Left)
                        {
                            x = -actualBounds.X;
                            y = actualBounds.Height - Height;
                        }
                        else if (Type == TemplateType.Bottom)
                        {
                            x = -actualBounds.X;
                            y = actualBounds.Height + section.GetBottomIndentHeight(document, page, false) - Height;
                        }
                        break;
                    }

                case PdfAlignmentStyle.BottomCenter:
                    {
                        x = (actualBounds.Width - Width) / 2f;
                        y = actualBounds.Height + section.GetBottomIndentHeight(document, page, false) - Height;
                        break;
                    }

                case PdfAlignmentStyle.BottomRight:
                    {
                        if (Type == TemplateType.Right)
                        {
                            x = actualBounds.Width + section.GetRightIndentWidth(document, page, false) - Width;
                            y = actualBounds.Height - Height;
                        }
                        else if (Type == TemplateType.Bottom)
                        {
                            x = actualBounds.Width + section.GetRightIndentWidth(document, page, false) - Width;
                            y = actualBounds.Height + section.GetBottomIndentHeight(document, page, false) - Height;
                        }
                        break;
                    }
            }

            result.X = x;
            result.Y = y;

            return result;
        }

        /// <summary>
        /// Calculates bounds according to the docking.
        /// </summary>
        /// <param name="page">Parent page.</param>
        /// <param name="document">Parent document.</param>
        /// <returns>Bounds according to the docking.</returns>
        private RectangleF GetDockBounds(PdfPage page, PdfDocument document)
        {
            if (page == null)
                throw new ArgumentNullException("page");

            if (document == null)
                throw new ArgumentNullException("document");

            RectangleF result = Bounds;

            if (Type == TemplateType.None)
            {
                result = GetSimpleDockBounds(page, document);
            }
            else
            {
                result = GetTemplateDockBounds(page, document);
            }

            return result;
        }

        /// <summary>
        /// Calculates template bounds basing on docking if template is not page template.
        /// </summary>
        /// <param name="page">Parent page.</param>
        /// <param name="document">Parent document.</param>
        private RectangleF GetSimpleDockBounds(PdfPage page, PdfDocument document)
        {
            if (page == null)
                throw new ArgumentNullException("page");

            if (document == null)
                throw new ArgumentNullException("document");

            RectangleF result = Bounds;
            PdfSection section = page.Section;
            RectangleF actualBounds = section.GetActualBounds(document, page, false);
            float x = X;
            float y = Y;
            float width = Width;
            float height = Height;

            switch (m_dockStyle)
            {
                case PdfDockStyle.Left:
                    x = 0f;
                    y = 0f;
                    width = Width;
                    height = actualBounds.Height;
                    break;

                case PdfDockStyle.Top:
                    x = 0f;
                    y = 0f;
                    width = actualBounds.Width;
                    height = Height;
                    break;

                case PdfDockStyle.Right:
                    x = actualBounds.Width - Width;
                    y = 0f;
                    width = Width;
                    height = actualBounds.Height;
                    break;

                case PdfDockStyle.Bottom:
                    x = 0f;
                    y = actualBounds.Height - Height;
                    width = actualBounds.Width;
                    height = Height;
                    break;

                case PdfDockStyle.Fill:
                    x = 0f;
                    x = 0f;
                    width = actualBounds.Width;
                    height = actualBounds.Height;
                    break;
            }

            result = new RectangleF(x, y, width, height);

            return result;
        }

        /// <summary>
        /// Calculates template bounds basing on docking if template is a page template.
        /// </summary>
        /// <param name="page">Parent page.</param>
        /// <param name="document">Parent document.</param>
        private RectangleF GetTemplateDockBounds(PdfPage page, PdfDocument document)
        {
            if (page == null)
                throw new ArgumentNullException("page");

            if (document == null)
                throw new ArgumentNullException("document");

            RectangleF result = Bounds;
            PdfSection section = page.Section;
            RectangleF actualBounds = section.GetActualBounds(document, page, false);
            SizeF actualSize = section.PageSettings.GetActualSize();
            float x = X;
            float y = Y;
            float width = Width;
            float height = Height;

            switch (m_dockStyle)
            {
                case PdfDockStyle.Left:
                    x = -actualBounds.X;
                    y = 0f;
                    width = Width;
                    height = actualBounds.Height;
                    break;

                case PdfDockStyle.Top:
                    x = -actualBounds.X;
                    y = -actualBounds.Y;
                    width = actualSize.Width;
                    height = Height;
					
					if (actualBounds.Height<0)
                        y=-actualBounds.Y+actualSize.Height;
                    break;

                case PdfDockStyle.Right:
                    x = actualBounds.Width + section.GetRightIndentWidth(document, page, false) - Width;
                    y = 0f;
                    width = Width;
                    height = actualBounds.Height;
                    break;

                case PdfDockStyle.Bottom:
                    x = -actualBounds.X;
                    y = actualBounds.Height + section.GetBottomIndentHeight(document, page, false) - Height;
                    width = actualSize.Width;
                    height = Height;

                    if (actualBounds.Height<0)
                        y -= actualSize.Height;
                    break;

                case PdfDockStyle.Fill:
                    x = 0f;
                    x = 0f;
                    width = actualBounds.Width;
                    height = actualBounds.Height;
                    break;
            }

            result = new RectangleF(x, y, width, height);

            return result;
        }
        #endregion
    }
}
