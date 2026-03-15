using DocumentManager.ComponentService;
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
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using UFInterfaces.PropertyControl;
using UIMsgBoxAlertService.ComponentService;
using WebBrowser.PropertyDataTemplate;
using Utilities;
using EditSettingsHelper;
using EditSettingsHelper.ComponentService;
using Microsoft.Web.WebView2.Wpf;
using Microsoft.Web.WebView2.Core;
using System.Xml.Serialization;

namespace WebBrowser
{
    /// <summary>
    /// Interaction logic for WebViewControl.xaml
    /// </summary>
    [SvgValueConverter(TypeName = "WebBrowserControl")]
    public partial class WebViewControl : UserControl, IContainPropertyEditors, IDataErrorInfo, IDisposable, ISettingsHelper
    {
        #region Dependency Properties

        [SvgValueConverter(ConverterType = typeof(ConvertDefaultBrushValue), HasOverrideBrushProperties = true, RequiredKey = true)]
        [Browsable(false)]
        [XmlIgnore]
        public bool FooProperty { get { return false; } }
        #region EditingWriteAccessLevel
        public static readonly DependencyProperty EditingWriteAccessLevelProperty = DependencyProperty.Register("EditingWriteAccessLevel", typeof(int), typeof(WebViewControl), new UIPropertyMetadata(0));
        public int EditingWriteAccessLevel
        {
            // IMPORTANT: To maintain parity between setting a property in XAML and procedural code, do not touch the getter and setter inside this dependency property!
            get
            {
                return (int)GetValue(EditingWriteAccessLevelProperty);
            }
            set
            {
                SetValue(EditingWriteAccessLevelProperty, value);
            }
        }
        #endregion

        #region EditingWriteAccessMask
        public static readonly DependencyProperty EditingWriteAccessMaskProperty = DependencyProperty.Register("EditingWriteAccessMask", typeof(int), typeof(WebViewControl), new UIPropertyMetadata(0));
        public int EditingWriteAccessMask
        {
            // IMPORTANT: To maintain parity between setting a property in XAML and procedural code, do not touch the getter and setter inside this dependency property!
            get
            {
                return (int)GetValue(EditingWriteAccessMaskProperty);
            }
            set
            {
                SetValue(EditingWriteAccessMaskProperty, value);
            }
        }
        #endregion

        #region UserBasedRuntimeSettings
        public static readonly DependencyProperty UserBasedRuntimeSettingsProperty = DependencyProperty.Register("UserBasedRuntimeSettings", typeof(bool), typeof(WebViewControl), new UIPropertyMetadata(false));
        public bool UserBasedRuntimeSettings
        {
            // IMPORTANT: To maintain parity between setting a property in XAML and procedural code, do not touch the getter and setter inside this dependency property!
            get
            {
                return (bool)GetValue(UserBasedRuntimeSettingsProperty);
            }
            set
            {
                SetValue(UserBasedRuntimeSettingsProperty, value);
            }
        }
        #endregion
        #region UserBasedRuntimeSettingsList
        [XmlIgnore]
        [Browsable(false)]
        [SvgValueConverter(ConverterType = typeof(ConvertUserBasedSettingsList), RequiredKey = true)]
        public List<Settings> UserBasedRuntimeSettingsList
        {
            get;
            set;
        }
        #endregion

        public static readonly DependencyProperty UriProperty = DependencyProperty.Register("Source", typeof(Uri), typeof(WebViewControl), new UIPropertyMetadata(null, new PropertyChangedCallback(OnUriChanged), new CoerceValueCallback(OnCoerceUri)));

        private static object OnCoerceUri(DependencyObject o, object value)
        {
            WebViewControl WebViewControl = o as WebViewControl;
            if (WebViewControl != null)
                return WebViewControl.OnCoerceUri((Uri)value);
            else
                return value;
        }

        private static void OnUriChanged(DependencyObject o, DependencyPropertyChangedEventArgs e)
        {
            WebViewControl WebViewControl = o as WebViewControl;
            if (WebViewControl != null)
                WebViewControl.OnUriChanged((Uri)e.OldValue, (Uri)e.NewValue);
        }

        protected virtual Uri OnCoerceUri(Uri value)
        {
            // TODO: Keep the proposed value within the desired range.
            return value;
        }

        protected virtual void OnUriChanged(Uri oldValue, Uri newValue)
        {
            if (!bDesignmode && oldValue != newValue && bInit)
            {
                if (newValue != null)
                {
                    NavigateURL(newValue);
                    txtLoad.Text = newValue.OriginalString;
                }
            }
        }

        public Uri Source
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
        #endregion

        #region Declarations
        IUIMsgBoxAlertService UIMsgBoxAlertService;
        IDocument Document;
        bool bLoaded;
        bool bDesignmode;
        bool bInit;
        WebView2 wvc;
        Helper helper;
        #endregion

        #region Constructor
        public WebViewControl()
        {
            InitializeComponent();

            Loaded += (o, e) =>
            {
                if (!bLoaded && !bDispose)
                {
                    bLoaded = true;
                    Document = ScreenSettings.ScreenDocument.GetScreenDocument(this);
                    ThemeImageHelper.LoadBitmapImageResourceDictionary(this.Resources, Document);
                    if (Document != null)
                        UIMsgBoxAlertService = Document.GetService(typeof(IUIMsgBoxAlertService)) as IUIMsgBoxAlertService;
                    if (bDesignmode || DesignerProperties.GetIsInDesignMode(this))
                    {
                        bDesignmode = true;
                        view.IsHitTestVisible = false;
                        Container.Content = new Rectangle() { Fill = new SolidColorBrush(Colors.Black), Opacity = 0.5 };
                        // NavigateURL(Source);
                        bInit = true;
                    }
                    else if (RunningOnServer)
                    {
                        view.IsHitTestVisible = false;
                        var grid = new Grid();
                        grid.Children.Add(new Rectangle() { Fill = new SolidColorBrush(Colors.Black), Opacity = 0.5 });
                        grid.Children.Add(new TextBlock()
                        {
                            Text = Properties.Resources.NotSupportedOnWebClient,
                            HorizontalAlignment = HorizontalAlignment.Center,
                            VerticalAlignment = VerticalAlignment.Center,
                            TextWrapping = TextWrapping.Wrap,
                            FontSize = 24.0
                        });
                        Container.Content = grid;
                        bInit = true;
                    }
                    else
                    {
                        helper = new Helper(Document, this as ISettingsHelper);
                        helper.RefreshCurrentUser();

                        EnsureSourceValue();
                        wvc = new WebView2();
                        wvc.CreationProperties = new CoreWebView2CreationProperties()
                        {
                            UserDataFolder = System.IO.Path.Combine(System.IO.Path.GetTempPath(),
                                System.IO.Path.GetRandomFileName())
                        };
                        InitializeAsync();
                        wvc.NavigationCompleted += Wvc_NavigationCompleted;
                        Container.Content = wvc;
                    }
                }
            };
        }
        #endregion

        #region Methods

        async void InitializeAsync()
        {
            try
            {
                if (wvc == null)
                    return;
                await wvc.EnsureCoreWebView2Async(null);
                wvc.CoreWebView2.WebMessageReceived += UpdateAddressBar;
                if (Source != null)
                    NavigateURL(Source);
                bInit = true;
            }
            catch (Exception ex)
            {

            }
        }

        private void btnPrevious_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (wvc != null)
                    wvc.GoBack();
            }
            catch (Exception ex)
            {
            }
        }

        private void btnNext_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (wvc != null)
                    wvc.GoForward();
            }
            catch (Exception ex)
            {
            }
        }

        private void textBox_PreviewMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            if (!txtLoad.IsKeyboardFocusWithin)
            {
                e.Handled = true;
                txtLoad.Focus();
            }
        }

        private void textBox_GotFocus(object sender, RoutedEventArgs e)
        {
            e.Handled = true;
            txtLoad.SelectAll();
        }

        private void textBox_GotKeyboardFocus(object sender, RoutedEventArgs e)
        {
            e.Handled = true;
            txtLoad.SelectAll();
        }

        private void Wvc_NavigationCompleted(object sender, Microsoft.Web.WebView2.Core.CoreWebView2NavigationCompletedEventArgs e)
        {
            txtLoad.Text = (sender as WebView2).Source.OriginalString;
            if (UIMsgBoxAlertService != null && e.WebErrorStatus == Microsoft.Web.WebView2.Core.CoreWebView2WebErrorStatus.HostNameNotResolved)
                UIMsgBoxAlertService.ShowError($"{Properties.Resources.UrlNotFound}");
        }

        void UpdateAddressBar(object sender, CoreWebView2WebMessageReceivedEventArgs args)
        {
            String uri = args.TryGetWebMessageAsString();
            txtLoad.Text = uri;
            if (wvc != null)
                wvc.CoreWebView2.PostWebMessageAsString(uri);
        }

        private void btnHome_Click(object sender, RoutedEventArgs e)
        {
            if (helper != null && helper.ValidateAccessLevel())
                return;
            NavigateURL(Source);
        }

        private void btnSetHome_Click(object sender, RoutedEventArgs e)
        {
            if (helper != null && helper.ValidateAccessLevel())
                return;
            Uri source = null;
            if (!string.IsNullOrEmpty(txtLoad.Text))
            {
                try
                {
                    source = new Uri(txtLoad.Text, UriKind.RelativeOrAbsolute);
                }
                catch (Exception)
                {
                }
            }
            else
            {
                source = new Uri("about:blank");
            }

            if (source != null)
            {
                StorageHelper.StorageHelper.SaveMemoryMap<Settings>(new Settings() { Source = source.OriginalString }, Document, Name, UserBasedRuntimeSettings ? helper.Username : null);
                Source = source;
            }
        }

        private void NavigateURL(Uri url)
        {
            if (wvc == null || url == null)
                return;

            bool bShowError = false;
            try
            {
                wvc.CoreWebView2.Navigate(url.OriginalString);
            }
            catch (Exception ex)
            {
                try
                {
                    string urlorig = url.OriginalString;
                    if (!urlorig.StartsWith("http"))
                    {
                        Uri uri;
                        if ((Uri.TryCreate("http://" + url, UriKind.Absolute, out uri)) &&
                            (uri.Scheme == Uri.UriSchemeHttp || uri.Scheme == Uri.UriSchemeHttps))
                            wvc.CoreWebView2.Navigate(uri.OriginalString);
                        else
                            bShowError = true;
                    }
                    else
                        bShowError = true;
                }
                catch
                {
                    bShowError = true;
                }

                if (bShowError && UIMsgBoxAlertService != null)
                    UIMsgBoxAlertService.ShowError($"{Properties.Resources.BrowserError}: {ex.Message}");
            }
        }

        private void NavigateURL(string Url)
        {
            if (!string.IsNullOrEmpty(Url))
            {
                try
                {

                    Uri ui = new Uri(txtLoad.Text, UriKind.RelativeOrAbsolute);
                    NavigateURL(ui);
                }
                catch
                {
                    if (UIMsgBoxAlertService != null)
                        UIMsgBoxAlertService.ShowError(Properties.Resources.InvalidUrl);
                }
            }
        }
        private void btnLoad_Click(object sender, RoutedEventArgs e)
        {
            if (helper != null && helper.ValidateAccessLevel())
                return;
            NavigateURL(txtLoad.Text.Trim());
        }
        #endregion

        #region ISettingsHelper
        public void Initialize()
        {
        }
        public void UpdateWriteAccessCommands()
        {

        }
        public void ReloadRuntimeSettings()
        {
            if (!UserBasedRuntimeSettings)
                return;
            EnsureSourceValue();
            NavigateURL(Source);
        }
        void EnsureSourceValue()
        {
            try
            {
                Settings sourceSetting = StorageHelper.StorageHelper.LoadMemoryMap<Settings>(Document, Name, helper.Username);
                Uri source = new Uri(sourceSetting.Source, UriKind.RelativeOrAbsolute);
                if (!Keyboard.IsKeyDown(Key.LeftCtrl) && !Keyboard.IsKeyDown(Key.RightCtrl))
                {
                    if (source != null)
                        Source = source;
                }
            }
            catch
            {
            }
        }
        #endregion

        [Browsable(false)]
        public bool RunningOnServer
        {
            // IMPORTANT: To maintain parity between setting a property in XAML and procedural code, do not touch the getter and setter inside this dependency property!
            get
            {
                return ScreenSettings.ScreenDocument.GetRunningOnServer(this);
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

                // Defines Data Template for 'UriProperty' dependency property.
                var dt = new DataTemplate();
                var factory = new FrameworkElementFactory(typeof(UriPropertyEditor));
                dt.DataType = typeof(Uri);
                dt.VisualTree = factory;
                mapDataTemplates.Add(UriProperty, dt);

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

            if (helper is IDisposable)
                (helper as IDisposable).Dispose();
            helper = null;

            if (bLoaded && Source != null)
            {
                bLoaded = false;
            }

            if (wvc != null)
            {
                wvc.NavigationCompleted -= Wvc_NavigationCompleted;
                if(wvc.CoreWebView2 != null)
                    wvc.CoreWebView2.WebMessageReceived -= UpdateAddressBar;
                wvc.Dispose();
                if (wvc.CreationProperties != null)
                {
                    if (!String.IsNullOrEmpty(wvc.CreationProperties.UserDataFolder))
                    {
                        try
                        {
                            System.IO.Directory.Delete(wvc.CreationProperties.UserDataFolder, true);
                        }
                        catch
                        { }
                    }
                }
            }
        }

        #endregion
    }
    public class Settings
    {
        public string Source { get; set; }
        public Settings() { }
    }

    class ConvertUserBasedSettingsList : CustomValueConverter
    {
        public override object ConvertFromStorageType(object value, object sender)
        {
            throw new NotImplementedException();
        }

        public override object ConvertFromStorageType(object value, object sender, object document)
        {
            throw new NotImplementedException();
        }

        public override object ConvertToStorageType(object value, object sender, object document, object property)
        {
            var control = (WebViewControl)sender;
            if (!control.UserBasedRuntimeSettings)
                return null;

            Dictionary<string, string> ret = new Dictionary<string, string>();
            try
            {
                var settings = StorageHelper.StorageHelper.LoadMemoryMaps<Settings>((IDocument)document, control.Name);
                foreach (var s in settings)
                    ret.Add(s.Key, s.Value.Source);
            }
            catch { }
            return ret;
        }
        public override Type StorageType
        {
            get
            {
                return typeof(List<Settings>);
            }
        }
    }
}
