#region Copyright Syncfusion Inc. 2001 - 2014
// Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Any infringement will be prosecuted under
// applicable laws. 
#endregion
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Controls;
using System.ComponentModel;
using System.Windows;
using Syncfusion.Windows.Data;
using System.Windows.Controls.Primitives;
using Syncfusion.Linq;
using System.Runtime.InteropServices;
using System.Windows.Data;
using System.Windows.Input;
using System.Reflection;
using Syncfusion.Windows.Controls.Scroll;
using System.Windows.Media;
using System.Collections;
using Syncfusion.Windows.Shared;
using System.Globalization;
using System.Windows.Shapes;
using Syncfusion.Windows.ComponentModel;
#if SILVERLIGHT
using System.Windows.Browser;
//using Syncfusion.Windows.Controls.Scroll;
using Syncfusion.Windows.Tools.Controls;
#endif


namespace Syncfusion.Windows.Controls.Grid
{

#if SyncfusionFramework4_0
    [System.ComponentModel.DesignTimeVisible(false)]
#endif

    public class GridDataExcelLikeFilterPane : Control, IDisposable
    {
        #region Constants
        const double conHeight = 300;
        const double conWidth = 280;
        #endregion

        #region Private Members

        //This will set the ItemsSource for the ExcelLikeFilter
        internal List<FilterElement> FilterListBoxItem = new List<FilterElement>();
        //To maintain the filtered item after search close button is clicked.
        List<FilterElement> ListBoxItems = new List<FilterElement>();
        bool propertyChangedFromSelectAll = false;
        //this flag used to denote selectAll checkbox check/Uncheck from propertychanged
        bool SkipSelectAllCheckBoxEvents = false;
        internal bool isSourceChangedasSearchedItems = false;

        #endregion

        #region Constructor

        /// <summary>
        /// Initializes the <see cref="GridDataExcelLikeFilterPane"/> class.
        /// </summary>
        public GridDataExcelLikeFilterPane()
        {
            
            this.DefaultStyleKey = typeof(GridDataExcelLikeFilterPane);
#if SILVERLIGHT
            DependencyObjectExtensions.SetEnableMousePosition(Application.Current.RootVisual, true);
#endif
        }      

        #endregion

        internal string VisualStyle
        {
            get;
            set;
        }

        #region Dependency Properties  

        #region AdvanceFilteringOptionVisibilityProperty

        public static readonly DependencyProperty AdvanceFilteringOptionVisibilityProperty = DependencyProperty.Register(
         "AdvanceFilteringOptionVisibility",
         typeof(bool),
         typeof(GridDataExcelLikeFilterPane),
         new PropertyMetadata(true));


        /// <summary>
        /// Gets or sets a value indicating whether [advance filtering option visibility].
        /// </summary>
        /// <value>
        /// 	<c>true</c> if [advance filtering option visibility]; otherwise, <c>false</c>.
        /// </value>
        public bool AdvanceFilteringOptionVisibility
        {
            get
            {
                return (bool)this.GetValue(GridDataExcelLikeFilterPane.AdvanceFilteringOptionVisibilityProperty);
            }

            set
            {
                this.SetValue(GridDataExcelLikeFilterPane.AdvanceFilteringOptionVisibilityProperty, value);
            }
        }

        #endregion
   
        #region AscendingSortStringProperty
   
        public static readonly DependencyProperty AscendingSortStringProperty = DependencyProperty.Register(
         "AscendingSortString",
         typeof(String),
         typeof(GridDataExcelLikeFilterPane),
         new PropertyMetadata(""));

        /// <summary>
        /// Gets or sets the ascending sort string.
        /// </summary>
        /// <value>The ascending sort string.</value>
        public String AscendingSortString
        {
            get
            {
                return (String)this.GetValue(GridDataExcelLikeFilterPane.AscendingSortStringProperty);
            }

            set
            {
                this.SetValue(GridDataExcelLikeFilterPane.AscendingSortStringProperty, value);
            }
        }

        #endregion

        #region ClearFilterEnableProperty

        public static readonly DependencyProperty ClearFilterEnableProperty = DependencyProperty.Register(
              "ClearFilterEnable",
              typeof(bool),
              typeof(GridDataExcelLikeFilterPane),
              new PropertyMetadata(true));

        /// <summary>
        /// Gets or sets a value indicating whether [clear filter enable].
        /// </summary>
        /// <value><c>true</c> if [clear filter enable]; otherwise, <c>false</c>.</value>
        public bool ClearFilterEnable
        {
            get
            {

                return (bool)this.GetValue(GridDataExcelLikeFilterPane.ClearFilterEnableProperty);
            }
            set
            {
                this.SetValue(GridDataExcelLikeFilterPane.ClearFilterEnableProperty, value);
            }
        }

        #endregion

        #region ClearFilterVisibilityProperty

        public static readonly DependencyProperty ClearFilterVisibilityProperty = DependencyProperty.Register(
      "ClearFilterVisibility",
      typeof(bool),
      typeof(GridDataExcelLikeFilterPane),
      new PropertyMetadata(true));

        /// <summary>
        /// Gets or sets a value indicating whether [clear filter visibility].
        /// </summary>
        /// <value>
        /// 	<c>true</c> if [clear filter visibility]; otherwise, <c>false</c>.
        /// </value>
        public bool ClearFilterVisibility
        {
            get
            {
                return (bool)this.GetValue(GridDataExcelLikeFilterPane.ClearFilterVisibilityProperty);
            }

            set
            {
                this.SetValue(GridDataExcelLikeFilterPane.ClearFilterVisibilityProperty, value);
            }
        }

        #endregion      

        #region ColumnNameProperty

        public static readonly DependencyProperty ColumnNameProperty = DependencyProperty.Register(
           "ColumnName",
           typeof(string),
           typeof(GridDataExcelLikeFilterPane),
           new PropertyMetadata(""));

        /// <summary>
        /// Gets or sets a value indicating whether [clear filter enable].
        /// </summary>
        /// <value><c>true</c> if [clear filter enable]; otherwise, <c>false</c>.</value>
        public string ColumnName
        {
            get
            {
                return this.GetValue(GridDataExcelLikeFilterPane.ColumnNameProperty).ToString();
            }
            set
            {
                this.SetValue(GridDataExcelLikeFilterPane.ColumnNameProperty, value);
            }
        }

        #endregion

        #region ColumnTypeProperty

        public static readonly DependencyProperty ColumnTypeProperty = DependencyProperty.Register(
            "ColumnType",
            typeof(string),
            typeof(GridDataExcelLikeFilterPane),
            new PropertyMetadata("Text"));


        /// <summary>
        /// Gets or sets the type of the column.
        /// </summary>
        /// <value>The type of the column.</value>
        public string ColumnType
        {
            get
            {
                return this.GetValue(GridDataExcelLikeFilterPane.ColumnTypeProperty).ToString();
            }
            set
            {
                this.SetValue(GridDataExcelLikeFilterPane.ColumnTypeProperty, value);
            }
        }

        #endregion

        #region DescendingSortStringProperty     

        public static readonly DependencyProperty DescendingSortStringProperty = DependencyProperty.Register(
        "DescendingSortString",
        typeof(String),
        typeof(GridDataExcelLikeFilterPane),
        new PropertyMetadata(""));

        /// <summary>
        /// Gets or sets the descending sort string.
        /// </summary>
        /// <value>The descending sort string.</value>
        public String DescendingSortString
        {
            get
            {
                return (String)this.GetValue(GridDataExcelLikeFilterPane.DescendingSortStringProperty);
            }

            set
            {
                this.SetValue(GridDataExcelLikeFilterPane.DescendingSortStringProperty, value);
            }
        }

        #endregion

        #region ExcelLikeFilterAdvVisibilityProperty

        public static readonly DependencyProperty ExcelLikeFilterAdvVisibilityProperty = DependencyProperty.Register(
    "ExcelLikeFilterAdvVisibility",
    typeof(bool),
    typeof(GridDataExcelLikeFilterPane),
    new PropertyMetadata(false));


        /// <summary>
        /// Gets or sets a value indicating whether [excel like filter adv visibility].
        /// </summary>
        /// <value>
        /// 	<c>true</c> if [excel like filter adv visibility]; otherwise, <c>false</c>.
        /// </value>
        public bool ExcelLikeFilterAdvVisibility
        {
            get
            {
                return (bool)this.GetValue(GridDataExcelLikeFilterPane.ExcelLikeFilterAdvVisibilityProperty);
            }

            set
            {
                this.SetValue(GridDataExcelLikeFilterPane.ExcelLikeFilterAdvVisibilityProperty, value);
            }
        }

        #endregion           

        #region ItemsTemplate
        /// <summary>
        /// Gets or Sets the Suggested Datatemplate of ItemsControl
        /// </summary>
        public DataTemplate ItemsTemplate
        {
            get { return (DataTemplate)GetValue(ItemsTemplateProperty); }
            set { SetValue(ItemsTemplateProperty, value); }
        }

        ///<summary>
        ///Dependency Property Registeration for ItemsTemplate
        /// </summary>
        // Using a DependencyProperty as the backing store for ItemsTemplate.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty ItemsTemplateProperty =
            DependencyProperty.Register("ItemsTemplate", typeof(DataTemplate), typeof(GridDataExcelLikeFilterPane), new PropertyMetadata(null));
        
        #endregion

        #region HeightProperty

        /// <summary>
        /// Dependency property Registration for Height
        /// </summary>
        public new static readonly DependencyProperty HeightProperty =
            DependencyProperty.Register("Height", typeof(double), typeof(GridDataExcelLikeFilterPane), new PropertyMetadata(null));

        /// <summary>
        /// Gets or sets the suggested height of the element.
        /// </summary>
        /// <value></value>
        /// <returns>The height of the element, in device-independent units (1/96th inch per unit). The default value is <see cref="F:System.Double.NaN"/>. This value must be equal to or greater than 0.0. See Remarks for upper bound information.</returns>
        public new double Height
        {
            get { return (double)GetValue(HeightProperty); }
            set { SetValue(HeightProperty, value); }
        }

        #endregion

        #region ItemsSourceProperty

        /// <summary>
        /// Dependency property Registration for ItemsSource
        /// </summary>
        public static readonly DependencyProperty ItemsSourceProperty = DependencyProperty.Register(
          "ItemsSource",
          typeof(object),
          typeof(GridDataExcelLikeFilterPane),
          new PropertyMetadata(null));

        /// <summary>
        /// Gets or sets the items source.
        /// </summary>
        /// <value>The items source.</value>
        public Object ItemsSource
        {
            get
            {
                return (object)this.GetValue(GridDataExcelLikeFilterPane.ItemsSourceProperty);
            }
            set
            {
                this.SetValue(GridDataExcelLikeFilterPane.ItemsSourceProperty, value);
            }
        }

        #endregion

        #region IsOpenProperty

        /// <summary>
        /// Dependency property Registration for IsOpen
        /// </summary>
        public static readonly DependencyProperty IsOpenProperty = DependencyProperty.Register(
              "IsOpen",
              typeof(object),
              typeof(GridDataExcelLikeFilterPane),
              new PropertyMetadata(false));

        /// <summary>
        /// Gets or sets a value indicating whether this instance is open.
        /// </summary>
        /// <value><c>true</c> if this instance is open; otherwise, <c>false</c>.</value>
        public bool IsOpen
        {
            get
            {
                return (bool)this.GetValue(GridDataExcelLikeFilterPane.IsOpenProperty);
            }
            set
            {
                // if(this.ItemsSource!=null)
                this.SetValue(GridDataExcelLikeFilterPane.IsOpenProperty, value);
            }
        }

        #endregion

        #region OkCancelButtonVisibilityProperty

        public static readonly DependencyProperty OkCancelButtonVisibilityProperty = DependencyProperty.Register(
         "OkCancelButtonVisibility",
         typeof(bool),
         typeof(GridDataExcelLikeFilterPane),
         new PropertyMetadata(true));


        /// <summary>
        /// Gets or sets a value indicating whether [ok cancel button visibility].
        /// </summary>
        /// <value>
        /// 	<c>true</c> if [ok cancel button visibility]; otherwise, <c>false</c>.
        /// </value>
        public bool OkCancelButtonVisibility
        {
            get
            {
                return (bool)this.GetValue(GridDataExcelLikeFilterPane.OkCancelButtonVisibilityProperty);
            }

            set
            {
                this.SetValue(GridDataExcelLikeFilterPane.OkCancelButtonVisibilityProperty, value);
            }
        }

        #endregion

        #region ResizingOptionVisibilityProperty

        public static readonly DependencyProperty ResizingOptionVisibilityProperty = DependencyProperty.Register(
    "ResizingOptionVisibility",
    typeof(bool),
    typeof(GridDataExcelLikeFilterPane),
    new PropertyMetadata(true));


        /// <summary>
        /// Gets or sets a value indicating whether [resizing option visibility].
        /// </summary>
        /// <value>
        /// 	<c>true</c> if [resizing option visibility]; otherwise, <c>false</c>.
        /// </value>
        public bool ResizingOptionVisibility
        {
            get
            {
                return (bool)this.GetValue(GridDataExcelLikeFilterPane.ResizingOptionVisibilityProperty);
            }

            set
            {
                this.SetValue(GridDataExcelLikeFilterPane.ResizingOptionVisibilityProperty, value);
            }
        }

        #endregion

        #region SearchOptionVisibilityProperty

        public static readonly DependencyProperty SearchOptionVisibilityProperty = DependencyProperty.Register(
           "SearchOptionVisibility",
           typeof(bool),
           typeof(GridDataExcelLikeFilterPane),
           new PropertyMetadata(true));


        public bool SearchOptionVisibility
        {
            get
            {
                return (bool)this.GetValue(GridDataExcelLikeFilterPane.SearchOptionVisibilityProperty);
            }

            set
            {
                this.SetValue(GridDataExcelLikeFilterPane.SearchOptionVisibilityProperty, value);
            }
        }

        #endregion

        #region SelectedItemProperty

        public static readonly DependencyProperty SelectedItemProperty = DependencyProperty.Register(
             "SelectedItem",
             typeof(List<FilterElement>),
             typeof(GridDataExcelLikeFilterPane),
             new PropertyMetadata(null));


        /// <summary>
        /// Gets or sets the selected item.
        /// </summary>
        /// <value>The selected item.</value>
        public List<FilterElement> SelectedItem
        {
            get
            {
                return (List<FilterElement>)this.GetValue(GridDataExcelLikeFilterPane.SelectedItemProperty);
            }
            set
            {

                this.SetValue(GridDataExcelLikeFilterPane.SelectedItemProperty, value);
            }
        }

        #endregion

        #region SortOptionVisibilityProperty

        public static readonly DependencyProperty SortOptionVisibilityProperty = DependencyProperty.Register(
          "SortOptionVisibility",
          typeof(bool),
          typeof(GridDataExcelLikeFilterPane),
          new PropertyMetadata(true));


        /// <summary>
        /// Gets or sets a value indicating whether [sort option visibility].
        /// </summary>
        /// <value>
        /// 	<c>true</c> if [sort option visibility]; otherwise, <c>false</c>.
        /// </value>
        public bool SortOptionVisibility
        {
            get
            {
                return (bool)this.GetValue(GridDataExcelLikeFilterPane.SortOptionVisibilityProperty);
            }

            set
            {
                this.SetValue(GridDataExcelLikeFilterPane.SortOptionVisibilityProperty, value);
            }
        }

        #endregion

        #region SortOrderProperty
       
        public static readonly DependencyProperty SortOrderProperty = DependencyProperty.Register(
          "SortOrder",
          typeof(String),
          typeof(GridDataExcelLikeFilterPane),
          new PropertyMetadata(""));
        
        public String SortOrder
        {
            get
            {
                return (String)this.GetValue(GridDataExcelLikeFilterPane.SortOrderProperty);
            }

            set
            {
                this.SetValue(GridDataExcelLikeFilterPane.SortOrderProperty, value);
            }
        }

        #endregion           

        #region StaysOpenProperty       

        public static readonly DependencyProperty StaysOpenProperty = DependencyProperty.Register(
         "StaysOpen",
         typeof(bool),
         typeof(GridDataExcelLikeFilterPane),
         new PropertyMetadata(true));


        /// <summary>
        /// Gets or sets a value indicating whether [stays open].
        /// </summary>
        /// <value><c>true</c> if [stays open]; otherwise, <c>false</c>.</value>
        public bool StaysOpen
        {
            get
            {
                return (bool)this.GetValue(GridDataExcelLikeFilterPane.StaysOpenProperty);
            }

            set
            {
                this.SetValue(GridDataExcelLikeFilterPane.StaysOpenProperty, value);
            }
        }

        #endregion      

        #region WidthProperty       

        /// <summary>
        /// Dependency property Registration for Width
        /// </summary>
        public new static readonly DependencyProperty WidthProperty =
            DependencyProperty.Register("Width", typeof(double), typeof(GridDataExcelLikeFilterPane), new PropertyMetadata(conWidth));

        /// <summary>
        /// Gets or sets the width of the element.
        /// </summary>
        /// <value></value>
        /// <returns>The width of the element, in device-independent units (1/96th inch per unit). The default value is <see cref="F:System.Double.NaN"/>. This value must be equal to or greater than 0.0. See Remarks for upper bound information.</returns>
        public new double Width
        {
            get { return (double)GetValue(WidthProperty); }
            set { SetValue(WidthProperty, value); }
        }

        #endregion

        #endregion

        #region UIElements
#if !SILVERLIGHT

        /// <summary>
        /// Gets or sets the clear filter menu item.
        /// </summary>
        /// <value>The clear filter menu item.</value>
        private MenuItem ClearFilterMenuItem
        {
            get;
            set;
        }
#else
       

        private MenuItemAdv PART_SortDescending
        {
            get;
            set;
        }

        private MenuItemAdv PART_SortAscending
        {
            get;
            set;
        }
        /// <summary>
        /// Gets or sets the clear filter menu item.
        /// </summary>
        /// <value>The clear filter menu item.</value>
        private MenuItemAdv ClearFilterMenuItem
        {
            get;
            set;
        }
#endif     

        /// <summary>
        /// Gets or sets the ok button.
        /// </summary>
        /// <value>The ok button.</value>
        private Button OkButton
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the cancel button.
        /// </summary>
        /// <value>The cancel button.</value>
        private Button CancelButton
        {
            get;
            set;
        }

#if !SILVERLIGHT

        /// <summary>
        /// Gets or sets the advance filter menuitem.
        /// </summary>
        /// <value>The advance filter menuitem.</value>
        private MenuItem AdvanceFilterMenuitem
        {
            get;
            set;
        }
#endif

        /// <summary>
        /// Gets or sets the select all check box.
        /// </summary>
        /// <value>The select all check box.</value>
        CheckBox SelectAllCheckBox
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the resizing thumb.
        /// </summary>
        /// <value>The resizing thumb.</value>
        Thumb ResizingThumb
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the search text box.
        /// </summary>
        /// <value>The search text box.</value>
        TextBox SearchTextBox
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the search button.
        /// </summary>
        /// <value>The search button.</value>
        Button SearchButton
        {
            get;
            set;
        } 
       
        /// <summary>
        /// Gets or sets the filter pop up.
        /// </summary>
        /// <value>The filter pop up.</value>
        internal Popup FilterPopUp
        {
            get;
            set;
        }

        #region SILVERLIGHT
      
#if SILVERLIGHT

        Border PART_FilterPopUpBorder
        {
            get;
            set;
        }

        MenuItemAdv AdvanceFiltering
        {
            get;
            set;
        }

        Border FilterText
        {
            get;
            set;
        }

        Popup FilterTextPopup
        {
            get;
            set;
        }
#endif
        #endregion

        #endregion

        #region Overridden Methods

        /// <summary>
        /// When overridden in a derived class, is invoked whenever application code or internal processes call <see cref="M:System.Windows.FrameworkElement.ApplyTemplate"/>.
        /// </summary>
        public override void OnApplyTemplate()
        {
#if !SILVERLIGHT
            this.Focusable = false;
#endif
            this.Loaded += new RoutedEventHandler(GridDataExcelLikeFilterPane_Loaded);           
            VisualContainer.SetWantsMouseInput(this, true);
            
            base.OnApplyTemplate();

            
            

            /// Fecthing the UIElements from the template
#if !SILVERLIGHT           
           
            this.AdvanceFilterMenuitem = this.GetTemplateChild("AdvanceFilter") as MenuItem;
#else         
           
            this.PART_FilterPopUpBorder = this.GetTemplateChild("PART_FilterPopUpBorder") as Border;
#endif      
            SelectAllCheckBox = this.GetTemplateChild("PART_CheckBox") as CheckBox;
            this.ResizingThumb = this.GetTemplateChild("PART_ResizingThumb") as Thumb;
            this.SearchTextBox = this.GetTemplateChild("Part_SearchTextBox") as TextBox;
            this.SearchButton = this.GetTemplateChild("PART_SearchButton") as Button;            
#if !SILVERLIGHT
            ClearFilterMenuItem = this.GetTemplateChild("PART_MenuItem") as MenuItem;
#else

            ClearFilterMenuItem = this.GetTemplateChild("PART_MenuItem") as MenuItemAdv;
            this.PART_SortAscending = this.GetTemplateChild("PART_SortAscending") as MenuItemAdv;
            this.PART_SortDescending = this.GetTemplateChild("PART_SortDescending") as MenuItemAdv;
#endif

            SelectAllCheckBox = this.GetTemplateChild("PART_CheckBox") as CheckBox;
            FilterPopUp = this.GetTemplateChild("PART_FilterPopup") as Popup;            
            OkButton = this.GetTemplateChild("PART_OkButton") as Button;
            CancelButton = this.GetTemplateChild("PART_CancelButton") as Button;          
           
#if SILVERLIGHT
            
#endif
            if (this.ColumnType == GridDataResourceWrapper.DateFilters)
            {
                this.AscendingSortString = GridDataResourceWrapper.DateTimeAscending;
                this.DescendingSortString = GridDataResourceWrapper.DateTimeDescending;
            }
            else if (this.ColumnType == GridDataResourceWrapper.TextFilters)
            {
                this.AscendingSortString = GridDataResourceWrapper.StringAscending;
                this.DescendingSortString = GridDataResourceWrapper.StringDescending;
            }
            else if (this.ColumnType ==GridDataResourceWrapper.NumberFilters)
            {
                this.AscendingSortString = GridDataResourceWrapper.NumberAscending;
                this.DescendingSortString = GridDataResourceWrapper.NumberDescending;
            }           
        }

        void GridDataExcelLikeFilterPane_Loaded(object sender, RoutedEventArgs e)
        {
            WireEvents();
        }
        #endregion

        #region Searrch TextBox
        /// <summary>
        /// Called when [search button click].
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The <see cref="System.Windows.RoutedEventArgs"/> instance containing the event data.</param>
        void OnClearSearchButtonClick(object sender, RoutedEventArgs e)
        {
            this.SearchTextBox.Text = "";
            //While clicking SearchBox clear button every item in the pane is to select (MS-Excel Behavior).
            if (this.ClearFilterEnable == false)
            {
                foreach (var item in this.FilterListBoxItem)
                {
                    if (item.IsSelected == false)
                    {
                        item.IsSelected = true;
                    }
                }
            }
            else
            {
                //If the search close button is clicked when the filter is applied
                this.FilterListBoxItem.Clear();
                this.FilterListBoxItem = this.ListBoxItems;
            }
            this.ItemsSource = this.FilterListBoxItem;
            var unCheckedItem = this.FilterListBoxItem.FirstOrDefault(x => x.IsSelected == false);
            this.SkipSelectAllCheckBoxEvents = true;
            if (unCheckedItem == null)
            {
                this.SelectAllCheckBox.IsChecked = true;
            }
            else
            {
                this.SelectAllCheckBox.IsChecked = null;
            }
            this.OkButton.IsEnabled = true;
            this.SkipSelectAllCheckBoxEvents = false;

        }

        /// <summary>
        /// Called when [search text box text changed].
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The <see cref="System.Windows.Controls.TextChangedEventArgs"/> instance containing the event data.</param>
        void OnSearchTextBoxTextChanged(object sender, TextChangedEventArgs e)
        {
            var textBox = sender as TextBox;
            textBox.Focus();
            Predicate<FilterElement> predicate = new Predicate<FilterElement>(
               delegate(FilterElement input)
               {
                   return input.Name.ToLower().ToString().Contains(textBox.Text.ToLower());
               }
           );
            if (this.FilterListBoxItem == null)
                return;

#if SILVERLIGHT
            var searchedItems = this.FilterListBoxItem.Where(x => x.Name.ToLower().Contains(textBox.Text.ToLower())).ToList<FilterElement>();

#else
            var searchedItems = this.FilterListBoxItem.FindAll(predicate);
           
#endif
            if (textBox.Text != "")
            {
                propertyChangedFromSelectAll = true;
                searchedItems.ToList<FilterElement>().ForEach(c => c.IsSelected = true);
                propertyChangedFromSelectAll = false;
                this.SkipSelectAllCheckBoxEvents = true;
                this.SelectAllCheckBox.IsChecked = true;
                this.SkipSelectAllCheckBoxEvents = false;
            }
            //when the SearchBox text is empty then every item in filterpane is selected (MS-Excel Behavior).
            else if (textBox.Text == "")
            {
                if (this.ClearFilterEnable == false)// if the filter is applied not to check all items.
                {
                    foreach (var item in searchedItems)
                    {
                        if (item.IsSelected == false)
                        {
                            item.IsSelected = true;
                        }
                    }
                    this.SkipSelectAllCheckBoxEvents = true;
                    this.SelectAllCheckBox.IsChecked = true;
                    this.SkipSelectAllCheckBoxEvents = false;
                }
                else
                {
                    // if the search box value is cleared we need to maintain the filtered value.
                    searchedItems.Clear();
                    searchedItems = this.ListBoxItems;
                    // we have assigned the filterd value to FilterListBoxItem.
                    this.FilterListBoxItem = this.ListBoxItems;
                }
            }
            this.ItemsSource = searchedItems;//.Select(c=>c.Clone()).ToList<FilterElement>();
            if (searchedItems.Count() == 0)
                this.OkButton.IsEnabled = false;
            else
                this.OkButton.IsEnabled = true;
        }

        #endregion      

        #region Wire/UnWire Events

        /// <summary>
        /// Wires the events.
        /// </summary>
        private void WireEvents()
        {

            if (this.SearchTextBox != null)
            {
                this.SearchTextBox.KeyDown += new KeyEventHandler(SearchTextBox_KeyDown);
                this.SearchTextBox.TextChanged += new TextChangedEventHandler(OnSearchTextBoxTextChanged);
            }   

            if (this.SearchButton != null)
            {
                this.SearchButton.Click += new RoutedEventHandler(OnClearSearchButtonClick);
            }

            if (ResizingThumb != null)
            {
                ResizingThumb.DragDelta += ResizingThumbDragDelta;
            }

            if (ClearFilterMenuItem != null)
            {
                this.ClearFilterMenuItem.Click += OnClearFilterClick;
            }

            if (FilterPopUp != null)
            {
                FilterPopUp.Opened += OnFilterPopUpOpened;                
            }

            if (SelectAllCheckBox != null)
            {
                SelectAllCheckBox.Checked += OnSelectAllChecked;
                SelectAllCheckBox.Unchecked += OnSelectAllUnchecked;
            }

            if (OkButton != null)
            {
                OkButton.Click += OnOkButtonClick;
            }

            if (CancelButton != null)
            {
                CancelButton.Click += OnCancelButtonClick;
            }

#if !SILVERLIGHT

            if (this.AdvanceFilterMenuitem != null)
            {
                this.AdvanceFilterMenuitem.MouseMove += new MouseEventHandler(OnAdvanceFilterMenuitemMouseMove);
               this.AdvanceFilterMenuitem.MouseLeave += new MouseEventHandler(OnAdvanceFilterMenuitemMouseLeave);
            }   
#else
            if (this.PART_SortDescending != null)
            {
                this.PART_SortDescending.MouseEnter += new MouseEventHandler(OnMenuItemMouseEnter);
                this.PART_SortDescending.MouseLeave += new MouseEventHandler(OnMenuItemMouseLeave);
            }

            if (this.PART_SortAscending != null)
            {
                this.PART_SortAscending.MouseEnter += new MouseEventHandler(OnMenuItemMouseEnter);
                this.PART_SortAscending.MouseLeave += new MouseEventHandler(OnMenuItemMouseLeave);
            }

            if (this.ClearFilterMenuItem != null)
            {
                this.ClearFilterMenuItem.MouseEnter+=new MouseEventHandler(OnMenuItemMouseEnter);
                this.ClearFilterMenuItem.MouseLeave += new MouseEventHandler(OnMenuItemMouseLeave);
            }

#endif

            this.Unloaded += OnFilterDropDownUnloaded;
        }

        void SearchTextBox_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Escape)
            {
                var textBox = sender as TextBox;
                textBox.Text = "";
                e.Handled = true;
            }
            else if(e.Key==Key.Enter)
            {
                ProcessOkButtonClick();
                this.IsOpen = false;
                e.Handled = true;
            }
        }

        void UnHookFilterElementPropertyChanged(List<FilterElement> filterList)
        {
            if (filterList == null)
                return;
            foreach (var item in filterList)
            {
                item.PropertyChanged -= new PropertyChangedEventHandler(this.OnFilterElementPropertyChanged);
            }
        }
       

        void OnMenuItemMouseEnter(object sender, MouseEventArgs e)
        {
            var MenuItem = sender as MenuItemAdv;
            VisualStateManager.GoToState(MenuItem, "MenuItemFocused", false);

        }

        void OnMenuItemMouseLeave(object sender, MouseEventArgs e)
        {
            var MenuItem = sender as MenuItemAdv;
            VisualStateManager.GoToState(MenuItem, "Normal", false);
        }

        /// <summary>
        /// Wires the events.
        /// </summary>
        private void UnWireEvents()
        {
            if (this.SearchTextBox != null)
            {
                this.SearchTextBox.TextChanged -= new TextChangedEventHandler(OnSearchTextBoxTextChanged);
            }

            if (this.SearchButton != null)
            {
                this.SearchButton.Click -= new RoutedEventHandler(OnClearSearchButtonClick);
            }         

            if (ResizingThumb != null)
            {
                ResizingThumb.DragDelta -= ResizingThumbDragDelta;
            }

            if (ClearFilterMenuItem != null)
            {
                this.ClearFilterMenuItem.Click -= OnClearFilterClick;
            }

            if (FilterPopUp != null)
            {
                FilterPopUp.Opened -= OnFilterPopUpOpened;
            }

            if (SelectAllCheckBox != null)
            {
                SelectAllCheckBox.Checked -= OnSelectAllChecked;
                SelectAllCheckBox.Unchecked -= OnSelectAllUnchecked;
            }

            if (OkButton != null)
            {
                OkButton.Click -= OnOkButtonClick;
            }

            if (CancelButton != null)
            {
                CancelButton.Click -= OnCancelButtonClick;
            }

#if !SILVERLIGHT

            if (this.AdvanceFilterMenuitem != null)
            {
                this.AdvanceFilterMenuitem.MouseMove -= new MouseEventHandler(OnAdvanceFilterMenuitemMouseMove);
                this.AdvanceFilterMenuitem.MouseLeave -= new MouseEventHandler(OnAdvanceFilterMenuitemMouseLeave);
            }  

#endif

            this.Unloaded -= OnFilterDropDownUnloaded;
        }

        /// <summary>
        /// Called when [filter drop down unloaded].
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The <see cref="System.Windows.RoutedEventArgs"/> instance containing the event data.</param>
        void OnFilterDropDownUnloaded(object sender, RoutedEventArgs e)
        {
            this.UnWireEvents();
        }

        #endregion
#if !SILVERLIGHT

        #region AdvanceFilterMenuitem Events

        //This Variable Store Background and Stroke Color of the MenuItem Selection Rectangle.
        Brush BackgroundBrush, StrokeColor,MenuItemForeground;

        /// <summary>
        /// Called when [advance filter menuitem mouse move].
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The <see cref="System.Windows.Input.MouseEventArgs"/> instance containing the event data.</param>

        void OnAdvanceFilterMenuitemMouseMove(object sender, MouseEventArgs e)
        {
           
            if (this.VisualStyle != "ShinyBlue" && this.VisualStyle != "ShinyRed" && this.VisualStyle != "Default" && this.VisualStyle != "Windows7")
            {
                var menuItem = sender as MenuItem;
                if (MenuItemForeground == null)
                {
                    MenuItemForeground = menuItem.Foreground;
                }
                menuItem.Foreground = MenuItemForeground;
                var backgroundRec = this.AdvanceFilterMenuitem.FindElementsOfType<Rectangle>();
                var backGroundRect = backgroundRec.FirstOrDefault(x => x != null && x.Name == "Bg");
                var innerBorder = backgroundRec.FirstOrDefault(x => x != null && x.Name == "InnerBorder");
                if (backGroundRect != null)
                {
                    if (BackgroundBrush != null)
                        backGroundRect.Fill = BackgroundBrush;
                    if (StrokeColor != null)
                        backGroundRect.Stroke = StrokeColor;
                    StrokeColor = backGroundRect.Stroke;
                    BackgroundBrush = backGroundRect.Fill;
                }
                if (innerBorder != null)
                    innerBorder.Visibility = System.Windows.Visibility.Visible;
            }
            else
            {
                var menuItem = sender as MenuItem;
                var backgroundRec = this.AdvanceFilterMenuitem.FindElementsOfType<Border>();
                var backGroundRect = backgroundRec.FirstOrDefault(x => x != null && x.Name == "border");
                if (backGroundRect == null)
                    return;
                if (BackgroundBrush == null)
                {
                    BackgroundBrush = backGroundRect.Background;    
                }
                if (MenuItemForeground == null)
                {
                    MenuItemForeground = menuItem.Foreground;
                }
                menuItem.Foreground = MenuItemForeground;
                backGroundRect.Background = BackgroundBrush;              

            }
            this.AdvanceFilterMenuitem.IsSubmenuOpen = true;

        }

        /// <summary>
        /// Called when [advance filter menuitem mouse leave].
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The <see cref="System.Windows.Input.MouseEventArgs"/> instance containing the event data.</param>
        void OnAdvanceFilterMenuitemMouseLeave(object sender, MouseEventArgs e)
        {
             var visualStyle = SkinStorage.GetVisualStyle(this);
             if (this.VisualStyle != "ShinyBlue" && this.VisualStyle != "ShinyRed" && this.VisualStyle != "Default" && this.VisualStyle != "Windows7")
             {
                 var menuItem = sender as MenuItem;
                 var sortMenuItem = this.GetTemplateChild("PART_SortDescending") as MenuItem;
                 menuItem.Foreground = sortMenuItem.Foreground;
                 var backgroundRec = this.AdvanceFilterMenuitem.FindElementsOfType<Rectangle>();
                 var backGroundRect = backgroundRec.FirstOrDefault(x => x != null && x.Name == "Bg");
                 var innerBorder = backgroundRec.FirstOrDefault(x => x != null && x.Name == "InnerBorder");
                 if(backGroundRect != null)
                 {
                     BackgroundBrush = backGroundRect.Fill;
                     StrokeColor = backGroundRect.Stroke;
                     backGroundRect.Fill = Brushes.Transparent;
                     backGroundRect.Stroke = Brushes.Transparent;
                 }

                 if (innerBorder != null)
                     innerBorder.Visibility = System.Windows.Visibility.Collapsed;
                
             }
             else
             {
                 var menuItem = sender as MenuItem;
                 var backgroundRec = this.AdvanceFilterMenuitem.FindElementsOfType<Border>();
                 var backGroundRect = backgroundRec.FirstOrDefault(x => x != null && x.Name == "border");
                 if (backGroundRect == null)
                     return;
                 backGroundRect.Background = Brushes.Transparent;
                 var sortMenuItem = this.GetTemplateChild("PART_SortDescending") as MenuItem;
                 menuItem.Foreground = sortMenuItem.Foreground;
             }
             this.AdvanceFilterMenuitem.IsSubmenuOpen = false;
        }

        #endregion       

#endif

        #region Select All State Listner

        /// <summary>
        /// Called when [select all unchecked].
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The <see cref="System.Windows.RoutedEventArgs"/> instance containing the event data.</param>
        void OnSelectAllUnchecked(object sender, RoutedEventArgs e)
        {

            propertyChangedFromSelectAll = true;
            if (!SkipSelectAllCheckBoxEvents && this.FilterListBoxItem != null)
            {
                this.FilterListBoxItem.ToList<FilterElement>().ForEach(c => c.IsSelected = false);

                if (!this.ExcelLikeFilterAdvVisibility)
                {
                    this.RaiseSelectAllUnCheckBoxChecked(this.FilterListBoxItem);
                }
            }
            this.MaintainSelectAllCheckBox();
            propertyChangedFromSelectAll = false;
        }

        /// <summary>
        /// Called when [select all checked].
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The <see cref="System.Windows.RoutedEventArgs"/> instance containing the event data.</param>
        void OnSelectAllChecked(object sender, RoutedEventArgs e)
        {
            propertyChangedFromSelectAll = true;

            if (!SkipSelectAllCheckBoxEvents && this.FilterListBoxItem != null)
            {
                this.FilterListBoxItem.ToList<FilterElement>().ForEach(c => c.IsSelected = true);
                if (!this.ExcelLikeFilterAdvVisibility)
                {
                    this.RaiseSelectAllCheckBoxChecked(this.FilterListBoxItem);
                }
            }
            this.MaintainSelectAllCheckBox();
            propertyChangedFromSelectAll = false;
        }

        #endregion 

        #region SortMenuItem Click Command
        
        private DelegateCommand<object> _sortMenuCommand;

        /// <summary>
        /// Gets the sort menu command.
        /// </summary>
        /// <value>The sort menu command.</value>
        public DelegateCommand<object> SortMenuCommand
        {
            get
            {
                if (_sortMenuCommand == null)
                {
                    _sortMenuCommand = new DelegateCommand<object>(SortMenuClick, CanExceCute);
                }
                return _sortMenuCommand;
            }
        }

        public bool CanExceCute(object param)
        {
            return true;
        }

        /// <summary>
        /// Sorts the menu click.
        /// </summary>
        /// <param name="param">The param.</param>
        private void SortMenuClick(object param)
        {
            this.RaiseSortMenuItemClick(param.ToString());
            SortOrder = param.ToString();
            this.IsOpen = false;
        }

        #endregion

#if !SILVERLIGHT
        #region AdvanceFiltering SubMenuItem ClickCommand
        
        private DelegateCommand _clickCommand;

        /// <summary>
        /// Gets the click command.
        /// </summary>
        /// <value>The click command.</value>
        public DelegateCommand ClickCommand
        {
            get
            {
                if (_clickCommand == null)
                {
                    _clickCommand = new DelegateCommand(ClickMenuItem,CanExceCute);
                }
                return _clickCommand;
            }
        }


        bool CanExecute(object param)
        {
            return true;
        }
       

        /// <summary>
        /// Clicks the menu item.
        /// </summary>
        /// <param name="param">The param.</param>
        /// 
        AdvanceFilteringViewModel viewModel = new AdvanceFilteringViewModel();

        public void ClickMenuItem(object param)
        {
            if (this.ColumnType != GridDataResourceWrapper.DateFilters)
            {
                var tempViewModel = viewModel;
                if (param.ToString() != GridDataResourceWrapper.AboveAverage && param.ToString() != GridDataResourceWrapper.BelowAverage)
                {
                    if (param.ToString() != viewModel.FilterPredicateSelectedItem1)
                        viewModel = new AdvanceFilteringViewModel();
                    viewModel.FilterElement = this.FilterListBoxItem;
                    viewModel.ColumnName = this.ColumnName;
                    viewModel.ColumnType = this.ColumnType;
                    if (param.ToString() == GridDataResourceWrapper.Between)
                    {
                        viewModel.FilterPredicateSelectedItem1 = GridDataResourceWrapper.IsGreaterThanOrEqualto;
                        viewModel.FilterPredicateSelectedItem2 = GridDataResourceWrapper.IsLessThanorEqualto;
                    }
                    else
                        viewModel.FilterPredicateSelectedItem1 = param.ToString();
                    AdvanceFilteringWindow window = new AdvanceFilteringWindow(viewModel, VisualStyle);
                    if (this.VisualStyle == "Metro")
                    {
                        window.Resources.MergedDictionaries.Clear();
                        foreach (ResourceDictionary rd1 in this.Resources.MergedDictionaries)
                            window.Resources.MergedDictionaries.Add(rd1);
                    }
                    this.IsOpen = false;
                    bool? canProceed = window.ShowDialog();
                    if (canProceed != null && (bool)canProceed)
                        this.AdvanceFiltering(viewModel);
                    else
                        viewModel = tempViewModel;
                }
                else
                {
                    this.RaiseAdvanceFilteringOkButtonClick(null, null, null, null, param.ToString());
                }
            }
            else
            {
                if (new[] { GridDataResourceWrapper.isbefore, GridDataResourceWrapper.isafter, GridDataResourceWrapper.Between, GridDataResourceWrapper.EqualsSmall }.Contains(param.ToString()))
                {
                    var tempFilterPredicateSelectedItem1 = viewModel.FilterPredicateSelectedItem1;
                    if (param.ToString() != viewModel.FilterPredicateSelectedItem1)
                        viewModel = new AdvanceFilteringViewModel();
                    viewModel.FilterElement = this.FilterListBoxItem;
                    viewModel.ColumnName = this.ColumnName;
                    viewModel.ColumnType = this.ColumnType;
                    if (this.ColumnType == GridDataResourceWrapper.DateFilters && param.ToString() == GridDataResourceWrapper.Between)
                    {
                        viewModel.FilterPredicateSelectedItem1 = GridDataResourceWrapper.isafterorequalto;
                        viewModel.FilterPredicateSelectedItem2 = GridDataResourceWrapper.isbeforeorequalto;
                    }
                    else
                        viewModel.FilterPredicateSelectedItem1 = param.ToString();
                    var visualStyle = SkinStorage.GetVisualStyle(this);
                    AdvanceFilteringWindow window = new AdvanceFilteringWindow(viewModel, VisualStyle);

                    if (this.VisualStyle == "Metro")
                    {
                        window.Resources.MergedDictionaries.Clear();
                        foreach (ResourceDictionary rd1 in this.Resources.MergedDictionaries)
                            window.Resources.MergedDictionaries.Add(rd1);
                    }
                    this.IsOpen = false;
                    bool? canProceed = window.ShowDialog();
                    if (canProceed != null && (bool)canProceed)
                        this.AdvanceFiltering(viewModel);
                    else
                        viewModel.FilterPredicateSelectedItem1 = tempFilterPredicateSelectedItem1;
                }
                else
                {
                    this.AdvanceFilterMenuitem.StaysOpenOnClick = false;
                    bool EventRaised = false;
                    DateTime StartDate = DateTime.Today;
                    DateTime EndDate = DateTime.Today;
                    if (new[] { GridDataResourceWrapper.Today , GridDataResourceWrapper.Tomorrow, GridDataResourceWrapper.Yesterday }.Contains(param.ToString()))
                    {
                        var filterValue = DateTime.Today;
                        if (param.ToString() == GridDataResourceWrapper.Tomorrow)
                        {
                            filterValue = filterValue.Date.Add(new TimeSpan(1, 0, 0, 0));
                            EventRaised = true;
                        }
                        else if (param.ToString() == GridDataResourceWrapper.Yesterday)
                        {
                            filterValue = filterValue.Date.Subtract(new TimeSpan(1, 0, 0, 0));
                            EventRaised = true;
                        }
                        this.RaiseAdvanceFilteringOkButtonClick(filterValue, null, "equals", null, "And");

                    }

                    else if (new[] { GridDataResourceWrapper.ThisMonth, GridDataResourceWrapper.NextMonth, GridDataResourceWrapper.LastMonth }.Contains(param.ToString()))
                    {
                        var today = DateTime.Today;
                        var daysInMonth = DateTime.DaysInMonth(today.Year, DateTime.Today.Month);
                        DateTime MonthStartDate = DateTime.Today.Date.Subtract(new TimeSpan(today.Day - 1, 0, 0, 0));
                        DateTime MonthEndDate = MonthStartDate.Date.Add(new TimeSpan((MonthStartDate.Day - 2 + daysInMonth), 0, 0, 0));
                        if (param.ToString() == GridDataResourceWrapper.LastMonth)
                        {
                            MonthEndDate = DateTime.Today.Date.Subtract(new TimeSpan(today.Day, 0, 0, 0));
                            daysInMonth = DateTime.DaysInMonth(MonthEndDate.Year, MonthEndDate.Month);
                            MonthStartDate = MonthEndDate.Date.Subtract(new TimeSpan(daysInMonth - 1, 0, 0, 0));
                        }
                        else if (param.ToString() == GridDataResourceWrapper.NextMonth)
                        {
                            MonthStartDate = MonthStartDate.Date.Add(new TimeSpan((MonthStartDate.Day - 1 + daysInMonth), 0, 0, 0));
                            daysInMonth = DateTime.DaysInMonth(MonthStartDate.Year, MonthStartDate.Month);
                            MonthEndDate = MonthStartDate.Date.Add(new TimeSpan((MonthStartDate.Day - 2 + daysInMonth), 0, 0, 0));
                        }

                        StartDate = MonthStartDate;
                        EndDate = MonthEndDate;
                    }

                    else if (new[] { GridDataResourceWrapper.ThisWeek , GridDataResourceWrapper.NextWeek, GridDataResourceWrapper.LastWeek }.Contains(param.ToString()))
                    {
                        var today = DateTime.Today;
                        var dayOfWeek = today.DayOfWeek.GetHashCode();
                        DateTime WeekStartDate = DateTime.Today.Date.Subtract(new TimeSpan(dayOfWeek, 0, 0, 0));
                        DateTime WeekEndDate = WeekStartDate.Date.Add(new TimeSpan(6, 0, 0, 0));

                        if (param.ToString() == GridDataResourceWrapper.LastWeek)
                        {
                            WeekStartDate = WeekStartDate.Subtract(new TimeSpan(7, 0, 0, 0));
                            WeekEndDate = WeekStartDate.Date.Add(new TimeSpan(6, 0, 0, 0));
                        }
                        else if (param.ToString() == GridDataResourceWrapper.NextWeek)
                        {
                            WeekStartDate = WeekEndDate.Add(new TimeSpan(1, 0, 0, 0));
                            WeekEndDate = WeekStartDate.Date.Add(new TimeSpan(6, 0, 0, 0));
                        }

                        StartDate = WeekStartDate;
                        EndDate = WeekEndDate;
                    }

                    else if (new[] { GridDataResourceWrapper.ThisYear , GridDataResourceWrapper.NextYear , GridDataResourceWrapper.LastYear }.Contains(param.ToString()))
                    {
                        var today = DateTime.Today;
                        var daysofYear = today.DayOfYear;
                        DateTime YearStartDate = DateTime.Today.Date.Subtract(new TimeSpan(daysofYear - 1, 0, 0, 0));
                        DateTime YearEndDate = DateTime.IsLeapYear(today.Year) ? YearStartDate.Date.Add(new TimeSpan(365, 0, 0, 0)) : YearStartDate.Date.Add(new TimeSpan(364, 0, 0, 0));

                        if (param.ToString() == GridDataResourceWrapper.LastYear)
                        {
                            YearEndDate = YearStartDate.Date.Subtract(new TimeSpan(1, 0, 0, 0));
                            YearStartDate = DateTime.IsLeapYear(YearEndDate.Year) ? YearEndDate.Date.Subtract(new TimeSpan(365, 0, 0, 0)) : YearEndDate.Date.Subtract(new TimeSpan(364, 0, 0, 0));

                        }
                        else if (param.ToString() == GridDataResourceWrapper.NextYear)
                        {
                            YearStartDate = YearEndDate.Add(new TimeSpan(1, 0, 0, 0));
                            YearEndDate = DateTime.IsLeapYear(YearStartDate.Year) ? YearStartDate.Date.Add(new TimeSpan(365, 0, 0, 0)) : YearStartDate.Date.Add(new TimeSpan(364, 0, 0, 0));

                        }
                        StartDate = YearStartDate;
                        EndDate = YearEndDate;
                    }
                    if (!EventRaised) /* To prevent event firing twice */
                    this.RaiseAdvanceFilteringOkButtonClick(StartDate, EndDate, GridDataResourceWrapper.IsGreaterThanOrEqualto, GridDataResourceWrapper.IsLessThanorEqualto, "And");
                }
            }
            this.IsOpen = false;
        }

        #endregion

        #region Filtering Method
        
        /// <summary>
        /// Advances the filtering.
        /// </summary>
        /// <param name="viewModel">The view model.</param>
        private void AdvanceFiltering(AdvanceFilteringViewModel viewModel)
        {
            string type = "And";
            if (viewModel.IsOrChecked)
                type = "Or";

            this.RaiseAdvanceFilteringOkButtonClick(viewModel.FilterValueSelectedItem1, viewModel.FilterValueSelectedItem2, viewModel.FilterPredicateSelectedItem1, viewModel.FilterPredicateSelectedItem2, type);
        }

        #endregion

#endif

        #region Popup State Listner
        /// <summary>
        /// Called when [filter pop up opened].
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
        void OnFilterPopUpOpened(object sender, EventArgs e)
        {         
           

            //if (this.FilterListBoxItem == null)
            //{
            //    return;
            //}

#if SILVERLIGHT
            this.PART_FilterPopUpBorder.Width = this.MinWidth;
            this.PART_FilterPopUpBorder.Height = this.MinHeight;
            
#else
             this.Height = this.MinHeight;
            this.Width = this.MinWidth;

#endif
            this.UnHookFilterElementPropertyChanged(this.FilterListBoxItem);
            if (FilterListBoxItem != null)
            this.FilterListBoxItem.Clear();
            if (ListBoxItems != null)
                this.ListBoxItems.Clear();
            
           var itemsSource= this.RaisePopupOpened();
           this.FilterListBoxItem = itemsSource;
           if (FilterListBoxItem != null)
           {
               //Sort the Collection
               this.FilterListBoxItem.Sort(new FilterElementAscendingOrder());
               // the FilterListBoxItem is cloned to maintain the filtered items when the search close button is clicked.
               ListBoxItems = this.FilterListBoxItem.Select(item => (FilterElement)item.Clone()).ToList<FilterElement>();
           }
            this.ItemsSource = null;
            this.ItemsSource = FilterListBoxItem;

            if (FilterListBoxItem == null)
            {
                if(this.OkButton!=null)
                    this.OkButton.IsEnabled = false;
                return;
            }
            if (FilterListBoxItem.Count > 0 && this.OkButton!=null)
            {
                this.OkButton.IsEnabled = true;
            }
            FilterElement uncheckedItem=null;
            List<FilterElement> uncheckedlist=new List<FilterElement>();
            //To Check/UnCheck the SelectAll CheckBox
            uncheckedItem = FilterListBoxItem.FirstOrDefault(x => x.IsSelected == false);
            uncheckedlist = FilterListBoxItem.Where(x => x.IsSelected == false).ToList<FilterElement>();

            if (this.SelectAllCheckBox != null)
            {
                if (uncheckedItem == null)
                {
                    this.SkipSelectAllCheckBoxEvents = true;
                    if(this.SelectAllCheckBox!=null)
                        this.SelectAllCheckBox.IsChecked = true;
                    this.SkipSelectAllCheckBoxEvents = false;
                }
                else
                {
                    this.SkipSelectAllCheckBoxEvents = true;
                    if(this.SelectAllCheckBox!=null)
                        this.SelectAllCheckBox.IsChecked = null;
                    this.SkipSelectAllCheckBoxEvents = false;
                }
            }          
            if (uncheckedlist.Count() == FilterListBoxItem.Count)
            {
                if (this.OkButton != null)
                    this.OkButton.IsEnabled = false;
                this.SkipSelectAllCheckBoxEvents = true;
                if (SelectAllCheckBox != null)
                    SelectAllCheckBox.IsChecked = false;
                this.SkipSelectAllCheckBoxEvents = false;
            }
            if (this.SearchTextBox!=null)
                this.SearchTextBox.Text = "";

#if SILVERLIGHT
            SortStateSetting();
#endif

        }

       
       

#if SILVERLIGHT
        private void SortStateSetting()
        {
            if(this.SortOrder=="Ascending")
            {
                VisualStateManager.GoToState(this, "Ascending", false);
            }
            else if(this.SortOrder=="Descending")
            {
                VisualStateManager.GoToState(this, "Descending", false);
            }
            else
            {
                VisualStateManager.GoToState(this, "Normal", false);
            }
        }

       
#endif

        #endregion

        #region Clear Filter     
#if !SILVERLIGHT

        /// <summary>
        /// Called when [clear filter clicked].
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The <see cref="System.Windows.RoutedEventArgs"/> instance containing the event data.</param>
        void OnClearFilterClick(object sender, RoutedEventArgs e)
        {
            viewModel = new AdvanceFilteringViewModel();
            this.RaiseClearMenuItemClick();
            this.IsOpen = false;
        }        
#else
        /// <summary>
        /// Called when [clear filter clicked].
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The <see cref="System.Windows.RoutedEventArgs"/> instance containing the event data.</param>
        void OnClearFilterClick(object sender, EventArgs e)
        {
            this.RaiseClearMenuItemClick();
            this.IsOpen = false;
        } 
#endif
        #endregion

        #region Button Click Listner

        /// <summary>
        /// Called when [cancel button click].
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The <see cref="System.Windows.RoutedEventArgs"/> instance containing the event data.</param>
        void  OnCancelButtonClick(object sender, RoutedEventArgs e)
        {
#if SILVERLIGHT
            var headercell = this.FindParentElementOfType<GridDataHeaderCellControl>();
            headercell.CloseFilterDropDown();
#else
            this.IsOpen = false;
#endif
        }

        /// <summary>
        /// Called when [ok button click].
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The <see cref="System.Windows.RoutedEventArgs"/> instance containing the event data.</param>
        void OnOkButtonClick(object sender, RoutedEventArgs e)
        {            
            ProcessOkButtonClick();            
            this.IsOpen = false;
        }

        /// <summary>
        /// This method called when Ok button is Clicked in FilterPane or EnterKey is pressed when focus is in Search Textbox.
        /// </summary>
        private void ProcessOkButtonClick()
        {
            if (this.SelectedItem != null && this.SelectedItem.Count > 0)
            {
                //If we edit style in the sample we have to add selected item to this SelectedItem. Based on this collection Filter will applied.
                this.RaiseOkButtonClick(SelectedItem, null);

            }
            else if (this.SearchTextBox == null || SearchTextBox.Text != "")
            {
                List<FilterPredicate> filterPredicate = new List<FilterPredicate>();
                var items = (this.ItemsSource as List<FilterElement>);
                var unCheckedItem = items.Where(x => x.IsSelected == false).ToList<FilterElement>();
                var checkedItem = items.Where(x => x.IsSelected == true).ToList<FilterElement>();
                foreach (var it in this.FilterListBoxItem)
                {
                    var itemNotinItemsSource = items.FirstOrDefault(x => x.ActualValue == it.ActualValue);
                    if (itemNotinItemsSource == null)
                    {
                        FilterElement element = new FilterElement() { ActualValue = it.ActualValue, IsSelected = false, Name = it.Name };
                        element.PropertyChanged += new PropertyChangedEventHandler(OnFilterElementPropertyChanged);
                        unCheckedItem.Add(element);
                    }
                }
                this.RaiseOkButtonClick(checkedItem, unCheckedItem);
            }
            else
            {
                //If search text box contains the item then it filtering should applied in the searched items.
                if (FilterListBoxItem != null)
                {
                    var UnCheckedItem = this.FilterListBoxItem.Where(x => x.IsSelected == false).ToList<FilterElement>();
                    var checkedCount = this.FilterListBoxItem.Count() - UnCheckedItem.Count();
                    var checkedItem = this.FilterListBoxItem.Where(x => x.IsSelected == true).ToList<FilterElement>();
                    this.RaiseOkButtonClick(checkedItem, UnCheckedItem);
                }
            }
        }

        #endregion

        #region Property Change Listner      
       
        /// <summary>
        /// Called when [filter element property changed].
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The <see cref="System.ComponentModel.PropertyChangedEventArgs"/> instance containing the event data.</param>
        internal void OnFilterElementPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (!propertyChangedFromSelectAll)
            {
                this.MaintainSelectAllCheckBox();
                //Following Code to check/Uncheck/Null state of SelectAllComboBox IsSelected Property.
                var uncheckedItem = FilterListBoxItem.FirstOrDefault(x => x.IsSelected == false);
                var uncheckedlist = FilterListBoxItem.Where(x => x.IsSelected == false);     
              
                var itemCount=(this.ItemsSource as List<FilterElement>).Count;
                if (!this.propertyChangedFromSelectAll)
                {
                    if (uncheckedlist.Count() ==itemCount )
                    {
                        if(this.OkButton!=null)
                            this.OkButton.IsEnabled = false;
                        if(SelectAllCheckBox!=null)
                            SelectAllCheckBox.IsChecked = false;
                    }
                    if (!this.ExcelLikeFilterAdvVisibility)
                    {

                        this.RaiseOnFilterElementPropertyChanged(sender as FilterElement, this.SelectAllCheckBox.IsChecked);
                    }
                }
            }
        }


        private void MaintainSelectAllCheckBox()
        {
            if (FilterListBoxItem == null)
                return;
            var uncheked = this.FilterListBoxItem.Select(x => x).Where(y => y.IsSelected == false).ToList();
            if (uncheked.Count == 0)
            {
                SkipSelectAllCheckBoxEvents = true;
                this.SelectAllCheckBox.IsThreeState = false;
                this.SelectAllCheckBox.IsChecked = true;
                if (this.OkButton != null)
                    this.OkButton.IsEnabled = true;
                SkipSelectAllCheckBoxEvents = false;
            }
            else if (uncheked.Count == this.FilterListBoxItem.Count)
            {

                SkipSelectAllCheckBoxEvents = true;
                this.SelectAllCheckBox.IsThreeState = false;
                this.SelectAllCheckBox.IsChecked = false;
                if (this.OkButton != null)
                    this.OkButton.IsEnabled = false;
                SkipSelectAllCheckBoxEvents = false;
            }
            else
            {
                SkipSelectAllCheckBoxEvents = true;
                this.SelectAllCheckBox.IsThreeState = true;
                if (this.OkButton != null)
                    this.OkButton.IsEnabled = true;
                this.SelectAllCheckBox.IsChecked = null;
                SkipSelectAllCheckBoxEvents = false;
            }
        }

        #endregion
       
        #region Resizing Behavior

        /// <summary>
        /// This is used to get the mouse point on the screen
        /// </summary>
        /// <param name="lpPoint"></param>
        /// <returns></returns>
        [DllImport("user32.dll")]
        [return: MarshalAs(UnmanagedType.Bool)]
        static extern bool GetCursorPos(out MousePosition lpPoint);
        [StructLayout(LayoutKind.Sequential)]
        internal struct MousePosition
        {
            public int X; public int Y; public MousePosition(int x, int y) { this.X = x; this.Y = y; }
        }

        /// <summary>
        /// Resizings the thumb drag delta.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The <see cref="System.Windows.Controls.Primitives.DragDeltaEventArgs"/> instance containing the event data.</param>
        void ResizingThumbDragDelta(object sender, DragDeltaEventArgs e)
        {

           
#if !SILVERLIGHT
             MousePosition p;
            /// Getting the mouser position relative to screen
            GetCursorPos(out p);
#endif


            var thumb = sender as Thumb;
#if !SILVERLIGHT

            var point = Mouse.GetPosition(thumb);
            var width = System.Windows.SystemParameters.PrimaryScreenWidth;
            var height = System.Windows.SystemParameters.PrimaryScreenHeight;
             if (thumb.Cursor == Cursors.SizeNWSE)
            {
                double yadjust = this.Height + e.VerticalChange;

                double xadjust = this.Width + e.HorizontalChange;              

                if ((xadjust >= 0) && (yadjust >= 0))
                {
                    if (p.X < width - 10)
                        this.Width = xadjust;
                    if (p.Y < height - 10)
                        this.Height = yadjust;
                }
            }
#else
            var p = DependencyObjectExtensions.GetMousePosition(Application.Current.RootVisual); ;
            var width =double.Parse( HtmlPage.Window.Eval("screen.availWidth").ToString());
            var height =double.Parse( HtmlPage.Window.Eval("screen.availHeight").ToString());
            if (thumb.Cursor == Cursors.SizeNWSE)
            {
                double yadjust = this.PART_FilterPopUpBorder.ActualHeight + e.VerticalChange;

                double xadjust = this.PART_FilterPopUpBorder.ActualWidth + e.HorizontalChange;

                if ((xadjust >= 0) && (yadjust >= 0))
                {
                    if (p.X < width - 10)
                        this.PART_FilterPopUpBorder.Width = xadjust;
                    if (p.Y < height - 10)
                        this.PART_FilterPopUpBorder.Height = yadjust;
                }
            }
#endif
            

        }

        #endregion       

        #region Events  
    

        #region OkButtonClick
#if !SILVERLIGHT
         /// <summary>
        /// Raise when clicking OK button in the ExcelLikeFilter Pane.
        /// </summary>
        public static readonly RoutedEvent OkButtonClickEvent = EventManager.RegisterRoutedEvent(
            "OkButtonClick",
            RoutingStrategy.Direct,
            typeof(OkButtonClickEventHandler),
            typeof(GridDataExcelLikeFilterPane));
#else
        public event OkButtonClickEventHandler OkButtonClick;
#endif

#if !SILVERLIGHT
         /// <summary>
        /// Raises the ok button click.
        /// </summary>
        internal void RaiseOkButtonClick(List<FilterElement> checkedElement, List<FilterElement> unCheckedElement)
        {
            OnOkButtonClick(new OkButtonClikEventArgs(GridDataExcelLikeFilterPane.OkButtonClickEvent, this) { UnCheckedElement = unCheckedElement, CheckedElement = checkedElement });
        }

        /// <summary>
        /// Raises the <see cref="E:OkButtonClick"/> event.
        /// </summary>
        /// <param name="e">The <see cref="Syncfusion.Windows.Controls.Grid.OkButtonClikEventArgs"/> instance containing the event data.</param>
        private void OnOkButtonClick(OkButtonClikEventArgs e)
        {
            base.RaiseEvent(e);            
        }

        /// <summary>
        /// Occurs when [ok button click].
        /// </summary>
        public event OkButtonClickEventHandler OkButtonClick
        {
            add
            {
                this.AddHandler(GridDataExcelLikeFilterPane.OkButtonClickEvent, value, false);
            }
            remove
            {
                this.RemoveHandler(GridDataExcelLikeFilterPane.OkButtonClickEvent, value);
            }
        } 
#else
        /// <summary>
        /// Raises the ok button click.
        /// </summary>
        internal void RaiseOkButtonClick(List<FilterElement> checkedElement, List<FilterElement> unCheckedElement)
        {
            OnOkButtonClick(new OkButtonClikEventArgs(this) { UnCheckedElement = unCheckedElement, CheckedElement = checkedElement });
        }

        /// <summary>
        /// Raises the <see cref="E:OkButtonClick"/> event.
        /// </summary>
        /// <param name="e">The <see cref="Syncfusion.Windows.Controls.Grid.OkButtonClikEventArgs"/> instance containing the event data.</param>
        private void OnOkButtonClick(OkButtonClikEventArgs e)
        {
            if (this.OkButtonClick != null)
                this.OkButtonClick(this, e);
        }
#endif


        #endregion

        #region ClearMenuItemClick
#if !SILVERLIGHT

        /// <summary>
        /// Raise when click Clear MenuItem
        /// </summary>
        public static readonly RoutedEvent ClearMenuItemClickEvent = EventManager.RegisterRoutedEvent(
            "ClearMenuItemClick",
            RoutingStrategy.Direct,
            typeof(ClearMenuItemClickEventHandler),
            typeof(GridDataExcelLikeFilterPane));
#else
        public event ClearMenuItemClickEventHandler ClearMenuItemClick;
#endif
#if !SILVERLIGHT

        /// <summary>
        /// Raises the clear menu item click.
        /// </summary>
        internal void RaiseClearMenuItemClick()
        {
            OnClearMenuItemClick(new SyncfusionRoutedEventArgs(GridDataExcelLikeFilterPane.ClearMenuItemClickEvent, this));
        }
         /// <summary>
        /// Raises the <see cref="GridControlBase.CurrentCellActivated"/> event.
        /// </summary>
        /// <param name="e">The <see cref="EventArgs"/> instance containing the event data.</param>
        private void OnClearMenuItemClick(SyncfusionRoutedEventArgs e)
        {
            base.RaiseEvent(e);            
        }

#else
        /// <summary>
        /// Raises the clear menu item click.
        /// </summary>
        internal void RaiseClearMenuItemClick()
        {
            OnClearMenuItemClick(new SyncfusionRoutedEventArgs(this));
        }
        /// <summary>
        /// Raises the <see cref="GridControlBase.CurrentCellActivated"/> event.
        /// </summary>
        /// <param name="e">The <see cref="EventArgs"/> instance containing the event data.</param>
        private void OnClearMenuItemClick(SyncfusionRoutedEventArgs e)
        {
            if (this.ClearMenuItemClick != null)
            {
                this.ClearMenuItemClick(this, e);
            }
        }
#endif

#if !SILVERLIGHT
         /// <summary>
        /// Occurs when [clear menu item click].
        /// </summary>
        public event ClearMenuItemClickEventHandler ClearMenuItemClick
        {
            add
            {
                this.AddHandler(GridDataExcelLikeFilterPane.ClearMenuItemClickEvent, value, false);
            }
            remove
            {
                 this.RemoveHandler(GridDataExcelLikeFilterPane.ClearMenuItemClickEvent, value);
            }
        }
#endif


        #endregion

        #region PopupOpened
#if !SILVERLIGHT
        /// <summary>
        /// Routed Event for PopupOpened.
        /// </summary>
        public static readonly RoutedEvent PopupOpenedEvent = EventManager.RegisterRoutedEvent(
            "PopupOpened",
            RoutingStrategy.Direct,
            typeof(PopupOpenedEventHandler),
            typeof(GridDataExcelLikeFilterPane));
#else
        public event PopupOpenedEventHandler PopupOpened;
#endif
#if !SILVERLIGHT
         /// <summary>
        /// Raises the popup opened.
        /// </summary>
        /// <returns></returns>
        internal List<FilterElement> RaisePopupOpened()
        {
           return OnPopupOpened(new PopupOpenedEventArgs(GridDataExcelLikeFilterPane.PopupOpenedEvent, this));
        }

        /// <summary>
        /// Raises the <see cref="GridControlBase.CurrentCellActivated"/> event.
        /// </summary>
        /// <param name="e">The <see cref="EventArgs"/> instance containing the event data.</param>
        private List<FilterElement> OnPopupOpened(PopupOpenedEventArgs e)
        {            
            base.RaiseEvent(e);
            return e.ItemsSource;
        }
#else
        /// <summary>
        /// Raises the popup opened.
        /// </summary>
        /// <returns></returns>
        internal List<FilterElement> RaisePopupOpened()
        {
            return OnPopupOpened(new PopupOpenedEventArgs());
        }

        /// <summary>
        /// Raises the <see cref="GridControlBase.CurrentCellActivated"/> event.
        /// </summary>
        /// <param name="e">The <see cref="EventArgs"/> instance containing the event data.</param>
        private List<FilterElement> OnPopupOpened(PopupOpenedEventArgs e)
        {
            if (PopupOpened != null)
                PopupOpened(this, e);

            return e.ItemsSource;
        }
#endif

#if !SILVERLIGHT
         /// <summary>
        /// Occurs when [popup opened].
        /// </summary>
        public event PopupOpenedEventHandler PopupOpened
        {
            add
            {
                this.AddHandler(GridDataExcelLikeFilterPane.PopupOpenedEvent, value, false);
            }
            remove
            {
                this.RemoveHandler(GridDataExcelLikeFilterPane.PopupOpenedEvent, value);
            }
        }
#endif


        #endregion

        #region SortMenuItemClick
#if !SILVERLIGHT
        /// <summary>
        /// Raise when click SortMenuItem in ExcelLikeFilter pane.
        /// </summary>
        public static readonly RoutedEvent SortMenuItemClickEvent = EventManager.RegisterRoutedEvent(
            "SortMenuItemClick",
            RoutingStrategy.Direct,
            typeof(SortMenuItemClickEventHandler),
            typeof(GridDataExcelLikeFilterPane));
#else
        public event SortMenuItemClickEventHandler SortMenuItemClick;
#endif

#if !SILVERLIGHT
         /// <summary>
        /// Raises the sort menu item click.
        /// </summary>
        internal void RaiseSortMenuItemClick(string sortString)
        {
            OnSortMenuItemClick(new SortMenuItemClickEventArgs(GridDataExcelLikeFilterPane.SortMenuItemClickEvent, this) { SortString = sortString });
        }

        /// <summary>
        /// Raises the <see cref="E:SortMenuItemClick"/> event.
        /// </summary>
        /// <param name="e">The <see cref="Syncfusion.Windows.Controls.Grid.PopupOpenedEventArgs"/> instance containing the event data.</param>
        private void OnSortMenuItemClick(SortMenuItemClickEventArgs e)
        {
            base.RaiseEvent(e);            
        }

        /// <summary>
        /// Occurs when [sort menu item click].        /// </summary>
        public event SortMenuItemClickEventHandler SortMenuItemClick
        {
            add
            {
                this.AddHandler(GridDataExcelLikeFilterPane.SortMenuItemClickEvent, value, false);
            }
            remove
            {
                this.RemoveHandler(GridDataExcelLikeFilterPane.SortMenuItemClickEvent, value);
            }
        }
#else
        /// <summary>
        /// Raises the sort menu item click.
        /// </summary>
        internal void RaiseSortMenuItemClick(string sortString)
        {
            OnSortMenuItemClick(new SortMenuItemClickEventArgs( this) { SortString = sortString });
        }

        /// <summary>
        /// Raises the <see cref="E:SortMenuItemClick"/> event.
        /// </summary>
        /// <param name="e">The <see cref="Syncfusion.Windows.Controls.Grid.PopupOpenedEventArgs"/> instance containing the event data.</param>
        private void OnSortMenuItemClick(SortMenuItemClickEventArgs e)
        {
            if (SortMenuItemClick != null)
                this.SortMenuItemClick(this, e);
        }
#endif



        #endregion

        #region AdvanceFilteringOkButtonClick
#if !SILVERLIGHT
        /// <summary>
        /// Raise when Clicking OK button in the AdvanceFiltering window.
        /// </summary>
        public static readonly RoutedEvent AdvanceFilteringOkButtonClickEvent = EventManager.RegisterRoutedEvent(
            "AdvanceFilteringOkButtonClick",
            RoutingStrategy.Direct,
            typeof(AdvanceFilteringOkButtonClickEventHandler),
            typeof(GridDataExcelLikeFilterPane));
#else
        public event AdvanceFilteringOkButtonClickEventHandler AdvanceFilteringOkButtonClick;
#endif

#if !SILVERLIGHT
         /// <summary>
        /// Raises the advance filtering ok button click.
        /// </summary>
        /// <param name="filterValue1">The filter value1.</param>
        /// <param name="filterValue2">The filter value2.</param>
        /// <param name="filterType1">The filter type1.</param>
        /// <param name="filterType2">The filter type2.</param>
        /// <param name="predicateType">Type of the predicate.</param>
        internal void RaiseAdvanceFilteringOkButtonClick(object filterValue1, object filterValue2, object filterType1, object filterType2, object predicateType)
        {
            OnAdvanceFilteringOkButtonClick(new AdvanceFilteringOkButtonClickEventArgs(GridDataExcelLikeFilterPane.AdvanceFilteringOkButtonClickEvent, this) { FilterValue1 = filterValue1, FilterValue2 = filterValue2, FilterType1 = filterType1, FilterType2 = filterType2, PredicateType = predicateType });
        }

        /// <summary>
        /// Raises the <see cref="E:AdvanceFilteringOkButtonClick"/> event.
        /// </summary>
        /// <param name="e">The <see cref="Syncfusion.Windows.Controls.Grid.AdvanceFilteringOkButtonClickEventArgs"/> instance containing the event data.</param>
        private void OnAdvanceFilteringOkButtonClick(AdvanceFilteringOkButtonClickEventArgs e)
        {
            base.RaiseEvent(e);
        }

        /// <summary>
        /// Occurs when [advance filtering ok button click].
        /// </summary>
        public event AdvanceFilteringOkButtonClickEventHandler AdvanceFilteringOkButtonClick
        {
            add
            {
                this.AddHandler(GridDataExcelLikeFilterPane.AdvanceFilteringOkButtonClickEvent, value, false);
            }
            remove
            {
                this.RemoveHandler(GridDataExcelLikeFilterPane.AdvanceFilteringOkButtonClickEvent, value);
            }
        }    
#else
        /// <summary>
        /// Raises the advance filtering ok button click.
        /// </summary>
        /// <param name="filterValue1">The filter value1.</param>
        /// <param name="filterValue2">The filter value2.</param>
        /// <param name="filterType1">The filter type1.</param>
        /// <param name="filterType2">The filter type2.</param>
        /// <param name="predicateType">Type of the predicate.</param>
        internal void RaiseAdvanceFilteringOkButtonClick(object filterValue1, object filterValue2, object filterType1, object filterType2, object predicateType)
        {
            OnAdvanceFilteringOkButtonClick(new AdvanceFilteringOkButtonClickEventArgs(this) { FilterValue1 = filterValue1, FilterValue2 = filterValue2, FilterType1 = filterType1, FilterType2 = filterType2, PredicateType = predicateType });
        }

        /// <summary>
        /// Raises the <see cref="E:AdvanceFilteringOkButtonClick"/> event.
        /// </summary>
        /// <param name="e">The <see cref="Syncfusion.Windows.Controls.Grid.AdvanceFilteringOkButtonClickEventArgs"/> instance containing the event data.</param>
        private void OnAdvanceFilteringOkButtonClick(AdvanceFilteringOkButtonClickEventArgs e)
        {
            if (AdvanceFilteringOkButtonClick != null)
                this.AdvanceFilteringOkButtonClick(this, e);
        }
#endif


        #endregion

        #region OnFilterElementPropertyChanged
#if !SILVERLIGHT
         public static readonly RoutedEvent OnFilterElementChangedEvent = EventManager.RegisterRoutedEvent(
            "OnFilterElementChanged",
            RoutingStrategy.Direct,
            typeof(OnFilterElementPropertyChangedEventHandler),
            typeof(GridDataExcelLikeFilterPane));
#else
        public event OnFilterElementPropertyChangedEventHandler OnFilterElementChanged;
#endif

#if !SILVERLIGHT
        /// <summary>
        /// Raises the on filter element property changed.
        /// </summary>
        /// <param name="FilterElement">The filter element.</param>
        /// <param name="SelectAllChecked">The select all checked.</param>
        internal void RaiseOnFilterElementPropertyChanged(FilterElement FilterElement, Nullable<bool> SelectAllChecked)
        {
            FilterElementPropertyChanged(new OnFilterElementPropertyChangedEventArgs(GridDataExcelLikeFilterPane.OnFilterElementChangedEvent, this) { FilterElement = FilterElement, SelectAllChecked = SelectAllChecked });
        }

        /// <summary>
        /// Filters the element property changed.
        /// </summary>
        /// <param name="e">The <see cref="Syncfusion.Windows.Controls.Grid.OnFilterElementPropertyChangedEventArgs"/> instance containing the event data.</param>
        private void FilterElementPropertyChanged(OnFilterElementPropertyChangedEventArgs e)
        {
            base.RaiseEvent(e);
        }

        /// <summary>
        /// Occurs when [on filter element changed].
        /// </summary>
        public event OnFilterElementPropertyChangedEventHandler OnFilterElementChanged
        {
            add
            {
                this.AddHandler(GridDataExcelLikeFilterPane.OnFilterElementChangedEvent, value, false);
            }
            remove
            {
                this.RemoveHandler(GridDataExcelLikeFilterPane.OnFilterElementChangedEvent, value);
            }
        }      
#else
        /// <summary>
        /// Raises the on filter element property changed.
        /// </summary>
        /// <param name="FilterElement">The filter element.</param>
        /// <param name="SelectAllChecked">The select all checked.</param>
        internal void RaiseOnFilterElementPropertyChanged(FilterElement FilterElement, Nullable<bool> SelectAllChecked)
        {
            FilterElementPropertyChanged(new OnFilterElementPropertyChangedEventArgs(this) { FilterElement = FilterElement, SelectAllChecked = SelectAllChecked });
        }

        /// <summary>
        /// Filters the element property changed.
        /// </summary>
        /// <param name="e">The <see cref="Syncfusion.Windows.Controls.Grid.OnFilterElementPropertyChangedEventArgs"/> instance containing the event data.</param>
        private void FilterElementPropertyChanged(OnFilterElementPropertyChangedEventArgs e)
        {
            if (this.OnFilterElementChanged != null)
                this.OnFilterElementChanged(this, e);
        }
#endif


        #endregion

        #region SelectAllCheckBoxChecked
#if !SILVERLIGHT
         public static readonly RoutedEvent SelectAllCheckBoxCheckedEvent = EventManager.RegisterRoutedEvent(
           "SelectAllCheckBoxChecked",
           RoutingStrategy.Direct,
           typeof(SelectAllCheckBoxCheckedEventHandler),
           typeof(GridDataExcelLikeFilterPane));
#else
        public event SelectAllCheckBoxCheckedEventHandler SelectAllCheckBoxChecked;
#endif

#if !SILVERLIGHT
         /// <summary>
        /// Raises the select all check box checked.
        /// </summary>
        /// <param name="FilterElements">The filter elements.</param>
        internal void RaiseSelectAllCheckBoxChecked(List<FilterElement> FilterElements)
        {
            OnSelectAllCheckBoxChecked(new SelectAllCheckBoxCheckedEventArgs(GridDataExcelLikeFilterPane.SelectAllCheckBoxCheckedEvent, this) { FilterElements = FilterElements });
        }

        /// <summary>
        /// Raises the <see cref="E:SelectAllCheckBoxChecked"/> event.
        /// </summary>
        /// <param name="e">The <see cref="Syncfusion.Windows.Controls.Grid.SelectAllCheckBoxCheckedEventArgs"/> instance containing the event data.</param>
        private void OnSelectAllCheckBoxChecked(SelectAllCheckBoxCheckedEventArgs e)
        {
            base.RaiseEvent(e);
        }

        /// <summary>
        /// Occurs when [select all check box checked].
        /// </summary>
        public event SelectAllCheckBoxCheckedEventHandler SelectAllCheckBoxChecked
        {
            add
            {
                this.AddHandler(GridDataExcelLikeFilterPane.SelectAllCheckBoxCheckedEvent, value, false);
            }
            remove
            {
                this.RemoveHandler(GridDataExcelLikeFilterPane.SelectAllCheckBoxCheckedEvent, value);
            }
        }
#else
        /// <summary>
        /// Raises the select all check box checked.
        /// </summary>
        /// <param name="FilterElements">The filter elements.</param>
        internal void RaiseSelectAllCheckBoxChecked(List<FilterElement> FilterElements)
        {
            OnSelectAllCheckBoxChecked(new SelectAllCheckBoxCheckedEventArgs(this) { FilterElements = FilterElements });
        }

        /// <summary>
        /// Raises the <see cref="E:SelectAllCheckBoxChecked"/> event.
        /// </summary>
        /// <param name="e">The <see cref="Syncfusion.Windows.Controls.Grid.SelectAllCheckBoxCheckedEventArgs"/> instance containing the event data.</param>
        private void OnSelectAllCheckBoxChecked(SelectAllCheckBoxCheckedEventArgs e)
        {
            if (this.SelectAllCheckBoxChecked != null)
                this.SelectAllCheckBoxChecked(this, e);
        }
#endif


        #endregion

        #region SelectAllUnCheckBoxChecked
#if !SILVERLIGHT
         public static readonly RoutedEvent SelectAllUnCheckBoxCheckedEvent = EventManager.RegisterRoutedEvent(
            "SelectAllUnCheckBoxChecked",
            RoutingStrategy.Direct,
            typeof(SelectAllCheckBoxUnCheckedEventHandler),
            typeof(GridDataExcelLikeFilterPane));
#else
        public event SelectAllCheckBoxUnCheckedEventHandler SelectAllUnCheckBoxChecked;
#endif
#if !SILVERLIGHT
         /// <summary>
        /// Raises the select all un check box checked.
        /// </summary>
        /// <param name="FilterElements">The filter elements.</param>
        internal void RaiseSelectAllUnCheckBoxChecked(List<FilterElement> FilterElements)
        {
            OnSelectAllUnCheckBoxChecked(new SelectAllCheckBoxUnCheckedEventArgs(GridDataExcelLikeFilterPane.SelectAllUnCheckBoxCheckedEvent, this) { FilterElements = FilterElements });
        }


        /// <summary>
        /// Raises the <see cref="E:SelectAllUnCheckBoxChecked"/> event.
        /// </summary>
        /// <param name="e">The <see cref="Syncfusion.Windows.Controls.Grid.SelectAllCheckBoxUnCheckedEventArgs"/> instance containing the event data.</param>
        private void OnSelectAllUnCheckBoxChecked(SelectAllCheckBoxUnCheckedEventArgs e)
        {
            base.RaiseEvent(e);
        }

       
        public event SelectAllCheckBoxUnCheckedEventHandler SelectAllUnCheckBoxChecked
        {
            add
            {
                this.AddHandler(GridDataExcelLikeFilterPane.SelectAllUnCheckBoxCheckedEvent, value, false);
            }
            remove
            {
                this.RemoveHandler(GridDataExcelLikeFilterPane.SelectAllUnCheckBoxCheckedEvent, value);
            }
        }    

#else
        /// <summary>
        /// Raises the select all un check box checked.
        /// </summary>
        /// <param name="FilterElements">The filter elements.</param>
        internal void RaiseSelectAllUnCheckBoxChecked(List<FilterElement> FilterElements)
        {
            OnSelectAllUnCheckBoxChecked(new SelectAllCheckBoxUnCheckedEventArgs(this) { FilterElements = FilterElements });
        }


        /// <summary>
        /// Raises the <see cref="E:SelectAllUnCheckBoxChecked"/> event.
        /// </summary>
        /// <param name="e">The <see cref="Syncfusion.Windows.Controls.Grid.SelectAllCheckBoxUnCheckedEventArgs"/> instance containing the event data.</param>
        private void OnSelectAllUnCheckBoxChecked(SelectAllCheckBoxUnCheckedEventArgs e)
        {
            if (this.SelectAllUnCheckBoxChecked != null)
                this.SelectAllUnCheckBoxChecked(this, e);
        }
#endif




        #endregion
        #endregion

        public void Dispose()
        {
            if (this.FilterListBoxItem != null)
            {
                foreach (var item in FilterListBoxItem)
                {
                    item.PropertyChanged -= new PropertyChangedEventHandler(this.OnFilterElementPropertyChanged);
                }
                this.FilterListBoxItem.Clear();
                this.FilterListBoxItem = null;
                this.ListBoxItems.Clear();
                this.ListBoxItems = null;
            }
        }
    }      
}
