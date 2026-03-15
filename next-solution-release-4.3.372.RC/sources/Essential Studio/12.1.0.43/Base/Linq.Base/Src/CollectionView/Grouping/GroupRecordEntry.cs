#region Copyright Syncfusion Inc. 2001 - 2014
// Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Any infringement will be prosecuted under
// applicable laws. 
#endregion
namespace Syncfusion.Windows.Data
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Linq;
    using System.Collections.Specialized;
#if !SILVERLIGHT
    using System.Data;
#endif

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
            this.Records = new RecordsEntryList(); //new List<RecordEntry>();
            this.Summaries = new List<SummaryRecordEntry>();
            this.IsRecords = true;
            this.UnfilteredRecords = new List<object>();
            this.topLevelGroup = this.Parent.GetTopLevelGroup();
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="GroupRecordEntry"/> class.
        /// </summary>
        /// <param name="parent">The parent.</param>
        /// <param name="level">The level.</param>
        /// <param name="source">The source.</param>
        /// <param name="filterPredicate"></param>
        public GroupRecordEntry(NodeEntry parent, int level, IEnumerable source, Predicate<object> filterPredicate)
            : this(parent, level)
        {
            this.UnfilteredRecords = new List<object>(source.Cast<object>());
            this.PopulateRecords(source, filterPredicate);
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

#if !SILVERLIGHT
        private bool IsDataTable(TopLevelGroup topLevelGroup)
        {
            var result = false;
            if (topLevelGroup.CollectionView.SourceCollection is DataView)
            {
                result = true;
            }
            else if (topLevelGroup.CollectionView.SourceCollection is DataTable)
            {
                result = true;
            }
            return result;
        }
#endif

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
            this.Records.ResumeUpdates();
        }



        /// <summary>
        /// This overload Method used to find the GroupRecords for paging Support
        /// </summary>
        /// <param name="source"></param>
        /// <param name="filterPredicate"></param>
        /// <param name="isViewLevelPaging"></param>
        public virtual void PopulateRecords(IEnumerable source, Predicate<object> filterPredicate, bool isViewLevelPaging)
        {
            List<object> _list = new List<object>();
            foreach (var item in this.Records)
            {
                _list.Add(item.Data);
            }

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
                    if (_list.Contains(record.Data))
                        this.Records.Add(record);
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
            RecordEntry record = this.topLevelGroup.CollectionView.CreateRecordEntry(data);
            if (record == null)
            {
                record = new RecordEntry(this.Parent, -1, data);
            }

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
            private set;
        }

        /// <summary>
        /// Gets the records count.
        /// </summary>
        /// <returns></returns>
        public virtual int GetRecordsCount()
        {
            int rcount = this.GetRelationsCount();
            if (rcount != 0)
            {
                return this.Records.Count * (rcount + 1);
            }
            else
            {
                return this.Records.Count;
            }
        }

        private TopLevelGroup topLevelGroup = null;
        /// <summary>
        /// Gets the relations count.
        /// </summary>
        /// <returns></returns>
        public virtual int GetRelationsCount()
        {
            //if (this.cachedTopLevelGroup == null)
            //{
            //this.topLevelGroup = this.GetTopLevelGroup();
            //}
            return this.topLevelGroup.RelationsCount;
        }

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
