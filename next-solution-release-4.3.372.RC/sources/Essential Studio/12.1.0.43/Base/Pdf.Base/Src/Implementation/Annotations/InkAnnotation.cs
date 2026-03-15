#region Copyright Syncfusion Inc. 2001 - 2014
// Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Any infringement will be prosecuted under
// applicable laws. 
#endregion
using System;
using System.Drawing;
using Syncfusion.Pdf.IO;
using Syncfusion.Pdf.Primitives;
using Syncfusion.Pdf.Graphics;
using System.Collections.Generic;

namespace Syncfusion.Pdf.Interactive
{
    /// <summary>
    ///// Represent the Ink Annotation class
    /// </summary>
    public class PdfInkAnnotation : PdfAnnotation
    {
        # region Fields
        /// <summary>
        /// Indicate the path of the ink annotation
        /// </summary>
        private List<float> m_inkList = null;
        /// <summary>
        /// Internal variable to store Border Dash.
        /// </summary>
        private int[] m_dashArray;
        /// <summary>
        /// border width
        /// </summary>
        private int m_borderWidth = 1;
        /// <summary>
        /// Indicate the border Dictionary
        /// </summary>
        private PdfDictionary m_borderDic = new PdfDictionary();
        /// <summary>
        /// Indicat the border style
        /// </summary>
        private PdfLineBorderStyle m_borderStyle = PdfLineBorderStyle.Solid;
        #endregion

        #region Properties
        /// <summary>
        /// Get or sets back the InkList value
        /// </summary>
        public List<float> InkList
        {
            get
            {
                return m_inkList;
            }
            set
            {
                m_inkList = value;
                PdfArray inkList = new PdfArray(m_inkList.ToArray());
                PdfArray temp = new PdfArray();
                temp.Add(new PdfReferenceHolder(inkList));
                Dictionary.SetProperty(DictionaryProperties.InkList, new PdfArray(temp));
            }
        }
        /// <summary>
        /// Get or sets back the border width
        /// </summary>
        public int BorderWidth
        {
            get
            {
                return m_borderWidth;
            }
            set
            {
                m_borderWidth = value;
                m_borderDic.SetProperty(DictionaryProperties.W, new PdfNumber(m_borderWidth));
            }
        }
        /// <summary>
        /// get or sets the border style
        /// </summary>
        public PdfLineBorderStyle BorderStyle
        {
            get
            {
                return m_borderStyle;
            }
            set
            {
                m_borderStyle = value;
                if (m_borderStyle == PdfLineBorderStyle.Solid)
                    m_borderDic.SetProperty(DictionaryProperties.S, new PdfName("S"));
                else if (m_borderStyle == PdfLineBorderStyle.Inset)
                    m_borderDic.SetProperty(DictionaryProperties.S, new PdfName("I"));
                else if (m_borderStyle == PdfLineBorderStyle.Dashed)
                    m_borderDic.SetProperty(DictionaryProperties.S, new PdfName("D"));
                else if (m_borderStyle == PdfLineBorderStyle.Beveled)
                    m_borderDic.SetProperty(DictionaryProperties.S, new PdfName("B"));
                else if (m_borderStyle == PdfLineBorderStyle.Underline)
                    m_borderDic.SetProperty(DictionaryProperties.S, new PdfName("U"));
            }
        }
        /// <summary>
        /// Get or sets the DashArry value
        /// </summary>
        public int[] DashArray
        {
            get
            {
                return this.m_dashArray;
            }

            set
            {
                this.m_dashArray = value;
                PdfArray m_dasharray = new PdfArray(m_dashArray);
                m_borderDic.SetProperty(DictionaryProperties.D, m_dasharray);
            }
        }
        #endregion

        #region Constructor
        /// <summary>
        /// Create the instance of the class
        /// </summary>
        /// <param name="rectangle"></param>
        /// <param name="linePoints"></param>
        public PdfInkAnnotation(RectangleF rectangle, List<float> linePoints)
            : base(rectangle)
        {


            InkList = linePoints;

        }
        #endregion

        #region Implementations
        /// <summary>
        /// Initializes annotation object.
        /// </summary>
        protected override void Initialize()
        {
            base.Initialize();
            Dictionary.SetProperty(DictionaryProperties.Subtype, new PdfName(DictionaryProperties.Ink));
        }

        /// <summary>
        /// Saves an annotation.
        /// </summary>
        protected override void Save()
        {
            base.Save();
            m_borderDic.SetProperty(DictionaryProperties.Type, new PdfName(DictionaryProperties.Border));

            Dictionary.SetProperty(DictionaryProperties.BS, new PdfReferenceHolder(m_borderDic));
        }
        #endregion
    }
}
