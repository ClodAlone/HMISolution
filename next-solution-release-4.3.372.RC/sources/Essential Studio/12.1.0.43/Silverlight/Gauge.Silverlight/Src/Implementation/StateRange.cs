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
    /// Represents state range object.
    /// </summary>
    /// <example>
    /// <list type="table">
    /// <listheader>
    /// <term>C# :</term></listheader>
    /// <item>
    /// <description>using System.Silverlight.Gauge;
    /// <para></para>
    /// <para>stateindicator.StateRanges.Add(new StateRange(70, 100));</para>
    /// <para>            </para></description></item></list>
    /// <para></para>
    /// <list type="table">
    /// <listheader>
    /// <term>Xaml :</term></listheader>
    /// <item>
    /// <description>
    /// <para>xmlns:syncfusion=&quot;clr-namespace:Syncfusion.Windows.Gauge;assembly=Syncfusion.Gauge.Silverlight&quot;</para>
    /// <para></para>
    /// <para> &lt;syncfusion:StateIndicator.StateRanges&gt;</para>
    /// <para>                        &lt;syncfusion:StateRange StartValue=&quot;70&quot; EndValue=&quot;100&quot;/&gt;</para>
    /// <para>                    &lt;/syncfusion:StateIndicator.StateRanges&gt;</para>
    /// <para></para>
    /// <para>    </para></description></item></list>
    /// </example>
    public class StateRange : Control
    {
        #region Dependency properties

        /// <summary>
        /// Identifies the <see cref="P:Syncfusion.Windows.Gauge.StateRange.EndValue">EndValue</see> dependency property.
        /// </summary>
        /// <returns>
        /// Type : <see cref="T:System.Double">System.Double</see>
        /// </returns>
        public static readonly DependencyProperty EndValueProperty =
            DependencyProperty.Register("EndValue", typeof(double), typeof(StateRange), new PropertyMetadata(0d, new PropertyChangedCallback(OnEndValueChanged)));

        /// <summary>
        /// Identifies the <see cref="P:Syncfusion.Windows.Gauge.StateRange.StartValue">StartValue</see> dependency property.
        /// </summary>
        /// <returns>
        /// Type : <see cref="T:System.Double">System.Double</see>
        /// </returns>
        public static readonly DependencyProperty StartValueProperty =
            DependencyProperty.Register("StartValue", typeof(double), typeof(StateRange), new PropertyMetadata(0d, new PropertyChangedCallback(OnStartValueChanged)));


        /// <summary>
        /// Identifies the <see cref="RangeColor"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty RangeColorProperty =
            DependencyProperty.Register("RangeColor", typeof(Brush), typeof(StateRange), new PropertyMetadata(null, new PropertyChangedCallback(OnRangeColorChanged)));
       
        #endregion

        #region Initialization

        /// <summary>
        /// Initializes a new instance of the <see cref="T:Syncfusion.Windows.Gauge.StateRange">StateRange</see> class.
        /// </summary>
        public StateRange()
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="T:Syncfusion.Windows.Gauge.StateRange">StateRange</see> class.
        /// </summary>
        /// <param name="startValue">Start range value.</param>
        /// <param name="endValue">End range value.</param>
        public StateRange(double startValue, double endValue)
        {
            this.StartValue = startValue;
            this.EndValue = endValue;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="T:Syncfusion.Windows.Gauge.StateRange">StateRange</see> class.
        /// </summary>
        /// <param name="startValue">Start range value.</param>
        /// <param name="endValue">End range value.</param>
        /// <param name="rangeColor">Range color value.</param>
        public StateRange(double startValue, double endValue,Brush rangeColor)
        {
            this.StartValue = startValue;
            this.EndValue = endValue;
            this.RangeColor = rangeColor;
        }

        #endregion
      
        #region Events

        /// <summary>
        /// Event that is raised when <see cref="P:Syncfusion.Windows.Gauge.StateRange.EndValue">EndValue</see> property is changed.
        /// </summary>
        public event PropertyChangedCallback EndValueChanged;

        /// <summary>
        /// Event that is raised when <see cref="P:Syncfusion.Windows.Gauge.StateRange.StartValue">StartValue</see> property is changed.
        /// </summary>
        public event PropertyChangedCallback StartValueChanged;


        /// <summary>
        /// Event that is raised when <see cref="P:Syncfusion.Windows.Gauge.StateRange.RangeColor">RangeColor</see> property is changed.
        /// </summary>
        public event PropertyChangedCallback RangeColorChanged;

        #endregion

        #region DP getters & setters

        /// <summary>
        /// Gets or sets end range value This is a dependency property.
        /// </summary>
        /// <remarks>
        /// Default value is 0.
        /// </remarks>
        /// <value>
        /// Type: <see cref="T:System.Double">System.Double</see>
        /// </value>
        /// <seealso cref="double">double</seealso>
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
        /// Gets or sets start range value This is a dependency property.
        /// </summary>
        /// <remarks>
        /// Default value is 0.
        /// </remarks>
        /// <value>
        /// Type : <see cref="T:System.Double">System.Double</see>
        /// </value>
        /// <seealso cref="double">double</seealso>
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
        // Gets or sets range color.This is a dependency property.
        /// </summary>
        /// <remarks>
        /// Default value is 0.
        /// </remarks>
        /// <value>
        /// Type : <see cref="T:System.Double">System.Double</see>
        /// </value>
        
        public Brush RangeColor
        {
            get
            {
                return (Brush)GetValue(RangeColorProperty);
            }

            set
            {
                SetValue(RangeColorProperty, value);
            }
        }

        #endregion

        #region Implementation

        /// <summary>
        /// Updates property value cache and raises <see cref="EndValueChanged"/> event.
        /// </summary>
        /// <param name="e">
        /// Property change details, such as old value and new value.</param>
        protected virtual void OnEndValueChanged(DependencyPropertyChangedEventArgs e)
        {
            if (this.EndValueChanged != null)
            {
                this.EndValueChanged(this, e);
            }
        }

        /// <summary>
        /// Updates property value cache and raises <see cref="StartValueChanged"/> event.
        /// </summary>
        /// <param name="e">
        /// Property change details, such as old value and new value.</param>
        protected virtual void OnStartValueChanged(DependencyPropertyChangedEventArgs e)
        {
            if (this.StartValueChanged != null)
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
            StateRange instance = (StateRange)d;
            instance.OnStartValueChanged(e);
        }

        /// <summary>
        /// Updates property value cache and raises <see cref="StartValueChanged"/> event.
        /// </summary>
        /// <param name="e">
        /// Property change details, such as old value and new value.</param>
        protected virtual void OnRangeColorChanged(DependencyPropertyChangedEventArgs e)
        {
            if (this.RangeColorChanged != null)
            {
                this.RangeColorChanged(this, e);
            }
        }

        /// <summary>
        /// Calls OnStartValueChanged method of the instance, notifies of the depencency property value changes.
        /// </summary>
        /// <param name="d">Dependency object, the change occures on.</param>
        /// <param name="e">Property change details, such as old value and new value.</param>
        private static void OnRangeColorChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            StateRange instance = (StateRange)d;
            instance.OnRangeColorChanged(e);
        }

        /// <summary>
        /// Calls OnEndValueChanged method of the instance, notifies of the depencency property value changes.
        /// </summary>
        /// <param name="d">Dependency object, the change occures on.</param>
        /// <param name="e">Property change details, such as old value and new value.</param>
        private static void OnEndValueChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            StateRange instance = (StateRange)d;
            instance.OnEndValueChanged(e);
        }

        #endregion
    }
}
