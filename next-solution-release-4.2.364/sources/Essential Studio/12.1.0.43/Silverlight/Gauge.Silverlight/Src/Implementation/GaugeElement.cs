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
    using System.Globalization;
    using System.Net;
    using System.Windows;
    using System.Windows.Controls;
    using System.Windows.Data;
    using System.Windows.Documents;
    using System.Windows.Ink;
    using System.Windows.Input;
    using System.Windows.Media;
    using System.Windows.Media.Animation;
    using System.Windows.Shapes;

    /// <summary>
    /// Represents gauge element.
    /// </summary>
    public class GaugeElement : Control
    {
        #region Dependency properties

        /// <summary>
        /// Identifies the <see cref="GaugeElementParent"/> dependency property.
        /// </summary>
        internal static readonly DependencyProperty GaugeElementParentProperty =
            DependencyProperty.Register("GaugeElementParent", typeof(FrameworkElement), typeof(GaugeElement), new PropertyMetadata(new PropertyChangedCallback(OnGaugeElementParentChanged)));



        /// <summary>
        /// Gets or sets Visual Style.
        /// </summary>
        /// <value></value>
        /// <remarks></remarks>
        protected  GaugeVisualStyle VisualStyle
        {
            get
            {
                return (GaugeVisualStyle)GetValue(VisualStyleProperty);
            }
            set { SetValue(VisualStyleProperty, value); }
        }
        /// <summary>
        /// Identifies the <see cref="VisualStyle"/> dependency property.
        /// </summary>
        // Using a DependencyProperty as the backing store for VisualStyle.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty VisualStyleProperty =
            DependencyProperty.Register("VisualStyle", typeof(GaugeVisualStyle), typeof(GaugeElement), new PropertyMetadata(GaugeVisualStyle.Blend, new PropertyChangedCallback(OnVisualStyleChanged)));


        #endregion

        #region Events

        /// <summary>
        /// Event that is raised when <see cref="GaugeElementParent"/> property is changed.
        /// </summary>
        internal event PropertyChangedCallback GaugeElementParentChanged;

        #endregion

        #region DP getters & setters

        /// <summary>
        /// Gets or sets the gauge parent element
        /// </summary>
        internal FrameworkElement GaugeElementParent
        {
            get
            {
                return (FrameworkElement)GetValue(GaugeElementParentProperty);
            }

            set
            {
                SetValue(GaugeElementParentProperty, value);
            }
        }
        #endregion

        #region Implementation

        /// <summary>
        /// Updates property value cache and raises <see cref="GaugeElementParentChanged"/> event.
        /// </summary>
        /// <param name="e">
        /// Property change details, such as old value and new value.</param>
        protected virtual void OnGaugeElementParentChanged(DependencyPropertyChangedEventArgs e)
        {
            if (this.GaugeElementParentChanged != null)
            {
                this.GaugeElementParentChanged(this, e);
            }
        }

        /// <summary>
        /// Calls OnGaugeElementParentChanged method of the instance, notifies of the depencency property value changes.
        /// </summary>
        /// <param name="d">Dependency object, the change occures on.</param>
        /// <param name="e">Property change details, such as old value and new value.</param>
        private static void OnGaugeElementParentChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            GaugeElement instance = (GaugeElement)d;
            instance.OnGaugeElementParentChanged(e);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="d"></param>
        /// <param name="args">An <see cref="T:System.Windows.DependencyPropertyChangedEventArgs">DependencyPropertyChangedEventArgs</see> that contains the event data.</param>
        /// <remarks></remarks>
        protected static void OnVisualStyleChanged(DependencyObject d, DependencyPropertyChangedEventArgs args)
        {

        }

        /// <summary>
        /// 
        /// </summary>
        /// <remarks></remarks>
        protected virtual void UpdateVisualStyle()
        {
            ResourceDictionary _resources = this.GetResources(this.VisualStyle);
            string _key;
            if (this is CircularScale)
            {
                _key = this.GetResourceKey("CircularScale", this.VisualStyle);
                (this as CircularScale).Style = _resources[_key] as Style;
                (this as CircularScale).RefreshScale();
            }
            else if (this is LinearScale)
            {
                _key = this.GetResourceKey("LinearScale", this.VisualStyle);
                (this as LinearScale).Style = _resources[_key] as Style;
                (this as LinearScale).RefreshScale();
            }
            else if (this is MarkTickSet)
            {
                _key = this.GetResourceKey("MarkTickSet", this.VisualStyle);
                (this as MarkTickSet).Style = _resources[_key] as Style;
                (this as MarkTickSet).RefreshTick();
            }
            else if (this is LabelTickSet)
            {
                _key = this.GetResourceKey("LabelTickSet", this.VisualStyle);
             //   (this as LabelTickSet).Style = _resources[_key] as Style;
            }
            else if (this is PointerCap)
            {
                _key = this.GetResourceKey("PointerCap", this.VisualStyle);
                (this as PointerCap).Style = _resources[_key] as Style;
                (this as PointerCap).RefreshPointerCap();
            }
            else if (this is LinearRange)
            {
                _key = this.GetResourceKey("LinearRange", this.VisualStyle);
                (this as LinearRange).Style = _resources[_key] as Style;
                (this as LinearRange).RefreshRange();
            }
            else if (this is CircularRange)
            {
                _key = this.GetResourceKey("CircularRange", this.VisualStyle);
                (this as CircularRange).Style = _resources[_key] as Style;
                (this as CircularRange).RefreshRange();
            }
            else if (this is GaugeImage)
            {
                _key = this.GetResourceKey("GaugeImage", this.VisualStyle);
                (this as GaugeImage).Style = _resources[_key] as Style;
            }
            else if (this is GaugeLabel)
            {
                _key = this.GetResourceKey("GaugeLabel", this.VisualStyle);
                (this as GaugeLabel).Style = _resources[_key] as Style;
            }
            else if (this is StateIndicator)
            {
                _key = this.GetResourceKey("StateIndicator", this.VisualStyle);
                (this as StateIndicator).Style = _resources[_key] as Style;
            }
            else if (this is GaugeBorder)
            {
                _key = this.GetResourceKey("GaugeBorder", this.VisualStyle);
                (this as GaugeBorder).Style = _resources[_key] as Style;
                (this as GaugeBorder).RefreshGaugeBorder();
            }
            else if (this is CircularKnob)
            {
                _key = this.GetResourceKey("CircularKnob", this.VisualStyle);
                (this as CircularKnob).Style = _resources[_key] as Style;
            }
            else if (this is CircularPointer)
            {
                _key = this.GetResourceKey("CircularPointer", this.VisualStyle);
                (this as CircularPointer).Style = _resources[_key] as Style;
                (this as CircularPointer).RefreshPointer();
            }
            else if (this is LinearBarPointer)
            {
                _key = this.GetResourceKey("LinearBarPointer", this.VisualStyle);
                (this as LinearBarPointer).Style = _resources[_key] as Style;
            }
            else if (this is LinearMarkerPointer)
            {
                _key = this.GetResourceKey("LinearMarkerPointer", this.VisualStyle);
                (this as LinearMarkerPointer).Style = _resources[_key] as Style;
            }
        }

        private ResourceDictionary GetResources(GaugeVisualStyle style)
        {
            ResourceDictionary res = new ResourceDictionary();
            switch (style)
            {
                case GaugeVisualStyle.Blend:
                    res = new ResourceDictionary() { Source = new Uri("/Syncfusion.Gauge.Silverlight;component/themes/BlendStyle.xaml", UriKind.RelativeOrAbsolute) };
                    break;
                case GaugeVisualStyle.Metro:
                    res = new ResourceDictionary() { Source = new Uri("/Syncfusion.Gauge.Silverlight;component/themes/MetroStyle.xaml", UriKind.RelativeOrAbsolute) };
                    break;
                case GaugeVisualStyle.Office2003:
                    res = new ResourceDictionary() { Source = new Uri("/Syncfusion.Gauge.Silverlight;component/themes/Office2003Style.xaml", UriKind.RelativeOrAbsolute) };
                    break;
                case GaugeVisualStyle.Office2007Black:
                    res = new ResourceDictionary() { Source = new Uri("/Syncfusion.Gauge.Silverlight;component/themes/Office2007Black.xaml", UriKind.RelativeOrAbsolute) };
                    break;
                case GaugeVisualStyle.Office2007Blue:
                    res = new ResourceDictionary() { Source = new Uri("/Syncfusion.Gauge.Silverlight;component/themes/Office2007Blue.xaml", UriKind.RelativeOrAbsolute) };
                    break;
                case GaugeVisualStyle.Office2007Silver:
                    res = new ResourceDictionary() { Source = new Uri("/Syncfusion.Gauge.Silverlight;component/themes/Office2007Silver.xaml", UriKind.RelativeOrAbsolute) };
                    break;
                case GaugeVisualStyle.VS2010:
                    res = new ResourceDictionary() { Source = new Uri("/Syncfusion.Gauge.Silverlight;component/themes/VS2010Style.xaml", UriKind.RelativeOrAbsolute) };
                    break;
                case GaugeVisualStyle.Default:
                    res = new ResourceDictionary() { Source = new Uri("/Syncfusion.Gauge.Silverlight;component/themes/generic.xaml", UriKind.RelativeOrAbsolute) };
                    break;
            }
            return res;
        }

        private string GetResourceKey(string element, GaugeVisualStyle currentStyle)
        {
            return currentStyle+element+"Style";
        }
        #endregion
    }
}
