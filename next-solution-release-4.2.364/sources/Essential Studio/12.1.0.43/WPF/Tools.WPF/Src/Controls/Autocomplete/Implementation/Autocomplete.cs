// <copyright file="Autocomplete.cs" company="Syncfusion">
// Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Any infringement will be prosecuted under
// applicable laws.
// </copyright>

using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.IO.IsolatedStorage;
using System.Linq;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Threading;
using Syncfusion.Licensing;
using Syncfusion.Windows.Shared;

namespace Syncfusion.Windows.Tools.Controls
{
    /// <summary>
    /// Represents the AutoComplet UI <para>AutoComplete control provides live drop-down
    /// hints to users as they type in the keywords. It guides the user by displaying
    /// the list of the text which was previously stored or used. The user can select
    /// from the list of the text instead of entering the whole text again.</para>
    /// </summary>
    /// <example>
    /// <code lang="XAML">
    /// <Window x:Class="AutoComplete.Window1" xmlns="http://schemas.microsoft.com/winfx/2006/xaml/presentation" xmlns:x="http://schemas.microsoft.com/winfx/2006/xaml" Title="Window1" Height="300" Width="300" xmlns:syncfusion="http://schemas.syncfusion.com/wpf">
    /// <Grid>
    /// <syncfusion:AutoComplete Width="200" Height="23" Name="autoComplete"/>
    /// </Grid>
    /// <para/>
    /// </Window>
    /// <para/>
    /// </code>
    /// <code lang="C#">
    /// using System;
    /// using System.Collections.Generic;
    /// using System.Linq;
    /// using System.Text;
    /// using System.Windows;
    /// using System.Windows.Controls;
    /// using System.Windows.Data;
    /// using System.Windows.Documents;
    /// using System.Windows.Input;
    /// using System.Windows.Media;
    /// using System.Windows.Media.Imaging;
    /// using System.Windows.Navigation;
    /// using System.Windows.Shapes;
    /// using Syncfusion.Windows.Tools.Controls;
    /// namespace AutoCompleteSample
    /// {
    /// /// <summary>
    /// /// Interaction logic for Window1.xaml
    /// /// </summary>
    /// public partial class Window1 : Window
    /// {
    /// public Window1()
    /// {
    /// InitializeComponent();
    /// AutoComplete autoComplete = new AutoComplete();
    /// autoComplete.Width = 200;
    /// autoComplete.Height = 23;
    /// this.Content = autoComplete;
    /// }
    /// }
    /// }
    /// <para/>
    /// </code>
    /// </example>
    ///
    [SkinType(SkinVisualStyle = Skin.Office2007Blue,
    Type = typeof(AutoComplete), XamlResource = "/Syncfusion.Tools.WPF;component/Controls/AutoComplete/Themes/Office2007BlueStyle.xaml")]
    [SkinType(SkinVisualStyle = Skin.Office2007Black,
    Type = typeof(AutoComplete), XamlResource = "/Syncfusion.Tools.WPF;component/Controls/AutoComplete/Themes/Office2007BlackStyle.xaml")]
    [SkinType(SkinVisualStyle = Skin.Office2007Silver,
    Type = typeof(AutoComplete), XamlResource = "/Syncfusion.Tools.WPF;component/Controls/AutoComplete/Themes/Office2007SilverStyle.xaml")]
    [SkinType(SkinVisualStyle = Skin.Office2010Blue,
    Type = typeof(AutoComplete), XamlResource = "/Syncfusion.Tools.WPF;component/Controls/AutoComplete/Themes/Office2010BlueStyle.xaml")]
    [SkinType(SkinVisualStyle = Skin.Office2010Black,
    Type = typeof(AutoComplete), XamlResource = "/Syncfusion.Tools.WPF;component/Controls/AutoComplete/Themes/Office2010BlackStyle.xaml")]
    [SkinType(SkinVisualStyle = Skin.Office2010Silver,
    Type = typeof(AutoComplete), XamlResource = "/Syncfusion.Tools.WPF;component/Controls/AutoComplete/Themes/Office2010SilverStyle.xaml")]
    [SkinType(SkinVisualStyle = Skin.Office2003,
    Type = typeof(AutoComplete), XamlResource = "/Syncfusion.Tools.WPF;component/Controls/AutoComplete/Themes/Office2003Style.xaml")]
    [SkinType(SkinVisualStyle = Skin.Blend,
    Type = typeof(AutoComplete), XamlResource = "/Syncfusion.Tools.WPF;component/Controls/AutoComplete/Themes/BlendStyle.xaml")]
    [SkinType(SkinVisualStyle = Skin.SyncOrange,
    Type = typeof(AutoComplete), XamlResource = "/Syncfusion.Tools.WPF;component/Controls/AutoComplete/Themes/SyncOrangeStyle.xaml")]
    [SkinType(SkinVisualStyle = Skin.ShinyRed,
    Type = typeof(AutoComplete), XamlResource = "/Syncfusion.Tools.WPF;component/Controls/AutoComplete/Themes/ShinyRedStyle.xaml")]
    [SkinType(SkinVisualStyle = Skin.ShinyBlue,
    Type = typeof(AutoComplete), XamlResource = "/Syncfusion.Tools.WPF;component/Controls/AutoComplete/Themes/ShinyBlueStyle.xaml")]
    [SkinType(SkinVisualStyle = Skin.Default,
    Type = typeof(AutoComplete), XamlResource = "/Syncfusion.Tools.WPF;component/Controls/AutoComplete/Themes/Generic.xaml")]
    [SkinType(SkinVisualStyle = Skin.VS2010,
  Type = typeof(AutoComplete), XamlResource = "/Syncfusion.Tools.WPF;component/Controls/AutoComplete/Themes/VS2010Style.xaml")]
    [SkinType(SkinVisualStyle = Skin.Metro,
 Type = typeof(AutoComplete), XamlResource = "/Syncfusion.Tools.WPF;component/Controls/AutoComplete/Themes/MetroStyle.xaml")]
    [SkinType(SkinVisualStyle = Skin.Transparent,
Type = typeof(AutoComplete), XamlResource = "/Syncfusion.Tools.WPF;component/Controls/AutoComplete/Themes/TransparentStyle.xaml")]
#if SyncfusionFramework4_0
    [System.ComponentModel.DesignTimeVisible(true)]
#endif

    public class AutoComplete : ItemsControl
    {
        #region Constants

        /// <property name="flag" value="Finished" />
        /// <summary>
        /// Sets string for search in template element.
        /// </summary>
        private const string PART_EditableTextBox = "PART_EditableTextBox";

        /// <property name="flag" value="Finished" />
        /// <summary>
        /// Sets string for search in template element.
        /// </summary>
        private const string PART_Container = "PART_Container";

        /// <property name="flag" value="Finished" />
        /// <summary>
        /// Sets string for search in template element.
        /// </summary>
        private const string PART_HistoryContainer = "PART_HistoryContainer";

        /// <property name="flag" value="Finished" />
        /// <summary>
        /// Sets string for searching in template element.
        /// </summary>
        private const string PART_CheckButton = "PART_CheckButton";

        /// <property name="flag" value="Finished" />
        /// <summary>
        /// Sets string for search in template element.
        /// </summary>
        private const string PART_VistaProgressBar = "PART_VistaProgressBar";

        /// <summary>
        /// set the string for the File Extension
        /// </summary>
        private const string FileExt = ".dat";

        /// <summary>
        /// set the maximum file name length
        /// </summary>
        private const int MaxFileNameLenght = 248;

        #endregion Constants

        #region Private members

        private bool IsInternalchange = true;

        /// <summary>
        /// Checks whether the History flag is loaded
        /// </summary>
        private bool LoadHistoryFlag = false;

        /// <summary>
        /// Chekcs whether it is a key back
        /// </summary>
        private bool m_keyback = false;

        private bool HasDuplicateItem = false;

        private bool m_TextChanged = false;

        /// <summary>
        /// Initialize the key bracket to 0
        /// </summary>
        private int keybackcaret = 0;

        /// <summary>
        /// Chekcs whether control or shift key is pressed
        /// </summary>
        private bool shift = false;

        /// <summary>
        /// Chekcs whether the item is added to the object
        /// </summary>
        private bool m_addtoselectedobj = false;

        /// <summary>
        /// Chekcs whether the key is up
        /// </summary>
        private bool m_keyupdown = false;

        /// <property name="flag" value="Finished" />
        /// <summary>
        /// This member contains history list.
        /// </summary>
        private readonly ObservableCollection<String> m_CustomHistoryList = new ObservableCollection<String>();

        private readonly ObservableCollection<String> m_FilteredCustomHistoryList = new ObservableCollection<String>();

        private readonly ObservableCollection<String> m_HistoryList = new ObservableCollection<String>();

        /// <summary>
        /// Contains the temproary selected item
        /// </summary>
        private System.Collections.IList m_tempSelectedItems = new List<object>();

        /// <property name="flag" value="Finished" />
        /// <summary>
        /// This member contains observer collection.
        /// </summary>
        private readonly ObservableCollection<String> m_AutoCompleteList = new ObservableCollection<String>();

        private readonly ObservableCollection<object> m_AutoCompleteObjectList = new ObservableCollection<object>();

        /// <summary>
        /// contains the temproary complete list
        /// </summary>
        private readonly ObservableCollection<String> m_tempAutoCompleteList = new ObservableCollection<String>();

        private readonly List<object> m_tempHistoryList = new List<object>();

        /// <property name="flag" value="Finished" />
        /// <summary>
        /// Presents file name for saving in internal isolated store.
        /// </summary>
        private string m_storeFileName = AppDomain.CurrentDomain.SetupInformation.ApplicationName + FileExt;

        /// <summary>
        /// contains the object file name
        /// </summary>
        private string m_storeObjectFileName = AppDomain.CurrentDomain.SetupInformation.ApplicationName + ".obj";

        /// <property name="flag" value="Finished" />
        /// <summary>
        /// This member contains a value indicating whether there is
        /// total clear items view.
        /// </summary>
        private bool m_isTotalClear = false;

        /// <property name="flag" value="Finished" />
        /// <summary>
        /// This member contains a value indicating whether the text
        /// change is internal (doesn't need to use auto-complete).
        /// </summary>
        private bool m_isInternalChange = false;

        /// <property name="flag" value="Finished" />
        /// <summary>
        /// This member contains the current tree level root.
        /// </summary>
        private IAutocompleteRoot m_root = null;

        /// <property name="flag" value="Finished" />
        /// <summary>
        /// This member contains auto-appends collection view.
        /// </summary>
        private ICollectionView m_iCollectionAutoComplete = null;

        /// <property name="flag" value="Finished" />
        /// <summary>
        /// This member contains history collection view.
        /// </summary>
        private ICollectionView m_iCollectionHistory = null;

        /// <property name="flag" value="Finished" />
        /// <summary>
        /// This member contains hash level.
        /// </summary>
        private IAutocompleteLevel m_hashLevel = null;

        /// <property name="flag" value="Finished" />
        /// <summary>
        /// This member contains TextBox for typing.
        /// </summary>
        private TextBox m_textBox = null;

        /// <property name="flag" value="Finished" />
        /// <summary>
        /// This member contains auto-completes ItemsControl for view.
        /// </summary>
        private ListBox m_itemsAutoComplete = null;

        /// <property name="flag" value="Finished" />
        /// <summary>
        /// This member contains history ItemsControl for view.
        /// </summary>
        private ListBox m_itemsHistory = null;

        /// <property name="flag" value="Finished" />
        /// <summary>
        /// This member contains control for view Vista-like progress
        /// bar.
        /// </summary>
        private VistaSpecificProgressBar m_vistaProgressBar = null;

        /// <summary>
        /// Member contains the selected item list
        /// </summary>
        private System.Collections.IList m_selectedItems = new List<object>();

        /// <summary>
        /// Member contains the selected item
        /// </summary>
        private object m_selectedItem = null;

        /// <summary>
        /// Member containslsit of custom source flag
        /// </summary>
        private List<object> CustomSourceString;

        /// <summary>
        /// Member contains list of object history
        /// </summary>
        private List<KeyValuePair<String, Object>> ObjectHistory;

        /// <summary>
        /// Member contains list of business object
        /// </summary>
        private List<KeyValuePair<string, object>> BusinessObject = new List<KeyValuePair<string, object>>();

        /// <summary>
        /// Checks whether it is auto append
        /// </summary>
        private bool m_autoappend = false;

        /// <summary>
        /// Checks the drop down flag
        /// </summary>
        private bool m_dropdownflag = true;

        /// <summary>
        /// Checks IsCustomToggleProperty
        /// </summary>
        private static bool isCustomToggleCheck = false;

        /// <summary>
        /// Checks IsIndex Property set in Design
        /// </summary>
        private bool isIndexChangedInDesign = true;

        /// <summary>
        /// This member contains the thumb to resize History popup
        /// </summary>
        private Thumb HistoryThumb;

        /// <summary>
        /// contains the temproary textbox caret index
        /// </summary>
        public static int index;

        /// <summary>
        /// This member contains the thumb to resize Normal popup
        /// </summary>
        private Thumb ContainerThumb;

        internal AutocompleteItemCollection tempfilterCollection;

        private bool IsBackKeyPressed = false;

        private bool canSelectHistoryItem = true;

        #endregion Private members

        #region Initialization

        /// <summary>
        /// Initializes static members of the <see cref="AutoComplete"/> class.
        /// </summary>
        static AutoComplete()
        {
            AutoComplete.DefaultStyleKeyProperty.OverrideMetadata(typeof(AutoComplete), new FrameworkPropertyMetadata(typeof(AutoComplete)));
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="AutoComplete"/> class.
        /// </summary>
        public AutoComplete()
        {
            if (EnvironmentTestTools.IsSecurityGranted)
            {
                EnvironmentTestTools.StartValidateLicense(typeof(AutoComplete));
            }
            Dispatcher.BeginInvoke(DispatcherPriority.Loaded, new DispatcherHandler(LoadDispatherLoaded));
            if (!DesignerProperties.GetIsInDesignMode(this))
            {
                if (Application.Current != null && !BindingUtils.GetEnableBindingErrors(Application.Current.MainWindow))
                    System.Diagnostics.PresentationTraceSources.DataBindingSource.Switch.Level = System.Diagnostics.SourceLevels.Critical;
            }
            this.Loaded += new RoutedEventHandler(AutoComplete_Loaded);
            this.Unloaded += new RoutedEventHandler(AutoComplete_Unloaded);
        }

        private bool Check_DuplicateKey()
        {
            if (SourceMode.Custom == Source && !DisplayMemberPath.Equals("") && this.CustomSource != null)
            {
                for (int i = 0; i < BusinessObject.Count; i++)
                {
                    if (i < BusinessObject.Count - 1)
                    {
                        if (BusinessObject[i].Key == BusinessObject[i + 1].Key)
                        {
                            return true;
                        }
                    }
                }
            }
            return false;
        }

        private bool isDuplicate = false;

        private void AutoComplete_Loaded(object sender, RoutedEventArgs e)
        {
            OnSelected(this, SelectedItem);
            if (Check_DuplicateKey())
            {
                m_AutoCompleteObjectList.Clear();
                for (int m = 0; m < BusinessObject.Count; m++)
                    m_AutoCompleteObjectList.Add(BusinessObject[m].Value);
                isDuplicate = true;
                if (m_itemsAutoComplete != null)
                {
                    m_itemsAutoComplete.ItemsSource = m_AutoCompleteObjectList;
                    m_itemsAutoComplete.DisplayMemberPath = this.DisplayMemberPath;
                }
            }
        }

        private void AutoComplete_Unloaded(object sender, RoutedEventArgs e)
        {
            m_tempAutoCompleteList.Clear();
        }

        #endregion Initialization

        #region Event

        /// <property name="flag" value="Finished" />
        /// <summary>
        /// Event that is raised when Source property is changed.
        /// </summary>
        public event PropertyChangedCallback SourceChanged;

        /// <summary>
        /// Occurs when [selected value changed].
        /// </summary>
        public event PropertyChangedCallback SelectedValueChanged;

        /// <property name="flag" value="Finished" />
        /// <summary>
        /// Event that is raised when IsDropDownOpen property is changed.
        /// </summary>
        public event PropertyChangedCallback IsDropDownOpenChanged;

        /// <property name="flag" value="Finished" />
        /// <summary>
        /// Event that is raised when CustomSource property is changed.
        /// </summary>
        public event PropertyChangedCallback CustomSourceChanged;

        /// <property name="flag" value="Finished" />
        /// <summary>
        /// Event that is raised when IsAutoAppend property is changed.
        /// </summary>
        public event PropertyChangedCallback IsAutoAppendChanged;

        /// <property name="flag" value="Finished" />
        /// <summary>
        /// Event that is raised when MaxDropHeight property is changed.
        /// </summary>
        public event PropertyChangedCallback MaxDropHeightChanged;

        /// <property name="flag" value="Finished" />
        /// <summary>
        /// Event that is raised when IsFilter property is changed.
        /// </summary>
        public event PropertyChangedCallback IsFilterChanged;

        /// <property name="flag" value="Finished" />
        /// <summary>
        /// Event that is raised when IsAutoCompleteItem property is changed.
        /// </summary>
        public event PropertyChangedCallback IsAutoCompleteItemChanged;

        /// <property name="flag" value="Finished" />
        /// <summary>
        /// Event that is raised when IsAsyncAddContent property is changed.
        /// </summary>
        public event PropertyChangedCallback IsAsyncAddContentChanged;

        /// <property name="flag" value="Finished" />
        /// <summary>
        /// Event that is raised when Text property is changed.
        /// </summary>
        public event PropertyChangedCallback TextChanged;

        /// <property name="flag" value="Finished" />
        /// <summary>
        /// Event that is raised when IsHistory property is changed.
        /// </summary>
        public event PropertyChangedCallback IsHistoryChanged;

        /// <property name="flag" value="Finished" />
        /// <summary>
        /// Event that is raised when IsHistoryDropDownOpen property is changed.
        /// </summary>
        public event PropertyChangedCallback IsHistoryDropDownOpenChanged;

        /// <property name="flag" value="Finished" />
        /// <summary>
        /// Event that is raised when m_HistoryListHeight property is changed.
        /// </summary>
        public event PropertyChangedCallback HistoryListHeightChanged;

        /// <property name="flag" value="Finished" />
        /// <summary>
        /// Event that is raised when FileStorageName property is
        /// changed.
        /// </summary>
        public event PropertyChangedCallback FileStorageNameChanged;

        /// <summary>
        /// Occurs when [selection mode changed].
        /// </summary>
        public event PropertyChangedCallback SelectionModeChanged;

        /// <summary>
        /// Occurs when [selection changed].
        /// </summary>
        public event SelectionChangedEventHandler SelectionChanged;

        #endregion Event

        #region Properties

        public bool CanResizePopup
        {
            get { return (bool)GetValue(CanResizePopupProperty); }
            set { SetValue(CanResizePopupProperty, value); }
        }

        public static readonly DependencyProperty CanResizePopupProperty =
            DependencyProperty.Register("CanResizePopup", typeof(bool), typeof(AutoComplete), new PropertyMetadata(true, OnCanResizePopupChanged));

        public static void OnCanResizePopupChanged(DependencyObject obj, DependencyPropertyChangedEventArgs args)
        {
            AutoComplete ac = (AutoComplete)obj;
            if (ac.CanResizePopup == true)
            {
                if (ac.HistoryThumb != null)
                    ac.HistoryThumb.Visibility = Visibility.Visible;

                if (ac.ContainerThumb != null)
                    ac.ContainerThumb.Visibility = Visibility.Visible;
            }
            else
            {
                if (ac.HistoryThumb != null)
                    ac.HistoryThumb.Visibility = Visibility.Collapsed;

                if (ac.ContainerThumb != null)
                    ac.ContainerThumb.Visibility = Visibility.Collapsed;
            }
        }

        public bool EnableDropDown
        {
            get { return (bool)GetValue(EnableDropDownProperty); }
            set { SetValue(EnableDropDownProperty, value); }
        }

        // Using a DependencyProperty as the backing store for EnableDropDown.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty EnableDropDownProperty =
            DependencyProperty.Register("EnableDropDown", typeof(bool), typeof(AutoComplete), new FrameworkPropertyMetadata(true, OnEnablDropDownChanged));

        public bool EnableSorting
        {
            get { return (bool)GetValue(EnableSortingProperty); }
            set { SetValue(EnableSortingProperty, value); }
        }

        // Using a DependencyProperty for enabling and disabling sorting with Custom source
        public static readonly DependencyProperty EnableSortingProperty =
            DependencyProperty.Register("EnableSorting", typeof(bool), typeof(AutoComplete), new FrameworkPropertyMetadata(true));

        private static void OnEnablDropDownChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            AutoComplete instance = (AutoComplete)d;
            if (!instance.EnableDropDown)
            {
                instance.IsDropDownOpen = false;
                instance.IsHistoryDropDownOpen = false;
            }
        }

        /// <summary>
        /// Gets or sets the index of the selected.
        /// </summary>
        /// <value>The index of the selected.</value>
        public int SelectedIndex
        {
            get { return (int)GetValue(SelectedIndexProperty); }
            set { SetValue(SelectedIndexProperty, value); }
        }

        /// <summary>
        /// TO set the default value of the Selected Index property & property change call back
        /// </summary>
        public static readonly DependencyProperty SelectedIndexProperty =
            DependencyProperty.Register("SelectedIndex", typeof(int), typeof(AutoComplete), new PropertyMetadata(-1, OnSelectedIndexChanged));

        public static void OnSelectedIndexChanged(DependencyObject obj, DependencyPropertyChangedEventArgs args)
        {
            AutoComplete ac = (AutoComplete)obj;
            if (ac != null && ac.Items.Count > 0 && ac.isIndexChangedInDesign)
            {
                ac.SelectedItem = ac.Items[(int)args.NewValue];
                if ((int)args.NewValue >= 0 && ac.BusinessObject.Count > 0)
                    ac.Text = ac.BusinessObject[(int)args.NewValue].Key;
            }
            else
            {
                if (ac != null && ac.Items.Count > 0 && !(ac.isIndexChangedInDesign))
                {
                    if ((int)args.NewValue >= 0)
                        ac.SelectedItem = ac.Items[(int)args.NewValue];
                }
            }
        }

        /// <summary>
        /// Gets or sets the selected value.
        /// </summary>
        /// <value>The selected value.</value>
        public object SelectedValue
        {
            get { return (object)GetValue(SelectedValueProperty); }
            set { SetValue(SelectedValueProperty, value); }
        }

        /// <summary>
        ///
        /// </summary>
        public static readonly DependencyProperty SelectedValueProperty =
            DependencyProperty.Register("SelectedValue", typeof(object), typeof(AutoComplete), new PropertyMetadata(null, OnSelectedValueChanged));

        /// <summary>
        /// Gets or sets the selected value path.
        /// </summary>
        /// <value>The selected value path.</value>
        public string SelectedValuePath
        {
            get { return (string)GetValue(SelectedValuePathProperty); }
            set { SetValue(SelectedValuePathProperty, value); }
        }

        /// <summary>
        /// to the Default value of selected value property & to property changed call back
        /// </summary>
        public static readonly DependencyProperty SelectedValuePathProperty =
            DependencyProperty.Register("SelectedValuePath ", typeof(string), typeof(AutoComplete), new PropertyMetadata(string.Empty));

        public static void OnSelectedValueChanged(DependencyObject obj, DependencyPropertyChangedEventArgs args)
        {
            AutoComplete instance = (AutoComplete)obj;
            if (instance != null)
            {
                if (!instance.SelectedValuePath.Equals(""))
                {
                    if (!instance.isinternal)
                    {
                        int iCount = 0;
                        foreach (object i in instance.Items)
                        {
                            Type t = i.GetType();
                            var prop = t.GetProperty(instance.SelectedValuePath);
                            object s = (prop != null) ? prop.GetValue(i, null) : null;

                            if (prop != null)
                            {
                                object pathstring = prop.GetValue(i, null);
                                if (pathstring != null && instance.SelectedValue != null && instance.SelectedValue.ToString() == pathstring.ToString())
                                {
                                    if (instance.Source == SourceMode.Custom)
                                    {
                                        if(instance.SelectedItem == null)
                                            instance.SelectedItem = instance.Items[iCount];
                                        else if (instance.SelectedItem != null && instance.SelectedItem == instance.Items[iCount])
                                            instance.SelectedItem = instance.Items[iCount];
                                    }
                                }
                            }
                            iCount++;
                        }
                    }
                }

                instance.OnSelectedValueChanged(args);
            }
        }

        protected void OnSelectedValueChanged(DependencyPropertyChangedEventArgs args)
        {
            if (this.SelectedValueChanged != null)
            {
                this.SelectedValueChanged(this, args);
            }
        }

        /// <summary>
        /// this property created to load the default items
        /// </summary>
        public static readonly DependencyProperty IsLoadCustomOnToggleProperty =
     DependencyProperty.Register("IsLoadCustomSourceOnToggle", typeof(bool),
     typeof(AutoComplete), new FrameworkPropertyMetadata(false, new PropertyChangedCallback(IsLoadCustomOnToggleChanged)));

        // .NET Property wrapper
        public bool IsLoadCustomSourceOnToggle
        {
            get { return (bool)GetValue(IsLoadCustomOnToggleProperty); }
            set { SetValue(IsLoadCustomOnToggleProperty, value); }
        }

        private static void IsLoadCustomOnToggleChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            AutoComplete instance = (AutoComplete)d;

            instance.IsLoadCustomOnToggleChanged(e);
        }

        private void IsLoadCustomOnToggleChanged(DependencyPropertyChangedEventArgs e)
        {
            //throw new NotImplementedException();
        }

        /// <summary>
        /// Gets or sets the value of the PopUpPlacement dependency property.
        /// </summary>
        public PopupPlacement PopupPlacement
        {
            get { return (PopupPlacement)GetValue(PopupPlacementProperty); }
            set { SetValue(PopupPlacementProperty, value); }
        }

        /// <summary>
        /// This is used to specify the string serach mode
        /// </summary>
        public StringMode StringMode
        {
            get { return (StringMode)GetValue(StringModeProperty); }
            set { SetValue(StringModeProperty, value); }
        }

        ///<summary>
        /// this is used to specify the index position to search
        /// </summary>
        public int StringModeIndex
        {
            get { return (int)GetValue(StringModeIndexProperty); }
            set { SetValue(StringModeIndexProperty, value); }
        }

        /// <property name="flag" value="Finished" />
        ///  <para/>
        /// <summary>
        /// Gets or sets the value of the Source dependency property.
        /// </summary>
        public SourceMode Source
        {
            get { return (SourceMode)GetValue(SourceProperty); }
            set { SetValue(SourceProperty, value); }
        }

        /// <summary>
        /// Gets or sets a value indicating whether this instance is drop down open.
        /// </summary>
        public bool IsDropDownOpen
        {
            get
            {
                return (Boolean)GetValue(IsDropDownOpenProperty);
            }

            set
            {
                if (value != IsDropDownOpen
                    && (!value || (0 < m_AutoCompleteList.Count || 0 < m_AutoCompleteObjectList.Count) && m_textBox.IsFocused))
                {
                    SetValue(IsDropDownOpenProperty, value);
                }
            }
        }

        /// <summary>
        /// Invoked when the <see cref="P:System.Windows.Controls.ItemsControl.Items"/> property changes.
        /// </summary>
        /// <param name="e">Information about the change.</param>
        protected override void OnItemsChanged(NotifyCollectionChangedEventArgs e)
        {
            base.OnItemsChanged(e);
            if (SourceMode.Custom == Source)
            {
                Root = null;
                ItemsSource = CustomSource;
                MapCustomObject();
            }
        }

        /// <summary>
        /// Invoked when the <see cref="P:System.Windows.Controls.ItemsControl.DisplayMemberPath"/> property changes.
        /// </summary>
        /// <param name="oldDisplayMemberPath">The old value of the <see cref="P:System.Windows.Controls.ItemsControl.DisplayMemberPath"/> property.</param>
        /// <param name="newDisplayMemberPath">New value of the <see cref="P:System.Windows.Controls.ItemsControl.DisplayMemberPath"/> property.</param>
        protected override void OnDisplayMemberPathChanged(string oldDisplayMemberPath, string newDisplayMemberPath)
        {
            base.OnDisplayMemberPathChanged(oldDisplayMemberPath, newDisplayMemberPath);
            MapCustomObject();
        }

        /// <summary>
        /// Gets the selected items.
        /// </summary>
        /// <value>The selected items.</value>
        public System.Collections.IList SelectedItems
        {
            get
            {
                return m_selectedItems;
            }
        }

        /// <summary>
        /// Gets or sets the selected item.
        /// </summary>
        /// <value>The selected item.</value>
        public object SelectedItem
        {
            get { return (object)GetValue(SelectedItemProperty); }
            set { SetValue(SelectedItemProperty, value); }
        }

        /// <property name="flag" value="Finished" />
        /// <para/>
        /// <summary>
        /// Gets or sets the value of the CustomSource dependency property.
        /// </summary>
        public System.Collections.IEnumerable CustomSource
        {
            get
            {
                return (System.Collections.IEnumerable)GetValue(CustomSourceProperty);
            }
            set
            {
                SetValue(CustomSourceProperty, value);
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether this instance is auto append.
        /// </summary>
        /// <value>
        /// <c>true</c> if this instance is auto append; otherwise, <c>false</c>.
        /// </value>
        public bool IsAutoAppend
        {
            get { return (Boolean)GetValue(IsAutoAppendProperty); }
            set { SetValue(IsAutoAppendProperty, value); }
        }

        /// <property name="flag" value="Finished" />
        /// <summary>
        /// Gets or sets the value of the MaxDropHeight dependency property.
        /// </summary>
        public double MaxDropHeight
        {
            get { return (Double)GetValue(MaxDropHeightProperty); }
            set { SetValue(MaxDropHeightProperty, value); }
        }

        /// <property name="flag" value="Finished" />
        /// <summary>
        /// Gets or sets the value of the HistoryListHeight dependency property.
        /// </summary>
        public double HistoryListHeight
        {
            get { return (Double)GetValue(HistoryListHeightProperty); }
            set { SetValue(HistoryListHeightProperty, value); }
        }

        /// <summary>
        /// Gets or sets a value indicating whether this instance is filter.
        /// </summary>
        /// <value><c>true</c> if this instance is filter; otherwise, <c>false</c>.</value>
        public bool IsFilter
        {
            get { return (Boolean)GetValue(IsFilterProperty); }
            set { SetValue(IsFilterProperty, value); }
        }

        /// <summary>
        /// Gets or sets a value indicating whether this instance is auto complete item.
        /// </summary>
        /// <value>
        /// <c>true</c> if this instance is auto complete item; otherwise, <c>false</c>.
        /// </value>
        public bool IsAutoCompleteItem
        {
            get { return (Boolean)GetValue(IsAutoCompleteItemProperty); }
            set { SetValue(IsAutoCompleteItemProperty, value); }
        }

        /// <summary>
        /// Gets or sets a value indicating whether this instance is async add content.
        /// </summary>
        public bool IsAsyncAddContent
        {
            get { return (Boolean)GetValue(IsAsyncAddContentProperty); }
            set { SetValue(IsAsyncAddContentProperty, value); }
        }

        /// <property name="flag" value="Finished" />
        /// <summary>
        /// Gets or sets the value of the Text dependency property.
        /// </summary>
        public string Text
        {
            get { return (String)GetValue(TextProperty); }
            set { SetValue(TextProperty, value); }
        }

        /// <summary>
        /// Gets or sets the selection mode.
        /// </summary>
        /// <value>The selection mode.</value>
        public SelectionMode SelectionMode
        {
            get { return (SelectionMode)GetValue(SelectionModeProperty); }
            set { SetValue(SelectionModeProperty, value); }
        }

        /// <summary>
        /// Gets or sets the separator char.
        /// </summary>
        /// <value>The separator char.</value>
        public char SeparatorChar
        {
            get { return (char)GetValue(SeparatorCharProperty); }
            set { SetValue(SeparatorCharProperty, value); }
        }

        /// <summary>
        /// Gets or sets a value indicating whether this instance is history.
        /// </summary>
        /// <value>
        /// <c>true</c> if this instance is history; otherwise, <c>false</c>.
        /// </value>
        public bool IsHistory
        {
            get { return (Boolean)GetValue(IsHistoryProperty); }
            set { SetValue(IsHistoryProperty, value); }
        }

        /// <summary>
        /// Gets or sets a value indicating whether this instance is history drop down open.
        /// </summary>
        /// <value>
        /// <c>true</c> if this instance is history drop down open; otherwise, <c>false</c>.
        /// </value>
        public bool IsHistoryDropDownOpen
        {
            get
            {
                return (Boolean)GetValue(IsHistoryDropDownOpenProperty);
            }
            set
            {
                if (value != IsHistoryDropDownOpen)
                {
                    SetValue(IsHistoryDropDownOpenProperty, value);
                }
            }
        }

        /// <property name="flag" value="Finished" />
        /// <para/>
        /// <summary>
        /// Gets or sets the value of the m_HistoryListHeight dependency property.
        /// </summary>
        [Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
        public double M_HistoryListHeight
        {
            get { return (Double)GetValue(HistoryListHeightProperty); }
            set { SetValue(HistoryListHeightProperty, value); }
        }

        /// <property name="flag" value="Finished" />
        /// <para/>
        /// <summary>
        /// Gets or sets the value of the FileStorageName dependency
        /// property.
        /// </summary>
        public string FileStorageName
        {
            get { return (String)GetValue(FileStorageNameProperty); }
            set { SetValue(FileStorageNameProperty, value); }
        }

        /// <property name="flag" value="Finished" />
        ///  <para/>
        /// <summary>
        /// Gets or sets the value of the m_Root dependency property.
        /// </summary>
        private IAutocompleteRoot Root
        {
            get
            {
                if (null == m_root)
                {
                    CreateRoot();
                }
                return m_root;
            }
            set
            {
                if (null != m_root)
                {
                    m_root = value;
                }
            }
        }

        /// <summary>
        /// Gets the name of the store file.
        /// </summary>
        /// <value>The name of the store file.</value>
        private string StoreFileName
        {
            get
            {
                int lenght = m_storeFileName.Length;

                if (MaxFileNameLenght <= lenght)
                {
                    return m_storeFileName.Remove(0, lenght - MaxFileNameLenght);
                }

                return m_storeFileName;
            }
        }

        #endregion Properties

        #region Public methods

        /// <property name="flag" value="Finished" />
        ///  <para/>
        /// <summary>
        /// Called when an internal process or application calls
        /// ApplyTemplate, which is used to build the current template's
        /// visual tree.
        /// </summary>
        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();
            ObjectHistory = new List<KeyValuePair<string, object>>();
            m_vistaProgressBar = GetTemplateChild(PART_VistaProgressBar) as VistaSpecificProgressBar;

            if (null != m_textBox)
            {
                m_textBox.TextChanged -= new TextChangedEventHandler(OnChildTextBoxTextChanged);
                m_textBox.GotMouseCapture -= new MouseEventHandler(m_textBox_GotMouseCapture);
                m_textBox.PreviewKeyDown -= new KeyEventHandler(OnChildTextBoxPreviewKeyDown);
                m_textBox.LostFocus -= new RoutedEventHandler(OnChildTextBoxLostFocus);
            }

            if (null != m_itemsAutoComplete)
            {
                m_itemsAutoComplete.MouseLeftButtonUp -= new MouseButtonEventHandler(OnChildm_ItemsAutoCompleteMouseLeftButtonUp);
                m_itemsAutoComplete.RemoveHandler(ListBox.KeyDownEvent, new KeyEventHandler(OnChildm_ItemsAutoCompleteKeyDown));
                m_itemsAutoComplete.SelectionChanged -= new SelectionChangedEventHandler(m_itemsAutoComplete_SelectionChanged);
            }

            m_textBox = GetTemplateChild(PART_EditableTextBox) as TextBox;
            if (m_textBox != null && this.IsFocused)
            {
                m_textBox.Focus();
            }
            m_itemsAutoComplete = GetTemplateChild(PART_Container) as ListBox;

            if (m_itemsAutoComplete != null)
            {
                m_itemsAutoComplete.SelectionChanged += new SelectionChangedEventHandler(m_itemsAutoComplete_SelectionChanged);
                m_itemsAutoComplete.AddHandler(ListBox.KeyDownEvent, new KeyEventHandler(OnChildm_ItemsAutoCompleteKeyDown), true);
                m_itemsAutoComplete.MouseLeftButtonUp += new MouseButtonEventHandler(OnChildm_ItemsAutoCompleteMouseLeftButtonUp);
            }

            if (null != m_textBox)
            {
                m_textBox.TextChanged += new TextChangedEventHandler(OnChildTextBoxTextChanged);
                m_textBox.GotMouseCapture += new MouseEventHandler(m_textBox_GotMouseCapture);
                m_textBox.PreviewKeyDown += new KeyEventHandler(OnChildTextBoxPreviewKeyDown);
                m_textBox.LostFocus += new RoutedEventHandler(OnChildTextBoxLostFocus);
                m_textBox.Text = Text;
                SetTextBoxErrorTemplate();

                if (!Check_DuplicateKey())
                    m_itemsAutoComplete.ItemsSource = m_AutoCompleteList;

                //// Methos to add the history listbox if IsHistory is true.
                CreateViewHistory();
            }
            else
            {
                throw new NotSupportedException("Set incorrect template.");
            }

            if (this.HistoryThumb != null)
            {
                this.HistoryThumb.MouseEnter -= new MouseEventHandler(HistoryThumb_MouseEnter);
                this.HistoryThumb.MouseLeave -= new MouseEventHandler(HistoryThumb_MouseLeave);
                this.HistoryThumb.DragDelta -= new DragDeltaEventHandler(HistoryThumb_DragDelta);
            }

            if (this.ContainerThumb != null)
            {
                this.ContainerThumb.MouseEnter -= new MouseEventHandler(HistoryThumb_MouseEnter);
                this.ContainerThumb.MouseLeave -= new MouseEventHandler(HistoryThumb_MouseLeave);
                this.ContainerThumb.DragDelta -= new DragDeltaEventHandler(ContainerThumb_DragDelta);
            }

            this.HistoryThumb = GetTemplateChild("HistoryThumb") as Thumb;
            this.ContainerThumb = GetTemplateChild("ContainerThumb") as Thumb;

            if (this.HistoryThumb != null)
            {
                this.HistoryThumb.MouseEnter += new MouseEventHandler(HistoryThumb_MouseEnter);
                this.HistoryThumb.MouseLeave += new MouseEventHandler(HistoryThumb_MouseLeave);
                this.HistoryThumb.DragDelta += new DragDeltaEventHandler(HistoryThumb_DragDelta);
            }

            if (this.ContainerThumb != null)
            {
                this.ContainerThumb.MouseEnter += new MouseEventHandler(HistoryThumb_MouseEnter);
                this.ContainerThumb.MouseLeave += new MouseEventHandler(HistoryThumb_MouseLeave);
                this.ContainerThumb.DragDelta += new DragDeltaEventHandler(ContainerThumb_DragDelta);
            }

            if (this.IsDropDownOpen == true)
                this.IsDropDownOpen = false;
        }

        /// <property name="flag" value="Finished" />
        /// <summary>
        /// Called when ChildTextBox is Lost Focus
        /// this element.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The instance containing the event data.</param>
        private void OnChildTextBoxLostFocus(object sender, RoutedEventArgs e)
        {
            if (IsAutoAppend)
                AddToSelectedObjects(m_textBox.Text);
        }

        /// <summary>
        /// Handles the DragDelta event of the ContainerThumb control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.Windows.Controls.Primitives.DragDeltaEventArgs"/> instance containing the event data.</param>
        private void ContainerThumb_DragDelta(object sender, DragDeltaEventArgs e)
        {
            double yadjust = this.m_itemsAutoComplete.ActualHeight + e.VerticalChange;
            double xadjust = this.m_itemsAutoComplete.ActualWidth + e.HorizontalChange;

            if ((xadjust >= 0) && (yadjust >= 0))
            {
                this.m_itemsAutoComplete.Width = xadjust;
                this.m_itemsAutoComplete.Height = yadjust;
                if (this.m_itemsHistory != null)
                {
                    this.m_itemsHistory.Width = xadjust;
                    this.m_itemsHistory.Height = yadjust;
                }
            }
        }

        /// <summary>
        /// Handles the DragDelta event of the HistoryThumb control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.Windows.Controls.Primitives.DragDeltaEventArgs"/> instance containing the event data.</param>
        private void HistoryThumb_DragDelta(object sender, DragDeltaEventArgs e)
        {
            if (this.m_itemsHistory != null)
            {
                double yadjust = this.m_itemsHistory.ActualHeight + e.VerticalChange;
                double xadjust = this.m_itemsHistory.ActualWidth + e.HorizontalChange;

                if ((xadjust >= 0) && (yadjust >= 0))
                {
                    this.m_itemsHistory.Width = xadjust;
                    this.m_itemsHistory.Height = yadjust;
                    this.m_itemsAutoComplete.Width = xadjust;
                    this.m_itemsAutoComplete.Height = yadjust;
                }
            }
        }

        /// <summary>
        /// Handles the MouseLeave event of the HistoryThumb control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.Windows.Input.MouseEventArgs"/> instance containing the event data.</param>
        private void HistoryThumb_MouseLeave(object sender, MouseEventArgs e)
        {
            Thumb t = sender as Thumb;
            t.Cursor = Cursors.None;
        }

        /// <summary>
        /// Handles the MouseEnter event of the HistoryThumb control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.Windows.Input.MouseEventArgs"/> instance containing the event data.</param>
        private void HistoryThumb_MouseEnter(object sender, MouseEventArgs e)
        {
            Thumb t = sender as Thumb;
            t.Cursor = Cursors.SizeNWSE;
        }

        /// <summary>
        /// Handles the GotMouseCapture event of the m_textBox control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.Windows.Input.MouseEventArgs"/> instance containing the event data.</param>
        private void m_textBox_GotMouseCapture(object sender, MouseEventArgs e)
        {
            IsHistoryDropDownOpen = false;
            IsDropDownOpen = false;
        }

        private void m_HistoryitemsAutoComplete_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (SourceMode.Custom == Source && !DisplayMemberPath.Equals(""))
            {
         
                for (int i = 0; i < m_itemsHistory.SelectedItems.Count; i++)
                {
                    for (int k = 0; k < BusinessObject.Count; k++)
                        if (m_itemsHistory.SelectedItem.ToString() == BusinessObject[k].Key)
                        {
                            m_selectedItem = BusinessObject[k].Value;
                            SelectedItem = m_selectedItem;
                        }
                    for (int j = 0; j < BusinessObject.Count; j++)
                        if (m_itemsHistory.SelectedItems[i].ToString() == BusinessObject[j].Key)
                            if (!m_selectedItems.Contains(BusinessObject[j].Value))
                            {
                                m_selectedItems.Add(BusinessObject[j].Value);
                                if (!m_tempSelectedItems.Contains((object)BusinessObject[j].Key))
                                    m_tempSelectedItems.Add((object)BusinessObject[j].Key);
                            }
                }

                for (int k = 0; k < BusinessObject.Count; k++)
                    if (!m_itemsHistory.SelectedItems.Contains((object)BusinessObject[k].Key))
                        if (m_selectedItems.Contains(BusinessObject[k].Value))
                        {
                            m_selectedItems.Remove(BusinessObject[k].Value);
                            if (SelectionMode == SelectionMode.Extended)
                            {
                                if (m_tempSelectedItems.Contains((object)BusinessObject[k].Key))
                                    m_tempSelectedItems.Remove((object)BusinessObject[k].Key);
                            }
                        }
            }
            else
            {
                if (SelectionMode == SelectionMode.Extended)
                {
                    m_tempSelectedItems.Clear();
                }
                m_selectedItem = m_itemsHistory.SelectedItem;
                SelectedItem = m_selectedItem;
                m_selectedItems = m_itemsHistory.SelectedItems;
                if (SelectionMode == SelectionMode.Single)
                    m_tempSelectedItems.Clear();
                foreach (object obj in m_selectedItems)
                {
                    if (!m_tempSelectedItems.Contains(obj))
                    {
                        m_tempSelectedItems.Add(obj);
                        string source = obj.ToString();
                        if (this.Items.Contains(source))
                        {
                            this.isIndexChangedInDesign = false;
                            this.SelectedIndex = this.Items.IndexOf(source);
                        }
                    }
                }
                if (SelectionMode == SelectionMode.Single && !m_addtoselectedobj)
                    SetSafeText(String.Empty);
            }
            this.OnSelectionChanged(e);
        }

        /// <summary>
        /// Handles the SelectionChanged event of the m_itemsAutoComplete control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.Windows.Controls.SelectionChangedEventArgs"/> instance containing the event data.</param>
        private void m_itemsAutoComplete_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (!isDuplicate)
            {
                bool compare_Check = false;
                IsInternalchange = false;
                if (this.m_itemsAutoComplete.SelectedItem != null && !isCalledfromToggleButtonCheck)
                {
                    if (SourceMode.Custom == Source && !DisplayMemberPath.Equals(""))
                    {
         
                        if (EnableSorting)
                        {
                            BusinessObject.Sort(SortBusinessObjectKey);
                        }

                        var _isValidSelectedValue = false;
                        object listBox_SelectedValue = null;
                        var listBox_SelectedItem = BusinessObject[m_itemsAutoComplete.SelectedIndex].Value;

                        for (int i = 0; i < m_itemsAutoComplete.SelectedItems.Count; i++)
                        {
                            for (int k = 0; k < BusinessObject.Count; k++)
                            {
                                compare_Check = ((m_itemsAutoComplete.SelectedItem == BusinessObject[k].Value) || (m_itemsAutoComplete.SelectedItem.ToString() == BusinessObject[k].Key));

                                if (compare_Check && !_isValidSelectedValue)
                                {
                                    if (listBox_SelectedItem != null)
                                    {
                                        listBox_SelectedValue = listBox_SelectedItem.GetType().GetProperty(SelectedValuePath).GetValue(listBox_SelectedItem, null);
                                    }

                                    m_selectedItem = BusinessObject[k].Value;
                                    SelectedItem = m_selectedItem;
                                    if (String.IsNullOrEmpty(SelectedValuePath))
                                    {
                                        SelectedValue = m_selectedItem;
                                    }
                                    else
                                    {
                                        if (m_selectedItem.GetType().GetProperty(SelectedValuePath) != null)
                                            SelectedValue = m_selectedItem.GetType().GetProperty(SelectedValuePath).GetValue(m_selectedItem, null);

                                        if (listBox_SelectedValue != null && SelectedValue.ToString() == listBox_SelectedValue.ToString())
                                        {
                                            _isValidSelectedValue = true;
                                        }
                                    }
                                    compare_Check = false;
                                }
                            }
                            for (int j = 0; j < BusinessObject.Count; j++)
                                if (m_itemsAutoComplete.SelectedItems[i].ToString() == BusinessObject[j].Key)
                                    if (!m_selectedItems.Contains(BusinessObject[j].Value) && SelectionMode != SelectionMode.Single)
                                    {
                                        m_selectedItems.Add(BusinessObject[j].Value);
                                        if (!m_tempSelectedItems.Contains((object)BusinessObject[j].Key))
                                            m_tempSelectedItems.Add((object)BusinessObject[j].Key);
                                    }
                                    else if (!m_selectedItems.Contains(BusinessObject[j].Value) && SelectionMode == SelectionMode.Single)
                                    {
                                        m_tempSelectedItems.Clear();
                                        if (!m_tempSelectedItems.Contains((object)BusinessObject[j].Key))
                                            m_tempSelectedItems.Add((object)BusinessObject[j].Key);
                                    }
                        }

                        for (int k = 0; k < BusinessObject.Count; k++)
                            if (!m_itemsAutoComplete.SelectedItems.Contains((object)BusinessObject[k].Key))
                                if (m_selectedItems.Contains(BusinessObject[k].Value))
                                {
                                    m_selectedItems.Remove(BusinessObject[k].Value);
                                    if (SelectionMode == SelectionMode.Extended)
                                    {
                                        if (m_tempSelectedItems.Contains((object)BusinessObject[k].Key))
                                            m_tempSelectedItems.Remove((object)BusinessObject[k].Key);
                                    }
                                }
                    }
                    else
                    {
                        if (SelectionMode == SelectionMode.Extended)
                        {
                            m_tempSelectedItems.Clear();
                        }
                        foreach (object obj in this.Items)
                        {
                            if (e.AddedItems.Count >= 1)
                            {
                                if (obj.ToString() == (e.AddedItems[0]).ToString())
                                {
                                    SelectedItem = obj;
                                }
                            }
                        }
                        m_selectedItem = m_itemsAutoComplete.SelectedItem;
                        m_selectedItems = m_itemsAutoComplete.SelectedItems;
                        if (SelectionMode == SelectionMode.Single)
                            m_tempSelectedItems.Clear();
                        foreach (object obj in m_selectedItems)
                        {
                            if (!m_tempSelectedItems.Contains(obj))
                            {
                                m_tempSelectedItems.Add(obj);
                                this.isIndexChangedInDesign = false;
                                this.SelectedIndex = this.Items.IndexOf(obj);
                            }
                        }
                        if (SelectionMode == SelectionMode.Single && !m_addtoselectedobj)
                            SetSafeText(String.Empty);
                    }
                }
                IsInternalchange = true;
                this.OnSelectionChanged(e);
            }
            else
                isDuplicate = false;
        }

        /// <summary>
        /// Sets the text box error template.
        /// </summary>
        private void SetTextBoxErrorTemplate()
        {
            Validation.SetErrorTemplate(m_textBox, null);
        }

        /// <summary>
        /// Returns the selected value from obj.
        /// </summary>
        /// <param name="input">The input.</param>
        /// <returns></returns>
        private object ReturnSelectedValueFromObj(object input)
        {
            if (this.ItemsSource == null)
            {
                this.SelectedValue = this.SelectedItem;
            }
            if (input is string)
            {
                return input.ToString();
            }
            else if (input is System.Xml.XmlNode)
            {
                return ((System.Xml.XmlNode)input).Value;
            }
            else
            {
                if (!SelectedValuePath.Equals(""))
                {
                    Type t = input.GetType();
                    var prop = t.GetProperty(SelectedValuePath);
                    object s = prop.GetValue(input, null);
                    return s;
                }
                else
                {
                    return input;
                }
            }
        }

        #endregion Public methods

        #region Implemenation

        /// <property name="flag" value="Finished" />
        ///  <para/>
        /// <summary>
        /// Updates property value cache and raises SourceChanged event.
        /// </summary>
        /// <param name="e">Property changes details, such as old value and new value.
        /// </param>
        protected virtual void OnSourceChanged(DependencyPropertyChangedEventArgs e)
        {
            if (null != m_textBox)
            {
                SetSafeText(String.Empty);
            }

            Root = null;
            m_hashLevel = null;

            if (SourceChanged != null)
            {
                SourceChanged(this, e);
            }
        }

        /// <property name="flag" value="Finished" />
        /// <summary>
        /// Updates property value cache and raises IsDropDownOpenChanged
        /// event.
        /// </summary>
        /// <param name="e">Property changes details, such as old value and new value.
        /// </param>
        protected virtual void OnIsDropDownOpenChanged(DependencyPropertyChangedEventArgs e)
        {
            if (m_dropdownflag)
            {
                if (IsDropDownOpen)
                {
                    Mouse.Capture(this, CaptureMode.SubTree);
                }
                else
                {
                    Mouse.Capture(null);
                }
            }

            if (IsDropDownOpenChanged != null)
            {
                IsDropDownOpenChanged(this, e);
            }
        }

        /// <property name="flag" value="Finished" />
        /// <summary>
        /// Updates property value cache and raises CustomSourceChanged
        /// event.
        /// </summary>
        /// <param name="e">Property changes details, such as old value
        /// and new value.</param>
        protected virtual void OnCustomSourceChanged(DependencyPropertyChangedEventArgs e)
        {
            if (SourceMode.Custom == Source)
            {
                Root = null;
                ItemsSource = CustomSource;
                MapCustomObject();
            }

            if (CustomSourceChanged != null)
            {
                CustomSourceChanged(this, e);
            }
        }

        /// <property name="flag" value="Finished" />
        /// <summary>
        /// Updates property value cache and raises IsAutoAppendChanged
        /// event.
        /// </summary>
        /// <param name="e">Property changes details, such as old value
        /// and new value.</param>
        protected virtual void OnIsAutoAppendChanged(DependencyPropertyChangedEventArgs e)
        {
            CoerceValue(AutoComplete.IsAutoCompleteItemProperty);

            if (IsAutoAppendChanged != null)
            {
                IsAutoAppendChanged(this, e);
            }
        }

        /// <property name="flag" value="Finished" />
        /// <summary>
        /// Updates property value cache and raises MaxDropHeightChanged
        /// event.
        /// </summary>
        /// <param name="e">Property changes details, such as old value
        /// and new value.</param>
        protected virtual void OnMaxDropHeightChanged(DependencyPropertyChangedEventArgs e)
        {
            if (MaxDropHeightChanged != null)
            {
                MaxDropHeightChanged(this, e);
            }
        }

        /// <property name="flag" value="Finished" />
        /// <summary>
        /// Updates property value cache and raises IsFilterChanged
        /// event.
        /// </summary>
        /// <param name="e">Property changes details, such as old value
        /// and new value.</param>
        protected virtual void OnIsFilterChanged(DependencyPropertyChangedEventArgs e)
        {
            CoerceValue(AutoComplete.IsAutoAppendProperty);

            if (IsFilterChanged != null)
            {
                IsFilterChanged(this, e);
            }
        }

        /// <property name="flag" value="Finished" />
        /// <summary>
        /// Updates property value cache and raises
        /// IsAutoCompleteItemChanged event.
        /// </summary>
        /// <param name="e">Property changes details, such as old value
        /// and new value.</param>
        protected virtual void OnIsAutoCompleteItemChanged(DependencyPropertyChangedEventArgs e)
        {
            if (IsAutoCompleteItemChanged != null)
            {
                IsAutoCompleteItemChanged(this, e);
            }
        }

        /// <property name="flag" value="Finished" />
        /// <summary>
        /// Updates property value cache and raises
        /// IsAsyncAddContentChanged event.
        /// </summary>
        /// <param name="e">Property changes details, such as old value
        /// and new value.</param>
        protected virtual void OnIsAsyncAddContentChanged(DependencyPropertyChangedEventArgs e)
        {
            if (IsAsyncAddContentChanged != null)
            {
                IsAsyncAddContentChanged(this, e);
            }
        }

        /// <property name="flag" value="Finished" />
        /// <summary>
        /// Updates property value cache and raises TextChanged event.
        /// </summary>
        /// <param name="e">Property changes details, such as old value
        /// and new value.</param>
        protected virtual void OnTextChanged(DependencyPropertyChangedEventArgs e)
        {
            if (TextChanged != null)
            {
                TextChanged(this, e);
            }
        }

        /// <property name="flag" value="Finished" />
        /// <summary>
        /// Updates property value cache and raises IsHistoryChanged
        /// event.
        /// </summary>
        /// <param name="e">Property changes details, such as old value
        /// and new value.</param>
        protected virtual void OnIsHistoryChanged(DependencyPropertyChangedEventArgs e)
        {
            if (DesignerProperties.GetIsInDesignMode(this))
            {
            }
            else
            {
                if (IsHistory)
                {
                    MapCustomObject();
                    CreateViewHistory();
                    RelateHistory();
                }
                else
                {
                    MapCustomObject();
                    m_CustomHistoryList.Clear();
                }
            }

            if (IsHistoryChanged != null)
            {
                IsHistoryChanged(this, e);
            }
        }

        /// <property name="flag" value="Finished" />
        /// <summary>
        /// Updates property value cache and raises
        /// IsHistoryDropDownOpenChanged event.
        /// </summary>
        /// <param name="e">Property changes details, such as old value
        /// and new value.</param>
        protected virtual void OnIsHistoryDropDownOpenChanged(DependencyPropertyChangedEventArgs e)
        {
            if (IsHistoryDropDownOpen)
            {
                Mouse.Capture(this, CaptureMode.SubTree);
                if (this.EnableDropDown)
                {
                    if (!this.IsFocused)
                        this.Focus();
                }
            }
            else
            {
                Mouse.Capture(null);
            }

            if (IsHistoryDropDownOpenChanged != null)
            {
                IsHistoryDropDownOpenChanged(this, e);
            }
        }

        /// <property name="flag" value="Finished" />
        /// <summary>
        /// Updates property value cache and raises
        /// m_HistoryListHeightChanged event.
        /// </summary>
        /// <param name="e">Property changes details, such as old value
        /// and new value.</param>
        protected virtual void Onm_HistoryListHeightChanged(DependencyPropertyChangedEventArgs e)
        {
            if (HistoryListHeightChanged != null)
            {
                HistoryListHeightChanged(this, e);
            }
        }

        /// <property name="flag" value="Finished" />
        /// <summary>
        /// Updates property value cache and raises
        /// FileStorageNameChanged event.
        /// </summary>
        /// <param name="e">Property changes details, such as old value
        /// and new value.</param>
        protected virtual void OnFileStorageNameChanged(DependencyPropertyChangedEventArgs e)
        {
            if (FileStorageNameChanged != null)
            {
                FileStorageNameChanged(this, e);
            }
        }

        /// <summary>
        /// Raises the <see cref="E:SelectionModeChanged"/> event.
        /// </summary>
        /// <param name="e">The <see cref="System.Windows.DependencyPropertyChangedEventArgs"/> instance containing the event data.</param>
        protected virtual void OnSelectionModeChanged(DependencyPropertyChangedEventArgs e)
        {
            if (SelectionModeChanged != null)
            {
                SelectionModeChanged(this, e);
            }
        }

        /// <summary>
        /// Raises the <see cref="E:SelectionChanged"/> event.
        /// </summary>
        /// <param name="e">The <see cref="System.Windows.Controls.SelectionChangedEventArgs"/> instance containing the event data.</param>
        protected virtual void OnSelectionChanged(SelectionChangedEventArgs e)
        {
            if (SelectionChanged != null)
            {
                SelectionChanged(this, e);
            }
        }

        /// <property name="flag" value="Finished" />
        /// <summary>
        /// This method creates some root object which depends on the
        /// source.
        /// </summary>
        protected virtual void CreateRoot()
        {
            Clear();
            m_hashLevel = null;

            if (SourceMode.FilePath == Source)
            {
                m_root = new FilePathRoot();
            }
            else if (SourceMode.Registry == Source)
            {
                m_root = new RegistryRoot();
            }
            else if (SourceMode.Custom == Source)
            {
                m_root = new CustomRoot(CustomSourceString, EnableSorting);
            }
            else
            {
                Debug.Fail("We don't support this Source!");
            }
        }

        /// <property name="flag" value="Finished" />
        /// <summary>
        /// Causes the object to scroll into view for auto-append.
        /// </summary>
        protected virtual void ScrollIntoAutoCompleteView()
        {
            ListBox litBox = m_itemsAutoComplete as ListBox;

            if (null != litBox && null != litBox.SelectedItem)
            {
                litBox.ScrollIntoView(litBox.SelectedItem);
            }
        }

        /// <property name="flag" value="Finished" />
        /// <summary>
        /// Causes the object to scroll into view for history.
        /// </summary>
        protected virtual void ScrollIntoHistoryView()
        {
            ListBox litBox = m_itemsHistory as ListBox;

            if (null != litBox && null != litBox.SelectedItem)
            {
                litBox.ScrollIntoView(litBox.SelectedItem);
            }
        }

        /// <summary>
        /// change the customToggleValue
        /// </summary>
        private void changeIsCustomToggleProperty()
        {
            if (isCustomToggleCheck == true && EnableDropDown == false)
            {
                isCustomToggleCheck = false;
                EnableDropDown = true;
            }
        }

        /// <property name="flag" value="Finished" />
        /// <summary>
        /// Invoked when an unhandled Mouse.LostMouseCapture attached
        /// event reaches an element in its route that is derived from
        /// this class.
        /// </summary>
        /// <param name="e">The instance containing the event data.</param>
        protected override void OnLostMouseCapture(MouseEventArgs e)
        {
            changeIsCustomToggleProperty();
            if (Mouse.Captured != this)
            {
                if (e.OriginalSource == this || e.OriginalSource is ToggleButton || e.OriginalSource is TextBox)
                {
                    if (Mouse.Captured == null || Mouse.Captured.GetType() == typeof(ToggleButton)
                        || Mouse.Captured.GetType() == typeof(ItemsControl))
                    {
                        if (IsDropDownOpen)
                        {
                            IsHistoryDropDownOpen = false;
                        }
                    }
                }
                else if (VisualUtils.IsDescendant((DependencyObject)e.Source, (DependencyObject)e.OriginalSource))
                {
                    Mouse.Capture(this, CaptureMode.SubTree);
                }
            }

            base.OnLostMouseCapture(e);
        }

        /// <summary>
        /// Invoked whenever an unhandled <see cref="E:System.Windows.UIElement.GotFocus"/> event reaches this element in its route.
        /// </summary>
        /// <param name="e">The <see cref="T:System.Windows.RoutedEventArgs"/> that contains the event data.</param>
        protected override void OnGotFocus(RoutedEventArgs e)
        {
            base.OnGotFocus(e);
            if (m_textBox != null)
            {
                m_textBox.Focus();
            }
        }

        /// <property name="flag" value="Finished" />
        /// <summary>
        /// Invoked when an unhandled Mouse.MouseDown attached event
        /// reaches an element in its route that is derived from this
        /// class.
        /// </summary>
        /// <param name="e">The instance that contains the event data.
        /// This event data reports details about the
        /// mouse button that was pressed and the
        /// handled state.</param>
        protected override void OnMouseDown(MouseButtonEventArgs e)
        {
            if (Mouse.Captured == this)
            {
                m_dropdownflag = false;
                IsDropDownOpen = false;
                IsHistoryDropDownOpen = false;
                m_dropdownflag = true;
                this.ReleaseMouseCapture();
            }

            base.OnMouseDown(e);
        }

        /// <summary>
        /// Invoked when an unhandled <see cref="E:System.Windows.Input.Keyboard.KeyUp"/>�attached event reaches an element in its route that is derived from this class. Implement this method to add class handling for this event.
        /// </summary>
        /// <param name="e">The <see cref="T:System.Windows.Input.KeyEventArgs"/> that contains the event data.</param>
        protected override void OnKeyUp(KeyEventArgs e)
        {
            if (Key.RightShift == e.Key || Key.LeftShift == e.Key)
            {
                if (SelectionMode != SelectionMode.Single)
                {
                    shift = false;
                }
            }
            base.OnKeyUp(e);
        }

        /// <property name="flag" value="Finished" />
        /// <summary>
        /// Responds to the KeyDown event.
        /// </summary>
        /// <param name="e">The instance that contains the event data.</param>
        protected override void OnKeyDown(KeyEventArgs e)
        {
            if (Key.RightCtrl == e.Key || Key.LeftCtrl == e.Key)
            {
                if (SelectionMode != SelectionMode.Single)
                {
                    if (!IsHistory)
                    {
                        if (IsHistoryDropDownOpen == true)
                            IsHistoryDropDownOpen = false;
                        IsDropDownOpen = true;
                    }
                }
            }
            if (Key.RightShift == e.Key || Key.LeftShift == e.Key)
            {
                if (SelectionMode != SelectionMode.Single)
                {
                    shift = true;
                }
            }
            if (Key.Return == e.Key)
            {
                if (IsDropDownOpen)
                {
                    IsDropDownOpen = false;
                    m_textBox.Focus();
                }
                else if (IsHistoryDropDownOpen)
                {
                    ScrollIntoHistoryView();
                    IsHistoryDropDownOpen = false;
                }
            }
            else if (Key.Escape == e.Key)
            {
                if (IsDropDownOpen)
                {
                    IsDropDownOpen = false;
                    m_textBox.Focus();
                    SetSafeText(String.Empty);
                }
                else if (IsHistoryDropDownOpen)
                {
                    IsHistoryDropDownOpen = false;
                }
            }
            base.OnKeyDown(e);
        }

        /// <property name="flag" value="Finished" />
        /// <summary>
        /// Calls after control's graphic is loaded.
        /// </summary>
        private void LoadDispatherLoaded()
        {
            RelateHistory();

            m_AutoCompleteList.CollectionChanged += new NotifyCollectionChangedEventHandler(OnObserverCollectionChanged);

            if (IsDropDownOpen && m_textBox != null)
            {
                m_textBox.Focus();
            }
        }

        /// <property name="flag" value="Finished" />
        /// <summary>
        /// Calls when observer collection is changed.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The instance containing the event data.</param>
        private void OnObserverCollectionChanged(Object sender, NotifyCollectionChangedEventArgs e)
        {
            if (0 == m_AutoCompleteList.Count && m_isTotalClear&&IsFilter)
            {
                IsDropDownOpen = false;
            }
            if (e.Action == NotifyCollectionChangedAction.Reset)
                MapCustomObject();
        }

        private void OnChildm_HistoryItemsAutoCompleteKeyDown(Object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Space)
            {
                EnterTextSelectText();
                m_textBox.ScrollToHorizontalOffset((double)m_textBox.Text.Length * 10);
            }
            else
                if (Key.Down == e.Key || Key.Up == e.Key)
                {
                    if (EnableDropDown == true)
                    {
                        isCustomToggleCheck = true;
                    }
                    if (SelectionMode == SelectionMode.Single)
                    {
                        ShowSelectedItem();
                    }
                    else
                    {
                        m_keyupdown = true;
                        string itemstr;
                        ListBox lst = sender as ListBox;
                        ListBoxItem item;

                        for (int i = 0; i < lst.Items.Count; i++)
                        {
                            try
                            {
                                item = lst.ItemContainerGenerator.ContainerFromItem(lst.Items[i]) as ListBoxItem;

                                if (item.IsKeyboardFocused)
                                {
                                    itemstr = item.Content.ToString();
                                    if (SelectionMode == SelectionMode.Extended)
                                    {
                                        SetSafeText(String.Empty);
                                    }
                                    else
                                    {
                                        if (!m_itemsHistory.SelectedItems.Contains((object)itemstr))
                                        {
                                            SetSafeText(itemstr);
                                        }
                                        else
                                        {
                                            SetSafeText(String.Empty);
                                        }
                                    }
                                    m_textBox.ScrollToHorizontalOffset((double)m_textBox.Text.Length * 10);
                                    break;
                                }
                            }
                            catch (Exception)
                            {
                            }
                        }

                        m_keyupdown = false;
                    }
                }
                else if (KeyCode.KeycodeToChar(e.Key, shift).Equals(SeparatorChar.ToString()))
                {
                    SetSafeText(String.Empty);
                    m_textBox.Focus();
                }
                else if (e.Key == Key.Tab)
                {
                    IsDropDownOpen = false;
                    if (SelectionMode == SelectionMode.Single)
                    {
                        EnterTextSelectText();
                    }
                }
                else
                {
                    changeIsCustomToggleProperty();
                }
        }

        /// <summary>
        /// Called when [childm_ items auto complete key down].
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The <see cref="System.Windows.Input.KeyEventArgs"/> instance containing the event data.</param>
        private void OnChildm_ItemsAutoCompleteKeyDown(Object sender, KeyEventArgs e)
        {
            HandleKey(sender, e);
        }

        private void HandleKey(Object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Space)
            {
                EnterTextSelectText();
                m_textBox.ScrollToHorizontalOffset((double)m_textBox.Text.Length * 10);
            }
            else
                if (Key.Down == e.Key || Key.Up == e.Key)
                {
                    if (EnableDropDown == true)
                    {
                        isCustomToggleCheck = true;
                    }
                    if (SelectionMode == SelectionMode.Single)
                    {
                        ShowSelectedItem();
                    }
                    else
                    {
                        m_keyupdown = true;
                        string itemstr;
                        ListBox lst = sender as ListBox;
                        ListBoxItem item;

                        for (int i = 0; i < lst.Items.Count; i++)
                        {
                            try
                            {
                                item = lst.ItemContainerGenerator.ContainerFromItem(lst.Items[i]) as ListBoxItem;

                                if (item.IsKeyboardFocused)
                                {
                                    itemstr = item.Content.ToString();
                                    if (SelectionMode == SelectionMode.Extended)
                                    {
                                        SetSafeText(String.Empty);
                                    }
                                    else
                                    {
                                        if (!m_itemsAutoComplete.SelectedItems.Contains((object)itemstr))
                                        {
                                            SetSafeText(itemstr);
                                        }
                                        else
                                        {
                                            SetSafeText(String.Empty);
                                        }
                                    }
                                    m_textBox.ScrollToHorizontalOffset((double)m_textBox.Text.Length * 10);
                                    break;
                                }
                            }
                            catch (Exception)
                            {
                            }
                        }

                        m_keyupdown = false;
                    }
                }
                else if (KeyCode.KeycodeToChar(e.Key, shift).Equals(SeparatorChar.ToString()))
                {
                    SetSafeText(String.Empty);
                    m_textBox.Focus();
                }

                else if (Key.Tab == e.Key && e.KeyboardDevice.Modifiers == ModifierKeys.Control)
                {
                    IsDropDownOpen = false;
                    IsHistoryDropDownOpen = false;
                }
                else if (e.Key == Key.Tab)
                {
                    ListBox lst = sender as ListBox;
                    if (Keyboard.IsKeyDown(Key.LeftShift) || Keyboard.IsKeyDown(Key.LeftShift))
                    {
                        if (lst.SelectedIndex == 0)
                            lst.SelectedIndex = lst.Items.Count - 1;
                        else if (lst.SelectedIndex > 0)
                            lst.SelectedIndex -= 1;
                    }
                    else if (lst.SelectedIndex == lst.Items.Count - 1)
                    {
                        lst.SelectedIndex = 0;
                    }
                    else if (lst.SelectedIndex < lst.Items.Count)
                        lst.SelectedIndex += 1;

                    if (EnableDropDown == true)
                    {
                        isCustomToggleCheck = true;
                    }

                    if (SelectionMode == SelectionMode.Single)
                    {
                        ShowSelectedItem();
                    }
                    else
                    {
                        string itemstr;
                        ListBoxItem item;

                        try
                        {
                            item = lst.ItemContainerGenerator.ContainerFromIndex(lst.SelectedIndex) as ListBoxItem;

                            itemstr = item.Content.ToString();
                            if (SelectionMode == SelectionMode.Extended)
                            {
                                SetSafeText(String.Empty);
                            }
                            else
                            {
                                if (!m_itemsAutoComplete.SelectedItems.Contains((object)itemstr))
                                {
                                    SetSafeText(itemstr);
                                }
                                else
                                {
                                    SetSafeText(String.Empty);
                                }
                            }
                            m_textBox.ScrollToHorizontalOffset((double)m_textBox.Text.Length * 10);
                        }
                        catch (Exception)
                        {
                        }
                    }
                }
                else
                {
                    changeIsCustomToggleProperty();
                }
        }

        /// <property name="flag" value="Finished" />
        /// <summary>
        /// Called when a key is pressed while the keyboard is focused on
        /// this element.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The instance containing the event data.</param>
        private void OnChildTextBoxPreviewKeyDown(Object sender, KeyEventArgs e)
        {
            if (IsHistory && EnableDropDown && (e.Key != Key.Tab && Keyboard.Modifiers != ModifierKeys.Shift))
            {
                IsHistoryDropDownOpen = true;
                IsDropDownOpen = false;
            }
            if (e.Key == Key.Tab)
            {
                IsHistoryDropDownOpen = false;
                IsDropDownOpen = false;
            }
            WorkUpAutoComplateList(e.Key);
            WorkUpm_HistoryList(e.Key);
            e.Handled = WorkUpAutoAppend(e.Key, e);

            if (!e.Handled && e.KeyboardDevice.Modifiers == ModifierKeys.Alt)
            {
                IsDropDownOpen = false;
                IsHistoryDropDownOpen = false;
            }
        }

        /// <summary>
        /// Called when [child text box text changed].
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The <see cref="System.Windows.Controls.TextChangedEventArgs"/> instance containing the event data.</param>
        private void OnChildTextBoxTextChanged(Object sender, TextChangedEventArgs e)
        {
            m_TextChanged = true;
            if (m_textBox != null)
            {
                if (m_textBox.Text != string.Empty)
                {
                    if (m_textBox.CaretIndex != 0)
                        index = m_textBox.CaretIndex;
                    else
                        m_textBox.CaretIndex = index;

                    Text = m_textBox.Text;
                }
                else if (string.IsNullOrEmpty(m_textBox.Text))
                {
                    if (!string.IsNullOrEmpty(Text))
                    {
                        Text = string.Empty;
                    }

                    if (SelectedItem != null || SelectedIndex > -1 || m_selectedItem != null)
                    {
                        SelectedItem = null;
                        SelectedIndex = -1;
                        SelectedValue = null;

                        m_selectedItem = null;
                        m_AutoCompleteList.Clear();
                        m_AutoCompleteObjectList.Clear();
                        m_tempAutoCompleteList.Clear();
                    }
                }
            }

            if (!m_isInternalChange || IsBackKeyPressed)
            {
                string contentText = GetContentText();
                if (!m_autoappend)
                {
                    if (SelectionMode != SelectionMode.Single)
                    {
                        if (m_selectedItems != null && m_selectedItems.Count > 0)
                        {
                            m_selectedItems.Clear();
                        }
                        if (!(SourceMode.Custom == Source && !DisplayMemberPath.Equals("")))
                        {
                            if (m_itemsAutoComplete != null && m_itemsAutoComplete.SelectedItems != null &&
                                m_itemsAutoComplete.SelectedItems.Count > 0)
                            {
                                m_itemsAutoComplete.SelectedItems.Clear();
                            }
                        }
                    }

                    if (m_textBox != null) AddToSelectedObjects(m_textBox.Text);
                }
                m_addtoselectedobj = true;
                if (IsAsyncAddContent)
                {
                    StartAddAsyncLevel(contentText);
                }
                else
                {
                    StartAddSyncLevel(contentText);
                }
                m_addtoselectedobj = false;
            }

            if (IsHistoryDropDownOpen)
            {
                IsDropDownOpen = false;
            }
            var textbox = sender as TextBox;
            if (textbox != null && textbox.Text.Length <= 0)
            {
                if (IsDropDownOpen)
                {
                    IsHistoryDropDownOpen = false;
                }
                m_selectedItem = null;
                IsHistoryDropDownOpen = false;
            }
            else
            {
                if (!IsFilter)
                {
                    if (Source == SourceMode.Custom)
                    {
                        if (SelectedItem == null||IsBackKeyPressed)
                            m_AutoCompleteList.Clear();
                        foreach (KeyValuePair<string, object> bObject in BusinessObject)
                        {          
                            if(!m_AutoCompleteList.Contains(bObject.Key))
                            m_AutoCompleteList.Add(bObject.Key);                           
                        }
                        m_itemsAutoComplete.ItemsSource = m_AutoCompleteList;
                    }
                }
                if (EnableDropDown)
                {
                    if (IsAutoAppend)
                        return;
                    else
                    {
                        IAutocompleteLevel currentLevel = Root.GetRoot(Text);
                        if (currentLevel != null)
                        {
                            string rootText = currentLevel.GetFullPath();
                            var searchstring = String.Empty;
                            if (m_textBox.Text.Contains(SeparatorChar.ToString()) || (IsAutoAppend) && (SelectionMode != SelectionMode.Single) && !(Text.Contains("\\")))
                            {
                                searchstring = Text.Substring(0);
                            }
                            else
                            {
                                if (IsFilter)
                                {
                                    searchstring = Text.Substring(rootText.Length);
                                }
                            }
                                tempfilterCollection = Root.CreateFilteredGhost(currentLevel, searchstring, StringMode, StringModeIndex);
                           
                            AutocompleteItemCollection collection = tempfilterCollection;
                            int countItems = collection.Count;

                            if (IsAutoAppend)
                            {
                                if (0 < countItems)
                                {
                                    m_addtoselectedobj = true;
                                    m_AutoCompleteList.Clear();
                                    m_AutoCompleteObjectList.Clear();
                                    m_tempAutoCompleteList.Clear();
                                    HasDuplicateItem = Check_DuplicateKey();
                                    for (var i = 0; i < countItems; ++i)
                                    {
                                        m_tempAutoCompleteList.Add(rootText + collection[i].Text);
                                        m_AutoCompleteList.Add(rootText + collection[i].Text);
                                        if (HasDuplicateItem)
                                            m_AutoCompleteObjectList.Add(BusinessObject[i].Value);
                                    }
                                }
                            }
                        }
                    }
                }
                if (IsDropDownOpen)
                {
                    IsHistoryDropDownOpen = false;
                }
            }
            m_TextChanged = false;
        }

        /// <property name="flag" value="Finished" />
        /// <summary>
        /// Occurs when the left mouse button is released while the mouse
        /// pointer is over this element.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The instance containing the event data.</param>
        private void OnChildm_ItemsAutoCompleteMouseLeftButtonUp(Object sender, MouseButtonEventArgs e)
        {
            EnterTextSelectText();
            m_textBox.Focus();
        }

        /// <property name="flag" value="Finished" />
        /// <summary>
        /// Calls when level items async loaded.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The instance containing the event data.</param>
        private void OnLevelItemsAsyncLoaded(Object sender, EventArgs e)
        {
            if (IsAsyncAddContent)
            {
                IAutocompleteLevel currentLevel = (IAutocompleteLevel)sender;
                currentLevel.ItemsLoaded -= new EventHandler(OnLevelItemsAsyncLoaded);
                string contentText = GetContentText();
                string rootText = currentLevel.GetFullPath();
                int rootTextLenght = rootText.Length;

                if (contentText.Length >= rootTextLenght && contentText.StartsWith(rootText, StringComparison.OrdinalIgnoreCase))
                {
                    LoadAsyncItems(currentLevel, contentText.Substring(rootTextLenght));
                }
                else
                {
                    StartAddAsyncLevel(contentText);
                }
            }
        }

        /// <property name="flag" value="Finished" />
        /// <summary>
        /// Occurs when the left mouse button is released while the mouse
        /// pointer is over this element.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The instance containing the event data.</param>
        private void OnChildItemsHistoryMouseLeftButtonUp(Object sender, MouseButtonEventArgs e)
        {
            EnterTextSelectText();
            m_textBox.Focus();
            if (SelectionMode != SelectionMode.Extended)
                IsHistoryDropDownOpen = false;
        }

        /// <property name="flag" value="Finished" />
        /// <summary>
        /// Called when a history ItemsControl reports a mouse move.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The instance containing the event data.</param>
        private void OnChildItemsHistoryMouseMove(Object sender, MouseEventArgs e)
        {
            MoveMouse(e.OriginalSource, false);
        }

        private bool isCalledfromToggleButtonCheck = false;

        /// <property name="flag" value="Finished" />
        /// <summary>
        /// Called when a history ToggleButton was checked.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The instance containing the event data.</param>
        private void OnChildToggleButtonChecked(Object sender, RoutedEventArgs e)
        {
            if (Items.Count == 0)
            {
                m_AutoCompleteList.Clear();
                m_tempHistoryList.Clear();
            }
            m_textBox.Focus();
            changeIsCustomToggleProperty();
            if (EnableDropDown == true)
            {
                string contentText = GetContentText();
                IAutocompleteLevel currentLevel;
                if (contentText == string.Empty)
                {
                    currentLevel = Root.GetRoot("");
                }
                else
                {
                    currentLevel = Root.GetRoot(contentText);
                }

                if (currentLevel != null)
                {
                    string rootText = currentLevel.GetFullPath();
                    AutocompleteItemCollection collection = currentLevel.Items;
                    int countItems = collection.Count;
                    if (0 < countItems)
                    {
                        m_addtoselectedobj = true;
                        m_AutoCompleteList.Clear();
                        m_AutoCompleteObjectList.Clear();
                        m_tempAutoCompleteList.Clear();
                        isCalledfromToggleButtonCheck = true;
                        HasDuplicateItem = Check_DuplicateKey();
                        for (int i = 0; i < countItems; ++i)
                        {
                            m_tempAutoCompleteList.Add(rootText + collection[i].Text);
                            m_AutoCompleteList.Add(rootText + collection[i].Text);
                            if (HasDuplicateItem)
                                m_AutoCompleteObjectList.Add(BusinessObject[i].Value);
                        }
                        isCalledfromToggleButtonCheck = false;
                    }
                }

                if (!this.IsDropDownOpen)
                    SetItems(currentLevel, false);
                else
                {
                    if (!IsHistory)
                        SetItems(currentLevel, false);
                }
                if (IsHistory)
                {
                    if (!IsHistoryDropDownOpen)
                        IsHistoryDropDownOpen = true;
                    IsDropDownOpen = false;
                }
                else
                    IsHistoryDropDownOpen = false;

                if (this.ItemTemplate != null)
                {
                    m_itemsAutoComplete.ItemsSource = (currentLevel as CustomLevel).m_NewItems;
                    m_itemsAutoComplete.ItemTemplate = null;
                    m_itemsAutoComplete.ItemTemplate = this.ItemTemplate;
                }
            }
            else
            {
                IsDropDownOpen = false;
            }

            m_textBox.Focus();
            m_textBox.CaretIndex = m_textBox.Text.Length;
        }

        /// <property name="flag" value="Finished" />
        /// <summary>
        /// This method allows to correct the show of selected item in
        /// ItemsControl.
        /// </summary>
        private void ShowSelectedItem()
        {
            if (this.IsHistoryDropDownOpen == true)
            {
                foreach (string str in m_CustomHistoryList)
                {
                    if (!m_itemsHistory.SelectedItems.Contains((object)str) && m_tempSelectedItems.Contains((object)str))
                    {
                        if (SelectionMode == SelectionMode.Single)
                            m_tempSelectedItems.Remove((object)str);
                    }
                }
            }
            else
            {
                if (Check_DuplicateKey())
                {
                    foreach (object str in m_AutoCompleteObjectList)
                    {
                        if (!m_itemsAutoComplete.SelectedItems.Contains((object)str) && m_tempSelectedItems.Contains((object)str))
                        {
                            if (SelectionMode == SelectionMode.Single)
                            {
                                m_tempSelectedItems.Remove((object)str);
                            }
                        }
                    }
                }
                else
                {
                    foreach (string str in m_AutoCompleteList)
                    {
                        if (!m_itemsAutoComplete.SelectedItems.Contains((object)str) && m_tempSelectedItems.Contains((object)str))
                        {
                            if (SelectionMode == SelectionMode.Single)
                            {
                                m_tempSelectedItems.Remove((object)str);
                            }
                        }
                    }
                }
            }
            try
            {
                if (IsHistory)
                {
                    if (null != m_itemsHistory && m_itemsHistory.SelectedItems.Count > 0)
                    {
                        foreach (var item in m_itemsHistory.SelectedItems)
                        {
                            if (!m_tempSelectedItems.Contains(item))
                                m_tempSelectedItems.Add(item);
                        }

                        SetSafeTextWithMoveCaret(String.Empty);
                    }
                }
                else
                {
                    if (null != m_itemsAutoComplete && m_itemsAutoComplete.SelectedItems.Count > 0)
                    {
                        if (Check_DuplicateKey())
                        {
                            if (m_tempSelectedItems.Count > 0)
                            {
                                for (int i = 0; i < m_tempSelectedItems.Count; i++)
                                {
                                    if (string.Compare((m_tempSelectedItems[i].ToString()), (BusinessObject[SelectedIndex].Key.ToString()), false) > 0)
                                    {
                                        m_tempSelectedItems.Add(BusinessObject[SelectedIndex].Key);
                                        break;
                                    }
                                    else
                                    {
                                        m_tempSelectedItems[i] = BusinessObject[SelectedIndex].Key;
                                    }
                                }
                            }
                            else
                            {
                                m_tempSelectedItems.Add(BusinessObject[SelectedIndex].Key);
                            }
                        }
                        else
                        {
                            if (!m_tempSelectedItems.Contains(m_itemsAutoComplete.SelectedItem))
                                m_tempSelectedItems.Add(m_itemsAutoComplete.SelectedItem);
                        }
                        SetSafeTextWithMoveCaret(String.Empty);
                    }
                }
            }
            catch (Exception)
            {
            }
        }

        /// <property name="flag" value="Finished" />
        /// <summary>
        /// This method sets only the new text (doesn't start
        /// auto-complete).
        /// </summary>
        /// <param name="insertString">The new text.</param>
        private void SetSafeText(string insertString)
        {
            m_isInternalChange = true;
            string temptext = String.Empty;
            if (m_keyback == true)
            {
                m_textBox.Text = insertString;
            }
            else
            {
                if (string.IsNullOrEmpty(temptext))
                {
                    if (SelectionMode == SelectionMode.Single)
                    {
                        foreach (var item in BusinessObject)
                        {
                            if (item.Value == SelectedItem)
                            {
                                temptext = item.Key.ToString();
                            }
                        }
                    }
                }

                if (string.IsNullOrEmpty(temptext))
                {
                    for (int i = 0; i < m_tempSelectedItems.Count; i++)
                    {
                        temptext += m_tempSelectedItems[i].ToString() + SeparatorChar.ToString();
                    }
                }

                if (String.IsNullOrEmpty(temptext) || m_keyupdown)
                {
                    temptext += insertString;
                }
                if (temptext.Contains(SeparatorChar.ToString()) && temptext.LastIndexOf(SeparatorChar) == temptext.Length - 1)
                {
                    temptext = temptext.Remove(temptext.Length - 1);
                }
                m_textBox.Text = temptext;
                m_textBox.CaretIndex = m_textBox.Text.Length;
            }
            m_isInternalChange = false;
        }

        /// <summary>
        /// Sets the Value for SelectedItemProperty for given input string
        /// </summary>
        /// <param name="input"></param>
        private void ReturnSelectedObject(string input)
        {
            if (Items.Count > 0)
            {
                bool StringFlag, MemberPathFlag, XmlDataBindingFlag;
                object TestObj = Items[0];
                StringFlag = TestObj is string;
                XmlDataBindingFlag = TestObj is System.Xml.XmlNode;
                MemberPathFlag = DisplayMemberPath.Equals("");
                bool SelectedFlag = false;

                if (StringFlag)
                {
                    foreach (object i in Items)
                    {
                        if (input.Equals((String)i))
                        {
                            SetValue(SelectedItemProperty, i);
                            SelectedFlag = true;
                        }
                    }
                    if (!SelectedFlag)
                    {
                        SetValue(SelectedItemProperty, null);
                        SelectedFlag = false;
                    }
                }
                else if (XmlDataBindingFlag)
                {
                    System.Xml.XmlNode TempNode;
                    foreach (object i in Items)
                    {
                        TempNode = (System.Xml.XmlNode)i;
                        if (input.Equals(TempNode.Value))
                        {
                            SetValue(SelectedItemProperty, TempNode.Value);
                            SelectedFlag = true;
                        }
                    }
                    if (!SelectedFlag)
                    {
                        SetValue(SelectedItemProperty, null);
                        SelectedFlag = false;
                    }
                }
                else if (LoadHistoryFlag)
                {
                    int cnt = ObjectHistory.Count;
                    for (int i = 0; i < cnt; i++)
                    {
                        if (input.Equals(ObjectHistory[i].Key))
                        {
                            SetValue(SelectedItemProperty, ObjectHistory[i].Value);
                            SelectedFlag = true;
                        }
                    }
                    if (!SelectedFlag)
                    {
                        SetValue(SelectedItemProperty, null);
                        SelectedFlag = false;
                    }
                }
                else
                {
                    foreach (object i in Items)
                    {
                        if (!MemberPathFlag)
                        {
                            Type t = i.GetType();
                            var prop = t.GetProperty(DisplayMemberPath);
                            object s = prop.GetValue(i, null);
                            if (input.Equals(s.ToString()))
                            {
                                SetValue(SelectedItemProperty, i);
                                SelectedFlag = true;
                            }
                        }
                        else
                        {
                            if (input.Equals(i.ToString()))
                            {
                                SetValue(SelectedItemProperty, i);
                                SelectedFlag = true;
                            }
                        }
                    }
                    if (!SelectedFlag)
                    {
                        SetValue(SelectedItemProperty, null);
                        SelectedFlag = false;
                    }
                }
            }
        }

        /// <summary>
        /// Map the Custom object to string view collections
        /// </summary>
        private void MapCustomObject()
        {
            if (Items.Count > 0)
            {
                List<object> Temp = new List<object>();
                bool StringFlag, MemberPathFlag, XmlDataBindingFlag;
                object TestObj = Items[0];
                StringFlag = TestObj is string;
                XmlDataBindingFlag = TestObj is System.Xml.XmlNode;
                MemberPathFlag = DisplayMemberPath.Equals(String.Empty);
                m_tempHistoryList.Clear();
                BusinessObject.Clear();
                if (StringFlag)
                {
                    foreach (object i in Items)
                    {
                        if (i != null)
                        {
                            Temp.Add((String)i);
                            m_tempHistoryList.Add((String)i);
                        }
                    }
                }
                else if (XmlDataBindingFlag)
                {
                    System.Xml.XmlNode TempNode;
                    foreach (object i in Items)
                    {
                        TempNode = (System.Xml.XmlNode)i;
                        Temp.Add(TempNode.Value);
                        m_tempHistoryList.Add(TempNode.Value);
                    }
                }
                else
                {
                    foreach (object i in Items)
                    {
                        if (!MemberPathFlag)
                        {
                            Type t = i.GetType();
                            var prop = t.GetProperty(DisplayMemberPath);
                            object s = prop == null ? null : prop.GetValue(i, null);
                            if (s != null)
                            {
                                BusinessObject.Add(new KeyValuePair<string, object>(s.ToString(), i));
                                Temp.Add(s.ToString());
                                m_tempHistoryList.Add(s.ToString());
                            }
                        }
                        else if (!(SelectedValuePath.Equals("")))
                        {
                            Type t = i.GetType();
                            var prop = t.GetProperty(SelectedValuePath);
                            object s = prop.GetValue(i, null);
                            if (s != null)
                            {
                                BusinessObject.Add(new KeyValuePair<string, object>(s.ToString(), i));
                                Temp.Add(s.ToString());
                                m_tempHistoryList.Add(s.ToString());
                            }
                        }
                        else
                        {
                            Temp.Add(i);
                            m_tempHistoryList.Add(i);
                        }
                    }
                }
                CustomSourceString = Temp;
            }
        }

        /// <property name="flag" value="Finished" />
        /// <summary>
        /// This method sets only the new text (doesn't start
        /// auto-complete). And moves caret to text's end.
        /// </summary>
        /// <param name="insertString">The new text.</param>
        private void SetSafeTextWithMoveCaret(string insertString)
        {
            SetSafeText(insertString);
            if (m_keyback == true)
            {
                m_textBox.CaretIndex = keybackcaret;
            }
            else
            {
                m_textBox.CaretIndex = m_textBox.Text.Length;
            }
        }

        /// <property name="flag" value="Finished" />
        /// <summary>
        /// This method moves focus to item which contains the desire
        /// text.
        /// </summary>
        /// <param name="desireText">Desired text.</param>
        /// <param name="isAutoComplete">Indicates for which list it is
        /// done, auto\-complete or history
        /// list.</param>
        private void MoveToText(string desireText, bool isAutoComplete)
        {
            m_iCollectionHistory = CollectionViewSource.GetDefaultView(m_CustomHistoryList);
            ObservableCollection<String> collection;
            ICollectionView collectionView;

            if (isAutoComplete)
            {
                collection = m_AutoCompleteList;
                collectionView = m_iCollectionAutoComplete;
            }
            else
            {
                collection = m_CustomHistoryList;
                collectionView = m_iCollectionHistory;
            }

            for (int i = 0, cnt = collection.Count; i < cnt; ++i)
            {
                if (desireText == collection[i])
                {
                    collectionView.MoveCurrentToPosition(i);
                    return;
                }
            }
        }

        /// <property name="flag" value="Finished" />
        /// <summary>
        /// This method moves focus to item after the mouse.
        /// </summary>
        /// <param name="originalSource">Source of the items which holds
        /// the content.</param>
        /// <param name="isAutoComplete">Indicates for which list it is
        /// done, auto\-complete or history
        /// list.</param>
        private void MoveMouse(Object originalSource, bool isAutoComplete)
        {
            TextBlock textBlock = originalSource as TextBlock;

            if (null != textBlock)
            {
                MoveToText(textBlock.Text, isAutoComplete);
            }
            else
            {
                Border border = originalSource as Border;

                if (null != border)
                {
                    ContentPresenter contentPresenter = border.Child as ContentPresenter;

                    if (null != contentPresenter)
                    {
                        MoveToText(contentPresenter.Content.ToString(), isAutoComplete);
                    }
                }
            }
        }

        /// <property name="flag" value="Finished" />
        /// <summary>
        /// This method corrects close popup. After changed content-text.
        /// </summary>
        private void EnterTextSelectText()
        {
            ShowSelectedItem();

            if (SelectionMode != SelectionMode.Single)
            {
                if (Keyboard.Modifiers == ModifierKeys.Control || Keyboard.Modifiers == ModifierKeys.Shift)
                {
                    IsDropDownOpen = true;
                }
                else
                {
                    IsDropDownOpen = false;
                }
            }
            else
            {
                IsDropDownOpen = false;
            }
        }

        /// <property name="flag" value="Finished" />
        /// <summary>
        /// This method clears the observe collection.
        /// </summary>
        private void Clear()
        {
            m_isTotalClear = true;
            m_AutoCompleteList.Clear();
            m_AutoCompleteObjectList.Clear();
            m_isTotalClear = false;
        }

        /// <property name="flag" value="Finished" />
        /// <summary>
        /// This method gets level for content text.
        /// </summary>
        /// <param name="nextLevel">Level for searching.</param>
        /// <param name="contentText">Text for searching.</param>
        /// <returns>
        /// Found level.
        /// </returns>
        private IAutocompleteLevel GetNextLevel(IAutocompleteLevel nextLevel, string contentText)
        {
            nextLevel.LoadItems();
            AutocompleteItemCollection items = nextLevel.Items;

            for (int i = 0, cnt = items.Count; i < cnt; ++i)
            {
                IAutocompleteLevel itemsLevel = items[i] as IAutocompleteLevel;

                if (null != itemsLevel)
                {
                    string levelText = itemsLevel.Text + itemsLevel.Splitter;

                    if (levelText.Length <= contentText.Length
                        && contentText.StartsWith(levelText, StringComparison.OrdinalIgnoreCase))
                    {
                        contentText = contentText.Substring(levelText.Length);
                        m_hashLevel = itemsLevel;

                        if (String.IsNullOrEmpty(contentText))
                        {
                            return itemsLevel;
                        }
                        else
                        {
                            return GetNextLevel(itemsLevel, contentText) ?? itemsLevel;
                        }
                    }
                }
            }
            return null;
        }

        /// <summary>
        /// Starts the add sync level.
        /// </summary>
        /// <param name="contentText">The content text.</param>
        private void StartAddSyncLevel(string contentText)
        {
            IAutocompleteLevel rootLevel = Root.GetRoot(contentText);

            if (null != rootLevel)
            {
                string rootPath = rootLevel.GetFullPath();
                int rootPathLenght = rootPath.Length;
                IAutocompleteLevel currentLevel = SearchInHash(contentText);

                if (null == currentLevel)
                {
                    currentLevel = rootLevel;
                    m_hashLevel = currentLevel;
                }
                else
                {
                    rootPathLenght = currentLevel.GetFullPath().Length;
                }

                currentLevel = GetNextLevel(currentLevel, contentText.Substring(rootPathLenght)) ?? currentLevel;
                currentLevel.LoadItems();

                SetItems(currentLevel, true);
            }
            else
            {
                Clear();
            }
        }

        /// <property name="flag" value="Finished" />
        /// <summary>
        /// This method searches the max approximate level for content
        /// text in hash.
        /// </summary>
        /// <param name="contentText">Text for searching.</param>
        /// <returns>
        /// Found level.
        /// </returns>
        private IAutocompleteLevel SearchInHash(string contentText)
        {
            int rootTextLenght = contentText.Length;

            while (null != m_hashLevel)
            {
                string hashPath = m_hashLevel.GetFullPath();

                if (hashPath.Length <= rootTextLenght
                        && contentText.StartsWith(hashPath, StringComparison.OrdinalIgnoreCase))
                {
                    break;
                }
                else
                {
                    m_hashLevel = m_hashLevel.Parent;
                }
            }
            return m_hashLevel;
        }

        /// <property name="flag" value="Finished" />
        /// <summary>
        /// This method starts to search the current level with async
        /// load items.
        /// </summary>
        /// <param name="contentText">Text for searching.</param>
        private void StartAddAsyncLevel(string contentText)
        {
            if (null != Root)
            {
                IAutocompleteLevel rootLevel = Root.GetRoot(contentText);
                IAutocompleteLevel currentLevel = null;
                if (null != rootLevel)
                {
                    string rootText = rootLevel.GetFullPath();
                    int rootTextLenght = rootText.Length;
                    currentLevel = SearchInHash(contentText);

                    if (null == currentLevel)
                    {
                        currentLevel = Root.GetRoot(contentText);
                        m_hashLevel = currentLevel;
                    }
                    else
                    {
                        rootTextLenght = currentLevel.GetFullPath().Length;
                    }

                    LoadAsyncItems(currentLevel, contentText.Substring(rootTextLenght));
                }
                else
                {
                    Clear();
                    if (IsHistory)
                        SetItems(currentLevel, true);
                }
            }
        }

        /// <property name="flag" value="Finished" />
        /// <summary>
        /// This method searches current level for content text.
        /// </summary>
        /// <param name="nextLevel">Level for searching.</param>
        /// <param name="contentText">Text for searching.</param>
        private void LoadAsyncItems(IAutocompleteLevel nextLevel, string contentText)
        {
            if (nextLevel.IsItemsLoaded)
            {
                AutocompleteItemCollection items = nextLevel.Items;

                for (int i = 0, cnt = items.Count; i < cnt; ++i)
                {
                    IAutocompleteLevel itemsLevel = items[i] as IAutocompleteLevel;

                    if (null != itemsLevel)
                    {
                        string levelText = itemsLevel.Text + itemsLevel.Splitter;

                        if (levelText.Length <= contentText.Length
                            && contentText.StartsWith(levelText, StringComparison.OrdinalIgnoreCase))
                        {
                            contentText = contentText.Substring(levelText.Length);
                            m_hashLevel = itemsLevel;

                            if (String.IsNullOrEmpty(contentText))
                            {
                                if (itemsLevel.IsItemsLoaded)
                                {
                                    SetItems(itemsLevel, true);
                                }
                                else
                                {
                                    LoadAsyncItems(itemsLevel, contentText);
                                }
                                return;
                            }
                            else
                            {
                                LoadAsyncItems(itemsLevel, contentText);
                                return;
                            }
                        }
                    }
                }

                SetItems(nextLevel, true);
                return;
            }
            else
            {
                StartProgressAnimation();
                nextLevel.ItemsLoaded += new EventHandler(OnLevelItemsAsyncLoaded);
                nextLevel.LoadItemsAsync();
            }
        }

        /// <property name="flag" value="Finished" />
        /// <summary>
        /// This method gets content text without selected part.
        /// </summary>
        /// <returns>
        /// The content text without selected part.
        /// </returns>
        private string GetContentText()
        {
            string str = "";
            string removestrback = "";
            if (m_textBox.Text.Contains(SeparatorChar.ToString()) && (SelectionMode != SelectionMode.Single))
            {
                string removestrfront = m_textBox.Text.Substring(0, m_textBox.CaretIndex);
                if (removestrfront.Contains(SeparatorChar.ToString()))
                    removestrfront = removestrfront.Substring(removestrfront.LastIndexOf(SeparatorChar) + 1);
                if (removestrfront != String.Empty)
                {
                    removestrback = m_textBox.Text.Substring(m_textBox.CaretIndex);
                    if (removestrback.Contains(SeparatorChar.ToString()))
                        removestrback = removestrback.Substring(0, removestrback.IndexOf(SeparatorChar));
                }
                int tempcaret = m_textBox.CaretIndex;
                str = removestrfront + removestrback;
                return str;
            }
            else if (IsAutoAppend && 0 < m_textBox.SelectedText.Length)
            {
                return m_textBox.Text.Remove(m_textBox.Text.Length - m_textBox.SelectedText.Length);
            }
            else
                return m_textBox.Text;
        }

        /// <property name="flag" value="Finished" />
        /// <summary>
        /// This method sets items for set level.
        /// </summary>
        /// <param name="currentLevel">Level for items.</param>
        /// <param name="isNotBackSpase">Value indicating whether it
        /// isn't BackSpace operation.</param>
        private void SetItems(IAutocompleteLevel currentLevel, bool isNotBackSpase)
        {
            StopProgressAnimation();

            if (this.Source == SourceMode.Custom)
            {
                if (IsHistory)
                {
                    var myList = m_CustomHistoryList.Cast<object>().ToList();
                    var cr = new CustomRoot(myList, EnableSorting);
                    currentLevel = cr.GetRoot("");
                }
                else
                {
                    var cr = new CustomRoot(m_tempHistoryList, EnableSorting);
                    currentLevel = cr.GetRoot("");
                }
            }

            if (null != currentLevel)
            {
                AutocompleteItemCollection collection = currentLevel.Items, tempCollection = null;
                string rootText = currentLevel.GetFullPath();

                string contentText = GetContentText();
                var contentLenght = contentText.Length;
                var searchstring = String.Empty;

                if (m_textBox != null && m_textBox.Text.Contains(SeparatorChar.ToString()) && (IsAutoAppend) && (SelectionMode != SelectionMode.Single) && !(contentText.Contains("\\")))
                {
                    searchstring = contentText.Substring(0);
                }
                else
                {
                    searchstring = contentText.Substring(rootText.Length);
                }
                    collection = tempCollection = Root.CreateFilteredGhost(currentLevel, searchstring, StringMode, StringModeIndex);
                    if (!IsFilter && IsDropDownOpen)
                    {
                        collection = Root.CreateFilteredGhost(currentLevel, "", StringMode.StartChar, 0);
                    }
                if (IsFilter || IsAutoAppend)
                {
                    if (EnableDropDown)
                    {
                        if (IsHistory && !m_TextChanged)
                            IsHistoryDropDownOpen = true;
                        else
                        {
                            if (!IsDropDownOpen)
                                IsDropDownOpen = true;
                        }

                    }
                    if (!IsBackKeyPressed && (IsAutoAppend || IsAutoCompleteItem) && 0 != tempCollection.Count
                        && !String.IsNullOrEmpty(searchstring))
                    {
                        if (!EnableDropDown || IsAutoAppend)
                        {
                            if (IsAutoAppend)
                                SetSafeTextAutoAppend(rootText + tempCollection[0].Text);
                        }

                        if (m_textBox != null && (!IsAutoCompleteItem || 1 != tempCollection.Count))
                        {
                            if (m_textBox.Text.Contains(SeparatorChar.ToString()))
                            {
                                string appendselect = m_textBox.Text.Substring(m_textBox.CaretIndex, m_textBox.Text.Length - m_textBox.CaretIndex);
                                if (appendselect.Contains(SeparatorChar.ToString()))
                                {
                                    m_textBox.Select(m_textBox.CaretIndex, appendselect.IndexOf(SeparatorChar));
                                }
                                else
                                {
                                    m_textBox.Select(m_textBox.CaretIndex, m_textBox.Text.Length);
                                }
                            }
                            else
                            {
                                m_textBox.CaretIndex = contentLenght;
                                m_textBox.Select(contentLenght, int.MaxValue);
                            }
                        }

                        else
                        {
                            if (m_textBox != null) m_textBox.CaretIndex = m_textBox.Text.Length;
                        }
                    }

                    if (IsFilter)
                    {
                        collection = tempCollection;
                    }
                }
                else
                {
                    if (EnableDropDown)
                        IsDropDownOpen = true;
                }

                var countItems = collection.Count;

                if (0 < countItems)
                {
                    m_addtoselectedobj = true;
                    m_AutoCompleteList.Clear();
                    m_AutoCompleteObjectList.Clear();
                    m_tempAutoCompleteList.Clear();
                    if (IsHistory)
                        m_FilteredCustomHistoryList.Clear();

                    if (HasDuplicateItem && EnableSorting)
                        BusinessObject.Sort(SortBusinessObjectKey);

                    for (var i = 0; i < countItems; ++i)
                    {
                        m_tempAutoCompleteList.Add(rootText + collection[i].Text);
                        if (HasDuplicateItem)
                        {
                            if (BusinessObject != null)
                            {
                                if (countItems < BusinessObject.Count)
                                {
                                    var countItem = i;
                                    foreach (KeyValuePair<string, object> bObject in BusinessObject.Where(bObject => bObject.Key == collection[countItem].Text))
                                    {
                                        m_AutoCompleteObjectList.Add(bObject.Value);
                                        m_AutoCompleteList.Add(bObject.Key);
                                        break;
                                    }
                                }
                                else
                                {
                                    m_AutoCompleteObjectList.Add(BusinessObject[i].Value);
                                    m_AutoCompleteList.Add(BusinessObject[i].Key);
                                }
                            }
                        }
                        else
                            m_AutoCompleteList.Add(rootText + collection[i].Text);
                        if (IsHistory)
                            m_FilteredCustomHistoryList.Add(rootText + collection[i].Text);
                    }

                    try
                    {
                        foreach (object obj in m_tempSelectedItems.Cast<object>().Where(obj => SelectionMode != SelectionMode.Single))
                        {
                            if (Source == SourceMode.Custom && !DisplayMemberPath.Equals(""))
                            {
                                var tempSelectedItem = obj;
                                foreach (KeyValuePair<string, object> bObject in BusinessObject.Where(bObject => tempSelectedItem.ToString() == bObject.Key))
                                {
                                    m_selectedItems.Add(bObject.Value);
                                    m_itemsAutoComplete.SelectedItems.Add((object)bObject.Key);
                                }
                            }
                            else
                            {
                                m_selectedItems.Add(obj);
                                m_itemsAutoComplete.SelectedItems.Add(obj);
                            }
                        }
                    }
                    catch (Exception)
                    {
                    }
                    if (!IsHistoryDropDownOpen && EnableDropDown)
                    {
                        IsDropDownOpen = true;
                    }
                    m_addtoselectedobj = false;
                    return;
                }
            }
            Clear();
        }

        private static int SortBusinessObjectKey(KeyValuePair<string, object> a, KeyValuePair<string, object> b)
        {
            if (a.Key is string && b.Key is string)
                return a.Key.CompareTo(b.Key);
            return 0;
        }

        /// <summary>
        /// Sets the safe text auto append.
        /// </summary>
        /// <param name="insertString">The insert string.</param>
        public void SetSafeTextAutoAppend(string insertString)
        {
            m_autoappend = true;
            if (m_textBox.Text.Contains(SeparatorChar.ToString()))
            {
                string removestrfront = m_textBox.Text.Substring(0, m_textBox.CaretIndex - 1);
                if (removestrfront.Contains(SeparatorChar.ToString()))
                {
                    removestrfront = removestrfront.Remove(removestrfront.LastIndexOf(SeparatorChar)) + SeparatorChar.ToString();
                }
                string removestrback = m_textBox.Text.Substring(m_textBox.CaretIndex);
                if (removestrback.Contains(SeparatorChar.ToString()))
                {
                    removestrback = removestrback.Remove(0, removestrback.IndexOf(SeparatorChar));
                }
                int tempcaret = m_textBox.CaretIndex;

                m_textBox.Text = removestrfront + insertString;
                m_textBox.CaretIndex = tempcaret;
            }
            else
            {
                m_textBox.Text = insertString;
            }
            m_autoappend = false;
        }

        /// <property name="flag" value="Finished" />
        /// <summary>
        /// This method starts Vista-lake progress bar animation.
        /// </summary>
        private void StartProgressAnimation()
        {
            if (null != m_vistaProgressBar)
            {
                m_vistaProgressBar.StartProgressBarAnimation();
            }
        }

        /// <property name="flag" value="Finished" />
        /// <summary>
        /// This method stops Vista-lake progress bar animation.
        /// </summary>
        private void StopProgressAnimation()
        {
            if (IsAsyncAddContent && null != m_vistaProgressBar)
            {
                m_vistaProgressBar.StopProgressBarAnimation();
            }
        }

        /// <property name="flag" value="Finished" />
        /// <summary>
        /// This method creates viewer for history list.
        /// </summary>
        private void CreateViewHistory()
        {
            if (null != m_itemsHistory)
            {
                m_itemsHistory.RemoveHandler(ListBox.KeyDownEvent, new KeyEventHandler(OnChildm_HistoryItemsAutoCompleteKeyDown));
                m_itemsHistory.MouseLeftButtonUp -= new MouseButtonEventHandler(OnChildItemsHistoryMouseLeftButtonUp);
                m_itemsHistory.SelectionChanged -= new SelectionChangedEventHandler(m_HistoryitemsAutoComplete_SelectionChanged);
            }

            if (IsHistory)
            {
                m_itemsHistory = GetTemplateChild(PART_HistoryContainer) as ListBox;
                ToggleButton toggleButton = GetTemplateChild(PART_CheckButton) as ToggleButton;

                if (null == m_itemsHistory && EnableDropDown && IsLoaded)
                {
                    throw new NotSupportedException("Set incorrect template for case with history.");
                }
                if (m_itemsHistory != null)
                {
                    m_itemsHistory.AddHandler(ListBox.KeyDownEvent, new KeyEventHandler(OnChildm_HistoryItemsAutoCompleteKeyDown), true);
                    m_itemsHistory.MouseLeftButtonUp += new MouseButtonEventHandler(OnChildItemsHistoryMouseLeftButtonUp);
                    m_itemsHistory.SelectionChanged += new SelectionChangedEventHandler(m_HistoryitemsAutoComplete_SelectionChanged);
                }
                if (null != toggleButton)
                {
                    toggleButton.Checked += new RoutedEventHandler(OnChildToggleButtonChecked);
                }
            }
            else
            {
                ToggleButton toggleButton = GetTemplateChild(PART_CheckButton) as ToggleButton;
                if (null != toggleButton)
                {
                    toggleButton.Checked += new RoutedEventHandler(OnChildToggleButtonChecked);
                }
                m_itemsHistory = null;
            }
        }

        /// <property name="flag" value="Finished" />
        /// <summary>
        /// This method works up specific key for auto-complete.
        /// </summary>
        /// <param name="key">Pressed key.</param>
        private void WorkUpAutoComplateList(Key key)
        {
            if (IsDropDownOpen)
            {
                if (Key.Down == key || Key.Tab == key)
                {
                    Keyboard.Focus(m_itemsAutoComplete);
                }
            }
            else if (IsHistoryDropDownOpen && m_itemsHistory != null && m_itemsHistory.Items.Count > 0)
            {
                if (Key.Down == key)
                {
                    Keyboard.Focus(m_itemsHistory);
                }
            }
            else if (0 != m_AutoCompleteList.Count && !IsHistoryDropDownOpen
                && (Key.Up == key || Key.Down == key))
            {
                if (!this.IsHistory && EnableDropDown)
                {
                    IsDropDownOpen = true;
                }
                else if (this.m_textBox.Text != string.Empty && EnableDropDown)
                {
                    IsDropDownOpen = true;
                }
                else
                {
                    if (EnableDropDown)
                        IsHistoryDropDownOpen = true;
                    this.canSelectHistoryItem = false;
                }
            }
            else if (m_AutoCompleteList.Count == 0 && (Key.Up == key || Key.Down == key))
            {
                if (this.IsHistory)
                {
                    IsHistoryDropDownOpen = true;
                }
            }
        }

        /// <property name="flag" value="Finished" />
        /// <summary>
        /// This method works up specific key for history.
        /// </summary>
        /// <param name="key">Pressed key.</param>
        private void WorkUpm_HistoryList(Key key)
        {
            if (IsHistoryDropDownOpen)
            {
                if (Key.Up == key && m_iCollectionHistory != null)
                {
                    m_iCollectionHistory.MoveCurrentToPrevious();

                    if (m_iCollectionHistory.IsCurrentBeforeFirst)
                    {
                        m_iCollectionHistory.MoveCurrentToLast();
                    }

                    ScrollIntoHistoryView();
                    SetTextFromHistory();
                }
                else if ((Key.Down == key || Key.Tab == key) && canSelectHistoryItem)
                {
                    if (m_iCollectionHistory != null)
                    {
                        m_iCollectionHistory.MoveCurrentToNext();

                        if (m_iCollectionHistory.IsCurrentAfterLast)
                        {
                            m_iCollectionHistory.MoveCurrentToFirst();
                        }

                        ScrollIntoHistoryView();
                    }
                }
                else if (this.IsHistoryDropDownOpen == true && Key.Down != key && Key.Up != key && !IsHistory)
                    this.IsHistoryDropDownOpen = false;

                canSelectHistoryItem = true;
            }
        }

        /// <property name="flag" value="Finished" />
        /// <summary>
        /// This method works up specific key for auto-complete in
        /// auto-append case.
        /// </summary>
        /// <param name="key">Pressed key.</param>
        /// <returns>
        /// Value indicating whether control's text was worked up.
        /// </returns>
        private bool WorkUpAutoAppend(Key key, KeyEventArgs keyArgs)
        {
            Boolean returnValue = false;
            if (IsAutoAppend)
            {
                if (Key.Return == key)
                {
                    if (0 < m_textBox.SelectionLength)
                    {
                        m_textBox.CaretIndex = m_textBox.Text.Length;
                        returnValue = true;
                    }
                }
                else if ((Key.Tab == key || Key.Right == key) && IsEndTextSelected())
                {
                    String str = m_textBox.Text;
                    int caretIndex = m_textBox.Text.Length;
                    string newText = String.Empty;

                    newText = m_textBox.Text.Substring(0, caretIndex);
                    m_tempSelectedItems.Clear();
                    if (SelectionMode != SelectionMode.Single)
                    {
                        m_selectedItems.Clear();
                        m_itemsAutoComplete.SelectedItems.Clear();
                    }
                    AddToSelectedObjects(newText);

                    m_textBox.CaretIndex = 0;
                    if (Source == SourceMode.Custom && !DisplayMemberPath.Equals(""))
                    {
                        for (int i = 0; i < m_tempSelectedItems.Count; i++)
                        {
                            m_textBox.CaretIndex += m_tempSelectedItems[i].ToString().Length;
                        }
                    }
                    else
                        for (int i = 0; i < m_selectedItems.Count; i++)
                        {
                            m_textBox.CaretIndex += m_selectedItems[i].ToString().Length;
                        }

                    keybackcaret = m_textBox.CaretIndex;
                    SetSafeTextWithMoveCaret(newText);
                    returnValue = true;
                }
            }

            if (Key.Back == key && this.m_textBox.Text != string.Empty)
            {
                IsBackKeyPressed = true;
                m_keyback = true;
                int caretIndex = m_textBox.CaretIndex;
                string newText = String.Empty;

                if (!String.IsNullOrEmpty(m_textBox.SelectedText))
                {
                    CutDownSelectedText(m_textBox.SelectedText);
                }

                else
                {
                    if (0 < caretIndex)
                    {
                        keybackcaret = m_textBox.CaretIndex - 1;
                        m_tempSelectedItems.Clear();

                        if (SelectionMode != SelectionMode.Single)
                        {
                            m_selectedItems.Clear();
                            m_itemsAutoComplete.SelectedItems.Clear();
                        }

                        if (m_textBox.CaretIndex == m_textBox.Text.Length)
                        {
                            newText = m_textBox.Text.Substring(0, caretIndex - 1);
                        }
                        else
                        {
                            newText = m_textBox.Text.Substring(0, caretIndex - 1) + m_textBox.Text.Substring(caretIndex);
                        }

                        AddToSelectedObjects(newText);
                        SetSafeTextWithMoveCaret(newText);
                    }
                }
                IsBackKeyPressed = false;
                returnValue = true;
            }

            else if (Key.Delete == key)
            {
                IsBackKeyPressed = true;
                string newText = String.Empty;
                int caretIndex = m_textBox.CaretIndex;
                m_keyback = true;
                if (!String.IsNullOrEmpty(m_textBox.SelectedText))
                {
                    CutDownSelectedText(m_textBox.SelectedText);
                }
                else
                {
                    if (caretIndex >= 0)
                    {
                        keybackcaret = m_textBox.CaretIndex;
                        m_tempSelectedItems.Clear();
                        if (SelectionMode != SelectionMode.Single)
                        {
                            m_selectedItems.Clear();
                            m_itemsAutoComplete.SelectedItems.Clear();
                        }
                        if (m_textBox.CaretIndex == m_textBox.Text.Length)
                        {
                            newText = m_textBox.Text.Substring(0, caretIndex);
                        }
                        else
                            newText = m_textBox.Text.Substring(0, caretIndex) + m_textBox.Text.Substring(caretIndex + 1);
                        AddToSelectedObjects(newText);
                        SetSafeTextWithMoveCaret(newText);
                    }
                }
                IsBackKeyPressed = false;
                returnValue = true;
            }
            else if (Key.Down == key || Key.Up == key)
            {
                HandleKey(this.m_itemsAutoComplete, keyArgs);
            }
            m_keyback = false;
            return returnValue;
        }

        /// <summary>
        /// Adds to selected objects.
        /// </summary>
        /// <param name="temptext">The temptext.</param>
        public void AddToSelectedObjects(String temptext)
        {
            m_addtoselectedobj = true;
            String checktext = String.Empty;
            for (; temptext != String.Empty; )
            {
                if (temptext.Contains(SeparatorChar.ToString()))
                {
                    checktext = temptext.Substring(0, temptext.IndexOf(SeparatorChar));
                    temptext = temptext.Remove(0, temptext.IndexOf(SeparatorChar) + 1);
                }
                else
                {
                    checktext = temptext.Substring(0, temptext.Length);
                    temptext = String.Empty;
                }
                for (int k = 0, found = 0; k < m_tempAutoCompleteList.Count; k++)
                {
                    if (String.Equals(checktext, m_tempAutoCompleteList[k], StringComparison.OrdinalIgnoreCase))
                    {
                        if (Source == SourceMode.Custom && !DisplayMemberPath.Equals(""))
                        {
                            int l = 0;
                            for (l = 0, found = 0; l < m_tempSelectedItems.Count; l++)
                            {
                                if (String.Equals(checktext, m_tempSelectedItems[l].ToString(), StringComparison.OrdinalIgnoreCase))
                                {
                                    found = 1;
                                    break;
                                }
                            }
                            if (found != 1)
                            {
                                if (!m_tempSelectedItems.Contains((object)checktext))
                                    m_tempSelectedItems.Add((object)checktext);
                                if (SelectionMode != SelectionMode.Single)
                                {
                                    m_itemsAutoComplete.SelectedItems.Add((object)checktext);
                                    for (int i = 0; i < BusinessObject.Count; i++)
                                    {
                                        if (String.Equals(checktext, BusinessObject[i].Key, StringComparison.OrdinalIgnoreCase))
                                        {
                                            m_selectedItems.Add(BusinessObject[i].Value);
                                        }
                                    }
                                    if (m_selectedItems.Count != 0)
                                    {
                                        m_selectedItem = m_selectedItems[0];
                                        SetValue(SelectedItemProperty, m_selectedItem);
                                    }
                                    else
                                    {
                                        m_selectedItem = null;
                                        SetValue(SelectedItemProperty, m_selectedItem);
                                    }
                                }
                                else
                                {
                                    for (int i = 0; i < BusinessObject.Count; i++)
                                    {
                                        if (String.Equals(checktext, BusinessObject[i].Key, StringComparison.OrdinalIgnoreCase))
                                        {
                                            m_selectedItem = BusinessObject[i].Value;
                                            SetValue(SelectedItemProperty, m_selectedItem);
                                        }
                                    }
                                }
                                break;
                            }
                        }
                        else
                        {
                            int l = 0;
                            for (l = 0, found = 0; l < m_selectedItems.Count; l++)
                            {
                                if (String.Equals(checktext, m_selectedItems[l].ToString(), StringComparison.OrdinalIgnoreCase))
                                {
                                    found = 1;
                                    break;
                                }
                            }
                            if (found != 1)
                            {
                                if (!m_tempSelectedItems.Contains((object)checktext))
                                {
                                    m_tempSelectedItems.Add((object)checktext);
                                }
                                if (SelectionMode != SelectionMode.Single)
                                {
                                    m_itemsAutoComplete.SelectedItems.Add((object)checktext);
                                    m_selectedItems.Add((object)checktext);
                                    if (m_selectedItems.Count != 0)
                                    {
                                        m_selectedItem = m_selectedItems[0];
                                        SetValue(SelectedItemProperty, m_selectedItem);
                                    }
                                    else
                                    {
                                        m_selectedItem = null;
                                        SetValue(SelectedItemProperty, m_selectedItem);
                                    }
                                }
                                else
                                {
                                    m_selectedItem = (object)checktext;
                                    m_itemsAutoComplete.SelectedItem = m_selectedItem;
                                }
                                break;
                            }
                        }
                    }
                }
            }
            m_addtoselectedobj = false;
        }

        /// <summary>
        /// Cuts down selected text.
        /// </summary>
        /// <param name="Selected_Text">The selected_ text.</param>
        private void CutDownSelectedText(String Selected_Text)
        {
            String newText = String.Empty;
            m_tempSelectedItems.Clear();
            if (SelectionMode != SelectionMode.Single)
            {
                m_selectedItems.Clear();
                m_itemsAutoComplete.SelectedItems.Clear();
            }
            int selectedlength = m_textBox.SelectionLength;
            if (m_textBox.SelectionStart + selectedlength == m_textBox.Text.Length)
            {
                if (m_textBox.SelectionStart == 0)
                {
                    keybackcaret = 0;
                    newText = "";
                }
                else
                {
                    keybackcaret = m_textBox.CaretIndex;
                    newText = m_textBox.Text.Substring(0, m_textBox.SelectionStart);
                }
            }
            else
            {
                if (m_textBox.SelectionStart == 0)
                {
                    keybackcaret = 0;
                    newText = m_textBox.Text.Substring(m_textBox.SelectionLength);
                }
                else
                {
                    keybackcaret = m_textBox.SelectionStart;
                    newText = m_textBox.Text.Substring(0, m_textBox.SelectionStart - 1) + m_textBox.Text.Substring(m_textBox.SelectionStart + selectedlength);
                }
            }
            AddToSelectedObjects(newText);
            SetSafeTextWithMoveCaret(newText);
        }

        /// <property name="flag" value="Finished" />
        /// <summary>
        /// This method binds logical and visual representation history.
        /// </summary>
        private void RelateHistory()
        {
            if (DesignerProperties.GetIsInDesignMode(this))
            {
            }
            else
            {
                if (IsHistory)
                {
                    if (m_itemsHistory != null)
                    {
                        m_itemsHistory.ItemsSource = m_FilteredCustomHistoryList;
                        m_iCollectionHistory = CollectionViewSource.GetDefaultView(m_CustomHistoryList);
                    }
                }
            }
        }

        /// <property name="flag" value="Finished" />
        /// <summary>
        /// This method indicates whether selected text is contained in
        /// the end of the control's text.
        /// </summary>
        /// <returns>
        /// True if selected text is contained in the end of the
        /// control's text; otherwise, false.
        /// </returns>
        private bool IsEndTextSelected()
        {
            return 0 != m_textBox.SelectedText.Length && m_textBox.Text.Contains(m_textBox.SelectedText);
        }

        /// <property name="flag" value="Finished" />
        /// <summary>
        /// This method sets the text selected from the history list.
        /// </summary>
        private void SetTextFromHistory()
        {
            if (null != m_iCollectionHistory.CurrentItem)
            {
                string insertString = m_iCollectionHistory.CurrentItem.ToString();
                m_textBox.Text = insertString;
                m_textBox.CaretIndex = m_textBox.Text.Length;
            }
        }

        /// <property name="flag" value="Finished" />
        /// <summary>
        /// This method corrects IsAutoAppendChanged.
        /// </summary>
        /// <param name="d">Object to which this property
        /// belongs.</param>
        /// <param name="baseValue">The property whish should be
        /// corrected.</param>
        /// <returns>
        /// Corrected value.
        /// </returns>
        private static Object CoerceIsAutoAppendChanged(DependencyObject d, Object baseValue)
        {
            AutoComplete instance = (AutoComplete)d;
            Boolean newValue = (Boolean)baseValue;

            if (!newValue && !instance.IsFilter)
            {
                return false;
            }

            return newValue;
        }

        /// <property name="flag" value="Finished" />
        /// <summary>
        /// This method corrects IsAutoCompleteItem.
        /// </summary>
        /// <param name="d">Object to which this property
        /// belongs.</param>
        /// <param name="baseValue">The property whish should be
        /// corrected.</param>
        /// <returns>
        /// Corrected value.
        /// </returns>
        private static Object CoerceIsAutoCompleteItemChanged(DependencyObject d, Object baseValue)
        {
            AutoComplete instance = (AutoComplete)d;
            Boolean newValue = (Boolean)baseValue;

            if (!newValue && !instance.IsFilter)
            {
                return false;
            }

            return newValue;
        }

        /// <property name="flag" value="Finished" />
        /// <summary>
        /// Calls OnSourceChanged method of the instance, notifies of the
        /// dependency property value changes.
        /// </summary>
        /// <param name="d">Dependency object, the change occurs on.</param>
        /// <param name="e">Property changes details, such as old value
        /// and new value.</param>
        private static void OnSourceChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            AutoComplete instance = (AutoComplete)d;
            instance.OnSourceChanged(e);
        }

        /// <property name="flag" value="Finished" />
        /// <summary>
        /// Calls OnIsDropDownOpenChanged method of the instance,
        /// notifies of the dependency property value changes.
        /// </summary>
        /// <param name="d">Dependency object, the change occurs on.</param>
        /// <param name="e">Property changes details, such as old value
        /// and new value.</param>
        private static void OnIsDropDownOpenChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            AutoComplete instance = (AutoComplete)d;
            instance.OnIsDropDownOpenChanged(e);
        }

        /// <summary>
        /// Calls OnItemsSourceChanged method of the instance, notifies
        /// </summary>
        /// <param name="oldValue"></param>
        /// <param name="newValue"></param>
       protected override void OnItemsSourceChanged(System.Collections.IEnumerable oldValue, System.Collections.IEnumerable newValue)
       {
            base.OnItemsSourceChanged(oldValue, newValue);
            if (SelectedValue != null)
            {
                if (!SelectedValuePath.Equals(""))
                {
                    if (!isinternal)
                    {
                        int iCount = 0;
                        foreach (object i in Items)
                        {
                            Type t = i.GetType();
                            var prop = t.GetProperty(SelectedValuePath);
                            object s = (prop != null) ? prop.GetValue(i, null) : null;

                            if (prop != null)
                            {
                                object pathstring = prop.GetValue(i, null);
                                if (pathstring != null && SelectedValue != null && SelectedValue.ToString() == pathstring.ToString())
                                {
                                    if (Source == SourceMode.Custom)
                                    {
                                        if (SelectedItem == null)
                                            SelectedItem = Items[iCount];
                                        else if (SelectedItem != null && SelectedItem == Items[iCount])
                                            SelectedItem = Items[iCount];
                                    }
                                }
                            }
                            iCount++;
                        }
                    }
                }

            }
        }

        /// <property name="flag" value="Finished" />
        /// <summary>
        /// Calls OnCustomSourceChanged method of the instance, notifies
        /// of the dependency property value changes.
        /// </summary>
        /// <param name="d">Dependency object, the change occurs on.</param>
        /// <param name="e">Property changes details, such as old value
        /// and new value.</param>
        private static void OnCustomSourceChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            AutoComplete instance = (AutoComplete)d;
            instance.OnCustomSourceChanged(e);
        }

        /// <property name="flag" value="Finished" />
        /// <summary>
        /// Calls OnIsAutoAppendChanged method of the instance, notifies
        /// of the dependency property value changes.
        /// </summary>
        /// <param name="d">Dependency object, the change occurs on.</param>
        /// <param name="e">Property changes details, such as old value
        /// and new value.</param>
        private static void OnIsAutoAppendChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            AutoComplete instance = (AutoComplete)d;
            instance.OnIsAutoAppendChanged(e);
        }

        /// <property name="flag" value="Finished" />
        /// <summary>
        /// Calls OnMaxDropHeightChanged method of the instance, notifies
        /// of the dependency property value changes.
        /// </summary>
        /// <param name="d">Dependency object, the change occurs on.</param>
        /// <param name="e">Property changes details, such as old value
        /// and new value.</param>
        private static void OnMaxDropHeightChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            AutoComplete instance = (AutoComplete)d;
            instance.OnMaxDropHeightChanged(e);
        }

        /// <property name="flag" value="Finished" />
        /// <summary>
        /// Calls OnIsFilterChanged method of the instance, notifies of
        /// the dependency property value changes.
        /// </summary>
        /// <param name="d">Dependency object, the change occurs on.</param>
        /// <param name="e">Property change details, such as old value
        /// and new value.</param>
        private static void OnIsFilterChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            AutoComplete instance = (AutoComplete)d;
            instance.OnIsFilterChanged(e);
        }

        /// <property name="flag" value="Finished" />
        /// <summary>
        /// Calls OnIsAutoCompleteItemChanged method of the instance,
        /// notifies of the dependency property value changes.
        /// </summary>
        /// <param name="d">Dependency object, the change occurs on.</param>
        /// <param name="e">Property change details, such as old value
        /// and new value.</param>
        private static void OnIsAutoCompleteItemChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            AutoComplete instance = (AutoComplete)d;
            instance.OnIsAutoCompleteItemChanged(e);
        }

        /// <property name="flag" value="Finished" />
        /// <summary>
        /// Calls OnIsAsyncAddContentChanged method of the instance,
        /// notifies of the dependency property value changes.
        /// </summary>
        /// <param name="d">Dependency object, the change occurs on.</param>
        /// <param name="e">Property changes details, such as old value
        /// and new value.</param>
        private static void OnIsAsyncAddContentChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            AutoComplete instance = (AutoComplete)d;
            instance.OnIsAsyncAddContentChanged(e);
        }

        /// <property name="flag" value="Finished" />
        /// <summary>
        /// Calls OnTextChanged method of the instance, notifies of the
        /// dependency property value changes.
        /// </summary>
        /// <param name="d">Dependency object, the change occurs on.</param>
        /// <param name="e">Property changes details, such as old value
        /// and new value.</param>
        private static void OnTextChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            AutoComplete instance = (AutoComplete)d;
            instance.OnTextChanged(e);
        }

        /// <property name="flag" value="Finished" />
        /// <summary>
        /// Calls OnIsHistoryChanged method of the instance, notifies of
        /// the dependency property value changes.
        /// </summary>
        /// <param name="d">Dependency object, the change occurs on.</param>
        /// <param name="e">Property change details, such as old value
        /// and new value.</param>
        private static void OnIsHistoryChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            AutoComplete instance = (AutoComplete)d;
            instance.OnIsHistoryChanged(e);
        }

        /// <property name="flag" value="Finished" />
        /// <summary>
        /// Calls OnIsHistoryDropDownOpenChanged method of the instance,
        /// notifies of the dependency property value changes.
        /// </summary>
        /// <param name="d">Dependency object, the change occurs on.</param>
        /// <param name="e">Property changes details, such as old value
        /// and new value.</param>
        private static void OnIsHistoryDropDownOpenChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            AutoComplete instance = (AutoComplete)d;
            instance.OnIsHistoryDropDownOpenChanged(e);
        }

        /// <property name="flag" value="Finished" />
        /// <summary>
        /// Calls Onm_HistoryListHeightChanged method of the instance,
        /// notifies of the dependency property value changes.
        /// </summary>
        /// <param name="d">Dependency object, the change occurs on.</param>
        /// <param name="e">Property change details, such as old value
        /// and new value.</param>
        private static void OnHistoryListHeightChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            AutoComplete instance = (AutoComplete)d;
            instance.Onm_HistoryListHeightChanged(e);
        }

        /// <property name="flag" value="Finished" />
        /// <summary>
        /// Calls OnFileStorageNameChanged method of the instance,
        /// notifies of the dependency property value changes.
        /// </summary>
        /// <param name="d">Dependency object, the change occurs on.</param>
        /// <param name="e">Property changes details, such as old value
        /// and new value.</param>
        private static void OnFileStorageNameChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            AutoComplete instance = (AutoComplete)d;
            instance.m_storeFileName = instance.FileStorageName + FileExt;
            instance.OnFileStorageNameChanged(e);
        }

        /// <summary>
        /// chekcks whether it is internal change
        /// </summary>
        private bool isinternal = false;

        /// <summary>
        /// Called when [selected item changed].
        /// </summary>
        /// <param name="d">The d.</param>
        /// <param name="e">The <see cref="System.Windows.DependencyPropertyChangedEventArgs"/> instance containing the event data.</param>
        private static void OnSelectedItemChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            OnSelected(d, e.NewValue);
        }

        public static void OnSelected(object d, object e)
        {
            AutoComplete instance = d as AutoComplete;
            if (instance != null && (instance.Source == SourceMode.Custom && instance.IsInternalchange))
            {
                int index = instance.Items.IndexOf(e);
                if (instance.BusinessObject != null && instance.BusinessObject.Count > index)
                {
                    if (index >= 0 && instance.Text != null && instance.Text.Length == instance.BusinessObject[index].Key.Length)
                    {
                        instance.Text = instance.BusinessObject[index].Key;
                        instance.SelectedIndex = instance.Items.IndexOf(e);
                        if (instance.m_itemsAutoComplete != null)
                            instance.m_itemsAutoComplete.SelectedIndex = instance.CustomSourceString.IndexOf(instance.BusinessObject[index].Key);
                    }
                    else
                    {
                        if (index >= 0)
                        {
                            if (instance.SelectedItem != null && instance.DisplayMemberPath != null)
                            {
                                Type t = instance.SelectedItem.GetType();
                                var prop = t.GetProperty(instance.DisplayMemberPath);
                                object s = prop == null ? null : prop.GetValue(instance.SelectedItem, null);
                                if (s != null)
                                {
                                    instance.Text = s.ToString();
                                }
                            }
                            else
                            {
                                instance.Text = instance.BusinessObject[index].Key;
                                instance.SelectedIndex = instance.Items.IndexOf(e);
                                if (instance.m_itemsAutoComplete != null)
                                    instance.m_itemsAutoComplete.SelectedIndex = instance.SelectedIndex;
                            }
                        }
                    }
                }
                else
                {
                    if (instance.CustomSourceString != null)
                        instance.Text = instance.CustomSourceString[instance.Items.IndexOf(e)].ToString();
                    instance.SelectedIndex = instance.Items.IndexOf(e);
                    if (instance.m_itemsAutoComplete != null)
                        instance.m_itemsAutoComplete.SelectedIndex = instance.SelectedIndex;
                }

                if (instance.SelectedItem != null)
                    instance.SelectedValue = instance.SelectedItem;

                if (instance.SelectedItem == null && instance.SelectionMode == SelectionMode.Multiple)
                {
                    instance.SelectedItems.Clear();
                    instance.Text = string.Empty;
                }
            }

            if (instance != null && !instance.DisplayMemberPath.Equals(""))
            {
                if (!instance.isinternal)
                {
                    object source = (object)e;

                    if (instance.Items.Contains(source))
                    {
                        instance.isIndexChangedInDesign = false;
                        instance.SelectedIndex = instance.Items.IndexOf(source);
                        instance.isinternal = true;
                        instance.SelectedItem = source;
                        instance.isinternal = false;

                        Type t = source.GetType();
                        if (instance.SelectedValuePath != null)
                        {
                            var prop = t.GetProperty(instance.SelectedValuePath);
                            if (prop != null)
                            {
                                object pathstring = prop.GetValue(source, null);

                                if (instance.Source == SourceMode.Custom)
                                {
                                    instance.SelectedValue = pathstring;
                                }
                            }
                        }
                    }
                }
            }
            else if (instance != null && (instance.SelectedValuePath != null && (!instance.SelectedValuePath.Equals(""))))
            {
                if (instance.DisplayMemberPath.Equals(""))
                {
                    if (!instance.isinternal)
                    {
                        object source = (object)e;
                        foreach (object i in instance.Items)
                        {
                            Type t = i.GetType();
                            var prop = t.GetProperty(instance.SelectedValuePath);
                            object s = prop.GetValue(i, null);

                            if (prop != null)
                            {
                                object pathstring = prop.GetValue(i, null);
                                if (source.ToString() == pathstring.ToString())
                                {
                                    if (instance.Source == SourceMode.Custom)
                                    {
                                        instance.SelectedValue = pathstring;
                                    }
                                }
                            }
                        }
                    }
                }
            }
            else
            {
                if (instance != null && !instance.isinternal)
                {
                    object source = (object)e;
                    if (instance.Items.Contains(source))
                    {
                        instance.isIndexChangedInDesign = false;
                        instance.SelectedIndex = instance.Items.IndexOf(source);
                        instance.isinternal = true;
                        instance.SelectedItem = source;
                        instance.isinternal = false;
                        if (instance.m_itemsAutoComplete != null)
                            if (instance.m_itemsAutoComplete.SelectedValue != null)
                                instance.SelectedValue = instance.m_itemsAutoComplete.SelectedValue;
                        if (instance.Source == SourceMode.Custom)
                        {
                            if (instance.SelectedItem != null)
                                instance.SelectedValue = instance.ReturnSelectedValueFromObj(instance.SelectedItem);
                        }
                    }
                }
            }
        }

        /// <summary>
        /// Called when [selection mode changed].
        /// </summary>
        /// <param name="d">The d.</param>
        /// <param name="e">The <see cref="System.Windows.DependencyPropertyChangedEventArgs"/> instance containing the event data.</param>
        private static void OnSelectionModeChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            AutoComplete instance = (AutoComplete)d;
            instance.OnSelectionModeChanged(e);
        }

        #endregion Implemenation

        #region Dependency property

        /// <summary>
        /// This property indicates the selected item
        /// </summary>
        public static readonly DependencyProperty SelectedItemProperty =
            DependencyProperty.Register("SelectedItem", typeof(object), typeof(AutoComplete), new FrameworkPropertyMetadata(null, FrameworkPropertyMetadataOptions.BindsTwoWayByDefault, new PropertyChangedCallback(OnSelectedItemChanged)));

        /// <summary>
        /// This Property indicates the string mode for search in drop-down hints
        /// </summary>
        public static readonly DependencyProperty StringModeProperty =
            DependencyProperty.Register("StringMode", typeof(StringMode), typeof(AutoComplete), new FrameworkPropertyMetadata(StringMode.StartChar));

        /// <summary>
        /// This Property indicates the string mode index position to search from
        /// </summary>
        public static readonly DependencyProperty StringModeIndexProperty =
           DependencyProperty.Register("StringModeIndex", typeof(int), typeof(AutoComplete), new FrameworkPropertyMetadata(0));

        /// <summary>
        /// This property indicates the position of the popup.
        /// </summary>
        public static readonly DependencyProperty PopupPlacementProperty =
            DependencyProperty.Register("PopupPlacement", typeof(PopupPlacement), typeof(AutoComplete), new FrameworkPropertyMetadata(PopupPlacement.Bottom));

        /// <property name="flag" value="Finished" />
        /// <summary>
        /// This property indicates the state of auto complete mode.
        /// </summary>
        public static readonly DependencyProperty SourceProperty =
            DependencyProperty.Register("Source", typeof(SourceMode), typeof(AutoComplete), new FrameworkPropertyMetadata(SourceMode.Custom, new PropertyChangedCallback(OnSourceChanged)));

        /// <property name="flag" value="Finished" />
        /// <summary>
        /// This property indicates state of the popup window.
        /// </summary>
        public static readonly DependencyProperty IsDropDownOpenProperty =
            DependencyProperty.Register("IsDropDownOpen", typeof(bool), typeof(AutoComplete), new FrameworkPropertyMetadata(false, new PropertyChangedCallback(OnIsDropDownOpenChanged)));

        /// <property name="flag" value="Finished" />
        /// <summary>
        /// This property indicates custom content for auto-complete.
        /// </summary>
        public static readonly DependencyProperty CustomSourceProperty =
            DependencyProperty.Register("CustomSource", typeof(System.Collections.IEnumerable), typeof(AutoComplete), new FrameworkPropertyMetadata(null, new PropertyChangedCallback(OnCustomSourceChanged)));

        /// <property name="flag" value="Finished" />
        /// <summary>
        /// This property indicates type of auto complete
        /// </summary>
        public static readonly DependencyProperty IsAutoAppendProperty =
            DependencyProperty.Register("IsAutoAppend", typeof(bool), typeof(AutoComplete), new FrameworkPropertyMetadata(false, new PropertyChangedCallback(OnIsAutoAppendChanged), new CoerceValueCallback(CoerceIsAutoAppendChanged)));

        /// <property name="flag" value="Finished" />
        /// <summary>
        /// This property indicates the max height of the popup window.
        /// </summary>
        public static readonly DependencyProperty MaxDropHeightProperty =
            DependencyProperty.Register("MaxDropHeight", typeof(double), typeof(AutoComplete), new FrameworkPropertyMetadata(150d, new PropertyChangedCallback(OnMaxDropHeightChanged)));

        /// <property name="flag" value="Finished" />
        /// <summary>
        /// This property indicates whether to use filter.
        /// </summary>
        public static readonly DependencyProperty IsFilterProperty =
            DependencyProperty.Register("IsFilter", typeof(bool), typeof(AutoComplete), new FrameworkPropertyMetadata(true, new PropertyChangedCallback(OnIsFilterChanged)));

        /// <property name="flag" value="Finished" />
        /// <summary>
        /// This property indicates whether auto completed content text
        /// is the only one correct variant.
        /// </summary>
        public static readonly DependencyProperty IsAutoCompleteItemProperty =
            DependencyProperty.Register("IsAutoCompleteItem", typeof(bool), typeof(AutoComplete), new FrameworkPropertyMetadata(false, new PropertyChangedCallback(OnIsAutoCompleteItemChanged), new CoerceValueCallback(CoerceIsAutoCompleteItemChanged)));

        /// <property name="flag" value="Finished" />
        /// <summary>
        /// This property indicates mode and adds content items.
        /// </summary>
        public static readonly DependencyProperty IsAsyncAddContentProperty =
            DependencyProperty.Register("IsAsyncAddContent", typeof(bool), typeof(AutoComplete), new FrameworkPropertyMetadata(true, new PropertyChangedCallback(OnIsAsyncAddContentChanged)));

        /// <property name="flag" value="Finished" />
        /// <summary>
        /// This property contains control's text content.
        /// </summary>
        public static readonly DependencyProperty TextProperty =
            DependencyProperty.Register("Text", typeof(string), typeof(AutoComplete), new FrameworkPropertyMetadata(String.Empty, FrameworkPropertyMetadataOptions.BindsTwoWayByDefault, new PropertyChangedCallback(OnTextChanged)));

        /// <property name="flag" value="Finished" />
        /// <summary>
        /// This property indicates whether to use history.
        /// </summary>
        public static readonly DependencyProperty IsHistoryProperty =
            DependencyProperty.Register("IsHistory", typeof(bool), typeof(AutoComplete), new FrameworkPropertyMetadata(false, new PropertyChangedCallback(OnIsHistoryChanged)));

        /// <property name="flag" value="Finished" />
        /// <summary>
        /// This property indicates state of the history popup window.
        /// </summary>
        public static readonly DependencyProperty IsHistoryDropDownOpenProperty =
            DependencyProperty.Register("IsHistoryDropDownOpen", typeof(bool), typeof(AutoComplete), new FrameworkPropertyMetadata(false, new PropertyChangedCallback(OnIsHistoryDropDownOpenChanged)));

        /// <property name="flag" value="Finished" />
        /// <summary>
        /// This property defines the height of the history list.
        /// </summary>
        public static readonly DependencyProperty HistoryListHeightProperty =
            DependencyProperty.Register("HistoryListHeight", typeof(double), typeof(AutoComplete), new FrameworkPropertyMetadata(150d, new PropertyChangedCallback(OnHistoryListHeightChanged)));

        /// <property name="flag" value="Finished" />
        /// <summary>
        /// File name for saving history in isolate storage.
        /// </summary>
        public static readonly DependencyProperty FileStorageNameProperty =
            DependencyProperty.Register("FileStorageName", typeof(string), typeof(AutoComplete), new FrameworkPropertyMetadata(FileExt, new PropertyChangedCallback(OnFileStorageNameChanged)));

        /// <summary>
        /// This property indicates the Selection mode
        /// </summary>
        public static readonly DependencyProperty SelectionModeProperty =
           DependencyProperty.Register("SelectionMode", typeof(SelectionMode), typeof(AutoComplete), new FrameworkPropertyMetadata(SelectionMode.Single, new PropertyChangedCallback(OnSelectionModeChanged)));

        /// <summary>
        /// This property indicates the seperator character
        /// </summary>
        public static readonly DependencyProperty SeparatorCharProperty =
           DependencyProperty.Register("SeparatorChar", typeof(char), typeof(AutoComplete), new FrameworkPropertyMetadata(';'));

        #endregion Dependency property

        #region History Related methods

        /// <summary>
        /// Gets the full object history.
        /// </summary>
        /// <returns></returns>
        private List<KeyValuePair<String, Object>> GetFullObjectHistory()
        {
            IsolatedStorageFile isoStorage =
                IsolatedStorageFile.GetStore(IsolatedStorageScope.User | IsolatedStorageScope.Assembly, null, null);
            List<KeyValuePair<String, Object>> fullHistory = null;

            if (0 < isoStorage.GetFileNames(m_storeObjectFileName).Length)
            {
                Stream stream = new IsolatedStorageFileStream(m_storeObjectFileName, FileMode.OpenOrCreate, isoStorage);

                try
                {
                    IFormatter formatter = new BinaryFormatter();
                    fullHistory = (List<KeyValuePair<String, Object>>)formatter.Deserialize(stream);
                }
                //SU I78477
                //catch (SerializationException e)
                catch (SerializationException)
                //EU I78477
                {
                }

                finally
                {
                    stream.Close();
                }
            }
            return fullHistory;
        }

        /// <summary>
        /// Saves the history.
        /// </summary>
        public void SaveHistory()
        {
            if (Source != SourceMode.Custom)
            {
                List<KeyValuePair<SourceMode, String>> fullHistory = GetFullHistory()
                    ?? new List<KeyValuePair<SourceMode, String>>();

                List<String> oldHistory = new List<String>();

                for (int i = 0, cnt = fullHistory.Count; i < cnt; ++i)
                {
                    if (Source == fullHistory[i].Key)
                    {
                        oldHistory.Add(fullHistory[i].Value);
                    }
                }

                for (int i = 0, cnt = m_CustomHistoryList.Count; i < cnt; ++i)
                {
                    if (!oldHistory.Contains(m_CustomHistoryList[i]))
                    {
                        fullHistory.Add(new KeyValuePair<SourceMode, String>(Source, m_CustomHistoryList[i]));
                    }
                }

                SaveHistoryList(fullHistory);
            }
            else
            {
                List<String> temp = new List<String>();
                List<KeyValuePair<String, Object>> fullHistory = GetFullObjectHistory();
                if (fullHistory == null)
                    fullHistory = new List<KeyValuePair<string, object>>();
                else
                {
                    for (int i = 0, cnt = fullHistory.Count; i < cnt; ++i)
                    {
                        temp.Add(fullHistory[i].Key);
                    }
                }

                if (ObjectHistory == null)
                    ObjectHistory = new List<KeyValuePair<String, Object>>();
                else
                {
                    for (int i = 0, cnt = ObjectHistory.Count; i < cnt; ++i)
                    {
                        if (!temp.Contains(ObjectHistory[i].Key))
                        {
                            fullHistory.Add(ObjectHistory[i]);
                        }
                    }
                }
                SaveObjectHistoryList(fullHistory);
            }
        }

        /// <property name="flag" value="Finished" />
        ///  <para/>
        /// <summary>
        /// This method loads history.
        /// </summary>
        public void LoadHistory()
        {
            m_CustomHistoryList.Clear();

            if (Source != SourceMode.Custom)
            {
                List<KeyValuePair<SourceMode, String>> fullHistory = GetFullHistory();
                if (null != fullHistory)
                {
                    for (int i = 0, count = fullHistory.Count; i < count; ++i)
                    {
                        if (Source == fullHistory[i].Key && (!(m_CustomHistoryList.Contains(fullHistory[i].Value))) && (this.m_AutoCompleteList.Contains(fullHistory[i].Value)))
                        {
                            m_CustomHistoryList.Add(fullHistory[i].Value);
                        }
                    }
                }
            }
            else
            {
                List<KeyValuePair<String, Object>> fullHistory = GetFullObjectHistory();
                if (null != fullHistory)
                {
                    for (int i = 0, count = fullHistory.Count; i < count; ++i)
                    {
                        m_CustomHistoryList.Add(fullHistory[i].Key);
                        if (ObjectHistory == null)
                            ObjectHistory = new List<KeyValuePair<string, object>>();
                        ObjectHistory.Add(fullHistory[i]);
                    }
                    LoadHistoryFlag = true;
                }
            }
        }

        /// <property name="flag" value="Finished" />
        /// <summary>
        /// This method saves history list.
        /// </summary>
        /// <param name="fullHistory">The list which should be saved.</param>
        private void SaveHistoryList(List<KeyValuePair<SourceMode, String>> fullHistory)
        {
            IsolatedStorageFile isolatedStorageFile =
                IsolatedStorageFile.GetStore(IsolatedStorageScope.User | IsolatedStorageScope.Assembly, null, null);
            Stream stream = new IsolatedStorageFileStream(StoreFileName, FileMode.Create, isolatedStorageFile);
            try
            {
                IFormatter formatter = new BinaryFormatter();
                formatter.Serialize(stream, fullHistory);
            }
            finally
            {
                stream.Close();
            }
        }

        /// <property name="flag" value="Finished" />
        ///  <para/>
        /// <summary>
        /// This method clears history for this source.
        /// </summary>
        public void ClearSourceHistory()
        {
            m_CustomHistoryList.Clear();

            if (Source != SourceMode.Custom)
            {
                List<KeyValuePair<SourceMode, String>> fullHistory = GetFullHistory()
                    ?? new List<KeyValuePair<SourceMode, String>>();
                List<KeyValuePair<SourceMode, String>> newHistory = new List<KeyValuePair<SourceMode, String>>();

                for (int i = 0, cnt = fullHistory.Count; i < cnt; ++i)
                {
                    if (Source != fullHistory[i].Key)
                    {
                        newHistory.Add(fullHistory[i]);
                    }
                }

                SaveHistoryList(newHistory);
            }
            else
            {
                ObjectHistory.Clear();
                SaveObjectHistoryList(new List<KeyValuePair<string, object>>());
            }
        }

        /// <property name="flag" value="Finished" />
        /// <summary>
        /// This method gets all modes history.
        /// </summary>
        /// <returns>
        /// History for all modes.
        /// </returns>
        private List<KeyValuePair<SourceMode, String>> GetFullHistory()
        {
            IsolatedStorageFile isoStorage =
                IsolatedStorageFile.GetStore(IsolatedStorageScope.User | IsolatedStorageScope.Assembly, null, null);

            List<KeyValuePair<SourceMode, String>> fullHistory = null;
            //SU I78477
            //List<KeyValuePair<SourceMode, String>> tempHistory = null;
            //EU I78477

            if (0 < isoStorage.GetFileNames(StoreFileName).Length)
            {
                Stream stream = new IsolatedStorageFileStream(StoreFileName, FileMode.OpenOrCreate, isoStorage);

                try
                {
                    IFormatter formatter = new BinaryFormatter();
                    fullHistory = (List<KeyValuePair<SourceMode, String>>)formatter.Deserialize(stream);
                }
                finally
                {
                    stream.Close();
                }
            }
            return fullHistory;
        }

        /// <property name="flag" value="Finished" />
        ///  <para/>
        /// <summary>
        /// This method clears all history.
        /// </summary>
        public void ClearAllHistory()
        {
            m_CustomHistoryList.Clear();
            ObjectHistory.Clear();
            SaveHistoryList(new List<KeyValuePair<SourceMode, String>>());
            SaveObjectHistoryList(new List<KeyValuePair<string, object>>());
        }

        /// <property name="flag" value="Finished" />
        ///  <para/>
        /// <summary>
        /// This method adds input text to the history list.
        /// </summary>
        /// <param name="inputText">Input text.</param>
        public void AddHistory(string inputText)
        {
            if (IsHistory)
            {
                ObjectHistory.Add(new KeyValuePair<string, object>(inputText, inputText));
                if (!m_HistoryList.Contains(inputText))
                {
                    m_HistoryList.Add(inputText);
                }

                if (!m_CustomHistoryList.Contains(inputText))
                {
                    if (!this.m_AutoCompleteList.Contains(inputText))
                    {
                        m_CustomHistoryList.Add(inputText);
                        SaveHistory();
                        LoadHistory();
                    }
                }
            }
        }

        /// <summary>
        /// Saves the object history list.
        /// </summary>
        /// <param name="fullHistory">The full history.</param>
        private void SaveObjectHistoryList(List<KeyValuePair<String, Object>> fullHistory)
        {
            //string StoreFileName = AppDomain.CurrentDomain.SetupInformation.ApplicationName + ".txt";
            IsolatedStorageFile isolatedStorageFile =
                IsolatedStorageFile.GetStore(IsolatedStorageScope.User | IsolatedStorageScope.Assembly, null, null);
            Stream stream = new IsolatedStorageFileStream(m_storeObjectFileName, FileMode.Create, isolatedStorageFile);
            bool SerializeFlag = true;
            object obj = new object();

            if (fullHistory.Count > 0)
            {
                obj = fullHistory[0].Value;
                SerializeFlag = obj.GetType().IsSerializable;
            }

            if (SerializeFlag)
            {
                try
                {
                    IFormatter formatter = new BinaryFormatter();
                    formatter.Serialize(stream, fullHistory);
                }
                finally
                {
                    stream.Close();
                }
            }
            else
            {
                Debug.Fail("Class " + obj.GetType().Name + " is not marked as serializable");
            }
        }

        /// <summary>
        /// This method inputs object to history list
        /// </summary>
        /// <param name="input"></param>
        public void AddHistory(object input)
        {
            if (Source == SourceMode.Custom)
            {
                if (input != null)
                {
                    ObjectHistory.Add(new KeyValuePair<string, object>(ReturnStringFromObj(input), input));
                    if (this.Items.Contains(input) && !(m_CustomHistoryList.Contains(ReturnStringFromObj(input))))
                        m_CustomHistoryList.Add(ReturnStringFromObj(input));
                    m_HistoryList.Add(ReturnStringFromObj(input));
                    LoadHistoryFlag = true;
                }
            }
            else
            {
                Debug.Fail("This Function is Supported Only in Custom Source Mode");
            }
        }

        /// <summary>
        /// This method returns string format corresponds to the object
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        private string ReturnStringFromObj(object input)
        {
            if (input is string)
            {
                return input.ToString();
            }
            else if (input is System.Xml.XmlNode)
            {
                return ((System.Xml.XmlNode)input).Value;
            }
            else
            {
                if (!DisplayMemberPath.Equals(""))
                {
                    Type t = input.GetType();
                    var prop = t.GetProperty(DisplayMemberPath);
                    object s = prop.GetValue(input, null);
                    return s.ToString();
                }
                else
                {
                    return input.ToString();
                }
            }
        }

        #endregion History Related methods
    }
}