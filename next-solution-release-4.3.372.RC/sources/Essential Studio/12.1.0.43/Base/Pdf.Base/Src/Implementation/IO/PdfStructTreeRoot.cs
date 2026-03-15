#region Copyright Syncfusion Inc. 2001 - 2014
// Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Any infringement will be prosecuted under
// applicable laws. 
#endregion
using System;
using System.Collections.Generic;
using System.Text;

using Syncfusion.Pdf.Primitives;
using System.Drawing;

namespace Syncfusion.Pdf.IO
{
    /// <summary>
    /// Represents structure tree required to store document logical structure.
    /// </summary>
    internal class PdfStructTreeRoot : PdfDictionary
    {
        # region Fields
        /// <summary>
        /// Internal variable to hold structure elements.
        /// </summary>
        private PdfArray m_childSTR;

        /// <summary>
        /// Internal variable to store PdfPage associated with the element.
        /// </summary>
        private PdfPageBase m_pdfPage;

        /// <summary>
        /// Internal variable to store structure id.
        /// </summary>
        private static int m_id;

        /// <summary>
        /// Internal variable to store the bounding rectangle of the element.
        /// </summary>
        private RectangleF m_BBoxBounds;
        # endregion

        # region Constructor
        /// <summary>
        /// Constructor
        /// </summary>
        public PdfStructTreeRoot()
        {
            this[DictionaryProperties.Type] = new PdfName("StructTreeRoot");
            m_id = 0;
            m_childSTR = new PdfArray();

            // Section implementation removed due to Adobe Reverse order reading issue.
            //PdfDictionary section = new PdfDictionary();
            //section[DictionaryProperties.K] = m_childSTR;
            //section[DictionaryProperties.S] = new PdfName("Sect");

            //this[DictionaryProperties.K] = new PdfReferenceHolder(section);

            this[DictionaryProperties.K] = m_childSTR;
        }
        # endregion

        # region Methods
        /// <summary>
        /// Adds the specified element to the document structure tree.
        /// </summary>
        /// <param name="structType"></param>
        /// <param name="altText"></param>
        /// <param name="page"></param>
        /// <param name="bounds"></param>
        /// <returns></returns>
        internal int Add(string structType, string altText, PdfPageBase page, RectangleF bounds)
        {
            m_pdfPage = page;
            m_BBoxBounds = bounds;
            int id = Add(structType, altText, m_BBoxBounds);
            m_pdfPage = null;

            return id;
        }

        /// <summary>
        /// Adds the specified element to the document structure tree.
        /// </summary>
        /// <param name="structType"></param>
        /// <param name="altText"></param>
        /// <param name="bounds"></param>
        /// <returns></returns>
        internal int Add(string structType, string altText, RectangleF bounds)
        {
            PdfDictionary childStructTreeRoot = new PdfDictionary();
            childStructTreeRoot[DictionaryProperties.S] = new PdfName(structType);
            childStructTreeRoot[DictionaryProperties.P] = new PdfReferenceHolder(this);
            childStructTreeRoot[DictionaryProperties.K] = new PdfNumber(m_id++);
            childStructTreeRoot[DictionaryProperties.Lang] = new PdfString("English"); // default

            // If the element type is paragraph, update alternative text.
            if (structType != "P")
                childStructTreeRoot[DictionaryProperties.Alt] = new PdfString(altText);

            // Adds page reference.
            if (m_pdfPage != null)
                childStructTreeRoot[DictionaryProperties.Pg] = new PdfReferenceHolder(m_pdfPage);

            //// Title for the element.
            //childStructTreeRoot[DictionaryProperties.T] = new PdfString(altText);

            // Adds bounding rectangle.
            PdfDictionary aDictionary = new PdfDictionary();
            float[] arr = new float[] { };
            aDictionary[DictionaryProperties.BBox] = new PdfArray(new float[] { bounds.X, bounds.Y, bounds.Width, bounds.Height });

            if (structType == "P" && bounds != RectangleF.Empty)
                childStructTreeRoot[DictionaryProperties.A] = aDictionary;

            // Add the element details as a indirect reference.
            PdfReferenceHolder childRef = new PdfReferenceHolder(childStructTreeRoot);
            m_childSTR.Add(childRef);

            // Parent Tree for any structure element can contain only Number Trees.
            PdfArray nums = new PdfArray();
            nums.Add(new PdfNumber(0));
            nums.Add(new PdfReferenceHolder(m_childSTR));

            PdfDictionary numsDict = new PdfDictionary();
            numsDict[DictionaryProperties.Nums] = nums;

            this[DictionaryProperties.ParentTree] = new PdfReferenceHolder(numsDict);
            this[DictionaryProperties.ParentTreeNextKey] = new PdfNumber(1);

            return m_id - 1;
        }
        # endregion
    }
}
