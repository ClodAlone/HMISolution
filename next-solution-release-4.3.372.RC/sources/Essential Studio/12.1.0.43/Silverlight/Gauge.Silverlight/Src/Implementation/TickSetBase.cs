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
    /// Represents base class for tick.
    /// </summary>
    public abstract class TickSetBase : GaugeElement
    {      
        #region Dependency properties
        
        /// <summary>
        /// Identifies the <see cref="Angle"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty AngleProperty =
            DependencyProperty.Register("Angle", typeof(double), typeof(TickSetBase), new PropertyMetadata(0d, new PropertyChangedCallback(OnAngleChanged)));

        /// <summary>
        /// Identifies the <see cref="DistanceFromScale"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty DistanceFromScaleProperty =
            DependencyProperty.Register("DistanceFromScale", typeof(double), typeof(TickSetBase), new PropertyMetadata(0d, new PropertyChangedCallback(OnDistanceFromScaleChanged)));

        /// <summary>
        /// Identifies <see cref="P:Syncfusion.Windows.Gauge.LabelTickSet.ShowToolTip">ShowToolTip</see> Dependency property
        /// </summary>
        /// <returns>
        /// Type:<see cref="T:System.Boolean">System.Boolean</see>
        /// </returns>
        public static readonly DependencyProperty ShowToolTipProperty = DependencyProperty.Register("ShowToolTip", typeof(bool), typeof(TickSetBase), new PropertyMetadata(false, new PropertyChangedCallback(OnShowToolTipChanged)));

        /// <summary>
        /// Identifies the <see cref="P:Syncfusion.Windows.Gauge.LabelTickSet.LabelFormula">LabelFormula</see> Dependency property.
        /// </summary>
        /// <returns>
        /// Type:<see cref="T:System.String">System.String</see>
        /// </returns>
        public static readonly DependencyProperty TooltipTextProperty = DependencyProperty.Register("TooltipText", typeof(string), typeof(TickSetBase), new PropertyMetadata("{0}", new PropertyChangedCallback(OnTooltipTextChanged)));

        /// <summary>
        /// Identifies the <see cref="TickPlacement"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty TickPlacementProperty =
            DependencyProperty.Register("TickPlacement", typeof(ScalePlacement), typeof(TickSetBase), new PropertyMetadata(ScalePlacement.Cross, new PropertyChangedCallback(OnTickPlacementChanged)));

        /// <summary>
        /// Identifies the <see cref="TickStyle"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty TickStyleProperty =
            DependencyProperty.Register("TickStyle", typeof(TickStyle), typeof(TickSetBase), new PropertyMetadata(TickStyle.MajorTick, new PropertyChangedCallback(OnTickStyleChanged)));

        #endregion
        
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
        /// Event that is raised when ShowToolTip dependency.
        /// </summary>
        public event PropertyChangedCallback ShowToolTipChanged;
        
        /// <summary>
        /// Event that is raised when <see cref="P:Syncfusion.Windows.Gauge.LabelTickSet.TooltipText">TooltipText</see> property is changed.
        /// </summary>
        public event PropertyChangedCallback TooltipTextChanged;

        /// <summary>
        /// Event that is raised when <see cref="TickStyle"/> property is changed.
        /// </summary>
        public event PropertyChangedCallback TickStyleChanged;

        #endregion

        #region DP getters & setters
        
        /// <summary>
        /// Gets or sets the rotation angle of the tick.
        /// This is a dependency property.
        /// </summary>
        /// <value>
        /// Type: <see cref="double"/>
        /// Default value is 0.
        /// </value>
        /// <seealso cref="double"/>
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
        /// Gets or sets distance between the tick and the scale.
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
        /// Gets or sets the placement of the tick relatively to the scale.
        /// This is a dependency property.
        /// </summary>
        /// <value>
        /// Type: <see cref="ScalePlacement"/>
        /// Default value is ScalePlacement.Cross.
        /// </value>
        /// <seealso cref="ScalePlacement"/>
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
        /// Gets or sets the TooltipText property.This is a dependencyproperty.
        /// </summary>
        /// <remarks>
        /// <b>Xaml</b>
        /// <para></para>
        /// <para>&lt;Syncfusion:LabelTickSet Name=&quot;labeltickset&quot; TooltipText=&quot;The Tick value is (0}&quot; /&gt;</para>
        /// <para></para>
        /// <para><b>C#</b></para>
        /// <para></para>
        /// <para>LabelTickSet labeltickset = new LabelTickSet();</para>
        /// <para>LabelTickSet.LabelFormula = &quot;(x+10)&quot;;</para>
        /// </remarks>
        /// <value>
        /// Type : <see cref="T:System.String">System.String</see>
        /// </value>
        public string TooltipText
        {
            get
            {
                return (string)GetValue(TooltipTextProperty);
            }

            set
            {
                SetValue(TooltipTextProperty, value);
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether .
        /// </summary>
        /// <remarks>
        /// <b>Xaml</b>
        /// <para></para>
        /// <para>&lt;Syncfusion:LabelTickSet Name=&quot;labeltickset&quot; ShowToolTip=&quot;true&quot; /&gt;</para>
        /// <para></para>
        /// <para><b>C#</b></para>
        /// <para></para>
        /// <para>LabelTickSet labeltickset = new LabelTickSet();</para>
        /// <para>LabelTickSet.ShowToolTip =&quot;true&quot;;</para>
        /// </remarks>
        public bool ShowToolTip
        {
            get
            {
                return (bool)GetValue(ShowToolTipProperty);
            }

            set
            {
                SetValue(ShowToolTipProperty, value);
            }
        }

        /// <summary>
        /// Gets or sets the styles of the tick.
        /// This is a dependency property.
        /// </summary>
        /// <value>
        /// Type: <see cref="TickStyle"/>
        /// Default value is TickStyle.MajorTick.
        /// </value>
        /// <seealso cref="TickStyle"/>
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

        #endregion

        #region Implementation
        
        /// <summary>
        /// Updates property value cache and raises <see cref="AngleChanged"/> event.
        /// </summary>
        /// <param name="e">
        /// Property change details, such as old value and new value.</param>
        protected virtual void OnAngleChanged(DependencyPropertyChangedEventArgs e)
        {
            if (this.AngleChanged != null)
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
            if (this.DistanceFromScaleChanged != null)
            {
                this.DistanceFromScaleChanged(this, e);
            }
        }

        /// <summary>
        /// Updates property value cache and raises <see cref="TickPlacementChanged"/> event.
        /// </summary>
        /// <param name="e">
        /// Property change details, such as old value and new value.</param>
        protected virtual void OnTickPlacementChanged(DependencyPropertyChangedEventArgs e)
        {
            if (this.TickPlacementChanged != null)
            {
                this.TickPlacementChanged(this, e);
            }
        }

        /// <summary>
        /// Updates property value cache and raises <see cref="TooltipTextChanged"/> event.
        /// </summary>
        /// <param name="e">
        /// Property change details, such as old value and new value.</param>
        protected virtual void OnTooltipTextChanged(DependencyPropertyChangedEventArgs e)
        {
            if (this.TooltipTextChanged != null)
            {
                this.TooltipTextChanged(this, e);
            }
        }

        /// <summary>
        /// Updates property value cache and raises <see cref="TickStyleChanged"/> event.
        /// </summary>
        /// <param name="e">
        /// Property change details, such as old value and new value.</param>
        protected virtual void OnTickStyleChanged(DependencyPropertyChangedEventArgs e)
        {
            if (this.TickStyleChanged != null)
            {
                this.TickStyleChanged(this, e);
            }
        }

        /// <summary>
        /// Updates property value cache and raises <see cref="ShowToolTipChanged"/> event.
        /// </summary>
        /// <param name="e">
        /// Property change details, such as old value and new value.</param>
        protected virtual void OnShowToolTipChanged(DependencyPropertyChangedEventArgs e)
        {
            if (this.ShowToolTipChanged != null)
            {
                this.ShowToolTipChanged(this, e);
            }           
        }

        /// <summary>
        /// Calls OnAngleChanged method of the instance, notifies of the depencency property value changes.
        /// </summary>
        /// <param name="d">Dependency object, the change occures on.</param>
        /// <param name="e">Property change details, such as old value and new value.</param>
        private static void OnAngleChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            TickSetBase instance = (TickSetBase)d;
            instance.OnAngleChanged(e);
        }

        /// <summary>
        /// Calls OnDistanceFromScaleChanged method of the instance, notifies of the depencency property value changes.
        /// </summary>
        /// <param name="d">Dependency object, the change occures on.</param>
        /// <param name="e">Property change details, such as old value and new value.</param>
        private static void OnDistanceFromScaleChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            TickSetBase instance = (TickSetBase)d;
            instance.OnDistanceFromScaleChanged(e);
        }

        /// <summary>
        /// Measures Size
        /// </summary>
        /// <param name="availableSize">available size</param>
        /// <returns>Returns the size</returns>
        protected override Size MeasureOverride(Size availableSize)
        {
            Size size = base.MeasureOverride(availableSize);
            
            return size;
        }

        /// <summary>
        /// Calls OnShowToolTipChanged method of the instance, notifies of the depencency property value changes.
        /// </summary>
        /// <param name="d">Dependency object, the change occures on.</param>
        /// <param name="e">Property change details, such as old value and new value.</param>
        private static void OnShowToolTipChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            TickSetBase instance = (TickSetBase)d;
            instance.OnShowToolTipChanged(e);
        }

        /// <summary>
        /// Calls OnTickPlacementChanged method of the instance, notifies of the depencency property value changes.
        /// </summary>
        /// <param name="d">Dependency object, the change occures on.</param>
        /// <param name="e">Property change details, such as old value and new value.</param>
        private static void OnTickPlacementChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            TickSetBase instance = (TickSetBase)d;
            instance.OnTickPlacementChanged(e);
        }

        /// <summary>
        /// Calls OnTickStyleChanged method of the instance, notifies of the depencency property value changes.
        /// </summary>
        /// <param name="d">Dependency object, the change occures on.</param>
        /// <param name="e">Property change details, such as old value and new value.</param>
        private static void OnTickStyleChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            TickSetBase instance = (TickSetBase)d;
            instance.OnTickStyleChanged(e);
        }

        /// <summary>
        /// Calls OnTooltipTextChanged method of the instance, notifies of the depencency property value changes.
        /// </summary>
        /// <param name="d">Dependency object, the change occures on.</param>
        /// <param name="e">Property change details, such as old value and new value.</param>
        private static void OnTooltipTextChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            TickSetBase instance = (TickSetBase)d;
            instance.OnTooltipTextChanged(e);
        }

        #endregion
    }
}
