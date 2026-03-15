#region Copyright
// Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Any infringement will be prosecuted under
// applicable laws. 
#endregion

namespace Syncfusion.Windows.Gauge
{
    using System;
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using System.Collections.Specialized;
    using System.Linq;
    using System.Text;
    using System.Windows;
    using System.Windows.Controls;
    using System.Windows.Documents;
    using System.Windows.Media;
    using System.Windows.Shapes;

    /// <summary>
    /// Represents Base class for Gauge control.
    /// </summary>
    public class GaugeBase : Control
    {
        #region Dependency properties

        /// <summary>
        /// Identifies the <see cref="InnerFrameBrush"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty InnerFrameBrushProperty =
            DependencyProperty.Register("InnerFrameBrush", typeof(Brush), typeof(GaugeBase), new PropertyMetadata(new PropertyChangedCallback(OnInnerFrameBrushChanged)));

        /// <summary>
        /// Identifies the <see cref="InnerFrameOffset"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty InnerFrameOffsetProperty =
            DependencyProperty.Register("InnerFrameOffset", typeof(double), typeof(GaugeBase), new PropertyMetadata(7d, new PropertyChangedCallback(OnInnerFrameOffsetChanged)));

        /// <summary>
        /// Identifies the <see cref="MiddleFrameBrush"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty MiddleFrameBrushProperty =
            DependencyProperty.Register("MiddleFrameBrush", typeof(Brush), typeof(GaugeBase), new PropertyMetadata(new PropertyChangedCallback(OnMiddleFrameBrushChanged)));

        /// <summary>
        /// Identifies the <see cref="MiddleFrameOffset"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty MiddleFrameOffsetProperty =
            DependencyProperty.Register("MiddleFrameOffset", typeof(double), typeof(GaugeBase), new PropertyMetadata(7d, new PropertyChangedCallback(OnMiddleFrameOffsetChanged)));

        /// <summary>
        /// Identifies the <see cref="OuterFrameBrush"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty OuterFrameBrushProperty =
            DependencyProperty.Register("OuterFrameBrush", typeof(Brush), typeof(GaugeBase), new PropertyMetadata(new PropertyChangedCallback(OnOuterFrameBrushChanged)));

        /// <summary>
        /// Identifies the <see cref="OuterFrameOffset"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty OuterFrameOffsetProperty =
            DependencyProperty.Register("OuterFrameOffset", typeof(double), typeof(GaugeBase), new PropertyMetadata(2d, new PropertyChangedCallback(OnOuterFrameOffsetChanged)));

        /// <summary>
        /// Identifies the <see cref="ShowDigitalValue"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty ShowDigitalValueProperty = DependencyProperty.Register("ShowDigitalValue", typeof(bool), typeof(GaugeBase), new PropertyMetadata(false, new PropertyChangedCallback(IsShowDigitalValueChanged)));

        /// <summary>
        /// Identifies the <see cref="DigitalValue"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty DigitalValueProperty =
           DependencyProperty.Register("DigitalValue", typeof(DigitalGauge), typeof(GaugeBase), new PropertyMetadata(null, new PropertyChangedCallback(OnDigitalValueChanged)));

        /// <summary>
        /// Identifies the <see cref="DigitalValue"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty EnableEffectsProperty =
           DependencyProperty.Register("EnableEffects", typeof(bool), typeof(GaugeBase), new PropertyMetadata(true, new PropertyChangedCallback(IsEnableEffectsChanged)));
        /// <summary>
        /// Identifies the <see cref="VisualStyle"/> dependency property.
        /// </summary>
        // Using a DependencyProperty as the backing store for VisualStyle.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty VisualStyleProperty =
            DependencyProperty.Register("VisualStyle", typeof(GaugeVisualStyle), typeof(GaugeBase), new PropertyMetadata(GaugeVisualStyle.Default, new PropertyChangedCallback(OnVisualStyleChanged)));

        
        #endregion

        #region Class members

        #endregion

        #region Private Members

        /// <summary>
        /// Collection of visual children.
        /// </summary>
        private VisualChildrenCollection<Control> mchildren;

        /// <summary>
        /// Collection of custom labels.
        /// </summary>
        private CustomLabelsCollection mcustomLabels;

        /// <summary>
        /// First frame.
        /// </summary>
        private GaugeBorder mfirstCircleBorder;


        /// <summary>
        /// Middle frame.
        /// </summary>
        private GaugeBorder mmiddleCircleBorder;

        /// <summary>
        /// Gray tint of the first frame.
        /// </summary>
        private Color mfirstFrameColor = Color.FromArgb(255, 149, 149, 149);

        /// <summary>
        /// Collection of images.
        /// </summary>
        private ImagesCollection mimages;

        /// <summary>
        /// Inner frame.
        /// </summary>
        private GaugeBorder minnerCircleBorder;

        /// <summary>
        /// Inner path.
        /// </summary>
        private Path minnerPath;

        /// <summary>
        /// Center gray tint of the inner frame.
        /// </summary>
        private Color minnerEndColor = Color.FromArgb(255, 140, 140, 140);

        /// <summary>
        /// Edge gray tint of the inner frame.
        /// </summary>
        private Color minnerStartColor = Color.FromArgb(255, 180, 180, 180);

        /// <summary>
        /// Second frame.
        /// </summary>
        private GaugeBorder msecondCircleBorder;

        /// <summary>
        /// End gray tint of the second frame.
        /// </summary>
        private Color msecondFrameEndColor = Color.FromArgb(255, 178, 178, 178);

        /// <summary>
        /// Start gray tint of the second frame.
        /// </summary>
        private Color msecondFrameStartColor = Color.FromArgb(255, 109, 109, 109);

        /// <summary>
        /// Collection of state indicators.
        /// </summary>
        private StateIndicatorsCollection mstateIndicators;

        #endregion

        #region Initialization

        /// <summary>
        /// Initializes a new instance of the <see cref="GaugeBase"/> class.
        /// </summary>
        public GaugeBase()
        {
            this.ChildrenCollection = new VisualChildrenCollection<Control>();
            this.StateIndicators = new StateIndicatorsCollection();
            this.CustomLabels = new CustomLabelsCollection();
            this.Images = new ImagesCollection();
            this.StateIndicators.CollectionChanged += new NotifyCollectionChangedEventHandler(this.CollectionChanged);
            this.CustomLabels.CollectionChanged += new NotifyCollectionChangedEventHandler(this.CollectionChanged);
            this.Images.CollectionChanged += new NotifyCollectionChangedEventHandler(this.CollectionChanged);

            this.Loaded += new RoutedEventHandler(this.GaugeBaseLoaded);
            this.SizeChanged += new SizeChangedEventHandler(this.GaugeBaseSizeChanged);
        }

        #endregion

        #region Events
        /// <summary>
        /// Event that is raised when <see cref="InnerFrameBrush"/> property is changed.
        /// </summary>
        public event PropertyChangedCallback EnableEffectsChanged;

       
        /// <summary>
        /// Event that is raised when <see cref="InnerFrameOffset"/> property is changed.
        /// </summary>
        public event PropertyChangedCallback InnerFrameOffsetChanged;

        
        /// <summary>
        /// Event that is raised when <see cref="MiddleFrameOffset"/> property is changed.
        /// </summary>
        public event PropertyChangedCallback MiddleFrameOffsetChanged;

        

        /// <summary>
        /// Event that is raised when <see cref="OuterFrameOffset"/> property is changed.
        /// </summary>
        public event PropertyChangedCallback OuterFrameOffsetChanged;

        /// <summary>
        /// Event that is raised when ShowDigtialValue property is changed.
        /// </summary>
        public event PropertyChangedCallback ShowDigtialValueChanged;

        #endregion

        #region DP getters & setters

        /// <summary>
        /// Gets or sets the collection of custom labels.
        /// </summary>
        /// <value>
        /// Type: <see cref="CustomLabelsCollection"/>
        /// </value>
        /// <seealso cref="CustomLabelsCollection"/>
        public CustomLabelsCollection CustomLabels
        {
            get
            {
                return this.mcustomLabels;
            }

            set
            {
                this.mcustomLabels = value;
            }
        }

        /// <summary>
        /// Gets or sets the collection of images.
        /// </summary>
        /// <value>
        /// Type: <see cref="ImagesCollection"/>
        /// </value>
        /// <seealso cref="ImagesCollection"/>
        public ImagesCollection Images
        {
            get
            {
                return this.mimages;
            }

            set
            {
                this.mimages = value;
            }
        }

        /// <summary>
        /// Gets or sets the inner frame brush
        /// </summary>
        public Brush InnerFrameBrush
        {
            get
            {
                return (Brush)GetValue(InnerFrameBrushProperty);
            }

            set
            {
                SetValue(InnerFrameBrushProperty, value);
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether the effects are enabled or not
        /// </summary>
        public bool EnableEffects
        {
            get
            {
                return (bool)GetValue(EnableEffectsProperty);
            }

            set
            {
                SetValue(EnableEffectsProperty, value);
            }
        }

        /// <summary>
        /// Gets or sets the inner frame offset
        /// </summary>
        public double InnerFrameOffset
        {
            get
            {
                return (double)GetValue(InnerFrameOffsetProperty);
            }

            set
            {
                SetValue(InnerFrameOffsetProperty, value);
            }
        }

        /// <summary>
        /// Gets or sets Visual Style.
        /// </summary>
        /// <value></value>
        /// <remarks></remarks>
        public GaugeVisualStyle VisualStyle
        {
            get { return (GaugeVisualStyle)GetValue(VisualStyleProperty); }
            set
            {
                SetValue(VisualStyleProperty, value);
            }
        }
        /// <summary>
        /// Gets or sets the inner frame brush
        /// </summary>
        public Brush MiddleFrameBrush
        {
            get
            {
                return (Brush)GetValue(MiddleFrameBrushProperty);
            }

            set
            {
                SetValue(MiddleFrameBrushProperty, value);
            }
        }

        /// <summary>
        /// Gets or sets the inner frame offset
        /// </summary>
        public double MiddleFrameOffset
        {
            get
            {
                return (double)GetValue(MiddleFrameOffsetProperty);
            }

            set
            {
                SetValue(MiddleFrameOffsetProperty, value);
            }
        }

        /// <summary>
        /// Gets or sets the outer Frame brush
        /// </summary>
        public Brush OuterFrameBrush
        {
            get
            {
                return (Brush)GetValue(OuterFrameBrushProperty);
            }

            set
            {
                SetValue(OuterFrameBrushProperty, value);
            }
        }

        /// <summary>
        /// Gets or sets the Outer frame offset
        /// </summary>
        public double OuterFrameOffset
        {
            get
            {
                return (double)GetValue(OuterFrameOffsetProperty);
            }

            set
            {
                SetValue(OuterFrameOffsetProperty, value);
            }
        }

        /// <summary>
        /// Gets or sets the collection of state indicators.
        /// </summary>
        /// <value>
        /// Type: <see cref="StateIndicatorsCollection"/>
        /// </value>
        /// <seealso cref="StateIndicatorsCollection"/>
        public StateIndicatorsCollection StateIndicators
        {
            get
            {
                return this.mstateIndicators;
            }

            set
            {
                this.mstateIndicators = value;
            }
        }

        /// <summary>
        /// Gets or sets the the digital gauge to host inside the gauge. 
        /// </summary>
        /// <value>
        /// Type: <see cref="object"/>
        /// Default value is null.
        /// </value>
        public DigitalGauge DigitalValue
        {
            get
            {
                return (DigitalGauge)GetValue(DigitalValueProperty);
            }

            set
            {
                SetValue(DigitalValueProperty, value);
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether to show Digital value or not
        /// </summary>
        public bool ShowDigitalValue
        {
            get
            {
                return (bool)GetValue(ShowDigitalValueProperty);
            }

            set
            {
                SetValue(ShowDigitalValueProperty, value);
            }
        }

        /// <summary>
        /// Gets or sets the collection of visual children.
        /// </summary>
        internal VisualChildrenCollection<Control> ChildrenCollection
        {
            get
            {
                return this.mchildren;
            }

            set
            {
                this.mchildren = value;
            }
        }

        /// <summary>
        /// Gets the Second Circle Border
        /// </summary>
        /// <value>
        /// Type: <see cref="Border"/>
        /// </value>
        /// <seealso cref="Border"/>
        internal GaugeBorder SecondCircleBorder
        {
            get
            {
                return this.msecondCircleBorder;
            }
        }

        /// <summary>
        /// Gets First CircleBorder
        /// </summary>
        /// <value>
        /// Type: <see cref="Border"/>
        /// </value>
        /// <seealso cref="Border"/>
        internal GaugeBorder FirstCircleBorder
        {
            get
            {
                return this.mfirstCircleBorder;
            }
        }

        internal GaugeBorder MiddleCircleBorder
        {
            get
            {
                return this.mmiddleCircleBorder;
            }
        }

        /// <summary>
        /// Gets Inner circle border
        /// </summary>
        /// <value>
        /// Type: <see cref="Border"/>
        /// </value>
        /// <seealso cref="Border"/>
        internal GaugeBorder InnerCircleBorder
        {
            get
            {
                return this.minnerCircleBorder;
            }
        }

        #endregion

        #region Overrides

        /// <summary>
        /// Builds the current template's visual tree if necessary.
        /// </summary>
        public override void OnApplyTemplate()
        {
            this.mfirstCircleBorder = this.GetTemplateChild("PART_FirstBorder") as GaugeBorder;
            this.msecondCircleBorder = this.GetTemplateChild("PART_SecondBorder") as GaugeBorder;
            this.mmiddleCircleBorder = this.GetTemplateChild("PART_MiddleBorder") as GaugeBorder;
            this.minnerCircleBorder = this.GetTemplateChild("PART_InnerBorder") as GaugeBorder;
            this.minnerPath = this.GetTemplateChild("PART_GlassPath") as Path;

            base.OnApplyTemplate();
        }

        #endregion

        #region Implementation

        /// <summary>
        /// Merges two color channels.
        /// </summary>
        /// <param name="baseChannel">Base Channel to merge.</param>
        /// <param name="blendChannel">Blend Channel to merge.</param>
        /// <returns>Merged channels.</returns>
        internal byte MergeChannels(int baseChannel, int blendChannel)
        {
            int mediana, dif, rest, max = 255;

            mediana = baseChannel * blendChannel / max;
            dif = ((max - baseChannel) * (max - blendChannel)) / max;
            rest = baseChannel * (max - dif - mediana);

            return (byte)(mediana + (rest / 255));
        }

        /// <summary>
        /// Merges two colors.
        /// </summary>
        /// <param name="color">Color to merge.</param>
        /// <param name="blendColor">Blend Color to merge.</param>
        /// <returns>Merged colors.</returns>
        internal Color MergeChannels(Color color, Color blendColor)
        {
            return Color.FromArgb(
                color.A, this.MergeChannels(color.R, blendColor.R), this.MergeChannels(color.G, blendColor.G), this.MergeChannels(color.B, blendColor.B));
        }

        /// <summary>
        /// To do it should refresh the scale panel
        /// </summary>
        internal virtual void RefreshScalesPanel()
        {
            // TODO
        }

        /// <summary>
        /// Called to arrange and size the content of a control.
        /// </summary>
        /// <param name="arrangeBounds">The computed size that is used to arrange the content.</param>
        /// <returns>The size of the control.</returns>
        protected override Size ArrangeOverride(Size arrangeBounds)
        {
            this.UpdateChildrenLocation();
            return base.ArrangeOverride(arrangeBounds);
        }

        /// <summary>
        /// Invoked when Location property of a child is changed.
        /// </summary>
        /// <param name="sender">The <see cref="CircularGauge"/> object.</param>
        /// <param name="e">The <see cref="System.Windows.DependencyPropertyChangedEventArgs"/> that contains the event data.</param>
        protected void ChildLocationChanged(DependencyObject sender, DependencyPropertyChangedEventArgs e)
        {
            this.UpdateChildrenLocation();
        }

        /// <summary>
        /// Occurs when an item is added, removed, changed, moved, or the entire collection is refreshed.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.Collections.Specialized.NotifyCollectionChangedEventArgs"/> that contains the event data.</param>
        protected virtual void CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            if (e.NewItems != null)
            {
                int count = e.NewItems.Count;
                if (e.Action == NotifyCollectionChangedAction.Add)
                {
                    for (int i = 0; i < count; i++)
                    {
                        if (e.NewItems[i] is LocalizableGaugeElement)
                        {
                            LocalizableGaugeElement elem = e.NewItems[i] as LocalizableGaugeElement;
                            if (elem != null)
                            {
                                int index = this.ChildrenCollection.Count - 1 >= 0 ? this.ChildrenCollection.Count - 1 : 0;
                                this.ChildrenCollection.Insert(index, elem);
                                elem.LocationChanged += new PropertyChangedCallback(this.ChildLocationChanged);
                                elem.GaugeElementParent = this;
                            }
                        }
                        else
                        {
                            Control elem = e.NewItems[i] as Control;
                            if (elem != null)
                            {
                                this.ChildrenCollection.Insert(0, elem);
                            }
                        }
                    }
                }
            }
            else if (e.OldItems != null)
            {
                int count = e.OldItems.Count;
                if (e.Action == NotifyCollectionChangedAction.Remove)
                {
                    for (int i = 0; i < count; i++)
                    {
                        Control elem = e.OldItems[i] as Control;
                        if (elem != null && this.ChildrenCollection.Contains(elem))
                        {
                            this.ChildrenCollection.Remove(elem);
                        }
                    }
                }
            }
            else if (e.Action == NotifyCollectionChangedAction.Reset)
            {
                //this.ChildrenCollection.Clear();
            }
        }

        /// <summary>
        /// Converts point from range (0,0)-(100,100) to control bounds.
        /// </summary>
        /// <param name="p">Point to convert.</param>
        /// <param name="width">Width of the gauge.</param>
        /// <param name="height">Height of the gauge.</param>
        /// <returns>Converted point.</returns>
        protected virtual Point ConvertLocation(Point p, double width, double height)
        {
            double x = 0;
            double y = 0;
            double ratioX = 0;
            double ratioY = 0;
            double gwidth = width;
            double gheight = height;

            if (p.X < 50)
            {
                ratioX = p.X / 50;
                x = ((gwidth / 2) * ratioX) - (gwidth / 2);
            }
            else
            {
                ratioX = (p.X - 50) / 50;
                x += (gwidth / 2) * ratioX;
            }

            if (p.Y < 50)
            {
                ratioY = p.Y / 50;
                y = ((gheight / 2) * ratioY) - (gheight / 2);
            }
            else
            {
                ratioY = (p.Y - 50) / 50;
                y += (gheight / 2) * ratioY;
            }

            return new Point(x, y);
        }

        /// <summary>
        /// Converts point from range (0,0)-(100,100) to control bounds.
        /// </summary>
        /// <param name="p">Point to convert.</param>
        /// <returns>Converted point.</returns>
        protected virtual Point ConvertLocation(Point p)
        {
            return this.ConvertLocation(p, this.ActualWidth, this.ActualHeight);
        }

        /// <summary>
        /// Invoked when the control is ready for presentation.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.Windows.RoutedEventArgs"/> that contains the event data.</param>
        protected virtual void GaugeBaseLoaded(object sender, RoutedEventArgs e)
        {
            this.ChildrenCollection.UpdatePanelChildren();
            this.RefreshBorders();
            this.UpdateChildrenLocation();
        }

        /// <summary>
        /// Updates the children location
        /// </summary>
        /// <param name="sender">sender that has raised the event </param>
        /// <param name="e">size event is rasied</param>
        protected void GaugeBaseSizeChanged(object sender, SizeChangedEventArgs e)
        {
            this.UpdateChildrenLocation();
        }

        /// <summary>
        /// Updates property value cache and raises InnerFrameBrushChanged event.
        /// </summary>
        /// <param name="e">
        /// Property change details, such as old value and new value.</param>
        protected virtual void OnInnerFrameBrushChanged(DependencyPropertyChangedEventArgs e)
        {
            this.RefreshBorders();

            //if (this.InnerFrameBrushChanged != null)
            //{
            //    this.InnerFrameBrushChanged(this, e);
            //}
        }

        /// <summary>
        /// Updates property value cache and raises InnerFrameOffsetChanged event.
        /// </summary>
        /// <param name="e">
        /// Property change details, such as old value and new value.</param>
        protected virtual void OnInnerFrameOffsetChanged(DependencyPropertyChangedEventArgs e)
        {
            this.RefreshBorders();
            if (this.InnerFrameOffsetChanged != null)
            {
                this.InnerFrameOffsetChanged(this, e);
            }
        }

        /// <summary>
        /// Updates property value cache and raises InnerFrameBrushChanged event.
        /// </summary>
        /// <param name="e">
        /// Property change details, such as old value and new value.</param>
        protected virtual void OnMiddleFrameBrushChanged(DependencyPropertyChangedEventArgs e)
        {
            this.RefreshBorders();
            //if (this.MiddleFrameBrushChanged != null)
            //{
            //    this.MiddleFrameBrushChanged(this, e);
            //}
        }

        /// <summary>
        /// Updates property value cache and raises InnerFrameOffsetChanged event.
        /// </summary>
        /// <param name="e">
        /// Property change details, such as old value and new value.</param>
        protected virtual void OnMiddleFrameOffsetChanged(DependencyPropertyChangedEventArgs e)
        {
            this.RefreshBorders();
            if (this.MiddleFrameOffsetChanged != null)
            {
                this.MiddleFrameOffsetChanged(this, e);
            }
        }

        /// <summary>
        /// Updates property value cache and raises OuterFrameBrushChanged event.
        /// </summary>
        /// <param name="e">
        /// Property change details, such as old value and new value.</param>
        protected virtual void OnOuterFrameBrushChanged(DependencyPropertyChangedEventArgs e)
        {
            this.RefreshBorders();
            //if (this.OuterFrameBrushChanged != null)
            //{
            //    this.OuterFrameBrushChanged(this, e);
            //}
        }

        /// <summary>
        /// Updates property value cache and raises OuterFrameBrushChanged event.
        /// </summary>
        /// <param name="e">
        /// Property change details, such as old value and new value.</param>
        protected virtual void OnOuterFrameOffsetChanged(DependencyPropertyChangedEventArgs e)
        {
            this.RefreshBorders();
            if (this.OuterFrameOffsetChanged != null)
            {
                this.OuterFrameOffsetChanged(this, e);
            }
        }

        /// <summary>
        ///  Calls when visual style changed
        /// </summary>
        /// <param name="d"></param>
        /// <param name="args">An <see cref="T:System.Windows.DependencyPropertyChangedEventArgs">DependencyPropertyChangedEventArgs</see> that contains the event data.</param>
        /// <remarks></remarks>
        public static void OnVisualStyleChanged(DependencyObject d, DependencyPropertyChangedEventArgs args)
        {
            GaugeBase instance = d as GaugeBase;
            instance.UpdateVisualStyle();

            instance.RefreshBorders(); 
            instance.InvalidateArrange();
        }
        /// <summary>
        /// updates visual style
        /// </summary>
        /// <remarks></remarks>
        protected virtual void UpdateVisualStyle()
        {
            ResourceDictionary _resources = this.GetResources(this.VisualStyle);
         
            string _key;
            if (this is CircularGauge)
            {
                _key = this.GetResourceKey("CircularGauge", this.VisualStyle);
                (this as CircularGauge).Style = _resources[_key] as Style;
            }
            else if (this is LinearGauge)
            {
                _key = this.GetResourceKey("LinearGauge", this.VisualStyle);
                (this as LinearGauge).Style = _resources[_key] as Style;
            }
            else if (this is DigitalGauge)
            {
                _key = this.GetResourceKey("DigitalGauge", this.VisualStyle);
                (this as DigitalGauge).Style = _resources[_key] as Style;
            }
           
        }

        private ResourceDictionary GetResources(GaugeVisualStyle style)
        {
            ResourceDictionary res = new ResourceDictionary();
            switch (style)
            {
                case GaugeVisualStyle.Blend:
                    res = new ResourceDictionary() { Source = new Uri("/Syncfusion.Gauge.Silverlight;component/themes/BlendStyle.xaml", UriKind.RelativeOrAbsolute) };
                    break;
                case GaugeVisualStyle.Metro:
                    res = new ResourceDictionary() { Source = new Uri("/Syncfusion.Gauge.Silverlight;component/themes/MetroStyle.xaml", UriKind.RelativeOrAbsolute) };
                    break;
                case GaugeVisualStyle.Office2003:
                    res = new ResourceDictionary() { Source = new Uri("/Syncfusion.Gauge.Silverlight;component/themes/Office2003Style.xaml", UriKind.RelativeOrAbsolute) };
                    break;
                case GaugeVisualStyle.Office2007Black:
                    res = new ResourceDictionary() { Source = new Uri("/Syncfusion.Gauge.Silverlight;component/themes/Office2007Black.xaml", UriKind.RelativeOrAbsolute) };
                    break;
                case GaugeVisualStyle.Office2007Blue:
                    res = new ResourceDictionary() { Source = new Uri("/Syncfusion.Gauge.Silverlight;component/themes/Office2007Blue.xaml", UriKind.RelativeOrAbsolute) };
                    break;
                case GaugeVisualStyle.Office2007Silver:
                    res = new ResourceDictionary() { Source = new Uri("/Syncfusion.Gauge.Silverlight;component/themes/Office2007Silver.xaml", UriKind.RelativeOrAbsolute) };
                    break;
                case GaugeVisualStyle.VS2010:
                    res = new ResourceDictionary() { Source = new Uri("/Syncfusion.Gauge.Silverlight;component/themes/VS2010Style.xaml", UriKind.RelativeOrAbsolute) };
                    break;
                case GaugeVisualStyle.Default:
                    res = new ResourceDictionary() { Source = new Uri("/Syncfusion.Gauge.Silverlight;component/themes/generic.xaml", UriKind.RelativeOrAbsolute) };
                    break;
            }
            return res;
        }

        private string GetResourceKey(string element, GaugeVisualStyle currentStyle)
        {
            return currentStyle + element + "Style";
        }
        /// <summary>
        /// Updates gauge frames.
        /// </summary>
        protected virtual void RefreshBorders()
        {
            if (this.mfirstCircleBorder != null)
            {
                if (this.OuterFrameBrush != null)
                {
                    this.mfirstCircleBorder.Background = this.OuterFrameBrush;
                }
                else
                {
                    this.mfirstCircleBorder.Background = this.Background;
                }

                this.mfirstCircleBorder.OpacityMask = this.OpacityMask;
                this.mfirstCircleBorder.Opacity = this.Opacity;
                this.mfirstCircleBorder.pathonce = true;
                this.mfirstCircleBorder.RefreshGaugeBorder();
            }

            if (this.mmiddleCircleBorder != null)
            {
                if (this.MiddleFrameBrush != null)
                {
                    this.mmiddleCircleBorder.Background = this.MiddleFrameBrush;
                }
                else
                {
                    this.mmiddleCircleBorder.Background = this.Background;
                }

                this.mmiddleCircleBorder.OpacityMask = this.OpacityMask;
                this.mmiddleCircleBorder.Opacity = this.Opacity;
                this.mmiddleCircleBorder.pathonce = true;
                this.mmiddleCircleBorder.RefreshGaugeBorder();
            }

            if (this.msecondCircleBorder != null)
            {
                if (this.InnerFrameBrush != null)
                {
                    this.msecondCircleBorder.Background = this.InnerFrameBrush;
                }
                else
                {
                    this.msecondCircleBorder.Background = this.Background;
                }

                this.msecondCircleBorder.OpacityMask = this.OpacityMask;
                this.msecondCircleBorder.Opacity = this.Opacity;
                this.msecondCircleBorder.RefreshGaugeBorder();
            }

            if (this.minnerCircleBorder != null)
            {
                // this.minnerCircleBorder.Background = this.Background;
                this.minnerCircleBorder.OpacityMask = this.OpacityMask;
                this.minnerCircleBorder.Opacity = this.Opacity;
                this.minnerCircleBorder.RefreshGaugeBorder();
            }

            if (this.minnerPath != null)
            {
                double width = 0, height = 0;
                if (this is LinearGauge)
                {
                    width = (this as LinearGauge).GetGaugeSize().Width;
                    height = (this as LinearGauge).GetGaugeSize().Height;
                }
                else if (this is DigitalGauge)
                {
                    width = (this as DigitalGauge).GetGaugeSize().Width;
                    height = (this as DigitalGauge).GetGaugeSize().Height;
                }

                if (!double.IsNaN(this.Height) && !double.IsNaN(this.Width))
                {
                    width = this.Width;
                    height = this.Height;
                }
                this.minnerPath.Height = (height - (this.InnerFrameOffset + this.OuterFrameOffset + this.MiddleFrameOffset) - 1) > 0 ? (height - (this.InnerFrameOffset + this.OuterFrameOffset + this.MiddleFrameOffset) - 1) : 0;
                this.minnerPath.Width = (width - (this.InnerFrameOffset + this.OuterFrameOffset + this.MiddleFrameOffset) - 1) > 0 ? (width - (this.InnerFrameOffset + this.OuterFrameOffset + this.MiddleFrameOffset) - 1) : 0;
                TranslateTransform transform = new TranslateTransform();
                transform.X = 0;
                transform.Y = 0;
                this.minnerPath.RenderTransform = transform;
                if (this.Tag != null)
                {
                    if (this.Tag.ToString() == "1")
                    {
                        this.minnerPath.Opacity = 20;
                    }
                }
            }
            if (this is DigitalGauge)
            {
                (this as DigitalGauge).UpdateBorder();
            }

        }

        /// <summary>
        /// Updates the location of children elements.
        /// </summary>
        protected virtual void UpdateChildrenLocation()
        {
            if (this.ChildrenCollection != null)
            {
                int count = this.ChildrenCollection.Count;
                for (int i = 0; i < count; i++)
                {
                    if (this.ChildrenCollection[i] is LocalizableGaugeElement)
                    {
                        LocalizableGaugeElement elem = this.ChildrenCollection[i] as LocalizableGaugeElement;
                        Point location = this.ConvertLocation(elem.Location);
                        TranslateTransform transform = new TranslateTransform();
                        transform.X = location.X;
                        transform.Y = location.Y;
                        elem.RenderTransform = transform;
                    }
                }
            }
        }

        /// <summary>
        /// Calls OnOuterFrameBrushChanged method of the instance, notifies of the depencency property value changes.
        /// </summary>
        /// <param name="d">Dependency object, the change occures on.</param>
        /// <param name="e">Property change details, such as old value and new value.</param>
        private static void OnOuterFrameBrushChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            GaugeBase instance = (GaugeBase)d;
            instance.OnOuterFrameBrushChanged(e);
        }

        /// <summary>
        /// Calls OnOuterFrameOffsetChanged method of the instance, notifies of the depencency property value changes.
        /// </summary>
        /// <param name="d">Dependency object, the change occures on.</param>
        /// <param name="e">Property change details, such as old value and new value.</param>
        private static void OnOuterFrameOffsetChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            GaugeBase instance = (GaugeBase)d;
            instance.OnOuterFrameOffsetChanged(e);
        }

        /// <summary>
        /// Calls OnInnerFrameBrushChanged method of the instance, notifies of the depencency property value changes.
        /// </summary>
        /// <param name="d">Dependency object, the change occures on.</param>
        /// <param name="e">Property change details, such as old value and new value.</param>
        private static void OnInnerFrameBrushChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            GaugeBase instance = (GaugeBase)d;
            instance.OnInnerFrameBrushChanged(e);
        }

        /// <summary>
        /// Calls OnInnerFrameOffsetChanged method of the instance, notifies of the depencency property value changes.
        /// </summary>
        /// <param name="d">Dependency object, the change occures on.</param>
        /// <param name="e">Property change details, such as old value and new value.</param>
        private static void OnInnerFrameOffsetChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            GaugeBase instance = (GaugeBase)d;
            instance.OnInnerFrameOffsetChanged(e);
        }

        /// <summary>
        /// Calls OnMiddleFrameBrushChanged method of the instance, notifies of the depencency property value changes.
        /// </summary>
        /// <param name="d">Dependency object, the change occures on.</param>
        /// <param name="e">Property change details, such as old value and new value.</param>
        private static void OnMiddleFrameBrushChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            GaugeBase instance = (GaugeBase)d;
            instance.OnMiddleFrameBrushChanged(e);
        }

        /// <summary>
        /// Calls OnMiddleFrameOffsetChanged method of the instance, notifies of the depencency property value changes.
        /// </summary>
        /// <param name="d">Dependency object, the change occures on.</param>
        /// <param name="e">Property change details, such as old value and new value.</param>
        private static void OnMiddleFrameOffsetChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            GaugeBase instance = (GaugeBase)d;
            instance.OnMiddleFrameOffsetChanged(e);
        }

        /// <summary>
        /// Check if Digital value is shown or not
        /// </summary>
        /// <param name="e">An <see cref="T:System.Windows.DependencyPropertyChangedEventArgs">DependencyPropertyChangedEventArgs</see> that contains the event data.</param>
        /// <remarks></remarks>
        protected virtual void IsShowDigitalValueChanged(DependencyPropertyChangedEventArgs e)
        {
            this.RefreshBorders();

            if (this.ShowDigtialValueChanged != null)
            {
                this.ShowDigtialValueChanged(this, e);
            }
        }

        /// <summary>
        /// Calls EnableEffectsChanged method of the instance, notifies of the dependency property value change.
        /// </summary>
        /// <param name="e">Property change details, such as old value and new value.</param>
        protected virtual void IsEnableEffectsChanged(DependencyPropertyChangedEventArgs e)
        {
            this.RefreshBorders();
            if (this.EnableEffectsChanged != null)
            {
                this.EnableEffectsChanged(this, e);
            }
        }

        /// <summary>
        /// Calls OnInnerFrameContentChanged method of the instance, notifies of the dependency property value change.
        /// </summary>
        /// <param name="d">Dependency object, the change occures on.</param>
        /// <param name="e">Property change details, such as old value and new value.</param>
        private static void OnDigitalValueChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            GaugeBase instance = (GaugeBase)d;
            instance.OnDigitalValueChanged(e);
        }

        /// <summary>
        /// Updates property value cache.
        /// </summary>
        /// <param name="e">Property change details, such as old value and new value.</param>
        protected virtual void OnDigitalValueChanged(DependencyPropertyChangedEventArgs e)
        {
            if (e.NewValue == null)
            {
                ////this.RemoveVisualChild(e.OldValue as Visual);
            }
            else
            {
                //// this.AddLogicalChild(e.NewValue as Visual);
            }
        }

        /// <summary>
        /// Calls IsShowDigitalValueChanged method of the instance, notifies of the dependency property value change.
        /// </summary>
        /// <param name="d">Dependency object, the change occures on.</param>
        /// <param name="e">Property change details, such as old value and new value.</param>
        private static void IsShowDigitalValueChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            GaugeBase instance = (GaugeBase)d;
            instance.IsShowDigitalValueChanged(e);
        }

        /// <summary>
        /// Calls IsEnableEffectsChanged method of the instance, notifies of the dependency property value change.
        /// </summary>
        /// <param name="d">Dependency object, the change occures on.</param>
        /// <param name="e">Property change details, such as old value and new value.</param>
        private static void IsEnableEffectsChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            GaugeBase instance = (GaugeBase)d;
            instance.IsEnableEffectsChanged(e);
        }
        #endregion
    }
}
