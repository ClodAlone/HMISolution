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
    /// This class used to sort the Data using the InsertionSort algorithm.
    /// </summary>
    class InsertionSortImpl : SortingAlgorithm
    {
        #region Initialization
        public InsertionSortImpl(object[][] data, Type[] types,OrderBy[] orderBy,Color[] colors)
            : base(data, types,orderBy,colors)
        {
        }
        #endregion

        #region Methods which sort rows in Ascending.
        /// <summary>
        /// Sorts the int value in Ascending order.
        /// </summary>
        /// <param name="left"></param>
        /// <param name="right"></param>
        /// <param name="columnIndex"></param>
        public void SortInt(int left, int right, int columnIndex)
        {
            int iColumn = columnIndex;

            int iIn, iOut;
            //  sorted on left of out
            for (iOut = left + 1; iOut <= right; iOut++)
            {
                object[] tmp = ExtractSingleRow(iOut);

                int temp =(int) m_data[iOut][iColumn];    // remove marked item
                iIn = iOut;                     // start shifts at out
                // until one is smaller,
                while (iIn > left && (int)m_data[iIn - 1][iColumn] >= temp)
                {
                    if (columnIndex+1<=count && temp == (int)m_data[iIn - 1][iColumn] && columnIndex + 1 < count)
                    {
                        SortOnTypes(iIn - 1, iIn, columnIndex + 1);
                    }
                    else
                        SwapRow(iIn - 1, iIn);
                    
                    --iIn;                      // go left one position
                }
                
            }  
        }  
        /// <summary>
        /// Sorts the Float value in Ascending order.
        /// </summary>
        /// <param name="left"></param>
        /// <param name="right"></param>
        /// <param name="columnIndex"></param>
        public void SortFloat(int left, int right, int columnIndex)
        {
            int iColumn = columnIndex;

            int iIn, iOut;
            //  sorted on left of out
            for (iOut = left + 1; iOut <= right; iOut++)
            {
                object[] tmp = ExtractSingleRow(iOut);

                double temp = (double)m_data[iOut][iColumn];    // remove marked item
                iIn = iOut;                     // start shifts at out
                // until one is smaller,
                while (iIn > left && (double)m_data[iIn - 1][iColumn] >= temp)
                {
                    if (columnIndex + 1 <= count && temp == (double)m_data[iIn - 1][iColumn])
                    {
                        SortOnTypes(iIn - 1, iIn, columnIndex + 1);
                    }
                    else
                        SwapRow(iIn - 1, iIn);
                    --iIn;                      // go left one position
                }
            } 
        }  
        /// <summary>
        /// Sort values in Ascending order.
        /// </summary>
        /// <param name="left"></param>
        /// <param name="right"></param>
        /// <param name="columnIndex"></param>
        public void SortDate(int left, int right, int columnIndex)
        {
            int iColumn = columnIndex;

            int iIn, iOut;
            //  sorted on left of out
            for (iOut = left + 1; iOut <= right; iOut++)
            {
                object[] tmp = ExtractSingleRow(iOut);

                DateTime temp = (DateTime)m_data[iOut][iColumn];    // remove marked item
                iIn = iOut;                     // start shifts at out
                // until one is smaller,
                while (iIn > left && (DateTime)m_data[iIn - 1][iColumn] >= temp)
                {
                    if (columnIndex + 1 <= count && temp == (DateTime)m_data[iIn - 1][iColumn])
                    {
                        SortOnTypes(iIn - 1, iIn, columnIndex + 1);
                    }
                    else
                        SwapRow(iIn - 1, iIn);
                    --iIn;                      // go left one position
                }
            } 
        }
        /// <summary>
        /// Sorts the int value in Ascending order.
        /// </summary>
        /// <param name="left"></param>
        /// <param name="right"></param>
        /// <param name="columnIndex"></param>
        public void SortString(int left, int right, int columnIndex)
        {
            int iColumn = columnIndex;

            int iIn, iOut;
            //  sorted on left of out
            for (iOut = left + 1; iOut <= right; iOut++)
            {
                object[] tmp = ExtractSingleRow(iOut);

                string temp = (string)m_data[iOut][iColumn];    // remove marked item
                iIn = iOut;                     // start shifts at out
                // until one is smaller,
                while (iIn > left && ((string)m_data[iIn - 1][iColumn]).CompareTo(temp) >= 0)
                {
                    if (columnIndex + 1 <= count && temp == (string)m_data[iIn - 1][iColumn])
                    {
                        SortOnTypes(iIn - 1, iIn, columnIndex + 1);
                    }
                    else
                        SwapRow(iIn - 1, iIn);
                    --iIn;                      // go left one position
                }
            }  
        }  
      
      
        #endregion

        #region Methods
        /// <summary>
        /// Sort Methods based on types.
        /// </summary>
        /// <param name="left">Represents the left index of the data.</param>
        /// <param name="right">Represents the right index of the data.</param>
        /// <param name="columnIndex">Sots based on this column.</param>
        public void SortOnTypes(int left, int right, int columnIndex)
        {
            if (types[columnIndex - 1] == typeof(int))
                SortInt(left, right, columnIndex);
            else if (types[columnIndex - 1] == typeof(double))
            {
                if (orderBy[columnIndex - 1]== OrderBy.Ascending)
                    SortFloat(left, right, columnIndex);
                else
                    SortFloatDesc(left, right, columnIndex);
            }
            else if (types[columnIndex - 1] == typeof(string))
            {
                if (orderBy[columnIndex - 1]== OrderBy.Ascending)
                    SortString(left, right, columnIndex);
                else
                    SortStringDesc(left, right, columnIndex);
            }
            else if (types[columnIndex - 1] == typeof(DateTime))
            {
                if (orderBy[columnIndex - 1] == OrderBy.Ascending)
                    SortDate(left, right, columnIndex);
                else
                    SortDateDesc(left, right, columnIndex);
            }
        }
        #endregion

        #region Methods which sort in Rows in descending.
        /// <summary>
        /// Sorts the Int values in Descending order.
        /// </summary>
        /// <param name="left">Represents the left index of the data.</param>
        /// <param name="right">Represents the right index of the data.</param>
        /// <param name="columnIndex">Sots based on this column.</param>
        public void SortIntDesc(int left, int right, int columnIndex)
        {
            int iColumn = columnIndex;

            int iIn, iOut;
            //  sorted on left of out
            for (iOut = left + 1; iOut <= right; iOut++)
            {
                object[] tmp = ExtractSingleRow(iOut);

                int temp = (int)m_data[iOut][iColumn];    // remove marked item
                iIn = iOut;                     // start shifts at out
                // until one is smaller,
                while (iIn > left && (int)m_data[iIn - 1][iColumn] <= temp)
                {
                    if (columnIndex + 1 <= count && temp == (int)m_data[iIn - 1][iColumn])
                    {
                        SortOnTypes(iIn - 1, iIn, columnIndex + 1);
                    }
                    else
                        SwapRow(iIn - 1, iIn);
                    --iIn;                      // go left one position
                }
            }
        }  
        /// <summary>
        /// Sorts the float values in Descending order.
        /// </summary>
        /// <param name="left">Represents the left index of the data.</param>
        /// <param name="right">Represents the right index of the data.</param>
        /// <param name="columnIndex">Sots based on this column.</param>
        public void SortFloatDesc(int left, int right, int columnIndex)
        {
            int iColumn = columnIndex;

            int iIn, iOut;
            //  sorted on left of out
            for (iOut = left + 1; iOut <= right; iOut++)
            {
                object[] tmp = ExtractSingleRow(iOut);

                double temp = (double)m_data[iOut][iColumn];    // remove marked item
                iIn = iOut;                     // start shifts at out
                // until one is smaller,
                while (iIn > left && (double)m_data[iIn - 1][iColumn] <= temp)
                {
                    if (columnIndex + 1 <= count && temp == (double)m_data[iIn - 1][iColumn])
                    {
                        SortOnTypes(iIn - 1, iIn, columnIndex + 1);
                    }
                    else
                        SwapRow(iIn - 1, iIn);
                    --iIn;                      // go left one position
                }
            }  
        }  
        /// <summary>
        /// Sorts the float values in Descending order.
        /// </summary>
        /// <param name="left">Represents the left index of the data.</param>
        /// <param name="right">Represents the right index of the data.</param>
        /// <param name="columnIndex">Sots based on this column.</param>
        public void SortDateDesc(int left, int right, int columnIndex)
        {
            int iColumn = columnIndex;

            int iIn, iOut;
            //  sorted on left of out
            for (iOut = left + 1; iOut <= right; iOut++)
            {
                object[] tmp = ExtractSingleRow(iOut);

                DateTime temp = (DateTime)m_data[iOut][iColumn];    // remove marked item
                iIn = iOut;                     // start shifts at out
                // until one is smaller,
                while (iIn > left && (DateTime)m_data[iIn - 1][iColumn] <= temp)
                {
                    if (columnIndex + 1 <= count && temp == (DateTime)m_data[iIn - 1][iColumn])
                    {
                        SortOnTypes(iIn - 1, iIn, columnIndex + 1);
                    }
                    else
                        SwapRow(iIn - 1, iIn);
                    --iIn;                      // go left one position
                }
            } 
        }  
        /// <summary>
        /// Sorts the float values in Descending order.
        /// </summary>
        /// <param name="left">Represents the left index of the data.</param>
        /// <param name="right">Represents the right index of the data.</param>
        /// <param name="columnIndex">Sots based on this column.</param>
        public void SortStringDesc(int left, int right, int columnIndex)
        {
            int iColumn = columnIndex;

            int iIn, iOut;
            //  sorted on left of out
            for (iOut = left + 1; iOut <= right; iOut++)
            {
                object[] tmp = ExtractSingleRow(iOut);

                string temp = (string)m_data[iOut][iColumn];    // remove marked item
                iIn = iOut;                     // start shifts at out
                // until one is smaller,
                while (iIn > left && ((string)m_data[iIn - 1][iColumn]).CompareTo(temp) <= 0)
                {
                    if (columnIndex + 1 <= count && temp == (string)m_data[iIn - 1][iColumn])
                    {
                        SortOnTypes(iIn - 1, iIn, columnIndex + 1);
                    }
                    else
                        SwapRow(iIn - 1, iIn);
                    --iIn;                      // go left one position
                }
            }  
        }  
        #endregion




        #region Overrided Methods
       /// <summary>
        /// Sorts the Data using InsertionSort Algorithm.
       /// </summary>
       /// <param name="left">Represents the left index of the data.</param>
       /// <param name="right">Represents the right index of the data.</param>
        public override void Sort(int left, int right, int columnIndex)
        {
            SortOnTypes(left, right, columnIndex);
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
       
    }
}
