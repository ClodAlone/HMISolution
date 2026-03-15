#region Copyright
// Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Any infringement will be prosecuted under
// applicable laws. 
#endregion

namespace Syncfusion.Windows.Tools.Controls
{
    using System;
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using System.Collections.Specialized;
    using System.ComponentModel;
    using System.Diagnostics;
    using System.IO;
    using System.IO.IsolatedStorage;
    using System.Runtime.Serialization;
    using System.Security;
    using System.Text;
    using System.Threading;
    using System.Windows;
    using System.Windows.Controls;
    using System.Windows.Controls.Primitives;
    using System.Windows.Data;
    using System.Windows.Input;
    using System.Windows.Interop;
    using System.Windows.Media;
    using System.Windows.Threading;
    using Microsoft.Win32;
    using Syncfusion.Silverlight.Shared;
    using System.Runtime.InteropServices;
    using Syncfusion.Windows.Shared;

    /// <summary>
    /// Main class of the control.
    /// </summary>
    [Syncfusion.Windows.Shared.SkinType(SkinVisualStyle = Syncfusion.Windows.Shared.VisualStyle.Blend,
        Type = typeof(AutoComplete), XamlResource = "/Syncfusion.Theming.Blend;component/AutoComplete.xaml")]
    [Syncfusion.Windows.Shared.SkinType(SkinVisualStyle = Syncfusion.Windows.Shared.VisualStyle.Office2007Blue,
        Type = typeof(AutoComplete), XamlResource = "/Syncfusion.Theming.Office2007Blue;component/AutoComplete.xaml")]
    [Syncfusion.Windows.Shared.SkinType(SkinVisualStyle = Syncfusion.Windows.Shared.VisualStyle.Office2007Black,
        Type = typeof(AutoComplete), XamlResource = "/Syncfusion.Theming.Office2007Black;component/AutoComplete.xaml")]
    [Syncfusion.Windows.Shared.SkinType(SkinVisualStyle = Syncfusion.Windows.Shared.VisualStyle.Office2007Silver,
        Type = typeof(AutoComplete), XamlResource = "/Syncfusion.Theming.Office2007Silver;component/AutoComplete.xaml")]
    [Syncfusion.Windows.Shared.SkinType(SkinVisualStyle = Syncfusion.Windows.Shared.VisualStyle.Default,
        Type = typeof(AutoComplete), XamlResource = "/Syncfusion.Theming.Default;component/AutoComplete.xaml")]
    [Syncfusion.Windows.Shared.SkinType(SkinVisualStyle = Syncfusion.Windows.Shared.VisualStyle.Office2003,
        Type = typeof(AutoComplete), XamlResource = "/Syncfusion.Theming.Office2003;component/AutoComplete.xaml")]
    [Syncfusion.Windows.Shared.SkinType(SkinVisualStyle = Syncfusion.Windows.Shared.VisualStyle.Office2010Blue,
        Type = typeof(AutoComplete), XamlResource = "/Syncfusion.Theming.Office2010Blue;component/AutoComplete.xaml")]
    [Syncfusion.Windows.Shared.SkinType(SkinVisualStyle = Syncfusion.Windows.Shared.VisualStyle.Office2010Black,
        Type = typeof(AutoComplete), XamlResource = "/Syncfusion.Theming.Office2010Black;component/AutoComplete.xaml")]
    [Syncfusion.Windows.Shared.SkinType(SkinVisualStyle = Syncfusion.Windows.Shared.VisualStyle.Office2010Silver,
        Type = typeof(AutoComplete), XamlResource = "/Syncfusion.Theming.Office2010Silver;component/AutoComplete.xaml")]
    [Syncfusion.Windows.Shared.SkinType(SkinVisualStyle = Syncfusion.Windows.Shared.VisualStyle.Windows7,
      Type = typeof(AutoComplete), XamlResource = "/Syncfusion.Theming.Windows7;component/AutoComplete.xaml")]
    [Syncfusion.Windows.Shared.SkinType(SkinVisualStyle = Syncfusion.Windows.Shared.VisualStyle.Metro,
     Type = typeof(AutoComplete), XamlResource = "/Syncfusion.Theming.Metro;component/AutoComplete.xaml")]
    [Syncfusion.Windows.Shared.SkinType(SkinVisualStyle = Syncfusion.Windows.Shared.VisualStyle.VS2010,
 Type = typeof(AutoComplete), XamlResource = "/Syncfusion.Theming.VS2010;component/AutoComplete.xaml")]
    [Syncfusion.Windows.Shared.SkinType(SkinVisualStyle = Syncfusion.Windows.Shared.VisualStyle.Transparent,
Type = typeof(AutoComplete), XamlResource = "/Syncfusion.Theming.Transparent;component/AutoComplete.xaml")]
    public class AutoComplete : ItemsControl
    {
        #region - Dependency property
        /// <summary>
        /// TO set the default value of the Selected Index property and property change call back
        /// </summary>
        public static readonly DependencyProperty SelectedIndexProperty =
            DependencyProperty.Register("SelectedIndex", typeof(int), typeof(AutoComplete), new PropertyMetadata(-1, OnSelectedIndexChanged));

        /// <summary>
        /// To set the default value of the Selected Value property and property change call back
        /// </summary>
        public static readonly DependencyProperty SelectedValueProperty =
            DependencyProperty.Register("SelectedValue  ", typeof(object), typeof(AutoComplete), new PropertyMetadata(null));

        /// <summary>
        /// To the default value of selected value property and to property change call back
        /// </summary>
        public static readonly DependencyProperty SelectedValuePathProperty =
            DependencyProperty.Register("SelectedValuePath ", typeof(string), typeof(AutoComplete), new PropertyMetadata(string.Empty, OnSelectedValuePathChanged));

        /// <summary>
        /// This property indicates the selected item
        /// </summary>
        public static readonly DependencyProperty SelectedItemProperty =
            DependencyProperty.Register("SelectedItem", typeof(object), typeof(AutoComplete), new PropertyMetadata(null, new PropertyChangedCallback(OnSelectedItemChanged)));

        /// <summary>
        /// 
        /// </summary>
        public new static readonly DependencyProperty DisplayMemberPathProperty =
            DependencyProperty.Register("DisplayMemberPath", typeof(String), typeof(AutoComplete), new PropertyMetadata(String.Empty, new PropertyChangedCallback(OnDisplayMemberPathChanged)));

        /// <summary>
        /// This property indicates state of the popup window. 
        /// </summary>
        public static readonly DependencyProperty IsDropDownOpenProperty =
            DependencyProperty.Register("IsDropDownOpen", typeof(bool), typeof(AutoComplete), new PropertyMetadata(false, new PropertyChangedCallback(OnIsDropDownOpenChanged)));

        /// <summary>
        /// This property indicates custom content for auto-complete. 
        /// </summary>
        public static readonly DependencyProperty CustomSourceProperty =
            DependencyProperty.Register("CustomSource", typeof(System.Collections.IEnumerable), typeof(AutoComplete), new PropertyMetadata(null, new PropertyChangedCallback(OnCustomSourceChanged)));

        /// <summary>
        /// This property indicates type of autocomplete. 
        /// </summary>
        public static readonly DependencyProperty IsAutoAppendProperty =
            DependencyProperty.Register("IsAutoAppend", typeof(bool), typeof(AutoComplete), new PropertyMetadata(false, new PropertyChangedCallback(OnIsAutoAppendChanged)));

        /// <summary>
        /// This property indicates the max height of the popup window. 
        /// </summary>
        public static readonly DependencyProperty MaxDropHeightProperty =
            DependencyProperty.Register("MaxDropHeight", typeof(double), typeof(AutoComplete), new PropertyMetadata(150d, new PropertyChangedCallback(OnMaxDropHeightChanged)));

        /// <summary>
        /// This property indicates whether to use filter. 
        /// </summary>
        public static readonly DependencyProperty IsFilterProperty =
            DependencyProperty.Register("IsFilter", typeof(bool), typeof(AutoComplete), new PropertyMetadata(true, new PropertyChangedCallback(OnIsFilterChanged)));

        /// <summary>
        /// This property indicates whether auto completed content text
        /// is the only one correct variant. 
        /// </summary>
        public static readonly DependencyProperty IsAutoCompleteItemProperty =
            DependencyProperty.Register("IsAutoCompleteItem", typeof(bool), typeof(AutoComplete), new PropertyMetadata(false, new PropertyChangedCallback(OnIsAutoCompleteItemChanged)));

        /// <summary>
        /// This property indicates mode and adds content items. 
        /// </summary>
        public static readonly DependencyProperty IsAsyncAddContentProperty =
            DependencyProperty.Register("IsAsyncAddContent", typeof(bool), typeof(AutoComplete), new PropertyMetadata(true, new PropertyChangedCallback(OnIsAsyncAddContentChanged)));

        /// <summary>
        /// This property contains control's text content. 
        /// </summary>
        public static readonly DependencyProperty TextProperty =
            DependencyProperty.Register("Text", typeof(string), typeof(AutoComplete), new PropertyMetadata(string.Empty, new PropertyChangedCallback(OnTextChanged)));

        /// <summary>
        /// This property indicates state of the history popup window. 
        /// </summary>
        internal static readonly DependencyProperty IsHistoryDropDownOpenProperty =
            DependencyProperty.Register("IsHistoryDropDownOpen", typeof(bool), typeof(AutoComplete), new PropertyMetadata(false, new PropertyChangedCallback(OnIsHistoryDropDownOpenChanged)));

        /// <summary>
        /// This property defines the height of the history list. 
        /// </summary>
        internal static readonly DependencyProperty HistoryListHeightProperty =
            DependencyProperty.Register("HistoryListHeight", typeof(double), typeof(AutoComplete), new PropertyMetadata(150d, new PropertyChangedCallback(OnHistoryListHeightChanged)));

        /// <summary>
        /// File name for saving history in isolate storage.
        /// </summary>
        internal static readonly DependencyProperty FileStorageNameProperty =
            DependencyProperty.Register("FileStorageName", typeof(string), typeof(AutoComplete), null);

        /// <summary>
        /// Set the Visibility State of DropDown Button in AutoCompleteTextBox
        /// </summary>
        public static readonly DependencyProperty DropDownButtonVisibilityProperty =
            DependencyProperty.Register("DropDownButtonVisibility", typeof(Visibility), typeof(AutoComplete), new PropertyMetadata(Visibility.Visible, new PropertyChangedCallback(OnDropDownButtonVisibilityChanged)));

        /// <summary>
        /// Set the Enabled State of History in AutoCompleteTextBox
        /// </summary>
        public static readonly DependencyProperty IsHistoryEnabledProperty =
            DependencyProperty.Register("IsHistoryEnabled", typeof(bool), typeof(AutoComplete), new PropertyMetadata(true, new PropertyChangedCallback(OnIsHistoryEnabledChanged)));

        /// <summary>
        /// This property indicates the seperator character
        /// </summary>
        public static readonly DependencyProperty SeparatorCharProperty =
           DependencyProperty.Register("SeparatorChar", typeof(char), typeof(AutoComplete), new PropertyMetadata(';'));

        /// <summary>
        /// This property indicates the Selection mode
        /// </summary>
        public static readonly DependencyProperty SelectionModeProperty =
           DependencyProperty.Register("SelectionMode", typeof(System.Windows.Controls.SelectionMode), typeof(AutoComplete), new PropertyMetadata(System.Windows.Controls.SelectionMode.Single, new PropertyChangedCallback(OnSelectionModeChanged)));

        /// <summary>
        /// 
        /// </summary>
        public static readonly DependencyProperty CanResizePopupProperty =
            DependencyProperty.Register("CanResizePopup", typeof(bool), typeof(AutoComplete), new PropertyMetadata(true));

        // Using a DependencyProperty as the backing store for IsLoadAllItemsOnToggle.  This enables animation, styling, binding, etc...
        /// <summary>
        /// 
        /// </summary>
        public static readonly DependencyProperty IsLoadCustomSourceOnToggleProperty =
            DependencyProperty.Register("IsLoadCustomSourceOnToggle", typeof(bool), typeof(AutoComplete), new PropertyMetadata(false));
        
        #endregion

        #region - Constants -
        /// <summary>
        /// Sets string for search in template element.
        /// </summary>
        private const string PART_EditableTextBox = "PART_EditableTextBox";

        /// <summary>
        /// Sets string for search in template element.
        /// </summary>
        private const string PART_Container = "PART_Container";

        /// <summary>
        /// AC Part Popup is initialized in this variable
        /// </summary>
        /// <returns>
        /// Type : string
        /// <para>Default Value : PART_ACPopup</para>
        /// </returns>
        private const string PART_ACPopup = "PART_ACPopup";

        /// <summary>
        /// History Popup is initialized in this variable
        /// </summary>
        /// <returns>
        /// Type : string
        /// <para>Default Value : PART_HistoryPopup</para>
        /// </returns>
        private const string PART_HistoryPopup = "PART_HistoryPopup";

        /// <summary>
        /// Sets string for search in template element.
        /// </summary>
        /// <returns>
        /// Type : string
        /// <para>Default Value : PART_HistoryContainer</para>
        /// </returns>
        private const string PART_HistoryContainer = "PART_HistoryContainer";

        /// <summary>
        /// Sets string for searching in template element.
        /// </summary>
        private const string PART_DropDownButton = "PART_DropDownButton";

        /// <summary>
        /// Sets string for search in template element.
        /// </summary>
        private const string PART_VistaProgressBar = "PART_VistaProgressBar";

        /// <summary>
        /// File Extention is initialized in this variable
        /// </summary>
        /// <returns>
        /// Type : string
        /// <para>Default Value : .dat</para>
        /// </returns>
        private const string FileExt = ".dat";

        /// <summary>
        /// Maximum File name Length is initialized in this variable
        /// </summary>
        /// <returns>
        /// Type : int 
        /// <para>Default Value : 248</para>
        /// </returns>
        private const int MaxFileNameLenght = 248;
        #endregion

        #region - Private Variables -
        /// <summary>
        /// Set the Length of the Textbox before a new Character Typed.
        /// </summary>
        private int textboxlengthbeforetextchange;

        /// <summary>
        /// Set True when the BackSpace Button is pressed
        /// </summary>
        private bool isnotbackspacekey = true;

        /// <summary>
        /// Presents file name for saving in internal isolated store.
        /// </summary>
        private string storefilename = string.Empty;

        /// <summary>
        /// This member contains a value indicating whether there is
        /// total clear items view.
        /// </summary>
        private bool istotalclear = false;

        /// <summary>
        /// This member contains a value indicating whether the text
        /// change is internal (doesn't need to use auto-complete).
        /// </summary>
        private bool isinternalchange = false;

        /// <summary>
        /// This member contains a value indicating whether to click on
        /// auto-complete items.
        /// </summary>
        private bool isclickitems = false;

        /// <summary>
        /// This will set true or false, when key down
        /// </summary>
        private bool iskeydown;

        /// <summary>
        /// This is set when last key occured.
        /// </summary>
        private Key lastkey = new Key();

        /// <summary>
        /// This member contains the current tree level root.
        /// </summary>
        private IAutocompleteRoot root = null;

        /// <summary>
        /// This member contains history list. 
        /// </summary>
        private ObservableCollection<string> historyList = new ObservableCollection<string>();

        /// <summary>
        /// This member contains observer collection. 
        /// </summary>
        private ObservableCollection<string> autocompletelist = new ObservableCollection<string>();

        /// <summary>
        /// This member contains hash level. 
        /// </summary>
        private IAutocompleteLevel hashlevel = null;

        /// <summary>
        /// This member contains TextBox for typing. 
        /// </summary>
        internal TextBox mtextbox = null;

        /// <summary>
        /// This member contains auto-completes ItemsControl for view. 
        /// </summary>
        internal AutoCompleteListBox mitemsautocomplete = null;

        /// <summary>
        /// Reference to the AC Popup
        /// </summary>
        /// <returns>
        /// Type : Popup
        /// <para>Default Value : null</para>
        /// </returns>

        internal Popup oldpopup = null;
        internal Popup macpopup = null;

        /// <summary>
        /// Reference to the AC History Popup
        /// </summary>
        /// <returns>
        /// Type : Popup
        /// <para>Default Value : null</para>
        /// </returns>
        internal Popup machistorypopup = null;

        /// <summary>
        /// This member contains history ItemsControl for view.
        /// </summary>
        internal ListBox mitemshistory = null;

        /// <summary>
        /// Button will get affected when drop down button is set.
        /// </summary>
        internal Button dropdownbutton = null;

        ////TODO:
        ////private VistaSpecificProgressBar m_VistaProgressBar = null;

        private bool isinternal = false;

        private Thumb HistoryThumb;

        private Thumb ContainerThumb;

        /// <summary>
        /// Member containslsit of custom source flag
        /// </summary>
        private List<String> CustomSourceString;

        /// <summary>
        /// Member contains list of business object
        /// </summary>
        private List<KeyValuePair<string, object>> BusinessObject = new List<KeyValuePair<string, object>>();

        private bool canCustomListBoxOpen = true;

        /// <summary>
        /// Member contains the selected item list
        /// </summary>
        private System.Collections.IList SelectedItems = new List<object>();

        private bool canAllowTextChangeMethods = true;

        private bool isNotSelectedUsingKey = false;

        private bool isIndexChangedinDesign = true;
        #endregion

        #region - Initialization -

        /// <summary>
        /// Initializes a new instance of the <see
        /// cref="T:Syncfusion.Windows.Tools.Controls.AutoComplete">AutoComplete</see>
        /// class.
        /// </summary>
        /// <remarks>
        /// Public Constructor of the AutoComplete Control, which will initialize all events
        /// and properties.
        /// </remarks>
        public AutoComplete()
        {
            DefaultStyleKey = typeof(AutoComplete);
            this.Dispatcher.BeginInvoke(this.LoadDispatherLoaded);
            CurrencyTextBox rr = new CurrencyTextBox();
        }

        /// <summary>
        /// Static constructor.
        /// </summary>
        static AutoComplete()
        {
            if (DesignerProperties.IsInDesignTool)
            {
                LoadDependentAssemblies load = new LoadDependentAssemblies();
                load = null;
            }
        }

        #endregion

        #region Event
        /// <summary>
        /// Occurs when [selection mode changed].
        /// </summary>
        public event PropertyChangedCallback SelectionModeChanged;

        /// <summary>
        /// Event that is raised when DropDownButtonVisibility property is changed.
        /// </summary>
        public event PropertyChangedCallback DropDownButtonVisibilityChanged;

        /// <summary>
        /// Event that is raised when IsHistoryEnabled property is changed.
        /// </summary>
        public event PropertyChangedCallback IsHistoryEnabledChanged;

        /// <summary>
        /// Event that is raised when IsDropDownOpen property is changed.
        /// </summary>
        public event PropertyChangedCallback IsDropDownOpenChanged;

        /// <summary>
        /// Event that is raised when CustomSource property is changed.
        /// </summary>
        public event PropertyChangedCallback CustomSourceChanged;

        /// <summary>
        /// Event that is raised when IsAutoAppend property is changed.
        /// </summary>
        public event PropertyChangedCallback IsAutoAppendChanged;

        /// <summary>
        /// Event that is raised when MaxDropHeight property is changed.
        /// </summary>
        public event PropertyChangedCallback MaxDropHeightChanged;

        /// <summary>
        /// Event that is raised when IsFilter property is changed.
        /// </summary>
        public event PropertyChangedCallback IsFilterChanged;

        /// <summary>
        /// Event that is raised when IsAutoCompleteItem property is changed.
        /// </summary>
        public event PropertyChangedCallback IsAutoCompleteItemChanged;

        /// <summary>
        /// Event that is raised when IsAsyncAddContent property is changed.
        /// </summary>
        public event PropertyChangedCallback IsAsyncAddContentChanged;

        /// <summary>
        /// Event that is raised when Text property is changed.
        /// </summary>
        public event PropertyChangedCallback TextChanged;

        /// <summary>
        /// Event that is raised when IsHistoryDropDownOpen property is changed.
        /// </summary>
        internal event PropertyChangedCallback IsHistoryDropDownOpenChanged;

        /// <summary>
        /// Event that is raised when m_HistoryListHeight property is changed.
        /// </summary>
        internal event PropertyChangedCallback HistoryListHeightChanged;

        internal event PropertyChangedCallback SelectedValuePathChanged;

        /// <summary>
        /// 
        /// </summary>
        public event SelectionChangedEventHandler SelectionChanged;
        #endregion

        #region - DP Getter and Setter

        /// <summary>
        /// 
        /// </summary>
        public bool CanResizePopup
        {
            get { return (bool)GetValue(CanResizePopupProperty); }
            set { SetValue(CanResizePopupProperty, value); }
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
        /// Gets or sets the selected value.
        /// </summary>
        /// <value>The selected value.</value>
        public object SelectedValue
        {
            get { return (object)GetValue(SelectedValueProperty); }
            set { SetValue(SelectedValueProperty, value); }
        }

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
        /// Gets or sets the selected item.
        /// </summary>
        /// <value>The selected item.</value>
        public object SelectedItem
        {
            get { return (object)GetValue(SelectedItemProperty); }
            set { SetValue(SelectedItemProperty, value); }
        }

        /// <summary>
        /// 
        /// </summary>
        public new String DisplayMemberPath
        {
            get { return (string)GetValue(DisplayMemberPathProperty); }
            set { SetValue(DisplayMemberPathProperty, value); }
        }

        /// <summary>
        /// 
        /// </summary>
        public bool IsLoadCustomSourceOnToggle
        {
            get { return (bool)GetValue(IsLoadCustomSourceOnToggleProperty); }
            set { SetValue(IsLoadCustomSourceOnToggleProperty, value); }
        }


        /// <summary>
        /// Gets or sets a value indicating whether the IsDropDownOpen dependency property.
        /// </summary>
        /// <remarks>
        /// <b>Xaml</b>
        /// <para></para>
        /// <para>&lt;Syncfusion:AutoComplete  Name=&quot;AutoCompleteTextBox&quot;
        /// IsDropDownOpen=&quot;true&quot;/&gt;</para>
        /// <para></para>
        /// <para><b>C#</b></para>
        /// <para></para>
        /// <para>AutoComplete AutoCompleteTextBox=new AutoComplete();</para>
        /// <para>AutoCompleteTextBox.IsDropDownOpen=true;</para>
        /// </remarks>
        /// <value>
        /// Type : bool
        /// </value>
        public bool IsDropDownOpen
        {
            get
            {
                return (bool)GetValue(IsDropDownOpenProperty);
            }

            set
            {
                if (value != this.IsDropDownOpen)
                {
                    if (value == false)
                    {
                        ////To Move ScrollViewer of Listbox to Origin Place
                        this.FindListBoxChildren(this.mitemsautocomplete);
                        this.dropdownbutton.Focus();
                    }

                    if (value == true && this.autocompletelist.Count == 0)
                    {
                        if (this.mtextbox != null)
                            this.mtextbox.Focus();
                    }
                    else
                    {
                        this.SetValue(IsDropDownOpenProperty, value);
                        if (value == false && macpopup.IsOpen == true)
                        {
                            macpopup.IsOpen = false;
                        }
                        else if (value == true)
                        {
                            macpopup.IsOpen = true;


                            //this.mtextbox.Focus();
                        }
                    }

                    if (value == false && this.autocompletelist.Count == 0)
                    {
                        this.mtextbox.Focus();
                    }
                }
            }
        }

        /// <summary>
        /// Gets or sets the value of the CustomSource dependency property.
        /// </summary>
        /// <remarks>
        /// <b>C#</b>
        /// <para></para>
        /// <para>List&lt;string&gt; customSource = new List&lt;string&gt;();</para>
        /// <para>            customSource.Add(&quot;Components&quot;);</para>
        /// <para>            customSource.Add(&quot;User Control&quot;);</para>
        /// <para>            customSource.Add(&quot;Custom Controls&quot;);</para>
        /// <para>AutoComplete AutoCompleteTextBox=new AutoComplete();</para>
        /// <para>AutoCompleteTextBox.CustomSource = customSource;</para>
        /// </remarks>
        public System.Collections.IEnumerable CustomSource
        {
            get
            {
                return (System.Collections.IEnumerable)GetValue(CustomSourceProperty);
            }
            set
            {
                SetValue(CustomSourceProperty, value);
                // this.ItemsSource = value;
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether the IsAutoAppend dependency property.
        /// </summary>
        /// <remarks>
        /// <b>Xaml</b>
        /// <para></para>
        /// <para>&lt;Syncfusion:AutoComplete  Name=&quot;AutoCompleteTextBox&quot;
        /// IsAutoAppend=&quot;true&quot;/&gt;</para>
        /// <para></para>
        /// <para><b>C#</b></para>
        /// <para></para>
        /// <para>AutoComplete AutoCompleteTextBox=new AutoComplete();</para>
        /// <para>AutoCompleteTextBox.IsAutoAppend=true;</para>
        /// </remarks>
        /// <value>
        /// Type : bool
        /// </value>
        public bool IsAutoAppend
        {
            get
            {
                return (bool)GetValue(IsAutoAppendProperty);
            }

            set
            {
                value = CoerceIsAutoAppendChanged(this, value);
                SetValue(IsAutoAppendProperty, value);
            }
        }

        /// <summary>
        /// Gets or sets the value of the DropDownButtonVisibility dependency property.
        /// </summary>
        /// <remarks>
        /// <b>Xaml</b>
        /// <para></para>
        /// <para>&lt;Syncfusion:AutoComplete  Name=&quot;AutoCompleteTextBox&quot;
        /// DropDownButtonVisibility=&quot;Visibility.Visible&quot;/&gt;</para>
        /// <para></para>
        /// <para><b>C#</b></para>
        /// <para></para>
        /// <para>AutoComplete AutoCompleteTextBox=new AutoComplete();</para>
        /// <para>AutoCompleteTextBox.DropDownButtonVisibility=Visibility.Visible;</para>
        /// </remarks>
        /// <value>
        /// Type : bool
        /// </value>
        public Visibility DropDownButtonVisibility
        {
            get
            {
                return (Visibility)GetValue(DropDownButtonVisibilityProperty);
            }

            set
            {
                SetValue(DropDownButtonVisibilityProperty, value);
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether the IsHistoryEnabled dependency property.
        /// </summary>
        /// <remarks>
        /// <b>Xaml</b>
        /// <para></para>
        /// <para>&lt;Syncfusion:AutoComplete  Name=&quot;AutoCompleteTextBox&quot;
        /// IsHistoryEnabled=&quot;true&quot;/&gt;</para>
        /// <para></para>
        /// <para><b>C#</b></para>
        /// <para></para>
        /// <para>AutoComplete AutoCompleteTextBox=new AutoComplete();</para>
        /// <para>AutoCompleteTextBox.IsHistoryEnabled=true;</para>
        /// </remarks>
        /// <value>
        /// Type : bool
        /// </value>
        public bool IsHistoryEnabled
        {
            get
            {
                return (bool)GetValue(IsHistoryEnabledProperty);
            }

            set
            {
                if (value == false)
                {
                    List<string> temp = new List<string>();
                    if (this.CustomSourceString != null && this.mitemshistory != null)
                    {
                        temp = this.CustomSourceString;

                        foreach (string removestr in this.mitemshistory.Items)
                        {
                            temp.Remove(removestr.ToString());
                        }

                        this.CustomSource = temp;
                        this.CreateRoot();
                    }
                }
                else
                {
                    if (this.CustomSourceString != null && this.mitemshistory != null && this.mitemshistory.Items.Count != 0)
                    {
                        List<string> temp = new List<string>();
                        temp = this.CustomSourceString;
                        foreach (string addstr in this.mitemshistory.Items)
                        {
                            temp.Add(addstr.ToString());
                        }

                        this.CustomSource = temp;
                        this.CreateRoot();
                    }
                }

                this.SetValue(IsHistoryEnabledProperty, value);
            }
        }

        /// <summary>
        /// Gets or sets the value of the MaxDropHeight dependency property.
        /// </summary>
        /// <remarks>
        /// <b>Xaml</b>
        /// <para></para>
        /// <para>&lt;Syncfusion:AutoComplete  Name=&quot;AutoCompleteTextBox&quot;
        /// MaxDropHeight=&quot;20&quot;/&gt;</para>
        /// <para></para>
        /// <para><b>C#</b></para>
        /// <para></para>
        /// <para>AutoComplete AutoCompleteTextBox=new AutoComplete();</para>
        /// <para>AutoCompleteTextBox.MaxDropHeight=20;  </para>
        /// </remarks>
        /// <value>
        /// Type : double
        /// </value>
        public double MaxDropHeight
        {
            get
            {
                return (double)GetValue(MaxDropHeightProperty);
            }

            set
            {
                SetValue(MaxDropHeightProperty, value);
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether the IsFilter dependency property.
        /// </summary>
        /// <remarks>
        /// <b>Xaml</b>
        /// <para></para>
        /// <para>&lt;Syncfusion:AutoComplete  Name=&quot;AutoCompleteTextBox&quot;
        /// IsFilter=&quot;true&quot;/&gt;</para>
        /// <para></para>
        /// <para><b>C#</b></para>
        /// <para></para>
        /// <para>AutoComplete AutoCompleteTextBox=new AutoComplete();</para>
        /// <para>AutoCompleteTextBox.IsFilter=true; </para>
        /// </remarks>
        /// <value>
        /// Type : bool
        /// </value>
        public bool IsFilter
        {
            get
            {
                return (bool)GetValue(IsFilterProperty);
            }

            set
            {
                SetValue(IsFilterProperty, value);
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether the IsAutoCompleteItem dependency property.
        /// </summary>
        /// <value>
        /// 	<c>true</c> if this instance is auto complete item; otherwise, <c>false</c>.
        /// </value>
        public bool IsAutoCompleteItem
        {
            get
            {
                return (bool)GetValue(IsAutoCompleteItemProperty);
            }

            set
            {
                value = CoerceIsAutoCompleteItemChanged(this, value);
                SetValue(IsAutoCompleteItemProperty, value);
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether the IsAsyncAddContent dependency property.
        /// </summary>
        /// <remarks>
        /// <b>Xaml</b>
        /// <para></para>
        /// <para>&lt;Syncfusion:AutoComplete  Name=&quot;AutoCompleteTextBox&quot;
        /// IsAsyncAddContent=&quot;true&quot;/&gt;</para>
        /// <para></para>
        /// <para><b>C#</b></para>
        /// <para></para>
        /// <para>AutoComplete AutoCompleteTextBox=new AutoComplete();</para>
        /// <para>AutoCompleteTextBox.IsAsyncAddContent=true;  </para>
        /// </remarks>
        /// <value>
        /// Type : bool
        /// </value>
        public bool IsAsyncAddContent
        {
            get
            {
                return (bool)GetValue(IsAsyncAddContentProperty);
            }

            set
            {
                SetValue(IsAsyncAddContentProperty, value);
            }
        }

        /// <summary>
        /// Gets or sets the value of the Text dependency property.
        /// </summary>
        /// <remarks>
        /// <b>Xaml</b>
        /// <para></para>
        /// <para>&lt;Syncfusion:AutoComplete  Name=&quot;AutoCompleteTextBox&quot;
        /// Text=&quot;string&quot;/&gt;</para>
        /// <para></para>
        /// <para><b>C#</b></para>
        /// <para></para>
        /// <para>AutoComplete AutoCompleteTextBox=new AutoComplete();</para>
        /// <para>AutoCompleteTextBox.Text=&quot;string&quot;; </para>
        /// </remarks>
        /// <value>
        /// Type : string
        /// </value>
        public string Text
        {
            get
            {
                return (string)GetValue(TextProperty);
            }

            set
            {
                SetValue(TextProperty, value);
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether the IsHistoryDropDownOpen dependency property.
        /// </summary>
        /// <remarks>
        ///  <b>Xaml</b>
        /// <para></para>
        /// <para>&lt;Syncfusion:AutoComplete  Name=&quot;AutoCompleteTextBox&quot;
        /// IsHistoryDropDownOpen=&quot;true&quot;/&gt;</para>
        /// <para></para>
        /// <para><b>C#</b></para>
        /// <para></para>
        /// <para>AutoComplete AutoCompleteTextBox=new AutoComplete();</para>
        /// <para>AutoCompleteTextBox.IsHistoryDropDownOpen=true; </para>
        /// </remarks>
        /// <value>
        /// Type : bool
        /// </value>
        public bool IsHistoryDropDownOpen
        {
            get
            {
                return (bool)this.GetValue(IsHistoryDropDownOpenProperty);
            }

            set
            {
                SetValue(IsHistoryDropDownOpenProperty, value);
                //if (value != this.IsHistoryDropDownOpen)
                //{
                //    this.SetValue(IsHistoryDropDownOpenProperty, value);
                //}
            }
        }

        /// <summary>
        /// Gets or sets the value of the m_HistoryListHeight dependency property.
        /// </summary>
        /// <remarks>
        /// <b>Xaml</b>
        /// <para></para>
        /// <para>&lt;Syncfusion:AutoComplete  Name=&quot;AutoCompleteTextBox&quot;
        /// HistoryListHeight=&quot;20&quot;/&gt;</para>
        /// <para></para>
        /// <para><b>C#</b></para>
        /// <para></para>
        /// <para>AutoComplete AutoCompleteTextBox=new AutoComplete();</para>
        /// <para>AutoCompleteTextBox.HistoryListHeight=20;</para>
        /// </remarks>
        /// <value>
        /// Type : double
        /// </value>
        public double HistoryListHeight
        {
            get
            {
                return (double)GetValue(HistoryListHeightProperty);
            }

            set
            {
                SetValue(HistoryListHeightProperty, value);
            }
        }

        /// <summary>
        /// Gets or sets the value of the FileStorageName dependency
        /// property.
        /// </summary>
        /// <value>The name of the file storage.</value>
        public string FileStorageName
        {
            get
            {
                return (string)GetValue(FileStorageNameProperty);
            }

            set
            {
                SetValue(FileStorageNameProperty, value);
            }
        }

        /// <summary>
        /// Gets or sets the value of the m_Root dependency property.
        /// </summary>
        /// <value>The root.</value>
        private IAutocompleteRoot Root
        {
            get
            {
                if (null == this.root)
                {
                    this.CreateRoot();
                }

                return this.root;
            }

            set
            {
                if (null != this.root)
                {
                    this.root = value;
                }
            }
        }

        /// <summary>
        /// Gets a value indicating whether true or false for this Dependency Property.
        /// </summary>
        /// <value><c>true</c> if this instance is init; otherwise, <c>false</c>.</value>
        private bool IsInit
        {
            get
            {
                return this.mtextbox != null;
            }
        }

        /// <summary>
        /// Gets a value indicating whether this value contains file name that should be stored.
        /// </summary>
        /// <value>The name of the store file.</value>
        private string StoreFileName
        {
            get
            {
                string sfName = this.storefilename;

                if (string.IsNullOrEmpty(sfName))
                {
                    sfName = this.Name;
                }

                if (string.IsNullOrEmpty(sfName))
                {
                    throw new Exception("Cannot Load or Save History because StoreFileName and Name are both string.Empty.");
                }

                int lenght = sfName.Length;

                if (MaxFileNameLenght <= lenght)
                {
                    sfName = sfName.Remove(0, lenght - MaxFileNameLenght);
                }

                sfName = sfName + FileExt;

                return sfName;
            }
        }

        /// <summary>
        /// Gets a value indicating whether History button is focused.
        /// </summary>
        /// <value>The history button.</value>
        private Button HistoryButton
        {
            get
            {
                if (this.dropdownbutton == null)
                {
                    this.dropdownbutton = this.GetTemplateChild(PART_DropDownButton) as Button;
                }

                return this.dropdownbutton;
            }
        }
        
        /// <summary>
        /// 
        /// </summary>
        public char SeparatorChar
        {
            get { return (char)GetValue(SeparatorCharProperty); }
            set { SetValue(SeparatorCharProperty, value); }
        }

        /// <summary>
        /// Gets or sets the selection mode.
        /// </summary>
        /// <value>The selection mode.</value>
        public System.Windows.Controls.SelectionMode SelectionMode
        {
            get { return (System.Windows.Controls.SelectionMode)GetValue(SelectionModeProperty); }
            set { SetValue(SelectionModeProperty, value); }
        }
        #endregion

        #region Public methods
        /// <summary>
        /// Called when an internal process or application calls ApplyTemplate, which is
        /// used to build the current template's visual tree.
        /// </summary>
        /// <remarks>
        /// This will executes only when the m_TextBox should not be null and the exception
        /// are generated if tis null.
        /// </remarks>

        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();
            //// m_VistaProgressBar = this.GetTemplateChild( PART_VistaProgressBar ) as VistaSpecificProgressBar;
            if (null != this.mtextbox)
            {
                this.mtextbox.TextChanged -= new TextChangedEventHandler(this.OnChildTextBoxTextChanged);
                this.mtextbox.KeyUp -= new KeyEventHandler(this.OnChildTextBoxKeyUp);
                this.mtextbox.LayoutUpdated -= new EventHandler(this.M_TextBox_LayoutUpdated);
                this.mtextbox.RemoveHandler(TextBox.KeyDownEvent, new KeyEventHandler(OnChildTextBox_KeyDown));
            }

            if (this.mitemsautocomplete != null)
            {
                this.mitemsautocomplete.MouseLeftButtonUp -= new MouseButtonEventHandler(this.OnChildm_ItemsAutoCompleteMouseLeftButtonUp);
                this.mitemsautocomplete.SelectionChanged -= new SelectionChangedEventHandler(mitemsautocomplete_SelectionChanged);
                this.mitemsautocomplete.RemoveHandler(ListBox.KeyDownEvent, new KeyEventHandler(OnCustomListBox_KeyDown));
            }

            this.mtextbox = this.GetTemplateChild(PART_EditableTextBox) as TextBox;
            this.mitemsautocomplete = this.GetTemplateChild(PART_Container) as AutoCompleteListBox;

            if (this.macpopup != null)
                oldpopup = this.macpopup;

            this.macpopup = this.GetTemplateChild(PART_ACPopup) as Popup;

            if (oldpopup != null && oldpopup != this.macpopup)
                oldpopup.IsOpen = false; 
          
            if (this.macpopup != null)
            {
                this.macpopup.Opened += new EventHandler(macpopup_Opened);
            }

            this.machistorypopup = this.GetTemplateChild(PART_HistoryPopup) as Popup;

            if (mtextbox != null)
            {
                this.mtextbox.TextChanged += new TextChangedEventHandler(this.OnChildTextBoxTextChanged);
                this.mtextbox.KeyUp += new KeyEventHandler(this.OnChildTextBoxKeyUp);
                this.mtextbox.LayoutUpdated += new EventHandler(this.M_TextBox_LayoutUpdated);
                this.mtextbox.AddHandler(TextBox.KeyDownEvent, new KeyEventHandler(OnChildTextBox_KeyDown), true);
            }

            if (mitemsautocomplete != null)
            {
                this.mitemsautocomplete.MouseLeftButtonUp += new MouseButtonEventHandler(this.OnChildm_ItemsAutoCompleteMouseLeftButtonUp);
                this.mitemsautocomplete.SelectionChanged += new SelectionChangedEventHandler(mitemsautocomplete_SelectionChanged);
                this.mitemsautocomplete.AddHandler(ListBox.KeyDownEvent, new KeyEventHandler(OnCustomListBox_KeyDown), true);
                this.mitemsautocomplete.ItemsSource = this.autocompletelist;
            }

            this.KeyUp += new KeyEventHandler(AutoComplete_KeyUp);

            if (System.Windows.Application.Current.RootVisual != null)
            {
                System.Windows.Application.Current.RootVisual.MouseLeftButtonDown += new MouseButtonEventHandler(RootVisual_MouseLeftButtonDown);
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
                this.ContainerThumb.MouseEnter += new MouseEventHandler(HistoryThumb_MouseEnter);
                this.ContainerThumb.MouseLeave += new MouseEventHandler(HistoryThumb_MouseLeave);
                this.ContainerThumb.DragDelta += new DragDeltaEventHandler(ContainerThumb_DragDelta);
            }            
            this.CreateViewHistory();
            this.RelateHistory();

            this.dropdownbutton.Focus();
            this.FindParent(this);          
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="e"></param>
        protected override void OnItemsChanged(NotifyCollectionChangedEventArgs e)
        {          
          
                MapCustomObject();
           
        }

        private void OnCustomListBox_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                this.IsDropDownOpen = false;
                this.mtextbox.Focus();
                this.mtextbox.SelectionStart = this.mtextbox.Text.Length;
            }
        }

        void mitemshistory_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (this.mitemshistory.SelectedItem != null)
            {
                isIndexChangedinDesign = false;
                if (!this.isNotSelectedUsingKey)
                {
                    this.canAllowTextChangeMethods = false;
                    if (!this.mtextbox.Text.Contains(this.SeparatorChar.ToString()))
                    {
                        this.SelectedItems.Clear();
                        this.mtextbox.Text = this.mitemshistory.SelectedItem.ToString();
                    }
                    else if (this.mtextbox.Text.Substring(this.mtextbox.Text.Length - 1, 1) != this.SeparatorChar.ToString() && this.SelectedItems.Count > 1)
                    {
                        this.SelectedItems.Remove(this.SelectedItems[this.SelectedItems.Count - 1]);
                        if (!this.SelectedItems.Contains(this.mitemshistory.SelectedItem))
                        {
                            SetSafeText(this.mitemshistory.SelectedItem.ToString());
                        }
                        else
                        {
                            SetSafeText(";");
                        }
                    }
                    else
                    {
                        if (!this.SelectedItems.Contains(this.mitemshistory.SelectedItem))
                        {
                            SetSafeText(this.mitemshistory.SelectedItem.ToString());
                        }
                        else
                        {
                            SetSafeText(";");
                        }
                    }
                }
                if (this.DisplayMemberPath == string.Empty)
                {
                    this.SelectedItem = this.mitemshistory.SelectedItem;
                    this.SelectedIndex = this.Items.IndexOf(this.mitemshistory.SelectedItem);
                    this.SelectedValue = this.mitemshistory.SelectedValue;

                    if (this.SelectionMode != System.Windows.Controls.SelectionMode.Single && !(this.SelectedItems.Contains(this.mitemshistory.SelectedItem)))
                    {
                        this.SelectedItems.Add(this.mitemshistory.SelectedItem);
                    }

                    //if (this.SelectionMode == System.Windows.Controls.SelectionMode.Extended && !(this.SelectedItems.Contains(this.mitemshistory.SelectedItems)))
                    //{
                    //    this.SelectedItems.Clear();
                    //    this.SelectedItems.Add(this.mitemshistory.SelectedItems);
                    //}
                }
                else
                {
                    string str = this.mitemshistory.SelectedItem.ToString();
                    foreach (object obj in this.Items)
                    {
                        Type t = obj.GetType();
                        var prop = t.GetProperty(DisplayMemberPath);
                        var prop1 = t.GetProperty(SelectedValuePath);
                        object s_value = prop1.GetValue(obj, null);
                        object s = prop.GetValue(obj, null);
                        if (str == s.ToString())
                        {
                            this.SelectedItem = obj;
                            this.SelectedIndex = this.Items.IndexOf(obj);
                            this.SelectedValue = s_value;
                        }
                    }

                    if (this.SelectionMode != System.Windows.Controls.SelectionMode.Single && !(this.SelectedItems.Contains(this.mitemshistory.SelectedItem)))
                    {
                        this.SelectedItems.Add(this.mitemshistory.SelectedItem);
                    }

                    //if (this.SelectionMode == System.Windows.Controls.SelectionMode.Extended && !(this.SelectedItems.Contains(this.mitemshistory.SelectedItems)))
                    //{
                    //    this.SelectedItems.Clear();
                    //    this.SelectedItems.Add(this.mitemshistory.SelectedItems);
                    //}
                }
                this.OnSelectionChanged(e);
            }
        }

        /// <summary>
        /// Handles the SelectionChanged event of the mitemsautocomplete control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.Windows.Controls.SelectionChangedEventArgs"/> instance containing the event data.</param>
        void mitemsautocomplete_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (this.mitemsautocomplete.SelectedItem != null)
            {               
                isIndexChangedinDesign = false;
                if (!this.isNotSelectedUsingKey)
                {
                    this.canAllowTextChangeMethods = false;
                    if (!this.mtextbox.Text.Contains(this.SeparatorChar.ToString()))
                    {
                        this.SelectedItems.Clear();
                        this.mtextbox.Text = this.mitemsautocomplete.SelectedItem.ToString();
                    }
                    else if (this.mtextbox.Text.Substring(this.mtextbox.Text.Length - 1, 1) != this.SeparatorChar.ToString() && this.SelectedItems.Count > 1)
                    {
                        this.SelectedItems.Remove(this.SelectedItems[this.SelectedItems.Count - 1]);
                        if (!this.SelectedItems.Contains(this.mitemsautocomplete.SelectedItem))
                        {
                            SetSafeText(this.mitemsautocomplete.SelectedItem.ToString());
                        }
                        else
                        {
                            SetSafeText(";");
                        }
                    }
                    else
                    {
                        if (this.SelectionMode != System.Windows.Controls.SelectionMode.Single && !(this.SelectedItems.Contains(this.mtextbox.Text.Substring(0, this.mtextbox.Text.Length - 1))))
                        {
                            this.SelectedItems.Clear();
                            this.SelectedItems.Add(this.mtextbox.Text.Substring(0, this.mtextbox.Text.Length - 1));
                        }
                        if (!this.SelectedItems.Contains(this.mitemsautocomplete.SelectedItem))
                        {
                            SetSafeText(this.mitemsautocomplete.SelectedItem.ToString());
                        }
                        else
                        {
                            SetSafeText(";");
                        }
                    }
                }

                if (this.DisplayMemberPath == string.Empty)
                {
                    this.SelectedItem = this.mitemsautocomplete.SelectedItem;
                    this.SelectedIndex = this.Items.IndexOf(this.mitemsautocomplete.SelectedItem);
                    this.SelectedValue = this.mitemsautocomplete.SelectedValue;

                    if (this.SelectionMode != System.Windows.Controls.SelectionMode.Single && !(this.SelectedItems.Contains(this.mitemsautocomplete.SelectedItem)))
                    {
                        this.SelectedItems.Add(this.mitemsautocomplete.SelectedItem);
                    }
                }
                else
                {
                    string str = this.mitemsautocomplete.SelectedItem.ToString();
                    foreach (object obj in this.Items)
                    {
                        Type t = obj.GetType();
                        var prop = t.GetProperty(DisplayMemberPath);
                        var prop1 = t.GetProperty(SelectedValuePath);
                        if (prop1 != null)
                        {
                            object s_value = prop1.GetValue(obj, null);
                       
                        object s = prop.GetValue(obj, null);
                        if (str == s.ToString())
                        {
                            this.SelectedItem = obj;
                            this.SelectedIndex = this.Items.IndexOf(obj);
                            this.SelectedValue = s_value;
                        }
                        }
                    }

                    if (this.SelectionMode != System.Windows.Controls.SelectionMode.Single && !(this.SelectedItems.Contains(this.mitemsautocomplete.SelectedItem)))
                    {
                        this.SelectedItems.Add(this.mitemsautocomplete.SelectedItem);
                    }
                }
                this.OnSelectionChanged(e);
            }
        }

        void ContainerThumb_DragDelta(object sender, DragDeltaEventArgs e)
        {
            if (CanResizePopup)
            {
                double yadjust = this.mitemsautocomplete.Height + e.VerticalChange;
                double xadjust = this.mitemsautocomplete.Width + e.HorizontalChange;

                if ((xadjust >= 0) && (yadjust >= 0))
                {
                    this.mitemsautocomplete.Width = xadjust;
                    this.mitemsautocomplete.Height = yadjust;
                }
            }
        }

        void HistoryThumb_DragDelta(object sender, DragDeltaEventArgs e)
        {
            if (CanResizePopup)
            {
                double yadjust = this.mitemshistory.Height + e.VerticalChange;
                double xadjust = this.mitemshistory.Width + e.HorizontalChange;

                if ((xadjust >= 0) && (yadjust >= 0))
                {
                    this.mitemshistory.Width = xadjust;
                    this.mitemshistory.Height = yadjust;
                }
            }
        }

        void HistoryThumb_MouseLeave(object sender, MouseEventArgs e)
        {
            Thumb t = sender as Thumb;
            t.Cursor = Cursors.None;
        }

        void HistoryThumb_MouseEnter(object sender, MouseEventArgs e)
        {
            Thumb t = sender as Thumb;
            t.Cursor = Cursors.SizeNWSE;
        }

        void RootVisual_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            this.IsDropDownOpen = false;
            this.IsHistoryDropDownOpen = false;
        }

        /// <summary>
        /// Handles the Opened event of the macpopup control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
        void macpopup_Opened(object sender, EventArgs e)
        {
            object obj = null;
            if (this.mitemsautocomplete.ObjectToListBoxItem.Count > 0)
            {
                if (mtextbox.Text == "")
                {
                    obj = this.mitemsautocomplete.Items[0];
                }
                else
                {
                    obj = this.mitemsautocomplete.SelectedItem;

                }
                if (obj != null)
                {
                    try
                    {
                        ListBoxItem item = this.mitemsautocomplete.ObjectToListBoxItem[obj];
                        item.IsSelected = true;
                        this.ShowSelectedItem();
                    }
                    catch { }
                }
            }
        }
        #endregion

        #region - Protected Methods
        /// <summary>
        /// Called when [selection mode changed].
        /// </summary>
        /// <param name="d">The d.</param>
        /// <param name="e">The <see cref="System.Windows.DependencyPropertyChangedEventArgs"/> instance containing the event data.</param>
        private static void OnSelectionModeChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            AutoComplete instance = (AutoComplete)d;
            instance.OnSelectionModeChanged(e);
            if (instance.SelectionMode == System.Windows.Controls.SelectionMode.Multiple)
                instance.SelectionMode = System.Windows.Controls.SelectionMode.Extended;
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

        /// <summary>
        /// Updates property value cache and raises DropDownButtonVisibilityChanged
        /// event.
        /// </summary>
        /// <param name="e">Property changes details, such as old value
        /// and new value.</param>
        protected virtual void OnDropDownButtonVisibilityChanged(DependencyPropertyChangedEventArgs e)
        {
            if (this.DropDownButtonVisibilityChanged != null)
            {
                this.DropDownButtonVisibilityChanged(this, e);
            }
        }

        /// <summary>
        /// Updates property value cache and raises IsHistoryEnabledChanged
        /// event.
        /// </summary>
        /// <param name="e">Property changes details, such as old value
        /// and new value.</param>
        protected virtual void OnIsHistoryEnabledChanged(DependencyPropertyChangedEventArgs e)
        {
            if (!this.IsInit)
            {
                return;
            }

            if (this.IsHistoryEnabled)
            {
                this.CreateViewHistory();
                this.RelateHistory();
            }

            if (this.IsHistoryEnabledChanged != null)
            {
                this.IsHistoryEnabledChanged(this, e);
            }
        }

        /// <summary>
        /// Updates property value cache and raises IsDropDownOpenChanged
        /// event.
        /// </summary>
        /// <param name="e">Property changes details, such as old value
        /// and new value.</param>
        protected virtual void OnIsDropDownOpenChanged(DependencyPropertyChangedEventArgs e)
        {
            ////if( this.IsDropDownOpen )
            ////{
            ////    Mouse.Capture( this, CaptureMode.SubTree );
            ////}
            ////else
            ////    Mouse.Capture( null );

            if (this.IsDropDownOpenChanged != null)
            {
                this.IsDropDownOpenChanged(this, e);
            }

            //// To prevent one DD over another
            if (this.IsDropDownOpen == true)
            {
                this.IsHistoryDropDownOpen = false;
            }
        }

        /// <summary>
        /// Updates property value cache and raises CustomSourceChanged
        /// event.
        /// </summary>
        /// <param name="e">Property changes details, such as old value
        /// and new value.</param>
        protected virtual void OnCustomSourceChanged(DependencyPropertyChangedEventArgs e)
        {
            this.Root = null;
            if (this.CustomSourceChanged != null)
            {
                this.CustomSourceChanged(this, e);
            }
                if (CustomSource != null)
                    ItemsSource = CustomSource;
                        
            MapCustomObject();
        }

        private void MapCustomObject()
        {
            if (Items.Count > 0)
            {
                List<String> Temp = new List<String>();
                bool StringFlag, MemberPathFlag;
                object TestObj = Items[0];
                StringFlag = TestObj is string;
                MemberPathFlag = DisplayMemberPath.Equals(String.Empty);

                if (StringFlag)
                {
                    foreach (object i in Items)
                    {
                        if (i != null && !(Temp.Contains((string)i)))
                        {
                            Temp.Add((String)i);
                        }
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

                            BusinessObject.Add(new KeyValuePair<string, object>(s.ToString(), i));
                            if (!(Temp.Contains(s.ToString())))
                                Temp.Add(s.ToString());
                        }
                        else
                        {
                            Temp.Add(i.ToString());
                        }
                    }
                }
                CustomSourceString = Temp;
            }
        }

        /// <summary>
        /// Updates property value cache and raises IsAutoAppendChanged
        /// event.
        /// </summary>
        /// <param name="e">Property changes details, such as old value
        /// and new value.</param>
        protected virtual void OnIsAutoAppendChanged(DependencyPropertyChangedEventArgs e)
        {
            this.IsAutoCompleteItem = CoerceIsAutoCompleteItemChanged(this, this.IsAutoCompleteItem);
            if (this.IsAutoAppendChanged != null)
            {
                this.IsAutoAppendChanged(this, e);
            }
        }

        /// <summary>
        /// Updates property value cache and raises MaxDropHeightChanged
        /// event.
        /// </summary>
        /// <param name="e">Property changes details, such as old value
        /// and new value.</param>
        protected virtual void OnMaxDropHeightChanged(DependencyPropertyChangedEventArgs e)
        {
            if (this.MaxDropHeightChanged != null)
            {
                this.MaxDropHeightChanged(this, e);
            }
        }

        /// <summary>
        /// Updates property value cache and raises IsFilterChanged
        /// event.
        /// </summary>
        /// <param name="e">Property changes details, such as old value
        /// and new value.</param>
        protected virtual void OnIsFilterChanged(DependencyPropertyChangedEventArgs e)
        {
            this.IsAutoAppend = CoerceIsAutoAppendChanged(this, this.IsAutoAppend);
            if (this.IsFilterChanged != null)
            {
                this.IsFilterChanged(this, e);
            }
        }

        /// <summary>
        /// Updates property value cache and raises
        /// IsAutoCompleteItemChanged event.
        /// </summary>
        /// <param name="e">Property changes details, such as old value
        /// and new value.</param>
        protected virtual void OnIsAutoCompleteItemChanged(DependencyPropertyChangedEventArgs e)
        {
            if (this.IsAutoCompleteItemChanged != null)
            {
                this.IsAutoCompleteItemChanged(this, e);
            }
        }

        /// <summary>
        /// Updates property value cache and raises
        /// IsAsyncAddContentChanged event.
        /// </summary>
        /// <param name="e">Property changes details, such as old value
        /// and new value.</param>
        protected virtual void OnIsAsyncAddContentChanged(DependencyPropertyChangedEventArgs e)
        {
            if (this.IsAsyncAddContentChanged != null)
            {
                this.IsAsyncAddContentChanged(this, e);
            }
        }

        /// <summary>
        /// Updates property value cache and raises TextChanged event.
        /// </summary>
        /// <param name="e">Property changes details, such as old value
        /// and new value.</param>
        protected virtual void OnTextChanged(DependencyPropertyChangedEventArgs e)
        {
            if (this.TextChanged != null)
            {
                this.TextChanged(this, e);
            }
        }

        /// <summary>
        /// Updates property value cache and raises
        /// IsHistoryDropDownOpenChanged event.
        /// </summary>
        /// <param name="e">Property changes details, such as old value
        /// and new value.</param>
        protected virtual void OnIsHistoryDropDownOpenChanged(DependencyPropertyChangedEventArgs e)
        {
            ////if( this.IsHistoryDropDownOpen )
            ////{
            ////    Mouse.Capture( this, CaptureMode.SubTree );
            ////}
            ////else
            ////    Mouse.Capture( null );

            if (this.IsHistoryDropDownOpenChanged != null)
            {
                this.IsHistoryDropDownOpenChanged(this, e);
            }
            //// To prevent one DD over another
            if (this.IsHistoryDropDownOpen == true)
            {
                this.IsDropDownOpen = false;
            }
        }

        /// <summary>
        /// Updates property value cache and raises
        /// m_HistoryListHeightChanged event.
        /// </summary>
        /// <param name="e">Property changes details, such as old value
        /// and new value.</param>
        protected virtual void OnHistoryListHeightChanged(DependencyPropertyChangedEventArgs e)
        {
            if (this.HistoryListHeightChanged != null)
            {
                this.HistoryListHeightChanged(this, e);
            }
        }

        /// <summary>
        /// This method creates some root object which depends on the
        /// source.
        /// </summary>
        protected virtual void CreateRoot()
        {
            this.Clear();
            this.hashlevel = null;

            this.root = new CustomRoot(CustomSourceString);
        }

        /// <summary>
        /// Causes the object to scroll into view for auto-append. 
        /// </summary>
        protected virtual void ScrollIntoAutoCompleteView()
        {
            ListBox listBox = this.mitemsautocomplete as ListBox;
            if (null != listBox && null != listBox.SelectedItem)
            {
                //// Need to call this before ScrollIntoView, otherwise crashes.
                listBox.UpdateLayout();
                if (listBox.ActualWidth > 0 && listBox.ActualHeight > 0)
                {
                    listBox.ScrollIntoView(listBox.SelectedItem);
                }
            }
        }

        /// <summary>
        /// Causes the object to scroll into view for history. 
        /// </summary>
        protected virtual void ScrollIntoHistoryView()
        {
            ListBox litBox = this.mitemshistory as ListBox;
            if (null != litBox && null != litBox.SelectedItem)
            {
                litBox.ScrollIntoView(litBox.SelectedItem);
            }
        }

        /// <summary>
        /// Handles, when the mouse left button is clicked
        /// </summary>
        /// <param name="e">The instance containing the event data.</param>
        protected override void OnMouseLeftButtonDown(MouseButtonEventArgs e)
        {
            base.OnMouseLeftButtonDown(e);
            this.IsDropDownOpen = false;
            this.IsHistoryDropDownOpen = false;
        }

        /// <summary>
        /// Handles, when the keys in the keyboard is pressed
        /// </summary>
        /// <param name="e">The instance containing the event data.</param>
        protected override void OnKeyDown(KeyEventArgs e)
        {
            base.OnKeyDown(e);
            this.iskeydown = true;
            this.lastkey = e.Key;
            SolidColorBrush colorValue = (SolidColorBrush)this.mtextbox.Background;
            string colorName = colorValue.Color.ToString();
            //if (!(colorName == "#FF333333"))
            //{
            //    this.mtextbox.Background = new SolidColorBrush(Colors.Red);
            //}

            if (Key.Enter == e.Key)
            {
                if (this.IsDropDownOpen)
                {
                    //if (IsAutoAppend && this.mitemsautocomplete.Items.Count >0)
                    //{
                    //    isNotSelectedUsingKey = true;
                    //    this.mitemsautocomplete.SelectedIndex = 0;
                    //}
                    this.EnterTextSelectText();
                }
                else if (this.IsHistoryDropDownOpen)
                {
                    this.ScrollIntoHistoryView();
                    this.SetTextFromHistory();
                    this.IsHistoryDropDownOpen = false;
                }
            }
            else if (Key.Escape == e.Key)
            {
                if (this.IsDropDownOpen)
                {
                    this.IsDropDownOpen = false;
                }
                else if (this.IsHistoryDropDownOpen)
                {
                    this.IsHistoryDropDownOpen = false;
                }
            }

            if (e.Key == Key.Up || e.Key == Key.Down)
            {
                e.Handled = true;
            }
        }

        /// <summary>
        /// Called before the <see cref="E:System.Windows.UIElement.MouseEnter"/> event occurs.
        /// </summary>
        /// <param name="e">The data for the event.</param>
        protected override void OnMouseEnter(MouseEventArgs e)
        {
            VisualStateManager.GoToState(this, "MouseOver", false);
        }

        /// <summary>
        /// Called before the <see cref="E:System.Windows.UIElement.MouseLeave"/> event occurs.
        /// </summary>
        /// <param name="e">The data for the event.</param>
        protected override void OnMouseLeave(MouseEventArgs e)
        {
            VisualStateManager.GoToState(this, "Normal", false);
        }

        /// <summary>
        /// Handler for handling when the control got focus
        /// </summary>
        /// <param name="e">The instance containing the event data.</param>
        protected override void OnGotFocus(RoutedEventArgs e)
        {
            base.OnGotFocus(e);
            if (this.isclickitems)
            {
                this.isclickitems = false;
            }
            else
            {
                if (FocusManager.GetFocusedElement() != this.HistoryButton)
                {
                    //Dispatcher.BeginInvoke(this.ShowItemsAfterSetFocus);
                }
            }
            VisualStateManager.GoToState(this, "Focused", false);
        }

        /// <summary>
        /// Handler for handling when the control has lost its  focus
        /// </summary>
        /// <param name="e">The instance containing the event data.</param>
        protected override void OnLostFocus(RoutedEventArgs e)
        {
            base.OnLostFocus(e);
            DependencyObject focused = FocusManager.GetFocusedElement() as DependencyObject;

            if (focused != null && (VisualUtils.IsDescendant(this, focused)
                || VisualUtils.IsDescendant(this.mitemsautocomplete, focused) || VisualUtils.IsDescendant(this.mitemshistory, focused)))
            {

                if (focused.GetType() == typeof(Button) || focused.GetType() == typeof(TextBox))
                {
                    this.IsDropDownOpen = false;
                    this.IsHistoryDropDownOpen = false;
                }
                else
                {
                    return;
                }
            }
            else
            {
                this.IsDropDownOpen = false;
                this.IsHistoryDropDownOpen = false;
            }

            VisualStateManager.GoToState(this, "Unfocused", false);
            VisualStateManager.GoToState(this, "Normal", true);
        }
        #endregion

        #region - Static Methods

        private static void OnDisplayMemberPathChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            AutoComplete instance = d as AutoComplete;
            instance.MapCustomObject();
        }

        private static void OnSelectedItemChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            AutoComplete instance = d as AutoComplete;
            if (!instance.isinternal)
            {
                if (instance.DisplayMemberPath == string.Empty)
                {
                    string source = (string)e.NewValue;
                    if (instance.Items.Contains(source))
                    {
                        instance.isinternal = true;
                        instance.SelectedIndex = instance.Items.IndexOf(source);
                        instance.SelectedItem = e.NewValue;
                        instance.SelectedValue = instance.SelectedItem;
                    }
                }
                else
                {
                    object obj = e.NewValue;
                    Type t = obj.GetType();
                    var prop = t.GetProperty(instance.DisplayMemberPath);
                    var prop1 = t.GetProperty(instance.SelectedValuePath);
                    object s_value = prop1.GetValue(obj, null);
                    object s = prop.GetValue(obj, null);
                    instance.SelectedItem = obj;
                    instance.SelectedIndex = instance.Items.IndexOf(obj);
                    instance.SelectedValue = s_value;
                }
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="obj"></param>
        /// <param name="args"></param>
        public static void OnSelectedIndexChanged(DependencyObject obj, DependencyPropertyChangedEventArgs args)
        {
            AutoComplete ac = (AutoComplete)obj;
            if (ac.Items != null && ac.Items.Count > 0 && (int)args.NewValue >= 0)
            {
                if (ac.DisplayMemberPath != string.Empty)
                {
                    object selectedObject = ac.Items[(int)args.NewValue];
                    Type t = selectedObject.GetType();
                    var prop = t.GetProperty(ac.DisplayMemberPath);
                    object s = prop.GetValue(selectedObject, null);
                    ac.SelectedItem = selectedObject;
                    if (ac.isIndexChangedinDesign)
                        ac.Text = s.ToString();
                }
                else
                {
                    ac.SelectedItem = ac.Items[(int)args.NewValue];
                    if (ac.isIndexChangedinDesign)
                        ac.Text = ac.Items[(int)args.NewValue].ToString();
                }
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="obj"></param>
        /// <param name="args"></param>
        public static void OnSelectedValuePathChanged(DependencyObject obj, DependencyPropertyChangedEventArgs args)
        {
            AutoComplete ac = (AutoComplete)obj;
            if (ac != null)
            {
                ac.OnSelectedValuePathChanged(args);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="args"></param>
        protected void OnSelectedValuePathChanged(DependencyPropertyChangedEventArgs args)
        {
            if (this.SelectedValuePathChanged != null)
            {
                this.SelectedValuePathChanged(this, args);
            }
        }

        /// <summary>
        /// This method corrects IsAutoAppendChanged.
        /// </summary>
        /// <param name="d">object to which this property
        /// belongs.</param>
        /// <param name="baseValue">The property whish should be
        /// corrected.</param>
        /// <returns>
        /// Corrected value.
        /// </returns>
        private static bool CoerceIsAutoAppendChanged(DependencyObject d, object baseValue)
        {
            AutoComplete instance = (AutoComplete)d;
            bool newValue = (bool)baseValue;
            if (newValue && !instance.IsFilter)
            {
                return false;
            }

            return newValue;
        }

        /// <summary>
        /// This method corrects IsAutoCompleteItem.
        /// </summary>
        /// <param name="d">object to which this property
        /// belongs.</param>
        /// <param name="baseValue">The property whish should be
        /// corrected.</param>
        /// <returns>
        /// Corrected value.
        /// </returns>
        private static bool CoerceIsAutoCompleteItemChanged(DependencyObject d, object baseValue)
        {
            AutoComplete instance = (AutoComplete)d;
            bool newValue = (bool)baseValue;
            if (newValue && (!instance.IsFilter || !instance.IsAutoAppend))
            {
                return false;
            }

            return newValue;
        }

        /// <summary>
        /// Calls OnDropDownButtonVisibilityChanged method of the instance,
        /// notifies of the dependency property value changes.
        /// </summary>
        /// <param name="d">Dependency object, the change occurs on.</param>
        /// <param name="e">Property changes details, such as old value
        /// and new value.</param>
        private static void OnDropDownButtonVisibilityChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            AutoComplete instance = (AutoComplete)d;
            instance.OnDropDownButtonVisibilityChanged(e);
        }

        /// <summary>
        /// Calls OnIsHistoryEnabledChanged method of the instance,
        /// notifies of the dependency property value changes.
        /// </summary>
        /// <param name="d">Dependency object, the change occurs on.</param>
        /// <param name="e">Property changes details, such as old value
        /// and new value.</param>
        private static void OnIsHistoryEnabledChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            AutoComplete instance = (AutoComplete)d;
            instance.OnIsHistoryEnabledChanged(e);
        }

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

        /// <summary>
        /// Calls OnHistoryListHeightChanged method of the instance,
        /// notifies of the dependency property value changes.
        /// </summary>
        /// <param name="d">Dependency object, the change occurs on.</param>
        /// <param name="e">Property change details, such as old value
        /// and new value.</param>
        private static void OnHistoryListHeightChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            AutoComplete instance = (AutoComplete)d;
            instance.OnHistoryListHeightChanged(e);
        }
        #endregion

        #region - Private Methods -
        /// <summary>
        /// When TextBox layout updates the sets the width using drop down button visibility.
        /// </summary>
        /// <param name="sender">The Sender</param>
        /// <param name="e">The instance containing the event data.</param>
        private void M_TextBox_LayoutUpdated(object sender, EventArgs e)
        {
            //if (this.DropDownButtonVisibility == Visibility.Visible)
            //{
            //    this.mitemsautocomplete.Width = this.mtextbox.ActualWidth + this.dropdownbutton.ActualWidth+5;
            //}
            //else
            //{
            //    this.mitemsautocomplete.Width = this.mtextbox.ActualWidth+5;
            //}

            //this.mitemshistory.Width = this.mtextbox.ActualWidth+5;
        }

        /// <summary>
        /// When the scrolling done, it gets affected.
        /// </summary>
        /// <param name="sender">The sender</param>
        /// <param name="e">The instance containing the event data.</param>
        private void Sc_Scroll(object sender, ScrollEventArgs e)
        {
            this.IsDropDownOpen = false;
        }

        ////void sc_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        ////{
        ////    //throw new NotImplementedException();
        ////    //if (AutoComplete.i == 1)
        ////    //{
        ////    //    AutoComplete.i = 0;
        ////        IsDropDownOpen = false;
        ////    //}
        ////}

        /// <summary>
        /// Finds the childrens and assigned in list box.
        /// </summary>
        /// <param name="control">this is used to pass the current control</param>
        private void FindListBoxChildren(DependencyObject control)
        {
            ScrollViewer listBScrollViewer = control as ScrollViewer;
            if (listBScrollViewer != null)
            {
                listBScrollViewer.ScrollToHorizontalOffset(0);
                listBScrollViewer.ScrollToVerticalOffset(0);
            }

            int children = VisualTreeHelper.GetChildrenCount(control);
            for (int i = 0; i < children; i++)
            {
                DependencyObject child = VisualTreeHelper.GetChild(control, i);
                this.FindListBoxChildren(child);
            }
        }

        /// <summary>
        /// To find the Parent of the current control.
        /// </summary>
        /// <param name="control">This is used to pass current element</param>
        private void FindParent(UIElement control)
        {
            UIElement p = VisualTreeHelper.GetParent(control) as UIElement;
            if (p != null)
            {
                if (p is ScrollViewer)
                {
                    this.FindChildren(p);
                }

                this.FindParent(p);
            }
        }

        /// <summary>
        /// To find the Children of the current control.
        /// </summary>
        /// <param name="control">This is used to pass current element</param>
        private void FindChildren(DependencyObject control)
        {
            ScrollBar sc = control as ScrollBar;
            if (sc != null)
            {
                sc.ValueChanged += new RoutedPropertyChangedEventHandler<double>(this.Sc_ValueChanged);
                sc.Scroll += new ScrollEventHandler(this.Sc_Scroll);
            }

            int children = VisualTreeHelper.GetChildrenCount(control);
            for (int i = 0; i < children; i++)
            {
                DependencyObject child = VisualTreeHelper.GetChild(control, i);
                this.FindChildren(child);
            }
        }

        /// <summary>
        /// To set the value which was key downed
        /// </summary>
        /// <param name="sender">The sender</param>
        /// <param name="e">Property change details, such as old value and new value.</param>
        private void Sc_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            if (this.iskeydown)
            {
                if (macpopup.IsOpen == true && lastkey != Key.Enter)
                {
                    macpopup.IsOpen = false;
                    macpopup.IsOpen = true;
                }
            }
        }
        #endregion

        #region - Implemenation
        /// <summary>
        /// This method creates viewer for history list.
        /// </summary>
        private void CreateViewHistory()
        {
            //// Template is not initialized yet.
            if (this.mtextbox == null)
            {
                return;
            }           

            if (null != this.mitemshistory)
            {
                //this.mitemshistory.MouseMove -= new MouseEventHandler(this.OnChildItemsHistoryMouseMove);
                this.mitemshistory.MouseLeftButtonUp -= new MouseButtonEventHandler(this.OnChildItemsHistoryMouseLeftButtonUp);
                this.mitemshistory.SelectionChanged -= new SelectionChangedEventHandler(mitemshistory_SelectionChanged);
            }

            if (this.HistoryButton != null)
            {
                this.HistoryButton.Click -= new RoutedEventHandler(this.OnChildHistoryButtonClicked);
            }

            if (this.IsHistoryEnabled)
            {
                this.mitemshistory = this.GetTemplateChild(PART_HistoryContainer) as ListBox;
                if (this.mitemshistory == null)
                {
                    throw new Exception("Cannot find Part Name:" + PART_HistoryContainer + " of type ListBox in AutoComplete template.");
                }

                if (null == this.HistoryButton)
                {
                    throw new Exception("Cannot find Part Name:" + PART_DropDownButton + " of type Button in AutoComplete template.");
                }

                ////HistoryButton.Visibility = Visibility.Visible;

                //this.mitemshistory.MouseMove += new MouseEventHandler(this.OnChildItemsHistoryMouseMove);
                this.mitemshistory.MouseLeftButtonUp += new MouseButtonEventHandler(this.OnChildItemsHistoryMouseLeftButtonUp);
                this.HistoryButton.Click += new RoutedEventHandler(this.OnChildHistoryButtonClicked);
                this.mitemshistory.SelectionChanged += new SelectionChangedEventHandler(mitemshistory_SelectionChanged);
            }
            else
            {
                if (this.HistoryButton != null)
                {
                    this.HistoryButton.Visibility = Visibility.Collapsed;
                }

                this.mitemshistory = null;
            }
        }

        internal bool ChildToggleClickFlag = false;

        /// <summary>
        /// Called when a history Button was checked.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The instance containing the event data.</param>
        private void OnChildHistoryButtonClicked(object sender, RoutedEventArgs e)
        {
            this.mitemshistory.Width = this.ActualWidth;
            ChildToggleClickFlag = true;
            if (IsLoadCustomSourceOnToggle)
            {
                if (!IsDropDownOpen)
                    IsDropDownOpen = true;
                IsHistoryDropDownOpen = false;
            }
            else
            {
                if (!IsHistoryDropDownOpen)
                    IsHistoryDropDownOpen = true;
                IsDropDownOpen = false;
            }
            //if (this.mtextbox.Text == string.Empty)
            //{
            //    if (this.IsDropDownOpen == true)
            //        this.IsDropDownOpen = false;
            //    this.IsHistoryDropDownOpen = true;
            //}
            //else
            //{
            //    if (this.IsHistoryDropDownOpen == true)
            //        this.IsHistoryDropDownOpen = false;
            //    this.IsDropDownOpen = true;
            //}

            if (IsLoadCustomSourceOnToggle)
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
                        autocompletelist.Clear();
                        for (int i = 0; i < countItems; ++i)
                        {
                            autocompletelist.Add(rootText + collection[i].Text);
                        }
                    }
                }
                IsHistoryDropDownOpen = false;
                SetItems(currentLevel, false);
                if (this.SelectedItem != null)
                {
                    this.mitemsautocomplete.SelectedItem = this.SelectedItem;
                    ShowSelectedItem();
                }
            }
            //else
            //{
            //    IsDropDownOpen = false;
            //}

            //this.IsHistoryDropDownOpen = !this.IsHistoryDropDownOpen;
            //if (this.IsHistoryDropDownOpen == false)
            //{
            //    if (this.IsDropDownOpen == false && this.autocompletelist.Count == 0)
            //    {
            //        this.mtextbox.Focus();
            //    }
            //    else
            //    {
            //        this.IsDropDownOpen = !this.IsDropDownOpen;
            //    }
            //}
            //else
            //{
            //    this.IsDropDownOpen = false;
            //}

            //if (this.IsDropDownOpen == true)
            //{
            //    this.IsDropDownOpen = false;
            //}

            //this.IsHistoryDropDownOpen = true;

            //this.mtextbox.SelectionStart = this.mtextbox.Text.Length;
        }

        /// <summary>
        /// This method binds logical and visual representation history. 
        /// </summary>
        private void RelateHistory()
        {
            if (!this.IsInit)
            {
                return;
            }

            if (this.IsHistoryEnabled)
            {
                this.mitemshistory.ItemsSource = this.historyList;
            }
        }

        /// <summary>
        /// Calls after control's graphic is loaded. 
        /// </summary>
        private void LoadDispatherLoaded()
        {
            this.RelateHistory();
            this.autocompletelist.CollectionChanged += new NotifyCollectionChangedEventHandler(this.OnObserverCollectionChanged);
        }

        /// <summary>
        /// Calls when observer collection is changed.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The instance containing the event data.</param>
        private void OnObserverCollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            if (0 == this.autocompletelist.Count && this.istotalclear)
            {
                this.IsDropDownOpen = false;
            }
        }

        private void OnChildTextBox_KeyDown(object sender, KeyEventArgs e)
        {
            if (mitemsautocomplete != null)
            {
                this.mitemsautocomplete.Width = this.ActualWidth;
            }

            if (mitemshistory != null)
            {
                this.mitemshistory.Width = this.ActualWidth;
            }

            canAllowTextChangeMethods = true;
            if (e.Key == Key.Delete)
            {
                if (mtextbox != null)
                {
                    if (this.mtextbox.Text.Contains(this.SeparatorChar.ToString()))
                    {
                        String[] items = this.mtextbox.Text.Split(SeparatorChar);
                        System.Collections.IList TempSelectedItems = new List<object>();
                        foreach (object item in this.SelectedItems)
                        {
                            TempSelectedItems.Add(item);
                        }

                        foreach (object obj in TempSelectedItems)
                        {
                            bool isItemPresent = false;
                            foreach (string str in items)
                            {
                                if (str == obj.ToString())
                                {
                                    isItemPresent = true;
                                    break;
                                }
                            }
                            if (!isItemPresent)
                            {
                                this.SelectedItems.Remove(obj);
                            }
                        }
                    }

                    else
                    {
                        this.SelectedItems.Clear();
                    }
                }
            }

            if (e.Key == Key.Back)
            {
                isnotbackspacekey = false;
                if (this.mtextbox.Text.Contains(this.SeparatorChar.ToString()))
                {
                    String[] items = this.mtextbox.Text.Split(SeparatorChar);
                    int i = 0;
                    int j = 0;
                    if (this.mtextbox.SelectionStart == this.mtextbox.Text.Length && this.CustomSourceString.Contains(items[items.Length - 1]))
                    {
                        j = items.Length;
                    }
                    else
                    {
                        j = items.Length - 1;
                    }
                    try
                    {
                        for (int k = 0; k <= j; k++)
                        {
                            if (!(this.CustomSourceString.Contains(items[k])))
                            {
                                this.SelectedItems.Remove(this.SelectedItems[i]);
                                i--;
                            }
                            i++;
                        }
                    }
                    catch
                    {
                    }
                }
                else if (!(SelectedItems.Count == 1 && SelectedItems[0].ToString() == this.mtextbox.Text))
                {
                    this.SelectedItems.Clear();
                }
            }
            else
            {
                isnotbackspacekey = true;
            }

            if (e.Key == Key.Down)
            {
                if (this.mtextbox.Text != string.Empty)
                {
                    this.IsDropDownOpen = true;
                }
                else if (this.IsDropDownOpen == false)
                {
                    this.IsHistoryDropDownOpen = true;
                }
            }

            if (e.Key == Key.End && this.IsAutoAppend && IsDropDownOpen)
            {
                string[] str = this.mtextbox.Text.Split(this.SeparatorChar);
                if (this.autocompletelist.Contains(str[str.Length - 1]))
                {
                    this.isNotSelectedUsingKey = true;
                    this.mitemsautocomplete.SelectedIndex = 0;
                }
            }
        }

        /// <summary>
        /// Called when a key is pressed while the keyboard is focused on
        /// this element.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The instance containing the event data.</param>
        private void OnChildTextBoxKeyUp(object sender, KeyEventArgs e)
        {
            this.iskeydown = false;
            if (!e.Handled && Key.Tab == e.Key)
            {
                this.IsDropDownOpen = false;
                this.IsHistoryDropDownOpen = false;
            }

            this.WorkUpAutoComplateList(e.Key);
            this.WorkUpm_HistoryList(e.Key);
            e.Handled = this.WorkUpAutoAppend(e.Key);
        }

        /// <summary>
        /// Called when text in internal TextBox is changed.
        /// </summary>
        /// <param name="sender">The Sender.</param>
        /// <param name="e">The instance containing the event data.</param>
        private void OnChildTextBoxTextChanged(object sender, TextChangedEventArgs e)
        {
            if (canAllowTextChangeMethods == true)
            {
                if (!this.isinternalchange)
                {
                    string contentText = this.GetContentText();

                    if (this.IsAsyncAddContent)
                    {
                        this.StartAddAsyncLevel(contentText);
                    }
                    else
                    {
                        this.StartAddSyncLevel(contentText);
                    }
                }

                this.isinternalchange = false;
                this.textboxlengthbeforetextchange = this.mtextbox.Text.Length;

                String[] items = this.mtextbox.Text.Split(this.SeparatorChar);
                if (this.mtextbox.Text != string.Empty && this.autocompletelist.Contains(items[items.Length - 1]) && (this.mtextbox.SelectionStart == this.mtextbox.Text.Length))
                {
                    if (this.DisplayMemberPath == string.Empty)
                    {
                        isNotSelectedUsingKey = true;
                        this.mitemsautocomplete.SelectedIndex = this.autocompletelist.IndexOf(items[items.Length - 1]);
                    }
                    else
                    {
                        foreach (object obj in this.Items)
                        {
                            Type t = obj.GetType();
                            var prop = t.GetProperty(DisplayMemberPath);
                            object s = prop.GetValue(obj, null);
                            if (items[items.Length - 1] == s.ToString())
                            {
                                isNotSelectedUsingKey = true;
                                this.mitemsautocomplete.SelectedIndex = this.autocompletelist.IndexOf(s.ToString());
                            }
                        }
                    }
                }
                else
                {
                    if (this.IsHistoryDropDownOpen == true)
                        this.IsHistoryDropDownOpen = false;
                }
            }
            this.Text = this.mtextbox.Text;
        }

        /// <summary>
        /// Occurs when the left mouse button is released while the mouse
        /// pointer is over this element.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The instance containing the event data.</param>
        private void OnChildm_ItemsAutoCompleteMouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            this.isNotSelectedUsingKey = true;
            canAllowTextChangeMethods = false;
            this.EnterTextSelectText();
            this.IsDropDownOpen = false;
        }

        /// <summary>
        /// Calls when level items async loaded.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The instance containing the event data.</param>
        private void OnLevelItemsAsyncLoaded(object sender, EventArgs e)
        {
            if (this.IsAsyncAddContent)
            {
                IAutocompleteLevel currentLevel = (IAutocompleteLevel)sender;
                currentLevel.ItemsLoaded -= new EventHandler(this.OnLevelItemsAsyncLoaded);
                string contentText = this.GetContentText();
                string rootText = currentLevel.GetFullPath();
                int rootTextLenght = rootText.Length;

                if (contentText.Length >= rootTextLenght && contentText.StartsWith(rootText, StringComparison.OrdinalIgnoreCase))
                {
                    this.LoadAsyncItems(currentLevel, contentText.Substring(rootTextLenght));
                }
                else
                {
                    this.StartAddAsyncLevel(contentText);
                }
            }
        }

        /// <summary>
        /// Occurs when the left mouse button is released while the mouse
        /// pointer is over this element.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The instance containing the event data.</param>
        private void OnChildItemsHistoryMouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            this.canAllowTextChangeMethods = false;
            this.IsHistoryDropDownOpen = false;
            if (null != this.mitemshistory.SelectedItem)
            {
                if (this.SelectionMode == System.Windows.Controls.SelectionMode.Single)
                {
                    this.mtextbox.Text = this.mitemshistory.SelectedItem.ToString();
                    this.mtextbox.SelectionStart = this.mtextbox.Text.Length;
                }
                else
                {
                    SetSafeTextWithMoveCaret(String.Empty);
                }
            }
            this.canCustomListBoxOpen = false;
            this.IsHistoryDropDownOpen = false;
            this.mtextbox.Focus();
        }

        /// <summary>
        /// This method allows to correct the show of selected item in
        /// ItemsControl.
        /// </summary>
        private void ShowSelectedItem()
        {
            this.ScrollIntoAutoCompleteView();
            if (null != this.mitemsautocomplete.SelectedItem)
            {
                if (this.SelectionMode == System.Windows.Controls.SelectionMode.Single)
                {
                    this.SetSafeTextWithMoveCaret(this.mitemsautocomplete.SelectedItem.ToString());
                }
                else
                {
                    this.SetSafeTextWithMoveCaret(string.Empty);
                }
            }
        }

        /// <summary>
        /// This method sets only the new text (doesn't start
        /// auto-complete).
        /// </summary>
        /// <param name="insertString">The new text.</param>
        private void SetSafeText(string insertString)
        {
            if (this.SelectionMode == System.Windows.Controls.SelectionMode.Single)
            {
                if (this.mtextbox.Text != insertString)
                {
                    this.isinternalchange = true;
                    this.mtextbox.Text = insertString;
                }
            }
            else if (this.mtextbox.Text.Contains(this.SeparatorChar.ToString()) && (this.CustomSourceString.Contains(insertString)))
            {
                String str = string.Empty;
                foreach (object obj in this.SelectedItems)
                {
                    str += obj.ToString() + ";";
                }
                if (this.SelectedItems[this.SelectedItems.Count - 1].ToString() != insertString)
                {
                    str += insertString;
                }
                else
                {
                    str = str.Substring(0, str.Length - 1);
                }

                if (this.mtextbox.Text != str)
                {
                    this.mtextbox.Text = str;
                }
            }
            else if (this.mtextbox.Text != insertString)
            {
                this.mtextbox.Text = string.Empty;
                foreach (object obj in this.SelectedItems)
                {
                    this.mtextbox.Text += obj.ToString() + ";";
                }
                if ((this.mtextbox.Text.Contains(this.SeparatorChar.ToString())))
                {
                    if (insertString != string.Empty)
                    {
                        this.mtextbox.Text += insertString;
                    }
                    else
                    {
                        this.mtextbox.Text = this.mtextbox.Text.Substring(0, this.mtextbox.Text.Length - 1);
                    }
                    if (insertString == this.SeparatorChar.ToString())
                    {
                        this.mtextbox.Text = this.mtextbox.Text.Substring(0, this.mtextbox.Text.Length - 1);
                    }
                }
                else
                {
                    this.mtextbox.Text = insertString;
                }
            }
            else if (!this.canAllowTextChangeMethods)
            {
                this.mtextbox.Text = string.Empty;
                foreach (object obj in this.SelectedItems)
                {
                    this.mtextbox.Text += obj.ToString() + ";";
                }
                this.mtextbox.Text = this.mtextbox.Text.Substring(0, this.mtextbox.Text.Length - 1);
            }
        }

        /// <summary>
        /// This method sets only the new text (doesn't start
        /// auto-complete). And moves caret to text's end.
        /// </summary>
        /// <param name="insertString">The new text.</param>
        private void SetSafeTextWithMoveCaret(string insertString)
        {
            this.SetSafeText(insertString);
            this.mtextbox.SelectionStart = this.mtextbox.Text.Length;
        }

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
            ObservableCollection<string> collection = null;
            ListBox collectionView = null;

            if (isAutoComplete)
            {
                collection = this.autocompletelist;
                collectionView = this.mitemsautocomplete;
            }
            else
            {
                collection = this.historyList;
                collectionView = this.mitemshistory;
            }

            for (int i = 0, cnt = collection.Count; i < cnt; ++i)
            {
                if (desireText == collection[i])
                {
                    collectionView.SelectedIndex = i;
                    return;
                }
            }
        }

        /// <summary>
        /// This method moves focus to item after the mouse.
        /// </summary>
        /// <param name="originalSource">Source of the items which holds
        /// the content.</param>
        /// <param name="isAutoComplete">Indicates for which list it is
        /// done, auto\-complete or history
        /// list.</param>
        private void MoveMouse(object originalSource, bool isAutoComplete)
        {
            TextBlock textBlock = originalSource as TextBlock;
            if (null != textBlock)
            {
                this.MoveToText(textBlock.Text, isAutoComplete);
            }
            else
            {
                Grid grid = originalSource as Grid;
                if (null != grid && grid.Children.Count > 4)
                {
                    ContentPresenter contentPresenter = grid.Children[3] as ContentPresenter;
                    if (null != contentPresenter)
                    {
                        this.MoveToText(contentPresenter.Content.ToString(), isAutoComplete);
                    }
                }
            }
        }

        /// <summary>
        /// This method corrects close popup. After changed content-text.
        /// </summary>
        private void EnterTextSelectText()
        {
            this.ShowSelectedItem();
            ////TODO:Add to history??
            ////this.AddHistory( m_TextBox.Text );
            this.IsDropDownOpen = false;
            this.isclickitems = true;
        }

        /// <summary>
        /// This method clears the observe collection. 
        /// </summary>
        private void Clear()
        {
            this.istotalclear = true;
            this.autocompletelist.Clear();
            this.istotalclear = false;
        }

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
                    if (levelText.Length <= contentText.Length && contentText.StartsWith(levelText, StringComparison.OrdinalIgnoreCase))
                    {
                        contentText = contentText.Substring(levelText.Length);
                        this.hashlevel = itemsLevel;
                        if (string.IsNullOrEmpty(contentText))
                        {
                            return itemsLevel;
                        }
                        else
                        {
                            return this.GetNextLevel(itemsLevel, contentText) ?? itemsLevel;
                        }
                    }
                }
            }

            return null;
        }

        /// <summary>
        /// This method gets the current level for content text.
        /// </summary>
        /// <param name="contentText">Text for searching.</param>
        private void StartAddSyncLevel(string contentText)
        {
            IAutocompleteLevel rootLevel = this.Root.GetRoot(contentText);
            if (null != rootLevel)
            {
                string rootPath = rootLevel.GetFullPath();
                int rootPathLenght = rootPath.Length;
                IAutocompleteLevel currentLevel = this.SearchInHash(contentText);
                if (null == currentLevel)
                {
                    currentLevel = rootLevel;
                    this.hashlevel = currentLevel;
                }
                else
                {
                    rootPathLenght = currentLevel.GetFullPath().Length;
                }

                currentLevel = this.GetNextLevel(currentLevel, contentText.Substring(rootPathLenght)) ?? currentLevel;
                currentLevel.LoadItems();
                this.SetItems(currentLevel, this.isnotbackspacekey);
            }
            else
            {
                this.Clear();
            }
        }

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
            while (null != this.hashlevel)
            {
                string hashPath = this.hashlevel.GetFullPath();
                if (hashPath.Length <= rootTextLenght && contentText.StartsWith(hashPath, StringComparison.OrdinalIgnoreCase))
                {
                    break;
                }
                else
                {
                    this.hashlevel = this.hashlevel.Parent;
                }
            }

            return this.hashlevel;
        }

        /// <summary>
        /// This method starts to search the current level with async
        /// load items.
        /// </summary>
        /// <param name="contentText">Text for searching.</param>
        private void StartAddAsyncLevel(string contentText)
        {
            if (null != this.Root)
            {
                IAutocompleteLevel rootLevel = this.Root.GetRoot(contentText);
                if (null != rootLevel)
                {
                    string rootText = rootLevel.GetFullPath();
                    int rootTextLenght = rootText.Length;
                    IAutocompleteLevel currentLevel = this.SearchInHash(contentText);
                    if (null == currentLevel)
                    {
                        currentLevel = this.Root.GetRoot(contentText);
                        this.hashlevel = currentLevel;
                    }
                    else
                    {
                        rootTextLenght = currentLevel.GetFullPath().Length;
                    }

                    this.LoadAsyncItems(currentLevel, contentText.Substring(rootTextLenght));
                }
                else
                {
                    this.Clear();
                }
            }
        }

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
                        if (levelText.Length <= contentText.Length && contentText.StartsWith(levelText, StringComparison.OrdinalIgnoreCase))
                        {
                            contentText = contentText.Substring(levelText.Length);
                            this.hashlevel = itemsLevel;
                            if (string.IsNullOrEmpty(contentText))
                            {
                                if (itemsLevel.IsItemsLoaded)
                                {
                                    this.SetItems(itemsLevel, true);
                                }
                                else
                                {
                                    this.LoadAsyncItems(itemsLevel, contentText);
                                }

                                return;
                            }
                            else
                            {
                                this.LoadAsyncItems(itemsLevel, contentText);
                                return;
                            }
                        }
                    }
                }

                this.SetItems(nextLevel, this.isnotbackspacekey);
                return;
            }
            else
            {
                this.StartProgressAnimation();
                nextLevel.ItemsLoaded += new EventHandler(this.OnLevelItemsAsyncLoaded);
                nextLevel.LoadItemsAsync();
            }
        }

        /// <summary>
        /// This method gets content text without selected part.
        /// </summary>
        /// <returns>
        /// The content text without selected part.
        /// </returns>
        private string GetContentText()
        {
            string str = "";
            //string removestrback = "";
            if (this.mtextbox.Text.Contains(SeparatorChar.ToString()) && (SelectionMode != System.Windows.Controls.SelectionMode.Single))
            {
                string removestrfront = this.mtextbox.Text.Substring(0, this.mtextbox.SelectionStart);
                if (removestrfront.Contains(SeparatorChar.ToString()))
                    removestrfront = removestrfront.Substring(removestrfront.LastIndexOf(SeparatorChar) + 1);
                //if (removestrfront != String.Empty)
                //{
                //    removestrback = this.mtextbox.Text.Substring(this.mtextbox.SelectionStart);
                //    if (removestrback.Contains(SeparatorChar.ToString()))
                //        removestrback = removestrback.Substring(0, removestrback.IndexOf(SeparatorChar));
                //}
                //int tempcaret = this.mtextbox.SelectionStart;
                //str = removestrfront + removestrback;
                str = removestrfront;
                return str;
            }
            else if (this.IsAutoAppend && 0 < this.mtextbox.SelectedText.Length)
            {
                return this.mtextbox.Text.Substring(0, this.mtextbox.Text.Length - this.mtextbox.SelectedText.Length);
            }
            else
            {
                return this.mtextbox.Text;
            }
        }

        /// <summary>
        /// This method sets items for set level.
        /// </summary>
        /// <param name="currentLevel">Level for items.</param>
        /// <param name="isNotBackSpase">Value indicating whether it
        /// isn't BackSpace operation.</param>
        private void SetItems(IAutocompleteLevel currentLevel, bool isNotBackSpase)
        {
            this.StopProgressAnimation();
            if (null != currentLevel)
            {
                AutocompleteItemCollection collection = currentLevel.Items;
                string rootText = currentLevel.GetFullPath();
                int rootTextLenght = rootText.Length;
                string contentText = this.GetContentText();
                int contentLenght = 0;
                if (!(this.mtextbox.Text.Contains(this.SeparatorChar.ToString())))
                {
                    contentLenght = contentText.Length;
                }
                else
                {
                    contentLenght = this.mtextbox.Text.Length - this.mtextbox.SelectedText.Length;
                }
                if (this.IsFilter || this.IsAutoAppend)
                {
                    string searchstring = contentText;
                    AutocompleteItemCollection tempCollection = this.Root.CreateFilteredGhost(currentLevel, searchstring);
                    if (IsLoadCustomSourceOnToggle && ChildToggleClickFlag)
                        tempCollection = currentLevel.Items;
                    bool canIterate = false;
                    foreach (object obj in tempCollection)
                    {
                        Type t = obj.GetType();
                        var prop = t.GetProperty("Text");
                        object value = prop.GetValue(obj, null);
                        if (!this.SelectedItems.Contains(value))
                        {
                            if (IsLoadCustomSourceOnToggle && !ChildToggleClickFlag && IsAutoCompleteItem)
                                this.SelectedItem = value;
                            canIterate = true;
                            break;
                        }
                    }
                    ChildToggleClickFlag = false;
                    if (isNotBackSpase && this.IsAutoAppend && canIterate && !string.IsNullOrEmpty(searchstring))
                    {
                        String AppendText = string.Empty;
                        if (this.SelectionMode != System.Windows.Controls.SelectionMode.Single)
                        {
                            foreach (object obj in tempCollection)
                            {
                                Type t = obj.GetType();
                                var prop = t.GetProperty("Text");
                                object value = prop.GetValue(obj, null);
                                if (!this.SelectedItems.Contains(value))
                                {
                                    AppendText = value.ToString();
                                    break;
                                }
                            }
                        }
                        else
                        {
                            AppendText = tempCollection[0].Text;
                        }
                        this.SetSafeText(rootText + AppendText);
                        if (!this.IsAutoCompleteItem || 1 != tempCollection.Count)
                        {
                            this.mtextbox.SelectionStart = contentLenght;
                            this.mtextbox.Select(contentLenght, int.MaxValue);
                        }
                        else
                        {
                            this.mtextbox.SelectionStart = this.mtextbox.Text.Length;
                        }
                    }

                    if (this.IsFilter)
                    {
                        collection = tempCollection;
                    }
                }

                int countItems = collection.Count;
                if (0 < countItems)
                {
                    this.autocompletelist.Clear();
                    for (int i = 0; i < countItems; ++i)
                    {
                        this.autocompletelist.Add(rootText + collection[i].Text);
                    }

                    if (!this.IsHistoryDropDownOpen && canCustomListBoxOpen)
                    {
                        this.IsDropDownOpen = true;
                    }
                    canCustomListBoxOpen = true;
                    return;
                }
            }

            this.Clear();
        }

        /// <summary>
        /// This method sets text after its cutting down (Del or
        /// BackSpace operation).
        /// </summary>
        /// <param name="newText">The new text.</param>
        private void SetNewTextAfterCutDown(string newText)
        {
            this.SetSafeTextWithMoveCaret(newText);
            IAutocompleteLevel currentLevel = this.SearchInHash(this.mtextbox.Text)
                ?? this.Root.GetRoot(newText);
            if (null != currentLevel)
            {
                this.SetItems(currentLevel, false);
            }
            else
            {
                this.Clear();
            }
        }

        /// <summary>
        /// Handles the KeyUp event of the AutoComplete control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.Windows.Input.KeyEventArgs"/> instance containing the event data.</param>
        void AutoComplete_KeyUp(object sender, KeyEventArgs e)
        {
            DependencyObject focused = FocusManager.GetFocusedElement() as DependencyObject;

            if (!(focused.GetType() == typeof(TextBox)))
            {
                //this.WorkUpAutoComplateList(e.Key);
            }
        }

        /// <summary>
        /// This method works up specific key for auto-complete.
        /// </summary>
        /// <param name="key">Pressed key.</param>
        private void WorkUpAutoComplateList(Key key)
        {
            if (this.IsDropDownOpen)
            {
                if (Key.Up == key)
                {
                    this.ShowSelectedItem();
                    isNotSelectedUsingKey = false;
                    if (this.mitemsautocomplete.SelectedIndex <= 0 && this.autocompletelist.Count > 0)
                    {
                        this.mitemsautocomplete.SelectedIndex = this.mitemsautocomplete.Items.Count - 1;
                    }
                    else if (this.autocompletelist.Count > 0)
                    {
                        this.mitemsautocomplete.SelectedIndex--;
                    }
                }
                else if (Key.Down == key)
                {
                    this.ShowSelectedItem();
                    isNotSelectedUsingKey = false;
                    if (this.mitemsautocomplete.SelectedIndex == this.mitemsautocomplete.Items.Count - 1 && this.autocompletelist.Count > 0)
                    {
                        this.mitemsautocomplete.SelectedIndex = 0;
                    }
                    else if (this.autocompletelist.Count > 0)
                    {
                        this.mitemsautocomplete.SelectedIndex++;
                    }
                    this.mitemsautocomplete.Focus();
                }
            }
            else if (0 != this.autocompletelist.Count && !this.IsHistoryDropDownOpen && (Key.Up == key || Key.Down == key))
            {
                if (this.mtextbox.Text != string.Empty)
                {
                    this.IsDropDownOpen = true;
                }
                else
                {
                    this.IsHistoryDropDownOpen = true;
                    this.mitemshistory.SelectedItem = null;
                }
                this.SetNewTextAfterCutDown(this.mtextbox.Text);
            }
        }

        /// <summary>
        /// This method works up specific key for history.
        /// </summary>
        /// <param name="key">Pressed key.</param>
        private void WorkUpm_HistoryList(Key key)
        {
            if (this.IsHistoryDropDownOpen)
            {
                if (Key.Up == key)
                {
                    isNotSelectedUsingKey = false;
                    if (this.mitemshistory.SelectedIndex <= 0 && this.historyList.Count > 0)
                    {
                        this.mitemshistory.SelectedIndex = this.mitemshistory.Items.Count - 1;
                    }
                    else if (this.historyList.Count > 0)
                    {
                        this.mitemshistory.SelectedIndex--;
                    }

                    this.ScrollIntoHistoryView();
                    this.SetTextFromHistory();
                }
                else if (Key.Down == key)
                {
                    isNotSelectedUsingKey = false;
                    if (this.mitemshistory.SelectedIndex == this.mitemshistory.Items.Count - 1 && this.historyList.Count > 0)
                    {
                        this.mitemshistory.SelectedIndex = 0;
                    }
                    else if (this.historyList.Count > 0)
                    {
                        this.mitemshistory.SelectedIndex++;
                    }
                    this.mitemshistory.Focus();
                    this.ScrollIntoHistoryView();
                    this.SetTextFromHistory();
                }
            }
        }

        /// <summary>
        /// This method works up specific key for auto-complete in
        /// auto-append case.
        /// </summary>
        /// <param name="key">Pressed key.</param>
        /// <returns>
        /// Value indicating whether control's text was worked up.
        /// </returns>
        private bool WorkUpAutoAppend(Key key)
        {
            bool returnValue = false;
            if (this.IsAutoAppend)
            {
                if (Key.Enter == key)
                {
                    if (0 < this.mtextbox.SelectionLength)
                    {
                        this.mtextbox.SelectionStart = this.mtextbox.Text.Length;
                        this.IsDropDownOpen = false;
                        this.IsHistoryDropDownOpen = false;
                        returnValue = true;
                    }
                }
                else if (Key.Back == key)
                {
                    int caretIndex = this.mtextbox.SelectionStart;
                    string newText = string.Empty;
                    int totalTextLength = this.mtextbox.Text.Length;
                    if (this.IsEndTextSelected())
                    {
                        newText = this.mtextbox.Text.Remove(this.mtextbox.SelectionStart, totalTextLength - this.mtextbox.SelectionStart);
                    }

                    if (!string.IsNullOrEmpty(newText))
                    {
                        this.SetNewTextAfterCutDown(newText);
                        returnValue = true;
                    }
                }
                else if (Key.Delete == key)
                {
                    int caretIndex = this.mtextbox.SelectionStart;
                    int totalTextLength = this.mtextbox.Text.Length;

                    if (!string.IsNullOrEmpty(this.mtextbox.SelectedText))
                    {
                        string inputString = this.mtextbox.Text.Remove(this.mtextbox.SelectionStart, this.mtextbox.SelectionLength);
                        this.SetSafeTextWithMoveCaret(inputString);
                        returnValue = true;
                    }
                    else if (0 < caretIndex && totalTextLength - 1 == caretIndex)
                    {
                        this.SetNewTextAfterCutDown(this.mtextbox.Text.Remove(caretIndex, totalTextLength - caretIndex));
                        returnValue = true;
                    }
                }
                else if (Key.Tab == key && this.IsEndTextSelected())
                {
                    this.mtextbox.SelectionStart = this.mtextbox.Text.Length;
                    returnValue = true;
                }
                else if (Key.X == key && 0 < this.mtextbox.SelectionLength && (Keyboard.Modifiers == ModifierKeys.Control))
                {
                    string newText = this.mtextbox.Text.Remove(this.mtextbox.SelectionStart, this.mtextbox.SelectionLength);
                    this.SetNewTextAfterCutDown(newText);
                    returnValue = true;
                }
            }

            return returnValue;
        }

        /// <summary>
        /// This method sets the text selected from the history list. 
        /// </summary>
        private void SetTextFromHistory()
        {
            if (null != this.mitemshistory.SelectedItem)
            {
                this.SetSafeTextWithMoveCaret(this.mitemshistory.SelectedItem.ToString());
                this.mtextbox.SelectionStart = this.mtextbox.Text.Length;
            }
        }

        /// <summary>
        /// Shows auto-complete items after setting focus. 
        /// </summary>
        private void ShowItemsAfterSetFocus()
        {
            string inputText = this.mtextbox.Text;
            IAutocompleteLevel currentLevel = this.hashlevel ?? this.Root.GetRoot(inputText);
            if (null != currentLevel && inputText.StartsWith(currentLevel.GetFullPath(), StringComparison.OrdinalIgnoreCase))
            {
                this.SetItems(currentLevel, true);
            }
        }

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
            return 0 != this.mtextbox.SelectedText.Length && this.mtextbox.Text.EndsWith(this.mtextbox.SelectedText);
        }

        /// <summary>
        /// This method starts Vista-lake progress bar animation. 
        /// </summary>
        private void StartProgressAnimation()
        {
            ////if( null != m_VistaProgressBar )
            ////{
            ////    m_VistaProgressBar.StartProgressBarAnimation();
            ////}
        }

        /// <summary>
        /// This method stops Vista-lake progress bar animation. 
        /// </summary>
        private void StopProgressAnimation()
        {
            ////if( this.IsAsyncAddContent && null != m_VistaProgressBar )
            ////{
            ////    m_VistaProgressBar.StopProgressBarAnimation();
            ////}
        }
        #endregion

        #region History Related Methods

        /// <summary>
        /// This method gets all modes history.
        /// </summary>
        /// <returns>
        /// History for all modes. 
        /// </returns>
        private List<string> GetFullHistory()
        {
            List<string> historyList = new List<string>();
            using (var store = IsolatedStorageFile.GetUserStoreForApplication())
            {
                string filePath = Path.Combine("AutoCompleteInfo", this.StoreFileName);
                if (store.FileExists(filePath))
                {
                    try
                    {
                        using (StreamReader sr = new StreamReader(store.OpenFile(filePath, FileMode.Open, FileAccess.Read)))
                        {
                            string line;
                            //// Read lines from the file until the end of 
                            //// the file is reached.
                            while ((line = sr.ReadLine()) != null)
                            {
                                if (!this.historyList.Contains(line) && (this.CustomSourceString != null) && (!this.CustomSourceString.Contains(line)))
                                historyList.Add(line);
                            }
                        }
                    }
                    catch (IsolatedStorageException)
                    {
                    }
                }
            }

            return historyList;
        }

        /// <summary>
        /// This method saves history list.
        /// </summary>
        /// <param name="fullHistory">The list which should be saved.</param>
        private void SaveHistoryList(List<string> fullHistory)
        {
            using (var store = IsolatedStorageFile.GetUserStoreForApplication())
            {
                if (store.DirectoryExists("AutoCompleteInfo") == false)
                {
                    store.CreateDirectory("AutoCompleteInfo");
                }

                string filePath = Path.Combine("AutoCompleteInfo", this.StoreFileName);
                if (store.FileExists(filePath) == false)
                {
                    IsolatedStorageFileStream file = store.CreateFile(filePath);
                    file.Close();
                }

                try
                {
                    using (StreamWriter sr = new StreamWriter(store.OpenFile(filePath, FileMode.Open, FileAccess.Write)))
                    {
                        foreach (string entry in fullHistory)
                        {
                            sr.WriteLine(entry);
                        }
                    }
                }
                catch (IsolatedStorageException)
                {
                }
            }
        }

        /// <summary>
        /// This method saves history of the from the list using the full history.
        /// </summary>
        /// <remarks>
        /// The full history gets added only if it contains the historylist and it saves in
        /// history list.
        /// </remarks>
        public void SaveHistory()
        {
            List<string> fullHistory = this.GetFullHistory() ?? new List<string>();
            for (int i = 0, cnt = this.historyList.Count; i < cnt; ++i)
            {
                if (!fullHistory.Contains(this.historyList[i]))
                {
                    fullHistory.Add(this.historyList[i]);
                }
            }

            this.SaveHistoryList(fullHistory);
        }

        /// <summary>
        /// This method loads history in the history list.
        /// </summary>
        /// <remarks>
        /// The full history must not be empty, then only it will add full history and added
        /// in history list.
        /// </remarks>
        public void LoadHistory()
        {
            this.historyList.Clear();
            List<string> fullHistory = this.GetFullHistory();

            if (null != fullHistory)
            {
                for (int i = 0, count = fullHistory.Count; i < count; ++i)
                {
                    this.historyList.Add(fullHistory[i]);
                }               
            }
        }

        /// <summary>
        /// This method clears all history, and saves in History List.
        /// </summary>
        /// <remarks>
        /// First the history list will clear, then it will save into historylist.
        /// </remarks>
        public void ClearAllHistory()
        {
            this.historyList.Clear();
            this.SaveHistoryList(new List<string>());
        }

        /// <summary>
        /// This method adds input text to the history list and create root.
        /// </summary>
        /// <param name="inputText">The input text gets to check whether it is not contains
        /// or not.</param>
        public void AddHistory(string inputText)
        {
            if (this.IsHistoryEnabled == true)
            {
                //List<string> temp = new List<string>();
                //temp = this.CustomSourceString;
                //if (!temp.Contains(inputText))
                //{
                //    temp.Add(inputText);
                //    this.CustomSource = temp;
                //    this.CreateRoot();
                //}

                if (!(this.historyList.Contains(inputText) && this.CustomSourceString.Contains(inputText)))
                {
                    this.historyList.Add(inputText);
                }
            }
        }

        #endregion
    }
}