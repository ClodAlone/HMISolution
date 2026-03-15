#region Copyright Syncfusion Inc. 2001 - 2014
//
//  Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
//
//  Use of this code is subject to the terms of our license.
//  A copy of the current license can be obtained at any time by e-mailing
//  licensing@syncfusion.com. Re-distribution in any form is strictly
//  prohibited. Any infringement will be prosecuted under applicable laws. 
//
#endregion

using System;
using System.Collections;
using System.ComponentModel;
using System.Runtime.Serialization;
using System.Text;
using System.Security.Permissions;

namespace Syncfusion.Olap.Engine
{
    /// <summary>
    /// A collection of <see cref="GridRangeInfo"/> objects.
    /// </summary>
    [
    TypeConverter(typeof(GridRangeInfoListConverter)),
    Serializable
    ]
    public class GridRangeInfoList : CollectionBase, ICloneable, IFormattable, ISerializable
    {
        private const char separator = ';';
        
        /// <summary>
        /// An empty and Read-only list.
        /// </summary>
        public static readonly GridRangeInfoList Empty = new GridRangeInfoList();

        /// <overload>
        /// Initializes a new <see cref="GridRangeInfoList"/>.
        /// </overload>
        /// <summary>
        /// Initializes a new empty <see cref="GridRangeInfoList"/>.
        /// </summary>
        public GridRangeInfoList()
        {
        }

        /// <summary>
        /// Initializes a new <see cref="GridRangeInfoList"/> from a serialization stream.
        /// </summary>
        /// <param name="info">An object that holds all the data needed to serialize or deserialize this instance.</param>
        /// <param name="context">Describes the source and destination of the serialized stream specified by info. </param>
        protected GridRangeInfoList(SerializationInfo info, StreamingContext context)
        {
            SerializationInfoEnumerator sie = info.GetEnumerator();
            while (sie.MoveNext())
            {
                List.Add(sie.Value);
            }
        }

        /// <summary>
        /// Implements the ISerializable interface and returns the data needed to serialize the <see cref="GridRangeInfoList"/>.
        /// </summary>
        /// <param name="info">A SerializationInfo object containing the information required to serialize the object.</param>
        /// <param name="context">A StreamingContext object containing the source and destination of the serialized stream.</param>
        [SecurityPermissionAttribute(SecurityAction.Demand, SerializationFormatter=true)]
        [SecurityPermissionAttribute(SecurityAction.LinkDemand, SerializationFormatter=true)]
        public virtual void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            for (int n = 0; n < this.Count; n++)
                info.AddValue(n.ToString(), List[n], typeof(GridRangeInfo));
        }

        /// <summary>
        /// The <see cref="GridRangeInfo"/> at a specific index.
        /// </summary>
        public GridRangeInfo this[int index] 
        {
            get 
            {
                return (GridRangeInfo)(List[index]);
            }
            set 
            {
                List[index] = value;
            }
        }

        /// <summary>
        /// Adds <see cref="GridRangeInfo"/> elements from an array to the range list.
        /// </summary>
        /// <param name="value">An array of <see cref="GridRangeInfo"/> objects.</param>
        public void AddRange(GridRangeInfo[] value)
        {
            foreach (object o in value)
            {
                if (o is GridRangeInfo)
                    Add((GridRangeInfo) o);
            }
        }   

        /// <summary>
        /// The last range in the range list.
        /// </summary>
        public GridRangeInfo ActiveRange
        {
            get
            {
                if (Count > 0)
                    return this[Count-1];
                return GridRangeInfo.Empty;
            }
        }

        /// <summary>
        /// <para>Adds <see cref="GridRangeInfo"/> to the end of the <see cref="GridRangeInfoList"/>.</para>
        /// </summary>
        /// <param name="value">The <see cref="GridRangeInfo"/> to be added to the list.</param>
        /// <returns>The index of the new element.</returns>
        public int Add(GridRangeInfo value) 
        {
            return List.Add(value);
        }

        /// <summary>
        /// Inserts a <see cref="GridRangeInfo"/> at a specified position.
        /// </summary>
        /// <param name="index">The index where the value should be inserted.</param>
        /// <param name="value">The value to be inserted.</param>
        public void Insert(int index, GridRangeInfo value) 
        {
            List.Insert(index, value);
        }


        /// <summary>
        /// Returns the position of a specific range in the list that equals a specifed range.
        /// </summary>
        /// <param name="value">The range to be searched for.</param>
        /// <returns>The position of the range; - 1 if not found.</returns>
        public int IndexOf(GridRangeInfo value) 
        {
            return List.IndexOf(value);
        }

        /// <summary>
        /// Checks if the range list contains a range object that equals the specified range.
        /// </summary>
        /// <param name="value">The range to be searched for.</param>
        /// <returns>True if range was found; False if not found.</returns>
        public bool Contains(GridRangeInfo value) 
        {
            return List.Contains(value);
        }

        /// <summary>
        /// Removes a specific range from the range list.
        /// </summary>
        /// <param name="value">The range to be removed.</param>
        public void Remove(GridRangeInfo value) 
        {
            List.Remove(value);
        }

        /// <overload>
        /// Copies all range objects into an array of <see cref="GridRangeInfo"/> starting at specified index.
        /// </overload>
        /// <summary>
        /// Copies all range objects into an array of <see cref="GridRangeInfo"/> starting at specified index.
        /// </summary>
        /// <param name="array">The array of <see cref="GridRangeInfo"/> where the values should be copied to.</param>
        /// <param name="index">The starting index in the destination array.</param>
        public void CopyTo(GridRangeInfo[] array, int index) 
        {
            List.CopyTo(array, index);
        }

        /// <summary>
        /// Copies all range objects into an array of <see cref="GridRangeInfo"/> starting at specified index.
        /// </summary>
        /// <param name="array">The array of <see cref="GridRangeInfo"/> where the values should be copied to.</param>
        /// <param name="index">The starting index in the destination array.</param>
        public void CopyTo(Array array, int index) 
        {
            List.CopyTo(array, index);
        }

        /// <summary>
        /// Copies all range objects into a <see cref="GridRangeInfoList"/> starting at specified index.
        /// </summary>
        /// <param name="rl">The <see cref="GridRangeInfoList"/> where the values should be copied to.</param>
        /// <param name="index">The starting index in the destination list.</param>
        public void CopyTo(GridRangeInfoList rl, int index) 
        {
            for (int i = index; i < Count; i++)
                if (!this[i].IsEmpty)
                    rl.Add(this[i]);
        }

        object ICloneable.Clone() 
        {
            return this.Clone();
        }   

        /// <summary>
        /// Makes an exact copy of the current object.
        /// </summary>
        /// <returns>A <see cref="GridRangeInfoList"/> with duplicated <see cref="GridRangeInfo"/> objects.</returns>
        public GridRangeInfoList Clone() 
        {
            GridRangeInfoList rl = new GridRangeInfoList();
            CopyTo(rl, 0);
            return rl;
        }   

        /// <summary>
        /// Removes any empty ranges from the <see cref="GridRangeInfoList"/>.
        /// </summary>
        public void RemoveEmptyRanges()
        {
            for (int n = this.Count-1; n >= 0; n--)
            {
                if (this[n].IsEmpty)
                    RemoveAt(n);
            }
        }

        /// <summary>
        /// Searches for ranges in the <see cref="GridRangeInfoList"/> that contain a specified range.
        /// </summary>
        /// <param name="range">The range to be searched for.</param>
        /// <returns>The <see cref="GridRangeInfoList"/> with ranges that match the criteria.</returns>
        public GridRangeInfoList GetRangesContaining(GridRangeInfo range)
        {
            GridRangeInfoList rl = new GridRangeInfoList();
            foreach (GridRangeInfo r in this)
            {
                if (r.Contains(range))
                    rl.Add(r);
            }

            return rl;
        }

        /// <summary>
        /// Searches for ranges in the <see cref="GridRangeInfoList"/> that intersect with a specified range.
        /// </summary>
        /// <param name="range">The range to be searched for.</param>
        /// <returns>The <see cref="GridRangeInfoList"/> with ranges that match the criteria.</returns>
        public GridRangeInfoList GetRangesIntersecting(GridRangeInfo range)
        {
            GridRangeInfoList rl = new GridRangeInfoList();
            foreach (GridRangeInfo r in this)
            {
                if (r.IntersectsWith(range))
                    rl.Add(r);
            }

            return rl;
        }


        /// <summary>
        /// Searches for ranges in the <see cref="GridRangeInfoList"/> that are contained in a specified range.
        /// </summary>
        /// <param name="range">The range to be searched for.</param>
        /// <returns>The <see cref="GridRangeInfoList"/> with ranges that match the criteria.</returns>
        public GridRangeInfoList GetRangesContained(GridRangeInfo range)
        {
            GridRangeInfoList rl = new GridRangeInfoList();
            foreach (GridRangeInfo r in this)
            {
                if (range.Contains(r))
                    rl.Add(r);
            }

            return rl;
        }

        /// <summary>
        /// Returns a <see cref="GridRangeInfo"/> that spans over all ranges that intersect with a specified range.
        /// </summary>
        /// <param name="range">The range to be searched for.</param>
        /// <returns>A <see cref="GridRangeInfo"/> that contains the original <paramref name="range"/> and
        /// the outer bounds of all ranges that intersect with <paramref name="range"/>.
        /// </returns>
        public GridRangeInfo GetOuterRange(GridRangeInfo range)
        {
            bool bContinue = true;
            while (bContinue)
            {
                bContinue = false;
                foreach (GridRangeInfo rangeItem in GetRangesIntersecting(range))
                {
                    if (!range.Contains(rangeItem))
                    {
                        bContinue = true;
                        range = range.UnionRange(rangeItem);
                    }
                }
            }
            return range;
        }

        [Obsolete("Not sure if needed")]
        internal GridRangeInfo GetInnerRange(GridRangeInfo range)
        {
            foreach (GridRangeInfo rangeItem in GetRangesIntersecting(range))
                range = range.IntersectRange(rangeItem);

            return range;
        }

        /// <summary>
        /// Determines if a range in the range list contains the specified range.
        /// </summary>
        /// <param name="range">The range to be searched for.</param>
        /// <returns>True if any range contains the specified range.</returns>
        public bool AnyRangeContains(GridRangeInfo range) 
        {
            foreach (GridRangeInfo r in this)
                if (r.Contains(range))
                    return true;

            return false;
        }

        /// <summary>
        /// Determines if a range in the range list intersects with the specified range.
        /// </summary>
        /// <param name="range">The range to be searched for.</param>
        /// <returns>True if any range contains the specified range.</returns>
        public bool AnyRangeIntersects(GridRangeInfo range) 
        {
            foreach (GridRangeInfo r in this)
                if (r.IntersectsWith(range))
                    return true;

            return false;
        }

        /// <summary>
        /// Updates the range list when rows have been inserted in the grid.
        /// </summary>
        /// <param name="row">The row index where rows have been inserted.</param>
        /// <param name="count">The number of rows that were inserted.</param>
        public void InsertRows(int row, int count)
        {
            for (int i = 0; i < Count; i++)
            {
                GridRangeInfo r = this[i];
                if (r.InsertRows(row, count))
                    this[i] = r;
            }
        }
        
        /// <summary>
        /// Updates the range list when columns have been inserted in the grid.
        /// </summary>
        /// <param name="col">The column index where columns have been inserted.</param>
        /// <param name="count">The number of columns that were inserted.</param>
        public void InsertCols(int col, int count)
        {
            for (int i = 0; i < Count; i++)
            {
                GridRangeInfo r = this[i];
                if (r.InsertCols(col, count))
                    this[i] = r;
            }
        }
        
        /// <summary>
        /// Updates the range list when rows were removed from the grid.
        /// </summary>
        /// <param name="from">The first row index.</param>
        /// <param name="last">The last row index.</param>
        /// <param name="maxrow">The new row count in the grid.</param>
        public void RemoveRows(int from, int last, int maxrow)
        {
            for (int i = 0; i < Count; i++)
            {
                GridRangeInfo r = this[i];
                if (r.RemoveRows(from, last, maxrow))
                    this[i] = r.Height > 1 || r.Width > 1 ? r : GridRangeInfo.Empty;
            }
        }
        
        internal void EnsureRowLimits(int maxrow)
        {
            for (int i = 0; i < Count; i++)
            {
                GridRangeInfo r = this[i];
                if (r.EnsureRowLimits(maxrow))
                    this[i] = r;
            }
        }
        
        /// <summary>
        /// Updates the range list when columns were removed from the grid.
        /// </summary>
        /// <param name="from">The first column index.</param>
        /// <param name="last">The last column index.</param>
        /// <param name="maxcol">The new column count in the grid.</param>
        public void RemoveCols(int from, int last, int maxcol)
        {
            for (int i = 0; i < Count; i++)
            {
                GridRangeInfo r = this[i];
                if (r.RemoveCols(from, last, maxcol))
                    this[i] = r.Height > 1 || r.Width > 1 ? r : GridRangeInfo.Empty;
            }
        }
        
        internal void EnsureColLimits(int maxcol)
        {
            for (int i = 0; i < Count; i++)
            {
                GridRangeInfo r = this[i];
                if (r.EnsureColLimits(maxcol))
                    this[i] = r;
            }
        }
        
        /// <summary>
        /// Updates the range list when rows were moved in the grid.
        /// </summary>
        /// <param name="nFromRow">The first row index.</param>
        /// <param name="nToRow">The last row index.</param>
        /// <param name="nDestRow">The destination row index.</param>
        /// <param name="maxrow">The new row count in the grid.</param>
        public void MoveRows(int nFromRow, int nToRow, int nDestRow, int maxrow)
        {
            for (int i = 0; i < Count; i++)
            {
                GridRangeInfo r = this[i];
                if (r.MoveRows(nFromRow, nToRow, nDestRow, maxrow))
                    this[i] = r;
            }
        }
        
        /// <summary>
        /// Updates the range list when columns were moved in the grid.
        /// </summary>
        /// <param name="nFromCol">The first column index.</param>
        /// <param name="nToCol">The last column index.</param>
        /// <param name="nDestCol">The destination column index.</param>
        /// <param name="maxcol">The new column count in the grid.</param>
        public void MoveCols(int nFromCol, int nToCol, int nDestCol, int maxcol)
        {
            for (int i = 0; i < Count; i++)
            {
                GridRangeInfo r = this[i];
                if (r.MoveCols(nFromCol, nToCol, nDestCol, maxcol))
                    this[i] = r;
            }
        }

        /// <summary>
        /// Returns a range list with ranges that match a specified <see cref="GridRangeInfoType"/>.
        /// </summary>
        /// <param name="filter">The <see cref="GridRangeInfoType"/> to search for.</param>
        /// <returns>A <see cref="GridRangeInfoList"/> with ranges that match the above criteria.</returns>
        public GridRangeInfoList FilterRangeType(GridRangeInfoType filter) 
        {
            GridRangeInfoList rl = new GridRangeInfoList();
            foreach (GridRangeInfo r in this)
            {
                if (r.RangeType == filter)
                    rl.Add(r);
            }
            return rl;
        }

        /// <summary>
        /// Creates a range list and convert column and row ranges into cell ranges with the specified bounds.
        /// </summary>
        /// <param name="nFirstRow">Row index for the first non-label cell in grid area.</param>
        /// <param name="nFirstCol">Column index for the first non-label cell in grid area.</param>
        /// <param name="nRowCount">Last row in the grid.</param>
        /// <param name="nColCount">Last column in the grid</param>
        /// <returns>The <see cref="GridRangeInfoList"/> with new <see cref="GridRangeInfo"/> objects that this method creates.</returns>
        /// /<remarks>
        /// Column ranges will be converted to cell ranges using nFirstRow and nRowCount. 
        /// Row ranges will be converted to cell ranges using nFirstCol and nColCount. 
        /// Column ranges will be converted to cell ranges using all input parameters. 
        /// </remarks>
        public GridRangeInfoList ExpandRanges(int nFirstRow, int nFirstCol, int nRowCount, int nColCount)
        {
            GridRangeInfoList rl = new GridRangeInfoList();
            foreach (GridRangeInfo range in this)
                rl.Add(range.ExpandRange(nFirstRow, nFirstCol, nRowCount, nColCount));
            return rl;
        }

        /// <summary>
        /// Creates a range list with column ranges that match a specified criteria.
        /// </summary>
        /// <param name="filter">The filter criteria.</param>
        /// <returns>The range list with column ranges.</returns>
        /// <remarks>
        /// Use <para/>
        /// rangeList.GetColRanges(GridRangeInfoType.Cells|GridRangeInfoType.Cols);<para/>
        /// if you want to get column ranges for both ranges that are cells or column ranges.<para/>
        /// Use <para/>
        /// rangeList.GetColRanges(GridRangeInfoType.Cols);<para/>
        /// if you only want to get column ranges and not cell ranges.<para/>
        /// Neighboring ranges will be combined if they intersect or have the same border (e.g. C4:C5 and C6:C7 will be combined into C4:C7).
        /// </remarks>
        public GridRangeInfoList GetColRanges(GridRangeInfoType filter)
        {
            GridRangeInfoList rl = new GridRangeInfoList();

            RemoveEmptyRanges();
            GridRangeInfo[] ranges = new GridRangeInfo[this.Count];
            CopyTo(ranges, 0);

            // Sort by column.
            Array.Sort(ranges, new GridRangeInfo.ColComparer());

            // Combine intersecting columns ranges.
            GridRangeInfo colRange = new GridRangeInfo();
            foreach (GridRangeInfo r in ranges)
            {
                if (r.IsEmpty || filter != GridRangeInfoType.Empty && (r.RangeType & filter) == 0)
                    continue;

                if ((r.RangeType & GridRangeInfoType.Rows) != 0)
                {
                    rl.Add(GridRangeInfo.Table());
                    return rl;
                }
                else if (colRange.IsEmpty)
                {
                    colRange = GridRangeInfo.Cols(r.Left, r.Right);
                }
                else if (colRange.IntersectsWith(GridRangeInfo.Cols(r.Left-1, r.Right)))
                    colRange = colRange.UnionRange(r);
                else
                {
                    rl.Add(colRange);
                    colRange = r;
                }
            }

            if (!colRange.IsEmpty)
                rl.Add(colRange);

            return rl;
        }
            
        /// <summary>
        /// Creates a range list with row ranges that match a specified criteria.
        /// </summary>
        /// <param name="filter">The filter criteria.</param>
        /// <returns>The range list with row ranges.</returns>
        /// <remarks>
        /// Use <para/>
        /// rangeList.GetRowRanges(GridRangeInfoType.Cells|GridRangeInfoType.Rows);<para/>
        /// if you want to get row ranges for both ranges that are cells or row ranges.<para/>
        /// Use <para/>
        /// rangeList.GetRowRanges(GridRangeInfoType.Rows);<para/>
        /// if you only want to get row ranges and not cell ranges.<para/>
        /// Neighboring ranges will be combined if they intersect or have the same border (e.g. R4:R5 and R6:R7 will be combined into R4:R7).
        /// </remarks>
        public GridRangeInfoList GetRowRanges(GridRangeInfoType filter)
        {
            GridRangeInfoList rl = new GridRangeInfoList();

            RemoveEmptyRanges();
            GridRangeInfo[] ranges = new GridRangeInfo[this.Count];
            CopyTo(ranges, 0);

            // Sort by column.
            Array.Sort(ranges, new GridRangeInfo.RowComparer());

            // Combine intersecting columns ranges.
            GridRangeInfo rowRange = new GridRangeInfo();
            foreach (GridRangeInfo r in ranges)
            {
                if (r.IsEmpty || filter != GridRangeInfoType.Empty && (r.RangeType & filter) == 0)
                    continue;

                if ((r.RangeType & GridRangeInfoType.Cols) != 0)
                {
                    rl.Add(GridRangeInfo.Table());
                    return rl;
                }
                else if (rowRange.IsEmpty)
                {
                    rowRange = GridRangeInfo.Rows(r.Top, r.Bottom);
                }
                else if (rowRange.IntersectsWith(GridRangeInfo.Rows(r.Top-1, r.Bottom)))
                    rowRange = rowRange.UnionRange(r);
                else
                {
                    rl.Add(rowRange);
                    rowRange = r;
                }
            }

            if (!rowRange.IsEmpty)
                rl.Add(rowRange);

            return rl;
        }

        internal static GridRangeInfoList FromString(string parseText) 
        {
            return Parse(parseText);
        }

        /// <summary>
        /// Creates a range list from a string with comma separated range descriptions.
        /// </summary>
        /// <param name="parseText">The text to be parsed.</param>
        /// <returns>The <see cref="GridRangeInfoList"/> with range objects described in the string.</returns>
        /// <remarks>
        /// The method parses a string that was previously created with <see cref="GridRangeInfoList.ToString"/>.
        /// </remarks>
        public static GridRangeInfoList Parse(string parseText) 
        {
            GridRangeInfoList rl = new GridRangeInfoList();
            string[] parts = parseText.Split(';');
            foreach (string s in parts)
            {
                GridRangeInfo r = GridRangeInfo.Parse(s.Trim());
                if (!r.IsEmpty)
                    rl.Add(r);
            }

            return rl;
        }

        
        /// <summary>
        /// Results of ToString method.
        /// </summary>
        [Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public string Info
        {
            get
            {
                return ToString();
            }
        }

        /// <overload>
        ///   <para>Converts the ranges of this <see cref="GridRangeInfo"/> to a comma separated human-readable string.</para>
        /// </overload>
        /// <summary>
        ///   <para>Converts the ranges of this <see cref="GridRangeInfo"/> to a comma separated human-readable string.</para>
        /// </summary>
        /// <remarks>
        /// The generate string will be in the format R#C#, R#, C#, T.<para/>
        /// An example for a range of cells is "R1C1:R10C20" or "R5C5".<para/>
        /// An example for a range of rows is "R1:R10" or "R6".<para/>
        /// An example for a range of columns is "C1:C10" or "C7".<para/>
        /// An example for a table range is "T".<para/>
        /// Each range object will be comma separated. For example: "R3:R5,R8:R10,R14:R15".<para/>
        /// </remarks>
        /// <returns>
        ///   <para>A string that contains the column and row index of the top-left and bottom-right position of this <see cref="GridRangeInfo"/>.</para>
        /// </returns>
        public override string ToString()
        {
            return this.ToString(null, null);
        }

        /// <summary>
        ///   <para>Converts the ranges of this <see cref="GridRangeInfo"/> to a comma separated human-readable string.</para>
        /// </summary>
        /// <param name="provider">The <see cref="IFormatProvider"/> to use to format the value. Is ignored.</param>
        /// <returns>
        ///   <para>A string that contains the column and row index of the top-left and bottom-right position of this <see cref="GridRangeInfo"/>.</para>
        /// </returns>
        /// <genoverload/>
        public string ToString(IFormatProvider provider)
        {
            return this.ToString(null, provider);
        }
        
        /// <summary>
        ///   <para>Converts the ranges of this <see cref="GridRangeInfo"/> to a comma separated human-readable string.</para>
        /// </summary>
        /// <param name="format">Ignored.</param>
        /// <returns>
        ///   <para>A string that contains the column and row index of the top-left and bottom-right position of this <see cref="GridRangeInfo"/>.</para>
        /// </returns>
        /// <genoverload/>
        public string ToString(string format)
        {
            return this.ToString(format, null);
        }

        /// <summary>
        ///   <para>Converts the ranges of this <see cref="GridRangeInfo"/> to a comma separated human-readable string.</para>
        /// </summary>
        /// <param name="formatProvider">The <see cref="IFormatProvider"/> to use to format the value. Is ignored.</param>
        /// <param name="format">Ignored.</param>
        /// <returns>
        ///   <para>A string that contains the column and row index of the top-left and bottom-right position of this <see cref="GridRangeInfo"/>.</para>
        /// </returns>
        /// <genoverload/>
        public string ToString(string format, IFormatProvider formatProvider)
        {
            if (format == null || format.Length == 0 || format == "G" )
            {
                StringBuilder sb = new StringBuilder();
                string separate = separator.ToString() + " ";
                bool first = true;
                foreach (GridRangeInfo r in this)
                {
                    if (r.IsEmpty)
                        continue;

                    if (!first)
                        sb.Append(separate);
                    else
                        first = false;

                    sb.Append(r.ToString(format, formatProvider));
                }

                return sb.ToString();
            }

            return "";
        }

        #if TESTA
        public static void Main( ) 
        {
            Console.WriteLine("Range 1..5");
            GridRangeInfo r1 = new GridRangeInfo();
            GridRangeInfo r2 = GridRangeInfo.Empty;
            GridRangeInfo r3 = GridRangeInfo.Cells(1, 1, 10, 5);
            GridRangeInfo r4 = GridRangeInfo.Cols(1, 4);
            GridRangeInfo r5 = GridRangeInfo.Table();

            Console.WriteLine("{0}", r1);
            Console.WriteLine(r1.ToString());
            Console.WriteLine("{0}", r1);
            Console.WriteLine(r2.ToString());
            Console.WriteLine("{0}", r3);
            Console.WriteLine(r3.ToString());
            Console.WriteLine("{0}", r4);
            Console.WriteLine(r4.ToString());
            Console.WriteLine("{0}", r5);
            Console.WriteLine(r5.ToString());
            Console.WriteLine("");

            Console.WriteLine("IntersectRange");
            r1 = GridRangeInfo.Cell(2, 2);
            r2 = GridRangeInfo.IntersectRange(r1, r3);
            r1.IntersectRange(r3);

            Console.WriteLine("{0}", r1);
            Console.WriteLine(r1.ToString());
            Console.WriteLine("{0}", r1);
            Console.WriteLine(r2.ToString());
            Console.WriteLine("{0}", r3);

            if (r1 == r2)
                Console.WriteLine("Equals");
            else
                Console.WriteLine("Error");

            Console.WriteLine("");
            Console.WriteLine("UnionRange");

            r1 = GridRangeInfo.Cell(1, 1);
            r2 = GridRangeInfo.Cells(3, 3, 5, 2);
            r3 = GridRangeInfo.FromTLHW(3, 3, 2, 2);

            r4 = GridRangeInfo.UnionRange(r1, r2);
            r5 = r1;
            r5.UnionRange(r2);

            Console.WriteLine("");
            Console.WriteLine("Range 1:5");
            Console.WriteLine("{0}", r1);
            Console.WriteLine(r1.ToString());
            Console.WriteLine("{0}", r1);
            Console.WriteLine(r2.ToString());
            Console.WriteLine("{0}", r3);
            Console.WriteLine(r3.ToString());
            Console.WriteLine("{0}", r4);
            Console.WriteLine(r4.ToString());
            Console.WriteLine("{0}", r5);
            Console.WriteLine(r5.ToString());

            Console.WriteLine("Add to rl1");
            GridRangeInfoList rl1 = new GridRangeInfoList();
            rl1.Add(r5);
            rl1.Add(r1);
            rl1.Add(r1);
            rl1.Add(r2);
            rl1.Add(GridRangeInfo.Empty);
            rl1.Add(GridRangeInfo.Table());

            Console.WriteLine("Contents of RangeList");
            foreach (GridRangeInfo r in rl1)
                Console.WriteLine(r.ToString());

            rl1.RemoveEmptyRanges();
            r1 = GridRangeInfo.Cell(9,9);

            Console.WriteLine("Contents of RangeList after RemoveEmptyRanges");
            foreach (GridRangeInfo r in rl1)
                Console.WriteLine(r.ToString());

            Console.WriteLine("");
            Console.WriteLine("IndexOf test");
            int i = rl1.IndexOf(GridRangeInfo.Cell(1, 1));
            Console.WriteLine("IndexOf(1,1) = " + i.ToString());
            int i2 = rl1.IndexOf(r1);
            Console.WriteLine("IndexOf(" + r1.ToString() + ") = " + i2.ToString());

            Console.WriteLine("");
            Console.WriteLine("RangeList Insert Test");
            rl1.Insert(0, GridRangeInfo.Cell(100,100));
            rl1.Insert(3, GridRangeInfo.Empty);
            Console.WriteLine("RangeList");
            foreach (GridRangeInfo r in rl1)
                Console.WriteLine(r.ToString());

            Console.WriteLine("");
            Console.WriteLine("Assignment");
           GridRangeInfoList rl2 = rl1;
            rl1[1] = GridRangeInfo.Row(999);
                // Note: this change in rl1 will be reflected in r2
            foreach (GridRangeInfo r in rl2)
                Console.WriteLine(r.ToString());

            Console.WriteLine("");
            Console.WriteLine("CopyTo");
            GridRangeInfoList rl3 = new GridRangeInfoList();
            rl2.CopyTo(rl3, 0);
            rl2[1] = GridRangeInfo.Row(1111);
            foreach (GridRangeInfo r in rl3)
                Console.WriteLine(r.ToString());

            Console.WriteLine("");
            Console.WriteLine("Clone");
            GridRangeInfoList rl4 = rl1.Clone();
            rl1[1] = GridRangeInfo.Col(4444);
            foreach (GridRangeInfo r in rl4)
                Console.WriteLine(r.ToString());
            r1 = GridRangeInfo.Col(4444);
            int i3 = rl1.IndexOf(r1);
            Console.WriteLine("IndexOf(" + r1.ToString() + ") = " + i3.ToString());


            Console.WriteLine("");
            Console.WriteLine("Intersect, Empty");
            GridRangeInfo o1 = GridRangeInfo.Cell(5,5);
            GridRangeInfo o2 = GridRangeInfo.Row(10);
            GridRangeInfo o3 = o1.IntersectRange(o2);
            Console.WriteLine(o3.ToString());
            if (o3 == GridRangeInfo.Empty)
                Console.WriteLine("Empty");


            Console.WriteLine("");
            Console.WriteLine("Intersect, RangeType");
            GridRangeInfo so1 = GridRangeInfo.Cols(5,6);
            GridRangeInfo so2 = GridRangeInfo.Cols(4,6);
            GridRangeInfo so3 = so1.IntersectRange(so2);
            Console.WriteLine(so3);
            if (so3 == GridRangeInfo.Empty)
                Console.WriteLine("Empty");


            // Note:
            // rl1[1].SetCell(1); // will not have any effect !!!
            // Therefore I made GridRangeInfo immutable

            Console.WriteLine("");
            rl4.Clear();
            rl4.Add(GridRangeInfo.Row(4));
            rl4.Add(GridRangeInfo.Row(7));
            rl4.Add(GridRangeInfo.Rows(3,5));
            rl4.Add(GridRangeInfo.Cell(10, 14));
            rl4.Add(GridRangeInfo.Rows(11, 14));
            rl4.Add(GridRangeInfo.Rows(13, 15));

            Console.WriteLine("rl4");
            foreach (GridRangeInfo r in rl4)
                Console.WriteLine(r.ToString());

            Console.WriteLine("colList1");
            GridRangeInfoList colList1 = rl4.GetColRanges(GridRangeInfoType.Cells|GridRangeInfoType.Cols);
            foreach (GridRangeInfo r in colList1)
                Console.WriteLine(r.ToString());

            Console.WriteLine("colList2");
            GridRangeInfoList colList2 = rl4.GetColRanges(GridRangeInfoType.Cols);
            foreach (GridRangeInfo r in colList2)
                Console.WriteLine(r.ToString());

            Console.WriteLine("rowList1");
            GridRangeInfoList rowList1 = rl4.GetRowRanges(GridRangeInfoType.Cells|GridRangeInfoType.Rows);
            foreach (GridRangeInfo r in rowList1)
                Console.WriteLine(r.ToString());

            Console.WriteLine("rowList2");
            GridRangeInfoList rowList2 = rl4.GetRowRanges(GridRangeInfoType.Rows);
            foreach (GridRangeInfo r in rowList2)
                Console.WriteLine(r.ToString());

            Console.Write(GridRangeInfo.GetAlphaLabel(0));
            for (int n = 1; n < 100; n++)
            {
                if (n%10 == 0)
                    Console.WriteLine();
                Console.Write(", " + GridRangeInfo.GetAlphaLabel(n));
            }
            Console.WriteLine();
            for (int n = 0; n < 100; n++)
            {
                if (n%10 == 0)
                    Console.WriteLine();
                Console.Write(", " + GridRangeInfo.GetAlphaLabel(n+26*26-5));
            }
            Console.WriteLine();
            for (int n = 0; n < 100; n++)
            {
                if (n%10 == 0)
                    Console.WriteLine();
                Console.Write(", " + GridRangeInfo.GetAlphaLabel(n+26*26*26+26*26-5));
            }
            Console.WriteLine();

            rl4.Add(GridRangeInfo.Table());
            rl4.Add(GridRangeInfo.Cell(11,11));
            rl4.Add(GridRangeInfo.Col(33));
            rl4.Add(GridRangeInfo.Cols(3333,3338));
            foreach (GridRangeInfo r in rl4)
                Console.WriteLine("Formatted: {0}, Parsed: {1}", r, GridRangeInfo.Parse(r.Format("G", null)));

            Console.WriteLine();
            string sl = rl4.Format("G", null);
            Console.WriteLine(sl);

            GridRangeInfoList rl9 = GridRangeInfoList.Parse(sl);
            string s2 = rl9.Format("G", null);
            Console.WriteLine(s2);


        }
        #endif
    }
}
