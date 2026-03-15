//-------------------------------------------------------------------------------------------------
// <copyright file="SummaryElements.cs" company="syncfusion">
//  Copyright (c) Syncfusion Inc. 2001 - 2014. All rights reserved.
//  Use of this code is subject to the terms of our license.
//  A copy of the current license can be obtained at any time by e-mailing
//  licensing@syncfusion.com. Re-distribution in any form is strictly
//  prohibited. Any infringement will be prosecuted under applicable laws. 
// </copyright>
//-------------------------------------------------------------------------------------------------

using System;
using System.Collections.Generic;


#if !SILVERLIGHT
namespace Syncfusion.Olap.Reports
#else
namespace Syncfusion.OlapSilverlight.Reports
#endif
{
#if !SILVERLIGHT
    /// <summary>
    /// Represents the summary element information (i.e., Measure or Fact section of Non-OLAP (IEnumerable or IList or DataTable) data).
    /// </summary>
    [Serializable]
#endif
    public class SummaryElements : Element
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="SummaryElements"/> class.
        /// </summary>
        public SummaryElements()
        {
            this.SummaryCollection = new List<SummaryInfo>();            
        }

        /// <summary>
        /// Gets or sets the summary collection.
        /// </summary>
        /// <value>The summary collection.</value>
        public List<SummaryInfo> SummaryCollection { get; private set; }

        /// <summary>
        /// Adds the specified summary info.
        /// </summary>
        /// <param name="summaryInfo">The summary info.</param>
        public void Add(SummaryInfo summaryInfo)
        {
            this.SummaryCollection.Add(summaryInfo);
        }

        /// <summary>
        /// Inserts the specified index.
        /// </summary>
        /// <param name="index">The index.</param>
        /// <param name="summaryInfo">The summary info.</param>
        public void Insert(int index, SummaryInfo summaryInfo)
        {
            this.SummaryCollection.Insert(index, summaryInfo);
        }

        /// <summary>
        /// Removes the specified summary info.
        /// </summary>
        /// <param name="summaryInfo">The summary info.</param>
        public void Remove(SummaryInfo summaryInfo)
        {
            this.SummaryCollection.Remove(summaryInfo);
        }

#if !SILVERLIGHT
        /// <summary>
        /// Removes all.
        /// </summary>
        /// <param name="match">The match.</param>
        public void RemoveAll(Predicate<SummaryInfo> match)
        {
            this.SummaryCollection.RemoveAll(match);
        }
#endif
        /// <summary>
        /// Removes at specified index.
        /// </summary>
        /// <param name="index">The index.</param>
        public void RemoveAt(int index)
        {
            this.SummaryCollection.RemoveAt(index);
        }
    }
}
