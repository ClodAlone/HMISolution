// <copyright file="PointerCapShadow.cs" company="Syncfusion Software">
// Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Any infringement will be prosecuted under
// applicable laws. 
// </copyright>

using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Permissions;
using System.Text;
using System.Windows;
using System.Windows.Media;

namespace Syncfusion.Windows.Gauge
{
    /// <summary>
    /// Represents a pointer cap's shadow visual element.
    /// </summary>
    /// <remarks>
    /// PointerCap is allowed only for <see cref="PointerNeedleType.Needle"/> type pointer. The shadow's
    /// offset from the <see cref="PointerCap"/> can be specified by using <see cref="ScaleBase.ShadowOffset"/> 
    /// property.
    /// </remarks>
    /// <seealso cref="PointerCap"/>
    /// <example>
    /// <code lang="XAML">
    /// <Window x:Class="PointerCapShadowSample.Window1" Title="PointerCapShadowSample" Height="400"
    /// Width="400" xmlns="http://schemas.microsoft.com/winfx/2006/xaml/presentation"
    /// xmlns:x="http://schemas.microsoft.com/winfx/2006/xaml"
    /// xmlns:syncfusion="http://schemas.syncfusion.com/wpf">
    ///     <syncfusion:CircularGauge Name="circularGauge">
    ///         <syncfusion:CircularGauge.Scales>
    ///             <syncfusion:CircularScale Name="circularScale" Radius="100"
    ///                                       ShadowOffset="5">
    ///                 <syncfusion:CircularScale.PointerCap>
    ///                     <syncfusion:PointerCap BackgroundBrush="Gray" BorderBrush="Black" 
    ///                                            PointerCapRadius="7" BorderWidth="2" CapOnTop="True" />
    ///                 </syncfusion:CircularScale.PointerCap> 
    ///             </syncfusion:CircularScale>
    ///         </syncfusion:CircularGauge.Scales>
    ///     </syncfusion:CircularGauge>
    /// </Window>
    /// </code>
    /// <code lang="C#">
    /// using System;
    /// using System.Collections.Generic;
    /// using System.Linq;
    /// using System.Text;
    /// using System.Windows;
    /// using System.Windows.Media;
    /// using System.Windows.Controls;
    /// using Syncfusion.Windows.Shared;
    /// using Syncfusion.Windows.Gauge;<para/>
    /// namespace PointerCapShadowSample
    /// {
    ///     public partial class Window1 : Window
    ///     {
    ///         private CircularScale m_scale;
    ///         private CircularGauge m_gauge;
    ///         public Window1()
    ///         {
    ///             InitializeComponent();<para/>
    ///             m_scale = new CircularScale();
    ///             m_gauge = new CircularGauge();
    ///             m_scale.Radius = 116;
    ///             m_scale.ShadowOffset = 5;
    ///             this.m_gauge.Scales.Add(m_scale);
    ///             m_scale.PointerCap.PointerCapRadius = 5;
    ///             m_scale.PointerCap.BackgroundBrush = new RadialGradientBrush(Color.FromRgb(194, 207, 229), Color.FromRgb(46, 94, 160));
    ///             this.Content = m_gauge;
    ///         }
    ///     }
    /// }
    /// </code>
    /// </example>
    internal class PointerCapShadow : PointerCap
    {
        #region Initialization
        /// <summary>
        /// Initializes a new instance of the <see cref="PointerCapShadow"/> class.
        /// </summary>
        public PointerCapShadow()
        {
            Syncfusion.Licensing.EnvironmentTest.ValidateLicense(typeof(PointerCapShadow));
            this.Loaded += new RoutedEventHandler(PointerCapShadowLoaded);
        }
        #endregion Initialization

        #region Overrides
        /// <summary>
        /// Participates in rendering operations that are directed by the layout system.
        /// </summary>
        /// <remarks>
        /// PointerCapShadow checks whether PointerCap is present and also checks wher
        /// PointerCap Visibility is not Hidden or Collapsed. Then it renders itself with a
        /// maximum Offset of 0.5.
        /// </remarks>
        /// <param name="dc">The drawing instructions for a specific element.</param>
        protected override void OnRender(DrawingContext dc)
        {
            CircularScale scale = this.VisualParent as CircularScale;
            if (scale.PointerCap != null && scale.PointerCap.Visibility == Visibility.Visible)
            {
                this.PointerCapRadius = scale.PointerCap.PointerCapRadius;
                double shadowOffset = (this.VisualParent as CircularScale).ShadowOffset;
                if (shadowOffset > 0.5)
                {
                    shadowOffset -= 0.5;
                }

                dc.DrawEllipse(new SolidColorBrush(Color.FromArgb(100, 123, 123, 118)), null, new Point(shadowOffset, shadowOffset), this.PointerCapRadius * 2, this.PointerCapRadius * 2);
            }
        }
        #endregion Overrides

        #region Implementaion
        /// <summary>
        /// Occurs when the element is laid out, rendered, and ready for interaction.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The object containing the event data.</param>
        private void PointerCapShadowLoaded(object sender, RoutedEventArgs e)
        {
            CircularScale scale = this.VisualParent as CircularScale;
            if (scale != null)
            {
                scale.PointerCap.VisibilityChanged += new PropertyChangedCallback(PointerCapVisibilityChanged);
            }
        }

        /// <summary>
        /// Occurs when the <see cref="Visibility"/> cap property changed.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The object containing the event data.</param>
        private void PointerCapVisibilityChanged(DependencyObject sender, DependencyPropertyChangedEventArgs e)
        {
            CircularScale scale = this.VisualParent as CircularScale;
            if (scale != null)
            {
                this.Visibility = scale.PointerCap.Visibility;
            }
        }
        #endregion Implementaion
    }
}
