#region Copyright Syncfusion Inc. 2001 - 2014
// Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Any infringement will be prosecuted under
// applicable laws. 
#endregion
using System;
#if ( WINRT )
using Windows.UI;
#endif

#if  (SILVERLIGHT || WP)
using System.Windows.Media;
#elif ( WINRT )
using Syncfusion.XlsIO;
#else
using System.Drawing;

#endif

namespace Syncfusion.XlsIO.Implementation.Sorting
{
    /// <summary>
    /// Parent of the sorting classes.
    /// </summary>
    abstract class SortingAlgorithm : ISortingAlgorithm
    {
        #region Members
        /// <summary>
        /// Data to sort.
        /// </summary>
        protected object[][] m_data;
        /// Represents the Column/Row count.
        /// </summary>
        protected int count;
        /// <summary>
        /// Represents the Column/Row Types.
        /// </summary>
        protected Type[] types;
        /// <summary>
        /// Represents the sorting order of each column/Row.
        /// </summary>
        protected OrderBy[] orderBy;
        /// <summary>
        /// Represents the current top position of Row/column with color.
        /// </summary>
        protected int m_iTopPosition;
        /// <summary>
        /// Represents the current bottom position of the Row/Column with color.
        /// </summary>
        protected int m_iBottomPosition;
        /// <summary>
        /// Represents the colors to find.
        /// </summary>
        protected Color[] m_colors;
        #endregion

        #region Properties

        /// <summary>
        /// Represents the sortable data.
        /// </summary>
        public object[][] Data
        {
            get
            {
                return m_data;
            }
        }
        #endregion

        #region ISortingAlgorithm Properties
        /// <summary>
        /// Range object to sort.
        /// </summary>
        public IRange Range
        {
            get
            {
                throw new NotSupportedException("Range");
            }
            set
            {
                throw new NotSupportedException("Range");
            }
        }
        #endregion

        #region Intialization
        /// <summary>
        /// Intialiazes the sorting algorithm atributes.
        /// </summary>
        /// <param name="data">Data to sort.</param>
        /// <param name="types">Type of the each column.</param>
        /// <param name="orderBy">Sorting order of each column.</param>
        /// <param name="colors">Colors to find.</param>
        public SortingAlgorithm(object[][] data, Type[] types, OrderBy[] orderBy, Color[] colors)
        {
            this.m_data = data;
            this.types = types;
            this.orderBy = orderBy;
            this.m_colors = colors;
            count = types.Length;
            m_iTopPosition = 0;
            m_iBottomPosition = data.Length - 1;
        }
        #endregion

        #region Sorting Methods
        /// <summary>
        /// Sorts the data with the specified column index.
        /// </summary>
        /// <param name="left">Left index of the data.</param>
        /// <param name="right">Right index of the data to sort.</param>
        /// <param name="columnIndex">column index of the data.</param>
        public abstract void Sort(int left, int right, int columnIndex);
        #endregion

        #region HelperMethods
        /// <summary>
        /// Extracts the single row from the Data.
        /// </summary>
        /// <param name="rowIndex">Index of the row to extract.</param>
        /// <returns>Row value in object array.</returns>
        protected object[] ExtractSingleRow(int rowIndex)
        {
            object[] dest = new object[m_data[0].Length];
            for (int column = 0; column < dest.Length; column++)
                dest[column] = m_data[rowIndex][column];
            return dest;
        }
        /// <summary>
        /// Extracts the single colum form the data.
        /// </summary>
        /// <param name="rowIndex">Row index to extract.</param>
        /// <returns>Column values in object array.</returns>
        protected object[] ExtractSingleColumn(int rowIndex)
        {
            object[] dest = new object[m_data.Length];
            for (int row = 0; row < dest.Length; row++)
                dest[row] = m_data[row][rowIndex];
            return dest;
        }
        /// <summary>
        /// Swaps the row.
        /// </summary>
        /// <param name="left">first row index.</param>
        /// <param name="right">second row index.</param>
        protected void SwapRow(int left, int right)
        {
            Swap(m_data[left], m_data[right]);
        }
        /// <summary>
        /// Swpas the array content.
        /// </summary>
        /// <param name="left">Left array content.</param>
        /// <param name="right">Right array content.</param>
        private void Swap(object[] left, object[] right)
        {
            for (int i = 0; i < left.Length; i++)
            {
                object tmp = right[i];
                right[i] = left[i];
                left[i] = tmp;
            }
        }
        /// <summary>
        /// Swaps the column content.
        /// </summary>
        /// <param name="left">First column Index.</param>
        /// <param name="right">Second column index.</param>
        protected void SwapColumn(int left, int right)
        {
            for (int i = 0; i < m_data[0].Length; i++)
            {
                object tmp = m_data[i][left];
                m_data[i][left] = m_data[i][right];
                m_data[i][right] = tmp;
            }
        }
        /// <summary>
        /// Swaps the column content.
        /// </summary>
        /// <param name="left">Left array content.</param>
        /// <param name="right">Right array content.</param>
        private void SwapColumn(object[] left, object[] right)
        {
            for (int i = 0; i < left.Length; i++)
            {
                object tmp = right[i];
                right[i] = left[i];
                left[i] = tmp;
            }
        }
        #endregion

        #region ISortingAlgorithm
        /// <summary>
        /// Sorts the integer type data with the specified column index.
        /// </summary>
        /// <param name="left">Represents the start index of the data.</param>
        /// <param name="right">Represents the end index of the data.</param>
        /// <param name="columnIndex">Reprsents the columnindex of the data.</param>
        public virtual void SortInt(int left, int right, int columnIndex)
        {
            throw new NotImplementedException();
        }
        /// <summary>
        /// Sorts the Float values with the specified column.
        /// </summary>
        /// <param name="left">Represents the start index of the data.</param>
        /// <param name="right">Represents the end index of the data.</param>
        /// <param name="columnIndex">Reprsents the columnindex of the data.</param>
        public virtual void SortFloat(int left, int right, int columnIndex)
        {
            throw new NotImplementedException();
        }
        /// <summary>
        /// Sorts the Date type values with the specified column.
        /// </summary>
        /// <param name="left">Represents the start index of the data.</param>
        /// <param name="right">Represents the end index of the data.</param>
        /// <param name="columnIndex">Reprsents the columnindex of the data.</param>
        public virtual void SortDate(int left, int right, int columnIndex)
        {
            throw new NotImplementedException();
        }
        /// <summary>
        /// Sorts the string type values with the specified column.
        /// </summary>
        /// <param name="left">Represents the start index of the data.</param>
        /// <param name="right">Represents the end index of the data.</param>
        /// <param name="columnIndex">Reprsents the columnindex of the data.</param>
        public virtual void SortString(int left, int right, int columnIndex)
        {
            throw new NotImplementedException();
        }
        /// <summary>
        /// Sorts the based on the types with the specified column.
        /// </summary>
        /// <param name="left">Represents the start index of the data.</param>
        /// <param name="right">Represents the end index of the data.</param>
        /// <param name="columnIndex">Reprsents the columnindex of the data.</param>
        public virtual void SortOnTypes(int left, int right, int columnIndex)
        {
            throw new NotImplementedException();
        }
        /// <summary>
        /// Sorts the Integer type values in Descending order with the specified column.
        /// </summary>
        /// <param name="left">Represents the start index of the data.</param>
        /// <param name="right">Represents the end index of the data.</param>
        /// <param name="columnIndex">Reprsents the columnindex of the data.</param>
        public virtual void SortIntDesc(int left, int right, int columnIndex)
        {
            throw new NotImplementedException();
        }
        /// <summary>
        /// Sorts the Float type values in Descending order with the specified column.
        /// </summary>
        /// <param name="left">Represents the start index of the data.</param>
        /// <param name="right">Represents the end index of the data.</param>
        /// <param name="columnIndex">Reprsents the columnindex of the data.</param>
        public virtual void SortFloatDesc(int left, int right, int columnIndex)
        {
            throw new NotImplementedException();
        }
        /// <summary>
        /// Sorts the Datea type values in Descending order with the specified column.
        /// </summary>
        /// <param name="left">Represents the start index of the data.</param>
        /// <param name="right">Represents the end index of the data.</param>
        /// <param name="columnIndex">Reprsents the columnindex of the data.</param>
        public virtual void SortDateDesc(int left, int right, int columnIndex)
        {
            throw new NotImplementedException();
        }
        /// <summary>
        /// Sorts the string type values in Descending order with the specified column.
        /// </summary>
        /// <param name="left">Represents the start index of the data.</param>
        /// <param name="right">Represents the end index of the data.</param>
        /// <param name="columnIndex">Reprsents the columnindex of the data.</param>
        public virtual void SortStringDesc(int left, int right, int columnIndex)
        {
            throw new NotImplementedException();
        }
        #endregion
    }
}


