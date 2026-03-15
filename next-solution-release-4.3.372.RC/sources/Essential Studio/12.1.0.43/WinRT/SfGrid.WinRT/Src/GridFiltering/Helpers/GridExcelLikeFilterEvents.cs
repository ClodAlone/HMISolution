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
using Syncfusion.Data;
#if WinRT
using Windows.UI.Xaml;
#else
using System.Windows;
#endif


namespace Syncfusion.UI.Xaml.Grid
{
    #region OKButtonClick

    public delegate void OkButtonClickEventHandler(object sender, OkButtonClikEventArgs args);

    [ClassReference(IsReviewed = false)]
    public class OkButtonClikEventArgs : RoutedEventArgs
    {

        /// <summary>
        /// Initializes a new instance of the <see cref="OkButtonClikEventArgs"/> class.
        /// </summary>
        public OkButtonClikEventArgs()
        {
        }

        public OkButtonClikEventArgs(object source) :
            base()
        {
        }

        /// <summary>
        /// Gets or sets the record.
        /// </summary>
        /// <value>The record.</value>
        public IEnumerable<FilterElement> UnCheckedElements
        {
            get;
            internal set;
        }

        /// <summary>
        /// Gets or sets the edited values.
        /// </summary>
        /// <value>The edited values.</value>
        public IEnumerable<FilterElement> CheckedElements
        {
            get;
            internal set;
        }
        public object FilterValue
        {
            get;
            set;
        }
        /// <summary>
        /// Gets or sets the filter value1.
        /// </summary>
        /// <value>The filter value1.</value>
        public object FilterValue1
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the filter value2.
        /// </summary>
        /// <value>The filter value2.</value>
        public object FilterValue2
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the filter type1.
        /// </summary>
        /// <value>The filter type1.</value>
        public object FilterType1
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the filter type2.
        /// </summary>
        /// <value>The filter type2.</value>
        public object FilterType2
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the type of the predicate.
        /// </summary>
        /// <value>The type of the predicate.</value>
        public object PredicateType
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the ColumnType.
        /// </summary>
        /// <value>The type of the predicate.</value>
        public AdvancedFilterType ColumnType
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the ColumnType.
        /// </summary>
        /// <value>The type of the predicate.</value>
        public bool IsCaseSensitive1
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the ColumnType.
        /// </summary>
        /// <value>The type of the predicate.</value>
        public bool IsCaseSensitive2
        {
            get;
            set;
        }

    }

    #endregion

    #region PopupOpened

    public delegate void PopupOpenedEventHandler(object sender, PopupOpenedEventArgs args);

    [ClassReference(IsReviewed = false)]
    public class PopupOpenedEventArgs : RoutedEventArgs
    {

        /// <summary>
        /// Initializes a new instance of the <see cref="PopupOpenedEventArgs"/> class.
        /// </summary>
        public PopupOpenedEventArgs()
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="PopupOpenedEventArgs"/> class.
        /// </summary>
        /// <param name="routedEvent">The routed event identifier for this instance of the <see cref="T:System.Windows.RoutedEventArgs"/> class.</param>
        /// <param name="source">An alternate source that will be reported when the event is handled. This pre-populates the <see cref="P:System.Windows.RoutedEventArgs.Source"/> property.</param>
        public PopupOpenedEventArgs(RoutedEvent routedEvent, object source) :
            base()
        {
        }


        /// <summary>
        /// Gets or sets the record.
        /// </summary>
        /// <value>The record.</value>
        public IEnumerable<FilterElement> ItemsSource
        {
            get;
            set;
        }

    }
    #endregion

    #region OnFilterElementPropertyChanged

    public delegate void OnFilterElementPropertyChangedEventHandler(object sender, OnFilterElementPropertyChangedEventArgs args);

    [ClassReference(IsReviewed = false)]
    public class OnFilterElementPropertyChangedEventArgs : RoutedEventArgs
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="OnFilterElementPropertyChangedEventArgs"/> class.
        /// </summary>
        public OnFilterElementPropertyChangedEventArgs()
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="OnFilterElementPropertyChangedEventArgs"/> class.
        /// </summary>
        /// <param name="routedEvent">The routed event identifier for this instance of the <see cref="T:System.Windows.RoutedEventArgs"/> class.</param>
        /// <param name="source">An alternate source that will be reported when the event is handled. This pre-populates the <see cref="P:System.Windows.RoutedEventArgs.Source"/> property.</param>
        public OnFilterElementPropertyChangedEventArgs(RoutedEvent routedEvent, object source) :
            base()
        {
        }


        /// <summary>
        /// Gets or sets the filter element.
        /// </summary>
        /// <value>The filter element.</value>
        public FilterElement FilterElement
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the select all checked.
        /// </summary>
        /// <value>The select all checked.</value>
        public Nullable<bool> SelectAllChecked
        {
            get;
            set;
        }
    }
    #endregion

    #region SelectAllCheckBoxChecked

    public delegate void SelectAllCheckBoxCheckedEventHandler(object sender, SelectAllCheckBoxCheckedEventArgs args);

    [ClassReference(IsReviewed = false)]
    public class SelectAllCheckBoxCheckedEventArgs : RoutedEventArgs
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="SelectAllCheckBoxCheckedEventArgs"/> class.
        /// </summary>
        public SelectAllCheckBoxCheckedEventArgs()
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="SelectAllCheckBoxCheckedEventArgs"/> class.
        /// </summary>
        /// <param name="routedEvent">The routed event identifier for this instance of the <see cref="T:System.Windows.RoutedEventArgs"/> class.</param>
        /// <param name="source">An alternate source that will be reported when the event is handled. This pre-populates the <see cref="P:System.Windows.RoutedEventArgs.Source"/> property.</param>
        public SelectAllCheckBoxCheckedEventArgs(RoutedEvent routedEvent, object source) :
            base()
        {
        }


        /// <summary>
        /// Gets or sets the filter elements.
        /// </summary>
        /// <value>The filter elements.</value>
        public List<FilterElement> FilterElements
        {
            get;
            set;
        }
    }

    #endregion

    #region SelectAllCheckBoxUnChecked
    public delegate void SelectAllCheckBoxUnCheckedEventHandler(object sender, SelectAllCheckBoxUnCheckedEventArgs args);

    [ClassReference(IsReviewed = false)]
    public class SelectAllCheckBoxUnCheckedEventArgs : RoutedEventArgs
    {

        /// <summary>
        /// Initializes a new instance of the <see cref="SelectAllCheckBoxUnCheckedEventArgs"/> class.
        /// </summary>
        public SelectAllCheckBoxUnCheckedEventArgs()
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="SelectAllCheckBoxUnCheckedEventArgs"/> class.
        /// </summary>
        /// <param name="routedEvent">The routed event identifier for this instance of the <see cref="T:System.Windows.RoutedEventArgs"/> class.</param>
        /// <param name="source">An alternate source that will be reported when the event is handled. This pre-populates the <see cref="P:System.Windows.RoutedEventArgs.Source"/> property.</param>
        public SelectAllCheckBoxUnCheckedEventArgs(RoutedEvent routedEvent, object source) :
            base()
        {
        }

        /// <summary>
        /// Gets or sets the filter elements.
        /// </summary>
        /// <value>The filter elements.</value>
        public List<FilterElement> FilterElements
        {
            get;
            set;
        }
    }
    #endregion

    internal static class FilterHelpers
    {
        /// <summary>
        /// Gets the type of the predicate.
        /// </summary>
        /// <param name="filterType">Type of the filter.</param>
        /// <returns></returns>
        internal static PredicateType GetPredicateType(String filterType)
        {
            PredicateType predicate = (PredicateType)Enum.Parse(typeof(PredicateType), filterType, true);
            return predicate;
        }

        /// <summary>
        /// Gets the type of the filter.
        /// </summary>
        /// <param name="filterType">Type of the filter.</param>
        /// <returns></returns>
        internal static FilterType GetFilterType(String filterType)
        {
            if (filterType == GridResourceWrapper.Equalss)
                return FilterType.Equals;
            else if (filterType == GridResourceWrapper.NotEquals)
                return FilterType.NotEquals;
            else if (filterType == GridResourceWrapper.GreaterThan)
                return FilterType.GreaterThan;
            else if (filterType == GridResourceWrapper.GreaterThanorEqual)
                return FilterType.GreaterThanOrEqual;
            else if (filterType == GridResourceWrapper.LessThan)
                return FilterType.LessThan;
            else if (filterType == GridResourceWrapper.LessThanorEqual)
                return FilterType.LessThanOrEqual;
            else if (filterType == GridResourceWrapper.BeginsWith)
                return FilterType.StartsWith;
            else if (filterType == GridResourceWrapper.EndsWith)
                return FilterType.EndsWith;
            else if (filterType == GridResourceWrapper.Contains)
                return FilterType.Contains;
            else if (filterType == GridResourceWrapper.NotContains)
                return FilterType.Contains;
            else if (filterType == GridResourceWrapper.Before)
                return FilterType.LessThan;
            else if (filterType == GridResourceWrapper.BeforeOrEqual)
                return FilterType.LessThanOrEqual;
            else if (filterType == GridResourceWrapper.After)
                return FilterType.GreaterThan;
            else if (filterType == GridResourceWrapper.AfterOrEqual)
                return FilterType.GreaterThanOrEqual;
            else if (filterType == GridResourceWrapper.Empty)
                return FilterType.Equals;
            else if (filterType == GridResourceWrapper.NotEmpty)
                return FilterType.NotEquals;
            else if (filterType == GridResourceWrapper.Null)
                return FilterType.Equals;
            else if (filterType == GridResourceWrapper.NotNull)
                return FilterType.NotEquals;
            return FilterType.Equals;
        }

        internal static string GetResourceWrapper(FilterType filterType, object FilterValue)
        {
            if (filterType == FilterType.Equals && FilterValue == null)
                return GridResourceWrapper.Null;
            else if (filterType == FilterType.NotEquals && FilterValue == null)
                return GridResourceWrapper.NotNull;
            else if (filterType == FilterType.Equals && FilterValue.Equals(string.Empty))
                return GridResourceWrapper.Empty;
            else if (filterType == FilterType.NotEquals && FilterValue.Equals(string.Empty))
                return GridResourceWrapper.NotEmpty;
            else if (filterType == FilterType.NotEquals)
                return GridResourceWrapper.NotEquals;
            else if (filterType == FilterType.Equals)
                return GridResourceWrapper.Equalss;
            if (FilterValue != null && !string.IsNullOrEmpty(FilterValue.ToString()))
            {
                if (TypeConverterHelper.CanConvert(typeof(DateTime), FilterValue.ToString()))
                {
                    if (filterType == FilterType.GreaterThan)
                        return GridResourceWrapper.After;
                    else if (filterType == FilterType.GreaterThanOrEqual)
                        return GridResourceWrapper.AfterOrEqual;
                    else if (filterType == FilterType.LessThan)
                        return GridResourceWrapper.Before;
                    else if (filterType == FilterType.LessThanOrEqual)
                        return GridResourceWrapper.BeforeOrEqual;
                }
            }
            if (filterType == FilterType.GreaterThan)
                return GridResourceWrapper.GreaterThan;
            else if (filterType == FilterType.GreaterThanOrEqual)
                return GridResourceWrapper.GreaterThanorEqual;
            else if (filterType == FilterType.LessThan)
                return GridResourceWrapper.LessThan;
            else if (filterType == FilterType.LessThanOrEqual)
                return GridResourceWrapper.LessThanorEqual;
            else if (filterType == FilterType.StartsWith)
                return GridResourceWrapper.BeginsWith;
            else if (filterType == FilterType.EndsWith)
                return GridResourceWrapper.EndsWith;
            else if (filterType == FilterType.Contains)
                return GridResourceWrapper.Contains;
            return GridResourceWrapper.Equalss;
        }
    }
}
