using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Media.Animation;
using System.Windows.Media;
using System.Windows.Data;
using System.Windows.Media.Effects;
using System.Diagnostics;

#if !SILVERLIGHT
using Utilities.Animations.Converters;
using Utilities.WPF;
using System.Windows.Media.Media3D;
using _3DTools;
#endif

namespace Utilities.Animations
{
#if !SILVERLIGHT
    public static class Animations3D
    {
        public static int GetTransformIndex<T>(this Model3D control) where T : Transform3D, new()
        {
            if (control.Transform == null)
                return -1;

            if (!(control.Transform is Transform3DGroup))
            {
                var newGroup = new Transform3DGroup();
                newGroup.Children.Add(control.Transform);
                control.Transform = newGroup;
            }

            var tg = control.Transform as Transform3DGroup;

            var tf = from fx in tg.Children
                     where fx is T && listCreatedTransform3D.Contains(fx)
                     select fx;
            if (tf.Count() == 0)
                return -1;

            return tg.Children.IndexOf(tf.First() as T);
        }

        public static void RemoveTransform(this Model3D control, Transform3D transform)
        {
            if (listCreatedCenterPoint3D3D.Contains(control))
                listCreatedCenterPoint3D3D.Remove(control);

            if (control.Transform == null)
                return;

            if (control.Transform == transform)
                control.Transform = null;
            else if (control.Transform is Transform3DGroup)
            {
                var tg = control.Transform as Transform3DGroup;
                if (tg.Children.Contains(transform))
                    tg.Children.Remove(transform);
            }

            if (listCreatedTransform3D.Contains(transform))
                listCreatedTransform3D.Remove(transform);
        }


        readonly static List<Model3D> listCreatedCenterPoint3D3D = new List<Model3D>();
        public static AxisAngleRotation3D SetAxisAngle(this Model3D control, Vector3D vector,
                                                        double centerX, double centerY, double centerZ)
        {
            var t = SetTransform<RotateTransform3D>(control, true);
            var rotate = (from c in (control.Transform as Transform3DGroup).Children.OfType<RotateTransform3D>()
                          where listCreatedTransform3D.Contains(c)
                          select c).ToList();
            var axisList = (from c in rotate
                            where c.Rotation is AxisAngleRotation3D && (c.Rotation as AxisAngleRotation3D).Axis == vector
                            select c.Rotation as AxisAngleRotation3D).ToList();

            if (!listCreatedCenterPoint3D3D.Contains(control))
            {
                listCreatedCenterPoint3D3D.Add(control);
                var center = MathUtils.GetCenter(control.Bounds);
                rotate[0].CenterX = center.X * (2 * centerX);
                rotate[0].CenterY = center.Y * (2 * centerY);
                rotate[0].CenterZ = center.Z * (2 * centerZ);
            }
            if (axisList.Count > 0)
                return axisList[0];

            var axis = new AxisAngleRotation3D(vector, 0);
            if (rotate.Count == 0 || rotate[0].Rotation is AxisAngleRotation3D)
            {
                var newTransofrm3D = new RotateTransform3D();
                (control.Transform as Transform3DGroup).Children.Add(newTransofrm3D);
                listCreatedTransform3D.Add(newTransofrm3D);

                var center = MathUtils.GetCenter(control.Bounds);
                newTransofrm3D.CenterX = center.X * (2 * centerX);
                newTransofrm3D.CenterY = center.Y * (2 * centerY);
                newTransofrm3D.CenterZ = center.Z * (2 * centerZ);

                newTransofrm3D.Rotation = axis;
            }
            else
                rotate[0].Rotation = axis;
            return axis;
        }

        readonly static List<Transform3D> listCreatedTransform3D = new List<Transform3D>();
        public static T SetTransform<T>(this Model3D control, bool bSet) where T : Transform3D, new()
        {
            //if (control.ReadLocalValue(UIElement.RenderTransformOriginProperty) == DependencyProperty.UnsetValue)
            //    control.RenderTransformOrigin = new Point(0.5, 0.5);

            T transform = null;
            if (bSet)
            {
                if (control.Transform == null ||
                    !(control.Transform is Transform3DGroup))
                {
                    if (control.Transform is T && listCreatedTransform3D.Contains(control.Transform))
                        transform = control.Transform as T;
                    else
                    {
                        var tg = new Transform3DGroup();
                        if (control.Transform != null && control.Transform is T && 
                            listCreatedTransform3D.Contains(control.Transform))
                        {
                            tg.Children.Add(control.Transform);
                            transform = control.Transform as T;
                        }
                        else
                        {
                            transform = new T();
                            listCreatedTransform3D.Add(transform);
                            if (control.Transform != null)
                                tg.Children.Add(control.Transform);
                            tg.Children.Add(transform);
                        }
                        control.Transform = tg;
                    }
                }
                else if (control.Transform is Transform3DGroup)
                {
                    var tg = control.Transform as Transform3DGroup;

                    var tf = from fx in tg.Children
                             where fx is T && listCreatedTransform3D.Contains(fx)
                             select fx;
                    if (tf.Count() == 0)
                    {
                        transform = new T();
                        listCreatedTransform3D.Add(transform);
                        tg.Children.Add(transform);
                    }
                    else
                        transform = tf.First() as T;
                }
            }
            else
            {
                if (control.Transform is T)
                    control.Transform = null;
                else if (control.Transform is Transform3DGroup)
                {
                    var tg = control.Transform as Transform3DGroup;

                    var tgcoll = new Transform3DGroup();
                    foreach (Transform3D t in tg.Children)
                    {
                        if (!(t is T))
                            tgcoll.Children.Add(t);
                    }
                    if (tgcoll.Children.Count == 0)
                        control.Transform = null;
                    else
                        control.Transform = tgcoll;
                }
            }

            return transform;
        }

        public static void Scale3D(this Model3D control, double targetScale, int milliseconds,
                                 bool bForever, bool autoreverse, EasingFunctionBase type,
                                 double centerX = 0.5, double centerY = 0.5, double centerZ = 0.5,
                                 EventHandler completed = null, bool isRelative = false)
        {
            if (control == null)
                // throw new ArgumentNullException("control", "control is null.");
                return;

            if (milliseconds < 0)
            {
                ScaleTransform3D t = SetTransform<ScaleTransform3D>(control, true);
#if SILVERLIGHT
                control.BeginAnimation(ScaleTransform3D.ScaleXProperty, null);
                control.BeginAnimation(ScaleTransform3D.ScaleYProperty, null);
                control.BeginAnimation(ScaleTransform3D.ScaleZProperty, null);
#else
                t.BeginAnimation(ScaleTransform3D.ScaleXProperty, null);
                t.BeginAnimation(ScaleTransform3D.ScaleYProperty, null);
                t.BeginAnimation(ScaleTransform3D.ScaleZProperty, null);
#endif
                RemoveTransform(control, t);
            }
            else
            {
                if (Double.IsNaN(targetScale))
                    return;

                ScaleTransform3D t = SetTransform<ScaleTransform3D>(control, true);
#if SILVERLIGHT
                control.BeginAnimation(ScaleTransform3D.ScaleXProperty, null);
                control.BeginAnimation(ScaleTransform3D.ScaleYProperty, null);
                control.BeginAnimation(ScaleTransform3D.ScaleZProperty, null);
#else
                t.BeginAnimation(ScaleTransform3D.ScaleXProperty, null);
                t.BeginAnimation(ScaleTransform3D.ScaleYProperty, null);
                t.BeginAnimation(ScaleTransform3D.ScaleZProperty, null);
#endif
                //int nTransformIndex = GetTransformIndex<ScaleTransform3D>(control);
                //if (nTransformIndex == -1)
                //    throw new ArgumentNullException("nTransformIndex");
                // t.ScaleX = t.ScaleY = t.ScaleZ = targetScale;
                var c = MathUtils.GetCenter(control.Bounds);
                t.CenterX = c.X * (2 * centerX);
                t.CenterY = c.Y * (2 * centerY);
                t.CenterZ = c.Z * (2 * centerZ);

                //if (bCenter)
                //{
                //    t.CenterX = (control as FrameworkElement).ActualWidth / 2;
                //    t.CenterY = (control as FrameworkElement).ActualHeight / 2;
                //}
                
                //Storyboard sb = new Storyboard();
                //if (completed != null)
                //    sb.Completed += completed;

                var dax = new DoubleAnimation()
                {
                    To = isRelative ? t.ScaleX + targetScale : targetScale,
                    Duration = new Duration(TimeSpan.FromMilliseconds(milliseconds)),
                    EasingFunction = type
                };
                dax.AutoReverse = autoreverse;
                if (bForever)
                    dax.RepeatBehavior = RepeatBehavior.Forever;
                //sb.Children.Add(dax);
                //Storyboard.SetTargetProperty(dax,
                //    new PropertyPath(String.Format("(Model3D.Transform).(Transform3DGroup.Children)[{0}].(ScaleTransform3D.ScaleX)", nTransformIndex)));
                //Storyboard.SetTarget(dax, control);

                var day = new DoubleAnimation()
                {
                    To = isRelative ? t.ScaleY + targetScale : targetScale,
                    Duration = new Duration(TimeSpan.FromMilliseconds(milliseconds)),
                    EasingFunction = type
                };
                day.AutoReverse = autoreverse;
                if (bForever)
                    day.RepeatBehavior = RepeatBehavior.Forever;
                //sb.Children.Add(day);
                //Storyboard.SetTargetProperty(day,
                //    new PropertyPath(String.Format("(Model3D.Transform).(Transform3DGroup.Children)[{0}].(ScaleTransform3D.ScaleY)", nTransformIndex)));
                //Storyboard.SetTarget(day, control);

                var daz = new DoubleAnimation()
                {
                    To = isRelative ? t.ScaleZ + targetScale : targetScale,
                    Duration = new Duration(TimeSpan.FromMilliseconds(milliseconds)),
                    EasingFunction = type
                };
                daz.AutoReverse = autoreverse;
                if (bForever)
                    daz.RepeatBehavior = RepeatBehavior.Forever;
                //sb.Children.Add(daz);
                //Storyboard.SetTargetProperty(daz,
                //    new PropertyPath(String.Format("(Model3D.Transform).(Transform3DGroup.Children)[{0}].(ScaleTransform3D.ScaleZ)", nTransformIndex)));
                //Storyboard.SetTarget(daz, control);

                //sb.Begin();

                if (completed != null)
                    daz.Completed += completed;

                t.BeginAnimation(ScaleTransform3D.ScaleXProperty, dax);
                t.BeginAnimation(ScaleTransform3D.ScaleYProperty, day);
                t.BeginAnimation(ScaleTransform3D.ScaleZProperty, daz);
#if SILVERLIGHT
                control.AddStoryBoard(sb, ScaleTransform3D.ScaleXProperty);
                control.AddStoryBoard(sb, ScaleTransform3D.ScaleYProperty);
                control.AddStoryBoard(sb, ScaleTransform3D.ScaleZProperty);
#endif
            }
        }


        public static void TranslateX3D(this Model3D control, double targetX, int milliseconds, bool bForever,
                                        bool autoreverse, EasingFunctionBase type, EventHandler completed = null)
        {
            if (control == null)
                // throw new ArgumentNullException("control", "control is null.");
                return;

            if (milliseconds < 0)
            {
                var t = SetTransform<TranslateTransform3D>(control, true);
                t.BeginAnimation(TranslateTransform3D.OffsetXProperty, null);
                RemoveTransform(control, t);
            }
            else
            {
                if (Double.IsNaN(targetX))
                    return;

                var t = SetTransform<TranslateTransform3D>(control, true);
                //t.BeginAnimation(TranslateTransform3D.OffsetXProperty, null);

                var da = new DoubleAnimation()
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
                t.BeginAnimation(TranslateTransform3D.OffsetXProperty, da);
            }
        }

        public static void TranslateY3D(this Model3D control, double targetY, int milliseconds, bool bForever,
                                        bool autoreverse, EasingFunctionBase type, EventHandler completed = null)
        {
            if (control == null)
                // throw new ArgumentNullException("control", "control is null.");
                return;

            if (milliseconds < 0)
            {
                var t = SetTransform<TranslateTransform3D>(control, true);
                t.BeginAnimation(TranslateTransform3D.OffsetYProperty, null);
                RemoveTransform(control, t);
            }
            else
            {
                if (Double.IsNaN(targetY))
                    return;

                var t = SetTransform<TranslateTransform3D>(control, true);
                //t.BeginAnimation(TranslateTransform3D.OffsetYProperty, null);

                var da = new DoubleAnimation()
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
                t.BeginAnimation(TranslateTransform3D.OffsetYProperty, da);
            }
        }

        public static void TranslateZ3D(this Model3D control, double targetZ, int milliseconds, bool bForever,
                                        bool autoreverse, EasingFunctionBase type, EventHandler completed = null)
        {
            if (control == null)
                // throw new ArgumentNullException("control", "control is null.");
                return;

            if (milliseconds < 0)
            {
                var t = SetTransform<TranslateTransform3D>(control, true);
                t.BeginAnimation(TranslateTransform3D.OffsetZProperty, null);
                RemoveTransform(control, t);
            }
            else
            {
                if (Double.IsNaN(targetZ))
                    return;

                var t = SetTransform<TranslateTransform3D>(control, true);
                //t.BeginAnimation(TranslateTransform3D.OffsetZProperty, null);

                var da = new DoubleAnimation()
                {
                    To = targetZ,
                    Duration = new Duration(TimeSpan.FromMilliseconds(milliseconds)),
                    EasingFunction = type
                };

                da.AutoReverse = autoreverse;
                if (bForever)
                    da.RepeatBehavior = RepeatBehavior.Forever;
                if (completed != null)
                    da.Completed += completed;
                t.BeginAnimation(TranslateTransform3D.OffsetZProperty, da);
            }
        }

        public static void RotateX3D(this Model3D control, double targetAngle,
                                    int milliseconds, bool bForever,
                                    bool autoreverse, EasingFunctionBase type,
                                    double centerX = 0.5, double centerY = 0.5, double centerZ = 0.5,
                                    EventHandler completed = null)
        {
            Rotate3D(control, new Vector3D(1, 0, 0), targetAngle,
                                     milliseconds, bForever,
                                     autoreverse, type,
                                     centerX, centerY, centerZ, completed);
        }

        public static void RotateY3D(this Model3D control, double targetAngle,
                                    int milliseconds, bool bForever,
                                    bool autoreverse, EasingFunctionBase type,
                                    double centerX = 0.5, double centerY = 0.5, double centerZ = 0.5,
                                    EventHandler completed = null)
        {
            Rotate3D(control, new Vector3D(0, 1, 0), targetAngle,
                                     milliseconds, bForever,
                                     autoreverse, type,
                                     centerX, centerY, centerZ, completed);
        }

        public static void RotateZ3D(this Model3D control, double targetAngle,
                                    int milliseconds, bool bForever,
                                    bool autoreverse, EasingFunctionBase type,
                                    double centerX = 0.5, double centerY = 0.5, double centerZ = 0.5,
                                    EventHandler completed = null)
        {
            Rotate3D(control, new Vector3D(0, 0, 1), targetAngle,
                                     milliseconds, bForever,
                                     autoreverse, type,
                                     centerX, centerY, centerZ, completed);
        }

        public static void Rotate3D(this Model3D control, Vector3D vector, double targetAngle, 
                                    int milliseconds, bool bForever,
                                    bool autoreverse, EasingFunctionBase type,
                                    double centerX = 0.5, double centerY = 0.5, double centerZ = 0.5,
                                    EventHandler completed = null)
        {
            if (control == null)
                // throw new ArgumentNullException("control", "control is null.");
                return;

            var axis = SetAxisAngle(control, vector, centerX, centerY, centerZ);
            if (milliseconds < 0)
            {
                var t = SetTransform<RotateTransform3D>(control, true);
                axis.BeginAnimation(AxisAngleRotation3D.AngleProperty, null);

                var rotate = (from c in (control.Transform as Transform3DGroup).Children.OfType<RotateTransform3D>()
                              where listCreatedTransform3D.Contains(c)
                              select c).ToList();
                var axisList = (from c in rotate
                                where c.Rotation is AxisAngleRotation3D && (c.Rotation as AxisAngleRotation3D).Axis == vector
                                select c).ToList();
                if (axisList.Count > 0)
                    RemoveTransform(control, axisList[0]);
            }
            else
            {
                if (Double.IsNaN(targetAngle))
                    return;

                //axis.BeginAnimation(AxisAngleRotation3D.AngleProperty, null);
                var da = new DoubleAnimation()
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
                axis.BeginAnimation(AxisAngleRotation3D.AngleProperty, da);
            }
        }
    }
#endif
}
