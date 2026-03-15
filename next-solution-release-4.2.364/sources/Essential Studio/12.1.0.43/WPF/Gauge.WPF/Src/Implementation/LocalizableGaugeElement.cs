// <copyright file="LocalizableGaugeElement.cs" company="Syncfusion Software">
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

namespace Syncfusion.Windows.Gauge
{
    /// <summary>
    /// Represents the gauge elements that can be placed on a particular location within the 
    /// Gauge using the <see cref="Location"/> property
    /// </summary>
    /// <remarks>
    /// All the localizable gauge elements inherits this Class.
    /// </remarks>
#if SyncfusionFramework4_0
    [System.ComponentModel.DesignTimeVisible(false)]
#endif
    public class LocalizableGaugeElement : GaugeElement
    {
        #region Dependency Properties
        /// <summary>
        /// Identifies the <see cref="Location"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty LocationProperty =
            DependencyProperty.Register("Location", typeof(Point), typeof(LocalizableGaugeElement), new FrameworkPropertyMetadata(new Point(50, 50), new PropertyChangedCallback(OnLocationChanged)));
        #endregion Dependency Properties

        #region Events
        /// <summary>
        /// Event that is raised when <see cref="Location"/> property is changed.
        /// </summary>
        public event PropertyChangedCallback LocationChanged;
        #endregion Events

        #region DP Getters & Setters
        /// <summary>
        /// Gets or sets the location of the element.
        /// This is a dependency property.
        /// </summary>
        /// <value>
        /// Type: <see cref="Point"/>
        /// Default value is 50,50.
        /// </value>
        public Point Location
        {
            get
            {
                return (Point)GetValue(LocationProperty);
            }

            set
            {
                SetValue(LocationProperty, value);
            }
        }
        #endregion DP Getters & Setters

        #region Implementation
        /// <summary>
        /// Calls OnLocationChanged method of the instance, notifies of the depencency property value changes.
        /// </summary>
        /// <param name="d">Dependency object, the change occures on.</param>
        /// <param name="e">Property change details, such as old value and new value.</param>
        private static void OnLocationChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            LocalizableGaugeElement instance = (LocalizableGaugeElement)d;
            instance.OnLocationChanged(e);
        }

        /// <summary>
        /// Updates property value cache and raises <see cref="LocationChanged"/> event.
        /// </summary>
        /// <param name="e">
        /// Property change details, such as old value and new value.</param>
        protected virtual void OnLocationChanged(DependencyPropertyChangedEventArgs e)
        {
            if (LocationChanged != null)
            {
                this.LocationChanged(this, e);
            }
        }
        #endregion Implementation       
    }
}
