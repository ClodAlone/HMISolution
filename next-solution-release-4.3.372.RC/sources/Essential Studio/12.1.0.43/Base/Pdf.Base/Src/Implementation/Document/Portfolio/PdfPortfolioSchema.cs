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
using System.Collections.Generic;

namespace Syncfusion.Pdf
{
    
    public class PdfPortfolioSchema:IPdfWrapper
    {
       #region Fields
        /// <summary>
        /// Internal variable to store dictionary;
        /// </summary>
        private PdfDictionary m_dictionary = new PdfDictionary();

        /// <summary>
        /// Varible to store Schema field
        /// </summary>
       private  PdfPortfolioSchemaField m_schemaField;

       /// <summary>
       /// internal variable to store field keys
       /// </summary>
       private string[] fieldkeys;

       /// <summary>
       /// Internal variable to store field collections
       /// </summary>
       private Dictionary<string, PdfPortfolioSchemaField> m_fieldCollections = new Dictionary<string, PdfPortfolioSchemaField>();
        #endregion

       #region Property

       /// <summary>
       /// Get the field keys from schema field
       /// </summary>
       public string[] FieldKeys
       {
          get
          {
           string[] array = new string[m_fieldCollections.Count];
           m_fieldCollections.Keys.CopyTo(array,0);
           return array;
          }
       }
       #endregion

       #region constructor
        /// <summary>
        /// Initialze the instance of the class
        /// </summary>
       public PdfPortfolioSchema()
            : base()
        {
           
            Initialize();
        }

        /// <summary>
        /// intialze the instance of the class
        /// </summary>
        /// <param name="schemaDictionary"></param>
       internal PdfPortfolioSchema(PdfDictionary schemaDictionary)
       {
           m_dictionary = schemaDictionary;

           if(m_dictionary!=null)
           {

                   foreach (KeyValuePair<PdfName, IPdfPrimitive> value in m_dictionary.Items)
                   {
                       if (((value.Key) as PdfName).Value == DictionaryProperties.Type)
                       {
                           continue;
                       }

                       PdfDictionary schemaField = m_dictionary[value.Key] as PdfDictionary;
                     
                       if (schemaField != null)
                       {
                           m_schemaField = new PdfPortfolioSchemaField(schemaField);
                           if (m_schemaField != null)
                           {
                               m_fieldCollections.Add(m_schemaField.Name, m_schemaField);
                           }
                       }

                   }
               
           }

       }
       #endregion

       #region implementation
       /// <summary>
       /// used to add the schemafield into schema dictionary
       /// </summary>
       /// <param name="field"></param>
       public void AddSchemaField(PdfPortfolioSchemaField field)
        {
            if (!m_fieldCollections.ContainsKey(field.Name) && !m_dictionary.ContainsKey(field.Name))
            {
                m_fieldCollections.Add(field.Name, field);
                m_dictionary.SetProperty(field.Name,field);
            }

        }

        /// <summary>
        /// used to remove the schemafield from schema dictionary
        /// </summary>
        /// <param name="key"></param>
        public void RemoveField(string key)
        {
            if (m_fieldCollections.ContainsKey(key) && m_dictionary.ContainsKey(key))
            {
                m_fieldCollections.Remove(key);
                m_dictionary.Remove(key);
            }

        }


        /// <summary>
        /// Initializes instance.
        /// </summary>
        private void Initialize()
        {
            m_dictionary.SetProperty(DictionaryProperties.Type, new PdfName(DictionaryProperties.CollectionSchema));

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
