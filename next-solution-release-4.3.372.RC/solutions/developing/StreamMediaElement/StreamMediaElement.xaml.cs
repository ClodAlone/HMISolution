namespace StreamMediaElement
{
    using DocumentManager.ComponentService;
    using Utilities;
    using System;
    using System.ComponentModel;
    using System.Windows.Controls;
    using System.Windows.Threading;
    using ScreenSettings;
    using System.Windows;
    using System.Windows.Media;
    using System.IO;
    using System.Collections.Generic;
    using System.Text.RegularExpressions;
    using System.Threading.Tasks;
    using System.Threading;
    using UFInterfaces.PropertyControl;
    using CommonControls.PropertyDataTemplate;
    using WPFUtilities.Extensions;


    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class StreamMediaElementUC : UserControl, IDisposable, INotifyPropertyChanged, IContainPropertyEditors
    {
        #region DP
        #region CameraFrameURL
        public static readonly DependencyProperty CameraFrameURLProperty = DependencyProperty.Register("CameraFrameURL", typeof(string), typeof(StreamMediaElementUC), new UIPropertyMetadata("", new PropertyChangedCallback(OnCameraFrameURLChanged), new CoerceValueCallback(OnCoerceCameraFrameURL)));

        private static object OnCoerceCameraFrameURL(DependencyObject o, object value)
        {
            StreamMediaElementUC StreamMediaElementUC = o as StreamMediaElementUC;
            if (StreamMediaElementUC != null)
                return StreamMediaElementUC.OnCoerceCameraFrameURL((string)value);
            else
                return value;
        }

        private static void OnCameraFrameURLChanged(DependencyObject o, DependencyPropertyChangedEventArgs e)
        {
            StreamMediaElementUC StreamMediaElementUC = o as StreamMediaElementUC;
            if (StreamMediaElementUC != null)
                StreamMediaElementUC.OnCameraFrameURLChanged((string)e.OldValue, (string)e.NewValue);
        }

        protected virtual string OnCoerceCameraFrameURL(string value)
        {
            // TODO: Keep the proposed value within the desired range.
            return value;
        }

        protected virtual void OnCameraFrameURLChanged(string oldValue, string newValue)
        {
            // TODO: Add your property changed side-effects. Descendants can override as well.
        }

        public string CameraFrameURL
        {
            // IMPORTANT: To maintain parity between setting a property in XAML and procedural code, do not touch the getter and setter inside this dependency property!
            get
            {
                return (string)GetValue(CameraFrameURLProperty);
            }
            set
            {
                SetValue(CameraFrameURLProperty, value);
            }
        }
        #endregion

        public static readonly DependencyProperty StreamingURLProperty = DependencyProperty.Register("StreamingURL", typeof(string), typeof(StreamMediaElementUC), new UIPropertyMetadata("", new PropertyChangedCallback(OnStreamingURLChanged), new CoerceValueCallback(OnCoerceStreamingURLChanges)));

        private static object OnCoerceStreamingURLChanges(DependencyObject o, object value)
        {
            StreamMediaElementUC streamPlayerControl = o as StreamMediaElementUC;
            if (streamPlayerControl != null)
                return streamPlayerControl.OnCoerceStreamingURLChanges((string)value);
            else
                return value;
        }

        private static void OnStreamingURLChanged(DependencyObject o, DependencyPropertyChangedEventArgs e)
        {
            StreamMediaElementUC streamPlayerControl = o as StreamMediaElementUC;
            if (streamPlayerControl != null)
                streamPlayerControl.OnStreamingURLChanged((string)e.OldValue, (string)e.NewValue);
        }

        protected virtual string OnCoerceStreamingURLChanges(string value)
        {
            return value;
        }

        protected virtual void OnStreamingURLChanged(string oldValue, string newValue)
        {
            if (oldValue != newValue && Media != null && bInit && !bDesign)
            {
                //RebuildStreamURL(newValue, StreamUser, StreamPassword);
                OpenCommand.Execute();
            }
        }

        public string StreamingURL
        {
            // IMPORTANT: To maintain parity between setting a property in XAML and procedural code, do not touch the getter and setter inside this dependency property!
            get
            {
                return (string)GetValue(StreamingURLProperty);
            }
            set
            {
                if (!Media.IsOpening)
                    SetValue(StreamingURLProperty, value);
                else
                    StreamStatus = Properties.Resources.CantChangeWhileOpening;
            }
        }

        public static readonly DependencyProperty ForceTCPProperty = DependencyProperty.Register("ForceTCP", typeof(bool), typeof(StreamMediaElementUC), new UIPropertyMetadata(true, new PropertyChangedCallback(OnForceTCPChanged), new CoerceValueCallback(OnCoerceForceTCPChanges)));

        private static object OnCoerceForceTCPChanges(DependencyObject o, object value)
        {
            StreamMediaElementUC streamPlayerControl = o as StreamMediaElementUC;
            if (streamPlayerControl != null)
                return streamPlayerControl.OnCoerceForceTCPChanges((bool)value);
            else
                return value;
        }

        private static void OnForceTCPChanged(DependencyObject o, DependencyPropertyChangedEventArgs e)
        {
            StreamMediaElementUC streamPlayerControl = o as StreamMediaElementUC;
            if (streamPlayerControl != null)
                streamPlayerControl.OnForceTCPChanged((bool)e.OldValue, (bool)e.NewValue);
        }

        protected virtual bool OnCoerceForceTCPChanges(bool value)
        {
            return value;
        }

        protected virtual void OnForceTCPChanged(bool oldValue, bool newValue)
        {
            if (oldValue != newValue && Media != null && bInit && !bDesign)
            {
                Media.UpdateForceTCP(newValue);
            }
        }

        public bool ForceTCP
        {
            // IMPORTANT: To maintain parity between setting a property in XAML and procedural code, do not touch the getter and setter inside this dependency property!
            get
            {
                return (bool)GetValue(ForceTCPProperty);
            }
            set
            {
                SetValue(ForceTCPProperty, value);
            }
        }

        #region IsHighPriority
        public static readonly DependencyProperty IsHighPriorityProperty = DependencyProperty.Register("IsHighPriority", typeof(bool), typeof(StreamMediaElementUC), new UIPropertyMetadata(false, new PropertyChangedCallback(OnIsHighPriorityChanged), new CoerceValueCallback(OnCoerceIsHighPriority)));

        private static object OnCoerceIsHighPriority(DependencyObject o, object value)
        {
            StreamMediaElementUC StreamMediaElementUC = o as StreamMediaElementUC;
            if (StreamMediaElementUC != null)
                return StreamMediaElementUC.OnCoerceIsHighPriority((bool)value);
            else
                return value;
        }

        private static void OnIsHighPriorityChanged(DependencyObject o, DependencyPropertyChangedEventArgs e)
        {
            StreamMediaElementUC StreamMediaElementUC = o as StreamMediaElementUC;
            if (StreamMediaElementUC != null)
                StreamMediaElementUC.OnIsHighPriorityChanged((bool)e.OldValue, (bool)e.NewValue);
        }

        protected virtual bool OnCoerceIsHighPriority(bool value)
        {
            // TODO: Keep the proposed value within the desired range.
            return value;
        }

        protected virtual void OnIsHighPriorityChanged(bool oldValue, bool newValue)
        {
            // TODO: Add your property changed side-effects. Descendants can override as well.
        }

        public bool IsHighPriority
        {
            // IMPORTANT: To maintain parity between setting a property in XAML and procedural code, do not touch the getter and setter inside this dependency property!
            get
            {
                return (bool)GetValue(IsHighPriorityProperty);
            }
            set
            {
                SetValue(IsHighPriorityProperty, value);
            }
        }
        #endregion

        #region AllowFramesBadTimestamp
        public static readonly DependencyProperty AllowFramesBadTimestampProperty = DependencyProperty.Register("AllowFramesBadTimestamp", typeof(bool), typeof(StreamMediaElementUC), new UIPropertyMetadata(true, new PropertyChangedCallback(OnAllowFramesBadTimestampChanged), new CoerceValueCallback(OnCoerceAllowFramesBadTimestamp)));

        private static object OnCoerceAllowFramesBadTimestamp(DependencyObject o, object value)
        {
            StreamMediaElementUC StreamMediaElementUC = o as StreamMediaElementUC;
            if (StreamMediaElementUC != null)
                return StreamMediaElementUC.OnCoerceAllowFramesBadTimestamp((bool)value);
            else
                return value;
        }

        private static void OnAllowFramesBadTimestampChanged(DependencyObject o, DependencyPropertyChangedEventArgs e)
        {
            StreamMediaElementUC StreamMediaElementUC = o as StreamMediaElementUC;
            if (StreamMediaElementUC != null)
                StreamMediaElementUC.OnAllowFramesBadTimestampChanged((bool)e.OldValue, (bool)e.NewValue);
        }

        protected virtual bool OnCoerceAllowFramesBadTimestamp(bool value)
        {
            // TODO: Keep the proposed value within the desired range.
            return value;
        }

        protected virtual void OnAllowFramesBadTimestampChanged(bool oldValue, bool newValue)
        {
            // TODO: Add your property changed side-effects. Descendants can override as well.
        }

        public bool AllowFramesBadTimestamp
        {
            // IMPORTANT: To maintain parity between setting a property in XAML and procedural code, do not touch the getter and setter inside this dependency property!
            get
            {
                return (bool)GetValue(AllowFramesBadTimestampProperty);
            }
            set
            {
                SetValue(AllowFramesBadTimestampProperty, value);
            }
        }
        #endregion

        public static readonly DependencyProperty StreamUserProperty = DependencyProperty.Register("StreamUser", typeof(string), typeof(StreamMediaElementUC), new UIPropertyMetadata("", new PropertyChangedCallback(OnStreamUserChanged), new CoerceValueCallback(OnCoerceStreamUserChanges)));

        private static object OnCoerceStreamUserChanges(DependencyObject o, object value)
        {
            StreamMediaElementUC streamPlayerControl = o as StreamMediaElementUC;
            if (streamPlayerControl != null)
                return streamPlayerControl.OnCoerceStreamUserChanges((string)value);
            else
                return value;
        }

        private static void OnStreamUserChanged(DependencyObject o, DependencyPropertyChangedEventArgs e)
        {
            StreamMediaElementUC streamPlayerControl = o as StreamMediaElementUC;
            if (streamPlayerControl != null)
                streamPlayerControl.OnStreamUserChanged((string)e.OldValue, (string)e.NewValue);
        }

        protected virtual string OnCoerceStreamUserChanges(string value)
        {
            return value;
        }

        protected virtual void OnStreamUserChanged(string oldValue, string newValue)
        {
            //if (newValue != oldValue)
            //    RebuildStreamURL(StreamingURL, newValue, StreamPassword);
        }

        public string StreamUser
        {
            // IMPORTANT: To maintain parity between setting a property in XAML and procedural code, do not touch the getter and setter inside this dependency property!
            get
            {
                return (string)GetValue(StreamUserProperty);
            }
            set
            {
                SetValue(StreamUserProperty, value);
            }
        }

        public static readonly DependencyProperty StreamPasswordProperty = DependencyProperty.Register("StreamPassword", typeof(string), typeof(StreamMediaElementUC), new UIPropertyMetadata("", new PropertyChangedCallback(OnStreamPasswordChanged), new CoerceValueCallback(OnCoerceStreamPasswordChanges)));

        private static object OnCoerceStreamPasswordChanges(DependencyObject o, object value)
        {
            StreamMediaElementUC streamPlayerControl = o as StreamMediaElementUC;
            if (streamPlayerControl != null)
                return streamPlayerControl.OnCoerceStreamPasswordChanges((string)value);
            else
                return value;
        }

        private static void OnStreamPasswordChanged(DependencyObject o, DependencyPropertyChangedEventArgs e)
        {
            StreamMediaElementUC streamPlayerControl = o as StreamMediaElementUC;
            if (streamPlayerControl != null)
                streamPlayerControl.OnStreamPasswordChanged((string)e.OldValue, (string)e.NewValue);
        }

        protected virtual string OnCoerceStreamPasswordChanges(string value)
        {
            return value;
        }

        protected virtual void OnStreamPasswordChanged(string oldValue, string newValue)
        {
            //if (newValue != oldValue)
            //    RebuildStreamURL(StreamingURL, StreamUser, newValue);
        }

        public string StreamPassword
        {
            // IMPORTANT: To maintain parity between setting a property in XAML and procedural code, do not touch the getter and setter inside this dependency property!
            get
            {
                return (string)GetValue(StreamPasswordProperty);
            }
            set
            {
                SetValue(StreamPasswordProperty, value);
            }
        }

        #region IsAudioDisabled
        public static readonly DependencyProperty IsAudioDisabledProperty = DependencyProperty.Register("IsAudioDisabled", typeof(bool), typeof(StreamMediaElementUC), new UIPropertyMetadata(false, new PropertyChangedCallback(OnIsAudioDisabledChanged), new CoerceValueCallback(OnCoerceIsAudioDisabled)));

        private static object OnCoerceIsAudioDisabled(DependencyObject o, object value)
        {
            StreamMediaElementUC StreamMediaElementUC = o as StreamMediaElementUC;
            if (StreamMediaElementUC != null)
                return StreamMediaElementUC.OnCoerceIsAudioDisabled((bool)value);
            else
                return value;
        }

        private static void OnIsAudioDisabledChanged(DependencyObject o, DependencyPropertyChangedEventArgs e)
        {
            StreamMediaElementUC StreamMediaElementUC = o as StreamMediaElementUC;
            if (StreamMediaElementUC != null)
                StreamMediaElementUC.OnIsAudioDisabledChanged((bool)e.OldValue, (bool)e.NewValue);
        }

        protected virtual bool OnCoerceIsAudioDisabled(bool value)
        {
            // TODO: Keep the proposed value within the desired range.
            return value;
        }

        protected virtual void OnIsAudioDisabledChanged(bool oldValue, bool newValue)
        {
            // TODO: Add your property changed side-effects. Descendants can override as well.
        }

        public bool IsAudioDisabled
        {
            // IMPORTANT: To maintain parity between setting a property in XAML and procedural code, do not touch the getter and setter inside this dependency property!
            get
            {
                return (bool)GetValue(IsAudioDisabledProperty);
            }
            set
            {
                SetValue(IsAudioDisabledProperty, value);
            }
        }
        #endregion
        #endregion

        public event PropertyChangedEventHandler PropertyChanged;
        //private Dictionary<string, Action<bool>> PropertyUpdaters;
        //private Dictionary<string, string[]> PropertyTriggers;
        internal IDocument Document;
        bool bLoaded, bDisposed, bToBeDisposed, bInit, bDesign, bMediaFailed, bUriNotValid, bMediaAuthMissing, bMediaAuthFailed, bMediaProtocolError;
        private Uri streamingURL;
        List<Action> pendingActions = new List<Action>();

        //public Visibility PauseButtonVisibility { get; set; } = Visibility.Visible;
        //public Visibility PlayButtonVisibility { get; set; } = Visibility.Visible;
        //public Visibility StopButtonVisibility { get; set; } = Visibility.Visible;
        //public Visibility BufferingProgressVisibility { get; set; } = Visibility.Visible;
        //public Visibility DownloadProgressVisibility { get; set; } = Visibility.Visible;
        DispatcherTimer timer;
        string oldstatus;
        string streamStatus = Properties.Resources.StreamIdle;
        [Browsable(false)]
        public string StreamStatus {
            get {
                return streamStatus;
            }
            set {
                if (value == Properties.Resources.CantChangeWhileOpening)
                {
                    if (timer == null)
                    {
                        timer = new DispatcherTimer();
                        timer.Tick += timer_Tick;
                        timer.Interval = TimeSpan.FromMilliseconds(1000);
                    }
                    oldstatus = streamStatus;
                    timer.Start();
                }
                else
                {
                    timer?.Stop();
                    timer = null;
                }
                if(streamStatus != value)
                    streamStatus = value;
                OnPropertyChanged("StreamStatus");
            }
        }
        private DelegateCommand m_OpenCommand = null;
        private DelegateCommand m_StopCommand = null;
        private DelegateCommand m_CloseCommand = null;
        //private DelegateCommand m_PauseCommand = null;
        //private DelegateCommand m_PlayCommand = null;
        Uri lastOpeningUrl;
        Uri newOpeningUrl;
        Action playNextUriAction;
        bool bLastIsPlaying = false;
        bool bReopenOnPlaying = false;

        #region Constructor and Initialization

        /// <summary>
        /// Initializes a new instance of the <see cref="MainWindow"/> class.
        /// </summary>
        public StreamMediaElementUC()
        {
            try
            {
                var uri = new Uri(System.Reflection.Assembly.GetExecutingAssembly().EscapedCodeBase);
                Unosquare.FFME.MediaElement.FFmpegDirectory = String.Format("{0}\\", Path.GetDirectoryName(uri.LocalPath));
            }
            catch
            {
                Unosquare.FFME.MediaElement.FFmpegDirectory = String.Format("{0}\\", Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location)); //@"E:\Sources\ffmpeg-3.3.2\64bit\";
            }

            InitializeComponent();

            ScreenSettings.ScreenDocument.SetAutoForceDynamicOnClient(this, true);

            Loaded += (o, e) =>
            {
                if (!bLoaded && !bDisposed)
                {
                    bLoaded = true;
                    if (Document == null)
                        Document = ScreenSettings.ScreenDocument.GetScreenDocument(this);
                    InitControl();
                    OverrideBaseProperties();

                    if (DesignerProperties.GetIsInDesignMode(this) || bDesign)
                    {
                        bDesign = true;
                    }
                    else
                    {
                        InitHandlers();
                        Media.RenderControl();
                        Media.UpdateForceTCP(ForceTCP);
                        Media.IsHighPriority = IsHighPriority;
                        Media.BypassPTSCheck = AllowFramesBadTimestamp;
                        OpenCommand.ExecuteWithDispatcher(Dispatcher); //Autoplay
                    }
                    bInit = true;
                }
            };
        }
        private void OverrideBaseProperties()
        {
            DependencyPropertyDescriptor dpd;

            dpd = DependencyPropertyDescriptor.FromProperty(BackgroundProperty, typeof(StreamMediaElementUC));
            dpd.AddValueChangedSafe(this, OnBackgroundChanged);
        }
        private void DetachOverrideBaseProperties()
        {
            DependencyPropertyDescriptor dpd;

            dpd = DependencyPropertyDescriptor.FromProperty(BackgroundProperty, typeof(StreamMediaElementUC));
            dpd.RemoveValueChangedSafe(this, OnBackgroundChanged);
        }
        private void OnBackgroundChanged(object sender, EventArgs e)
        {
            var control = sender as StreamMediaElementUC;
            if (control != null)
            {
                control.OnBackgroundChanged();
            }
        }
        protected virtual void OnBackgroundChanged()
        {
            // TODO: Add your property changed side-effects. Descendants can override as well.
            if (!IsManipulationEnabled)
                mainBorder.Background = Background;
        }

        public void InitControl()
        {
            object bkp = this.ReadLocalValue(BackgroundProperty);
            if (bkp != DependencyProperty.UnsetValue && bkp!=  null)
                mainBorder.Background = Background;
            else
                mainBorder.Background = new SolidColorBrush(Color.FromRgb(0x34, 0x34, 0x34));
        }

        public void InitHandlers()
        {
            if (bDisposed)
                return;

            Media.PropertyChanged += Media_PropertyChanged;
            Media.MediaFailed += OnMediaFail;
            Media.MediaOpening += OnMediaOpening;

            //PropertyUpdaters = new Dictionary<string, Action<bool>>
            //{
            //{ nameof(PauseButtonVisibility), () => { PauseButtonVisibility = Media.CanPause && Media.IsPlaying ? Visibility.Visible : Visibility.Collapsed; } },
            //{ nameof(PlayButtonVisibility), (forceShow) => { PlayButtonVisibility = forceShow || (Media.IsPlaying == false && Media.HasMediaEnded == false && Media.IsOpening == false) ? Visibility.Visible : Visibility.Collapsed; } },
            //{ nameof(StopButtonVisibility), (forceShow) => { StopButtonVisibility = forceShow || Media.IsOpening || (Media.IsOpen && Media.MediaState != MediaState.Stop) ? Visibility.Visible : Visibility.Collapsed; } },
            //{ nameof(BufferingProgressVisibility), (forceShow) => { BufferingProgressVisibility = Media.IsBuffering ? Visibility.Visible : Visibility.Hidden; } },
            //{ nameof(StreamStatus), (forceShow) => { UpdateStreamStatus(); } }
            //};

            /*PropertyTriggers = new Dictionary<string, string[]>
            {
                { nameof(Media.IsOpen), PropertyUpdaters.Keys.ToArray() },
                { nameof(Media.IsOpening), PropertyUpdaters.Keys.ToArray() },
                { nameof(Media.MediaState), PropertyUpdaters.Keys.ToArray() },
                { nameof(Media.HasMediaEnded), PropertyUpdaters.Keys.ToArray() },
                { nameof(Media.IsBuffering), new[] { nameof(BufferingProgressVisibility) } },
            };

            foreach (var kvp in PropertyUpdaters)
            {
                kvp.Value.Invoke(bDesign);
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(kvp.Key));
            }*/
        }

        private string lastMediaFailErrorDetail = String.Empty;
        private void OnMediaFail(object sender, ExceptionRoutedEventArgs args)
        {
            if (args.ErrorException.Message.Contains("401 Unauthorized"))
                if (streamingURL.ToString().Contains("@"))
                    bMediaAuthFailed = true;
                else
                    bMediaAuthMissing = true;
            else if (args.ErrorException.Message.Contains("Protocol not found"))
                bMediaProtocolError = true;
            else
            {
                var innerMsg = args.ErrorException.Message;
                if (innerMsg == "waveOutOpen")
                    innerMsg = String.Format(Properties.Resources.StreamAudioException);
                lastMediaFailErrorDetail = innerMsg;
                bMediaFailed = true;
            }
            //StopButtonVisibility = Visibility.Hidden;
            //PlayButtonVisibility = Visibility.Visible;
        }

        private void OnMediaOpening(object sender, Unosquare.FFME.MediaOpeningRoutedEventArgs args)
        {
            args.Options.IsAudioDisabled = IsAudioDisabled;
        }

        private void Media_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (bToBeDisposed || bDisposed)
                return;

            switch (e.PropertyName)
            {
                case "HasChangedWhileOpening":
                    if ((sender as Unosquare.FFME.MediaElement).HasChangedWhileOpening)
                        StreamStatus = Properties.Resources.CantChangeWhileOpening;
                    break;
                case "IsOpen":
                case "IsOpening":
                case "IsBuffering":
                    {
                        UpdateStreamStatus();
                        break;
                    }
                case "IsPlaying":
                    {
                        if (Media.IsPlaying == bLastIsPlaying)
                            return;
                        bLastIsPlaying = Media.IsPlaying;

                        if (!Media.IsPlaying && !pendingActions.Contains(playNextUriAction) && lastOpeningUrl != newOpeningUrl)
                        {
                            playNextUriAction = new Action(() =>
                            {
                                if (!Media.ct.IsCancellationRequested)
                                    OpenAndPlay(newOpeningUrl);
                            });

                            StartTaskAndCheckDispose(playNextUriAction);
                        }
                        else if (Media.IsPlaying && bReopenOnPlaying)
                        {
                            bReopenOnPlaying = false;
                            OpenCommand.Execute();
                        }
                        
                        UpdateStreamStatus();
                        break;
                    }
                default: break;
            }
        }

        private void StartTaskAndCheckDispose(Action action)
        {
            var tsk = Task.Factory.StartNew(action, Media.tokenSource.Token);
            pendingActions.Add(action);
            var tskContinue = tsk.ContinueWith(ret =>
            {
                pendingActions.Remove(action);
                if (pendingActions.Count == 0 && bToBeDisposed)
                    Dispose();
            }, TaskScheduler.FromCurrentSynchronizationContext());
        }

        private void OpenAndPlay(Uri newUri)
        {
            if (bToBeDisposed || bDisposed || lastOpeningUrl == newUri)
                return;

            lastOpeningUrl = newUri;

            if (newUri == null)
                return;

            if (!Media.ct.IsCancellationRequested)
                Media.Open(newUri);
            if (!Media.ct.IsCancellationRequested)
                Media.Play();
        }

        private void timer_Tick(object sender, EventArgs e)
        {
            if (StreamStatus == Properties.Resources.CantChangeWhileOpening)
                StreamStatus = oldstatus;
            (sender as DispatcherTimer).Stop();
        }

        private void UpdateStreamStatus()
        {
            if (bUriNotValid)
            {
                StreamStatus = Properties.Resources.UriNotValid;
            }
            else if (bMediaAuthMissing)
            {
                StreamStatus = Properties.Resources.StreamAuthMissing;
            }
            else if (bMediaAuthFailed)
            {
                StreamStatus = Properties.Resources.StreamAuthFailed;
            }
            else if (bMediaProtocolError)
            {
                StreamStatus = Properties.Resources.StreamProtocolError;
            }
            else if (bMediaFailed)
            {
                StreamStatus = String.Format("{0} [{1}] ({2})", Properties.Resources.StreamFailed, lastMediaFailErrorDetail, streamingURL);
            }
            else if (Media.IsPlaying)
            {
                StreamStatus = Properties.Resources.StreamPlaying;
            }
            else if (Media.IsBuffering && Media.IsPlaying)
            {
                StreamStatus = Properties.Resources.StreamBuffering;
            }
            else if (Media.IsOpening)
            {
                StreamStatus = Properties.Resources.StreamConnecting;
            }
            else
                StreamStatus = Properties.Resources.StreamIdle;
        }

        private void RebuildStreamURL(string newURL, string newStreamUser, string newStreamPassword)
        {
            streamingURL = new Uri(newURL);
            if (newURL.Contains("@")) //URL auth data has priority
                return;
            string authPart = (String.IsNullOrEmpty(newStreamUser) || String.IsNullOrEmpty(newStreamPassword)) ? "" : (newStreamUser + ":" + newStreamPassword);
            Regex r = new Regex(@"^([0-9A-Za-z]+:\/\/)(.*?)(\/.*?)?$", RegexOptions.IgnoreCase);
            Match m = r.Match(newURL);
            if (m.Success && m.Groups.Count >= 4)
            {
                newURL = m.Groups[1].Value + (authPart!="" ? String.Format("{0}{1}", authPart, "@") : "") + m.Groups[2].Value + m.Groups[3].Value;
            }
            Uri u = null;
            try
            {
                string stdUri = newURL
                    .Replace("axrtsphttp://", "rtsp://")
                    .Replace("axrtpm://", "rtsp://")
                    .Replace("axrtpu://", "rtsp://")
                    .Replace("axrtsp://", "rtsp://");
                u = new Uri(stdUri);
            }
            catch (UriFormatException) { }
            if (u != null)
            {
                streamingURL = u;
            }
        }

        #endregion
        [Browsable(false)]
        public DelegateCommand OpenCommand
        {
            get
            {
                bUriNotValid = false;
                bMediaFailed = false;
                bMediaAuthMissing = bMediaAuthFailed = false;
                bMediaProtocolError = false;
                if (m_OpenCommand == null)
                    m_OpenCommand = new DelegateCommand((a) =>
                    {
                        Dispatcher.BeginInvokeInBackgroundIfRequired(() =>
                        {
                            Uri sourceUri = null;
                            try
                            {
                                RebuildStreamURL(StreamingURL, StreamUser, StreamPassword);
                                sourceUri = streamingURL;
                            }
                            catch (Exception)
                            {
                                bUriNotValid = true;
                                UpdateStreamStatus();
                                return;
                            }
                            if (sourceUri != null && sourceUri != Media.Source)
                            {
                                Media.Source = sourceUri;
                                if (Media.tokenSource == null)
                                {
                                    Media.tokenSource = new CancellationTokenSource();
                                    Media.ct = Media.tokenSource.Token;
                                }

                                var closeAndOpenAction = new Action(() =>
                                {
                                    try
                                    {
                                        if (!Media.ct.IsCancellationRequested)
                                        {
                                            newOpeningUrl = sourceUri;
                                            if (Media.IsPlaying)
                                                Media.Close();
                                            else if (!Media.ct.IsCancellationRequested)
                                            {
                                                if (!Media.IsOpening && !Media.IsOpen)
                                                    OpenAndPlay(sourceUri);
                                                else //uri changed before last uri was opened: must wait its open before opening the new uri
                                                    bReopenOnPlaying = true;
                                            }
                                        }
                                    }
                                    catch (Exception ex)
                                    {
                                        //System.Diagnostics.Debug.WriteLine("{0}: {1}", this.Name, ex.Message);
                                    }
                                });
                                StartTaskAndCheckDispose(closeAndOpenAction);
                            }
                        });
                    }, null);

                return m_OpenCommand;
            }
        }
        [Browsable(false)]
        public DelegateCommand StopCommand
        {
            get
            {
                bUriNotValid = false;
                bMediaFailed = false;
                bMediaAuthMissing = bMediaAuthFailed = false;
                bMediaProtocolError = false;
                if (m_StopCommand == null)
                    m_StopCommand = new DelegateCommand((o) => {
                        Dispatcher.BeginInvokeInBackgroundIfRequired(() =>
                        {
                            Media.Stop();
                        });
                    }, null);

                return m_StopCommand;
            }
        }
        //[Browsable(false)]
        //public DelegateCommand PauseCommand
        //{
        //    get
        //    {
        //        if (m_PauseCommand == null)
        //            m_PauseCommand = new DelegateCommand((o) => {
        //                Dispatcher.BeginInvokeInBackgroundIfRequired(() =>
        //                {
        //                    Media.Pause();
        //                });
        //            }, null);

        //        return m_PauseCommand;
        //    }
        //}
        [Browsable(false)]
        public DelegateCommand CloseCommand
        {
            get
            {
                if (m_CloseCommand == null)
                    m_CloseCommand = new DelegateCommand((o) => {
                        Dispatcher.BeginInvokeInBackgroundIfRequired(() =>
                        {
                            Media.Close();
                        });
                    }, null);

                return m_CloseCommand;
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

                // Defines Data Template for 'PasswordProperty' dependency property.
                var dt = new DataTemplate();
                var factory = new FrameworkElementFactory(typeof(PasswordPropertyEditor));
                dt.DataType = typeof(String);
                dt.VisualTree = factory;
                mapDataTemplates.Add(StreamPasswordProperty, dt);

                return mapDataTemplates;
            }
        }

        #endregion

        #region INotifyPropertyChanged Members

        /// <summary>
        /// Raised when a property on this object has a new value.
        /// </summary>
#if !WINDOWS_UWP

        /// <summary>
        /// Raises this object's PropertyChanged event.
        /// </summary>
        /// <param name="propertyName">The property that has a new value.</param>
        protected virtual void OnPropertyChanged(string propertyName)
        {
            // VerifyPropertyName(propertyName);

            PropertyChangedEventHandler handler = PropertyChanged;
            if (handler != null)
            {
                DispatcherObject dispatcherObject = handler.Target as DispatcherObject;

                var e = new PropertyChangedEventArgs(propertyName);
                // If the subscriber is a DispatcherObject and different thread
                if (dispatcherObject != null && dispatcherObject.CheckAccess() == false)
                {
                    // Invoke handler in the target dispatcher's thread
                    dispatcherObject.Dispatcher.BeginInvoke(DispatcherPriority.DataBind, handler, this, e);
                }
                else // Execute handler as is
                    handler(this, e);
            }
        }
#endif
        #endregion // INotifyPropertyChanged Members

        #region IDisposable
        public void Dispose()
        {
            if (pendingActions.Count > 0)
            {
                bToBeDisposed = true; //Using Task.WaitAll() causes deadlocks if tasks try to access the UI thread
                if (Media.tokenSource != null && !Media.tokenSource.IsCancellationRequested)
                    Media.tokenSource.Cancel();
                return;
            }

            if (bDisposed)
                return;
            bDisposed = true;

            if (Media.tokenSource != null)
                Media.tokenSource.Dispose();

            DetachOverrideBaseProperties();
            Media.PropertyChanged -= Media_PropertyChanged;
            Media.MediaFailed -= OnMediaFail;
            Media.MediaOpening -= OnMediaOpening;

            if (Media is IDisposable)
                (Media as IDisposable).Dispose();
        }
        #endregion
    }
}
