using System;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using UFUAModel;
using Utilities;
using Utilities.WPF;
using WPFUtilities;
using DriverCodeBaseEx.Extensions;
using OPCUAViewModel;
using OPCUAViewModelService.ComponentService;
using UFUAEditor.ComponentService;
using DocumentManager.ComponentService;
using DevExpress.Xpf.Editors;

namespace DriverCodeBaseEx.UI.SettingsControls
{
    /// <summary>
    /// Interaction logic for StateCommand.xaml
    /// </summary>
    public partial class JobConditionalVariable : UserControl
    {
        #region Declarations
        bool bLoaded, bFilled, bEditing;
        TagEntityReference original;
        #endregion
        public JobConditionalVariable()
        {
            InitializeComponent();

        }

        #region Commands
        private void DlgButton_Click(object sender, RoutedEventArgs e)
        {
            var value = Tag as TagEntityReference;

            if (value == null)
                value = TagEntityReference.Empty;

            bEditing = false;
            var tag = value.Edit(this.FindParent<Window>());
            if (tag != null)
                Tag = tag;
        }

        private void DlgButton_Click_Clear(object sender, RoutedEventArgs e)
        {
            Tag = null;
        }
        #endregion

        #region Tag Combobox
        private void uriLabel_PopupOpening(object sender, DevExpress.Xpf.Editors.OpenPopupEventArgs e)
        {
            FillComboBox();
        }

        private void uriLabel_GotFocus(object sender, RoutedEventArgs e)
        {
            uriLabel.SelectAll();
        }

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

        private async void uriLabel_LostFocus(object sender, RoutedEventArgs e)
        {
            NewTag(uriLabel.Text);
        }

        private async void NewTag(string sTag)
        {
            if (!bEditing || !OPCUAViewModelComponent.workspaceServiceAvailable || sTag == null)
            {
                bEditing = false;
                return;
            }
            bEditing = false;
            IDocument doc = OPCUAViewModelComponent.workspaceService.ContextDocument as IDocument;
            if (doc == null)
                return;

            var editor = doc.GetService(typeof(IUFUAEditorManager)) as IUFUAEditorManager;
            if (editor == null)
                return;

            var split = sTag.Split(':');
            var instance = split[0];
            var name = split[0];
            if (split.Length > 1)
                name = split[1];
            else
                instance = null;

            if (original == null)
                original = Tag as TagEntityReference;
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

                if (original as TagEntityReference != null && !String.IsNullOrEmpty((original as TagEntityReference).HumanReadableNoProject))
                    Tag = original;
                else
                    Tag = TagEntityReference.Empty;
                uriLabel.GetBindingExpression(ComboBoxEdit.TextProperty).UpdateTarget();
            }
            else
            {
                var tagOPCUA = xml.FromXml<OPCUAEntityReference>();

                Guid tagGuid = Guid.Empty;
                if (tagOPCUA.ResolvedNodeId.IdType == Opc.Ua.IdType.Guid)
                    tagGuid = (Guid)tagOPCUA.ResolvedNodeId.Identifier;
                else if (tagOPCUA.ResolvedNodeId.IdType == Opc.Ua.IdType.String)
                {
                    var identifier = tagOPCUA.ResolvedNodeId.Identifier.ToString();
                    var index = identifier.LastIndexOf('?');
                    if (index != -1)
                        identifier = identifier.Substring(index + 1);
                    Guid.TryParse(identifier, out tagGuid);
                }

                var tagRef = new TagEntityReference(tagGuid, tagOPCUA.RelativePath, tagOPCUA.ResolvedNodeId, tagOPCUA.HumanReadable);
                Tag = tagRef;

                original = tagRef.Clone() as TagEntityReference;
            }
        }

        private async void uriLabel_PopupClosed(object sender, ClosePopupEventArgs e)
        {
            NewTag(e.EditValue as string);
        }

        private void uriLabel_KeyDown(object sender, KeyEventArgs e)
        {
            bEditing = true;

            FillComboBox(e.Key == Key.F5);
        }

        private void uriLabel_SelectedIndexChanged(object sender, RoutedEventArgs e)
        {
            bEditing = true;
        }
        #endregion
    }
}
