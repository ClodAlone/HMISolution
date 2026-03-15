#region Copyright
// Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Any infringement will be prosecuted under
// applicable laws. 
#endregion

namespace Syncfusion.Windows.Controls.Grid
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Windows;
    using Syncfusion.Linq;
    using System.Windows.Data;
    using System.Windows.Controls;
    using System.ComponentModel;
    using System.Windows.Controls.Primitives;
    using Syncfusion.Windows.Data;
    using Syncfusion.Windows.Tools.Controls;
    using System.Windows.Media;

    public class GridDataFilterWrapper : DependencyObject
    {
        #region Dependency Properties

#if !SILVERLIGHT
        internal static readonly DependencyProperty BackgroundProperty = DependencyProperty.Register("Background", typeof(Brush), typeof(GridDataFilterWrapper));
        internal static readonly DependencyProperty ForegroundProperty = DependencyProperty.Register("Foreground", typeof(Brush), typeof(GridDataFilterWrapper));
        public static readonly DependencyProperty FilterValueProperty = DependencyProperty.Register("FilterValue", typeof(object), typeof(GridDataFilterWrapper), new FrameworkPropertyMetadata(null, OnFilterValueChanged));
        public static readonly DependencyProperty StartDateFilterValueProperty = DependencyProperty.Register("StartDateFilterValue", typeof(DateTime?), typeof(GridDataFilterWrapper), new FrameworkPropertyMetadata(null, OnStartDateFilterValueChanged));
        internal static readonly DependencyProperty SelectedComboValueProperty = DependencyProperty.Register("SelectedComboValue", typeof(object), typeof(GridDataFilterWrapper), new FrameworkPropertyMetadata(null, OnSelectedComboValueChanged));
        public static readonly DependencyProperty FilterTypeProperty = DependencyProperty.Register("FilterType", typeof(FilterType), typeof(GridDataFilterWrapper), new PropertyMetadata(FilterType.Undefined));
        public static readonly DependencyProperty PredicateTypeProperty = DependencyProperty.Register("PredicateType", typeof(PredicateType), typeof(GridDataFilterWrapper), new FrameworkPropertyMetadata(PredicateType.Or));
        public static readonly DependencyProperty MatchCaseProperty = DependencyProperty.Register("MatchCase", typeof(bool), typeof(GridDataFilterWrapper), new FrameworkPropertyMetadata(true));
        public static readonly DependencyProperty EndDateFilterValueProperty = DependencyProperty.Register("EndDateFilterValue", typeof(DateTime?), typeof(GridDataFilterWrapper), new FrameworkPropertyMetadata(null, OnEndDateFilterValueChanged));
#else
        internal static readonly DependencyProperty BackgroundProperty = DependencyProperty.Register("Background", typeof(Brush), typeof(GridDataFilterWrapper), new PropertyMetadata(null));
        internal static readonly DependencyProperty ForegroundProperty = DependencyProperty.Register("Foreground", typeof(Brush), typeof(GridDataFilterWrapper), new PropertyMetadata(null));
        public static readonly DependencyProperty FilterValueProperty = DependencyProperty.Register("FilterValue", typeof(object), typeof(GridDataFilterWrapper), new PropertyMetadata(null, OnFilterValueChanged));
        public static readonly DependencyProperty StartDateFilterValueProperty = DependencyProperty.Register("StartDateFilterValue", typeof(DateTime?), typeof(GridDataFilterWrapper), new PropertyMetadata(null, OnStartDateFilterValueChanged));
        internal static readonly DependencyProperty SelectedComboValueProperty = DependencyProperty.Register("SelectedComboValue", typeof(object), typeof(GridDataFilterWrapper), new PropertyMetadata(null, OnSelectedComboValueChanged));
        public static readonly DependencyProperty FilterTypeProperty = DependencyProperty.Register("FilterType", typeof(FilterType), typeof(GridDataFilterWrapper), new PropertyMetadata(FilterType.Undefined));
        public static readonly DependencyProperty PredicateTypeProperty = DependencyProperty.Register("PredicateType", typeof(PredicateType), typeof(GridDataFilterWrapper), new PropertyMetadata(PredicateType.Or));
        public static readonly DependencyProperty MatchCaseProperty = DependencyProperty.Register("MatchCase", typeof(bool), typeof(GridDataFilterWrapper), new PropertyMetadata(true));
        public static readonly DependencyProperty EndDateFilterValueProperty = DependencyProperty.Register("EndDateFilterValue", typeof(DateTime?), typeof(GridDataFilterWrapper), new PropertyMetadata(null, OnEndDateFilterValueChanged));
#endif
        /// <summary>
        /// Gets or sets the background.
        /// </summary>
        /// <value>The background.</value>
        internal Brush Background
        {
            get { return (Brush)this.GetValue(GridDataFilterWrapper.BackgroundProperty); }
            set { this.SetValue(GridDataFilterWrapper.BackgroundProperty, value); }
        }

        

        /// <summary>
        /// Gets or sets the foreground.
        /// </summary>
        /// <value>The foreground.</value>
        internal Brush Foreground
        {
            get { return (Brush)this.GetValue(GridDataFilterWrapper.ForegroundProperty); }
            set { this.SetValue(GridDataFilterWrapper.ForegroundProperty, value); }
        }

        

        /// <summary>
        /// The Current FilterValue that is used to bind to the internal Filters for each column. If this is set to NULL, then the filter would be cleared.
        /// </summary>
        public object FilterValue
        {
            get { return this.GetValue(GridDataFilterWrapper.FilterValueProperty); }
            set { this.SetValue(GridDataFilterWrapper.FilterValueProperty, value); }
        }

        

        /// <summary>
        /// The Current FilterValue that is used to bind to the internal Filters for each column. If this is set to NULL, then the filter would be cleared.
        /// </summary>
        public DateTime? StartDateFilterValue
        {
            get { return (DateTime?)this.GetValue(GridDataFilterWrapper.StartDateFilterValueProperty); }
            set { this.SetValue(GridDataFilterWrapper.StartDateFilterValueProperty, value); }
        }

        

        /// <summary>
        /// The Current FilterValue that is used to bind to the internal Filters for each column. If this is set to NULL, then the filter would be cleared.
        /// </summary>
        public DateTime? EndDateFilterValue
        {
            get { return (DateTime?)this.GetValue(GridDataFilterWrapper.EndDateFilterValueProperty); }
            set { this.SetValue(GridDataFilterWrapper.EndDateFilterValueProperty, value); }
        }

        

        /// <summary>
        /// The Current FilterValue that is used to bind to the internal Filters for each column. If this is set to NULL, then the filter would be cleared.
        /// </summary>
        internal object SelectedComboValue
        {
            get { return this.GetValue(GridDataFilterWrapper.SelectedComboValueProperty); }
            set { this.SetValue(GridDataFilterWrapper.SelectedComboValueProperty, value); }
        }

        

        /// <summary>
        /// Gets or sets the type of the filter.
        /// </summary>
        /// <value>The type of the filter.</value>
        public FilterType FilterType
        {
            get { return (FilterType)this.GetValue(GridDataFilterWrapper.FilterTypeProperty); }
            set { this.SetValue(GridDataFilterWrapper.FilterTypeProperty, value); }
        }
        

        /// <summary>
        /// Gets or sets the type of the predicate.
        /// </summary>
        /// <value>The type of the predicate.</value>
        public PredicateType PredicateType
        {
            get { return (PredicateType)this.GetValue(GridDataFilterWrapper.PredicateTypeProperty); }
            set { this.SetValue(GridDataFilterWrapper.PredicateTypeProperty, value); }
        }
        

        /// <summary>
        /// Gets or sets a value indicating whether [match case].
        /// </summary>
        /// <value><c>true</c> if [match case]; otherwise, <c>false</c>.</value>
        public bool MatchCase
        {
            get { return (bool)this.GetValue(GridDataFilterWrapper.MatchCaseProperty); }
            set { this.SetValue(GridDataFilterWrapper.MatchCaseProperty, value); }
        }
        #endregion

        #region Dependency Properties Changed Events

        /// <summary>
        /// Called when [filter value changed].
        /// </summary>
        /// <param name="d">The d.</param>
        /// <param name="args">The <see cref="System.Windows.DependencyPropertyChangedEventArgs"/> instance containing the event data.</param>
        private static void OnFilterValueChanged(DependencyObject d, DependencyPropertyChangedEventArgs args)
        {
            var filterWrapper = d as GridDataFilterWrapper;
            filterWrapper.ApplyFilter(args.NewValue);
        }

        /// <summary>
        /// Called when [start date filter value changed].
        /// </summary>
        /// <param name="d">The d.</param>
        /// <param name="args">The <see cref="System.Windows.DependencyPropertyChangedEventArgs"/> instance containing the event data.</param>
        private static void OnStartDateFilterValueChanged(DependencyObject d, DependencyPropertyChangedEventArgs args)
        {
            var filterWrapper = d as GridDataFilterWrapper;
            if (filterWrapper != null && filterWrapper.FilterType == FilterType.Between && filterWrapper.VisibleColumn.Filters.Count == 0)
            {
                return;
            }
            else if (filterWrapper != null && filterWrapper.FilterType == FilterType.Between && filterWrapper.VisibleColumn.Filters.Count > 0)
                filterWrapper.ApplyFilter(args.NewValue, filterWrapper.EndDateFilterValue);
            else
                filterWrapper.ApplyFilter(args.NewValue);
            
        }

        /// <summary>
        /// Called when [end date filter value changed].
        /// </summary>
        /// <param name="d">The d.</param>
        /// <param name="args">The <see cref="System.Windows.DependencyPropertyChangedEventArgs"/> instance containing the event data.</param>
        private static void OnEndDateFilterValueChanged(DependencyObject d, DependencyPropertyChangedEventArgs args)
        {
            var filterWrapper = d as GridDataFilterWrapper;


            if (filterWrapper != null && filterWrapper.StartDateFilterValue != null && filterWrapper.FilterType == FilterType.Between)
            {
                filterWrapper.ApplyFilter(filterWrapper.StartDateFilterValue, args.NewValue);
            }
            else
                return;
                

        }

        private static void OnSelectedComboValueChanged(DependencyObject d, DependencyPropertyChangedEventArgs args)
        {
            var filterWrapper = d as GridDataFilterWrapper;
            filterWrapper.ComboSelectionChagedMethod(args.NewValue, filterWrapper);
        }

        #endregion

        #region Methods

        /// <summary>
        /// Applies the filter.
        /// </summary>
        /// <param name="filterValue">The filter value.</param>
        private void ApplyFilter(object filterValue)
        {
            if (!this.IsInSuspend)
            {
                var tableModel = this.VisibleColumn.TableModel;
                if (tableModel != null)
                {
                    var filterType = this.FilterType;
                    if (filterType != Linq.FilterType.Undefined)
                        tableModel.FilterColumn(this.VisibleColumn, filterValue != null && filterValue.ToString() != string.Empty ? filterValue : null, filterType, this.PredicateType, this.MatchCase, true);
                }
            }
        }

        /// <summary>
        /// Applies the filter.
        /// </summary>
        /// <param name="startFilterValue">The start filter value.</param>
        /// <param name="endFilterValue">The end filter value.</param>
        private void ApplyFilter(object startFilterValue, object endFilterValue)
        {
            if (!this.IsInSuspend)
            {
                var tableModel = this.VisibleColumn.TableModel;
                if (tableModel != null)
                {
                    var filterType = this.FilterType;
                    if (filterType == Linq.FilterType.Between)
                    {
                        var predicate = new List<FilterPredicate>();
                        predicate.Add(new FilterPredicate()
                        {
                            FilterBehavior = FilterBehavior.StringTyped,
                            FilterType = Linq.FilterType.GreaterThanOrEqual,
                            FilterValue = startFilterValue,
                            IsCaseSensitive = this.MatchCase,
                            PredicateType = PredicateType.And
                        });
                        predicate.Add(new FilterPredicate()
                        {
                            FilterBehavior = FilterBehavior.StringTyped,
                            FilterType = Linq.FilterType.LessThanOrEqual,
                            FilterValue = endFilterValue,
                            IsCaseSensitive = this.MatchCase,
                            PredicateType = PredicateType.And
                        });
                        tableModel.FilterColumn(this.VisibleColumn, predicate, true);
                    }

                }
            }
        }

        /// <summary>
        /// Refreshes the filter.
        /// </summary>
        internal void RefreshFilter()
        {
            if (!this.IsInSuspend && this.VisibleColumn != null)
            {
                var tableModel = this.VisibleColumn.TableModel;
                var filterValue = this.FilterValue;
                var startFilterValue = this.StartDateFilterValue;
                var endFilterValue = this.EndDateFilterValue;
                var viscol = this.VisibleColumn;
                if (filterValue != null && tableModel != null && !string.IsNullOrEmpty(filterValue.ToString()))
                {
                    tableModel.FilterColumn(viscol, filterValue, this.FilterType, this.PredicateType, this.MatchCase, true);
                }
                else if (startFilterValue != null && endFilterValue != null && this.FilterType == Linq.FilterType.Between && tableModel != null && !string.IsNullOrEmpty(startFilterValue.ToString()) && !string.IsNullOrEmpty(endFilterValue.ToString()))
                {
                    tableModel.FilterColumn(viscol, viscol.Filters.ToList(), true);
                }
                else if (startFilterValue != null && this.FilterType != Linq.FilterType.Between && tableModel != null && !string.IsNullOrEmpty(startFilterValue.ToString()))
                {
                    //Unwantedly removing the filters while changing the filtertype.
                    //viscol.Filters.Clear();
                    tableModel.FilterColumn(viscol, startFilterValue, this.FilterType, this.PredicateType, this.MatchCase, true);
                }
            }
        }

        /// <summary>
        /// Sets the visible column.
        /// </summary>
        /// <param name="column">The column.</param>
        internal void SetVisibleColumn(GridDataVisibleColumn column)
        {
            this.VisibleColumn = column;
        }

        public void ComboSelectionChagedMethod(object newValue, GridDataFilterWrapper filterWrapper)
        {
            var currentFilterType = Linq.FilterType.Undefined;
            if (newValue != null && filterWrapper != null)
            {
                if (filterWrapper.VisibleColumn.ColumnType == typeof(System.DateTime))
                {
                    string filterType = newValue.ToString();
                    if (filterType == GridDataResourceWrapper.AdvanceFilteringEqualsString) 
                        currentFilterType = FilterType.Equals;
                    else if (filterType == GridDataResourceWrapper.NotEquals)
                        currentFilterType = FilterType.NotEquals;
                    else if (filterType == GridDataResourceWrapper.AdvanceFilteringGreaterThanString)
                        currentFilterType = FilterType.GreaterThan;
                    else if (filterType == GridDataResourceWrapper.GreaterThanOrEqual)
                        currentFilterType = FilterType.GreaterThanOrEqual;
                    else if (filterType == GridDataResourceWrapper.LessThan)
                        currentFilterType = FilterType.LessThan;
                    else if (filterType == GridDataResourceWrapper.LessThanOrEqual)
                        currentFilterType = FilterType.LessThanOrEqual;
                    else if (filterType == GridDataResourceWrapper.AdvnaceFilteringBetweenString)
                        currentFilterType = FilterType.Between;
                    
                }
                else if (filterWrapper.VisibleColumn.FilterPane is GridDataInt32SliderFilteringPane)
                {
                    var intFilterPane = filterWrapper.VisibleColumn.FilterPane as GridDataInt32SliderFilteringPane;
                    if (newValue.ToString().Equals("None"))
                    {
                        if (filterWrapper != null)
                            filterWrapper.FilterValue = null;
                        if (intFilterPane.FilterSlider != null && intFilterPane.FilterSlider.IsEnabled)
                        {
                            intFilterPane.FilterSlider.IsEnabled = false;
                        }
                        return;
                    }

                    if (intFilterPane.FilterSlider != null && !intFilterPane.FilterSlider.IsEnabled)
                    {
                        intFilterPane.FilterSlider.IsEnabled = true;
                    }

                   
                    string filterType = newValue.ToString();
                    if (filterType == GridDataResourceWrapper.AdvanceFilteringEqualsString)
                        currentFilterType = FilterType.Equals;
                    else if (filterType == GridDataResourceWrapper.NotEquals)
                        currentFilterType = FilterType.NotEquals;
                    else if (filterType == GridDataResourceWrapper.AdvanceFilteringGreaterThanString)
                        currentFilterType = FilterType.GreaterThan;
                    else if (filterType == GridDataResourceWrapper.GreaterThanOrEqual)
                        currentFilterType = FilterType.GreaterThanOrEqual;
                    else if (filterType == GridDataResourceWrapper.LessThan)
                        currentFilterType = FilterType.LessThan;
                    else if (filterType == GridDataResourceWrapper.LessThanOrEqual)
                        currentFilterType = FilterType.LessThanOrEqual;
                    

                    var wrapperInstance = intFilterPane.GetFilterWrapper();
                    if (intFilterPane.FilterSlider != null)
                        wrapperInstance.FilterValue = intFilterPane.FilterSlider.Value;
                    wrapperInstance.FilterType = currentFilterType;
                    wrapperInstance.FilterType = currentFilterType;
                    wrapperInstance.PredicateType = intFilterPane.PredicateType;

                }

                else if (!(this.VisibleColumn.ColumnType == typeof(string)))
                {
                    

                    string filterType = newValue.ToString();
                    if (filterType == GridDataResourceWrapper.AdvanceFilteringEqualsString)
                        currentFilterType= FilterType.Equals;
                    else if (filterType == GridDataResourceWrapper.NotEquals)
                        currentFilterType = FilterType.NotEquals;
                    else if (filterType == GridDataResourceWrapper.GreaterThanOrEqual)
                        currentFilterType= FilterType.GreaterThanOrEqual;
                    else if (filterType == GridDataResourceWrapper.AdvanceFilteringGreaterThanString)
                        currentFilterType= FilterType.GreaterThan;
                    else if (filterType == GridDataResourceWrapper.LessThan)
                        currentFilterType= FilterType.LessThan;
                    else if (filterType == GridDataResourceWrapper.LessThanOrEqual)
                        currentFilterType = FilterType.LessThanOrEqual;
                    
                   
                }

                else
                {


                    string filterType = newValue.ToString();
                    if (filterType == GridDataResourceWrapper.AdvanceFilteringEqualsString)
                        currentFilterType = FilterType.Equals;
                    else if (filterType == GridDataResourceWrapper.NotEquals)
                        currentFilterType = FilterType.NotEquals;
                    else if (filterType == GridDataResourceWrapper.StartsWith)
                        currentFilterType = FilterType.StartsWith;
                    else if (filterType == GridDataResourceWrapper.EndsWith)
                        currentFilterType = FilterType.EndsWith;
                    else if (filterType == GridDataResourceWrapper.AdvanceFilteringContainsString)
                        currentFilterType = FilterType.Contains;
                    
                    
                }
                filterWrapper.VisibleColumn.FilterPane.CurrentFilterType = currentFilterType;
                filterWrapper.VisibleColumn.FilterPane.OnPopupInvoked();
            }
        }

        #endregion

        #region Properties

        /// <summary>
        /// Gets or sets the visible column.
        /// </summary>
        /// <value>The visible column.</value>
        internal GridDataVisibleColumn VisibleColumn
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets a value indicating whether this instance is in suspend.
        /// </summary>
        /// <value>
        /// 	<c>true</c> if this instance is in suspend; otherwise, <c>false</c>.
        /// </value>
        public bool IsInSuspend
        {
            get;
            set;
        }

        #endregion

    }

}