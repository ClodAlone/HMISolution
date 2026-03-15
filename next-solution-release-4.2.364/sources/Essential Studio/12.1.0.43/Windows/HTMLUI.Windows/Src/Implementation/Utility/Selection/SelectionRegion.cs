#region Copyright Syncfusion Inc. 2001 - 2014
////  Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
////  Use of this code is subject to the terms of our license.
////  A copy of the current license can be obtained at any time by e-mailing
////  licensing@syncfusion.com. Re-distribution in any form is strictly
////  prohibited. Any infringement will be prosecuted under applicable laws. 
#endregion

#region file using directives
using System;
using System.Collections;
using System.Diagnostics;
using System.Drawing;
#endregion

namespace Syncfusion.Windows.Forms.HTMLUI.Implementation.Utility.Selection
{
    /// <summary>
    /// Utility class. Holds the first and the last selected symbols' indices in the document.
    /// </summary>
    internal class SelectionRegion
    {
        #region Class constants
        /// <summary>
        /// Default index for selected symbol.
        /// </summary>
        private const int DEF_INDEX = -1;
        #endregion

        #region Class members
        /// <summary>
        /// Index of the first symbol in the document.
        /// </summary>
        private int m_firstIndex;

        /// <summary>
        /// Index of the last symbol in the document.
        /// </summary>
        private int m_lastIndex;

        /// <summary>
        /// Index of the symbol from which selection has been started.
        /// </summary>
        private int m_startIndex;

        /// <summary>
        /// Indicates whether selection is forward or backward.
        /// </summary>
        private bool m_bForwardOrder;

        /// <summary>
        /// Collection of elements as keys and their first / last text 
        /// position in the text of the document as values.
        /// </summary>
        private Hashtable m_elementsIndex;
        #endregion

        #region Class static properties
        /// <summary>
        /// Gets a new empty selection region.
        /// </summary>
        public static SelectionRegion Empty
        {
            get
            {
                return new SelectionRegion(DEF_INDEX, DEF_INDEX, DEF_INDEX);
            }
        }
        #endregion

        #region Class properties
        /// <summary>
        /// Gets or sets the start symbol's index in the document.
        /// </summary>
        public int FirstIndex
        {
            get
            {
                return m_firstIndex;
            }
            set
            {
                if (m_firstIndex != value)
                {
                    m_firstIndex = value;
                }
            }
        }

        /// <summary>
        /// Gets or sets the last symbol's index in the document.
        /// </summary>
        public int LastIndex
        {
            get
            {
                return m_lastIndex;
            }
            set
            {
                if (m_lastIndex != value)
                {
                    m_lastIndex = value;
                }
            }
        }

        /// <summary>
        /// Gets a value indicating whether the region is empty.
        /// </summary>
        public bool IsEmpty
        {
            get
            {
                return m_startIndex == DEF_INDEX && m_firstIndex == DEF_INDEX && m_lastIndex == DEF_INDEX;
            }
        }
        #endregion

        #region Class Initialize/Finalize methods
        /// <summary>
        /// Initializes a new instance of the SelectionRegion class
        /// </summary>
        /// <param name="startIndex">First selected symbol's index in the document.</param>
        /// <param name="firstIndex">First selected symbol.</param>
        /// <param name="lastIndex">Last selected symbol's index in the document.</param>
        public SelectionRegion(int startIndex, int firstIndex, int lastIndex)
        {
            m_bForwardOrder = false;
            m_startIndex = startIndex;
            m_firstIndex = firstIndex;
            m_lastIndex = lastIndex;
            m_elementsIndex = new Hashtable();
        }
        #endregion

        #region Class Public Methods
        /// <summary>
        /// Resets the selection indices.
        /// </summary>
        public void ResetSelection()
        {
            m_firstIndex = DEF_INDEX;
            m_startIndex = DEF_INDEX;
            m_lastIndex = DEF_INDEX;
            m_bForwardOrder = false;
        }

        /// <summary>
        /// Resets the data for calculation.
        /// </summary>
        public void ResetCalculation()
        {
            m_elementsIndex.Clear();
        }

        /// <summary>
        /// Adds the specified index of the selected element.
        /// </summary>
        /// <param name="index">Integer index value</param>
        public void AddSymbolIndex(int index)
        {
            // Selection just starting.
            if (this.IsEmpty)
            {
                m_startIndex = index;
                m_firstIndex = index;
                m_lastIndex = index;
            }
            else
            {
                //// check if selection direction has been chaged.
                bool directionChanged = m_bForwardOrder != (index >= m_startIndex);

                //// direction has not been changed.
                if (!directionChanged)
                {
                    if (index < m_firstIndex || index < m_startIndex)
                    {
                        m_firstIndex = index;
                    }
                    else
                    {
                        m_lastIndex = index;
                    }
                }
                else
                {
                    if (index < m_startIndex)
                    {
                        m_firstIndex = index;
                        m_lastIndex = m_startIndex;
                    }
                    else
                    {
                        m_firstIndex = m_startIndex;
                        m_lastIndex = index;
                    }
                }

                m_bForwardOrder = index >= m_startIndex;
            }
        }

        /// <summary>
        /// Assigns a value to the FirstIndex.
        /// </summary>
        /// <param name="index">New first index.</param>
        public void SetFirstIndex(int index)
        {
            this.FirstIndex = index;
        }

        /// <summary>
        /// Assigns a value to the LastIndex.
        /// </summary>
        /// <param name="index">New last index.</param>
        public void SetLastIndex(int index)
        {
            this.LastIndex = index;
        }

        /// <summary>
        /// Sets the first / last index element's text in the text of the document.
        /// </summary>
        /// <param name="element">Element processing.</param>
        /// <param name="startIndex">Current start index.</param>
        /// <param name="lastIndex">Current last index.</param>
        public void SetElementIndex(BaseElement element, int startIndex, int lastIndex)
        {
            if (element == null)
                throw new ArgumentNullException("element");

            object value = m_elementsIndex[element];
            Point indexes = Point.Empty;

            if (value != null)
            {
                indexes = (Point)value;
                indexes.X = Math.Min(indexes.X, startIndex);
                indexes.Y = Math.Max(indexes.Y, lastIndex);
            }
            else
            {
                indexes.X = startIndex;
                indexes.Y = lastIndex;
            }

            m_elementsIndex[element] = indexes;
        }

        /// <summary>
        /// Returns the start index of the element's text in the text of the document.
        /// </summary>
        /// <param name="element">Element object.</param>
        /// <returns>Start index of the element's text in the text of the document if found; -1 otherwise.</returns>
        public int FirstIndexOf(BaseElement element)
        {
            if (element == null)
                throw new ArgumentNullException("element");

            object value = m_elementsIndex[element];
            Point indexes = Point.Empty;

            if (value != null)
            {
                indexes = (Point)value;
            }

            return indexes.X;
        }

        /// <summary>
        /// Returns the last index of the element's text in the text of the document.
        /// </summary>
        /// <param name="element">Element object.</param>
        /// <returns>Last index of the element's text in the text of the document if found; -1 otherwise.</returns>
        public int LastIndexOf(BaseElement element)
        {
            if (element == null)
                throw new ArgumentNullException("element");

            object value = m_elementsIndex[element];
            Point indexes = Point.Empty;

            if (value != null)
            {
                indexes = (Point)value;
            }

            return indexes.Y;
        }

        /// <summary>
        /// Gets first / last index of the element's text in the text of the document.
        /// </summary>
        /// <param name="element">Element object.</param>
        /// <returns>First / Last index of the element's text in the text of the document if found; -1 otherwise.</returns>
        public Point IndexOf(BaseElement element)
        {
            if (element == null)
                throw new ArgumentNullException("element");

            object value = m_elementsIndex[element];
            Point indexes = Point.Empty;

            if (value != null)
            {
                indexes = (Point)value;
            }

            return indexes;
        }
        #endregion
    }
}
