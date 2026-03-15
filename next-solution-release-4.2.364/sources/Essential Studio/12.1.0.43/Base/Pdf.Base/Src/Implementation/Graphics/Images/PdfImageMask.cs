#region Copyright Syncfusion Inc. 2001 - 2014
// Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Any infringement will be prosecuted under
// applicable laws. 
#endregion

#if !SILVERLIGHT && !NETFX_CORE && !WP
using System;


using System.Drawing.Imaging;


namespace Syncfusion.Pdf.Graphics
{
    /// <summary>
    /// Represents the image mask object for bitmaps.
    /// </summary>
    public class PdfImageMask : PdfMask
    {
#region Fields
        /// <summary>
        /// Holds mask image.
        /// </summary>
        private PdfBitmap m_imageMask;
        /// <summary>
        /// Holds mask type flag.
        /// </summary>
        private bool m_softMask;
        #endregion

#region Properties
        /// <summary>
        /// Gets the image mask.
        /// </summary>
        /// <value>The image mask.</value>
        public PdfBitmap Mask
        {
            get
            {
                return m_imageMask;
            }
        }

        /// <summary>
        /// Gets the mask type.
        /// </summary>
        /// <value><c>true</c> if soft mask; otherwise, hard mask <c>false</c>.</value>
        public bool SoftMask
        {
            get
            {
                return m_softMask;
            }
        }
        #endregion

#region Constructor
        /// <summary>
        /// Creates new PdfImageMask object.
        /// </summary>
        /// <param name="imageMask">The image mask.</param>
        public PdfImageMask(PdfBitmap imageMask)
        {
            if (imageMask == null)
                throw new ArgumentNullException("imageMask");

            PixelFormat format = imageMask.InternalImage.PixelFormat;

            if (format == PixelFormat.Format8bppIndexed)
            {
                m_softMask = true;
            }
            else if (format == PixelFormat.Format1bppIndexed)
            {
                m_softMask = false;
            }
            else
            {
                throw new ArgumentException("imageMask", "Image mask should be gray scale or black and white.");
            }

            m_imageMask = imageMask;
        }
        #endregion
    }
}
#endif