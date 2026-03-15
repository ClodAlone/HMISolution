// <copyright file="GaugeCustomLabel.cs" company="Syncfusion Software">
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
using System.Windows.Markup;
using System.Windows.Media;

namespace Syncfusion.Windows.Gauge
{
    /// <summary>
    /// Represents a customizable label child element. 
    /// </summary>
    /// <remarks>
    /// The Label can be placed anywhere within the Gauge using <see cref="LocalizableGaugeElement.Location"/>
    /// property.
    /// </remarks>
    /// <seealso cref="CircularCustomLabel"/>
    /// <seealso cref="GaugeImage"/>
#if SyncfusionFramework4_0
    [System.ComponentModel.DesignTimeVisible(false)]
#endif
    public class GaugeCustomLabel : LocalizableGaugeElement
    {
        #region Events
        /// <summary>
        /// Event that is raised when <see cref="LabelValue"/> property is changed.
        /// </summary>
        public event PropertyChangedCallback LabelValueChanged;

        /// <summary>
        /// Event that is raised when <see cref="TextAngle"/> property is changed.
        /// </summary>
        public event PropertyChangedCallback TextAngleChanged;
        #endregion Events

        #region Dependency Properties
        /// <summary>
        /// Identifies the <see cref="FontFamily"/> dependency property.
        /// </summary>
        public static readonly new DependencyProperty FontFamilyProperty =
            DependencyProperty.Register("FontFamily", typeof(FontFamily), typeof(GaugeCustomLabel), new FrameworkPropertyMetadata(new FontFamily()));

        /// <summary>
        /// Identifies the <see cref="FontSize"/> dependency property.
        /// </summary>
        public static readonly new DependencyProperty FontSizeProperty =
            DependencyProperty.Register("FontSize", typeof(double), typeof(GaugeCustomLabel), new FrameworkPropertyMetadata(10d));

        /// <summary>
        /// Identifies the <see cref="FontWeight"/> dependency property.
        /// </summary>
        public static readonly new DependencyProperty FontWeightProperty =
            DependencyProperty.Register("FontWeight", typeof(FontWeight), typeof(GaugeCustomLabel), new FrameworkPropertyMetadata(new FontWeight()));

        /// <summary>
        /// Identifies the <see cref="LabelValue"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty LabelValueProperty =
            DependencyProperty.Register("LabelValue", typeof(string), typeof(GaugeCustomLabel), new FrameworkPropertyMetadata(string.Empty, FrameworkPropertyMetadataOptions.AffectsRender, new PropertyChangedCallback(OnLabelValueChanged)));

        /// <summary>
        /// Identifies the <see cref="TextAngle"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty TextAngleProperty =
            DependencyProperty.Register("TextAngle", typeof(double), typeof(GaugeCustomLabel), new FrameworkPropertyMetadata(0d, FrameworkPropertyMetadataOptions.AffectsRender, new PropertyChangedCallback(OnTextAngleChanged)));
        #endregion Dependency Properties

        #region DP Getters & Setters
        /// <summary>
        /// Gets or sets the font family for the label.
        /// This is a dependency property.
        /// </summary>
        /// <value>
        /// Type: <see cref="FontFamily"/> 
        /// </value>
        public new FontFamily FontFamily
        {
            get
            {
                return (FontFamily)GetValue(FontFamilyProperty);
            }

            set
            {
                SetValue(FontFamilyProperty, value);
            }
        }

        /// <summary>
        /// Gets or sets the font size for the label.
        /// This is a dependency property.
        /// </summary>
        /// <value>
        /// Type: <see cref="double"/>
        /// Default value is 10.
        /// </value>
        public new double FontSize
        {
            get
            {
                return (double)GetValue(FontSizeProperty);
            }

            set
            {
                SetValue(FontSizeProperty, value);
            }
        }

        /// <summary>
        /// Gets or sets the font weight for the label.
        /// This is a dependency property.
        /// </summary>
        /// <value>
        /// Type: <see cref="FontWeight"/>
        /// </value>
        public new FontWeight FontWeight
        {
            get
            {
                return (FontWeight)GetValue(FontWeightProperty);
            }

            set
            {
                SetValue(FontWeightProperty, value);
            }
        }

        /// <summary>
        /// Gets or sets the text of the label.
        /// This is a dependency property.
        /// </summary>
        /// <value>
        /// Type: <see cref="string"/>
        /// Default value is string.empty.
        /// </value>
        public string LabelValue
        {
            get
            {
                return (string)GetValue(LabelValueProperty);
            }

            set
            {
                SetValue(LabelValueProperty, value);
            }
        }

        /// <summary>
        /// Gets or sets the rotation angle applied to the label.
        /// This is a dependency property.
        /// </summary>
        /// <value>
        /// Type: <see cref="double"/>
        /// Default value is 0.
        /// </value>
        public double TextAngle
        {
            get
            {
                return (double)GetValue(TextAngleProperty);
            }

            set
            {
                SetValue(TextAngleProperty, value);
            }
        }
        #endregion DP Getters & Setters

        #region Overrides
        /// <summary>
        /// Measures the size in layout required for child elements 
        /// and determines a size for the element.
        /// </summary>
        /// <param name="constraint">The available size that this element can give to child elements.</param>
        /// <returns>The size that this element determines it needs during layout.</returns>
        protected override Size MeasureOverride(Size constraint)
        {
            Typeface typeFace = new Typeface(this.FontFamily, new FontStyle(), this.FontWeight, new FontStretch());
            FormattedText formattedText = new FormattedText(this.LabelValue, CultureInfo.CurrentUICulture, FlowDirection.LeftToRight, typeFace, this.FontSize, this.BackgroundBrush);
            return new Size(formattedText.Width, formattedText.Height);
        }

        /// <summary>
        /// Participates in rendering operations that are directed by the layout system.
        /// </summary>
        /// <param name="drawingContext">The drawing instructions for a specific element.</param>
        protected override void OnRender(DrawingContext drawingContext)
        {
            Typeface typeFace = new Typeface(this.FontFamily, new FontStyle(), this.FontWeight, new FontStretch());
            FormattedText formattedText = new FormattedText(this.LabelValue, CultureInfo.CurrentUICulture, FlowDirection.LeftToRight, typeFace, this.FontSize, this.BackgroundBrush);

            Point location = new Point(0, 0);
            TransformGroup transform = new TransformGroup();
            transform.Children.Add(new RotateTransform(this.TextAngle, location.X, location.Y));
            drawingContext.PushTransform(transform);

            drawingContext.DrawText(formattedText, location);

            drawingContext.Pop();
        }
        #endregion Overrides

        #region Implementation

        /// <summary>
        /// Updates property value cache and raises <see cref="LabelValueChanged"/> event.
        /// </summary>
        /// <param name="e">
        /// Property change details, such as old value and new value.</param>
        protected virtual void OnLabelValueChanged(DependencyPropertyChangedEventArgs e)
        {
            if (LabelValueChanged != null)
            {
                this.LabelValueChanged(this, e);
            }
        }

        /// <summary>
        /// Calls OnLabelValueChanged method of the instance, notifies of the depencency property value changes.
        /// </summary>
        /// <param name="d">Dependency object, the change occures on.</param>
        /// <param name="e">Property change details, such as old value and new value.</param>
        private static void OnLabelValueChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            GaugeCustomLabel instance = (GaugeCustomLabel)d;
            instance.OnLabelValueChanged(e);
        }

        /// <summary>
        /// Calls OnTextAngleChanged method of the instance, notifies of the depencency property value changes.
        /// </summary>
        /// <param name="d">Dependency object, the change occures on.</param>
        /// <param name="e">Property change details, such as old value and new value.</param>
        private static void OnTextAngleChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            GaugeCustomLabel instance = (GaugeCustomLabel)d;
            instance.OnTextAngleChanged(e);
        }

        /// <summary>
        /// Updates property value cache and raises <see cref="TextAngleChanged"/> event.
        /// </summary>
        /// <param name="e">
        /// Property change details, such as old value and new value.</param>
        protected virtual void OnTextAngleChanged(DependencyPropertyChangedEventArgs e)
        {
            if (TextAngleChanged != null)
            {
                this.TextAngleChanged(this, e);
            }
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
        /// This method is invoked during initialization.
        /// </summary>
        /// <param name="gauge">The <see cref="GaugeBase"/> that contains the reference to the Gauge.</param>
        internal void SetScope(GaugeBase gauge)
        {
            CalculateScope(gauge);
        }

        /// <summary>
        /// Calculates the scope. 
        /// This method is invoked during initialization.
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
