// <copyright file="LinearPointer.cs" company="Syncfusion Software">
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
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Input;

namespace Syncfusion.Windows.Gauge
{
    /// <summary>
    /// Represents the abstract base class for <see cref="LinearBarPointer"/> and <see cref="LinearMarkerPointer"/>
    /// </summary>
    /// <seealso cref="LinearBarPointer"/>
    /// <seealso cref="LinearMarkerPointer"/>
    /// <example>
    /// <code lang="XAML">
    /// <Window x:Class="LinearPointerSample.Window1" Title="LinearPointerSample" Height="400"
    /// Width="400" xmlns="http://schemas.microsoft.com/winfx/2006/xaml/presentation"
    /// xmlns:x="http://schemas.microsoft.com/winfx/2006/xaml"
    /// xmlns:syncfusion="http://schemas.syncfusion.com/wpf">
    ///     <Grid>
    ///         <syncfusion:LinearGauge CenterFrameFillColor="Brown" Name="linearGauge1">
    ///             <syncfusion:LinearGauge.Scales>
    ///                 <syncfusion:LinearScale Name="LinearScale" Minimum="0" Maximum="100" 
    ///                                         MinorIntervalValue="2" MajorIntervalValue="10" 
    ///                                         ScaleBarSize="20" ScaleBarLength="260">
    ///                     <syncfusion:LinearScale.Pointers>
    ///                         <syncfusion:LinearBarPointer Name="barpointer" BackgroundBrush="Red" 
    ///                                                      BorderBrush="Black" PointerWidth="5" Value="45" />
    ///                     </syncfusion:LinearScale.Pointers>
    ///                 </syncfusion:LinearScale>
    ///             </syncfusion:LinearGauge.Scales>
    ///         </syncfusion:LinearGauge>
    ///     </Grid>
    /// </Window>
    /// </code>
    /// <code lang="C#">
    /// using System;
    /// using System.Collections.Generic;
    /// using System.Linq;
    /// using System.Text;
    /// using System.Windows;
    /// using System.Windows.Media;
    /// using System.Windows.Controls;
    /// using System.Windows.Data;
    /// using Syncfusion.Windows.Shared;
    /// using Syncfusion.Windows.Gauge;<para/>
    /// namespace LinearPointerSample<para/>
    /// {
    ///         public partial class Window1 : Window
    ///         {
    ///             private LinearGauge linearGauge1;<para/>
    ///             public Window1()
    ///             {                
    ///                 InitializeComponent();<para/>
    ///                 linearGauge1 = new LinearGauge();
    ///                 this.linearGauge1.CenterFrameFillColor = Colors.Brown;<para/>
    ///                 LinearScale scale = new LinearScale();
    ///                 scale.Minimum = 0;
    ///                 scale.Maximum = 100;
    ///                 scale.MinorIntervalValue = 2;
    ///                 scale.MajorIntervalValue = 10;
    ///                 scale.ScaleBarSize = 20;
    ///                 scale.ScaleBarLength = 260;
    ///                 linearGauge1.Scales.Add(scale);<para/>
    ///                 LinearBarPointer pointer = new LinearBarPointer();
    ///                 pointer.BackgroundBrush = Brushes.Red;
    ///                 pointer.BorderBrush = new SolidColorBrush(Colors.Red);
    ///                 pointer.PointerWidth = 8;
    ///                 pointer.Value = 45;
    ///                 scale.Pointers.Add(pointer);<para/>
    ///                 this.Content = linearGauge1;
    ///             }
    ///         }
    /// }
    /// </code>
    /// </example>
#if SyncfusionFramework4_0
    [System.ComponentModel.DesignTimeVisible(false)]
#endif
    public abstract class LinearPointer : GaugeElement
    {
        #region Private Members
        /// <summary>
        /// Event arguments used to fire the ValueChanged event.
        /// </summary>
        private DependencyPropertyChangedEventArgs m_valueArgs;

        /// <summary>
        /// The ratio between the scale size and gauge size.
        /// </summary>
        private double m_sizeRatio;


        #endregion Private Members

        #region Events
        /// <summary>
        /// Event that is raised when <see cref="PointerWidth"/> property is changed.
        /// </summary>
        public event PropertyChangedCallback PointerWidthChanged;

        /// <summary>
        /// Event that is raised when <see cref="Position"/> property is changed.
        /// </summary>
        public event PropertyChangedCallback PositionChanged;

        /// <summary>
        /// Event that is raised when <see cref="Value"/> property is changed.
        /// </summary>
        public event PropertyChangedCallback ValueChanged;

        /// <summary>
        /// Event that is raised when <see cref="AnimationDuration"/> property is changed.
        /// </summary>
        public event PropertyChangedCallback AnimationDurationChanged;

        /// <summary>
        /// Event that is raised when <see cref="EnablePointerInteraction"/> property is changed.
        /// </summary>
        public event PropertyChangedCallback EnablePointerInteractionChanged;

        /// <summary>
        /// Event that is raised when <see cref="IncrementKey"/> property is changed.
        /// </summary>
        public event PropertyChangedCallback IncrementKeyChanged;

        /// <summary>
        /// Event that is raised when <see cref="DecrementKey"/> property is changed.
        /// </summary>
        public event PropertyChangedCallback DecrementKeyChanged;
        #endregion Events

        #region Dependency Properties
        /// <summary>
        /// Identifies the <see cref="PointerWidth"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty PointerWidthProperty =
            DependencyProperty.Register("PointerWidth", typeof(double), typeof(LinearPointer), new FrameworkPropertyMetadata(0d, FrameworkPropertyMetadataOptions.AffectsArrange | FrameworkPropertyMetadataOptions.AffectsRender | FrameworkPropertyMetadataOptions.AffectsMeasure, new PropertyChangedCallback(OnPointerWidthChanged)));

        /// <summary>
        /// Identifies the <see cref="Position"/> dependency property.<para/>
        /// It stores the position of the pointer relative to ScaleBarlength.
        /// </summary>
        internal static readonly DependencyProperty PositionProperty =
            DependencyProperty.Register("Position", typeof(double), typeof(LinearPointer), new FrameworkPropertyMetadata(0d, FrameworkPropertyMetadataOptions.AffectsRender, new PropertyChangedCallback(OnPositionChanged), CoercePosition));

        /// <summary>
        /// Identifies the <see cref="Value"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty ValueProperty =
            DependencyProperty.Register("Value", typeof(double), typeof(LinearPointer), new FrameworkPropertyMetadata(0d, FrameworkPropertyMetadataOptions.AffectsRender, new PropertyChangedCallback(OnValueChanged), CoerceValue));

        /// <summary>
        /// Identifies the <see cref="AnimationDuration"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty AnimationDurationProperty =
            DependencyProperty.Register("AnimationDuration", typeof(double), typeof(LinearPointer), new FrameworkPropertyMetadata(Double.NaN, FrameworkPropertyMetadataOptions.AffectsRender, new PropertyChangedCallback(OnAnimationDurationChanged)));

        /// <summary>
        /// Identifies the <see cref="EnablePointerInteraction"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty EnablePointerInteractionProperty =
            DependencyProperty.Register("EnablePointerInteraction", typeof(bool), typeof(LinearPointer), new FrameworkPropertyMetadata(true, FrameworkPropertyMetadataOptions.AffectsRender, new PropertyChangedCallback(OnEnablePointerInteractionChanged)));

        /// <summary>
        /// Identifies the <see cref="IncrementKey"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty IncrementKeyProperty =
            DependencyProperty.Register("IncrementKey", typeof(Key), typeof(LinearPointer), new FrameworkPropertyMetadata(Key.Up, FrameworkPropertyMetadataOptions.AffectsRender, new PropertyChangedCallback(OnIncrementKeyChanged)));

        /// <summary>
        /// Identifies the <see cref="DecrementKey"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty DecrementKeyProperty =
            DependencyProperty.Register("DecrementKey", typeof(Key), typeof(LinearPointer), new FrameworkPropertyMetadata(Key.Down, FrameworkPropertyMetadataOptions.AffectsRender, new PropertyChangedCallback(OnDecrementKeyChanged)));

        #endregion Dependency Properties

        #region DP Getters & Setters
        /// <summary>
        /// Gets or sets the width of the pointer.
        /// This is a dependency property.
        /// </summary>
        /// <value>
        /// Type: <see cref="double"/>
        /// Default value is 0.
        /// </value>
        /// <seealso cref="LinearMarkerPointer.PointerLength"/>
        public double PointerWidth
        {
            get
            {
                return (double)GetValue(PointerWidthProperty);
            }

            set
            {
                SetValue(PointerWidthProperty, value);
            }
        }

        /// <summary>
        /// Gets or sets the pointer's position relative to the ScaleBarLength.<para/>
        /// This is an "internal" dependency property.
        /// </summary>
        /// <value>
        /// Type: <see cref="double"/>
        /// Default value is 0.
        /// </value>
        internal double Position
        {
            get
            {
                return (double)GetValue(PositionProperty);
            }

            set
            {
                SetValue(PositionProperty, value);
            }
        }

        /// <summary>
        /// Gets or sets the value to be indicated by the pointer.<para/>
        /// This is a dependency property.
        /// </summary>
        /// <value>
        /// Type: <see cref="double"/>
        /// Default value is 0.
        /// </value>
        public double Value
        {
            get
            {
                return (double)GetValue(ValueProperty);
            }

            set
            {
                LinearScale scale = this.VisualParent as LinearScale;
                if (scale != null)
                {
                    if (value >= scale.Minimum && value <= scale.Maximum)
                    {

                        SetValue(ValueProperty, value);
                    }
                    else
                    {
                        if (value < scale.Minimum)
                        {
                            SetValue(ValueProperty, scale.Minimum);
                        }
                        else if (value > scale.Maximum)
                        {
                            SetValue(ValueProperty, scale.Maximum);
                        }
                    }
                }
                else
                {
                    SetValue(ValueProperty, value);
                }
            }
        }

        /// <summary>
        /// Gets or sets the animation duration of the pointer.
        /// This is a dependency property.
        /// </summary>
        /// <value>
        /// Type: <see cref="double"/>
        /// Default value is Double.NaN.
        /// </value>
        public double AnimationDuration
        {
            get
            {
                return (double)GetValue(AnimationDurationProperty);
            }

            set
            {
                SetValue(AnimationDurationProperty, value);
            }
        }

        /// <summary>
        /// Gets or sets whether the pointer interactivity of the pointer is enabled or not.
        /// This is a dependency property.
        /// </summary>
        /// <value>
        /// Type: <see cref="Boolean"/>
        /// Default value is true.
        /// </value>
        public bool EnablePointerInteraction
        {
            get
            {
                return (bool)GetValue(EnablePointerInteractionProperty);
            }

            set
            {
                SetValue(EnablePointerInteractionProperty, value);
            }
        }

        /// <summary>
        /// Gets or sets the Increment key for the pointer.
        /// This is a dependency property.
        /// </summary>
        /// <value>
        /// Type: <see cref="Key"/>
        /// Default value is Up key.
        /// </value>
        public Key IncrementKey
        {
            get
            {
                return (Key)GetValue(IncrementKeyProperty);
            }

            set
            {
                SetValue(IncrementKeyProperty, value);
            }
        }


        /// <summary>
        /// Gets or sets the Decrement key for the pointer.
        /// This is a dependency property.
        /// </summary>
        /// <value>
        /// Type: <see cref="Key"/>
        /// Default value is Down key.
        /// </value>
        public Key DecrementKey
        {
            get
            {
                return (Key)GetValue(DecrementKeyProperty);
            }

            set
            {
                SetValue(DecrementKeyProperty, value);
            }
        }
       
        /// <summary>
        /// Gets or sets the ratio of ScaleBarSize to that of LinearGauge's Width.
        /// </summary>
        internal double SizeRatio
        {
            get
            {
                return m_sizeRatio;
            }

            set
            {
                m_sizeRatio = value;
            }
        }
        #endregion DP Getters & Setters

        #region Initialization
        /// <summary>
        /// Initializes static members of the <see cref="LinearPointer"/> class.
        /// Overrides the meta data for default template.
        /// </summary>
        static LinearPointer()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(LinearPointer), new FrameworkPropertyMetadata(typeof(LinearPointer)));
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="LinearPointer"/> class.
        /// </summary>
        public LinearPointer()
        {

        }

        #endregion Initialization

        #region Implementation
        /// <summary>
        /// Fulfils the logic before setting the value of <see cref="Position"/> dependency property.
        /// </summary>
        /// <param name="value">The value that should be corrected.</param>
        /// <returns>The corrected value.</returns>
        protected internal virtual object CoercePosition(object value)
        {
            double val = (double)value;
            if (val < 0)
            {
                val = 0;
            }

            return val;
        }

        /// <summary>
        /// Coerces the value of the <see cref="Position"/> property.
        /// </summary>
        /// <param name="d">The <see cref="LinearPointer"/> object.</param>
        /// <param name="value">The instance containing the event data.</param>
        /// <returns>
        /// The checked value.
        /// </returns>
        public static object CoercePosition(DependencyObject d, object value)
        {
            LinearPointer owner = d as LinearPointer;
            return owner.CoercePosition(value);
        }

        /// <summary>
        /// Coerces the value of the <see cref="Value"/> property.
        /// </summary>
        /// <param name="d">The <see cref="LinearPointer"/> object.</param>
        /// <param name="value">The instance containing the event data.</param>
        /// <returns>
        /// The checked value.
        /// </returns>
        public static object CoerceValue(DependencyObject d, object value)
        {
            LinearPointer owner = d as LinearPointer;
            return owner.CoerceValue(value);
        }

        /// <summary>
        /// Fulfils the logic before setting the value of <see cref="Value"/> dependency property.
        /// </summary>
        /// <param name="value">The value that should be corrected.</param>
        /// <returns>The corrected value.</returns>
        protected internal virtual object CoerceValue(object value)
        {
            double val = (double)value;
            if (this.VisualParent is LinearScale)
            {
                LinearScale scale = this.VisualParent as LinearScale;
                if (val < scale.Minimum)
                {
                    val = scale.Minimum;
                }

                if (val > scale.Maximum)
                {
                    val = scale.Maximum;
                }
            }

            return val;
        }

        /// <summary>
        /// Updates property value cache and raises <see cref="PointerWidthChanged"/> event.
        /// </summary>
        /// <param name="e">
        /// Property change details, such as old value and new value.</param>
        protected virtual void OnPointerWidthChanged(DependencyPropertyChangedEventArgs e)
        {
            if (PointerWidthChanged != null)
            {
                this.PointerWidthChanged(this, e);
            }
        }

        /// <summary>
        /// Calls OnPointerWidthChanged method of the instance, notifies of the depencency property value changes.
        /// </summary>
        /// <param name="d">Dependency object, the change occures on.</param>
        /// <param name="e">Property change details, such as old value and new value.</param>
        private static void OnPointerWidthChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            LinearPointer instance = (LinearPointer)d;
            instance.OnPointerWidthChanged(e);
        }

        /// <summary>
        /// Updates property value cache and raises <see cref="PositionChanged"/> event.
        /// </summary>
        /// <param name="e">
        /// Property change details, such as old value and new value.</param>
        protected virtual void OnPositionChanged(DependencyPropertyChangedEventArgs e)
        {
            if (this.IsLoaded)
            {
                this.RefreshPointerPosition();
            }

            if (PositionChanged != null)
            {
                this.PositionChanged(this, e);
            }
        }

        /// <summary>
        /// Calls OnPositionChanged method of the instance, notifies of the depencency property value changes.
        /// </summary>
        /// <param name="d">Dependency object, the change occures on.</param>
        /// <param name="e">Property change details, such as old value and new value.</param>
        private static void OnPositionChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            LinearPointer instance = (LinearPointer)d;
            instance.OnPositionChanged(e);
        }

        /// <summary>
        /// Updates property value cache and raises <see cref="EnablePointerInteractionChanged"/> event.
        /// </summary>
        /// <param name="e">
        /// Property change details, such as old value and new value.</param>
        protected virtual void OnEnablePointerInteractionChanged(DependencyPropertyChangedEventArgs e)
        {
            if (EnablePointerInteractionChanged != null)
            {
                EnablePointerInteractionChanged(this, e);
            }
        }

        /// <summary>
        /// Updates property value cache and raises <see cref="IncrementKeyChanged"/> event.
        /// </summary>
        /// <param name="e">
        /// Property change details, such as old value and new value.</param>
        protected virtual void OnIncrementKeyChanged(DependencyPropertyChangedEventArgs e)
        {
            if (IncrementKeyChanged != null)
            {
                IncrementKeyChanged(this, e);
            }
        }

        /// <summary>
        /// Updates property value cache and raises <see cref="DecrementKeyChanged"/> event.
        /// </summary>
        /// <param name="e">
        /// Property change details, such as old value and new value.</param>
        protected virtual void OnDecrementKeyChanged(DependencyPropertyChangedEventArgs e)
        {
            if (DecrementKeyChanged != null)
            {
                DecrementKeyChanged(this, e);
            }
        }


        /// <summary>
        /// Updates property value cache and raises <see cref="ValueChanged"/> event.<para/>
        /// Animation to the pointer is done here.
        /// </summary>
        /// <param name="e">
        /// Property change details, such as old value and new value.</param>
        protected virtual void OnValueChanged(DependencyPropertyChangedEventArgs e)
        {
            if (this.IsLoaded)
            {
                double duration;
                LinearScale scale = this.VisualParent as LinearScale;
                double oldPosition = scale.GetPositionByPointerValue((double)e.OldValue);
                double newPosition = scale.GetPositionByPointerValue((double)e.NewValue);
                DoubleAnimation positionAnimation = new DoubleAnimation();
                positionAnimation.FillBehavior = FillBehavior.Stop;
                positionAnimation.From = oldPosition;
                positionAnimation.To = newPosition;
                if (Double.IsNaN(this.AnimationDuration) == true)
                {
                    duration = Math.Abs(oldPosition - newPosition) * 3;
                    if (duration < 300)
                    {
                        duration = 300;
                    }
                }
                else
                {
                    duration = this.AnimationDuration;
                }

                positionAnimation.Duration = new Duration(TimeSpan.FromMilliseconds(duration));
                positionAnimation.AccelerationRatio = 0.3;
                positionAnimation.DecelerationRatio = 0.3;
                positionAnimation.Completed += new EventHandler(PositionAnimationCompleted);
                this.BeginAnimation(PositionProperty, positionAnimation);
                this.Position = newPosition;
                m_valueArgs = e;
            }
            else
            {
                if (ValueChanged != null)
                {
                    this.ValueChanged(this, e);
                }
            }

            this.RefreshPointerPosition();
        }

        /// <summary>
        /// Calls OnValueChanged method of the instance, notifies of the depencency property value changes.
        /// </summary>
        /// <param name="d">Dependency object, the change occures on.</param>
        /// <param name="e">Property change details, such as old value and new value.</param>
        private static void OnValueChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            LinearPointer instance = (LinearPointer)d;
            instance.OnValueChanged(e);
        }

        /// <summary>
        /// Calls OnEnablePointerInteractionChanged method of the instance, notifies of the depencency property value changes.
        /// </summary>
        /// <param name="d">Dependency object, the change occures on.</param>
        /// <param name="e">Property change details, such as old value and new value.</param>
        private static void OnEnablePointerInteractionChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            LinearPointer instance = (LinearPointer)d;
            instance.OnEnablePointerInteractionChanged(e);
            if (d.GetType() == typeof(LinearBarPointer))
            {
                LinearBarPointer instance1 = (LinearBarPointer)d;
                if (instance.IsLoaded)
                {
                    if (instance.EnablePointerInteraction == true)
                    {
                        instance1.EnableMouseEvents(instance1);
                    }
                    else
                    {
                        instance1.DisableMouseEvents(instance1);
                    }
                }
            }
            else
            {
                LinearMarkerPointer instance2 = (LinearMarkerPointer)d;

                if (instance.IsLoaded)
                {
                    if (instance.EnablePointerInteraction == true)
                    {
                        instance2.EnableMouseEvents(instance2);
                    }
                    else
                    {
                        instance2.DisableMouseEvents(instance2);
                    }
                }
            }
        }

        /// <summary>
        /// Calls OnIncrementKeyChanged method of the instance, notifies of the depencency property value changes.
        /// </summary>
        /// <param name="d">Dependency object, the change occures on.</param>
        /// <param name="e">Property change details, such as old value and new value.</param>
        private static void OnIncrementKeyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            LinearPointer instance = (LinearPointer)d;
            instance.OnIncrementKeyChanged(e);
        }


        /// <summary>
        /// Calls OnDecrementKeyChanged method of the instance, notifies of the depencency property value changes.
        /// </summary>
        /// <param name="d">Dependency object, the change occures on.</param>
        /// <param name="e">Property change details, such as old value and new value.</param>
        private static void OnDecrementKeyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            LinearPointer instance = (LinearPointer)d;
            instance.OnDecrementKeyChanged(e);
        }

        /// <summary>
        /// Raises the <see cref="ValueChanged"/> event.
        /// </summary>
        /// <param name="sender">The source of this event.</param>
        /// <param name="e">The instance containing the event data.</param>
        private void PositionAnimationCompleted(object sender, EventArgs e)
        {
            if (ValueChanged != null)
            {
                this.ValueChanged(this, m_valueArgs);
            }
        }

        /// <summary>
        /// Updates the position of the pointer.
        /// </summary>
        /// <remarks>Checks whether the pointer is the visual child of LinearScale and 
        /// transforms the pointer to the appropriate location from the center of the gauge.</remarks>
        protected internal virtual void RefreshPointerPosition()
        {
            LinearScale scale = this.VisualParent as LinearScale;
            if (scale != null)
            {
                if (scale.Orientation == GaugeOrientation.Vertical)
                {
                    this.RenderTransform = new TranslateTransform(0, (scale.ScaleBarLength / 2) - this.Position - (this.PointerWidth / 2));
                }
                else
                {
                    TransformGroup transform = new TransformGroup();
                    RotateTransform rotate = new RotateTransform(-90d, scale.ScaleBarSize / 2, scale.ScaleBarLength / 2);
                    TranslateTransform translate = new TranslateTransform(0, (scale.ScaleBarLength / 2) - this.Position - (this.PointerWidth / 2));
                    transform.Children.Add(rotate);
                    transform.Children.Add(translate);
                    this.RenderTransform = transform;
                }
            }
        }

        /// <summary>
        /// Calls OnPointerWidthChanged method of the instance, notifies of the depencency property value change.
        /// </summary>
        /// <param name="d">Dependency object, the change occures on.</param>
        /// <param name="e">Property change details, such as old value and new value.</param>
        private static void OnAnimationDurationChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            LinearPointer instance = (LinearPointer)d;
            instance.OnAnimationDurationChanged(e);
        }

        /// <summary>
        /// Updates property value cache and raises <see cref="AnimationDurationChanged"/> event.
        /// </summary>
        /// <param name="e">
        /// Property change details, such as old value and new value.</param>
        protected virtual void OnAnimationDurationChanged(DependencyPropertyChangedEventArgs e)
        {
            if (AnimationDurationChanged != null)
            {
                AnimationDurationChanged(this, e);
            }
        }
        #endregion Implementation
    }
}
