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
    using System.Globalization;
    using System.Linq;
    using System.Text;
    using System.Windows;
    using System.Windows.Controls;
    using System.Windows.Media;

    /// <summary>
    /// Represents the custom label child element of the circular gauge control. It can be placed anywhere within the control.
    /// </summary>
    /// <example>
    /// <list type="table">
    /// <listheader>
    /// <term>C# :</term></listheader>
    /// <item>
    /// <description>using Syncfusion.Windows.Gauge; 
    /// <para></para>
    /// <para>GaugeLabel label = new GaugeLabel();</para>
    /// <para>            label.FontSize = 17;</para>
    /// <para>            label.FontFamily = new FontFamily( &quot;Verdana&quot; );</para>
    /// <para>            label.Location = new Point( 50, 80 );</para>
    /// <para>            label.Text = &quot;Silverlight&quot;;</para>
    /// <para>            label.Background = new SolidColorBrush( Colors.Black );</para>
    /// <para>            circularGauge1.CustomLabels.Add( label ); </para></description></item></list>
    /// <list type="table">
    /// <listheader>
    /// <term>Xaml :</term></listheader>
    /// <item>
    /// <description>xmlns:syncfusion=&quot;clr-namespace:Syncfusion.Windows.Gauge;assembly=Syncfusion.Gauge.Silverlight&quot;
    /// <para>    </para>
    /// <para>&lt;syncfusion:CircularGauge.CustomLabels&gt;</para>
    /// <para>         &lt;syncfusion:GaugeLabel Text=&quot;Syncfusion&quot; FontSize=&quot;20&quot; Foreground=&quot;Black&quot; Location=&quot;20,50&quot;/&gt;</para>
    /// <para>  &lt;/syncfusion:CircularGauge.CustomLabels&gt;</para></description></item></list>
    /// </example>
    public class GaugeLabel : LocalizableGaugeElement
    {
        #region Dependency properties

        /// <summary>
        /// Identifies the <see
        /// cref="P:Syncfusion.Windows.Gauge.GaugeLabel.TextAngle">TextAngle</see>
        /// dependency property.
        /// </summary>
        /// <returns>
        /// Type : <see cref="T:System.Double">System.Double</see>
        /// </returns>
        public static readonly DependencyProperty TextAngleProperty =
            DependencyProperty.Register("TextAngle", typeof(double), typeof(GaugeLabel), new PropertyMetadata(0d, new PropertyChangedCallback(OnTextAngleChanged)));
        /// <summary>
        /// Identifies the <see cref="AdjustAngle">Text</see> dependency property.
        /// </summary>
        public static readonly DependencyProperty AdjustAngleProperty =
           DependencyProperty.Register("AdjustAngle", typeof(double), typeof(GaugeLabel), new PropertyMetadata(0d, new PropertyChangedCallback(OnAdjustAngleChanged)));

        /// <summary>
        /// Identifies the <see cref="P:Syncfusion.Windows.Gauge.GaugeLabel.Text">Text</see> dependency property.
        /// </summary>
        /// <returns>
        /// <para>Type : <see cref="T:System.String">System.String</see></para>
        /// </returns>
        public static readonly DependencyProperty TextProperty =
            DependencyProperty.Register("Text", typeof(string), typeof(GaugeLabel), new PropertyMetadata(new PropertyChangedCallback(OnTextChanged)));

        #endregion

        #region Private members

        /// <summary>
        /// The text block used to draw the text.
        /// </summary>
        private TextBlock mtextBlock;

        #endregion

        #region Initialization

        /// <summary>
        /// Initializes a new instance of the <see
        /// cref="T:Syncfusion.Windows.Gauge.GaugeLabel">GaugeLabel</see> class
        /// </summary>
        public GaugeLabel()
        {
            DefaultStyleKey = typeof(GaugeLabel);
        }

        #endregion

        #region Events

        /// <summary>
        /// Event that is raised when <see cref="P:Syncfusion.Windows.Gauge.GaugeLabel.TextAngle">TextAngle</see> property is changed.
        /// </summary>
        public event PropertyChangedCallback TextAngleChanged;

        /// <summary>
        /// Event that is raised when <see cref="P:Syncfusion.Windows.Gauge.GaugeLabel.Text">Text</see> property is changed.
        /// </summary>
        public event PropertyChangedCallback TextChanged;

        #endregion

        #region DP getters & setters

        /// <summary>
        /// Gets or sets the text of the label. This is a dependency property.
        /// </summary>
        /// <remarks>
        /// <para>Default value is string.empty.</para>
        /// </remarks>
        /// <value>
        /// <para>Type: <see cref="N:System.Text">System.Text</see></para>
        /// <para> </para>
        /// </value>
        /// <seealso cref="string">string</seealso>
        public string Text
        {
            get
            {
                return (string)GetValue(TextProperty);
            }

            set
            {
                SetValue(TextProperty, value);
            }
        }

        /// <summary>
        /// Gets or sets the rotation angle applied to the label. This is a dependency property.
        /// </summary>
        /// <remarks>
        /// <para>Default value is 0.</para>
        /// </remarks>
        /// <value>
        /// Type : <see cref="T:System.Double">System.Double</see>
        /// </value>
        /// <seealso cref="double">double</seealso>
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



        /// <summary>
        /// Gets or sets the Angle adustment
        /// </summary>
        internal double AdjustAngle
        {
            get
            {
                return (double)GetValue(AdjustAngleProperty);//this.madjustAngle;
            }

            set
            {
                SetValue(AdjustAngleProperty,value);//this.madjustAngle = value;
            }
        }

        #endregion

        #region Overrides

        /// <summary>
        /// Builds the current template's visual tree if necessary.
        /// </summary>
        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();

            this.mtextBlock = this.GetTemplateChild("TextBlock") as TextBlock;

            if (this.mtextBlock != null)
            {
                this.mtextBlock.Text = this.Text;
                this.mtextBlock.FontFamily = this.FontFamily;
                this.mtextBlock.FontSize = this.FontSize;
                this.mtextBlock.FontWeight = this.FontWeight;
            }
            this.UpdateVisualStyle();
            this.RefreshTextAngle();
        }

        #endregion

        #region Implementation

        /// <summary>
        /// Measures the size in layout required for child elements 
        /// and determines a size for the element.
        /// </summary>
        /// <param name="constraint">The available size that this element can give to child elements.</param>
        /// <returns>The size that this element determines it needs during layout.</returns>
        protected override Size MeasureOverride(Size constraint)
        {
            Size size = base.MeasureOverride(constraint);
            if (this.mtextBlock != null && this.mtextBlock.Text != string.Empty)
            {
                size = new Size(this.mtextBlock.ActualWidth, this.mtextBlock.ActualHeight);
                this.RefreshTextAngle();
            }

            return size;
        }

        /// <summary>
        /// Updates property value cache and raises <see cref="TextAngleChanged"/> event.
        /// </summary>
        /// <param name="e">
        /// Property change details, such as old value and new value.</param>
        protected virtual void OnTextAngleChanged(DependencyPropertyChangedEventArgs e)
        {
            this.RefreshTextAngle();
            if (this.TextAngleChanged != null)
            {
                this.TextAngleChanged(this, e);
            }
        }

        /// <summary>
        /// Updates property value cache and raises <see cref="TextChanged"/> event.
        /// </summary>
        /// <param name="e">
        /// Property change details, such as old value and new value.</param>
        protected virtual void OnTextChanged(DependencyPropertyChangedEventArgs e)
        {
            if (this.mtextBlock != null)
            {
                this.mtextBlock.Text = this.Text;
                this.InvalidateMeasure();
            }

            if (this.TextChanged != null)
            {
                this.TextChanged(this, e);
            }
        }

        private static void OnAdjustAngleChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            GaugeLabel label = d as GaugeLabel;
            if (label.mtextBlock != null)
            {
                label.InvalidateMeasure();
            }
        }

        /// <summary>
        /// Calls OnTextAngleChanged method of the instance, notifies of the depencency property value changes.
        /// </summary>
        /// <param name="d">Dependency object, the change occures on.</param>
        /// <param name="e">Property change details, such as old value and new value.</param>
        private static void OnTextAngleChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            GaugeLabel instance = (GaugeLabel)d;
            instance.OnTextAngleChanged(e);
        }

        /// <summary>
        /// Calls OnTextChanged method of the instance, notifies of the depencency property value changes.
        /// </summary>
        /// <param name="d">Dependency object, the change occures on.</param>
        /// <param name="e">Property change details, such as old value and new value.</param>
        private static void OnTextChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            GaugeLabel instance = (GaugeLabel)d;
            instance.OnTextChanged(e);
        }

        /// <summary>
        /// Refreshes the text angle
        /// </summary>
        private void RefreshTextAngle()
        {
            if (this.mtextBlock != null)
            {
                RotateTransform transform = new RotateTransform();
                transform.Angle = this.TextAngle + this.AdjustAngle;
                transform.CenterX = this.mtextBlock.ActualWidth / 2;
                transform.CenterY = this.mtextBlock.ActualHeight / 2;
                this.mtextBlock.RenderTransform = transform;
            }
        }
        #endregion
    }
}
