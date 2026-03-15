using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using Utilities;
using Utilities.WPF;
using DevExpress.Xpo;
using System.Reflection;
using DevExpress.Xpo.DB;
using Ookii.Dialogs.Wpf;
using UFUAModel;
using Aga.Controls.Tree;
using System.Globalization;
using System.Windows.Data;
using System.Collections;
using Microsoft.Win32;
using System.Collections.ObjectModel;
using System.Windows.Shapes;
using System.Windows.Documents;
using System.Windows.Media;
using System.ComponentModel;
using System.Text;
using log4net;
using DocumentManager.ComponentService;
using UFUAEditor.Document;
using System.Threading.Tasks;
using System.Xml.Linq;
using System.Xml;
using System.Windows.Markup;
using UIMsgBoxAlertService.ComponentService;
using UFUAEditor.ComponentService;
using System.IO;
using System.Text.RegularExpressions;
using System.Diagnostics;
using WPFUtilities.ImportExportHelpers;

namespace UFUAEditor.Controls
{
    
    /// <summary>
    /// Interaction logic for ImportTagsEditor.xaml
    /// </summary>
    public partial class ImportEditor : UserControl
    {
        private static readonly ILog log = LogManager.GetLogger(Properties.Resources.ImportEditorTitle);
        UFUAServerDocument Document;

        #region EnumAction
        public static readonly DependencyProperty EnumActionProperty = DependencyProperty.Register("EnumAction", typeof(ActionType), typeof(ImportEditor), new UIPropertyMetadata(ActionType.Export, new PropertyChangedCallback(OnEnumActionChanged), new CoerceValueCallback(OnCoerceEnumAction)));

        private static object OnCoerceEnumAction(DependencyObject o, object value)
        {
            ImportEditor control = o as ImportEditor;
            if (control != null)
                return control.OnCoerceEnumAction((ActionType)value);
            else
                return value;
        }

        private static void OnEnumActionChanged(DependencyObject o, DependencyPropertyChangedEventArgs e)
        {
            ImportEditor control = o as ImportEditor;
            if (control != null)
                control.OnEnumActionChanged((ActionType)e.OldValue, (ActionType)e.NewValue);
        }

        protected virtual ActionType OnCoerceEnumAction(ActionType value)
        {
            // TODO: Keep the proposed value within the desired range.
            return value;
        }

        protected virtual void OnEnumActionChanged(ActionType oldValue, ActionType newValue)
        {
            // TODO: Add your property changed side-effects. Descendants can override as well.
        }

        public ActionType EnumAction
        {
            // IMPORTANT: To maintain parity between setting a property in XAML and procedural code, do not touch the getter and setter inside this dependency property!
            get
            {
                return (ActionType)GetValue(EnumActionProperty);
            }
            set
            {
                SetValue(EnumActionProperty, value);
            }
        }

        #endregion

        #region FilePath
        public static readonly DependencyProperty FilePathProperty = DependencyProperty.Register("FilePath", typeof(string), typeof(ImportEditor), new UIPropertyMetadata(string.Empty, new PropertyChangedCallback(OnFilePathChanged), new CoerceValueCallback(OnCoerceFilePath)));

        private static object OnCoerceFilePath(DependencyObject o, object value)
        {
            ImportEditor control = o as ImportEditor;
            if (control != null)
                return control.OnCoerceFilePath((string)value);
            else
                return value;
        }

        private static void OnFilePathChanged(DependencyObject o, DependencyPropertyChangedEventArgs e)
        {
            ImportEditor control = o as ImportEditor;
            if (control != null)
                control.OnFilePathChanged((string)e.OldValue, (string)e.NewValue);
        }

        protected virtual string OnCoerceFilePath(string value)
        {
            // TODO: Keep the proposed value within the desired range.
            return value;
        }

        protected virtual void OnFilePathChanged(string oldValue, string newValue)
        {
            // TODO: Add your property changed side-effects. Descendants can override as well.
        }

        public string FilePath
        {
            // IMPORTANT: To maintain parity between setting a property in XAML and procedural code, do not touch the getter and setter inside this dependency property!
            get
            {
                return (string)GetValue(FilePathProperty);
            }
            set
            {
                SetValue(FilePathProperty, value);
            }
        }

        #endregion


        #region MatchCase
        public static readonly DependencyProperty MatchCaseProperty = DependencyProperty.Register("MatchCase", typeof(bool), typeof(ImportEditor), new UIPropertyMetadata(false, new PropertyChangedCallback(OnMatchCaseChanged), new CoerceValueCallback(OnCoerceMatchCase)));

        private static object OnCoerceMatchCase(DependencyObject o, object value)
        {
            ImportEditor control = o as ImportEditor;
            if (control != null)
                return control.OnCoerceMatchCase((bool)value);
            else
                return value;
        }

        private static void OnMatchCaseChanged(DependencyObject o, DependencyPropertyChangedEventArgs e)
        {
            ImportEditor control = o as ImportEditor;
            if (control != null)
                control.OnMatchCaseChanged((bool)e.OldValue, (bool)e.NewValue);
        }

        protected virtual bool OnCoerceMatchCase(bool value)
        {
            // TODO: Keep the proposed value within the desired range.
            return value;
        }

        protected virtual void OnMatchCaseChanged(bool oldValue, bool newValue)
        {
            // TODO: Add your property changed side-effects. Descendants can override as well.
        }

        public bool MatchCase
        {
            // IMPORTANT: To maintain parity between setting a property in XAML and procedural code, do not touch the getter and setter inside this dependency property!
            get
            {
                return (bool)GetValue(MatchCaseProperty);
            }
            set
            {
                SetValue(MatchCaseProperty, value);
            }
        }

        #endregion
        

        #region Filter
        public static readonly DependencyProperty FilterProperty = DependencyProperty.Register("Filter", typeof(string), typeof(ImportEditor), new UIPropertyMetadata(string.Empty, new PropertyChangedCallback(OnFilterChanged), new CoerceValueCallback(OnCoerceFilter)));

        private static object OnCoerceFilter(DependencyObject o, object value)
        {
            ImportEditor control = o as ImportEditor;
            if (control != null)
                return control.OnCoerceFilter((string)value);
            else
                return value;
        }

        private static void OnFilterChanged(DependencyObject o, DependencyPropertyChangedEventArgs e)
        {
            ImportEditor control = o as ImportEditor;
            if (control != null)
                control.OnFilterChanged((string)e.OldValue, (string)e.NewValue);
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



        #region Separator
        public static readonly DependencyProperty SeparatorProperty = DependencyProperty.Register("Separator", typeof(SepType), typeof(ImportEditor), new UIPropertyMetadata(SepType.SemiColon, new PropertyChangedCallback(OnSeparatorChanged), new CoerceValueCallback(OnCoerceSeparator)));

        private static object OnCoerceSeparator(DependencyObject o, object value)
        {
            ImportEditor control = o as ImportEditor;
            if (control != null)
                return control.OnCoerceSeparator((SepType)value);
            else
                return value;
        }

        private static void OnSeparatorChanged(DependencyObject o, DependencyPropertyChangedEventArgs e)
        {
            ImportEditor control = o as ImportEditor;
            if (control != null)
                control.OnSeparatorChanged((SepType)e.OldValue, (SepType)e.NewValue);
        }

        protected virtual SepType OnCoerceSeparator(SepType value)
        {
            // TODO: Keep the proposed value within the desired range.
            return value;
        }

        protected virtual void OnSeparatorChanged(SepType oldValue, SepType newValue)
        {
            // TODO: Add your property changed side-effects. Descendants can override as well.
        }

        public SepType Separator
        {
            // IMPORTANT: To maintain parity between setting a property in XAML and procedural code, do not touch the getter and setter inside this dependency property!
            get
            {
                return (SepType)GetValue(SeparatorProperty);
            }
            set
            {
                SetValue(SeparatorProperty, value);
            }
        }

        #endregion



        #region FileExtension
        public static readonly DependencyProperty FileExtensionProperty = DependencyProperty.Register("FileExtension", typeof(string), typeof(ImportEditor), new UIPropertyMetadata("csv"));
        public string FileExtension
        {
            // IMPORTANT: To maintain parity between setting a property in XAML and procedural code, do not touch the getter and setter inside this dependency property!
            get
            {
                return (string)GetValue(FileExtensionProperty);
            }
            set
            {
                SetValue(FileExtensionProperty, value);
            }
        }

        #endregion



        #region ShowFilter
        public static readonly DependencyProperty ShowFilterProperty = DependencyProperty.Register("ShowFilter", typeof(bool), typeof(ImportEditor), new UIPropertyMetadata(true));
        public bool ShowFilter
        {
            // IMPORTANT: To maintain parity between setting a property in XAML and procedural code, do not touch the getter and setter inside this dependency property!
            get
            {
                return (bool)GetValue(ShowFilterProperty);
            }
            set
            {
                SetValue(ShowFilterProperty, value);
            }
        }

        #endregion


        #region ShowSeparator
        public static readonly DependencyProperty ShowSeparatorProperty = DependencyProperty.Register("ShowSeparator", typeof(bool), typeof(ImportEditor), new UIPropertyMetadata(true));
        public bool ShowSeparator
        {
            // IMPORTANT: To maintain parity between setting a property in XAML and procedural code, do not touch the getter and setter inside this dependency property!
            get
            {
                return (bool)GetValue(ShowSeparatorProperty);
            }
            set
            {
                SetValue(ShowSeparatorProperty, value);
            }
        }

        #endregion


        public ImportEditor(UFUAServerDocument doc)
        {
            InitializeComponent();
            Document = doc;

            Loaded += (o, e) =>
            {
                DataContext = this;
            };

        }

        private void SelectFile_Click(object sender, RoutedEventArgs e)
        {
            string file = String.Empty;
            string extension = FileExtension;
            if (Environment.UserInteractive)
            {
                VistaOpenFileDialog dialog = new VistaOpenFileDialog();
                dialog.CheckFileExists = false;
                dialog.ValidateNames = true;
                dialog.FileName = FilePath;
                dialog.Filter = string.Format("{0} files|*.{0}",FileExtension);
                if (dialog.ShowDialog() == true)
                {
                    file = dialog.FileName;
                }
            }
            if (String.IsNullOrEmpty(file))
                return;

            //file = file.ToLower();
            if (string.IsNullOrEmpty(System.IO.Path.GetExtension(file)))
                file = string.Format("{0}.{1}",file, FileExtension);

            if (System.IO.Path.GetExtension(file).ToLower().Equals(string.Format(".{0}", FileExtension)))
            {
                try
                {
                    FilePath = file;
                }
                catch (Exception ex)
                {
                }
            }

        }

    }

}
