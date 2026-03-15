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
    using System.Linq;
    using System.Text;
    using System.Windows;
    using System.Windows.Media;
    using System.Windows.Shapes;
    using System.Windows.Controls;
    

    /// <summary>
    /// Represents the tick for the scale. It can be dislayed as major or minor tick.
    /// </summary>
    /// <example>
    /// <list type="table">
    /// <listheader>
    /// <term>C# :</term></listheader>
    /// <item>
    /// <description>using Syncfusion.Windows.Gauge;
    /// <para> </para>
    /// <para>            //creating major MarkTick object and adding it to scale</para>
    /// <para>            MarkTickSet majortick = new MarkTickSet();</para>
    /// <para>            majortick.TickShape = TickShape.Rectangle;</para>
    /// <para>            majortick.TickWidth = 5;</para>
    /// <para>            majortick.TickHeight = 20;</para>
    /// <para>            majortick.DistanceFromScale = 0;</para>
    /// <para>            majortick.TickStyle = TickStyle.MajorTick;</para>
    /// <para>            majortick.TickPlacement = ScalePlacement.Cross;</para>
    /// <para>            majortick.Background = new SolidColorBrush(Colors.Green);</para>
    /// <para>            majortick.BorderBrush = new SolidColorBrush(Colors.Gray);</para>
    /// <para>            majortick.BorderThickness = new Thickness(0.5);</para>
    /// <para>            scale.Ticks.Add(majortick);</para>
    /// <para></para>
    /// <para>            //creating minor MarkTick object and adding it to scale</para>
    /// <para>            MarkTickSet c_minortick = new MarkTickSet();</para>
    /// <para>            minortick.TickShape = TickShape.Rectangle;</para>
    /// <para>            minortick.TickWidth = 2;</para>
    /// <para>            minortick.TickHeight = 10;</para>
    /// <para>            minortick.TickStyle = TickStyle.MinorTick;</para>
    /// <para>            minortick.DistanceFromScale = 0;</para>
    /// <para>            minortick.TickPlacement = ScalePlacement.Inside;</para>
    /// <para>            minortick.Background = new SolidColorBrush(Colors.Green);</para>
    /// <para>            minortick.BorderBrush = new SolidColorBrush(Colors.Gray);</para>
    /// <para>            minortick.BorderThickness = new Thickness(0.5);</para>
    /// <para>            scale.Ticks.Add(minortick);</para></description></item></list>
    /// <list type="table">
    /// <listheader>
    /// <term>Xaml :</term></listheader>
    /// <item>
    /// <description>  xmlns:syncfusion=&quot;clr-namespace:Syncfusion.Windows.Gauge;assembly=Syncfusion.Gauge.Silverlight&quot;
    /// <para>   </para>
    /// <para>&lt;!--Adding markticks to the scale--&gt;</para>
    /// <para>         &lt;syncfusion:MarkTickSet TickHeight=&quot;8&quot; TickShape=&quot;Triangle&quot; TickStyle=&quot;MajorTick&quot; Name=&quot;majorTick&quot;   TickWidth=&quot;4&quot; Background=&quot;Silver&quot;/&gt;</para>
    /// <para></para>
    /// <para>         &lt;syncfusion:MarkTickSet TickHeight=&quot;4&quot; TickWidth=&quot;1&quot; TickStyle=&quot;MinorTick&quot; Name=&quot;minorTick&quot;    Background=&quot;Silver&quot; TickPlacement=&quot;Inside&quot;/&gt;</para>
    /// <para></para>
    /// <para> &lt;/syncfusion:CircularScale.Ticks&gt;</para></description></item></list>
    /// </example>
    public class MarkTickSet : TickSetBase
    {
        #region Dependency properties

        /// <summary>
        /// Identifies the <see cref="P:Syncfusion.Windows.Gauge.MarkTickSet.TickHeight">TickHeight</see> dependency property.
        /// </summary>
        /// <returns>
        /// Type : <see cref="T:System.Double">System.Double</see>
        /// </returns>
        public static readonly DependencyProperty TickHeightProperty =
            DependencyProperty.Register("TickHeight", typeof(double), typeof(MarkTickSet), new PropertyMetadata(10d, new PropertyChangedCallback(OnTickHeightChanged)));

        /// <summary>
        /// Identifies the <see cref="P:Syncfusion.Windows.Gauge.MarkTickSet.TickShape">TickShape</see> dependency property.
        /// </summary>
        /// <returns>
        /// Type : <see cref="T:Syncfusion.Windows.Gauge.TickShape">TickShape</see>
        /// </returns>
        public static readonly DependencyProperty TickShapeProperty =
            DependencyProperty.Register("TickShape", typeof(TickShape), typeof(MarkTickSet), new PropertyMetadata(TickShape.Rectangle, new PropertyChangedCallback(OnTickShapeChanged)));

        /// <summary>
        /// Identifies the <see cref="P:Syncfusion.Windows.Gauge.MarkTickSet.TickWidth">TickWidth</see> dependency property.
        /// </summary>
        /// <returns>
        /// Type : <see cref="T:System.Double">System.Double</see>
        /// </returns>
        public static readonly DependencyProperty TickWidthProperty =
            DependencyProperty.Register("TickWidth", typeof(double), typeof(MarkTickSet), new PropertyMetadata(2d, new PropertyChangedCallback(OnTickWidthChanged)));
        
        /// <summary>
        /// Identifies the <see cref="P:Syncfusion.Windows.Gauge.MarkTickSet.XOffset">XOffset</see> dependency property.
        /// </summary>
        public static readonly DependencyProperty TickBackgroundProperty =
            DependencyProperty.Register("TickBackground", typeof(Brush), typeof(MarkTickSet), new PropertyMetadata(new SolidColorBrush(Colors.Black), new PropertyChangedCallback(OnBackgroundChanged)));

        /// <summary>
        /// Identifies the <see cref="P:Syncfusion.Windows.Gauge.MarkTickSet.XOffset">XOffset</see> dependency property.
        /// </summary>
        internal static readonly DependencyProperty XOffsetProperty =
            DependencyProperty.Register("XOffset", typeof(double), typeof(MarkTickSet), new PropertyMetadata(new PropertyChangedCallback(OnXOffsetChanged)));

        /// <summary>
        /// Identifies the <see cref="YOffset"/> dependency property.
        /// </summary>
        internal static readonly DependencyProperty YOffsetProperty =
            DependencyProperty.Register("YOffset", typeof(double), typeof(MarkTickSet), new PropertyMetadata(new PropertyChangedCallback(OnYOffsetChanged)));
        
        
        #endregion

        /// <summary>
        /// The path used to draw the tick.
        /// </summary>
        private Path mtickPath;

        internal string ticktooltip;

        internal int majorflag=0;
        internal int middleflag = 0;
        internal int minorflag = 0;

        #region Initialization

        /// <summary>
        /// Initializes a new instance of the <see cref="T:Syncfusion.Windows.Gauge.MarkTickSet">MarkTickSet</see> class.
        /// </summary>
        public MarkTickSet()
        {
            DefaultStyleKey = typeof(MarkTickSet);
            this.Loaded += new RoutedEventHandler(this.MarkTickSetLoaded);
        }

        #endregion

        #region Overrides

        /// <summary>
        /// Builds the current template's visual tree if necessary.
        /// </summary>
        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();
            this.mtickPath = this.GetTemplateChild("TickPath") as Path;
            
            if (this.ticktooltip != null)
            {
                ToolTip t = new ToolTip()
                {
                    Content = this.ticktooltip
                };
                ToolTipService.SetToolTip(this.mtickPath, t);
            }
            else
            {
                ToolTipService.SetToolTip(this.mtickPath, null);
            }
            this.UpdateVisualStyle();
            this.RefreshTick();
            this.IsTabStop = false;
        }     
       
        protected override void UpdateVisualStyle()
        {
            if (this.GaugeElementParent != null && (this.GaugeElementParent as MarkTickSet) != null)
            {
                MarkTickSet tickSet = (this.GaugeElementParent as MarkTickSet);
                if(tickSet.GaugeElementParent != null && (tickSet.GaugeElementParent as ScaleBase) != null)
                {
                    ScaleBase scaleBase = (tickSet.GaugeElementParent as ScaleBase);
                    if (scaleBase.GaugeElementParent != null && (scaleBase.GaugeElementParent as GaugeBase) != null)
                    {
                        this.VisualStyle = (scaleBase.GaugeElementParent as GaugeBase).VisualStyle;
                    }
                }
            }
            //this.VisualStyle = (((this.GaugeElementParent as MarkTickSet).GaugeElementParent as ScaleBase).GaugeElementParent as GaugeBase).VisualStyle;
            base.UpdateVisualStyle();
        }
        #endregion


        #region Events
        /// <summary>
        /// Event that is raised when <see cref="Background"/> property is changed.
        /// </summary>
        //public event PropertyChangedCallback BackgroundChanged;

        /// <summary>
        /// Event that is raised when <see cref="P:Syncfusion.Windows.Gauge.MarkTickSet.TickHeight">TickHeight</see> property is changed.
        /// </summary>
        public event PropertyChangedCallback TickHeightChanged;

        /// <summary>
        /// Event that is raised when <see cref="P:Syncfusion.Windows.Gauge.MarkTickSet.TickShape">TickShape</see> property is changed.
        /// </summary>
        public event PropertyChangedCallback TickShapeChanged;

        /// <summary>
        /// Event that is raised when <see cref="P:Syncfusion.Windows.Gauge.MarkTickSet.TickWidth">TickWidth</see> property is changed.
        /// </summary>
        public event PropertyChangedCallback TickWidthChanged;
        /// <summary>
        /// Event that is raised when <see cref="XOffset"/> property is changed.
        /// </summary>
        internal event PropertyChangedCallback XOffsetChanged;

        /// <summary>
        /// Event that is raised when <see cref="YOffset"/> property is changed.
        /// </summary>
        internal event PropertyChangedCallback YOffsetChanged;

     
        #endregion

        #region DP getters & setters
        /// <summary>
        /// Gets or sets the x-offset of the tick.
        /// This is a dependency property.
        /// </summary>
        /// <value>
        /// Type: <see cref="double"/>
        /// Default value is 0.
        /// </value>
        /// <seealso cref="double"/>
        public Brush TickBackground
        {
            get
            {
                return (Brush)GetValue(TickBackgroundProperty);
            }

            set
            {
                SetValue(TickBackgroundProperty, value);
            }
        }

        /// <summary>
        /// Gets or sets the height of the mark tick. This is a dependency property.
        /// </summary>
        /// <remarks>
        /// Default value is 0.
        /// </remarks>
        /// <value>
        /// Type: <see cref="T:System.Double">System.Double</see>
        /// </value>
        /// <seealso cref="double">double</seealso>
        public double TickHeight
        {
            get
            {
                return (double)GetValue(TickHeightProperty);
            }

            set
            {
                SetValue(TickHeightProperty, value);
            }
        }

        /// <summary>
        /// Gets or sets different shapes for the mark tick. This is a dependency property.
        /// </summary>
        /// <remarks>
        /// <para>Default value is <see cref="F:Syncfusion.Windows.Gauge.TickShape.Rectangle">TickShape.Rectangle</see>.</para>
        /// </remarks>
        /// <value>
        /// Type : <see cref="T:Syncfusion.Windows.Gauge.TickShape">TickShape</see>
        /// </value>
        /// <seealso cref="TickShape">TickShape</seealso>
        public TickShape TickShape
        {
            get
            {
                return (TickShape)GetValue(TickShapeProperty);
            }

            set
            {
                SetValue(TickShapeProperty, value);
            }
        }

        /// <summary>
        /// Gets or sets the width of the mark tick. This is a dependency property.
        /// </summary>
        /// <remarks>
        /// Default value is 0.
        /// </remarks>
        /// <value>
        /// Type: <see cref="T:System.Double">System.Double</see>
        /// </value>
        /// <seealso cref="double">double</seealso>
        public double TickWidth
        {
            get
            {
                return (double)GetValue(TickWidthProperty);
            }

            set
            {
                SetValue(TickWidthProperty, value);
            }
        }
        /// <summary>
        /// Gets or sets the x-offset of the tick.
        /// This is a dependency property.
        /// </summary>
        /// <value>
        /// Type: <see cref="double"/>
        /// Default value is 0.
        /// </value>
        /// <seealso cref="double"/>
        internal double XOffset
        {
            get
            {
                return (double)GetValue(XOffsetProperty);
            }

            set
            {
                SetValue(XOffsetProperty, value);
            }
        }

        /// <summary>
        /// Gets or sets the y-offset of the tick.
        /// This is a dependency property.
        /// </summary>
        /// <value>
        /// Type: <see cref="double"/>
        /// Default value is 0.
        /// </value>
        /// <seealso cref="double"/>
        internal double YOffset
        {
            get
            {
                return (double)GetValue(YOffsetProperty);
            }

            set
            {
                SetValue(YOffsetProperty, value);
            }
        }
        #endregion

        #region Implementation
       
        /// <summary>
        // /// Updates property value cache and raises <see cref="AngleChanged"/> event.
        /// </summary>
        /// <param name="e">Property change details, such as old value and new value.</param>
        protected override void OnAngleChanged(DependencyPropertyChangedEventArgs e)
        {
            if (this.GaugeElementParent is ScaleBase)
            {
                ScaleBase scale = this.GaugeElementParent as ScaleBase;
                scale.RefreshTickSet();
                scale.RefreshScale();
            }

            base.OnAngleChanged(e);
        }

        private static void OnBackgroundChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            MarkTickSet instance = (MarkTickSet)d;
            
            if ((e.NewValue is SolidColorBrush) && instance.Tag != null)
            {
                Color c = Color.FromArgb(255, 55, 93, 129);
                Color c1 = Color.FromArgb(255, 170, 170, 170);
                Color c2 = Color.FromArgb(255, 255, 255, 255);
                Color c3 = Color.FromArgb(255, 13, 85, 226);
                if (((SolidColorBrush)e.NewValue).Color == c && instance.Tag.ToString() == "1" )
                {
                    if (instance.majorflag != 4)
                        instance.majorflag = 1;
                    if (instance.middleflag != 4)
                        instance.middleflag = 1;
                    if (instance.minorflag != 4)
                        instance.minorflag = 1;

                }
                else if (((SolidColorBrush)e.NewValue).Color == c1 && instance.Tag.ToString() == "2")
                {
                    if (instance.majorflag != 4)
                        instance.majorflag = 2;
                    if (instance.middleflag != 4)
                        instance.middleflag = 2;
                    if (instance.minorflag != 4)
                        instance.minorflag = 2;

                }
                else if (((SolidColorBrush)e.NewValue).Color == c2 && instance.Tag.ToString() == "3")
                {
                    if (instance.majorflag != 4)
                        instance.majorflag = 3;
                    if (instance.middleflag != 4)
                        instance.middleflag = 3;
                    if (instance.minorflag != 4)
                        instance.minorflag = 3;
                }
                else if (((SolidColorBrush)e.NewValue).Color == c3 && instance.Tag.ToString() == "5")
                {
                    if (instance.majorflag != 4)
                        instance.majorflag = 5;
                    if (instance.middleflag != 4)
                        instance.middleflag = 5;
                    if (instance.minorflag != 4)
                        instance.minorflag = 5;
                }
                else
                {
                    if (instance.TickStyle == TickStyle.MajorTick)
                        instance.majorflag = 4;
                    if (instance.TickStyle == TickStyle.MidTick)
                        instance.middleflag = 4;
                    if (instance.TickStyle == TickStyle.MinorTick)
                        instance.minorflag = 4;
                }


            }
            else
            {
                if (instance.TickStyle==TickStyle.MajorTick)
                    instance.majorflag = 4;
                if (instance.TickStyle == TickStyle.MidTick)
                    instance.middleflag = 4;
                if (instance.TickStyle == TickStyle.MinorTick)
                    instance.minorflag = 4;
            }
            instance.OnBackgroundChanged(e);
        }

        protected void OnBackgroundChanged(DependencyPropertyChangedEventArgs e)
        {
            if (this.GaugeElementParent is ScaleBase)
            {
                ScaleBase scale = this.GaugeElementParent as ScaleBase;
                scale.RefreshTickSet();
                scale.RefreshScale();
            }

            //if (this.BackgroundChanged != null)
            //{
            //    this.BackgroundChanged(this, e);
            //}
        }

        /// <summary>
        // /// Updates property value cache and raises <see cref="DistanceFromScaleChanged"/> event.
        /// </summary>
        /// <param name="e">Property change details, such as old value and new value.</param>
        protected override void OnDistanceFromScaleChanged(DependencyPropertyChangedEventArgs e)
        {
            if (this.GaugeElementParent is ScaleBase)
            {
                ScaleBase scale = this.GaugeElementParent as ScaleBase;
                scale.RefreshTickSet();
                scale.RefreshScale();
            }

            base.OnDistanceFromScaleChanged(e);
        }

        /// <summary>
        /// Updates property value cache and raises <see cref="TickHeightChanged"/> event.
        /// </summary>
        /// <param name="e">
        /// Property change details, such as old value and new value.</param>
        protected virtual void OnTickHeightChanged(DependencyPropertyChangedEventArgs e)
        {
            if (this.GaugeElementParent is ScaleBase)
            {
                ScaleBase scale = this.GaugeElementParent as ScaleBase;
                scale.RefreshTickSet();
                scale.RefreshScale();
            }

            if (this.TickHeightChanged != null)
            {
                this.TickHeightChanged(this, e);
            }
        }

        /// <summary>
        // /// Updates property value cache and raises <see cref="TickPlacementChanged"/> event.
        /// </summary>
        /// <param name="e">Property change details, such as old value and new value.</param>
        protected override void OnTickPlacementChanged(DependencyPropertyChangedEventArgs e)
        {
            if (this.GaugeElementParent is ScaleBase)
            {
                ScaleBase scale = this.GaugeElementParent as ScaleBase;
                scale.RefreshTickSet();
                scale.RefreshScale();
                if(this.Parent is LinearScaleLayoutPanel)
                (this.Parent as LinearScaleLayoutPanel).InvalidateArrange();
                   
            }

            base.OnTickPlacementChanged(e);
        }

        /// <summary>
        /// Updates property value cache and raises <see cref="TickShapeChanged"/> event.
        /// </summary>
        /// <param name="e">
        /// Property change details, such as old value and new value.</param>
        protected virtual void OnTickShapeChanged(DependencyPropertyChangedEventArgs e)
        {
            if (this.GaugeElementParent is ScaleBase)
            {
                ScaleBase scale = this.GaugeElementParent as ScaleBase;
                scale.RefreshTickSet();
                scale.RefreshScale();
            }

            if (this.TickShapeChanged != null)
            {
                this.TickShapeChanged(this, e);
            }
        }

        /// <summary>
        /// Updates property value cache and raises <see cref="TickWidthChanged"/> event.
        /// </summary>
        /// <param name="e">
        /// Property change details, such as old value and new value.</param>
        protected void OnTickWidthChanged(DependencyPropertyChangedEventArgs e)
        {
            if (this.GaugeElementParent is ScaleBase)
            {
                ScaleBase scale = this.GaugeElementParent as ScaleBase;
                scale.RefreshTickSet();
                scale.RefreshScale();
            }

            if (this.TickWidthChanged != null)
            {
                this.TickWidthChanged(this, e);
            }
        }

        /// <summary>
        /// Calls OnTickShapeChanged method of the instance, notifies of the depencency property value changes.
        /// </summary>
        /// <param name="d">Dependency object, the change occures on.</param>
        /// <param name="e">Property change details, such as old value and new value.</param>
        private static void OnTickShapeChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            MarkTickSet instance = (MarkTickSet)d;
            instance.OnTickShapeChanged(e);
        }

        /// <summary>
        /// Calls OnTickWidthChanged method of the instance, notifies of the depencency property value changes.
        /// </summary>
        /// <param name="d">Dependency object, the change occures on.</param>
        /// <param name="e">Property change details, such as old value and new value.</param>
        private static void OnTickWidthChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            MarkTickSet instance = (MarkTickSet)d;
            instance.OnTickWidthChanged(e);
        }

        /// <summary>
        /// Calls OnTickHeightChanged method of the instance, notifies of the depencency property value changes.
        /// </summary>
        /// <param name="d">Dependency object, the change occures on.</param>
        /// <param name="e">Property change details, such as old value and new value.</param>
        private static void OnTickHeightChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            MarkTickSet instance = (MarkTickSet)d;
            instance.OnTickHeightChanged(e);
        }

        /// <summary>
        /// Invoked when the MarkTick set is loaded
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The object containing the event data.</param>
        private void MarkTickSetLoaded(object sender, RoutedEventArgs e)
        {
            if (this.GaugeElementParent is ScaleBase)
            {
                ScaleBase scale = this.GaugeElementParent as ScaleBase;                
                scale.RefreshTickSet();
                scale.RefreshScale();
            }
        }
        /// <summary>
        /// Refreshs the MarkTickSet
        /// </summary>
        internal void RefreshTick()
        {
            if (this.mtickPath != null)
            {
                switch (this.TickShape)
                {
                    case TickShape.Rectangle:
                        RectangleGeometry rectangleGeometry = new RectangleGeometry();
                        rectangleGeometry.Rect = new Rect(0, 0, this.TickHeight, this.TickWidth);
                        this.mtickPath.Data = rectangleGeometry;
                        break;

                    case TickShape.Ellipse:
                        EllipseGeometry ellipseGeometry = new EllipseGeometry();
                        ellipseGeometry.Center = new Point(this.TickHeight / 2, this.TickWidth / 2);
                        ellipseGeometry.RadiusX = this.TickHeight / 2;
                        ellipseGeometry.RadiusY = this.TickWidth / 2;
                        this.mtickPath.Data = ellipseGeometry;
                        break;

                    case TickShape.Triangle:
                        PathGeometry trianglePath = new PathGeometry();
                        PathFigure figure1 = new PathFigure();
                        figure1.StartPoint = new Point(0, this.TickWidth / 2);
                        LineSegment lineSegment = new LineSegment();
                        lineSegment.Point = new Point(0, this.TickWidth / 2);
                        figure1.Segments.Add(lineSegment);
                        lineSegment = new LineSegment();
                        lineSegment.Point = new Point(this.TickHeight, 0);
                        figure1.Segments.Add(lineSegment);
                        lineSegment = new LineSegment();
                        lineSegment.Point = new Point(this.TickHeight, this.TickWidth);
                        figure1.Segments.Add(lineSegment);
                        lineSegment = new LineSegment();
                        lineSegment.Point = new Point(0, this.TickWidth / 2);
                        figure1.Segments.Add(lineSegment);
                        trianglePath.Figures.Add(figure1);
                        this.mtickPath.Data = trianglePath;
                        break;
                }

                if (this.ticktooltip != null)
                {
                    ToolTip t = new ToolTip()
                    {
                        Content = this.ticktooltip
                    };
                    ToolTipService.SetToolTip(this.mtickPath, t);
                }
                else
                {
                    ToolTipService.SetToolTip(this.mtickPath, null);
                }
            }
        }

        /// <summary>
        /// Invoked to arrange the Size
        /// </summary>
        /// <param name="finalSize">Given final size</param>
        /// <returns>Arranged Size</returns>
        protected override Size ArrangeOverride(Size finalSize)
        {
            return base.ArrangeOverride(finalSize);
        }

        /// <summary>
        /// Measures the size
        /// </summary>
        /// <param name="availableSize">Available size</param>
        /// <returns>Desired Size</returns>
        protected override Size MeasureOverride(Size availableSize)
        {
            Size size = base.MeasureOverride(availableSize);
            if (this.GaugeElementParent is ScaleBase)
            {
               // MarkTickSet tickSet = this.GaugeElementParent as MarkTickSet;
                size = new Size(this.TickHeight, this.TickWidth);
            }

            return size;
        }

        /// <summary>
        /// Updates property value cache and raises <see cref="XOffsetChanged"/> event.
        /// </summary>
        /// <param name="e">
        /// Property change details, such as old value and new value.</param>
        protected virtual void OnXOffsetChanged(DependencyPropertyChangedEventArgs e)
        {
            if (this.XOffsetChanged != null)
            {
                this.XOffsetChanged(this, e);
            }
        }

        /// <summary>
        /// Updates property value cache and raises <see cref="YOffsetChanged"/> event.
        /// </summary>
        /// <param name="e">
        /// Property change details, such as old value and new value.</param>
        protected virtual void OnYOffsetChanged(DependencyPropertyChangedEventArgs e)
        {
            if (this.YOffsetChanged != null)
            {
                this.YOffsetChanged(this, e);
            }
        }

        /// <summary>
        /// Calls OnXOffsetChanged method of the instance, notifies of the depencency property value changes.
        /// </summary>
        /// <param name="d">Dependency object, the change occures on.</param>
        /// <param name="e">Property change details, such as old value and new value.</param>
        private static void OnXOffsetChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            MarkTickSet instance = (MarkTickSet)d;
            instance.OnXOffsetChanged(e);
        }

        /// <summary>
        /// Calls OnYOffsetChanged method of the instance, notifies of the depencency property value changes.
        /// </summary>
        /// <param name="d">Dependency object, the change occures on.</param>
        /// <param name="e">Property change details, such as old value and new value.</param>
        private static void OnYOffsetChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            MarkTickSet instance = (MarkTickSet)d;
            instance.OnYOffsetChanged(e);
        }
        #endregion
    }
}
