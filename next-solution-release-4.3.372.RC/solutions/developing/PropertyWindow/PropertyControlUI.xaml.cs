using System;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Input;
using System.Windows.Media;
using Utilities;
using Utilities.WPF;
using System.ComponentModel;
using Mindscape.WpfElements.WpfPropertyGrid;
using System.Collections.Specialized;
using Mindscape.WpfElements.PropertyEditing;
using System.IO;
using PropertyControl.ComponentService;
using PropertyControl.Localization;
using WPFUtilities;
using System.Collections;
using System.Collections.Generic;
using static PropertyControl.PropertyVisibleState;

namespace PropertyControl
{
    /// <summary>
    /// Interaction logic for PropertyControlUI.xaml
    /// </summary>
    public partial class PropertyControlUI : UserControl, IDisposable, INotifyPropertyChanged
    {
        #region Declaration

        readonly static PropertyGroupDescription nodeByCategory = new PropertyGroupDescription("Node", new LocalizableNodeToCategoryConverter(), StringComparison.CurrentCultureIgnoreCase);
        readonly static PropertyGroupDescription nodeByType = new PropertyGroupDescription("Node", new PropertyTypeConverter());
        readonly PropertyGroupDescription nodeByAdvanced;

        readonly static SortDescription sortByTypeName = new SortDescription("Node.PropertyType.FullName", ListSortDirection.Ascending);
        readonly static SortDescription sortByPropertyName = new SortDescription("Node.HumanName", ListSortDirection.Ascending);
        readonly static SortDescription sortByCategoryPriority = new SortDescription("Node.CategoryPriority", ListSortDirection.Ascending);
        readonly static SortDescription sortByPriority = new SortDescription("Node.Priority", ListSortDirection.Ascending);
        readonly static SortDescription sortByAdvPriority = new SortDescription("Node.AdvPropertyPriority", ListSortDirection.Ascending);

        readonly Dictionary<string, Dictionary<string, PropertyVisibleState>> mapTypePropertyVisibleStates = new Dictionary<string, Dictionary<string, PropertyVisibleState>>();

        readonly PropertyResourceManager resourceManager;
        readonly PriorityResourceManager priorityManager;
        bool bLoaded;

        internal bool isPopup;

        const string FolderNormalMode = "PropertyStates";
        const string FolderConfigMode = "PropertyStatesConfig";

        const string patternFullName = @"[a-zA-Z0-9._]$";
        System.Collections.ObjectModel.ObservableCollection<String> filterList = new System.Collections.ObjectModel.ObservableCollection<string>();

        internal static int maxCategoryPriority = int.MaxValue - 1;
        internal static int maxAdvancedCategoryPriority = int.MaxValue;
        #endregion

        #region Constructors

        public PropertyControlUI()
        {
            InitializeComponent();

            for (int ii = ComponentService.PropertyControlComponent.propertyEditors.Count - 1; ii >= 0; ii--)
                propertyGrid.Editors.Insert(0, ComponentService.PropertyControlComponent.propertyEditors[ii]);
            ComponentService.PropertyControlComponent.propertyEditors.CollectionChanged += propertyEditors_CollectionChanged;

            nodeByAdvanced = new PropertyGroupDescription("Node", new PropertyAdvancedConverter(this));

            Loaded += (o, e) =>
            {
                if (bLoaded)
                    return;
                bLoaded = true;
                Dispatcher.BeginInvokeAsynchronouslyInRender(() =>
                {
                    if (bDisposed)
                        return;

                    textSearchFilter.DataContext = filterList;
                    bSetPropertyFilter = true;
                    RefreshGrid();
                });
            };

            SetGridFilter();

            if (IsConfigModeEnabled)
            {
                PropertyResourceManager.bDisableUILocalization = true;
                PriorityResourceManager.bDisableUILocalization = true;
                propertyGrid.SelectedGridItemChanged += (s, e) =>
                {
                    OnPropertyChanged("IsPropertyShowHideEnabled");
                    OnPropertyChanged("IsPropertyHide");
                    OnPropertyChanged("IsPropertyShowInEasyMode");
                    OnPropertyChanged("IsAdvancedProperty");
                    OnPropertyChanged("SelectedPropertyTypeOwner");
                };
            }

            resourceManager = new PropertyResourceManager("PropertyControlUI");
            priorityManager = new PriorityResourceManager(Properties.Settings.Default.CategoryConfigFileName, "CategoryOrder");

            var currentStyle = ApplicationPropertiesHelper.GetProperty<String>("CurrentSkin");
            if (!String.IsNullOrEmpty(currentStyle))
            {
                try
                {
                    var uriStyle = new Uri(String.Format("/PropertyControl;component/Themes/ButtonStyle_{0}.xaml", currentStyle), UriKind.RelativeOrAbsolute);
                    Resources.MergedDictionaries.Add(new ResourceDictionary() { Source = uriStyle });
                }
                catch
                { }
            }

            ShowCustomExpander(null);
        }

        #endregion

        #region Public Methods
        public string GetString(string name)
        {
            var s = resourceManager.GetString(name);
            return s ?? name;
        }
        #endregion

        #region Properties
        internal bool IsDisposed
        {
            get
            {
                return bDisposed;
            }
        }
        #endregion

        #region Property Custom Expander

        readonly Dictionary<Type, double> mapControlHeights = new Dictionary<Type, double>();
        internal void ShowCustomExpander(UserControl control)
        {
            if (contentControl.Content != null)
            {
                var type = contentControl.Content.GetType();
                var height = rowContent.Height.Value;
                mapControlHeights[type] = rowContent.Height.Value;
            }

            if (contentControl.Content is IDisposable && contentControl.Content != control)
                (contentControl.Content as IDisposable).Dispose();
            contentControl.Content = control;
            if (control != null)
            {
                var type = control.GetType();
                var height = control.Height;
                if (mapControlHeights.ContainsKey(type))
                    height = mapControlHeights[type];
                if (Double.IsNaN(height))
                    height = 200;
                control.ClearValue(FrameworkElement.WidthProperty);
                control.ClearValue(FrameworkElement.HeightProperty);

                rowSplitter.Height = new GridLength(3);
                rowContent.Height = new GridLength(height);
                contentControl.Visibility = Visibility.Visible;

                var currentStyle = ApplicationPropertiesHelper.GetProperty<String>("CurrentSkin");
                ThemeHelper.SetTheme(control, currentStyle);
            }
            else
            {
                rowSplitter.Height = new GridLength(0);
                rowContent.Height = new GridLength(0);
                contentControl.Visibility = Visibility.Collapsed;
            }
        }

        #endregion

        #region Property Editors Manager

        void propertyEditors_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            if (e.NewItems != null && e.NewItems.Count != 0)
            {
                foreach (var v in e.NewItems)
                    propertyGrid.Editors.Insert(0, v as Editor);
            }

            if (e.OldItems != null && e.OldItems.Count != 0)
            {
                foreach (var v in e.OldItems)
                    propertyGrid.Editors.Remove(v as Editor);
            }
        }

        Brush GetBrushChanged(Brush brush)
        {
            Brush ret = brush;

            if (brush is LinearGradientBrush)
            {
                var c = brush as LinearGradientBrush;
                var b = new LinearGradientBrush();
                b.ColorInterpolationMode = c.ColorInterpolationMode;
                b.EndPoint = c.EndPoint;
                b.StartPoint = c.StartPoint;
                b.MappingMode = c.MappingMode;
                b.Opacity = c.Opacity;
                b.SpreadMethod = c.SpreadMethod;
                foreach (var gradient in c.GradientStops)
                    b.GradientStops.Add(new GradientStop()
                    {
                        Color = gradient.Color,
                        Offset = gradient.Offset
                    });

                ret = b;
            }
            else if (brush is RadialGradientBrush)
            {
                var c = brush as RadialGradientBrush;
                var b = new RadialGradientBrush();
                b.RadiusX = c.RadiusX;
                b.RadiusY = c.RadiusY;
                b.GradientOrigin = c.GradientOrigin;
                b.Center = c.Center;
                b.ColorInterpolationMode = c.ColorInterpolationMode;
                b.MappingMode = c.MappingMode;
                b.Opacity = c.Opacity;
                b.SpreadMethod = c.SpreadMethod;
                foreach (var gradient in c.GradientStops)
                    b.GradientStops.Add(new GradientStop()
                    {
                        Color = gradient.Color,
                        Offset = gradient.Offset
                    });

                ret = b;
            }

            return ret;
        }

        private void ContextualHelp_Clicked(object sender, MouseButtonEventArgs e)
        {

        }

        #endregion Property Editors Manager

        #region Property Visible State Manger

        internal bool IsPropertyVisible(PropertyDescriptor property, object source)
        {
            // disable some attached properties
            if (property.Name.Contains('.') && !IsValidAttachedProperty(property.Name))
                return false;

            if (!IsShowAllSet)
            {
                var sourceType = source.GetType();
                if (!IsPropertyVisible(sourceType, property.Name) || 
                    !IsPropertyVisible(property.ComponentType, property.Name) ||
                    (bPropertySetEasyMode && 
                    (!IsPropertyVisibleInEasyMode(sourceType, property.Name) ||
                    !IsPropertyVisibleInEasyMode(property.ComponentType, property.Name))))
                    return false;

                if (!String.IsNullOrEmpty(ProjectType))
                {
                    List<string> strings = new List<string>();
                    strings = GetProjectsTypeToHide(property.ComponentType, property.Name);

                    if (strings != null)
                    {
                        if (strings.Contains(ProjectType))
                            return false;
                    }
                }
            }

            if (PropertyFilterState == PropertyFilterEnum.SortByPriority)
            {
                var priority = GetPropertyPriority(property.ComponentType, property.Name);
                if (priority == 0)
                    return false;
            }


            if (bPropertyWritableFilter)
            {
                if (property.IsReadOnly)
                    return false;
            }

            DependencyObject depObject = null;
            if (source is ICustomTypeDescriptor)
            {
                var typeDesc = source as ICustomTypeDescriptor;
                depObject = typeDesc.GetPropertyOwner(property) as DependencyObject;
            }
            else if (source is DependencyObject)
                depObject = source as DependencyObject;

            if (depObject != null)
            {
                DependencyPropertyDescriptor dpd =
                    DependencyPropertyDescriptor.FromProperty(property);
                if (dpd != null)
                {
                    var depValue = depObject.ReadLocalValue(dpd.DependencyProperty);
                    if (depValue is BindingExpression ||
                        (bPropertySetFilter && depObject.ReadLocalValue(dpd.DependencyProperty) == DependencyProperty.UnsetValue))
                        return false;
                }
            }

            if (bPropertySetFilter)
            {
                object value = property.GetValue(source);
                if (value == null)
                    return false;
            }

            if (!String.IsNullOrEmpty(textSearchFilter.EditValue as String))
            {
                var prop = new LocalizablePropertyDescriptor(property);
                return prop.DisplayName.IndexOf(textSearchFilter.EditValue as String, StringComparison.CurrentCultureIgnoreCase) >= 0;
            }

            return true;
        }

        bool IsPropertyVisible(Type objectType, String propertyName)
        {
            Dictionary<string, PropertyVisibleState> mapVisibleStates;
            FillMapVisibleState(objectType, out mapVisibleStates);
            if (mapVisibleStates.ContainsKey(propertyName))
                return !mapVisibleStates[propertyName].IsAlwaysHiden;

            return true;
        }

        bool IsPropertyVisibleInEasyMode(Type objectType, String propertyName)
        {
            Dictionary<string, PropertyVisibleState> mapVisibleStates;
            FillMapVisibleState(objectType, out mapVisibleStates);
            if (mapVisibleStates.ContainsKey(propertyName))
                return mapVisibleStates[propertyName].IsShowInEsyMode;

            return false;
        }

        List<string> GetProjectsTypeToHide(Type objectType, String propertyName)
        {
            Dictionary<string, PropertyVisibleState> mapVisibleStates;
            FillMapVisibleState(objectType, out mapVisibleStates);
            if (mapVisibleStates.ContainsKey(propertyName))
                return mapVisibleStates[propertyName].ProjectTypeToHide;

            return null;
        }

        internal bool IsAdvancedPropertyName(Node node)
        {
            return IsAdvancedPropertyName(node.PropertyInfo.DeclaringType, node.PropertyInfo.Name);
        }

        bool IsAdvancedPropertyName(Type objectType, String propertyName)
        {
            Dictionary<string, PropertyVisibleState> mapVisibleStates;
            FillMapVisibleState(objectType, out mapVisibleStates);
            if (mapVisibleStates.ContainsKey(propertyName))
                return mapVisibleStates[propertyName].IsAdvancedProperty;

            return false;
        }

        internal static bool IsValidAttachedProperty(String propertyName)
        {
            return propertyName == "Canvas.Left" ||
                   propertyName == "Canvas.Top";

        }

        int GetPropertyPriority(Type objectType, String propertyName)
        {
            Dictionary<string, PropertyVisibleState> mapVisibleStates;
            FillMapVisibleState(objectType, out mapVisibleStates);
            if (mapVisibleStates.ContainsKey(propertyName))
                return mapVisibleStates[propertyName].Priority;

            return 0;
        }

        void FillMapVisibleState(Type objectType, out Dictionary<string, PropertyVisibleState> mapToFill)
        {
            if (!mapTypePropertyVisibleStates.TryGetValue(objectType.FullName, out mapToFill))
            {
                mapToFill = new Dictionary<string, PropertyVisibleState>();
                mapTypePropertyVisibleStates[objectType.FullName] = mapToFill;

                string filepath = String.Format("{0}\\{1}.xml", PropertyVisibleStatePath, TakeValidPartOfTypeFullName(objectType.FullName));
                PropertyVisibleState.LoadPropertyVisibleStatesFromXml(filepath, mapToFill);
            }
        }

        bool bSetPropertyFilter = true;
        PropertyFilterEnum propertyFilterState;
        public PropertyFilterEnum PropertyFilterState
        {
            get 
            {
                return propertyFilterState;
            }
            set
            {
                if (propertyFilterState == value)
                    return;

                bSetPropertyFilter = true;
                propertyFilterState = value;
                OnPropertyChanged("PropertyFilterState");
                OnPropertyChanged("IsExpandCollapseEnabled");
                
                RefreshGrid(true);
            }
        }

        public bool IsExpandCollapseEnabled
        {
            get
            {
                return PropertyFilterState == PropertyFilterEnum.GroupByCategory || 
                    PropertyFilterState == PropertyFilterEnum.GroupByType;
            }
        }

        /*
        bool bPropertySetGroupByCategory = true;
        public bool IsPropertySetGroupByCategory
        {
            get
            {
                return bPropertySetGroupByCategory;
            }
            set
            {
                if (bPropertySetGroupByCategory == value)
                    return;

                bPropertySetGroupByCategory = value;
                if (bPropertySetGroupByCategory)
                {
                    bPropertySetGroupByType = false;
                    bPropertySortAlhabetically = false;
                }


                OnPropertyChanged("IsPropertySetGroupByCategory");
                OnPropertyChanged("IsPropertySortAlphabetically");
                OnPropertyChanged("IsPropertySetGroupByType");

                RefreshGrid(true);
            }
        }

        bool bPropertySetGroupByType = false;
        public bool IsPropertySetGroupByType
        {
            get
            {
                return bPropertySetGroupByType;
            }
            set
            {
                if (bPropertySetGroupByType == value)
                    return;

                bPropertySetGroupByType = value;
                if (bPropertySetGroupByType)
                {
                    bPropertySetGroupByCategory = false;
                    bPropertySortAlhabetically = false;
                }
                OnPropertyChanged("IsPropertySetGroupByType");
                OnPropertyChanged("IsPropertySortAlphabetically");
                OnPropertyChanged("IsPropertySetGroupByCategory");

                RefreshGrid(true);
            }
        }

        bool bPropertySortAlhabetically = false;
        public bool IsPropertySortAlphabetically
        {
            get
            {
                return bPropertySortAlhabetically;
            }
            set
            {
                if (bPropertySortAlhabetically == value)
                    return;

                bPropertySortAlhabetically = value;
                if (bPropertySortAlhabetically)
                {
                    bPropertySetGroupByType = false;
                    bPropertySetGroupByCategory = false;
                }

                OnPropertyChanged("IsPropertySortAlphabetically");
                OnPropertyChanged("IsPropertySetGroupByType");
                OnPropertyChanged("IsPropertySetGroupByCategory");

                RefreshGrid(true);
            }
        }
        */
        
        string PropertyVisibleStatePath
        {
            get
            {
                string mainversion = Utilities.AssemblyInfo.FileFormatMainVersion;
                return String.Format("{0}.{2}\\{1}", 
                                    Utilities.ApplicationPropertiesHelper.GetProperty<String>("CommonFolder"), 
                                    IsConfigModeEnabled ? FolderConfigMode : FolderNormalMode, mainversion);
            }
        }
        
        public bool IsConfigModeEnabled
        {
            get
            {
                string mainversion = Utilities.AssemblyInfo.FileFormatMainVersion;
                string path = String.Format("{0}.{1}\\PropertyStatesConfig", Utilities.ApplicationPropertiesHelper.GetProperty<String>("CommonFolder"),mainversion);
                return Directory.Exists(path);
            }
        }

        public string SelectedPropertyTypeOwner
        {
            get
            {
                if (propertyGrid != null && propertyGrid.SelectedGridItem != null && propertyGrid.SelectedGridItem.Node != null)
                {
                    Node node = propertyGrid.SelectedGridItem.Node;
                    return node.PropertyInfo.DeclaringType.ToString();
                }

                return String.Empty;
            }
        }

        string title;
        public string Title
        {
            get
            {
                if (title == null)
                    return null;

                var s = resourceManager.GetString(title);
                return s ?? title;
            }
            set
            {
                if (title == value)
                    return;

                title = value;
                OnPropertyChanged("Title");
            }
        }

        public bool IsMultipleSelection
        {
            get
            {
                return propertyGrid != null && propertyGrid.SelectedObjects != null && propertyGrid.SelectedObjects.Count > 1;
            }
        }

        bool bShowAllSet = false;
        public bool IsShowAllSet
        {
            get
            {
                return bShowAllSet;
            }
            set
            {
                if (bShowAllSet == value)
                    return;

                bShowAllSet = value;
                OnPropertyChanged("IsShowAllSet");

                RefreshGrid(true);
            }
        }

        public bool IsPropertyHide
        {
            get
            {
                if (propertyGrid != null && propertyGrid.SelectedGridItem != null && propertyGrid.SelectedGridItem.Node != null)
                {
                    Node node = propertyGrid.SelectedGridItem.Node;

                    Dictionary<string, PropertyVisibleState> mapVisibleStates;
                    FillMapVisibleState(node.PropertyInfo.DeclaringType, out mapVisibleStates);
                    if (mapVisibleStates.ContainsKey(node.PropertyInfo.Name))
                        return mapVisibleStates[node.PropertyInfo.Name].IsAlwaysHiden;
                }

                return IsPropertySetEasyMode;
            }
            set
            {
                if (propertyGrid != null && propertyGrid.SelectedGridItem != null && propertyGrid.SelectedGridItem.Node != null)
                {
                    Node node = propertyGrid.SelectedGridItem.Node;

                    Dictionary<string, PropertyVisibleState> mapVisibleStates;
                    FillMapVisibleState(node.PropertyInfo.DeclaringType, out mapVisibleStates);

                    PropertyVisibleState state = PropertyVisibleState.UnsetValue;
                    if (mapVisibleStates.ContainsKey(node.PropertyInfo.Name))
                        state = mapVisibleStates[node.PropertyInfo.Name];
                    
                    state.IsAlwaysHiden = value;

                    if (state.IsAlwaysHiden)
                        state.IsShowInEsyMode = false;
                    
                    mapVisibleStates[node.PropertyInfo.Name] = state;

                    string filepath = String.Format("{0}\\{1}.xml", PropertyVisibleStatePath, TakeValidPartOfTypeFullName(node.PropertyInfo.DeclaringType.FullName));
                    PropertyVisibleState.WritePropertyVisibleStatesToXml(filepath, mapVisibleStates);
                }

                OnPropertyChanged("IsPropertyHide");
                OnPropertyChanged("IsPropertyShowInEasyMode");

                if (!IsShowAllSet)
                    RefreshGrid(true);
            }
        }

        public int SortPriority
        {
            get
            {
                if (propertyGrid != null && propertyGrid.SelectedGridItem != null && propertyGrid.SelectedGridItem.Node != null)
                {
                    Node node = propertyGrid.SelectedGridItem.Node;

                    Dictionary<string, PropertyVisibleState> mapVisibleStates;
                    FillMapVisibleState(node.PropertyInfo.DeclaringType, out mapVisibleStates);
                    if (mapVisibleStates.ContainsKey(node.PropertyInfo.Name))
                        return mapVisibleStates[node.PropertyInfo.Name].Priority;
                }

                return 0;
            }
            set
            {
                if (propertyGrid != null && propertyGrid.SelectedGridItem != null && propertyGrid.SelectedGridItem.Node != null)
                {
                    Node node = propertyGrid.SelectedGridItem.Node;

                    Dictionary<string, PropertyVisibleState> mapVisibleStates;
                    FillMapVisibleState(node.PropertyInfo.DeclaringType, out mapVisibleStates);

                    PropertyVisibleState state = PropertyVisibleState.UnsetValue;
                    if (mapVisibleStates.ContainsKey(node.PropertyInfo.Name))
                        state = mapVisibleStates[node.PropertyInfo.Name];

                    state.Priority = value;

                    mapVisibleStates[node.PropertyInfo.Name] = state;

                    string filepath = String.Format("{0}\\{1}.xml", PropertyVisibleStatePath, TakeValidPartOfTypeFullName(node.PropertyInfo.DeclaringType.FullName));
                    PropertyVisibleState.WritePropertyVisibleStatesToXml(filepath, mapVisibleStates);
                }

                OnPropertyChanged("SortPriority");
                //RefreshGrid(true);
            }
        }

        public bool IsPropertyShowInEasyMode
        {
            get
            {
                if (propertyGrid != null && propertyGrid.SelectedGridItem != null && propertyGrid.SelectedGridItem.Node != null)
                {
                    Node node = propertyGrid.SelectedGridItem.Node;

                    Dictionary<string, PropertyVisibleState> mapVisibleStates;
                    FillMapVisibleState(node.PropertyInfo.DeclaringType, out mapVisibleStates);
                    if (mapVisibleStates.ContainsKey(node.PropertyInfo.Name))
                        return mapVisibleStates[node.PropertyInfo.Name].IsShowInEsyMode;
                }

                return false;
            }
            set
            {
                if (propertyGrid != null && propertyGrid.SelectedGridItem != null && propertyGrid.SelectedGridItem.Node != null)
                {
                    Node node = propertyGrid.SelectedGridItem.Node;

                    Dictionary<string, PropertyVisibleState> mapVisibleStates;
                    FillMapVisibleState(node.PropertyInfo.DeclaringType, out mapVisibleStates);

                    PropertyVisibleState state = PropertyVisibleState.UnsetValue;
                    if (mapVisibleStates.ContainsKey(node.PropertyInfo.Name))
                        state = mapVisibleStates[node.PropertyInfo.Name];

                    state.IsShowInEsyMode = value;

                    if (state.IsShowInEsyMode)
                        state.IsAlwaysHiden = false;

                    mapVisibleStates[node.PropertyInfo.Name] = state;

                    string filepath = String.Format("{0}\\{1}.xml", PropertyVisibleStatePath, TakeValidPartOfTypeFullName(node.PropertyInfo.DeclaringType.FullName));
                    PropertyVisibleState.WritePropertyVisibleStatesToXml(filepath, mapVisibleStates);
                }

                OnPropertyChanged("IsPropertyShowInEasyMode");
                OnPropertyChanged("IsPropertyHide");

                if (!IsShowAllSet && IsPropertySetEasyMode)
                    RefreshGrid(true);
            }
        }

        public bool IsAdvancedProperty
        {
            get
            {
                if (propertyGrid != null && propertyGrid.SelectedGridItem != null && propertyGrid.SelectedGridItem.Node != null)
                {
                    Node node = propertyGrid.SelectedGridItem.Node;

                    Dictionary<string, PropertyVisibleState> mapVisibleStates;
                    FillMapVisibleState(node.PropertyInfo.DeclaringType, out mapVisibleStates);
                    if (mapVisibleStates.ContainsKey(node.PropertyInfo.Name))
                        return mapVisibleStates[node.PropertyInfo.Name].IsAdvancedProperty;
                }

                return false;
            }
            set
            {
                if (propertyGrid != null && propertyGrid.SelectedGridItem != null && propertyGrid.SelectedGridItem.Node != null)
                {
                    Node node = propertyGrid.SelectedGridItem.Node;

                    Dictionary<string, PropertyVisibleState> mapVisibleStates;
                    FillMapVisibleState(node.PropertyInfo.DeclaringType, out mapVisibleStates);

                    PropertyVisibleState state = PropertyVisibleState.UnsetValue;
                    if (mapVisibleStates.ContainsKey(node.PropertyInfo.Name))
                        state = mapVisibleStates[node.PropertyInfo.Name];

                    state.IsAdvancedProperty = value;

                    mapVisibleStates[node.PropertyInfo.Name] = state;

                    string filepath = String.Format("{0}\\{1}.xml", PropertyVisibleStatePath, TakeValidPartOfTypeFullName(node.PropertyInfo.DeclaringType.FullName));
                    PropertyVisibleState.WritePropertyVisibleStatesToXml(filepath, mapVisibleStates);
                }

                OnPropertyChanged("IsAdvancedProperty");

                RefreshGrid(true);
            }
        }

        public string ProjectTypeToHide
        {
            get
            {
                if (propertyGrid != null && propertyGrid.SelectedGridItem != null && propertyGrid.SelectedGridItem.Node != null)
                {
                    Node node = propertyGrid.SelectedGridItem.Node;

                    Dictionary<string, PropertyVisibleState> mapVisibleStates;
                    FillMapVisibleState(node.PropertyInfo.DeclaringType, out mapVisibleStates);
                    if (mapVisibleStates.ContainsKey(node.PropertyInfo.Name))
                        return String.Join("|", mapVisibleStates[node.PropertyInfo.Name].ProjectTypeToHide);
                }

                return null;
            }
            set
            {
                if (propertyGrid != null && propertyGrid.SelectedGridItem != null && propertyGrid.SelectedGridItem.Node != null)
                {
                    Node node = propertyGrid.SelectedGridItem.Node;

                    Dictionary<string, PropertyVisibleState> mapVisibleStates;
                    FillMapVisibleState(node.PropertyInfo.DeclaringType, out mapVisibleStates);

                    PropertyVisibleState state = PropertyVisibleState.UnsetValue;
                    if (mapVisibleStates.ContainsKey(node.PropertyInfo.Name))
                        state = mapVisibleStates[node.PropertyInfo.Name];

                    state.ProjectTypeToHide = value.Split('|').ToList();

                    mapVisibleStates[node.PropertyInfo.Name] = state;

                    string filepath = String.Format("{0}\\{1}.xml", PropertyVisibleStatePath, TakeValidPartOfTypeFullName(node.PropertyInfo.DeclaringType.FullName));
                    PropertyVisibleState.WritePropertyVisibleStatesToXml(filepath, mapVisibleStates);
                }

                OnPropertyChanged("ProjectTypeToHide");

                RefreshGrid(true);
            }
        }

        string projectType;
        public string ProjectType
        {
            get
            {
                if (projectType == null)
                    return null;

                return projectType;
            }
            set
            {
                if (projectType == value)
                    return;

                projectType = value;
                OnPropertyChanged("ProjectType");
            }
        }

        public bool IsPropertyShowHideEnabled
        {
            get
            {
                return propertyGrid != null && propertyGrid.SelectedGridItem != null && propertyGrid.SelectedGridItem.Node != null; 
            }
        }

        bool bPropertyWritableFilter = false;
        public bool IsPropertyWritableFilter
        {
            get 
            {
                return bPropertyWritableFilter;
            }
            set
            {
                if (bPropertyWritableFilter == value)
                    return;
                
                bPropertyWritableFilter = value;
                OnPropertyChanged("IsPropertyWritableFilter");

                RefreshGrid(true);
            }
        }

        bool bPropertySetFilter = false;
        public bool IsPropertySetFilter
        {
            get
            {
                return bPropertySetFilter;
            }
            set
            {
                if (bPropertySetFilter == value)
                    return;

                bPropertySetFilter = value;
                OnPropertyChanged("IsPropertySetFilter");

                RefreshGrid(true);
            }
        }

        bool bPropertySetFriendMode = false;
        public bool IsPropertySetFriendMode
        {
            get
            {
                return bPropertySetFriendMode;
            }
            set
            {
                if (bPropertySetFriendMode == value)
                    return;

                bPropertySetFriendMode = value;
                OnPropertyChanged("IsPropertySetFriendMode");

                //RefreshGrid(true);
            }
        }

        bool bPropertySetEasyMode = false;
        public bool IsPropertySetEasyMode
        {
            get
            {
                return bPropertySetEasyMode;
            }
            set
            {
                if (bPropertySetEasyMode == value)
                    return;

                bPropertySetEasyMode = value;
                OnPropertyChanged("IsPropertySetEasyMode");

                RefreshGrid(true);
            }
        }

        public bool IsPropertyGridVisible
        {
            get 
            {
                return propertyGrid != null && !propertyGrid.BindingView.DefaultView.IsEmpty;
            }
        }

        void filter_EditValueChanged(object sender, DevExpress.Xpf.Editors.EditValueChangedEventArgs e)
        {
            if (e.NewValue != null && !filterList.Contains(e.NewValue.ToString()))
                filterList.Insert(0,e.NewValue.ToString());
            if (filterList.Count > Properties.Settings.Default.MaxFilterListCount)
                filterList.RemoveAt(Properties.Settings.Default.MaxFilterListCount);
        }

        void SearchFilter_OnTextChanged(object sender, RoutedEventArgs e)
        {
            e.Handled = true;
            RefreshGrid(true);
        }

        void OnPropertyGridNodeExpanded(object sender, RoutedEventArgs e)
        {
            TreeViewItem tvi = (TreeViewItem)(e.OriginalSource);
            PropertyGridRow expandingRow = (PropertyGridRow)(tvi.Header);
            SetGridFilter(expandingRow.Children);
        }

        void OnPropertyGridGroupExpanded(object sender, RoutedEventArgs e)
        {
            e.Handled = true;
            Expander expander = (Expander)(e.OriginalSource);
            GroupToExpandedConverter.NotifyExpanded(expander);
        }

        void OnPropertyGridGroupCollapsed(object sender, RoutedEventArgs e)
        {
            e.Handled = true;
            Expander expander = (Expander)(e.OriginalSource);
            GroupToExpandedConverter.NotifyCollapsed(expander);
        }

        string TakeValidPartOfTypeFullName(string fullname)
        {
            if (String.IsNullOrEmpty(fullname))
                throw new ArgumentNullException("fullname");

            StringBuilder builder = new StringBuilder(fullname.Length);
            for (int i = 0; i < fullname.Length; i++)
            {
                if (!System.Text.RegularExpressions.Regex.IsMatch(fullname.Substring(0, i + 1), patternFullName))
                    break;

                builder.Append(fullname[i]);
            }

            return builder.Length > 0 ? builder.ToString() : fullname;
        }

        internal void RefreshGrid(bool bForce = false)
        {
            using (var wait = new WaitCursor())
            {
                if (bSetPropertyFilter)
                {
                    bSetPropertyFilter = false;
                    ICollectionView view = CollectionViewSource.GetDefaultView(propertyGrid.BindingView);
                    using (view.DeferRefresh())
                    {
                        if (PropertyFilterState == PropertyFilterEnum.GroupByCategory)
                        {
                            view.GroupDescriptions.Clear();
                            view.SortDescriptions.Clear();

                            view.GroupDescriptions.Add(nodeByCategory);
                            view.GroupDescriptions.Add(nodeByAdvanced);
                            view.SortDescriptions.Add(sortByCategoryPriority);
                            view.SortDescriptions.Add(sortByAdvPriority);
                            view.SortDescriptions.Add(sortByPriority);
                        }
                        else if (PropertyFilterState == PropertyFilterEnum.GroupByType)
                        {
                            view.GroupDescriptions.Clear();
                            view.SortDescriptions.Clear();

                            view.GroupDescriptions.Add(nodeByType);
                            view.SortDescriptions.Add(sortByTypeName);
                        }
                        else if (PropertyFilterState == PropertyFilterEnum.SortAlphabetically)
                        {
                            view.GroupDescriptions.Clear();
                            view.SortDescriptions.Clear();

                            view.SortDescriptions.Add(sortByPropertyName);
                        }
                        else if (PropertyFilterState == PropertyFilterEnum.SortByPriority)
                        {
                            view.GroupDescriptions.Clear();
                            view.SortDescriptions.Clear();

                            view.SortDescriptions.Add(sortByPriority);
                        }
                        else if (view.GroupDescriptions.Count > 0) // default sorting
                        {
                            view.SortDescriptions.Clear();
                            view.GroupDescriptions.Clear();
                        }
                    }
                }

                if (bForce)
                {
                    if (IsMultipleSelection)
                    {
                        foreach (var selectedObject in propertyGrid.SelectedObjects)
                        {
                            var aggregated = selectedObject as Aggregated;
                            if (aggregated != null)
                                aggregated.InvalidateTypeDescriptor();
                        }
                    }
                    else
                    {
                        var aggregated = propertyGrid.SelectedObject as Aggregated;
                        if (aggregated != null)
                            aggregated.InvalidateTypeDescriptor();
                    }
                    propertyGrid.Refresh();
                }
                OnPropertyChanged("IsPropertyGridVisible");
                OnPropertyChanged("IsMultipleSelection");

                CheckAndExpandSingleGroup();
            }
        }

        void CheckAndExpandSingleGroup()
        {
            Dispatcher.BeginInvokeAsynchronouslyInRender(() =>
            {
                if (GroupToExpandedConverter.bAtLeastOneGroupExpanded)
                {
                    GroupToExpandedConverter.bAtLeastOneGroupExpanded = false;
                    return;
                }

                DependencyObjectExtensions.CleanChildrenOfTypeCache(propertyGrid);
                var expanders = (from c in propertyGrid.GetVisualChildrenOfType<Expander>() select c).ToList();
                if (expanders.Count > 0 && !expanders[0].IsExpanded)
                    expanders[0].IsExpanded = true;
            });
        }

        void SetGridFilter(PropertyGridBindingView bv = null)
        {
            using (var wait = new WaitCursor())
            {
                if (bv == null)
                    bv = propertyGrid.BindingView;

                ICollectionView view = CollectionViewSource.GetDefaultView(bv);
                view.Filter = obj =>
                    {
                        var node = ((PropertyGridRow)obj).Node;

                        if (PropertyFilterState == PropertyFilterEnum.SortByPriority && !(obj is Node))
                        {
                            node.Priority = GetPropertyPriority(node.PropertyInfo.DeclaringType, node.PropertyInfo.Name);
                        }
                        else if (PropertyFilterState == PropertyFilterEnum.GroupByCategory)
                        {
                            var priority = priorityManager.GetPriority(node.Property.Category ?? Properties.Resources.MiscellaneousCategoryDisplayName);
                            node.CategoryPriority = priority == - 1 ? maxCategoryPriority : priority;
                            
                            if (IsAdvancedPropertyName(node.PropertyInfo.DeclaringType, node.PropertyInfo.Name))
                            {
                                if (node.CategoryPriority != -1 && node.CategoryPriority != maxCategoryPriority)
                                {
                                    priority = GetPropertyPriority(node.PropertyInfo.DeclaringType, node.PropertyInfo.Name);
                                    node.Priority = priority == 0 ? maxCategoryPriority : priority;
                                    node.AdvPropertyPriority = priority == 0 ? maxCategoryPriority : priority;
                                }
                                else
                                {
                                    node.CategoryPriority = maxAdvancedCategoryPriority;
                                    priority = GetPropertyPriority(node.PropertyInfo.DeclaringType, node.PropertyInfo.Name);
                                    node.Priority = priority == 0 ? maxCategoryPriority : priority;
                                }
                            }
                            else
                            {
                                priority = GetPropertyPriority(node.PropertyInfo.DeclaringType, node.PropertyInfo.Name);
                                node.Priority = priority == 0 ? maxCategoryPriority : priority;
                            }
                        }

                        return true;
                    };
            }
        }

        #endregion Property Visible State Manger

        #region INotifyPropertyChanged Members

        public event PropertyChangedEventHandler PropertyChanged;

        /// <summary>
        /// Raises this object's PropertyChanged event.
        /// </summary>
        /// <param name="propertyName">The property that has a new value.</param>
        protected virtual void OnPropertyChanged(string propertyName)
        {
            PropertyChangedEventHandler handler = PropertyChanged;
            if (handler != null)
            {
                var e = new PropertyChangedEventArgs(propertyName);
                handler(this, e);
            }
        }

        #endregion

        #region Commands

        private void PropertyControl_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.F1)
            {
                //show help...
                if (propertyGrid.SelectedGridItem != null && ComponentService.PropertyControlComponent.propertyControlComponent.HelpProvider != null)
                {
                    string prop = string.Format("{0}:{1}", title ?? propertyGrid.SelectedGridItem.Node.DeclaringType.FullName, propertyGrid.SelectedGridItem.Node.Name);
                    ComponentService.PropertyControlComponent.propertyControlComponent.HelpProvider.OpenDialogHelpPage(prop);
                }
                e.Handled = true;
            }
            else if (e.Key == Key.Escape)
            {
                var el = Keyboard.FocusedElement;
                if (el is System.Windows.Controls.TextBox)
                {
                    var textBox = el as System.Windows.Controls.TextBox;
                    if (textBox.IsEnabled && textBox.CanUndo)
                    {
                        textBox.Undo();
                        e.Handled = true;
                    }
                }
            }
        }

        #region Localization Helpers
        private void CreateResourceBtn_Click(object sender, RoutedEventArgs e)
        {
            e.Handled = true;
            if (propertyGrid.SelectedObject is Aggregated)
            {
                var aggregated = propertyGrid.SelectedObject as Aggregated;
                SaveLocalizationResourceFiles(aggregated);
            }

            foreach (var node in propertyGrid.Nodes)
            {
                if (node.Value is Aggregated)
                {
                    var aggregated = node.Value as Aggregated;
                    SaveLocalizationResourceFiles(aggregated);
                }
            }
        }

        private void LocalizePropertyBtn_Click(object sender, RoutedEventArgs e)
        {
            e.Handled = true;
            if (propertyGrid.SelectedGridItem == null || 
                propertyGrid.SelectedGridItem.Node == null || 
                propertyGrid.SelectedGridItem.Node.Property == null ||
                propertyGrid.SelectedGridItem.Node.Property.AsPropertyDescriptor == null)
                return;

            var property = new DisplayInvariantStrings(propertyGrid.SelectedGridItem.Node, ProjectTypeToHide);
            var content = new Localization.Helpers.InvariantStrings() { DataContext = property };

            if (IsAdvancedPropertyName(propertyGrid.SelectedGridItem.Node))
            {
                content.txtPropertyCategoryPriority.Text = "";
                content.txtPropertyCategoryPriority.IsEnabled = false;
            }

            content.txtProjectTypeToHide.Text = ProjectTypeToHide;

            if (!propertyGrid.SelectedGridItem.Node.Parent.PropertyType.IsPrimitive &&
                propertyGrid.SelectedGridItem.Node.Parent.PropertyType.IsValueType &&
                propertyGrid.SelectedGridItem.Node.PropertyType.IsPrimitive)
            {
                var tempResourceManager = new PropertyResourceManager(propertyGrid.SelectedGridItem.Node.Parent.Property.PropertyType.FullName);
                content.txtPropertyName.Text = tempResourceManager.GetString(property.DisplayName);
                content.txtPropertyHelp.Text = tempResourceManager.GetString(String.Format("{0}_Help", property.DisplayName));

                content.txtPropertyCategory.Text = content.txtPropertyPriority.Text = content.txtPropertyCategoryPriority.Text = content.txtProjectTypeToHide.Text = "";
                content.txtPropertyCategory.IsEnabled = content.txtPropertyPriority.IsEnabled = content.txtPropertyCategoryPriority.IsEnabled = content.txtProjectTypeToHide.IsEnabled = false;
            }

            GeneralDialogContent localizeDialog = new GeneralDialogContent(content)
            {
                Owner = this.FindParent<Window>(),
                HelpLink = "LocalizeProperty"
            };

            if (localizeDialog.ShowDialog() == true)
            {
                var propertyDescriptor = propertyGrid.SelectedGridItem.Node.Property.AsPropertyDescriptor;
                var prop = new LocalizablePropertyDescriptor(propertyDescriptor);
                prop.ChangeDisplayName(content.txtPropertyName.Text);
                prop.ChangeCategory(content.txtPropertyCategory.Text);
                prop.ChangeDescription(content.txtPropertyHelp.Text);
                if (propertyGrid.SelectedGridItem.Node.Source is Aggregated)
                {
                    if ((propertyGrid.SelectedGridItem.Node.Source as Aggregated).HasChildren)
                    {
                        foreach (Attribute attribute in propertyDescriptor.Attributes)
                        {
                            if (attribute is DisplayNameExtension)
                            {
                                prop.UpdateAttributeToFind(attribute.GetType());
                                break;
                            }
                        }
                    }
                }
                
                prop.UpdateLocalizationResourceFiles();
                LocalizablePropertyDescriptorCollection.CleanTypeResourceCache();

                int newCategoryPriority = 0;
                int oldCategoryPriority = priorityManager.GetPriority(propertyDescriptor.Category ?? Properties.Resources.MiscellaneousCategoryDisplayName);
                if (int.TryParse(content.txtPropertyCategoryPriority.Text, out newCategoryPriority) && 
                    oldCategoryPriority != newCategoryPriority &&
                    !IsAdvancedPropertyName(propertyGrid.SelectedGridItem.Node.PropertyInfo.DeclaringType, propertyGrid.SelectedGridItem.Node.PropertyInfo.Name))
                    priorityManager.SetPriority(content.txtPropertyCategory.Text, newCategoryPriority);

                var oldPriority = SortPriority;
                int newPriority = 0;
                if (!string.IsNullOrWhiteSpace(content.txtPropertyPriority.Text) && 
                    int.TryParse(content.txtPropertyPriority.Text, out newPriority))
                    SortPriority = newPriority;

                var aggregated = propertyGrid.SelectedGridItem.Node.Source as Aggregated;
                if (aggregated != null)
                    aggregated.InvalidateTypeDescriptor();

                ProjectTypeToHide = content.txtProjectTypeToHide.Text;

                if (propertyGrid.SelectedObject != null)
                {
                    propertyGrid.BeginInit();
                    var value = propertyGrid.SelectedObject;
                    propertyGrid.SelectedObject = null;
                    propertyGrid.SelectedObject = value;
                    propertyGrid.EndInit();
                }
                else if (propertyGrid.SelectedObjects != null)
                {
                    propertyGrid.BeginInit();
                    var value = propertyGrid.SelectedObjects;
                    propertyGrid.SelectedObjects = null;
                    propertyGrid.SelectedObjects = value;
                    propertyGrid.EndInit();
                }
                else
                    RefreshGrid(true);
            }
        }

        void SaveLocalizationResourceFiles(Aggregated aggregated)
        {
            var typeDrscriptor = aggregated.CreateDuplicatedInstanceAndEnableDependencyProperties() as ICustomTypeDescriptor;
            var attributes = new Attribute[] { new BrowsableAttribute(true) };
            var properties = typeDrscriptor.GetProperties(attributes);
            var collection = new LocalizablePropertyDescriptorCollection();
            foreach (PropertyDescriptor prop in properties)
                AddPropertyDescriptorToCollection(prop, ref collection);
            collection.CreateLocalizationResourceFiles();
            LocalizablePropertyDescriptorCollection.CleanTypeResourceCache();
        }

        void AddPropertyDescriptorToCollection(PropertyDescriptor prop, ref LocalizablePropertyDescriptorCollection collection)
        {
            if (!prop.IsBrowsable ||
                !IsPropertyVisible(prop.ComponentType, prop.Name) ||
                (prop.Name.Contains('.') && !IsValidAttachedProperty(prop.Name)))
                return;

            if (prop is LocalizablePropertyDescriptor)
                collection.Insert((prop as LocalizablePropertyDescriptor).BasePropertyDescriptor);
            else
                collection.Insert(prop);
        }        
        #endregion

        bool bExpanded = Properties.Settings.Default.ShowPropertyGroupsExpanded;
        private void btnExpandCollapseAll_Click(object sender, RoutedEventArgs e)
        {
            e.Handled = true;
            if (bExpanded)
                propertyGrid.CollapseGroups();
            else
                propertyGrid.ExpandGroups();
            bExpanded = !bExpanded;
        }

        #endregion

        #region IDisposable
        bool bDisposed;
        public void Dispose()
        {
            if (bDisposed)
                return;
            bDisposed = true;

            ComponentService.PropertyControlComponent.propertyEditors.CollectionChanged -= propertyEditors_CollectionChanged;
            DependencyObjectExtensions.CleanChildrenOfTypeCache(propertyGrid);

            if (propertyGrid.SelectedObject is Aggregated)
                (propertyGrid.SelectedObject as Aggregated).Dispose();

            var selectedObjects = propertyGrid.SelectedObjects as List<Aggregated>;
            if (selectedObjects != null)
            {
                foreach (var aggregated in selectedObjects)
                    aggregated.Dispose();
            }
        }
        #endregion
    }
}
