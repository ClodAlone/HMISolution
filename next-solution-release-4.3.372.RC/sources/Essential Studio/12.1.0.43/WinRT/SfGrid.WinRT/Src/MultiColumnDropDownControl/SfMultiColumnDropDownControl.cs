#region Copyright Syncfusion Inc. 2001 - 2014
// Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Any infringement will be prosecuted under
// applicable laws. 
#endregion
#if WinRT
using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using Syncfusion.Data;
using Syncfusion.Data.Extensions;
using Windows.Devices.Input;
using Windows.Foundation;
using Windows.UI;
using Windows.UI.Core;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Syncfusion.Dynamic;
#else
using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Input;
using System.Windows.Media;
using Syncfusion.Data;
using Syncfusion.UI.Xaml.ScrollAxis;

#endif

namespace Syncfusion.UI.Xaml.Grid
{
#if WinRT
    using Key = Windows.System.VirtualKey;
    using KeyEventArgs = KeyRoutedEventArgs;
    using MouseButtonEventArgs = PointerRoutedEventArgs;
#endif
    #region Event Arguments and Handlers

    #region PopupOpening EventHandler
    public delegate void PopupOpeningEventHandler(object sender, PopupOpeningEventArgs args);

    public class PopupOpeningEventArgs : CancelEventArgs
    {

    }
    #endregion

    #region PopupOpened EventHandler
    public delegate void PopupopenedEventHandler(object sender, PopupopenedEventArgs args);

    public class PopupopenedEventArgs : EventArgs
    {

    }
    #endregion

    #region PopupClosing EventHandler
    public delegate void PopupClosingEventHandler(object sender, PopupClosingEventArgs args);

    public class PopupClosingEventArgs : CancelEventArgs
    {

    }
    #endregion

    #region PopupClosed EventHandler
    public delegate void PopupClosedEventHandler(object sender, PopupClosedEventArgs args);

    public class PopupClosedEventArgs : EventArgs
    {

    }
    #endregion

    #region SelectionChanged EventHandler

    public delegate void SelectionChangedEventHandler(object sender, SelectionChangedEventArgs args);

    public class SelectionChangedEventArgs : EventArgs
    {
        public int SelectedIndex { get; set; }

        public object SelectedItem { get; set; }
    }

    #endregion
    #endregion

    [TemplatePart(Name = "PART_RootBorder", Type = typeof(Border))]
    [TemplatePart(Name = "PART_TextBox", Type = typeof(TextBox))]
    [TemplatePart(Name = "PART_ToggleButton", Type = typeof(ToggleButton))]
    [TemplatePart(Name = "PART_Popup", Type = typeof(Popup))]
    [TemplatePart(Name = "PART_PopupBorder", Type = typeof(Border))]
    [TemplatePart(Name = "PART_SfDataGrid", Type = typeof(SfDataGrid))]
    [TemplatePart(Name = "PART_ThumbGripper", Type = typeof(Thumb))]
    public class SfMultiColumnDropDownControl : Control, IDisposable
    {
        #region Private Fields
#if SILVERLIGHT
        private static SolidColorBrush PopUpBackGround=new SolidColorBrush(Colors.LightGray);
        private static SolidColorBrush PopUpSfDataGridBackGround=new SolidColorBrush(Colors.White);

#else
        private static readonly SolidColorBrush PopUpBackGround = new SolidColorBrush(Colors.Gainsboro);
        private static readonly SolidColorBrush PopUpSfDataGridBackGround = new SolidColorBrush(Colors.White);
#endif

        private IEnumerable<object> appendSource;
        private static double DefaultPopupMinHeight = 300.0;
        private static double DefaultPopupMinWidth = 200.0;
        private bool isSuspendUpdate;
        private bool isTextChanged;
        private bool isSelectedIndexLoadedBeforeGridLoaded;
        private bool suspendSelectedValueChangedBeforeGridLoaded;
        private bool isSelectedValueLoadedBeforeGridLoaded;
        private bool allowFilter;
        private string filterText = string.Empty;
        #endregion

        #region Internal Fields

        internal Border MainBorder;
        internal TextBox Editor;
        internal ToggleButton DropDownButton;
        internal Popup InternalPopup;
        internal Border InternalPopupBorder;
        internal SfDataGrid InternalGrid;
        internal Thumb ResizeThumb;

        internal double actualPopupHeight;
        internal double actualPopupWidth;
        internal IPropertyAccessProvider dataHelper = null;
        internal object PreviousSelectedItem;
        #endregion

        #region Dependency Properties

        #region SfMultiColumnProperties

        /// <summary>
        /// Gets or sets a value indicating whether AutoComplete is enabled or not.
        /// </summary>
        /// <value><see langword="true"/> if ; otherwise, <see langword="false"/>.</value>
        /// <remarks></remarks>
        public bool AllowAutoComplete
        {
            get { return (bool)GetValue(AllowAutoCompleteProperty); }
            set { SetValue(AllowAutoCompleteProperty, value); }
        }

        /// <summary>
        /// Dependency registration for AllowAutoComplete
        /// </summary>
        /// <remarks></remarks>
        public static readonly DependencyProperty AllowAutoCompleteProperty =
            DependencyProperty.Register("AllowAutoComplete", typeof(bool), typeof(SfMultiColumnDropDownControl), new PropertyMetadata(true));

        /// <summary>
        /// Gets or sets a value indicating whether NULL Input is enabled or not.
        /// </summary>
        /// <value><see langword="true"/> if ; otherwise, <see langword="false"/>.</value>
        /// <remarks></remarks>
        public bool AllowNullInput
        {
            get { return (bool)GetValue(AllowNullInputProperty); }
            set { SetValue(AllowNullInputProperty, value); }
        }

        /// <summary>
        /// Dependency registration for AllowAutoComplete
        /// </summary>
        /// <remarks></remarks>
        public static readonly DependencyProperty AllowNullInputProperty =
            DependencyProperty.Register("AllowNullInput", typeof(bool), typeof(SfMultiColumnDropDownControl), new PropertyMetadata(false));

        /// <summary>
        /// Gets the actual height of the popup.
        /// </summary>
        /// <value>
        /// The actual height of the popup.
        /// </value>
        public double ActualPopupHeight
        {
            get { return (double)GetValue(ActualPopupHeightProperty); }
            internal set { SetValue(ActualPopupHeightProperty, null); }
        }

        /// <summary>
        /// The actual popup height property
        /// </summary>
        public static readonly DependencyProperty ActualPopupHeightProperty =
            DependencyProperty.Register("ActualPopupHeight", typeof(double), typeof(SfMultiColumnDropDownControl), new PropertyMetadata(double.NaN));

        /// <summary>
        /// Gets the actual width of the popup.
        /// </summary>
        /// <value>
        /// The actual width of the popup.
        /// </value>
        public double ActualPopupWidth
        {
            get { return (double)GetValue(ActualPopupWidthProperty); }
            internal set { SetValue(ActualPopupWidthProperty, null); }
        }

        /// <summary>
        /// The actual popup width property
        /// </summary>
        public static readonly DependencyProperty ActualPopupWidthProperty =
            DependencyProperty.Register("ActualPopupWidth", typeof(double), typeof(SfMultiColumnDropDownControl), new PropertyMetadata(double.NaN));

        /// <summary>
        /// Gets or sets a value indicating whether [allow immediate popup].
        /// </summary>
        /// <value>
        ///   <c>true</c> if [allow immediate popup]; otherwise, <c>false</c>.
        /// </value>
        public bool AllowImmediatePopup
        {
            get { return (bool)GetValue(AllowImmediatePopupProperty); }
            set { SetValue(AllowImmediatePopupProperty, value); }
        }

        /// <summary>
        /// The allow immediate popup property
        /// </summary>
        public static readonly DependencyProperty AllowImmediatePopupProperty =
            DependencyProperty.Register("AllowImmediatePopup", typeof(bool), typeof(SfMultiColumnDropDownControl), new PropertyMetadata(false));


        /// <summary>
        /// Gets or sets a value indicating whether this instance is auto popup size.
        /// </summary>
        /// <value>
        /// <c>true</c> if this instance is auto popup size; otherwise, <c>false</c>.
        /// </value>
        public bool IsAutoPopupSize
        {
            get { return (bool)GetValue(IsAutoPopupSizeProperty); }
            set { SetValue(IsAutoPopupSizeProperty, value); }
        }

        /// <summary>
        /// The is auto popup size property
        /// </summary>
        public static readonly DependencyProperty IsAutoPopupSizeProperty =
            DependencyProperty.Register("IsAutoPopupSize", typeof(bool), typeof(SfMultiColumnDropDownControl), new PropertyMetadata(false));

        /// <summary>
        /// Gets the search text for the MultiColumnDropDownControl.
        /// </summary>
        /// <value>
        /// The search text.
        /// </value>
        public string SearchText
        {
            get { return (string)GetValue(SearchTextProperty); }
            private set { SetValue(SearchTextProperty, value); }
        }

        /// <summary>
        /// The search text property
        /// </summary>
        public static readonly DependencyProperty SearchTextProperty =
            DependencyProperty.Register("SearchText", typeof(string), typeof(SfMultiColumnDropDownControl), new PropertyMetadata(string.Empty));

        /// <summary>
        /// Gets the filtered items from the DropDownGrid.
        /// </summary>
        /// <value>
        /// The filtered items.
        /// </value>
        public IEnumerable FilteredItems
        {
            get { return (IEnumerable)GetValue(FilteredItemsProperty); }
            private set { SetValue(FilteredItemsProperty, value); }
        }

        public static readonly DependencyProperty FilteredItemsProperty =
            DependencyProperty.Register("FilteredItems", typeof(IEnumerable), typeof(SfMultiColumnDropDownControl), new PropertyMetadata(null));

        /// <summary>
        /// Gets or sets Template for PopupContent.
        /// </summary>
        /// <value></value>
        /// <remarks></remarks>
        public ControlTemplate PopupContentTemplate
        {
            get { return (ControlTemplate)GetValue(PopupContentTemplateProperty); }
            set { SetValue(PopupContentTemplateProperty, value); }
        }

        /// <summary>
        /// Dependency registration for PopupContentTemplate
        /// </summary>
        /// <remarks></remarks>
        public static DependencyProperty PopupContentTemplateProperty =
            DependencyProperty.Register("PopupContentTemplate", typeof(ControlTemplate), typeof(SfMultiColumnDropDownControl), new PropertyMetadata(null));

        /// <summary>
        /// Gets or sets a value indicating whether Incremental Filtering is enabled or not.
        /// </summary>
        /// <value><see langword="true"/> if ; otherwise, <see langword="false"/>.</value>
        /// <remarks></remarks>
        public bool AllowIncrementalFiltering
        {
            get { return (bool)GetValue(AllowIncrementalFilteringProperty); }
            set { SetValue(AllowIncrementalFilteringProperty, value); }
        }

        /// <summary>
        /// Dependency registration for AllowIncrementalFiltering
        /// </summary>
        /// <remarks></remarks>
        public static readonly DependencyProperty AllowIncrementalFilteringProperty =
            DependencyProperty.Register("AllowIncrementalFiltering", typeof(bool), typeof(SfMultiColumnDropDownControl), new PropertyMetadata(true));

        /// <summary>
        /// Gets or sets a value indicating whether Filtering is applied with Text Case.
        /// </summary>
        /// <value><see langword="true"/> if ; otherwise, <see langword="false"/>.</value>
        /// <remarks></remarks>
        public bool AllowCaseSensitiveFiltering
        {
            get { return (bool)GetValue(AllowCaseSensitiveFilteringProperty); }
            set { SetValue(AllowCaseSensitiveFilteringProperty, value); }
        }

        /// <summary>
        /// Dependency registration for AllowCasingforFilter
        /// </summary>
        /// <remarks></remarks>
        public static readonly DependencyProperty AllowCaseSensitiveFilteringProperty =
            DependencyProperty.Register("AllowCaseSensitiveFiltering", typeof(bool), typeof(SfMultiColumnDropDownControl), new PropertyMetadata(false));

        /// <summary>
        /// Property which is get or set the Grid Columns
        /// </summary>
        /// <value></value>
        /// <remarks></remarks>
        public Columns Columns
        {
            get { return (Columns)GetValue(ColumnsProperty); }
            set { SetValue(ColumnsProperty, value); }
        }

        /// <summary>
        /// Dependency registration of GridColumns property
        /// </summary>
        /// <remarks></remarks>
        public static readonly DependencyProperty ColumnsProperty =
            DependencyProperty.Register("Columns", typeof(Columns), typeof(SfMultiColumnDropDownControl), new PropertyMetadata(new Columns()));

        /// <summary>
        /// Gets or sets a value indicating whether AllowSpinOnMouseWheel is enabled or not.
        /// </summary>
        /// <value><see langword="true"/> if ; otherwise, <see langword="false"/>.</value>
        /// <remarks></remarks>
        public bool AllowSpinOnMouseWheel
        {
            get { return (bool)GetValue(AllowSpinOnMouseWheelProperty); }
            set { SetValue(AllowSpinOnMouseWheelProperty, value); }
        }

        /// <summary>
        /// Dependency registration for AllowSpinOnMouseWheel
        /// </summary>
        /// <remarks></remarks>
        public static readonly DependencyProperty AllowSpinOnMouseWheelProperty =
            DependencyProperty.Register("AllowSpinOnMouseWheel", typeof(bool), typeof(SfMultiColumnDropDownControl), new PropertyMetadata(true));

        /// <summary>
        /// Gets or sets a value indicating whether this instance is text read only.
        /// </summary>
        /// <value>
        /// <c>true</c> if this instance is text read only; otherwise, <c>false</c>.
        /// </value>
        public bool ReadOnly
        {
            get { return (bool)GetValue(ReadOnlyProperty); }
            set { SetValue(ReadOnlyProperty, value); }
        }

        /// <summary>
        /// Dependency registration for IsTextReadOnly
        /// </summary>
        /// <remarks></remarks>
        public static readonly DependencyProperty ReadOnlyProperty =
            DependencyProperty.Register("ReadOnly", typeof(bool), typeof(SfMultiColumnDropDownControl), new PropertyMetadata(false));

        /// <summary>
        /// Gets or sets a value indicating whether the drop-down is open.
        /// </summary>
        /// <value>
        /// 	<c>true</c> if the drop-down is opened; otherwise, <c>false</c>.
        /// </value>
        public bool IsDropDownOpen
        {
            get { return (bool)GetValue(IsDropDownOpenProperty); }
            set { SetValue(IsDropDownOpenProperty, value); }
        }

        /// <summary>
        /// The is drop down open property
        /// </summary>
        public static readonly DependencyProperty IsDropDownOpenProperty =
            DependencyProperty.Register("IsDropDownOpen", typeof(bool), typeof(SfMultiColumnDropDownControl), new PropertyMetadata(false, OnIsDropDownOpenChanged));

        public string Text
        {
            get { return (string)GetValue(TextProperty); }
            set { SetValue(TextProperty, value); }
        }

        public static readonly DependencyProperty TextProperty =
            DependencyProperty.Register("Text", typeof(string), typeof(SfMultiColumnDropDownControl), new PropertyMetadata(string.Empty));

        /// <summary>
        /// Gets or sets the Display Member .
        /// </summary>
        /// <value></value>
        /// <remarks></remarks>
        public string DisplayMember
        {
            get { return (string)GetValue(DisplayMemberProperty); }
            set { SetValue(DisplayMemberProperty, value); }
        }

        /// <summary>
        /// The display member property
        /// </summary>
        public static readonly DependencyProperty DisplayMemberProperty =
            DependencyProperty.Register("DisplayMember", typeof(string), typeof(SfMultiColumnDropDownControl), new PropertyMetadata(string.Empty));

        /// <summary>
        /// Gets or sets the Value Member.
        /// </summary>
        /// <value></value>
        /// <remarks></remarks>
        public string ValueMember
        {
            get { return (string)GetValue(ValueMemberProperty); }
            set { SetValue(ValueMemberProperty, value); }
        }

        /// <summary>
        /// The value member property
        /// </summary>
        public static readonly DependencyProperty ValueMemberProperty =
            DependencyProperty.Register("ValueMember", typeof(string), typeof(SfMultiColumnDropDownControl), new PropertyMetadata(string.Empty));

        /// <summary>
        /// Gets or sets the corner radius.
        /// </summary>
        /// <value>
        /// The corner radius.
        /// </value>
        public Thickness CornerRadius
        {
            get { return (Thickness)GetValue(CornerRadiusProperty); }
            set { SetValue(CornerRadiusProperty, value); }
        }

        /// <summary>
        /// The corner radius property
        /// </summary>
        public static readonly DependencyProperty CornerRadiusProperty =
            DependencyProperty.Register("CornerRadius", typeof(Thickness), typeof(SfMultiColumnDropDownControl), new PropertyMetadata(new Thickness(2)));

        /// <summary>
        /// Gets or sets the popup background.
        /// </summary>
        /// <value>
        /// The popup background.
        /// </value>
        public Brush PopupBackground
        {
            get { return (Brush)GetValue(PopupBackgroundProperty); }
            set { SetValue(PopupBackgroundProperty, value); }
        }

        /// <summary>
        /// The popup background property
        /// </summary>
        public static readonly DependencyProperty PopupBackgroundProperty =
            DependencyProperty.Register("PopupBackground", typeof(Brush), typeof(SfMultiColumnDropDownControl), new PropertyMetadata(PopUpBackGround));

        /// <summary>
        /// Gets or sets the popup drop down grid background.
        /// </summary>
        /// <value>
        /// The popup drop down grid background.
        /// </value>
        public Brush PopupDropDownGridBackground
        {
            get { return (Brush)GetValue(PopupDropDownGridBackgroundProperty); }
            set { SetValue(PopupDropDownGridBackgroundProperty, value); }
        }

        /// <summary>
        /// The popup drop down grid background property
        /// </summary>
        public static readonly DependencyProperty PopupDropDownGridBackgroundProperty =
            DependencyProperty.Register("PopupDropDownGridBackground", typeof(Brush), typeof(SfMultiColumnDropDownControl), new PropertyMetadata(PopUpSfDataGridBackGround));

        /// <summary>
        /// Gets or sets the SelectedIndex property. 
        /// </summary>
        /// <value></value>
        /// <remarks></remarks>
        public int SelectedIndex
        {
            get { return (int)GetValue(SelectedIndexProperty); }
            set { SetValue(SelectedIndexProperty, value); }
        }

        /// <summary>
        /// Dependency registration for SelectedIndex
        /// </summary>
        /// <remarks></remarks>
        public static readonly DependencyProperty SelectedIndexProperty =
            DependencyProperty.Register("SelectedIndex", typeof(int), typeof(SfMultiColumnDropDownControl), new PropertyMetadata(-1, (o, args) =>
                {
                    if ((int)args.NewValue == (int)args.OldValue)
                        return;
                    var internalGrid = ((SfMultiColumnDropDownControl)o).InternalGrid;
                    var multiColumnControl = ((SfMultiColumnDropDownControl)o);
                    if (internalGrid != null)
                        multiColumnControl.ProcessOnSelectedItemChanged(internalGrid.SelectedItem);
                    else
                        multiColumnControl.isSelectedIndexLoadedBeforeGridLoaded = true;
                }));

        /// <summary>
        /// Gets or Set the value for SelectedItem
        /// </summary>
        /// <value></value>
        /// <remarks></remarks>
        public object SelectedItem
        {
            get { return GetValue(SelectedItemProperty); }
            set { SetValue(SelectedItemProperty, value); }
        }

        /// <summary>
        /// Dependency registration for SelectedItem
        /// </summary>
        /// <remarks></remarks>
        public static readonly DependencyProperty SelectedItemProperty =
            DependencyProperty.Register("SelectedItem", typeof(object), typeof(SfMultiColumnDropDownControl), new PropertyMetadata(null, (o, args) => ((SfMultiColumnDropDownControl)o).ProcessOnSelectedItemChanged(args.NewValue)));

        /// <summary>
        /// Gets or sets the Selected Value Member
        /// </summary>
        /// <value></value>
        /// <remarks></remarks>
        public object SelectedValue
        {
            get { return GetValue(SelectedValueProperty); }
            set { SetValue(SelectedValueProperty, value); }
        }

        /// <summary>
        /// The selected value property
        /// </summary>
        public static readonly DependencyProperty SelectedValueProperty =
            DependencyProperty.Register("SelectedValue", typeof(object), typeof(SfMultiColumnDropDownControl), new PropertyMetadata(null, OnSelectedValueChanged));

        #endregion

        #region SfDataGrid Properties

        /// <summary>
        /// Gets or sets a value that indicating whether BoundColumn objects are automatically created and displayed in the DataGrid control for each field in the data source.
        /// </summary>
        /// <value>
        ///   <c>true</c> if [auto generate columns]; otherwise, <c>false</c>.
        /// </value>
        public bool AutoGenerateColumns
        {
            get { return (bool)GetValue(AutoGenerateColumnsProperty); }
            set { SetValue(AutoGenerateColumnsProperty, value); }
        }

        public static readonly DependencyProperty AutoGenerateColumnsProperty =
            DependencyProperty.Register("AutoGenerateColumns", typeof(bool), typeof(SfMultiColumnDropDownControl), new PropertyMetadata(true));

        public AutoGenerateColumnsMode AutoGenerateColumnsMode
        {
            get { return (AutoGenerateColumnsMode)GetValue(AutoGenerateColumnsModeProperty); }
            set { SetValue(AutoGenerateColumnsModeProperty, value); }
        }

        public static readonly DependencyProperty AutoGenerateColumnsModeProperty =
            DependencyProperty.Register("AutoGenerateColumnsMode", typeof(AutoGenerateColumnsMode), typeof(SfMultiColumnDropDownControl), new PropertyMetadata(AutoGenerateColumnsMode.Reset));

        /// <summary>
        /// Gets or sets Width of the GridColumns in the DataGrid according to the Sizer.
        /// </summary>
        /// <value></value>
        /// <remarks></remarks>
        public GridLengthUnitType GridColumnSizer
        {
            get { return (GridLengthUnitType)GetValue(GridColumnSizerProperty); }
            set { SetValue(GridColumnSizerProperty, value); }
        }

        public static readonly DependencyProperty GridColumnSizerProperty =
            DependencyProperty.Register("GridColumnSizer", typeof(GridLengthUnitType), typeof(SfMultiColumnDropDownControl), new PropertyMetadata(GridLengthUnitType.None));

        /// <summary>
        /// Gets or Sets the ItemsSource for the DataGrid in the DropDown Popup
        /// </summary>
        /// <value></value>
        /// <remarks></remarks>
        public object ItemsSource
        {
            get { return GetValue(ItemsSourceProperty); }
            set { SetValue(ItemsSourceProperty, value); }
        }

        /// <summary>
        /// Dependency Registration for ItemsSource property
        /// </summary>
        /// <remarks></remarks>
        public static readonly DependencyProperty ItemsSourceProperty =
            DependencyProperty.Register("ItemsSource", typeof(object), typeof(SfMultiColumnDropDownControl), new PropertyMetadata(null, OnItemsSourceChanged));

        /// <summary>
        /// Called when ItemsSource is changed.
        /// </summary>
        /// <param name="d">The d.</param>
        /// <param name="e">The <see cref="DependencyPropertyChangedEventArgs"/> instance containing the event data.</param>
        private static void OnItemsSourceChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var sfMultiColumnControl = (SfMultiColumnDropDownControl)d;
            sfMultiColumnControl.ResetProperties();
            sfMultiColumnControl.appendSource = e.NewValue as IEnumerable<object>;
        }

        #endregion

        #region Customization Properties
        /// <summary>
        /// Gets or sets the popup border brush.
        /// </summary>
        /// <value>
        /// The popup border brush.
        /// </value>
        public Brush PopupBorderBrush
        {
            get { return (Brush)GetValue(PopupBorderBrushProperty); }
            set { SetValue(PopupBorderBrushProperty, value); }
        }

        /// <summary>
        /// The popup border brush property
        /// </summary>
        public static readonly DependencyProperty PopupBorderBrushProperty =
            DependencyProperty.Register("PopupBorderBrush", typeof(Brush), typeof(SfMultiColumnDropDownControl), new PropertyMetadata(new SolidColorBrush(Colors.Black)));

        /// <summary>
        /// Gets or sets the popup border thickness.
        /// </summary>
        /// <value>
        /// The popup border thickness.
        /// </value>
        public Thickness PopupBorderThickness
        {
            get { return (Thickness)GetValue(PopupBorderThicknessProperty); }
            set { SetValue(PopupBorderThicknessProperty, value); }
        }

        /// <summary>
        /// The popup border thickness property
        /// </summary>
        public static readonly DependencyProperty PopupBorderThicknessProperty =
            DependencyProperty.Register("PopupBorderThickness", typeof(Thickness), typeof(SfMultiColumnDropDownControl),
                                        new PropertyMetadata(new Thickness(1)));
        #endregion

        #region Popup Properties
        /// <summary>
        /// Gets or Sets the Maximum Height for the Popup
        /// </summary>
        /// <value></value>
        /// <remarks></remarks>
        public double PopupMaxHeight
        {
            get { return (double)GetValue(PopupMaxHeightProperty); }
            set { SetValue(PopupMaxHeightProperty, value); }
        }

        /// <summary>
        /// Dependency registration for PopupMaxHeight
        /// </summary>
        /// <remarks></remarks>
        public static readonly DependencyProperty PopupMaxHeightProperty =
            DependencyProperty.Register("PopupMaxHeight", typeof(double), typeof(SfMultiColumnDropDownControl), new PropertyMetadata(double.MaxValue));

        /// <summary>
        /// Gets or Sets the Maximum Width for the Popup
        /// </summary>
        /// <value></value>
        /// <remarks></remarks>
        public double PopupMaxWidth
        {
            get { return (double)GetValue(PopupMaxWidthProperty); }
            set { SetValue(PopupMaxWidthProperty, value); }
        }

        /// <summary>
        /// Dependency registration for PopupMaxWidth
        /// </summary>
        /// <remarks></remarks>
        public static readonly DependencyProperty PopupMaxWidthProperty =
            DependencyProperty.Register("PopupMaxWidth", typeof(double), typeof(SfMultiColumnDropDownControl), new PropertyMetadata(double.MaxValue));

        /// <summary>
        /// Gets or Sets the Minimum Height for the Popup
        /// </summary>
        /// <value></value>
        /// <remarks></remarks>
        public double PopupMinHeight
        {
            get { return (double)GetValue(PopupMinHeightProperty); }
            set { SetValue(PopupMinHeightProperty, value); }
        }

        /// <summary>
        /// Dependency registration for PopupMinHeight
        /// </summary>
        /// <remarks></remarks>
        public static DependencyProperty PopupMinHeightProperty =
            DependencyProperty.Register("PopupMinHeight", typeof(double), typeof(SfMultiColumnDropDownControl), new PropertyMetadata(DefaultPopupMinHeight));

        /// <summary>
        /// Gets or Sets the Minimum Width for the Popup
        /// </summary>
        /// <value></value>
        /// <remarks></remarks>
        public double PopupMinWidth
        {
            get { return (double)GetValue(PopupMinWidthProperty); }
            set { SetValue(PopupMinWidthProperty, value); }
        }

        /// <summary>
        /// Dependency registration for PopupMinWidth
        /// </summary>
        /// <remarks></remarks>
        public static DependencyProperty PopupMinWidthProperty =
            DependencyProperty.Register("PopupMinWidth", typeof(double), typeof(SfMultiColumnDropDownControl), new PropertyMetadata(DefaultPopupMinWidth));

        /// <summary>
        /// Gets or Sets the Height for the Popup
        /// </summary>
        /// <value></value>
        /// <remarks></remarks>
        public double PopupHeight
        {
            get { return (double)GetValue(PopupHeightProperty); }
            set { SetValue(PopupHeightProperty, value); }
        }

        /// <summary>
        /// Dependency registration for PopupHeight
        /// </summary>
        /// <remarks></remarks>
        public static readonly DependencyProperty PopupHeightProperty =
            DependencyProperty.Register("PopupHeight", typeof(double), typeof(SfMultiColumnDropDownControl), new PropertyMetadata(DefaultPopupMinHeight));

        /// <summary>
        /// Gets or Sets the Width for the Popup
        /// </summary>
        /// <value></value>
        /// <remarks></remarks>
        public double PopupWidth
        {
            get { return (double)GetValue(PopupWidthProperty); }
            set { SetValue(PopupWidthProperty, value); }
        }

        /// <summary>
        /// Dependency registration for PopupWidth
        /// </summary>
        /// <remarks></remarks>
        public static readonly DependencyProperty PopupWidthProperty =
            DependencyProperty.Register("PopupWidth", typeof(double), typeof(SfMultiColumnDropDownControl), new PropertyMetadata(DefaultPopupMinWidth));
        #endregion

        #endregion

        #region Public Events

        /// <summary>
        /// Occurs when Popup is Opening. 
        /// </summary>
        /// <remarks></remarks>
        public event PopupOpeningEventHandler PopupOpening;

        /// <summary>
        /// Occurs when Popup is Opened. 
        /// </summary>
        /// <remarks></remarks>
        public event PopupOpenedEventHandler PopupOpened;

        /// <summary>
        /// Occurs when Popup is Closing. 
        /// </summary>
        /// <remarks></remarks>
        public event PopupClosingEventHandler PopupClosing;

        /// <summary>
        /// Occurs when Popup is Closed. 
        /// </summary>
        /// <remarks></remarks>
        public event PopupClosedEventHandler PopupClosed;

        /// <summary>
        /// Occurs when Selection is Changed. 
        /// </summary>
        /// <remarks></remarks>
        public event SelectionChangedEventHandler SelectionChanged;
        #endregion

        #region Events Helper Methods
        private bool RaisePopupOpeningEvent(PopupOpeningEventArgs e)
        {
            if (PopupOpening != null)
            {
                PopupOpening(this, e);
                return e.Cancel;
            }
            return false;
        }

        private void RaisePopupOpenedEvent(PopupOpenedEventArgs e)
        {
            if (PopupOpened != null)
            {
                PopupOpened(this, e);
            }
        }

        private bool RaisePopupClosingEvent(PopupClosingEventArgs e)
        {
            if (PopupClosing != null)
            {
                PopupClosing(this, e);
                return e.Cancel;
            }
            return false;
        }

        private void RaisePopupClosedEvent(PopupClosedEventArgs e)
        {
            if (PopupClosed != null)
            {
                PopupClosed(this, e);
            }
        }

        private void RaiseSelectionChangedEvent(SelectionChangedEventArgs e)
        {
            if (SelectionChanged != null)
            {
                SelectionChanged(this, e);
            }
        }
        #endregion

        #region Dependency CallBack
        /// <summary>
        /// Called when [is drop down open changed].
        /// </summary>
        /// <param name="d">The d.</param>
        /// <param name="e">The <see cref="DependencyPropertyChangedEventArgs"/> instance containing the event data.</param>
        /// <exception cref="System.NotImplementedException"></exception>
        private static void OnIsDropDownOpenChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var sfMultiColumnDropDown = (SfMultiColumnDropDownControl)d;
            if ((bool)e.NewValue)
            {
                var isCancel = sfMultiColumnDropDown.RaisePopupOpeningEvent(new PopupOpeningEventArgs());
                sfMultiColumnDropDown.IsDropDownOpen = !isCancel;
                if (!isCancel)
                {
                    if (sfMultiColumnDropDown.IsAutoPopupSize)
                    {
                        double actualWidth = 0, actualHeight = 0;
                        var gridWidth = sfMultiColumnDropDown.InternalGrid.VisualContainer.ScrollColumns.ScrollBar.Maximum;
                        var gridHeight = sfMultiColumnDropDown.InternalGrid.VisualContainer.ScrollRows.ScrollBar.Maximum;
#if WinRT
                        var locationfromScreen = sfMultiColumnDropDown.TransformToVisual(null).TransformPoint(new Point(0, 0));
                        actualWidth = Window.Current.Bounds.Width - locationfromScreen.X;
                        actualHeight = Window.Current.Bounds.Height - locationfromScreen.Y;
#elif WPF
                        var locationfromWindow = sfMultiColumnDropDown.TranslatePoint(new Point(0, 0), sfMultiColumnDropDown);
                        var locationfromScreen = sfMultiColumnDropDown.PointToScreen(locationfromWindow);
                        var ypos = locationfromScreen.Y - locationfromWindow.Y;
                        var screenheight = SystemParameters.WorkArea.Height;
                        actualHeight = Math.Max((SystemParameters.WorkArea.Height - ypos), ypos);
                        actualWidth = SystemParameters.WorkArea.Width;
#elif SILVERLIGHT
                        var locationfromScreen = sfMultiColumnDropDown.TransformToVisual(null).Transform(new Point(0, 0));
                        actualWidth = (double)System.Windows.Browser.HtmlPage.Window.Eval("screen.availWidth") - locationfromScreen.X;
                        actualHeight = (double)System.Windows.Browser.HtmlPage.Window.Eval("screen.availHeight") - locationfromScreen.Y;
#endif
                        sfMultiColumnDropDown.actualPopupWidth = gridWidth < actualWidth ? gridWidth : actualWidth;
                        sfMultiColumnDropDown.actualPopupHeight = gridHeight < actualHeight ? gridHeight : actualHeight;

                        sfMultiColumnDropDown.PopupWidth = sfMultiColumnDropDown.actualPopupWidth;
                        sfMultiColumnDropDown.PopupHeight = sfMultiColumnDropDown.actualPopupHeight;
                    }
                    sfMultiColumnDropDown.RaisePopupOpenedEvent(new PopupOpenedEventArgs());
                    sfMultiColumnDropDown.ScrollInView();
                }
            }
            else
            {
                //sfMultiColumnDropDown.CommitValue();
                sfMultiColumnDropDown.ClearFilter();
                var isCancel = sfMultiColumnDropDown.RaisePopupClosingEvent(new PopupClosingEventArgs());
                sfMultiColumnDropDown.IsDropDownOpen = isCancel;
                if (!isCancel)
                    sfMultiColumnDropDown.RaisePopupClosedEvent(new PopupClosedEventArgs());
            }
        }

        /// <summary>
        /// Called when [selected value changed].
        /// </summary>
        /// <param name="d">The d.</param>
        /// <param name="e">The <see cref="DependencyPropertyChangedEventArgs"/> instance containing the event data.</param>
        /// <exception cref="System.NotImplementedException"></exception>
        private static void OnSelectedValueChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (!((SfMultiColumnDropDownControl)d).suspendSelectedValueChangedBeforeGridLoaded)
            {
                var sfMultiColumnDropDown = (SfMultiColumnDropDownControl)d;
                if (!string.IsNullOrEmpty(sfMultiColumnDropDown.ValueMember))
                {
                    if (sfMultiColumnDropDown.InternalGrid != null && sfMultiColumnDropDown.InternalGrid.View != null)
                    {
                        var record = sfMultiColumnDropDown.appendSource.FirstOrDefault
                            (o => (sfMultiColumnDropDown.InternalGrid.View.GetPropertyAccessProvider()
                                                   .GetValue(o, sfMultiColumnDropDown.ValueMember)
                                                   .Equals(e.NewValue)));
                        if (record != null)
                        {
                            sfMultiColumnDropDown.isSuspendUpdate = false;
                            sfMultiColumnDropDown.SelectedItem = record;
                        }
                    }
                    else
                    {
                        ((SfMultiColumnDropDownControl)d).isSelectedValueLoadedBeforeGridLoaded = true;
                    }
                }
            }
            ((SfMultiColumnDropDownControl)d).suspendSelectedValueChangedBeforeGridLoaded = false;
        }
        #endregion

        #region Ctor
        /// <summary>
        /// Initializes a new instance of the <see cref="SfMultiColumnDropDownControl"/> class.
        /// </summary>
        public SfMultiColumnDropDownControl()
        {
#if !WPF
            base.DefaultStyleKey = typeof (SfMultiColumnDropDownControl);
#endif
            SetValue(ColumnsProperty, new Columns());
        }

        /// <summary>
        /// Initializes the <see cref="SfMultiColumnDropDownControl"/> class.
        /// </summary>
        static SfMultiColumnDropDownControl()
        {
#if WPF
            DefaultStyleKeyProperty.OverrideMetadata(typeof(SfMultiColumnDropDownControl), new FrameworkPropertyMetadata(typeof(SfMultiColumnDropDownControl)));
#endif
        }
        #endregion

        #region Private Methods
        protected virtual void ProcessKeyDown(KeyEventArgs args)
        {
#if WPF
            var isAltKey = (args.KeyboardDevice.Modifiers & ModifierKeys.Alt) != ModifierKeys.None;
            var isShiftKey = (args.KeyboardDevice.Modifiers & ModifierKeys.Shift) != ModifierKeys.None;
            if (isAltKey && args.SystemKey == Key.Down || isAltKey && args.SystemKey == Key.Up || args.Key == Key.F4)
#elif SILVERLIGHT
            var isAltKey = ((Keyboard.Modifiers & ModifierKeys.Alt) == ModifierKeys.Alt);
            var isShiftKey = ((Keyboard.Modifiers & ModifierKeys.Shift) == ModifierKeys.Shift);
            if (isAltKey && args.Key == Key.Down || isAltKey && args.Key == Key.Up || args.Key == Key.F4)
#else
            var Alt = Window.Current.CoreWindow.GetAsyncKeyState(Key.Menu);
            var isAltKey = Alt.HasFlag(CoreVirtualKeyStates.Down);
            var Shift = Window.Current.CoreWindow.GetAsyncKeyState(Key.Shift);
            var isShiftKey = Shift.HasFlag(CoreVirtualKeyStates.Down);
            if(isAltKey && args.Key == Key.Down || isAltKey && args.Key == Key.Up || args.Key == Key.F4)
#endif
            {
                IsDropDownOpen = !IsDropDownOpen;
#if WinRT
                Editor.Focus(FocusState.Programmatic);
#else
                Editor.Focus();
#endif
                args.Handled = true;
                return;
            }
            if ((isShiftKey && args.Key == Key.Home || args.Key == Key.Home) && AllowAutoComplete)
            {
                Editor.SelectAll();
                args.Handled = true;
            }
            switch (args.Key)
            {
                case Key.Down:
                    {
                        isSuspendUpdate = false;
                        isTextChanged = true;
                        SelectedIndex = GetNextRowIndex();
                        if (!IsDropDownOpen)
                            SetDisplayText();
                        Editor.SelectAll();
                        ScrollInView();
                        args.Handled = true;
                        break;
                    }
                case Key.Up:
                    {
                        isSuspendUpdate = false;
                        isTextChanged = true;
                        SelectedIndex = GetPreviousRowIndex();
                        if (!IsDropDownOpen)
                            SetDisplayText();
                        Editor.SelectAll();
                        ScrollInView();
                        args.Handled = true;
                        break;
                    }
                case Key.PageDown:
                case Key.PageUp:
                    {
                        isSuspendUpdate = false;
                        isTextChanged = true;
                        InternalGrid.SelectionController.HandleKeyDown(args);
                        args.Handled = true;
                        break;
                    }
                case Key.Escape:
                    {
                        isTextChanged = true;
                        IsDropDownOpen = false;
                        break;
                    }
                case Key.Tab:
                case Key.Enter:
                    {
                        if (args.Key == Key.Escape)
                            SearchText = string.Empty;
                        if (args.Key == Key.Enter || (args.Key == Key.Tab && AllowAutoComplete))
                            CommitValue(true);
                        IsDropDownOpen = false;
#if WinRT
                    Editor.Focus(FocusState.Programmatic);
#elif WPF
                        Editor.SelectAll();
#endif
                        break;
                    }
                case Key.Back:
                case Key.Delete:
                    {
                        isTextChanged = true;
                        allowFilter = true;
                        if (ReadOnly)
                            break;
                        if (AllowAutoComplete)
                        {
                            isTextChanged = false;
#if WPF
                            if ((args.Key == Key.Back || args.Key == Key.Left) && (Editor.CaretIndex - 1 > 0))
#else
                        if (args.Key == Key.Back && Editor.SelectionStart - 1 >= 0)
#endif
                            {
#if WPF
                                if ((Editor.SelectedText.Length > 0) && Editor.SelectionStart > 0)
#endif
                                {
#if WPF
                                    Editor.Select(Editor.CaretIndex - 1, Editor.Text.Length);
                                    SearchText = Editor.Text.Substring(0, (Editor.Text.Length - Editor.SelectedText.Length));
#else
                                if(InternalGrid.SelectedItem!=null)
                                Editor.Select(Editor.SelectionStart - 1, Editor.Text.Length);
#endif
                                    args.Handled = true;
                                }
                            }
                            else if (Editor.SelectionStart == (Editor.Text.Length - Editor.SelectedText.Length))
                            {
#if WPF
                                if (Editor.CaretIndex <= 1)
#else
                            if (Editor.SelectionStart <= 1)
#endif
                                    //ClearSelection();
                                    Editor.SelectAll();
                            }
                        }
                        if (!AllowIncrementalFiltering)
                            isSuspendUpdate = false;
                        else
                        {
                            isTextChanged = false;
                            ProcessOnEditorTextChanged(Editor, null);
                        }
                        break;
                    }

                default:
                    {
#if WinRT
                    Editor.Focus(FocusState.Programmatic);
#endif
                        allowFilter = true;
                        isSuspendUpdate = !AllowAutoComplete;
                        isTextChanged = false;
                        if (AllowImmediatePopup)
#if WPF
                            if (!(args.Key == Key.Home || args.Key == Key.End || args.Key == Key.LeftAlt || args.Key == Key.LeftCtrl || args.Key == Key.LeftShift || args.Key == Key.RightAlt || args.Key == Key.RightCtrl || args.Key == Key.RightShift || args.Key == Key.System || args.Key == Key.Left || args.Key == Key.Right))
#elif WinRT
                    if(!(args.Key==Key.Home||args.Key==Key.End||args.Key==Key.Menu||args.Key==Key.Control||args.Key==Key.Shift))
#else
                    if(!(args.Key==Key.Home||args.Key==Key.End||args.Key==Key.Alt||args.Key==Key.Ctrl||args.Key==Key.Shift))
#endif
                            {
                                IsDropDownOpen = true;
#if WinRT
                        Editor.Focus(FocusState.Programmatic);
#endif
                            }
                        break;
                    }
            }
        }

        protected virtual void ProcessOnSelectedItemChanged(object selectedItem)
        {
            if (selectedItem == null || isTextChanged)
            {
                isTextChanged = false;
                return;
            }
            if (InternalGrid.SelectedItem != selectedItem)
                InternalGrid.SelectedItem = selectedItem;

            SetDisplayText(selectedItem);
            SetSelectedValue(selectedItem);
        }

        protected virtual void SetDisplayText(object selectedItem)
        {
            if (!string.IsNullOrEmpty(DisplayMember) && !isSuspendUpdate)
            {
                var displayValue = InternalGrid.View.GetPropertyAccessProvider().GetValue(selectedItem, DisplayMember);
                if (displayValue != null)
                {
                    allowFilter = false;
#if WPF
                    Text = string.Empty;
#endif
                    Text = displayValue.ToString();
                }
            }
            else
            {
                if (!isSuspendUpdate)
                    Text = selectedItem.ToString();
            }
        }

        protected virtual void SetDisplayText()
        {
            if (!string.IsNullOrEmpty(DisplayMember) && !isSuspendUpdate && InternalGrid.SelectedItem != null)
            {
                var displayValue = InternalGrid.View.GetPropertyAccessProvider().GetValue(InternalGrid.SelectedItem, DisplayMember);
                if (displayValue != null)
                {
                    Text = displayValue.ToString();
                }
            }
            else
            {
                if (!isSuspendUpdate)
                    Text = string.Empty;
            }
        }

        protected virtual void SetSelectedValue(object selectedItem)
        {
            if (!string.IsNullOrEmpty(ValueMember))
            {
                var value = InternalGrid.View.GetPropertyAccessProvider().GetValue(selectedItem, ValueMember);
                suspendSelectedValueChangedBeforeGridLoaded = true;
                if ((SelectedValue == null && value != null) || (SelectedValue != null && !SelectedValue.Equals(value)))
                    SelectedValue = value;
            }
            else
            {
                suspendSelectedValueChangedBeforeGridLoaded = true;
                if (SelectedValue != selectedItem)
                    SelectedValue = selectedItem;
            }
            suspendSelectedValueChangedBeforeGridLoaded = false;
        }

        protected virtual void CommitValue(bool autoCommit = false)
        {
            isSuspendUpdate = false;
            object _selectedItem = null;

            bool canRaiseSelectionChanged = PreviousSelectedItem != null
                ? !PreviousSelectedItem.Equals(InternalGrid.SelectedItem)
                : PreviousSelectedItem != InternalGrid.SelectedItem;
            if (autoCommit)
            {
                PreviousSelectedItem = InternalGrid.SelectedItem;
                _selectedItem = InternalGrid.SelectedItem;
            }
            else
            {
                var displayText = string.Empty;
                if (!string.IsNullOrEmpty(DisplayMember) && !isSuspendUpdate && InternalGrid.SelectedItem != null)
                {
                    var displayValue = InternalGrid.View.GetPropertyAccessProvider()
                                                   .GetValue(InternalGrid.SelectedItem, DisplayMember);
                    if (displayValue != null)
                        displayText = displayValue.ToString();
                }
                _selectedItem = string.Equals(displayText, Editor.Text)
                                    ? InternalGrid.SelectedItem
                                    : PreviousSelectedItem;
            }
            if (_selectedItem != null)
            {
                if (AllowNullInput && string.IsNullOrEmpty(Editor.Text))
                {
                    InternalGrid.SelectedItems.Clear();
                    SelectedItem = null;
                    ScrollInView();
                    Text = string.Empty;
                }
                else
                {
                    SelectedItem = _selectedItem;
                    if (!autoCommit)
                        PreviousSelectedItem = SelectedItem;
                    SetDisplayText(SelectedItem);
                }
                if (canRaiseSelectionChanged)
                    RaiseSelectionChangedEvent(new SelectionChangedEventArgs
                    {
                        SelectedIndex = InternalGrid.SelectedIndex,
                        SelectedItem = SelectedItem
                    });
            }
            else
            {
                Text = null;
                SelectedItem = null;
                suspendSelectedValueChangedBeforeGridLoaded = true;
                SelectedValue = string.Empty;
            }
            SearchText = string.Empty;
        }

#if WinRT
        protected virtual void ProcessOnMouseWheelSpin(object sender, MouseButtonEventArgs e)
#else
        protected virtual void ProcessOnMouseWheelSpin(object sender, MouseWheelEventArgs e)
#endif
        {
#if WinRT
            var point = e.GetCurrentPoint(this);
            var delta = point.Properties.MouseWheelDelta;
#else
            var delta = e.Delta;
#endif
            if (!AllowSpinOnMouseWheel) return;
            SelectedIndex = delta < 0 ? GetNextRowIndex() : GetPreviousRowIndex();
            SelectedItem = InternalGrid.SelectedItem;
            Editor.SelectAll();
        }

        protected virtual void ProcessOnEditorTextChanged(object sender, TextChangedEventArgs e)
        {
            if (isTextChanged)
            {
                isTextChanged = false;
                return;
            }
            var collection = new ObservableCollection<object>();
            var textBoxEditor = sender as TextBox;
            if (textBoxEditor != null)
            {
                filterText = textBoxEditor.Text;
                filterText = filterText.Substring(0, (textBoxEditor.Text.Length - textBoxEditor.SelectedText.Length));
                SearchText = filterText;
            }
            appendSource = ItemsSource as IEnumerable<object>;
            if (appendSource != null)
            {
                var textLength = filterText.Length;
                var iterationCount = 0;
                var exactValue = string.Empty;
                bool isSelectedItemSet = false;
                foreach (var item in appendSource)
                {
                    ++iterationCount;
                    if (string.IsNullOrEmpty(DisplayMember))
                        return;
                    dataHelper = InternalGrid.View.GetPropertyAccessProvider();
                    exactValue = dataHelper.GetValue(item, DisplayMember).ToString();
                    if (!AllowCaseSensitiveFiltering)
                    {
                        filterText = filterText.ToLower();
                        exactValue = exactValue.ToLower();
                    }
                    if (ProcessAppendText(item, exactValue, filterText))
                    {
                        if ((!AllowIncrementalFiltering || (AllowIncrementalFiltering && !IsDropDownOpen)) && !string.IsNullOrEmpty(Editor.Text))
                        {
#if WPF
                            if (AllowAutoComplete)
                                isTextChanged = true;
#else
                            isTextChanged = true;
#endif
                            if (!string.IsNullOrEmpty(filterText))
                            {
                                isSelectedItemSet = true;
                                InternalGrid.SelectedItem = item;
                            }
                        }
                        var _value = dataHelper.GetValue(item, DisplayMember).ToString();
                        var _source = ProcessAppendStringList(item, _value);
                        foreach (var _item in _source)
                            collection.Add(_item as string);
                    }
                    if (collection.Count > 0)
                        break;
                }
                if (!isSelectedItemSet && PreviousSelectedItem != null && collection.Count < 1)
                {
                    isSuspendUpdate = true;
                    InternalGrid.SelectedItem = PreviousSelectedItem;
                }
                if (AllowAutoComplete)
                {
                    if (collection.Count > 0)
                    {
#if !WPF
                        if (!string.IsNullOrEmpty(Text) && Editor.SelectionLength == Text.Length - textLength)
                            return;
#endif
                        isSuspendUpdate = true;
                        var append = string.Empty;
                        var firstOrDefault = collection.FirstOrDefault();
                        if (firstOrDefault != null && Editor.Text != string.Empty)
                            append = firstOrDefault.ToString();
                        isTextChanged = true;
                        Text = string.Empty;
                        isTextChanged = true;
#if WPF
                        Text = append;
#else
                        if(!string.IsNullOrEmpty(filterText))
                        Text = append;
#endif
                        if (string.IsNullOrEmpty(Text))
                            isTextChanged = false;
                        Editor.SelectionStart = textLength;
                        Editor.SelectionLength = Text.Length - textLength;
                        isSuspendUpdate = false;
                    }
                }
                if (AllowIncrementalFiltering && allowFilter && IsDropDownOpen)
                {
                    ProcessIncrementalFiltering();
#if WPF
                    allowFilter = false;
                    if (AllowIncrementalFiltering)
                        allowFilter = true;
                    else
                    {
                        allowFilter = false;
                    }
#else
                    allowFilter = false;
#endif
                }

            }
            ScrollInView();
        }

        protected virtual bool ProcessAppendText(object item, string exactValue, string filterText)
        {
            return exactValue.StartsWith(filterText);
        }

        protected virtual List<string> ProcessAppendStringList(object item, string _value)
        {
            var _newList = new List<string> { _value };
            return _newList;
        }

        /// <summary>
        /// Processes the Incremental Filtering.
        /// </summary>
        protected virtual void ProcessIncrementalFiltering()
        {
            if (AllowIncrementalFiltering && InternalGrid.VisualContainer != null)
            {
                isSuspendUpdate = true;
                InternalGrid.View.Filter += FilterRecord;
                InternalGrid.View.RefreshFilter();
                InternalGrid.SelectionController.HandleGridOperations(new GridOperationsHandle(GridOperation.Filtering, null));
                FilteredItems = InternalGrid.View.Records;
                isSuspendUpdate = false;
            }
        }

        /// <summary>
        /// Scrolls the Row in View.
        /// </summary>
        private void ScrollInView()
        {
            if (InternalGrid == null || InternalGrid.ItemsSource == null || InternalGrid.VisualContainer == null ||
                InternalGrid.SelectedIndex < 0 || InternalGrid.SelectedIndex > InternalGrid.VisualContainer.RowCount)
                return;
            InternalGrid.VisualContainer.ScrollRows.ScrollInView(InternalGrid.SelectedIndex);
            InternalGrid.VisualContainer.InvalidateMeasureInfo();
        }

        /// <summary>
        /// Resets the MultiColumn Control.
        /// </summary>
        private void ResetProperties()
        {
            appendSource = null;
            Text = string.Empty;
        }

        /// <summary>
        /// Returns TRUE if the item will be in View in the Filtered List, else returns FALSE
        /// </summary>
        /// <param name="item"></param>
        /// <returns></returns>
        /// <remarks></remarks>
        protected virtual bool FilterRecord(object item)
        {
            if (item == null || DisplayMember == null)
                return false;
            dataHelper = InternalGrid.View.GetPropertyAccessProvider();
            var exactValue = dataHelper.GetValue(item, DisplayMember).ToString();
            if (!AllowCaseSensitiveFiltering)
                exactValue = exactValue.ToLower();
            return exactValue.StartsWith(filterText);
        }

        private void ProcessInitialization()
        {
#if !WPF
            InternalPopup.Margin = !double.IsNaN(Height) ? new Thickness(0, Height, 0, 0) : new Thickness(0, MinHeight, 0, 0);
#else
            if (Columns.IsFrozen)
                Columns = (Columns)Columns.Clone();
#endif
            InternalGrid.Columns = Columns;
            InternalGrid.AutoGenerateColumns = this.AutoGenerateColumns;
            InternalGrid.AutoGenerateColumnsMode = this.AutoGenerateColumnsMode;
            if (isSelectedValueLoadedBeforeGridLoaded)
            {
                if (InternalGrid != null && InternalGrid.View != null)
                {
                    var record = appendSource.FirstOrDefault
                        (o => (InternalGrid.View.GetPropertyAccessProvider()
                                               .GetValue(o, ValueMember)
                                               .Equals(this.SelectedValue)));
                    if (record != null)
                    {
                        isSuspendUpdate = false;
                        SelectedItem = record;
                    }
                    isSelectedValueLoadedBeforeGridLoaded = false;
                }
            }
        
            InternalGrid.Measure(new Size(PopupWidth, PopupHeight));
        }

        /// <summary>
        /// Clears the selection.
        /// </summary>
        private void ClearSelection()
        {
            SelectedValue = null;
            SelectedItem = null;
        }

        private void ClearFilter()
        {
            if (InternalGrid != null && InternalGrid.View != null)
            {
                if (InternalGrid.View.Filter != null)
                {
                    InternalGrid.View.Filter = null;
                    InternalGrid.View.RefreshFilter();
                }
                FilteredItems = InternalGrid.View.Records;
                if (SelectedItem != null)
                {
                    if (InternalGrid.View.GroupDescriptions.Count > 0)
                    {
                        var record = InternalGrid.View.Records.GetRecord(SelectedItem);
                        InternalGrid.SelectedIndex = InternalGrid.View.TopLevelGroup.DisplayElements.IndexOf(record);
                    }
                    else
                        InternalGrid.SelectedIndex = InternalGrid.View.Records.IndexOfRecord(SelectedItem);
                }
            }
        }

        /// <summary>
        /// Gets the index of the previous row from the Internal Grid.
        /// </summary>
        /// <returns></returns>
        private int GetPreviousRowIndex()
        {
            return (SelectedIndex > 0 ? SelectedIndex - 1 : SelectedIndex);
        }

        /// <summary>
        /// Gets the index of the next row from the Internal Grid.
        /// </summary>
        /// <returns></returns>
        private int GetNextRowIndex()
        {
            return (SelectedIndex < InternalGrid.VisualContainer.RowCount - InternalGrid.headerLineCount - 1 ? SelectedIndex + 1 : SelectedIndex);
        }
        #endregion

        #region Public Methods
        /// <summary>
        /// Returns the Embedded DropDown DataGrid in the Popup.
        /// </summary>
        /// <returns></returns>
        /// <remarks></remarks>
        public SfDataGrid GetDropDownGrid()
        {
            return InternalGrid;
        }
        #endregion

        #region Override Methods
#if WinRT
        protected override void OnApplyTemplate()
#else
        public override void OnApplyTemplate()
#endif
        {
            base.OnApplyTemplate();
            MainBorder = GetTemplateChild("PART_RootBorder") as Border;
            Editor = GetTemplateChild("PART_TextBox") as TextBox;
            DropDownButton = GetTemplateChild("PART_ToggleButton") as ToggleButton;
            InternalPopup = GetTemplateChild("PART_Popup") as Popup;
            InternalPopupBorder = GetTemplateChild("PART_PopupBorder") as Border;
            InternalGrid = GetTemplateChild("PART_SfDataGrid") as SfDataGrid;
            ResizeThumb = GetTemplateChild("PART_ThumbGripper") as Thumb;
            WireEvents();
            ProcessInitialization();
        }

        protected override Size ArrangeOverride(Size arrangeBounds)
        {
            if (DefaultPopupMinWidth < arrangeBounds.Width)
                PopupMinWidth = arrangeBounds.Width;
            return base.ArrangeOverride(arrangeBounds);
        }
        #endregion

        #region Events

#if WinRT
        private void OnDropDownButtonClicked(object sender, TappedRoutedEventArgs e)
#else
        private void OnDropDownButtonClicked(object sender, RoutedEventArgs e)
#endif
        {
            Editor.SelectAll();
            IsDropDownOpen = !IsDropDownOpen;
            if (IsDropDownOpen)
            {
#if WinRT
                Editor.Focus(FocusState.Programmatic);
#else
                Editor.Focus();
#endif
            }
        }

        private void OnMouseOverToggleButton(object sender, MouseEventArgs e)
        {
#if WPF
            InternalPopup.StaysOpen = !InternalPopup.StaysOpen;
#endif
        }

        private void OnEditorKeyDown(object sender, KeyEventArgs e)
        {
            ProcessKeyDown(e);
        }

#if WinRT
        private void OnInternalGridTapped(object sender, TappedRoutedEventArgs e)
#else
        private void OnInternalGridTapped(object sender, MouseButtonEventArgs e)
#endif
        {
            isSuspendUpdate = false;
#if !WinRT
            var point = e.GetPosition(InternalGrid);
            if (point.Y < InternalGrid.HeaderRowHeight)
            {
                Editor.Focus();
                return;
            }
#else
            var pp = e.GetPosition(null);
            var uielements = VisualTreeHelper.FindElementsInHostCoordinates(pp, InternalGrid);
            if (uielements.Count() > 0 && uielements.Any(element => element is GridHeaderCellControl))
            {
                Editor.Focus(FocusState.Programmatic);
                return;
            }
#endif
            isSuspendUpdate = false;
            SelectedItem = InternalGrid.SelectedItem;
            CommitValue(true);
            IsDropDownOpen = false;
#if WinRT
            Editor.Focus(FocusState.Programmatic);
#else
            Editor.Focus();
#endif
        }

        private void ProcessDragCompleted(object sender, DragCompletedEventArgs e)
        {
#if WinRT
            Window.Current.CoreWindow.PointerCursor = new CoreCursor(CoreCursorType.Arrow, 1);
#endif
        }

#if WinRT
        private void ProcessResizeThumbOnMouseLeave(object sender, MouseButtonEventArgs e)
#else
        private void ProcessResizeThumbOnMouseLeave(object sender, MouseEventArgs e)
#endif
        {
#if WinRT
            Window.Current.CoreWindow.PointerCursor = new CoreCursor(CoreCursorType.Arrow, 1);
#else
            ResizeThumb.Cursor = Cursors.Arrow;
#endif
        }

#if WinRT
        private void ProcessResizeThumbOnMouseEnter(object sender, MouseButtonEventArgs e)
#else
        private void ProcessResizeThumbOnMouseEnter(object sender, MouseEventArgs e)
#endif
        {
#if WinRT
            Window.Current.CoreWindow.PointerCursor = new CoreCursor(CoreCursorType.SizeNorthwestSoutheast, 1);
#else
            ResizeThumb.Cursor = Cursors.SizeNWSE;
#endif
        }

        private void ProcessResizeDragDelta(object sender, DragDeltaEventArgs e)
        {
            var popupHeight = PopupHeight + e.VerticalChange;
            if (popupHeight > 0 || popupHeight >= PopupMinHeight)
                PopupHeight += e.VerticalChange;
            var popupWidth = PopupWidth + e.HorizontalChange;
            if (popupWidth > 0 || popupWidth >= PopupMinWidth)
                PopupWidth += e.HorizontalChange;
        }

        void InternalGrid_ItemsSourceChanged(object sender, GridItemsSourceChangedEventArgs e)
        {
            if (InternalGrid != null && InternalGrid.View != null)
            {
                var record = appendSource.FirstOrDefault
                    (o => (InternalGrid.View.GetPropertyAccessProvider()
                                           .GetValue(o, ValueMember)
                                           .Equals(this.SelectedValue)));
                if (record != null)
                {
                    isSuspendUpdate = false;
                    SelectedItem = record;
                }
                isSelectedValueLoadedBeforeGridLoaded = false;
            }
        }

        private void ProcessOnInternalGridLoaded(object sender, RoutedEventArgs e)
        {
            if (InternalGrid.View != null)
                InternalGrid.View.RecordPropertyChanged += OnRecordPropertyChanged;
        }

        private void OnRecordPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (sender == InternalGrid.SelectedItem)
                ProcessOnSelectedItemChanged(InternalGrid.SelectedItem);
        }

        private void ProcessOnSelectionChanged(object sender, GridSelectionChangedEventArgs e)
        {
#if !SILVERLIGHT
            if (isSelectedIndexLoadedBeforeGridLoaded)
#endif
            {
                SelectedItem = InternalGrid.SelectedItem;
                ProcessOnSelectedItemChanged(InternalGrid.SelectedItem);
                ScrollInView();
                isSelectedIndexLoadedBeforeGridLoaded = false;
            }
        }

        private void OnEditorLostFocus(object sender, RoutedEventArgs e)
        {
            SearchText = string.Empty;
            if (!IsDropDownOpen)
                CommitValue();
        }

        private void ProcessPopupClosed(object sender, object e)
        {
            Editor.SelectAll();
        }
#if WinRT
        void ProcessPopupOpened(object sender, object e)
#else
        private void ProcessPopupOpened(object sender, EventArgs e)
#endif
        {
            if (AllowIncrementalFiltering && InternalGrid.SelectedItem != null && !string.IsNullOrEmpty(DisplayMember))
            {
                var _value = InternalGrid.View.GetPropertyAccessProvider().GetValue(InternalGrid.SelectedItem, DisplayMember);
                if (_value != null)
                {
                    if (!string.Equals(Editor.Text, _value.ToString()))
                        ProcessIncrementalFiltering();
                }
            }
        }
        #endregion

        #region WireEvents
        /// <summary>
        /// Wires the events in SfMultiColumnDropDown Control.
        /// </summary>
        private void WireEvents()
        {
            if (InternalGrid != null)
            {
                InternalGrid.Loaded += ProcessOnInternalGridLoaded;
                InternalGrid.ItemsSourceChanged += InternalGrid_ItemsSourceChanged;
                InternalGrid.SelectionChanged += ProcessOnSelectionChanged;
#if WinRT
                InternalGrid.Tapped += OnInternalGridTapped;
#elif WPF
                InternalGrid.MouseLeftButtonUp += OnInternalGridTapped;
                this.GotFocus += OnGotFocus;
#else
                InternalGrid.MouseLeftButtonDown += OnInternalGridTapped;
#endif
            }
            if (DropDownButton != null)
            {
#if WPF
                DropDownButton.AddHandler(ButtonBase.ClickEvent, (RoutedEventHandler)OnDropDownButtonClicked);
                DropDownButton.AddHandler(MouseEnterEvent, (MouseEventHandler)OnMouseOverToggleButton);
                DropDownButton.AddHandler(MouseLeaveEvent, (MouseEventHandler)OnMouseOverToggleButton);
#elif SILVERLIGHT
                DropDownButton.Click += OnDropDownButtonClicked;
#else
                DropDownButton.AddHandler(TappedEvent, (TappedEventHandler)OnDropDownButtonClicked, true);
#endif
            }
            if (Editor != null)
            {
#if WPF
                Editor.AddHandler(PreviewKeyDownEvent, (KeyEventHandler)OnEditorKeyDown);
                Editor.AddHandler(MouseWheelEvent, (MouseWheelEventHandler)ProcessOnMouseWheelSpin);
                Editor.AddHandler(TextBoxBase.TextChangedEvent, (TextChangedEventHandler)ProcessOnEditorTextChanged);
#elif SILVERLIGHT
                Editor.AddHandler(KeyDownEvent, (KeyEventHandler)OnEditorKeyDown, true);
                Editor.MouseWheel += ProcessOnMouseWheelSpin;
                Editor.TextChanged += ProcessOnEditorTextChanged;
                Editor.LostFocus += (sender, args) => { IsDropDownOpen = false; };
#else
                Editor.AddHandler(KeyDownEvent, (KeyEventHandler)OnEditorKeyDown, true);
                Editor.TextChanged += ProcessOnEditorTextChanged;
                Editor.PointerWheelChanged += ProcessOnMouseWheelSpin;
#endif
                Editor.LostFocus += OnEditorLostFocus;
            }
            if (ResizeThumb != null)
            {
#if WPF
                ResizeThumb.AddHandler(MouseEnterEvent, (MouseEventHandler)ProcessResizeThumbOnMouseEnter);
                ResizeThumb.AddHandler(MouseLeaveEvent, (MouseEventHandler)ProcessResizeThumbOnMouseLeave);
                ResizeThumb.AddHandler(Thumb.DragDeltaEvent, (DragDeltaEventHandler)ProcessResizeDragDelta);
#elif SILVERLIGHT
                ResizeThumb.MouseEnter += ProcessResizeThumbOnMouseEnter;
                ResizeThumb.MouseLeave += ProcessResizeThumbOnMouseLeave;
                ResizeThumb.DragDelta += ProcessResizeDragDelta;
#else
                ResizeThumb.PointerEntered += ProcessResizeThumbOnMouseEnter;
                ResizeThumb.PointerExited += ProcessResizeThumbOnMouseLeave;
                ResizeThumb.DragDelta += ProcessResizeDragDelta;
                ResizeThumb.DragCompleted += ProcessDragCompleted;
#endif
            }
            if (InternalPopup != null)
            {
                InternalPopup.Closed += ProcessPopupClosed;
                InternalPopup.Opened += ProcessPopupOpened;
            }
        }

        private void OnGotFocus(object sender, RoutedEventArgs e)
        {
            if (Editor == null)
                return;
#if WinRT
            Editor.Focus(FocusState.Programmatic);
#else
            Editor.Focus();
#endif
        }
        #endregion

        #region UnWireEvents
        /// <summary>
        /// Unsubscribes the Wired events in SfMultiColumnDropDown Control.
        /// </summary>
        private void UnWireEvents()
        {
            if (InternalGrid != null)
            {
                InternalGrid.Loaded -= ProcessOnInternalGridLoaded;
                InternalGrid.ItemsSourceChanged -= InternalGrid_ItemsSourceChanged;
                InternalGrid.SelectionChanged -= ProcessOnSelectionChanged;
#if WinRT
                InternalGrid.Tapped -= OnInternalGridTapped;
#elif WPF
                InternalGrid.MouseLeftButtonUp -= OnInternalGridTapped;
                this.GotFocus -= OnGotFocus;
#endif
            }
            if (DropDownButton != null)
            {
#if WPF
                DropDownButton.RemoveHandler(ButtonBase.ClickEvent, (RoutedEventHandler)OnDropDownButtonClicked);
                DropDownButton.RemoveHandler(MouseEnterEvent, (MouseEventHandler)OnMouseOverToggleButton);
                DropDownButton.RemoveHandler(MouseLeaveEvent, (MouseEventHandler)OnMouseOverToggleButton);
#elif SILVERLIGHT
                DropDownButton.Click -= OnDropDownButtonClicked;
#else
                DropDownButton.RemoveHandler(TappedEvent, (TappedEventHandler)OnDropDownButtonClicked);
#endif
            }
            if (Editor != null)
            {
#if WPF
                Editor.RemoveHandler(PreviewKeyDownEvent, (KeyEventHandler)OnEditorKeyDown);
                Editor.RemoveHandler(MouseWheelEvent, (MouseWheelEventHandler)ProcessOnMouseWheelSpin);
                Editor.RemoveHandler(TextBoxBase.TextChangedEvent, (TextChangedEventHandler)ProcessOnEditorTextChanged);
#elif SILVERLIGHT
                Editor.RemoveHandler(KeyDownEvent, (KeyEventHandler)OnEditorKeyDown);
                Editor.MouseWheel -= ProcessOnMouseWheelSpin;
                Editor.TextChanged -= ProcessOnEditorTextChanged;
#else
                Editor.RemoveHandler(KeyDownEvent, (KeyEventHandler)OnEditorKeyDown);
                Editor.TextChanged -= ProcessOnEditorTextChanged;
                Editor.PointerWheelChanged -= ProcessOnMouseWheelSpin;
#endif
                Editor.LostFocus -= OnEditorLostFocus;
            }
            if (ResizeThumb != null)
            {
#if WPF
                ResizeThumb.RemoveHandler(MouseEnterEvent, (MouseEventHandler)ProcessResizeThumbOnMouseEnter);
                ResizeThumb.RemoveHandler(MouseLeaveEvent, (MouseEventHandler)ProcessResizeThumbOnMouseLeave);
                ResizeThumb.RemoveHandler(Thumb.DragDeltaEvent, (DragDeltaEventHandler)ProcessResizeDragDelta);
#elif SILVERLIGHT
                ResizeThumb.MouseEnter -= ProcessResizeThumbOnMouseEnter;
                ResizeThumb.MouseLeave -= ProcessResizeThumbOnMouseLeave;
                ResizeThumb.DragDelta -= ProcessResizeDragDelta;
#else
                ResizeThumb.PointerEntered -= ProcessResizeThumbOnMouseEnter;
                ResizeThumb.PointerExited -= ProcessResizeThumbOnMouseLeave;
                ResizeThumb.DragDelta -= ProcessResizeDragDelta;
#endif
            }
            if (InternalPopup != null)
            {
                InternalPopup.Closed -= ProcessPopupClosed;
                InternalPopup.Opened -= ProcessPopupOpened;
            }
        }
        #endregion

        #region IDisposeable
        public void Dispose()
        {
            UnWireEvents();
            if (InternalGrid != null)
            {
                InternalGrid.Dispose();
                InternalGrid = null;
            }
        }
        #endregion
    }
}