using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media.Imaging;
using System.Windows.Threading;
using CommonControls.PropertyDataTemplate;
using IpCameraControl.PropertyDataTemplate;
using log4net;
using MjpegProcessor;
using UFInterfaces.PropertyControl;
using Utilities;
using WPFUtilities;
using UFProjectManager.ComponentService;
using DocumentManager.ComponentService;


namespace IpCameraControl
{
    /// <summary>
    /// Interaction logic for IpCameraControl.xaml
    /// </summary>
    public partial class IpCameraControl : UserControl, IContainPropertyEditors, IDataErrorInfo, IDisposable
    {
        #region DP

        #region KeepAspectRatio
        public static readonly DependencyProperty KeepAspectRatioProperty = DependencyProperty.Register("KeepAspectRatio", typeof(bool), typeof(IpCameraControl), new UIPropertyMetadata(false, new PropertyChangedCallback(OnKeepAspectRatioChanged), new CoerceValueCallback(OnCoerceKeepAspectRatio)));

        private static object OnCoerceKeepAspectRatio(DependencyObject o, object value)
        {
            IpCameraControl IpCameraControl = o as IpCameraControl;
            if (IpCameraControl != null)
                return IpCameraControl.OnCoerceKeepAspectRatio((bool)value);
            else
                return value;
        }

        private static void OnKeepAspectRatioChanged(DependencyObject o, DependencyPropertyChangedEventArgs e)
        {
            IpCameraControl IpCameraControl = o as IpCameraControl;
            if (IpCameraControl != null)
                IpCameraControl.OnKeepAspectRatioChanged((bool)e.OldValue, (bool)e.NewValue);
        }

        protected virtual bool OnCoerceKeepAspectRatio(bool value)
        {
            // TODO: Keep the proposed value within the desired range.
            return value;
        }

        protected virtual void OnKeepAspectRatioChanged(bool oldValue, bool newValue)
        {
            // TODO: Add your property changed side-effects. Descendants can override as well.
        }

        public bool KeepAspectRatio
        {
            // IMPORTANT: To maintain parity between setting a property in XAML and procedural code, do not touch the getter and setter inside this dependency property!
            get
            {
                return (bool)GetValue(KeepAspectRatioProperty);
            }
            set
            {
                SetValue(KeepAspectRatioProperty, value);
            }
        }
        #endregion

        public static readonly DependencyProperty UriProperty = DependencyProperty.Register("Uri", typeof(Uri), typeof(IpCameraControl), new UIPropertyMetadata(null, new PropertyChangedCallback(OnUriChanged), new CoerceValueCallback(OnCoerceUri)));

        private static object OnCoerceUri(DependencyObject o, object value)
        {
            IpCameraControl ipCameraControl = o as IpCameraControl;
            if (ipCameraControl != null)
                return ipCameraControl.OnCoerceUri((Uri)value);
            else
                return value;
        }

        private static void OnUriChanged(DependencyObject o, DependencyPropertyChangedEventArgs e)
        {
            IpCameraControl ipCameraControl = o as IpCameraControl;
            if (ipCameraControl != null)
                ipCameraControl.OnUriChanged((Uri)e.OldValue, (Uri)e.NewValue);
        }

        protected virtual Uri OnCoerceUri(Uri value)
        {
            // TODO: Keep the proposed value within the desired range.
            return value;
        }

        protected virtual void OnUriChanged(Uri oldValue, Uri newValue)
        {
            if (oldValue != newValue)
                CheckAndStart();
        }

        public Uri Uri
        {
            // IMPORTANT: To maintain parity between setting a property in XAML and procedural code, do not touch the getter and setter inside this dependency property!
            get
            {
                return (Uri)GetValue(UriProperty);
            }
            set
            {
                SetValue(UriProperty, value);
            }
        }

        public static readonly DependencyProperty UserProperty = DependencyProperty.Register("User", typeof(String), typeof(IpCameraControl), new UIPropertyMetadata(null, new PropertyChangedCallback(OnUserChanged), new CoerceValueCallback(OnCoerceUser)));

        private static object OnCoerceUser(DependencyObject o, object value)
        {
            IpCameraControl ipCameraControl = o as IpCameraControl;
            if (ipCameraControl != null)
                return ipCameraControl.OnCoerceUser((String)value);
            else
                return value;
        }

        private static void OnUserChanged(DependencyObject o, DependencyPropertyChangedEventArgs e)
        {
            IpCameraControl ipCameraControl = o as IpCameraControl;
            if (ipCameraControl != null)
                ipCameraControl.OnUserChanged((String)e.OldValue, (String)e.NewValue);
        }

        protected virtual String OnCoerceUser(String value)
        {
            // TODO: Keep the proposed value within the desired range.
            return value;
        }

        protected virtual void OnUserChanged(String oldValue, String newValue)
        {
            if (oldValue != newValue)
                CheckAndStart();
        }

        public String User
        {
            // IMPORTANT: To maintain parity between setting a property in XAML and procedural code, do not touch the getter and setter inside this dependency property!
            get
            {
                return (String)GetValue(UserProperty);
            }
            set
            {
                SetValue(UserProperty, value);
            }
        }

        public static readonly DependencyProperty PasswordProperty = DependencyProperty.Register("Password", typeof(String), typeof(IpCameraControl), new UIPropertyMetadata(null, new PropertyChangedCallback(OnPasswordChanged), new CoerceValueCallback(OnCoercePassword)));

        private static object OnCoercePassword(DependencyObject o, object value)
        {
            IpCameraControl ipCameraControl = o as IpCameraControl;
            if (ipCameraControl != null)
                return ipCameraControl.OnCoercePassword((String)value);
            else
                return value;
        }

        private static void OnPasswordChanged(DependencyObject o, DependencyPropertyChangedEventArgs e)
        {
            IpCameraControl ipCameraControl = o as IpCameraControl;
            if (ipCameraControl != null)
                ipCameraControl.OnPasswordChanged((String)e.OldValue, (String)e.NewValue);
        }

        protected virtual String OnCoercePassword(String value)
        {
            // TODO: Keep the proposed value within the desired range.
            return value;
        }

        protected virtual void OnPasswordChanged(String oldValue, String newValue)
        {
            if (oldValue != newValue)
                CheckAndStart();
        }

        public String Password
        {
            // IMPORTANT: To maintain parity between setting a property in XAML and procedural code, do not touch the getter and setter inside this dependency property!
            get
            {
                return (String)GetValue(PasswordProperty);
            }
            set
            {
                SetValue(PasswordProperty, value);
            }
        }

        #region CloseTimeout
        public static readonly DependencyProperty CloseTimeoutProperty = DependencyProperty.Register("CloseTimeout", typeof(double), typeof(IpCameraControl), new UIPropertyMetadata(1000d, new PropertyChangedCallback(OnCloseTimeoutChanged), new CoerceValueCallback(OnCoerceCloseTimeout)));

        private static object OnCoerceCloseTimeout(DependencyObject o, object value)
        {
            IpCameraControl IpCameraControl = o as IpCameraControl;
            if (IpCameraControl != null)
                return IpCameraControl.OnCoerceCloseTimeout((double)value);
            else
                return value;
        }

        private static void OnCloseTimeoutChanged(DependencyObject o, DependencyPropertyChangedEventArgs e)
        {
            IpCameraControl IpCameraControl = o as IpCameraControl;
            if (IpCameraControl != null)
                IpCameraControl.OnCloseTimeoutChanged((double)e.OldValue, (double)e.NewValue);
        }

        protected virtual double OnCoerceCloseTimeout(double value)
        {
            // TODO: Keep the proposed value within the desired range.
            return value;
        }

        protected virtual void OnCloseTimeoutChanged(double oldValue, double newValue)
        {
            // TODO: Add your property changed side-effects. Descendants can override as well.
        }

        public double CloseTimeout
        {
            // IMPORTANT: To maintain parity between setting a property in XAML and procedural code, do not touch the getter and setter inside this dependency property!
            get
            {
                return (double)GetValue(CloseTimeoutProperty);
            }
            set
            {
                SetValue(CloseTimeoutProperty, value);
            }
        }
        #endregion

        #region ReconnectInterval
        public static readonly DependencyProperty ReconnectIntervalProperty = DependencyProperty.Register("ReconnectInterval", typeof(double), typeof(IpCameraControl), new UIPropertyMetadata(0d, new PropertyChangedCallback(OnReconnectIntervalChanged), new CoerceValueCallback(OnCoerceReconnectInterval)));

        private static object OnCoerceReconnectInterval(DependencyObject o, object value)
        {
            IpCameraControl IpCameraControl = o as IpCameraControl;
            if (IpCameraControl != null)
                return IpCameraControl.OnCoerceReconnectInterval((double)value);
            else
                return value;
        }

        private static void OnReconnectIntervalChanged(DependencyObject o, DependencyPropertyChangedEventArgs e)
        {
            IpCameraControl IpCameraControl = o as IpCameraControl;
            if (IpCameraControl != null)
                IpCameraControl.OnReconnectIntervalChanged((double)e.OldValue, (double)e.NewValue);
        }

        protected virtual double OnCoerceReconnectInterval(double value)
        {
            // TODO: Keep the proposed value within the desired range.
            return value;
        }

        protected virtual void OnReconnectIntervalChanged(double oldValue, double newValue)
        {
            // TODO: Add your property changed side-effects. Descendants can override as well.
        }
        
        public double ReconnectInterval
        {
            // IMPORTANT: To maintain parity between setting a property in XAML and procedural code, do not touch the getter and setter inside this dependency property!
            get
            {
                return (double)GetValue(ReconnectIntervalProperty);
            }
            set
            {
                SetValue(ReconnectIntervalProperty, value);
            }
        }
        #endregion

        #region FrameRateUpdateImage
        public static readonly DependencyProperty FrameRateUpdateImageProperty = DependencyProperty.Register("FrameRateUpdateImage", typeof(int), typeof(IpCameraControl), new UIPropertyMetadata(100, new PropertyChangedCallback(OnFrameRateUpdateImageChanged), new CoerceValueCallback(OnCoerceFrameRateUpdateImage)));

        private static object OnCoerceFrameRateUpdateImage(DependencyObject o, object value)
        {
            IpCameraControl IpCameraControl = o as IpCameraControl;
            if (IpCameraControl != null)
                return IpCameraControl.OnCoerceFrameRateUpdateImage((int)value);
            else
                return value;
        }

        private static void OnFrameRateUpdateImageChanged(DependencyObject o, DependencyPropertyChangedEventArgs e)
        {
            IpCameraControl IpCameraControl = o as IpCameraControl;
            if (IpCameraControl != null)
                IpCameraControl.OnFrameRateUpdateImageChanged((int)e.OldValue, (int)e.NewValue);
        }

        protected virtual int OnCoerceFrameRateUpdateImage(int value)
        {
            // TODO: Keep the proposed value within the desired range.
            return value;
        }

        protected virtual void OnFrameRateUpdateImageChanged(int oldValue, int newValue)
        {
            // TODO: Add your property changed side-effects. Descendants can override as well.
            delayFrameReadyInvoker = null;
        }

        public int FrameRateUpdateImage
        {
            // IMPORTANT: To maintain parity between setting a property in XAML and procedural code, do not touch the getter and setter inside this dependency property!
            get
            {
                return (int)GetValue(FrameRateUpdateImageProperty);
            }
            set
            {
                SetValue(FrameRateUpdateImageProperty, value);
            }
        }
        #endregion
        #endregion

        #region Declarations
        IDocument Document;
        IUFProjectManager iUFProjectManager;
        MjpegDecoder _mjpeg;
        private static readonly ILog log = LogManager.GetLogger(Properties.Resources.IpCameraControl);
        DispatcherTimer reconnectionTimer;
        bool bLoaded;
        #endregion

        public IpCameraControl()
        {
            InitializeComponent();

            ScreenSettings.ScreenDocument.SetAutoForceDynamicOnClient(this, true);

            Loaded += (o, e) =>
            {
                if (Document == null)
                    Document = ScreenSettings.ScreenDocument.GetScreenDocument(this);
                if (Document != null)
                    iUFProjectManager = Document.GetService(typeof(IUFProjectManager)) as IUFProjectManager;
                if (DesignerProperties.GetIsInDesignMode(this))
                {
                    if (!bLoaded && !bDispose)
                    {
                        bLoaded = true;
                       
                        busyContent.Visibility = Visibility.Visible;
                        busyContent.Text = Properties.Resources.ContentAvailableInRuntime;
                    }
                    return;
                }
                else
                {
                    if (!bLoaded && !bDispose)
                    {
                        bLoaded = true;
                        Dispatcher.BeginInvokeAsynchronouslyInBackground(() =>
                        {
                            if (bDispose)
                                return;
                            view.Visibility = System.Windows.Visibility.Visible;
                        });

                        CheckAndStart();
                    }
                }
            };
            Unloaded += (o, e) =>
            {
                if (bLoaded)
                {
                    bLoaded = false;
                    Stop();
                }
            };
            
        }

        void CheckAndStart()
        {
            if (!bLoaded)
                return;

            if (DesignerProperties.GetIsInDesignMode(this))
            {
                busyContent.Visibility = Visibility.Visible;
                busyContent.Text = Properties.Resources.ContentAvailableInRuntime;
                return;
            }

            Stop();
            Start();
        }

        void Start()
        {
            if (Uri == null || !Uri.IsAbsoluteUri)
            {
                busyContent.Visibility = Visibility.Visible;
                busyContent.Text = Properties.Resources.UriNotSet;
                return;
            }

            if (_mjpeg != null)
            {
                _mjpeg.Error -= _mjpeg_Error;
                _mjpeg.FrameReady -= mjpeg_FrameReady;
                _mjpeg.Dispose();
                _mjpeg = null;
            }

            busyControl.Visibility = Visibility.Visible;
            busyContent.Visibility = Visibility.Visible;
            busyContent.Text = Properties.Resources.WaitText;


            _mjpeg = new MjpegDecoder(TimeSpan.FromMilliseconds(CloseTimeout)) { Dispatcher = Dispatcher };
            
            _mjpeg.FrameReady += mjpeg_FrameReady;
            _mjpeg.Error += _mjpeg_Error;
            _mjpeg.ParseStream(Uri, User, Password);
        }

        void _mjpeg_Error(object sender, System.IO.ErrorEventArgs e)
        {
            busyContent.Visibility = Visibility.Visible;
            busyContent.Text = String.Format(Properties.Resources.Error, e.GetException().Message);
            log.Error(Name, e.GetException());
            if (iUFProjectManager != null)
                iUFProjectManager.AddLogEntity(Document, Properties.Resources.IpCameraControl,
                DateTime.UtcNow, $"{Name}: {e.GetException().Message}",
                System.Diagnostics.EventLogEntryType.Error);
            if (ReconnectInterval > 0)
            {
                busyContent.Text = String.Format("{0}\n{1}", busyContent.Text, String.Format(Properties.Resources.ReconnectingIn, ReconnectInterval));

                if (reconnectionTimer == null)
                {
                    reconnectionTimer = new DispatcherTimer();
                    reconnectionTimer.Interval = TimeSpan.FromMilliseconds(ReconnectInterval);
                    reconnectionTimer.Tick += (o, ea) =>
                    {
                        CheckAndStart();
                    };
                }
                if (!reconnectionTimer.IsEnabled)
                    reconnectionTimer.Start();
            }
        }

        void Stop()
        {
            if (reconnectionTimer != null)
                reconnectionTimer.Stop();

            if (_mjpeg == null)
                return;

            delayFrameReadyInvoker = null;
            _mjpeg.Error -= _mjpeg_Error;
            _mjpeg.FrameReady -= mjpeg_FrameReady;
            _mjpeg.Dispose();
            _mjpeg = null;
        }

        DelayedSingleActionInvoker delayFrameReadyInvoker;
        BitmapImage lastBitmapImage;
        private void mjpeg_FrameReady(object sender, FrameReadyEventArgs e)
        {
            if (FrameRateUpdateImage > 0)
            {
                if (delayFrameReadyInvoker == null)
                {
                    delayFrameReadyInvoker = new DelayedSingleActionInvoker(() =>
                    {
                        if (bDispose)
                            return;

                        busyControl.Visibility = Visibility.Collapsed;
                        busyContent.Visibility = Visibility.Collapsed;

                        image.Source = lastBitmapImage;
                    }, TimeSpan.FromMilliseconds(FrameRateUpdateImage), restart: false);
                }

                lastBitmapImage = e.BitmapImage;
                delayFrameReadyInvoker.BeginInvoke();
            }
            else
            {
                busyControl.Visibility = Visibility.Collapsed;
                busyContent.Visibility = Visibility.Collapsed;

                image.Source = e.BitmapImage;
            }
        }

        #region IContainPropertyEditors Members

        [Browsable(false)]
        public Type ObjectType
        {
            get 
            { 
                return this.GetType(); 
            }
        }

        [Browsable(false)]
        [EditorBrowsable(EditorBrowsableState.Never)]
        public IDictionary<DependencyProperty, DataTemplate> GetListDataTemplates
        {
            get 
            {
                var mapDataTemplates = new Dictionary<DependencyProperty, DataTemplate>();

                // Defines Data Template for 'UriProperty' dependency property.
                var dt = new DataTemplate();
                var factory = new FrameworkElementFactory(typeof(UriPropertyEditor));
                dt.DataType = typeof(Uri);
                dt.VisualTree = factory;
                mapDataTemplates.Add(UriProperty, dt);

                // Defines Data Template for 'PasswordProperty' dependency property.
                dt = new DataTemplate();
                factory = new FrameworkElementFactory(typeof(PasswordPropertyEditor));
                dt.DataType = typeof(String);
                dt.VisualTree = factory;
                mapDataTemplates.Add(PasswordProperty, dt);

                return mapDataTemplates;
            }
        }

        #endregion

        #region IDataErrorInfo Members

        [Browsable(false)]
        public string Error
        {
            get
            {
                var context = new System.ComponentModel.DataAnnotations.ValidationContext(this, null, null);
                var results = new List<System.ComponentModel.DataAnnotations.ValidationResult>();

                return !System.ComponentModel.DataAnnotations.Validator.TryValidateObject(this, context, results)
                    ? string.Join(Environment.NewLine, results.Select(x => x.ErrorMessage))
                    : null;
            }
        }

        public string this[string propertyName]
        {
            get
            {
                if (propertyName == "Uri")
                {
                    if (Uri != null)
                    {
                        if (!Uri.IsAbsoluteUri ||
                        !Regex.IsMatch(Uri.OriginalString, @"^(http|https|ftp)://([\w+?\.\w+])+([a-zA-Z0-9\~\!\@\#\$\%\^\&\*\(\)_\-\=\+\\\/\?\.\:\;\'\,]*)?$"))
                            return Properties.Resources.UriNotSet;
                    }
                }

                return null;
            }
        }

        #endregion

        #region IDisposable Members
        bool bDispose;
        public void Dispose()
        {
            if (bDispose)
                return;
            bDispose = true;
            Stop();
        }

        #endregion
    }
}
