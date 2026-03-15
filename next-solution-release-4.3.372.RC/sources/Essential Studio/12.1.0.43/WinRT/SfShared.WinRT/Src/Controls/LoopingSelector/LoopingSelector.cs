#region Copyright Syncfusion Inc. 2001 - 2014
// Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Any infringement will be prosecuted under
// applicable laws. 
#endregion
using Syncfusion.UI.Xaml.Primitives;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Foundation;
using Windows.UI.Input;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Media.Animation;

namespace Syncfusion.UI.Xaml.Controls
{
    /// <summary>
    /// Defines the different states options that a <see
    /// cref="T:Syncfusion.UI.Xaml.Controls.LoopingSelector"/> can be animated.
    /// </summary>
    public enum State
    {
        /// <summary>
        /// Looping is in Normal state
        /// </summary>
        Normal,
        /// <summary>
        /// Looping is in Expanded state
        /// </summary>
        Expanded,
        /// <summary>
        /// Looping is in Dragging state
        /// </summary>
        Dragging,
        /// <summary>
        /// Looping is in Snapping state
        /// </summary>
        Snapping,
        /// <summary>
        /// Looping is in Flicking state
        /// </summary>
        Flicking
    }

    /// <summary>
    ///  Provides information about the DateTime changes of all kinds.
    /// </summary>
    [ClassReference(IsReviewed = false, ShouldInclude = false)]
    public class SnappedEventArgs : EventArgs
    {
        private DateTime datetime;

        /// <summary>
        /// Gets or sets the DateTime
        /// </summary>
        public DateTime DateTime
        {
            get { return datetime; }
            set { datetime = value; }
        }

    }

    /// <summary>
    ///  LoopingSelector is a <see cref="N:Windows.UI.Xaml.Controls.Control"/>, that
    /// allow the user to select the items based the the <see
    /// cref="T:Syncfusion.UI.Xaml.Controls.State"/>. It is touch friendly and resembles
    /// the Windows phone looping selector.
    /// </summary>
    [ClassReference(IsReviewed = false)]
    public class LoopingSelector : Control
    {
        
        #region construtor

        /// <summary>
        /// Initializes a new instance of the <see
        /// cref="T:Syncfusion.UI.Xaml.Controls.LoopingSelector"/> class.
        /// </summary>
        [ClassReference(IsReviewed = false)]
        public LoopingSelector()
        {
            DefaultStyleKey = typeof(LoopingSelector);
            CreateEventHandlers();
        }

        #endregion

        #region Variables
        // The names of the template parts
        private const string ItemsPanelName = "ItemsPanel";
        private const string CenteringTransformName = "CenteringTransform";
        private const string PanningTransformName = "PanningTransform";

        // Amount of finger movement before the manipulation is considered a dragging manipulation.
        private const double DragSensitivity = 12;

        private static readonly Duration _selectDuration = new Duration(TimeSpan.FromMilliseconds(100));
        private readonly EasingFunctionBase _selectEase = new ExponentialEase() { EasingMode = EasingMode.EaseInOut };

        private static readonly Duration _panDuration = new Duration(TimeSpan.FromMilliseconds(100));
        private readonly EasingFunctionBase _panEase = new ExponentialEase();

        private DoubleAnimation _panelAnimation;
        private Storyboard _panelStoryboard;

        private Panel _itemsPanel;
        private TranslateTransform _panningTransform;
        private TranslateTransform _centeringTransform;

        private bool _isSelecting;
        private bool _isSizeChanging;
        private LoopingSelectorItem _selectedItem;

        private Queue<LoopingSelectorItem> _temporaryItemsPool;

        private double _minimumPanelScroll = float.MinValue;
        private double _maximumPanelScroll = float.MaxValue;

        private int _additionalItemsCount = 0;

        private bool _isAnimating;

        private double _dragTarget;

        // Once the user starts dragging horizontally, he is not allowed to drag vertically
        // until he completes his touch gesture and starts again.
        private bool _isAllowedToDragVertically = true;

        // Specify whether or not the user is dragging with his finger.
        private bool _isDragging;

        private int previousenableditemindex = 0;

        //Increment the count until getting the Enable Item 
        private double targetCount = 0;

        //Specify whether or not the user is dragging Programmatically
        private bool IsDraggingManually = false;

        #endregion

        #region getsetproperties
        /// <summary>
        /// Gets or sets the state
        /// </summary>
        public State State
        {
            get
            {
                return _state;
            }
        }

        /// <summary>
        /// Represents a variable for the <see cref="T:Syncfusion.UI.Xaml.Controls.State"/>
        /// </summary>
        public State _state;
        #endregion

        #region Dependency properties
        /// <summary>
        /// Gets or sets the data source that the control is displaying data for.
        /// </summary>
        /// <value>
        /// The default value is null.
        /// </value>
        [ClassReference(IsReviewed = false)]
        public ILoopingSelectorDataSource DataSource
        {
            get { return (ILoopingSelectorDataSource)GetValue(DataSourceProperty); }
            set
            {
                if (DataSource != null)
                {
                    DataSource.SelectionChanged -= value_SelectionChanged;
                }

                SetValue(DataSourceProperty, value);

                if (value != null)
                {
                    value.SelectionChanged += value_SelectionChanged;
                }
            }
        }

        /// <summary>
        /// Using a DependencyProperty as the backing store for DataSource.  This enables animation, styling, binding, etc...
        /// </summary>
        public static readonly DependencyProperty DataSourceProperty =
            DependencyProperty.Register("DataSource", typeof(ILoopingSelectorDataSource), typeof(LoopingSelector), new PropertyMetadata(null, OnDataModelChanged));

          /// <summary>
        /// Gets or sets the background for <see
        /// cref="T:Syncfusion.UI.Xaml.Controls.Input.LoopingSelector"/>.
        /// </summary>
        /// <value>
        /// The default value is null.
        /// </value>
        /// <seealso cref="T:Syncfusion.UI.Xaml.Controls.Input.LoopingSelector"/>
        [ClassReference(IsReviewed = false)]
        public Brush SelectedForeground
        {
            get { return (Brush)GetValue(SelectedForegroundProperty); }
            set { SetValue(SelectedForegroundProperty, value); }
        }

        /// <summary>
        /// Using a DependencyProperty as the backing store for AccentBrush.  This enables animation, styling, binding, etc...
        /// </summary>
        public static readonly DependencyProperty SelectedForegroundProperty =
            DependencyProperty.Register("SelectedForeground", typeof(Brush), typeof(LoopingSelector), new PropertyMetadata(null));
        /// <summary>
        /// Gets or sets the <see cref="N:Windows.UI.Xaml.Controls.DataTemplate"/> used to
        /// display each item.
        /// </summary>
        /// <value>
        /// The default value is null.
        /// </value>
        /// <seealso cref="P:Syncfusion.UI.Xaml.Controls.LoopingSelector.ItemHeight"/>
        /// <seealso cref="P:Syncfusion.UI.Xaml.Controls.LoopingSelector.ItemMargin"/>
        /// <seealso cref="P:Syncfusion.UI.Xaml.Controls.LoopingSelector.ItemWidth"/>
        [ClassReference(IsReviewed = false)]
        public DataTemplate ItemTemplate
        {
            get { return (DataTemplate)GetValue(ItemTemplateProperty); }
            set { SetValue(ItemTemplateProperty, value); }
        }

        /// <summary>
        /// Using a DependencyProperty as the backing store for ItemTemplate.  This enables animation, styling, binding, etc...
        /// </summary>
        public static readonly DependencyProperty ItemTemplateProperty =
            DependencyProperty.Register("ItemTemplate", typeof(DataTemplate), typeof(LoopingSelector), new PropertyMetadata(null));



        /// <summary>
        /// Getsor sets the Templae selector for the item
        /// </summary>
        public DataTemplateSelector ItemTemplateSelector
        {
            get { return (DataTemplateSelector)GetValue(ItemTemplateSelectorProperty); }
            set { SetValue(ItemTemplateSelectorProperty, value); }
        }

        /// <summary>
        /// Using a DependencyProperty as the backing store for ItemTemplateSelector.  This enables animation, styling, binding, etc...
        /// </summary>
        public static readonly DependencyProperty ItemTemplateSelectorProperty =
            DependencyProperty.Register("ItemTemplateSelector", typeof(DataTemplateSelector), typeof(LoopingSelector), new PropertyMetadata(null));



        /// <summary>
        /// Gets or sets the height of the items.
        /// </summary>
        /// <value>
        /// The default value is zero.
        /// </value>
        /// <seealso cref="P:Syncfusion.UI.Xaml.Controls.LoopingSelector.ItemMargin"/>
        /// <seealso cref="P:Syncfusion.UI.Xaml.Controls.LoopingSelector.ItemTemplate"/>
        /// <seealso cref="P:Syncfusion.UI.Xaml.Controls.LoopingSelector.ItemWidth"/>
        [ClassReference(IsReviewed = false)]
        public double ItemHeight
        {
            get { return (double)GetValue(ItemHeightProperty); }
            set { SetValue(ItemHeightProperty, value); }
        }

        /// <summary>
        /// Using a DependencyProperty as the backing store for ItemHeight.  This enables animation, styling, binding, etc...
        /// </summary>
        public static readonly DependencyProperty ItemHeightProperty =
            DependencyProperty.Register("ItemHeight", typeof(double), typeof(LoopingSelector), new PropertyMetadata(0.0,OnItemSizeChanged));

        /// <summary>
        /// Gets or sets the width of the items.
        /// </summary>
        /// <value>
        /// The default value is zero.
        /// </value>
        /// <seealso cref="P:Syncfusion.UI.Xaml.Controls.LoopingSelector.ItemHeight"/>
        /// <seealso cref="P:Syncfusion.UI.Xaml.Controls.LoopingSelector.ItemMargin"/>
        /// <seealso cref="P:Syncfusion.UI.Xaml.Controls.LoopingSelector.ItemTemplate"/>
        [ClassReference(IsReviewed = false)]
        public double ItemWidth
        {
            get { return (double)GetValue(ItemWidthProperty); }
            set { SetValue(ItemWidthProperty, value); }
        }

        /// <summary>
        /// Using a DependencyProperty as the backing store for ItemWidth.  This enables animation, styling, binding, etc...
        /// </summary>
        public static readonly DependencyProperty ItemWidthProperty =
            DependencyProperty.Register("ItemWidth", typeof(double), typeof(LoopingSelector), new PropertyMetadata(0.0,OnItemSizeChanged));

        /// <summary>
        /// Gets or sets margin around the items, to be a part of the touchable area.
        /// </summary>
        /// <value>
        /// The default value is null.
        /// </value>
        /// <seealso cref="P:Syncfusion.UI.Xaml.Controls.LoopingSelector.ItemHeight"/>
        /// <seealso cref="P:Syncfusion.UI.Xaml.Controls.LoopingSelector.ItemWidth"/>
        /// <seealso cref="P:Syncfusion.UI.Xaml.Controls.LoopingSelector.ItemTemplate"/>
        [ClassReference(IsReviewed = false)]
        public Thickness ItemMargin
        {
            get { return (Thickness)GetValue(ItemMarginProperty); }
            set { SetValue(ItemMarginProperty, value); }
        }

        /// <summary>
        /// Using a DependencyProperty as the backing store for ItemMargin.  This enables animation, styling, binding, etc...
        /// </summary>
        public static readonly DependencyProperty ItemMarginProperty =
            DependencyProperty.Register("ItemMargin", typeof(Thickness), typeof(LoopingSelector), new PropertyMetadata(null));

        /// <summary>
        /// Gets or sets the style that is applied to the container element generated for
        /// each item.
        /// </summary>
        /// <value>
        /// The default value is null.
        /// </value>
        [ClassReference(IsReviewed = false)]
        public Style ItemContainerStyle
        {
            get { return (Style)GetValue(ItemContainerStyleProperty); }
            set { SetValue(ItemContainerStyleProperty, value); }
        }

        /// <summary>
        /// Using a DependencyProperty as the backing store for ItemContainerStyle.  This enables animation, styling, binding, etc...
        /// </summary>
        public static readonly DependencyProperty ItemContainerStyleProperty =
            DependencyProperty.Register("ItemContainerStyle", typeof(Style), typeof(LoopingSelector), new PropertyMetadata(null));

        /// <summary>
        /// Gets or sets the background for the <see
        /// cref="T:Syncfusion.UI.Xaml.Controls.LoopingSelector"/>.
        /// </summary>
        /// <value>
        /// The default value is null.
        /// </value>
        [ClassReference(IsReviewed = false)]
        public Brush AccentBrush
        {
            get { return (Brush)GetValue(AccentBrushProperty); }
            set { SetValue(AccentBrushProperty, value); }
        }

        /// <summary>
        /// Using a DependencyProperty as the backing store for AccentBrush.  This enables animation, styling, binding, etc...
        /// </summary>
        public static readonly DependencyProperty AccentBrushProperty =
            DependencyProperty.Register("AccentBrush", typeof(Brush), typeof(LoopingSelector), new PropertyMetadata(null));


        /// <summary>
        /// Gets or sets a value indicating whether this instance the looping selector is
        /// expanded .
        /// </summary>
        /// <value>
        /// <see langword="true"/> if the looping selector is open; otherwise, <see
        /// langword="false"/>.
        /// </value>
        [ClassReference(IsReviewed = false)]
        public bool IsExpanded
        {
            get { return (bool)GetValue(IsExpandedProperty); }
            set { SetValue(IsExpandedProperty, value); }
        }

        /// <summary>
        /// The IsExpanded DependencyProperty.
        /// </summary>
        public static readonly DependencyProperty IsExpandedProperty =
            DependencyProperty.Register("IsExpanded", typeof(bool), typeof(LoopingSelector), new PropertyMetadata(false, OnIsExpandedChanged));
       
        #endregion

        #region events

        /// <summary>
        /// Occurs when the <see
        /// cref="P:Syncfusion.UI.Xaml.Controls.LoopingSelector.IsExpanded"/> is changed.
        /// </summary>
        [ClassReference(IsReviewed = false)]
        public event DependencyPropertyChangedEventHandler IsExpandedChanged;

        [ClassReference(IsReviewed = false)]

        #endregion

        #region Touch Events

        private void OnTap(object sender, TappedRoutedEventArgs e)
        {
            Focus(FocusState.Keyboard);
            if (_panningTransform != null)
            {
                foreach (LoopingSelectorItem child in _itemsPanel.Children)
                {
                    if (child.IsTapped)
                    {
                        SelectAndSnapTo(child);
                        child.IsTapped = false;
                        return;
                    }
                }
            }
        }

        private void OnManipulationStarted(object sender, ManipulationStartedRoutedEventArgs e)
        {
            _isAllowedToDragVertically = true;
            _isDragging = false;
        }

        private void OnManipulationDelta(object sender, ManipulationDeltaRoutedEventArgs e)
        {
            if (_isDragging)
            {
                AnimatePanel(_panDuration, _panEase, _dragTarget += e.Delta.Translation.Y, false);
                e.Handled = true;
            }
            else if (Math.Abs(e.Cumulative.Translation.X) > DragSensitivity)
            {
                _isAllowedToDragVertically = false;
            }
            else if (_isAllowedToDragVertically && Math.Abs(e.Cumulative.Translation.Y) > DragSensitivity)
            {
                _isDragging = true;
                _state = State.Dragging;
                e.Handled = true;
                _selectedItem = null;

                if (!IsExpanded)
                {
                    IsExpanded = true;
                }

                _dragTarget = _panningTransform.Y;
                UpdateItemState();
            }
        }

        private void OnManipulationCompleted(object sender, ManipulationCompletedRoutedEventArgs e)
        {
            if (_isDragging)
            {

                // See if it was a flick
                if (e.IsInertial)
                {
                    //_state = State.Flicking;
                    _selectedItem = null;

                    if (!IsExpanded)
                    {
                        IsExpanded = true;
                    }
                    _selectedItem = null;
                    UpdateItemState();
                }

                if (_state == State.Dragging)
                {
                    SelectAndSnapToClosest();
                }

                _state = State.Expanded;

            }
        }
        
        #endregion

        #region helper methods


        private static void OnItemSizeChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            LoopingSelector picker = (LoopingSelector)d;
            picker._isSizeChanging = true;
            picker.UpdateData();
            if(picker.IsReady)
                picker.UpdateItemTemplate();
            picker._isSizeChanging = false;
        }

        private static void OnDataModelChanged(DependencyObject obj, DependencyPropertyChangedEventArgs e)
        {
            LoopingSelector picker = (LoopingSelector)obj;
            picker.UpdateData();
        }

        void DataModel_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (!IsReady)
            {
                return;
            }

            if (!_isSelecting && e.AddedItems.Count == 1)
            {
                object selection = e.AddedItems[0];

                foreach (LoopingSelectorItem child in _itemsPanel.Children)
                {
                    if (child.DataContext == selection)
                    {
                        SelectAndSnapTo(child);
                        break;
                    }
                }

                UpdateData();
            }
        }

        void value_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (!IsReady)
            {
                return;
            }

            if (!_isSelecting && e.AddedItems.Count == 1)
            {
                object selection = e.AddedItems[0];

                foreach (LoopingSelectorItem child in _itemsPanel.Children)
                {
                    if (child.DataContext == selection)
                    {
                        SelectAndSnapTo(child);
                        return;
                    }
                }
                UpdateData();
            }
        }

        private static void OnIsExpandedChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            LoopingSelector picker = (LoopingSelector)sender;

            picker.UpdateItemState();
            if (!picker.IsExpanded)
            {
                picker.SelectAndSnapToClosest();
            }

            if (picker._state == State.Normal || picker._state == State.Expanded)
            {
                picker._state = picker.IsExpanded ? State.Expanded : State.Normal;
            }

            var listeners = picker.IsExpandedChanged;
            if (listeners != null)
            {
                listeners(picker, e);
            }
            if (!(bool)e.NewValue) 
                picker._isDragging = false;
        }

        void LoopingSelector_MouseLeftButtonDown(object sender, PointerRoutedEventArgs e)
        {
            if (_isAnimating)
            {
                double y = _panningTransform.Y;
                StopAnimation();
                _panningTransform.Y = y;
                _isAnimating = false;
                _state = State.Dragging;
            }
        }

        void LoopingSelector_MouseLeftButtonUp(object sender, PointerRoutedEventArgs e)
        {
            if (_selectedItem != sender && _state == State.Dragging && !_isAnimating)
            {
                SelectAndSnapToClosest();
                _state = State.Expanded;
                IsExpanded = true;
            }
            UpdateItemTemplate();
        }

        private void LoopingSelector_PointerWheelChanged(object sender, PointerRoutedEventArgs e)
        {
            if (!IsExpanded)
            {
                IsExpanded = true;
                _state = State.Expanded;
            }
            if (e.GetCurrentPoint(this).Properties.MouseWheelDelta > 0 && this._selectedItem != null && this._selectedItem.Previous != null && this._selectedItem.Previous._state != LoopingSelectorItem.State.Disabled)
            {
                AnimatePanel(_panelAnimation.Duration, _panelAnimation.EasingFunction, _panningTransform.Y + ItemHeight, false);
            }
            else if(e.GetCurrentPoint(this).Properties.MouseWheelDelta < 0 && this._selectedItem!=null &&this._selectedItem.Next!=null && this._selectedItem.Next._state != LoopingSelectorItem.State.Disabled)
            {
                AnimatePanel(_panelAnimation.Duration, _panelAnimation.EasingFunction, _panningTransform.Y - ItemHeight, false); 
            }
            _isAnimating = false;
            SelectAndSnapToClosest();
        }

        void OnSizeChanged(object sender, SizeChangedEventArgs e)
        {
            _centeringTransform.Y = Math.Round(e.NewSize.Height / 2);
            Clip = new RectangleGeometry() { Rect = new Rect(0, 0, e.NewSize.Width, e.NewSize.Height) };
            UpdateData();
        }

        void OnWrapperClick(object sender, EventArgs e)
        {
            if (_state == State.Normal)
            {
                _state = State.Expanded;
                IsExpanded = true;
            }
            else if (_state == State.Expanded)
            {
                if (!_isAnimating && sender == _selectedItem)
                {
                    _state = State.Normal;
                    IsExpanded = false;
                }
                else if (sender != _selectedItem && !_isAnimating)
                {
                    SelectAndSnapTo((LoopingSelectorItem)sender);
                }
            }
        }

        private void SelectAndSnapTo(LoopingSelectorItem item)
        {
           if (item == null)
                return;
            if(item._state==LoopingSelectorItem.State.Disabled)
             {
                 LoopingSelectorItem itemenabled = (LoopingSelectorItem) _itemsPanel.Children[previousenableditemindex];
                 if (itemenabled != null && itemenabled._state != LoopingSelectorItem.State.Disabled)
                     SelectAndSnapTo(itemenabled);
                else
                   {
                       AnimatePanel(_selectDuration, _selectEase, targetCount, true);
                       targetCount++;
                       IsDraggingManually = true;
                    }
                 return;
             }
            else if (IsDraggingManually)
            {
                IsDraggingManually = false;
                _selectedItem = item;
                UpdateItemState();
            }

            if (_selectedItem != null)
            {
                _selectedItem.SetState(IsExpanded ? LoopingSelectorItem.State.Expanded : LoopingSelectorItem.State.Normal, true);
            }

            if (_selectedItem != item)
            {
                TranslateTransform previousitemtransform = _selectedItem != null ? _selectedItem.Transform : null ;
                _selectedItem = item;
                // Update DataSource.SelectedItem aynchronously so that animations have a chance to start.
                Dispatcher.RunAsync(Windows.UI.Core.CoreDispatcherPriority.Normal, () =>
                {
                    _isSelecting = true;
                    DataSource.SelectedItem = item.DataContext;
                    _isSelecting = false;
                }).AsTask();
                UpdateItemTemplate();
            }

            _selectedItem.SetState(LoopingSelectorItem.State.Selected, true);

            TranslateTransform transform = item.Transform;
            if (transform != null)
            {
                double newPosition = -transform.Y - Math.Round(item.ActualHeight / 2);
                if (_panningTransform.Y != newPosition)
                {
                    AnimatePanel(_selectDuration, _selectEase, newPosition, true);
                }
            }
            
        }

        /// <summary>
        /// Updates the template for the item
        /// </summary>
        public void UpdateItemTemplate()
        {
            foreach(LoopingSelectorItem item in _itemsPanel.Children)
            {
                item.ContentTemplate = this.ItemTemplateSelector != null
                                               ? this.ItemTemplateSelector.SelectTemplate(item.Content, item)
                                               : this.ItemTemplate;
            }
            UpdateItemState();
        }
        private void UpdateData()
        {
            if (!IsReady)
            {
                return;
            }

            // Save all items
            _temporaryItemsPool = new Queue<LoopingSelectorItem>(_itemsPanel.Children.Count);
            foreach (LoopingSelectorItem item in _itemsPanel.Children)
            {
                if (item.GetState() == LoopingSelectorItem.State.Selected)
                {
                    item.SetState(LoopingSelectorItem.State.Normal, false);
                }
                _temporaryItemsPool.Enqueue(item);
                item.Remove();
            }

            _itemsPanel.Children.Clear();
            StopAnimation();
            _panningTransform.Y = 0;

            // Reset the extents
            _minimumPanelScroll = float.MinValue;
            _maximumPanelScroll = float.MaxValue;

            Balance();
        }

        private void AnimatePanel(Duration duration, EasingFunctionBase ease, double to, bool animate)
        {
            // Be sure not to run past the first or last items
            double newTo = Math.Max(_minimumPanelScroll, Math.Min(_maximumPanelScroll, to));
            if (to != newTo)
            {
                // Adjust the duration
                double originalDelta = Math.Abs(_panningTransform.Y - to);
                double modifiedDelta = Math.Abs(_panningTransform.Y - newTo);
                double factor = modifiedDelta / originalDelta;
                if (duration.HasTimeSpan) 
                    duration = new Duration(TimeSpan.FromMilliseconds(duration.TimeSpan.Milliseconds * factor));

                to = newTo;
            }

            double from = _panningTransform.Y;
            StopAnimation();
            CompositionTarget.Rendering += AnimationPerFrameCallback;

            if (animate)
            {
                _panelAnimation.Duration = duration;
                _panelAnimation.EasingFunction = ease;
                _panelAnimation.From = from;
                _panelAnimation.To = to;
                _panelStoryboard.Begin();
                _panelStoryboard.SeekAlignedToLastTick(TimeSpan.Zero);

            }
            else
            {
                _panningTransform.Y = to;
            }
            _isAnimating = true;
        }

        void AnimationPerFrameCallback(object sender, object e)
        {
            Balance();
            UpdateItemTemplate();
        }

        private void StopAnimation()
        {
            _panelStoryboard.Stop();
            CompositionTarget.Rendering -= AnimationPerFrameCallback;
        }

        private void Brake(double newStoppingPoint)
        {
            double originalDelta = _panelAnimation.To.Value - _panelAnimation.From.Value;
            double remainingDelta = newStoppingPoint - _panningTransform.Y;
            double factor = remainingDelta / originalDelta;

            Duration duration = new Duration(TimeSpan.FromMilliseconds(_panelAnimation.Duration.TimeSpan.Milliseconds * factor));

            AnimatePanel(duration, _panelAnimation.EasingFunction, newStoppingPoint, false);
        }

        private bool IsReady
        {
            get { return (ActualHeight > 0 || _isSizeChanging) && DataSource != null && _itemsPanel != null; }
        }

        private void Balance()
        {
            if (!IsReady)
            {
                return;
            }

            double actualItemWidth = ActualItemWidth;
            double actualItemHeight = ActualItemHeight;

            _additionalItemsCount = (int)Math.Round((ActualHeight * 1.5) / actualItemHeight);

            LoopingSelectorItem closestToMiddle = null;
            int closestToMiddleIndex = -1;

            if (_itemsPanel.Children.Count == 0)
            {
                // We need to get the selection and start from there
                closestToMiddleIndex = 0;
                _selectedItem = closestToMiddle = CreateAndAddItem(_itemsPanel, DataSource.SelectedItem);
                closestToMiddle.Transform.Y = -actualItemHeight / 2;
                closestToMiddle.Transform.X = (ActualWidth - actualItemWidth) / 2;
                closestToMiddle.SetState(LoopingSelectorItem.State.Selected, false);
            }
            else
            {
                closestToMiddleIndex = GetClosestItem();
                closestToMiddle = (LoopingSelectorItem)_itemsPanel.Children[closestToMiddleIndex];
            }

            int itemsBeforeCount;
            LoopingSelectorItem firstItem = GetFirstItem(closestToMiddle, out itemsBeforeCount);

            int itemsAfterCount;
            LoopingSelectorItem lastItem = GetLastItem(closestToMiddle, out itemsAfterCount);

            // Does the top need items?
            if (itemsBeforeCount < itemsAfterCount || itemsBeforeCount < _additionalItemsCount)
            {
                while (itemsBeforeCount < _additionalItemsCount)
                {
                    object newData = DataSource.GetPrevious(firstItem.DataContext);
                    if (newData == null)
                    {
                        // There may be room to display more items, but there is no more data.
                        _maximumPanelScroll = -firstItem.Transform.Y - actualItemHeight / 2;
                        if (_panelAnimation.To.HasValue)
                        {
                            if (_isAnimating && _panelAnimation.To.Value > _maximumPanelScroll)
                            {
                                Brake(_maximumPanelScroll);
                            }
                        }
                        break;
                    }

                    LoopingSelectorItem newItem = null;

                    // Can an item from the bottom be re-used?
                    if (itemsAfterCount > _additionalItemsCount)
                    {
                        newItem = lastItem;
                        lastItem = lastItem.Previous;
                        newItem.Remove();
                        newItem.Content = newItem.DataContext = newData;
                    }
                    else
                    {
                        // Make a new item
                        newItem = CreateAndAddItem(_itemsPanel, newData);
                        newItem.Transform.X = (ActualWidth - actualItemWidth) / 2;
                    }

                    // Put the new item on the top
                    newItem.Transform.Y = firstItem.Transform.Y - actualItemHeight;
                    newItem.InsertBefore(firstItem);
                    firstItem = newItem;

                    ++itemsBeforeCount;
                }
            }

            // Does the bottom need items?
            if (itemsAfterCount < itemsBeforeCount || itemsAfterCount < _additionalItemsCount)
            {
                while (itemsAfterCount < _additionalItemsCount)
                {
                    object newData = DataSource.GetNext(lastItem.DataContext);
                    if (newData == null)
                    {
                        // There may be room to display more items, but there is no more data.
                        _minimumPanelScroll = -lastItem.Transform.Y - actualItemHeight / 2;
                        if (_isAnimating && _panelAnimation.To.HasValue &&_panelAnimation.To.Value < _minimumPanelScroll)
                        {
                            Brake(_minimumPanelScroll);
                        }
                        break;
                    }

                    LoopingSelectorItem newItem = null;

                    // Can an item from the top be re-used?
                    if (itemsBeforeCount > _additionalItemsCount)
                    {
                        newItem = firstItem;
                        firstItem = firstItem.Next;
                        newItem.Remove();
                        newItem.Content = newItem.DataContext = newData;
                    }
                    else
                    {
                        // Make a new item
                        newItem = CreateAndAddItem(_itemsPanel, newData);
                        newItem.Transform.X = (ActualWidth - actualItemWidth) / 2;
                    }

                    // Put the new item on the bottom
                    newItem.Transform.Y = lastItem.Transform.Y + actualItemHeight;
                    newItem.InsertAfter(lastItem);
                    lastItem = newItem;

                    ++itemsAfterCount;
                }
            }

            _temporaryItemsPool = null;
        }

        private static LoopingSelectorItem GetFirstItem(LoopingSelectorItem item, out int count)
        {
            count = 0;
            while (item.Previous != null)
            {
                ++count;
                item = item.Previous;
            }

            return item;
        }

        private static LoopingSelectorItem GetLastItem(LoopingSelectorItem item, out int count)
        {
            count = 0;
            while (item.Next != null)
            {
                ++count;
                item = item.Next;
            }

            return item;
        }

        private int GetClosestItem()
        {
            if (!IsReady)
            {
                return -1;
            }

            double actualItemHeight = ActualItemHeight;

            int count = _itemsPanel.Children.Count;
            double panelY = _panningTransform.Y;
            double halfHeight = actualItemHeight / 2;
            int found = -1;
            double closestDistance = double.MaxValue;
            previousenableditemindex = 0;
            LoopingSelectorItem previouswrapper = null;
            for (int index = 0; index < count; ++index)
            {
                LoopingSelectorItem wrapper = (LoopingSelectorItem)_itemsPanel.Children[index];
                if (previouswrapper == null || panelY > 0 || (previouswrapper != null && previouswrapper.DataContext is DateTimeWrapper
                                                && wrapper.DataContext is DateTimeWrapper &&
                                                (previouswrapper.DataContext as DateTimeWrapper).DateTime <
                                                (wrapper.DataContext as DateTimeWrapper).DateTime))
                {
                    previousenableditemindex = wrapper._state == LoopingSelectorItem.State.Disabled
                                                   ? previousenableditemindex
                                                   : index;
                }
                previouswrapper = wrapper;
                double distance = Math.Abs((wrapper.Transform.Y + halfHeight) + panelY);
                if (distance <= halfHeight)
                {
                    found = index;
                    break;
                }
                else if (closestDistance > distance)
                {
                    closestDistance = distance;
                    found = index;
                }
            }

            return found;
        }

        void PanelStoryboardCompleted(object sender, object e)
        {
            CompositionTarget.Rendering -= AnimationPerFrameCallback;
            _isAnimating = false;
            if (_state != State.Dragging)
            {
                SelectAndSnapToClosest();
            }
            
        }

        private void SelectAndSnapToClosest()
        {
            if (!IsReady)
            {
                return;
            }

            int index = GetClosestItem();
            if (index == -1)
            {
                return;
            }

            LoopingSelectorItem item = (LoopingSelectorItem)_itemsPanel.Children[index];
            SelectAndSnapTo(item);
        }

        private void UpdateItemState()
        {
            if (!IsReady)
            {
                return;
            }

            bool isExpanded = IsExpanded;

            foreach (LoopingSelectorItem child in _itemsPanel.Children)
            {
                if (child == _selectedItem)
                {
                    child.SetState(LoopingSelectorItem.State.Selected, true);
                }
                else
                {
                    if (child._state == LoopingSelectorItem.State.Disabled)
                    {
                        child.SetState(isExpanded ? LoopingSelectorItem.State.Expanded : LoopingSelectorItem.State.Normal, true);
                        child.SetState(LoopingSelectorItem.State.Disabled, false);
                    }
                    else
                    {
                        child.SetState(isExpanded ? LoopingSelectorItem.State.Expanded : LoopingSelectorItem.State.Normal, true);
                    }
                }
                Binding binding =new Binding();
                binding.Source=this;
                binding.Path=new PropertyPath("SelectedForeground");
                binding.Mode=BindingMode.TwoWay;
                child.SetBinding(LoopingSelectorItem.SelectedForegroundProperty, binding);
            }
        }

        private double ActualItemWidth { get { return Padding.Left + Padding.Right + ItemWidth; } }
        
        private double ActualItemHeight { get { return Padding.Top + Padding.Bottom + ItemHeight; } }

        private void CreateVisuals()
        {
            _panelAnimation = new DoubleAnimation();
            Storyboard.SetTarget(_panelAnimation, _panningTransform);
            Storyboard.SetTargetProperty(_panelAnimation, "Y");

            _panelStoryboard = new Storyboard();
            _panelStoryboard.Children.Add(_panelAnimation);
            _panelStoryboard.Completed += PanelStoryboardCompleted;
        }

        private void CreateEventHandlers()
        {

            SizeChanged += OnSizeChanged;

            this.ManipulationStarted += OnManipulationStarted;
            this.ManipulationCompleted += OnManipulationCompleted;
            this.ManipulationInertiaStarting += LoopingSelector_ManipulationInertiaStarting;
            this.ManipulationDelta += OnManipulationDelta;
            this.Tapped += OnTap;
            
            AddHandler(PointerPressedEvent, new PointerEventHandler(LoopingSelector_MouseLeftButtonDown), true);
            AddHandler(PointerReleasedEvent, new PointerEventHandler(LoopingSelector_MouseLeftButtonUp), true);
            AddHandler(PointerWheelChangedEvent, new PointerEventHandler(LoopingSelector_PointerWheelChanged), true);
        }

        void LoopingSelector_ManipulationInertiaStarting(object sender, ManipulationInertiaStartingRoutedEventArgs e)
        {
            e.TranslationBehavior.DesiredDeceleration = 0.007;
            e.TranslationBehavior.DesiredDisplacement = 0.0;
        }
        
        private LoopingSelectorItem CreateAndAddItem(Panel parent, object content)
        {
            bool reuse = _isSizeChanging ? false : _temporaryItemsPool != null && _temporaryItemsPool.Count > 0;

            LoopingSelectorItem wrapper = reuse ? _temporaryItemsPool.Dequeue() : new LoopingSelectorItem() { Style = ItemContainerStyle , AccentBrush = AccentBrush};

            if (!reuse)
            {
                wrapper.ContentTemplate = this.ItemTemplateSelector != null? this.ItemTemplateSelector.SelectTemplate(content, wrapper) : this.ItemTemplate;
                wrapper.Width = ItemWidth;
                wrapper.Height = ItemHeight;
                wrapper.Padding = ItemMargin;

                wrapper.Click += OnWrapperClick;
            }

            wrapper.DataContext = wrapper.Content = content;

            parent.Children.Add(wrapper); // Need to do this before calling ApplyTemplate
            if (!reuse)
            {
                wrapper.ApplyTemplate();
            }

            return wrapper;
        }
        
        #endregion

        #region override

        /// <summary>
        /// Initializes all the child elements of <see
        /// cref="T:Syncfusion.UI.Xaml.Controls.LoopingSelector"/> control.
        /// </summary>
        protected override void OnApplyTemplate()
        {
            base.OnApplyTemplate();

            // Find the template parts. Create dummy objects if parts are missing to avoid
            // null checks throughout the code (although we can't escape them completely.)
            _itemsPanel = GetTemplateChild(ItemsPanelName) as Panel ?? new Canvas();
            _centeringTransform = GetTemplateChild(CenteringTransformName) as TranslateTransform ?? new TranslateTransform();
            _panningTransform = GetTemplateChild(PanningTransformName) as TranslateTransform ?? new TranslateTransform();

            CreateVisuals();
        }

        #endregion
    }
}
