#region Copyright Syncfusion Inc. 2001 - 2014
// Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Any infringement will be prosecuted under
// applicable laws. 
#endregion
using System;
using System.Collections.Generic;
using System.Text;

namespace Syncfusion.XlsIO.Implementation.PivotTables
{
    class PivotFilterCollections: IPivotFilters
    {
        List<PivotFilterImpl> m_pivotFilterImpl = new List<PivotFilterImpl>();

        /// <summary>
        /// Pivot value (or) Label filter
        /// </summary>
        private IPivotValueLableFilter m_valueFilter;

        /// <summary>
        /// 
        /// </summary>
        private IPivotField m_parent;

        /// <summary>
        /// Property for ValueLabel filter
        /// </summary>
        public IPivotValueLableFilter ValueFilter
        {
            get
            {
                return m_valueFilter;
            }
            set
            {
                m_valueFilter = value;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        public IPivotField Parent
        {
            get
            {
                return m_parent;
            }
            set
            {
                m_parent = value;
            }
        }

        public PivotFilterCollections(IPivotField field)
        {
            m_parent = field;
        }
        /// <summary>
        /// Adding the pivot value (or) label filter to pivot field
        /// </summary>
        /// <param name="filterType">Type of the pivot filter</param>
        /// <param name="dataField">Datafield to which pivot filter is applied</param>
        /// <param name="Value1">Value 1 of the pivot filter</param>
        /// <param name="Value2">value 2 of the pivot filter</param>
        /// <returns></returns>
        public IPivotValueLableFilter Add(PivotFilterType filterType, IPivotField dataField, string Value1, string Value2)
        {
            PivotFieldImpl field = Parent as PivotFieldImpl;
            PivotTableImpl m_table = field.m_table;
            m_table.Cache.IsRefreshOnLoad = true;
            field.IsMultiSelected = false;
            if (!(field.Axis != PivotAxisTypes.Row || field.Axis != PivotAxisTypes.Column))
            {
                throw new ArgumentException("Field must be Row based or Column Based");
            }

            if (filterType.ToString().Contains("value"))
            {
                if (filterType == PivotFilterType.ValueNotBetween || filterType == PivotFilterType.ValueBetween)
                {
                    if (!IsIntValue(Value2.ToString()))
                        throw new ArgumentException("Value2 must be Number");
                }
                if (!IsIntValue(Value1.ToString()))
                    throw new ArgumentException("Value1 must be Number");

                if (dataField == null)
                    throw new ArgumentException("DataField must have values");
            }
            PivotValueLableFilter valueFilter = new PivotValueLableFilter();
            valueFilter.Value1 = Value1;
            valueFilter.Value2 = Value2;
            valueFilter.DataField = dataField;
            valueFilter.Type = filterType;
            this.ValueFilter = valueFilter;
            PivotTableFields fields = m_table.Fields;
            int dataFieldIndex = 0;
            int iDataFieldIndexCount = 0;

            for (int fieldIndex = 0, fieldCount = m_table.Fields.Count; fieldIndex < fieldCount; fieldIndex++)
            {
                if (m_table.Fields[fieldIndex].IsDataField)
                {
                    if (fieldIndex == fields.IndexOf((PivotFieldImpl)dataField))
                        dataFieldIndex = iDataFieldIndexCount;
                    iDataFieldIndexCount++;
                }
            }

            PivotTableFilters pivotTableFilters = new PivotTableFilters();
            for (int filterIndex = 0, filterCount = m_table.Filters.Count; filterIndex < filterCount; filterIndex++)
            {
                if (m_table.Filters[filterIndex].Field == fields.IndexOf(field))
                {
                    m_table.Filters.Remove(m_table.Filters[filterIndex]);
                }
                pivotTableFilters = m_table.Filters;
            }

            Dictionary<int, PivotItemOptions> items = field.ItemOptions;
            if (items.Count > 0)
            {
                foreach (KeyValuePair<int, PivotItemOptions> item in items)
                {
                    if (item.Value != null)
                        item.Value.IsHidden = false;
                }
            }

            PivotTableFilter pivotFilter = new PivotTableFilter();
            pivotFilter.Type = filterType;
            pivotFilter.EvalOrder = -1;
            pivotFilter.FilterId = 2;
            if (dataField != null)
                pivotFilter.MeasureFld = dataFieldIndex;
            pivotFilter.Field = fields.IndexOf(field);
            pivotFilter.Value1 = Value1;
            if (Value2 != null)
                pivotFilter.Value2 = Value2;

            PivotAutoFilter autoFilter = new PivotAutoFilter();
            autoFilter.FilterRange = "A1";
            PivotFilterColumn filterColumn = new PivotFilterColumn();
            filterColumn.ColumnId = 0;
            if (filterType != PivotFilterType.Count)
            {
                PivotCustomFilters customFilters = new PivotCustomFilters();
                customFilters.HasAnd = false;
                if (filterType == PivotFilterType.CaptionBetween || filterType == PivotFilterType.ValueBetween)
                    customFilters.HasAnd = true;

                PivotCustomFilter customFilter = new PivotCustomFilter();

                if (!(filterType == PivotFilterType.CaptionNotBetween || filterType == PivotFilterType.CaptionBetween || filterType == PivotFilterType.ValueNotBetween || filterType == PivotFilterType.ValueBetween))
                    customFilter.FilterOperator = GetOperator(filterType);
                else if (filterType == PivotFilterType.CaptionBetween || filterType == PivotFilterType.ValueBetween)
                    customFilter.FilterOperator = FilterOperator2007.GreaterThanOrEqual;
                else if (filterType == PivotFilterType.CaptionNotBetween || filterType == PivotFilterType.ValueNotBetween)
                    customFilter.FilterOperator = FilterOperator2007.LessThan;

                customFilter.Value = GetValue(Value1, filterType);

                customFilters.Add(customFilter);
                if ((filterType == PivotFilterType.CaptionNotBetween || filterType == PivotFilterType.CaptionBetween) || filterType == PivotFilterType.ValueNotBetween || filterType == PivotFilterType.ValueBetween)
                {
                    PivotCustomFilter customFilter1 = new PivotCustomFilter();
                    if (filterType == PivotFilterType.CaptionBetween || filterType == PivotFilterType.ValueBetween)
                        customFilter1.FilterOperator = FilterOperator2007.LessThanOrEqual;
                    else if (filterType == PivotFilterType.CaptionNotBetween || filterType == PivotFilterType.ValueNotBetween)
                        customFilter.FilterOperator = FilterOperator2007.GreaterThan;
                    if (Value2 == null)
                        throw new ArgumentException("Value2 is not set");
                    customFilter1.Value = GetValue(Value2, filterType);
                    customFilters.Add(customFilter1);
                }
                filterColumn.CustomFilters = customFilters;
            }
            else
            {
                PivotTop10Filter top10Filter = new PivotTop10Filter();
                top10Filter.Value = Convert.ToDouble(Value1);
                top10Filter.FilterValue = top10Filter.Value;
                filterColumn.Top10Filters = top10Filter;
            }
            autoFilter.Add(filterColumn);
            pivotFilter.Add(autoFilter);
            pivotTableFilters.Add(pivotFilter);
            m_table.Filters = pivotTableFilters;

            return valueFilter;
        }

        /// <summary>
        /// Check whether values is Integer.
        /// </summary>
        /// <param name="Value"></param>
        /// <returns></returns>
        internal bool IsIntValue(string Value)
        {
            int tempIntValue;
            bool checkInteger = int.TryParse(Value.ToString(), out tempIntValue);
            return checkInteger;
        }
        /// <summary>
        /// Get operator method of pivot filter
        /// </summary>
        /// <param name="filterType"></param>
        /// <returns></returns>
        public FilterOperator2007 GetOperator(PivotFilterType filterType)
        {
            FilterOperator2007 filterOperator = FilterOperator2007.Equal;
            switch (filterType)
            {
                case PivotFilterType.CaptionNotEqual:
                case PivotFilterType.CaptionNotEndsWith:
                case PivotFilterType.CaptionNotBeginsWith:
                case PivotFilterType.CaptionNotContains:
                case PivotFilterType.ValueNotEqual:
                    filterOperator = FilterOperator2007.NotEqual;
                    break;
                case PivotFilterType.CaptionLessThanOrEqual:
                case PivotFilterType.ValueLessThanOrEqual:
                    filterOperator = FilterOperator2007.LessThanOrEqual;
                    break;
                case PivotFilterType.CaptionLessThan:
                case PivotFilterType.ValueLessThan:
                    filterOperator = FilterOperator2007.LessThan;
                    break;
                case PivotFilterType.CaptionGreaterThan:
                case PivotFilterType.ValueGreaterThan:
                    filterOperator = FilterOperator2007.GreaterThan;
                    break;
                case PivotFilterType.CaptionGreaterThanOrEqual:
                case PivotFilterType.ValueGreaterThanOrEqual:
                    filterOperator = FilterOperator2007.GreaterThanOrEqual;
                    break;
            }
            return filterOperator;
        }

        /// <summary>
        /// Get value method of pivot filter
        /// </summary>
        /// <param name="Value"></param>
        /// <returns></returns>
        public string GetValue(string Value, PivotFilterType filterType)
        {
            string changedValue = Value;
            string Symbol = "*";
            if (filterType == PivotFilterType.CaptionNotContains || filterType == PivotFilterType.CaptionContains)
                changedValue = Symbol + changedValue + Symbol;
            else if (filterType == PivotFilterType.CaptionNotEndsWith || filterType == PivotFilterType.CaptionEndsWith)
                changedValue = Symbol + changedValue;
            else if (filterType == PivotFilterType.CaptionBeginsWith || filterType == PivotFilterType.CaptionNotBeginsWith)
                changedValue = changedValue + Symbol;

            return changedValue;
        }

        public IPivotFilter Add()
        {
            PivotFilterImpl filterImpl = new PivotFilterImpl();
            filterImpl.Value1 = "";
            m_pivotFilterImpl.Add(filterImpl);
            return filterImpl;
        }



        public IPivotFilter this[int index]
        {
            get
            {
                if (m_pivotFilterImpl.Count > 0)
                    return m_pivotFilterImpl[index];
                else
                    return null;
            }
        }


        public void Remove(int index)
        {
            m_pivotFilterImpl.RemoveAt(index);
        }
    }
}
