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
    /// Represents the base class for the scale.
    /// </summary>
    public class ScaleBase : LocalizableGaugeElement
    {
        #region Dependency properties

        /// <summary>
        /// Identifies the <see cref="MajorIntervalValue"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty MajorIntervalValueProperty =
            DependencyProperty.Register("MajorIntervalValue", typeof(double), typeof(ScaleBase), new PropertyMetadata(10d, new PropertyChangedCallback(OnMajorIntervalValueChanged)));
        
        /// <summary>
        /// Identifies the <see cref="MidIntervalValue"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty MidIntervalValueProperty =
            DependencyProperty.Register("MidIntervalValue", typeof(double), typeof(ScaleBase), new PropertyMetadata(5d, new PropertyChangedCallback(OnMidIntervalValueChanged)));

        /// <summary>
        /// Identifies the <see cref="MinorIntervalValue"/> dependency property.
        /// </summary>`
        public static readonly DependencyProperty MinorIntervalValueProperty =
            DependencyProperty.Register("MinorIntervalValue", typeof(double), typeof(ScaleBase), new PropertyMetadata(2d, new PropertyChangedCallback(OnMinorIntervalValueChanged)));

        /// <summary>
        /// Identifies the <see cref="MajorTicks"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty MajorTicksProperty =
            DependencyProperty.Register("MajorTicks", typeof(int), typeof(ScaleBase), new PropertyMetadata(10, new PropertyChangedCallback(OnMajorTicksChanged)));

        /// <summary>
        /// Identifies the <see cref="MajorTicks"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty MidTicksProperty =
            DependencyProperty.Register("MidTicks", typeof(int), typeof(ScaleBase), new PropertyMetadata(2, new PropertyChangedCallback(OnMidTicksChanged)));

        /// <summary>
        /// Identifies the <see cref="MajorTicks"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty MinorTicksProperty =
            DependencyProperty.Register("MinorTicks", typeof(int), typeof(ScaleBase), new PropertyMetadata(5, new PropertyChangedCallback(OnMinorTicksChanged)));

        /// <summary>
        /// Identifies the <see cref="Maximum"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty MaximumProperty =
            DependencyProperty.Register("Maximum", typeof(double), typeof(ScaleBase), new PropertyMetadata(100d, new PropertyChangedCallback(OnMaximumChanged)));

        /// <summary>
        /// Identifies the <see cref="Minimum"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty MinimumProperty =
            DependencyProperty.Register("Minimum", typeof(double), typeof(ScaleBase), new PropertyMetadata(0d, new PropertyChangedCallback(OnMinimumChanged)));

        /// <summary>
        /// Identifies the <see cref="ShowLastValue"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty ShowLastValueProperty =
            DependencyProperty.Register("ShowLastValue", typeof(bool), typeof(ScaleBase), new PropertyMetadata(true, new PropertyChangedCallback(OnShowLastValueChanged)));
       
        /// <summary>
        /// Identifies the <see cref="ShowLastValue"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty IsReversedProperty =
            DependencyProperty.Register("IsReversed", typeof(bool), typeof(ScaleBase), new PropertyMetadata(false, new PropertyChangedCallback(OnIsReversedChanged)));
        
        /// <summary>
        /// Identifies the <see cref="Minimum"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty ScaleBarSizeProperty =
            DependencyProperty.Register("ScaleBarSize", typeof(double), typeof(ScaleBase), new PropertyMetadata(0d, new PropertyChangedCallback(OnScaleBarSizeChanged)));

        /// <summary>
        /// Identifies the <see cref="ShadowOffset"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty ShadowOffsetProperty =
            DependencyProperty.Register("ShadowOffset", typeof(double), typeof(ScaleBase), new PropertyMetadata(2d, new PropertyChangedCallback(OnShadowOffsetChanged)));

        /// <summary>
        /// Identifies the <see cref="ScalePath"/> dependency property.
        /// </summary>
        internal static readonly DependencyProperty ScalePathProperty =
            DependencyProperty.Register("ScalePath", typeof(Geometry), typeof(ScaleBase), new PropertyMetadata(new PropertyChangedCallback(OnScalePathChanged)));

        #endregion

        #region Private members

        /// <summary>
        /// nested level private member
        /// </summary>
        private int mnestLevel = 0;

        /// <summary>
        /// Collection of the ranges.
        /// </summary>
        private RangesCollection mranges;

        /// <summary>
        /// Collection of the ticks.
        /// </summary>
        private TicksCollection mticks;

        #endregion

        #region Events

        /// <summary>
        /// Event that is raised when <see cref="MajorIntervalValue"/> property is changed.
        /// </summary>
        public event PropertyChangedCallback MajorIntervalValueChanged;

        /// <summary>
        /// Event that is raised when <see cref="MidIntervalValue"/> property is changed.
        /// </summary>
        public event PropertyChangedCallback MidIntervalValueChanged;

        /// <summary>
        /// Event that is raised when <see cref="MinorIntervalValue"/> property is changed.
        /// </summary>
        public event PropertyChangedCallback MinorIntervalValueChanged;

        /// <summary>
        /// Event that is raised when <see cref="MajorTicks"/> property is changed.
        /// </summary>
        public event PropertyChangedCallback MajorTicksChanged;

        /// <summary>
        /// Event that is raised when <see cref="MidTicks"/> property is changed.
        /// </summary>
        public event PropertyChangedCallback MidTicksChanged;

        /// <summary>
        /// Event that is raised when <see cref="MinorTicks"/> property is changed.
        /// </summary>
        public event PropertyChangedCallback MinorTicksChanged;

        /// <summary>
        /// Event that is raised when <see cref="Maximum"/> property is changed.
        /// </summary>
        public event PropertyChangedCallback MaximumChanged;

        /// <summary>
        /// Event that is raised when <see cref="Minimum"/> property is changed.
        /// </summary>
        public event PropertyChangedCallback MinimumChanged;

        /// <summary>
        /// Event that is raised when <see cref="ShowLastValue"/> property is changed.
        /// </summary>
        public event PropertyChangedCallback ShowLastValueChanged;

        /// <summary>
        /// Event that is raised when <see cref="IsReversed"/> property is changed.
        /// </summary>
        public event PropertyChangedCallback IsReversedChanged;

         /// <summary>
        /// Event that is raised when <see cref="ScaleBarSize"/> property is changed.
        /// </summary>
        public event RoutedPropertyChangedEventHandler<double> ScaleBarSizeChanged;

        /// <summary>
        /// Event that is raised when <see cref="ShadowOffset"/> property is changed.
        /// </summary>
        public event PropertyChangedCallback ShadowOffsetChanged;

        /// <summary>
        /// Event that is raised when <see cref="ScalePath"/> property is changed.
        /// </summary>
        internal event PropertyChangedCallback ScalePathChanged;

        #endregion

        #region DP getters & setters

        /// <summary>
        /// Gets or sets the interval between the major ticks.
        /// This is a dependency property.
        /// </summary>
        /// <value>
        /// Type: <see cref="double"/>
        /// Default value is 0.
        /// </value>
        /// <seealso cref="double"/>
        public double MajorIntervalValue
        {
            get
            {
                return (double)GetValue(MajorIntervalValueProperty);
            }

            set
            {
                SetValue(MajorIntervalValueProperty, value);
            }
        }

        /// <summary>
        /// Gets or sets the interval between the middle ticks.
        /// This is a dependency property.
        /// </summary>
        /// <value>
        /// Type: <see cref="double"/>
        /// Default value is 0.
        /// </value>
        /// <seealso cref="double"/>
        public double MidIntervalValue
        {
            get
            {
                return (double)GetValue(MidIntervalValueProperty);
            }

            set
            {
                SetValue(MidIntervalValueProperty, value);
            }
        }

        /// <summary>
        /// Gets or sets the interval between the minor ticks.
        /// This is a dependency property.
        /// </summary>
        /// <value>
        /// Type: <see cref="double"/>
        /// Default value is 0.
        /// </value>
        /// <seealso cref="double"/>
        public double MinorIntervalValue
        {
            get
            {
                return (double)GetValue(MinorIntervalValueProperty);
            }

            set
            {
                SetValue(MinorIntervalValueProperty, value);
            }
        }

        /// <summary>
        /// Gets or sets the number of major ticks.
        /// This is a dependency property.
        /// </summary>
        /// <value>
        /// Type: <see cref="double"/>
        /// Default value is 0.
        /// </value>
        /// <seealso cref="double"/>
        public int MajorTicks
        {
            get
            {
                return (int)GetValue(MajorTicksProperty);
            }

            set
            {
                SetValue(MajorTicksProperty, value);
            }
        }

        /// <summary>
        /// Gets or sets the number of the middle ticks.
        /// This is a dependency property.
        /// </summary>
        /// <value>
        /// Type: <see cref="double"/>
        /// Default value is 0.
        /// </value>
        /// <seealso cref="double"/>
        public int MidTicks
        {
            get
            {
                return (int)GetValue(MidTicksProperty);
            }

            set
            {
                SetValue(MidTicksProperty, value);
            }
        }

        /// <summary>
        /// Gets or sets the number of minor ticks.
        /// This is a dependency property.
        /// </summary>
        /// <value>
        /// Type: <see cref="double"/>
        /// Default value is 0.
        /// </value>
        /// <seealso cref="double"/>
        public int MinorTicks
        {
            get
            {
                return (int)GetValue(MinorTicksProperty);
            }

            set
            {
                SetValue(MinorTicksProperty, value);
            }
        }

        /// <summary>
        /// Gets or sets the maximum value that can be displayed in the scale.
        /// This is a dependency property.
        /// </summary>
        /// <value>
        /// Type: <see cref="double"/>
        /// Default value is 0.
        /// </value>
        /// <seealso cref="double"/>
        public double Maximum
        {
            get
            {
                return (double)GetValue(MaximumProperty);
            }

            set
            {
                SetValue(MaximumProperty, value);
            }
        }

        /// <summary>
        /// Gets or sets the minimum value that can be displayed in the scale.
        /// This is a dependency property.
        /// </summary>
        /// <value>
        /// Type: <see cref="double"/>
        /// Default value is 0.
        /// </value>
        /// <seealso cref="double"/>
        public double Minimum
        {
            get
            {
                return (double)GetValue(MinimumProperty);
            }

            set
            {
                SetValue(MinimumProperty, value);
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether LastValue is to be displayed in the scale.
        /// This is a dependency property.
        /// </summary>
        /// <value>
        /// Type: <see cref="double"/>
        /// Default value is 0.
        /// </value>
        /// <seealso cref="double"/>
        public bool ShowLastValue
        {
            get
            {
                return (bool)GetValue(ShowLastValueProperty);
            }

            set
            {
                SetValue(ShowLastValueProperty, value);
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether the scale is reversed.
        /// This is a dependency property.
        /// </summary>
        /// <value>
        /// Type: <see cref="Boolean"/>
        /// Default value is false.
        /// </value>
        /// <seealso cref="double"/>
        public bool IsReversed
        {
            get
            {
                return (bool)GetValue(IsReversedProperty);
            }

            set
            {
                SetValue(IsReversedProperty, value);
            }
        }        

        /// <summary>
        /// Gets or sets the collection of ranges.
        /// </summary>
        /// <value>
        /// Type: <see cref="RangesCollection"/>
        /// </value>
        /// <seealso cref="RangesCollection"/>
        public RangesCollection Ranges
        {
            get
            {
                return this.mranges;
            }

            set
            {
                this.mranges = value;
            }
        }

        /// <summary>
        /// Gets or sets the width of the scale.
        /// This is a dependency property.
        /// </summary>
        /// <value>
        /// Type: <see cref="double"/>
        /// Default value is 0.
        /// </value>
        /// <seealso cref="double"/>
        public double ScaleBarSize
        {
            get
            {
                return (double)GetValue(ScaleBarSizeProperty);
            }

            set
            {
                SetValue(ScaleBarSizeProperty, value);
            }
        }

        /// <summary>
        /// Gets or sets the offset of elements shadow.
        /// This is a dependency property.
        /// </summary>
        /// <value>
        /// Type: <see cref="double"/>
        /// Default value is 0.
        /// </value>
        /// <seealso cref="double"/>
        public double ShadowOffset
        {
            get
            {
                return (double)GetValue(ShadowOffsetProperty);
            }

            set
            {
                SetValue(ShadowOffsetProperty, value);
            }
        }

        /// <summary>
        /// Gets or sets the collection of ticks.
        /// </summary>
        /// <value>
        /// Type: <see cref="TicksCollection"/>
        /// </value>
        /// <seealso cref="TicksCollection"/>
        public TicksCollection Ticks
        {
            get
            {
                return this.mticks;
            }

            set
            {
                this.mticks = value;
            }
        }

        /// <summary>
        /// Gets or sets the scale drawing path.
        /// This is a dependency property.
        /// </summary>
        /// <value>
        /// Type: <see cref="Geometry"/>
        /// </value>
        /// <seealso cref="Geometry"/>
        internal Geometry ScalePath
        {
            get
            {
                return (Geometry)GetValue(ScalePathProperty);
            }

            set
            {
                SetValue(ScalePathProperty, value);
            }
        }

        #endregion

        #region Implementation
       
        /// <summary>
        /// Fulfils the logic before setting the value of <see cref="ScaleBarSize"/> dependency property.
        /// </summary>
        /// <param name="value">The value that should be corrected.</param>
        /// <returns>The corrected value.</returns>
        protected internal virtual double CoerceScaleBarSize(double value)
        {
            if (value < 0)
            {
                value = 0;
            }

            return value;
        }

        /// <summary>
        /// Refreshes Label Tickset
        /// </summary>
        protected internal virtual void RefreshLabelTickSet()
        {
        }

        /// <summary>
        /// Refreshes Label Tick
        /// </summary>
        protected internal virtual void RefreshLabelTick()
        {
        }

        /// <summary>
        /// Refreshes Scale
        /// </summary>
        protected internal virtual void RefreshScale()
        {
        }

        /// <summary>
        /// Refreshes Tickset 
        /// </summary>
        protected internal virtual void RefreshTickSet()
        {
        }

        /// <summary>
        /// Updates property value cache and raises <see cref="MajorIntervalValueChanged"/> event.
        /// </summary>
        /// <param name="e">
        /// Property change details, such as old value and new value.</param>
        protected virtual void OnMajorIntervalValueChanged(DependencyPropertyChangedEventArgs e)
        {
            if (this.MajorIntervalValueChanged != null)
            {
                this.MajorIntervalValueChanged(this, e);
            }
        }

        /// <summary>
        /// Updates property value cache and raises <see cref="MidIntervalValueChanged"/> event.
        /// </summary>
        /// <param name="e">
        /// Property change details, such as old value and new value.</param>
        protected virtual void OnMidIntervalValueChanged(DependencyPropertyChangedEventArgs e)
        {
            if (this.MidIntervalValueChanged != null)
            {
                this.MidIntervalValueChanged(this, e);
            }
        }

        /// <summary>
        /// Updates property value cache and raises <see cref="MinorIntervalValueChanged"/> event.
        /// </summary>
        /// <param name="e">
        /// Property change details, such as old value and new value.</param>
        protected virtual void OnMinorIntervalValueChanged(DependencyPropertyChangedEventArgs e)
        {
            if (this.MinorIntervalValueChanged != null)
            {
                this.MinorIntervalValueChanged(this, e);
            }
        }
        
        /// <summary>
        /// Updates property value cache and raises <see cref="MajorTicksChanged"/> event.
        /// </summary>
        /// <param name="e">
        /// Property change details, such as old value and new value.</param>
        protected virtual void OnMajorTicksChanged(DependencyPropertyChangedEventArgs e)
        {
            if (this.MajorTicksChanged != null)
            {
                this.MajorTicksChanged(this, e);
            }
        }

        /// <summary>
        /// Updates property value cache and raises <see cref="MidTicksChanged"/> event.
        /// </summary>
        /// <param name="e">
        /// Property change details, such as old value and new value.</param>
        protected virtual void OnMidTicksChanged(DependencyPropertyChangedEventArgs e)
        {
            if (this.MidTicksChanged != null)
            {
                this.MidTicksChanged(this, e);
            }
        }

        /// <summary>
        /// Updates property value cache and raises <see cref="MinorTicksChanged"/> event.
        /// </summary>
        /// <param name="e">
        /// Property change details, such as old value and new value.</param>
        protected virtual void OnMinorTicksChanged(DependencyPropertyChangedEventArgs e)
        {
            if (this.MinorTicksChanged != null)
            {
                this.MinorTicksChanged(this, e);
            }
        }

        /// <summary>
        /// Updates property value cache and raises <see cref="MaximumChanged"/> event.
        /// </summary>
        /// <param name="e">
        /// Property change details, such as old value and new value.</param>
        protected virtual void OnMaximumChanged(DependencyPropertyChangedEventArgs e)
        {
            if (this.MaximumChanged != null)
            {
                this.MaximumChanged(this, e);
            }
        }

        /// <summary>
        /// Updates property value cache and raises <see cref="MinimumChanged"/> event.
        /// </summary>
        /// <param name="e">
        /// Property change details, such as old value and new value.</param>
        protected virtual void OnMinimumChanged(DependencyPropertyChangedEventArgs e)
        {
            if (this.MinimumChanged != null)
            {
                this.MinimumChanged(this, e);
            }
        }

        /// <summary>
        /// Updates property value cache and raises <see cref="ShowLastValueChanged"/> event.
        /// </summary>
        /// <param name="e">
        /// Property change details, such as old value and new value.</param>
        protected virtual void OnShowLastValueChanged(DependencyPropertyChangedEventArgs e)
        {
            if (this.ShowLastValueChanged != null)
            {
                this.ShowLastValueChanged(this, e);
            }
        }

        /// <summary>
        /// Updates property value cache and raises <see cref="ShowLastValueChanged"/> event.
        /// </summary>
        /// <param name="e">
        /// Property change details, such as old value and new value.</param>
        protected virtual void OnIsReversedChanged(DependencyPropertyChangedEventArgs e)
        {
            if (this.IsReversedChanged != null)
            {
                this.IsReversedChanged(this, e);
            }
        }
        
        /// <summary>
        /// Updates property value cache and raises <see cref="ScaleBarSizeChanged"/> event.
        /// </summary>
        /// <param name="e">
        /// Property change details, such as old value and new value.</param>
        protected virtual void OnScaleBarSizeChanged(RoutedPropertyChangedEventArgs<double> e)
        {
            if (this.ScaleBarSizeChanged != null)
            {
                this.ScaleBarSizeChanged(this, e);
            }
        }

        /// <summary>
        /// Updates property value cache and raises <see cref="ScalePathChanged"/> event.
        /// </summary>
        /// <param name="e">
        /// Property change details, such as old value and new value.</param>
        protected virtual void OnScalePathChanged(DependencyPropertyChangedEventArgs e)
        {
            if (this.ScalePathChanged != null)
            {
                this.ScalePathChanged(this, e);
            }
        }

        /// <summary>
        /// Updates property value cache and raises <see cref="ShadowOffsetChanged"/> event.
        /// </summary>
        /// <param name="e">
        /// Property change details, such as old value and new value.</param>
        protected virtual void OnShadowOffsetChanged(DependencyPropertyChangedEventArgs e)
        {
            if (this.ShadowOffsetChanged != null)
            {
                this.ShadowOffsetChanged(this, e);
            }
        }

        /// <summary>
        /// Calls OnMajorIntervalValueChanged method of the instance, notifies of the depencency property value changes.
        /// </summary>
        /// <param name="d">Dependency object, the change occures on.</param>
        /// <param name="e">Property change details, such as old value and new value.</param>
        private static void OnMajorIntervalValueChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            ScaleBase instance = (ScaleBase)d;
            instance.OnMajorIntervalValueChanged(e);
        }

        /// <summary>
        /// Calls OnMidIntervalValueChanged method of the instance, notifies of the depencency property value changes.
        /// </summary>
        /// <param name="d">Dependency object, the change occures on.</param>
        /// <param name="e">Property change details, such as old value and new value.</param>
        private static void OnMidIntervalValueChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            ScaleBase instance = (ScaleBase)d;
            instance.OnMidIntervalValueChanged(e);
        }

        /// <summary>
        /// Calls OnMinorIntervalValueChanged method of the instance, notifies of the depencency property value changes.
        /// </summary>
        /// <param name="d">Dependency object, the change occures on.</param>
        /// <param name="e">Property change details, such as old value and new value.</param>
        private static void OnMinorIntervalValueChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            ScaleBase instance = (ScaleBase)d;
            instance.OnMinorIntervalValueChanged(e);
        }

        /// <summary>
        /// Calls OnMajorTicksChanged method of the instance, notifies of the depencency property value changes.
        /// </summary>
        /// <param name="d">Dependency object, the change occures on.</param>
        /// <param name="e">Property change details, such as old value and new value.</param>
        private static void OnMajorTicksChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            ScaleBase instance = (ScaleBase)d;
            instance.OnMajorTicksChanged(e);
        }

        /// <summary>
        /// Calls OnMidTicksChanged method of the instance, notifies of the depencency property value changes.
        /// </summary>
        /// <param name="d">Dependency object, the change occures on.</param>
        /// <param name="e">Property change details, such as old value and new value.</param>
        private static void OnMidTicksChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            ScaleBase instance = (ScaleBase)d;
            instance.OnMidTicksChanged(e);
        }

        /// <summary>
        /// Calls OnMinorTicksChanged method of the instance, notifies of the depencency property value changes.
        /// </summary>
        /// <param name="d">Dependency object, the change occures on.</param>
        /// <param name="e">Property change details, such as old value and new value.</param>
        private static void OnMinorTicksChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            ScaleBase instance = (ScaleBase)d;
            instance.OnMinorTicksChanged(e);
        }

        /// <summary>
        /// Calls OnMaximumChanged method of the instance, notifies of the depencency property value changes.
        /// </summary>
        /// <param name="d">Dependency object, the change occures on.</param>
        /// <param name="e">Property change details, such as old value and new value.</param>
        private static void OnMaximumChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            ScaleBase instance = (ScaleBase)d;
            instance.OnMaximumChanged(e);
        }

        /// <summary>
        /// Calls OnMinimumChanged method of the instance, notifies of the depencency property value changes.
        /// </summary>
        /// <param name="d">Dependency object, the change occures on.</param>
        /// <param name="e">Property change details, such as old value and new value.</param>
        private static void OnMinimumChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            ScaleBase instance = (ScaleBase)d;
            instance.OnMinimumChanged(e);
        }

        /// <summary>
        /// Calls OnMinimumChanged method of the instance, notifies of the depencency property value changes.
        /// </summary>
        /// <param name="d">Dependency object, the change occures on.</param>
        /// <param name="e">Property change details, such as old value and new value.</param>
        private static void OnShowLastValueChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            ScaleBase instance = (ScaleBase)d;
            instance.OnShowLastValueChanged(e);
        }

        /// <summary>
        /// Calls OnMinimumChanged method of the instance, notifies of the depencency property value changes.
        /// </summary>
        /// <param name="d">Dependency object, the change occures on.</param>
        /// <param name="e">Property change details, such as old value and new value.</param>
        private static void OnIsReversedChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            ScaleBase instance = (ScaleBase)d;
            instance.OnIsReversedChanged(e);
        }

        /// <summary>
        /// Calls OnShadowOffsetChanged method of the instance, notifies of the depencency property value changes.
        /// </summary>
        /// <param name="d">Dependency object, the change occures on.</param>
        /// <param name="e">Property change details, such as old value and new value.</param>
        private static void OnShadowOffsetChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            ScaleBase instance = (ScaleBase)d;
            instance.OnShadowOffsetChanged(e);
        }

        /// <summary>
        /// Calls OnScalePathChanged method of the instance, notifies of the depencency property value changes.
        /// </summary>
        /// <param name="d">Dependency object, the change occures on.</param>
        /// <param name="e">Property change details, such as old value and new value.</param>
        private static void OnScalePathChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            ScaleBase instance = (ScaleBase)d;
            instance.OnScalePathChanged(e);
        }

        /// <summary>
        /// Calls OnScaleBarSizeChanged method of the instance, notifies of the depencency property value changes.
        /// </summary>
        /// <param name="d">Dependency object, the change occures on.</param>
        /// <param name="e">Property change details, such as old value and new value.</param>
        private static void OnScaleBarSizeChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            ScaleBase instance = (ScaleBase)d;

            double newValue = (double)e.NewValue;
            double oldValue = (double)e.OldValue;
            instance.mnestLevel++;

            // coerce newValue
            double coercedValue = instance.CoerceScaleBarSize(newValue);
            if (newValue != coercedValue)
            {
                instance.ScaleBarSize = coercedValue;
            }

            instance.mnestLevel--;
            if (instance.mnestLevel == 0)
            {
                RoutedPropertyChangedEventArgs<double> args =
                    new RoutedPropertyChangedEventArgs<double>(oldValue, newValue);
                instance.OnScaleBarSizeChanged(args);
            }
        }

        #endregion
    }
}
