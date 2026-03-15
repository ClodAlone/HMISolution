// <copyright file="ScaleBase.cs" company="Syncfusion Software">
// Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Any infringement will be prosecuted under
// applicable laws. 
// </copyright>

using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace Syncfusion.Windows.Gauge
{
    /// <summary>
    /// Represents the abstract base class for the Scale, used in Gauges.
    /// </summary>
#if SyncfusionFramework4_0
    [System.ComponentModel.DesignTimeVisible(false)]
#endif
    public abstract class ScaleBase : LocalizableGaugeElement
    {       
        #region Private Members
        /// <summary>
        /// Collection of the ranges.
        /// </summary>
        private RangesCollection m_ranges;

        /// <summary>
        /// Collection of the ticks.
        /// </summary>
        private TicksCollection m_ticks;
        #endregion Private Memebers

        #region CLR Getters & Setters
        /// <summary>
        /// Gets or sets a Collection of ranges.
        /// </summary>
        /// <value>
        /// Type: <see cref="RangesCollection"/>
        /// </value>
        public RangesCollection Ranges
        {
            get
            {
                return m_ranges;
            }

            set
            {
                m_ranges = value;
            }
        }

        /// <summary>
        /// Gets or sets a Collection of ticks.
        /// </summary>
        /// <value>
        /// Type: <see cref="TicksCollection"/>
        /// </value>
        public TicksCollection Ticks
        {
            get
            {
                return m_ticks;
            }

            set
            {
                m_ticks = value;
            }
        }
        #endregion CLR Getters & Setters

        #region Events
        /// <summary>
        /// Event that is raised when <see cref="MajorIntervalValue"/> property is changed.
        /// </summary>
        public event PropertyChangedCallback MajorIntervalValueChanged;

        /// <summary>
        /// Event that is raised when <see cref="Maximum"/> property is changed.
        /// </summary>
        public event PropertyChangedCallback MaximumChanged;

        /// <summary>
        /// Event that is raised when <see cref="Minimum"/> property is changed.
        /// </summary>
        public event PropertyChangedCallback MinimumChanged;

        /// <summary>
        /// Event that is raised when <see cref="MinorIntervalValue"/> property is changed.
        /// </summary>
        public event PropertyChangedCallback MinorIntervalValueChanged;
                
        /// <summary>
        /// Event that is raised when <see cref="ScaleBarSize"/> property is changed.
        /// </summary>
        public event PropertyChangedCallback ScaleBarSizeChanged;

        /// <summary>
        /// Event that is raised when <see cref="ShadowOffset"/> property is changed.
        /// </summary>
        public event PropertyChangedCallback ShadowOffsetChanged;

        /// <summary>
        /// Event that is raised when IsReversed property is changed.
        /// </summary>
        public event PropertyChangedCallback ScaleDirectionChanged;

        #endregion Events

        #region Dependency Properties
        /// <summary>
        /// Identifies the <see cref="MajorIntervalValue"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty MajorIntervalValueProperty =
            DependencyProperty.Register("MajorIntervalValue", typeof(double), typeof(ScaleBase), new FrameworkPropertyMetadata(0d, new PropertyChangedCallback(OnMajorIntervalValueChanged)));

        /// <summary>
        /// Identifies the <see cref="Maximum"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty MaximumProperty =
            DependencyProperty.Register("Maximum", typeof(double), typeof(ScaleBase), new FrameworkPropertyMetadata(0d, new PropertyChangedCallback(OnMaximumChanged), CoerceMaximum));

        /// <summary>
        /// Identifies the <see cref="Minimum"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty MinimumProperty =
            DependencyProperty.Register("Minimum", typeof(double), typeof(ScaleBase), new FrameworkPropertyMetadata(0d, new PropertyChangedCallback(OnMinimumChanged), CoerceMinimum));

        /// <summary>
        /// Identifies the <see cref="MinorIntervalValue"/> dependency property.
        /// </summary>`
        public static readonly DependencyProperty MinorIntervalValueProperty =
            DependencyProperty.Register("MinorIntervalValue", typeof(double), typeof(ScaleBase), new FrameworkPropertyMetadata(0d, new PropertyChangedCallback(OnMinorIntervalValueChanged)));

        /// <summary>
        /// Identifies the <see cref="IsNumberDivision"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty IsNumberDivisionProperty =
            DependencyProperty.Register("IsNumberDivision", typeof(bool), typeof(ScaleBase), new FrameworkPropertyMetadata(false, new PropertyChangedCallback(OnIntervalValueChanged)));

        /// <summary>
        /// Identifies the <see cref="NumberMinorDivision"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty NumberMinorDivisionProperty =
            DependencyProperty.Register("NumberMinorDivision", typeof(double), typeof(ScaleBase), new FrameworkPropertyMetadata(0d, new PropertyChangedCallback(OnMinorIntervalValueChanged)));

        /// <summary>
        /// Identifies the <see cref="NumberMajorDivision"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty NumberMajorDivisionProperty =
            DependencyProperty.Register("NumberMajorDivision", typeof(double), typeof(ScaleBase), new FrameworkPropertyMetadata(0d, new PropertyChangedCallback(OnMajorIntervalValueChanged)));

        /// <summary>
        /// Identifies the <see cref="Minimum"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty ScaleBarSizeProperty =
            DependencyProperty.Register("ScaleBarSize", typeof(double), typeof(ScaleBase), new FrameworkPropertyMetadata(0d, FrameworkPropertyMetadataOptions.AffectsRender | FrameworkPropertyMetadataOptions.AffectsMeasure | FrameworkPropertyMetadataOptions.AffectsParentMeasure, new PropertyChangedCallback(OnScaleBarSizeChanged), CoerceScaleBarSize));

        /// <summary>
        /// Identifies the <see cref="ShadowOffset"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty ShadowOffsetProperty =
            DependencyProperty.Register("ShadowOffset", typeof(double), typeof(ScaleBase), new FrameworkPropertyMetadata(0d, new PropertyChangedCallback(OnShadowOffsetChanged)));
       
        /// <summary>
        /// Identifies the <see cref="ScaleDirection"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty ScaleDirectionProperty =
            DependencyProperty.Register("ScaleDirection", typeof(ScaleDirection), typeof(ScaleBase), new PropertyMetadata(ScaleDirection.Clockwise, new PropertyChangedCallback(OnScaleDirectionChanged)));
        #endregion Dependency Properties

        #region DP Getters & Setters
        /// <summary>
        /// Gets or sets the interval between the major ticks.
        /// This is a dependency property.
        /// </summary>
        /// <value>
        /// Type: <see cref="double"/>
        /// Default value is 0.
        /// </value>
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
        /// Gets or sets the maximum value that can be displayed in the scale.
        /// This is a dependency property.
        /// </summary>
        /// <value>
        /// Type: <see cref="double"/>
        /// Default value is 0.
        /// </value>
        /// <remarks>
        /// Maximum should be set before <seealso cref="Minimum"/> value. Since Minimum
        /// should not be greater than Maximum value.
        /// </remarks>
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
        /// <remarks>
        /// Minimum should be set after <seealso cref="Maximum"/> value is set. Since Minimum
        /// should not be greater than Maximum value.
        /// </remarks>
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
        /// Gets or sets the interval between the minor ticks.
        /// This is a dependency property.
        /// </summary>
        /// <value>
        /// Type: <see cref="double"/>
        /// Default value is 0.
        /// </value>
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
        /// Gets or sets the boolean value to set the number division for Ticks.
        /// This is a dependency property.
        /// </summary>
        /// <value>
        /// Type: <see cref="double"/>
        /// Default value is false.
        /// </value>
        public bool IsNumberDivision
        {
            get
            {
                return (bool)GetValue(IsNumberDivisionProperty);
            }

            set
            {
                SetValue(IsNumberDivisionProperty, value);
            }
        }

        /// <summary>
        /// Gets or sets the value to set the number division for Ticks.
        /// This is a dependency property.
        /// </summary>
        /// <value>
        /// Type: <see cref="double"/>
        /// Default value is false.
        /// </value>
        public double NumberMinorDivision
        {
            get
            {
                return (double)GetValue(NumberMinorDivisionProperty);
            }

            set
            {
                SetValue(NumberMinorDivisionProperty, value);
            }
        }

        /// <summary>
        /// Gets or sets the value to set the number major division.
        /// This is a dependency property.
        /// </summary>
        /// <value>
        /// Type: <see cref="double"/>
        /// Default value is 0.
        /// </value>
        public double NumberMajorDivision
        {
            get
            {
                return (double)GetValue(NumberMajorDivisionProperty);
            }

            set
            {
                SetValue(NumberMajorDivisionProperty, value);
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
        /// Gets or sets the offset of element's shadow.
        /// This is a dependency property.
        /// </summary>
        /// <value>
        /// Type: <see cref="double"/>
        /// Default value is 0.
        /// </value>
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
        /// Gets or sets the ScaleDirection of scale.
        /// This is a dependency property.
        /// </summary>
        /// <value>
        /// Type: <see cref="double"/>
        /// Default value is Clockwise direction.
        /// </value>
        public ScaleDirection ScaleDirection
        {
            get
            {
                return (ScaleDirection)GetValue(ScaleDirectionProperty);
            }

            set
            {
                SetValue(ScaleDirectionProperty, value);
            }
        }
        #endregion DP Getters & Setters
        
        #region Implementation
        /// <summary>
        /// Fulfils the logic before setting the value of <see cref="Maximum"/> dependency property.
        /// </summary>
        /// <param name="value">The value that should be corrected.</param>
        /// <returns>The corrected value.</returns>
        protected internal virtual object CoerceMaximum(object value)
        {
            double val = (double)value;            

            /*
            double pointerValue = val;
            double rangeValue = val;
            double scaleminValue = val;
            int count = this.VisualChildrenCount;
            for (int i = 0; i < count; i++)
            {
                Visual visual = this.GetVisualChild(i);
                if (visual is RangeBase)
                {
                    RangeBase range = visual as RangeBase;
                    if (coercedValue < range.EndValue)
                    {
                        // rangeValue = Math.Max(rangeValue, range.EndValue);                        
                        range.EndValue = val;
                    }
                }
                else if (visual is CircularPointer)
                {
                    CircularPointer pointer = visual as CircularPointer;
                    if (coercedValue < pointer.Value)
                    {
                        // pointerValue = Math.Max(pointerValue, pointer.Value);
                        pointer.Value = val;
                    }
                }
            }*/

            //if (val < this.Minimum)
            //{
            //    // scaleminValue = this.Minimum;
            //    // this.Minimum = val;
            //    val = this.Minimum; 
            //}

            return val;

            // return Math.Max(Math.Max(pointerValue, rangeValue), scaleminValue);
        }

        /// <summary>
        /// Coerces the value of the <see cref="Maximum"/> property.
        /// </summary>
        /// <param name="d">The <see cref="ScaleBase"/> object.</param>
        /// <param name="value">The instance containing the event data.</param>
        /// <returns>
        /// The checked value.
        /// </returns>
        public static object CoerceMaximum(DependencyObject d, object value)
        {
            ScaleBase owner = d as ScaleBase;
            return owner.CoerceMaximum(value);
        }

        /// <summary>
        /// Coerces the value of the <see cref="Minimum"/> property.
        /// </summary>
        /// <param name="d">The <see cref="ScaleBase"/> object.</param>
        /// <param name="value">The instance containing the event data.</param>
        /// <returns>
        /// The checked value.
        /// </returns>
        public static object CoerceMinimum(DependencyObject d, object value)
        {
            ScaleBase owner = d as ScaleBase;
            return owner.CoerceMinimum(value);
        }

        /// <summary>
        /// Fulfils the logic before setting the value of <see cref="Minimum"/> dependency property.
        /// </summary>
        /// <param name="value">The value that should be corrected.</param>
        /// <returns>The corrected value.</returns>
        protected internal virtual object CoerceMinimum(object value)
        {
            double val = (double)value;
            
            /*
            double pointerValue = val;
            double rangeValue = val;
            double scalemaxValue = val;
            int count = this.VisualChildrenCount;
            for (int i = 0; i < count; i++)
            {
                Visual visual = this.GetVisualChild(i);
                if (visual is RangeBase)
                {
                    RangeBase range = visual as RangeBase;
                    if (coercedValue > range.StartValue)
                    {
                        // rangeValue = Math.Min(rangeValue, range.StartValue);
                        range.StartValue = val;
                    }
                }
                else if (visual is CircularPointer)
                {
                    CircularPointer pointer = visual as CircularPointer;
                    if (coercedValue > pointer.Value)
                    {
                        // pointerValue = Math.Min(pointerValue, pointer.Value);
                        pointer.Value = val;
                    }
                }
            }*/

            //if (val > this.Maximum)
            //{
            //    // scalemaxValue = this.Maximum;
            //    val = this.Maximum;
            //}

            // return Math.Min(Math.Min(pointerValue, rangeValue), scalemaxValue);
            return val;
        }

        /// <summary>
        /// Coerces the value of the <see cref="ScaleBarSize"/> property.
        /// </summary>
        /// <param name="d">The <see cref="ScaleBase"/> object.</param>
        /// <param name="value">The instance containing the event data.</param>
        /// <returns>
        /// The checked value.
        /// </returns>
        public static object CoerceScaleBarSize(DependencyObject d, object value)
        {
            ScaleBase owner = d as ScaleBase;
            return owner.CoerceScaleBarSize(value);
        }

        /// <summary>
        /// Fulfils the logic before setting the value of <see cref="ScaleBarSize"/> dependency property.
        /// </summary>
        /// <param name="value">The value that should be corrected.</param>
        /// <returns>The corrected value.</returns>
        protected internal virtual object CoerceScaleBarSize(object value)
        {
            double val = (double)value;
            if (val < 0)
            {
                val = 0;
            }

            return val;
        }  

        /// <summary>
        /// Calls OnMajorIntervalValueChanged method of the instance, notifies of the dependency property value changes.
        /// </summary>
        /// <param name="d">Dependency object, the change occures on.</param>
        /// <param name="e">Property change details, such as old value and new value.</param>
        private static void OnMajorIntervalValueChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            ScaleBase instance = (ScaleBase)d;
            instance.OnMajorIntervalValueChanged(e);
        }

        /// <summary>
        /// Updates property value cache and raises <see cref="MajorIntervalValueChanged"/> event.
        /// </summary>
        /// <param name="e">
        /// Property change details, such as old value and new value.</param>
        protected virtual void OnMajorIntervalValueChanged(DependencyPropertyChangedEventArgs e)
        {
            int count = this.VisualChildrenCount;
            for (int i = 0; i < count; i++)
            {
                (this.GetVisualChild(i) as FrameworkElement).InvalidateVisual();
            }

            if (MajorIntervalValueChanged != null)
            {
                MajorIntervalValueChanged(this, e);
            }
        }

        /// <summary>
        /// Updates property value cache and raises <see cref="MaximumChanged"/> event.
        /// </summary>
        /// <param name="e">
        /// Property change details, such as old value and new value.</param>
        protected virtual void OnMaximumChanged(DependencyPropertyChangedEventArgs e)
        {
            int count = this.VisualChildrenCount;
            for (int i = 0; i < count; i++)
            {
                (this.GetVisualChild(i) as FrameworkElement).InvalidateVisual();
            }

            if (MaximumChanged != null)
            {
                MaximumChanged(this, e);
            }
        }

        /// <summary>
        /// Calls OnMaximumChanged method of the instance, notifies of the dependency property value changes.
        /// </summary>
        /// <param name="d">Dependency object, the change occures on.</param>
        /// <param name="e">Property change details, such as old value and new value.</param>
        private static void OnMaximumChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            ScaleBase instance = (ScaleBase)d;
            instance.OnMaximumChanged(e);
        }

        /// <summary>
        /// Calls OnMinimumChanged method of the instance, notifies of the dependency property value changes.
        /// </summary>
        /// <param name="d">Dependency object, the change occures on.</param>
        /// <param name="e">Property change details, such as old value and new value.</param>
        private static void OnMinimumChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            ScaleBase instance = (ScaleBase)d;
            instance.OnMinimumChanged(e);
        }

        /// <summary>
        /// Updates property value cache and raises <see cref="MinimumChanged"/> event.
        /// </summary>
        /// <param name="e">
        /// Property change details, such as old value and new value.</param>
        protected virtual void OnMinimumChanged(DependencyPropertyChangedEventArgs e)
        {
            int count = this.VisualChildrenCount;
            for (int i = 0; i < count; i++)
            {
                (this.GetVisualChild(i) as FrameworkElement).InvalidateVisual();
            }

            if (MinimumChanged != null)
            {
                MinimumChanged(this, e);
            }
        }

        /// <summary>
        /// Calls OnMinorIntervalValueChanged method of the instance, notifies of the dependency property value changes.
        /// </summary>
        /// <param name="d">Dependency object, the change occures on.</param>
        /// <param name="e">Property change details, such as old value and new value.</param>
        private static void OnMinorIntervalValueChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            ScaleBase instance = (ScaleBase)d;
            instance.OnMinorIntervalValueChanged(e);
        }

        /// <summary>
        /// Calls OnIntervalValueChanged method of the instance, notifies of the dependency property value changes.
        /// </summary>
        /// <param name="d">Dependency object, the change occures on.</param>
        /// <param name="e">Property change details, such as old value and new value.</param>
        private static void OnIntervalValueChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            ScaleBase instance = (ScaleBase)d;
            //instance.OnMinorIntervalValueChanged(e);
            instance.OnMajorIntervalValueChanged(e);
        }

        /// <summary>
        /// Calls OnNumberDivisionValueChanged method of the instance, notifies of the dependency property value changes.
        /// </summary>
        /// <param name="d">Dependency object, the change occures on.</param>
        /// <param name="e">Property change details, such as old value and new value.</param>
        private static void OnNumberDivisionValueChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            ScaleBase instance = (ScaleBase)d;
            instance.InvalidateVisual();
        }

        /// <summary>
        /// Updates property value cache and raises <see cref="MinorIntervalValueChanged"/> event.
        /// </summary>
        /// <param name="e">
        /// Property change details, such as old value and new value.</param>
        protected virtual void OnMinorIntervalValueChanged(DependencyPropertyChangedEventArgs e)
        {
            int count = this.VisualChildrenCount;
            for (int i = 0; i < count; i++)
            {
                (this.GetVisualChild(i) as FrameworkElement).InvalidateVisual();
            }

            if (MinorIntervalValueChanged != null)
            {
                MinorIntervalValueChanged(this, e);
            }
        }

        /// <summary>
        /// Calls OnScaleBarSizeChanged method of the instance, notifies of the dependency property value changes.
        /// </summary>
        /// <param name="d">Dependency object, the change occures on.</param>
        /// <param name="e">Property change details, such as old value and new value.</param>
        private static void OnScaleBarSizeChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            ScaleBase instance = (ScaleBase)d;
            instance.OnScaleBarSizeChanged(e);
        }

        /// <summary>
        /// Calls OnScaleBarSizeChanged method of the instance, notifies of the dependency property value changes.
        /// </summary>
        /// <param name="d">Dependency object, the change occures on.</param>
        /// <param name="e">Property change details, such as old value and new value.</param>
        private static void OnScaleDirectionChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            ScaleBase instance = (ScaleBase)d;
            instance.OnScaleDirectionChanged(e);
        }

        /// <summary>
        /// Updates property value cache and raises <see cref="ScaleBarSizeChanged"/> event.
        /// </summary>
        /// <param name="e">
        /// Property change details, such as old value and new value.</param>
        protected virtual void OnScaleBarSizeChanged(DependencyPropertyChangedEventArgs e)
        {
            int count = this.VisualChildrenCount;
            for (int i = 0; i < count; i++)
            {
                (this.GetVisualChild(i) as FrameworkElement).InvalidateVisual();
            }

            if (ScaleBarSizeChanged != null)
            {
                ScaleBarSizeChanged(this, e);
            }
        }


        /// <summary>
        /// Updates property value cache and raises <see cref="ScaleBarSizeChanged"/> event.
        /// </summary>
        /// <param name="e">
        /// Property change details, such as old value and new value.</param>
        protected virtual void OnScaleDirectionChanged(DependencyPropertyChangedEventArgs e)
        {            
        
            if (ScaleDirectionChanged != null)
            {
                ScaleDirectionChanged(this, e);
            }
        }

        /// <summary>
        /// Updates property value cache and raises <see cref="ShadowOffsetChanged"/> event.
        /// </summary>
        /// <param name="e">
        /// Property change details, such as old value and new value.</param>
        protected virtual void OnShadowOffsetChanged(DependencyPropertyChangedEventArgs e)
        {
            if (ShadowOffsetChanged != null)
            {
                ShadowOffsetChanged(this, e);
            }
        }

        /// <summary>
        /// Calls OnShadowOffsetChanged method of the instance, notifies of the dependency property value changes.
        /// </summary>
        /// <param name="d">Dependency object, the change occures on.</param>
        /// <param name="e">Property change details, such as old value and new value.</param>
        private static void OnShadowOffsetChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            ScaleBase instance = (ScaleBase)d;
            instance.OnShadowOffsetChanged(e);
        }
        #endregion Implementation
    }
}