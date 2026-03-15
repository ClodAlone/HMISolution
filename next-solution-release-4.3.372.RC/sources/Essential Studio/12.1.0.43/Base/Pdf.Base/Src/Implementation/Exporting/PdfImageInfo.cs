#region Copyright Syncfusion Inc. 2001 - 2014
// Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Any infringement will be prosecuted under
// applicable laws. 
#endregion
#if !SILVERLIGHT && !NETFX_CORE

using System;
using System.Collections.Generic;
using System.Drawing;
using System.Text;
using Syncfusion.Pdf.IO;

namespace Syncfusion.Pdf.Exporting
{
    /// <summary>
    /// Represents the utility class to store information about Images and its location.
    /// </summary>
    public class PdfImageInfo
    {
#region Fields
        /// <summary>
        /// Local Variable to store the image bounds.
        /// </summary>
        private RectangleF m_bounds;

        /// <summary>
        /// Local Variable to store the image.
        /// </summary>
        private Image m_image;

        /// <summary>
        /// Local Variable to store the image index.
        /// </summary>
        private int m_index;

        /// <summary>
        /// Internal variable to store the image name.
        /// </summary>
        private string m_name;
        /// <summary>
        /// Internal variable to store the matrix.
        /// </summary>
        private PdfMatrix m_matrix;
        /// <summary>
        /// Internal variable to store the image name.
        /// </summary>
        private bool m_maskImage;
        /// <summary>
        /// Internal variable , it identifies the image is extracted or not.
        /// </summary>
        private bool m_bisImageExtracted = false;

        #endregion

#region Constructors
        /// <summary>
        ///  Initializes a new instance of the <see cref="T:PdfImagesInfo"/> class.
        /// </summary>
        internal PdfImageInfo()
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="T:PdfImagesInfo"/> class.
        /// </summary>
        /// <param name="bounds">Image bounds</param>
        /// <param name="image">Image</param>
        /// <param name="index">Image index</param>
        internal PdfImageInfo(RectangleF bounds, Image image, int index)
        {
            m_bounds = bounds;
            m_image = image;
            m_index = index;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="T:PdfImagesInfo"/> class.
        /// </summary>
        /// <param name="bounds">Image bounds</param>
        /// <param name="image">Image</param>
        /// <param name="index">Image index</param>
        /// <param name="name">Image name</param>
        internal PdfImageInfo(RectangleF bounds, Image image, int index, string name)
        {
            m_bounds = bounds;
            m_image = image;
            m_index = index;
            m_name = name;
        }
        #endregion

#region Properties
        /// <summary>
        /// Gets the Image Boundary location.
        /// </summary>
        public RectangleF Bounds
        {
            get
            {
                return m_bounds;
            }
            internal set
            {
                m_bounds = value;
            }
        }

        /// <summary>
        /// Gets the Image.
        /// </summary>
        public Image Image
        {
            get
            {
                return m_image;
            }
            internal set
            {
                m_image = value;
            }
        }

        /// <summary>
        /// Gets the Image index.
        /// </summary>
        public int Index
        {
            get
            {
                return m_index;
            }
            internal set
            {
                m_index = value;
            }
        }

        /// <summary>
        /// Gets the image name.
        /// </summary>
        internal string Name
        {
            get
            {
                return m_name;
            }
            set
            {
                m_name = value;
            }
        }
        /// <summary>
        /// Gets the matrix.
        /// </summary>
        internal PdfMatrix Matrix
        {
            get
            {
                return m_matrix;
            }
            set
            {
                m_matrix = value;
            }
        }

        /// <summary>
        /// Gets the matrix.
        /// </summary>
        internal bool MaskImage
        {
            get
            {
                return m_maskImage;
            }
            set
            {
                m_maskImage = value;
            }
        }
        /// <summary>
        /// Gets or sets the image is extracted or not.
        /// </summary>
        internal bool IsImageExtracted
        {
            get
            {
                return m_bisImageExtracted;
            }
            set
            {
                m_bisImageExtracted = value;
            }
        }
        #endregion
    }
}
#endif