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
using Utilities.WPF;
using OPCUABrowser.ComponentService;
using OPCUAViewModelService.ComponentService;
using System.Windows.Controls.Primitives;
using System.Windows.Media.Effects;
using System.Windows.Threading;
using Utilities;
using DocumentManager.ComponentService;
using UFUAEditor.ComponentService;
using System.Threading.Tasks;
using WPFUtilities;

namespace OPCUAViewModel.PropertyDataTemplate
{
    /// <summary>
    /// Interaction logic for OPCUAEntityReferencePropertyEditor.xaml
    /// </summary>
    public partial class OPCUAEntityReferencePropertyEditor : UserControl
    {
        #region Dependency Properties

        #region AllowDataSync
        public static readonly DependencyProperty AllowDataSyncProperty = DependencyProperty.Register("AllowDataSync", typeof(bool), typeof(OPCUAEntityReferencePropertyEditor), new UIPropertyMetadata(true, new PropertyChangedCallback(OnAllowDataSyncChanged), new CoerceValueCallback(OnCoerceAllowDataSync)));

        private static object OnCoerceAllowDataSync(DependencyObject o, object value)
        {
            OPCUAEntityReferencePropertyEditor control = o as OPCUAEntityReferencePropertyEditor;
            if (control != null)
                return control.OnCoerceAllowDataSync((bool)value);
            else
                return value;
        }

        private static void OnAllowDataSyncChanged(DependencyObject o, DependencyPropertyChangedEventArgs e)
        {
            OPCUAEntityReferencePropertyEditor control = o as OPCUAEntityReferencePropertyEditor;
            if (control != null)
                control.OnAllowDataSyncChanged((bool)e.OldValue, (bool)e.NewValue);
        }

        protected virtual bool OnCoerceAllowDataSync(bool value)
        {
            // TODO: Keep the proposed value within the desired range.
            return value;
        }

        protected virtual void OnAllowDataSyncChanged(bool oldValue, bool newValue)
        {
            // TODO: Add your property changed side-effects. Descendants can override as well.
        }

        public bool AllowDataSync
        {
            // IMPORTANT: To maintain parity between setting a property in XAML and procedural code, do not touch the getter and setter inside this dependency property!
            get
            {
                return (bool)GetValue(AllowDataSyncProperty);
            }
            set
            {
                SetValue(AllowDataSyncProperty, value);
            }
        }
        #endregion

        #region AllowRemoteServer
        public static readonly DependencyProperty AllowRemoteServerProperty = DependencyProperty.Register("AllowRemoteServer", typeof(bool), typeof(OPCUAEntityReferencePropertyEditor), new UIPropertyMetadata(true, new PropertyChangedCallback(OnAllowRemoteServerChanged), new CoerceValueCallback(OnCoerceAllowRemoteServer)));

        private static object OnCoerceAllowRemoteServer(DependencyObject o, object value)
        {
            OPCUAEntityReferencePropertyEditor control = o as OPCUAEntityReferencePropertyEditor;
            if (control != null)
                return control.OnCoerceAllowRemoteServer((bool)value);
            else
                return value;
        }

        private static void OnAllowRemoteServerChanged(DependencyObject o, DependencyPropertyChangedEventArgs e)
        {
            OPCUAEntityReferencePropertyEditor control = o as OPCUAEntityReferencePropertyEditor;
            if (control != null)
                control.OnAllowRemoteServerChanged((bool)e.OldValue, (bool)e.NewValue);
        }

        protected virtual bool OnCoerceAllowRemoteServer(bool value)
        {
            // TODO: Keep the proposed value within the desired range.
            return value;
        }

        protected virtual void OnAllowRemoteServerChanged(bool oldValue, bool newValue)
        {
            // TODO: Add your property changed side-effects. Descendants can override as well.
        }

        public bool AllowRemoteServer
        {
            // IMPORTANT: To maintain parity between setting a property in XAML and procedural code, do not touch the getter and setter inside this dependency property!
            get
            {
                return (bool)GetValue(AllowRemoteServerProperty);
            }
            set
            {
                SetValue(AllowRemoteServerProperty, value);
            }
        }
        #endregion


        #region DesignMode
        public static readonly DependencyProperty DesignModeProperty = DependencyProperty.Register("DesignMode", typeof(bool), typeof(OPCUAEntityReferencePropertyEditor), new UIPropertyMetadata(true));
        public bool DesignMode
        {
            // IMPORTANT: To maintain parity between setting a property in XAML and procedural code, do not touch the getter and setter inside this dependency property!
            get
            {
                return (bool)GetValue(DesignModeProperty);
            }
            set
            {
                SetValue(DesignModeProperty, value);
            }
        }
        #endregion


        #region Skin
        public static readonly DependencyProperty SkinProperty = DependencyProperty.Register("Skin", typeof(string), typeof(OPCUAEntityReferencePropertyEditor), new UIPropertyMetadata(null));
        public string Skin
        {
            // IMPORTANT: To maintain parity between setting a property in XAML and procedural code, do not touch the getter and setter inside this dependency property!
            get
            {
                return (string)GetValue(SkinProperty);
            }
            set
            {
                SetValue(SkinProperty, value);
            }
        }
        #endregion


        #region Document
        public static readonly DependencyProperty DocumentProperty = DependencyProperty.Register("Document", typeof(IDocument), typeof(OPCUAEntityReferencePropertyEditor), new UIPropertyMetadata(null));
        public IDocument Document
        {
            // IMPORTANT: To maintain parity between setting a property in XAML and procedural code, do not touch the getter and setter inside this dependency property!
            get
            {
                return (IDocument)GetValue(DocumentProperty);
            }
            set
            {
                SetValue(DocumentProperty, value);
            }
        }

        #endregion


        #endregion
        bool bLoaded;
        public OPCUAEntityReferencePropertyEditor()
        {
            InitializeComponent();
             
            Loaded += (o, e) =>
            {
                if (!bLoaded)
                {
                    bLoaded = true;

                    if (Document != null)
                        ThemeImageHelper.LoadBitmapImageResourceDictionary(this.Resources, Document, !DesignMode);

                    cancelImg.SetResourceReference(Image.SourceProperty, "CancelSmall");
                    editImg.SetResourceReference(Image.SourceProperty, "EditGeneralSmall");

                    var currentStyle = string.IsNullOrEmpty(Skin) ? ApplicationPropertiesHelper.GetProperty<String>("CurrentSkin") : Skin;
                    if (!string.IsNullOrEmpty(currentStyle))
                        ThemeHelper.SetTheme(this, currentStyle);
                }
            };
        }

        private void DlgButton_Click(object sender, RoutedEventArgs e)
        {
            OPCUAEntityReference value = (OPCUAEntityReference)(uriButton.Tag);
            if (value == null)
                value = new OPCUAEntityReference(null);

            if (OPCUAViewModelComponent.workspaceService.ContextDocument != null &&
                OPCUAViewModelComponent.workspaceService.ContextDocument is IDocument)
            {
                var doc = OPCUAViewModelComponent.workspaceService.ContextDocument as IDocument;
                value.Editor = doc.GetService(typeof(IUFUAEditorManager)) as IUFUAEditorManager;
                value.Document = OPCUAViewModelComponent.workspaceService.ContextDocument;
            }

            bEditing = false;
            if (value.Edit(sync: true, localserver: !AllowRemoteServer, noDataSinks: !AllowDataSync))
            {
                if (value.HasValidValue)
                    Dispatcher.CurrentDispatcher.BeginInvokeAsynchronouslyInBackground(() =>
                        {
                            OPCUAEntityReference newValue = new OPCUAEntityReference(value);
                            uriButton.Tag = newValue;
                            uriLabel.Text = newValue.StringRepresentationWithProject;
                        });
            }
        }

        private void DlgButton_Click_Clear(object sender, RoutedEventArgs e)
        {
            uriButton.Tag = null;
            uriButton.GetBindingExpression(Button.TagProperty).UpdateSource();
            uriLabel.Text = String.Empty;
        }

        OPCUAEntityReference original;
        private async void uriLabel_LostFocus(object sender, RoutedEventArgs e)
        {
            if (uriLabel.Text == null || !bEditing || !OPCUAViewModelComponent.workspaceServiceAvailable)
            {
                bEditing = false;
                if (uriButton.Tag as OPCUAEntityReference != null && (uriButton.Tag as OPCUAEntityReference).StringRepresentation == uriLabel.Text)
                {
                    uriLabel.Text = (uriButton.Tag as OPCUAEntityReference).StringRepresentationWithProject;
                    oldText = null;
                }
                return;
            }
            bEditing = false;
            IDocument doc = OPCUAViewModelComponent.workspaceService.ContextDocument as IDocument;
            if (doc == null)
                return;

            var editor = doc.GetService(typeof(IUFUAEditorManager)) as IUFUAEditorManager;
            if (editor == null)
                return;

            if (String.IsNullOrEmpty(uriLabel.Text))
            {
                uriButton.Tag = original = null;
                return;
            }

            var split = uriLabel.Text.Split(':');
            var instance = split[0]?.Replace('/', '\\');
            var tagName = split[0];
            if (split.Length > 1)
                tagName = split[1];
            else
                instance = null;

            if (original == null)
                original = uriButton.Tag as OPCUAEntityReference;
            var xml = editor.GetTagEntityReference(doc, tagName, instance);
            if (xml == null)
            {
                var datasync = OPCUAEntityReference.GetDataSinkInterface("TemporaryVariables");
                if (datasync != null)
                {
                    var relPath = tagName.Replace('\\', '&').Replace('/', '&');
                    datasync.GetVariables();
                    var tagRef = datasync.GetReference(relPath);
                    if (!String.IsNullOrEmpty(tagRef.ReadablePath))
                        xml = tagRef.ToXml();
                }
            }
            if (String.IsNullOrEmpty(xml))
            {
                if (original == null)
                {
                    original = new OPCUAEntityReference(null);
                    original.HumanReadable = original.RelativePath = original.ReadablePath = uriLabel.Text;
                    uriButton.Tag = original;
                }
                else
                    original.HumanReadable = original.RelativePath = original.ReadablePath = uriLabel.Text;

                original.ResolvedNodeId = null;

                var oldColor = uriLabel.Foreground;
                uriLabel.Foreground = Brushes.Red;
                await Task.Delay(TimeSpan.FromMilliseconds(100));
                uriLabel.Foreground = oldColor;
                await Task.Delay(TimeSpan.FromMilliseconds(100));
                uriLabel.Foreground = Brushes.Red;
                await Task.Delay(TimeSpan.FromMilliseconds(100));
                uriLabel.Foreground = oldColor;
                /*
                if (!String.IsNullOrEmpty(oldText))
                {
                    uriLabel.Text = oldText;
                    oldText = null;
                }
                */
            }
            else
            {
                var tag = xml.FromXml<OPCUAEntityReference>();
                uriButton.Tag = tag;
                uriButton.GetBindingExpression(Button.TagProperty).UpdateSource();

                oldText = null;
                uriLabel.Text = tag.StringRepresentationWithProject;

                if (original != null)
                    original.UpdateValue(tag);
            }
        }

        bool bEditing;
        String oldText;
        private void uriLabel_KeyDown(object sender, KeyEventArgs e)
        {
            bEditing = true;

            FillComboBox(e.Key == Key.F5);
        }

        private void uriLabel_GotFocus(object sender, RoutedEventArgs e)
        {
            if (uriButton.Tag != null && uriButton.Tag is OPCUAEntityReference)
            {
                var reference = uriButton.Tag as OPCUAEntityReference;
                if (String.IsNullOrEmpty(oldText) && 
                   (uriLabel.Text.Contains(reference.HumanReadable) ||
                    uriLabel.Text.Contains(reference.HumanReadable.Replace('\\', '/').Replace('&', '/')) ||
                    uriLabel.Text.Contains(reference.HumanReadable.Replace('/', '\\').Replace('&', '\\'))))
                {
                    oldText = uriLabel.Text;
                    var s = reference.StringRepresentation;
                    if (!String.IsNullOrEmpty(s))
                        uriLabel.Text = s;
                }
            }

            uriLabel.SelectAll();
        }

        private void uriLabel_PopupOpening(object sender, DevExpress.Xpf.Editors.OpenPopupEventArgs e)
        {
            FillComboBox();
        }

        bool bFilled;
        void FillComboBox(bool bForceRefresh = false)
        {
            if ((bFilled && !bForceRefresh) || !OPCUAViewModelComponent.workspaceServiceAvailable)
                return;
            IDocument doc = OPCUAViewModelComponent.workspaceService.ContextDocument as IDocument;
            if (doc == null)
                return;
            var editor = doc.GetService(typeof(IUFUAEditorManager)) as IUFUAEditorManager;
            if (editor == null)
                return;
            bFilled = true;
            progressBar.Visibility = Visibility.Visible;
            var task = Task.Factory.StartNew(() =>
            {
                return editor.GetFlatFullTagNameCollectionOrderByName(doc, false, bForceRefresh);
            });
            task.ContinueWith(ret =>
            {
                uriLabel.ItemsSource = ret.Result;
                progressBar.Visibility = Visibility.Collapsed;
                uriLabel.IsPopupOpen = true;
            }, TaskScheduler.FromCurrentSynchronizationContext());
        }

        private void uriLabel_SelectedIndexChanged(object sender, RoutedEventArgs e)
        {
            bEditing = true;
        }
    }
}
