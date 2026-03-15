#region Copyright Syncfusion Inc. 2001 - 2014
// Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Any infringement will be prosecuted under
// applicable laws. 
#endregion
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;

#if WINRT
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Media;
using Windows.Foundation;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Markup;
using System.Threading.Tasks;
using Windows.UI.Xaml.Media.Animation;
using Windows.UI;
using System.Collections;
#else
using System.Windows;
using System.Windows.Media;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Markup;
using System.Windows.Media.Animation;
using System.Windows.Threading;
#endif


namespace Syncfusion.UI.Xaml.Gauges
{


    /// <summary>
    /// <para>CircularPointer is a class that includes properties and methods to define
    /// different types of pointers</para>
    /// </summary>
    public class CircularPointer : DependencyObject
    {
#if WINRT
        internal static ResourceDictionary resourcedictionary = new ResourceDictionary() { Source = new Uri("ms-appx:///Syncfusion.SfGauge.WinRT/CircularGauge/Themes/CircularGauge.xaml", UriKind.RelativeOrAbsolute) };
#elif WINDOWSPHONE_8
        internal static ResourceDictionary resourcedictionary = new ResourceDictionary() { Source = new Uri("/Syncfusion.SfGauge.WP8;component/CircularGauge/Themes/CircularGauge.xaml", UriKind.Relative) };
#elif WINDOWSPHONE_7
        internal static ResourceDictionary resourcedictionary = new ResourceDictionary() { Source = new Uri("/Syncfusion.SfGauge.WP7;component/CircularGauge/Themes/CircularGauge.xaml", UriKind.Relative) };

#elif SILVERLIGHT
        internal static ResourceDictionary resourcedictionary = new ResourceDictionary() { Source = new Uri("/Syncfusion.SfGauge.Silverlight;component/CircularGauge/Themes/CircularGauge.xaml", UriKind.Relative) };
#else
        internal static ResourceDictionary resourcedictionary = new ResourceDictionary() { Source = new Uri("/Syncfusion.SfGauge.WPF;component/CircularGauge/Themes/CircularGauge.xaml", UriKind.Relative) };
#endif

#if WPF
        internal bool IsDefaultPointer;
#endif
        /// <summary>
        /// Initializes a new instance of the <see
        /// cref="T:Syncfusion.UI.Xaml.Gauges.CircularPointer"/> class.
        /// </summary>
        public CircularPointer()
        {
            SetSymbolForPointer();

            Timer = new DispatcherTimer();
            Timer.Interval = new TimeSpan(0, 0, 0, 0, 1);
            Timer.Tick += timer_Tick;
            Rangepointer = new PathSegmentCollection();
            ArcSegment arcseg = new ArcSegment() { RotationAngle = 0 };
            Binding sweepbinding = new Binding() { Path = new PropertyPath("SweepDirection"), Source = this, Mode = BindingMode.TwoWay };
            Binding islargearcbinding = new Binding() { Path = new PropertyPath("IsLargeArc"), Source = this, Mode = BindingMode.TwoWay };
            Binding halfsizebinding = new Binding() { Path = new PropertyPath("HalfSize"), Source = this, Mode = BindingMode.TwoWay };
            Binding rangeendptbinding = new Binding() { Path = new PropertyPath("RangePointerEndPoint"), Source = this, Mode = BindingMode.TwoWay };
            BindingOperations.SetBinding(arcseg, ArcSegment.SweepDirectionProperty, sweepbinding);
            BindingOperations.SetBinding(arcseg, ArcSegment.IsLargeArcProperty, islargearcbinding);
            BindingOperations.SetBinding(arcseg, ArcSegment.SizeProperty, halfsizebinding);
            BindingOperations.SetBinding(arcseg, ArcSegment.PointProperty, rangeendptbinding);
            Rangepointer.Add(arcseg);
            NeedleTranform = new RotateTransform();
            Binding transformbinding = new Binding() { Path = new PropertyPath("Angle"), Source = this.NeedleTranform, Mode = BindingMode.TwoWay };
            BindingOperations.SetBinding(this, CircularPointer.AngleProperty, transformbinding);
            sb.Children.Add(angleanimation);
            Storyboard.SetTarget(angleanimation, NeedleTranform);
#if WINRT
            Storyboard.SetTargetProperty(angleanimation, "Angle"); 
#else
            Storyboard.SetTargetProperty(angleanimation, new PropertyPath("Angle"));            
#endif
        }

        private void timer_Tick(object sender, object e)
        {
            double tValue = ValueDiff / 10;
            if ((Math.Round(this.TimeValue, 5) < this.Value && tValue > 0) || (Math.Round(this.TimeValue, 5) > this.Value && tValue < 0))//&& this.Value - Math.Round(this.TimeValue, 5) >= Math.Round(tValue, 5))
               this.TimeValue = Math.Round(TimeValue,5) + Math.Round( tValue,5);
           else
               Timer.Stop();
        }


        internal Storyboard sb = new Storyboard();
        internal DoubleAnimation angleanimation = new DoubleAnimation() {  Duration = new Duration(TimeSpan.FromMilliseconds(500))};
        internal void SetProperties()
        {
            Angle = ValueToAngle(TimeValue);
            if (ParentSize.Width > 0 && AvailSize.Width >0)
            {
                NeedleLength = ParentSize.Width/2 * NeedleLengthFactor;
                double radX = AvailSize.Width / 2 ;
                double radY = AvailSize.Height / 2;
                double angle = Angle;
                if (this.SweepDirection == SweepDirection.Clockwise)
                {
                    if ((angle % 360) == ParentStartAngle && ParentStartAngle != angle)
                    {
                        angle = (ParentStartAngle + 359.99) % 360;
                    }
                    IsLargeArc = Angle - ParentStartAngle > 180;
                }
                else
                {
                    if ((angle + 360) == ParentStartAngle && ParentStartAngle != angle)
                    {
                        angle = (ParentStartAngle - 359.99) % 360;
                    }
                    IsLargeArc = angle + 360 - ParentStartAngle < 180;
                }
                RangePointerStartPoint = new Point(radX + radX * Math.Cos(DegToRad(ParentStartAngle)), radY + radY * Math.Sin(DegToRad(ParentStartAngle)));
                RangePointerEndPoint = new Point(radX + radX * Math.Cos(DegToRad(angle)), radY + radY * Math.Sin(DegToRad(angle)));
                HalfSize = new Size(radX, radY);
                
            }
        }

        internal Thickness RangePointerMargin
        {
            get { return (Thickness)GetValue(RangePointerMarginProperty); }
            set { SetValue(RangePointerMarginProperty, value); }
        }

        // Using a DependencyProperty as the backing store for RangePointerMargin.  This enables animation, styling, binding, etc...
        internal static readonly DependencyProperty RangePointerMarginProperty =
            DependencyProperty.Register("RangePointerMargin", typeof(Thickness), typeof(CircularPointer), new PropertyMetadata(new Thickness(0d)));



        internal PathSegmentCollection Rangepointer
        {
            get { return (PathSegmentCollection)GetValue(RangepointerProperty); }
            set { SetValue(RangepointerProperty, value); }
        }

        // Using a DependencyProperty as the backing store for Rangepointer.  This enables animation, styling, binding, etc...
        internal static readonly DependencyProperty RangepointerProperty =
            DependencyProperty.Register("Rangepointer", typeof(PathSegmentCollection), typeof(CircularPointer), new PropertyMetadata(new PathSegmentCollection()));

        

        internal Thickness SymbolPointerMargin
        {
            get { return (Thickness)GetValue(SymbolPointerMarginProperty); }
            set { SetValue(SymbolPointerMarginProperty, value); }
        }

        // Using a DependencyProperty as the backing store for SymbolPointerMargin.  This enables animation, styling, binding, etc...
        internal static readonly DependencyProperty SymbolPointerMarginProperty =
            DependencyProperty.Register("SymbolPointerMargin", typeof(Thickness), typeof(CircularPointer), new PropertyMetadata(new Thickness(10d)));


        /// <summary>
        /// Gets or sets the thickness of the RangePointer of the CircularPointer.
        /// </summary>
        /// <value>
        /// double
        /// </value>
        /// <example>
        /// <code language="C#">
        /// using System;
        /// using System.Collections.Generic;
        /// using System.IO;
        /// using System.Linq;
        /// using Windows.Foundation;
        /// using Windows.Foundation.Collections;
        /// using Windows.UI.Xaml;
        /// using Windows.UI.Xaml.Controls;
        /// using Windows.UI.Xaml.Controls.Primitives;
        /// using Windows.UI.Xaml.Data;
        /// using Windows.UI.Xaml.Input;
        /// using Windows.UI.Xaml.Media;
        /// using Windows.UI.Xaml.Navigation;
        /// using Common;
        /// using Syncfusion.UI.Xaml.Gauges;
        /// 
        /// namespace GaugeWinRTSamples
        /// {
        ///     public sealed partial class GaugePosition :SampleView
        ///     {
        ///         public GaugePosition()
        ///         {
        ///            this.InitializeComponent();
        ///            SfCircularGauge gauge = new SfCircularGauge();
        ///            CircularPointer pointer = new CircularPointer();
        ///            pointer.Value = 80;
        ///            pointer.PointerType = PointerType.RangePointer;
        ///            pointer.RangePointerStrokeThickness = 10;
        ///            gauge.MainScale.Pointers.Add(pointer);
        ///         }
        ///     }
        /// }
        /// 
        /// </code>
        /// </example>
        [ClassReference(IsReviewed = false)]
        public double RangePointerStrokeThickness
        {
            get { return (double)GetValue(RangePointerStrokeThicknessProperty); }
            set { SetValue(RangePointerStrokeThicknessProperty, value); }
        }

        // Using a DependencyProperty as the backing store for RangePointerStrokeThickness.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty RangePointerStrokeThicknessProperty =
            DependencyProperty.Register("RangePointerStrokeThickness", typeof(double), typeof(CircularPointer), new PropertyMetadata(8d, OnRangePointerStrokeThicknessPropertyChanged));

        private static void OnRangePointerStrokeThicknessPropertyChanged(DependencyObject obj, DependencyPropertyChangedEventArgs args)
        {
            CircularPointer circularpointer = obj as CircularPointer;
            if (!(circularpointer.RangePointerStrokeThickness >= 0))
                circularpointer.RangePointerStrokeThickness = 0;
            if(!(circularpointer.RangePointerStrokeThickness<=40))
                circularpointer.RangePointerStrokeThickness=40;
        }

        /// <summary>
        /// Gets or sets the brush that describes the RangePointerStroke of the CircularPointer.
        /// </summary>
        /// <value>
        /// Brush
        /// </value>
        /// <example>
        /// <code language="C#">
        /// using System;
        /// using System.Collections.Generic;
        /// using System.IO;
        /// using System.Linq;
        /// using Windows.Foundation;
        /// using Windows.Foundation.Collections;
        /// using Windows.UI.Xaml;
        /// using Windows.UI.Xaml.Controls;
        /// using Windows.UI.Xaml.Controls.Primitives;
        /// using Windows.UI.Xaml.Data;
        /// using Windows.UI.Xaml.Input;
        /// using Windows.UI.Xaml.Media;
        /// using Windows.UI.Xaml.Navigation;
        /// using Common;
        /// using Syncfusion.UI.Xaml.Gauges;
        /// 
        /// namespace GaugeWinRTSamples
        /// {
        ///     public sealed partial class GaugePosition :SampleView
        ///     {
        ///         public GaugePosition()
        ///         {
        ///            this.InitializeComponent();
        ///            SfCircularGauge gauge = new SfCircularGauge();
        ///            CircularPointer pointer = new CircularPointer();
        ///            pointer.Value = 80;
        ///            pointer.PointerType = PointerType.RangePointer;
        ///            pointer.RangePointerStroke = new SolidColorBrush(Colors.White);
        ///            gauge.MainScale.Pointers.Add(pointer);
        ///         }
        ///     }
        /// }
        /// </code>
        /// </example>
        [ClassReference(IsReviewed = false)]
        public Brush RangePointerStroke
        {
            get { return (Brush)GetValue(RangePointerStrokeProperty); }
            set { SetValue(RangePointerStrokeProperty, value); }
        }

        // Using a DependencyProperty as the backing store for Stroke.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty RangePointerStrokeProperty =
            DependencyProperty.Register("RangePointerStroke", typeof(Brush), typeof(CircularPointer), new PropertyMetadata(new SolidColorBrush(Colors.Orange)));

        /// <summary>
        /// Gets or sets the brush that describes the SymbolPointerStroke of the CircularPointer.
        /// </summary>
        /// <value>
        /// Brush
        /// </value>
        /// <example>
        /// <code language="C#">
        /// using System;
        /// using System.Collections.Generic;
        /// using System.IO;
        /// using System.Linq;
        /// using Windows.Foundation;
        /// using Windows.Foundation.Collections;
        /// using Windows.UI.Xaml;
        /// using Windows.UI.Xaml.Controls;
        /// using Windows.UI.Xaml.Controls.Primitives;
        /// using Windows.UI.Xaml.Data;
        /// using Windows.UI.Xaml.Input;
        /// using Windows.UI.Xaml.Media;
        /// using Windows.UI.Xaml.Navigation;
        /// using Common;
        /// using Syncfusion.UI.Xaml.Gauges;
        /// 
        /// namespace GaugeWinRTSamples
        /// {
        ///     public sealed partial class GaugePosition :SampleView
        ///     {
        ///         public GaugePosition()
        ///         {
        ///            this.InitializeComponent();
        ///            SfCircularGauge gauge = new SfCircularGauge();
        ///            CircularPointer pointer = new CircularPointer();
        ///            pointer.Value = 80;
        ///            pointer.PointerType = PointerType.SymbolPointer;
        ///            pointer.SymbolPointerStroke = new SolidColorBrush(Colors.White);
        ///            gauge.MainScale.Pointers.Add(pointer);
        ///         }
        ///     }
        /// }
        /// </code>
        /// </example>
        [ClassReference(IsReviewed = false)]
        public Brush SymbolPointerStroke
        {
            get { return (Brush)GetValue(SymbolPointerStrokeProperty); }
            set { SetValue(SymbolPointerStrokeProperty, value); }
        }

        public static readonly DependencyProperty SymbolPointerStrokeProperty =
            DependencyProperty.Register("SymbolPointerStroke", typeof(Brush), typeof(CircularPointer), new PropertyMetadata(new SolidColorBrush(Colors.Orange)));

        /// <summary>
        /// Gets or sets the thickness value of the NeedlePointer of the CircularPointer.
        /// </summary>
        /// <value>
        /// double
        /// </value>
        /// <example>
        /// <code language="C#">
        /// using System;
        /// using System.Collections.Generic;
        /// using System.IO;
        /// using System.Linq;
        /// using Windows.Foundation;
        /// using Windows.Foundation.Collections;
        /// using Windows.UI.Xaml;
        /// using Windows.UI.Xaml.Controls;
        /// using Windows.UI.Xaml.Controls.Primitives;
        /// using Windows.UI.Xaml.Data;
        /// using Windows.UI.Xaml.Input;
        /// using Windows.UI.Xaml.Media;
        /// using Windows.UI.Xaml.Navigation;
        /// using Common;
        /// using Syncfusion.UI.Xaml.Gauges;
        /// 
        /// namespace GaugeWinRTSamples
        /// {
        ///     public sealed partial class GaugePosition :SampleView
        ///     {
        ///         public GaugePosition()
        ///         {
        ///            this.InitializeComponent();
        ///            SfCircularGauge gauge = new SfCircularGauge();
        ///            CircularPointer pointer = new CircularPointer();
        ///            pointer.Value = 80;
        ///            pointer.PointerType = PointerType.NeedlePointer;
        ///            pointer.NeedlePointerStrokeThickness = 2;
        ///            gauge.MainScale.Pointers.Add(pointer);
        ///         }
        ///     }
        /// }
        /// 
        /// </code>
        /// </example>
        [ClassReference(IsReviewed = false)]
        public double NeedlePointerStrokeThickness
        {
            get { return (double)GetValue(NeedlePointerStrokeThicknessProperty); }
            set { SetValue(NeedlePointerStrokeThicknessProperty, value); }
        }

        public static readonly DependencyProperty NeedlePointerStrokeThicknessProperty =
            DependencyProperty.Register("NeedlePointerStrokeThickness", typeof(double), typeof(CircularPointer), new PropertyMetadata(3d));

        /// <summary>
        /// Gets or sets the brush that describes the NeedlePointerStroke of the CircularPointer.
        /// </summary>
        /// <value>
        /// Brush
        /// </value>
        /// <example>
        /// <code language="C#">
        /// using System;
        /// using System.Collections.Generic;
        /// using System.IO;
        /// using System.Linq;
        /// using Windows.Foundation;
        /// using Windows.Foundation.Collections;
        /// using Windows.UI.Xaml;
        /// using Windows.UI.Xaml.Controls;
        /// using Windows.UI.Xaml.Controls.Primitives;
        /// using Windows.UI.Xaml.Data;
        /// using Windows.UI.Xaml.Input;
        /// using Windows.UI.Xaml.Media;
        /// using Windows.UI.Xaml.Navigation;
        /// using Common;
        /// using Syncfusion.UI.Xaml.Gauges;
        /// 
        /// namespace GaugeWinRTSamples
        /// {
        ///     public sealed partial class GaugePosition :SampleView
        ///     {
        ///         public GaugePosition()
        ///         {
        ///            this.InitializeComponent();
        ///            SfCircularGauge gauge = new SfCircularGauge();
        ///            CircularPointer pointer = new CircularPointer();
        ///            pointer.Value = 80;
        ///            pointer.PointerType = PointerType.NeedlePointer;
        ///            pointer.NeedlePointerStroke = new SolidColorBrush(Colors.White);
        ///            gauge.MainScale.Pointers.Add(pointer);
        ///         }
        ///     }
        /// }
        /// </code>
        /// </example>
        [ClassReference(IsReviewed = false)]
        public Brush NeedlePointerStroke
        {
            get { return (Brush)GetValue(NeedlePointerStrokeProperty); }
            set { SetValue(NeedlePointerStrokeProperty, value); }
        }

        public static readonly DependencyProperty NeedlePointerStrokeProperty =
            DependencyProperty.Register("NeedlePointerStroke", typeof(Brush), typeof(CircularPointer), new PropertyMetadata(new SolidColorBrush(Colors.Red)));

        /// <summary>
        /// Gets or sets the brush that describes the PointerCapStroke of the CircularPointer.
        /// </summary>
        /// <value>
        /// Brush
        /// </value>
        /// <example>
        /// <code language="C#">
        /// using System;
        /// using System.Collections.Generic;
        /// using System.IO;
        /// using System.Linq;
        /// using Windows.Foundation;
        /// using Windows.Foundation.Collections;
        /// using Windows.UI.Xaml;
        /// using Windows.UI.Xaml.Controls;
        /// using Windows.UI.Xaml.Controls.Primitives;
        /// using Windows.UI.Xaml.Data;
        /// using Windows.UI.Xaml.Input;
        /// using Windows.UI.Xaml.Media;
        /// using Windows.UI.Xaml.Navigation;
        /// using Common;
        /// using Syncfusion.UI.Xaml.Gauges;
        /// 
        /// namespace GaugeWinRTSamples
        /// {
        ///     public sealed partial class GaugePosition :SampleView
        ///     {
        ///         public GaugePosition()
        ///         {
        ///            this.InitializeComponent();
        ///            SfCircularGauge gauge = new SfCircularGauge();
        ///            CircularPointer pointer = new CircularPointer();
        ///            pointer.Value = 80;
        ///            pointer.PointerType = PointerType.NeedlePointer;
        ///            pointer.PointerCapStroke = new SolidColorBrush(Colors.White);
        ///            gauge.MainScale.Pointers.Add(pointer);
        ///         }
        ///     }
        /// }
        /// 
        /// </code>
        /// </example>
        [ClassReference(IsReviewed = false)]
        public Brush PointerCapStroke
        {
            get { return (Brush)GetValue(PointerCapStrokeProperty); }
            set { SetValue(PointerCapStrokeProperty, value); }
        }

        public static readonly DependencyProperty PointerCapStrokeProperty =
            DependencyProperty.Register("PointerCapStroke", typeof(Brush), typeof(CircularPointer), new PropertyMetadata(new SolidColorBrush(Colors.Green)));

        /// <summary>
        /// Gets or sets user interface (UI) visibility of RangePointer.
        /// </summary>
        /// <remarks>
        /// There are three types of pointers. While choosing a pointer using the
        /// PointerType property the other two pointers&apos; visibility property will be
        /// set to Collapsed. Default value is Visible.
        /// </remarks>
        /// <value>
        /// Visibility
        /// </value>
        /// <example>
        /// <code language="C#">
        /// using System;
        /// using System.Collections.Generic;
        /// using System.IO;
        /// using System.Linq;
        /// using Windows.Foundation;
        /// using Windows.Foundation.Collections;
        /// using Windows.UI.Xaml;
        /// using Windows.UI.Xaml.Controls;
        /// using Windows.UI.Xaml.Controls.Primitives;
        /// using Windows.UI.Xaml.Data;
        /// using Windows.UI.Xaml.Input;
        /// using Windows.UI.Xaml.Media;
        /// using Windows.UI.Xaml.Navigation;
        /// using Common;
        /// using Syncfusion.UI.Xaml.Gauges;
        /// 
        /// namespace GaugeWinRTSamples
        /// {
        ///     public sealed partial class GaugePosition :SampleView
        ///     {
        ///         public GaugePosition()
        ///         {
        ///            this.InitializeComponent();
        ///            SfCircularGauge gauge = new SfCircularGauge();
        ///            CircularPointer needlePointer = new CircularPointer();
        ///            needlePointer.Value = 80;
        ///            needlePointer.PointerType = PointerType.NeedlePointer;
        ///	           needlePointer.RangePointerVisibility = Visibility.Collapsed;
        ///            gauge.MainScale.Pointers.Add(needlePointer);
        ///         }
        ///     }
        /// }
        /// 
        /// </code>
        /// </example>
        [ClassReference(IsReviewed = false)]
        public Visibility RangePointerVisibility
        {
            get { return (Visibility)GetValue(RangePointerVisibilityProperty); }
            set { SetValue(RangePointerVisibilityProperty, value); }
        }

        // Using a DependencyProperty as the backing store for RangePointerVisibility.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty RangePointerVisibilityProperty =
            DependencyProperty.Register("RangePointerVisibility", typeof(Visibility), typeof(CircularPointer), new PropertyMetadata(Visibility.Collapsed));

        /// <summary>
        /// Gets or sets the user interface (UI) visibility of SymbolPointer.
        /// </summary>
        /// <remarks>
        /// There are three types of pointers. While choosing a pointer using the
        /// PointerType property the other two pointers&apos; visibility property will be
        /// set to Collapsed. Default value is Visible.
        /// </remarks>
        /// <value>
        /// Visibility
        /// </value>
        /// <example>
        /// <code language="C#">
        /// using System;
        /// using System.Collections.Generic;
        /// using System.IO;
        /// using System.Linq;
        /// using Windows.Foundation;
        /// using Windows.Foundation.Collections;
        /// using Windows.UI.Xaml;
        /// using Windows.UI.Xaml.Controls;
        /// using Windows.UI.Xaml.Controls.Primitives;
        /// using Windows.UI.Xaml.Data;
        /// using Windows.UI.Xaml.Input;
        /// using Windows.UI.Xaml.Media;
        /// using Windows.UI.Xaml.Navigation;
        /// using Common;
        /// using Syncfusion.UI.Xaml.Gauges;
        /// 
        /// namespace GaugeWinRTSamples
        /// {
        ///     public sealed partial class GaugePosition :SampleView
        ///     {
        ///         public GaugePosition()
        ///         {
        ///            this.InitializeComponent();
        ///            SfCircularGauge gauge = new SfCircularGauge();
        ///            CircularPointer needlePointer = new CircularPointer();
        ///            needlePointer.Value = 80;
        ///            needlePointer.PointerType = PointerType.NeedlePointer;
        ///	           needlePointer.SymbolPointerVisibility = Visibility.Collapsed;
        ///            gauge.MainScale.Pointers.Add(needlePointer);
        ///         }
        ///     }
        /// }
        /// 
        /// </code>
        /// </example>
        [ClassReference(IsReviewed = false)]
        public Visibility SymbolPointerVisibility
        {
            get { return (Visibility)GetValue(SymbolPointerVisibilityProperty); }
            set { SetValue(SymbolPointerVisibilityProperty, value); }
        }

        // Using a DependencyProperty as the backing store for SymbolPointerVisibility.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty SymbolPointerVisibilityProperty =
            DependencyProperty.Register("SymbolPointerVisibility", typeof(Visibility), typeof(CircularPointer), new PropertyMetadata(Visibility.Collapsed));



        /// <summary>
        /// Gets or sets user interface visibility of the NeedlePointer.
        /// </summary>
        /// <remarks>
        /// There are three types of pointers. While choosing a pointer using the
        /// PointerType property the other two pointers&apos; visibility property will be
        /// set to Collapsed. Default value is Visible.
        /// </remarks>
        /// <value>
        /// Visibility
        /// </value>
        /// <example>
        /// <code language="C#">
        /// using System;
        /// using System.Collections.Generic;
        /// using System.IO;
        /// using System.Linq;
        /// using Windows.Foundation;
        /// using Windows.Foundation.Collections;
        /// using Windows.UI.Xaml;
        /// using Windows.UI.Xaml.Controls;
        /// using Windows.UI.Xaml.Controls.Primitives;
        /// using Windows.UI.Xaml.Data;
        /// using Windows.UI.Xaml.Input;
        /// using Windows.UI.Xaml.Media;
        /// using Windows.UI.Xaml.Navigation;
        /// using Common;
        /// using Syncfusion.UI.Xaml.Gauges;
        /// 
        /// namespace GaugeWinRTSamples
        /// {
        ///     public sealed partial class GaugePosition :SampleView
        ///     {
        ///         public GaugePosition()
        ///         {
        ///            this.InitializeComponent();
        ///            SfCircularGauge gauge = new SfCircularGauge();
        ///            CircularPointer needlePointer = new CircularPointer();
        ///            needlePointer.Value = 80;
        ///            needlePointer.PointerType = PointerType.NeedlePointer;
        ///	           needlePointer.NeedlePointerVisibility = Visibility.Collapsed;
        ///            gauge.MainScale.Pointers.Add(needlePointer);
        ///         }
        ///     }
        /// }
        /// 
        /// </code>
        /// </example>
        [ClassReference(IsReviewed = false)]
        public Visibility NeedlePointerVisibility
        {
            get { return (Visibility)GetValue(NeedlePointerVisibilityProperty); }
            set { SetValue(NeedlePointerVisibilityProperty, value); }
        }

        // Using a DependencyProperty as the backing store for NeedlePointerVisibility.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty NeedlePointerVisibilityProperty =
            DependencyProperty.Register("NeedlePointerVisibility", typeof(Visibility), typeof(CircularPointer), new PropertyMetadata(Visibility.Visible));

        

        internal double ParentStartValue
        {
            get { return (double)GetValue(ParentStartValueProperty); }
            set { SetValue(ParentStartValueProperty, value); }
        }

        // Using a DependencyProperty as the backing store for ParentStartValue.  This enables animation, styling, binding, etc...
        internal static readonly DependencyProperty ParentStartValueProperty =
            DependencyProperty.Register("ParentStartValue", typeof(double), typeof(CircularPointer), new PropertyMetadata(0d, OnParentPropertyChanged));




        internal double ParentEndValue
        {
            get { return (double)GetValue(ParentEndValueProperty); }
            set { SetValue(ParentEndValueProperty, value); }
        }

        // Using a DependencyProperty as the backing store for ParentEndValue.  This enables animation, styling, binding, etc...
        internal static readonly DependencyProperty ParentEndValueProperty =
            DependencyProperty.Register("ParentEndValue", typeof(double), typeof(CircularPointer), new PropertyMetadata(0d, OnParentPropertyChanged));



        internal Size AvailSize
        {
            get { return (Size)GetValue(AvailSizeProperty); }
            set { SetValue(AvailSizeProperty, value); }
        }

        // Using a DependencyProperty as the backing store for AvailSize.  This enables animation, styling, binding, etc...
        internal static readonly DependencyProperty AvailSizeProperty =
            DependencyProperty.Register("AvailSize", typeof(Size), typeof(CircularPointer), new PropertyMetadata(new Size(0, 0), OnParentSizeChanged));



        internal Transform NeedleTranform
        {
            get { return (Transform)GetValue(NeedleTranformProperty); }
            set { SetValue(NeedleTranformProperty, value); }
        }

        // Using a DependencyProperty as the backing store for NeedleTranform.  This enables animation, styling, binding, etc...
        internal static readonly DependencyProperty NeedleTranformProperty =
            DependencyProperty.Register("NeedleTranform", typeof(Transform), typeof(CircularPointer), new PropertyMetadata(new RotateTransform()));

        

        internal Size ParentSize
        {
            get { return (Size)GetValue(ParentSizeProperty); }
            set { SetValue(ParentSizeProperty, value); }
        }

        // Using a DependencyProperty as the backing store for ParentSize.  This enables animation, styling, binding, etc...
        internal static readonly DependencyProperty ParentSizeProperty =
            DependencyProperty.Register("ParentSize", typeof(Size), typeof(CircularPointer), new PropertyMetadata(new Size(0,0), OnParentSizeChanged));

        private static void OnParentSizeChanged(DependencyObject obj , DependencyPropertyChangedEventArgs args)
        {
            if (obj is CircularPointer)
            {
                CircularPointer ptr = obj as CircularPointer;
                ptr.SetProperties();
            }
        }

        /// <summary>
        /// Gets or sets Type of the CircularPointer.
        /// </summary>
        /// <remarks>
        /// There are three types of pointers. User can choose a pointer using the PointerType property. 
        /// Options are
        /// 1. NeedlePointer (Default)
        /// 2. RangePointer
        /// 3. Symbolointer
        /// </remarks>
        /// <value>
        /// PointerType
        /// </value>
        /// <example>
        /// <code language="C#">
        /// using System;
        /// using System.Collections.Generic;
        /// using System.IO;
        /// using System.Linq;
        /// using Windows.Foundation;
        /// using Windows.Foundation.Collections;
        /// using Windows.UI.Xaml;
        /// using Windows.UI.Xaml.Controls;
        /// using Windows.UI.Xaml.Controls.Primitives;
        /// using Windows.UI.Xaml.Data;
        /// using Windows.UI.Xaml.Input;
        /// using Windows.UI.Xaml.Media;
        /// using Windows.UI.Xaml.Navigation;
        /// using Common;
        /// using Syncfusion.UI.Xaml.Gauges;
        /// 
        /// namespace GaugeWinRTSamples
        /// {
        ///     public sealed partial class GaugePosition :SampleView
        ///     {
        ///         public GaugePosition()
        ///         {
        ///            this.InitializeComponent();
        ///            SfCircularGauge gauge = new SfCircularGauge();
        ///            CircularPointer pointer = new CircularPointer();
        ///            pointer.Value = 80;
        ///            pointer.PointerType = PointerType.NeedlePointer;
        ///            gauge.MainScale.Pointers.Add(pointer);
        ///         }
        ///     }
        /// }
        /// 
        /// </code>
        /// </example>
        [ClassReference(IsReviewed = false)]
        public PointerType PointerType
        {
            get { return (PointerType)GetValue(PointerTypeProperty); }
            set { SetValue(PointerTypeProperty, value); }
        }

        // Using a DependencyProperty as the backing store for PointerType.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty PointerTypeProperty =
            DependencyProperty.Register("PointerType", typeof(PointerType), typeof(CircularPointer), new PropertyMetadata(PointerType.NeedlePointer, OnPointerTypeChanged));

        private static void OnPointerTypeChanged(DependencyObject obj, DependencyPropertyChangedEventArgs args)
        {

            if (obj is CircularPointer)
            {
                CircularPointer circularPointer = obj as CircularPointer;
                if (circularPointer.PointerType == PointerType.NeedlePointer)
                {
                    circularPointer.NeedlePointerVisibility = Visibility.Visible;
                    circularPointer.RangePointerVisibility = Visibility.Collapsed;
                    circularPointer.SymbolPointerVisibility = Visibility.Collapsed;
                }
                else if (circularPointer.PointerType == PointerType.RangePointer)
                {
                    circularPointer.NeedlePointerVisibility = Visibility.Collapsed;
                    circularPointer.RangePointerVisibility = Visibility.Visible;
                    circularPointer.SymbolPointerVisibility = Visibility.Collapsed;
                }
                else
                {
                    circularPointer.NeedlePointerVisibility = Visibility.Collapsed;
                    circularPointer.RangePointerVisibility = Visibility.Collapsed;
                    circularPointer.SymbolPointerVisibility = Visibility.Visible;
                }
            }
        }

        internal double ParentStartAngle
        {
            get { return (double)GetValue(ParentStartAngleProperty); }
            set { SetValue(ParentStartAngleProperty, value); }
        }

        // Using a DependencyProperty as the backing store for ParentStartAngle.  This enables animation, styling, binding, etc...
        internal static readonly DependencyProperty ParentStartAngleProperty =
            DependencyProperty.Register("ParentStartAngle", typeof(double), typeof(CircularPointer), new PropertyMetadata(0d, OnParentPropertyChanged));

        

        internal double ParentSweepAngle
        {
            get { return (double)GetValue(ParentSweepAngleProperty); }
            set { SetValue(ParentSweepAngleProperty, value); }
        }

        // Using a DependencyProperty as the backing store for ParentSweepAngle.  This enables animation, styling, binding, etc...
        internal static readonly DependencyProperty ParentSweepAngleProperty =
            DependencyProperty.Register("ParentSweepAngle", typeof(double), typeof(CircularPointer), new PropertyMetadata(0d, OnParentPropertyChanged));



        internal SweepDirection SweepDirection
        {
            get { return (SweepDirection)GetValue(SweepDirectionProperty); }
            set { SetValue(SweepDirectionProperty, value); }
        }

        // Using a DependencyProperty as the backing store for SweeepDirection.  This enables animation, styling, binding, etc...
        internal static readonly DependencyProperty SweepDirectionProperty =
            DependencyProperty.Register("SweepDirection", typeof(SweepDirection), typeof(CircularPointer), new PropertyMetadata(SweepDirection.Clockwise, OnParentPropertyChanged));



        internal double NeedleLength
        {
            get { return (double)GetValue(NeedleLengthProperty); }
            set { SetValue(NeedleLengthProperty, value); }
        }

        // Using a DependencyProperty as the backing store for NeedleLength.  This enables animation, styling, binding, etc...
        internal static readonly DependencyProperty NeedleLengthProperty =
            DependencyProperty.Register("NeedleLength", typeof(double), typeof(CircularPointer), new PropertyMetadata(0d));


        /// <summary>
        /// Gets or sets the Value that decides the position of the CircularPointer in the CircularScale.
        /// </summary>
        /// <remarks>
        /// Value property is one of the most important property. Based on the Value
        /// property pointer has been positioned on the CircularScale. Value should resides
        /// between the start and end value of the scale. If the value of this property is
        /// less than StartValue of the CircularScale, then the StartValue of the
        /// CircularScale is set to the Value of the pointer. Similarly EndValue of the
        /// CircularScale is set to the Value of the pointer when Value is greater that
        /// EndValue of the scale.
        /// </remarks>
        /// <value>
        /// double
        /// </value>
        /// <example>
        /// <code language="C#">
        /// using System;
        /// using System.Collections.Generic;
        /// using System.IO;
        /// using System.Linq;
        /// using Windows.Foundation;
        /// using Windows.Foundation.Collections;
        /// using Windows.UI.Xaml;
        /// using Windows.UI.Xaml.Controls;
        /// using Windows.UI.Xaml.Controls.Primitives;
        /// using Windows.UI.Xaml.Data;
        /// using Windows.UI.Xaml.Input;
        /// using Windows.UI.Xaml.Media;
        /// using Windows.UI.Xaml.Navigation;
        /// using Common;
        /// using Syncfusion.UI.Xaml.Gauges;
        /// 
        /// namespace GaugeWinRTSamples
        /// {
        ///     public sealed partial class GaugePosition :SampleView
        ///     {
        ///         public GaugePosition()
        ///         {
        ///            this.InitializeComponent();
        ///            SfCircularGauge gauge = new SfCircularGauge();
        ///            CircularPointer needlePointer = new CircularPointer();
        ///            needlePointer.Value = 80;
        ///            needlePointer.PointerType = PointerType.NeedlePointer;
        ///            gauge.MainScale.Pointers.Add(needlePointer);
        ///         }
        ///     }
        /// }
        /// 
        /// </code>
        /// </example>
        [ClassReference(IsReviewed = false)]
        public double Value
        {
            get { return (double)GetValue(ValueProperty); }
            set { SetValue(ValueProperty, value); }
        }

        // Using a DependencyProperty as the backing store for Value.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty ValueProperty =
            DependencyProperty.Register("Value", typeof(double), typeof(CircularPointer), new PropertyMetadata(0d,OnValueChanged));

        private static void OnValueChanged(DependencyObject obj, DependencyPropertyChangedEventArgs args)
        {
            if(obj is CircularPointer)
            {
                CircularPointer circularPointer = obj as CircularPointer;
                if (circularPointer.RangePointerVisibility == Visibility.Visible && circularPointer.EnableAnimation)
                {
                    circularPointer.ValueDiff = (double)args.NewValue - (double)args.OldValue;
                    circularPointer.TimeValue = (double)args.OldValue;
                    circularPointer.Timer.Start();
                }
                else
                {
                    circularPointer.TimeValue = circularPointer.Value;
                }
            }
        }



        private DispatcherTimer Timer
        {
            get { return (DispatcherTimer)GetValue(TimerProperty); }
            set { SetValue(TimerProperty, value); }
        }

        // Using a DependencyProperty as the backing store for Timer.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty TimerProperty =
            DependencyProperty.Register("Timer", typeof(DispatcherTimer), typeof(CircularPointer), new PropertyMetadata(new DispatcherTimer()));

        

        private double ValueDiff
        {
            get { return (double)GetValue(ValueDiffProperty); }
            set { SetValue(ValueDiffProperty, value); }
        }

        // Using a DependencyProperty as the backing store for ValueDiff.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty ValueDiffProperty =
            DependencyProperty.Register("ValueDiff", typeof(double), typeof(CircularPointer), new PropertyMetadata(0d));

        

        internal double TimeValue
        {
            get { return (double)GetValue(TimeValueProperty); }
            set { SetValue(TimeValueProperty, value); }
        }

        // Using a DependencyProperty as the backing store for TimeValue.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty TimeValueProperty =
            DependencyProperty.Register("TimeValue", typeof(double), typeof(CircularPointer), new PropertyMetadata(0d, OnTimeValueChanged));

         private static void OnTimeValueChanged(DependencyObject obj, DependencyPropertyChangedEventArgs args)
        {
            if(obj is CircularPointer)
            {
                CircularPointer circularPointer = obj as CircularPointer;
                circularPointer.SetProperties();
            }
        }

        /// <summary>
        /// Gets or sets the value which helps to calculate the NeedleLength of the CircularPointer.
        /// </summary>
        /// <value>
        /// double
        /// </value>
        /// <remarks>
        /// <para>NeedleLengthFactor property is used to calculate the NeedleLength based on the size of the SfCircularGauge. 
        /// Value of the property should reside between 0.1 and 1.0.
        /// The default value is 0.75.
        /// </para>
        /// </remarks>
        /// <example>
        /// <code language="C#">
        /// using System;
        /// using System.Collections.Generic;
        /// using System.IO;
        /// using System.Linq;
        /// using Windows.Foundation;
        /// using Windows.Foundation.Collections;
        /// using Windows.UI.Xaml;
        /// using Windows.UI.Xaml.Controls;
        /// using Windows.UI.Xaml.Controls.Primitives;
        /// using Windows.UI.Xaml.Data;
        /// using Windows.UI.Xaml.Input;
        /// using Windows.UI.Xaml.Media;
        /// using Windows.UI.Xaml.Navigation;
        /// using Common;
        /// using Syncfusion.UI.Xaml.Gauges;
        /// 
        /// namespace GaugeWinRTSamples
        /// {
        ///     public sealed partial class GaugePosition :SampleView
        ///     {
        ///         public GaugePosition()
        ///         {
        ///            this.InitializeComponent();
        ///            SfCircularGauge gauge = new SfCircularGauge();
        ///            CircularPointer pointer = new CircularPointer();
        ///            pointer.Value = 80;
        ///            pointer.PointerType = PointerType.NeedlePointer;
        ///            pointer.NeedleLengthFactor = 0.5;
        ///            gauge.MainScale.Pointers.Add(pointer);
        ///         }
        ///     }
        /// }
        /// 
        /// </code>
        /// </example>
        [ClassReference(IsReviewed = false)]
        public double NeedleLengthFactor
        {
            get { return (double)GetValue(NeedleLengthFactorProperty); }
            set { SetValue(NeedleLengthFactorProperty, value); }
        }

        // Using a DependencyProperty as the backing store for NeedleLength.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty NeedleLengthFactorProperty =
            DependencyProperty.Register("NeedleLengthFactor", typeof(double), typeof(CircularPointer), new PropertyMetadata(0.75d, OnNeedleLengthFactorChanged));


        private static void OnNeedleLengthFactorChanged(DependencyObject obj, DependencyPropertyChangedEventArgs args)
        {
           
            if (obj is CircularPointer)
            {

                CircularPointer circularPointer = obj as CircularPointer;
                circularPointer.NeedleLengthFactor = double.IsNaN(circularPointer.NeedleLengthFactor) ? 0.75 : circularPointer.NeedleLengthFactor < 0 ? 0 : circularPointer.NeedleLengthFactor > 1 ? 1 : circularPointer.NeedleLengthFactor;
                if(circularPointer.NeedleLengthFactor>=0&&circularPointer.NeedleLengthFactor<1)
                circularPointer.NeedleLength = circularPointer.ParentSize.Width * circularPointer.NeedleLengthFactor / 2;
            }
        }

        internal double Angle
        {
            get { return (double)GetValue(AngleProperty); }
            set { SetValue(AngleProperty, value); }
        }

        // Using a DependencyProperty as the backing store for Angle.  This enables animation, styling, binding, etc...
        internal static readonly DependencyProperty AngleProperty =
            DependencyProperty.Register("Angle", typeof(double), typeof(CircularPointer), new PropertyMetadata(0d, OnAngleChanged));


        /// <summary>
        /// Gets or sets the SymbolPointerHeight value that decides the size of the SymbolPointer .
        /// </summary>
        /// <value>
        /// double
        /// </value>
        /// <remarks>
        /// <para>This property used to change the size of the SymbolPointer.
        /// </para>
        /// </remarks>
        /// <example>
        /// <code language="C#">
        /// using System;
        /// using System.Collections.Generic;
        /// using System.IO;
        /// using System.Linq;
        /// using Windows.Foundation;
        /// using Windows.Foundation.Collections;
        /// using Windows.UI.Xaml;
        /// using Windows.UI.Xaml.Controls;
        /// using Windows.UI.Xaml.Controls.Primitives;
        /// using Windows.UI.Xaml.Data;
        /// using Windows.UI.Xaml.Input;
        /// using Windows.UI.Xaml.Media;
        /// using Windows.UI.Xaml.Navigation;
        /// using Common;
        /// using Syncfusion.UI.Xaml.Gauges;
        /// 
        /// namespace GaugeWinRTSamples
        /// {
        ///     public sealed partial class GaugePosition :SampleView
        ///     {
        ///         public GaugePosition()
        ///         {
        ///            this.InitializeComponent();
        ///            SfCircularGauge gauge = new SfCircularGauge();
        ///            CircularPointer pointer = new CircularPointer();
        ///            pointer.Value = 80;
        ///            pointer.PointerType = PointerType.SymbolPointer;
        ///            pointer.SymbolPointerHeight = 20;
        ///            pointer.SymbolPointerWidth = 20;
        ///            gauge.MainScale.Pointers.Add(pointer);
        ///         }
        ///     }
        /// }
        /// 
        /// </code>
        /// </example>
        [ClassReference(IsReviewed = false)]
        public double SymbolPointerHeight
        {
            get { return (double)GetValue(SymbolPointerHeightProperty); }
            set { SetValue(SymbolPointerHeightProperty, value); }
        }

        // Using a DependencyProperty as the backing store for SymbolPointerDiameter.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty SymbolPointerHeightProperty =
            DependencyProperty.Register("SymbolPointerHeight", typeof(double), typeof(CircularPointer), new PropertyMetadata(20d, OnSymbolPointerHeightChanged));

        private static void OnSymbolPointerHeightChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if ((Double)e.NewValue < 0)
            {
                (d as CircularPointer).SymbolPointerHeight = 0;
            }
           
        }

        /// <summary>
        /// Gets or sets the SymbolPointerWidth value that decides the size of the SymbolPointer .
        /// </summary>
        /// <value>
        /// double
        /// </value>
        /// <remarks>
        /// <para>This property used to change the size of the SymbolPointer.
        /// </para>
        /// </remarks>
        /// <example>
        /// <code language="C#">
        /// using System;
        /// using System.Collections.Generic;
        /// using System.IO;
        /// using System.Linq;
        /// using Windows.Foundation;
        /// using Windows.Foundation.Collections;
        /// using Windows.UI.Xaml;
        /// using Windows.UI.Xaml.Controls;
        /// using Windows.UI.Xaml.Controls.Primitives;
        /// using Windows.UI.Xaml.Data;
        /// using Windows.UI.Xaml.Input;
        /// using Windows.UI.Xaml.Media;
        /// using Windows.UI.Xaml.Navigation;
        /// using Common;
        /// using Syncfusion.UI.Xaml.Gauges;
        /// 
        /// namespace GaugeWinRTSamples
        /// {
        ///     public sealed partial class GaugePosition :SampleView
        ///     {
        ///         public GaugePosition()
        ///         {
        ///            this.InitializeComponent();
        ///            SfCircularGauge gauge = new SfCircularGauge();
        ///            CircularPointer pointer = new CircularPointer();
        ///            pointer.Value = 80;
        ///            pointer.PointerType = PointerType.SymbolPointer;
        ///            pointer.SymbolPointerHeight = 20;
        ///            pointer.SymbolPointerWidth = 20;
        ///            gauge.MainScale.Pointers.Add(pointer);
        ///         }
        ///     }
        /// }
        /// 
        /// </code>
        /// </example>
        [ClassReference(IsReviewed = false)]
        public double SymbolPointerWidth
        {
            get { return (double)GetValue(SymbolPointerWidthProperty); }
            set { SetValue(SymbolPointerWidthProperty, value); }            
        }

        // Using a DependencyProperty as the backing store for SymbolPointerDiameter.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty SymbolPointerWidthProperty =
            DependencyProperty.Register("SymbolPointerWidth", typeof(double), typeof(CircularPointer), new PropertyMetadata(20d, OnSymbolPointerWidthChanged));

        private static void OnSymbolPointerWidthChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if ((Double)e.NewValue < 0)
            {
                (d as CircularPointer).SymbolPointerWidth = 0;
            }

        }
        /// <summary>
        /// Gets or sets the Symbol value that decides the shape of the SymbolPointer .
        /// </summary>
        /// <value>
        /// double
        /// </value>
        /// <remarks>
        /// <para>This property used to change the symbol of the SymbolPointer.
        /// </para>
        /// </remarks>
        /// <example>
        /// <code language="C#">
        /// using System;
        /// using System.Collections.Generic;
        /// using System.IO;
        /// using System.Linq;
        /// using Windows.Foundation;
        /// using Windows.Foundation.Collections;
        /// using Windows.UI.Xaml;
        /// using Windows.UI.Xaml.Controls;
        /// using Windows.UI.Xaml.Controls.Primitives;
        /// using Windows.UI.Xaml.Data;
        /// using Windows.UI.Xaml.Input;
        /// using Windows.UI.Xaml.Media;
        /// using Windows.UI.Xaml.Navigation;
        /// using Common;
        /// using Syncfusion.UI.Xaml.Gauges;
        /// 
        /// namespace GaugeWinRTSamples
        /// {
        ///     public sealed partial class GaugePosition :SampleView
        ///     {
        ///         public GaugePosition()
        ///         {
        ///            this.InitializeComponent();
        ///            SfCircularGauge gauge = new SfCircularGauge();
        ///            CircularPointer pointer = new CircularPointer();
        ///            pointer.Value = 80;
        ///            pointer.PointerType = PointerType.SymbolPointer;
        ///            pointer.Symbol = Symbol.Ellipse;
        ///            pointer.SymbolPointerHeight = 20;
        ///            pointer.SymbolPointerWidth = 20;
        ///            gauge.MainScale.Pointers.Add(pointer);
        ///         }
        ///     }
        /// }
        /// 
        /// </code>
        /// </example>
        public Symbol Symbol
        {
            get { return (Symbol)GetValue(SymbolProperty); }
            set { SetValue(SymbolProperty, value); }
        }

        // Using a DependencyProperty as the backing store for Symbol.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty SymbolProperty =
            DependencyProperty.Register("Symbol", typeof(Symbol), typeof(CircularPointer), new PropertyMetadata(Symbol.Ellipse, OnSymbolChanged));


        /// <summary>
        /// Gets or sets the template for the custom symbol for the SymbolPointer.
        /// </summary>
        /// <remarks>
        /// SymbolPointerTemplate will get applied when the Symbol property of the
        /// CircularPointer is set to Custom.
        /// </remarks>
        /// <value>
        /// DataTemplate
        /// </value>
        /// <example>
        /// <code language="C#">
        /// using System;
        /// using System.Collections.Generic;
        /// using System.IO;
        /// using System.Linq;
        /// using Windows.Foundation;
        /// using Windows.Foundation.Collections;
        /// using Windows.UI.Xaml;
        /// using Windows.UI.Xaml.Controls;
        /// using Windows.UI.Xaml.Controls.Primitives;
        /// using Windows.UI.Xaml.Data;
        /// using Windows.UI.Xaml.Input;
        /// using Windows.UI.Xaml.Media;
        /// using Windows.UI.Xaml.Navigation;
        /// using Common;
        /// using Syncfusion.UI.Xaml.Gauges;
        /// 
        /// namespace GaugeWinRTSamples
        /// {
        ///     public sealed partial class GaugePosition :SampleView
        ///     {
        ///         public GaugePosition()
        ///         {
        ///            this.InitializeComponent();
        ///            SfCircularGauge gauge = new SfCircularGauge();
        ///            CircularPointer pointer = new CircularPointer();
        ///            pointer.Value = 80;
        ///            pointer.PointerType = PointerType.SymbolPointer;
        ///            pointer.Symbol = Symbol.Custom;
        ///            pointer.SymbolPointerTemplate = this.Resources["star"];
        ///            gauge.MainScale.Pointers.Add(pointer);
        ///         }
        ///     }
        /// }
        /// 
        /// </code>
        /// </example>
        public DataTemplate SymbolPointerTemplate
        {
            get { return (DataTemplate)GetValue(SymbolPointerTemplateProperty); }
            set { SetValue(SymbolPointerTemplateProperty, value); }
        }

        // Using a DependencyProperty as the backing store for SymbolPointerTemplate.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty SymbolPointerTemplateProperty =
            DependencyProperty.Register("SymbolPointerTemplate", typeof(DataTemplate), typeof(CircularPointer), new PropertyMetadata(null, OnSymbolChanged));

        #region SymbolContent
        internal object SymbolContent
        {
            get { return GetValue(SymbolContentProperty); }
            set { SetValue(SymbolContentProperty, value); }
        }

        // Using a DependencyProperty as the backing store for SymbolContent.  This enables animation, styling, binding, etc...
        internal static readonly DependencyProperty SymbolContentProperty =
            DependencyProperty.Register("SymbolContent", typeof(object), typeof(CircularPointer), new PropertyMetadata(null)); 
        #endregion

        

        private static void OnSymbolChanged(DependencyObject obj, DependencyPropertyChangedEventArgs args)
        {
            if (obj is CircularPointer)
            {
                CircularPointer circularPointer = obj as CircularPointer;
                circularPointer.SetSymbolForPointer();
            }
        }

        void SetSymbolForPointer()
        {
            if (resourcedictionary != null)
            {
                if (Symbol != Symbol.Custom)
                {
                    DataTemplate symbolTemplate = resourcedictionary[Symbol.ToString()] as DataTemplate;
                    if (symbolTemplate != null)
                    {
                        SymbolContent = symbolTemplate.LoadContent();
                    }
                }
                else
                {
                    if (SymbolPointerTemplate != null)
                        SymbolContent = SymbolPointerTemplate.LoadContent();
                }

            }
            SetProperties();
        }


        /// <summary>
        /// Gets or sets the PointerCapDiameter value that decides the size of the PointerCap of the NeedlePointer .
        /// </summary>
        /// <value>
        /// double
        /// </value>
        /// <remarks>
        /// <para>This property used to change the size of the NeedlePointer cap.
        /// </para>
        /// </remarks>
        /// <example>
        /// <code language="C#">
        /// using System;
        /// using System.Collections.Generic;
        /// using System.IO;
        /// using System.Linq;
        /// using Windows.Foundation;
        /// using Windows.Foundation.Collections;
        /// using Windows.UI.Xaml;
        /// using Windows.UI.Xaml.Controls;
        /// using Windows.UI.Xaml.Controls.Primitives;
        /// using Windows.UI.Xaml.Data;
        /// using Windows.UI.Xaml.Input;
        /// using Windows.UI.Xaml.Media;
        /// using Windows.UI.Xaml.Navigation;
        /// using Common;
        /// using Syncfusion.UI.Xaml.Gauges;
        /// 
        /// namespace GaugeWinRTSamples
        /// {
        ///     public sealed partial class GaugePosition :SampleView
        ///     {
        ///         public GaugePosition()
        ///         {
        ///            this.InitializeComponent();
        ///            SfCircularGauge gauge = new SfCircularGauge();
        ///            CircularPointer pointer = new CircularPointer();
        ///            pointer.Value = 80;
        ///            pointer.PointerType = PointerType.NeedlePointer;
        ///            pointer.PointerCapDiameter = 20;
        ///            gauge.MainScale.Pointers.Add(pointer);
        ///         }
        ///     }
        /// }
        /// 
        /// </code>
        /// </example>
        [ClassReference(IsReviewed = false)]
        public double PointerCapDiameter
        {
            get { return (double)GetValue(PointerCapDiameterProperty); }
            set { SetValue(PointerCapDiameterProperty, value); }
        }

        // Using a DependencyProperty as the backing store for PointerCapDiameter.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty PointerCapDiameterProperty =
            DependencyProperty.Register("PointerCapDiameter", typeof(double), typeof(CircularPointer), new PropertyMetadata(10d,OnPointerCapDiameterPropertyChanged));
        private static void OnPointerCapDiameterPropertyChanged(DependencyObject obj, DependencyPropertyChangedEventArgs args)
        {
            CircularPointer ptr = obj as CircularPointer;
            if (!(ptr.PointerCapDiameter >= 0))
               ptr.PointerCapDiameter = 0;
            if (!(ptr.PointerCapDiameter <= 40))
                ptr.PointerCapDiameter = 40;
        }
        private static void OnParentPropertyChanged(DependencyObject obj, DependencyPropertyChangedEventArgs args)
        {
            CircularPointer ptr = obj as CircularPointer;
            if (ptr != null)
            {
                ptr.SetProperties();
            }
        }

        internal double DegToRad(double deg)
        {
            return deg * Math.PI / 180;
        }




        internal bool IsLargeArc
        {
            get { return (bool)GetValue(IsLargeArcProperty); }
            set { SetValue(IsLargeArcProperty, value); }
        }

        // Using a DependencyProperty as the backing store for IsLargeArc.  This enables animation, styling, binding, etc...
        internal static readonly DependencyProperty IsLargeArcProperty =
            DependencyProperty.Register("IsLargeArc", typeof(bool), typeof(CircularPointer), new PropertyMetadata(false));



        internal Size HalfSize
        {
            get { return (Size)GetValue(HalfSizeProperty); }
            set { SetValue(HalfSizeProperty, value); }
        }

        // Using a DependencyProperty as the backing store for HalfSize.  This enables animation, styling, binding, etc...
        internal static readonly DependencyProperty HalfSizeProperty =
            DependencyProperty.Register("HalfSize", typeof(Size), typeof(CircularPointer), new PropertyMetadata(new Size(0,0)));

        

        internal Point RangePointerEndPoint
        {
            get { return (Point)GetValue(RangePointerEndPointProperty); }
            set { SetValue(RangePointerEndPointProperty, value); }
        }

        // Using a DependencyProperty as the backing store for RangePointerEndPoint.  This enables animation, styling, binding, etc...
        internal static readonly DependencyProperty RangePointerEndPointProperty =
            DependencyProperty.Register("RangePointerEndPoint", typeof(Point), typeof(CircularPointer), new PropertyMetadata(new Point(0, 0)));

        

        internal Point RangePointerStartPoint
        {
            get { return (Point)GetValue(RangePointerStartPointProperty); }
            set { SetValue(RangePointerStartPointProperty, value); }
        }

        // Using a DependencyProperty as the backing store for RangePointerStartPoint.  This enables animation, styling, binding, etc...
        internal static readonly DependencyProperty RangePointerStartPointProperty =
            DependencyProperty.Register("RangePointerStartPoint", typeof(Point), typeof(CircularPointer), new PropertyMetadata(new Point(0,0)));


        /// <summary>
        /// Gets or sets value indicating whether this element can be animated.
        /// </summary>
        /// <remarks>
        /// <para>EnableAnimation property is used to animate the movement of the pointer 
        /// when the Value is of the pointer is changed. Its default value is true.</para>
        /// </remarks>
        /// <value>
        /// <see langword="true"/> if ; otherwise, <see langword="false"/>.
        /// </value>
        /// <example>
        /// <code language="C#">
        /// using System;
        /// using System.Collections.Generic;
        /// using System.IO;
        /// using System.Linq;
        /// using Windows.Foundation;
        /// using Windows.Foundation.Collections;
        /// using Windows.UI.Xaml;
        /// using Windows.UI.Xaml.Controls;
        /// using Windows.UI.Xaml.Controls.Primitives;
        /// using Windows.UI.Xaml.Data;
        /// using Windows.UI.Xaml.Input;
        /// using Windows.UI.Xaml.Media;
        /// using Windows.UI.Xaml.Navigation;
        /// using Common;
        /// using Syncfusion.UI.Xaml.Gauges;
        /// 
        /// namespace GaugeWinRTSamples
        /// {
        ///     public sealed partial class GaugePosition :SampleView
        ///     {
        ///         public GaugePosition()
        ///         {
        ///            this.InitializeComponent();
        ///            SfCircularGauge gauge = new SfCircularGauge();
        ///            CircularPointer pointer = new CircularPointer();
        ///            pointer.Value = 80;
        ///            pointer.PointerType = PointerType.NeedlePointer;
        ///            pointer.EnableAnimation = false;
        ///         }
        ///     }
        /// }
        /// 
        /// </code>
        /// </example>
        [ClassReference(IsReviewed = false)]
        public bool EnableAnimation
        {
            get { return (bool)GetValue(EnableAnimationProperty); }
            set { SetValue(EnableAnimationProperty, value); }
        }

        // Using a DependencyProperty as the backing store for EnableAnimation.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty EnableAnimationProperty =
            DependencyProperty.Register("EnableAnimation", typeof(bool), typeof(CircularPointer), new PropertyMetadata(true));



        private static void OnAngleChanged(DependencyObject obj, DependencyPropertyChangedEventArgs args)
        {
            var circularpointer = obj as CircularPointer;
            if (circularpointer != null && args.OldValue != null && circularpointer.EnableAnimation)
            {
                circularpointer.angleanimation.From = (double)args.OldValue == 0 ? (double)args.NewValue : (double)args.OldValue;
                circularpointer.angleanimation.To= (double)args.NewValue;
                circularpointer.sb.Begin();
            }
        }
         
         private double ValueToAngle(double TimeValue)
        {
            double angle;
            if (TimeValue < ParentStartValue || double.IsNaN(TimeValue))
            {
                TimeValue = ParentStartValue;
            }
            if (TimeValue > ParentEndValue)
            {
                TimeValue = ParentEndValue;
            }
            if (this.SweepDirection == SweepDirection.Clockwise)
            {
                angle = ParentStartAngle + ((ParentSweepAngle / Math.Abs(ParentEndValue - ParentStartValue)) * Math.Abs(ParentStartValue - TimeValue));
            }
            else
            {
                angle = ParentStartAngle - ((ParentSweepAngle / Math.Abs(ParentEndValue - ParentStartValue)) * Math.Abs(ParentStartValue - TimeValue));
            }
            return double.IsNaN(angle) ? 0 : angle;
        }

    }

    /// <summary>
    /// It is a collection that contains a set of Pointers that can be used to point
    /// values in CircularScale .
    /// </summary>
    public class CircularPointerCollection : ObservableCollection<CircularPointer>
    {

    }

#if WINRT
    public class MyPointer : Control
    {
        public MyPointer()
        {
            this.DefaultStyleKey = typeof(MyPointer);
        }

    }

    public class Gridpanel : Grid
    {

        public Gridpanel()
        {
            this.Setpointers();
        }
          #region Dependency Properties



        internal object ItemsSource
        {
            get { return (object)GetValue(ItemsSourceProperty); }
            set { SetValue(ItemsSourceProperty, value); }
        }

        // Using a DependencyProperty as the backing store for ItemsSource.  This enables animation, styling, binding, etc...
        internal static readonly DependencyProperty ItemsSourceProperty =
            DependencyProperty.Register("ItemsSource", typeof(object), typeof(Gridpanel), new PropertyMetadata(null,OnItemsSourceChanged));

        private static void OnItemsSourceChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d is Gridpanel)
            {
                Gridpanel obj = (Gridpanel)d;
                obj.Setpointers();
            }
        }
        
        #endregion
        private void Setpointers()
        {
            if (ItemsSource != null)
            {
                foreach (var item in (IEnumerable)ItemsSource)
                {
                    MyPointer child1 = new MyPointer() { DataContext = item };
                    UIElement child = child1;
                    this.Children.Add(child);
                } 
            }            
        }
    }
#endif
}
