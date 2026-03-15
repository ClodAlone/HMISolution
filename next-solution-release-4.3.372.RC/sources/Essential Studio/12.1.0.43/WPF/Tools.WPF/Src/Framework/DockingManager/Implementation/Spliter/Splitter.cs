// <copyright file="Splitter.cs" company="Syncfusion">
// Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Any infringement will be prosecuted under
// applicable laws. 
// </copyright>

using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Input;
using Syncfusion.Windows.Shared;

namespace Syncfusion.Windows.Tools.Controls
{
    /// <property name="flag" value="Finished" />
    /// <summary>
    /// Splitter class.
    /// </summary>
    /// <remarks>
    /// When you start resizing, you can see horizontal or vertical line appear when dragging any side of the dock window. 
    /// This line is splitter.
    /// You can override style or template of the Splitter.
    /// </remarks>
    /// <example>
    /// <para/>This example shows how to override the style of the Splitter in XAML.
    /// <code language="XAML">
    /// <![CDATA[
    /// <Style x:Key="SplitterBaseStyle" TargetType="{x:Type Syncfusion:Splitter}">
    /// <Setter Property="MinWidth" Value="{Binding Path=SplitterSize
    /// , RelativeSource={RelativeSource FindAncestor, AncestorType={x:Type Syncfusion:DockingManager}}}"/>
    /// <Setter Property="MinHeight" Value="{Binding Path=SplitterSize
    /// , RelativeSource={RelativeSource FindAncestor, AncestorType={x:Type Syncfusion:DockingManager}}}"/>
    /// <Setter Property="Margin" Value="0"/>
    /// <Setter Property="Background" Value="{Binding Path=SplitterBackground
    /// , RelativeSource={RelativeSource FindAncestor, AncestorType={x:Type Syncfusion:DockingManager}}}" />
    /// <Setter Property="Cursor" Value="SizeNS"/>
    /// <Setter Property="AdornerTemplate" Value="{StaticResource SplitterAdornerTemplate}"/>
    /// <Setter Property="Template" Value="{StaticResource SplitterTemplate}"/>
    /// </Style>
    /// ]]>
    /// </code>
    /// </example>
    /// <seealso cref="SplitterAdorner"/>
#if SyncfusionFramework4_0
    [System.ComponentModel.DesignTimeVisible(false)]
#endif
    public class Splitter : UserControl
    {
        #region Private members
        /// <property name="flag" value="Finished" />
        /// <summary>
        /// Specifies currently used adorner.
        /// </summary>
        private SplitterAdorner m_adorner;

        /// <property name="flag" value="Finished" />
        /// <summary>
        /// Stores currently used adorner layer.
        /// </summary>
        private AdornerLayer m_adorners;

        /// <property name="flag" value="Finished" />
        /// <summary>
        /// Initial point of the mouse click (relative to the parent
        /// panel).
        /// </summary>
        private Point m_pointClick;

        /// <property name="flag" value="Finished" />
        /// <summary>
        /// Cached value of the TestProperty property.
        /// </summary>
        private double m_maxOffsetLeft = double.NaN;

        /// <property name="flag" value="Finished" />
        /// <summary>
        /// Cached value of the TestProperty property.
        /// </summary>
        private double m_maxOffsetRight = double.NaN;

        /// <property name="flag" value="Finished" />
        /// <summary>
        /// Cached value of the TestProperty property.
        /// </summary>
        private double m_offsetStep = 1.0;

        //private bool m_TouchDown;

        private SystemGesture m_SystemGesture;

        //private int m_TouchDeviceId = -1;
        #endregion

        #region Properties

        /// <summary>
        /// Gets a value indicating whether this instance is pressed.
        /// </summary>
        /// <value>
        /// <c>true</c> if this instance is pressed; otherwise, <c>false</c>.
        /// </value>
        internal bool IsPressed
        {
            get
            {
                return (bool)GetValue(IsPressedProperty);
            }
        }

        /// <property name="flag" value="Finished" />
        /// <summary>
        /// Gets or sets adorner template used to show adorner.
        /// </summary>
        /// <example>
        /// <para/>This example shows how to write ControlTemplate for Splitter in XAML.
        /// <code language="XAML">
        /// <![CDATA[
        /// <ControlTemplate x:Key="SplitterAdornerTemplate">
        /// <StackPanel x:Name="stPanel" Orientation="Horizontal">
        /// <Border MinWidth="4" MinHeight="4" >
        /// <Border.Background>
        /// <SolidColorBrush Color="Gray" Opacity="0.6" />
        /// </Border.Background>
        /// </Border>
        /// </StackPanel>
        /// <ControlTemplate.Triggers>
        /// <Trigger Property="Syncfusion:SplitterAdorner.Orientation" Value="Horizontal">
        /// <Setter TargetName="stPanel" Property="LayoutTransform">
        /// <Setter.Value>
        /// <RotateTransform Angle="-90"/>
        /// </Setter.Value>
        /// </Setter>
        /// </Trigger>
        /// </ControlTemplate.Triggers>
        /// </ControlTemplate>
        /// ]]>
        /// </code>
        /// </example>
        /// <example>
        /// <para/>This example shows how to use AdornerTemplate property in XAML.
        /// <code language="XAML">
        /// <![CDATA[
        /// <Style x:Key="SplitterBaseStyle" TargetType="{x:Type Syncfusion:Splitter}">
        /// <Setter Property="AdornerTemplate" Value="{StaticResource SplitterAdornerTemplate}"/>
        /// </Style>
        /// ]]>
        /// </code>
        /// </example>
        /// <seealso cref="ControlTemplate"/>
        public ControlTemplate AdornerTemplate
        {
            get
            {
                return (ControlTemplate)GetValue(AdornerTemplateProperty);
            }

            set
            {
                SetValue(AdornerTemplateProperty, value);
            }
        }

        /// <property name="flag" value="Finished" />
        /// <summary>
        /// Gets or sets the value of the MaxOffsetLeft dependency
        /// property.
        /// </summary>
        internal double MaxOffsetLeft
        {
            get
            {
                return m_maxOffsetLeft;
            }

            set
            {
                SetValue(MaxOffsetLeftProperty, value);
            }
        }

        /// <property name="flag" value="Finished" />
        /// <summary>
        /// Gets or sets the value of the MaxOffsetRight dependency
        /// property.
        /// </summary>
        internal double MaxOffsetRight
        {
            get
            {
                return m_maxOffsetRight;
            }

            set
            {
                SetValue(MaxOffsetRightProperty, value);
            }
        }

        /// <property name="flag" value="Finished" />
        /// <summary>
        /// Gets or sets the value of the OffsetStep dependency property.
        /// </summary>
        internal double OffsetStep
        {
            get
            {
                return m_offsetStep;
            }

            set
            {
                SetValue(OffsetStepProperty, value);
            }
        }

        /// <property name="flag" value="Finished" />
        /// <summary>
        /// Gets or sets splitter orientation.
        /// </summary>
        /// <example>
        /// To set Orientation property please see <see cref="AdornerTemplate"/> property example.
        /// </example>
        public Orientation Orientation
        {
            get
            {
                return (Orientation)GetValue(OrientationProperty);
            }

            set
            {
                SetValue(OrientationProperty, value);
            }
        }

        /// <property name="flag" value="Finished" />
        /// <summary>
        /// Gets or sets the value of the Offset dependency property.
        /// </summary>
        public double Offset
        {
            get
            {
                return (double)GetValue(OffsetProperty);
            }

            protected set
            {
                SetValue(OffsetPropertyKey, value);
            }
        }
        #endregion

        #region Events
        /// <property name="flag" value="Finished" />
        /// <summary>
        /// Event that occurs when splitter's pressed state changes.
        /// </summary>
        public event EventHandler IsPressedChanged;

        /// <property name="flag" value="Finished" />
        /// <summary>
        /// Event that is raised when MaxOffsetLeft property is changed.
        /// </summary>
        public event PropertyChangedCallback MaxOffsetLeftChanged;

        /// <property name="flag" value="Finished" />
        /// <summary>
        /// Event that is raised when MaxOffsetRight property is changed.
        /// </summary>
        public event PropertyChangedCallback MaxOffsetRightChanged;

        /// <property name="flag" value="Finished" />
        /// <summary>
        /// Event that is raised when Offset property is changed.
        /// </summary>
        public event PropertyChangedCallback OffsetChanging;

        /// <property name="flag" value="Finished" />
        /// <summary>
        /// Event that is raised when OffsetStep property is changed.
        /// </summary>
        public event PropertyChangedCallback OffsetStepChanged;

        /// <property name="flag" value="Finished" />
        /// <summary>
        /// Event that is raised when user releases mouse button after
        /// moving scroll bar.
        /// </summary>
        public event PropertyChangedCallback OffsetChanged;
        #endregion

        #region Initialization

        /// <summary>
        /// Initializes static members of the <see cref="Splitter"/> class.
        /// </summary>
        static Splitter()
        {
            isPressedPropertyKey = DependencyProperty.RegisterReadOnly("IsPressed", typeof(bool), typeof(Splitter), new FrameworkPropertyMetadata(false, FrameworkPropertyMetadataOptions.None, new PropertyChangedCallback(OnIsPressedChanged)));
            IsPressedProperty = isPressedPropertyKey.DependencyProperty;

            DefaultStyleKeyProperty.OverrideMetadata(typeof(Splitter), new FrameworkPropertyMetadata(typeof(Splitter)));
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="Splitter"/> class.
        /// </summary>
        public Splitter()
        {
        }
        
        /// <summary>
        /// Initializes a new instance of the <see cref="Splitter"/> class.
        /// </summary>
        /// <param name="orientation">The orientation.</param>
        public Splitter(Orientation orientation)
            :this()
        {
            Orientation = orientation;
        }
        #endregion

        #region Implementation
        /// <property name="flag" value="Finished" />
        /// <summary>
        /// Processes IsPressed dependency property changes.
        /// </summary>
        /// <param name="newValue">New value of the dependency property.</param>
        protected virtual void OnIsPressedChanged(bool newValue)
        {
           
            if (newValue)
            {
                m_adorners = AdornerLayer.GetAdornerLayer(this);
                m_adorner = CreateSplitterAdorner();
                if (m_adorners != null && m_adorner != null)
                   m_adorners.Add(m_adorner);
                Offset = 0;
            }
            else
            {
                if (m_adorners != null && m_adorner != null) 
                    m_adorners.Remove(m_adorner);
                m_adorners = null;
                m_adorner = null;
            }

            if (IsPressedChanged != null)
            {
                IsPressedChanged(this, EventArgs.Empty);
            }
        }

        /// <property name="flag" value="Finished" />
        /// <summary>
        /// Updates cursor depending on the splitter orientation.
        /// </summary>
        protected virtual void OnOrientationChanged()
        {
            switch (Orientation)
            {
                case Orientation.Vertical:
                    VerticalAlignment = VerticalAlignment.Stretch;
                    Cursor = System.Windows.Input.Cursors.SizeWE;
                    break;
                case Orientation.Horizontal:
                    HorizontalAlignment = HorizontalAlignment.Stretch;
                    Cursor = System.Windows.Input.Cursors.SizeNS;
                    break;
            }
        }

        /// <property name="flag" value="Finished" />
        /// <summary>
        /// Raises MaxOffsetRightChanged event.
        /// </summary>
        /// <param name="e">Property changes details, such as old value
        /// and new value.</param>
        protected virtual void OnMaxOffsetRightChanged(DependencyPropertyChangedEventArgs e)
        {
            m_maxOffsetRight = (double)e.NewValue;

            if (MaxOffsetRightChanged != null)
            {
                MaxOffsetRightChanged(this, e);
            }
        }

        /// <property name="flag" value="Finished" />
        /// <summary>
        /// Raises MaxOffsetLeftChanged event.
        /// </summary>
        /// <param name="e">Property changes details, such as old value
        /// and new value.</param>
        protected virtual void OnMaxOffsetLeftChanged(DependencyPropertyChangedEventArgs e)
        {
            m_maxOffsetLeft = (double)e.NewValue;

            if (MaxOffsetLeftChanged != null)
            {
                MaxOffsetLeftChanged(this, e);
            }
        }

        /// <property name="flag" value="Finished" />
        /// <summary>
        /// Updates property value cache and raises OffsetChanged event.
        /// </summary>
        /// <param name="e">Property changes details, such as old value
        /// and new value.</param>
        protected virtual void OnOffsetChanged(DependencyPropertyChangedEventArgs e)
        {
            double deltaX = 0;
            double deltaY = 0;
            double step = OffsetStep;
            double value = Math.Round((double)e.NewValue / step) * step;

            m_adorner.LeftLimitReached = m_maxOffsetLeft != double.NaN && -m_maxOffsetLeft >= value;
            m_adorner.RightLimitReached = m_maxOffsetRight != double.NaN && m_maxOffsetRight <= value;
            //m_adorner.LeftLimitReached = false;
            //m_adorner.RightLimitReached = false;

            if (m_adorner.LeftLimitReached)
            {
                value = -m_maxOffsetLeft;
            }

            if (m_adorner.RightLimitReached)
            {
                value = m_maxOffsetRight;
            }

            if (Orientation == Orientation.Vertical)
            {
                deltaX = value;
            }
            else
            {
                deltaY = value;
            }

            if (m_adorner.ChangeOffsets(deltaX, deltaY) || m_adorner.LeftLimitReached || m_adorner.RightLimitReached)
            {
                m_adorners.Update();
            }

            if (OffsetChanging != null)
            {
                OffsetChanging(this, e);
            }
        }

        /// <property name="flag" value="Finished" />
        /// <summary>
        /// Raises OffsetStepChanged event.
        /// </summary>
        /// <param name="e">Property changes details, such as old value
        /// and new value.</param>
        protected virtual void OnOffsetStepChanged(DependencyPropertyChangedEventArgs e)
        {
            m_offsetStep = (double)e.NewValue;

            if (OffsetStepChanged != null)
            {
                OffsetStepChanged(this, e);
            }
        }
        
        /// <summary>
        /// Called to remeasure a control.
        /// </summary>
        /// <param name="constraint">The maximum size that the method can return.</param>
        /// <returns>
        /// The size of the control, up to the maximum specified by <paramref name="constraint"/>.
        /// </returns>
        protected override Size MeasureOverride(Size constraint)
        {
            Size size = base.MeasureOverride(constraint);

            if (Orientation == Orientation.Horizontal)
            {
                size.Height = MinHeight;
            }
            else
            {
                size.Width = MinWidth;
            }

            return size;
        }

        /// <summary>
        /// Invoked whenever the effective value of any dependency property on this <see cref="T:System.Windows.FrameworkElement"/> has been updated. The specific dependency property that changed is reported in the arguments parameter. Overrides <see cref="M:System.Windows.DependencyObject.OnPropertyChanged(System.Windows.DependencyPropertyChangedEventArgs)"/>.
        /// </summary>
        /// <param name="e">The event data that describes the property that changed, as well as old and new values.</param>
        protected override void OnPropertyChanged(DependencyPropertyChangedEventArgs e)
        {
            base.OnPropertyChanged(e);
            if (e.Property == SkinStorage.VisualStyleProperty)
            {
                if (this.DesiredSize.Height > 0 && this.DesiredSize.Width > 0)
                {
                   // this.ArrangeOverride(this.DesiredSize);
                }
                else
                {
                    this.Measure(new Size(double.PositiveInfinity, double.PositiveInfinity));
                    this.ArrangeOverride(this.DesiredSize);
                }
            }
        }

        /// <property name="flag" value="Finished" />
        /// <summary>
        /// Processes left mouse button press.
        /// </summary>
        /// <param name="e">The instance containing the event data.</param>
        protected override void OnMouseLeftButtonDown(MouseButtonEventArgs e)
        {
            base.OnMouseLeftButtonDown(e);
            CaptureMouse();

            m_pointClick = e.GetPosition(this);
            SetValue(isPressedPropertyKey, true);
        }

        /// <property name="flag" value="Finished" />
        /// <summary>
        /// Processes Escape key.
        /// </summary>
        /// <param name="e">The instance containing the event data.</param>
        protected override void OnKeyDown(KeyEventArgs e)
        {
            if (e.Key == Key.Escape)
            {
                Offset = 0;
                SetValue(isPressedPropertyKey, false);
            }

            base.OnKeyDown(e);
        }

        /// <property name="flag" value="Finished" />
        /// <summary>
        /// Processes left mouse button release.
        /// </summary>
        /// <param name="e">The instance containing the event data.</param>
        protected override void OnMouseLeftButtonUp(MouseButtonEventArgs e)
        {
            ReleaseMouseCapture();
            base.OnMouseLeftButtonUp(e);

            if (IsPressed)
            {
                SetValue(isPressedPropertyKey, false);

                if (OffsetChanged != null)
                {
                    OffsetChanged(this, new DependencyPropertyChangedEventArgs(OffsetProperty, 0, Offset));
                }
            }
        }

        /// <property name="flag" value="Finished" />
        /// <summary>
        /// Processes mouse move.
        /// </summary>
        /// <param name="e">The instance containing the event data.</param>
        protected override void OnMouseMove(MouseEventArgs e)
        {
            base.OnMouseMove(e);
            if (IsPressed)
            {
                Point point = e.GetPosition(this);
                double deltaX = point.X - m_pointClick.X;
                double deltaY = point.Y - m_pointClick.Y;

                if (m_adorner != null)
                {
                    Offset = Orientation == Orientation.Vertical ? deltaX : deltaY;
                }
            }
        }

        protected override void OnStylusSystemGesture(StylusSystemGestureEventArgs e)
        {
            m_SystemGesture = e.SystemGesture;
            base.OnStylusSystemGesture(e);
        }

# if !SyncfusionFramework3_5
        //protected override void OnTouchDown(TouchEventArgs e)
        //{
        //    if (DockingManager != null && DockingManager.IsTouchEnabled && m_TouchDeviceId == e.TouchDevice.Id)
        //    {
        //        base.OnTouchDown(e);
        //        OnTouchLeftFingerDown(e);
        //    }
        //}

        //protected override void OnTouchEnter(TouchEventArgs e)
        //{
        //    base.OnTouchEnter(e);
        //    m_TouchDeviceId = (m_TouchDeviceId == -1) ? e.TouchDevice.Id : m_TouchDeviceId;
        //}

        //protected override void OnTouchLeave(TouchEventArgs e)
        //{
        //    if (DockingManager != null && DockingManager.IsTouchEnabled && m_TouchDeviceId == e.TouchDevice.Id)
        //    {
        //        m_TouchDeviceId = -1;
        //        m_SystemGesture = SystemGesture.None;
        //        base.OnTouchLeave(e);
        //    }
        //}

        //protected override void OnTouchMove(TouchEventArgs e)
        //{
        //    if (DockingManager != null && DockingManager.IsTouchEnabled && m_TouchDeviceId == e.TouchDevice.Id)
        //    {
        //        if (m_SystemGesture == SystemGesture.Tap)
        //            OnTouchLeftFingerDown(e);
        //        else
        //        {
        //            if (IsPressed)
        //            {
        //                Point point = e.GetTouchPoint(this).Position;
        //                double deltaX = point.X - m_pointClick.X;
        //                double deltaY = point.Y - m_pointClick.Y;

        //                if (m_adorner != null)
        //                {
        //                    Offset = Orientation == Orientation.Vertical ? deltaX : deltaY;
        //                }
        //            }
        //        }
        //    }
        //    base.OnTouchMove(e);
        //}

        //protected override void OnTouchUp(TouchEventArgs e)
        //{
        //    if (DockingManager != null && DockingManager.IsTouchEnabled && m_TouchDeviceId == e.TouchDevice.Id)
        //    {
        //        if (m_SystemGesture == SystemGesture.Tap)
        //            OnTouchLeftFingerUp(e);
        //        base.OnTouchUp(e);
        //    }
        //}

        //private void OnTouchLeftFingerDown(TouchEventArgs e)
        //{
        //    CaptureTouch(e.TouchDevice);

        //    m_pointClick = e.GetTouchPoint(this).Position;
        //    SetValue(isPressedPropertyKey, true);
        //}

        //private void OnTouchLeftFingerUp(TouchEventArgs e)
        //{
        //    ReleaseTouchCapture(e.TouchDevice);

        //    if (IsPressed)
        //    {
        //        SetValue(isPressedPropertyKey, false);

        //        if (OffsetChanged != null)
        //        {
        //            OffsetChanged(this, new DependencyPropertyChangedEventArgs(OffsetProperty, 0, Offset));
        //        }
        //    }
        //}
#endif
        
        /// <summary>
        /// Creates adorner used to display splitter moving.
        /// </summary>
        /// <returns>return splitter adorner.</returns>
        protected virtual SplitterAdorner CreateSplitterAdorner()
        {
            return new SplitterAdorner(this);
        }
        
        /// <summary>
        /// Coerces the offset.
        /// </summary>
        /// <param name="d">The dependency object.</param>
        /// <param name="baseValue">The base value.</param>
        /// <returns>return object.</returns>
        private static object CoerceOffset(DependencyObject d, object baseValue)
        {
            Splitter splitter = (Splitter)d;
            double value = (double)baseValue;
            double left = splitter.MaxOffsetLeft;
            double right = splitter.MaxOffsetRight;

            if (left != double.NaN && value < -left)
            {
                value = -left;
            }

            if (right != double.NaN && value > right)
            {
                value = right;
            }

            return value;
        }

        /// <summary>
        /// Called when [is pressed changed].
        /// </summary>
        /// <param name="d">The dependency object.</param>
        /// <param name="e">The <see cref="System.Windows.DependencyPropertyChangedEventArgs"/> instance containing the event data.</param>
        private static void OnIsPressedChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            Splitter splitter = (Splitter)d;
            splitter.OnIsPressedChanged((bool)e.NewValue);
        }

        /// <property name="flag" value="Finished" />
        /// <summary>
        /// Calls OnMaxOffsetLeftChanged method of the instance, notifies
        /// of the dependency property value changes.
        /// </summary>
        /// <param name="d">Dependency object, the change occurs on.</param>
        /// <param name="e">Property changes details, such as old value
        /// and new value.</param>
        private static void OnMaxOffsetLeftChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            Splitter instance = (Splitter)d;
            instance.OnMaxOffsetLeftChanged(e);
        }

        /// <property name="flag" value="Finished" />
        /// <summary>
        /// Calls OnMaxOffsetRightChanged method of the instance,
        /// notifies of the dependency property value changes.
        /// </summary>
        /// <param name="d">Dependency object, the change occurs on.</param>
        /// <param name="e">Property changes details, such as old value
        /// and new value.</param>
        private static void OnMaxOffsetRightChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            Splitter instance = (Splitter)d;
            instance.OnMaxOffsetRightChanged(e);
        }

        /// <property name="flag" value="Finished" />
        /// <summary>
        /// Calls OnOffsetChanged method of the instance, notifies of the
        /// dependency property value changes.
        /// </summary>
        /// <param name="d">Dependency object, the change occurs on.</param>
        /// <param name="e">Property changes details, such as old value
        /// and new value.</param>
        private static void OnOffsetChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            Splitter instance = (Splitter)d;
            instance.OnOffsetChanged(e);
        }

        /// <property name="flag" value="Finished" />
        /// <summary>
        /// Calls OnOffsetStepChanged method of the instance, notifies of
        /// the dependency property value changes.
        /// </summary>
        /// <param name="d">Dependency object, the change occurs on.</param>
        /// <param name="e">Property changes details, such as old value
        /// and new value.</param>
        private static void OnOffsetStepChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            Splitter instance = (Splitter)d;
            instance.OnOffsetStepChanged(e);
        }

        /// <property name="flag" value="Finished" />
        /// <summary>
        /// Calls OnOrientationChanged method of the instance, notifies
        /// of the dependency property value changes.
        /// </summary>
        /// <param name="d">Dependency object, the change occurs on.</param>
        /// <param name="e">Property changes details, such as old value
        /// and new value.</param>
        private static void OnOrientationChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            Splitter instance = (Splitter)d;
            instance.OnOrientationChanged();
        }
        #endregion

        #region Dependency Properties
        /// <property name="flag" value="Finished" />
        /// <summary>
        /// Read only dependency property that indicates whether splitter
        /// is pressed.
        /// </summary>
        public static DependencyProperty IsPressedProperty;

        /// <property name="flag" value="Finished" />
        /// <summary>
        /// Dependency property key used to manipulate IsPressed
        /// dependency property's value.
        /// </summary>
        private static DependencyPropertyKey isPressedPropertyKey;

        /// <property name="flag" value="Finished" />
        /// <summary>
        /// Dependency property used to specify adorner's template.
        /// </summary>
        public static readonly DependencyProperty AdornerTemplateProperty =
            DependencyProperty.Register("AdornerTemplate", typeof(ControlTemplate), typeof(Splitter), new FrameworkPropertyMetadata(null, FrameworkPropertyMetadataOptions.AffectsParentArrange | FrameworkPropertyMetadataOptions.AffectsRender | FrameworkPropertyMetadataOptions.AffectsArrange | FrameworkPropertyMetadataOptions.AffectsMeasure));

        /// <property name="flag" value="Finished" />
        /// <summary>
        /// Dependency property used to specify splitter's orientation.
        /// </summary>
        public static readonly DependencyProperty OrientationProperty =
            DependencyProperty.Register("Orientation", typeof(Orientation), typeof(Splitter), new FrameworkPropertyMetadata(Orientation.Horizontal, OnOrientationChanged));

        /// <property name="flag" value="Finished" />
        /// <summary>
        /// Dependency property that determines maximum splitter offset
        /// to the left/top.
        /// </summary>
        public static readonly DependencyProperty MaxOffsetLeftProperty =
            DependencyProperty.Register("MaxOffsetLeft", typeof(double), typeof(Splitter), new FrameworkPropertyMetadata(double.NaN, OnMaxOffsetLeftChanged));

        /// <property name="flag" value="Finished" />
        /// <summary>
        /// Dependency property that determines maximum splitter offset
        /// to the right/bottom.
        /// </summary>
        public static readonly DependencyProperty MaxOffsetRightProperty =
            DependencyProperty.Register("MaxOffsetRight", typeof(double), typeof(Splitter), new FrameworkPropertyMetadata(double.NaN, OnMaxOffsetRightChanged));

        /// <summary>
        /// Identifies Splitter.Offset dependency property key.
        /// </summary>
        protected static readonly DependencyPropertyKey OffsetPropertyKey =
            DependencyProperty.RegisterReadOnly("Offset", typeof(double), typeof(Splitter), new FrameworkPropertyMetadata(0.0, OnOffsetChanged, CoerceOffset));

        /// <property name="flag" value="Finished" />
        /// <summary>
        /// Offset Dependency Property.
        /// </summary>
        public static readonly DependencyProperty OffsetProperty = OffsetPropertyKey.DependencyProperty;

        /// <property name="flag" value="Finished" />
        /// <summary>
        /// OffsetStep Dependency Property.
        /// </summary>
        public static readonly DependencyProperty OffsetStepProperty =
            DependencyProperty.Register("OffsetStep", typeof(double), typeof(Splitter), new FrameworkPropertyMetadata(1.0, OnOffsetStepChanged));



        public bool UseNativeFloatWindow
        {
            get { return (bool)GetValue(UseNativeFloatWindowProperty); }
           internal set { SetValue(UseNativeFloatWindowProperty, value); }
        }

        // Using a DependencyProperty as the backing store for UseNativeFloatWindow.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty UseNativeFloatWindowProperty =
            DependencyProperty.Register("UseNativeFloatWindow", typeof(bool), typeof(Splitter), new UIPropertyMetadata(false));



        public DockingManager DockingManager
        {
            get { return (DockingManager)GetValue(DockingManagerProperty); }
         internal  set { SetValue(DockingManagerProperty, value); }
        }

        // Using a DependencyProperty as the backing store for DockingManager.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty DockingManagerProperty =
            DependencyProperty.Register("DockingManager", typeof(DockingManager), typeof(Splitter), new UIPropertyMetadata(null));

        #endregion
    }
}
