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

    /// <summary>
    /// Represents the base class for the range.
    /// </summary>
    public abstract class RangeBase : GaugeElement
    {
        #region Dependency properties

        /// <summary>
        /// Identifies the <see cref="DistanceFromScale"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty DistanceFromScaleProperty =
            DependencyProperty.Register("DistanceFromScale", typeof(double), typeof(RangeBase), new PropertyMetadata(0d, new PropertyChangedCallback(OnDistanceFromScaleChanged)));

        /// <summary>
        /// Identifies the <see cref="EndValue"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty EndValueProperty =
            DependencyProperty.Register("EndValue", typeof(double), typeof(RangeBase), new PropertyMetadata(0d, new PropertyChangedCallback(OnEndValueChanged)));

        /// <summary>
        /// Identifies the <see cref="EndWidth"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty EndWidthProperty =
            DependencyProperty.Register("EndWidth", typeof(double), typeof(RangeBase), new PropertyMetadata(0d, new PropertyChangedCallback(OnEndWidthChanged)));

        /// <summary>
        /// Identifies the <see cref="RangePosition"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty RangePositionProperty =
            DependencyProperty.Register("RangePosition", typeof(ScalePlacement), typeof(RangeBase), new PropertyMetadata(ScalePlacement.Inside, new PropertyChangedCallback(OnRangePositionChanged)));

        /// <summary>
        /// Identifies the <see cref="StartValue"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty StartValueProperty =
            DependencyProperty.Register("StartValue", typeof(double), typeof(RangeBase), new PropertyMetadata(0d, new PropertyChangedCallback(OnStartValueChanged)));

        /// <summary>
        /// Identifies the <see cref="StartWidth"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty StartWidthProperty =
            DependencyProperty.Register("StartWidth", typeof(double), typeof(RangeBase), new PropertyMetadata(0d, new PropertyChangedCallback(OnStartWidthChanged)));

        /// <summary>
        /// Identifies the <see cref="BindIndicator"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty BindIndicatorProperty =
            DependencyProperty.Register("BindIndicator", typeof(Boolean), typeof(RangeBase), new PropertyMetadata(false, new PropertyChangedCallback(OnBindIndicatorChanged)));

        /// <summary>
        /// Identifies the <see cref="BindIndicator"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty IndicatorColorProperty =
            DependencyProperty.Register("IndicatorColor", typeof(Brush), typeof(RangeBase),new PropertyMetadata(null, new PropertyChangedCallback(OnIndicatorColorChanged)));

        #endregion

        #region Private members
        
        /// <summary>
        /// Variable to count the level
        /// </summary>
        private int mnestLevel = 0;

        #endregion

        #region Events

        /// <summary>
        /// Event that is raised when <see cref="DistanceFromScale"/> property is changed.
        /// </summary>
        public event PropertyChangedCallback DistanceFromScaleChanged;

        /// <summary>
        /// Event that is raised when <see cref="EndValue"/> property is changed.
        /// </summary>
        public event RoutedPropertyChangedEventHandler<double> EndValueChanged;

        /// <summary>
        /// Event that is raised when <see cref="EndWidth"/> property is changed.
        /// </summary>
        public event PropertyChangedCallback EndWidthChanged;

        /// <summary>
        /// Event that is raised when <see cref="RangePosition"/> property is changed.
        /// </summary>
        public event PropertyChangedCallback RangePositionChanged;

        /// <summary>
        /// Event that is raised when <see cref="StartValue"/> property is changed.
        /// </summary>
        public event RoutedPropertyChangedEventHandler<double> StartValueChanged;

        /// <summary>
        /// Event that is raised when <see cref="StartWidth"/> property is changed.
        /// </summary>
        public event PropertyChangedCallback StartWidthChanged;

         /// <summary>
        /// Event that is raised when <see cref="BindIndicator"/> property is changed.
        /// </summary>
        public event PropertyChangedCallback BindIndicatorChanged;

        /// <summary>
        /// Event that is raised when <see cref="IndicatorColor"/> property is changed.
        /// </summary>
        public event PropertyChangedCallback IndicatorColorChanged;

        #endregion

        #region DP getters & setters
        
        /// <summary>
        /// Gets or sets the distance between the range and the scale.
        /// This is a dependency property.
        /// </summary>
        /// <value>
        /// Type: <see cref="double"/>
        /// Default value is 0.
        /// </value>
        /// <seealso cref="double"/>
        public double DistanceFromScale
        {
            get
            {
                return (double)GetValue(DistanceFromScaleProperty);
            }

            set
            {
                SetValue(DistanceFromScaleProperty, value);
            }
        }

        /// <summary>
        /// Gets or sets the end value of the range.
        /// This is a dependency property.
        /// </summary>
        /// <value>
        /// Type: <see cref="double"/>
        /// Default value is 0.
        /// </value>
        /// <seealso cref="double"/>
        public double EndValue
        {
            get
            {
                return (double)GetValue(EndValueProperty);
            }

            set
            {
                SetValue(EndValueProperty, value);
            }
        }

        /// <summary>
        /// Gets or sets the width of the range at its end.
        /// This is a dependency property.
        /// </summary>
        /// <value>
        /// Type: <see cref="double"/>
        /// Default value is 0.
        /// </value>
        /// <seealso cref="double"/>
        public double EndWidth
        {
            get
            {
                return (double)GetValue(EndWidthProperty);
            }

            set
            {
                SetValue(EndWidthProperty, value);
            }
        }

        /// <summary>
        /// Gets or sets the placement of the range 
        /// relatively to the scale. This is a dependency property.
        /// </summary>
        /// <value>
        /// Type: <see cref="double"/>
        /// Default value is ScalePlacement.Inside.
        /// </value>
        /// <seealso cref="double"/>
        public ScalePlacement RangePosition
        {
            get
            {
                return (ScalePlacement)GetValue(RangePositionProperty);
            }

            set
            {
                SetValue(RangePositionProperty, value);
            }
        }

        /// <summary>
        /// Gets or sets the start value of the range.
        /// This is a dependency property.
        /// </summary>
        /// <value>
        /// Type: <see cref="double"/>
        /// Default value is 0.
        /// </value>
        /// <seealso cref="double"/>
        public double StartValue
        {
            get
            {
                return (double)GetValue(StartValueProperty);
            }

            set
            {
                SetValue(StartValueProperty, value);
            }
        }

        /// <summary>
        /// Gets or sets the width of the range at its start.
        /// This is a dependency property.
        /// </summary>
        /// <value>
        /// Type: <see cref="double"/>
        /// Default value is 0.
        /// </value>
        /// <seealso cref="double"/>
        public double StartWidth
        {
            get
            {
                return (double)GetValue(StartWidthProperty);
            }

            set
            {
                SetValue(StartWidthProperty, value);
            }
        }

         /// <summary>
        /// Gets or sets the width of the range at its start.
        /// This is a dependency property.
        /// </summary>
        /// <value>
        /// Type: <see cref="bool"/>
        /// Default value is false.
        /// </value>
        /// <seealso cref="BindIndicator"/>
        public bool BindIndicator
        {
            get
            {
                return (bool)GetValue(BindIndicatorProperty);
            }

            set
            {
                SetValue(BindIndicatorProperty, value);
            }
        }

        /// <summary>
        /// Gets or sets the indicator color of the range.
        /// This is a dependency property.
        /// </summary>
        /// <value>
        /// Type: <see cref="Brush"/>
        /// Default value is false.
        /// </value>
        public Brush IndicatorColor
        {
            get
            {
                return (Brush)GetValue(IndicatorColorProperty);
            }

            set
            {
                SetValue(IndicatorColorProperty, value);
            }
        }
        #endregion

        #region Implementation

        /// <summary>
        /// Calculates the end value.
        /// </summary>
        /// <param name="val">Provided value</param>
        /// <returns>Returns the End value</returns>
        protected internal virtual double CoerceEndValue(double val)
        {
            ScaleBase scale = this.GaugeElementParent as ScaleBase;
            if (scale != null)
            {
                if (val > scale.Maximum)
                {
                    val = scale.Maximum;
                }
            }

            if (val < this.StartValue)
            {
                this.StartValue = val;
            }

            return val;
        }

        /// <summary>
        /// Calculates the start value.
        /// </summary>
        /// <param name="val">Provided value</param>
        /// <returns>Returns the Start value</returns>
        protected internal virtual double CoerceStartValue(double val)
        {
            ScaleBase scale = this.GaugeElementParent as ScaleBase;
            if (scale != null)
            {
                if (val < scale.Minimum)
                {
                    val = scale.Minimum;
                }
            }

            if (val > this.EndValue)
            {
                this.EndValue = val;
            }

            return val;
        }

        /// <summary>
        /// Updates property value cache and raises <see cref="DistanceFromScaleChanged"/> event.
        /// </summary>
        /// <param name="e">
        /// Property change details, such as old value and new value.</param>
        protected virtual void OnDistanceFromScaleChanged(DependencyPropertyChangedEventArgs e)
        {
            if (this.DistanceFromScaleChanged != null)
            {
                this.DistanceFromScaleChanged(this, e);
            }
        }

        /// <summary>
        /// Updates property value cache and raises <see cref="EndValueChanged"/> event.
        /// </summary>
        /// <param name="e">
        /// Property change details, such as old value and new value.</param>
        protected virtual void OnEndValueChanged(RoutedPropertyChangedEventArgs<double> e)
        {
            if (this.EndValueChanged != null)
            {
                this.EndValueChanged(this, e);
            }
        }

        /// <summary>
        /// Updates property value cache and raises <see cref="EndWidthChanged"/> event.
        /// </summary>
        /// <param name="e">
        /// Property change details, such as old value and new value.</param>
        protected virtual void OnEndWidthChanged(DependencyPropertyChangedEventArgs e)
        {
            if (this.EndWidthChanged != null)
            {
                this.EndWidthChanged(this, e);
            }
        }

        /// <summary>
        /// Updates property value cache and raises <see cref="RangePositionChanged"/> event.
        /// </summary>
        /// <param name="e">
        /// Property change details, such as old value and new value.</param>
        protected virtual void OnRangePositionChanged(DependencyPropertyChangedEventArgs e)
        {
            if (this.RangePositionChanged != null)
            {
                this.RangePositionChanged(this, e);
            }
        }

        /// <summary>
        /// Updates property value cache and raises <see cref="StartValueChanged"/> event.
        /// </summary>
        /// <param name="e">
        /// Property change details, such as old value and new value.</param>
        protected virtual void OnStartValueChanged(RoutedPropertyChangedEventArgs<double> e)
        {
            if (this.StartValueChanged != null)
            {
                this.StartValueChanged(this, e);
            }
        }

        /// <summary>
        /// Updates property value cache and raises <see cref="StartWidthChanged"/> event.
        /// </summary>
        /// <param name="e">
        /// Property change details, such as old value and new value.</param>
        protected virtual void OnStartWidthChanged(DependencyPropertyChangedEventArgs e)
        {
            if (this.StartWidthChanged != null)
            {
                this.StartWidthChanged(this, e);
            }
        }

        /// <summary>
        /// Calls OnEndWidthChanged method of the instance, notifies of the depencency property value changes.
        /// </summary>
        /// <param name="d">Dependency object, the change occures on.</param>
        /// <param name="e">Property change details, such as old value and new value.</param>
        private static void OnEndWidthChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            RangeBase instance = (RangeBase)d;
            instance.OnEndWidthChanged(e);
        }

         /// <summary>
        /// Updates property value cache and raises <see cref="StartValueChanged"/> event.
        /// </summary>
        /// <param name="e">
        /// Property change details, such as old value and new value.</param>
        protected virtual void OnBindIndicatorChanged(DependencyPropertyChangedEventArgs e)
        {
            if (BindIndicatorChanged != null)
            {
                this.BindIndicatorChanged(this, e);
            }
        }

        /// <summary>
        /// Calls OnStartValueChanged method of the instance, notifies of the depencency property value changes.
        /// </summary>
        /// <param name="d">Dependency object, the change occures on.</param>
        /// <param name="e">Property change details, such as old value and new value.</param>
        private static void OnBindIndicatorChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            RangeBase instance = (RangeBase)d;
            instance.OnBindIndicatorChanged(e);
        }

        /// <summary>
        /// Updates property value cache and raises <see cref="IndicatorColorChanged"/> event.
        /// </summary>
        /// <param name="e">
        /// Property change details, such as old value and new value.</param>
        protected virtual void OnIndicatorColorChanged(DependencyPropertyChangedEventArgs e)
        {
            if (IndicatorColorChanged != null)
            {
                this.IndicatorColorChanged(this, e);
            }
        }

        /// <summary>
        /// Calls OnIndicatorColorChanged method of the instance, notifies of the depencency property value changes.
        /// </summary>
        /// <param name="d">Dependency object, the change occures on.</param>
        /// <param name="e">Property change details, such as old value and new value.</param>
        private static void OnIndicatorColorChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            RangeBase instance = (RangeBase)d;
            instance.OnIndicatorColorChanged(e);
        }

        /// <summary>
        /// Calls OnDistanceFromScaleChanged method of the instance, notifies of the depencency property value changes.
        /// </summary>
        /// <param name="d">Dependency object, the change occures on.</param>
        /// <param name="e">Property change details, such as old value and new value.</param>
        private static void OnDistanceFromScaleChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            RangeBase instance = (RangeBase)d;
            instance.OnDistanceFromScaleChanged(e);
        }

        /// <summary>
        /// Calls OnEndValueChanged method of the instance, notifies of the depencency property value changes.
        /// </summary>
        /// <param name="d">Dependency object, the change occures on.</param>
        /// <param name="e">Property change details, such as old value and new value.</param>
        private static void OnEndValueChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            RangeBase instance = (RangeBase)d;

            double newValue = (double)e.NewValue;
            double oldValue = (double)e.OldValue;
            instance.mnestLevel++;
            double coercedValue = instance.CoerceEndValue(newValue);
            if (newValue != coercedValue)
            {
                instance.EndValue = coercedValue;
            }

            instance.mnestLevel--;
            if (instance.mnestLevel == 0)
            {
                RoutedPropertyChangedEventArgs<double> args =
                    new RoutedPropertyChangedEventArgs<double>(oldValue, newValue);

                instance.OnEndValueChanged(args);
            }
        }

        /// <summary>
        /// Calls OnStartWidthChanged method of the instance, notifies of the depencency property value changes.
        /// </summary>
        /// <param name="d">Dependency object, the change occures on.</param>
        /// <param name="e">Property change details, such as old value and new value.</param>
        private static void OnStartWidthChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            RangeBase instance = (RangeBase)d;
            instance.OnStartWidthChanged(e);
        }

        /// <summary>
        /// Calls OnRangePositionChanged method of the instance, notifies of the depencency property value changes.
        /// </summary>
        /// <param name="d">Dependency object, the change occures on.</param>
        /// <param name="e">Property change details, such as old value and new value.</param>
        private static void OnRangePositionChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            RangeBase instance = (RangeBase)d;
            instance.OnRangePositionChanged(e);
        }

        /// <summary>
        /// Calls OnStartValueChanged method of the instance, notifies of the depencency property value changes.
        /// </summary>
        /// <param name="d">Dependency object, the change occures on.</param>
        /// <param name="e">Property change details, such as old value and new value.</param>
        private static void OnStartValueChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            RangeBase instance = (RangeBase)d;

            double newValue = (double)e.NewValue;
            double oldValue = (double)e.OldValue;

            instance.mnestLevel++;
            double coercedValue = instance.CoerceStartValue(newValue);
            if (newValue != coercedValue)
            {
                instance.StartValue = coercedValue;
            }

            instance.mnestLevel--;
            if (instance.mnestLevel == 0)
            {
                RoutedPropertyChangedEventArgs<double> args =
                    new RoutedPropertyChangedEventArgs<double>(oldValue, newValue);
                instance.OnStartValueChanged(args);
            }
        }

        #endregion
    }
}
