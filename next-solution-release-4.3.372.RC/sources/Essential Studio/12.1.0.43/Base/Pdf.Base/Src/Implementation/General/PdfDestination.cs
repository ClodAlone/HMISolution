#region Copyright Syncfusion Inc. 2001 - 2014
// Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Any infringement will be prosecuted under
// applicable laws. 
#endregion

using System;
using System.Drawing;

using Syncfusion.Pdf.IO;
using Syncfusion.Pdf.Primitives;

namespace Syncfusion.Pdf.Interactive
{
    /// <summary>
    /// Represents an anchor in the document where bookmarks or annotations can direct when clicked.
    /// </summary>
    public class PdfDestination : IPdfWrapper
    {
        #region Fields
        /// <summary>
        /// Type of the destination.
        /// </summary>
        private PdfDestinationMode m_destinationMode = PdfDestinationMode.Location;

        /// <summary>
        /// Zoom factor.
        /// </summary>
        private float m_zoom = 0;

        /// <summary>
        /// Location of the destination.
        /// </summary>
        private PointF m_location = PointF.Empty;

        RectangleF m_bounds = RectangleF.Empty;

        /// <summary>
        /// Parent page reference.
        /// </summary>
        private PdfPageBase m_page;

        /// <summary>
        /// Pdf primitive representing this object.
        /// </summary>
        private PdfArray m_array = new PdfArray();

        /// <summary>
        /// Indicates whether destination is valid. 
        /// Destination is not valid if it has null location or zoom
        /// otherwise it is valid.
        /// </summary>
        private bool m_isValid = true;
        #endregion

        #region Constructors
        /// <summary>
        /// Initializes a new instance of the <see cref="PdfDestination"/> class.
        /// </summary>
        /// <param name="page">The page.</param>
        public PdfDestination(PdfPageBase page)
        {
            if (page == null)
            {
                throw new ArgumentNullException("page");
            }

            PdfPageRotateAngle angle = PdfPageRotateAngle.RotateAngle0;

            if (page.Rotation != PdfPageRotateAngle.RotateAngle0 && page.Rotation != PdfPageRotateAngle.RotateAngle90)
                angle = page.Rotation;

            if (page is PdfPage)
            {
                PdfPageRotateAngle newAngle = (page as PdfPage).Section.PageSettings.Rotate;
                
                if (newAngle != PdfPageRotateAngle.RotateAngle0 && newAngle != PdfPageRotateAngle.RotateAngle90 && newAngle != angle)
                    angle = newAngle;
            }

            if (page.Rotation == PdfPageRotateAngle.RotateAngle180)
                m_location = new PointF(page.Size.Width, m_location.Y);
            else if (page.Rotation == PdfPageRotateAngle.RotateAngle90)
                m_location = new PointF(0f, 0f);
            else if (page.Rotation == PdfPageRotateAngle.RotateAngle270)
                m_location = new PointF(page.Size.Width, 0f);
            else
                m_location = new PointF(0f,  m_location.Y);

            m_page = page;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="PdfDestination"/> class.
        /// </summary>
        /// <param name="page">The page.</param>
        /// <param name="location">The location.</param>
        public PdfDestination(PdfPageBase page, PointF location)
            : this(page)
        {
            if (page == null)
            {
                throw new ArgumentNullException("page");
            }

            m_location = location;
        }

        internal PdfDestination(PdfPageBase page, RectangleF rect)
            : this(page)
        {
            if (page == null)
            {
                throw new ArgumentNullException("page");
            }

            m_bounds = rect;
        }
        #endregion

        #region Properties
        /// <summary>
        /// Gets or sets zoom factor.
        /// </summary>
        public float Zoom
        {
            get
            {
                return m_zoom;
            }

            set
            {
                if (m_zoom != value)
                {
                    m_zoom = value;
                    InitializePrimitive();
                }
            }
        }

        /// <summary>
        /// Gets or sets a page where the destination is situated.
        /// </summary>
        public PdfPageBase Page
        {
            get
            {
                return m_page;
            }

            set
            {
                if (value == null)
                {
                    throw new ArgumentNullException("Page");
                }

                if (m_page != value)
                {
                    m_page = value;
                    InitializePrimitive();
                }
            }
        }

        /// <summary>
        /// Gets or sets mode of the destination.
        /// </summary>
        public PdfDestinationMode Mode
        {
            get
            {
                return m_destinationMode;
            }

            set
            {
                if (m_destinationMode != value)
                {
                    m_destinationMode = value;
                    InitializePrimitive();
                }
            }
        }

        /// <summary>
        /// Gets or sets a location of the destination.
        /// </summary>
        public PointF Location
        {
            get
            {
                return m_location;
            }

            set
            {
                if (m_location != value)
                {
                    m_location = value;
                    InitializePrimitive();
                }
            }
        }

        /// <summary>
        /// Gets a value indicating whether this instance is valid.
        /// </summary>
        /// <value><c>true</c> if this instance is valid; otherwise, <c>false</c>.</value>
        public bool IsValid
        {
            get
            {
                return m_isValid;
            }
        }
        #endregion

        #region Implementation

        /// <summary>
        /// Sets the validation.
        /// </summary>
        /// <param name="valid">if it is valid, set to <c>true</c>.</param>
        internal void SetValidation(bool valid)
        {
            m_isValid = valid;
        }

        /// <summary>
        /// Translates co-ordinates to PDF co-ordinate system (lower/left).
        /// </summary>
        /// <param name="page">The page.</param>
        /// <param name="point">Point in left/top co-ordinate system.</param>
        /// <returns>
        /// Co-ordinates to PDF co-ordinate system (lower/left).
        /// </returns>
        private PointF PointToNativePdf(PdfPage page, PointF point)
        {
            PdfSection section = page.Section;
            return section.PointToNativePdf(page, point);
        }

        /// <summary>
        /// Infills array by correct values.
        /// </summary>
        private void InitializePrimitive()
        {
            m_array.Clear();
            m_array.Add(new PdfReferenceHolder(m_page));

            switch (m_destinationMode)
            {
                case PdfDestinationMode.Location:

                    PdfPage simplePage = m_page as PdfPage;

                    PointF point = PointF.Empty;

                    if (simplePage != null)
                    {
                        point = PointToNativePdf(simplePage, m_location);
                    }
                    else
                    {
                        PdfLoadedPage loadedPage = m_page as PdfLoadedPage;
                        if (m_page.Rotation == PdfPageRotateAngle.RotateAngle180)
                        {
                            point.X = loadedPage.Size.Width;
                            point.Y = m_location.Y;
                        }
                        else if (m_page.Rotation == PdfPageRotateAngle.RotateAngle90)
                            point.X = m_location.Y;
                        else if (m_page.Rotation == PdfPageRotateAngle.RotateAngle270)
                        {
                            point.X = loadedPage.Size.Width - m_location.Y;
                            point.Y = loadedPage.Size.Height;
                        }
                        else
                            point.Y = loadedPage.Size.Height - m_location.Y;
                    }

                    m_array.Add(new PdfName(DictionaryProperties.XYZ));
                    m_array.Add(new PdfNumber(point.X));
                    m_array.Add(new PdfNumber(point.Y));
                    m_array.Add(new PdfNumber(m_zoom));
                    break;

                case PdfDestinationMode.FitToPage:
                    m_array.Add(new PdfName(DictionaryProperties.Fit));
                    break;
               
                case PdfDestinationMode.FitR:
                    
                    PdfLoadedPage loadedFitR = m_page as PdfLoadedPage;
                    if (loadedFitR != null)
                    {
                       
                        
                        m_array.Add(new PdfName(DictionaryProperties.FitR));
                        m_array.Add(new PdfNumber(m_bounds.X));
                        m_array.Add(new PdfNumber(m_bounds.Y));
                        m_array.Add(new PdfNumber(m_bounds.Width));
                        m_array.Add(new PdfNumber(m_bounds.Height));
                    }

                    break;
            }
        }

        /// <summary>
        /// Initializes instance.
        /// </summary>
        private void Initialize()
        {
        }

        #endregion

        #region IPdfWrapper Members
        /// <summary>
        /// Gets pdf primitive representing this object.
        /// </summary>
        IPdfPrimitive IPdfWrapper.Element
        {
            get
            {
                InitializePrimitive();
                return m_array;
            }
        }
        #endregion
    }
}
