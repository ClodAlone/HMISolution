//-------------------------------------------------------------------------------------------------
// <copyright file="GridSummarySection.cs" company="syncfusion">
// Copyright (c) Syncfusion Inc. 2001 - 2014. All rights reserved.
//
//  Use of this code is subject to the terms of our license.
//  A copy of the current license can be obtained at any time by e-mailing
//  licensing@syncfusion.com. Re-distribution in any form is strictly
//  prohibited. Any infringement will be prosecuted under applicable laws. 
// </copyright>
//-------------------------------------------------------------------------------------------------

using System;
using System.Collections;
using System.ComponentModel;
using System.ComponentModel.Design;
using System.Windows.Forms;
using System.Xml.Serialization;

using Syncfusion.Collections;
using Syncfusion.Collections.BinaryTree;
using Syncfusion.ComponentModel;
using Syncfusion.Diagnostics;
using Syncfusion.Grouping;
using Syncfusion.Windows.Forms;

using ISummary = Syncfusion.Collections.BinaryTree.ITreeTableSummary;

#if ASPNET
namespace Syncfusion.Web.UI.WebControls.Grid.Grouping
#else
namespace Syncfusion.Windows.Forms.Grid.Grouping
#endif
{
    /// <summary>
    /// The summary section that contains one or multiple row elements for showing the summary below a group in <see cref="Table.DisplayElements"/>. The summary
    /// information can be determined by calling <see cref="Group.GetSummaries"/> on the <see cref="Group"/> that this summary section
    /// belongs to.
    /// </summary>
    public class GridSummarySection : RowElementsSection, ISummarySection, IGridTableCellAppearanceSource
    {
        GridSummaryRowCollection _summaryRows;
        bool summariesChanged
        {
            get
            {
                return this.Reserved1;
            }

            set
            {
                this.Reserved1 = value;
            }
        }

        bool inAdjustSummaryRowCount
        {
            get
            {
                return this.Reserved2;
            }

            set
            {
                this.Reserved2 = value;
            }
        }

        int lastSummaryRowsCount
        {
            get
            {
                return this.Reserved16a;
            }

            set
            {
                this.Reserved16a = value;
            }
        }

        /// <summary>
        /// Initializes a new section in the specified group.
        /// </summary>
        /// <param name="group">The group this section is created in.</param>
        public GridSummarySection(Group group)
            : base(group)
        {
            summariesChanged = true;
        }
        
        /// <override/>
        protected override void Dispose(bool disposing)
        {
            if (appearance != null)
            {
                this.appearance.Dispose();
            }

            appearance = null;

            _summaryRows = null;

            base.Dispose(disposing);
        }

        /// <override/>
        /// <summary>Gets the kind of display element.</summary>
        public override DisplayElementKind Kind
        {
            get { return DisplayElementKind.Summary; }
        }

        /// <summary>
        /// This overridden Syncfusion.Grouping.Element.OnEnsureInitialized(System.Object)
        /// lets the elements implement element-specific logic to ensure object
        /// is up to data.
        /// </summary>
        /// <param name="sender">The object that triggered the Syncfusion.Grouping.Element.EnsureInitialized(System.Object) call.</param>
        /// <returns>True if changes were detected and the object was updated; False otherwise.</returns>
        /// <override/>
        protected override bool OnEnsureInitialized(object sender)
        {
            AdjustSummaryRowCount();
            return base.OnEnsureInitialized(sender);
        }

        /// <override/>
        /// <summary>Walks down to the child branches and resets the counter.</summary>
        /// <param name="notifyCounterSource">If true, notifies the counter source.</param>
        public override void InvalidateCounterTopDown(bool notifyCounterSource)
        {
            base.InvalidateCounterTopDown(notifyCounterSource);
            summariesChanged = true;
        }

        /// <override/>
        /// <summary>Ensures this object, nested objects and parent elements reflect any changes
        /// made to the engine or table descriptor.</summary>
        /// <param name="sender">The object that triggered the call.</param>
        /// <param name="notifyParent">Specifies if the parent elements Element.EnsureInitialized(Object) should also be called.</param>
        /// <returns>True if they reflect the changes.</returns>
        public override bool EnsureInitialized(object sender, bool notifyParent)
        {
            if (inAdjustSummaryRowCount || IsDisposed)
            {
                return false;
            }

            return base.EnsureInitialized(sender, notifyParent);
        }

        bool AdjustSummaryRowCount()
        {
            inAdjustSummaryRowCount = true;

            if (summariesChanged)
            {
                //// Add summary rows on demand - this is trigger when InvalidateCounterTopDown is called on Table
                //// see GridTable.SummaryRows_Changed: CountersDirty = true;
                GridTable table = (GridTable) this.ParentTable;
                if (table != null)
                {
                    int count = table.TableDescriptor.SummaryRows.Count;
                    if (lastSummaryRowsCount != count)
                    {
                        while (SummaryRows.Count > count)
                        {
                            SummaryRows.Remove(SummaryRows[SummaryRows.Count - 1]);
                        }

                        while (SummaryRows.Count < count)
                        {
                            SummaryRows.Add(new GridSummaryRow(this, SummaryRows.Count));
                        }

                        lastSummaryRowsCount = count;
                        return true;
                    }
                }

                summariesChanged = false;
            }

            inAdjustSummaryRowCount = false;
            return false;
        }

        /// <override/>
        /// <summary>Gets the ITreeTableCounter.</summary>
        /// <returns>returns the ITreeTableCounter.</returns>
        public override ITreeTableCounter GetCounter()
        {
            AdjustSummaryRowCount();
            return base.GetCounter();
        }

        /// <summary>
        /// Indicates if ChildElementTreeTable to be created
        /// </summary>
        /// <returns>returns boolean value true</returns>
        /// <override/>
        protected override bool ShouldCreateChildElementTreeTable()
        {
            return true;
        }

        /// <summary>
        /// The collection of <see cref="GridSummaryRow"/> elements that are contained in this section.
        /// </summary>
        public GridSummaryRowCollection SummaryRows
        {
            get
            {
                if (_summaryRows == null)
                {
                    _summaryRows = new GridSummaryRowCollection(this);
                }

                return _summaryRows;
            }
        }

        /// <override/>
        /// <summary>Indicates if the specified element is the direct child of this element and if it should be made visible.</summary>
        /// <param name="el">The specified element.</param>
        /// <returns>True if it is a child and is visible.</returns>
        public override bool IsChildVisible(Element el)
        {
            GridSummaryRow sr = el as GridSummaryRow;
            if (sr != null)
            {
                GridSummaryRowDescriptorCollection summaryRowDescriptorCollection = ParentTable.TableDescriptor.SummaryRows;
                return sr.rowIndex < summaryRowDescriptorCollection.Count
                    && summaryRowDescriptorCollection[sr.rowIndex].Visible;
            }

            return base.IsChildVisible(el);
        }
        
        /// <override/>
        /// <summary>
        /// Returns a string holding the current object.
        /// </summary>
        /// <returns>String representation of the current object.</returns>
        public override string ToString()
        {
            return base.ToString();
        }

        #region Strong Typed Parent Elements
        /// <override/>
        /// <summary>Returns the table descriptor this element belongs to.</summary>
        [System.Xml.Serialization.XmlIgnore]
        [DesignerSerializationVisibilityAttribute(DesignerSerializationVisibility.Hidden)]
        [Browsable(false)]
        public new GridTableDescriptor ParentTableDescriptor
        {
            get
            {
                return (GridTableDescriptor) base.ParentTableDescriptor;
            }
        }

        /// <override/>
        /// <summary>
        /// Gets the engine this element belongs to.
        /// </summary>
        public new GridEngine Engine
        {
            get
            {
                return (GridEngine) base.Engine;
            }
        }

        /// <override/>
        /// <summary>Gets the <see cref="Table"/> of the engine this element belongs to.</summary>
        public new GridTable EngineTable
        {
            get
            {
                return (GridTable) base.EngineTable;
            }
        }

        /// <override/>
        /// <summary>A reference to the parent record this element belongs to.</summary>
        [System.Xml.Serialization.XmlIgnore]
        [DesignerSerializationVisibilityAttribute(DesignerSerializationVisibility.Hidden)]
        [Browsable(false)]
        public new GridRecord ParentRecord
        {
            get
            {
                return (GridRecord) base.ParentRecord;
            }
        }

        /// <override/>
        /// <summary>A reference to the child table this element belongs.</summary>
        [System.Xml.Serialization.XmlIgnore]
        [DesignerSerializationVisibilityAttribute(DesignerSerializationVisibility.Hidden)]
        [Browsable(false)]
        public new GridChildTable ParentChildTable
        {
            get
            {
                return (GridChildTable) base.ParentChildTable;
            }
        }

        /// <override/>
        /// <summary>A reference to the parent table this section belongs to.</summary>
        [System.Xml.Serialization.XmlIgnore]
        [DesignerSerializationVisibilityAttribute(DesignerSerializationVisibility.Hidden)]
        [Browsable(false)]
        public new GridTable ParentTable
        {
            get
            {
                return (GridTable) base.ParentTable;
            }

            set
            {
                base.ParentTable = value;
            }
        }

        #endregion
        #region Appearance
        GridTableCellAppearance appearance;

        /// <summary>
        /// Returns a <see cref="GridTableCellAppearance"/> of the first parent element with appearance in the hierarchy.
        /// </summary>
        /// <returns>A <see cref="GridTableCellAppearance"/>.</returns>
        public GridTableCellAppearance GetBaseAppearance()
        {
            Element parent = ParentElement;
            while (parent != null)
            {
                if (parent is IGridTableCellAppearanceSource && ((IGridTableCellAppearanceSource)parent).ShouldSerializeAppearance())
                {
                    return ((IGridTableCellAppearanceSource)parent).GetAppearance();
                }

                parent = parent.ParentElement;
            }

            return null;
        }

        /// <summary>
        /// If this element is modified, the Appearance returns this object's Appearance; otherwise it
        /// returns a <see cref="GridTableCellAppearance"/> of the first parent element with appearance
        /// in the hierarchy.
        /// </summary>
        [Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden), XmlIgnore]
        public GridTableCellAppearance ReadOnlyAppearance
        {
            get
            {
                if (appearance == null)
                {
                    return GetBaseAppearance();
                }

                return Appearance;
            }
        }

        /// <summary>
        /// The default <see cref="GridTableCellAppearance"/> with <see cref="GridTableCellStyleInfo"/>
        /// information for cells of this element.
        /// </summary>
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
        public GridTableCellAppearance Appearance
        {
            get
            {
                if (appearance == null)
                {
                    appearance = new GridTableCellAppearance(this);
                }

                return appearance;
            }

            set
            {
                if (value != null)
                {
                    Appearance.InitializeFrom(value);
                }
                else
                {
                    ResetAppearance();
                }
            }
        }

        /// <summary>
        /// Determines whether <see cref="Appearance"/> has been modified
        /// and contents should be serialized at design-time.
        /// </summary>
        /// <returns>True if contents were changed; False otherwise.</returns>
        public bool ShouldSerializeAppearance()
        {
            return appearance != null && appearance.IsModified;
        }

        /// <summary>
        /// Discards any changes for the <see cref="Appearance"/> object.
        /// </summary>
        public void ResetAppearance()
        {
            if (appearance != null && appearance.IsModified)
            {
                appearance.Reset();
            }
        }

        GridTableCellAppearance IGridTableCellAppearanceSource.GetAppearance()
        {
            return Appearance;
        }

        void IGridTableCellAppearanceSource.RaiseAppearanceChanged(GridTableCellStyleInfoChangedEventArgs e)
        {
        }

        void IGridTableCellAppearanceSource.RaiseAppearanceChanging(GridTableCellStyleInfoChangedEventArgs e)
        {
        }
        #endregion
    }

    /// <summary>
    /// A summary row element that is contained in the <see cref="GridSummarySection.SummaryRows"/> collection
    /// of a <see cref="GridSummarySection"/>. A summary row is added to a GridSummarySection for every visible
    /// <see cref="GridSummaryRowDescriptor"/>.
    /// </summary>
    public class GridSummaryRow : RowElement, IGridTableCellAppearanceSource
    {
        internal int rowIndex
        {
            get
            {
                return this.Reserved16a;
            }

            set
            {
                this.Reserved16a = value;
            }
        }

        /// <summary>
        /// Initializes an object with the row index and parent section.
        /// </summary>
        /// <param name="parent">The parent element.</param>
        /// <param name="rowIndex">The row index in the <see cref="GridSummarySection.SummaryRows"/> collection
        /// of the parent <see cref="GridSummarySection"/>.</param>
        public GridSummaryRow(GridSummarySection parent, int rowIndex)
            : base(parent)
        {
            this.rowIndex = rowIndex;
        }

        /// <override/>
        /// <summary>Gets a collection of summaries for the table.</summary>
        /// <param name="parentTable">The parent table.</param>
        /// <param name="summaryChanged">Returns true if changes were detected.</param>
        /// <returns>A collection of summaries.</returns>
        public override ISummary[] GetSummaries(Table parentTable, out bool summaryChanged)
        {
            summaryChanged = false;
            SummaryDescriptorCollection sdc = parentTable.TableDescriptor.Summaries;
            return sdc.CreateSummaries(this);
        }

        /// <override/>
        /// <summary>
        /// Returns a string holding the current object.
        /// </summary>
        /// <returns>String representation of the current object.</returns>
        public override string ToString()
        {
            return SummaryRowDescriptor.ToString();
        }

        /// <override/>
        /// <summary>Gets the kind of display element.</summary>
        public override DisplayElementKind Kind
        {
            get { return DisplayElementKind.Summary; }
        }

        /// <override/>
        /// <summary>Gets the parent this element belongs to.</summary>
        public new GridSummarySection ParentElement
        {
            get
            {
                return (GridSummarySection) base.ParentElement;
            }

            set
            {
                base.ParentElement = value;
            }
        }

        /// <override/>
        /// <summary>Gets the number of elements.</summary>
        /// <returns>returns value 1.</returns>
        public override int GetElementCount()
        {
            return 1;
        }

        /// <summary>
        /// This overridden Syncfusion.Grouping.Element.OnEnsureInitialized(System.Object)
        /// lets the elements implement element-specific logic to ensure object
        /// is up to data.
        /// </summary>
        /// <param name="sender">The object that triggered the Syncfusion.Grouping.Element.EnsureInitialized(System.Object) call.</param>
        /// <returns>True if changes were detected and the object was updated; False otherwise.</returns>
        /// <override/>
        protected override bool OnEnsureInitialized(object sender)
        {
            return base.OnEnsureInitialized(sender);
        }

        /// <override/>
        /// <summary>Gets the number of visible elements.</summary>
        /// <returns>returns the value 1.</returns>
        public override int GetVisibleCount()
        {
            return 1;
        }

        /// <override/>
        /// <summary>Gets the summary row height.</summary>
        /// <returns>Summary row height.</returns>
        public override double GetYAmountCount()
        {
            return ParentTable.DefaultSummaryRowHeight;
        }
        
        /// <summary>
        /// Returns a reference to the <see cref="GridSummaryRowDescriptor"/> this row belongs to.
        /// </summary>
        public GridSummaryRowDescriptor SummaryRowDescriptor
        {
            get
            {
                GridTable table = (GridTable) ParentTable;
                if (table != null)
                {
                    int index = ParentElement.SummaryRows.IndexOf(this);
                    if (index < table.TableDescriptor.SummaryRows.Count)
                    {
                        return table.TableDescriptor.SummaryRows[index];
                    }
                }

                return null;
            }
        }

        /// <summary>
        /// Gets whether this row displays summary columns in grid cells below a certain header column
        /// or if the summary row is one large covered cell that spans the whole row.
        /// </summary>
        public bool IsFillRow
        {
            get
            {
                GridSummaryRowDescriptor sd = SummaryRowDescriptor;
                if (sd != null)
                {
                    return sd.IsFillRow;
                }

                return false;
            }
        }

        #region Strong Typed Parent Elements
        /// <override/>
        /// <summary>Returns the table descriptor this element belongs to.</summary>
        [System.Xml.Serialization.XmlIgnore]
        [DesignerSerializationVisibilityAttribute(DesignerSerializationVisibility.Hidden)]
        [Browsable(false)]
        public new GridTableDescriptor ParentTableDescriptor
        {
            get
            {
                return (GridTableDescriptor) base.ParentTableDescriptor;
            }
        }

        /// <override/>
        /// <summary>
        /// Gets the engine this element belongs to.
        /// </summary>
        public new GridEngine Engine
        {
            get
            {
                return (GridEngine) base.Engine;
            }
        }

        /// <override/>
        /// <summary>Gets the <see cref="Table"/> of the engine this element belongs to.</summary>
        public new GridTable EngineTable
        {
            get
            {
                return (GridTable) base.EngineTable;
            }
        }

        /// <override/>
        /// <summary>A reference to the parent record this element belongs to.</summary>
        [System.Xml.Serialization.XmlIgnore]
        [DesignerSerializationVisibilityAttribute(DesignerSerializationVisibility.Hidden)]
        [Browsable(false)]
        public new GridRecord ParentRecord
        {
            get
            {
                return (GridRecord) base.ParentRecord;
            }
        }

        /// <override/>
        /// <summary>A reference to the child table this element belongs.</summary>
        [System.Xml.Serialization.XmlIgnore]
        [DesignerSerializationVisibilityAttribute(DesignerSerializationVisibility.Hidden)]
        [Browsable(false)]
        public new GridChildTable ParentChildTable
        {
            get
            {
                return (GridChildTable) base.ParentChildTable;
            }
        }

        /// <override/>
        /// <summary>A reference to the parent table this section belongs to.</summary>
        [System.Xml.Serialization.XmlIgnore]
        [DesignerSerializationVisibilityAttribute(DesignerSerializationVisibility.Hidden)]
        [Browsable(false)]
        public new GridTable ParentTable
        {
            get
            {
                return (GridTable) base.ParentTable;
            }

            set
            {
                base.ParentTable = value;
            }
        }

        #endregion
        #region Appearance
        GridTableCellAppearance appearance;

        /// <summary>
        /// Returns a <see cref="GridTableCellAppearance"/> of the first parent element with appearance in the hierarchy.
        /// </summary>
        /// <returns>A <see cref="GridTableCellAppearance"/>.</returns>
        public GridTableCellAppearance GetBaseAppearance()
        {
            Element parent = ParentElement;
            while (parent != null)
            {
                if (parent is IGridTableCellAppearanceSource && ((IGridTableCellAppearanceSource)parent).ShouldSerializeAppearance())
                {
                    return ((IGridTableCellAppearanceSource)parent).GetAppearance();
                }

                parent = parent.ParentElement;
            }

            return null;
        }

        /// <summary>
        /// If this element is modified, the Appearance returns this object's Appearance; otherwise it
        /// returns a <see cref="GridTableCellAppearance"/> of the first parent element with appearance
        /// in the hierarchy.
        /// </summary>
        [Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden), XmlIgnore]
        public GridTableCellAppearance ReadOnlyAppearance
        {
            get
            {
                if (appearance == null)
                {
                    return GetBaseAppearance();
                }

                return Appearance;
            }
        }

        /// <summary>
        /// The default <see cref="GridTableCellAppearance"/> with <see cref="GridTableCellStyleInfo"/>
        /// information for cells of this element.
        /// </summary>
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
        public GridTableCellAppearance Appearance
        {
            get
            {
                if (appearance == null)
                {
                    appearance = new GridTableCellAppearance(this);
                }

                return appearance;
            }

            set
            {
                if (value != null)
                {
                    Appearance.InitializeFrom(value);
                }
                else
                {
                    ResetAppearance();
                }
            }
        }

        /// <summary>
        /// Determines whether <see cref="Appearance"/> has been modified
        /// and contents should be serialized at design-time.
        /// </summary>
        /// <returns>True if contents were changed; False otherwise.</returns>
        public bool ShouldSerializeAppearance()
        {
            return appearance != null && appearance.IsModified;
        }

        /// <summary>
        /// Discards any changes for the <see cref="Appearance"/> object.
        /// </summary>
        public void ResetAppearance()
        {
            if (appearance != null && appearance.IsModified)
            {
                appearance.Reset();
            }
        }

        GridTableCellAppearance IGridTableCellAppearanceSource.GetAppearance()
        {
            return Appearance;
        }

        void IGridTableCellAppearanceSource.RaiseAppearanceChanged(GridTableCellStyleInfoChangedEventArgs e)
        {
        }

        void IGridTableCellAppearanceSource.RaiseAppearanceChanging(GridTableCellStyleInfoChangedEventArgs e)
        {
        }
        #endregion
    }

    #region SummaryRowCollection
    /// <summary>
    /// A collection of <see cref="GridSummaryRow"/> elements that are children of a <see cref="GridSummarySection"/>.
    /// An instance of this collection is returned by the <see cref="GridSummarySection.SummaryRows"/> property
    /// of a <see cref="GridSummarySection"/> object.
    /// </summary>
    public class GridSummaryRowCollection : IList
    {
        internal RowElementCollection _inner;
        internal GridSummarySection _parentSummarySection;

        internal GridSummaryRowCollection(GridSummarySection parentSummarySection)
        {
            _inner = parentSummarySection.RowElements;
            _parentSummarySection = parentSummarySection;
        }

        /// <summary>
        /// Gets / sets the element at the zero-based index.
        /// </summary>
        public GridSummaryRow this[int index]
        {
            get
            {
                return (GridSummaryRow) _inner[index];
            }

            set
            {
                _inner[index] = value;
            }
        }

        /// <summary>
        /// Determines if the element belongs to this collection.
        /// </summary>
        /// <param name="value">The Object to locate in the collection. The value can be a NULL reference (Nothing in Visual Basic).</param>
        /// <returns>True if item is found in the collection; otherwise, False.</returns>
        public bool Contains(GridSummaryRow value)
        {
            return _inner.Contains(value);
        }

        /// <summary>
        /// Returns the zero-based index of the occurrence of the element in the collection.
        /// </summary>
        /// <param name="value">The element to locate in the collection. The value can be a NULL reference (Nothing in Visual Basic). </param>
        /// <returns>The zero-based index of the occurrence of the element within the entire collection, if found; otherwise, -1.</returns>
        public int IndexOf(GridSummaryRow value)
        {
            return _inner.IndexOf(value);
        }

        /// <summary>
        /// Copies the entire collection to a compatible one-dimensional array, starting at the specified index of the target array.
        /// </summary>
        /// <param name="array">The one-dimensional array that is the destination of the elements copied from the ArrayList. The array must have zero-based indexing. </param>
        /// <param name="index">The zero-based index in an array at which copying begins. </param>
        public void CopyTo(GridSummaryRow[] array, int index)
        {
            _inner.CopyTo(array, index);
        }

        ////        public GridSummaryRowCollection SyncRoot
        ////        {
        ////            get
        ////            {
        ////                return null;
        ////            }
        ////        }

        /// <summary>
        /// Returns an enumerator for the entire collection.
        /// </summary>
        /// <returns>An Enumerator for the entire collection.</returns>
        /// <remarks>Enumerators only allow reading of the data in the collection.
        /// Enumerators cannot be used to modify the underlying collection.</remarks>
        public GridSummaryRowCollectionEnumerator GetEnumerator()
        {
            return new GridSummaryRowCollectionEnumerator(this);
        }

        /// <summary>
        /// Inserts an element into the collection at the specified index.
        /// </summary>
        /// <param name="index">The zero-based index at which the element should be inserted.</param>
        /// <param name="value">The element to insert. The value must not be a NULL reference (Nothing in Visual Basic). </param>
        /// <remarks>
        /// The method
        /// ensures that the collection is in sync with the underlying
        /// table if changes have been made to the table or the TableDescriptor.
        /// <para/>
        /// The method calls <see cref="Element"/>.
        /// </remarks>
        public void Insert(int index, GridSummaryRow value)
        {
            _inner.Insert(index, value);
        }

        /// <summary>
        /// Removes the specified element from the collection.
        /// </summary>
        /// <param name="value">The element to remove from the collection. If the value is NULL or the element is not contained
        /// in the collection, the method will do nothing.</param>
        /// <remarks>
        /// The method
        /// ensures that the collection is in sync with the underlying
        /// table if changes have been made to the table or the TableDescriptor.
        /// <para/>
        /// The method calls <see cref="Element"/>
        /// </remarks>
        public void Remove(GridSummaryRow value)
        {
            _inner.Remove(value);
        }

        /// <summary>
        /// Adds a value to the end of the collection.
        /// </summary>
        /// <param name="value">The element to be added to the end of the collection. The value must not be a NULL reference (Nothing in Visual Basic). </param>
        /// <returns>The zero-based collection index at which the value has been added.</returns>
        public int Add(GridSummaryRow value)
        {
            return _inner.Add(value);
        }

        /// <summary>
        /// Removes the element at the specified index of the collection.
        /// </summary>
        /// <param name="index">The zero-based index of the element to remove. </param>
        public void RemoveAt(int index)
        {
            _inner.RemoveAt(index);
        }

        /// <summary>
        /// Removes all elements from the collection.
        /// </summary>
        public void Clear()
        {
            _inner.Clear();
        }

        /// <summary>
        /// Returns False.
        /// </summary>
        public bool IsReadOnly
        {
            get
            {
                return false;
            }
        }

        /// <summary>
        /// Returns False since this collection has no fixed size.
        /// </summary>
        public bool IsFixedSize
        {
            get
            {
                return false;
            }
        }

        /// <summary>
        /// Returns False.
        /// </summary>
        public bool IsSynchronized
        {
            get
            {
                return false;
            }
        }

        /// <summary>
        /// Gets the number of elements contained in the collection. The property also
        /// ensures that the collection is in sync with the underlying
        /// table if changes have been made to the table or the TableDescriptor.
        /// </summary>
        /// <remarks>
        /// If changes in the TableDescriptor are detected, the
        /// method will reinitialize the collection before returning the count.
        /// </remarks>
        public int Count
        {
            get
            {
                return _inner.Count;
            }
        }

        #region IList Members

        object IList.this[int index]
        {
            get
            {
                return this[index];
            }

            set
            {
                this[index] = (GridSummaryRow) value;
            }
        }

        void IList.Insert(int index, object value)
        {
            Insert(index, (GridSummaryRow) value);
        }

        void IList.Remove(object value)
        {
            Remove((GridSummaryRow) value);
        }

        bool IList.Contains(object value)
        {
            return Contains((GridSummaryRow) value);
        }

        int IList.IndexOf(object value)
        {
            return IndexOf((GridSummaryRow) value);
        }

        int IList.Add(object value)
        {
            return Add((GridSummaryRow) value);
        }

        #endregion

        #region ICollection Members

        void ICollection.CopyTo(Array array, int index)
        {
            CopyTo((GridSummaryRow[]) array, index);
        }

        object ICollection.SyncRoot
        {
            get
            {
                return null;
            }
        }

        #endregion

        #region IEnumerable Members

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        #endregion
    }

    /// <summary>
    /// Enumerator class for <see cref="GridSummaryRow"/> elements of a <see cref="GridSummaryRowCollection"/>.
    /// </summary>
    public class GridSummaryRowCollectionEnumerator : IEnumerator
    {
        RowElementCollectionEnumerator inner;

        /// <summary>
        /// Initalizes the enumerator and attaches it to the collection.
        /// </summary>
        /// <param name="collection">The parent collection to enumerate.</param>
        public GridSummaryRowCollectionEnumerator(GridSummaryRowCollection collection)
        {
            inner = new RowElementCollectionEnumerator(collection);
        }

        #region IEnumerator Members

        /// <summary>
        /// Sets the enumerator to its initial position, which is before the first element in the collection.
        /// </summary>
        public virtual void Reset()
        {
            inner.Reset();
        }

        object IEnumerator.Current
        {
            get
            {
                return inner.Current;
            }
        }

        /// <summary>
        /// Gets the current element in the collection.
        /// </summary>
        public GridSummaryRow Current
        {
            get
            {
                return (GridSummaryRow) inner.Current;
            }
        }

        /// <summary>
        /// Advances the enumerator to the next element of the collection.
        /// </summary>
        /// <returns>
        /// True if the enumerator was successfully advanced to the next element; False if the enumerator has passed the end of the collection.
        /// </returns>
        public bool MoveNext()
        {
            return inner.MoveNext();
        }
        #endregion
    }
    #endregion
}
