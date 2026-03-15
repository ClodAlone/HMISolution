#region Copyright Syncfusion Inc. 2001 - 2014
// Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Any infringement will be prosecuted under
// applicable laws. 
#endregion
using Syncfusion.UI.Xaml.ScrollAxis;
using System;
using System.Linq;
using Syncfusion.Data.Extensions;
#if !WP
using System.ComponentModel.DataAnnotations;
using System.Dynamic;
#endif

#if WinRT
using System.Reflection;
using System.Threading.Tasks;
using Windows.Foundation;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media.Animation;
using Syncfusion.Data;
using Windows.UI.Xaml.Data;
using Windows.ApplicationModel.Resources;
#else
using System.Windows;
using System.Collections.ObjectModel;
using System.Windows.Controls;
using System.Windows.Media.Animation;
using System.Windows.Input;
using System.ComponentModel;
using System.Windows.Controls.Primitives;
using System.Windows.Threading;
using System.Windows.Data;
using System.Threading;
using Pointer = System.Object;
using System.Resources;
using System.Reflection;
#if !SILVERLIGHT && !WP7
using System.Threading.Tasks;
using System.Collections.Specialized;
#endif
#endif


namespace Syncfusion.UI.Xaml.Grid
{ 
    
    [TemplatePart(Name = "PART_StackPanel", Type = typeof(StackPanel))]
#if !WinRT
    [TemplatePart(Name = "PART_GroupDropAreaGrid", Type = typeof(System.Windows.Controls.Grid))]
#else
    [TemplatePart(Name = "PART_GroupDropAreaGrid", Type = typeof(Windows.UI.Xaml.Controls.Grid))]
#endif
    public class GroupDropArea : Control, IDisposable
    {
        #region Fields

        internal SfDataGrid dataGrid;
        private bool isGroupDropAreaExpandedSetBeforeGridLoaded;
        internal StackPanel Panel;
#if !WinRT
        internal System.Windows.Controls.Grid groupItemsGrid;
#if WP
        internal ScrollViewer GroupItemScroller;
        internal bool scrollersDisabled;
#endif
#else
        internal Windows.UI.Xaml.Controls.Grid groupItemsGrid;
#endif
        
        #endregion

        #region Ctor
        static GroupDropArea()

        {
#if WPF
            DefaultStyleKeyProperty.OverrideMetadata(typeof(GroupDropArea), new FrameworkPropertyMetadata(typeof(GroupDropArea)));
#endif
        }

        public GroupDropArea()
        {
#if !WPF
            this.DefaultStyleKey = typeof(GroupDropArea);
#endif      
        }

        #endregion

        #region DependencyProperty

        public bool IsExpanded
        {
            get { return (bool) GetValue(IsExpandedProperty); }
            set { SetValue(IsExpandedProperty, value); }
        }

        public static readonly DependencyProperty IsExpandedProperty =
            DependencyProperty.Register("IsExpanded", typeof (bool), typeof (GroupDropArea),
                                        new PropertyMetadata(false, OnIsExpandedPropertyChanged));

        private static void OnIsExpandedPropertyChanged(DependencyObject obj, DependencyPropertyChangedEventArgs agrs)
        {
            var groupDropArea = obj as GroupDropArea;
            if (groupDropArea != null)
                (obj as GroupDropArea).ToggleExpanded((bool) agrs.NewValue);
        }

        /// <summary>
        /// Gets or sets the group drop area text.
        /// </summary>
        /// <value>The group drop area text.</value>
        public string GroupDropAreaText
        {
            get { return (string) GetValue(GroupDropAreaTextProperty); }
            set { SetValue(GroupDropAreaTextProperty, value); }
        }

#if WP
        public static readonly DependencyProperty GroupDropAreaTextProperty =
            DependencyProperty.Register("GroupDropAreaText", typeof (string), typeof (GroupDropArea),
                                        new PropertyMetadata("Drag To Group"));
#else

        public static readonly DependencyProperty GroupDropAreaTextProperty =
            DependencyProperty.Register("GroupDropAreaText", typeof (string), typeof (GroupDropArea), new PropertyMetadata(GridResourceWrapper.GroupDropAreaText));
#endif
        public Visibility WatermarkTextVisibility
        {
            get { return (Visibility) GetValue(WatermarkTextVisibilityProperty); }
            set { SetValue(WatermarkTextVisibilityProperty, value); }
        }

        public static readonly DependencyProperty WatermarkTextVisibilityProperty =
            DependencyProperty.Register("WatermarkTextVisibility", typeof (Visibility), typeof (GroupDropArea),
                                        new PropertyMetadata(Visibility.Visible));

        #endregion

        #region Override Methods

#if WinRT
        protected override void OnApplyTemplate()
#else
        public override void OnApplyTemplate()
#endif
        {
            base.OnApplyTemplate();
            Panel = this.GetTemplateChild("PART_StackPanel") as StackPanel;
#if !WinRT
            groupItemsGrid = this.GetTemplateChild("PART_GroupDropAreaGrid") as System.Windows.Controls.Grid;
#else
            groupItemsGrid = this.GetTemplateChild("PART_GroupDropAreaGrid") as Windows.UI.Xaml.Controls.Grid;
#endif

            if (Panel != null)
            {
#if WP
                GroupItemScroller = this.GetTemplateChild("PART_ScrollViewer") as ScrollViewer;
                GroupItemScroller.ManipulationMode = ManipulationMode.Control;
                GroupItemScroller.ManipulationStarted += GroupItemScroller_ManipulationStarted;
#endif
                //When we set ShowGroupDropArea as true after grouping the column, GroupDropAreaItem is not added in GroupDropArea.
                //Hence, we initialize the panel childrens if GroupColumnDescriptions Count is greater than zero.
                this.InitializeGroupDropAreaPanel();
            }
            if (isGroupDropAreaExpandedSetBeforeGridLoaded)
                UpdateVisualState(true);
#if WPF
            this.ContextMenuOpening += OnContextMenuOpening;

            var bind = new Binding
            {
                Path = new PropertyPath("GroupDropAreaContextMenu"),
                Source = dataGrid,
                Mode = BindingMode.TwoWay,
            };
            this.SetBinding(GroupDropArea.ContextMenuProperty, bind);

#endif
        }

#if WPF
        #region ContenxtMenu Event
        internal void OnContextMenuOpening(Pointer sender, ContextMenuEventArgs e)
        {
            if (this.ContextMenu == null)
                return;

            var dataContext = new GridGroupDropAreaContextMenuInfo() { DataGrid = this.dataGrid };
            var args = new GridContextMenuEventArgs(dataGrid.GroupDropAreaContextMenu, dataContext, RowColumnIndex.Empty, ContextMenuType.GroupDropArea);
            this.ContextMenu.DataContext = dataContext;
            dataGrid.RaiseGridContextMenuEvent(args);
            e.Handled = args.Handled;
        }
        #endregion
#elif WP
        void GroupItemScroller_ManipulationStarted(Pointer sender, ManipulationStartedEventArgs e)
        {
            if (scrollersDisabled)
            {
                e.Handled = true;
                e.Complete();
            }
        }
#endif


        #endregion

        #region Private Methods

        private void ToggleExpanded(bool isExpanded)
        {
            this.dataGrid.IsGroupDropAreaExpanded = isExpanded;
            if (this.Panel != null)
            {
                UpdateVisualState(true);
                this.dataGrid.InvalidateMeasure();
            }
            else
                isGroupDropAreaExpandedSetBeforeGridLoaded = true;
        }

        private void UpdateVisualState(bool useTransitions)
        {
            VisualStateManager.GoToState(this, IsExpanded ? "Expanded" : "Collapsed", useTransitions);
        }

        private void InitializeGroupDropAreaPanel()
        {
            //Here we check the IRowGenerator.Items count to avoid the null reference exception while iterate the HeaderRow.
            if (this.dataGrid != null && this.dataGrid.View != null && this.dataGrid.View.GroupDescriptions.Count > 0 && this.dataGrid.RowGenerator.Items.Count > 0)
            {
                foreach (var desc in this.dataGrid.View.GroupDescriptions)
                {
                    var headerRow =
                        this.dataGrid.RowGenerator.Items.FirstOrDefault(item => item.RowRegion == RowRegion.Header && item.RowIndex == this.dataGrid.GetHeaderIndex());
                    if (headerRow != null)
                    {
                        var headerColumn =
                            headerRow.VisibleColumns.FirstOrDefault(
                                col => col.GridColumn != null && col.GridColumn.MappingName == (desc as PropertyGroupDescription).PropertyName);
                        var headerCellControl = headerColumn.ColumnElement as GridHeaderCellControl;
                        this.AddGroupAreaItem(headerColumn.GridColumn, headerCellControl.SortDirection);
                    }
                }
            }
        }

        #endregion

        

        #region Draggable popup for Group

        #region private methods



        private void OnGroupDropAreaItemTapped(GridColumn column, int ClickCount)
        {
            if (dataGrid.SortClickAction == SortClickAction.DoubleClick)
            {
                if (ClickCount == 2)
                    this.dataGrid.GridModel.MakeSort(column);
            }
            else
                this.dataGrid.GridModel.MakeSort(column);
        }

        private void OnGroupDropAreaItemRemoved(GridColumn column)
        {
            this.RemoveGroupDropAreaItem(column);
        }

        private UIElement CreatePopupContent(GridColumn column)
        {
            var textblock = new TextBlock { Text = column.HeaderText };
            return textblock;
        }

        private GroupDropAreaItem CreateGroupDropAreaItem(GridColumn column)
        {
            if (column.HeaderText == null)
                column.HeaderText = column.MappingName;
            string groupname = null;

            if (!column.IsUnbound)
            {

#if !WP
                if (this.dataGrid.View.IsDynamicBound)
                    groupname = this.GetDynamicColumnName(column);
#endif
            }
            
            var groupItem = new GroupDropAreaItem
            {
                GroupDropAreaItemTapped = OnGroupDropAreaItemTapped,
                GroupDropAreaItemRemoved = OnGroupDropAreaItemRemoved,
                GridColumn = column,
                GroupDropArea = this,
                Margin = new Thickness(5, 0, 5, 0),
#if WP
                MinWidth = 180
#else
                MinWidth = 120
#endif
            };
            if (groupname != null)
                groupItem.GroupName = groupname;
            else
            {
                var binding = new Binding() { Path = new PropertyPath("HeaderText"), Source = groupItem.GridColumn };
                groupItem.SetBinding(GroupDropAreaItem.GroupNameProperty, binding);
            }
            return groupItem;
        }
#if !WP
        private string GetDynamicColumnName(GridColumn column)
        {
            if (this.dataGrid.View.Records.Count > 0)
            {
                var dynObj = this.dataGrid.View.Records[0].Data as IDynamicMetaObjectProvider;
                if (dynObj != null)
                {

                    var metaType = dynObj.GetType();
                    var metaData =
                        dynObj.GetMetaObject(System.Linq.Expressions.Expression.Parameter(metaType, metaType.Name));

                    return metaData.GetDynamicMemberNames().FirstOrDefault(x => x == column.MappingName);
                }
            }
            return column.HeaderText;
        }
#endif


        internal void MoveGroupDropAreaItem(GridColumn column, ListSortDirection direction, int moveToIndex)
        {
            GroupDropAreaItem item = this.Panel.Children.ToList<GroupDropAreaItem>().FirstOrDefault(x => (x as GroupDropAreaItem).GridColumn == column) as GroupDropAreaItem;
            if (item != null)
            {
                var oldIndex = this.Panel.Children.IndexOf(item);
                this.Panel.Children.Remove(item);
                this.Panel.Children.Insert(moveToIndex, item);
#if !SILVERLIGHT && !WP7
                this.dataGrid.GroupColumnDescriptions.Move(oldIndex, moveToIndex);
#else
                this.dataGrid.GridModel.isGroupDescriptionMoved = true;
                this.dataGrid.GroupColumnDescriptions.MoveTo(oldIndex, moveToIndex);
                this.dataGrid.GridModel.isGroupDescriptionMoved = false;
#endif
            }
        }

#if (WinRT || WP) && !WP7
        internal async void AddGroupAreaItem(GridColumn column, object direction)
#else
        internal void AddGroupAreaItem(GridColumn column, object direction)
#endif
        {
#if (WinRT || WP) && !WP7
            this.dataGrid.SetBusyState("Busy");
            await Task.Delay(100);

#elif WPF  || WP7
            BackgroundWorker worker = new BackgroundWorker();
            worker.DoWork += (o, args) =>
                {
                    Thread.Sleep(50);
#if WPF
                    Dispatcher.Invoke(new Action(() =>
#else 
                    Dispatcher.BeginInvoke(new Action(() =>
#endif
                    {

#endif
                        AddGroupDropAreaItem(column, direction,true);
#if (WinRT || WP) && !WP7
                        this.dataGrid.SetBusyState("Normal");
#elif WPF || WP7
                    }));
                };
            worker.RunWorkerCompleted += (o, args) => { this.dataGrid.SetBusyState("Normal");};
            this.dataGrid.SetBusyState("Busy");
            worker.RunWorkerAsync();
#endif
        }

#if (WinRT || WP) && !WP7
        internal async void AddGroupAreaItem(GridColumn column, object direction, int insertAt)
#else
        internal void AddGroupAreaItem(GridColumn column, object direction, int insertAt)
#endif
        {
#if (WinRT || WP) && !WP7
            this.dataGrid.SetBusyState("Busy");
            await Task.Delay(100);
#elif WPF || WP7
            var worker = new BackgroundWorker();
            worker.DoWork += (o, args) =>
            {
                Thread.Sleep(50);
#if WPF
                Dispatcher.Invoke(new Action(() =>
#else 
                Dispatcher.BeginInvoke(new Action(() =>
#endif
                {

#endif
                    AddGroupDropAreaItem(column, direction, insertAt, true);
#if(WinRT || WP) && !WP7
                    this.dataGrid.SetBusyState("Normal");
#elif WPF || WP7

                }));
            };

            worker.RunWorkerCompleted += (o, args) => this.dataGrid.SetBusyState("Normal");
            this.dataGrid.SetBusyState("Busy");
            worker.RunWorkerAsync();
#endif
        }

        internal void AddGroupDropAreaItem(GridColumn column, object direction,bool needToGroup)
        {
            if (this.Panel != null && this.Panel.Children.ToList<GroupDropAreaItem>().All(item => (item as GroupDropAreaItem).GridColumn.MappingName != column.MappingName) && this.dataGrid.View != null)
            {
                if (column.MappingName == null)
                    throw new InvalidOperationException("MappingName is neccessary for Sorting, Grouping and Filtering");
                var content = this.CreateGroupDropAreaItem(column);
                content.SortDirection = direction;
                this.Panel.Children.Add(content);
                if (needToGroup)
                    this.dataGrid.GroupBy(column.MappingName, null);
                this.WatermarkTextVisibility = Visibility.Collapsed;
            }
            else
#if !WPF
                this.dataGrid.GridColumnDragDropController.reverseAnimationStoryboard.Begin();
#else
                this.dataGrid.GridColumnDragDropController.DraggablePopup.IsOpen = false;
#endif
        }
        
        internal void AddGroupDropAreaItem(GridColumn column, object direction, int insertAt,bool needToGroup)
        {
            if (this.Panel != null && this.Panel.Children.ToList<GroupDropAreaItem>().All(item => (item as GroupDropAreaItem).GridColumn.MappingName != column.MappingName) && this.dataGrid.View != null)
            {
                var content = this.CreateGroupDropAreaItem(column);
                content.SortDirection = direction;
                this.Panel.Children.Insert(insertAt, content);
                if (needToGroup)
                    this.dataGrid.GroupBy(column.MappingName, insertAt, null);
                this.WatermarkTextVisibility = Visibility.Collapsed;
            }
            else if(this.dataGrid.GridColumnDragDropController != null)
            {
                
#if !WPF
                this.dataGrid.GridColumnDragDropController.reverseAnimationStoryboard.Begin();
#else
                this.dataGrid.GridColumnDragDropController.DraggablePopup.IsOpen = false;
#endif
            }
        }

        internal void RemoveGroupDropAreaItem(GridColumn column)
        {
            if (this.Panel != null && this.Panel.Children.Count > 0)
            {
                var element = this.Panel.Children.ToList<GroupDropAreaItem>().FirstOrDefault(item => (item as GroupDropAreaItem).GridColumn.MappingName == column.MappingName);
                if (element != null)
                    this.Panel.Children.Remove(element);
                var groupedcolumn = this.dataGrid.GroupColumnDescriptions.FirstOrDefault(groupcolumn => groupcolumn.ColumnName == column.MappingName);
                if (groupedcolumn != null)
                    this.dataGrid.RemoveGroup(groupedcolumn.ColumnName);
                if (Panel.Children.Count == 0)
                    this.WatermarkTextVisibility = Visibility.Visible;
            }
        }

        internal void RemoveGroupDropItem(GridColumn column)
        {
            if (this.Panel != null && this.Panel.Children.Count > 0)
            {
                var element = this.Panel.Children.ToList<GroupDropAreaItem>().FirstOrDefault(item => (item as GroupDropAreaItem).GridColumn.MappingName == column.MappingName);
                if (element != null)
                    this.Panel.Children.Remove(element);
                if (Panel.Children.Count == 0)
                    this.WatermarkTextVisibility = Visibility.Visible;
            }
        }

        


        #endregion

        #region Internal Methods

        internal void RemoveAllGroupDropItems()
        {
            if (this.Panel != null && this.Panel.Children.Count > 0)
            {
                this.Panel.Children.Clear();
                this.WatermarkTextVisibility = Visibility.Visible;
            }
        }

        internal void UpdateGroupDropItemSortIcon(GridColumn column)
        {
            var sortedcolumn = this.dataGrid.View.SortDescriptions.FirstOrDefault(c => c.PropertyName == column.MappingName);

            if (this.Panel != null && this.Panel.Children.ToList<GroupDropAreaItem>().Any(item => (item as GroupDropAreaItem).GridColumn == column))
            {
                var groupDropAreaItem = this.Panel.Children.ToList<GroupDropAreaItem>().FirstOrDefault(item => (item as GroupDropAreaItem).GridColumn == column) as GroupDropAreaItem;
                if (groupDropAreaItem != null)
                {
                    if (sortedcolumn != default(SortDescription))
                        groupDropAreaItem.SortDirection = sortedcolumn.Direction;
                    else
                        groupDropAreaItem.SortDirection = null;
                }
            }
        }

        #endregion

        #endregion

        public void Dispose()
        {
#if WPF
            if (this.dataGrid.GroupDropAreaContextMenu != null)
            {
                this.ContextMenuOpening -= OnContextMenuOpening;
                this.ContextMenu = null;
            }
#endif
            if (this.Panel != null)
            {
                foreach (var item in this.Panel.Children)
                    (item as GroupDropAreaItem).Dispose();
                this.Panel = null;
            }
            this.dataGrid = null;
        }
    }
}



