using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
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
using WebBrowser.PropertyDataTemplate;
using Utilities;
using System.Windows.Interop;
using System.Reflection;
using DocumentManager.ComponentService;
using UIMsgBoxAlertService.ComponentService;
using Microsoft.Win32;
using System.Xml.Serialization;
using EditSettingsHelper;
using EditSettingsHelper.ComponentService;
using WPFUtilities.Extensions;
using ScreenSettings;

namespace WebBrowser
{
    /// <summary>
    /// Interaction logic for UserControl1.xaml
    /// </summary>
    public partial class WebBrowserControl : UserControl, IContainPropertyEditors, IDataErrorInfo, IDisposable, ISettingsHelper
    {
        #region DP
        [SvgValueConverter(ConverterType = typeof(ConvertDefaultBrushValue), HasOverrideBrushProperties = true, RequiredKey = true)]
        [Browsable(false)]
        [XmlIgnore]
        public bool FooProperty { get { return false; } }
        #region OverrideBaseProperties

        private void OverrideBaseProperties()
        {
            DependencyPropertyDescriptor dpd;

            dpd = DependencyPropertyDescriptor.FromProperty(BackgroundProperty, typeof(WebBrowserControl));
            dpd.AddValueChangedSafe(this, OnBackgroundChanged);
        }

        private void DetachOverrideBaseProperties()
        {
            DependencyPropertyDescriptor dpd;

            dpd = DependencyPropertyDescriptor.FromProperty(BackgroundProperty, typeof(WebBrowserControl));
            dpd.RemoveValueChangedSafe(this, OnBackgroundChanged);
        }
        private void OnBackgroundChanged(object sender, EventArgs e)
        {
            var control = sender as WebBrowserControl;
            if (control != null)
            {
                control.OnBackgroundChanged();
            }
        }
        protected virtual void OnBackgroundChanged()
        {
            // TODO: Add your property changed side-effects. Descendants can override as well.
            if (bInit && !IsManipulationEnabled)
                ControlBackground = Background;
        }

        #endregion

        #region EditingWriteAccessLevel
        public static readonly DependencyProperty EditingWriteAccessLevelProperty = DependencyProperty.Register("EditingWriteAccessLevel", typeof(int), typeof(WebBrowserControl), new UIPropertyMetadata(0));
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
        public static readonly DependencyProperty EditingWriteAccessMaskProperty = DependencyProperty.Register("EditingWriteAccessMask", typeof(int), typeof(WebBrowserControl), new UIPropertyMetadata(0));
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
        public static readonly DependencyProperty UserBasedRuntimeSettingsProperty = DependencyProperty.Register("UserBasedRuntimeSettings", typeof(bool), typeof(WebBrowserControl), new UIPropertyMetadata(false));
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

        #region ControlBackground
        public static readonly DependencyProperty ControlBackgroundProperty = DependencyProperty.Register("ControlBackground", typeof(Brush), typeof(WebBrowserControl), new UIPropertyMetadata(new SolidColorBrush(Color.FromArgb(255,51,51,51))));
        [Browsable(false)]
        [XmlIgnore]
        public Brush ControlBackground
        {
            // IMPORTANT: To maintain parity between setting a property in XAML and procedural code, do not touch the getter and setter inside this dependency property!
            get
            {
                return (Brush)GetValue(ControlBackgroundProperty);
            }
            set
            {
                SetValue(ControlBackgroundProperty, value);
            }
        }

        #endregion


        public static readonly DependencyProperty UriProperty = DependencyProperty.Register("Source", typeof(Uri), typeof(WebBrowserControl), new UIPropertyMetadata(null, new PropertyChangedCallback(OnUriChanged), new CoerceValueCallback(OnCoerceUri)));

        private static object OnCoerceUri(DependencyObject o, object value)
        {
            WebBrowserControl WebBrowserControl = o as WebBrowserControl;
            if (WebBrowserControl != null)
                return WebBrowserControl.OnCoerceUri((Uri)value);
            else
                return value;
        }

        private static void OnUriChanged(DependencyObject o, DependencyPropertyChangedEventArgs e)
        {
            WebBrowserControl WebBrowserControl = o as WebBrowserControl;
            if (WebBrowserControl != null)
                WebBrowserControl.OnUriChanged((Uri)e.OldValue, (Uri)e.NewValue);
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
        System.Windows.Controls.WebBrowser BrowserWindow;
        Helper helper;
        #endregion

        public WebBrowserControl()
        {
            InitializeComponent();

            ScreenSettings.ScreenDocument.SetAutoForceDynamicOnClient(this, true);

            Loaded += (o, e) =>
                {
                    if (!bLoaded && !bDispose)
                    {
                        bLoaded = true;
                        OverrideBaseProperties();
                        Document = ScreenSettings.ScreenDocument.GetScreenDocument(this);
                        ThemeImageHelper.LoadBitmapImageResourceDictionary(this.Resources, Document);
                        if (Document != null)
                            UIMsgBoxAlertService = Document.GetService(typeof(IUIMsgBoxAlertService)) as IUIMsgBoxAlertService;
                        if (bDesignmode || DesignerProperties.GetIsInDesignMode(this))
                        {
                            bDesignmode = true;
                            view.IsHitTestVisible = false;
                        }
                        else
                        {
                            helper = new Helper(Document, this as ISettingsHelper);
                            helper.RefreshCurrentUser();
                            EnsureSourceValue();
                            BrowserWindow = new System.Windows.Controls.WebBrowser() { Cursor = Cursors.Arrow};
                            if(BrowserWindow != null)
                            {
                                SetBrowserEmulation(RegistryHive.CurrentUser);
                                BrowserWindow.Navigated += BrowserWindow_Navigated;
                                BrowserWindow.ClearValue(FrameworkElement.WidthProperty);
                                BrowserWindow.ClearValue(FrameworkElement.HeightProperty);
                                Container.Content = BrowserWindow;
                            }
                            if (Source != null)
                            {
                                NavigateURL(Source);
                                txtLoad.Text = Source.OriginalString;
                            }
                        }
                        bInit = true;
                    }
                };

        }

        private void SetBrowserEmulation(RegistryHive regHive)
        {
            string path = "Software\\Microsoft\\Internet Explorer\\Main\\FeatureControl\\FEATURE_BROWSER_EMULATION";
            string appName = System.IO.Path.GetFileName(System.Diagnostics.Process.GetCurrentProcess().MainModule.FileName);
            try
            {
                using (var hklm = Microsoft.Win32.RegistryKey.OpenBaseKey(regHive, Environment.Is64BitOperatingSystem ? RegistryView.Registry64 : RegistryView.Registry32))
                {
                    using (var key = hklm.OpenSubKey(path, true))
                    {
                        if (key != null)
                            key.SetValue(appName, 11001, RegistryValueKind.DWord); //emulate latest IE version
                    }
                }
            }
            catch (Exception)
            {
            }
        }

        private void BrowserWindow_Navigated(object sender, NavigationEventArgs e)
        {
            SuppressJavaScriptErrors(sender);
            txtLoad.Text = e.Uri.OriginalString;
        }

        private void SuppressJavaScriptErrors(object sender)
        {
            dynamic activeX = sender.GetType().InvokeMember("ActiveXInstance",
                                                     BindingFlags.GetProperty | BindingFlags.Instance | BindingFlags.NonPublic,
                                                     null, this.BrowserWindow, new object[] { });
            if (activeX != null)
                activeX.Silent = true;
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

        private void btnPrevious_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                BrowserWindow.GoBack();
            }
            catch (Exception ex)
            {
            }
        }

        private void btnNext_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                BrowserWindow.GoForward();
            }
            catch (Exception ex)
            {
            }
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

        private void NavigateURL(Uri Url)
        {
            bool bShowError = false;
            try
            {
                BrowserWindow.Navigate(Url);
            }
            catch(Exception ex)
            {
                try
                {
                    string urlorig = Url.OriginalString;
                    if (!urlorig.StartsWith("http"))
                    {
                        Uri uri;
                        if ((Uri.TryCreate("http://" + Url, UriKind.Absolute, out uri)) &&
                            (uri.Scheme == Uri.UriSchemeHttp || uri.Scheme == Uri.UriSchemeHttps))
                            BrowserWindow.Navigate(uri);
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

            DetachOverrideBaseProperties();

            if (bLoaded && Source != null)
            {
                bLoaded = false;
            }

            if (BrowserWindow != null)
            {
                BrowserWindow.Navigated -= BrowserWindow_Navigated;

                try
                {
                    IKeyboardInputSite keyboardInputSite = ((IKeyboardInputSink)BrowserWindow).KeyboardInputSite;
                    if (keyboardInputSite != null)
                    {
                        keyboardInputSite.Unregister();

                        Type type = keyboardInputSite.GetType();
                        FieldInfo fieldInfo = type.GetField("_sinkElement", BindingFlags.NonPublic | BindingFlags.Instance);
                        if (fieldInfo != null)
                        {
                            fieldInfo.SetValue(keyboardInputSite, null);
                        }
                    }
                }
                catch
                {
                    //Catch everything, this is a nasty hack to dispose the WebBrowser
                }

                BrowserWindow.Dispose();
            }
        }

        #endregion
    }

    internal class ConvertDefaultBrushValue : CustomValueConverter
    {
        public override object ConvertFromStorageType(object value, object sender)
        {
            return new Dictionary<string, Brush>();
        }

        public override object ConvertFromStorageType(object value, object sender, object document)
        {
            Dictionary<string, Brush> ret = new Dictionary<string, Brush>();
            if (sender is UserControl)
            {
                UserControl control = sender as UserControl;
                Brush foreground = WPFUtilities.DeployHelper.GetBorderResources<Border>(sender as FrameworkElement, "editArea", (document as ScreenDocument).Theme)?.BorderBrush; ;
                Brush background = WPFUtilities.DeployHelper.GetBorderResources<Border>(sender as FrameworkElement, "editArea", (document as ScreenDocument).Theme)?.Background;
                if (control.ReadLocalValue(UserControl.ForegroundProperty) != DependencyProperty.UnsetValue)
                    ret.Add("Foreground", control.Foreground);
                else
                    ret.Add("Foreground", foreground);

                if (control.ReadLocalValue(UserControl.BackgroundProperty) != DependencyProperty.UnsetValue && control.Background != null)
                    ret.Add("Background", control.Background);
                else
                    ret.Add("Background", background);
            }
            return ret;
        }

        public override object ConvertToStorageType(object value, object sender, object document, object property, object parameter = null)
        {
            if (sender == null)
                return null;

            return value;
        }
        public override Type StorageType
        {
            get
            {
                return typeof(string);
            }
        }
    }
}
