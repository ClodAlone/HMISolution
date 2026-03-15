#region Copyright Syncfusion Inc. 2001 - 2014
// Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Any infringement will be prosecuted under
// applicable laws. 
#endregion
using System;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Ink;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;
using System.ComponentModel;
using System.Collections.Generic;
using System.Reflection;
using System.Collections.ObjectModel;
using System.Windows.Data;
using System.Linq;
using System.Windows.Controls.Primitives;
using System.Windows.Threading;
using Syncfusion.Windows.Shared;
using System.Windows.Resources;
using System.IO;
using System.Windows.Markup;
using Syncfusion.Windows.Tools.Controls;
using System.Collections;

#if SILVERLIGHT
using Syncfusion.Windows.Controls.Theming;
#endif

#if WPF
using System.Data;
using Syncfusion.Licensing;
#endif

namespace Syncfusion.Windows.PropertyGrid
{   
#if SILVERLIGHT
    [Syncfusion.Windows.Shared.SkinType(SkinVisualStyle = Syncfusion.Windows.Shared.VisualStyle.Blend,
      Type = typeof(PropertyGrid), XamlResource = "/Syncfusion.PropertyGrid.Silverlight;component/Themes/Blend.xaml")]
    [Syncfusion.Windows.Shared.SkinType(SkinVisualStyle = Syncfusion.Windows.Shared.VisualStyle.Windows7,
      Type = typeof(PropertyGrid), XamlResource = "/Syncfusion.PropertyGrid.Silverlight;component/Themes/Generic.xaml")]
    [Syncfusion.Windows.Shared.SkinType(SkinVisualStyle = Syncfusion.Windows.Shared.VisualStyle.Office2007Black,
      Type = typeof(PropertyGrid), XamlResource = "/Syncfusion.PropertyGrid.Silverlight;component/Themes/Office2007Black.xaml")]
    [Syncfusion.Windows.Shared.SkinType(SkinVisualStyle = Syncfusion.Windows.Shared.VisualStyle.Office2007Blue,
      Type = typeof(PropertyGrid), XamlResource = "/Syncfusion.PropertyGrid.Silverlight;component/Themes/Office2007Blue.xaml")]
    [Syncfusion.Windows.Shared.SkinType(SkinVisualStyle = Syncfusion.Windows.Shared.VisualStyle.Office2007Silver,
      Type = typeof(PropertyGrid), XamlResource = "/Syncfusion.PropertyGrid.Silverlight;component/Themes/Office2007Silver.xaml")]
    [Syncfusion.Windows.Shared.SkinType(SkinVisualStyle = Syncfusion.Windows.Shared.VisualStyle.Office2010Black,
      Type = typeof(PropertyGrid), XamlResource = "/Syncfusion.PropertyGrid.Silverlight;component/Themes/Office2010Black.xaml")]
    [Syncfusion.Windows.Shared.SkinType(SkinVisualStyle = Syncfusion.Windows.Shared.VisualStyle.Office2010Blue,
      Type = typeof(PropertyGrid), XamlResource = "/Syncfusion.PropertyGrid.Silverlight;component/Themes/Office2010Blue.xaml")]
    [Syncfusion.Windows.Shared.SkinType(SkinVisualStyle = Syncfusion.Windows.Shared.VisualStyle.Office2010Silver,
      Type = typeof(PropertyGrid), XamlResource = "/Syncfusion.PropertyGrid.Silverlight;component/Themes/Office2010Silver.xaml")]
    [Syncfusion.Windows.Shared.SkinType(SkinVisualStyle = Syncfusion.Windows.Shared.VisualStyle.VS2010,
      Type = typeof(PropertyGrid), XamlResource = "/Syncfusion.PropertyGrid.Silverlight;component/Themes/VS2010.xaml")]
    [Syncfusion.Windows.Shared.SkinType(SkinVisualStyle = Syncfusion.Windows.Shared.VisualStyle.Default,
      Type = typeof(PropertyGrid), XamlResource = "/Syncfusion.PropertyGrid.Silverlight;component/Themes/Generic.xaml")]
    [Syncfusion.Windows.Shared.SkinType(SkinVisualStyle = Syncfusion.Windows.Shared.VisualStyle.Metro,
     Type = typeof(PropertyGrid), XamlResource = "/Syncfusion.PropertyGrid.Silverlight;component/Themes/Metro.xaml")]
    [Syncfusion.Windows.Shared.SkinType(SkinVisualStyle = Syncfusion.Windows.Shared.VisualStyle.Transparent,
    Type = typeof(PropertyGrid), XamlResource = "/Syncfusion.PropertyGrid.Silverlight;component/Themes/Transparent.xaml")]
#endif
#if WPF
    [SkinType(SkinVisualStyle = Skin.Office2007Blue,
Type = typeof(PropertyGrid), XamlResource = "/Syncfusion.PropertyGrid.WPF;component/Themes/Office2007BlueStyle.xaml")]
    [SkinType(SkinVisualStyle = Skin.Office2007Black,
    Type = typeof(PropertyGrid), XamlResource = "/Syncfusion.PropertyGrid.WPF;component/Themes/Office2007BlackStyle.xaml")]
    [SkinType(SkinVisualStyle = Skin.Office2007Silver,
    Type = typeof(PropertyGrid), XamlResource = "/Syncfusion.PropertyGrid.WPF;component/Themes/Office2007SilverStyle.xaml")]
    [SkinType(SkinVisualStyle = Skin.Office2010Blue,
Type = typeof(PropertyGrid), XamlResource = "/Syncfusion.PropertyGrid.WPF;component/Themes/Office2010BlueStyle.xaml")]
    [SkinType(SkinVisualStyle = Skin.Office2010Black,
    Type = typeof(PropertyGrid), XamlResource = "/Syncfusion.PropertyGrid.WPF;component/Themes/Office2010BlackStyle.xaml")]
    [SkinType(SkinVisualStyle = Skin.Office2010Silver,
    Type = typeof(PropertyGrid), XamlResource = "/Syncfusion.PropertyGrid.WPF;component/Themes/Office2010SilverStyle.xaml")]
    [SkinType(SkinVisualStyle = Skin.Office2003,
    Type = typeof(PropertyGrid), XamlResource = "/Syncfusion.PropertyGrid.WPF;component/Themes/Office2003Style.xaml")]
    [SkinType(SkinVisualStyle = Skin.Blend,
    Type = typeof(PropertyGrid), XamlResource = "/Syncfusion.PropertyGrid.WPF;component/Themes/BlendStyle.xaml")]
    [SkinType(SkinVisualStyle = Skin.SyncOrange,
    Type = typeof(PropertyGrid), XamlResource = "/Syncfusion.PropertyGrid.WPF;component/Themes/SyncOrangeStyle.xaml")]
    [SkinType(SkinVisualStyle = Skin.ShinyRed,
    Type = typeof(PropertyGrid), XamlResource = "/Syncfusion.PropertyGrid.WPF;component/Themes/ShinyRedStyle.xaml")]
    [SkinType(SkinVisualStyle = Skin.ShinyBlue,
    Type = typeof(PropertyGrid), XamlResource = "/Syncfusion.PropertyGrid.WPF;component/Themes/ShinyBlueStyle.xaml")]
    [SkinType(SkinVisualStyle = Skin.VS2010,
    Type = typeof(PropertyGrid), XamlResource = "/Syncfusion.PropertyGrid.WPF;component/Themes/VS2010Style.xaml")] 
    [SkinType(SkinVisualStyle = Skin.Default,
    Type = typeof(PropertyGrid), XamlResource = "/Syncfusion.PropertyGrid.WPF;component/Themes/Generic.xaml")]
    [SkinType(SkinVisualStyle = Skin.Metro,
   Type = typeof(PropertyGrid), XamlResource = "/Syncfusion.PropertyGrid.WPF;component/Themes/MetroStyle.xaml")]
    [SkinType(SkinVisualStyle = Skin.Transparent,
Type = typeof(PropertyGrid), XamlResource = "/Syncfusion.PropertyGrid.WPF;component/Themes/TransparentStyle.xaml")]
#endif
#if SILVERLIGHT
    public class PropertyGrid : Control, ISkinStylePropagator,IDisposable
#endif
#if WPF
    public class PropertyGrid : Control,IDisposable
#endif
    {
        #region Events
        /// <summary>
        /// Occurs when [selected object changed].
        /// </summary>
        public event PropertyChangedCallback SelectedObjectChanged;
        /// <summary>
        /// Occurs when [selected property changed].
        /// </summary>
        public event PropertyChangedCallback SelectedPropertyItemChanged;
        /// <summary>
        /// Occurs when [sort direction changed].
        /// </summary>
        public event PropertyChangedCallback SortDirectionChanged;
        /// <summary>
        /// Occurs when [enable grouping changed].
        /// </summary>
        public event PropertyChangedCallback EnableGroupingChanged;
        /// <summary>
        /// Occurs when [value changed].
        /// </summary>
        public event ValueChangedEventHandler ValueChanged;
        #endregion

        #region PrivateMembers
        internal List<string> CatagoryCollection;
        public ObservableCollection<Type> FlatTypeCollection;
        internal bool isOldType = false;
        #endregion

        internal List<object> CollapsedCategoryViewItems = new List<object>();
        internal bool isCalledFromFilterPropertyGrid = false;
        #region Constructor

        /// <summary>
        /// Initializes the <see cref="PropertyGrid"/> class.
        /// </summary>
        static PropertyGrid()
        {
#if WPF
            //EnvironmentTest.ValidateLicense(typeof(PropertyGrid));
#endif
            //DefaultStyleKeyProperty.OverrideMetadata(typeof(PropertyGrid), new FrameworkPropertyMetadata(typeof(PropertyGrid)));
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="PropertyGrid"/> class.
        /// </summary>
        public PropertyGrid()
        {
            DefaultStyleKey = typeof(PropertyGrid);
            CategoryEditors = new CategoryEditorCollection();
            CustomEditorCollection = new CustomEditorCollection();
            HidePropertiesCollection = new ObservableCollection<string>();
            SelectedObjectProperties = new PropertyItemCollection();
            Properties = new PropertyCollection();

            FlatTypeCollection = new ObservableCollection<Type>();
            FlatTypeCollection.Add(typeof(String));
            FlatTypeCollection.Add(typeof(int));
            FlatTypeCollection.Add(typeof(float));
            FlatTypeCollection.Add(typeof(Char));
            FlatTypeCollection.Add(typeof(Enum));
            FlatTypeCollection.Add(typeof(Double));
            FlatTypeCollection.Add(typeof(Boolean));           
            FlatTypeCollection.Add(typeof(VerticalAlignment));
            FlatTypeCollection.Add(typeof(HorizontalAlignment)); 
            timer.Tick += new EventHandler(timer_Tick);
            timer.Interval = new TimeSpan(0,0,0,0,5);
            this.Loaded -= new RoutedEventHandler(PropertyGrid_Loaded);
            this.Loaded += new RoutedEventHandler(PropertyGrid_Loaded);
#if WPF
            if (EnvironmentTestTools.IsSecurityGranted)
            {
                EnvironmentTestTools.StartValidateLicense(typeof(PropertyGrid));
            }
#endif
        }

        void CategoryEditors_CollectionChanged(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
        {
            if (this.EnableGrouping)
            {
                this.Properties = this.GroupAndSort(this.SelectedObjectProperties);// this.ParseObject(this.SelectedObject);
                //this.GroupAndSort(this.SelectedObjectProperties);
            }
        }
        #endregion

        #region Dependency Properties




        public PropertyItem SelectedPropertyItem      
        {
            get { return (PropertyItem)GetValue(SelectedPropertyItemProperty); }
            set { SetValue(SelectedPropertyItemProperty, value); }
        }

        // Using a DependencyProperty as the backing store for MyProperty.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty SelectedPropertyItemProperty =
            DependencyProperty.Register("SelectedPropertyItem", typeof(PropertyItem), typeof(PropertyGrid), new PropertyMetadata(null, OnSelectedPropertyItemChanged));

        /// <summary>
        /// enable or disable animation while loading selectedobject
        /// </summary>
        public bool DisableAnimationOnObjectSelection
        {
            get { return (bool)GetValue(DisableAnimationOnObjectSelectionProperty); }
            set { SetValue(DisableAnimationOnObjectSelectionProperty, value); }
        }

        /// <summary>
        /// enable or disable animation while loading selectedobject
        /// </summary>
        public static readonly DependencyProperty DisableAnimationOnObjectSelectionProperty =
            DependencyProperty.Register("DisableAnimationOnObjectSelection", typeof(bool), typeof(PropertyGrid), new PropertyMetadata(false));

    
        

        public IEnumerable SelectedItems
        {
            get { return (IEnumerable)GetValue(SelectedItemsProperty); }
            set { SetValue(SelectedItemsProperty, value); }
        }

        // Using a DependencyProperty as the backing store for SelectedItems.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty SelectedItemsProperty =
            DependencyProperty.Register("SelectedItems", typeof(IEnumerable), typeof(PropertyGrid), new PropertyMetadata(null, new PropertyChangedCallback(OnSelectedItemsChanged)));




        /// <summary>
        /// Gets or sets the editable background.
        /// </summary>
        /// <value>The editable background.</value>
        public Brush EditableBackground
        {
            get { return (Brush)GetValue(EditableBackgroundProperty); }
            set { SetValue(EditableBackgroundProperty, value); }
        }

        // Using a DependencyProperty as the backing store for EditableBackground.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty EditableBackgroundProperty =
            DependencyProperty.Register("EditableBackground", typeof(Brush), typeof(PropertyGrid), new PropertyMetadata(null));



        public Visibility SearchBoxVisibility
        {
            get { return (Visibility)GetValue(SearchBoxVisibilityProperty); }
            set { SetValue(SearchBoxVisibilityProperty, value); }
        }

        // Using a DependencyProperty as the backing store for SearchBoxVisibility.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty SearchBoxVisibilityProperty =
            DependencyProperty.Register("SearchBoxVisibility", typeof(Visibility), typeof(PropertyGrid), new PropertyMetadata(Visibility.Visible));


        public PropertyExpandModes PropertyExpandMode
        {
            get { return (PropertyExpandModes)GetValue(PropertyExpandModeProperty); }
            set { SetValue(PropertyExpandModeProperty, value); }
        }

        // Using a DependencyProperty as the backing store for PropertyVisiblityMode.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty PropertyExpandModeProperty =
            DependencyProperty.Register("PropertyExpandMode", typeof(PropertyExpandModes), typeof(PropertyGrid), new PropertyMetadata(PropertyExpandModes.FlatMode, new PropertyChangedCallback(OnPropertyExpandModeChanged)));



        public bool EnableToolTip
        {
            get { return (bool)GetValue(EnableToolTipProperty); }
            set { SetValue(EnableToolTipProperty, value); }
        }

        // Using a DependencyProperty as the backing store for EnableToolTip.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty EnableToolTipProperty =
            DependencyProperty.Register("EnableToolTip", typeof(bool), typeof(PropertyGrid), new PropertyMetadata(true,new PropertyChangedCallback(OnEnableToolTipChanged)));


        


        /// <summary>
        /// Gets or sets the editable font weight.
        /// </summary>
        /// <value>The editable font weight.</value>
        public FontWeight EditableFontWeight
        {
            get { return (FontWeight)GetValue(EditableFontWeightProperty); }
            set { SetValue(EditableFontWeightProperty, value); }
        }
       
        // Using a DependencyProperty as the backing store for EditableFontWeight.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty EditableFontWeightProperty =
            DependencyProperty.Register("EditableFontWeight", typeof(FontWeight), typeof(PropertyGrid), new PropertyMetadata(FontWeights.Normal));


        /// <summary>
        /// Gets or sets the read only background.
        /// </summary>
        /// <value>The read only background.</value>
        public Brush ReadOnlyBackground
        {
            get { return (Brush)GetValue(ReadOnlyBackgroundProperty); }
            set { SetValue(ReadOnlyBackgroundProperty, value); }
        }

        // Using a DependencyProperty as the backing store for ReadOnlyBackground.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty ReadOnlyBackgroundProperty =
            DependencyProperty.Register("ReadOnlyBackground", typeof(Brush), typeof(PropertyGrid), new PropertyMetadata(null));



        /// <summary>
        /// Gets or sets the read only font weight.
        /// </summary>
        /// <value>The read only font weight.</value>
        public FontWeight ReadOnlyFontWeight
        {
            get { return (FontWeight)GetValue(ReadOnlyFontWeightProperty); }
            set { SetValue(ReadOnlyFontWeightProperty, value); }
        }

        // Using a DependencyProperty as the backing store for ReadOnlyFontWeight.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty ReadOnlyFontWeightProperty =
            DependencyProperty.Register("ReadOnlyFontWeight", typeof(FontWeight), typeof(PropertyGrid), new PropertyMetadata(FontWeights.Normal));

        


        /// <summary>
        /// Gets or sets the button panel visibility.
        /// </summary>
        /// <value>The button panel visibility.</value>
        public Visibility ButtonPanelVisibility
        {
            get { return (Visibility)GetValue(ButtonPanelVisibilityProperty); }
            set { SetValue(ButtonPanelVisibilityProperty, value); }
        }

#if WPF
        public static readonly DependencyProperty ButtonPanelVisibilityProperty =
            DependencyProperty.Register("ButtonPanelVisibility", typeof(Visibility), typeof(PropertyGrid), new PropertyMetadata(Visibility.Visible));
#endif
#if SILVERLIGHT
        public static readonly DependencyProperty ButtonPanelVisibilityProperty =
            DependencyProperty.Register("ButtonPanelVisibility", typeof(Visibility), typeof(PropertyGrid), new PropertyMetadata(Visibility.Collapsed));
#endif
        

        /// <summary>
        /// Gets or sets the text color used for category headings.
        /// </summary>
        /// <value>The category foreground.</value>
        [Category("Appearance")]
        public Brush CategoryForeground
        {
            get { return (Brush)GetValue(CategoryForegroundProperty); }
            set { SetValue(CategoryForegroundProperty, value); }
        }

        /// <summary>
        /// 
        /// </summary>
        public static readonly DependencyProperty CategoryForegroundProperty =
            DependencyProperty.Register("CategoryForeground", typeof(Brush), typeof(PropertyGrid), new PropertyMetadata(OnCategoryForegroundChanged));

        /// <summary>
        /// Gets or sets the color of the borders and category heading background.
        /// </summary>
        /// <value>The color of the line.</value>
        [Category("Appearance")]
        public Brush LineColor
        {
            get { return (Brush)GetValue(LineColorProperty); }
            set { SetValue(LineColorProperty, value); }
        }

        /// <summary>
        /// 
        /// </summary>
        public static readonly DependencyProperty LineColorProperty =
            DependencyProperty.Register("LineColor", typeof(Brush), typeof(PropertyGrid), new PropertyMetadata(OnLineColorChanged));

        /// <summary>
        /// Gets or sets the background color of the propertyview.
        /// </summary>
        /// <value>The color of the view background.</value>
        [Category("Appearance")]
        public Brush ViewBackgroundColor
        {
            get { return (Brush)GetValue(ViewBackgroundColorProperty); }
            set { SetValue(ViewBackgroundColorProperty, value); }
        }

        /// <summary>
        /// 
        /// </summary>
        public static readonly DependencyProperty ViewBackgroundColorProperty =
            DependencyProperty.Register("ViewBackgroundColor", typeof(Brush), typeof(PropertyGrid), new PropertyMetadata(OnViewBackgroundColorChanged));


        [Category("Behavior")]
        public string DefaultPropertyPath
        {
            get { return (string)GetValue(DefaultPropertyPathProperty); }
            set { SetValue(DefaultPropertyPathProperty, value); }
        }

        // Using a DependencyProperty as the backing store for DefaultPropertyPath.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty DefaultPropertyPathProperty =
            DependencyProperty.Register("DefaultPropertyPath", typeof(string), typeof(PropertyGrid), new PropertyMetadata(String.Empty));


        

        ///// <summary>
        ///// Gets or sets the expander style.
        ///// </summary>
        ///// <value>The expander style.</value>
        //[Category("Appearance")]
        //public Style ExpanderStyle
        //{
        //    get { return (Style)GetValue(ExpanderStyleProperty); }
        //    set { SetValue(ExpanderStyleProperty, value); }
        //}

        ///// <summary>
        ///// 
        ///// </summary>
        //public static readonly DependencyProperty ExpanderStyleProperty =
        //    DependencyProperty.Register("ExpanderStyle", typeof(Style), typeof(PropertyGrid), new PropertyMetadata(OnExpanderStyleChanged));

        /// <summary>
        /// Gets or sets a value indicating whether [enable grouping].
        /// </summary>
        /// <value><c>true</c> if [enable grouping]; otherwise, <c>false</c>.</value>
        public bool EnableGrouping
        {
            get { return (bool)GetValue(EnableGroupingProperty); }
            set { SetValue(EnableGroupingProperty, value); }
        }

        /// <summary>
        /// 
        /// </summary>
        public static readonly DependencyProperty EnableGroupingProperty =
            DependencyProperty.Register("EnableGrouping", typeof(bool), typeof(PropertyGrid), new PropertyMetadata(false, OnEnableGroupingChanged));

        /// <summary>
        /// Gets or sets the object for which the grid displays properties.
        /// </summary>
        /// <value>The selected object.</value>
        [CategoryAttribute("Behavior"), DescriptionAttribute("Sets the currently selected object that the grid will browse.")]
        public object SelectedObject
        {
            get { return (object)GetValue(SelectedObjectProperty); }
            set { SetValue(SelectedObjectProperty, value); }
        }
       
        /// <summary>
        /// 
        /// </summary>
        public static readonly DependencyProperty SelectedObjectProperty =
            DependencyProperty.Register("SelectedObject", typeof(object), typeof(PropertyGrid), new PropertyMetadata(null, OnSelectedObjectChanged));
        /// <summary>
        /// 
        /// </summary>
#if WPF
        /// <summary>
        /// Gets or sets the visual style for PropertyGrid.
        /// </summary>
        /// <value>The visual style.</value>
        public string VisualStyle
        {
            get { return (string)GetValue(VisualStyleProperty); }
            set { SetValue(VisualStyleProperty, value); }
        }
        // Using a DependencyProperty as the backing store for VisualStyle.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty VisualStyleProperty =
                   DependencyProperty.Register("VisualStyle", typeof(string), typeof(PropertyGrid), new PropertyMetadata("Default", OnVisualStyleChanged));
#endif
        /// <summary>
        /// Gets or sets the properties.
        /// </summary>
        /// <value>The properties.</value>
        [EditorBrowsable(EditorBrowsableState.Never)]
        [BrowsableAttribute(false)]
        public PropertyCollection Properties
        {
            get { return (PropertyCollection)GetValue(PropertiesProperty); }
            internal set { SetValue(PropertiesProperty, value); }
        }

        /// <summary>
        /// 
        /// </summary>
        public static readonly DependencyProperty PropertiesProperty =
            DependencyProperty.Register("Properties", typeof(PropertyCollection), typeof(PropertyGrid), new PropertyMetadata(null));

        /// <summary>
        /// Gets or sets the category editors.
        /// </summary>
        /// <value>The category editors.</value>
        public CategoryEditorCollection CategoryEditors
        {
            get { return (CategoryEditorCollection)GetValue(CategoryEditorsProperty); }
            set { SetValue(CategoryEditorsProperty, value); }
        }

        public static readonly DependencyProperty CategoryEditorsProperty =
            DependencyProperty.Register("CategoryEditors", typeof(CategoryEditorCollection), typeof(PropertyGrid), new PropertyMetadata(null));

#if SILVERLIGHT
        internal PagedCollectionView PagedCollection
        {
            get { return (PagedCollectionView)GetValue(PagedCollectionProperty); }
            set { SetValue(PagedCollectionProperty, value); }
        }

        internal static readonly DependencyProperty PagedCollectionProperty =
            DependencyProperty.Register("PagedCollection", typeof(PagedCollectionView), typeof(PropertyGrid), new PropertyMetadata(null));
#endif

        /// <summary>
        /// Gets or sets the selected object properties.
        /// </summary>
        /// <value>The selected object properties.</value>
        internal PropertyItemCollection SelectedObjectProperties
        {
            get { return (PropertyItemCollection)GetValue(SelectedObjectPropertiesProperty); }
            set { SetValue(SelectedObjectPropertiesProperty, value); }
        }

        /// <summary>
        /// 
        /// </summary>
        internal static readonly DependencyProperty SelectedObjectPropertiesProperty =
            DependencyProperty.Register("SelectedObjectProperties", typeof(PropertyItemCollection), typeof(PropertyGrid), new PropertyMetadata(null));




        public Visibility DescriptionPanelVisibility
        {
            get { return (Visibility)GetValue(DescriptionPanelVisibilityProperty); }
            set { SetValue(DescriptionPanelVisibilityProperty, value); }
        }

        // Using a DependencyProperty as the backing store for DescriptionPanelVisibility.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty DescriptionPanelVisibilityProperty =
            DependencyProperty.Register("DescriptionPanelVisibility", typeof(Visibility), typeof(PropertyGrid), new PropertyMetadata(Visibility.Collapsed));

#if WPF
        public GridLength DescriptionPanelHeight
        {
            get { return (GridLength)GetValue(DescriptionPanelHeightProperty); }
            set { SetValue(DescriptionPanelHeightProperty, value); }
        }

        // Using a DependencyProperty as the backing store for DescriptionPanelHeight.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty DescriptionPanelHeightProperty =
            DependencyProperty.Register("DescriptionPanelHeight", typeof(GridLength), typeof(PropertyGrid), new PropertyMetadata(new PropertyChangedCallback(OnDescriptionPanelHeightChanged)));
#endif
        /// <summary>
        ///   Gets or sets the type of sorting the PropertyGrid uses to display properties.
        /// </summary>
        /// <value>The sort direction.</value>
        [Category("Appearance")]
        public ListSortDirection SortDirection
        {
            get { return (ListSortDirection)GetValue(SortDirectionProperty); }
            set { SetValue(SortDirectionProperty, value); }
        }

        /// <summary>
        /// 
        /// </summary>
        public static readonly DependencyProperty SortDirectionProperty =
            DependencyProperty.Register("SortDirection", typeof(ListSortDirection), typeof(PropertyGrid), new PropertyMetadata(ListSortDirection.Ascending, OnSortDirectionChanged));



        /// <summary>
        /// Gets or sets the CollapseSidePanel properties.
        /// </summary>
        /// <value>The CollapseSidePanel.</value>
        public bool CollapseSidePanel
        {
            get { return (bool)GetValue(CollapseSidePanelProperty); }
            set { SetValue(CollapseSidePanelProperty, value); }
        }

        // Using a DependencyProperty as the backing store for CollapseSidePanel.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty CollapseSidePanelProperty =
            DependencyProperty.Register("CollapseSidePanel", typeof(bool), typeof(PropertyGrid), new PropertyMetadata(false));

        

        /// <summary>
        /// Gets or sets the hide properties collection.
        /// </summary>
        /// <value>The hide properties collection.</value>
        public ObservableCollection<string> HidePropertiesCollection
        {
            get { return (ObservableCollection<string>)GetValue(HidePropertiesCollectionProperty); }
            set { SetValue(HidePropertiesCollectionProperty, value); }
        }

        /// <summary>
        /// 
        /// </summary>
        public static readonly DependencyProperty HidePropertiesCollectionProperty =
            DependencyProperty.Register("HidePropertiesCollection", typeof(ObservableCollection<string>), typeof(PropertyGrid), new PropertyMetadata(null));

        /// <summary>
        /// Gets or sets the custom editor collection.
        /// </summary>
        /// <value>The custom editor collection.</value>
        public CustomEditorCollection CustomEditorCollection
        {
            get { return (CustomEditorCollection)GetValue(CustomEditorCollectionProperty); }
            set { SetValue(CustomEditorCollectionProperty, value); }
        }

        /// <summary>
        /// 
        /// </summary>
        public static readonly DependencyProperty CustomEditorCollectionProperty =
            DependencyProperty.Register("CustomEditorCollection", typeof(CustomEditorCollection), typeof(PropertyGrid), new PropertyMetadata(null));


        #endregion

        #region Methods

        /// <summary>
        /// Refesh the property Grid.
        /// </summary>
        public void RefreshPropertygrid()
        {
            if (PART_PropertyView != null)
            {
                PART_PropertyView.SelectedItem = null;
            }
            if(this.Properties!=null)
                this.Properties.Clear();
            this.UpdatePropertyCollection();
        }
        public void Dispose()
        {
            if (Properties != null)
            {
                Properties = null;
            }  
            if (this.CustomEditorCollection != null)
            {
                this.CustomEditorCollection.Clear();
                this.CustomEditorCollection = null;
            }
        }
        internal PropertyItemCollection propertyCollection;
        internal bool ToggleCheckedFalg = false;
        /// <summary>
        /// Parses the object.
        /// </summary>
        /// <param name="objItem">The obj item.</param>
        /// <returns></returns>
        internal PropertyCollection ParseObject(object objItem)
        {
            if (null == objItem)
            {
                this.SelectedObjectProperties = null;
                return new PropertyCollection();
            }

            propertyCollection = new PropertyItemCollection();
            CatagoryCollection = new List<string>();

            Type type = objItem.GetType();
            var properties = type.GetProperties();

            foreach (PropertyInfo pinfo in properties)
            {
                PropertyItem propertyItem = new PropertyItem();
                propertyItem.PropertyLevel = 1;
                if (DefaultPropertyPath == pinfo.Name)
                {
                    if (PART_PropertyView != null && PART_PropertyView.SelectedItem == null)
                    {
                        propertyItem.IsSelected = true;
                    }
                }
#if WPF
                BrowsableAttribute browsableAttribute = null;
                if (TypeDescriptor.GetProperties(type)[pinfo.Name] != null)
                {
                    browsableAttribute =
                        TypeDescriptor.GetProperties(type)[pinfo.Name].Attributes[typeof (BrowsableAttribute)] as
                            BrowsableAttribute;
                    var expandMode = TypeDescriptor.GetProperties(type)[pinfo.Name].Attributes[typeof(PropertyExpandModeAttribute)] as PropertyExpandModeAttribute;
                    if (expandMode != null && expandMode.ExpandMode!=null)
                    {
                        propertyItem.PropertyExpandMode =(PropertyExpandModes)Enum.Parse(typeof(PropertyExpandModes), (string)expandMode.ExpandMode);
                    }
                }
#endif
#if SILVERLIGHT
                BrowsableAttribute browsableAttribute =PropertyItem.GetAttribute<BrowsableAttribute>(pinfo);
#endif
                if (null != browsableAttribute)
                    propertyItem._browsable = browsableAttribute.Browsable;

                if (propertyItem.Browsable)
                {
#if WPF
                    EditorBrowsableAttribute editorBrowsableAttribute = null;
                    if (TypeDescriptor.GetProperties(type)[pinfo.Name] != null)
                    {
                       editorBrowsableAttribute =
                            TypeDescriptor.GetProperties(type)[pinfo.Name].Attributes[typeof (EditorBrowsableAttribute)]
                                as EditorBrowsableAttribute;
                    }
#endif
#if SILVERLIGHT
                    EditorBrowsableAttribute editorBrowsableAttribute = PropertyItem.GetAttribute<EditorBrowsableAttribute>(pinfo);
#endif
                    if (null != editorBrowsableAttribute && editorBrowsableAttribute.State == EditorBrowsableState.Never)
                        propertyItem._browsable = false;
                }

                if (propertyItem.Browsable)
                {
#if WPF
                    ReadOnlyAttribute readOnlyAttribute = null;
                    if (TypeDescriptor.GetProperties(type)[pinfo.Name] != null)
                    {
                        readOnlyAttribute =
                            TypeDescriptor.GetProperties(type)[pinfo.Name].Attributes[typeof (ReadOnlyAttribute)] as
                                ReadOnlyAttribute;
                    }
#endif
#if SILVERLIGHT
                    ReadOnlyAttribute readOnlyAttribute = PropertyItem.GetAttribute<ReadOnlyAttribute>(pinfo);
#endif
                    if (readOnlyAttribute != null)
                        propertyItem._isReadOnly = readOnlyAttribute.IsReadOnly;

                    try
                    {
                        object value = pinfo.GetValue(objItem, null);
                        propertyItem.Value = value;
                        propertyItem.PropertyInformation = pinfo;
                        propertyItem.SelectedObject = this.SelectedObject;
                        propertyItem.PropertyGrid = this;
#if WPF
                        CategoryAttribute attr = null;
                        if (TypeDescriptor.GetProperties(type)[pinfo.Name] != null)
                        {
                            attr =
                                TypeDescriptor.GetProperties(type)[pinfo.Name].Attributes[typeof (CategoryAttribute)] as
                                    CategoryAttribute;
                        }
#endif
#if SILVERLIGHT
                        CategoryAttribute attr = PropertyItem.GetAttribute<CategoryAttribute>(pinfo);
#endif
                        if (attr != null && !string.IsNullOrEmpty(attr.Category))
                            propertyItem._category = attr.Category;
                        else
                            propertyItem._category = "Misc";
                        //string _category;
                        //CategoryAttribute attr = PropertyItem.GetAttribute<CategoryAttribute>(pinfo);
                        //if (attr != null && !string.IsNullOrEmpty(attr.Category))
                        //    _category = attr.Category;
                        //else
                        //    _category = "Other";
                        //propertyItem.Category = _category;
                        //propertyItem.Template = null;
#if WPF
                        DisplayNameAttribute display = null;
                        if (TypeDescriptor.GetProperties(type)[pinfo.Name] != null)
                        {
                            display =
                                TypeDescriptor.GetProperties(type)[pinfo.Name].Attributes[typeof (DisplayNameAttribute)]
                                    as DisplayNameAttribute;
                        }
                        if (display != null && !string.IsNullOrEmpty(display.DisplayName))
                            propertyItem.displayName = display.DisplayName;

                        DescriptionAttribute description = null;
                        if (TypeDescriptor.GetProperties(type)[pinfo.Name] != null)
                        {
                            description =
                                TypeDescriptor.GetProperties(type)[pinfo.Name].Attributes[typeof (DescriptionAttribute)]
                                    as DescriptionAttribute;
                        }
                        if (display != null && !string.IsNullOrEmpty(description.Description))
                            propertyItem._description = description.Description;
#endif
#if SILVERLIGHT
                        propertyItem.Template = ResourceManager.GetValueTemplate(pinfo.PropertyType,SkinManager.GetVisualStyle(this).ToString());
#endif
#if WPF
                        try
                        {
                            propertyItem.Template = ResourceManager.GetValueTemplate(pinfo.PropertyType);
                        }
                        catch
                        { }
#endif

                        bool IsCustomEditorSet = false;

                        if (this.CustomEditorCollection != null)
                        {
                            if (this.CustomEditorCollection.Count > 0)
                            {
                                ITypeCustomEditor customeditor = this.CustomEditorCollection[propertyItem.Name, propertyItem.PropertyType];
                                if (customeditor != null)
                                {
                                    propertyItem.Editor = customeditor.Editor;
                                    IsCustomEditorSet = true;
                                }
                            }
                        }

                        if (!IsCustomEditorSet)
                        {
                            if (propertyItem.PropertyType == typeof(Brush))
                            {
#if SILVERLIGHT
                                propertyItem.Editor = new BrushSelectorEditor();
#endif
#if WPF
                                //propertyItem.Editor = new TextBoxEditor();
                                propertyItem.Editor = new BrushSelectorEditor();
#endif
                            }
                            else if (propertyItem.PropertyType.IsEnum)
                                propertyItem.Editor = new EnumComboEditor();
                            else if (propertyItem.PropertyType == typeof(bool))
                                propertyItem.Editor = new CheckBoxEditor();
                            else if (propertyItem.PropertyType == typeof(bool?))
                                propertyItem.Editor = new BooleanComboEditor();
                            else if (propertyItem.PropertyType == typeof(double))
                                propertyItem.Editor = new DoubleTextBoxEditor();
                            else if (propertyItem.PropertyType == typeof(double?))
                                propertyItem.Editor = new DoubleTextBoxEditor();
                            else if (propertyItem.PropertyType == typeof(int))
                                propertyItem.Editor = new IntegerTextBoxEditor();
                            else if (propertyItem.PropertyType == typeof(int?))
                                propertyItem.Editor = new IntegerTextBoxEditor();
                            else if (propertyItem.PropertyType == typeof(DateTime))
                                propertyItem.Editor = new DateTimeEditor();
                            else if (propertyItem.PropertyType == typeof(System.Windows.Controls.ItemCollection))
                            {
                                propertyItem.Editor = new TextBoxEditor();
                            }
                            else if (propertyItem.PropertyType == typeof(ImageSource))
#if WPF
                            propertyItem.Editor = new TextBoxEditor();
#endif
#if SILVERLIGHT
                                propertyItem.Editor = new TextBoxEditor();
#endif
                            else if (propertyItem.PropertyType == typeof(string))
                                propertyItem.Editor = new TextBoxEditor();
                            else if (propertyItem.PropertyType == typeof(FontFamily) || propertyItem.PropertyType == typeof(FontWeight) || propertyItem.PropertyType == typeof(FontStyle) || propertyItem.PropertyType == typeof(FontStretch))
                                propertyItem.Editor = new FontComboEditor();

#if WPF
                            else if (propertyItem.Name.ToString().Equals("ItemsSource"))
                            {
                                propertyItem.Editor = new ITypeItemsSourceControl();
                            }
#endif
                            else
                                propertyItem.Editor = new TextBoxEditor();
                        }

                        if (HidePropertiesCollection.Count > 0)
                        {
                            if (!HidePropertiesCollection.Contains(propertyItem.Name))
                            {
                                propertyCollection.Add(propertyItem);
                                if (!CatagoryCollection.Contains(propertyItem.Category))
                                {
                                    CatagoryCollection.Add(propertyItem.Category);
                                }
                            }
                        }
                        else
                        {
                            propertyCollection.Add(propertyItem);
                            if (!CatagoryCollection.Contains(propertyItem.Category))
                            {
                                CatagoryCollection.Add(propertyItem.Category);
                            }
                        }
                        //Heirarchy Parsing
#if WPF
                        if (this.PropertyExpandMode == PropertyExpandModes.NestedMode || propertyItem.PropertyExpandMode== PropertyExpandModes.NestedMode)
                        {
                            if (value != null && properties.Count() > 0 &&  !FlatTypeCollection.Contains(value.GetType()))
                            {
                                object objvalue = pinfo.GetValue(objItem, null);

                                propertyItem.SelectedObjectProperties = ParseInternal(objvalue, pinfo.Name, propertyItem.PropertyLevel);
                            }
                        }
#endif

                    }
                    catch
                    {
                    }
                }
            }
            this.SelectedObjectProperties = propertyCollection;

#if SILVERLIGHT
            return GroupAndSort(propertyCollection);
#endif
#if WPF

            return GroupAndSort(propertyCollection);
#endif

        }

#if WPF
        /// <summary>
        /// Groups the and sort.
        /// </summary>
        /// <param name="propertyCollection">The property collection.</param>
        /// <returns></returns>
        internal PropertyCollection GroupAndSort(PropertyItemCollection propertyCollection)
        {
            if (this.EnableGrouping)
            {
                if (propertyCollection !=null && propertyCollection.Count > 0 && this.CategoryEditors != null)
                {   
                    foreach (CategoryEditor item in this.CategoryEditors)
                    {
                        ClearCategoryValues(item, propertyCollection);
                        PropertyItem valueEditorItem = new PropertyItem() { PropertyGrid = this };
                        foreach (CategoryEditorProperty property in item.Properties)
                        {
                            PropertyItem pitem = new PropertyItem();
                            pitem = propertyCollection[property.Name];

                            if (pitem != null)
                            {
                                valueEditorItem.Template = item.EditorTemplate;
                                valueEditorItem.SelectedObject = pitem.SelectedObject;
                                valueEditorItem.PropertyInformation = pitem.PropertyInformation;
                                valueEditorItem.IsCategoryEditorEnabled = true;
                                if (!string.IsNullOrEmpty(item.Category))
                                    valueEditorItem._category = item.Category;
                                else
                                    valueEditorItem._category = "Other";

                                valueEditorItem.CategoryValueProperties.Add(pitem);
                                if (pitem != null)
                                    propertyCollection.Remove(pitem);
                            }
                        }
                        propertyCollection.Add(valueEditorItem);
                    }

                    src = new CollectionViewSource();
                    src.Source = propertyCollection;
                    src.SortDescriptions.Add(new SortDescription("Name", SortDirection));
                    src.GroupDescriptions.Add(new PropertyGroupDescription("Category"));
                    if (PART_SearchText != null && !String.IsNullOrEmpty(PART_SearchText.Value))
                    {
                        src.Filter += new FilterEventHandler(view_Filter);
                    }
                    CollectionViewSource viewsource = new CollectionViewSource();
                    viewsource.Source = src.View.Groups;
                    viewsource.SortDescriptions.Add(new SortDescription("Name", SortDirection));
                    PropertyCollection _propertyCollection = new PropertyCollection();
                    foreach (System.Windows.Data.CollectionViewGroup item in viewsource.View)
                    {
                        PropertyCategoryViewItemCollection _propertyCategoryViewItemCollection = new PropertyCategoryViewItemCollection();
                        if (item.Name != null)
                        {
                            _propertyCategoryViewItemCollection.Category = item.Name.ToString();
                        }
                        foreach (var propertyitem in item.Items)
                        {
                            _propertyCategoryViewItemCollection.Properties.Add((PropertyItem)propertyitem);
                        }
                        if (item.Name != null)
                        {
                            _propertyCollection.Add(_propertyCategoryViewItemCollection);
                        }

                    }
                    return _propertyCollection;
                }
                return new PropertyCollection();
            }
            else
            {
                src = new CollectionViewSource();
                src.Source = propertyCollection;
                src.SortDescriptions.Add(new SortDescription("Name", SortDirection));

                PropertyCollection _propertyCollection = new PropertyCollection();

                foreach (var item in ((System.Windows.Data.ListCollectionView)(src.View)))
                {
                    _propertyCollection.Add((PropertyItem)item);
                }
                return _propertyCollection;
            }
        }

        internal void ClearCategoryValues(CategoryEditor categoryeditor, PropertyItemCollection propertycollection)
        {
            foreach (CategoryEditorProperty property in categoryeditor.Properties)
            {
                PropertyItem item = propertycollection[property.Name];
                if (item != null)
                item.CategoryValueProperties.Clear();
            }
        }
#endif
#if WPF
        internal PropertyItemCollection ParseInternal(object objItem, string name, double level)
        {
            if (null == objItem)
                return new PropertyItemCollection();

            PropertyItemCollection propertyCollection = new PropertyItemCollection();
            CatagoryCollection = new List<string>();

            Type type = objItem.GetType();

            PropertyDescriptorCollection propertycollection = TypeDescriptor.GetProperties(type);

            var properties = type.GetProperties();

            foreach (PropertyInfo pinfo in properties)
            {
                PropertyItem propertyItem = new PropertyItem();
                propertyItem.PropertyLevel = level + 1;
                if (DefaultPropertyPath == pinfo.Name)
                {
                    if (PART_PropertyView != null && PART_PropertyView.SelectedItem == null)
                    {
                        propertyItem.IsSelected = true;
                    }
                }

#if WPF
                if (TypeDescriptor.GetProperties(type)[pinfo.Name] != null)
                {
                    var expandMode = TypeDescriptor.GetProperties(type)[pinfo.Name].Attributes[typeof(PropertyExpandModeAttribute)] as PropertyExpandModeAttribute;
                    if (expandMode != null && expandMode.ExpandMode != null)
                    {
                        propertyItem.PropertyExpandMode = (PropertyExpandModes)Enum.Parse(typeof(PropertyExpandModes), (string)expandMode.ExpandMode);
                    }
                }
#endif
                BrowsableAttribute browsableAttribute = PropertyItem.GetAttribute<BrowsableAttribute>(pinfo);

                if (null != browsableAttribute)
                    propertyItem._browsable = browsableAttribute.Browsable;

                if (propertyItem.Browsable)
                {
                    EditorBrowsableAttribute editorBrowsableAttribute = PropertyItem.GetAttribute<EditorBrowsableAttribute>(pinfo);
                    if (null != editorBrowsableAttribute && editorBrowsableAttribute.State == EditorBrowsableState.Never)
                        propertyItem._browsable = false;
                }

                //    if ((propertyDescriptor.Converter.GetType()).BaseType.UnderlyingSystemType.FullName == "System.ComponentModel.ExpandableObjectConverter" || (propertyDescriptor.Converter.GetType() == typeof(System.ComponentModel.ExpandableObjectConverter)))
                //    {o
                //        bol IsExpandProperties = propertyDescriptor.Converter.GetPropertiesSupported();
                //        if (IsExpandProperties)
                //        {
                if (propertyItem.Browsable)
                {
                    ReadOnlyAttribute readOnlyAttribute = PropertyItem.GetAttribute<ReadOnlyAttribute>(pinfo);
                    if (readOnlyAttribute != null)
                        propertyItem._isReadOnly = readOnlyAttribute.IsReadOnly;
                    try
                    {
                        if (pinfo.Name != "Length")
                        {
                            object value = pinfo.GetValue(objItem, null);

                            propertyItem.Value = value;
                            propertyItem.PropertyInformation = pinfo;
                            propertyItem.SelectedObject = name;

                            propertyItem.PropertyGrid = this;
                        }
                        CategoryAttribute attr = PropertyItem.GetAttribute<CategoryAttribute>(pinfo);
                        if (attr != null && !string.IsNullOrEmpty(attr.Category))
                            propertyItem._category = attr.Category;
                        else
                            propertyItem._category = "Misc";
                        //string _category;
                        //CategoryAttribute attr = PropertyItem.GetAttribute<CategoryAttribute>(pinfo);
                        //if (attr != null && !string.IsNullOrEmpty(attr.Category))
                        //    _category = attr.Category;
                        //else
                        //    _category = "Other";
                        //propertyItem.Category = _category;

                        try
                        {
                            propertyItem.Template = ResourceManager.GetValueTemplate(pinfo.PropertyType);
                        }
                        catch { }


                        bool IsCustomEditorSet = false;

                        if (this.CustomEditorCollection != null)
                        {
                            if (this.CustomEditorCollection.Count > 0)
                            {
                                ITypeCustomEditor customeditor = this.CustomEditorCollection[propertyItem.Name, propertyItem.PropertyType];
                                if (customeditor != null)
                                {
                                    propertyItem.Editor = customeditor.Editor;
                                    IsCustomEditorSet = true;
                                }
                            }
                        }

                        if (!IsCustomEditorSet)
                        {
                            if (propertyItem.PropertyType == typeof(Brush))
                            {

                                //propertyItem.Editor = new TextBoxEditor();
                                propertyItem.Editor = new BrushSelectorEditor();

                            }
                            else if (propertyItem.PropertyType.IsEnum)
                                propertyItem.Editor = new EnumComboEditor();
                            else if (propertyItem.PropertyType == typeof(bool))
                                propertyItem.Editor = new CheckBoxEditor();
                            else if (propertyItem.PropertyType == typeof(bool?))
                                propertyItem.Editor = new BooleanComboEditor();
                            else if (propertyItem.PropertyType == typeof(double))
                                propertyItem.Editor = new DoubleTextBoxEditor();
                            else if (propertyItem.PropertyType == typeof(double?))
                                propertyItem.Editor = new DoubleTextBoxEditor();
                            else if (propertyItem.PropertyType == typeof(int))
                                propertyItem.Editor = new IntegerTextBoxEditor();
                            else if (propertyItem.PropertyType == typeof(int?))
                                propertyItem.Editor = new IntegerTextBoxEditor();
                            else if (propertyItem.PropertyType == typeof(DateTime))
                                propertyItem.Editor = new DateTimeEditor();
                            else if (propertyItem.PropertyType == typeof(System.Windows.Controls.ItemCollection))
                            {
                                propertyItem.Editor = new TextBoxEditor();
                            }
                            else if (propertyItem.PropertyType == typeof(ImageSource))

                                propertyItem.Editor = new TextBoxEditor();


                            else if (propertyItem.PropertyType == typeof(string))
                                propertyItem.Editor = new TextBoxEditor();
                            else if (propertyItem.PropertyType == typeof(FontFamily) || propertyItem.PropertyType == typeof(FontWeight) || propertyItem.PropertyType == typeof(FontStyle) || propertyItem.PropertyType == typeof(FontStretch))
                                propertyItem.Editor = new FontComboEditor();


                            else if (propertyItem.Name.ToString().Equals("ItemsSource"))
                            {
                                propertyItem.Editor = new ITypeItemsSourceControl();
                            }

                            else
                                propertyItem.Editor = new TextBoxEditor();
                        }

                        if (HidePropertiesCollection.Count > 0)
                        {
                            if (!HidePropertiesCollection.Contains(propertyItem.Name))
                            {
                                propertyCollection.Add(propertyItem);
                            }
                        }
                        else
                        {
                            propertyCollection.Add(propertyItem);

                            if (properties.Count() > 0)
                            {
                                foreach (PropertyDescriptor propertyDescriptor in propertycollection)
                                {

                                    if ((propertyDescriptor.Converter.GetType()).BaseType.UnderlyingSystemType.FullName == "System.ComponentModel.ExpandableObjectConverter" || (propertyDescriptor.Converter.GetType() == typeof(System.ComponentModel.ExpandableObjectConverter)))
                                    {
                                        object objvalue = pinfo.GetValue(objItem, null);
                                        //if (!objvalue.Equals(0) && objvalue != null)
                                        //    propertyItem.PropertyLevel = propertyItem.PropertyLevel + 1;
                                        propertyItem.SelectedObjectProperties = ParseInternal(objvalue, pinfo.Name, propertyItem.PropertyLevel);
                                    }
                                }
                            }
                        }
#if WPF
                        if (propertyItem.PropertyExpandMode == PropertyExpandModes.NestedMode)
                        {
                            object objvalue = pinfo.GetValue(objItem, null);
                            if (objvalue != null && properties.Count() > 0 && !FlatTypeCollection.Contains(objvalue.GetType()))
                            {
                                propertyItem.SelectedObjectProperties = ParseInternal(objvalue, pinfo.Name, propertyItem.PropertyLevel);
                            }
                        }
#endif
                    }

                    catch
                    {
                       
                    }
                }
                
            }


            return propertyCollection;
        }

#endif

#if SILVERLIGHT
        public PropertyCollection GroupAndSort(PropertyItemCollection propertyCollection)
        {
            if (propertyCollection !=null && this.EnableGrouping)
            {
                if (propertyCollection.Count > 0)
                {
                    foreach (CategoryEditor item in this.CategoryEditors)
                    {
                        PropertyItem valueEditorItem = new PropertyItem() { PropertyGrid = this };
                        foreach (CategoryEditorProperty property in item.Properties)
                        {
                            PropertyItem pitem = new PropertyItem();
                            pitem = propertyCollection[property.Name];

                            if (pitem != null)
                            {
                                valueEditorItem.Template = item.EditorTemplate;
                                valueEditorItem.IsCategoryEditorEnabled = true;
                                valueEditorItem.SelectedObject = pitem.SelectedObject;
                                valueEditorItem.PropertyInformation = pitem.PropertyInformation;
                                if (!string.IsNullOrEmpty(item.Category))
                                    valueEditorItem._category = item.Category;
                                else
                                    valueEditorItem._category = "Other";

                                pitem.CategoryValueProperties.Add(pitem);
                                if (pitem != null)
                                    propertyCollection.Remove(pitem);
                            }
                        }
                        propertyCollection.Add(valueEditorItem);
                    }

                    PropertyCollection _propertyCollection = new PropertyCollection();
                    PagedCollection = new PagedCollectionView(propertyCollection);
                    PagedCollection.GroupDescriptions.Add(new PropertyGroupDescription("Category"));
                    if (PART_SearchText != null && !String.IsNullOrEmpty(PART_SearchText.Value))
                    {
                        PagedCollection.Filter = Filter;
                    }
                    PagedCollection.SortDescriptions.Add(new SortDescription("Name", SortDirection));

                    _propertyCollection = new PropertyCollection();
                    PagedCollectionView sortedgroups = new PagedCollectionView(PagedCollection.Groups);
                    sortedgroups.SortDescriptions.Add(new SortDescription("Name", SortDirection));
                    for (int i = 0; i < sortedgroups.Count; i++)
                    {
                        try
                        {
                            System.Windows.Data.CollectionViewGroup c = ((System.Windows.Data.CollectionViewGroup)sortedgroups[i]);
                            System.Collections.ObjectModel.ReadOnlyCollection<object> co = ((System.Collections.ObjectModel.ReadOnlyCollection<object>)(c.Items));
                            PropertyCategoryViewItemCollection _propertyCategoryViewItemCollection = new PropertyCategoryViewItemCollection();
                            _propertyCategoryViewItemCollection.Category = c.Name.ToString();

                            foreach (var item in c.Items)
                            {
                                PropertyItem _propertyItem = ((PropertyItem)(item));
                                _propertyCategoryViewItemCollection.Properties.Add(_propertyItem);
                            }
                            _propertyCollection.Add(_propertyCategoryViewItemCollection);
                        }
                        catch
                        {

                        }
                    }
                    
                    return _propertyCollection;
                }
                return new PropertyCollection();
            }
            else
            {
                if (propertyCollection != null)
                {
                    PropertyCollection _propertyCollection = new PropertyCollection();
                    PagedCollection = new PagedCollectionView(propertyCollection);
                    PagedCollection.SortDescriptions.Add(new SortDescription("Name", SortDirection));

                    int temp_i = 0;
                    foreach (var item in PagedCollection)
                    {
                        _propertyCollection.Add((PropertyItem)PagedCollection[temp_i]);
                        temp_i++;
                    }
                    return _propertyCollection;
                }
                return new PropertyCollection();
            }
        }
#endif
       
        /// <summary>
        /// Fires the value changed.
        /// </summary>
        /// <param name="propertyItem">The property item.</param>
        internal void FireValueChanged(PropertyItem propertyItem,DependencyPropertyChangedEventArgs args)
        {
            if (this.ValueChanged != null)
            {
                Property property = new Property()
                {
                    _browsable = propertyItem.Browsable,
                    _category = propertyItem.Category,
                    _DisplayName = propertyItem.DisplayName,
                    _isReadOnly = propertyItem.IsReadOnly,
                    _Value = propertyItem.Value,
                    PropertyInformation = propertyItem.PropertyInformation,
                    SelectedObject = propertyItem.SelectedObject,
                };
                this.ValueChanged(this, new ValueChangedEventArgs(property,args.NewValue,args.OldValue));
            }
        }
       
        #endregion

        #region PropertyChangedCallbacks


        private static void OnDescriptionPanelHeightChanged(DependencyObject obj, DependencyPropertyChangedEventArgs args)
        {
            if (obj is PropertyGrid)
            {
                var propertyGrid = obj as PropertyGrid;
#if WPF
                if (propertyGrid.DescriptionPanelVisibility == Visibility.Collapsed)
                    propertyGrid.DescriptionPanelHeight = GridLength.Auto;
#endif
            }
        }


        private static void OnEnableToolTipChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            PropertyGrid propertyGrid = (PropertyGrid)d;
            propertyGrid.OnEnableToolTipChanged(e);
        }

        private void OnEnableToolTipChanged(DependencyPropertyChangedEventArgs e)
        {
            if (e.NewValue != e.OldValue)
            {
                this.RefreshPropertygrid();
            }
        }

        private static void OnSelectedPropertyItemChanged(DependencyObject obj, DependencyPropertyChangedEventArgs args)
        {
            PropertyGrid propertyGrid = (PropertyGrid)obj;
            propertyGrid.OnSelectedPropertyItemChanged(args);
        }

        /// <summary>
        /// Raises the <see cref="E:SelectedPropertyItemChanged"/> event.
        /// </summary>
        /// <param name="args">The <see cref="System.Windows.DependencyPropertyChangedEventArgs"/> instance containing the event data.</param>
        protected void OnSelectedPropertyItemChanged(DependencyPropertyChangedEventArgs args)
        {
            if (this.SelectedPropertyItemChanged != null)
            {
                this.SelectedPropertyItemChanged(this, args);
            }
        }

        private static void OnSelectedItemsChanged(DependencyObject sender, DependencyPropertyChangedEventArgs args)
        {
            PropertyGrid control = sender as PropertyGrid;
            if (control != null)
            {
                PropertyCollection collection1 = control.ParseObject(control.SelectedItems.Cast<FrameworkElement>().ToList()[0]);
                PropertyCollection collection2 = control.ParseObject(control.SelectedItems.Cast<FrameworkElement>().ToList()[1]);

            }
        }


        /// <summary>
        /// Called when [expander style changed].
        /// </summary>
        /// <param name="obj">The obj.</param>
        /// <param name="args">The <see cref="System.Windows.DependencyPropertyChangedEventArgs"/> instance containing the event data.</param>
        private static void OnExpanderStyleChanged(DependencyObject obj, DependencyPropertyChangedEventArgs args)
        {
        }

        private static void OnPropertyExpandModeChanged(DependencyObject obj, DependencyPropertyChangedEventArgs args)
        {
#if WPF
            PropertyGrid propertyGrid = (PropertyGrid)obj;
            propertyGrid.OnSelectedObjectChanged(args);
#endif
        }

        /// <summary>
        /// Called when [selected object changed].
        /// </summary>
        /// <param name="obj">The obj.</param>
        /// <param name="args">The <see cref="System.Windows.DependencyPropertyChangedEventArgs"/> instance containing the event data.</param>
        private static void OnSelectedObjectChanged(DependencyObject obj, DependencyPropertyChangedEventArgs args)
        {
            PropertyGrid propertyGrid = (PropertyGrid)obj;
            propertyGrid.OnSelectedObjectChanged(args);
        }
#if WPF
        /// <summary>
        /// Raises the <see cref="E:VisualStyleChanged"/> event.
        /// </summary>
        /// <param name="args">The <see cref="System.Windows.DependencyPropertyChangedEventArgs"/> instance containing the event data.</param>
        private static void OnVisualStyleChanged(DependencyObject obj, DependencyPropertyChangedEventArgs args)
        {

            SkinStorage.IsThemeChangeNotNeeded = true;
            string theme = args.NewValue.ToString();
            var rdict1 = new ResourceDictionary();
            if (theme != "Default")
            {
                try
                {
                    rdict1.Source = new Uri(@"/Syncfusion.PropertyGrid.WPF;component/Themes/" + theme + "Style.xaml", UriKind.RelativeOrAbsolute);
                    (obj as FrameworkElement).Resources.MergedDictionaries.Add(rdict1);
                }
                catch { }

            }
            else
            {
                rdict1.Source = new Uri(@"/Syncfusion.PropertyGrid.WPF;component/Themes/Generic.xaml", UriKind.RelativeOrAbsolute);
                (obj as FrameworkElement).Resources.MergedDictionaries.Add(rdict1);
            }
        }
#endif

        private PropertyCollection propertyCollectionTemp = null;

        private DispatcherTimer timer = new DispatcherTimer();

        private int count = 0;
        /// <summary>
        /// Raises the <see cref="E:SelectedObjectChanged"/> event.
        /// </summary>
        /// <param name="args">The <see cref="System.Windows.DependencyPropertyChangedEventArgs"/> instance containing the event data.</param>
        protected void OnSelectedObjectChanged(DependencyPropertyChangedEventArgs args)
        {
#if WPF
                SkinStorage.IsThemeChangeNotNeeded = true;
#endif
              if (propertyCollection!=null && this.propertyCollection.Count > 0)
                {
                    this.propertyCollection.Clear();
                }
                if (this.SelectedPropertyItem != null)
                {
                    this.SelectedPropertyItem = null;
                }
                if (this.Properties!=null && this.Properties.Count > 0)
                {
//#if WPF
//                    SkinStorage.IsThemeChangeNotNeeded = true;
//#endif
                    this.Properties.Clear();
//#if WPF
//                    SkinStorage.IsThemeChangeNotNeeded = false;
//#endif
                }
            if (args.OldValue != null && args.NewValue != null)
            {
                if (args.OldValue.GetType() == args.NewValue.GetType())
                {
                    isOldType = true;
                    if (this.SelectedObjectProperties != null)
                    {
                        foreach (PropertyItem item in this.SelectedObjectProperties)
                        {
                            item.SelectedObject = SelectedObject;
                        }

                        if (this.SelectedObjectProperties.Count == 0 && this.Properties.Count == 0)
                        {
                            UpdatePropertyCollection();
                        }
                    }
                }
                else
                {
                    isOldType = false;
//#if WPF
//                    SkinStorage.IsThemeChangeNotNeeded = true;
//#endif
                   UpdatePropertyCollection();
//#if WPF
//                    SkinStorage.IsThemeChangeNotNeeded = false;
//#endif
                }
            }
            else
            {
                isOldType = false;
                UpdatePropertyCollection();
//#if WPF
//                SkinStorage.IsThemeChangeNotNeeded = true;
//#endif

//#if WPF
//                SkinStorage.IsThemeChangeNotNeeded = false;
//#endif
            }
            if (view != null)
                this.FilterPropertyGrid();
            if (this.SelectedObjectChanged != null)
            {
                this.SelectedObjectChanged(this, args);
            }
#if WPF
           SkinStorage.IsThemeChangeNotNeeded = false;
#endif

        }

        private void UpdatePropertyCollection()
        {
            count = 0;
            timer.Stop();
            propertyCollectionTemp = ParseObject(SelectedObject);
            if (!this.DisableAnimationOnObjectSelection)
            {
                timer.Start();
            }
            else
            {
                this.Properties = propertyCollectionTemp;
            }
          
        }

        void timer_Tick(object sender, EventArgs e)
        {
            if (propertyCollectionTemp.Count > 0)
            {
                this.Properties.Add(propertyCollectionTemp[count]);
                count++;
            }
            if(count== propertyCollectionTemp.Count)
                timer.Stop();
        }

        /// <summary>
        /// Called when [enable grouping changed].
        /// </summary>
        /// <param name="obj">The obj.</param>
        /// <param name="args">The <see cref="System.Windows.DependencyPropertyChangedEventArgs"/> instance containing the event data.</param>
        private static void OnEnableGroupingChanged(DependencyObject obj, DependencyPropertyChangedEventArgs args)
        {
            PropertyGrid propertyGrid = (PropertyGrid)obj;
            propertyGrid.OnEnableGroupingChanged(args);
        }

        /// <summary>
        /// Raises the <see cref="E:EnableGroupingChanged"/> event.
        /// </summary>
        /// <param name="args">The <see cref="System.Windows.DependencyPropertyChangedEventArgs"/> instance containing the event data.</param>
        protected void OnEnableGroupingChanged(DependencyPropertyChangedEventArgs args)
        {
            if (this.EnableGroupingChanged != null)
                this.EnableGroupingChanged(this, args);

            if (this.PART_PropertyView != null)
            {
                this.PART_PropertyView.EnableGrouping = this.EnableGrouping;
            }

            this.Properties = ParseObject(this.SelectedObject);

            if (PART_SearchText != null && !String.IsNullOrEmpty(PART_SearchText.Value))
            {
                if (EnableGrouping)
                {
                    Binding binding = new Binding("Properties");
                    binding.Source = this;
                    binding.Mode = BindingMode.TwoWay;
                    this.PART_PropertyView.SetBinding(ItemsControl.ItemsSourceProperty, binding);
                }
                else
                {
                    FilterPropertyGrid();
                }
            }
            //this.Properties = this.GroupAndSort(this.SelectedObjectProperties);

        }

        /// <summary>
        /// Called when [sort direction changed].
        /// </summary>
        /// <param name="obj">The obj.</param>
        /// <param name="args">The <see cref="System.Windows.DependencyPropertyChangedEventArgs"/> instance containing the event data.</param>
        private static void OnSortDirectionChanged(DependencyObject obj, DependencyPropertyChangedEventArgs args)
        {
            PropertyGrid propertyGrid = (PropertyGrid)obj;
            if (propertyGrid != null)
            {
                propertyGrid.OnSortDirectionChanged(args);
            }
        }

        /// <summary>
        /// Raises the <see cref="E:SortDirectionChanged"/> event.
        /// </summary>
        /// <param name="args">The <see cref="System.Windows.DependencyPropertyChangedEventArgs"/> instance containing the event data.</param>
        protected void OnSortDirectionChanged(DependencyPropertyChangedEventArgs args)
        {
            if (this.SortDirectionChanged != null)
                this.SortDirectionChanged(this, args);

#if SILVERLIGHT
            this.Properties = ParseObject(this.SelectedObject);
#endif
#if WPF
            this.Properties = this.GroupAndSort(this.SelectedObjectProperties);
#endif

        }

        /// <summary>
        /// Called when [category foreground changed].
        /// </summary>
        /// <param name="obj">The obj.</param>
        /// <param name="args">The <see cref="System.Windows.DependencyPropertyChangedEventArgs"/> instance containing the event data.</param>
        private static void OnCategoryForegroundChanged(DependencyObject obj, DependencyPropertyChangedEventArgs args)
        {

        }

        /// <summary>
        /// Called when [line color changed].
        /// </summary>
        /// <param name="obj">The obj.</param>
        /// <param name="args">The <see cref="System.Windows.DependencyPropertyChangedEventArgs"/> instance containing the event data.</param>
        private static void OnLineColorChanged(DependencyObject obj, DependencyPropertyChangedEventArgs args)
        {

        }

        /// <summary>
        /// Called when [view background color changed].
        /// </summary>
        /// <param name="obj">The obj.</param>
        /// <param name="args">The <see cref="System.Windows.DependencyPropertyChangedEventArgs"/> instance containing the event data.</param>
        private static void OnViewBackgroundColorChanged(DependencyObject obj, DependencyPropertyChangedEventArgs args)
        {

        }

        /// <summary>
        /// Called when [view foreground color changed].
        /// </summary>
        /// <param name="obj">The obj.</param>
        /// <param name="args">The <see cref="System.Windows.DependencyPropertyChangedEventArgs"/> instance containing the event data.</param>
        private static void OnViewForegroundColorChanged(DependencyObject obj, DependencyPropertyChangedEventArgs args)
        {

        }
        #endregion

        #region Overrides
        public ToggleButton PART_GroupButton;
        public ToggleButton PART_SortButton;
        PropertyView PART_PropertyView;
        MaskedTextBox PART_SearchText;
        Button PART_Clear;        
        /// <summary>
        /// When overridden in a derived class, is invoked whenever application code or internal processes call <see cref="M:System.Windows.FrameworkElement.ApplyTemplate"/>.
        /// </summary>
        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();
            if (PART_SortButton != null)
            {
                PART_SortButton.Click -= new RoutedEventHandler(PART_SortButton_Click);
            }
            if (PART_GroupButton != null)
            {
                PART_GroupButton.Click -= new RoutedEventHandler(PART_GroupButton_Click);
            }
            if (PART_SearchText != null)
            {
                PART_SearchText.ValueChanged -= new PropertyChangedCallback(PART_SearchText_ValueChanged);
                PART_SearchText.KeyDown -= new KeyEventHandler(PART_SearchText_KeyDown);
            }
            if (this.PART_Clear != null)
            {
                PART_Clear.Click -= new RoutedEventHandler(PART_Clear_Click);
            }
            this.PART_GroupButton = this.GetTemplateChild("PART_GroupButton") as ToggleButton;
            this.PART_SortButton = this.GetTemplateChild("PART_SortButton") as ToggleButton;
            this.PART_PropertyView = this.GetTemplateChild("PART_PropertyView") as PropertyView;
            this.PART_SearchText = this.GetTemplateChild("PART_SearchText") as MaskedTextBox;
            this.PART_Clear = this.GetTemplateChild("PART_Clear") as Button;

            if (this.PART_PropertyView != null)
            {
                this.PART_PropertyView.PropertyGrid = this;
            }
            if (PART_SortButton != null)
            {
                PART_SortButton.Click += new RoutedEventHandler(PART_SortButton_Click);
            }
            if (PART_GroupButton != null)
            {
                PART_GroupButton.Click += new RoutedEventHandler(PART_GroupButton_Click);
            }
            if (PART_SearchText != null)
            {
                PART_SearchText.ValueChanged += new PropertyChangedCallback(PART_SearchText_ValueChanged);
                PART_SearchText.KeyDown += new KeyEventHandler(PART_SearchText_KeyDown);
            }
            if (this.PART_Clear != null)
            {
                PART_Clear.Click += new RoutedEventHandler(PART_Clear_Click);
            }
            view = new CollectionViewSource();


            this.Unloaded -= new RoutedEventHandler(PropertyGrid_Unloaded);
            this.Unloaded += new RoutedEventHandler(PropertyGrid_Unloaded);
        }

        void PropertyGrid_Loaded(object sender, RoutedEventArgs e)
        {
            if (CategoryEditors == null)
            {
                CategoryEditors = new CategoryEditorCollection();
            }
           else
            {
                if (EnableGrouping && CategoryEditors.Count> 0)
                {
                    this.UpdatePropertyCollection();
                }
            }
            if (this.PART_PropertyView == null)
            {
                this.PART_PropertyView = this.GetTemplateChild("PART_PropertyView") as PropertyView;
            }
            if (CustomEditorCollection == null)
            {
                CustomEditorCollection = new CustomEditorCollection();
            }
            if (HidePropertiesCollection == null)
            {
                HidePropertiesCollection = new ObservableCollection<string>();
            }
            if (SelectedObjectProperties == null)
                SelectedObjectProperties = new PropertyItemCollection();
            //if (Properties == null)
            //{
            //    Properties = new PropertyCollection();
            //    RefreshPropertygrid();
            //}
            if (CategoryEditors != null)
            {
                CategoryEditors.CollectionChanged -= new System.Collections.Specialized.NotifyCollectionChangedEventHandler(CategoryEditors_CollectionChanged);
                CategoryEditors.CollectionChanged += new System.Collections.Specialized.NotifyCollectionChangedEventHandler(CategoryEditors_CollectionChanged);
            }
            if (PART_SortButton != null)
            {
                PART_SortButton.Click -= new RoutedEventHandler(PART_SortButton_Click);
                PART_SortButton.Click += new RoutedEventHandler(PART_SortButton_Click);
            }
            if (PART_GroupButton != null)
            {
                PART_GroupButton.Click -= new RoutedEventHandler(PART_GroupButton_Click);
                PART_GroupButton.Click += new RoutedEventHandler(PART_GroupButton_Click);
            }
            if (PART_SearchText != null)
            {
                PART_SearchText.ValueChanged -= new PropertyChangedCallback(PART_SearchText_ValueChanged);
                PART_SearchText.KeyDown -= new KeyEventHandler(PART_SearchText_KeyDown);
                PART_SearchText.ValueChanged += new PropertyChangedCallback(PART_SearchText_ValueChanged);
                PART_SearchText.KeyDown += new KeyEventHandler(PART_SearchText_KeyDown);

            }
            if (this.PART_Clear != null)
            {
                PART_Clear.Click -= new RoutedEventHandler(PART_Clear_Click);
                PART_Clear.Click += new RoutedEventHandler(PART_Clear_Click);
            }

        }

        void PropertyGrid_Unloaded(object sender, RoutedEventArgs e)
        {
#if WPF
            SkinStorage.IsThemeChangeNotNeeded = false;
#endif
            if (SelectedObjectProperties != null)
            {
                SelectedObjectProperties.Clear();
                SelectedObjectProperties = null;
            }
                   
            if (propertyCollection != null)
            {
                 propertyCollection = null;
            }
            if (CatagoryCollection != null)
            {
                CatagoryCollection.Clear();
                CatagoryCollection = null;
            }
            if (CategoryEditors != null)
            {
                CategoryEditors.CollectionChanged -= new System.Collections.Specialized.NotifyCollectionChangedEventHandler(CategoryEditors_CollectionChanged);
                CategoryEditors = null;
            }
            if (PART_SortButton != null)
            {
                PART_SortButton.Click -= new RoutedEventHandler(PART_SortButton_Click);
            }
            if (PART_GroupButton != null)
            {
                PART_GroupButton.Click -= new RoutedEventHandler(PART_GroupButton_Click);
            }
            if (PART_SearchText != null)
            {
                PART_SearchText.ValueChanged -= new PropertyChangedCallback(PART_SearchText_ValueChanged);
                PART_SearchText.KeyDown -= new KeyEventHandler(PART_SearchText_KeyDown);
            }
            if (this.PART_Clear != null)
            {
                PART_Clear.Click -= new RoutedEventHandler(PART_Clear_Click);
            }

#if SILVERLIGHT
            if (PagedCollection != null)
            {
                PagedCollection = null;
            }
#endif
            if (PART_PropertyView != null)
            {
                PART_PropertyView = null;
            }

            if (view != null)
                view.Filter -= new FilterEventHandler(view_Filter);
            if (src != null)
                src.Filter -= new FilterEventHandler(view_Filter);
            this.src = null;
            this.view = null;
            this.Unloaded -= new RoutedEventHandler(PropertyGrid_Unloaded);
        }

        void PART_SearchText_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Escape)
            {
#if SILVERLIGHT
                PART_SearchText.ClearValue(MaskedTextBox.ValueProperty);                
#endif
#if WPF
                PART_SearchText.Clear();
#endif
            }
        }
        
        void PART_Clear_Click(object sender, RoutedEventArgs e)
        {
#if SILVERLIGHT            
            PART_SearchText.ClearValue(MaskedTextBox.ValueProperty);
#endif
#if WPF
            PART_SearchText.Clear();
#endif
        }

        CollectionViewSource view;
        CollectionViewSource src;

        void PART_SearchText_ValueChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (String.IsNullOrEmpty(PART_SearchText.Value))
            {
                if (!EnableGrouping)
                {
                    Binding binding = new Binding("Properties");
                    binding.Source = this;
                    //binding.Mode = BindingMode.TwoWay;
                    isCalledFromFilterPropertyGrid = true;
                    if (this.PART_PropertyView != null)
                        this.PART_PropertyView.SetBinding(ItemsControl.ItemsSourceProperty, binding);
                    isCalledFromFilterPropertyGrid = false;
                }
                else
                {
                    this.Properties = ParseObject(SelectedObject);
                }
            }
            else
            {
                if (!EnableGrouping)
                {
                    FilterPropertyGrid();
                }
                else
                {

                    this.Properties = ParseObject(SelectedObject);
                }
            }
        }

        private void FilterPropertyGrid()
        {
            if (PART_SearchText != null && !String.IsNullOrEmpty(PART_SearchText.Value))
            {
                if (view != null)
                {
                    view.Source = Properties;
                    isCalledFromFilterPropertyGrid = true;
                    view.Filter += new FilterEventHandler(view_Filter);
                    if (this.PART_PropertyView != null)
                    {
                        this.PART_PropertyView.ItemsSource = null;
                        this.PART_PropertyView.ItemsSource = view.View;
                    }
                    isCalledFromFilterPropertyGrid = false;
                }
            }
        }

        private bool Filter(object item)
        {
            PropertyItem viewItem = item as PropertyItem;
            if (viewItem != null)
            {
                if (String.IsNullOrEmpty(PART_SearchText.Value) || viewItem.DisplayName.ToLower().Contains(PART_SearchText.Value.ToLower()))
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }
            return true;
        }

        void view_Filter(object sender, FilterEventArgs e)
        {
            PropertyItem viewItem = e.Item as PropertyItem;
            if (viewItem != null)
            {
                if (String.IsNullOrEmpty(PART_SearchText.Value) || viewItem.DisplayName.ToLower().Contains(PART_SearchText.Value.ToLower()))
                {
                    e.Accepted = true;
                }
                else
                {
                    e.Accepted = false;
                }
            }
            
        }

#if SILVERLIGHT
        void PART_GroupButton_Click(object sender, RoutedEventArgs e)
        {
            
        }

        void PART_SortButton_Click(object sender, RoutedEventArgs e)
        {
            
        }
#endif

#if WPF
        void PART_SortButton_Click(object sender, RoutedEventArgs e)
        {
            PART_SortButton.IsChecked = true;
            e.Handled = true;
        }

        void PART_GroupButton_Click(object sender, RoutedEventArgs e)
        {
            PART_GroupButton.IsChecked = true;
            PART_GroupButton.IsTabStop = false;
            e.Handled = true;
        }
#endif

        #endregion
#if SILVERLIGHT
        public void OnStyleChanged(Controls.Theming.VisualStyle visualStyle)
        {
           this.Properties=this.ParseObject(SelectedObject);
        }
#endif
    }
    public enum PropertyExpandModes
    {
#if WPF
        NestedMode = 0,
#endif
        FlatMode = 1
    }
}
