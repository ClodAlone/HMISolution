// <copyright file="Magnifier.cs" company="Syncfusion">
// Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Any infringement will be prosecuted under
// applicable laws. 
// </copyright>

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
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.IO;
using System.Windows.Xps.Packaging;
using System.IO.Packaging;
using System.Windows.Xps;
using System.Globalization;
using System.ComponentModel;
using Syncfusion.Licensing;

namespace Syncfusion.Windows.Shared
{
    /// <summary>
    /// Represents the Magnifier control. 
    /// Magnifier is used to zoom the visual elements on the screen and enables to view the objects closely. 
    /// It contains the zoomed image of the area around the mouse pointer. 
    /// </summary>
    /// <example>
    /// <para/>This example shows how to create the Magnifier control in XAML.
    /// <code><![CDATA[<Window x:Class="MagnifierDemo.SampleWindow"
    /// xmlns="http://schemas.microsoft.com/winfx/2006/xaml/presentation"
    /// xmlns:x="http://schemas.microsoft.com/winfx/2006/xaml"
    /// xmlns:shared="clr-namespace:Syncfusion.Windows.Shared;assembly=Syncfusion.Shared.WPF"/>
    /// <Grid Name="grid">
    ///      <shared:Magnifier.Current>
    ///          <shared:Magnifier ZoomFactor="0.5" FrameBackground="White"/>
    ///      </shared:Magnifier.Current>
    /// </Grid>
    /// </Window> ]]></code>
    /// <para/>
    /// <para/>This example shows how to the Magnifier control in C#.
    /// <code>    
    /// using System;
    /// using System.Windows;
    /// using Syncfusion.Windows.Shared;
    /// namespace MagnifierDemo
    /// {
    ///     public partial class SampleWindow : Window
    ///     {
    ///         internal System.Windows.Controls.Grid grid;
    ///         private Magnifier magnifier = new Magnifier();
    ///         <para/>
    ///         public SampleWindow()
    ///         {
    ///           magnifier.TargetElement = grid;
    ///           magnifier.FrameBackground = Brushes.White;
    ///           magnifier.ZoomFactor = 0.5;
    ///         }
    ///     }
    /// }
    /// </code>
    /// </example>
    /// <remarks>
    /// In XAML code Magnifier.Current attached property plays the same role as Magnifier.TargetElement property in procedural code. There is no need to set TargetElement value in XAML under <![CDATA[ <Magnifier.Current> ]]> definition.
    /// </remarks>
#if SyncfusionFramework4_0
    [System.ComponentModel.DesignTimeVisible(false)]
#endif
    public class Magnifier : Control
    {
        #region Private Members

        /// <summary>
        ///  Define Is added
        /// </summary>
        private bool m_bIsAdded = false;

        /// <summary>
        /// Define adorner layer
        /// </summary>
        private AdornerLayer m_adornerLayer;

        /// <summary>
        /// Define Adorner
        /// </summary>
        private MagnifierAdorner mAdorner;

        /// <summary>
        /// Define IsInDesign mode
        /// </summary>
        private static readonly bool c_isInDesignMode;

        #endregion

        #region Constants

        /// <summary>
        /// Define Default DPI
        /// </summary>
        private const int CdefaultDPI = 96;

        #endregion

        #region Constructors

        /// <summary>
        /// Initializes static members of the <see cref="Magnifier"/> class.
        /// </summary>
        static Magnifier()
        {
          //  EnvironmentTest.ValidateLicense(typeof(Magnifier));
            DefaultStyleKeyProperty.OverrideMetadata(typeof(Magnifier), new FrameworkPropertyMetadata(typeof(Magnifier)));
            c_isInDesignMode = (bool)DesignerProperties.IsInDesignModeProperty.GetMetadata(typeof(DependencyObject)).DefaultValue;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="Magnifier"/> class.
        /// </summary>
        public Magnifier()
        {
            if (EnvironmentTest.IsSecurityGranted)
            {
                EnvironmentTest.StartValidateLicense(typeof(Magnifier));
            }
            UpdateTemplateDependentProperties();
            SizeChanged += new SizeChangedEventHandler(Magnifier_SizeChanged);
        }

        #endregion

        #region Properties

        /// <summary>
        /// Gets or sets the value specifying the shape of frame in the Magnifier.
        /// </summary>
        /// <value>
        /// Type: <see cref="FrameType"/> enum.
        /// Default value is Rectangle.
        /// </value>
        /// <seealso cref="FrameType"/>
        public FrameType FrameType
        {
            get
            {
                return (FrameType)GetValue(FrameTypeProperty);
            }

            set
            {
                SetValue(FrameTypeProperty, value);
            }
        }

        /// <summary>
        /// Gets or sets the value specifying height of the frame when the FrameLayout is Rectangle or Rounded Rectangle.
        /// </summary>
        /// <value>
        /// Type: <see cref="double"/>.
        /// Default value is 200.
        /// </value>
        public double FrameHeight
        {
            get
            {
                return (double)GetValue(FrameHeightProperty);
            }

            set
            {
                SetValue(FrameHeightProperty, value);
            }
        }

        /// <summary>
        /// Gets or sets the value specifying width of the frame when the FrameLayout is Rectangle or Rounded Rectangle.
        /// </summary>
        /// <value>
        /// Type: <see cref="double"/>.
        /// Default value is 200.
        /// </value>
        public double FrameWidth
        {
            get
            {
                return (double)GetValue(FrameWidthProperty);
            }

            set
            {
                SetValue(FrameWidthProperty, value);
            }
        }

        /// <summary>
        /// Gets or sets the value specifying the radius of the circle when the FrameLayout is Circle.
        /// </summary>
        /// <value>
        /// Type: <see cref="double"/>.
        /// Default value is 100.
        /// </value>
        public double FrameRadius
        {
            get
            {
                return (double)GetValue(FrameRadiusProperty);
            }

            set
            {
                SetValue(FrameRadiusProperty, value);
            }
        }

        /// <summary>
        /// Gets or sets the value specifying the corner radius when the FrameLayout is RoundedRectangle.
        /// </summary>
        /// <value>
        /// Type: <see cref="double"/>.
        /// Default value is 5.
        /// </value>
        public double FrameCornerRadius
        {
            get
            {
                return (double)GetValue(FrameCornerRadiusProperty);
            }

            set
            {
                SetValue(FrameCornerRadiusProperty, value);
            }
        }

        /// <summary>
        /// Gets or sets the value specifying background brush of the frame.
        /// </summary>
        /// <value>
        /// Type: <see cref="Brush"/>.
        /// Default value is transparent brush.
        /// </value>
        public Brush FrameBackground
        {
            get
            {
                return (Brush)GetValue(FrameBackgroundProperty);
            }

            set
            {
                SetValue(FrameBackgroundProperty, value);
            }
        }

        /// <summary>
        /// Gets or sets the value indicating relative size of the area displayed inside Magnifier.
        /// This determines zoom level. The value range is between 0.0 (infinite zoom) and 1.0 (no zoom).
        /// </summary>
        /// <value>
        /// Type: <see cref="double"/>.
        /// Default value is 1.0.
        /// </value>
        /// <remarks>
        /// When user is setting value greater than 1.0, ZoomFactor will equal to 1.0, in case of value less than zero ZoomFactor will be equal zero.
        /// </remarks>
        public double ZoomFactor
        {
            get
            {
                return (double)GetValue(ZoomFactorProperty);
            }

            set
            {
                SetValue(ZoomFactorProperty, value);
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether [enable export].
        /// </summary>
        /// <value><c>true</c> if [enable export]; otherwise, <c>false</c>.</value>
        public bool EnableExport
        {
            get
            {
                return (bool)GetValue(EnableExportProperty);
            }

            set
            {
                SetValue(EnableExportProperty, value);
            }
        }

        /// <summary>
        /// Gets or sets UIElement available for magnifying.
        /// </summary>
        /// <value>
        /// Type: <see cref="UIElement"/>.
        /// Default value is null.
        /// </value>
        /// <remarks>
        /// Null value means that the magnifier is inactive.
        /// </remarks>
        public UIElement TargetElement
        {
            get
            {
                return (UIElement)GetValue(TargetElementProperty);
            }

            set
            {
                SetValue(TargetElementProperty, value);
            }
        }

        /// <summary>
        /// Gets or sets the view box
        /// </summary>
        /// <value>The view box</value>
        internal Rect Viewbox
        {
            get
            {
                return (Rect)GetValue(ViewboxProperty);
            }

            set
            {
                SetValue(ViewboxProperty, value);
            }
        }

        /// <summary>
        /// Gets the size of the current.
        /// </summary>
        /// <value>The size of the current.</value>
        internal Size CurrentSize
        {
            get
            {
                switch (FrameType)
                {
                    case FrameType.Circle:
                        return new Size(2 * FrameRadius, 2 * FrameRadius);
                    case FrameType.Rectangle:
                    case FrameType.RoundedRectangle:
                        return new Size(FrameWidth, FrameHeight);
                    default:
                        return Size.Empty;
                }
            }
        }

        /// <summary>
        /// Gets or sets the width of the background.
        /// </summary>
        /// <value>The width of the background.</value>
        internal double BackgroundWidth
        {
            get
            {
                return (double)GetValue(BackgroundWidthProperty);
            }

            set
            {
                SetValue(BackgroundWidthProperty, value);
            }
        }

        /// <summary>
        /// Gets or sets the height of the background.
        /// </summary>
        /// <value>The height of the background.</value>
        internal double BackgroundHeight
        {
            get
            {
                return (double)GetValue(BackgroundHeightProperty);
            }

            set
            {
                SetValue(BackgroundHeightProperty, value);
            }
        }

        /// <summary>
        /// Gets or sets the actual target element.
        /// </summary>
        /// <value>The actual target element.</value>
        internal UIElement ActualTargetElement
        {
            get
            {
                return (UIElement)GetValue(ActualTargetElementProperty);
            }

            set
            {
                SetValue(ActualTargetElementProperty, value);
            }
        }

        #endregion

        #region Events

        /// <summary>
        /// Event that is raised when <see cref="FrameType"/> property is changed.
        /// </summary>
        public event PropertyChangedCallback FrameTypeChanged;

        /// <summary>
        /// Event that is raised when <see cref="FrameHeight"/> property is changed.
        /// </summary>
        public event PropertyChangedCallback FrameHeightChanged;

        /// <summary>
        /// Event that is raised when <see cref="FrameWidth"/> property is changed.
        /// </summary>
        public event PropertyChangedCallback FrameWidthChanged;

        /// <summary>
        /// Event that is raised when <see cref="FrameRadius"/> property is changed.
        /// </summary>
        public event PropertyChangedCallback FrameRadiusChanged;

        /// <summary>
        /// Event that is raised when <see cref="FrameCornerRadius"/> property is changed.
        /// </summary>
        public event PropertyChangedCallback FrameCornerRadiusChanged;

        /// <summary>
        /// Event that is raised when <see cref="FrameBackground"/> property is changed.
        /// </summary>
        public event PropertyChangedCallback FrameBackgroundChanged;

        /// <summary>
        /// Event that is raised when <see cref="ZoomFactor"/> property is changed.
        /// </summary>
        public event PropertyChangedCallback ZoomFactorChanged;

        /// <summary>
        /// Event that is raised when <see cref="EnableExport"/> property is changed.
        /// </summary>
        public event PropertyChangedCallback EnableExportChanged;

        /// <summary>
        /// Event that is raised when <see cref="TargetElement"/> property is changed.
        /// </summary>
        public event PropertyChangedCallback TargetElementChanged;

        /// <summary>
        /// Event that is raised when <see cref="TargetElement"/> property is being changed.
        /// </summary>
        protected internal event CoerceValueCallback TargetElementChanging;
        #endregion

        #region Dependency / Attached properties

        /// <summary>
        /// Identifies the <see cref="FrameType"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty FrameTypeProperty =
          DependencyProperty.Register(
          "FrameType",
          typeof(FrameType),
          typeof(Magnifier),
          new FrameworkPropertyMetadata(FrameType.Rectangle, new PropertyChangedCallback(OnFrameTypeChanged)));

        /// <summary>
        /// Identifies the <see cref="FrameHeight"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty FrameHeightProperty =
          DependencyProperty.Register(
          "FrameHeight",
          typeof(double),
          typeof(Magnifier),
          new FrameworkPropertyMetadata(200.0, new PropertyChangedCallback(OnFrameHeightChanged)));

        /// <summary>
        /// Identifies the <see cref="FrameWidth"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty FrameWidthProperty =
          DependencyProperty.Register(
          "FrameWidth",
          typeof(double),
          typeof(Magnifier),
          new FrameworkPropertyMetadata(200.0, new PropertyChangedCallback(OnFrameWidthChanged)));

        /// <summary>
        /// Identifies the <see cref="FrameRadius"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty FrameRadiusProperty =
          DependencyProperty.Register(
          "FrameRadius",
          typeof(double),
          typeof(Magnifier),
          new FrameworkPropertyMetadata(100.0, new PropertyChangedCallback(OnFrameRadiusChanged)));

        /// <summary>
        /// Identifies the <see cref="FrameCornerRadius"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty FrameCornerRadiusProperty =
          DependencyProperty.Register(
          "FrameCornerRadius",
          typeof(double),
          typeof(Magnifier),
          new FrameworkPropertyMetadata(5.0, new PropertyChangedCallback(OnFrameCornerRadiusChanged)));

        /// <summary>
        /// Identifies the <see cref="FrameBackground"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty FrameBackgroundProperty =
          DependencyProperty.Register(
          "FrameBackground",
          typeof(Brush),
          typeof(Magnifier),
          new FrameworkPropertyMetadata(new SolidColorBrush(), new PropertyChangedCallback(OnFrameBackgroundChanged)));

        /// <summary>
        /// Identifies the <see cref="ZoomFactor"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty ZoomFactorProperty =
          DependencyProperty.Register(
          "ZoomFactor",
          typeof(double),
          typeof(Magnifier),
          new FrameworkPropertyMetadata(1.0, new PropertyChangedCallback(OnZoomFactorChanged), new CoerceValueCallback(CoerceZoomFactor)));

        /// <summary>
        /// Identifies the <see cref="EnableExport"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty EnableExportProperty =
          DependencyProperty.Register(
          "EnableExport",
          typeof(bool),
          typeof(Magnifier),
          new FrameworkPropertyMetadata(true, new PropertyChangedCallback(OnEnableExportChanged)));

        /// <summary>
        /// Identifies the <see cref="TargetElement"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty TargetElementProperty =
          DependencyProperty.Register(
          "TargetElement",
          typeof(UIElement),
          typeof(Magnifier),
          new FrameworkPropertyMetadata(null, new PropertyChangedCallback(OnTargetElementChanged), new CoerceValueCallback(CoerceTargetElement)));

        /// <summary>
        /// Identifies the Magnifier.Current attached dependency property.
        /// </summary>
        public static readonly DependencyProperty CurrentProperty =
          DependencyProperty.RegisterAttached(
          "Current",
          typeof(Magnifier),
          typeof(Magnifier),
          new FrameworkPropertyMetadata(null, new PropertyChangedCallback(OnCurrentChanged)));

        /// <summary>
        /// Identifies the <see cref="Viewbox"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty ViewboxProperty =
          DependencyProperty.Register(
          "Viewbox",
          typeof(Rect),
          typeof(Magnifier),
          new FrameworkPropertyMetadata(Rect.Empty));

        /// <summary>
        /// Identifies the <see cref="BackgroundWidth"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty BackgroundWidthProperty =
          DependencyProperty.Register("BackgroundWidth", typeof(double), typeof(Magnifier));

        /// <summary>
        /// Identifies the <see cref="BackgroundHeight"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty BackgroundHeightProperty =
          DependencyProperty.Register("BackgroundHeight", typeof(double), typeof(Magnifier));

        /// <summary>
        /// Identifies the <see cref="ActualTargetElement"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty ActualTargetElementProperty =
          DependencyProperty.Register("ActualTargetElement", typeof(UIElement), typeof(Magnifier));

        #endregion

        #region Implementation

        /// <summary>
        /// Calls OnFrameTypeChanged method of the instance,
        /// notifies of the dependency property value changes.
        /// </summary>
        /// <param name="d">Dependency object, the change occurs on.</param>
        /// <param name="e">Property change details, such as old value and new value.</param>
        private static void OnFrameTypeChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            Magnifier instance = (Magnifier)d;
            instance.OnFrameTypeChanged(e);
        }

        /// <summary>
        /// Updates property value cache and raises <see cref="FrameTypeChanged"/> event.
        /// </summary>
        /// <param name="e">Property change details, such as old value and new value.</param>
        protected virtual void OnFrameTypeChanged(DependencyPropertyChangedEventArgs e)
        {
            UpdateTemplateDependentProperties();
            if (FrameTypeChanged != null)
            {
                FrameTypeChanged(this, e);
            }
        }

        /// <summary>
        /// Calls OnFrameHeightChanged method of the instance,
        /// notifies of the dependency property value changes.
        /// </summary>
        /// <param name="d">Dependency object, the change occurs on.</param>
        /// <param name="e">Property change details, such as old value and new value.</param>
        private static void OnFrameHeightChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            Magnifier instance = (Magnifier)d;
            instance.OnFrameHeightChanged(e);
        }

        /// <summary>
        /// Updates property value cache and raises <see cref="FrameHeightChanged"/> event.
        /// </summary>
        /// <param name="e">Property change details, such as old value and new value.</param>
        protected virtual void OnFrameHeightChanged(DependencyPropertyChangedEventArgs e)
        {
            if (FrameType == FrameType.Rectangle || FrameType == FrameType.RoundedRectangle)
            {
                UpdateTemplateDependentProperties();
            }

            if (FrameHeightChanged != null)
            {
                FrameHeightChanged(this, e);
            }
        }

        /// <summary>
        /// Calls OnFrameWidthChanged method of the instance,
        /// notifies of the dependency property value changes.
        /// </summary>
        /// <param name="d">Dependency object, the change occurs on.</param>
        /// <param name="e">Property change details, such as old value and new value.</param>
        private static void OnFrameWidthChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            Magnifier instance = (Magnifier)d;
            instance.OnFrameWidthChanged(e);
        }

        /// <summary>
        /// Updates property value cache and raises <see cref="FrameWidthChanged"/> event.
        /// </summary>
        /// <param name="e">Property change details, such as old value and new value.</param>
        protected virtual void OnFrameWidthChanged(DependencyPropertyChangedEventArgs e)
        {
            if (FrameType == FrameType.Rectangle || FrameType == FrameType.RoundedRectangle)
            {
                UpdateTemplateDependentProperties();
            }

            if (FrameWidthChanged != null)
            {
                FrameWidthChanged(this, e);
            }
        }

        /// <summary>
        /// Calls OnFrameRadiusChanged method of the instance,
        /// notifies of the dependency property value changes.
        /// </summary>
        /// <param name="d">Dependency object, the change occurs on.</param>
        /// <param name="e">Property change details, such as old value and new value.</param>
        private static void OnFrameRadiusChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            Magnifier instance = (Magnifier)d;
            instance.OnFrameRadiusChanged(e);
        }

        /// <summary>
        /// Updates property value cache and raises <see cref="FrameRadiusChanged"/> event.
        /// </summary>
        /// <param name="e">Property change details, such as old value and new value.</param>
        protected virtual void OnFrameRadiusChanged(DependencyPropertyChangedEventArgs e)
        {
            if (FrameType == FrameType.Circle)
            {
                UpdateTemplateDependentProperties();
            }

            if (FrameRadiusChanged != null)
            {
                FrameRadiusChanged(this, e);
            }
        }

        /// <summary>
        /// Calls OnFrameCornerRadiusChanged method of the instance,
        /// notifies of the dependency property value changes.
        /// </summary>
        /// <param name="d">Dependency object, the change occurs on.</param>
        /// <param name="e">Property change details, such as old value and new value.</param>
        private static void OnFrameCornerRadiusChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            Magnifier instance = (Magnifier)d;
            instance.OnFrameCornerRadiusChanged(e);
        }

        /// <summary>
        /// Updates property value cache and raises <see cref="FrameCornerRadiusChanged"/> event.
        /// </summary>
        /// <param name="e">Property change details, such as old value and new value.</param>
        protected virtual void OnFrameCornerRadiusChanged(DependencyPropertyChangedEventArgs e)
        {
            if (FrameCornerRadiusChanged != null)
            {
                FrameCornerRadiusChanged(this, e);
            }
        }

        /// <summary>
        /// Calls OnFrameBackgroundChanged method of the instance,
        /// notifies of the dependency property value changes.
        /// </summary>
        /// <param name="d">Dependency object, the change occurs on.</param>
        /// <param name="e">Property change details, such as old value and new value.</param>
        private static void OnFrameBackgroundChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            Magnifier instance = (Magnifier)d;
            instance.OnFrameBackgroundChanged(e);
        }

        /// <summary>
        /// Updates property value cache and raises <see cref="FrameBackgroundChanged"/> event.
        /// </summary>
        /// <param name="e">Property change details, such as old value and new value.</param>
        protected virtual void OnFrameBackgroundChanged(DependencyPropertyChangedEventArgs e)
        {
            if (FrameBackgroundChanged != null)
            {
                FrameBackgroundChanged(this, e);
            }
        }

        /// <summary>
        /// Calls OnZoomFactorChanged method of the instance,
        /// notifies of the dependency property value changes.
        /// </summary>
        /// <param name="d">Dependency object, the change occurs on.</param>
        /// <param name="e">Property change details, such as old value and new value.</param>
        private static void OnZoomFactorChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            Magnifier instance = (Magnifier)d;
            instance.OnZoomFactorChanged(e);
        }

        /// <summary>
        /// Updates property value cache and raises <see cref="ZoomFactorChanged"/> event.
        /// </summary>
        /// <param name="e">Property change details, such as old value and new value.</param>
        protected virtual void OnZoomFactorChanged(DependencyPropertyChangedEventArgs e)
        {
            UpdateTemplateDependentProperties();
            if (ZoomFactorChanged != null)
            {
                ZoomFactorChanged(this, e);
            }
        }

        /// <summary>
        /// Calls CoerceZoomFactor method of the instance.
        /// </summary>
        /// <param name="d">Current Magnifier instance.</param>
        /// <param name="baseValue">New value.</param>
        /// <returns>Value that should be set.</returns>
        private static object CoerceZoomFactor(DependencyObject d, object baseValue)
        {
            Magnifier instance = (Magnifier)d;
            return instance.CoerceZoomFactor(baseValue);
        }

        /// <summary>
        /// Fulfils the logic before setting the value of <see cref="ZoomFactor"/> dependency property.
        /// </summary>
        /// <param name="baseValue">The value that should be corrected.</param>
        /// <returns>Corrected value.</returns>
        protected virtual object CoerceZoomFactor(object baseValue)
        {
            double zoomFactor = (double)baseValue;
            if (zoomFactor > 1)
            {
                return 1;
            }
            else if (zoomFactor < 0)
            {
                return 0;
            }
            else
            {
                return baseValue;
            }
        }

        /// <summary>
        /// Calls OnEnableExportChanged method of the instance,
        /// notifies of the dependency property value changes.
        /// </summary>
        /// <param name="d">Dependency object, the change occurs on.</param>
        /// <param name="e">Property change details, such as old value and new value.</param>
        private static void OnEnableExportChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            Magnifier instance = (Magnifier)d;
            instance.OnEnableExportChanged(e);
        }

        /// <summary>
        /// Updates property value cache and raises <see cref="EnableExportChanged"/> event.
        /// </summary>
        /// <param name="e">Property change details, such as old value and new value.</param>
        protected virtual void OnEnableExportChanged(DependencyPropertyChangedEventArgs e)
        {
            if (EnableExportChanged != null)
            {
                EnableExportChanged(this, e);
            }
        }

        /// <summary>
        /// Calls OnTargetElementChanged method of the instance,
        /// notifies of the dependency property value changes.
        /// </summary>
        /// <param name="d">Dependency object, the change occurs on.</param>
        /// <param name="e">Property change details, such as old value and new value.</param>
        private static void OnTargetElementChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            Magnifier instance = (Magnifier)d;
            instance.OnTargetElementChanged(e);
        }

        /// <summary>
        /// Updates property value cache and raises <see cref="TargetElementChanged"/> event.
        /// </summary>
        /// <param name="e">Property change details, such as old value and new value.</param>
        protected virtual void OnTargetElementChanged(DependencyPropertyChangedEventArgs e)
        {
            ChangeTargetElement(e.OldValue as UIElement, e.NewValue as UIElement);
            if (TargetElementChanged != null)
            {
                TargetElementChanged(this, e);
            }
        }

        /// <summary>
        /// Calls CoerceTargetElement method of the instance.
        /// </summary>
        /// <param name="d">Current Magnifier instance.</param>
        /// <param name="baseValue">New value.</param>
        /// <returns>Value that should be set.</returns>
        private static object CoerceTargetElement(DependencyObject d, object baseValue)
        {
            Magnifier instance = (Magnifier)d;
            return instance.CoerceTargetElement(baseValue);
        }

        /// <summary>
        /// Fulfils the logic before setting the value of <see cref="TargetElement"/> dependency property.
        /// </summary>
        /// <param name="baseValue">The value that should be corrected.</param>
        /// <returns>Corrected value.</returns>
        protected virtual object CoerceTargetElement(object baseValue)
        {
            if (TargetElementChanging != null)
            {
                TargetElementChanging(this, baseValue);
            }

            return baseValue;
        }

        /// <summary>
        /// Calls OnCurrentChanged method of the instance,
        /// notifies of the dependency property value changes.
        /// </summary>
        /// <param name="d">Dependency object, the change occurs on.</param>
        /// <param name="e">Property change details, such as old value and new value.</param>
        private static void OnCurrentChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            UIElement target = d as UIElement;

            if (target == null)
            {
                throw new ArgumentException("incorrect type");
            }

            if (e.OldValue != null)
            {
                Magnifier magnifier = e.OldValue as Magnifier;
                magnifier.TargetElement = null;
            }

            if (e.NewValue != null)
            {
                Magnifier magnifier = e.NewValue as Magnifier;
                magnifier.TargetElement = target;
            }
        }

        #endregion

        #region Public methods

        /// <summary>
        /// Attaches the Magnifier to the UIElement specified in the parameter and receives ability to zoom it with all visual children.
        /// </summary>
        /// <param name="target">The <see cref="UIElement"/> which the Magnifier has ability to zoom.</param>
        public void AssociateWith(UIElement target)
        {
            TargetElement = target;
        }

        /// <summary>
        /// Zooms in the area around the mouse pointer with the factor mentioned as the parameter.
        /// </summary>
        /// <param name="zoomFactor">Factor to zoom in.</param>
        public void ZoomIn(double zoomFactor)
        {
            ZoomFactor /= zoomFactor;
        }

        /// <summary>
        /// Zooms out the area around the mouse pointer with the factor mentioned as the parameter.
        /// </summary>
        /// <param name="zoomFactor">Factor to zoom out.</param>
        public void ZoomOut(double zoomFactor)
        {
            ZoomFactor *= zoomFactor;
        }

        /// <summary>
        /// Sets value of the Magnifier.Current attached property.
        /// </summary>
        /// <param name="d">The d control.</param>
        /// <param name="magnifier">The magnifier.</param>
        public static void SetCurrent(DependencyObject d, Magnifier magnifier)
        {
            d.SetValue(CurrentProperty, magnifier);
        }

        /// <summary>
        /// Gets value of the Magnifier.Current attached property.
        /// </summary>
        /// <param name="d">The d control.</param>
        /// <returns>Return the current control</returns>
        public static Magnifier GetCurrent(DependencyObject d)
        {
            return d.GetValue(CurrentProperty) as Magnifier;
        }

        /// <summary>
        /// Saves content of the Magnifier to the specified stream.
        /// </summary>
        /// <param name="stream">The stream.</param>
        /// <param name="encoder">The encoder.</param>
        /// <exception cref="ArgumentNullException">Magnifier area cannot be retrieved.</exception>
        public void Save(Stream stream, BitmapEncoder encoder)
        {
            if (EnableExport)
            {
                Visual visual = Template.FindName("PART_MagnifierArea", this) as Visual;

                if (visual != null)
                {
                    RenderTargetBitmap bmpSource = new RenderTargetBitmap(
                        (int)this.CurrentSize.Width,
                        (int)this.CurrentSize.Height,
                        CdefaultDPI,
                        CdefaultDPI,
                        PixelFormats.Default);

                    Rectangle backgroundRect = new Rectangle();

                    backgroundRect.Fill = Brushes.Transparent;
                    backgroundRect.Arrange(new Rect(this.RenderSize));

                    bmpSource.Render(backgroundRect);
                    bmpSource.Render(visual);

                    encoder.Frames.Add(BitmapFrame.Create(bmpSource));
                    encoder.Save(stream);
                }
                else
                {
                    throw new ArgumentNullException("visual");
                }
            }
        }

        /// <summary>
        /// Saves content of the Magnifier to the specified stream.
        /// </summary>
        /// <param name="stream">The stream.</param>
        public void Save(Stream stream)
        {
            if (EnableExport)
            {
                this.Save(stream, new BmpBitmapEncoder());
            }
        }

        /// <summary>
        /// Saves content of the Magnifier to the file with specified filename.
        /// </summary>
        /// <param name="fileName">The filename.</param>
        public void Save(string fileName)
        {
            if (EnableExport)
            {
                string extension = new FileInfo(fileName).Extension.ToLower(CultureInfo.InvariantCulture);

                using (Stream stream = File.Create(fileName))
                {
                    if (extension == ".xps")
                    {
                        this.SaveToXps(stream);
                    }
                    else
                    {
                        this.Save(stream, this.CreateEncoderByExtension(extension));
                    }
                }
            }
        }

        /// <summary>
        /// Saves content of the Magnifier to the file with specified filename using encoder.
        /// </summary>
        /// <param name="fileName">The fileName.</param>
        /// <param name="encoder">The encoder.</param>
        public void Save(string fileName, BitmapEncoder encoder)
        {
            if (EnableExport)
            {
                Stream stream = File.Create(fileName);
                this.Save(stream, encoder);
            }
        }

        /// <summary>
        /// Saves content of the Magnifier to XPS format.
        /// </summary>
        /// <param name="stream">The stream.</param>
        public void SaveToXps(Stream stream)
        {
            if (EnableExport)
            {
                Visual visual = Template.FindName("PART_MagnifierArea", this) as Visual;

                if (visual != null)
                {
                    Package package = Package.Open(stream, FileMode.Create, FileAccess.ReadWrite);
                    XpsDocument doc = new XpsDocument(package, CompressionOption.Normal);
                    XpsDocumentWriter writer = XpsDocument.CreateXpsDocumentWriter(doc);

                    writer.Write(visual);

                    doc.Close();
                    package.Close();
                }
                else
                {
                    throw new ArgumentNullException("visual");
                }
            }
        }

        /// <summary>
        /// Saves content of the Magnifier to XPS format.
        /// </summary>
        /// <param name="filename">The filename.</param>
        public void SaveToXps(string filename)
        {
            if (EnableExport)
            {
                Stream stream = File.Create(filename);
                this.SaveToXps(stream);
            }
        }

        /// <summary>
        /// Copies the content of the Magnifier to clipboard.
        /// </summary>
        /// <exception cref="ArgumentNullException">Magnifier area cannot be retrieved.</exception>
        public void CopyToClipboard()
        {
            if (!EnableExport)
            {
                return;
            }

            Visual visual = Template.FindName("PART_MagnifierArea", this) as Visual;

            if (visual != null)
            {
                RenderTargetBitmap bmpSource = new RenderTargetBitmap(
                    (int)this.CurrentSize.Width,
                  (int)this.CurrentSize.Height,
                  CdefaultDPI,
                  CdefaultDPI, 
                  PixelFormats.Default);

                Rectangle backgroundRect = new Rectangle();

                backgroundRect.Fill = Brushes.White;
                backgroundRect.Arrange(new Rect(this.RenderSize));

                bmpSource.Render(backgroundRect);
                bmpSource.Render(visual);

                Clipboard.SetImage(bmpSource);
            }
            else
            {
                throw new ArgumentNullException("visual");
            }
        }

        #endregion

        #region Overrides

        /// <summary>
        /// Override Executes when visual parent of Magnifier has changed.
        /// </summary>
        /// <param name="oldParent">Determines old parent of the control. Not used.</param>
        protected override void OnVisualParentChanged(DependencyObject oldParent)
        {
            if (this.VisualParent is MagnifierAdorner == false)
            {
                //// Magnifier was declared from XAML alone, without using <Magnifier.Current> attached property:
                if (VisualParent is Panel)
                {
                    (VisualParent as Panel).Children.Remove(this);
                }
            }

           base.OnVisualParentChanged(oldParent);
        }

        #endregion

        #region Private methods

        /// <summary>
        /// Performs TargetElement changing logic in the Magnifier.
        /// </summary>
        /// <param name="uieOld">The uie old.</param>
        /// <param name="uieNew">The uie new.</param>
        private void ChangeTargetElement(UIElement uieOld, UIElement uieNew)
        {
            if (uieOld != null)
            {
                if (mAdorner != null && m_adornerLayer != null)
                {
                    m_adornerLayer.Remove(mAdorner);
                    mAdorner.DisconnectMagnifier();
                }

                if (ActualTargetElement != null)
                {
                    ActualTargetElement.MouseEnter -= new MouseEventHandler(TargetElement_MouseEnter);
                    ActualTargetElement.MouseLeave -= new MouseEventHandler(TargetElement_MouseLeave);
                }
            }

            if (uieNew != null)
            {
                m_adornerLayer = AdornerLayer.GetAdornerLayer(uieNew);

                if (m_adornerLayer == null)
                {
                    uieNew.LayoutUpdated += new EventHandler(TargetElement_LayoutUpdated);
                }
                else
                {
                    UpdateActualTargetElement();
                    mAdorner = new MagnifierAdorner(ActualTargetElement, this);
                    m_adornerLayer.Add(mAdorner);
                    ActualTargetElement.MouseEnter += new MouseEventHandler(TargetElement_MouseEnter);
                    ActualTargetElement.MouseLeave += new MouseEventHandler(TargetElement_MouseLeave);
                }

                if (ActualTargetElement != null && ActualTargetElement.IsMouseOver && !c_isInDesignMode)
                {
                    Visibility = Visibility.Visible;
                }
                else
                {
                    Visibility = Visibility.Hidden;
                }
            }
        }

        /// <summary>
        /// Hides Magnifier when mouse cursor moves outside the bounds of TargetElement.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.Windows.Input.MouseEventArgs"/> instance containing the event data.</param>
        private void TargetElement_MouseLeave(object sender, MouseEventArgs e)
        {
            Visibility = Visibility.Hidden;
        }

        /// <summary>
        /// Shows Magnifier when mouse cursor enters TargetElement's area.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.Windows.Input.MouseEventArgs"/> instance containing the event data.</param>
        private void TargetElement_MouseEnter(object sender, MouseEventArgs e)
        {
            Visibility = Visibility.Visible;
        }

        /// <summary>
        /// Provides additional logic after the target element for magnifier is set.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
        private void TargetElement_LayoutUpdated(object sender, EventArgs e)
        {
            if (!m_bIsAdded && !c_isInDesignMode)
            {
                if (m_adornerLayer == null)
                {
                    UpdateActualTargetElement();
                    m_adornerLayer = AdornerLayer.GetAdornerLayer(this.ActualTargetElement);
                    mAdorner = new MagnifierAdorner(this.ActualTargetElement, this);
                    m_adornerLayer.Add(mAdorner);
                    ActualTargetElement.MouseEnter += new MouseEventHandler(TargetElement_MouseEnter);
                    ActualTargetElement.MouseLeave += new MouseEventHandler(TargetElement_MouseLeave);
                }

                m_bIsAdded = true;
            }
        }

        /// <summary>
        /// Cancel size changing of the magnifier using Width or Height property.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.Windows.SizeChangedEventArgs"/> instance containing the event data.</param>
        private void Magnifier_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            if (e.NewSize != this.CurrentSize)
            {
                this.Width = this.CurrentSize.Width;
                this.Height = this.CurrentSize.Height;
            }
        }

        /// <summary>
        /// Updates value of View box and other internal properties the template is depending on.
        /// </summary>
        private void UpdateTemplateDependentProperties()
        {
            double correction = (BorderThickness.Bottom + BorderThickness.Left + BorderThickness.Right + BorderThickness.Top == 0) ? 1 : 0;
            double width = CurrentSize.Width * ZoomFactor;
            double height = CurrentSize.Height * ZoomFactor;

            BackgroundWidth = CurrentSize.Width - correction;
            BackgroundHeight = CurrentSize.Height - correction;

            Viewbox = new Rect(Viewbox.Location, new Size(width, height));
            if (mAdorner != null)
            {
                mAdorner.UpdateMagnifierViewbox();
            }
        }

        /// <summary>
        /// Creates appropriate BitmapEncoder depending on the extension specified.
        /// </summary>
        /// <param name="extension">The extension.</param>
        /// <returns>Return the bitmap encoder</returns>
        private BitmapEncoder CreateEncoderByExtension(string extension)
        {
            BitmapEncoder encoder = null;

            switch (extension)
            {
                case ".bmp":
                    encoder = new BmpBitmapEncoder();
                    break;

                case ".jpg":
                case ".jpeg":
                    encoder = new JpegBitmapEncoder();
                    break;

                case ".png":
                    encoder = new PngBitmapEncoder();
                    break;

                case ".gif":
                    encoder = new GifBitmapEncoder();
                    break;

                case ".tif":
                case ".tiff":
                    encoder = new TiffBitmapEncoder();
                    break;

                case ".wdp":
                    encoder = new WmpBitmapEncoder();
                    break;

                default:
                    encoder = new BmpBitmapEncoder();
                    break;
            }

            return encoder;
        }

        /// <summary>
        /// Updates value of actual target element.
        /// </summary>
        private void UpdateActualTargetElement()
        {
            UIElement uieActual = null;
            Window targetWindow = TargetElement as Window;
            if (targetWindow != null)
            {
                AdornerDecorator decorator = FindAdornerDecorator(targetWindow);
                if (decorator != null)
                {
                    for (int i = 0; i < VisualTreeHelper.GetChildrenCount(decorator);)
                    {
                        uieActual = VisualTreeHelper.GetChild(decorator, i) as UIElement;
                        break;
                    }
                }
            }
            else
            {
                uieActual = TargetElement;
            }

            ActualTargetElement = uieActual;
        }

        /// <summary>
        /// Finds recursively and returns adorner decorator in visual tree starting from rootElement.
        /// </summary>
        /// <param name="rootElement">The root element.</param>
        /// <returns>Return the adorner decorator</returns>
        private AdornerDecorator FindAdornerDecorator(UIElement rootElement)
        {
            AdornerDecorator result = null;
            for (int i = 0; i < VisualTreeHelper.GetChildrenCount(rootElement); i++)
            {
                UIElement child = VisualTreeHelper.GetChild(rootElement, i) as UIElement;
                if (child is AdornerDecorator)
                {
                    result = child as AdornerDecorator;
                    break;
                }
                else if (child != null)
                {
                    result = FindAdornerDecorator(child as UIElement);
                }
            }

            return result;
        }

        #endregion
    }
}
