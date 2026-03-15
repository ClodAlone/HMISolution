#region Copyright Syncfusion Inc. 2001 - 2014
// Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Any infringement will be prosecuted under
// applicable laws. 
#endregion
using System;
using Syncfusion.Pdf.Primitives;
using Syncfusion.Pdf.IO;
using Syncfusion.Pdf.Interactive;

namespace Syncfusion.Pdf
{
    public class PdfPortfolioInformation : IPdfWrapper
    {
        #region fields
        /// <summary>
        /// Internal variable to store value specifying document's catalog.
        /// </summary>
        private PdfCatalog m_catalog = null;

        /// <summary>
        /// Internal variable to store dictionary;
        /// </summary>
        private PdfDictionary m_dictionary = new PdfDictionary();

        /// <summary>
        /// internal variable to store schema of portfolio
        /// </summary>
        private PdfPortfolioSchema m_Schema;

        /// <summary>
        /// internal variable to store view mode
        /// </summary>
        private PdfPortfolioViewMode m_viewMode=PdfPortfolioViewMode.Details;

        /// <summary>
        /// internal varible to store startup attachment document
        /// </summary>
        private PdfAttachment m_startupDocument;

        #endregion

        #region Property
        /// <summary>
        /// Gets or sets the portfolio schema field
        /// </summary>
        public PdfPortfolioSchema Schema
        {
            get
            {
                return m_Schema;
            }
            set
            {
                m_Schema = value;
                m_dictionary.SetProperty(DictionaryProperties.Schema, m_Schema);
            }
        }

        /// <summary>
        /// Get and set the viewmode of the portfolio
        /// </summary>
        public PdfPortfolioViewMode ViewMode
        {
            get
            {
                return m_viewMode;
            }
            set
            {
                m_viewMode = value;

                if (m_viewMode == PdfPortfolioViewMode.Details)
                {
                    m_dictionary.SetProperty(DictionaryProperties.View,new PdfName(DictionaryProperties.D));
                }
               else if (m_viewMode == PdfPortfolioViewMode.Hidden)
                {
                    m_dictionary.SetProperty(DictionaryProperties.View,new PdfName( DictionaryProperties.H));
                }
                else if (m_viewMode == PdfPortfolioViewMode.Tile)
                {
                    m_dictionary.SetProperty(DictionaryProperties.View,new PdfName( DictionaryProperties.T));
                }

            }
        }

        /// <summary>
        /// Get and set the startup document of portfolio
        /// </summary>
        public PdfAttachment StartupDocument
        {
            get
            {
                return m_startupDocument;
            }

            set
            {
                m_startupDocument = value;
                m_dictionary.SetProperty(DictionaryProperties.D, new PdfString(m_startupDocument.FileName));
            }
        }
        #endregion

        #region constructor
        /// <summary>
        /// Initializes new  instance.
        /// </summary>
        public PdfPortfolioInformation()
            : base()
        {

            Initialize();
        }
        /// <summary>
        /// Initialize the new instance
        /// </summary>
        /// <param name="portfolioDictionary">portfolio dictionary </param>
        internal PdfPortfolioInformation(PdfDictionary portfolioDictionary)
        {
            if (portfolioDictionary != null)
            {
                m_dictionary = portfolioDictionary;

                PdfDictionary schemaDictionary = m_dictionary[DictionaryProperties.Schema] as PdfDictionary;

                if (schemaDictionary != null)
                {
                    m_Schema = new PdfPortfolioSchema(schemaDictionary);

                }

                PdfName viewMode = m_dictionary[DictionaryProperties.View] as PdfName;

                if (viewMode != null)
                {
                    if(viewMode.Value.Equals(DictionaryProperties.D))
                    {
                       this.ViewMode=PdfPortfolioViewMode.Details;
                    }
                    else if (viewMode.Value.Equals(DictionaryProperties.T))
                    {
                        this.ViewMode = PdfPortfolioViewMode.Tile;
                    }
                    else if (viewMode.Value.Equals(DictionaryProperties.H))
                    {
                        this.ViewMode = PdfPortfolioViewMode.Hidden;
                    }
                }
            }
        }
        #endregion

        #region implementation
        /// <summary>
        /// Initializes instance.
        /// </summary>
        private void Initialize()
        {
            m_dictionary.SetProperty(DictionaryProperties.Type, new PdfName(DictionaryProperties.Collection));

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
