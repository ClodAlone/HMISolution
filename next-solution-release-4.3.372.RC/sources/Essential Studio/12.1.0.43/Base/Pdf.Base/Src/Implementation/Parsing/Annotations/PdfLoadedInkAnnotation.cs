#region Copyright Syncfusion Inc. 2001 - 2014
// Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Any infringement will be prosecuted under
// applicable laws. 
#endregion
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Text;
using Syncfusion.Pdf.Graphics;
using Syncfusion.Pdf.IO;
using Syncfusion.Pdf.Primitives;

namespace Syncfusion.Pdf.Interactive
{
    /// <summary>
    /// Represent Loaded ink annotation
    /// </summary>
    public class PdfLoadedInkAnnotation : PdfLoadedStyledAnnotation
    {
        # region Fields
        /// <summary>
        /// Cross table
        /// </summary>
        private PdfCrossTable m_crossTable;
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
        /// Get or sets back the Path of the ink annotation
        /// </summary>
        public List<float> InkList
        {
            get
            {
                return GetInkList();
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
                return GetBorderWidth();
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
                return GetLineBorder();
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
        /// get or sets DashArray value
        /// </summary>
        public int[] DashArray
        {
            get
            {
                return GetDashArray();
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
        /// Initializes a new instance of the class
        /// </summary>
        /// <param name="dictionary">The Dictionary</param>
        /// <param name="crossTable">The Crosstable</param>
        /// <param name="rectangle">the Rectangle</param>
        internal PdfLoadedInkAnnotation(PdfDictionary dictionary, PdfCrossTable crossTable, RectangleF rectangle)
            : base(dictionary, crossTable)
        {

            Dictionary = dictionary;
            m_crossTable = crossTable;
            if (Dictionary.ContainsKey(DictionaryProperties.BS))
                m_borderDic = m_crossTable.GetObject(Dictionary[DictionaryProperties.BS]) as PdfDictionary;
        }
        #endregion

        #region Implementations
        /// <summary>
        /// Get the Path of the Ink annotation
        /// </summary>
        /// <returns>the line InkList </returns>
        private List<float> GetInkList()
        {
            List<float> path = new List<float>();
            PdfArray inkList = null;
            if (Dictionary.ContainsKey(DictionaryProperties.InkList))
            {
                inkList = m_crossTable.GetObject(Dictionary[DictionaryProperties.InkList]) as PdfArray;
                PdfArray temp = m_crossTable.GetObject(inkList[0]) as PdfArray;
                if (temp != null)
                {
                    foreach (PdfNumber value in temp)
                    {
                        path.Add(value.FloatValue);
                    }
                }

            }
            return path;
        }
        /// <summary>
        /// Get the border width
        /// </summary>
        /// <returns>The border width</returns>
        private int GetBorderWidth()
        {
            int width = 1;
            if (Dictionary.ContainsKey(DictionaryProperties.BS))
            {
                PdfDictionary lbDic = m_crossTable.GetObject(Dictionary[DictionaryProperties.BS]) as PdfDictionary;

                if (lbDic.ContainsKey(DictionaryProperties.W))
                    width = (lbDic[DictionaryProperties.W] as PdfNumber).IntValue;

            }

            return width;
        }
        /// <summary>
        /// Get the border dtyle
        /// </summary>
        /// <returns>The Line border style</returns>
        private PdfLineBorderStyle GetLineBorder()
        {
            PdfLineBorderStyle style = PdfLineBorderStyle.Solid;
            if (Dictionary.ContainsKey(DictionaryProperties.BS))
            {
                PdfDictionary lbDic = m_crossTable.GetObject(Dictionary[DictionaryProperties.BS]) as PdfDictionary;

                if (lbDic.ContainsKey(DictionaryProperties.S))
                {
                    PdfName bstr = lbDic[DictionaryProperties.S] as PdfName;
                    style = GetBorderStyle(bstr.Value.ToString());
                }

            }
            return style;
        }

        /// <summary>
        /// get the Line style 
        /// </summary>
        /// <param name="bstyle"></param>
        /// <returns>The line Style</returns>
        private PdfLineBorderStyle GetBorderStyle(string bstyle)
        {
            PdfLineBorderStyle style = PdfLineBorderStyle.Solid;
            switch (bstyle)
            {
                case "S":
                    style = PdfLineBorderStyle.Solid;
                    break;
                case "D":
                    style = PdfLineBorderStyle.Dashed;
                    break;
                case "B":
                    style = PdfLineBorderStyle.Beveled;
                    break;
                case "I":
                    style = PdfLineBorderStyle.Inset;
                    break;
                case "U":
                    style = PdfLineBorderStyle.Underline;
                    break;
            }
            return style;
        }
        
        /// <summary>
        /// Get the Dash array value
        /// </summary>
        /// <returns>The dashArray </returns>
        private int[] GetDashArray()
        {
            List<int> temp = new List<int>();
            PdfArray m_dasharray = null;
            if (Dictionary.ContainsKey(DictionaryProperties.BS))
            {
                PdfDictionary lbDic = m_crossTable.GetObject(Dictionary[DictionaryProperties.BS]) as PdfDictionary;
                if (lbDic.ContainsKey(DictionaryProperties.D))
                {
                    m_dasharray = m_crossTable.GetObject(lbDic[DictionaryProperties.D]) as PdfArray;
                    for (int i = 0; i < m_dasharray.Count; i++)
                    {
                        temp.Add((m_dasharray[i] as PdfNumber).IntValue);
                    }
                }
            }
            return temp.ToArray();
        }
        #endregion
    }
}

