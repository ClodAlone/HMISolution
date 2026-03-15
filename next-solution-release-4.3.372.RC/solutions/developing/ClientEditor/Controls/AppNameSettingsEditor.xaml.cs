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
using DevExpress.Xpf.Core.Native;
using System.Collections;
using System.Runtime.InteropServices.WindowsRuntime;

namespace ClientEditor.Controls
{
    /// <summary>
    /// Interaction logic for AppNameSettingsEditor.xaml
    /// </summary>
    public partial class AppNameSettingsEditor : UserControl, IDisposable, INotifyPropertyChanged, IDataErrorInfo
    {
        #region declarations
        bool bCanAddEntries;
        AppNameSettings defAppNameSettings;
        bool bInit = false;
        string appName;
        GeneralDialogContent Dialog;
        string currentName;
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
                if (mapAppNameSettings[name].HasOverriddenString())
                {
                    mapAppNameSettings[name].SetOverriddenString(mapAppNameSettings[name].ToString());
                }
                else
                {
                    mapAppNameSettings[name].SetOverriddenString(name);                   
                }
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
            
            currentName = (layoutItems.DataContext as AppNameSettings).ToString();

            var newName = new NewAppNameEditor();
            NewName = currentName;
            newName.DataContext = this;
            
            Dialog = new GeneralDialogContent(newName)
            {
                Owner = this.FindParent<Window>(),
                HelpLink = "NewAppNameEditor"
            };            
            if (Dialog.ShowDialog() != true)
            {
                return;
            }

            RemoveFromMap(layoutItems.DataContext);
            AddToMap(layoutItems.DataContext as AppNameSettings);
        }

        private void ButtonAdd_Click(object sender, RoutedEventArgs e)
        {
            NewName = "";
            currentName = "";

            var newName = new NewAppNameEditor();
            newName.DataContext = this;
            
            Dialog = new GeneralDialogContent(newName)
            {
                Owner = this.FindParent<Window>(),
                Title = Properties.Resources.AppNameEdit,
                HelpLink = "NewAppNameEditor"
            };
            if (Dialog.ShowDialog() != true)
            {
                return;
            }

            AddToMap();
        }

        private void ButtonDelete_Click(object sender, RoutedEventArgs e)
        {
            if (layoutItems.DataContext == null)
                return;
            
            RemoveFromMap(layoutItems.DataContext);
            UpdateItemsSource();
        }

        private void RemoveFromMap(object itemToRemove)
        {
            var currentName = layoutItems.DataContext;
            var found = (from c in mapAppNameSettings
                         where c.Value == itemToRemove
                         select c.Key).Single();
            if (found == appName)
                return;

            if(found != null)
            {
                mapAppNameSettings[found].PropertyChanged -= apsettins_PropertyChanged;
                mapAppNameSettings.Remove(found);
            }            
        }

        private void AddToMap(AppNameSettings settings = null)
        {
            if(settings == null)
                settings = new AppNameSettings();           

            mapAppNameSettings.Add(NewName, settings);
            mapAppNameSettings[NewName].SetOverriddenString(null);
            mapAppNameSettings[NewName].PropertyChanged += apsettins_PropertyChanged;
            UpdateItemsSource();
            layoutItems.DataContext = settings;
        }

        private void UpdateItemsSource()
        {
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

        #region Public properties
        private string newName;
        public string NewName
        { get { return newName; }
            set 
            { 
                newName = value;
                OnPropertyChanged(nameof(NewName));
            }
        }
        #endregion

        #region IDataErrorInfo
        public string Error
        {
            get
            {
                var context = new System.ComponentModel.DataAnnotations.ValidationContext(this, null, null);
                var results = new List<System.ComponentModel.DataAnnotations.ValidationResult>();

                return !System.ComponentModel.DataAnnotations.Validator.TryValidateObject(this, context, results)
                    ? string.Join(Environment.NewLine, results.Select(x => x.ErrorMessage))
                    : null;
            }
        }

        public string this[string propertyname] 
        { 
            get             
            {
                String s = PerformValidation(propertyname);
                if (!String.IsNullOrEmpty(s))
                    return s;
                var context = new System.ComponentModel.DataAnnotations.ValidationContext(this, null, null)
                {
                    MemberName = propertyname
                };

                var results = new List<System.ComponentModel.DataAnnotations.ValidationResult>();
                var propertyInfo = GetType().GetProperty(propertyname);
                if (propertyInfo != null)
                {
                    var value = propertyInfo.GetValue(this, null);

                    return !System.ComponentModel.DataAnnotations.Validator.TryValidateProperty(value, context, results)
                        ? string.Join(Environment.NewLine, results.Select(x => x.ErrorMessage))
                        : null;
                }

                return null;
            } 
        }

        private string PerformValidation(string propertyName)
        {
            if(propertyName == nameof(NewName))
            {
                if(String.IsNullOrEmpty(NewName))
                {
                    return Properties.Resources.NameNull;                    
                }
                else if(currentName == NewName)
                    return null;
                else
                {
                    var found = (from c in mapAppNameSettings.Keys
                                 where c == NewName && !mapAppNameSettings[c].HasOverriddenString() ||
                                 mapAppNameSettings[c].HasOverriddenString() && NewName == mapAppNameSettings[c].ToString()
                                 select c).ToList();

                    if(found.Any())
                    {
                        return Properties.Resources.NameAlreadyExist;
                    }
                    return null;
                }
            }
            return null;
        }
        #endregion

       
        #region INotifyPropertyChanged Members
        
        public event PropertyChangedEventHandler PropertyChanged;
        public event EventHandler<DataErrorsChangedEventArgs> ErrorsChanged;

        protected void OnPropertyChanged(string propertyName)
        {
            PropertyChangedEventHandler handler = PropertyChanged;
            if (handler != null)
            {
                var e = new PropertyChangedEventArgs(propertyName);
                handler(this, e);
            }
        }
        
        #endregion
    }
}
