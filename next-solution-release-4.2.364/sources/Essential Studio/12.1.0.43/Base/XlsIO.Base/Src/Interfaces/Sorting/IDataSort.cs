#region Copyright Syncfusion Inc. 2001 - 2014
// Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Any infringement will be prosecuted under
// applicable laws. 
#endregion

namespace Syncfusion.XlsIO
{
    /// <summary>
    /// Represents the sort of range.
    /// </summary>
    public interface IDataSort
    {
        #region Properties
        /// <summary>
        /// Indicates whether to perform case sensitive sort.
        /// </summary>
        bool IsCaseSensitive { get; set; }
        /// <summary>
        /// Indicates whether the range has header.
        /// </summary>
        bool HasHeader { get; set; }
        /// <summary>
        /// Represents the sort orientation.
        /// </summary>
        SortOrientation Orientation { get; set; }
        /// <summary>
        /// Represents the SortFields Collection.
        /// </summary>
        ISortFields SortFields { get; set; }
        /// <summary>
        /// Represents	 the sort range.
        /// </summary>
        IRange SortRange { get; set; }
        /// <summary>
        /// Represents the algorithm to sort.
        /// </summary>
        SortingAlgorithms Algorithm { get; set; }
        #endregion

        #region Methods
        /// <summary>
        /// Sorts the range based on the sort fields.
        /// </summary>
        void Sort();
        #endregion
    }
}
