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
    public class SegmentTemplateSelector : TemplateSelector
    {
        #region members

        Binding firstPointBinding, lastPointBinding, negativePointBinding, highPointBinding, lowPointBinding;

        #endregion

        #region properties

        /// <summary>
        /// Gets or Sets the brush to paint the first point of the sparkline.
        /// </summary>
        public Brush FirstPointBrush
        {
            get { return (Brush)GetValue(FirstPointBrushProperty); }
            set { SetValue(FirstPointBrushProperty, value); }
        }

        // Using a DependencyProperty as the backing store for FirstPointBrush.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty FirstPointBrushProperty =
            DependencyProperty.Register("FirstPointBrush", typeof(Brush), typeof(SegmentTemplateSelector), new PropertyMetadata(new SolidColorBrush(Color.FromArgb(0xFF, 0x1B, 0xA1, 0xE2)), OnMarkerTemplateChanged));

        /// <summary>
        /// Gets or Sets the brush to paint the last point of the sparkline.
        /// </summary>
        public Brush LastPointBrush
        {
            get { return (Brush)GetValue(LastPointBrushProperty); }
            set { SetValue(LastPointBrushProperty, value); }
        }

        // Using a DependencyProperty as the backing store for LastPointBrush.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty LastPointBrushProperty =
            DependencyProperty.Register("LastPointBrush", typeof(Brush), typeof(SegmentTemplateSelector), new PropertyMetadata(new SolidColorBrush(Color.FromArgb(0xFF, 0x1B, 0xA1, 0xE2))));

        /// <summary>
        /// Gets or Sets the brush to paint the negative points of the sparkline.
        /// </summary>
        public Brush NegativePointBrush
        {
            get { return (Brush)GetValue(NegativePointBrushProperty); }
            set { SetValue(NegativePointBrushProperty, value); }
        }

        // Using a DependencyProperty as the backing store for NegativePointBrush.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty NegativePointBrushProperty =
            DependencyProperty.Register("NegativePointBrush", typeof(Brush), typeof(SegmentTemplateSelector), new PropertyMetadata(new SolidColorBrush(Color.FromArgb(0xFF, 0x1B, 0xA1, 0xE2))));

        /// <summary>
        /// Gets or Sets the brush to paint the high points of the sparkline.
        /// </summary>
        public Brush HighPointBrush
        {
            get { return (Brush)GetValue(HighPointBrushProperty); }
            set { SetValue(HighPointBrushProperty, value); }
        }

        // Using a DependencyProperty as the backing store for HighPointBrush.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty HighPointBrushProperty =
            DependencyProperty.Register("HighPointBrush", typeof(Brush), typeof(SegmentTemplateSelector), new PropertyMetadata(new SolidColorBrush(Color.FromArgb(0xFF, 0x1B, 0xA1, 0xE2))));

        /// <summary>
        /// Gets or Sets the brush to paint the low point(s) of the sparkline.
        /// </summary>
        public Brush LowPointBrush
        {
            get { return (Brush)GetValue(LowPointBrushProperty); }
            set { SetValue(LowPointBrushProperty, value); }
        }

        // Using a DependencyProperty as the backing store for LowPointBrush.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty LowPointBrushProperty =
            DependencyProperty.Register("LowPointBrush", typeof(Brush), typeof(SegmentTemplateSelector), new PropertyMetadata(new SolidColorBrush(Color.FromArgb(0xFF, 0x1B, 0xA1, 0xE2))));

        #endregion

        #region methods

        internal virtual void BindVisual(double x, double y, Shape marker)
        {
            if (y < 0 && NegativePointBrush != null)
                if (negativePointBinding == null)
                    BindFillProperty("NegativePointBrush", marker, ref negativePointBinding);
                else
                    marker.SetBinding(Shape.FillProperty, negativePointBinding);

            if (x == MinimumX && FirstPointBrush != null)
                if (firstPointBinding == null)
                    BindFillProperty("FirstPointBrush", marker, ref firstPointBinding);
                else
                    marker.SetBinding(Shape.FillProperty, firstPointBinding);

            else if (x == DataCount - 1 && LastPointBrush != null)
                if (lastPointBinding == null)
                    BindFillProperty("LastPointBrush", marker, ref lastPointBinding);
                else
                    marker.SetBinding(Shape.FillProperty, lastPointBinding);

            else if (y == MaximumY && HighPointBrush != null)
                if (highPointBinding == null)
                    BindFillProperty("HighPointBrush", marker, ref highPointBinding);
                else
                    marker.SetBinding(Shape.FillProperty, highPointBinding);

            else if (y == MinimumY && LowPointBrush != null)
                if (lowPointBinding == null)
                    BindFillProperty("LowPointBrush", marker, ref lowPointBinding);
                else
                    marker.SetBinding(Shape.FillProperty, lowPointBinding);
        }

        protected void BindFillProperty(string propertyName, Shape marker, ref Binding binding)
        {
            binding = new Binding();
            binding.Source = this;
            binding.Path = new PropertyPath(propertyName);
            marker.SetBinding(Shape.FillProperty, binding);
        }

        #endregion

        private static void OnMarkerTemplateChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
        }
    }
}
