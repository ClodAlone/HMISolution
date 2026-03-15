//-------------------------------------------------------------------------------------------------
// <copyright file="GridCellsMemento.cs" company="syncfusion">
//  Copyright (c) Syncfusion Inc. 2001 - 2014. All rights reserved.
//  Use of this code is subject to the terms of our license.
//  A copy of the current license can be obtained at any time by e-mailing
//  licensing@syncfusion.com. Re-distribution in any form is strictly
//  prohibited. Any infringement will be prosecuted under applicable laws. 
// </copyright>
//-------------------------------------------------------------------------------------------------

using System;
using System.Drawing;
using System.Collections;
using System.Collections.Specialized;
using System.Xml;
using System.Xml.Schema;
using System.Xml.Serialization;
using System.Security.Permissions;
using System.Runtime.Serialization;
using System.Data;
using System.Diagnostics;
using Syncfusion.Windows.Forms.Grid;
using Syncfusion.Diagnostics;
using Syncfusion.Collections;
using Syncfusion.Styles;

namespace Syncfusion.Windows.Forms.Grid.Design
{
    #region GridCellsMemento
    /// <summary>
    /// Contains information needed for Xml serialization relating to col/row styles, individual cell StyleInfo objects, etc.
    /// </summary>
    [Serializable]
    public class GridCellsMemento : ISerializable
    {
        #region Members
        ColumnStyles colStyles;
        RowStyles rowStyles;
        GridCellInfoCollection cells;
        GridNonImmutableRangeInfoCollection coveredRanges;
        GridNonImmutableRangeInfoCollection banneredRanges;
        GridBaseStylesMap baseStylesMap;
        GridBaseStyleCollection gridBaseStyles;
        #endregion

        #region Constructors
        /// <summary>
        ///     Default constructor for Xml serialization, setting style objects to null;
        /// </summary>
        public GridCellsMemento()
        {
            rowStyles = null;
            colStyles = null;
        }

        /// <summary>
        ///     Creates the GridCellsMemento object, initialized from the supplied GridControl.
        /// </summary>
        /// <param name="grid" type="Syncfusion.Windows.Forms.Grid.GridControl">
        ///     <para>
        ///         The GridControl to initialize the GridCellsMemento object.
        ///     </para>
        /// </param>
        public GridCellsMemento(GridControl grid)
        {
            this.InitializeFrom(grid);
        }

        /// <summary>
        /// Initializes a new <see cref="GridCellsMemento"/> from a serialization stream.
        /// </summary>
        /// <param name="info">An object that holds all the data needed to serialize or deserialize this instance.</param>
        /// <param name="context">Describes the source and destination of the serialized stream specified by info. </param>
        protected GridCellsMemento(SerializationInfo info, StreamingContext context)
        {
#if DEBUG
            if (Switches.Serialization.TraceVerbose)
            {
                TraceUtil.TraceCurrentMethodInfo(info.FullTypeName, info.MemberCount);
            }
#else
            ;
#endif
            colStyles = (ColumnStyles)info.GetValue("ColumnStyles", typeof(ColumnStyles));
            rowStyles = (RowStyles)info.GetValue("RowStyles", typeof(RowStyles));
            cells = (GridCellInfoCollection)info.GetValue("Data", typeof(GridCellInfoCollection));
            coveredRanges = (GridNonImmutableRangeInfoCollection)info.GetValue("CoveredRanges", typeof(GridNonImmutableRangeInfoCollection));
            banneredRanges = (GridNonImmutableRangeInfoCollection)info.GetValue("BanneredRanges", typeof(GridNonImmutableRangeInfoCollection));
            baseStylesMap = (GridBaseStylesMap)info.GetValue("BaseStylesMap", typeof(GridBaseStylesMap));
        }

        /// <summary>
        /// Implements the ISerializable interface and returns the data needed to serialize the <see cref="GridCellsMemento"/>.
        /// </summary>
        /// <param name="info">A SerializationInfo object containing the information required to serialize the object.</param>
        /// <param name="context">A StreamingContext object containing the source and destination of the serialized stream.</param>
        void ISerializable.GetObjectData(SerializationInfo info, StreamingContext context)
        {
#if DEBUG
            if (Switches.Serialization.TraceVerbose)
            {
                TraceUtil.TraceCurrentMethodInfo(info.FullTypeName, info.MemberCount);
            }
#else
            ;
#endif
            GetObjectData(info, context);
        }

        /// <summary>
        /// Returns the data needed to serialize the <see cref="GridCellsMemento"/>.
        /// </summary>
        /// <param name="info">A SerializationInfo object containing the information required to serialize the object.</param>
        /// <param name="context">A StreamingContext object containing the source and destination of the serialized stream.</param>
        [SecurityPermissionAttribute(SecurityAction.Demand, SerializationFormatter = true)]
        protected virtual void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            info.AddValue("ColumnStyles", colStyles);
            info.AddValue("RowStyles", rowStyles);
            info.AddValue("Data", cells);
            info.AddValue("CoveredRanges", coveredRanges);
            info.AddValue("BanneredRanges", banneredRanges);
            info.AddValue("BaseStylesMap", baseStylesMap);
        }
        #endregion

        #region Public Properties
        /// <copyfrom cref="ColumnStyles"/>
        /// <summary>Gets or sets the grid column styles.</summary>
        [XmlElement("ColumnStyleCollection")]
        public ColumnStyles GridColStyles
        {
            get
            {
                if (colStyles == null)
                {
                    colStyles = new ColumnStyles();
                }

                return colStyles;
            }

            set
            {
                colStyles = value;
            }
        }

        /// <copyfrom cref="RowStyles"/>
        /// <summary>Gets or sets the grid row styles.</summary>
        [XmlElement("RowStyleCollection")]
        public RowStyles GridRowStyles
        {
            get
            {
                if (rowStyles == null)
                {
                    rowStyles = new RowStyles();
                }

                return rowStyles;
            }

            set
            {
                rowStyles = value;
            }
        }

        /// <summary>
        ///  Gets or sets Collection of <see cref="GridNonImmutableRangeInfo"/> objects that are to be treated as Covered
        /// </summary>
        public GridNonImmutableRangeInfoCollection CoveredRanges
        {
            get
            {
                if (this.coveredRanges == null)
                {
                    coveredRanges = new GridNonImmutableRangeInfoCollection();
                }

                return this.coveredRanges;
            }

            set
            {
                this.coveredRanges = value;
            }
        }

        /// <summary>
        /// Gets or sets Collection of <see cref="GridNonImmutableRangeInfo"/> objects that are to be treated as Bannered
        /// </summary>
        public GridNonImmutableRangeInfoCollection BanneredRanges
        {
            get
            {
                if (this.banneredRanges == null)
                {
                    banneredRanges = new GridNonImmutableRangeInfoCollection();
                }
                
                return this.banneredRanges;
            }

            set
            {
                this.banneredRanges = value;
            }
        }

        /// <summary>
        /// Gets or sets <see cref="GridBaseStyleCollection"/> to Serialize     
        /// </summary>
        [XmlElement("BaseStylesMap")]
        public GridBaseStyleCollection GridBaseStylesToSerialize
        {
            get
            {
                return gridBaseStyles;
            }

            set
            {
                gridBaseStyles = value;
            }
        }

        /// <internalonly/>
        [Syncfusion.Documentation.DocumentationExclude()]
        [XmlIgnore()]
        private GridBaseStylesMap BaseStylesMap
        {
            get
            {
                if (gridBaseStyles != null)
                {
                    baseStylesMap = new GridBaseStylesMap();
                    GridBaseStyleCollection.GridBaseStyleEnumerator styleEnum = gridBaseStyles.GetEnumerator();
                    while (styleEnum.MoveNext())
                    {
                        baseStylesMap.Add(styleEnum.Current);
                    }
                }

                if (baseStylesMap == null)
                {
                    baseStylesMap = new GridBaseStylesMap();
                    baseStylesMap.RegisterStandardStyles();
                }

                return baseStylesMap;
            }

            set
            {
                baseStylesMap = value;
                SetGridBaseStylesToSerialize(baseStylesMap);
            }
        }

        /// <summary>
        /// Retrieves the <see cref="GridBaseStyle"/> objects from the provided map, and places into the collection.
        /// </summary>
        private void SetGridBaseStylesToSerialize(GridBaseStylesMap map)
        {
            gridBaseStyles = new GridBaseStyleCollection();

            IEnumerator ienum = map.baseStyles.Keys.GetEnumerator();
            while (ienum.MoveNext())
            {
                gridBaseStyles.Add(map[ienum.Current.ToString()]);
            }
        }

        /// <summary>
        ///  Gets or sets collection of <see cref="GridCellInfo"/> objects containing the cell data to save out.
        /// </summary>
        [XmlElement("CellData")]
        public GridCellInfoCollection Cells
        {
            get
            {
                if (cells == null)
                {
                    cells = new GridCellInfoCollection();
                }

                return cells;
            }

            set
            {
                cells = value;
            }
        }
        #endregion //Public Properties

        #region Actions
        /// <summary>
        ///     Initializes the GridCellsMemento object with styles, data, etc from the supplied GridControl.
        /// </summary>
        /// <param name="grid" type="Syncfusion.Windows.Forms.Grid.GridControl">
        ///     <para>
        ///            The GridControl that the values will be retrieved from
        ///     </para>
        /// </param>
        public void InitializeFrom(Syncfusion.Windows.Forms.Grid.GridControl grid)
        {
            this.CoveredRanges.Clear();
            this.BanneredRanges.Clear();
            this.BaseStylesMap = grid.BaseStylesMap;

            ////null out rowheightentries to ensure that the proper collection is retrieved on the get.
            grid.RowHeightEntries = null;
            this.GridRowStyles.RowHeights = new GridRowColEntryCollection();

            foreach (GridRowHeight rh in grid.RowHeightEntries)
            {
                this.GridRowStyles.RowHeights.Add(new GridRowColEntry(rh.RowIndex, rh.Height));
            }

            this.GridRowStyles.HiddenRows = new GridRowColEntryCollection();
            foreach (GridRowHidden row in grid.RowHiddenEntries)
            {
                this.GridRowStyles.HiddenRows.Add(new GridRowColEntry(row.RowIndex, 0));
            }

            ////null out colwidthentries to ensure that the proper collection is retrieved on the get.
            grid.ColWidthEntries = null;

            this.GridColStyles.ColumnWidths = new GridRowColEntryCollection();
            foreach (GridColWidth cw in grid.ColWidthEntries)
            {
                this.GridColStyles.ColumnWidths.Add(new GridRowColEntry(cw.ColIndex, cw.Width));
            }

            this.GridColStyles.HiddenColumns = new GridRowColEntryCollection();
            foreach (GridColHidden col in grid.ColHiddenEntries)
            {
                this.GridColStyles.HiddenColumns.Add(new GridRowColEntry(col.ColIndex, 0));
            }

            for (int rangeID = 0; rangeID < grid.CoveredRanges.Count; rangeID++)
            {
                this.CoveredRanges.Add(new GridNonImmutableRangeInfo(grid.CoveredRanges.Ranges[rangeID]));
            }

            for (int bannerID = 0; bannerID < grid.BanneredRanges.Count; bannerID++)
            {
                this.BanneredRanges.Add(new GridNonImmutableRangeInfo(grid.BanneredRanges.Ranges[bannerID]));
            }

            ////force refresh;
            grid.GridCells = null;
            this.Cells = new GridCellInfoCollection(grid.GridCells);
            foreach (GridCellInfo cellInfo in this.Cells)
            {
                GridStylesParser.ParsePropertyStore(cellInfo.StyleInfo.Store);
            }
        }

        /// <summary>
        ///     Applies the values contained in the GridCellsMemento object to the supplied GridControl.
        /// </summary>
        /// <param name="grid" type="Syncfusion.Windows.Forms.Grid.GridControl">
        ///     <para>
        ///         The GridControl to apply the values to.
        ///     </para>
        /// </param>
        public void ApplyTo(Syncfusion.Windows.Forms.Grid.GridControl grid)
        {
            grid.BaseStylesMap = this.BaseStylesMap;
            grid.RowHiddenEntries.Clear();
            foreach (GridRowColEntry entry in this.GridRowStyles.HiddenRows)
            {
                grid.RowHiddenEntries.Add(new GridRowHidden(entry.Index));
            }

            grid.RowHeightEntries.Clear();
            foreach (GridRowColEntry entry in this.GridRowStyles.RowHeights)
            {
                grid.RowHeightEntries.Add(new GridRowHeight(entry.Index, entry.Length));
            }

            grid.ColHiddenEntries.Clear();
            foreach (GridRowColEntry entry in this.GridColStyles.HiddenColumns)
            {
                grid.ColHiddenEntries.Add(new GridColHidden(entry.Index));
            }

            grid.ColWidthEntries.Clear();
            foreach (GridRowColEntry entry in this.GridColStyles.ColumnWidths)
            {
                grid.ColWidthEntries.Add(new GridColWidth(entry.Index, entry.Length));
            }

            ////covered ranges
            GridNonImmutableRangeInfoCollection.GridNonImmutableRangeInfoEnumerator ienum = CoveredRanges.GetEnumerator();
            grid.CoveredRanges.Clear();

            while (ienum.MoveNext())
            {
                grid.CoveredRanges.Add(ienum.Current.ToRangeInfo());
            }

           ////bannered ranges
            ienum = BanneredRanges.GetEnumerator();
            grid.BanneredRanges.Clear();

            while (ienum.MoveNext())
            {
                grid.BanneredRanges.Add(ienum.Current.ToRangeInfo());
            }

            grid.GridCells = new GridCellInfoCollection(this.Cells);
        }

        #endregion //Actions
    }
    #endregion

    #region ColumnStyles
    /// <summary>
    ///     Contains hidden entries and column widths of the grid, in <see cref="GridRowColEntryCollection"/> objects.
    /// </summary>
    [Serializable]
    public class ColumnStyles
    {
        GridRowColEntryCollection hiddenEntries;
        GridRowColEntryCollection widthEntries;

        static ColumnStyles()
        { 
        }

        /// <summary>
        ///     Creates an empty <see cref="ColumnStyles"/> object.
        /// </summary>
        public ColumnStyles()
        {
        }

        /// <summary>
        ///  Gets or sets hidden entries in <see cref="GridRowColEntryCollection"/> objects.
        /// </summary>
        public GridRowColEntryCollection HiddenColumns
        {
            get
            {
                return hiddenEntries;
            }

            set
            {
                hiddenEntries = value;
            }
        }

        /// <summary>
        ///  Gets or sets column widths of the grid, in <see cref="GridRowColEntryCollection"/> objects.
        /// </summary>
        public GridRowColEntryCollection ColumnWidths
        {
            get
            {
                return widthEntries;
            }

            set
            {
                widthEntries = value;
            }
        }
    }
    #endregion //ColumnStyles

    #region RowStyles
    /// <summary>
    ///     Contains hidden entries and row heights of the grid, in <see cref="GridRowColEntryCollection"/> objects.
    /// </summary>
    [Serializable]
    public class RowStyles
    {
        GridRowColEntryCollection hiddenEntries;
        GridRowColEntryCollection heightEntries;

        static RowStyles()
        { 
        }

        /// <summary>
        ///     Creates an empty <see cref="RowStyles"/> object.
        /// </summary>
        public RowStyles()
        {
        }

        /// <summary>
        ///  Gets or sets hidden row entries of the grid, in <see cref="GridRowColEntryCollection"/> objects.
        /// </summary>
        public GridRowColEntryCollection HiddenRows
        {
            get
            {
                return hiddenEntries;
            }

            set
            {
                hiddenEntries = value;
            }
        }
        
        /// <summary>
        ///   Gets or sets row heights of the grid, in <see cref="GridRowColEntryCollection"/> objects.
        /// </summary>
        public GridRowColEntryCollection RowHeights
        {
            get
            {
                return heightEntries;
            }

            set
            {
                heightEntries = value;
            }
        }
    }
    #endregion //RowStyles

    #region GridRowColEntry
    /// <summary>
    ///     Defines a row or a column by an index, and length (width or height).
    /// </summary>
    [Serializable]
    public class GridRowColEntry
    {
        int index;
        int length;

        /// <summary>
        ///     Creates an empty <see cref="GridRowColEntry"/> object.
        /// </summary>
        public GridRowColEntry()
        {
        }

        /// <summary>
        ///     Creates a new <see cref="GridRowColEntry"/> object with the supplied index and length.
        /// </summary>
        /// <param name="index">Index of the column or row</param>
        /// <param name="length">
        /// <para>If the <see cref="GridRowColEntry"/> object is for a column, this is the width of the column.</para>
        /// <para>If the <see cref="GridRowColEntry"/> object is for a row, this is the height of the row.</para>
        /// </param>
        public GridRowColEntry(int index, int length)
        {
            this.index = index;
            this.length = length;
        }

        /// <summary>Gets or sets the index of the column or row represented in the <see cref="GridRowColEntry"/> object.</summary>
        [XmlAttribute("Index")]
        public int Index
        {
            get
            {
                return index;
            }

            set
            {
                index = value;
            }
        }

        /// <summary>
        ///    <para>Gets or sets the Length. If the <see cref="GridRowColEntry"/> object is for a column, this is the width of the column.</para>
        ///    <para>If the <see cref="GridRowColEntry"/> object is for a row, this is the height of the row.</para>
        /// </summary>
        [XmlAttribute("Length")]
        public int Length
        {
            get
            {
                return length;
            }

            set
            {
                length = value;
            }
        }
    }
    #endregion //GridRowColEntry

    #region "'GridRowColEntryCollection' strongly typed collection class"

    /// <summary>
    ///     A collection that stores 'GridRowColEntry' objects.
    /// </summary>
    [Serializable()]
    public class GridRowColEntryCollection : System.Collections.CollectionBase
    {
        /// <summary>
        ///     Initializes a new instance of 'GridRowColEntryCollection'.
        /// </summary>
        public GridRowColEntryCollection()
        {
        }

        /// <summary>
        ///     Initializes a new instance of 'GridRowColEntryCollection' based on an already existing instance.
        /// </summary>
        /// <param name='griValue'>
        ///     A 'GridRowColEntryCollection' from which the contents is copied
        /// </param>
        public GridRowColEntryCollection(GridRowColEntryCollection griValue)
        {
            this.AddRange(griValue);
        }

        /// <summary>
        ///     Initializes a new instance of 'GridRowColEntryCollection' with an array of 'GridRowColEntry' objects.
        /// </summary>
        /// <param name='griValue'>
        ///     An array of 'GridRowColEntry' objects with which to initialize the collection
        /// </param>
        public GridRowColEntryCollection(GridRowColEntry[] griValue)
        {
            this.AddRange(griValue);
        }

        /// <summary>
        ///     Represents the 'GridRowColEntry' item at the specified index position.
        /// </summary>
        /// <param name='intIndex'>
        ///     The zero-based index of the entry to locate in the collection.
        /// </param>
        /// <value>
        ///     The entry at the specified index of the collection.
        /// </value>
        public GridRowColEntry this[int intIndex]
        {
            get
            {
                return (GridRowColEntry)List[intIndex];
            }

            set
            {
                List[intIndex] = value;
            }
        }

        /// <summary>
        ///     Adds a 'GridRowColEntry' item with the specified value to the 'GridRowColEntryCollection'
        /// </summary>
        /// <param name='griValue'>
        ///     The 'GridRowColEntry' to add.
        /// </param>
        /// <returns>
        ///     The index at which the new element was inserted.
        /// </returns>
        public int Add(GridRowColEntry griValue)
        {
            return List.Add(griValue);
        }

        /// <summary>
        ///     Copies the elements of an array at the end of this instance of 'GridRowColEntryCollection'.
        /// </summary>
        /// <param name='griValue'>
        ///     An array of 'GridRowColEntry' objects to add to the collection.
        /// </param>
        public void AddRange(GridRowColEntry[] griValue)
        {
            for (int intCounter = 0; intCounter < griValue.Length; intCounter = intCounter + 1)
            {
                this.Add(griValue[intCounter]);
            }
        }

        /// <summary>
        ///     Adds the contents of another 'GridRowColEntryCollection' at the end of this instance.
        /// </summary>
        /// <param name='griValue'>
        ///     A 'GridRowColEntryCollection' containing the objects to add to the collection.
        /// </param>
        public void AddRange(GridRowColEntryCollection griValue)
        {
            for (int intCounter = 0; intCounter < griValue.Count; intCounter = intCounter + 1)
            {
                this.Add(griValue[intCounter]);
            }
        }

        /// <summary>
        ///     Gets a value indicating whether the 'GridRowColEntryCollection' contains the specified value.
        /// </summary>
        /// <param name='griValue'>
        ///     The item to locate.
        /// </param>
        /// <returns>
        ///     True if the item exists in the collection; false otherwise.
        /// </returns>
        public bool Contains(GridRowColEntry griValue)
        {
            return List.Contains(griValue);
        }

        /// <summary>
        ///     Copies the 'GridRowColEntryCollection' values to a one-dimensional System.Array
        ///     instance starting at the specified array index.
        /// </summary>
        /// <param name='griArray'>
        ///     The one-dimensional System.Array that represents the copy destination.
        /// </param>
        /// <param name='intIndex'>
        ///     The index in the array where copying begins.
        /// </param>
        public void CopyTo(GridRowColEntry[] griArray, int intIndex)
        {
            List.CopyTo(griArray, intIndex);
        }

        /// <summary>
        ///     Returns the index of a 'GridRowColEntry' object in the collection.
        /// </summary>
        /// <param name='griValue'>
        ///     The 'GridRowColEntry' object whose index will be retrieved.
        /// </param>
        /// <returns>
        ///     If found, the index of the value; otherwise, -1.
        /// </returns>
        public int IndexOf(GridRowColEntry griValue)
        {
            return List.IndexOf(griValue);
        }

        /// <summary>
        ///     Inserts an existing 'GridRowColEntry' into the collection at the specified index.
        /// </summary>
        /// <param name='intIndex'>
        ///     The zero-based index where the new item should be inserted.
        /// </param>
        /// <param name='griValue'>
        ///     The item to insert.
        /// </param>
        public void Insert(int intIndex, GridRowColEntry griValue)
        {
            List.Insert(intIndex, griValue);
        }

        /// <summary>
        ///     Returns an enumerator that can be used to iterate through
        ///     the 'GridRowColEntryCollection'.
        /// </summary>
        /// <returns>returns Enumerator.</returns>
        public new GridRowColEntryEnumerator GetEnumerator()
        {
            return new GridRowColEntryEnumerator(this);
        }

        /// <summary>
        ///     Removes a specific item from the 'GridRowColEntryCollection'.
        /// </summary>
        /// <param name='griValue'>
        ///     The item to remove from the 'GridRowColEntryCollection'.
        /// </param>
        public void Remove(GridRowColEntry griValue)
        {
            List.Remove(griValue);
        }

        /// <summary>
        ///     TODO: Describe what custom processing this method does
        ///     before setting an item in the collection
        /// </summary>
        protected override void OnSet(int intIndex, object objOldValue, object objNewValue)
        {
            ////  TODO: Add code here to handle an existing value within
            ////  the collection be replaced with a new value
        }

        /// <summary>
        ///     TODO: Describe what custom processing this method does
        ///     before inserting a new item in the collection
        /// </summary>
        protected override void OnInsert(int intIndex, object objValue)
        {
            ////  TODO: Add code here to handle inserting a new item into the collection
        }

        /// <summary>
        ///     A strongly typed enumerator for 'GridRowColEntryCollection'
        /// </summary>
        public class GridRowColEntryEnumerator : object, System.Collections.IEnumerator
        {
            private System.Collections.IEnumerator iEnBase;

            private System.Collections.IEnumerable iEnLocal;

            /// <summary>
            ///     Enumerator constructor
            /// </summary>
            /// <param name="griMappings">A collection of Row Col entries.</param>
            public GridRowColEntryEnumerator(GridRowColEntryCollection griMappings)
            {
                this.iEnLocal = (System.Collections.IEnumerable)griMappings;
                this.iEnBase = iEnLocal.GetEnumerator();
            }

            /// <summary>
            ///     Gets the current element from the collection (strongly typed)
            /// </summary>
            public GridRowColEntry Current
            {
                get
                {
                    return (GridRowColEntry)iEnBase.Current;
                }
            }

            /// <summary>
            ///     Gets the current element from the collection
            /// </summary>
            object System.Collections.IEnumerator.Current
            {
                get
                {
                    return iEnBase.Current;
                }
            }

            /// <summary>
            ///     Advances the enumerator to the next element of the collection
            /// </summary>
            /// <returns>True if next element exists; False otherwise.</returns>
            public bool MoveNext()
            {
                return iEnBase.MoveNext();
            }

            /// <summary>
            /// Advances the enumerator to the next element of the collection
            /// </summary>
            /// <returns>
            /// true if the enumerator was successfully advanced to the next element; false if the enumerator has passed the end of the collection.
            /// </returns>
            /// <exception cref="T:System.InvalidOperationException">
            /// The collection was modified after the enumerator was created.
            /// </exception>
            bool System.Collections.IEnumerator.MoveNext()
            {
                return iEnBase.MoveNext();
            }

            /// <summary>
            ///     Sets the enumerator to the first element in the collection
            /// </summary>
            public void Reset()
            {
                iEnBase.Reset();
            }

            /// <summary>
            ///     Sets the enumerator to the first element in the collection
            /// </summary>
            void System.Collections.IEnumerator.Reset()
            {
                iEnBase.Reset();
            }
        }
    }

    #endregion //('GridRowColEntryCollection' strongly typed collection class)
}
