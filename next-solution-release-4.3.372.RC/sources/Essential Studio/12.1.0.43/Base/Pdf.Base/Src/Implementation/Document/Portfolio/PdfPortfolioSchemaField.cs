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
using System.Collections.Generic;

namespace Syncfusion.Pdf
{
    public class PdfPortfolioSchemaField : IPdfWrapper
    {
        #region Fields
        /// <summary>
        /// variable to store editable value
        /// </summary>
        private bool m_editable = false;

        /// <summary>
        /// Variable to store name of the schema field
        /// </summary>
        private string m_name;

        /// <summary>
        /// variable to store the order of the schema field
        /// </summary>
        private int m_order;

        /// <summary>
        /// variable to store visibility of schema field
        /// </summary>
        private bool m_visible = true;


        private PdfPortfolioSchemaFieldType m_type;
        /// <summary>
        /// Internal variable to store dictionary;
        /// </summary>
        private PdfDictionary m_dictionary = new PdfDictionary();

        #endregion

        #region Property

        /// <summary>
        /// Gets or Sets the Editable value
        /// </summary>
        public bool Editable
        {

            get
            {
                return m_editable;
            }
            set
            {
                m_editable = value;
                m_dictionary.SetBoolean(DictionaryProperties.E, m_editable);
            }
        }

        /// <summary>
        /// Gets or Sets the name of the schema field
        /// </summary>
        public string Name
        {
            get
            {
                return m_name;
            }
            set
            {
                m_name = value;
                m_dictionary.SetProperty(DictionaryProperties.N, new PdfString(m_name));
            }

        }

        /// <summary>
        /// Gets or sets the Order of the Schema field
        /// </summary>
        public int Order
        {
            get
            {
                return m_order;
            }
            set
            {
                m_order = value;
                m_dictionary.SetNumber(DictionaryProperties.O, m_order);
            }
        }

        /// <summary>
        /// Gets or Sets the schema field Type
        /// </summary>
        public PdfPortfolioSchemaFieldType Type
        {

            get
            {
                return m_type;
            }
            set
            {
                m_type = value;
                if (m_type == PdfPortfolioSchemaFieldType.String)
                    m_dictionary.SetName(DictionaryProperties.Subtype, DictionaryProperties.S);
                else if (m_type == PdfPortfolioSchemaFieldType.Date)
                    m_dictionary.SetName(DictionaryProperties.Subtype, DictionaryProperties.D);
                else if (m_type == PdfPortfolioSchemaFieldType.Number)
                    m_dictionary.SetName(DictionaryProperties.Subtype, DictionaryProperties.N);
            }
        }

        /// <summary>
        /// Gets or Sets the visiblity of the Schema field
        /// </summary>
        public bool Visible
        {
            get
            {
                return m_visible;
            }
            set
            {
                m_visible = value;
                m_dictionary.SetBoolean(DictionaryProperties.V, m_visible);

            }
        }
        #endregion

        #region constructor
        /// <summary>
        /// initialize the instance of the class
        /// </summary>
        public PdfPortfolioSchemaField()
            : base()
        {
            Initialize();
        }
        /// <summary>
        /// initialize the instance of the class
        /// </summary>
        /// <param name="schemaField">scemafield dictionary</param>
        internal PdfPortfolioSchemaField(PdfDictionary schemaField)
        {
            m_dictionary = schemaField;

            if(m_dictionary.ContainsKey(DictionaryProperties.N))
            {
                this.Name = (m_dictionary[DictionaryProperties.N] as PdfString).Value;
            }
            if (m_dictionary.ContainsKey(DictionaryProperties.O))
            {
                this.Order = (m_dictionary[DictionaryProperties.O] as PdfNumber).IntValue;
            }
            if (m_dictionary.ContainsKey(DictionaryProperties.V))
            {
                this.Visible = (m_dictionary[DictionaryProperties.V] as PdfBoolean).Value;
            }
            if (m_dictionary.ContainsKey(DictionaryProperties.E))
            {
                this.Editable = (m_dictionary[DictionaryProperties.E] as PdfBoolean).Value;
            }

            if (m_dictionary.ContainsKey(DictionaryProperties.Subtype))
            {
                string filedtype=(m_dictionary[DictionaryProperties.Subtype] as PdfName).Value;
                if(filedtype.Equals(DictionaryProperties.S))
                {
                
                this.Type=PdfPortfolioSchemaFieldType.String;
                }
                else if(filedtype.Equals(DictionaryProperties.Size))
                {
                    this.Type=PdfPortfolioSchemaFieldType.Size;
                }
                else if(filedtype.Equals(DictionaryProperties.N))
                {
                    this.Type=PdfPortfolioSchemaFieldType.Number;
                }
                else if (filedtype.Equals(DictionaryProperties.D))
                {
                    this.Type = PdfPortfolioSchemaFieldType.Date;
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
            m_dictionary.SetProperty(DictionaryProperties.Type, new PdfName(DictionaryProperties.CollectionField));
            m_dictionary.SetBoolean(DictionaryProperties.V, true);
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
