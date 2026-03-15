#region Copyright Syncfusion Inc. 2001 - 2014
// Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Any infringement will be prosecuted under
// applicable laws. 
#endregion
using System;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Shapes;
using System.Text;
using Syncfusion.Windows.Controls.Gantt.Schedule;
using System.Collections.Generic;
using System.ComponentModel;
using System.Collections;
using Syncfusion.Windows.Controls.Gantt;
using System.Reflection;
using System.Collections.ObjectModel;
using System.Windows.Data;
using System.Collections.Specialized;
using System.Windows.Controls.Primitives;

namespace Syncfusion.Windows.Controls.Gantt.Chart
{
    /// <summary>
    /// Represents a control that will be displayed as a row in the Gantt Chart.
    /// </summary>
    [
    TemplatePart(Name = "PART_ResizingRect", Type = typeof(Rectangle)),
    TemplatePart(Name = "PART_BackgoundCanvas", Type = typeof(Canvas)),
    TemplatePart(Name = "PART_DateTimeResizingTooltip", Type = typeof(Popup)),
    TemplatePart(Name = "PART_NumericResizingTooltip", Type = typeof(Popup)),
    TemplatePart(Name = "PART_ProgressResizingTooltip", Type = typeof(Popup))
    ]
    public class GanttChartRow : ContentControl, IDisposable
    {
        #region Template Parts

        internal GanttChartRowItemsPresenter ItemsPresenter
        {
            get { return _itemsPresenter; }
            set
            {
                if (value == null)
                { }
                _itemsPresenter = value;
            }
        }

        internal Rectangle ResizingRect { get; set; }
        internal GanttChart ParentControl { get; set; }
        internal Canvas BackgoundCanvas { get; set; }
        internal Popup ResizingPopup { get; set; }
        internal Popup ProgressPopup { get; set; }

        #endregion

        #region Properties

        Dictionary<string, PropertyInfo> ItemProperties = new Dictionary<string, PropertyInfo>();
        bool isNodeRegenerated = false;
        private GanttChartRowItemsPresenter _itemsPresenter;

#if SILVERLIGHT
        bool isExpanded = false;
#endif

        /// <summary>
        /// Gets the model.
        /// </summary>
        /// <value>The model.</value>
        public GanttModel Model
        {
            get
            {
                return this.ParentControl != null ? this.ParentControl.Model : null;
            }
        }

        #endregion

        #region Constructors and overrides

        /// <summary>
        /// Initializes a new instance of the <see cref="GanttChartRow"/> class.
        /// </summary>
        public GanttChartRow()
        {
            DefaultStyleKey = GetType();

#if !SyncfusionFramework3_5
            UseLayoutRounding = false;
#endif
        }

        /// <summary>
        /// When overridden in a derived class, is invoked whenever application code or internal processes call <see cref="M:System.Windows.FrameworkElement.ApplyTemplate"/>.
        /// </summary>
        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();

#if !SILVERLIGHT
            if (this.DataContext != null)
            {
                GanttRecord record = this.DataContext as GanttRecord;
                if (record.DataItem != null)
                {
                    // To prevent repopulating child nodes which are already in view
                    if (record.IsExpanded && !record.IsRealized)
                    {
                        Model.SyncExpandState(record, record.IsExpanded);

                        // To prevent repopulating the child nodes which are already in view
                        record.IsRealized = true;
                    }
                }
            }
#endif
            ItemsPresenter = (GanttChartRowItemsPresenter)GetTemplateChild("ItemsPresenterElement");
            BackgoundCanvas = (Canvas)GetTemplateChild("PART_BackgoundCanvas");
            ResizingRect = (Rectangle)GetTemplateChild("PART_ResizingRect");
            ResizingPopup = this.ParentControl.ParentControl.ScheduleType != ScheduleType.CustomNumeric ? (Popup)GetTemplateChild("PART_DateTimeResizingTooltip") : (Popup)GetTemplateChild("PART_NumericResizingTooltip");
            ProgressPopup = (Popup)GetTemplateChild("PART_ProgressResizingTooltip");

            if (ItemsPresenter != null)
            {
                ItemsPresenter.ParentRow = this;
            }

            GenerateItems();

#if !SILVERLIGHT
            this.MouseDown -= GanttChartRow_MouseDown;
            this.MouseDown += GanttChartRow_MouseDown;

            this.ParentControl.ResourceNameVisibilityChanged -= ParentControl_ResourceNameVisibilityChanged;
            this.ParentControl.ResourceNameVisibilityChanged += ParentControl_ResourceNameVisibilityChanged;
#else
            this.MouseLeftButtonDown -= GanttChartRow_MouseDown;
            this.MouseLeftButtonDown += GanttChartRow_MouseDown;

            this.MouseRightButtonDown -= GanttChartRow_MouseDown;
            this.MouseRightButtonDown += GanttChartRow_MouseDown;
#endif
            this.LayoutUpdated -= GanttChartRow_LayoutUpdated;
            this.LayoutUpdated += GanttChartRow_LayoutUpdated;
        }

        void GanttChartRow_LayoutUpdated(object sender, EventArgs e)
        {
            if (this.isNodeRegenerated)
            {
                this.ParentControl.UpdateResizedNodeConnector(this);
                this.isNodeRegenerated = false;
            }

#if SILVERLIGHT
            if (!isExpanded && this.DataContext != null)
            {
                GanttRecord record = this.DataContext as GanttRecord;
                if (record.DataItem != null)
                {
                    // To prevent repopulating child nodes which are already in view
                    if (record.IsExpanded && !record.IsRealized)
                    {
                        Model.SyncExpandState(record, record.IsExpanded);

                        // To prevent repopulating the child nodes which are already in view
                        record.IsRealized = true;

                        isExpanded = true;
                    }
                }
            }
#endif
        }

        /// <summary>
        /// When overridden in a derived class, positions child elements and determines a size for a <see cref="T:System.Windows.FrameworkElement"/> derived class.
        /// </summary>
        /// <param name="finalSize">The final area within the parent that this element should use to arrange itself and its children.</param>
        /// <returns>The actual size used.</returns>
        protected override Size ArrangeOverride(Size finalSize)
        {
            ItemsPresenter.Arrange(new Rect(0, 0, finalSize.Width, finalSize.Height));
            return base.ArrangeOverride(finalSize);
        }

        /// <summary>
        /// When overridden in a derived class, measures the size in layout required for child elements and determines a size for the <see cref="T:System.Windows.FrameworkElement"/>-derived class.
        /// </summary>
        /// <param name="availableSize">The available size that this element can give to child elements. Infinity can be specified as a value to indicate that the element will size to whatever content is available.</param>
        /// <returns>
        /// The size that this element determines it needs during layout, based on its calculations of child element sizes.
        /// </returns>
        protected override Size MeasureOverride(Size availableSize)
        {
            ItemsPresenter.Measure(new Size(ParentControl.ActualWidth, availableSize.Height));
            return base.MeasureOverride(availableSize);
        }

        /// <summary>
        /// Generates the items.
        /// </summary>
        private void GenerateItems()
        {
            if (this.DataContext == null || this.DataContext == DependencyProperty.UnsetValue)
                return;

            GanttRecord currentRecord = this.DataContext as GanttRecord;
            var currentItem = currentRecord.DataItem;
            this.ItemProperties = new Dictionary<string, PropertyInfo>();

            currentItem.GetType().GetProperties().ToList().ForEach((p) =>
            {
                ItemProperties.Add(p.Name, p);
            });

            if (currentItem == null || currentItem == DependencyProperty.UnsetValue || !CanGenerateItems())
                return;

            // To check and draw inline items
            if (currentRecord.InLineRecords.Count > 0)
            {
                this.GenerateInlineItems(currentRecord);
                return;
            }

            // Temp variable initialization
            GanttNode node = null;
            bool isHeader = false;
            bool isMilStone = false;
            this.ItemProperties = new Dictionary<string, PropertyInfo>();

            currentItem.GetType().GetProperties().ToList().ForEach((p) =>
            {
                ItemProperties.Add(p.Name, p);
            });

            //If the TaskAttribute mapping contains the MileStoneMapping means "isMileStone" will be True
            if (this.Model.TaskAttributeMapping.HasMileStoneMapping)
            {
                isMilStone = (bool)this.ItemProperties[this.Model.TaskAttributeMapping.MileStoneMapping].GetValue(currentItem);
            }

            if (this.Model.TaskAttributeMapping.HasChildMapping)
            {
                var child = this.ItemProperties[this.Model.TaskAttributeMapping.ChildMapping].GetValue(currentItem);
                isHeader = child != null && this.Model.GetObservableCollection(child as IEnumerable).Count > 0;
            }

            // Node Creation for DateTime Schedule.
            if (this.ParentControl.ParentControl.ScheduleType != ScheduleType.CustomNumeric)
            {
                DateTime startDate = (DateTime)this.ItemProperties[this.Model.TaskAttributeMapping.StartDateMapping].GetValue(currentItem);
                DateTime endDate = (DateTime)this.ItemProperties[this.Model.TaskAttributeMapping.FinishDateMapping].GetValue(currentItem);

                // This need to be changed when we move to hour basis calculation
                if (!isMilStone && !isHeader)
                {
                    if ((endDate - startDate).TotalDays <= 0)
                        isMilStone = true;
                }

                if (isMilStone && !isHeader)
                {
                   node = new MileStone();

                    //Here we storing the EndDate vale in one variable this will be used when uncheck the IsMilStone column Cell
                    //This temprory value will be strored to EndDate to retain the GanttNode from MileStone 
                    if (endDate.CompareTo(startDate) > 0)
                    {
                        node.OldEndTime = endDate;
                    }
                    // This is to ensure that end date should not be minimum of start date so we are enforcing the start date to end date.
                    this.ItemProperties[this.Model.TaskAttributeMapping.FinishDateMapping].SetValue(currentItem, startDate);

                    if (this.Model.TaskAttributeMapping.HasMileStoneMapping)
                        this.ItemProperties[this.Model.TaskAttributeMapping.MileStoneMapping].SetValue(currentItem, isMilStone);
                }
                else if (isHeader)
                {
                    node = new HeaderNode();
                }
                else
                {
                    node = new GanttNode();
                }
            }

            // Node Creation for Numeric Schedule.
            else
            {
                double start = (double)this.ItemProperties[this.Model.TaskAttributeMapping.StartPointMapping].GetValue(currentItem);
                double end = (double)this.ItemProperties[this.Model.TaskAttributeMapping.FinishPointMapping].GetValue(currentItem);

                if (!isMilStone)
                {
                    if ((end - start) <= 0)
                        isMilStone = true;
                }

                if (isHeader && !isMilStone)
                {
                    node = new HeaderNode();
                }

                else if (isMilStone)
                {
                    node = new MileStone();

                    //Here we storing the EndPoint vale in one variable this will be used when uncheck the IsMilStone column Cell
                    //This temprory value will be strored to EndPoint to retain the GanttNode from MileStone 
                    if (end != start)
                        node.OldEndPoint = end;
                    // This is to ensure that end date should not be minimum of start point so we are enforcing the start point to end point.
                    this.ItemProperties[this.Model.TaskAttributeMapping.FinishPointMapping].SetValue(currentItem, start);
                    if (this.Model.TaskAttributeMapping.HasMileStoneMapping)
                        this.ItemProperties[this.Model.TaskAttributeMapping.MileStoneMapping].SetValue(currentItem, isMilStone);
                }
                else
                {
                    node = new GanttNode();
                }
            }

            // checking for the existance of custom tooltip and applying the internal tooltip
            if (this.ParentControl.ToolTipTemplate != null)
            {
                node.ToolTipTemplate = this.ParentControl.ToolTipTemplate;
            }
            else
            {
                // Get the toop tip with current mapping attributes
                node.ToolTipTemplate = this.ParentControl.InBuiltTooltipTemplate;
            }
            node.DataContext = currentItem;
            node.ParentRow = this;

            if (this.IsInHighlightedItems(currentItem))
                node.IsInHighlightedItems = true;

            this.ParentControl.ParentControl.RaiseNodeCreated(new NodeCreatedEventArgs()
            {
                Node = node,
                CurrrentDataItem = currentItem
#if !SILVERLIGHT
,
                RoutedEvent = GanttControl.NodeCreatedEvent
#endif
            });

            ItemsPresenter.Children.Add(node);
            // Generate the resource info text boxes
            GenerateResourceInfo(currentItem);
        }

        /// <summary>
        /// Generates the inline items.
        /// </summary>
        /// <param name="currentItem">The current item.</param>
        private void GenerateInlineItems(GanttRecord currentItem)
        {
            // Check of inline properties
            if (this.ItemProperties != null && this.ItemProperties.Count <= 0)
                return;

            // To get the current item/task properties
            //Dictionary<string, PropertyInfo> inLineProperties = new Dictionary<string, PropertyInfo>();
            //currentItem.GetType().GetProperties().ToList().ForEach((p) =>
            //{
            //    inLineProperties.Add(p.Name, p);
            //});

            //if (inLineProperties == null && inLineProperties.Count <= 0)
            //    return;

            //var inLineItems = inLineProperties[this.Model.TaskAttributeMapping.InLineTaskMapping].GetValue(currentItem);

            // Iterating inline items ot generate nodes
            foreach (GanttRecord inLineitem in currentItem.InLineRecords)
            {
                CreateInLineItem(inLineitem.DataItem);
            }
        }

        /// <summary>
        /// Creates the in line item.
        /// </summary>
        /// <param name="inLineitem">The in lineitem.</param>
        private void CreateInLineItem(object inLineitem)
        {
            GanttNode node = null;
            bool isMileStone = false;


            //If the TaskAttribute mapping contains the MileStoneMapping means "isMileStone" will be True
            if (this.Model.TaskAttributeMapping.HasMileStoneMapping)
            {
                isMileStone = (bool)this.Model.ChildItemProperties[this.Model.TaskAttributeMapping.MileStoneMapping].GetValue(inLineitem);
            }

            if (this.ParentControl.ParentControl.ScheduleType != ScheduleType.CustomNumeric)
            {
                DateTime startDate = (DateTime)this.Model.ChildItemProperties[this.Model.TaskAttributeMapping.StartDateMapping].GetValue(inLineitem);
                DateTime endDate = (DateTime)this.Model.ChildItemProperties[this.Model.TaskAttributeMapping.FinishDateMapping].GetValue(inLineitem);

                // Based on hour basis calculation
                if (!isMileStone)
                    isMileStone = (endDate - startDate).TotalDays <= 0;

                if (isMileStone)
                {
                    node = new MileStone();

                    // This is to ensure that end date should not be minimum of start date so we are enforcing the start date to end date.
                    this.ItemProperties[this.Model.TaskAttributeMapping.FinishDateMapping].SetValue(inLineitem, startDate);
                }
                else
                {
                    node = new GanttNode();
                }
            }
            else
            {
                double start = (double)this.Model.ChildItemProperties[this.Model.TaskAttributeMapping.StartPointMapping].GetValue(inLineitem);
                double end = (double)this.Model.ChildItemProperties[this.Model.TaskAttributeMapping.FinishPointMapping].GetValue(inLineitem);

                if (!isMileStone)
                    isMileStone = (end - start) <= 0;

                if (isMileStone)
                {
                    node = new MileStone();

                    // This is to ensure that end date should not be minimum of start point so we are enforcing the start point to end point.
                    this.ItemProperties[this.Model.TaskAttributeMapping.FinishPointMapping].SetValue(inLineitem, start);
                }
                else
                {
                    node = new GanttNode();
                }
            }

            node.ToolTipTemplate = this.ParentControl.ToolTipTemplate;
            node.DataContext = inLineitem;
            node.ParentRow = this;

            if (this.IsInHighlightedItems(inLineitem))
                node.IsInHighlightedItems = true;

            this.ParentControl.ParentControl.RaiseNodeCreated(new NodeCreatedEventArgs()
            {
                Node = node,
                CurrrentDataItem = inLineitem
#if !SILVERLIGHT
,
                RoutedEvent = GanttControl.NodeCreatedEvent
#endif
            });

            ItemsPresenter.Children.Add(node);
            GenerateResourceInfo(inLineitem);
        }

        /// <summary>
        /// Determines whether this instance [can generate items].
        /// </summary>
        /// <returns>
        /// 	<c>true</c> if this instance [can generate items]; otherwise, <c>false</c>.
        /// </returns>
        private bool CanGenerateItems()
        {
            if (this.ParentControl.ParentControl.ScheduleType == ScheduleType.CustomNumeric)
            {
                // Checking for the existance of start point and finish point mapping
                if (string.IsNullOrEmpty(this.ParentControl.Model.TaskAttributeMapping.StartPointMapping) ||
                    string.IsNullOrEmpty(this.ParentControl.Model.TaskAttributeMapping.FinishPointMapping))
                    return false;
            }
            else
            {
                // Checking for the existance of start date and finish date mapping
                if (string.IsNullOrEmpty(this.ParentControl.Model.TaskAttributeMapping.StartDateMapping) ||
                   string.IsNullOrEmpty(this.ParentControl.Model.TaskAttributeMapping.FinishDateMapping))
                    return false;
            }

            return true;
        }


        /// <summary>
        /// Checks for in line items.
        /// </summary>
        /// <param name="currentItem">The current item.</param>
        /// <returns></returns>
        private bool CheckForInLineItems(object currentItem)
        {
            // To get the properties of current Item/ task
            Dictionary<string, PropertyInfo> inLineProperties = new Dictionary<string, PropertyInfo>();

            currentItem.GetType().GetProperties().ToList().ForEach((p) =>
            {
                inLineProperties.Add(p.Name, p);
            });

            if (inLineProperties == null && inLineProperties.Count <= 0)
                return false;

            var inLineItems = inLineProperties[this.Model.TaskAttributeMapping.InLineTaskMapping].GetValue(currentItem);
            if (inLineItems is IEnumerable)
            {
                foreach (object inLineitem in (inLineItems as IEnumerable))
                {
                    // To get the item properties of inline item
                    if (this.ItemProperties.Count == 0)
                    {
                        inLineitem.GetType().GetProperties().ToList().ForEach((p) =>
                        {
                            // if(!this.ItemProperties.ContainsKey(p.Name))
                            this.ItemProperties.Add(p.Name, p);
                        });
                    }
                    return true;
                }
            }
            return false;
        }

        /// <summary>
        /// Called when [in line collection changed].
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The <see cref="System.Collections.Specialized.NotifyCollectionChangedEventArgs"/> instance containing the event data.</param>
        internal void OnInLineCollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            if (e.Action == NotifyCollectionChangedAction.Add)
            {
                // Adding new inline items
                foreach (object inLineitem in (e.NewItems as IEnumerable))
                {
                    CreateInLineItem(inLineitem);
                }
            }
            else if (e.Action == NotifyCollectionChangedAction.Remove)
            {
                // Removing specific inline items
                foreach (object inLineitem in (e.OldItems as IEnumerable))
                {
                    for (int i = 0; i < this.ItemsPresenter.Children.Count; i++)
                    {
                        UIElement uie = this.ItemsPresenter.Children[i];
                        if (uie is GanttNode && (uie as GanttNode).DataContext == inLineitem)
                        {
                            this.ItemsPresenter.Children.Remove(uie);
                            break;
                        }
                    }
                }
            }
            else if (e.Action == NotifyCollectionChangedAction.Reset)
            {
                // Removing all inline items
                for (int i = 0; i < this.ItemsPresenter.Children.Count; i++)
                {
                    if (this.ItemsPresenter.Children[i] is GanttNode)
                    {
                        this.ItemsPresenter.Children.Remove(this.ItemsPresenter.Children[i]);
                    }
                }
            }
        }

        /// <summary>
        /// Generates the resource info.
        /// </summary>
        /// <param name="currentItem">The current item.</param>
        private void GenerateResourceInfo(object currentItem)
        {
            ContentControl resourceContainer = new ContentControl();
            string resourceNames = string.Empty;
            var parent = this.ParentControl.ParentControl as GanttControl;
            DataTemplate resourceTemplate = parent.ResourceContainerTemplate;
            ResourceContainerCreatedEventArgs args;
            string mappingName = this.Model.TaskAttributeMapping.ResourceInfoMapping;

            if (currentItem == null || string.IsNullOrEmpty(mappingName) || this.ItemProperties[mappingName] == null)
                return;

            var resources = this.ItemProperties[mappingName].GetValue(currentItem);

            if (!(resources is IEnumerable<Resource>))
                return;

            if (resources is INotifyCollectionChanged)
            {
                var resourceCollection = resources as INotifyCollectionChanged;

                resourceCollection.CollectionChanged -= ResourceCollection_CollectionChanged;
                resourceCollection.CollectionChanged += ResourceCollection_CollectionChanged;
            }

            foreach (Resource res in (resources as IEnumerable<Resource>))
            {
                resourceNames += resourceNames.Length > 0 ? ", " + res.Name : res.Name;
            }
            if (resourceNames.Length > 0)
            {
                // resourceContainer.Name = "ResourceContainer";
                resourceContainer.Content = resourceNames;
                resourceContainer.IsHitTestVisible = false;
                resourceContainer.Visibility = this.ParentControl.ResourceNameVisibility;
                resourceContainer.HorizontalContentAlignment = System.Windows.HorizontalAlignment.Stretch;
                resourceContainer.VerticalContentAlignment = System.Windows.VerticalAlignment.Stretch;
                resourceContainer.DataContext = parent.Model.Resources;
#if !SILVERLIGHT
                if (parent.ResourceContainerTemplateSelector != null)
                    resourceTemplate = parent.ResourceContainerTemplateSelector.SelectTemplate(resourceContainer, this.ParentControl);
#endif
                if (resourceTemplate != null)
                    resourceContainer.ContentTemplate = resourceTemplate;

                args = new ResourceContainerCreatedEventArgs()
                {
                    CurrentResource = resourceContainer
#if !SILVERLIGHT
,
                    RoutedEvent = GanttControl.ResourceContainerCreatedEvent
#endif
                };
                this.ParentControl.ParentControl.RaiseResourceCreated(args);

                ItemsPresenter.Children.Add(resourceContainer);
            }
        }

        /// <summary>
        /// Handles the CollectionChanged event of the resourceCollection control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.Collections.Specialized.NotifyCollectionChangedEventArgs"/> instance containing the event data.</param>
        void ResourceCollection_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            if (this.DataContext == null || (this.DataContext as GanttRecord) == null || (this.DataContext as GanttRecord).DataItem == null)
                return;

            //if (e.Action == NotifyCollectionChangedAction.Add)
            //{
            //    foreach (Resource res in e.NewItems)
            //    {
            //        if (!this.ParentControl.Model.Resources.Contains(res))
            //            return;
            //    }
            //}

            if (e.Action != NotifyCollectionChangedAction.Replace)
            {
                this.RecreateResourceInfo();

                // Since it is only the collection change Grid will not listen to it, so to refresh the cell we need to invalidate it.
                this.ParentControl.Model.InvalidateGrid((this.DataContext as GanttRecord).DataItem);
            }
        }

        /// <summary>
        /// Recreates the resource info.
        /// </summary>
        void RecreateResourceInfo()
        {
            if (this.ItemsPresenter == null)
                return;

            ContentControl resourceName = null;
            while (this.ItemsPresenter.FindName("ResourceContainer") as ContentControl != null)
            {
                resourceName = this.ItemsPresenter.FindName("ResourceContainer") as ContentControl;
                this.ItemsPresenter.Children.Remove(resourceName);
            }
            this.GenerateResourceInfo((this.DataContext as GanttRecord).DataItem);
        }

        /// <summary>
        /// Handles the MouseDown event of the GanttChartRow control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.Windows.Input.MouseButtonEventArgs"/> instance containing the event data.</param>
        void GanttChartRow_MouseDown(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            this.ParentControl.UpdateSlectedItem(this);
        }

        /// <summary>
        /// Handles the ResourceNameVisibilityChanged event of the ParentControl control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.Windows.DependencyPropertyChangedEventArgs"/> instance containing the event data.</param>
        void ParentControl_ResourceNameVisibilityChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            if (this.ItemsPresenter == null)
                return;

            // var textBlocks = this.ItemsPresenter.FindElementsOfType<TextBlock>();

            // To enforce the visibility of resource name text box.
            foreach (UIElement uie in this.ItemsPresenter.Children)
            {
                if (uie is ContentControl)
                    (uie as ContentControl).Visibility = (System.Windows.Visibility)e.NewValue;
            }
        }

        /// <summary>
        /// Determines whether [is in highlighted items] [the specified item].
        /// </summary>
        /// <param name="item">The item.</param>
        /// <returns>
        /// 	<c>true</c> if [is in highlighted items] [the specified item]; otherwise, <c>false</c>.
        /// </returns>
        internal bool IsInHighlightedItems(object item)
        {
            if (this.Model.HighlightedItems != null && this.Model.HighlightedItems.Count > 0)
                return this.Model.HighlightedItems.Contains(item);

            return false;
        }

        #endregion

        #region Internal/Public functions

        /// <summary>
        /// Invalidates this instance.
        /// </summary>
        internal void Invalidate()
        {
            if (ItemsPresenter == null)
                return;

            ItemsPresenter.InvalidateArrange();
            ItemsPresenter.InvalidateMeasure();
        }

        /// <summary>
        /// Regenerates the items.
        /// </summary>
        internal void RegenerateItems()
        {
            if (ItemsPresenter == null)
                return;
            foreach (var item in ItemsPresenter.Children)
            {
                if (item is IDisposable)
                    (item as IDisposable).Dispose();
            }
            ItemsPresenter.Children.Clear();
            this.GenerateItems();

            this.isNodeRegenerated = true;

            this.ItemsPresenter.InvalidateMeasure();
            this.ItemsPresenter.InvalidateArrange();
        }


        /// <summary>
        /// Brinds the drag rect to position.
        /// </summary>
        internal void BringResizingRectToPosition(FrameworkElement node)
        {
            //FrameworkElement node = this.ItemsPresenter.Children[0] as FrameworkElement;

#if !SILVERLIGHT
            Point pos = node.TranslatePoint(new Point(0, 0), this);
#else
            GeneralTransform objGeneralTransform = node.TransformToVisual(this as UIElement);
            Point pos = objGeneralTransform.Transform(new Point(0, 0));
            ResizingRect.Height = this.ActualHeight;
#endif
            Canvas.SetLeft(ResizingRect, pos.X);
            Canvas.SetTop(ResizingRect, pos.Y);
            ResizingRect.Width = node.ActualWidth;

            // To rearrange the rect based on new position
            this.BackgoundCanvas.InvalidateArrange();

            ResizingRect.Visibility = Visibility.Visible;
        }

        #endregion

        public void Dispose()
        {
#if !SILVERLIGHT
            this.MouseDown -= GanttChartRow_MouseDown;
            this.ParentControl.ResourceNameVisibilityChanged -= ParentControl_ResourceNameVisibilityChanged;
#else
            this.MouseLeftButtonDown -= GanttChartRow_MouseDown;
            this.MouseRightButtonDown -= GanttChartRow_MouseDown;
#endif
            this.LayoutUpdated -= GanttChartRow_LayoutUpdated;


            if (this.DataContext != null && this.DataContext is GanttRecord)
            {
                var currentItem = (this.DataContext as GanttRecord).DataItem;
                if (currentItem != null && this.ItemProperties != null)
                {
                    string mappingName = this.Model.TaskAttributeMapping.ResourceInfoMapping;
                    if (this.ItemProperties.ContainsKey(mappingName))
                    {
                        var resources = this.ItemProperties[mappingName].GetValue(currentItem);

                        if (!(resources is IEnumerable<Resource>))
                            return;

                        if (resources is INotifyCollectionChanged)
                        {
                            var resourceCollection = resources as INotifyCollectionChanged;
                            resourceCollection.CollectionChanged -= ResourceCollection_CollectionChanged;
                        }
                    }
                }
            }
            this.BackgoundCanvas = null;
            this.DataContext = null;
            if (this.ItemsPresenter != null)
            {
                foreach (var ganttNode in this.ItemsPresenter.Children)
                {
                    if (ganttNode is IDisposable)
                        (ganttNode as IDisposable).Dispose();
                }
                this.ItemsPresenter.Children.Clear();
            }
            this.ItemsPresenter = null;
            this.ParentControl = null;
        }
    }
}
