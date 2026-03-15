using System;
using System.Windows;
using System.ComponentModel;
using System.Collections.Generic;
using DevExpress.Xpf.PivotGrid;
using DevExpress.Mvvm.UI.Interactivity;
using System.Runtime.Serialization;
using System.IO;
using DocumentManager.ComponentService;
using VFS;
using UIMsgBoxAlertService.ComponentService;
using System.Xml;
using System.Linq;
using WPFUtilities.Extensions;

namespace GridLayout
{
    public class PivotGridLayoutHelper : Behavior<PivotGridControl>
    {
        public event EventHandler<PivotEventArgs> LayoutChanged;
        List<PivotLayoutChangedType> PivotLayoutChangedTypes = new List<PivotLayoutChangedType>();
        PivotGridControl Grid { get { return AssociatedObject; } }
        bool IsLocked;

        #region DependencyPropertyDescriptors
        DependencyPropertyDescriptor ActualWidthDescriptor
        {
            get
            {
                return DependencyPropertyDescriptor.FromProperty(PivotGridField.WidthProperty, typeof(PivotGridField));
            }
        }
        DependencyPropertyDescriptor AreaIndexDescriptor
        {
            get
            {
                return DependencyPropertyDescriptor.FromProperty(PivotGridField.AreaIndexProperty, typeof(PivotGridField));
            }
        }
        DependencyPropertyDescriptor GroupIndexDescriptor
        {
            get
            {
                return DependencyPropertyDescriptor.FromProperty(PivotGridField.GroupIndexProperty, typeof(PivotGridField));
            }
        }
        DependencyPropertyDescriptor VisibleDescriptor
        {
            get
            {
                return DependencyPropertyDescriptor.FromProperty(PivotGridField.VisibleProperty, typeof(PivotGridField));
            }
        }

        #endregion

        protected override void OnAttached()
        {
            base.OnAttached();
            LocalDetach();

            SubscribeColumns();
            Grid.Loaded += OnGridLoaded;
        }
        protected override void OnDetaching()
        {
            LocalDetach();
            base.OnDetaching();
        }
        private void LocalDetach()
        {
            if (Grid.Fields != null)
                UnSubscribeColumns();
            Grid.Loaded -= OnGridLoaded;
            if(PivotLayoutChangedTypes != null)
                PivotLayoutChangedTypes.Clear();
        }

        public void SubscribeColumns()
        {
            Grid.Fields.CollectionChanged += ColumnsCollectionChanged;
            foreach (PivotGridField field in Grid.Fields)
            {
                SubscribeColumn(field);
            }
        }
        public void UnSubscribeColumns()
        {
            Grid.Fields.CollectionChanged -= ColumnsCollectionChanged;
            foreach (PivotGridField field in Grid.Fields)
            {
                UnSubscribeColumn(field);
            }
        }
        void SubscribeColumn(PivotGridField field)
        {
            ActualWidthDescriptor.AddValueChangedSafe(field, OnColumnWidthChanged);
            AreaIndexDescriptor.AddValueChangedSafe(field, OnColumnVisibleIndexChanged);
            GroupIndexDescriptor.AddValueChangedSafe(field, OnColumnGroupIndexChanged);
            VisibleDescriptor.AddValueChangedSafe(field, OnColumnVisibleChanged);
        }
        void UnSubscribeColumn(PivotGridField field)
        {
            ActualWidthDescriptor.RemoveValueChangedSafe(field, OnColumnWidthChanged);
            AreaIndexDescriptor.RemoveValueChangedSafe(field, OnColumnVisibleIndexChanged);
            GroupIndexDescriptor.RemoveValueChangedSafe(field, OnColumnGroupIndexChanged);
            VisibleDescriptor.RemoveValueChangedSafe(field, OnColumnVisibleChanged);
        }
        private void OnPivotLayoutChangedType(PivotEventArgs e)
        {
            EventHandler<PivotEventArgs> temp = LayoutChanged;
            if (temp != null)
                temp(null, e);
        }
        void ProcessLayoutChanging(PivotLayoutChangedType type)
        {
            if (PivotLayoutChangedTypes != null && !PivotLayoutChangedTypes.Contains(type))
                PivotLayoutChangedTypes.Add(type);
            if (IsLocked)
                return;
            IsLocked = true;
            Dispatcher.BeginInvoke(new Action(() => {
                IsLocked = false;
                PivotEventArgs m = new PivotEventArgs { PivotLayoutChangedTypes = PivotLayoutChangedTypes };
                OnPivotLayoutChangedType(m);
                if(PivotLayoutChangedTypes != null)
                    PivotLayoutChangedTypes.Clear();
            }));
        }
        void OnGridLoaded(object sender, System.Windows.RoutedEventArgs e)
        {
            UnSubscribeColumns();
            SubscribeColumns();
        }
        void ColumnsCollectionChanged(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
        {
            if (e.OldItems != null)
                foreach (PivotGridField field in e.OldItems)
                    UnSubscribeColumn(field);
            if (e.NewItems != null)
                foreach (PivotGridField field in e.NewItems)
                    SubscribeColumn(field);
            if (e.NewItems != null || e.OldItems != null)
            {
                ProcessLayoutChanging(PivotLayoutChangedType.ColumnsCollection);
            }
        }
        void OnColumnWidthChanged(object sender, EventArgs args)
        {
            ProcessLayoutChanging(PivotLayoutChangedType.ColumnWidth);
        }
        void OnColumnVisibleIndexChanged(object sender, EventArgs args)
        {
            ProcessLayoutChanging(PivotLayoutChangedType.ColumnVisibleIndex);
        }
        void OnColumnGroupIndexChanged(object sender, EventArgs args)
        {
            ProcessLayoutChanging(PivotLayoutChangedType.ColumnGroupIndex);
        }
        void OnColumnVisibleChanged(object sender, EventArgs args)
        {
            ProcessLayoutChanging(PivotLayoutChangedType.ColumnVisible);
        }

    }
    public class PivotEventArgs : EventArgs
    {
        public List<PivotLayoutChangedType> PivotLayoutChangedTypes { get; set; }
    }
    public enum PivotLayoutChangedType
    {
        ColumnsCollection,
        FilerChanged,
        ColumnGroupIndex,
        ColumnVisibleIndex,
        ColumnWidth,
        ColumnVisible,
        None
    }
}
