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
using Rectangle = Syncfusion.XlsIO.Implementation.WINRT.Rectangle;
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
    /// Sort the data based on the style.
    /// </summary>
    class StyleSorting : SortingAlgorithm
    {


        #region Initialization
        /// <summary>
        /// Initializes the Style sorting attributes.
        /// </summary>
        /// <param name="data">Data to sort.</param>
        /// <param name="types">Represents the type of the column.</param>
        /// <param name="orderBy">Sorting order of the column.</param>
        /// <param name="colors">Colors to sort.</param>
        public StyleSorting(object[][] data, Type[] types, OrderBy[] orderBy,Color[] colors)
            : base(data, types, orderBy,colors)
        {

        }

        #endregion

        #region Override Methods
        /// <summary>
        /// Sort the data based on specified column.
        /// </summary>
        /// <param name="left"></param>
        /// <param name="right"></param>
        /// <param name="columnIndex"></param>
        public override void Sort(int left, int right,int columnIndex)
        {

            SortByAlign(left, right, columnIndex);
        }
        #endregion
        /// <summary>
        /// Sort the data based on the Style.
        /// </summary>
        /// <param name="left"></param>
        /// <param name="right"></param>
        /// <param name="columnIndex"></param>
        void SortByAlign(int left, int right, int columnIndex)
        {
            if (orderBy[columnIndex - 1] == OrderBy.OnBottom)
                SortBottomByCellColor(left, right, columnIndex);
            else
                SortTopByCellColor(left, right, columnIndex);
        }

        #region Methods
        /// <summary>
        /// Sorts the data based on cell color.
        /// </summary>
        /// <param name="left">Left index of the data.</param>
        /// <param name="right">Right index of the data.</param>
        /// <param name="columnIndex">Column index of the data to sort.</param>
        void SortTopByCellColor(int left, int right, int columnIndex)
        {
            for (int i = m_iTopPosition; i <=right; i++)
            {
                if (Compare(m_colors[columnIndex-1],(Color)m_data[i][columnIndex]))
                {
                    MoveUp(i, m_iTopPosition);
                    m_iTopPosition ++;
                }

            }

        }
        /// <summary>
        /// Compares the color.
        /// </summary>
        /// <param name="color1">First color object.</param>
        /// <param name="color2">Second color object.</param>
        /// <returns></returns>
        bool  Compare(Color color1, Color color2)
        {
            if (color1.R == color2.R && color1.G == color2.G && color1.B == color2.B)
                return true;
            return false;
        }
        /// <summary>
        /// Sorts by cell color and align bottom.
        /// </summary>
        /// <param name="left"></param>
        /// <param name="right"></param>
        /// <param name="columnIndex"></param>
        void SortBottomByCellColor(int left, int right, int columnIndex)
        {
            for (int i = m_iBottomPosition; i >=left; i--)
            {
                if (Compare(m_colors[columnIndex-1], (Color)m_data[i][columnIndex]))
                {
                    MoveDown(i, m_iBottomPosition);

                    m_iBottomPosition -- ;
                    
                }
            }

        }
        /// <summary>
        /// Moves the Style to top.
        /// </summary>
        /// <param name="srcIndex">source index. </param>
        /// <param name="destIndex">Moves to destination index.</param>
        internal void MoveDown(int srcIndex, int destIndex)
        {
            for (int i = srcIndex  ; i <destIndex; i++)
                SwapRow(i, i + 1);
        }
        /// <summary>
        /// Moves the Style to top.
        /// </summary>
        /// <param name="srcIndex">source index. </param>
        /// <param name="destIndex">Moves to destination index.</param>
        internal void MoveUp(int srcIndex, int destIndex)
        {
            for (int i = srcIndex; i > destIndex; i--)
                SwapRow(i, i - 1);
        }

        #endregion

        #region ISortingAlgorithm
        /// <summary>
        /// Sorts the integer type data with the specified column index.
        /// </summary>
        /// <param name="left">Represents the start index of the data.</param>
        /// <param name="right">Represents the end index of the data.</param>
        /// <param name="columnIndex">Reprsents the columnindex of the data.</param>
        public void SortInt(int left, int right, int columnIndex)
        {
            throw new NotImplementedException();
        }
        /// <summary>
        /// Sorts the Float values with the specified column.
        /// </summary>
        /// <param name="left">Represents the start index of the data.</param>
        /// <param name="right">Represents the end index of the data.</param>
        /// <param name="columnIndex">Reprsents the columnindex of the data.</param>
        public void SortFloat(int left, int right, int columnIndex)
        {
            throw new NotImplementedException();
        }
        /// <summary>
        /// Sorts the Date type values with the specified column.
        /// </summary>
        /// <param name="left">Represents the start index of the data.</param>
        /// <param name="right">Represents the end index of the data.</param>
        /// <param name="columnIndex">Reprsents the columnindex of the data.</param>
        public void SortDate(int left, int right, int columnIndex)
        {
            throw new NotImplementedException();
        }
        /// <summary>
        /// Sorts the string type values with the specified column.
        /// </summary>
        /// <param name="left">Represents the start index of the data.</param>
        /// <param name="right">Represents the end index of the data.</param>
        /// <param name="columnIndex">Reprsents the columnindex of the data.</param>
        public void SortString(int left, int right, int columnIndex)
        {
            throw new NotImplementedException();
        }
        /// <summary>
        /// Sorts the based on the types with the specified column.
        /// </summary>
        /// <param name="left">Represents the start index of the data.</param>
        /// <param name="right">Represents the end index of the data.</param>
        /// <param name="columnIndex">Reprsents the columnindex of the data.</param>
        public void SortOnTypes(int left, int right, int columnIndex)
        {
            throw new NotImplementedException();
        }
        /// <summary>
        /// Sorts the Integer type values in Descending order with the specified column.
        /// </summary>
        /// <param name="left">Represents the start index of the data.</param>
        /// <param name="right">Represents the end index of the data.</param>
        /// <param name="columnIndex">Reprsents the columnindex of the data.</param>
        public void SortIntDesc(int left, int right, int columnIndex)
        {
            throw new NotImplementedException();
        }
        /// <summary>
        /// Sorts the Float type values in Descending order with the specified column.
        /// </summary>
        /// <param name="left">Represents the start index of the data.</param>
        /// <param name="right">Represents the end index of the data.</param>
        /// <param name="columnIndex">Reprsents the columnindex of the data.</param>
        public void SortFloatDesc(int left, int right, int columnIndex)
        {
            throw new NotImplementedException();
        }
        /// <summary>
        /// Sorts the Datea type values in Descending order with the specified column.
        /// </summary>
        /// <param name="left">Represents the start index of the data.</param>
        /// <param name="right">Represents the end index of the data.</param>
        /// <param name="columnIndex">Reprsents the columnindex of the data.</param>
        public void SortDateDesc(int left, int right, int columnIndex)
        {
            throw new NotImplementedException();
        }
        /// <summary>
        /// Sorts the string type values in Descending order with the specified column.
        /// </summary>
        /// <param name="left">Represents the start index of the data.</param>
        /// <param name="right">Represents the end index of the data.</param>
        /// <param name="columnIndex">Reprsents the columnindex of the data.</param>
        public void SortStringDesc(int left, int right, int columnIndex)
        {
            throw new NotImplementedException();
        }
        #endregion


    }
}
