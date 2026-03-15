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
using OPCUAViewModel;
using Utilities;
using Utilities.WPF;
using UFProjectManager;
using System.ComponentModel;

namespace ClientEditor.Controls
{
    /// <summary>
    /// Interaction logic for AppNameSettingsEditor.xaml
    /// </summary>
    public partial class AppNameSettingsEditor : UserControl, IDisposable
    {
        #region declarations
        bool bCanAddEntries;
        AppNameSettings defAppNameSettings;
        bool bInit = false;
        string appName;
        #endregion

        #region ctor
        public AppNameSettingsEditor(Dictionary<String, AppNameSettings> mapAppNameSettings, AppNameSettings appNameSettings, string appName, bool bCanAdd = true)
        {
            InitializeComponent();
            defAppNameSettings = appNameSettings;
            this.appName = appName;
            MapAppNameSettings = mapAppNameSettings;
            bCanAddEntries = bCanAdd;
            stackAdd.Visibility = bCanAddEntries ? Visibility.Visible : Visibility.Collapsed;
            bInit = true;
        }
        #endregion

        #region Events
        public event EventHandler AppNameSettingsChanged;
        protected void OnAppNameSettingsChanged()
        {
            AppNameSettingsChanged?.Invoke(this, EventArgs.Empty);
        }
        #endregion

        #region IDisposable
        bool bDisposed;
        public void Dispose()
        {
            if (bDisposed)
                return;
            bDisposed = true;
            mapAppNameSettings.Values.ToList().ForEach(v => v.PropertyChanged -= apsettins_PropertyChanged);
            mapAppNameSettings.Clear();
        }
        #endregion

        #region method
        void InitSourceItems()
        {
            mapAppNameSettings.Keys.ToList().ForEach(name =>
            {
                mapAppNameSettings[name].SetOverriddenString(name);
            });

            if (mapAppNameSettings.ContainsKey(appName))
            {
                var appSettings = mapAppNameSettings[appName];
                if (defAppNameSettings != null)
                {
                    defAppNameSettings.RemoveDisabledItemAfterSecs = appSettings.RemoveDisabledItemAfterSecs;
                    defAppNameSettings.MaxCleanCount = appSettings.MaxCleanCount;
                    defAppNameSettings.UseAlwaysSecureConnections = appSettings.UseAlwaysSecureConnections;
                    defAppNameSettings.FastSamplingInterval = appSettings.FastSamplingInterval;
                    defAppNameSettings.SlowSamplingInterval = appSettings.SlowSamplingInterval;
                    defAppNameSettings.DisableWhenNotUsed = appSettings.DisableWhenNotUsed;
                    defAppNameSettings.PublishingInterval = appSettings.PublishingInterval;
                }
            }
            var orderedList = mapAppNameSettings.OrderBy(x => x.Key).Select(x => x.Value);
            listboxItems.ItemsSource = orderedList;
            layoutItems.DataContext = orderedList.FirstOrDefault();
        }

        readonly Dictionary<String, AppNameSettings> mapAppNameSettings = new Dictionary<String, AppNameSettings>();
        public Dictionary<String, AppNameSettings> MapAppNameSettings
        {
            get
            {
                return mapAppNameSettings;
            }
            set
            {
                mapAppNameSettings.Values.ToList().ForEach(v => v.PropertyChanged -= apsettins_PropertyChanged);
                mapAppNameSettings.Clear();
                foreach (var entry in value.Keys)
                {
                    mapAppNameSettings.Add(entry, value[entry]);
                    mapAppNameSettings[entry].PropertyChanged += apsettins_PropertyChanged;
                }
                InitSourceItems();
            }
        }

        private void apsettins_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if(bInit)
                OnAppNameSettingsChanged();
        }

        private void ButtonFetch_Click(object sender, RoutedEventArgs e)
        {
            var list = RealTimeConnectionManagerViewModel.GetActiveAppNames();
            if (list.Count == 0)
            {
                MessageBox.Show(Properties.Resources.NoAppNamesFetched);
                return;
            }
            bool notifyChanging = false;
            list.ForEach(name =>
                {
                    var found = (from c in mapAppNameSettings.Keys
                                 where c == name && !mapAppNameSettings[c].HasOverriddenString()  ||
                                 mapAppNameSettings[c].HasOverriddenString() && name == mapAppNameSettings[c].ToString()
                                 select c).ToList();
                    if (found.Count == 0)
                    {
                        notifyChanging = true;
                        mapAppNameSettings.Add(name, new AppNameSettings());
                        mapAppNameSettings[name].PropertyChanged += apsettins_PropertyChanged;
                    }
                });
            InitSourceItems();
            if(notifyChanging)
                OnAppNameSettingsChanged();
        }

        private void listboxItems_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            if (!bCanAddEntries || layoutItems.DataContext == null)
                return;
            
            var currentName = (layoutItems.DataContext as AppNameSettings).ToString();

            var newName = new NewAppNameEditor();
            newName.txtName.Text = currentName;
            while (true)
            {
                GeneralDialogContent Dialog = new GeneralDialogContent(newName)
                {
                    Owner = this.FindParent<Window>(),
                    HelpLink = "NewAppNameEditor"
                };
                if (Dialog.ShowDialog() != true)
                {
                    return;
                }
                if (CheckValidName(newName.txtName.Text))
                    break;
            }

            var currItem = (layoutItems.DataContext as AppNameSettings);
            currItem.SetOverriddenString(newName.txtName.Text);
            listboxItems.ItemsSource = mapAppNameSettings.OrderBy(x => x.Key).Select(x => x.Value);
            listboxItems.SelectedItem = null;
            listboxItems.SelectedItem = currItem;
            if (!currentName.Equals(newName.txtName.Text))
                OnAppNameSettingsChanged();
        }

        bool CheckValidName(String name)
        {
            if (String.IsNullOrEmpty(name))
                return false;

            var found = (from c in mapAppNameSettings.Keys
                            where c == name && !mapAppNameSettings[c].HasOverriddenString()  ||
                            mapAppNameSettings[c].HasOverriddenString() && name == mapAppNameSettings[c].ToString()
                            select c).ToList();
            return found.Count == 0;
        }

        private void ButtonAdd_Click(object sender, RoutedEventArgs e)
        {
            var newName = new NewAppNameEditor();
            while (true)
            {
                GeneralDialogContent Dialog = new GeneralDialogContent(newName)
                {
                    Owner = this.FindParent<Window>(),
                    Title = Properties.Resources.AppNameEdit,
                    HelpLink = "NewAppNameEditor"
                };
                if (Dialog.ShowDialog() != true)
                {
                    return;
                }
                if (CheckValidName(newName.txtName.Text))
                    break;
            }

            var newsettings = new AppNameSettings();
            mapAppNameSettings.Add(newName.txtName.Text, newsettings);
            mapAppNameSettings[newName.txtName.Text].PropertyChanged += apsettins_PropertyChanged;
            InitSourceItems();
            OnAppNameSettingsChanged();
            layoutItems.DataContext = newsettings;
        }

        private void ButtonDelete_Click(object sender, RoutedEventArgs e)
        {
            if (layoutItems.DataContext == null)
                return;
            var currentName = layoutItems.DataContext;
            var found = (from c in mapAppNameSettings
                         where c.Value == layoutItems.DataContext
                         select c.Key).Single();
            if (found == appName)
                return;
            mapAppNameSettings.Remove(found);
            InitSourceItems();
            OnAppNameSettingsChanged();
        }

        private void String_Clear(object sender, RoutedEventArgs e)
        {
            (sender as Button).Tag = null;
        }

        private void PublishingInterval_Clear(object sender, RoutedEventArgs e)
        {
            AppNameSettings settings = new AppNameSettings();
            (sender as Button).Tag = settings.PublishingInterval;
        }

        private void SlowSamplingInterval_Clear(object sender, RoutedEventArgs e)
        {
            AppNameSettings settings = new AppNameSettings();
            (sender as Button).Tag = settings.SlowSamplingInterval;
        }

        private void FastSamplingInterval_Clear(object sender, RoutedEventArgs e)
        {
            AppNameSettings settings = new AppNameSettings();
            (sender as Button).Tag = settings.FastSamplingInterval;
        }

        private void MaxCleanCount_Clear(object sender, RoutedEventArgs e)
        {
            AppNameSettings settings = new AppNameSettings();
            (sender as Button).Tag = settings.MaxCleanCount;
        }

        private void RemoveDisabledItemAfterSecs_Clear(object sender, RoutedEventArgs e)
        {
            AppNameSettings settings = new AppNameSettings();
            (sender as Button).Tag = settings.RemoveDisabledItemAfterSecs;
        }
        #endregion
    }
}
