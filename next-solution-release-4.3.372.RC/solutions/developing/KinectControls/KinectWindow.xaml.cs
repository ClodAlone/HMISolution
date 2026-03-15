using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Threading;
using System.Windows.Interop;
// using System.Windows.Forms;
using System.Windows.Media.Animation;
using System.Windows.Automation.Peers;
using System.Windows.Automation.Provider;
using Microsoft.Kinect;
using Microsoft.Kinect.Samples.CursorControl;
using Coding4Fun.Kinect.Wpf;
using Microsoft.Speech.Recognition;
using System.IO;
using Microsoft.Speech.AudioFormat;
using Utilities;
using Utilities.WPF;
using System.Windows.Media.Media3D;
using Microsoft.Samples.Kinect.WpfViewers;
using Fizbin.Kinect.Gestures;
using Fizbin.Kinect.Gestures.Segments;
using Microsoft.Samples.Kinect.InteractionGallery.Utilities;
using System.Diagnostics;
using Microsoft.Kinect.Toolkit.Controls;
using System.Threading.Tasks;
using System.Windows.Threading;

namespace KinectControls
{
    public partial class KinectWindow : Window, IDisposable
    {
        #region Events
        public event EventHandler ReadyForContent;
        void OnReadyForContent()
        {
            var e = ReadyForContent;
            if (e != null)
                e(null/*this*/, new EventArgs());
        }

        public event EventHandler MoveBack;
        void OnMoveBack()
        {
            var e = MoveBack;
            if (e != null)
                e(null/*this*/, new EventArgs());
        }

        public event EventHandler MoveForward;
        void OnMoveForward()
        {
            var e = MoveForward;
            if (e != null)
                e(null/*this*/, new EventArgs());
        }

        public event EventHandler Home;
        void OnHome()
        {
            var e = Home;
            if (e != null)
                e(null/*this*/, new EventArgs());
        }

        public event EventHandler ZoomIn;
        void OnZoomIn()
        {
            var e = ZoomIn;
            if (e != null)
                e(null/*this*/, new EventArgs());
        }

        public event EventHandler ZoomOut;
        void OnZoomOut()
        {
            var e = ZoomOut;
            if (e != null)
                e(null/*this*/, new EventArgs());
        }

        public event EventHandler SwipeUp;
        void OnSwipeUp()
        {
            var e = SwipeUp;
            if (e != null)
                e(null/*this*/, new EventArgs());
        }

        public event EventHandler SwipeDown;
        void OnSwipeDown()
        {
            var e = SwipeDown;
            if (e != null)
                e(null/*this*/, new EventArgs());
        }

        public event EventHandler Reset;
        void OnReset()
        {
            var e = Reset;
            if (e != null)
                e(null/*this*/, new EventArgs());
        }

        public event EventHandler<SpeechCommandEventArgs> SpeechCommand;
        void OnSpeechCommand(String command)
        {
            var e = SpeechCommand;
            if (e != null)
                e(null/*this*/, new SpeechCommandEventArgs(command));
        }
        #endregion

        #region Declarations
        //private const float ClickThreshold = 0.33f;
        //private const float SkeletonMaxX = 0.60f;
        //private const float SkeletonMaxY = 0.40f;

        //private const int touchDeviceRight = 2;
        //private const int touchDeviceLeft = 3;

        //bool LeftHand;

        bool bLoaded;
        private const String nextCommand = "next";
        private const String previousCommand = "back";

        private Skeleton[] skeletons = new Skeleton[0];

        // skeleton gesture recognizer
        private GestureController gestureController;
        private MouseMovementDetector movementDetector;
        private KinectSensor currentSensor;

        #endregion

        #region calibration
        private CalibrateState calibrationState = CalibrateState.Idle;

        // Track skeleton joints for these angles
        // NOTE: Do not put too more angles or Kinect won't like it and will throw exceptions.
        private readonly int[] angles = new int[] { -20, -13, -6, 0, 6, 13, 20 };
        private int curAngleIndex;
        private int maxNumTracked;
        private int bestAngle;

        private const long WaitTime = 4000;
        private const long WaitTimeGoingUp = 1300;
        private long curTime;
        private Stopwatch watch;

        private enum CalibrateState
        {
            Idle,
            GoingDown,
            GoingUp,
            GoingBest
        }

        public bool IsCalibrating
        {
            get { return calibrationState != CalibrateState.Idle; }
        }

        #endregion

        #region DP

        public static readonly DependencyProperty KinectSensorManagerProperty =
            DependencyProperty.Register(
                "KinectSensorManager",
                typeof(KinectSensorManager),
                typeof(KinectWindow),
                new PropertyMetadata(null));

        public KinectSensorManager KinectSensorManager
        {
            get { return (KinectSensorManager)GetValue(KinectSensorManagerProperty); }
            set { SetValue(KinectSensorManagerProperty, value); }
        }

        public static readonly DependencyProperty EnableSpeechRecognitionProperty = DependencyProperty.Register("EnableSpeechRecognition", typeof(bool), typeof(KinectWindow), new UIPropertyMetadata(false, new PropertyChangedCallback(OnEnableSpeechRecognitionChanged), new CoerceValueCallback(OnCoerceEnableSpeechRecognition)));

        private static object OnCoerceEnableSpeechRecognition(DependencyObject o, object value)
        {
            KinectWindow kinectWindow = o as KinectWindow;
            if (kinectWindow != null)
                return kinectWindow.OnCoerceEnableSpeechRecognition((bool)value);
            else
                return value;
        }

        private static void OnEnableSpeechRecognitionChanged(DependencyObject o, DependencyPropertyChangedEventArgs e)
        {
            KinectWindow kinectWindow = o as KinectWindow;
            if (kinectWindow != null)
                kinectWindow.OnEnableSpeechRecognitionChanged((bool)e.OldValue, (bool)e.NewValue);
        }

        protected virtual bool OnCoerceEnableSpeechRecognition(bool value)
        {
            // TODO: Keep the proposed value within the desired range.
            return value;
        }

        protected virtual void OnEnableSpeechRecognitionChanged(bool oldValue, bool newValue)
        {
            // TODO: Add your property changed side-effects. Descendants can override as well.
        }

        public bool EnableSpeechRecognition
        {
            // IMPORTANT: To maintain parity between setting a property in XAML and procedural code, do not touch the getter and setter inside this dependency property!
            get
            {
                return (bool)GetValue(EnableSpeechRecognitionProperty);
            }
            set
            {
                SetValue(EnableSpeechRecognitionProperty, value);
            }
        }

        public static readonly DependencyProperty SpeechConfidenceLevelProperty = DependencyProperty.Register("SpeechConfidenceLevel", typeof(double), typeof(KinectWindow), new UIPropertyMetadata(0.7, new PropertyChangedCallback(OnSpeechConfidenceLevelChanged), new CoerceValueCallback(OnCoerceSpeechConfidenceLevel)));

        private static object OnCoerceSpeechConfidenceLevel(DependencyObject o, object value)
        {
            KinectWindow kinectWindow = o as KinectWindow;
            if (kinectWindow != null)
                return kinectWindow.OnCoerceSpeechConfidenceLevel((double)value);
            else
                return value;
        }

        private static void OnSpeechConfidenceLevelChanged(DependencyObject o, DependencyPropertyChangedEventArgs e)
        {
            KinectWindow kinectWindow = o as KinectWindow;
            if (kinectWindow != null)
                kinectWindow.OnSpeechConfidenceLevelChanged((double)e.OldValue, (double)e.NewValue);
        }

        protected virtual double OnCoerceSpeechConfidenceLevel(double value)
        {
            // TODO: Keep the proposed value within the desired range.
            return value;
        }

        protected virtual void OnSpeechConfidenceLevelChanged(double oldValue, double newValue)
        {
            // TODO: Add your property changed side-effects. Descendants can override as well.
        }

        public double SpeechConfidenceLevel
        {
            // IMPORTANT: To maintain parity between setting a property in XAML and procedural code, do not touch the getter and setter inside this dependency property!
            get
            {
                return (double)GetValue(SpeechConfidenceLevelProperty);
            }
            set
            {
                SetValue(SpeechConfidenceLevelProperty, value);
            }
        }

        public static readonly DependencyProperty ShowSpeechFailedProperty = DependencyProperty.Register("ShowSpeechFailed", typeof(bool), typeof(KinectWindow), new UIPropertyMetadata(false, new PropertyChangedCallback(OnShowSpeechFailedChanged), new CoerceValueCallback(OnCoerceShowSpeechFailed)));

        private static object OnCoerceShowSpeechFailed(DependencyObject o, object value)
        {
            KinectWindow kinectWindow = o as KinectWindow;
            if (kinectWindow != null)
                return kinectWindow.OnCoerceShowSpeechFailed((bool)value);
            else
                return value;
        }

        private static void OnShowSpeechFailedChanged(DependencyObject o, DependencyPropertyChangedEventArgs e)
        {
            KinectWindow kinectWindow = o as KinectWindow;
            if (kinectWindow != null)
                kinectWindow.OnShowSpeechFailedChanged((bool)e.OldValue, (bool)e.NewValue);
        }

        protected virtual bool OnCoerceShowSpeechFailed(bool value)
        {
            // TODO: Keep the proposed value within the desired range.
            return value;
        }

        protected virtual void OnShowSpeechFailedChanged(bool oldValue, bool newValue)
        {
            // TODO: Add your property changed side-effects. Descendants can override as well.
        }

        public bool ShowSpeechFailed
        {
            // IMPORTANT: To maintain parity between setting a property in XAML and procedural code, do not touch the getter and setter inside this dependency property!
            get
            {
                return (bool)GetValue(ShowSpeechFailedProperty);
            }
            set
            {
                SetValue(ShowSpeechFailedProperty, value);
            }
        }

        public static readonly DependencyProperty ShowSpeechRecognizedProperty = DependencyProperty.Register("ShowSpeechRecognized", typeof(bool), typeof(KinectWindow), new UIPropertyMetadata(true, new PropertyChangedCallback(OnShowSpeechRecognizedChanged), new CoerceValueCallback(OnCoerceShowSpeechRecognized)));

        private static object OnCoerceShowSpeechRecognized(DependencyObject o, object value)
        {
            KinectWindow kinectWindow = o as KinectWindow;
            if (kinectWindow != null)
                return kinectWindow.OnCoerceShowSpeechRecognized((bool)value);
            else
                return value;
        }

        private static void OnShowSpeechRecognizedChanged(DependencyObject o, DependencyPropertyChangedEventArgs e)
        {
            KinectWindow kinectWindow = o as KinectWindow;
            if (kinectWindow != null)
                kinectWindow.OnShowSpeechRecognizedChanged((bool)e.OldValue, (bool)e.NewValue);
        }

        protected virtual bool OnCoerceShowSpeechRecognized(bool value)
        {
            // TODO: Keep the proposed value within the desired range.
            return value;
        }

        protected virtual void OnShowSpeechRecognizedChanged(bool oldValue, bool newValue)
        {
            // TODO: Add your property changed side-effects. Descendants can override as well.
        }

        public bool ShowSpeechRecognized
        {
            // IMPORTANT: To maintain parity between setting a property in XAML and procedural code, do not touch the getter and setter inside this dependency property!
            get
            {
                return (bool)GetValue(ShowSpeechRecognizedProperty);
            }
            set
            {
                SetValue(ShowSpeechRecognizedProperty, value);
            }
        }

        public static readonly DependencyProperty ShowGesturesRecognizedProperty = DependencyProperty.Register("ShowGesturesRecognized", typeof(bool), typeof(KinectWindow), new UIPropertyMetadata(true, new PropertyChangedCallback(OnShowGesturesRecognizedChanged), new CoerceValueCallback(OnCoerceShowGesturesRecognized)));

        private static object OnCoerceShowGesturesRecognized(DependencyObject o, object value)
        {
            KinectWindow kinectWindow = o as KinectWindow;
            if (kinectWindow != null)
                return kinectWindow.OnCoerceShowGesturesRecognized((bool)value);
            else
                return value;
        }

        private static void OnShowGesturesRecognizedChanged(DependencyObject o, DependencyPropertyChangedEventArgs e)
        {
            KinectWindow kinectWindow = o as KinectWindow;
            if (kinectWindow != null)
                kinectWindow.OnShowGesturesRecognizedChanged((bool)e.OldValue, (bool)e.NewValue);
        }

        protected virtual bool OnCoerceShowGesturesRecognized(bool value)
        {
            // TODO: Keep the proposed value within the desired range.
            return value;
        }

        protected virtual void OnShowGesturesRecognizedChanged(bool oldValue, bool newValue)
        {
            // TODO: Add your property changed side-effects. Descendants can override as well.
        }

        public bool ShowGesturesRecognized
        {
            // IMPORTANT: To maintain parity between setting a property in XAML and procedural code, do not touch the getter and setter inside this dependency property!
            get
            {
                return (bool)GetValue(ShowGesturesRecognizedProperty);
            }
            set
            {
                SetValue(ShowGesturesRecognizedProperty, value);
            }
        }
        
        #endregion

        public KinectWindow()
        {
            InitializeComponent();

            Loaded += (o, e) =>
                {
                    if (!bLoaded)
                    {
                        bLoaded = true;

                        try
                        {
                            movementDetector = new MouseMovementDetector(this);
                            movementDetector.IsMovingChanged += OnIsMouseMovingChanged;
                            movementDetector.Start();

                            KinectSensorManager = new KinectSensorManager();
                            KinectSensorManager.KinectSensorChanged += KinectSensorManager_KinectSensorChanged;
                            kinectSensorChooser.KinectSensorChooser = new Microsoft.Kinect.Toolkit.KinectSensorChooser();
                            kinectSensorChooser.KinectSensorChooser.Start();

                            // bind chooser's sensor value to the local sensor manager
                            var kinectSensorBinding = new Binding("Kinect") { Source = kinectSensorChooser.KinectSensorChooser };
                            BindingOperations.SetBinding(this.KinectSensorManager, KinectSensorManager.KinectSensorProperty, kinectSensorBinding);

                            DepthViewer.KinectSensorManager = KinectSensorManager;
                            SkeletonViewer.KinectSensorManager = KinectSensorManager;

                            kinectRegion.QueryPrimaryUserTrackingIdCallback = QueryPrimaryUserTrackingIdCallback;
                        }
                        catch(Exception ex)
                        {
                            MessageBox.Show(String.Format(Properties.Resources.ErrorInitializingKinect, ex.Message));
                        }
                    }
                };
        }

        public int QueryPrimaryUserTrackingIdCallback (
                 int proposedTrackingId,
                 IEnumerable<HandPointer> candidateHandPointers,
                 long timestamp)
        {
            if (CurrentTrackingId == -1)
                return proposedTrackingId;

            foreach (var handpointer in candidateHandPointers)
            {
                if (handpointer.TrackingId == CurrentTrackingId)
                    return handpointer.TrackingId;
            }

            return proposedTrackingId;
        }

        private void StopKinect(KinectSensor sensor)
        {
            if (sensor != null)
            {
                try
                {
                    sensor.DepthStream.Range = DepthRange.Default;
                    sensor.SkeletonStream.EnableTrackingInNearRange = false;
                    sensor.DepthStream.Disable();
                    sensor.SkeletonStream.Disable();
                }
                catch (InvalidOperationException)
                {
                    // KinectSensor might enter an invalid state while enabling/disabling streams or stream features.
                    // E.g.: sensor might be abruptly unplugged.
                }

                sensor.SkeletonFrameReady -= OnSkeletonFrameReady;
                gestureController.GestureRecognized -= OnGestureRecognized;

                StopRecognition();
                if (sensor.IsRunning)
                {
                    sensor.Stop();
                    if (EnableSpeechRecognition)
                        sensor.AudioSource.Stop();
                }
                sensor.Dispose();
            }
        }

        void KinectSensorManager_KinectSensorChanged(object sender, KinectSensorManagerEventArgs<KinectSensor> e)
        {
            KinectSensor old = (KinectSensor)e.OldValue;

            StopKinect(old);
            currentSensor = null;

            KinectSensor sensor = (KinectSensor)e.NewValue;

            if (sensor == null)
            {
                return;
            }

            currentSensor = sensor;

            /*
            KinectSensorManager.ColorFormat = ColorImageFormat.RgbResolution640x480Fps30;
            KinectSensorManager.ColorStreamEnabled = true;

            // configure the depth stream
            KinectSensorManager.DepthStreamEnabled = true;

            KinectSensorManager.TransformSmoothParameters =
                new TransformSmoothParameters
                {
                    Smoothing = 0.5f,
                    Correction = 0.5f,
                    Prediction = 0.5f,
                    JitterRadius = 0.05f,
                    MaxDeviationRadius = 0.04f
                };
            */
            try
            {
                watch = new Stopwatch();
                watch.Start();
                curAngleIndex = 0;

                calibrationState = CalibrateState.GoingDown;
                bestAngle = sensor.ElevationAngle;
                try
                {
                    Thread.Sleep(1000);
                    sensor.ElevationAngle = sensor.MinElevationAngle;
                }
                catch (Exception ex)
                {
                    
                }

                sensor.DepthStream.Enable(DepthImageFormat.Resolution640x480Fps30);
                sensor.SkeletonStream.Enable();

                try
                {
                    sensor.DepthStream.Range = DepthRange.Near;
                    sensor.SkeletonStream.EnableTrackingInNearRange = true;
                }
                catch (InvalidOperationException)
                {
                    // Non Kinect for Windows devices do not support Near mode, so reset back to default mode.
                    sensor.DepthStream.Range = DepthRange.Default;
                    sensor.SkeletonStream.EnableTrackingInNearRange = false;
                }
            }
            catch (InvalidOperationException)
            {
                // KinectSensor might enter an invalid state while enabling/disabling streams or stream features.
                // E.g.: sensor might be abruptly unplugged.
            }            

            // configure the skeleton stream
            sensor.SkeletonFrameReady += OnSkeletonFrameReady;
            // KinectSensorManager.SkeletonStreamEnabled = true;

            // KinectSensorManager.KinectSensorEnabled = true;

            if (!KinectSensorManager.KinectSensorAppConflict)
            {
                // initialize the gesture recognizer
                gestureController = new GestureController();
                gestureController.GestureRecognized += OnGestureRecognized;

                // register the gestures for this demo
                RegisterGestures();
            }

            kinectRegion.KinectSensor = sensor;
            if (EnableSpeechRecognition)
            {
                sensor.AudioSource.Start();
                StartRecognition(sensor);
            }

            OnReadyForContent();
            txtSearching.Visibility = Visibility.Collapsed;

            kinectSensorChooser.Visibility = Visibility.Collapsed;
            kinectAudioViewer.Visibility = Visibility.Visible;
            video.Visibility = Visibility.Visible;
        }

        private void OnWindowClosing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            try
            {
                kinectAudioViewer.KinectSensorManager = null;
                if (kinectSensorChooser.KinectSensorChooser != null)
                {
                    StopKinect(kinectSensorChooser.KinectSensorChooser.Kinect);
                    kinectSensorChooser.KinectSensorChooser.Start();
                }
                currentSensor = null;
            }
            catch
            {

            }

            StopRecognition();
        }

        #region Gestures

        /// <summary>
        /// Helper function to register all available 
        /// </summary>
        private void RegisterGestures()
        {
            // define the gestures for the demo

            IRelativeGestureSegment[] joinedhandsSegments = new IRelativeGestureSegment[20];
            JoinedHandsSegment1 joinedhandsSegment = new JoinedHandsSegment1();
            for (int i = 0; i < 20; i++)
            {
                // gesture consists of the same thing 10 times 
                joinedhandsSegments[i] = joinedhandsSegment;
            }
            gestureController.AddGesture("JoinedHands", joinedhandsSegments);

            IRelativeGestureSegment[] menuSegments = new IRelativeGestureSegment[20];
            MenuSegment1 menuSegment = new MenuSegment1();
            for (int i = 0; i < 20; i++)
            {
                // gesture consists of the same thing 20 times 
                menuSegments[i] = menuSegment;
            }
            gestureController.AddGesture("Menu", menuSegments);

            IRelativeGestureSegment[] swipeleftSegments = new IRelativeGestureSegment[3];
            swipeleftSegments[0] = new SwipeLeftSegment1();
            swipeleftSegments[1] = new SwipeLeftSegment2();
            swipeleftSegments[2] = new SwipeLeftSegment3();
            gestureController.AddGesture("SwipeLeft", swipeleftSegments);

            IRelativeGestureSegment[] swiperightSegments = new IRelativeGestureSegment[3];
            swiperightSegments[0] = new SwipeRightSegment1();
            swiperightSegments[1] = new SwipeRightSegment2();
            swiperightSegments[2] = new SwipeRightSegment3();
            gestureController.AddGesture("SwipeRight", swiperightSegments);

            IRelativeGestureSegment[] waveRightSegments = new IRelativeGestureSegment[6];
            WaveRightSegment1 waveRightSegment1 = new WaveRightSegment1();
            WaveRightSegment2 waveRightSegment2 = new WaveRightSegment2();
            waveRightSegments[0] = waveRightSegment1;
            waveRightSegments[1] = waveRightSegment2;
            waveRightSegments[2] = waveRightSegment1;
            waveRightSegments[3] = waveRightSegment2;
            waveRightSegments[4] = waveRightSegment1;
            waveRightSegments[5] = waveRightSegment2;
            gestureController.AddGesture("WaveRight", waveRightSegments);

            IRelativeGestureSegment[] waveLeftSegments = new IRelativeGestureSegment[6];
            WaveLeftSegment1 waveLeftSegment1 = new WaveLeftSegment1();
            WaveLeftSegment2 waveLeftSegment2 = new WaveLeftSegment2();
            waveLeftSegments[0] = waveLeftSegment1;
            waveLeftSegments[1] = waveLeftSegment2;
            waveLeftSegments[2] = waveLeftSegment1;
            waveLeftSegments[3] = waveLeftSegment2;
            waveLeftSegments[4] = waveLeftSegment1;
            waveLeftSegments[5] = waveLeftSegment2;
            gestureController.AddGesture("WaveLeft", waveLeftSegments);

            IRelativeGestureSegment[] zoomInSegments = new IRelativeGestureSegment[3];
            zoomInSegments[0] = new ZoomSegment1();
            zoomInSegments[1] = new ZoomSegment2();
            zoomInSegments[2] = new ZoomSegment3();
            gestureController.AddGesture("ZoomIn", zoomInSegments);

            IRelativeGestureSegment[] zoomOutSegments = new IRelativeGestureSegment[3];
            zoomOutSegments[0] = new ZoomSegment3();
            zoomOutSegments[1] = new ZoomSegment2();
            zoomOutSegments[2] = new ZoomSegment1();
            gestureController.AddGesture("ZoomOut", zoomOutSegments);

            IRelativeGestureSegment[] swipeUpSegments = new IRelativeGestureSegment[3];
            swipeUpSegments[0] = new SwipeUpSegment1();
            swipeUpSegments[1] = new SwipeUpSegment2();
            swipeUpSegments[2] = new SwipeUpSegment3();
            gestureController.AddGesture("SwipeUp", swipeUpSegments);

            IRelativeGestureSegment[] swipeDownSegments = new IRelativeGestureSegment[3];
            swipeDownSegments[0] = new SwipeDownSegment1();
            swipeDownSegments[1] = new SwipeDownSegment2();
            swipeDownSegments[2] = new SwipeDownSegment3();
            gestureController.AddGesture("SwipeDown", swipeDownSegments);
        }

        #endregion

        #region Event Handlers

        /// <summary>
        ///
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e">Gesture event arguments.</param>
        private void OnGestureRecognized(object sender, GestureEventArgs e)
        {
            if (ShowGesturesRecognized)
                SetSpeechText(e.GestureName);

            switch (e.GestureName)
            {
                case "Menu":
                    OnHome();
                    break;
                case "WaveRight":
                    break;
                case "WaveLeft":
                    kinectRegion.IsCursorVisible = !kinectRegion.IsCursorVisible;
                    break;
                case "JoinedHands":
                    OnReset();
                    break;
                case "SwipeLeft":
                    OnMoveForward();
                    break;
                case "SwipeRight":
                    OnMoveBack();
                    break;
                case "SwipeUp":
                    OnSwipeUp();
                    break;
                case "SwipeDown":
                    OnSwipeDown();
                    break;
                case "ZoomIn":
                    OnZoomIn();
                    break;
                case "ZoomOut":
                    OnZoomOut();
                    break;

                default:
                    break;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>

        int CurrentTrackingId = -1;
        private void OnSkeletonFrameReady(object sender, SkeletonFrameReadyEventArgs e)
        {
            if (movementDetector.IsMoving)
            {
                CurrentTrackingId = -1;
                return;
            }

            try
            {
                using (SkeletonFrame frame = e.OpenSkeletonFrame())
                {
                    if (frame == null)
                    {
                        CurrentTrackingId = -1;
                        return;
                    }
                    // resize the skeletons array if needed
                    if (skeletons.Length != frame.SkeletonArrayLength)
                        skeletons = new Skeleton[frame.SkeletonArrayLength];

                    // get the skeleton data
                    frame.CopySkeletonDataTo(skeletons);

                    bool bSomeoneIsTracked = false;
                    foreach (var skeleton in skeletons)
                    {
                        // skip the skeleton if it is not being tracked
                        if (skeleton.TrackingState != SkeletonTrackingState.Tracked)
                            continue;
                        if (CurrentTrackingId != -1 && skeleton.TrackingId != CurrentTrackingId)
                            continue;
                        bSomeoneIsTracked = true;
                        CurrentTrackingId = skeleton.TrackingId;
                        // update the gesture controller
                        gestureController.UpdateAllGestures(skeleton);
                    }

                    if (!bSomeoneIsTracked)
                    {
                        CurrentTrackingId = -1;
                        calibrationState = CalibrateState.GoingUp;
                    }
                }

                if (CurrentTrackingId != -1 || currentSensor == null)
                    return;

                curTime = watch.ElapsedMilliseconds;

                if (calibrationState == CalibrateState.GoingDown)
                {
                    // If the sensor reach its lowest angle, let's do a full scan from min angle to max angle.
                    if (curTime >= WaitTime)
                    {
                        watch.Reset();
                        watch.Start();

                        bestAngle = currentSensor.MinElevationAngle;
                        maxNumTracked = 0;

                        calibrationState = CalibrateState.GoingUp;
                        currentSensor.ElevationAngle = angles[curAngleIndex++];
                    }
                }
                else if (calibrationState == CalibrateState.GoingUp)
                {
                    if (curTime >= WaitTimeGoingUp)
                    {
                        watch.Reset();
                        watch.Start();

                        // If we scanned all the angles, lets adjust kinect to the best angle.
                        if (curAngleIndex > angles.Length - 1)
                        {
                            calibrationState = CalibrateState.GoingDown;
                            currentSensor.ElevationAngle = bestAngle;
                            return;
                        }

                        currentSensor.ElevationAngle = angles[curAngleIndex++];
                    }
                }
            }
            catch (Exception ex)
            {
                
            }
        }

        #endregion Event Handlers

        public void SetContent(UIElement content)
        {
            kinectRegion.Content = content;
            // contentControl.Content = content;
        }

        #region Speech Recognition

        RecognizerInfo ri;
        SpeechRecognitionEngine sre;
        Stream speechStream;
        Grammar currentGrammar;
        IDisposable observableTimer;

        void SetSpeechText(String text)
        {
            Dispatcher.InvokeIfRequired(() =>
                {
                    lableSpeech.Content = text;
                    var storyboard = Resources["SpeechAnimation"] as Storyboard;
                    storyboard.Begin();
                });
        }

        void StopRecognition()
        {
            try
            {
                if (pendingGrammarOperation != null)
                {
                    pendingGrammarOperation.Abort();
                    pendingGrammarOperation = null;
                }

                if (speechStream != null)
                {
                    speechStream.Dispose();
                    speechStream = null;
                }
                if (sre != null)
                {
                    sre.RecognizeAsyncStop();
                    sre.Dispose();
                    sre = null;
                }
            }
            catch
            {

            }
        }

        List<String> pendingList;
        DispatcherOperation pendingGrammarOperation;

        public void LoadDynamicGrammar(List<String> list)
        {
            if (sre == null)
            {
                pendingList = list;
                return;
            }

            if (pendingGrammarOperation != null)
                pendingGrammarOperation.Abort();

            pendingGrammarOperation = Dispatcher.BeginInvokeAsynchronouslyInBackground(() =>
            {
                var pendingTask = Task.Factory.StartNew(() =>
                {
                    if (sre == null)
                        return;

                    SetSpeechText("Unloading Grammar...");

                    try
                    {
                        if (currentGrammar != null)
                            sre.UnloadGrammar(currentGrammar);
                    }
                    catch (Exception ex)
                    {

                    }
                    if (list == null || list.Count == 0)
                        return;
                    SetSpeechText("Preparing Grammar...");
                    currentGrammar = LoadGrammar(list);
                    SetSpeechText("Grammar is ready");
                });
            });
            if (pendingGrammarOperation != null)
            {
                if (pendingGrammarOperation.Status != DispatcherOperationStatus.Completed)
                    pendingGrammarOperation.Completed += (o, e) => { pendingGrammarOperation = null; };
                else
                    pendingGrammarOperation = null;
            }
        }

        Grammar LoadGrammar(List<String> list)
        {
            if (list == null)
                return null;

            var commands = new Choices();
            list.ForEach(s => commands.Add(s));

            var gb = new GrammarBuilder { Culture = ri.Culture };

            // Specify the culture to match the recognizer in case we are running in a different culture.                                 
            gb.Append(commands);

            // Create the actual Grammar instance, and then load it into the speech recognizer.
            var grammar = new Grammar(gb);

            sre.LoadGrammar(grammar);
            return grammar;
        }

        void StartRecognition(KinectSensor sensor)
        {
            if (!EnableSpeechRecognition)
                return;

            // Obtain the KinectAudioSource to do audio capture
            KinectAudioSource source = sensor.AudioSource;
            source.EchoCancellationMode = EchoCancellationMode.None; // No AEC for this sample
            source.AutomaticGainControlEnabled = false; // Important to turn this off for speech recognition

            ri = GetKinectRecognizer();

            if (ri == null)
            {
                SetSpeechText("Could not find Kinect speech recognizer");
                return;
            }

            SetSpeechText(String.Format("Using: {0}", ri.Name));

            int wait = 4;
            if (observableTimer != null)
                observableTimer.Dispose();
            observableTimer = System.Linq.Observable.Interval(TimeSpan.FromMilliseconds(1000)).Subscribe((l) =>
            {
                if (--wait > 0)
                {
                    SetSpeechText(String.Format("Device will be ready for speech recognition in {0} second(s)", wait));
                }
                else
                {
                    observableTimer.Dispose();
                    sre = new SpeechRecognitionEngine(ri.Id);

                    var list = new List<String>();
                    list.Add(nextCommand);
                    list.Add(previousCommand);
                    LoadGrammar(list);

                    sre.SpeechRecognized += SreSpeechRecognized;
                    sre.SpeechHypothesized += SreSpeechHypothesized;
                    sre.SpeechRecognitionRejected += SreSpeechRecognitionRejected;

                    speechStream = source.Start();
                            
                    sre.SetInputToAudioStream(
                        speechStream, new SpeechAudioFormatInfo(EncodingFormat.Pcm, 16000, 16, 1, 32000, 2, null));

                    // Console.WriteLine("Recognizing speech. Say: 'red', 'green' or 'blue'. Press ENTER to stop");

                    if (pendingList != null)
                    {
                        LoadDynamicGrammar(pendingList);
                        pendingList = null;
                    }
                    sre.RecognizeAsync(RecognizeMode.Multiple);
                }
            });
        }

        private static RecognizerInfo GetKinectRecognizer()
        {
            Func<RecognizerInfo, bool> matchingFunc = r =>
            {
                string value;
                r.AdditionalInfo.TryGetValue("Kinect", out value);
                return "True".Equals(value, StringComparison.InvariantCultureIgnoreCase) && "en-US".Equals(r.Culture.Name, StringComparison.InvariantCultureIgnoreCase);
            };
            return SpeechRecognitionEngine.InstalledRecognizers().Where(matchingFunc).FirstOrDefault();
        }

        private void SreSpeechRecognitionRejected(object sender, SpeechRecognitionRejectedEventArgs e)
        {
            /*
            Console.WriteLine("\nSpeech Rejected");
            if (e.Result != null)
            {
                DumpRecordedAudio(e.Result.Audio);
            }
            */
        }

        private void SreSpeechHypothesized(object sender, SpeechHypothesizedEventArgs e)
        {
            // Console.Write("\rSpeech Hypothesized: \t{0}", e.Result.Text);
        }

        private void SreSpeechRecognized(object sender, SpeechRecognizedEventArgs e)
        {
            Dispatcher.InvokeIfRequired(() =>
                {
                    if (e.Result.Confidence >= SpeechConfidenceLevel)
                    {
                        if (ShowSpeechRecognized)
                            SetSpeechText(String.Format("{0}\t\t(Confidence:\t{1})", e.Result.Text, e.Result.Confidence));

                        Dispatcher.InvokeIfRequired(() =>
                        {
                            if (e.Result.Text == nextCommand)
                                OnMoveForward();
                            else if (e.Result.Text == previousCommand)
                                OnMoveBack();
                            else
                                OnSpeechCommand(e.Result.Text);
                        });
                        // Console.WriteLine("\nSpeech Recognized: \t{0}\tConfidence:\t{1}", e.Result.Text, e.Result.Confidence);
                    }
                    else
                    {
                        if (ShowSpeechFailed)
                            SetSpeechText(String.Format("Speech Recognized but confidence was too low: \t{0}", e.Result.Confidence));
                        // Console.WriteLine("\nSpeech Recognized but confidence was too low: \t{0}", e.Result.Confidence);
                        // DumpRecordedAudio(e.Result.Audio);
                    }
                });
        }

        private static void DumpRecordedAudio(RecognizedAudio audio)
        {
            if (audio == null)
            {
                return;
            }

            int fileId = 0;
            string filename;
            while (File.Exists((filename = "RetainedAudio_" + fileId + ".wav")))
            {
                fileId++;
            }

            Console.WriteLine("\nWriting file: {0}", filename);
            using (var file = new FileStream(filename, System.IO.FileMode.CreateNew))
            {
                audio.WriteToWaveStream(file);
            }
        }

        private void OnIsMouseMovingChanged(object sender, EventArgs e)
        {
            WindowBezelHelper.UpdateBezel(this, movementDetector.IsMoving);
            if (movementDetector.IsMoving)
            {
                if (kinectRegion.IsCursorVisible)
                    kinectRegion.IsCursorVisible = false;
                DepthViewer.Visibility = Visibility.Collapsed;
                SkeletonViewer.Visibility = Visibility.Collapsed;

                // StopRecognition();
            }
            else
            {
                DepthViewer.Visibility = Visibility.Visible;
                SkeletonViewer.Visibility = Visibility.Visible;

                //if (currentSensor != null)
                //{
                //    currentSensor.AudioSource.Start();
                //    StartRecognition(currentSensor);
                //}
            }
        }

        #endregion

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (disposing)
            {
                StopRecognition();
            }
        }

        ~KinectWindow()
        {
            Dispose(false);
        }
    }
}
