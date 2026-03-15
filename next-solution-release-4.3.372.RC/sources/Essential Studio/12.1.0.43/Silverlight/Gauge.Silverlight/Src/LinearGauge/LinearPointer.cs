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
    using System.Windows.Media.Animation;
    using System.Windows.Shapes;
    using System.Windows.Input;
    using System.Windows.Controls;


    /// <summary>
    /// Represents the linear pointer visual element.
    /// </summary>
    public abstract class LinearPointer : GaugeElement
    {
        #region Dependency properties

        /// <summary>
        /// Identifies the <see cref="P:Syncfusion.Windows.Gauge.LinearPointer.PointerWidth">PointerWidth</see> dependency property.
        /// </summary>
        /// <returns>
        /// Type :  <see cref="T:System.Double">System.Double</see>
        /// </returns>
        public static readonly DependencyProperty PointerWidthProperty =
            DependencyProperty.Register("PointerWidth", typeof(double), typeof(LinearPointer), new PropertyMetadata(0d, new PropertyChangedCallback(OnPointerWidthChanged)));

        /// <summary>
        /// Identifies the <see cref="P:Syncfusion.Windows.Gauge.LinearPointer.Value">Value</see> dependency property.
        /// </summary>
        /// <returns>
        /// Type : <see cref="T:System.Double">System.Double</see>
        /// </returns>
        public static readonly DependencyProperty ValueProperty =
            DependencyProperty.Register("Value", typeof(double), typeof(LinearPointer), new PropertyMetadata(0d, new PropertyChangedCallback(OnValueChanged)));

        /// <summary>
        /// Identifies the <see cref="P:Syncfusion.Windows.Gauge.LinearPointer.Position">Position</see> dependency property.
        /// </summary>
        /// <returns>
        /// Type : <see cref="T:System.Double">System.Double</see>
        /// </returns>
        internal static readonly DependencyProperty PositionProperty =
            DependencyProperty.Register("Position", typeof(double), typeof(LinearPointer), new PropertyMetadata(0d, new PropertyChangedCallback(OnPositionChanged)));

        /// <summary>
        /// Identifies the <see cref="P:Syncfusion.Windows.Gauge.CircularKnob.EnablePointerInteraction">EnablePointerInteraction</see> Dependency property.
        /// </summary>
        /// <returns>
        /// Type:<see cref="T:System.Boolean">System.Boolean</see>
        /// </returns>
        public static readonly DependencyProperty EnablePointerInteractionProperty =
            DependencyProperty.Register("EnablePointerInteraction", typeof(bool), typeof(LinearPointer), new PropertyMetadata(true, new PropertyChangedCallback(OnEnablePointerInteractionChanged)));

        /// <summary>
        /// Identifies the <see cref="P:Syncfusion.Windows.Gauge.CircularPointer.PointerSelectionBrush">HilightSelection</see> Dependency property.
        /// </summary>
        /// <returns>
        /// Type:<see cref="T:Brush">Brush</see>
        /// </returns>
        public static readonly DependencyProperty PointerSelectionBrushProperty =
           DependencyProperty.Register("PointerSelectionBrush", typeof(Brush), typeof(LinearPointer), new PropertyMetadata(new SolidColorBrush(Colors.Black), new PropertyChangedCallback(OnPointerSelectionBrushChanged)));

        /// <summary>
        /// Identifies the <see cref="P:Syncfusion.Windows.Gauge.CircularPointer.IncrementKey">IncrementKey</see> Dependency property.
        /// </summary>
        /// <returns>
        /// Type:<see cref="T:Key">Key</see>
        /// </returns>
        public static readonly DependencyProperty IncrementKeyProperty =
           DependencyProperty.Register("IncrementKey", typeof(Key), typeof(LinearPointer), new PropertyMetadata(Key.Up, new PropertyChangedCallback(OnIncrementKeyChanged)));

        /// <summary>
        /// Identifies the <see cref="P:Syncfusion.Windows.Gauge.CircularPointer.DecrementKey">DecrementKey</see> Dependency property.
        /// </summary>
        /// <returns>
        /// Type:<see cref="T:Key">Key</see>
        /// </returns>
        public static readonly DependencyProperty DecrementKeyProperty =
            DependencyProperty.Register("DecrementKey", typeof(Key), typeof(LinearPointer), new PropertyMetadata(Key.Down, new PropertyChangedCallback(OnDecrementKeyChanged)));

        #endregion

        #region Private members

        /// <summary>
        /// is loaded or not
        /// </summary>
        private bool misLoaded = false;

        private double startvalue = 0;

        /// <summary>
        /// Event arguments used to fire the ValueChanged event.
        /// </summary>
        private DependencyPropertyChangedEventArgs mvalueArgs;

        #endregion

        #region Initialization

        /// <summary>
        /// Initializes a new instance of the <see cref="T:Syncfusion.Windows.Gauge.LinearPointer">LinearPointer</see> class.
        /// </summary>
        public LinearPointer()
        {
            this.Loaded += new RoutedEventHandler(this.LinearPointerLoaded);
        }
        
        #endregion

        #region Events

        /// <summary>
        /// Event that is raised when <see cref="P:Syncfusion.Windows.Gauge.LinearPointer.PointerWidth">PointerWidth</see> property is changed.
        /// </summary>
        public event PropertyChangedCallback PointerWidthChanged;

        /// <summary>
        /// Event that is raised when <see cref="P:Syncfusion.Windows.Gauge.LinearPointer.Position">Position</see> property is changed.
        /// </summary>
        public event PropertyChangedCallback PositionChanged;

        /// <summary>
        /// Event that is raised when <see cref="P:Syncfusion.Windows.Gauge.LinearPointer.Value">Value</see> property is changed.
        /// </summary>
        public event PropertyChangedCallback ValueChanged;

        /// <summary>
        /// Event that is raised when <see cref="P:Syncfusion.Windows.Gauge.CircularKnob.EnablePointerInteraction">EnablePointerInteraction</see> property is changed.
        /// </summary>
        public event PropertyChangedCallback EnablePointerInteractionChanged;

        /// <summary>
        /// Event that is raised when <see cref="P:Syncfusion.Windows.Gauge.CircularPointer.HighlightPinterSelection">PointerSelectionBrush</see> property is changed.
        /// </summary>
        public event PropertyChangedCallback PointerSelectionBrushChanged;

        /// <summary>
        /// Event that is raised when <see cref="P:Syncfusion.Windows.Gauge.CircularPointer.IncrementKey">Incrementkey</see> property is changed.
        /// </summary>
        public event PropertyChangedCallback IncrementKeyChanged;

        /// <summary>
        /// Event that is raised when <see cref="P:Syncfusion.Windows.Gauge.CircularPointer.DecrementKey">Decrementkey</see> property is changed.
        /// </summary>
        public event PropertyChangedCallback DecrementKeyChanged;


        #endregion

        #region DP getters & setters

        /// <summary>
        /// Gets or sets the width of the pointer. This is a dependency property.
        /// </summary>
        /// <remarks>
        /// Default value is 0.
        /// </remarks>
        /// <value>
        /// Type : <see cref="T:System.Double">System.Double</see>
        /// </value>
        /// <seealso cref="double">double</seealso>
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
        /// Gets or sets the Highlight Selection Brush of the pointer. This is a dependency property.
        /// </summary>
        /// <remarks>
        /// Default value is Colors.Green.
        /// </remarks>
        /// <value>
        /// Type : <see cref="T:Brush">Brush</see>
        /// </value>
        /// <seealso cref="Brush">Brush</seealso>
        public Brush PointerSelectionBrush
        {
            get
            {
                return (Brush)GetValue(PointerSelectionBrushProperty);
            }
            set
            {
                SetValue(PointerSelectionBrushProperty, value);
            }
        }

        /// <summary>
        /// Gets or sets the value to indicate. This is a dependency property.
        /// </summary>
        /// <remarks>
        /// Default value is 0.
        /// </remarks>
        /// <value>
        /// Type : <see cref="T:System.Double">System.Double</see>
        /// </value>
        /// <seealso cref="double">double</seealso>
        public double Value
        {
            get
            {
                return (double)GetValue(ValueProperty);
            }

            set
            {
                SetValue(ValueProperty, value);
            }
        }

        /// <summary>
        /// Gets or sets the position to move the pointer to. This is a dependency property.
        /// </summary>
        /// <remarks>
        /// Default value is 0.
        /// </remarks>
        /// <value>
        /// Type : <see cref="T:System.Double">System.Double</see>
        /// </value>
        /// <seealso cref="double">double</seealso>
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
        /// Gets or sets a value indicating whether to enable PointerInteraction.
        /// </summary>
        /// <remarks>
        /// <para>Default value is true. </para>
        /// </remarks>
        /// <value>
        /// Type: <see cref="T:System.Boolean">System.Boolean</see>
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
        /// Gets or sets a value for Increase Key
        /// </summary>
        /// <remarks>
        /// <para>Default value is Key.Up </para>
        /// </remarks>
        /// <value>
        /// Type: <see cref="T:key">Key</see>
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
        /// Gets or sets a value for Decrease Key
        /// </summary>
        /// <remarks>
        /// <para>Default value is Key.Down </para>
        /// </remarks>
        /// <value>
        /// Type: <see cref="T:key">Key</see>
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
        #endregion

        #region Implementation       
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
        /// Updates the position of the pointer.
        /// </summary>
        protected internal virtual void RefreshPointerPosition()
        {
            LinearScale scale = this.GaugeElementParent as LinearScale;
            if (scale != null)
            {
                TranslateTransform transform = new TranslateTransform();
                transform.X = 0;
                transform.Y = (scale.ScaleBarLength / 2) - this.Position;
                this.RenderTransform = transform;

                if (scale.Orientation == GaugeOrientation.Horizontal)
                {
                    TransformGroup gro = new TransformGroup();
                    TranslateTransform trans = new TranslateTransform();
                    trans.X = 0;
                    trans.Y = (scale.ScaleBarLength / 2) - this.Position;
                    RotateTransform rot = new RotateTransform() { Angle = 90 };
                    gro.Children.Add(trans);
                    gro.Children.Add(rot);
                    this.RenderTransform = gro;
                }
                
            }
        }

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
        /// Fulfils the logic before setting the value of <see cref="Value"/> dependency property.
        /// </summary>
        /// <param name="value">The value that should be corrected.</param>
        /// <returns>The corrected value.</returns>
        protected internal virtual object CoerceValue(object value)
        {
            double val = (double)value;
            if (this.GaugeElementParent is LinearScale)
            {
                LinearScale scale = this.GaugeElementParent as LinearScale;
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
        /// Updates property value cache and raises <see cref="ValueChanged"/> event.
        /// </summary>
        /// <param name="e">
        /// Property change details, such as old value and new value.</param>
        protected virtual void OnValueChanged(DependencyPropertyChangedEventArgs e)
        {
            if (this.misLoaded)
            {
                LinearScale scale = this.GaugeElementParent as LinearScale;
                if (this.Value >= scale.Maximum)
                {
                    this.Value = scale.Maximum;
                    startvalue = 0;
                }
                else if (this.Value <= scale.Minimum)
                {
                    this.Value = scale.Minimum;
                    startvalue = 0;
                    
                }
                else
                {
                    startvalue = 0;
                }
                
                double oldPosition = (this is LinearBarPointer)?scale.GetPointerPositionByValue((double)e.OldValue):scale.GetPositionByValue((double)e.OldValue);
                double newPosition = (this is LinearBarPointer)?scale.GetPointerPositionByValue(this.Value + this.startvalue):scale.GetPositionByValue(this.Value + this.startvalue);
              
                    DoubleAnimation positionAnimation = new DoubleAnimation();
                    positionAnimation.From = oldPosition;
                    positionAnimation.To = newPosition;
                    double duration = Math.Abs(oldPosition - newPosition) * 3;
                    if (duration < 300)
                    {
                        duration = 300;
                    }

                    positionAnimation.Duration = new Duration(TimeSpan.FromMilliseconds(duration));
                    positionAnimation.Completed += new EventHandler(this.PositionAnimationCompleted);
                    Storyboard storyBoard = new Storyboard();
                    storyBoard.Children.Add(positionAnimation);
                    Storyboard.SetTarget(positionAnimation, this);
                    Storyboard.SetTargetProperty(positionAnimation, new PropertyPath("(LinearPointer.Position)"));
                    storyBoard.Begin();
                    this.Position = newPosition;
                    this.mvalueArgs = e;
                    this.RefreshPointerPosition();   
            }
            else
            {
                if (this.ValueChanged != null)
                {
                    this.ValueChanged(this, e);
                }
            }
        }

        /// <summary>
        /// Updates property value cache and raises <see cref="PointerWidthChanged"/> event.
        /// </summary>
        /// <param name="e">
        /// Property change details, such as old value and new value.</param>
        protected virtual void OnPointerWidthChanged(DependencyPropertyChangedEventArgs e)
        {
            this.RefreshPointerPosition();
            if (this.PointerWidthChanged != null)
            {
                this.PointerWidthChanged(this, e);
            }
        }

        /// <summary>
        /// Updates property value cache and raises  event.
        /// </summary>
        /// <param name="e">Property change details, such as old value and new
        /// value.</param>
        protected virtual void OnEnablePointerInteractionChanged(DependencyPropertyChangedEventArgs e)
        {
            if (this.EnablePointerInteractionChanged != null)
            {
                this.EnablePointerInteractionChanged(this, e);
            }
        }

        /// <summary>
        /// Updates property value cache and raises <see cref="PositionChanged"/> event.
        /// </summary>
        /// <param name="e">
        /// Property change details, such as old value and new value.</param>
        protected virtual void OnPositionChanged(DependencyPropertyChangedEventArgs e)
        {
            if (this.misLoaded)
            {
                this.RefreshPointerPosition();
            }

            if (this.PositionChanged != null)
            {
                this.PositionChanged(this, e);
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
                if (instance.misLoaded)
                {
                    if (instance.EnablePointerInteraction == true)
                    {
                        instance1.EnableMouseEvents(instance1.mpointerPath);
                    }
                    else
                    {
                        instance1.DisableMouseEvents(instance1.mpointerPath);
                    }
                }
            }
            else
            {
                LinearMarkerPointer instance2 = (LinearMarkerPointer)d;

                if (instance.misLoaded)
                {
                    if (instance.EnablePointerInteraction == true)
                    {
                        instance2.EnableMouseEvents(instance2.mpointerPath);
                    }
                    else
                    {
                        instance2.DisableMouseEvents(instance2.mpointerPath);
                    }
                }
            }
        }

        /// <summary>
        /// Raises the <see cref="ValueChanged"/> event.
        /// </summary>
        /// <param name="sender">The source of this event.</param>
        /// <param name="e">The instance containing the event data.</param>
        private void PositionAnimationCompleted(object sender, EventArgs e)
        {
            if (this.ValueChanged != null)
            {
                this.ValueChanged(this, this.mvalueArgs);
            }
        }

        /// <summary>
        /// Invoked when the Linear Pointer is loaded
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The object containing the event data.</param>
        private void LinearPointerLoaded(object sender, RoutedEventArgs e)
        {
            this.misLoaded = true;
            if (this.GaugeElementParent is LinearScale)
            {
                LinearScale scale = this.GaugeElementParent as LinearScale;
               
                this.Position = (this is LinearBarPointer)?scale.GetPointerPositionByValue(this.Value):scale.GetPositionByValue(this.Value);
                this.RefreshPointerPosition();
            }
        }

        /// <summary>
        /// Updates property value cache and raises <see cref="PositionChanged"/> event.
        /// </summary>
        /// <param name="e">
        /// Property change details, such as old value and new value.</param>
        protected virtual void OnPointerSelectionBrushChanged(DependencyPropertyChangedEventArgs e)
        {

            if (this.PointerSelectionBrushChanged != null)
            {
                this.PointerSelectionBrushChanged(this, e);
            }
        }

        /// <summary>
        /// Updates property value cache and raises <see cref="PositionChanged"/> event.
        /// </summary>
        /// <param name="e">
        /// Property change details, such as old value and new value.</param>
        protected virtual void OnIncrementKeyChanged(DependencyPropertyChangedEventArgs e)
        {
            if (this.IncrementKeyChanged != null)
            {
                this.IncrementKeyChanged(this, e);
            }
        }

        /// <summary>
        /// Updates property value cache and raises <see cref="PositionChanged"/> event.
        /// </summary>
        /// <param name="e">
        /// Property change details, such as old value and new value.</param>
        protected virtual void OnDecrementKeyChanged(DependencyPropertyChangedEventArgs e)
        {
            if (this.DecrementKeyChanged != null)
            {
                this.DecrementKeyChanged(this, e);
            }
        }

        /// <summary>
        /// Calls OnHighlightSelectionChanged method of the instance, notifies of the depencency property value changes.
        /// </summary>
        /// <param name="d">Dependency object, the change occures on.</param>
        /// <param name="e">Property change details, such as old value and new value.</param>
        public static void OnPointerSelectionBrushChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            LinearPointer instance = (LinearPointer)d;
            instance.OnPointerSelectionBrushChanged(e);
            //if (d.GetType() == typeof(LinearBarPointer))
            //{
            //    LinearBarPointer pointer = (LinearBarPointer)d;
                         
            //}
            //else
            //{
            //    LinearMarkerPointer pointer = (LinearMarkerPointer)d;
                
            //}

        }

        /// <summary>
        /// Calls OnIncreaseKeyChanged method of the instance, notifies of the depencency property value changes.
        /// </summary>
        /// <param name="d">Dependency object, the change occures on.</param>
        /// <param name="e">Property change details, such as old value and new value.</param>

        private static void OnIncrementKeyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            LinearPointer instance = (LinearPointer)d;
            instance.OnPointerSelectionBrushChanged(e);
            if (d.GetType() == typeof(LinearBarPointer))
            {
                LinearBarPointer pointer = (LinearBarPointer)d;
                pointer.OnIncrementKeyChanged(e);
                Key t = (Key)e.NewValue;
                pointer.increaseKeyVal = (Key)e.NewValue;
            }
            else
            {
                LinearMarkerPointer pointer = (LinearMarkerPointer)d;
                pointer.OnIncrementKeyChanged(e);
                pointer.increaseKeyVal = (Key)e.NewValue;
            }

        }

        /// <summary>
        /// Calls OnDecreaseKeyChanged method of the instance, notifies of the depencency property value changes.
        /// </summary>
        /// <param name="d">Dependency object, the change occures on.</param>
        /// <param name="e">Property change details, such as old value and new value.</param>

        private static void OnDecrementKeyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            LinearPointer instance = (LinearPointer)d;
            if (d.GetType() == typeof(LinearBarPointer))
            {

                LinearBarPointer pointer = (LinearBarPointer)d;
                pointer.OnDecrementKeyChanged(e);
                pointer.decreaseKeyVal = (Key)e.NewValue;
            }
            else
            {
                LinearMarkerPointer pointer = (LinearMarkerPointer)d;
                pointer.OnDecrementKeyChanged(e);
                pointer.decreaseKeyVal = (Key)e.NewValue;
            }

        }

        #endregion
    }
}
