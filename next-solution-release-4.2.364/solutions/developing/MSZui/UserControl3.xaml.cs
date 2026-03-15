using MSZ;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using Utilities;

namespace MSZui
{
    /// <summary>
    /// Interaction logic for UserControl3.xaml
    /// </summary>
    public partial class UserControl3 : UserControl
    {
        #region Public Props
        public LicenseDataModel LicenseModel {
            get;
            private set;
        }
        #endregion

        #region Ctor
        public UserControl3(bool onlyServerOptions, bool bRemote = false)
        {
            InitializeComponent();

            Dictionary<string, string> licenseOpts = new Dictionary<string, string>();
            foreach (System.Configuration.SettingsProperty setting in Properties.Settings.Default.Properties)
            {
                if (setting.PropertyType == typeof(string))
                    licenseOpts.Add(setting.Name, setting.DefaultValue as string);
            }
            LicenseModel = new LicenseDataModel(onlyServerOptions) { OptionsTags = licenseOpts };
            DataContext = LicenseModel;

            mainGroup.Visibility = Visibility.Collapsed;
            SetBusy(true);
            if (!bRemote)
                LoadLocalLicenseData();
            else
                remoteLicenseInfo.Visibility = Visibility.Visible;
        }
        #endregion

        #region Methods
        public void LoadLocalLicenseData()
        {
            LicenseModel.LoadDataAsync().ContinueWith(ret =>
            {
                UpdateUI();
            }, TaskScheduler.FromCurrentSynchronizationContext());
        }

        public void UpdateUI()
        {
            foreach (var key in LicenseModel.BOptionMap)
            {
                var fe = FindName(key) as CheckBox;
                if (fe != null && !LicenseModel.ClientOptions)
                {
                    Visibility _vis = (bool)Properties.Settings.Default[string.Format("EnTag{0}", key)] ? Visibility.Visible : Visibility.Collapsed;
                    fe.Visibility = _vis;
                }
            }

            foreach (var key in LicenseModel.IOptionMap)
            {
                var fe = FindName(key) as TextBox;
                if (fe != null && !LicenseModel.ClientOptions)
                {
                    Visibility _vis = (bool)Properties.Settings.Default[string.Format("EnTag{0}", key)] ? Visibility.Visible : Visibility.Collapsed;
                    fe.Visibility = _vis;
                    var fe2 = FindName($"T{key}") as UIElement;
                    if (fe2 != null)
                        fe2.Visibility = _vis;
                }
            }
            mainGroup.Visibility = Visibility.Visible;
            SetBusy(false);
        }

        private void SelectAll_onFocus(object sender, RoutedEventArgs e)
        {
            (sender as TextBox).SelectAll();
        }

        private void SelectAll_onMouseDown(object sender, RoutedEventArgs e)
        {
            (sender as TextBox).Focus();
            e.Handled = true;
        }
        #endregion

        #region Busy Content
        void SetBusy(bool bSet)
        {
            busyControl.Visibility = bSet ? Visibility.Visible : Visibility.Collapsed;
            busyContent.Visibility = busyControl.Visibility;
            busyControl.Refresh();
            busyContent.Refresh();
        }
        #endregion
    }
}
