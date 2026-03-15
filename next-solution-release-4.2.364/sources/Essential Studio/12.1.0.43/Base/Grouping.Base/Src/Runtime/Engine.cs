//-------------------------------------------------------------------------------------------------
// <copyright file="Engine.cs" company="syncfusion">
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
using System.Data;
using System.Diagnostics;
using System.Globalization;
using System.Text;
using System.Windows.Forms.Design;
using System.ComponentModel.Design;
using System.Drawing.Design;
using Syncfusion.Diagnostics;
using Syncfusion.Collections;
using Syncfusion.Collections.BinaryTree;
using Syncfusion.Grouping.Internals;
using Syncfusion.Windows.Forms;

namespace Syncfusion.Grouping
{
    /// <summary>
    /// Provides the <see cref="GetEngine"/> method that returns
    /// an <see cref="Engine"/>.
    /// </summary>
    public interface IEngineSource
    {
        /// <summary>
        /// Returns a reference to an <see cref="Engine"/>
        /// </summary>
        /// <returns>An engine.</returns>
        Engine GetEngine();
    }

    /// <summary>
    /// The engine lets you set the main data source for the whole engine. The TableDescriptor will
    /// pick up the ItemProperties (schema information) from the SourceList and
    /// table will be intialized at run-time with records from the list.
    /// </summary>
    /// <remarks>
    /// TableDescriptor is browsable. You can modify its collections and properties in the
    /// designer. <para/>
    /// By default TableDescriptor is autopopulated. If you do not modify its settings and
    /// later change the datasource, it will be automatically reinitialized. If you have
    /// made modifications to TableDescriptor and change the SourceList, the modifications
    /// will be kept. To discard modifications of a TableDescriptor, you need to explicitly
    /// call ResetTableDescriptor.<para/>
    /// Table is dependent on information provided by TableDescriptor and the records from
    /// SourceList. It is created on the fly and can not be designed with designer.
    /// </remarks>
    public class Engine : DescriptorBase, ITableEventsTarget
    {
        TableDescriptor tableDescriptor;
        Table table;
        IEnumerable sourceList;
        SourceListSet sourceListSet;
        bool inSetSourceList;
        bool inSourceListChanged;
        internal int version;
        int sourceListVersion;
        bool shouldCreateRecordWithCache = false;
        internal int tableNoCounter;

        bool useInvariantCultureInfoForExpressions = false;

        /// <summary>
        /// Returns true if underlying datasource does not raise ColumnChanging events
        /// and therefore the record should cached old values in order to be able
        /// to compare values when a ListChanged event is handled. The property
        /// returns the value of CacheRecordValues if you explicitly set the <see cref="CacheRecordValues"/> property
        /// thus allowing you to ovveride the default caching behavior of the engine.
        /// </summary>
        [Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public bool ShouldCreateRecordWithCache
        {
            get
            {
                if (cacheRecordValuesModified)
                {
                    return cacheRecordValues;
                }

                return shouldCreateRecordWithCache;
            }
        }

        bool allowSetRelatedTablesDirty = true;
        /// <summary>
        /// Gets or sets whether related tables are set dirty when the parent table receives a 
        /// ListChangedType.Reset in its ListChanged event. The default value is true.
        /// </summary>
        [DefaultValue(true)]
        [Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public bool AllowSetRelatedTablesDirty
        {
            get { return allowSetRelatedTablesDirty; }
            set { allowSetRelatedTablesDirty = value; }
        }

        bool cacheRecordValues = false;
        bool cacheRecordValuesModified = false;

        /// <summary>
        /// Gets / sets if the engine should cache copies of the old values from a record in the record object.
        /// You can access these values with the Record.GetOldValue method. Setting this property will override
        /// the default value returned by <see cref="ShouldCreateRecordWithCache"/>.
        /// </summary>
        [Description("Specifies if the engine should cache copies of the old values from a record in the record object.")]
        [Category("Optimization")]
        public bool CacheRecordValues
        {
            get
            {
                return cacheRecordValues;
            }

            set
            {
                if (this.cacheRecordValues != value)
                {
                    this.OnPropertyChanging(new DescriptorPropertyChangedEventArgs("CacheRecordValues"));
                    this.cacheRecordValues = value;
                    this.OnPropertyChanged(new DescriptorPropertyChangedEventArgs("CacheRecordValues"));
                }

                cacheRecordValuesModified = true;
            }
        }

        /// <summary>
        /// Determines if the <see cref="CacheRecordValues"/> has been modified from its
        /// default state.
        /// </summary>
        /// <returns>True if CacheRecordValues was manually modified; </returns>
        public bool ShouldSerializeCacheRecordValues()
        {
            return cacheRecordValuesModified;
        }

        /// <summary>
        /// Resets the <see cref="CacheRecordValues"/> back to its default value.
        /// </summary>
        public void ResetCacheRecordValues()
        {
            bool resetTable = false;
            if (this.cacheRecordValuesModified && cacheRecordValues != shouldCreateRecordWithCache)
            {
                resetTable = true;
            }

            if (resetTable)
            {
                this.OnPropertyChanging(new DescriptorPropertyChangedEventArgs("CacheRecordValues"));
            }

            cacheRecordValuesModified = false;
            cacheRecordValues = shouldCreateRecordWithCache;

            if (resetTable)
            {
                this.OnPropertyChanged(new DescriptorPropertyChangedEventArgs("CacheRecordValues"));
            }
        }

        ////tableDirtyOnSourceListReset 

        bool tableDirtyOnSourceListReset = false;
        bool tableDirtyOnSourceListResetModified = false;

        /// <summary>
        /// Gets / sets if the engine should set <see cref="Table.TableDirty"/> to true when the data source
        /// raises a ListChanged event with ListChangedType.Reset notification. The default is false.
        /// </summary>
        [Description("Specifies if the engine should set TableDirty to true when the data source raises ListChangedType.Reset.")]
        [Category("Optimization")]
        public bool TableDirtyOnSourceListReset
        {
            get
            {
                return tableDirtyOnSourceListReset;
            }

            set
            {
                if (this.tableDirtyOnSourceListReset != value)
                {
                    this.OnPropertyChanging(new DescriptorPropertyChangedEventArgs("TableDirtyOnSourceListReset"));
                    this.tableDirtyOnSourceListReset = value;
                    tableDirtyOnSourceListResetModified = true;
                    this.OnPropertyChanged(new DescriptorPropertyChangedEventArgs("TableDirtyOnSourceListReset"));
                }
            }
        }

        /// <summary>
        /// Determines if the <see cref="TableDirtyOnSourceListReset"/> has been modified from its
        /// default state.
        /// </summary>
        /// <returns>True if TableDirtyOnSourceListReset was manually modified; </returns>
        public bool ShouldSerializeTableDirtyOnSourceListReset()
        {
            return tableDirtyOnSourceListResetModified;
        }

        /// <summary>
        /// Resets the <see cref="TableDirtyOnSourceListReset"/> back to its default value.
        /// </summary>
        public void ResetTableDirtyOnSourceListReset()
        {
            tableDirtyOnSourceListResetModified = false;
            tableDirtyOnSourceListReset = false;
        }

        bool showDefaultValuesInAddNewRecord = false;

        /// <summary>
        /// Gets / sets if the default value for fields in the AddNewRecord should be shown when it is not in edit-mode. If 
        /// the ShowDefaultValuesInAddNewRecord is false then the default values will only be assigned when
        /// AddNewRecord.BeginEdit is called. Prior calls to AddNewRecord will return no value in that case.
        /// </summary>
        [DefaultValue(false)]
        [Description("Specifies whether the default value for fields in the AddNewRecord should be shown when it is not in edit-mode.")]
        [Category("Optimization")]
        public bool ShowDefaultValuesInAddNewRecord
        {
            get
            {
                return showDefaultValuesInAddNewRecord;
            }

            set
            {
                if (showDefaultValuesInAddNewRecord != value)
                {
                    OnPropertyChanging(new DescriptorPropertyChangedEventArgs("ShowDefaultValuesInAddNewRecord"));
                    showDefaultValuesInAddNewRecord = value;
                    OnPropertyChanged(new DescriptorPropertyChangedEventArgs("ShowDefaultValuesInAddNewRecord"));
                }
            }
        }

        /// <summary>
        /// Gets or sets whether numbers and dates in expressions should be in invariant format or UI culture dependant format
        /// </summary>
        /// <remarks>
        /// In invariant format numbers and dates are always in english format, e.g. use a '.' as decimal separator and dates in format mm/dd/yy.
        /// In UI culture dependant format the decimal separator can be a ',' or some other setting specified in CultureInfo.CurrentUICulture.
        /// </remarks>
        [DefaultValue(false)]
        [Description("Specifies whether numbers and dates in expressions should be in invariant format or UI culture dependant format")]
        [Category(@"Control")]
        public bool UseInvariantCulture
        {
            get
            {
                return useInvariantCultureInfoForExpressions;
            }

            set
            {
                useInvariantCultureInfoForExpressions = value;
                if (!cultureInfoModified)
                {
                    cultureInfo = null;
                }
            }
        }

        static bool defaultUseOldUniformChildListRelation = false;

        /// <summary>
        /// With version 4.2 the engine changed the way how UniformChildList relations are handled internally
        /// to fix short-comings of the design that was in place earlier. This property lets you switch back
        /// the behavior of the engine to the old mechanism if you notice compatibility issues. This static member
        /// defines the defaut value for the instance-specific UseOldUniformChildListRelation property for all
        /// engines in your application.
        /// </summary>
        [System.Xml.Serialization.XmlIgnore]
        public static bool DefaultUseOldUniformChildListRelation
        {
            get { return defaultUseOldUniformChildListRelation; }
            set { defaultUseOldUniformChildListRelation = value; }
        }

        private bool useOldUniformChildListRelation;

        /// <summary>
        /// With version 4.2 the engine changed the way how UniformChildList relations are handled internally
        /// to fix short-comings of the design that was in place earlier. This property lets you switch back
        /// the behavior of the engine to the old mechanism if you notice compatibility issues. The default value is false.
        /// </summary>
        [Description("Specifies whether UniformChildList relations should be handled internally the same way as vith version 4.1 and earlier.")]
        public bool UseOldUniformChildListRelation
        {
            get { return useOldUniformChildListRelation; }
            set { useOldUniformChildListRelation = value; }
        }

        /// <summary>
        /// Determines whether <see cref="UseOldUniformChildListRelation"/> has been modified and its value should be serialized.
        /// </summary>
        /// <returns>True if the content is changed; False otherwise.</returns>
        public bool ShouldSerializeUseOldUniformChildListRelation()
        {
            return useOldUniformChildListRelation != defaultUseOldUniformChildListRelation;
        }

        /// <summary>
        /// Resets UseOldUniformChildListRelation.
        /// </summary>
        public void ResetUseOldUniformChildListRelation()
        {
            useOldUniformChildListRelation = defaultUseOldUniformChildListRelation;
        }

        static bool defaultUseLazyUniformChildListRelation = false;

        /// <summary>
        /// When you use the new UniformChildList behavior (i.e. when UseOldUniformChildListRelation = false) 
        /// you can specify with this UseLazyUniformChildListRelation property whether the engine should 
        /// access and enumerate the child collections only once the user expands a record.
        /// This will speed up load time of the grid and reduce memory usage when not all records 
        /// get expanded.
        /// </summary>
        [System.Xml.Serialization.XmlIgnore]
        public static bool DefaultUseLazyUniformChildListRelation
        {
            get { return defaultUseLazyUniformChildListRelation; }
            set { defaultUseLazyUniformChildListRelation = value; }
        }

        private bool useLazyUniformChildListRelation;

        /// <summary>
        /// When you use the new UniformChildList behavior (i.e. when UseOldUniformChildListRelation = false) 
        /// you can specify with this UseLazyUniformChildListRelation property whether the engine should 
        /// access and enumerate the child collections only once the user expands a record.
        /// This will speed up load time of the grid and reduce memory usage when not all records 
        /// get expanded.
        /// </summary>
        [Description("Specifies whether UniformChildList relations should be handled internally the same way as vith version 4.1 and earlier.")]
        public bool UseLazyUniformChildListRelation
        {
            get { return useLazyUniformChildListRelation; }
            set { useLazyUniformChildListRelation = value; }
        }

        /// <summary>
        /// Determines whether <see cref="UseLazyUniformChildListRelation"/> has been modified and its value should be serialized.
        /// </summary>
        /// <returns>True if the content is changed; False otherwise.</returns>
        public bool ShouldSerializeUseLazyUniformChildListRelation()
        {
            return useLazyUniformChildListRelation != defaultUseLazyUniformChildListRelation;
        }

        /// <summary>
        /// Resets UseLazyUniformChildListRelation.
        /// </summary>
        public void ResetUseLazyUniformChildListRelation()
        {
            useLazyUniformChildListRelation = defaultUseLazyUniformChildListRelation;
        }

        static bool defaultUseOldListChangedHandler = false;

        /// <summary>
        /// With version 4.4 the engine changed the way how the ListChanged event is handled internally
        /// to fix short-comings with performance of the code that was in place earlier. This property 
        /// lets you switch back
        /// the behavior of the engine to the old mechanism if you notice compatibility issues. This static member
        /// defines the defaut value for the instance-specific UseOldListChangedHandler property for all
        /// engines in your application.
        /// </summary>
        [System.Xml.Serialization.XmlIgnore]
        public static bool DefaultUseOldListChangedHandler
        {
            get { return defaultUseOldListChangedHandler; }
            set { defaultUseOldListChangedHandler = value; }
        }

        private bool useOldListChangedHandler;

        /// <summary>
        /// With version 4.4 the engine changed the way how the ListChanged event is handled internally
        /// to fix short-comings with performance of the code that was in place earlier. This property 
        /// lets you switch back
        /// the behavior of the engine to the old mechanism if you notice compatibility issues. The default value is false.
        /// </summary>
        [Description("Specifies whether ListChanged event should be handled internally the same way as vith version 4.3 and earlier.")]
        [Category("Optimization")]
        public bool UseOldListChangedHandler
        {
            get { return useOldListChangedHandler; }
            set { useOldListChangedHandler = value; }
        }

        /// <summary>
        /// Determines whether <see cref="UseOldListChangedHandler"/> has been modified and its value should be serialized.
        /// </summary>
        /// <returns>True if the content is changed; False otherwise.</returns>
        public bool ShouldSerializeUseOldListChangedHandler()
        {
            return useOldListChangedHandler != defaultUseOldListChangedHandler;
        }

        /// <summary>
        /// Resets UseOldListChangedHandler;
        /// </summary>
        public void ResetUseOldListChangedHandler()
        {
            useOldListChangedHandler = defaultUseOldListChangedHandler;
        }

        /*public virtual void ApplyDefaultOptimizations()
        {
            CounterLogic = EngineCounters.YAmount;
            AllowedOptimizations = EngineOptimizations.DisableCounters;
        } */

        bool raiseSourceListChangedEventsOnEngineOnly = true;

        /// <summary>
        /// When the engine handles the ListChanged event it will itsself raise numerous events. When set
        /// to true this the events will only be raised on the Engine object. If set to false
        /// then events will also be raised on inner objects (will bubble up on nested tables which
        /// caused some performance overhead). Property will only have effect if UseOldListChangedHandler = false.
        /// </summary>
        [DefaultValue(true)]
        [Description("Raise events that are triggerer by the tables ListChanged handler only on the engine object or also on nested inner objects.")]
        [Category("Optimization")]
        public bool RaiseSourceListChangedEventsOnEngineOnly
        {
            get { return raiseSourceListChangedEventsOnEngineOnly; }
            set { raiseSourceListChangedEventsOnEngineOnly = value; }
        }

        CultureInfo cultureInfo;
        bool cultureInfoModified = false;

        /// <summary>
        /// Gets or sets the the culture information which holds rules for parsing and formatting numbers and dates
        /// in expression fields.
        /// </summary>
        /// <remarks>
        /// In invariant format numbers and dates are always in english format, e.g. use a '.' as decimal separator and dates in format mm/dd/yy.
        /// In UI culture dependant format the decimal separator can be a ',' or some other setting specified in CultureInfo.CurrentUICulture.
        /// </remarks>
        [Description("The culture information holds rules for parsing and formatting the cell's value."),
        Browsable(false),
        TypeConverter(typeof(CultureInfoConverter)),
        RefreshProperties(RefreshProperties.Repaint),
        ImmutableObject(true)]
        [System.Xml.Serialization.XmlIgnore]
        [DesignerSerializationVisibilityAttribute(DesignerSerializationVisibility.Hidden)]
        [NotifyParentProperty(true)]
        public CultureInfo Culture
        {
            get
            {
                if (cultureInfo == null)
                {
                    if (useInvariantCultureInfoForExpressions)
                    {
                        cultureInfo = CultureInfo.InvariantCulture;
                    }
                    else
                    {
                        cultureInfo = CultureInfo.CurrentCulture;
                    }
                }

                return cultureInfo;
            }

            set
            {
                cultureInfoModified = !Object.ReferenceEquals(cultureInfo, CultureInfo.CurrentCulture);
                cultureInfo = value;
            }
        }

        /// <summary>
        /// Determines whether <see cref="Culture"/> has been modified and its value should be serialized.
        /// </summary>
        /// <returns>True if the content is changed; False otherwise.</returns>
        public bool ShouldSerializeCultureInfo()
        {
            return cultureInfoModified;
        }

        /// <summary>
        /// Resets CultureInfo.
        /// </summary>
        public void ResetCultureInfo()
        {
            cultureInfoModified = false;
            cultureInfo = null;
        }

        internal char NumberDecimalSeparator
        {
            get
            {
                return Convert.ToChar(Culture.NumberFormat.NumberDecimalSeparator);
            }
        }

        /// <summary>
        /// Debug helper. Produces a debug log for creation and desctruction of important objects.
        /// </summary>
        [System.Xml.Serialization.XmlIgnore]
        public static bool VerboseEnsureObjectLifeTime = false;

        /// <summary>
        /// Occurs when a property is changed.
        /// </summary>
        [Description("Occurs when a property is changed.")]
        public event DescriptorPropertyChangedEventHandler PropertyChanged;

        /// <summary>
        /// Occurs before a property is changed.
        /// </summary>
        [Description("Occurs before a property is changed.")]
        public event DescriptorPropertyChangedEventHandler PropertyChanging;

        /// <internalonly/>
        [Syncfusion.Documentation.DocumentationExclude()]
        [EditorBrowsable(EditorBrowsableState.Never)]
        [System.Xml.Serialization.XmlIgnore]
        public static bool HelpTracing = false;

        /// <internalonly/>
        [Syncfusion.Documentation.DocumentationExclude()]
        [EditorBrowsable(EditorBrowsableState.Never)]
        [System.Xml.Serialization.XmlIgnore]
        public static bool ThrowExceptionIfSourceListSetEntryNotFound = true;

        /// <summary>
        /// Initializes this object and copies properties from another object. <see cref="PropertyChanging"/>
        /// and <see cref="PropertyChanged"/> events are raised for every property that is modified. If both
        /// objects are equal, no events are raised.
        /// </summary>
        /// <param name="other">The source object.</param>
        public virtual void InitializeFrom(Engine other)
        {
            inInitializeFrom = true;
            if (other.ShouldSerializeTableDescriptor())
            {
                TableDescriptor.InitializeFrom(other.TableDescriptor);
            }
            else
            {
                ResetTableDescriptor();
            }

            this.AllowedOptimizations = other.AllowedOptimizations;
            this.AllowSwapDataViewWithDataTableList = other.AllowSwapDataViewWithDataTableList;
            this.AutoPopulateRelations = other.AutoPopulateRelations;
            this.CounterLogic = other.CounterLogic;
            this.Culture = other.Culture;
            this.ShowNestedPropertiesFields = other.ShowNestedPropertiesFields;
            this.ShowRelationFields = other.ShowRelationFields;
            this.UseInvariantCulture = other.UseInvariantCulture;
            #if SyncfusionFramework4_0
            this.IsDynamicData = other.IsDynamicData;
            #endif

            if (other.ShouldSerializeCacheRecordValues())
            {
                this.CacheRecordValues = other.CacheRecordValues;
            }
            else
            {
                this.ResetCacheRecordValues();
            }

            if (other.ShouldSerializeTableDirtyOnSourceListReset())
            {
                this.TableDirtyOnSourceListReset = other.TableDirtyOnSourceListReset;
            }
            else
            {
                this.ResetTableDirtyOnSourceListReset();
            }

            if (other.ShouldSerializeUseOldListChangedHandler())
            {
                this.UseOldListChangedHandler = other.UseOldListChangedHandler;
            }
            else
            {
                this.ResetUseOldListChangedHandler();
            }

            this.RaiseSourceListChangedEventsOnEngineOnly = other.RaiseSourceListChangedEventsOnEngineOnly;

            if (other.ShouldSerializeUseOldUniformChildListRelation())
            {
                this.UseOldUniformChildListRelation = other.UseOldUniformChildListRelation;
            }
            else
            {
                this.ResetUseOldUniformChildListRelation();
            }

            this.SortMappingNames = other.SortMappingNames;

            inInitializeFrom = false;
        }

        bool inInitializeFrom = false;

        /// <summary>For internal use.</summary>
        /// <internalonly/>
        [Syncfusion.Documentation.DocumentationExclude()]
        [EditorBrowsable(EditorBrowsableState.Never)]
        [System.Xml.Serialization.XmlIgnore]
        [Browsable(false)]
        [DesignerSerializationVisibilityAttribute(DesignerSerializationVisibility.Hidden)]
        public virtual bool InInitializeFrom
        {
            get
            {
                return inInitializeFrom;
            }
        }

#if SyncfusionFramework4_0
        private bool isDynamicData = false;
        /// <summary>
        /// Gets or Sets whether the bounded datasource is composed of dynamic objects. Applicable only for .NET Framework 4.0 and later.
        /// </summary>
        [DesignerSerializationVisibilityAttribute(DesignerSerializationVisibility.Visible)]
        [Browsable(false)]
        [Description("Gets or sets whether the bounded datasource is composed of dynamic objects or not. Applicable only for .NET Framework 4.0 and later.")]
        public bool IsDynamicData
        {
            get
            {
                return isDynamicData;
            }
            set
            {
                isDynamicData = value;
            }
        }
#endif
        ITableEventsTarget tableEventsTarget;

        /// <summary>
        /// Gets / sets a object that handles events raised by contained <see cref="Table"/> objects.
        /// </summary>
        [System.Xml.Serialization.XmlIgnore]
        [DesignerSerializationVisibilityAttribute(DesignerSerializationVisibility.Hidden)]
        [Browsable(false)]
        public ITableEventsTarget ForwardTableEvents
        {
            get
            {
                return this.tableEventsTarget;
            }

            set
            {
                this.tableEventsTarget = value;
            }
        }

        /// <summary>Returns table descriptor name.</summary>
        /// <returns>Table descriptor name.</returns>
        /// <override/>
        public override string GetName()
        {
            return TableDescriptor.Name;
        }

        /// <summary>Returns string representation of the engine.</summary>
        /// <returns>String representation of the current object.</returns>
        /// <override/>
        public override string ToString()
        {
            return GetType().Name + " { " + TableDescriptor.Name + " }";
        }

        /// <summary>
        /// Occurs when a record is checked whether it meets filter criteria and should appear visible in the tables DisplayElements.
        /// </summary>
        [Description("Occurs when a record is checked whether it meets filter criteria and should appear visible in the tables DisplayElements.")]
        public event QueryRecordMeetsFilterCriteriaEventHandler QueryRecordMeetsFilterCriteria;

        /// <summary>
        /// Raises the  <see cref="QueryRecordMeetsFilterCriteria"/> event.
        /// </summary>
        /// <param name="e">A <see cref="QueryRecordMeetsFilterCriteriaEventArgs" /> that contains the event data.</param>
        protected virtual void OnQueryRecordMeetsFilterCriteria(QueryRecordMeetsFilterCriteriaEventArgs e)
        {
            ////TraceUtil.TraceCurrentMethodInfoIf(Switches.GridControlBaseEvents.TraceVerbose, Name, e);
            if (QueryRecordMeetsFilterCriteria != null)
            {
                QueryRecordMeetsFilterCriteria(this, e);
            }
        }

        internal void RaiseQueryRecordMeetsFilterCriteria(QueryRecordMeetsFilterCriteriaEventArgs e)
        {
            OnQueryRecordMeetsFilterCriteria(e);
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
#if DEBUG

            if (Switches.DescriptorPropertyChange.TraceVerbose)
            {
                TraceUtil.TraceCurrentMethodInfo(e);
            }
#else

            ;
#endif
            if (PropertyChanged != null)
            {
                PropertyChanged(this, e);
            }

            if (e.PropertyName == "CacheRecordValues")
            {
                this.ResetTable();
            }

            TableDescriptor.EnableOneTimePopulate();
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
#if DEBUG
            if (Switches.DescriptorPropertyChange.TraceVerbose)
            {
                TraceUtil.TraceCurrentMethodInfo(e);
            }
#else

            ;
#endif
            if (PropertyChanging != null)
            {
                PropertyChanging(this, e);
            }
        }

        /// <summary>
        /// A reference to the <see cref="SourceListSet"/> that maintains a collection
        /// of IList or DataTables that are used by main table or related tables as datasources.
        /// </summary>
        [System.Xml.Serialization.XmlIgnore]
        [DesignerSerializationVisibilityAttribute(DesignerSerializationVisibility.Hidden)]
        [Browsable(false)]
        public SourceListSet SourceListSet
        {
            get
            {
                return sourceListSet;
            }
        }

        /// <summary>
        /// The version number of this engine. The version is increased each time the
        /// engine (or any container schema descriptor) was modified.
        /// </summary>
        [System.Xml.Serialization.XmlIgnore]
        [DesignerSerializationVisibilityAttribute(DesignerSerializationVisibility.Hidden)]
        [Browsable(false)]
        public int Version
        {
            get
            {
                return version;
            }
        }

        /// <summary>
        /// The version number for the underlying data source. This number is increased each time
        /// <see cref="SetSourceList"/> is called.
        /// </summary>
        [System.Xml.Serialization.XmlIgnore]
        [DesignerSerializationVisibilityAttribute(DesignerSerializationVisibility.Hidden)]
        [Browsable(false)]
        public int SourceListVersion
        {
            get
            {
                return sourceListVersion;
            }
        }

        /// <summary>
        /// Increase the <see cref="Version"/> number of this engine.
        /// </summary>
        public void BumpVersion()
        {
            lastGetVisibleInHierarchyElement = null;
            version++;
        }

        internal Element lastGetVisibleInHierarchyElement;
        internal bool lastGetVisibleInHierarchyElementReturnValue;

        /// <summary>
        /// Determines whether the engine is attached to a component or control that is currently being
        /// designed in the Visual Studio designer.
        /// </summary>
        /// <returns>True if the engine is attached to the control being designed; False otherwise.</returns>
        public virtual bool GetDesignMode()
        {
            return false;
        }

        /// <summary>
        /// Initializes the new Engine
        /// </summary>
        public Engine()
        {
            useOldUniformChildListRelation = defaultUseOldUniformChildListRelation;
            if (Utilities.IsSecurityPermissionAvailable())
            {
                Utilities.ValidateLicense(typeof(GroupingConfig));
            }
            ////comments other codes here, that are related to security check, which was added in UtilityClass.
            ////try
            ////{
            ////    AppDomain.CurrentDomain.AssemblyResolve += new ResolveEventHandler(AssemblyInfo.AssemblyResolver);
            ////   new Syncfusion.Core.Licensing.LicensedComponent(typeof(GroupingConfig));
            ////}
            ////finally
            ////{
            ////AppDomain.CurrentDomain.AssemblyResolve -= new ResolveEventHandler(AssemblyInfo.AssemblyResolver);
            ////}            
            this.sourceListSet = new SourceListSet(this);
        }

        /// <override/>
        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                this.UnwireSourceList();
                if (this.table != null)
                {
                    table.Dispose();
                }

                if (this.tableDescriptor != null)
                {
                    tableDescriptor.Dispose();
                }

                if (this.sourceListSet != null)
                {
                    this.sourceListSet.Dispose();
                }

                table = null;
                tableDescriptor = null;
                this.sourceListSet = null;
                this.tableEventsTarget = null;
                sourceList = null;
                this.table = null;
            }

            base.Dispose(disposing);
        }

        /// <summary>
        /// Occurs after the main data source was replaced with a <see cref="SetSourceList"/> method call.
        /// </summary>
        /// <remarks>
        /// With a GridGroupingControl when you set the data source and data member, the <see cref="SetSourceList"/> method
        /// will be called on demand the first time data or schema information needs to be retrieved.
        /// </remarks>
        [Description("Occurs after the main data source was replaced.")]
        public event EventHandler SourceListChanged;

        /// <summary>
        /// Raises the <see cref="SourceListChanged"/> event.
        /// </summary>
        /// <param name="e">An <see cref="EventArgs" /> that contains the event data.</param>
        protected virtual void OnSourceListChanged(EventArgs e)
        {
            object source = this.GetSourceList();
            if (source != null)
            {
                if (source is DataSet
                    || source is DataView
                    || source is IGroupingColumnChanging
                    || source is DataTable
                    || source is DataTableList)
                {
                }
                else
                {
                    this.shouldCreateRecordWithCache = true;
                }
            }

            if (SourceListChanged != null)
            {
                SourceListChanged(this, e);
            }
        }

        /// <summary>
        /// Returns True when the <see cref="SetSourceList"/> method is called and False after the method returns.
        /// </summary>
        [System.Xml.Serialization.XmlIgnore]
        [DesignerSerializationVisibilityAttribute(DesignerSerializationVisibility.Hidden)]
        [Browsable(false)]
        public bool InSetSourceList
        {
            get
            {
                return inSetSourceList;
            }
        }

        /// <summary>
        /// Returns True when the <see cref="SourceListChanged"/> event is raised and False after the event returns.
        /// </summary>
        [System.Xml.Serialization.XmlIgnore]
        [DesignerSerializationVisibilityAttribute(DesignerSerializationVisibility.Hidden)]
        [Browsable(false)]
        public bool InSourceListChanged
        {
            get
            {
                return inSourceListChanged;
            }
        }

        /// <summary>
        /// This virtual method is called when there has previously been no datasource specified and
        /// a <see cref="GetSourceList"/> call occurs. Override this method to allow retrieving the
        /// source list on demand.
        /// </summary>
        /// <returns>The source list for this engine.</returns>
        /// <remarks>
        /// With a GridGroupingControl, the GridEngineBase method overloads this method to bind the engine
        /// to a datasource on demand. When this method is called, GridEngineBase will locate a CurrencyManager
        /// based on the given DataSource and DataMember properties. The resulting source list will then be
        /// passed as an argument to a <see cref="SetSourceList"/> method call.
        /// <para/>
        /// The reasoning for the way the <see cref="GetSourceListBase"/>, <see cref="GetSourceList"/>,
        /// <see cref="SetSourceList"/>, and <see cref="ShouldReplaceSourceList"/> methods work together is to
        /// delay initializing the source list until all information has been specified to correctly locate
        /// the list. This allows the end user to set DataSource and DataMember properties without having to worry
        /// about the order when the properties are set. It will also ensure that with Windows Forms applications,
        /// the currency manager context has been initalized.
        /// </remarks>
        protected virtual IEnumerable GetSourceListBase()
        {
            return null;
        }

        /// <summary>
        /// Determines if <see cref="SetSourceList"/> was called and source list has been set.
        /// </summary>
        /// <returns>True if <see cref="SetSourceList"/> was called and source list has been set; False otherwise.</returns>
        public bool HasSourceList()
        {
            return sourceList != null;
        }

        /// <summary>
        /// Returns a source list previously specifed with <see cref="SetSourceList"/>. If no source list
        /// was specified or if <see cref="ShouldReplaceSourceList"/> returns True, the virtual <see cref="GetSourceListBase"/>
        /// method is called and the  resulting source list will then be
        /// passed as argument to a <see cref="SetSourceList"/> method call.
        /// </summary>
        /// <returns>The source list for this engine.</returns>
        /// <remarks>
        /// The reasoning for the way the <see cref="GetSourceListBase"/>, <see cref="GetSourceList"/>,
        /// <see cref="SetSourceList"/>, and <see cref="ShouldReplaceSourceList"/> methods work together is to
        /// delay initializing the source list until all information has been specified to correctly locate
        /// the list. This allows the end user to set DataSource and DataMember properties without having to worry
        /// about the order when the properties are set. It will also ensure that with Windows Forms applications,
        /// the currency manager context has been initalized.
        /// </remarks>
        public IEnumerable GetSourceList()
        {
            if (!inSetSourceList && !inSourceListChanged && (sourceList == null || ShouldReplaceSourceList()))
            {
                SetSourceList(GetSourceListBase());
            }

            return sourceList;
        }

        /// <summary>
        /// Determines if the sourcelist should be reinitialized the next time <see cref="GetSourceList"/> is called.
        /// </summary>
        /// <returns>True if the sourcelist should be reinitialized the next time <see cref="GetSourceList"/> is called; False if not.</returns>
        /// <remarks>
        /// With a GridGroupingControl, the GridEngineBase method overloads this method and returns
        /// True if either the DataSource or DataMember property was changed.
        /// <para/>
        /// The reasoning for the way the <see cref="GetSourceListBase"/>, <see cref="GetSourceList"/>,
        /// <see cref="SetSourceList"/>, and <see cref="ShouldReplaceSourceList"/> methods work together is to
        /// delay initializing the source list until all information has been specified to correctly locate
        /// the list. This allows the end user to set DataSource and DataMember properties without having to worry
        /// about the order when the properties are set. It will also ensure that with Windows Forms applications,
        /// the currency manager context has been initalized.
        /// </remarks>
        protected virtual bool ShouldReplaceSourceList()
        {
            return false;
        }

        IEnumerable swSourceList;

        /// <summary>
        /// Initalizes the the source list for the engine.
        /// </summary>
        /// <param name="value">The new source list for the engine.</param>
        /// <remarks>
        /// If the passed in source list is the same as a previous <see cref="SetSourceList"/> call, the
        /// method will return without change to avoid duplicate initialization of the same list.
        /// <para/>
        /// The <see cref="Table"/> will be reinitialized and the <see cref="Syncfusion.Grouping.SourceListDescriptor.SetItemProperties"/>
        /// of the <see cref="Syncfusion.Grouping.TableDescriptor"/> is called. A <see cref="SourceListChanged"/> event is raised.
        /// </remarks>
        public void SetSourceList(IEnumerable value)
        {
            if (inSourceListChanged)
            {
                swSourceList = value;
                if (value is System.Data.DataView && this.AllowSwapDataViewWithDataTableList)
                {
                    value = new DataTableList(((System.Data.DataView)value).Table);
                }

                sourceList = value;
                return;
            }

            if (swSourceList != value)
            {
                inSetSourceList = true;
                UnwireSourceList();
                swSourceList = value;
                if (value is System.Data.DataView && this.AllowSwapDataViewWithDataTableList)
                {
                    value = new DataTableList(((System.Data.DataView)value).Table);
                }

                sourceList = value;
                ResetTable();
                // && tableDescriptor.ShouldSerializeItemProperties())
                if (tableDescriptor != null)
                {
                    tableDescriptor.SetItemProperties(sourceList);
                }

                WireSourceList();
                BumpVersion();
                sourceListVersion++;
                inSetSourceList = false;
                try
                {
                    inSourceListChanged = true;
                    OnSourceListChanged(EventArgs.Empty);
                }
                finally
                {
                    inSourceListChanged = false;
                }
            }
        }

        /// <summary>
        /// This method is called after the source list has been attached.
        /// </summary>
        protected virtual void WireSourceList()
        {
            if (HasSourceList())
            {
                IBindingList bindingList = GetSourceList() as IBindingList;
                if (bindingList != null)
                {
                    bindingList.ListChanged += new ListChangedEventHandler(bindingList_ListChanged);
                }
            }
        }

        /// <summary>
        /// This method is called before the source list is detached.
        /// </summary>
        protected virtual void UnwireSourceList()
        {
            if (HasSourceList())
            {
                IBindingList bindingList = GetSourceList() as IBindingList;
                if (bindingList != null)
                {
                    bindingList.ListChanged -= new ListChangedEventHandler(bindingList_ListChanged);
                }
            }
        }

        private void bindingList_ListChanged(object sender, ListChangedEventArgs e)
        {
            switch (e.ListChangedType)
            {
                case ListChangedType.PropertyDescriptorAdded:
                case ListChangedType.PropertyDescriptorChanged:
                case ListChangedType.PropertyDescriptorDeleted:
                case ListChangedType.Reset:
                    if (this.table == null && this.tableDescriptor != null)
                    {
                        this.TableDescriptor.SetItemProperties(GetSourceList());
                    }
                    // otherwise the Table's bindingList_ListChanged handler will notify TableDescriptor
                    break;
            }
        }

        /// <summary>
        /// Maintains the table schema information of the root table in the datasource.
        /// </summary>
        [DesignerSerializationVisibilityAttribute(DesignerSerializationVisibility.Content)]
        [Browsable(true)]
        public TableDescriptor TableDescriptor
        {
            get
            {
                if (tableDescriptor == null)
                {
                    tableDescriptor = CreateTableDescriptor(null);
                    if (sourceList != null)
                    {
                        tableDescriptor.SetItemProperties(sourceList);
                    }

                    tableDescriptor.ForwardTableEvents = this;
                    OnTableDescriptorCreated(EventArgs.Empty);
                }

                return tableDescriptor;
            }
        }

        /// <summary>
        /// Determines if the <see cref="TableDescriptor"/> has been modified from its
        /// default state.
        /// </summary>
        /// <returns>True if TableDescriptor was manually modified; False if it only contains auto-populated
        /// data.</returns>
        public bool ShouldSerializeTableDescriptor()
        {
            return tableDescriptor != null && tableDescriptor.GetModified();
        }

        public override void Reset()
        {
            ResetCacheRecordValues();
            ResetCultureInfo();
            ResetMaxNestedCollectionRecurseLevel();
            ResetMaxNestedFieldRecurseLevel();
            ResetTable();
            ResetTableDescriptor();
            ResetTableDirtyOnSourceListReset();
            ResetUseLazyUniformChildListRelation();
            ResetUseOldListChangedHandler();
            ResetUseOldUniformChildListRelation();
            base.Reset();
        }
        /// <summary>
        /// Resets the <see cref="TableDescriptor"/> back to its
        /// default state and auto-populates schema information on demand.
        /// </summary>
        public void ResetTableDescriptor()
        {
            ResetTable();

            if (tableDescriptor != null)
            {
                tableDescriptor.ResetTableDescriptor();
            }
        }

        bool inGetTable = false;

        /// <summary>
        /// Occurs after the <see cref="TableDescriptor"/> object is created.
        /// </summary>
        [Description("Occurs after the TableDescriptor object is created.")]
        public event EventHandler TableDescriptorCreated;

        /// <summary>
        /// Raises the <see cref="TableDescriptorCreated"/> event.
        /// </summary>
        /// <param name="e">An <see cref="EventArgs" /> that contains the event data.</param>
        protected virtual void OnTableDescriptorCreated(EventArgs e)
        {
            if (TableDescriptorCreated != null)
            {
                TableDescriptorCreated(this, e);
            }
        }

        /// <summary>
        /// Occurs after the <see cref="Table"/> object is created.
        /// </summary>
        [Description("Occurs after the Table object is created.")]
        public event EventHandler TableCreated;

        /// <summary>
        /// Raises the <see cref="TableCreated"/> event.
        /// </summary>
        /// <param name="e">An <see cref="EventArgs" /> that contains the event data.</param>
        protected virtual void OnTableCreated(EventArgs e)
        {
            if (TableCreated != null)
            {
                TableCreated(this, e);
            }
        }

        /// <summary>
        /// The Table object manages the records from the engine's DataSource and
        /// provides access to records and grouped elements through several
        /// collection classes, most prominent the <see cref="DisplayElementsInTableCollection"/>.
        /// </summary>
        [System.Xml.Serialization.XmlIgnore]
        [DesignerSerializationVisibilityAttribute(DesignerSerializationVisibility.Hidden)]
        [Browsable(false)]
        public Table Table
        {
            get
            {
                if (table == null)
                {
                    inGetTable = true;
                    table = CreateTable(TableDescriptor, null);
                    ////                    if (this.HasSourceList())
                    ////                        table.SourceList = GetSourceList();
                    table.TableEventsTarget = this.TableDescriptor;
                    OnTableCreated(EventArgs.Empty);
                    inGetTable = false;
                }

                return table;
            }
        }

        /// <summary>
        /// Resets the table.
        /// </summary>
        public void ResetTable()
        {
            if (table != null && !inGetTable)
            {
                table.SourceList = null;
                table.CurrentRecordManager.Reset();
                table.TableDirty = true;
            }
        }

        /// <summary>
        /// Determines if the <see cref="Table"/> object was created.
        /// </summary>
        /// <returns>True if the <see cref="Table"/> object was created; False if it is NULL</returns>
        public bool ShouldSerializeTable()
        {
            return table != null;
        }

        /// <summary>
        /// Set this True if you do not want the engine to treat Record and ColumnHeaderSection
        /// elements as ContainerElements and instead have these elements be returned as
        /// a display element in the Table.DisplayElements collection.
        /// </summary>
        /// <remarks>
        /// With a GridGroupingControl, you must not change this property since a GridGroupingControl
        /// relies on the behavior that a record is not a display element but a container for rows
        /// and nested tables.
        /// </remarks>
        [System.Xml.Serialization.XmlIgnore]
        [DesignerSerializationVisibilityAttribute(DesignerSerializationVisibility.Hidden)]
        [Browsable(false)]
        public bool RecordAsDisplayElements
        {
            get
            {
                return (this.AllowedOptimizations & EngineOptimizations.RecordsAsDisplayElements) != 0;
            }

            set
            {
                if (value)
                {
                    AllowedOptimizations |= EngineOptimizations.RecordsAsDisplayElements;
                }
                else
                {
                    AllowedOptimizations &= ~EngineOptimizations.RecordsAsDisplayElements;
                }
            }
        }

        // Factory methods

        /// <summary>
        /// Creates a <see cref="AddNewRecord"/> element.
        /// </summary>
        /// <param name="parent">The parent section.</param>
        /// <returns>The new element.</returns>
        public virtual AddNewRecord CreateAddNewRecord(AddNewRecordSection parent)
        {
            return new AddNewRecord(parent);
        }

        /// <summary>
        /// Creates a <see cref="AddNewRecordSection"/> element.
        /// </summary>
        /// <param name="parent">The parent group.</param>
        /// <returns>The new element.</returns>
        public virtual AddNewRecordSection CreateAddNewRecordSection(Group parent)
        {
            return new AddNewRecordSection(parent);
        }

        /// <summary>
        /// Creates a <see cref="CaptionSection"/> element.
        /// </summary>
        /// <param name="parent">The parent group.</param>
        /// <returns>The new element.</returns>
        public virtual CaptionSection CreateCaptionSection(Group parent)
        {
            return new CaptionSection(parent);
        }

        /// <summary>
        /// Creates a <see cref="CaptionRow"/> element.
        /// </summary>
        /// <param name="parent">The parent section.</param>
        /// <returns>The new element.</returns>
        public virtual CaptionRow CreateCaptionRow(CaptionSection parent)
        {
            return new CaptionRow(parent);
        }

        /// <summary>
        /// Creates a <see cref="FilterBarRow"/> element.
        /// </summary>
        /// <param name="parent">The parent section.</param>
        /// <returns>The new element.</returns>
        public virtual FilterBarRow CreateFilterBarRow(FilterBarSection parent)
        {
            return new FilterBarRow(parent);
        }

        /// <summary>
        /// Creates a <see cref="ChildTable"/> element.
        /// </summary>
        /// <param name="parent">The parent element.</param>
        /// <returns>The new element.</returns>
        public virtual ChildTable CreateChildTable(Element parent)
        {
            return new ChildTable(parent);
        }

        /// <summary>
        /// Creates a <see cref="ColumnHeaderRow"/> element.
        /// </summary>
        /// <param name="parent">The parent section.</param>
        /// <returns>The new element.</returns>
        public virtual ColumnHeaderRow CreateColumnHeaderRow(ColumnHeaderSection parent)
        {
            return new ColumnHeaderRow(parent);
        }

        /// <summary>
        /// Creates a <see cref="ColumnHeaderSection"/> element.
        /// </summary>
        /// <param name="parent">The parent group.</param>
        /// <returns>The new element.</returns>
        public virtual ColumnHeaderSection CreateColumnHeaderSection(Group parent)
        {
            return new ColumnHeaderSection(parent);
        }

        /// <summary>
        /// Creates a <see cref="EmptySection"/> element.
        /// </summary>
        /// <param name="parent">The parent group.</param>
        /// <returns>The new element.</returns>
        public virtual EmptySection CreateEmptySection(Group parent)
        {
            return new EmptySection(parent);
        }

        /// <summary>
        /// Creates a <see cref="FilterBarSection"/> element.
        /// </summary>
        /// <param name="parent">The parent group.</param>
        /// <returns>The new element.</returns>
        public virtual FilterBarSection CreateFilterBarSection(Group parent)
        {
            return new FilterBarSection(parent);
        }

        /// <summary>
        /// Creates a <see cref="Group"/> element.
        /// </summary>
        /// <param name="parent">The parent section.</param>
        /// <returns>The new element.</returns>
        public virtual Group CreateGroup(Section parent)
        {
            return new Group(parent);
        }

        /// <summary>
        /// Creates a <see cref="GroupFooterSection"/> element.
        /// </summary>
        /// <param name="parent">The parent group.</param>
        /// <returns>The new element.</returns>
        public virtual GroupFooterSection CreateGroupFooterSection(Group parent)
        {
            return new GroupFooterSection(parent);
        }

        /// <summary>
        /// Creates a <see cref="GroupPreviewSection"/> element.
        /// </summary>
        /// <param name="parent">The parent group.</param>
        /// <returns>The new element.</returns>
        public virtual GroupPreviewSection CreateGroupPreviewSection(Group parent)
        {
            return new GroupPreviewSection(parent);
        }

        /// <summary>
        /// Creates a <see cref="GroupHeaderSection"/> element.
        /// </summary>
        /// <param name="parent">The parent group.</param>
        /// <returns>The new element.</returns>
        public virtual GroupHeaderSection CreateGroupHeaderSection(Group parent)
        {
            return new GroupHeaderSection(parent);
        }

        /// <summary>
        /// Creates a <see cref="GroupsDetails"/> element.
        /// </summary>
        /// <param name="parent">The parent group.</param>
        /// <returns>The new element.</returns>
        public virtual GroupsDetails CreateGroupsDetails(Group parent)
        {
            return new GroupsDetails(parent);
        }

        /// <summary>
        /// Creates a <see cref="NestedTable"/> element.
        /// </summary>
        /// <param name="parent">The parent element.</param>
        /// <returns>The new element.</returns>
        public virtual NestedTable CreateNestedTable(RecordNestedTablesPart parent)
        {
            return new NestedTable(parent);
        }

        /// <summary>
        /// Creates a <see cref="Record"/> element.
        /// </summary>
        /// <param name="parentTable">The parent table.</param>
        /// <returns>The new element.</returns>
        public virtual Record CreateRecord(Table parentTable)
        {
            if (ShouldCreateRecordWithCache)
            {
                return new RecordWithValueCache(parentTable);
            }

            return new Record(parentTable);
        }

        /// <summary>
        /// Creates a <see cref="RecordNestedTablesPart"/> element.
        /// </summary>
        /// <param name="parent">The parent record.</param>
        /// <returns>The new element.</returns>
        public virtual RecordNestedTablesPart CreateRecordNestedTablesPart(Record parent)
        {
            return new RecordNestedTablesPart(parent);
        }

        /// <summary>
        /// Creates a <see cref="RecordRow"/> element.
        /// </summary>
        /// <param name="parent">The parent element.</param>
        /// <returns>The new element.</returns>
        public virtual RecordRow CreateRecordRow(RecordRowsPart parent)
        {
            return new RecordRow(parent);
        }

        /// <summary>
        /// Creates a <see cref="RecordRowsPart"/> element.
        /// </summary>
        /// <param name="parent">The parent record.</param>
        /// <returns>The new element.</returns>
        public virtual RecordRowsPart CreateRecordRowsPart(Record parent)
        {
            return new RecordRowsPart(parent);
        }

        /// <summary>
        /// Creates a <see cref="RecordPreviewRowsPart"/> element.
        /// </summary>
        /// <param name="parent">The parent record.</param>
        /// <returns>The new element.</returns>
        public virtual RecordPreviewRowsPart CreateRecordPreviewRowsPart(Record parent)
        {
            return new RecordPreviewRowsPart(parent);
        }

        /// <summary>
        /// Creates a <see cref="RecordPreviewRow"/> element.
        /// </summary>
        /// <param name="parent">The parent element.</param>
        /// <returns>The new element.</returns>
        public virtual RecordPreviewRow CreateRecordPreviewRow(RecordPreviewRowsPart parent)
        {
            return new RecordPreviewRow(parent);
        }

        /// <summary>
        /// Creates a <see cref="RecordsDetails"/> element.
        /// </summary>
        /// <param name="parent">The parent group.</param>
        /// <returns>The new element.</returns>
        public virtual RecordsDetails CreateRecordsDetails(Group parent)
        {
            return new RecordsDetails(parent);
        }

        /// <summary>
        /// Creates a <see cref="RowElementsSection"/> element.
        /// </summary>
        /// <param name="parent">The parent group.</param>
        /// <returns>The new element.</returns>
        public virtual RowElementsSection CreateRowElementsSection(Group parent)
        {
            return new RowElementsSection(parent);
        }

        /// <summary>
        /// Creates a <see cref="Section"/> element.
        /// </summary>
        /// <param name="parent">The parent group.</param>
        /// <returns>The new element.</returns>
        public virtual Section CreateSummarySection(Group parent)
        {
            return new SummarySection(parent);
        }

        /// <overload>
        /// Creates a <see cref="Table"/> element for the specified <see cref="TableDescriptor"/>.
        /// </overload>
        /// <summary>
        /// Creates a <see cref="Table"/> element for the specified <see cref="TableDescriptor"/>.
        /// </summary>
        /// <param name="tableDescriptor">The <see cref="TableDescriptor"/> with schema information for the table.</param>
        /// <returns>A new <see cref="Table"/> object.</returns>
        public Table CreateTable(TableDescriptor tableDescriptor)
        {
            Table table = CreateTable(tableDescriptor, null);
            return table;
        }

        /// <summary>
        /// Creates a <see cref="Table"/> element for the specified <see cref="TableDescriptor"/> and parent table.
        /// </summary>
        /// <param name="tableDescriptor">The <see cref="TableDescriptor"/> with schema information for the table.</param>
        /// <param name="parentTable">The parent table that has a relation with this new table.</param>
        /// <returns>A new <see cref="Table"/> object.</returns>
        public virtual Table CreateTable(TableDescriptor tableDescriptor, Table parentTable)
        {
            return new Table(tableDescriptor, parentTable);
        }

        /// <summary>
        /// Creates a <see cref="TableDescriptor"/> that belongs to the specified <see cref="RelationDescriptor"/>.
        /// </summary>
        /// <param name="parentRelation">The <see cref="RelationDescriptor"/> that holds this <see cref="TableDescriptor"/>. If it is NULL,
        /// the return <see cref="TableDescriptor"/> is the main TableDescripor and has no parent relation.</param>
        /// <returns>A new <see cref="TableDescriptor"/> object.</returns>
        public virtual TableDescriptor CreateTableDescriptor(RelationDescriptor parentRelation)
        {
            return new TableDescriptor(this, parentRelation);
        }

        /// <summary>
        /// Creates a <see cref="ExpressionFieldEvaluator"/> that is bound to the specified <see cref="TableDescriptor"/>
        /// and provides routines for parsing and evaluating formula expressions in fields.
        /// </summary>
        /// <param name="tableDescriptor">The <see cref="TableDescriptor"/> this object is is bound to.</param>
        /// <returns>A new <see cref="ExpressionFieldEvaluator"/>.</returns>
        public virtual ExpressionFieldEvaluator CreateExpressionFieldEvaluator(TableDescriptor tableDescriptor)
        {
            return new ExpressionFieldEvaluator(tableDescriptor);
        }

        /// <summary>
        /// Creates a <see cref="RelationDescriptor"/>.
        /// </summary>
        /// <returns>A new <see cref="RelationDescriptor"/>.</returns>
        public virtual RelationDescriptor CreateRelationDescriptor()
        {
            return new RelationDescriptor();
        }

        /// <summary>
        /// Returns the main table descriptor or a table descriptor of any nested relation that matches the
        /// specified name.
        /// </summary>
        /// <param name="name">The name of the table descriptor to search.</param>
        /// <returns>The table descriptor or NULL if not found.</returns>
        public TableDescriptor GetTableDescriptor(string name)
        {
            return FindTableDescriptor(this.TableDescriptor, name);
        }

        TableDescriptor FindTableDescriptor(TableDescriptor tableDescriptor, string name)
        {
            if (name == tableDescriptor.Name)
            {
                return tableDescriptor;
            }

            foreach (RelationDescriptor rd in tableDescriptor.Relations)
            {
                TableDescriptor td = FindTableDescriptor(rd.ChildTableDescriptor, name);
                if (td != null)
                {
                    return td;
                }
            }

            return null;
        }

        // tevent ExceptionRaised ExceptionRaisedEventArgs

        /// <summary>
        /// Occurs when an unknown exception has been cached while modifying underlying data in the datasource.
        /// </summary>
        /// <remarks>
        /// If necessary, you can rethrow the exception in your event handler.
        /// </remarks>
        [Category("Table")]
        [Description("Occurs when a unknown exception has been cached while modifying underlying data in the datasource.")]
        public event ExceptionRaisedEventHandler ExceptionRaised;

        /// <summary>
        /// Raises the <see cref="ExceptionRaised"/> event.
        /// </summary>
        /// <param name="e">A <see cref="ExceptionRaisedEventArgs" /> that contains the event data.</param>
        protected virtual void OnExceptionRaised(ExceptionRaisedEventArgs e)
        {
            if (ExceptionRaised != null)
            {
                ExceptionRaised(this, e);
            }
        }

        void ITableEventsTarget.OnExceptionRaised(ExceptionRaisedEventArgs e)
        {
            if (this.tableEventsTarget != null)
            {
                tableEventsTarget.OnExceptionRaised(e);
            }

            OnExceptionRaised(e);
        }

        // tevent GroupCollapsing GroupEventArgs

        /// <summary>
        /// Occurs before a group is collapsed.
        /// </summary>
        [Description("Occurs before a group is collapsed.")]
        [Category("Table")]
        public event GroupEventHandler GroupCollapsing;

        /// <summary>
        /// Raises the <see cref="GroupCollapsing"/> event.
        /// </summary>
        /// <param name="e">A <see cref="GroupEventArgs" /> that contains the event data.</param>
        protected virtual void OnGroupCollapsing(GroupEventArgs e)
        {
            if (GroupCollapsing != null)
            {
                GroupCollapsing(this, e);
            }
        }

        void ITableEventsTarget.OnGroupCollapsing(GroupEventArgs e)
        {
            if (this.tableEventsTarget != null)
            {
                tableEventsTarget.OnGroupCollapsing(e);
            }

            OnGroupCollapsing(e);
        }

        // tevent GroupCollapsed GroupEventArgs

        /// <summary>
        /// Occurs after a group is collapsed.
        /// </summary>
        [Description("Occurs after a group is collapsed.")]
        [Category("Table")]
        public event GroupEventHandler GroupCollapsed;

        /// <summary>
        /// Raises the <see cref="GroupCollapsed"/> event.
        /// </summary>
        /// <param name="e">A <see cref="GroupEventArgs" /> that contains the event data.</param>
        protected virtual void OnGroupCollapsed(GroupEventArgs e)
        {
            if (GroupCollapsed != null)
            {
                GroupCollapsed(this, e);
            }
        }

        void ITableEventsTarget.OnGroupCollapsed(GroupEventArgs e)
        {
            if (this.tableEventsTarget != null)
            {
                tableEventsTarget.OnGroupCollapsed(e);
            }

            OnGroupCollapsed(e);
        }

        // tevent GroupExpanding GroupEventArgs

        /// <summary>
        /// Occurs before a group is expanded.
        /// </summary>
        [Description("Occurs before a group is expanded.")]
        [Category("Table")]
        public event GroupEventHandler GroupExpanding;

        /// <summary>
        /// Raises the <see cref="GroupExpanding"/> event.
        /// </summary>
        /// <param name="e">A <see cref="GroupEventArgs" /> that contains the event data.</param>
        protected virtual void OnGroupExpanding(GroupEventArgs e)
        {
            if (GroupExpanding != null)
            {
                GroupExpanding(this, e);
            }
        }

        void ITableEventsTarget.OnGroupExpanding(GroupEventArgs e)
        {
            if (this.tableEventsTarget != null)
            {
                tableEventsTarget.OnGroupExpanding(e);
            }

            OnGroupExpanding(e);
        }

        // tevent GroupExpanded GroupEventArgs

        /// <summary>
        /// Occurs after a group is expanded.
        /// </summary>
        [Description("Occurs after a group is expanded.")]
        [Category("Table")]
        public event GroupEventHandler GroupExpanded;

        /// <summary>
        /// Raises the <see cref="GroupExpanded"/> event.
        /// </summary>
        /// <param name="e">A <see cref="GroupEventArgs" /> that contains the event data.</param>
        protected virtual void OnGroupExpanded(GroupEventArgs e)
        {
            if (GroupExpanded != null)
            {
                GroupExpanded(this, e);
            }
        }

        void ITableEventsTarget.OnGroupExpanded(GroupEventArgs e)
        {
            if (this.tableEventsTarget != null)
            {
                tableEventsTarget.OnGroupExpanded(e);
            }

            OnGroupExpanded(e);
        }

        // tevent RecordCollapsing RecordEventArgs

        /// <summary>
        /// Occurs before a record with nested tables is collapsed.
        /// </summary>
        [Description("Occurs before a record with nested tables is collapsed.")]
        [Category("Table")]
        public event RecordEventHandler RecordCollapsing;

        /// <summary>
        /// Raises the <see cref="RecordCollapsing"/> event.
        /// </summary>
        /// <param name="e">A <see cref="RecordEventArgs" /> that contains the event data.</param>
        protected virtual void OnRecordCollapsing(RecordEventArgs e)
        {
            if (RecordCollapsing != null)
            {
                RecordCollapsing(this, e);
            }
        }

        void ITableEventsTarget.OnRecordCollapsing(RecordEventArgs e)
        {
            if (this.tableEventsTarget != null)
            {
                tableEventsTarget.OnRecordCollapsing(e);
            }

            OnRecordCollapsing(e);
        }

        // tevent RecordCollapsed RecordEventArgs

        /// <summary>
        /// Occurs after a record with nested tables is collapsed.
        /// </summary>
        [Description("Occurs after a record with nested tables is collapsed.")]
        [Category("Table")]
        public event RecordEventHandler RecordCollapsed;

        /// <summary>
        /// Raises the <see cref="RecordCollapsed"/> event.
        /// </summary>
        /// <param name="e">A <see cref="RecordEventArgs" /> that contains the event data.</param>
        protected virtual void OnRecordCollapsed(RecordEventArgs e)
        {
            if (RecordCollapsed != null)
            {
                RecordCollapsed(this, e);
            }
        }

        void ITableEventsTarget.OnRecordCollapsed(RecordEventArgs e)
        {
            if (this.tableEventsTarget != null)
            {
                tableEventsTarget.OnRecordCollapsed(e);
            }

            OnRecordCollapsed(e);
        }

        // tevent RecordExpanding RecordEventArgs

        /// <summary>
        /// Occurs before a record with nested tables is expanded.
        /// </summary>
        [Description("Occurs before a record with nested tables is expanded.")]
        [Category("Table")]
        public event RecordEventHandler RecordExpanding;

        /// <summary>
        /// Raises the <see cref="RecordExpanding"/> event.
        /// </summary>
        /// <param name="e">A <see cref="RecordEventArgs" /> that contains the event data.</param>
        protected virtual void OnRecordExpanding(RecordEventArgs e)
        {
            if (RecordExpanding != null)
            {
                RecordExpanding(this, e);
            }
        }

        void ITableEventsTarget.OnRecordExpanding(RecordEventArgs e)
        {
            if (this.tableEventsTarget != null)
            {
                tableEventsTarget.OnRecordExpanding(e);
            }

            OnRecordExpanding(e);
        }

        // tevent RecordExpanded RecordEventArgs

        /// <summary>
        /// Occurs after a record with nested tables is expanded.
        /// </summary>
        [Description("Occurs after a record with nested tables is expanded.")]
        [Category("Table")]
        public event RecordEventHandler RecordExpanded;

        /// <summary>
        /// Raises the <see cref="RecordExpanded"/> event.
        /// </summary>
        /// <param name="e">A <see cref="RecordEventArgs" /> that contains the event data.</param>
        protected virtual void OnRecordExpanded(RecordEventArgs e)
        {
            if (RecordExpanded != null)
            {
                RecordExpanded(this, e);
            }
        }

        void ITableEventsTarget.OnRecordExpanded(RecordEventArgs e)
        {
            if (this.tableEventsTarget != null)
            {
                tableEventsTarget.OnRecordExpanded(e);
            }

            OnRecordExpanded(e);
        }

        // tevent RecordDeleting RecordEventArgs

        /// <summary>
        /// Occurs before a record is deleted.
        /// </summary>
        /// <remarks>
        /// This event is raised only when the <see cref="Table"/> or <see cref="Record"/> triggers the deletion. If
        /// the underlying source list deletes the record, a <see cref="SourceListListChanged"/> event is raised instead.
        /// </remarks>
        [Description("Occurs before a record is deleted.")]
        [Category("Table")]
        public event RecordEventHandler RecordDeleting;

        /// <summary>
        /// Raises the <see cref="RecordDeleting"/> event.
        /// </summary>
        /// <param name="e">A <see cref="RecordEventArgs" /> that contains the event data.</param>
        protected virtual void OnRecordDeleting(RecordEventArgs e)
        {
            if (RecordDeleting != null)
            {
                RecordDeleting(this, e);
            }
        }

        void ITableEventsTarget.OnRecordDeleting(RecordEventArgs e)
        {
            if (this.tableEventsTarget != null)
            {
                tableEventsTarget.OnRecordDeleting(e);
            }

            OnRecordDeleting(e);
        }

        // tevent RecordDeleted RecordEventArgs

        /// <summary>
        /// Occurs after a record is deleted.
        /// </summary>
        /// <remarks>
        /// This event is raised only when the <see cref="Table"/> or <see cref="Record"/> triggers the deletion. If
        /// the underlying source list deletes the record, a <see cref="SourceListListChanged"/> event is raised instead.
        /// </remarks>
        [Description("Occurs after a record is deleted.")]
        [Category("Table")]
        public event RecordEventHandler RecordDeleted;

        /// <summary>
        /// Raises the <see cref="RecordDeleted"/> event.
        /// </summary>
        /// <param name="e">A <see cref="RecordEventArgs" /> that contains the event data.</param>
        protected virtual void OnRecordDeleted(RecordEventArgs e)
        {
            if (RecordDeleted != null)
            {
                RecordDeleted(this, e);
            }
        }

        void ITableEventsTarget.OnRecordDeleted(RecordEventArgs e)
        {
            if (this.tableEventsTarget != null)
            {
                tableEventsTarget.OnRecordDeleted(e);
            }

            OnRecordDeleted(e);
        }

        // tevent CurrentRecordContextChange CurrentRecordContextChangeEventArgs

        /// <summary>
        /// Occurs before and after the status of the current record was changed. Check the <see cref="CurrentRecordContextChangeEventArgs.Action"/>
        /// of the <see cref="CurrentRecordContextChangeEventArgs"/> to get information which current record state was changed.
        /// </summary>
        [Description("Occurs before and after the status of the current record was changed.")]
        [Category("Table")]
        public event CurrentRecordContextChangeEventHandler CurrentRecordContextChange;

        /// <summary>
        /// Raises the <see cref="CurrentRecordContextChange"/> event.
        /// </summary>
        /// <param name="e">A <see cref="CurrentRecordContextChangeEventArgs" /> that contains the event data.</param>
        protected virtual void OnCurrentRecordContextChange(CurrentRecordContextChangeEventArgs e)
        {
            if (CurrentRecordContextChange != null)
            {
                CurrentRecordContextChange(this, e);
            }
        }

        void ITableEventsTarget.OnCurrentRecordContextChange(CurrentRecordContextChangeEventArgs e)
        {
            if (this.tableEventsTarget != null)
            {
                tableEventsTarget.OnCurrentRecordContextChange(e);
            }

            OnCurrentRecordContextChange(e);
        }

        // tevent CurrentRecordManagerReset tableEventsTarget

        /// <summary>
        /// Occurs when the <see cref="CurrentRecordManager.Reset"/> method of the <see cref="CurrentRecordManager"/> was called.
        /// </summary>
        /// <remarks>
        /// The GridGroupingControl listens to this event and resets any "Current Cell" state when this
        /// event was raised.
        /// </remarks>
        [Description("Occurs when the CurrentRecordManager.Reset method of the CurrentRecordManager was called.")]
        [Category("Table")]
        public event TableEventHandler CurrentRecordManagerReset;

        /// <summary>
        /// Raises the <see cref="CurrentRecordManagerReset"/> event.
        /// </summary>
        /// <param name="e">A <see cref="TableEventArgs" /> that contains the event data.</param>
        protected virtual void OnCurrentRecordManagerReset(TableEventArgs e)
        {
            if (CurrentRecordManagerReset != null)
            {
                CurrentRecordManagerReset(this, e);
            }
        }

        void ITableEventsTarget.OnCurrentRecordManagerReset(TableEventArgs e)
        {
            if (this.tableEventsTarget != null)
            {
                tableEventsTarget.OnCurrentRecordManagerReset(e);
            }

            OnCurrentRecordManagerReset(e);
        }

        // tevent GroupSummaryInvalidated GroupEventArgs

        /// <summary>
        /// Occurs when a summary has been marked dirty.
        /// </summary>
        /// <remarks>
        /// The GridGroupingControl listens to this event and forces a repaint of the specified summary if it is visible
        /// when this event was raised.
        /// </remarks>
        [Description("Occurs when a summary has been marked dirty. ")]
        [Category("Table")]
        public event GroupEventHandler GroupSummaryInvalidated;

        /// <summary>
        /// Raises the <see cref="GroupSummaryInvalidated"/> event.
        /// </summary>
        /// <param name="e">A <see cref="GroupEventArgs" /> that contains the event data.</param>
        protected virtual void OnGroupSummaryInvalidated(GroupEventArgs e)
        {
            if (GroupSummaryInvalidated != null)
            {
                GroupSummaryInvalidated(this, e);
            }
        }

        void ITableEventsTarget.OnGroupSummaryInvalidated(GroupEventArgs e)
        {
            if (this.tableEventsTarget != null)
            {
                tableEventsTarget.OnGroupSummaryInvalidated(e);
            }

            OnGroupSummaryInvalidated(e);
        }

        // tevent SourceListListChanged TableListChangedEventArgs

        /// <summary>
        /// Occurs before the <see cref="Table"/> processes the <see cref="IBindingList.ListChanged"/> event
        /// of an attached source list. More detailed <see cref="SourceListRecordChanged"/> events will be
        /// raised after this event.
        /// </summary>
        /// <remarks>
        /// The reason for firing this event is to give a programmer the chance to react to a <see cref="IBindingList.ListChanged"/>
        /// event before the engine since there is otherwise no order guaranteed when an IBindingList raises a ListChanged
        /// event.
        /// </remarks>
        [Description("Occurs before the table processes the IBindingList.ListChanged event.")]
        [Category("Table")]
        public event TableListChangedEventHandler SourceListListChanged;

        /// <summary>
        /// Raises the <see cref="SourceListListChanged"/> event.
        /// </summary>
        /// <param name="e">A <see cref="TableListChangedEventArgs" /> that contains the event data.</param>
        protected virtual void OnSourceListListChanged(TableListChangedEventArgs e)
        {
            if (SourceListListChanged != null)
            {
                SourceListListChanged(this, e);
            }
        }

        void ITableEventsTarget.OnSourceListListChanged(TableListChangedEventArgs e)
        {
            if (this.tableEventsTarget != null)
            {
                tableEventsTarget.OnSourceListListChanged(e);
            }

            OnSourceListListChanged(e);
        }

        // tevent SourceListListChangedCompleted TableListChangedEventArgs

        /// <summary>
        /// Occurs after the <see cref="Table"/> processes the <see cref="IBindingList.ListChanged"/> event
        /// of an attached source list.
        /// </summary>
        /// <remarks>
        /// The reason for firing this event is to give a programmer the chance to react to a <see cref="IBindingList.ListChanged"/>
        /// event right after the engine since there is otherwise no order guaranteed when an IBindingList raises a ListChanged
        /// event.
        /// </remarks>
        [Description("Occurs after the Table processes the IBindingList.ListChanged event.")]
        [Category("Table")]
        public event TableListChangedEventHandler SourceListListChangedCompleted;

        /// <summary>
        /// Raises the <see cref="SourceListListChangedCompleted"/> event.
        /// </summary>
        /// <param name="e">A <see cref="TableListChangedEventArgs" /> that contains the event data.</param>
        protected virtual void OnSourceListListChangedCompleted(TableListChangedEventArgs e)
        {
            if (SourceListListChangedCompleted != null)
            {
                SourceListListChangedCompleted(this, e);
            }
        }

        void ITableEventsTarget.OnSourceListListChangedCompleted(TableListChangedEventArgs e)
        {
            if (this.tableEventsTarget != null)
            {
                tableEventsTarget.OnSourceListListChangedCompleted(e);
            }

            OnSourceListListChangedCompleted(e);
        }

        // tevent SourceListRecordChanged RecordChangedEventArgs

        /// <summary>
        /// Occurs when a record in the underlying data source is added, removed, or changed and after
        /// the <see cref="Table"/> is updated with that change.
        /// </summary>
        [Description("Occurs when a record in the underlying data source is added, removed, or changed and the table is updated.")]
        [Category("Table")]
        public event RecordChangedEventHandler SourceListRecordChanged;

        /// <summary>
        /// Raises the <see cref="SourceListRecordChanged"/> event.
        /// </summary>
        /// <param name="e">A <see cref="RecordChangedEventArgs" /> that contains the event data.</param>
        protected virtual void OnSourceListRecordChanged(RecordChangedEventArgs e)
        {
            if (SourceListRecordChanged != null)
            {
                SourceListRecordChanged(this, e);
            }
        }

        void ITableEventsTarget.OnSourceListRecordChanged(RecordChangedEventArgs e)
        {
            if (this.tableEventsTarget != null)
            {
                tableEventsTarget.OnSourceListRecordChanged(e);
            }

            OnSourceListRecordChanged(e);
        }

        // tevent SourceListRecordChanging RecordChangedEventArgs

        /// <summary>
        /// Occurs when a record in the underlying data source is added, removed, or changed and before
        /// the <see cref="Table"/> is updated with that change.
        /// </summary>
        [Description("Occurs when a record in the underlying data source is added, removed, or changed and before the table is updated.")]
        [Category("Table")]
        public event RecordChangedEventHandler SourceListRecordChanging;

        /// <summary>
        /// Raises the <see cref="SourceListRecordChanging"/> event.
        /// </summary>
        /// <param name="e">A <see cref="RecordChangedEventArgs" /> that contains the event data.</param>
        protected virtual void OnSourceListRecordChanging(RecordChangedEventArgs e)
        {
            if (SourceListRecordChanging != null)
            {
                SourceListRecordChanging(this, e);
            }
        }

        void ITableEventsTarget.OnSourceListRecordChanging(RecordChangedEventArgs e)
        {
            if (this.tableEventsTarget != null)
            {
                tableEventsTarget.OnSourceListRecordChanging(e);
            }

            OnSourceListRecordChanging(e);
        }

        // tevent GroupAdded GroupEventArgs

        /// <summary>
        /// Occurs when a new group is added in a table after the table is categorized and when a record is changed. The event does not
        /// occur during categorization of the table. See the <see cref="CategorizedRecords"/> elements to when categorization
        /// finishes.
        /// </summary>
        [Description("Occurs when a new group is added in a table after the table is categorized and when a record is changed.")]
        [Category("Table")]
        public event GroupEventHandler GroupAdded;

        /// <summary>
        /// Raises the <see cref="GroupAdded"/> event.
        /// </summary>
        /// <param name="e">A <see cref="GroupEventArgs" /> that contains the event data.</param>
        protected virtual void OnGroupAdded(GroupEventArgs e)
        {
            if (GroupAdded != null)
            {
                GroupAdded(this, e);
            }
        }

        void ITableEventsTarget.OnGroupAdded(GroupEventArgs e)
        {
            if (this.tableEventsTarget != null)
            {
                tableEventsTarget.OnGroupAdded(e);
            }

            OnGroupAdded(e);
        }

        // tevent GroupRemoving GroupEventArgs

        /// <summary>
        /// Occurs when a group is removed from a table after the table is categorized and when a record is changed. The event does not
        /// occur during categorization of the table. See the <see cref="CategorizedRecords"/> elements to when categorization
        /// finishes.
        /// </summary>
        [Description("Occurs when a group is removed from a table after the table is categorized and when a record is changed.")]
        [Category("Table")]
        public event GroupEventHandler GroupRemoving;

        /// <summary>
        /// Raises the <see cref="GroupRemoving"/> event.
        /// </summary>
        /// <param name="e">A <see cref="GroupEventArgs" /> that contains the event data.</param>
        protected virtual void OnGroupRemoving(GroupEventArgs e)
        {
            if (GroupRemoving != null)
            {
                GroupRemoving(this, e);
            }
        }

        void ITableEventsTarget.OnGroupRemoving(GroupEventArgs e)
        {
            if (this.tableEventsTarget != null)
            {
                tableEventsTarget.OnGroupRemoving(e);
            }

            OnGroupRemoving(e);
        }

        // tevent SortingItemsInGroup GroupEventArgs

        /// <summary>
        /// Occurs before the records for a group are sorted.
        /// </summary>
        /// <remarks>
        /// The engine has a built-in optimization for sorting columns that allows it to perform the sorting
        /// on an on-demand basis group-by-group. Suppose you have a table with 200 different countries and
        /// you change the sort order of city. It is not necessary to sort the whole table. Instead
        /// the individual groups can be sorted when they are scrolled into view. SortingItemsInGroup and
        /// SortedItemsInGroup events are fired in such cases when a specific group is sorted on demand.
        /// <para/>
        /// If the whole table was set dirty (see <see cref="Syncfusion.Grouping.Table.TableDirty"/>), then the whole table
        /// is simply recategorized. In that case only a CategorizedElements event is raised but no
        /// SortingItemsInGroup event.
        /// </remarks>
        [Description("Occurs before the records for a group are sorted.")]
        [Category("Table")]
        public event GroupEventHandler SortingItemsInGroup;

        /// <summary>
        /// Raises the <see cref="SortingItemsInGroup"/> event.
        /// </summary>
        /// <param name="e">A <see cref="GroupEventArgs" /> that contains the event data.</param>
        protected virtual void OnSortingItemsInGroup(GroupEventArgs e)
        {
            if (SortingItemsInGroup != null)
            {
                SortingItemsInGroup(this, e);
            }
        }

        void ITableEventsTarget.OnSortingItemsInGroup(GroupEventArgs e)
        {
            if (this.tableEventsTarget != null)
            {
                tableEventsTarget.OnSortingItemsInGroup(e);
            }

            OnSortingItemsInGroup(e);
        }

        // tevent SortedItemsInGroup GroupEventArgs

        /// <summary>
        /// Occurs after the records for a group are sorted.
        /// </summary>
        /// <remarks>
        /// The engine has a built-in optimization for sorting columns that allows it to perform the sorting
        /// on an on-demand basis group-by-group. Suppose you have a table with 200 different countries and
        /// you change the sort order of city. It is not necessary to sort the whole table. Instead
        /// the individual groups can be sorted when they are scrolled into view. SortingItemsInGroup and
        /// SortedItemsInGroup  events are fired in such case when a specific group was sorted on demand.
        /// <para/>
        /// If the whole table was set dirty (see <see cref="Syncfusion.Grouping.Table.TableDirty"/>), then the whole table
        /// is simply recategorized. In that case only a CategorizedElements event is raised but no
        /// SortedItemsInGroup event.
        /// </remarks>
        [Category("Table")]
        [Description("Occurs after the records for a group are sorted.")]
        public event GroupEventHandler SortedItemsInGroup;

        /// <summary>
        /// Raises the <see cref="SortedItemsInGroup"/> event.
        /// </summary>
        /// <param name="e">A <see cref="GroupEventArgs" /> that contains the event data.</param>
        protected virtual void OnSortedItemsInGroup(GroupEventArgs e)
        {
            if (SortedItemsInGroup != null)
            {
                SortedItemsInGroup(this, e);
            }
        }

        void ITableEventsTarget.OnSortedItemsInGroup(GroupEventArgs e)
        {
            if (this.tableEventsTarget != null)
            {
                tableEventsTarget.OnSortedItemsInGroup(e);
            }

            OnSortedItemsInGroup(e);
        }

        // tevent InvalidatingCounters tableEventsTarget

        /// <summary>
        /// Occurs when the <see cref="Syncfusion.Grouping.Table.InvalidateCounterTopDown"/> of a <see cref="Table"/> is called
        /// and before all counters are marked dirty.
        /// </summary>
        [Category("Table")]
        [Description("Occurs when the Table.InvalidateCounterTopDown method of a Table is called.")]
        public event TableEventHandler InvalidatingCounters;

        /// <summary>
        /// Raises the <see cref="InvalidatingCounters"/> event.
        /// </summary>
        /// <param name="e">A <see cref="TableEventArgs" /> that contains the event data.</param>
        protected virtual void OnInvalidatingCounters(TableEventArgs e)
        {
            if (InvalidatingCounters != null)
            {
                InvalidatingCounters(this, e);
            }
        }

        void ITableEventsTarget.OnInvalidatingCounters(TableEventArgs e)
        {
            if (this.tableEventsTarget != null)
            {
                tableEventsTarget.OnInvalidatingCounters(e);
            }

            OnInvalidatingCounters(e);
        }

        // tevent InvalidatingSummaries tableEventsTarget

        /// <summary>
        /// Occurs when the <see cref="Syncfusion.Grouping.Table.InvalidateSummariesTopDown"/> of a <see cref="Table"/> is called
        /// and before all summaries in that table are marked dirty.
        /// </summary>
        [Category("Table")]
        [Description("Occurs when the Table.InvalidateSummariesTopDown of a table is called.")]
        public event TableEventHandler InvalidatingSummaries;

        /// <summary>
        /// Raises the <see cref="InvalidatingSummaries"/> event.
        /// </summary>
        /// <param name="e">A <see cref="TableEventArgs" /> that contains the event data.</param>
        protected virtual void OnInvalidatingSummaries(TableEventArgs e)
        {
            if (InvalidatingSummaries != null)
            {
                InvalidatingSummaries(this, e);
            }
        }

        void ITableEventsTarget.OnInvalidatingSummaries(TableEventArgs e)
        {
            if (this.tableEventsTarget != null)
            {
                tableEventsTarget.OnInvalidatingSummaries(e);
            }

            OnInvalidatingSummaries(e);
        }

        // tevent CategorizingRecords tableEventsTarget

        /// <summary>
        /// Occurs before records are categorized after a table is marked dirty (<see cref="Syncfusion.Grouping.Table.TableDirty"/>).
        /// </summary>
        /// <remarks>
        /// When <see cref="Syncfusion.Grouping.Table.TableDirty"/> is set to True, e.g. because schema information for a table was changed
        /// or because the grouped columns were changed, the table will categorize records on demand the first time
        /// information about a record is accessed. At that time, the <see cref="Syncfusion.Grouping.Element.EnsureInitialized"/> of the <see cref="Table"/> will start
        /// categorization.
        /// </remarks>
        [Category("Table")]
        [Description("Occurs before records are categorized after a table is marked dirty")]
        public event TableEventHandler CategorizingRecords;

        /// <summary>
        /// Raises the <see cref="CategorizingRecords"/> event.
        /// </summary>
        /// <param name="e">A <see cref="TableEventArgs" /> that contains the event data.</param>
        protected virtual void OnCategorizingRecords(TableEventArgs e)
        {
            if (CategorizingRecords != null)
            {
                CategorizingRecords(this, e);
            }
        }

        void ITableEventsTarget.OnCategorizingRecords(TableEventArgs e)
        {
            if (this.tableEventsTarget != null)
            {
                tableEventsTarget.OnCategorizingRecords(e);
            }

            OnCategorizingRecords(e);
        }

        // tevent CategorizedRecords tableEventsTarget

        /// <summary>
        /// Occurs after records are categorized when a table is marked dirty (<see cref="Syncfusion.Grouping.Table.TableDirty"/>).
        /// </summary>
        /// <remarks>
        /// When <see cref="Syncfusion.Grouping.Table.TableDirty"/> is set True, e.g. because schema information for a table was changed
        /// or because the grouped columns were changed, the table will categorize records on demand the first time
        /// information about a record is accessed. At that time, the <see cref="Element.EnsureInitialized"/> of the <see cref="Table"/>
        /// will start
        /// categorization.
        /// </remarks>
        [Category("Table")]
        [Description("Occurs after records are categorized when a table is marked dirty.")]
        public event TableEventHandler CategorizedRecords;

        /// <summary>
        /// Raises the <see cref="CategorizedRecords"/> event.
        /// </summary>
        /// <param name="e">A <see cref="TableEventArgs" /> that contains the event data.</param>
        protected virtual void OnCategorizedRecords(TableEventArgs e)
        {
            if (CategorizedRecords != null)
            {
                CategorizedRecords(this, e);
            }
        }

        void ITableEventsTarget.OnCategorizedRecords(TableEventArgs e)
        {
            if (this.tableEventsTarget != null)
            {
                tableEventsTarget.OnCategorizedRecords(e);
            }

            OnCategorizedRecords(e);
        }

        // tevent TableSourceListChanged Table

        /// <summary>
        /// Occurs after the datasource is replaced.
        /// </summary>
        [Category("Table")]
        [Description("Occurs after the datasource is replaced.")]
        public event TableEventHandler TableSourceListChanged;

        /// <summary>
        /// Raises the <see cref="SourceListChanged"/> event.
        /// </summary>
        /// <param name="e">A <see cref="TableEventArgs" /> that contains the event data.</param>
        protected virtual void OnTableSourceListChanged(TableEventArgs e)
        {
            if (TableSourceListChanged != null)
            {
                TableSourceListChanged(this, e);
            }
        }

        void ITableEventsTarget.OnTableSourceListChanged(TableEventArgs e)
        {
            if (this.tableEventsTarget != null)
            {
                tableEventsTarget.OnTableSourceListChanged(e);
            }

            OnTableSourceListChanged(e);
        }

        // tevent RecordValueChanging RecordValueChanging

        /// <summary>
        /// Occurs when a RecordFieldCell cell's value is changed and before Record.SetValue is called.
        /// </summary>
        [Category("Table")]
        [Description("Occurs when a RecordFieldCell cell's value is changed and before Record.SetValue is called.")]
        public event RecordValueChangingEventHandler RecordValueChanging;

        /// <summary>
        /// Raises the <see cref="RecordValueChanging"/> event.
        /// </summary>
        /// <param name="e">A <see cref="RecordValueChangingEventArgs" /> that contains the event data.</param>
        protected virtual void OnRecordValueChanging(RecordValueChangingEventArgs e)
        {
            if (RecordValueChanging != null)
            {
                RecordValueChanging(this, e);
            }
        }

        void ITableEventsTarget.OnRecordValueChanging(RecordValueChangingEventArgs e)
        {
            if (this.tableEventsTarget != null)
            {
                tableEventsTarget.OnRecordValueChanging(e);
            }

            OnRecordValueChanging(e);
        }

        // tevent RecordValueChanged RecordValueChanged

        /// <summary>
        /// Occurs when a RecordFieldCell cell's value is changed and after Record.SetValue is returned.
        /// </summary>
        public event RecordValueChangedEventHandler RecordValueChanged;

        /// <summary>
        /// Raises the <see cref="RecordValueChanged"/> event.
        /// </summary>
        /// <param name="e">A <see cref="RecordValueChangedEventArgs" /> that contains the event data.</param>
        protected virtual void OnRecordValueChanged(RecordValueChangedEventArgs e)
        {
            if (RecordValueChanged != null)
            {
                RecordValueChanged(this, e);
            }
        }

        void ITableEventsTarget.OnRecordValueChanged(RecordValueChangedEventArgs e)
        {
            if (this.tableEventsTarget != null)
            {
                tableEventsTarget.OnRecordValueChanged(e);
            }

            OnRecordValueChanged(e);
        }

        // tevent DisplayElementChanging DisplayElementChanging

        /// <summary>
        /// When number of visible elements are changed.
        /// </summary>
        public event DisplayElementChangingEventHandler DisplayElementChanging;

        /// <summary>
        /// Raises the <see cref="DisplayElementChanging"/> event.
        /// </summary>
        /// <param name="e">A <see cref="DisplayElementChangingEventArgs" /> that contains the event data.</param>
        protected virtual void OnDisplayElementChanging(DisplayElementChangingEventArgs e)
        {
            if (DisplayElementChanging != null)
            {
                DisplayElementChanging(this, e);
            }
        }

        void ITableEventsTarget.OnDisplayElementChanging(DisplayElementChangingEventArgs e)
        {
            if (this.tableEventsTarget != null)
            {
                tableEventsTarget.OnDisplayElementChanging(e);
            }

            OnDisplayElementChanging(e);
        }

        // tevent DisplayElementChanged DisplayElementChanged

        /// <summary>
        /// When number of visible elements are changed.
        /// </summary>
        public event DisplayElementChangedEventHandler DisplayElementChanged;

        /// <summary>
        /// Raises the <see cref="DisplayElementChanged"/> event.
        /// </summary>
        /// <param name="e">A <see cref="DisplayElementChangedEventArgs" /> that contains the event data.</param>
        protected virtual void OnDisplayElementChanged(DisplayElementChangedEventArgs e)
        {
            if (DisplayElementChanged != null)
            {
                DisplayElementChanged(this, e);
            }
        }

        void ITableEventsTarget.OnDisplayElementChanged(DisplayElementChangedEventArgs e)
        {
            if (this.tableEventsTarget != null)
            {
                tableEventsTarget.OnDisplayElementChanged(e);
            }

            OnDisplayElementChanged(e);
        }

        // tevent SelectedRecordsChanging SelectedRecordsChanging

        /// <summary>
        /// Occurs after the <see cref="Syncfusion.Grouping.Table.SelectedRecords"/> collection was modified.
        /// </summary>
        public event SelectedRecordsChangedEventHandler SelectedRecordsChanging;

        /// <summary>
        /// Raises the <see cref="SelectedRecordsChanging"/> event.
        /// </summary>
        /// <param name="e">A <see cref="SelectedRecordsChangedEventArgs" /> that contains the event data.</param>
        protected virtual void OnSelectedRecordsChanging(SelectedRecordsChangedEventArgs e)
        {
            if (SelectedRecordsChanging != null)
            {
                SelectedRecordsChanging(this, e);
            }
        }

        void ITableEventsTarget.OnSelectedRecordsChanging(SelectedRecordsChangedEventArgs e)
        {
            if (this.tableEventsTarget != null)
            {
                tableEventsTarget.OnSelectedRecordsChanging(e);
            }

            OnSelectedRecordsChanging(e);
        }

        // tevent SelectedRecordsChanged SelectedRecordsChanged

        /// <summary>
        /// Occurs after the <see cref="Syncfusion.Grouping.Table.SelectedRecords"/> collection was modified.
        /// </summary>
        public event SelectedRecordsChangedEventHandler SelectedRecordsChanged;

        /// <summary>
        /// Raises the <see cref="SelectedRecordsChanged"/> event.
        /// </summary>
        /// <param name="e">A <see cref="SelectedRecordsChangedEventArgs" /> that contains the event data.</param>
        protected virtual void OnSelectedRecordsChanged(SelectedRecordsChangedEventArgs e)
        {
            if (SelectedRecordsChanged != null)
            {
                SelectedRecordsChanged(this, e);
            }
        }

        void ITableEventsTarget.OnSelectedRecordsChanged(SelectedRecordsChangedEventArgs e)
        {
            if (this.tableEventsTarget != null)
            {
                tableEventsTarget.OnSelectedRecordsChanged(e);
            }

            OnSelectedRecordsChanged(e);
        }

        bool allowSwapDataViewWithDataTableList = false;

        /// <summary>
        /// Gets / sets if the engine can wrap access to a DataTable with a <see cref="DataTableList"/> which provides
        /// optimized access to the rows of the DataTable. <para/>
        /// The Engine will access a DataTable through this wrapper class instead of accessing records through the DataTable.DefaultView
        /// to increase performance when adding, removing and changing records when <see cref="Syncfusion.Grouping.Engine.AllowSwapDataViewWithDataTableList"/>
        /// is enabled. Default is False.
        /// </summary>
        [DefaultValue(false)]
        [Browsable(false)]
        [Category("Optimization")]
        [Description("Specifies if engine can wrap access to a DataTable with a DataTableList which provides optimized access to the rows of the DataTable.")]
        public bool AllowSwapDataViewWithDataTableList
        {
            get
            {
                return allowSwapDataViewWithDataTableList;
            }

            set
            {
                if (this.allowSwapDataViewWithDataTableList != value)
                {
                    this.OnPropertyChanging(new DescriptorPropertyChangedEventArgs("AllowSwapDataViewWithDataTableList"));
                    this.allowSwapDataViewWithDataTableList = value;
                    if (!TableDescriptor.Fields.IsModified)
                    {
                        TableDescriptor.Fields.Reset();
                    }
                    // TODO: should replace sourcelist here ...
                    this.OnPropertyChanged(new DescriptorPropertyChangedEventArgs("AllowSwapDataViewWithDataTableList"));
                }
            }
        }

        #region FieldValue events
        // event FieldValueEventHandler QueryValue

        /// <summary>
        /// Occurs when a value for a field descriptor and record is returned. See the Grid\Grouping\Samples\CustomSummary
        /// sample how to use this event with unbound field descriptors.
        /// </summary>
        public event FieldValueEventHandler QueryValue;

        /// <summary>
        /// Raises the <see cref="QueryValue"/> event
        /// </summary>
        /// <param name="e">The <see cref="FieldValueEventArgs"/> with event data.</param>
        protected virtual void OnQueryValue(FieldValueEventArgs e)
        {
            if (QueryValue != null)
            {
                QueryValue(this, e);
            }
        }

        /// <summary>
        /// Raises the <see cref="QueryValue"/> event.
        /// </summary>
        /// <param name="e">The <see cref="FieldValueEventArgs"/> with event data.</param>
        public void RaiseQueryValue(FieldValueEventArgs e)
        {
            OnQueryValue(e);
        }

        // event FieldValueEventHandler SaveValue

        /// <summary>
        /// Occurs when values for a field descriptor and record are saved. See the Grid\Grouping\Samples\CustomSummary
        /// sample how to use this event with unbound field descriptors.
        /// </summary>
        public event FieldValueEventHandler SaveValue;

        /// <summary>
        /// Raises the <see cref="SaveValue"/> event.
        /// </summary>
        /// <param name="e">The <see cref="FieldValueEventArgs"/> with event data.</param>
        protected virtual void OnSaveValue(FieldValueEventArgs e)
        {
            if (SaveValue != null)
            {
                SaveValue(this, e);
            }
        }

        /// <summary>
        /// Raises the <see cref="SaveValue"/> event.
        /// </summary>
        /// <param name="e">The <see cref="FieldValueEventArgs"/> with event data.</param>
        public void RaiseSaveValue(FieldValueEventArgs e)
        {
            OnSaveValue(e);
        }
        #endregion

        bool showNestedProperties = true;

        /// <summary>
        /// This property affects the autopopulation of the FieldDescriptorCollection. <para/>
        /// It specifies if individual fields should be added for every property of a type
        /// when a type has nested properties. You can also control this behavior at run-time
        /// with the QueryShowNestedPropertiesFields event. Default is True.
        /// </summary>
        [DefaultValue(true)]
        [Description("Specifies if individual fields should be added for every property of a type when a type has nested properties.")]
        [Category("Grouping Control")]
        public bool ShowNestedPropertiesFields
        {
            get
            {
                return this.showNestedProperties;
            }

            set
            {
                if (this.showNestedProperties != value)
                {
                    this.OnPropertyChanging(new DescriptorPropertyChangedEventArgs("ShowNestedPropertiesFields"));
                    this.showNestedProperties = value;
                    if (!TableDescriptor.Fields.IsModified)
                    {
                        TableDescriptor.Fields.Reset();
                    }

                    this.OnPropertyChanged(new DescriptorPropertyChangedEventArgs("ShowNestedPropertiesFields"));
                }
            }
        }

        // event QueryShowNestedPropertiesFields QueryShowNestedPropertiesFields

        /// <summary>
        /// This event affects the autopopulation of the FieldDescriptorCollection. <para/>
        /// It lets you control at run-time if individual fields should be added for every property of a type
        /// when a type has nested properties. You can set e.Cancel = true to avoid nested fields
        /// being generated for a specific type.
        /// </summary>
        public event QueryShowNestedPropertiesFieldsEventHandler QueryShowNestedPropertiesFields;

        /// <summary>
        /// Raises the <see cref="QueryShowNestedPropertiesFields"/> event.
        /// </summary>
        /// <param name="e">A <see cref="QueryShowNestedPropertiesFieldsEventArgs" /> that contains the event data.</param>
        protected virtual void OnQueryShowNestedPropertiesFields(QueryShowNestedPropertiesFieldsEventArgs e)
        {
            if (QueryShowNestedPropertiesFields != null)
            {
                QueryShowNestedPropertiesFields(this, e);
            }
        }

        /// <summary>
        /// Raises the <see cref="QueryShowNestedPropertiesFields"/> event.
        /// </summary>
        /// <param name="e">A <see cref="QueryShowNestedPropertiesFieldsEventArgs" /> that contains the event data.</param>
        public void RaiseQueryShowNestedPropertiesFields(QueryShowNestedPropertiesFieldsEventArgs e)
        {
            OnQueryShowNestedPropertiesFields(e);
        }

        // event QueryShowField QueryShowField

        /// <summary>
        /// This event affects the autopopulation of the FieldDescriptorCollection. <para/>
        /// It is called for each field and lets you control at run-time if a specific field should be added
        /// to the FieldDescriptorCollection. You can set e.Cancel = True to avoid specific fields
        /// being added.
        /// </summary>
        public event QueryShowFieldEventHandler QueryShowField;

        /// <summary>
        /// Raises the <see cref="QueryShowField"/> event.
        /// </summary>
        /// <param name="e">A <see cref="QueryShowFieldEventArgs" /> that contains the event data.</param>
        protected virtual void OnQueryShowField(QueryShowFieldEventArgs e)
        {
            if (QueryShowField != null)
            {
                QueryShowField(this, e);
            }
        }

        /// <summary>
        /// Raises the <see cref="QueryShowField"/> event.
        /// </summary>
        /// <param name="e">A <see cref="QueryShowFieldEventArgs" /> that contains the event data.</param>
        public void RaiseQueryShowField(QueryShowFieldEventArgs e)
        {
            OnQueryShowField(e);
        }

        ShowRelationFields showRelationFields = ShowRelationFields.ShowDisplayFieldsOnly;

        /// <summary>
        /// This property affects the autopopulation of the FieldDescriptorCollection. <para/>
        /// It specifies if dependent fields from a related table in a foreign key relation (or related collection)
        /// should be added to the
        /// main tables FieldDescriptorCollection. You can also control this behavior at run-time
        /// with the QueryShowRelationDisplayFields event. Default is ShowRelationFields.ShowDisplayFieldsOnly.
        /// </summary>
        [DefaultValue(ShowRelationFields.ShowDisplayFieldsOnly)]
        [Description("Specifies if dependent fields from a related table in a foreign key relation (or related collection) should be added to the main tables FieldDescriptorCollection.")]
        [Category("Grouping Control")]
        public ShowRelationFields ShowRelationFields
        {
            get
            {
                return this.showRelationFields;
            }

            set
            {
                if (this.showRelationFields != value)
                {
                    this.OnPropertyChanging(new DescriptorPropertyChangedEventArgs("ShowRelationFields"));
                    this.showRelationFields = value;
                    if (!TableDescriptor.Fields.IsModified)
                    {
                        TableDescriptor.Fields.Reset();
                    }

                    this.OnPropertyChanged(new DescriptorPropertyChangedEventArgs("ShowRelationFields"));
                }
            }
        }

        // event RelationDescriptor QueryShowRelationDisplayFields

        /// <summary>
        /// This event affects the autopopulation of the FieldDescriptorCollection. <para/>
        /// It is called for each foreign key relation and lets you control at run-time if the related fields of the
        /// child table should be added to the FieldDescriptorCollection. You can set e.Cancel = True to avoid specific fields
        /// being added.
        /// </summary>
        public event QueryShowRelationFieldsEventHandler QueryShowRelationDisplayFields;

        /// <summary>
        /// Raises the <see cref="QueryShowRelationDisplayFields"/> event.
        /// </summary>
        /// <param name="e">A <see cref="QueryShowRelationFieldsEventArgs" /> that contains the event data.</param>
        protected virtual void OnQueryShowRelationDisplayFields(QueryShowRelationFieldsEventArgs e)
        {
            if (QueryShowRelationDisplayFields != null)
            {
                QueryShowRelationDisplayFields(this, e);
            }
        }

        /// <summary>
        /// Raises the <see cref="QueryShowRelationDisplayFields"/> event.
        /// </summary>
        /// <param name="e">A <see cref="QueryShowRelationFieldsEventArgs" /> that contains the event data.</param>
        public void RaiseQueryShowRelationDisplayFields(QueryShowRelationFieldsEventArgs e)
        {
            OnQueryShowRelationDisplayFields(e);
        }

        // event QueryAddRelation QueryAddRelation

        /// <summary>
        /// This event affects the autopopulation of the RelationDescriptorCollection. <para/>
        /// It is called for each relation and lets you control at run-time if the relation
        /// should be added to the RelationDescriptorCollection. You can set e.Cancel = True to avoid specific
        /// relations being added.
        /// </summary>
        public event QueryAddRelationEventHandler QueryAddRelation;

        /// <summary>
        /// Raises the <see cref="QueryAddRelation"/> event.
        /// </summary>
        /// <param name="e">A <see cref="QueryAddRelationEventArgs" /> that contains the event data.</param>
        protected virtual void OnQueryAddRelation(QueryAddRelationEventArgs e)
        {
            if (QueryAddRelation != null)
            {
                QueryAddRelation(this, e);
            }
        }

        /// <summary>
        /// Raises the <see cref="QueryAddRelation"/> event.
        /// </summary>
        /// <param name="e">A <see cref="QueryAddRelationEventArgs" /> that contains the event data.</param>
        public void RaiseQueryAddRelation(QueryAddRelationEventArgs e)
        {
            OnQueryAddRelation(e);
        }
        
        bool autoPopulateRelations = true;

        /// <summary>
        /// This property affects the autopopulation of the RelationDescriptorCollection. <para/>
        /// It specifies if relations should be automatically generated when you assign a
        /// DataSource a DataTable with constraints or a DataSet with relations defined.
        /// Default is True.
        /// </summary>
        [DefaultValue(true)]
        [Description("Specifies if relations should be automatically generated when a DataTable with constraints or a DataSet with relations is assigned as DataSource.")]
        [Category("Grouping Control")]
        public bool AutoPopulateRelations
        {
            get
            {
                return this.autoPopulateRelations;
            }

            set
            {
                if (this.autoPopulateRelations != value)
                {
                    this.OnPropertyChanging(new DescriptorPropertyChangedEventArgs("AutoPopulateRelations"));
                    this.autoPopulateRelations = value;
                    if (!TableDescriptor.Relations.IsModified)
                    {
                        TableDescriptor.Relations.Reset();
                    }

                    this.OnPropertyChanged(new DescriptorPropertyChangedEventArgs("AutoPopulateRelations"));
                }
            }
        }

        /// <summary>
        /// For internal use.
        /// </summary>
        /// <returns>returns EngineTableEnumerable</returns>
        /// <internalonly/>
        [Syncfusion.Documentation.DocumentationExclude()]
        public EngineTableEnumerable EnumerateTables()
        {
            return new EngineTableEnumerable(this);
        }

        /// <summary>
        /// For internal use.
        /// </summary>
        /// <returns>returns EngineTableDescriptorEnumerable</returns>
        /// <internalonly/>
        [Syncfusion.Documentation.DocumentationExclude()]
        public EngineTableDescriptorEnumerable EnumerateTableDescriptor()
        {
            return new EngineTableDescriptorEnumerable(this);
        }

        /// <summary>
        /// The assembly version of the GridGroupingControl at the time
        /// it was dropped onto a form with designer.
        /// </summary>
        [Browsable(false)]
        public virtual string VersionInfo
        {
            get
            {
                return string.Empty;
            }
        }

        ICounterFactory _counterFactory;

        internal ICounterFactory CounterFactory
        {
            get
            {
                if (_counterFactory == null)
                {
                    _counterFactory = new DefaultCounterFactory();
                }

                return _counterFactory;
            }

            set
            {
                if (_counterFactory != value)
                {
                    _counterFactory = value;
                    Table.TableDirty = true;
                    Table.ClearCollectionCaches();
                }
            }
        }

        EngineCounters engineCounters = EngineCounters.All;

        /// <summary>
        /// Specifies the counter logic to be used within the engine. If you have a large datasource
        /// and need support for groups and filtered records you can reduce the memory footprint
        /// by selectively disabling counters you do not need in your application. <para/>
        /// See EngineOptimizations.DisableCounters for completely disabling counter logic for the RecordsDetails collection if you do not need
        /// grouping and filtering. <para/>
        /// See EngineOptimizations.VirtualMode for using the engine in a virtual mode if you do not
        /// need support for sorting.
        /// </summary>
        [DefaultValue(EngineCounters.All)]
        [Category("Optimization")]
        [Description("Specifies the counter logic to be used within the engine.")]
        public EngineCounters CounterLogic
        {
            get
            {
                return engineCounters;
            }

            set
            {
                if (engineCounters != value)
                {
                    this.OnPropertyChanging(new DescriptorPropertyChangedEventArgs("CounterLogic"));
                    engineCounters = value;
                    switch (engineCounters)
                    {
                        case EngineCounters.FilteredRecords:
                            CounterFactory = new FilteredRecordCounterFactory();
                            break;

                        case EngineCounters.YAmount:
                            CounterFactory = new YAmountCounterFactory();
                            break;

                        case EngineCounters.All:
                            CounterFactory = new DefaultCounterFactory();
                            break;
                    }

                    this.OnPropertyChanged(new DescriptorPropertyChangedEventArgs("CounterLogic"));
                }
            }
        }

        EngineOptimizations allowedOptimizations = EngineOptimizations.None;

        /// <summary>
        /// Specifies optimizations the engine is allowed use when applicable. These optimizations can be used 
        /// in combination with EngineCounter setting.
        /// </summary>
        [Category("Optimization")]
        [Description("Specifies optimizations the engine is allowed use when applicable.")]
        [DefaultValue(EngineOptimizations.None)]
        [Editor(typeof(Syncfusion.Windows.Forms.Design.EnumFlagsEditor), typeof(System.Drawing.Design.UITypeEditor))]
        public EngineOptimizations AllowedOptimizations
        {
            get
            {
                return allowedOptimizations;
            }

            set
            {
                if (AllowedOptimizations != value)
                {
                    this.OnPropertyChanging(new DescriptorPropertyChangedEventArgs("AllowedOptimizations"));
                    allowedOptimizations = value;
                    this.OnPropertyChanging(new DescriptorPropertyChangedEventArgs("AllowedOptimizations"));
                }
            }
        }

        [Browsable(false)]
        [System.Xml.Serialization.XmlIgnore]
        [DesignerSerializationVisibilityAttribute(DesignerSerializationVisibility.Hidden)]
        public bool SupportsYAmount
        {
            get
            {
                return CounterFactory is YAmountCounterFactory || CounterFactory is DefaultCounterFactory;
            }
        }

        bool forceShowCurrentRecord = true;

        /// <summary>
        /// Gets or sets whether the engine should ensure that a record is visible and all its parent
        /// elements are expanded when setting the current record. The default setting is true.
        /// </summary>
        [DefaultValue(true)]
        public bool ForceShowCurrentRecord
        {
            get { return forceShowCurrentRecord; }
            set { forceShowCurrentRecord = value; }
        }

        bool sortMappingNames = false;

        /// <summary>
        /// Gets or sets whether the engine should sort mapping names alphabetically
        /// in the dropdown editors of the property grid. Default is false.
        /// </summary>
        [DefaultValue(false)]
        public bool SortMappingNames
        {
            get { return sortMappingNames; }
            set { sortMappingNames = value; }
        }

        private int maxNestedFieldRecurseLevel = -1;

        /// <summary>
        /// Specifies the number of levels to recurse when ShowNestedPropertiesFields is set to true.
        /// </summary>
        /// <value>Default value is <see cref="AbortNestedFieldsRecursionLevel"/>. You can reset this property by setting it's value to -1.</value>
        /// <remarks>
        /// While AbortNestedFieldsRecursionLevel lets you specify this recursion level
        /// globally, this property lets you specify it for a particular Engine instance.
        /// Take a look at AbortNestedFieldsRecursionLevel for more information.
        /// </remarks>
        [Description("Specifies the number of levels to recurse when ShowNestedPropertiesFields is set to true.")]
        public int MaxNestedFieldRecurseLevel
        {
            get
            {
                if (this.maxNestedFieldRecurseLevel == -1)
                {
                    return AbortNestedFieldsRecursionLevel;
                }
                else
                {
                    return this.maxNestedFieldRecurseLevel;
                }
            }

            set
            {
                this.maxNestedFieldRecurseLevel = value;
            }
        }

        /// <summary>
        /// Determines whether <see cref="MaxNestedFieldRecurseLevel"/> has been modified
        /// and its value should be serialized at design-time.
        /// </summary>
        /// <returns>true if contents were changed; false otherwise.</returns>
        public bool ShouldSerializeMaxNestedFieldRecurseLevel()
        {
            return this.maxNestedFieldRecurseLevel != -1;
        }

        /// <summary>
        /// Resets the <see cref="MaxNestedFieldRecurseLevel"/> property back to its default value <see cref="AbortNestedFieldsRecursionLevel"/>
        /// </summary>
        public void ResetMaxNestedFieldRecurseLevel()
        {
            this.maxNestedFieldRecurseLevel = -1;
        }

        private static int maximumNestedFieldLevel = 3;

        /// <summary>
        /// Specifies the amount of recursion allowed when stepping into nested properties of
        /// a type. Default is 3. 
        /// </summary>
        /// <remarks>
        /// The property is checked by the FieldDescriptor collection to avoid an infinite 
        /// recursion when two types reference each other as nested properties. <para/>
        /// If you have a situation where Class1 has a property of type Class2 and Class2 has
        /// a property of type Class1 it is recommended that you mark one of the properties as
        /// Browsable(false). If you do not mark it Browsable(false) then the grid will step into
        /// a recursion and abort this recursion when the level reaches the value specified for 
        /// MaximumNestedFieldLevel.
        /// </remarks>
        [System.Xml.Serialization.XmlIgnore]
        public static int AbortNestedFieldsRecursionLevel
        {
            get { return maximumNestedFieldLevel; }
            set { maximumNestedFieldLevel = value; }
        }
        
        #region Self-relations
        
        private int maxNestedCollectionRecurseLevel = -1;

        /// <summary>
        /// Specifies the number of levels to recurse when ShowNestedPropertiesCollections is set to true.
        /// </summary>
        /// <value>Default value is <see cref="AbortNestedCollectionsRecursionLevel"/>. You can reset this property by setting it's value to -1.</value>
        /// <remarks>
        /// While AbortNestedCollectionsRecursionLevel lets you specify this recursion level
        /// globally, this property lets you specify it for a particular Engine instance.
        /// Take a look at AbortNestedCollectionsRecursionLevel for more information.
        /// </remarks>
        [Description("Specifies the number of levels to recurse when ShowNestedPropertiesCollections is set to true.")]
        public int MaxNestedCollectionRecurseLevel
        {
            get
            {
                if (this.maxNestedCollectionRecurseLevel == -1)
                {
                    return AbortNestedCollectionsRecursionLevel;
                }
                else
                {
                    return this.maxNestedCollectionRecurseLevel;
                }
            }

            set
            {
                this.maxNestedCollectionRecurseLevel = value;
            }
        }

        /// <summary>
        /// Determines whether <see cref="MaxNestedCollectionRecurseLevel"/> has been modified
        /// and its value should be serialized at design-time.
        /// </summary>
        /// <returns>true if contents were changed; false otherwise.</returns>
        public bool ShouldSerializeMaxNestedCollectionRecurseLevel()
        {
            return this.maxNestedCollectionRecurseLevel != -1;
        }

        /// <summary>
        /// Resets the <see cref="MaxNestedCollectionRecurseLevel"/> property back to its default value <see cref="AbortNestedCollectionsRecursionLevel"/>
        /// </summary>
        public void ResetMaxNestedCollectionRecurseLevel()
        {
            this.maxNestedCollectionRecurseLevel = -1;
        }

        private static int maximumNestedCollectionLevel = 5;

        /// <summary>
        /// Specifies the amount of recursion allowed when stepping into nested collections of
        /// a type with self-relations. Default is 5.
        /// </summary>
        /// <remarks>
        /// The property is checked by the RelationDescriptor collection to avoid an infinite 
        /// recursion a type has a self-relations. <para/>
        /// </remarks>
        [System.Xml.Serialization.XmlIgnore]
        public static int AbortNestedCollectionsRecursionLevel
        {
            get { return maximumNestedCollectionLevel; }
            set { maximumNestedCollectionLevel = value; }
        }
        
        #endregion

        static bool allowSkipItemChangedWhileEditing = true;

        /// <summary>
        /// A BindingList in .NET 2.0 immeditaley raises a ItemChanged event after each
        /// change even if BeginEdit was called. The following flag specifies whether 
        /// to proceed with ItemChanged event handler logic only if the record was 
        /// modified outside the engine or if the CurrentRecordManager called EndEdit.
        /// Default is true.
        /// </summary>
        public static bool AllowSkipItemChangedWhileEditing
        {
            get { return allowSkipItemChangedWhileEditing; }
            set { allowSkipItemChangedWhileEditing = value; }
        }
        
        static bool allowSaveIntoEditableObjectWhileEditing = true;

        /// <summary>
        /// This flag specifies whether the engine should save a value into
        /// the current editable row of the underlying datasource when
        /// BeginEdit was called on a record and the field is modified. Saving the
        /// value into the row in the datsource has the benefit that
        /// validation can occur in the datasource immediately once the end-user
        /// moves to the next cell in the current record. <para/>
        /// Set this flag to false to save values into the current record
        /// only at the time EndEdit was called (when the user navigates away from 
        /// the current record and wants to commit changes.)
        /// Default is true.
        /// </summary>
        public static bool AllowSaveIntoEditableObjectWhileEditing
        {
            get { return allowSaveIntoEditableObjectWhileEditing; }
            set { allowSaveIntoEditableObjectWhileEditing = value; }
        }        
    }

    /// <summary>For internal use.</summary>
    /// <internalonly/>
    [Syncfusion.Documentation.DocumentationExclude()]
    public class EngineTableEnumerable : IEnumerable
    {
        Engine engine;

        /// <summary>For internal use.</summary>
        /// <internalonly/>
        public EngineTableEnumerable(Engine engine)
        {
            this.engine = engine;
        }

        #region IEnumerable Members

        /// <summary>
        /// For internal use.
        /// </summary>
        /// <returns>
        /// An <see cref="T:System.Collections.IEnumerator"/> object that can be used to iterate through the collection.
        /// </returns>
        /// <internalonly/>
        public IEnumerator GetEnumerator()
        {
            return new EngineTableEnumerator(engine);
        }
        #endregion
    }

    /// <summary>
    /// For internal use.
    /// </summary>
    /// <internalonly/>
    [Syncfusion.Documentation.DocumentationExclude()]
    public class EngineTableDescriptorEnumerable : IEnumerable
    {
        Engine engine;

        /// <summary>For internal use.</summary>
        /// <internalonly/>
        public EngineTableDescriptorEnumerable(Engine engine)
        {
            this.engine = engine;
        }

        #region IEnumerable Members

        /// <summary>
        /// For internal use.
        /// </summary>
        /// <returns>
        /// An <see cref="T:System.Collections.IEnumerator"/> object that can be used to iterate through the collection.
        /// </returns>
        /// <internalonly/>
        public IEnumerator GetEnumerator()
        {
            return new EngineTableDescriptorEnumerator(engine);
        }

        #endregion
    }

    /// <summary>For internal use.</summary>
    /// <internalonly/>
    [Syncfusion.Documentation.DocumentationExclude()]
    public class EngineTableEnumerator : IEnumerator
    {
        Engine _engine;
        ArrayList inner;
        IEnumerator innerEnumerator;

        /// <summary>For internal use.</summary>
        /// <internalonly/>
        public EngineTableEnumerator(Engine engine)
        {
            _engine = engine;
            inner = new ArrayList();
            inner.Add(engine.Table);
            AddRelatedTables(engine.Table);

            innerEnumerator = inner.GetEnumerator();
        }

        void AddRelatedTables(Table table)
        {
            foreach (Table t in table.RelatedTables)
            {
                inner.Add(t);
                AddRelatedTables(t);
            }
        }

        #region IEnumerator Members

        /// <summary>
        /// Sets the enumerator to its initial position, which is before the first element in the collection.
        /// </summary>
        public virtual void Reset()
        {
            inner = new ArrayList();
            inner.Add(_engine.Table);
            AddRelatedTables(_engine.Table);
            innerEnumerator = inner.GetEnumerator();
        }

        object IEnumerator.Current
        {
            get
            {
                return innerEnumerator.Current;
            }
        }

        /// <summary>
        /// Gets the current element in the collection.
        /// </summary>
        public Table Current
        {
            get
            {
                return (Table)innerEnumerator.Current;
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
            return innerEnumerator.MoveNext();
        }
        #endregion
    }

    /// <summary>For internal use.</summary>
    /// <internalonly/>
    [Syncfusion.Documentation.DocumentationExclude()]
    public class EngineTableDescriptorEnumerator : IEnumerator
    {
        Engine _engine;
        ArrayList inner;
        IEnumerator innerEnumerator;

        /// <summary>For internal use.</summary>
        /// <internalonly/>
        public EngineTableDescriptorEnumerator(Engine engine)
        {
            _engine = engine;
            inner = new ArrayList();
            inner.Add(engine.TableDescriptor);
            AddRelatedTables(engine.TableDescriptor);

            innerEnumerator = inner.GetEnumerator();
        }

        void AddRelatedTables(TableDescriptor table)
        {
            foreach (RelationDescriptor rd in table.Relations)
            {
                inner.Add(rd.ChildTableDescriptor);
                AddRelatedTables(rd.ChildTableDescriptor);
            }
        }

        #region IEnumerator Members

        /// <summary>
        /// Sets the enumerator to its initial position, which is before the first element in the collection.
        /// </summary>
        public virtual void Reset()
        {
            inner = new ArrayList();
            inner.Add(_engine.Table);
            AddRelatedTables(_engine.TableDescriptor);
            innerEnumerator = inner.GetEnumerator();
        }

        object IEnumerator.Current
        {
            get
            {
                return innerEnumerator.Current;
            }
        }

        /// <summary>
        /// Gets the current element in the collection.
        /// </summary>
        public TableDescriptor Current
        {
            get
            {
                return (TableDescriptor)innerEnumerator.Current;
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
            return innerEnumerator.MoveNext();
        }
        #endregion
    }

    /// <summary>
    /// Specifies the counter logic to be used within the engine. If you have large datasources
    /// and need support for groups and filtered records you can reduce the memory footprint
    /// by selectively disabling counters you do not need in your application. See WithoutCounter
    /// for completely disabling counter logic for the RecordsDetails collection and  VirtualMode
    /// for using the engine in a virtual mode.
    /// </summary>
    public enum EngineCounters
    {
        /// <summary>
        /// Counts visible elements and filtered records. Smallest memory footprint.
        /// </summary>
        /// <remarks>
        /// <list type="bullet">
        /// <item><term>Table.DisplayElements.GetItemAtYAmount and Table.DisplayElements.GetYAmountPositionOf
        /// are not supported.</term></item>
        /// <item><term>The Table.Elements collection will only return visible elements (same resultset as Table.DisplayElements)</term></item>
        /// <item><term>The Table.Records collection will return only filtered records (same resultset as Table.FilteredRecords)</term></item>
        /// <item><term>The Group.Records collection will return only filtered records (same resultset as Group.FilteredRecords)</term></item>
        /// <item><term>Table.DisplayElements.GetVisibleCustomCountPositionOf and Table.DisplayElements.GetItemAtVisibleCustomCount
        /// are not supported.</term></item>
        /// <item><term>Table.DisplayElements.GetCustomCountPositionOf and Table.DisplayElements.GetItemAtCustomCount
        /// are not supported.</term></item>
        /// </list>
        /// </remarks>
        FilteredRecords,

        /// <summary>
        /// Counts visible elements, filtered records and YAmount. Medium memory footprint. 
        /// </summary>
        /// <remarks>
        /// <list type="bullet">
        /// <item><term>Table.DisplayElements.GetItemAtYAmount and Table.DisplayElements.YAmountIndexOf
        /// are supported.</term></item>
        /// <item><term>The Table.Elements collection will only return visible elements (same resultset as Table.DisplayElements)</term></item>
        /// <item><term>The Table.Records collection will return only filtered records (same resultset as Table.FilteredRecords)</term></item>
        /// <item><term>The Group.Records collection will return only filtered records (same resultset as Group.FilteredRecords)</term></item>
        /// <item><term>Table.DisplayElements.GetVisibleCustomCountPositionOf and Table.DisplayElements.GetItemAtVisibleCustomCount
        /// are not supported.</term></item>
        /// <item><term>Table.DisplayElements.GetCustomCountPositionOf and Table.DisplayElements.GetItemAtCustomCount
        /// are not supported.</term></item>
        /// </list>
        /// </remarks>
        YAmount,

        /// <summary>
        /// Default. All counters are supported: visible elements, filtered records, YAmount, hidden elements, hidden records, CustomCount and VisibleCustomCount. Highest memory footprint. 
        /// </summary>
        /// <remarks>
        /// <list type="bullet">
        /// <item><term>Table.DisplayElements.GetItemAtYAmount and Table.DisplayElements.YAmountIndexOf
        /// are supported.</term></item>
        /// <item><term>The Table.Elements collection will return all elements (including hidden elements that are not visible)</term></item>
        /// <item><term>The Table.Records collection will return all records (including records that do not meet filter criteria)</term></item>
        /// <item><term>The Group.Records collection will return all records (including records that do not meet filter criteria)</term></item>
        /// <item><term>Table.DisplayElements.GetVisibleCustomCountPositionOf and Table.DisplayElements.GetItemAtVisibleCustomCount
        /// are supported.</term></item>
        /// <item><term>Table.DisplayElements.GetCustomCountPositionOf and Table.DisplayElements.GetItemAtCustomCount
        /// are supported.</term></item>
        /// </list>
        /// </remarks>
        All
    }

    /// <summary>
    /// Specifies various optimization the engine can use when applicable. These optimizations can be used 
    /// in combination with EngineCounter setting. 
    /// </summary>
    /// <remarks>
    /// Allowing certain optimizations does not mean that the optimzation is necessarily used. Optimizations
    /// will only be used when applicable. Take for example the <see cref="VirtualMode"/> optimization. If you
    /// allow this optimization the engine will check schema settings when loading the table. If there are
    /// no SortedColumns, RecordFilters, GroupedColumns and no nested relations for a table, then virtual mode
    /// can be used and no records need to be loaded into memory. If the user later sorts by one column, the
    /// virtual mode can not be used any more. Records will need to be iterated through and sorted and tree
    /// structures will be built that allow quick access to records and IndexOf operations. When initializing the
    /// table the engine will check if criteria for DisableCounters optimization are met.
    /// </remarks>
    [Flags]
    public enum EngineOptimizations
    {
        /// <summary>
        /// All optimizations are disabled.
        /// </summary>
        None = 0,

        /// <summary>
        /// When the engine detects that a table does not have RecordFilters, GroupedColumns
        /// or nested relations, counter logic will be disabled for the RecordsDetails collection
        /// since all counters are in sync with actual records (e.g. all records in datasource
        /// are shown in TopLevelGroup). With this optimization the engine does still have full
        /// support for sorting. 
        /// </summary>
        DisableCounters = 1,

        /// <summary>
        /// When all criteria are met for the <see cref="DisableCounters"/> optimization and in addition to that
        /// no SortedColumns are set, the RecordsDetails collection does not have to be initialized at all.
        /// Instead, it can create records elements on demand and discard them using regular garbage
        /// collection when no references to a Record exist any more (e.g. once you scroll them out of view).
        /// This approach reduces memory footprint to absolute minumum. You should be able to load and
        /// display millions of records in a table. <para/>
        /// The PrimaryKey collection is still supported, but it will be initialized only on demand if you
        /// do access the Table.PrimaryKeyRecords collection. In such case all records will be enumerated. 
        /// </summary>
        VirtualMode = 2,

        /// <summary>
        /// When all criteria are met for the <see cref="DisableCounters"/> optimization and SortedColumns are set, 
        /// the engine will normally have to loop through records and sort them. When you specify <see cref="PassThroughSort"/>
        /// the engine will check if the datasource is an IBindingList and if IBindingList.SupportsSort returns true.
        /// In such case the datasource will be sorted using its IBindingList.Sort routine and the engine will
        /// access records using VirtualMode. Using the IBindingList is usually a bit faster than the engines own
        /// sorting routines, but the disadvantage is that you will loose CurrentRecord and SelectedRecords information.
        /// Also, inserting and removing records will be slower (especially if the underlying datasource is a DataView).
        /// PassThroughSort will be ignored if criteria are met for the <see cref="DisableCounters"/> optimization are not
        /// met. If you want to force a Pass-through sort mechanism in such case check out the GroupingPerf example. It
        /// implements the IGroupingList interface. Normally, it is recommended to use the engines own Sort mechanism
        /// and only rely on PassThroughSort for Virtual mode scenarios.
        /// </summary>
        PassThroughSort = 4,

        /// <summary>
        /// When the engine detects that records do not have nested child tables, no record preview rows are being used
        /// and each record only has one row (no ColumnSets are used), records do not have to be split into RecordParts.
        /// Instead when qerying the DisplayElements collection for a specific row, the engine can simply
        /// return a Record element instead of a RecordRow element. The same applies to CaptionSection, ColumnHeaderSection
        /// and FilterBarSection. Instead of returning a CaptionRow, ColumnHeaderRow or FilterBarRow element
        /// the DisplayElements collection retuens the section element.
        /// <para/>
        /// If you use this optimizaton you need to carefull in your own code and be aware that when you query
        /// the DisplayElements collection instead of a RecordRow element a Record elemnt can be returned. Same issue
        /// also with ColumnHeader, FilterBase and Caption.
        /// </summary>
        RecordsAsDisplayElements = 8,

        /// <summary>
        /// Enables the <see cref="DisableCounters"/>, <see cref="VirtualMode"/> and <see cref="RecordsAsDisplayElements"/> 
        /// optimizations.
        /// </summary>
        [Browsable(false)]
        All = DisableCounters | VirtualMode | PassThroughSort | RecordsAsDisplayElements
    }    
}
