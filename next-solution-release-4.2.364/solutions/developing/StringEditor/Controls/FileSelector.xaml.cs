using System;
using System.Collections.Generic;
using System.Dynamic;
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
using WPFUtilities.ImportExportHelpers;
using Ookii.Dialogs.Wpf;
using StringManager.Converters;

namespace StringManager.Controls
{
    /// <summary>
    /// Interaction logic for StringSelector.xaml
    /// </summary>
    public partial class FileSelector : UserControl
    {
        #region ExportOption
        public static readonly DependencyProperty ExportOptionProperty = DependencyProperty.Register("ExportOption", typeof(ExportOption), typeof(FileSelector), new UIPropertyMetadata(ExportOption.All));
        public ExportOption ExportOption
        {
            // IMPORTANT: To maintain parity between setting a property in XAML and procedural code, do not touch the getter and setter inside this dependency property!
            get
            {
                return (ExportOption)GetValue(ExportOptionProperty);
            }
            set
            {
                SetValue(ExportOptionProperty, value);
            }
        }
        #endregion

        #region Separator
        public static readonly DependencyProperty SeparatorProperty = DependencyProperty.Register("Separator", typeof(SepType), typeof(FileSelector), new UIPropertyMetadata(SepType.SemiColon));
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

        #region FilePath
        public static readonly DependencyProperty FilePathProperty = DependencyProperty.Register("FilePath", typeof(string), typeof(FileSelector), new UIPropertyMetadata(null));
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
        
        public FileSelector(bool bExport = false)
        {
            InitializeComponent();
            if (bExport)
                exportOptions.Visibility = Visibility.Visible;
        }

        private void SelectFile_Click(object sender, RoutedEventArgs e)
        {
            string file = String.Empty;
            string extension = Properties.Settings.Default.ImportExportFileExtension;
            if (Environment.UserInteractive)
            {
                VistaOpenFileDialog dialog = new VistaOpenFileDialog();
                dialog.CheckFileExists = false;
                dialog.ValidateNames = true;
                dialog.FileName = FilePath;
                dialog.DefaultExt = "CSV Files (*.csv)|*.csv";
                dialog.CheckFileExists = false;
                dialog.Multiselect = false;
                dialog.Filter = $"{extension.ToUpper()} Files (*.{extension.ToLower()})|*.{extension.ToLower()}";
                if (dialog.ShowDialog() == true)
                {
                    file = dialog.FileName;
                }
            }
            if (String.IsNullOrEmpty(file))
                return;

            //file = file.ToLower();
            if (string.IsNullOrEmpty(System.IO.Path.GetExtension(file)))
                file = string.Format("{0}.{1}", file, extension);

            if (System.IO.Path.GetExtension(file).ToLower().Equals(string.Format(".{0}", extension)))
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
