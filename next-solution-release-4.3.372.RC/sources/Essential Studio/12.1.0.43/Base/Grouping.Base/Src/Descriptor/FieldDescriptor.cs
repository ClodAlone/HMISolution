//-------------------------------------------------------------------------------------------------
// <copyright file="FieldDescriptor.cs" company="syncfusion">
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
using System.Text;
using System.Globalization;
using System.ComponentModel.Design.Serialization;

using Syncfusion.Collections;
using Syncfusion.ComponentModel;
using Syncfusion.Diagnostics;
using Syncfusion.Grouping;
using Syncfusion.Windows.Forms; // IItemPropertiesSource

using ITreeTableSummary = Syncfusion.Collections.BinaryTree.ITreeTableSummary;
using System.Collections.Generic;

namespace Syncfusion.Grouping
{
    #region FieldDescriptorCollection
    /// <summary>
    /// A collection of <see cref="FieldDescriptor"/> fields with mapping information to columns of the underlying datasource.
    /// An instance of this collection is returned by the <see cref="TableDescriptor.Fields"/> property
    /// of a <see cref="TableDescriptor"/>.
    /// </summary>
    [ListBindableAttribute(false)]
    [EditorAttribute(typeof(GroupingCollectionEditor), typeof(System.Drawing.Design.UITypeEditor))]
    [TypeConverter(typeof(CustomTypeDescriptorConverter))]
    public class FieldDescriptorCollection : IDisposable, IList, ICloneable, IInsideCollectionEditorProperty, ICustomTypeDescriptor
    {
        internal ArrayList _inner;
        internal SortedList _sorted;
        internal TableDescriptor _tableDescriptor;
        internal int version;
        bool modified = false;
        bool readOnly = false;
        bool inReset = false;
        bool inEnsureInitialized = false;
        bool isReset = true;
        internal bool shouldPopulate = false;
        int expressionFieldVersion = -1;
        int unboundFieldVersion = -1;
        int relationVersion = -1;
        internal int tableDescriptorVersion = -1;
        int engineSourceListVersion = -1;
        internal bool insideCollectionEditor = false;
        bool inInitializeFrom = false;
        bool inInitializeFromChanged = false;

        bool disableShouldPopulate = false;

        /// <summary>
        /// Gets or sets whether collection should check for changes
        /// in engine schema or underlying datasource schema when EnsureInitialized gets called.
        /// </summary>
        [System.Xml.Serialization.XmlIgnore]
        [Browsable(false)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public bool ShouldPopulate
        {
            get
            {
                return shouldPopulate;
            }

            set
            {
                shouldPopulate = value;
            }
        }

        /// <summary>
        /// When called the ShouldPopulate property will be set true temporarily until
        /// the next EnsureInitialized call and then be reset again to optimize subsequent lookups.
        /// The Engine calls this method when schema changes occured (PropertyChanged was raised).
        /// </summary>
        public void EnableOneTimePopulate()
        {
            this.disableShouldPopulate = true;
            if (!Engine.InInitializeFrom)
            {
                this.shouldPopulate = true;
            }
        }

        Engine Engine
        {
            get
            {
                return this._tableDescriptor != null ? this._tableDescriptor.Engine : null;
            }
        }

        /// <internalonly/>
        [Syncfusion.Documentation.DocumentationExclude()]
        [EditorBrowsable(EditorBrowsableState.Never)]
        [System.Xml.Serialization.XmlIgnore]
        [Obsolete("Replaced with Engine.ShowNestedPropertiesFields")]
        public bool ExpandProperties
        {
            get
            {
                return Engine != null ? Engine.ShowNestedPropertiesFields : false;
            }

            set
            {
                if (Engine != null)
                {
                    Engine.ShowNestedPropertiesFields = value;
                }
            }
        }

        /// <internalonly/>
        [Syncfusion.Documentation.DocumentationExclude()]
        [EditorBrowsable(EditorBrowsableState.Never)]
        [System.Xml.Serialization.XmlIgnore]
        [Obsolete("Replaced with Engine.ShowRelationFields")]
        public bool PopulateForeignKeyFields
        {
            get
            {
                return Engine != null ? Engine.ShowRelationFields != ShowRelationFields.Hide : false;
            }

            set
            {
                if (Engine != null)
                {
                    Engine.ShowRelationFields = value ? ShowRelationFields.ShowAllRelatedFields : ShowRelationFields.Hide;
                }
            }
        }

        /// <summary>
        /// Occurs after a property in a nested element or the collection is changed.
        /// </summary>
        public event ListPropertyChangedEventHandler Changed;

        /// <summary>
        /// Occurs before a property in a nested element or the collection is changed.
        /// </summary>
        public event ListPropertyChangedEventHandler Changing;

        /// <summary>
        /// A Read-only and empty collection.
        /// </summary>
        public static readonly FieldDescriptorCollection Empty = new FieldDescriptorCollection(null);

        /// <summary>
        /// Returns string representation of this object.
        /// </summary>
        /// <returns>
        /// A <see cref="T:System.String"/> that represents the current <see cref="T:System.Object"/>.
        /// </returns>
        /// <override/>
        public override string ToString()
        {
            return String.Format("FieldDescriptorCollection: Count {0}, InsideColl {1}", Count, insideCollectionEditor);
        }

        /// <summary>
        /// Ensure type correctness when a new element is added to the collection.
        /// </summary>
        /// <param name="obj">The newly added object.</param>
        protected virtual void CheckType(object obj)
        {
            if (obj != null && !(obj is FieldDescriptor))
            {
                throw new ArgumentException("Wrong type");
            }
        }

        /// <summary>
        /// Gets / sets whether the collection is manipulated inside a collection editor.
        /// </summary>
        public bool InsideCollectionEditor
        {
            get
            {
                return insideCollectionEditor;
            }

            set
            {
#if DEBUG
                if (Switches.GroupingEngine.TraceVerbose)
                {
                    TraceUtil.TraceCurrentMethodInfo(this);
                }
#else
                ;
#endif
                if (insideCollectionEditor != value)
                {
                    insideCollectionEditor = value;
                }
            }
        }

        void IInsideCollectionEditorProperty.InitializeFrom(object other)
        {
            InitializeFrom((FieldDescriptorCollection)other);
        }

        /// <summary>
        /// Copies settings from another collection and raises <see cref="Changing"/> and <see cref="Changed"/>
        /// events if differences to the other collection are detected.
        /// </summary>
        /// <param name="other">The source collection.</param>
        public void InitializeFrom(FieldDescriptorCollection other)
        {
            ////inInitializeFrom = true;
            inInitializeFromChanged = false;

            bool savedshouldPopulateThis = shouldPopulate;
            bool savedshouldPopulateOther = other.shouldPopulate;
            this.shouldPopulate = false;
            other.shouldPopulate = false;

            try
            {
                int i;
                int count = Math.Min(Count, other.Count);

                _sorted = null;

                while (Count > other.Count)
                {
                    RemoveAt(Count - 1);
                }

                for (i = 0; i < count; i++)
                {
                    this[i].InitializeFrom(other[i]);
                }

                for (; i < other.Count; i++)
                {
                    Add(other[i].Clone());
                }

                _sorted = new SortedList();
                foreach (FieldDescriptor cd in this)
                {
                    if (!_sorted.ContainsKey(cd.Name))
                    {
                        _sorted.Add(cd.Name, cd);
                    }
                }

                version++;

                inInitializeFrom = false;
                if (inInitializeFromChanged)
                {
                    this.OnChanged(new ListPropertyChangedEventArgs(ListPropertyChangedType.Refresh, -1, null, null));
                }
            }
            finally
            {
                this.shouldPopulate = true;
                if (!savedshouldPopulateThis)
                {
                    this.disableShouldPopulate = true;
                }

                other.shouldPopulate = savedshouldPopulateOther;
            }
        }

        /// <overload>
        /// Initializes a new empty collection.
        /// </overload>
        /// <summary>
        /// Initializes a new empty collection.
        /// </summary>
        public FieldDescriptorCollection()
            : this(null)
        {
        }

        /// <summary>
        /// Initializes a new empty collection and attaches it to a <see cref="TableDescriptor"/>.
        /// </summary>
        /// <param name="tableDescriptor">Table Descriptor.</param>
        public FieldDescriptorCollection(TableDescriptor tableDescriptor)
        {
            this._inner = new ArrayList();
            this._sorted = new SortedList(); ////;new FieldDescriptorNameComparer());
            _tableDescriptor = tableDescriptor;
            if (_tableDescriptor != null)
            {
                if (!(this is ExpressionFieldDescriptorCollection))
                {
                    _tableDescriptor.ExpressionFields.Changed += new ListPropertyChangedEventHandler(ExpressionFields_Changed);
                }
            }
        }

        internal FieldDescriptorCollection(TableDescriptor tableDescriptor, FieldDescriptor[] fieldDescriptors)
            : this(tableDescriptor)
        {
            this.AddRange(fieldDescriptors);
        }

        /// <summary>
        /// Resets the collection to its default state. If the collection is bound to a <see cref="TableDescriptor"/>,
        /// the collection will autopopulate itself the next time an item inside the collection is accessed.
        /// </summary>
        public void Reset()
        {
            this.tableDescriptorVersion = -1;
            this.unboundFieldVersion = -1;
            this.relationVersion = -1;
            if (this.modified || _inner.Count > 0)
            {
                OnChanging(new ListPropertyChangedEventArgs(ListPropertyChangedType.Refresh, -1, null, null));
                inReset = true;
                isReset = true;
                _inner.Clear();
                if (_sorted != null)
                {
                    _sorted.Clear();
                }

                this.modified = false;
                OnChanged(new ListPropertyChangedEventArgs(ListPropertyChangedType.Refresh, -1, null, null));
                inReset = false;
            }

            isReset = true;
            this.modified = false;
            if (!shouldPopulate)
            {
                this.shouldPopulate = true;
                this.disableShouldPopulate = true;
            }
        }

        /// <summary>
        /// Resets the collection to its default state, autopopulates the collection, and marks the collection
        /// as modified. Call this method if you want to load the default items for the collection and then
        /// modify it (e.g. remove members from the auto-populated list).
        /// </summary>
        /// <example>
        /// <code lang="C#">
        /// gridGroupingControl1.TableDescriptor.Fields.LoadDefault();
        /// gridGroupingControl1.TableDescriptor.Fields.Remove("MyChildTable.ForeignCategoryID");
        /// </code>
        /// </example>
        public void LoadDefault()
        {
            if (this.IsModified)
            {
                Reset();
            }

            modified = false;
            this.EnsureInitialized(true);
            this.modified = true;
        }

        static bool allowAddRangeChangedWarning = true;

        /// <summary>Internal only.</summary>
        /// <internalonly/>
        [Syncfusion.Documentation.DocumentationExclude]
        public static bool AllowAddRangeChangedWarning
        {
            get
            {
                return allowAddRangeChangedWarning;
            }

            set
            {
                allowAddRangeChangedWarning = value;
            }
        }

        /// <summary>Internal only.</summary>
        /// <internalonly/>
        [Syncfusion.Documentation.DocumentationExclude]
        public static void ShowAddRangeChangedWarning(string colName)
        {
            if (!allowAddRangeChangedWarning)
            {
                return;
            }

            Console.WriteLine("Warning: With version 3.0.0.12, we changed the behavior of the {0}.AddRange and {0}.Add", colName);
            Console.WriteLine("methods when called the first time on a collection that was not previously modified.");
            Console.WriteLine("In previous versions the AddRange and Add methods called Clear() and marked the collection modified before adding the entry.");
            Console.WriteLine("The new behavior is to populate the collection from the datasource and add the new entry at the end of the collection.");
            Console.WriteLine("If you actually want to have the collection be cleared before you call Add or AddRange you need to");
            Console.WriteLine("explicitly call {0}.Clear().", colName);
            Console.WriteLine("To avoid this warning message, you can call {0}.LoadDefault() before adding or set FieldDescriptorCollection.AllowAddRangeChangedWarning = False.");
        }

        /// <summary>
        /// Adds multiple elements at the end of the collection.
        /// </summary>
        /// <param name="fieldDescriptors">The array with elements that should be added to the end of the collection.
        /// The array and its elements cannot be NULL references (Nothing in Visual Basic).
        /// </param>
        public void AddRange(FieldDescriptor[] fieldDescriptors)
        {
            foreach (FieldDescriptor fd in fieldDescriptors)
            {
                this.Add(fd);
            }
        }

        /// <summary>
        /// Raises the <see cref="Changing"/> event.
        /// </summary>
        /// <param name="e">A <see cref="ListPropertyChangedEventArgs" /> that contains the event data.</param>
        protected virtual void OnChanging(ListPropertyChangedEventArgs e)
        {
            if (!this.insideCollectionEditor && !this.inEnsureInitialized)
            {
#if DEBUG
                if (Switches.GroupingEngine.TraceVerbose)
                {
                    TraceUtil.TraceCurrentMethodInfo(this, e.Item, e.Index, version);
                }
#else
                ;
#endif

                if (Changing != null)
                {
                    Changing(this, e);
                }
            }
        }

        internal void RaisePropertyItemChanging(FieldDescriptor field, DescriptorPropertyChangedEventArgs e)
        {
            if (!this.InsideCollectionEditor && !this.inEnsureInitialized)
            {
                OnChanging(new ListPropertyChangedEventArgs(ListPropertyChangedType.ItemPropertyChanged, Find(field.Name), field, e.PropertyName, e));
#if DEBUG
                if (Switches.GroupingEngine.TraceVerbose)
                {
                    TraceUtil.TraceCurrentMethodInfo(this, field.Name, e.PropertyName);
                }
#else
                ;
#endif
            }
        }

        /// <summary>
        /// Raises the <see cref="Changed"/> event.
        /// </summary>
        /// <param name="e">A <see cref="ListPropertyChangedEventArgs" /> that contains the event data.</param>
        protected virtual void OnChanged(ListPropertyChangedEventArgs e)
        {
            version++;
            if (!inEnsureInitialized)
            {
                this.tableDescriptorVersion = -1;
            }

            if (!inEnsureInitialized && !inReset)
            {
                modified = true;

                if (!this.insideCollectionEditor)
                {
                    if (inInitializeFrom)
                    {
                        inInitializeFromChanged = true;
                        return;
                    }
#if DEBUG
                    if (Switches.GroupingEngine.TraceVerbose)
                    {
                        TraceUtil.TraceCurrentMethodInfo(this, e.Item, e.Index, version);
                    }
#else

                    ;
#endif
                    if (Changed != null)
                    {
                        Changed(this, e);
                    }

                    if (this._tableDescriptor != null && this._tableDescriptor.IsDesignTime())
                    {
                        this.EnsureInitialized(true);
                    }
                }
            }
        }

        internal void RaisePropertyItemChanged(FieldDescriptor field, DescriptorPropertyChangedEventArgs e)
        {
            if (e.PropertyName == "Name")
            {
                if (_sorted != null)
                {
                    int index = _sorted.IndexOfValue(field);
                    if (index != -1)
                    {
                        _sorted.RemoveAt(index);
                    }

                    _sorted.Add(field.Name, field);
                }
            }

            if (!this.InsideCollectionEditor && e.PropertyName != "PropertyDescriptor")
            {
                OnChanged(new ListPropertyChangedEventArgs(ListPropertyChangedType.ItemPropertyChanged, Find(field.Name), field, e.PropertyName, e));
#if DEBUG
                if (Switches.GroupingEngine.TraceVerbose)
                {
                    TraceUtil.TraceCurrentMethodInfo(this, field.Name, e.PropertyName);
                }
#else
                ;
#endif
            }
        }

        /// <summary>
        /// Determines if the specified PropertyDescriptor has nested properties.
        /// </summary>
        /// <param name="pd">The PropertyDescriptor to be checked.</param>
        /// <returns>True if nested properties are found; False otherwise.</returns>
        protected virtual bool IsComplexType(PropertyDescriptor pd)
        {
            return ListUtil.IsComplexType(pd);
        }

        ArrayList CreateNestedFieldDescriptors(string prefix, Type propertyType, int level)
        {
            ArrayList al = new ArrayList();

            int recLevel = this.Engine != null ? this.Engine.MaxNestedFieldRecurseLevel : Engine.AbortNestedFieldsRecursionLevel;
            if (level >= recLevel)
            {
                return al;
            }

            PropertyDescriptorCollection complexPdc = ListUtil.GetItemProperties(propertyType);
            if (complexPdc.Count > 0)
            {
                //// Make sure this is really a complex type in case IsComplexType
                //// failed to detect a type correctly.

                if (complexPdc["IsMarshalByRef"] == null)
                {
                    foreach (PropertyDescriptor nestedPd in complexPdc)
                    {
                        if (ListUtil.PropertyDescriptorIsARelation(nestedPd))
                        {
                            continue;
                        }

                        if (IsComplexType(nestedPd))
                        {
                            QueryShowNestedPropertiesFieldsEventArgs e = new QueryShowNestedPropertiesFieldsEventArgs(_tableDescriptor, nestedPd);
                            Engine.RaiseQueryShowNestedPropertiesFields(e);
                            if (!e.Cancel)
                            {
                                al.AddRange(CreateNestedFieldDescriptors(prefix + "." + nestedPd.Name, nestedPd.PropertyType, level + 1));
                                continue;
                            }
                        }

                        ////if (!ListUtil.PropertyDescriptorIsARelation(nestedPd)
                        ////    && !IsComplexType(nestedPd))
                        {
                            FieldDescriptor field = InternalCreateFieldDescriptor(prefix + "." + nestedPd.Name);
                            field.MappingName = prefix + "." + nestedPd.Name;
                            field.ReadOnly = nestedPd.IsReadOnly;
                            al.Add(field);
#if DEBUG
                            if (Switches.AutoPopulate.TraceVerbose)
                            {
                                TraceUtil.TraceCurrentMethodInfo("Add", field);
                            }
#else
                            ;
#endif
                        }
                    }
                }
            }

            return al;
        }

        /// <summary>
        /// Ensures the collection is initialized and auto-populates the collection on demand.
        /// </summary>
        /// <param name="populate">True if collection should auto-populate itself based on properties of
        /// the underlying datasource.</param>
        protected virtual void EnsureInitialized(bool populate)
        {
            if (inEnsureInitialized || !shouldPopulate || _tableDescriptor.Engine == null || _tableDescriptor.inOnInitializeItemProperties)
            {
                return;
            }

            if (disableShouldPopulate)
            {
                shouldPopulate = false;
            }

            _tableDescriptor.Engine.GetSourceList();

            // _tableDescriptor.ItemProperties getter might trigger InitializeItemProperties event
            // and increase _tableDescriptor.ItemPropertiesVersion.
            PropertyDescriptorCollection pdc = _tableDescriptor.ItemProperties;
            bool update = false;

            // compare versions
            if (this.tableDescriptorVersion != this._tableDescriptor.ItemPropertiesVersion
                || this.expressionFieldVersion != this._tableDescriptor.ExpressionFields.version
                || this.unboundFieldVersion != this._tableDescriptor.UnboundFields.version
                || this.relationVersion != this._tableDescriptor.Relations.Version
                || engineSourceListVersion != this._tableDescriptor.Engine.SourceListVersion)
            {
                this.tableDescriptorVersion = this._tableDescriptor.ItemPropertiesVersion;
                this.expressionFieldVersion = this._tableDescriptor.ExpressionFields.version;
                this.unboundFieldVersion = this._tableDescriptor.UnboundFields.version;
                this.relationVersion = this._tableDescriptor.Relations.Version;
                this.engineSourceListVersion = this._tableDescriptor.Engine.SourceListVersion;
                this.version++;

                if (!this.modified)
                {
                    isReset = true;
                }

                update = true;
            }

            if (!isReset || modified)
            {
                if (update)
                {
                    foreach (FieldDescriptor fd in this._inner)
                    {
                        fd.EnsureMapping(_tableDescriptor);
                    }
                }

                return;
            }

            inEnsureInitialized = true;

            if (_tableDescriptor == null || !populate)
            {
            }
            else
            {
                TraceUtil.TraceCalledFromIf(Switches.AutoPopulate.TraceVerbose, 10, _tableDescriptor, version);
                isReset = false;
                FieldDescriptorCollection fdc = new FieldDescriptorCollection();
                if (pdc == null || pdc.Count == 0)
                {
                }
                else
                {
                    foreach (PropertyDescriptor pd in pdc)
                    {
                        try
                        {
                            if (!ListUtil.PropertyDescriptorIsARelation(pd))
                            {
                                bool isComplex = false;
                                if (Engine.ShowNestedPropertiesFields)
                                {
                                    if (IsComplexType(pd))
                                    {
                                        QueryShowNestedPropertiesFieldsEventArgs e = new QueryShowNestedPropertiesFieldsEventArgs(_tableDescriptor, pd);
                                        Engine.RaiseQueryShowNestedPropertiesFields(e);
                                        if (!e.Cancel)
                                        {
                                            ArrayList al = CreateNestedFieldDescriptors(pd.Name, pd.PropertyType, 0);
                                            foreach (FieldDescriptor field in al)
                                            {
                                                field.Name = field.Name.Replace('.', '_');
                                                QueryShowFieldEventArgs qe = new QueryShowFieldEventArgs(_tableDescriptor, field);
                                                if (Engine != null)
                                                {
                                                    Engine.RaiseQueryShowField(qe);
                                                }

                                                if (!qe.Cancel)
                                                {
                                                    isComplex = true;
                                                    fdc.Add(field);
                                                }
                                            }
                                        }
                                    }
                                }

                                FieldDescriptor newField = InternalCreateFieldDescriptor(pd.Name);
                                newField.MappingName = pd.Name;
                                newField.ReadOnly = pd.IsReadOnly;
                                newField.Hide = isComplex;
                                newField.isObjectReferenceField = isComplex;
                                QueryShowFieldEventArgs _qe = new QueryShowFieldEventArgs(_tableDescriptor, newField);
                                Engine.RaiseQueryShowField(_qe);
                                if (!_qe.Cancel)
                                {
                                    fdc.Add(newField);
                                }
#if DEBUG
                                if (Switches.AutoPopulate.TraceVerbose)
                                {
                                    TraceUtil.TraceCurrentMethodInfo("Add", newField);
                                }
#else
                                ;
#endif
                            }
                        }
                        catch
                        {
                        }
                    }

                    foreach (ExpressionFieldDescriptor fieldDescriptor in _tableDescriptor.ExpressionFields)
                    {
                        try
                        {
                            FieldDescriptor field = InternalCreateFieldDescriptor(fieldDescriptor.Name);
                            field.MappingName = fieldDescriptor.Name;
                            field.ReadOnly = true;
                            QueryShowFieldEventArgs qe = new QueryShowFieldEventArgs(_tableDescriptor, field);
                            if (Engine != null)
                            {
                                Engine.RaiseQueryShowField(qe);
                            }

                            if (!qe.Cancel)
                            {
                                fdc.Add(field);
                            }
#if DEBUG
                            if (Switches.AutoPopulate.TraceVerbose)
                            {
                                TraceUtil.TraceCurrentMethodInfo("Add", field);
                            }
#else
                            ;
#endif
                        }
                        catch
                        {
                        }
                    }

                    foreach (FieldDescriptor fieldDescriptor in _tableDescriptor.UnboundFields)
                    {
                        try
                        {
                            FieldDescriptor field = InternalCreateFieldDescriptor(fieldDescriptor.Name);
                            field.MappingName = fieldDescriptor.Name;
                            QueryShowFieldEventArgs qe = new QueryShowFieldEventArgs(_tableDescriptor, field);
                            Engine.RaiseQueryShowField(qe);
                            if (!qe.Cancel)
                            {
                                fdc.Add(field);
                            }
#if DEBUG
                            if (Switches.AutoPopulate.TraceVerbose)
                            {
                                TraceUtil.TraceCurrentMethodInfo("Add", field);
                            }
#else
                            ;
#endif
                        }
                        catch
                        {
                        }
                    }

                    if (Engine.ShowRelationFields != ShowRelationFields.Hide)
                    {
                        foreach (RelationDescriptor rd in _tableDescriptor.Relations)
                        {
                            //// ForeignListItems
                            if (rd.RelationKind == RelationKind.ForeignKeyReference
                                || rd.RelationKind == RelationKind.ForeignKeyKeyWords)    
                            {
                                QueryShowRelationFieldsEventArgs e = new QueryShowRelationFieldsEventArgs(_tableDescriptor, rd, Engine.ShowRelationFields);
                                if (Engine != null)
                                {
                                    Engine.RaiseQueryShowRelationDisplayFields(e);
                                }

                                if (e.ShowRelationFields != ShowRelationFields.Hide)
                                {
                                    rd.SetParentTableDescriptor(this._tableDescriptor);
                                    rd.EnsureInitialized();  // this is gonna reenter myself (ParentTableDescriptor.Fields.Count is count - but we'll just ignore that and return 0)
                                    foreach (FieldDescriptor fieldDescriptor in rd.ChildTableDescriptor.Fields)
                                    {
                                        string name = rd.Name + "." + fieldDescriptor.Name;
                                        FieldDescriptor field = InternalCreateFieldDescriptor(name);
                                        field.MappingName = rd.Name + "." + fieldDescriptor.Name;
                                        field.Name = name.Replace('.', '_');
                                        field.Hide = fieldDescriptor.Hide;
                                        if (e.ShowRelationFields == ShowRelationFields.ShowDisplayFieldsOnly)
                                        {
                                            if (rd.RelationKeys.Count > 0)
                                            {
                                                field.parentFieldDescriptor = fdc[rd.RelationKeys[rd.RelationKeys.Count - 1].ParentKeyFieldName];
                                                if (field.parentFieldDescriptor != null)
                                                {
                                                    field.parentFieldDescriptor.Hide = true; //// Hide foreign keys
                                                }
                                            }
                                        }

                                        QueryShowFieldEventArgs qe = new QueryShowFieldEventArgs(_tableDescriptor, field);
                                        if (Engine != null)
                                        {
                                            Engine.RaiseQueryShowField(qe);
                                        }

                                        if (!qe.Cancel)
                                        {
                                            fdc.Add(field);
                                        }
#if DEBUG
                                        if (Switches.AutoPopulate.TraceVerbose)
                                        {
                                            TraceUtil.TraceCurrentMethodInfo("Add", field);
                                        }
#else
                                        ;
#endif
                                    }

                                    if (e.ShowRelationFields == ShowRelationFields.ShowDisplayFieldsOnly)
                                    {
                                        foreach (RelationKeyDescriptor rdKey in rd.RelationKeys)
                                        {
                                            ////                                FieldDescriptor field = fdc[rdKey.ParentKeyFieldName];
                                            ////                                if (field != null)
                                            ////                                    field.Hide = true;  // Hide foreign keys

                                            FieldDescriptor field = fdc[rd.Name + "_" + rdKey.ChildKeyFieldName];
                                            if (field != null)
                                            {
                                                field.Hide = true;  //// Hide foreign keys
                                            }
                                        }
                                    }
                                }
                            }
                            else if (rd.RelationKind == RelationKind.ListItemReference)
                            {
                                if (rd.RelationKeys.Count > 0)
                                {
                                    throw new Exception("No relation key supported with RelationKind.ListItemReference.");
                                }
                            }
                        }
                    }

                    RelationDescriptor parentRelation = this._tableDescriptor.ParentRelation;
                    if (parentRelation != null && (parentRelation.RelationKind == RelationKind.RelatedMasterDetails
                        || parentRelation.RelationKind == RelationKind.ForeignKeyReference))
                    {
                        QueryShowRelationFieldsEventArgs e = new QueryShowRelationFieldsEventArgs(this._tableDescriptor.ParentRelation.ParentTableDescriptor, this._tableDescriptor.ParentRelation, Engine.ShowRelationFields);
                        if (Engine != null)
                        {
                            Engine.RaiseQueryShowRelationDisplayFields(e);
                        }

                        if (e.ShowRelationFields == ShowRelationFields.ShowDisplayFieldsOnly)
                        {
                            int count = parentRelation.RelationKeys.Count;
                            if (parentRelation.RelationKind == RelationKind.ForeignKeyReference)
                            {
                                count--;
                            }

                            for (int ri = 0; ri < count; ri++)
                            {
                                RelationKeyDescriptor sd = parentRelation.RelationKeys[ri];
                                FieldDescriptor field = fdc[sd.ChildKeyFieldName];
                                if (field != null)
                                {
                                    field.Hide = true;  // Hide foreign keys
                                }
                            }
                        }
                    }
                }

                this.InitializeFrom(fdc);

                modified = false;

                foreach (FieldDescriptor fd in this._inner)
                {
                    fd.EnsureMapping(_tableDescriptor);
                }

                if (Engine.HelpTracing)
                {
                    TraceUtil.TraceCurrentMethodInfo(this._tableDescriptor, pdc.Count);
                    TraceUtil.TraceCalledFrom(20);
                }

                this.version++;
                this.tableDescriptorVersion = this._tableDescriptor.ItemPropertiesVersion;
                this.expressionFieldVersion = this._tableDescriptor.ExpressionFields.version;
                this.unboundFieldVersion = this._tableDescriptor.UnboundFields.version;
                this.relationVersion = this._tableDescriptor.Relations.Version;
                this.engineSourceListVersion = this._tableDescriptor.Engine.SourceListVersion;
            }

            inEnsureInitialized = false;
        }

        internal void EnsureMapping(FieldDescriptor field)
        {
            if (this.tableDescriptorVersion != this._tableDescriptor.ItemPropertiesVersion
                || this.expressionFieldVersion != this._tableDescriptor.ExpressionFields.version
                || this.unboundFieldVersion != this._tableDescriptor.UnboundFields.version
                || this.relationVersion != this._tableDescriptor.Relations.Version)
            {
                this.version++;
                this.tableDescriptorVersion = this._tableDescriptor.ItemPropertiesVersion;
                this.expressionFieldVersion = this._tableDescriptor.ExpressionFields.version;
                this.unboundFieldVersion = this._tableDescriptor.UnboundFields.version;
                this.relationVersion = this._tableDescriptor.Relations.Version;
            }

            field.EnsureMapping(_tableDescriptor);
        }

        /// <summary>
        /// Creates a new empty field descriptor with the specified name.
        /// </summary>
        /// <param name="name">The name of the new field descriptor.</param>
        /// <returns>A new FieldDescriptor.</returns>
        protected virtual FieldDescriptor InternalCreateFieldDescriptor(string name)
        {
            return new FieldDescriptor(name);
        }

        /// <summary>
        /// Creates a copy of the collection and all its elements.
        /// </summary>
        /// <returns>A copy of the collection and all its elements.</returns>
        public FieldDescriptorCollection Clone()
        {
            return InternalClone();
        }

        /// <summary>
        /// Creates a copy of this collection and all its inner elements. This method is called from Clone.
        /// </summary>
        /// <returns>A copy of the collection and all its elements.</returns>
        protected FieldDescriptorCollection InternalClone()
        {
            int count = Count;
            FieldDescriptor[] fieldDescriptors = new FieldDescriptor[count];
            for (int n = 0; n < count; n++)
            {
                fieldDescriptors[n] = this[n].Clone();
            }

            FieldDescriptorCollection c = CreateCollection(_tableDescriptor, fieldDescriptors);
            c.modified = modified;
            c.version = version + 1000;
            c.engineSourceListVersion = -1;
            c.expressionFieldVersion = -1;
            c.relationVersion = -1;
            c.tableDescriptorVersion = -1;
            c.unboundFieldVersion = -1;
            return c;
        }

        /// <summary>
        /// Called from InternalClone to create a new collection and attach it to the specified table descriptor
        /// and insert the specified fields. The fields have already been cloned.
        /// </summary>
        /// <param name="td">The table descriptor.</param>
        /// <param name="fieldDescriptors">The cloned field descriptors.</param>
        /// <returns>A new FieldDescriptorCollection.</returns>
        protected virtual FieldDescriptorCollection CreateCollection(TableDescriptor td, FieldDescriptor[] fieldDescriptors)
        {
            return new FieldDescriptorCollection(td, fieldDescriptors);
        }

        bool inDispose = false;

        /// <summary>Compares two descriptors.</summary>
        /// <param name="obj">Field descriptor to compare.</param>
        /// <returns>True if the descriptors are equal; False otherwise.</returns>
        /// <override/>
        public override bool Equals(object obj)
        {
            if (inDispose)
            {
                return Object.ReferenceEquals(this, obj);
            }

            if (this == null && obj == null)
            {
                return true;
            }
            else if (this == null)
            {
                return false;
            }
            else if (!(obj is FieldDescriptorCollection))
            {
                return false;
            }

            return InternalEquals((FieldDescriptorCollection)obj);
        }

        /// <summary>Serves as a hash function.</summary>
        /// <returns>Hash code.</returns>
        /// <override/>
        public override int GetHashCode()
        {
            return base.GetHashCode();
        }

        /// <summary>
        /// The version number of this collection. The version is increased each time the
        /// collection or an element within the collection is modified. When getting the version,
        /// <see cref="EnsureInitialized"/> is called to ensure the collection is auto-populated
        /// if needed.
        /// </summary>
        public int Version
        {
            get
            {
                EnsureInitialized(true);
                return version;
            }

            set
            {
                version = value;
            }
        }

        /// <summary>
        /// Determines if the collection was modified from its default state.
        /// </summary>
        public bool IsModified
        {
            get
            {
                return modified;
            }
        }

        /// <summary>
        /// Marks the collection as modified and avoids auto-population.
        /// </summary>
        public void Modify()
        {
            this.modified = true;
        }

        /// <summary>
        /// Compares each element with the element of another collection.
        /// </summary>
        /// <param name="other">The collection to compare to.</param>
        /// <returns>True if all elements are equal and in the same order; False otherwise.</returns>
        protected bool InternalEquals(FieldDescriptorCollection other)
        {
            int count = Count;
            if (other.Count != count)
            {
                return false;
            }

            for (int n = 0; n < count; n++)
            {
                if (!this[n].Equals(other[n]))
                {
                    return false;
                }
            }

            return true;
        }

        /// <summary>
        /// Gets / sets the element at the zero-based index.
        /// </summary>
        public FieldDescriptor this[int index]
        {
            get
            {
                if (index < 0 || index >= Count)
                {
                    throw new ArgumentOutOfRangeException();
                }

                return (FieldDescriptor)_inner[index];
            }

            set
            {
                if (readOnly)
                {
                    throw new InvalidOperationException("Collection is Read-only.");
                }

                if (value == null)
                {
                    throw new ArgumentNullException();
                }

                CheckType(value);

                if (!modified)
                {
                    EnsureInitialized(false);
                }

                if (_inner[index] != value)
                {
                    OnChanging(new ListPropertyChangedEventArgs(ListPropertyChangedType.ItemChanged, index, value, null));
                    version++;
                    if (_sorted != null)
                    {
                        _sorted.Remove(this[index].Name);
                    }

                    _inner[index] = value;
                    value.index = index;
                    value.SetCollection(this);
                    if (_sorted != null)
                    {
                        _sorted.Add(value.Name, value);
                    }

                    OnChanged(new ListPropertyChangedEventArgs(ListPropertyChangedType.ItemChanged, index, value, null));
                }
            }
        }

        /// <summary>
        /// Gets / sets the element with the specified name.
        /// </summary>
        public FieldDescriptor this[string name]
        {
            get
            {
                if (!modified)
                {
                    EnsureInitialized(true);
                }

                int index = Find(name);
                if (index == -1)
                {
                    return null;
                }

                return (FieldDescriptor)_inner[index];
            }

            set
            {
                if (readOnly)
                {
                    throw new InvalidOperationException("Collection is Read-only.");
                }

                if (value == null)
                {
                    throw new ArgumentNullException();
                }

                CheckType(value);

                if (!modified)
                {
                    EnsureInitialized(false);
                }

                int index = Find(name);
                value.Name = name;
                if (index == -1)
                {
                    Add(value);
                }
                else
                {
                    this[index] = value;
                }
            }
        }

        internal int Find(string name)
        {
            if (_sorted != null)
            {
                FieldDescriptor cd = (FieldDescriptor)_sorted[name];

                if (cd == null)
                {
                    bool b = false;

                    if (name.IndexOf(".") != -1)
                    {
                        b = true;
                        cd = (FieldDescriptor)_sorted[name.Replace(".", "_")];  // Backward compatibility
                    }
                    else if (name.IndexOf("_") != -1)
                    {
                        b = true;
                        cd = (FieldDescriptor)_sorted[name.Replace("_", ".")];  // Backward compatibility
                    }

                    if (cd == null && b)
                    {
                        string s = name.Replace(".", "_");

                        foreach (FieldDescriptor fd in _inner)
                        {
                            string t = fd.Name.Replace(".", "_");
                            if (t == s)
                            {
                                cd = fd;
                                break;
                            }
                        }
                    }
                }

                if (cd != null)
                {
                    return cd.index;
                }
            }

            return -1;
        }

        internal int Find(PropertyDescriptor pd)
        {
            return Find(pd.Name);
        }

        /// <overload>
        /// Determines if the element belongs to this collection.
        /// </overload>
        /// <summary>
        /// Determines if the element belongs to this collection.
        /// </summary>
        /// <param name="value">The Object to locate in the collection. The value can be a NULL reference (Nothing in Visual Basic).</param>
        /// <returns>True if item is found in the collection; otherwise, False.</returns>
        /// <remarks>
        /// The method
        /// ensures that the collection is in sync with the underlying
        /// table if changes have been made to the table or the TableDescriptor.
        /// <para/>
        /// The method calls <see cref="EnsureInitialized"/>.
        /// </remarks>
        public bool Contains(FieldDescriptor value)
        {
            if (value == null)
            {
                return false;
            }

            CheckType(value);

            if (!modified)
            {
                EnsureInitialized(true);
            }

            return _sorted != null && (_sorted.Contains(value.Name) || (value.Name.IndexOf(".") != -1 && _sorted.Contains(value.Name.Replace(".", "_"))));
        }

        /// <summary>
        /// Determines if an element with the specified name belongs to this collection.
        /// </summary>
        /// <param name="name">The name of the element to locate in the collection. </param>
        /// <returns>True if item is found in the collection; otherwise, False.</returns>
        public bool Contains(string name)
        {
            if (!modified)
            {
                EnsureInitialized(true);
            }

            return _sorted != null && (_sorted.Contains(name) || (name.IndexOf(".") != -1 && _sorted.Contains(name.Replace(".", "_")))); 
        }

        /// <summary>
        /// Returns the zero-based index of the occurrence of the element in the collection.
        /// </summary>
        /// <param name="value">The element to locate in the collection. The value can be a NULL reference (Nothing in Visual Basic). </param>
        /// <returns>The zero-based index of the occurrence of the element within the entire collection, if found; otherwise, -1.</returns>
        public int IndexOf(FieldDescriptor value)
        {
            if (!modified)
            {
                EnsureInitialized(true);
            }

            if (!Contains(value))
            {
                return -1;
            }

            return this[value.Name].index;
        }

        /// <summary>
        /// Searches for the element with the specified name.
        /// </summary>
        /// <param name="name">The name of the element to locate in the collection. </param>
        /// <returns>The zero-based index of the occurrence of the element with matching name within the entire collection, if found; otherwise, -1.</returns>
        public int IndexOf(string name)
        {
            if (!modified)
            {
                EnsureInitialized(true);
            }

            return Find(name);
        }

        /// <summary>
        /// Copies the entire collection to a compatible one-dimensional array, starting at the specified index of the target array.
        /// </summary>
        /// <param name="array">The one-dimensional array that is the destination of the elements copied from ArrayList. The array must have zero-based indexing. </param>
        /// <param name="index">The zero-based index in array at which copying begins. </param>
        public void CopyTo(FieldDescriptor[] array, int index)
        {
            int n = 0;
            foreach (FieldDescriptor item in this)
            {
                array[index + n] = item;
                n++;
            }
        }

        /// <summary>
        /// Not supported.
        /// </summary>
        FieldDescriptorCollection SyncRoot
        {
            get
            {
                throw new NotSupportedException();
            }
        }

        /// <summary>
        /// Returns an enumerator for the entire collection.
        /// </summary>
        /// <returns>An Enumerator for the entire collection.</returns>
        /// <remarks>Enumerators only allow reading the data in the collection.
        /// Enumerators cannot be used to modify the underlying collection.</remarks>
        public FieldDescriptorCollectionEnumerator GetEnumerator()
        {
            return new FieldDescriptorCollectionEnumerator(this);
        }

        /// <summary>
        /// Inserts a descriptor element into the collection at the specified index.
        /// </summary>
        /// <param name="index">The zero-based index at which the element should be inserted.</param>
        /// <param name="value">The element to insert. The value must not be a NULL reference (Nothing in Visual Basic). </param>
        /// <remarks>
        /// The method
        /// ensures that the collection is in sync with the underlying
        /// table if changes have been made to the table or the TableDescriptor.
        /// </remarks>
        public void Insert(int index, FieldDescriptor value)
        {
            if (readOnly)
            {
                throw new InvalidOperationException("Collection is Read-only.");
            }

            if (value == null)
            {
                throw new ArgumentNullException();
            }

            CheckType(value);

            if (!modified)
            {
                this.EnsureInitialized(true);
            }

            OnChanging(new ListPropertyChangedEventArgs(ListPropertyChangedType.Insert, index, value, null));
            version++;
            _inner.Insert(index, value);
            if (_sorted != null)
            {
                _sorted.Add(value.Name, value);
            }

            value.SetCollection(this);
            for (int n = index; n < Count; n++)
            {
                this[n].index = n;
            }

            OnChanged(new ListPropertyChangedEventArgs(ListPropertyChangedType.Insert, index, value, null));
        }

        /// <summary>
        /// Removes the specified descriptor element from the collection.
        /// </summary>
        /// <param name="value">The element to remove from the collection. If the value is NULL or the element is not contained
        /// in the collection, the method will do nothing.</param>
        /// <remarks>
        /// The method
        /// ensures that the collection is in sync with the underlying
        /// table if changes have been made to the table or the TableDescriptor.
        /// </remarks>
        public void Remove(FieldDescriptor value)
        {
            if (readOnly)
            {
                throw new InvalidOperationException("Collection is Read-only.");
            }

            if (value == null)
            {
                return;
            }

            CheckType(value);

            if (!modified)
            {
                this.EnsureInitialized(true);
            }

            int index = IndexOf(value);
            OnChanging(new ListPropertyChangedEventArgs(ListPropertyChangedType.Remove, index, value, null));

            version++;
            if (_sorted != null)
            {
                _sorted.Remove(value.Name);
            }

            _inner.Remove(value);
            for (int n = index; n < Count; n++)
            {
                this[n].index = n;
            }

            OnChanged(new ListPropertyChangedEventArgs(ListPropertyChangedType.Remove, index, value, null));
        }

        /// <summary>
        /// Allow GridColumnDescriptorCollection to add a value without increasing version of this collection.
        /// </summary>
        /// <param name="name">The Field name.</param>
        /// <param name="silent">Param Not used.</param>
        /// <returns>Returns the index of the record that has been added internally</returns>
        [Syncfusion.Documentation.DocumentationExclude()]
        public int InternalAdd(string name, bool silent)
        {
            FieldDescriptor value = this.InternalCreateFieldDescriptor(name);

            // If value is added with a name that already exists, replace 
            // the old value with the new descriptor.
            int index = -1;
            if (value.Name != null && value.Name.Length > 0)
            {
                index = IndexOf(value.Name);
                if (index != -1)
                {
                    _inner[index] = value;
                    _sorted[value.Name] = value;
                }
            }

            if (index == -1)
            {
                index = _inner.Add(value);
                if (_sorted != null)
                {
                    _sorted.Add(value.Name, value);
                }
            }

            value.index = index;
            value.SetCollection(this);
            return index;
        }

        /// <summary>
        /// Adds an object to the end of the collection.
        /// </summary>
        /// <param name="value">The element to be added to the end of the collection. The value must not be a NULL reference (Nothing in Visual Basic). </param>
        /// <returns>The zero-based collection index at which the value has been added.</returns>
        public int Add(FieldDescriptor value)
        {
            if (readOnly)
            {
                throw new InvalidOperationException("Collection is read only");
            }

            if (value == null)
            {
                throw new ArgumentNullException();
            }

            CheckType(value);
            
            if (!this.inEnsureInitialized)
            {
                OnChanging(new ListPropertyChangedEventArgs(ListPropertyChangedType.Add, -1, value, null));

                // Changed Add behavior for 3.0.0.12 - user should call explicitly Clear()
                // if collection should be reset before adding fields.
                this.EnsureInitialized(true);
                if (!this.modified && this._inner.Count > 0)
                {
                    if (Contains(value.Name) && Engine.VersionInfo.CompareTo("3.0.0.12") <= 0)
                    {
                        FieldDescriptorCollection.ShowAddRangeChangedWarning("Fields");
                    }
                }
            }

            // If value is added with a name that already exists, replace 
            // the old value with the new descriptor.
            int index = -1;
            if (value.Name != null && value.Name.Length > 0)
            {
                index = IndexOf(value.Name);
                if (index != -1)
                {
                    _inner[index] = value;
                    _sorted[value.Name] = value;
                }
            }
            else
            {
                SuggestName(value);
            }

            if (index == -1)
            {
                index = _inner.Add(value);
                if (_sorted != null)
                {
                    _sorted.Add(value.Name, value);
                }
            }

            value.index = index;
            value.SetCollection(this);

            if (!this.inEnsureInitialized)
            {
                OnChanged(new ListPropertyChangedEventArgs(ListPropertyChangedType.Add, index, value, null));
            }

            return index;
        }

        /// <summary>
        /// Called to get a new default name when a new field descriptor is created (e.g. when pressing "Add" in a collection editor).
        /// </summary>
        /// <param name="value">The field descriptor to be named.</param>
        protected internal virtual void SuggestName(FieldDescriptor value)
        {
            if (_tableDescriptor != null && (value.MappingName == null || value.MappingName.Length == 0))
            {
                PropertyDescriptorCollection pdc = this._tableDescriptor.ItemProperties;
                if (pdc.Count > this.Count)
                {
                    foreach (PropertyDescriptor pd in pdc)
                    {
                        if (IndexOf(pd.Name) == -1)
                        {
                            value.MappingName = pd.Name;
                            return;
                        }
                    }
                }
            }

            int n = 1;
            foreach (FieldDescriptor col in this)
            {
                if (col.Name.StartsWith("Column "))
                {
                    double d;
                    if (double.TryParse(col.Name.Substring("Column ".Length), System.Globalization.NumberStyles.Number, null, out d))
                    {
                        n = (int)d + 1;
                    }
                }
            }

            value.Name = "Column " + n.ToString();
            value.nameModified = false;
        }

        /// <summary>
        /// Adds an object to the end of the collection.
        /// </summary>
        /// <param name="name">The name of the element to be added to the end of the collection. The value must not be a NULL reference (Nothing in Visual Basic). </param>
        /// <returns>The zero-based collection index at which the value has been added.</returns>
        public int Add(string name)
        {
            return InternalAdd(name);
        }

        /// <summary>
        /// Called from Add(string name) to create a new field descriptor with the given name.
        /// </summary>
        /// <param name="name">The name of the new field descriptor.</param>
        /// <returns>A new field descriptor.</returns>
        protected virtual int InternalAdd(string name)
        {
            return Add(this.InternalCreateFieldDescriptor(name));
        }

        /// <summary>
        /// Removes the specified descriptor element with the specified name from the collection.
        /// </summary>
        /// <param name="name">The name of the element to remove from the collection. If no element with that name is found
        /// in the collection, the method will do nothing.</param>
        /// <remarks>
        /// The method
        /// ensures that the collection is in sync with the underlying
        /// table if changes have been made to the table or the TableDescriptor.
        /// </remarks>
        public void Remove(string name)
        {
            if (readOnly)
            {
                throw new InvalidOperationException("Collection is Read-only.");
            }

            if (!modified)
            {
                this.EnsureInitialized(true);
            }

            int index = Find(name);
            if (index != -1)
            {
                RemoveAt(index);
            }
        }

        /// <summary>
        /// Removes the element at the specified index of the collection.
        /// </summary>
        /// <param name="index">The zero-based index of the element to remove. </param>
        public void RemoveAt(int index)
        {
            if (readOnly)
            {
                throw new InvalidOperationException("Collection is Read-only.");
            }

            if (!modified)
            {
                this.EnsureInitialized(true);
            }

            FieldDescriptor value = (FieldDescriptor)_inner[index];
            OnChanging(new ListPropertyChangedEventArgs(ListPropertyChangedType.Remove, index, value, null));
            version++;
            if (_sorted != null)
            {
                _sorted.Remove(value.Name);
            }

            _inner.RemoveAt(index);
            for (int n = index; n < Count; n++)
            {
                this[n].index = n;
            }

            OnChanged(new ListPropertyChangedEventArgs(ListPropertyChangedType.Remove, index, value, null));
        }

        /// <summary>
        /// Disposes the object and collection items.
        /// </summary>
        public void Dispose()
        {
            inDispose = true;
            if (!(this is ExpressionFieldDescriptorCollection))
            {
                _tableDescriptor.ExpressionFields.Changed -= new ListPropertyChangedEventHandler(ExpressionFields_Changed);
            }

            foreach (DescriptorBase db in _inner)
            {
                db.Dispose();
            }

            _inner.Clear();
            if (_sorted != null)
            {
                _sorted.Clear();
            }

            _sorted = null;
            inDispose = false;
            GC.SuppressFinalize(this);
        }

        /// <summary>
        /// Removes all elements from the collection.
        /// </summary>
        public void Clear()
        {
            if (readOnly)
            {
                throw new InvalidOperationException("Collection is Read-only.");
            }

            if (_inner.Count > 0)
            {
                if (!modified)
                {
                    this.EnsureInitialized(false);
                }

                OnChanging(new ListPropertyChangedEventArgs(ListPropertyChangedType.Refresh, -1, null, null));
                version++;
                _inner.Clear();
                if (_sorted != null)
                {
                    _sorted.Clear();
                }

                OnChanged(new ListPropertyChangedEventArgs(ListPropertyChangedType.Refresh, -1, null, null));
            }

            this.modified = true;
        }
        
        /// <summary>
        /// Determines if the collection is Read-only.
        /// </summary>
        public bool IsReadOnly
        {
            get
            {
                return readOnly;
            }
        }

        /// <summary>
        /// Returns normally False since this collection has no fixed size. Only when it is Read-only,
        /// IsFixedSize returns True.
        /// </summary>
        public bool IsFixedSize
        {
            get
            {
                return readOnly;
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
        /// The method calls <see cref="EnsureInitialized"/>.
        /// </remarks>
        public int Count
        {
            get
            {
                if (!this.modified)
                {
                    this.EnsureInitialized(true);
                }

                return _inner.Count;
            }
        }

        #region ICloneable Private Members
        object ICloneable.Clone()
        {
            return Clone();
        }
        #endregion

        #region IList Private Members

        object IList.this[int index]
        {
            get
            {
                return this[index];
            }

            set
            {
                this[index] = (FieldDescriptor)value;
            }
        }

        void IList.Insert(int index, object value)
        {
            Insert(index, (FieldDescriptor)value);
        }

        void IList.Remove(object value)
        {
            Remove((FieldDescriptor)value);
        }

        bool IList.Contains(object value)
        {
            return Contains((FieldDescriptor)value);
        }

        int IList.IndexOf(object value)
        {
            return IndexOf((FieldDescriptor)value);
        }

        int IList.Add(object value)
        {
            return Add((FieldDescriptor)value);
        }

        #endregion

        #region ICollection Private Members

        void ICollection.CopyTo(Array array, int index)
        {
            CopyTo((FieldDescriptor[])array, index);
        }

        object ICollection.SyncRoot
        {
            get
            {
                return null;
            }
        }

        #endregion

        #region IEnumerable Private Members

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        #endregion

        private void ExpressionFields_Changed(object sender, ListPropertyChangedEventArgs e)
        {
            this.tableDescriptorVersion = -1;
        }

        #region ICustomTypeDescriptor
        AttributeCollection ICustomTypeDescriptor.GetAttributes()
        {
            return TypeDescriptor.GetAttributes(this, true);
        }

        string ICustomTypeDescriptor.GetClassName()
        {
            return TypeDescriptor.GetClassName(this, true);
        }

        string ICustomTypeDescriptor.GetComponentName()
        {
            return TypeDescriptor.GetComponentName(this, true);
        }

        TypeConverter ICustomTypeDescriptor.GetConverter()
        {
            return TypeDescriptor.GetConverter(this, true);
        }

        EventDescriptor ICustomTypeDescriptor.GetDefaultEvent()
        {
            return TypeDescriptor.GetDefaultEvent(this, true);
        }

        PropertyDescriptor ICustomTypeDescriptor.GetDefaultProperty()
        {
            return TypeDescriptor.GetDefaultProperty(this, true);
        }

        object ICustomTypeDescriptor.GetEditor(Type editorBaseType)
        {
            return TypeDescriptor.GetEditor(this, editorBaseType, true);
        }

        EventDescriptorCollection ICustomTypeDescriptor.GetEvents()
        {
            return TypeDescriptor.GetEvents(this, true);
        }

        EventDescriptorCollection ICustomTypeDescriptor.GetEvents(Attribute[] attributes)
        {
            return TypeDescriptor.GetEvents(this, attributes, true);
        }

        PropertyDescriptorCollection ICustomTypeDescriptor.GetProperties()
        {
            return ((ICustomTypeDescriptor)this).GetProperties(null);
        }

        PropertyDescriptorCollection ICustomTypeDescriptor.GetProperties(Attribute[] attributes)
        {
            ArrayList pds = new ArrayList();
            Attribute[] att = new Attribute[] 
            {
                                                  new BrowsableAttribute(true),
                                                  new System.Xml.Serialization.XmlIgnoreAttribute(),
                                                  new RefreshPropertiesAttribute(RefreshProperties.All),
                                                  new DesignerSerializationVisibilityAttribute(DesignerSerializationVisibility.Hidden),
                                                  new CategoryAttribute("Items")
                                              };

            ArrayList names = new ArrayList();
            foreach (DescriptorBase descriptor in this)
            {
                string name = descriptor.GetName();
                if (name == string.Empty)
                {
                    name = "Empty";
                }

                pds.Add(new DescriptorBasePropertyDescriptor(name, descriptor, att, GetType()));
                names.Add(descriptor.GetName());
            }

            PropertyDescriptorCollection pdc = new PropertyDescriptorCollection((PropertyDescriptor[])pds.ToArray(typeof(PropertyDescriptor)));
            return pdc.Sort((string[])names.ToArray(typeof(string)));
        }

        object ICustomTypeDescriptor.GetPropertyOwner(PropertyDescriptor pd)
        {
            return this;
        }

        #endregion
    }

    /// <summary>
    /// Enumerator class for <see cref="FieldDescriptor"/> elements of a <see cref="FieldDescriptorCollection"/>.
    /// </summary>
    public class FieldDescriptorCollectionEnumerator : IEnumerator
    {
        int _cursor = -1, _next = -1;
        FieldDescriptorCollection _coll;

        /// <summary>
        /// Initalizes the enumerator and attaches it to the collection.
        /// </summary>
        /// <param name="collection">The parent collection to enumerate.</param>
        public FieldDescriptorCollectionEnumerator(FieldDescriptorCollection collection)
        {
            _coll = collection;
            _next = _coll.Count > 0 ? 0 : -1;
        }

        #region IEnumerator Members

        /// <summary>
        /// Sets the enumerator to its initial position, which is before the first element in the collection.
        /// </summary>
        public virtual void Reset()
        {
            _cursor = -1;
            _next = _coll.Count > 0 ? 0 : -1;
        }

        object IEnumerator.Current
        {
            get
            {
                return Current;
            }
        }

        /// <summary>
        /// Gets the current element in the collection.
        /// </summary>
        public FieldDescriptor Current
        {
            get
            {
                return _coll[_cursor];
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
            if (_next == -1)
            {
                return false;
            }

            _cursor = _next;

            _next++;
            if (_next >= _coll.Count)
            {
                _next = -1;
            }

            return _cursor != -1;
        }
        #endregion
    }

    #endregion

    #region TypeConverter
    /// <summary>
    /// The type converter for <see cref="FieldDescriptor"/> objects. <see cref="FieldDescriptorTypeConverter"/>
    /// is a <see cref="DescriptorBaseConverter"/>. It overrides the default behavior of the
    /// <see cref="ConvertTo"/> method and adds support for design-time code serialization.
    /// </summary>
    public class FieldDescriptorTypeConverter : DescriptorBaseConverter
    {
        /// <override/>
        /// <summary>Determines whether this object can be converted to the specified type, using the given format.</summary>
        /// <param name="context">Format context.</param>
        /// <param name="destinationType">Target type.</param>
        /// <returns>True if this conversion is supported; False otherwise.</returns>
        /// <override/>
        public override /*TypeConverter*/ bool CanConvertTo(ITypeDescriptorContext context, Type destinationType)
        {
            if (destinationType == typeof(System.ComponentModel.Design.Serialization.InstanceDescriptor))
            {
                return true;
            }
            else
            {
                return base.CanConvertTo(context, destinationType);
            }
        }

        /// <override/>
        /// <summary>Converts the given value to the specified type, using the given format and culture.</summary>
        /// <param name="context">Format context.</param>
        /// <param name="culture">The CultureInfo.</param>
        /// <param name="value">The object value.</param>
        /// <param name="destinationType">The Target type.</param>
        /// <returns>Converted object.</returns>
        /// <override/>
        public override /*TypeConverter*/ object ConvertTo(ITypeDescriptorContext context, CultureInfo culture, object value, Type destinationType)
        {
            if (destinationType == typeof(System.ComponentModel.Design.Serialization.InstanceDescriptor)
                && (value is FieldDescriptor))
            {
                FieldDescriptor field = (FieldDescriptor)value;
                Type type = value.GetType();

                return new InstanceDescriptor(
                    type.GetConstructor(new Type[] { typeof(string), typeof(string), typeof(bool), typeof(string) }),
                    new object[] { field.Name, field.MappingName, field.ReadOnly, field.DefaultValue }, 
                    true);
            }

            return base.ConvertTo(context, culture, value, destinationType);
        } // end of method ConvertTo
        
        /// <override/>
        /// <summary>
        /// Gets a list of <see cref="PropertyDescriptor"/> objects for the specified component type.
        /// </summary>
        /// <param name="context">Format context.</param>
        /// <param name="value">Value specifying the component type.</param>
        /// <param name="attributes">Array of System.Attribute objects that will be used as a filter.</param>
        /// <returns>Property descriptor collection.</returns>
        public override PropertyDescriptorCollection GetProperties(ITypeDescriptorContext context, object value, Attribute[] attributes)
        {
            System.ComponentModel.PropertyDescriptorCollection pds
                = TypeDescriptor.GetProperties(value.GetType(), attributes);

            string[] atts = new string[]
            {
                "Name",
                "MappingName",
                "ReadOnly",
                "DefaultValue",
            };

            return pds.Sort(atts);
        }
    }
    #endregion

#if SyncfusionFramework4_0
    /// <summary>
    /// A class that provides an abstraction of a dynamic property.
    /// </summary>
    public class DynamicPropertyDescriptor : PropertyDescriptor
    {
        private Type type = typeof(object);

        /// <summary>
        /// Initializes a new instance of the <see cref="T:Syncfusion.Grouping.DynamicPropertyDescriptor">DynamicPropertyDescriptor</see> class. 
        /// </summary>
        /// <param name="name">Name of the property.</param>
        /// <param name="attributes">An array of attributes to associate with the property.</param>
        public DynamicPropertyDescriptor(string name, Attribute[] attributes)
            : base(name, attributes)
        {
            
        }

        /// <summary>
        /// Returns the basic type of the dynamic property.
        /// </summary>
        /// <param name="tableDescriptor">table descriptor that holds the property details.</param>
        /// <returns>Type of the property</returns>
        internal Type GetPropertyType(TableDescriptor tableDescriptor)
        {
            type = tableDescriptor.ItemPropertyTypes[Name];
            return type;
        }

        /// <summary>
        /// When overridden in a derived class, returns whether resetting an object changes
        /// its value.
        /// </summary>
        /// <param name="component">The component to test for reset capability. </param>
        /// <returns>
        /// true if resetting the component changes its value; otherwise, false.
        /// </returns>
        public override bool CanResetValue(object component)
        {
            return ShouldSerializeValue(component);
        }

        /// <summary>
        /// When overridden in a derived class, gets the type of the component this property
        /// is bound to.
        /// </summary>
        /// <returns>
        /// A <see cref="T:System.Type" /> that represents the type of component this
        /// property is bound to. When the <see
        /// cref="M:System.ComponentModel.PropertyDescriptor.GetValue(System.Object)" /> or
        /// <see
        /// cref="M:System.ComponentModel.PropertyDescriptor.SetValue(System.Object,System.Object)"
        /// /> methods are invoked, the object specified might be an instance of this type.
        /// </returns>
        public override Type ComponentType
        {
            get { return typeof(object); }
        }

        /// <summary>
        /// When overridden in a derived class, gets the current value of the property on a
        /// component.
        /// </summary>
        /// <param name="component">The component with the property for which to retrieve
        /// the value. </param>
        /// <returns>
        /// The value of a property for a given component.
        /// </returns>
        public override object GetValue(object component)
        {
            IDictionary<string, object> dynamicDictionary = component as IDictionary<string, object>;
            object result;
            if (dynamicDictionary != null)
            {
                if (!dynamicDictionary.TryGetValue(Name, out result))
                {
                    foreach (KeyValuePair<string,object> entry in dynamicDictionary)
                    {
                        if (entry.Value is IDictionary<string, object>)
                        {
                            return GetValue(entry.Value);
                        }
                    }
                }
                if (dynamicDictionary[Name] != null)
                    type = dynamicDictionary[Name].GetType();
                else
                    type = typeof(object);
                return dynamicDictionary[Name];
            }
            return null;
        }

        /// <summary>
        /// When overridden in a derived class, gets a value indicating whether this
        /// property is read-only.
        /// </summary>
        /// <returns>
        /// true if the property is read-only; otherwise, false.
        /// </returns>
        public override bool IsReadOnly
        {
            get { return false; }
        }

        /// <summary>
        /// When overridden in a derived class, gets the type of the property.
        /// </summary>
        /// <returns>
        /// A <see cref="T:System.Type" /> that represents the type of the property.
        /// </returns>
        public override Type PropertyType
        {
            get { return type; }
        }

        /// <summary>
        /// When overridden in a derived class, resets the value for this property of the
        /// component to the default value.
        /// </summary>
        /// <param name="component">The component with the property value that is to be
        /// reset to the default value. </param>
        public override void ResetValue(object component)
        {
            
        }

        /// <summary>
        /// When overridden in a derived class, sets the value of the component to a
        /// different value.
        /// </summary>
        /// <param name="value">The new value. </param>
        /// <param name="component">The component with the property value that is to be set.
        /// </param>
        public override void SetValue(object component, object value)
        {
            IDictionary<string, object> dynamicDictionary = component as IDictionary<string, object>;
            if (dynamicDictionary != null)
            {
                dynamicDictionary[Name] = value;
            }
        }

        /// <summary>
        /// When overridden in a derived class, determines a value indicating whether the
        /// value of this property needs to be persisted.
        /// </summary>
        /// <param name="component">The component with the property to be examined for
        /// persistence. </param>
        /// <returns>
        /// true if the property should be persisted; otherwise, false.
        /// </returns>
        public override bool ShouldSerializeValue(object component)
        {
            return true;
        }
    }

#endif

    #region FieldDescriptor

    /// <summary>
    /// An IComparer implementation for comparing the names of
    /// two <see cref="FieldDescriptor"/> objects.
    /// </summary>
    public class FieldDescriptorNameComparer : IComparer
    {
        #region IComparer Members

        /// <summary>
        /// Casts the objects to <see cref="FieldDescriptor"/> and compares
        /// the <see cref="FieldDescriptor.Name"/> of both objects.
        /// </summary>
        /// <param name="x">The first object.</param>
        /// <param name="y">The second object.</param>
        /// <returns>
        /// The result of string.Compare for the <see cref="FieldDescriptor.Name"/> of the
        /// two <see cref="FieldDescriptor"/> objects; 0 if both are the same.
        /// </returns>
        public int Compare(object x, object y)
        {
            FieldDescriptor c = (FieldDescriptor)x;
            FieldDescriptor d = (FieldDescriptor)y;
            return c.Name.CompareTo(d.Name);
        }
        #endregion
    }

    /// <summary>
    /// FieldDescriptor provides mapping information to a column of the underlying datasource.
    /// <para/>
    /// Fields are managed by the <see cref="FieldDescriptorCollection"/> that
    /// is returned by the <see cref="Syncfusion.Grouping.TableDescriptor.Fields"/> property
    /// of a <see cref="TableDescriptor"/>.
    /// </summary>
    [TypeConverter(typeof(FieldDescriptorTypeConverter))]
    public class FieldDescriptor : DescriptorBase, ICloneable, IStandardValuesProvider
    {
        string name = string.Empty;
        PropertyDescriptor pd = null;
        internal int version = 0;
        PropertyDescriptor[] complexPropertyDescriptors;
        ExpressionFieldDescriptor expressionFieldDescriptor; // for Expressions and RelationDescriptors
        FieldDescriptor unboundFieldDescriptor; // for Expressions and RelationDescriptors
        internal FieldDescriptor relatedDescriptor; // for Expressions and RelationDescriptors
        internal SummaryDescriptor relatedSummaryDescriptor;
        internal FieldDescriptor parentFieldDescriptor; // for nested properties fields RelationDescriptors
        internal RelationDescriptor relation; // for Relations both relation and relatedDescriptor must be set
        FieldDescriptorCollection collection;
        TableDescriptor tableDescriptor;
        string mappingName = string.Empty;
        bool readOnly = false;
        internal int index;
        internal bool nameModified = false;
        bool readOnlyModified = false;
        string defaultValue = string.Empty;
        bool defaultValueModified = false;
        internal bool isForeignKeyField = false;
        internal bool isObjectReferenceField = false;
        bool hide = false;
        bool hideModified = false;
        bool allowTrimEnd = true;
        bool allowTrimEndModified = false;
        bool isSorting = false;
        bool forceImmediateSaveValue = false;
        bool forceImmediateSaveValueModified = false;
        Type fieldPropertyType = null;
        internal string referencedFields = string.Empty;
        
        ////bool isDefault = false;
        
        /// <summary>
        /// Occurs when a property is changed.
        /// </summary>
        public event DescriptorPropertyChangedEventHandler PropertyChanged;

        /// <summary>
        /// Occurs before a property is changed.
        /// </summary>
        public event DescriptorPropertyChangedEventHandler PropertyChanging;

        /// <overload>
        /// Initializes a new empty FieldDescriptor.
        /// </overload>
        /// <summary>
        /// Initializes a new empty FieldDescriptor.
        /// </summary>
        public FieldDescriptor()
        {
        }

        /// <summary>
        /// Initializes a new FieldDescriptor with a name.
        /// </summary>
        /// <param name="name">Name of the field descriptor.</param>
        public FieldDescriptor(string name)
        {
            this.name = name;
            this.mappingName = name;
        }

        /// <summary>
        /// Initializes a new FieldDescriptor with a name and a type. Use this ctor only for unbound fields.
        /// </summary>
        /// <param name="name">Field descriptor name.</param>
        /// <param name="type">Type of the descriptor.</param>
        public FieldDescriptor(string name, Type type)
            : this(name)
        {
            this.fieldPropertyType = type;
        }

        /// <summary>
        /// Initializes a new FieldDescriptor with a name, mapping name, Read-only setting, and default value for NULL fields.
        /// </summary>
        /// <param name="name">The Name for FieldDescriptor.</param>
        /// <param name="mappingName">Mapping name.</param>
        /// <param name="readOnly">Indicates if the descriptor is read-only.</param>
        /// <param name="defaultValue">Default value.</param>
        public FieldDescriptor(string name, string mappingName, bool readOnly, string defaultValue)
        {
            this.name = name;
            this.mappingName = mappingName;
            this.readOnly = readOnly;
            this.defaultValue = defaultValue;
            this.nameModified = true;
        }

        /// <override/>
        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                name = "Disposed";
                pd = null;
                expressionFieldDescriptor = null;
                unboundFieldDescriptor = null;
                relatedDescriptor = null;
                relatedSummaryDescriptor = null;
                parentFieldDescriptor = null;
                relation = null;
                collection = null;
                tableDescriptor = null;
                fieldPropertyType = null;
            }

            base.Dispose(disposing);
        }

        /// <summary>
        /// Initializes this object and copies properties from another object. <see cref="PropertyChanging"/>
        /// and <see cref="PropertyChanged"/> events are raised for every property that is modified. If both
        /// objects are equal, no events are raised.
        /// </summary>
        /// <param name="other">The source object.</param>
        public virtual void InitializeFrom(FieldDescriptor other)
        {
            this.Name = other.Name;
            this.MappingName = other.MappingName;
            if (other.ShouldSerializeReadOnly())
            {
                this.ReadOnly = other.ReadOnly;
            }
            else
            {
                ResetReadOnly();
            }

            if (other.ShouldSerializeDefaultValue())
            {
                this.DefaultValue = other.DefaultValue;
            }
            else
            {
                ResetDefaultValue();
            }

            if (other.ShouldSerializeHide())
            {
                this.Hide = other.Hide;
            }
            else
            {
                ResetHide();
            }

            if (other.ShouldSerializeReferencedFields())
            {
                this.ReferencedFields = other.ReferencedFields;
            }
            else
            {
                ResetReferencedFields();
            }

            if (other.ShouldSerializeAllowTrimEnd())
            {
                this.AllowTrimEnd = other.AllowTrimEnd;
            }
            else
            {
                ResetAllowTrimEnd();
            }

            if (other.ShouldSerializeForceImmediateSaveValue())
            {
                this.ForceImmediateSaveValue = other.ForceImmediateSaveValue;
            }
            else
            {
                ResetForceImmediateSaveValue();
            }

            if (other.ShouldSerializeFieldPropertyType())
            {
                this.FieldPropertyType = other.FieldPropertyType;
            }
            else
            {
                ResetFieldPropertyType();
            }
        }

        /// <summary>
        /// The version number of this object. The version is increased each time the
        /// element was modified.
        /// </summary>
        internal int Version
        {
            get
            {
                return version;
            }
        }

        ICollection IStandardValuesProvider.GetStandardValues(PropertyDescriptor pd)
        {
            if (TableDescriptor == null || this.collection is UnboundFieldDescriptorCollection)
            {
                return new ArrayList();
            }

            ArrayList al = new ArrayList();
            SortedList sl = new SortedList();
            foreach (PropertyDescriptor ppd in TableDescriptor.ItemProperties)
            {
                al.Add(ppd.Name);
            }

            foreach (ExpressionFieldDescriptor cd in TableDescriptor.ExpressionFields)
            {
                if (!sl.ContainsKey(cd.Name))
                {
                    al.Add(cd.Name);
                }
            }

            foreach (FieldDescriptor cd in TableDescriptor.UnboundFields)
            {
                if (!sl.ContainsKey(cd.Name))
                {
                    al.Add(cd.Name);
                }
            }

            return al;
        }

        internal void SetCollection(FieldDescriptorCollection collection)
        {
            this.collection = collection;
            this.tableDescriptor = collection._tableDescriptor;
        }

        /// <summary>
        /// The collection this descriptor belongs to.
        /// </summary>
        [System.Xml.Serialization.XmlIgnore]
        [DesignerSerializationVisibilityAttribute(DesignerSerializationVisibility.Hidden)]
        [Browsable(false)]
        public FieldDescriptorCollection Collection
        {
            get
            {
                return collection;
            }
        }

        internal void SetTableDescriptor(TableDescriptor tableDescriptor)
        {
            this.tableDescriptor = tableDescriptor;
        }

        /// <summary>
        /// The TableDescriptor that this descriptor belongs to.
        /// </summary>
        [System.Xml.Serialization.XmlIgnore]
        [DesignerSerializationVisibilityAttribute(DesignerSerializationVisibility.Hidden)]
        [Browsable(false)]
        public TableDescriptor TableDescriptor
        {
            get
            {
                return tableDescriptor;
            }
        }

        /// <summary>
        /// Raises the <see cref="PropertyChanging"/> event.
        /// </summary>
        /// <param name="e">A <see cref="DescriptorPropertyChangedEventArgs" /> that contains the event data.</param>
        protected virtual void OnPropertyChanging(DescriptorPropertyChangedEventArgs e)
        {
            if (this.Disposing)
            {
                return;
            }

            if (PropertyChanging != null)
            {
                PropertyChanging(this, e);
            }

            if (collection != null)
            {
                collection.RaisePropertyItemChanging(this, e);
            }
        }

        /// <summary>
        /// Raises the <see cref="PropertyChanged"/> event.
        /// </summary>
        /// <param name="e">A <see cref="DescriptorPropertyChangedEventArgs" /> that contains the event data.</param>
        protected virtual void OnPropertyChanged(DescriptorPropertyChangedEventArgs e)
        {
            if (this.Disposing)
            {
                return;
            }

            this.refFields = null;

            if (PropertyChanged != null)
            {
                PropertyChanged(this, e);
            }

            if (collection != null)
            {
                collection.RaisePropertyItemChanged(this, e);
            }
        }

        object ICloneable.Clone()
        {
            return Clone();
        }

        /// <summary>
        /// Creates a copy of this descriptor.
        /// </summary>
        /// <returns>A copy of this descriptor.</returns>
        public virtual FieldDescriptor Clone()
        {
            FieldDescriptor fd = new FieldDescriptor();
            CopyAllMembersTo(fd);
            return fd;
        }

        /// <summary>
        /// Copies all properties to another element without raising Changing or Changed events.
        /// </summary>
        /// <param name="fd">The target object.</param>
        protected void CopyAllMembersTo(FieldDescriptor fd)
        {
            fd.collection = this.collection;
            fd.complexPropertyDescriptors = this.complexPropertyDescriptors;
            fd.defaultValue = this.defaultValue;
            fd.defaultValueModified = this.defaultValueModified;
            fd.expressionFieldDescriptor = this.expressionFieldDescriptor;
            fd.unboundFieldDescriptor = this.unboundFieldDescriptor;
            fd.index = this.index;
            fd.isSorting = this.isSorting;
            fd.mappingName = this.mappingName;
            fd.name = this.name;
            fd.nameModified = this.nameModified;
            fd.fieldPropertyType = this.fieldPropertyType;
            fd.pd = this.pd;
            fd.readOnly = this.readOnly;
            fd.readOnlyModified = this.readOnlyModified;
            fd.parentFieldDescriptor = this.parentFieldDescriptor;
            fd.relatedDescriptor = this.relatedDescriptor;
            fd.relatedSummaryDescriptor = this.relatedSummaryDescriptor;
            fd.relation = this.relation;
            fd.tableDescriptor = this.tableDescriptor;
            fd.hide = hide;
            fd.hideModified = hideModified;
            fd.referencedFields = referencedFields;
            fd.allowTrimEnd = allowTrimEnd;
            fd.allowTrimEndModified = allowTrimEndModified;
            fd.forceImmediateSaveValue = forceImmediateSaveValue;
            fd.forceImmediateSaveValueModified = forceImmediateSaveValueModified;
            fd.version = version + 1000;
        }

        /// <summary>Compares two field descriptors.</summary>
        /// <param name="obj">Field descriptor.</param>
        /// <returns>True if the descriptors are equal; False otherwise.</returns>
        /// <override/>
        public override bool Equals(object obj)
        {
            if (this == null && obj == null)
            {
                return true;
            }
            else if (this == null)
            {
                return false;
            }
            else if (!(obj is FieldDescriptor))
            {
                return false;
            }

            return InternalEquals((FieldDescriptor)obj);
        }

        bool InternalEquals(FieldDescriptor other)
        {
            return other.defaultValue == this.defaultValue
                && other.defaultValueModified == this.defaultValueModified
                && other.mappingName == this.mappingName
                && other.name == this.name
                && other.nameModified == this.nameModified
                && other.readOnly == this.readOnly
                && other.fieldPropertyType == this.fieldPropertyType
                && other.referencedFields == this.referencedFields
                && other.readOnlyModified == this.readOnlyModified
                && other.hide == hide
                && other.hideModified == hideModified
                && other.allowTrimEnd == allowTrimEnd
                && other.allowTrimEndModified == allowTrimEndModified
                && other.forceImmediateSaveValue == forceImmediateSaveValue
                && other.forceImmediateSaveValueModified == forceImmediateSaveValueModified;
        }

        void CollectionEnsureMapping()
        {
            if (collection != null)
            {
                collection.EnsureMapping(this);
            }
        }

        /// <summary>Serves as a hash function.</summary>
        /// <returns>Hash code.</returns>
        /// <override/>
        public override int GetHashCode()
        {
            return base.GetHashCode();
        }

        /// <summary>Returns the name of the descriptor.</summary>
        /// <returns>Field descriptor name.</returns>
        /// <override/>
        public override string GetName()
        {
            return Name;
        }

        /// <summary>
        /// The name of the field.
        /// </summary>
        [RefreshProperties(RefreshProperties.All)]
        [Description("The name of the field")]
        public virtual string Name
        {
            get
            {
                return name;
            }

            set
            {
                if (name != value)
                {
                    OnPropertyChanging(new DescriptorPropertyChangedEventArgs("Name"));
                    string str = name;
                    name = value;
                    try
                    {
                        nameModified = true;
                        OnPropertyChanged(new DescriptorPropertyChangedEventArgs("Name"));
                    }
                    catch
                    {
                        name = str;
                    }
                }
            }
        }

        /// <summary>
        /// Determines if the field's name was modified.
        /// </summary>
        /// <returns>True if name is modified.</returns>
        public bool ShouldSerializeName()
        {
            return nameModified;
        }

        /// <summary>
        /// Resets the field's name.
        /// </summary>
        public void ResetName()
        {
            Name = string.Empty;
            nameModified = false;
        }

        /// <summary>
        /// Gets a related field descriptor of a related table that this descriptor holds. If this field descriptor is
        /// not a wrapper object for a field in a related table, the method will return NULL.
        /// </summary>
        /// <returns>A field descriptor of the related table.</returns>
        public FieldDescriptor GetParentFieldDescriptor()
        {
            CollectionEnsureMapping();
            return parentFieldDescriptor;
        }
        
        /// <summary>
        /// Gets a related summary descriptor of a related table that this descriptor holds. (for RelationKind.ForeignKeyKeyWords)
        /// </summary>
        /// <returns>A summary descriptor of the related table.</returns>
        public SummaryDescriptor GetRelatedSummaryDescriptor()
        {
            CollectionEnsureMapping();
            return relatedSummaryDescriptor;
        }

        /// <summary>
        /// Gets a related field descriptor of a related table that this descriptor holds. If this field descriptor is
        /// not a wrapper object for a field in a related table, the method will return NULL.
        /// </summary>
        /// <returns>A field descriptor of the related table.</returns>
        public FieldDescriptor GetRelatedDescriptor()
        {
            CollectionEnsureMapping();
            return relatedDescriptor;
        }

        /// <summary>
        /// Gets a nested related field descriptor of a related table that this descriptor holds. 
        /// </summary>
        /// <returns>Nested related field descriptor.</returns>
        public FieldDescriptor GetNestedRelatedDescriptor()
        {
            CollectionEnsureMapping();
            FieldDescriptor rfd = relatedDescriptor;
            while (rfd != null && rfd.relatedDescriptor != null)
            {
                rfd = rfd.relatedDescriptor;
            }

            return rfd;
        }

        /// <summary>
        /// Gets the expression field descriptor this descriptor holds. If this field descriptor is
        /// not a wrapper object for a expression field, the method will return NULL.
        /// </summary>
        /// <returns>An expression field descriptor.</returns>
        public ExpressionFieldDescriptor GetExpressionFieldDescriptor()
        {
            CollectionEnsureMapping();
            return expressionFieldDescriptor;
        }

        /// <summary>
        /// Gets the unbound field descriptor this descriptor holds. If this field descriptor is
        /// not a wrapper object for a unbound field, the method will return NULL.
        /// </summary>
        /// <returns>An expression field descriptor.</returns>
        public FieldDescriptor GetUnboundFieldDescriptor()
        {
            CollectionEnsureMapping();
            return unboundFieldDescriptor;
        }

        /// <summary>
        /// Gets the relation descriptor this descriptor holds. If this field descriptor is
        /// not a wrapper object for a field in a related table, the method will return NULL.
        /// </summary>
        /// <returns>A expression field descriptor.</returns>
        public RelationDescriptor GetRelation()
        {
            CollectionEnsureMapping();
            return relation;
        }

        /// <summary>
        /// Gets the property descriptor this descriptor holds. If this field descriptor is
        /// not a wrapper object for a property in the main table (and also not a nested property inside
        /// a complex object), the method will return NULL.
        /// </summary>
        /// <returns>A property descriptor.</returns>
        public PropertyDescriptor GetPropertyDescriptor()
        {
            CollectionEnsureMapping();
            return pd;
        }

        /// <summary>
        /// Gets the property descriptor this descriptor holds. If this field descriptor is
        /// a wrapper object for a property (IsComplexPropertyField) in the main table, the method will return NULL.
        /// </summary>
        /// <returns>A property descriptor.</returns>
        public PropertyDescriptor GetSimplePropertyDescriptor()
        {
            if (IsPropertyField() && !IsComplexPropertyField())
            {
                return GetPropertyDescriptor();
            }

            return null;
        }

        /// <summary>
        /// Gets the complex property descriptor this descriptor holds. If this field descriptor is
        /// not a wrapper object for a nested property inside
        /// a complex object, the method will return NULL.
        /// </summary>
        /// <returns>A complex property descriptor.</returns>
        public PropertyDescriptor[] GetComplexPropertyDescriptors()
        {
            CollectionEnsureMapping();
            return this.complexPropertyDescriptors;
        }

        internal void SetMapping(PropertyDescriptor pd, RelationDescriptor rd, FieldDescriptor fd, ExpressionFieldDescriptor ed)
        {
            if (pd != null && pd != this.pd)
            {
                if (name.Length == 0)
                {
                    name = pd.Name;
                }

                if (this.readOnlyModified)
                {
                    this.readOnly = pd.IsReadOnly;
                }
            }

            this.pd = pd;
            this.expressionFieldDescriptor = ed;
            this.relation = rd;
            this.relatedDescriptor = fd;
        }

        internal void SetMapping(PropertyDescriptor[] complexPropertyDescriptors, RelationDescriptor rd, FieldDescriptor fd, FieldDescriptor parentFieldDescriptor)
        {
            PropertyDescriptor pd = complexPropertyDescriptors[complexPropertyDescriptors.Length - 1];
            if (pd != null && pd != this.pd)
            {
                if (name.Length == 0)
                {
                    name = pd.Name;
                }

                if (this.readOnlyModified)
                {
                    this.readOnly = pd.IsReadOnly;
                }
            }

            this.pd = pd;
            this.complexPropertyDescriptors = complexPropertyDescriptors;
            this.expressionFieldDescriptor = null;
            this.relation = rd;
            this.relatedDescriptor = fd;
            this.parentFieldDescriptor = parentFieldDescriptor;
        }

        /// <summary>
        /// Gets a related field descriptor of a related table that this descriptor holds. If this field descriptor is
        /// not a wrapper object for a field in a related table, the method will return NULL.
        /// </summary>
        /// <returns>True if it is a related field; False otherwise.</returns>
        public bool IsRelatedField()
        {
            if (!isSorting)
            {
                CollectionEnsureMapping();
            }

            return this.relatedDescriptor != null && relation != null;
        }

        /// <summary>
        /// Determines if this field descriptor is a foreign key field (and therefore changes to the field will affect other fields
        /// in the record).
        /// </summary>
        /// <returns>True if it is a foreign key field; False otherwise.</returns>
        public bool IsForeignKeyField()
        {
            if (!isSorting)
            {
                CollectionEnsureMapping();
            }

            return this.isForeignKeyField;
        }

        /// <summary>
        /// Determines if this field descriptor is an object reference field.
        /// </summary>
        /// <returns>True if it is an object reference field; False otherwise.</returns>
        public bool IsObjectReferenceField()
        {
            if (!isSorting)
            {
                CollectionEnsureMapping();
            }

            return this.isObjectReferenceField;
        }

        /// <summary>
        /// Determines if this field descriptor is a wrapper object for an expression field.
        /// </summary>
        /// <returns>True if it is an expression field; False otherwise.</returns>
        public bool IsExpressionField()
        {
            if (!isSorting)
            {
                CollectionEnsureMapping();
            }

            return this.expressionFieldDescriptor != null && relation == null;
        }

        /// <summary>
        /// Determines if this field descriptor is an unbound field (no mappingname to an underlying property of the unerlying datasource given).
        /// </summary>
        /// <returns>True if it is an unbound field; False otherwise.</returns>
        public bool IsUnboundField()
        {
            if (!isSorting)
            {
                CollectionEnsureMapping();
            }

            return this.unboundFieldDescriptor != null;
        }

        /// <summary>
        /// Determines if this field descriptor is a wrapper object for a property in the main table (and not a nested property inside
        /// a complex object).
        /// </summary>
        /// <returns>True if it is a property field; False otherwise.</returns>
        public bool IsPropertyField()
        {
            if (!isSorting)
            {
                CollectionEnsureMapping();
            }

            return this.pd != null;
        }

#if SyncfusionFramework4_0
        /// <summary>
        /// Determines if this field descriptor is a wrapper object for a dynamic property in the main table (and not a nested property inside
        /// a complex object).
        /// </summary>
        /// <returns>True if it is a property field; False otherwise.</returns>
        public bool IsDynamicPropertyField()
        {
            if (!isSorting)
            {
                CollectionEnsureMapping();
            }

            return (this.pd != null && this.pd is DynamicPropertyDescriptor);
        }
#endif

        /// <summary>
        /// Determines if this field descriptor is a wrapper object for a nested property inside
        /// a complex object. If so, the method will return NULL.
        /// </summary>
        /// <returns>True if this is a complex property field; False otherwise.</returns>
        public bool IsComplexPropertyField()
        {
            if (!isSorting)
            {
                CollectionEnsureMapping();
            }

            return this.complexPropertyDescriptors != null && this.complexPropertyDescriptors.Length > 1;
        }
        
        /// <summary>
        /// Gets the result type of this field.
        /// </summary>
        /// <returns>Property type.</returns>
        public virtual Type GetPropertyType()
        {
            if (this.fieldPropertyType != null)
            {
                return this.fieldPropertyType;
            }

            if (this.expressionFieldDescriptor != null)
            {
                return this.expressionFieldDescriptor.GetPropertyType();
            }

            if (this.unboundFieldDescriptor != null && !Object.ReferenceEquals(this, unboundFieldDescriptor))
            {
                return this.unboundFieldDescriptor.GetPropertyType();
            }

            if (this.relatedSummaryDescriptor != null)
            {
                return typeof(object[]);
            }

            if (pd == null)
            {
                return typeof(string);
            }

#if SyncfusionFramework4_0
            if (pd is DynamicPropertyDescriptor)
            {
                DynamicPropertyDescriptor dpd = pd as DynamicPropertyDescriptor;
                return dpd.GetPropertyType(this.TableDescriptor);
            }
#endif
            return pd.PropertyType;
        }

        internal void ResetMapping()
        {
            this.version = -1;
            this.pd = null;
            this.relation = null;
            this.relatedDescriptor = null;
            this.relatedSummaryDescriptor = null;
            this.parentFieldDescriptor = null;
            this.complexPropertyDescriptors = null;
            this.expressionFieldDescriptor = null; // for Expressions and RelationDescriptors
            this.unboundFieldDescriptor = null; // for Expressions and RelationDescriptors
            this.relation = null; // for Relations both relation and relatedDescriptor must be set
            this.isForeignKeyField = false;
            this.isObjectReferenceField = false;
        }

        /// <summary>
        /// The mapping for this field. You should specify which field of a <see cref="System.Data.DataTable"/>
        /// or strong-typed collection you want to display in the grid at this field. The MappingName can also point to a property
        /// inside a complex object with nested properties of a collection when a '.' is specified.
        /// </summary>
        [TypeConverter(typeof(StandardValuesCollectionConverter))]
        [DefaultValue("")]
        [RefreshProperties(RefreshProperties.All)]
        [Description("Specifies which field of a DataTable or strong-typed collection to be displayed in the grid at this field.")]
        public string MappingName
        {
            get
            {
                return mappingName;
            }

            set
            {
                if (mappingName != value)
                {
                    OnPropertyChanging(new DescriptorPropertyChangedEventArgs("MappingName"));
                    ResetMapping();
                    string strName = name;
                    string strMappingName = mappingName;
                    if (!nameModified && (mappingName == string.Empty || InPropertyGrid()))
                    {
                        if (mappingName != string.Empty)
                        {
                            name = mappingName;
                        }
                        else if (collection != null)
                        {
                            collection.SuggestName(this);
                        }

                        name = value;
                        nameModified = false;
                    }

                    mappingName = value;
                    try
                    {
                        OnPropertyChanged(new DescriptorPropertyChangedEventArgs("MappingName"));
                    }
                    catch
                    {
                        name = strName;
                        mappingName = strMappingName;
                        throw;
                    }
                }
            }
        }

        bool InPropertyGrid()
        {
            return (this.collection != null && collection.insideCollectionEditor) || (tableDescriptor != null && tableDescriptor.IsDesignTime());
        }

        /// <summary>
        /// A default value that should be used for empty fields in the AddNewRecord.
        /// </summary>
        [Description("A default value that should be used for empty fields in the AddNewRecord.")]
        public string DefaultValue
        {
            get
            {
                return defaultValue;
            }

            set
            {
                if (defaultValue != value)
                {
                    OnPropertyChanging(new DescriptorPropertyChangedEventArgs("DefaultValue"));
                    defaultValue = value;
                    defaultValueModified = true;
                    OnPropertyChanged(new DescriptorPropertyChangedEventArgs("DefaultValue"));
                }
            }
        }

        /// <summary>
        /// Determines whether the default value is empty.
        /// </summary>
        /// <returns>True if the default value is not empty; False otherwise.</returns>
        public bool ShouldSerializeDefaultValue()
        {
            return defaultValueModified;
        }

        /// <summary>
        /// Resets the default value to empty.
        /// </summary>
        public void ResetDefaultValue()
        {
            defaultValue = string.Empty;
            defaultValueModified = false;
        }

        /// <summary>
        /// Gets / set Read-only state of the field.
        /// </summary>
        [Description("Gets / set Read-only state of the field.")]
        public virtual bool ReadOnly
        {
            get
            {
                if (this.tableDescriptor == null)
                {
                    return readOnly;
                }

                return readOnly || (!this.IsPropertyField() && !this.IsUnboundField()) || this.IsRelatedField() || (this.pd != null && this.pd.IsReadOnly);
            }

            set
            {
                if (readOnly != value)
                {
                    OnPropertyChanging(new DescriptorPropertyChangedEventArgs("ReadOnly"));
                    readOnly = value;
                    readOnlyModified = true;
                    OnPropertyChanged(new DescriptorPropertyChangedEventArgs("ReadOnly"));
                }
            }
        }

        /// <summary>
        /// Determines if the Read-only state of the field was modified.
        /// </summary>
        /// <returns>True if the content were modified; False otherwise.</returns>
        public bool ShouldSerializeReadOnly()
        {
            return readOnlyModified;
        }

        /// <summary>
        /// Resets the Read-only to False.
        /// </summary>
        public void ResetReadOnly()
        {
            readOnly = false;
            readOnlyModified = false;
        }

        /// <summary>
        /// Gets / sets the property type. 
        /// </summary>
        [Description("Gets / Sets the property type the field.")]
        [System.Xml.Serialization.XmlIgnore]
        [TypeConverter(typeof(TypeTypeConverter))]
        public Type FieldPropertyType
        {
            get
            {
                if (fieldPropertyType == null)
                {
                    return GetPropertyType();
                }

                return fieldPropertyType;
            }

            set
            {
                if (fieldPropertyType != value)
                {
                    OnPropertyChanging(new DescriptorPropertyChangedEventArgs("FieldPropertyType"));
                    fieldPropertyType = value;
                    OnPropertyChanged(new DescriptorPropertyChangedEventArgs("FieldPropertyType"));
                }
            }
        }

        /// <summary>
        /// Serialize helper for Xml Serialization of FieldPropertyType
        /// </summary>
        [Browsable(false)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public string FieldPropertyTypeString
        {
            get
            {
                return Syncfusion.Styles.ValueConvert.GetTypeName(FieldPropertyType);
            }

            set
            {
                FieldPropertyType = Syncfusion.Styles.ValueConvert.GetType(value);
            }
        }

        /// <summary>
        /// Determines if the FieldPropertyType of the field was modified.
        /// </summary>
        /// <returns>True if the content were modified; Fasle otherwise.</returns>
        public bool ShouldSerializeFieldPropertyType()
        {
            return fieldPropertyType != null;
        }

        /// <summary>
        /// Resets the FieldPropertyType property.
        /// </summary>
        public void ResetFieldPropertyType()
        {
            FieldPropertyType = null;
        }
        
        /// <summary>
        /// Gets / sets Hide state of the field.
        /// </summary>
        [Description("Gets / Sets Hide state of the field.")]
        public virtual bool Hide
        {
            get
            {
                return hide;
            }

            set
            {
                if (Hide != value)
                {
                    OnPropertyChanging(new DescriptorPropertyChangedEventArgs("Hide"));
                    hide = value;
                    hideModified = true;
                    OnPropertyChanged(new DescriptorPropertyChangedEventArgs("Hide"));
                }
            }
        }

        /// <summary>
        /// Determines if the Hide state of the field was modified.
        /// </summary>
        /// <returns>True if the content were modified; False otherwise.</returns>
        public bool ShouldSerializeHide()
        {
            return hideModified;
        }

        /// <summary>
        /// Resets the Hide property to False.
        /// </summary>
        public void ResetHide()
        {
            Hide = false;
            hideModified = false;
        }

        /// <summary>
        /// Gets / sets whether the <see cref="GetValue"/> method should trim whitespace at end of text
        /// if field is a string. This is useful if databases return string with fixed character length
        /// and fill up the rest of field with blanks. (Default is True.)
        /// </summary>
        [Description("Gets / Sets whether the GetValue method should trim whitespace at the end of the text.")]
        public virtual bool AllowTrimEnd
        {
            get
            {
                return allowTrimEnd;
            }

            set
            {
                if (AllowTrimEnd != value)
                {
                    OnPropertyChanging(new DescriptorPropertyChangedEventArgs("AllowTrimEnd"));
                    allowTrimEnd = value;
                    allowTrimEndModified = true;
                    OnPropertyChanged(new DescriptorPropertyChangedEventArgs("AllowTrimEnd"));
                }
            }
        }

        /// <summary>
        /// Determines if the AllowTrimEnd state of the field was modified.
        /// </summary>
        /// <returns>True if the state was modified; False otherwise.</returns>
        public bool ShouldSerializeAllowTrimEnd()
        {
            return allowTrimEndModified;
        }

        /// <summary>
        /// Resets the AllowTrimEnd property to False.
        /// </summary>
        public void ResetAllowTrimEnd()
        {
            AllowTrimEnd = false;
            allowTrimEndModified = false;
        }

        /// <summary>
        /// Gets / sets whether changes to the field in a record should immeditaly trigger a SaveValue
        /// event without first calling BeginEdit on the current record. This property is usefull if you do
        /// want to have unbound fields that should not trigger ListChanged events when only the unbound property
        /// is modified on the current record.
        /// </summary>
        [Description("Gets / sets whether changes to the field in a record should immediately trigger a SaveValue event without first calling BeginEdit on the current record.")]
        public virtual bool ForceImmediateSaveValue
        {
            get
            {
                if (!forceImmediateSaveValueModified)
                {
                    if (this.unboundFieldDescriptor != null)
                    {
                        return this.unboundFieldDescriptor.forceImmediateSaveValue;
                    }
                }

                return forceImmediateSaveValue;
            }

            set
            {
                if (ForceImmediateSaveValue != value)
                {
                    OnPropertyChanging(new DescriptorPropertyChangedEventArgs("ForceImmediateSaveValue"));
                    forceImmediateSaveValue = value;
                    forceImmediateSaveValueModified = true;
                    OnPropertyChanged(new DescriptorPropertyChangedEventArgs("ForceImmediateSaveValue"));
                }
            }
        }

        /// <summary>
        /// Determines if the ForceImmediateSaveValue state of the field was modified.
        /// </summary>
        /// <returns>True if the content were modified; False otherwise.</returns>
        public bool ShouldSerializeForceImmediateSaveValue()
        {
            return forceImmediateSaveValueModified;
        }

        /// <summary>
        /// Resets the ForceImmediateSaveValue property to False.
        /// </summary>
        public void ResetForceImmediateSaveValue()
        {
            ForceImmediateSaveValue = false;
            forceImmediateSaveValueModified = false;
        }

        /// <summary>
        /// Retrieves the value for this field from the underlying record.
        /// </summary>
        /// <param name="data">The DataRow with data for this field.</param>
        /// <returns>The value from the underlying record.</returns>
        public object GetValueFromDataRow(object data)
        {
            if (data == null || data is DBNull)
            {
                return null;
            }

            try
            {
                return pd.GetValue(data);
            }
            catch (Exception ex)
            {
                TraceUtil.TraceExceptionCatched(ex);
                return null;
            }
        }

        /// <summary>
        /// Retrieves the value for this field from the underlying record.
        /// </summary>
        /// <param name="record">The record with data for this field.</param>
        /// <returns>The value from the underlying record.</returns>
        public virtual object GetValue(Record record)
        {
            object obj = _GetValue(record);

            if (obj is string && this.AllowTrimEnd)
            {
                obj = ((string)obj).TrimEnd();
            }

            return obj;
        }

        object _GetValue(Record record)
        {
            if (this.IsExpressionField())
            {
                object value = TableDescriptor.ExpressionFieldEvaluator.ComputeFormulaValueAt(this.expressionFieldDescriptor.GetCompiledExpression(), record, this.expressionFieldDescriptor.Name);
                ////return NullableHelper.ChangeType(value, this.GetPropertyType(), this.TableDescriptor.Engine.Culture);
                return Syncfusion.Styles.ValueConvert.ChangeType(value, this.GetPropertyType(), this.TableDescriptor.Engine.Culture);
            }
            else if (this.IsComplexPropertyField())
            {
                object data = record.GetData();
                if (data == null)
                {
                    return null;
                }

                try
                {
                    for (int n = 0; n < complexPropertyDescriptors.Length - 1; n++)
                    {
                        data = this.complexPropertyDescriptors[n].GetValue(data);
                        if (data == null || data is DBNull)
                        {
                            // System.Diagnostics.Trace.WriteLine(complexPropertyDescriptors[n].Name + " is null");
                            return null;
                        }
                    }

                    return pd.GetValue(data);
                }
                catch (Exception ex)
                {
                    TraceUtil.TraceExceptionCatched(ex);
                    return null;
                    ////                    this.tableDescriptor.itemPropertiesVersion++;
                    ////                    this.collection.EnsureMapping(this);
                    ////                    return pd.GetValue(data);
                }
            }
            else if (this.IsRelatedField())
            {
                // RelationKind.ForeignKeyKeyWords
                if (this.relation.RelationKind == RelationKind.ForeignKeyKeyWords)
                {
                    ChildTable childTable = record.GetRelatedChildTable(relation);
                    if (childTable == null)
                    {
                        return null;
                    }

                    // Now get the value from the related childTable.
                    Table relatedTable = childTable.ParentTable;
                    ITreeTableSummary[] summaries = childTable.GetSummaries(relatedTable);
                    int index = relatedTable.TableDescriptor.Summaries.IndexOf(this.GetRelatedSummaryDescriptor());
                    if (index == -1)
                    {
                        return null;
                    }

                    VectorSummary summary = summaries[index] as VectorSummary;
                    return summary.Values;
                }                                             
                else
                {
                    //// ForeignListItems
                    //// RelationKind.ListItemReference
                    //// RelationKind.ForeignKeyReference
                    Record relatedRecord = record.GetRelatedRecord(this.relation);
                    if (relatedRecord == null)
                    {
                        return null;
                    }

                    return relatedRecord.GetValue(this.relatedDescriptor);
                }
            }
            else if (this.IsPropertyField())
            {
                object data = record.GetData();
                if (data == null)
                {
                    return null;
                }

                try
                {
                    return GetPropertyDescriptor().GetValue(data);
                }
                catch (Exception ex)
                {
                    TraceUtil.TraceExceptionCatched(ex);
                    return null;
                    ////                    this.tableDescriptor.itemPropertiesVersion++;
                    ////                    this.collection.EnsureMapping(this);
                    ////                    return pd.GetValue(data);
                }
            }
            else
            {
                if (this.tableDescriptor != null)
                {
                    FieldValueEventArgs ev = new FieldValueEventArgs(this.unboundFieldDescriptor != null ? unboundFieldDescriptor : this, record, this.defaultValue);
                    this.tableDescriptor.RaiseQueryValue(ev);

                    if (!preventUnboundFieldTypeCheck
                        && unboundFieldDescriptor != null
                        && unboundFieldDescriptor.fieldPropertyType != null)
                    {
                        return Syncfusion.Styles.ValueConvert.ChangeType(ev.Value, unboundFieldDescriptor.fieldPropertyType, this.TableDescriptor.Engine.Culture);
                    }

                    return ev.Value;
                }
            }

            return null;
        }

        /// <summary>
        /// Set this true if the newly added type check for value returned from
        /// QueryValue causes compatibility issues.
        /// </summary>
        static bool preventUnboundFieldTypeCheck = false;

        /// <summary>Internal only.</summary>
        /// <internalonly/>
        [Syncfusion.Documentation.DocumentationExclude()]
        [EditorBrowsable(EditorBrowsableState.Never)]
        [System.Xml.Serialization.XmlIgnore]
        [DesignerSerializationVisibilityAttribute(DesignerSerializationVisibility.Hidden)]
        [Browsable(false)]
        public static bool PreventUnboundFieldTypeCheck
        {
            get { return FieldDescriptor.preventUnboundFieldTypeCheck; }
            set { FieldDescriptor.preventUnboundFieldTypeCheck = value; }
        }

        internal bool SaveInEditableRow(Record r, object value)
        {
            if (!Syncfusion.Grouping.Engine.AllowSaveIntoEditableObjectWhileEditing)
            {
                return false;
            }

            if (this.IsPropertyField() && !this.IsComplexPropertyField())
            {
                object data = r.GetData();
                if (data is IEditableObject || data is System.Data.DataRow)
                {
                    if (value == null && (data is System.Data.DataRow || data is System.Data.DataRowView))
                    {
                        value = DBNull.Value;
                    }

                    GetPropertyDescriptor().SetValue(data, value);
                    return true;
                }
#if SyncfusionFramework4_0
                else if(this.TableDescriptor.Engine.IsDynamicData)
                {
                    GetPropertyDescriptor().SetValue(data, value);
                    return true;
                }
#endif
            }

            return false;
        }
        
        /// <overload>
        /// Saves the value into the underlying datasource. If the field is
        /// an unbound field, a <see cref="Engine.SaveValue"/> event is raised.
        /// </overload>
        /// <summary>
        /// Saves the value into the underlying datasource. If the field is
        /// an unbound field, a <see cref="Engine.SaveValue"/> event is raised.
        /// </summary>
        /// <param name="record">The record</param>
        /// <param name="value">The new value.</param>
        public void SetValue(Record record, object value)
        {
            SetValue(record, value, true);
        }

        /// <summary>
        /// Saves the value into the underlying datasource. If the field is
        /// an unbound field, a <see cref="Engine.SaveValue"/> event is raised.
        /// </summary>
        /// <param name="record">The record.</param>
        /// <param name="value">The new value.</param>
        /// <param name="forceListChanged">Specifies if a ListChanged event should be raised.</param>
        public virtual void SetValue(Record record, object value, bool forceListChanged)
        {
            PropertyDescriptor pd = this.GetPropertyDescriptor();
            if (pd != null)
            {
                Table table = record.ParentTable;

                if (this.IsComplexPropertyField())
                {
                    object data = record.GetData();
                    for (int n = 0; n < complexPropertyDescriptors.Length - 1; n++)
                    {
                        PropertyDescriptor ipd = this.complexPropertyDescriptors[n];
                        object pdata = data;
                        data = ipd.GetValue(data);
                        if (data == null || data is DBNull)
                        {
                            if (ipd.IsReadOnly)
                            {
                                throw new Exception("Trying to save value into a complex property that is Read-only and NULL. Make sure complex objects are always allocated in your data structure or make sure that it is not Read-only.");
                            }

                            data = Activator.CreateInstance(ipd.PropertyType);
                            if (data == null)
                            {
                                throw new Exception("Trying to save value into a complex property that NULL and Activator.CreateInstance(ipd.PropertyType) failed.");
                            }

                            ipd.SetValue(pdata, data);
                        }
                    }

                    if (data != null)
                    {
                        pd.SetValue(data, value);
                    }
                }
                else
                {
                    table.wasItemChanged = false;

                    value = NullableHelper.FixDbNUllasNull(value, pd.PropertyType);
                    value = NullableHelper.ChangeType(value, pd.PropertyType);
                    pd.SetValue(record.GetData(), value);
                }

                if (forceListChanged && !table.wasItemChanged)
                {
                    int index = table.UnsortedRecords.IndexOf(record);
                    if (!table.wasItemChanged)
                    {
                        table.SimulateListChanged(new ListChangedEventArgs(ListChangedType.ItemChanged, index, index));
                    }
                }
            }
            else
            {
                this.TableDescriptor.RaiseSaveValue(new FieldValueEventArgs(this, record, value));
            }
        }

        internal void InitializeMapping(TableDescriptor tableDescriptor)
        {
            ////            TableDescriptor parentTableDescriptor = tableDescriptor.ParentTableDescriptor;
            string mappingName = MappingName;
            int dot = mappingName.IndexOf('.');
            if (dot != -1)
            {
                bool shouldReturn = false;
                string relationName = mappingName.Substring(0, dot);
                string fieldName = mappingName.Substring(dot + 1);
                /* Possible ParentTableDescriptor.Fields support
                                if (parentTableDescriptor != null && relationName == parentTableDescriptor.Name)
                                {
                                    int relatedFieldIndex = parentTableDescriptor.Fields.IndexOf(fieldName);
                                    if (relatedFieldIndex != -1)
                                    {
                                        this.SetMapping(null, null, parentTableDescriptor.Fields[relatedFieldIndex], null);
                                        return;
                                    }
                                }
                */
                int relationIndex = tableDescriptor.Relations.IndexOf(relationName);
                FieldDescriptor parentField = tableDescriptor.Fields[relationName];
                RelationDescriptor rd = null;
                int relatedFieldIndex = -1;
                FieldDescriptor relatedFieldDescriptor = null;

                if (relationIndex != -1)
                {
                    rd = tableDescriptor.Relations[relationIndex];

                    if (rd.RelationKind == RelationKind.ForeignKeyKeyWords)
                    {
                        rd.ChildTableDescriptor.EnsureSummaryDescriptors();
                        int summaryIndex = rd.ChildTableDescriptor.Summaries.IndexOf(fieldName);
                        if (summaryIndex != -1)
                        {
                            relatedSummaryDescriptor = rd.ChildTableDescriptor.Summaries[summaryIndex];
                        }
                        else
                        {
                            Console.WriteLine("Could not find Summary {0} in {1}", fieldName, rd.ChildTableDescriptor.Name);
                            Console.WriteLine(rd.ChildTableDescriptor.Summaries.ToString());
                        }             // ForeignListItems
                    }

                    relatedFieldIndex = rd.ChildTableDescriptor.Fields.IndexOf(fieldName);
                    if (relatedFieldIndex != -1)
                    {
                        shouldReturn = true;
                        relatedFieldDescriptor = rd.ChildTableDescriptor.Fields[relatedFieldIndex];
                        this.SetMapping(null, rd, relatedFieldDescriptor, null);
                        if (parentField == null)
                        {
                            return;
                        }
                    }
                }

                if (rd == null || rd.RelationKind == RelationKind.ListItemReference)
                {
                    ArrayList al = new ArrayList();
                    PropertyDescriptor complexPd = tableDescriptor.ItemProperties[relationName];
                    if (complexPd != null)
                    {
                        al.Add(complexPd);
                        FillNestedPropertyDescriptors(al, ListUtil.GetItemProperties(complexPd.PropertyType), fieldName);
                    }

                    if (al.Count > 1)
                    {
                        this.SetMapping((PropertyDescriptor[])al.ToArray(typeof(PropertyDescriptor)), rd, relatedFieldDescriptor, parentField);
                        if (relationIndex != -1)
                        {
                        }

                        return;
                    }
                }

                if (shouldReturn)
                {
                    return;
                }
            }

            int expressionFieldNum = tableDescriptor.ExpressionFields.IndexOf(mappingName);
            if (expressionFieldNum != -1)
            {
                SetMapping(null, null, null, tableDescriptor.ExpressionFields[expressionFieldNum]);
                return;
            }

            int unboundFieldNum = tableDescriptor.UnboundFields.IndexOf(mappingName);
            if (unboundFieldNum != -1)
            {
                this.unboundFieldDescriptor = tableDescriptor.UnboundFields[unboundFieldNum];
                return;
            }

            SetMapping(tableDescriptor.ItemProperties[mappingName], null, null, null);
        }

        void FillNestedPropertyDescriptors(ArrayList al, PropertyDescriptorCollection pdc, string mappingName)
        {
            int dot = mappingName.IndexOf('.');
            if (dot == -1)
            {
                if (pdc.Count > 0 && pdc[mappingName] != null)
                {
                    al.Add(pdc[mappingName]);
                }
            }
            else
            {
                string relationName = mappingName.Substring(0, dot);
                string fieldName = mappingName.Substring(dot + 1);
                PropertyDescriptor complexPd = pdc[relationName];
                if (complexPd == null)
                {
                    al.Clear();
                }
                else
                {
                    al.Add(complexPd);
                    FillNestedPropertyDescriptors(al, ListUtil.GetItemProperties(complexPd.PropertyType), fieldName);
                }
            }
        }

        internal void EnsureMapping(TableDescriptor tableDescriptor)
        {
            if (this.version != this.Collection.Version)
            {
                version = Collection.version;
                InitializeMapping(tableDescriptor);
            }
        }
        
        /// <summary>Returns string representation of the descriptor.</summary>
        /// <returns>String representation of the current object.</returns>
        /// <override/>
        public override string ToString()
        {
            return GetType().Name + " { " + Name + " }";
        }

        /// <summary>Internal only.</summary>
        /// <internalonly/>
        [Syncfusion.Documentation.DocumentationExclude()]
        [EditorBrowsable(EditorBrowsableState.Never)]
        [System.Xml.Serialization.XmlIgnore]
        [DesignerSerializationVisibilityAttribute(DesignerSerializationVisibility.Hidden)]
        [Browsable(false)]
        public bool IsSorting
        {
            get
            {
                return isSorting;
            }

            set
            {
                if (value)
                {
                    CollectionEnsureMapping();
                }

                isSorting = value;
            }
        }

        /// <summary>
        /// Gets / sets ReferencedFields state of the field. Use a semicolon as delimiter
        /// when specifying multiple fields. The Engine will use these fields in the ListChanged event
        /// to determine which cells to update when a change was made in an underlying field.
        /// </summary>
        [Description("Gets / Sets ReferencedFields state of the field.")]
        public virtual string ReferencedFields
        {
            get
            {
                if (unboundFieldDescriptor != null
                    && unboundFieldDescriptor.referencedFields.Length > 0)
                {
                    return unboundFieldDescriptor.referencedFields;
                }

                if (this.referencedFields.Length > 0)
                {
                    return referencedFields;
                }

                if (refFields == null)
                {
                    refFields = DetermineReferencedFields();
                }

                return refFields;
            }

            set
            {
                ////if (!this.IsUnboundField())
                ////    return;

                if (value == null)
                {
                    value = string.Empty;
                }

                if (referencedFields != value)
                {
                    OnPropertyChanging(new DescriptorPropertyChangedEventArgs("ReferencedFields"));
                    referencedFields = value;
                    OnPropertyChanged(new DescriptorPropertyChangedEventArgs("ReferencedFields"));
                }
            }
        }

        /// <summary>
        /// Determines if the ReferencedFields state of the field was modified.
        /// </summary>
        /// <returns>True if the content were modified; False otherwise.</returns>
        public virtual bool ShouldSerializeReferencedFields()
        {
           ////if (!this.IsUnboundField())
            ////    return false;

            return referencedFields.Length > 0;
        }

        /// <summary>
        /// Resets the ReferencedFields property to False.
        /// </summary>
        public void ResetReferencedFields()
        {
            ReferencedFields = string.Empty;
        }

        string refFields = null;

        /// <summary>
        /// Returns an array of field descriptor names that the expression references
        /// </summary>
        /// <returns>returns array of field descriptor names</returns>
        protected virtual string DetermineReferencedFields()
        {
            if (this.IsUnboundField())
            {
                return string.Empty;
            }

            // Foreign Key Display Fields, Complex Property Fields etc.
            FieldDescriptor fd = this.GetParentFieldDescriptor();
            if (fd != null)
            {
                return fd.Name;
            }

            return string.Empty;
        }
    }

    #endregion
    #region QueryShowNestedPropertiesFields
    // eva QueryShowNestedPropertiesFields SyncfusionCancel TableDescriptor tableDescriptor QueryShowNestedPropertiesFields propertyDescriptor

    /// <summary>
    /// Represents a method that handles an event with <see cref="QueryShowNestedPropertiesFieldsEventArgs"/>.
    /// </summary>
    /// <param name="sender">The source of the event.</param>
    /// <param name="e">The event data.</param>
    public delegate void QueryShowNestedPropertiesFieldsEventHandler(object sender, QueryShowNestedPropertiesFieldsEventArgs e);

    /// <summary>
    /// The Engine.QueryShowNestedPropertiesFields event affects the autopopulation of the FieldDescriptorCollection. <para/>
    /// It lets you control at run-time if individual fields should be added for every property of a type
    /// when a type has nested properties. You can set e.Cancel = True to avoid nested fields
    /// being generated for a specific type.
    /// </summary>
    public sealed class QueryShowNestedPropertiesFieldsEventArgs : SyncfusionCancelEventArgs
    {
        TableDescriptor tableDescriptor;
        PropertyDescriptor propertyDescriptor;

        /// <summary>
        /// Initializes the event args
        /// </summary>
        /// <param name="tableDescriptor">The tableDescriptor </param>
        /// <param name="propertyDescriptor">the propertyDescriptor</param>
        public QueryShowNestedPropertiesFieldsEventArgs(TableDescriptor tableDescriptor, PropertyDescriptor propertyDescriptor)
        {
            this.tableDescriptor = tableDescriptor;
            this.propertyDescriptor = propertyDescriptor;
        }

        /// <summary>
        /// The TableDescriptor
        /// </summary>
        [TraceProperty(true)]
        public TableDescriptor TableDescriptor
        {
            get
            {
                return tableDescriptor;
            }

            set
            {
                tableDescriptor = value;
            }
        }

        /// <summary>
        /// The PropertyDescriptor
        /// </summary>
        [TraceProperty(true)]
        public PropertyDescriptor PropertyDescriptor
        {
            get
            {
                return propertyDescriptor;
            }

            set
            {
                propertyDescriptor = value;
            }
        }
    }

    #endregion
    #region QueryShowField
    // eva QueryShowField SyncfusionCancel TableDescriptor tableDescriptor FieldDescriptor field

    /// <summary>
    /// Represents a method that handles an event with <see cref="QueryShowFieldEventArgs"/>.
    /// </summary>
    /// <param name="sender">The source of the event.</param>
    /// <param name="e">The event data.</param>
    public delegate void QueryShowFieldEventHandler(object sender, QueryShowFieldEventArgs e);

    /// <summary>
    /// The Engine.QueryShowField event affects the autopopulation of the FieldDescriptorCollection. <para/>
    /// It is called for each field and lets you control at run-time if a specific field should be added
    /// to the FieldDescriptorCollection. You can set e.Cancel = True to avoid specific fields
    /// being added.
    /// </summary>
    public sealed class QueryShowFieldEventArgs : SyncfusionCancelEventArgs
    {
        TableDescriptor tableDescriptor;
        FieldDescriptor field;

        /// <summary>
        /// Initializes the event args
        /// </summary>
        public QueryShowFieldEventArgs(TableDescriptor tableDescriptor, FieldDescriptor field)
        {
            this.tableDescriptor = tableDescriptor;
            this.field = field;
        }

        /// <summary>
        /// The TableDescriptor
        /// </summary>
        [TraceProperty(true)]
        public TableDescriptor TableDescriptor
        {
            get
            {
                return tableDescriptor;
            }
        }

        /// <summary>
        /// The FieldDescriptor
        /// </summary>
        [TraceProperty(true)]
        public FieldDescriptor Field
        {
            get
            {
                return field;
            }
        }
    }

    #endregion
    #region QueryShowRelationFields

    // eva QueryShowRelationFields SyncfusionCancel TableDescriptor tableDescriptor RelationDescriptor relation ShowRelationFields showRelationFields

    /// <summary>
    /// Represents a method that handles an event with <see cref="QueryShowRelationFieldsEventArgs"/>.
    /// </summary>
    /// <param name="sender">The source of the event.</param>
    /// <param name="e">The event data.</param>
    public delegate void QueryShowRelationFieldsEventHandler(object sender, QueryShowRelationFieldsEventArgs e);

    /// <summary>
    /// The Engine.QueryShowRelationFields event affects the autopopulation of the FieldDescriptorCollection. <para/>
    /// It is called for each foreign key relation and lets you control at run-time if the related fields of the
    /// child table should be added to the FieldDescriptorCollection. You can set e.Cancel = True to avoid specific fields
    /// being added.
    /// </summary>
    public sealed class QueryShowRelationFieldsEventArgs : SyncfusionCancelEventArgs
    {
        TableDescriptor tableDescriptor;
        RelationDescriptor relation;
        ShowRelationFields showRelationFields;

        /// <summary>
        /// Initializes the event args
        /// </summary>
        public QueryShowRelationFieldsEventArgs(TableDescriptor tableDescriptor, RelationDescriptor relation, ShowRelationFields showRelationFields)
        {
            this.tableDescriptor = tableDescriptor;
            this.relation = relation;
            this.showRelationFields = showRelationFields;
        }

        /// <summary>
        /// The TableDescriptor
        /// </summary>
        [TraceProperty(true)]
        public TableDescriptor TableDescriptor
        {
            get
            {
                return tableDescriptor;
            }
        }

        /// <summary>
        /// The Relation
        /// </summary>
        [TraceProperty(true)]
        public RelationDescriptor Relation
        {
            get
            {
                return relation;
            }
        }

        /// <summary>
        /// Specifies if dependent fields from a related table in a foreign key relation (or related collection)
        /// should be added to the
        /// main table's FieldDescriptorCollection. You can also control this behavior at run-time
        /// with the QueryShowRelationDisplayFields event. Default is ShowRelationFields.ShowDisplayFieldsOnly.
        /// </summary>
        [TraceProperty(true)]
        public ShowRelationFields ShowRelationFields
        {
            get
            {
                return showRelationFields;
            }

            set
            {
                showRelationFields = value;
            }
        }
    }

    /// <summary>
    /// The Engine.ShowRelationField property affects the autopopulation of the FieldDescriptorCollection. <para/>
    /// It specifies if dependent fields from a related table in a foreign key relation (or related collection)
    /// should be added to the
    /// main tables FieldDescriptorCollection. You can also control this behavior at run-time
    /// with the QueryShowRelationDisplayFields event. Default is ShowRelationFields.ShowDisplayFieldsOnly.
    /// </summary>
    public enum ShowRelationFields
    {
        /// <summary>
        /// Don't show fields.
        /// </summary>
        Hide,

        /// <summary>
        /// Show only dependent fields; hide primary and foreign keys.
        /// </summary>
        ShowDisplayFieldsOnly,

        /// <summary>
        /// Show all related fields including primary and foreign keys.
        /// </summary>
        ShowAllRelatedFields
    }
    #endregion

    /// <summary>
    /// Implements a <see cref="TypeConverter"/> for the <see cref="FieldDescriptor.FieldPropertyType"/> property in
    /// <see cref="FieldDescriptor"/>.
    /// </summary>
    internal class TypeTypeConverter : TypeConverter
    {
        /// <summary>
        /// Returns whether this converter can convert an object of the given type to the type of this converter, using the specified context.
        /// </summary>
        /// <param name="context">An <see cref="T:System.ComponentModel.ITypeDescriptorContext"/> that provides a format context.</param>
        /// <param name="sourceType">A <see cref="T:System.Type"/> that represents the type you want to convert from.</param>
        /// <returns>
        /// true if this converter can perform the conversion; otherwise, false.
        /// </returns>
        /// <override/>
        public override bool CanConvertFrom(ITypeDescriptorContext context, Type sourceType)
        {
            if (sourceType == typeof(System.String))
            {
                return true;
            }

            return base.CanConvertFrom(context, sourceType);
        }

        /// <summary>
        /// Converts the given object to the type of this converter, using the specified context and culture information.
        /// </summary>
        /// <param name="context">An <see cref="T:System.ComponentModel.ITypeDescriptorContext"/> that provides a format context.</param>
        /// <param name="culture">The <see cref="T:System.Globalization.CultureInfo"/> to use as the current culture.</param>
        /// <param name="value">The <see cref="T:System.Object"/> to convert.</param>
        /// <returns>
        /// An <see cref="T:System.Object"/> that represents the converted value.
        /// </returns>
        /// <override/>
        public override object ConvertFrom(ITypeDescriptorContext context, CultureInfo culture, object value)
        {
            if (value is System.String)
            {
                if (((string)value).Length > 0)
                {
                    return Type.GetType((string)value);
                }
                else
                {
                    return null;
                }
            }

            return base.ConvertFrom(context, culture, value);
        }

        // no string conversion

        /// <summary>
        /// Converts the given value object to the specified type, using the specified context and culture information.
        /// </summary>
        /// <param name="context">An <see cref="T:System.ComponentModel.ITypeDescriptorContext"/> that provides a format context.</param>
        /// <param name="culture">A <see cref="T:System.Globalization.CultureInfo"/>. If null is passed, the current culture is assumed.</param>
        /// <param name="value">The <see cref="T:System.Object"/> to convert.</param>
        /// <param name="destinationType">The <see cref="T:System.Type"/> to convert the <paramref name="value"/> parameter to.</param>
        /// <returns>
        /// An <see cref="T:System.Object"/> that represents the converted value.
        /// </returns>
        /// <override/>
        public override object ConvertTo(ITypeDescriptorContext context, CultureInfo culture, object value, Type destinationType)
        {
            if (destinationType == typeof(String))
            {
                Type type = (Type)value;
                if (type == null)
                {
                    return String.Empty;
                }
                else if (type.Namespace == "System")
                {
                    return type.ToString();
                }
                else
                {
                    return String.Concat(type.FullName, ",", type.AssemblyQualifiedName.Split(',')[1]);
                }
            }

            return base.ConvertFrom(context, culture, value);
        }

        /// <summary>
        /// Returns a collection of standard values for the data type this type converter is designed for when provided with a format context.
        /// </summary>
        /// <param name="context">An <see cref="T:System.ComponentModel.ITypeDescriptorContext"/> that provides a format context that can be used to extract additional information about the environment from which this converter is invoked. This parameter or properties of this parameter can be null.</param>
        /// <returns>
        /// A <see cref="T:System.ComponentModel.TypeConverter.StandardValuesCollection"/> that holds a standard set of valid values, or null if the data type does not support a standard set of values.
        /// </returns>
        /// <override/>
        public override System.ComponentModel.TypeConverter.StandardValuesCollection GetStandardValues(ITypeDescriptorContext context)
        {
            return svc;
        }

        /// <summary>
        /// Returns whether the collection of standard values returned from <see cref="M:System.ComponentModel.TypeConverter.GetStandardValues"/> is an exclusive list of possible values, using the specified context.
        /// </summary>
        /// <param name="context">An <see cref="T:System.ComponentModel.ITypeDescriptorContext"/> that provides a format context.</param>
        /// <returns>
        /// true if the <see cref="T:System.ComponentModel.TypeConverter.StandardValuesCollection"/> returned from <see cref="M:System.ComponentModel.TypeConverter.GetStandardValues"/> is an exhaustive list of possible values; false if other values are possible.
        /// </returns>
        /// <override/>
        public override bool GetStandardValuesExclusive(ITypeDescriptorContext context)
        {
            return false;
        }

        /// <summary>
        /// Returns whether this object supports a standard set of values that can be picked from a list, using the specified context.
        /// </summary>
        /// <param name="context">An <see cref="T:System.ComponentModel.ITypeDescriptorContext"/> that provides a format context.</param>
        /// <returns>
        /// true if <see cref="M:System.ComponentModel.TypeConverter.GetStandardValues"/> should be called to find a common set of values the object supports; otherwise, false.
        /// </returns>
        /// <override/>
        public override bool GetStandardValuesSupported(ITypeDescriptorContext context)
        {
            return true;
        }

        // Fields
        static TypeTypeConverter()
        {
            values = new string[] 
                {
                    "System.String",
                    "System.Double",
                    "System.Int32",
                    "System.Boolean",
                    ////"System.Drawing.Color, System.Drawing",
                    "System.DateTime",
                    "System.Int16",
                    "System.Int64",
                    "System.Single",
                    "System.Byte",
                    "System.Char",
                    "System.Decimal",
                    "System.UInt16",
                    "System.UInt32",
                    "System.UInt64",
                    ////"System.Windows.Forms.DockStyle, System.Windows.Forms",
                /*typeof(System.String),
                    typeof(System.Double),
                    typeof(System.Int32),
                    typeof(System.Boolean),
                    typeof(System.Drawing.Color),
                    typeof(System.DateTime),
                    typeof(System.Int16),
                    typeof(System.Int64),
                    typeof(System.Single),
                    typeof(System.SByte),
                    typeof(System.Byte),
                    typeof(System.Char),
                    typeof(System.Decimal),
                    typeof(System.DBNull),
                    typeof(System.UInt16),
                    typeof(System.UInt32),
                    typeof(System.UInt64),*/
            };
            Array.Sort(values);
            Type[] types = new Type[values.Length];
            for (int i = 0; i < values.Length; i++)
            {
                types[i] = Type.GetType(values[i]);
            }

            svc = new TypeConverter.StandardValuesCollection(types);
        }

        private static string[] values;
        private static TypeConverter.StandardValuesCollection svc;
    }
}

