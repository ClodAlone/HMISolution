#region Copyright Syncfusion Inc. 2001 - 2014
// Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Any infringement will be prosecuted under
// applicable laws. 
#endregion

namespace Syncfusion.XlsIO
{
    public interface ISortingAlgorithm
    {
        /// <summary>
        /// Range object to sort.
        /// </summary>
        IRange Range { get; set; }
        /// <summary>
        /// Sorts the integer type data with the specified column index.
        /// </summary>
        /// <param name="left">Represents the start index of the data.</param>
        /// <param name="right">Represents the end index of the data.</param>
        /// <param name="columnIndex">Reprsents the columnindex of the data.</param>
        void SortInt(int left, int right, int columnIndex);
        /// <summary>
        /// Sorts the Float values with the specified column.
        /// </summary>
        /// <param name="left">Represents the start index of the data.</param>
        /// <param name="right">Represents the end index of the data.</param>
        /// <param name="columnIndex">Reprsents the columnindex of the data.</param>
        void SortFloat(int left, int right, int columnIndex);
        /// <summary>
        /// Sorts the Date type values with the specified column.
        /// </summary>
        /// <param name="left">Represents the start index of the data.</param>
        /// <param name="right">Represents the end index of the data.</param>
        /// <param name="columnIndex">Reprsents the columnindex of the data.</param>
        void SortDate(int left, int right, int columnIndex);
        /// <summary>
        /// Sorts the string type values with the specified column.
        /// </summary>
        /// <param name="left">Represents the start index of the data.</param>
        /// <param name="right">Represents the end index of the data.</param>
        /// <param name="columnIndex">Reprsents the columnindex of the data.</param>
        void SortString(int left, int right, int columnIndex);
        /// <summary>
        /// Sorts the based on the types with the specified column.
        /// </summary>
        /// <param name="left">Represents the start index of the data.</param>
        /// <param name="right">Represents the end index of the data.</param>
        /// <param name="columnIndex">Reprsents the columnindex of the data.</param>
        void SortOnTypes(int left, int right, int columnIndex);
        /// <summary>
        /// Sorts the Integer type values in Descending order with the specified column.
        /// </summary>
        /// <param name="left">Represents the start index of the data.</param>
        /// <param name="right">Represents the end index of the data.</param>
        /// <param name="columnIndex">Reprsents the columnindex of the data.</param>
        void SortIntDesc(int left, int right, int columnIndex);
        /// <summary>
        /// Sorts the Float type values in Descending order with the specified column.
        /// </summary>
        /// <param name="left">Represents the start index of the data.</param>
        /// <param name="right">Represents the end index of the data.</param>
        /// <param name="columnIndex">Reprsents the columnindex of the data.</param>
        void SortFloatDesc(int left, int right, int columnIndex);
        /// <summary>
        /// Sorts the Datea type values in Descending order with the specified column.
        /// </summary>
        /// <param name="left">Represents the start index of the data.</param>
        /// <param name="right">Represents the end index of the data.</param>
        /// <param name="columnIndex">Reprsents the columnindex of the data.</param>
        void SortDateDesc(int left, int right, int columnIndex);
        /// <summary>
        /// Sorts the string type values in Descending order with the specified column.
        /// </summary>
        /// <param name="left">Represents the start index of the data.</param>
        /// <param name="right">Represents the end index of the data.</param>
        /// <param name="columnIndex">Reprsents the columnindex of the data.</param>
        void SortStringDesc(int left, int right, int columnIndex);
    }
}
