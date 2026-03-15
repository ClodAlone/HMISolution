using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using DevExpress.XtraReports.Diagnostics;
#if !WINDOWS_UWP
using System.Windows.Media.Animation;
using System.Windows.Media;
using System.Windows.Data;
using System.Windows.Media.Effects;
using System.Diagnostics;
using System.Windows.Controls;

using Utilities.Animations.Converters;
using Utilities.WPF;
using System.Windows.Shapes;
#else
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Shapes;
using Windows.UI;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Media.Animation;
using Windows.Foundation;
#endif

namespace Utilities.Animations
{
#if WINDOWS_UWP
    static class WINDOWS_UWPHelpers 
    {
        static Dictionary<DependencyObject, Dictionary<DependencyProperty, Storyboard>> mapStoryboards = new Dictionary<DependencyObject, Dictionary<DependencyProperty, Storyboard>>();

        public static void AddStoryBoard(this DependencyObject obj, Storyboard storyboard, DependencyProperty property)
        {
            if (!mapStoryboards.ContainsKey(obj))
                mapStoryboards.Add(obj, new Dictionary<DependencyProperty, Storyboard>());
            if (mapStoryboards[obj].ContainsKey(property))
            {
                mapStoryboards[obj][property].Stop();
                mapStoryboards[obj].Remove(property);
            }
            mapStoryboards[obj].Add(property, storyboard);
        }

        public static void BeginAnimation(this DependencyObject obj, DependencyProperty property,
#if WINDOWS_UWP
            String name,
#endif                        
            Timeline animation = null) 
        {
            if (animation != null)
            {
                var storyboard = new Storyboard();

                storyboard.Children.Add(animation);
                Storyboard.SetTarget(storyboard, obj);
                Storyboard.SetTargetProperty(storyboard, name);

                storyboard.Begin();

                if (!mapStoryboards.ContainsKey(obj))
                    mapStoryboards.Add(obj, new Dictionary<DependencyProperty, Storyboard>());
                if (mapStoryboards[obj].ContainsKey(property))
                {
                    mapStoryboards[obj][property].Stop();
                    mapStoryboards[obj].Remove(property);
                }
                mapStoryboards[obj].Add(property, storyboard); 
            }
            else
            {
                if (mapStoryboards.ContainsKey(obj))
                {
                    var list = new List<Storyboard>();
                    var Dic = mapStoryboards[obj];
                    if (Dic.ContainsKey(property))
                    {
                        Dic[property].Stop();
                        Dic.Remove(property);
                    }
                }
            }
        }
    }
#endif

    public static class Animations
    {
        private static T FindElementOfType<T>(DependencyObject element) where T : DependencyObject
        {
            T found = element as T;

            if (found != null)
            {
                return found;
            }

            for (int i = 0; i < VisualTreeHelper.GetChildrenCount(element); i++)
            {
                found = FindElementOfType<T>(VisualTreeHelper.GetChild(element, i));
                if (found != null)
                {
                    return found;
                }
            }

            return null;
        }

        public static void RemoveTransform(this UIElement control, Transform transform)
        {
            if (control.RenderTransform == null)
                return;

            if (control.RenderTransform == transform)
                control.RenderTransform = null;
            else if (control.RenderTransform is TransformGroup)
            {
                var tg = control.RenderTransform as TransformGroup;
                if (tg.Children.Contains(transform))
                    tg.Children.Remove(transform);
            }

            lock (listCreatedTransform)
            {
                if (listCreatedTransform.Contains(transform))
                    listCreatedTransform.Remove(transform);
            }
        }

        public static int GetTransformIndex<T>(this UIElement control) where T : Transform, new()
        {
            if (control.RenderTransform == null)
                return -1;

            if (!(control.RenderTransform is TransformGroup))
            {
                TransformGroup newGroup = new TransformGroup();
                newGroup.Children.Add(control.RenderTransform);
                control.RenderTransform = newGroup;
            }

            TransformGroup tg = control.RenderTransform as TransformGroup;

            lock (listCreatedTransform)
            {
                var tf = from fx in tg.Children
                         where fx is T && listCreatedTransform.Contains(fx)
                         select fx;
                if (tf.Count() == 0)
                    return -1;

                return tg.Children.IndexOf(tf.First() as T);
            }
        }

        readonly static List<Transform> listCreatedTransform = new List<Transform>();
        public static T SetTransform<T>(this UIElement control, bool bSet, bool bGetAvailable = false) where T : Transform, new()
        {
            lock (listCreatedTransform)
            {
                if (control.ReadLocalValue(UIElement.RenderTransformOriginProperty) == DependencyProperty.UnsetValue)
                    control.RenderTransformOrigin = new Point(0.5, 0.5);

                T transform = null;
                if (bSet)
                {
                    if (control.RenderTransform == null ||
                        !(control.RenderTransform is TransformGroup))
                    {
                        if (control.RenderTransform is T && (bGetAvailable && !listCreatedTransform.Contains(control.RenderTransform) ||
                            !bGetAvailable && listCreatedTransform.Contains(control.RenderTransform)))
                            transform = control.RenderTransform as T;
                        else
                        {
                            TransformGroup tg = new TransformGroup();
                            if (control.RenderTransform != null && control.RenderTransform is T &&
                                listCreatedTransform.Contains(control.RenderTransform))
                            {
                                tg.Children.Add(control.RenderTransform);
                                transform = control.RenderTransform as T;
                            }
                            else
                            {
                                transform = new T();
                                if (!bGetAvailable)
                                    listCreatedTransform.Add(transform);
                                if (control.RenderTransform != null)
                                    tg.Children.Add(control.RenderTransform);
                                tg.Children.Add(transform);
                            }
                            control.RenderTransform = tg;
                        }
                    }
                    else if (control.RenderTransform is TransformGroup)
                    {
                        TransformGroup tg = control.RenderTransform as TransformGroup;

                        var tf = from fx in tg.Children
                                 where fx is T && (bGetAvailable && !listCreatedTransform.Contains(fx) || 
                                                  !bGetAvailable && listCreatedTransform.Contains(fx))
                                 select fx;
                        if (tf.Count() == 0)
                        {
                            transform = new T();
                            if (!bGetAvailable)
                                listCreatedTransform.Add(transform);
                            tg.Children.Add(transform);
                        }
                        else
                            transform = tf.First() as T;
                    }
                }
                else
                {
                    if (control.RenderTransform is T)
                        control.RenderTransform = null;
                    else if (control.RenderTransform is TransformGroup)
                    {
                        TransformGroup tg = control.RenderTransform as TransformGroup;

                        TransformGroup tgcoll = new TransformGroup();
                        foreach (Transform t in tg.Children)
                        {
                            if (!(t is T))
                                tgcoll.Children.Add(t);
                        }
                        if (tgcoll.Children.Count == 0)
                            control.RenderTransform = null;
                        else
                            control.RenderTransform = tgcoll;
                    }
                }

                return transform;
            }
        }

        //System.Windows.Media.Animation ..::.BackEase 
        //  System.Windows.Media.Animation ..::.BounceEase 
        //  System.Windows.Media.Animation ..::.CircleEase 
        //  System.Windows.Media.Animation ..::.CubicEase 
        //  System.Windows.Media.Animation ..::.ElasticEase 
        //  System.Windows.Media.Animation ..::.ExponentialEase 
        //  System.Windows.Media.Animation ..::.PowerEase 
        //  System.Windows.Media.Animation ..::.QuadraticEase 
        //  System.Windows.Media.Animation ..::.QuarticEase 
        //  System.Windows.Media.Animation ..::.QuinticEase 
        //  System.Windows.Media.Animation ..::.SineEase 

        public static void Animate(this UIElement control, DependencyProperty dp,
#if WINDOWS_UWP
                                    String name,
#endif                        
                                    double target, int milliseconds,
                                    EasingFunctionBase type, EventHandler 
#if WINDOWS_UWP
                                    <Object>
#endif                        
                                    completed = null)
        {
            if (control == null)
                throw new ArgumentNullException("control", "control is null.");

            if (milliseconds < 0)
                control.BeginAnimation(dp, null);
            else
            {
                if (Double.IsNaN(target))
                    return;

                control.BeginAnimation(dp, null);
                if (milliseconds == 0)
                {
                    control.SetValue(dp, target);
                }
                else
                {
                    DoubleAnimation da = new DoubleAnimation()
                    {
                        To = target,
                        Duration = new Duration(TimeSpan.FromMilliseconds(milliseconds)),
                        EasingFunction = type
                    };
                    if (completed != null)
                        da.Completed += completed;
                    control.BeginAnimation(dp,
#if WINDOWS_UWP
                    name, 
#endif
                        da);
                }
            }
        }

        public static void Animate(this UIElement control, DependencyProperty[] dpArray,
#if WINDOWS_UWP
                                    String[] nameArray,
#endif
                                double target, int milliseconds,
                                EasingFunctionBase type, EventHandler
#if WINDOWS_UWP
                                    <Object>
#endif
            completed = null)
        {
            if (control == null)
                throw new ArgumentNullException("control", "control is null.");

            if (milliseconds < 0)
            {
                foreach(var dp in  dpArray)
                    control.BeginAnimation(dp, null);
            }
            else
            {
                if (Double.IsNaN(target))
                    return;

                foreach(var dp in dpArray)
                    control.BeginAnimation(dp, null);
                if(milliseconds == 0)
                {
                    foreach(DependencyProperty dp in dpArray)
                    {
                        control.SetValue(dp, target);
                    }
                }
                Storyboard sb = new Storyboard();
                if (completed != null)
                    sb.Completed += completed;

#if WINDOWS_UWP
                foreach(var name in nameArray)
                {
                    DoubleAnimation da = new DoubleAnimation()
                    {
                        To = target,
                        Duration = new Duration(TimeSpan.FromMilliseconds(milliseconds)),
                        EasingFunction = type
                    };

                    sb.Children.Add(da);
                    Storyboard.SetTargetProperty(da, name);
                    Storyboard.SetTarget(da, control);
                }
#else
                Array.ForEach(dpArray, dp =>
                {
                    DoubleAnimation da = new DoubleAnimation()
                    {
                        To = target,
                        Duration = new Duration(TimeSpan.FromMilliseconds(milliseconds)),
                        EasingFunction = type
                    };

                    sb.Children.Add(da);
                    Storyboard.SetTargetProperty(da, new PropertyPath(dp));
                    Storyboard.SetTarget(da, control);
                });
#endif

                sb.Begin();
#if WINDOWS_UWP
                foreach(var dp in dpArray)
                    control.AddStoryBoard(sb, dp);
#endif
            }
        }

        public static void Fade(this UIElement control, double targetOpacity, int milliseconds,
                                EasingFunctionBase type, EventHandler
#if WINDOWS_UWP
                                    <Object>
#endif
            completed = null, bool bSetVisibility = true)
        {
            if (control == null)
                throw new ArgumentNullException("control", "control is null.");

#if DEBUG
            System.Diagnostics.Debug.WriteLine(String.Format("Fade Control : {0}, target : {1}, ms : {2}, SetVisibility : {3}",
                (control as FrameworkElement).Name, targetOpacity, milliseconds, bSetVisibility));
#endif

            if (milliseconds < 0)
            {
                if (bSetVisibility)
                    control.Visibility = Visibility.Visible;
                else
                    control.BeginAnimation(UIElement.OpacityProperty,
    #if WINDOWS_UWP
                            "Opacity",
    #endif
                            null);
            }
            else
            {
                if (Double.IsNaN(targetOpacity))
                    return;
                //control.BeginAnimation(UIElement.OpacityProperty, null);
                if (bSetVisibility)
                    control.Visibility = targetOpacity != 0 ? Visibility.Visible : Visibility.Collapsed;
                else
                {
//                    control.BeginAnimation(UIElement.OpacityProperty,
//#if WINDOWS_UWP
//                            "Opacity",
//#endif
//                            null);

                    if (milliseconds > 0)
                    {
                        DoubleAnimation da = new DoubleAnimation()
                        {
                            To = targetOpacity,
                            Duration = new Duration(TimeSpan.FromMilliseconds(milliseconds)),
                            EasingFunction = type
                        };

                        if (completed != null)
                            da.Completed += completed;


                        control.BeginAnimation(UIElement.OpacityProperty,
#if WINDOWS_UWP
                    "Opacity",
#endif
                    da);
                    }
                    else
                        control.Opacity = targetOpacity;
                }
            }
        }

        public static void Scale(this UIElement control, double targetScale, int milliseconds,
                                 bool bForever, bool autoreverse, EasingFunctionBase type, bool bCenter = false, 
                                 EventHandler
#if WINDOWS_UWP
                                    <Object>
#endif
            completed = null, bool isRelative = false)
        {
            if (control == null)
                throw new ArgumentNullException("control", "control is null.");

            if (Double.IsNaN(targetScale))
                return;

            if (milliseconds < 0)
            {
                ScaleTransform t = SetTransform<ScaleTransform>(control, true);
#if WINDOWS_UWP
                int nTransformIndex = GetTransformIndex<ScaleTransform>(control);
                if (nTransformIndex == -1)
                    throw new ArgumentNullException("nTransformIndex");
                control.BeginAnimation(ScaleTransform.ScaleXProperty, String.Format("(UIElement.RenderTransform).(TransformGroup.Children)[{0}].(ScaleTransform.ScaleX)", nTransformIndex), null);
                control.BeginAnimation(ScaleTransform.ScaleYProperty, String.Format("(UIElement.RenderTransform).(TransformGroup.Children)[{0}].(ScaleTransform.ScaleY)", nTransformIndex), null);
#else
                t.BeginAnimation(ScaleTransform.ScaleXProperty, null);
                t.BeginAnimation(ScaleTransform.ScaleYProperty, null);
#endif
                RemoveTransform(control, t);
            }
            else
            {
                if (Double.IsNaN(targetScale))
                    return;

                ScaleTransform t = SetTransform<ScaleTransform>(control, true);

                int nTransformIndex = GetTransformIndex<ScaleTransform>(control);
                if (nTransformIndex == -1)
                    throw new ArgumentNullException("nTransformIndex");
                /*
#if WINDOWS_UWP
                control.BeginAnimation(ScaleTransform.ScaleXProperty, null);
                control.BeginAnimation(ScaleTransform.ScaleYProperty, null);
#else
                t.BeginAnimation(ScaleTransform.ScaleXProperty, null);
                t.BeginAnimation(ScaleTransform.ScaleYProperty, null);
#endif
                */
                if (bCenter)
                {
                    t.CenterX = (control as FrameworkElement).ActualWidth / 2;
                    t.CenterY = (control as FrameworkElement).ActualHeight / 2;
                }
                if (milliseconds == 0)
                {
                    t.ScaleX = isRelative ? t.ScaleX + targetScale : targetScale;
                    t.ScaleY = isRelative ? t.ScaleY + targetScale : targetScale;
                }
                else
                {
                    Storyboard sb = new Storyboard();
                    if (completed != null)
                        sb.Completed += completed;

                    DoubleAnimation dax = new DoubleAnimation()
                    {
                        To = isRelative ? t.ScaleX + targetScale : targetScale,
                        Duration = new Duration(TimeSpan.FromMilliseconds(milliseconds)),
                        EasingFunction = type
                    };
                    dax.AutoReverse = autoreverse;
                    if (bForever)
                        dax.RepeatBehavior = RepeatBehavior.Forever;
                    sb.Children.Add(dax);
                    Storyboard.SetTargetProperty(dax,
#if WINDOWS_UWP
                    String.Format("(UIElement.RenderTransform).(TransformGroup.Children)[{0}].(ScaleTransform.ScaleX)", nTransformIndex));
#else
                        new PropertyPath(String.Format("(UIElement.RenderTransform).(TransformGroup.Children)[{0}].(ScaleTransform.ScaleX)", nTransformIndex)));
#endif
                    Storyboard.SetTarget(dax, control);

                    DoubleAnimation day = new DoubleAnimation()
                    {
                        To = isRelative ? t.ScaleY + targetScale : targetScale,
                        Duration = new Duration(TimeSpan.FromMilliseconds(milliseconds)),
                        EasingFunction = type
                    };
                    day.AutoReverse = autoreverse;
                    if (bForever)
                        day.RepeatBehavior = RepeatBehavior.Forever;
                    sb.Children.Add(day);
                    Storyboard.SetTargetProperty(day,
#if WINDOWS_UWP
                    String.Format("(UIElement.RenderTransform).(TransformGroup.Children)[{0}].(ScaleTransform.ScaleY)", nTransformIndex));
#else
                        new PropertyPath(String.Format("(UIElement.RenderTransform).(TransformGroup.Children)[{0}].(ScaleTransform.ScaleY)", nTransformIndex)));
#endif
                    Storyboard.SetTarget(day, control);

                    sb.Begin();

#if WINDOWS_UWP
                control.AddStoryBoard(sb, ScaleTransform.ScaleXProperty);
                control.AddStoryBoard(sb, ScaleTransform.ScaleYProperty);
#endif
                }
            }
        }

        public static void ScaleX(this UIElement control, double targetScale, int milliseconds,
                                 bool bForever, bool autoreverse, EasingFunctionBase type, bool bCenter = false,
                                 EventHandler
#if WINDOWS_UWP
                                    <Object>
#endif
            completed = null, bool isRelative = false)
        {
            if (control == null)
                throw new ArgumentNullException("control", "control is null.");

            if (milliseconds < 0)
            {
                ScaleTransform t = SetTransform<ScaleTransform>(control, true);
#if WINDOWS_UWP
                int nTransformIndex = GetTransformIndex<ScaleTransform>(control);
                if (nTransformIndex == -1)
                    throw new ArgumentNullException("nTransformIndex");
                control.BeginAnimation(ScaleTransform.ScaleXProperty, String.Format("(UIElement.RenderTransform).(TransformGroup.Children)[{0}].(ScaleTransform.ScaleX)", nTransformIndex), null);
#else
                t.BeginAnimation(ScaleTransform.ScaleXProperty, null);
#endif
                RemoveTransform(control, t);
            }
            else
            {
                if (Double.IsNaN(targetScale))
                    return;

                ScaleTransform t = SetTransform<ScaleTransform>(control, true);

                int nTransformIndex = GetTransformIndex<ScaleTransform>(control);
                if (nTransformIndex == -1)
                    throw new ArgumentNullException("nTransformIndex");
//#if WINDOWS_UWP
//                control.BeginAnimation(ScaleTransform.ScaleXProperty, null);
//#else
//                t.BeginAnimation(ScaleTransform.ScaleXProperty, null);
//#endif

                if (bCenter)
                {
                    t.CenterX = (control as FrameworkElement).ActualWidth / 2;
                    t.CenterY = (control as FrameworkElement).ActualHeight / 2;
                }
                if (milliseconds == 0)
                {
                    t.ScaleX = targetScale;
                }
                else
                {
                    Storyboard sb = new Storyboard();
                    if (completed != null)
                        sb.Completed += completed;

                    DoubleAnimation dax = new DoubleAnimation()
                    {
                        To = isRelative ? t.ScaleX + targetScale : targetScale,
                        Duration = new Duration(TimeSpan.FromMilliseconds(milliseconds)),
                        EasingFunction = type
                    };
                    dax.AutoReverse = autoreverse;
                    if (bForever)
                        dax.RepeatBehavior = RepeatBehavior.Forever;
                    sb.Children.Add(dax);
                    Storyboard.SetTargetProperty(dax,
#if WINDOWS_UWP
                    String.Format("(UIElement.RenderTransform).(TransformGroup.Children)[{0}].(ScaleTransform.ScaleX)", nTransformIndex));
#else
                        new PropertyPath(String.Format("(UIElement.RenderTransform).(TransformGroup.Children)[{0}].(ScaleTransform.ScaleX)", nTransformIndex)));
#endif
                    Storyboard.SetTarget(dax, control);

                    sb.Begin();
#if WINDOWS_UWP
                control.AddStoryBoard(sb, ScaleTransform.ScaleXProperty);
#endif
                }
            }
        }

        public static void ScaleY(this UIElement control, double targetScale, int milliseconds,
                                 bool bForever, bool autoreverse, EasingFunctionBase type, bool bCenter = false,
                                 EventHandler
#if WINDOWS_UWP
                                    <Object>
#endif
            completed = null, bool isRelative = false)
        {
            if (control == null)
                throw new ArgumentNullException("control", "control is null.");

            if (milliseconds < 0)
            {
                ScaleTransform t = SetTransform<ScaleTransform>(control, true);
#if WINDOWS_UWP
                int nTransformIndex = GetTransformIndex<ScaleTransform>(control);
                if (nTransformIndex == -1)
                    throw new ArgumentNullException("nTransformIndex");
                control.BeginAnimation(ScaleTransform.ScaleYProperty, String.Format("(UIElement.RenderTransform).(TransformGroup.Children)[{0}].(ScaleTransform.ScaleY)", nTransformIndex), null);
#else
                t.BeginAnimation(ScaleTransform.ScaleYProperty, null);
#endif
                RemoveTransform(control, t);
            }
            else
            {
                if (Double.IsNaN(targetScale))
                    return;

                ScaleTransform t = SetTransform<ScaleTransform>(control, true);

                int nTransformIndex = GetTransformIndex<ScaleTransform>(control);
                if (nTransformIndex == -1)
                    throw new ArgumentNullException("nTransformIndex");
//#if WINDOWS_UWP
//                control.BeginAnimation(ScaleTransform.ScaleYProperty, null);
//#else
//                t.BeginAnimation(ScaleTransform.ScaleYProperty, null);
//#endif

                if (bCenter)
                {
                    t.CenterX = (control as FrameworkElement).ActualWidth / 2;
                    t.CenterY = (control as FrameworkElement).ActualHeight / 2;
                }
                if(milliseconds == 0)
                {
                    t.ScaleY = targetScale;
                }
                Storyboard sb = new Storyboard();
                if (completed != null)
                    sb.Completed += completed;

                DoubleAnimation day = new DoubleAnimation()
                {
                    To = isRelative ? t.ScaleY + targetScale : targetScale,
                    Duration = new Duration(TimeSpan.FromMilliseconds(milliseconds)),
                    EasingFunction = type
                };
                day.AutoReverse = autoreverse;
                if (bForever)
                    day.RepeatBehavior = RepeatBehavior.Forever;
                sb.Children.Add(day);
                Storyboard.SetTargetProperty(day,
#if WINDOWS_UWP
                    String.Format("(UIElement.RenderTransform).(TransformGroup.Children)[{0}].(ScaleTransform.ScaleY)", nTransformIndex));
#else
                    new PropertyPath(String.Format("(UIElement.RenderTransform).(TransformGroup.Children)[{0}].(ScaleTransform.ScaleY)", nTransformIndex)));
#endif
                Storyboard.SetTarget(day, control);

                sb.Begin();
#if WINDOWS_UWP
                control.AddStoryBoard(sb, ScaleTransform.ScaleYProperty);
#endif
            }
        }

#if !WINDOWS_UWP
        public static void ScaleEffect(this UIElement control, double from, double to, Color color, int milliseconds, bool bForever,
                                  bool autoreverse, EasingFunctionBase type, EventHandler completed = null)
        {
            if (milliseconds < 0)
            {
                if (control.Effect != null)
                {
                    control.Effect.BeginAnimation(DropShadowEffect.BlurRadiusProperty, null);
                    control.Effect = null;
                }
            }
            else
            {
                if (Double.IsNaN(from) || Double.IsNaN(to))
                    return;

                if (control.Effect != null)
                {
                    control.Effect.BeginAnimation(DropShadowEffect.BlurRadiusProperty, null);
                    control.Effect = null;
                }
                if (milliseconds == 0)
                {
                    var effect = new DropShadowEffect
                    {
                        ShadowDepth = 0,
                        BlurRadius = to,
                        Color = color
                    };
                    control.Effect = effect;
                    
                }
                else
                {
                    var effect = new DropShadowEffect
                    {
                        ShadowDepth = 0,
                        BlurRadius = from,
                        Color = color
                    };
                    control.Effect = effect;
#if !WINDOWS_UWP
                    control.ClipToBounds = false;
#endif

                    DoubleAnimation da = new DoubleAnimation()
                    {
                        To = to,
                        Duration = new Duration(TimeSpan.FromMilliseconds(milliseconds)),
                        EasingFunction = type
                    };
                    da.AutoReverse = autoreverse;
                    if (bForever)
                        da.RepeatBehavior = RepeatBehavior.Forever;
                    if (completed != null)
                        da.Completed += completed;

                    control.Effect.BeginAnimation(DropShadowEffect.BlurRadiusProperty, da);
                }
            }
        }

        public static void ScaleEffectColor(this UIElement control, double radius, Color from, Color to, int milliseconds, bool bForever,
                                  bool autoreverse, EasingFunctionBase type, EventHandler completed = null)
        {
            if (milliseconds < 0)
            {
                if (control.Effect != null)
                {
                    control.Effect.BeginAnimation(DropShadowEffect.ColorProperty, null);
                    control.Effect = null;
                }
            }
            else
            {
                if (control.Effect != null)
                {
                    control.Effect.BeginAnimation(DropShadowEffect.ColorProperty, null);
                    control.Effect = null;
                }

                var effect = new DropShadowEffect
                {
                    ShadowDepth = 0,
                    BlurRadius = radius,
                    Color = from
                };
                control.Effect = effect;
#if !WINDOWS_UWP
                control.ClipToBounds = false;
#endif
                ColorAnimation da = new ColorAnimation()
                {
                    To = to,
                    Duration = new Duration(TimeSpan.FromMilliseconds(milliseconds)),
                    EasingFunction = type
                };
                da.AutoReverse = autoreverse;
                if (bForever)
                    da.RepeatBehavior = RepeatBehavior.Forever;
                if (completed != null)
                    da.Completed += completed;

                control.Effect.BeginAnimation(DropShadowEffect.ColorProperty, da);
            }
        }
#endif

        public static void SolidBrushColorAnimation(this SolidColorBrush control, Color from, Color to, int milliseconds, bool bForever,
                                  bool autoreverse, EasingFunctionBase type, EventHandler
#if WINDOWS_UWP
                                    <Object>
#endif
            completed = null)
        {
            if (milliseconds < 0)
            {
                control.BeginAnimation(SolidColorBrush.ColorProperty,
#if WINDOWS_UWP
                    "Color",
#endif
                    null);
            }
            else
            {
                control.BeginAnimation(SolidColorBrush.ColorProperty,
#if WINDOWS_UWP
                    "Color",
#endif
                    null);

                if (from == to || milliseconds == 0)
                {
                    control.SetValue(GradientStop.ColorProperty, to);
                }
                else
                {
                    var da = new ColorAnimation()
                    {
                        From = from,
                        To = to,
                        Duration = new Duration(TimeSpan.FromMilliseconds(milliseconds)),
                        EasingFunction = type
                    };
                    da.AutoReverse = autoreverse;
                    if (bForever)
                        da.RepeatBehavior = RepeatBehavior.Forever;
                    if (completed != null)
                        da.Completed += completed;

                    control.BeginAnimation(SolidColorBrush.ColorProperty,
#if WINDOWS_UWP
                    "Color",
#endif
                    da);
                }
            }
        }

        static Color ChangeColorHue(Color from, Color to)
        {
            var cfrom = RGBHSL.ColorToHSLConverter.Convert(System.Drawing.Color.FromArgb(from.A, from.R, from.G, from.B));
            var cto = RGBHSL.ColorToHSLConverter.Convert(System.Drawing.Color.FromArgb(to.A, to.R, to.G, to.B));
            cto.L = cfrom.L;
            // cto.S = /*cfrom.S*/1;
            var newColor = RGBHSL.HSLToColorConverter.Convert(cto);
            var ret = Color.FromRgb(newColor.R, newColor.G, newColor.B);
            ret.A = newColor.A;
            return ret;
        }

        public static void GradientBrushColorAnimation(this GradientBrush control, Color from, Color to, int milliseconds, bool bForever,
                                  bool autoreverse, EasingFunctionBase type, EventHandler
#if WINDOWS_UWP
                                    <Object>
#endif
            completed = null)
        {
            if (control.GradientStops.Count == 0)
                return;

            // var gradientStop = control.GradientStops.Last();

            foreach(var g in control.GradientStops)
            {
                if (milliseconds < 0)
                {
                    g.BeginAnimation(GradientStop.ColorProperty,
#if WINDOWS_UWP
                    "Color",
#endif
                    null);
                }
                else
                {
                    var toC = ChangeColorHue(g.Color, to);
                    var fromC = ChangeColorHue(g.Color, from);

                    g.BeginAnimation(GradientStop.ColorProperty,
#if WINDOWS_UWP
                    "Color",
#endif
                    null);

                    if (toC == fromC)
                    {
                        g.SetValue(GradientStop.ColorProperty, toC);
                    }
                    else
                    {
                        if(milliseconds == 0)
                        {
                            g.Color = toC;
                        }
                        var da = new ColorAnimation()
                        {
                            From = fromC,
                            To = toC,
                            Duration = new Duration(TimeSpan.FromMilliseconds(milliseconds)),
                            EasingFunction = type
                        };
                        da.AutoReverse = autoreverse;
                        if (bForever)
                            da.RepeatBehavior = RepeatBehavior.Forever;
                        if (completed != null)
                            da.Completed += completed;

                        g.BeginAnimation(GradientStop.ColorProperty,
#if WINDOWS_UWP
                    "Color",
#endif
                    da);
                    }
                }
            }
        }

        public static void StrokeDashOffsetAnimation(this Shape control, double to, int milliseconds, bool bForever,
                                  bool autoreverse, EasingFunctionBase type, EventHandler
#if WINDOWS_UWP
                                    <Object>
#endif
            completed = null)
        {
            if (milliseconds < 0)
            {
                control.BeginAnimation(Shape.StrokeDashOffsetProperty,
#if WINDOWS_UWP
                    "StrokeDashOffset",
#endif
                    null);
            }
            else
            {
                if (Double.IsNaN(to))
                    return;

                control.BeginAnimation(Shape.StrokeDashOffsetProperty,
#if WINDOWS_UWP
                    "StrokeDashOffset",
#endif
                    null);
                if (milliseconds == 0)
                {
                    control.StrokeDashOffset = to;
                }
                else
                {


                    var da = new DoubleAnimation()
                    {
                        To = to,
                        Duration = new Duration(TimeSpan.FromMilliseconds(milliseconds)),
                        EasingFunction = type
                    };
                    da.AutoReverse = autoreverse;
                    da.IsCumulative = !autoreverse;
                    if (bForever)
                        da.RepeatBehavior = RepeatBehavior.Forever;
                    if (completed != null)
                        da.Completed += completed;

                    control.BeginAnimation(Shape.StrokeDashOffsetProperty,
#if WINDOWS_UWP
                    "StrokeDashOffset",
#endif
                        da);
                }
            }
        }

        public static void ScaleFade(this UIElement control, double targetScale, double targetOpacity,
                                    int milliseconds, EasingFunctionBase type, bool bCenter = false, EventHandler
#if WINDOWS_UWP
                                    <Object>
#endif
            completed = null)
        {
            if (control == null)
                throw new ArgumentNullException("control", "control is null.");

            if (milliseconds < 0)
            {
                ScaleTransform t = SetTransform<ScaleTransform>(control, true);
#if WINDOWS_UWP
                int nTransformIndex = GetTransformIndex<ScaleTransform>(control);
                if (nTransformIndex == -1)
                    throw new ArgumentNullException("nTransformIndex");

                control.BeginAnimation(ScaleTransform.ScaleXProperty, String.Format("(UIElement.RenderTransform).(TransformGroup.Children)[{0}].(ScaleTransform.ScaleX)", nTransformIndex), null);
                control.BeginAnimation(ScaleTransform.ScaleYProperty, String.Format("(UIElement.RenderTransform).(TransformGroup.Children)[{0}].(ScaleTransform.ScaleY)", nTransformIndex), null);
                control.BeginAnimation(UIElement.OpacityProperty, "Opacity", null);
#else
                t.BeginAnimation(ScaleTransform.ScaleXProperty, null);
                t.BeginAnimation(ScaleTransform.ScaleYProperty, null);
                control.BeginAnimation(UIElement.OpacityProperty, null);
#endif
                RemoveTransform(control, t);
            }
            else
            {
                if (Double.IsNaN(targetScale) || Double.IsNaN(targetOpacity))
                    return;

                ScaleTransform t = SetTransform<ScaleTransform>(control, true);

                int nTransformIndex = GetTransformIndex<ScaleTransform>(control);
                if (nTransformIndex == -1)
                    throw new ArgumentNullException("nTransformIndex");
#if WINDOWS_UWP
                control.BeginAnimation(ScaleTransform.ScaleXProperty, String.Format("(UIElement.RenderTransform).(TransformGroup.Children)[{0}].(ScaleTransform.ScaleX)", nTransformIndex), null);
                control.BeginAnimation(ScaleTransform.ScaleYProperty, String.Format("(UIElement.RenderTransform).(TransformGroup.Children)[{0}].(ScaleTransform.ScaleY)", nTransformIndex), null);
                control.BeginAnimation(UIElement.OpacityProperty, "Opacity", null);
#else
                t.BeginAnimation(ScaleTransform.ScaleXProperty, null);
                t.BeginAnimation(ScaleTransform.ScaleYProperty, null);
                control.BeginAnimation(UIElement.OpacityProperty, null);
#endif

                if (bCenter)
                {
                    t.CenterX = (control as FrameworkElement).ActualWidth / 2;
                    t.CenterY = (control as FrameworkElement).ActualHeight / 2;
                }
                if(milliseconds == 0)
                {
                    t.ScaleX = targetScale;
                    t.ScaleY = targetScale;
                }
                Storyboard sb = new Storyboard();
                if (completed != null)
                    sb.Completed += completed;

                DoubleAnimation dax = new DoubleAnimation()
                {
                    To = targetScale,
                    Duration = new Duration(TimeSpan.FromMilliseconds(milliseconds)),
                    EasingFunction = type
                };

                sb.Children.Add(dax);
                Storyboard.SetTargetProperty(dax,
#if WINDOWS_UWP
                    String.Format("(UIElement.RenderTransform).(TransformGroup.Children)[{0}].(ScaleTransform.ScaleX)", nTransformIndex));
#else
                    new PropertyPath(String.Format("(UIElement.RenderTransform).(TransformGroup.Children)[{0}].(ScaleTransform.ScaleX)", nTransformIndex)));
#endif
                Storyboard.SetTarget(dax, control);

                DoubleAnimation day = new DoubleAnimation()
                {
                    To = targetScale,
                    Duration = new Duration(TimeSpan.FromMilliseconds(milliseconds)),
                    EasingFunction = type
                };

                sb.Children.Add(day);
                Storyboard.SetTargetProperty(day,
#if WINDOWS_UWP
                    String.Format("(UIElement.RenderTransform).(TransformGroup.Children)[{0}].(ScaleTransform.ScaleY)", nTransformIndex));
#else
                    new PropertyPath(String.Format("(UIElement.RenderTransform).(TransformGroup.Children)[{0}].(ScaleTransform.ScaleY)", nTransformIndex)));
#endif
                Storyboard.SetTarget(day, control);

                DoubleAnimation da = new DoubleAnimation()
                {
                    To = targetOpacity,
                    Duration = new Duration(TimeSpan.FromMilliseconds(milliseconds)),
                    EasingFunction = type
                };
                sb.Children.Add(da);
                Storyboard.SetTargetProperty(da,
#if WINDOWS_UWP
                    "(UIElement.Opacity)");
#else
                    new PropertyPath("(UIElement.Opacity)"));
#endif
                Storyboard.SetTarget(da, control);

                sb.Begin();
#if WINDOWS_UWP
                control.AddStoryBoard(sb, ScaleTransform.ScaleXProperty);
                control.AddStoryBoard(sb, ScaleTransform.ScaleYProperty);
                control.AddStoryBoard(sb, UIElement.OpacityProperty);
#endif
            }
        }

        public static void Rotate(this UIElement control, double targetAngle, int milliseconds, bool bForever,
                                  bool autoreverse, EasingFunctionBase type, bool bCenter = true, EventHandler
#if WINDOWS_UWP
                                    <Object>
#endif
            completed = null)
        {
            if (control == null)
                throw new ArgumentNullException("control", "control is null.");

            if (milliseconds < 0)
            {
                RotateTransform t = SetTransform<RotateTransform>(control, true);
                t.BeginAnimation(RotateTransform.AngleProperty,
#if WINDOWS_UWP
                    "Angle",
#endif
                    null);
                RemoveTransform(control, t);
            }
            else
            {
                if (Double.IsNaN(targetAngle))
                    return;

                RotateTransform t = SetTransform<RotateTransform>(control, true);
                //int nTransformIndex = GetTransformIndex<RotateTransform>(control);

                //if (nTransformIndex == -1)
                //    throw new ArgumentNullException("nTransformIndex");
                //t.BeginAnimation(RotateTransform.AngleProperty, null);

                if (bCenter)
                {
                    t.CenterX = (control as FrameworkElement).ActualWidth / 2;
                    t.CenterY = (control as FrameworkElement).ActualHeight / 2;
                }

                if (milliseconds == 0)
                {
                    t.Angle = targetAngle;
                }
                else
                {
                    DoubleAnimation da = new DoubleAnimation()
                    {
                        To = targetAngle,
                        Duration = new Duration(TimeSpan.FromMilliseconds(milliseconds)),
                        EasingFunction = type
                    };
                    da.AutoReverse = autoreverse;
                    if (bForever)
                        da.RepeatBehavior = RepeatBehavior.Forever;
                    if (completed != null)
                        da.Completed += completed;
                    t.BeginAnimation(RotateTransform.AngleProperty,
#if WINDOWS_UWP
                        "Angle",
#endif
                        da);
                }
            }
        }

        public static void Blink(this UIElement control, int milliseconds,
                                 double from, double to, EasingFunctionBase type,
                                 EventHandler
#if WINDOWS_UWP
                                    <Object>
#endif
            completed = null, int nRepeatCount = 0)
        {
            if (control == null)
                throw new ArgumentNullException("control", "control is null.");

            if (milliseconds < 0)
                control.BeginAnimation(UIElement.OpacityProperty,
#if WINDOWS_UWP
                    "Opacity",
#endif
                    null);
            else
            {
                if (Double.IsNaN(from) || Double.IsNaN(to))
                    return;

                control.BeginAnimation(UIElement.OpacityProperty,
#if WINDOWS_UWP
                    "Opacity",
#endif
                    null);
                if (milliseconds == 0)
                {
                    control.Opacity = to;
                }
                else
                {
                    DoubleAnimation da = new DoubleAnimation
                    {
                        EasingFunction = type,
                        Duration = new Duration(TimeSpan.FromMilliseconds(milliseconds)),
                        AutoReverse = true,
                        From = Double.IsNaN(from) ? 1.0 : from,
                        To = Double.IsNaN(to) ? 0.1 : to,
                        RepeatBehavior = nRepeatCount == 0 ? RepeatBehavior.Forever : new RepeatBehavior(nRepeatCount)
                    };
                    if (completed != null)
                        da.Completed += completed;
                    control.BeginAnimation(UIElement.OpacityProperty,
#if WINDOWS_UWP
                    "Opacity",
#endif
                        da);
                }
            }
        }

        public static void BlinkScale(this UIElement control, double targetScale, int milliseconds,
                                        EasingFunctionBase type, bool bCenter = false, 
                                        EventHandler
#if WINDOWS_UWP
                                    <Object>
#endif
            completed = null)
        {
            if (control == null)
                throw new ArgumentNullException("control", "control is null.");

            if (milliseconds < 0)
            {
                ScaleTransform t = SetTransform<ScaleTransform>(control, true);
                int nTransformIndex = GetTransformIndex<ScaleTransform>(control);
                if (nTransformIndex == -1)
                    throw new ArgumentNullException("nTransformIndex");
#if WINDOWS_UWP
                control.BeginAnimation(ScaleTransform.ScaleXProperty, String.Format("(UIElement.RenderTransform).(TransformGroup.Children)[{0}].(ScaleTransform.ScaleX)", nTransformIndex), null);
                control.BeginAnimation(ScaleTransform.ScaleYProperty, String.Format("(UIElement.RenderTransform).(TransformGroup.Children)[{0}].(ScaleTransform.ScaleY)", nTransformIndex), null);
#else
                t.BeginAnimation(ScaleTransform.ScaleXProperty, null);
                t.BeginAnimation(ScaleTransform.ScaleYProperty, null);
#endif
                RemoveTransform(control, t);
            }
            else
            {
                if (Double.IsNaN(targetScale))
                    return;

                ScaleTransform t = SetTransform<ScaleTransform>(control, true);
                int nTransformIndex = GetTransformIndex<ScaleTransform>(control);
                if (nTransformIndex == -1)
                    throw new ArgumentNullException("nTransformIndex");
#if WINDOWS_UWP
                control.BeginAnimation(ScaleTransform.ScaleXProperty, String.Format("(UIElement.RenderTransform).(TransformGroup.Children)[{0}].(ScaleTransform.ScaleX)", nTransformIndex), null);
                control.BeginAnimation(ScaleTransform.ScaleYProperty, String.Format("(UIElement.RenderTransform).(TransformGroup.Children)[{0}].(ScaleTransform.ScaleY)", nTransformIndex), null);
#else
                t.BeginAnimation(ScaleTransform.ScaleXProperty, null);
                t.BeginAnimation(ScaleTransform.ScaleYProperty, null);
#endif

                if (bCenter)
                {
                    t.CenterX = (control as FrameworkElement).ActualWidth / 2;
                    t.CenterY = (control as FrameworkElement).ActualHeight / 2;
                }
                if (milliseconds == 0)
                {
                    t.ScaleX = targetScale;
                    t.ScaleY = targetScale;
                }
                else
                {
                    Storyboard sb = new Storyboard();
                    if (completed != null)
                        sb.Completed += completed;

                    DoubleAnimation dax = new DoubleAnimation()
                    {
                        To = targetScale,
                        Duration = new Duration(TimeSpan.FromMilliseconds(milliseconds)),
                        EasingFunction = type,
                        AutoReverse = true,
                        RepeatBehavior = RepeatBehavior.Forever
                    };
                    sb.Children.Add(dax);
                    Storyboard.SetTargetProperty(dax,
#if WINDOWS_UWP
                    String.Format("(UIElement.RenderTransform).(TransformGroup.Children)[{0}].(ScaleTransform.ScaleX)", nTransformIndex));
#else
                        new PropertyPath(String.Format("(UIElement.RenderTransform).(TransformGroup.Children)[{0}].(ScaleTransform.ScaleX)", nTransformIndex)));
#endif
                    Storyboard.SetTarget(dax, control);

                    DoubleAnimation day = new DoubleAnimation()
                    {
                        To = targetScale,
                        Duration = new Duration(TimeSpan.FromMilliseconds(milliseconds)),
                        EasingFunction = type,
                        AutoReverse = true,
                        RepeatBehavior = RepeatBehavior.Forever
                    };
                    sb.Children.Add(day);
                    Storyboard.SetTargetProperty(day,
#if WINDOWS_UWP
                    String.Format("(UIElement.RenderTransform).(TransformGroup.Children)[{0}].(ScaleTransform.ScaleY)", nTransformIndex));
#else
                        new PropertyPath(String.Format("(UIElement.RenderTransform).(TransformGroup.Children)[{0}].(ScaleTransform.ScaleY)", nTransformIndex)));
#endif
                    Storyboard.SetTarget(day, control);

                    sb.Begin();

#if WINDOWS_UWP
                control.AddStoryBoard(sb, ScaleTransform.ScaleXProperty);
                control.AddStoryBoard(sb, ScaleTransform.ScaleYProperty);
#endif
                }
            }
        }

        public static void TranslateX(this UIElement control, double targetX, int milliseconds, bool bForever,
                                        bool autoreverse, EasingFunctionBase type, EventHandler
#if WINDOWS_UWP
                                    <Object>
#endif
            completed = null)
        {
            if (control == null)
                throw new ArgumentNullException("control", "control is null.");

            if (milliseconds < 0)
            {
                TranslateTransform t = SetTransform<TranslateTransform>(control, true);
                t.BeginAnimation(TranslateTransform.XProperty,
#if WINDOWS_UWP
                    "X",
#endif
                    null);
                RemoveTransform(control, t);
            }
            else
            {
                if (Double.IsNaN(targetX))
                    return;
                
                TranslateTransform t = SetTransform<TranslateTransform>(control, true);
                //t.BeginAnimation(TranslateTransform.XProperty, null);
                if (milliseconds == 0)
                {
                    t.X = targetX;
                }
                else
                {
                    DoubleAnimation da = new DoubleAnimation()
                    {
                        To = targetX,
                        Duration = new Duration(TimeSpan.FromMilliseconds(milliseconds)),
                        EasingFunction = type
                    };

                    da.AutoReverse = autoreverse;
                    if (bForever)
                        da.RepeatBehavior = RepeatBehavior.Forever;
                    if (completed != null)
                        da.Completed += completed;
                    t.BeginAnimation(TranslateTransform.XProperty,
#if WINDOWS_UWP
                    "X",
#endif
                        da);
                }
            }
        }

        public static void TranslateY(this UIElement control, double targetY, int milliseconds, bool bForever,
                                        bool autoreverse, EasingFunctionBase type, EventHandler
#if WINDOWS_UWP
                                    <Object>
#endif
            completed = null)
        {
            if (control == null)
                throw new ArgumentNullException("control", "control is null.");

            if (milliseconds < 0)
            {
                TranslateTransform t = SetTransform<TranslateTransform>(control, true);
                t.BeginAnimation(TranslateTransform.YProperty,
#if WINDOWS_UWP
                    "Y",
#endif
                    null);
                RemoveTransform(control, t);
            }
            else
            {
                if (Double.IsNaN(targetY))
                    return;

                TranslateTransform t = SetTransform<TranslateTransform>(control, true);
                //t.BeginAnimation(TranslateTransform.YProperty, null);
                if (milliseconds == 0)
                {
                    t.Y = targetY;
                }
                else
                {
                    DoubleAnimation da = new DoubleAnimation()
                    {
                        To = targetY,
                        Duration = new Duration(TimeSpan.FromMilliseconds(milliseconds)),
                        EasingFunction = type
                    };
                    da.AutoReverse = autoreverse;
                    if (bForever)
                        da.RepeatBehavior = RepeatBehavior.Forever;
                    if (completed != null)
                        da.Completed += completed;
                    t.BeginAnimation(TranslateTransform.YProperty,
#if WINDOWS_UWP
                    "Y",
#endif
                        da);
                }
            }
        }

        public static void TranslateXY(this UIElement control, double targetX, double targetY, int milliseconds,
                                        bool bForever, bool autoreverse, EasingFunctionBase type, EventHandler
#if WINDOWS_UWP
                                    <Object>
#endif
            completed = null)
        {
            if (control == null)
                throw new ArgumentNullException("control", "control is null.");

            if (milliseconds < 0)
            {
                TranslateTransform t = SetTransform<TranslateTransform>(control, true);
#if WINDOWS_UWP
                int nTransformIndex = GetTransformIndex<TranslateTransform>(control);
                if (nTransformIndex == -1)
                    throw new ArgumentNullException("nTransformIndex");
                control.BeginAnimation(TranslateTransform.XProperty, String.Format("(UIElement.RenderTransform).(TransformGroup.Children)[{0}].(TranslateTransform.X)", nTransformIndex), null);
                control.BeginAnimation(TranslateTransform.YProperty, String.Format("(UIElement.RenderTransform).(TransformGroup.Children)[{0}].(TranslateTransform.Y)", nTransformIndex), null);
#else
                t.BeginAnimation(TranslateTransform.XProperty, null);
                t.BeginAnimation(TranslateTransform.YProperty, null);
#endif
                RemoveTransform(control, t);
            }
            else
            {
                if (Double.IsNaN(targetY) || Double.IsNaN(targetX))
                    return;

                TranslateTransform t = SetTransform<TranslateTransform>(control, true);

                int nTransformIndex = GetTransformIndex<TranslateTransform>(control);
                if (nTransformIndex == -1)
                    throw new ArgumentNullException("nTransformIndex");
                //#if WINDOWS_UWP
                //                control.BeginAnimation(TranslateTransform.XProperty, null);
                //                control.BeginAnimation(TranslateTransform.YProperty, null);
                //#else
                //                t.BeginAnimation(TranslateTransform.XProperty, null);
                //                t.BeginAnimation(TranslateTransform.YProperty, null);
                //#endif
                if (milliseconds == 0)
                {
                    t.X = targetX;
                    t.Y = targetY;
                }
                else
                {
                    Storyboard sb = new Storyboard();
                    if (completed != null)
                        sb.Completed += completed;

                    DoubleAnimation dax = new DoubleAnimation()
                    {
                        To = targetX,
                        Duration = new Duration(TimeSpan.FromMilliseconds(milliseconds)),
                        EasingFunction = type
                    };
                    dax.AutoReverse = autoreverse;
                    if (bForever)
                        dax.RepeatBehavior = RepeatBehavior.Forever;
                    sb.Children.Add(dax);
                    Storyboard.SetTargetProperty(dax,
#if WINDOWS_UWP
                    String.Format("(UIElement.RenderTransform).(TransformGroup.Children)[{0}].(TranslateTransform.X)", nTransformIndex));
#else
                        new PropertyPath(String.Format("(UIElement.RenderTransform).(TransformGroup.Children)[{0}].(TranslateTransform.X)", nTransformIndex)));
#endif
                    Storyboard.SetTarget(dax, control);

                    DoubleAnimation day = new DoubleAnimation()
                    {
                        To = targetY,
                        Duration = new Duration(TimeSpan.FromMilliseconds(milliseconds)),
                        EasingFunction = type
                    };
                    day.AutoReverse = autoreverse;
                    if (bForever)
                        day.RepeatBehavior = RepeatBehavior.Forever;
                    sb.Children.Add(day);
                    Storyboard.SetTargetProperty(day,
#if WINDOWS_UWP
                    String.Format("(UIElement.RenderTransform).(TransformGroup.Children)[{0}].(TranslateTransform.Y)", nTransformIndex));
#else
                        new PropertyPath(String.Format("(UIElement.RenderTransform).(TransformGroup.Children)[{0}].(TranslateTransform.Y)", nTransformIndex)));
#endif
                    Storyboard.SetTarget(day, control);

                    sb.Begin();
#if WINDOWS_UWP
                control.AddStoryBoard(sb, TranslateTransform.XProperty);
                control.AddStoryBoard(sb, TranslateTransform.YProperty);
#endif
                }
            }
        }

        public static void TranslateXY(this UIElement control, Point[] ptPath, int milliseconds,
                                        bool bForever, bool autoreverse, bool bSpline, EventHandler
#if WINDOWS_UWP
                                    <Object>
#endif
            completed = null)
        {
            if (control == null)
                throw new ArgumentNullException("control", "control is null.");

            if (milliseconds < 0)
            {
                TranslateTransform t = SetTransform<TranslateTransform>(control, true);
#if WINDOWS_UWP
                int nTransformIndex = GetTransformIndex<TranslateTransform>(control);
                if (nTransformIndex == -1)
                    throw new ArgumentNullException("nTransformIndex");
                control.BeginAnimation(TranslateTransform.XProperty, String.Format("(UIElement.RenderTransform).(TransformGroup.Children)[{0}].(TranslateTransform.X)", nTransformIndex), null);
                control.BeginAnimation(TranslateTransform.YProperty, String.Format("(UIElement.RenderTransform).(TransformGroup.Children)[{0}].(TranslateTransform.Y)", nTransformIndex), null);
#else
                t.BeginAnimation(TranslateTransform.XProperty, null);
                t.BeginAnimation(TranslateTransform.YProperty, null);
#endif
                RemoveTransform(control, t);
            }
            else
            {
                if (ptPath == null)
                    return;
                foreach (var pt in ptPath)
                {
                    if (Double.IsNaN(pt.X) || Double.IsNaN(pt.Y))
                        return;
                }

                TranslateTransform t = SetTransform<TranslateTransform>(control, true);

                int nTransformIndex = GetTransformIndex<TranslateTransform>(control);
                if (nTransformIndex == -1)
                    throw new ArgumentNullException("nTransformIndex");
//#if WINDOWS_UWP
//                control.BeginAnimation(TranslateTransform.XProperty, null);
//                control.BeginAnimation(TranslateTransform.YProperty, null);
//#else
//                t.BeginAnimation(TranslateTransform.XProperty, null);
//                t.BeginAnimation(TranslateTransform.YProperty, null);
//#endif

                Storyboard sb = new Storyboard();
                if (completed != null)
                    sb.Completed += completed;

                DoubleAnimationUsingKeyFrames dakfx = new DoubleAnimationUsingKeyFrames
                {
                    Duration = new Duration(TimeSpan.FromMilliseconds(milliseconds))
                };
                int nCount = 0;
                foreach(var pt in ptPath)
                {
#if !WINDOWS_UWP
                    if (bSpline)
                        dakfx.KeyFrames.Add(new SplineDoubleKeyFrame(pt.X, KeyTime.FromPercent(++nCount / ptPath.Length * 100)));
                    else
                        dakfx.KeyFrames.Add(new LinearDoubleKeyFrame(pt.X, KeyTime.FromPercent(++nCount / ptPath.Length * 100)));
#else
                    if (bSpline)
                        dakfx.KeyFrames.Add(new SplineDoubleKeyFrame() 
                            {  
                                Value = pt.X, 
                                KeyTime = KeyTime.FromTimeSpan(TimeSpan.FromMilliseconds(milliseconds / ptPath.Length * ++nCount))
                            });
                    else
                        dakfx.KeyFrames.Add(new LinearDoubleKeyFrame()
                            {  
                                Value = pt.X, 
                                KeyTime = KeyTime.FromTimeSpan(TimeSpan.FromMilliseconds(milliseconds / ptPath.Length * ++nCount))
                            });
#endif
                }

                dakfx.AutoReverse = autoreverse;
                if (bForever)
                    dakfx.RepeatBehavior = RepeatBehavior.Forever;
                sb.Children.Add(dakfx);
                if (milliseconds == 0)
                {
                    if (ptPath != null && ptPath.Length > 0)
                    {
                        var lastindex = ptPath.Length;
                        t.X = ptPath[lastindex].X;
                        t.Y = ptPath[lastindex].Y;
                    }
                }
                else
                {


                    Storyboard.SetTargetProperty(dakfx,
#if WINDOWS_UWP
                    String.Format("(UIElement.RenderTransform).(TransformGroup.Children)[{0}].(TranslateTransform.X)", nTransformIndex));
#else
                        new PropertyPath(String.Format("(UIElement.RenderTransform).(TransformGroup.Children)[{0}].(TranslateTransform.X)", nTransformIndex)));
#endif
                    Storyboard.SetTarget(dakfx, control);

                    DoubleAnimationUsingKeyFrames dakfy = new DoubleAnimationUsingKeyFrames
                    {
                        Duration = new Duration(TimeSpan.FromMilliseconds(milliseconds))
                    };
                    nCount = 0;
                    foreach (var pt in ptPath)
                    {
#if !WINDOWS_UWP
                        if (bSpline)
                            dakfy.KeyFrames.Add(new SplineDoubleKeyFrame(pt.X, KeyTime.FromPercent(++nCount / ptPath.Length * 100)));
                        else
                            dakfy.KeyFrames.Add(new LinearDoubleKeyFrame(pt.X, KeyTime.FromPercent(++nCount / ptPath.Length * 100)));
#else
                    if (bSpline)
                        dakfy.KeyFrames.Add(new SplineDoubleKeyFrame() 
                            {  
                                Value = pt.Y, 
                                KeyTime = KeyTime.FromTimeSpan(TimeSpan.FromMilliseconds(milliseconds / ptPath.Length * ++nCount))
                            });
                    else
                        dakfy.KeyFrames.Add(new LinearDoubleKeyFrame()
                            {  
                                Value = pt.Y, 
                                KeyTime = KeyTime.FromTimeSpan(TimeSpan.FromMilliseconds(milliseconds / ptPath.Length * ++nCount))
                            });
#endif
                    }

                    dakfy.AutoReverse = autoreverse;
                    if (bForever)
                        dakfy.RepeatBehavior = RepeatBehavior.Forever;
                    sb.Children.Add(dakfy);
                    Storyboard.SetTargetProperty(dakfy,
#if WINDOWS_UWP
                    String.Format("(UIElement.RenderTransform).(TransformGroup.Children)[{0}].(TranslateTransform.Y)", nTransformIndex));
#else
                        new PropertyPath(String.Format("(UIElement.RenderTransform).(TransformGroup.Children)[{0}].(TranslateTransform.Y)", nTransformIndex)));
#endif
                    Storyboard.SetTarget(dakfy, control);

                    sb.Begin();
#if WINDOWS_UWP
                control.AddStoryBoard(sb, TranslateTransform.XProperty);
                control.AddStoryBoard(sb, TranslateTransform.YProperty);
#endif
                }
            }
        }

        public static readonly DependencyProperty LeftProperty = DependencyProperty.RegisterAttached("Left",
                         typeof(double), typeof(Animations),
#if WINDOWS_UWP
                         new PropertyMetadata(0.0));
#else
                         new UIPropertyMetadata(0.0));
#endif

        public static readonly DependencyProperty WidthProperty = DependencyProperty.RegisterAttached("WidthStored",
                         typeof(double), typeof(Animations),
#if WINDOWS_UWP
                         new PropertyMetadata(0.0));
#else
                         new UIPropertyMetadata(0.0));
#endif

        public static void WidthAnimation(this UIElement control, double targetWidth, int milliseconds, bool bForever,
                                        bool autoreverse, EasingFunctionBase type, EventHandler
#if WINDOWS_UWP
                                    <Object>
#endif
            completed = null, bool bAdaptPosition = true)
        {
            if (control == null)
                throw new ArgumentNullException("control", "control is null.");

            var fe = control as FrameworkElement;
            // fe.Width = fe.ActualWidth;

            if (targetWidth <= 0)
                targetWidth = 1;
            double dLeft = 0;
            if (targetWidth < 0)
            {
                targetWidth = -targetWidth;
                double dValue = 0;
                var actLeft = fe.ReadLocalValue(LeftProperty);
                if (actLeft == DependencyProperty.UnsetValue)
                {
                    dValue = Canvas.GetLeft(fe);
                    fe.SetValue(LeftProperty, dValue);
                }
                else
                    dValue = (double)actLeft;

                if (Double.IsNaN(dValue))
                    return;

                dLeft = dValue - targetWidth;
            }
            else if (bAdaptPosition)
                control.BeginAnimation(Canvas.LeftProperty,
#if WINDOWS_UWP
                    "Left",
#endif
                    null);

            if (milliseconds < 0)
            {
                control.BeginAnimation(FrameworkElement.WidthProperty,
#if WINDOWS_UWP
                    "Width",
#endif
                    null);
                if (bAdaptPosition)
                    control.BeginAnimation(Canvas.LeftProperty,
#if WINDOWS_UWP
                        "Left",
#endif
                        null);

                var actWidth = fe.ReadLocalValue(WidthProperty);
                if (actWidth != DependencyProperty.UnsetValue)
                {
                    fe.Width = (double)actWidth;
                    fe.ClearValue(WidthProperty);
                }
            }
            else
            {
                if (Double.IsNaN(targetWidth))
                    return;

                //control.BeginAnimation(FrameworkElement.WidthProperty, null);
                //control.BeginAnimation(Canvas.LeftProperty, null);

                var actWidth = fe.ReadLocalValue(WidthProperty);
                if (actWidth == DependencyProperty.UnsetValue)
                {
                    fe.SetValue(WidthProperty, fe.Width);
                }

                if (milliseconds == 0)
                {
                    fe.Width = targetWidth;
                }
                else
                {
                    DoubleAnimation da = new DoubleAnimation()
                    {
                        To = targetWidth,
                        Duration = new Duration(TimeSpan.FromMilliseconds(milliseconds)),
                        EasingFunction = type
                    };
                    da.AutoReverse = autoreverse;
                    if (bForever)
                        da.RepeatBehavior = RepeatBehavior.Forever;
                    if (completed != null)
                        da.Completed += completed;
                    control.BeginAnimation(FrameworkElement.WidthProperty,
#if WINDOWS_UWP
                        "Width",
#endif
                        da);
                }

                if (bAdaptPosition && dLeft != 0)
                {
                    if (milliseconds == 0)
                    {
                        Canvas.SetLeft(control, dLeft);
                    }
                    else
                    {
                        DoubleAnimation daLeft = new DoubleAnimation()
                        {
                            To = dLeft,
                            Duration = new Duration(TimeSpan.FromMilliseconds(milliseconds)),
                            EasingFunction = type
                        };
                        daLeft.AutoReverse = autoreverse;
                        if (bForever)
                            daLeft.RepeatBehavior = RepeatBehavior.Forever;
                        if (completed != null)
                            daLeft.Completed += completed;
                        control.BeginAnimation(Canvas.LeftProperty,
#if WINDOWS_UWP
                            "Left",
#endif
                            daLeft);
                    }
                }
            }
        }

        public static void XAnimation(this UIElement control, double targetX, int milliseconds, bool bForever,
                                        bool autoreverse, EasingFunctionBase type, EventHandler
#if WINDOWS_UWP
                                    <Object>
#endif
            completed = null)
        {
            if (control == null)
                throw new ArgumentNullException("control", "control is null.");

            if (milliseconds < 0)
            {
                //if (control.FindParent<InkCanvas>() != null)
                //    control.BeginAnimation(InkCanvas.LeftProperty, null);
                //else
                control.BeginAnimation(Canvas.LeftProperty,
#if WINDOWS_UWP
                        "Left",
#endif
                        null);
            }
            else
            {
                if (Double.IsNaN(targetX))
                    return;

                //if (control.FindParent<InkCanvas>() != null)
                //    control.BeginAnimation(InkCanvas.LeftProperty, null);
                //else
                //    control.BeginAnimation(Canvas.LeftProperty, null);

                var fe = control as FrameworkElement;
                double dValue = 0;
                var actLeft = fe.ReadLocalValue(LeftProperty);
                if (actLeft == DependencyProperty.UnsetValue)
                {
                    //if (control.FindParent<InkCanvas>() != null)
                    //    dValue = InkCanvas.GetLeft(fe);
                    //else
                    dValue = Canvas.GetLeft(fe);
                    fe.SetValue(LeftProperty, dValue);
                }
                else
                    dValue = (double)actLeft;

                if (Double.IsNaN(dValue))
                    return;

                if (milliseconds == 0)
                {
                    Canvas.SetLeft(fe, dValue + targetX);
                }
                else
                {
                    DoubleAnimation daLeft = new DoubleAnimation()
                    {
                        To = dValue + targetX,
                        Duration = new Duration(TimeSpan.FromMilliseconds(milliseconds)),
                        EasingFunction = type
                    };
                    daLeft.AutoReverse = autoreverse;
                    if (bForever)
                        daLeft.RepeatBehavior = RepeatBehavior.Forever;
                    if (completed != null)
                        daLeft.Completed += completed;
                    //if (control.FindParent<InkCanvas>() != null)
                    //    control.BeginAnimation(InkCanvas.LeftProperty, daLeft);
                    //else
                    control.BeginAnimation(Canvas.LeftProperty,
#if WINDOWS_UWP
                        "Left",
#endif
                        daLeft);
                }
            }
        }

        public static readonly DependencyProperty TopProperty = DependencyProperty.RegisterAttached("Top",
                         typeof(double), typeof(Animations),
#if WINDOWS_UWP
                         new PropertyMetadata(0.0));
#else
                         new UIPropertyMetadata(0.0));
#endif

        public static readonly DependencyProperty HeightProperty = DependencyProperty.RegisterAttached("HeightStored",
                         typeof(double), typeof(Animations),
#if WINDOWS_UWP
                         new PropertyMetadata(0.0));
#else
                         new UIPropertyMetadata(0.0));
#endif

        public static void HeightAnimation(this UIElement control, double targetHeight, int milliseconds, bool bForever,
                                        bool autoreverse, EasingFunctionBase type, EventHandler
#if WINDOWS_UWP
                                    <Object>
#endif
            completed = null, bool bAdaptPosition = true)
        {
            if (control == null)
                throw new ArgumentNullException("control", "control is null.");

            var fe = control as FrameworkElement;
            // fe.Height = fe.ActualHeight;

            if (targetHeight <= 0)
                targetHeight = 1;

            double dTop = 0;
            if (targetHeight < 0)
            {
                targetHeight = -targetHeight;
                double dValue = 0;
                var actTop = fe.ReadLocalValue(TopProperty);
                if (actTop == DependencyProperty.UnsetValue)
                {
                    dValue = Canvas.GetTop(fe);
                    fe.SetValue(TopProperty, dValue);
                }
                else
                    dValue = (double)actTop;

                if (Double.IsNaN(dValue))
                    return;

                dTop = dValue - targetHeight;
            }
            else if (bAdaptPosition)
                control.BeginAnimation(Canvas.TopProperty,
#if WINDOWS_UWP
                    "Top",
#endif
                    null);

            if (milliseconds < 0)
            {
                control.BeginAnimation(FrameworkElement.HeightProperty,
#if WINDOWS_UWP
                    "Height",
#endif
                    null);
                if (bAdaptPosition)
                    control.BeginAnimation(Canvas.TopProperty,
#if WINDOWS_UWP
                        "Top",
#endif
                        null);

                var actHeight = fe.ReadLocalValue(HeightProperty);
                if (actHeight != DependencyProperty.UnsetValue)
                {
                    fe.Height = (double)actHeight;
                    fe.ClearValue(HeightProperty);
                }
            }
            else
            {
                if (Double.IsNaN(targetHeight))
                    return;

                //control.BeginAnimation(FrameworkElement.HeightProperty, null);
                //control.BeginAnimation(Canvas.TopProperty, null);

                var actHeight = fe.ReadLocalValue(HeightProperty);
                if (actHeight == DependencyProperty.UnsetValue)
                {
                    fe.SetValue(HeightProperty, fe.Height);
                }

                if (milliseconds == 0)
                {
                    fe.Height = targetHeight;
                }
                else
                {
                    DoubleAnimation da = new DoubleAnimation()
                    {
                        To = targetHeight,
                        Duration = new Duration(TimeSpan.FromMilliseconds(milliseconds)),
                        EasingFunction = type
                    };
                    da.AutoReverse = autoreverse;
                    if (bForever)
                        da.RepeatBehavior = RepeatBehavior.Forever;
                    if (completed != null)
                        da.Completed += completed;
                    control.BeginAnimation(FrameworkElement.HeightProperty,
#if WINDOWS_UWP
                        "Height",
#endif
                        da);
                }

                if (bAdaptPosition && dTop != 0)
                {
                    if (milliseconds == 0)
                    {
                        Canvas.SetTop(control, dTop);
                    }
                    else
                    {
                        DoubleAnimation daTop = new DoubleAnimation()
                        {
                            To = dTop,
                            Duration = new Duration(TimeSpan.FromMilliseconds(milliseconds)),
                            EasingFunction = type
                        };
                        daTop.AutoReverse = autoreverse;
                        if (bForever)
                            daTop.RepeatBehavior = RepeatBehavior.Forever;
                        if (completed != null)
                            daTop.Completed += completed;
                        control.BeginAnimation(Canvas.TopProperty,
#if WINDOWS_UWP
                            "Top",
#endif
                            daTop);
                    }
                }
            }
        }

        public static void YAnimation(this UIElement control, double targetY, int milliseconds, bool bForever,
                                        bool autoreverse, EasingFunctionBase type, EventHandler
#if WINDOWS_UWP
                                    <Object>
#endif
            completed = null)
        {
            if (control == null)
                throw new ArgumentNullException("control", "control is null.");

            if (milliseconds < 0)
            {
                //if (control.FindParent<InkCanvas>() != null)
                //    control.BeginAnimation(InkCanvas.TopProperty, null);
                //else
                    control.BeginAnimation(Canvas.TopProperty,
#if WINDOWS_UWP
                        "Top",
#endif
                        null);
            }
            else
            {
                if (Double.IsNaN(targetY))
                    return;

                //if (control.FindParent<InkCanvas>() != null)
                //    control.BeginAnimation(InkCanvas.TopProperty, null);
                //else
                //    control.BeginAnimation(Canvas.TopProperty, null);

                var fe = control as FrameworkElement;
                double dValue = 0;
                var actTop = fe.ReadLocalValue(TopProperty);
                if (actTop == DependencyProperty.UnsetValue)
                {
                    //if (control.FindParent<InkCanvas>() != null)
                    //    dValue = InkCanvas.GetTop(fe);
                    //else
                        dValue = Canvas.GetTop(fe);
                    fe.SetValue(TopProperty, dValue);
                }
                else
                    dValue = (double)actTop;

                if (Double.IsNaN(dValue))
                    return;

                if (milliseconds == 0)
                {
                    Canvas.SetTop(control, dValue + targetY);
                }
                else
                {
                    DoubleAnimation daTop = new DoubleAnimation()
                    {
                        To = dValue + targetY,
                        Duration = new Duration(TimeSpan.FromMilliseconds(milliseconds)),
                        EasingFunction = type
                    };
                    daTop.AutoReverse = autoreverse;
                    if (bForever)
                        daTop.RepeatBehavior = RepeatBehavior.Forever;
                    if (completed != null)
                        daTop.Completed += completed;
                    //if (control.FindParent<InkCanvas>() != null)
                    //    control.BeginAnimation(InkCanvas.TopProperty, daTop);
                    //else
                    control.BeginAnimation(Canvas.TopProperty,
#if WINDOWS_UWP
                        "Top",
#endif
                        daTop);
                }
            }
        }

#if !WINDOWS_UWP
        public static void MoveXYAlongPath(this UIElement control, Point[] ptPath, int milliseconds,
                                        bool bForever, bool autoreverse, EventHandler completed = null)
        {
            if (control == null)
                throw new ArgumentNullException("control", "control is null.");

            //bool isInk = control.FindParent<InkCanvas>() != null;
            if (milliseconds < 0)
            {
                //if (isInk)
                //{
                //    control.BeginAnimation(InkCanvas.TopProperty, null);
                //    control.BeginAnimation(InkCanvas.LeftProperty, null);
                //}
                //else
                {
                    control.BeginAnimation(Canvas.TopProperty, null);
                    control.BeginAnimation(Canvas.LeftProperty, null);
                }
            }
            else
            {
                //if (isInk)
                //{
                //    control.BeginAnimation(InkCanvas.TopProperty, null);
                //    control.BeginAnimation(InkCanvas.LeftProperty, null);
                //}
                //else
                //{
                //    control.BeginAnimation(Canvas.TopProperty, null);
                //    control.BeginAnimation(Canvas.LeftProperty, null);
                //}

                if (ptPath == null || ptPath.Length <= 1 ||
                    ptPath.Length == 2 && Math.Floor(ptPath[0].X) == Math.Floor(ptPath[1].X) &&
                                          Math.Floor(ptPath[0].Y) == Math.Floor(ptPath[1].Y))
                    return;
                foreach (var pt in ptPath)
                {
                    if (Double.IsNaN(pt.X) || Double.IsNaN(pt.Y))
                        return;
                }

                var fe = control as FrameworkElement;
                double dValueTop = 0;
                var actTop = fe.ReadLocalValue(TopProperty);
                if (actTop == DependencyProperty.UnsetValue)
                {
                    //if (isInk)
                    //    dValueTop = InkCanvas.GetTop(fe);
                    //else
                        dValueTop = Canvas.GetTop(fe);
                    fe.SetValue(TopProperty, dValueTop);
                }
                else
                    dValueTop = (double)actTop;

                if (Double.IsNaN(dValueTop))
                    return;

                double dValueLeft = 0;
                var actLeft = fe.ReadLocalValue(LeftProperty);
                if (actLeft == DependencyProperty.UnsetValue)
                {
                    //if (isInk)
                    //    dValueLeft = InkCanvas.GetLeft(fe);
                    //else
                        dValueLeft = Canvas.GetLeft(fe);
                    fe.SetValue(LeftProperty, dValueLeft);
                }
                else
                    dValueLeft = (double)actLeft;

                if (Double.IsNaN(dValueLeft))
                    return;
                if(milliseconds == 0)
                {
                    if (ptPath != null && ptPath.Length > 0)
                    {
                        var lastpt = ptPath[0];
                        Canvas.SetLeft(fe,lastpt.X);
                        Canvas.SetTop(fe,lastpt.Y);
                    }
                }
                Storyboard sb = new Storyboard();
                if (completed != null)
                    sb.Completed += completed;

                /*
                DoubleAnimationUsingKeyFrames dakfx = new DoubleAnimationUsingKeyFrames
                {
                    Duration = new Duration(TimeSpan.FromMilliseconds(milliseconds))
                };
                int nCount = 0;
                Array.ForEach(ptPath, pt =>
                {
#if !WINDOWS_UWP
                    if (bSpline)
                        dakfx.KeyFrames.Add(new SplineDoubleKeyFrame(pt.X, KeyTime.FromPercent(++nCount / ptPath.Length)));
                    else
                        dakfx.KeyFrames.Add(new LinearDoubleKeyFrame(pt.X, KeyTime.FromPercent(++nCount / ptPath.Length)));
#else
                    if (bSpline)
                        dakfx.KeyFrames.Add(new SplineDoubleKeyFrame()
                        {
                            Value = pt.X,
                            KeyTime = KeyTime.FromTimeSpan(TimeSpan.FromMilliseconds(milliseconds / ptPath.Length * ++nCount))
                        });
                    else
                        dakfx.KeyFrames.Add(new LinearDoubleKeyFrame()
                        {
                            Value = pt.X,
                            KeyTime = KeyTime.FromTimeSpan(TimeSpan.FromMilliseconds(milliseconds / ptPath.Length * ++nCount))
                        });
#endif
                });
                */
                DoubleAnimationUsingPath dakfx = new DoubleAnimationUsingPath()
                {
                    Duration = new Duration(TimeSpan.FromMilliseconds(milliseconds)),
                    Source = PathAnimationSource.X
                };
                var geometry = new PathGeometry();
                var list = new List<PathSegment>();
                list.Add(new PolyLineSegment(ptPath, true));
                var figure = new PathFigure(ptPath[0], list, false);
                geometry.Figures.Add(figure);
                dakfx.PathGeometry = geometry;
                dakfx.AutoReverse = autoreverse;
                if (bForever)
                    dakfx.RepeatBehavior = RepeatBehavior.Forever;
                sb.Children.Add(dakfx);

                //if (isInk)
                //    Storyboard.SetTargetProperty(dakfx,
                //        new PropertyPath("(InkCanvas.Left)"));
                //else
                    Storyboard.SetTargetProperty(dakfx,
                        new PropertyPath("(Canvas.Left)"));
                Storyboard.SetTarget(dakfx, control);

                /*
                DoubleAnimationUsingKeyFrames dakfy = new DoubleAnimationUsingKeyFrames
                {
                    Duration = new Duration(TimeSpan.FromMilliseconds(milliseconds))
                };
                nCount = 0;
                Array.ForEach(ptPath, pt =>
                {
#if !WINDOWS_UWP
                    if (bSpline)
                        dakfy.KeyFrames.Add(new SplineDoubleKeyFrame(pt.Y, KeyTime.FromPercent(++nCount / ptPath.Length)));
                    else
                        dakfy.KeyFrames.Add(new LinearDoubleKeyFrame(pt.Y, KeyTime.FromPercent(++nCount / ptPath.Length)));
#else
                    if (bSpline)
                        dakfy.KeyFrames.Add(new SplineDoubleKeyFrame()
                        {
                            Value = pt.Y,
                            KeyTime = KeyTime.FromTimeSpan(TimeSpan.FromMilliseconds(milliseconds / ptPath.Length * ++nCount))
                        });
                    else
                        dakfy.KeyFrames.Add(new LinearDoubleKeyFrame()
                        {
                            Value = pt.Y,
                            KeyTime = KeyTime.FromTimeSpan(TimeSpan.FromMilliseconds(milliseconds / ptPath.Length * ++nCount))
                        });
#endif
                });
                */
                DoubleAnimationUsingPath dakfy = new DoubleAnimationUsingPath()
                {
                    Duration = new Duration(TimeSpan.FromMilliseconds(milliseconds)),
                    Source = PathAnimationSource.Y
                };
                dakfy.PathGeometry = geometry;
                dakfy.AutoReverse = autoreverse;
                if (bForever)
                    dakfy.RepeatBehavior = RepeatBehavior.Forever;
                sb.Children.Add(dakfy);
                //if (isInk)
                //    Storyboard.SetTargetProperty(dakfy,
                //        new PropertyPath("(InkCanvas.Top)"));
                //else
                    Storyboard.SetTargetProperty(dakfy,
                        new PropertyPath("(Canvas.Top)"));
                Storyboard.SetTarget(dakfy, control);

                sb.Begin();
#if WINDOWS_UWP
                control.AddStoryBoard(sb, TranslateTransform.XProperty);
                control.AddStoryBoard(sb, TranslateTransform.YProperty);
#endif
            }
        }
#endif

        public static Storyboard ApplyStoryBoard(this UIElement element, Storyboard sb, bool bForever = false, bool autoreverse = false, EventHandler
#if WINDOWS_UWP
                                    <Object>
#endif
            completed = null)
        {
            if (sb == null)
                throw new ArgumentException("Cannot find the storyboard resource");
#if !WINDOWS_UWP
            else if (sb.IsFrozen)
                sb = sb.Clone();
#endif
            foreach (var child in sb.Children)
            {
                if (String.IsNullOrEmpty(Storyboard.GetTargetName(child)))
                    Storyboard.SetTarget(child, element);
            }
            if (completed != null)
                sb.Completed += completed;

            sb.AutoReverse = autoreverse;
            if (bForever)
                sb.RepeatBehavior = RepeatBehavior.Forever;

            SetTransform<ScaleTransform>(element, true);
            SetTransform<TranslateTransform>(element, true);
            SetTransform<SkewTransform>(element, true);
            SetTransform<RotateTransform>(element, true);
            sb.Begin();
            return sb;
        }

        public static Storyboard ApplyStoryBoard(this UIElement element, String ResourceName, bool bForever = false, bool autoreverse = false, EventHandler
#if WINDOWS_UWP
                                    <Object>
#endif
            completed = null)
        {
            if (String.IsNullOrEmpty(ResourceName))
                return null;

            Storyboard sb;
            FrameworkElement fe = element as FrameworkElement;
            if (fe is ContentControl && (fe as ContentControl).Content != null)
                fe = (fe as ContentControl).Content as FrameworkElement;
            if (fe != null)
#if !WINDOWS_UWP
                sb = fe.TryFindResource(ResourceName) as Storyboard;
#else
                sb = fe.Resources[ResourceName] as Storyboard;
#endif
            else
            {
#if !WINDOWS_UWP
                FrameworkElement Owner = element.FindAncestor<FrameworkElement>();
                if (Owner != null)
                    sb = fe.TryFindResource(ResourceName) as Storyboard;
                else
                    sb = Application.Current.TryFindResource(ResourceName) as Storyboard;
                if (sb == null)
                {
                    var map = fe.GetAllResourceTypesInChildren(typeof(Storyboard));
                    if (map.ContainsKey(ResourceName))
                        sb = map[ResourceName] as Storyboard;
                }
#else
                sb = Application.Current.Resources[ResourceName] as Storyboard;
#endif
            }

            if (sb != null)
                ApplyStoryBoard(fe, sb, bForever, autoreverse, completed);
            return sb;
        }

#if !WINDOWS_UWP
        public static void ApplyStoryBoard(this FrameworkElement element, bool bPrev, int milliseconds, bool bHorizontal)
        {
            DoubleAnimation followingAnimation = new DoubleAnimation();
            DoubleAnimation opacityAnimation = new DoubleAnimation();
            Duration duration = new Duration(TimeSpan.FromSeconds(200));
            Storyboard mMonthStoryboard = new Storyboard();
            TransformGroup tg = new TransformGroup();
            TranslateTransform tt = new TranslateTransform();
            mMonthStoryboard.Duration = duration;
            followingAnimation.Duration = new Duration(TimeSpan.FromMilliseconds(milliseconds));
            opacityAnimation.Duration = new Duration(TimeSpan.FromMilliseconds(milliseconds));
            if (bPrev)
            {
                if (bHorizontal)
                {
                    tt.X = -element.ActualWidth / 6; ////0;
                    followingAnimation.To = 0;
                    Storyboard.SetTargetProperty(followingAnimation, new PropertyPath("(UIElement.RenderTransform).(TransformGroup.Children)[0].(TranslateTransform.X)"));
                }
                else
                {
                    tt.Y = -15;
                    followingAnimation.To = 0;
                    Storyboard.SetTargetProperty(followingAnimation, new PropertyPath("(UIElement.RenderTransform).(TransformGroup.Children)[0].(TranslateTransform.Y)"));
                }
            }
            else
            {
                if (bHorizontal)
                {
                    tt.X = element.ActualWidth; ////0;
                    followingAnimation.To = 0;
                    followingAnimation.From = element.ActualWidth / 6;
                    Storyboard.SetTargetProperty(followingAnimation, new PropertyPath("(UIElement.RenderTransform).(TransformGroup.Children)[0].(TranslateTransform.X)"));
                }
                else
                {
                    tt.Y = element.ActualHeight / 4;
                    followingAnimation.To = 0;
                    Storyboard.SetTargetProperty(followingAnimation, new PropertyPath("(UIElement.RenderTransform).(TransformGroup.Children)[0].(TranslateTransform.Y)"));
                }
            }

            tg.Children.Add(tt);
            element.RenderTransform = tg;
            opacityAnimation.To = 1;
            element.Opacity = 0.00001;
            Storyboard.SetTargetProperty(opacityAnimation, new PropertyPath("(FrameworkElement.Opacity)"));
            Storyboard.SetTarget(opacityAnimation, element);
            mMonthStoryboard.Children.Add(opacityAnimation);
            Storyboard.SetTarget(followingAnimation, element);
            Storyboard.SetTargetName(followingAnimation, "element");
            //// Storyboard.SetTargetProperty(followingAnimation, new PropertyPath("(Height)"));
            ////followingAnimation.Completed += new EventHandler(followingAnimation_Completed);
            mMonthStoryboard.Children.Add(followingAnimation);

            mMonthStoryboard.Begin();
        }
#endif
        static DoubleAnimation CreateDoubleAnimation(double from, double to, int AnimationTime)
        {
            if (double.IsInfinity(from))
                from = 0.0;
            return new DoubleAnimation()
            {
                From = from,
                To = to,
                Duration = new Duration(TimeSpan.FromMilliseconds(AnimationTime)),
                FillBehavior = FillBehavior.HoldEnd
            };

        }

        static double GetScaleMinValue(Rect r, Size sz)
        {
            return Math.Min(r.Width / sz.Width, r.Height / sz.Height);
        }

#if !WINDOWS_UWP
        static public void MakeZoomInOutAnimation(this UIElement control, double scaleFrom, double scaleTo, double opacityFrom, double opacityTo, int AnimationTime)
        {
            if (control == null)
                throw new ArgumentNullException("control", "control is null.");

            Rect r = LayoutHelper.GetRelativeElementRect(control, VisualTreeHelper.GetParent(control) as UIElement);
            if (double.IsNaN(scaleTo))
                scaleTo = GetScaleMinValue(r, control.DesiredSize);
            if (double.IsNaN(scaleFrom))
                scaleFrom = GetScaleMinValue(r, control.DesiredSize);
            DoubleAnimation width = CreateDoubleAnimation(scaleFrom, scaleTo, AnimationTime);
            DoubleAnimation height = CreateDoubleAnimation(scaleFrom, scaleTo, AnimationTime);
            DoubleAnimation opacity = CreateDoubleAnimation(opacityFrom, opacityTo, AnimationTime);
            ScaleTransform scale = SetTransform<ScaleTransform>(control, true);

            //scale.CenterX = (control as FrameworkElement).ActualWidth / 2;
            //scale.CenterY = (control as FrameworkElement).ActualHeight / 2;
            // scale.CenterX = r.X + r.Width / 2;
            // scale.CenterY = r.Y + r.Height / 2;

            width.AutoReverse = true;
            width.RepeatBehavior = RepeatBehavior.Forever;
            height.AutoReverse = true;
            height.RepeatBehavior = RepeatBehavior.Forever;
            opacity.AutoReverse = true;
            opacity.RepeatBehavior = RepeatBehavior.Forever;

            scale.BeginAnimation(ScaleTransform.ScaleXProperty, width);
            scale.BeginAnimation(ScaleTransform.ScaleYProperty, height);
            control.BeginAnimation(UIElement.OpacityProperty, opacity);
        }
#endif
    }
}
