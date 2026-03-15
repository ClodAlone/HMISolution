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
using Utilities;
using Utilities.WPF;
using OPCUABrowser.ComponentService;
using OPCUAViewModelService.ComponentService;
using System.Windows.Controls.Primitives;
using System.Windows.Media.Effects;
using System.Windows.Threading;
using DocumentManager.ComponentService;
using UFUAEditor.ComponentService;
using System.Threading.Tasks;
using WPFUtilities;

namespace OPCUAViewModel.PropertyDataTemplate
{
    /// <summary>
    /// Interaction logic for OPCUAEntityReferencePropertyEditorFromXML.xaml
    /// </summary>
    public partial class OPCUAEntityReferencePropertyEditorFromXML : UserControl
    {
        #region Dependency Properties

        #region AllowDataSync
        public static readonly DependencyProperty AllowDataSyncProperty = DependencyProperty.Register("AllowDataSync", typeof(bool), typeof(OPCUAEntityReferencePropertyEditorFromXML), new UIPropertyMetadata(true, new PropertyChangedCallback(OnAllowDataSyncChanged), new CoerceValueCallback(OnCoerceAllowDataSync)));

        private static object OnCoerceAllowDataSync(DependencyObject o, object value)
        {
            OPCUAEntityReferencePropertyEditorFromXML control = o as OPCUAEntityReferencePropertyEditorFromXML;
            if (control != null)
                return control.OnCoerceAllowDataSync((bool)value);
            else
                return value;
        }

        private static void OnAllowDataSyncChanged(DependencyObject o, DependencyPropertyChangedEventArgs e)
        {
            OPCUAEntityReferencePropertyEditorFromXML control = o as OPCUAEntityReferencePropertyEditorFromXML;
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
        public static readonly DependencyProperty AllowRemoteServerProperty = DependencyProperty.Register("AllowRemoteServer", typeof(bool), typeof(OPCUAEntityReferencePropertyEditorFromXML), new UIPropertyMetadata(true, new PropertyChangedCallback(OnAllowRemoteServerChanged), new CoerceValueCallback(OnCoerceAllowRemoteServer)));

        private static object OnCoerceAllowRemoteServer(DependencyObject o, object value)
        {
            OPCUAEntityReferencePropertyEditorFromXML control = o as OPCUAEntityReferencePropertyEditorFromXML;
            if (control != null)
                return control.OnCoerceAllowRemoteServer((bool)value);
            else
                return value;
        }

        private static void OnAllowRemoteServerChanged(DependencyObject o, DependencyPropertyChangedEventArgs e)
        {
            OPCUAEntityReferencePropertyEditorFromXML control = o as OPCUAEntityReferencePropertyEditorFromXML;
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

        #endregion
        bool bLoaded;
        public OPCUAEntityReferencePropertyEditorFromXML()
        {
            InitializeComponent();

            Loaded += (o, e) =>
            {
                if (!bLoaded)
                {
                    bLoaded = true;
                    var currentStyle = ApplicationPropertiesHelper.GetProperty<String>("CurrentSkin");
                    if (!string.IsNullOrEmpty(currentStyle))
                        ThemeHelper.SetTheme(uriLabel, currentStyle);
                }
            };
        }

        private void DlgButton_Click(object sender, RoutedEventArgs e)
        {

            string xml = uriButton.Tag as string;
            OPCUAEntityReference value = null;
            if (!string.IsNullOrEmpty(xml))
                value = xml.FromXml<OPCUAEntityReference>();

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
                        uriButton.Tag = newValue.ToXml();
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

        void Empty_uriLabel()
        {
            if (!String.IsNullOrEmpty(oldText))
            {
                uriLabel.Text = oldText;
                oldText = null;
            }
        }

        private async void uriLabel_LostFocus(object sender, RoutedEventArgs e)
        {
            if (uriLabel.Text == null || !bEditing || !OPCUAViewModelComponent.workspaceServiceAvailable)
            {
                bEditing = false;
                Empty_uriLabel();
                return;
            }
            bEditing = false;
            IDocument doc = OPCUAViewModelComponent.workspaceService.ContextDocument as IDocument;
            if (doc == null)
            {
                Empty_uriLabel();
                return;
            }

            var editor = doc.GetService(typeof(IUFUAEditorManager)) as IUFUAEditorManager;
            if (editor == null)
                return;

            var split = uriLabel.Text.Split(':');
            var instance = split[0];
            var name = split[0];
            if (split.Length > 1)
                name = split[1];
            else
                instance = null;

            var xml = editor.GetTagEntityReference(doc, name, instance);
            if (String.IsNullOrEmpty(xml))
            {
                var oldColor = uriLabel.Foreground;
                uriLabel.Foreground = Brushes.Red;
                await Task.Delay(TimeSpan.FromMilliseconds(100));
                uriLabel.Foreground = oldColor;
                await Task.Delay(TimeSpan.FromMilliseconds(100));
                uriLabel.Foreground = Brushes.Red;
                await Task.Delay(TimeSpan.FromMilliseconds(100));
                uriLabel.Foreground = oldColor;

                if (!String.IsNullOrEmpty(oldText))
                {
                    uriLabel.Text = oldText;
                    oldText = null;
                }
            }
            else
            {
                var tag = xml.FromXml<OPCUAEntityReference>();
                uriButton.Tag = xml;
                uriButton.GetBindingExpression(Button.TagProperty).UpdateSource();

                oldText = null;
                uriLabel.Text = tag.StringRepresentationWithProject;
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
            if (uriButton.Tag != null && uriButton.Tag is string)
            {
                if (String.IsNullOrEmpty(oldText) && !string.IsNullOrEmpty((string)uriButton.Tag))
                {
                    try
                    {
                        OPCUAEntityReference tag = (uriButton.Tag as string).FromXml<OPCUAEntityReference>();
                        var reference = tag;

                        if (String.IsNullOrEmpty(oldText) &&
                           (uriLabel.Text.Contains(reference.HumanReadable) ||
                            uriLabel.Text.Contains(reference.HumanReadable.Replace("\\", "/")) ||
                            uriLabel.Text.Contains(reference.HumanReadable.Replace('/', '\\').Replace('&', '\\'))))
                        {
                            oldText = uriLabel.Text;
                            var s = reference.StringRepresentation;
                            if (!String.IsNullOrEmpty(s))
                                uriLabel.Text = s;
                        }
                    }
                    catch (Exception)
                    {
                    }
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
