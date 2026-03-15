// <copyright file="TickBase.cs" company="Syncfusion Software">
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
using System.Windows.Media;

namespace Syncfusion.Windows.Gauge
{
    /// <summary>
    /// Represents the abstract base class for Tick elements used in the Gauge control.
    /// </summary>
#if SyncfusionFramework4_0
    [System.ComponentModel.DesignTimeVisible(false)]
#endif
    public abstract class TickBase : GaugeElement
    {
        #region Events
        /// <summary>
        /// Event that is raised when <see cref="Angle"/> property is changed.
        /// </summary>
        public event PropertyChangedCallback AngleChanged;

        /// <summary>
        /// Event that is raised when <see cref="DistanceFromScale"/> property is changed.
        /// </summary>
        public event PropertyChangedCallback DistanceFromScaleChanged;

        /// <summary>
        /// Event that is raised when <see cref="TickPlacement"/> property is changed.
        /// </summary>
        public event PropertyChangedCallback TickPlacementChanged;

        /// <summary>
        /// Event that is raised when <see cref="TickStyle"/> property is changed.
        /// </summary>
        public event PropertyChangedCallback TickStyleChanged;
        #endregion Events

        #region Dependency Properties
        /// <summary>
        /// Identifies the <see cref="Angle"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty AngleProperty =
            DependencyProperty.Register("Angle", typeof(double), typeof(TickBase), new FrameworkPropertyMetadata(0d, FrameworkPropertyMetadataOptions.AffectsRender, new PropertyChangedCallback(OnAngleChanged)));

        /// <summary>
        /// Identifies the <see cref="DistanceFromScale"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty DistanceFromScaleProperty =
            DependencyProperty.Register("DistanceFromScale", typeof(double), typeof(TickBase), new FrameworkPropertyMetadata(0d, FrameworkPropertyMetadataOptions.AffectsArrange | FrameworkPropertyMetadataOptions.AffectsRender, new PropertyChangedCallback(OnDistanceFromScaleChanged)));

        /// <summary>
        /// Identifies the <see cref="TickPlacement"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty TickPlacementProperty =
            DependencyProperty.Register("TickPlacement", typeof(ScalePlacement), typeof(TickBase), new FrameworkPropertyMetadata(ScalePlacement.Cross, FrameworkPropertyMetadataOptions.AffectsRender | FrameworkPropertyMetadataOptions.AffectsParentMeasure | FrameworkPropertyMetadataOptions.AffectsParentArrange, new PropertyChangedCallback(OnTickPlacementChanged)));

        /// <summary>
        /// Identifies the <see cref="TickStyle"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty TickStyleProperty =
            DependencyProperty.Register("TickStyle", typeof(TickStyle), typeof(TickBase), new FrameworkPropertyMetadata(TickStyle.MajorTick, new PropertyChangedCallback(OnTickStyleChanged)));

        /// <summary>
        /// Identifies the <see cref="YDistanceFromScale"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty YDistanceFromScaleProperty =
            DependencyProperty.Register("YDistanceFromScale", typeof(double), typeof(TickBase), new FrameworkPropertyMetadata(0d, FrameworkPropertyMetadataOptions.AffectsArrange | FrameworkPropertyMetadataOptions.AffectsRender));

        /// <summary>
        /// Identifies the <see cref="RangedBrush"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty RangedBrushProperty =
            DependencyProperty.Register("RangedBrush", typeof(Brush), typeof(TickBase), new FrameworkPropertyMetadata(null, FrameworkPropertyMetadataOptions.AffectsRender));

        /// <summary>
        /// Identifies the <see cref="RangedBrushStartValue"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty RangedBrushStartValueProperty =
            DependencyProperty.Register("RangedBrushStartValue", typeof(double), typeof(TickBase), new FrameworkPropertyMetadata(0d, FrameworkPropertyMetadataOptions.AffectsRender));

        /// <summary>
        /// Identifies the <see cref="RangedBrushEndValue"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty RangedBrushEndValueProperty =
            DependencyProperty.Register("RangedBrushEndValue", typeof(double), typeof(TickBase), new FrameworkPropertyMetadata(0d, FrameworkPropertyMetadataOptions.AffectsRender));
        #endregion Dependency Properties

        #region DP Getters & Setters
        /// <summary>
        /// Gets or sets the rotation angle of the tick.
        /// This is a dependency property.
        /// </summary>
        /// <value>
        /// Type: <see cref="double"/>
        /// Default value is 0.
        /// </value>
        public double Angle
        {
            get
            {
                return (double)GetValue(AngleProperty);
            }

            set
            {
                SetValue(AngleProperty, value);
            }
        }

        /// <summary>
        /// Gets or sets the distance between the tick and the scale.
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
        /// Gets or sets the placement of tick, relative to the scale.
        /// This is a dependency property.
        /// </summary>
        /// <value>
        /// Type: <see cref="ScalePlacement"/>
        /// Default value is ScalePlacement.Cross.
        /// </value>
        public ScalePlacement TickPlacement
        {
            get
            {
                return (ScalePlacement)GetValue(TickPlacementProperty);
            }

            set
            {
                SetValue(TickPlacementProperty, value);
            }
        }

        /// <summary>
        /// Gets or sets the style of the tick.
        /// This is a dependency property.
        /// </summary>
        /// <value>
        /// Type: <see cref="TickStyle"/>
        /// Default value is TickStyle.MajorTick.
        /// </value>
        public TickStyle TickStyle
        {
            get
            {
                return (TickStyle)GetValue(TickStyleProperty);
            }

            set
            {
                SetValue(TickStyleProperty, value);
            }
        }

        /// <summary>
        /// Gets or sets the Y axis distance between the tick and the scale bottom.
        /// This is a dependency property.
        /// </summary>
        /// <value>
        /// Type: <see cref="double"/>
        /// Default value is 0.
        /// </value>
        public double YDistanceFromScale
        {
            get
            {
                return (double)GetValue(YDistanceFromScaleProperty);
            }

            set
            {
                SetValue(YDistanceFromScaleProperty, value);
            }
        }

        /// <summary>
        /// Gets or sets the Y axis distance between the tick and the scale bottom.
        /// This is a dependency property.
        /// </summary>
        /// <value>
        /// Type: <see cref="Brush"/>
        /// Default value is null.
        /// </value>
        public Brush RangedBrush
        {
            get
            {
                return (Brush)GetValue(RangedBrushProperty);
            }

            set
            {
                SetValue(RangedBrushProperty, value);
            }
        }

        /// <summary>
        /// Gets or sets the start value of the ticks from which the <see cref="RangedBrush"/> should be used.
        /// This is a dependency property.
        /// </summary>
        /// <value>
        /// Type: <see cref="double"/>
        /// Default value is 0.
        /// </value>
        public double RangedBrushStartValue
        {
            get
            {
                return (double)GetValue(RangedBrushStartValueProperty);
            }

            set
            {
                SetValue(RangedBrushStartValueProperty, value);
            }
        }

        /// <summary>
        /// Gets or sets the end value of the ticks from which the <see cref="RangedBrush"/> should be used.
        /// This is a dependency property.
        /// </summary>
        /// <value>
        /// Type: <see cref="double"/>
        /// Default value is 0.
        /// </value>
        public double RangedBrushEndValue
        {
            get
            {
                return (double)GetValue(RangedBrushEndValueProperty);
            }

            set
            {
                SetValue(RangedBrushEndValueProperty, value);
            }
        }
        #endregion DP Getters & Setters

        #region Implementation
        /// <summary>
        /// Calls OnAngleChanged method of the instance, notifies of the depencency property value changes.
        /// </summary>
        /// <param name="d">Dependency object, the change occures on.</param>
        /// <param name="e">Property change details, such as old value and new value.</param>
        private static void OnAngleChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            TickBase instance = (TickBase)d;
            instance.OnAngleChanged(e);
        }

        /// <summary>
        /// Updates property value cache and raises <see cref="AngleChanged"/> event.
        /// </summary>
        /// <param name="e">
        /// Property change details, such as old value and new value.</param>
        protected virtual void OnAngleChanged(DependencyPropertyChangedEventArgs e)
        {
            if (AngleChanged != null)
            {
                this.AngleChanged(this, e);
            }
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
        /// Calls OnDistanceFromScaleChanged method of the instance, notifies of the depencency property value changes.
        /// </summary>
        /// <param name="d">Dependency object, the change occures on.</param>
        /// <param name="e">Property change details, such as old value and new value.</param>
        private static void OnDistanceFromScaleChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            TickBase instance = (TickBase)d;
            instance.OnDistanceFromScaleChanged(e);
        }

        /// <summary>
        /// Updates property value cache and raises <see cref="TickPlacementChanged"/> event.
        /// </summary>
        /// <param name="e">
        /// Property change details, such as old value and new value.</param>
        protected virtual void OnTickPlacementChanged(DependencyPropertyChangedEventArgs e)
        {
            if (TickPlacementChanged != null)
            {
                this.TickPlacementChanged(this, e);
            }
        }

        /// <summary>
        /// Calls OnTickPlacementChanged method of the instance, notifies of the depencency property value changes.
        /// </summary>
        /// <param name="d">Dependency object, the change occures on.</param>
        /// <param name="e">Property change details, such as old value and new value.</param>
        private static void OnTickPlacementChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            TickBase instance = (TickBase)d;
            instance.OnTickPlacementChanged(e);
        }

        /// <summary>
        /// Calls OnTickStyleChanged method of the instance, notifies of the depencency property value changes.
        /// </summary>
        /// <param name="d">Dependency object, the change occures on.</param>
        /// <param name="e">Property change details, such as old value and new value.</param>
        private static void OnTickStyleChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            TickBase instance = (TickBase)d;
            instance.OnTickStyleChanged(e);
        }

        /// <summary>
        /// Updates property value cache and raises <see cref="TickStyleChanged"/> event.
        /// </summary>
        /// <param name="e">
        /// Property change details, such as old value and new value.</param>
        protected virtual void OnTickStyleChanged(DependencyPropertyChangedEventArgs e)
        {
            if (TickStyleChanged != null)
            {
                this.TickStyleChanged(this, e);
            }
        }
        #endregion Implementation
    }
}
