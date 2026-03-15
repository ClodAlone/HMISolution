#region Copyright
// Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Any infringement will be prosecuted under
// applicable laws. 
#endregion

using System;
using System.Text;

namespace Syncfusion.DocIO.DLS
{
    /// <summary>
    /// Represent document saving options
    /// </summary>
    public class SaveOptions
    {
        #region Fields
        private CssStyleSheetType m_htmlExportCssStyleSheetType = CssStyleSheetType.External;
        private string m_htmlExportCssStyleSheetFileName;
        private bool m_htmlExportHeadersFooters = true;
        private bool m_htmlExportTextInputFormFieldAsText;
        private string m_htmlExportImagesFolder = string.Empty;
        private int m_EPubHeadingLevels = 3;
        private bool m_EPubExportFont = false;
        private string[] m_fontFiles;
        #endregion

        #region Properties
        /// <summary>
        /// Gets of sets the font file references
        /// </summary>
        internal string[] FontFiles
        {
            get
            {
                return m_fontFiles;
            }
            set
            {
                m_fontFiles = value;
            }
        }
        /// <summary>
        /// Gets or sets if font should be embedded in EPub
        /// </summary>
        public bool EPubExportFont
        {
            get
            {
                return m_EPubExportFont;
            }
            set
            {
                m_EPubExportFont = value;
            }
        }
        /// <summary>
        /// Gets or sets number of heading levels for EPub format
        /// </summary>
        internal int EPubHeadingLevels
        {
            get
            {
                return m_EPubHeadingLevels;
            }
            set
            {
                m_EPubHeadingLevels = value;
            }
        }
        /// <summary>
        /// Gets or sets the type of the HTML export CSS style sheet.
        /// </summary>
        /// <value>The type of the HTML export CSS style sheet.</value>
        public CssStyleSheetType HtmlExportCssStyleSheetType
        {
            get
            {
                return m_htmlExportCssStyleSheetType;
            }
            set
            {
                m_htmlExportCssStyleSheetType = value;
            }
        }
        /// <summary>
        /// Gets or sets the name of the HTML export CSS style sheet file.
        /// </summary>
        /// <value>The name of the HTML export CSS style sheet file.</value>
        public string HtmlExportCssStyleSheetFileName
        {
            get
            {
                return m_htmlExportCssStyleSheetFileName;
            }
            set
            {
                m_htmlExportCssStyleSheetFileName = value;
            }
        }
        /// <summary>
        /// Gets or sets a value indicating whether [ERROR: invalid expression End][HTML export headers footers].
        /// </summary>
        /// <value>
        /// 	If the HTML export headers footers, set to <c>true</c>.
        /// </value>
        public bool HtmlExportHeadersFooters
        {
            get
            {
                return m_htmlExportHeadersFooters;
            }
            set
            {
                m_htmlExportHeadersFooters = value;
            }
        }
        /// <summary>
        /// Gets or sets a value indicating whether [ERROR: invalid expression End][HTML export text input form field as text].
        /// </summary>
        /// <value>
        /// 	If HTML export text input form field as text, set to <c>true</c>.
        /// </value>
        public bool HtmlExportTextInputFormFieldAsText
        {
            get
            {
                return m_htmlExportTextInputFormFieldAsText;
            }
            set
            {
                m_htmlExportTextInputFormFieldAsText = value;
            }
        }
        /// <summary>
        /// Gets or sets the HTML export images folder.
        /// </summary>
        /// <value>The HTML export images folder.</value>
        public string HtmlExportImagesFolder
        {
            get
            {
                return m_htmlExportImagesFolder;
            }
            set
            {
                m_htmlExportImagesFolder = value;
            }
        }
        #endregion
    }
    /// <summary>
    /// Specifies CssStyleSheetType.
    /// </summary>
    public enum CssStyleSheetType
    {
        /// <summary>
        /// Specifies External sheet type.
        /// </summary>
        External,
        /// <summary>
        ///  Specifies Internal sheet type.
        /// </summary>
        Internal
    }
}
