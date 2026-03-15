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
using System.Linq;
using Syncfusion.Data;
#if !WinRT
using System.Windows;
using Syncfusion.Data.Extensions;
#else
using System.Reflection;
using Windows.Foundation;
using Windows.UI.Xaml;
#endif

namespace Syncfusion.UI.Xaml.Grid
{
    internal interface IDetailsViewNotifier
    {
        IDetailsViewNotifyListener NotifyListener { get; }
        bool IsListenerSuspended { get; }
        void SetNotifierListener(IDetailsViewNotifyListener notifyListener);
        void SuspendNotifyListener();
        void ResumeNotifyListener();
    }

    internal interface IDetailsViewNotifyListener
    {
        SfDataGrid RootDataGrid { get; }
        SfDataGrid GetParentDataGrid();
        void NotifyPropertyChanged(object source, string propertyName, DependencyPropertyChangedEventArgs e, Func<SfDataGrid, object> target, IDetailsViewNotifier notifier, Type ownerType);
        void NotifyCollectionChanged(object source, NotifyCollectionChangedEventArgs e, Func<SfDataGrid, object> target, IDetailsViewNotifier notifier, Type baseType);
        void EnsureCollection<T, S>(T source, Func<SfDataGrid, T> target, Func<S, S, bool> predicate, IDetailsViewNotifier notifier) where T : IList<S>;
    }

    internal interface IDetailsViewInfo
    {
        double GetExtendedWidth();
        void SetClipRect(Rect rect);
        void SetHorizontalOffset(double offset);
        void SetVerticalOffset(double offset);
    }

    internal class DetailsViewNotifyListener : IDetailsViewNotifyListener, IDisposable
    {
        internal List<SfDataGrid> ClonedDataGrid;
        private readonly SfDataGrid _parentDataGrid;
        public SfDataGrid RootDataGrid { get; private set; }

        public DetailsViewNotifyListener(SfDataGrid rootDataGrid, SfDataGrid parentDataGrid)
        {
            RootDataGrid = rootDataGrid;
            _parentDataGrid = parentDataGrid;
            RootDataGrid.ForceInitializeDetailsViewGrid();
            (RootDataGrid as IDetailsViewNotifier).SetNotifierListener(this);
            ClonedDataGrid = new List<SfDataGrid>();
        }

        public DetailsViewDataGrid CopyPropertiesFromRootGrid(SfDataGrid sourceDataGrid)
        {
            var destinationDataGrid = new DetailsViewDataGrid();
            CloneHelper.CloneProperties(sourceDataGrid, destinationDataGrid, typeof(SfDataGrid));
            CloneHelper.CloneCollection(sourceDataGrid.Columns, destinationDataGrid.Columns, typeof(GridColumn));
            CloneHelper.CloneCollection(sourceDataGrid.SortColumnDescriptions, destinationDataGrid.SortColumnDescriptions, typeof(SortColumnDescription));
            CloneHelper.CloneCollection(sourceDataGrid.GroupColumnDescriptions, destinationDataGrid.GroupColumnDescriptions, typeof(GroupColumnDescription));
            CloneHelper.CloneCollection(sourceDataGrid.GroupSummaryRows, destinationDataGrid.GroupSummaryRows, typeof(GridSummaryRow));
            CloneHelper.CloneCollection(sourceDataGrid.TableSummaryRows, destinationDataGrid.TableSummaryRows, typeof(GridSummaryRow));
            CloneHelper.CloneCollection(sourceDataGrid.StackedHeaderRows, destinationDataGrid.StackedHeaderRows, typeof(StackedHeaderRow));
            CloneHelper.CloneCollection(sourceDataGrid.DetailsViewDefinition, destinationDataGrid.DetailsViewDefinition, typeof(ViewDefinition));
            foreach (var targetColumn in destinationDataGrid.Columns)
            {
                var sourceColumn = sourceDataGrid.Columns.FirstOrDefault(x => x.MappingName == targetColumn.MappingName);
                if(sourceColumn != null)
                    CloneHelper.CloneCollection(sourceColumn.FilterPredicates, targetColumn.FilterPredicates, typeof(FilterPredicate));
            }
            foreach (var targetColumn in destinationDataGrid.StackedHeaderRows)
            {
                var sourceColumn = sourceDataGrid.StackedHeaderRows.ElementAt(destinationDataGrid.StackedHeaderRows.IndexOf(targetColumn));
                if (sourceColumn != null)
                    CloneHelper.CloneCollection(sourceColumn.StackedColumns, targetColumn.StackedColumns, typeof(StackedColumn));
            }
            destinationDataGrid.SelectionMode = _parentDataGrid.SelectionMode;
            destinationDataGrid.NavigationMode = _parentDataGrid.NavigationMode;
            destinationDataGrid.ColumnSizer = _parentDataGrid.ColumnSizer;
            destinationDataGrid.DetailsViewPadding = _parentDataGrid.DetailsViewPadding;
            destinationDataGrid.InitializeDetailsViewDataGrid();
            ClonedDataGrid.Add(destinationDataGrid);
            (destinationDataGrid as IDetailsViewNotifier).SetNotifierListener(this);
            return destinationDataGrid;
        }

        public SfDataGrid GetParentDataGrid()
        {
            return _parentDataGrid;
        }

        public void NotifyPropertyChanged(object source, string propertyName, DependencyPropertyChangedEventArgs e, Func<SfDataGrid, object> target, IDetailsViewNotifier notifier, Type ownerType)
        {
            if (notifier.IsListenerSuspended) return;
#if WinRT
            if (!ownerType.GetTypeInfo().IsAssignableFrom(source.GetType().GetTypeInfo())) return;
#else
            if (!ownerType.IsInstanceOfType(source)) return;
#endif
            var propertyDescriptor = CloneHelper.GetCloneableProperty(source.GetType(), ownerType, propertyName);
            if (propertyDescriptor == null) return;
            notifier.SuspendNotifyListener();
            if (this.RootDataGrid != notifier)
            {
                var rootNotifier = RootDataGrid as IDetailsViewNotifier;
                if (!rootNotifier.IsListenerSuspended)
                {
                    var targetElement = target(RootDataGrid);
#if WPF
                    propertyDescriptor.SetValue(targetElement, e.NewValue);
#else
                    propertyDescriptor.SetValue(targetElement, e.NewValue, null);
#endif
                }
            }
            else
            {
                foreach (var clonedDataGrid in ClonedDataGrid)
                {
                    var clonedNotifier = clonedDataGrid as IDetailsViewNotifier;
                    if (clonedNotifier != null && clonedNotifier.IsListenerSuspended) continue;
                    var clonedTargetElement = target(clonedDataGrid);
#if WPF
                    propertyDescriptor.SetValue(clonedTargetElement, e.NewValue);
#else
                    propertyDescriptor.SetValue(clonedTargetElement, e.NewValue, null);
#endif
                }
            }
            notifier.ResumeNotifyListener();
        }

        public void NotifyCollectionChanged(object source, NotifyCollectionChangedEventArgs e, Func<SfDataGrid, object> target, IDetailsViewNotifier notifier, Type baseType)
        {
            if (notifier.IsListenerSuspended) return;
            notifier.SuspendNotifyListener();
            if (this.RootDataGrid != notifier)
            {
                var rootNotifier = RootDataGrid as IDetailsViewNotifier;
                if (!rootNotifier.IsListenerSuspended)
                {
                    var targetElement = target(RootDataGrid);
                    ProcessCollectionChanged(e, (IList) source, (IList) targetElement, baseType);
                }
            }
            else
            {
                foreach (var clonedDataGrid in ClonedDataGrid)
                {
                    var clonedNotifier = clonedDataGrid as IDetailsViewNotifier;
                    if (clonedNotifier != null && clonedNotifier.IsListenerSuspended) continue;
                    var clonedTargetElement = target(clonedDataGrid);
                    ProcessCollectionChanged(e, (IList) source, (IList) clonedTargetElement, baseType);
                }
            }
            notifier.ResumeNotifyListener();
        }

        public void EnsureCollection<T, S>(T source, Func<SfDataGrid, T> target, Func<S, S, bool> predicate, IDetailsViewNotifier notifier) where T : IList<S>
        {
            if (this.RootDataGrid == notifier) return;
            var targetList = target(this.RootDataGrid);
            if (source.Count != targetList.Count)
            {
                targetList.Clear();
                CloneHelper.CloneCollection((IList)source, this.RootDataGrid.Columns, typeof(S));
                return;
            }
            if (!source.Any(column => targetList.All(col => predicate(col, column))))
                return;
            targetList.Clear();
            CloneHelper.CloneCollection((IList)source, this.RootDataGrid.Columns, typeof(S));
        }

        public static void ProcessCollectionChanged(NotifyCollectionChangedEventArgs e, IList source, IList target, Type baseType)
        {
            if (target == null) return;
            switch (e.Action)
            {
                case NotifyCollectionChangedAction.Add:
                    {
                        foreach (var newItem in e.NewItems)
                        {
                            var clonedNewItem = CloneHelper.CreateClonedInstance(newItem, baseType);
                            var index = source.IndexOf(newItem);
                            target.Insert(index, clonedNewItem);
                        }
                    }
                    break;
                case NotifyCollectionChangedAction.Remove:
                    {
                        foreach (var oldItem in e.OldItems)
                            target.RemoveAt(e.OldStartingIndex);
                    }
                    break;
#if WPF
                 case NotifyCollectionChangedAction.Move:
                    {
                        var obj1 = target[e.OldStartingIndex];
                        var obj2 = target[e.NewStartingIndex];
                        var num = (e.NewStartingIndex > e.OldStartingIndex) ? 1 : 0;
                        target.Remove(obj1);
                        target.Insert(target.IndexOf(obj2) + num, obj1);
                    }
                    break;
#endif
                case NotifyCollectionChangedAction.Reset:
                    {
                        target.Clear();
                        CloneHelper.CloneCollection(source, target, baseType);
                    }
                    break;
            }
        }

        public void Dispose()
        {
            RootDataGrid = null;
            if (ClonedDataGrid == null) return;
            ClonedDataGrid.Clear();
            ClonedDataGrid = null;
        }
    }
}
