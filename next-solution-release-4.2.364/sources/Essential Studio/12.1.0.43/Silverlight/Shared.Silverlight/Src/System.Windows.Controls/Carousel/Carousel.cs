#region Copyright Syncfusion Inc. 2001 - 2014
// Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Any infringement will be prosecuted under
// applicable laws. 
#endregion
using System;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Ink;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;
using System.Linq;
using System.ComponentModel;

namespace Syncfusion.Windows.Tools.Controls
{
    /// <summary>
    /// 
    /// </summary>
    public class Carousel : ItemsControl
    {
        /// <summary>
        /// Occurs when [selection changed].
        /// </summary>
        public event PropertyChangedCallback SelectionChanged;
        /// <summary>
        /// Occurs when [selected index changed].
        /// </summary>
        public event PropertyChangedCallback SelectedIndexChanged;
        /// <summary>
        /// Occurs when [selected value changed].
        /// </summary>
        public event PropertyChangedCallback SelectedValueChanged;
        /// <summary>
        /// Initializes a new instance of the <see cref="Carousel"/> class.
        /// </summary>
        public Carousel()
        {
            DefaultStyleKey = typeof(Carousel);
            LayoutUpdated += new EventHandler(Carousel_LayoutUpdated);
        }

        /// <summary>
        /// Gets or sets the items per page.
        /// </summary>
        /// <value>The items per page.</value>
        public int ItemsPerPage
        {
            get { return (int)GetValue(ItemsPerPageProperty); }
            set { SetValue(ItemsPerPageProperty, value); }
        }

        /// <summary>
        /// 
        /// </summary>
        public static readonly DependencyProperty ItemsPerPageProperty =
            DependencyProperty.Register("ItemsPerPage", typeof(int), typeof(Carousel), new PropertyMetadata(6, (s, a) => ((Carousel)s).OnItemsPerPageChanged(s)));

        /// <summary>
        /// Called when [items per page changed].
        /// </summary>
        /// <param name="obj">The obj.</param>
        protected void OnItemsPerPageChanged(DependencyObject obj)
        {
            if (this._panel != null)
            {
                this._panel.ItemsPerPage = this.ItemsPerPage;
                this._panel.InvalidateMeasure();
            }
        }

        /// <summary>
        /// Gets or sets the global offset X.
        /// </summary>
        /// <value>The global offset X.</value>
        public double GlobalOffsetX
        {
            get { return (double)GetValue(GlobalOffsetXProperty); }
            set { SetValue(GlobalOffsetXProperty, value); }
        }

        /// <summary>
        /// 
        /// </summary>
        public static readonly DependencyProperty GlobalOffsetXProperty =
            DependencyProperty.Register("GlobalOffsetX", typeof(double), typeof(Carousel), new PropertyMetadata(0.4, (s, a) => ((Carousel)s).OnProjectionChanged(s)));

        /// <summary>
        /// Gets or sets the global offset Y.
        /// </summary>
        /// <value>The global offset Y.</value>
        public double GlobalOffsetY
        {
            get { return (double)GetValue(GlobalOffsetYProperty); }
            set { SetValue(GlobalOffsetYProperty, value); }
        }

        /// <summary>
        /// 
        /// </summary>
        public static readonly DependencyProperty GlobalOffsetYProperty =
            DependencyProperty.Register("GlobalOffsetY", typeof(double), typeof(Carousel), new PropertyMetadata(0.0, (s, a) => ((Carousel)s).OnProjectionChanged(s)));

        /// <summary>
        /// Gets or sets the global offset Z.
        /// </summary>
        /// <value>The global offset Z.</value>
        public double GlobalOffsetZ
        {
            get { return (double)GetValue(GlobalOffsetZProperty); }
            set { SetValue(GlobalOffsetZProperty, value); }
        }

        /// <summary>
        /// 
        /// </summary>
        public static readonly DependencyProperty GlobalOffsetZProperty =
            DependencyProperty.Register("GlobalOffsetZ", typeof(double), typeof(Carousel), new PropertyMetadata(0.5, (s, a) => ((Carousel)s).OnProjectionChanged(s)));

        /// <summary>
        /// Called when [projection changed].
        /// </summary>
        /// <param name="obj">The obj.</param>
        protected void OnProjectionChanged(DependencyObject obj)
        {
            if (this.ItemsHost != null)
            {
                CarouselPanel panel = this.ItemsHost as CarouselPanel;
                if (panel != null)
                {
                    panel.GlobalOffsetX = this.GlobalOffsetX;
                    panel.GlobalOffsetY = this.GlobalOffsetY;
                    panel.GlobalOffsetZ = this.GlobalOffsetZ;
                    panel.InvalidateMeasure();
                }
            }
        }

        Panel _itemsHost;
        /// <summary>
        /// Get the current ItemsHost (FlowPanel)
        /// </summary>
        private Panel ItemsHost
        {
            get
            {
                if (_itemsHost == null && ItemContainerGenerator != null)
                {
                    _itemsHost =  this.GetVisualDescendants().OfType<Panel>().Where(p => p is CarouselPanel).FirstOrDefault();
                }
                return _itemsHost;
            }
        }

        /// <summary>
        /// Handles the LayoutUpdated event of the Carousel control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
        void Carousel_LayoutUpdated(object sender, EventArgs e)
        {
            var host = ItemsHost;
            if (host != null)
            {
                LayoutUpdated -= Carousel_LayoutUpdated;
                _panel = (CarouselPanel)ItemsHost;
                if (_panel != null)
                {
                    _panel.GlobalOffsetX = this.GlobalOffsetX;
                    _panel.GlobalOffsetY = this.GlobalOffsetY;
                    _panel.GlobalOffsetZ = this.GlobalOffsetZ;
                    this._panel.ItemsPerPage = this.ItemsPerPage;
                    _panel.InvalidateMeasure();
                }
            }
            else
            {
#if SILVERLIGHT
                Dispatcher.BeginInvoke(() => InvalidateMeasure());
#endif
            }
        }
        
        CarouselPanel _panel;

        /// <summary>
        /// Gets or sets the selected item.
        /// </summary>
        /// <value>The selected item.</value>
        public CarouselItem SelectedItem
        {
            get { return (CarouselItem)GetValue(SelectedItemProperty); }
            set { SetValue(SelectedItemProperty, value); }
        }

        /// <summary>
        /// 
        /// </summary>
        public static readonly DependencyProperty SelectedItemProperty =
            DependencyProperty.Register("SelectedItem", typeof(CarouselItem), typeof(Carousel), new PropertyMetadata(null, OnSelectedItemChanged));

        /// <summary>
        /// Called when [selected item changed].
        /// </summary>
        /// <param name="obj">The obj.</param>
        /// <param name="args">The <see cref="System.Windows.DependencyPropertyChangedEventArgs"/> instance containing the event data.</param>
        private static void OnSelectedItemChanged(DependencyObject obj, DependencyPropertyChangedEventArgs args)
        {
            Carousel carousel = (Carousel)obj;
            if (carousel != null)
            {
                carousel.OnSelectedItemChanged(args);
            }
        }

        /// <summary>
        /// Called when [selected item changed].
        /// </summary>
        protected void OnSelectedItemChanged(DependencyPropertyChangedEventArgs args)
        {
            if (args.OldValue != null)
            {
                CarouselItem item = args.OldValue as CarouselItem;
                item.IsSelected = false;
            }
            if (this.SelectedItem == null)
            {
                SelectedIndex = -1;
                SelectedValue = null;
            }
            else
            {
                if (this.ItemsSource !=null)
                {
                    if (Items.Contains(this.SelectedItem.Content))
                    {
                        SelectedIndex = Items.IndexOf(SelectedItem.Content);
                        SelectedValue = SelectedItem.Content;
                    }
                    else
                    {
                        SelectedIndex = -1;
                        SelectedValue = null;
                    }
                }
                else if (this.Items.Count > 0)
                {
                    if (this.Items[0].GetType() == typeof(CarouselItem))
                    {
                        if (Items.Contains(this.SelectedItem))
                        {
                            SelectedIndex = Items.IndexOf(SelectedItem);
                            SelectedValue = SelectedItem;
                        }
                        else
                        {
                            SelectedIndex = -1;
                            SelectedValue = null;
                        }
                    }
                    else
                    {
                        if (Items.Contains(this.SelectedItem.Content))
                        {
                            SelectedIndex = Items.IndexOf(SelectedItem.Content);
                            SelectedValue = SelectedItem.Content;
                        }
                        else
                        {
                            SelectedIndex = -1;
                            SelectedValue = null;
                        }
                    }
                }
                AnimateLayoutOffset(SelectedIndex);
            }

            if (SelectionChanged != null)
            {
                SelectionChanged(this, args);
            }
        }

        /// <summary>
        /// Gets or sets the index of the selected.
        /// </summary>
        /// <value>The index of the selected.</value>
        public int SelectedIndex
        {
            get { return (int)GetValue(SelectedIndexProperty); }
            set { SetValue(SelectedIndexProperty, value); }
        }

        /// <summary>
        /// 
        /// </summary>
        public static readonly DependencyProperty SelectedIndexProperty =
            DependencyProperty.Register("SelectedIndex", typeof(int), typeof(Carousel), new PropertyMetadata(-1, (s, a) => ((Carousel)s).OnSelectedIndexChanged(a)));

        /// <summary>
        /// Called when [selected index changed].
        /// </summary>
        protected virtual void OnSelectedIndexChanged(DependencyPropertyChangedEventArgs args)
        {
            if (SelectedIndex == -1)
            {
                SelectedItem = null;
            }
            else 
            {
                var selectedContainer = ItemContainerGenerator.ContainerFromIndex(SelectedIndex) as CarouselItem;
                if (selectedContainer != SelectedItem)
                {
                    this.SelectedItem = selectedContainer;
                }
            }
            if (this.SelectedIndexChanged != null)
            {
                this.SelectedIndexChanged(this, args);
            }
        }

        /// <summary>
        /// Gets or sets the selected value.
        /// </summary>
        /// <value>The selected value.</value>
        public object SelectedValue
        {
            get { return (object)GetValue(SelectedValueProperty); }
            set { SetValue(SelectedValueProperty, value); }
        }

        /// <summary>
        /// 
        /// </summary>
        public static readonly DependencyProperty SelectedValueProperty =
            DependencyProperty.Register("SelectedValue", typeof(object), typeof(Carousel), new PropertyMetadata(null, (s, a) => ((Carousel)s).OnSelectedValueChanged(a)));

        /// <summary>
        /// 
        /// </summary>
        /// <param name="args"></param>
        protected void OnSelectedValueChanged(DependencyPropertyChangedEventArgs args)
        {
            if (this.SelectedValueChanged != null)
                this.SelectedValueChanged(this, args);
        }

        /// <summary>
        /// Determines if the specified item is (or is eligible to be) its own container.
        /// </summary>
        /// <param name="item">The item to check.</param>
        /// <returns>
        /// true if the item is (or is eligible to be) its own container; otherwise, false.
        /// </returns>
        protected override bool IsItemItsOwnContainerOverride(object item)
        {
            return item is CarouselItem;
        }

        /// <summary>
        /// Creates or identifies the element that is used to display the given item.
        /// </summary>
        /// <returns>
        /// The element that is used to display the given item.
        /// </returns>
        protected override DependencyObject GetContainerForItemOverride()
        {
            return new CarouselItem();
        }

        /// <summary>
        /// Prepares the specified element to display the specified item.
        /// </summary>
        /// <param name="element">The element used to display the specified item.</param>
        /// <param name="item">The item to display.</param>
        protected override void PrepareContainerForItemOverride(DependencyObject element, object item)
        {
            var ele = element as CarouselItem;
            if (ele != null)
            {
                ele.Owner = this;
                ele.ItemContent = item;
                if (SelectedItem == ele)
                {
                    ele.IsSelected = true;
                }
            }
            base.PrepareContainerForItemOverride(element, item);
        }

        /// <summary>
        /// Animates the layout offset.
        /// </summary>
        /// <param name="to">To.</param>
        private void AnimateLayoutOffset(double to)
        {
            double layoutOffset = LayoutOffset;

            if (true)
            {
                if (layoutOffset + Items.Count - to < to - LayoutOffset)
                {
                    layoutOffset += Items.Count;
                }
                else if (to + Items.Count - LayoutOffset < LayoutOffset - to)
                {
                    layoutOffset -= Items.Count;
                }
            }

            if (double.IsNaN(layoutOffset))
                layoutOffset = 0;
            ResetLayoutOffsetAnim(layoutOffset);

            TimeSpan perItemBasedDuration = TimeSpan.FromMilliseconds(400); 
            _layoutOffsetAnim.Duration = new Duration(perItemBasedDuration);

            _layoutOffsetAnim.To = to;
            _layoutStoryBoard.Begin();
        }

        Storyboard _layoutStoryBoard;
        DoubleAnimation _layoutOffsetAnim;
        /// <summary>
        /// Resets the layout offset anim.
        /// </summary>
        /// <param name="from">From.</param>
        private void ResetLayoutOffsetAnim(double from)
        {
            if (double.IsNaN(from))
                from = 0;
            if (_layoutStoryBoard != null)
            {
                _layoutStoryBoard.Stop();
                _layoutStoryBoard.Children.Clear();
            }
            _layoutOffsetAnim = new DoubleAnimation { EasingFunction = new CubicEase { EasingMode = EasingMode.EaseInOut }, Duration = new Duration(TimeSpan.FromMilliseconds(500)), From = from };
            Storyboard.SetTarget(_layoutOffsetAnim, this);
            Storyboard.SetTargetProperty(_layoutOffsetAnim, new PropertyPath(LayoutOffsetProperty));
            _layoutStoryBoard = new Storyboard { Children = { _layoutOffsetAnim } };
        }

        /// <summary>
        /// Gets or sets the layout offset.
        /// </summary>
        /// <value>The layout offset.</value>
        [EditorBrowsable(EditorBrowsableState.Never)]
        [BrowsableAttribute(false)]
        internal double LayoutOffset
        {
            get { return (double)GetValue(LayoutOffsetProperty); }
            set { SetValue(LayoutOffsetProperty, value); }
        }

        /// <summary>
        /// 
        /// </summary>
        internal static readonly DependencyProperty LayoutOffsetProperty =
            DependencyProperty.Register("LayoutOffset", typeof(double), typeof(Carousel), new PropertyMetadata(0.0,(s,a) => ((Carousel)s).OnLayoutOffsetCHanged()));

        /// <summary>
        /// Called when [layout offset C hanged].
        /// </summary>
        private void OnLayoutOffsetCHanged()
        {
            if (_panel != null)
            {
                _panel.LayoutOffset = LayoutOffset;
            }
        }

        
    }
}
