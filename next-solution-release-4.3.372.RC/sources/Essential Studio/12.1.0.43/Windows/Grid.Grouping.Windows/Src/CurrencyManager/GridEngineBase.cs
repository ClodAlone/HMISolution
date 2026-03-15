//-------------------------------------------------------------------------------------------------
// <copyright file="GridEngineBase.cs" company="syncfusion">
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
using System.Diagnostics;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using System.Reflection;

using Syncfusion.Collections;
using Syncfusion.Diagnostics;
using Syncfusion.Drawing;
using Syncfusion.Grouping;
using System.Data;

#if ASPNET
using System.Web.UI;
namespace Syncfusion.Web.UI.WebControls.Grid.Grouping
#else
namespace Syncfusion.Windows.Forms.Grid.Grouping
#endif
{
    /// <summary>
    /// The engine lets you set the main datasource for the whole engine. The TableDescriptor will
    /// pick up the ItemProperties (schema information) from the datasource and
    /// the table will be initialized at run-time with records from the list.
    /// </summary>
    /// <remarks>
    /// TableDescriptor is browsable. You can modify its collections and properties in the
    /// designer. <para/>
    /// By default, TableDescriptor is autopopulated. If you do not modify its settings and
    /// later change the datasource, it will be automatically reinitialized. If you have
    /// made modifications to TableDescriptor and change the SourceList, the modifications
    /// will be kept. To discard modifications of a TableDescriptor, you need to explicitly
    /// call ResetTableDescriptor.<para/>
    /// The table is dependent on information provided by the TableDescriptor and the records from
    /// SourceList. It is created on the fly and can not be designed with the designer.
    /// <para/>
    /// The GridEngineBase class adds design-time support for the engine class. It can be dropped as a component
    /// into the component tray of the designer. It can be initialized with a BindingContext so that the
    /// CurrencyManager can be kept in sync. You can specify a datasource using the DataSource and DataMember
    /// properties through the designer.
    /// <para/>
    /// The engine base class has no dependencies on System.Design and System.Windows.Forms. Only GridEngineBase adds these
    /// dependencies.
    /// </remarks>
    [TypeConverter(typeof(DescriptorBaseConverter))]
    [ToolboxItem(false)]
    public class GridEngineBase : Engine, IComponent
    {
        object dataSource;
        string dataMember;
#if ASPNET
#else
        CurrencyManager currencyManager;
        BindingContext bindingContext;
        int shadoweddataMemberVersion = -1;
        int shadoweddataSourceVersion = -1;
        bool inSetCurrencyManager;
        bool inBindingContextChanged;
        bool bindToCurrencyManager = true;

        static FieldInfo fInfo = null;
        static MethodInfo mInfo = null;
#endif
        ISite site;

        bool replaceSourceList = true;

        bool inDataSourceChanged;
        bool inDataMemberChanged;
        bool inSetDataBinding;

#if ASPNET
#else
        /// <summary>
        /// Specifies if list should be attached to <see cref="CurrencyManager"/>
        /// or if you would like the engine to be detached from a CurrencyManager. (Default is True).
        /// </summary>
        [DefaultValue(true)]
        [Description("Specifies if list should be attached to the CurrencyManager of the form or if you would like the engine to be detached from a CurrencyManager.")]
        [Category("Optimization")]
        public bool BindToCurrencyManager
        {
            get
            {
                return this.bindToCurrencyManager;
            }

            set
            {
                this.bindToCurrencyManager = value;
            }
        }
#endif

        /// <summary>
        /// Initializes the new engine.
        /// </summary>
        public GridEngineBase()
        {
        }

        /// <override/>
        /// <summary>Initializes this object and copies properties from another object.</summary>
        /// <param name="source">The source object.</param>
        public override void InitializeFrom(Engine source)
        {
            base.InitializeFrom(source);

            if (source is GridEngineBase)
            {
                GridEngineBase other = (GridEngineBase)source;
                this.BindToCurrencyManager = other.BindToCurrencyManager;
            }
        }

#if ASPNET
#else
        /// <summary>
        /// Returns True when SetCurrencyManager was called and False after SetCurrencyManager returned.
        /// </summary>
        [System.Xml.Serialization.XmlIgnore]
        [DesignerSerializationVisibilityAttribute(DesignerSerializationVisibility.Hidden)]
        [Browsable(false)]
        public bool InSetCurrencyManager
        {
            get
            {
                return inSetCurrencyManager;
            }
        }

        /// <summary>
        /// Returns True when OnBindingContextChanged was called and False after OnBindingContextChanged returned.
        /// </summary>
        [System.Xml.Serialization.XmlIgnore]
        [DesignerSerializationVisibilityAttribute(DesignerSerializationVisibility.Hidden)]
        [Browsable(false)]
        public bool InBindingContextChanged
        {
            get
            {
                return inBindingContextChanged;
            }
        }
#endif
        /// <summary>
        /// Returns True when OnDataSourceChanged was called and False after OnDataSourceChanged returned.
        /// </summary>
        [System.Xml.Serialization.XmlIgnore]
        [DesignerSerializationVisibilityAttribute(DesignerSerializationVisibility.Hidden)]
        [Browsable(false)]
        public bool InDataSourceChanged
        {
            get
            {
                return inDataSourceChanged;
            }
        }

        /// <summary>
        /// Returns True when OnDataMemberChanged was called and False after OnDataMemberChanged returned.
        /// </summary>
        [System.Xml.Serialization.XmlIgnore]
        [DesignerSerializationVisibilityAttribute(DesignerSerializationVisibility.Hidden)]
        [Browsable(false)]
        public bool InDataMemberChanged
        {
            get
            {
                return inDataMemberChanged;
            }
        }

        /// <summary>
        /// Returns True when SetDataBinding was called and False after SetDataBinding returned.
        /// </summary>
        [System.Xml.Serialization.XmlIgnore]
        [DesignerSerializationVisibilityAttribute(DesignerSerializationVisibility.Hidden)]
        [Browsable(false)]
        public bool InSetDataBinding
        {
            get
            {
                return inSetDataBinding;
            }
        }

        /// <summary>
        /// Occurs when the <see cref="DataSource"/> property is changed.
        /// </summary>
        [Description("Occurs when the DataSource property is changed.")]
        public event EventHandler DataSourceChanged;

        /// <summary>
        /// Occurs when the <see cref="DataMember"/> property is changed.
        /// </summary>
        [Description("Occurs when the DataMember property is changed.")]
        public event EventHandler DataMemberChanged;

#if ASPNET
#else
        /// <summary>
        /// Occurs when the <see cref="BindingContext"/> is changed.
        /// </summary>
        [Description("Occurs when the BindingContext is changed.")]
        public event EventHandler BindingContextChanged;
#endif
        /// <summary>
        /// Raises the <see cref="DataMemberChanged"/> event.
        /// </summary>
        /// <param name="e">An <see cref="EventArgs" /> that contains the event data.</param>
        protected virtual void OnDataMemberChanged(EventArgs e)
        {
            if (DataMemberChanged != null)
            {
                DataMemberChanged(this, e);
            }
        }

        /// <summary>
        /// Raises the <see cref="DataSourceChanged"/> event.
        /// </summary>
        /// <param name="e">An <see cref="EventArgs" /> that contains the event data.</param>
        protected virtual void OnDataSourceChanged(EventArgs e)
        {
            if (DataSourceChanged != null)
            {
                DataSourceChanged(this, e);
            }
        }

#if ASPNET
        /// <override/>
        protected override IList GetSourceListBase()
        {
            replaceSourceList = false;
            return this.GetResolvedDataSource();
        }
        internal static IList GetResolvedDataSource(object dataSource, string dataMember)
        {
            if (dataSource == null)
            {
                return null;
            }
            if (dataSource is IListSource || dataSource is IList)
            {
                IList memberList = null;
                if(dataSource is IListSource)
                {
                    IListSource listSource = dataSource as IListSource;
#if SyncfusionFramework2_0
                    if (listSource is IDataSource)
                    {
                        memberList = ListSourceHelper.GetList(listSource as IDataSource);
                        if (ListSourceHelper.ContainsListCollection(listSource as IDataSource) == false)
                        {
                            return memberList;
                        }
                    }
                    else
#endif
                    {
                        memberList = listSource.GetList();
                        if (listSource.ContainsListCollection == false)
                        {
                            return memberList;
                        }
                    }

                    // Previously we used to try to retrieve the child-list based on the DataMember even if the
                    // dataSource is a IList, but now we do this only if the data source is a IListSource.

                    ITypedList typedMemberList = memberList as ITypedList;
                    if (typedMemberList != null)
                    {
                        PropertyDescriptorCollection propDescs = typedMemberList.GetItemProperties(new PropertyDescriptor[0]);
                        PropertyDescriptor memberProperty = null;
                        if ((propDescs != null) && (propDescs.Count != 0))
                        {
                            if (dataMember == null || dataMember.Length == 0)
                            {
                                memberProperty = propDescs[0];
                            }
                            else
                            {
                                memberProperty = propDescs.Find(dataMember, true);
                            }
                            if (memberProperty != null)
                            {
                                object listRow = memberList[0];
                                object list = memberProperty.GetValue(listRow);
                                if (list is IList)
                                {
                                    return (IList)list;
                                }
                        else if (list is ICollection)
              {
                // Here we use CollectionWrapper to convert a ICollection into a IList so that the grid can work with it.
                // Note that the resultant will be a readonly IList. When the user tries to save an editd a record, an error will occur
                // but that's fine for now since the same happens with the GridView.
                return new CollectionWrapper(list as ICollection);
              }
                            }
                            throw new Exception("A list corresponding to the selected DataMember was not found.");
                        }
                        throw new Exception("The selected datasource did not contain any data members to bind to.");
                    }
                }
                else
                {
                    memberList = dataSource as IList;
                    // We will return the memberList anyway
                    //if(memberList is System.Data.DataView)
                    //    return memberList;
                }

                
                return memberList;
            }
            return null;
        }
        /// <summary>
        /// Returns a IList source from the specified DataSource and DataMember.
        /// </summary>
        /// <returns>A valid IList list if available or NULL.</returns>
        protected virtual IList GetResolvedDataSource()
        {
            return GetResolvedDataSource(this.dataSource, this.DataMember);
        }
#else
        /// <summary>
        /// Raises the <see cref="BindingContextChanged"/> event.
        /// </summary>
        /// <param name="e">An <see cref="EventArgs" /> that contains the event data.</param>
        protected virtual void OnBindingContextChanged(EventArgs e)
        {
            if (BindingContextChanged != null)
            {
                BindingContextChanged(this, e);
            }
        }

        /// <summary>
        /// Gets / sets the BindingContext for the control.
        /// </summary>
        /// <remarks>
        /// The BindingContext object of a Control is used to return a single BindingManagerBase object for all
        /// data-bound controls contained by the Control. The BindingManagerBase object keeps all controls that
        /// are bound to the same datasource synchronized. For example, setting the Position property of the
        /// BindingManagerBase specifies the item in the underlying list that all data-bound controls point to.<para/>
        /// For more information about creating a new BindingContext and assigning it to the BindingContext property,
        /// see the BindingContext.
        /// </remarks>
        [System.Xml.Serialization.XmlIgnore]
        [DesignerSerializationVisibilityAttribute(DesignerSerializationVisibility.Hidden)]
        [Browsable(false)]
        public BindingContext BindingContext
        {
            get
            {
                if (bindingContext == null)
                {
                    bindingContext = new BindingContext();
                }

                return bindingContext;
            }

            set
            {
                if (bindingContext != value)
                {
                    bindingContext = value;
                    try
                    {
                        inBindingContextChanged = true;
                        OnBindingContextChanged(EventArgs.Empty);
                    }
                    finally
                    {
                        inBindingContextChanged = false;
                    }
                }
            }
        }

        /// <summary>
        /// Gets the CurrencyManager for the assigned DataSource and DataMember.
        /// </summary>
        [System.Xml.Serialization.XmlIgnore]
        [DesignerSerializationVisibilityAttribute(DesignerSerializationVisibility.Hidden)]
        [Browsable(false)]
        public CurrencyManager CurrencyManager
        {
            get
            {
                if (!bindToCurrencyManager)
                {
                    throw new NotSupportedException("Support for CurrencyManager is disabled (BindToCurrencyManager = False).");
                }

                CurrencyManager cm = currencyManager;
                if (shadoweddataMemberVersion != dataMemberVersion
                    || shadoweddataSourceVersion != dataSourceVersion)
                {
                    shadoweddataMemberVersion = dataMemberVersion;
                    shadoweddataSourceVersion = dataSourceVersion;
                    cm = null;
                }

                if (cm == null)
                {
                    try
                    {
                        if (DataSource != null)
                        {
                            cm = (CurrencyManager)BindingContext[DataSource, DataMember];
                        }
                        else if (HasSourceList())
                        {
                            cm = (CurrencyManager)BindingContext[GetSourceList()];
                        }
                    }
                    catch (Exception ex)
                    {
                        TraceUtil.TraceExceptionCatched(ex);
                    }
                }

                SetCurrencyManager(cm);

                return currencyManager;
            }
        }

        /// <summary>
        /// This virtual method is called when there has previously been no datasource specified and
        /// a <see cref="M:Syncfusion.Grouping.Engine.GetSourceList"/> call occurs. Override this method to allow retrieving the
        /// source list on demand.
        /// </summary>
        /// <returns>The source list for this engine.</returns>
        /// <override/>
        protected override IEnumerable GetSourceListBase()
        {
            try
            {
                if (this.bindToCurrencyManager && !(DataSource is IPassThroughGroupingResult))
                {
                    if (DataSource == null)
                    {
                        return null;
                    }

                    if (CurrencyManager != null)
                    {
                        return (IList)ResolveBindingSource(CurrencyManager.List);
                    }
                }
                else
                {
                    if (!this.HasSourceList() && DataSource != null)
                    {
                        object value = ResolveBindingSource(DataSource);

                        IEnumerable list = null;

                        if (this.AllowSwapDataViewWithDataTableList)
                        {
                            if (value is System.Data.DataView)
                            {
                                list = new DataTableList(((System.Data.DataView)value).Table);
                            }
                            else if (value is System.Data.DataTable && this.AllowSwapDataViewWithDataTableList)
                            {
                                list = new DataTableList((System.Data.DataTable)value);
                            }
                        }

                        if (list == null)
                        {
                            if (value is IEnumerable)
                            {
                                list = (IEnumerable)value;
                            }
                            else if (value is IListSource)
                            {
                                list = ((IListSource)value).GetList();
                            }
                        }

                        if (list != null)
                        {
                            SetSourceList(list);
                            return list;
                        }
                    }
                }

                return null;
            }
            finally
            {
                replaceSourceList = false;
            }
        }

#if !ASPNET
        bool allowResolveBindingSource = true;

        /// <summary>
        /// This property indicates if the engine is allowed to resolve a BindingSource created by Whidbey design-time to the DataSet it wraps. Only when the engine is allowed to resolve the 
        /// BindingSource it will be able to properly detected nested relation and foreign-key relations that
        /// were setup in the DataSet.
        /// </summary>
        [Category("Data"),
    DefaultValue(true),
    Description("This property indicates if the engine is allowed to resolve a BindingSource created by Whidbey design-time to the DataSet it wraps."),
        Browsable(false),
        DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        [System.Xml.Serialization.XmlIgnore]
        public bool AllowResolveBindingSource
        {
            get { return allowResolveBindingSource; }
            set { allowResolveBindingSource = value; }
        }
#endif
        object ResolveBindingSource(object value)
        {
#if !ASPNET && SyncfusionFramework2_0
            if (AllowResolveBindingSource)
            {
                // Resolve BindingSource to real list.
                System.Windows.Forms.BindingSource bs = value as System.Windows.Forms.BindingSource;
                if (bs != null && bs.List is System.Data.DataSet)
                {
                    value = bs.List;
                }

                //// look only for a dataset - if it is a regular list, e.g. custom collection 
                //// bind directly to BindingSource.
                IListSource ls = value as System.Data.DataSet;
                while (ls != null && ls.ContainsListCollection)
                {
                    value = ls.GetList();
                    ls = value as IListSource;
                }
            }
#endif
            return value;
        }

        void SetCurrencyManager(CurrencyManager value)
        {
            if (!bindToCurrencyManager)
            {
                ////currencyManager = null;
                ////return;
                throw new NotSupportedException("Support for CurrencyManager is disabled (BindToCurrencyManager = False).");
            }

            if (currencyManager != value)
            {
                inSetCurrencyManager = true;
                UnwireCurrencyManager();
                currencyManager = value;
                if (value == null)
                {
                    shadoweddataMemberVersion = -1;
                }
                ////                calling below: else if (!InSourceListChanged)
                ////                    SetSourceList(currencyManager != null ? currencyManager.List : null);
                WireCurrencyManager();
                inSetCurrencyManager = false;
            }
            else if (value == null)
            {
                inSetCurrencyManager = true;
                ////                calling below: if (!InSourceListChanged)
                ////                    SetSourceList(null);
                inSetCurrencyManager = false;
            }

            ////              cm_CurrentChanged calls this
            inSetCurrencyManager = true;
            SetSourceList((IList)this.ResolveBindingSource(currencyManager != null ? currencyManager.List : null));
            inSetCurrencyManager = false;
        }

        /// <summary>
        /// Used internally.
        /// </summary>
        /// <param name="cm">The CurrencyManager.</param>
        /// <returns>returns the parent CurrencyManager</returns>
        /// <internalonly/>
        [Syncfusion.Documentation.DocumentationExclude()]
        public static CurrencyManager GetParentCurrenyManager(CurrencyManager cm)
        {
            if (cm.GetType().FullName == "System.Windows.Forms.RelatedCurrencyManager")
            {
                if (mInfo == null && fInfo == null)
                {
                    Type t = cm.GetType();
                    PropertyInfo pi = t.GetProperty(
                        "ParentManager",
                        System.Reflection.BindingFlags.GetProperty | System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.IgnoreReturn | System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.NonPublic);
                    if (pi != null)
                    {
                        mInfo = pi.GetGetMethod(true);
                    }
                    else 
                    {
                        //// In Whidbey RelatedCurrencyManager.ParentManager property has been removed. 
                    //// A parentManager field is available.
                        fInfo = t.GetField("parentManager", System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.NonPublic);
                    }
                }

                if (mInfo != null)
                {
                    return mInfo.Invoke(cm, new object[0]) as CurrencyManager;
                }

                if (fInfo != null)
                {
                    return fInfo.GetValue(cm) as CurrencyManager;
                }
            }
#if SyncfusionFramework2_0
            if (cm.List is ICurrencyManagerProvider)
            {
                return (cm.List as ICurrencyManagerProvider).CurrencyManager;
                ////manager1.CurrentItemChanged += new EventHandler(this.ParentCurrencyManager_CurrentItemChanged);
                ////manager1.MetaDataChanged += new EventHandler(this.ParentCurrencyManager_MetaDataChanged);
            }
#endif

            return null;
        }

        void WireCurrencyManager()
        {
            if (currencyManager != null)
            {
                CurrencyManager cm = GetParentCurrenyManager(currencyManager);
                if (cm != null)
                {
                    cm.CurrentChanged += new EventHandler(cm_CurrentChanged);
                }
            }
        }

        private void cm_CurrentChanged(object sender, EventArgs e)
        {
            //// Work around issue that in VS 2005 the currencyManager.List sometimes
            //// returns the old list when called from within CurrentChanged event handler.
            //// Accessing the property after the event returns fixes the problem in
            //// MasterDetailForm example.
            Timer t = new Timer();
            t.Tick += new EventHandler(t_Tick);
            t.Interval = 10;
            t.Start();
        }

        void t_Tick(object sender, EventArgs e)
        {
            Timer t = (Timer)sender;
            t.Tick -= new EventHandler(t_Tick);
            t.Stop();
            t.Dispose();
            this.SetCurrencyManager(this.currencyManager);
        }

        void UnwireCurrencyManager()
        {
            if (currencyManager != null)
            {
                CurrencyManager cm = GetParentCurrenyManager(currencyManager);
                if (cm != null)
                {
                    cm.CurrentChanged -= new EventHandler(cm_CurrentChanged);
                }
            }
        }
#endif
        /// <summary>
        /// Determines if the sourcelist should be reinitialized the next time <see cref="M:Syncfusion.Grouping.Engine.GetSourceList"/> is called.
        /// </summary>
        /// <returns>
        /// True if the sourcelist should be reinitialized the next time <see cref="M:Syncfusion.Grouping.Engine.GetSourceList"/> is called; False if not.
        /// </returns>
        /// <override/>
        protected override bool ShouldReplaceSourceList()
        {
            ////            if (shadoweddataMemberVersion != dataMemberVersion
            ////                || shadoweddataSourceVersion != dataSourceVersion)
            ////            {
            ////                shadoweddataMemberVersion = dataMemberVersion;
            ////                shadoweddataSourceVersion = dataSourceVersion;
            ////                SetCurrencyManager(null);
            ////                return true;
            ////            }
            return replaceSourceList;
        }
#if ASPNET
#else
        /// <override/>
        protected override void OnSourceListChanged(EventArgs e)
        {
            if (this.bindToCurrencyManager && !InSetCurrencyManager)
            {
                SetCurrencyManager(null);
            }

            base.OnSourceListChanged(e);
        }
#endif

        static bool defaultAllowResetTableDescriptorWhenDataSourceSetNull = true;

        /// <summary>
        /// This property specifies if the engine should reset the TableDescriptor and Relations when you set the Engine.DataSource = null. 
        /// </summary>
        [Category("Data"),
        DefaultValue(true),
        Description("This property specifies if the engine should reset the TableDescriptor and Relations when you set the Engine.DataSource = null. ")]
        public static bool DefaultAllowResetTableDescriptorWhenDataSourceSetNull
        {
            get { return defaultAllowResetTableDescriptorWhenDataSourceSetNull; }
            set { defaultAllowResetTableDescriptorWhenDataSourceSetNull = value; }
        }

        bool allowResetTableDescriptorWhenDataSourceSetNull = true;
        
        /// <summary>
        /// This property specifies if the engine should reset the TableDescriptor, Relations
        /// and clear out the SourceListSet when you set the Engine.DataSource = null. 
        /// </summary>
        [Category("Data"),
        DefaultValue(true),
        Description("This property specifies if the engine should reset the TableDescriptor and Relations when you set the Engine.DataSource = null. ")]
        public bool AllowResetTableDescriptorWhenDataSourceSetNull
        {
            get { return allowResetTableDescriptorWhenDataSourceSetNull; }
            set { allowResetTableDescriptorWhenDataSourceSetNull = value; }
        }

        /// <summary>
        /// Determines whether the value of AllowResetTableDescriptorWhenDataSourceSetNull property was modified.
        /// </summary>
        /// <returns>True if it was modified.</returns>
        public bool ShouldSerializeAllowResetTableDescriptorWhenDataSourceSetNull()
        {
            return allowResetTableDescriptorWhenDataSourceSetNull != defaultAllowResetTableDescriptorWhenDataSourceSetNull;
        }

        /// <summary>
        /// Resets the value of AllowResetTableDescriptorWhenDataSourceSetNull property to its default value.
        /// </summary>
        public void ResetAllowResetTableDescriptorWhenDataSourceSetNull()
        {
            allowResetTableDescriptorWhenDataSourceSetNull = defaultAllowResetTableDescriptorWhenDataSourceSetNull;
        }

        #region AllowResetSourceListWhenDataSourceChanged
        static bool defaultAllowResetSourceListWhenDataSourceChanged = true;

        /// <summary>
        /// This property specifies if the engine should immediately reset the SourceListSet and SourceList when you change the Engine.DataSource. 
        /// </summary>
        [Category("Data"),
        DefaultValue(true),
        Description("This property specifies if the engine should immediately reset the SourceListSet and SourceList when you change the Engine.DataSource. ")]
        public static bool DefaultAllowResetSourceListWhenDataSourceChanged
        {
            get { return defaultAllowResetSourceListWhenDataSourceChanged; }
            set { defaultAllowResetSourceListWhenDataSourceChanged = value; }
        }

        bool allowAllowResetSourceListWhenDataSourceChanged = true;

        /// <summary>
        /// This property specifies if the engine should immediately reset the SourceListSet and SourceList when you change the Engine.DataSource. 
        /// </summary>
        [Category("Data"),
        DefaultValue(true),
        Description("This property specifies if the engine should immediately reset the SourceListSet and SourceList when you change the Engine.DataSource. ")]
        public bool AllowResetSourceListWhenDataSourceChanged
        {
            get { return allowAllowResetSourceListWhenDataSourceChanged; }
            set { allowAllowResetSourceListWhenDataSourceChanged = value; }
        }

        /// <summary>
        /// Determines whether the value of AllowResetSourceListWhenDataSourceChanged proeprty was modified.
        /// </summary>
        /// <returns>True if it was modified.</returns>
        public bool ShouldSerializeAllowResetSourceListWhenDataSourceChanged()
        {
            return allowAllowResetSourceListWhenDataSourceChanged != defaultAllowResetSourceListWhenDataSourceChanged;
        }
        /// <summary>
        /// Determine the reset.
        /// </summary>
        public override void Reset()
        {
            ResetAllowResetSourceListWhenDataSourceChanged();
            ResetAllowResetTableDescriptorWhenDataSourceSetNull();
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
        /// Resets the value of AllowResetSourceListWhenDataSourceChanged property to its default value.
        /// </summary>
        public void ResetAllowResetSourceListWhenDataSourceChanged()
        {
            allowAllowResetSourceListWhenDataSourceChanged = defaultAllowResetSourceListWhenDataSourceChanged;
        }
        #endregion

        private bool optimizeGroupingPerformance = false;
        /// <summary>
        /// This property gets / sets if the optimized grouping performance needed for IList datasource.
        /// </summary>
        /// <remarks>
        /// When this property is set to true, the method OptimizeIListGroupingPeformance need to be invoked explicitly
        /// to provide support for real-time updates in grid.
        /// </remarks>
        [Category("Data"),
        DefaultValue(true),
        Description("This property gets / sets if the optimized grouping performance needed for IList datasource.")]
        public bool OptimizeIListGroupingPerformance
        {
            get
            {
                return optimizeGroupingPerformance;
            }
            set
            {
                optimizeGroupingPerformance = value;
            }
        }
        
        int dataSourceVersion;

        /// <summary>
        ///   <para>Gets / sets the data source that the control is displaying data for.</para>
        /// </summary>
        [Description(@"Indicates the source of data for the DataGrid."),
        DefaultValue(null),
        Category(@"Data"),
        RefreshProperties(RefreshProperties.All),
        TypeConverter("System.Windows.Forms.Design.DataSourceConverter, System.Design")]
        [System.Xml.Serialization.XmlIgnore]
#if SyncfusionFramework2_0
        [AttributeProvider(typeof(IListSource))]
#endif
        public object DataSource
        {
            get
            {
                return dataSource;
            }

            set
            {
                CheckValidDataSource(value);
                if (dataSource != value)
                {
                    this.Table.CurrentElement = null;
                    if (this.Table.CurrentElement != null)
                    {
                        this.Table.CurrentRecordManager.Reset();
                    }

                    this.OnPropertyChanging(new DescriptorPropertyChangedEventArgs("DataSource"));

                    bool shouldResetTD = value == null && dataSource != null && AllowResetTableDescriptorWhenDataSourceSetNull;
                    bool shouldResetSourceList = value != null && dataSource != null && AllowResetSourceListWhenDataSourceChanged;

                    if (value is IList && OptimizeIListGroupingPerformance)
                    {
                        IList list = (IList)value;
                        PropertyDescriptorCollection propCol = ListUtil.GetItemProperties(list);
                        DataTable dt = new DataTable(propCol[0].ComponentType.Name);
                        for (int i = 0; i < propCol.Count; i++)
                        {
                            Type type = propCol[i].PropertyType;
                            if (type.IsGenericType && type.GetGenericTypeDefinition() == typeof(Nullable<>))
                                type = Nullable.GetUnderlyingType(type);
                            DataColumn col = new DataColumn(propCol[i].Name, type);
                            dt.Columns.Add(col);
                        }
                        foreach (object item in list)
                        {
                            DataRow row = dt.NewRow();
                            foreach (DataColumn col in dt.Columns)
                            {
                                object obj = propCol[col.ColumnName].GetValue(item);
                                if (obj == null)
                                    row[col.ColumnName] = DBNull.Value;
                                else
                                    row[col.ColumnName] = obj;
                            }
                            dt.Rows.Add(row);
                        }
                        dataSource = dt;
                    }
                    else
                        dataSource = value;

                    if (shouldResetTD)
                    {
                        ResetTableDescriptor();
                        SourceListSet.Clear();
                        SetSourceList(null);
                    }
                    else if (shouldResetSourceList)
                    {
                        ////SourceListSet.Clear();
                        SetSourceList(null);
                    }

                    if (dataSource is IPassThroughGroupingResult)
                    {
                        bindToCurrencyManager = false;
                    }

                    try
                    {
                        inDataSourceChanged = true;
                        dataSourceVersion++;
                        replaceSourceList = true;
                        OnDataSourceChanged(EventArgs.Empty);
                        this.OnPropertyChanged(new DescriptorPropertyChangedEventArgs("DataSource"));
#if ASPNET
                        this.GetSourceList();
#endif
                    }
                    finally
                    {
                        inDataSourceChanged = false;
                    }
                }
            }
        }

        void CheckValidDataSource(object value)
        {
            if (!(value is DBNull) && value != null && !(value is IEnumerable) && !(value is IListSource) && !(value is System.Data.DataTable))
            {
                throw new Exception("BadDataSourceForComplexBinding");
            }
        }

        string FixNullString(string s)
        {
            return s != null ? s : string.Empty;
        }

        int dataMemberVersion;

        /// <summary>
        /// Gets / sets the specific list in a <see cref="DataSource" /> for which the control displays the data.
        /// </summary>
        [Category("Data"),
        RefreshProperties(RefreshProperties.All),
        DefaultValue(""),
        Editor("System.Windows.Forms.Design.DataMemberListEditor, System.Design", "System.Drawing.Design.UITypeEditor, System.Drawing"),
        Description("Indicates a sub-list of the DataSource to show in the grid.")]
        public string DataMember
        {
            get
            {
#if ASPNET
                return dataMember;
#else
                return DataSource != null ? dataMember : string.Empty;
#endif
            }

            set
            {
                value = FixNullString(value);
                if (dataMember != value)
                {
                    this.Table.CurrentElement = null;
                    if (this.Table.CurrentElement != null)
                    {
                        this.Table.CurrentRecordManager.Reset();
                    }

                    this.OnPropertyChanging(new DescriptorPropertyChangedEventArgs("DataMember"));
                    dataMember = value;

                    try
                    {
                        inDataMemberChanged = true;
                        dataMemberVersion++;
                        replaceSourceList = true;
                        ////SetCurrencyManager(null);
                        OnDataMemberChanged(EventArgs.Empty);
                        this.OnPropertyChanged(new DescriptorPropertyChangedEventArgs("DataMember"));
                    }
                    finally
                    {
                        inDataMemberChanged = false;
                    }
                }
            }
        }

        /// <summary>
        ///   <para>Sets the <see cref="DataSource" /> and <see cref="DataMember" /> properties at run-time.</para>
        /// </summary>
        /// <param name="dataSource">The data source, typed as <see cref="System.Object" />.</param>
        /// <param name="dataMember">The <see cref="DataMember" /> string that specifies the table to bind to within the object returned by the <see cref="DataSource" /> property.</param>
        public void SetDataBinding(object dataSource, string dataMember)
        {
            inSetDataBinding = true;
            DataSource = dataSource;
            DataMember = dataMember;
            inSetDataBinding = false;
        }

        #region IComponent Members

        /// <summary>
        /// Gets / sets the ISite of the Component.
        /// </summary>
        /// <remarks>
        /// The ISite associated with the Component, if any.<para/>
        /// This value is a null reference (Nothing in Visual Basic) if the Component is
        /// not encapsulated in an IContainer, the Component does not have an ISite associated with it,
        /// or the Component is removed from its IContainer.
        /// </remarks>
        [System.Xml.Serialization.XmlIgnore]
        [DesignerSerializationVisibilityAttribute(DesignerSerializationVisibility.Hidden)]
        [Browsable(false)]
        public ISite Site
        {
            get
            {
                return site;
            }

            set
            {
                site = value;
            }
        }
        #endregion

        /// <summary>
        /// Gets a value that indicates whether the Component is currently in design mode.
        /// </summary>
        [System.Xml.Serialization.XmlIgnore]
        [DesignerSerializationVisibilityAttribute(DesignerSerializationVisibility.Hidden)]
        [Browsable(false)]
        protected bool DesignMode
        {
            get
            {
#if EMUDESIGN
                return true;
#endif
                if (site != null)
                {
                    return site.DesignMode;
                }

                return false;
            }
        }

        /// <summary>
        /// Returns an object that represents a service provided by the Component or by its Container.
        /// </summary>
        /// <param name="service">A service provided by the Component. </param>
        /// <returns>An Object that represents a service provided by the Component.
        /// This value is a NULL reference (Nothing in Visual Basic) if the Component does not provide
        /// the specified service.
        /// </returns>
        public virtual object GetService(Type service)
        {
            if (site != null)
            {
                return site.GetService(service);
            }

            return null;
        }

        /// <summary>
        /// Gets the IContainer that contains the Component.
        /// </summary>
        /// <remarks>
        /// The IContainer that contains the Component, if any.<para/>
        /// This value is a NULL reference (Nothing in Visual Basic) if the Component is not encapsulated in an IContainer.
        /// </remarks>
        [System.Xml.Serialization.XmlIgnore]
        [DesignerSerializationVisibilityAttribute(DesignerSerializationVisibility.Hidden)]
        [Browsable(false)]
        public IContainer Container
        {
            get
            {
                if (site != null)
                {
                    return site.Container;
                }

                return null;
            }
        }

        /// <override/>
        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                lock (this)
                {
                    if (this.site != null && this.site.Container != null)
                    {
                        this.site.Container.Remove(this);
                    }
                }
            }

            base.Dispose(disposing);
        }
    }
}