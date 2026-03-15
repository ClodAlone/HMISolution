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
    using System.Windows.Controls;
    using System.Windows.Media;
    using System.Windows.Shapes;

    /// <summary>
    /// Represents a scale visual element that &quot;caps&quot; a needle pointer.
    /// </summary>
    /// <example>
    /// <list type="table">
    /// <listheader>
    /// <term>C# :</term></listheader>
    /// <item>
    /// <description>using Syncfusion.Windows.Gauge;
    /// <para></para>
    /// <para>circularscale.PointerCap.PointerCapRadius = 10;</para>
    /// <para>           circularscale.PointerCap.Background = new SolidColorBrush(Colors.Black);</para>
    /// <para>            circularscale.PointerCap.BorderThickness = new Thickness(0.5);</para>
    /// <para>            circularscale.PointerCap.BorderBrush = new SolidColorBrush(Colors.Green);</para></description></item></list>
    /// <para>                                          </para>
    /// <list type="table">
    /// <listheader>
    /// <term>Xaml :</term></listheader>
    /// <item>
    /// <description>xmlns:syncfusion=&quot;clr-namespace:Syncfusion.Windows.Gauge;assembly=Syncfusion.Gauge.Silverlight&quot; 
    /// <para>    </para>
    /// <para></para>
    /// <para>&lt;syncfusion:CircularScale.PointerCap&gt;</para>
    /// <para>                        &lt;syncfusion:PointerCap  BorderBrush=&quot;Black&quot;  BorderThickness=&quot;1&quot; PointerCapRadius=&quot;20&quot; Background=&quot;Black&quot; &gt;</para>
    /// <para>                        &lt;/syncfusion:PointerCap&gt;</para>
    /// <para>                    &lt;/syncfusion:CircularScale.PointerCap&gt;</para>
    /// <para> </para></description></item></list>
    /// </example>
    public class PointerCap : GaugeElement
    {
        #region Dependency properties
        /// <summary>
        /// Identifies the <see cref="P:Syncfusion.Windows.Gauge.PointerCap.PointerCapRadius">PointerCapRadius</see>  dependency property.
        /// </summary>
        /// <remarks>
        /// <para>The default velue for the pointer cap radius is 20d.</para>
        /// </remarks>
        /// <returns>
        /// <para>Type : <see cref="T:System.Double">System.Double</see></para>
        /// </returns>
        public static readonly DependencyProperty PointerCapRadiusProperty =
            DependencyProperty.Register("PointerCapRadius", typeof(double), typeof(PointerCap), new PropertyMetadata(20d, new PropertyChangedCallback(OnPointerCapRadiusChanged)));

        /// <summary>
        /// Identifies the <see cref="P:Syncfusion.Windows.Gauge.PointerCap.PointerCapType">PointerCapType</see>  dependency property.
        /// </summary>
        /// <remarks>
        /// <para>There's only one type of Pointer Cap i.e <see cref="F:Syncfusion.Windows.Gauge.PointerCapType.Default">PointerCapType.Default</see></para>
        /// </remarks>
        /// <returns>
        /// <para>Type : <see cref="T:Syncfusion.Windows.Gauge.PointerCapType">PointerCapType</see></para>
        /// </returns>
        public static readonly DependencyProperty PointerCapTypeProperty =
            DependencyProperty.Register("PointerCapType", typeof(PointerCapType), typeof(PointerCap), new PropertyMetadata(new PropertyChangedCallback(OnPointerCapTypeChanged)));

        /// <summary>
        /// Identifies the <see cref="P:Syncfusion.Windows.Gauge.PointerCap.CapOnTop">CapOnTop</see> dependency property.
        /// </summary>
        /// <remarks>
        /// This property indicates whether the Pointer Cap is above the pointer or below.
        /// </remarks>
        /// <returns>
        /// <para>Type: <see cref="T:System.Boolean">System.Boolean</see></para>
        /// </returns>
        public static readonly DependencyProperty CapOnTopProperty =
            DependencyProperty.Register("CapOnTop", typeof(bool), typeof(PointerCap), new PropertyMetadata(true, new PropertyChangedCallback(OnCapOnTopChanged)));

        /// <summary>
        // /// Identifies the <see cref="InnerFrameBrush"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty InnerCapBrushProperty =
            DependencyProperty.Register("InnerCapBrush", typeof(Brush), typeof(PointerCap), new PropertyMetadata(new PropertyChangedCallback(OnInnerCapBrushChanged)));

        /// <summary>
        // /// Identifies the <see cref="OuterFrameBrush"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty OuterCapBrushProperty =
            DependencyProperty.Register("OuterCapBrush", typeof(Brush), typeof(PointerCap), new PropertyMetadata(new PropertyChangedCallback(OnOuterCapBrushChanged)));

        #endregion

        #region Private members

        /// <summary>
        /// The path used to draw the pointer cap.
        /// </summary>
        private Path mpointerCapPath;

        /// <summary>
        /// The path used to draw the pointer cap.
        /// </summary>
        private Path mpointerCapPathShadow;

        /// <summary>
        /// The path used to draw the pointer cap.
        /// </summary>
        private Path mpointerCapfirstPath;

        /// <summary>
        /// The path used to draw the pointer cap.
        /// </summary>
        private Path mpointerCapsecondPath;

        #endregion

        #region Initialization

        /// <summary>
        /// Initializes a new instance of the <see cref="T:Syncfusion.Windows.Gauge.PointerCap">PointerCap</see> class.
        /// </summary>
        public PointerCap()
        {
            DefaultStyleKey = typeof(PointerCap);
            this.SizeChanged += new SizeChangedEventHandler(this.PointerCapSizeChanged);
            Canvas.SetZIndex(this, 1);
        }

        #endregion

        #region Events

        /// <summary>
        /// Event that is raised when <see cref="P:Syncfusion.Windows.Gauge.PointerCap.PointerCapRadius">PointerCapRadius</see>  property is changed.
        /// </summary>
        public event PropertyChangedCallback PointerCapRadiusChanged;

        /// <summary>
        /// Event that is raised when <see cref="P:Syncfusion.Windows.Gauge.PointerCap.PointerCapType">PointerCapType</see> property is changed.
        /// </summary>
        public event PropertyChangedCallback PointerCapTypeChanged;

        /// <summary>
        /// Event that is raised when <see cref="InnerCapBrush"/> property is changed.
        /// </summary>
        internal event PropertyChangedCallback InnerCapBrushChanged;

        /// <summary>
        /// Event that is raised when <see cref="OuterCapBrush"/> property is changed.
        /// </summary>
        internal event PropertyChangedCallback OuterCapBrushChanged;
        #endregion

        #region DP getters & setters

        /// <summary>
        /// Gets or sets a value indicating whether pointer Cap is on top or not. This is a dependency property.
        /// </summary>
        /// <remarks>
        /// <para>Default value is true.</para>
        /// </remarks>
        /// <value>
        /// Type: <see cref="T:System.Boolean">System.Boolean</see>
        /// </value>
        /// <seealso cref="bool">bool</seealso>
        public bool CapOnTop
        {
            get
            {
                return (bool)GetValue(CapOnTopProperty);
            }

            set
            {
                SetValue(CapOnTopProperty, value);
            }
        }

        /// <summary>
        /// Gets or sets the radius for the cap.
        /// </summary>
        /// <remarks>
        /// <para>Default value is 0. </para>
        /// </remarks>
        /// <value>
        /// Type: <see cref="T:System.Double">System.Double</see>
        /// </value>
        /// <seealso cref="double">double</seealso>
        public double PointerCapRadius
        {
            get
            {
                return (double)GetValue(PointerCapRadiusProperty);
            }

            set
            {
                SetValue(PointerCapRadiusProperty, value);
            }
        }

        /// <summary>
        /// Gets or sets different cap styles.
        /// This is a dependency property.
        /// </summary>
        /// <value>
        /// Type: <see cref="PointerCapType"/>
        /// Default value is PointerCapType.Default.
        /// </value>
        /// <seealso cref="PointerCapType"/>
        public PointerCapType PointerCapType
        {
            get
            {
                return (PointerCapType)GetValue(PointerCapTypeProperty);
            }

            set
            {
                SetValue(PointerCapTypeProperty, value);
            }
        }

        /// <summary>
        /// Gets or sets the inner frame brush
        /// </summary>
        public Brush InnerCapBrush
        {
            get
            {
                return (Brush)GetValue(InnerCapBrushProperty);
            }

            set
            {
                SetValue(InnerCapBrushProperty, value);
            }
        }

        /// <summary>
        /// Gets or sets the inner frame brush
        /// </summary>
        public Brush OuterCapBrush
        {
            get
            {
                return (Brush)GetValue(OuterCapBrushProperty);
            }

            set
            {
                SetValue(OuterCapBrushProperty, value);
            }
        }
        #endregion

        #region Overrides

        /// <summary>
        /// Builds the current template's visual tree if necessary.
        /// </summary>
        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();
            this.mpointerCapPath = this.GetTemplateChild("PointerCapPath") as Path;
            this.mpointerCapPathShadow = this.GetTemplateChild("PointerCapPathShadow") as Path;
            this.mpointerCapfirstPath = this.GetTemplateChild("PointerCapFirstPath") as Path;
            this.mpointerCapsecondPath = this.GetTemplateChild("PointerCapSecondPath") as Path;
            this.UpdateVisualStyle();
            this.RefreshPointerCap();
        }

        protected override void UpdateVisualStyle()
        {
            if (this.GaugeElementParent != null)
                this.VisualStyle = ((this.GaugeElementParent as CircularScale).GaugeElementParent as GaugeBase).VisualStyle;
            base.UpdateVisualStyle();
        }

        #endregion

        #region Implementation

        /// <summary>
        /// Updates the Cap on top property
        /// </summary>
        internal void UpdateCapOnTop()
        {
            if (this.GaugeElementParent is CircularScale)
            {
                CircularScale scale = this.GaugeElementParent as CircularScale;

                scale.RemovePointerCap();
                PointersCollection tempCollection = new PointersCollection();
                for (int k = 0; k < scale.Pointers.Count; k++)
                {
                    if (scale.Pointers[k].ToString().Equals("Syncfusion.Windows.Gauge.CircularPointer"))
                    {
                        tempCollection.Add(scale.Pointers[k]);
                    }
                }

                foreach (CircularPointer pointer in tempCollection)
                {
                    scale.Pointers.Remove(pointer);
                }

                if (!this.CapOnTop)
                {
                    scale.AddPointerCap();
                    foreach (CircularPointer pointer in tempCollection)
                    {
                        scale.Pointers.Add(pointer);
                    }
                }
                else
                {
                    foreach (CircularPointer pointer in tempCollection)
                    {
                        scale.Pointers.Add(pointer);
                    }

                    scale.AddPointerCap();
                }
            }
        }

        /// <summary>
        /// Measures the size in layout required for child elements 
        /// and determines a size for the cap element.
        /// </summary>
        /// <param name="constraint">The available size that this element can give to child elements.</param>
        /// <returns>The size that this element determines it needs during layout.</returns>
        protected override Size MeasureOverride(Size constraint)
        {
            base.MeasureOverride(constraint);
            return new Size(this.PointerCapRadius, this.PointerCapRadius);
        }

        /// <summary>
        // /// Updates property value cache and raises <see cref="CapOnTopChanged"/> event.
        /// </summary>
        /// <param name="e">
        /// Property change details, such as old value and new value.</param>
        protected virtual void OnCapOnTopChanged(DependencyPropertyChangedEventArgs e)
        {
            this.UpdateCapOnTop();
        }

        /// <summary>
        /// Updates property value cache and raises <see cref="PointerCapRadiusChanged"/> event.
        /// </summary>
        /// <param name="e">
        /// Property change details, such as old value and new value.</param>
        protected virtual void OnPointerCapRadiusChanged(DependencyPropertyChangedEventArgs e)
        {
            this.InvalidateMeasure();
            this.RefreshPointerCap();

            if (this.PointerCapRadiusChanged != null)
            {
                this.PointerCapRadiusChanged(this, e);
            }
        }

        /// <summary>
        /// Updates property value cache and raises OnInnerCapBrushChanged event.
        /// </summary>
        /// <param name="e">
        /// Property change details, such as old value and new value.</param>
        protected virtual void OnInnerCapBrushChanged(DependencyPropertyChangedEventArgs e)
        {
            this.RefreshPointerCap();
            if (this.InnerCapBrushChanged != null)
            {
                this.InnerCapBrushChanged(this, e);
            }
        }

        /// <summary>
        /// Updates property value cache and raises InnerFrameBrushChanged event.
        /// </summary>
        /// <param name="e">
        /// Property change details, such as old value and new value.</param>
        protected virtual void OnOuterCapBrushChanged(DependencyPropertyChangedEventArgs e)
        {
            this.RefreshPointerCap();
            if (this.OuterCapBrushChanged != null)
            {
                this.OuterCapBrushChanged(this, e);
            }
        }

        /// <summary>
        /// Updates property value cache and raises <see cref="PointerCapTypeChanged"/> event.
        /// </summary>
        /// <param name="e">
        /// Property change details, such as old value and new value.</param>
        protected virtual void OnPointerCapTypeChanged(DependencyPropertyChangedEventArgs e)
        {
            if (this.PointerCapTypeChanged != null)
            {
                this.PointerCapTypeChanged(this, e);
            }
        }

        /// <summary>
        /// Calls OnPointerCapTypeChanged method of the instance, notifies of the depencency property value changes.
        /// </summary>
        /// <param name="d">Dependency object, the change occures on.</param>
        /// <param name="e">Property change details, such as old value and new value.</param>
        private static void OnPointerCapTypeChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            PointerCap instance = (PointerCap)d;
            instance.OnPointerCapTypeChanged(e);
        }

        /// <summary>
        /// Calls OnCapOnTopChanged method of the instance, notifies of the depencency property value changes.
        /// </summary>
        /// <param name="d">Dependency object, the change occures on.</param>
        /// <param name="e">Property change details, such as old value and new value.</param>
        private static void OnCapOnTopChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            PointerCap instance = (PointerCap)d;
            instance.OnCapOnTopChanged(e);
        }

        /// <summary>
        /// Calls OnPointerCapRadiusChanged method of the instance, notifies of the depencency property value changes.
        /// </summary>
        /// <param name="d">Dependency object, the change occures on.</param>
        /// <param name="e">Property change details, such as old value and new value.</param>
        private static void OnPointerCapRadiusChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            PointerCap instance = (PointerCap)d;
            instance.OnPointerCapRadiusChanged(e);
        }

        /// <summary>
        /// Calls OnInnerCapBrushChanged method of the instance, notifies of the depencency property value changes.
        /// </summary>
        /// <param name="d">Dependency object, the change occures on.</param>
        /// <param name="e">Property change details, such as old value and new value.</param>
        private static void OnInnerCapBrushChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            PointerCap instance = (PointerCap)d;
            instance.OnInnerCapBrushChanged(e);
        }

        /// <summary>
        /// Calls OnOuterCapBrushChanged method of the instance, notifies of the depencency property value changes.
        /// </summary>
        /// <param name="d">Dependency object, the change occures on.</param>
        /// <param name="e">Property change details, such as old value and new value.</param>
        private static void OnOuterCapBrushChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            PointerCap instance = (PointerCap)d;
            instance.OnOuterCapBrushChanged(e);
        }

        /// <summary>
        /// Invoked when the pointercap size is changed
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The object containing the event data.</param>
        private void PointerCapSizeChanged(object sender, RoutedEventArgs e)
        {
            if (!double.IsNaN(this.Width) && this.Width > 0)
            {
                this.PointerCapRadius = this.Width / 2;
            }
        }

        /// <summary>
        /// Refreshs the Pointer cap
        /// </summary>
        internal void RefreshPointerCap()
        {
            if (this.mpointerCapPath != null)
            {
                double radius=0;
                CircularScale scale = this.GaugeElementParent as CircularScale;
                if (scale != null)
                {
                    CircularGauge gauge = scale.GaugeElementParent as CircularGauge;
                    if (gauge == null)
                    {
                        gauge = scale.getGaugeParent(scale);
                    }
                    if (gauge != null)
                    {
                        if (gauge.EnableEffects)
                        {
                            radius = 3.5 * this.PointerCapRadius / 5;
                            this.mpointerCapfirstPath.Visibility = Visibility.Visible;
                            this.mpointerCapsecondPath.Visibility = Visibility.Visible;
                        }
                        else
                        {
                            radius = this.PointerCapRadius;
                            this.mpointerCapfirstPath.Visibility = Visibility.Collapsed;
                            this.mpointerCapsecondPath.Visibility = Visibility.Collapsed;
                        }
                    }
                }
                EllipseGeometry ellipseGeometry = new EllipseGeometry();
                ellipseGeometry.RadiusX = radius;
                ellipseGeometry.RadiusY = radius;
                ellipseGeometry.Center = new Point(this.PointerCapRadius / 2, this.PointerCapRadius / 2);
                this.mpointerCapPath.Data = ellipseGeometry;

                ellipseGeometry = new EllipseGeometry();
                ellipseGeometry.RadiusX = this.PointerCapRadius;
                ellipseGeometry.RadiusY = this.PointerCapRadius;
                ellipseGeometry.Center = new Point(this.PointerCapRadius / 2, this.PointerCapRadius / 2);
                this.mpointerCapfirstPath.Data = ellipseGeometry;
                
                ellipseGeometry = new EllipseGeometry();
                ellipseGeometry.RadiusX = 4 * this.PointerCapRadius / 5;
                ellipseGeometry.RadiusY = 4 * this.PointerCapRadius / 5;
                ellipseGeometry.Center = new Point(this.PointerCapRadius / 2, this.PointerCapRadius / 2);
                this.mpointerCapsecondPath.Data = ellipseGeometry;               

                ellipseGeometry = new EllipseGeometry();
                ellipseGeometry.RadiusX = this.PointerCapRadius;
                ellipseGeometry.RadiusY = this.PointerCapRadius;
                ellipseGeometry.Center = new Point(this.PointerCapRadius / 2, this.PointerCapRadius / 2);
                this.mpointerCapPathShadow.Data = ellipseGeometry;
                this.mpointerCapPathShadow.Fill = new SolidColorBrush(Color.FromArgb(100, 123, 123, 118));

                if (scale != null)
                {
                    TranslateTransform transform = new TranslateTransform();
                    if (scale.ShadowOffset > 1)
                    {
                        transform.X = scale.ShadowOffset - 1;
                        transform.Y = scale.ShadowOffset - 1;
                    }

                    this.mpointerCapPathShadow.RenderTransform = transform;
                }
            }
        }

        #endregion
    }
}
