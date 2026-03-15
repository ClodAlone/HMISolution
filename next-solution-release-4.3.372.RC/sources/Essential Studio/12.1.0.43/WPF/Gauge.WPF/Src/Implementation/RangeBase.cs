// <copyright file="RangeBase.cs" company="Syncfusion Software">
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

namespace Syncfusion.Windows.Gauge
{
    /// <summary>
    /// Represents the abstract base class for the Ranges used in Gauge.
    /// </summary>
#if SyncfusionFramework4_0
    [System.ComponentModel.DesignTimeVisible(false)]
#endif
    public abstract class RangeBase : GaugeElement
    {
        #region Dependency Properties
        /// <summary>
        /// Identifies the <see cref="DistanceFromScale"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty DistanceFromScaleProperty =
            DependencyProperty.Register("DistanceFromScale", typeof(double), typeof(RangeBase), new FrameworkPropertyMetadata(0d, FrameworkPropertyMetadataOptions.AffectsArrange | FrameworkPropertyMetadataOptions.AffectsRender, new PropertyChangedCallback(OnDistanceFromScaleChanged)));

        /// <summary>
        /// Identifies the <see cref="EndValue"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty EndValueProperty =
            DependencyProperty.Register("EndValue", typeof(double), typeof(RangeBase), new FrameworkPropertyMetadata(0d, FrameworkPropertyMetadataOptions.AffectsArrange | FrameworkPropertyMetadataOptions.AffectsRender, new PropertyChangedCallback(OnEndValueChanged), CoerceEndValue));

        /// <summary>
        /// Identifies the <see cref="EndWidth"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty EndWidthProperty =
            DependencyProperty.Register("EndWidth", typeof(double), typeof(RangeBase), new FrameworkPropertyMetadata(0d, FrameworkPropertyMetadataOptions.AffectsArrange | FrameworkPropertyMetadataOptions.AffectsRender, new PropertyChangedCallback(OnEndWidthChanged)));

        /// <summary>
        /// Identifies the <see cref="RangePosition"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty RangePositionProperty =
            DependencyProperty.Register("RangePosition", typeof(ScalePlacement), typeof(RangeBase), new FrameworkPropertyMetadata(ScalePlacement.Inside, FrameworkPropertyMetadataOptions.AffectsArrange | FrameworkPropertyMetadataOptions.AffectsRender, new PropertyChangedCallback(OnRangePositionChanged)));

        /// <summary>
        /// Identifies the <see cref="StartValue"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty StartValueProperty =
            DependencyProperty.Register("StartValue", typeof(double), typeof(RangeBase), new FrameworkPropertyMetadata(0d, FrameworkPropertyMetadataOptions.AffectsArrange | FrameworkPropertyMetadataOptions.AffectsRender, new PropertyChangedCallback(OnStartValueChanged), CoerceStartValue));

        /// <summary>
        /// Identifies the <see cref="StartWidth"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty StartWidthProperty =
            DependencyProperty.Register("StartWidth", typeof(double), typeof(RangeBase), new FrameworkPropertyMetadata(0d, FrameworkPropertyMetadataOptions.AffectsArrange | FrameworkPropertyMetadataOptions.AffectsRender, new PropertyChangedCallback(OnStartWidthChanged)));

        /// <summary>
        /// Identifies the <see cref="BindIndicator"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty BindIndicatorProperty =
            DependencyProperty.Register("BindIndicator", typeof(Boolean), typeof(RangeBase), new FrameworkPropertyMetadata(false, FrameworkPropertyMetadataOptions.AffectsArrange | FrameworkPropertyMetadataOptions.AffectsRender, new PropertyChangedCallback(OnBindIndicatorChanged)));

        /// <summary>
        /// Identifies the <see cref="BindIndicator"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty IndicatorColorProperty =
            DependencyProperty.Register("IndicatorColor", typeof(Brush), typeof(RangeBase), new FrameworkPropertyMetadata(null, FrameworkPropertyMetadataOptions.AffectsArrange | FrameworkPropertyMetadataOptions.AffectsRender, new PropertyChangedCallback(OnIndicatorColorChanged)));

       #endregion Dependency Properties

        #region Events
        /// <summary>
        /// Event that is raised when <see cref="DistanceFromScale"/> property is changed.
        /// </summary>
        public event PropertyChangedCallback DistanceFromScaleChanged;

        /// <summary>
        /// Event that is raised when <see cref="EndValue"/> property is changed.
        /// </summary>
        public event PropertyChangedCallback EndValueChanged;

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
        public event PropertyChangedCallback StartValueChanged;

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
        #endregion Events

        #region DP Getters & Setters
        /// <summary>
        /// Gets or sets the distance between the range and the scale.
        /// This is a dependency property.
        /// </summary>
        /// <value>
        /// Type: <see cref="double"/>
        /// Default value is 0.
        /// </value>
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
        /// <remarks>
        /// The Range's EndValue should be specified before the <see cref="StartValue"/>, because of 
        /// the constraint, that <see cref="StartValue"/> should be lesser than the EndValue
        /// </remarks>
        /// <seealso cref="StartValue"/>
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
        /// <seealso cref="StartWidth"/>
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
        /// <remarks>
        /// The Range's StartValue should be specified after the <see cref="EndValue"/>, because of 
        /// the constraint, that <see cref="EndValue"/> should be greater than the StartValue
        /// </remarks>
        /// <seealso cref="EndValue"/>
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
        /// <seealso cref="EndWidth"/>
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
        #endregion DP Getters & Setters

        #region Implementation
        /// <summary>
        /// Fulfils the logic before setting the value of <see cref="EndValue"/> dependency property.
        /// </summary>
        /// <param name="value">The value that should be corrected.</param>
        /// <returns>The corrected value.</returns>
        protected internal virtual object CoerceEndValue(object value)
        {
            double val = (double)value;
            ScaleBase scale = this.VisualParent as ScaleBase;
            if (scale != null)
            {
                if (val > scale.Maximum)
                {
                    return scale.Maximum;

                    // val = scale.Maximum;
                }

                if (val < scale.Minimum)
                {
                    return scale.Minimum;

                    // val = scale.Maximum;
                }
            }            

            return val;
        }

        /// <summary>
        /// Coerces the value of the <see cref="EndValue"/> property.
        /// </summary>
        /// <param name="d">The <see cref="RangeBase"/> object.</param>
        /// <param name="value">The instance containing the event data.</param>
        /// <returns>
        /// The checked value.
        /// </returns>
        public static object CoerceEndValue(DependencyObject d, object value)
        {
            RangeBase owner = d as RangeBase;
            return owner.CoerceEndValue(value);
        }

        /// <summary>
        /// Coerces the value of the <see cref="StartValue"/> property.
        /// </summary>
        /// <param name="d">The <see cref="RangeBase"/> object.</param>
        /// <param name="value">The instance containing the event data.</param>
        /// <returns>
        /// The checked value.
        /// </returns>
        public static object CoerceStartValue(DependencyObject d, object value)
        {
            RangeBase owner = d as RangeBase;
            return owner.CoerceStartValue(value);
        }

        /// <summary>
        /// Fulfils the logic before setting the value of <see cref="StartValue"/> dependency property.
        /// </summary>
        /// <param name="value">The value that should be corrected.</param>
        /// <returns>The corrected value.</returns>
        protected internal virtual object CoerceStartValue(object value)
        {
            double val = (double)value;
            ScaleBase scale = this.VisualParent as ScaleBase;
            if (scale != null)
            {
                if (val < scale.Minimum)
                {
                    return scale.Minimum;

                    // val = scale.Minimum;
                }

                if (val > scale.Maximum)
                {
                    return scale.Maximum;
                }
            }

            return val;
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
        /// Updates property value cache and raises <see cref="DistanceFromScaleChanged"/> event.
        /// </summary>
        /// <param name="e">
        /// Property change details, such as old value and new value.</param>
        protected virtual void OnDistanceFromScaleChanged(DependencyPropertyChangedEventArgs e)
        {
            if (DistanceFromScaleChanged != null)
            {
                this.DistanceFromScaleChanged(this, e);
            }
        }

        /// <summary>
        /// Updates property value cache and raises <see cref="EndValueChanged"/> event.
        /// </summary>
        /// <param name="e">
        /// Property change details, such as old value and new value.</param>
        protected virtual void OnEndValueChanged(DependencyPropertyChangedEventArgs e)
        {
            if (EndValueChanged != null)
            {
                this.EndValueChanged(this, e);
            }
        }

        /// <summary>
        /// Calls OnEndValueChanged method of the instance, notifies of the depencency property value changes.
        /// </summary>
        /// <param name="d">Dependency object, the change occures on.</param>
        /// <param name="e">Property change details, such as old value and new value.</param>
        private static void OnEndValueChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            RangeBase instance = (RangeBase)d;
            instance.OnEndValueChanged(e);
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
        /// Updates property value cache and raises <see cref="EndWidthChanged"/> event.
        /// </summary>
        /// <param name="e">
        /// Property change details, such as old value and new value.</param>
        protected virtual void OnEndWidthChanged(DependencyPropertyChangedEventArgs e)
        {
            if (EndWidthChanged != null)
            {
                this.EndWidthChanged(this, e);
            }
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
        /// Updates property value cache and raises <see cref="RangePositionChanged"/> event.
        /// </summary>
        /// <param name="e">
        /// Property change details, such as old value and new value.</param>
        protected virtual void OnRangePositionChanged(DependencyPropertyChangedEventArgs e)
        {
            if (RangePositionChanged != null)
            {
                this.RangePositionChanged(this, e);
            }
        }

        /// <summary>
        /// Updates property value cache and raises <see cref="StartValueChanged"/> event.
        /// </summary>
        /// <param name="e">
        /// Property change details, such as old value and new value.</param>
        protected virtual void OnStartValueChanged(DependencyPropertyChangedEventArgs e)
        {
            if (StartValueChanged != null)
            {
                this.StartValueChanged(this, e);
            }
        }

        /// <summary>
        /// Calls OnStartValueChanged method of the instance, notifies of the depencency property value changes.
        /// </summary>
        /// <param name="d">Dependency object, the change occures on.</param>
        /// <param name="e">Property change details, such as old value and new value.</param>
        private static void OnStartValueChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            RangeBase instance = (RangeBase)d;
            instance.OnStartValueChanged(e);
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
        /// Updates property value cache and raises <see cref="StartWidthChanged"/> event.
        /// </summary>
        /// <param name="e">
        /// Property change details, such as old value and new value.</param>
        protected virtual void OnStartWidthChanged(DependencyPropertyChangedEventArgs e)
        {
            if (StartWidthChanged != null)
            {
                this.StartWidthChanged(this, e);
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
        #endregion Implementation
    }
}
