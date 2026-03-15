using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;
#if !NET_STANDARD
using System.Windows;
using Utilities.Animations;
using Utilities.WPF;
#endif
#if !WINDOWS_UWP
#if !NET_STANDARD
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Shapes;
using System.Windows.Data;
#endif
using System.ComponentModel;
#else
using Windows.UI.Xaml;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Shapes;
using Windows.Foundation;
#endif
using UFInterfaces;
using DocumentManager.ComponentService;
using WPFUtilities;

namespace AnimationManager
{
    [DataContract(Name = "ComposedMovementAnimation")]
    public class ComposedMovementAnimation : AnimationManager
    {
#region Declarations
#if !NET_STANDARD
        List<double> composedMoveHip;
        PointCollection points;
        PointCollection composedMovePoint;
        double dCompletePath;

#if !WINDOWS_UWP
        PropertyChangeNotifier notifierX;
        PropertyChangeNotifier notifierY;
        PropertyChangeNotifier notifierVisibility;
#endif
        TranslateTransform translateTransform;
        double transformX;
        double transformY;
#endif
#endregion

#region Properties

        String pathControl;
        [DataMember]
        public String PathControl
        {
            get { return pathControl; }
            set
            {
                if (value == pathControl)
                    return;
                pathControl = value;
#if !NET_STANDARD
#if !WINDOWS_UWP
                OnPropertyChanged("PathControl");
#endif
                dCompletePath = Double.NaN;
                Reexecute();
#endif
            }
        }

        bool center;
        [DataMember]
        public bool Center
        {
            get { return center; }
            set
            {
                if (value == center)
                    return;
                center = value;
#if !NET_STANDARD
#if !WINDOWS_UWP
                OnPropertyChanged("Center");
#endif
                Reexecute();
#endif
            }
        }

        bool hidePath;
        [DataMember]
        public bool HidePath
        {
            get { return hidePath; }
            set
            {
                if (value == hidePath)
                    return;
                hidePath = value;
#if !NET_STANDARD
#if !WINDOWS_UWP
                OnPropertyChanged("HidePath");
#endif
                Reexecute();
#endif
            }
        }

#endregion

#region Methods
#if !NET_STANDARD
        void FindPoints()
        {
            if (Control == null || String.IsNullOrEmpty(PathControl))
                return;
            FrameworkElement canvas = Control.FindParent<Canvas>();
            //if (canvas == null)
            //    canvas = Control.FindParent<InkCanvas>();
            if (canvas != null)
            {
                var control = canvas.FindName(PathControl) as Shape;
                if (control == null)
                    return;
                double top, left;
                //if (canvas is InkCanvas)
                //{
                //    top = InkCanvas.GetTop(control);
                //    left = InkCanvas.GetLeft(control);
                //}
                //else
                {
                    top = Canvas.GetTop(control);
                    left = Canvas.GetLeft(control);
                }
                if (control is Polygon)
                {
                    points = (control as Polygon).Points;
                    if (points != null && points.Count > 0)
                        points.Add(points[0]);
                }
                else if (control is Polyline)
                    points = (control as Polyline).Points;
                else if (control is Line)
                {
                    var line = control as Line;
                    points = new PointCollection();
                    points.Add(new Point(line.X1, line.Y1));
                    points.Add(new Point(line.X2, line.Y2));
                }
                if (!Double.IsNaN(top) && !Double.IsNaN(left) && points != null)
                {
                    var offsetCollection = new PointCollection();
                    foreach (var pt in points)
                    {
#if !WINDOWS_UWP
                        pt.Offset(left, top);
#endif
                        offsetCollection.Add(pt);
                    }
                    points = offsetCollection;
                }

                if (!bDemo && HidePath)
                    control.Visibility = Visibility.Collapsed;
            }
        }

        Point GetCurrentPosition()
        {
            var ret = new Point(0, 0);
            if (Control == null)
                return ret;
            FrameworkElement canvas = Control.FindParent<Canvas>();
            //if (canvas == null)
            //    canvas = Control.FindParent<InkCanvas>();
            if (canvas != null)
            {
                double top, left;
                //if (canvas is InkCanvas)
                //{
                //    top = InkCanvas.GetTop(Control);
                //    left = InkCanvas.GetLeft(Control);
                //}
                //else
                {
                    top = Canvas.GetTop(Control);
                    left = Canvas.GetLeft(Control);
                }
                ret = new Point(left, top);
            }

            if (Center)
            {
                var fe = Control as FrameworkElement;
#if !WINDOWS_UWP
                ret.Offset(fe.ActualWidth / 2, fe.ActualHeight / 2);
#endif
            }
            return ret;
        }

        double CalculateComposedMoveMaxPath()
        {
            FindPoints();
            if (points == null)
                return 0;
            if (composedMoveHip == null)
                composedMoveHip = new List<double>();
            else
                composedMoveHip.Clear();

            Point pt1, pt2;
            double nCat1, nCat2;
            double dPath = 0;
            for (int i = 0; points.Count > 0 && i < points.Count - 1; i++)
            {
                pt1 = points[i];
                pt2 = points[i + 1];

                nCat1 = Math.Abs((double)(pt2.X - pt1.X));
                nCat2 = Math.Abs((double)(pt2.Y - pt1.Y));
                var mathSqrt = Math.Sqrt((nCat1 * nCat1) + (nCat2 * nCat2));
                if (mathSqrt == 0)
                    mathSqrt = 0.0000000000000000000000000000000000000000001;
                composedMoveHip.Add(mathSqrt);
                dPath += mathSqrt;
            }

            return dPath;
        }

        double dLastTargetValue;
        void FindComposedMoveMaxPath()
        {
            if (points == null || points.Count == 0)
                return;
            if (composedMoveHip == null)
                CalculateComposedMoveMaxPath();
            if (composedMovePoint == null)
                composedMovePoint = new PointCollection();
            else
                composedMovePoint.Clear();

            if (dLastTargetValue == dTargetValue)
                return;

            Point pt1, pt2;
            double nCat1, nCat2;
            double dPath = 0;
            double dPrevPath;
            double dHipot;

            for (int i = 0; points.Count > 0 && i < points.Count; i++)
            {
                if (i < points.Count - 1)
                {
                    dPrevPath = dPath;
                    dPath += composedMoveHip[i];
                    if (dLastTargetValue < dPath)
                    {
                        pt1 = points[i];
                        pt2 = points[i + 1];

                        nCat1 = pt2.X - pt1.X;
                        nCat2 = pt2.Y - pt1.Y;

                        dHipot = dLastTargetValue - dPrevPath;

                        composedMovePoint.Add(new Point(pt1.X + (nCat1 * dHipot / composedMoveHip[i]),
                                                        pt1.Y + (nCat2 * dHipot / composedMoveHip[i])));
                        break;
                    }
                }
            }

            if (Double.IsNaN(dLastTargetValue) || dLastTargetValue < dTargetValue)
            {
                if (composedMovePoint.Count == 0)
                    composedMovePoint.Add(GetCurrentPosition());

                dPath = 0;
                for (int i = 0; points.Count > 0 && i < points.Count; i++)
                {
                    if (i < points.Count - 1)
                    {
                        dPrevPath = dPath;
                        dPath += composedMoveHip[i];
                        if (dTargetValue < dPath)
                        {
                            pt1 = points[i];
                            pt2 = points[i + 1];

                            nCat1 = pt2.X - pt1.X;
                            nCat2 = pt2.Y - pt1.Y;

                            dHipot = dTargetValue - dPrevPath;

                            composedMovePoint.Add(new Point(pt1.X + (nCat1 * dHipot / composedMoveHip[i]),
                                                            pt1.Y + (nCat2 * dHipot / composedMoveHip[i])));
                            dLastTargetValue = dTargetValue;
                            return;
                        }
                        else if (dLastTargetValue < dPath)
                            composedMovePoint.Add(points[i + 1]);
                    }
                    else
                        composedMovePoint.Add(points[i]);
                }
            }
            else
            {
                if (composedMovePoint.Count == 0)
                    composedMovePoint.Add(points[points.Count - 1]);

                dPath = dCompletePath;
                for (int i = points.Count - 2; i >= 0; i--)
                {
                    dPrevPath = dPath;
                    dPath -= composedMoveHip[i];
                    if (dTargetValue > dPath)
                    {
                        /*
                        pt1 = points[i - 1];
                        pt2 = points[i];

                        nCat1 = pt2.X - pt1.X;
                        nCat2 = pt2.Y - pt1.Y;

                        dHipot = dPath - dTargetValue;

                        composedMovePoint.Add(new Point(pt2.X - (nCat1 * dHipot / composedMoveHip[i - 1]),
                                                        pt2.Y - (nCat2 * dHipot / composedMoveHip[i - 1])));
                        dLastTargetValue = dTargetValue;
                        */

                        dPath = 0;
                        for (i = 0; points.Count > 0 && i < points.Count; i++)
                        {
                            if (i < points.Count - 1)
                            {
                                dPrevPath = dPath;
                                dPath += composedMoveHip[i];
                                if (dTargetValue < dPath)
                                {
                                    pt1 = points[i];
                                    pt2 = points[i + 1];

                                    nCat1 = pt2.X - pt1.X;
                                    nCat2 = pt2.Y - pt1.Y;

                                    dHipot = dTargetValue - dPrevPath;

                                    composedMovePoint.Add(new Point(pt1.X + (nCat1 * dHipot / composedMoveHip[i]),
                                                                    pt1.Y + (nCat2 * dHipot / composedMoveHip[i])));
                                    dLastTargetValue = dTargetValue;
                                    break;
                                }
                            }
                        }
                        return;
                    }
                    else if (dLastTargetValue > dPath)
                        composedMovePoint.Add(points[i]);
                }
            }
            dLastTargetValue = dTargetValue;
        }

        void CleanPoints()
        {
            for(int i = 0; i < composedMovePoint.Count - 1; ++i)
            {
                if (composedMovePoint[i] == composedMovePoint[i + 1])
                {
                    composedMovePoint.RemoveAt(i + 1);
                    i = -1;
                } 
            }
        }
#endif
#endregion

#region Overrides
#if !NET_STANDARD
        public override void Init(IEntityReference entity, IDocument parent, String sessionname)
        {
            base.Init(entity, parent, sessionname);
            if (AnimationBehavior == AnimationBehavior.Trigger)
                dLastTargetValue = 0;
            else
                dLastTargetValue = Double.NaN;
            dCompletePath = CalculateComposedMoveMaxPath();
        }

        public override bool IsFreezable
        {
            get
            {
                return false;
            }
        }

        void SetCenterPositionBinding()
        {
            if (Control == null ||
#if !WINDOWS_UWP
                notifierX != null || notifierY != null || notifierVisibility != null ||
#endif
                !Center)
                return;

            var fe = Control as FrameworkElement;
            translateTransform = Utilities.Animations.Animations.SetTransform<TranslateTransform>(fe, true);
            transformX = translateTransform.X;
            transformY = translateTransform.Y;
            translateTransform.X = -(fe.ActualWidth / 2);
            translateTransform.Y = -(fe.ActualHeight / 2);

#if !WINDOWS_UWP
            if (notifierX == null)
            {
                var propDesc = DependencyPropertyDescriptor.FromProperty(FrameworkElement.ActualWidthProperty, typeof(FrameworkElement));
                notifierX = new PropertyChangeNotifier(translateTransform, propDesc.Name);
                notifierX.ValueChanged += (o, e) =>
                {
                    translateTransform.X = -(fe.ActualWidth / 2);
                };
            }

            if (notifierY == null)
            {
                var propDesc = DependencyPropertyDescriptor.FromProperty(FrameworkElement.ActualHeightProperty, typeof(FrameworkElement));
                notifierY = new PropertyChangeNotifier(translateTransform, propDesc.Name);
                notifierY.ValueChanged += (o, e) =>
                {
                    translateTransform.Y = -(fe.ActualHeight / 2);
                };
            }

            if (notifierVisibility == null)
            {
                var propDesc = DependencyPropertyDescriptor.FromProperty(FrameworkElement.VisibilityProperty, typeof(FrameworkElement));
                notifierVisibility = new PropertyChangeNotifier(fe, propDesc.Name);
                notifierVisibility.ValueChanged += (o, e) =>
                {
                    if (fe.IsVisible)
                    {
                        translateTransform.X = -(fe.ActualWidth / 2);
                        translateTransform.Y = -(fe.ActualHeight / 2);
                        Reexecute();
                    }
                };
            }
#endif
        }

        bool bDemo;
#if !WINDOWS_UWP
        protected override String PerformValidation(String propertyName)
        {
            return base.PerformValidation(propertyName);
        }
        public override bool this[string propertyName]
        {
            get
            {
                if (propertyName == "AnimationEquation")
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
            bDemo = true;
            dTargetValue = CommonTarget;
            dTargetValue = Math.Max(dTargetValue, Range.Low);
            dTargetValue = Math.Min(dTargetValue, dCompletePath);
            // CalculateComposedMoveMaxPath();
            SetCenterPositionBinding();
            FindComposedMoveMaxPath();
            if (composedMovePoint == null || composedMovePoint.Count <= 1)
                return;
            CleanPoints();
            Control.MoveXYAlongPath(composedMovePoint.ToArray(), AnimationTime, Repeatable, Autoreverse);
            base.Demo();
        }
#endif

        bool bPendingLoaded;
        public override void Execute()
        {
            if (Control == null)
                return;
            var fe = Control as FrameworkElement;
            if (!fe.IsLoaded)
            {
                if (!bPendingLoaded)
                {
                    bPendingLoaded = true;
                    fe.Loaded += Fe_Loaded;
                }
                return;
            }

            bDemo = false;
            dTargetValue = Math.Max(dTargetValue, Range.Low);
            dTargetValue = Math.Min(dTargetValue, dCompletePath);
            SetCenterPositionBinding();
            FindComposedMoveMaxPath();
            if (composedMovePoint == null || composedMovePoint.Count <= 1)
                return;
            CleanPoints();
#if !WINDOWS_UWP
            Control.MoveXYAlongPath(composedMovePoint.ToArray(), AnimationTime, Repeatable, Autoreverse);
#endif
        }

        private void Fe_Loaded(object sender, RoutedEventArgs e)
        {
            var fe = Control as FrameworkElement;
            if (bPendingLoaded)
            {
                bPendingLoaded = false;
                fe.Loaded -= Fe_Loaded;
                Execute();
            }
        }

        public override void Stop()
        {
            var fe = Control as FrameworkElement;
            if (bPendingLoaded)
            {
                bPendingLoaded = false;
                fe.Loaded -= Fe_Loaded;
            }

#if !WINDOWS_UWP
            if (notifierX != null)
            {
                notifierX.Dispose();
                notifierX = null;
            }
            if (notifierY != null)
            {
                notifierY.Dispose();
                notifierY = null;
            }
            if (notifierVisibility != null)
            {
                notifierVisibility.Dispose();
                notifierVisibility = null;
            }
#endif
            if (translateTransform != null)
            {
                translateTransform.X = transformX;
                translateTransform.Y = transformY;
            }

#if !WINDOWS_UWP
            if (Control != null)
                Control.MoveXYAlongPath(null, -1, Repeatable, Autoreverse);
#endif
            dLastTargetValue = 0;
            base.Stop();
        }
#endif
        public override double CommonTarget
        {
            get
            {
#if !NET_STANDARD
                if (Double.IsNaN(dCompletePath))
                    dCompletePath = CalculateComposedMoveMaxPath();
                return dCompletePath;
#else
                return base.CommonTarget;
#endif
            }
        }
        public override String Name
        {
            get
            {
                return Properties.Resources.ComposedMovementName;
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
#endregion
    }
}
