using CommonControls.Converters;
using System;
using System.ComponentModel;
using System.Windows;
using System.Windows.Controls;
using UIMsgBoxAlertService.ComponentService;

namespace CommonControls
{
    [TypeConverter(typeof(LocalizedEnumConverter))]
    public enum SQLiteDateTimeKind
    {
        Unspecified,
        Utc,
        Local
    }

    /// <summary>
    /// Interaction logic for SQLiteConnectControl.xaml
    /// </summary>
    public partial class SQLiteConnectControl : UserControl
    {
        #region DP
        #region UIService
        public static readonly DependencyProperty UIServiceProperty = DependencyProperty.Register("UIService", typeof(IUIMsgBoxAlertService), typeof(SQLiteConnectControl), new UIPropertyMetadata(null));

        public IUIMsgBoxAlertService UIService
        {
            // IMPORTANT: To maintain parity between setting a property in XAML and procedural code, do not touch the getter and setter inside this dependency property!
            get
            {
                return (IUIMsgBoxAlertService)GetValue(UIServiceProperty);
            }
            set
            {
                SetValue(UIServiceProperty, value);
            }
        }
        #endregion
        #endregion

        public SQLiteConnectControl()
        {
            InitializeComponent();
        }

        public string GetDataBaseConnectionString()
        {
            return String.Format("data source={0};password={1};DateTimeKind={2}", tbDatabase.Text, tbPassword.Password, cbDateTimeKind.EditValue);
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            if (UIService == null)
                return;

            String file = UIService.ShowOpenFileDialog(Properties.Settings.Default.SQLiteFileType, 
                new FileOpenOptions() 
                { 
                    CheckFileExists = false, 
                    AddExtension = true
                });
            if (!String.IsNullOrEmpty(file))
                tbDatabase.Text = file;
        }
    }
}
