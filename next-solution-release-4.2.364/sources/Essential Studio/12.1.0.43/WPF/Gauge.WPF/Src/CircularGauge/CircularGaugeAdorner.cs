// <copyright file="CircularGaugeAdorner.cs" company="Syncfusion Software">
// Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Any infringement will be prosecuted under
// applicable laws. 
// </copyright>

using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using Syncfusion.Windows.Shared;

namespace Syncfusion.Windows.Gauge
{
    /// <summary>
    /// Represents the adorner that displays the child elements of the circular gauge control.
    /// </summary>
    /// <remarks>
    /// All the elements of the <see cref="CircularGauge"/> are laid on top of the CircularGaugeAdorner.
    /// </remarks>
#if SyncfusionFramework4_0
    [System.ComponentModel.DesignTimeVisible(false)]
#endif
    public class CircularGaugeAdorner : GaugeAdorner
    {
        #region Events
        /// <summary>
        /// Event that is raised when <see cref="Radius"/> property is changed.
        /// </summary>
        public event PropertyChangedCallback RadiusChanged;
        #endregion Events

        #region Dependency Properties
        /// <summary>
        /// Identifies the <see cref="Radius"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty RadiusProperty =
            DependencyProperty.Register("Radius", typeof(double), typeof(CircularGaugeAdorner), new FrameworkPropertyMetadata(0d, new PropertyChangedCallback(OnRadiusChanged)));
        #endregion Dependency Properties

        #region DP Getters & Setters
        /// <summary>
        /// Gets or sets the radius for the adorner.
        /// This is a dependency property.
        /// </summary>
        /// <value>
        /// Type: <see cref="double"/>
        /// </value>
        public double Radius
        {
            get
            {
                return (double)GetValue(RadiusProperty);
            }

            set
            {
                SetValue(RadiusProperty, value);
            }
        }
        #endregion DP Getters & Setters

        #region Initialization
        /// <summary>
        /// Initializes a new instance of the <see cref="CircularGaugeAdorner"/> class.
        /// </summary>
        /// <param name="adornedElement">Represents adorned element.</param>
        public CircularGaugeAdorner(UIElement adornedElement)
            : base(adornedElement)
        {
        }
        #endregion Initialization

        #region Implementation
        /// <summary>
        /// Calls OnRadiusChanged method of the instance, notifies of the depencency property value changes.
        /// </summary>
        /// <param name="d">Dependency object, the change occures on.</param>
        /// <param name="e">Property change details, such as old value and new value.</param>
        private static void OnRadiusChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            CircularGaugeAdorner instance = (CircularGaugeAdorner)d;
            instance.OnRadiusChanged(e);
        }

        /// <summary>
        /// Updates property value cache and raises RadiusChanged event.
        /// </summary>
        /// <param name="e">Property change details, such as old value and new value.</param>
        protected virtual void OnRadiusChanged(DependencyPropertyChangedEventArgs e)
        {
            if (RadiusChanged != null)
            {
                RadiusChanged(this, e);
            }
        }
        #endregion Implementation
    }
}
