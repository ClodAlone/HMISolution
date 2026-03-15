#region Copyright Syncfusion Inc. 2001 - 2014
// Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Any infringement will be prosecuted under
// applicable laws. 
#endregion
using Syncfusion.Data.Extensions;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
#if !SILVERLIGHT && !WP7
using System.Threading.Tasks;
#endif

namespace Syncfusion.Data
{
    public class VirtualGroup:Group
    {
        #region Internal Members

        internal IEnumerable<object> GroupSource;

        #endregion

        #region Private Members
        IList internalList;
        internal IList InternalList
        {
            get
            {
                if (internalList == null && this.GroupSource != null)
                {
                    internalList = new List<object>(GroupSource);
                }
                return internalList;
            }
        }
        Dictionary<int, RecordEntry> recordDictionary;
        int virtualItemCount = 10;

        #endregion

        #region Ctor

        public VirtualGroup(Group parent, int level)
            : base(parent, level)
        {
            recordDictionary = new Dictionary<int, RecordEntry>();
            if (GroupSource == null)
            {
                GroupSource = new List<object>();
            }
        }

        #endregion

        #region Overrides

        public override int ItemsCount
        {
            get
            {
                if (this.Groups != null)
                {
                    return this.GetGroupsCount();
                }
                else
                {
                    if (InternalList != null && InternalList.Count > 0)
                        return InternalList.Count;
                    return this.Records.Count;
                }
            }
        }

        protected override Group CreateNewGroup(Group parent, Extensions.GroupResult groupResult, int level)
        {
            return new VirtualGroup(parent, level) { Key = groupResult.Key, GroupSource = groupResult.Items.OfType<object>() };
        }

        public override Group CreateNewGroup(Group parent, object Key, int level)
        {
            return new VirtualGroup(parent, level) { Key = Key };
        }

        public override int GetRecordCount()
        {
            if (this.InternalList != null)
            {
                return InternalList.Count;
            }
            return base.GetRecordCount();
        }

        public override int GetSourceCount()
        {
            if (this.GroupSource != null)
                return this.GroupSource.Count();
            return base.GetGroupsCount();
        }

        public override RecordEntry GetRecordAt(int index)
        {
            if (this.GroupSource != null)
            {
                if (!recordDictionary.ContainsKey(index))
                    this.LoadRecordForIndex(index);
                return this.recordDictionary[index];
            }
            else
                return this.Records[index];
        }

        public override int GetRecordIndex(RecordEntry record)
        {
            if (this.InternalList != null && this.GroupSource != null)
                return InternalList.IndexOf(record.Data);
            else
                return Records.IndexOf(record);
        }

        public override int GetRecordIndex(object item)
        {
            int index = -1;
            if (item is RecordEntry)
                index = this.InternalList.IndexOf((item as RecordEntry).Data);
            else
                index = this.InternalList.IndexOf(item);
            return index;
        }

        public override void AddRecord(RecordEntry record, bool isInSourceCollectionChange)
        {
            if (isInSourceCollectionChange)
                this.AddToGroupSource(record.Data);
            this.AddInternalRecord(record);
        }

        public override void InsertRecord(int index, RecordEntry record, bool isInSourceCollectionChange)
        {
            if (isInSourceCollectionChange)
                this.AddToGroupSource(record.Data);
            this.AddInternalRecord(record);
        }

        public override bool RemoveRecord(RecordEntry record, bool isInSourceCollectionChange)
        {
            if (this.InternalList != null && this.GroupSource != null)
            {
                var removeAtIndex = this.InternalList.IndexOf(record.Data);
                if (removeAtIndex > -1)
                {
                    if (this.recordDictionary.ContainsKey(removeAtIndex))
                        UpdateRecordDictionary(removeAtIndex);
                    this.InternalList.Remove(record.Data);
                    if (isInSourceCollectionChange)
                        this.RemoveFromGroupSource(record.Data);
                    return true;
                }
                return false;
            }
            return base.RemoveRecord(record, isInSourceCollectionChange);
        }

        public override void AddItem(object record)
        {
            this.AddToGroupSource(record);
        }

        public override bool RemoveItem(object record)
        {
            return this.RemoveFromGroupSource(record);
        }

        internal override object[] GetGroupItems()
        {
            if (this.InternalList != null)
                return this.InternalList.Cast<object>().ToArray();
            return base.GetGroupItems();
        }

        protected override void Dispose(bool disposing)
        {
            base.Dispose(disposing);
            if (disposing)
            {
                if (GroupSource != null)
                {
                    GroupSource = null;
                }
                if (this.internalList != null)
                {
                    if (this.internalList.Count > 0)
                    {
                        this.internalList.Clear();
                    }
                    internalList = null;
                }
                if (this.recordDictionary != null)
                {
                    if (this.recordDictionary.Count > 0)
                        this.recordDictionary.Clear();
                    this.recordDictionary = null;
                }
            }
        }

        public override void CreateDetailsForRecords(Group parent, int level)
        {
            this.IsBottomLevel = true;
            this.Details = new VirtualGroupRecordEntry(parent, level);
        }

        #endregion

        #region Private Methods

        private void LoadRecordForIndex(int index)
        {
            if (this.InternalList != null && this.GroupSource != null)
            {
                var startIndex = index;
                var endIndex = index + virtualItemCount;
                endIndex = endIndex > this.InternalList.Count ? InternalList.Count : endIndex;
                while (startIndex < endIndex)
                {
                    if (!recordDictionary.ContainsKey(startIndex))
                    {
                        var item = this.InternalList[startIndex];
                        this.recordDictionary.Add(startIndex, CreateRecordEntry(item));
                    }
                    startIndex++;
                }
            }
        }

        private RecordEntry CreateRecordEntry(object data)
        {
            return new RecordEntry(this, this.Level+1, data);
        }

        private void UpdateRecordDictionary(int changedIndex)
        {
            List<int> needToRemoveIndex = new List<int>();
            foreach (var item in recordDictionary)
            {
                if (item.Key >= changedIndex)
                    needToRemoveIndex.Add(item.Key);
            }

            foreach (var index in needToRemoveIndex)
            {
                recordDictionary.Remove(index);
            }
        }

        private void AddInternalRecord(RecordEntry record)
        {
            //When the new group group created at Runtime, InternalList will be manipulated from GroupSource. No need to add again in InternalList.
            //This will lead to adding duplicate record in Group.
            if (internalList == null)
                return;
            this.InternalList.Add(record.Data);
        }

        private void AddToGroupSource(object record)
        {
            var addedRecords = new List<object>();
            addedRecords.Add(record);
            GroupSource = GroupSource.Concat(addedRecords);
        }

        private bool RemoveFromGroupSource(object record)
        {
            var removedRecords = new List<object>();
            removedRecords.Add(record);
            GroupSource = GroupSource.Except(removedRecords);
            return true;
        }

        #endregion

        #region Internal Methods

        internal void ApplyFiltering(Predicate<object> filter)
        {
            if (filter != null && this.internalList != null)
            {
                this.InternalList.Clear();
                var enumerator = this.GroupSource.GetEnumerator();
                while (enumerator.MoveNext())
                {
                    if (filter(enumerator.Current))
                    {
                        InternalList.Add(enumerator.Current);
                    }
                }
            }
            this.recordDictionary.Clear();
        }

        #endregion
    }
}
