#region Copyright Syncfusion Inc. 2001 - 2014
// Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Any infringement will be prosecuted under
// applicable laws. 
#endregion
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Syncfusion.Data
{
    /// <summary>
    /// Implements an IList structure to contain the list of RecordEntry.
    /// </summary>
    public interface IRecordsList : IRecordsEntryList
    {
        /// <summary>
        /// Gets the table summaries.
        /// </summary>
        /// <value>The table summaries.</value>
        IList<SummaryRecordEntry> TableSummaries { get; }

        /// <summary>
        /// Returns the index for the underlying record.
        /// </summary>
        /// <param name="data">The data.</param>
        /// <returns></returns>
        //int IndexOfRecord(object data);


        /// <summary>
        /// Gets the item at index specified.
        /// </summary>
        /// <param name="recordIndex">Index of the record.</param>
        /// <returns></returns>
        object GetItemAt(int recordIndex);


        /// <summary>
        /// Gets the <see cref="RecordEntry"/> for the underlying business object.
        /// </summary>
        /// <param name="data">The data.</param>
        /// <returns></returns>
        RecordEntry GetRecord(object data);


        /// <summary>
        /// Creates the <see cref="RecordEntry"/> for the business object.
        /// </summary>
        /// <param name="data">The data.</param>
        /// <returns></returns>
        RecordEntry CreateRecordEntry(object data);

        /// <summary>
        /// Dispose all items from the <see cref="T:System.Collections.Generic.ICollection`1"/>.
        /// </summary>
        void ReomveAll();
    }
}