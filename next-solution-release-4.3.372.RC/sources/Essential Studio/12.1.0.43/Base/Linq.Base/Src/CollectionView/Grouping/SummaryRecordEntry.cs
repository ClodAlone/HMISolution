#region Copyright Syncfusion Inc. 2001 - 2014
// Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Any infringement will be prosecuted under
// applicable laws. 
#endregion
namespace Syncfusion.Windows.Data
{
    using System.Collections.Generic;

    /// <summary>
    /// Contains the list of Summary aggregates computed using <see cref="ISummaryRow" /> instance in this class.
    /// </summary>
    public class SummaryRecordEntry : NodeEntry
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="SummaryRecordEntry"/> class.
        /// </summary>
        /// <param name="parent">The parent.</param>
        /// <param name="level">The level.</param>
        public SummaryRecordEntry(NodeEntry parent, int level)
            : base(parent, level)
        {
            this.SummaryValues = new List<SummaryValue>();
        }

        /// <summary>
        /// Releases unmanaged and - optionally - managed resources
        /// </summary>
        /// <param name="disposing"><c>true</c> to release both managed and unmanaged resources; <c>false</c> to release only unmanaged resources.</param>
        protected override void Dispose(bool disposing)
        {
            base.Dispose(disposing);
            if (disposing)
            {
                this.SummaryValues.Clear();
                this.SummaryRow = null;
            }
        }

        /// <summary>
        /// Gets the summary values.
        /// </summary>
        /// <value>The summary values.</value>
        public List<SummaryValue> SummaryValues
        {
            get;
            private set;
        }

        /// <summary>
        /// Gets or sets the summary row.
        /// </summary>
        /// <value>The summary row.</value>
        public ISummaryRow SummaryRow
        {
            get;
            set;
        }
    }

    /// <summary>
    /// Instance to cache the summary aggregate values into the <see cref="SummaryRecordEntry" /> instance.
    /// </summary>
    public class SummaryValue
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="SummaryValue"/> class.
        /// </summary>
        public SummaryValue()
        {
            this.AggregateValues = new Dictionary<string, object>();
        }

        /// <summary>
        /// Gets or sets the name.
        /// </summary>
        /// <value>The name.</value>
        public string Name
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the aggregate values.
        /// </summary>
        /// <value>The aggregate values.</value>
        public Dictionary<string, object> AggregateValues
        {
            get;
            private set;
        }
    }
}
