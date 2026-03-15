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
#if !SILVERLIGHT
using System.Data;
#endif
using System.Globalization;
using System.Reflection;
using System.ComponentModel;
#if XLSIO
namespace Syncfusion.XlsIO.Implementation.PivotAnalysis
#elif SILVERLIGHT
namespace Syncfusion.PivotAnalysis.Base.Silverlight
#else
namespace Syncfusion.PivotAnalysis.Base
#endif
{
    /// <summary>
    /// Specifies the Index
    /// </summary>
    public class IndexEngine
    {
        #region Constructors
        /// <summary>
        /// Constructor of IndexEngine with single paramater
        /// </summary>
        /// <param name="pivotEngine">PivotEngine</param>
        public IndexEngine(PivotEngine pivotEngine)
        {
            this.pivotEngine = pivotEngine;
            if (pivotEngine == null)
            {
                throw new NullReferenceException("pivotEngine cannot be null.");
            }
        }
        /// <summary>
        ///  Constructor of IndexEngine with two paramater
        /// </summary>
        /// <param name="pivotEngine">PivotEngine</param>
        /// <param name="del">GetValueDelegate</param>
        public IndexEngine(PivotEngine pivotEngine, GetValueDelegate del)
        {
            this.pivotEngine = pivotEngine;
            if (pivotEngine == null)
            {
                throw new NullReferenceException("pivotEngine cannot be null.");
            }
            this.GetValue = del;
        }
        /// <summary>
        /// Empty conStructor of IndexEngine class
        /// </summary>
        public IndexEngine()
        {

        }

        #endregion

        #region public APIs
#if DEBUG
        /// <summary>
        /// Used for timing the indexing process. After calling the IndexData method, IndexTimeSlice
        /// will hold the time it took to complete the pivoting.
        /// </summary>
        public static TimeSpan IndexTimeSlice = TimeSpan.FromSeconds(0); //for debug output
        public static TimeSpan IndexTimeSlice1 = TimeSpan.FromSeconds(0); //for debug output
        public static TimeSpan IndexTimeSlice2 = TimeSpan.FromSeconds(0); //for debug output
        public static TimeSpan IndexTimeSlice3 = TimeSpan.FromSeconds(0); //for debug output
        public static TimeSpan IndexTimeSlice4 = TimeSpan.FromSeconds(0); //for debug output
        public static TimeSpan IndexTimeSlice5 = TimeSpan.FromSeconds(0); //for debug output
        public static TimeSpan IndexTimeSlice6 = TimeSpan.FromSeconds(0); //for debug output
        public static TimeSpan IndexTimeSlice7 = TimeSpan.FromSeconds(0); //for debug output
        public static TimeSpan IndexTimeSlice8 = TimeSpan.FromSeconds(0); //for debug output
        public static TimeSpan IndexTimeSlice9 = TimeSpan.FromSeconds(0); //for debug output
#endif
        int highRowLevel = -1;
        /// <summary>
        /// Gets the index of the last row that has been loaded when you are using the on-demand loading.
        /// </summary>
        /// <remarks>
        /// You do on-demand loading by passing a true value when you can the IndexEngine method to populate
        /// the IndexEngine. When this is done, IndexEngine will returns after loading the number of rows 
        /// indicated <see cref="initialRowLoadAmount"/>. The rest of the rows will be populated on-demand 
        /// as your code requests a row higher than this property.
        /// </remarks>
        public int HighRowLevel
        {
            get { return highRowLevel; }

        }
        int initialRowLoadAmount = 40;

        /// <summary>
        /// Gets or sets the numner of rows that are initially loaded when you are using on-demand loading.
        /// </summary>
        /// <remarks>
        /// You do on-demand loading by passing a true value when you can the IndexEngine method to populate
        /// the IndexEngine. When this is done, IndexEngine will returns after loading the number of rows 
        /// indicated by this property. The rest of the rows will be populated on-demand as your code requests
        /// a row higher than <see cref="HighRowLevel"/>.
        /// </remarks>
        public int InitialRowLoadAmount
        {
            get { return initialRowLoadAmount; }
            set { initialRowLoadAmount = value; }
        }


      
        /// <summary>
        /// Call this method to have the IndexEngine create index information for the current
        /// content reflected in <see cref="DataSource"/>, <see cref="PivotRows"/>, <see cref="PivotColumns"/>
        /// and <see cref="PivotCalculations"/>.
        /// </summary>
        /// <returns>True if indexing was completed and false if no indexing was done.</returns>
        /// <remarks>
        /// After successfully executing this method, you can access the pivot contents using an zero-based
        /// row, column indexer on this IndexEngine.
        /// </remarks>
        public bool IndexData()
        {
            return IndexData(false);
        }

        /// <summary>
        /// Call this method to have the IndexEngine create index information for the current
        /// content reflected in <see cref="DataSource"/>, <see cref="PivotRows"/>, <see cref="PivotColumns"/>
        /// and <see cref="PivotCalculations"/>.
        /// </summary>
        /// <param name="onDemand">True if you want this method to return before all the rows are populated.</param>
        /// <returns>True if indexing was completed and false if no indexing was done.</returns>
        /// <remarks>
        /// If onDemand is true, the IndexEngine will returns when the number of rows given in <see cref="InitialRowLoadAmount"/> 
        /// have been loaded. After this method returns, the rest of the rows will be loaded on demand
        /// as you index this IndexEngine using row and column indexes. Anytime you index this IndexEngine,
        /// if your row request exceeds <see cref="HighRowLevel"/>, additional rows are populated until it is
        /// possible to return your requested value.
        /// </remarks>
        /// <see cref="PivotCalculations">        
        /// </see>
        public bool IndexData(bool onDemand)
        {
            if (onDemand)
            {
                highRowLevel = 0;
            }
            DateTime dt = DateTime.Now;

            sortKeys = null;

            bool b = DataSourceList == null;
            b = b || ((PivotColumns == null || PivotColumns.Count == 0) &&
                      (PivotRows == null || PivotRows.Count == 0) &&
                      (PivotCalculations == null || PivotCalculations.Count == 0));

            if (!b)
            {
                headerColsCount = PivotColumns.Count
                                 + ((PivotColumns.Count == 0 || PivotCalculations.Count > 1) ? 1 : 0);
                headerRowsCount = PivotRows.Count
                                 + ((PivotColumns.Count > 0 && PivotRows.Count == 0 && PivotCalculations.Count > 0) ? 1 : 0);

                if (PivotColumns.Count == 0 && PivotRows.Count == 0 && PivotCalculations.Count > 0)
                {
                    headerColsCount = PivotCalculations.Count;
                    headerRowsCount = 1;
                }

                this.rowOffset = headerColsCount + 1;
                this.columnOffset = headerRowsCount + 1;
                List<object> sortedRawList;
                //this list is a indexed (sorted of all row+column pivots) version of the DataSourceList list.
                if (DataSourceList is IEnumerable<object>)
                {
                    sortedRawList = new List<object>(DataSourceList as IEnumerable<object>);
                }
                else
                {
                    sortedRawList=new List<object>();
                    foreach (var item in DataSourceList)
                    {
                        sortedRawList.Add(item);
                    }
                }
                List<string> allPivotsUsedRowFirst = new List<string>();
                List<string> allFormatsUsedRowFirst = new List<string>();
                List<IComparer> allComparersUsedRowFirst = new List<IComparer>();
                List<string> columnPivotsUsed = new List<string>();
                List<string> columnFormatsUsed = new List<string>();
                List<IComparer> columnComparersUsed = new List<IComparer>();
                List<string> rowPivotsUsed = new List<string>();
                List<string> rowFormatsUsed = new List<string>();
                List<IComparer> rowComparersUsed = new List<IComparer>();

                if (PivotRows != null && PivotRows.Count > 0)
                {
                    rowPivotsUsed.AddRange(PivotRows.Select(pi => pi.FieldMappingName));
                    rowFormatsUsed.AddRange(PivotRows.Select(pi => pi.Format != null && pi.Format.Length > 0 ? "{0:" + pi.Format + "}" : null));
                    rowComparersUsed.AddRange(PivotRows.Select(pi => pi.Comparer));
                    allPivotsUsedRowFirst.AddRange(rowPivotsUsed);
                    allFormatsUsedRowFirst.AddRange(rowFormatsUsed);
                    allComparersUsedRowFirst.AddRange(rowComparersUsed);
                }
                if (PivotColumns != null && PivotColumns.Count > 0)
                {
                    columnPivotsUsed.AddRange(PivotColumns.Select(pi => pi.FieldMappingName));
                    columnFormatsUsed.AddRange(PivotColumns.Select(pi => pi.Format != null && pi.Format.Length > 0 ? "{0:" + pi.Format + "}" : null));
                    columnComparersUsed.AddRange(PivotColumns.Select(pi => pi.Comparer));
                    allPivotsUsedRowFirst.AddRange(columnPivotsUsed);
                    allFormatsUsedRowFirst.AddRange(columnFormatsUsed);
                    allComparersUsedRowFirst.AddRange(columnComparersUsed);
                }

                //first sort by the column pivots and get its indexes
#if DEBUG
                DateTime dt1 = DateTime.Now;
#endif
                List<object> tempRawList = new List<object>(sortedRawList);
#if DEBUG
                IndexTimeSlice1 = DateTime.Now.Subtract(dt1);
                dt1 = DateTime.Now;
#endif
                tempRawList.Sort(new SortComparer(columnPivotsUsed, columnFormatsUsed, columnComparersUsed, this.GetValue));
#if DEBUG
                IndexTimeSlice2 = DateTime.Now.Subtract(dt1);

                dt1 = DateTime.Now;
#endif
                this.columnIndexes = IndexTheList(tempRawList, columnPivotsUsed, columnFormatsUsed, columnComparersUsed, false);
#if DEBUG
                IndexTimeSlice3 = DateTime.Now.Subtract(dt1);
                dt1 = DateTime.Now;
#endif
                //then sort by the all pivots and get the row indexes and the indexes for all sorts.
                sortedRawList.Sort(new SortComparer(allPivotsUsedRowFirst, allFormatsUsedRowFirst, allComparersUsedRowFirst, this.GetValue));
#if DEBUG
                IndexTimeSlice4 = DateTime.Now.Subtract(dt1);
                //+++++
                dt1 = DateTime.Now;
#endif
                this.rowIndexes = IndexTheList(sortedRawList, rowPivotsUsed, rowFormatsUsed, rowComparersUsed, true);
#if DEBUG
                IndexTimeSlice5 = DateTime.Now.Subtract(dt1);
                dt1 = DateTime.Now;
#endif
                this.allIndexesRowFirst = IndexTheList(sortedRawList, allPivotsUsedRowFirst, allFormatsUsedRowFirst, allComparersUsedRowFirst, true);
#if DEBUG
                IndexTimeSlice6 = DateTime.Now.Subtract(dt1);
                dt1 = DateTime.Now;
#endif
                this.allComparersRowFirst = allComparersUsedRowFirst;
                //do the calculations....
                rowCount = GetRowDimension();
                columnCount = GetColumnDimension();
                ProcessCalcs(sortedRawList, rowPivotsUsed, rowIndexes);
                ProcessCalcs(sortedRawList, allPivotsUsedRowFirst, allIndexesRowFirst);
#if DEBUG
                IndexTimeSlice7 = DateTime.Now.Subtract(dt1);
                dt1 = DateTime.Now;
#endif

                //special cases
                if (PivotColumns.Count == 0 && PivotRows.Count == 0 && PivotCalculations.Count > 0)
                {
                    columnCount = PivotCalculations.Count + 2;
                    rowCount = 2;
                }
                else if (PivotColumns.Count == 1 && PivotRows.Count == 0 && PivotCalculations.Count == 0)
                {
                    columnCount = 3;
                    rowCount = 1;
                }

                pivotInfoCache = new PivotCellInfos(rowCount, columnCount);
                PopulateCache();
#if DEBUG
                IndexTimeSlice9 = DateTime.Now.Subtract(dt1);

                IndexTimeSlice = DateTime.Now.Subtract(dt);
#endif
            }

            
            return b;
        }

        #region calculation sorting
        class SortKey
        {
            public int Index { get; set; }
            public IComparable Key { get; set; }
        }

        List<SortKey> sortKeys = null;
        ListSortDirection calcSortDirection = ListSortDirection.Ascending;
      
        /// <summary>
        /// Gets or sets the sorting direction
        /// </summary>
        public ListSortDirection SortDirection
        {
            get { return calcSortDirection; }
        }

        /// <summary>
        /// Sorts the value by the calculated value
        /// </summary>
        /// <param name="colIndex"></param>
        public void SortByCalculation(int colIndex)
        {
            calcSortDirection = calcSortDirection == ListSortDirection.Ascending ? ListSortDirection.Descending : ListSortDirection.Ascending;
            CalcSortComparer comparer = new CalcSortComparer(calcSortDirection);

            List<SortKey> calcSortKeys = new List<SortKey>(RowCount);
            sortKeys = null;
            for (int i = 0; i < RowCount; ++i)
            {
                calcSortKeys.Add(new SortKey() { Index = i, Key = this[i, colIndex] != null ? this[i, colIndex].DoubleValue : double.MaxValue });
            }

            int row = 0;
            List<SortKey> tempKeys = new List<SortKey>();
            int edge = PivotRows.Count;
            while (row < rowCount)
            {
                int k = 0;
                int startRow = row;
                tempKeys.Clear();
                while (row < rowCount && this[row, colIndex] != null && ((this[row, colIndex].CellType == PivotCellType.ValueCell)))
                {
                    tempKeys.Add(new SortKey() { Key = calcSortKeys[row].Key, Index = calcSortKeys[row].Index });
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
                while (row < rowCount && (this[row, colIndex] == null || (this[row, colIndex].CellType != PivotCellType.ValueCell)))
                {
                    row++;
                }
                if (startRow == row) row++;
            }
            sortKeys = calcSortKeys;
        }

        class CalcSortComparer : IComparer<SortKey>
        {
            ListSortDirection dir = ListSortDirection.Ascending;
            public CalcSortComparer(ListSortDirection dir)
            {
                this.dir = dir;
            }
            public int Compare(SortKey x, SortKey y)
            {
                int c = 0;
                c = x.Key.CompareTo(y.Key);

                return dir == ListSortDirection.Descending ? -c : c;
            }
        }

        #endregion


        /// <summary>
        /// Gets the PivotCellInfo that holds the information being displayed
        /// by the pivot at the given row and column. 
        /// </summary>
        /// <param name="row">The row index (zero-based).</param>
        /// <param name="col">The column index (zero-based).</param>
        /// <returns>The PivotCellInfo associated with the cell pointed to by the row and col.</returns>
        /// <remarks>
        /// Note that this indexer will return a null value if the row and col values pick out a cell
        /// that is not visible due to being part of a covered cell.
        /// </remarks>
        public PivotCellInfo this[int row, int col]
        {
            get
            {
                if (row < 0 || row >= rowCount)
                    throw new ArgumentException("row outside of valid range.");
                if (col < 0 || col >= columnCount)
                    throw new ArgumentException("col outside of valid range.");
                if (highRowLevel > -1 && row > highRowLevel)
                {
                    for (int i = highRowLevel + 1; i <= row; ++i)
                    {
                        ProcessIJ(i);
                    }
                    highRowLevel = row;
                }
                if (sortKeys != null && col >= this.PivotRows.Count - 1)
                {
                    return pivotInfoCache[sortKeys[row].Index, col];
                }
                return pivotInfoCache[row, col];
            }
        }

        GetValueDelegate getValue = null;

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
            get
            {
                if (getValue == null)
                {
                    getValue = new GetValueDelegate(GetReflectedValue);
                }
                return getValue;
            }
            set { getValue = value; }
        }



        private List<PivotItem> pivotRows = null;
        /// <summary>
        /// Gets the PivotRows used in this pivot.
        /// </summary>
     
        public List<PivotItem> PivotRows
        {
            get
            {
                if (pivotEngine != null)
                    return pivotEngine.PivotRows;

                if (pivotRows == null)
                    pivotRows = new List<PivotItem>();
                return pivotRows;
            }
        }
    
        private List<PivotItem> pivotColumns = null;
        
        /// <summary>
        /// Gets the PivotColumns used in this pivot.
        /// </summary>       
        public List<PivotItem> PivotColumns
        {
            get
            {
                if (pivotEngine != null)
                    return pivotEngine.PivotColumns;

                if (pivotColumns == null)
                    pivotColumns = new List<PivotItem>();
                return pivotColumns;
            }
        }
        private List<PivotComputationInfo> pivotCalculations = null;
        /// <summary>
        /// Gets the PivotCalculations used in this pivot.
        /// </summary>
        /// <remarks>
        /// If the IndexEngine is created using a PivotEngine, then this PivotCalculations is
        /// the pivotEngine.PivotCalculations collection from the associated PivotEngine.
        /// </remarks>
        public List<PivotComputationInfo> PivotCalculations
        {
            get
            {
                if (pivotEngine != null)
                    return pivotEngine.PivotCalculations;

                if (pivotCalculations == null)
                    pivotCalculations = new List<PivotComputationInfo>();
                return pivotCalculations;
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
                if (pivotEngine != null)
                    return pivotEngine.Filters;
                if (filters == null)
                {
                    filters = new FilterHelper();
                }
                return filters.filterExpressions;
            }
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
                }
            }
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
                if (coveredRanges == null)
                    coveredRanges = new List<CoveredCellRange>();
                return coveredRanges;
            }
        }

        /// <summary>
        /// Gets the number of rows in the pivot.
        /// </summary>
        public int RowCount
        {
            get { return GetRowDimension()/* + 1*/; }
        }

        /// <summary>
        /// Gets the number of columns in the pivot.
        /// </summary>
        public int ColumnCount
        {
            get { return GetColumnDimension()/* + 1*/; }
        }

        int rowOffset, columnOffset;
        /// <summary>
        /// Get the number of columns that hold the row header information to the left of the numerical
        /// values in the pivot.
        /// </summary>
        public int ColumnOffSetToValues
        {
            get { return columnOffset; }
        }

        /// <summary>
        /// Gets the number of rows that hold the column header information above the numerical
        /// values in the pivot.
        /// </summary>
        public int RowOffSetToValues
        {
            get { return rowOffset; }
        }
        #endregion

        #region internals

        //this is used if the GetValue delegate is not set as part of the caller.
        Dictionary<string, PropertyInfo> lookUp = null;

        internal IComparable GetReflectedValue(object component, string property)
        {
            object o = null;
            if (lookUp == null && component != null)
            {
                PropertyInfo[] propInfos = component.GetType().GetProperties();
                lookUp = new Dictionary<string, PropertyInfo>();
                foreach (PropertyInfo pi in propInfos)
                {
                    lookUp.Add(pi.Name, pi);
                }
            }
            PropertyInfo pInfo = null;
            if (lookUp != null && lookUp.TryGetValue(property, out pInfo))
            {
                o = pInfo.GetValue(component, null);
            }
            return o as IComparable;
        }

        internal IEnumerable dataSourceList = null;
        // An enumerable object that the pivot can integrate through 
        internal IEnumerable DataSourceList
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
                        dataSourceList = ((DataTable)DataSource).DefaultView as IList;
                    }
#endif
                }
                return dataSourceList;
            }
            set { dataSourceList = value; }
        }

        //the PivotEngine being indexed.
        PivotEngine pivotEngine = null;

        //number of pivotrows 
        internal int headerColsCount = 0;

        //number of pivotcols (maybe +1 if ShowCalculationsAsColumns is true which is default)
        internal int headerRowsCount = 0;

        //This is an hierarchical collection of LisIndexInfo objects that hold the structure of the pivot.
        //This collection is ordered by RowPivots, then Column pivots. If there are 2 rowpivots and 3 column pivots,
        //this collection has 5 levels in its hierarchy with allIndexes[0].Children[0].Children[0].Children[0].Children[0]
        //picking out the set of values that go into the calculations that make up the summary at the top-left most summary cell.
        /// <summary>
        /// allIndexesRowFirst
        /// </summary>
        public List<ListIndexInfo> allIndexesRowFirst = null;
        /// <summary>
        /// allComparersRowFirst
        /// </summary>
        public List<IComparer> allComparersRowFirst = null;

        //This is an hierarchical collection of LisIndexInfo objects that hold the structure of the rowpivot headers.
        //This collection is ordered by RowPivots. If there are 2 rowpivots, then rowIndexes[].Display holds the unique values
        //that are the headers in the 1st column, and given a member in the first column, say rowIndexes[k], rowIndexes[k].Children[].Display
        //provides the unique headers for the 2nd column that go with rowIndexes[k].Display.
        internal List<ListIndexInfo> rowIndexes = null;

        //This is an hierarchical collection of LisIndexInfo objects that hold the structure of the columnpivot headers.
        //This collection is ordered by ColumnPivots. If there are 2 columnpivots, then columnIndexes[].Display holds the unique values
        //that are the headers in the 1st row, and given a member in the first row, say columnIndexes[k], columnIndexes[k].Children[].Display
        //provides the unique headers for the 2nd row that go with columnIndexes[k].Display.
        internal List<ListIndexInfo> columnIndexes = null;

        private bool CheckIndexes(int targetCol, int startCol, List<ListIndexInfo> list)
        {
            bool b = false;
            foreach (ListIndexInfo info in list)
            {
                if (b)
                    break;
                if (info.Children == null || info.Children.Count == 0)
                {
                    b = false;
                    break;
                }
                else
                {
                    int count = GetCount(info.Children);
                    if (startCol + count > targetCol)
                    {
                        b = startCol + count - 1 == targetCol;
                        if (!b)
                        {
                            b |= CheckIndexes(targetCol, startCol, info.Children);
                            break;
                        }
                    }
                    startCol += count;
                }
            }
            return b;
        }

        internal List<ListIndexInfo> IndexTheList(IList list, List<string> properties, List<string> formats, List<IComparer> comparers, bool generateCalcs)
        {
            return GetListFrom(list, 0, properties, formats, comparers, 0, list.Count, null, "");
        }

        int levelGetListFrom = -1;
        internal List<ListIndexInfo> GetListFrom(IList list, int propertyIndex, List<string> properties, List<string> formats, List<IComparer> comparers, int start, int count, ListIndexInfo parentInfo, IComparable parentDisplay)
        {
            levelGetListFrom++;
            List<ListIndexInfo> indexes = new List<ListIndexInfo>();
            if (properties != null && properties.Count > 0)
            {
                HashSet<IComparable> unique = new HashSet<IComparable>();
                int c1 = 0;
                string format = formats[propertyIndex];
                IComparable last = null;
                int end = Math.Min(start + count, list.Count);
                for (int i = start; i < end; ++i)
                {
                    PropertyInfo[] propInfo = list[i].GetType().GetProperties();
                    object o = list[i];
                    IComparable ic = GetValue(list[i], properties[propertyIndex]);
                    bool doEval = Filters.Count > 0;
                    bool pass = true;
                    if (doEval)
                    {
                        pass = true;
                        foreach (FilterExpression exp in Filters)
                        {
                            if (exp.Name != properties[propertyIndex])
                                continue;
                            if (exp.Format != null && formats[propertyIndex] != null && !formats[propertyIndex].Contains(exp.Format))
                                continue;
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
                            pass = (bool)exp.ComputedValue(o);
                            if (!pass)
                                break;
                        }
                        if (!pass)
                            continue; //fails filter - skip this object
                    }
                    if (ic != null && format != null)
                    {
                        ic = string.Format(CultureInfo.CurrentUICulture, format, ic);
                    }

                    if (last == null || last.CompareTo(ic) == 0)
                    {
                        c1++;
                    }
                    last = ic;
                    if (!unique.Contains(ic))
                    {
                        unique.Add(ic);
                        indexes.Add(new ListIndexInfo() { Display = ic, StartIndex = i, Children = new List<ListIndexInfo>(), ParentInfo = parentInfo });
                        if (indexes.Count > 1)
                        {
                            indexes[indexes.Count - 2].LastIndex = indexes[indexes.Count - 2].StartIndex + c1 - 1;
                            c1 = 1;
                        }
                    }
                }
                if (indexes.Count > 0)
                {
                    indexes[indexes.Count - 1].LastIndex = indexes[indexes.Count - 1].StartIndex + c1 - 1;
                }
                if (propertyIndex < properties.Count - 1)
                {
                    foreach (ListIndexInfo info in indexes)
                    {
                        info.Children = GetListFrom(list, propertyIndex + 1, properties, formats, comparers, info.StartIndex, info.LastIndex - info.StartIndex + 1, info, /* "=" + */ info.Display);
                    }
                }
            }
            levelGetListFrom--;

            indexes.Add(new ListIndexInfo() { Display = string.Format("{0}", parentDisplay), Type = RowType.Summary, Summaries = InitSummaries(), ParentInfo = parentInfo });
            return indexes;
        }

        private List<SummaryBase> FindCalcFromKeys(List<IComparable> keys, List<ListIndexInfo> list, bool isColSummary)
        {
            ListIndexInfo info = null;
            ListIndexInfo lastInfo = null;
            List<ListIndexInfo> search = list;
            int k = 0;
            foreach (IComparable key in keys)
            {
                if (search != null)
                {
                    //  int loc = search.BinarySearch(new ListIndexInfo() { Display = key });
                    int loc = search.BinarySearch(0, search.Count - 1, new ListIndexInfo() { Display = key }, new IndexSorter(allComparersRowFirst[k]));
                    if (loc > -1)
                    {
                        info = search[loc];
                        lastInfo = info;
                        search = info.Children;
                    }
                    else
                    {
                        info = null;
                        break;
                    }
                }
                else
                {
                    info = null;
                    break;
                }
                k++;
            }
            return info == null ? (lastInfo == null || !isColSummary || lastInfo.Type == RowType.None ? null : lastInfo.Summaries) : info.Summaries;
        }

        private void ProcessCalcs(List<object> allListCopy, List<string> allPivotsUsed, List<ListIndexInfo> indexes)
        {
            foreach (ListIndexInfo info in indexes)
            {
                if (info.Type != RowType.Summary)
                {
                    ProcessCalcsOnIndexInfo(allListCopy, allPivotsUsed, info);
                }
            }
            List<SummaryBase> summaries = InitSummaries();

            ProcessCalcsOnList(indexes, summaries); //this call will set indexes[last] properly...
        }

        private void ProcessCalcsOnList(List<ListIndexInfo> indexes, List<SummaryBase> summaries)
        {
            int last = indexes.Count - 1;
            if (last > 0)
            {
                if (summaries == null || (summaries.Count == 0 && PivotCalculations.Count > 0))
                {
                    foreach (PivotComputationInfo c in PivotCalculations)
                    {
                        summaries.Add(c.Summary.GetInstance());
                    }
                }

                for (int k = 0; k < PivotCalculations.Count; ++k)
                {
                    for (int i = 0; i < last; ++i)
                    {
                        summaries[k].Combine(indexes[i].Summaries[k].GetResult());
                    }
                }
                indexes[last].Summaries = summaries;
            }
        }

        private void ProcessCalcsOnIndexInfo(List<object> allListCopy, List<string> allPivotsUsed, ListIndexInfo info)
        {
            
            if (info.Summaries == null)
            {
                info.Summaries = InitSummaries();
            }

            if (info.Children == null || info.Children.Count == 0)
            {
                for (int i = info.StartIndex; i <= info.LastIndex; ++i)
                {
                    int j = 0;
                    foreach (SummaryBase sb in info.Summaries)
                    {
                        object other = GetValue(allListCopy[i], PivotCalculations[j].FieldName);
                        if (other != null)
                            sb.Combine(other);
#if !SILVERLIGHT
                        else if(other == null && allListCopy[i] is DataRowView)
                        {
                            sb.Combine((allListCopy[i] as DataRowView)[PivotCalculations[j].FieldName]);
                        }
#endif
                        j++;
                    }
                }
            }
            else
            {
                foreach (ListIndexInfo info1 in info.Children)
                {
                    if (info1.Type != RowType.Summary)
                    {
                        ProcessCalcsOnIndexInfo(allListCopy, allPivotsUsed, info1);
                    }
                }
                List<SummaryBase> summaries = InitSummaries();
                ProcessCalcsOnList(info.Children, summaries);
                info.Summaries = summaries;
            }
       
        }

        bool HasNoPivots()
        {
            return PivotRows.Count == 0 && PivotColumns.Count == 0 && PivotCalculations.Count > 0;
        }

        int rowCount = 0;
        int columnCount = 0;

        private void PopulateCache()
        {
            this.CoveredRanges.Clear();
            if (HasNoPivots())
            {
                this.CoveredRanges.Add(new CoveredCellRange(0, 0, 0, 0));
                pivotInfoCache[0, 0] = new PivotCellInfo() { CellType = PivotCellType.TopLeftCell };
                pivotInfoCache[0, 0].CellRange = this.CoveredRanges[0];
            }
            else if (rowOffset >= 2 && columnOffset >= 2)
            {
                int offSet = PivotRows.Count;
                pivotInfoCache[0, 0] = new PivotCellInfo() { CellType = PivotCellType.TopLeftCell, CellRange = new CoveredCellRange(0, 0, PivotColumns.Count - (PivotCalculations.Count > 1 ? 0 : 1), offSet - 1) };
                this.CoveredRanges.Add(new CoveredCellRange(0, 0, rowOffset - 2, columnOffset - 2));
            }


            //do the row headers first...
            int row = rowOffset - 1;
            int col = 0;
            levelPopulateHeaders = -1;
            PopulateHeaders(true, this.rowIndexes, headerRowsCount, ref row, ref col, null);

            //now do the column headers
            row = 0;
            col = columnOffset - 1;
            levelPopulateHeaders = -1;
            PopulateHeaders(false, this.columnIndexes, headerColsCount, ref row, ref col, null);

            //now populate all the value cells...
            PopulateCalculations();
        }

        void ApplyFormat(int i, int j)
        {
            object v = null;
            if (pivotInfoCache[i, j].Summary != null && PivotCalculations[(j - (columnOffset - 1)) % PivotCalculations.Count].Formula == null)
                v = pivotInfoCache[i, j].Summary.GetResult();
            else if (PivotCalculations[(j - (columnOffset - 1)) % PivotCalculations.Count].Formula != null)
                v = pivotInfoCache[i, j].Value;
            string format = "{0:" + PivotCalculations[(j - (columnOffset - 1)) % PivotCalculations.Count].Format + "}";
            if (format.Equals("{0:#.##}") && v != null && v.Equals(0.0d)) ////Condition added since the format string string.Format({0:#.##},0.0) returns empty.
            {
                pivotInfoCache[i, j].FormattedText = "0.0";
            }
            else if (format.Equals("{0:C}") && v != null)
            {
                pivotInfoCache[i, j].FormattedText = string.Format(CultureInfo.CurrentUICulture, format, v);
                if (Convert.ToDouble(v) < 0)
                {
                    CultureInfo culture = new CultureInfo(CultureInfo.CurrentUICulture.ToString());
                    culture.NumberFormat.CurrencyNegativePattern = 1;
                    pivotInfoCache[i, j].FormattedText = string.Format(culture, "{0:C}", v);
                }
            }
            else
            {
                pivotInfoCache[i, j].FormattedText = string.Format(CultureInfo.CurrentUICulture, format, v);
            }
            pivotInfoCache[i, j].Format = PivotCalculations[j % PivotCalculations.Count].Format;
        }

        List<IComparable> calcList = null;
        private void PopulateCalculations()
        {
            calcList = new List<IComparable>();
            ProcessCalculations();
        }

        private int GetRowIndentLevel(int row)
        {
            int edge = columnOffset - 2;
            while (edge > 0 && pivotInfoCache[row, edge] == null)
            {
                edge--;
            }
            return edge;
        }

        private int GetColIndentLevel(int col)
        {
            int edge = rowOffset - 2;
            if (PivotCalculations.Count > 1 || (PivotCalculations.Count == 1 && PivotColumns.Count == 0))
            {
                edge--;
            }
            while (edge > 0 && pivotInfoCache[edge, col] == null)
            {
                edge--;
            }
            return edge;
        }

        // This method iterates through all the cells that hold calculations and populates
        // the pivotInfoCache that hold the PivotCellInfo for each cell. The method iterates though 
        // all the value cells in pivotInfoCache, populating them. The method has three parts:
        //      I) used to populate cells in a row that is a summary.
        //     II) used to populate cells in a column this is a summary.
        //    III) used to populate value cells (not a summary cell).
        private void ProcessCalculations()
        {
            if (PivotCalculations.Count == 0)
                return;

            if (HasNoPivots())
            {
                HandleNoPivots();
                return;
            }

            bool hasCalculationHeaders = (PivotColumns.Count == 0 && PivotCalculations.Count == 1) || (PivotCalculations.Count > 1);
            int edgeRow = (rowOffset - 2) - (hasCalculationHeaders ? 1 : 0);
            int startRow = hasCalculationHeaders ? edgeRow + 2 : edgeRow + 1;
            int count = highRowLevel == 0 && initialRowLoadAmount < rowCount ? initialRowLoadAmount : rowCount;
            for (int i = startRow; i < count; ++i)
            {
                ProcessIJ(i);//noColumns, noRows, colInc, edgeColumn, edgeRow, i);
                highRowLevel = i;
            }
        }

        private void ProcessIJ(int i)//(bool noColumns, bool noRows, int colInc, int edgeColumn, int edgeRow, int i)
        {
            bool noColumns = PivotColumns.Count == 0 && PivotCalculations.Count > 0 && PivotRows.Count > 0;
            bool noRows = PivotRows.Count == 0 && PivotCalculations.Count > 0 && PivotColumns.Count > 0;

            bool hasCalculationHeaders = (PivotColumns.Count == 0 && PivotCalculations.Count == 1) || (PivotCalculations.Count > 1);
            int colInc = Math.Max(1, PivotCalculations.Count);
            int edgeColumn = columnOffset - 2;
            int edgeRow = (rowOffset - 2) - (hasCalculationHeaders ? 1 : 0);
            int startRow = hasCalculationHeaders ? edgeRow + 2 : edgeRow + 1;

            bool isSummaryRow = edgeColumn == -1 || pivotInfoCache[i, edgeColumn] == null || i == rowCount - 1;
            bool saveSummaryRow = isSummaryRow;
            calcList.Clear();
            PivotCellInfo info = edgeColumn >= 0 ? pivotInfoCache[i, edgeColumn] : null;
            
            while (info != null)
            {
                calcList.Insert(0, info.Value as IComparable);
                info = info.ParentCell;
            }
            int len = calcList.Count;
            for (int j = edgeColumn + 1; j < columnCount; ++j)
            {
                isSummaryRow = saveSummaryRow;
                int calcOffset = (j - (columnOffset - 1)) % Math.Max(PivotCalculations.Count, 1);
                PivotComputationInfo compInfo = PivotCalculations.Count == 0 ? null : this.PivotCalculations[(j - (columnOffset - 1)) % PivotCalculations.Count];
                bool isSummaryColumn = edgeRow == -1 || pivotInfoCache[edgeRow, j - calcOffset] == null || j - calcOffset == columnCount - colInc;

                //PART I
                //This code handles populating cells in row summaries. It does it by
                //iterating through the summaries (down the column) that go into computing 
                // the target summary at pivotInfoCache[i, j].
                if (isSummaryRow && !noRows)
                {
                    PivotCellInfo pci = new PivotCellInfo();
                    bool isGrandTotal = (j >= columnCount - colInc && PivotColumns.Count > 0) || (i == rowCount - 1);
                    pci.CellType = PivotCellType.ValueCell | (isGrandTotal ? PivotCellType.GrandTotalCell : PivotCellType.TotalCell);

                    List<SummaryBase> summaries = InitSummaries();
                    info = edgeColumn >= 0 ? pivotInfoCache[i, edgeColumn] : null;
                    PivotCellInfo infoOuter = edgeColumn <= 0 ? null : pivotInfoCache[i, edgeColumn - 1];
                    if (infoOuter == null || (infoOuter.CellType & PivotCellType.GrandTotalCell) != 0) // nested summary
                    {
                        int j0 = edgeColumn - 2;
                        while (infoOuter == null && j0 >= 0)
                        {
                            infoOuter = pivotInfoCache[i, j0--];
                        }
                        int i0 = i - 1;
                        isSummaryRow = edgeColumn < 0 || pivotInfoCache[i0, edgeColumn] == null;
                        int edge = GetRowIndentLevel(i0);
                        bool done = false;

                        while (!done && i0 > edgeRow && (j0 < 0 || pivotInfoCache[i0, j0] == null))
                        {
                            PivotCellInfo info1 = pivotInfoCache[i0, j];
                            if (info1 != null)
                            {
                                foreach (SummaryBase sb in summaries)
                                {
                                    if (info1 != null && info1.Summary != null && info1.Summary.ToString() == sb.ToString())
                                    {
                                        if (info1.Summary != null)
                                        {
                                            sb.CombineSummary(info1.Summary);
                                        }
                                        else
                                        {
                                            sb.Combine(info1.DoubleValue);
                                        }
                                    }
                                }
                                if (isSummaryRow)
                                {
                                    {
                                        PivotCellInfo edgeInfo = pivotInfoCache[i0, edge];
                                        if (edgeInfo != null && edgeInfo.ParentCell != null && edgeInfo.ParentCell.CellRange != null)
                                        {
                                            i0 = edgeInfo.ParentCell.CellRange.Top - 1;
                                        }
                                        else
                                        {
                                            i0--;
                                        }
                                        done = i0 <= edgeRow || edge != GetRowIndentLevel(i0);
                                    }
                                }
                                else
                                {
                                    i0--;
                                }
                            }
                            else
                                i0--;
                        }
                        if (summaries.Count > 0)
                        {
                            pci.Summary = summaries[(j - (columnOffset - 1)) % PivotCalculations.Count];
                            object o = summaries[(j - (columnOffset - 1)) % PivotCalculations.Count].GetResult();
                            if (PivotCalculations[(j - (columnOffset - 1)) % PivotCalculations.Count].CalculationType == CalculationType.Formula)
                                pci.Value = CalculateFormula(i, j, (j - (columnOffset - 1)), compInfo);
                            else
                                pci.Value = //"sCC " + //used for debug
                                    (o == null ? "" : o.ToString());
                            pivotInfoCache[i, j] = pci;
                            ApplyFormat(i, j);
                        }
                    }
                    else
                    {
                        int k0 = infoOuter.ParentCell.CellRange.Top;
                        int k1 = infoOuter.ParentCell.CellRange.Bottom;
                        foreach (SummaryBase sb in summaries)
                        {
                            for (int k = k0; k <= k1; ++k)
                            {
                                PivotCellInfo info1 = pivotInfoCache[k, j];
                                if (info1 != null && info1.Summary != null && info1.Summary.ToString() == sb.ToString())
                                {
                                    if (info1.Summary != null)
                                    {
                                        sb.CombineSummary(info1.Summary);
                                    }
                                    else
                                    {
                                        sb.Combine(info1.DoubleValue);
                                    }
                                }
                            }
                        }
                        if (summaries.Count > 0)
                        {
                            pci.Summary = summaries[(j - (columnOffset - 1)) % PivotCalculations.Count];
                            object o = summaries[(j - (columnOffset - 1)) % PivotCalculations.Count].GetResult();
                            if (PivotCalculations[(j - (columnOffset - 1)) % PivotCalculations.Count].CalculationType == CalculationType.Formula)
                                pci.Value = CalculateFormula(i, j, (j - (columnOffset - 1)), compInfo);
                            else
                                pci.Value = //"sCC " + //used for debug
                                    (o == null ? "" : o.ToString());
                            pivotInfoCache[i, j] = pci;
                            ApplyFormat(i, j);
                        }
                    }
                    continue; //summary row
                }

                // PART II
                // This code handles populating cells in column summaries. It does it by
                // iterating through the summaries (across row) that go into computing 
                // the target summary at pivotInfoCache[i, j].
                if (isSummaryColumn && !noColumns)
                {
                    PivotCellInfo pci = new PivotCellInfo();
                    bool isGrandTotal = (j >= columnCount - colInc) || (i == rowCount - 1);
                    pci.CellType = PivotCellType.ValueCell | (isGrandTotal ? PivotCellType.GrandTotalCell : PivotCellType.TotalCell);

                    SummaryBase summary = PivotCalculations[calcOffset].Summary.GetInstance();

                    info = edgeRow < 0 ? null : pivotInfoCache[edgeRow, j];
                    PivotCellInfo infoOuter = edgeRow < 1 ? null : pivotInfoCache[edgeRow - 1, j - calcOffset];
                    if (infoOuter == null || (infoOuter.CellType & PivotCellType.GrandTotalCell) != 0) // nested summary
                    {
                        int i0 = edgeRow - 2;
                        while (infoOuter == null && i0 >= 0)
                        {
                            infoOuter = pivotInfoCache[i0--, j - calcOffset];
                        }
                        int j0 = j - colInc - calcOffset;
                        isSummaryColumn = edgeRow < 0 || j0 < 0 ? false : pivotInfoCache[edgeRow, j0] == null;
                        int edge = GetColIndentLevel(j0);
                        bool done = false;
                        // i0 = Math.Max(0, i0);
                        while (!done && j0 > edgeColumn && (i0 < 0 || pivotInfoCache[i0, j0] == null))
                        {
                            PivotCellInfo info1 = pivotInfoCache[i, j0 + calcOffset];
                            if (info1 != null)
                            {
                                if (summary == null)
                                    summary = info1.Summary.GetInstance();
                                if (info1 != null)
                                {
                                    if (info1.Summary != null)
                                    {
                                        summary.CombineSummary(info1.Summary);
                                    }
                                    else
                                    {
                                        summary.Combine(info1.DoubleValue);
                                    }
                                }
                                if (isSummaryColumn)
                                {
                                    {
                                        PivotCellInfo edgeInfo = pivotInfoCache[edge, j0];
                                        if (edgeInfo != null && edgeInfo.ParentCell != null && edgeInfo.ParentCell.CellRange != null)
                                        {
                                            j0 = edgeInfo.ParentCell.CellRange.Left - colInc;
                                        }
                                        else
                                            j0 -= colInc;
                                        done = j0 <= edgeColumn || edge != GetColIndentLevel(j0);
                                    }
                                }
                                else
                                {
                                    j0 -= colInc;
                                }
                            }
                            else
                                j0 -= colInc;
                        }
                        pci.Summary = summary;
                        object o = summary.GetResult();
                        if (PivotCalculations[(j - (columnOffset - 1)) % PivotCalculations.Count].CalculationType == CalculationType.Formula)
                            pci.Value = CalculateFormula(i, j, (j - (columnOffset - 1)), compInfo);
                        else
                            pci.Value = //"sCC " + //used for debug
                                    (o == null ? "" : o.ToString());
                        pivotInfoCache[i, j] = pci;
                        ApplyFormat(i, j);
                    }
                    else if (infoOuter.ParentCell != null)
                    {
                        int k0 = infoOuter.ParentCell.CellRange.Left + calcOffset;
                        int k1 = infoOuter.ParentCell.CellRange.Right + calcOffset;
                        int kkk = k0;
                        for (int k = k0; k <= k1; ++k)
                        {
                            PivotCellInfo info1 = pivotInfoCache[i, kkk];
                            if (info1 != null)
                            {
                                if (summary == null)
                                    summary = info1.Summary.GetInstance();
                                if (info1.Summary != null)
                                {
                                    summary.CombineSummary(info1.Summary);
                                }
                                else
                                {
                                    summary.Combine(info1.DoubleValue);
                                }
                            }
                            kkk += colInc;
                            if (kkk >= columnCount)
                                break;
                        }
                        pci.Summary = summary;
                        object o = summary.GetResult();
                        if (PivotCalculations[(j - (columnOffset - 1)) % PivotCalculations.Count].CalculationType == CalculationType.Formula)
                            pci.Value = CalculateFormula(i, j, (j - (columnOffset - 1)), compInfo);
                        else
                            pci.Value = //"sCC " + //used for debug
                                    (o == null ? "" : o.ToString());
                        pivotInfoCache[i, j] = pci;
                        ApplyFormat(i, j);
                    }
                    continue; //summary column
                }

                //Part III
                // The code from here to the bottom of the method is for only value cells with 
                // the calculations taken from the ones cached in allIndexesRowFirst.
                info = edgeRow < 0 ? null : pivotInfoCache[edgeRow, j - calcOffset];
                bool infoStartsOutNull = info == null;
                if (noRows)
                {
                    calcList.Clear();
                    len = 0;
                }
                while (info != null && !noColumns)
                {
                    calcList.Insert(len, info.Value as IComparable);
                    info = info.ParentCell;
                }
                object val = null;
                var v1 = FindCalcFromKeys(calcList, allIndexesRowFirst, false);
                while (calcList.Count > len)
                    calcList.RemoveAt(calcList.Count - 1);
                if (v1 != null)
                {
                    //  foreach (SummaryBase sb in v1)
                    SummaryBase sb = v1[calcOffset];
                    {
                        val = sb.GetResult();
                        if (val != null)
                        {
                            PivotCellInfo pci = new PivotCellInfo();
                            pci.CellType = PivotCellType.ValueCell;
                            if (i == rowCount - 1 || (j >= columnCount - colInc && PivotColumns.Count > 0))
                            {
                                pci.CellType |= PivotCellType.GrandTotalCell;
                            }
                            if (PivotCalculations[(j - (columnOffset - 1)) % PivotCalculations.Count].CalculationType == CalculationType.Formula)
                            {
                                pci.Value = CalculateFormula(i, j, j - (columnOffset - 1), compInfo);
                            }
                            else
                                pci.Value = val;
                            pci.Summary = sb;
                            pivotInfoCache[i, j] = pci;
                            ApplyFormat(i, j);
                        }
                        j++;
                    }
                    j--;
                }
            } //end
        }

        private object CalculateFormula(int row1, int col1, int col, PivotComputationInfo compInfo)
        {
            Dictionary<string, double> component = new Dictionary<string, double>();
            int c = col1 - (col % this.PivotCalculations.Count);
            for (int i = 0; i < this.PivotCalculations.Count; ++i)
            {
                if (this.PivotCalculations[i].CalculationName == null)
                {
                    this.PivotCalculations[i].CalculationName = string.Format("Computation{0}", i);
                }
                if (this.PivotCalculations[i].Formula == null)
                {
                    component.Add(this.PivotCalculations[i].CalculationName, this.pivotInfoCache[row1, c] == null ? 0d : this.pivotInfoCache[row1, c].DoubleValue);
                }
                else
                    PivotCalculations[i].AllowRunTimeGroupByField = false;
                c++;
            }
            if (compInfo.Expression == null)
            {
                compInfo.Expression = new FilterExpression(compInfo.CalculationName, compInfo.Formula);
            }
            
            return compInfo.Expression.ComputedValue(component);
        }

       
        private void HandleNoPivots()
        {   //just iterate through all the objects to get the grand totals
            List<SummaryBase> summaries = InitSummaries();
            foreach (object o in this.DataSourceList)
            {
                for (int i = 0; i < PivotCalculations.Count; ++i)
                {
                    summaries[i].Combine(GetValue(o, PivotCalculations[i].FieldName));
                }
            }

            PivotCellInfo pci = new PivotCellInfo();
            pci.CellType = PivotCellType.GrandTotalCell | PivotCellType.HeaderCell | PivotCellType.RowHeaderCell;
            pci.Value = "Grand";
            pci.FormattedText = "Grand";
            pivotInfoCache[1, 0] = pci;
            int j = 1;
            for (int i = 0; i < PivotCalculations.Count; ++i)
            {
                pci = new PivotCellInfo();
                pci.CellType = PivotCellType.GrandTotalCell | PivotCellType.ValueCell;
                pci.Value = summaries[i].GetResult();
                pci.Summary = summaries[i];
                pivotInfoCache[1, j] = pci;
                ApplyFormat(1, j++);
            }
        }

        //initializes a summary collection for a node.
        private List<SummaryBase> InitSummaries()
        {
            List<SummaryBase> v1 = new List<SummaryBase>();
            foreach (PivotComputationInfo c in PivotCalculations)
            {
                v1.Add(c.Summary.GetInstance());
            }
            return v1;
        }

        int levelPopulateHeaders = -1;
        // This recursively called method populates either the row headers or the column headers from list depending upon isRowHeader.
        private void PopulateHeaders(bool isRowHeaders, List<ListIndexInfo> list, int depth, ref int row, ref int col, PivotCellInfo parent)
        {
            bool hasCalculationHeaders = (PivotColumns.Count == 0 && PivotCalculations.Count > 0) || (PivotCalculations.Count > 1);
            levelPopulateHeaders++;
            if (isRowHeaders)
            {
                col = levelPopulateHeaders;
                if (col >= columnOffset - 1)
                    return;
            }
            else
            {
                if (hasCalculationHeaders && (row == rowOffset - 1 || PivotColumns.Count == 0))
                {
                    foreach (PivotComputationInfo c in PivotCalculations)
                    {
                        PivotCellInfo cellInfo = new PivotCellInfo()
                        {
                            CellType = PivotCellType.ColumnHeaderCell | PivotCellType.CalculationHeaderCell,
                            Value = c.FieldHeader == "" || c.FieldHeader == null ? c.FieldName : c.FieldHeader
                        };
                        cellInfo.FormattedText = cellInfo.Value.ToString();
                        pivotInfoCache[row, col++] = cellInfo;
                    }
                    return;
                }
                row = levelPopulateHeaders;
                if (row >= rowOffset - 1)
                    return;
            }

            int index1 = 0;
            int inc = isRowHeaders ? 1 : Math.Max(1, PivotCalculations.Count);
            foreach (ListIndexInfo info in list)
            {
                if (levelPopulateHeaders < depth - 1 && info.Children != null && info.Children.Count > 0)
                {
                    PivotCellInfo parent1 = SetCellInfo(isRowHeaders, GetCount(info), ref row, ref col, depth, index1, info.Display, index1 == list.Count - 1, true, parent);
                    PopulateHeaders(isRowHeaders, info.Children, isRowHeaders ? headerRowsCount : headerColsCount, ref row, ref col, parent1);
                }
                else if (levelPopulateHeaders < depth)
                {
                    SetCellInfo(isRowHeaders, list.Count, ref row, ref col, depth, index1, info.Display, index1 == list.Count - 1, false, parent);

                    if (!isRowHeaders && hasCalculationHeaders && row <= PivotColumns.Count - 1)
                    {
                        int saveRow = row;
                        row = PivotColumns.Count;
                        if (col < columnCount - inc && PivotCalculations.Count > 1 && row > 0 && pivotInfoCache[row - 1, col] != null)
                        {
                            pivotInfoCache[row - 1, col].CellRange = new CoveredCellRange(row - 1, col, row - 1, col + PivotCalculations.Count - 1);
                            this.CoveredRanges.Add(pivotInfoCache[row - 1, col].CellRange);
                        }
                        int r = row - 1;
                        while (r >= 0 && pivotInfoCache[r, col] == null)
                            r--;
                        bool isSummaryColumn = r >= 0 && (pivotInfoCache[r, col].CellType & PivotCellType.TotalCell) != 0;
                        PivotCellType pct = isSummaryColumn ? PivotCellType.TotalCell | PivotCellType.ColumnHeaderCell | PivotCellType.CalculationHeaderCell : PivotCellType.ColumnHeaderCell | PivotCellType.CalculationHeaderCell;
                        foreach (PivotComputationInfo c in PivotCalculations)
                        {
                            PivotCellInfo cellInfo = new PivotCellInfo()
                            {
                                CellType = pct,
                                Value = c.FieldHeader == "" || c.FieldHeader == null ? c.FieldName : c.FieldHeader
                            };
                            cellInfo.FormattedText = cellInfo.Value.ToString();
                            if (col >= columnCount - inc)
                            {
                                cellInfo.CellType |= PivotCellType.GrandTotalCell;
                            }
                            pivotInfoCache[row, col++] = cellInfo;
                        }
                        row = saveRow;
                    }
                    else
                    {
                        if (isRowHeaders) row += inc; else col += inc;
                    }
                }
                index1++;
            }
            levelPopulateHeaders--;
        }

        PivotCellInfo lastExpander = null;
        // This method is called from PopulateHeaders to create the PivotCellInfo and set into the pivotInfoCache for 
        // the cell at row, col after setting things like CellType, CellRange and ParentCell.
        private PivotCellInfo SetCellInfo(bool isRowHeaders, int count, ref int row, ref int col, int depth, int index, IComparable display, bool isLast, bool isExpander, PivotCellInfo parent)
        {
            int inc = isRowHeaders ? 1 : Math.Max(1, PivotCalculations.Count);
            PivotCellInfo cellInfo = new PivotCellInfo();
            cellInfo.Value = display;
            cellInfo.FormattedText = display != null ? display.ToString() : ""; //may need to tweak this to work with formatted text (ie DateTime to Month)
            if (isRowHeaders)
            {
                cellInfo.CellType = (col == PivotRows.Count - 1 || PivotRows.Count == 0) ? PivotCellType.HeaderCell | PivotCellType.RowHeaderCell : PivotCellType.RowHeaderCell;
            }
            else
            {
                cellInfo.CellType = (row == PivotColumns.Count - 1) ? PivotCellType.HeaderCell | PivotCellType.ColumnHeaderCell : PivotCellType.ColumnHeaderCell;
            }
            if (isExpander) //first one is expander if more than 1
            {
                cellInfo.CellType |= PivotCellType.ExpanderCell;
                cellInfo.CellType &= ~PivotCellType.HeaderCell; //take off the header to match PivotEngine
                CoveredCellRange range = null;
                if (isRowHeaders)
                {
                    range = new CoveredCellRange() { Top = row, Left = col, Bottom = row + (isRowHeaders ? count - 2 : 1), Right = col };
                }
                else
                {
                    int colwidth = !isRowHeaders ? (count - 2) * inc + (PivotCalculations.Count > 1 ? PivotCalculations.Count - 1 : 0) : 1;
                    range = new CoveredCellRange() { Top = row, Left = col, Bottom = row, Right = col + colwidth };
                }
                cellInfo.CellRange = range;
                this.CoveredRanges.Add(range);
                lastExpander = cellInfo;
            }
            if (isLast) //last one
            {
                if (lastExpander != null)
                {
                    parent = lastExpander;
                    lastExpander = null;
                }
                cellInfo.CellType |= levelPopulateHeaders == 0 ? PivotCellType.GrandTotalCell : PivotCellType.TotalCell;
                if ((cellInfo.CellType & PivotCellType.GrandTotalCell) == 0)
                {
                    cellInfo.CellType &= ~PivotCellType.HeaderCell; //take off the header to match PivotEngine
                }
                if (isRowHeaders)
                {
                    if (col < columnOffset - 1 && (col > 0 || col < columnOffset - 2))
                    {
                        CoveredCellRange range = new CoveredCellRange() { Top = row, Left = col > 0 ? col - 1 : 0, Bottom = row, Right = columnOffset - 2 };
                        this.CoveredRanges.Add(range);
                        cellInfo.CellRange = range;
                    }
                }
                else
                {
                    CoveredCellRange range = new CoveredCellRange() { Top = row > 0 ? row - 1 : 0, Left = col, Bottom = rowOffset - 2 - (inc > 1 ? 1 : 0), Right = col + inc - 1 };
                    if (range.Top != range.Bottom || range.Left != range.Right)
                    {
                        this.CoveredRanges.Add(range);
                        cellInfo.CellRange = range;
                    }
                }
                if (levelPopulateHeaders == 0)
                {
                    string totalLabel = (isRowHeaders && PivotRows.Count > 0) ? PivotRows[0].TotalHeader :
                                         ((!isRowHeaders && PivotColumns.Count > 0) ? PivotColumns[0].TotalHeader : string.Empty);
                    string s = "Grand";
                    cellInfo.Value = string.Format("{1}{0}", s, totalLabel);
                    if (totalLabel != null && totalLabel.Length > 0)
                    {
                        cellInfo.FormattedText = string.Format("{1} {0}", totalLabel, s);
                    }
                    else
                    {
                        cellInfo.FormattedText = string.Format("{0}", s);
                    }
                }
                else
                {
                    string totalLabel = (isRowHeaders && PivotRows.Count > levelPopulateHeaders - 1) ? PivotRows[levelPopulateHeaders - 1].TotalHeader :
                                     ((!isRowHeaders && PivotColumns.Count > levelPopulateHeaders - 1) ? PivotColumns[levelPopulateHeaders - 1].TotalHeader : string.Empty);
                    string s = string.Format("{0}", cellInfo.Value);
                    cellInfo.Value = string.Format("{1}{0}", totalLabel, cellInfo.Value);
                    if (totalLabel != null && totalLabel.Length > 0)
                    {
                        cellInfo.FormattedText = string.Format("{1} {0}", totalLabel, s);
                    }
                    else
                    {
                        cellInfo.FormattedText = string.Format("{0}", s);
                    }
                }

                if (isRowHeaders)
                {
                    if (col > 0)
                        col--;
                }
                else
                {
                    if (row > 0)
                        row--;
                }
            }

            //no ranges unless calculations > 1
            if (inc > 1 && !isLast && row == PivotColumns.Count)
            {
                CoveredCellRange range = new CoveredCellRange() { Top = row > 0 ? row - 1 : 0, Left = col, Bottom = row > 0 ? row - 1 : 0, Right = col + inc - 1 };
                if (range.Top != range.Bottom || range.Left != range.Right)
                {
                    this.CoveredRanges.Add(range);
                    cellInfo.CellRange = range;
                }
            }

            if (col < columnCount && row < rowCount)
            {
                cellInfo.ParentCell = parent;
                pivotInfoCache[row, col] = cellInfo;
            }

            return cellInfo;
        }

        // internal cache that holds all the PivotCellInfo for each cell in the pivot.
        PivotCellInfos pivotInfoCache = null;

        /// <summary>
        /// Gets the total number of unique slices under this given ListIndexInfo object
        /// </summary>
        /// <param name="info">The parent ListIndexInfo object.</param>
        /// <returns>The number of row or columns that are contained by this ListIndexInfo item.</returns>
        /// <remarks>
        /// Given an outer row node, the GetCount value for this node is the number of child nodes that are contain
        /// under this outer node. This method uses recursions to iterate through all children contained
        /// at any level under this outer node. Summing up all the GetCounts for all the outer nodes will give
        /// you the total number of all nodes.
        /// </remarks>
        internal int GetCount(ListIndexInfo info)
        {
            if (info.Children == null || info.Children.Count == 0)
                return 1;

            int count = 0;
            foreach (ListIndexInfo info1 in info.Children)
            {
                count += GetCount(info1);
            }
            return count;
        }

        internal int GetCount(List<ListIndexInfo> list)
        {
            int count = 0;
            if (list != null)
            {
                foreach (ListIndexInfo info in list)
                    count += GetCount(info);
            }
            return count;
        }

        internal int GetRowDimension()
        {
            if (PivotCalculations.Count > 0 && PivotColumns.Count == 0 && PivotRows.Count == 0)
                return 2; //special cases...
            else if (PivotCalculations.Count == 0 && PivotColumns.Count > 0 && PivotRows.Count == 0)
                return PivotColumns.Count; //special cases...

            int count = GetCount(rowIndexes) + rowOffset;
            return count - 1;
        }

        private int SummaryCount(ListIndexInfo info, ref int currentLevel, ref int levelCutOff)
        { //asume 1 calc && then adjust for multiple ones later
            int count = 1;
            currentLevel++;
            if (currentLevel < levelCutOff)
            {
                if (info.Children != null)
                {
                    foreach (ListIndexInfo info1 in info.Children)
                    {
                        count += SummaryCount(info1, ref currentLevel, ref levelCutOff);
                    }
                }
            }
            currentLevel--;
            return count;
        }

        internal int GetColumnDimension()
        {
            if (PivotRows.Count > 0 && PivotCalculations.Count == 0 && PivotColumns.Count == 0)
                return PivotRows.Count; //special cases
            int colsWithSummaries = GetCount(columnIndexes);
            int count = columnOffset;
            count += colsWithSummaries * Math.Max(1, PivotCalculations.Count);
            return count - 1;
        }

        #endregion

        class IndexSorter : IComparer<ListIndexInfo>
        {

            IComparer fieldComparer = null;
            public IndexSorter(IComparer fieldComparer)
            {
                this.fieldComparer = fieldComparer;
            }
            public int Compare(ListIndexInfo x, ListIndexInfo y)
            {
                if (fieldComparer != null)
                    return fieldComparer.Compare(x.Display, y.Display);
                else
                {
                    if (x != null)
                        return x.CompareTo(y);
                    else if (y != null)
                        return -y.CompareTo(x);
                    return 0; //both null
                }
            }
        }

    }
      #region support classes

    /// <summary>
    /// Use to get the property value without using reflection
    /// </summary>
    /// <param name="o">object</param>
    /// <param name="name">string</param>
    /// <returns>IComparable</returns>
    public delegate IComparable GetValueDelegate(object o, string name);

    /// <summary>
    /// Class used compare and sort the given collection
    /// </summary>
    public class SortComparer : IComparer<object>
    {
        List<string> propertyNames = null;
        GetValueDelegate GetValue = null;
        List<string> formats = null;
        List<IComparer> comparers = null;
        /// <summary>
        /// Performs the comaparing operation in Sorting
        /// </summary>
        /// <param name="propertyNames">List</param>
        /// <param name="formats">List</param>
        /// <param name="comparers">List</param>
        /// <param name="GetValue">GetValueDelegate</param>
        public SortComparer(List<string> propertyNames, List<string> formats, List<IComparer> comparers, GetValueDelegate GetValue)
        {
            this.propertyNames = propertyNames;
            this.GetValue = GetValue;
            this.formats = formats;
            this.comparers = comparers;
        }
        /// <summary>
        /// Compares the different objects.
        /// </summary>
        /// <param name="x">object</param>
        /// <param name="y">object</param>
        /// <returns>int</returns>
        public int Compare(object x, object y)
        {
            int c = 0;
            IComparable xc = null;
            IComparable yc = null;
            for (int i = 0; i < propertyNames.Count; ++i)
            {
                xc = GetValue(x, propertyNames[i]) as IComparable;
                yc = GetValue(y, propertyNames[i]) as IComparable;

                if (xc == null && yc != null)
                    c = -1;
                else
                {
                    if (formats != null && formats[i] != null && formats[i].Length > 0)
                    {
                        if (xc != null)
                        {
                            xc = string.Format(CultureInfo.CurrentUICulture, formats[i], xc);
                        }
                        if (yc != null)
                        {
                            yc = string.Format(CultureInfo.CurrentUICulture, formats[i], yc);
                        }
                    }
                    if (comparers != null && comparers[i] != null)
                    {
                        c = comparers[i].Compare(xc, yc);
                    }
                    else if (xc == null)
                    {
                        c = 0;
                    }
                    else
                    {
                        c = xc.CompareTo(yc);
                    }
                }
                if (c != 0)
                    break;
            }
            return c;
        }
    }

    /// <summary>
    /// Class that holds primarily the information on one row in a pivot table. Additionally, this
    /// calls can be used to hold information on the row/column header structures.
    /// </summary>
    public class ListIndexInfo : IComparable<ListIndexInfo>
    {
        /// <summary>
        /// Gets the Type of the variable
        /// </summary>
        public ListIndexInfo()
        {
            Type = RowType.None;
        }
        /// <summary>
        /// Holds the starting Index value.
        /// </summary>
        public int StartIndex { get; set; }
        int lastIndex = -1;
        /// <summary>
        /// Holds the Ending Index value.
        /// </summary>
        public int LastIndex
        {
            get { return lastIndex; }
            set
            {
                lastIndex = value;
            }
        }
        /// <summary>
        /// Gets and sets teh display property
        /// </summary>
        public IComparable Display { get; set; }
        /// <summary>
        /// Gets and sets the list of children
        /// </summary>
        public List<ListIndexInfo> Children { get; set; }
        /// <summary>
        ///  Gets and sets the list of Summaries
        /// </summary>
        public List<SummaryBase> Summaries { get; set; }
        /// <summary>
        /// Converts the data to string type.
        /// </summary>
        /// <returns>string</returns>
        public override string ToString()
        {
            return string.Format("{0} st={2} last={3}", Display, Children.Count, StartIndex, LastIndex);
        }
        /// <summary>
        /// Gets ad sets the RowType
        /// </summary>
        public RowType Type { get; set; }
        /// <summary>
        /// Compares the different values
        /// </summary>
        /// <param name="other">ListIndexInfo</param>
        /// <returns>int</returns>
        public int CompareTo(ListIndexInfo other)
        {
            if (Display == null && other != null)
            {
                if (other.Display == null)
                    return 0;
                else
                    return -1;
            }

            return Display.CompareTo(other != null ? other.Display : null);
        }
        /// <summary>
        /// Get the information abou the parent
        /// </summary>
        public ListIndexInfo ParentInfo { get; set; }
    }

    /// <summary>
    /// Enum provides option to select row type
    /// </summary>
    public enum RowType
    {
        /// <summary>
        /// When no type is set for Row
        /// </summary>
        None,
        /// <summary>
        /// When type is set for Row as Summary
        /// </summary>
        Summary
    }
    #endregion
}
