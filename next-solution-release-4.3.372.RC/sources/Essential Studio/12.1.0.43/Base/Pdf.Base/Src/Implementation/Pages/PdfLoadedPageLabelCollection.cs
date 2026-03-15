#region Copyright Syncfusion Inc. 2001 - 2014
// Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Any infringement will be prosecuted under
// applicable laws. 
#endregion

using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using Syncfusion.Pdf.Primitives;

namespace Syncfusion.Pdf
{
    /// <summary>
    /// This class allows to manipulate with page
    /// labels of one of the sections.
    /// </summary>
    public class PdfLoadedPageLabelCollection : IPdfWrapper
    {
        #region Fields
        /// <summary>
        /// Local variable to store the PageLabel Count.
        /// </summary>
        private int m_count;
        /// <summary>
        /// Internal variable to store page Label.
        /// </summary>
        private List<PdfPageLabel> m_pageLabel = new List<PdfPageLabel>();
        /// <summary>
        /// Internal variable to store page Label Collection.
        /// </summary>
        private List<PdfReferenceHolder> m_pageLabelCollection = new List<PdfReferenceHolder>();
        #endregion


        #region Properties
        /// <summary>
        /// Gets the count.
        /// </summary>
        /// <value>The count.</value>
        public int Count
        {
            get
            {
                return this.m_count;
            }
        }

        /// <summary>
        /// Gets the <see cref="Syncfusion.Pdf.PdfPageLabel"/> at the specified index.
        /// </summary>
        /// <value></value>
        public PdfPageLabel this[int index]
        {
            get
            {
                return (this.m_pageLabel[index] as PdfPageLabel);
            }
        }
        #endregion

        #region Implementation
        /// <summary>
        /// Add the Page Label.
        /// </summary>
        /// <param name="pageLabel"></param>
        public void Add(PdfPageLabel pageLabel)
        {
            if (pageLabel == null)
            {
                throw new ArgumentNullException("section");
            }
            PdfReferenceHolder holder = new PdfReferenceHolder(pageLabel);
            this.m_pageLabel.Add(pageLabel);
            this.m_pageLabelCollection.Add(holder);
            this.m_count++;
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
                return null;
            }
        }
        #endregion
    }


}
