#region Copyright Syncfusion Inc. 2001 - 2014
// Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Any infringement will be prosecuted under
// applicable laws. 
#endregion

/// <summary>
/// The Syncfusion.Pdf.ColorSpace namespace contains classes for enhanced printing support with various Color channels.
/// </summary>
namespace Syncfusion.Pdf.ColorSpace
{
    /// <summary>
    /// Represents the extended color, based on a complex colorspace. 
    /// </summary>
    public abstract class PdfExtendedColor
    {
        #region Fields
        /// <summary>
        /// To store the Colorspace.
        /// </summary>
        protected PdfColorSpaces m_colorspace;
        #endregion

        #region Constructors
        /// <summary>
        /// Initializes a new instance of the <see cref="PdfExtendedColor"/> class.
        /// </summary>
        /// <param name="colorspace">The colorspace.</param>
        public PdfExtendedColor(PdfColorSpaces colorspace)
        {
            m_colorspace = colorspace;
        }
        #endregion

        #region Properties
        /// <summary>
        /// Gets the Colorspace
        /// </summary>
        public PdfColorSpaces ColorSpace
        {
            get
            {
                return m_colorspace;
            }
        }
        #endregion
    }
}
