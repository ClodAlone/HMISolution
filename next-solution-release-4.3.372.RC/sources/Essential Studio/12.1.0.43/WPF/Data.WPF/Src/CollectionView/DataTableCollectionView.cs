#region Copyright Syncfusion Inc. 2001 - 2014
// Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Any infringement will be prosecuted under
// applicable laws. 
#endregion
using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows.Data;
using Syncfusion.Data.Extensions;

namespace Syncfusion.Data
{
    public class DataTableCollectionView : CollectionViewAdv
    {
        public DataView ViewSource { get; private set; }

        public DataTableCollectionView(IEnumerable source)
            : base(source)
        {
            this.IsLegacyDataTable = true;
            this.EnsureSourceList();
        }

        protected override IEnumerable GetSource()
        {
            return ViewSource;
        }

        protected override void SetSource(IEnumerable _source)
        {
            this.SetSourceList(_source);
            base.SetSource(_source);
        }

        private void SetSourceList(IEnumerable source)
        {
            DataView dv = null;
            if (source is DataTable)
            {
                var table = source as DataTable;
                dv = table.AsDataView();
            }
            else if (source is DataView)
                dv = source as DataView;
            else
                throw new InvalidOperationException("Cannot find DataTable / DataView source list");
            this.ViewSource = dv;
        }

        protected override IRecordsList CreateRecords()
        {
            IEnumerable source = this.ViewSource;
            //if (this.IsGrouping)
            //{
            //    var recordList = new List<object>();
            //    foreach (var record in this.TopLevelGroup)
            //    {
            //        if (record is RecordEntry)
            //        {
            //            recordList.Add(((RecordEntry)record).Data);
            //        }
            //    }
            //    source = recordList.AsQueryable();
            //}
            return EnumerableRecordsWrapper.CreateNew(source, this);
        }

        protected override void OnSortDescriptionChanged(System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
        {
            this.RefreshSort();
            base.OnSortDescriptionChanged(e);
        }

        public override bool FilterRecord(object record)
        {
            return true;
        }

        protected override void RefreshSort()
        {
            if (!this.IsGrouping)
            {
                if (this.SortDescriptions.Count == 0)
                    return;
                this.ViewSource.Sort = GetSortString();
            }
            else
            {
                this.TopLevelGroup.SuspendEvents();
                var grpRefresh = this.TopLevelGroup as IGroupRefresh;
                grpRefresh.RefreshSortingOrder();
                //this.ViewSource.Sort = this.GetSortString();
                // no need to call the below method, sorting is handled in GetGroupResult()
                //this.RefreshSortingOrderWithFiltersForBottomLevel(this.TopLevelGroup.Groups, string.Empty, 0);
                this.TopLevelGroup.ResumeEvents();
            }
        }

        private string GetSortString()
        {
            var sortString = string.Empty;
            this.SortDescriptions.IterateIndex((i, s) =>
            {
                if (s.Direction == ListSortDirection.Ascending)
                {
                    sortString = i == 0 ? sortString.OrderBy(s.PropertyName) : sortString.ThenBy(s.PropertyName);
                }
                else
                {
                    sortString = i == 0 ? sortString.OrderByDescending(s.PropertyName) : sortString.ThenByDescending(s.PropertyName);
                }
            });
            return sortString;
        }

        protected override IEnumerable<GroupResult> GetGroupResult(string[] groupBy)
        {
            return this.ViewSource.GroupByMany(groupBy);
        }

        public override void RefreshFilter()
        {
            var filterString = this.GetFilterString();
            bool needsRefresh = this.ViewSource.RowFilter != string.Empty && filterString == string.Empty;
            this.ViewSource.RowFilter = filterString;

            if (needsRefresh && !IsInEndeferal)
            {
                this.Refresh();
                if (this.IsGrouping && this.TopLevelGroup != null)
                {
                    var grpRefresh = this.TopLevelGroup as IGroupRefresh;
                    grpRefresh.RefreshFilters();
                }
            }
        }

        protected virtual void RefreshSortingOrderWithFiltersForBottomLevel(List<Group> groups, string prevFilter, int level)
        {
            var groupBy = this.GroupDescriptions.OfType<PropertyGroupDescription>().Select(g => g.PropertyName).ToArray();
            int count = groupBy.GetLength(0);
            string filter = string.Empty;
            foreach (var group in groups)
            {
                //filter = prevFilter + string.Format("[{1}] = '{0}'", group.Key, groupBy[level]);
                //filter = prevFilter + string.Format("[{1}] = '{0}'", group.Key, groupBy[level]);
                if (group.Key is DateTime)
                {
                    filter = prevFilter + string.Format("[{1}] = #{0}#", ((DateTime)group.Key).ToString(System.Globalization.CultureInfo.InvariantCulture.DateTimeFormat), groupBy[level]);
                }
                else if (group.Key == null || group.Key is DBNull)
                {
                    filter = prevFilter + string.Format("[{1}] is null", group.Key, groupBy[level]);
                }
                else
                {
                    filter = prevFilter + string.Format("[{1}] = '{0}'", group.Key, groupBy[level]);
                }

                if (group.IsBottomLevel)
                {
                    var groupRecordsEntry = group.Details as GroupRecordEntry;

                    var filteredRecords = new DataView(this.ViewSource.Table, filter, this.ViewSource.Sort, DataViewRowState.CurrentRows);
                    if (this.SortDescriptions.Count == 0)
                    {
                        groupRecordsEntry.PopulateRecords(filteredRecords, null);
                    }
                    else
                    {
                        IEnumerable sortedSource = this.GetSortedSource(filteredRecords);
                        groupRecordsEntry.PopulateRecords(sortedSource, null);
                    }
                    this.TopLevelGroup.UpdateSummaries(group);
                }
                else
                {
                    filter += " AND ";
                    this.RefreshSortingOrderWithFiltersForBottomLevel(group.Groups, filter, level + 1);
                }
            }
        }

        private IEnumerable GetSortedSource(DataView filteredView)
        {
            filteredView.Sort = this.GetSortString();
            return filteredView;
        }

        protected override void SourceListChanged(object sender, ListChangedEventArgs e)
        {
            base.SourceListChanged(sender, e);
        }

        protected override void UpdateCollectionView(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
        {
            RecordEntry record = null;
            switch (e.Action)
            {
                case NotifyCollectionChangedAction.Add:
                    var index = e.NewStartingIndex;
                    foreach (var item in e.NewItems)
                    {
                        record = this.Records.CreateRecordEntry(item);
                        this.Records.Insert(index,record);
                        index++;
                    }
                    break;

                case NotifyCollectionChangedAction.Move:
                    record = this.Records.CreateRecordEntry(e.NewItems[0]);
                    this.Records.RemoveAt(e.OldStartingIndex);
                    this.Records.Insert(e.NewStartingIndex, record);
                    break;

                case NotifyCollectionChangedAction.Remove:
                    this.Records.RemoveAt(e.OldStartingIndex);
                    break;

                case NotifyCollectionChangedAction.Replace:
                    this.Records.RemoveAt(e.OldStartingIndex);
                    record = this.Records.CreateRecordEntry(e.NewItems[0]);
                    this.Records.Insert(e.NewStartingIndex, record);
                    break;

                case NotifyCollectionChangedAction.Reset:
                    this.Refresh();
                    break;
            }
        }

        public  object AddNew()
        {
            if (CurrentAddItem != null)
                throw new InvalidOperationException("CurrentAddItem should be null");
            this.IsInSuspend = true;

            if (this.ViewSource != null)
            {
                var dataRow = ViewSource.Table.NewRow();
                ViewSource.Table.Rows.Add(dataRow);
                //CurrentAddItem = ViewSource[ViewSource.Table.Rows.IndexOf(dataRow)];
            }
            this.IsInSuspend = false;
            return CurrentAddItem;
        }

        public  void CancelNew()
        {
            if (CurrentAddItem == null)
                throw new InvalidOperationException("CurrentAddItem sholud not be null to perform this operation");
            this.IsInSuspend = true;
            if (this.ViewSource !=null)
            {
                this.ViewSource.Table.Rows.Remove((CurrentAddItem as DataRowView).Row);
            }
            this.IsInSuspend = false;
        }

        public  void CommitNew()
        {
            if (CurrentAddItem == null)
                throw new InvalidOperationException("CurrentAddItem should not null when CommitNew");
            this.AddNotifyListener(CurrentAddItem);
            var rowIndex = this.ViewSource.Table.Rows.IndexOf((CurrentAddItem as DataRowView).Row);
            if (rowIndex >= 0)
            {
                this.IsInSourceCollectionChange = true;
                if (!this.IsGrouping)
                {
                    var newRecord = this.Records.CreateRecordEntry(CurrentAddItem);
                    this.Records.Insert(rowIndex, newRecord);
                }
                else
                {
                    this.GroupList.Add(CurrentAddItem, true);
                }
                this.IsInSourceCollectionChange = false;
            }
          //  this.CurrentAddItem = null;
        }
    }

    public static class DataTableCollectionViewExt
    {
        public static String GetFilterString(this DataTableCollectionView view)
        {
            return view.GetFilterString(null, false);
        }

        public static String GetFilterString(this DataTableCollectionView view, string columnName, bool returncolExpression)
        {
            var rowfilter = string.Empty;
            var filterColumns =
                view.FilterPredicates.Where(v => v.FilterPredicates != null && v.FilterPredicates.Count > 0).ToList();

            foreach (var column in filterColumns)
            {
                var colfilter = string.Empty;
                if (columnName != null)
                {
                    if (returncolExpression)
                    {
                        if (!column.MappingName.Equals(columnName))
                            continue;
                    }
                    else
                    {
                        if (column.MappingName.Equals(columnName))
                            continue;
                    }
                }

                for (var i = 0; i < column.FilterPredicates.Count; i++)
                {
                    var fp = column.FilterPredicates[i];
                    var cName = EscapeSpecialChars(column.MappingName);

                    if (fp.FilterValue == DBNull.Value || fp.FilterValue == null)
                    {
                        if (!string.IsNullOrEmpty(colfilter)
                            && (fp.FilterType == FilterType.Equals || fp.FilterType == FilterType.NotEquals))
                        {
                            if (fp.PredicateType == PredicateType.Or)
                                colfilter = colfilter.OrPredicate();
                            else
                                colfilter = colfilter.AndPredicate();
                        }

                        if (fp.FilterType == FilterType.Equals)
                            colfilter += cName + " IS NULL";
                        //colfilter += "IsNull(" + cName + ", 'Null Column')='Null Column'";
                        else if (fp.FilterType == FilterType.NotEquals)
                            colfilter += cName + " IS NOT NULL";
                        //colfilter += "IsNull(" + cName + ", 'Null Column')<>'Null Column'";
                    }
                    else if (fp.FilterValue != null)
                    {
                        if (!string.IsNullOrEmpty(colfilter))
                        {
                            if (fp.PredicateType == PredicateType.Or)
                                colfilter = colfilter.OrPredicate();
                            else
                                colfilter = colfilter.AndPredicate();
                        }
                        colfilter = colfilter.Predicate(cName, fp.FilterValue, fp.FilterType);
                    }
                }
                if (string.IsNullOrEmpty(colfilter))
                    continue;

                if (string.IsNullOrEmpty(rowfilter))
                    rowfilter = "(" + colfilter + ")";
                else
                {
                    if (column.FilterPredicates[0].PredicateType == PredicateType.Or)
                        rowfilter = rowfilter.OrPredicate() + "(" + colfilter + ")";
                    else
                        rowfilter = rowfilter.AndPredicate() + "(" + colfilter + ")";
                }
            }
            return rowfilter;
        }

        public static DataTable GetClonedSource(this DataTableCollectionView view)
        {
            var dv = view.SourceCollection as DataView;
            var filterString = dv.RowFilter;
            var canSuspend = !string.IsNullOrEmpty(filterString);
            if (canSuspend)
            {
                view.IsInSuspend = true;
                dv.RowFilter = string.Empty;
            }
            DataTable dt = dv.Table.Clone();
            foreach (DataRowView dvr in dv)
            {
                dt.ImportRow(dvr.Row);
            }
            dt.AcceptChanges();
            if (canSuspend)
            {
                dv.RowFilter = filterString;
                view.IsInSuspend = false;
            }
            return dt;
        }

        public static string EscapeSpecialChars(string value)
        {
            var pattern = @"([-_&\]\\*\\%\\#\\[\\\\])";
            if (System.Text.RegularExpressions.Regex.IsMatch(value, pattern))
            {
                if ((value.Contains("]") | value.Contains(@"\")) && !value.Contains(@"\]"))
                {
                    value = value.Replace(@"\", @"\\");
                    value = value.Replace("]", @"\]");
                }
                value = "[" + value + "]";
            }
            return value;
        }
    }
}
