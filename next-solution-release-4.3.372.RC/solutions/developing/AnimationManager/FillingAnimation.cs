using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Media;
#if !NET_STANDARD
using Utilities.Animations;
using Utilities.WPF;
#endif
#if !WINDOWS_UWP
#if !NET_STANDARD
using System.Windows.Controls;
using System.Windows.Shapes;
using System.Windows.Media.Animation;
#endif
#else
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml;
using Windows.UI;
using Windows.Foundation;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Shapes;
using Windows.UI.Xaml.Media.Animation;
#endif
using System.Runtime.Serialization;
using System.Windows;
using System.ComponentModel;
using Utilities;

namespace AnimationManager
{
#if !WINDOWS_UWP && !NET_STANDARD
    [TypeConverter(typeof(LocalizedEnumConverter))]
#endif
    public enum FillingBehavior
    {
        BottomUp,
        UpBottom,
        LeftRight,
        RightLeft,
        Center,
        Custom
    }

#if !WINDOWS_UWP && !NET_STANDARD
    [TypeConverter(typeof(LocalizedEnumConverter))]
#endif
    public enum GradientType
    {
        Linear,
        Radial
    }

    [DataContract(Name = "FillingAnimation")]
    public class FillingAnimation : AnimationManager
    {
#region Declarations
#if !NET_STANDARD
        GradientStopCollection gradientStopCollection;
        GradientStop gradientStop1;
        GradientStop gradientStop2;
        GradientStop gradientStop3;
        LinearGradientBrush linearBrush;
#if !WINDOWS_UWP
        RadialGradientBrush radialBrush;
#endif
        Dictionary<UIElement, Brush> mapoldBrush;
        Object lockObject = new Object();
        double fromValue = 0;
#endif
#endregion

#region Properties

        double offset = 1;
        [DataMember]
        public double Offset
        {
            get { return offset; }
            set
            {
                if (value == offset)
                    return;
                offset = value;
#if !NET_STANDARD
#if !WINDOWS_UWP
                OnPropertyChanged("Offset");
#endif
                Reexecute();
#endif
            }
        }

        Color startColor = Colors.Black;
        [DataMember]
        public Color StartColor
        {
            get { return startColor; }
            set
            {
                if (value == startColor)
                    return;
                startColor = value;
#if !NET_STANDARD
#if !WINDOWS_UWP
                OnPropertyChanged("StartColor");
#endif
                Reexecute();
#endif
            }
        }

        Color endColor = Colors.White;

        [DataMember]
        public Color EndColor
        {
            get { return endColor; }
            set
            {
                if (value == endColor)
                    return;
                endColor = value;
#if !NET_STANDARD
#if !WINDOWS_UWP
                OnPropertyChanged("EndColor");
#endif
                Reexecute();
#endif
            }
        }

        Point startPoint;
        [DataMember]
        public Point StartPoint
        {
            get { return startPoint; }
            set
            {
                if (value == startPoint)
                    return;
                startPoint = value;
#if !NET_STANDARD
#if !WINDOWS_UWP
                OnPropertyChanged("StartPoint");
#endif
                Reexecute();
#endif
            }
        }

        Point endPoint;
        [DataMember]
        public Point EndPoint
        {
            get { return endPoint; }
            set
            {
                if (value == endPoint)
                    return;
                endPoint = value;
#if !NET_STANDARD
#if !WINDOWS_UWP
                OnPropertyChanged("EndPoint");
#endif
                Reexecute();
#endif
            }
        }

        FillingBehavior fillingBehavior = FillingBehavior.BottomUp;
        [DataMember]
        public FillingBehavior FillingBehavior
        {
            get { return fillingBehavior; }
            set
            {
                if (value == fillingBehavior)
                    return;
                fillingBehavior = value;
#if !NET_STANDARD
#if !WINDOWS_UWP
                OnPropertyChanged("FillingBehavior");
#endif
                Reexecute();
#endif
            }
        }

        
        GradientType gradientType = GradientType.Linear;
        [DataMember]
        public GradientType GradientType
        {
            get { return gradientType; }
            set
            {
                if (value == gradientType)
                    return;
                gradientType = value;
#if !NET_STANDARD
#if !WINDOWS_UWP
                OnPropertyChanged("GradientType");
#endif
                Reexecute();
#endif
            }
        }

        double radialRadiusX = 1;
        [DataMember]
        public double RadialRadiusX
        {
            get { return radialRadiusX; }
            set
            {
                if (value == radialRadiusX)
                    return;
                radialRadiusX = value;
#if !NET_STANDARD
#if !WINDOWS_UWP
                OnPropertyChanged("RadialRadiusX");
#endif
                Reexecute();
#endif
            }
        }

        double radialRadiusY = 1;
        [DataMember]
        public double RadialRadiusY
        {
            get { return radialRadiusY; }
            set
            {
                if (value == radialRadiusY)
                    return;
                radialRadiusY = value;
#if !NET_STANDARD
#if !WINDOWS_UWP
                OnPropertyChanged("RadialRadiusY");
#endif
                Reexecute();
#endif
            }
        }

#endregion

#region Methods

        [OnDeserialized]
        public void PostInitialize(StreamingContext context)
        {
#if !NET_STANDARD
            lockObject = new Object();
#endif
        }

        Color GetEndColor()
        {
#if !NET_STANDARD
            if (currentTarget == 0)
#endif
                return StartColor;
#if !NET_STANDARD
            return Blend(EndColor, StartColor, currentTarget);
#endif
        }

#if !NET_STANDARD
        public static Color Blend(Color color, Color backColor, double amount)
        {
            byte r = (byte)((color.R * amount) + backColor.R * (1 - amount));
            byte g = (byte)((color.G * amount) + backColor.G * (1 - amount));
            byte b = (byte)((color.B * amount) + backColor.B * (1 - amount));
            byte a = (byte)((color.A * amount) + backColor.A * (1 - amount));
            return Color.FromArgb(a, r, g, b);
        }
#endif

#if !NET_STANDARD
        void CreateBrush()
        {
            lock (lockObject)
            {
#if !WINDOWS_UWP
                if (GradientType == GradientType.Radial)
                {
                    linearBrush = null;

                    //if (radialBrush != null)
                    //    return;

                    gradientStopCollection = new GradientStopCollection();
                    gradientStop3 = new GradientStop() { Color = Colors.Transparent, Offset = 0 };
                    gradientStop2 = new GradientStop() { Color = GetEndColor(), Offset = 0 };
                    gradientStop1 = new GradientStop() { Color = StartColor, Offset = 0 };
                    gradientStopCollection.Add(gradientStop1);
                    gradientStopCollection.Add(gradientStop2);
                    gradientStopCollection.Add(gradientStop3);

                    radialBrush = new RadialGradientBrush();
                    radialBrush.GradientStops = gradientStopCollection;
                    radialBrush.RadiusX = RadialRadiusX;
                    radialBrush.RadiusY = RadialRadiusY;

                    switch (FillingBehavior)
                    {
                        case FillingBehavior.Center:
                            radialBrush.Center = new Point(0.5, 0.5);
                            radialBrush.GradientOrigin = new Point(0.5, 0.5);
                            break;
                        case FillingBehavior.BottomUp:
                            radialBrush.Center = new Point(1, 1);
                            radialBrush.GradientOrigin = new Point(1, 0);
                            break;
                        case FillingBehavior.UpBottom:
                            radialBrush.Center = new Point(1, 0);
                            radialBrush.GradientOrigin = new Point(1, 1);
                            break;
                        case FillingBehavior.LeftRight:
                            radialBrush.Center = new Point(0, 1);
                            radialBrush.GradientOrigin = new Point(1, 1);
                            break;
                        case FillingBehavior.RightLeft:
                            radialBrush.Center = new Point(1, 1);
                            radialBrush.GradientOrigin = new Point(0, 1);
                            break;
                        default:
                            radialBrush.Center = StartPoint;
                            radialBrush.GradientOrigin = EndPoint;
                            break;
                    }

                    GetControlBrush();
                    SetControlBrush(radialBrush);
                }
                else if (GradientType == GradientType.Linear)
#endif
                {
#if !WINDOWS_UWP
                    radialBrush = null;
#endif
                    //if (linearBrush != null)
                    //    return;

                    gradientStopCollection = new GradientStopCollection();
                    gradientStop3 = new GradientStop() { Color = Colors.Transparent, Offset = 0 };
                    gradientStop2 = new GradientStop() { Color = GetEndColor(), Offset = 0 };
                    gradientStop1 = new GradientStop() { Color = StartColor, Offset = 0 };
                    gradientStopCollection.Add(gradientStop1);
                    gradientStopCollection.Add(gradientStop2);
                    gradientStopCollection.Add(gradientStop3);

                    linearBrush = new LinearGradientBrush();
                    linearBrush.GradientStops = gradientStopCollection;

                    switch (FillingBehavior)
                    {
                        case FillingBehavior.Center:
                            linearBrush.StartPoint = new Point(0.5, 0.5);
                            linearBrush.EndPoint = new Point(0.5, 0.5);
                            break;
                        case FillingBehavior.BottomUp:
                            linearBrush.StartPoint = new Point(1, 1);
                            linearBrush.EndPoint = new Point(1, 0);
                            break;
                        case FillingBehavior.UpBottom:
                            linearBrush.StartPoint = new Point(1, 0);
                            linearBrush.EndPoint = new Point(1, 1);
                            break;
                        case FillingBehavior.LeftRight:
                            linearBrush.StartPoint = new Point(0, 1);
                            linearBrush.EndPoint = new Point(1, 1);
                            break;
                        case FillingBehavior.RightLeft:
                            linearBrush.StartPoint = new Point(1, 1);
                            linearBrush.EndPoint = new Point(0, 1);
                            break;
                        default:
                            linearBrush.StartPoint = StartPoint;
                            linearBrush.EndPoint = EndPoint;
                            break;
                    }

                    GetControlBrush();
                    SetControlBrush(linearBrush);
                }
            }
        }

#if !WINDOWS_UWP
        static readonly String tagBackground = Properties.Settings.Default.BackgroundTag;
#else
        static readonly String tagBackground = "Background";
#endif

        void SetControlBrush(UIElement element, Brush brush, bool bChild = true)
        {
            if (element is Panel)
            {
                var panel = element as Panel;
                if (panel.Children.Count == 0)
                {
                    CommonControls.CommonProperties.SetIsBrushAnimating(element, true);
                    panel.Background = brush;
                    CommonControls.CommonProperties.SetIsBrushAnimating(element, false);
                }
                else
                {
                    foreach (UIElement child in panel.Children)
                        SetControlBrush(child, brush);
                }
            }
            else if (element is Shape)
            {
                CommonControls.CommonProperties.SetIsBrushAnimating(element, true);
                (element as Shape).Fill = brush;
                CommonControls.CommonProperties.SetIsBrushAnimating(element, false);
            }
            else if (element is Border) {
                CommonControls.CommonProperties.SetIsBrushAnimating(element, true);
                (element as Border).Background = brush;
                CommonControls.CommonProperties.SetIsBrushAnimating(element, false);
            }
            else if (element is Viewbox)
            {
                element = (element as Viewbox).Child;
                SetControlBrush(element, brush);
            }
            else if (element is ContentControl && !(element is UserControl) &&
                    (element as ContentControl).Content is UIElement)
            {
                element = (element as ContentControl).Content as UIElement;
                SetControlBrush(element, brush);
            }
            else if (element is Control)
            {
                CommonControls.CommonProperties.SetIsBrushAnimating(element, true);
                (element as Control).Background = brush;
                CommonControls.CommonProperties.SetIsBrushAnimating(element, false);
            }

            if (bChild && element != null)
            {
                (from c in element.GetVisualChildrenOfType<Panel>()
                 where (c.Tag as String) == tagBackground
                 select c).ToList().ForEach(child =>
                 {
                     SetControlBrush(child, brush, false);
                 });
                (from c in element.GetVisualChildrenOfType<Control>()
                 where (c.Tag as String) == tagBackground
                 select c).ToList().ForEach(child =>
                 {
                     SetControlBrush(child, brush, false);
                 });
#if !WINDOWS_UWP
                (from c in element.GetVisualChildrenOfType<System.Windows.Shapes.Shape>()
#else
                (from c in element.GetVisualChildrenOfType<Shape>()
#endif
                 where (c.Tag as String) == tagBackground
                 select c).ToList().ForEach(child =>
                 {
                     SetControlBrush(child, brush, false);
                 });
            }
        }

        void SetControlBrush(Brush brush)
        {
            SetControlBrush(Control, brush);
        }

        void GetControlBrush()
        {
            if (mapoldBrush != null)
                return;
            mapoldBrush = new Dictionary<UIElement, Brush>();
            GetControlBrush(Control);
        }

        void RestorePreviousBruhes()
        {
            if (mapoldBrush == null)
                return;
            mapoldBrush.Keys.ToList().ForEach(element =>
                {
                    SetControlBrush(element, mapoldBrush[element]);
                });
        }

        void GetControlBrush(UIElement element, bool bChild = true)
        {
            if (element != null)
            {
                if (mapoldBrush.ContainsKey(element))
                    mapoldBrush.Remove(element);
            }

            if (element is Panel)
            {
                var panel = element as Panel;
                mapoldBrush.Add(element, panel.Background);
                foreach (UIElement child in panel.Children)
                    GetControlBrush(child);
            }
            else if (element is Shape)
                mapoldBrush.Add(element, (element as Shape).Fill);
            else if (element is Border)
                mapoldBrush.Add(element, (element as Border).Background);
            else if (element is Viewbox)
            {
                element = (element as Viewbox).Child;
                GetControlBrush(element);
            }
            else if (element is ContentControl && !(element is UserControl) &&
                (element as ContentControl).Content is UIElement)
            {
                element = (element as ContentControl).Content as UIElement;
                GetControlBrush(element);
            }
            else if (element is Control)
                mapoldBrush.Add(element, (element as Control).Background);

            if (bChild && element != null)
            {
                (from c in element.GetChildrenOfType<Panel>()
                 where (c.Tag as String) == tagBackground
                 select c).ToList().ForEach(child =>
                 {
                     GetControlBrush(child, false);
                 });
                (from c in element.GetChildrenOfType<Control>()
                 where (c.Tag as String) == tagBackground
                 select c).ToList().ForEach(child =>
                 {
                     GetControlBrush(child, false);
                 });
#if !WINDOWS_UWP
                (from c in element.GetChildrenOfType<System.Windows.Shapes.Shape>()
#else
                (from c in element.GetChildrenOfType<Shape>()
#endif
                 where (c.Tag as String) == tagBackground
                 select c).ToList().ForEach(child =>
                 {
                     GetControlBrush(child, false);
                 });
            }
        }

        void CreateAnimationOnControl(DoubleAnimation da, int index)
        {
            CreateAnimationOnControl(Control, da, index);
        }

        void CreateAnimationOnControl(UIElement element, DoubleAnimation da, int index, bool bChild = true)
        {
            if (element is Shape)
                Storyboard.SetTargetProperty(da,
#if !WINDOWS_UWP
                        new PropertyPath(String.Format("(Shape.Fill).(GradientStops)[{0}].(Offset)", index)));
#else
                        String.Format("(Shape.Fill).(GradientStops)[{0}].(Offset)", index));
#endif
            else if (element is Panel)
            {
                var panel = element as Panel;
                if (panel.Children.Count == 0)
                    Storyboard.SetTargetProperty(da,
#if !WINDOWS_UWP
                        new PropertyPath(String.Format("(Panel.Background).(GradientStops)[{0}].(Offset)", index)));
#else
                        String.Format("(Panel.Background).(GradientStops)[{0}].(Offset)", index));
#endif
                else
                {
                    foreach(UIElement child in panel.Children)
                        CreateAnimationOnControl(child, da, index);
                }
            }
            if (element is Border)
                Storyboard.SetTargetProperty(da,
#if !WINDOWS_UWP
                        new PropertyPath(String.Format("(Border.Background).(GradientStops)[{0}].(Offset)", index)));
#else
                        String.Format("(Border.Background).(GradientStops)[{0}].(Offset)", index));
#endif
            else if (element is Viewbox)
            {
                element = (element as Viewbox).Child;
                CreateAnimationOnControl(element, da, index);
            }
            else if (element is ContentControl && !(element is UserControl) &&
                (element as ContentControl).Content is UIElement)
            {
                element = (element as ContentControl).Content as UIElement;
                CreateAnimationOnControl(element, da, index);
            }
            else if (element is Control)
                Storyboard.SetTargetProperty(da,
#if !WINDOWS_UWP
                    new PropertyPath(String.Format("(Control.Background).(GradientStops)[{0}].(Offset)", index)));
#else
                    String.Format("(Control.Background).(GradientStops)[{0}].(Offset)", index));
#endif
            if (bChild && element != null)
            {
                (from c in element.GetVisualChildrenOfType<Panel>()
                 where (c.Tag as String) == tagBackground
                 select c).ToList().ForEach(child =>
                 {
                     CreateAnimationOnControl(element, da, index, false);
                 });
                (from c in element.GetVisualChildrenOfType<Control>()
                 where (c.Tag as String) == tagBackground
                 select c).ToList().ForEach(child =>
                 {
                     CreateAnimationOnControl(element, da, index, false);
                 });
#if !WINDOWS_UWP
                (from c in element.GetChildrenOfType<System.Windows.Shapes.Shape>()
#else
                (from c in element.GetChildrenOfType<Shape>()
#endif
                 where (c.Tag as String) == tagBackground
                 select c).ToList().ForEach(child =>
                 {
                     CreateAnimationOnControl(element, da, index, false);
                 });
            }
        }

        double currentTarget = 0;
        void CreateAnimation(double target)
        {
            currentTarget = target;
            CreateAnimation(Control, target);
        }

        bool bTryFirstException;
        void CreateAnimation(UIElement element, double target)
        {
            if (element is Panel)
            {
                var panel = element as Panel;
                if (panel.Children.Count > 0)
                {
                    foreach (UIElement child in panel.Children)
                        CreateAnimation(child, target);
                    return;
                }
            }
            else if (element is Viewbox)
            {
                element = (element as Viewbox).Child;
                CreateAnimation(element, target);
                return;
            }
            else if (element is ContentControl && !(element is Border) && !(element is UserControl) &&
                (element as ContentControl).Content is UIElement)
            {
                element = (element as ContentControl).Content as UIElement;
                CreateAnimation(element, target);
                return;
            }

            if (element != null)
            {
                Storyboard sb = new Storyboard();

                try
                {
                    DoubleAnimation da1 = new DoubleAnimation()
                    {
                        From = fromValue,
                        To = target,
                        Duration = new Duration(TimeSpan.FromMilliseconds(AnimationTime)),
                        EasingFunction = AnimationEquation
                    };

                    if (Repeatable)
                    {
                        da1.AutoReverse = true;
                        da1.RepeatBehavior = RepeatBehavior.Forever;
                    }

                    sb.Children.Add(da1);
                    CreateAnimationOnControl(da1, 1);
                    Storyboard.SetTarget(da1, element);

                    DoubleAnimation da2 = new DoubleAnimation()
                    {
                        From = fromValue,
                        To = target,
                        Duration = new Duration(TimeSpan.FromMilliseconds(AnimationTime)),
                        EasingFunction = AnimationEquation
                    };

                    if (Repeatable)
                    {
                        da2.AutoReverse = true;
                        da2.RepeatBehavior = RepeatBehavior.Forever;
                    }

                    sb.Children.Add(da2);
                    CreateAnimationOnControl(da2, 2);
                    Storyboard.SetTarget(da2, element);

                    CommonControls.CommonProperties.SetIsBrushAnimating(element, true);

                    da2.Completed += (sender, e) =>
                    {
                        CommonControls.CommonProperties.SetIsBrushAnimating(element, false);
                    };
                    sb.Begin();
                }
                catch (Exception ex)
                {
                    if (!bTryFirstException)
                    {
                        bTryFirstException = true;
                        try
                        {
                            Reexecute();
                        }
                        finally
                        {
                            bTryFirstException = false;
                        }
                    }
                    return;
                }

                fromValue = target;
            }
        }
#endif
#endregion

#region Overrides
#if !NET_STANDARD
        protected override void Reexecute()
        {
            linearBrush = null;
#if !WINDOWS_UWP
            radialBrush = null;
#endif
            RestorePreviousBruhes();

            base.Reexecute();
        }

#if !WINDOWS_UWP
        protected override String PerformValidation(String propertyName)
        {
            if (propertyName == "AnimationBehavior")
            {
                if (AnimationBehavior == AnimationBehavior.Trigger)
                    return Properties.Resources.InvalidAnimationBehaviour;
            }
            
            return base.PerformValidation(propertyName);
        }
        public override bool this[string propertyName]
        {
            get
            {
                if (propertyName == "AnimationEquation" || propertyName == "Autoreverse")
                {
                    return false;
                }

                if (propertyName == "TagMinValue" || propertyName == "TagMaxValue")
                {
                    return AnimationBehavior == AnimationBehavior.Proportional;
                }

                return base[propertyName];
            }
        }
        public override void Demo()
        {
            currentTarget = CommonTarget;
            CreateBrush();
            CreateAnimation(CommonTarget);
            base.Demo();
        }
#endif
        public override void Execute()
        {
            currentTarget = dTargetValue;
            CreateBrush();
            CreateAnimation(dTargetValue);
        }

        public override void Stop()
        {
            RestorePreviousBruhes();
#if !WINDOWS_UWP
            radialBrush = null;
#endif
            linearBrush = null;
            if (bDemoMode)
                mapoldBrush = null;
            base.Stop();
        }
#endif
        public override String Name
        {
            get
            {
                return Properties.Resources.FillingName;
            }
        }

#if !WINDOWS_UWP
        [Browsable(false)]
#endif
        public override double CommonTarget
        {
            get
            {
                return Offset;
            }
        }

#if !WINDOWS_UWP
        [Browsable(false)]
#endif
        public override bool Is2D
        {
            get
            {
                return true;
            }
        }

#if !WINDOWS_UWP
        [Browsable(false)]
        public override String AnimationSummary
        {
            get
            {
                return base.AnimationSummary;
            }
        }
#endif

#endregion
    }
}
