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
   public class PdfPortfolioAttributes : IPdfWrapper
   {
       #region Fields
       /// <summary>
        /// Internal variable to store dictionary;
        /// </summary>
       private PdfDictionary m_dictionary = null;

       /// <summary>
       /// Internal varible to store attribute keys
       /// </summary>
       private string[] m_attributeKeys;

      /// <summary>
      /// Internal variable to store attributes
      /// </summary>
       private Dictionary<string, string> m_attributes = new Dictionary<string, string>();

       #endregion

       #region Constructor
       /// <summary>
       /// Initialize the instance of the class
       /// </summary>
        public PdfPortfolioAttributes()
            : base()
        {
            Initialize();
        }

      
       /// <summary>
        /// Initialize the instance of the class
       /// </summary>
       /// <param name="dictionary"></param>
 
       internal PdfPortfolioAttributes(PdfDictionary dictionary)
        {
            if (m_dictionary == null)
            {
                m_dictionary = dictionary;
               foreach(KeyValuePair<PdfName,IPdfPrimitive> value in dictionary.Items)
               {
                   if (((value.Key) as PdfName).Value == DictionaryProperties.Type)
                   {
                       continue;
                   }
                   m_attributes.Add(((value.Key) as PdfName).Value, ((value.Value) as PdfString).Value);
               }
            }
        }
        
        #endregion

       #region Property
       /// <summary>
       /// Get Attribute keys
       /// </summary>
 
       public string[] AttributesKey
        {
           get
            {
                string[] array = new string[m_attributes.Count];
                m_attributes.Keys.CopyTo(array, 0);
                return array;
            }

        }
       #endregion 

       #region Implementation
       /// <summary>
        /// Initializes instance.
        /// </summary>
        private void Initialize()
        {
            m_dictionary = new PdfDictionary();
            m_dictionary.SetProperty(DictionaryProperties.Type, new PdfName(DictionaryProperties.CollectionItem));

        }
       
       /// <summary>
       /// Add the attributes into portfolio
       /// </summary>
       /// <param name="key"></param>
       /// <param name="value"></param>
       public void AddAttributes(string key, string value)
        {
            if (!m_attributes.ContainsKey(key) && !m_dictionary.ContainsKey(key))
            {
                m_attributes.Add(key, value);
                m_dictionary.SetProperty(key, new PdfString(value));
            }
        }

       /// <summary>
       /// Remove the attributes from portfolio
       /// </summary>
       /// <param name="key"></param>
        public void RemoveAttributes(string key)
        {
            if (m_attributes.ContainsKey(key) && m_dictionary.ContainsKey(key))
            {
                m_attributes.Remove(key);
                m_dictionary.Remove(key);
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
