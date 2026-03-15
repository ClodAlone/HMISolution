#region Copyright Syncfusion Inc. 2001 - 2014
// Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Any infringement will be prosecuted under
// applicable laws. 
#endregion
using System;
using System.Collections;

using Syncfusion.Pdf;
using Syncfusion.Pdf.IO;
using Syncfusion.Pdf.Parsing;
using Syncfusion.Pdf.Primitives;
using Syncfusion.Pdf.Interactive;
using System.Collections.Generic;

namespace Syncfusion.Pdf.Interactive
{
    /// <summary>
    /// It's a base class for all annotations   
    /// </summary>
    class PdfAnnotationBase : PdfAnnotation
    {
        #region Fields
        /// <summary>
        /// Internal variable to store dictinary.
        /// </summary>
        private PdfDictionary m_dictionary = new PdfDictionary();
        /// <summary>
        /// Cross table of the document;
        /// </summary>
        private PdfCrossTable m_crossTable = new PdfCrossTable();
        /// <summary>
        /// Temp variable to store loaded annotation.
        /// </summary>
        //private ArrayList m_annotations = null;
        /// <summary>
        /// Collection of the descend annotation.
        /// </summary>
        private List<PdfAnnotation> m_list = new List<PdfAnnotation>();
        ///// <summary>
        ///// Collection of fields
        ///// </summary>
        //private PdfAnnotationCollection  m_annotations;

        #endregion

        #region Properties
       
        /// <summary>
        /// Gets an <see cref="T:PdfOutline"/> instance at the specified index.
        /// </summary>
        public PdfAnnotation this[int index]
        {
            get
            {
                return List[index] as PdfAnnotation;
            }
        }

        /// <summary>
        /// Gets the sub items.
        /// </summary>
        internal virtual List<PdfAnnotation> List
        {
            get
            {
                return m_list;
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

        /// <summary>
        /// Gets the cross table.
        /// </summary>
        internal PdfCrossTable CrossTable
        {
            get
            {
                return m_crossTable;
            }
        }
        #endregion
        
        #region Constructors
        /// <summary>
        /// Initializes a new instance of the <see cref="T:PdfAnnotationBase"/> class.
		/// </summary>
		/// <remarks>Note that the Type field shouldn't be generated.</remarks>
        internal PdfAnnotationBase()
			: base()
		{
		}
        	/// <summary>
		/// Initializes a new instance of the <see cref="PdfBookmarkBase"/> class.
		/// </summary>
		/// <param name="dictionary">The dictionary.</param>
		/// <param name="crossTable">The cross table.</param>
        internal PdfAnnotationBase(PdfDictionary dictionary, PdfCrossTable crossTable)
		{
			m_dictionary = dictionary;

			if( crossTable != null )
			{
				m_crossTable = crossTable;
			}
		}
        #endregion

        #region Public methods
        /// <summary>
        /// Creates and adds an outline.
        /// </summary>
        /// <param name="title">The title of the new outline.</param>
        /// <returns>The outline created.</returns>
        public PdfAnnotation Add(string title)
        {
            if (title == null)
                throw new ArgumentNullException("title");

            PdfAnnotation annot = null;
            annot.Text = title;
            
            List.Add(annot);
            UpdateFields();

            return annot;
        }
        #endregion

   
        #region Implementation 
        /// <summary>
        /// Updates all outline dictionary fields.
        /// </summary>
        private void UpdateFields()
        {
            if (List.Count > 0)
            {
                m_dictionary.SetNumber(DictionaryProperties.Count, List.Count);             
            }
            else
            {
                m_dictionary.Clear();
            }

            m_dictionary.Modify();
        }

        #endregion

     
    }
}
