#region Copyright Syncfusion Inc. 2001 - 2014
// Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Any infringement will be prosecuted under
// applicable laws. 
#endregion
#if WINDOWS_PHONE
using System.Windows;
using System.Windows.Media;
using System.Windows.Shapes;
using System.Windows.Data;
#else
using Windows.UI.Xaml;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Shapes;
using Windows.UI.Xaml.Data;
using Windows.UI;
#endif

namespace Syncfusion.UI.Xaml.Charts
{
    public class MarkerTemplateSelector : SegmentTemplateSelector
    {
        #region fields

        Binding defaultBinding;

        #endregion

        #region methods

        protected internal override DataTemplate SelectTemplate(double x, double y)
        {
            return MarkerTemplate;
        }

        internal override void BindVisual(double x, double y, Shape marker)
        {
            base.BindVisual(x, y, marker);
            if (MarkerBrush != null)
            {
                if (NegativePointBrush == null)
                {
                    if (defaultBinding == null)
                        BindFillProperty("MarkerBrush", marker, ref defaultBinding);
                    else
                        marker.SetBinding(Shape.FillProperty, defaultBinding);
                }
                else
                {
                    if (y >= 0)
                    {
                        if (defaultBinding == null)
                            BindFillProperty("MarkerBrush", marker, ref defaultBinding);
                        else
                            marker.SetBinding(Shape.FillProperty, defaultBinding);
                    }
                }
            }
            base.BindVisual(x, y, marker);
            BindVisualSize(marker);
        }

        private static void OnMarkerTemplateChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            (d as MarkerTemplateSelector).ClearMarkerPresenter();
        }

        private void ClearMarkerPresenter()
        {
            MarkerBase markerObj = Sparkline as MarkerBase;
            if (markerObj != null)
                markerObj.MarkerPresenter.Children.Clear();
        }

        private void BindVisualSize(Shape elelment)
        {
            Binding binding = new Binding();
            binding.Source = this;
            binding.Path = new PropertyPath("MarkerWidth");
            elelment.SetBinding(Shape.WidthProperty, binding);

            binding = new Binding();
            binding.Source = this;
            binding.Path = new PropertyPath("MarkerHeight");
            elelment.SetBinding(Shape.HeightProperty, binding);
        }

        #endregion

        #region properties

        /// <summary>
        /// Gets or Sets the brush to paint the markers of the sparkline.
        /// </summary>
        public Brush MarkerBrush
        {
            get { return (Brush)GetValue(MarkerBrushProperty); }
            set { SetValue(MarkerBrushProperty, value); }
        }

        // Using a DependencyProperty as the backing store for MarkerBrush.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty MarkerBrushProperty =
            DependencyProperty.Register("MarkerBrush", typeof(Brush), typeof(MarkerTemplateSelector), new PropertyMetadata(new SolidColorBrush(Color.FromArgb(0xFF, 0x1B, 0xA1, 0xE2))));

        /// <summary>
        /// Gets or sets the marker template.
        /// </summary>
        public DataTemplate MarkerTemplate
        {
            get { return (DataTemplate)GetValue(MarkerTemplateProperty); }
            set { SetValue(MarkerTemplateProperty, value); }
        }

        // Using a DependencyProperty as the backing store for MarkerTemplate.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty MarkerTemplateProperty =
            DependencyProperty.Register("MarkerTemplate", typeof(DataTemplate), typeof(MarkerTemplateSelector), new PropertyMetadata(null, OnMarkerTemplateChanged));

        /// <summary>
        /// Gets or sets height of the marker.
        /// </summary>
        public double MarkerHeight
        {
            get { return (double)GetValue(MarkerHeightProperty); }
            set { SetValue(MarkerHeightProperty, value); }
        }

        // Using a DependencyProperty as the backing store for MarkerHeight.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty MarkerHeightProperty =
            DependencyProperty.Register("MarkerHeight", typeof(double), typeof(MarkerTemplateSelector), new PropertyMetadata(5d));

        /// <summary>
        /// Gets or sets width of the marker.
        /// </summary>
        public double MarkerWidth
        {
            get { return (double)GetValue(MarkerWidthProperty); }
            set { SetValue(MarkerWidthProperty, value); }
        }

        // Using a DependencyProperty as the backing store for MarkerWidth.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty MarkerWidthProperty =
            DependencyProperty.Register("MarkerWidth", typeof(double), typeof(MarkerTemplateSelector), new PropertyMetadata(5d));


        #endregion
    }
}
