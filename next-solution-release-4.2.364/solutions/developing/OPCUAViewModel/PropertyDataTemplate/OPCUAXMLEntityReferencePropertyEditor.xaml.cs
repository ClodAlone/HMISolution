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
    /// Interaction logic for OPCUAEntityReferencePropertyEditor.xaml
    /// </summary>
    public partial class OPCUAXMLEntityReferencePropertyEditor : UserControl
    {
        bool bLoaded;
        public OPCUAXMLEntityReferencePropertyEditor()
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
            OPCUAXMLEntityReference tag = (OPCUAXMLEntityReference)(uriButton.Tag);
            if (tag == null)
                tag = new OPCUAXMLEntityReference();
            if (tag.TagReference == null)
                tag.TagReference = new OPCUAEntityReference(null);

            OPCUAEntityReference value = tag.TagReference;

            if (OPCUAViewModelComponent.workspaceService.ContextDocument != null &&
                OPCUAViewModelComponent.workspaceService.ContextDocument is IDocument)
            {
                var doc = OPCUAViewModelComponent.workspaceService.ContextDocument as IDocument;
                value.Editor = doc.GetService(typeof(IUFUAEditorManager)) as IUFUAEditorManager;
                value.Document = OPCUAViewModelComponent.workspaceService.ContextDocument;
            }

            bEditing = false;
            if (value.Edit(sync: true))
            {
                if (value.HasValidValue)
                    Dispatcher.CurrentDispatcher.BeginInvokeAsynchronouslyInBackground(() =>
                        {
                            OPCUAXMLEntityReference newValue = new OPCUAXMLEntityReference();
                            newValue.TagReference = new OPCUAEntityReference(value);
                            uriButton.Tag = newValue;
                            uriLabel.Text = newValue.TagReference.StringRepresentationWithProject;
                        });
            }
        }

        private void DlgButton_Click_Clear(object sender, RoutedEventArgs e)
        {
            uriButton.Tag = new OPCUAXMLEntityReference();
            //uriButton.GetBindingExpression(Button.TagProperty).UpdateSource();
            uriLabel.Text = String.Empty;
        }

        OPCUAXMLEntityReference original;
        private async void uriLabel_LostFocus(object sender, RoutedEventArgs e)
        {
            if (uriLabel.Text == null || !bEditing || !OPCUAViewModelComponent.workspaceServiceAvailable)
            {
                bEditing = false;
                if (uriButton.Tag as OPCUAXMLEntityReference != null && (uriButton.Tag as OPCUAXMLEntityReference).TagReference != null && (uriButton.Tag as OPCUAXMLEntityReference).TagReference.StringRepresentation == uriLabel.Text)
                {
                    uriLabel.Text = (uriButton.Tag as OPCUAXMLEntityReference).TagReference.StringRepresentationWithProject;
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

            var split = uriLabel.Text.Split(':');
            var instance = split[0]?.Replace('/', '\\');
            var name = split[0];
            if (split.Length > 1)
                name = split[1];
            else
                instance = null;

            if (original == null)
                original = uriButton.Tag as OPCUAXMLEntityReference;
            var xml = editor.GetTagEntityReference(doc, name, instance);
            if (xml == null)
            {
                var datasync = OPCUAEntityReference.GetDataSinkInterface("TemporaryVariables");
                if (datasync != null)
                {
                    var relPath = name.Replace('\\', '&').Replace('/', '&');
                    datasync.GetVariables();
                    var tagRef = datasync.GetReference(relPath);
                    if (!String.IsNullOrEmpty(tagRef.ReadablePath))
                        xml = tagRef.ToXml();
                }
            }
            if (String.IsNullOrEmpty(xml))
            {
                original = new OPCUAXMLEntityReference();
                OPCUAEntityReference newTag = new OPCUAEntityReference(null);
                newTag.HumanReadable = newTag.RelativePath = newTag.ReadablePath = uriLabel.Text;
                newTag.ResolvedNodeId = null;
                original.TagReference = newTag;
                uriButton.Tag = original;
                /*
                if (!String.IsNullOrEmpty(oldText))
                {
                    uriLabel.Text = oldText;
                    oldText = null;
                }*/

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
                var tag = new OPCUAXMLEntityReference();
                tag.TagReference = xml.FromXml<OPCUAEntityReference>();
                uriButton.Tag = tag;
                uriButton.GetBindingExpression(Button.TagProperty).UpdateSource();

                oldText = null;
                uriLabel.Text = tag.TagReference.StringRepresentationWithProject;

                if (original != null)
                {
                    if(original.TagReference != null)
                        original.TagReference.UpdateValue(tag.TagReference);
                    else
                        original.TagReference = new OPCUAEntityReference(tag.TagReference);
                }
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
            if (uriButton.Tag != null && uriButton.Tag is OPCUAXMLEntityReference && (uriButton.Tag as OPCUAXMLEntityReference).TagReference is OPCUAEntityReference)
            {
                var reference = uriButton.Tag as OPCUAXMLEntityReference;
                if (String.IsNullOrEmpty(oldText) &&
                   (uriLabel.Text.Contains(reference.TagReference.HumanReadable) ||
                    uriLabel.Text.Contains(reference.TagReference.HumanReadable.Replace('\\', '/').Replace('&', '/')) ||
                    uriLabel.Text.Contains(reference.TagReference.HumanReadable.Replace('/', '\\').Replace('&', '\\'))))
                {
                    oldText = uriLabel.Text;
                    var s = reference.TagReference.StringRepresentation;
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
