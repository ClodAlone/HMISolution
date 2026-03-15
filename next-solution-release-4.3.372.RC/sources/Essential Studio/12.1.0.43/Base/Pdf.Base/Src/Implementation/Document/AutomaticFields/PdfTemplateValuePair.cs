#region Copyright Syncfusion Inc. 2001 - 2014
// Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Any infringement will be prosecuted under
// applicable laws. 
#endregion

using System;

using Syncfusion.Pdf.Graphics;

/// <summary>
/// The Syncfusion.Pdf namespace contains classes for creating PDF document.
/// </summary>
namespace Syncfusion.Pdf
{
    /// <summary>
    /// Represent class to store information about template and value pairs 
    /// used in automatic fields.
    /// </summary>
    internal class PdfTemplateValuePair
    {
        #region Fields
        /// <summary>
        /// Internal variable to store template.
        /// </summary>
        private PdfTemplate m_template = null;

        /// <summary>
        /// Intenal variable to store value.
        /// </summary>
        private string m_value = String.Empty;
        #endregion

        #region Constructros
        /// <summary>
        /// Initializes a new instance of the <see cref="PdfTemplateValuePair"/> class.
        /// </summary>
        public PdfTemplateValuePair()
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="PdfTemplateValuePair"/> class.
        /// </summary>
        /// <param name="template">The template.</param>
        /// <param name="value">The value.</param>
        public PdfTemplateValuePair(PdfTemplate template, string value)
        {
            Template = template;
            Value = value;
        }
        #endregion

        #region Properties
        /// <summary>
        /// Gets or sets the template.
        /// </summary>
        /// <value>The template.</value>
        public PdfTemplate Template
        {
            get
            {
                return m_template;
            }

            set
            {
                if (value == null)
                {
                    throw new ArgumentNullException("Template");
                }

                m_template = value;
            }
        }

        /// <summary>
        /// Gets or sets the value.
        /// </summary>
        /// <value>The value.</value>
        public string Value
        {
            get
            {
                return m_value;
            }

            set
            {
                if (m_value == null)
                {
                    throw new ArgumentNullException("Value");
                }

                m_value = value;
            }
        }
        #endregion
    }
}
