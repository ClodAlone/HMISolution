#region Copyright Syncfusion Inc. 2001 - 2014
// Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Any infringement will be prosecuted under
// applicable laws. 
#endregion
using System;
using System.Collections;
using System.Reflection;
using System.Collections.Generic;
using System.Linq;
using System.Collections.Specialized;
#if WINDOWS_PHONE
using System.Windows.Controls;
using System.Windows;
using System.Collections.ObjectModel;
using System.Windows.Media;
using System.Windows.Markup;
#else
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Documents;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using System.Collections.ObjectModel;
using Windows.Foundation;
using Windows.UI.Xaml.Markup;
#endif
#if WPF
using System.Data;
#endif
// The Templated Control item template is documented at http://go.microsoft.com/fwlink/?LinkId=234235

namespace Syncfusion.UI.Xaml.Charts
{
#if WINDOWS_PHONE
    [ContentProperty("Content")]
#else
    [ContentProperty(Name = "Content")]
#endif
    public class SfRangeNavigator : Control
    {
        #region Members

        internal ResizableScrollBar navigator;

#if !WINDOWS_PHONE8 && !WINDOWS_PHONE7
        internal ResizableScrollBar scrollbar;
        internal double xrange;
#endif

        private bool isViewRangeSet = false;
        
        public event EventHandler ValueChanged;

        internal double DataStart = 0, DataEnd = 0;
        
        internal ObservableCollection<object> m_selected;

        internal IEnumerable XValues
        {
            get;
            set;
        }
        
        internal double zoomPosition=0;
        internal double zoomFactor=1;

        #endregion

        #region ctor

        public SfRangeNavigator()
        {
            this.DefaultStyleKey = typeof(SfRangeNavigator);
        }

        #endregion

        #region dp

        /// <summary>
        /// Gets or Sets zoom factor. Value must fall within 0 to 1. It determines delta of visible range.
        /// </summary>
        public double ZoomFactor
        {
            get { return (double)GetValue(ZoomFactorProperty); }
            set { SetValue(ZoomFactorProperty, value); }
        }

        // Using a DependencyProperty as the backing store for ZoomFactor.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty ZoomFactorProperty =
            DependencyProperty.Register("ZoomFactor", typeof(double), typeof(SfRangeNavigator), new PropertyMetadata(1d,OnZoomFactorChanged));

        private static void OnZoomFactorChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            (d as SfRangeNavigator).OnZoomFactorChanged(Convert.ToDouble(e.NewValue));
        }

        internal virtual void OnZoomFactorChanged(double newValue)
        {
            if (navigator == null)
            {
                zoomFactor = newValue;
            }
            else
            {
                navigator.IsValueChangedTrigger = false;
                navigator.RangeEnd = ZoomPosition + newValue ;
            }
        }

        /// <summary>
        /// Gets or Sets zoom position. Value must fall within 0 to 1. It determines starting value of visible range
        /// </summary>
        public double ZoomPosition
        {
            get { return (double)GetValue(ZoomPositionProperty); }
            set { SetValue(ZoomPositionProperty, value); }
        }

        // Using a DependencyProperty as the backing store for ZoomPosition.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty ZoomPositionProperty =
            DependencyProperty.Register("ZoomPosition", typeof(double), typeof(SfRangeNavigator), new PropertyMetadata(0d,OnZoomPositionChanged));

        private static void OnZoomPositionChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            (d as SfRangeNavigator).OnZoomPositionChanged(Convert.ToDouble(e.NewValue));
        }

        internal virtual void OnZoomPositionChanged(double newValue)
        {
            if (navigator != null && this is SfRangeNavigator)
            {
                navigator.IsValueChangedTrigger = false;
                navigator.RangeEnd = ZoomFactor + newValue;
                navigator.IsValueChangedTrigger = false;
                navigator.RangeStart = newValue;
            }
            else if (navigator == null)
            {
               zoomPosition = newValue;
            }
        }
        
        /// <summary>
        /// Gets or Sets Navigator's Start Thumb value, Value can be DateTime if Minimum and Maximum are set as DateTime values.
        /// </summary>
        public object ViewRangeStart
        {
            get { return (object)GetValue(ViewRangeStartProperty); }
            set { SetValue(ViewRangeStartProperty, value); }
        }

        // Using a DependencyProperty as the backing store for ViewRangeStart.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty ViewRangeStartProperty =
            DependencyProperty.Register("ViewRangeStart", typeof(object), typeof(SfRangeNavigator), new PropertyMetadata(null, new PropertyChangedCallback (OnViewRangeStartChanged)));

        private static void OnViewRangeStartChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if((d as SfRangeNavigator)!=null)
                (d as SfRangeNavigator).OnViewRangeStartChanged();
        }


        /// <summary>
        /// Gets or Sets Navigator's End Thumb value, Value can be DateTime if Minimum and Maximum are set as DateTime values.
        /// </summary>
        public object ViewRangeEnd
        {
            get { return (object)GetValue(ViewRangeEndProperty); }
            set { SetValue(ViewRangeEndProperty, value); }
        }

        // Using a DependencyProperty as the backing store for ViewRangeEnd.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty ViewRangeEndProperty =
            DependencyProperty.Register("ViewRangeEnd", typeof(object), typeof(SfRangeNavigator), new PropertyMetadata(null,new PropertyChangedCallback(OnViewRangeEndChanged)));

        private  static void OnViewRangeEndChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if((d as SfRangeNavigator)!=null)
                (d as SfRangeNavigator).OnViewRangeEndChanged();
        }

        internal virtual void OnViewRangeEndChanged()
        {
            if (navigator != null && navigator.TrackSize != 0)
                if (!(ViewRangeEnd is DateTime))
                {
                    navigator.IsValueChangedTrigger = false;
                    navigator.RangeEnd = Convert.ToDouble(ViewRangeEnd);
                }
        }

        internal virtual void OnViewRangeStartChanged()
        {
            if (navigator != null && navigator.TrackSize != 0)
               if (!(ViewRangeStart is DateTime))
               {
                        navigator.IsValueChangedTrigger = false;
                        navigator.RangeStart = Convert.ToDouble(ViewRangeStart) ;
               }
                
        }


        /// <summary>
        /// Gets or Sets the content that needs to be hosted inside the Navigator, the content can be any UI element.
        /// </summary>
        public object Content
        {
            get { return (object)GetValue(ContentProperty); }
            set { SetValue(ContentProperty, value); }
        }

        // Using a DependencyProperty as the backing store for Content.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty ContentProperty =
            DependencyProperty.Register("Content", typeof(object), typeof(SfRangeNavigator), new PropertyMetadata(null));



        #endregion

        #region methods

        protected virtual void OnValueChanged()
        {
            if (ValueChanged != null)
            {
                this.ValueChanged(this, EventArgs.Empty);
            }
        }

#if NETFX_CORE
        protected override void OnApplyTemplate()
#else
        public override void OnApplyTemplate()
#endif
        {
            navigator = this.GetTemplateChild("Part_RangePicker") as ResizableScrollBar;
#if !WINDOWS_PHONE8 && !WINDOWS_PHONE7
            scrollbar = this.GetTemplateChild("Part_Scroll") as ResizableScrollBar;
            if(scrollbar!=null)
            scrollbar.ValueChanged += OnScrollbarValueChanged;
#endif
            this.ZoomPosition = 0;
            this.ZoomFactor = 1;
            this.ZoomFactor = this.zoomFactor;
            this.ZoomPosition = this.zoomPosition;
            this.ZoomFactor = navigator.RangeEnd - navigator.RangeStart;
            this.zoomFactor = 1;
            if (navigator != null)
            {
                navigator.ScrollButtonVisibility = Visibility.Collapsed;
                navigator.SizeChanged += OnTimeLineSizeChanged;
            }
            Loaded += OnSfRangeNavigatorLoaded;
            base.OnApplyTemplate();
        }

        void OnSfRangeNavigatorLoaded(object sender, RoutedEventArgs e)
        {
            if (navigator != null)
            {
                navigator.ValueChanged -= OnTimeLineValueChanged;
                navigator.ValueChanged += OnTimeLineValueChanged;
                CalculateSelectedData();
            }
        }

        protected virtual void OnScrollbarValueChanged(object sender, EventArgs e)
        {
#if !WINDOWS_PHONE8 && !WINDOWS_PHONE7
            if (navigator!=null && navigator.Content != null)
            {
                xrange = -((this.navigator.DesiredSize.Width * scrollbar.Scale) * scrollbar.RangeStart);
                navigator.Width = navigator.DesiredSize.Width * scrollbar.Scale;
                navigator.Margin = new Thickness(xrange, 0, 0, 0);
                this.Clip = new RectangleGeometry { Rect = new Rect(0, 0, this.ActualWidth, this.ActualHeight) };
            }
#endif
        }

        internal virtual void CalculateSelectedData()
        {
            if (isViewRangeSet || ViewRangeStart == null && ViewRangeEnd == null)
            {
                this.ZoomFactor = (navigator.RangeEnd - navigator.RangeStart);
                this.ZoomPosition = navigator.RangeStart;
                this.ViewRangeEnd = navigator.RangeEnd;
                this.ViewRangeStart = navigator.RangeStart;
            }
            else if (Convert.ToDouble(ViewRangeStart) >= navigator.Minimum && Convert.ToDouble(ViewRangeEnd) <= navigator.Maximum)
            {
                navigator.RangeStart = Convert.ToDouble(ViewRangeStart);
                navigator.RangeEnd = Convert.ToDouble(ViewRangeEnd);
                this.ZoomFactor = (navigator.RangeEnd - navigator.RangeStart);
                this.ZoomPosition = navigator.RangeStart;
            }
            else
            {
                isViewRangeSet = true;
                CalculateSelectedData();
            }
            isViewRangeSet = true;
       }

        protected virtual void OnTimeLineValueChanged(object sender, EventArgs e)
        {
            CalculateSelectedData();
            OnValueChanged();
        }

        protected virtual void OnTimeLineSizeChanged(object sender, SizeChangedEventArgs e)
        {
            
        }

        

        #endregion
    }
}
