using System;
using System.Collections.Generic;
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
using DocumentManager.ComponentService;
using OPCUAViewModelService.ComponentService;
using UFUAEditor.ComponentService;
using Utilities;
using Utilities.WPF;
using UFUAEditor.Extensions;
using WPFUtilities;

namespace UFUAEditor.PropertyDataTemplate
{
    /// <summary>
    /// Interaction logic for TagEntityReferencePropertyEditor.xaml
    /// </summary>
    public partial class TagEntityReferencePropertyEditor : UserControl
    {
        bool bLoaded;
        public TagEntityReferencePropertyEditor()
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
            e.Handled = true;
            Button button = (Button)sender;

            UFUAModel.TagEntityReference value = UFUAModel.TagEntityReference.Empty;
            if (button.Tag is UFUAModel.TagEntityReference)
                value = (UFUAModel.TagEntityReference)button.Tag;

            var tag = value.Edit(this.FindParent<Window>())?.First();
            if (tag != null)
            {
                button.Tag = tag.TagReference;
                uriLabel.Text = tag.TagReference.ToString();
            }
        }

        private void DlgButton_Click_Clear(object sender, RoutedEventArgs e)
        {
            e.Handled = true;
            Button button = (Button)sender;
            button.Tag = UFUAModel.TagEntityReference.Empty;
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

            var split = uriLabel.Text.Split(':');
            string member = null;
            var name = split[0];
            if (split.Length > 1)
                member = split[1];

            var tag = UFUAEditorManagerComponent.ufuaEditorManagerComponent.GetObjectTagEntityReference(doc, name, member) as UFUAModel.TagEntityReference;
            if (tag == null)
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
                uri.Tag = tag;
                oldText = null;
                uriLabel.Text = tag.ToString();
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
            if (uri.Tag != null && uri.Tag is UFUAModel.TagEntityReference)
            {
                if (String.IsNullOrEmpty(oldText))
                {
                    var reference = uri.Tag as UFUAModel.TagEntityReference;
                    oldText = uriLabel.Text;
                    uriLabel.Text = reference.StringRepresentation;
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
