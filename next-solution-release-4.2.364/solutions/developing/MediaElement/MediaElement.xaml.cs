using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Markup;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using CommonControls.PropertyDataTemplate;
using DocumentManager.ComponentService;
using UFInterfaces.PropertyControl;
using Utilities;
using WPFUtilities.PropertyDataTemplate;
using UIMsgBoxAlertService.ComponentService;
using UFInterfaces;
using Utilities.WPF;

namespace MediaElement
{
    /// <summary>
    /// Interaction logic for UserControl1.xaml
    /// </summary>
    public partial class MediaElement : UserControl, IContainPropertyEditors, IDataErrorInfo, IDisposable
    {
        #region DP
        #region KeepAspectRatio
        public static readonly DependencyProperty KeepAspectRatioProperty = DependencyProperty.Register("KeepAspectRatio", typeof(bool), typeof(MediaElement), new UIPropertyMetadata(false, new PropertyChangedCallback(OnKeepAspectRatioChanged), new CoerceValueCallback(OnCoerceKeepAspectRatio)));

        private static object OnCoerceKeepAspectRatio(DependencyObject o, object value)
        {
            MediaElement MediaElement = o as MediaElement;
            if (MediaElement != null)
                return MediaElement.OnCoerceKeepAspectRatio((bool)value);
            else
                return value;
        }

        private static void OnKeepAspectRatioChanged(DependencyObject o, DependencyPropertyChangedEventArgs e)
        {
            MediaElement MediaElement = o as MediaElement;
            if (MediaElement != null)
                MediaElement.OnKeepAspectRatioChanged((bool)e.OldValue, (bool)e.NewValue);
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
        #region Source
        public static readonly DependencyProperty SourceProperty = DependencyProperty.Register("Source", typeof(Uri), typeof(MediaElement), new UIPropertyMetadata(null, new PropertyChangedCallback(OnSourceChanged), new CoerceValueCallback(OnCoerceSource)));

        private static object OnCoerceSource(DependencyObject o, object value)
        {
            MediaElement mediaElement = o as MediaElement;
            if (mediaElement != null)
                return mediaElement.OnCoerceSource((Uri)value);
            else
                return value;
        }

        private static void OnSourceChanged(DependencyObject o, DependencyPropertyChangedEventArgs e)
        {
            MediaElement mediaElement = o as MediaElement;
            if (mediaElement != null)
                mediaElement.OnSourceChanged((Uri)e.OldValue, (Uri)e.NewValue);
        }

        protected virtual Uri OnCoerceSource(Uri value)
        {
            // TODO: Keep the proposed value within the desired range.
            return value;
        }

        protected virtual void OnSourceChanged(Uri oldValue, Uri newValue)
        {
            // TODO: Add your property changed side-effects. Descendants can override as well.
            if (bLoaded && bInit && !bDispose)
                ManageState(newValue);
        }

        public static readonly DependencyProperty LoopProperty = DependencyProperty.Register("Loop", typeof(bool), typeof(MediaElement), new UIPropertyMetadata(false, new PropertyChangedCallback(OnLoopChanged), new CoerceValueCallback(OnCoerceLoop)));

        private static object OnCoerceLoop(DependencyObject o, object value)
        {
            MediaElement mediaElement = o as MediaElement;
            if (mediaElement != null)
                return mediaElement.OnCoerceLoop((bool)value);
            else
                return value;
        }

        private static void OnLoopChanged(DependencyObject o, DependencyPropertyChangedEventArgs e)
        {
            MediaElement mediaElement = o as MediaElement;
            if (mediaElement != null)
                mediaElement.OnLoopChanged((bool)e.OldValue, (bool)e.NewValue);
        }

        protected virtual bool OnCoerceLoop(bool value)
        {
            // TODO: Keep the proposed value within the desired range.
            return value;
        }

        protected virtual void OnLoopChanged(bool oldValue, bool newValue)
        {
            // TODO: Add your property changed side-effects. Descendants can override as well.
        }
        readonly IEnumerable<string> images = new string[5] { ".png", ".bmp", ".jpg", ".gif", ".tif" };
        private void ManageState(Uri source)
        {
            if (source == null)
            {
                mediaElement.Source = null;
                errorContent.Visibility = Visibility.Collapsed;
                mediaElement.Visibility = Visibility.Visible;
                controls.IsEnabled = true;
                controls.Visibility = LoadedBehaviour == MediaState.Manual ? Visibility.Visible : System.Windows.Visibility.Collapsed;
                return;
            }

            string sourcepath = source.GetPathString();
            if (!source.IsAbsoluteUri || source.IsAbsoluteUri && source.IsFile)
            {
                mediaElement.LoadedBehavior = MediaState.Manual;
                mediaElement.Stop();
                mediaElement.Source = null;

                if (System.IO.File.Exists(sourcepath))
                {
                    (mediaElement as IUriContext).BaseUri = new Uri(System.IO.Path.GetDirectoryName(sourcepath));
                    mediaElement.Source = source;
                }
                else if (Document != null)
                {
                    Uri docUri = Document.GetSpecialFolder(SpecialFolders.Images);
                    var dest = System.IO.Path.GetDirectoryName(docUri.GetPathString());
                    string destfilename = System.IO.Path.Combine(dest, sourcepath);
                    if (Document.fileSystemProviderBase != null)
                    {
                        VFS.FileManagerFile fileManagerFile = new VFS.FileManagerFile(Document.fileSystemProviderBase, destfilename);
                        if(Document.fileSystemProviderBase.Exists(fileManagerFile))
                        {
                            try
                            {
                                byte[] data = Document.fileSystemProviderBase.ReadFile(fileManagerFile);
                                tempFileName = GetTempFile(System.IO.Path.GetExtension(sourcepath));
                                System.IO.File.WriteAllBytes(tempFileName, data);
                                (mediaElement as IUriContext).BaseUri = new Uri(System.IO.Path.GetDirectoryName(tempFileName));
                                mediaElement.Source = new Uri(tempFileName);
                            }
                            catch (Exception ex)
                            {
                                mediaElement.Source = null;
                                errorContent.Visibility = Visibility.Visible;
                                controls.IsEnabled = false;
                                mediaElement.Visibility = Visibility.Collapsed;
                                return;
                            }
                        }
                    }
                    else if (System.IO.File.Exists(destfilename))
                    {
                        (mediaElement as IUriContext).BaseUri = docUri;
                        mediaElement.Source = source;
                    }
                    else
                    {
                        destfilename = System.IO.Path.Combine(System.IO.Path.GetDirectoryName(Document.FilePath), sourcepath);
                        if (System.IO.File.Exists(destfilename))
                        {
                            mediaElement.Source = new Uri(destfilename, UriKind.RelativeOrAbsolute);
                        }
                        else
                        {
                            mediaElement.Source = source;
                        }
                    }
                }
            }
            else
            {
                try
                {
                    if (source.IsAbsoluteUri)
                        mediaElement.Source = new Uri(source.AbsoluteUri, UriKind.Absolute);
                    else
                        mediaElement.Source = source;
                }
                catch
                {
                    mediaElement.Source = null;
                    errorContent.Visibility = Visibility.Visible;
                    controls.IsEnabled = false;
                    mediaElement.Visibility = Visibility.Collapsed;
                    return;
                }
            }

            try
            {
                if (bDesign)
                {
                    string _extension = string.Empty;
                    if (mediaElement.Source != null)
                    {
                        _extension = System.IO.Path.GetExtension(sourcepath);
                        if ((from i in images where i == _extension select i).FirstOrDefault() != null)
                            mediaElement.Play();
                    }
                }
                else
                {
                    string _extension = string.Empty;
                    if (mediaElement.Source != null)
                    {
                        _extension = System.IO.Path.GetExtension(sourcepath);
                        if ((from i in images where i == _extension select i).FirstOrDefault() != null)
                            mediaElement.Play();
                    }
                    else
                        switch (LoadedBehaviour)
                        {
                            case MediaState.Play:
                                mediaElement.Play();
                                break;
                            case MediaState.Stop:
                                mediaElement.Stop();
                                break;
                            case MediaState.Pause:
                                mediaElement.Pause();
                                break;
                            case MediaState.Manual:
                                mediaElement.Stop();
                                break;
                            case MediaState.Close:
                                mediaElement.Close();
                                break;
                        }
                    mediaElement.LoadedBehavior = LoadedBehaviour;
                    mediaElement.Stretch = Stretch;
                    mediaElement.StretchDirection = StretchDirection;
                    controls.Visibility = LoadedBehaviour == MediaState.Manual ? Visibility.Visible : System.Windows.Visibility.Collapsed;
                }

            }
            catch
            {
                mediaElement.Source = null;
                errorContent.Visibility = Visibility.Visible;
                controls.IsEnabled = false;
                mediaElement.Visibility = Visibility.Collapsed;
                return;
            }

            controls.IsEnabled = true;
            controls.Visibility = LoadedBehaviour == MediaState.Manual ? Visibility.Visible : System.Windows.Visibility.Collapsed;
            errorContent.Visibility = Visibility.Collapsed;
            mediaElement.Visibility = Visibility.Visible;
        }

        private string GetTempFile(string extension)
        {
            try
            {
                if (!string.IsNullOrEmpty(tempFileName) && System.IO.File.Exists(tempFileName))
                    System.IO.File.Delete(tempFileName);
            }
            catch
            {
            }

            string tempDirectory = System.IO.Path.GetTempPath();
            string filename = Guid.NewGuid().ToString() + extension;
            var originalName = filename;
            var rnd = new Random();
            bool bok = false;
            var ret = System.IO.Path.Combine(tempDirectory, filename);
            do
            {
                if (!System.IO.File.Exists(System.IO.Path.Combine(tempDirectory, filename)))
                    bok = true;
                else
                    filename = rnd.Next(1000).ToString() + originalName;
            } while (!bok);
            return ret;
        }

        public Uri Source
        {
            // IMPORTANT: To maintain parity between setting a property in XAML and procedural code, do not touch the getter and setter inside this dependency property!
            get
            {
                return (Uri)GetValue(SourceProperty);
            }
            set
            {
                SetValue(SourceProperty, value);
            }
        }
        public bool Loop
        {
            // IMPORTANT: To maintain parity between setting a property in XAML and procedural code, do not touch the getter and setter inside this dependency property!
            get
            {
                return (bool)GetValue(LoopProperty);
            }
            set
            {
                SetValue(LoopProperty, value);
            }
        }
        #endregion
        #region LoadedBehaviour
        public static readonly DependencyProperty LoadedBehaviourProperty = DependencyProperty.Register("LoadedBehaviour", typeof(MediaState), typeof(MediaElement), new UIPropertyMetadata(MediaState.Manual, new PropertyChangedCallback(OnLoadedBehaviourChanged), new CoerceValueCallback(OnCoerceLoadedBehaviour)));

        private static object OnCoerceLoadedBehaviour(DependencyObject o, object value)
        {
            MediaElement mediaElement = o as MediaElement;
            if (mediaElement != null)
                return mediaElement.OnCoerceLoadedBehaviour((MediaState)value);
            else
                return value;
        }

        private static void OnLoadedBehaviourChanged(DependencyObject o, DependencyPropertyChangedEventArgs e)
        {
            MediaElement mediaElement = o as MediaElement;
            if (mediaElement != null)
                mediaElement.OnLoadedBehaviourChanged((MediaState)e.OldValue, (MediaState)e.NewValue);
        }

        protected virtual MediaState OnCoerceLoadedBehaviour(MediaState value)
        {
            // TODO: Keep the proposed value within the desired range.
            return value;
        }

        protected virtual void OnLoadedBehaviourChanged(MediaState oldValue, MediaState newValue)
        {
            // TODO: Add your property changed side-effects. Descendants can override as well.
            if (bLoaded && bInit)
                ManageState(Source);
        }

        public MediaState LoadedBehaviour
        {
            // IMPORTANT: To maintain parity between setting a property in XAML and procedural code, do not touch the getter and setter inside this dependency property!
            get
            {
                return (MediaState)GetValue(LoadedBehaviourProperty);
            }
            set
            {
                SetValue(LoadedBehaviourProperty, value);
            }
        }
        #endregion
        #region Stretch
        public static readonly DependencyProperty StretchProperty = DependencyProperty.Register("Stretch", typeof(Stretch), typeof(MediaElement), new UIPropertyMetadata(Stretch.Fill, new PropertyChangedCallback(OnStretchChanged), new CoerceValueCallback(OnCoerceStretch)));

        private static object OnCoerceStretch(DependencyObject o, object value)
        {
            MediaElement MediaElement = o as MediaElement;
            if (MediaElement != null)
                return MediaElement.OnCoerceStretch((Stretch)value);
            else
                return value;
        }

        private static void OnStretchChanged(DependencyObject o, DependencyPropertyChangedEventArgs e)
        {
            MediaElement MediaElement = o as MediaElement;
            if (MediaElement != null)
                MediaElement.OnStretchChanged((Stretch)e.OldValue, (Stretch)e.NewValue);
        }

        protected virtual Stretch OnCoerceStretch(Stretch value)
        {
            // TODO: Keep the proposed value within the desired range.
            return value;
        }

        protected virtual void OnStretchChanged(Stretch oldValue, Stretch newValue)
        {
            // TODO: Add your property changed side-effects. Descendants can override as well.
            if (bLoaded && bInit)
                ManageState(Source);
        }

        public Stretch Stretch
        {
            // IMPORTANT: To maintain parity between setting a property in XAML and procedural code, do not touch the getter and setter inside this dependency property!
            get
            {
                return (Stretch)GetValue(StretchProperty);
            }
            set
            {
                SetValue(StretchProperty, value);
            }
        }
        #endregion
        #region StretchDirection
        public static readonly DependencyProperty StretchDirectionProperty = DependencyProperty.Register("StretchDirection", typeof(StretchDirection), typeof(MediaElement), new UIPropertyMetadata(StretchDirection.Both, new PropertyChangedCallback(OnStretchDirectionChanged), new CoerceValueCallback(OnCoerceStretchDirection)));

        private static object OnCoerceStretchDirection(DependencyObject o, object value)
        {
            MediaElement MediaElement = o as MediaElement;
            if (MediaElement != null)
                return MediaElement.OnCoerceStretchDirection((StretchDirection)value);
            else
                return value;
        }

        private static void OnStretchDirectionChanged(DependencyObject o, DependencyPropertyChangedEventArgs e)
        {
            MediaElement MediaElement = o as MediaElement;
            if (MediaElement != null)
                MediaElement.OnStretchDirectionChanged((StretchDirection)e.OldValue, (StretchDirection)e.NewValue);
        }

        protected virtual StretchDirection OnCoerceStretchDirection(StretchDirection value)
        {
            // TODO: Keep the proposed value within the desired range.
            return value;
        }

        protected virtual void OnStretchDirectionChanged(StretchDirection oldValue, StretchDirection newValue)
        {
            // TODO: Add your property changed side-effects. Descendants can override as well.
            if (bLoaded && bInit)
                ManageState(Source);
        }

        public StretchDirection StretchDirection
        {
            // IMPORTANT: To maintain parity between setting a property in XAML and procedural code, do not touch the getter and setter inside this dependency property!
            get
            {
                return (StretchDirection)GetValue(StretchDirectionProperty);
            }
            set
            {
                SetValue(StretchDirectionProperty, value);
            }
        }
        #endregion

        #endregion
        #region Declarations
        bool bLoaded;
        bool bDesign;
        bool bInit;
        IDocument Document;
        IWorkspace Workspace;
        IUIMsgBoxAlertService MsgBoxAlertService;
        #endregion
        string tempFileName;
        public MediaElement()
        {
            InitializeComponent();

            ScreenSettings.ScreenDocument.SetAutoForceDynamicOnClient(this, true);

            Loaded += (o, e) =>
            {
                Document = ScreenSettings.ScreenDocument.GetScreenDocument(this);
                ThemeImageHelper.LoadBitmapImageResourceDictionary(this.Resources, Document);
                play.SetResourceReference(Image.SourceProperty, "MELPlay");
                pause.SetResourceReference(Image.SourceProperty, "MELPause");
                stop.SetResourceReference(Image.SourceProperty, "MELStop");

                if (Document != null)
                    MsgBoxAlertService = Document.GetService(typeof(IUIMsgBoxAlertService)) as IUIMsgBoxAlertService;

                if (bDesign || DesignerProperties.GetIsInDesignMode(this))
                {
                    bDesign = true;

                    if (!bLoaded && !bDispose)
                    {
                        bLoaded = true;
                       
                        mediaElement.IsEnabled = false;
                        ManageState(Source);
                    }
                    DesignerProperties.SetIsInDesignMode(this, true);
                }
                else
                {
                    if (!bLoaded && !bDispose)
                    {
                        bLoaded = true;

                        bool runningOnServer = ScreenSettings.ScreenDocument.GetRunningOnServer(this);
                        if (runningOnServer)
                        {
                            LoadedBehaviour = MediaState.Manual;
                            ManageState(Source);
                            disabledControlContent.Visibility = Visibility.Visible;
                            controls.Visibility = System.Windows.Visibility.Collapsed;
                        }
                        else
                        {
                            mediaElement.IsEnabled = true;
                            if (Source != null)
                            {
                                ManageState(Source);
                            }
                        }
                    }
                }
                bInit = true;
            };
            Unloaded += (o, e) =>
            {
                if (bLoaded)
                {
                    bLoaded = false;
                    mediaElement.IsEnabled = false;
                    mediaElement.LoadedBehavior = MediaState.Manual;
                    if(mediaElement.Source != null && (!mediaElement.Source.IsAbsoluteUri || (mediaElement.Source.IsAbsoluteUri && mediaElement.Source.IsFile)))
                        mediaElement.Stop();
                }
            };
        }

        // Play the media.
        private void OnClickPlayMedia(object sender, RoutedEventArgs e)
        {
            e.Handled = true;
            // The Play method will begin the media if it is not currently active or 
            // resume media if it is paused. This has no effect if the media is
            // already running.
            try
            {
                if (mediaElement != null)
                    mediaElement.Play();
            }
            catch
            {

            }

            // Initialize the MediaElement property values.
            InitializePropertyValues();
        }

        // Pause the media.
        private void OnClickPauseMedia(object sender, RoutedEventArgs e)
        {
            e.Handled = true;
            // The Pause method pauses the media if it is currently running.
            // The Play method can be used to resume.
            try
            {
                if (mediaElement != null)
                    mediaElement.Pause();
            }
            catch
            {

            }
        }

        // Stop the media
        private void OnClickStopMedia(object sender, RoutedEventArgs e)
        {
            e.Handled = true;
            // The Stop method stops and resets the media to be played from
            // the beginning.
            try
            {
                if (mediaElement != null)
                    mediaElement.Stop();
            }
            catch
            {

            }
        }

        // Change the volume of the media.
        private void ChangeMediaVolume(object sender, RoutedPropertyChangedEventArgs<double> args)
        {
            try
            {
                if (mediaElement != null && mediaElement.HasAudio)
                    mediaElement.Volume = (double)volumeSlider.Value;
            }
            catch
            {

            }
        }

        // Change the speed of the media.
        private void ChangeMediaSpeedRatio(object sender, RoutedPropertyChangedEventArgs<double> args)
        {
            try
            {
                if (mediaElement != null)
                    mediaElement.SpeedRatio = (double)speedRatioSlider.Value;
            }
            catch
            {
                
            }
        }

        // When the media opens, initialize the "Seek To" slider maximum value
        // to the total number of miliseconds in the length of the media clip.
        private void Element_MediaOpened(object sender, EventArgs e)
        {
            if (mediaElement.NaturalDuration.HasTimeSpan)
                timelineSlider.Maximum = mediaElement.NaturalDuration.TimeSpan.TotalMilliseconds;
            else
            {
                timelineSlider.Maximum = timelineSlider.Minimum;
            }

        }

        // When the media playback is finished. Stop() the media to seek to media start.
        private void Element_MediaEnded(object sender, EventArgs e)
        {
            try
            {
                if (mediaElement != null)
                {
                    if (Loop)
                        mediaElement.Position = TimeSpan.FromMilliseconds(1);
                    else if (mediaElement.LoadedBehavior == MediaState.Manual)
                        mediaElement.Stop();
                }
            }
            catch
            {

            }
        }

        // Jump to different parts of the media (seek to). 
        private void SeekToMediaPosition(object sender, RoutedPropertyChangedEventArgs<double> args)
        {
            int SliderValue = (int)timelineSlider.Value;

            // Overloaded constructor takes the arguments days, hours, minutes, seconds, miniseconds.
            // Create a TimeSpan with miliseconds equal to the slider value.
            TimeSpan ts = new TimeSpan(0, 0, 0, 0, SliderValue);
            if (mediaElement != null)
                mediaElement.Position = ts;
        }

        void InitializePropertyValues()
        {
            // Set the media's starting Volume and SpeedRatio to the current value of the
            // their respective slider controls.
            if (mediaElement != null)
            {
                mediaElement.Volume = (double)volumeSlider.Value;
                mediaElement.SpeedRatio = (double)speedRatioSlider.Value;
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
        public IDictionary<DependencyProperty, DataTemplate> GetListDataTemplates
        {
            get
            {
                var mapDataTemplates = new Dictionary<DependencyProperty, DataTemplate>();
                if (Workspace == null)
                {
                    if (Document == null)
                        Document = ScreenSettings.ScreenDocument.GetScreenDocument(this);
                    if (Document != null)
                        Workspace = Document.GetService(typeof(IWorkspace)) as IWorkspace;
                }

                // Defines Data Template for 'SourceProperty' dependency property.
                var dt = new DataTemplate();
                var factory = new FrameworkElementFactory(typeof(SourceFilePropertyEditor));
                factory.SetValue(SourceFilePropertyEditor.WorkspaceProperty, Workspace);
                factory.SetValue(SourceFilePropertyEditor.CopyOptionProperty, SourceFileCopyOption.Ask);
                factory.SetValue(SourceFilePropertyEditor.DefaultFolderProperty, SpecialFolders.Images);
                dt.DataType = typeof(Uri);
                dt.VisualTree = factory;
                mapDataTemplates.Add(SourceProperty, dt);

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
                if (propertyName == "Source")
                {
                    if (Source != null)
                    {
                        if (!Source.IsValidFile())
                            return Properties.Resources.InvalidSource;
                        if (Source.IsAbsoluteUri && !Source.IsFile)
                           return Properties.Resources.InvalidSource;
                    }
                }

                return null;
            }
        }

        #endregion
        #region IDisposable Member
        bool bDispose;
        public void Dispose()
        {
            if (bDispose)
                return;
            bDispose = true;

            mediaElement.LoadedBehavior = MediaState.Manual;
            mediaElement.Stop();
            mediaElement.Source = null;
            mediaElement.Close();

            try
            {
                if (!string.IsNullOrEmpty(tempFileName) && System.IO.File.Exists(tempFileName))
                    System.IO.File.Delete(tempFileName);
            }
            catch
            {
            }
        }
        #endregion
    }
}
