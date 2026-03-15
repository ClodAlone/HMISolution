// <copyright file="PointerCap.cs" company="Syncfusion Software">
// Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Any infringement will be prosecuted under
// applicable laws. 
// </copyright>

using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Permissions;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Markup;
using System.Windows.Media;
using Syncfusion.Licensing;

namespace Syncfusion.Windows.Gauge
{
    /// <summary>
    /// Represents the pointer cap visual element that "caps" the needle pointer.
    /// </summary>
    /// <remarks>
    /// PointerCap is allowed only for <see cref="PointerNeedleType.Needle"/> type pointer.
    /// </remarks>
    /// <seealso cref="PointerCapShadow"/>
    /// <example>
    /// <code lang="XAML">
    /// <Window x:Class="PointerCapSample.Window1" Title="PointerCapSample" Height="400"
    /// Width="400" xmlns="http://schemas.microsoft.com/winfx/2006/xaml/presentation"
    /// xmlns:x="http://schemas.microsoft.com/winfx/2006/xaml"
    /// xmlns:syncfusion="http://schemas.syncfusion.com/wpf">
    ///     <syncfusion:CircularGauge Name="circularGauge">
    ///         <syncfusion:CircularGauge.Scales>
    ///             <syncfusion:CircularScale Name="circularScale" Radius="100">
    ///                 <syncfusion:CircularScale.PointerCap>
    ///                     <syncfusion:PointerCap BackgroundBrush="Gray" BorderBrush="Black" 
    ///                                            PointerCapRadius="7" BorderWidth="2" CapOnTop="True" />
    ///                 </syncfusion:CircularScale.PointerCap>
    ///                 <syncfusion:CircularScale.Pointers>
    ///                     <syncfusion:CircularPointer BorderWidth="1" PointerWidth="15" 
    ///                                                 PointerLength="100" NeedleStyle="Triangle"
    ///                                                 PointerNeedleType="Needle" PointerPlacement="Cross" 
    ///                                                 Value="50" />
    ///                 </syncfusion:CircularScale.Pointers>
    ///             </syncfusion:CircularScale>
    ///         </syncfusion:CircularGauge.Scales>
    ///     </syncfusion:CircularGauge>
    /// </Window>
    /// </code>
    /// <code lang="C#">
    /// using System;
    /// using System.Windows;
    /// using System.Windows.Controls;
    /// using Syncfusion.Windows.Shared;
    /// using Syncfusion.Windows.Gauge;<para/>
    /// namespace PointerCapSample
    /// {
    ///     public partial class Window1 : Window
    ///     {
    ///         private CircularScale m_scale;
    ///         private CircularGauge m_gauge;
    ///         public Window1()
    ///         {
    ///             InitializeComponent();<para/>
    ///             m_scale = new CircularScale();
    ///             m_gauge = new CircularGauge();        
    ///             m_scale.Radius = 116;          
    ///             this.m_gauge.Scales.Add(m_scale);    
    ///             m_scale.PointerCap.PointerCapRadius = 5;
    ///             m_scale.PointerCap.BackgroundBrush = new RadialGradientBrush(Color.FromRgb(194, 207, 229), Color.FromRgb(46, 94, 160));
    ///             CircularPointer pointer1 = new CircularPointer();
    ///             pointer1.BackgroundBrush = Brushes.Orange;
    ///             pointer1.BorderBrush = Brushes.Black;
    ///             pointer1.PointerLength = 100;
    ///             pointer1.PointerWidth = 10;
    ///             pointer1.PointerPlacement = ScalePlacement.Outside;
    ///             m_scale.Pointers.Add(pointer1);
    ///             this.Content = m_gauge;
    ///         }
    ///     }
    /// }
    /// </code>
    /// </example>
#if SyncfusionFramework4_0
    [System.ComponentModel.DesignTimeVisible(false)]
#endif
    public class PointerCap : GaugeElement
    {

        #region Events

        /// <summary>
        /// Event that is raised when <see cref="PointerCapRadius"/> property is changed.
        /// </summary>
        public event PropertyChangedCallback PointerCapRadiusChanged;

        /// <summary>
        /// Event that is raised when <see cref="PointerCapType"/> property is changed.
        /// </summary>
        public event PropertyChangedCallback PointerCapTypeChanged;

        /// <summary>
        /// Event that is raised when <see cref="Visibility"/> property is changed.
        /// </summary>
        public event PropertyChangedCallback VisibilityChanged;

        /// <summary>
        /// Event that is raised when <see cref="PointerCapCustomGeometry"/> property is changed.
        /// </summary>
        public event PropertyChangedCallback PointerCapCustomGeometryChanged;
        #endregion Events

        #region Dependency Properties
        /// <summary>
        /// Identifies the <see cref="CapOnTop"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty CapOnTopProperty =
            DependencyProperty.Register("CapOnTop", typeof(bool), typeof(PointerCap), new FrameworkPropertyMetadata(true));

        /// <summary>
        /// Identifies the <see cref="PointerCapRadius"/> dependency p0roperty.
        /// </summary>
        public static readonly DependencyProperty PointerCapRadiusProperty =
            DependencyProperty.Register("PointerCapRadius", typeof(double), typeof(PointerCap), new FrameworkPropertyMetadata(0d, FrameworkPropertyMetadataOptions.AffectsRender, new PropertyChangedCallback(OnPointerCapRadiusChanged)));

        /// <summary>
        /// Identifies the <see cref="PointerCapType"/> dependency p0roperty.
        /// </summary>
        public static readonly DependencyProperty PointerCapTypeProperty =
            DependencyProperty.Register("PointerCapType", typeof(PointerCapType), typeof(PointerCap), new FrameworkPropertyMetadata(PointerCapType.Default, new PropertyChangedCallback(OnPointerCapTypeChanged)));

        /// <summary>
        /// Identifies the <see cref="PointerCapCustomGeometryProperty"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty PointerCapCustomGeometryProperty = DependencyProperty.Register("PointerCapCustomGeometry", typeof(Geometry), typeof(PointerCap), new FrameworkPropertyMetadata(null, FrameworkPropertyMetadataOptions.AffectsRender, new PropertyChangedCallback(OnPointerCapCustomGeometryChanged)));
        #endregion Dependency Properties

        #region DP Getters & Setters
        /// <summary>
        /// Gets or sets a value indicating whether a cap locates at the top or at the<para/>
        /// bottom of the needle.
        /// This is a dependency property.
        /// </summary>
        /// <value>
        /// Type: <see cref="bool"/>
        /// Default value is true.
        /// </value>
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
        /// <value>
        /// Type: <see cref="double"/>
        /// Default value is 0.
        /// </value>
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
        /// Gets or sets the cap style.
        /// This is a dependency property.
        /// </summary>
        /// <value>
        /// Type: <see cref="PointerCapType"/>
        /// Default value is PointerCapType.Default.
        /// </value>
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
        /// Gets or sets a custom geometry for the Pointer Cap.
        /// This is a dependency property.
        /// </summary>
        /// <value>
        /// Type: <see cref="Geometry"/>
        /// Default value is null.
        /// </value>
        public Geometry PointerCapCustomGeometry
        {
            get
            {
                return (Geometry)GetValue(PointerCapCustomGeometryProperty);
            }

            set
            {
                SetValue(PointerCapCustomGeometryProperty, value);
            }
        }
        #endregion DP Getters & Setters

        #region Initialization
        /// <summary>
        /// Initializes static members of the <see cref="PointerCap"/> class.
        /// Overrides some dependency properties.
        /// </summary>
        static PointerCap()
        {
            EnvironmentTest.ValidateLicense(typeof(PointerCap));
            VisibilityProperty.OverrideMetadata(typeof(PointerCap), new FrameworkPropertyMetadata(OnVisibilityChanged));
            DefaultStyleKeyProperty.OverrideMetadata(typeof(PointerCap), new FrameworkPropertyMetadata(typeof(PointerCap)));
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="PointerCap"/> class.
        /// </summary>
        public PointerCap()
        {
        }
        #endregion Initialization

        #region Overrides
        /// <summary>
        /// Measures the size in layout required for child elements 
        /// and determines a size for the cap element.
        /// </summary>
        /// <param name="constraint">The available size that this element can give to child elements.</param>
        /// <returns>The size that this element determines it needs during layout.</returns>
        protected override Size MeasureOverride(Size constraint)
        {
            return new Size(this.PointerCapRadius, this.PointerCapRadius);
        }

        /// <summary>
        /// Participates in rendering operations that are directed by the layout system.
        /// </summary>
        /// <param name="drawingContext">The drawing instructions for a specific element.</param>
        protected override void OnRender(DrawingContext drawingContext)
        {
            base.OnRender(drawingContext);
            if (this.VisualParent is CircularScale)
            {
                if (this.PointerCapType == PointerCapType.Custom)
                {
                    if (this.PointerCapCustomGeometry != null)
                    {
                        drawingContext.DrawGeometry(this.BackgroundBrush, new Pen(this.BorderBrush, this.BorderWidth), PointerCapCustomGeometry);
                    }
                    else
                    {
                        drawingContext.DrawEllipse(this.BackgroundBrush, new Pen(this.BorderBrush, this.BorderWidth), new Point(0, 0), this.PointerCapRadius * 2, this.PointerCapRadius * 2);
                    }
                }
                else
                {
                    drawingContext.DrawEllipse(this.BackgroundBrush, new Pen(this.BorderBrush, this.BorderWidth), new Point(0, 0), this.PointerCapRadius * 2, this.PointerCapRadius * 2);
                }
            }
        }

        /// <summary>
        ///  Raises the System.Windows.FrameworkElement.SizeChanged event, using the specified
        ///  information as part of the eventual event data.
        /// </summary>
        /// <param name="sizeInfo">Details of the old and new size involved in the change.</param>
        protected override void OnRenderSizeChanged(SizeChangedInfo sizeInfo)
        {
            base.OnRenderSizeChanged(sizeInfo);
            if (!double.IsNaN(this.Width) && this.Width > 0)
            {
                this.PointerCapRadius = this.Width / 2;
            }
        }

        #endregion Overrides

        #region Implementation

        /// <summary>
        /// Updates property value cache and raises <see cref="PointerCapRadiusChanged"/> event.
        /// </summary>
        /// <param name="e">
        /// Property change details, such as old value and new value.</param>
        protected virtual void OnPointerCapRadiusChanged(DependencyPropertyChangedEventArgs e)
        {
            if (PointerCapRadiusChanged != null)
            {
                this.PointerCapRadiusChanged(this, e);
            }
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
        /// Updates property value cache and raises <see cref="PointerCapTypeChanged"/> event.
        /// </summary>
        /// <param name="e">
        /// Property change details, such as old value and new value.</param>
        protected virtual void OnPointerCapTypeChanged(DependencyPropertyChangedEventArgs e)
        {
            if (PointerCapTypeChanged != null)
            {
                this.PointerCapTypeChanged(this, e);
            }
        }

        /// <summary>
        /// Raises when value of Visibility depencency property is changed.
        /// </summary>
        /// <param name="d">The <see cref="PointerCap"/> object.</param>
        /// <param name="e">The <see cref="System.Windows.DependencyPropertyChangedEventArgs"/> that contains the event data.</param>
        private static void OnVisibilityChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            PointerCap owner = d as PointerCap;
            if (owner != null)
            {
                if (owner.VisibilityChanged != null)
                {
                    owner.VisibilityChanged(owner, e);
                }
            }
        }

        /// <summary>
        /// Calls OnPointerCapCustomGeometryChanged method of the instance, notifies of the dependency property value changes.
        /// </summary>
        /// <param name="d">Dependency object, the change occures on.</param>
        /// <param name="e">Property change details, such as old value and new value.</param>
        private static void OnPointerCapCustomGeometryChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            PointerCap instance = (PointerCap)d;
            instance.OnPointerCapCustomGeometryChanged(e);
        }

        /// <summary>
        /// Updates property value cache and raises PointerCapCustomGeometryChanged event.
        /// </summary>
        /// <param name="e">
        /// Property change details, such as old value and new value.</param>
        protected virtual void OnPointerCapCustomGeometryChanged(DependencyPropertyChangedEventArgs e)
        {
            if (PointerCapCustomGeometryChanged != null)
            {
                this.PointerCapCustomGeometryChanged(this, e);
            }
        }
        #endregion Implementation

        #region Added Code
        /// <summary>
        /// Raises the <see cref="System.Windows.FrameworkElement.Initialized"/> event. 
        /// This method is invoked whenever <see cref="System.Windows.FrameworkElement.IsInitialized"/> property 
        /// is set to true internally.
        /// </summary>
        /// <param name="e">The <see cref="System.EventArgs"/> that contains the event data.</param>
        protected override void OnInitialized(EventArgs e)
        {
            base.OnInitialized(e);
            NameScope.SetNameScope(this, null);
           
        }

        /// <summary>
        /// Sets the scope to <see cref="CircularGauge"/>to facilitate Binding. 
        /// This method is invoked during initialization.
        /// </summary>
        /// <param name="gauge">The <see cref="CircularGauge"/> that contains the reference to the Gauge.</param>
        internal void SetScope(CircularGauge gauge)
        {
            CalculateScope(gauge);
        }

        /// <summary>
        /// Calculates the scope. 
        /// This method is invoked during initialization.
        /// </summary>
        /// <param name="obj">The <see cref="DependencyObject"/> which contains the element to set the scope for.</param>
        internal void CalculateScope(DependencyObject obj)
        {
            DependencyObject ele = obj;
            while (ele != null)
            {
                INameScope ns = NameScope.GetNameScope(ele);
                if (ns != null)
                {
                    if (!(ns is System.Windows.NameScope))
                    {
                        break;
                    }

                    NameScope.SetNameScope(this, ns);
                    break;
                }

                ele = LogicalTreeHelper.GetParent(ele) ?? VisualTreeHelper.GetParent(ele);
            }
        }
        #endregion Added Code
    }
}
