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
using System.Windows.Media.Media3D;
using Syncfusion.Licensing;


namespace Syncfusion.Windows.Shared
{
    /// <summary>
    /// 
    /// </summary>
    public class Carousel : ItemsControl
    {
        /// <summary>
        /// 
        /// </summary>
        internal CarouselItem previousSelected;
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
           DependencyProperty.Register("ItemsPerPage", typeof(int), typeof(Carousel), new PropertyMetadata(-1, (s, a) => ((Carousel)s).OnItemsPerPageChanged(s)));

       
        /// <summary>
        /// Called when [items per page changed].
        /// </summary>
        /// <param name="obj">The obj.</param>
        protected void OnItemsPerPageChanged(DependencyObject obj)
        {
            if (this.ItemsHost != null)
            {
               CustomPathCarouselPanel pane = (CustomPathCarouselPanel)this.ItemsHost;
               pane.ItemsPerPage = this.ItemsPerPage;                  
            }          
        }

        /// <summary>
        /// Gets or sets the scale fractions.
        /// </summary>
        /// <value>The scale fractions.</value>
        public PathFractionCollection ScaleFractions
        {
            get { return (PathFractionCollection)GetValue(ScaleFractionsProperty); }
            set { SetValue(ScaleFractionsProperty, value); }
        }

        /// <summary>
        /// 
        /// </summary>
        public static readonly DependencyProperty ScaleFractionsProperty =
            DependencyProperty.Register("ScaleFractions", typeof(PathFractionCollection), typeof(Carousel), new PropertyMetadata(null, (s, a) => ((Carousel)s).OnItemsVisualChanged(s)));

        /// <summary>
        /// Gets or sets a value indicating whether [scaling enabled].
        /// </summary>
        /// <value><c>true</c> if [scaling enabled]; otherwise, <c>false</c>.</value>
        public bool ScalingEnabled
        {
            get { return (bool)GetValue(ScalingEnabledProperty); }
            set { SetValue(ScalingEnabledProperty, value); }
        }

        /// <summary>
        /// 
        /// </summary>
        public static readonly DependencyProperty ScalingEnabledProperty =
            DependencyProperty.Register("ScalingEnabled", typeof(bool), typeof(Carousel), new PropertyMetadata(true, (s, a) => ((Carousel)s).OnItemsVisualChanged(s)));

        /// <summary>
        /// Gets or sets a value indicating whether [opacity enabled].
        /// </summary>
        /// <value><c>true</c> if [opacity enabled]; otherwise, <c>false</c>.</value>
        public bool OpacityEnabled
        {
            get { return (bool)GetValue(OpacityEnabledProperty); }
            set { SetValue(OpacityEnabledProperty, value); }
        }

        /// <summary>
        /// 
        /// </summary>
        public static readonly DependencyProperty OpacityEnabledProperty =
            DependencyProperty.Register("OpacityEnabled", typeof(bool), typeof(Carousel), new PropertyMetadata(true, (s, a) => ((Carousel)s).OnItemsVisualChanged(s)));


     


        /// <summary>
        /// Gets or sets the opacity fractions.
        /// </summary>
        /// <value>The opacity fractions.</value>
        public PathFractionCollection OpacityFractions
        {
            get { return (PathFractionCollection)GetValue(OpacityFractionsProperty); }
            set { SetValue(OpacityFractionsProperty, value); }
        }

        /// <summary>
        /// 
        /// </summary>
        public static readonly DependencyProperty OpacityFractionsProperty =
            DependencyProperty.Register("OpacityFractions", typeof(PathFractionCollection), typeof(Carousel), new PropertyMetadata(null, (s, a) => ((Carousel)s).OnItemsVisualChanged(s)));

        /// <summary>
        /// Gets or sets a value indicating whether [skew angle X enabled].
        /// </summary>
        /// <value><c>true</c> if [skew angle X enabled]; otherwise, <c>false</c>.</value>
        public bool SkewAngleXEnabled
        {
            get { return (bool)GetValue(SkewAngleXEnabledProperty); }
            set { SetValue(SkewAngleXEnabledProperty, value); }
        }
        /// <summary>
        /// 
        /// </summary>
        public static readonly DependencyProperty SkewAngleXEnabledProperty =
            DependencyProperty.Register("SkewAngleXEnabled", typeof(bool), typeof(Carousel), new PropertyMetadata(false, (s, a) => ((Carousel)s).OnItemsVisualChanged(s)));

        /// <summary>
        /// Gets or sets the skew angle X fractions.
        /// </summary>
        /// <value>The skew angle X fractions.</value>
        public PathFractionCollection SkewAngleXFractions
        {
            get { return (PathFractionCollection)GetValue(SkewAngleXFractionsProperty); }
            set { SetValue(SkewAngleXFractionsProperty, value); }
        }

        /// <summary>
        /// 
        /// </summary>
        public static readonly DependencyProperty SkewAngleXFractionsProperty =
            DependencyProperty.Register("SkewAngleXFractions", typeof(PathFractionCollection), typeof(Carousel), new PropertyMetadata(null, (s, a) => ((Carousel)s).OnItemsVisualChanged(s)));

        /// <summary>
        /// Gets or sets a value indicating whether [skew angle Y enabled].
        /// </summary>
        /// <value><c>true</c> if [skew angle Y enabled]; otherwise, <c>false</c>.</value>
        public bool SkewAngleYEnabled
        {
            get { return (bool)GetValue(SkewAngleYEnabledProperty); }
            set { SetValue(SkewAngleYEnabledProperty, value); }
        }

        /// <summary>
        /// 
        /// </summary>
        public static readonly DependencyProperty SkewAngleYEnabledProperty =
            DependencyProperty.Register("SkewAngleYEnabled", typeof(bool), typeof(Carousel), new PropertyMetadata(false, (s, a) => ((Carousel)s).OnItemsVisualChanged(s)));

        /// <summary>
        /// Gets or sets the skew angle Y fractions.
        /// </summary>
        /// <value>The skew angle Y fractions.</value>
        public PathFractionCollection SkewAngleYFractions
        {
            get { return (PathFractionCollection)GetValue(SkewAngleYFractionsProperty); }
            set { SetValue(SkewAngleYFractionsProperty, value); }
        }

        /// <summary>
        /// 
        /// </summary>
        public static readonly DependencyProperty SkewAngleYFractionsProperty =
            DependencyProperty.Register("SkewAngleYFractions", typeof(PathFractionCollection), typeof(Carousel), new UIPropertyMetadata(null, (s, a) => ((Carousel)s).OnItemsVisualChanged(s)));
        /// <summary>
        /// 
        /// </summary>
        public double RadiusX
        {
            get { return (double)GetValue(RadiusXProperty); }
            set { SetValue(RadiusXProperty, value); }
        }

        // Using a DependencyProperty as the backing store for RadiusX.  This enables animation, styling, binding, etc...
        /// <summary>
        /// 
        /// </summary>
        public static readonly DependencyProperty RadiusXProperty =
            DependencyProperty.Register("RadiusX", typeof(double), typeof(Carousel), new UIPropertyMetadata(250.0));


        /// <summary>
        /// 
        /// </summary>
        public double RadiusY
        {
            get { return (double)GetValue(RadiusYProperty); }
            set { SetValue(RadiusYProperty, value); }
        }

        // Using a DependencyProperty as the backing store for RadiusY.  This enables animation, styling, binding, etc...
        /// <summary>
        /// 
        /// </summary>
        public static readonly DependencyProperty RadiusYProperty =
            DependencyProperty.Register("RadiusY", typeof(double), typeof(Carousel), new UIPropertyMetadata(150.0));


        /// <summary>
        /// 
        /// </summary>
        private bool IsVisualChanged = false;
        /// <summary>
        /// Called when [items visual changed].
        /// </summary>
        /// <param name="obj">The obj.</param>
        protected void OnItemsVisualChanged(DependencyObject obj)
        {
            if (this.ItemsHost != null)
            {
                this.SetVisualProperties();
                this.ItemsHost.InvalidateMeasure();
                CustomPathCarouselPanel panel = (CustomPathCarouselPanel)this.ItemsHost;
                panel.ItemsPerPage = this.ItemsPerPage;
                if (TopItemPosition >= 1)
                {
                    panel.TopItemPosition = this.TopItemPosition;
                    panel.InvalidateMeasure();
                   
                }              
            }
            else
            {
                IsVisualChanged = true;
            }
        }
        /// <summary>
        /// 
        /// </summary>
        public double OpacityFraction
        {
            get { return (double)GetValue(OpacityFractionProperty); }
            set { SetValue(OpacityFractionProperty, value); }
        }

        // Using a DependencyProperty as the backing store for OpacityFraction.  This enables animation, styling, binding, etc...
        /// <summary>
        /// 
        /// </summary>
        public static readonly DependencyProperty OpacityFractionProperty =
            DependencyProperty.Register("OpacityFraction", typeof(double), typeof(Carousel), new UIPropertyMetadata(0.0));


        /// <summary>
        /// 
        /// </summary>
        public double ScaleFraction
        {
            get { return (double)GetValue(ScaleFractionProperty); }
            set { SetValue(ScaleFractionProperty, value); }
        }

        // Using a DependencyProperty as the backing store for ScaleFraction.  This enables animation, styling, binding, etc...
        /// <summary>
        /// 
        /// </summary>
        public static readonly DependencyProperty ScaleFractionProperty =
            DependencyProperty.Register("ScaleFraction", typeof(double), typeof(Carousel), new UIPropertyMetadata(0.0));

        /// <summary>
        /// 
        /// </summary>
        public double RotationAngle
        {
            get { return (double)GetValue(RotationAngleProperty); }
            set { SetValue(RotationAngleProperty, value); }
        }

        // Using a DependencyProperty as the backing store for RotationAngle.  This enables animation, styling, binding, etc...
        /// <summary>
        /// 
        /// </summary>
        public static readonly DependencyProperty RotationAngleProperty =
            DependencyProperty.Register("RotationAngle", typeof(double), typeof(Carousel), new UIPropertyMetadata(0.0, new PropertyChangedCallback(OnRationAngleChanged)));

       
        /// <summary>
        /// 
        /// </summary>
        public double RotationSpeed
        {
            get { return (double)GetValue(RotationSpeedProperty); }
            set { SetValue(RotationSpeedProperty, value); }
        }

        // Using a DependencyProperty as the backing store for RotationSpeed.  This enables animation, styling, binding, etc...
        /// <summary>
        /// 
        /// </summary>
        public static readonly DependencyProperty RotationSpeedProperty =
            DependencyProperty.Register("RotationSpeed", typeof(double), typeof(Carousel), new UIPropertyMetadata(200.0));





        private static void OnRationAngleChanged(DependencyObject sender, DependencyPropertyChangedEventArgs args)
        {
            Carousel control = sender as Carousel;
            if (control != null)
            {
                RotateTransform transform = new RotateTransform();
                transform.Angle = (double)args.NewValue;
                control.RenderTransform = transform;
            }
        }

        
        /// <summary>
        /// Sets the visual properties.
        /// </summary>
        private void SetVisualProperties()
        {
            CustomPathCarouselPanel pane = (CustomPathCarouselPanel)this.ItemsHost;
            if (pane != null)
            {
                pane.ScalingEnabled = this.ScalingEnabled;
                pane.ScaleFractions = this.ScaleFractions;

                pane.OpacityEnabled = this.OpacityEnabled;
                pane.OpacityFractions = this.OpacityFractions;

                pane.SkewAngleXEnabled = this.SkewAngleXEnabled;
                pane.SkewAngleXFractions = this.SkewAngleXFractions;

                pane.SkewAngleYEnabled = this.SkewAngleYEnabled;
                pane.SkewAngleYFractions = this.SkewAngleYFractions;
            }
        }

        /// <summary>
        /// Gets or sets the top item position.
        /// </summary>
        /// <value>The top item position.</value>
        public double TopItemPosition
        {
            get { return (double)GetValue(TopItemPositionProperty); }
            set { SetValue(TopItemPositionProperty, value); }
        }

        /// <summary>
        /// 
        /// </summary>
        public static readonly DependencyProperty TopItemPositionProperty =
            DependencyProperty.Register("TopItemPosition", typeof(double), typeof(Carousel), new PropertyMetadata(0.5, (s, a) => ((Carousel)s).OnTopItemPositionChanged(s)));

        /// <summary>
        /// Called when [top item position changed].
        /// </summary>
        /// <param name="obj">The obj.</param>
        protected void OnTopItemPositionChanged(DependencyObject obj)
        {
            
        }

        private Path _Path;
        /// <summary>
        /// Gets or sets the path.
        /// </summary>
        /// <value>The path.</value>
        public Path Path
        {
            get { return _Path; }
            set
            {
                _Path = value;
                OnPathChanged();
            }
        }

        /// <summary>
        /// Called when [path changed].
        /// </summary>
        protected void OnPathChanged()
        {
            if (this.ItemsHost != null)
            {
                CustomPathCarouselPanel panel = (CustomPathCarouselPanel)this.ItemsHost;
                if (panel != null)
                {
                    panel.Path = this.Path;
                    panel.Invalidate(false);
                }
            }
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="Carousel"/> class.
        /// </summary>
        public Carousel()
        {
            if (EnvironmentTest.IsSecurityGranted)
            {
                EnvironmentTest.StartValidateLicense(typeof(Carousel));
            }
            DefaultStyleKey = typeof(Carousel);
            LayoutUpdated += new EventHandler(Carousel_LayoutUpdated);
        }

#if WPF
        /// <summary>
        /// Initializes the <see cref="Carousel"/> class.
        /// </summary>
        static Carousel()
        {
            EnvironmentTest.ValidateLicense(typeof(Carousel));
        }
#endif
        Panel _itemsHost;
        /// <summary>
        /// Get the current ItemsHost (FlowPanel)
        /// </summary>
        /// <value>The items host.</value>
        internal Panel ItemsHost
        {
            get
            {
                if (_itemsHost == null && ItemContainerGenerator != null)
                {                    
                    _itemsHost = VisualUtils.GetItemsPanel(this, typeof(CustomPathCarouselPanel));
                    if (_itemsHost != null)
                    {
                        (_itemsHost as CustomPathCarouselPanel).Owner = this;
                    }
                }
                return _itemsHost;
            }
        }

        internal Panel HostPanel
        {
            get
            {
                CarouselPanel panel = VisualUtils.FindDescendant(this, typeof(CarouselPanel)) as CarouselPanel;
                return panel;
            }
        }
        /// <summary>
        /// 
        /// </summary>
        public bool EnableVirtualization
        {
            get { return (bool)GetValue(EnableVirtualizationProperty); }
            set { SetValue(EnableVirtualizationProperty, value); }
        }

        // Using a DependencyProperty as the backing store for EnableVirtualization.  This enables animation, styling, binding, etc...
        /// <summary>
        /// 
        /// </summary>
        public static readonly DependencyProperty EnableVirtualizationProperty =
            DependencyProperty.Register("EnableVirtualization", typeof(bool), typeof(Carousel), new UIPropertyMetadata(false));

        /// <summary>
        /// Handles the LayoutUpdated event of the Carousel control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
        void Carousel_LayoutUpdated(object sender, EventArgs e)
        {
            if (this.ItemsHost != null)
            {
                if (this.IsVisualChanged)
                {
                    this.SetVisualProperties();
                    this.InvalidateMeasure();
                    this.IsVisualChanged = false;
                }

                CustomPathCarouselPanel panel = (CustomPathCarouselPanel)this.ItemsHost;
                if (panel != null)
                {
                    if (this.Path != null)
                    {
                        panel.Path = this.Path;
                    }
                    if (this.TopItemPosition != panel.TopItemPosition)
                    {
                        panel.TopItemPosition = this.TopItemPosition;
                    }
                    if (panel.ItemsPerPage <= 0)
                    {
                        if (this.ItemsPerPage == -1 || (this.Items.Count <= this.ItemsPerPage && this.Items.Count > 0))
                        {
                            panel.ItemsPerPage = this.Items.Count;
                        }
                        else
                        {
                            panel.ItemsPerPage = this.ItemsPerPage;
                        }
                    }
                }
                LayoutUpdated -= Carousel_LayoutUpdated;
            }
        }

        /// <summary>
        /// Gets or sets the selected item.
        /// </summary>
        /// <value>The selected item.</value>
        public object SelectedItem
        {
            get { return (object)GetValue(SelectedItemProperty); }
            set { SetValue(SelectedItemProperty, value); }
        }

        /// <summary>
        /// 
        /// </summary>
        public static readonly DependencyProperty SelectedItemProperty =
            DependencyProperty.Register("SelectedItem", typeof(object), typeof(Carousel), new PropertyMetadata(null, OnSelectedItemChanged));

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
            if (VisualMode == Shared.VisualMode.Standard)
            {
                if (this.HostPanel != null)
                {
                    if (HostPanel is CarouselPanel)
                    {
                        CarouselItem item = null;
                        item = this.ItemContainerGenerator.ContainerFromItem(this.SelectedItem) as CarouselItem;
                        if (item != null)
                        {
                            SelectedValue = item.Content;
                            SelectedItem = item.Content;
                        }
                        ((CarouselPanel)HostPanel).SelectElement(item);
                        ((CarouselPanel)HostPanel).currentIndex = ((CarouselPanel)HostPanel).GetSelecteItem(item);
                    }
                }
            }
            else if (VisualMode == Shared.VisualMode.CustomPath)
            {
                if (args.OldValue != null)
                {
                    CarouselItem item = null;
                    if (args.OldValue is CarouselItem)
                        item = args.OldValue as CarouselItem;
                    else if (this.ItemContainerGenerator.ContainerFromItem(args.OldValue) is CarouselItem)
                        item = this.ItemContainerGenerator.ContainerFromItem(args.OldValue) as CarouselItem;
                    if(item!=null)
                        item.IsSelected = false;
                }
                if (this.SelectedItem == null)
                {
                    SelectedIndex = -1;
                    SelectedValue = null;
                }
                else
                {
                    int index = this.Items.IndexOf(this.SelectedItem);

                    if (index != -1)
                    {
                        this.SelectedIndex = index;
                        this.SelectedValue = this.SelectedItem;
                    }
                }

                CarouselItem selectedContainer = null;
                if (this.SelectedItem is CarouselItem)
                    selectedContainer = args.NewValue as CarouselItem;
                else if (this.ItemContainerGenerator.ContainerFromItem(args.NewValue) is CarouselItem)
                    selectedContainer = this.ItemContainerGenerator.ContainerFromItem(args.NewValue) as CarouselItem;



                if (this.ItemsHost != null && selectedContainer != null)
                {
                   ((CustomPathCarouselPanel)this.ItemsHost).FinishItemMovements();
                    ((CustomPathCarouselPanel)this.ItemsHost).BringItemIntoView((UIElement)selectedContainer, true);
                }
               else if (this.ItemsHost != null)
               {
                    int m_oldindex = this.Items.IndexOf(args.OldValue);
                    int m_newindex = this.Items.IndexOf(args.NewValue);                   
                    ((CustomPathCarouselPanel)this.ItemsHost).FinishItemMovements();
                    ((CustomPathCarouselPanel)this.ItemsHost).MoveItemInternallyforoutofrange((m_newindex - m_oldindex));                  
               }
            }
            if (SelectionChanged != null)
            {
                SelectionChanged(this, args);
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
        /// Gets or sets the index of the selected
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
            DependencyProperty.Register("SelectedIndex", typeof(int), typeof(Carousel), new FrameworkPropertyMetadata(-1,FrameworkPropertyMetadataOptions.BindsTwoWayByDefault,(s, a) => ((Carousel)s).OnSelectedIndexChanged(a)));
        /// <summary>
        /// 
        /// </summary>
        public VisualMode VisualMode
        {
            get { return (VisualMode)GetValue(VisualModeProperty); }
            set { SetValue(VisualModeProperty, value); }
        }

        // Using a DependencyProperty as the backing store for VisualMode.  This enables animation, styling, binding, etc...
        /// <summary>
        /// 
        /// </summary>
        public static readonly DependencyProperty VisualModeProperty =
            DependencyProperty.Register("VisualMode", typeof(VisualMode), typeof(Carousel), new UIPropertyMetadata(VisualMode.Standard));

        /// <summary>
        /// Raises the <see cref="E:SelectedIndexChanged"/> event.
        /// </summary>
        /// <param name="args">The <see cref="System.Windows.DependencyPropertyChangedEventArgs"/> instance containing the event data.</param>
        protected virtual void OnSelectedIndexChanged(DependencyPropertyChangedEventArgs args)
        {
            if (SelectedIndex < 0)
            {
                SelectedItem = null;
            }
            else if(SelectedIndex < this.Items.Count)
            {
                this.SelectedItem = this.Items[this.SelectedIndex];
            }
            
            if (this.SelectedIndexChanged != null)
            {
                this.SelectedIndexChanged(this, args);
            }
        }

        #region Overrides

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
        /// 
        /// </summary>
        /// <param name="oldValue"></param>
        /// <param name="newValue"></param>
        protected override void OnItemsSourceChanged(System.Collections.IEnumerable oldValue, System.Collections.IEnumerable newValue)
        {
            base.OnItemsSourceChanged(oldValue, newValue);
            if (this.ItemsHost != null)
            {
                this.SelectedItem = null;
                
            }
        }

        /// <summary>
        /// Raises on Items Changed
        /// </summary>
        /// <param name="e">NotifyCollection event args</param>
        protected override void OnItemsChanged(System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
        {
            base.OnItemsChanged(e);
            if (ItemsHost!=null && IsLoaded && e.Action == System.Collections.Specialized.NotifyCollectionChangedAction.Reset)
            {
                (this.ItemsHost as CustomPathCarouselPanel).Refresh(false, 0);
            }
        }

        /// <summary>
        /// Prepares the specified element to display the specified item.
        /// </summary>
        /// <param name="element">Element used to display the specified item.</param>
        /// <param name="item">Specified item.</param>
        protected override void PrepareContainerForItemOverride(DependencyObject element, object item)
        {
            CarouselItem cItem = element as CarouselItem;
            if (cItem != null)
            {
             cItem.Owner = this;
            }
            base.PrepareContainerForItemOverride(element, item);
        }
        /// <summary>
        /// 
        /// </summary>
        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();
        }

        #endregion

#if SyncfusionFramework4_0
        /// <summary>
        /// 
        /// </summary>
        public bool EnableTouch
        {
            get { return (bool)GetValue(EnableTouchProperty); }
            set { SetValue(EnableTouchProperty, value); }
        }

        // Using a DependencyProperty as the backing store for EnableTouch.  This enables animation, styling, binding, etc...
        /// <summary>
        /// 
        /// </summary>
        public static readonly DependencyProperty EnableTouchProperty =
            DependencyProperty.Register("EnableTouch", typeof(bool), typeof(Carousel), new PropertyMetadata(false));
        

        TranslateTransform transform =new TranslateTransform();
        /// <summary>
        /// 
        /// </summary>
        /// <param name="e"></param>
        protected override void OnManipulationStarting(ManipulationStartingEventArgs e)
        {
            if (EnableTouch)
            {
                e.ManipulationContainer = this;
                this.RenderTransform = transform;
                e.Handled = true;
                base.OnManipulationStarting(e);
            }
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="e"></param>
        protected override void OnManipulationDelta(ManipulationDeltaEventArgs e)
        {
            if (EnableTouch)
            {
                if (Math.Abs(e.CumulativeManipulation.Translation.Y) >= 30d)
                    RadiusY = RadiusY - (e.CumulativeManipulation.Translation.Y % 10);
                if (Math.Abs(e.CumulativeManipulation.Translation.X) >= 30d)
                {
                    if (HostPanel is CarouselPanel)
                    {

                        (HostPanel as CarouselPanel)._currentRotation = ((e.CumulativeManipulation.Translation.X % 360));
                        (HostPanel as CarouselPanel).InvalidateArrange();
                    }
                }
                base.OnManipulationDelta(e);
            }
        }     
       
#endif
    }
    /// <summary>
    /// 
    /// </summary>
    public enum VisualMode
    {
        /// <summary>
        /// 
        /// </summary>
        Standard,
        /// <summary>
        /// 
        /// </summary>
        CustomPath
    }
}
