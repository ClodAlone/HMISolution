#region Copyright Syncfusion Inc. 2001 - 2014
// Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Any infringement will be prosecuted under
// applicable laws. 
#endregion
using System;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Ink;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;

namespace Syncfusion.Windows.Chart
{
    /// <summary>
    /// Class implementation for ChartAreaWatermarkControl
    /// </summary>
    public class ChartAreaWatermarkControl : Grid
    {
        #region Dependency properties

        /// <summary>
        /// Identifies the Axes dependency property.
        /// </summary>
        public static readonly DependencyProperty AxesProperty =
            DependencyProperty.Register("Axes", typeof(AxesCollection), typeof(ChartAreaWatermarkControl), new PropertyMetadata(null));

        /// <summary>
        /// Get or Set AxesProperty
        /// </summary>
        public AxesCollection Axes
        {
            get
            {
                return (AxesCollection)GetValue(AxesProperty);
            }
            set
            {
                SetValue(AxesProperty, value);
            }
        }

        #endregion

        /// <summary>
        /// Called when instance created for ChartAreaWatermarkControl class
        /// </summary>
        public ChartAreaWatermarkControl()
        {
            this.Axes = new AxesCollection();
        }
    }
}
