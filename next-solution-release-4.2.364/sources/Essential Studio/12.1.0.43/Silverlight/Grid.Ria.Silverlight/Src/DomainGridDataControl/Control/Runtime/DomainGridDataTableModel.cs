#region Copyright
// Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Any infringement will be prosecuted under
// applicable laws. 
#endregion

namespace Syncfusion.Windows.Controls.Grid.Ria
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using System.ComponentModel;
    using System.Reflection;
    using System.Windows.Controls.Primitives;
    using System.Windows.Controls;
#if SyncfusionFramework3_5
    using System.Windows.Controls.Ria;
#else
    using System.Windows.Controls.Primitives;
#endif
    using System.Linq;
    using System.Linq.Expressions;
    using System.Windows;
    using System.Windows.Media;
    using Syncfusion.Linq;
    using Syncfusion.Linq.Data;
    using Syncfusion.Windows.Collections;
    using Syncfusion.Windows.Controls.Cells;
    using Syncfusion.Windows.Controls.Grid;
    using Syncfusion.Windows.Controls.Scroll;
    using System.Diagnostics;
    using System.Collections.Specialized;
    using System.Windows.Data;
    using System.Text;
    using Syncfusion.Windows.ComponentModel;
    using Syncfusion.Windows.Data;

    public class DomainGridDataTableModel : GridDataTableModel
    {
        public DomainGridDataTableModel()
            : base()
        {
        }

        private bool IsInRIAFilter
        {
            get;
            set;
        }

        public bool HasRIASource
        {
            get;
            set;
        }

        public DomainDataSource RIASource
        {
            get;
            internal set;
        }

        public new bool IsInFilter
        {
            get
            {
                return this.IsInFilterOverride;
            }
            set
            {
                this.IsInFilterOverride = value;
            }
        }

        public override ICollectionViewAdv CreateCollectionViewAdv(IEnumerable source)
        {
            ICollectionViewAdv view = null;
            if (source != null)
            {
                if (source is DomainDataSourceView)
                {
                    view = new DomainQueryableCollectionView((ICollectionView)source, this, (rec) =>
                    {
                        var record = new GridDataRecord(rec, this.Table);
                        return record;
                    });
                }
                else
                {
                    return base.CreateCollectionViewAdv(source);
                }

                if (this.TableProperties.CaptionSummaryRow != null)
                {
                    view.CaptionSummaryRow = this.TableProperties.CaptionSummaryRow;
                }
            }

            return view;
        }

        public override IEnumerable GetSourceList(object source)
        {
            IEnumerable result = null;
            if (source != null)
            {
                if (source is DomainDataSourceView)
                {
                    result = source as IEnumerable;
                }
                else
                {
                    return base.GetSourceList(source);
                }
            }

            return result;
        }

        protected override void SortColumnMethod(GridDataVisibleColumn column)
        {
            if (this.HasRIASource && !this.RIASource.CanLoad)
            {
                return;
            }

            base.SortColumnMethod(column);
        }


        protected override void FilterColumnMethod(GridDataVisibleColumn column, object filterValue, FilterType filterType, PredicateType predicateType, bool isCaseSensitive, bool canApplyFilter)
        {
            if (column == null)
            {
                return;
            }

            if (this.HasRIASource && !this.RIASource.CanLoad)
            {
                return;
            }

            this.IsInFilter = true;
            this.TableProperties.VisibleColumns.SuspendEvents();

            if (filterValue != null)
            {
                //GridDataFilterPredicate predicate = null;
                if (column.Filters.Count == 0)
                {
                    //predicate = new GridDataFilterPredicate() { FilterValue = filterValue, FilterType = filterType, PredicateType = predicateType, IsCaseSensitive = isCaseSensitive };
                    column.Filters.Add(new FilterPredicate() { FilterType = filterType, FilterValue = filterValue, PredicateType = predicateType, IsCaseSensitive = isCaseSensitive });
                }
                else
                {
                    // take the last predicate
                    var fpredicate = column.Filters[column.Filters.Count - 1];
                    fpredicate.FilterType = filterType;
                    fpredicate.FilterValue = filterValue;
                    fpredicate.PredicateType = predicateType;
                    fpredicate.IsCaseSensitive = isCaseSensitive;
                    //predicate = new GridDataFilterPredicate() { FilterType = fpredicate.FilterType, FilterValue = fpredicate.FilterValue, IsCaseSensitive = fpredicate.IsCaseSensitive, PredicateType = fpredicate.PredicateType };
                }
            }
            else
            {
                if (column.Filters.Count > 0)
                {
                    column.Filters.Clear();
                }
            }

            if (canApplyFilter)
            {


                this.IsInRIAFilter = true;
                if (filterValue != null && this.HasRIASource)
                {
                    FilterDescriptor previousDescriptor = null;
                    if (this.RIASource.FilterDescriptors.Count > 0)
                    {
                        previousDescriptor = this.RIASource.FilterDescriptors.Where(d => d.PropertyPath == column.MappingName).FirstOrDefault() as FilterDescriptor;
                    }

                    if (previousDescriptor == null)
                    {
                        var filter = new FilterDescriptor()
                        {
                            IsCaseSensitive = isCaseSensitive,
                            PropertyPath = column.MappingName,
                            Operator = (FilterOperator)filterType,
                        };

#if SyncfusionFramework3_5
                        filter.Value.Value = filterValue.ToString();
#else
                        filter.Value = filterValue.ToString();                        
#endif
                        this.RIASource.FilterDescriptors.Add(filter);
                    }
                    else
                    {
#if SyncfusionFramework3_5
                        previousDescriptor.Value.Value = filterValue.ToString();
#else
                        previousDescriptor.Value = filterValue.ToString();
#endif
                    }

#if SyncfusionFramework3_5
                    this.RIASource.FilterDescriptors.LogicalOperator = (FilterDescriptorLogicalOperator)predicateType;
#endif
                }
                else if (filterValue == null && this.HasRIASource)
                {
                    var previousDescriptor = this.RIASource.FilterDescriptors.Where(d => d.PropertyPath == column.MappingName).FirstOrDefault() as FilterDescriptor;
                    this.RIASource.FilterDescriptors.Remove(previousDescriptor);
                }
                else
                {
                    var filters = this.GetFilters();
                    this.View.FilterPredicates = filters;
                }

                this.RefreshDisplayMethod(true);
                var gridDataTableModel = (GridDataTableModel)this;

                var startIdx = GetResolveStartIndexBasedOnPosition();
                this.InvalidateCell(GridRangeInfo.Rows(startIdx, this.RowCount));
            }

            this.RefreshSourceListCountMethod();
            this.IsInFilter = false;
            this.TableProperties.VisibleColumns.ResumeEvents();
        }

        protected override Expression<Func<string, object, object>> GetUnboundExpressionFuncMethod(string propertyName)
        {
            return this.GetUnboundExpressionFunc(propertyName);
        }

        internal Expression<Func<string, object, object>> GetUnboundExpressionFunc(string propertyName)
        {
            return base.GetUnboundExpressionFuncMethod(propertyName);
        }

    }
}
