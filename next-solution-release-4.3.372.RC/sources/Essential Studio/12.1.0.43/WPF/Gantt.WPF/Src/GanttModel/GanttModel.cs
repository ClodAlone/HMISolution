#region Copyright Syncfusion Inc. 2001 - 2014
// Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Any infringement will be prosecuted under
// applicable laws. 
#endregion
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Linq;
using System.Collections;
using System.Windows.Data;
using System.Collections.ObjectModel;
using System.Windows;
using System.Reflection;
using Syncfusion.Windows.Controls.Grid;
using System.Windows.Input;
using Syncfusion.Windows.Controls.Gantt.Chart;

namespace Syncfusion.Windows.Controls.Gantt
{
    /// <summary>
    /// Class that will handles the internal operations of Gantt.
    /// </summary>
    public class GanttModel
    {
        #region Private properties

        private GanttControl _ganttControl;
        private GanttRecordCollection _expandCollection;
        private ObservableCollection<TaskDetails> _inbuiltTaskCollection;
        private TaskAttributeMapping _taskAttributeMapping;
        private ObservableCollection<Resource> _resouces;
        private Dictionary<object, GanttRecord> ChangeListner;
        private Dictionary<object, List<GanttRecord>> CollapsedNodes;
        private Dictionary<object, GanttRecord> InLineChangeListner; 
        private Dictionary<object, GanttRecord> InLinePropChangeListner;
        private IList _highlightedItems;

        #endregion

        #region Constructor

        /// <summary>
       /// Initializes a new instance of the <see cref="GanttModel"/> class.
       /// </summary>
       /// <param name="gantt">The gantt.</param>
       public GanttModel(GanttControl gantt)
            : this()
        {
            _ganttControl = gantt;
            WireGanttEvents();
        }

       /// <summary>
       /// Initializes a new instance of the <see cref="GanttModel"/> class.
       /// </summary>
        public GanttModel()
        {

        }

        /// <summary>
        /// Wires the gantt events.
        /// </summary>
        private void WireGanttEvents()
        {
            if (this.GanttControl != null)
            {
                this.GanttControl.SelectedItems.CollectionChanged -= SelectedItems_CollectionChanged;
                this.GanttControl.SelectedItems.CollectionChanged += SelectedItems_CollectionChanged;
            }
        }

       #endregion

        #region Internal properties

        internal bool IsAllNodesExpanded = false;

        internal bool IsInSelection = false;

        internal bool IsInExpSync = false;

        internal IEnumerable SourceList;

        internal Dictionary<string, PropertyInfo> ItemProperties;

        internal Dictionary<string, PropertyInfo> ChildItemProperties;

        /// <summary>
        /// Determines the Tasks are started or not
        /// </summary>
        internal bool IsWorkStarted = false;

        /// <summary>
        /// Gets the gantt control.
        /// </summary>
        /// <value>The gantt control.</value>
        internal GanttControl GanttControl
        {
            get
            {
                return _ganttControl ?? (_ganttControl = new GanttControl());
            }
        }

        /// <summary>
        /// Gets or sets the expanded collection.
        /// </summary>
        /// <value>The expanded collection.</value>
        internal GanttRecordCollection ExpandedCollection
        {
            get
            {
                if (_expandCollection == null)
                {
                    _expandCollection = new GanttRecordCollection();
                }
                return _expandCollection;
            }
            set
            {
                _expandCollection = value;
            }
        }

        /// <summary>
        /// Gets or sets the appointment mapping.
        /// </summary>
        /// <value>The appointment mapping.</value>
        internal TaskAttributeMapping TaskAttributeMapping
        {
            get
            {
                return _taskAttributeMapping;
            }
            set
            {
                if (_taskAttributeMapping != value)
                {
                    _taskAttributeMapping = value;
                }
            }
        }

        /// <summary>
        /// Gets or sets the resources.
        /// </summary>
        /// <value>The resources.</value>
        internal ObservableCollection<Resource> Resources
        {
            get
            {
                if (_resouces == null)
                    _resouces = new ObservableCollection<Resource>();

                return _resouces;
            }
            set
            {
                _resouces = value;
            }
        }

        /// <summary>
        /// Gets or sets the highlighted items.
        /// </summary>
        /// <value>The highlighted items.</value>
        internal IList HighlightedItems
        {
            get
            {
                return _highlightedItems;
            }
            set
            {
                if (_highlightedItems != null && _highlightedItems is INotifyCollectionChanged)
                {
                    // Unhooking the collection changed from old collection.
                    (_highlightedItems as INotifyCollectionChanged).CollectionChanged -= OnHighlightedItemsChanged;
                }

                _highlightedItems = value;

                if (_highlightedItems != null && _highlightedItems is INotifyCollectionChanged)
                {
                    // Hooking the collection changed in new collection.
                    (_highlightedItems as INotifyCollectionChanged).CollectionChanged += OnHighlightedItemsChanged;
                }
            }
        }

        /// <summary>
        /// Gets a value indicating whether this instance is auto update enabled.
        /// </summary>
        /// <value>
        /// 	<c>true</c> if this instance is auto update enabled; otherwise, <c>false</c>.
        /// </value>
        internal bool IsAutoUpdateEnabled
        {
            get
            {
                return this.GanttControl.UseAutoUpdateHierarchy;
            }
        }

        #endregion

        #region Public properties

        /// <summary>
        /// Gets or sets the inbuilt task collection.
        /// </summary>
        /// <value>The inbuilt task collection.</value>
        public ObservableCollection<TaskDetails> InbuiltTaskCollection
        {
            get
            {
                return _inbuiltTaskCollection ?? (_inbuiltTaskCollection = new ObservableCollection<TaskDetails>());
            }
            internal set
            {
                _inbuiltTaskCollection = value;
            }
        }

        #endregion

        #region Calculated Start & End date

        /// <summary>
        /// Gets the start date.
        /// </summary>
        /// <returns></returns>
        internal DateTime GetStartDate()
        {
            var startdate = this.GanttControl.StartTime;

            // Checking for the existance of mapping name
            if (this.TaskAttributeMapping == null || string.IsNullOrEmpty(this.TaskAttributeMapping.StartDateMapping))
                return startdate;

            if (this.SourceList != null)
            {
                var collection = GetObservableCollection(this.SourceList);
                if (collection != null && collection.Count >= 1)
                {
                    try
                    {
                        var query = from taskdetail in collection select (DateTime)this.ItemProperties[this.TaskAttributeMapping.StartDateMapping].GetValue(taskdetail);

                        // This is to ensure the start date is not set on min or max value of the date time data type
                        var filteredQuery = query.Where(d=> !d.Equals(DateTime.MinValue) && !d.Equals(DateTime.MaxValue));

                        // This is to ensure the existance of date
                        if (filteredQuery != null && filteredQuery.Count() > 0)
                            startdate = filteredQuery.Min<DateTime>();
                    }
                    catch (Exception)
                    {
                        throw new Exception("Start date mapping name is missing or mapping name is wrong.");
                    }

                    //This is to ensure the start date when some tasks dont have start date.
                    //if (DateTime.MinValue.Equals(startdate) || DateTime.MaxValue.Equals(startdate))
                    //    startdate = DateTime.Today;

                    return this.GetExtendedDate(startdate, true);
                }
            }

            return this.GetDefaultDate(startdate, true);
        }

        /// <summary>
        /// Gets the end date.
        /// </summary>
        /// <returns></returns>
        internal DateTime GetEndDate()
        {
            var endDate = this.GanttControl.EndTime;

            // Checking for the existance of the mapping name
            if (this.TaskAttributeMapping == null || string.IsNullOrEmpty(this.TaskAttributeMapping.FinishDateMapping))
                return endDate;

            if (this.SourceList != null)
            {
                var collection = GetObservableCollection(this.SourceList);
                if (collection != null && collection.Count >= 1)
                {
                    try
                    {
                        var query = from taskdetail in collection select (DateTime)this.ItemProperties[this.TaskAttributeMapping.FinishDateMapping].GetValue(taskdetail);

                        // This is to ensure the start date is not set on min or max value of the date time data type
                        var filteredQuery = query.Where(d => !d.Equals(DateTime.MinValue) && !d.Equals(DateTime.MaxValue));

                        // This is to ensure the existance of date
                        if (filteredQuery != null && filteredQuery.Count() > 0)
                            endDate = filteredQuery.Max<DateTime>();

                    }
                    catch (Exception)
                    {
                        throw new Exception("Finish date mapping name is missing or mapping name is wrong.");
                    }

                    //This is to ensure the end date when some tasks dont have the end date.
                    //if (DateTime.MinValue.Equals(endDate) || DateTime.MaxValue.Equals(endDate))
                    //    endDate = DateTime.Today;
                    if (this.GanttControl.StartTime > endDate)
                        this.GanttControl.StartTime = endDate;

                     return this.GetExtendedDate(endDate, false);
                }
            }

            return this.GetDefaultDate(endDate, false);
        }

        /// <summary>
        /// Gets the extended date.
        /// </summary>
        /// <param name="srcDate">The SRC date.</param>
        /// <param name="isStartDate">if set to <c>true</c> [is start date].</param>
        /// <returns></returns>
        private DateTime GetExtendedDate(DateTime srcDate, bool isStartDate)
        {
            if (this.GanttControl.GanttSchedule == null || this.GanttControl.GanttSchedule.ItemsSource == null)
                return srcDate;

            List<GanttScheduleRowInfo> srcList = new List<GanttScheduleRowInfo>(this.GanttControl.GanttSchedule.ItemsSource.Cast<GanttScheduleRowInfo>());
            if (srcList == null || srcList.Count <= 0)
                return srcDate;

            TimeUnit lowerUnit = srcList[srcList.Count - 1].TimeUnit;
            double cellsPerUnit = srcList[srcList.Count - 1].CellsPerUnit;

            // validating lower cell unit
            if (cellsPerUnit <= 0 )
                return srcDate;

            int operatorSwitcher = isStartDate ? -1 : 1;

            // Since schedule is working based on 24hours the end date will show the 12:00 AM so the las cell will not get drawn
            // to over come this we have added slack count, this will add an additional day to the end date.
            int slackCount = isStartDate ? 0 : 1;
            DateTime tempDt = srcDate;

            // Calcuating the extended date based on the cells perunit
            switch (lowerUnit)
            {
#if !SILVERLIGHT
                case TimeUnit.Minutes:
                    tempDt = srcDate.AddMinutes(((5 + slackCount) * cellsPerUnit) * operatorSwitcher);
                    return new DateTime(tempDt.Year, tempDt.Month, tempDt.Day, tempDt.Hour, tempDt.Minute, 0);
                case TimeUnit.Hours:
                    tempDt = srcDate.AddHours(((5 + slackCount) * cellsPerUnit) * operatorSwitcher);
                    return new DateTime(tempDt.Year, tempDt.Month, tempDt.Day, tempDt.Hour, 0, 0);
#endif
                case TimeUnit.Days:
                    return srcDate.Date.AddDays(((14 + slackCount) * cellsPerUnit) * operatorSwitcher);
                case TimeUnit.Weeks:
                    // Get the remaining days in the week and add it first and then add the weeks based on cells perunit
                    double extraDays = isStartDate ? -(int)srcDate.Date.DayOfWeek : 7 - (int)srcDate.Date.DayOfWeek;
                    srcDate = srcDate.Date.AddDays(extraDays);
                    // Adding days for five cells to include five week cells in the schedule.
                    return srcDate.Date.AddDays((7 * ((5 + slackCount) * cellsPerUnit) * operatorSwitcher));
                case TimeUnit.Months:
                    tempDt = srcDate.Date.AddMonths((int)(((5 + slackCount) * cellsPerUnit) * operatorSwitcher));
                    return new DateTime(tempDt.Year, tempDt.Month, 1);
                case TimeUnit.Years:
                    return srcDate.Date.AddYears((int)(((5 + slackCount) * cellsPerUnit) * operatorSwitcher));
            }

            return srcDate;
        }

        /// <summary>
        /// Gets the default date.
        /// </summary>
        /// <param name="srcDate">The SRC date.</param>
        /// <param name="isStartDate">if set to <c>true</c> [is start date].</param>
        /// <returns></returns>
        private DateTime GetDefaultDate(DateTime srcDate, bool isStartDate)
        {
            if (this.SourceList == null)
                return srcDate;

            DateTime defaultDate = DateTime.Today;
            DateTime tempDate = DateTime.Now;

             int operatorSwitcher = isStartDate ? -1 : 1;

            // Calculating the default start and end date based on schedule date.
             switch (this.GanttControl.ScheduleType)
             {
                 case ScheduleType.YearWithMonths:
                     return defaultDate.Date.AddMonths((7 * operatorSwitcher));
                 case ScheduleType.YearWithDays:
                 case ScheduleType.MonthWithDays:
                 case ScheduleType.WeekWithDays:
                     return srcDate.Date;
#if !SILVERLIGHT
                 case ScheduleType.MonthWithHours:
                     defaultDate = tempDate.AddHours((14 * operatorSwitcher));
                     return new DateTime(defaultDate.Year, defaultDate.Month, defaultDate.Day, defaultDate.Hour, defaultDate.Minute, defaultDate.Second);
                 case ScheduleType.DayWithHours:
                     defaultDate = tempDate.AddHours((14 * operatorSwitcher));
                     return new DateTime(defaultDate.Year, defaultDate.Month, defaultDate.Day, defaultDate.Hour, defaultDate.Minute, defaultDate.Second);
                 case ScheduleType.DayWithMinutes:
                     defaultDate = tempDate.AddMinutes((14 * operatorSwitcher));
                     return new DateTime(defaultDate.Year, defaultDate.Month, defaultDate.Day, defaultDate.Hour, defaultDate.Minute, defaultDate.Second);
#endif
             }

            return defaultDate;
        }

        /// <summary>
        /// Sets the Start Date While DragDrop
        /// </summary>
        /// <param name="srcDate">The date.</param>
        /// <param name="isStartDate">IsStartDate.</param>
        internal void SetExtendedDate(DateTime srcDate, bool isStartDate)
        {
            if (this.GanttControl.GanttSchedule == null || this.GanttControl.GanttSchedule.ItemsSource == null)
                return;

            List<GanttScheduleRowInfo> srcList = new List<GanttScheduleRowInfo>(this.GanttControl.GanttSchedule.ItemsSource.Cast<GanttScheduleRowInfo>());
            if (srcList == null || srcList.Count <= 0)
                return;

            TimeUnit lowerUnit = srcList[srcList.Count - 1].TimeUnit;
            double cellsPerUnit = srcList[srcList.Count - 1].CellsPerUnit;

            // Since schedule is working based on 24hours the end date will show the 12:00 AM so the las cell will not get drawn
            // to over come this we have added an additional day to the end date.
            double cellCount = isStartDate ? 3 : 4;

            int operatorSwitcher = isStartDate ? -1 : 1;
            DateTime tempDt = srcDate;

            // Calculating the extend date based on cells per unit.
            switch (lowerUnit)
            {
#if !SILVERLIGHT
                case TimeUnit.Minutes:
                    tempDt = srcDate.AddMinutes((cellCount * cellsPerUnit) * operatorSwitcher);
                    tempDt = new DateTime(tempDt.Year, tempDt.Month, tempDt.Day, tempDt.Hour, tempDt.Minute, 0);
                    break;
                case TimeUnit.Hours:
                    tempDt = srcDate.AddHours((cellCount * cellsPerUnit) * operatorSwitcher);
                    tempDt = new DateTime(tempDt.Year, tempDt.Month, tempDt.Day, tempDt.Hour, 0, 0);
                    break;
#endif
                case TimeUnit.Days:
                    tempDt = srcDate.Date.AddDays((cellCount * cellsPerUnit) * operatorSwitcher);
                    break;
                case TimeUnit.Weeks:
                    // Get the remaining days in the week and add it first and then add the weeks based on cells perunit
                    double extraDays = isStartDate ? -(int)srcDate.Date.DayOfWeek : 7 - (int)srcDate.Date.DayOfWeek;
                    srcDate = srcDate.Date.AddDays(extraDays);
                    // Adding days for five cells to include five week cells in the schedule.
                    tempDt= srcDate.Date.AddDays((7 * (cellsPerUnit * cellCount) * operatorSwitcher));
                    break;
                case TimeUnit.Months:
                    tempDt = srcDate.Date.AddMonths((int)((cellCount * cellsPerUnit) * operatorSwitcher));
                    tempDt = new DateTime(tempDt.Year, tempDt.Month, 1);
                    break;
                case TimeUnit.Years:
                    tempDt = srcDate.Date.AddYears((int)((cellCount * cellsPerUnit) * operatorSwitcher));
                    break;
            }

            if (isStartDate)
                this.GanttControl.StartTime = tempDt;
            else
                this.GanttControl.EndTime = tempDt;
        }

        /// <summary>
        /// Gets the start point.
        /// </summary>
        /// <returns></returns>
        internal double GetStartPoint()
        {
            double startPoint = this.GanttControl.StartPoint;

            // Checking for the existance of mapping name
            if (this.TaskAttributeMapping == null || string.IsNullOrEmpty(this.TaskAttributeMapping.StartPointMapping))
                return startPoint;

            if (this.SourceList != null)
            {
                var collection = GetObservableCollection(this.SourceList);
                if (collection != null && collection.Count >= 1)
                {
                    try
                    {
                        var query = from taskdetail in collection select (double)this.ItemProperties[this.TaskAttributeMapping.StartPointMapping].GetValue(taskdetail);
                        startPoint = query.Min<double>();
                    }
                    catch (Exception)
                    {
                        throw new Exception("Start point mapping name is missing or mapping name is wrong.");
                    }
                    return startPoint - 5;
                }
            }
            return startPoint;
        }

        /// <summary>
        /// Gets the end point.
        /// </summary>
        /// <returns></returns>
        internal double GetEndPoint()
        {
            double endPoint = this.GanttControl.EndPoint;

            // Checking for the existance of the mapping name
            if (this.TaskAttributeMapping == null || string.IsNullOrEmpty(this.TaskAttributeMapping.FinishPointMapping))
                return endPoint;

            if (this.SourceList != null)
            {
                var collection = GetObservableCollection(this.SourceList);
                if (collection != null && collection.Count >= 1)
                {
                    try
                    {
                        var query = from taskdetail in collection select (double)this.ItemProperties[this.TaskAttributeMapping.FinishPointMapping].GetValue(taskdetail);
                        endPoint = query.Max<double>();
                    }
                    catch (Exception)
                    {
                        throw new Exception("Finish point mapping name is missing or mapping name is wrong.");
                    }
                    return endPoint + 5;
                }
            }
            return endPoint;
        }

        /// <summary>
        /// Gets the observable collection.
        /// </summary>
        /// <param name="original">The original.</param>
        /// <returns></returns>
        internal ObservableCollection<object> GetObservableCollection(IEnumerable original)
        {
            return new ObservableCollection<object>(original.Cast<object>());
        }

        #endregion

        #region Expanded collection creation

        /// <summary>
        /// Sets the source list.
        /// </summary>
        /// <param name="value">The value.</param>
        internal void SetSourceList(object value)
        {
            this.SourceList = CreateSourceList(value);

            if (SourceList != null)
            {
                this.ClearProperties();

                var prop = SourceList.GetType().GetProperty("Item");
                prop.PropertyType.GetProperties().ToList().ForEach((p) =>
                    {
                        ItemProperties.Add(p.Name, p);
                        ChildItemProperties.Add(p.Name, p);
                    });

                WireCollectionChanged(SourceList, null);
                this.CreateExpandedCollection(SourceList);
            }
        }

        /// <summary>
        /// Gets the source list.
        /// </summary>
        /// <param name="source">The source.</param>
        /// <returns></returns>
        internal IEnumerable CreateSourceList(object source)
        {
            IEnumerable result = null;
            if (source != null)
            {
                if (source is CollectionViewSource)
                {
                    var cvs = source as CollectionViewSource;
                    if (cvs.View != null)
                    {
                        result = CreateSourceList(cvs.View.SourceCollection);
                    }
                }
                else if (source is ICollectionView)
                {
                    var sourceList = ((ICollectionView)source).SourceCollection;
                    result = CreateSourceList(sourceList);
                }
                else
                {
                    result = source as IEnumerable;
                }
            }
            return result;
        }

        /// <summary>
        /// Clears the properties.
        /// </summary>
        private void ClearProperties()
        {
            ItemProperties = new Dictionary<string, PropertyInfo>();
            ChildItemProperties = new Dictionary<string, PropertyInfo>();
            this.ExpandedCollection = new GanttRecordCollection();
            this.ChangeListner = new Dictionary<object, GanttRecord>();
            this.CollapsedNodes = new Dictionary<object, List<GanttRecord>>();
            this.InLineChangeListner = new Dictionary<object, GanttRecord>();
            this.InLinePropChangeListner = new Dictionary<object, GanttRecord>();
            this.IsAllNodesExpanded = false;
            this.IsInExpSync = false;
            this.IsInSelection = false;
            this.IsWorkStarted = false;
        }

        /// <summary>
        /// Checks the type of the source.
        /// </summary>
        /// <returns></returns>
        internal bool CheckSourceType()
        {
            bool InbuiltSource = false;

            var prop = this.SourceList.GetType().GetProperty("Item");

            if (prop.PropertyType.Equals(typeof(TaskDetails)))
                return true;

            return InbuiltSource;
        }

        /// <summary>
        /// Creates the expanded collection.
        /// </summary>
        /// <param name="sourceList">The source list.</param>
        /// <returns></returns>
        private int CreateExpandedCollection(IEnumerable sourceList)
        {
            int count = 0;

            foreach (object obj in (IEnumerable)sourceList)
            {
                GanttRecord record = new GanttRecord { DataItem = obj, IsExpanded = true };
                this.CheckForInLineItems(obj, record);
                if (!this.ExpandedCollection.Contains(record))
                    this.ExpandedCollection.Add(record);

                if (!record.IsRealized)
                {
                    this.SyncExpandState(record, record.IsExpanded);
                    record.IsRealized = true;
                }

                // validating auto update mode
                if (IsAutoUpdateEnabled)
                {
                    this.WirePropertyChanged(obj);
                }
                count++;
            }
            return count;
        }

        #endregion

        #region Source collection change

        /// <summary>
        /// Wires the collection changed.
        /// </summary>
        /// <param name="SourceList">The source list.</param>
        /// <param name="parent">The parent.</param>
        private void WireCollectionChanged(IEnumerable SourceList, GanttRecord parent)
        {
            if (SourceList is INotifyCollectionChanged)
            {
                var sourceList = SourceList as INotifyCollectionChanged;

                if (!this.ChangeListner.Keys.Contains(sourceList))
                {
                    sourceList.CollectionChanged += Source_CollectionChanged;
                    this.ChangeListner.Add(sourceList, parent);
                }
            }
        }

        /// <summary>
        /// Uns the wire collection changed.
        /// </summary>
        /// <param name="SourceList">The source list.</param>
        /// <param name="parent">The parent.</param>
        private void UnWireCollectionChanged(IEnumerable SourceList, object parent)
        {
            if (SourceList is INotifyCollectionChanged)
            {
                var sourceList = SourceList as INotifyCollectionChanged;

                sourceList.CollectionChanged -= Source_CollectionChanged;
                this.ChangeListner.Remove(sourceList);
            }
        }

        /// <summary>
        /// Handles the CollectionChanged event of the Source control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.Collections.Specialized.NotifyCollectionChangedEventArgs"/> instance containing the event data.</param>
        void Source_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            if (this.SourceList == null)
                return;

            GanttRecord parent = null;
            int startIndex = -1;

            this.ChangeListner.TryGetValue(sender, out parent);
            int changeIndex = e.Action == NotifyCollectionChangedAction.Add ? e.NewStartingIndex : e.OldStartingIndex;

            if (parent != null)
            {
                if (e.Action == NotifyCollectionChangedAction.Add)
                {
                    // +1 is added to insert the new object next to the existing object
                    startIndex = this.ExpandedCollection.IndexOf(parent) + changeIndex + 1;

                    //// This is to check whether the parent is in expand state or not, when the parent is in collapsed state
                    //// just removing that fromt the collapsed node is enough, it will get recreated on expanding.
                    if (parent.IsExpanded)
                    {
                        //// This to check whether new record is the first child or not, if it is first child
                        //// No need to iterate throught the hierarchy to get the exact index
                        if (changeIndex > 0)
                        {
                            // Getting the immediate sibling to get its index in the expand collection
                            int tempIndex = changeIndex - 1;
                            var tempsource = this.GetObservableCollection(sender as IEnumerable);

                            object obj = tempsource != null ? tempsource[tempIndex] : null;
                            GanttRecord record = this.ExpandedCollection.RecordFromItem(obj);

                            // Getting the immediate sibiling gantt record to check wheter it is expand state
                            // if it is not in expand state just adding one to sibiling index will insert the new item
                            // in exact location on expand collecton, if the sibiling is in expand state need to
                            // ride through the child of the sibiling to get the exact index for the new item
                            if (record != null)
                            {
                                if (record.IsExpanded)
                                {
                                    // Getting the exact index of the new item by iterating through the child of sibiling
                                    startIndex = this.ExpandedCollection.IndexOf(record) + this.GetHierarchyCount(record.DataItem) + 1;
                                }
                                else
                                {
                                    startIndex = this.ExpandedCollection.IndexOf(record) + 1;
                                }
                            }
                        }
                        // To add the new items ot the Expanded collection.
                        AddItems(e, parent, startIndex);
                        // This is to refresh the parrent row on iserting the child row

                        this.InvalidateGrid(parent.DataItem);
                        // To change the node type based on collection changed
                        this.GanttControl.GanttChart.UpdateNodeType(parent);
                    }
                    else
                    {
                        // Removing the parent from collapsed nodes collection to recreate it on expanding.
                        this.CollapsedNodes.Remove(parent.DataItem);
                    }
                }
                else if (e.Action == NotifyCollectionChangedAction.Remove)
                {
                    foreach (object o in e.OldItems)
                    {
                        if (parent.IsExpanded)
                        {
                            startIndex = this.ExpandedCollection.IndexOf(o);

                            if (startIndex == -1)
                            {
                                // This condition will work when the root node is collappsed with out collapsing all the child, 
                                // and we try to remove a child from any one of the inner parrent
                                if (this.CollapsedNodes.Count > 0)
                                {
                                    // Since we cant get the inner parrent in the collapsed nodes dictionary directly,
                                    // all the values fo collapsed nodes are retirved and compared to remove it
                                    for (int i = 0; i < CollapsedNodes.Count; i++)
                                    {
                                        for (int j = 0; j < CollapsedNodes[i].Count; j++)
                                        {
                                            // Comparing the collapsed item and removing it from the collapsed nodes
                                            if (o == CollapsedNodes[i][j].DataItem)
                                                CollapsedNodes[i].RemoveAt(j);
                                        }
                                    }
                                }
                            }
                            else
                            {
                                // To remove the  items from Expanded collection.
                                RemoveItems(e, parent, startIndex);
                            }
                            if (parent != null)
                            {
                                // To change the node type based on collection changed
                                this.GanttControl.GanttChart.UpdateNodeType(parent);
                            }
                            break;
                        }
                        else
                        {
                            // When a record is get deleted from collapsed parent, that item should be get deleted from 
                            // the corresponding collased record collection 
                            List<GanttRecord> records = new List<GanttRecord>();
                            if (this.CollapsedNodes.TryGetValue(parent.DataItem, out records))
                            {
                                for (int j = 0; j < records.Count; j++)
                                {
                                    // Removing the item from the collapsed nodes collection.
                                    if (o == records[j].DataItem)
                                        CollapsedNodes[parent.DataItem].RemoveAt(j);
                                }
                            }
                        }
                    }
                }
                else if (e.Action == NotifyCollectionChangedAction.Reset)
                {
                    // Reset will occur when the child collectoin was get cleared completely.
                    var sourceCollection = this.GetObservableCollection(this.SourceList);

                    // Checking whether the parent is in expand state, if expanded need to get the exact index to remove the 
                    // items from the expanded collection.
                    if (parent.IsExpanded)
                    {
                        int index = sourceCollection.IndexOf(parent.DataItem);
                        if (sourceCollection.Count > index + 1)
                        {
                            GanttRecord rec = this.ExpandedCollection.RecordFromItem(sourceCollection[index + 1]);
                            int start = this.ExpandedCollection.IndexOf(parent);
                            int end = this.ExpandedCollection.IndexOf(rec);

                            for (int i = start + 1; i < end; i++)
                                this.ExpandedCollection.RemoveAt(start + 1);
                        }
                    }
                    else
                    {
                        // When the parent is in collapsed state, removing the item from collapsed nodes will remove the record.
                        this.CollapsedNodes.Remove(parent);
                    }
                }
            }
            else
            {
                startIndex = changeIndex;

                var sourceCollection = this.GetObservableCollection(this.SourceList);
                if (e.Action == NotifyCollectionChangedAction.Add)
                {
                    // Get the immediate sibiling and based on its index the new item index will be calcualted.
                    if (changeIndex > 0 && (changeIndex + 1) < sourceCollection.Count)
                    {
                        object currentObj = sourceCollection[changeIndex + 1];
                        GanttRecord record = this.ExpandedCollection.RecordFromItem(currentObj);
                        startIndex = this.ExpandedCollection.IndexOf(record);
                    }
                    else if ((changeIndex + 1) == sourceCollection.Count)
                    {
                        startIndex = this.ExpandedCollection.Count;
                    }

                    // To add the new items to expander collection.
                    AddItems(e, parent, startIndex);
                }
                else if (e.Action == NotifyCollectionChangedAction.Remove)
                {
                    if (changeIndex > 0 && changeIndex <= sourceCollection.Count)
                    {
                        object currentObj = e.OldItems[0];
                        GanttRecord record = this.ExpandedCollection.RecordFromItem(currentObj);
                        startIndex = this.ExpandedCollection.IndexOf(record);
                    }

                    // To remove the old items from the expanded collection
                    RemoveItems(e, parent, startIndex);
                }
                else if (e.Action == NotifyCollectionChangedAction.Reset)
                {
                    this.ExpandedCollection.Clear();
                    this.ChangeListner.Clear();
                    this.InLineChangeListner.Clear();
                    this.InLinePropChangeListner.Clear();
                    this.CollapsedNodes.Clear();
                    this.GanttControl.GanttChart.CollectionChanged(null, e.Action);
                }
            }
        }

        /// <summary>
        /// Adds the items.
        /// </summary>
        /// <param name="e">The <see cref="System.Collections.Specialized.NotifyCollectionChangedEventArgs"/> instance containing the event data.</param>
        /// <param name="parent">The parent.</param>
        /// <param name="startIndex">The start index.</param>
        private void AddItems(NotifyCollectionChangedEventArgs e, GanttRecord parent, int startIndex)
        {
            if (startIndex < 0 || e.NewItems == null)
                return;

            // To add the new items to the expanded collection
            this.AddToExpandedCollection(e.NewItems, parent, startIndex);

            // To refresh the connectors
            this.GanttControl.GanttChart.CollectionChanged(e.NewItems, e.Action);
            return;
        }

        /// <summary>
        /// Removes the items.
        /// </summary>
        /// <param name="e">The <see cref="System.Collections.Specialized.NotifyCollectionChangedEventArgs"/> instance containing the event data.</param>
        /// <param name="parent">The parent.</param>
        /// <param name="startIndex">The start index.</param>
        private void RemoveItems(NotifyCollectionChangedEventArgs e, GanttRecord parent, int startIndex)
        {
            if (startIndex < 0 || e.OldItems == null)
                return;

            // To remove the items from expanded collection
            this.RemoveFromExpandedCollection(e.OldItems, parent, startIndex, true);

            // To refresh the connectors
            this.GanttControl.GanttChart.CollectionChanged(e.OldItems, e.Action);
        }

        /// <summary>
        /// Gets the hierarchy count.
        /// </summary>
        /// <param name="item">The item.</param>
        /// <returns></returns>
        int GetHierarchyCount(object item)
        {
            int count = 0;
            if (!this.TaskAttributeMapping.HasChildMapping || item == null)
                return count;
            object child = null;
            try
            {
                child = this.ItemProperties[this.TaskAttributeMapping.ChildMapping].GetValue(item) as IEnumerable;
            }
            catch (Exception)
            {
                throw new Exception("Child task mapping name is missing or mapping name is wrong.");
            }

            if (child == null || (child as IEnumerable) == null)
                return count;

            // Iterating throught the child to get the exact count of this whole hierarchy.
            foreach (object obj in (IEnumerable)child)
            {
                count += 1;

                GanttRecord record = this.ExpandedCollection.RecordFromItem(obj);
                if (record == null || !record.IsExpanded)
                    continue;

                // Getting the child for the is current item
                count += GetHierarchyCount(obj);
            }

            return count;
        }

        /// <summary>
        /// Releases unmanaged and - optionally - managed resources
        /// </summary>
        internal void Dispose()
        {
            this.UnWireCollectionChanged(this.SourceList, null);
            this.ChangeListner = null;
            this.SourceList = null;
            this.ExpandedCollection = null;
            this.ItemProperties = null;
            this.InLineChangeListner = null;
            this.InLinePropChangeListner = null;
        }

        #endregion

        #region Source property change

        /// <summary>
        /// Wires the property changed.
        /// </summary>
        /// <param name="sender">The sender.</param>
        private void WirePropertyChanged(object sender)
        {
            if (sender is INotifyPropertyChanged)
            {
                (sender as INotifyPropertyChanged).PropertyChanged += OnPropertyChanged;
            }   
        }

        /// <summary>
        /// Uns the wire property changed.
        /// </summary>
        /// <param name="sender">The sender.</param>
        private void UnWirePropertyChanged(object sender)
        {
            if (sender is INotifyPropertyChanged)
            {
                (sender as INotifyPropertyChanged).PropertyChanged -= OnPropertyChanged;

                // To unwire the collection change of inline items
                if (!string.IsNullOrEmpty(this.TaskAttributeMapping.InLineTaskMapping))
                    UnWireInLineCollectionChanged(sender);
            }
        }

        /// <summary>
        /// Called when [property changed].
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The <see cref="System.ComponentModel.PropertyChangedEventArgs"/> instance containing the event data.</param>
        void OnPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            //string name = this.ItemProperties[this.TaskAttributeMapping.TaskNameMapping].GetValue(sender).ToString();
            //System.Diagnostics.Debug.WriteLine("\" " + name + " \" : " + e.PropertyName);

            // Initial validation
            if (sender == null || this.ItemProperties == null || this.ItemProperties.Count <= 0 || this.TaskAttributeMapping == null ||
                this.TaskAttributeMapping.MappedAttributes == null || !this.TaskAttributeMapping.MappedAttributes.Values.Contains(e.PropertyName))
                return;

            // Getting the record from the data item
            GanttRecord record = this.ExpandedCollection.RecordFromItem(sender);

            if (record == null || record.DataItem == null)
                return;

            // To update the start/finish date
            if (e.PropertyName.Equals(this.TaskAttributeMapping.StartDateMapping) || e.PropertyName.Equals(this.TaskAttributeMapping.FinishDateMapping))
            {
                // Change in start/finish date will affect the duration of the corresponding task, hence the duration is updated and corresponding
                // property(start/finish) of parent node is updated.

                DateTime start = (DateTime)this.ItemProperties[this.TaskAttributeMapping.StartDateMapping].GetValue(sender);
                DateTime finish = (DateTime)this.ItemProperties[this.TaskAttributeMapping.FinishDateMapping].GetValue(sender);

                if (!string.IsNullOrEmpty(this.TaskAttributeMapping.DurationMapping) && start !=DateTime.MinValue && finish !=DateTime.MinValue)
                {
                    PropertyInfoExtensions.SetValue(ItemProperties[this.TaskAttributeMapping.DurationMapping], sender, finish.Subtract(start));
                }

                // Validating the parent
                if (record.ParentRecord != null && record.ParentRecord.DataItem != null)
                {
                    GanttRecord parent = record.ParentRecord;

                    if (e.PropertyName.Equals(this.TaskAttributeMapping.StartDateMapping))
                    {
                        // Updating the parent start with minimum start among the children
                        var childRecords = record.ParentRecord.ChildRecords.Where(child => !this.ItemProperties[this.TaskAttributeMapping.StartDateMapping].GetValue(child.DataItem).Equals(DateTime.MinValue));
                        if (childRecords.Count() > 0)
                        {
                            start = childRecords.Select(child => (DateTime)this.ItemProperties[this.TaskAttributeMapping.StartDateMapping].GetValue(child.DataItem)).Min<DateTime>();
                            if (!start.Equals(DateTime.MinValue))
                                PropertyInfoExtensions.SetValue(ItemProperties[this.TaskAttributeMapping.StartDateMapping], parent.DataItem, start);
                        }
                    }
                    else
                    {
                        // Updating the parent finish with maximum finish among the children
                        finish = record.ParentRecord.ChildRecords.Select(child => (DateTime)this.ItemProperties[this.TaskAttributeMapping.FinishDateMapping].GetValue(child.DataItem)).Max<DateTime>();
                        PropertyInfoExtensions.SetValue(ItemProperties[this.TaskAttributeMapping.FinishDateMapping], parent.DataItem, finish);
                    }
                }
            }
            // To update the duration
            else if (e.PropertyName.Equals(this.TaskAttributeMapping.DurationMapping))
            {
                // Duration of parent/child should be dependes on the start and end.
                DateTime start = (DateTime)this.ItemProperties[this.TaskAttributeMapping.StartDateMapping].GetValue(sender);
                TimeSpan duration = (TimeSpan)this.ItemProperties[this.TaskAttributeMapping.DurationMapping].GetValue(sender);

                PropertyInfoExtensions.SetValue(ItemProperties[this.TaskAttributeMapping.FinishDateMapping], sender, start.AddDays(duration.TotalDays));
            }
            else if (e.PropertyName.Equals(this.TaskAttributeMapping.ProgressMapping) && record.ParentRecord != null)
            {
                // To update the progress in parent node
                UpdateParentProgress(record.ParentRecord, record.ParentRecord.ChildRecords);
            }
            else if (e.PropertyName.Equals(this.TaskAttributeMapping.CostMapping) && record.ParentRecord != null)
            {
                // To update the cost in parent node
                UpdateParentCost(record.ParentRecord, record.ParentRecord.ChildRecords);
            }
            else if (e.PropertyName.Equals(this.TaskAttributeMapping.BaselineStartMapping) && record.ParentRecord != null)
            {
                // To update the Base line start in parend node
                UpdateParentBaseStart(record.ParentRecord, record.ParentRecord.ChildRecords);
            }
            else if (e.PropertyName.Equals(this.TaskAttributeMapping.BaselineFinishMapping) && record.ParentRecord != null)
            {
                // To update the Base line finish in parend node
                UpdateParentBaseFinish(record.ParentRecord, record.ParentRecord.ChildRecords);
            }
            else if (e.PropertyName.Equals(this.TaskAttributeMapping.BaselineCostMapping) && record.ParentRecord != null)
            {
                // To update the Base line cost in parend node
                UpdateParentBaseCost(record.ParentRecord, record.ParentRecord.ChildRecords);
            }
            else if ((e.PropertyName.Equals(this.TaskAttributeMapping.StartPointMapping) || e.PropertyName.Equals(this.TaskAttributeMapping.FinishPointMapping)) && record.ParentRecord != null)
            {
                // To update the parent node for numeric start/finish for custom numeric schedule
                UpdateParentNumericInfo(record.ParentRecord, record.ParentRecord.ChildRecords);
            }
        }

        /// <summary>
        /// Update the parent record values when we remove all the inline items from the parent.
        /// </summary>
        /// <param name="parentRecord">Corresponding parrent records value</param>
        /// <remarks>We are setting min value for the all columns of parent when we rome all the items from parent</remarks>
        private void UpdateInlineParentInfo(GanttRecord parentRecord)
        {
            if (!string.IsNullOrEmpty(this.TaskAttributeMapping.StartPointMapping) && !string.IsNullOrEmpty(this.TaskAttributeMapping.FinishPointMapping))
            {
                // To update the start/finish points
                PropertyInfoExtensions.SetValue(ItemProperties[this.TaskAttributeMapping.StartPointMapping], parentRecord.DataItem, double.MinValue);
                PropertyInfoExtensions.SetValue(ItemProperties[this.TaskAttributeMapping.FinishPointMapping], parentRecord.DataItem, double.MinValue);
                if (!string.IsNullOrEmpty(this.TaskAttributeMapping.DurationMapping))
                {
                    // Updating the duration based on start and finish.
                    PropertyInfoExtensions.SetValue(ItemProperties[this.TaskAttributeMapping.DurationMapping], parentRecord.DataItem, 0d);
                }
            }

            if (!string.IsNullOrEmpty(this.TaskAttributeMapping.StartDateMapping) && !string.IsNullOrEmpty(this.TaskAttributeMapping.FinishDateMapping))
            {
                // To update the start/finish date
                PropertyInfoExtensions.SetValue(ItemProperties[this.TaskAttributeMapping.StartDateMapping], parentRecord.DataItem, DateTime.MinValue);
                PropertyInfoExtensions.SetValue(ItemProperties[this.TaskAttributeMapping.FinishDateMapping], parentRecord.DataItem, DateTime.MinValue);
                if (!string.IsNullOrEmpty(this.TaskAttributeMapping.DurationMapping))
                {
                    // Updating the duration based on start and finish.
                    PropertyInfoExtensions.SetValue(ItemProperties[this.TaskAttributeMapping.DurationMapping], parentRecord.DataItem, new TimeSpan(0));
                }
            }

            if (!string.IsNullOrEmpty(this.TaskAttributeMapping.ProgressMapping))
            {
                // To update the progress

                PropertyInfoExtensions.SetValue(ItemProperties[this.TaskAttributeMapping.ProgressMapping], parentRecord.DataItem, 0);
            }

            if (!string.IsNullOrEmpty(this.TaskAttributeMapping.CostMapping))
            {
                // To update the cost
                PropertyInfoExtensions.SetValue(ItemProperties[this.TaskAttributeMapping.CostMapping], parentRecord.DataItem, 0);
            }

            if (!string.IsNullOrEmpty(this.TaskAttributeMapping.BaselineStartMapping))
            {
                // To update the base line cost
                PropertyInfoExtensions.SetValue(ItemProperties[this.TaskAttributeMapping.BaselineStartMapping], parentRecord.DataItem, DateTime.MinValue);
            }

            if (!string.IsNullOrEmpty(this.TaskAttributeMapping.BaselineFinishMapping))
            {
                // To update the base line finish
                PropertyInfoExtensions.SetValue(ItemProperties[this.TaskAttributeMapping.BaselineFinishMapping], parentRecord.DataItem, DateTime.MinValue);
            }

            if (!string.IsNullOrEmpty(this.TaskAttributeMapping.BaselineCostMapping))
            {
                // To update the base line cost
                PropertyInfoExtensions.SetValue(ItemProperties[this.TaskAttributeMapping.BaselineCostMapping], parentRecord.DataItem, 0);
            }
        }

        /// <summary>
        /// Updates the parent info.
        /// </summary>
        /// <param name="currentRecord">The current record.</param>
        /// <param name="ChildRecords">The ChildRecords.</param>
        private void UpdateParentInfo(GanttRecord currentRecord, List<GanttRecord> ChildRecords)
        {
            if (ChildRecords.Count <= 0)
                return;

            if (ChildItemProperties.Count > 0)
                ChildItemProperties.Clear();
            ChildRecords.First().DataItem.GetType().GetProperties().ToList().ForEach((p)=>
            {
                ChildItemProperties.Add(p.Name, p);
            });

            if (!string.IsNullOrEmpty(this.TaskAttributeMapping.StartPointMapping) && !string.IsNullOrEmpty(this.TaskAttributeMapping.FinishPointMapping))
            {
                // To update the start/finish points
                UpdateParentNumericInfo(currentRecord,ChildRecords);
            }

            if (!string.IsNullOrEmpty(this.TaskAttributeMapping.StartDateMapping) && !string.IsNullOrEmpty(this.TaskAttributeMapping.FinishDateMapping))
            {
                // To update the start/finish date
                UpdateParentBasicValues(currentRecord, ChildRecords);
            }

            if (!string.IsNullOrEmpty(this.TaskAttributeMapping.ProgressMapping))
            {
                // To update the progress
                UpdateParentProgress(currentRecord, ChildRecords);
            }

            if (!string.IsNullOrEmpty(this.TaskAttributeMapping.CostMapping))
            {
                // To update the cost
                UpdateParentCost(currentRecord, ChildRecords);
            }

            if (!string.IsNullOrEmpty(this.TaskAttributeMapping.BaselineStartMapping))
            {
                // To update the base line cost
                UpdateParentBaseStart(currentRecord, ChildRecords);
            }

            if (!string.IsNullOrEmpty(this.TaskAttributeMapping.BaselineFinishMapping))
            {
                // To update the base line finish
                UpdateParentBaseFinish(currentRecord, ChildRecords);
            }

            if (!string.IsNullOrEmpty(this.TaskAttributeMapping.BaselineCostMapping))
            {
                // To update the base line cost
                UpdateParentBaseCost(currentRecord, ChildRecords);
            }

            if (ChildRecords.First().InLineRecords.Count > 0)
            {
                ChildItemProperties.Clear();
                ChildRecords.First().InLineRecords.First().DataItem.GetType().GetProperties().ToList().ForEach((p) =>
                {
                    ChildItemProperties.Add(p.Name, p);
                });
            }
        }

        /// <summary>
        /// Updates the parent basic values.
        /// </summary>
        /// <param name="currentRecord">The current record.</param>
        /// <param name="ChildRecords">The ChildRecords.</param>
        private void UpdateParentBasicValues(GanttRecord currentRecord, List<GanttRecord> ChildRecords)
        {
            if (ChildRecords.Count <= 0)
                return;

            try
            {
                // Updating the start of parten with minimum start among the children
                DateTime start = ChildRecords.Select(child => (DateTime)this.ChildItemProperties[this.TaskAttributeMapping.StartDateMapping].GetValue(child.DataItem)).Min<DateTime>();
                PropertyInfoExtensions.SetValue(ItemProperties[this.TaskAttributeMapping.StartDateMapping], currentRecord.DataItem, start);

                // Updating the parent finish with maximum finish among the children
                DateTime finish = ChildRecords.Select(child => (DateTime)this.ChildItemProperties[this.TaskAttributeMapping.FinishDateMapping].GetValue(child.DataItem)).Max<DateTime>();
                PropertyInfoExtensions.SetValue(ItemProperties[this.TaskAttributeMapping.FinishDateMapping], currentRecord.DataItem, finish);

                if (!string.IsNullOrEmpty(this.TaskAttributeMapping.DurationMapping))
                {
                    // Updating the duration based on start and finish.
                    PropertyInfoExtensions.SetValue(ItemProperties[this.TaskAttributeMapping.DurationMapping], currentRecord.DataItem, finish.Subtract(start));
                }
            }
            catch (Exception)
            {
                throw new Exception("Start/Finish/Duration mapping name is missing or mapping name is wrong.");
            }
        }

        /// <summary>
        /// Updates the parent numeric info.
        /// </summary>
        /// <param name="currentRecord">The current record.</param>
        /// <param name="ChildRecords">The child records.</param>
        private void UpdateParentNumericInfo(GanttRecord currentRecord, List<GanttRecord> ChildRecords)
        {
            try
            {

                double start = -1, finish=-1;
                /// Updating the parent start point with minimim start among the children
                ///Updating the parent finish point with maximum finish among the children
                if (ChildRecords.First().InLineRecords.Count > 0)
                {
                    start = ChildRecords.Select(child => (double)this.ItemProperties[this.TaskAttributeMapping.StartPointMapping].GetValue(child.DataItem)).Min<double>();
                    finish = ChildRecords.Select(child => (double)this.ItemProperties[this.TaskAttributeMapping.FinishPointMapping].GetValue(child.DataItem)).Max<double>();
                }
                else
                {
                    start = ChildRecords.Select(child => (double)this.ChildItemProperties[this.TaskAttributeMapping.StartPointMapping].GetValue(child.DataItem)).Min<double>();
                    finish = ChildRecords.Select(child => (double)this.ChildItemProperties[this.TaskAttributeMapping.FinishPointMapping].GetValue(child.DataItem)).Max<double>();
                }
                PropertyInfoExtensions.SetValue(ItemProperties[this.TaskAttributeMapping.FinishPointMapping], currentRecord.DataItem, start);
                PropertyInfoExtensions.SetValue(ItemProperties[this.TaskAttributeMapping.FinishPointMapping], currentRecord.DataItem, finish);

            }
            catch (Exception)
            {
                throw new Exception("Start/Finish point mapping name is missing or mapping name is wrong.");
            }
            return;
        }

        /// <summary>
        /// Updates the parent progress.
        /// </summary>
        /// <param name="currentRecord">The current record.</param>
        /// <param name="ChildRecords">The ChildRecords.</param>
        private void UpdateParentProgress(GanttRecord currentRecord, List<GanttRecord> ChildRecords)
        {
            try
            {
                double progress;
                // Updating the progress by aggreating the progress values of the children
                if (ChildRecords.First().InLineRecords.Count > 0)
                {
                    progress = ChildRecords.Aggregate(0d, (current, task) =>
                    {
                        return current + (double)this.ItemProperties[this.TaskAttributeMapping.ProgressMapping].GetValue(task.DataItem);
                    });
                }
                else
                {
                    progress = ChildRecords.Aggregate(0d, (current, task) =>
                    {
                        return current + (double)this.ChildItemProperties[this.TaskAttributeMapping.ProgressMapping].GetValue(task.DataItem);
                    });
                }

                progress = progress / ChildRecords.Count;

                // To avoid entering value more than 100
                progress = progress > 100 ? 100 : progress;
                PropertyInfoExtensions.SetValue(ItemProperties[this.TaskAttributeMapping.ProgressMapping], currentRecord.DataItem, progress);
             }
            catch (Exception)
            {
                throw new Exception("Progress mapping name is missing or mapping name is wrong.");
            }
        }

        /// <summary>
        /// Updates the parent cost.
        /// </summary>
        /// <param name="currentRecord">The current record.</param>
        /// <param name="ChildRecords">The ChildRecords.</param>
        private void UpdateParentCost(GanttRecord currentRecord, List<GanttRecord> ChildRecords)
        {
            try
            {
                // Updating the cost by aggreating the cost values of the children
                double cost = ChildRecords.Aggregate(0d, (current, task) =>
                {
                    return current + (double)this.ChildItemProperties[this.TaskAttributeMapping.CostMapping].GetValue(task.DataItem);
                });
                PropertyInfoExtensions.SetValue(ItemProperties[this.TaskAttributeMapping.CostMapping], currentRecord.DataItem, cost);
            }
            catch (Exception)
            {
                throw new Exception("Cost mapping name is missing or mapping name is wrong.");
            }
        }

        /// <summary>
        /// Updates the parent base start.
        /// </summary>
        /// <param name="currentRecord">The current record.</param>
        /// <param name="ChildRecords">The ChildRecords.</param>
        private void UpdateParentBaseStart(GanttRecord currentRecord, List<GanttRecord> ChildRecords)
        {
            try
            {
                // Updating the base line start of parten with minimum base line start among the children
                DateTime baseStart = ChildRecords.Select(child => (DateTime)this.ChildItemProperties[this.TaskAttributeMapping.BaselineStartMapping].GetValue(child.DataItem)).Min<DateTime>();
                PropertyInfoExtensions.SetValue(ItemProperties[this.TaskAttributeMapping.BaselineStartMapping], currentRecord.DataItem, baseStart);

            }
            catch (Exception)
            {
                throw new Exception("BaseLine Start mapping name is missing or mapping name is wrong.");
            }
        }

        /// <summary>
        /// Updates the parent base finish.
        /// </summary>
        /// <param name="currentRecord">The current record.</param>
        /// <param name="ChildRecords">The ChildRecords.</param>
        private void UpdateParentBaseFinish(GanttRecord currentRecord, List<GanttRecord> ChildRecords)
        {
            try
            {
                // Updating the base line finish of parten with maximum base line finish among the children
                DateTime baseFinish = ChildRecords.Select(child => (DateTime)this.ChildItemProperties[this.TaskAttributeMapping.BaselineFinishMapping].GetValue(child.DataItem)).Min<DateTime>();
                PropertyInfoExtensions.SetValue(ItemProperties[this.TaskAttributeMapping.BaselineFinishMapping], currentRecord.DataItem, baseFinish);
            }
            catch (Exception)
            {
                throw new Exception("BaseLine Finish mapping name is missing or mapping name is wrong.");
            }
        }

        /// <summary>
        /// Updates the parent base cost.
        /// </summary>
        /// <param name="currentRecord">The current record.</param>
        /// <param name="ChildRecords">The ChildRecords.</param>
        private void UpdateParentBaseCost(GanttRecord currentRecord, List<GanttRecord> ChildRecords)
        {
            try
            {
                // Updating the base line cost by aggreating the base line cost values of the children
                double baseCost = ChildRecords.Aggregate(0d, (current, task) =>
                {
                    return current + (double)this.ChildItemProperties[this.TaskAttributeMapping.BaselineCostMapping].GetValue(task.DataItem);
                });
                PropertyInfoExtensions.SetValue(ItemProperties[this.TaskAttributeMapping.BaselineCostMapping], currentRecord.DataItem, baseCost);
            }
            catch (Exception)
            {
                throw new Exception("BaseLine Cost mapping name is missing or mapping name is wrong.");
            }
        }

        /// <summary>
        /// Calculates the duration.
        /// </summary>
        /// <param name="obj">The obj.</param>
        private void CalculateDuration(object obj)
        {
            if (obj == null || string.IsNullOrEmpty(this.TaskAttributeMapping.DurationMapping) ||
                string.IsNullOrEmpty(this.TaskAttributeMapping.StartDateMapping) || string.IsNullOrEmpty(this.TaskAttributeMapping.FinishDateMapping))
                return;

            try
            {
                // Fetching the existing values
                DateTime start = (DateTime)ItemProperties[this.TaskAttributeMapping.StartDateMapping].GetValue(obj);
                DateTime finish = (DateTime)ItemProperties[this.TaskAttributeMapping.FinishDateMapping].GetValue(obj);
                TimeSpan duration = (TimeSpan)ItemProperties[this.TaskAttributeMapping.DurationMapping].GetValue(obj);

                if (DateTime.MinValue == start || DateTime.MaxValue == start)
                    return;

                // Checking for existance of duration
                if (duration == null || duration.TotalDays <= 0 || duration == TimeSpan.MaxValue)
                {
                    if (DateTime.MinValue == finish || DateTime.MaxValue == finish || finish.CompareTo(start) < 1)
                    {
                        PropertyInfoExtensions.SetValue(ItemProperties[this.TaskAttributeMapping.DurationMapping], obj, new TimeSpan(0, 0, 0, 0));
                    }
                    else
                    {
                        PropertyInfoExtensions.SetValue(ItemProperties[this.TaskAttributeMapping.DurationMapping], obj, finish.Subtract(start));
                    }
                }
                else
                {
                    PropertyInfoExtensions.SetValue(ItemProperties[this.TaskAttributeMapping.FinishDateMapping], obj, start.AddDays(duration.TotalDays));
                }
            }
            catch (Exception)
            {
                throw new Exception("Start/Finish/Duration mapping name is missing or mapping name is wrong.");
            }
        }

        #endregion

        #region Expand/Collapse State

        /// <summary>
        /// Expands all nodes.
        /// </summary>
        internal void ExpandAllNodes()
        {
            // This method is to create the expanded collection after the chart get loaded
            // so that it wont affect the loading time of the Gantt
            for (int i = 0; i < this.ExpandedCollection.Count; i++)
            {
                // Based on the realized state the expanded collection will get created
                // this avoid recreating the node that already realized
                if (this.ExpandedCollection[i].IsExpanded && !this.ExpandedCollection[i].IsRealized)
                {
                    // creating the expanded collection
                    this.SyncExpandState(this.ExpandedCollection[i], true);

                    // Changing the realized state to avoid recreating child nodes
                    this.ExpandedCollection[i].IsRealized = true;
                }
                // setting the flag
                this.IsAllNodesExpanded = true;
            }

            if (this.IsAllNodesExpanded)
            {
                this.GanttControl.InitializeStartAndEnd();
                this.GanttControl.GanttSchedule.RedrawSchedule();
            }
        }

        /// <summary>
        /// Sets the state of the expand.
        /// </summary>
        /// <param name="rowIndex">Index of the row.</param>
        /// <param name="nodeAction">The node action.</param>
        internal void SetExpandState(int rowIndex, GridTreeNodeActions nodeAction)
        {
            if (rowIndex < 0)
                return;

            // To get the corresponding Chart row to change its ExpandState
            GanttChartRow row = this.GanttControl.GanttChart.ItemContainerGenerator.ContainerFromIndex(rowIndex) as GanttChartRow;

            if (row == null || row.DataContext == null)
                return;

            this.IsInExpSync = true;

            GanttRecord record = row.DataContext as GanttRecord;

            if (nodeAction == GridTreeNodeActions.Expanded)
            {
                record.IsExpanded = true;
                this.SyncExpandState(record, record.IsExpanded);
            }
            else if (nodeAction == GridTreeNodeActions.Collapsed)
            {
                record.IsExpanded = false;
                this.SyncExpandState(record, record.IsExpanded);
            }

            this.IsInExpSync = false;
        }

        /// <summary>
        /// Sets the state of the expand.
        /// </summary>
        /// <param name="item">The item.</param>
        /// <param name="nodeAction">The node action.</param>
        internal void SetExpandState(object item, GridTreeNodeActions nodeAction)
        {
            this.IsInExpSync = true;

            GanttRecord record = this.ExpandedCollection.RecordFromItem(item);

            if (record == null)
                return;
            //GanttRecord record = this.ExpandedCollection[rowIndex];

            if (nodeAction == GridTreeNodeActions.Expanded)
            {
                record.IsExpanded = true;
                this.SyncExpandState(record, record.IsExpanded);
            }
            else if (nodeAction == GridTreeNodeActions.Collapsed)
            {
                record.IsExpanded = false;
                this.SyncExpandState(record, record.IsExpanded);
            }

            this.IsInExpSync = false;

        }

        /// <summary>
        /// Synchronize the state of the expand.
        /// </summary>
        /// <param name="currentRecord">The current record.</param>
        /// <param name="isExpanded">if set to <c>true</c> [is expanded].</param>
        internal void SyncExpandState(GanttRecord currentRecord, bool isExpanded)
        {
            object dataItem = currentRecord.DataItem;

            if (!this.TaskAttributeMapping.HasChildMapping || dataItem == null)
                return;

            object child = null;
            try
            {
                child = this.ItemProperties[this.TaskAttributeMapping.ChildMapping].GetValue(dataItem) as IEnumerable;
            }
            catch (Exception)
            {
                throw new Exception("Child task mapping name is missing or mapping name is wrong.");
            }

            if (child == null)
                return;

            IEnumerable childNodes = CreateSourceList(child);
            if (childNodes != null)
            {
                int start = this.ExpandedCollection.IndexOf(currentRecord);

                if (start < 0)
                    return;

                if (!isExpanded)
                {
                    this.CollapsedNodes.Add(currentRecord.DataItem, new List<GanttRecord>());
                    this.RemoveFromExpandedCollection(childNodes, currentRecord, start + 1, false);
                }
                else
                {
                    this.WireCollectionChanged((IEnumerable)child, currentRecord);
                    this.AddToExpandedCollection(childNodes, currentRecord, start +1);
                }
            }
        }

        /// <summary>
        /// Adds to expanded collection.
        /// </summary>
        /// <param name="childNodes">The child nodes.</param>
        /// <param name="currentRecord">The current record.</param>
        /// <param name="start">The start.</param>
        private void AddToExpandedCollection(IEnumerable childNodes, GanttRecord currentRecord, int start)
        {
            List<GanttRecord> tempRecords;
            if (currentRecord != null && this.CollapsedNodes.TryGetValue(currentRecord.DataItem, out tempRecords))
            {
                if (tempRecords.Count == 0 && childNodes.Cast<object>().Count() > 0)
                {
                    this.UpdateExpandedCollection(childNodes, currentRecord, start);
                }
                else
                {
                    foreach (GanttRecord record in tempRecords)
                    {
                        if (!this.ExpandedCollection.Contains(record))
                            this.ExpandedCollection.Insert(start++, record);
                        else
                            start++;
                    }
                }
                this.CollapsedNodes.Remove(currentRecord.DataItem);
            }
            else
            {
                this.UpdateExpandedCollection(childNodes, currentRecord, start);
            }
        }

        private void UpdateExpandedCollection(IEnumerable childNodes, GanttRecord currentRecord, int start)
        {
            foreach (object node in childNodes)
            {
                if (node is TaskDetails && currentRecord != null)
                {
                    (node as TaskDetails).ParentNode = currentRecord.DataItem as IGanttTask;
                }
                GanttRecord record = new GanttRecord { DataItem = node, ParentRecord = currentRecord, IsExpanded = true };
                
                if (!this.ExpandedCollection.Any(item => item.DataItem.Equals(record.DataItem)))
                    this.ExpandedCollection.Insert(start++, record);
                else
                    start++;

                if (currentRecord != null)
                {
                    currentRecord.ChildRecords.Add(record);
                }

                // To create the Inline records
                this.CheckForInLineItems(node, record);

                // Validating auto update to wire the listner
                if (!IsAutoUpdateEnabled)
                    continue;

                // To listen the proeprty changed
                this.WirePropertyChanged(node);

                if (this.GanttControl.ScheduleType != ScheduleType.CustomNumeric)
                {
                    // To calucate the duration based on end date or end date based on duration.
                    this.CalculateDuration(record.DataItem);
                }
            }
            // To update the summary/header node
            if (IsAutoUpdateEnabled && currentRecord != null && currentRecord.ChildRecords.Count > 0)
            {
                this.UpdateParentInfo(currentRecord, currentRecord.ChildRecords);
            }
        }

        /// <summary>
        /// Removes from expanded collection.
        /// </summary>
        /// <param name="childNodes">The child nodes.</param>
        /// <param name="currentRecord">The current record.</param>
        /// <param name="start">The start.</param>
        /// <param name="isOnSourceChange">if set to <c>true</c> [is on source change].</param>
        /// <returns></returns>
        private int RemoveFromExpandedCollection(IEnumerable childNodes, GanttRecord currentRecord, int start, bool isOnSourceChange)
        {
            int count = 0;
            List<GanttRecord> tempRecord;

            foreach (object node in (IEnumerable)childNodes)
            {
                if (this.ExpandedCollection.Count <= start)
                    continue;

                var childRecord = this.ExpandedCollection.FirstOrDefault(item => item.DataItem == node);

                if (!isOnSourceChange && childRecord!=null)
                {
                    this.CollapsedNodes[currentRecord.DataItem].Add(childRecord);
                }
                else if (currentRecord != null && childRecord!=null)
                {
                    // Updating the hierachy and unwireing the property change from the deleted item.
                    this.UnWirePropertyChanged(node);
                    currentRecord.ChildRecords.Remove(childRecord);
                }

                if (childRecord != null)
                    this.ExpandedCollection.Remove(childRecord);
                count++;

                if ((!isOnSourceChange && this.CollapsedNodes.TryGetValue(node, out tempRecord)) || this.TaskAttributeMapping == null || !this.TaskAttributeMapping.HasChildMapping)
                    continue;

                try
                {
                    var child = this.ItemProperties[this.TaskAttributeMapping.ChildMapping].GetValue(node) as IEnumerable;

                    if (child == null)
                        continue;

                    int ChildCount = RemoveFromExpandedCollection(child, currentRecord, start, isOnSourceChange);
                    count += ChildCount;

                    if (isOnSourceChange)
                        this.UnWireCollectionChanged(child, currentRecord);
                }
                catch (Exception)
                {
                    throw new Exception("Child task mapping name is missing or mapping name is wrong.");
                }
            }

            // To update the summary/header node on deleting a node
            if (IsAutoUpdateEnabled && isOnSourceChange && currentRecord != null)
            {
                // Based on the existance of child parent record will be updated.
                if (currentRecord.ChildRecords.Count > 0)
                    this.UpdateParentInfo(currentRecord, currentRecord.ChildRecords);
                else
                    this.CalculateDuration(currentRecord.DataItem);
            }

            return count;
        }

        #endregion

        #region  Selected Items

        /// <summary>
        /// Handles the CollectionChanged event of the SelectedItems control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.Collections.Specialized.NotifyCollectionChangedEventArgs"/> instance containing the event data.</param>
        void SelectedItems_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            if (IsInSelection)
                return;

            if (e.Action == NotifyCollectionChangedAction.Add)
            {
                this.GanttControl.GanttGrid.SyncSelectedItems(e.NewItems, e.Action);
            }
            else if (e.Action == NotifyCollectionChangedAction.Remove)
            {
                this.GanttControl.GanttGrid.SyncSelectedItems(e.OldItems, e.Action);
            }
            else if (e.Action == NotifyCollectionChangedAction.Reset)
            {
                this.GanttControl.GanttGrid.SyncSelectedItems(null, e.Action);
            }
        }

        /// <summary>
        /// Syncs the selected items between Gantt Chart and Gantt Grid.
        /// </summary>
        /// <param name="iList">The ilist.</param>
        /// <param name="ChangeAction">The change action.</param>
        internal void SyncSelectedItems(IEnumerable iList, NotifyCollectionChangedAction ChangeAction)
        {
            IsInSelection = true;

            if (ChangeAction == NotifyCollectionChangedAction.Reset)
            {
                this.GanttControl.SelectedItems.Clear();
                IsInSelection = false;
                return;
            }

            foreach (GridTreeNode node in iList.OfType<GridTreeNode>())
            {
                if (ChangeAction == NotifyCollectionChangedAction.Add)
                {
                    this.GanttControl.SelectedItems.Clear();
                    if (!this.GanttControl.SelectedItems.Contains(node.Item))
                        this.GanttControl.SelectedItems.Add(node.Item);
                }
                else if (ChangeAction == NotifyCollectionChangedAction.Remove)
                {
                    this.GanttControl.SelectedItems.Remove(node.Item);
                }
            }
            var internalGrid = this.GanttControl.GanttGrid.InternalGrid;
#if !SILVERLIGHT
            if (this.GanttControl.GanttGrid.isinexternalselection)
#else
            if (!internalGrid.CurrentCell.IsInMoveTo)
#endif
                internalGrid.InvalidateCells();
            IsInSelection = false;
        }

        /// <summary>
        /// Updates the selected item.
        /// </summary>
        /// <param name="obj">The obj.</param>
        internal void UpdateSelectedItem(object obj)
        {
            if (Keyboard.Modifiers == ModifierKeys.Control)
            {
                if (this.GanttControl.SelectedItems.Contains(obj))
                    this.GanttControl.GanttGrid.RemoveSelectedItem(obj);
                else
                    this.GanttControl.GanttGrid.AddSelectedItem(obj);
            }
            else
            {
                if (this.GanttControl.SelectedItems.Count > 0)
                    this.GanttControl.SelectedItems.Clear();

                this.GanttControl.GanttGrid.AddSelectedItem(obj);
            }
        }

        /// <summary>
        /// Gets the parent of item.
        /// </summary>
        /// <param name="item">The item.</param>
        /// <returns></returns>
        public object GetParentOfItem(object item)
        {
            int rowIndex = GetRowIndexFromItem(item);
            if (rowIndex < 0)
                return null;

            GridTreeNode node = this.GanttControl.GanttGrid.InternalGrid.GetNodeAtRowIndex(rowIndex);
            if(node == null)
                return null;

            return node.ParentNode != null ? node.ParentNode.Item : null;
        }

        /// <summary>
        /// Gets the row index from item.
        /// </summary>
        /// <param name="item">The item.</param>
        /// <returns></returns>
        public int GetRowIndexFromItem(object item)
        {
            if (item == null || this.GanttControl == null)
                return -1;

            if (this.GanttControl.GanttGrid == null || this.GanttControl.GanttGrid.InternalGrid == null)
                return -1;

           return this.GanttControl.GanttGrid.InternalGrid.GetRowIndexFromItem(item);
        }

        /// <summary>
        /// Invalidates the grid.
        /// </summary>
        /// <param name="currentItem">The current item.</param>
        internal void InvalidateGrid(object currentItem)
        {
            int row = this.GetRowIndexFromItem(currentItem);

            if (row >= 0)
                this.GanttControl.GanttGrid.InternalGrid.InvalidateCell(GridRangeInfo.Row(row));
        }

        #endregion

        #region Highlighted Items

        /// <summary>
        /// Called when [highlighted items changed].
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The <see cref="System.Collections.Specialized.NotifyCollectionChangedEventArgs"/> instance containing the event data.</param>
        void OnHighlightedItemsChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            if (this.GanttControl.GanttChart != null)
                this.GanttControl.GanttChart.RaiseHighlightedItemsChanged(new DependencyPropertyChangedEventArgs());
        }

        #endregion

        #region InLine Items Source/Property Change

        /// <summary>
        /// Wires the in line collection changed.
        /// </summary>
        /// <param name="items">The items.</param>
        /// <param name="record">The record.</param>
        void WireInLineCollectionChanged(object items, GanttRecord record)
        {
            if (items is INotifyCollectionChanged)
            {
                var sourceList = items as INotifyCollectionChanged;

                if (!this.InLineChangeListner.Keys.Contains(sourceList))
                {
                    sourceList.CollectionChanged += OnInLineCollectionChanged;
                    this.InLineChangeListner.Add(sourceList, record);
                }
            }
        }

        /// <summary>
        /// Wires the in line property changed.
        /// </summary>
        /// <param name="item">The item.</param>
        /// <param name="record">The record.</param>
        void WireInLinePropertyChanged(object item, GanttRecord record)
        {
            if (item is INotifyPropertyChanged)
            {
                var source = item as INotifyPropertyChanged;

                if (!this.InLinePropChangeListner.Keys.Contains(source))
                {
                    source.PropertyChanged += OnInLinePropertyChanged;
                    this.InLinePropChangeListner.Add(source, record);
                }
            }
        }

        /// <summary>
        /// Uns the wire in line collection changed.
        /// </summary>
        /// <param name="items">The items.</param>
        void UnWireInLineCollectionChanged(object items)
        {
            if (items is INotifyCollectionChanged)
            {
                var sourceList = items as INotifyCollectionChanged;

                sourceList.CollectionChanged -= OnInLineCollectionChanged;

                // Removing the inline items before unwiring the parent item
                GanttRecord record = this.InLineChangeListner[sourceList];
                if (record != null && record.InLineRecords.Count > 0)
                {
                    foreach (var obj in record.InLineRecords)
                    {
                        this.UnWireInLinePropertyChanged(obj.DataItem);
                    }
                    record.InLineRecords.Clear();
                }
                this.InLineChangeListner.Remove(sourceList);
            }
        }

        /// <summary>
        /// Uns the wire in line property changed.
        /// </summary>
        /// <param name="item">The item.</param>
        void UnWireInLinePropertyChanged(object item)
        {
            if (item is INotifyPropertyChanged)
            {
                var source = item as INotifyPropertyChanged;

                source.PropertyChanged -= OnInLinePropertyChanged;
                this.InLinePropChangeListner.Remove(source);
            }
        }

        /// <summary>
        /// Checks for in line items.
        /// </summary>
        /// <param name="node">The node.</param>
        /// <param name="currentRecord">The current record.</param>
        private void CheckForInLineItems(object node, GanttRecord currentRecord)
        {
            if (node == null || string.IsNullOrEmpty(this.TaskAttributeMapping.InLineTaskMapping))
                return;

            IEnumerable inLineItems = null;

            // Fetching the inline items from soruce
            try
            {
                inLineItems = this.ItemProperties[this.TaskAttributeMapping.InLineTaskMapping].GetValue(node) as IEnumerable;
            }
            catch (Exception)
            {
                throw new Exception("InLine task mapping name is missing or mapping name is wrong.");
            }

            if (inLineItems == null)
                return;

            // Creating inline Items
            CreateInLineRecrods(currentRecord, inLineItems);
        }

        /// <summary>
        /// Creates the in line recrods.
        /// </summary>
        /// <param name="currentRecord">The current record.</param>
        /// <param name="inLineItems">The in line items.</param>
        private void CreateInLineRecrods(GanttRecord currentRecord, IEnumerable inLineItems)
        {
            foreach (var item in inLineItems)
            {
                // Creating Gantt record for each inline item
                GanttRecord record = new GanttRecord { DataItem = item, ParentRecord = currentRecord, IsExpanded = true };
                currentRecord.InLineRecords.Add(record);

                if (IsAutoUpdateEnabled)
                {
                    // calculating duration for each inline item
                    CalculateDuration(item);

                    // wiring property changed for each inline item
                    this.WireInLinePropertyChanged(item, record);
                }
            }

            // wiring collection change for inline item
            this.WireInLineCollectionChanged(inLineItems, currentRecord);

            // Updating the parent item
            if (IsAutoUpdateEnabled && currentRecord.InLineRecords.Count > 0)
            {
                this.UpdateParentInfo(currentRecord, currentRecord.InLineRecords);
            }
        }

        /// <summary>
        /// Called when [in line collection changed].
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The <see cref="System.Collections.Specialized.NotifyCollectionChangedEventArgs"/> instance containing the event data.</param>
        void OnInLineCollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            GanttRecord record;

            // Initial validation
            if (!this.InLineChangeListner.TryGetValue(sender, out record) || record == null)
                return;

            if (e.Action == NotifyCollectionChangedAction.Add)
            {
                // Creating inline records
                CreateInLineRecrods(record, e.NewItems);
            }
            else if (e.Action == NotifyCollectionChangedAction.Remove)
            {
                bool isInlineItemsCleared = false;
                // Removing inline records
                for (int i = 0; i < e.OldItems.Count; i++)
                {
                    this.UnWireInLinePropertyChanged(record.InLineRecords[e.OldStartingIndex]);
                    record.InLineRecords.RemoveAt(e.OldStartingIndex);
                    if (record.InLineRecords.Count == 0)
                        isInlineItemsCleared = true;
                }

                // Updating the parent item
                if (IsAutoUpdateEnabled && record.InLineRecords.Count > 0)
                {
                    this.UpdateParentInfo(record, record.InLineRecords);
                }

                //Updating the parent item when all the inline items removed.
                if (IsAutoUpdateEnabled && isInlineItemsCleared)
                {
                    this.UpdateInlineParentInfo(record);
                }
            }
            else if (e.Action == NotifyCollectionChangedAction.Reset)
            {
                // un wiring the propety changed of the inline record before clearing it
                foreach (var item in e.OldItems)
                {
                    this.UnWireInLinePropertyChanged(item);
                }
                record.InLineRecords.Clear();
            }

            this.GanttControl.GanttChart.InLineCollectionChanged(record, e);
        }

        /// <summary>
        /// Called when [in line property changed].
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The <see cref="System.ComponentModel.PropertyChangedEventArgs"/> instance containing the event data.</param>
        void OnInLinePropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            GanttRecord record;

            if (!this.InLinePropChangeListner.TryGetValue(sender, out record) || record == null || record.DataItem == null)
                return;

            // Initial validation
            if (sender == null || this.ItemProperties == null || this.ItemProperties.Count <= 0 || this.TaskAttributeMapping == null ||
                this.TaskAttributeMapping.MappedAttributes == null || !this.TaskAttributeMapping.MappedAttributes.Values.Contains(e.PropertyName))
                return;

            // To update the start/finish date and duration.
            if (e.PropertyName.Equals(this.TaskAttributeMapping.StartDateMapping) || e.PropertyName.Equals(this.TaskAttributeMapping.FinishDateMapping) ||
                e.PropertyName.Equals(this.TaskAttributeMapping.DurationMapping))
            {

                UpdateParentBasicValues(record.ParentRecord, record.ParentRecord.InLineRecords);
            }
            else if (e.PropertyName.Equals(this.TaskAttributeMapping.ProgressMapping) && record.ParentRecord != null)
            {
                // To update the progress in parent node
                UpdateParentProgress(record.ParentRecord, record.ParentRecord.InLineRecords);
            }
            else if (e.PropertyName.Equals(this.TaskAttributeMapping.CostMapping) && record.ParentRecord != null)
            {
                // To update the cost in parent node
                UpdateParentCost(record.ParentRecord, record.ParentRecord.InLineRecords);
            }
            else if (e.PropertyName.Equals(this.TaskAttributeMapping.BaselineStartMapping) && record.ParentRecord != null)
            {
                // To update the Base line start in parend node
                UpdateParentBaseStart(record.ParentRecord, record.ParentRecord.InLineRecords);
            }
            else if (e.PropertyName.Equals(this.TaskAttributeMapping.BaselineFinishMapping) && record.ParentRecord != null)
            {
                // To update the Base line finish in parend node
                UpdateParentBaseFinish(record.ParentRecord, record.ParentRecord.InLineRecords);
            }
            else if (e.PropertyName.Equals(this.TaskAttributeMapping.BaselineCostMapping) && record.ParentRecord != null)
            {
                // To update the Base line cost in parend node
                UpdateParentBaseCost(record.ParentRecord, record.ParentRecord.InLineRecords);
            }
            else if ((e.PropertyName.Equals(this.TaskAttributeMapping.StartPointMapping) || e.PropertyName.Equals(this.TaskAttributeMapping.FinishPointMapping)) && record.ParentRecord != null)
            {
                // To update the parent node for numeric start/finish for custom numeric schedule
                UpdateParentNumericInfo(record.ParentRecord, record.ParentRecord.InLineRecords);
            }
        }

        #endregion

        #region Project Statistics Information
        /// <summary>
        /// Gets the Project Start Date
        /// </summary>
        /// <returns>DateTime instance</returns>
        internal DateTime GetProjStartDate()
        {
            var startDate = new DateTime();
            if (this.TaskAttributeMapping == null || string.IsNullOrEmpty(this.TaskAttributeMapping.StartDateMapping))
                return startDate;
            if (this.SourceList != null)
            {
                var collection = GetObservableCollection(this.SourceList);
                if (collection != null && collection.Count >= 1)
                {
                    try
                    {
                        var query = from taskdetail in collection select (DateTime)this.ItemProperties[this.TaskAttributeMapping.StartDateMapping].GetValue(taskdetail);
                        startDate = query.Min<DateTime>();
                    }
                    catch (Exception)
                    {
                        throw new Exception("Finish date mapping name is missing or mapping name is wrong.");
                    }
                    return startDate;
                }
            }
            return startDate;
        }

        /// <summary>
        /// Gets the Project BaselineStartDate
        /// </summary>
        /// <returns>DateTime Instance</returns>
        internal DateTime GetProjBaselineStartDate()
        {
            var baselineStartDate = new DateTime();
            if (this.TaskAttributeMapping == null || string.IsNullOrEmpty(this.TaskAttributeMapping.BaselineStartMapping))
                return baselineStartDate;
            if (this.SourceList != null)
            {
                var collection = GetObservableCollection(this.SourceList);
                if (collection != null && collection.Count >= 1)
                {
                    try
                    {
                        var query = from taskdetail in collection select (DateTime)this.ItemProperties[this.TaskAttributeMapping.BaselineStartMapping].GetValue(taskdetail);
                        baselineStartDate = query.Min<DateTime>();
                    }
                    catch (Exception)
                    {
                        throw new Exception("Baseline start date mapping name is missing or mapping name is wrong.");
                    }
                    return baselineStartDate;
                }
            }
            return baselineStartDate;
        }

        /// <summary>
        /// Gets the Project End Date
        /// </summary>
        /// <returns>DateTime instance</returns>
        internal DateTime GetProjEndDate()
        {
            var endDate = new DateTime();
            if (this.TaskAttributeMapping == null || string.IsNullOrEmpty(this.TaskAttributeMapping.FinishDateMapping))
                return endDate;
            if (this.SourceList != null)
            {
                var collection = GetObservableCollection(this.SourceList);
                if (collection != null && collection.Count >= 1)
                {
                    try
                    {
                        var query = from taskdetail in collection select (DateTime)this.ItemProperties[this.TaskAttributeMapping.FinishDateMapping].GetValue(taskdetail);
                        endDate = query.Max<DateTime>();
                    }
                    catch (Exception)
                    {
                        throw new Exception("Finish date mapping name is missing or mapping name is wrong.");
                    }
                    return endDate;
                }
            }
            return endDate;
        }

        /// <summary>
        /// Gets the Baseline End Date of the Project
        /// </summary>
        /// <returns>DateTime instance</returns>
        internal DateTime GetProjBaselineEndDate()
        {
            var baselineEndDate = new DateTime();
            if (this.TaskAttributeMapping == null || string.IsNullOrEmpty(this.TaskAttributeMapping.BaselineFinishMapping))
                return baselineEndDate;
            if (this.SourceList != null)
            {
                var collection = GetObservableCollection(this.SourceList);
                if (collection != null && collection.Count >= 1)
                {
                    try
                    {
                        var query = from taskdetail in collection select (DateTime)this.ItemProperties[this.TaskAttributeMapping.BaselineFinishMapping].GetValue(taskdetail);
                        baselineEndDate = query.Max<DateTime>();
                    }
                    catch (Exception)
                    {
                        throw new Exception("Baseline finish date mapping name is missing or mapping name is wrong.");
                    }

                    return baselineEndDate;
                }
            }
            return baselineEndDate;
        }

        /// <summary>
        /// Gets the cost spent for the project
        /// </summary>
        /// <returns>Double</returns>
        internal Double GetProjCost()
        {
            var cost = 0d;
            if (this.TaskAttributeMapping == null || string.IsNullOrEmpty(this.TaskAttributeMapping.CostMapping))
                return cost;
            if (this.SourceList != null)
            {
                var collection = GetObservableCollection(this.SourceList);
                if (collection != null && collection.Count >= 1)
                {
                    try
                    {
                        var query = from taskdetail in collection select (Double)this.ItemProperties[this.TaskAttributeMapping.CostMapping].GetValue(taskdetail);
                        var sum = 0d;
                        cost = query.Aggregate(sum, (temp, cst) => temp + cst);
                    }
                    catch (Exception)
                    {
                        throw new Exception("Cost mapping name is missing or mapping name is wrong.");
                    }
                    return cost;
                }
            }
            return cost;
        }

        /// <summary>
        /// Gets the project baseline Cost [the cost supposed to spend]
        /// </summary>
        /// <returns>Double</returns>
        internal Double GetProjBaselineCost()
        {
            var baselineCost = 0d;
            if (this.TaskAttributeMapping == null || string.IsNullOrEmpty(this.TaskAttributeMapping.BaselineCostMapping))
                return baselineCost;
            if (this.SourceList != null)
            {
                var collection = GetObservableCollection(this.SourceList);
                if (collection != null && collection.Count >= 1)
                {
                    try
                    {
                        var query = from taskdetail in collection select (Double)this.ItemProperties[this.TaskAttributeMapping.BaselineCostMapping].GetValue(taskdetail);
                        var sum = 0d;
                        baselineCost = query.Aggregate(sum, (temp, cst) => temp + cst);
                    }
                    catch (Exception)
                    {
                        throw new Exception("Baseline cost mapping name is missing or mapping name is wrong.");
                    }
                    return baselineCost;
                }
            }
            return baselineCost;
        }

        /// <summary>
        /// Gets the Current Progress of the Project
        /// </summary>
        /// <returns>Double</returns>
        internal Double GetProjectProgress()
        {
            var progress = 0d;
            if (this.TaskAttributeMapping == null || string.IsNullOrEmpty(this.TaskAttributeMapping.ProgressMapping))
                return progress;
            if (this.SourceList != null)
            {
                var collection = GetObservableCollection(this.SourceList);
                if (collection != null && collection.Count >= 1)
                {
                    try
                    {
                        var query = from taskdetail in collection select (Double)this.ItemProperties[this.TaskAttributeMapping.ProgressMapping].GetValue(taskdetail);
                        if (query.Any(prog => prog > 0))
                        {
                            this.IsWorkStarted = true;
                        }
                        progress = query.Average();
                    }
                    catch (Exception)
                    {
                        throw new Exception("Progress mapping name is missing or mapping name is wrong.");
                    }
                    return progress;
                }
            }
            return progress;
        }

        /// <summary>
        /// Gets the Project Statistics Information
        /// </summary>
        /// <returns>ProjectInfo Object</returns>
        internal ProjectInfo GetProjectStatistics()
        {
            // Temp Variables used in Progress Calculations
            double actualProgress; int days, hours;
            Double Progress = this.GetProjectProgress();

            // Creating instance for ProjectInfo
            ProjectInfo CurrentProjectInfo = new ProjectInfo();

            // Sets the current Project Name
            CurrentProjectInfo.ProjectName = this.GanttControl.ProjectName;

            // Sets the Current Project Start Date
            CurrentProjectInfo.StartDate = this.GetProjStartDate();

            // sets the Actual start Date
            if (this.IsWorkStarted)
                CurrentProjectInfo.ActualStartDate = CurrentProjectInfo.StartDate;

            // Sets the Actual Start Date
            CurrentProjectInfo.BaselineStart = this.GetProjBaselineStartDate();

            // Calculating start variance 
            if (CurrentProjectInfo.StartDate == DateTime.MinValue || CurrentProjectInfo.BaselineStart == DateTime.MinValue)
                CurrentProjectInfo.StartVariance = new TimeSpan(0, 0, 0, 0);
            else
                CurrentProjectInfo.StartVariance = CurrentProjectInfo.StartDate.Subtract(CurrentProjectInfo.BaselineStart);

            // Sets the End Date of Project
            CurrentProjectInfo.FinishDate = this.GetProjEndDate();

            // Sets the Actual End Date
            if (Progress == 100)
                CurrentProjectInfo.ActualFinishDate = CurrentProjectInfo.FinishDate;

            // Sets the Baseline End Date and Calculates End Variance
            CurrentProjectInfo.BaselineFinish = this.GetProjBaselineEndDate();
            if (CurrentProjectInfo.FinishDate == DateTime.MinValue || CurrentProjectInfo.BaselineFinish == DateTime.MinValue)
                CurrentProjectInfo.FinishVariance = new TimeSpan(0, 0, 0, 0);
            else
                CurrentProjectInfo.FinishVariance = CurrentProjectInfo.FinishDate.Subtract(CurrentProjectInfo.BaselineFinish);

            // Sets the current project Duration 
            CurrentProjectInfo.Duration = CurrentProjectInfo.FinishDate.AddDays(1).Subtract(CurrentProjectInfo.StartDate);

            // * Calculations for Actual Duration of the Project
            // * This is based on the Project Progress.
            actualProgress = ((CurrentProjectInfo.Duration.TotalDays / 100) * Progress);
            actualProgress = Math.Round(actualProgress, 2);
            days = (int)Math.Floor(actualProgress);
            double temp = actualProgress - days;
            hours = (int)Math.Round(temp * 24, 0);
            CurrentProjectInfo.ActualDuration = new TimeSpan(days, hours, 0, 0);

            if (CurrentProjectInfo.BaselineFinish == DateTime.MinValue || CurrentProjectInfo.BaselineStart == DateTime.MinValue)
                CurrentProjectInfo.BaselineDuration = new TimeSpan(0, 0, 0, 0);
            else
                CurrentProjectInfo.BaselineDuration = CurrentProjectInfo.BaselineFinish.AddDays(1).Subtract(CurrentProjectInfo.BaselineStart);

            // Calculation of Remaining Duration
            CurrentProjectInfo.RemainingDuration = CurrentProjectInfo.Duration.Subtract(CurrentProjectInfo.ActualDuration);

            // Calculation for Actual cost
            CurrentProjectInfo.Cost = Math.Round(this.GetProjCost(), 2);
            CurrentProjectInfo.ActualCost = Math.Round(((CurrentProjectInfo.Cost / 100) * Progress), 2);

            CurrentProjectInfo.BaselineCost = Math.Round(this.GetProjBaselineCost(), 2);

            // Calculation for Remaining Cost
            CurrentProjectInfo.RemainingCost = Math.Round((CurrentProjectInfo.Cost - CurrentProjectInfo.ActualCost), 2);

            return CurrentProjectInfo;
        }
        #endregion
    }
}
