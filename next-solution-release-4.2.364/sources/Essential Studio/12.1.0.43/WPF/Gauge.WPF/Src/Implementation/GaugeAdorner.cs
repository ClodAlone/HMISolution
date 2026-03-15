// <copyright file="GaugeAdorner.cs" company="Syncfusion Software">
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
using System.ComponentModel;

namespace Syncfusion.Windows.Gauge
{
    /// <summary>
    /// Represents a Adorner that displays the child elements of the Gauge.
    /// </summary>
    /// <remarks>
    /// The adorner is initialized by <see cref="GaugeBase"/> class. All the child elements of the gauge
    /// are drawn on the adorner. This class is derived by <see cref="CircularGaugeAdorner"/> class.
    /// </remarks>
#if SyncfusionFramework4_0
    [System.ComponentModel.DesignTimeVisible(false)]
#endif
    public class GaugeAdorner : TemplatedAdornerBase
    {
        #region Events
        /// <summary>
        /// Event that is raised when <see cref="AdornerHeight"/> property is changed.
        /// </summary>
        public event PropertyChangedCallback AdornerHeightChanged;

        /// <summary>
        /// Event that is raised when <see cref="AdornerWidth"/> property is changed.
        /// </summary>
        public event PropertyChangedCallback AdornerWidthChanged;

        /// <summary>
        /// Event that is raised when <see cref="GaugeParent"/> property is changed.
        /// </summary>
        public event PropertyChangedCallback GaugeParentChanged;
        #endregion Events

        #region CLR Getters & Setters
        /// <summary>
        /// Gets the collection of visual elements.
        /// </summary>
        /// <value>
        /// Type: <see cref="VisualChildrenCollection{FrameworkElement}"/>
        /// </value>
        public VisualChildrenCollection<FrameworkElement> ChildrenCollection
        {
            get
            {
                if (this.GaugeParent != null)
                {
                    return this.GaugeParent.ChildrenCollection;
                }

                return null;
            }
        }
        #endregion CLR Getters & Setters

        #region Dependency Properties
        /// <summary>
        /// Identifies the <see cref="AdornerHeight"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty AdornerHeightProperty =
            DependencyProperty.Register("AdornerHeight", typeof(double), typeof(GaugeAdorner), new FrameworkPropertyMetadata(0d, new PropertyChangedCallback(OnAdornerHeightChanged)));

        /// <summary>
        /// Identifies the <see cref="AdornerWidth"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty AdornerWidthProperty =
            DependencyProperty.Register("AdornerWidth", typeof(double), typeof(GaugeAdorner), new FrameworkPropertyMetadata(0d, new PropertyChangedCallback(OnAdornerWidthChanged)));

        /// <summary>
        /// Identifies the <see cref="GaugeParent"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty GaugeParentProperty =
            DependencyProperty.Register("GaugeParent", typeof(GaugeBase), typeof(GaugeAdorner), new FrameworkPropertyMetadata(null, new PropertyChangedCallback(OnGaugeParentChanged)));
        #endregion Dependency Properties

        #region DP Getters & Setters
        /// <summary>
        /// Gets or sets the width for the adorner.
        /// This is a dependency property.
        /// </summary>
        /// <value>
        /// Type: <see cref="double"/>
        /// </value>
        /// <seealso cref="AdornerHeight"/>
        public double AdornerWidth
        {
            get
            {
                return (double)GetValue(AdornerWidthProperty);
            }

            set
            {
                SetValue(AdornerWidthProperty, value);
            }
        }

        /// <summary>
        /// Gets or sets the height of the adorner.
        /// This is a dependency property.
        /// </summary>
        /// <value>
        /// Type: <see cref="double"/>
        /// </value>
        /// <seealso cref="AdornerWidth"/>
        public double AdornerHeight
        {
            get
            {
                return (double)GetValue(AdornerHeightProperty);
            }

            set
            {
                SetValue(AdornerHeightProperty, value);
            }
        }

        /// <summary>
        /// Gets or sets the parent of the adorner.
        /// This is a dependency property.
        /// </summary>
        /// <value>
        /// Type: <see cref="GaugeBase"/>
        /// </value>
        public GaugeBase GaugeParent
        {
            get
            {
                return (GaugeBase)GetValue(GaugeParentProperty);
            }

            set
            {
                SetValue(GaugeParentProperty, value);
            }
        }
        #endregion DP Getters & Setters

        #region Initialization
        /// <summary>
        /// Initializes static members of the <see cref="GaugeAdorner"/> class.
        /// Overrides some dependency properties.
        /// </summary>
        static GaugeAdorner()
        {
            //BlendException for override
            //DefaultStyleKeyProperty.OverrideMetadata(typeof(TemplatedAdornerInternalControl), new FrameworkPropertyMetadata(typeof(TemplatedAdornerBase)));
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="GaugeAdorner"/> class.
        /// </summary>
        /// <param name="adornedElement">Represents adorned element.</param>
        public GaugeAdorner(UIElement adornedElement)
            : base(adornedElement)
        {
            if (adornedElement is GaugeBase)
            {
                this.GaugeParent = adornedElement as GaugeBase;
            }
        }
        #endregion Initialization

        #region Implementaion
        /// <summary>
        /// Sets the needed style for the adorner.
        /// </summary>
        /// <param name="style">Style to be used</param>
        protected internal void SetStyle(Style style)
        {
            if (!DesignerProperties.GetIsInDesignMode(this))
                InnerControl.Style = style;
        }

        /// <summary>
        /// Calls OnAdornerWidthChanged method of the instance, notifies of the depencency property value changes.
        /// </summary>
        /// <param name="d">Dependency object, the change occures on.</param>
        /// <param name="e">Property change details, such as old value and new value.</param>
        private static void OnAdornerWidthChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            GaugeAdorner instance = (GaugeAdorner)d;
            instance.OnAdornerWidthChanged(e);
        }

        /// <summary>
        /// Updates property value cache and raises AdornerWidthChanged event.
        /// </summary>
        /// <param name="e">Property change details, such as old value and new value.</param>
        protected virtual void OnAdornerWidthChanged(DependencyPropertyChangedEventArgs e)
        {
            if (AdornerWidthChanged != null)
            {
                this.AdornerWidthChanged(this, e);
            }
        }

        /// <summary>
        /// Updates property value cache and raises AdornerHeightChanged event.
        /// </summary>
        /// <param name="e">Property change details, such as old value and new value.</param>
        protected virtual void OnAdornerHeightChanged(DependencyPropertyChangedEventArgs e)
        {
            if (AdornerHeightChanged != null)
            {
                this.AdornerHeightChanged(this, e);
            }
        }

        /// <summary>
        /// Calls OnAdornerHeightChanged method of the instance, notifies of the depencency property value changes.
        /// </summary>
        /// <param name="d">Dependency object, the change occures on.</param>
        /// <param name="e">Property change details, such as old value and new value.</param>
        private static void OnAdornerHeightChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            GaugeAdorner instance = (GaugeAdorner)d;
            instance.OnAdornerHeightChanged(e);
        }

        /// <summary>
        /// Updates property value cache and raises GaugeParentChanged event.
        /// </summary>
        /// <param name="e">Property change details, such as old value and new value.</param>
        protected virtual void OnGaugeParentChanged(DependencyPropertyChangedEventArgs e)
        {
            if (GaugeParentChanged != null)
            {
                GaugeParentChanged(this, e);
            }
        }

        /// <summary>
        /// Calls OnGaugeParentChanged method of the instance, notifies of the depencency property value changes.
        /// </summary>
        /// <param name="d">Dependency object, the change occures on.</param>
        /// <param name="e">Property change details, such as old value and new value.</param>
        private static void OnGaugeParentChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            GaugeAdorner instance = (GaugeAdorner)d;
            instance.OnGaugeParentChanged(e);
        }
        #endregion Implementaion
    }
}
