#region Copyright Syncfusion Inc. 2001 - 2014

//// Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
////  Use of this code is subject to the terms of our license.
////  A copy of the current license can be obtained at any time by e-mailing
////  licensing@syncfusion.com. Re-distribution in any form is strictly
////  prohibited. Any infringement will be prosecuted under applicable laws. 
////
#endregion

#region file using directives
using System;
using System.Collections;
using System.Drawing;

using Syncfusion.Windows.Forms.HTMLUI.Implementation.Utility.Selection;
#endregion

namespace Syncfusion.Windows.Forms.HTMLUI.Implementation.Collections
{
    /// <summary>
    /// Collection storing symbols and their bounds in the document  
    /// useful for text selection.
    /// </summary>
    internal class SymbolCollection : DictionaryBase
    {
        #region Class constants

        /// <summary>
        /// Default number for selection and range numbers.
        /// </summary>
        private const int DEF_NUMBER = -1;
        #endregion

        #region Class members

        /// <summary>
        /// Min number of symbol's start number in the collection.
        /// </summary>
        private int m_minNumber = DEF_NUMBER;

        /// <summary>
        /// Max number of symbol's start number in the collection.
        /// </summary>
        private int m_maxNumber = DEF_NUMBER;
        #endregion

        #region Class properties

        /// <summary>
        /// Returns the information about the symbol at the specified point.
        /// </summary>
        /// <param name="point">A Point instance</param>
        public LetterPair this[Point point]
        {
            get
            {
                return SearchValue(point);
            }
        }

        /// <summary>
        /// Returns the information about the symbol by it's start number in the parent element.
        /// </summary>
        /// <param name="symbolNumber">An integer value</param>
        public LetterPair this[int symbolNumber]
        {
            get
            {
                return SearchValue(symbolNumber);
            }
        }

        /// <summary>
        /// Gets the Min number of the symbol's start number in the collection.
        /// </summary>
        public int MinNumber
        {
            get
            {
                return m_minNumber;
            }
        }

        /// <summary>
        /// Gets the Max number of the symbol's start number in the collection.
        /// </summary>
        public int MaxNumber
        {
            get
            {
                return m_maxNumber;
            }
        }
        #endregion

        #region Class Initialize/Finalize methods

        /// <summary>
        /// Initializes a new instance of the SymbolCollection class
        /// </summary>
        public SymbolCollection()
            : base()
        {
        }
        #endregion

        #region Class Public Methods

        /// <summary>
        /// Adds information about a symbol to the collection.
        /// </summary>
        /// <param name="startNumber">Start number of the symbol in the element.</param>
        /// <param name="info">Information about the symbol.</param>
        public void Add(int startNumber, LetterPair info)
        {
            if (info == null)
                throw new ArgumentNullException("info");

            // Assign min / max numbers.
            if (this.InnerHashtable.Keys.Count == 0)
            {
                m_minNumber = startNumber;
                m_maxNumber = startNumber;
            }
            else
            {
                m_minNumber = Math.Min(m_minNumber, startNumber);
                m_maxNumber = Math.Max(m_maxNumber, startNumber);
            }

            this.InnerHashtable[startNumber] = info;
        }

        /// <summary>
        /// Removes the element from the collection.
        /// </summary>
        /// <param name="key">Start number of the symbol in the element.</param>
        public void Remove(int key)
        {
            LetterPair value = this.InnerHashtable[key] as LetterPair;

            if (value != null)
            {
                this.InnerHashtable.Remove(key);

                // Refresh min / max numbers.
                if (this.InnerHashtable.Count == 0)
                {
                    m_minNumber = DEF_NUMBER;
                    m_maxNumber = DEF_NUMBER;
                }
                else if (key == m_minNumber)
                {
                    m_minNumber++;
                }
                else if (key == m_maxNumber)
                {
                    m_maxNumber--;
                }
            }
        }

        /// <summary>
        /// Clears the collection.
        /// </summary>
        public new void Clear()
        {
            base.Clear();

            m_minNumber = DEF_NUMBER;
            m_maxNumber = DEF_NUMBER;
        }
        #endregion

        #region Class utility methods

        /// <summary>
        /// Overloaded. Searches information about the symbol at the specified point.
        /// </summary>
        /// <param name="point">Location of the symbol.</param>
        /// <returns>Information about the symbol if found; Null otherwise.</returns>
        private LetterPair SearchValue(PointF point)
        {
            LetterPair result = null;

            foreach (LetterPair keyPair in this.InnerHashtable.Values)
            {
                if (keyPair.Bounds.Contains(point))
                {
                    result = keyPair;
                    break;
                }
            }

            return result;
        }

        /// <summary>
        /// Searches information about the symbol by it's number in the element.
        /// </summary>
        /// <param name="startPoint">Start number of the symbol.</param>
        /// <returns>Information about the symbol if found; Null otherwise.</returns>
        private LetterPair SearchValue(int startPoint)
        {
            LetterPair result = this.InnerHashtable[startPoint] as LetterPair;

            return result;
        }
        #endregion
    }
}
