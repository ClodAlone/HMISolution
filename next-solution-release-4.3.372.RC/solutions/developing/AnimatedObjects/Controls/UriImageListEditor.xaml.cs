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
using Utilities;
using UIMsgBoxAlertService.ComponentService;
using UFInterfaces.Converters;
using WPFUtilities;
using WPFUtilities.PropertyDataTemplate;

namespace AnimatedObjects.Controls
{
    /// <summary>
    /// Interaction logic for UriImageListEditor.xaml
    /// </summary>
    public partial class UriImageListEditor : UserControl
    {
        BackImageItemList datalist = new BackImageItemList();
        internal IDocument Document = null;
        IUIMsgBoxAlertService UIMsgBoxAlertService;

        public UriImageListEditor(BackImageItemList list, IDocument doc)
        {
            InitializeComponent();
            datalist = list;

            gridControl.ItemsSource = datalist;
            DataContext = datalist;
            Document = doc;
        }

        private void ButtonImageClear_Click(object sender, RoutedEventArgs e)
        {
            int focusedrow = tableView.FocusedRowHandle;
            int listIndex = gridControl.GetListIndexByRowHandle(focusedrow);
            if (listIndex >= 0 && listIndex < datalist.Count)
            {
                datalist.ElementAt(listIndex).Value = null;
                gridControl.ItemsSource = null;
                gridControl.ItemsSource = datalist;
                tableView.FocusedRowHandle = focusedrow;
            }
        }

        private void ButtonImage_Click(object sender, RoutedEventArgs e)
        {
            int focusedrow = tableView.FocusedRowHandle;
            int listIndex = gridControl.GetListIndexByRowHandle(focusedrow);
            if (listIndex >= 0)
            {

                Button button = (Button)sender;
                Uri value = null;
                if (button.Tag is String)
                {
                    try
                    {
                        value = new Uri(button.Tag as String);
                    }
                    catch (Exception ex)
                    {

                    }
                }
                else if (button.Tag is Uri)
                    value = (Uri)(button.Tag);

                // Create OpenFileDialog
                Microsoft.Win32.OpenFileDialog dlg = new Microsoft.Win32.OpenFileDialog();
                dlg.Title = Properties.Resources.SourceFileTitle;
                // Set filter for file extension and default file extension
                dlg.DefaultExt = "ico";
                dlg.Filter = Properties.Resources.AllPictureFiles;
                dlg.ValidateNames = false;
                dlg.CheckPathExists = false;
                if (value != null)
                {
                    if (value.IsAbsoluteUri && value.IsFile)
                    {
                        dlg.InitialDirectory = System.IO.Path.GetDirectoryName(value.LocalPath);
                        dlg.FileName = System.IO.Path.GetFileName(value.LocalPath);
                    }
                    else
                    {
                        dlg.FileName = System.IO.Path.GetFileName(value.OriginalString);
                    }
                }

                // Display OpenFileDialog by calling ShowDialog method
                Nullable<bool> result = dlg.ShowDialog();




                // Get the selected file name and display in a TextBox
                if (result == true)
                {
                    if (Document != null)
                    {
                        string filename = dlg.FileName;

                        if (UIMsgBoxAlertService == null)
                            UIMsgBoxAlertService = Document.GetService(typeof(IUIMsgBoxAlertService)) as IUIMsgBoxAlertService;

                        IUriToUriAbsoluteImageConverter uriToUriAbsoluteImageConverter = Resources["uriToUriAbsoluteImageConverter"] as IUriToUriAbsoluteImageConverter;
                        string dest = System.IO.Path.GetDirectoryName(Document.FilePath) + "\\";
                        var absolutePath = new Uri(dest, UriKind.RelativeOrAbsolute);
                        var absolutePath2 = Document.GetSpecialFolder(SpecialFolders.Images);
                        uriToUriAbsoluteImageConverter.FileSystemProviderBase = Document.fileSystemProviderBase;
                        uriToUriAbsoluteImageConverter.AbsolutePath = absolutePath;
                        uriToUriAbsoluteImageConverter.AbsolutePath2 = absolutePath2;

                        try
                        {
                            var dropHelper = new DropFileHelper(Document, uriToUriAbsoluteImageConverter, UIMsgBoxAlertService, SourceFileCopyOption.Ask);
                            Uri uriFound;
                            var mp = dropHelper.DropFile(filename, out uriFound);

                            if(uriFound != null)
                            {
                                var _image = uriFound;
                                if (datalist.Count > listIndex)
                                    datalist[listIndex].Value = _image;
                                gridControl.ItemsSource = null;
                                gridControl.ItemsSource = datalist;
                                tableView.FocusedRowHandle = focusedrow;
                            }
                        }
                        catch (OperationCanceledException)
                        {
                        }
                    }
                    else
                    {
                        Uri filename = new Uri(dlg.FileName, UriKind.RelativeOrAbsolute);
                        if (datalist.Count > listIndex)
                            datalist[listIndex].Value = filename;
                        gridControl.ItemsSource = null;
                        gridControl.ItemsSource = datalist;
                        tableView.FocusedRowHandle = focusedrow;
                    }

                }
            }

        }

        private void OnAdd(object sender, RoutedEventArgs e)
        {
            datalist.Add(new BackImage());
            gridControl.ItemsSource = null;
            gridControl.ItemsSource = datalist;
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

                gridControl.ItemsSource = null;
                gridControl.ItemsSource = datalist;
            }
        }

        private void OnMoveDown(object sender, RoutedEventArgs e)
        {
            int focusedrow = tableView.FocusedRowHandle;
            int listIndex = gridControl.GetListIndexByRowHandle(focusedrow);
            if (listIndex < 0 || listIndex >= datalist.Count - 1)
                return;
            datalist.Move(listIndex, listIndex + 1);
            gridControl.ItemsSource = null;
            gridControl.ItemsSource = datalist;
            gridControl.SelectedItem = datalist[listIndex + 1];
        }

        private void OnMoveUp(object sender, RoutedEventArgs e)
        {
            int focusedrow = tableView.FocusedRowHandle;
            int listIndex = gridControl.GetListIndexByRowHandle(focusedrow);
            if (listIndex <= 0 || listIndex >= datalist.Count)
                return;
            datalist.Move(listIndex, listIndex - 1);
            gridControl.ItemsSource = null;
            gridControl.ItemsSource = datalist;
            gridControl.SelectedItem = datalist[listIndex - 1];
        }
    }
}
