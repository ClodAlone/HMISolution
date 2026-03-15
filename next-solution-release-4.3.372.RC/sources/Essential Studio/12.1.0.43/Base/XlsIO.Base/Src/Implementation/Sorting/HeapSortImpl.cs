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
    /// This class used to sort the Data using the HeapSort algorithm.
    /// </summary>
    class HeapSortImpl : SortingAlgorithm
    {
        #region Initialization
        /// <summary>
        /// Initializes the Heap sorting attributes.
        /// </summary>
        /// <param name="data">Data to sort.</param>
        /// <param name="types">Represents the type of the column.</param>
        /// <param name="orderBy">Sorting order of the column.</param>
        /// <param name="colors">Colors to sort.</param>
        public HeapSortImpl(object[][] data, Type[] types, OrderBy[] orderBy, Color[] m_colors)
            : base(data, types, orderBy, m_colors)
        {
        }
        #endregion

        #region Override Methods
        /// <summary>
        /// Sorts the data using HeapSort Algorithm.
        /// </summary>
        /// <param name="left">Start index of the data.</param>
        /// <param name="right">End index of the data.</param>
        public override void Sort(int left, int right, int columnIndex)
        {
            SortOnTypes(left, right, columnIndex);
        }
        #endregion

        #region Methods
        /// <summary>
        /// Sorts the data based on the types.
        /// </summary>
        /// <param name="left">left index of the data.</param>
        /// <param name="right">Right data of the index.</param>
        /// <param name="columnIndex">column Index to sort.</param>
        public void SortOnTypes(int left, int right, int columnIndex)
        {
            if (types[columnIndex - 1] == typeof(double))
            {
                if (orderBy[columnIndex - 1] == OrderBy.Ascending)
                    SortFloat(left, right, columnIndex);
                else
                    SortFloatDesc(left, right, columnIndex);

            }
            else if (types[columnIndex - 1] == typeof(string))
            {
                if (orderBy[columnIndex - 1] == OrderBy.Ascending)
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

        #region Methods which sort rows in Ascending.
        /// <summary>
        /// Creates the heap tree to sort.
        /// </summary>
        /// <param name="position">Represents the posiont of the heap.</param>
        /// <param name="length">length the data.</param>
        /// <param name="columnIndex">Sots based on this column.</param>
        void CreateIntHeap(int position, int length, int columnIndex)
        {
            int child = (2 * position) + 1;
            int temp;

            while (child <= length)
            {
                if (child < length && (int)m_data[child][columnIndex] < (int)m_data[child + 1][columnIndex])
                {
                    child++;
                }

                if ((double)m_data[position][columnIndex] < (int)m_data[child][columnIndex])
                {
                    SwapRow(position, child);
                    position = child;
                    child = (2 * position) + 1;
                }
                else
                {
                    return;
                }
            }
        }
        /// <summary>
        /// Sorts integer value using HeapSort.
        /// </summary>
        /// <param name="root">Represents the root element in the heap.</param>
        /// <param name="length">Represents the length of the heap.</param>
        /// <param name="columnIndex">Represents the column index of the data to sort.</param>
        public void SortInt(int root, int length, int columnIndex)
        {
            int counter;
            int temp;

            for (counter = (length - 1) / 2; counter >= root; counter--)
            {
                CreateIntHeap(counter, length - 1, columnIndex);
            }
            for (counter = length - 1; counter > root; counter--)
            {
                SwapRow(counter, 0);
                CreateIntHeap(0, counter - 1, columnIndex);
            }

        }
        /// <summary>
        /// Creates the heap tree to sort.
        /// </summary>
        /// <param name="position">Represents the posiont of the heap.</param>
        /// <param name="length">length the data.</param>
        /// <param name="columnIndex">Sots based on this column.</param>
        void CreateFloatHeap(int position, int length, int columnIndex)
        {
            int child = (2 * position) + 1;
            int temp;

            while (child <= length)
            {
                if (child < length && (double)m_data[child][columnIndex] < (double)m_data[child + 1][columnIndex])
                {
                    child++;
                }

                if ((double)m_data[position][columnIndex] < (double)m_data[child][columnIndex])
                {
                    SwapRow(position, child);
                    position = child;
                    child = (2 * position) + 1;
                }
                else
                {
                    return;
                }
            }
        }
        /// <summary>
        /// Sorts float value using HeapSort.
        /// </summary>
        /// <param name="root">Represents the root element in the heap.</param>
        /// <param name="length">Represents the length of the heap.</param>
        /// <param name="columnIndex">Represents the column index of the data to sort.</param>
       public  void SortFloat(int root, int length, int columnIndex)
        {
            int counter;
            int temp;

            for (counter = (length - 1) / 2; counter >= root; counter--)
            {
                CreateFloatHeap(counter, length - 1, columnIndex);
            }
            for (counter = length - 1; counter > root; counter--)
            {
                SwapRow(counter, 0);
                CreateFloatHeap(0, counter - 1, columnIndex);
            }

        }
        /// <summary>
        /// Creates the Date Heap.
        /// </summary>
        /// <param name="position">Represents the posiont of the heap.</param>
        /// <param name="length">Represents the length of the heap.</param>
        /// <param name="columnIndex">Represents the column index of the data to sort.</param>
        void CreateDateHeap(int position, int length, int columnIndex)
        {
            int child = (2 * position) + 1;
            int temp;

            while (child <= length)
            {
                if (child < length && (DateTime)m_data[child][columnIndex] < (DateTime)m_data[child + 1][columnIndex])
                {
                    child++;
                }

                if ((DateTime)m_data[position][columnIndex] < (DateTime)m_data[child][columnIndex])
                {
                    SwapRow(position, child);
                    position = child;
                    child = (2 * position) + 1;
                }
                else
                {
                    return;
                }
            }
        }

        /// <summary>
        /// Sorts the Date type value.
        /// </summary>
        /// <param name="root">Represents the root element in the heap.</param>
        /// <param name="length">Represents the length of the heap.</param>
        /// <param name="columnIndex">Represents the column index of the data to sort.</param>
        public void SortDate(int root, int length, int columnIndex)
        {
            int counter;
            int temp;

            for (counter = (length - 1) / 2; counter >= root; counter--)
            {
                CreateDateHeap(counter, length - 1, columnIndex);
            }
            for (counter = length - 1; counter > root; counter--)
            {
                SwapRow(counter, 0);
                CreateDateHeap(0, counter - 1, columnIndex);
            }
        }
        /// <summary>
        /// Creates the Heap with the string.
        /// </summary>
        /// <param name="position">Represents the posiont of the heap.</param>
        /// <param name="length">Represents the length of the heap.</param>
        /// <param name="columnIndex">Represents the column index of the data to sort.</param>
        void CreateStringHeap(int position, int length, int columnIndex)
        {
            int child = (2 * position) + 1;
            int temp;

            while (child <= length)
            {
                if (child < length && ((string)m_data[child][columnIndex]).CompareTo((string)m_data[child + 1][columnIndex]) < 0)
                {
                    child++;
                }

                if (((string)m_data[position][columnIndex]).CompareTo((string)m_data[child][columnIndex]) < 0)
                {
                    SwapRow(position, child);
                    position = child;
                    child = (2 * position) + 1;
                }
                else
                {
                    return;
                }
            }
        }
        /// <summary>
        /// Sorts the string type.
        /// </summary>
        /// <param name="root">Represents the root element in the heap.</param>
        /// <param name="length">Represents the length of the heap.</param>
        /// <param name="columnIndex">Represents the column index of the data to sort.</param>
        public void SortString(int root, int length, int columnIndex)
        {
            int counter;
            int temp;

            for (counter = (length - 1) / 2; counter >= root; counter--)
            {
                CreateStringHeap(counter, length - 1, columnIndex);
            }
            for (counter = length - 1; counter > root; counter--)
            {
                SwapRow(counter, 0);
                CreateStringHeap(0, counter - 1, columnIndex);
            }
        }
        #endregion


        #region Methods which sort rows in Descending.
        /// <summary>
        /// Creates the Float Heap in Descending order. 
        /// </summary>
        /// <param name="position">Represents the posiont of the heap.</param>
        /// <param name="length">Represents the length of the heap.</param>
        /// <param name="columnIndex">Represents the column index of the data to sort.</param>
        void CreateDescIntHeap(int position, int length, int columnIndex)
        {
            int child = (2 * position) + 1;
            int temp;

            while (child <= length)
            {
                if (child < length && (double)m_data[child][columnIndex] > (double)m_data[child + 1][columnIndex])
                {
                    child++;
                }

                if ((double)m_data[position][columnIndex] > (double)m_data[child][columnIndex])
                {
                    SwapRow(position, child);
                    position = child;
                    child = (2 * position) + 1;
                }
                else
                {
                    return;
                }
            }
        }

        /// <summary>
        /// Sorts the integer value in descending order.
        /// </summary>
        /// <param name="root">Represents the root element in the heap.</param>
        /// <param name="length">Represents the length of the heap.</param>
        /// <param name="columnIndex">Represents the column index of the data to sort.</param>
        public void SortIntDesc(int root, int length, int columnIndex)
        {
            int counter;
            int temp;

            for (counter = (length - 1) / 2; counter >= root; counter--)
            {
                CreateDescIntHeap(counter, length - 1, columnIndex);
            }
            for (counter = length - 1; counter > root; counter--)
            {
                SwapRow(counter, 0);
                CreateDescIntHeap(0, counter - 1, columnIndex);
            }

        }
        /// <summary>
        /// Creates the Float Heap in Descending order. 
        /// </summary>
        /// <param name="position">Represents the posiont of the heap.</param>
        /// <param name="length">Represents the length of the heap.</param>
        /// <param name="columnIndex">Represents the column index of the data to sort.</param>
        void CreateDescFloatHeap(int position, int length, int columnIndex)
        {
            int child = (2 * position) + 1;
            int temp;

            while (child <= length)
            {
                if (child < length && (double)m_data[child][columnIndex] > (double)m_data[child + 1][columnIndex])
                {
                    child++;
                }

                if ((double)m_data[position][columnIndex] > (double)m_data[child][columnIndex])
                {
                    SwapRow(position, child);
                    position = child;
                    child = (2 * position) + 1;
                }
                else
                {
                    return;
                }
            }
        }

        /// <summary>
        /// Sorts the Float value in descending order.
        /// </summary>
        /// <param name="root">Represents the root element in the heap.</param>
        /// <param name="length">Represents the length of the heap.</param>
        /// <param name="columnIndex">Represents the column index of the data to sort.</param>
        public void SortFloatDesc(int root, int length, int columnIndex)
        {
            int counter;
            int temp;

            for (counter = (length - 1) / 2; counter >= root; counter--)
            {
                CreateDescFloatHeap(counter, length - 1, columnIndex);
            }
            for (counter = length - 1; counter > root; counter--)
            {
                SwapRow(counter, 0);
                CreateDescFloatHeap(0, counter - 1, columnIndex);
            }

        }
        /// <summary>
        /// Create the Heap with the Date type in Descending order.
        /// </summary>
        /// <param name="position">Represents the posiont of the heap.</param>
        /// <param name="length">Represents the length of the heap.</param>
        /// <param name="columnIndex">Represents the column index of the data to sort.</param>
        void CreateDescDateHeap(int position, int length, int columnIndex)
        {
            int child = (2 * position) + 1;
            int temp;

            while (child <= length)
            {
                if (child < length && (DateTime)m_data[child][columnIndex] > (DateTime)m_data[child + 1][columnIndex])
                {
                    child++;
                }

                if ((DateTime)m_data[position][columnIndex] > (DateTime)m_data[child][columnIndex])
                {
                    SwapRow(position, child);
                    position = child;
                    child = (2 * position) + 1;
                }
                else
                {
                    return;
                }
            }
        }

        /// <summary>
        /// Sorts the Date value in descending order.
        /// </summary>
        /// <param name="root">Represents the root element in the heap.</param>
        /// <param name="length">Represents the length of the heap.</param>
        /// <param name="columnIndex">Represents the column index of the data to sort.</param>
        public void SortDateDesc(int root, int length, int columnIndex)
        {
            int counter;
            int temp;

            for (counter = (length - 1) / 2; counter >= root; counter--)
            {
                CreateDescDateHeap(counter, length - 1, columnIndex);
            }
            for (counter = length - 1; counter > root; counter--)
            {
                SwapRow(counter, 0);
                CreateDescDateHeap(0, counter - 1, columnIndex);
            }
        }
        /// <summary>
        /// Create the Heap with the String type in Descending order.
        /// </summary>
        /// <param name="position">Represents the posiont of the heap.</param>
        /// <param name="length">Represents the length of the heap.</param>
        /// <param name="columnIndex">Represents the column index of the data to sort.</param>
        void CreateDescStringHeap(int position, int length, int columnIndex)
        {
            int child = (2 * position) + 1;
            int temp;

            while (child <= length)
            {
                if (child < length && ((string)m_data[child][columnIndex]).CompareTo((string)m_data[child + 1][columnIndex]) > 0)
                {
                    child++;
                }

                if (((string)m_data[position][columnIndex]).CompareTo((string)m_data[child][columnIndex]) > 0)
                {
                    SwapRow(position, child);
                    position = child;
                    child = (2 * position) + 1;
                }
                else
                {
                    return;
                }
            }
        }

        /// <summary>
        /// Sorts the String value in descending order.
        /// </summary>
        /// <param name="root">Represents the root element in the heap.</param>
        /// <param name="length">Represents the length of the heap.</param>
        /// <param name="columnIndex">Represents the column index of the data to sort.</param>

        public void SortStringDesc(int root, int length, int columnIndex)
        {
            int counter;
            int temp;

            for (counter = (length - 1) / 2; counter >= root; counter--)
            {
                CreateDescStringHeap(counter, length - 1, columnIndex);
            }
            for (counter = length - 1; counter > root; counter--)
            {
                SwapRow(counter, 0);
                CreateDescStringHeap(0, counter - 1, columnIndex);
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



    }
}
