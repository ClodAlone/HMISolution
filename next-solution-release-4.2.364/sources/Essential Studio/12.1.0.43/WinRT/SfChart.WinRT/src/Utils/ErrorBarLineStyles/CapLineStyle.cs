#region Copyright Syncfusion Inc. 2001 - 2014
// Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Any infringement will be prosecuted under
// applicable laws. 
#endregion
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
#if WINDOWS_PHONE
using System.Windows;
#else
using Windows.UI.Xaml;
#endif
namespace Syncfusion.UI.Xaml.Charts
{
    public class CapLineStyle : LineStyle
    {

        public CapLineStyle(ChartSeriesBase series)
            : base(series)
        {
        }

        public CapLineStyle()
        {
            
        }
        public static readonly DependencyProperty VisibilityProperty =
         DependencyProperty.Register("Visibility", typeof(Visibility), typeof(CapLineStyle), new PropertyMetadata(Visibility.Visible, OnPropertyChange));

        public Visibility Visibility
        {
            get { return (Visibility)GetValue(VisibilityProperty); }
            set { SetValue(VisibilityProperty, value); }
        }

        public static readonly DependencyProperty LineWidthProperty =
            DependencyProperty.Register("LineWidth", typeof(double), typeof(CapLineStyle), new PropertyMetadata(10d, OnPropertyChange));

        private static void OnPropertyChange(DependencyObject obj, DependencyPropertyChangedEventArgs dependencyPropertyChangedEventArgs)
        {
            var chartSeries = (obj as CapLineStyle).Series;
            if (chartSeries != null) chartSeries.ActualArea.ScheduleUpdate();
        }


        public double LineWidth
        {
            get { return (double)GetValue(LineWidthProperty); }
            set { SetValue(LineWidthProperty, value); }
        }
       
    }
}
