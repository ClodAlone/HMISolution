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
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using System.Collections.ObjectModel;
using Syncfusion.Windows.Controls.PivotSchemaDesigner;
using System.ComponentModel;
using Syncfusion.PivotAnalysis.Base;
using System.Windows.Interop;
using System.Runtime.InteropServices;
using System.Collections;
using Syncfusion.Windows.Shared;
using System.Windows.Controls.Primitives;
using System.Linq.Expressions;

namespace Syncfusion.Windows.Controls.PivotGrid
{
    /// <summary>
    /// Interaction logic for PivotFieldChooser.xaml
    /// </summary>
    public partial class PivotValueChooser : ChromelessWindow
    {

        ListBox dragSource = null;

        Point startPoint;

        private double itemHeight;
        /// <summary>
        /// Gets or Sets the height of the item
        /// </summary>
        public double ItemHeight
        {
            get { return itemHeight; }
            set { itemHeight = value; }
        }

        private double itemFontSize = 11;
        /// <summary>
        /// Gets or sets the font size of the item
        /// </summary>
        public double ItemFontSize
        {
            get { return itemFontSize; }
            set { itemFontSize = value; }
        }
        /// <summary>
        /// Initializes the <see cref="PivotValueChooser"/> class.
        /// </summary>
        public PivotValueChooser()
        {
            InitializeComponent();
        }



        internal IEnumerable FieldListItemsSource
        {
            get { return (IEnumerable)GetValue(FieldListItemsSourceProperty); }
            set { SetValue(FieldListItemsSourceProperty, value); }
        }
        
        // Using a DependencyProperty as the backing store for FieldListItemsSource.  This enables animation, styling, binding, etc...
        /// <summary>
        /// Identifies the <see cref="P:Syncfusion.Windows.Controls.PivotGrid.PivotValueChooser.FieldListItemsSource"/> dependency property.
        /// </summary>
        /// 
        /// <returns>
        /// The identifier for the <see cref="P:Syncfusion.Windows.Controls.PivotGrid..PivotValueChooser.FieldListItemsSource"/> dependency property.
        /// </returns>
        public static readonly DependencyProperty FieldListItemsSourceProperty =
            DependencyProperty.Register("FieldListItemsSource", typeof(IEnumerable), typeof(PivotValueChooser), new UIPropertyMetadata(null, new PropertyChangedCallback(
                (obj, args) => 
                {
                    PivotValueChooser valueChooser = obj as PivotValueChooser;
                    if (valueChooser != null)
                    {
                        valueChooser.pivotFieldList.ItemsSource = args.NewValue as IEnumerable;
                        valueChooser.PivotTableFields = args.NewValue as ObservableCollection<PivotValueField>;
                    }
                })));



        /// <summary>
        /// Initializes the <see cref="PivotValueChooser"/> class.
        /// </summary>
        /// <param name="gridControl">The PivotGridControl</param>
        public PivotValueChooser(PivotGridControl gridControl)
        {
            InitializeComponent();
            this.GridControl = gridControl;
            this.PivotTableFields = FieldListItemsSource as ObservableCollection<PivotValueField>;
            this.pivotFieldList.ItemsSource = FieldListItemsSource;
            WireEvents();
        }


        private void WireEvents()
        {
            this.Loaded += new RoutedEventHandler(PivotFieldChooser_Loaded);
            this.Closed += new EventHandler(PivotFieldChooser_Closed);
            this.pivotFieldList.PreviewMouseLeftButtonDown +=
                new MouseButtonEventHandler(pivotFieldList_PreviewMouseLeftButtonDown);
            this.pivotFieldList.PreviewMouseMove += new MouseEventHandler(pivotFieldList_PreviewMouseMove);
            this.pivotFieldList.PreviewDrop += new DragEventHandler(pivotFieldList_PreviewDrop);
            this.pivotFieldList.PreviewDragOver += new DragEventHandler(pivotFieldList_PreviewDragOver);
            this.pivotFieldList.DragOver += new DragEventHandler(pivotFieldList_DragOver);
            this.pivotFieldList.PreviewMouseLeftButtonUp +=
                new MouseButtonEventHandler(pivotFieldList_PreviewMouseLeftButtonUp);
        }

        void pivotFieldList_PreviewMouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            this.RemoveAdorners();
        }

        void pivotFieldList_DragOver(object sender, DragEventArgs e)
        {
            ListBox dropTarget = sender as ListBox;
            ScrollViewer sv = Common.FindVisualChild<ScrollViewer>(this.pivotFieldList);
            double initialValue = 10;
            double yValue = e.GetPosition(dropTarget).Y;
            double offset = 0.3;//by reducing this offset we can control the speed of the scrolling.

            if (yValue < initialValue)
            {
                sv.ScrollToVerticalOffset(sv.VerticalOffset - offset);
            }
            else if (yValue > dropTarget.ActualHeight - initialValue)
            {
                sv.ScrollToVerticalOffset(sv.VerticalOffset + offset);
            }
        }


        void pivotFieldList_PreviewDragOver(object sender, DragEventArgs e)
        {
            ListBox dropTarget = (sender as ListBox);
            Point hitPoint = e.GetPosition((IInputElement)dropTarget);
            HitTestResult result = VisualTreeHelper.HitTest(dropTarget, hitPoint);
            ListBoxItem lstItem = Common.GetParentElement<ListBoxItem>(result.VisualHit);
            ListBox control = Common.GetParentElement<ListBox>(result.VisualHit);

            if (dropTarget.SelectedItem != null)
                this.AddAdorners(hitPoint, dropTarget.SelectedItem);
        }

        void pivotFieldList_PreviewDrop(object sender, DragEventArgs e)
        {
            this.RemoveAdorners();
            ListBox dropTarget = (sender as ListBox);
            if (dropTarget != null)
            {
                Point hitPoint = e.GetPosition(dropTarget);
                HitTestResult result = VisualTreeHelper.HitTest(dropTarget, hitPoint);
                ListBoxItem lstItem = Common.GetParentElement<ListBoxItem>(result.VisualHit);
                PivotValueField selectedPivotField = (dropTarget.SelectedItem as PivotValueField);
                if (selectedPivotField == null)
                    return;
                int fromIndex = this.PivotTableFields.IndexOf(selectedPivotField);
                if (lstItem != null)
                {
                    PivotValueField pivotTableField = (lstItem.Content as PivotValueField);
                    int toIndex = this.PivotTableFields.IndexOf(pivotTableField);
                    this.PivotTableFields.RemoveAt(fromIndex);
                    if (toIndex == 0)
                    {
                        if ((lstItem.ActualHeight / 2) < hitPoint.Y)
                            toIndex++;
                    }
                    else if (fromIndex > toIndex && selectedPivotField.IsSelected)
                    {
                        toIndex++;
                    }
                    this.PivotTableFields.Insert(toIndex, selectedPivotField);

                    int target = toIndex;
                    if (toIndex < this.PivotTableFields.Where(i => i.IsSelected == true).Count() - 1 && selectedPivotField.IsSelected)
                        toIndex++;

                    if (GridControl.LocalPossibleCalculations != null)
                    {
                        this.GridControl.InternalGrid.MoveValueColumn(fromIndex + this.GridControl.PivotRows.Count, toIndex + this.GridControl.PivotRows.Count, target + this.GridControl.PivotRows.Count);
                        this.GridControl.InvalidateCells();
                    }
                }
                else if (hitPoint.Y < 5.0)
                {
                    this.PivotTableFields.RemoveAt(fromIndex);
                    this.PivotTableFields.Insert(0, selectedPivotField);
                    if (GridControl.LocalPossibleCalculations != null)
                    {
                        this.GridControl.InternalGrid.MoveValueColumn(fromIndex + this.GridControl.PivotRows.Count, 0 + this.GridControl.PivotRows.Count);
                        this.GridControl.InvalidateCells();
                    }
                }
            }
        }

        Point emptyPoint = new Point(0, 0);

        void pivotFieldList_PreviewMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            ListBox source = (sender as ListBox);
            HitTestResult htr = VisualTreeHelper.HitTest(source, e.GetPosition(source));
            if (htr.VisualHit is ListBox || htr.VisualHit is TextBlock)
            {
                dragSource = source;
                startPoint = e.GetPosition(dragSource);
            }
            else
            {
                startPoint = emptyPoint;
            }
        }

        void pivotFieldList_PreviewMouseMove(object sender, MouseEventArgs e)
        {
            ListBox source = (sender as ListBox);
            if (e.LeftButton == MouseButtonState.Pressed && startPoint != emptyPoint)
            {
                 Point position = e.GetPosition((IInputElement)source);
                 if (Math.Abs(position.X - startPoint.X) > SystemParameters.MinimumHorizontalDragDistance ||
                     Math.Abs(position.Y - startPoint.Y) > SystemParameters.MinimumVerticalDragDistance)
                 {
                     object _draggedData = GetDataFromListBox(source, position);
                     try
                     {
                         if (_draggedData != null)
                             DragDrop.DoDragDrop(source, _draggedData, DragDropEffects.Move);
                     }
                     catch
                     {
                         DragDrop.DoDragDrop(source, _draggedData, DragDropEffects.Move);
                     }
                 }
            }
        }

        void PivotFieldChooser_Closed(object sender, EventArgs e)
        {
            this.GridControl.ShowPivotValueChooser = false;
        }

        void PivotFieldChooser_Loaded(object sender, RoutedEventArgs e)
        {
            this.GridControl.ShowPivotValueChooser = true;
        }

        private ObservableCollection<PivotValueField> PivotTableFields { get; set; }

        internal PivotGridControl GridControl
        {
            get { return (PivotGridControl)GetValue(GridControlProperty); }
            set { SetValue(GridControlProperty, value); }
        }

        // Using a DependencyProperty as the backing store for GridControl.  This enables animation, styling, binding, etc...
        /// <summary>
        /// Identifies the <see cref="P:Syncfusion.Windows.Controls.PivotGrid.PivotValueChooser.GridControl"/> dependency property.
        /// </summary>
        /// 
        /// <returns>
        /// The identifier for the <see cref="P:Syncfusion.Windows.Controls.PivotGrid.PivotValueChooser.GridControl"/> dependency property.
        /// </returns>
        public static readonly DependencyProperty GridControlProperty =
            DependencyProperty.Register("GridControl", typeof(PivotGridControl), typeof(PivotValueChooser), new UIPropertyMetadata(null));


        /// <summary>
        /// Gets the list item based on co-ordinates
        /// </summary>
        /// <param name="source">Source list box</param>
        /// <param name="point">co-ordinates</param>
        /// <returns>object on co-ordinate</returns>
        internal static object GetDataFromListBox(ListBox source, Point point)
        {
            if (source != null)
            {
                UIElement element = source.InputHitTest(point) as UIElement;
                if (element != null)
                {
                    object data = DependencyProperty.UnsetValue;
                    while (data == DependencyProperty.UnsetValue)
                    {
                        if (element != null)
                        {
                            data = source.ItemContainerGenerator.ItemFromContainer(element);
                            if (data == DependencyProperty.UnsetValue)
                            {
                                element = VisualTreeHelper.GetParent(element) as UIElement;
                            }
                            if (Equals(element, source))
                            {
                                return null;
                            }
                        }
                    }
                    if (data != DependencyProperty.UnsetValue)
                    {
                        return data;
                    }
                }
            }
            return null;
        }

        internal void RemoveAdorners()
        {
            AdornerLayer layer = AdornerLayer.GetAdornerLayer(this.pivotFieldList);
            Adorner[] adorners = layer.GetAdorners(this.pivotFieldList);
            if (adorners != null)
            {
                foreach (var item in adorners)
                {
                    layer.Remove(item);
                }
            }
        }

        internal void RemoveAdorners(out AdornerLayer layer)
        {
            layer = AdornerLayer.GetAdornerLayer(this.pivotFieldList);
            Adorner[] adorners = layer.GetAdorners(this.pivotFieldList);
            if (adorners != null)
            {
                foreach (var item in adorners)
                {
                    layer.Remove(item);
                }
            }
        }

        internal void AddAdorners(Point point, object DraggedItem)
        {
            AdornerLayer layer = null;
            this.RemoveAdorners(out layer);
            layer.Opacity = 0.6;
            layer.Add(new ValueChooserAdorner(this.pivotFieldList, point.X + 5, point.Y + 5, DraggedItem));
        }

        private void btnSyncToGrid_Click(object sender, RoutedEventArgs e)
        {
            this.Cursor = Cursors.Wait;
            this.Cursor = Cursors.Arrow;
        }

        private void btnCancel_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

    }

    #region [ValueChooserAdorner]
    /// <summary>
    /// The class ValueChooserAdorners can be used to add functional handles to PivotValueChooser elements and also provide state information about PivotValueChooser.
    /// i.e, a overlapped layer and it can use it as an indicator to notify the user about PivotValueChooser's state .
    /// </summary>
    public class ValueChooserAdorner : Adorner
    {
        /// <summary>
        /// Gets or sets the visual children of PivotValueChooser window
        /// </summary>
        public VisualCollection VisualChildren { get; set; }
        /// <summary>
        /// Gets or sets the Pivot ValueChooser
        /// </summary>
        public PivotValueChooser ValueChooser { get; set; }
        /// <summary>
        /// Gets or sets the item which is dragged
        /// </summary>
        public PivotValueField DraggedItem { get; set; }
        /// <summary>
        /// Gets or sets the value X to create a Rect instance
        /// </summary>
        public double X { get; set; }
        /// <summary>
        /// Gets or sets the value Y to create a Rect instance
        /// </summary>
        public double Y { get; set; }
        /// <summary>
        /// Gets or sets the StackPanel instance
        /// </summary>
        public StackPanel StackPanel { get; set; }
        /// <summary>
        /// Gets or sets the text of the Item
        /// </summary>
        public string ItemText { get; set; }
        /// <summary>
        /// Initializes the <see cref="ValueChooserAdorner"/> class.
        /// </summary>
        /// <param name="adornedElement">UI element</param>
        /// <param name="x">double</param>
        /// <param name="y">double</param>
        /// <param name="draggedItem">object</param>
        public ValueChooserAdorner(UIElement adornedElement, double x, double y, object draggedItem)
            : base(adornedElement)
        {
            VisualChildren = new VisualCollection(this);
            ValueChooser = adornedElement as PivotValueChooser;
            DraggedItem = draggedItem as PivotValueField;

            StackPanel = new StackPanel();
            StackPanel.Background = Brushes.Transparent;
            StackPanel.Orientation = Orientation.Horizontal;
            X = x;
            Y = y;

            CheckBox checkBox = new CheckBox();
            checkBox.IsChecked = DraggedItem.IsSelected;
            TextBlock txtBlock = new TextBlock();
            txtBlock.Margin = new Thickness(4, 0, 0, 0);
            txtBlock.Height = 24;
            txtBlock.Text = DraggedItem.FieldHeader;

            StackPanel.Children.Add(checkBox);
            StackPanel.Children.Add(txtBlock);

            VisualChildren.Add(StackPanel);
        }

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
#endregion
    /// <summary>
    /// Class which holds the property of PivotValueField
    /// </summary>

    public class PivotValueField : PivotComputationInfo, INotifyPropertyChanged
    {
        private bool isSelected;
        /// <summary>
        /// Gets or sets whether the PivotValueField is selected or not
        /// </summary>
        public bool IsSelected
        {
            get { return isSelected; }
            set 
            {
                isSelected = value;
                this.OnPropertyChanged(sp=>sp.IsSelected);
            }
        }

        #region INotifyPropertyChanged Members
        /// <summary>
        /// PropertyChanged event that is triggered when the property of the PivotValueField is changed.
        /// </summary>
        public new event PropertyChangedEventHandler PropertyChanged;

        private void OnPropertyChanged<R>(Expression<Func<PivotTableField, R>> expr)
        {
            OnPropertyChanged(((MemberExpression)expr.Body).Member.Name);
        }

        private void OnPropertyChanged(string propertyName)
        {
            if (PropertyChanged != null)
                PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
        }

        #endregion
    }
}
