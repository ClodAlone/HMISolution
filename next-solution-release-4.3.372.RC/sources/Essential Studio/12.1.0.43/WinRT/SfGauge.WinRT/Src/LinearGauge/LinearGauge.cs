#region Copyright Syncfusion Inc. 2001 - 2014
// Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Any infringement will be prosecuted under
// applicable laws. 
#endregion
#if WINRT
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Media;
using Windows.Foundation;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Markup;
using System.Threading.Tasks;
#else
using System.Windows;
using System.Windows.Media;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Markup;
#endif

namespace Syncfusion.UI.Xaml.Gauges
{
    #region SfLinearGauge

    [TemplatePart(Name = "PART_LinearGaugeGrid", Type = typeof(Grid))]
    [TemplatePart(Name = "PART_LinearScalePresenter", Type = typeof(ContentPresenter))]

    public class SfLinearGauge : Control
    {
        #region Constructor

        public SfLinearGauge()
        {
            DefaultStyleKey = typeof(SfLinearGauge);
            SizeChanged += LinearGauge_SizeChanged;
        }

        #endregion

        #region Public Dependency Properties

        #region Orientation
        public Orientation Orientation
        {
            get { return (Orientation)GetValue(OrientationProperty); }
            set { SetValue(OrientationProperty, value); }
        }

        public static readonly DependencyProperty OrientationProperty =
            DependencyProperty.Register("Orientation", typeof(Orientation), typeof(SfLinearGauge), new PropertyMetadata(Orientation.Horizontal, OnOrientationChanged));

        private static void OnOrientationChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var linearGauge = (d as SfLinearGauge);
            if (linearGauge != null)
            {
                SetOrientationAndScaleDirection(linearGauge);
                if (linearGauge.MainScale != null)
                    linearGauge.MainScale.ResetScale();
            }
        }
        #endregion

        #region MainScale
        public LinearScale MainScale
        {
            get { return (LinearScale)GetValue(MainScaleProperty); }
            set
            {
                value.ParentGauge = this;
                SetValue(MainScaleProperty, value);
            }
        }

        public static readonly DependencyProperty MainScaleProperty =
            DependencyProperty.Register("MainScale", typeof(LinearScale), typeof(SfLinearGauge), new PropertyMetadata(null, OnMainScaleChanged
                ));

        private static void OnMainScaleChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d is SfLinearGauge)
            {
                (e.NewValue as LinearScale).ParentGauge = d as SfLinearGauge;
            }
        }
        #endregion

        #endregion

        #region Internal Dependency Properties

        #region GaugeSize
        public Size GaugeSize
        {
            get { return (Size)GetValue(GaugeSizeProperty); }
            set { SetValue(GaugeSizeProperty, value); }
        }

        // Using a DependencyProperty as the backing store for GaugeSize.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty GaugeSizeProperty =
            DependencyProperty.Register("GaugeSize", typeof(Size), typeof(SfLinearGauge), new PropertyMetadata(null));
        #endregion

        #region Transform
        internal TransformGroup Transform
        {
            get { return (TransformGroup)GetValue(TransformProperty); }
            set { SetValue(TransformProperty, value); }
        }

        // Using a DependencyProperty as the backing store for Transform.  This enables animation, styling, binding, etc...
        internal static readonly DependencyProperty TransformProperty =
            DependencyProperty.Register("Transform", typeof(TransformGroup), typeof(SfLinearGauge), new PropertyMetadata(null));
        #endregion

        #region TransformOrigin
        internal Point TransformOrigin
        {
            get { return (Point)GetValue(TransformOriginProperty); }
            set { SetValue(TransformOriginProperty, value); }
        }

        // Using a DependencyProperty as the backing store for TransformOrigin.  This enables animation, styling, binding, etc...
        internal static readonly DependencyProperty TransformOriginProperty =
            DependencyProperty.Register("TransformOrigin", typeof(Point), typeof(SfLinearGauge), new PropertyMetadata(new Point()));
        #endregion

        #endregion

        #region Override Methods

        protected override Size MeasureOverride(Size availableSize)
        {
            GaugeSize = availableSize;
            if(MainScale != null)
                MainScale.ResetScale();
            SetOrientationAndScaleDirection(this);
            return base.MeasureOverride(availableSize);
        }

        #endregion

        #region Implementation

        internal static void SetOrientationAndScaleDirection(SfLinearGauge linearGauge)
        {
            if (linearGauge.Orientation == Orientation.Vertical)
            {
                linearGauge.TransformOrigin = new Point(0.5, 0.5);
                var transform = new TransformGroup();
                if (linearGauge.MainScale != null)
                {
                    if (linearGauge.MainScale.ScaleDirection == LinearScaleDirection.Forward)
                    {
                        transform.Children.Add(new RotateTransform { Angle = 90 });
                        transform.Children.Add(new ScaleTransform { ScaleX = -1 });
                    }
                    else
                    {
                        transform.Children.Add(new RotateTransform { Angle = -90 });
                    }
                }
                else
                    transform.Children.Add(new RotateTransform { Angle = 90 });

                linearGauge.Transform = transform;
            }
            else
            {
                var transform = new TransformGroup();
                if (linearGauge.MainScale != null)
                {
                    if (linearGauge.MainScale.ScaleDirection == LinearScaleDirection.Forward)
                    {
                        linearGauge.TransformOrigin = new Point();
                    }
                    else
                    {
                        linearGauge.TransformOrigin = new Point(0.5, 0.5);
                        transform.Children.Add(new ScaleTransform { ScaleX = -1 });
                    }
                }
                linearGauge.Transform = transform;
            }
            if (linearGauge.MainScale != null)
            {
                linearGauge.MainScale.SetTicksAndLabels();
            }
        }

        void LinearGauge_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            GaugeSize = e.NewSize;
            if (MainScale != null)
            {
                MainScale.ResetScale();
            }
        }

        #endregion
    }

    #endregion
}
