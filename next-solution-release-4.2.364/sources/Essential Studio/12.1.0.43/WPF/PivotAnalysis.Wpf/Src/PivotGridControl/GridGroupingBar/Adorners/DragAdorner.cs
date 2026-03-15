#region Copyright Syncfusion Inc. 2001 - 2014
// Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Any infringement will be prosecuted under
// applicable laws. 
#endregion
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Documents;
using System.Windows;
using Syncfusion.PivotAnalysis.Base;
using Syncfusion.Windows.Controls.PivotSchemaDesigner;

namespace Syncfusion.Windows.Controls.PivotGrid
{
    /// <summary>
    /// DragIndicatorAdorner class to create an Adorner Layer while performing drag 'n' drop operation.
    /// </summary>
    
#if SyncfusionFramework4_0 
    [System.ComponentModel.DesignTimeVisible(false)]
#endif
    public class DragIndicatorAdorner : Adorner
    {
        #region [ Public Properties ]
        /// <summary>
        /// Gets or sets the Visual children
        /// </summary>
        public VisualCollection VisualChildren { get; set; }
        /// <summary>
        /// Gets or sets the value X to create a Rect instance
        /// </summary>
        public double X { get; set; }
        /// <summary>
        /// Gets or sets the value Y to create a Rect instance
        /// </summary>
        public double Y { get; set; }
        /// <summary>
        /// Gets or sets the instance of PivotGrid Control
        /// </summary>
        public PivotGridControl PivotGrid { get; set; }
        /// <summary>
        /// Gets or sets the instance of StackPanel
        /// </summary>
        public StackPanel StackPanel { get; set; }
        /// <summary>
        /// Gets or sets the text of Items
        /// </summary>
        public string ItemText { get; set; }

        #endregion

        #region [ Initialize/Finalize ]
        /// <summary>
        /// Initializes the <see cref="DragIndicatorAdorner"/> class.
        /// </summary>
        /// <param name="adornedElement">The UI Element</param>
        /// <param name="x">double</param>
        /// <param name="y">double</param>
        /// <param name="ItemText">string</param>
        /// <param name="Item">object</param>
        public DragIndicatorAdorner(UIElement adornedElement, double x, double y, string ItemText, object Item)
            : base(adornedElement)
        {
            VisualChildren = new VisualCollection(this);
            PivotGrid = adornedElement as PivotGridControl;


            StackPanel = new StackPanel();
            StackPanel.Background = Brushes.Transparent;
            StackPanel.Orientation = Orientation.Horizontal;
            this.ItemText = ItemText;
            X = x;
            Y = y;
            VisualChildren.Add(StackPanel);
            this.AllowDrop = true;

            DragIndicatorButton button = new DragIndicatorButton(this.PivotGrid.GroupingBar, ItemText, Item, false);

            ResourceDictionary resource = new ResourceDictionary()
            {
                Source = new Uri("/Syncfusion.PivotAnalysis.Wpf;component/PivotGridControl/Themes/Generic.xaml", UriKind.RelativeOrAbsolute)
            };

            button.Style = resource["DragIndicatorStyle"] as Style;
            button.Height = 24;
            button.VerticalAlignment = VerticalAlignment.Bottom;
            button.Margin = new Thickness(1, 0, 0, 1);
            StackPanel.Children.Add(button);
        }

        #endregion

        #region [ Overrides ]

        /// <summary>
        /// Gets the number of visual child elements within this element.
        /// </summary>
        /// <value></value>
        /// <returns>The number of visual child elements for this element.</returns>
        protected override int VisualChildrenCount
        {
            get
            {
                return VisualChildren.Count;
            }
        }

        /// <summary>
        /// Overrides <see cref="M:System.Windows.Media.Visual.GetVisualChild(System.Int32)"/>, and returns a child at the specified index from a collection of child elements.
        /// </summary>
        /// <param name="index">The zero-based index of the requested child element in the collection.</param>
        /// <returns>
        /// The requested child element. This should not return null; if the provided index is out of range, an exception is thrown.
        /// </returns>
        protected override Visual GetVisualChild(int index)
        {
            return VisualChildren[index];
        }

        /// <summary>
        /// When overridden in a derived class, positions child elements and determines a size for a <see cref="T:System.Windows.FrameworkElement"/> derived class.
        /// </summary>
        /// <param name="finalSize">The final area within the parent that this element should use to arrange itself and its children.</param>
        /// <returns>The actual size used.</returns>
        protected override Size ArrangeOverride(Size finalSize)
        {
            double width = Common.GetTextSize(ItemText).Width + 50;
            double height = 24;
            StackPanel.Arrange(new Rect(X, Y, width, height));
            return finalSize;
        }

        #endregion
    }
    /// <summary>
    /// FieldListAdorner class to create an Adorner Layer when perform any operations in PivotGridFieldListWindow.
    /// </summary>
    
#if SyncfusionFramework4_0 
    [System.ComponentModel.DesignTimeVisible(false)]
#endif
    public class FieldListAdorner : Adorner
    {
        #region [ Public Properties ]
        /// <summary>
        /// Gets or sets the Visual children
        /// </summary>
        public VisualCollection VisualChildren { get; set; }
        /// <summary>
        /// Gets or sets the field list
        /// </summary>
        public PivotGridFieldList FieldList { get; set; }
        /// <summary>
        /// Gets or sets the Pivot item panel
        /// </summary>
        public PivotGroupingItemsControl PivotItemPanel { get; set; }
        /// <summary>
        /// Gets or sets the value X to create a Rect instance
        /// </summary>
        public double X { get; set; }
        /// <summary>
        ///  Gets or sets the value Y to create a Rect instance
        /// </summary>
        public double Y { get; set; }
        /// <summary>
        /// Gets or sets the StackPanel instance
        /// </summary>
        public StackPanel StackPanel { get; set; }
        /// <summary>
        /// Gets or sets the item's text
        /// </summary>
        public string ItemText { get; set; }

        #endregion

        #region [ Initialize/Finalize ]
        /// <summary>
        /// Initializes the <see cref="FieldListAdorner"/> class.
        /// </summary>
        /// <param name="adornedElement">The adorner element</param>
        /// <param name="x">double</param>
        /// <param name="y">double</param>
        /// <param name="ItemText">The item text</param>
        /// <param name="fieldList">The PivotGrid field list</param>
        /// <param name="Item">The PivotItem</param>
         public FieldListAdorner(UIElement adornedElement, double x, double y, string ItemText, PivotGridFieldList fieldList, PivotItem Item)
            : base(adornedElement)
        {
            VisualChildren = new VisualCollection(this);
            PivotItemPanel = adornedElement as PivotGroupingItemsControl;
            FieldList = fieldList;
            StackPanel = new StackPanel();
            StackPanel.Background = Brushes.Transparent;
            StackPanel.Orientation = Orientation.Horizontal;
            this.ItemText = ItemText;
            X = x;
            Y = y;
            VisualChildren.Add(StackPanel);
            this.AllowDrop = true;

            if (ItemText == null)
            {
                ItemText = this.FieldList.DragPivotItem.FieldHeader;
                Item = this.FieldList.DragPivotItem;
            }
            DragIndicatorButton button = new DragIndicatorButton(this.FieldList.GridControl.GroupingBar, ItemText, Item, this.FieldList.IsFieldList);

            ResourceDictionary resource = new ResourceDictionary()
            {
                Source = new Uri("/Syncfusion.PivotAnalysis.Wpf;component/PivotGridControl/Themes/Generic.xaml", UriKind.RelativeOrAbsolute)
            };

            button.Style = resource["DragIndicatorStyle"] as Style;
            button.Height = 24;
            button.VerticalAlignment = VerticalAlignment.Bottom;
            button.Margin = new Thickness(1, 0, 0, 1);

            StackPanel.Children.Add(button);
            this.PreviewDragOver += new DragEventHandler(FieldListAdorner_PreviewDragOver);
            this.PreviewDragLeave += new DragEventHandler(FieldListAdorner_PreviewDragLeave);
            this.PreviewDrop += new DragEventHandler(FieldListAdorner_PreviewDrop);
        }

        #endregion

        #region [ Events ]

        void FieldListAdorner_PreviewDrop(object sender, DragEventArgs e)
        {
            if (e.Data.GetDataPresent(typeof(PivotItem)))
            {
                this.FieldList.DroppedItem = e.Data.GetData(typeof(PivotItem)) as PivotItem;
            }
            else if (e.Data.GetDataPresent(typeof(FilterItemsCollection)))
            {
                this.FieldList.DroppedItem = e.Data.GetData(typeof(FilterItemsCollection)) as FilterItemsCollection;
            }
            else if (e.Data.GetDataPresent(typeof(PivotComputationInfo)))
            {
                PivotComputationInfo compInfo = e.Data.GetData(typeof(PivotComputationInfo)) as PivotComputationInfo;
                this.FieldList.DroppedItem = compInfo;
            }

            RaiseEvent(new RoutedEventArgs(PivotGridFieldList.GenericDrop));
        }

        void FieldListAdorner_PreviewDragLeave(object sender, DragEventArgs e)
        {
            Console.WriteLine("Adorner Drag Leave");
        }

        void FieldListAdorner_PreviewDragOver(object sender, DragEventArgs e)
        {
            Console.WriteLine("Adorner Drag Over");
            RaiseEvent(new RoutedEventArgs(PivotGridFieldList.GenericDragOver));
        }

        #endregion

        #region [ Overrides ]

        /// <summary>
        /// Gets the number of visual child elements within this element.
        /// </summary>
        /// <value></value>
        /// <returns>The number of visual child elements for this element.</returns>
        protected override int VisualChildrenCount
        {
            get
            {
                return VisualChildren.Count;
            }
        }

        /// <summary>
        /// Overrides <see cref="M:System.Windows.Media.Visual.GetVisualChild(System.Int32)"/>, and returns a child at the specified index from a collection of child elements.
        /// </summary>
        /// <param name="index">The zero-based index of the requested child element in the collection.</param>
        /// <returns>
        /// The requested child element. This should not return null; if the provided index is out of range, an exception is thrown.
        /// </returns>
        protected override Visual GetVisualChild(int index)
        {
            return VisualChildren[index];
        }

        /// <summary>
        /// When overridden in a derived class, positions child elements and determines a size for a <see cref="T:System.Windows.FrameworkElement"/> derived class.
        /// </summary>
        /// <param name="finalSize">The final area within the parent that this element should use to arrange itself and its children.</param>
        /// <returns>The actual size used.</returns>
        protected override Size ArrangeOverride(Size finalSize)
        {
            double width = Common.GetTextSize(ItemText).Width + 150;
            double height = 24;
            StackPanel.Arrange(new Rect(X, Y, width, height));
            return finalSize;
        }

        #endregion
    }

    /// <summary>
    /// ComputationListAdorner class to create an Adorner Layer when perform any operations in PivotComputationListWindow.
    /// </summary>
#if SyncfusionFramework4_0 
    [System.ComponentModel.DesignTimeVisible(false)]
#endif
    public class ComputationListAdorner : Adorner
    {
        #region [ Public Properties ]
        /// <summary>
        /// Gets or sets the visual children
        /// </summary>
        public VisualCollection VisualChildren { get; set; }
        /// <summary>
        /// Gets or sets the Computation list
        /// </summary>
        public PivotGridComputationList ComputationList { get; set; }
        /// <summary>
        /// Gets or sets the PivotItem's ComputationPanel
        /// </summary>
        public PivotGroupingItemsControl PivotItemComputationPanel { get; set; }
        /// <summary>
        /// Gets or sets the value X to create a Rect instance
        /// </summary>
        public double X { get; set; }
        /// <summary>
        /// Gets or sets the value Y to create a Rect instance
        /// </summary>
        public double Y { get; set; }
        /// <summary>
        /// Gets or sets the instance of StackPanel
        /// </summary>
        public StackPanel StackPanel { get; set; }
        /// <summary>
        /// Gets or sets the text of Items
        /// </summary>
        public string ItemText { get; set; }

        #endregion

        #region [ Initialize/Finalize ]

        /// <summary>
        /// Initializes a new instance of the <see cref="ComputationListAdorner"/> class.
        /// </summary>   
        /// <param name="adornedElement">PivotGroupingItemsControl(PivotItemComputationPanel).</param>
        /// <param name="x">X Position of the dragged item</param>
        /// <param name="y">Y Position of the dragged item</param>
        /// <param name="ItemText">FieldHeader of the dragged item</param>
        /// <param name="computationList">PivotGridComputationList Window with List of items in ComputationArea</param>
        /// <param name="Item">Dragged Item</param>
        public ComputationListAdorner(UIElement adornedElement, double x, double y, string ItemText, PivotGridComputationList computationList, PivotComputationInfo Item)
            : base(adornedElement)
        {
            VisualChildren = new VisualCollection(this);
            PivotItemComputationPanel = adornedElement as PivotGroupingItemsControl;
            ComputationList = computationList;
            StackPanel = new StackPanel();
            StackPanel.Background = Brushes.Transparent;
            StackPanel.Orientation = Orientation.Horizontal;
            this.ItemText = ItemText;
            X = x;
            Y = y;
            VisualChildren.Add(StackPanel);
            this.AllowDrop = true;

            if (ItemText == null)
            {
                ItemText = this.ComputationList.DragPivotItem.FieldHeader;
                Item = this.ComputationList.DragPivotItem;
            }
            DragIndicatorButton button = new DragIndicatorButton(this.ComputationList.GridControl.GroupingBar, ItemText, Item, this.ComputationList.IsComputationList);

            ResourceDictionary resource = new ResourceDictionary()
            {
                Source = new Uri("/Syncfusion.PivotAnalysis.Wpf;component/PivotGridControl/Themes/Generic.xaml", UriKind.RelativeOrAbsolute)
            };

            button.Style = resource["DragIndicatorStyle"] as Style;
            button.Height = 24;
            button.VerticalAlignment = VerticalAlignment.Bottom;
            button.Margin = new Thickness(1, 0, 0, 1);

            StackPanel.Children.Add(button);
            this.PreviewDragOver += new DragEventHandler(ComputationListAdorner_PreviewDragOver);
            this.PreviewDragLeave += new DragEventHandler(ComputationListAdorner_PreviewDragLeave);
            this.PreviewDrop += new DragEventHandler(ComputationListAdorner_PreviewDrop);
        }

        #endregion

        #region [ Events ]

        /// <summary>
        /// Handles the PreviewDrop event of the ComputationListAdorner.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.Windows.DragEventArgs"/> instance containing the event data.</param>       
        void ComputationListAdorner_PreviewDrop(object sender, DragEventArgs e)
        {
            if (e.Data.GetDataPresent(typeof(PivotItem)))
            {
                this.ComputationList.DroppedItem = e.Data.GetData(typeof(PivotItem)) as PivotItem;
            }
            else if (e.Data.GetDataPresent(typeof(FilterItemsCollection)))
            {
                this.ComputationList.DroppedItem = e.Data.GetData(typeof(FilterItemsCollection)) as FilterItemsCollection;
            }
            else if (e.Data.GetDataPresent(typeof(PivotComputationInfo)))
            {
                PivotComputationInfo compInfo = e.Data.GetData(typeof(PivotComputationInfo)) as PivotComputationInfo;
                this.ComputationList.DroppedItem = compInfo;
            }

            RaiseEvent(new RoutedEventArgs(PivotGridComputationList.GenericDrop));
        }

        /// <summary>
        /// Handles the PreviewDragOverLeave event of the ComputationListAdorner.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.Windows.DragEventArgs"/> instance containing the event data.</param>        
        void ComputationListAdorner_PreviewDragLeave(object sender, DragEventArgs e)
        {
            Console.WriteLine("Adorner Drag Leave");
        }

        /// <summary>
        /// Handles the PreviewDragOver event of the ComputationListAdorner.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.Windows.DragEventArgs"/> instance containing the event data.</param>
        void ComputationListAdorner_PreviewDragOver(object sender, DragEventArgs e)
        {
            Console.WriteLine("Adorner Drag Over");
            RaiseEvent(new RoutedEventArgs(PivotGridComputationList.GenericDragOver));
        }

        #endregion

        #region [ Overrides ]

        /// <summary>
        /// Gets the number of visual child elements within this element.
        /// </summary>
        /// <value></value>
        /// <returns>The number of visual child elements for this element.</returns>
        protected override int VisualChildrenCount
        {
            get
            {
                return VisualChildren.Count;
            }
        }

        /// <summary>
        /// Overrides <see cref="M:System.Windows.Media.Visual.GetVisualChild(System.Int32)"/>, and returns a child at the specified index from a collection of child elements.
        /// </summary>
        /// <param name="index">The zero-based index of the requested child element in the collection.</param>
        /// <returns>
        /// The requested child element. This should not return null; if the provided index is out of range, an exception is thrown.
        /// </returns>
        protected override Visual GetVisualChild(int index)
        {
            return VisualChildren[index];
        }

        /// <summary>
        /// When overridden in a derived class, positions child elements and determines a size for a <see cref="T:System.Windows.FrameworkElement"/> derived class.
        /// </summary>
        /// <param name="finalSize">The final area within the parent that this element should use to arrange itself and its children.</param>
        /// <returns>The actual size used.</returns>
        protected override Size ArrangeOverride(Size finalSize)
        {
            double width = Common.GetTextSize(ItemText).Width + 150;
            double height = 24;
            StackPanel.Arrange(new Rect(X, Y, width, height));
            return finalSize;
        }

        #endregion
    }
    /// <summary>
    /// Class that hold the members needed for Dragging indicator button 
    /// </summary>
#if SyncfusionFramework4_0 
    [System.ComponentModel.DesignTimeVisible(false)]
#endif
    public class DragIndicatorButton : Button
    {
        #region [ Public Proeprties ]
        /// <summary>
        /// Gets or sets Background brush
        /// </summary>
        public Brush BackgroundBrush { get; set; }
        /// <summary>
        /// Gets or sets border brush
        /// </summary>
        public new Brush BorderBrush { get; set; }
        /// <summary>
        /// Gets or sets border item
        /// </summary>
        public Border BorderItem { get; set; }
        /// <summary>
        /// Gets or sets item's text
        /// </summary>
        public string ItemText { get; set; }
        /// <summary>
        /// Gets or sets SortPath
        /// </summary>
        public System.Windows.Shapes.Path SortPath { get; set; }
        /// <summary>
        /// Gets or sets Filter button's instance
        /// </summary>
        public Button FilterButton { get; set; }
        /// <summary>
        /// Gets or sets instance of GroupingBar
        /// </summary>
        public PivotGridGroupingBar GroupingBar { get; set; }
        /// <summary>
        /// Gets or sets PivotItem
        /// </summary>
        public object PivotItem { get; set; }
        /// <summary>
        /// Gets or sets whether it is a field list
        /// </summary>
        public bool IsFieldList { get; set; }

        #endregion

        #region [ Initialize/Finalize ]
        /// <summary>
        /// Initializes the <see cref="DragIndicatorButton"/> class.
        /// </summary>
        /// <param name="groupingBar">The PivotGrid GroupingBar</param>
        /// <param name="itemText">string</param>
        /// <param name="item">object</param>
        /// <param name="isFieldList">bool</param>
        public DragIndicatorButton(PivotGridGroupingBar groupingBar, string itemText, object item, bool isFieldList)
        {
            this.GroupingBar = groupingBar;
            this.BackgroundBrush = groupingBar.ItemsBackground;
            this.BorderBrush = groupingBar.ItemsBorderBrush;
            this.ItemText = itemText;
            this.PivotItem = item;
            this.IsFieldList = isFieldList;
        }

        #endregion

        #region [ Overrides ]
        /// <summary>
        /// When overridden in a derived class, is invoked whenever application code or internal processes call <see cref="M:System.Windows.FrameworkElement.ApplyTemplate"/>.
        /// </summary>
        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();
            this.FilterButton = GetTemplateChild("Filterbtn") as Button;
            this.SortPath = GetTemplateChild("SortPath") as System.Windows.Shapes.Path;

            if (PivotItem is PivotItem)
            {
                if (!this.GroupingBar.AllowFiltering || IsFieldList)
                    this.FilterButton.Visibility = System.Windows.Visibility.Collapsed;
                if (!this.GroupingBar.AllowSorting || IsFieldList)
                    this.SortPath.Visibility = System.Windows.Visibility.Collapsed;

            }

            else if (PivotItem is PivotComputationInfo)
            {
                this.FilterButton.Visibility = System.Windows.Visibility.Collapsed;
                this.SortPath.Visibility = System.Windows.Visibility.Collapsed;
            }

            else if (PivotItem is FilterItemsCollection)
            {
                if (!this.GroupingBar.AllowFiltering || IsFieldList)
                    this.FilterButton.Visibility = System.Windows.Visibility.Collapsed;
                this.SortPath.Visibility = System.Windows.Visibility.Collapsed;
            }


            if (PivotItem != null && PivotItem is PivotItem)
            {
                if (((PivotItem)PivotItem).Comparer != null)
                {
                    this.SortPath.Data = Geometry.Parse("F1 M 174.248,174.201L 174.248,174.201C 174.119,174.201 174.014,174.403 174.014,174.654C 174.014,174.774 174.039,174.891 174.082,174.972L 177.906,180.759C 177.956,180.83 178.018,180.873 178.084,180.873C 178.151,180.873 178.213,180.83 178.263,180.759L 179.156,179.411L 182.092,174.976C 182.135,174.896 182.161,174.778 182.161,174.659C 182.161,174.407 182.057,174.205 181.929,174.205L 180.434,174.204L 175.744,174.202L 174.248,174.201 Z ");
                }
                else
                {
                    this.SortPath.Data = Geometry.Parse("F1 M 181.926,180.872L 181.926,180.872C 182.055,180.872 182.16,180.671 182.16,180.419C 182.16,180.299 182.134,180.182 182.091,180.101L 178.264,174.316C 178.215,174.246 178.152,174.203 178.086,174.203C 178.019,174.203 177.957,174.246 177.908,174.316L 177.015,175.665L 174.081,180.101C 174.038,180.182 174.012,180.299 174.012,180.419C 174.012,180.671 174.116,180.872 174.245,180.872L 175.74,180.872L 180.429,180.872L 181.926,180.872 Z");
                }
            }
        }

        #endregion
    }

}
