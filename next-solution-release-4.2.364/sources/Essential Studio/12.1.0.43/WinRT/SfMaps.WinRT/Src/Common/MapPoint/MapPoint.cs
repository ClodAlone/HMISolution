#region Copyright Syncfusion Inc. 2001 - 2014
// Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Any infringement will be prosecuted under
// applicable laws. 
#endregion
using System.Collections.Generic;
using System.Linq;
#if WINRT
using Windows.Foundation;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
#else
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
#if !WPF
using System;
using System.Windows.Threading;
#endif
#endif

namespace Syncfusion.UI.Xaml.Maps
{
    public class MapPoint : Control
    {

        #region Internal Fields

        internal Point mappt;
        internal ShapeFileLayer shapeLayer;

        #endregion

        #region Constructor

        public MapPoint()
        {
            DefaultStyleKey = typeof(MapPoint);
#if SILVERLIGHT || WPF || SILVERLIGHT_5
            MouseLeave += MapPoint_MouseLeave;
#endif

#if WINRT
            PointerEntered += MapPoint_PointerEntered;
            PointerExited += MapPoint_PointerExited;

#else

#endif
            PointData = new Dictionary<string, string>();
        }

        #endregion

#if SILVERLIGHT || WPF || SILVERLIGHT_5

        void MapPoint_MouseLeave(object sender, MouseEventArgs e)
        {
            shapeLayer.MapPointPopupVisibility = Visibility.Collapsed;
        }

#endif

#if WINRT
        void MapPoint_PointerEntered(object sender, Windows.UI.Xaml.Input.PointerRoutedEventArgs e)
        {
            shapeLayer.PointData = this;
            shapeLayer.MapPointMargin = PointMargin;
            shapeLayer.MapPointPopupVisibility = Visibility.Visible;
        }
#endif

#if WINRT
        void MapPoint_PointerExited(object sender, Windows.UI.Xaml.Input.PointerRoutedEventArgs e)
        {
            shapeLayer.MapPointPopupVisibility = Visibility.Collapsed;
        }
#else
        protected override void OnMouseMove(MouseEventArgs e)
        {
            shapeLayer.PointData = new MapPoint { PointData = PointData };
            shapeLayer.MapPointMargin = PointMargin;
            shapeLayer.MapPointPopupVisibility = Visibility.Visible;
            base.OnMouseMove(e);
        }
#if !WPF
        protected override void OnHold(GestureEventArgs e)
        {
            var timer = new DispatcherTimer { Interval = new TimeSpan(0, 0, 3) };
            timer.Tick += timer_Tick;
            timer.Start();
            shapeLayer.PointData = new MapPoint { PointData = PointData };
            shapeLayer.MapPointMargin = PointMargin;
            shapeLayer.MapPointPopupVisibility = Visibility.Visible;
            base.OnHold(e);
        }

        void timer_Tick(object sender, EventArgs e)
        {
            if (sender is DispatcherTimer)
                (sender as DispatcherTimer).Stop();
            shapeLayer.MapPointPopupVisibility = Visibility.Collapsed;
        }
#endif

#endif

        public Dictionary<string, string> PointData
        {
            get { return (Dictionary<string, string>)GetValue(PointDataProperty); }
            set { SetValue(PointDataProperty, value); }
        }

        // Using a DependencyProperty as the backing store for Vals.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty PointDataProperty =
            DependencyProperty.Register("PointData", typeof(Dictionary<string, string>), typeof(MapPoint), new PropertyMetadata(null));

        public object this[string key]
        {
            get
            {
                if (PointData.Keys.Contains(key))
                {
                    return PointData[key];
                }
                return null;
            }
            set
            {
                if (PointData.Keys.Contains(key))
                {
                    PointData[key] = value.ToString();
                }
                else
                {
                    PointData.Add(key, value.ToString());
                }
            }
        }

        public string TooltipText
        {
            get { return (string)GetValue(TooltipTextProperty); }
            set { SetValue(TooltipTextProperty, value); }
        }

        // Using a DependencyProperty as the backing store for TooltipText.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty TooltipTextProperty =
            DependencyProperty.Register("TooltipText", typeof(string), typeof(MapPoint), new PropertyMetadata(null));

        public DataTemplate PointPopupTemplate
        {
            get { return (DataTemplate)GetValue(PointPopupTemplateProperty); }
            set { SetValue(PointPopupTemplateProperty, value); }
        }

        // Using a DependencyProperty as the backing store for PointPopupTemplate.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty PointPopupTemplateProperty =
            DependencyProperty.Register("PointPopupTemplate", typeof(DataTemplate), typeof(MapPoint), new PropertyMetadata(null));

        public DataTemplate ActualTemplate
        {
            get { return (DataTemplate)GetValue(ActualTemplateProperty); }
            set { SetValue(ActualTemplateProperty, value); }
        }

        // Using a DependencyProperty as the backing store for ActualTemplate.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty ActualTemplateProperty =
            DependencyProperty.Register("ActualTemplate", typeof(DataTemplate), typeof(MapPoint), new PropertyMetadata(null));

        public Thickness PointMargin
        {
            get { return (Thickness)GetValue(PointMarginProperty); }
            set { SetValue(PointMarginProperty, value); }
        }

        // Using a DependencyProperty as the backing store for PointMargin.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty PointMarginProperty =
            DependencyProperty.Register("PointMargin", typeof(Thickness), typeof(MapPoint), new PropertyMetadata(null));

        #region NormalKmlStyle
        internal KmlStyle NormalKmlStyle
        {
            get { return (KmlStyle)GetValue(NormalKmlStyleProperty); }
            set { SetValue(NormalKmlStyleProperty, value); }
        }

        // Using a DependencyProperty as the backing store for NormalKmlStyle.  This enables animation, styling, binding, etc...
        internal static readonly DependencyProperty NormalKmlStyleProperty =
            DependencyProperty.Register("NormalKmlStyle", typeof(KmlStyle), typeof(MapShape), new PropertyMetadata(null));
        #endregion

        #region HighlightKmlStyle
        internal KmlStyle HighlightKmlStyle
        {
            get { return (KmlStyle)GetValue(HighlightKmlStyleProperty); }
            set { SetValue(HighlightKmlStyleProperty, value); }
        }

        // Using a DependencyProperty as the backing store for HighlightKmlStyle.  This enables animation, styling, binding, etc...
        internal static readonly DependencyProperty HighlightKmlStyleProperty =
            DependencyProperty.Register("HighlightKmlStyle", typeof(KmlStyle), typeof(MapShape), new PropertyMetadata(null));
        #endregion
    }
}
