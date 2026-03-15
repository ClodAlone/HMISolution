#region Copyright Syncfusion Inc. 2001 - 2014
// Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Any infringement will be prosecuted under
// applicable laws. 
#endregion

using System;

using Syncfusion.Pdf.IO;
using Syncfusion.Pdf.Primitives;

namespace Syncfusion.Pdf
{
    /// <summary>
    /// Defines additional parameters for the embedded file.
    /// </summary>
    internal class EmbeddedFileParams : IPdfWrapper
    {
        #region Fields
        /// <summary>
        /// Internal variable to store creation date.
        /// </summary>
        private DateTime m_creationDate = DateTime.Now;

        /// <summary>
        /// Internal variable to store modification date.
        /// </summary>
        private DateTime m_modificationDate = DateTime.Now;

        /// <summary>
        /// Internal variable to store size of the embedded file.
        /// </summary>
        private int m_size;

        /// <summary>
        /// Internal variable to store dictionary.
        /// </summary>
        private PdfDictionary m_dictionary = new PdfDictionary();
        #endregion

        #region Constructors
        /// <summary>
        /// Initializes a new instance of the <see cref="EmbeddedFileParams"/> class.
        /// </summary>
        public EmbeddedFileParams()
            : base()
        {
            CreationDate = DateTime.Now;
            ModificationDate = DateTime.Now;
        }
        #endregion

        #region Properties
        /// <summary>
        /// Gets or sets creation date.
        /// </summary>
        /// <value>Creation date.</value>
        public DateTime CreationDate
        {
            get
            {
                return m_creationDate;
            }

            set
            {
                m_creationDate = value;
                m_dictionary.SetDateTime(DictionaryProperties.CreationDate, value);
            }
        }

        /// <summary>
        /// Gets or sets modification date.
        /// </summary>
        /// <value>Modification date.</value>
        public DateTime ModificationDate
        {
            get
            {
                return m_modificationDate;
            }

            set
            {
                m_modificationDate = value;
                m_dictionary.SetDateTime(DictionaryProperties.ModificationDate, value);
            }
        }

        /// <summary>
        /// Gets or sets the size of the embedded file.
        /// </summary>
        /// <value>The size.</value>
        internal int Size
        {
            get
            {
                return m_size;
            }

            set
            {
                if (m_size != value)
                {
                    m_size = value;
                    m_dictionary.SetNumber(DictionaryProperties.Size, m_size);
                }
            }
        }
        #endregion

        #region IPdfWrapper Members
        /// <summary>
        /// Gets the element.
        /// </summary>
        /// <value></value>
        IPdfPrimitive IPdfWrapper.Element
        {
            get
            {
                return m_dictionary;
            }
        }
        #endregion
    }
}
