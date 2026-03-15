#region Copyright Syncfusion Inc. 2001 - 2014
// Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Any infringement will be prosecuted under
// applicable laws. 
#endregion
using System;
using System.Collections;

#if ( WINRT )
using Windows.UI;
using System.Collections.Generic;
#endif

#if  (SILVERLIGHT || WP)
using System.Windows.Media;
using System.Collections.Generic;
#elif ( WINRT )
using Syncfusion.XlsIO;
#else
using System.Drawing;

#endif
namespace Syncfusion.XlsIO.Implementation.Sorting
{
    /// <summary>
    /// This class used to sort the data using MergeSort.
    /// </summary>
    class MergeSortImpl : SortingAlgorithm
    {

        #region Intialization
        /// <summary>
        /// Initailize the sort data attributes.
        /// </summary>
        /// <param name="data">Represents the data to sort.</param>
        /// <param name="types">Represents the datatype of each column.</param>
        /// <param name="orderBy">Represents the sort order.</param>
        /// <param name="colors">Represents the color to sort.</param>
        public MergeSortImpl(object[][] data, Type[] types, OrderBy[] orderBy, Color[] colors)
            : base(data, types, orderBy, colors)
        {
        }
        #endregion
        /// <summary>
        /// Sort based on their types.
        /// </summary>
        /// <param name="arrValues">Represents the Data To Sort.</param>
        /// <param name="columnIndex">Represents the column index.</param>
        /// <returns>Sorted Data.</returns>
        public object[][] SortOnTypes(object[][] arrValues, int columnIndex)
        {
            if (types[columnIndex - 1] == typeof(int))
            {
                if (orderBy[columnIndex - 1] == OrderBy.Ascending)
                    return SortInt(arrValues, columnIndex);
                else
                    return SortIntDesc(arrValues, columnIndex);
            }
            else if (types[columnIndex - 1] == typeof(double))
            {
                if (orderBy[columnIndex - 1] == OrderBy.Ascending)
                    return SortFloat(arrValues, columnIndex);
                else
                    return SortFloatDesc(arrValues, columnIndex);
            }
            else if (types[columnIndex - 1] == typeof(string))
            {
                if (orderBy[columnIndex - 1] == OrderBy.Ascending)
                    return SortString(arrValues, columnIndex);
                else
                    return SortStringDesc(arrValues, columnIndex);
            }
            else if (types[columnIndex - 1] == typeof(DateTime))
            {
                if (orderBy[columnIndex - 1] == OrderBy.Ascending)
                    return SortDate(arrValues, columnIndex);
                else
                    return SortDateDesc(arrValues, columnIndex);
            }
            return new object[0][];
        }
        /// <summary>
        /// Gets the specified range from the array.
        /// </summary>
        /// <param name="arrData">Array of data to get the range.</param>
        /// <param name="startIndex">start index.</param>
        /// <param name="endIndex">end index.</param>
        /// <returns>object array.</returns>
        public object[][] GetRange(object[][] arrData, int startIndex, int endIndex)
        {
            object[][] tmp = new object[endIndex - startIndex][];
            int index = 0;
            for (int i = startIndex; i < endIndex; i++)
            {
                tmp[index] = new object[arrData[0].Length];
                tmp[index++] = arrData[i];
            }
            return tmp;
        }
        /// <summary>
        /// Adds the source content to the destination array content from the start index.
        /// </summary>
        /// <param name="destArray">Destination array.</param>
        /// <param name="srcArray">Source Array.</param>
        /// <param name="startIndex">Start Index.</param>
        public void AddRange(object[][] destArray, object[][] srcArray, int startIndex)
        {
            for (int i = 0; i < srcArray.Length; i++)
                destArray[startIndex++] = srcArray[i];
        }
        #region Methods which sort rows in ascending.
        /// <summary>
        /// Sort the data in Ascending order.
        /// </summary>
        /// <param name="arrData">Represents the array of data to sort.</param>
        /// <param name="columnIndex">Represents the column Index.</param>
        /// <returns>Sorted Data.</returns>
        public object[][] SortInt(object[][] arrData, int columnIndex)
        {
            if (arrData.Length == 1)
            {
                return arrData;
            }
            object[][] arrSortedInt = new object[arrData.Length][];
            int middle = (int)arrData.Length / 2;
            int sortedIndex = 0;
            object[][] leftArray = GetRange(arrData, 0, middle);
            object[][] rightArray = GetRange(arrData, middle, arrData.Length);
            leftArray = SortInt(leftArray, columnIndex);
            rightArray = SortInt(rightArray, columnIndex);
            int leftptr = 0;
            int rightptr = 0;
            for (int i = 0; i < leftArray.Length + rightArray.Length; i++)
            {
                if (leftptr == leftArray.Length)
                {
                    arrSortedInt[sortedIndex++] = rightArray[rightptr];
                    rightptr++;
                }
                else if (rightptr == rightArray.Length)
                {
                    arrSortedInt[sortedIndex++] = leftArray[leftptr];
                    leftptr++;
                }
                else
                {
                    int leftValue = (int)leftArray[leftptr][columnIndex];
                    int rightValue = (int)rightArray[rightptr][columnIndex];
                    if (leftValue < rightValue)
                    {
                        //need the cast above since object[][]   returns Type object
                        arrSortedInt[sortedIndex++] = leftArray[leftptr];
                        leftptr++;
                    }
                    else if (columnIndex + 1 <= count && leftValue == rightValue)
                    {
                        object[][] tmp = new object[2][];
                        tmp[0] = leftArray[leftptr];
                        tmp[1] = rightArray[rightptr];
                        tmp = SortOnTypes(tmp, ++columnIndex);
                        AddRange(arrSortedInt, tmp, sortedIndex);
                        leftptr++;
                        rightptr++;
                        i++;
                    }
                    else
                    {
                        arrSortedInt[sortedIndex++] = rightArray[rightptr];
                        rightptr++;
                    }
                }


            }
            return arrSortedInt;
        }
        /// <summary>
        /// Sort the data in Ascending order.
        /// </summary>
        /// <param name="arrData">Represents the array of data to sort.</param>
        /// <param name="columnIndex">Represents the column Index.</param>
        /// <returns>Sorted Data.</returns>
        public object[][] SortString(object[][] arrData, int columnIndex)
        {
            if (arrData.Length == 1)
            {
                return arrData;
            }
            object[][] arrSortedInt = new object[arrData.Length][];
            int sortedIndex = 0;
            int middle = (int)arrData.Length / 2;
            object[][] leftArray = GetRange(arrData, 0, middle);
            object[][] rightArray = GetRange(arrData, middle, arrData.Length);
            leftArray = SortString(leftArray, columnIndex);
            rightArray = SortString(rightArray, columnIndex);
            int leftptr = 0;
            int rightptr = 0;
            for (int i = 0; i < leftArray.Length + rightArray.Length; i++)
            {
                if (leftptr == leftArray.Length)
                {
                    arrSortedInt[sortedIndex++] = rightArray[rightptr];
                    rightptr++;
                }
                else if (rightptr == rightArray.Length)
                {
                    arrSortedInt[sortedIndex++] = leftArray[leftptr];
                    leftptr++;
                }
                else
                {
                    string leftValue = (string)leftArray[leftptr][columnIndex];
                    string rightValue = (string)rightArray[rightptr][columnIndex];
                    if (leftValue.CompareTo(rightValue) < 0)
                    {
                        //need the cast above since object[][]   returns Type object
                        arrSortedInt[sortedIndex++] = leftArray[leftptr];
                        leftptr++;
                    }
                    else if (columnIndex + 1 <= count && leftValue == rightValue)
                    {
                        object[][] tmp = new object[2][];
                        tmp[0] = leftArray[leftptr];
                        tmp[1] = rightArray[rightptr];
                        tmp = SortOnTypes(tmp, ++columnIndex);
                        AddRange(arrSortedInt, tmp, sortedIndex);
                        sortedIndex += 2;
                        leftptr++;
                        rightptr++;
                        i++;
                    }
                    else
                    {
                        arrSortedInt[sortedIndex++] = rightArray[rightptr];
                        rightptr++;
                    }
                }


            }
            return arrSortedInt;
        }
        /// <summary>
        /// Sort the data in Ascending order.
        /// </summary>
        /// <param name="arrData">Represents the array of data to sort.</param>
        /// <param name="columnIndex">Represents the column Index.</param>
        /// <returns>Sorted Data.</returns>
        public object[][] SortFloat(object[][] arrData, int columnIndex)
        {
            if (arrData.Length == 1)
            {
                return arrData;
            }
            object[][] arrSortedInt = new object[arrData.Length][];
            int sortedIndex = 0;
            int middle = (int)arrData.Length / 2;
            object[][] leftArray = GetRange(arrData, 0, middle);
            object[][] rightArray = GetRange(arrData, middle, arrData.Length);
            leftArray = SortFloat(leftArray, columnIndex);
            rightArray = SortFloat(rightArray, columnIndex);
            int leftptr = 0;
            int rightptr = 0;
            for (int i = 0; i < leftArray.Length + rightArray.Length; i++)
            {
                if (leftptr == leftArray.Length)
                {
                    arrSortedInt[sortedIndex++] = rightArray[rightptr];
                    rightptr++;
                }
                else if (rightptr == rightArray.Length)
                {
                    arrSortedInt[sortedIndex++] = leftArray[leftptr];
                    leftptr++;
                }
                else
                {
                    double leftValue = (double)leftArray[leftptr][columnIndex];
                    double rightValue = (double)rightArray[rightptr][columnIndex];
                    if (leftValue < rightValue)
                    {
                        //need the cast above since object[][]   returns Type object
                        arrSortedInt[sortedIndex++] = leftArray[leftptr];
                        leftptr++;
                    }
                    else if (columnIndex + 1 <= count && leftValue == rightValue)
                    {
                        object[][] tmp = new object[2][];
                        tmp[0] = leftArray[leftptr];
                        tmp[1] = rightArray[rightptr];
                        tmp = SortOnTypes(tmp, columnIndex + 1);

                        AddRange(arrSortedInt, tmp, sortedIndex);
                        sortedIndex += 2;
                        leftptr++;
                        rightptr++;
                        i++;
                    }
                    else
                    {
                        arrSortedInt[sortedIndex++] = rightArray[rightptr];
                        rightptr++;
                    }
                }


            }
            return arrSortedInt;
        }
        /// <summary>
        /// Sort the data in Ascending order.
        /// </summary>
        /// <param name="arrData">Represents the array of data to sort.</param>
        /// <param name="columnIndex">Represents the column Index.</param>
        /// <returns>Sorted Data.</returns>
        public object[][] SortDate(object[][] arrData, int columnIndex)
        {
            if (arrData.Length == 1)
            {
                return arrData;
            }
            object[][] arrSortedInt = new object[arrData.Length][];
            int middle = (int)arrData.Length / 2;
            int sortedIndex = 0;
            object[][] leftArray = GetRange(arrData, 0, middle);
            object[][] rightArray = GetRange(arrData, middle, arrData.Length);
            leftArray = SortDate(leftArray, columnIndex);
            rightArray = SortDate(rightArray, columnIndex);
            int leftptr = 0;
            int rightptr = 0;
            for (int i = 0; i < leftArray.Length + rightArray.Length; i++)
            {
                if (leftptr == leftArray.Length)
                {
                    arrSortedInt[sortedIndex++] = rightArray[rightptr];
                    rightptr++;
                }
                else if (rightptr == rightArray.Length)
                {
                    arrSortedInt[sortedIndex++] = leftArray[leftptr];
                    leftptr++;
                }
                else if(leftArray[leftptr] != null && rightArray[rightptr] !=null)
                {
                    DateTime leftValue = (DateTime)leftArray[leftptr][columnIndex];
                    DateTime rightValue = (DateTime)rightArray[rightptr][columnIndex];
                    if (leftValue < rightValue)
                    {
                        //need the cast above since object[][]   returns Type object
                        arrSortedInt[sortedIndex++] = leftArray[leftptr];
                        leftptr++;
                    }
                    else if (columnIndex + 1 <= count && leftValue == rightValue)
                    {
                        object[][] tmp = new object[2][];
                        tmp[0] = leftArray[leftptr];
                        tmp[1] = rightArray[rightptr];
                        tmp = SortOnTypes(tmp, columnIndex + 1);
                        AddRange(arrSortedInt, tmp, sortedIndex);
                        sortedIndex++;
                        leftptr++;
                        rightptr++;
                        i++;
                    }
                    else
                    {
                        arrSortedInt[sortedIndex++] = rightArray[rightptr];
                        rightptr++;
                    }
                }


            }
            return arrSortedInt;
        }
        #endregion


        #region Methods which sort rows in Descending.
        /// <summary>
        /// Sort the data in Decending order.
        /// </summary>
        /// <param name="arrData">Represents the array of data to sort.</param>
        /// <param name="columnIndex">Represents the column Index.</param>
        /// <returns>Sorted Data.</returns>
        public object[][] SortIntDesc(object[][] arrData, int columnIndex)
        {
            if (arrData.Length == 1)
            {
                return arrData;
            }
            object[][] arrSortedInt = new object[arrData.Length][];
            int middle = (int)arrData.Length / 2;
            int sortedIndex = 0;
            object[][] leftArray = GetRange(arrData, 0, middle);
            object[][] rightArray = GetRange(arrData, middle, arrData.Length);
            leftArray = SortIntDesc(leftArray, columnIndex);
            rightArray = SortIntDesc(rightArray, columnIndex);
            int leftptr = 0;
            int rightptr = 0;
            for (int i = 0; i < leftArray.Length + rightArray.Length; i++)
            {
                if (leftptr == leftArray.Length)
                {
                    arrSortedInt[sortedIndex++] = rightArray[rightptr];
                    rightptr++;
                }
                else if (rightptr == rightArray.Length)
                {
                    arrSortedInt[sortedIndex++] = leftArray[leftptr];
                    leftptr++;
                }
                else
                {
                    int leftValue = (int)leftArray[leftptr][columnIndex];
                    int rightValue = (int)rightArray[rightptr][columnIndex];
                    if (leftValue > rightValue)
                    {
                        arrSortedInt[sortedIndex++] = leftArray[leftptr];
                        leftptr++;
                    }
                    else if (columnIndex + 1 <= count && leftValue == rightValue)
                    {
                        object[][] tmp = new object[2][];
                        tmp[0] = leftArray[leftptr];
                        tmp[1] = rightArray[rightptr];
                        tmp = SortOnTypes(tmp, columnIndex + 1);
                        AddRange(arrSortedInt, tmp, sortedIndex);
                        sortedIndex += 2;
                        leftptr++;
                        rightptr++;
                        i++;
                    }
                    else
                    {
                        arrSortedInt[sortedIndex++] = rightArray[rightptr];
                        rightptr++;
                    }
                }


            }
            return arrSortedInt;
        }
        /// <summary>
        /// Sort the data in Decending order.
        /// </summary>
        /// <param name="arrData">Represents the array of data to sort.</param>
        /// <param name="columnIndex">Represents the column Index.</param>
        /// <returns>Sorted Data.</returns>
        public object[][] SortStringDesc(object[][] arrData, int columnIndex)
        {
            if (arrData.Length == 1)
            {
                return arrData;
            }
            object[][] arrSortedInt = new object[arrData.Length][];

            int middle = (int)arrData.Length / 2;
            int sortedIndex = 0;
            object[][] leftArray = GetRange(arrData, 0, middle);
            object[][] rightArray = GetRange(arrData, middle, arrData.Length);
            leftArray = SortStringDesc(leftArray, columnIndex);
            rightArray = SortStringDesc(rightArray, columnIndex);
            int leftptr = 0;
            int rightptr = 0;
            for (int i = 0; i < leftArray.Length + rightArray.Length; i++)
            {
                if (leftptr == leftArray.Length)
                {
                    arrSortedInt[sortedIndex++] = rightArray[rightptr];
                    rightptr++;
                }
                else if (rightptr == rightArray.Length)
                {
                    arrSortedInt[sortedIndex++] = leftArray[leftptr];
                    leftptr++;
                }
                else
                {
                    string leftValue = (string)leftArray[leftptr][columnIndex];
                    string rightValue = (string)rightArray[rightptr][columnIndex];
                    if (leftValue.CompareTo(rightValue) > 0)
                    {
                        //need the cast above since object[][]   returns Type object
                        arrSortedInt[sortedIndex++] = leftArray[leftptr];
                        leftptr++;
                    }
                    else if (columnIndex + 1 <= count && leftValue == rightValue)
                    {
                        object[][] tmp = new object[2][];
                        tmp[0] = leftArray[leftptr];
                        tmp[1] = rightArray[rightptr];
                        tmp = SortOnTypes(tmp, ++columnIndex);

                        AddRange(arrSortedInt, tmp, sortedIndex);
                        sortedIndex+=2;
                        leftptr++;
                        rightptr++;
                        i++;
                    }
                    else
                    {
                        arrSortedInt[sortedIndex++] = rightArray[rightptr];
                        rightptr++;
                    }
                }


            }
            return arrSortedInt;
        }
        /// <summary>
        /// Sort the data in Decending order.
        /// </summary>
        /// <param name="arrData">Represents the array of data to sort.</param>
        /// <param name="columnIndex">Represents the column Index.</param>
        /// <returns>Sorted Data.</returns>
        public object[][] SortFloatDesc(object[][] arrData, int columnIndex)
        {
            if (arrData.Length == 1)
            {
                return arrData;
            }
            object[][] arrSortedInt = new object[arrData.Length][];
            int middle = (int)arrData.Length / 2;
            int sortedIndex = 0;
            object[][] leftArray = GetRange(arrData, 0, middle);
            object[][] rightArray = GetRange(arrData, middle, arrData.Length);
            leftArray = SortFloatDesc(leftArray, columnIndex);
            rightArray = SortFloatDesc(rightArray, columnIndex);
            int leftptr = 0;
            int rightptr = 0;
            for (int i = 0; i < leftArray.Length + rightArray.Length; i++)
            {
                if (leftptr == leftArray.Length)
                {
                    arrSortedInt[sortedIndex++] = rightArray[rightptr];
                    rightptr++;
                }
                else if (rightptr == rightArray.Length)
                {
                    arrSortedInt[sortedIndex++] = leftArray[leftptr];
                    leftptr++;
                }
                else
                {
                    double leftValue = (double)leftArray[leftptr][columnIndex];
                    double rightValue = (double)rightArray[rightptr][columnIndex];
                    if (leftValue > rightValue)
                    {
                        //need the cast above since object[][]   returns Type object
                        arrSortedInt[sortedIndex++] = leftArray[leftptr];
                        leftptr++;
                    }
                    else if (columnIndex + 1 <= count && leftValue == rightValue)
                    {
                        object[][] tmp = new object[2][];
                        tmp[0] = leftArray[leftptr];
                        tmp[1] = rightArray[rightptr];
                        tmp = SortOnTypes(tmp, columnIndex + 1);
                        AddRange(arrSortedInt, tmp, sortedIndex);
                        sortedIndex += 2;
                        leftptr++;
                        rightptr++;
                        i++;
                    }
                    else
                    {
                        arrSortedInt[sortedIndex++] = rightArray[rightptr];
                        rightptr++;
                    }
                }


            }
            return arrSortedInt;
        }
        /// <summary>
        /// Sort the data in Decending order.
        /// </summary>
        /// <param name="arrData">Represents the array of data to sort.</param>
        /// <param name="columnIndex">Represents the column Index.</param>
        /// <returns>Sorted Data.</returns>
        public object[][] SortDateDesc(object[][] arrData, int columnIndex)
        {
            if (arrData.Length == 1)
            {
                return arrData;
            }
            object[][] arrSortedInt = new object[arrData.Length][];
            int middle = (int)arrData.Length / 2;
            int sortedIndex = 0;
            object[][] leftArray = GetRange(arrData, 0, middle);
            object[][] rightArray = GetRange(arrData, middle, arrData.Length);
            leftArray = SortDateDesc(leftArray, columnIndex);
            rightArray = SortDateDesc(rightArray, columnIndex);
            int leftptr = 0;
            int rightptr = 0;
            for (int i = 0; i < leftArray.Length + rightArray.Length; i++)
            {
                if (leftptr == leftArray.Length)
                {
                    arrSortedInt[sortedIndex++] = rightArray[rightptr];
                    rightptr++;
                }
                else if (rightptr == rightArray.Length)
                {
                    arrSortedInt[sortedIndex++] = leftArray[leftptr];
                    leftptr++;
                }
                else if(leftArray[leftptr] !=null && rightArray[rightptr]!=null)
                {
                    DateTime leftValue = (DateTime)leftArray[leftptr][columnIndex];
                    DateTime rightValue = (DateTime)rightArray[rightptr][columnIndex];
                    if (leftValue > rightValue)
                    {
                        //need the cast above since object[][]   returns Type object
                        arrSortedInt[sortedIndex++] = leftArray[leftptr];
                        leftptr++;
                    }
                    else if (columnIndex + 1 <= count && leftValue == rightValue)
                    {
                        object[][] tmp = new object[2][];
                        tmp[0] = leftArray[leftptr];
                        tmp[1] = rightArray[rightptr];
                        tmp = SortOnTypes(tmp, columnIndex + 1);
                        AddRange(arrSortedInt, tmp, sortedIndex);
                        leftptr++;
                        rightptr++;
                        i++;
                    }
                    else
                    {
                        arrSortedInt[sortedIndex++] = rightArray[rightptr];
                        rightptr++;
                    }
                }


            }
            return arrSortedInt;
        }
        #endregion

        #region Overrided Methods
        /// <summary>
        /// Sort the data using MergeSort.
        /// </summary>
        /// <param name="left">Represents the left position of the data.</param>
        /// <param name="right">Represents the right psoition of the data.</param>
        public override void Sort(int left, int right, int columnIndex)
        {
            m_data = SortOnTypes(m_data,columnIndex);
        }
        #endregion

        #region ISorting Algorithm Methods
        public void SortInt(int left, int right, int columnIndex)
        {
            m_data = SortInt(m_data, columnIndex);
        }
        public void SortFloat(int left, int right, int columnIndex)
        {
            m_data = SortFloat(m_data, columnIndex);
        }
        public void SortDate(int left, int right, int columnIndex)
        {
            m_data = SortDate(m_data, columnIndex);
        }
        public void SortString(int left, int right, int columnIndex)
        {
            m_data = SortString(m_data, columnIndex);
        }
        public void SortOnTypes(int left, int right, int columnIndex)
        {
            m_data = SortOnTypes(m_data, columnIndex);
        }
        public void SortIntDesc(int left, int right, int columnIndex)
        {
            m_data = SortIntDesc(m_data, columnIndex);
        }
        public void SortFloatDesc(int left, int right, int columnIndex)
        {
            m_data = SortFloatDesc(m_data, columnIndex);
        }
        public void SortDateDesc(int left, int right, int columnIndex)
        {
            m_data = SortDateDesc(m_data, columnIndex);
        }
        public void SortStringDesc(int left, int right, int columnIndex)
        {
            m_data = SortStringDesc(m_data, columnIndex);
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
