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
using System.Collections;
using System.ComponentModel;
using System.Reflection;
using System.Threading;
using System.Globalization;
#if SILVERLIGHT
using Syncfusion.Windows.Data;
using System.Xml.Serialization;
using Syncfusion.Linq;
using System.Linq.Expressions;
#endif

#if XLSIO
using System.Data;
using System.Xml.Serialization;
#if XLSIO
using System.Linq.Expressions;
#endif
namespace Syncfusion.XlsIO.Implementation.PivotAnalysis
#elif !SILVERLIGHT
using System.Data;
using System.Xml.Serialization;
using System.Linq.Expressions;
#if !SyncfusionFramework3_5
using System.Threading.Tasks;
using Syncfusion.PivotAnalysis.Base;
using System.Collections.ObjectModel;
#endif
namespace Syncfusion.PivotAnalysis.Base
#else
namespace Syncfusion.PivotAnalysis.Base.Silverlight
#endif
{
	#region PivotEngine class

	/// <summary>
	/// This class encapsulates pivoting calculation support. To use it, you first populate the <see cref="PivotColumns"/> and 
	/// <see cref="PivotRows"/> collections to define the properties being pivoted. You then populate the <see cref="PivotCalculations"/>
	/// collection to define the values you would like to see populated.
	/// </summary>
	public class PivotEngine : INotifyPropertyChanged
	{
		#region properties
		private IndexEngine indexEngine;

		/// <summary>
		/// Gets or sets the Indexed engine.
		/// </summary>
		/// <remarks>This is way of access the <see cref="IndexEngine"/> object from the pivot engine class in order to perform the on demand process.</remarks>
		public IndexEngine IndexEngine
		{
			get { return indexEngine; }
			set { indexEngine = value; }
		}

		GetValueDelegate getValue;

		/// <summary>
		/// Gets or sets a <see cref="GetValueDelegate"/> delegate that returns a property value for a given object.
		/// </summary>
		/// <remarks>
		/// If this member is null, the IndexEngine will use reflection to obtain property values from objects in
		/// the <see cref="DataSource"/>. Note usually, a well written delegate can provide values more efficiently
		/// than using reflection. For performance improvements, you will want to provide a well written delegate for
		/// this property.
		/// </remarks>
		public GetValueDelegate GetValue
		{
			get { return getValue; }
			set { getValue = value; }
		}
		private bool useIndexedEngine = false;

		/// <summary>
		/// Gets or sets whether an optimized algorithm that relies on indexing the raw
		/// data should be used to compute the pivot information.
		/// </summary>
		/// Setting this property indicates that the PivotEngine should use an a newer way of computing
		/// the pivot information. This technique requires more memory but will work significantly quicker 
		/// for pivots that have a large number of cells. For smaller pivots, there is no gain in performance
		/// with this newer technique.
		/// <remarks>
		/// </remarks>
		public bool UseIndexedEngine
		{
			get { return useIndexedEngine; }
			set
			{
				if (useIndexedEngine != value)
				{
					useIndexedEngine = value;
					if (!useIndexedEngine)
					{
						indexEngine = null;
					}
					RaisePivotSchemaChangedEvent(new PivotSchemaChangedArgs() { ChangeHints = SchemaChangeHints.None });

				}
			}
		}

        private bool applyFormattedSummary;
        /// <summary>
        /// Gets or Sets the formatted total value for the subtotal/grandtotal cells
        /// </summary>
        public bool ApplyFormattedSummary
        {
            get { return applyFormattedSummary; }
            set { applyFormattedSummary = value; }
        }
        
		private bool isDataDynamic = false;

		/// <summary>
		/// Gets or sets whether the DataSource is a collection of dynamic objects supported in the .Net 4.0 framework.
		/// </summary>
		public bool IsDataDynamic
		{
			get { return isDataDynamic; }
			set { isDataDynamic = value; }
		}
       
		private bool showGrandTotals = true;

		/// <summary>
		/// Gets or sets whether grand total calculations should be completed by the engine.
		/// </summary>
		/// <remarks>
		/// The default value is true.
		/// </remarks>
		public bool ShowGrandTotals
		{
			get { return showGrandTotals; }
			set
			{
				showGrandTotals = value;
				CoverGrandTotalRanges();
				RaisePivotSchemaChangedEvent(new PivotSchemaChangedArgs() { ChangeHints = SchemaChangeHints.GrandTotalVisibility });

			}
		}
        private GridLayout gridLayout;

        /// <summary>
        /// Gets or sets whether sub totals calculations should be shown on top or not.
        /// </summary>
        /// <remarks>
        /// The default value is Normal.
        /// </remarks>
        [DefaultValue(GridLayout.Normal)]
        public GridLayout GridLayout
        {
            get { return gridLayout; }
            set
            {
                gridLayout = value;
                RaisePivotSchemaChangedEvent(new PivotSchemaChangedArgs());
            }
        }
        private bool showSingleCalculationHeader = false;

        /// <summary>
        /// Gets or sets whether a calculation header row is visible when there is a single calculation. The default is false. This property does not affect settings when UseIndexedEngine is true.
        /// </summary>
        public bool ShowSingleCalculationHeader
        {
            get { return showSingleCalculationHeader; }
            set { showSingleCalculationHeader = value; }
        }

        private bool IsCalculationHeaderVisible
        {
            get
            {
                return ShowSingleCalculationHeader || PivotCalculations.Count > 1;
            }
        }

		

		private bool _populationCompleted = false;
		private bool _enableOnDemandCalculations = false;
        private bool _enableLazyLoadOnDemandCalculations = false;

        /// <summary>
        /// When EnableOnDemandCalculations is true, setting this property to true, allows the UI to trigger the summary calculations
        /// during the application idle cycles.
        /// </summary>
        /// <remarks>
        /// This means that the summaries in the pipvot may be fully populated when the user does something like sort or filter a pivot item
        /// which avoids a delay. If EnableOnDemandCalculations is false, setting this property has no effect.
        /// </remarks>
        public bool EnableLazyLoadOnDemandCalculations
        {
            get { return _enableLazyLoadOnDemandCalculations; }
            set { _enableLazyLoadOnDemandCalculations = value; }
        }

		/// <summary>
		/// Gets of sets whether the calculations are postponed until the value is requested through the Indexer on PivotEngine. The default value is false.
		/// </summary>
		/// <remarks>
		/// The default calculation behavior is for all the value cells to be populated during the call to PivotEngine.Calculate. When EnableOnDemandCalculations
		/// is set true, then the calculations are postponed until there is a initial request for a calculated value. At that time, the calculation is completed
		/// and stored. Subsequent requests for a value return the stored value instead of redoing the calculations.
		/// 
		/// Setting the EnableOnDemandCalculations allows the initial display of a large pivot table as only the visible cells will need to be calculated. This
		/// speed up in initial display, does come at the cost of a slight degradation in first-time scrolling performance as the cells that are newly made visible
		/// through the scrolling require a calculation to be completed. This setting is not available when RowPivotsOnly is false.
		/// </remarks>
		public bool EnableOnDemandCalculations
		{
			get { return _enableOnDemandCalculations && !RowPivotsOnly && PivotCalculations.Count > 0 && (PivotColumns.Count > 0 || PivotRows.Count > 0); }
			set { _enableOnDemandCalculations = value; }
		}


		private bool _showNullAsBlank = true;

		/// <summary>
		/// Gets or sets whether the PivotGrid cell should display Null value as blank instead of 0(which is the default behavior)
		/// </summary>
		/// <remarks>
		/// The default value is false.
		/// </remarks>       
		public bool ShowNullAsBlank
		{
			get
			{
				return _showNullAsBlank;
			}
			set
			{
				_showNullAsBlank = value;
			}
		}

		private int rowCount;
		private int columnCount;

		/// <summary>
		/// Gets the number of rows in this pivot table.
		/// </summary>
		public int RowCount
		{
			get
			{
                int adjustment = 0;
				if (indexEngine != null)
				{
					if (!ShowGrandTotals)
                    {
                        adjustment = !ShowCalculationsAsColumns && PivotCalculations.Count > 1 ? PivotCalculations.Count : 1;
                        return indexEngine.RowCount - adjustment;
                    }

					return indexEngine.RowCount;
				}
				if (!ShowGrandTotals)
				{
					adjustment = !ShowCalculationsAsColumns && PivotCalculations.Count > 1 ? PivotCalculations.Count : 1;
				}
				//return rowCount - (ShowGrandTotals || PivotRows.Count == 0 ? 0 : 1); 
				return rowCount - adjustment;
			}
		}

		/// <summary>
		/// Gets the number of columns in this pivot table;
		/// </summary>
		public int ColumnCount
		{
			get
            {
                int adjustment = 0;
				if (indexEngine != null)
				{
					if (!ShowGrandTotals)
                    {
                        adjustment = ShowCalculationsAsColumns && PivotCalculations.Count > 1 ? PivotCalculations.Count : 1;
                        return indexEngine.ColumnCount - adjustment;
                    }

					return indexEngine.ColumnCount;
				}
				if (!ShowGrandTotals)
				{
					adjustment = ShowCalculationsAsColumns && PivotCalculations.Count > 1 ? PivotCalculations.Count : 1;
				}
				return columnCount - adjustment;
			}
			//  get { return columnCount - (ShowGrandTotals || PivotColumns.Count == 0 ? 0 : PivotCalculations.Count); }
		}

		List<CoveredCellRange> coveredRanges = null;
		/// <summary>
		/// Gets a list of <see cref="CoveredCellRange"/> objects that specify
		/// the cells that need to be covered.
		/// </summary>
		public List<CoveredCellRange> CoveredRanges
		{
			get
			{
				if (indexEngine != null)
					return indexEngine.CoveredRanges;

				if (coveredRanges == null)
					coveredRanges = new List<CoveredCellRange>();
				return coveredRanges;
			}

		}

		private FilterHelper filters = null;

		/// <summary>
		/// Gets a List holding <see cref="FilterExpression"/> values that
		/// specify any filters that need to be applied to this pivot table.
		/// </summary>
		public List<FilterExpression> Filters
		{
			get
			{
				if (filters == null)
				{
					filters = new FilterHelper();
				}
				return filters.filterExpressions;
			}
			set
			{
				if (filters == null)
				{
					filters = new FilterHelper();
				}
				if (value == null)
					filters.filterExpressions = new List<FilterExpression>();
				else
					filters.filterExpressions = value;
			}
		}

		/// <summary>
		/// Adds a <see cref="FilterExpression"/> to the pivot schema for this engine.
		/// </summary>
		/// <param name="item">FilterExpression</param>
		public void AddFilter(FilterExpression item)
		{
			if (this == null)
				return;
			if (this.DataSource != null)
			{
                 PropertyInfo[] pdCollection = null;
                if ((this.DataSource as IList).GetEnumerator().MoveNext())
                {
                    pdCollection = ((this.DataSource as IList)[0]).GetType().GetProperties();
                }
                else
                {
                    ItemType = (this.DataSource as IList).GetType().GetProperties()[1].PropertyType;
                    if (ItemType != null)
                    {
                        pdCollection = ItemType.GetProperties();
                    }
                }
#if SILVERLIGHT
				//var pdCollection = ((this.DataSource as IList)[0]).GetType().GetProperties();
				System.Reflection.PropertyInfo propertyDescriptor = pdCollection.Cast<System.Reflection.PropertyInfo>().Where(p => p.Name == item.DimensionName).FirstOrDefault();
                if (propertyDescriptor == null)
					return;
#else
                PropertyDescriptor propertyDescriptor = pdCollection.OfType<PropertyDescriptor>().Cast<PropertyDescriptor>().Where(p => p.Name == item.DimensionName).FirstOrDefault();
                if (propertyDescriptor == null && this.ItemProperties != null && this.ItemProperties.Count > 0)
                {
                    if (item.DimensionName != null)
                        propertyDescriptor = this.ItemProperties[item.DimensionName];
                    else
                        propertyDescriptor = this.ItemProperties[item.Name];
                }
                else if(propertyDescriptor == null)
                {
                    return;
                }
#endif
				
				itemCollection = new FilterItemsCollection() { FilterProperty = propertyDescriptor, Name = item.DimensionName, DisplayHeader = item.DimensionHeader };

				foreach (var item1 in this.DataSource as IList)
				{
					if (propertyDescriptor.PropertyType == typeof(int) ||
						propertyDescriptor.PropertyType == typeof(double) ||
						propertyDescriptor.PropertyType == typeof(float) ||
						propertyDescriptor.PropertyType == typeof(decimal) ||
						propertyDescriptor.PropertyType == typeof(short) ||
						propertyDescriptor.PropertyType == typeof(long) ||
						propertyDescriptor.PropertyType == typeof(DateTime))
#if !SILVERLIGHT
						itemCollection.AddIfUnique(new FilterItemElement { Key = propertyDescriptor.GetValue(item1).ToString() });
					else
						itemCollection.AddIfUnique(new FilterItemElement { Key = propertyDescriptor.GetValue(item1) as string });
#else
						itemCollection.AddIfUnique(new FilterItemElement { Key = propertyDescriptor.GetValue(item1, new object[] { }).ToString() });
					else
						itemCollection.AddIfUnique(new FilterItemElement { Key = propertyDescriptor.GetValue(item1, new object[] { }) as string });
#endif
				}
				itemCollection.Reverse();
			}
		}

		/// <summary>
		/// Inserts a <see cref="FilterExpression"/> to the pivot schema for this engine at a specified Index
		/// </summary>
		/// <param name="index">The index.</param>
		/// <param name="exp">The FilterExpression.</param>
		public void InsertFilter(int index, FilterExpression exp)
		{
			Filters.Insert(index, exp);
			RaisePivotSchemaChangedEvent(new PivotSchemaChangedArgs());
		}

		private Dictionary<string, SummaryBase> summaryLibrary;
		/// <summary>
		/// Gets a dictionary of SummaryBase computations available in this PivotEngine.
		/// </summary>
		public Dictionary<string, SummaryBase> SummaryLibrary
		{
			get
			{
				if (summaryLibrary == null)
				{
					summaryLibrary = new Dictionary<string, SummaryBase>();
					PopulateDefaultSummaryLibrary();
				}
				return summaryLibrary;
			}
		}

		private void PopulateDefaultSummaryLibrary()
		{
			SummaryLibrary.Clear();
			foreach (string name in PivotComputationInfo.GetComputationTypes())
			{
#if SILVERLIGHT
				SummaryLibrary.Add(name, PivotComputationInfo.GetSummaryInstance((SummaryType)Enum.Parse(typeof(SummaryType), name, false)));
#else
				SummaryLibrary.Add(name, PivotComputationInfo.GetSummaryInstance((SummaryType)Enum.Parse(typeof(SummaryType), name)));
#endif
			}
		}


		/// <summary>
		/// Removes a <see cref="FilterExpression"/> from the pivot schema for this engine.
		/// </summary>
		/// <param name="exp">The FIlterExpression</param>
		public void RemoveFilter(FilterExpression exp)
		{
			if (exp!=null)
			{
				Filters.Remove(exp);
#if !SILVERLIGHT

                if (DataSourceList is DataView)
                {
                    string filter = "";
                    foreach (FilterExpression f in Filters)
                    {
                        if (filter != "")
                        {
                            filter += " AND ";
                        }
                        filter += "(" + f.Expression + ")";
                    }
                    ((DataView)this.DataSourceList).RowFilter = filter;

                }

#endif
                RaisePivotSchemaChangedEvent(new PivotSchemaChangedArgs());
			}

			else
			{
				FilterExpression filterExpression = this.Filters.Where(q => q.Name == itemCollection.DisplayHeader).FirstOrDefault();
				if (filterExpression != null)
				{
					this.Filters.Remove(filterExpression);
				}
			}
		}

	  
		/// <summary>
		/// Clears all FilterExpressions from the pivot schema for this engine.
		/// </summary>
		public void ClearFilters()
		{
			Filters.Clear();
			RaisePivotSchemaChangedEvent(new PivotSchemaChangedArgs());
		}

		/// <summary>
		/// Finds a <see cref="FilterExpression"/> in <see cref="Filters"/> given its name.
		/// </summary>
		/// <param name="filterName">The name of the desired FilterExpression.</param>
		/// <returns>The FilterExpression.</returns>
		public FilterExpression FindFilterByName(string filterName)
		{
			return Filters.Where(x => x.Name == filterName).FirstOrDefault();
		}


		PivotCellInfos pivotValues;

		///Gets the underlying object collection represented in the PivotEngine. 
		public PivotCellInfos PivotValues
		{
			get { return pivotValues; }
		}

		#region calculation sorting

		private HashSet<PivotCellInfo> hiddenRowIndexes = null;
        /// <summary>
        /// Gets the Values of the different hidden rows
        /// </summary>
		public HashSet<PivotCellInfo> HiddenRowIndexes
		{
			get
			{
				if (hiddenRowIndexes == null)
					hiddenRowIndexes = new HashSet<PivotCellInfo>();
				return hiddenRowIndexes;
			}
			set
			{
				hiddenRowIndexes = value;
			}
		}
        /// <summary>
        /// Gets of sets the different hidden row groups
        /// </summary>
        public Dictionary<int, List<HiddenGroup>> HiddenPivotRowGroups { get; set; }
        /// <summary>
        /// Gets of sets the different hidden column groups
        /// </summary>
        public Dictionary<int, List<HiddenGroup>> HiddenPivotColumnGroups { get; set; }  

    
		bool rowPivotsOnly = false;

		/// <summary>
		/// Gets or sets whether column pivots will be used.
		/// </summary>
		/// <remarks>Turning this property on enables value column
		/// support such as sorting and filtering the value calculations 
		/// that is normally found in a flat grid.</remarks>
		public bool RowPivotsOnly
		{
			get { return rowPivotsOnly; }
			set { rowPivotsOnly = value; }
		}
		
        /// <summary>
        /// Class that holds properties which is used to handle grid level single sort as well as multi sort
        /// </summary>
		public class SortKeys
		{
            /// <summary>
            /// Gets or sets the row index
            /// </summary>
			public int Index { get; set; }
            /// <summary>
            /// Gets or sets the collection of row indexes
            /// </summary>
			public IComparable[] Keys { get; set; }
            /// <summary>
            /// Gets or sets the size of grid cell range
            /// </summary>
            public int BlockSize { get; set; }
		}
        /// <summary>
        /// Sets the flag for peforming sorting operation
        /// </summary>
        #pragma warning disable
		bool ExpanderSortflag = false;
        #pragma warning enable
        /// <summary>
		/// Gets the sort direction of a particular column even if the column
		/// has been moved from it initial position. This is only used when RowPivotsOnly is true.
		/// </summary>
		/// <param name="col"></param>
		/// <returns></returns>
		public ListSortDirection GetSortDirection(int col)
		{
			int loc = sortColIndexes.IndexOf(col);
			return loc > -1 ? sortDirs[loc] : ListSortDirection.Descending;
		}
        /// <summary>
        /// Clears all the sorted order in grid
        /// </summary>
		public void ClearSorts()
		{
			if (RowPivotsOnly)
			{
                calcSortDirection = ListSortDirection.Ascending;
             	sortColIndexes.Clear();
				sortDirs.Clear();
				if (sortKeys != null)
				{
					sortKeys.Clear();
					sortKeys = null;
				}
			}
			 
		}
        /// <summary>
        /// Clears the sort at the specified index.
        /// </summary>
        /// <param name="index"></param>
		public void ClearSortAt(int index)
		{
			sortColIndexes.Remove(index);
		}
        /// <summary>
        /// returns the sorted columns' count
        /// </summary>
        /// <returns></returns>
		public bool AnyValueColumnsSorted()
		{
			return sortColIndexes.Count > 0;
		}
		/// <summary>
		/// Returns the original position of the column in the PivotCalculations collection. 
		/// This is only used when RowPivotsOnly is true.
		/// </summary>
		/// <param name="columnIndex">The current position of the column in the display.</param>
		/// <returns>Returns the original position of the column as determined 
		/// by its position in the PivotCalculations collection.</returns>
		public int ResolveColumnIndex(int columnIndex)
		{
			return (RowPivotsOnly && columnIndexes != null && columnIndexes.Count > 0 && columnIndex < columnIndexes.Count) ? columnIndexes[columnIndex] : columnIndex;
		}

		/// <summary>
		/// Checks whether a column at the index in the display is sorted or not.
		/// </summary>
		/// <param name="columnIndex">The index of the column in the display.</param>
		/// <returns>True if sorted, false otherwise.</returns>
		public bool IsColumnSorted(int columnIndex)
		{
			int loc = sortColIndexes.IndexOf(columnIndex);
			return loc > -1;
		}

		

		/// <summary>
		/// Checks whether a column at the given display index is filterable. This is only used when RowPivotsOnly is true.
		/// </summary>
		/// <param name="columnIndex">The index of the column in the display.</param>
		/// <returns>True if the column can be filtered and false otherwise.</returns>
		public bool CanFilterColumn(int columnIndex)
		{
			int col1 = ResolveColumnIndex(columnIndex);
			return RowPivotsOnly && ((col1 < PivotRows.Count && PivotRows[col1].AllowFilter) || (col1 >= PivotRows.Count && (col1 - PivotRows.Count) < pivotCalculations.Count && pivotCalculations[col1 - PivotRows.Count].AllowFilter));
		}

		/// <summary>
		/// Checks whether a column at the given display index is sortable. This is only used when RowPivotsOnly is true.
		/// </summary>
		/// <param name="columnIndex">The index of the column in the display.</param>
		/// <returns>True if the column can be sorted and false otherwise.</returns>
		public bool CanSortColumn(int columnIndex)
		{
			int col1 = ResolveColumnIndex(columnIndex);
			return RowPivotsOnly && ((col1 < PivotRows.Count && PivotRows[col1].AllowSort) || (col1 >= PivotRows.Count && pivotCalculations[col1 - PivotRows.Count].AllowSort));
		}

		/// <summary>
		/// Returns the field name at a given column even if the column has been moved from its original
		/// position. This is only used when RowPivotsOnly is true.
		/// </summary>
		/// <param name="columnIndex"></param>
		/// <returns></returns>
		public string GetFieldNameAtIndex(int columnIndex)
		{
			int col1 = ResolveColumnIndex(columnIndex);
			return RowPivotsOnly && col1 < ColumnCount && col1 >= PivotRows.Count ? pivotCalculations[col1 - PivotRows.Count].FieldName
				: (col1 < PivotRows.Count ? PivotRows[col1].FieldMappingName : "");

		}

		List<int> sortColIndexes = new List<int>();
		List<ListSortDirection> sortDirs = new List<ListSortDirection>();
		List<SortKeys> sortKeys = null;
        /// <summary>
        /// List that holds columnIndexes
        /// </summary>
		public List<int> columnIndexes = null;
		
		/// <summary>
		/// This method is used internally within the Syncfusion library code.
		/// </summary>
		/// <param name="from">Old index position.</param>
		/// <param name="to">New index position.</param>
		public void AdjustColumnIndexes(int from, int to)
		{
			int k = columnIndexes[from];
			columnIndexes.RemoveAt(from);
			columnIndexes.Insert(to, k);
			AdjustSortKeysForMoving(from, to);
		}

		private void AdjustSortKeysForMoving(int from, int to)
		{
			if (sortColIndexes != null && sortColIndexes.Count > 0)
			{
				int loc = sortColIndexes.IndexOf(from);
			   // if (loc > -1)
				{
					if (from < to)
					{
						for (int i = 0; i < sortColIndexes.Count; ++i)
						{
							if (sortColIndexes[i] >= from && sortColIndexes[i] <= to)
								sortColIndexes[i] -= 1;
						}
					}
					else if (from > to)
					{
						for (int i = 0; i < sortColIndexes.Count; ++i)
						{
							if (sortColIndexes[i] >= to && sortColIndexes[i] <= from)
								sortColIndexes[i] += 1;
						}
					}
					if (loc > -1)
					{
						sortColIndexes[loc] = to;
					}
				}
			}
		   
		}
        /// <summary>
        /// Sorts the different values by calculation
        /// </summary>
        /// <param name="colIndex">int</param>
		public void SortByCalculation(int colIndex)
		{
			SortByCalculation(colIndex, true);
		}
		private ListSortDirection calcSortDirection;
		/// <summary>
		/// Gets or sets the sorting direction
		/// </summary>
		public ListSortDirection SortDirection
		{
			get { return calcSortDirection; }
		}
        /// <summary>
        /// Sorts the different values by calculation
        /// </summary>
        /// <param name="colIndex">int</param>
        /// <param name="isMultiColumn">bool</param>
		public void SortByCalculation(int colIndex, bool isMultiColumn)
		{
            SortByCalculation(colIndex, isMultiColumn, calcSortDirection);
        }
        /// <summary>
        /// Performs sorting after the calculation
        /// </summary>
        /// <param name="colIndex">int</param>
        /// <param name="isMultiColumn">bool</param>
        /// <param name="dir">ListSortDirection</param>
        public void SortByCalculation(int colIndex, bool isMultiColumn, ListSortDirection dir)
        {
			if (UseIndexedEngine)
			{
				indexEngine.SortByCalculation(colIndex);
				return;
			}
            if (!RowPivotsOnly)
            {
                sortColIndexes.Clear();
				sortDirs.Clear();
            }
            calcSortDirection = dir;
          	calcSortDirection = calcSortDirection == ListSortDirection.Ascending ? ListSortDirection.Descending : ListSortDirection.Ascending;

			if (!isMultiColumn)
			{
				sortColIndexes.Clear();
				sortDirs.Clear();
			}
			else
			{
				int loc = sortColIndexes.IndexOf(colIndex);
				if (loc > -1)
				{
					calcSortDirection = sortDirs[loc] == ListSortDirection.Ascending ? ListSortDirection.Descending : ListSortDirection.Ascending;
					sortDirs.RemoveAt(loc);
					sortColIndexes.RemoveAt(loc);
				}
			}

			bool triggerRefresh = false;
			if (RowPivotsOnly && (colIndex < this.PivotRows.Count - 1 || (colIndex == this.PivotRows.Count - 1 && PivotRows[colIndex].Comparer != null)))
			{
  
				PivotItem pivotItem = PivotRows[colIndex];
				if (pivotItem.Comparer == null)
				{
					pivotItem.Comparer = new ReverseOrderComparer();

				}
				else if (pivotItem.Comparer is ReverseOrderComparer)
				{
					pivotItem.Comparer = null;
				}
				else
				{
					if (pivotItem.Comparer is SortWithDirComparer)
					{
						((SortWithDirComparer)pivotItem.Comparer).dir = calcSortDirection;
					}
					else
					{
						pivotItem.Comparer = new SortWithDirComparer(pivotItem.Comparer, calcSortDirection);
					}
				}

                triggerRefresh = colIndex < this.PivotRows.Count - 1;
				//let it drop through and resort any sorted value columns as these were reset during the SortRowPivotColumnAsValue call... 
			}
			sortColIndexes.Add(colIndex);
			sortDirs.Add(calcSortDirection);
		 
			CalcSortComparer comparer = new CalcSortComparer(sortDirs);
			int rowCountMinusGrandTotal = RowCount - 1;
			List<SortKeys> calcSortKeys = new List<SortKeys>(rowCountMinusGrandTotal);
			bool isInnerMostPivotColumn = colIndex == this.PivotRows.Count - 1;
			sortKeys = null;
			for (int i = 0; i < rowCountMinusGrandTotal; ++i)
			{
				IComparable[] keys = new IComparable[sortColIndexes.Count];
				int j = 0;
				foreach (int k in sortColIndexes)
				{

                    if (this[i, k] != null && (k == this.PivotRows.Count - 1 || (k >= this.PivotRows.Count && PivotCalculations.Count > (k - this.PivotRows.Count) && PivotCalculations[ ResolveColumnIndex(k) - this.PivotRows.Count].SummaryType == SummaryType.DisplayIfDiscreteValuesEqual)))
                    {
                        //keys[j++] = this[i, k].FormattedText != null ? this[i, k].FormattedText : string.Empty;
                        if (this[i, k].Value != null && !(this[i, k].Value is string))
                        {
                            keys[j++] = this[i, k].Value as IComparable;
                        }
                        else
                        {
                            keys[j++] = this[i, k].FormattedText != null ? this[i, k].FormattedText : string.Empty;
                        }
                    }
                    else if (this[i, k] != null)
                    {
                        keys[j++] = this[i, k].DoubleValue;
                    }
                    else
                    {
                        keys[j++] = double.MaxValue;
                    }
				}
				calcSortKeys.Add(new SortKeys() { Index = i, Keys = keys });
			}

			int row = 0;
			List<SortKeys> tempKeys = new List<SortKeys>();
			if (isInnerMostPivotColumn)
			{
				//base the sorting on the first value column layout
				 colIndex += 1;
			}
			int edge = PivotRows.Count;
			if (triggerRefresh == true)
			{
				row = PivotColumns.Count + (/*PivotCalculations.Count > 1*/ IsCalculationHeaderVisible ? 1 : 0);
			}
			Dictionary<string, List<SortKeys>> dictionForExpander = new Dictionary<string, List<SortKeys>>();
			string tempString = null;
			List<SortKeys> templist = new List<SortKeys>();
			while (row < rowCountMinusGrandTotal)
			{
				int k = 0;
				int startRow = row;
				tempKeys.Clear();
				while (row < rowCountMinusGrandTotal && triggerRefresh != true && this[row, colIndex] != null && 
                    ((this[row, colIndex].CellType == PivotCellType.ValueCell) || (this[row, colIndex].CellType == (PivotCellType.ValueCell | PivotCellType.GrandTotalCell)) || (this[row, colIndex].CellType == (PivotCellType.ValueCell | PivotCellType.TotalCell)) || (isInnerMostPivotColumn && this[row, colIndex].CellType == (PivotCellType.RowHeaderCell | PivotCellType.HeaderCell))) 
                 && !IsRowSummary(row))
                {

					tempKeys.Add(new SortKeys() { Keys = calcSortKeys[row].Keys, Index = calcSortKeys[row].Index });
					k++;
					row++;
				}

                if (k > 0)
                {
                    tempKeys.Sort(comparer);
                    for (int i = 0; i < k; ++i)
                    {
                        calcSortKeys[i + startRow] = tempKeys[i];
                    }
                }
				while (row < rowCountMinusGrandTotal && (this[row, colIndex] == null || (this[row, colIndex].CellType != PivotCellType.ValueCell && (isInnerMostPivotColumn && this[row, colIndex].CellType != (PivotCellType.RowHeaderCell | PivotCellType.HeaderCell)))))
				{
					row++;
				}
				if (startRow == row) row++;
			}
			sortKeys = calcSortKeys;
			if (triggerRefresh)
			{
				RaisePivotSchemaChangedEvent(new PivotSchemaChangedArgs() { ChangeHints = SchemaChangeHints.None });
			}


		}

		ListSortDirection sortDirection = ListSortDirection.Ascending;
        /// <summary>
        /// Performs the sorting operation in Column header
        /// </summary>
        /// <param name="colHeaderIndex">int</param>
		public void SortColumnHeader(int colHeaderIndex)
		{
			Dictionary<int, string> dictionColumnHeaders = new Dictionary<int, string>();
			Dictionary<int, List<PivotCellInfo>> tempDictionary = new Dictionary<int, List<PivotCellInfo>>();
			List<string> uniqueHeader = new List<string>();
			int col = PivotRows.Count;
			sortDirection = sortDirection == ListSortDirection.Ascending ? ListSortDirection.Descending : ListSortDirection.Ascending;
			for (int i = PivotRows.Count; i < ColumnCount - (PivotRows.Count + PivotCalculations.Count - 1); i++)
			{
				if (this[colHeaderIndex, i].FormattedText == null && this[colHeaderIndex, i].UniqueText.Contains("Total"))
					dictionColumnHeaders.Add(i, this[colHeaderIndex, i].UniqueText);
				else
					dictionColumnHeaders.Add(i, this[colHeaderIndex, i].FormattedText);
				if ((!uniqueHeader.Any(x => x == this[colHeaderIndex, i].FormattedText) || this[colHeaderIndex, i].FormattedText == null || this[colHeaderIndex, i - 1].FormattedText == null) && !this[colHeaderIndex, i].CellType.ToString().Contains("TotalCell") && (this[colHeaderIndex, i].FormattedText != null || this[colHeaderIndex, i].UniqueText == "x") && !this[colHeaderIndex, i].UniqueText.ToString().Contains("Total"))
					uniqueHeader.Add(this[colHeaderIndex, i].FormattedText);
			}
			if (!uniqueHeader.Contains(null))
			{
				uniqueHeader.Reverse();
				col = SwapPivotColumns(colHeaderIndex, dictionColumnHeaders, tempDictionary, uniqueHeader, col);
			}
			else
			{
				for (int i = 0; i < uniqueHeader.Count; )
				{
					List<string> tempList = new List<string>();
					if (uniqueHeader[i] == null)
					{
						uniqueHeader.RemoveAt(i);
					}
					else
					{
						while (i < uniqueHeader.Count && uniqueHeader[i] != null)
						{
							tempList.Add(uniqueHeader[i]);
							i++;
						}
						tempList.Reverse();
						col = SwapPivotColumns(colHeaderIndex, dictionColumnHeaders, tempDictionary, tempList, col);
						if (colHeaderIndex == PivotColumns.Count - 1)
							col = col + PivotCalculations.Count;
					}
				}

			}
		}


		private int SwapPivotColumns(int colHeaderIndex, Dictionary<int, string> dictionColumnHeaders, Dictionary<int, List<PivotCellInfo>> tempDictionary, List<string> uniqueHeader, int col)
		{
			foreach (string headerItem in uniqueHeader)
			{
				int startKey = dictionColumnHeaders.Where(pair => pair.Value == headerItem)
				   .Select(pair => pair.Key)
				   .FirstOrDefault();
				if (headerItem == null)
				{
					col = col - PivotCalculations.Count;
				}
				while (startKey <= dictionColumnHeaders.ElementAt(dictionColumnHeaders.Count - 1).Key && dictionColumnHeaders[startKey] != null && dictionColumnHeaders[startKey].ToString().Contains(headerItem))
				{
					int j = 0;
					List<PivotCellInfo> cellInfo = new List<PivotCellInfo>();
					for (int i = colHeaderIndex; i < RowCount - PivotRows.Count; i++)
					{
						if (this.PivotValues[i, col] != null)
							cellInfo.Add(this.PivotValues[i, col]);
						if (!tempDictionary.Keys.Contains(startKey))
						{
							this.PivotValues[i, col] = this.PivotValues[i, startKey];
						}
						else
						{
							if (tempDictionary.Any(x => x.Key == startKey))
							{
								this.PivotValues[i, col] = tempDictionary.Single(x => x.Key == startKey).Value[j];
								j++;
							}
						}
					}
					tempDictionary.Add(col, cellInfo);
					col++;
					startKey++;
				}
			}
			return col;
		}
	  
		//this routine is working directly on the un-indexed pivot contents, and physically re-arranging them
		private void SortRowPivotColumnAsValue(int colIndex)
		{
			List<PivotCellInfo> cells = new List<PivotCellInfo>();
			int row = 1;
			while (row < this.RowCount && (this[row, colIndex] == null || (this[row, colIndex].CellType & PivotCellType.ExpanderCell) == 0))
			{
				row++;
			}
			int rowStart = 1;
			while (row < this.RowCount) 
			{
				rowStart = row;
				PivotCellInfo info = this[row, colIndex];
				cells.Add(info);
				row += info.CellRange.Bottom - info.CellRange.Top + 2; //+2 because of total row
				bool first = true;
				while (row < this.RowCount && ( this[row, colIndex] == null || (this[row, colIndex].CellType & PivotCellType.ExpanderCell) == 0))
				{
					if (first)
					{
						//save reordered pivots in a list

						first = false;
					}
					row++;
				}
			}
			int rowCountMinusGrandTotal = RowCount - 1;
			List<SortKeys> calcSortKeys = new List<SortKeys>(rowCountMinusGrandTotal);
			if (sortKeys == null)
			{
				for (int i = 0; i < rowCountMinusGrandTotal; i++ )
				{
					calcSortKeys.Add(new SortKeys() { Index = i });
				}
			}
			else
			{
				calcSortKeys = sortKeys;
			}

			sortKeys = null;

			sortKeys = calcSortKeys;
		}

		 class ReverseOrderComparer : IComparer
		{
			#region IComparer Members

			public int Compare(object x, object y)
			{
				if (x == null && y == null)
					return 0;
				else if (y == null)
					return 1;
				else if (x == null)
					return -1;
				else
					return -x.ToString().CompareTo(y.ToString());
			}

			#endregion
		}
		 class SortWithDirComparer : IComparer
		 {
			 IComparer comparer = null;
			 public ListSortDirection dir = ListSortDirection.Ascending;

			 public SortWithDirComparer(IComparer comparer, ListSortDirection dir)
			 {
				 this.comparer = comparer;
				 this.dir = dir;
			 }

			 public int Compare(object x, object y)
			 {
				 int c = this.comparer.Compare(x, y);
				 if (dir == ListSortDirection.Descending)
					 c = -c;
				 return c;
			 }
		 }

        /// <summary>
         /// Class used for sorting the PivotedRow/column 
        /// </summary>
		public class CalcSortComparer : IComparer<SortKeys>
		{
			List<ListSortDirection> dirs = new List<ListSortDirection>();
			public CalcSortComparer(List<ListSortDirection> sortDirs)
			{
				this.dirs = sortDirs;
			}
			public int Compare(SortKeys x, SortKeys y)
			{
				int c = 0;
				int count = dirs.Count;
				for (int i = 0; i < count; ++i)
				{
					if (x.Keys[i] == null && y.Keys[i] == null)
						c = 0;
					else if (x.Keys[i] == null)
						c = -1;
					else if (y.Keys[i] == null)
						c = 1;
					else if (x.Keys[i].GetType() != y.Keys[i].GetType())
					{
						c = (x.Keys[i] is string) ? -1 : 1;
					}
					else
						c = x.Keys[i].CompareTo(y.Keys[i]);

					if (c != 0)
					{
						if (dirs[i] == ListSortDirection.Descending)
							c = -c;
						break;
					}
				}

				return c;
			}
		}


        #endregion 

        private bool? notPopulated = false;

        /// <summary>
        /// Used internally.
        /// </summary>
        public bool? NotPopulated
        {
            get { return notPopulated; }
            set { notPopulated = value; }
        }

        private double populationStatus = 0;

        /// <summary>
        /// Used internally.
        /// </summary>
        public double PopulationStatus
        {
            get { return populationStatus; }
            set { populationStatus = value; }
        }


        /// <summary>
		/// Gets the <see cref="PivotCellInfo"/> object associated with a particular cell
		/// in the pivot table. The PivotCellInfo contains Value and FormattedText properties
		/// that hold this cell's contents.
		/// </summary>
		/// <param name="rowIndex">The 0-based row index of the cell.</param>
		/// <param name="columnIndex1">The 0-based column index of the cell.</param>
		/// <returns>A PivotCellInfo object that contains information on the cell contents.</returns>
		public PivotCellInfo this[int rowIndex, int columnIndex1]
		{
			get
			{
                return GetPivotEngineValueFor(rowIndex, columnIndex1, true);
			}
		}

        public PivotCellInfo GetPivotEngineValueFor(int rowIndex, int columnIndex1, bool shouldCalculateTotal)
        {
            int columnIndex = rowPivotsOnly && columnIndexes != null && columnIndex1 < columnIndexes.Count
                            ? columnIndexes[columnIndex1] : columnIndex1;
            if (notPopulated != false)
                return new PivotCellInfo();
            if (rowIndex < 0 || rowIndex >= rowCount)
                throw new ArgumentOutOfRangeException("RowIndex out of range.");
            if (columnIndex < 0 || columnIndex >= columnCount)
                throw new ArgumentOutOfRangeException("ColumnIndex out of range.");

            //code to use IndexEngine
            if (indexEngine != null)
            {
                PivotCellInfo pci = indexEngine[rowIndex, columnIndex];
                return pci != null ? pci : new PivotCellInfo();
            }

            //code to do ondemand loading of calculations
            if (EnableOnDemandCalculations &&
                rowIndex >= (showCalculationsAsColumns? this.rowOffSet: PivotRows.Count) &&
                columnIndex >= (ShowCalculationsAsColumns ? this.colOffSet : PivotColumns.Count + (/*PivotCalculations.Count > 1*/ IsCalculationHeaderVisible ? 1 : 0)) &&
                _populationCompleted)
            {
                EnsureCalculationAt(rowIndex, columnIndex, shouldCalculateTotal);
            }
            if (sortKeys != null && rowIndex < sortKeys.Count && columnIndex >= this.PivotRows.Count - 1)
            {
                return pivotValues[sortKeys[rowIndex].Index, columnIndex];
            }
            return pivotValues[rowIndex, columnIndex];
        }

        private void EnsureCalculationAt(int rowIndex, int columnIndex, bool shouldCalculateTotal)
        {
            if (pivotValues[rowIndex, columnIndex] != null && pivotValues[rowIndex, columnIndex].Summary == null)
            {//first make sure SUmmary is defined...
                pivotValues[rowIndex, columnIndex].Summary = GetOnDemandValue(rowIndex, columnIndex);
            }
            if (shouldCalculateTotal && pivotValues[rowIndex, columnIndex] != null && pivotValues[rowIndex, columnIndex].Summary == null && pivotValues[rowIndex, columnIndex].CellType != PivotCellType.ValueCell)
            {//if summary has not been totalled, then do the total on demand
                pivotValues[rowIndex, columnIndex].Summary = GetOnDemandTotal(rowIndex, columnIndex);
            }
        }

        public int highWaterRowIndex = 0;
        int highWaterColumnIndex = 0;
        bool onDemandCalculationsLoaded = false;

        /// <summary>
        /// When EnableOnDemandCalculations is true, calling this method will ensure that all the summary calculations are fully computed.
        /// This is used internally.
        /// </summary>
        public void EnsureCalculationsLoaded()
        {
            if (EnableOnDemandCalculations)
            {
                while (this.highWaterRowIndex < this.RowCount - 1)
                {
                    DoLazyCalculation();
                }
            }
        }

        /// <summary>
        /// When <see cref="EnableOnDemandCalculations"/> is true, this method will compute all summary calculation for the first row that has not been calculated yet.
        /// </summary>
        /// <returns>Returns true if all rows have their summary calculation completed, false otherwise.</returns>
        public bool DoLazyCalculation()
        {
            if (EnableOnDemandCalculations && UseIndexedEngine == false)
            {
                if (this.highWaterRowIndex < this.RowCount - 1)
                {
                    for (int j = this.colOffSet; j < this.ColumnCount - 1; ++j)
                    {
                        EnsureCalculationAt(this.highWaterRowIndex, j, true);
                    }
                    this.highWaterRowIndex++;
                }

                return this.highWaterRowIndex >= this.RowCount - 1;
            }
            return true;
        }

		private SummaryBase GetOnDemandTotal(int row, int col)
		{
			SummaryBase sb = null;
			if (PivotCalculations.Count > 0)
			{
				int k = 0;
                int calcIndex = 0;
                if(showCalculationsAsColumns)
                   calcIndex = (col - colOffSet) % PivotCalculations.Count;
                else
                    calcIndex = (row - (PivotRows.Count)) % PivotCalculations.Count;
				sb = PivotCalculations[calcIndex].Summary.GetInstance();
				InitSummary(row, col, sb);

                bool chk;
                if (this.GridLayout != GridLayout.TopSummary)
                    chk = pivotValues[rowOffSet, col] != null && 0 != (pivotValues[rowOffSet, col].CellType & PivotCellType.TotalCell);
                else
                    chk = pivotValues[rowOffSet, col] != null && 0 != (pivotValues[rowOffSet, col].CellType & PivotCellType.ExpanderCell);

				k = 0;
                if ((!chk && IsSummaryRow(row, ref k) && this.GridLayout != GridLayout.TopSummary) || (this.GridLayout == GridLayout.TopSummary && !chk && IsSummaryRowWhileTopSummary(row)))
				{
					if (RowSummands.ContainsKey(row))
					{
						bool hit = false;
                        foreach (int i1 in RowSummands[row])
                        {
                            if (this[i1, col] != null && this[i1, col].Summary != null)
                            {
                                sb.CombineSummary(this[i1, col].Summary);
                                hit = true;
                            }
                        }
						pivotValues[row, col].Summary = sb;
						InitSummary(row, col, sb);
						if (!hit && (pivotValues[row, col].CellType & PivotCellType.TotalCell) != 0)
						{
							pivotValues[row, col].Value = "0";
							pivotValues[row, col].FormattedText = (0).ToString(PivotCalculations[calcIndex].Format);
						}
					}
					else
					{
						if (PivotColumns.Count == 0 && row == 0)
						{
							pivotValues[row, col].FormattedText = PivotCalculations[calcIndex].CalculationName;
						}
					}
				}
				else
				{
					int off = (col - colOffSet) % PivotCalculations.Count;
					int kStart = off;
					int kEnd = off + 1;

					if (ColSummands.ContainsKey(col - off))
					{
						bool adjustColumn = false;
						if (PivotColumns.Count <= 1 && PivotRows.Count == 0)
						{
							adjustColumn = true;
							kEnd = PivotCalculations.Count;
							kStart = 0;
						}
						for (int k1 = kStart; k1 < kEnd; ++k1)
						{
							sb = PivotCalculations[k1].Summary.GetInstance();
							bool hit = false;
							foreach (int i1 in colSummands[col - off])
							{
								if (i1 != 0 && (i1 - (ShowCalculationsAsColumns ? colOffSet : (PivotRows.Count + 1)) + off)% PivotCalculations.Count == k1)
								{
									if (this[row, i1 + off] != null && this[row, i1 + off].Summary != null)
									{
										sb.CombineSummary(this[row, i1 + off].Summary);
										hit = true;
									}
								}
							}

							int col1 = adjustColumn ? col + k1 : col;
							pivotValues[row, col1].Summary = sb;
							InitSummary(row, col1, sb);
							if (!hit && (pivotValues[row, col1].CellType & PivotCellType.TotalCell) != 0 && !ShowNullAsBlank)
							{
								pivotValues[row, col1].Value = "0";
								pivotValues[row, col1].FormattedText = (0).ToString(PivotCalculations[k1].Format);
							}
						}
					}
					else
					{
						if (PivotColumns.Count == 0 && row == 0)
						{
							pivotValues[row, col].FormattedText = PivotCalculations[calcIndex].CalculationName != null ? PivotCalculations[calcIndex].CalculationName : PivotCalculations[calcIndex].FieldHeader;
						}
						else if (PivotColumns.Count == 0 && row > 0 && this.GridLayout!=GridLayout.TopSummary)
						{
							if (RowSummands.ContainsKey(row))
							{
								bool hit = false;
								foreach (int i1 in RowSummands[row])
								{
									if (this[i1, col] != null && this[i1, col].Summary != null)
									{
										sb.CombineSummary(this[i1, col].Summary);
										hit = true;
									}
								}
								pivotValues[row, col].Summary = sb;
								InitSummary(row, col, sb);
								if (!hit && (pivotValues[row, col].CellType & PivotCellType.TotalCell) != 0)
								{
									pivotValues[row, col].Value = "0";
									pivotValues[row, col].FormattedText = (0).ToString(PivotCalculations[calcIndex].Format);
								}
							}
						}
						else
						{
							if (PivotRows.Count == 0 && col == 0)
							{
								pivotValues[row, col].FormattedText = GrandString + " " + PivotColumns[0].TotalHeader;
							}
							else
							{
								int off2 = (PivotColumns.Count > 0 && pivotRows.Count == 0 && PivotCalculations.Count > 0) ? 1 : rowOffSet;

								int off1 = (col - off2) % PivotCalculations.Count;
                                if (ColSummands.ContainsKey(col - off1))
                                {
                                    bool hit = false;
                                    foreach (int i1 in colSummands[col - off1])
                                    {
                                        if (this[row, i1 + off1] != null && this[row, i1 + off1].Summary != null)
                                        {
                                            sb.CombineSummary(this[row, i1 + off1].Summary);
                                            hit = true;
                                        }
                                    }
                                    pivotValues[row, col].Summary = sb;
                                    InitSummary(row, col, sb);
                                    if (!hit && (pivotValues[row, col].CellType & PivotCellType.TotalCell) != 0)
                                    {
                                        pivotValues[row, col].Value = "0";
                                        pivotValues[row, col].FormattedText = (0).ToString(PivotCalculations[off1].Format);
                                    }
                                }
                                }
							}
						}
					}
				}
			return sb;
		}
		private SummaryBase GetOnDemandValue(int row, int col)
		{
			SummaryBase sb = null;
			if (row <= RowCount && col <= ColumnCount)
			{
				if (PivotCalculations.Count > 0)
				{
					List<IComparable> keys = GetKeyAt(row, col,true);
					int loc = keys.Count > 0 ? tableKeysCalcValues.BinarySearch(new KeysCalculationValues() { Keys = keys }) : -1;
					if (loc > -1)
					{
						KeysCalculationValues tempValue = tableKeysCalcValues[loc] as KeysCalculationValues;

                        int k = 0;
                        if (showCalculationsAsColumns)
                            k = (col - colOffSet) % PivotCalculations.Count;
                        else
                            k = (row - PivotColumns.Count) % PivotCalculations.Count;
						{
							sb = tempValue.Values[k];
						}
						InitSummary(row, col, sb);
					}
				}
			}
			return sb;
		}

        private void InitSummary(int row, int col, SummaryBase sb)
        {
            if (sb == null)
                return;
            if (sb.GetResult() == null)
            {
                sb = sb.GetInstance();

            }
            object v = sb.GetResult();
            string format = null;
            if (ShowCalculationsAsColumns)
                format = "{0:" + PivotCalculations[(col - this.colOffSet) % PivotCalculations.Count].Format + "}";
            else
                format = "{0:" + PivotCalculations[(row - PivotRows.Count) % PivotCalculations.Count].Format + "}";
            FieldInfo item = AllowedFields.FirstOrDefault(x => x.Name == PivotCalculations[(col - this.colOffSet) % PivotCalculations.Count].FieldName);
            if ((item != null && item.FieldType != FieldTypes.Expression) || (item == null))
            {
                if (format.Equals("{0:#.##}") && v != null && v.Equals(0.0d) && (!ShowNullAsBlank && this.pivotValues[row, col].FormattedText == null)) ////Condition added since the format string string.Format({0:#.##},0.0) returns empty.
                {
                    this.pivotValues[row, col].FormattedText = "0.0";
                }
                else if ((this.pivotValues[row, col].FormattedText != null || EnableOnDemandCalculations)
                         && ((v != null && !v.Equals(0.0d) && !v.Equals(0) && ShowNullAsBlank && this.pivotValues[row, col].Value == null) || this.pivotValues[row, col].Value != null || !ShowNullAsBlank)
                         && (col > (ShowCalculationsAsColumns ? PivotRows.Count - 1: PivotRows.Count ) && row > (PivotColumns.Count - 1 + ((showCalculationsAsColumns && IsCalculationHeaderVisible) ? 1 : 0))))
                {
                    this.pivotValues[row, col].FormattedText = string.Format(CultureInfo.CurrentUICulture, format, v);
                }
            }
            this.pivotValues[row, col].Value = v;
            this.pivotValues[row, col].Format = PivotCalculations[col % PivotCalculations.Count].Format;
        }
		private List<PivotItem> pivotRows = null;

		/// <summary>
		/// Gets or set a collection of row pivot properties.
		/// </summary>
		public List<PivotItem> PivotRows
		{
			get
			{
				if (pivotRows == null)
					pivotRows = new List<PivotItem>();
				return pivotRows;
			}
			set { pivotRows = value; }
		}

		private List<PivotItem> pivotColumns = null;

		/// <summary>
		/// Gets or sets a collection of column pivot properties.
		/// </summary>
		public List<PivotItem> PivotColumns
		{
			get
			{
				if (pivotColumns == null)
					pivotColumns = new List<PivotItem>();
				return pivotColumns;
			}
			set { pivotColumns = value; }
		}

		private List<PivotComputationInfo> pivotCalculations = null;

		/// <summary>
		/// Gets or sets a collection of pivot calculations.
		/// </summary>
		public List<PivotComputationInfo> PivotCalculations
		{
			get
			{
				if (pivotCalculations == null)
					pivotCalculations = new List<PivotComputationInfo>();
				return pivotCalculations;
			}
			set { pivotCalculations = value; }
		}

   
		private bool useDescriptionInCalculationHeader = false;
		/// <summary>
        /// Gets or sets whether the <see cref="UseDescriptionInCalculationHeader"/> or the
        /// <see cref="UseDescriptionInCalculationHeader"/> is displayed in the header cell
		/// when more than one calculation is being used. The default behavior is to use
		/// the FieldName.
		/// </summary>
		[DefaultValue(false)]
		public bool UseDescriptionInCalculationHeader
		{
			get { return useDescriptionInCalculationHeader; }
			set { useDescriptionInCalculationHeader = value; }
		}

		private bool emptyPivot = false;

		/// <summary>
		/// Gets whether a pivot results contains any items.
		/// <remarks>For example, if you apply a filter which filters
		/// out all items in the underlying data source, then this
		/// EmptyPivot property will be set true. The RowCount and ColumnCount will
		/// be set to one, and the Engine[0, 0] will hold the value
		/// <see cref="EmptyPivotString"/>.</remarks>
		/// </summary>
		public bool EmptyPivot
		{
			get { return emptyPivot; }
			internal set { emptyPivot = value; }
		}

		private string emptyPivotString = "No items in result.";

		/// <summary>
		/// Gets or sets the string that appears when no items are present in a pivot result.
		/// </summary>
		[DefaultValue("No items in result.")]
		public string EmptyPivotString
		{
			get { return emptyPivotString; }
			set { emptyPivotString = value; }
		}
		private string grandString = "Grand";

		/// <summary>
		/// Gets or sets the prefix string for the collection-wide totals that appear below and to the right of the pivot table.
		/// </summary>
		[DefaultValue("Grand")]
		public string GrandString
		{
			get { return grandString; }
			set { grandString = value; }
		}
        private bool usePercentageFormat = true;
        /// <summary>
        /// Gets or sets whether the formatted text of the columns which has Percentage caluclation type, must applied with given format or default percentage format #,##%
        /// </summary>
        
        public bool UsePercentageFormat 
        {
            get { return usePercentageFormat; }
            set { usePercentageFormat = value; }
        }


        private bool loadInBackground = false;
        /// <summary>
        /// Gets or sets the value for the LoadInBackground property which controls whether the initial pivot operations
        /// are done off the UI thread.
        /// </summary>
        public bool LoadInBackground 
        {
            get
            {
                return loadInBackground;
            }
            set
            {
                loadInBackground = value;
                OnPropertyChanged("LoadInBackground");
            }
        }

        private bool cacheRawValues = false;
        /// <summary>
        /// Gets or sets whether underlying raw values are cached as the pivot is being built. If these values are cached,
        /// then GetRawItemsFor method calls do not require tranversing the underlying data to complete.
        /// </summary>
        public bool CacheRawValues
        {
            get { return cacheRawValues; }
            set { cacheRawValues = value; }
        }

		bool showCalculationsAsColumns = true;
		/// <summary>
		/// Gets or sets whether the calculations should appear as rows or columns. THe default behavior is 
		/// for the calculations to appear as columns.
		/// </summary>
		public bool ShowCalculationsAsColumns
		{
			get { return showCalculationsAsColumns; }
			set
			{
				if (showCalculationsAsColumns != value)
				{
					showCalculationsAsColumns = value;
					RaisePivotSchemaChangedEvent(new PivotSchemaChangedArgs());
				}
			}
		}


		private bool showEmptyCell = true;

		/// <summary>
		/// Gets or sets a value indicating whether show empty value cell if summary value has null value.
		/// By default true.
		/// </summary>       
		[DefaultValue(true)]
		public bool ShowEmptyCells
		{
			get { return showEmptyCell; }
			set { showEmptyCell = value; }
		}

		private object dataSource = null;
		/// <summary>
		/// Gets or sets source of data for this pivot table. This object should be either 
		/// an IEnumerable list, or a DataTable.
		/// </summary>
		public object DataSource
		{
			get { return dataSource; }
			set
			{
				if (dataSource != value)
				{
					dataSource = value;

					//force list to be reset
					dataSourceList = null;
					ItemType = null;
					itemProperties = null;

					RaisePivotSchemaChangedEvent(new PivotSchemaChangedArgs());
				}
			}
		}

		private IEnumerable dataSourceList = null;

		/// <summary>
		/// Used internally. Gets or sets an IEnumerable list that is used as the data for the
		/// pivot table. The default behavior is to initialize this list from DataSource.
		/// </summary>
		public virtual IEnumerable DataSourceList
		{
			get
			{
				if (dataSourceList == null)
				{
					if (DataSource is IEnumerable)
					{
						dataSourceList = DataSource as IEnumerable;
					}
#if !SILVERLIGHT
					else if (DataSource is DataTable)
					{
						dataSourceList = ((DataTable)DataSource).DefaultView as IEnumerable;
					}
#endif
				}
				return dataSourceList;
			}
			set { dataSourceList = value; }
		}


		/// <summary>
		/// Gets of sets the type of the objects in the DataSource. This property is usually initialized
		/// directly from the DataSource.
		/// </summary>
		public Type ItemType { get; set; }

#if SILVERLIGHT

		private IDictionary itemProperties;

        /// <summary>
        /// Gets a collection of dictionary objects describing the type of the underlying data item.
        /// </summary>
        public IDictionary ItemProperties
        {
            get
            {
                CalculationExtensions.Engine = this;
#if SyncfusionFramework4_0
                if (isDataDynamic)
                {
                    IDictionary<string, object> dynamicObject = null;
                    foreach (object o in DataSourceList)
                    {
                        dynamicObject = o as IDictionary<string, object>;
                        if (dynamicObject == null)
                            continue;
                        break;
                    }
                    if (dynamicObject != null)
                    {
                        List<DynamicPropertyInfo> props = new List<DynamicPropertyInfo>();
                        Dictionary<string, Type> types = new Dictionary<string, Type>();
                        foreach (string key in dynamicObject.Keys)
                        {
                            if (dynamicObject[key] == null)
                                types.Add(key, typeof(object));
                            else
                                types.Add(key, dynamicObject[key].GetType());
                            props.Add(new DynamicPropertyInfo(){ Name = key, Type = types[key] });
                        }
                        itemProperties = new Dictionary<string, object>();
                        Array.ForEach<DynamicPropertyInfo>(props.ToArray(), p => itemProperties.Add(p.Name, p));
                        CalculationExtensions.DynamicPropertyTypeTable = types;
                    }
                    return itemProperties;
                }
                else

#endif
                if (itemProperties == null && (DataSourceList != null || ItemType != null))
                {

					if (ItemType == null)
					{
						foreach (object o in DataSourceList)
						{
							ItemType = o.GetType();
							break;
						}
					}
				}

				if (ItemType != null && itemProperties == null)
				{
					itemProperties = new Dictionary<object, object>();
					PropertyInfo[] array = ItemType.GetProperties();
					Array.ForEach<PropertyInfo>(array, p => itemProperties.Add(p.Name, p));
				}

				Dictionary<object, object> includes = new Dictionary<object, object>();
				if (itemProperties != null)
				{
					foreach (DictionaryEntry entry in itemProperties)
					{
						object key = entry.Key;
						if (entry.Value is PropertyInfo)
						{
							PropertyInfo pd = entry.Value as PropertyInfo;
							if (AllowedFields.Count == 0 || AllowedFields.IndexOf(new FieldInfo() { Name = pd.Name }) > -1)
							{
								includes.Add(key, pd);
							}
						}
						else if (entry.Value is ExpressionPropertyDescriptor)
						{
							ExpressionPropertyDescriptor exPD = entry.Value as ExpressionPropertyDescriptor;
							if (AllowedFields.Count == 0 || AllowedFields.IndexOf(new FieldInfo() { Name = exPD.Name }) > -1)
							{
								includes.Add(key, exPD);
							}
						}
					}
				}

				foreach (FieldInfo fi in AllowedFields)
				{
					if (fi.FieldType == FieldTypes.Expression && !includes.ContainsKey(fi.Name))
					{
						ExpressionPropertyDescriptor expDesc = new ExpressionPropertyDescriptor(fi.Name, null, fi.Expression, fi.Format, filters);
						includes.Add(fi.Name, expDesc);
					}
				}
				
				foreach (KeyValuePair<object, object> dictPair in includes)
				{
					if (!itemProperties.Contains(dictPair.Key))
						itemProperties.Add(dictPair.Key, dictPair.Value);
				}

				if (itemProperties == null) itemProperties = new Dictionary<object, object>(includes);

				return itemProperties;
			}
			set { itemProperties = value; }
		}

#else
        /// <summary>
        /// Used to fill/refill the ItemProperties with newly added/updated fields.
        /// </summary>
        public void RefreshItemProperties()
        {
            IsItemPropertiesFilled = false;
        }
        internal bool IsItemPropertiesFilled { get; set; }


		private PropertyDescriptorCollection itemProperties = null;

		/// <summary>
		/// Gets or sets a collection of property descriptors for the items in the DataSource. This property is 
		/// usually initialized directly from the DataSource.
		/// </summary>
		public PropertyDescriptorCollection ItemProperties
		{
			get
            {
                if (!IsItemPropertiesFilled)
                {
                   if (DataSourceList != null || ItemType != null)
                    {
#if SyncfusionFramework4_0
                    if (isDataDynamic)
                    {
                        IDictionary<string, object> dynamicObject = null;
                        foreach (object o in DataSourceList)
                        {
                            dynamicObject = o as IDictionary<string, object>;
                            break;
                        }
                        if (dynamicObject != null)
                        {
                            List<DynamicPropertyDescriptor> props = new List<DynamicPropertyDescriptor>();
                            Dictionary<string, Type> types = new Dictionary<string, Type>();
                            foreach (string key in dynamicObject.Keys)
                            {
                                props.Add(new DynamicPropertyDescriptor(key, null));
                                object val = dynamicObject[key];
                                if(val==null)
                                    types.Add(key,typeof(object));
                             else
                                types.Add(key, val.GetType());
                            }
                            itemProperties = new PropertyDescriptorCollection(props.ToArray());
                            CalculationExtensions.DynamicPropertyTypeTable = types;
                        }
                    }
                    else

#endif
						if (dataSourceList is ITypedList)
						{
							itemProperties = ((ITypedList)dataSourceList).GetItemProperties(null);
						}
						else
						{
							if (ItemType == null)
							{
                                if ((DataSourceList as IList).Count > 0)
                                {
                                    foreach (object o in DataSourceList)
                                    {
                                        ItemType = o.GetType();
                                        break;
                                    }
                                }
                                else
                                {
                                    ItemType = (DataSourceList as IList).GetType().GetProperties()[1].PropertyType;                                   
                                }
							}
							if (ItemType != null)
							{
								itemProperties = TypeDescriptor.GetProperties(ItemType);
							}
						}
				}
				List<PropertyDescriptor> includes = new List<PropertyDescriptor>();
				if (itemProperties != null)
				{
					foreach (PropertyDescriptor pd in itemProperties)
					{
						if (AllowedFields.Count == 0 || AllowedFields.IndexOf(new FieldInfo() { Name = pd.Name }) > -1)
						{
							includes.Add(pd);
						}
					}
				}

				foreach (FieldInfo fi in AllowedFields)
				{
					if (fi.FieldType == FieldTypes.Expression)
					{
						if (includes.IndexOf(new ExpressionPropertyDescriptor(fi.Name, null, fi.Expression, fi.Format, filters)) == -1)
						includes.Add(new ExpressionPropertyDescriptor(fi.Name, null, fi.Expression, fi.Format, filters));
					}
				}
				itemProperties = new PropertyDescriptorCollection(includes.ToArray());
				
			}
                if (itemProperties != null && itemProperties.Count > 0)
                    IsItemPropertiesFilled = true;
            return itemProperties;
		}
			set { itemProperties = value; }
		}
#endif

        private List<FieldInfo> allowedFields = null;
		/// <summary>
		/// Gets a collection of <see cref="FieldInfo"/> objects that hold field names that you want
		/// to be visible in the engine. The names can be either public property names of the underlying data
		/// objects, or they can be expression field.
		/// </summary>
		/// <remarks>
		/// If your data contains fields that you do not want exposed to the pivoting process,
		/// then add the names of the properties you want to include to this list. All other
		/// fields will be excluded. 
		/// 
		/// If you leave this collection empty, the default behavior will be to make all public
		/// properties available for use in the pivot table.
		/// 
		/// To add an expression field, set the FieldInfo.FieldType to FieldsType.Expression and set FieldInfo.Expression
		/// to be a string holding a well formed expression defining the value that should appear in this field.</remarks>
		public List<FieldInfo> AllowedFields
		{
			get
			{
				if (allowedFields == null)
					allowedFields = new List<FieldInfo>();
				return allowedFields;
			}
		}

		#endregion

		#region methods
        /// <summary>
        /// Adds the fields to AllowedField
        /// </summary>
        /// <param name="fi">FieldInfo</param>
		public void AddAllowedField(FieldInfo fi)
		{
			if (AllowedFields.IndexOf(new FieldInfo() { Name = fi.Name }) == -1)
			{
				AllowedFields.Add(fi);
				RaisePivotSchemaChangedEvent(new PivotSchemaChangedArgs());
			}
			else
			{
				throw new ArgumentException(string.Format("{0} already in collection.", fi.Name));
			}
		}

#if SILVERLIGHT
		public void AddAllowedField(FieldInfo fi, bool updatePivotSchema)
		{
			if (!AllowedFields.Any(i => i.Name == fi.Name))
			{
				AllowedFields.Add(fi);
				if (updatePivotSchema)
				{
					RaisePivotSchemaChangedEvent(new PivotSchemaChangedArgs());
				}
			}
		} 
#endif


		private FilterItemsCollection itemCollection;
		/// <summary>
		/// Gets or Sets the Collection of Items
		/// </summary>
		public FilterItemsCollection ItemCollection
		{
			get { return itemCollection; }
			set { itemCollection = value; }
		}
        /// <summary>
        /// Get Row's and Column's PivotValues
        /// </summary>
        /// <param name="row">int</param>
        /// <param name="column">int</param>
        /// <param name="calcFieldName">string</param>
        /// <returns></returns>
		public List<IComparable> GetRowColumnPivotValuesAt(int row, int column, out string calcFieldName)
		{
			calcFieldName = GetCalculationNameFromGridRowColumnIndex(column);
			List<IComparable> list = this.GetKeyAt(row, column);
			return list;
		}
 
		string GetCalculationNameFromGridRowColumnIndex(int index)
		{
			int index1 = index - (ShowCalculationsAsColumns ? this.PivotRows.Count : this.PivotColumns.Count);
			string name = this.PivotCalculations.Count > 0 && index1 >= 0 ? this.PivotCalculations[index1 % this.PivotCalculations.Count].FieldName : "";
			return name;
		}
        /// <summary>
        /// Gets the List of Raw Items 
        /// </summary>
        /// <param name="row">int</param>
        /// <param name="col">int</param>
        /// <returns></returns>
        public List<object> GetRawItemsFor(int row, int col)
        {
            if (CacheRawValues)
            {
                return GetCachedRawValues(row, col); //cached lookups
            }

            int col1;
            List<object> list1 = new List<object>();
            if (((this.rowHeaders.Length == 0)) && row == RowCount - 2)
            {
                for (int i = 0; i < PivotCalculations.Count; i++)
                {
                    if (col == this.columnHeaders.GetLength(1) - i)
                    {
                        IList data = (this.DataSource as IList == null) ? this.DataSourceList as IList : this.DataSource as IList;
                        foreach (object item in data)
                        {
                            list1.Add(item);
                        }
                        return list1;
                    }
                }
            }
            else if (row == RowCount - 2)
            {
                for (int i = 0; i < PivotCalculations.Count; i++)
                {
                    if (col == this.columnHeaders.GetLength(1) - i)
                    {
                        IList data = (this.DataSource as IList == null) ? this.DataSourceList as IList : this.DataSource as IList;
                        foreach (object item in data)
                        {
                            list1.Add(item);
                        }
                        return list1;
                    }
                }
            }

            if (IsRowSummary(row) && !IsGrandTotalCell(row, col) && !IsSummaryColumn(col))
            {
                int c;
                for (c = 0; c < pivotRows.Count; c++)
                {
                    if (rowHeaders[row, c] == null)
                    {
                        row = row - (pivotRows.Count - c);
                        break;
                    }
                }
                if (row > c)
                {
                    while (row >= c && !IsRowSummary(row))
                    {
                        foreach (var item in GetRawItemsForEach(row, col))
                        {
                            list1.Add(item);
                        }
                        row--;
                    }
                }
                else
                {
                    while (row <= c && !IsRowSummary(row))
                    {
                        foreach (var item in GetRawItemsForEach(row, col))
                        {
                            list1.Add(item);
                        }
                        row++;
                    }

                }
            }

            else if (IsSummaryColumn(col) && !IsGrandTotalCell(row, col))
            {
                int c;
                if (IsRowSummary(row))
                {
                    for (c = 0; c < pivotRows.Count; c++)
                    {
                        if (rowHeaders[row, c] == null)
                        {
                            row = row - (pivotRows.Count - c);
                            break;
                        }
                    }
                    col = col - (pivotCalculations.Count);
                    col1 = col;
                    while (row >= c && !IsRowSummary(row))
                    {
                        col = col1;
                        while (col >= pivotColumns.Count && !IsSummaryColumn(col))
                        {
                            foreach (var item in GetRawItemsForEach(row, col))
                            {
                                list1.Add(item);
                            }
                            col = col - (pivotCalculations.Count);
                        }
                        row--;
                    }
                }

                else
                {
                    col = col - (pivotCalculations.Count);
                    while (col >= pivotRows.Count && !IsSummaryColumn(col))
                    {
                        foreach (var item in GetRawItemsForEach(row, col))
                        {
                            list1.Add(item);
                        }
                        col = col - (pivotCalculations.Count);
                    }
                }
            }

            else if (IsGrandTotalCell(row, col))
            {
                col1 = col;
                if ((pivotValues[row, col].CellType == (PivotCellType.GrandTotalCell | PivotCellType.ValueCell)) && IsRowSummary(row) && IsSummaryColumn(col))
                {
                    row = row - 1;
                    if (col >= PivotValues.GetLength(1) - (pivotCalculations.Count + 1))
                    {
                        col = col - (pivotCalculations.Count);
                        col1 = col;
                        while (row >= (pivotColumns.Count + pivotCalculations.Count < 1 ? 0 : 1))
                        {
                            col = col1;
                            while (col >= pivotRows.Count && !IsRowSummary(row))
                            {
                                if (IsSummaryColumn(col))
                                    col = col - (pivotCalculations.Count);
                                foreach (var item in GetRawItemsForEach(row, col))
                                {
                                    list1.Add(item);
                                }
                                col = col - (pivotCalculations.Count);
                            }
                            row--;
                        }
                    }
                    else
                    {
                        col = col - (pivotCalculations.Count);
                        col1 = col;
                        while (row >= (pivotColumns.Count))
                        {
                            col = col1;
                            while (col >= pivotRows.Count && !IsSummaryColumn(col) && !IsRowSummary(row))
                            {

                                foreach (var item in GetRawItemsForEach(row, col))
                                {
                                    list1.Add(item);
                                }
                                col = col - (pivotCalculations.Count);
                            }
                            row--;
                        }
                    }
                }

                else if ((pivotValues[row, col].CellType == (PivotCellType.TotalCell | PivotCellType.ValueCell | PivotCellType.GrandTotalCell)) && IsRowSummary(row))
                {
                    int c;
                    for (c = 0; c < pivotRows.Count; c++)
                    {
                        if (rowHeaders[row, c] == null)
                        {
                            row = row - (pivotRows.Count - c);
                            break;
                        }
                    }
                    if (row >= c)
                    {
                        for (; row >= c && !IsRowSummary(row); row--)
                        {
                            col = col1;
                            for (col = col - (pivotCalculations.Count - c); col > ((PivotCalculations.Count > 1) ? PivotCalculations.Count : 0); col = col - PivotCalculations.Count)
                            {
                                while (!IsSummaryColumn(col) && col >= pivotColumns.Count)
                                {
                                    foreach (var item in GetRawItemsForEach(row, col))
                                    {
                                        list1.Add(item);
                                    }

                                    col = col - PivotCalculations.Count;
                                }

                            }
                        }
                    }

                    else
                    {

                        while (col >= pivotColumns.Count - 1)
                        {
                            foreach (var item in GetRawItemsForEach(row, col))
                            {
                                list1.Add(item);
                            }
                            col = col - (pivotCalculations.Count);
                        }
                    }
                }

                else if ((pivotValues[row, col].CellType == (PivotCellType.GrandTotalCell | PivotCellType.ValueCell)) && IsRowSummary(row))
                {
                    for (row = row - 1; row > pivotRows.Count - col; row--)
                    {
                        if (!IsRowSummary(row))
                            foreach (var item in GetRawItemsForEach(row, col))
                            {
                                list1.Add(item);
                            }
                    }
                }

                else
                {
                    while (col >= pivotColumns.Count)
                    {
                        foreach (var item in GetRawItemsForEach(row, col))
                        {
                            list1.Add(item);
                        }
                        col = col - (pivotCalculations.Count);
                    }
                }
            }

            else
            {
                list1 = GetRawItemsForEach(row, col);
            }


            return list1;
        }

        private List<object> GetCachedRawValues(int row, int col)
        {
            List<object> list = null;
            if (!(IsRowSummaryWhileOnDemand(row) && HiddenPivotRowGroups.Count > 0) && !(IsSummaryColumnWhileOnDemand(col) && HiddenPivotColumnGroups.Count > 0))
                list = this[row, col].RawValues;
            if (list == null)
            {
                int loc = tableKeysCalcValues.BinarySearch(new KeysCalculationValues() { Keys = GetKeyAt(row, col, true) });
                if (loc > -1)
                {
                    list = ((KeysCalculationValues)tableKeysCalcValues[loc]).RawValues;
                }
                else if(this.pivotValues[row,col].Value != null)
                {
                    HashSet<object> innerList = new HashSet<object>();
                    int cutOff = ShowCalculationsAsColumns ? PivotRows.Count : PivotRows.Count + 1;
                    list = new List<object>();
                    bool isSumRow = IsRowSummaryWhileOnDemand(row);
                    bool isSumCol = IsSummaryColumnWhileOnDemand(col);
                    if (isSumRow && !isSumCol)
                    {
                        int dec = Math.Max(1, PivotCalculations.Count);
                        int bottom = row;
                        if (showCalculationsAsColumns)
                            bottom--;
                        else
                            while (((bottom - colOffSet) % dec) != 0 && bottom > colOffSet) bottom--;
                        int left = PivotRows.Count - 1;
                        int inc = 0;
                        if (!showCalculationsAsColumns)
                            inc = (row - pivotColumns.Count) % pivotCalculations.Count;
                        while (left > 0 && (this[row - inc, left].CellRange == null || this[row - inc, left].UniqueText == "x")) { left--; }
                        PivotCellInfo info = null;
                        bool isGrandTotal = (this[row, left].CellType & PivotCellType.GrandTotalCell) != 0;
                        bool done = false;
                        while (bottom >= PivotColumns.Count + ((showCalculationsAsColumns && /*PivotCalculations.Count > 1*/ IsCalculationHeaderVisible) ? 1 : 0)
                                                            && (((info = this[bottom, left]) != null && !done) || isGrandTotal))
                        {
                            if (HiddenPivotRowGroups.Values.Any(x => x.Any(n => bottom >= n.From && bottom <= n.To)))
                            {
                                if (showCalculationsAsColumns)
                                    bottom--;
                                else
                                    bottom -= dec;
                                continue;
                            }
                            done = (this[bottom, left].CellType & PivotCellType.ExpanderCell) != 0;
                            info = this[bottom, col];
                            if (info.CellType == PivotCellType.ValueCell) //include only the simple value cells, not the valuecells|totalcells
                            {
                                List<object> rawItems = GetRawItemsFor(bottom, col);
                                if (rawItems != null)
                                {
                                    foreach (object t in rawItems)
                                        innerList.Add(t);
                                }
                            }
                            if (showCalculationsAsColumns)
                                bottom--;
                            else
                                bottom -= dec;
                        }
                        list.AddRange(innerList);
                    }
                    else if (!isSumRow && isSumCol)
                    {
                        int dec = Math.Max(1, PivotCalculations.Count);
                        int right = col;
                        if (showCalculationsAsColumns)
                            while (((right - colOffSet) % dec) != 0 && right > colOffSet) { right--; }
                        int top = PivotColumns.Count - 1;
                        while (top > 0 && (this[top, right].CellRange == null || this[top, right].UniqueText == "x")) { top--; }
                        if (showCalculationsAsColumns)
                            right -= dec;
                        PivotCellInfo info = null;
                        bool isGrandTotal = (this[top, col].CellType & PivotCellType.GrandTotalCell) != 0;
                        bool done = false;
                        while (right >= cutOff /* this.colOffSet */ && (((info = this[top, right]) != null && !done) || isGrandTotal))
                        {
                            if (HiddenPivotColumnGroups.Values.Any(x => x.Any(n => right >= n.From && right <= n.To)))
                            {
                                if (showCalculationsAsColumns)
                                    right -= dec;
                                else
                                    right--;
                                continue;
                            }
                            done = (info.CellType & PivotCellType.ExpanderCell) != 0;
                            info = this[row, right];
                            if (info.CellType == PivotCellType.ValueCell) //include only the simple value cells, not the valuecells|totalcells
                            {
                                List<object> rawItems = GetRawItemsFor(row, right);
                                if (rawItems != null)
                                {
                                    foreach (object t in rawItems)
                                        innerList.Add(t);
                                }
                            }
                            if (showCalculationsAsColumns)
                                right -= dec;
                            else
                                right--;
                        }
                        list.AddRange(innerList);

                    }
                    else //both are summaries
                    {
                        int dec = Math.Max(1, PivotCalculations.Count);
                        int right = col;
                        if (showCalculationsAsColumns)
                            while (((right - cutOff /* this.colOffSet */) % dec) != 0 && right > cutOff /* this.colOffSet */) { right--; }
                        int top = PivotColumns.Count - 1;
                        while (top > 0 && (this[top, right].CellRange == null || this[top, right].UniqueText == "x")) { top--; }
                        if (showCalculationsAsColumns)
                            right -= dec;
                        else
                            right--;
                        PivotCellInfo info = null;
                        bool isGrandTotal = (this[top, col].CellType & PivotCellType.GrandTotalCell) != 0;
                        bool done = false;
                        while (right >= cutOff /* this.colOffSet */ && (((info = this[top, right]) != null && !done) || isGrandTotal))
                        {
                            if (HiddenPivotColumnGroups.Values.Any(x => x.Any(n => right >= n.From && right <= n.To)))
                            {
                                if (showCalculationsAsColumns)
                                    right -= dec;
                                else
                                    right--;
                                continue;
                            }
                            done = (info.CellType & PivotCellType.ExpanderCell) != 0;
                            info = this[row, right];
                            bool needToInclude = false;
                            for (int i = 0; i < row; i++) //the ,= is needed to make sure grandtotals are properly handled.
                            {
                                if (row - i >= 0 && (info = this[row - i, right]) != null && info.CellType == PivotCellType.ValueCell
                                    && (!HiddenPivotRowGroups.Values.Any(x => x.Any(n => row >= n.From && row <= n.To))))
                                {
                                    while (right > this.pivotRows.Count + 1 && this.pivotValues[row, right].Value == null)
                                    {
                                        right--;
                                        while (right > this.pivotRows.Count + 1 && this.pivotValues[row - i, right].CellType != PivotCellType.ValueCell)
                                            right--;
                                    }
                                    needToInclude = true;
                                    break;
                                }
                            }
                            if (needToInclude) //include only the simple value cells, not the valuecells|totalcells
                            {
                                List<object> rawItems = GetRawItemsFor(row, right);
                                if (rawItems != null)
                                {
                                    foreach (object t in rawItems)
                                        innerList.Add(t);
                                }
                            }
                            if (showCalculationsAsColumns)
                                right -= dec;
                            else
                                right--;
                        }
                        list.AddRange(innerList);
                    }
                }
                this[row, col].RawValues = list;
            }
            return list;
        }
		
		/// <summary>
		/// Gets the List collection for Raw Items
		/// </summary>
		/// <param name="row">int</param>
		/// <param name="col">int</param>
		/// <returns></returns>
		public List<object>

              GetRawItemsForEach(int row, int col)
        {

            if (this.PivotCalculations.Count <= 1 && row > 0 && this.PivotColumns.Count == 0)
                row--;

            List<object> list = new List<object>();

            string unused = "";
            List<IComparable> pivotValues = GetRowColumnPivotValuesAt(row, col, out unused);
            IList data = (this.DataSource as IList == null) ? this.DataSourceList as IList : this.DataSource as IList;
            if (data != null)
            {
                object o1 = null;
                foreach (object o in data)
                {
                    bool b = true;
                    int loc = 0;
#if !SILVERLIGHT
                    PropertyDescriptorCollection pdc = null;
                    if (data is ITypedList)
                    {
                        pdc = ((ITypedList)data).GetItemProperties(null);
                    }
                    else if (data.Count > 0)
                    {
                        object item = data[0];
                        if (item is ICustomTypeDescriptor)
                        {
                            pdc = ((ICustomTypeDescriptor)item).GetProperties();
                        }
                        else
                        {
                            pdc = TypeDescriptor.GetProperties(item.GetType());
                        }
                    }

                    foreach (PivotItem pi in this.PivotRows)
                    {
                        o1 = pdc[pi.FieldMappingName].GetValue(o);
                        if (o1 != null)
                        {
                            if (pi.Format != null && pi.Format.Length > 0)
                            {
                                string format = "{0:" + pi.Format + "}";
                                o1 = string.Format(format, o1);
                            }
                            else
                                o1 = o1.ToString();
                        }

                        if (pivotValues.Count > 0 && ((o1 != null && !o1.Equals(pivotValues[loc])) || (o1 == null && pivotValues[loc] != null)))
                        {
                            b = false;
                            break;
                        }
                        loc++;

                    }
                    if (b)
                    {
                        foreach (PivotItem pi in this.PivotColumns)
                        {
                            o1 = pdc[pi.FieldMappingName].GetValue(o);
                            if (o1 != null)
                            {
                                if (pi.Format != null && pi.Format.Length > 0)
                                {
                                    string format = "{0:" + pi.Format + "}";
                                    o1 = string.Format(format, o1);
                                }
                                else
                                    o1 = o1.ToString();
                            }
                            if (pivotValues.Count > 0 && pivotValues.Count>loc && ((o1 != null && !o1.Equals(pivotValues[loc])) || (o1 == null && pivotValues[loc] != null)))


#else

           
                   object[] colPDs = ProcessList(PivotColumns);
                   object[] rowPDs = ProcessList(PivotRows);
                   for(int k=0;k<PivotRows.Count;k++)
                   {
                    if (rowPDs[k] is PropertyInfo)
                        {
                            PropertyInfo info = rowPDs[k] as PropertyInfo;
                            o1 = info.GetValue(o) as IComparable;
                        }
                        else if (rowPDs[k] is ExpressionPropertyDescriptor)
                        {
                            ExpressionPropertyDescriptor expDesc = rowPDs[k] as ExpressionPropertyDescriptor;
                            o1 = expDesc.GetValue(o) as IComparable;
                        }
                       if(o1!=null)
                       {
                           o1=o1.ToString();
                       }
                       if ((o1 != null && !o1.Equals(pivotValues[loc])) || (o1 == null && pivotValues[loc] != null))
                                {
                                    b = false;
                                    break;
                                }
                                loc++;

                            }
                            if (b)
                            {
                                for (int k=0;k<PivotColumns.Count;k++)
                                {
                                    if (colPDs[k] is PropertyInfo)
                                     {
                                           PropertyInfo info = colPDs[k] as PropertyInfo;
                                           o1 = info.GetValue(o) as IComparable;
                                    }
                                    else if (colPDs[k] is ExpressionPropertyDescriptor)
                                    {
                                           ExpressionPropertyDescriptor expDesc = colPDs[k] as ExpressionPropertyDescriptor;
                                           o1 = expDesc.GetValue(o) as IComparable;
                                    }
                                  
                                    if ((o1 != null && !o1.Equals(pivotValues[loc])) || (o1 == null && pivotValues[loc] != null))
#endif
                            {
                                b = false;
                                break;
                            }
                            loc++;
                        }
                    }

                    if (b)
                    {
                        list.Add(o);
                    }
                }


            }


            return list;
        }  
        /// <summary>
        /// Removes the Allowed field from grid
        /// </summary>
        /// <param name="fi"></param>
	  public void RemoveAllowedField(FieldInfo fi)
		{
			if (AllowedFields.Remove(fi))
			{
				RaisePivotSchemaChangedEvent(new PivotSchemaChangedArgs());
			}
			else
			{
				throw new ArgumentException(string.Format("{0} not in collection.", fi.Name));
			}
		}
        /// <summary>
        ///  populates the default property field
        /// </summary>
		public void PopulateDefaultPropertyFields()
		{
			AllowedFields.Clear();
#if !SILVERLIGHT
			foreach (PropertyDescriptor pd in ItemProperties)
			{
				AllowedFields.Add(new FieldInfo() { Name = pd.Name });
			}
#else
			foreach (string name in ItemProperties.Keys)
			{
				AllowedFields.Add(new FieldInfo() { Name = name });
			}
#endif
			RaisePivotSchemaChangedEvent(new PivotSchemaChangedArgs());
		}

    /// <summary>
    /// Clears the allowed fields
    /// </summary>
		public void ClearAllowedFields()
		{
			AllowedFields.Clear();
			RaisePivotSchemaChangedEvent(new PivotSchemaChangedArgs());
		}
        private bool lockComputations = false;
        /// <summary>
        /// Gets or sets the boolean value to indicate whether the computation need to happen. 
        /// It is handled in suspend and resume computation method
        /// </summary>
        public bool LockComputations
        {
            get { return lockComputations; }
            set { lockComputations = value; }
        }
        
		
		private const double defaultSummaryValue = 0.0d;

		/// <summary>
		/// Suspends calculations as pivot items are added or removed from either the RowPivots or ColumnPivots 
		/// collections, or when pivot calculations are added or removed from the PivotCalcualtions collection.
		/// </summary>
		/// <remarks>
		/// By default, calculations are always suspended. Once you have populated all the appropriate collections
		/// to define your pivot table, you call <see cref="Populate"/> to tell the engine to generate the pivot
		/// table contents that you can access by indexing the engine.
		/// </remarks>
		public void SuspendComputations()
		{
			this.lockComputations = true;
		}

		/// <summary>
		/// Resumes calculations after a call to SuspendComputations without resetting the pivot table.
		/// </summary>
		public void ResumeComputations()
		{
			ResumeComputations(false,false);
        }
        /// <summary>
        /// Resumes calculations after a call to SuspendComputations with the option of resetting the pivot table.
        /// </summary>
        /// <param name="resetPivotCollections">True if the pivot table should be recomputed, false otherwise.</param>
        public void ResumeComputations(bool resetPivotCollections)
        {
            ResumeComputations(true, true);
        }
        /// <summary>
        /// Resumes calculations after a call to SuspendComputations with the option of resetting the pivot table.
        /// </summary>
        /// <param name="resetPivotCollections">True if the pivot table should be recomputed, false otherwise.</param>
		/// <param name="shouldRefresh">True if the pivot grid should be refreshed, false otherwise</param>
		public void ResumeComputations(bool resetPivotCollections,bool shouldRefresh)
		{
            if (resetPivotCollections)
            {
                bool saveShowGrandTotal = this.ShowGrandTotals;
                if (!ShowCalculationsAsColumns)
                {
                    this.showGrandTotals = true;
                    SwapRowsColumns(true);
                }
                PopulatePivotTable();

                if (EmptyPivot)
                {
                    _populationCompleted = true;
                    return;
                }
                CalculateValues();
                PopulatePivotGridControl();
                if (this.GridLayout == GridLayout.TopSummary && !this.RowPivotsOnly)
                {
                    ReArrangePivotValuesForRows();
                    ReArrangePivotValuesForColumn();
                }
                if (!ShowCalculationsAsColumns)
                {
                    TransposePivotTable();
                    SwapRowsColumns(false);
                    if (!saveShowGrandTotal)
                        this.showGrandTotals = saveShowGrandTotal;
                }
                if (EnableOnDemandCalculations)
                {
                    SetSummands();
                }
                RemoveDelimeter();
                if (!RowPivotsOnly)
                {
                    if (shouldRefresh && !this.LoadInBackground)
                    {
                        this.lockComputations = false;
                        RaisePivotSchemaChangedEvent(new PivotSchemaChangedArgs() { ChangeHints = SchemaChangeHints.GrandTotalVisibility });
                    }
                }
                else if (RowPivotsOnly && this.lockComputations)
                {
                    if(shouldRefresh)
                    {
                        this.lockComputations = false;
                        RaisePivotSchemaChangedEvent(new PivotSchemaChangedArgs() { ChangeHints = SchemaChangeHints.GrandTotalVisibility });
                    }
                }

            }
		}

		/// <summary>
		/// Use this method to populate the values of the current pivot table.
		/// </summary>
		/// <remarks>
		/// Once you have populated the PivotColumns, PivotRows, and PivotCalculations collections, you
		/// call this method to actually populate this engine with the pivot calculations. After this call,
		/// you can then access information on the contents of any cell by indexing the engine object. The 
		/// indexers are zero based.
		/// </remarks>
		public void Populate()
		{
            bool saveEnableOnDemandCalculations = this._enableOnDemandCalculations;
            if (_enableOnDemandCalculations && (PivotCalculations.Count == 0 || PivotCalculations.Any(o => o.CalculationType != CalculationType.NoCalculation && o.CalculationType != CalculationType.Formula) == true))
            {
                _enableOnDemandCalculations = false;
            }

			if (UseIndexedEngine)
			{
				if (indexEngine == null)
				{
					indexEngine = new IndexEngine(this);
				}
				indexEngine.GetValue = this.GetValue;
				indexEngine.DataSource = this.DataSource;
				this.CoveredRanges.Clear();
				indexEngine.IndexData(this.EnableOnDemandCalculations);
				this.rowCount = indexEngine.RowCount + 1;
				this.columnCount = indexEngine.ColumnCount + 1;
                _enableOnDemandCalculations = saveEnableOnDemandCalculations;
				return;
			
			}

		   this.sortKeys = null;
			DateTime start = DateTime.Now;

			_populationCompleted = false;

			CoveredRanges.Clear();
            if (!lockComputations)
            {
                ResumeComputations(true);
            }
			if (RowPivotsOnly)
			{
				this.columnIndexes = Enumerable.Range(0, ColumnCount - 1).ToList();
			}

            highWaterRowIndex = this.rowOffSet;
            highWaterColumnIndex = this.colOffSet;

			_populationCompleted = true;
            _enableOnDemandCalculations = saveEnableOnDemandCalculations;
#if DEBUG
			IndexEngine.IndexTimeSlice = DateTime.Now.Subtract(start);
#endif
		}

		/// <summary>
		/// Resets the pivot table and all its collections to an empty status.
		/// </summary>
		public void Reset()
		{
			this.lockComputations = true;
			this.sortKeys = null;
			this.DataSource = null;
			this.DataSourceList = null;
			this.PivotCalculations.Clear();
			this.PivotColumns.Clear();
			this.PivotRows.Clear();
			this.Filters.Clear();


			valuesArea = null;
			rowHeaders = null;
			columnHeaders = null;
			this.rowCount = 0;
			this.columnCount = 0;
			this.CoveredRanges.Clear();
			this.lockComputations = false;
		}

		private void ResetForSwapRowsColumns(bool reset)
		{
			this.lockComputations = true;
			this.PivotColumns.Clear();
			this.PivotRows.Clear();

			valuesArea = null;
			rowHeaders = null;
			columnHeaders = null;
			this.rowCount = 0;
			this.columnCount = 0;
			if (reset)
				this.CoveredRanges.Clear();
			this.lockComputations = false;
		}
		/// <summary>
		/// Adds a calculation to the PivotCalculations collection.
		/// </summary>
		/// <param name="info"></param>
		public void AddPivotCalculation(PivotComputationInfo info)
		{
            if (NotPopulated.HasValue && NotPopulated.Value)
                return;

			PivotCalculations.Add(info);
			RaisePivotSchemaChangedEvent(new PivotSchemaChangedArgs());
		}

		/// <summary>
		/// Inserts the pivot calculation at a specified index
		/// </summary>
		/// <param name="index">The index.</param>
		/// <param name="info">The info.</param>        
		public void InsertPivotCalculation(int index, PivotComputationInfo info)
		{
            if (NotPopulated.HasValue && NotPopulated.Value)
                return;

			PivotCalculations.Insert(index, info);
			RaisePivotSchemaChangedEvent(new PivotSchemaChangedArgs());
		}

		/// <summary>
		/// Removes a calculation from the PivotCalculations collection.
		/// </summary>
		/// <param name="info">The PivotComputationInfo object to be removed.</param>
		public void RemovePivotCalculation(PivotComputationInfo info)
		{
            if (NotPopulated.HasValue && NotPopulated.Value)
                return;

			PivotCalculations.Remove(info);
		    RaisePivotSchemaChangedEvent(new PivotSchemaChangedArgs());
		}

		/// <summary>
		/// Adds a PivotItem to the PivotRows collection.
		/// </summary>
		/// <param name="pivotItem">The item to be added.</param>
		public void AddRowPivot(PivotItem pivotItem)
		{
            if (NotPopulated.HasValue && NotPopulated.Value)
                return;

			if (PivotRows == null)
			{
				PivotRows = new List<PivotItem>();
			}

			PivotRows.Add(pivotItem);
		    RaisePivotSchemaChangedEvent(new PivotSchemaChangedArgs());
		}

		/// <summary>
		/// Inserts a PivotItem to the PivotRows collection to a specified index
		/// </summary>
		/// <param name="index">The index.</param>
		/// <param name="pivotItem">The pivot item.</param>
		public void InsertRowPivot(int index, PivotItem pivotItem)
		{
            if (NotPopulated.HasValue && NotPopulated.Value)
                return;
			
			if (PivotRows == null)
			{
				PivotRows = new List<PivotItem>();
			}

			PivotRows.Insert(index, pivotItem);
			RaisePivotSchemaChangedEvent(new PivotSchemaChangedArgs());
		}

		/// <summary>
		/// Removes a PivotItem from the PivotRows collection.
		/// </summary>
		/// <param name="pivotItem">The item to be removed.</param>
		public void RemoveRowPivot(PivotItem pivotItem)
		{
            if (NotPopulated.HasValue && NotPopulated.Value)
                return;

			PivotRows.Remove(pivotItem);
			RaisePivotSchemaChangedEvent(new PivotSchemaChangedArgs());
		}

		/// <summary>
		/// Adds a PivotItem to the PivotColumns collection.
		/// </summary>
		/// <param name="pivotItem">The item to be added.</param>
		public void AddColumnPivot(PivotItem pivotItem)
		{
			if (PivotColumns == null)
			{
				PivotColumns = new List<PivotItem>();
			}

            if (NotPopulated.HasValue && NotPopulated.Value)
                return;

			PivotColumns.Add(pivotItem);
		    RaisePivotSchemaChangedEvent(new PivotSchemaChangedArgs());
		}

		/// <summary>
		/// Inserts a PivotItem to the PivotColumns collection at a specified Index
		/// </summary>
		/// <param name="index">The index.</param>
		/// /// <param name="pivotItem">The item to be added.</param>
		public void InsertColumnPivot(int index, PivotItem pivotItem)
		{
			if (PivotColumns == null)
			{
				PivotColumns = new List<PivotItem>();
			}

            if (NotPopulated.HasValue && NotPopulated.Value)
                return;

			PivotColumns.Insert(index, pivotItem);
			RaisePivotSchemaChangedEvent(new PivotSchemaChangedArgs());
		}

		
		/// <summary>
		/// Removes a PivotItem from the PivotColumns collection.
		/// </summary>
		/// <param name="pivotItem">The item to be removed.</param>
		public void RemoveColumnPivot(PivotItem pivotItem)
		{
            if (NotPopulated.HasValue && NotPopulated.Value)
                return;

			PivotColumns.Remove(pivotItem);
			RaisePivotSchemaChangedEvent(new PivotSchemaChangedArgs());
		}

		#region private implementation methods

		//  This method calculates the pivot information using the contents of the PivotRows, PivotColumns and PivotCalculations collections.
		private void CalculateValues()
		{
            bool moreThanOneCalculation = /*PivotCalculations.Count > 1*/ IsCalculationHeaderVisible;
			int rowCount1 = GetRowCountInValuesArea();
			int colCount1 = GetColumnCountInValuesArea();


			//populate the row headers
			rowHeaders = new IComparable[rowCount1 + PivotColumns.Count + (moreThanOneCalculation ? 1 : 0), PivotRows.Count];
			rowHeaderUniqueValues = new string[rowCount1 + PivotColumns.Count + (moreThanOneCalculation ? 1 : 0), PivotRows.Count];
			PopulateRowHeaders();

			//populate the column headers
			int colHeaderRowCount = PivotColumns.Count + (moreThanOneCalculation ? 1 : 0);
			int colHeaderColCount = colCount1 + PivotRows.Count;
			columnHeaders = new IComparable[colHeaderRowCount, colHeaderColCount];
			columnHeaderUniqueValues = new string[colHeaderRowCount, colHeaderColCount];
			PopulateColumnHeaders();

			//get the value area row counts and create the value area
			valuesArea = new SummaryBase[rowCount1, colCount1];

			this.rowOffSet = PivotColumns.Count + (moreThanOneCalculation ? 1 : 0);
			this.colOffSet = PivotRows.Count;
			if (EnableOnDemandCalculations && colOffSet == 0 && PivotColumns.Count > 0 && PivotCalculations.Count > 0)
			{
				colOffSet = 1;
			}

			if (!EnableOnDemandCalculations)
			{
				DoCalculationTable();
			}
		}

		Dictionary<int, List<int>> colSummands = null;

		internal Dictionary<int, List<int>> ColSummands
		{
			get
			{
				if (colSummands == null)
					colSummands = new Dictionary<int, List<int>>();
				return colSummands;
			}
		}

		Dictionary<int, List<int>> rowSummands = null;

		internal Dictionary<int, List<int>> RowSummands
		{
			get
			{
				if (rowSummands == null)
					rowSummands = new Dictionary<int, List<int>>();
				return rowSummands;
			}
		}

		private void SetSummands()
		{
			ProcessColSums();
			ProcessRowSums();
		}

		void ProcessRowSums()
		{
			RowSummands.Clear();
			if (PivotRows.Count == 0)
				return;

			int col = 0;
            int row = Math.Max(1, PivotColumns.Count + (/*PivotCalculations.Count > 1*/ IsCalculationHeaderVisible ? 1 : 0));

			List<int>[] pendings = new List<int>[PivotRows.Count];
			for (int i = 0; i < PivotRows.Count; ++i)
				pendings[i] = new List<int>();

			int inc = 1;
			int level = 0;
            if (this.GridLayout != GridLayout.TopSummary)
            {
                while (row < RowCount - inc - 1)
                {
                    if (col < PivotRows.Count)
                    {
                        if (this.PivotValues[row, col] != null &&
                            0 == (this.PivotValues[row, col].CellType & PivotCellType.ExpanderCell))
                        { //inner most level
                            CoveredCellRange range = col > 0 ? this.PivotValues[row, col - 1].CellRange : new CoveredCellRange(0, 0, 0, 0);
                            for (int i = 0; i <= range.Bottom - range.Top; i += inc)
                            {
                                pendings[level].Add(i + row);
                            }
                            row += range.Bottom - range.Top + 1;

                            bool brk = false;
                            while (level > 0 && col > 0 && !brk)
                            {
                                col--;
                                level--;
                                brk = !(this.PivotValues[row, col] != null && 0 != (this.PivotValues[row, col].CellType & PivotCellType.TotalCell));
                                if (!brk)
                                {
                                    RowSummands.Add(row, new List<int>(pendings[level + 1]));
                                    pendings[level + 1].Clear();//clear lower level
                                    pendings[level].Add(row);
                                    row += inc;
                                }
                                else
                                {
                                    col++;
                                    level++;
                                }
                            }
                        }
                        else
                        {
                            col++;
                            level++;
                        }
                    }
                }
            }
             else
            {
                List<int> list = new List<int>();
                int j = 0;
                int n, start = n = Math.Max(1, PivotColumns.Count + (/*PivotCalculations.Count > 1*/ IsCalculationHeaderVisible ? 1 : 0));
                for (int i = start; i < this.RowCount; i++)
                {
                    if (this.PivotValues[i, 0] != null && this.PivotValues[i, 0].CellType == (PivotCellType.ExpanderCell | PivotCellType.RowHeaderCell))
                        list.Add(i);
                }
                if (PivotRows.Count > 3)
                    start = PivotRows.Count - 2;
                while (row < RowCount - inc - 1)
                {
                    if (col < PivotRows.Count)
                    {
                        if (this.PivotValues[row, col] != null &&
                                    0 == (this.PivotValues[row, col].CellType & PivotCellType.ExpanderCell))
                        {
                            CoveredCellRange range = col > 0 ? this.PivotValues[row, col - 1].CellRange : new CoveredCellRange(0, 0, 0, 0);
                            for (int i = 0; i <= range.Bottom - range.Top; i += inc)
                            {
                                pendings[level].Add(i + row);
                            }
                            int row1 = row - 1;
                            row += range.Bottom - range.Top + 1;
                            bool brk = false;
                            while (level > 0 && col > 0 && !brk)
                            {
                                col--;
                                level--;
                                brk = !(this.PivotValues[row1, col] != null && (this.PivotValues[row1, col].CellType == (PivotCellType.RowHeaderCell | PivotCellType.ExpanderCell)));
                                if (brk)
                                {
                                    brk = !(this.PivotValues[row, col] != null && ((this.PivotValues[row, col].CellType == (PivotCellType.RowHeaderCell | PivotCellType.ExpanderCell)) || (0 != (this.PivotValues[row, col].CellType & PivotCellType.GrandTotalCell))));
                                    if (!brk)
                                    {
                                        if (PivotRows.Count > 3 && this.PivotValues[start, 0] != null && ((this.PivotValues[start, 0].CellType == (PivotCellType.ExpanderCell | PivotCellType.RowHeaderCell)) || (0 != (this.PivotValues[start, 0].CellType & PivotCellType.GrandTotalCell))))
                                        {
                                            start = list[j];
                                            j++;
                                            RowSummands.Add(start, new List<int>(pendings[level + 1]));
                                            pendings[level + 1].Clear();//clear lower level
                                            pendings[level].Add(start);
                                            start = row + 1;
                                        }
                                        else
                                        {
                                            RowSummands.Add(start, new List<int>(pendings[level + 1]));
                                            pendings[level + 1].Clear();//clear lower level
                                            pendings[level].Add(start);
                                            start = row;
                                        }
                                    }
                                    else
                                    {
                                        col++;
                                        level++;
                                    }
                                }
                                else
                                {
                                    if (!brk)
                                    {
                                        RowSummands.Add(row1, new List<int>(pendings[level + 1]));
                                        pendings[level + 1].Clear();//clear lower level
                                        pendings[level].Add(row1);
                                        //  row += inc;
                                    }
                                    else
                                    {
                                        col++;
                                        level++;
                                    }
                                }
                            }
                        }
                        else
                        {
                            row++;
                            col++;
                            level++;
                        }
                    }
                }
            }
			RowSummands.Add(row, new List<int>(pendings[0]));
			pendings[0].Clear();//clear lower level
		}


        void ProcessColSums()
        {
            ColSummands.Clear();
            if (PivotColumns.Count == 0)
                return;

            int col = PivotRows.Count;
            int row = 0;
            List<int>[] pendings = new List<int>[PivotColumns.Count];
            for (int i = 0; i < PivotColumns.Count; ++i)
                pendings[i] = new List<int>();

            int inc = this.PivotCalculations.Count;
            int level = 0;
            if (this.GridLayout != GridLayout.TopSummary)
            {
                while (col < ColumnCount - inc - 1)
                {
                    if (row < PivotColumns.Count)
                    {
                        if (this.PivotValues[row, col] != null &&
                            0 == (this.PivotValues[row, col].CellType & PivotCellType.ExpanderCell))
                        { //inner most level

                            CoveredCellRange range = row > 0 ? this.PivotValues[row - 1, col].CellRange : new CoveredCellRange(0, 0, 0, 0);
                            for (int i = 0; i <= range.Right - range.Left; i += inc)
                            {
                                pendings[level].Add(i + col);
                            }
                            col += range.Right - range.Left + 1;

                            bool brk = false;
                            while (level > 0 && row > 0 && !brk)
                            {
                                row--;
                                level--;
                                brk = !(this.PivotValues[row, col] != null && 0 != (this.PivotValues[row, col].CellType & PivotCellType.TotalCell));
                                if (!brk)
                                {
                                    ColSummands.Add(col, new List<int>(pendings[level + 1]));
                                    pendings[level + 1].Clear();//clear lower level
                                    pendings[level].Add(col);
                                    col += inc;
                                }
                                else
                                {
                                    row++;
                                    level++;
                                }
                            }
                        }
                        else
                        {
                            row++;
                            level++;
                        }
                    }
                }
            }
            else
            {
                List<int> list = new List<int>();
                int j = 0;
                int n, start = n = col;
                // int n, start = n = Math.Max(1, PivotColumns.Count + (/*PivotCalculations.Count > 1*/ IsCalculationHeaderVisible ? 1 : 0));
                for (int i = start; i < this.ColumnCount; i++)
                {
                    if (this.PivotValues[0, i] != null && this.PivotValues[0, i].CellType == (PivotCellType.ExpanderCell | PivotCellType.ColumnHeaderCell))
                        list.Add(i);
                }
                if (PivotColumns.Count > 3)
                    start = (PivotColumns.Count - 3) * PivotCalculations.Count + 1;
                while (col < ColumnCount - inc - 1)
                {
                    if (row < PivotColumns.Count)
                    {
                        if (this.PivotValues[row, col] != null &&
                            0 == (this.PivotValues[row, col].CellType & PivotCellType.ExpanderCell))
                        {
                            CoveredCellRange range = row > 0 ? this.PivotValues[row - 1, col].CellRange : new CoveredCellRange(0, 0, 0, 0);
                            for (int i = 0; i <= range.Right - range.Left; i += inc)
                            {
                                pendings[level].Add(i + col);
                            }
                            int col1 = col - PivotCalculations.Count;
                            col += range.Right - range.Left + 1;

                            bool brk = false;
                            while (level > 0 && row > 0 && !brk)
                            {
                                row--;
                                level--;
                                brk = !(this.PivotValues[row, col1] != null && (this.PivotValues[row, col1].CellType == (PivotCellType.ExpanderCell | PivotCellType.ColumnHeaderCell) || 0 != (this.PivotValues[row, col1].CellType & PivotCellType.GrandTotalCell)));
                                if (brk)
                                {
                                    brk = !(this.PivotValues[row, col] != null && (this.PivotValues[row, col].CellType == (PivotCellType.ColumnHeaderCell | PivotCellType.ExpanderCell) || this.PivotValues[row, col].CellType == (PivotCellType.ColumnHeaderCell | PivotCellType.GrandTotalCell)));
                                    if (!brk)
                                    {
                                        if (PivotColumns.Count > 3 && this.PivotValues[0, start] != null && ((this.PivotValues[0, start].CellType == (PivotCellType.ExpanderCell | PivotCellType.ColumnHeaderCell)) || (0 != (this.PivotValues[0, start].CellType & PivotCellType.GrandTotalCell))))
                                        {
                                            start = list[j];
                                            j++;
                                            ColSummands.Add(start, new List<int>(pendings[level + 1]));
                                            pendings[level + 1].Clear();//clear lower level
                                            pendings[level].Add(start);
                                            start = col + PivotCalculations.Count;
                                        }
                                        else
                                        {
                                            ColSummands.Add(start, new List<int>(pendings[level + 1]));
                                            pendings[level + 1].Clear();//clear lower level
                                            pendings[level].Add(start);
                                            start = col;
                                        }
                                    }
                                    else
                                    {
                                        row++;
                                        level++;
                                    }
                                }
                                else
                                {
                                    if (!brk)
                                    {
                                        ColSummands.Add(col1, new List<int>(pendings[level + 1]));
                                        pendings[level + 1].Clear();//clear lower level
                                        pendings[level].Add(col1);
                                    }
                                    else
                                    {
                                        row++;
                                        level++;
                                    }
                                }
                            }
                        }
                        else
                        {
                            col = col + PivotCalculations.Count;
                            row++;
                            level++;
                        }
                    }
                }
            }
            ColSummands.Add(col, new List<int>(pendings[0]));
            pendings[0].Clear();//clear lower level
        }


		private void RemoveDelimeter()
		{
			if (pivotValues == null)
				return;

			for (int row = 0; row < PivotColumns.Count; ++row)
			{
				for (int col = 0; col < pivotValues.GetLength(1); ++col)
				{
					if (pivotValues[row, col] != null && pivotValues[row, col].FormattedText != null && pivotValues[row, col].FormattedText.IndexOf(delimiter) > -1)
					{
						if (string.IsNullOrEmpty(this.PivotColumns[row].TotalHeader))
						{
							if (pivotValues[row, col].Value != null)
							{
								pivotValues[row, col].Value = pivotValues[row, col].Value.ToString().Remove(pivotValues[row, col].Value.ToString().IndexOf(delimiter));
							}
							pivotValues[row, col].FormattedText = pivotValues[row, col].FormattedText.Remove(pivotValues[row, col].FormattedText.IndexOf(delimiter));
						}
						else
						{
							pivotValues[row, col].FormattedText = pivotValues[row, col].FormattedText.Replace(delimiter, " ");
						}
					}
				}
			}
			for (int row = 0; row < pivotValues.GetLength(0); ++row)
			{
				for (int col = 0; col < PivotRows.Count; ++col)
				{
					if (pivotValues[row, col] != null && pivotValues[row, col].FormattedText != null && pivotValues[row, col].FormattedText.IndexOf(delimiter) > -1)
					{
						if (string.IsNullOrEmpty(this.PivotRows[col].TotalHeader))
						{
							if (pivotValues[row, col].Value != null)
							{
								pivotValues[row, col].Value = pivotValues[row, col].Value.ToString().Remove(pivotValues[row, col].Value.ToString().IndexOf(delimiter));
							}
							pivotValues[row, col].FormattedText = pivotValues[row, col].FormattedText.Remove(pivotValues[row, col].FormattedText.IndexOf(delimiter));
						}
						else
						{
							pivotValues[row, col].FormattedText = pivotValues[row, col].FormattedText.Replace(delimiter, " ");
						}
					}
				}
			}
		}

		//populates the row headers from the PivotRows collection and the DataSource.
		private void PopulateRowHeaders()
		{
			if (rowKeysCalcValues == null)
				return;

            bool moreThanOneCalculation = /*PivotCalculations.Count > 1*/ IsCalculationHeaderVisible;
			int rowIndex = PivotColumns.Count + (moreThanOneCalculation ? 1 : 0);
			int lastCol = PivotRows.Count;
			KeysCalculationValues prevValue = null;
			int keyValue = 0;
			int lastNullCorrection = -1;
			string key = "";
            const char underScoreMarker = (char)129;
            foreach (KeysCalculationValues row in rowKeysCalcValues)
            {
                if (row.Keys == null && prevValue != null && prevValue.Keys != null) //total row to be done
                {
                    lastNullCorrection++;

					keyValue = prevValue.Keys.Count - 2 - lastNullCorrection;
					if (keyValue > -1)
					{
						key = prevValue.Keys[keyValue] + delimiter /* + "a" */ + PivotRows[keyValue].TotalHeader;
						rowHeaders[rowIndex, keyValue] = key;
						for (int temp = 0; temp < prevValue.Keys.Count - 1; temp++)
						{
							if (keyValue != temp)
							{
								rowHeaderUniqueValues[rowIndex, keyValue] = prevValue.Keys[temp] + underScoreMarker.ToString();
							}
							else
							{
								rowHeaderUniqueValues[rowIndex, keyValue] += key;
								break;
							}
						}
						//make sure cells above this have the proper value instead of null...
						for (int k = 0; k < keyValue; ++k)
						{
							rowHeaders[rowIndex, k] = prevValue.Keys[k];
						}
						for (int k = keyValue + 1; k < prevValue.Keys.Count; ++k)
						{
							rowHeaders[rowIndex, k] = null;
						}
						rowIndex++;
					}
				}
				else
				{
					lastNullCorrection = -1;
					prevValue = row;
					//    for (int k = 0; k < loopCount; ++k)
					{
						int columnIndex = 0;
						if (row.Keys != null)
						{
							//foreach (object o in row.Keys)
							for (int temp = 0; temp < row.Keys.Count; temp++)
							{
								//rowHeaders[rowIndex, columnIndex] = o as IComparable;
								rowHeaders[rowIndex, columnIndex] = row.Keys[temp] as IComparable;
								//  rowHeaders[rowIndex, columnIndex] = o == null ? "(null)" : o.ToString();

								for (int temp2 = 0; temp2 <= temp; temp2++)
								{
									if (temp2 == 0 && row.Keys[temp2] != null)
									{
										rowHeaderUniqueValues[rowIndex, columnIndex] = row.Keys[temp2].ToString();
									}
									else
									{
										rowHeaderUniqueValues[rowIndex, columnIndex] = rowHeaderUniqueValues[rowIndex, columnIndex] + "." + Convert.ToString(row.Keys[temp2]);
									}
								}
								columnIndex++;
							}
						}
						rowIndex++;
					}
				}
			}

			if (rowHeaders.GetLength(1) > 0 && rowIndex < rowHeaders.GetLength(0))
			{
				if (pivotRows[0].TotalHeader == null)
				{
					rowHeaders[rowIndex, 0] = GrandString + delimiter;
					rowHeaderUniqueValues[rowIndex, 0] = GrandString + delimiter;
				}
				else
				{
					rowHeaders[rowIndex, 0] = GrandString + delimiter /* + "b" */ + PivotRows[0].TotalHeader;
					rowHeaderUniqueValues[rowIndex, 0] = GrandString + delimiter /* + "b" */ + PivotRows[0].TotalHeader;
				}
				rowIndex++;
			}
		}

		//populates the column headers from the PivotColumns collection and the DataSource.
		private void PopulateColumnHeaders()
		{
			if (columnKeysCalcValues == null)
				return;

            bool moreThanOneCalculation = /*PivotCalculations.Count > 1*/ IsCalculationHeaderVisible;
			int columnIndex = PivotRows.Count;
			int loopCount = moreThanOneCalculation ? PivotCalculations.Count : 1;
			int lastRow = PivotColumns.Count;
			string lastSetHeader = string.Empty;
			KeysCalculationValues prevValue = null;
			int keyValue = 0;
			int lastNullCorrection = -1;
			string header = "";
			foreach (KeysCalculationValues col in columnKeysCalcValues)
			{
                if (col.Keys == null && prevValue != null) //total row to be done
                {
                    lastNullCorrection++;
                    if(prevValue.Keys != null)
                    for (int i = 0; i < loopCount; ++i)
                    {
                        keyValue = prevValue.Keys.Count - 2 - lastNullCorrection;
						if (keyValue > -1)
						{
							if (string.IsNullOrEmpty(Convert.ToString(prevValue.Keys[keyValue])))
							{
								header =  delimiter /* + "c" */ + PivotColumns[keyValue].TotalHeader;
								columnHeaders[keyValue, columnIndex] = header;
							}
                            else if ((!ShowCalculationsAsColumns && prevValue.Keys[keyValue] + delimiter /* + "c" */ + PivotColumns[keyValue].TotalHeader != lastSetHeader)
                                      || (ShowCalculationsAsColumns && PivotCalculations.Count > 0 && ((columnIndex - PivotRows.Count) % PivotCalculations.Count == 0)))
                            {
                                header = prevValue.Keys[keyValue] + delimiter /* + "c" */ + PivotColumns[keyValue].TotalHeader;
                                columnHeaders[keyValue, columnIndex] = header;
                                lastSetHeader = header;
                            }

							for (int temp = 0; temp < prevValue.Keys.Count - 1; temp++)
							{
								if (keyValue != temp)
								{
									columnHeaderUniqueValues[keyValue, columnIndex] = prevValue.Keys[temp] + ".";
								}
								else
								{
									columnHeaderUniqueValues[keyValue, columnIndex] += header;
									break;
								}
							}
							 
							
							//make sure cells above this have the proper value instead of null...
							for (int k = 0; k < keyValue; ++k)
							{
								columnHeaders[k, columnIndex] = prevValue.Keys[k];//.ToString();
							}
							if (moreThanOneCalculation)
							{
								columnHeaders[lastRow, columnIndex] = UseDescriptionInCalculationHeader ? PivotCalculations[i].Description : PivotCalculations[i].FieldHeader;
								columnHeaderUniqueValues[lastRow, columnIndex] = columnHeaderUniqueValues[keyValue, columnIndex] + delimiter + (UseDescriptionInCalculationHeader ? PivotCalculations[i].Description : PivotCalculations[i].FieldHeader);
								//columnHeaderUniqueValues[lastRow, columnIndex] = UseDescriptionInCalculationHeader ? PivotCalculations[i].Description : PivotCalculations[i].FieldHeader;                           
							}
							columnIndex++;
						}
					}
				}
				else
				{
					lastNullCorrection = -1;
					prevValue = col;
					for (int k = 0; k < loopCount; ++k)
					{
						int rowIndex = 0;
						if (col.Keys != null)
						{
							for (int temp = 0; temp < col.Keys.Count; temp++)
							{
								columnHeaders[rowIndex, columnIndex] = col.Keys[temp] as IComparable;
								for (int temp2 = 0; temp2 <= temp; temp2++)
								{
									if (temp2 == 0 && col.Keys[temp2] != null)
									{
										columnHeaderUniqueValues[rowIndex, columnIndex] = col.Keys[temp2].ToString();
									}
									else
									{
										columnHeaderUniqueValues[rowIndex, columnIndex] = columnHeaderUniqueValues[rowIndex, columnIndex] + "." + Convert.ToString(col.Keys[temp2]);
									}
								}
								rowIndex++;
							}
						}

						if (moreThanOneCalculation && columnIndex < columnHeaders.GetLength(1))
						{
							columnHeaders[rowIndex, columnIndex] = UseDescriptionInCalculationHeader ? PivotCalculations[k].Description : PivotCalculations[k].FieldHeader;
							//columnHeaderUniqueValues[rowIndex, columnIndex] = (rowIndex > 1 ? columnHeaderUniqueValues[rowIndex - 1, columnIndex] : string.Empty) + delimiter + (UseDescriptionInCalculationHeader ? PivotCalculations[k].Description : PivotCalculations[k].FieldHeader);
							columnHeaderUniqueValues[rowIndex, columnIndex] = UseDescriptionInCalculationHeader ? PivotCalculations[k].Description : PivotCalculations[k].FieldHeader;
						}
						columnIndex++;
					}
				}
			}
			for (int i = 0; i < loopCount && columnIndex < columnHeaders.GetLength(1); ++i)
			{
				if (PivotColumns.Count > 0)
				{
                    if (i == 0)
                    {
                        if (pivotColumns[0].TotalHeader == null)
                        {
                            columnHeaders[0, columnIndex] = GrandString + delimiter;
                            columnHeaderUniqueValues[0, columnIndex] = GrandString + delimiter;
                        }

                        else
                        {
                            columnHeaders[0, columnIndex] = GrandString + delimiter /* + "d" */ + PivotColumns[0].TotalHeader;
                            columnHeaderUniqueValues[0, columnIndex] = GrandString + delimiter /* + "d" */ + PivotColumns[0].TotalHeader;
                        }
                    }
                    else
                    {
                        if (pivotColumns[0].TotalHeader == null)
                        {
                            columnHeaderUniqueValues[0, columnIndex] = GrandString + delimiter;
                        }
                        else
                        {
                            columnHeaderUniqueValues[0, columnIndex] = GrandString + delimiter /* + "d" */ + PivotColumns[0].TotalHeader;
                        }
                    }
				}
				if (moreThanOneCalculation && columnIndex < columnHeaders.GetLength(1))
				{
					columnHeaders[lastRow, columnIndex] = UseDescriptionInCalculationHeader ? PivotCalculations[i].Description
																							: PivotCalculations[i].FieldHeader;
					columnHeaderUniqueValues[lastRow, columnIndex] = GrandString + delimiter + (UseDescriptionInCalculationHeader ? PivotCalculations[i].Description
																							: PivotCalculations[i].FieldHeader);
				}
				columnIndex++;
			}
		}

		private void TransposePivotTable()
		{
			int newColCount = RowCount;
			int newRowCount = ColumnCount;

			// PivotCellInfo[,] newPivotValues = new PivotCellInfo[newRowCount, newColCount];
			PivotCellInfos newPivotValues = new PivotCellInfos(newRowCount, newColCount);

			for (int row = 0; row < newRowCount; ++row)
			{
				for (int col = 0; col < newColCount; ++col)
				{
                    PivotCellInfo cellinfo = null;
                    newPivotValues[row, col] = pivotValues[col, row];
                    //changes the cell range according to the value of ShowCalculationAsColumn
                    if (pivotValues[col, row] != null && pivotValues[col, row].CellRange != null)
                    {
                        newPivotValues[row, col].CellRange = new CoveredCellRange(pivotValues[col, row].CellRange.Left, pivotValues[col, row].CellRange.Top, pivotValues[col, row].CellRange.Right, pivotValues[col, row].CellRange.Bottom);
                    }
                    cellinfo = newPivotValues[row, col];
					if (cellinfo != null)
					{
						if ((cellinfo.CellType & PivotCellType.RowHeaderCell) != 0)
						{
							cellinfo.CellType &= ~PivotCellType.RowHeaderCell;
							cellinfo.CellType |= PivotCellType.ColumnHeaderCell;
							
						}
						else if ((cellinfo.CellType & PivotCellType.ColumnHeaderCell) != 0)
						{
							cellinfo.CellType &= ~PivotCellType.ColumnHeaderCell;
							cellinfo.CellType |= PivotCellType.RowHeaderCell;
						}
					}
				}
			}

			rowCount = newRowCount;
			columnCount = newColCount;

			this.pivotValues = newPivotValues;

			List<CoveredCellRange> newCoveredRanges = new List<CoveredCellRange>();
			foreach (var range in CoveredRanges)
			{
				newCoveredRanges.Add(new CoveredCellRange(range.Left, range.Top, range.Right, range.Bottom));
			}
			this.coveredRanges = newCoveredRanges;
		}


		private void SwapRowsColumns(bool reset)
		{
			PivotItem[] tempCols = new PivotItem[PivotColumns.Count];
			PivotItem[] tempRows = new PivotItem[PivotRows.Count];
			PivotColumns.CopyTo(tempCols);
			PivotRows.CopyTo(tempRows);
			PivotColumns.Clear();
			PivotRows.Clear();
			if (reset)
				ResetForSwapRowsColumns(reset);
			PivotColumns.AddRange(tempRows);
			PivotRows.AddRange(tempCols);
		}

		string delimiter = new string((char)131, 1);//"_"; //special marker used to mark title being added to summary header.
		private SummaryBase[,] valuesArea = null;
		private IComparable[,] rowHeaders = null;
		private IComparable[,] columnHeaders = null;

		private string[,] rowHeaderUniqueValues;
		private string[,] columnHeaderUniqueValues;

		private int GetRowCountInValuesArea()
		{
		   //return rowKeysCalcValues == null ? 0 : (rowKeysCalcValues.Count == 1 && this.ShowGrandTotals ? rowKeysCalcValues.Count : rowKeysCalcValues.Count + 1); //when only 1, do not need grand total...
		   return rowKeysCalcValues == null ? 0 : (this.PivotRows.Count == 0 && this.ShowGrandTotals) ? rowKeysCalcValues.Count : rowKeysCalcValues.Count + 1;
		}

		private int GetColumnCountInValuesArea()
		{
		   //return columnKeysCalcValues == null ? 0 : (((columnKeysCalcValues.Count == 1 && this.ShowGrandTotals) ? columnKeysCalcValues.Count : columnKeysCalcValues.Count + 1)) * Math.Max(1, PivotCalculations.Count);
		   return columnKeysCalcValues == null ? 0 : ((this.PivotColumns.Count == 0 && this.ShowGrandTotals)?columnKeysCalcValues.Count:columnKeysCalcValues.Count + 1) * Math.Max(1, PivotCalculations.Count);
		}

		private void PopulatePivotGridControl()
		{
			//to remove grand totals, get rid of the 1's here - there is more work to be done for the column
			this.rowCount = rowHeaders.GetLength(0) + ((PivotRows.Count > 0 || PivotCalculations.Count > 0) ? 1 : 0);

			this.columnCount = columnHeaders.GetLength(1) + ((PivotColumns.Count > 0 || PivotCalculations.Count > 0) ? 1 : 0);
			// this.pivotValues = new PivotCellInfo[rowCount, columnCount];
			this.pivotValues = new PivotCellInfos(rowCount, columnCount);

			int valueColStart = rowHeaders.GetLength(1);
			int valueRowStart = columnHeaders.GetLength(0) + 1;

			for (int row = 0; row < rowHeaders.GetLength(0); ++row)
			{
				for (int col = 0; col < valueColStart; col++)
				{
					this.pivotValues[row, col] = new PivotCellInfo();
					if (row == 0 || (rowHeaders[row, col] != null && rowHeaders[row, col].CompareTo(rowHeaders[row - 1, col]) != 0))
					{
						this.pivotValues[row, col].Value = rowHeaders[row, col];
						this.pivotValues[row, col].FormattedText = rowHeaders[row, col] == null
														? null : rowHeaders[row, col].ToString();
					}

					this.pivotValues[row, col].UniqueText = rowHeaderUniqueValues[row, col];
                    if (rowHeaders[row, col] != null)
                        this.PivotValues[row, col].Key = rowHeaders[row, col].ToString();
                    else if (this.pivotValues[row, col].UniqueText != null)
                        this.PivotValues[row, col].Key = this.pivotValues[row, col].UniqueText;
                    else
                        this.PivotValues[row, col].Key = null;

				}
			}

			for (int row = 0; row < columnHeaders.GetLength(0); ++row)
			{
				for (int col = 0; col < columnHeaders.GetLength(1); col++)
				{
					if (this.pivotValues[row, col] == null)
					{
						this.pivotValues[row, col] = new PivotCellInfo();
					}
					if (col == 0 || (columnHeaderUniqueValues[row, col] != null))
				  
						 {
						this.pivotValues[row, col].Value = columnHeaders[row, col];
						this.pivotValues[row, col].FormattedText = columnHeaders[row, col] == null ?
																	 null : columnHeaders[row, col].ToString();
					}

                    this.pivotValues[row, col].UniqueText = columnHeaderUniqueValues[row, col];
                    if (columnHeaders[row, col] != null)
                        this.PivotValues[row, col].Key = columnHeaders[row, col].ToString();
                    else if (this.pivotValues[row, col].UniqueText != null)
                        this.PivotValues[row, col].Key = this.pivotValues[row, col].UniqueText;
                    else
                        this.PivotValues[row, col].Key = null;
                }
            }

			PopulateValueCells();

			CoverRowHeaders();
			CoverColumnHeaders();

			MarkGrandTotalCellType();


			//code to handle special cases of a partially populated schema 
			if (PivotColumns.Count == 0 && PivotRows.Count > 0 && PivotCalculations.Count <= 1)
			{
				//handle special case where no column pivots, but have 0 or 1 calculation and some row pivots.

				int rowCount1 = this.pivotValues.GetLength(0) + 1;
				int colCount1 = this.pivotValues.GetLength(1);
				List<CoveredCellRange> ranges = new List<CoveredCellRange>();
				PivotCellInfos cells = new PivotCellInfos(rowCount1, colCount1);
				for (int i = 1; i < rowCount1; ++i)
				{
					for (int j = 0; j < colCount1; ++j)
					{
						cells[i, j] = pivotValues[i - 1, j];
						if (cells[i, j] != null && cells[i, j].CellRange != null)
						{
							cells[i, j].CellRange = new CoveredCellRange(cells[i, j].CellRange.Top + 1, cells[i, j].CellRange.Left, cells[i, j].CellRange.Bottom + 1, cells[i, j].CellRange.Right);
							if (OkToAddRange(cells[i, j].CellRange, ranges))
								ranges.Add(cells[i, j].CellRange);
						}
					}
				}
				int offSet = pivotRows.Count;
				for (int j = offSet; j < offSet + PivotCalculations.Count; ++j)
				{
					cells[0, j] = new PivotCellInfo()
					{
						CellType = PivotCellType.CalculationHeaderCell | PivotCellType.ColumnHeaderCell,
						Value = UseDescriptionInCalculationHeader ? PivotCalculations[j - offSet].Description : PivotCalculations[j - offSet].FieldName,
						FormattedText = UseDescriptionInCalculationHeader ? PivotCalculations[j - offSet].Description : PivotCalculations[j - offSet].FieldHeader
					};
				}
				cells[0, 0] = new PivotCellInfo() { CellType = PivotCellType.TopLeftCell, CellRange = new CoveredCellRange(0, 0, 0, offSet - 1) };
				ranges.Add(cells[0, 0].CellRange);
				this.pivotValues = cells;
				rowCount += 1;
				this.coveredRanges = ranges;
			}
			else if (PivotColumns.Count == 0 && PivotRows.Count > 0 && PivotCalculations.Count > 0)
			{
				//handle special case with no pivot columns and more than 1 calculation
				int offSet = pivotRows.Count;
				for (int j = offSet; j < offSet + PivotCalculations.Count; ++j)
				{
                    if (j < pivotValues.GetLength(1) && pivotValues[0, j] != null)
                    {
                        pivotValues[0, j].CellType = PivotCellType.CalculationHeaderCell | PivotCellType.ColumnHeaderCell;
                    }
				}
			}
			else if (PivotColumns.Count > 0 && PivotRows.Count == 0 && PivotCalculations.Count > 0)
			{
				//handle special case where no row pivots, but have some calculations and some row pivots.

				int rowCount1 = this.pivotValues.GetLength(0);
				int colCount1 = this.pivotValues.GetLength(1) + 1;
				List<CoveredCellRange> ranges = new List<CoveredCellRange>();
				PivotCellInfos cells = new PivotCellInfos(rowCount1, colCount1);
				for (int i = 0; i < rowCount1; ++i)
				{
					for (int j = 1; j < colCount1; ++j)
					{
						cells[i, j] = pivotValues[i, j - 1];
						if (cells[i, j] != null && cells[i, j].CellRange != null)
						{
							cells[i, j].CellRange = new CoveredCellRange(cells[i, j].CellRange.Top, cells[i, j].CellRange.Left + 1, cells[i, j].CellRange.Bottom, cells[i, j].CellRange.Right + 1);
							if (OkToAddRange(cells[i, j].CellRange, ranges))
								ranges.Add(cells[i, j].CellRange);
						}
					}
				}

                cells[0, 0] = new PivotCellInfo() { CellType = PivotCellType.TopLeftCell, CellRange = new CoveredCellRange(0, 0, PivotColumns.Count - (/*PivotCalculations.Count > 1*/ IsCalculationHeaderVisible ? 0 : 1), 0) };
				ranges.Add(cells[0, 0].CellRange);
				for (int i = rowCount1 - 1; i > 0; --i)
				{
					cells[i, 0] = new PivotCellInfo() { CellType = PivotCellType.GrandTotalCell | PivotCellType.HeaderCell | PivotCellType.RowHeaderCell, FormattedText = GrandString + delimiter + PivotColumns[0].TotalHeader };

				}
				this.pivotValues = cells;
				columnCount += 1;
				this.coveredRanges = ranges;

			}
			else if (PivotColumns.Count == 0 && PivotRows.Count == 0 && PivotCalculations.Count > 0)
			{
				int rowCount1 = this.pivotValues.GetLength(0);
				int colCount1 = this.pivotValues.GetLength(1) + 1;
				List<CoveredCellRange> ranges = new List<CoveredCellRange>();
				PivotCellInfos cells = new PivotCellInfos(rowCount1, colCount1);
				for (int i = 0; i < rowCount1; ++i)
				{
					for (int j = 1; j < colCount1; ++j)
					{
						cells[i, j] = pivotValues[i, j - 1];
						if (cells[i, j] != null && cells[i, j].CellRange != null)
						{
							cells[i, j].CellRange = new CoveredCellRange(cells[i, j].CellRange.Top, cells[i, j].CellRange.Left + 1, cells[i, j].CellRange.Bottom, cells[i, j].CellRange.Right + 1);
							if (OkToAddRange(cells[i, j].CellRange, ranges))
								ranges.Add(cells[i, j].CellRange);
						}
					}
				}

				if (this.PivotCalculations.Count == 1)
				{
					cells[0, 0] = new PivotCellInfo() { CellType = PivotCellType.TopLeftCell, CellRange = new CoveredCellRange(0, 0, 0, 0) };
				}
				else
				{
                    cells[0, 0] = new PivotCellInfo() { CellType = PivotCellType.TopLeftCell, CellRange = new CoveredCellRange(0, 0, PivotColumns.Count - (/*PivotCalculations.Count > 1*/ IsCalculationHeaderVisible ? 0 : 1), 0) };
				}
				ranges.Add(cells[0, 0].CellRange);
				for (int i = rowCount1 - 1; i > 0; --i)
				{
					cells[i, 0] = new PivotCellInfo() { CellType = PivotCellType.GrandTotalCell | PivotCellType.HeaderCell | PivotCellType.RowHeaderCell, FormattedText = GrandString + delimiter + " Total" };
				}
				this.pivotValues = cells;
				columnCount += 1;
				this.coveredRanges = ranges;
			}            

			if (PivotColumns.Count > 0 || PivotRows.Count > 0)
			{  //make sure TopLeftCell is set...
				this.pivotValues[0, 0].CellType = PivotCellType.TopLeftCell;
			}
		}

		/// <summary>
		/// Populates the value cells.
		/// </summary>
		public void PopulateValueCells()
		{
			int valueColStart = rowHeaders.GetLength(1);
			int valueRowStart = columnHeaders.GetLength(0) + 1;
			int row1 = 0;
			int col1 = 0;
			string format = "";
			int k = 0;
			bool isSummaryRow = false;
			bool isSummaryColumn = false;
			int[] parentRowLocation = new int[2];
			int[] parentColumnLocation = new int[2];
			for (int row = 0; row < valuesArea.GetLength(0); ++row)
			{
				row1 = row + valueRowStart - 1;
				isSummaryRow = IsRowSummary(row1);
				for (int col = 0; col < valuesArea.GetLength(1); ++col)
				{
					col1 = col + valueColStart;
					isSummaryColumn = IsSummaryColumn(col1);
					if (this.pivotValues[row1, col1] == null)
					{
						this.pivotValues[row1, col1] = new PivotCellInfo();

					}
					if (col1 < pivotValues.GetLength(1) && row1 < pivotValues.GetLength(0))
					{
						if (valuesArea[row, col] != null)
						{
                            PivotComputationInfo compInfo = PivotCalculations.Count == 0 ? null : this.PivotCalculations[col % PivotCalculations.Count];
							object v = ((SummaryBase)valuesArea[row, col]).GetResult();
                            this.pivotValues[row1, col1].Summary = (SummaryBase)valuesArea[row, col];
                            format = "{0:" + compInfo.Format + "}";
                            if (format.Equals("{0:#.##}") && v != null && v.Equals(0.0d) && compInfo.CalculationType != CalculationType.Formula) ////Condition added since the format string string.Format({0:#.##},0.0) returns empty.
							{
								if (this.PivotCalculations[col % PivotCalculations.Count].CalculationType.ToString().Contains("Percentage"))
								{
									this.pivotValues[row1, col1].FormattedText = "0.00%";
								}
								else
								{
									this.pivotValues[row1, col1].FormattedText = "0.0";
								}
							}
							else
							{
								switch (this.PivotCalculations[col % PivotCalculations.Count].CalculationType)
								{
									case CalculationType.NoCalculation:
                                        if ((v != null && (!v.Equals(0.0d) && !v.Equals(0)) && ShowNullAsBlank && this.pivotValues[row1, col1].Value == null)|| this.pivotValues[row1,col1].Value != null  || !ShowNullAsBlank) 
                                            this.pivotValues[row1, col1].FormattedText = string.Format(CultureInfo.CurrentUICulture, format, v);
                                        this.pivotValues[row1, col1].Value = v;
						     			break;
									case CalculationType.PercentageOfGrandTotal:
										object grandTotal = valuesArea[valuesArea.GetLength(0) - 1, (valuesArea.GetLength(1) + ((col % this.PivotCalculations.Count) - this.PivotCalculations.Count))].GetResult();
										this.pivotValues[row1, col1].Value = (Convert.ToDouble(v) / Convert.ToDouble(grandTotal) * 100);
                                        if (!UsePercentageFormat)
                                           this.pivotValues[row1, col1].FormattedText = string.Format(CultureInfo.CurrentUICulture, format, ((double)this.pivotValues[row1, col1].Value));
                                        else
                                            this.pivotValues[row1, col1].FormattedText = ((double)this.pivotValues[row1, col1].Value).ToString("0.00", CultureInfo.CurrentUICulture) + "%";
                                        break;
									case CalculationType.PercentageOfColumnTotal:
										object colTotal = valuesArea[valuesArea.GetLength(0) - 1, col].GetResult();
										this.pivotValues[row1, col1].Value = (Convert.ToDouble(v) / Convert.ToDouble(colTotal) * 100);
                                        if (!UsePercentageFormat)
                                            this.pivotValues[row1, col1].FormattedText = string.Format(CultureInfo.CurrentUICulture, format, (double)this.pivotValues[row1, col1].Value);
                                        else
                                            this.pivotValues[row1, col1].FormattedText = ((double)this.pivotValues[row1, col1].Value).ToString("0.00", CultureInfo.CurrentUICulture) + "%";
										break;
									case CalculationType.PercentageOfRowTotal:
										object rowTotal = valuesArea[row, (valuesArea.GetLength(1) + ((col % this.PivotCalculations.Count) - this.PivotCalculations.Count))].GetResult();
										this.pivotValues[row1, col1].Value = (Convert.ToDouble(v) / Convert.ToDouble(rowTotal) * 100);
                                        if (!UsePercentageFormat)
                                            this.pivotValues[row1, col1].FormattedText = string.Format(CultureInfo.CurrentUICulture, format, (double)this.pivotValues[row1, col1].Value);
                                        else
                                            this.pivotValues[row1, col1].FormattedText = ((double)this.pivotValues[row1, col1].Value).ToString("0.00", CultureInfo.CurrentUICulture) + "%";
                                        break;
                                    case CalculationType.PercentageOfParentColumnTotal:
                                        object parentColTotal;
                                        if (isSummaryColumn)
                                        {
                                            parentColumnLocation = GetNextParentColumnIndex(parentColumnLocation[0], col1);
                                            parentColTotal = valuesArea[row, parentColumnLocation[1]].GetResult();
                                        }
                                        else
                                        {
                                            parentColumnLocation = GetNextParentColumnIndex(columnHeaders.GetLength(0) - (1 + (/*PivotCalculations.Count > 1*/ IsCalculationHeaderVisible ? 1 : 0)), col1);
                                            parentColTotal = valuesArea[row, parentColumnLocation[1]].GetResult();
                                        }
                                        this.pivotValues[row1, col1].Value = (Convert.ToDouble(v) / Convert.ToDouble(parentColTotal) * 100);
                                        if (!UsePercentageFormat)
                                            this.pivotValues[row1, col1].FormattedText = string.Format(CultureInfo.CurrentUICulture, format, (double)this.pivotValues[row1, col1].Value);
                                        else
                                            this.pivotValues[row1, col1].FormattedText = ((double)this.pivotValues[row1, col1].Value).ToString("0.00", CultureInfo.CurrentUICulture) + "%";
                                        break;
									case CalculationType.PercentageOfParentRowTotal:
                                        object parentRowTotal = 0;
                                        if (isSummaryRow)
                                        {
                                            int subtotalColumn = 0;
                                            for (int k1 = 0; k1 < this.PivotRows.Count; k1++)
                                            {
                                                if (rowHeaders[row1, k1] == null)
                                                {
                                                    subtotalColumn = k1 - 1;
                                                    break;
                                                }
                                            }
                                            if ((PivotRows.Count > 2) && subtotalColumn > 0)
                                            {
                                                parentRowLocation = GetNextParentRowIndex(row1, subtotalColumn);
                                                parentRowTotal = valuesArea[parentRowLocation[0], col].GetResult();
                                            }
                                            else
                                            {
                                                parentRowLocation = GetNextParentRowIndex(row1, rowHeaders.GetLength(1) - PivotRows.Count);
                                                parentRowTotal = valuesArea[parentRowLocation[0], col].GetResult();
                                            }
                                        }
                                        else
                                        {
                                            parentRowLocation = GetNextParentRowIndex(row1, rowHeaders.GetLength(1) - 1);
                                            parentRowTotal = valuesArea[parentRowLocation[0], col].GetResult();
                                        }
										this.pivotValues[row1, col1].Value = (Convert.ToDouble(v) / Convert.ToDouble(parentRowTotal) * 100);
                                        if (!UsePercentageFormat)
                                            this.pivotValues[row1, col1].FormattedText = string.Format(CultureInfo.CurrentUICulture, format, (double)this.pivotValues[row1, col1].Value);
                                        else
                                            this.pivotValues[row1, col1].FormattedText = ((double)this.pivotValues[row1, col1].Value).ToString("0.00", CultureInfo.CurrentUICulture) + "%";
										break;
									case CalculationType.PercentageOfParentTotal:
										if (compInfo.BaseField != null)
										{
#if !SILVERLIGHT
											int index = this.PivotRows.FindIndex(i => i.FieldMappingName == compInfo.BaseField);
#else
											int index = -1;
											for (int i = 0; i < this.PivotRows.Count; i++)
											{
												if (this.PivotRows[i].FieldMappingName == compInfo.BaseField)
												{
													index = i;
													break;
												}
											}
#endif
											object baseItemValue = null;
											if (index != -1) //Selected base field is in Pivot Rows. Then calculation should be based on Row values.
											{
												index = GetParentRowIndex(row, index, isSummaryRow);
												baseItemValue = index != -1 ? this.valuesArea[index, col].GetResult() : null;
											}
											else
											{
#if !SILVERLIGHT
												index = this.PivotColumns.FindIndex(i => i.FieldMappingName == compInfo.BaseField);  
#else
												for (int i = 0; i < this.PivotColumns.Count; i++)
												{
													if (this.PivotColumns[i].FieldMappingName == compInfo.BaseField)
													{
														index = i;
														break;
													}
												}
#endif
												if (index != -1)
												{
													index = GetParentColumnIndex(index, col, isSummaryColumn);
													baseItemValue = index != -1 ? this.valuesArea[row, index].GetResult() : null;
												}
											}
											if (baseItemValue != null)
											{
												this.pivotValues[row1, col1].Value = (Convert.ToDouble(v) / Convert.ToDouble(baseItemValue) * 100);
                                                if (!UsePercentageFormat)
                                                    this.pivotValues[row1, col1].FormattedText = string.Format(CultureInfo.CurrentUICulture, format, (double)this.pivotValues[row1, col1].Value);
                                                else
                                                    this.pivotValues[row1, col1].FormattedText = ((double)this.pivotValues[row1, col1].Value).ToString("0.00", CultureInfo.CurrentUICulture) + "%";
											}
											else
											{
												this.pivotValues[row1, col1].Value = null;
												this.pivotValues[row1, col1].FormattedText = string.Empty;
											}
										}
										else
										{
											throw new ArgumentNullException("Base Field value should be required.");
										}
										break;
									case CalculationType.Index:
										grandTotal = valuesArea[valuesArea.GetLength(0) - 1, (valuesArea.GetLength(1) + ((col % this.PivotCalculations.Count) - this.PivotCalculations.Count))].GetResult();
										colTotal = valuesArea[valuesArea.GetLength(0) - 1, col].GetResult();
										rowTotal = valuesArea[row, (valuesArea.GetLength(1) + ((col % this.PivotCalculations.Count) - this.PivotCalculations.Count))].GetResult();
										this.pivotValues[row1, col1].Value = (Convert.ToDouble(v) * Convert.ToDouble(grandTotal)) / (Convert.ToDouble(rowTotal) * Convert.ToDouble(colTotal));
                                        if (!UsePercentageFormat)
                                            this.pivotValues[row1, col1].FormattedText = string.Format(CultureInfo.CurrentUICulture, format, (double)this.pivotValues[row1, col1].Value, 9);
                                        else
                                            this.pivotValues[row1, col1].FormattedText = Math.Round(((double)this.pivotValues[row1, col1].Value), 9).ToString(CultureInfo.CurrentUICulture);
										break;
                                    case CalculationType.Formula:
                                        CalculateFormula(row1, col1, col, compInfo);
                                        break;
									default:
										break;
								}
							}
							this.pivotValues[row1, col1].Format = PivotCalculations[col % PivotCalculations.Count].Format;
						}
						else
						{
							if (!(valuesArea[row, col] == null && this.ShowEmptyCells))
							{
								format = "{0:" + PivotCalculations[col % PivotCalculations.Count].Format + "}";
								this.pivotValues[row1, col1].FormattedText = string.Format(CultureInfo.CurrentUICulture, format, defaultSummaryValue);
								this.pivotValues[row1, col1].Format = PivotCalculations[col % PivotCalculations.Count].Format;
							}
						}

						if (isSummaryRow)
						{
							this.pivotValues[row1, col1].CellType = PivotCellType.TotalCell | PivotCellType.ValueCell;
						}
						else if (IsSummaryColumn(col1, ref k))
						{
							for (k = 0; k < PivotCalculations.Count; ++k)
							{
								if (this.pivotValues[row1, col1 + k] == null)
									this.pivotValues[row1, col1 + k] = new PivotCellInfo();
								this.pivotValues[row1, col1 + k].CellType = PivotCellType.TotalCell | PivotCellType.ValueCell;
							}
						}

					}
				}
			}
		}

        private void CalculateFormula(int row1, int col1, int col, PivotComputationInfo compInfo)
        {
            Dictionary<string, double> component = new Dictionary<string, double>();
            int c = col1 - (col % this.PivotCalculations.Count);
            for (int i = 0; i < this.PivotCalculations.Count; ++i)
            {
                if (this.PivotCalculations[i].CalculationName == null)
                {
                    this.PivotCalculations[i].CalculationName = string.Format("Computation{0}", i);
                }
                if (this.PivotCalculations[i].CalculationType != CalculationType.Formula)
                {
                    component.Add(this.PivotCalculations[i].CalculationName, this.pivotValues[row1, c] == null ? 0d : this.pivotValues[row1, c].DoubleValue);
                }
                else
                    this.PivotCalculations[i].AllowRunTimeGroupByField = false;
                c++;
            }
            if (compInfo.Expression == null)
            {
                compInfo.Expression = new FilterExpression(compInfo.CalculationName, compInfo.Formula);
            }
            this.pivotValues[row1, col1].Value = compInfo.Expression.ComputedValue(component);
            if (compInfo.Format != null && compInfo.Format.Length > 0)
            {
                this.pivotValues[row1, col1].FormattedText = ((double)this.pivotValues[row1, col1].Value).ToString(compInfo.Format, CultureInfo.CurrentUICulture);
            }
            else
            {
                this.pivotValues[row1, col1].FormattedText = Math.Round(((double)this.pivotValues[row1, col1].Value), 9).ToString(CultureInfo.CurrentUICulture);
            }
        }
       
       int[] parentRowLocation = new int[2];
       int[] parentColLocation = new int[2];
       public string UpdateCalculatedValue(int row, int col, object v, string fieldName)
       {
           string formattedText = DoCalculatedValue(row, col, v, fieldName);
           return formattedText;
       }

       private string DoCalculatedValue(int row, int col, object v, string field)
       {
           var calculationType = this.PivotCalculations[(col - this.PivotRows.Count) % this.PivotCalculations.Count].CalculationType;
           bool isSummaryRow = IsRowSummary(row);
           bool isSummaryColumn = IsSummaryColumn(col);
           string format = "{0:" + PivotCalculations[(col - this.PivotRows.Count) % PivotCalculations.Count].Format + "}";
           int Calc = (col - this.PivotRows.Count) % this.PivotCalculations.Count;
           int row1 = row - (this.PivotColumns.Count + 1);
           int col1 = col - this.pivotRows.Count;
           PivotComputationInfo compInfo = this.PivotCalculations[col1 % PivotCalculations.Count];
           switch (calculationType)
           {
               case CalculationType.NoCalculation:
                   this.pivotValues[row, col].FormattedText = string.Format(CultureInfo.CurrentUICulture, format, v);
                   break;
               case CalculationType.PercentageOfGrandTotal:
                   object grandTotal = valuesArea[valuesArea.GetLength(0) - 1, valuesArea.GetLength(1) - (this.PivotRows.Count - 1)].GetResult();
                   this.pivotValues[row, col].Value = (Convert.ToDouble(v) / Convert.ToDouble(grandTotal) * 100);
                   this.pivotValues[row, col].FormattedText = ((double)this.pivotValues[row, col].Value).ToString("0.00", CultureInfo.CurrentUICulture) + "%";
                   break;
               case CalculationType.PercentageOfColumnTotal:
                   object columnTotal = valuesArea[valuesArea.GetLength(0) - 1, col1].GetResult();
                   this.pivotValues[row, col].Value = (Convert.ToDouble(v) / Convert.ToDouble(columnTotal) * 100);
                   this.pivotValues[row, col].FormattedText = ((double)this.pivotValues[row, col].Value).ToString("0.00", CultureInfo.CurrentUICulture) + "%";
                   break;
               case CalculationType.PercentageOfRowTotal:
                   object rowTotal = valuesArea[row1, ((valuesArea.GetLength(1) - 1) - (this.PivotCalculations.Count - (Calc + 1)))].GetResult();
                   this.pivotValues[row, col].Value = (Convert.ToDouble(v) / Convert.ToDouble(rowTotal) * 100);
                   this.pivotValues[row, col].FormattedText = ((double)this.pivotValues[row, col].Value).ToString("0.00", CultureInfo.CurrentUICulture) + "%";
                   break;
               case CalculationType.PercentageOfParentColumnTotal:
                   object parentColTotal;
                   if (isSummaryColumn)
                   {
                       parentColLocation = GetNextParentColumnIndex(parentColLocation[0], col);
                       parentColTotal = valuesArea[row1, parentColLocation[1]].GetResult();
                   }
                   else
                   {
                       parentColLocation = GetNextParentColumnIndex(columnHeaders.GetLength(0) - (1 + (/*PivotCalculations.Count > 1*/ IsCalculationHeaderVisible ? 1 : 0)), col);
                       parentColTotal = valuesArea[row1, parentColLocation[1]].GetResult();
                   }
                   this.pivotValues[row, col].Value = (Convert.ToDouble(v) / Convert.ToDouble(parentColTotal) * 100);
                   this.pivotValues[row, col].FormattedText = ((double)this.pivotValues[row, col].Value).ToString("0.00", CultureInfo.CurrentUICulture) + "%";
                   break;
               case CalculationType.PercentageOfParentRowTotal:
                   object parentRowTotal;
                   if (isSummaryRow)
                   {
                       int subtotalColumn = 0;
                       for (int k1 = 0; k1 < this.PivotRows.Count; k1++)
                       {
                           if (rowHeaders[row1, k1] == null)
                           {
                               subtotalColumn = k1 - 1;
                               break;
                           }
                       }
                       if ((PivotRows.Count > 2) && subtotalColumn > 0)
                       {
                           parentRowLocation = GetNextParentRowIndex(row, subtotalColumn);
                           parentRowTotal = valuesArea[parentRowLocation[0], col1].GetResult();
                       }
                       else
                       {
                           parentRowLocation = GetNextParentRowIndex(row, rowHeaders.GetLength(1) - PivotRows.Count);
                           parentRowTotal = valuesArea[parentRowLocation[0], col1].GetResult();
                       }
                   }
                   else
                   {
                       parentRowLocation = GetNextParentRowIndex(row, rowHeaders.GetLength(1) - 1);
                       parentRowTotal = valuesArea[parentRowLocation[0], col1].GetResult();
                   }
                   this.pivotValues[row, col].Value = (Convert.ToDouble(v) / Convert.ToDouble(parentRowTotal) * 100);
                   this.pivotValues[row, col].FormattedText = ((double)this.pivotValues[row, col].Value).ToString("0.00", CultureInfo.CurrentUICulture) + "%";
                   break;
               case CalculationType.PercentageOfParentTotal:
                   if (compInfo.BaseField != null)
                   {
#if !SILVERLIGHT
                       int index = this.PivotRows.FindIndex(i => i.FieldMappingName == compInfo.BaseField);
#else
											int index = -1;
											for (int i = 0; i < this.PivotRows.Count; i++)
											{
												if (this.PivotRows[i].FieldMappingName == compInfo.BaseField)
												{
													index = i;
													break;
												}
											}
#endif
                       object baseItemValue = null;
                       if (index != -1) //Selected base field is in Pivot Rows. Then calculation should be based on Row values.
                       {
                           index = GetParentRowIndex(row1, index, isSummaryRow);
                           baseItemValue = index != -1 ? this.valuesArea[index, col1].GetResult() : null;
                       }
                       else
                       {
#if !SILVERLIGHT
                           index = this.PivotColumns.FindIndex(i => i.FieldMappingName == compInfo.BaseField);
#else
												for (int i = 0; i < this.PivotColumns.Count; i++)
												{
													if (this.PivotColumns[i].FieldMappingName == compInfo.BaseField)
													{
														index = i;
														break;
													}
												}
#endif
                           if (index != -1)
                           {
                               index = GetParentColumnIndex(index, col1, isSummaryColumn);
                               baseItemValue = index != -1 ? this.valuesArea[row1, index].GetResult() : null;
                           }
                       }
                       if (baseItemValue != null)
                       {
                           this.pivotValues[row, col].Value = (Convert.ToDouble(v) / Convert.ToDouble(baseItemValue) * 100);
                           this.pivotValues[row, col].FormattedText = ((double)this.pivotValues[row, col].Value).ToString("0.00", CultureInfo.CurrentUICulture) + "%";
                       }
                       else
                       {
                           this.pivotValues[row, col].Value = null;
                           this.pivotValues[row, col].FormattedText = string.Empty;
                       }
                   }
                   else
                   {
                       throw new ArgumentNullException("Base Field value should be required.");
                   }
                   break;
               case CalculationType.Index:
                   grandTotal = valuesArea[valuesArea.GetLength(0) - 1, (valuesArea.GetLength(1) + ((col % this.PivotCalculations.Count) - this.PivotCalculations.Count))].GetResult();
                   columnTotal = valuesArea[valuesArea.GetLength(0) - 1, col1].GetResult();
                   rowTotal = valuesArea[row1, (valuesArea.GetLength(1) + ((col % this.PivotCalculations.Count) - this.PivotCalculations.Count))].GetResult();
                   this.pivotValues[row, col].Value = (Convert.ToDouble(v) * Convert.ToDouble(grandTotal)) / (Convert.ToDouble(rowTotal) * Convert.ToDouble(columnTotal));
                   this.pivotValues[row, col].FormattedText = Math.Round(((double)this.pivotValues[row,col].Value), 9).ToString(CultureInfo.CurrentUICulture);
                   break;
               case CalculationType.Formula:
                   CalculateFormula(row1, col1, col, compInfo);
                   break;
               default:
                   break;
           }
            
            return this.pivotValues[row, col].FormattedText;

        }
		private void MarkGrandTotalCellType()
		{
			if (!this.ShowGrandTotals)
				return;

			if (PivotColumns.Count == 0 && PivotRows.Count > 0)
			{
                if (/*PivotCalculations.Count > 1*/ IsCalculationHeaderVisible && ShowCalculationsAsColumns)
				{
					int count = this.pivotValues.GetLength(1);
					for (int i = count - 2; i >= count - PivotCalculations.Count - 1 && i > 0; i--)
					{
						if (this.pivotValues[0, i] != null)
						{
							this.pivotValues[0, i].CellType = PivotCellType.HeaderCell | PivotCellType.ColumnHeaderCell | PivotCellType.GrandTotalCell;
						}

					}
				}

				for (int i = 0; i < this.pivotValues.GetLength(0); ++i)
				{
					if (this.pivotValues[i, PivotRows.Count - 1] != null)
					{
						this.pivotValues[i, PivotRows.Count - 1].CellType = PivotCellType.HeaderCell | PivotCellType.RowHeaderCell;
						if (i == this.pivotValues.GetLength(0) - 1)
						{
							this.pivotValues[i, PivotRows.Count - 1].CellType |= PivotCellType.GrandTotalCell;
						}
					}
				}
			}

			if (RowCount < 2)
				return;

			for (int col = 0; col < ColumnCount; ++col)
			{
				if (pivotValues[RowCount - 2, col] != null)
				{
					pivotValues[RowCount - 2, col].CellType |= PivotCellType.GrandTotalCell;
					pivotValues[RowCount - 2, col].CellType &= ~PivotCellType.TotalCell;
				}
			}

			if (PivotColumns.Count > 0)
			{
				if (PivotCalculations.Count > 0)
				{
					for (int row = 0; row < RowCount; ++row)
					{
						for (int col = ColumnCount - PivotCalculations.Count - 1; col < ColumnCount - 1; ++col)
						{
							if (pivotValues[row, col] != null)
							{
								pivotValues[row, col].CellType |= PivotCellType.GrandTotalCell;
								if (!IsRowSummary(row))
									pivotValues[row, col].CellType &= ~PivotCellType.TotalCell;
							}
						}
					}
				}
				else
				{
					int calculationAdjustment = 1; //If there is no calculation then we should assume that there is a calculation field in order to cover grand total cell.
					for (int row = 0; row < RowCount; ++row)
					{
						for (int col = ColumnCount - calculationAdjustment - 1; col < ColumnCount - 1; ++col)
						{
							if (pivotValues[row, col] != null)
							{
								pivotValues[row, col].CellType |= PivotCellType.GrandTotalCell;
								pivotValues[row, col].CellType &= ~PivotCellType.TotalCell;
							}
						}
					}
				}
			}
		}
        /// <summary>
        /// Covers the column headers
        /// </summary>
		public void CoverColumnHeaders()
		{
			CoveredCellRange range = null;
			int lastRow = PivotColumns.Count + 1;
			int startRow = 1;
			if (PivotColumns.Count > 0)
			{
				while (startRow < lastRow)
				{
					int col = PivotRows.Count;
					//cover the columns
					int startCol = ++col;
					++col;

					while (col <= ColumnCount && startRow < lastRow)
					{
						int parentRow = startRow - 1;
						int parentCol = col - 2;
						string o = this.pivotValues[startRow - 1, col - 2].UniqueText;
						while (col < ColumnCount && (this.pivotValues[startRow - 1, col - 1].UniqueText == null || this.pivotValues[startRow - 1, col - 1].UniqueText.Equals(o)))
						{
							this.pivotValues[startRow - 1, col - 1].CellType = PivotCellType.ColumnHeaderCell | PivotCellType.HeaderCell;
							col++;
						}

						int row = startRow + 1;
						while (row < lastRow && this.pivotValues[row - 1, startCol - 1].UniqueText == null)
						{
							this.pivotValues[row - 1, startCol - 1].CellType = PivotCellType.ColumnHeaderCell | PivotCellType.HeaderCell;
							row++;
						}
						row--;
						if (col > startCol + 1 || row > startRow)
						{
							//mark all empty cells to easily spot successive covered cells...
							for (int r = startRow; r <= Math.Min(lastRow, row); ++r)
							{
								for (int c = startCol; c <= col - 1; ++c)
								{
									if (this.pivotValues[r - 1, c - 1].UniqueText == null)
									{
										this.pivotValues[r - 1, c - 1].Value = 'x';
										this.pivotValues[r - 1, c - 1].UniqueText = "x";
										this.pivotValues[r - 1, c - 1].CellType = PivotCellType.ColumnHeaderCell | PivotCellType.HeaderCell;
										this.pivotValues[r - 1, c - 1].ParentCell = this.pivotValues[parentRow, parentCol];
									}
								}
							}
                            if (this.pivotValues[startRow - 1, startCol - 1].Key != null)
                            {
                                range = new CoveredCellRange(startRow - 1, startCol - 1, Math.Min(lastRow, row) - 1, col - 2);
                                if (OkToAddRange(range))
                                    CoveredRanges.Add(range);
                                this.pivotValues[startRow - 1, startCol - 1].CellRange = range;

                                ////TODO
                                this.pivotValues[startRow - 1, startCol - 1].CellType = PivotCellType.ColumnHeaderCell | (((startRow == lastRow - 1 && /*PivotCalculations.Count > 1*/ IsCalculationHeaderVisible && this.pivotValues[row - 1, startCol - 1].UniqueText.ToString() != "x"))
                                                                                        ? PivotCellType.HeaderCell : ((range.Bottom - range.Top == 0)
                                                                                        ? PivotCellType.ExpanderCell : PivotCellType.TotalCell));
                            }
						}
                        else if ((startRow < lastRow - 1 || (startRow < lastRow - 2 && /*PivotCalculations.Count > 1*/ IsCalculationHeaderVisible))
						   && this.pivotValues[row - 1, startCol - 1] != null && this.pivotValues[row - 1, startCol - 1].UniqueText != null
						   && this.pivotValues[row - 1, startCol - 1].UniqueText.ToString().IndexOf(delimiter) == -1
						   && this.pivotValues[row - 1, startCol - 1].UniqueText.ToString() != "x")
						{
							range = new CoveredCellRange(startRow - 1, startCol - 1, startRow - 1, startCol - 1);
							if (OkToAddRange(range))
								CoveredRanges.Add(range);
							this.pivotValues[startRow - 1, startCol - 1].CellRange = range;
							////TODO
                            this.pivotValues[startRow - 1, startCol - 1].CellType = PivotCellType.ColumnHeaderCell | (((startRow == lastRow - 1 && /*PivotCalculations.Count > 1*/ IsCalculationHeaderVisible))
																					? PivotCellType.HeaderCell : ((range.Bottom - range.Top == 0)
																					? PivotCellType.ExpanderCell : PivotCellType.TotalCell));
						}
						else
						{
							this.pivotValues[startRow - 1, startCol - 1].CellType = PivotCellType.ColumnHeaderCell | PivotCellType.HeaderCell;
						}

						startCol = col;
						col++;
					}
					startRow++;
				}
			}

            if (/*PivotCalculations.Count > 1*/ IsCalculationHeaderVisible)
			{
				int k = 0;
				for (int c = PivotRows.Count + 1; c < ColumnCount; ++c)
				{
					////TODO
					if (this.pivotValues[lastRow - 1, c - 1].CellType == PivotCellType.ValueCell)
						this.pivotValues[lastRow - 1, c - 1].CellType = PivotCellType.ColumnHeaderCell | PivotCellType.CalculationHeaderCell;
					if (IsSummaryColumn(c - 1, ref k))
					{
						for (int j = 0; j < PivotCalculations.Count; ++j)
						{
                            this.pivotValues[lastRow - 1, c - 1 + j].CellType = (/*PivotCalculations.Count > 1*/ IsCalculationHeaderVisible) ? PivotCellType.TotalCell | PivotCellType.ColumnHeaderCell | PivotCellType.CalculationHeaderCell : PivotCellType.TotalCell | PivotCellType.ColumnHeaderCell;
;
						}
					}
				}
			}
		}
        /// <summary>
        /// Covers the GrandTotalRanges
        /// </summary>
		public void CoverGrandTotalRanges()
		{
			if (ShowGrandTotals && PivotCalculations.Count > 0)
			{
				int col0 = 0;
				int width = PivotRows.Count;
				int height = ShowCalculationsAsColumns ? 1 : Math.Max(1, PivotCalculations.Count);
				int row0 = RowCount - height - 1;
				CoveredCellRange range;

				if (RowCount > 0)
				{
					range = new CoveredCellRange(row0, col0, row0 + height - 1, col0 + width - 1);
					if (OkToAddRange(range))
						CoveredRanges.Add(range);
					this.pivotValues[row0, col0].CellRange = range;
					this.pivotValues[row0, col0].CellType = PivotCellType.GrandTotalCell | PivotCellType.RowHeaderCell;
                    if (!ShowCalculationsAsColumns && /*PivotCalculations.Count > 1*/ IsCalculationHeaderVisible && col0 + width < pivotValues.GetLength(1))
					{
						for (int i = 0; i < height; ++i)
						{
							this.pivotValues[row0 + i, col0 + width].CellType = PivotCellType.GrandTotalCell | PivotCellType.CalculationHeaderCell | PivotCellType.RowHeaderCell;
						}
					}
				}
				if (ColumnCount > 0)
				{
					width = ShowCalculationsAsColumns ? Math.Max(1, PivotCalculations.Count) : 1;
					col0 = ColumnCount - width - 1;
					height = PivotColumns.Count;
					row0 = 0;

					range = new CoveredCellRange(row0, col0, row0 + height - 1, col0 + width - 1);
					if (OkToAddRange(range))
						CoveredRanges.Add(range);
					this.pivotValues[row0, col0].CellRange = range;
					this.pivotValues[row0, col0].CellType = PivotCellType.GrandTotalCell | PivotCellType.ColumnHeaderCell;
                    if (ShowCalculationsAsColumns && /*PivotCalculations.Count > 1*/ IsCalculationHeaderVisible && row0 + height < pivotValues.GetLength(0))
					{
						for (int i = 0; i < width; ++i)
						{
							this.pivotValues[row0 + height, col0 + i].CellType = PivotCellType.GrandTotalCell | PivotCellType.CalculationHeaderCell | PivotCellType.ColumnHeaderCell;
						}
					}
				}
			}

		}

		private bool OkToAddRange(CoveredCellRange range)
		{
			return OkToAddRange(range, CoveredRanges);
		}
		private bool OkToAddRange(CoveredCellRange range, List<CoveredCellRange> ranges)
		{
			bool b = true;
			if (range.Left >= 0 && range.Right >= 0 && range.Top >= 0 && range.Bottom >= 0)
			{
				foreach (CoveredCellRange r in ranges)
				{
					if (range.Left > r.Right || range.Right < r.Left || range.Top > r.Bottom || range.Bottom < r.Top)
						continue;

					if (range.Left >= r.Left && range.Left <= r.Right && range.Top >= r.Top && range.Top <= r.Bottom)
					{
						b = false;
						break;
					}
					if (range.Right >= r.Left && range.Right <= r.Right && range.Bottom >= r.Top && range.Bottom <= r.Bottom)
					{
						b = false;
						break;
					}
				}
			}
			else
			{
				b = false;
			}

			return b;
		}

        /// <summary>
        /// CoverRowHeaders
        /// </summary>

		public void CoverRowHeaders()
		{
			if (PivotRows.Count == 0)
				return;

			//cover upper left corner
			int lastColumn = PivotRows.Count;
            int row = PivotColumns.Count + ((/*PivotCalculations.Count > 1*/ IsCalculationHeaderVisible) ? 1 : 0);

			CoveredCellRange range = new CoveredCellRange(0, 0, row - 1, lastColumn - 1);
			if (row >= 1)
			{
				this.pivotValues[0, 0].CellRange = range;
				this.pivotValues[0, 0].CellType = PivotCellType.TopLeftCell;
				this.CoveredRanges.Add(range);
			}

			int row0 = row;
			int lastCol = PivotRows.Count;
			int startCol = 1;

			while (startCol < lastCol)
			{
				row = row0;
				//cover the columns
				int startRow = ++row;
				++row;

				while (row <= RowCount && startCol < lastCol)
				{
					int parentRow = row - 2;
					int parentCol = startCol - 1;

					string o = this.pivotValues[row - 2, startCol - 1].UniqueText;
					while (row < RowCount && (this.pivotValues[row - 1, startCol - 1].UniqueText == null || this.pivotValues[row - 1, startCol - 1].UniqueText.Equals(o)))
					{
						row++;
					}

					int col = startCol + 1;
					while (col <= lastCol && this.pivotValues[startRow - 1, col - 1].UniqueText == null)
					{
						col++;
					}
					col--;

					if (row > startRow + 1 || col > startCol)
					{
						//mark all empty cells to easily spot successive covered cells...
						for (int r = startRow; r <= row - 1; ++r)
						{
							for (int c = startCol; c <= Math.Min(lastCol, col); ++c)
							{
								if (this.pivotValues[r - 1, c - 1].UniqueText == null)
								{
									this.pivotValues[r - 1, c - 1].Value = 'x';
									this.pivotValues[r - 1, c - 1].UniqueText = "x";
									this.pivotValues[r - 1, c - 1].ParentCell = this.pivotValues[parentRow, parentCol];
								}
							}
						}

						range = new CoveredCellRange(startRow - 1, startCol - 1, row - 2, Math.Min(col, lastCol) - 1);
						if (OkToAddRange(range))
							CoveredRanges.Add(range);
						this.pivotValues[startRow - 1, startCol - 1].CellRange = range;
						////TODO
						this.pivotValues[startRow - 1, startCol - 1].CellType = PivotCellType.RowHeaderCell | ((range.Bottom - range.Top != 0) ? PivotCellType.ExpanderCell : PivotCellType.TotalCell);
					}
					//////////////////////////////
					else if (PivotCalculations.Count > 0
					 && this.pivotValues[row - 1, startCol - 1] != null && this.pivotValues[row - 1, startCol - 1].UniqueText != null
					 && this.pivotValues[row - 1, startCol - 1].UniqueText.ToString().IndexOf(delimiter) > -1
					 && this.pivotValues[row - 1, startCol - 1].UniqueText.ToString() != "x")
					{
						range = new CoveredCellRange(startRow - 1, startCol - 1, startRow - 1, startCol - 1);
						if (OkToAddRange(range))
							CoveredRanges.Add(range);
						this.pivotValues[startRow - 1, startCol - 1].CellRange = range;
						////TODO
						this.pivotValues[startRow - 1, startCol - 1].CellType = PivotCellType.RowHeaderCell | PivotCellType.ExpanderCell;
					}
					else
					{
						this.pivotValues[startRow - 1, startCol - 1].CellType = PivotCellType.RowHeaderCell | PivotCellType.HeaderCell;
					}

					///////////////////////////////
					startRow = row;
					row++;
				}
				startCol++;
			}

			if (row0 > 0)
			{
				for (row = row0; row < RowCount; ++row)
				{
					if ((this.pivotValues[row - 1, lastCol - 1].CellType & PivotCellType.ValueCell) != 0)
					{
						this.pivotValues[row - 1, lastCol - 1].CellType = PivotCellType.RowHeaderCell | PivotCellType.HeaderCell;
					}
				}
			}
		}
        public void ReArrangePivotValuesForColumn()
        {
            int startIndex;
            List<PivotCellInfo> tempInfo = new List<PivotCellInfo>();
            List<int> subtotalColumnIndex = new List<int>();
            for (int pivotColumn = 0; pivotColumn < this.PivotColumns.Count - 1; pivotColumn++)
            {
                for (int j = 0; j < this.PivotValues[pivotColumn].Count; j++)
                {
                    if (this.PivotValues[pivotColumn, j] != null && this.PivotValues[pivotColumn, j].CellType == (PivotCellType.TotalCell | PivotCellType.ColumnHeaderCell))
                    {
                        int n = this.PivotCalculations.Count - 1;
                        subtotalColumnIndex.Add(j + n);
                    }
                }
                for (int i = pivotColumn; i < this.PivotValues.Count; i++)
                {
                    startIndex = (this.PivotRows.Count > 0) ? this.PivotRows.Count + (pivotColumn * this.PivotCalculations.Count) : (1 + (pivotColumn * this.PivotCalculations.Count));
                    for (int j = 0; j < this.PivotValues[i].Count; j++)
                    {
                        for (int k = 0; k < subtotalColumnIndex.Count; k++)
                        {
                            if (subtotalColumnIndex[k] == j)
                            {
                                int coveredIndex;
                                foreach (PivotComputationInfo pi in PivotCalculations)
                                {
                                    if (this.PivotValues[i, j] != null)
                                    {
                                        tempInfo.Add(this.PivotValues[i, j]);
                                        this.PivotValues[i].RemoveAt(j);
                                        this.PivotValues[i].Insert(startIndex, tempInfo[0]);
                                        for (int n = startIndex; n <= j; n++)
                                        {
                                            if (this.pivotValues[i, n].CellRange != null && this.pivotValues[i, n].CellRange == tempInfo[0].CellRange)
                                            {
                                                coveredIndex = CoveredRanges.IndexOf(tempInfo[0].CellRange);
                                                this.PivotValues[i, n].CellRange = new CoveredCellRange(tempInfo[0].CellRange.Top, startIndex, tempInfo[0].CellRange.Bottom, startIndex + PivotCalculations.Count - 1);
                                                if (this.PivotValues[i, n].CellType == (PivotCellType.ColumnHeaderCell | PivotCellType.TotalCell))
                                                    this.PivotValues[i, n].CellType = (PivotCellType.ExpanderCell | PivotCellType.ColumnHeaderCell);
                                                CoveredRanges.RemoveAt(coveredIndex);
                                                CoveredRanges.Insert(coveredIndex - 1, this.PivotValues[i, n].CellRange);
                                            }
                                            else if (this.pivotValues[i, n].CellRange != null)
                                            {
                                                coveredIndex = CoveredRanges.IndexOf(this.PivotValues[i, n].CellRange);
                                                this.PivotValues[i, n].CellRange = new CoveredCellRange(this.PivotValues[i, n].CellRange.Top, this.PivotValues[i, n].CellRange.Left + 1, this.PivotValues[i, n].CellRange.Bottom, this.PivotValues[i, n].CellRange.Right + 1);
                                                if (this.PivotValues[i, n].CellType == (PivotCellType.ColumnHeaderCell | PivotCellType.ExpanderCell))
                                                    this.PivotValues[i, n].CellType = (PivotCellType.HeaderCell | PivotCellType.ColumnHeaderCell);
                                                CoveredRanges.RemoveAt(coveredIndex);
                                                CoveredRanges.Insert(coveredIndex, this.PivotValues[i, n].CellRange);
                                            }
                                        }
                                        tempInfo.Clear();
                                    }
                                }
                                startIndex = j + ((pivotColumn > 0) ? ((this.PivotValues[pivotColumn, j + 1].CellRange == null) ? (pivotColumn * this.PivotCalculations.Count) + 1 : 1) : 1);
                            }
                        }
                    }
                }
                subtotalColumnIndex.Clear();
            }
        }

        public void ReArrangePivotValuesForRows()
        {
            int startIndex = (this.PivotCalculations.Count > 1) ? this.PivotColumns.Count : (PivotColumns.Count >= 1) ? this.PivotColumns.Count - 1 : this.PivotColumns.Count;
            PivotCellInfos tempInfo = new PivotCellInfos();
            if (this.PivotRows.Count >= 2)
            {
                for (int pivotRow = 0; pivotRow < this.PivotRows.Count; pivotRow++)
                {
                    startIndex++;
                    int n = startIndex;
                    for (int i = 0; i < this.PivotValues.Count; i++)
                    {
                        if (this.PivotValues[i, pivotRow] != null && this.PivotValues[i, pivotRow].CellType == (PivotCellType.TotalCell | PivotCellType.RowHeaderCell))
                        {
                            tempInfo.Add(this.PivotValues[i]);
                            for (int k = pivotRow; k >= 0; k--)
                            {
                                if (tempInfo[0, k].CellRange == null && tempInfo[0, k].FormattedText == null)
                                {
                                    tempInfo[0, k].CellType = (PivotCellType.HeaderCell | PivotCellType.RowHeaderCell);
                                    tempInfo[0, k].FormattedText = this.PivotValues[startIndex, pivotRow - 1].FormattedText;
                                }
                            }
                            int coveredIndex = CoveredRanges.IndexOf(tempInfo[0, pivotRow].CellRange);
                            tempInfo[0, pivotRow].CellRange = new CoveredCellRange(startIndex, tempInfo[0, pivotRow].CellRange.Left, startIndex, tempInfo[0, pivotRow].CellRange.Right);
                            CoveredRanges.RemoveAt(coveredIndex);
                            tempInfo[0, pivotRow].CellType = (PivotCellType.RowHeaderCell | PivotCellType.ExpanderCell);
                            this.PivotValues.RemoveAt(i);
                            this.PivotValues.Insert(startIndex, tempInfo[0]);
                            coveredIndex = CoveredRanges.IndexOf(this.PivotValues[startIndex + 1, pivotRow].CellRange);
                            this.PivotValues[startIndex + 1, pivotRow].CellRange = new CoveredCellRange(this.PivotValues[startIndex + 1, pivotRow].CellRange.Top + pivotRow + 1, this.PivotValues[startIndex + 1, pivotRow].CellRange.Left, this.PivotValues[startIndex + 1, pivotRow].CellRange.Bottom + pivotRow + 1, this.PivotValues[startIndex + 1, pivotRow].CellRange.Right);
                            CoveredRanges.RemoveAt(coveredIndex);
                            CoveredRanges.Insert(coveredIndex, this.PivotValues[startIndex + 1, pivotRow].CellRange);
                            CoveredRanges.Insert(coveredIndex, tempInfo[0, pivotRow].CellRange);
                            this.PivotValues[startIndex + 1, pivotRow].CellType = (PivotCellType.RowHeaderCell | PivotCellType.HeaderCell);
                            tempInfo.Clear();
                            int count = 1;
                            if (pivotRow > 0)
                            {
                                if (IsRowSummary(i - pivotRow) && !(IsGrandTotalCell(i - pivotRow, 0)))
                                {
                                    for (int rowsummary = i - pivotRow + 1; rowsummary <= (i - pivotRow) + this.pivotRows.Count; rowsummary++)
                                    {
                                        if (IsRowSummary(rowsummary) && !(IsGrandTotalCell(i - pivotRow, 0)))
                                        {
                                            count++;
                                        }
                                        else
                                            break;
                                    }
                                }
                                startIndex = i + count;
                            }

                            //if (pivotRow > 0 && (i + pivotRow) < this.PivotValues.Count && this.PivotValues[i + pivotRow, pivotRow - 1] != null && this.PivotValues[i + pivotRow, pivotRow - 1].CellRange != null)
                            //    startIndex = i + pivotRow + 1;
                            else
                                startIndex = i + 1;

                        }
                    }
                    startIndex = n;
                }
            }
        }

        private bool okToPopulate
		{
			get { return !lockComputations && DataSourceList != null; }
		}

		BinaryList columnKeysCalcValues;
		BinaryList rowKeysCalcValues;
		BinaryList tableKeysCalcValues;

        private void PopulatePivotTable()
        {
            emptyPivot = false;


            if (DataSourceList != null)
            {

                columnKeysCalcValues = new BinaryList();
                rowKeysCalcValues = new BinaryList();
                tableKeysCalcValues = new BinaryList();

#if !SILVERLIGHT
                PropertyDescriptor[] colPDs = ProcessList(PivotColumns);
                PropertyDescriptor[] rowPDs = ProcessList(PivotRows);
                PropertyDescriptor[] calcValuesPDs = GetCalcValuesPDs();

#else
				object[] colPDs = ProcessList(PivotColumns);
				object[] rowPDs = ProcessList(PivotRows);
				object[] calcValuesPDs = GetCalcValuesPDs();
#endif


                IComparer[] colComparers = GetComparers(PivotColumns);
                IComparer[] rowComparers = GetComparers(PivotRows);

                List<IComparable> cols;
                List<IComparable> rows;
                List<IComparable> table;
                List<object> values;
                SummaryBase sb;
                bool pass = true;

                KeysCalculationValues tempValue = null;
                int loc = -1;
                IEnumerable list = DataSourceList;
                bool doEval = Filters.Count > 0;
                bool listEmpty = Filters.Count > 0;
                string delimiter = " ";

#if !SILVERLIGHT
                string saveRowFilter = "";
                bool isDataView = false;
                if (DataSourceList is DataView)
                {
                    saveRowFilter = ((DataView)list).RowFilter;
                    list = new DataView(((DataView)list).Table);
                    isDataView = true;
                    if (doEval)
                    {
                        string s = "";
                        foreach (FilterExpression exp in Filters)
                        {
                            if (s.Length > 0)
                            {
                                s += " AND " + "(" + exp.Expression + ")";
                            }
                            else
                            {
                                s = "(" + exp.Expression + ")";
                            }
                        }
                        ((DataView)list).RowFilter = saveRowFilter.Length == 0 ? s
                                                       : string.Format("({0}) AND ({1})", saveRowFilter, s);
                        doEval = false;
                        listEmpty = ((DataView)list).Count == 0;
                    }
                }
#endif

#if SILVERLIGHT
                Dictionary<string, object> propCollection = this.ItemProperties as Dictionary<string, object>;
#endif
                PropertyInfo[] propInfo = null;
                int lastPopulate = 100;
                int listIndex = 0;
                int listCount = 1000;
                if (list is IList)
                    listCount = ((IList)list).Count;
                foreach (object o in list)
                {
                    bool refreshRowKeys = false;
                    bool refreshColKeys = false;
                    if (NotPopulated != false && listIndex > lastPopulate)
                    {
                        if (listIndex >= listCount)
                            listCount += 1000;
                        populationStatus = 0.95 * ((double)listIndex) / listCount;
                        lastPopulate = listIndex;
                    }
                    listIndex++;
#if SILVERLIGHT
                    if(!IsDataDynamic)
                        propInfo = o.GetType().GetProperties();
#else
                    propInfo = o.GetType().GetProperties();
#endif

                    //check for filter first thing
                    if (doEval)
                    {
                        pass = true;
                        foreach (FilterExpression exp in Filters)
                        {
#if SILVERLIGHT
                            if (isDataDynamic && propCollection != null && propCollection.Count() > 0)
                            {
                                foreach (PropertyInfo p in propCollection.Values)
                                {
                                    if (p.CanWrite && (p.Name.Equals(exp.Name)) && (p.GetValue(o, null) != null))
                                    {
                                        string _type = p.GetValue(o, null).GetType().Name;
                                        string _value = p.GetValue(o, null).ToString();
                                        if (_value.StartsWith(" ") || _value.EndsWith(" "))
                                        {
                                            var _val = p.GetValue(o, null).ToString().Trim();
                                            if (_type.Contains("Int32"))
                                                p.SetValue(o, Convert.ToInt32(_val), null);
                                            if (_type.Contains("Int64"))
                                                p.SetValue(o, Convert.ToInt64(_val), null);
                                            if (_type.Contains("Double"))
                                                p.SetValue(o, Convert.ToDouble(_val), null);
                                            if (_type.Contains("Single") || _type.Contains("float"))
                                                p.SetValue(o, Convert.ToSingle(_val), null);
                                            if (_type.Contains("Decimal"))
                                                p.SetValue(o, Convert.ToDecimal(_val), null);
                                        }
                                        break;
                                    }
                                }
                            }
                            else
                            {
#endif
                                if (propInfo != null && propInfo.Count() > 0)
                                {
                                    foreach (PropertyInfo p in propInfo)
                                    {
                                        if (p.CanWrite && (p.Name.Equals(exp.Name)) && (p.GetValue(o, null) != null))
                                        {
                                            string _type = p.GetValue(o, null).GetType().Name;
                                            string _value = p.GetValue(o, null).ToString();
                                            if (_value.StartsWith(" ") || _value.EndsWith(" "))
                                            {
                                                var _val = p.GetValue(o, null).ToString().Trim();
                                                if (_type.Contains("Int32"))
                                                    p.SetValue(o, Convert.ToInt32(_val), null);
                                                if (_type.Contains("Int64"))
                                                    p.SetValue(o, Convert.ToInt64(_val), null);
                                                if (_type.Contains("Double"))
                                                    p.SetValue(o, Convert.ToDouble(_val), null);
                                                if (_type.Contains("Single") || _type.Contains("float"))
                                                    p.SetValue(o, Convert.ToSingle(_val), null);
                                                if (_type.Contains("Decimal"))
                                                    p.SetValue(o, Convert.ToDecimal(_val), null);
                                            }
                                            break;
                                        }
                                    }
                                }
#if SILVERLIGHT
                            }
#endif
                            object computedResult = exp.ComputedValue(o);
                            if (computedResult is bool)
                            {
                                pass = (bool)computedResult;
                            }
                            if (!pass)
                                break;
                        }
                        if (!pass)
                            continue; //fails filter - skip this object
                        listEmpty = false;
                    }

                    cols = new List<IComparable>();
                    rows = new List<IComparable>();
                    table = new List<IComparable>();
                    values = new List<object>();
#if !SILVERLIGHT
                    foreach (PropertyDescriptor pd in calcValuesPDs)
#else
					foreach (object pd in calcValuesPDs)
#endif
                    {
                        if (pd == null)
                        {
                            values.Add(o);
                        }
#if !SILVERLIGHT
                        else
                        {

                            object val = null;
                            if (!(o is DataRowView))
                                val = GetReflectedValue(o, pd.Name);
                            if (val == null)
                                val = pd.GetValue(o);
                            values.Add(val);
                        }
#else
                        else
                        {
                            if (pd is DynamicPropertyInfo)
                            {
                                DynamicPropertyInfo info = pd as DynamicPropertyInfo;
                                values.Add(info.GetValue(o));
                            }
                            else if (pd is PropertyInfo)
                            {
                                PropertyInfo info = pd as PropertyInfo;
                                values.Add(info.GetValue(o));
                            }
                            else if (pd is ExpressionPropertyDescriptor)
                            {
                                ExpressionPropertyDescriptor expDesc = pd as ExpressionPropertyDescriptor;
                                values.Add(expDesc.GetValue(o));
                            }
                        }
#endif
                    }
                    for (int k = 0; k < PivotRows.Count; ++k)// (PropertyDescriptor pd in rowPDs)
                    {
#if !SILVERLIGHT
                        IComparable val = null;
                        if (rowPDs[k] != null)
                        {
                            if (!(o is DataRowView))
                                val = GetReflectedValue(o, rowPDs[k].Name) as IComparable;
                            if (val == null)
                                val = rowPDs[k] != null ? rowPDs[k].GetValue(o) as IComparable : null;
                        }
#else
                        IComparable val = null;
                        if (rowPDs[k] is DynamicPropertyInfo)
                        {
                            DynamicPropertyInfo info = rowPDs[k] as DynamicPropertyInfo;
                            val = info.GetValue(o) as IComparable;
                        }
                        else if (rowPDs[k] is PropertyInfo)
                        {
                            PropertyInfo info = rowPDs[k] as PropertyInfo;
                            val = info.GetValue(o) as IComparable;
                        }
                        else if (rowPDs[k] is ExpressionPropertyDescriptor)
                        {
                            ExpressionPropertyDescriptor expDesc = rowPDs[k] as ExpressionPropertyDescriptor;
                            val = expDesc.GetValue(o) as IComparable;
                        }
#endif
                        if (val != null)
                        {
                            if (PivotRows[k].Format != null && PivotRows[k].Format.Length > 0)
                            {
                                string format = "{0:" + PivotRows[k].Format + "}";
                                val = string.Format(CultureInfo.CurrentUICulture, format, val);
                            }
                            else
                                val = val.ToString();
                        }
                        else
                        {
                            val = delimiter;
                            refreshRowKeys = true;
                        }
                        rows.Add(val);
                        table.Add(val);//"(null)");
                    }
                    for (int k = 0; k < PivotColumns.Count; ++k)//foreach (PropertyDescriptor pd in colPDs)
                    {
#if !SILVERLIGHT
                        IComparable val = null;
                        if (!(o is DataRowView))
                            val = GetReflectedValue(o, colPDs[k].Name) as IComparable;
                        if (val == null)
                            val = colPDs[k] != null ? colPDs[k].GetValue(o) as IComparable : null;
#else
                        IComparable val = null;
                        if (colPDs[k] is DynamicPropertyInfo)
                        {
                            DynamicPropertyInfo info = colPDs[k] as DynamicPropertyInfo;
                            val = info.GetValue(o) as IComparable;
                        }
                        else if (colPDs[k] is PropertyInfo)
                        {
                            PropertyInfo info = colPDs[k] as PropertyInfo;
                            val = info.GetValue(o) as IComparable;
                        }
                        else if (colPDs[k] is ExpressionPropertyDescriptor)
                        {
                            ExpressionPropertyDescriptor expDesc = colPDs[k] as ExpressionPropertyDescriptor;
                            val = expDesc.GetValue(o) as IComparable;
                        }
#endif

                        if (val != null)
                        {
                            if (PivotColumns[k].Format != null && PivotColumns[k].Format.Length > 0)
                            {
                                string format = "{0:" + PivotColumns[k].Format + "}";
                                val = string.Format(CultureInfo.CurrentUICulture, format, val);
                            }
                            else
                                val = val.ToString();
                        }
                        else
                        {
                            val = delimiter;
                            refreshColKeys = true;
                        }
                        cols.Add(val);//"(null)");
                        table.Add(val);//"(null)");
                    }
                    ////foreach (PropertyDescriptor pd in rowPDs)
                    ////{
                    ////    IComparable val = pd.GetValue(o) as IComparable;
                    ////    rows.Add(val != null ? val.ToString() : val);//"(null)");
                    ////    table.Add(val != null ? val.ToString() : val);//"(null)");
                    ////}
                    ////foreach (PropertyDescriptor pd in colPDs)
                    ////{
                    ////    IComparable val = pd.GetValue(o) as IComparable;
                    ////    cols.Add(val != null ? val.ToString() : val);//"(null)");
                    ////    table.Add(val != null ? val.ToString() : val);//"(null)");
                    ////}

                    if ((loc = columnKeysCalcValues.AddIfUnique(tempValue = new KeysCalculationValues() { Keys = cols, Comparers = colComparers },refreshColKeys)) < 0)
                    {
                        int k = 0;
                        tempValue.Values = new List<SummaryBase>();
                        foreach (PivotComputationInfo info in PivotCalculations)
                        {
                            sb = info.Summary.GetInstance();
                            sb.ShowNullAsBlank = this.ShowNullAsBlank;
                            sb.Combine(values[k++]);
                            tempValue.Values.Add(sb);
                        }
                    }
                    else
                    {
                        tempValue = columnKeysCalcValues[loc] as KeysCalculationValues;
                        for (int k = 0; k < PivotCalculations.Count; ++k)
                        {
                            if (tempValue != null && tempValue.Values[k] != null)
                            {
                                tempValue.Values[k].ShowNullAsBlank = this.ShowNullAsBlank;
                                tempValue.Values[k].Combine(values[k]);
                            }
                        }
                    }
                    if ((loc = rowKeysCalcValues.AddIfUnique(tempValue = new KeysCalculationValues() { Keys = rows, Comparers = rowComparers },refreshRowKeys)) < 0)
                    {
                        int k = 0;
                        tempValue.Values = new List<SummaryBase>();
                        foreach (PivotComputationInfo info in PivotCalculations)
                        {
                            sb = info.Summary.GetInstance();
                            sb.ShowNullAsBlank = this.ShowNullAsBlank;
                            sb.Combine(values[k++]);
                            tempValue.Values.Add(sb);
                        }
                    }
                    else
                    {
                        tempValue = rowKeysCalcValues[loc] as KeysCalculationValues;
                        for (int k = 0; k < PivotCalculations.Count; ++k)
                        {
                            if (tempValue != null && tempValue.Values != null)
                            {
                                tempValue.Values[k].ShowNullAsBlank = this.ShowNullAsBlank;
                                tempValue.Values[k].Combine(values[k++]);
                            }
                        }
                    }
                    if ((loc = tableKeysCalcValues.AddIfUnique(tempValue = new KeysCalculationValues() { Keys = table })) < 0)
                    {
                        int k = 0;
                        tempValue.Values = new List<SummaryBase>();
                        foreach (PivotComputationInfo info in PivotCalculations)
                        {
                            sb = info.Summary.GetInstance();
                            sb.ShowNullAsBlank = this.ShowNullAsBlank;
                            sb.Combine(values[k++]);
                            tempValue.Values.Add(sb);
                        }
                    }
                    else
                    {
                        tempValue = tableKeysCalcValues[loc] as KeysCalculationValues;
                        for (int k = 0; k < PivotCalculations.Count; ++k)
                        {
                            if (tempValue != null && tempValue.Values != null)
                            {
                                tempValue.Values[k].ShowNullAsBlank = this.ShowNullAsBlank;
                                tempValue.Values[k].Combine(values[k]);
                            }
                        }
                    }
                    if (CacheRawValues)
                    {
                        tempValue.RawValues.Add(o);
                    }
                }

                if (listEmpty)
                {
                    rowCount = 1;
                    emptyPivot = true;
                    columnCount = 1;
                    //pivotValues = new PivotCellInfo[1, 1];
                    pivotValues = new PivotCellInfos(1, 1);
                    pivotValues[0, 0] = new PivotCellInfo();
                    pivotValues[0, 0].CellType = PivotCellType.TopLeftCell;
                    pivotValues[0, 0].Value = EmptyPivotString;
                    pivotValues[0, 0].FormattedText = EmptyPivotString;
                    return;
                }
#if !SILVERLIGHT
                if (isDataView)
                {
                    ((DataView)list).RowFilter = saveRowFilter;
                }
#endif

            }
            DoUniqueValuesCount(PivotColumns, columnKeysCalcValues, 1);
            DoUniqueValuesCount(PivotRows, rowKeysCalcValues, 1);
            if (NotPopulated != false)
            {
                populationStatus = 1;
            }

        }
#if !SILVERLIGHT
        Dictionary<string, PropertyInfo> lookUp = null;
        internal object GetReflectedValue(object component, string property)
        {
            object o = null;
            PropertyInfo pInfo = null;
            if (lookUp == null && component != null)
            {
                PropertyDescriptorCollection propdesc = itemProperties;
                PropertyInfo[] propInfos = component.GetType().GetProperties();
                lookUp = new Dictionary<string, PropertyInfo>();
                foreach (PropertyInfo pi in propInfos)
                {
                    lookUp.Add(pi.Name, pi);
                }
            }
            if (lookUp != null && lookUp.TryGetValue(property, out pInfo))
            {
                o = pInfo.GetValue(component, null);
            }
            return o;
        }
#endif
		int rowOffSet = 0;
		int colOffSet = 0;

		private void DoCalculationTable()
		{
			if (PivotCalculations.Count == 0)
				return;

            bool moreThanOneCalculation = /*PivotCalculations.Count > 1*/ IsCalculationHeaderVisible;
			int rowCount = valuesArea.GetLength(0);
			int colCount = valuesArea.GetLength(1);

			List<IComparable> keys = new List<IComparable>();
			int row1 = 0;
#if !SyncfusionFramework3_5 && !XLSIO
            Parallel.For(0, rowCount, i =>

#else
            for (int i = 0; i < rowCount; ++i)
#endif
            {
                for (int j = 0; j < colCount; j += PivotCalculations.Count)
                {
                    int loc = tableKeysCalcValues.BinarySearch(new KeysCalculationValues() { Keys =  GetKeyAt(i + rowOffSet, j + colOffSet) });
                    if (loc > -1)
                    {
                        for (int k = 0; k < PivotCalculations.Count; k++)
                        {
                            valuesArea[i, j + k] = ((KeysCalculationValues)tableKeysCalcValues[loc]).Values[k];
                            valuesArea[i, j + k].ShowNullAsBlank = this.ShowNullAsBlank;
                        }
                    }
                    else
                    {
                        /*Since below lines not yet used and commented.
                        //List<IComparable> newKeys = new List<IComparable>();
                        //foreach (IComparable c in keys)
                        //{
                        //    if (c != null)
                        //    {
                        //        if ((loc = c.ToString().IndexOf(delimiter)) > -1)
                        //        {
                        //            s = c.ToString().Substring(0, loc);
                        //            if (s != GrandString)
                        //            {
                        //                newKeys.Add(s);
                        //            }
                        //        }
                        //        else
                        //        {
                        //            newKeys.Add(c);
                        //        }
                        //    }
                        //}
						 
						//List<SummaryBase> values = new List<SummaryBase>();
						//foreach (PivotComputationInfo info in PivotCalculations)
						//   values.Add(info.Summary.GetInstance());*/

						SummaryBase tempSB;
						int tempCount = 0;
						foreach (PivotComputationInfo info in PivotCalculations)
						{
							tempSB = info.Summary.GetInstance();
							tempSB.ShowNullAsBlank = this.ShowNullAsBlank;
							if (!this.ShowNullAsBlank && info.DefaultValue == null)
							{
								tempSB.Combine(info.DefaultValue);
								valuesArea[i, j + tempCount] = tempSB;
							}
							else
							{
								if (info.DefaultValue != null) // Checking whether end-user provided default value for replacing the summary values if null.
								//if so then place the default value and automatically calculated combine summary in next method.
								{
									tempSB.Combine(info.DefaultValue);
									valuesArea[i, j + tempCount] = tempSB;
								}
							}

                            tempCount++;
                        }
                    }
                }
            }
#if !SyncfusionFramework3_5 && !XLSIO
            ); //end of Parallel.For
#endif

			if (!EnableOnDemandCalculations)
			{
				ProcessTotals3();
			}
		}

		private void ProcessTotals3()
		{
			int rowCount = valuesArea.GetLength(0);
			int colCount = valuesArea.GetLength(1);

			int valueColStart = rowHeaders.GetLength(1) + 1;
			int valueRowStart = columnHeaders.GetLength(0) + 1;

			List<SummaryBase> values = new List<SummaryBase>();
			int kRow = 0;
			int kCol = 0;

            SummaryBase tempSB;

            int count2 = PivotRows.Count < 2 ? PivotRows.Count + 1 : PivotRows.Count;
            if(rowCount>0)
            for (int columnPos = 0; columnPos < valuesArea.GetLength(1); ++columnPos)
            {
                if (IsSummaryColumn(valueColStart + columnPos - 1, ref kCol))
					continue;

				values.Clear();
				for (int i1 = 0; i1 < count2; ++i1)
				{
					values.Add(PivotCalculations[columnPos % PivotCalculations.Count].Summary.GetInstance());
				}

				for (int rowPos = 0; rowPos < valuesArea.GetLength(0); ++rowPos)
				{
					if (IsSummaryRow(valueRowStart + rowPos - 1, ref kRow))
					{
						if (this.ShowNullAsBlank && values[kRow].GetResult() == null)
						{
							tempSB = values[kRow].GetInstance();
							tempSB.ShowNullAsBlank = this.ShowNullAsBlank;
							valuesArea[rowPos, columnPos] = tempSB;
						}
						else
						{
							valuesArea[rowPos, columnPos] = values[kRow];
							values[kRow] = values[kRow].GetInstance();
						}
					}
					else if (valuesArea[rowPos, columnPos] is SummaryBase)
					{
						foreach (SummaryBase sum in values)
						{
							sum.ShowNullAsBlank = this.ShowNullAsBlank;
                            if (ApplyFormattedSummary)
                            {
                                double dummy;
                                int dummy1;
                                string format = "{0:" + PivotCalculations[columnPos % PivotCalculations.Count].Format + "}";
                                SummaryType type = PivotCalculations[columnPos % PivotCalculations.Count].SummaryType;
                                if (type == SummaryType.DoubleTotalSum && double.TryParse(string.Format(CultureInfo.CurrentUICulture, format, ((DoubleTotalSummary)valuesArea[rowPos, columnPos]).total), out dummy))
                                    ((DoubleTotalSummary)valuesArea[rowPos, columnPos]).total = double.Parse(string.Format(CultureInfo.CurrentUICulture, format, ((DoubleTotalSummary)valuesArea[rowPos, columnPos]).total));
                                else if (type == SummaryType.IntTotalSum && int.TryParse(string.Format(CultureInfo.CurrentUICulture, format, ((IntTotalSummary)valuesArea[rowPos, columnPos]).total), out dummy1))
                                    ((IntTotalSummary)valuesArea[rowPos, columnPos]).total = int.Parse(string.Format(CultureInfo.CurrentUICulture, format, ((IntTotalSummary)valuesArea[rowPos, columnPos]).total));
                            }
                            sum.CombineSummary(valuesArea[rowPos, columnPos]);
                      	}
					}
				}
				valuesArea[valuesArea.GetLength(0) - 1, columnPos] = values[values.Count - 1];
			}

			kRow = 0;
			int j2 = 0;
			count2 = PivotColumns.Count < 2 ? PivotColumns.Count + 1 : PivotColumns.Count;
			for (int rowPos = 0; rowPos < valuesArea.GetLength(0); ++rowPos)
			{
				values.Clear();
				for (int i1 = 0; i1 < count2; ++i1)
				{
					for (int i2 = 0; i2 < PivotCalculations.Count; ++i2)
						values.Add(PivotCalculations[i2].Summary.GetInstance());
					values[i1].ShowNullAsBlank = this.ShowNullAsBlank;
				}

				for (int columnPos = 0; columnPos < valuesArea.GetLength(1); ++columnPos)
				{
					if (IsSummaryColumn(valueColStart + columnPos - 1, ref kCol))
					{
						for (int i2 = 0; i2 < PivotCalculations.Count; ++i2)
						{
							if (this.ShowNullAsBlank && values[kCol * PivotCalculations.Count + i2].GetResult() == null)
							{
								tempSB = values[kCol * PivotCalculations.Count + i2].GetInstance();
								tempSB.ShowNullAsBlank = this.ShowNullAsBlank;
								valuesArea[rowPos, columnPos++] = tempSB;
							}
							else
							{
								valuesArea[rowPos, columnPos++] = values[kCol * PivotCalculations.Count + i2];
								values[kCol * PivotCalculations.Count + i2] = values[kCol * PivotCalculations.Count + i2].GetInstance();
							}
						}
						columnPos--;
					}
					else if (valuesArea[rowPos, columnPos] is SummaryBase)
					{
						j2 = columnPos % PivotCalculations.Count;
						while (j2 < values.Count)
						{
							values[j2].CombineSummary(valuesArea[rowPos, columnPos]);
							j2 += PivotCalculations.Count;
						}
					}
				}
				for (int i2 = 0; i2 < PivotCalculations.Count; ++i2)
				{
					if (valuesArea.GetLength(1) != 0)
					valuesArea[rowPos, valuesArea.GetLength(1) - PivotCalculations.Count + i2] = values[values.Count - PivotCalculations.Count + i2];
				}
			}

		}

		private int GetParentRowIndex(int currentRow, int column, bool isSummaryRow)
		{
            if (column == this.PivotRows.Count - 1)
            {
                return currentRow;
            }
            for (int i = currentRow + (this.PivotColumns.Count + (/*PivotCalculations.Count > 1*/ IsCalculationHeaderVisible ? 1 : 0)); i < rowHeaders.GetLength(0); i++)
			{
				if (rowHeaders[i, column] == null && isSummaryRow)
				{
					return -1;
				}
				if (rowHeaders[i, column] != null)
				{
					 if (rowHeaders[i, column].ToString().IndexOf(delimiter) > -1 || rowHeaders[i, column].ToString().IndexOf(this.GrandString + delimiter) > -1)
                         return i - (this.PivotColumns.Count + (/*PivotCalculations.Count > 1*/ IsCalculationHeaderVisible ? 1 : 0));
				}
			}
			return -1;
		}

        private int GetParentColumnIndex(int row, int currentColumn, bool isSummaryColumn)
        {
            if (row == this.PivotColumns.Count - 1)
            {
                return currentColumn;
            }
            for (int i = currentColumn + this.PivotRows.Count; i < columnHeaders.GetLength(1); i++)
            {
                if ((columnHeaders[row, i] == null && isSummaryColumn))
                {
                    return i - this.PivotRows.Count;
                }
                if (columnHeaders[row, i] != null)
                {
                    if (columnHeaders[row, i].ToString().IndexOf(delimiter) > -1 || columnHeaders[row, i].ToString().IndexOf(this.GrandString + delimiter) > -1)
                        return i - this.PivotRows.Count + (currentColumn % PivotCalculations.Count);
                }
            }
            return -1;
        }



		private int GetImmediateNextParentRowIndex(int currentRow, int column)
		{
			for (int i = currentRow + 1; i < rowHeaders.GetLength(0); i++)
			{
				if (rowHeaders[i, column] != null && rowHeaders[i, column].ToString().IndexOf(delimiter) > -1)
				{
					return i;
				}
			}
			return -1;
		}

		private int GetImmediateNextParentColumnIndex(int row, int currentColumn)
		{
			for (int i = currentColumn + 1; i < columnHeaders.GetLength(1); i++)
			{
				if (rowHeaders[row, i] != null && rowHeaders[row, i].ToString().IndexOf(delimiter) > -1)
				{
					return i;
				}
			}
			return -1;
		}

		private int[] GetNextParentRowIndex(int currentRowIndex, int parentColumnIndex)
		{
			int[] parentLocation = new int[2];

			for (int i = currentRowIndex + 1; i < rowHeaders.GetLength(0); i++)
			{
				if (parentColumnIndex > 0)
				{
					for (int j = parentColumnIndex - 1; j >= 0; j--)
					{
						if (rowHeaders[i, j] != null && rowHeaders[i, j].ToString().IndexOf(delimiter) > -1)
						{
                            parentLocation[0] = i - (this.PivotColumns.Count + (/*PivotCalculations.Count > 1*/ IsCalculationHeaderVisible ? 1 : 0)); parentLocation[1] = j;
							return parentLocation;
						}
					}
				}
				else
				{
					if (rowHeaders[i, parentColumnIndex] != null && rowHeaders[i, parentColumnIndex].ToString().IndexOf(this.GrandString + delimiter) > -1)
					{
                        parentLocation[0] = i - (this.PivotColumns.Count + (/*PivotCalculations.Count > 1*/ IsCalculationHeaderVisible ? 1 : 0)); parentLocation[1] = parentColumnIndex;
						return parentLocation;
					}
				}

            }
            parentLocation[0] = currentRowIndex - (this.PivotColumns.Count + (/*PivotCalculations.Count > 1*/ IsCalculationHeaderVisible ? 1 : 0));
			return parentLocation;

		}

		private int[] GetNextParentColumnIndex(int parentRowIndex, int currentColumnIndex)
		{
			int[] parentLocation = new int[2];
			for (int i = currentColumnIndex + 1; i < columnHeaders.GetLength(1); i++)
			{
				if (parentRowIndex > 0)
				{
					for (int j = parentRowIndex - 1; j >= 0; j--)
					{
						if (columnHeaders[j, i] != null && columnHeaders[j, i].ToString().IndexOf(delimiter) > -1)
						{
							parentLocation[1] = i - (this.PivotRows.Count - ((currentColumnIndex-this.PivotRows.Count) % this.PivotCalculations.Count)); parentLocation[0] = j;
							return parentLocation;
						}
					}
				}
				else
				{
					if (columnHeaders[parentRowIndex, i] != null && columnHeaders[parentRowIndex, i].ToString().IndexOf(this.GrandString + delimiter) > -1)
					{
                        parentLocation[1] = i - (this.PivotRows.Count - ((currentColumnIndex - this.PivotRows.Count) % this.PivotCalculations.Count)); parentLocation[0] = parentRowIndex;
						return parentLocation;
					}
				}
			}
            parentLocation[1] = currentColumnIndex - this.PivotRows.Count;
			return parentLocation;

		}
        /// <summary>
        ///  Checks whether a column at the index is Summary column or not.
        /// </summary>
        /// <param name="colIndex">int</param>
        /// <returns>bool</returns>
        public bool IsSummaryColumn(int colIndex)
        {
            bool b = false;
            if (colIndex < columnHeaders.GetLength(1) && colIndex >= 0)
            {
                for (int row = 0; row < columnHeaders.GetLength(0); ++row)
                {
                    if (columnHeaderUniqueValues[row, colIndex] != null && (columnHeaderUniqueValues[row, colIndex].ToString().IndexOf(delimiter) > -1 || (columnHeaderUniqueValues[row, colIndex].ToString().IndexOf(grandString + delimiter) > -1)))
                    {
                        b = true;
                        break;
                    }
                }
            }
            return b;
        }

        private bool IsSummaryColumnWhileOnDemand(int colIndex)
        {
            bool b = false;
            for (int row = 0; row < columnHeaders.GetLength(0); ++row)
            {
                if (row < rowCount && colIndex < ColumnCount && this.PivotValues != null && this.PivotValues[row, colIndex] != null && this.PivotValues[row, colIndex].Key != null 
                    && (this.PivotValues[row, colIndex].Key.IndexOf(delimiter) > -1 || this.pivotValues[row,colIndex].Key.IndexOf(grandString + delimiter) > -1 ))
                {
                    b = true;
                    break;
                }
            }
            return b;
        }

        private bool IsRowSummaryWhileOnDemand(int rowIndex)
        {
            bool b = false;
            for (int c = 0; c < rowHeaders.GetLength(1); ++c)
            {
                if (rowIndex < rowCount && c < ColumnCount && this.PivotValues != null && this.PivotValues[rowIndex, c] != null && this.PivotValues[rowIndex, c].Key != null
                    && (this.PivotValues[rowIndex, c].Key.IndexOf(delimiter) > -1 || this.pivotValues[rowIndex, c].Key.IndexOf(grandString + delimiter) > -1))
                {
                    b = true;
                    break;
                }
            }
            return b;
        }
        private bool IsGrandTotalCell(int rowIndex, int colIndex)
        {

            bool b = false;
            if (colIndex < columnHeaders.GetLength(1))
            {
                for (int row = 0; row < columnHeaders.GetLength(0); ++row)
                {
                    if (columnHeaderUniqueValues[row, colIndex] != null && columnHeaderUniqueValues[row, colIndex].ToString().IndexOf(grandString + delimiter) > -1)
                    {
                        b = true;
                        break;
                    }
                }
            }

            if (b == false)
            {

                if (rowIndex < rowHeaders.GetLength(0))
                {
                    for (int c = 0; c < rowHeaders.GetLength(1); ++c)
                    {
                        if (rowHeaders[rowIndex, c] != null && rowHeaders[rowIndex, c].ToString().IndexOf(grandString + delimiter) > -1)
                        {
                            b = true;
                            break;
                        }
                    }
                }

            }

            return b;
        }
        /// <summary>
        ///  Checks whether a column at the index is Summary column or not.
        /// </summary>
        /// <param name="colIndex">int</param>
        /// <param name="k">ref int</param>
        /// <returns>bool</returns>
        public bool IsSummaryColumn(int colIndex, ref int k)
        {
            bool b = false;
            if (colIndex < columnHeaders.GetLength(1))
            {
                for (int row = 0; row < columnHeaders.GetLength(0); ++row)
                {
                    if(columnHeaders[row, colIndex] != null && (columnHeaders[row, colIndex].ToString().IndexOf(delimiter) > -1 || (columnHeaders[row, colIndex].ToString().IndexOf(grandString + delimiter) > -1)))
                    {
                        k = row;
                        b = true;
                        break;
                    }
                }
            }
            return b;
        }
        /// <summary>
        /// Checks whether a column at the index is Summary column or not.
        /// </summary>
        /// <param name="colIndex">int</param>
        /// <param name="k">ref</param>
        /// <returns>bool</returns>
        public bool IsSummaryColumnWhileOnDemand(int colIndex, ref int k)
        {
            bool b = false;
            if (colIndex < columnHeaders.GetLength(1))
            {
                for (int row = 0; row < columnHeaders.GetLength(0); ++row)
                {
                    if (row < rowCount && colIndex < ColumnCount && this.PivotValues != null && this.PivotValues[row, colIndex] != null && this.PivotValues[row, colIndex].Key != null
                        && (this.PivotValues[row, colIndex].Key.IndexOf(delimiter) > -1 || this.PivotValues[row, colIndex].Key.IndexOf(grandString + delimiter) > -1))
                    {
                        k = row;
                        b = true;
                        break;
                    }
                }
            }
            return b;
        }

        /// <summary>
        /// Checks whether a column at the index is Summary column or not when GridLayout is TopSummary.
        /// </summary>
        /// <param name="colIndex"></param>
        /// <returns>bool</returns>
        public bool IsSummaryColumnWhileTopSummary(int colIndex)
        {
            bool b = false;
            if (colIndex < this.RowCount && colIndex < this.ColumnCount)
            {
                for (int row = 0; row < this.RowCount; row++)
                {
                    if (this.PivotValues[row, colIndex].CellType == (PivotCellType.ColumnHeaderCell | PivotCellType.ExpanderCell))
                        b = true;
                }
            }
            return b;
        }

        /// <summary>
        /// Checks whether a row at the index is Summary row or not when GridLayout is TopSummary.
        /// </summary>
        /// <param name="rowIndex"></param>
        /// <returns>bool</returns>
        public bool IsSummaryRowWhileTopSummary(int rowIndex)
        {
            bool b = false;
            int k = 0;
            if (rowIndex < this.RowCount)
            {
                for (int col = 0; col < this.ColumnCount; col++)
                {
                    if (this.PivotValues[rowIndex, col] != null && this.PivotValues[rowIndex, col].CellType == (PivotCellType.RowHeaderCell | PivotCellType.ExpanderCell))
                        b = true;
                    else if (this.PivotValues[rowIndex, col] != null && this.PivotValues[rowIndex, col].FormattedText != null && this.PivotValues[rowIndex, col].FormattedText.ToString().Contains(grandString + delimiter))
                        b = true;
                }
            }
            return b;
        }
        /// <summary>
        /// Checks whether a row at the index is Summary row or not.
        /// </summary>
        /// <param name="rowIndex">int</param>
        /// <param name="k">ref int</param>
        /// <returns>bool</returns>
        public bool IsSummaryRow(int rowIndex, ref int k)
        {
            bool b = false;
            if (rowIndex < rowHeaders.GetLength(0))
            {
                for (int col = 0; col < rowHeaders.GetLength(1); ++col)
                {
                    if (rowHeaders[rowIndex, col] != null && (rowHeaders[rowIndex, col].ToString().IndexOf(delimiter) > -1 || rowHeaders[rowIndex, col].ToString().IndexOf(grandString + delimiter) > -1))
                    {
                        b = true;
                        k = col;
                        break;
                    }
                }
            }
            return b;
        }

        public bool IsSummaryRowWhileOnDemand(int rowIndex, ref int k)
        {
            bool b = false;
            if (rowIndex < rowHeaders.GetLength(0))
            {
                for (int col = 0; col < rowHeaders.GetLength(1); ++col)
                {
                    if (rowIndex < rowCount && col < ColumnCount && this.PivotValues != null && this.PivotValues[rowIndex,col] != null && this.PivotValues[rowIndex,col].Key != null && this.PivotValues[rowIndex,col].Key.IndexOf(delimiter) > -1)
                    {
                        b = true;
                        k = col;
                        break;
                    }
                }
            }
            return b;
        }
        /// <summary>
        ///  Checks whether a column at the index is Summary row or not.
        /// </summary>
        /// <param name="colIndex">int</param>
        /// <returns>bool</returns>
        public bool IsRowSummary(int rowIndex)
        {
            bool b = false;
            if (rowIndex < rowHeaders.GetLength(0) && rowIndex >= 0)
            {
                for (int c = 0; c < rowHeaders.GetLength(1); ++c)
                {
                    if (rowHeaders[rowIndex, c] != null && (rowHeaders[rowIndex, c].ToString().IndexOf(delimiter) > -1 || rowHeaders[rowIndex, c].ToString().IndexOf(grandString + delimiter) > -1))
                    {
                        b = true;
                        break;
                    }
                }
            }
            return b;
        }

        private List<IComparable> GetKeyAt(int i, int j)
        {
            return GetKeyAt(i, j, false);
        }

        private List<IComparable> GetKeyAt(int i, int j, bool isForGetRawItem)
        {
            List<IComparable> keys = new List<IComparable>();
            if (!isForGetRawItem)
            {
                if (i < rowHeaders.GetLength(0) && i > -1)
                {
                    for (int k = 0; k < PivotRows.Count; ++k)
                    {
                        if (rowHeaders[i, k] != null)
                        {
                            keys.Add(rowHeaders[i, k].ToString());
                        }
                        else
                        {
                            String str = rowHeaderUniqueValues[i, k];
                            if (str != null)
                            {
                                String[] tokens = str.Split('.');
                                if (tokens[tokens.Length - 1] != string.Empty)
                                    keys.Add(tokens[tokens.Length - 1]);
                                else
                                    keys.Add(null);
                            }
                            else
                                keys.Add(null);
                        }
                    }
                }

                if (j < columnHeaders.GetLength(1) && j > -1)
                {

                    for (int k = 0; k < PivotColumns.Count; ++k)
                    {
                        if (columnHeaders[k, j] != null)
                        {
                            keys.Add(columnHeaders[k, j].ToString());
                        }
                        else
                        {
                            if (j < this.columnHeaders.GetLength(1))
                            {
                                String str = this.columnHeaderUniqueValues[k, j];
                                if (str != null)
                                {
                                    String[] tokens = str.Split('.');
                                    if (tokens[tokens.Length - 1] != string.Empty)
                                        keys.Add(tokens[tokens.Length - 1]);
                                    else
                                        keys.Add(null);
                                }
                                else
                                    keys.Add(null);
                                //"(null)");//columnHeaders[k, j]);
                            }
                        }
                    }
                }
                else
                {
                    if (this.PivotRows.Count == 0 || this.PivotCalculations.Count == 1)
                    {
                        j--;

                    }
                    for (int k = 0; k < PivotColumns.Count; ++k)
                    {
                        if (j < columnHeaders.GetLength(1) && columnHeaders[k, j] != null)
                        {
                            keys.Add(columnHeaders[k, j]);//"(null)");//columnHeaders[k, j]);
                        }
                        j--;
                    }
                }
            }
            else
            {
                if (!ShowCalculationsAsColumns)
                {
                    if (j < columnHeaders.GetLength(1) && j > -1)
                    {

                        for (int k = 0; k < PivotColumns.Count; ++k)
                        {
                            if (this[k, j] != null && this[k, j].Key != null)
                            {
                                keys.Add(this[k, j].Key.ToString());
                            }
                            else
                            {
                                keys.Add(null);
                            }
                        }
                    }
                    else
                    {
                        if (this.PivotRows.Count == 0 || this.PivotCalculations.Count == 1)
                        {
                            j--;

                        }
                        for (int k = 0; k < PivotColumns.Count; ++k)
                        {
                            if (this[k, j] != null)
                            {
                                keys.Add(this[k, j].Key);//"(null)");//columnHeaders[k, j]);
                            }
                        }
                    }

                    for (int k = 0; k < PivotRows.Count; ++k)
                    {
                        if (this[i, k] != null && this[i, k].Key != null)
                        {
                            keys.Add(this[i, k].Key.ToString());
                        }
                        else
                        {
                            keys.Add(null);
                        }
                    }

                }
                else
                {
                    if (i < rowHeaders.GetLength(0) && i > -1)
                    {
                        for (int k = 0; k < PivotRows.Count; ++k)
                        {
                            if (this[i, k] != null && this[i, k].Key != null)
                            {
                                keys.Add(this[i, k].Key.ToString());
                            }
                            else
                            {
                                keys.Add(null);
                            }
                        }
                    }

                    if (j < columnHeaders.GetLength(1) && j > -1)
                    {

                        for (int k = 0; k < PivotColumns.Count; ++k)
                        {
                            if (this[k, j] != null && this[k, j].Key != null)
                            {
                                keys.Add(this[k, j].Key.ToString());
                            }
                            else
                            {
                                keys.Add(null);
                            }
                        }
                    }
                    else
                    {
                        for (int k = 0; k < PivotColumns.Count; ++k)
                        {
                            if (this[k, j] != null)
                            {
                                keys.Add(this[k, j].Key);//"(null)");//columnHeaders[k, j]);
                            }
                        }
                    }
                }
            }

            return keys;
        }



		private void DoUniqueValuesCount(List<PivotItem> list, BinaryList bList, int insertCount)
		{
			if (list.Count <= 1)
				return;

			int level = -1;
			for (int k = list.Count - 2; k >= 0; k--)
			{
				level++;
				int colPivotCount = bList.Count;
				int count = colPivotCount > 0 ? 1 : 0;
				if (colPivotCount > 0)
				{
					IComparable prevValue = ((KeysCalculationValues)bList[0]).Keys[level];
					int prevCount = 0;
					IComparable newValue = null;
					for (int i = 1; i < colPivotCount; ++i)
					{
						if (((KeysCalculationValues)bList[i]).Keys != null)
						{
							newValue = ((KeysCalculationValues)bList[i]).Keys[level];
							if (newValue != null)
							{
								if ((newValue.CompareTo(prevValue) != 0) || (level > 0 && AnyPreviousLevelNotEqual(bList, i, prevCount, level)))
								{
									for (int j = 0; j < insertCount; ++j)
										bList.Insert(i, new KeysCalculationValues());
									i += insertCount;
									colPivotCount += insertCount;
									prevValue = newValue;
									prevCount = i;
								}
							}
							else if (prevValue != null)
							{
								for (int j = 0; j < insertCount; ++j)
									bList.Insert(i, new KeysCalculationValues());
								i += insertCount;
								colPivotCount += insertCount;
								prevValue = newValue;
								prevCount = i;
							}
						}
					}
				}
			}

			for (int j = 0; j < level + 1; ++j)
				bList.Add(new KeysCalculationValues());
		}
		private bool AnyPreviousLevelNotEqual(BinaryList bList, int i, int prevCount, int level)
		{
			IComparable prevLevelKeyValue = null, newLevelKeyValue = null;
			for (int k = level - 1; k >= 0; k--)
			{
				prevLevelKeyValue = ((KeysCalculationValues)bList[i]).Keys[k];
				newLevelKeyValue = ((KeysCalculationValues)bList[prevCount]).Keys[k];
				if (prevLevelKeyValue != null && prevLevelKeyValue.CompareTo(newLevelKeyValue) != 0)
				{
					return true;
				}
			}

			return false;
		}

		private IComparer[] GetComparers(List<PivotItem> pivotItems)
		{
			int count = pivotItems.Count;

			IComparer[] pds = new IComparer[count];
			for (int i = 0; i < count; i++)
			{
				pds[i] = pivotItems[i].Comparer;
#if !SILVERLIGHT
                if (pds[i] == null && ItemProperties[pivotItems[i].FieldMappingName] != null)
                {
                    pds[i] = (IComparer)AddComparers(ItemProperties[pivotItems[i].FieldMappingName].PropertyType);
                    pivotItems[i].Comparer = pds[i];
                }
#endif
            }

			return pds;
		}

        /// <summary>
        /// Adds a <see cref="FilterExpression"/> comparer for the specific pivot item.
        /// </summary>
        /// <param name="item">property type of the pivot item</param>
        public IComparer AddComparers(Type type)
        {
            if (type.Name.ToString() == "Int32")
                return new IntComparer();
            else if (type.Name.ToString() == "Double")
                return new DoubleComparer();
            else if (type.Name.ToString() == "Decimal")
                return new DecimalComparer();
            else if (type.Name.ToString() == "DateTime")
                    return new DateComparer();
            else
                return null;
        }

#if !SILVERLIGHT
		private PropertyDescriptor[] ProcessList(List<PivotItem> pivotItems)
		{
			int count = pivotItems.Count;
			PropertyDescriptor[] pds = new PropertyDescriptor[count];
#else
		private object[] ProcessList(List<PivotItem> pivotItems)
		{
			int count = pivotItems.Count;
			object[] pds = new object[count];
#endif

			for (int i = 0; i < count; i++)
			{
#if !SILVERLIGHT
				pds[i] = ItemProperties[pivotItems[i].FieldMappingName];
#else
                if (ItemProperties != null && ItemProperties[pivotItems[i].FieldMappingName] is DynamicPropertyInfo)
                    pds[i] = ItemProperties[pivotItems[i].FieldMappingName] as DynamicPropertyInfo;
                else if(ItemProperties != null && ItemProperties[pivotItems[i].FieldMappingName] is PropertyInfo)
                    pds[i] = ItemProperties[pivotItems[i].FieldMappingName] as PropertyInfo;
                else if (itemProperties != null && ItemProperties[pivotItems[i].FieldMappingName] is ExpressionPropertyDescriptor)
                    pds[i] = ItemProperties[pivotItems[i].FieldMappingName] as ExpressionPropertyDescriptor;
#endif
			}

			return pds;
		}

#if !SILVERLIGHT
		private PropertyDescriptor[] GetCalcValuesPDs()
		{
			PropertyDescriptor[] pds = new PropertyDescriptor[PivotCalculations.Count];
#else
		private object[] GetCalcValuesPDs()
		{
			object[] pds = new object[PivotCalculations.Count];
#endif
			for (int i = 0; i < PivotCalculations.Count; i++)
			{
#if !SILVERLIGHT
				pds[i] = ItemProperties[PivotCalculations[i].FieldName];
#else
                if (ItemProperties != null && ItemProperties[PivotCalculations[i].FieldName] is DynamicPropertyInfo)
                    pds[i] = ItemProperties[PivotCalculations[i].FieldName] as DynamicPropertyInfo;
                else if(ItemProperties != null && ItemProperties[PivotCalculations[i].FieldName] is PropertyInfo)
                    pds[i] = ItemProperties[PivotCalculations[i].FieldName] as PropertyInfo;
                else if(itemProperties != null && ItemProperties[PivotCalculations[i].FieldName] is ExpressionPropertyDescriptor)
                    pds[i] = ItemProperties[PivotCalculations[i].FieldName] as ExpressionPropertyDescriptor;
#endif
			}

			return pds;
		}

		#endregion

		#endregion

		#region PivotSchemaChanged event

		/// <summary>
		/// Event raised whenever the pivot schema for the engine changed.
		/// </summary>
		public event PivotSchemaChangedEventHandler PivotSchemaChanged;

		/// <summary>
		/// Raises the <see cref="PivotSchemaChanged"/> event.
		/// </summary>
		/// <param name="e">The event argument.</param>
		protected virtual void OnPivotSchemaChanged(PivotSchemaChangedArgs e)
		{
			if (PivotSchemaChanged != null)
			{
				PivotSchemaChanged(this, e);
			}
		}

		/// <summary>
		/// Raises the <see cref="PivotSchemaChanged"/> event.
		/// </summary>
		/// <param name="e">The event argument.</param>
		public void RaisePivotSchemaChangedEvent(PivotSchemaChangedArgs e)
		{
            if (!this.ShowCalculationsAsColumns && e.ChangeHints == SchemaChangeHints.CalculationChanged)
            {
                this.TransposePivotTable();// Transpose the PivotTable to get the ShowCalculationAsColumn as True form for recalculate the PivotValues
            }
			OnPivotSchemaChanged(e);
            if (!this.ShowCalculationsAsColumns && e.ChangeHints == SchemaChangeHints.CalculationChanged)
            {
                this.TransposePivotTable();// Transpose it again to get the original form (ShowCalculationAsColumn as False)
            }
		}

		#endregion

		#region Updating summaries when values are filtered using PivotRowsOnly

		/// <summary>
		/// This method will recompute all summaries ignoring any summary row whose index is
		/// in HiddenRowIndexes.
		/// </summary>
		public void UpdateAllSummariesRespectingHiddenRowIndexes()
		{
			SummaryBase[] parentSummaries = GetEmptySummaries();
			int startRow = 1;
			UpdateSummariesForNextLevel(0, parentSummaries, ref startRow);

			for (int k = 0; k < PivotCalculations.Count; ++k)
			{
				pivotValues[startRow, PivotRows.Count + k].Summary = parentSummaries[k];
				InitSummary(startRow, PivotRows.Count + k, parentSummaries[k]);
			}
		 }
        /// <summary>
        /// GetHiddenRowKeyValueColumnIndex
        /// </summary>
        /// <returns>int</returns>
		public int GetHiddenRowKeyValueColumnIndex()
		{
			if (columnIndexes != null && columnIndexes.Count > 0)
			{
				for (int i = 0; i < columnIndexes.Count; ++i)
				{
					if (columnIndexes[i] == PivotRows.Count)
						return i;
				}
			}
			return PivotRows.Count;
		}
        /// <summary>
        /// Gets the UnindexedPivotCellInfo 
        /// </summary>
        /// <param name="row">int</param>
        /// <param name="col">int</param>
        /// <returns>PivotCellInfo</returns>
		public PivotCellInfo GetUnindexedPivotCellInfo(int row, int col)
		{
		   
			return this.pivotValues[row, col];
		}
		//recursively called method to iterate through all levels and redo the calculations at each level.
        private void UpdateSummariesForNextLevel(int levelCol, SummaryBase[] parentSummaries, ref int startRow)
        {
            if (levelCol == PivotRows.Count - 1)
            {
                int i = startRow;
                int col = PivotRows.Count;

                while (i < RowCount - 1 && pivotValues[i, col].CellType == PivotCellType.ValueCell)
                {
                    if (!HiddenRowIndexes.Contains(this.pivotValues[i, col]))
                    {
                        for (int k = 0; k < PivotCalculations.Count; ++k)
                        {
                            if (this.pivotValues[i, col + k].Summary != null)
                            {
                                parentSummaries[k].CombineSummary(this.pivotValues[i, col + k].Summary);
                            }
                        }
                    }
                    i++;
                }
                startRow = i;
            }
            else
            {
                while (startRow < RowCount - PivotRows.Count
                            && 0 != (this.pivotValues[startRow, levelCol].CellType & PivotCellType.ExpanderCell)
                             && this.pivotValues[startRow, levelCol].CellType != (PivotCellType.GrandTotalCell | PivotCellType.RowHeaderCell)
                             && this.pivotValues[startRow, levelCol].Value.ToString() != "x"
                             )
                {
                    SummaryBase[] levelSummaries = GetEmptySummaries();
                    UpdateSummariesForNextLevel(levelCol + 1, levelSummaries, ref startRow);

                    for (int k = 0; k < PivotCalculations.Count; ++k)
                    {
                        this.pivotValues[startRow, PivotRows.Count + k].Summary = levelSummaries[k];
                        InitSummary(startRow, PivotRows.Count + k, levelSummaries[k]);
                    }
                    for (int k = 0; k < PivotCalculations.Count; ++k)
                    {
                        parentSummaries[k].CombineSummary(levelSummaries[k]);
                    }
                    startRow++;
                }
            }
        }

		private void CombineSummaries(SummaryBase[] levelSummaries, SummaryBase[] parentSummaries)
		{
			for (int i = 0; i < PivotCalculations.Count; ++i)
			{
				parentSummaries[i].CombineSummary(levelSummaries[i]);
			}
		}

		private SummaryBase[] GetEmptySummaries()
		{
			SummaryBase[] summaries = new SummaryBase[PivotCalculations.Count];
			for (int i = 0; i < PivotCalculations.Count; i++)
			{
				summaries[i] = PivotCalculations[i].Summary.GetInstance();
			}
			return summaries;
		}
		#endregion

        #region INotifyPropertyChanged Members
        /// <summary>
        /// PropertyChanged event that is triggered when the property of the cell is changed.
        /// </summary>
        public event PropertyChangedEventHandler PropertyChanged;

        private void OnPropertyChanged<R>(Expression<Func<FilterItemElement, R>> expr)
        {
            OnPropertyChanged(((MemberExpression)expr.Body).Member.Name);
        }

        private void OnPropertyChanged(string propertyName)
        {
            if (PropertyChanged != null)
                PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
        }


        #endregion

	}

	#endregion

	#region internal classes
    internal class IntComparer : IComparer
    {
        public int Compare(object x, object y)
        {
            int x1, y1, result;
            System.Globalization.NumberStyles ns = System.Globalization.NumberStyles.Currency;
            int.TryParse(x.ToString(), ns, null, out result);
            x1 = result;
            int.TryParse(y.ToString(), ns, null, out result);
            y1 = result;
            if (x1 == 0 && y1 == 0)
                return 0;
            else if (y1 == 0)
                return 1;
            else if (x1 == 0)
                return -1;

            else
                return x1.CompareTo(y1);
        }
    }
    internal class DateComparer : IComparer
    {
        /// <summary>
        /// Compare for DateTime objects
        /// </summary>
        /// <param name="x">object</param>
        /// <param name="y">object</param>
        /// <returns>integer</returns>
        public int Compare(object x, object y)
        {
            DateTime dummyX,dummyY;
            if (x == null && y == null)
                return 0;
            else if (y == null)
                return 1;
            else if (x == null)
                return -1;
            else if (DateTime.TryParse(x.ToString(), out dummyX) && DateTime.TryParse(y.ToString(), out dummyY))
                return DateTime.Compare(dummyX, dummyY);
            else
                return 0;
        }
    }
    internal class DoubleComparer : IComparer
    {
        public int Compare(object x, object y)
        {
            double result, x1, y1;
            System.Globalization.NumberStyles ns = System.Globalization.NumberStyles.Currency;
            double.TryParse(x.ToString(), ns, null, out result);
            x1 = result;
            double.TryParse(y.ToString(), ns, null, out result);
            y1 = result;
            if (x1 == 0.0 && y1 == 0.0)
                return 0;
            else if (y1 == 0.0)
                return 1;
            else if (x1 == 0.0)
                return -1;
            else
                return x1.CompareTo(y1);
        }
    }
    internal class DecimalComparer : IComparer
    {
        public int Compare(object x, object y)
        {
            decimal x1, y1, result;
            System.Globalization.NumberStyles ns = System.Globalization.NumberStyles.Currency;
            decimal.TryParse(x.ToString(), ns, null, out result);
            x1 = result;
            decimal.TryParse(y.ToString(), ns, null, out result);
            y1 = result;
            if (x1 == 0.0M && y1 == 0.0M)
                return 0;
            else if (y1 == 0.0M)
                return 1;
            else if (x1 == 0.0M)
                return -1;

            else
                return x1.CompareTo(y1);
        }
    }
	internal class KeysCalculationValues : IComparable
	{
		public List<IComparable> Keys { get; set; }
		public List<SummaryBase> Values { get; set; }
		public IComparer[] Comparers { get; set; }
        private List<object> _rawValues = new List<object>();
        public List<object> RawValues { get { return _rawValues; } }

		#region IComparable Members

		public int CompareTo(object obj)
		{
			int c = 0;
			if (Keys != null)
			{
				int i = 0;
				KeysCalculationValues objX = obj as KeysCalculationValues;
				if (objX != null)
				{
					while (c == 0 && i < Keys.Count && objX.Keys.Count > i)
					{
						if (Comparers != null && Comparers[i] != null)
						{
							c = Comparers[i].Compare(this.Keys[i], objX.Keys[i]);
						}
                        else if ((this.Keys[i] != " ") && (this.Keys[i] != null))
						{
                            if (objX.Keys[i] == null)
                            {
                                
                                c = -1;
                            }
                            else
                               c = this.Keys[i].CompareTo(objX.Keys[i]);
						}
                        else if ((this.Keys[i] == " "))
                        {
                            if ((objX.Keys[i] == null) || (objX.Keys[i] == " "))
                            {
                                c = 0;
                            }
                            else
                                c = -1;
                        }
						else
                            c = this.Keys[i].CompareTo(objX.Keys[i]);
						i++;
					}
				}
			}
			return c;
		}



		//public int CompareTo(object obj)
		//{
		//    int c = 0;
		//    if (Keys != null)
		//    {
		//        int i = 0;
		//        KeysCalculationValues objX = obj as KeysCalculationValues;
		//        if (objX != null)
		//        {
		//            while (c == 0 && i < Keys.Count)
		//            {
		//                if (Comparers != null && Comparers[i] != null)
		//                {
		//                    c = Comparers[i].Compare(this.Keys[i], objX.Keys[i]);
		//                    i++;
		//                }
		//                else
		//                {
		//                    c = this.Keys[i].CompareTo(objX.Keys[i]);
		//                    i++;
		//                }
		//            }
		//        }
		//    }
		//    return c;
		//}

		public override string ToString()
		{
			string key = string.Empty;
			foreach (var item in this.Keys)
			{
				key += (key == string.Empty) ? item.ToString() : "-" + item.ToString();
			}
			return key;
		}

		#endregion

	}

	#endregion

    /// <summary>
    ///  Class that holds different Grid Constants.
    /// </summary>
	public class PivotGridConstants
	{
        /// <summary>
        /// Variable that holds all the values in filter.
        /// </summary>
		public const string AllString = "(All)";
        /// <summary>
        /// Holds the Total String
        /// </summary>
		public const string TotalString = "Total";
	}

    /// <summary>
    /// Holds the different Filter Item's collection
    /// </summary>
	#region Filter Element Class
	public class FilterItemElement : IComparable, INotifyPropertyChanged
	{
		bool suspendPropertyChangedTrigger = false;
		/// <summary>
		/// Gets or Sets the Filter item's elements
		/// </summary>
		public FilterItemElement()
		{
			this._IsSelected = true;
			this.SelectedState = true;
		}
		/// <summary>
		/// Gets or Sets the key
		/// </summary>
		public string Key
		{
			get;
			set;
		}

		bool? _IsSelected;
		/// <summary>
		/// Gets or Sets the bool value of Selected option
		/// </summary>
		public bool? IsSelected
		{
			get
			{
				return _IsSelected;
			}
			set
			{
				_IsSelected = value;
				if (!suspendPropertyChangedTrigger)
					this.OnPropertyChanged(pc => pc.IsSelected);
			}
		}
		/// <summary>
		/// Gets or Sets the bool value of SelectedState
		/// </summary>

		public bool? SelectedState { get; set; }
		/// <summary>
		/// To accept the changes in Engine
		/// </summary>

		public void AcceptChanges()
		{
			this.suspendPropertyChangedTrigger = true;
			this.SelectedState = this.IsSelected;
			this.suspendPropertyChangedTrigger = false;
		}

		/// <summary>
		/// To reject the changes in Engine
		/// </summary>
		public void RejectChanges()
		{
			//this.suspendPropertyChangedTrigger = true;
			this.IsSelected = this.SelectedState;
			this.suspendPropertyChangedTrigger = false;
		}

		#region IComparable Members
		/// <summary>
		///  To compare the filter items
		/// </summary>
		/// <param name="obj">object</param>
		/// <returns>int</returns>
		public int CompareTo(object obj)
		{
			FilterItemElement filterItem = obj as FilterItemElement;
			if (filterItem != null && filterItem.Key != null)
			{
				return filterItem.Key.CompareTo(this.Key);
			}
			return 0;
		}

		#endregion
        /// <summary>
        /// Converts the data to String type.
        /// </summary>
        /// <returns></returns>
		public override string ToString()
		{
			return this.Key;
		}

		#region INotifyPropertyChanged Members
        /// <summary>
        /// PropertyChanged event that is triggered when the property of the cell is changed.
        /// </summary>
		public event PropertyChangedEventHandler PropertyChanged;

		private void OnPropertyChanged<R>(Expression<Func<FilterItemElement, R>> expr)
		{
			OnPropertyChanged(((MemberExpression)expr.Body).Member.Name);
		}

		private void OnPropertyChanged(string propertyName)
		{
			if (PropertyChanged != null)
				PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
		}


		#endregion

	}
    /// <summary>
    /// Class that holds the collection of the objects that are filtered.
    /// </summary>
#if !SILVERLIGHT
	[Serializable]
#else
#endif    
	public class FilterItemsCollection : List<FilterItemElement>
	{
		bool isSuspendUpdate = false;
        /// <summary>
        /// Constructor setting different properties for the filter collection
        /// </summary>
		public FilterItemsCollection()
		{
			this.AllFilterItem = new FilterItemElement();
			this.AllFilterItem.Key = PivotGridConstants.AllString;
			this.FilteredValues = new List<string>();
			this.AllFilterItem.PropertyChanged += new PropertyChangedEventHandler(PropertyChanged);
			this.Add(this.AllFilterItem);
		}

#if SILVERLIGHT
		public object FilterProperty
		{
			get;
			set;
		}

#else
        /// <summary>
        /// Gets and sets the Filter property.
        /// </summary>
		public PropertyDescriptor FilterProperty
		{
			get;
			set;
		}
#endif
        /// <summary>
        /// Gets and sets teh property for all the filter items.
        /// </summary>
		public FilterItemElement AllFilterItem { get; set; }

		private string m_Name = string.Empty;
        /// <summary>
        /// Gets and sets the data for the Name property.
        /// </summary>
		public string Name
		{
			get
			{
				if (m_Name == string.Empty || m_Name == null)
				{
					if (this.FilterProperty != null)
#if !SILVERLIGHT
						m_Name = this.FilterProperty.Name;
#else
                    {
                        if (this.FilterProperty is DynamicPropertyInfo)
                            m_Name = (this.FilterProperty as DynamicPropertyInfo).Name;
                        else if (this.FilterProperty is PropertyInfo)
                            m_Name = (this.FilterProperty as PropertyInfo).Name;
                        else if (this.FilterProperty is Syncfusion.PivotAnalysis.Base.Silverlight.ExpressionPropertyDescriptor)
                            m_Name = (this.FilterProperty as Syncfusion.PivotAnalysis.Base.Silverlight.ExpressionPropertyDescriptor).Name;
                    }
#endif
				}
				return m_Name;
			}
			set
			{
				if (this.FilterProperty != null)
#if !SILVERLIGHT
					m_Name = this.FilterProperty.Name;
#else
                {
                    if (this.FilterProperty is DynamicPropertyInfo)
                        m_Name = (this.FilterProperty as DynamicPropertyInfo).Name;
                    else if (this.FilterProperty is PropertyInfo)
                        m_Name = (this.FilterProperty as PropertyInfo).Name;
                    else if (this.FilterProperty is Syncfusion.PivotAnalysis.Base.Silverlight.ExpressionPropertyDescriptor)
                        m_Name = (this.FilterProperty as Syncfusion.PivotAnalysis.Base.Silverlight.ExpressionPropertyDescriptor).Name;
                }
#endif
				else
					m_Name = value;
			}
		}
		/// <summary>
		/// Gets or Sets DisplayHeader
		/// </summary>

		public string DisplayHeader { get; set; }
		/// <summary>
		/// Gets or sets the format
		/// </summary>
		public string Format { get; set; }
        /// <summary>
        /// Gets or Sets the value to show/hide the sub total of PivotItem. Default value is true
        /// </summary>
        private bool showSubTotal = true;
        public bool ShowSubTotal
        {
            get
            {
                return showSubTotal;
            }
            set
            {
                showSubTotal = value;
            }
        }

		private bool allowRunTimeGroupByField = true;
		/// <summary>
		/// Gets or sets the value to enable/disable grouping for this filter item. Default value is true.
		/// </summary>
		public bool AllowRunTimeGroupByField
		{
			get
			{
				return allowRunTimeGroupByField;
			}
			set
			{
				if (allowRunTimeGroupByField != value)
				{
					allowRunTimeGroupByField = value;
				}
			}
		}
		/// <summary>
		/// Add the filter Item elements at the respective location
		/// </summary>
		/// <param name="filterItemElement"></param>
		/// <returns>int</returns>
		public int AddIfUnique(FilterItemElement filterItemElement)
		{
			int loc = -1;
			if (filterItemElement != null)
			{
				loc = this.BinarySearch(filterItemElement);
				if (loc < 0)
				{
					this.Insert(-loc - 1, filterItemElement);
					filterItemElement.PropertyChanged += new PropertyChangedEventHandler(PropertyChanged);
				}
			}
			return loc;
		}
		/// <summary>
		/// Accept the changes in items
		/// </summary>
		public void AcceptChanges()
		{
			foreach (var item in this)
			{
				item.AcceptChanges();
			}
		}
		/// <summary>
		/// Rejects the changes in items
		/// </summary>
		public void RejectChanges()
		{
			foreach (var item in this)
			{
				item.RejectChanges();
			}
		}

		/// <summary>
		/// Method used to sets the Name of the FilterItem
		/// </summary>
		/// <param name="name">string</param>
		public void SetName(string name)
		{
			this.Name = name;
		}

		/// <summary>
		/// Add the filter items in collection
		/// </summary>
		/// <param name="element"></param>

		public void AddWireEvent(FilterItemElement element)
		{
			this.Add(element);
			element.PropertyChanged += new PropertyChangedEventHandler(PropertyChanged);
		}

		void PropertyChanged(object sender, PropertyChangedEventArgs e)
		{
			if (e.PropertyName == "IsSelected" && !this.isSuspendUpdate)
			{
				FilterItemElement filterItemElement = (FilterItemElement)sender;
				if (filterItemElement.Key == PivotGridConstants.AllString && !isSuspendUpdate)
				{
					this.isSuspendUpdate = true;
					foreach (var item in this)
					{
						if (item.Key != PivotGridConstants.AllString)
							item.IsSelected = filterItemElement.IsSelected;
					}
					this.isSuspendUpdate = false;
				}
				else
				{
					this.isSuspendUpdate = true;
					if (this.Count - 1 == this.Where(i => i.IsSelected == false && i.Key != PivotGridConstants.AllString).Count())
					{
						this.AllFilterItem.IsSelected = false;
					}
					else if (this.Count - 1 == this.Where(i => i.IsSelected == true && i.Key != PivotGridConstants.AllString).Count())
					{
						this.AllFilterItem.IsSelected = true;
					}
					else
					{
						this.AllFilterItem.IsSelected = null; // this.SeletedState();
					}

					this.isSuspendUpdate = false;
				}
			}
		}

		bool? SeletedState()
		{
			if (this.Count() > 1)
			{
				bool? isSeleted = this[1].IsSelected;

				foreach (var item in this)
				{
					if (item != this.AllFilterItem)
						if (item.IsSelected != isSeleted)
							return true;
				}
				return isSeleted;
			}
			return true;
		}

		/// <summary>
		///  Gets or Sets the filter values
		/// </summary>
		public List<string> FilteredValues { get; set; }
		/// <summary>
		///  To Get the DataView of Filter Expression
		/// </summary>
		/// <returns>string</returns>
		public string GetFilterExpressionForDataView()
		{
			if (this.Count > 0)
			{
				StringBuilder sb = new StringBuilder();
				bool orFlag = false;
				this.FilteredValues.Clear();
				foreach (FilterItemElement item in this)
				{
					if (item != this.AllFilterItem)
					{
						if (item.IsSelected.Value == true)
						{
							if (orFlag)
								sb.Append(" OR ");
							else
								orFlag = true;

							//sb.Append(this.FilterProperty.Name + " = " + item.Key);
							if (!string.IsNullOrEmpty(item.Key))
							{
#if !SILVERLIGHT
								sb.Append(string.Format("[{0}] = '{1}'", HandleBracketsInColumnNamesInFilters(this.FilterProperty.Name), item.Key.Contains("'")? item.Key.Replace("'","''"):item.Key));
#else
                                if (this.FilterProperty is DynamicPropertyInfo)
                                    sb.Append(string.Format("[{0}] = '{1}'", HandleBracketsInColumnNamesInFilters((this.FilterProperty as DynamicPropertyInfo).Name), item.Key.Contains("'")? item.Key.Replace("'","''"):item.Key));
                                else if (this.FilterProperty is PropertyInfo)
                                    sb.Append(string.Format("[{0}] = '{1}'", HandleBracketsInColumnNamesInFilters((this.FilterProperty as PropertyInfo).Name), item.Key.Contains("'")? item.Key.Replace("'","''"):item.Key));
                                else if(this.FilterProperty is Syncfusion.PivotAnalysis.Base.Silverlight.ExpressionPropertyDescriptor)
                                    sb.Append(string.Format("[{0}] = '{1}'", HandleBracketsInColumnNamesInFilters((this.FilterProperty as Syncfusion.PivotAnalysis.Base.Silverlight.ExpressionPropertyDescriptor).Name), item.Key.Contains("'")? item.Key.Replace("'","''"):item.Key));
#endif
                            }
							else
							{
#if !SILVERLIGHT
                                sb.Append(string.Format("IsNull({0}, 'Null Column') = 'Null Column' OR [{0}] = '{1}'", HandleBracketsInColumnNamesInFilters(this.FilterProperty.Name), item.Key.Contains("'") ? item.Key.Replace("'", "''") : item.Key));
#else
                                if (this.FilterProperty is DynamicPropertyInfo)
                                    sb.Append(string.Format("IsNull({0}, 'Null Column') = 'Null Column' OR [{0}] = '{1}'", HandleBracketsInColumnNamesInFilters((this.FilterProperty as DynamicPropertyInfo).Name), item.Key.Contains("'")? item.Key.Replace("'","''"):item.Key));
                                else if (this.FilterProperty is PropertyInfo)
                                    sb.Append(string.Format("IsNull({0}, 'Null Column') = 'Null Column' OR [{0}] = '{1}'", HandleBracketsInColumnNamesInFilters((this.FilterProperty as PropertyInfo).Name), item.Key.Contains("'")? item.Key.Replace("'","''"):item.Key));
                                else if(this.FilterProperty is Syncfusion.PivotAnalysis.Base.Silverlight.ExpressionPropertyDescriptor)
                                    sb.Append(string.Format("IsNull({0}, 'Null Column') = 'Null Column' OR [{0}] = '{1}'", HandleBracketsInColumnNamesInFilters((this.FilterProperty as Syncfusion.PivotAnalysis.Base.Silverlight.ExpressionPropertyDescriptor).Name), item.Key.Contains("'")? item.Key.Replace("'","''"):item.Key));
#endif
                            }

							if (!this.FilteredValues.Contains(item.Key))
								this.FilteredValues.Add(item.Key);

						}
						else
						{
							if (this.FilteredValues.Contains(item.Key))
								this.FilteredValues.Remove(item.Key);
						}
					}
				}
				return sb.ToString();
			}
			return string.Empty;

		}

		private string HandleBracketsInColumnNamesInFilters(string name)
		{
			name = name.Replace("\\", "\\\\");
			name = name.Replace("]", "\\]");
			return name;
		}
		/// <summary>
		/// To get the Expression of Filter Item
		/// </summary>
		/// <param name="IEnumerableSource">bool</param>
		/// <returns>string</returns>
		public string GetFilterExpression(bool IEnumerableSource)
		{
			if (IEnumerableSource)
			{
				if (this.Count > 0)
				{
					StringBuilder sb = new StringBuilder();
					const char underScoreMarker = (char)129;
					bool orFlag = false;
					this.FilteredValues.Clear();
					foreach (FilterItemElement item in this)
					{
						if (item != this.AllFilterItem)
						{
							if (item.IsSelected.Value == true)
							{
								if (orFlag)
									sb.Append(" || ");
								else
									orFlag = true;

#if !SILVERLIGHT
								if (this.FilterProperty is ExpressionPropertyDescriptor)
								{
									ExpressionPropertyDescriptor exp = this.FilterProperty as ExpressionPropertyDescriptor;
									string s = exp.Format;
									int i = -1;

									if (s != null && (i = s.IndexOf(":")) > -1)
									{
										s = s.Substring(i + 1).Replace("}", "");
										sb.Append("(" + exp.Expression + " ToString " + s + ")" + " = " + formatMarker + item.Key + formatMarker);
									}
									else
									{
										sb.Append(exp.Expression + " = " + item.Key);
									}

									//sb.Append(this.FilterProperty.Name + " = " + item.Key);
								}
								else
								{
									if (item.Key.Contains(" "))
									{
										item.Key = item.Key.Replace(" ", underScoreMarker.ToString());
									}
									sb.Append(this.FilterProperty.Name + " = " + item.Key);
								}
#else
								if (this.FilterProperty is Syncfusion.PivotAnalysis.Base.Silverlight.ExpressionPropertyDescriptor)
								{
									Syncfusion.PivotAnalysis.Base.Silverlight.ExpressionPropertyDescriptor exp = this.FilterProperty as Syncfusion.PivotAnalysis.Base.Silverlight.ExpressionPropertyDescriptor;
									string s = exp.Format;
									int i = -1;

                                    if (s != null && (i = s.IndexOf(":")) > -1)
                                    {
                                        s = s.Substring(i + 1).Replace("}", "");
                                        sb.Append("(" + exp.Expression + " ToString " +  s  + ")" + " = " + formatMarker + item.Key + formatMarker);
                                    }
                                    else
                                    {
                                        sb.Append(exp.Expression + " = " + item.Key);
                                    }
                                    //sb.Append(exp.Expression + " = " + item.Key);
                                }
                                else if (this.FilterProperty is DynamicPropertyInfo)
                                {
                                    sb.Append((this.FilterProperty as DynamicPropertyInfo).Name + " = " + item.Key);
                                }
                                else if (this.FilterProperty is PropertyInfo)
                                {
                                    sb.Append((this.FilterProperty as PropertyInfo).Name + " = " + item.Key);
                                }
#endif
								if (!this.FilteredValues.Contains(item.Key))
									this.FilteredValues.Add(item.Key);

							}

							else
							{
								if (this.FilteredValues.Contains(item.Key))
								{
									this.FilteredValues.Remove(item.Key);
								}
							}
						}
					}
					return sb.ToString();
				}
				return string.Empty;
			}
			else
			{
				if (this.Count > 0)
				{
					StringBuilder sb = new StringBuilder();
					bool orFlag = false;
					this.FilteredValues.Clear();
					foreach (FilterItemElement item in this)
					{
						if (item != this.AllFilterItem)
						{
							if (item.IsSelected.Value == true)
							{
								if (orFlag)
									sb.Append(" OR ");
								else
									orFlag = true;

#if !SILVERLIGHT
								if (this.FilterProperty is ExpressionPropertyDescriptor)
								{
									ExpressionPropertyDescriptor exp = this.FilterProperty as ExpressionPropertyDescriptor;
									sb.Append(exp.Expression + " = " + "'" + item.Key + "'");
								}
								else
									sb.Append(this.FilterProperty.Name + " = " + "'" + item.Key + "'");
#else
                                if (this.FilterProperty is ExpressionPropertyDescriptor)
                                {
                                    ExpressionPropertyDescriptor exp = this.FilterProperty as Syncfusion.PivotAnalysis.Base.Silverlight.ExpressionPropertyDescriptor;
                                    sb.Append(exp.Expression + " = " + item.Key);
                                }
                                else if (this.FilterProperty is DynamicPropertyInfo)
                                {
                                    sb.Append((this.FilterProperty as DynamicPropertyInfo).Name + " = " + item.Key);
                                }
                                else if (this.FilterProperty is PropertyInfo)
                                {
                                    sb.Append((this.FilterProperty as PropertyInfo).Name + " = " + item.Key);
                                }
#endif
								if (!this.FilteredValues.Contains(item.Key))
									this.FilteredValues.Add(item.Key);

							}
							else
							{
								if (this.FilteredValues.Contains(item.Key))
								{
									this.FilteredValues.Remove(item.Key);
								}
							}
						}
					}
					return sb.ToString();
				}
				return string.Empty;
			}
		}

		/// <summary>
		/// Gives the Items in Filter
		/// </summary>
		/// <returns>string</returns>
		public string GetFilterItem()
		{
			if (this.Count > 0)
			{
				StringBuilder sb = new StringBuilder();
				foreach (FilterItemElement item in this)
				{
					if (item != this.AllFilterItem && item.IsSelected.Value == true)
					{
						//if (orFlag)
						//    sb.Append(" OR ");
						//else
						//    orFlag = true;

						//sb.Append(this.FilterProperty.Name + " = " + item.Key);

						return item.Key;
					}
				}
				return sb.ToString();
			}
			return string.Empty;
		}

		private const char formatMarker = (char)157;

        public string GetFilterExpression(bool IEnumerableSource, FilterItemsCollection filterItemsCollection, string format)
        {
            if (IEnumerableSource)
            {
                if (this.Count > 0)
                {
                    StringBuilder sb = new StringBuilder();
                    const char underScoreMarker = (char)129;
                    this.FilteredValues.Clear();
                    foreach (FilterItemElement item in this)
                    {
                        bool orFlag = true;
                        if (item != this.AllFilterItem)
                        {
                            if (item.IsSelected.Value == true)
                            {
                                for( int i=0; i< filterItemsCollection.Count;i++)
                                {
#if SILVERLIGHT
                                    if (this.FilterProperty is Syncfusion.PivotAnalysis.Base.Silverlight.ExpressionPropertyDescriptor)
                                    {
                                        Syncfusion.PivotAnalysis.Base.Silverlight.ExpressionPropertyDescriptor exp = this.FilterProperty as Syncfusion.PivotAnalysis.Base.Silverlight.ExpressionPropertyDescriptor;
                                        string s = exp.Format;
                                        int i1 = -1;

                                        if (s != null && (i1 = s.IndexOf(":")) > -1)
                                        {
                                            s = s.Substring(i1 + 1).Replace("}", "");
                                            sb.Append("(" + exp.Expression + " ToString " + s + ")" + " = " + formatMarker + filterItemsCollection[i].Key + formatMarker);
                                            if (orFlag)
                                                sb.Append(" || ");
                                        }
                                        else
                                        {
                                            sb.Append(exp.Expression + " = " + filterItemsCollection[i].Key);
                                            if (orFlag)
                                                sb.Append(" || ");
                                        }
                                    }
                                    else if (this.FilterProperty is DynamicPropertyInfo)
                                    {
                                        if (filterItemsCollection[i] != filterItemsCollection.AllFilterItem)
                                        {
                                            DateTime t1 = Convert.ToDateTime(filterItemsCollection[i].Key);
                                            string fmt = format != null && format.Length > 0 ? string.Format("{{0:{0}}}", format) : null;
                                            string value = string.Format(fmt, t1);
                                            if (value == item.Key)
                                            {
                                                sb.Append((this.FilterProperty as DynamicPropertyInfo).Name + " = " + filterItemsCollection[i].Key);
                                                if (orFlag)
                                                    sb.Append(" || ");
                                            }
                                        }
                                    }
                                    else if (this.FilterProperty is PropertyInfo)
                                    {
                                        if (filterItemsCollection[i] != filterItemsCollection.AllFilterItem)
                                        {
                                            DateTime t1 = Convert.ToDateTime(filterItemsCollection[i].Key);
                                            string fmt = format != null && format.Length > 0 ? string.Format("{{0:{0}}}", format) : null;
                                            string value = string.Format(fmt, t1);
                                            if (value == item.Key)
                                            {
                                                sb.Append((this.FilterProperty as PropertyInfo).Name + " = " + filterItemsCollection[i].Key);
                                                if (orFlag)
                                                    sb.Append(" || ");
                                            }
                                        }
                                    }
#else
                                        if (this.FilterProperty is ExpressionPropertyDescriptor)
                                        {
                                            ExpressionPropertyDescriptor exp = this.FilterProperty as ExpressionPropertyDescriptor;
                                            sb.Append(exp.Expression + " = " + "'" + filterItemsCollection[i].Key + "'");
                                            if (orFlag)
                                                sb.Append(" || ");
                                        }
                                        else
                                        {
                                            if (filterItemsCollection[i] != filterItemsCollection.AllFilterItem)
                                            {
                                                DateTime t1 = Convert.ToDateTime(filterItemsCollection[i].Key);
                                                string fmt = format != null && format.Length > 0 ? string.Format("{{0:{0}}}", format) : null;
                                                string value = string.Format(fmt, t1);
                                                if (value == item.Key)
                                                {
                                                    sb.Append(this.FilterProperty.Name + " = " + filterItemsCollection[i].Key);
                                                    if (orFlag)
                                                        sb.Append(" || ");
                                                }
                                            }
                                        }
#endif
                                    if (!this.FilteredValues.Contains(item.Key))
                                        this.FilteredValues.Add(item.Key);
                                }
                            }
                            else
                            {
                                if (this.FilteredValues.Contains(item.Key))
                                {
                                    this.FilteredValues.Remove(item.Key);
                                }
                            }
                        }
                    }
                    if (sb.ToString().EndsWith(" || "))
                    {
                        sb.Remove(sb.Length - 4, 4);
                    }
                    return sb.ToString();
                }
                return string.Empty;
            }
            else
            {
                if (this.Count > 0)
                {
                    StringBuilder sb = new StringBuilder();
                    bool orFlag = false;
                    this.FilteredValues.Clear();
                    foreach (FilterItemElement item in this)
                    {
                        if (item != this.AllFilterItem)
                        {
                            if (item.IsSelected.Value == true)
                            {
                                for (int i = 0; i < filterItemsCollection.Count; i++)
                                {
#if SILVERLIGHT
                                    if (this.FilterProperty is Syncfusion.PivotAnalysis.Base.Silverlight.ExpressionPropertyDescriptor)
                                    {
                                        Syncfusion.PivotAnalysis.Base.Silverlight.ExpressionPropertyDescriptor exp = this.FilterProperty as Syncfusion.PivotAnalysis.Base.Silverlight.ExpressionPropertyDescriptor;
                                        string s = exp.Format;
                                        int i1 = -1;

                                        if (s != null && (i1 = s.IndexOf(":")) > -1)
                                        {
                                            s = s.Substring(i1 + 1).Replace("}", "");
                                            sb.Append("(" + exp.Expression + " ToString " + s + ")" + " = " + formatMarker + filterItemsCollection[i].Key + formatMarker);
                                            if (orFlag)
                                                sb.Append(" OR ");
                                        }
                                        else
                                        {
                                            sb.Append(exp.Expression + " = " + filterItemsCollection[i].Key);
                                            if (orFlag)
                                                sb.Append(" OR ");
                                        }
                                    }
                                    else if (this.FilterProperty is DynamicPropertyInfo)
                                    {
                                        if (filterItemsCollection[i] != filterItemsCollection.AllFilterItem)
                                        {
                                            DateTime t1 = Convert.ToDateTime(filterItemsCollection[i].Key);
                                            string fmt = format != null && format.Length > 0 ? string.Format("{{0:{0}}}", format) : null;
                                            string value = string.Format(fmt, t1);
                                            if (value == item.Key)
                                            {
                                                sb.Append((this.FilterProperty as PropertyInfo).Name + " = " + filterItemsCollection[i].Key);
                                                if (orFlag)
                                                    sb.Append(" OR ");
                                            }
                                        }
                                    }
                                    if (this.FilterProperty is PropertyInfo)
                                    {
                                        if (filterItemsCollection[i] != filterItemsCollection.AllFilterItem)
                                        {
                                            DateTime t1 = Convert.ToDateTime(filterItemsCollection[i].Key);
                                            string fmt = format != null && format.Length > 0 ? string.Format("{{0:{0}}}", format) : null;
                                            string value = string.Format(fmt, t1);
                                            if (value == item.Key)
                                            {
                                                sb.Append((this.FilterProperty as PropertyInfo).Name + " = " + filterItemsCollection[i].Key);
                                                if (orFlag)
                                                    sb.Append(" OR ");
                                            }
                                        }
                                    }
#else
                                    if (this.FilterProperty is ExpressionPropertyDescriptor)
                                    {
                                        ExpressionPropertyDescriptor exp = this.FilterProperty as ExpressionPropertyDescriptor;
                                        sb.Append(exp.Expression + " = " + "'" + filterItemsCollection[i].Key + "'");
                                        if (orFlag)
                                            sb.Append(" OR ");
                                    }
                                    else
                                    {
                                        sb.Append(this.FilterProperty.Name + " = " + "'" + filterItemsCollection[i].Key + "'");
                                        if (orFlag)
                                            sb.Append(" OR ");
                                    }
#endif
                                    if (!this.FilteredValues.Contains(item.Key))
                                        this.FilteredValues.Add(item.Key);
                                }
                            }
                            else
                            {
                                if (this.FilteredValues.Contains(item.Key))
                                {
                                    this.FilteredValues.Remove(item.Key);
                                }
                            }
                        }
                    }
                    if (sb.ToString().EndsWith(" OR "))
                    {
                        sb.Remove(sb.Length - 4, 4);
                    }
                    return sb.ToString();
                }
                return string.Empty;
            }
        }
    } 

	#endregion

	#region BinaryList class
    /// <summary>
    /// Gets and sets the filter's Binary List.
    /// </summary>
	public class BinaryList : List<IComparable>
	{
        /// <summary>
        /// Method that returns the Integer value after performing insertion.
        /// </summary>
        /// <param name="o">IComparable</param>
        /// <returns>int</returns>
        public int AddIfUnique(IComparable o)
        {
            return AddIfUnique(o, false);
        }

        internal int AddIfUnique(IComparable o, bool ShouldRefreshKeys)
        {
            int loc = -1;
            if (o != null)
            {
                if (ShouldRefreshKeys && !(o as KeysCalculationValues).Keys.All(x => x == " "))
                {
                    IComparable temp = new KeysCalculationValues() { Keys = new List<IComparable>() }; ;
                     
                     foreach (IComparable key in (o as KeysCalculationValues).Keys)
                     {
                         (temp as KeysCalculationValues).Keys.Add(key);
                     }

                     for (int i = 0; (temp as KeysCalculationValues).Keys != null && i < (temp as KeysCalculationValues).Keys.Count; i++)
                        if ((temp as KeysCalculationValues).Keys[i] == " ")
                            (temp as KeysCalculationValues).Keys[i] = null;
                    loc = this.BinarySearch(temp);
                }
                else
                    loc = this.BinarySearch(o);
                if (loc < 0)
                {
                    this.Insert(-loc - 1, o);
                }
            }
            return loc;
        }
	}
	#endregion

	#region PivotItem class
	/// <summary>
	/// Enacapulates the information needed to define a pivot item, for either a row or column pivot.
	/// </summary>
	/// <remarks>
	/// A pivot item is a property in the underlying data objects 
	/// that is used to grouped the data in a pivot table. You can add pivot items to both the 
	/// PivotColumns and PivotRows collection in a mutually exclusive manner.</remarks>
	public class PivotItem :INotifyPropertyChanged
	{
         /// <summary>
       /// Gets or sets the summary type for calculations use.
       /// </summary>
       public SummaryType SummaryType
       {
           get;
           set;
       }


       /// <summary>
       /// Gets or sets the summary for calculations use.
       /// </summary>

       public SummaryBase Summary
       {
           get;
           set;
       }
       private bool showSubTotal = true;

        /// <summary>
        /// Gets or sets whether the subtotal for this item can be shown or hidden
        /// </summary>
        public bool ShowSubTotal
        {
            get 
            { 
                return showSubTotal; 
            }
            set 
            { 
                showSubTotal = value;
                OnPropertyChanged("ShowSubTotal");
            }
        }

       
		private bool allowSort = false;

		/// <summary>
		/// Gets or sets whether this calculation column can be sorted when
		/// RowPivotsOnly is true in the PivotEngine. 
		/// </summary>
		public bool AllowSort
		{
			get { return allowSort; }
			set 
            { 
                allowSort = value;
                OnPropertyChanged("AllowSort");
            }
		}
		private bool allowFilter = false;

		/// <summary>
		/// Gets or sets whether this calculation column can be filtered when
		/// RowPivotsOnly is true in the PivotEngine. 
		/// </summary>
		public bool AllowFilter
		{
			get { return allowFilter; }
			set
            {
                allowFilter = value;
                OnPropertyChanged("AllowFilter");
            }
		}

		private bool enableHyperLinks = false;

		/// <summary>
		/// Gets or sets whether this row pivot column should be hyperlinked when RowPivotsOnly is true in the PivotEngine.
		/// </summary>
		public bool EnableHyperlinks
		{
			get { return enableHyperLinks; }
			set 
            { 
                enableHyperLinks = value;
                OnPropertyChanged("EnableHyperlinks");
            }
		}

		/// <summary>
		/// Gets or sets the property's mappingname.
		/// </summary>
		public string FieldMappingName { get; set; }
		/// <summary>
		/// Gets or sets the title you want to see in the header for this pivot item.
		/// </summary>
		public string FieldHeader { get; set; }

		/// <summary>
		/// Gets or sets the string you want appended to the pivot item's summary cells.
		/// </summary>
		public string TotalHeader { get; set; }
        /// <summary>
        /// Gets or dets the format for the string
        /// </summary>
		public string Format { get; set; }

		/// <summary>
		/// Gets or sets the IComparer object used for sorting. If this value is null, then sorting is done assuming this field is IComparable.
		/// </summary>
		[XmlIgnore]
		public IComparer Comparer { get; set; }

		private bool allowRunTimeGroupByField = true;
		/// <summary>
		/// Gets or sets the value to enable/disable grouping for this pivot item. Default value is true.
		/// </summary>
        public bool AllowRunTimeGroupByField
        {
            get
            {
                return allowRunTimeGroupByField;
            }
            set
            {
                allowRunTimeGroupByField = value;
                OnPropertyChanged("AllowRunTimeGroupByField");
            }
        }
	

        #region INotifyPropertyChanged Members
        /// <summary>
        /// PropertyChanged event that is triggered when the property of the item is changed.
        /// </summary>
        public event PropertyChangedEventHandler PropertyChanged;

        private void OnPropertyChanged<R>(Expression<Func<FilterItemElement, R>> expr)
        {
            OnPropertyChanged(((MemberExpression)expr.Body).Member.Name);
        }

        private void OnPropertyChanged(string propertyName)
        {
            if (PropertyChanged != null)
                PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
        }
        #endregion
    }

	#endregion
    #region HiddenGroup
    /// <summary>
    /// Class used to hold the properties of each hidden group
    /// </summary>
    public class HiddenGroup
    {
        public HiddenGroup()
        {

        }
        public HiddenGroup(int from, int to, int level, string groupName)
            : this(from, to, level, groupName, String.Empty)
        {

        }


        public HiddenGroup(int from, int to, int level, string groupName, string totalHeader)
        {
            this.From = from;
            this.To = to;
            this.Level = level;
            this.GroupName = groupName;
            this.ItemTotalHeader = totalHeader;
        }

        public int From { get; set; }

        public int To { get; set; }

        public int Level { get; set; }

        public string GroupName { get; set; }

        public string ItemTotalHeader { get; set; }

        public override string ToString()
        {
            return string.Format("{0}-{1}", this.From, this.To);
        }

        public HiddenGroup Clone(HiddenGroup ParentGroup)
        {
            HiddenGroup m_HiddenGroup = new HiddenGroup();
            m_HiddenGroup.From = ParentGroup.From;
            m_HiddenGroup.To = ParentGroup.To;
            m_HiddenGroup.GroupName = ParentGroup.GroupName;
            m_HiddenGroup.Level = ParentGroup.Level;
            return m_HiddenGroup;
        }
    }
    /// <summary>
    /// Class which has method to determines whether the specified item is in the collection
    /// </summary>
    public static class ExtensionClass
    {
        /// <summary>
        /// Determines whether the specified item is in the collection
        /// </summary>
        /// <param name="t">The Collection.</param>
        /// <param name="hiddenGroup">The hidden group.</param>
        public static bool Has(this List<HiddenGroup> t, HiddenGroup hiddenGroup)
        {
            bool isPresent = false;

            foreach (var item in t)
            {
                if (item.From == hiddenGroup.From && item.To == hiddenGroup.To &&
             item.GroupName == hiddenGroup.GroupName && item.Level == hiddenGroup.Level)
                {
                    isPresent = true;
                    break;
                }
            }

            return isPresent;
        }
    }
    #endregion

    #region SummaryPivotItem class

    /// <summary>
	/// This class is primarily for internal use. It is used to generate the rows and columns that hold summaries of pivot calculations.
	/// </summary>
	public class SummaryPivotItem : IComparable
	{
		/// <summary>
		/// Gets or sets the row index of this item.
		/// </summary>
		public int RowIndex { get; set; }

		/// <summary>
		/// Gets or sets the column index of this item.
		/// </summary>
		public int ColIndex { get; set; }

		/// <summary>
		/// Gets or sets the list of calculations in the pivot item.
		/// </summary>
		public List<SummaryBase> Values { get; set; }

		/// <summary>
		/// Gets or sets row/column key values for an indexed look-up of this item.
		/// </summary>
		public List<IComparable> Keys { get; set; }

		#region IComparable Members

		/// <summary>
		/// IComparable implementation using the Keys.
		/// </summary>
		/// <param name="obj"></param>
		/// <returns></returns>
		public int CompareTo(object obj)
		{
			int c = 0;
			if (Keys != null)
			{
				int i = 0;
				SummaryPivotItem objX = obj as SummaryPivotItem;
				if (objX != null)
				{
					c = this.Keys.Count.CompareTo(objX.Keys.Count);
					while (c == 0 && i < Keys.Count)
					{
						c = this.Keys[i].CompareTo(objX.Keys[i++]);
					}
				}
			}
			return c;
		}
		#endregion
	}

	#endregion

	#region CoveredCellRange class
	/// <summary>
	/// This class defines a set of four integers that define a covered range in the zero-based coordinate system of a pivot table.
	/// </summary>
	public class CoveredCellRange
	{
		/// <summary>
		/// Gets or sets the top index.
		/// </summary>
		public int Top { get; set; }
		/// <summary>
		/// Gets or sets the left indeex.
		/// </summary>
		public int Left { get; set; }
		/// <summary>
		/// Gets or sets the bottom index.
		/// </summary>
		public int Bottom { get; set; }
		/// <summary>
		/// Gets or sets the right index.
		/// </summary>
		public int Right { get; set; }
        /// <summary>
        /// Gets the Range of the covered cells
        /// </summary>
		public CoveredCellRange()
		{

		}

		/// <summary>
		/// Contructor.
		/// </summary>
		/// <param name="top"></param>
		/// <param name="left"></param>
		/// <param name="bottom"></param>
		/// <param name="right"></param>
		public CoveredCellRange(int top, int left, int bottom, int right)
		{
			this.Top = top;
			this.Bottom = bottom;
			this.Left = left;
			this.Right = right;
		}

		/// <summary>
		/// overridden.
		/// </summary>
		/// <returns></returns>
		public override string ToString()
		{
			return string.Format("({0},{1})-({2},{3})", Top, Left, Bottom, Right);
		}
	}

	#endregion

	#region PivotCellInfo class

	/// <summary>
	/// This class provides information about a specific cell in a pivot table.
	/// </summary>
	public class PivotCellInfo
	{
        /// <summary>
        /// Gets the information of the pivot cells
        /// </summary>
		public PivotCellInfo()
		{
			CellType = PivotCellType.ValueCell;
		}

		/// <summary>
		/// Gets the double value.
		/// </summary>
		/// <value>The double value.</value>
		public double DoubleValue
		{
			get
			{
				if (this.Value != null)
				{
					double val = 0.0;
					if (double.TryParse(this.Value.ToString(), out val))
					{
						return val;
					}
				}
				return 0;
			}
		}

		/// <summary>
		/// Gets or sets the value in the cell.
		/// </summary>
		public object Value { get; set; }
		/// <summary>
		/// Gets or sets any covered range associated with this cell. 
		/// </summary>
		public CoveredCellRange CellRange { get; set; }
		/// <summary>
		/// Gets or sets the <see cref="PivotCellType"/> of this cell.
		/// </summary>
		public PivotCellType CellType { get; set; }
		
		/// <summary>
		/// Gets or sets the formatted text displayed in this cell.
		/// </summary>
        
        public string Key { get; set; }

		public string FormattedText { get; set; }
        /// <summary>
        /// Gets or Sets the format for the cells
        /// </summary>
		public string Format { get; set; }
        /// <summary>
        /// Gets and sets the Name.
        /// </summary>
		public object Tag { get; set; }
        /// <summary>
        /// Gets and sets the Summary  for the cell.
        /// </summary>
		public SummaryBase Summary { get; set; }
		/// <summary>
		/// Gets or sets the parent cell.
		/// </summary>
		/// <value>The parent cell.</value>
		public PivotCellInfo ParentCell { get; set; }

		/// <summary>
		/// Gets or sets the unique text.
		/// </summary>
		public string UniqueText { get; set; }

        /// <summary>
        /// Returns the string type of the data
        /// </summary>
        /// <returns></returns>
		public override string ToString()
		{
			return this.FormattedText;
		}
        /// <summary>
        /// Gets the RawValues of the particular PivotCellInfo 
        /// </summary>
        public List<object> RawValues { get; internal set; }
	}
    /// <summary>
    /// Gets the Information for the Pivot Grid.
    /// </summary>
	public class PivotCellInfos : List<List<PivotCellInfo>>
	{
        /// <summary>
        /// Empty constructor for Pivot Grid.
        /// </summary>
		public PivotCellInfos()
		{

		}
        /// <summary>
        /// Constructor accepting the rowcount and colCount.
        /// </summary>
        /// <param name="rowCount">int</param>
        /// <param name="colCount">int</param>
		public PivotCellInfos(int rowCount, int colCount)
		{
			this.Capacity = rowCount;
			for (int i = 0; i < rowCount; ++i)
			{
				//List<PivotCellInfo> list = new List<PivotCellInfo>(colCount);
				List<PivotCellInfo> list = new List<PivotCellInfo>(new PivotCellInfo[colCount]);
				this.Add(list);
			}
		}
        /// <summary>
        /// Gets the length of the value at specified index.
        /// </summary>
        /// <param name="index">int</param>
        /// <returns>int</returns>
		public int GetLength(int index)
		{
			return index == 0 ? this.Count : this[0].Count;
		}
        /// <summary>
        /// Gets and sets the rowIndex and colIndex.
        /// </summary>
        /// <param name="rowIndex">rowIndex</param>
        /// <param name="colIndex">colIndex</param>
        /// <returns></returns>
		public PivotCellInfo this[int rowIndex, int colIndex]
		{
			get { return this[rowIndex][colIndex]; }
			set { this[rowIndex][colIndex] = value; }
		}
	}

	#endregion

	#region PivotCellType enumeration

	/// <summary>
	/// Enumerates the possible pivot cell types.
	/// </summary>
	[Flags]
	public enum PivotCellType
	{
		/// <summary>
		/// Cell holds a summary value.
		/// </summary>
		ValueCell = 1,
		/// <summary>
		/// Cell is a row/column header that holds an expander.
		/// </summary>
		ExpanderCell = 2,
		/// <summary>
		/// Cell is a non-expander row or column header.
		/// </summary>
		HeaderCell = 4,
		/// <summary>
		/// Cell is the top left portion of the pivot table.
		/// </summary>
		TopLeftCell = 8,
		/// <summary>
		/// Cell is a row/column header that marks a total row or column.
		/// </summary>
		TotalCell = 16,
		/// <summary>
		/// Cell is a header cell holding a calculation name.
		/// </summary>
		CalculationHeaderCell = 32,
		/// <summary>
		/// Cell is row header
		/// </summary>
		RowHeaderCell = 64,
		/// <summary>
		/// Cell is column header
		/// </summary>
		ColumnHeaderCell = 128,
		/// <summary>
		/// Cell is a grand total cell.
		/// </summary>
		GrandTotalCell = 256 //,
		//MarkA = 512,
		// MarkB = 1024,
		//MarkC = 2048,
		// MarkD = 4096,
		// MarkE = 8192
	}

	#endregion

	#region Pivot SchemaChanged event
	/// <summary>
    /// Event handler for the <see cref="PivotSchemaChangedEventHandler"/> event.
	/// </summary>
	/// <param name="sender">The PivotEngine raising the event.</param>
	/// <param name="e">The event arguments.</param>
	public delegate void PivotSchemaChangedEventHandler(object sender, PivotSchemaChangedArgs e);

	/// <summary>
    /// Event argument for the <see cref="PivotSchemaChangedArgs"/> event.
	/// </summary>
	public class PivotSchemaChangedArgs : EventArgs
	{
		/// <summary>
		/// Gets or sets <see cref="SchemaChangeHints"/> regarding the schema information changed.
		/// </summary>
		public SchemaChangeHints ChangeHints { get; set; }

		/// <summary>
		/// Gets or sets weather should override the defer layout update settings
		/// </summary>
		public bool OverrideDeferLayoutUpdate { get; set; }
	}

	/// <summary>
	/// Enumerates information on what changes have occurred in the pivot schema.
	/// </summary>
	[Flags]
	public enum SchemaChangeHints
	{
		/// <summary>
		/// No specific information available about the change. The entire engine should be populated to ensure the change is properly reflected.
		/// </summary>
		None = 0,
		/// <summary>
        /// Indicate the engine's <see cref="GrandTotalVisibility"/> property has changed.
		/// </summary>
		GrandTotalVisibility = 1,
		/// <summary>
		/// Indicate that the engine should refresh because of a new row being added.
		/// </summary>
		RowAdded = 2,
		/// <summary>
		/// Indicate that the engine should re-populate based on selected calculation type.
		/// </summary>
		CalculationChanged = 3,
		/// <summary>
		/// Indicate that the engine should re-populate the headers based on field headers.
		/// </summary>
		HeadersChanged = 4
	}
	#endregion

	#region FieldInfo class
    /// <summary>
    /// Gets the information about the field.
    /// </summary>
	public class FieldInfo : IComparable
	{
        /// <summary>
        /// Gets and sets the field type
        /// </summary>
		public FieldTypes FieldType { get; set; }
        /// <summary>
        /// Gets and sets the Name
        /// </summary>
		public string Name { get; set; }
        /// <summary>
        /// Gets and sets the Expression
        /// </summary>
		public string Expression { get; set; }
        /// <summary>
        /// Gets and sets the Format
        /// </summary>
		public string Format { get; set; }

        /// <summary>
        /// Returns the boolean value for the function
        /// </summary>
        /// <param name="obj">object</param>
        /// <returns></returns>
		public override bool Equals(object obj)
		{
			if (obj is FieldInfo)
			{
				return this.Name == ((FieldInfo)obj).Name;
			}
			else if (obj is string)
			{
				return this.Name == ((string)obj);
			}
			return false;
		}
        /// <summary>
        /// REturnd the Hash code for the given value.
        /// </summary>
        /// <returns></returns>
		public override int GetHashCode()
		{
			if(Name !=null)
				return Name.GetHashCode();
			return 0;
		}

		#region IComparable Members
        /// <summary>
        /// Comapres the value with the other values
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
		public int CompareTo(object obj)
		{
			if (obj == null)
				return 1;
			else
			{
				return this.Name.CompareTo(((FieldInfo)obj).Name);
			}
		}

		#endregion
	}

	#region Expression PropertyDescriptor

#if !SILVERLIGHT
	/// <summary>
	/// A class that illustrates the expression field support along with filtering in PivotEngine.
	/// </summary>
	public class ExpressionPropertyDescriptor : PropertyDescriptor
	{
		string expression;

		/// <summary>
		/// Gets or sets the expression.
		/// </summary>
		public string Expression
		{
			get { return expression; }
			set { expression = value; }
		}
		string name;
		FilterHelper helper;
		FilterExpression exp;

		/// <summary>
		/// Gets or sets the logical expression for filtering.
		/// </summary>
		public FilterExpression Exp
		{
			get { return exp; }
			set { exp = value; }
		}
		string format;

		/// <summary>
		/// Gets or sets a format associated with the value of this expression.
		/// </summary>
		public string Format
		{
			get { return format; }
			set { format = value; }
		}

		/// <summary>
		/// A Constructor that initializes the attributes of ExpressionPropertyDescriptor object.
		/// </summary>
		/// <param name="name">Name of the Expression Field.</param>
		/// <param name="attributes">Attributes</param>
		/// <param name="expression">Expression</param>
		/// <param name="format">Format</param>
		/// <param name="helper">An object that holds attributes to compute filter values and expressions.</param>
		public ExpressionPropertyDescriptor(string name, Attribute[] attributes, string expression, string format, FilterHelper helper)
			: base(name, attributes)
		{
			this.name = name;
			this.expression = expression;
			this.helper = helper;
			this.format = format;

			exp = new FilterExpression(name, expression);
			if (exp.Error != ExpressionError.None)
				throw new ArgumentException(string.Format("Invalid expression: {0}", exp.ErrorString));
		}
        /// <summary>
        /// Resets the value
        /// </summary>
        /// <param name="component"></param>
        /// <returns>object</returns>
		public override bool CanResetValue(object component)
		{
			return true;
		}
        /// <summary>
        /// Gets and sets the ComponentType
        /// </summary>
		public override Type ComponentType
		{
			get { return typeof(object); }
		}
        /// <summary>
        /// Gets the value of the concerned data
        /// </summary>
        /// <param name="component"></param>
        /// <returns>object</returns>
		public override object GetValue(object component)
		{
			return format != null && format.Length > 0 ? string.Format(CultureInfo.CurrentUICulture, format, exp.ComputedValue(component)) : exp.ComputedValue(component);
		}
        /// <summary>
        /// Checks whether the data is read-only
        /// </summary>
		public override bool IsReadOnly
		{
			get { return true; }
		}
        /// <summary>
        /// Returns the PropertyType.
        /// </summary>
		public override Type PropertyType
		{
			get { return typeof(object); }
		}
        /// <summary>
        /// Resets the value
        /// </summary>
        /// <param name="component">object</param>
		public override void ResetValue(object component)
		{
			exp = new FilterExpression(name, expression);
			if (exp.Error != ExpressionError.None)
				throw new ArgumentException(string.Format("Invalid expression: {0}", exp.ErrorString));
		}
        /// <summary>
        /// Sets the value for the method
        /// </summary>
        /// <param name="component">object</param>
        /// <param name="value">object</param>
		public override void SetValue(object component, object value)
		{
			//readonly
		}
        /// <summary>
        /// 
        /// </summary>
        /// <param name="component"></param>
        /// <returns></returns>
		public override bool ShouldSerializeValue(object component)
		{
			return false;
		}
	}
#else
	/// <summary>
	/// A class that illustrates the expression field support along with filtering in PivotEngine.
	/// </summary>
	public class ExpressionPropertyDescriptor
	{
		private string expression;
		private string name;
		private string format;
		private FilterHelper helper;
		private FilterExpression exp;     

		/// <summary>
		/// A Constructor that initializes the attributes of ExpressionPropertyDescriptor object.
		/// </summary>
		/// <param name="name">Name of the Expression Field.</param>
		/// <param name="attributes">Attributes</param>
		/// <param name="expression">Expression</param>
		/// <param name="format">Format</param>
		/// <param name="helper">An object that holds attributes to compute filter values and expressions.</param>
		public ExpressionPropertyDescriptor(string name, Attribute[] attributes, string expression, string format, FilterHelper helper)
		{
			this.name = name;
			this.expression = expression;
			this.helper = helper;
			this.format = format;

			exp = new FilterExpression(name, expression);
			if (exp.Error != ExpressionError.None)
				throw new ArgumentException(string.Format("Invalid expression: {0}", exp.ErrorString));
		}

		/// <summary>
		/// Gets or sets the expression.
		/// </summary>
		public string Expression
		{
			get { return expression; }
			set { expression = value; }
		}        

		/// <summary>
		/// Gets or sets the logical expression for filtering.
		/// </summary>
		public FilterExpression Exp
		{
			get { return exp; }
			set { exp = value; }
		}

		/// <summary>
		/// Gets or sets the name of the expression field.
		/// </summary>
		public string Name
		{
			get { return name; }
			set { name = value; }
		}

		/// <summary>
		/// Gets or sets the display format of the computed value.
		/// </summary>
		public string Format
		{
			get { return format; }
			set { format = value; }
		}

		/// <summary>
		/// For internal use.
		/// </summary>
		internal FilterHelper Helper
		{
			get { return helper; }
			set { helper = value; }
		}

		/// <summary>
		/// Gets the type of the property. For internal use.
		/// </summary>
		public Type PropertyType
		{
			get { return typeof(object); }
		}

		/// <summary>
		/// Gets the computed value of the expression.
		/// </summary>
		public object GetValue(object component)
		{
			return format != null && format.Length > 0 ? string.Format(CultureInfo.CurrentUICulture, format, exp.ComputedValue(component)) : exp.ComputedValue(component);
		}
	}
#endif
	#endregion
    /// <summary>
    /// Enum holding the field tye
    /// </summary>
	public enum FieldTypes
	{
        /// <summary>
        /// Gets whether the Field is Property
        /// </summary>
		Property,
        /// <summary>
        /// Gets whether the Field is Expression
        /// </summary>
		Expression,
        /// <summary>
        /// Gets whether the Field is Unbound
        /// </summary>
		Unbound
	}
    #endregion
    #region GridLayout Enums
    /// <summary>
    /// Specifies the layout for the PivotGridControl
    /// </summary>
    public enum GridLayout
    {
        /// <summary>
        /// Subtotals will be shown after the details
        /// </summary>
        Normal,
        /// <summary>
        /// Subtotals will be shown before the details
        /// </summary>
        TopSummary
    }
	#endregion

#if !SILVERLIGHT
    /// <summary>
    /// Class that holds the methods for setting the value for the variable
    /// </summary>
	public class DynamicPropertyDescriptor : PropertyDescriptor
	{
        /// <summary>
        /// Constructor for DynamicPropertyDescriptor 
        /// </summary>
        /// <param name="name">string</param>
        /// <param name="attributes">Attribute</param>
		public DynamicPropertyDescriptor(string name, Attribute[] attributes)
			: base(name, attributes)
		{
		}
        /// <summary>
        /// Returns the value whether the value is reset. 
        /// </summary>
        /// <param name="component">object</param>
        /// <returns></returns>
		public override bool CanResetValue(object component)
		{
			return false;
		}

        /// <summary>
        /// Returns the component type.
        /// </summary>
		public override Type ComponentType
		{
			get { return typeof(object); }
		}
        /// <summary>
        /// Gets the Values of the value of the type dynamicDictionary
        /// </summary>
        /// <param name="component"></param>
        /// <returns>object</returns>
		public override object GetValue(object component)
		{
			IDictionary<string, object> dynamicDictionary = component as IDictionary<string, object>;
			if (dynamicDictionary != null)
			{
				return dynamicDictionary[Name];
			}
			return null;
		}
        /// <summary>
        /// Sets the Property whether the variable is ReadOnly
        /// </summary>
		public override bool IsReadOnly
		{
			get { return true; }
		}
        /// <summary>
        /// Gets the PropertyType
        /// </summary>
		public override Type PropertyType
		{
			get { return typeof(object); }
		}
        /// <summary>
        /// Resets the value of the variables
        /// </summary>
        /// <param name="component">object</param>
		public override void ResetValue(object component)
		{

		}
        /// <summary>
        /// sets the value for the value
        /// </summary>
        /// <param name="component">object</param>
        /// <param name="value">object</param>
		public override void SetValue(object component, object value)
		{

		}
        /// <summary>
        /// Returns boolean value Whether to serialize the values
        /// </summary>
        /// <param name="component"></param>
        /// <returns>bool</returns>
		public override bool ShouldSerializeValue(object component)
		{
			return false;
		}
	}
#else
    /// <summary>
    /// A class that provides access to metadata of dynamic property(.NET 4.0).
    /// </summary>
    public class DynamicPropertyInfo
    {
        /// <summary>
        /// Gets or sets the name of the dynamic property.
        /// </summary>
        public string Name
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the type of the dynamic property.
        /// </summary>
        public Type Type
        {
            get;
            set;
        }

        /// <summary>
        /// Returns the property value of the corresponding item.
        /// </summary>
        /// <param name="obj">item</param>
        /// <returns>property value</returns>
        public object GetValue(object obj)
        {
            IDictionary<string, object> dynamicDictionary = obj as IDictionary<string, object>;
            if (dynamicDictionary != null)
            {
                return dynamicDictionary[Name];
            }
            return null;
        }

        /// <summary>
        /// Sets the property value to the corresponding item.
        /// </summary>
        /// <param name="obj">item</param>
        /// <param name="value">property value</param>
        public void SetValue(object obj, object value)
        {
            IDictionary<string, object> dynamicDictionary = obj as IDictionary<string, object>;
            if (dynamicDictionary != null)
            {
                dynamicDictionary[Name] = obj;
            }
        }
    }
#endif
}