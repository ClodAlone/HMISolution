#region Copyright Syncfusion Inc. 2001 - 2014
// Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Any infringement will be prosecuted under
// applicable laws. 
#endregion
using System;
using System.Collections.Generic;

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
    /// This class used to sort the data using QuickSort3.
    /// </summary>
    class QuickSort3Impl : SortingAlgorithm
    {
        private const int CUTOFF = 10;

        #region Initialization
        /// <summary>
        /// Initailize the sort data attributes.
        /// </summary>
        /// <param name="data">Represents the data to sort.</param>
        /// <param name="types">Represents the datatype of each column.</param>
        /// <param name="orderBy">Represents the sort order.</param>
        /// <param name="colors">Represents the color to sort.</param>
        public QuickSort3Impl(object[][] data, Type[] types, OrderBy[] orderBy, Color[] colors)
            : base(data, types, orderBy, colors)
        {
        }
        #endregion

        #region Methods which sort rows in ascending
        /// <summary>
        /// Sorts the data in Ascending sort.
        /// </summary>
        /// <param name="left">Represents the left index of the data.</param>
        /// <param name="right">Represents the right index of the data.</param>
        /// <param name="iColumn">Represents the column index of the data.</param>
        public void SortInt(int left, int right, int iColumn)
        {
            int k;
            if (right <= left) return;
            int pivot_value = (int)m_data[right][iColumn];

            int current_left_index = left - 1;
            int current_right_index = right;
            int left_equal_key_index = left - 1;
            int right_equal_key_index = right;

            for (; ; )
            {
                while ((int)m_data[++current_left_index][iColumn] < pivot_value)
                    ;

                while (pivot_value <= (int)m_data[--current_right_index][iColumn])
                    if (current_right_index == left) break;

                if (current_left_index >= current_right_index) break;

                SwapRow(current_left_index, current_right_index);

                if ((int)m_data[current_left_index][iColumn] == pivot_value)
                {
                    left_equal_key_index++;
                    SwapRow(left_equal_key_index, current_left_index);
                }

                if (pivot_value == (int)m_data[current_right_index][iColumn])
                {
                    right_equal_key_index--;
                    SwapRow(right_equal_key_index, current_right_index);
                }
            }

            SwapRow(current_left_index, right);

            current_right_index = current_left_index - 1;
            current_left_index = current_left_index + 1;

            for (k = left; k < left_equal_key_index; k++, current_right_index--)
                SwapRow(k, current_right_index);

            for (k = right - 1; k > right_equal_key_index; k--, current_left_index++)
                SwapRow(k, current_left_index);

            SortInt(left, current_right_index, iColumn);
            SortInt(current_left_index, right, iColumn);
        }
        /// <summary>
        /// Sorts the data in Ascending sort.
        /// </summary>
        /// <param name="left">Represents the left index of the data.</param>
        /// <param name="right">Represents the right index of the data.</param>
        /// <param name="iColumn">Represents the column index of the data.</param>
        public void SortFloat(int left, int right, int iColumn)
        {
            int k;
            if (right <= left) return;

            double pivot_value = (double)m_data[right][iColumn];

            int current_left_index = left - 1;
            int current_right_index = right;
            int left_equal_key_index = left - 1;
            int right_equal_key_index = right;

            for (; ; )
            {
                while ((double)m_data[++current_left_index][iColumn] < pivot_value)
                    ;

                while (pivot_value <= (double)m_data[--current_right_index][iColumn])
                    if (current_right_index == left) break;

                if (current_left_index >= current_right_index) break;

                SwapRow(current_left_index, current_right_index);

                if ((double)m_data[current_left_index][iColumn] == pivot_value)
                {
                    left_equal_key_index++;
                    SwapRow(left_equal_key_index, current_left_index);
                }

                if (pivot_value == (double)m_data[current_right_index][iColumn])
                {
                    right_equal_key_index--;
                    SwapRow(right_equal_key_index, current_right_index);
                }
            }

            SwapRow(current_left_index, right);

            current_right_index = current_left_index - 1;
            current_left_index = current_left_index + 1;

            for (k = left; k < left_equal_key_index; k++, current_right_index--)
                SwapRow(k, current_right_index);

            for (k = right - 1; k > right_equal_key_index; k--, current_left_index++)
                SwapRow(k, current_left_index);

            SortFloat(left, current_right_index, iColumn);
            SortFloat(current_left_index, right, iColumn);
        }
        /// <summary>
        /// Sorts the data in Ascending sort.
        /// </summary>
        /// <param name="left">Represents the left index of the data.</param>
        /// <param name="right">Represents the right index of the data.</param>
        /// <param name="iColumn">Represents the column index of the data.</param>
        public void SortDate(int left, int right, int iColumn)
        {
            int k;
            if (right <= left) return;

            DateTime pivot_value = (DateTime)m_data[right][iColumn];

            int current_left_index = left - 1;
            int current_right_index = right;
            int left_equal_key_index = left - 1;
            int right_equal_key_index = right;

            for (; ; )
            {
                while ((DateTime)m_data[++current_left_index][iColumn] < pivot_value)
                    ;

                while (pivot_value <= (DateTime)m_data[--current_right_index][iColumn])
                    if (current_right_index == left) break;

                if (current_left_index >= current_right_index) break;

                SwapRow(current_left_index, current_right_index);

                if ((DateTime)m_data[current_left_index][iColumn] == pivot_value)
                {
                    left_equal_key_index++;
                    SwapRow(left_equal_key_index, current_left_index);
                }

                if (pivot_value == (DateTime)m_data[current_right_index][iColumn])
                {
                    right_equal_key_index--;
                    SwapRow(right_equal_key_index, current_right_index);
                }
            }

            SwapRow(current_left_index, right);

            current_right_index = current_left_index - 1;
            current_left_index = current_left_index + 1;

            for (k = left; k < left_equal_key_index; k++, current_right_index--)
                SwapRow(k, current_right_index);

            for (k = right - 1; k > right_equal_key_index; k--, current_left_index++)
                SwapRow(k, current_left_index);

            SortDate(left, current_right_index, iColumn);
            SortDate(current_left_index, right, iColumn);
        }
        /// <summary>
        /// Sorts the data in Ascending sort.
        /// </summary>
        /// <param name="left">Represents the left index of the data.</param>
        /// <param name="right">Represents the right index of the data.</param>
        /// <param name="iColumn">Represents the column index of the data.</param>
        public void SortString(int left, int right, int iColumn)
        {
            int k;
            if (right <= left) return;
            string pivot_value = (string)m_data[right][iColumn];

            int current_left_index = left - 1;
            int current_right_index = right;
            int left_equal_key_index = left - 1;
            int right_equal_key_index = right;

            for (; ; )
            {
                while (((string)m_data[++current_left_index][iColumn]).CompareTo(pivot_value) < 0)
                    ;

                while (pivot_value.CompareTo((string)m_data[--current_right_index][iColumn]) <= 0)
                    if (current_right_index == left) break;

                if (current_left_index >= current_right_index) break;

                SwapRow(current_left_index, current_right_index);

                if ((string)m_data[current_left_index][iColumn] == pivot_value)
                {
                    left_equal_key_index++;
                    SwapRow(left_equal_key_index, current_left_index);
                }

                if (pivot_value == (string)m_data[current_right_index][iColumn])
                {
                    right_equal_key_index--;
                    SwapRow(right_equal_key_index, current_right_index);
                }
            }

            SwapRow(current_left_index, right);

            current_right_index = current_left_index - 1;
            current_left_index = current_left_index + 1;

            for (k = left; k < left_equal_key_index; k++, current_right_index--)
                SwapRow(k, current_right_index);

            for (k = right - 1; k > right_equal_key_index; k--, current_left_index++)
                SwapRow(k, current_left_index);

            SortString(left, current_right_index, iColumn);
            SortString(current_left_index, right, iColumn);
        }

        #endregion


        #region Methods which sort rows in Descending
        /// <summary>
        /// Sorts the data in Decending sort.
        /// </summary>
        /// <param name="left">Represents the left index of the data.</param>
        /// <param name="right">Represents the right index of the data.</param>
        /// <param name="iColumn">Represents the column index of the data.</param>
        public void SortIntDesc(int left, int right, int iColumn)
        {
            int k;
            if (right <= left) return;

            int pivot_value = (int)m_data[right][iColumn];

            int current_left_index = left - 1;
            int current_right_index = right;
            int left_equal_key_index = left - 1;
            int right_equal_key_index = right;

            for (; ; )
            {

                while ((int)m_data[++current_left_index][iColumn] > pivot_value)
                    ;

                while (pivot_value >= (int)m_data[--current_right_index][iColumn])
                    if (current_right_index == left) break;

                if (current_left_index <= current_right_index) break;

                SwapRow(current_left_index, current_right_index);

                if ((int)m_data[current_left_index][iColumn] == pivot_value)
                {
                    left_equal_key_index++;
                    SwapRow(left_equal_key_index, current_left_index);
                }


                if (pivot_value == (int)m_data[current_right_index][iColumn])
                {
                    right_equal_key_index--;
                    SwapRow(right_equal_key_index, current_right_index);
                }
            }

            SwapRow(current_left_index, right);

            current_right_index = current_left_index - 1;
            current_left_index = current_left_index + 1;

            for (k = left; k < left_equal_key_index; k++, current_right_index--)
                SwapRow(k, current_right_index);


            for (k = right - 1; k > right_equal_key_index; k--, current_left_index++)
                SwapRow(k, current_left_index);


            SortIntDesc(left, current_right_index, iColumn);
            SortIntDesc(current_left_index, right, iColumn);
        }
        /// <summary>
        /// Sorts the data in Decending sort.
        /// </summary>
        /// <param name="left">Represents the left index of the data.</param>
        /// <param name="right">Represents the right index of the data.</param>
        /// <param name="iColumn">Represents the column index of the data.</param>
        public void SortFloatDesc(int left, int right, int iColumn)
        {
            int k;
            if (right <= left) return;

            double pivot_value = (double)m_data[right][iColumn];

            int current_left_index = left - 1;
            int current_right_index = right;
            int left_equal_key_index = left - 1;
            int right_equal_key_index = right;

            for (; ; )
            {

                while ((double)m_data[++current_left_index][iColumn] > pivot_value)
                    ;

                while (pivot_value >= (double)m_data[--current_right_index][iColumn])
                    if (current_right_index == left) break;

                if (current_left_index >= current_right_index) break;

                SwapRow(current_left_index, current_right_index);

                if ((double)m_data[current_left_index][iColumn] == pivot_value)
                {
                    left_equal_key_index++;
                    SwapRow(left_equal_key_index, current_left_index);
                }


                if (pivot_value == (double)m_data[current_right_index][iColumn])
                {
                    right_equal_key_index--;
                    SwapRow(right_equal_key_index, current_right_index);
                }
            }

            SwapRow(current_left_index, right);

            current_right_index = current_left_index - 1;
            current_left_index = current_left_index + 1;

            for (k = left; k < left_equal_key_index; k++, current_right_index--)
                SwapRow(k, current_right_index);


            for (k = right - 1; k > right_equal_key_index; k--, current_left_index++)
                SwapRow(k, current_left_index);


            SortFloatDesc(left, current_right_index, iColumn);
            SortFloatDesc(current_left_index, right, iColumn);
        }
        /// <summary>
        /// Sorts the data in Decending sort.
        /// </summary>
        /// <param name="left">Represents the left index of the data.</param>
        /// <param name="right">Represents the right index of the data.</param>
        /// <param name="iColumn">Represents the column index of the data.</param>
        public void SortDateDesc(int left, int right, int iColumn)
        {
            int k;
            if (right <= left) return;

            DateTime pivot_value = (DateTime)m_data[right][iColumn];

            int current_left_index = left - 1;
            int current_right_index = right;
            int left_equal_key_index = left - 1;
            int right_equal_key_index = right;

            for (; ; )
            {
                while ((DateTime)m_data[++current_left_index][iColumn] > pivot_value)
                    ;

                while (pivot_value >= (DateTime)m_data[--current_right_index][iColumn])
                    if (current_right_index == left) break;
                
                if (current_left_index >= current_right_index) break;

                SwapRow(current_left_index, current_right_index);

                if ((DateTime)m_data[current_left_index][iColumn] == pivot_value)
                {
                    left_equal_key_index++;
                    SwapRow(left_equal_key_index, current_left_index);
                }


                if (pivot_value == (DateTime)m_data[current_right_index][iColumn])
                {
                    right_equal_key_index--;
                    SwapRow(right_equal_key_index, current_right_index);
                }
            }

            SwapRow(current_left_index, right);

            current_right_index = current_left_index - 1;
            current_left_index = current_left_index + 1;

            for (k = left; k < left_equal_key_index; k++, current_right_index--)
                SwapRow(k, current_right_index);


            for (k = right - 1; k > right_equal_key_index; k--, current_left_index++)
                SwapRow(k, current_left_index);


            SortDateDesc(left, current_right_index, iColumn);
            SortDateDesc(current_left_index, right, iColumn);
        }
        /// <summary>
        /// Sorts the data in Decending sort.
        /// </summary>
        /// <param name="left">Represents the left index of the data.</param>
        /// <param name="right">Represents the right index of the data.</param>
        /// <param name="iColumn">Represents the column index of the data.</param>
        public void SortStringDesc(int left, int right, int iColumn)
        {
            int k;
            if (right <= left) return;
            string pivot_value = (string)m_data[right][iColumn];

            int current_left_index = left - 1;
            int current_right_index = right;
            int left_equal_key_index = left - 1;
            int right_equal_key_index = right;

            for (; ; )
            {

                while (((string)m_data[++current_left_index][iColumn]).CompareTo(pivot_value) > 0)
                    ;

                while (pivot_value.CompareTo((string)m_data[--current_right_index][iColumn]) >= 0)
                    if (current_right_index == left) break;

                if (current_left_index >= current_right_index) break;

                SwapRow(current_left_index, current_right_index);

                if ((string)m_data[current_left_index][iColumn] == pivot_value)
                {
                    left_equal_key_index++;
                    SwapRow(left_equal_key_index, current_left_index);
                }

                if (pivot_value == (string)m_data[current_right_index][iColumn])
                {
                    right_equal_key_index--;
                    SwapRow(right_equal_key_index, current_right_index);
                }
            }
            SwapRow(current_left_index, right);

            current_right_index = current_left_index - 1;
            current_left_index = current_left_index + 1;
            for (k = left; k < left_equal_key_index; k++, current_right_index--)
                SwapRow(k, current_right_index);
            for (k = right - 1; k > right_equal_key_index; k--, current_left_index++)
                SwapRow(k, current_left_index);
            SortStringDesc(left, current_right_index, iColumn);
            SortStringDesc(current_left_index, right, iColumn);
        }

        #endregion



        #region Override Methods
        /// <summary>
        /// Sorts the data with the specified column index using QuickSort3
        /// </summary>
        /// <param name="left">Represents the start index of the data.</param>
        /// <param name="right">Represents the end index of the data.</param>
        /// <param name="columnIndex">Reprsents the columnindex of the data.</param>
        public override void Sort(int left, int right, int columnIndex)
        {
            SortOnTypes(left, right, columnIndex);
        }
        /// <summary>
        /// Sorts the data using QuickSort algorithm.
        /// </summary>
        /// <param name="left">Represents the left index of the data.</param>
        /// <param name="right">Represents the right index of the data.</param>
        public void SortOnTypes(int left, int right,int columnIndex)
        {
            QuickSort(m_data, 0, m_data.Length - 1);
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

        /// <summary>
        /// Internal quicksort method that makes recursive calls.
        /// Uses median-of-three partitioning and a cutoff of 10.
        /// </summary>
        /// <param name="data"> An array of Comparable items.</param>
        /// <param name="low"> low the left-most index of the subarray.</param>
        /// <param name="high"> high the right-most index of the subarray.</param>
        private  void QuickSort(object[][] data, int low, int high)
        {
            if (low + CUTOFF > high)
                InsertionSort(data, low, high);
            else
            {
                // Sort low, middle, high
                int middle = (low + high) / 2;
                if (CompareRows<object>(data[middle], (data[low])) < 0)
                    SwapReferences(data, low, middle);
                if (CompareRows<object>(data[high], (data[low])) < 0)
                    SwapReferences(data, low, high);
                if (CompareRows<object>(data[high], (data[middle])) < 0)
                    SwapReferences(data, middle, high);

                // Place pivot at position high - 1
                SwapReferences(data, middle, high - 1);
                int pivot = high - 1;

                // Begin partitioning
                int i, j;
                for (i = low, j = high - 1; ; )
                {
                    while (CompareRows<object>(data[++i], data[pivot]) < 0)
                        ;
                    while (CompareRows<object>(data[pivot], data[--j]) < 0)
                        ;
                    if (i >= j)
                        break;
                    SwapReferences(data, i, j);
                }

                // Restore pivot
                SwapReferences(data, i, high - 1);

                QuickSort(data, low, i - 1);    // Sort small elements
                QuickSort(data, i + 1, high);   // Sort large elements
            }
        }
               
        /// <summary>
        /// Method to swap to elements in an array.
        /// </summary>
        /// <param name="data"> an array of objects..</param>
        /// <param name="firstIndex"> low the left-most index of the subarray.</param>
        /// <param name="secondIndex"> high the right-most index of the subarray.</param>
        public  void SwapReferences(object[][] data, int firstIndex, int secondIndex)
        {
            object[] tmp = data[firstIndex];
            data[firstIndex] = data[secondIndex];
            data[secondIndex] = tmp;
        }
                       
        /// <summary>
        /// Internal insertion sort routine for subarrays
        /// </summary>
        /// <param name="data"> an array of objects..</param>
        /// <param name="low"> low the left-most index of the subarray.</param>
        /// <param name="high"> high the right-most index of the subarray.</param>
        private  void InsertionSort(object[][] data, int low, int high)
        {
            
            for (int p = low + 1; p <= high; p++)
            {
                object[] tmp = data[p];
                int j;

                for (j = p; j > low && CompareRows<object>(tmp, data[j - 1]) < 0; j--)
                    data[j] = data[j - 1];
                data[j] = tmp;
            }
        }

        /// <summary>
        /// Compares two array objects.
        /// </summary>
        /// <param name="firstObject"> First Array Objects.</param>
        /// <param name="secondObject"> Second Array Objects.</param>
        private  int CompareRows<T>(T[] firstObject, T[] secondObject)
        {
            int length = firstObject.Length > secondObject.Length ? firstObject.Length : secondObject.Length; // maxlength
            IComparer<T> comparer = Comparer<T>.Default;
            for (int i = 1; i < length; i++)
            {
                if (orderBy[i - 1] == OrderBy.Ascending)
                {
                    int delta = comparer.Compare(firstObject[i], secondObject[i]);
                    if (delta != 0) return delta;
                }
                else
                {
                    int delta = comparer.Compare(secondObject[i], firstObject[i]);
                    if (delta != 0) return delta;
                }
               
            }
            //if same in the overlapping portion, compare by size instead
            return firstObject.Length.CompareTo(secondObject.Length);
        }
        #endregion
    }
}
