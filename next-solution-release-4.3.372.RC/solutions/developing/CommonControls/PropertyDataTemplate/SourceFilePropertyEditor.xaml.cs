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
using DocumentManager.ComponentService;
using OPCUAViewModelService.ComponentService;
using PropertyControl.ComponentService;
using WPFUtilities.PropertyDataTemplate;
using UFInterfaces;
using UIMsgBoxAlertService.ComponentService;
using Utilities;
using System.IO;

namespace CommonControls.PropertyDataTemplate
{
    /// <summary>
    /// Interaction logic for SourceFilePropertyEditor.xaml
    /// </summary>
    public partial class SourceFilePropertyEditor : UserControl
    {
        #region Dependency Properties

        #region CopyOption
        /// <summary>
        /// Allow to define how the selecrion of the file must be handle if don't exist in the project folder (default ask to user if copy it).
        /// </summary>
        public static readonly DependencyProperty CopyOptionProperty = DependencyProperty.Register("CopyOption", typeof(SourceFileCopyOption), typeof(SourceFilePropertyEditor), new UIPropertyMetadata(SourceFileCopyOption.Ask, new PropertyChangedCallback(OnCopyOptionChanged), new CoerceValueCallback(OnCoerceCopyOption)));

        private static object OnCoerceCopyOption(DependencyObject o, object value)
        {
            SourceFilePropertyEditor prop = o as SourceFilePropertyEditor;
            if (prop != null)
                return prop.OnCoerceCopyOption((SourceFileCopyOption)value);
            else
                return value;
        }

        private static void OnCopyOptionChanged(DependencyObject o, DependencyPropertyChangedEventArgs e)
        {
            SourceFilePropertyEditor prop = o as SourceFilePropertyEditor;
            if (prop != null)
                prop.OnCopyOptionChanged((SourceFileCopyOption)e.OldValue, (SourceFileCopyOption)e.NewValue);
        }

        protected virtual SourceFileCopyOption OnCoerceCopyOption(SourceFileCopyOption value)
        {
            // TODO: Keep the proposed value within the desired range.
            return value;
        }

        protected virtual void OnCopyOptionChanged(SourceFileCopyOption oldValue, SourceFileCopyOption newValue)
        {
            // TODO: Add your property changed side-effects. Descendants can override as well.
        }

        public SourceFileCopyOption CopyOption
        {
            // IMPORTANT: To maintain parity between setting a property in XAML and procedural code, do not touch the getter and setter inside this dependency property!
            get
            {
                return (SourceFileCopyOption)GetValue(CopyOptionProperty);
            }
            set
            {
                SetValue(CopyOptionProperty, value);
            }
        }
        #endregion

        #region Title
        /// <summary>
        /// Allow to set the title of the file browser.
        /// </summary>
        public static readonly DependencyProperty TitleProperty = DependencyProperty.Register("Title", typeof(string), typeof(SourceFilePropertyEditor), new UIPropertyMetadata(null, new PropertyChangedCallback(OnTitleChanged), new CoerceValueCallback(OnCoerceTitle)));

        private static object OnCoerceTitle(DependencyObject o, object value)
        {
            SourceFilePropertyEditor prop = o as SourceFilePropertyEditor;
            if (prop != null)
                return prop.OnCoerceTitle((string)value);
            else
                return value;
        }

        private static void OnTitleChanged(DependencyObject o, DependencyPropertyChangedEventArgs e)
        {
            SourceFilePropertyEditor prop = o as SourceFilePropertyEditor;
            if (prop != null)
                prop.OnTitleChanged((string)e.OldValue, (string)e.NewValue);
        }

        protected virtual string OnCoerceTitle(string value)
        {
            // TODO: Keep the proposed value within the desired range.
            return value;
        }

        protected virtual void OnTitleChanged(string oldValue, string newValue)
        {
            // TODO: Add your property changed side-effects. Descendants can override as well.
        }

        public string Title
        {
            // IMPORTANT: To maintain parity between setting a property in XAML and procedural code, do not touch the getter and setter inside this dependency property!
            get
            {
                return (string)GetValue(TitleProperty);
            }
            set
            {
                SetValue(TitleProperty, value);
            }
        }
        #endregion

        #region DefaultExt
        /// <summary>
        /// Default extension of the selected file (for example 'txt').
        /// </summary>
        public static readonly DependencyProperty DefaultExtProperty = DependencyProperty.Register("DefaultExt", typeof(string), typeof(SourceFilePropertyEditor), new UIPropertyMetadata(null, new PropertyChangedCallback(OnDefaultExtChanged), new CoerceValueCallback(OnCoerceDefaultExt)));

        private static object OnCoerceDefaultExt(DependencyObject o, object value)
        {
            SourceFilePropertyEditor prop = o as SourceFilePropertyEditor;
            if (prop != null)
                return prop.OnCoerceDefaultExt((string)value);
            else
                return value;
        }

        private static void OnDefaultExtChanged(DependencyObject o, DependencyPropertyChangedEventArgs e)
        {
            SourceFilePropertyEditor prop = o as SourceFilePropertyEditor;
            if (prop != null)
                prop.OnDefaultExtChanged((string)e.OldValue, (string)e.NewValue);
        }

        protected virtual string OnCoerceDefaultExt(string value)
        {
            // TODO: Keep the proposed value within the desired range.
            return value;
        }

        protected virtual void OnDefaultExtChanged(string oldValue, string newValue)
        {
            // TODO: Add your property changed side-effects. Descendants can override as well.
        }

        public string DefaultExt
        {
            // IMPORTANT: To maintain parity between setting a property in XAML and procedural code, do not touch the getter and setter inside this dependency property!
            get
            {
                return (string)GetValue(DefaultExtProperty);
            }
            set
            {
                SetValue(DefaultExtProperty, value);
            }
        }
        #endregion

        #region Filter
        /// <summary>
        /// Filter to use in dialog opened for selecting the file (for example 'All (.*)|*.*').
        /// </summary>
        public static readonly DependencyProperty FilterProperty = DependencyProperty.Register("Filter", typeof(string), typeof(SourceFilePropertyEditor), new UIPropertyMetadata(null, new PropertyChangedCallback(OnFilterChanged), new CoerceValueCallback(OnCoerceFilter)));

        private static object OnCoerceFilter(DependencyObject o, object value)
        {
            SourceFilePropertyEditor prop = o as SourceFilePropertyEditor;
            if (prop != null)
                return prop.OnCoerceFilter((string)value);
            else
                return value;
        }

        private static void OnFilterChanged(DependencyObject o, DependencyPropertyChangedEventArgs e)
        {
            SourceFilePropertyEditor prop = o as SourceFilePropertyEditor;
            if (prop != null)
                prop.OnFilterChanged((string)e.OldValue, (string)e.NewValue);
        }

        protected virtual string OnCoerceFilter(string value)
        {
            // TODO: Keep the proposed value within the desired range.
            return value;
        }

        protected virtual void OnFilterChanged(string oldValue, string newValue)
        {
            // TODO: Add your property changed side-effects. Descendants can override as well.
        }

        public string Filter
        {
            // IMPORTANT: To maintain parity between setting a property in XAML and procedural code, do not touch the getter and setter inside this dependency property!
            get
            {
                return (string)GetValue(FilterProperty);
            }
            set
            {
                SetValue(FilterProperty, value);
            }
        }
        #endregion

        #region DefaultFolder
        public static readonly DependencyProperty DefaultFolderProperty = DependencyProperty.Register("DefaultFolder", typeof(SpecialFolders), typeof(SourceFilePropertyEditor), new UIPropertyMetadata(SpecialFolders.Documents, new PropertyChangedCallback(OnDefaultFolderChanged), new CoerceValueCallback(OnCoerceDefaultFolder)));

        private static object OnCoerceDefaultFolder(DependencyObject o, object value)
        {
            SourceFilePropertyEditor control = o as SourceFilePropertyEditor;
            if (control != null)
                return control.OnCoerceDefaultFolder((SpecialFolders)value);
            else
                return value;
        }

        private static void OnDefaultFolderChanged(DependencyObject o, DependencyPropertyChangedEventArgs e)
        {
            SourceFilePropertyEditor control = o as SourceFilePropertyEditor;
            if (control != null)
                control.OnDefaultFolderChanged((SpecialFolders)e.OldValue, (SpecialFolders)e.NewValue);
        }

        protected virtual SpecialFolders OnCoerceDefaultFolder(SpecialFolders value)
        {
            // TODO: Keep the proposed value within the desired range.
            return value;
        }

        protected virtual void OnDefaultFolderChanged(SpecialFolders oldValue, SpecialFolders newValue)
        {
            // TODO: Add your property changed side-effects. Descendants can override as well.
        }

        public SpecialFolders DefaultFolder
        {
            // IMPORTANT: To maintain parity between setting a property in XAML and procedural code, do not touch the getter and setter inside this dependency property!
            get
            {
                return (SpecialFolders)GetValue(DefaultFolderProperty);
            }
            set
            {
                SetValue(DefaultFolderProperty, value);
            }
        }

        #endregion

        #region Workspace
        public static readonly DependencyProperty WorkspaceProperty = DependencyProperty.Register("Workspace", typeof(IWorkspace), typeof(SourceFilePropertyEditor), new UIPropertyMetadata(null, new PropertyChangedCallback(OnWorkspaceChanged), new CoerceValueCallback(OnCoerceWorkspace)));

        private static object OnCoerceWorkspace(DependencyObject o, object value)
        {
            SourceFilePropertyEditor control = o as SourceFilePropertyEditor;
            if (control != null)
                return control.OnCoerceWorkspace((IWorkspace)value);
            else
                return value;
        }

        private static void OnWorkspaceChanged(DependencyObject o, DependencyPropertyChangedEventArgs e)
        {
            SourceFilePropertyEditor control = o as SourceFilePropertyEditor;
            if (control != null)
                control.OnWorkspaceChanged((IWorkspace)e.OldValue, (IWorkspace)e.NewValue);
        }

        protected virtual IWorkspace OnCoerceWorkspace(IWorkspace value)
        {
            // TODO: Keep the proposed value within the desired range.
            return value;
        }

        protected virtual void OnWorkspaceChanged(IWorkspace oldValue, IWorkspace newValue)
        {
            // TODO: Add your property changed side-effects. Descendants can override as well.
        }

        public IWorkspace Workspace
        {
            // IMPORTANT: To maintain parity between setting a property in XAML and procedural code, do not touch the getter and setter inside this dependency property!
            get
            {
                return (IWorkspace)GetValue(WorkspaceProperty);
            }
            set
            {
                SetValue(WorkspaceProperty, value);
            }
        }

        #endregion

        #region UseUri
        public static readonly DependencyProperty UseUriProperty = DependencyProperty.Register("UseUri", typeof(bool), typeof(SourceFilePropertyEditor), new UIPropertyMetadata(true, new PropertyChangedCallback(OnUseUriChanged), new CoerceValueCallback(OnCoerceUseUri)));

        private static object OnCoerceUseUri(DependencyObject o, object value)
        {
            SourceFilePropertyEditor prop = o as SourceFilePropertyEditor;
            if (prop != null)
                return prop.OnCoerceUseUri((bool)value);
            else
                return value;
        }

        private static void OnUseUriChanged(DependencyObject o, DependencyPropertyChangedEventArgs e)
        {
            SourceFilePropertyEditor prop = o as SourceFilePropertyEditor;
            if (prop != null)
                prop.OnUseUriChanged((bool)e.OldValue, (bool)e.NewValue);
        }

        protected virtual bool OnCoerceUseUri(bool value)
        {
            // TODO: Keep the proposed value within the desired range.
            return value;
        }

        protected virtual void OnUseUriChanged(bool oldValue, bool newValue)
        {
            // TODO: Add your property changed side-effects. Descendants can override as well.
        }

        public bool UseUri
        {
            // IMPORTANT: To maintain parity between setting a property in XAML and procedural code, do not touch the getter and setter inside this dependency property!
            get
            {
                return (bool)GetValue(UseUriProperty);
            }
            set
            {
                SetValue(UseUriProperty, value);
            }
        }
        #endregion

        #endregion

        public SourceFilePropertyEditor()
        {
            InitializeComponent();
        }

        private void DlgButton_Click(object sender, RoutedEventArgs e)
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
            dlg.Title = Title ?? Properties.Resources.SourceFileTitle;
            // Set filter for file extension and default file extension
            dlg.DefaultExt = DefaultExt ?? ".*";
            dlg.Filter = Filter ?? Properties.Resources.AllFiles;
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
                IDocument document = Workspace?.ContextDocument;
                IUIMsgBoxAlertService uiInterface = document?.GetService(typeof(IUIMsgBoxAlertService)) as IUIMsgBoxAlertService;
                if (uiInterface != null)
                {
                    byte[] data = null;
                    string filename = dlg.FileName;
                    Uri fileUri = new Uri(filename);
                    //Uri docUri = new Uri(document.FilePath);
                    Uri docUri = document.GetSpecialFolder(DefaultFolder);
                    
                    var parent = document;
                    if (parent.Parent != null)
                        parent = document.Parent;
                    if (CopyOption != SourceFileCopyOption.Never && docUri.IsBaseOf(fileUri))
                    {
                        Uri newUri = docUri.MakeRelativeUri(fileUri);
                        if (UseUri)
                            button.Tag = newUri;
                        else
                            button.Tag = newUri.GetPathString();
                        text.GetBindingExpression(TextBox.TextProperty).UpdateTarget();
                    }
                    else
                    {
                        bool copyFile = CopyOption == SourceFileCopyOption.Always;
                        if (CopyOption == SourceFileCopyOption.Ask)
                        {
                            copyFile = uiInterface.ShowYesNo(Properties.Resources.AskCopyFile,
                                    CustomDialogIcons.Question) == CustomDialogResults.Yes;
                        }
                        
                        if (copyFile)
                        {
                            var name = System.IO.Path.GetFileName(dlg.FileName);
                            var dest = System.IO.Path.GetDirectoryName(docUri.GetPathString());
                            string destfilename = System.IO.Path.Combine(dest, name);

                            bool bOverwrite = true;
                            if (document.fileSystemProviderBase == null)
                            {
                                if (!destfilename.Equals(filename))
                                {
                                    if (File.Exists(destfilename))
                                    {
                                        if (uiInterface != null)
                                        {
                                            var res = uiInterface.ShowYesNoCancel(Properties.Resources.FileExistsOverwriteRename,
                                                UIMsgBoxAlertService.ComponentService.CustomDialogIcons.Question);
                                            if (res == UIMsgBoxAlertService.ComponentService.CustomDialogResults.No)
                                                bOverwrite = false;
                                            else if (res == CustomDialogResults.Cancel)
                                                return;
                                        }
                                        else
                                        {
                                            var res = MessageBox.Show(Properties.Resources.FileExistsOverwriteRename, name, MessageBoxButton.YesNoCancel, MessageBoxImage.Question);
                                            if (res == MessageBoxResult.No)
                                                bOverwrite = false;
                                            else if (res == MessageBoxResult.Cancel)
                                                return;
                                        }
                                    }

                                    var originalName = name;
                                    var rnd = new Random();
                                    bool bok = false;
                                    do
                                    {
                                        try
                                        {
                                            System.IO.File.Copy(filename, destfilename, bOverwrite);
                                            bok = true;
                                        }
                                        catch (Exception ex)
                                        {
                                            name = rnd.Next(1000).ToString() + originalName;
                                            destfilename = System.IO.Path.Combine(dest, name);
                                        }
                                    } while (!bok);
                                }
                            }
                            else
                            {
                                if (document.fileSystemProviderBase.Exists(new VFS.FileManagerFile(document.fileSystemProviderBase, destfilename)))
                                {
                                    if (uiInterface != null)
                                    {
                                        var res = uiInterface.ShowYesNoCancel(Properties.Resources.FileExistsOverwriteRename,
                                            UIMsgBoxAlertService.ComponentService.CustomDialogIcons.Question);
                                        if (res == UIMsgBoxAlertService.ComponentService.CustomDialogResults.No)
                                            bOverwrite = false;
                                        else if (res == UIMsgBoxAlertService.ComponentService.CustomDialogResults.Cancel)
                                            return;
                                    }
                                    else
                                    {
                                        var res = MessageBox.Show(Properties.Resources.FileExistsOverwriteRename, name, MessageBoxButton.YesNoCancel, MessageBoxImage.Question);
                                        if (res == MessageBoxResult.No)
                                            bOverwrite = false;
                                        else if (res == MessageBoxResult.Cancel)
                                            return;
                                    }
                                }

                                if (!bOverwrite)
                                {
                                    var originalName = name;
                                    var rnd = new Random();
                                    while (true)
                                    {
                                        if (!document.fileSystemProviderBase.Exists(new VFS.FileManagerFile(document.fileSystemProviderBase, destfilename)))
                                            break;

                                        name = rnd.Next(1000).ToString() + originalName;
                                        destfilename = System.IO.Path.Combine(dest, name);
                                    }
                                }

                                if (Utilities.IO.FileSystem.IsBinaryFile(filename))
                                {
                                    data = File.ReadAllBytes(filename);
                                }
                                else
                                {
                                    var text = File.ReadAllText(filename);
                                    data = System.Text.Encoding.Unicode.GetBytes(text);
                                }

                                document.fileSystemProviderBase.UploadFile(null, destfilename, data);

                                name = System.IO.Path.GetFileName(destfilename);
                                dest = System.IO.Path.GetDirectoryName(destfilename) + "\\";
                            }

                            //fileUri = new Uri(destfilename);
                            var newUri = new Uri(System.IO.Path.GetFileName(destfilename), UriKind.Relative);// docUri.MakeRelativeUri(fileUri);
                            if (UseUri)
                                button.Tag = newUri;
                            else
                                button.Tag = newUri.GetPathString();
                            text.GetBindingExpression(TextBox.TextProperty).UpdateTarget();
                        }
                        else 
                        {
                            if(UseUri)
                                button.Tag = fileUri;
                            else
                                button.Tag = fileUri.GetPathString();
                            text.GetBindingExpression(TextBox.TextProperty).UpdateTarget();
                        }
                    }
                }
                else
                {
                    string filename = dlg.FileName;
                    var newUri = new Uri(filename);
                    if (UseUri)
                        button.Tag = newUri;
                    else
                        button.Tag = newUri.GetPathString();
                    text.GetBindingExpression(TextBox.TextProperty).UpdateTarget();
                }
            }
        }

        private void DlgButton_Click_Clear(object sender, RoutedEventArgs e)
        {
            Button button = (Button)sender;
            button.Tag = null;
            text.GetBindingExpression(TextBox.TextProperty).UpdateTarget();
        }
    }
}
