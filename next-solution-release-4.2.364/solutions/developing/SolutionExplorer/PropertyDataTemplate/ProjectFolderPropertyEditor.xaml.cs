using System;
using System.Windows;
using System.Windows.Controls;
using UFInterfaces;
using DocumentManager.ComponentService;
using UIMsgBoxAlertService.ComponentService;
using Utilities.WPF;

namespace UFProjectManager.PropertyDataTemplate
{
    /// <summary>
    /// Interaction logic for ScreenUriPropertyEditor.xaml
    /// </summary>
    public partial class ProjectFolderPropertyEditor : UserControl
    {
        public ProjectFolderPropertyEditor()
        {
            InitializeComponent();
        }

        private void DlgButton_Click(object sender, RoutedEventArgs e)
        {
            e.Handled = true;
            Button button = (Button)sender;
            string folderpath = button.Tag as string;

            try
            {
                if (!string.IsNullOrEmpty(folderpath) && System.IO.Directory.Exists(folderpath))
                {
                    string path = "explorer.exe";
                    string arguments = System.IO.Path.GetDirectoryName(folderpath);
                    System.Diagnostics.Process.Start(path, arguments);
                }
            }
            catch (Exception ex)
            {
            }
        }
    }
}
