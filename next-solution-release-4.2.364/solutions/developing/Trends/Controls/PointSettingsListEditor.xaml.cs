using System;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using DocumentManager.ComponentService;
using Utilities;
using StringManager.ComponentService;
using Utilities.WPF;
using System.Windows.Media;

namespace Trends.Controls
{
    /// <summary>
    /// Interaction logic for UriImageListEditor.xaml
    /// </summary>
    public partial class PointSettingsListEditor : UserControl
    {
        readonly PointSettings datalist;
        readonly IDocument document;

        public PointSettingsListEditor(PointSettings list, IDocument doc)
        {
            InitializeComponent();
            datalist = list;
            document = doc;
            ThemeImageHelper.LoadBitmapImageResourceDictionary(this.Resources, document);

            gridControl.ItemsSource = datalist;
            DataContext = datalist;
        }

        private void ButtonImageClear_Click(object sender, RoutedEventArgs e)
        {
            int focusedrow = tableView.FocusedRowHandle;
            int listIndex = gridControl.GetListIndexByRowHandle(focusedrow);
            if (listIndex >= 0)
            {
                gridControl.SelectedItem = null;
                gridControl.RefreshData();
                tableView.FocusedRowHandle = focusedrow;
            }
        }

        private void OnAdd(object sender, RoutedEventArgs e)
        {
            datalist.Add(new PointSetting() { PColor = SmartControlUtilities.TagColorHelper.RandomColor()});
        }

        private void OnDelete(object sender, RoutedEventArgs e)
        {
            if (datalist.Count < 1)
                return;

            if (tableView.IsEditing)
                return;

            int listIndex = gridControl.GetListIndexByRowHandle(tableView.FocusedRowHandle);
            if (listIndex >= 0 && datalist.Count > listIndex)
            {
                datalist[listIndex] = null;
                datalist.RemoveAt(listIndex);
            }
        }

        private void OnMoveDown(object sender, RoutedEventArgs e)
        {
            int focusedrow = tableView.FocusedRowHandle;
            int listIndex = gridControl.GetListIndexByRowHandle(focusedrow);
            if (listIndex < 0 || listIndex >= datalist.Count - 1)
                return;
            datalist.Move(listIndex, listIndex + 1);
            gridControl.SelectedItem = datalist[listIndex + 1];
        }

        private void OnMoveUp(object sender, RoutedEventArgs e)
        {
            int focusedrow = tableView.FocusedRowHandle;
            int listIndex = gridControl.GetListIndexByRowHandle(focusedrow);
            if (listIndex <= 0 || listIndex >= datalist.Count)
                return;
            datalist.Move(listIndex, listIndex - 1);
            gridControl.SelectedItem = datalist[listIndex - 1];
        }

        private void DlgButton_Click_ClearLabel(object sender, RoutedEventArgs e)
        {
            int focusedrow = tableView.FocusedRowHandle;
            int listIndex = gridControl.GetListIndexByRowHandle(focusedrow);
            if (listIndex >= 0)
            {
                (datalist.ElementAt(listIndex).PLabel) = string.Empty;
                tableView.FocusedRowHandle = focusedrow;
                gridControl.RefreshRow(focusedrow);
            }

        }

        private void DlgButton_Click_Label(object sender, RoutedEventArgs e)
        {
            int focusedrow = tableView.FocusedRowHandle;
            int listIndex = gridControl.GetListIndexByRowHandle(focusedrow);
            if (listIndex >= 0)
            {
                if (document == null)
                    return;
                Button button = (Button)sender;
                String value;
                value = (String)button.Tag;

                IStringEditorManager stringEditorManager = document.GetService(typeof(IStringEditorManager)) as IStringEditorManager;
                if (stringEditorManager == null)
                    return;
                var stringEditor = stringEditorManager.GetStringEditor(document);

                if (stringEditor == null)
                    return;

                var Dialog = new GeneralDialogContent(stringEditor)
                {
                    Owner = this.FindParent<Window>(),
                    DialogKeepContent = true,
                    Title = Properties.Resources.SelectStringEditor,
                    HelpLink = "StringEditor"
                };
                if (Dialog.ShowDialog() != true)
                    return;

                if (stringEditor.DataContext != null && (stringEditor.DataContext as String) != null)
                {
                    Dispatcher.BeginInvokeInBackgroundIfRequired(() =>
                    {
                        (datalist.ElementAt(listIndex).PLabel) = new String((stringEditor.DataContext as String).ToArray());
                        tableView.FocusedRowHandle = focusedrow;
                        gridControl.RefreshRow(focusedrow);
                    });
                }

            }
        }

        private void OnBrushSelected(object sender, EventArgs e)
        {
            int focusedrow = tableView.FocusedRowHandle;
            int listIndex = gridControl.GetListIndexByRowHandle(focusedrow);
            if (listIndex >= 0 && listIndex < datalist.Count)
            {
                tableView.FocusedRowHandle = focusedrow;
                gridControl.RefreshRow(focusedrow);
            }
        }
    }
}
