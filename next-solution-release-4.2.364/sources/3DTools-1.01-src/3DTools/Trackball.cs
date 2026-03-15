//---------------------------------------------------------------------------
//
// (c) Copyright Microsoft Corporation.
// This source is subject to the Microsoft Limited Permissive License.
// See http://www.microsoft.com/resources/sharedsource/licensingbasics/limitedpermissivelicense.mspx
// All other rights reserved.
//
// This file is part of the 3D Tools for Windows Presentation Foundation
// project.  For more information, see:
// 
// http://CodePlex.com/Wiki/View.aspx?ProjectName=3DTools
//
// The following article discusses the mechanics behind this
// trackball implementation: http://viewport3d.com/trackball.htm
//
// Reading the article is not required to use this sample code,
// but skimming it might be useful.
//
//---------------------------------------------------------------------------

using System;
using System.Linq;
using System.Diagnostics;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media.Media3D;
using System.Windows.Markup;
using System.Windows.Media.Animation;
using System.Windows.Media;
using System.Windows.Controls;

namespace _3DTools
{
    /// <summary>
    ///     Trackball is a utility class which observes the mouse events
    ///     on a specified FrameworkElement and produces a Transform3D
    ///     with the resultant rotation and scale.
    /// 
    ///     Example Usage:
    /// 
    ///         Trackball trackball = new Trackball();
    ///         trackball.EventSource = myElement;
    ///         myViewport3D.Camera.Transform = trackball.Transform;
    /// 
    ///     Because Viewport3Ds only raise events when the mouse is over the
    ///     rendered 3D geometry (as opposed to not when the mouse is within
    ///     the layout bounds) you usually want to use another element as 
    ///     your EventSource.  For example, a transparent border placed on
    ///     top of your Viewport3D works well:
    ///     
    ///         <Grid>
    ///           <ColumnDefinition />
    ///           <RowDefinition />
    ///           <Viewport3D Name="myViewport" ClipToBounds="True" Grid.Row="0" Grid.Column="0" />
    ///           <Border Name="myElement" Background="Transparent" Grid.Row="0" Grid.Column="0" />
    ///         </Grid>
    ///     
    ///     NOTE: The Transform property may be shared by multiple Cameras
    ///           if you want to have auxilary views following the trackball.
    /// 
    ///           It can also be useful to share the Transform property with
    ///           models in the scene that you want to move with the camera.
    ///           (For example, the Trackport3D's headlight is implemented
    ///           this way.)
    /// 
    ///           You may also use a Transform3DGroup to combine the
    ///           Transform property with additional Transforms.
    /// </summary> 
    public class Trackball
    {
        private FrameworkElement _eventSource;
        private Point _previousPosition2D;
        private Point _startingPosition2D;
        private Vector3D _previousPosition3D = new Vector3D(0, 0, 1);

        private Transform3DGroup _transform;
        private ScaleTransform3D _scale = new ScaleTransform3D();
        private AxisAngleRotation3D _rotation = new AxisAngleRotation3D();
        private TranslateTransform3D _translate = new TranslateTransform3D();

        public Trackball()
        {
            _transform = new Transform3DGroup();
            _transform.Children.Add(_scale);
            _transform.Children.Add(new RotateTransform3D(_rotation));
            _transform.Children.Add(_translate);
        }

        /// <summary>
        ///     A transform to move the camera or scene to the trackball's
        ///     current orientation and scale.
        /// </summary>
        public Transform3D Transform
        {
            get { return _transform; }
        }

        public ScaleTransform3D ScaleTransform
        {
            get { return _scale; }
        }

        public AxisAngleRotation3D RotateTransform
        {
            get { return _rotation; }
        }

        public TranslateTransform TranslateTransform2D
        {
            get 
            {
                return SetTransform<TranslateTransform>(_eventSource);
            }
        }

        public bool IsOnTouch { get; set; }

        double maxZoom = 0;
        public double MaxZoom
        {
            get
            {
                return maxZoom;
            }
            set
            {
                maxZoom = value;
            }
        }

        double minZoom = 0;
        public double MinZoom
        {
            get
            {
                return minZoom;
            }
            set
            {
                minZoom = value;
            }
        }

        double maxRotationAngleX = 0;
        public double MaxRotationAngleX
        { 
            get
            {
                return maxRotationAngleX;
            }
            set
            {
                maxRotationAngleX = value;
            }
        }

        double minRotationAngleX = 0;
        public double MinRotationAngleX
        {
            get
            {
                return minRotationAngleX;
            }
            set
            {
                minRotationAngleX = value;
            }
        }

        double maxRotationAngleY = 0;
        public double MaxRotationAngleY
        {
            get
            {
                return maxRotationAngleY;
            }
            set
            {
                maxRotationAngleY = value;
            }
        }

        double minRotationAngleY = 0;
        public double MinRotationAngleY
        {
            get
            {
                return minRotationAngleY;
            }
            set
            {
                minRotationAngleY = value;
            }
        }

        double maxRotationAngleZ = 0;
        public double MaxRotationAngleZ
        {
            get
            {
                return maxRotationAngleZ;
            }
            set
            {
                maxRotationAngleZ = value;
            }
        }

        double minRotationAngleZ = 0;
        public double MinRotationAngleZ
        {
            get
            {
                return minRotationAngleZ;
            }
            set
            {
                minRotationAngleZ = value;
            }
        }

        double maxTranslateOffsetX = 0;
        public double MaxTranslateOffsetX
        {
            get
            {
                return maxTranslateOffsetX;
            }
            set
            {
                maxTranslateOffsetX = value;
            }
        }

        double minTranslateOffsetX = 0;
        public double MinTranslateOffsetX
        {
            get
            {
                return minTranslateOffsetX;
            }
            set
            {
                minTranslateOffsetX = value;
            }
        }

        double maxTranslateOffsetY = 0;
        public double MaxTranslateOffsetY
        {
            get
            {
                return maxTranslateOffsetY;
            }
            set
            {
                maxTranslateOffsetY = value;
            }
        }

        double minTranslateOffsetY = 0;
        public double MinTranslateOffsetY
        {
            get
            {
                return minTranslateOffsetY;
            }
            set
            {
                minTranslateOffsetY = value;
            }
        }

        bool scalexPending;
        bool scaleyPending;
        bool scalezPending;
        bool centerxPending;
        bool centeryPending;
        bool centerzPending;
        bool anglePending;
        bool axisPending;
        bool translatexPending;
        bool translateyPending;
        
        public void ResetToInitialPosition()
        {
            _transform.Children.Clear();
            originalCameraTranform = null;
        }

        public void GetCameraPositionFromScreen()
        {
            if (_eventSource is Viewport3D)
            {
                var vp3d = _eventSource as Viewport3D;
                originalCameraTranform = vp3d.Camera.Transform;
                AddTranform(originalCameraTranform);
            }
        }

        public void Animate(ScaleTransform3D s, AxisAngleRotation3D r, TranslateTransform translate,
                            int milliseconds, EasingFunctionBase type)
        {
            if (scalexPending ||
                scaleyPending ||
                scalezPending ||
                centerxPending ||
                centeryPending ||
                centerzPending ||
                anglePending ||
                axisPending ||
                translatexPending ||
                translateyPending)
                return;

            //if (_transform.Children.Count > 2)
            {
                _transform.Children.Clear();
                _transform.Children.Add(_scale);
                _transform.Children.Add(new RotateTransform3D(_rotation));
                _transform.Children.Add(_translate);
                AddTranform(originalCameraTranform);
            }

            var t = SetTransform<TranslateTransform>(_eventSource);
            double tx = 0;
            double ty = 0;
            if (translate != null)
            {
                tx = translate.X;
                ty = translate.Y;
            }
            var translatex = new DoubleAnimation()
            {
                To = tx,
                AutoReverse = false,
                Duration = new Duration(TimeSpan.FromMilliseconds(milliseconds)),
                EasingFunction = type
            };
            translatex.Completed += (o, e) =>
            {
                t.BeginAnimation(TranslateTransform.XProperty, null);
                t.X = tx;
                translatexPending = false;
            };

            var translatey = new DoubleAnimation()
            {
                To = ty,
                AutoReverse = false,
                Duration = new Duration(TimeSpan.FromMilliseconds(milliseconds)),
                EasingFunction = type
            };
            translatey.Completed += (o, e) =>
            {
                t.BeginAnimation(TranslateTransform.YProperty, null);
                t.Y = ty;
                translateyPending = false;
            };

            var scalex = new DoubleAnimation()
            {
                To = s.ScaleX,
                AutoReverse = false,
                Duration = new Duration(TimeSpan.FromMilliseconds(milliseconds)),
                EasingFunction = type
            };
            scalex.Completed += (o, e) =>
                {
                    _scale.BeginAnimation(ScaleTransform3D.ScaleXProperty, null);
                    _scale.ScaleX = s.ScaleX;
                    scalexPending = false;
                    OnZoomChanged();
                };

            var scaley = new DoubleAnimation()
            {
                To = s.ScaleY,
                AutoReverse = false,
                Duration = new Duration(TimeSpan.FromMilliseconds(milliseconds)),
                EasingFunction = type
            };
            scaley.Completed += (o, e) =>
            {
                _scale.BeginAnimation(ScaleTransform3D.ScaleYProperty, null);
                _scale.ScaleY = s.ScaleY;
                scaleyPending = false;
            };

            var scalez = new DoubleAnimation()
            {
                To = s.ScaleZ,
                AutoReverse = false,
                Duration = new Duration(TimeSpan.FromMilliseconds(milliseconds)),
                EasingFunction = type
            };
            scalez.Completed += (o, e) =>
            {
                _scale.BeginAnimation(ScaleTransform3D.ScaleZProperty, null);
                _scale.ScaleZ = s.ScaleZ;
                scalezPending = false;
            };

            var centerx = new DoubleAnimation()
            {
                To = s.CenterX,
                AutoReverse = false,
                Duration = new Duration(TimeSpan.FromMilliseconds(milliseconds)),
                EasingFunction = type
            };
            centerx.Completed += (o, e) =>
            {
                _scale.BeginAnimation(ScaleTransform3D.CenterXProperty, null);
                _scale.CenterX = s.CenterX;
                centerxPending = false;
            };

            var centery = new DoubleAnimation()
            {
                To = s.CenterY,
                AutoReverse = false,
                Duration = new Duration(TimeSpan.FromMilliseconds(milliseconds)),
                EasingFunction = type
            };
            centery.Completed += (o, e) =>
            {
                _scale.BeginAnimation(ScaleTransform3D.CenterYProperty, null);
                _scale.CenterY = s.CenterY;
                centeryPending = false;
            };

            var centerz = new DoubleAnimation()
            {
                To = s.CenterZ,
                AutoReverse = false,
                Duration = new Duration(TimeSpan.FromMilliseconds(milliseconds)),
                EasingFunction = type
            };
            centerz.Completed += (o, e) =>
            {
                _scale.BeginAnimation(ScaleTransform3D.CenterZProperty, null);
                _scale.CenterZ = s.CenterZ;
                centerzPending = false;
            };

            var angle = new DoubleAnimation()
            {
                To = r.Angle,
                AutoReverse = false,
                Duration = new Duration(TimeSpan.FromMilliseconds(milliseconds)),
                EasingFunction = type
            };
            angle.Completed += (o, e) =>
            {
                _rotation.BeginAnimation(AxisAngleRotation3D.AngleProperty, null);
                _rotation.Angle = r.Angle;
                anglePending = false;
            };

            var axis = new Vector3DAnimation()
            {
                To = r.Axis,
                AutoReverse = false,
                Duration = new Duration(TimeSpan.FromMilliseconds(milliseconds)),
                EasingFunction = type
            };
            axis.Completed += (o, e) =>
            {
                _rotation.BeginAnimation(AxisAngleRotation3D.AxisProperty, null);
                _rotation.Axis = r.Axis;
                axisPending = false;
            };

            scalexPending =
            scaleyPending =
            scalezPending =
            centerxPending =
            centeryPending =
            centerzPending =
            anglePending =
            translateyPending = 
            translatexPending =
            axisPending = true;

            t.BeginAnimation(TranslateTransform.XProperty, translatex);
            t.BeginAnimation(TranslateTransform.YProperty, translatey);

            _scale.BeginAnimation(ScaleTransform3D.ScaleXProperty, scalex);
            _scale.BeginAnimation(ScaleTransform3D.ScaleYProperty, scaley);
            _scale.BeginAnimation(ScaleTransform3D.ScaleZProperty, scalez);

            _scale.BeginAnimation(ScaleTransform3D.CenterXProperty, centerx);
            _scale.BeginAnimation(ScaleTransform3D.CenterYProperty, centery);
            _scale.BeginAnimation(ScaleTransform3D.CenterZProperty, centerz);

            _rotation.BeginAnimation(AxisAngleRotation3D.AngleProperty, angle);
            _rotation.BeginAnimation(AxisAngleRotation3D.AxisProperty, axis);
        }


        #region Event Handling

        bool panMode;
        public bool PanMode
        {
            get
            {
                return panMode;
            }
            set
            {
                panMode = value;
            }
        }

        /// <summary>
        ///     The FrameworkElement we listen to for mouse events.
        /// </summary>
        /// 
        Transform3D originalCameraTranform;
        void AddTranform(Transform3D t)
        {
            if (t == null)
                return;

            if (t is Transform3DGroup)
            {
                var group = t as Transform3DGroup;
                foreach (var child in group.Children)
                    _transform.Children.Add(child);
            }
            else
                _transform.Children.Add(t);


        }

        public FrameworkElement EventSource
        {
            get { return _eventSource; }
            
            set
            {
                if (_eventSource == value)
                    return;

                ClearEvents();
                _eventSource = value;
                SetTouchDevice(IsOnTouch);
                if (_eventSource is Viewport3D)
                {
                    var vp3d = _eventSource as Viewport3D;
                    originalCameraTranform = null; // vp3d.Camera.Transform;
                    AddTranform(originalCameraTranform);
                }
                /*
                if (_eventSource != null)
                {
                    if (LayoutHelper.HasTouchInput())
                    {
                        EventSource.ManipulationStarting -= ManipulationStarting;
                        EventSource.ManipulationDelta -= ManipulationDelta;
                    }
                    else
                    {
                        _eventSource.MouseDown -= this.OnMouseDown;
                        _eventSource.MouseUp -= this.OnMouseUp;
                        _eventSource.MouseMove -= this.OnMouseMove;
                    }
                }

                _eventSource = value;

                if (_eventSource != null)
                {
                    EventSource.ManipulationStarting += ManipulationStarting;
                    EventSource.ManipulationDelta += ManipulationDelta;
                    //_eventSource.MouseDown += this.OnMouseDown;
                    //_eventSource.MouseUp += this.OnMouseUp;
                    //_eventSource.MouseMove += this.OnMouseMove;
                }
                */
            }
        }

        void ClearEvents()
        {
            if (_eventSource == null)
                return;

            _eventSource.ManipulationStarting -= ManipulationStarting;
            _eventSource.ManipulationDelta -= ManipulationDelta;
            _eventSource.ManipulationCompleted -= ManipulationCompleted;
            _eventSource.MouseDown -= this.OnMouseDown;
            _eventSource.MouseUp -= this.OnMouseUp;
            _eventSource.MouseMove -= this.OnMouseMove;
            _eventSource.TouchDown -= OnTouchDown;
            _eventSource.TouchUp -= OnTouchUp;
            _eventSource.TouchMove -= OnTouchMove;
        }

        public void SetTouchDevice(bool bTouch)
        {
            ClearEvents();

            if (_eventSource == null)
                return;

            // if (bTouch)
            {
                IsOnTouch = bTouch;
                _eventSource.ManipulationStarting += ManipulationStarting;
                _eventSource.ManipulationDelta += ManipulationDelta;
                _eventSource.ManipulationCompleted += ManipulationCompleted;
                _eventSource.TouchDown += OnTouchDown;
                _eventSource.TouchUp += OnTouchUp;
                _eventSource.TouchMove += OnTouchMove;
            }
            // else
            {
                _eventSource.MouseDown += this.OnMouseDown;
                _eventSource.MouseUp += this.OnMouseUp;
                _eventSource.MouseMove += this.OnMouseMove;
            }
        }

        bool bManipulating;
        void ManipulationStarting(object sender, ManipulationStartingEventArgs e)
        {
            e.ManipulationContainer = EventSource;
            e.Handled = true;
            Console.WriteLine("manipulation starting");
        }

        void ManipulationCompleted(object sender, ManipulationCompletedEventArgs e)
        {
            e.Handled = true;
            bManipulating = false;
            Console.WriteLine("manipulation completed");
        }

        void ManipulationDelta(object sender, ManipulationDeltaEventArgs e)
        {
            int nManipulators = 0;
            foreach(var man in e.Manipulators)
                ++nManipulators;

            if (nManipulators > 1)
            {
                bManipulating = true;
                var prevValue = _scale.ScaleX;
                _scale.ScaleX /= e.DeltaManipulation.Scale.X;
                _scale.ScaleY /= e.DeltaManipulation.Scale.Y;
                _scale.ScaleZ /= e.DeltaManipulation.Scale.X;

                if (MaxZoom != 0)
                {
                    _scale.ScaleX = Math.Min(_scale.ScaleX, MaxZoom);
                    _scale.ScaleY = Math.Min(_scale.ScaleY, MaxZoom);
                    _scale.ScaleZ = Math.Min(_scale.ScaleZ, MaxZoom);
                }
                if (MinZoom != 0)
                {
                    _scale.ScaleX = Math.Max(_scale.ScaleX, MinZoom);
                    _scale.ScaleY = Math.Max(_scale.ScaleY, MinZoom);
                    _scale.ScaleZ = Math.Max(_scale.ScaleZ, MinZoom);
                }

                if (prevValue != _scale.ScaleX)
                    OnZoomChanged();

                e.Handled = true;
            }
            //else
            //{
            //    Vector delta = e.DeltaManipulation.Translation;
            //    if (delta.X != 0 || delta.Y != 0)
            //    {
            //        /*
            //        // Convert delta to a 3D vector.
            //        Vector3D vOriginal = new Vector3D(delta.X, -delta.Y, 0d);
            //        Vector3D vZ = new Vector3D(0, 0, 1);
            //        // Find a vector that is perpendicular with the delta vector on the XY surface. This will be the rotation axis.
            //        Vector3D perpendicular = Vector3D.CrossProduct(vOriginal, vZ);
            //        // The QuaternionRotation3D allows you to easily specify a rotation axis.
            //        QuaternionRotation3D quatenion = new QuaternionRotation3D() { Quaternion = new Quaternion(perpendicular, 3) };

            //        _transform.Children.Add(new RotateTransform3D(quatenion));

            //        // Write the new orientation back to the Rotation3D
            //        // _rotation.Axis = quatenion.Quaternion.Axis;
            //        // _rotation.Angle = quatenion.Quaternion.Angle;
            //         */
            //        totalDeltaX += delta.X;
            //        totalDeltaY += delta.Y;

            //        var currentPosition = new Point(e.ManipulationOrigin.X + totalDeltaX, e.ManipulationOrigin.Y + totalDeltaY);
            //        Vector3D currentPosition3D = ProjectToTrackball(
            //            EventSource.ActualWidth, EventSource.ActualHeight, currentPosition);

            //        Vector3D axis = Vector3D.CrossProduct(_previousPosition3D, currentPosition3D);
            //        double angle = Vector3D.AngleBetween(_previousPosition3D, currentPosition3D);
            //        if (angle == 0)
            //            return;
            //        Quaternion d = new Quaternion(axis, -angle);

            //        // Get the current orientantion from the RotateTransform3D
            //        AxisAngleRotation3D r = _rotation;
            //        Quaternion q = new Quaternion(_rotation.Axis, _rotation.Angle);

            //        // Compose the delta with the previous orientation
            //        q *= d;

            //        // Write the new orientation back to the Rotation3D
            //        _rotation.Axis = q.Axis;
            //        _rotation.Angle = q.Angle;

            //        _previousPosition3D = currentPosition3D;
            //    }
            //}
        }

        readonly Stopwatch _doubleTapStopwatch = new Stopwatch();
        Point _lastTapLocation;
        Object _lastTouchSource;
        static double GetDistanceBetweenPoints(Point p, Point q)
        {
            if (p == q)
                return 0;

            double a = p.X - q.X;
            double b = p.Y - q.Y;
            double distance = Math.Sqrt(a * a + b * b);
            return distance;
        }

        static readonly int doubleClickTime = System.Windows.Forms.SystemInformation.DoubleClickTime;
        int nDoubleTapTouchDeviceId = 0;
        bool IsDoubleTap(TouchEventArgs e)
        {
            Point currentTapPosition = e.GetTouchPoint(EventSource).Position;
            bool tapsAreCloseInDistance = GetDistanceBetweenPoints(currentTapPosition, _lastTapLocation) < 40;
            _lastTapLocation = currentTapPosition;

            TimeSpan elapsed = _doubleTapStopwatch.Elapsed;
            _doubleTapStopwatch.Restart();
            bool tapsAreCloseInTime = (elapsed != TimeSpan.Zero && elapsed < TimeSpan.FromMilliseconds(doubleClickTime));

            if (nDoubleTapTouchDeviceId == 0)
            {
                nDoubleTapTouchDeviceId = e.TouchDevice.Id;
                return false;
            }
            else if (nDoubleTapTouchDeviceId != e.TouchDevice.Id)
            {
                nDoubleTapTouchDeviceId = 0;
                return false;
            }
            else
                nDoubleTapTouchDeviceId = 0;

            var ret = tapsAreCloseInDistance && tapsAreCloseInTime;
            if (ret)
                ret = _lastTouchSource == e.Source;
            _lastTouchSource = e.Source;

            return ret;
        }
        bool IsDoubleClick(MouseEventArgs e)
        {
            Point currentTapPosition = e.GetPosition(EventSource);
            bool tapsAreCloseInDistance = GetDistanceBetweenPoints(currentTapPosition, _lastTapLocation) < 40;
            _lastTapLocation = currentTapPosition;

            TimeSpan elapsed = _doubleTapStopwatch.Elapsed;
            _doubleTapStopwatch.Restart();
            bool tapsAreCloseInTime = (elapsed != TimeSpan.Zero && elapsed < TimeSpan.FromMilliseconds(doubleClickTime));

            var ret = tapsAreCloseInDistance && tapsAreCloseInTime;
            if (ret)
                ret = _lastTouchSource == e.Source;
            _lastTouchSource = e.Source;

            return ret;
        }

        TouchDevice capturedDevice;
        private void OnTouchDown(object sender, TouchEventArgs e)
        {
            if (IsDoubleTap(e))
            {
                PanMode = !PanMode;
                e.Handled = true;
                return;
            }

            if (bManipulating || capturedDevice != null)
                return;

            try
            {
                EventSource.CaptureTouch(e.TouchDevice);
            }
            catch
            {

            }
            capturedDevice = e.TouchDevice;
            var parent = LogicalTreeHelper.GetParent(EventSource) as IInputElement;
            if (!PanMode)
                parent = EventSource;
            _previousPosition2D = e.GetTouchPoint(parent).Position;
            _startingPosition2D = _previousPosition2D;
            _previousPosition3D = ProjectToTrackball(
                EventSource.ActualWidth,
                EventSource.ActualHeight,
                _previousPosition2D);
        }

        bool bMouseCaptured;
        private void OnMouseDown(object sender, MouseEventArgs e)
        {
            if (IsDoubleClick(e))
            {
                PanMode = !PanMode;
                return;
            }

            Mouse.Capture(EventSource, CaptureMode.Element);
            bMouseCaptured = true;
            var parent = LogicalTreeHelper.GetParent(EventSource) as IInputElement;
            if (!PanMode)
                parent = EventSource;
            _previousPosition2D = e.GetPosition(parent);
            _startingPosition2D = _previousPosition2D;
            _previousPosition3D = ProjectToTrackball(
                EventSource.ActualWidth,
                EventSource.ActualHeight,
                _previousPosition2D);
        }

        private void OnTouchUp(object sender, TouchEventArgs e)
        {
            if (e.TouchDevice != capturedDevice)
                return;

            try
            {
                EventSource.ReleaseTouchCapture(e.TouchDevice);
            }
            catch
            {

            }
            capturedDevice = null;
        }

        private void OnMouseUp(object sender, MouseEventArgs e)
        {
            Mouse.Capture(EventSource, CaptureMode.None);
            bMouseCaptured = false;
            OnTrackCompleted();
        }

        private void OnTouchMove(object sender, TouchEventArgs e)
        {
            if (bManipulating || e.TouchDevice != capturedDevice)
                return;
            var parent = LogicalTreeHelper.GetParent(EventSource) as IInputElement;
            if (!PanMode)
                parent = EventSource;
            var currentPosition = e.GetTouchPoint(parent).Position;
            Track(currentPosition);

            _previousPosition2D = currentPosition;
        }

        private void OnMouseMove(object sender, MouseEventArgs e)
        {
            if (!bMouseCaptured)
                return;

            var parent = LogicalTreeHelper.GetParent(EventSource) as IInputElement;
            if (!PanMode)
                parent = EventSource;
            var currentPosition = e.GetPosition(parent);

            // Prefer tracking to zooming if both buttons are pressed.
            if (e.LeftButton == MouseButtonState.Pressed)
            {
                Track(currentPosition);
            }
            else if (e.RightButton == MouseButtonState.Pressed)
            {
                Zoom(currentPosition);
            }

            _previousPosition2D = currentPosition;
        }

        #endregion Event Handling

        private void Track(Point currentPosition)
        {
            if (!PanMode)
            {
                Vector3D currentPosition3D = ProjectToTrackball(
                    EventSource.ActualWidth, EventSource.ActualHeight, currentPosition);

                Vector3D axis = Vector3D.CrossProduct(_previousPosition3D, currentPosition3D);
                double angle = Vector3D.AngleBetween(_previousPosition3D, currentPosition3D);
                if (angle == 0)
                    return;
                Quaternion delta = new Quaternion(axis, -angle);

                // Get the current orientantion from the RotateTransform3D
                AxisAngleRotation3D r = _rotation;
                Quaternion q = new Quaternion(_rotation.Axis, _rotation.Angle);

                // Compose the delta with the previous orientation
                q *= delta;

                // Write the new orientation back to the Rotation3D
                _rotation.Axis = q.Axis;
                if (MaxRotationAngleX != 0)
                    q.X = Math.Min(q.X, MaxRotationAngleX);
                if (MaxRotationAngleY != 0)
                    q.Y = Math.Min(q.Y, MaxRotationAngleY);
                if (MaxRotationAngleZ != 0)
                    q.Z = Math.Min(q.Z, MaxRotationAngleZ);
                if (MinRotationAngleX != 0)
                    q.X = Math.Max(q.X, MinRotationAngleX);
                if (MinRotationAngleY != 0)
                    q.Y = Math.Max(q.Y, MinRotationAngleY);
                if (MinRotationAngleZ != 0)
                    q.Z = Math.Max(q.Z, MinRotationAngleZ);

                _rotation.Angle = q.Angle;

                _previousPosition3D = currentPosition3D;                
            }
            else
            {
                //Vector3D currentPosition3D = ProjectToTrackball(
                //    EventSource.ActualWidth, EventSource.ActualHeight, currentPosition);
                //Vector3D delta3D = _previousPosition3D - currentPosition3D;

                var t = SetTransform<TranslateTransform>(_eventSource);

                Debug.WriteLine(String.Format("Starting X = {0} Starting Y = {1} Current X = {2} Current Y = {3}", _startingPosition2D.X, _startingPosition2D.Y,
                    currentPosition.X, currentPosition.Y));

                t.X -= _startingPosition2D.X - currentPosition.X;
                t.Y -= _startingPosition2D.Y - currentPosition.Y;
                //_translate.OffsetX += delta3D.X;
                //_translate.OffsetY += delta3D.Y;
                if (MaxTranslateOffsetX != 0)
                    t.X = Math.Min(t.X, MaxTranslateOffsetX);
                if (MinTranslateOffsetX != 0)
                    t.X = Math.Max(t.X, MinTranslateOffsetX);
                if (MaxTranslateOffsetY != 0)
                    t.Y = Math.Min(t.Y, MaxTranslateOffsetY);
                if (MinTranslateOffsetY != 0)
                    t.Y = Math.Max(t.Y, MinTranslateOffsetY);

                _startingPosition2D = currentPosition;
                // _previousPosition3D = currentPosition3D;
            }
        }

        private Vector3D ProjectToTrackball(double width, double height, Point point)
        {
            double x = point.X / (width / 2);    // Scale so bounds map to [0,0] - [2,2]
            double y = point.Y / (height / 2);

            x = x - 1;                           // Translate 0,0 to the center
            y = 1 - y;                           // Flip so +Y is up instead of down

            double z2 = 1 - x * x - y * y;       // z^2 = 1 - x^2 - y^2
            double z = z2 > 0 ? Math.Sqrt(z2) : 0;

            return new Vector3D(x, y, z);
        }

        private void Zoom(Point currentPosition)
        {
            double yDelta = currentPosition.Y - _previousPosition2D.Y;
            
            double scale = Math.Exp(yDelta / 100);    // e^(yDelta/100) is fairly arbitrary.

            var prevValue = _scale.ScaleX;
            _scale.ScaleX *= scale;
            _scale.ScaleY *= scale;
            _scale.ScaleZ *= scale;

            if (MaxZoom != 0)
            {
                _scale.ScaleX = Math.Min(_scale.ScaleX, MaxZoom);
                _scale.ScaleY = Math.Min(_scale.ScaleY, MaxZoom);
                _scale.ScaleZ = Math.Min(_scale.ScaleZ, MaxZoom);
            }
            if (MinZoom != 0)
            {
                _scale.ScaleX = Math.Max(_scale.ScaleX, MinZoom);
                _scale.ScaleY = Math.Max(_scale.ScaleY, MinZoom);
                _scale.ScaleZ = Math.Max(_scale.ScaleZ, MinZoom);
            }

            if (prevValue != _scale.ScaleX)
                OnZoomChanged();
        }

        public double ScaleValue
        {
            get
            {
                return _scale.ScaleX;
            }
            set
            {
                if (value == _scale.ScaleX)
                    return;
                _scale.ScaleX = value;
                _scale.ScaleY = value;
                _scale.ScaleZ = value;

                if (MaxZoom != 0)
                {
                    _scale.ScaleX = Math.Min(_scale.ScaleX, MaxZoom);
                    _scale.ScaleY = Math.Min(_scale.ScaleY, MaxZoom);
                    _scale.ScaleZ = Math.Min(_scale.ScaleZ, MaxZoom);
                }
                if (MinZoom != 0)
                {
                    _scale.ScaleX = Math.Max(_scale.ScaleX, MinZoom);
                    _scale.ScaleY = Math.Max(_scale.ScaleY, MinZoom);
                    _scale.ScaleZ = Math.Max(_scale.ScaleZ, MinZoom);
                }
            }
        }
        public event EventHandler ZoomChanged;
        void OnZoomChanged()
        {
            var t = ZoomChanged;
            if (t != null)
                t(this, EventArgs.Empty);
        }

        public event EventHandler TrackCompleted;
        void OnTrackCompleted()
        {
            var t = TrackCompleted;
            if (t != null)
            {
                t(this, EventArgs.Empty);
            }
        }

        public static T SetTransform<T>(UIElement control) where T : Transform, new()
        {
            T transform = new T();
            if (control.RenderTransform == null ||
                !(control.RenderTransform is TransformGroup))
            {
                if (control.RenderTransform is T)
                    transform = control.RenderTransform as T;
                else
                {
                    TransformGroup tg = new TransformGroup();
                    if (control.RenderTransform != null && control.RenderTransform is T)
                    {
                        tg.Children.Add(control.RenderTransform);
                        transform = control.RenderTransform as T;
                    }
                    else
                    {
                        transform = new T();
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
                            where fx is T
                            select fx;
                if (tf.Count() == 0)
                {
                    transform = new T();
                    tg.Children.Add(transform);
                }
                else
                    transform = tf.First() as T;
            }

            return transform;
        }
    }
}
