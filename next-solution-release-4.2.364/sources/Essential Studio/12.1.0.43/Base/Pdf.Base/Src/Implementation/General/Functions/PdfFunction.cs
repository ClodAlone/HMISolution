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

namespace Syncfusion.Pdf.Functions
{
    /// <summary>
    /// Implements the base class for all functions.
    /// </summary>
    public abstract class PdfFunction : IPdfWrapper
    {
        #region Properties
        /// <summary>
        /// Internal variable to store dictionary.
        /// </summary>
        private PdfDictionary m_dictionary = null;
        #endregion

        #region Constructors
        /// <summary>
        /// Initializes a new instance of the <see cref="T:PdfFunction"/> class.
        /// </summary>
        /// <param name="dic">The internal dictionary.</param>
        internal PdfFunction(PdfDictionary dic)
        {
            m_dictionary = dic;
        }
        #endregion

        #region Properties
        /// <summary>
        /// Gets or sets the domain of the function.
        /// </summary>
        internal PdfArray Domain
        {
            get
            {
                PdfArray domain = m_dictionary[DictionaryProperties.Domain] as PdfArray;
                return domain;
            }

            set
            {
                if (value == null)
                    throw new ArgumentNullException("Domain");

                m_dictionary.SetProperty(DictionaryProperties.Domain, value);
            }
        }

        /// <summary>
        /// Gets or sets the range.
        /// </summary>
        internal PdfArray Range
        {
            get
            {
                PdfArray range = m_dictionary[DictionaryProperties.Range] as PdfArray;
                return range;
            }

            set
            {
                m_dictionary.SetProperty(DictionaryProperties.Range, value);
            }
        }

        /// <summary>
        /// Gets the dictionary.
        /// </summary>
        /// <value>The dictionary.</value>
        internal PdfDictionary Dictionary
        {
            get
            {
                return m_dictionary;
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
