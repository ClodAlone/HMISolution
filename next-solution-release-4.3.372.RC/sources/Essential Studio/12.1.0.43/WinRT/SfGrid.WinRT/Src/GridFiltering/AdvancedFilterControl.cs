#region Copyright Syncfusion Inc. 2001 - 2014
// Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Any infringement will be prosecuted under
// applicable laws. 
#endregion
using System;
using System.Linq;
using System.ComponentModel;
using Syncfusion.Data;
using System.Collections.ObjectModel;

#if WinRT
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls.Primitives;
using Syncfusion.UI.Xaml.Controls.Input;
using Syncfusion.UI.Xaml.Controls.Data;
#else
using System.Windows.Controls;
using System.Windows;
using System.Windows.Controls.Primitives;
using Syncfusion.Windows.Shared;
using System.Globalization;
using Syncfusion.Windows.Tools.Controls;
#endif

namespace Syncfusion.UI.Xaml.Grid
{
#if WinRT
    public class AdvancedFilterControl : ContentControl, IDisposable, INotifyPropertyChanged, IDataValidation
#else
    public class AdvancedFilterControl : ContentControl, IDisposable, INotifyPropertyChanged, IDataErrorInfo
#endif

    {
        #region private fields

        private string filterType1;
        private string filterType2;
        private object filterValue1;
        private object filterValue2;
#if !WPF
        bool datapickervisibility = true;
#endif
#if WPF
        bool casingbuttonvisibility = true;
#endif

        private object datefilterValue1;
        private object datefilterValue2;

        private object filterSelectedItem1;
        private object filterSelectedItem2;
        private ObservableCollection<FilterElement> comboSource;
        private bool isORChecked = true;
        #endregion

        #region internal fields
        internal bool isCaseSensitive1 = false;
        internal bool isCaseSensitive2 = false;
        internal bool propertyChangedfromsettingControlValues = false;
        internal GridFilterControl gridFilterCtrl = null;
        #endregion

        #region CLR Properties

        public string FilterType1
        {
            get { return filterType1; }
            set
            {
                filterType1 = value;
                if (FilterType1 != null && (FilterType1.ToString() == GridResourceWrapper.Null || FilterType1.ToString() == GridResourceWrapper.NotNull
                    || FilterType1.ToString() == GridResourceWrapper.Empty || FilterType1.ToString() == GridResourceWrapper.NotEmpty))
                {
                    FilterValue1 = null;
                    FilterSelectedItem1 = null;
                }
                OnPropertyChanged("FilterType1");
            }
        }

        public string FilterType2
        {
            get { return filterType2; }
            set
            {
                filterType2 = value;
                if (FilterType2 != null && (FilterType2.ToString() == GridResourceWrapper.Null || FilterType2.ToString() == GridResourceWrapper.NotNull
                    || FilterType2.ToString() == GridResourceWrapper.Empty || FilterType2.ToString() == GridResourceWrapper.NotEmpty))
                {
                    FilterValue2 = null;
                    FilterSelectedItem2 = null;
                }
                OnPropertyChanged("FilterType2");
            }
        }

        public object DateFilterValue1
        {
            get { return datefilterValue1; }
            set
            {
                datefilterValue1 = value;
                if (value != null && !string.IsNullOrEmpty(value.ToString()))
                {
                    if (this.gridFilterCtrl != null)
                        FilterValue1 = this.gridFilterCtrl.GetFormatedString(value);
                }
                else
                    FilterValue1 = value;
                OnPropertyChanged("DateFilterValue1");
            }
        }

        public object DateFilterValue2
        {
            get { return datefilterValue2; }
            set
            {

                datefilterValue2 = value;
                if (value != null && !string.IsNullOrEmpty(value.ToString()))
                {
                    if (this.gridFilterCtrl != null)
                        FilterValue2 = this.gridFilterCtrl.GetFormatedString(value);
                }
                else
                    FilterValue2 = value;
                OnPropertyChanged("DateFilterValue2");

            }
        }

        public object FilterValue1
        {
            get { return filterValue1; }
            set
            {
                if (FilterValue1 != value)
                {
                    if ((FilterValue1 != null && !FilterValue1.Equals(value)) || FilterValue1 == null)
                    {
                        filterValue1 = value;
                        OnPropertyChanged("FilterValue1");
                    }
                }
            }
        }


        public object FilterValue2
        {
            get { return filterValue2; }
            set
            {
                if (FilterValue2 != value)
                {
                    if ((FilterValue2 != null && !FilterValue2.Equals(value)) || FilterValue2 == null)
                    {
                        filterValue2 = value;
                        OnPropertyChanged("FilterValue2");
                    }
                }
            }
        }
        public object FilterSelectedItem1
        {
            get { return filterSelectedItem1; }
            set
            {
                if (filterSelectedItem1 != value)
                {
                    filterSelectedItem1 = value;
                    OnPropertyChanged("FilterSelectedItem1");
                }
            }
        }

        public object FilterSelectedItem2
        {
            get { return filterSelectedItem2; }
            set
            {
                if (filterSelectedItem2 != value)
                {
                    filterSelectedItem2 = value;
                    OnPropertyChanged("FilterSelectedItem2");
                }
            }
        }

        public ObservableCollection<FilterElement> ComboItemsSource
        {
            get
            {
                return comboSource;
            }
            set
            {
                comboSource = value;
                OnPropertyChanged("ComboItemsSource");
            }
        }
#if !WPF
        public bool DatePickerVisibility
        {
            get
            {
                return datapickervisibility;
            }
            set
            {
                datapickervisibility = value;
                OnPropertyChanged("DatePickerVisibility");
            }
        }
#endif
#if WPF
        public bool CasingButtonVisibility
        {
            get
            {
                return casingbuttonvisibility;
            }
            set
            {
                casingbuttonvisibility = value;
                OnPropertyChanged("CasingButtonVisibility");
            }
        }
#endif
        public bool IsORChecked
        {
            get { return isORChecked; }
            set { isORChecked = value; OnPropertyChanged("IsORChecked"); }
        }

        public bool IsCaseSensitive1
        {
            get { return isCaseSensitive1; }
            set
            {
                isCaseSensitive1 = value;
                RefreshCasingButton1State();
                OnPropertyChanged("IsCaseSensitive1");
            }
        }

        public bool IsCaseSensitive2
        {
            get { return isCaseSensitive2; }
            set
            {
                isCaseSensitive2 = value;
                RefreshCasingButton2State();
                OnPropertyChanged("IsCaseSensitive2");
            }
        }

        #endregion

        #region Dependency Properties

        #region CanGenerateUniqueItems
        /// <summary>
        /// DependencyProperty Registration for CanGenerateUniqueItems of AdvancedFilterControl
        /// </summary>
        /// <remarks></remarks>
        public static readonly DependencyProperty CanGenerateUniqueItemsProperty = DependencyProperty.Register(
          "CanGenerateUniqueItems", typeof(bool), typeof(AdvancedFilterControl), new PropertyMetadata(true, OnCanGenerateUniqueItemsChanged));
        private static void OnCanGenerateUniqueItemsChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
#if !WPF
            var advancedFilterControl = d as AdvancedFilterControl;
            if ((bool)e.NewValue == false && advancedFilterControl.gridFilterCtrl != null && advancedFilterControl.gridFilterCtrl.FilterColumnType == GridResourceWrapper.DateFilters)
                advancedFilterControl.DatePickerVisibility = false;
            else
                advancedFilterControl.DatePickerVisibility = true;
#endif
        }

        /// <summary>
        /// Gets or sets CanGenerateUniqueItems for AdvancedFilterControl.
        /// </summary>
        /// <value></value>
        /// <remarks></remarks>
        public bool CanGenerateUniqueItems
        {
            get { return (bool)this.GetValue(AdvancedFilterControl.CanGenerateUniqueItemsProperty); }
            set { this.SetValue(AdvancedFilterControl.CanGenerateUniqueItemsProperty, value); }
        }
        #endregion

        #region FilterTypeComboItems
        /// <summary>
        /// DependencyProperty Registration for FilterTypeCombo ItemsSource of AdvancedFilterControl
        /// </summary>
        /// <remarks></remarks>
        public static readonly DependencyProperty FilterTypeComboItemsProperty = DependencyProperty.Register(
          "FilterTypeComboItems", typeof(object), typeof(AdvancedFilterControl), new PropertyMetadata(null));

        /// <summary>
        /// Gets or sets ItemsSource for ComboBox.
        /// </summary>
        /// <value></value>
        /// <remarks></remarks>
        public object FilterTypeComboItems
        {
            get { return (object)this.GetValue(AdvancedFilterControl.FilterTypeComboItemsProperty); }
            set { this.SetValue(AdvancedFilterControl.FilterTypeComboItemsProperty, value); }
        }
        #endregion

        #endregion

        #region Ctor
        public AdvancedFilterControl()
        {
            this.DefaultStyleKey = typeof(AdvancedFilterControl);
        }

        #endregion

        #region UIElements

        ToggleButton CasingButton1;
        ToggleButton CasingButton2;
#if WPF||SILVERLIGHT
        DatePicker datePicker1;
        DatePicker datePicker2;
#else
        SfDatePicker datePicker1;
        SfDatePicker datePicker2;

#endif
        RadioButton radioButton1;
        RadioButton radioButton2;
#if WPF||SILVERLIGHT
        ComboBox MenuComboBox1;
        ComboBox MenuComboBox2;
#else
        SfComboBox MenuComboBox1;
        SfComboBox MenuComboBox2;
#endif
#if WinRT
        TextBox textBox1;
        TextBox textBox2;
#endif

        #endregion

        #region Overrides

#if WinRT
        protected override void OnApplyTemplate()
#else
        public override void OnApplyTemplate()
#endif
        {
            UnWireEvents();
            base.OnApplyTemplate();
            CasingButton1 = this.GetTemplateChild("PART_CasingButton1") as ToggleButton;
            CasingButton2 = this.GetTemplateChild("PART_CasingButton2") as ToggleButton;
#if WPF||SILVERLIGHT
            MenuComboBox1 = this.GetTemplateChild("PART_MenuComboBox1") as ComboBox;
            MenuComboBox2 = this.GetTemplateChild("PART_MenuComboBox2") as ComboBox;
#else
            MenuComboBox1 = this.GetTemplateChild("PART_MenuComboBox1") as SfComboBox;
            MenuComboBox2 = this.GetTemplateChild("PART_MenuComboBox2") as SfComboBox;
#endif
#if WPF||SILVERLIGHT
            datePicker1 = this.GetTemplateChild("PART_DatePicker1") as DatePicker;
            datePicker2 = this.GetTemplateChild("PART_DatePicker2") as DatePicker;
#else
            datePicker1 = this.GetTemplateChild("PART_DatePicker1") as SfDatePicker;
            datePicker2 = this.GetTemplateChild("PART_DatePicker2") as SfDatePicker;

#endif
            radioButton1 = this.GetTemplateChild("PART_RadioButton1") as RadioButton;
            radioButton2 = this.GetTemplateChild("PART_RadioButton2") as RadioButton;
#if WinRT
            textBox1 = this.GetTemplateChild("PART_TextBox1") as TextBox;
            textBox2 = this.GetTemplateChild("PART_TextBox2") as TextBox;
#endif
            WireEvents();
            GenerateFilterTypeComboItems();
            RefreshCasingButton1State();
            RefreshCasingButton2State();
        }
#if WPF
        protected override void OnMouseDown(System.Windows.Input.MouseButtonEventArgs e)
        {
            base.OnMouseDown(e);
            if (datePicker1 != null && datePicker1.IsDropDownOpen)
                datePicker1.IsDropDownOpen = false;
            if (datePicker2 != null && datePicker2.IsDropDownOpen)
                datePicker2.IsDropDownOpen = false;
        }
#endif

        public void Dispose()
        {
            UnWireEvents();
            this.gridFilterCtrl = null;
            if (this.ComboItemsSource != null)
            {
                this.ComboItemsSource.Clear();
                this.ComboItemsSource = null;
            }
            this.FilterTypeComboItems = null;
        }

        #endregion

        #region Methods
        private void ApplyImmediateFilters()
        {
            if (this.gridFilterCtrl == null || !this.gridFilterCtrl.ImmediateUpdateColumnFilter) return;
            var error1 = this["FilterValue1"];
            var error2 = this["FilterValue2"];
            if (!string.IsNullOrEmpty(error1) || !string.IsNullOrEmpty(error2))
                return;
            this.gridFilterCtrl.InvokeFilter();
        }
        public virtual object GetFirstFilterValue()
        {
            if (this.gridFilterCtrl != null && this.gridFilterCtrl.AdvancedFilterType == AdvancedFilterType.TextFilter && this.ColumnDataType != typeof(object))
#if WPF
                return FilterValue1;
#endif
#if SILVERLIGHT || WinRT
            {
                if (!this.CanGenerateUniqueItems)
                    return FilterValue1;
                if (FilterSelectedItem1 != null && FilterSelectedItem1 is FilterElement)
                    return (FilterSelectedItem1 as FilterElement).DisplayText;
            }
#endif
            else
            {
                // For datetime editing issue - after filtering
#if !WPF
                if (FilterSelectedItem1 != null && this.CanGenerateUniqueItems)
#else
                if (FilterSelectedItem1 is FilterElement &&
                    (FilterSelectedItem1 as FilterElement).DisplayText.Equals(FilterValue1))
#endif
                    return GetFilterElementValue(FilterSelectedItem1);
                else
                {
                    if (this.gridFilterCtrl != null &&
                        this.gridFilterCtrl.AdvancedFilterType == AdvancedFilterType.DateFilter)
                    {
#if WPF
                        if (CanGenerateUniqueItems && DateFilterValue1 != null)
                        {
                            if (this.gridFilterCtrl.GetFormatedString(DateFilterValue1).Equals(FilterValue1))
                                return DateFilterValue1;
                            else
                                return FilterValue1;
                        }
#else
                              if (CanGenerateUniqueItems && DateFilterValue1 != null)
                            return DateFilterValue1;
#endif
                        else
                        {
                            if (FilterValue1 != null &&
                                TypeConverterHelper.CanConvert(ColumnDataType, FilterValue1.ToString()))
                            {
                                return DateTime.Parse(FilterValue1.ToString());
                            }
                            return FilterValue1;
                        }
                    }
                    return FilterValue1;
                }
            }
#if SILVERLIGHT || WinRT
            return FilterValue1;
#endif
        }

        public virtual object GetSecondFilterValue()
        {
            if (this.gridFilterCtrl != null && this.gridFilterCtrl.AdvancedFilterType == AdvancedFilterType.TextFilter)
#if WPF
                return FilterValue2;

#else
            {
                if (FilterSelectedItem2 != null && FilterSelectedItem2 is FilterElement)
                    return (FilterSelectedItem2 as FilterElement).DisplayText;
            }
#endif
            else
            {
                // For datetime editing issue - after filtering
#if !WPF
                if (FilterSelectedItem2 != null && this.CanGenerateUniqueItems)
#else
                if (FilterSelectedItem2 is FilterElement &&
                    (FilterSelectedItem2 as FilterElement).DisplayText.Equals(FilterValue2))
#endif
                    return GetFilterElementValue(FilterSelectedItem2);
                else
                {
                    if (this.gridFilterCtrl != null &&
                        this.gridFilterCtrl.AdvancedFilterType == AdvancedFilterType.DateFilter)
                    {
#if WPF
                        if (CanGenerateUniqueItems && DateFilterValue2 != null)
                        {
                            if (this.gridFilterCtrl.GetFormatedString(DateFilterValue2).Equals(FilterValue2))
                                return DateFilterValue2;
                            else
                                return FilterValue2;      
                        }
#else
                              if (CanGenerateUniqueItems && DateFilterValue2 != null)
                            return DateFilterValue2;
#endif
                        else
                        {
                            if (FilterValue2 != null &&
                                TypeConverterHelper.CanConvert(ColumnDataType, FilterValue2.ToString()))
                            {
                                return DateTime.Parse(FilterValue2.ToString());
                            }
                            return FilterValue2;
                        }
                    }
                    return FilterValue2;
                }
            }
#if !WPF
            return FilterValue2;
#endif
        }

        internal void GenerateFilterTypeComboItems()
        {
            ObservableCollection<String> items = new ObservableCollection<string>();
            if (this.gridFilterCtrl.AdvancedFilterType == AdvancedFilterType.TextFilter)
            {
                VisualStateManager.GoToState(this, "TextFilter", true);
                items.Add(GridResourceWrapper.Equalss);
                items.Add(GridResourceWrapper.NotEquals);
#if !WPF
                if (!this.CanGenerateUniqueItems)
                {
#endif
                    items.Add(GridResourceWrapper.BeginsWith);
                    items.Add(GridResourceWrapper.EndsWith);
                    items.Add(GridResourceWrapper.Contains);
#if !WPF
                }
#endif
                items.Add(GridResourceWrapper.Empty);
                items.Add(GridResourceWrapper.NotEmpty);
                if (this.gridFilterCtrl == null || this.gridFilterCtrl.Column.AllowBlankFilters)
                {
                    items.Add(GridResourceWrapper.Null);
                    items.Add(GridResourceWrapper.NotNull);
                }
            }
            else if (this.gridFilterCtrl.AdvancedFilterType == AdvancedFilterType.NumberFilter)
            {
                VisualStateManager.GoToState(this, "NumberFilter", true);
                items.Add(GridResourceWrapper.Equalss);
                items.Add(GridResourceWrapper.NotEquals);
                if (this.gridFilterCtrl == null || this.gridFilterCtrl.Column.AllowBlankFilters)
                {
                    items.Add(GridResourceWrapper.Null);
                    items.Add(GridResourceWrapper.NotNull);
                }
                items.Add(GridResourceWrapper.LessThan);
                items.Add(GridResourceWrapper.LessThanorEqual);
                items.Add(GridResourceWrapper.GreaterThan);
                items.Add(GridResourceWrapper.GreaterThanorEqual);
            }
            else if (this.gridFilterCtrl.AdvancedFilterType == AdvancedFilterType.DateFilter)
            {
                VisualStateManager.GoToState(this, "DateFilter", true);
                items.Add(GridResourceWrapper.Equalss);
                items.Add(GridResourceWrapper.NotEquals);
                items.Add(GridResourceWrapper.Before);
                items.Add(GridResourceWrapper.BeforeOrEqual);
                items.Add(GridResourceWrapper.After);
                items.Add(GridResourceWrapper.AfterOrEqual);
                if (this.gridFilterCtrl == null || this.gridFilterCtrl.Column.AllowBlankFilters)
                {
                    items.Add(GridResourceWrapper.Null);
                    items.Add(GridResourceWrapper.NotNull);
                }
            }
            if (FilterTypeComboItems == null)
                FilterTypeComboItems = items;
            else
            {
                var hasblankfilters = (FilterTypeComboItems as ObservableCollection<string>).Any(s => s.Equals(GridResourceWrapper.Null));
                if (!this.gridFilterCtrl.Column.AllowBlankFilters && hasblankfilters)
                {
                    (FilterTypeComboItems as ObservableCollection<string>).Remove(GridResourceWrapper.Null);
                    (FilterTypeComboItems as ObservableCollection<string>).Remove(GridResourceWrapper.NotNull);
                }
                else if (this.gridFilterCtrl.Column.AllowBlankFilters && !hasblankfilters)
                {
                    (FilterTypeComboItems as ObservableCollection<string>).Add(GridResourceWrapper.Null);
                    (FilterTypeComboItems as ObservableCollection<string>).Add(GridResourceWrapper.NotNull);
                }
            }
        }

        internal void MaintainAPIChanges()
        {
            if (this.gridFilterCtrl.AdvancedFilterStyle != null)
                this.Style = this.gridFilterCtrl.AdvancedFilterStyle;
        }

        private object GetFilterElementValue(object filtervalue)
        {
            if (filtervalue != null)
            {
                if (filtervalue is FilterElement)
                    return (filtervalue as FilterElement).ActualValue;
                return filtervalue;
            }
            else return null;
        }

        private object GetFilterElementDisplayValue(object filtervalue)
        {
            if (filtervalue != null)
            {
                if (comboSource != null)
                {
#if WPF
                    var value = comboSource.FirstOrDefault(element => element != null && element.ActualValue != null && element.ActualValue.Equals(filtervalue));
                    if (value != null)
                        return value.DisplayText;
#endif
#if SILVERLIGHT || WinRT
                    if (this.gridFilterCtrl.AdvancedFilterType == AdvancedFilterType.TextFilter)
                    {
                        var value = comboSource.FirstOrDefault(element => element != null && element.ActualValue != null && element.DisplayText.Equals(filtervalue));
                        if (value != null)
                            return value;
                    }
                    else
                    {
                        var value = comboSource.FirstOrDefault(element => element != null && element.ActualValue != null && element.ActualValue.Equals(filtervalue));
                        // For Win RT datepicker issue filterElement is not supported(cast exception)
                        if (value != null && this.CanGenerateUniqueItems)
                            return value;
                    }
#endif
                    return filtervalue;
                }
            }
            return null;
        }

        internal void SetAdvancedFilterControlValues(ObservableCollection<FilterPredicate> fp)
        {
            propertyChangedfromsettingControlValues = true;
            if (comboSource == null)
            {
                if (fp.Count == 1)
                {
                    FilterType2 = GridResourceWrapper.Equalss;
                    FilterValue2 = null;
                    DateFilterValue2 = null;
                }
                propertyChangedfromsettingControlValues = false;
                return;
            }
#if !WinRT
            string emptyStringValue = string.Empty;
            if (this.gridFilterCtrl.Column is GridMaskColumn)
            {
                var column = this.gridFilterCtrl.Column as GridMaskColumn;
                emptyStringValue = MaskedEditorModel.GetMaskedText(column.Mask, string.Empty,
                    column.DateSeparator,
                    column.TimeSeparator,
                    column.DecimalSeparator,
                    NumberFormatInfo.CurrentInfo.NumberGroupSeparator,
                    column.PromptChar,
                    NumberFormatInfo.CurrentInfo.CurrencySymbol);
            }
#endif
            if (fp.Count > 0)
            {
#if WPF
                if (this.gridFilterCtrl != null &&
                    this.gridFilterCtrl.AdvancedFilterType == AdvancedFilterType.DateFilter)
#else
                 if (this.gridFilterCtrl != null &&
                    this.gridFilterCtrl.AdvancedFilterType == AdvancedFilterType.DateFilter && !CanGenerateUniqueItems)
#endif
                {
                    if(this.gridFilterCtrl!=null && fp[0].FilterValue!=null)
                    FilterValue1 = this.gridFilterCtrl.GetFormatedString(fp[0].FilterValue);
                }
                else
                    FilterValue1 = GetFilterElementDisplayValue(fp[0].FilterValue);
#if SILVERLIGHT || WinRT
                FilterSelectedItem1 = FilterValue1;
                if (FilterValue1 is FilterElement)
                    FilterValue1 = (FilterValue1 as FilterElement).DisplayText;
#endif
                FilterType1 = FilterHelpers.GetResourceWrapper(fp[0].FilterType, FilterValue1);
#if !WinRT
                if (this.gridFilterCtrl.Column is GridMaskColumn && FilterValue1 != null)
                {
                    if (FilterValue1.Equals(emptyStringValue))
                        FilterType1 = GridResourceWrapper.Empty;
                }
#endif
                IsCaseSensitive1 = fp[0].IsCaseSensitive;
                if (fp.Count == 1)
                {
                    FilterType2 = GridResourceWrapper.Equalss;
                    FilterValue2 = null;
                    DateFilterValue2 = null;
                }
            }
            if (fp.Count == 2)
            {
#if WPF
                if (this.gridFilterCtrl != null &&
                    this.gridFilterCtrl.AdvancedFilterType == AdvancedFilterType.DateFilter)
#else
                if (this.gridFilterCtrl != null &&
                   this.gridFilterCtrl.AdvancedFilterType == AdvancedFilterType.DateFilter && !CanGenerateUniqueItems)
#endif
                {
                    if (this.gridFilterCtrl != null && fp[1].FilterValue != null)
                        FilterValue2 = this.gridFilterCtrl.GetFormatedString(fp[1].FilterValue);
                }
                else
                    FilterValue2 = GetFilterElementDisplayValue(fp[1].FilterValue);
#if SILVERLIGHT || WinRT
                FilterSelectedItem2 = FilterValue2;
                if (FilterValue2 is FilterElement)
                    FilterValue2 = (FilterValue2 as FilterElement).DisplayText;
#endif
                FilterType2 = FilterHelpers.GetResourceWrapper(fp[1].FilterType, FilterValue2);
#if !WinRT
                if (this.gridFilterCtrl.Column is GridMaskColumn && FilterValue2 != null)
                {
                    if (FilterValue2.Equals(emptyStringValue))
                        FilterType2 = GridResourceWrapper.Empty;
                }
#endif
                IsCaseSensitive2 = fp[1].IsCaseSensitive;
                isORChecked = fp[1].PredicateType == PredicateType.Or ? true : false;
            }
            propertyChangedfromsettingControlValues = false;
        }

        internal void ResetAdvancedFilterControlValues()
        {
            propertyChangedfromsettingControlValues = true;
            FilterValue1 = null;
            FilterValue2 = null;

            DateFilterValue1 = null;
            DateFilterValue2 = null;

            FilterSelectedItem1 = null;
            FilterSelectedItem2 = null;
            FilterType1 = GridResourceWrapper.Equalss;
            FilterType2 = GridResourceWrapper.Equalss;
            IsORChecked = true;
            if (CasingButton1 != null)
            {
                IsCaseSensitive1 = false;
            }
            if (CasingButton2 != null)
            {
                IsCaseSensitive2 = false;
            }
            propertyChangedfromsettingControlValues = false;
        }

        #endregion

        #region Events
       
        private void RefreshCasingButton1State()
        {
            if (CasingButton1 == null)
                return;

            if (IsCaseSensitive1)
                VisualStateManager.GoToState(CasingButton1, "CaseSensitive", true);
            else
                VisualStateManager.GoToState(CasingButton1, "NotCaseSensitive", true);
        }

       
        private void RefreshCasingButton2State()
        {
            if (CasingButton2 == null)
                return;

            if (IsCaseSensitive2)
                VisualStateManager.GoToState(CasingButton2, "CaseSensitive", true);
            else
                VisualStateManager.GoToState(CasingButton2, "NotCaseSensitive", true);
        }

#if WinRT
        void OnTextBoxTextChanged(object sender, TextChangedEventArgs e)
        {
            var textBox = sender as TextBox;
            if (this.gridFilterCtrl.ImmediateUpdateColumnFilter)
            {
                if (textBox.Equals(textBox1))
                    FilterValue1 = textBox.Text;
                else
                    FilterValue2 = textBox.Text;
            }
            string error = string.Empty;
            if (!IsValidFilterValue(textBox.Text))
                error = GridResourceWrapper.EnterValidFilterValue;
            if (textBox.Equals(textBox1))
                ErrorMessage1 = error;
            else
                ErrorMessage2 = error;
            if (!string.IsNullOrEmpty(error))
                VisualStateManager.GoToState(textBox, "HasError", true);
            else
                VisualStateManager.GoToState(textBox, "NoError", true);
        }

#endif
        void OnRadioButtonClick(object sender, RoutedEventArgs e)
        {
#if SILVERLIGHT
            var radioButton = sender as RadioButton;
            if (radioButton.Name == "PART_RadioButton1")
                isORChecked = false;
            else
                IsORChecked = true;
#endif
            ApplyImmediateFilters();
        }
#if WPF
        private void OnDatePickerLostFocus(object sender, RoutedEventArgs e)
        {
            var datepicker = sender as DatePicker;
            if (datepicker != null && !datepicker.IsKeyboardFocusWithin)
                datepicker.IsDropDownOpen = false;
        }
        void OnDatePickerMouseDown(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            e.Handled = true;
        }
#endif

#if !WinRT
        void OnMenuComboBoxSelectionChanged(object sender, System.Windows.Controls.SelectionChangedEventArgs e)
#else
        void OnMenuComboBoxSelectionChanged(object sender, Windows.UI.Xaml.Controls.SelectionChangedEventArgs e)
#endif
        {
#if WinRT
            var comboBox = sender as SfComboBox;
#else
            var comboBox = sender as ComboBox;
#endif
            if (comboBox.SelectedValue != null && !propertyChangedfromsettingControlValues)
                ApplyImmediateFilters();
        }
        #region PropertyChanged

        public event PropertyChangedEventHandler PropertyChanged;

        public void OnPropertyChanged(string propertyName)
        {
#if WPF
            if ((propertyName == "FilterValue1" || propertyName == "FilterValue2") && !propertyChangedfromsettingControlValues)
                ApplyImmediateFilters();
#endif
#if SILVERLIGHT || WinRT
            if ((propertyName == "FilterSelectedItem1" || propertyName == "FilterSelectedItem2") && !propertyChangedfromsettingControlValues)
                ApplyImmediateFilters();

            if (!CanGenerateUniqueItems && (propertyName == "FilterValue1" || propertyName == "FilterValue2") && !propertyChangedfromsettingControlValues)
                ApplyImmediateFilters();
#endif
            if (PropertyChanged != null)
            {
                if (propertyName == "ComboItemsSource")
                {
                    propertyChangedfromsettingControlValues = true;
                    PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
                    propertyChangedfromsettingControlValues = false;
                }
                else
                    PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
            }
        }

        #endregion

        #region WireEvents

        private void WireEvents()
        {
            this.Loaded += AdvancedFilterControl_Loaded;
            if (MenuComboBox1 != null) MenuComboBox1.SelectionChanged += OnMenuComboBoxSelectionChanged;
            if (MenuComboBox2 != null) MenuComboBox2.SelectionChanged += OnMenuComboBoxSelectionChanged;
#if WPF
            if (datePicker1 != null)
            {
                datePicker1.LostFocus += OnDatePickerLostFocus;
                datePicker1.MouseDown += OnDatePickerMouseDown;
            }
            if (datePicker2 != null)
            {
                datePicker2.LostFocus += OnDatePickerLostFocus;
                datePicker2.MouseDown += OnDatePickerMouseDown;

            }
#endif
            if (radioButton1 != null) radioButton1.Click += OnRadioButtonClick;
            if (radioButton2 != null) radioButton2.Click += OnRadioButtonClick;
#if WinRT
            if (textBox1 != null)
                textBox1.TextChanged += OnTextBoxTextChanged;
            if (textBox2 != null)
                textBox2.TextChanged += OnTextBoxTextChanged;
#endif
        }

        void AdvancedFilterControl_Loaded(object sender, RoutedEventArgs e)
        {
            this.RefreshCasingButton1State();
            this.RefreshCasingButton2State();
        }


        #endregion

        #region UnwireEvents
        private void UnWireEvents()
        {
            this.Loaded -= AdvancedFilterControl_Loaded;
            if (MenuComboBox1 != null) MenuComboBox1.SelectionChanged -= OnMenuComboBoxSelectionChanged;
            if (MenuComboBox2 != null) MenuComboBox2.SelectionChanged -= OnMenuComboBoxSelectionChanged;
#if WPF
            if (datePicker1 != null)
            {
                datePicker1.LostFocus -= OnDatePickerLostFocus;
                datePicker1.MouseDown -= OnDatePickerMouseDown;
            }
            if (datePicker2 != null)
            {
                datePicker2.LostFocus -= OnDatePickerLostFocus;
                datePicker2.MouseDown -= OnDatePickerMouseDown;
            }
#endif
            if (radioButton1 != null) radioButton1.Click -= OnRadioButtonClick;
            if (radioButton2 != null) radioButton2.Click -= OnRadioButtonClick;
#if WinRT
            if (textBox1 != null)
                textBox1.TextChanged -= OnTextBoxTextChanged;
            if (textBox2 != null)
                textBox2.TextChanged -= OnTextBoxTextChanged;
#endif
        }
        #endregion
        #endregion

        #region IDataErrorInfo

        internal Type ColumnDataType;
        public string Error
        {
            get { return null; }
        }
#if WinRT
        private string errorMessage1 = string.Empty;
        public string ErrorMessage1
        {
            get
            {
                return errorMessage1;
            }
            set
            {
                errorMessage1 = value;
                OnPropertyChanged("ErrorMessage1");
            }
        }

        private string errorMessage2 = string.Empty;
        public string ErrorMessage2
        {
            get
            {
                return errorMessage2;
            }
            set
            {
                errorMessage2 = value;
                OnPropertyChanged("ErrorMessage2");
            }
        }
#endif
        public string this[string columnName]
        {
            get
            {
                string result = string.Empty;
                if (columnName == "FilterValue1")
                {
                    var value = GetFirstFilterValue();
                    if (!IsValidFilterValue(value))
                    {
#if WinRT
                        ErrorMessage1 = GridResourceWrapper.EnterValidFilterValue;
#endif
                        return GridResourceWrapper.EnterValidFilterValue;
                    }
                }
                else if (columnName == "FilterValue2")
                {
                    var value = GetSecondFilterValue();
                    if (!IsValidFilterValue(value))
                    {
#if WinRT
                        ErrorMessage2 = GridResourceWrapper.EnterValidFilterValue;
#endif
                        return GridResourceWrapper.EnterValidFilterValue;
                    }
                }
                return result;
            }
        }

        private bool IsValidFilterValue(object value)
        {
            if (ColumnDataType == typeof(object) || this.gridFilterCtrl != null && this.gridFilterCtrl.AdvancedFilterType == AdvancedFilterType.TextFilter)
                return true;
            if (value != null && !string.IsNullOrEmpty(value.ToString()))
                return TypeConverterHelper.CanConvert(ColumnDataType, value.ToString());
            return true;
        }
        #endregion
    }
}
