#region Copyright Syncfusion Inc. 2001 - 2014
// Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
//
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Re-distribution in any form is strictly
// prohibited. Any infringement will be prosecuted under applicable laws. 
#endregion

using System;
using System.ComponentModel;
using System.Drawing.Printing;
using System.Globalization;
using System.Runtime.Serialization;
using Syncfusion.Windows.Forms.Diagram;

namespace Syncfusion.Windows.Forms.Diagram
{
    /// <summary>
    /// The HeaderFooterData class encapsulates the header and footer settings for the diagram. Used by the <see cref="Syncfusion.Windows.Forms.Diagram.Model"/> class 
    /// for configuring the header and footer when printing the diagram.
    /// <see cref="Syncfusion.Windows.Forms.Diagram.Model.HeaderFooterData"/>
    /// </summary>
    [Serializable]
    [TypeConverter(typeof(ExpandableObjectConverter))]
    public class HeaderFooterData
        : ISerializable,
          ICloneable
    {
        #region Class members
        /// <summary>
        /// Header class
        /// </summary>
        private Header m_header;

        /// <summary>
        /// Footer class
        /// </summary>
        private Footer m_footer;

        /// <summary>
        /// Culture info for header and footer
        /// </summary>
        private CultureInfo m_culture;
        #endregion

        #region Class initialize/fianlize methods
        /// <summary>
        /// Initializes a new instance of the <see cref="HeaderFooterData"/> class.
        /// </summary>
        public HeaderFooterData()
        { 
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="HeaderFooterData"/> class.
        /// </summary>
        /// <param name="dpd">The source <see cref="Syncfusion.Windows.Forms.Diagram.HeaderFooterData"/>.</param>
        public HeaderFooterData(HeaderFooterData dpd)
        {
            m_header = (Header)dpd.Header.Clone();
            m_footer = (Footer)dpd.Footer.Clone();
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="HeaderFooterData"/> class.
        /// </summary>
        /// <param name="info">The info.</param>
        /// <param name="context">The context.</param>
        public HeaderFooterData(SerializationInfo info, StreamingContext context)
        {
            m_header = (Header)info.GetValue("header", typeof(Header));
            m_footer = (Footer)info.GetValue("footer", typeof(Footer));
        }
        #endregion

        #region Class properties
        /// <summary>
        /// Gets or sets the diagram <see cref="Syncfusion.Windows.Forms.Diagram.Header"/>.
        /// </summary>
        [Browsable(true)]
        [Category("Header and Footer")]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
        [Description("The diagram header.")]
        public Header Header
        {
            get
            {
                if (m_header == null)
                {
                    m_header = new Header();
                }

                return m_header;
            }
            set
            {
                if (m_header != value)
                {
                    m_header = value;
                }
            }
        }

        /// <summary>
        /// Gets or sets the diagram print <see cref="Syncfusion.Windows.Forms.Diagram.Footer"/>.
        /// </summary>
        [Browsable(true)]
        [Category("Header and Footer")]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
        [Description("The diagram footer.")]
        public Footer Footer
        {
            get
            {
                if (m_footer == null)
                    m_footer = new Footer();

                return m_footer;
            }
            set
            {
                if (m_footer != value)
                {
                    m_footer = value;
                }
            }
        }

        /// <summary>
        /// Gets or sets the culture information for the header and footer data.
        /// </summary>
        /// <value>The culture.</value>
        [Browsable(true)]
        [Category("Header and Footer")]
        [Description("Culture information used for the diagram header and footer.")]
        public CultureInfo Culture
        {
            get
            {
                if (m_culture == null)
                    m_culture = new CultureInfo(CultureInfo.CurrentCulture.Name);

                return m_culture;
            }
            set
            {
                if (m_culture != value)
                {
                    m_header.Culture = value;
                    m_footer.Culture = value;
                    m_culture = value;
                }
            }
        }

        /// <summary>
        /// Check if should to serialize culture.
        /// </summary>
        /// <returns>true, if serialize culture.</returns>
        [Documentation.DocumentationExclude()]
        protected bool ShouldSerializeCulture()
        {
            return (m_culture != null && m_culture.Name != CultureInfo.CurrentCulture.Name);
        }

        /// <summary>
        /// Resets the culture.
        /// </summary>
        [Documentation.DocumentationExclude()]
        protected void ResetCulture()
        {
            m_culture = new CultureInfo(CultureInfo.CurrentCulture.Name);
        }

        /// <summary>
        /// Gets the size of the default paper.
        /// </summary>
        /// <value>The size of the default paper.</value>
        [Documentation.DocumentationExclude()]
        [Browsable(false)]
        [EditorBrowsable(EditorBrowsableState.Never)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public static PaperSize DefaultPaperSize
        {
            get { return new PaperSize("Custom Size", 850, 1100); }
        }
        #endregion

        #region Class utility methods
        /// <summary>
        /// Initializes the header and footer layout using the specified paper size and page margins.
        /// </summary>
        /// <param name="papersize">A <see cref="System.Drawing.Printing.PaperSize"/> value.</param>
        /// <param name="pagemargins">A <see cref="System.Drawing.Printing.Margins"/> value.</param>
        public void InitializeHeaderFooterBounds(PaperSize papersize, Margins pagemargins)
        {
            this.Header.Bounds.PageWidth = papersize.Width - pagemargins.Left - pagemargins.Right;
            this.Footer.Bounds.PageWidth = papersize.Width - pagemargins.Left - pagemargins.Right;
        }

        /// <summary>
        /// Initializes the header and footer using the paper size and margins specified by the PageSettings value.
        /// </summary>
        /// <param name="pagesettings">A <see cref="System.Drawing.Printing.PageSettings"/> value.</param>
        public void InitializeHeaderFooterBounds(PageSettings pagesettings)
        {
            if (pagesettings.PrinterSettings.IsValid)
            {
                int nWidth;

                if (pagesettings.Landscape)
                    nWidth = pagesettings.PaperSize.Height;
                else
                    nWidth = pagesettings.PaperSize.Width;

                this.Header.Bounds.PageWidth = nWidth - pagesettings.Margins.Left - pagesettings.Margins.Right;
                this.Footer.Bounds.PageWidth = nWidth - pagesettings.Margins.Left - pagesettings.Margins.Right;
            }
        }

        /// <summary>
        /// Populates a <see cref="T:System.Runtime.Serialization.SerializationInfo"/> with the data needed to serialize the target object.
        /// </summary>
        /// <param name="info">The <see cref="T:System.Runtime.Serialization.SerializationInfo"/> to populate with data.</param>
        /// <param name="context">The destination (see <see cref="T:System.Runtime.Serialization.StreamingContext"/>) for this serialization.</param>
        /// <exception cref="T:System.Security.SecurityException">The caller does not have the required permission. </exception>
        [Documentation.DocumentationExclude()]
        protected virtual void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            info.AddValue("header", m_header);
            info.AddValue("footer", m_footer);
        }
        #endregion

        #region ISerializable
        /// <summary>
        /// Populates a <see cref="T:System.Runtime.Serialization.SerializationInfo"/> with the data needed to serialize the target object.
        /// </summary>
        /// <param name="info">The <see cref="T:System.Runtime.Serialization.SerializationInfo"/> to populate with data.</param>
        /// <param name="context">The destination (see <see cref="T:System.Runtime.Serialization.StreamingContext"/>) for this serialization.</param>
        /// <exception cref="T:System.Security.SecurityException">The caller does not have the required permission. </exception>
        [Documentation.DocumentationExclude()]
        void ISerializable.GetObjectData(SerializationInfo info, StreamingContext context)
        {
            this.GetObjectData(info, context);
        }
        #endregion

        #region ICloneable Members
        /// <summary>
        /// Creates a new object that is a copy of the current instance.
        /// </summary>
        /// <returns>
        /// A new object that is a copy of this instance.
        /// </returns>
        public object Clone()
        {
            return new HeaderFooterData(this);
        }
        #endregion
    }
}
