//-------------------------------------------------------------------------------------------------
// <copyright file="Element.cs" company="syncfusion">
// Copyright (c) Syncfusion Inc. 2001 - 2014. All rights reserved.
//
//  Use of this code is subject to the terms of our license.
//  A copy of the current license can be obtained at any time by e-mailing
//  licensing@syncfusion.com. Re-distribution in any form is strictly
//  prohibited. Any infringement will be prosecuted under applicable laws. 
// </copyright>
//-------------------------------------------------------------------------------------------------

using System;
using System.Collections.Specialized;
using System.Collections;
using System.ComponentModel;
using System.Diagnostics;
using System.Text;

using Syncfusion.Diagnostics;

using Syncfusion.Collections;
using Syncfusion.Collections.BinaryTree;
using Syncfusion.Grouping.Internals;

using ITreeTableSummary = Syncfusion.Collections.BinaryTree.ITreeTableSummary;

namespace Syncfusion.Grouping
{
    /// <summary>
    /// The base class for all elements in the grouping engine that represent the grouped
    /// structure of an underlying datasource.
    /// </summary>
    public abstract class Element : IDisposable, IDisposedEvent, IIsDisposedProperty, ITreeTableCounterSource, ITreeTableSummaryArraySource
    {
        internal static bool traceOnInvalidateCounterBottomUp = false;
        Element _parentElement;
        BitVector32 bv = new BitVector32(0);

        /// <summary>
        /// The <see cref="Engine"/> this element belongs to.
        /// </summary>
        public virtual Engine Engine
        {
            get
            {
                return ParentTable.Engine;
            }
        }

////#if DEBUG
       /// <exclude/>
        /// <summary>
        /// Returns row index of the element.
        /// </summary>
        /// <returns>Row index.</returns>
        public int GetRowIndex()
        {
            return Engine.Table.NestedDisplayElements.IndexOf(this);
        }
////#endif

        /// <summary>
        /// A reference to the main <see cref="Syncfusion.Grouping.Engine.Table"/> of the <see cref="Engine"/>
        /// this element belongs to.
        /// </summary>
        public virtual Table EngineTable
        {
            get
            {
                Engine engine = Engine;
                if (engine == null)
                {
                    return this as Table;
                }

                return engine.Table;
            }
        }

        /// <summary>
        /// Gets a reference to the parent <see cref="Table"/> this element belongs to.
        /// </summary>
        [System.Xml.Serialization.XmlIgnore]
        [DesignerSerializationVisibilityAttribute(DesignerSerializationVisibility.Hidden)]
        [Browsable(false)]
        public virtual Table ParentTable
        {
            get
            {
                if (this._parentElement == null)
                {
                    return null;
                }

                if (this._parentElement is Table)
                {
                    return (Table)_parentElement;
                }

                return this._parentElement.ParentTable;
            }

            set
            {
                throw new NotSupportedException();
            }
        }

        /// <summary>
        /// Returns the table hierarchy level of this element. It is -1 for
        /// the root element. 0 for elements belonging to the main table and
        /// greater than 0 for elements that belong to nested table.
        /// </summary>
        [System.Xml.Serialization.XmlIgnore]
        [DesignerSerializationVisibilityAttribute(DesignerSerializationVisibility.Hidden)]
        [Browsable(false)]
        public int TableLevel
        {
            get
            {
                Table table = ParentTable;
                if (table == null)
                {
                    return -1;
                }

                return table.TableLevel + 1;
            }
        }

        bool inEnsureInitialized
        {
            get
            {
                return bv[__inEnsureInitialized] != 0;
            }

            set
            {
                bv[__inEnsureInitialized] = value ? 1 : 0;
            }
        }

        bool inDispose
        {
            get
            {
                return bv[__inDispose] != 0;
            }

            set
            {
                bv[__inDispose] = value ? 1 : 0;
            }
        }

        bool inDisposed
        {
            get
            {
                return bv[__inDisposed] != 0;
            }

            set
            {
                bv[__inDisposed] = value ? 1 : 0;
            }
        }

        bool isDisposed
        {
            get
            {
                return bv[__isDisposed] != 0;
            }

            set
            {
                bv[__isDisposed] = value ? 1 : 0;
            }
        }

        /// <summary>
        /// Gets if this object can be uniquely identified with the <see cref="Id"/>
        /// property. Internal caches use this id to look up objects.
        /// </summary>
        /// <returns>True if <see cref="Id"/> property returns a unique identifier.</returns>
        [Syncfusion.Documentation.DocumentationExclude()]
        [EditorBrowsable(EditorBrowsableState.Never)]
        public virtual bool SupportsId()
        {
            return false;
        }

        /// <summary>
        /// Internal caches use this id to look up objects.
        /// </summary>
        /// <returns>A unique integer <see cref="Id"/>.</returns>
        [Syncfusion.Documentation.DocumentationExclude()]
        [EditorBrowsable(EditorBrowsableState.Never)]
        public virtual int Id
        {
            get
            {
                return 0;
            }

            set
            {
            }
        }

        /// <internalonly/>
        [Syncfusion.Documentation.DocumentationExclude()]
        [EditorBrowsable(EditorBrowsableState.Never)]
        public bool Reserved1
        {
            get
            {
                return bv[__reserved1] != 0;
            }

            set
            {
                bv[__reserved1] = value ? 1 : 0;
            }
        }

        /// <internalonly/>
        [Syncfusion.Documentation.DocumentationExclude()]
        [EditorBrowsable(EditorBrowsableState.Never)]
        public bool Reserved2
        {
            get
            {
                return bv[__reserved2] != 0;
            }

            set
            {
                bv[__reserved2] = value ? 1 : 0;
            }
        }

        /// <internalonly/>
        [Syncfusion.Documentation.DocumentationExclude()]
        [EditorBrowsable(EditorBrowsableState.Never)]
        public bool Reserved3
        {
            get
            {
                return bv[__reserved3] != 0;
            }

            set
            {
                bv[__reserved3] = value ? 1 : 0;
            }
        }

        /// <internalonly/>
        [Syncfusion.Documentation.DocumentationExclude()]
        [EditorBrowsable(EditorBrowsableState.Never)]
        public bool Reserved4
        {
            get
            {
                return bv[__reserved4] != 0;
            }

            set
            {
                bv[__reserved4] = value ? 1 : 0;
            }
        }

        /// <internalonly/>
        [Syncfusion.Documentation.DocumentationExclude()]
        [EditorBrowsable(EditorBrowsableState.Never)]
        public bool Reserved5
        {
            get
            {
                return bv[__reserved5] != 0;
            }

            set
            {
                bv[__reserved5] = value ? 1 : 0;
            }
        }

        /// <internalonly/>
        [Syncfusion.Documentation.DocumentationExclude()]
        [EditorBrowsable(EditorBrowsableState.Never)]
        public bool Reserved6
        {
            get
            {
                return bv[__reserved6] != 0;
            }

            set
            {
                bv[__reserved6] = value ? 1 : 0;
            }
        }

        /// <internalonly/>
        [Syncfusion.Documentation.DocumentationExclude()]
        [EditorBrowsable(EditorBrowsableState.Never)]
        public bool Reserved7
        {
            get
            {
                return bv[__reserved7] != 0;
            }

            set
            {
                bv[__reserved7] = value ? 1 : 0;
            }
        }

        /// <internalonly/>
        [Syncfusion.Documentation.DocumentationExclude()]
        [EditorBrowsable(EditorBrowsableState.Never)]
        public bool Reserved8
        {
            get
            {
                return bv[__reserved8] != 0;
            }

            set
            {
                bv[__reserved8] = value ? 1 : 0;
            }
        }

        /// <internalonly/>
        [Syncfusion.Documentation.DocumentationExclude()]
        [EditorBrowsable(EditorBrowsableState.Never)]
        public bool Reserved9
        {
            get
            {
                return bv[__reserved9] != 0;
            }

            set
            {
                bv[__reserved9] = value ? 1 : 0;
            }
        }

        /// <internalonly/>
        [Syncfusion.Documentation.DocumentationExclude()]
        [EditorBrowsable(EditorBrowsableState.Never)]
        public bool Reserved10
        {
            get
            {
                return bv[__reserved10] != 0;
            }

            set
            {
                bv[__reserved10] = value ? 1 : 0;
            }
        }

        /// <internalonly/>
        [Syncfusion.Documentation.DocumentationExclude()]
        [EditorBrowsable(EditorBrowsableState.Never)]
        public bool Reserved11
        {
            get
            {
                return bv[__reserved11] != 0;
            }

            set
            {
                bv[__reserved11] = value ? 1 : 0;
            }
        }

        /// <internalonly/>
        [Syncfusion.Documentation.DocumentationExclude()]
        [EditorBrowsable(EditorBrowsableState.Never)]
        public int Reserved16a
        {
            get
            {
                return bv[__reserved16a];
            }

            set
            {
                bv[__reserved16a] = value;
            }
        }

        /// <internalonly/>
        [Syncfusion.Documentation.DocumentationExclude()]
        [EditorBrowsable(EditorBrowsableState.Never)]
        public int Reserved16b
        {
            get
            {
                return bv[__reserved16b];
            }

            set
            {
                bv[__reserved16b] = value;
            }
        }

        /// <internalonly/>
        [Syncfusion.Documentation.DocumentationExclude()]
        [EditorBrowsable(EditorBrowsableState.Never)]
        public int Reserved16c
        {
            get
            {
                return bv[__reserved16c];
            }

            set
            {
                bv[__reserved16c] = value;
            }
        }
        
        static int globalId;
        static BitVector32.Section __inDispose;
        static BitVector32.Section __inDisposed;
        static BitVector32.Section __inEnsureInitialized;
        static BitVector32.Section __isDisposed;
        static BitVector32.Section __reserved1;
        static BitVector32.Section __reserved2;
        static BitVector32.Section __reserved3;
        static BitVector32.Section __reserved4;
        static BitVector32.Section __reserved5;
        static BitVector32.Section __reserved6;
        static BitVector32.Section __reserved7;
        static BitVector32.Section __reserved8;
        static BitVector32.Section __reserved9;
        static BitVector32.Section __reserved10;
        static BitVector32.Section __reserved11;
        static BitVector32.Section __filterState;
        static BitVector32.Section __reserved16a;
        static BitVector32.Section __reserved16b;
        static BitVector32.Section __reserved16c;
        internal int filterState
        {
            get
            {
                return bv[__filterState] - 1;  // -1, 0, 1
            }

            set
            {
                bv[__filterState] = value + 1;
            }
        }

        static Element()
        {
#if TRACE
            traceOnInvalidateCounterBottomUp = Switches.InvalidateCounterBottomUp.TraceVerbose;
#endif
            __inDispose = BitVector32.CreateSection(1);
            __inDisposed = BitVector32.CreateSection(1, __inDispose);
            __inEnsureInitialized = BitVector32.CreateSection(1, __inDisposed);
            __inDispose = BitVector32.CreateSection(1, __inEnsureInitialized);

            __inDisposed = BitVector32.CreateSection(1, __inDispose);
            __isDisposed = BitVector32.CreateSection(1, __inDisposed);
            __reserved1 = BitVector32.CreateSection(1, __isDisposed);
            __reserved2 = BitVector32.CreateSection(1, __reserved1);

            __reserved3 = BitVector32.CreateSection(1, __reserved2);
            __reserved4 = BitVector32.CreateSection(1, __reserved3);
            __reserved5 = BitVector32.CreateSection(1, __reserved4);
            __reserved6 = BitVector32.CreateSection(1, __reserved5);

            __reserved7 = BitVector32.CreateSection(1, __reserved6);
            __reserved8 = BitVector32.CreateSection(1, __reserved7);
            __reserved9 = BitVector32.CreateSection(1, __reserved8);

            // extra caerful - Never use __reserved10 and __filterState for same kind of element!
            __filterState = BitVector32.CreateSection(2, __reserved9);     // 2 total: 14, avail: 18
            __reserved10 = BitVector32.CreateSection(1, __reserved9);     // 2 total: 14, avail: 18
            __reserved11 = BitVector32.CreateSection(1, __reserved10);     // 2 total: 14, avail: 18

            __reserved16a = BitVector32.CreateSection(15, __filterState);  // 5
            __reserved16b = BitVector32.CreateSection(15, __reserved16a);  // 5
            __reserved16c = BitVector32.CreateSection(15, __reserved16b);  // 5
        }

        /// <summary>
        /// Initializes a new empty element.
        /// </summary>
        protected Element()
        {
            if (this.SupportsId())
            {
                this.Id = globalId++;
            }
        }

        /// <summary>
        /// Initializes a new empty element that belongs to the specified parent.
        /// </summary>
        protected Element(Element parent)
            : this()
        {
            this._parentElement = parent;
        }

        ////        ~Element()
        ////        {
        ////            inDispose = true;
        ////            Dispose(false);
        ////            inDispose = false;
        ////        }

        /// <summary>
        /// Gets debug text information about the element.
        /// </summary>
        [System.Xml.Serialization.XmlIgnore]
        [DesignerSerializationVisibilityAttribute(DesignerSerializationVisibility.Hidden)]
        [Browsable(false)]
        public string Info
        {
            get
            {
                return ToString();
            }
        }

        /// <summary>
        /// The kind of element.
        /// </summary>
        [System.Xml.Serialization.XmlIgnore]
        [DesignerSerializationVisibilityAttribute(DesignerSerializationVisibility.Hidden)]
        [Browsable(false)]
        public virtual DisplayElementKind Kind
        {
            get { return DisplayElementKind.None; }
        }
        
        /// <summary>
        /// Returns True if object is executing a <see cref="Dispose"/> method call.
        /// </summary>
        [System.Xml.Serialization.XmlIgnore]
        [DesignerSerializationVisibilityAttribute(DesignerSerializationVisibility.Hidden)]
        [Browsable(false)]
        public bool Disposing
        {
            get
            {
                return inDispose;
            }
        }

        /// <summary>
        /// Gets if object has been disposed.
        /// </summary>
        [System.Xml.Serialization.XmlIgnore]
        [DesignerSerializationVisibilityAttribute(DesignerSerializationVisibility.Hidden)]
        [Browsable(false)]
        public bool IsDisposed
        {
            get
            {
                return isDisposed;
            }
        }
        
        /// <summary>
        /// Returns after object was disposed and object is executing a <see cref="Disposed"/> event.
        /// </summary>
        [System.Xml.Serialization.XmlIgnore]
        [DesignerSerializationVisibilityAttribute(DesignerSerializationVisibility.Hidden)]
        [Browsable(false)]
        public bool InDisposed
        {
            get
            {
                return inDisposed;
            }
        }

        /// <overload>
        /// Disposes the object.
        /// </overload>
        /// <summary>
        /// Disposes the object.
        /// </summary>
        public void Dispose()
        {
            inDispose = true;
            Dispose(true);
            inDispose = false;
            inDisposed = true;
            OnDisposed(EventArgs.Empty);
            inDisposed = false;
            isDisposed = true;
            GC.SuppressFinalize(this);
        }

        /// <summary>
        /// Occurs after the object is disposed.
        /// </summary>
        public event EventHandler Disposed;

        /// <summary>
        /// Raises the <see cref="Disposed"/> event.
        /// </summary>
        /// <param name="e">An <see cref="EventArgs" /> that contains the event data.</param>
        protected virtual void OnDisposed(EventArgs e)
        {
            if (Disposed != null)
            {
                Disposed(this, e);
            }
        }

        /// <summary>
        /// Called to clean up state of this object when it is disposed.
        /// </summary>
        /// <param name="disposing">True if called from <see cref="Dispose"/>; False if called from Finalizer.</param>
        protected virtual void Dispose(bool disposing)
        {
            if (disposing)
            {
                ////                this.tag = null;
                ////                this.table = null;
                ////                this.engine = null;
                this._parentElement = null;
            }

            GC.SuppressFinalize(this);
        }
        
        /// <summary>
        /// Returns the TableDescriptor the element belongs to.
        /// </summary>
        [System.Xml.Serialization.XmlIgnore]
        [DesignerSerializationVisibilityAttribute(DesignerSerializationVisibility.Hidden)]
        [Browsable(false)]
        public virtual TableDescriptor ParentTableDescriptor
        {
            get
            {
                return ParentTable.TableDescriptor;
            }
        }

        internal Table GetRootTable()
        {
            if (_parentElement == null)
            {
                return null;
            }

            Element root = _parentElement;
            while (root._parentElement != null)
            {
                root = root._parentElement;
                if (root is Table)
                {
                    break;
                }
            }

            return root as Table;
        }
        
        /// <summary>
        /// Returns the parent this element belongs to.
        /// </summary>
        [System.Xml.Serialization.XmlIgnore]
        [DesignerSerializationVisibilityAttribute(DesignerSerializationVisibility.Hidden)]
        [Browsable(false)]
        public virtual Element ParentElement
        {
            get
            {
                return _parentElement;
            }

            set
            {
                _parentElement = value;
                ////if (_parentElement == null)
                ////    TraceUtil.TraceCurrentMethodInfo();
            }
        }

        /// <summary>
        /// Returns the parent this element belongs to. If this element is a <see cref="ChildTable"/>,
        /// the <see cref="NestedTable"/> that links the parent table with the child table is returned.
        /// </summary>
        [System.Xml.Serialization.XmlIgnore]
        [DesignerSerializationVisibilityAttribute(DesignerSerializationVisibility.Hidden)]
        [Browsable(false)]
        public virtual Element ParentDisplayElement
        {
            get
            {
                return _parentElement;
            }
        }

        /// <summary>
        /// Ensures the object and nested objects reflect any changes made to the engine or table descriptor. This is
        /// an integral part of the engine's "on-demand execution" of schema changes. Before elements
        /// in the engine are accessed, they call <see cref="EnsureInitialized"/>. If changes were
        /// previously made that affect the queried element, all changes will be appllied at this time.
        /// </summary>
        /// <param name="sender">The object that triggered the call.</param>
        /// <returns>True if changes were detected and the object was updated; False otherwise.</returns>
        public bool EnsureInitialized(object sender)
        {
            return EnsureInitialized(sender, false);
        }

        /// <summary>
        /// Ensures the object, nested objects, and parent elements reflect any changes made to the engine or table descriptor. This is
        /// an integral part of the engine's "on-demand execution" of schema changes. Before elements
        /// in the engine are accessed, they call <see cref="EnsureInitialized"/>. If changes were
        /// previously made that affect the queried element, all changes will be applied at this time.
        /// </summary>
        /// <param name="sender">The object that triggered the call.</param>
        /// <param name="notifyParent">Specifes if the parent elements <see cref="EnsureInitialized"/>
        /// should also be called.</param>
        /// <returns>True if changes were detected and the object was updated; False otherwise.</returns>
        public virtual bool EnsureInitialized(object sender, bool notifyParent)
        {
            if (IsDisposed || inEnsureInitialized || (_parentElement != null && _parentElement.IsDisposed))
            {
                return false;
            }

            inEnsureInitialized = true;
            bool ret = false;
            if (notifyParent && _parentElement != null)
            {
                ret = _parentElement.EnsureInitialized(sender, notifyParent);
            }

            if (IsDisposed)
            {
                return false;
            }

            if (!ret)
            {
                ret = OnEnsureInitialized(sender);
            }

            inEnsureInitialized = false;

            return ret;
        }

        /// <summary>
        /// This virtual method is called from <see cref="OnEnsureInitialized"/> and
        /// lets derived elements implement element-specific logic to ensure object
        /// is up to data.
        /// </summary>
        /// <param name="sender">The object that triggered the <see cref="EnsureInitialized"/> call.</param>
        /// <returns>True if changes were detected and the object was updated; False otherwise.</returns>
        protected virtual bool OnEnsureInitialized(object sender)
        {
            return false;
        }

        /// <summary>
        /// A reference to the parent <see cref="Group"/> this element belongs to. If the element is
        /// a toplevel group, then NULL is returned.
        /// </summary>
        [System.Xml.Serialization.XmlIgnore]
        [DesignerSerializationVisibilityAttribute(DesignerSerializationVisibility.Hidden)]
        [Browsable(false)]
        public virtual Group ParentGroup
        {
            get
            {
                if (_parentElement is Table)
                {
                    return null;
                }
                else if (_parentElement != null)
                {
                    if (_parentElement is Group)
                    {
                        return (Group)_parentElement;
                    }

                    return _parentElement.ParentGroup;
                }

                return null;
            }
        }

        /// <summary>
        /// A reference to the parent <see cref="Record"/> this element belongs to. If the element
        /// is not a child of a record (e.g. a group), then NULL is returned.
        /// </summary>
        [System.Xml.Serialization.XmlIgnore]
        [DesignerSerializationVisibilityAttribute(DesignerSerializationVisibility.Hidden)]
        [Browsable(false)]
        public virtual Record ParentRecord
        {
            get
            {
                if (_parentElement is Table)
                {
                    return null;
                }
                else if (_parentElement != null)
                {
                    if (_parentElement is Record)
                    {
                        return (Record)_parentElement;
                    }

                    return _parentElement.ParentRecord;
                }

                return null;
            }
        }

        /// <summary>
        /// A reference to the child table this element belongs. A ChildTable is either a TopLevelGroup or a group that can be referenced
        /// as a nested table from a record in a parent table.
        /// </summary>
        [System.Xml.Serialization.XmlIgnore]
        [DesignerSerializationVisibilityAttribute(DesignerSerializationVisibility.Hidden)]
        [Browsable(false)]
        public virtual ChildTable ParentChildTable
        {
            get
            {
                if (_parentElement is Table)
                {
                    return null;
                }
                else if (_parentElement != null)
                {
                    if (_parentElement is ChildTable)
                    {
                        return (ChildTable)_parentElement;
                    }

                    return _parentElement.ParentChildTable;
                }

                return null;
            }
        }

        /// <summary>
        /// A reference to the parent <see cref="Section"/> this element belongs to.
        /// </summary>
        [System.Xml.Serialization.XmlIgnore]
        [DesignerSerializationVisibilityAttribute(DesignerSerializationVisibility.Hidden)]
        [Browsable(false)]
        public virtual Section ParentSection
        {
            get
            {
                if (_parentElement != null)
                {
                    if (_parentElement is Section)
                    {
                        return (Section)_parentElement;
                    }

                    return _parentElement.ParentSection;
                }

                return null;
            }
        }

        /// <summary>
        /// Returns the level for a nested group.
        /// </summary>
        [System.Xml.Serialization.XmlIgnore]
        [DesignerSerializationVisibilityAttribute(DesignerSerializationVisibility.Hidden)]
        [Browsable(false)]
        public int GroupLevel
        {
            get
            {
                if (ParentGroup == null || this is ChildTable)
                {
                    return -1;
                }

                return ParentGroup.GroupLevel + 1;
            }
        }

        /// <summary>
        /// Returns the level for a nested child table.
        /// </summary>
        [System.Xml.Serialization.XmlIgnore]
        [DesignerSerializationVisibilityAttribute(DesignerSerializationVisibility.Hidden)]
        [Browsable(false)]
        public int ChildTableGroupLevel
        {
            get
            {
                if (ParentGroup == null)
                {
                    return -1;
                }

                return ParentGroup.ChildTableGroupLevel + 1;
            }
        }

        /// <summary>For internal use.</summary>
        /// <returns>Returns ITreeTableCounter</returns>
        /// <internalonly/>
        [Syncfusion.Documentation.DocumentationExclude()]
        [EditorBrowsable(EditorBrowsableState.Never)]
        public virtual ITreeTableCounter GetCounter()
        {
            ////#if DEBUG
            ////            Table parentTable = ParentTable;
            ////#if true
            ////            if (this.ParentTable != null && parentTable.InInitialize)
            ////                throw new InvalidOperationException("Do not call GetCounter while structures are being initialized!");
            ////#else
            ////                return new Counter(0, 0, 0, 0, 0);
            ////#endif

            if (this.ParentElement == null || this.ParentElement.IsChildVisible(this))
            {
                return CounterFactory.CreateCounter(GetVisibleCount(), GetYAmountCount(), GetFilteredRecordCount(), GetElementCount(), GetRecordCount(), GetCustomCount(), GetVisibleCustomCount());
            }
            else
            {
                return CounterFactory.CreateCounter(0, 0, GetFilteredRecordCount(), GetElementCount(), GetRecordCount(), GetCustomCount(), 0);
            }
        }

        internal ICounterFactory CounterFactory
        {
            get
            {
                return this.Engine.CounterFactory;
            }
        }

        /// <summary>For internal use.</summary>
        /// <internalonly/>
        [Syncfusion.Documentation.DocumentationExclude()]
        [EditorBrowsable(EditorBrowsableState.Never)]
        public abstract void InvalidateCounterTopDown(bool notifyCounterSource);

        /// <summary>For internal use.</summary>
        /// <internalonly/>
        [Syncfusion.Documentation.DocumentationExclude()]
        [EditorBrowsable(EditorBrowsableState.Never)]
        public virtual void InvalidateCounterBottomUp()
        {
            ElementTreeTableEntry e = this.GetElementEntry();
            if (e != null)
            {
                e.InvalidateCounterBottomUp(true);
            }
        }

        /// <summary>For internal use.</summary>
        /// <internalonly/>
        [Syncfusion.Documentation.DocumentationExclude()]
        [EditorBrowsable(EditorBrowsableState.Never)]
        public virtual void InvalidateCounter()
        {
            ElementTreeTableEntry e = this.GetElementEntry();
            if (e != null)
            {
                e.InvalidateCounter();
            }
        }

        /// <summary>For internal use.</summary>
        /// <internalonly/>
        [Syncfusion.Documentation.DocumentationExclude()]
        [EditorBrowsable(EditorBrowsableState.Never)]
        public virtual void OnElementTreeInvalidateCounterBottomUp()
        {
        }

        /// <overload>
        /// Gets summary information for this element and child elements. The summaries
        /// are in the same order as the <see cref="TableDescriptor.Summaries"/> of the
        /// parent table descriptor.
        /// </overload>
        /// <summary>
        /// Gets summary information for this element and child elements. The summaries
        /// are in the same order as the <see cref="TableDescriptor.Summaries"/> of the
        /// parent table descriptor.
        /// </summary>
        /// <param name="parentTable">A reference to the parent table of this element.</param>
        /// <returns>An array of <see cref="ITreeTableSummary"/> objects.</returns>
        public ITreeTableSummary[] GetSummaries(Table parentTable)
        {
            bool summaryChanged;
            return GetSummaries(parentTable, out summaryChanged);
        }

        /// <summary>
        /// Gets summary information for this element and child elements. The summaries
        /// are in the same order as the <see cref="TableDescriptor.Summaries"/> of the
        /// parent table descriptor.
        /// </summary>
        /// <param name="parentTable">A reference to the parent table of this element.</param>
        /// <param name="summaryChanged">Returns True if changes were detected.</param>
        /// <returns>An array of <see cref="ITreeTableSummary"/> objects.</returns>
        public virtual ITreeTableSummary[] GetSummaries(Table parentTable, out bool summaryChanged)
        {
            ElementTreeTable e = this.TreeEntries;
            if (e != null)
            {
                ////EnsureInitialized(this);
                ITreeTableSummary[] ss = e.GetSummaries(parentTable.tableEmptySummaries, out summaryChanged);
                if (ss == null)
                {
                    return parentTable.GetEmptySummaries();
                }

                ITreeTableSummary[] summaries = new ITreeTableSummary[ss.Length];
                ss.CopyTo(summaries, 0);
                return summaries;
            }

            summaryChanged = false;
            return parentTable.GetEmptySummaries();
        }

        /// <summary>For internal use.</summary>
        /// <internalonly/>
        [Syncfusion.Documentation.DocumentationExclude()]
        [EditorBrowsable(EditorBrowsableState.Never)]
        public abstract void InvalidateSummariesTopDown();

        /// <summary>For internal use.</summary>
        /// <internalonly/>
        [Syncfusion.Documentation.DocumentationExclude()]
        [EditorBrowsable(EditorBrowsableState.Never)]
        public abstract void InvalidateSummary();

        /// <summary>For internal use.</summary>
        /// <internalonly/>
        [Syncfusion.Documentation.DocumentationExclude()]
        [EditorBrowsable(EditorBrowsableState.Never)]
        public virtual void InvalidateSummariesBottomUp()
        {
            ElementTreeTableEntry e = this.GetElementEntry();
            if (e != null)
            {
                e.InvalidateSummariesBottomUp(true);
            }
        }

        /// <summary>
        /// Gets the number of visible elements in this group. Do not check parent's visible state when doing returning FilterCount in display elements.
        /// Only container elements behave different. They will ignore values returned form CounterTreeTable.
        /// </summary>
        /// <returns>Number of visible elements.</returns>
        public abstract int GetVisibleCount();

        /// <summary>
        /// Gets the number of visible elements in this group.
        /// </summary>
        /// <returns>Element count.</returns>
        public abstract int GetElementCount();

        /// <summary>
        /// Gets the number of visible records (excluding records that do not meet filter criteria).
        /// </summary>
        /// <returns>Filtered record count.</returns>
        public abstract int GetFilteredRecordCount();

        /// <summary>
        /// Gets the number of visible elements (0 for captions, groups etc. 1 for records).
        /// </summary>
        /// <returns>Record count.</returns>
        public abstract int GetRecordCount();

        /// <summary>
        /// Gets the height (e.g. screen pixels) for the element.
        /// </summary>
        /// <returns>Element height.</returns>
        public abstract double GetYAmountCount();

        /// <summary>
        /// Gets custom count for the element.
        /// </summary>
        /// <returns>returns 0.</returns>
        public virtual double GetCustomCount()
        {
            return 0;
        }

        /// <summary>
        /// Gets custom count for visible elements.
        /// </summary>
        /// <returns>returns 0.</returns>
        public virtual double GetVisibleCustomCount()
        {
            return 0;
        }
        
        /// <summary>
        /// Determines if the specified element is a direct child element of this element
        /// and if it should appear in visible display elements collection.
        /// </summary>
        /// <param name="el">The child element to be analyzed.</param>
        /// <returns>True if element is visible; False otherwise.</returns>
        public virtual bool IsChildVisible(Element el)
        {
            return el != null && (el.ParentElement == this || el.ParentDisplayElement == this);
        }
        
        /// <summary>
        /// Determines if this element is visible in the parent element it belongs to as returned
        /// by <see cref="IsChildVisible"/> of the parent element.
        /// </summary>
        /// <returns>True if element is visible; False otherwise.</returns>
        public bool GetVisibleInParent()
        {
            Element parent = ParentDisplayElement;
            return parent == null || parent.IsChildVisible(this);
        }

        /// <summary>
        /// Determines if this element is visible (see <see cref="IsChildVisible"/>) and if all
        /// of its parents are also visible. If this element belongs to a parent group that
        /// has been collapsed, it will return False.
        /// </summary>
        /// <returns>True if this element and all its parent elements are visible; False otherwise.</returns>
        public bool GetVisibleInHierarchy()
        {
            if (this.IsDisposed || ParentTable == null || Engine == null)
            {
                return false;
            }

            if (Object.ReferenceEquals(this, Engine.lastGetVisibleInHierarchyElement))
            {
                return Engine.lastGetVisibleInHierarchyElementReturnValue;
            }

            Engine.lastGetVisibleInHierarchyElement = this;
            bool b = _GetVisibleInHierarchy();
            Engine.lastGetVisibleInHierarchyElementReturnValue = b;
            return b;
        }

        bool _GetVisibleInHierarchy()
        {
            Element value = this;
            Element parent = ParentDisplayElement;

            // orphaned child table without parent record.
            if (value is ChildTable && parent == null && value.ParentTableDescriptor.ParentRelation != null)
            {
                return false;
            }

            while (value != null)
            {
                if (parent == null)
                {
                    return true;
                }

                if (parent.IsDisposed || !parent.IsChildVisible(value))
                {
                    return false;
                }

#if DIAGNOSING
                bool isExp;
                if (parent is Group)
                {
                    isExp = ((Group) parent).IsExpanded;
                }
                if (parent is Record)
                {
                    isExp = ((Record) parent).IsExpanded;
                }
#endif
                value = parent;
                parent = parent.ParentDisplayElement;

                if (value is ChildTable && parent == null && value.ParentTableDescriptor.ParentRelation != null)
                {
                    return false;
                }
            }

            return false;
        }

        /// <summary>Returns a string holding the element.</summary>
        /// <returns>String representation of the current object.</returns>
        /// <override/>
        public override string ToString()
        {
            return GetType().Name;
            ////
            ////            if (this.ParentElement == null)
            ////                return "";
            ////
            ////            this.EnsureInitialized(this, true);
            ////            string s = "";
            ////            for (int n = 0; n < GroupLevel; n++)
            ////                s += "   ";
            ////
            ////            return String.Concat(s,
            ////                GetType().Name,
            ////                " { ",
            ////                "GroupLevel=", GroupLevel.ToString(),
            ////                ", Visible=", (IsVisible && this.FilteredRecordCount > 0).ToString(),
            ////                ", Ci=", this.ElementPosition.ToString(),
            ////                ", Vpos=", this.DisplayPosition.ToString(),
            ////                ", Fpos=", FilteredRecordPosition.ToString(),
            ////                //", Grop= ", this.FilterCaptionPosition.ToString(),
            ////                " }");
        }

        internal ElementTreeTable TreeEntries
        {
            get
            {
                return ElementHelper.GetTreeEntries(this, false);
            }
        }

        /// <summary>
        /// Gets the number of direct child elements that belong to this element's collection or 0 if this is not a container element. Good
        /// for determining the number of items to be displayed in a group's "(#) Items" caption.
        /// </summary>
        /// <returns>Number of childs.</returns>
        public virtual int GetChildCount()
        {
            ElementTreeTable t = TreeEntries;
            if (t != null)
            {
                return t.Count;
            }

            return 0;
        }
        
        ////        public object Tag
        ////        {
        ////            get
        ////            {
        ////                return tag;
        ////            }
        ////            set
        ////            {
        ////                tag = value;
        ////            }
        ////        }

        /// <summary>
        /// The ElementTreeTableEntry this element is associated with (either SectionsTreeTableEntry or SortedRecordsTreeTableEntry).
        /// <returns>returns ElementTreeTableEntry</returns>
        /// </summary>
        /// <returns>returns ElementTreeTableEntry</returns>
        internal abstract ElementTreeTableEntry GetElementEntry();

        internal virtual ElementTreeTableEntry GetSummaryElementEntry()
        {
            return GetElementEntry();
        }

        internal ElementTreeTable GetElementEntryTable() 
        { 
            return ParentElement != null ? ParentElement.TreeEntries as ElementTreeTable : null; 
        }

        // Use Table.DisplayElements.IndexOf instead.
        [Syncfusion.Documentation.DocumentationExclude()]
        internal int GetDisplayPosition()
        {
            return (int)ElementHelper.GetCumulatedPosition(this, CounterKind.DisplayElementCount);
        }

        // Use Table.FilteredRecords.IndexOf instead.
        internal int GetFilteredRecordPosition()
        {
            return (int)ElementHelper.GetCumulatedPosition(this, CounterKind.FilteredRecordsCount);
        }

        // Use Table.SortedRecords.IndexOf instead.
        internal int GetSortedRecordPosition()
        {
            return (int)ElementHelper.GetCumulatedPosition(this, CounterKind.RecordsCount);
        }

        // Use Table.GroupedElements.IndexOf instead.
        internal int GetElementPosition()
        {
            return (int)ElementHelper.GetCumulatedPosition(this, CounterKind.ElementsCount);
        }

        // Use Table.FilteredRecords.IndexOf instead.
        internal double GetYAmountPosition()
        {
            return ElementHelper.GetCumulatedPosition(this, CounterKind.YAmountCount);
        }

        /// <summary>
        /// Gets the position for the "Custom Counter" of this element. See the "Grid.Grouping\Samples\CustomSummaries"
        /// sample for example usage of custom counters.
        /// </summary>
        /// <returns>The custom counter position for this element.</returns>
        public double GetCustomPosition()
        {
            return ElementHelper.GetCumulatedPosition(this, CounterKind.CustomCount);
        }

        /// <summary>
        /// Gets the position for the "Visible Custom Counter" of this element. See the "Grid.Grouping\Samples\CustomSummaries"
        /// sample for example usage of custom counters.
        /// </summary>
        /// <returns>The visible custom counter position for this element.</returns>
        public double GetVisibleCustomPosition()
        {
            return ElementHelper.GetCumulatedPosition(this, CounterKind.VisibleCustomCount);
        }

        ITreeTableSummary[] Syncfusion.Collections.BinaryTree.ITreeTableSummaryArraySource.GetSummaries(ITreeTableEmptySummaryArraySource emptySummaries, out bool summaryChanged)
        {
            return GetSummaries(((ITableSource)emptySummaries).GetTable(), out summaryChanged);
        }

        #region Methods used by CurrentRecordManager

        /// <summary>
        /// If the element is a record, GetData returns a reference to the underlying data for the record, e.g. the DataRowView
        /// of a DataView.
        /// </summary>
        /// <returns>A reference to the underlying data for the record.</returns>
        public virtual object GetData()
        {
            Record r = ParentRecord;
            if (r != null)
            {
                return r.GetData();
            }

            return null;
        }
        
        /// <summary>
        /// Called when <see cref="CurrentRecordManager.BeginEdit"/> is called.
        /// </summary>
        /// <returns>True if <see cref="CurrentRecordManager.BeginEdit"/> can proceed; False if it should abort.</returns>
        public virtual bool OnBeginEditCalled()
        {
            return true;
        }

        /// <summary>
        /// Called when <see cref="CurrentRecordManager.BeginEdit"/> successfully finishes.
        /// </summary>
        /// <param name="success">True, if it is successfully finished; False, otherwise.</param>
        public virtual void OnBeginEditComplete(bool success)
        {
        }

        /// <summary>
        /// Called when <see cref="CurrentRecordManager.EndEdit"/> is called.
        /// </summary>
        /// <returns>True if <see cref="CurrentRecordManager.EndEdit"/> can proceed; False if it should abort.</returns>
        public virtual bool OnEndEditCalled()
        {
            return true;
        }

        /// <summary>
        /// Called when <see cref="CurrentRecordManager.EndEdit"/> successfully finishes.
        /// </summary>
        /// <param name="success">True, if it is successfully finished; False, otherwise.</param>
        public virtual void OnEndEditComplete(bool success)
        {
        }

        /// <summary>
        /// Called when <see cref="CurrentRecordManager.CancelEdit"/> is called.
        /// </summary>
        /// <returns>True if <see cref="CurrentRecordManager.CancelEdit"/> can proceed; False if it should abort.</returns>
        public virtual bool OnCancelEditCalled()
        {
            return true;
        }

        /// <summary>
        /// Called when <see cref="CurrentRecordManager.CancelEdit"/> successfully finishes.
        /// </summary>
        /// <param name="success">True, if it is successfully finished; False, otherwise.</param>
        public virtual void OnCancelEditComplete(bool success)
        {
        }

        /// <summary>
        /// Called when <see cref="CurrentRecordManager.LeaveRecord"/> is called.
        /// </summary>
        /// <returns>True if <see cref="CurrentRecordManager.LeaveRecord"/> can proceed; False if it should abort.</returns>
        public virtual bool OnLeaveRecordCalled()
        {
            return true;
        }

        /// <summary>
        /// Called when <see cref="CurrentRecordManager.LeaveRecord"/> successfully finishes.
        /// </summary>
        /// <param name="success">True, if it is successfully finished; False, otherwise.</param>
        public virtual void OnLeaveRecordComplete(bool success)
        {
        }

        /// <summary>
        /// Called when <see cref="CurrentRecordManager.EnterRecord"/> is called.
        /// </summary>
        /// <returns>True if <see cref="CurrentRecordManager.EnterRecord"/> can proceed; False if it should abort.</returns>
        public virtual bool OnEnterRecordCalled()
        {
            return true;
        }

        /// <summary>
        /// Called when <see cref="CurrentRecordManager.EnterRecord"/> successfully finishes.
        /// </summary>
        /// <param name="success">True, if it is successfully finished; False, otherwise.</param>
        public virtual void OnEnterRecordComplete(bool success)
        {
        }

        #endregion

        #region IsCaption IsRecord GetRecord methods
        /// <summary>
        /// Checks if this is a caption element
        /// </summary>
        /// <returns>True if this is a caption; False, otherwise.</returns>
        public static bool IsCaption(Element el)
        {
            return el is CaptionRow || el is CaptionSection;
        }

        /// <summary>
        /// Checks if this is a record element
        /// </summary>
        /// <returns>True if this is a record; False otherwise.</returns>
        public static bool IsRecord(Element el)
        {
            return el is RecordRow || el is Record;
        }

        /// <summary>
        /// Checks if this is a column header element
        /// </summary>
        /// <returns>True if this is a column header; False, otherwise.</returns>
        public static bool IsColumnHeader(Element el)
        {
            return el is ColumnHeaderRow || el is ColumnHeaderSection;
        }

        /// <summary>
        /// Checks if this is a filterbar element
        /// </summary>
        /// <returns>True if this is a filter bar; False otherwise.</returns>
        public static bool IsFilterBar(Element el)
        {
            return el is FilterBarRow || el is FilterBarSection;
        }

        /// <summary>
        /// Checks if this is a caption element
        /// </summary>
        /// <returns>True if this is a caption; False, otherwise.</returns>
        public bool IsCaption()
        {
            return this is CaptionRow || this is CaptionSection;
        }

        /// <summary>
        /// Checks if this is a record element
        /// </summary>
        /// <returns>True if this is a record; False otherwise.</returns>
        public bool IsRecord()
        {
            return this is RecordRow || this is Record;
        }

        /// <summary>
        /// Checks if this is a column header element
        /// </summary>
        /// <returns>True if this is a column header; False, otherwise.</returns>
        public bool IsColumnHeader()
        {
            return this is ColumnHeaderRow || this is ColumnHeaderSection;
        }

        /// <summary>
        /// Checks if this is a filterbar element
        /// </summary>
        /// <returns>True if this is a filter bar; False otherwise.</returns>
        public bool IsFilterBar()
        {
            return this is FilterBarRow || this is FilterBarSection;
        }

        /// <summary>
        /// Gets the CaptionSection this element belongs to or the element itsself if it is a CaptionSection.
        /// </summary>
        /// <returns>Caption section.</returns>
        public static CaptionSection GetCaptionSection(Element el)
        {
            CaptionSection cs = el as CaptionSection;
            if (cs == null && el != null)
            {
                cs = el.ParentSection as CaptionSection;
            }

            return cs;
        }

        /// <summary>
        /// Gets the Record this element belongs to or the element itsself if it is a Record.
        /// </summary>
        /// <returns>returns Record.</returns>
        public static Record GetRecord(Element el)
        {
            Record cs = el as Record;
            if (cs == null && el != null)
            {
                cs = el.ParentRecord as Record;
            }

            return cs;
        }

        /// <summary>
        /// Gets the ColumnHeaderSection this element belongs to or the element itsself if it is a ColumnHeaderSection.
        /// </summary>
        /// <returns>Column header section.</returns>
        public static ColumnHeaderSection GetColumnHeaderSection(Element el)
        {
            ColumnHeaderSection cs = el as ColumnHeaderSection;
            if (cs == null && el != null)
            {
                cs = el.ParentSection as ColumnHeaderSection;
            }

            return cs;
        }

        /// <summary>
        /// Gets the FilterBarSection this element belongs to or the element itsself if it is a FilterBarSection.
        /// </summary>      
        /// <returns>returns FilterBarSection</returns>
        public static FilterBarSection GetFilterBar(Element el)
        {
            FilterBarSection cs = el as FilterBarSection;
            if (cs == null && el != null)
            {
                cs = el.ParentSection as FilterBarSection;
            }

            return cs;
        }

        /// <summary>
        /// Gets the CaptionSection this element belongs to or the element itsself if it is a CaptionSection.
        /// </summary>
        /// <returns>Caption section.</returns>
        public CaptionSection GetCaptionSection()
        {
            CaptionSection cs = this as CaptionSection;
            if (cs == null && this != null)
            {
                cs = this.ParentSection as CaptionSection;
            }

            return cs;
        }

        /// <summary>
        /// Gets the Record this element belongs to or the element itsself if it is a Record.
        /// </summary>
        /// <returns>returns Record.</returns>
        public Record GetRecord()
        {
            Record cs = this as Record;
            if (cs == null && this != null)
            {
                cs = this.ParentRecord as Record;
            }

            return cs;
        }

        /// <summary>
        /// Gets the ColumnHeaderSection this element belongs to or the element itsself if it is a ColumnHeaderSection.
        /// </summary>
        /// <returns>Column header section.</returns>
        public ColumnHeaderSection GetColumnHeaderSection()
        {
            ColumnHeaderSection cs = this as ColumnHeaderSection;
            if (cs == null && this != null)
            {
                cs = this.ParentSection as ColumnHeaderSection;
            }

            return cs;
        }

        /// <summary>
        /// Gets the FilterBarSection this element belongs to or the element itsself if it is a FilterBarSection.
        /// </summary>
        /// <returns>returns FilterBarSection</returns>
        public FilterBarSection GetFilterBar()
        {
            FilterBarSection cs = this as FilterBarSection;
            if (cs == null && this != null)
            {
                cs = this.ParentSection as FilterBarSection;
            }

            return cs;
        }
        #endregion
    }
}