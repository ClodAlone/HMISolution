#region Copyright Syncfusion Inc. 2001 - 2014
// Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Any infringement will be prosecuted under
// applicable laws. 
#endregion
using System;
using Syncfusion.Pdf;


namespace Syncfusion.DocToPDFConverter
{
    /// <summary>
    /// Represent class with setting of converter.
    /// </summary>
    public class DocToPDFConverterSettings
    {
        #region Fields
        /// <summary>
        /// Indicates the quality of the image.
        /// </summary>
        private int m_imageQuality =100;

       /// <summary>
       /// Indicates the Image resolution
       /// </summary>
        internal int m_imageResolution =0;
        #endregion

        #region Properties
        /// <summary>
        /// Gets or sets the quality. The value indicates in percentage, max value represents best quality and min value represents best compression.
        /// </summary>
        /// <remarks>The value indicates in percentage, max value represents best quality and min value represents best compression</remarks>
        public int ImageQuality
        {
            get
            {
                return m_imageQuality;
            }
            set
            {
                if (value <= 100)
                {
                    m_imageQuality = value;
                }
                else
                {
                    throw new PdfException("The value should be between 0 and 100");
                }
            }
        }
        /// <summary>
        /// Sets the image resolution to the image, which are Embedded
        /// in the Word document
        /// </summary>
        public int ImageResolution
        {
            set
            {
                if (value>0)
                {
                    m_imageResolution = value;
                }
                else
                {
                    throw new PdfException("The value should be valid DPI");
                }

            }
        }
        #endregion
    }
}
