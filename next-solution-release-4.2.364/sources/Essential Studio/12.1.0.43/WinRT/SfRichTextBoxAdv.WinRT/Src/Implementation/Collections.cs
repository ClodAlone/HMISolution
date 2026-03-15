#region Copyright Syncfusion Inc. 2001 - 2014
// Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Any infringement will be prosecuted under
// applicable laws. 
#endregion
using System;
using System.Collections.ObjectModel;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
#if WPF
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
#else
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
#endif

#if WPF
namespace Syncfusion.Windows.Tools.RichTextBoxAdv
#else
namespace Syncfusion.UI.Xaml.RichTextBoxAdv
#endif
{
    public abstract class NodeCollection : ObservableCollection<Node>
    {
        #region Fields
        private Node ownerNode;
        #endregion

        #region Properties
        /// <summary>
        /// Gets the owner node.
        /// </summary>
        /// <value>
        /// The owner node.
        /// </value>
        internal Node OwnerNode
        {
            get
            {
                return ownerNode;
            }
        }
        #endregion

        #region Constructor
        /// <summary>
        /// Initializes a new instance of the <see cref="NodeCollection"/> class.
        /// </summary>
        /// <param name="owner">The owner.</param>
        public NodeCollection(Node owner)
        {
            ownerNode = owner;
            CollectionChanged += NodeCollection_CollectionChanged;
        }
        /// <summary>
        /// Initializes a new instance of the <see cref="NodeCollection"/> class.
        /// </summary>
        /// <param name="owner">The owner.</param>
        /// <param name="collection">The collection.</param>
        public NodeCollection(Node owner, IEnumerable<Node> collection)
            : base(collection)
        {
            ownerNode = owner;
            CollectionChanged += NodeCollection_CollectionChanged;
        }
        #endregion

        #region Abstract Methods
        /// <summary>
        /// Handles the CollectionChanged event of the NodeCollection control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="NotifyCollectionChangedEventArgs"/> instance containing the event data.</param>
        internal abstract void NodeCollection_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e);
        #endregion

        #region Implementations
        /// <summary>
        /// Removes the node from previous parent.
        /// </summary>
        /// <param name="node">The node.</param>
        /// <param name="index">The index.</param>
        internal void SetOwnerToNode(Node node, int index)
        {
            if (node.Owner is CompositeNode)
            {
                CompositeNode previousOwner = node.Owner as CompositeNode;
                if (Contains(node))
                {
                    //Clears the event handling to remove item from collection internally, to completely remove item's link from its previous owner.
                    CollectionChanged -= NodeCollection_CollectionChanged;
                    //Removes the node at the specified index temporarily.
                    RemoveAt(index);
                    //If item is within previous owner, clear from previous owner.
                    if (previousOwner == OwnerNode)
                        CollectionChanged += NodeCollection_CollectionChanged;
                    previousOwner.ChildNodes.Remove(node);
                    if (previousOwner == OwnerNode)
                        CollectionChanged -= NodeCollection_CollectionChanged;
                    //Inserts the temporarily removed node back at the specified index.
                    Insert(index, node);
                    //Adds the collection changed event handler, after above internal manipulation.
                    CollectionChanged += NodeCollection_CollectionChanged;
                }
                else
                    previousOwner.ChildNodes.Remove(node);
            }
            //Sets the owner this instance as node's owner.
            node.SetOwner(OwnerNode);
        }
        /// <summary>
        /// Gets the next node.
        /// </summary>
        /// <param name="node">The node.</param>
        /// <returns></returns>
        internal Node GetNextNode(Node node)
        {
            int index = IndexOf(node);

            if (index < 0 || index > Count - 2)
            {
                return null;
            }

            return this[index + 1];
        }
        /// <summary>
        /// Gets the previous node.
        /// </summary>
        /// <param name="node">The node.</param>
        /// <returns></returns>
        internal Node GetPreviousNode(Node node)
        {
            int index = IndexOf(node);

            if (index < 1 || index > Count - 1)
            {
                return null;
            }

            return this[index - 1];
        }
        #endregion
    }
    internal class HeaderFooterCollection : NodeCollection
    { 
        #region Constructor
        /// <summary>
        /// Initializes a new instance of the <see cref="HeaderFooterCollection"/> class.
        /// </summary>
        /// <param name="owner">The owner.</param>
        public HeaderFooterCollection(Node owner)
            : base(owner)
        {
        }
        /// <summary>
        /// Initializes a new instance of the <see cref="HeaderFooterCollection"/> class.
        /// </summary>
        /// <param name="owner">The owner.</param>
        /// <param name="collection">The collection.</param>
        public HeaderFooterCollection(Node owner, IEnumerable<Node> collection)
            : base(owner, collection)
        {
        }
        #endregion

        #region Override methods
        /// <summary>
        /// Handles the CollectionChanged event of the NodeCollection control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="NotifyCollectionChangedEventArgs" /> instance containing the event data.</param>
        internal override void NodeCollection_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            if (e.OldItems != null)
            {
                foreach (var item in e.OldItems)
                {
                    if (item is Node)
                        (item as Node).SetOwner(null);
                }
            }
            if (e.NewItems != null)
            {
                int index = e.NewStartingIndex;
                foreach (var item in e.NewItems)
                {
                    if (item is Node)
                        SetOwnerToNode(item as Node, index);
                    index++;
                }
            }
        }
        #endregion
    }
    public class SectionAdvCollection : NodeCollection
    {
        #region Properties
        /// <summary>
        /// Gets or sets the element at the specified index.
        /// </summary>
        /// <param name="index">The index.</param>
        /// <returns></returns>
        new public SectionAdv this[int index]
        {
            get
            {
                return base[index] as SectionAdv;
            }
        }
        #endregion

        #region Constructor
        /// <summary>
        /// Initializes a new instance of the <see cref="SectionAdvCollection"/> class.
        /// </summary>
        /// <param name="owner">The owner.</param>
        public SectionAdvCollection(Node owner)
            : base(owner)
        {
        }
        /// <summary>
        /// Initializes a new instance of the <see cref="SectionAdvCollection"/> class.
        /// </summary>
        /// <param name="owner">The owner.</param>
        /// <param name="collection">The collection.</param>
        public SectionAdvCollection(Node owner, IEnumerable<SectionAdv> collection)
            : base(owner, (IEnumerable<Node>)collection)
        {
        }
        #endregion

        #region Override methods
        /// <summary>
        /// Handles the CollectionChanged event of the NodeCollection control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="NotifyCollectionChangedEventArgs" /> instance containing the event data.</param>
        internal override void NodeCollection_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            if (e.OldItems != null)
            {
                foreach (var item in e.OldItems)
                {
                    if (item is Node)
                        //Sets the owner as null.
                        (item as Node).SetOwner(null);
                    if (item is SectionAdv)
                        //Clears the layouted widgets of section.
                        (item as SectionAdv).ClearWidgets();
                }
                //Relayouts from the removed section index.
                if (e.OldStartingIndex < Count)
                    Layout(e.OldStartingIndex);
            }
            if (e.NewItems != null)
            {
                int index = e.NewStartingIndex;
                foreach (var item in e.NewItems)
                {
                    if (item is Node)
                        SetOwnerToNode(item as Node, index);
                    index++;
                }
                //Relayouts from the inserted section index.
                Layout(e.NewStartingIndex);
            }
        }
        #endregion

        #region Implementations
        /// <summary>
        /// Layouts the specified start index.
        /// </summary>
        /// <param name="startIndex">The start index.</param>
        private void Layout(int startIndex)
        {
            SfRichTextBoxAdv rte = null;
            if (OwnerNode is DocumentAdv && (rte = (OwnerNode as DocumentAdv).OwnerControl) != null
                && rte.IsLayoutEnabled && startIndex < Count)
                (this[startIndex] as SectionAdv).LayoutItems();
        }
        public SectionAdv First()
        {
            return (this as ObservableCollection<Node>).First() as SectionAdv;
        }
        public SectionAdv First(Func<Node, bool> predicate)
        {
            return (this as ObservableCollection<Node>).First(predicate) as SectionAdv;
        }
        public SectionAdv Last()
        {
            return (this as ObservableCollection<Node>).Last() as SectionAdv;
        }
        public SectionAdv Last(Func<Node, bool> predicate)
        {
            return (this as ObservableCollection<Node>).Last(predicate) as SectionAdv;
        }
        /// <summary>
        /// Adds the specified SectionAdv.
        /// </summary>
        /// <param name="sectionAdv">The section adv.</param>
        public new void Add(Node sectionAdv)
        {
            Insert(Count, sectionAdv);
        }
        /// <summary>
        /// Inserts the SectionAdv at specified index.
        /// </summary>
        /// <param name="index">The index.</param>
        /// <param name="sectionAdv">The section adv.</param>
        /// <exception cref="System.ArgumentOutOfRangeException">Index is less than zero.-or-Index is greater than item count.</exception>
        public new void Insert(int index, Node sectionAdv)
        {
            if (index < 0 || index > Count)
                throw new ArgumentOutOfRangeException("Index is less than zero.-or-Index is greater than item count.");
            base.Insert(index, sectionAdv);
        }
        /// <summary>
        /// Removes the SectionAdv at the specified index of the SectionAdvCollection.
        /// </summary>
        /// <param name="index">The index.</param>
        public new void RemoveAt(int index)
        {
            if (index < 0 || index >= Count)
                throw new ArgumentOutOfRangeException("Index is less than zero.-or-Index is equal to or greater than item count.");
            Remove(this[index]);
        }
        /// <summary>
        /// Removes the specified SectionAdv.
        /// </summary>
        /// <param name="sectionAdv">The SectionAdv.</param>
        /// <returns></returns>
        public new bool Remove(Node sectionAdv)
        {
            return base.Remove(sectionAdv);
        }
        #endregion
    }
    public class BlockAdvCollection : NodeCollection
    {
        #region Properties
        /// <summary>
        /// Gets or sets the element at the specified index.
        /// </summary>
        /// <param name="index">The index.</param>
        /// <returns></returns>
        new public BlockAdv this[int index]
        {
            get
            {
                return base[index] as BlockAdv;
            }
        }
        #endregion

        #region Constructor
        /// <summary>
        /// Initializes a new instance of the <see cref="BlockAdvCollection"/> class.
        /// </summary>
        /// <param name="owner">The owner.</param>
        public BlockAdvCollection(Node owner)
            : base(owner)
        {
        }
        /// <summary>
        /// Initializes a new instance of the <see cref="BlockAdvCollection"/> class.
        /// </summary>
        /// <param name="owner">The owner.</param>
        /// <param name="collection">The collection.</param>
        public BlockAdvCollection(Node owner, IEnumerable<BlockAdv> collection)
            : base(owner, (IEnumerable<Node>)collection)
        {
        }
        #endregion

        #region Override methods
        /// <summary>
        /// Handles the CollectionChanged event of the NodeCollection control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="NotifyCollectionChangedEventArgs" /> instance containing the event data.</param>
        internal override void NodeCollection_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            if (e.OldItems != null)
            {
                foreach (var item in e.OldItems)
                {
                    if (item is Node)
                        //Sets the owner as null.
                        (item as Node).SetOwner(null);
                    if (item is BlockAdv)
                        //Clears the layouted widgets of block.
                        (item as BlockAdv).ClearWidgets();
                }
                //Relayouts from the removed block index.
                if (e.OldStartingIndex < Count)
                    Layout(e.OldStartingIndex);
            }
            if (e.NewItems != null)
            {
                int index = e.NewStartingIndex;
                foreach (var item in e.NewItems)
                {
                    if (item is Node)
                        SetOwnerToNode(item as Node, index);
                    index++;
                }
                //Relayouts from the inserted block index.
                Layout(e.NewStartingIndex);
            }
        }
        #endregion

        #region Implementations
        /// <summary>
        /// Layouts the specified start index.
        /// </summary>
        /// <param name="startIndex">The start index.</param>
        internal void Layout(int startIndex)
        {
            if (startIndex >= Count)
                return;
            SectionAdv section = OwnerNode as SectionAdv;
            if (OwnerNode is SectionAdv)
            {
                if (section.BaseParent != null
                    && section.BaseParent.IsLayoutEnabled)
                    section.Layout(startIndex);
            }
            else if (OwnerNode is TableCellAdv)
            {
                TableCellAdv cell = OwnerNode as TableCellAdv;
                cell = cell.GetContainerCell();
                section = cell.Section;
                if (section != null && section.BaseParent != null)
                {
                    DocumentAdv ownerDocument = section.Document;
                    LayoutViewer viewer = ownerDocument.OwnerControl.Viewer;
                    if (viewer.FieldEndParagraph != null && viewer.FieldEndParagraph.BaseParent == null)
                        //If field end mark or its entire owner is removed, sets the current paragraph as relayout end.
                        viewer.FieldEndParagraph = cell.GetFirstParagraph();
                    if (viewer.FieldToLayout != null)
                    {
                        ParagraphAdv ownerParagraph = viewer.FieldToLayout.OwnerParagraph;
                        Inline fieldBegin = viewer.FieldToLayout;
                        viewer.FieldToLayout = null;
                        if (ownerParagraph != null && ownerParagraph.BaseParent != null)
                        {
                            //If field separator or end mark or its entire owner is removed, relayouts from the field begin.
                            ownerParagraph.Relayout(fieldBegin.GetIndexInOwnerCollection());
                            return;
                        }
                    }
                    if (ownerDocument.OwnerControl.IsLayoutEnabled)
                        cell.OwnerRow.Layout(viewer);
                }
            }
        }
        public BlockAdv First()
        {
            return (this as ObservableCollection<Node>).First() as BlockAdv;
        }
        public BlockAdv First(Func<Node, bool> predicate)
        {
            return (this as ObservableCollection<Node>).First(predicate) as BlockAdv;
        }
        public BlockAdv Last()
        {
            return (this as ObservableCollection<Node>).Last() as BlockAdv;
        }
        public BlockAdv Last(Func<Node, bool> predicate)
        {
            return (this as ObservableCollection<Node>).Last(predicate) as BlockAdv;
        }
        public new void Add(Node blockAdv)
        {
            Insert(Count, blockAdv);
        }
        public new void Insert(int index, Node blockAdv)
        {
            if (index < 0 || index > Count)
                throw new ArgumentOutOfRangeException("Index is less than zero.-or-Index is greater than item count.");
            base.Insert(index, blockAdv);
        }
        /// <summary>
        /// Removes the BlockAdv at the specified index of the BlockAdvCollection.
        /// </summary>
        /// <param name="index">The index.</param>
        public new void RemoveAt(int index)
        {
            if (index < 0 || index >= Count)
                throw new ArgumentOutOfRangeException("Index is less than zero.-or-Index is equal to or greater than item count.");
            Remove(this[index]);
        }
        public new bool Remove(Node blockAdv)
        {
            return base.Remove(blockAdv);
        }
        #endregion
    }
    public class InlineCollection : NodeCollection
    {
        #region Properties
        /// <summary>
        /// Gets or sets the element at the specified index.
        /// </summary>
        /// <param name="index">The index.</param>
        /// <returns></returns>
        new public Inline this[int index]
        {
            get
            {
                return base[index] as Inline;
            }
        }
        #endregion

        #region Constructor
        /// <summary>
        /// Initializes a new instance of the <see cref="InlineCollection"/> class.
        /// </summary>
        /// <param name="owner">The owner.</param>
        public InlineCollection(Node owner)
            : base(owner)
        {
        }
        /// <summary>
        /// Initializes a new instance of the <see cref="InlineCollection"/> class.
        /// </summary>
        /// <param name="owner">The owner.</param>
        /// <param name="collection">The collection.</param>
        public InlineCollection(Node owner, IEnumerable<Inline> collection)
            : base(owner, (IEnumerable<Node>)collection)
        {
        }
        #endregion

        #region Override methods
        /// <summary>
        /// Handles the CollectionChanged event of the NodeCollection control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="NotifyCollectionChangedEventArgs" /> instance containing the event data.</param>
        internal override void NodeCollection_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            if (e.OldItems != null)
            {
                foreach (var item in e.OldItems)
                {
                    if (item is Node)
                        //Sets the owner as null.
                        (item as Node).SetOwner(null);
                }
                //Relayouts the paragraph from the item previous of removed inline, till the paragraph ends.
                if (e.OldStartingIndex - 1 < Count)
                    Layout(e.OldStartingIndex - 1);
            }
            if (e.NewItems != null)
            {
                int index = e.NewStartingIndex;
                foreach (var item in e.NewItems)
                {
                    if (item is Node)
                        SetOwnerToNode(item as Node, index);
                    index++;
                }
                //Relayouts the paragraph from the inserted inline. 
                Layout(e.NewStartingIndex);
            }
        }
        #endregion

        #region Implementations
        /// <summary>
        /// Layouts the specified start index.
        /// </summary>
        /// <param name="startIndex">The start index.</param>
        internal void Layout(int startIndex)
        {
            if (OwnerNode is ParagraphAdv && (OwnerNode as ParagraphAdv).BaseParent != null)
                (OwnerNode as ParagraphAdv).Relayout(startIndex);
        }
        /// <summary>
        /// Adds the specified inline.
        /// </summary>
        /// <param name="inline">The inline.</param>
        public new void Add(Node inline)
        {
            Insert(Count, inline);
        }
        /// <summary>
        /// Inserts the specified index.
        /// </summary>
        /// <param name="index">The index.</param>
        /// <param name="inline">The inline.</param>
        /// <exception cref="System.ArgumentOutOfRangeException">Index is less than zero.-or-Index is greater than item count.</exception>
        public new void Insert(int index, Node inline)
        {
            if (index < 0 || index > Count)
                throw new ArgumentOutOfRangeException("Index is less than zero.-or-Index is greater than item count.");
            base.Insert(index, inline);
        }
        /// <summary>
        /// Removes the inline at the specified index of the InlineCollection.
        /// </summary>
        /// <param name="index">The zero-based index of the element to remove.</param>
        /// <exception cref="System.ArgumentOutOfRangeException">Index is less than zero.-or-Index is equal to or greater than item count.</exception>
        public new void RemoveAt(int index)
        {
            if (index < 0 || index >= Count)
                throw new ArgumentOutOfRangeException("Index is less than zero.-or-Index is equal to or greater than item count.");
            Remove(this[index]);
        }
        /// <summary>
        /// Removes the specified inline.
        /// </summary>
        /// <param name="inline">The inline.</param>
        /// <returns></returns>
        public new bool Remove(Node inline)
        {
            return base.Remove(inline);
        }
        public Inline First()
        {
            return (this as ObservableCollection<Node>).First() as Inline;
        }
        public Inline First(Func<Node, bool> predicate)
        {
            return (this as ObservableCollection<Node>).First(predicate) as Inline;
        }
        public Inline Last()
        {
            return (this as ObservableCollection<Node>).Last() as Inline;
        }
        public Inline Last(Func<Node, bool> predicate)
        {
            return (this as ObservableCollection<Node>).Last(predicate) as Inline;
        }
        #endregion
    }
    public class TableRowAdvCollection : NodeCollection
    {
        #region Properties
        /// <summary>
        /// Gets or sets the element at the specified index.
        /// </summary>
        /// <param name="index">The index.</param>
        /// <returns></returns>
        new public TableRowAdv this[int index]
        {
            get
            {
                return base[index] as TableRowAdv;
            }
        }
        #endregion

        #region Constructor
        /// <summary>
        /// Initializes a new instance of the <see cref="TableRowAdvCollection"/> class.
        /// </summary>
        /// <param name="owner">The owner.</param>
        public TableRowAdvCollection(Node owner)
            : base(owner)
        {
        }
        /// <summary>
        /// Initializes a new instance of the <see cref="TableRowAdvCollection"/> class.
        /// </summary>
        /// <param name="owner">The owner.</param>
        /// <param name="collection">The collection.</param>
        public TableRowAdvCollection(Node owner, IEnumerable<TableRowAdv> collection)
            : base(owner, (IEnumerable<Node>)collection)
        {
        }
        #endregion

        #region Override methods
        /// <summary>
        /// Handles the CollectionChanged event of the NodeCollection control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="NotifyCollectionChangedEventArgs" /> instance containing the event data.</param>
        internal override void NodeCollection_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            if (e.OldItems != null)
            {
                foreach (var item in e.OldItems)
                {
                    if (item is Node)
                        //Sets the owner as null.
                        (item as Node).SetOwner(null);
                }
                //Relayouts the entire table.
                Layout();
            }
            if (e.NewItems != null)
            {
                int index = e.NewStartingIndex;
                foreach (var item in e.NewItems)
                {
                    if (item is Node)
                        SetOwnerToNode(item as Node, index);
                    index++;
                }
                //Relayouts the entire table.
                Layout();
            }
        }
        #endregion

        #region Implementations
        /// <summary>
        /// Layouts this instance.
        /// </summary>
        internal void Layout()
        {
            if (OwnerNode is TableAdv)
            {
                TableAdv table = OwnerNode as TableAdv;
                while (table.IsInsideTable)
                {
                    table = table.AssociatedCell.OwnerTable;
                }
                if (table != null)
                {
                    SectionAdv section = table.Section;
                    if (section != null)
                    {
                        DocumentAdv ownerDocument = section.Document;
                        LayoutViewer viewer = ownerDocument.OwnerControl.Viewer;
                        int tableIndex = section.Blocks.IndexOf(table);
                        if (tableIndex >= 0 && section.BaseParent != null
                            && section.BaseParent.IsLayoutEnabled)
                            section.Layout(tableIndex);
                    }
                }
            }
        }
        /// <summary>
        /// Adds the specified tableRow.
        /// </summary>
        /// <param name="tableRow">The tableRow.</param>
        public new void Add(Node tableRow)
        {
            Insert(Count, tableRow);
        }
        /// <summary>
        /// Inserts the specified index.
        /// </summary>
        /// <param name="index">The index.</param>
        /// <param name="tableRow">The tableRow.</param>
        /// <exception cref="System.ArgumentOutOfRangeException">Index is less than zero.-or-Index is greater than item count.</exception>
        public new void Insert(int index, Node tableRow)
        {
            if (index < 0 || index > Count)
                throw new ArgumentOutOfRangeException("Index is less than zero.-or-Index is greater than item count.");
            base.Insert(index, tableRow);
        }
        /// <summary>
        /// Removes the tableRow at the specified index of the TableRowAdvCollection.
        /// </summary>
        /// <param name="index">The zero-based index of the element to remove.</param>
        /// <exception cref="System.ArgumentOutOfRangeException">Index is less than zero.-or-Index is equal to or greater than item count.</exception>
        public new void RemoveAt(int index)
        {
            if (index < 0 || index >= Count)
                throw new ArgumentOutOfRangeException("Index is less than zero.-or-Index is equal to or greater than item count.");
            Remove(this[index]);
        }
        /// <summary>
        /// Removes the specified tableRow.
        /// </summary>
        /// <param name="tableRow">The tableRow.</param>
        /// <returns></returns>
        public new bool Remove(Node tableRow)
        {
            return base.Remove(tableRow);
        }
        public TableRowAdv First()
        {
            return (this as ObservableCollection<Node>).First() as TableRowAdv;
        }
        public TableRowAdv First(Func<Node, bool> predicate)
        {
            return (this as ObservableCollection<Node>).First(predicate) as TableRowAdv;
        }
        public TableRowAdv Last()
        {
            return (this as ObservableCollection<Node>).Last() as TableRowAdv;
        }
        public TableRowAdv Last(Func<Node, bool> predicate)
        {
            return (this as ObservableCollection<Node>).Last(predicate) as TableRowAdv;
        }
        #endregion
    }
    public class TableCellAdvCollection : NodeCollection
    {
        #region Properties
        /// <summary>
        /// Gets or sets the element at the specified index.
        /// </summary>
        /// <param name="index">The index.</param>
        /// <returns></returns>
        new public TableCellAdv this[int index]
        {
            get
            {
                return base[index] as TableCellAdv;
            }
        }
        #endregion

        #region Constructor
        /// <summary>
        /// Initializes a new instance of the <see cref="TableCellAdvCollection"/> class.
        /// </summary>
        /// <param name="owner">The owner.</param>
        public TableCellAdvCollection(Node owner)
            : base(owner)
        {
        }
        /// <summary>
        /// Initializes a new instance of the <see cref="TableCellAdvCollection"/> class.
        /// </summary>
        /// <param name="owner">The owner.</param>
        /// <param name="collection">The collection.</param>
        public TableCellAdvCollection(Node owner, IEnumerable<TableCellAdv> collection)
            : base(owner, (IEnumerable<Node>)collection)
        {
        }
        #endregion

        #region Override methods
        /// <summary>
        /// Handles the CollectionChanged event of the NodeCollection control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="NotifyCollectionChangedEventArgs" /> instance containing the event data.</param>
        internal override void NodeCollection_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            if (e.OldItems != null)
            {
                foreach (var item in e.OldItems)
                {
                    if (item is Node)
                        //Sets the owner as null.
                        (item as Node).SetOwner(null);
                }
                //Relayouts the entire table.
                Layout();
            }
            if (e.NewItems != null)
            {
                int index = e.NewStartingIndex;
                foreach (var item in e.NewItems)
                {
                    if (item is Node)
                        SetOwnerToNode(item as Node, index);
                    index++;
                }
                //Relayouts the entire table.
                Layout();
            }
        }
        #endregion

        #region Implementations
        /// <summary>
        /// Layouts this instance.
        /// </summary>
        internal void Layout()
        {
            if (OwnerNode is TableRowAdv && (OwnerNode as TableRowAdv).OwnerTable != null)
            {
                TableAdv table = (OwnerNode as TableRowAdv).OwnerTable;
                while (table.IsInsideTable)
                {
                    table = table.AssociatedCell.OwnerTable;
                }
                if (table != null)
                {
                    SectionAdv section = table.Section;
                    if (section != null)
                    {
                        DocumentAdv ownerDocument = section.Document;
                        LayoutViewer viewer = ownerDocument.OwnerControl.Viewer;
                        int tableIndex = section.Blocks.IndexOf(table);
                        if (tableIndex >= 0 && section.BaseParent != null
                            && section.BaseParent.IsLayoutEnabled)
                            section.Layout(tableIndex);
                    }
                }
            }
        }
        /// <summary>
        /// Adds the specified table cell.
        /// </summary>
        /// <param name="tableCell">The tableCell.</param>
        public new void Add(Node tableCell)
        {
            Insert(Count, tableCell);
        }
        /// <summary>
        /// Inserts the specified index.
        /// </summary>
        /// <param name="index">The index.</param>
        /// <param name="tableCell">The tableCell.</param>
        /// <exception cref="System.ArgumentOutOfRangeException">Index is less than zero.-or-Index is greater than item count.</exception>
        public new void Insert(int index, Node tableCell)
        {
            if (index < 0 || index > Count)
                throw new ArgumentOutOfRangeException("Index is less than zero.-or-Index is greater than item count.");
            base.Insert(index, tableCell);
        }
        /// <summary>
        /// Removes the tableCell at the specified index of the TableCellAdvCollection.
        /// </summary>
        /// <param name="index">The zero-based index of the element to remove.</param>
        /// <exception cref="System.ArgumentOutOfRangeException">Index is less than zero.-or-Index is equal to or greater than item count.</exception>
        public new void RemoveAt(int index)
        {
            if (index < 0 || index >= Count)
                throw new ArgumentOutOfRangeException("Index is less than zero.-or-Index is equal to or greater than item count.");
            Remove(this[index]);
        }
        /// <summary>
        /// Removes the specified tableCell.
        /// </summary>
        /// <param name="tableCell">The tableCell.</param>
        /// <returns></returns>
        public new bool Remove(Node tableCell)
        {
            return base.Remove(tableCell);
        }
        public TableCellAdv First()
        {
            return (this as ObservableCollection<Node>).First() as TableCellAdv;
        }
        public TableCellAdv First(Func<Node, bool> predicate)
        {
            return (this as ObservableCollection<Node>).First(predicate) as TableCellAdv;
        }
        public TableCellAdv Last()
        {
            return (this as ObservableCollection<Node>).Last() as TableCellAdv;
        }
        public TableCellAdv Last(Func<Node, bool> predicate)
        {
            return (this as ObservableCollection<Node>).Last(predicate) as TableCellAdv;
        }
        #endregion
    }

    #region UnusedCollection
    internal class ParagraphAdvCollection : NodeCollection
    {
        public ParagraphAdvCollection(Node owner)
            : base(owner)
        {
        }
        public ParagraphAdvCollection(Node owner, IEnumerable<ParagraphAdv> collection)
            : base(owner, (IEnumerable<Node>)collection)
        {
        }
        internal override void NodeCollection_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
        }
    }
    internal class TableAdvCollection : NodeCollection
    {
        public TableAdvCollection(Node owner)
            : base(owner)
        {
        }
        public TableAdvCollection(Node owner, IEnumerable<TableAdv> collection)
            : base(owner, (IEnumerable<Node>)collection)
        {
        }
        internal override void NodeCollection_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
        }
    }
    #endregion

    #region Element collection
    internal class ElementCollection
    {
        #region Fields
        List<ElementBox> elements = null;
        private double maxBaseLineOffset;
        private double maxTextElementHeight = double.NaN;
        private double maxTextElementBaselineOffset;
        internal bool SkipClipImage = false;
        #endregion

        #region Properties
        /// <summary>
        /// Gets the <see cref="ElementBox"/> at the specified index.
        /// </summary>
        /// <value>
        /// The <see cref="ElementBox"/>.
        /// </value>
        /// <param name="index">The index.</param>
        /// <returns></returns>
        internal ElementBox this[int index]
        {
            get
            {
                if (elements.Count > 0)
                    return elements[index];
                return null;
            }
        }
        /// <summary>
        /// Gets the count.
        /// </summary>
        /// <value>
        /// The count.
        /// </value>
        internal int Count
        {
            get
            {
                return elements.Count;
            }
        }
        /// <summary>
        /// Gets the height of the max text element.
        /// </summary>
        /// <value>
        /// The height of the max text element.
        /// </value>
        internal double MaxTextElementHeight
        {
            get
            {
                return maxTextElementHeight;
            }
        }
        /// <summary>
        /// Gets the max text element baseline offset.
        /// </summary>
        /// <value>
        /// The max text element baseline offset.
        /// </value>
        internal double MaxTextElementBaselineOffset
        {
            get
            {
                return maxTextElementBaselineOffset;
            }
        }
        /// <summary>
        /// Gets the max baseline offset.
        /// </summary>
        /// <value>
        /// The max baseline offset.
        /// </value>
        internal double MaxBaselineOffset
        {
            get
            {
                return maxBaseLineOffset;
            }
        }
        #endregion

        #region Constructor
        /// <summary>
        /// Initializes a new instance of the <see cref="ElementCollection"/> class.
        /// </summary>
        internal ElementCollection()
        {
            elements = new List<ElementBox>();
        }
        #endregion

        #region Implementations
        /// <summary>
        /// Adds the specified element.
        /// </summary>
        /// <param name="element">The element.</param>
        internal void Add(ElementBox element)
        {
            elements.Add(element);
            UpdateMaxElementHeight(element);
        }
        /// <summary>
        /// Inserts the element at specified index.
        /// </summary>
        /// <param name="index">The index.</param>
        /// <param name="element">The element.</param>
        internal void Insert(int index, ElementBox element)
        {
            elements.Insert(index, element);
            UpdateMaxElementHeight(element);
        }
        /// <summary>
        /// Removes at.
        /// </summary>
        /// <param name="index">The index.</param>
        internal void RemoveAt(int index)
        {
            elements.RemoveAt(index);
        }
        /// <summary>
        /// Determines whether contains the specified element.
        /// </summary>
        /// <param name="element">The element.</param>
        /// <returns>
        ///   <c>true</c> if contains the specified element; otherwise, <c>false</c>.
        /// </returns>
        internal bool Contains(ElementBox element)
        {
            return elements.Contains(element);
        }
        /// <summary>
        /// To the list.
        /// </summary>
        /// <returns></returns>
        internal List<ElementBox> ToList()
        {
            return elements.ToList<ElementBox>();
        }
        /// <summary>
        /// Clears this instance.
        /// </summary>
        internal void Clear()
        {
            elements.Clear();
            maxBaseLineOffset = 0;
            maxTextElementHeight = double.NaN;
            maxTextElementBaselineOffset = 0;
            SkipClipImage = false;
        }
        /// <summary>
        /// Updates the max element.
        /// </summary>
        internal void UpdateMaxElement()
        {
            maxBaseLineOffset = 0;
            maxTextElementHeight = double.NaN;
            maxTextElementBaselineOffset = 0;
            for (int i = 0; i < Count; i++)
            {
                UpdateMaxElementHeight(elements[i]);
            }
        }
        /// <summary>
        /// Updates the height of the max element.
        /// </summary>
        /// <param name="element">The element.</param>
        private void UpdateMaxElementHeight(ElementBox element)
        {
            if (element is TextElementBox || element is ListTextElementBox)
            {
                if (double.IsNaN(maxTextElementHeight))
                    maxTextElementHeight = 0;
                if (maxTextElementHeight < element.Height)
                {
                    maxTextElementHeight = element.Height;
                    maxTextElementBaselineOffset = element is TextElementBox ? (element as TextElementBox).BaselineOffset : (element as ListTextElementBox).BaselineOffset;
                }
                if (maxBaseLineOffset < maxTextElementBaselineOffset)
                    maxBaseLineOffset = maxTextElementBaselineOffset;
            }
            else if (maxBaseLineOffset < element.Height)
                maxBaseLineOffset = element.Height;
        }
        #endregion
    }
    #endregion
}
