// <copyright file="StateRange.cs" company="Syncfusion Software">
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
using System.Windows.Markup;
using System.Windows.Media;

namespace Syncfusion.Windows.Gauge
{
    /// <summary>
    /// Represents state range object that helps specifying the range in which the indicator
    /// should be active.
    /// </summary>
    /// <remarks>
    /// The <see cref="StartValue"/> and <see cref="EndValue"/> should be properly initialized
    /// inorder to change the state of the <see cref="StateIndicator"/>
    /// </remarks>
    /// <seealso cref="StateIndicator"/>
#if SyncfusionFramework4_0
    [System.ComponentModel.DesignTimeVisible(false)]
#endif
    public class StateRange : FrameworkElement
    {
        #region Dependency Propterties
        /// <summary>
        /// Identifies the <see cref="EndValue"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty EndValueProperty =
            DependencyProperty.Register("EndValue", typeof(double), typeof(StateRange), new FrameworkPropertyMetadata(0d, new PropertyChangedCallback(OnEndValueChanged)));

        /// <summary>
        /// Identifies the <see cref="StartValue"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty StartValueProperty =
            DependencyProperty.Register("StartValue", typeof(double), typeof(StateRange), new FrameworkPropertyMetadata(0d, new PropertyChangedCallback(OnStartValueChanged)));

        /// <summary>
        /// Identifies the <see cref="RangeColor"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty RangeColorProperty =
            DependencyProperty.Register("RangeColor", typeof(Brush), typeof(StateRange), new FrameworkPropertyMetadata(null, new PropertyChangedCallback(OnRangeColorChanged)));
        
        #endregion Dependency Propterties

        #region Initialization
        /// <summary>
        /// Initializes a new instance of the <see cref="StateRange"/> class.
        /// </summary>
        public StateRange()
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="StateRange"/> class.
        /// </summary>
        /// <param name="startValue">Start range value.</param>
        /// <param name="endValue">End range value.</param>
        public StateRange(double startValue, double endValue)
        {
            this.StartValue = startValue;
            this.EndValue = endValue;            
        }
        
        /// <summary>
        /// Initializes a new instance of the <see cref="StateRange"/> class.
        /// </summary>
        /// <param name="startValue">Start range value.</param>
        /// <param name="endValue">End range value.</param>
        /// <param name="rangecolor">Range Color value.</param>
        public StateRange(double startValue, double endValue,Brush rangecolor)
        {
            this.StartValue = startValue;
            this.EndValue = endValue;
            this.RangeColor = rangecolor;
        }
        #endregion Initialization

        #region Events
        /// <summary>
        /// Event that is raised when <see cref="EndValue"/> property is changed.
        /// </summary>
        public event PropertyChangedCallback EndValueChanged;

        /// <summary>
        /// Event that is raised when <see cref="StartValue"/> property is changed.
        /// </summary>
        public event PropertyChangedCallback StartValueChanged;

        /// <summary>
        /// Event that is raised when <see cref="RangeColor"/> property is changed.
        /// </summary>
        public event PropertyChangedCallback RangeColorChanged;
        #endregion Events

        #region DP Getters & Setters
        /// <summary>
        /// Gets or sets the end range value
        /// This is a dependency property.
        /// </summary>
        /// <value>
        /// Type: <see cref="double"/>
        /// Default value is 0.
        /// </value>
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
        /// Gets or sets the start range value
        /// This is a dependency property.
        /// </summary>
        /// <value>
        /// Type: <see cref="double"/>
        /// Default value is 0.
        /// </value>
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
        /// Gets or sets the range color value
        /// This is a dependency property.
        /// </summary>
        /// <value>
        /// Type: <see cref="Brush"/>
        /// Default value is Red.
        /// </value>
        /// <seealso cref="EndValue"/>
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

        #endregion DP Getters & Setters

        #region Implementation
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
            StateRange instance = (StateRange)d;
            instance.OnEndValueChanged(e);
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
            StateRange instance = (StateRange)d;
            instance.OnStartValueChanged(e);
        }

        /// <summary>
        /// Updates property value cache and raises <see cref="RangeColorChanged"/> event.
        /// </summary>
        /// <param name="e">
        /// Property change details, such as old value and new value.</param>
        protected virtual void OnRangeColorChanged(DependencyPropertyChangedEventArgs e)
        {
            if (RangeColorChanged != null)
            {
                this.RangeColorChanged(this, e);
            }
        }

        /// <summary>
        /// Calls OnRangeColorChanged method of the instance, notifies of the depencency property value changes.
        /// </summary>
        /// <param name="d">Dependency object, the change occures on.</param>
        /// <param name="e">Property change details, such as old value and new value.</param>
        private static void OnRangeColorChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            StateRange instance = (StateRange)d;
            instance.OnRangeColorChanged(e);
        }
        #endregion Implementation

        #region Added Code
        /// <summary>
        /// Raises the <see cref="System.Windows.FrameworkElement.Initialized"/> event. 
        /// This method is invoked whenever <see cref="System.Windows.FrameworkElement.IsInitialized"/> property 
        /// is set to true internally.
        /// </summary>
        /// <param name="e">The <see cref="System.EventArgs"/> that contains the event data.</param>
        protected override void OnInitialized(EventArgs e)
        {
            base.OnInitialized(e);
            NameScope.SetNameScope(this, null);
        }
        
        /// <summary>
        /// Sets the scope to <see cref="CircularGauge"/>to facilitate Binding. 
        /// This method is invoked during collection changed.
        /// </summary>
        /// <param name="gauge">The <see cref="GaugeBase"/> that contains the reference to the Gauge.</param>
        internal void SetScope(GaugeBase gauge)
        {
            CalculateScope(gauge);
        }

        /// <summary>
        /// Calculates the scope. 
        /// This method is invoked during collection changed.
        /// </summary>
        /// <param name="obj">The <see cref="DependencyObject"/> which contains the element to set the scope for.</param>
        internal void CalculateScope(DependencyObject obj)
        {
            DependencyObject ele = obj;
            while (ele != null)
            {
                INameScope ns = NameScope.GetNameScope(ele);
                if (ns != null)
                {
                    if (!(ns is System.Windows.NameScope))
                    {
                        break;
                    }

                    NameScope.SetNameScope(this, ns);
                    break;
                }

                ele = LogicalTreeHelper.GetParent(ele) ?? VisualTreeHelper.GetParent(ele);
            }
        }
        #endregion Added Code
    }
}
