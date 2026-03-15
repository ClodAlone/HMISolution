#region Copyright Syncfusion Inc. 2001 - 2014
// Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Any infringement will be prosecuted under
// applicable laws. 
#endregion
namespace Syncfusion.Data
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Linq;
    using Syncfusion.Data.Extensions;
    using System.Collections.Specialized;

    /// <summary>
    /// Contains a list of records with its related summaries and unfiltered records. The <see cref="Group"/> class uses
    /// GroupRecordEntry if the group.IsBottomLevel = true;.
    /// </summary>
    public class GroupRecordEntry : NodeEntry
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="GroupRecordEntry"/> class.
        /// </summary>
        /// <param name="parent">The parent.</param>
        /// <param name="level">The level.</param>
        public GroupRecordEntry(NodeEntry parent, int level)
            : base((NodeEntry)parent, level)
        {
             //new List<RecordEntry>();
            InitializeRecords();
            this.UnfilteredRecords = new List<object>();
            this.Summaries = new List<SummaryRecordEntry>();
            this.IsRecords = true;
            this.topLevelGroup = this.Parent.GetTopLevelGroup();
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="GroupRecordEntry"/> class.
        /// </summary>
        /// <param name="parent">The parent.</param>
        /// <param name="level">The level.</param>
        /// <param name="source">The source.</param>
        public GroupRecordEntry(NodeEntry parent, int level, IEnumerable source, Predicate<object> filterPredicate)
            : this(parent, level)
        {
            this.UnfilteredRecords = new List<object>(source.Cast<object>());
            this.PopulateRecords(source, filterPredicate);
        }

        protected virtual void InitializeRecords()
        {
            this.Records = new RecordsEntryList();
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
                this.topLevelGroup = null;
                this.Records.Clear();
                this.UnfilteredRecords = null;
                this.Summaries.Clear();
            }
        }


        /// <summary>
        /// Populates the records.
        /// </summary>
        /// <param name="source">The source.</param>
        /// <param name="filterPredicate">The filter predicate.</param>
        public virtual void PopulateRecords(IEnumerable source, Predicate<object> filterPredicate)
        {
            this.Records.SuspendUpdates();
            this.Records.Clear();
            var enumerator = source.GetEnumerator();
            while (enumerator.MoveNext())
            {
                var currentRecord = enumerator.Current;
                var canAdd = true;
                if (filterPredicate != null)
                {
                    canAdd = filterPredicate(currentRecord);
                }
                if (canAdd)
                {
                    var record = this.CreateRecord(enumerator.Current);
                    this.Records.Add(record);
                }
            }

            if (this.Records.Count == 0)
            {
                
                // Resetting Value for Caption Summaries
                if ((this.Parent as Group) != null && (this.Parent as Group).SummaryDetails != null)
                {
                    (this.Parent as Group).SummaryDetails.SummaryValues.ForEach(val => 
                    {
                        for (int i = 0; i < val.AggregateValues.Count; i++)
                        {
                            val.AggregateValues[val.AggregateValues.ElementAt(i).Key] = 0;
                        }
                    });
                }

                // Resetting value for Group Summaries

                if (this.Summaries.Count > 0)
                {
                    this.Summaries.ForEach(summary =>
                    {
                        summary.SummaryValues.ForEach(val =>
                        {
                            for (int i = 0; i < val.AggregateValues.Count; i++)
                            {
                                val.AggregateValues[val.AggregateValues.ElementAt(i).Key] = 0;
                            }
                        });
                    });
                }
            }
                
            this.Records.ResumeUpdates();
        }

        /// <summary>
        /// Creates the record.
        /// </summary>
        /// <param name="data">The data.</param>
        /// <returns></returns>
        protected virtual RecordEntry CreateRecord(object data)
        {
            // var toplevelGroup = this.Parent.GetTopLevelGroup();
            var record = this.topLevelGroup.CollectionView.CreateRecordEntry(data) ??
                         new RecordEntry(this.Parent, -1, data);

            record.Parent = this.Parent;
            record.Level = this.Level;
            return record;
        }

        /// <summary>
        /// Gets the unfiltered records.
        /// </summary>
        /// <value>The unfiltered records.</value>
        public List<object> UnfilteredRecords
        {
            get;
            private set;
        }

        /// <summary>
        /// Gets or sets the records.
        /// </summary>
        /// <value>The records.</value>
        public IRecordsEntryList Records
        {
            get;
            protected set;
        }

        /// <summary>
        /// Gets the records count.
        /// </summary>
        /// <returns></returns>
        public virtual int GetRecordsCount()
        {
            var rcount = this.GetRelationsCount();
            if (rcount != 0)
                return this.Records.Count*(rcount + 1);
            return this.Records.Count;
        }

        /// <summary>
        /// Gets the relations count.
        /// </summary>
        /// <returns></returns>
        public virtual int GetRelationsCount()
        {
            return this.topLevelGroup.RelationsCount;
        }

        private TopLevelGroup topLevelGroup = null;

        /// <summary>
        /// returns the underlying objects array from the record entries.
        /// </summary>
        /// <returns></returns>
        public object[] ToArray()
        {
            return this.Records.Select(o => o.Data).ToArray();
        }

        /// <summary>
        /// Gets the summaries for the bottom level records.
        /// </summary>
        /// <value>The summaries.</value>
        public List<SummaryRecordEntry> Summaries
        {
            get;
            private set;
        }
    }
}
