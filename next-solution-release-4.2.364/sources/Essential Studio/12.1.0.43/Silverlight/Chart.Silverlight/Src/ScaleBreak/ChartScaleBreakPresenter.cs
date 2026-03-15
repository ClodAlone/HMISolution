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
    /// Class implentation for ChartScaleBreakPresenter
    /// </summary>
    public class ChartScaleBreakPresenter : ItemsControl
    {
        #region Dependency Properties

        /// <summary>
        /// Identifies the Area dependency property.
        /// </summary>
        public static readonly DependencyProperty AreaProperty =
            DependencyProperty.Register("Area", typeof(ChartArea), typeof(ChartScaleBreakPresenter), new PropertyMetadata(null));

        /// <summary>
        /// Get or Set Area property
        /// </summary>
        public ChartArea Area
        {
            get
            {
                return (ChartArea)GetValue(AreaProperty);
            }

            set
            {
                SetValue(AreaProperty, value);
            }
        }

        /// <summary>
        /// Identifies the Xaxis dependency property.
        /// </summary>
        public static readonly DependencyProperty XAxisProperty =
           DependencyProperty.Register("XAxis", typeof(ChartAxis), typeof(ChartScaleBreakPresenter), new PropertyMetadata(null));

        /// <summary>
        /// Get or Set XaxisProperty
        /// </summary>
        public ChartAxis XAxis
        {
            get
            {
                return (ChartAxis)GetValue(XAxisProperty);
            }

            set
            {
                SetValue(XAxisProperty, value);
            }
        }

        /// <summary>
        /// Identifies the YAxis dependency property.
        /// </summary>
        public static readonly DependencyProperty YAxisProperty =
           DependencyProperty.Register("YAxis", typeof(ChartAxis), typeof(ChartScaleBreakPresenter), new PropertyMetadata(null));

        /// <summary>
        /// Get or Set YAxis Property
        /// </summary>
        public ChartAxis YAxis
        {
            get
            {
                return (ChartAxis)GetValue(YAxisProperty);
            }

            set
            {
                SetValue(YAxisProperty, value);
            }
        }

        #endregion

        /// <summary>
        /// Prepares the specified element to display the specified item. 
        /// </summary>
        /// <param name="element">The element used to display the specified item.</param><param name="item">The item to display.</param>
        protected override void PrepareContainerForItemOverride(DependencyObject element, object item)
        {
            //ChartScaleBreak scaleBreak = element as ChartScaleBreak;
            //ItemsPanelTemplate  temp = this.ItemsPanel;
            //int count = VisualTreeHelper.GetChildrenCount(this);

            //ItemsPresenter presenter = VisualTreeHelper.GetChild(this, 0) as ItemsPresenter;
            //object panel = VisualTreeHelper.GetChild(presenter, 0) as object;

            ContentPresenter presenter = element as ContentPresenter;
            ChartScaleBreak scaleBreak = item as ChartScaleBreak;
            if (presenter != null && item != null)
            {
                presenter.Tag = scaleBreak;

            }

            
           
        }

    }
}
