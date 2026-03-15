//-------------------------------------------------------------------------------------------------
// <copyright file="GridGroupingControl.cs" company="syncfusion">
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
using System.Data;
using System.Globalization;
using System.Text;
using System.Windows.Forms;
using System.Windows.Forms.Design;
using System.Xml.Serialization;
using System.Xml;

using Syncfusion.Collections;
using Syncfusion.ComponentModel;
using Syncfusion.Drawing;
using Syncfusion.Grouping;
using Syncfusion.Diagnostics;
using Syncfusion.Windows.Forms;
using Syncfusion.Windows.Forms.Grid.Grouping.Design;
using Syncfusion.Windows.Forms.Grid.Grouping.Localization;
using Syncfusion.Styles;
using System.Drawing.Design;
using System.Collections.Generic;

namespace Syncfusion.Windows.Forms.Grid.Grouping
{
    /// <summary>
    /// Provides support for displaying ADO.NET data and other datasources in a grid. Data will be
    /// loaded from the given datasource and changes will be written back to the datasource. Both
    /// flat tables and hierarchical data can be displayed. Records can be grouped into categories and
    /// summaries can be displayed for each group.
    /// </summary>
    /// <remarks>
    /// To display a table in the <see cref="GridGroupingControl"/> at run-time, set the <see cref="DataSource"/>
    /// and <see cref="DataMember"/> properties to a valid datasource. The following datasources are valid:
    /// <list type="bullet">
    /// <item><term>A DataTable</term></item>
    /// <item><term>A DataView</term></item>
    /// <item><term>A DataSet</term></item>
    /// <item><term>A single dimension array</term></item>
    /// <item><term>Any component that implements the IListSource interface</term></item>
    /// <item><term>Any component that implements the IList interface.</term></item>
    /// </list>
    /// <para/>
    /// GridGroupingControl is a user control that hosts a <see cref="GridTableControl"/>,
    /// a <see cref="GridGroupDropArea"/>, and a <see cref="RecordNavigationBar"/>.
    /// <para/>
    /// Its main properties for the DataModel are: <see cref="DataSource"/>, <see cref="DataMember"/>,
    /// <see cref="Engine"/>, <see cref="TableDescriptor"/>, and <see cref="Table"/>.
    /// <para/>
    /// Properties for user display controls are: <see cref="TableControl"/>, <see cref="GridGroupDropArea"/>, and <see cref="RecordNavigationBar"/>.
    /// <para/>
    /// The <see cref="GridGroupingControl"/> provides properties to get / set the DataSource using the <see cref="DataSource"/>
    /// and <see cref="DataMember"/> properties. The DataSource can be set through the designer.
    /// <para/>
    /// The <see cref="TableControl"/> property gives access to the hosted <see cref="GridTableControl"/>.
    /// The <see cref="GridTableControl"/> is a grid derived from <see cref="GridControlBase"/>.
    /// <para/>
    /// The <see cref="GridGroupDropArea"/> property gives access to the hosted <see cref="Syncfusion.Windows.Forms.Grid.Grouping.GridGroupDropArea"/> control.
    /// Its purpose is to allow dragging header columns from the <see cref="GridTableControl"/>
    /// for user-interactive grouping of records. You can hide and show this area with the <see cref="ShowGroupDropArea"/>
    /// property.
    /// <para/>
    /// The <see cref="RecordNavigationBar"/> property gives access to the hosted <see cref="Syncfusion.Windows.Forms.RecordNavigationBar"/> control.
    /// You can show or hide the record navigation bar with the <see cref="ShowNavigationBar"/> property.
    /// <para/>
    /// The <see cref="GridTableControl"/> is the main element in the <see cref="GridGroupingControl"/>. The
    /// <see cref="GridTableControl"/> displays the rows from the <see cref="Syncfusion.Grouping.Table.DisplayElements"/> collection
    /// of the <see cref="GridGroupingControl.Table"/> using schema information stored in the <see cref="GridGroupingControl.TableDescriptor"/>.
    /// <para/>
    /// <see cref="GridGroupingControl.TableDescriptor"/> gives access to the table schema information of the
    /// root table in the datasource.
    /// <para/>
    /// The <see cref="GridGroupingControl.TableDescriptor"/> object is instantiated by the <see cref="GridEngine"/> class
    /// and initialized with default schema information from the list assigned to <see cref="GridGroupingControl.DataSource"/>.
    /// <para/>
    /// The <see cref="GridEngine"/> is accessed through the <see cref="GridGroupingControl.Engine"/> property.
    /// The <see cref="GridEngine"/> object is instantiated with the virtual <see cref="GridGroupingControl.CreateEngine"/> method.
    /// If you want to subclass the <see cref="GridEngine"/>, you should override this method.
    /// <para/>
    /// The <see cref="GridGroupingControl.Engine"/> object is the main grouping engine object. It is derived from the
    /// <see cref="Syncfusion.Grouping.Engine"/> base class and adds Windows Forms specific functionality such as support
    /// for a Forms BindingContext and CurrencyManager. GridEngine also has special overrides of the virtual
    /// Engine.CreateTableDescriptor and Engine.CreateTable methods so that the grid-specific derived GridTable class
    /// (derived from Synfusion.Grouping.Engine) and GridTableDescriptor class (derived from Syncfusion.Grouping.TableDescriptor)
    /// are instantiated.
    /// <para/>
    /// There is only one <see cref="GridEngine"/> object for a <see cref="GridGroupingControl"/>.
    /// <see cref="GridTableDescriptor"/> and <see cref="GridTable"/> objects on the other side can be more than one when
    /// hierarchies are displayed. For each hierarchy level, a <see cref="GridTableDescriptor"/> and <see cref="GridTable"/>
    ///  are initialized. For example, if you have an ADO.NET DataSet with three tables: "Products", "Orders", and "OrderDetails", there will be three GridTableDescriptors and GridTables.
    /// <para/>
    /// Relations between tables are defined with a <see cref="GridTableDescriptor.Relations"/> collection of a <see cref="TableDescriptor"/>.
    /// Each TableDescriptor can have one or multiple <see cref="RelationDescriptor"/> objects. A <see cref="RelationDescriptor"/>
    /// defines the foreign key columns in the parent table, a child <see cref="TableDescriptor"/> with information about the
    /// related child table and the primary key columns in the child table.
    /// <para/>
    /// The <see cref="GridTable"/> object is instantiated by the <see cref="GridEngine"/> class. The Table object manages the
    /// records from the engine's DataSource and provides access to records and grouped elements through several collection classes.
    /// The most important collection used by the GridTableControl is the DisplayElements collection. This collection provides
    /// the GridTableControl with information on which element to display at a row.
    /// It returns elements such as CaptionSection, RecordRow, SummaryRow, and others.
    /// Based on the elements returned by this collection, the GridTableControl will display a record, a summary or a group caption bar.
    /// There are several collections returned such as Records which contains all records in the table. FilterRecords contains all
    ///  visible records.
    /// <para/>
    /// </remarks>
    [ToolboxItem(true)]
    [System.Drawing.ToolboxBitmap(typeof(GridGroupingControl), "ToolboxIcons.GridGroupingControl.bmp")]
    [DefaultProperty("DataSource")]
    [Designer(typeof(GridGroupingControlDesigner))]
#if SyncfusionFramework2_0
    [Description("Displays and groups hierarchical ADO.NET data and other datasources in a grid"),
    DefaultEvent("TableControlCellClick"),
    Docking(DockingBehavior.Ask)]
#endif
    public class GridGroupingControl : System.Windows.Forms.Control, IEngineSource, ISupportInitialize, ICustomTypeDescriptor, ITableEventsTarget, IVisualStyle
    {
        private Syncfusion.Windows.Forms.RecordNavigationControl recordNavigationControl1;
        private Syncfusion.Windows.Forms.Grid.Grouping.GridTableControl tableControl1;
        private Syncfusion.Windows.Forms.Grid.Grouping.GridGroupDropArea groupDropArea1;
        internal GridControl captionGrid = new GridControl();
        private System.Windows.Forms.Panel gridTablePanel;
        private System.Windows.Forms.Panel groupDropPanel;
        private System.Windows.Forms.Splitter splitter1;
        GridEngine engine;
        int navigationBarWidth;
        private BorderStyle borderStyle = BorderStyle.FixedSingle;
        private bool showGroupDropArea;
        private bool allowProportionalColumnSizing = false;
        private bool browseOnly = false;
        private bool isMetroSettingsApplied = false;
        ////private bool themesEnabled = false;
        private ColorStyles colorStyles = ColorStyles.SystemTheme;
        GridCaptionRow capRow;
        Group g;

        string versionInfo = string.Empty;
        Dictionary<string, int> sizedColumns = null;
        /// <summary>
        /// Will be written out by CodeDom serializer to help us internally
        /// determine the version that was used when code was generated.
        /// </summary>
        [Browsable(false)]
        [DefaultValue("")]
        [Description("Product Version Information at the time this control was dropped on a form.")]
        [Category("Grouping Control")]
        public string VersionInfo
        {
            get
            {
                return versionInfo;
            }

            set
            {
                versionInfo = value;
            }
        }
        /// <summary>
        /// Gets or sets a value to resize the columns proportionally fit its content
        /// </summary>
        [DefaultValue(false)]
        [Browsable(false)]
        [Category("Appearance")]
        [Description("Resizes the columns to proportionally fit its content")]
        public bool AllowProportionalColumnSizing
        {
            get
            {
                return allowProportionalColumnSizing;
            }

            set
            {
                allowProportionalColumnSizing = value;
            }
        }

        /// <summary>
        /// Gets or sets a value to assign the placement of the SortIcon
        /// </summary>
        [DesignerSerializationVisibilityAttribute(DesignerSerializationVisibility.Hidden), Category("Appearance")]
        [Syncfusion.Documentation.DocumentationExclude()]
        [Description("Gets or sets a value to assign the placement of the SortIcon.")]
        [System.Xml.Serialization.XmlIgnore]
        public SortIconPlacement SortIconPlacement
        {
            get
            {
                return this.TableControl.SortIconPlacement;
            }
            set
            {
                if (this.TableControl.SortIconPlacement != value)
                    this.TableControl.SortIconPlacement = value;
            }
        }

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
                return Engine.BindToCurrencyManager;
            }

            set
            {
                Engine.BindToCurrencyManager = value;
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
                return Engine.CurrencyManager;
            }
        }

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
                return Engine.AllowSwapDataViewWithDataTableList;
            }

            set
            {
                Engine.AllowSwapDataViewWithDataTableList = value;
            }
        }

        /// <summary>
        /// Gets / sets if the default value for fields in the AddNewRecord should be shown when it is not in edit-mode. If 
        /// the ShowDefaultValuesInAddNewRecord is false then the default values will only be assigned when
        /// AddNewRecord.BeginEdit is called. Prior calls to AddNewRecord will return no value in that case.
        /// </summary>
        [DefaultValue(false)]
        [Description("Specifies whether the default value for fields in the AddNewRecord should be shown when it is not in edit-mode.")]
        [Category("Grouping Control")]
        public bool ShowDefaultValuesInAddNewRecord
        {
            get
            {
                return Engine.ShowDefaultValuesInAddNewRecord;
            }

            set
            {
                Engine.ShowDefaultValuesInAddNewRecord = value;
            }
        }

        /// <summary>
        /// Gets / sets if the engine should cache copies of the old values from a record in the record object.
        /// You can access these values with the Record.GetOldValue method. Setting this property will override
        /// the default value returned by <see cref="CacheRecordValues"/>.
        /// </summary>
        [Description("Specifies if the engine should cache copies of the old values from a record in the record object.")]
        [Category("Optimization")]
        public bool CacheRecordValues
        {
            get
            {
                return Engine.CacheRecordValues;
            }

            set
            {
                Engine.CacheRecordValues = value;
            }
        }

        /// <summary>
        /// To specify whether the Grid is to be Read-Only
        /// </summary>
        [Description("To specify whether the Grid is to be Read-Only")]
        [Category("Grouping Control")]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public bool BrowseOnly
        {
            get
            {
                return browseOnly;
            }

            set
            {
                if (browseOnly != value)
                {
                    browseOnly = value;
                    this.TableModel.ReadOnly = value;
                    this.TableDescriptor.AllowEdit = !value;
                }
            }
        }

        /// <summary>
        /// Determines if the <see cref="CacheRecordValues"/> has been modified from its
        /// default state.
        /// </summary>
        /// <returns>True if CacheRecordValues was manually modified</returns>
        public bool ShouldSerializeCacheRecordValues()
        {
            return Engine.ShouldSerializeCacheRecordValues();
        }

        /// <summary>
        /// Resets the <see cref="CacheRecordValues"/> back to its default value.
        /// </summary>
        public void ResetCacheRecordValues()
        {
            Engine.ResetCacheRecordValues();
        }

        /// <summary>
        /// Gets / sets if the engine should set <see cref="TableDirtyOnSourceListReset"/> to true when the data source
        /// raises a ListChanged event with ListChangedType.Reset notification. The default is false.
        /// </summary>
        [Description("Specifies if the engine should set TableDirty to true when the data source raises ListChangedType.Reset.")]
        [Category("Optimization")]
        public bool TableDirtyOnSourceListReset
        {
            get
            {
                return Engine.TableDirtyOnSourceListReset;
            }

            set
            {
                Engine.TableDirtyOnSourceListReset = value;
            }
        }

        /// <summary>
        /// Determines if the <see cref="TableDirtyOnSourceListReset"/> has been modified from its
        /// default state.
        /// </summary>
        /// <returns>True if TableDirtyOnSourceListReset was manually modified; </returns>
        public bool ShouldSerializeTableDirtyOnSourceListReset()
        {
            return Engine.ShouldSerializeTableDirtyOnSourceListReset();
        }

        /// <summary>
        /// Resets the <see cref="TableDirtyOnSourceListReset"/> back to its default value.
        /// </summary>
        public void ResetTableDirtyOnSourceListReset()
        {
            Engine.ResetTableDirtyOnSourceListReset();
        }

        /// <summary>
        /// This property affects the autopopulation of the RelationDescriptorCollection. <para/>
        /// It specifies if relations should be automatically generated when you assign a
        /// DataSource a DataTable with constraints or a DataSet with relations defined.
        /// Default is True.
        /// <para/>
        /// Version 4.4 also added support for automatically populated nested collections. If
        /// you upgrade from an earlier version and relied on the engine not populating nested collections
        /// from your strong typed collection you should set this property to be false.
        /// <para/>
        /// With nested collection you can also specify the level of recursion allowed when self-relations
        /// are detected. See the Engine.MaxNestedCollectionRecurseLevel property.
        /// </summary>
        [DefaultValue(true)]
        [Description("Specifies if relations should be automatically generated when a DataTable with constraints or a DataSet with relations is assigned as DataSource.")]
        [Category("Grouping Control")]
        public bool AutoPopulateRelations
        {
            get
            {
                return Engine.AutoPopulateRelations;
            }

            set
            {
                Engine.AutoPopulateRelations = value;
            }
        }

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
                return Engine.ShowNestedPropertiesFields;
            }

            set
            {
                Engine.ShowNestedPropertiesFields = value;
            }
        }

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
                return Engine.ShowRelationFields;
            }

            set
            {
                Engine.ShowRelationFields = value;
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
                return Engine.SourceListSet;
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
        [Category(@"Grouping Control")]
        public bool UseInvariantCulture
        {
            get
            {
                return Engine.UseInvariantCulture;
            }

            set
            {
                Engine.UseInvariantCulture = value;
            }
        }

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
                return Engine.Culture;
            }

            set
            {
                Engine.Culture = value;
            }
        }

        /// <summary>
        /// Determines whether <see cref="Culture"/> was modified.
        /// </summary>
        /// <returns>True if content is changed; False otherwise.</returns>
        public bool ShouldSerializeCultureInfo()
        {
            return Engine.ShouldSerializeCultureInfo();
        }

        /// <summary>
        /// Discards any changes for the <see cref="Culture"/> object.
        /// </summary>
        public void ResetCultureInfo()
        {
            Engine.ResetCultureInfo();
        }

        /// <override/>
        /// <summary>Specifies the site of the control.</summary>
        public override ISite Site
        {
            get
            {
                return base.Site;
            }

            set
            {
                base.Site = value;
            }
        }

        /// <summary>
        /// Occurs when a property is changed.
        /// </summary>
        [Description("Occurs when a property is changed."), Category("Property Changed")]
        public event DescriptorPropertyChangedEventHandler PropertyChanged;

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

            if (PropertyChanged != null)
            {
                PropertyChanged(this, e);
            }

            this.pdcCache = null;

            if (gridHelper != null)
            {
                gridHelper.OnGridPropertyChanged(this, e);
            }
        }

        /// <summary>
        /// Occurs before a property is changed.
        /// </summary>
        [Description("Occurs before a property is changed."), Category("Property Changed")]
        public event DescriptorPropertyChangedEventHandler PropertyChanging;

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

            pdcCache = null;

            if (PropertyChanging != null)
            {
                PropertyChanging(this, e);
            }
        }

        /// <summary>
        /// Initializes this object and copies properties from another object. <see cref="PropertyChanging"/>
        /// and <see cref="PropertyChanged"/> events are raised for every property that is modified. If both
        /// objects are equal, no events are raised.
        /// </summary>
        /// <param name="other">The source object.</param>
        public void InitializeFrom(GridGroupingControl other)
        {
            BorderStyle = other.BorderStyle;
            ThemesEnabled = other.ThemesEnabled;
            ////this.navigationBarWidth = other.NavigationBarWidth;
            ShowGroupDropArea = other.ShowGroupDropArea;
            ShowNavigationBar = other.ShowNavigationBar;
            AllowOptimizeLoadTime = other.AllowOptimizeLoadTime;
            if (!Font.Equals(other.Font))
            {
                Font = other.Font;
            }

            BackColor = other.BackColor;
            ForeColor = other.ForeColor;
            UseCustomUpdateOnListChanged = other.UseCustomUpdateOnListChanged;

            if (engine != other.engine)
            {
                Engine.InitializeFrom(other.engine);
            }

            ////            TableControl.synchronizeGridShouldInvalidate = true;
            ////            TableControl.synchronizeGridShouldnextUpdateScrollBars = true;
            ////            TableControl.synchronizeGridShouldUpdateColumnWidths = true;
            ////            Table.InvalidateSummariesTopDown();
            ////            Table.InvalidateCounterTopDown(true);
            ////            TableControl.
        }

        /// <summary>
        /// A method to improve Grouping performance with IList datasource.
        /// </summary>
        /// <param name="src">The source object.</param>
        /// <param name="e">A <see cref="ListChangedEventArgs" /> that contains the event data.</param>
        public void OptimizeIListGroupingPeformance(object src,ListChangedEventArgs e)
        {
            if (src is IList && this.Engine.OptimizeIListGroupingPerformance)
            {
                IList list = (IList)src;
                PropertyDescriptorCollection propCol = ListUtil.GetItemProperties(list);
                DataTable tbl = (DataTable)DataSource;
                DataRow row;
                
                switch (e.ListChangedType)
                {
                    case ListChangedType.ItemAdded:
                        row = tbl.NewRow();
                        foreach (DataColumn col in tbl.Columns)
                        {
                            object obj = propCol[col.ColumnName].GetValue(list[e.NewIndex]);
                            if (obj == null)
                                row[col.ColumnName] = DBNull.Value;
                            else
                                row[col.ColumnName] = obj;
                        }
                        if (tbl.Rows.Count <= e.NewIndex)
                            tbl.Rows.Add(row);
                        else
                            tbl.Rows.InsertAt(row, e.NewIndex);
                        break;
                    case ListChangedType.ItemChanged:
                        row = tbl.Rows[e.NewIndex];
                        foreach (DataColumn col in tbl.Columns)
                        {
                            object obj = propCol[col.ColumnName].GetValue(list[e.NewIndex]);
                            if (obj == null)
                                row[col.ColumnName] = DBNull.Value;
                            else
                                row[col.ColumnName] = obj;
                        }
                        tbl.Rows.RemoveAt(e.NewIndex);
                        tbl.Rows.InsertAt(row, e.NewIndex);
                        break;
                    case ListChangedType.ItemDeleted:
                        row = tbl.Rows[e.NewIndex];
                        foreach (DataColumn col in tbl.Columns)
                        {
                            object obj = propCol[col.ColumnName].GetValue(list[e.NewIndex]);
                            if (obj == null)
                                row[col.ColumnName] = DBNull.Value;
                            else
                                row[col.ColumnName] = obj;
                        }
                        tbl.Rows.RemoveAt(e.NewIndex);
                        break;
                    case ListChangedType.Reset:
                        tbl.Reset();
                        break;
                }
            }
        }

        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.Container components = null;

        internal bool InDesigner = false;

        /// <summary>
        /// The panel which hosts the <see cref="GridGroupDropArea"/> control.
        /// </summary>
        [System.Xml.Serialization.XmlIgnore]
        [DesignerSerializationVisibilityAttribute(DesignerSerializationVisibility.Hidden)]
        [Browsable(false)]
        public Panel GroupDropPanel
        {
            get
            {
                return this.groupDropPanel;
            }
        }

        /// <summary>
        /// The panel which hosts the <see cref="TableControl"/> control.
        /// </summary>
        [System.Xml.Serialization.XmlIgnore]
        [DesignerSerializationVisibilityAttribute(DesignerSerializationVisibility.Hidden)]
        [Browsable(false)]
        public Panel GridTablePanel
        {
            get
            {
                return this.gridTablePanel;
            }
        }

        /// <summary>
        /// The vertical splitter which divides the <see cref="GroupDropPanel"/> and <see cref="GridTablePanel"/>.
        /// </summary>
        [System.Xml.Serialization.XmlIgnore]
        [DesignerSerializationVisibilityAttribute(DesignerSerializationVisibility.Hidden)]
        [Browsable(false)]
        public Splitter Splitter
        {
            get
            {
                return this.splitter1;
            }
        }

        /// <overload>
        /// Initializes a new <see cref="GridGroupingControl"/>.
        /// </overload>
        /// <summary>
        /// Initializes the <see cref="GridGroupingControl"/> and creates a new
        /// empty engine object.
        /// </summary>
        public GridGroupingControl()
            : this(null)
        {
        }
        #region For Touch

        bool _touchMode = false;

        /// <summary>
        /// gets or sets the touchmode
        /// </summary>
        [DesignerSerializationVisibilityAttribute(DesignerSerializationVisibility.Hidden)]
        public virtual bool EnableTouchMode
        {
            get
            {
                return _touchMode;
            }
            set
            {
                if (_touchMode != value)
                {
                    _touchMode = value;
                    if (_touchMode)
                    {
                        ApplyScaleToControl(1.5F);
                    }
                    else
                    {
                        ApplyScaleToControl(1);
                    }

                }
            }
        }
        private bool ShouldSerializeTouchMode()
        {
            return EnableTouchMode != false;
        }

        /// <summary></summary>
        private void ResetTouchMode()
        {
            EnableTouchMode = false;
        }
        /// <summary>
        /// applies the scaling
        /// </summary>
        /// <param name="scaleFactor"></param>
        public void ApplyScaleToControl(float sf)
        {
            int oldSize = 0;
            this.TableModel.BeginUpdate();
            this.SuspendLayout();
            oldSize = this.TableModel.RowHeights[TableModel.RowCount];
            if (sf == 1.5)
            {
                //Code customization for handling individual column resizing.  
                this.TableModel.RowHeights[TableModel.RowCount] = this.TableModel.RowHeights[TableModel.RowCount] + 5;
                if (TableModel.RowCount > 1)
                {
                    if (oldSize == this.TableModel.RowHeights[TableModel.RowCount - 1])
                    {
                        if (this.UseOldListChangedHandler)
                            this.Table.DefaultRecordRowHeight = this.TableModel.RowHeights[TableModel.RowCount - 1] + 5;
                        else
                        {
                            for (int i = 0; i < this.TableModel.RowCount - 1; i++)
                            {
                                if (this.TableModel.RowHeights[i] != 0)
                                    this.TableModel.RowHeights[i] += 5;
                            }
                        }
                    }
                    for (int i = 0; i < this.TableModel.ColCount; i++)
                    {
                        if (this.TableModel.ColWidths[i] != 0)
                            this.TableModel.ColWidths[i] += 15;
                    }
                }
                //for Caption Row Height
                if (this.TableDescriptor.GroupedColumns.Count > 0)
                {
                    this.Table.DefaultCaptionRowHeight = 27;
                }
                //for expanding the GroupDopArea
                if (hGroupDropArea)
                {
                    for (int i = 0; i <= this.TableDescriptor.GroupedColumns.Count; i++)
                    {
                        this.groupDropArea1.Height += 7;
                        this.groupDropPanel.Height += 7;
                    }
                }
                else if (GroupDropAreaAlignment == GridGroupDropAreaAlignment.Top || GroupDropAreaAlignment == GridGroupDropAreaAlignment.Bottom)
                {
                    this.groupDropArea1.Size = new System.Drawing.Size(tableControl1.Width + 5, groupDropArea1.Model.RowHeights.GetTotal(1, 2) + 1);
                    this.groupDropPanel.Size = new System.Drawing.Size(tableControl1.Width + 5, this.groupDropArea1.Height + groupDropArea1.Model.RowHeights[1] + 1);
                }
                else if (GroupDropAreaAlignment == GridGroupDropAreaAlignment.Left)
                {
                    this.groupDropArea1.Size = new System.Drawing.Size(groupDropArea1.Model.ColWidths.GetTotal(1, 2), tableControl1.Height + 1);
                    this.groupDropPanel.Size = new System.Drawing.Size(this.groupDropArea1.Width + groupDropArea1.Model.ColWidths[1], tableControl1.Height + 1);
                }
                
            }
            else
            {
                if (this.TableModel.RowHeights[TableModel.RowCount] - 5 != 0)
                    this.TableModel.RowHeights[TableModel.RowCount] = this.TableModel.RowHeights[TableModel.RowCount] - 5;
                if (TableModel.RowCount > 1)
                {
                    if (oldSize == this.TableModel.RowHeights[TableModel.RowCount - 1])
                    {
                        if (this.UseOldListChangedHandler)
                            this.Table.DefaultRecordRowHeight = this.TableModel.RowHeights[TableModel.RowCount - 1] + 5;
                        else
                        {
                            for (int i = 0; i < this.TableModel.RowCount - 1; i++)
                            {
                                if (this.TableModel.RowHeights[i] != 0)
                                    this.TableModel.RowHeights[i] -= 5;
                            }
                        }
                    }
                    for (int i = 0; i < this.TableModel.ColCount; i++)
                    {
                        if (this.TableModel.ColWidths[i] != 0)
                            this.TableModel.ColWidths[i] -= 15;
                    }
                }
                //for re-sizing Caption Row height.
                if (this.TableDescriptor.GroupedColumns.Count > 0)
                {
                    this.Table.DefaultCaptionRowHeight = 22;
                }
                // for resizing the GroupDropArea to normal size.
                if (hGroupDropArea)
                {
                    for (int i = 0; i <= this.TableDescriptor.GroupedColumns.Count; i++)
                    {
                        this.groupDropArea1.Height -= 7;
                        this.groupDropPanel.Height -= 7;
                    }
                }
                else if (GroupDropAreaAlignment == GridGroupDropAreaAlignment.Top || GroupDropAreaAlignment == GridGroupDropAreaAlignment.Bottom)
                {
                    this.groupDropArea1.Size = new System.Drawing.Size(tableControl1.Width, groupDropArea1.Model.RowHeights.GetTotal(1, 2));
                    this.groupDropPanel.Size = new System.Drawing.Size(tableControl1.Width, this.groupDropArea1.Height + groupDropArea1.Model.RowHeights[1]);
                }
                else if (GroupDropAreaAlignment == GridGroupDropAreaAlignment.Left)
                {
                    this.groupDropArea1.Size = new System.Drawing.Size(groupDropArea1.Model.ColWidths.GetTotal(1, 2), tableControl1.Height);
                    this.groupDropPanel.Size = new System.Drawing.Size(this.groupDropArea1.Width + groupDropArea1.Model.ColWidths[1], tableControl1.Height);
                }
            }
            this.ResumeLayout();
            this.Invalidate();
            this.TableModel.EndUpdate();
        }
        #endregion
        GridTableModel savedModel = null;
        
        /// <summary>
        /// Initializes a new <see cref="GridGroupingControl"/> and attaches it to the specified
        /// engine object.
        /// </summary>
        /// <param name="engine">The engine to be used by the GridGroupingControl.</param>
        public GridGroupingControl(GridEngine engine)
        {
            try
            {
                AppDomain.CurrentDomain.AssemblyResolve += new ResolveEventHandler(AssemblyInfo.AssemblyResolver);
                new Syncfusion.Core.Licensing.LicensedComponent(typeof(GridGroupingControl));
            }
            finally
            {
                AppDomain.CurrentDomain.AssemblyResolve -= new ResolveEventHandler(AssemblyInfo.AssemblyResolver);
            }

            sizedColumns = new Dictionary<string, int>();
            this.engine = engine;
            if (engine != null)
            {
                if (engine.ParentControl != null)
                {
                    savedModel = engine.ParentControl.TableModel;
                }

                engine.SetParentControl(this);
                engine.BindingContext = this.BindingContext;
                WireEngine();
            }

            base.BackColor = SystemColors.Window;

            //// This call is required by the Windows.Forms Form Designer.
            InitializeComponent();

            tableControl1.ScrollControlMouseDown += new CancelMouseEventHandler(tableControl1_ScrollControlMouseDown);
            tableControl1.KeyDown += new KeyEventHandler(tableControl1_KeyDown);
            savedModel = null;
            ////this.Font = GridTableCellStyleInfo.Default.Font.GdipFont;

            this.recordNavigationControl1.NavigationBarWidth += 40;  // make space for "of xxx"
            navigationBarWidth = this.recordNavigationControl1.NavigationBarWidth;
            this.recordNavigationControl1.ThemesEnabled = true;

            groupDropArea1.Initialize();
            this.groupDropArea1.Size = new System.Drawing.Size(tableControl1.Width, groupDropArea1.Model.RowHeights.GetTotal(1, 2));
            this.groupDropPanel.Size = new System.Drawing.Size(tableControl1.Width, this.groupDropArea1.Height + groupDropArea1.Model.RowHeights[1]);
            ////this.groupDropPanel.Anchor = AnchorStyles.Left|AnchorStyles.Right;
            ////this.groupDropArea1.Dock = DockStyle.Fill;
            groupDropArea1.Visible = true;

            this.recordNavigationControl1.ThemesEnabled = ThemesEnabled;
            this.tableControl1.ThemesEnabled = ThemesEnabled;
            this.ShowNavigationBar = false;

            this.tableControl1.Model.ActiveGridView = this.tableControl1;

            this.tableControl1.GridControlBaseEventsTarget = new GridControlBaseEventsTarget(tableControl1, this);

            this.recordNavigationControl1.PaneClosing += new SplitterPaneEventHandler(recordNavigationControl1_PaneClosing);
            this.recordNavigationControl1.PaneCreated += new SplitterPaneEventHandler(recordNavigationControl1_PaneCreated);

            RecordNavigationBar = this.recordNavigationControl1.NavigationBar;

            Application.Idle += new EventHandler(Application_Idle);
            gridHelper = CreateGridGroupingControlOptimizeListChanged();
        }

        private bool freezecaption = false;
        /// <summary>
        /// Gets or sets whether the caption row is frozen or not. It works only when ShowCaption property is set to True.
        /// </summary>
        [Browsable(true)]
        [Description("It enables to freeze the caption row. This freezes caption row only when ShowCaption property is set to True")]
        [Category("Appearance")]
        public bool FreezeCaption
        {
            get
            {
                return freezecaption;
            }
            set
            {
                freezecaption = value;
            }
        }

        /// <summary>
        /// Creates the grid grouping control optimize list changed.
        /// </summary>
        /// <returns>returns the instance of GridGroupingControlOptimizeListChanged</returns>
        /// <exclude/>
        protected virtual GridGroupingControlOptimizeListChanged CreateGridGroupingControlOptimizeListChanged()
        {
            return new GridGroupingControlOptimizeListChanged(this);
        }

        void tableControl1_KeyDown(object sender, KeyEventArgs e)
        {
            if (gridHelper != null)
            {
                gridHelper.OnGridTableControlTableControlKeyDown(this, e);
            }
        }

        void tableControl1_ScrollControlMouseDown(object sender, CancelMouseEventArgs e)
        {
            if (gridHelper != null)
            {
                gridHelper.OnGridTableControlScrollControlMouseDown(this, e);
            }
        }

        void Application_Idle(object sender, EventArgs e)
        {
            if (gridHelper != null)
            {
                gridHelper.OnGridApplicationIdle(sender, e);
            }
        }

        /// <override/>
        protected override void OnSizeChanged(EventArgs e)
        {
            foreach (Control c in this.GroupDropPanel.Controls)
            {
                c.Width = this.Width;
            }
            base.OnSizeChanged(e);
            if (gridHelper != null)
            {
                if (this.AllowProportionalColumnSizing)
                    this.SetColumnWidths();
                gridHelper.OnGridSizeChanged(this, e);
            }
        }

        bool stateOnly = false;

        /// <summary>
        /// Initializes the <see cref="GridGroupingControl"/> in a special mode where
        /// it is only used to hold state information. This is used for XML serialization
        /// and saving and restoring state of a GridGroupingControl.
        /// </summary>
        /// <param name="stateOnly">The state information.</param>
        public GridGroupingControl(bool stateOnly)
        {
            this.stateOnly = stateOnly;
        }

        /// <override/>
        protected override void OnCreateControl()
        {
            if (stateOnly)
            {
                throw new InvalidOperationException("State only control does not support creating handle.");
            }

            base.OnCreateControl();
        }

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                if (WF_timer != null)
                {
                    ResetWF_Timer();
                }
                if (Sys_timer != null)
                {
                    ResetSys_Timer();
                }
                if(idleHooked)
                {
                    ResetIdle();
                }
                UnwireTable();
                UnwireTableDescriptor();
                UnwireEngine();
                UnwireRecordNavigationBar();
                recordNavigationBar = null;

                if (parentForm != null)
                {
                    parentForm.HandleCreated -= new EventHandler(parentForm_HandleCreated);
                    parentForm.Load -= new EventHandler(f_Load);
                    parentForm = null;
                }

                if (this.recordNavigationControl1 != null)
                {
                    ////this.recordNavigationControl1.Controls.Clear();
                    this.recordNavigationControl1.Dispose();
                    this.recordNavigationControl1 = null;
                }

                if (this.tableControl1 != null)
                {
                    tableControl1.KeyDown -= new KeyEventHandler(tableControl1_KeyDown);
                    tableControl1.ScrollControlMouseDown -= new CancelMouseEventHandler(tableControl1_ScrollControlMouseDown);

                    this.TableModel.Dispose();

                    ////this.tableControl1.Controls.Clear();
                    this.tableControl1.Dispose();
                    this.tableControl1 = null;
                }

                if (this.groupDropArea1 != null)
                {
                    ////this.groupDropArea1.Controls.Clear();
                    this.groupDropArea1.Dispose();
                    this.groupDropArea1 = null;
                }

                if (this.gridTablePanel != null)
                {
                    ////this.gridTablePanel.Controls.Clear();
                    this.gridTablePanel.Dispose();
                    this.gridTablePanel = null;
                }

                if (this.groupDropPanel != null)
                {
                    ////this.groupDropPanel.Controls.Clear();
                    this.groupDropPanel.Dispose();
                    this.groupDropPanel = null;
                }

                if (this.splitter1 != null)
                {
                    ////this.splitter1.Controls.Clear();
                    this.splitter1.Dispose();
                    this.splitter1 = null;
                }

                if (this.engine != null)
                {
                    this.engine.Dispose();
                    this.engine = null;
                }

                if (this._themedDrawing != null)
                {
                    this._themedDrawing.Dispose();
                    this._themedDrawing = null;
                }

                if (this.recordNavigationControl1 != null)
                {
                    this.recordNavigationControl1.Dispose();
                    this.recordNavigationControl1 = null;
                }

                if (this.recordNavigationControl1 != null)
                {
                    this.recordNavigationControl1.Dispose();
                    this.recordNavigationControl1 = null;
                }

                tdPropertiesCache.Clear();
                pdcCache = null;
                tableEventsTarget = null;

                if (components != null)
                {
                    components.Dispose();
                }
            }

            base.Dispose(disposing);
        }

        /// <summary>
        /// Virtual method to create a <see cref="TableControl"/>.
        /// </summary>
        /// <param name="model">The <see cref="TableModel"/>.</param>
        /// <returns>The table control.</returns>
        public virtual GridTableControl CreateTableControl(GridTableModel model)
        {
            return Engine.CreateTableControl(model);
        }

        /// <summary>
        /// Virtual method to create the <see cref="TableModel"/>.
        /// </summary>
        /// <returns>The table model used by the <see cref="TableControl"/>.</returns>
        public virtual GridTableModel CreateTableModel()
        {
            return Engine.CreateTableModel();
        }

        /// <summary>
        /// Virtual method to create a <see cref="GridNestedTableControl"/>.
        /// </summary>
        /// <param name="relatedTableModel">The <see cref="TableModel"/>.</param>
        /// <param name="parentGrid">The parent grid.</param>
        /// <param name="parentRenderer">The GridNestedTableControlCellRenderer.</param>
        /// <returns>The table control.</returns>
        public virtual GridNestedTableControl CreateNestedTableControl(GridTableModel relatedTableModel, GridTableControl parentGrid, GridNestedTableControlCellRenderer parentRenderer)
        {
            return Engine.CreateNestedTableControl(relatedTableModel, parentGrid, parentRenderer);
        }

        /// <summary>
        /// Virtual method to create the <see cref="RecordNavigationControl"/>.
        /// </summary>
        /// <returns>The RecordNavigationControl.</returns>
        public virtual RecordNavigationControl CreateRecordNavigationControl()
        {
            return Engine.CreateRecordNavigationControl();
        }

        /// <summary>
        /// Virtual method to create the <see cref="GridGroupDropArea"/>.
        /// </summary>
        /// <returns>returns the GroupDropArea.</returns>
        public virtual GridGroupDropArea CreateGroupDropArea(GridTableControl tableControl, GridGroupDropAreaModel groupDropAreaModel)
        {
            return Engine.CreateGroupDropArea(tableControl, groupDropAreaModel);
        }

        #region Component Designer generated code
        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            components = new System.ComponentModel.Container();
            this.recordNavigationControl1 = CreateRecordNavigationControl();
            this.tableControl1 = CreateTableControl(savedModel != null ? savedModel : CreateTableModel());
            this.tableControl1.groupingControl = this;
            this.gridTablePanel = new System.Windows.Forms.Panel();
            this.groupDropPanel = new System.Windows.Forms.Panel();
            this.groupDropArea1 = CreateGroupDropArea(this.tableControl1, this.tableControl1.Model.GroupDropAreaModel);
            this.splitter1 = new System.Windows.Forms.Splitter();
            this.recordNavigationControl1.SuspendLayout();
            this.gridTablePanel.SuspendLayout();
            this.groupDropPanel.SuspendLayout();
            this.SuspendLayout();

            captionGrid.HScrollBehavior = GridScrollbarMode.Disabled;
            captionGrid.VScrollBehavior = GridScrollbarMode.Disabled;
            captionGrid.VScroll = false;
            captionGrid.RowCount = 1;
            captionGrid.ColCount = 1;
            captionGrid.ThemesEnabled = true;
            captionGrid.BorderStyle = System.Windows.Forms.BorderStyle.None;
            captionGrid.ShowColumnHeaders = false;
            captionGrid.ShowRowHeaders = false;          
            captionGrid.TabStop = false;
            captionGrid.Visible = false;
            this.captionGrid.CellModels.Add("GroupCaptionCell", new GridTableHeaderCellModel(captionGrid.Model));
            captionGrid[1, 1].CellType = "GroupCaptionCell";            
            captionGrid[1, 1].Enabled = false;
            captionGrid[1, 1].TextColor = Color.Black;
            captionGrid[1, 1].Font.Size = 9f;
            captionGrid.Location = new System.Drawing.Point(0, 0);
            captionGrid[1, 1].VerticalAlignment = GridVerticalAlignment.Middle;
                        
            this.gridTablePanel.Controls.Add(captionGrid);

            ////
            //// recordNavigationControl1
            ////
            if (this.showNavigationBar)
            {
                this.recordNavigationControl1.Controls.Add(this.tableControl1);
            }

            this.recordNavigationControl1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.recordNavigationControl1.Font = new System.Drawing.Font("Microsoft Sans Serif", 10F, System.Drawing.FontStyle.Bold);
            this.recordNavigationControl1.Location = new System.Drawing.Point(0, 0);
            this.recordNavigationControl1.MaxLabel = SR.GetString(SR.RecordNavigatorOF) + " 0";
            this.recordNavigationControl1.MaxRecord = 0;
            this.recordNavigationControl1.Name = "recordNavigationControl1";
            this.recordNavigationControl1.ShowToolTips = false;
            this.recordNavigationControl1.Size = new System.Drawing.Size(688, 318);
            this.recordNavigationControl1.SplitBars = Syncfusion.Windows.Forms.DynamicSplitBars.None;
            this.recordNavigationControl1.TabStop = false;
            this.recordNavigationControl1.Text = "recordNavigationControl1";
            this.recordNavigationControl1.ThemesEnabled = false;
            this.recordNavigationControl1.Visible = this.showNavigationBar;
            this.recordNavigationControl1.BorderStyle = BorderStyle.None;
            this.recordNavigationControl1.GridOfficeScrollBars = GridOfficeScrollBars;
            this.recordNavigationControl1.Office2010ScrollBarsColorScheme = Office2010ScrollBarsColorScheme;
            this.recordNavigationControl1.Office2007ScrollBars = Office2007ScrollBars;
            this.recordNavigationControl1.Office2007ScrollBarsColorScheme = Office2007ScrollBarsColorScheme;
            ////
            //// tableControl1
            ////
            if (this.showNavigationBar)
            {
                this.tableControl1.FillSplitterPane = true;
            }
            else
            {
                this.tableControl1.FillSplitterPane = false;
                this.tableControl1.Dock = DockStyle.Fill;
                this.tableControl1.VScrollBehavior = GridScrollbarMode.Automatic | GridScrollbarMode.AutoScroll;
                this.tableControl1.HScrollBehavior = GridScrollbarMode.Automatic | GridScrollbarMode.AutoScroll;
            }

            this.tableControl1.Location = new System.Drawing.Point(0, 0);
            this.tableControl1.Name = "tableControl1";
            this.tableControl1.Text = "tableControl1";
            this.tableControl1.TabStop = true;
            this.tableControl1.TabStop = true;
            this.tableControl1.HorizontalScrollTips = false;
            this.tableControl1.VerticalScrollTips = false;
            this.tableControl1.HorizontalThumbTrack = true;
            this.tableControl1.VerticalThumbTrack = true;
            this.tableControl1.BorderStyle = BorderStyle.None;
            this.tableControl1.Office2007ScrollBars = Office2007ScrollBars;
            this.tableControl1.Office2007ScrollBarsColorScheme = Office2007ScrollBarsColorScheme;
            this.tableControl1.GridOfficeScrollBars = GridOfficeScrollBars;
            this.tableControl1.Office2010ScrollBarsColorScheme = Office2010ScrollBarsColorScheme;
            ////
            ////
            //// gridTablePanel
            ////
            this.gridTablePanel.Controls.Add(this.recordNavigationControl1);
            if (!this.showNavigationBar)
            {
                this.gridTablePanel.Controls.Add(this.tableControl1);
            }

            this.gridTablePanel.Dock = System.Windows.Forms.DockStyle.Fill;
            this.gridTablePanel.Location = new System.Drawing.Point(0, 35);
            this.gridTablePanel.Name = "gridTablePanel";
            this.gridTablePanel.Size = new System.Drawing.Size(688, 318);
            this.gridTablePanel.TabIndex = 1;
            this.gridTablePanel.TabStop = false;
            this.GridTablePanel.BorderStyle = BorderStyle.None;
            ////
            //// groupDropPanel
            ////
            this.groupDropPanel.Controls.Add(this.groupDropArea1);
            this.groupDropPanel.Dock = System.Windows.Forms.DockStyle.Top;
            this.groupDropPanel.Location = new System.Drawing.Point(0, 0);
            this.groupDropPanel.Name = "groupDropPanel";
            this.groupDropPanel.Size = new System.Drawing.Size(688, 25);
            this.groupDropPanel.TabIndex = 0;
            this.groupDropPanel.TabStop = false;
            this.groupDropPanel.BackColor = SystemColors.ControlDark;
            this.groupDropPanel.Visible = this.showGroupDropArea;
            this.groupDropPanel.BorderStyle = BorderStyle.None;
            ////
            //// groupDropArea1
            ////
            this.groupDropArea1.Location = new System.Drawing.Point(0, 0);
            this.groupDropArea1.Name = "groupDropArea1";
            this.groupDropArea1.TabIndex = 0;
            this.groupDropArea1.ThemesEnabled = true;
            this.groupDropArea1.TabStop = false;
            this.groupDropArea1.BorderStyle = BorderStyle.None;
            ////
            //// splitter1
            ////
            this.splitter1.Dock = System.Windows.Forms.DockStyle.Top;
            this.splitter1.Location = new System.Drawing.Point(0, 32);
            this.splitter1.Name = "splitter1";
            this.splitter1.Size = new System.Drawing.Size(688, 3);
            this.splitter1.TabIndex = 0;
            this.splitter1.TabStop = false;
            this.splitter1.BackColor = SystemColors.ControlDark;
            this.groupDropPanel.Visible = this.showGroupDropArea;

            this.Controls.Add(this.gridTablePanel);
            if (this.showGroupDropArea)
            {
                this.Controls.Add(this.splitter1);
                this.Controls.Add(this.groupDropPanel);
            }
            this.recordNavigationControl1.ResumeLayout(false);
            this.gridTablePanel.ResumeLayout(false);
            this.groupDropPanel.ResumeLayout(false);
            this.ResumeLayout(false);
        }
        #endregion

        /// <summary>
        /// Gets or initializes the engine object which maintains all TableDescriptors and tables
        /// to be displayed in the grid control.
        /// </summary>
        /// <remarks>
        /// When you assign a GridEngine object using this property, the existing engine
        /// object is not replaced. Instead all properties and collections are copied
        /// from the assigned engine object using the <see cref="GridEngine.InitializeFrom"/> method.
        /// <para/>
        /// <see cref="GridEngine.InitializeFrom"/> initializes the object and copies properties from another object.
        /// <see cref="PropertyChanging"/>
        /// and <see cref="PropertyChanged"/> events are raised for every property that is modified. If both
        /// objects are equal, no events are raised.
        /// </remarks>
        [System.Xml.Serialization.XmlIgnore]
        [DesignerSerializationVisibilityAttribute(DesignerSerializationVisibility.Hidden)]
        [Browsable(false)]
        public GridEngine Engine
        {
            get
            {
                if (initEngine != null)
                {
                    return initEngine;
                }

                if (engine == null)
                {
                    engine = CreateEngine();
                    engine.SetParentControl(this);
                    engine.BindingContext = this.BindingContext;
                    engine.ForwardTableEvents = this;
                    WireEngine();
                }

                return engine;
            }

            set
            {
                Engine.InitializeFrom(value);
            }
        }

        /// <summary>
        /// Override this method if you would like to have a derived engine object. The method is called
        /// to create a default engine object when no engine objects were manually
        /// instantiated. See also the discussion in <see cref="GridEngineFactory"/>.
        /// </summary>
        /// <returns>
        /// If the CreateEngine method is not overriden, a new <see cref="GridEngine"/>
        /// object is returned, otherwise an object derived from GridEngine might be returned.
        /// </returns>
        public virtual GridEngine CreateEngine()
        {
            return GridEngineFactory.CreateEngine();
        }

        /// <override/>
        /// <summary>Forces the control to invalidate its client area and immediately redraws itself and any child controls.</summary>
        public override void Refresh()
        {
            if (gridHelper != null)
            {
                gridHelper.isInvalidated = true;
            }

            Engine.BumpVersion();
            this.Engine.TableDescriptor.EnableOneTimePopulate();
            this.Engine.GetSourceList();
            this.TableControl.ViewLayout.Reset();
            this.TableControl.synchronizeGridShouldUpdateColumnWidths = true;
            this.TableControl.synchronizeGridShouldnextUpdateScrollBars = true;
            this.GridGroupDropArea.Invalidate();
            this.TableControl.Invalidate();
            this.TableControl.SynchronizeGridWithEngine();
            base.Refresh();
        }

        /// <override/>
        protected override void OnGotFocus(EventArgs e)
        {
            ////if (this.tableControl1.HasControlFocus)
            ////    this.tableControl1.Focus();
            base.OnGotFocus(e);
        }

        bool allowSetCurrentRecordOnFocus = false;

        /// <summary>
        /// Gets or sets whether the grid should move the current cell to the first
        /// visible record when the focus is moved to the grid.
        /// </summary>
        [Category("Grouping Control")]
        [DefaultValue(false)]
        [Browsable(true)]
        [Description("Specifies whether the grid should move the current cell to the first visible record when the focus is moved to the grid")]
        public bool AllowSetCurrentRecordOnFocus
        {
            get { return allowSetCurrentRecordOnFocus; }
            set { allowSetCurrentRecordOnFocus = value; }
        }
        /// <summary>
        /// Gets or sets the <see cref="GridBorderStyle"/> value to be used as default for cell borders.
        /// </summary>
        [Browsable(true), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        [Description("Specifies the border style to be used as default for cell borders.")]
        [RefreshProperties(RefreshProperties.Repaint)]
        [Category("Grouping Control")]
        [DefaultValue(GridBorderStyle.Dotted)]
        public GridBorderStyle DefaultGridBorderStyle
        {
            get
            {
                return TableModel.Options.DefaultGridBorderStyle;
            }

            set
            {
                TableModel.Options.DefaultGridBorderStyle = value;
            }
        }
        /// <copyfrom cref="GridModelOptions.ActivateCurrentCellBehavior"/><summary>Specifies current cell activation behavior when moving the current cell or clicking inside a cell.</summary>
        [Browsable(true),
        DefaultValue(GridCellActivateAction.ClickOnCell)]
        [DesignerSerializationVisibilityAttribute(DesignerSerializationVisibility.Hidden)]
        [Description("Specifies current cell activation behavior when moving the current cell or clicking inside a cell.")]
        [Category("Grouping Control")]
        public GridCellActivateAction ActivateCurrentCellBehavior
        {
            get
            {
                return TableModel.Options.ActivateCurrentCellBehavior;
            }

            set
            {
                TableModel.Options.ActivateCurrentCellBehavior = value;
            }
        }
        /// <copyfrom cref="GridModelOptions.AllowScrollCurrentCellInView"/><summary>Defines scroll cell in view behavior of the grid.</summary>
        [Browsable(true),
        Editor(typeof(Syncfusion.Windows.Forms.Design.EnumFlagsEditor), typeof(UITypeEditor)),
        DefaultValue(GridScrollCurrentCellReason.Any)]
        [DesignerSerializationVisibilityAttribute(DesignerSerializationVisibility.Hidden)]
        [Description("Defines scroll cell in view behavior of the grid.")]
        [Category("Scrolling")]
        public GridScrollCurrentCellReason AllowScrollCurrentCellInView
        {
            get
            {
                return TableModel.Options.AllowScrollCurrentCellInView;
            }

            set
            {
                TableModel.Options.AllowScrollCurrentCellInView = value;
            }
        }
        /// <copyfrom cref="GridModelOptions.AlphaBlendSelectionColor"/><summary>Specifies the color for alpha blended cell selections.</summary>
        [Browsable(true)]
        [DesignerSerializationVisibilityAttribute(DesignerSerializationVisibility.Hidden)]
        [Description("Specifies the color for alpha blended cell selections.")]
        [Category("Grouping Control")]
        [RefreshProperties(RefreshProperties.Repaint)]
        public Color AlphaBlendSelectionColor
        {
            get
            {
                return TableModel.Options.AlphaBlendSelectionColor;
            }

            set
            {
                TableModel.Options.AlphaBlendSelectionColor = value;
            }
        }
        /// <copyfrom cref="GridModelOptions.ClickedOnDisabledCellBehavior"/>
        /// <summary>
        /// Gets or sets Excel-like current cell behavior. When the user clicks on a cell out of a selected range for which .Enabled has been set to false.
        /// </summary>
        [Browsable(true),
        DefaultValue(GridClickedOnDisabledCellBehavior.Default)]
        [RefreshProperties(RefreshProperties.Repaint)]
        [DesignerSerializationVisibilityAttribute(DesignerSerializationVisibility.Hidden)]
        [Description("Defines Excel-like current cell behavior. When the user clicks on a cell out of a selected range for which .Enabled has been set to false.")]
        [Category("Grouping Control")]
        public GridClickedOnDisabledCellBehavior ClickedOnDisabledCellBehavior
        {
            get
            {
                return TableModel.Options.ClickedOnDisabledCellBehavior;
            }

            set
            {
                TableModel.Options.ClickedOnDisabledCellBehavior = value;
            }
        }
        /// <copyfrom cref="GridModelOptions.ShowCurrentCellBorderBehavior"/><summary>See <see cref="GridModelOptions.ShowCurrentCellBorderBehavior"/> in the GridModel class for information.</summary>
        [Browsable(true),
        DefaultValue(GridShowCurrentCellBorder.WhenGridActive)]
        [DesignerSerializationVisibilityAttribute(DesignerSerializationVisibility.Hidden)]
        [Description("Defines when to show current cell frame or border.")]
        [Category("Grouping Control")]
        public GridShowCurrentCellBorder ShowCurrentCellBorderBehavior
        {
            get
            {
                return TableModel.Options.ShowCurrentCellBorderBehavior;
            }

            set
            {
                TableModel.Options.ShowCurrentCellBorderBehavior = value;
            }
        }
        /// <summary>
        /// Gets or sets a value indicating whether to control the kind of textbox control that is created for TextBox cells. 
        /// In general the original text box behaves better than the richtext box with Hebrew and arabic languages.
        /// By default the grid uses the RichTextBox control for cell editing, but if you set
        /// UseRightToLeftCompatibleTextBox to true then the grid will do editing with original TextBox controls
        /// instead.
        /// </summary>
        [Browsable(true),
        DefaultValue(false)]
        [DesignerSerializationVisibilityAttribute(DesignerSerializationVisibility.Hidden)]
        [Description("Controls the kind of textbox control that is created for TextBox cells. In general the original text box behaves better than the default richtext box with Hebrew and arabic languages")]
        [Category("Grouping Control")]
        public bool UseRightToLeftCompatibleTextBox
        {
            get
            {
                return TableModel.Options.UseRightToLeftCompatibleTextBox;
            }

            set
            {
                TableModel.Options.UseRightToLeftCompatibleTextBox = value;
            }
        }
        /// <summary>
        /// Gets or sets a value indicating whether the column header for the current cell should be highlighted.
        /// </summary>        
        [Browsable(true),
        DefaultValue(false)]
        [DesignerSerializationVisibilityAttribute(DesignerSerializationVisibility.Hidden)]
        [Description("Specifies if the column header for the current cell should be highlighted.")]
        [Category("Grouping Control")]
        public bool MarkColHeader
        {
            get
            {
                return TableModel.Properties.MarkColHeader;
            }

            set
            {
                TableModel.Properties.MarkColHeader = value;
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether the row header for the current cell should be highlighted.
        /// </summary>       
        [Browsable(true),
        DefaultValue(false)]
        [DesignerSerializationVisibilityAttribute(DesignerSerializationVisibility.Hidden)]
         [Description("Specifies if the row header for the current cell should be highlighted.")]   
        [Category("Grouping Control")]
        public bool MarkRowHeader
        {
            get
            {
                return TableModel.Properties.MarkRowHeader;
            }

            set
            {
                TableModel.Properties.MarkRowHeader = value;
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether the grid should be display column headers.
        /// </summary>
        [Description("Specifies if column headers should be displayed or hidden.")]
        [Browsable(true), DefaultValue(true)]
        [DesignerSerializationVisibilityAttribute(DesignerSerializationVisibility.Hidden)]
        [RefreshProperties(RefreshProperties.Repaint)]
        [Category("Grouping Control")]
        public bool ShowColumnHeaders
        {
            get
            {
                if (this.TopLevelGroupOptions != null)
                    return this.TopLevelGroupOptions.ShowColumnHeaders;
                return TableModel.Properties.ColHeaders;
            }

            set
            {
                if (this.TopLevelGroupOptions != null)
                    this.TopLevelGroupOptions.ShowColumnHeaders = value;
                TableModel.Properties.ColHeaders = value;
            }
        }
        /// <summary>
        /// Gets or sets a value indicating whether horizontal lines should be displayed.
        /// </summary>
        [Description("Specifies if horizontal lines should be displayed.")]
        [Browsable(true), DefaultValue(true)]
        [DesignerSerializationVisibilityAttribute(DesignerSerializationVisibility.Hidden)]
        [RefreshProperties(RefreshProperties.Repaint)]
        [Category("Grouping Control")]
        public bool DisplayHorizontalLines
        {
            get
            {
                return TableModel.Properties.DisplayHorzLines;
            }

            set
            {
                TableModel.Properties.DisplayHorzLines = value;
            }
        }
        /// <summary>
        /// Gets or sets a value indicating whether vertical lines should be displayed.
        /// </summary>
        [Description("Specifies if vertical lines should be displayed.")]
        [Browsable(true), DefaultValue(true)]
        [RefreshProperties(RefreshProperties.Repaint)]
        [DesignerSerializationVisibilityAttribute(DesignerSerializationVisibility.Hidden)]
        [Category("Grouping Control")]
        public bool DisplayVerticalLines
        {
            get
            {
                return TableModel.Properties.DisplayVertLines;
            }

            set
            {
                TableModel.Properties.DisplayVertLines = value;
            }
        }
        /// <summary>
        /// Enable or Disable the Legacy styles in the Table Model
        /// Value should be false to apply ColorStyles
        /// </summary>
        [Description("Allow Legacy Styles to Enable or Disable")]
        [Browsable(true), DefaultValue(true)]
        [Category("Grouping Control")]
        public bool ApplyVisualStyles
        {
            get
            {
                return TableModel.EnableLegacyStyle;
            }
            set
            {
                if (TableModel.EnableLegacyStyle != value)
                {
                    TableModel.EnableLegacyStyle = value;
                }
            }
        }
        /// <summary>
        /// Gets or sets the color of grid lines.
        /// </summary>
        [Description("The color of grid lines.")]
        [RefreshProperties(RefreshProperties.Repaint)]
        [DesignerSerializationVisibilityAttribute(DesignerSerializationVisibility.Hidden)]
        [Category("Grouping Control")]
        public Color GridLineColor
        {
            get
            {
                return TableModel.Properties.GridLineColor;
            }

            set
            {
                TableModel.Properties.GridLineColor = value;
            }
        }
        /// <summary>
        /// Gets or sets a value indicating whether column headers should be printed when printing the grid.
        /// </summary>
        [Description("Specifies if column headers should be printed when printing the grid.")]
        [Browsable(true), DefaultValue(true), Category("Grouping Control")]
        [DesignerSerializationVisibilityAttribute(DesignerSerializationVisibility.Hidden)]
        public bool PrintColumnHeader
        {
            get
            {
                return TableModel.Properties.PrintColHeader;
            }

            set
            {
                TableModel.Properties.PrintColHeader = value;
            }
        }
        /// <summary>
        /// Gets or sets a value indicating whether the grid should draw horizontal lines when printing.
        /// </summary>
        [Description("Specifies if the grid should draw horizontal lines when printing.")]
        [Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public bool PrintHorizontalLines
        {
            get
            {
                return TableModel.Properties.PrintHorzLines;
            }

            set
            {
                TableModel.Properties.PrintHorzLines = value;
            }
        }
        /// <summary>
        /// Gets or sets a value indicating whether row headers should be printed when printing the grid.
        /// </summary>
        [Description("Specifies if row headers should be printed when printing the grid.")]
        [Browsable(true), DefaultValue(true), Category("Grouping Control")]
        [DesignerSerializationVisibilityAttribute(DesignerSerializationVisibility.Hidden)]
        public bool PrintRowHeader
        {
            get
            {
                return TableModel.Properties.PrintRowHeader;
            }

            set
            {
                TableModel.Properties.PrintRowHeader = value;
            }
        }
        /// <summary>
        /// Gets or sets a value indicating whether the grid should draw vertical lines when printing.
        /// </summary>
        [Description("Specifies if the grid should draw vertical lines when printing.")]
        [Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public bool PrintVerticalLines
        {
            get
            {
                return TableModel.Properties.PrintVertLines;
            }

            set
            {
               TableModel.Properties.PrintVertLines = value;
            }
        }
        /// <summary>
        /// Gets or sets a value indicating whether row headers should be displayed or hidden. (Might be better to use HideCols[0] = false) instead.
        /// </summary>
        [Description("Specifies if row headers should be displayed or hidden.")]
        [Browsable(true), DefaultValue(true), Category("Grouping Control")]
        [RefreshProperties(RefreshProperties.Repaint)]
        [DesignerSerializationVisibilityAttribute(DesignerSerializationVisibility.Hidden)]
        public bool ShowRowHeaders
        {
            get
            {
                return TableModel.Properties.RowHeaders;
            }

            set
            {
                TableModel.Properties.RowHeaders = value;
            }
        }
        /// <override/>
        protected override void OnEnter(EventArgs e)
        {
            base.OnEnter(e);

            if (this.Table.CurrentElement == null && AllowSetCurrentRecordOnFocus)
            {
                int n = 0;

                foreach (Element el in Table.NestedDisplayElements)
                {
                    if (el is GridRecordRow || el is Record)
                    {
                        Record.GetRecord(el).SetCurrent();
                        break;
                    }

                    if (n++ > 20)
                    {
                        break;
                    }
                }
            }

            this.TableControl.CurrentCell.Refresh();
            this.TableControl.Focus();
        }

        /// <override/>
        protected override void OnValidating(CancelEventArgs e)
        {
            base.OnValidating(e);

            //// Fixes issues with new records and related tables in MasterDetails scenario
            if (this.TableControl != null && this.TableControl.CurrentCell != null)
            {
                this.TableControl.CurrentCell.EndEdit();
            }

            if (Engine.BindToCurrencyManager && Engine.CurrencyManager != null)
            {
                Engine.CurrencyManager.Refresh();
            }
        }

        Form parentForm;

        /// <override/>
        protected override void InitLayout()
        {
            base.InitLayout();

            if (parentForm == null)
            {
                parentForm = this.FindForm(); // do not use FindFormHelper here. null is wanted for nested grids.
                if (parentForm != null)
                {
                    //// Reduce flickering of grid at startup - I actual want to hook up with the Load event
                    //// but I do also want to be that last one in the chain of that event (especially
                    //// after the default Form1_Load handler that is created at design time).
                    //// Hooking myself up in parentForm_HandleCreated helps with this.
                    parentForm.HandleCreated += new EventHandler(parentForm_HandleCreated);
                }
            }
        }

        /// <override/>
        protected override void OnHandleCreated(EventArgs e)
        {
            base.OnHandleCreated(e);
            handleDestroyed = false;

            if (parentForm == null)
            {
                parentForm = this.FindForm();
                if (parentForm != null)
                {
                    //// Reduce flickering of grid at startup
                    parentForm.Load += new EventHandler(f_Load);
                }
                else
                {
                    ApplyCachedSettings();
                }
            }
        }

        private bool handleDestroyed;

        /// <summary>
        /// Determines if window handle was destroyed
        /// </summary>
        [Browsable(false), XmlIgnore(), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public bool IsHandleDestroyed
        {
            get { return handleDestroyed; }
        }

        /// <override/>
        protected override void OnHandleDestroyed(EventArgs e)
        {
            handleDestroyed = true;
            base.OnHandleDestroyed(e);
        }

        private void parentForm_HandleCreated(object sender, EventArgs e)
        {
            //// This will happen after the Form ctor returns.
            ((Form)sender).HandleCreated -= new EventHandler(parentForm_HandleCreated);

            //// Now my Load event handler will be added after the default Form1_Load handler. Phew!
            ((Form)sender).Load += new EventHandler(f_Load);
        }

        private void f_Load(object sender, EventArgs e)
        {
            ((Form)sender).Load -= new EventHandler(f_Load);

            if (!this.DesignMode)
            {
                //// Reduce flickering of grid at startup.
                OptimizeLoadTime();
            }

            ApplyCachedSettings();
        }

        /// <summary>
        /// Delay certain settings, e.g. ShowToolTips - execute them either in HandleCreated event or shortly after
        /// </summary>
        void ApplyCachedSettings()
        {
            if (this.recordNavigationBar != null && recordNavigationControlShowToolTips != this.recordNavigationBar.ShowToolTips)
            {
                this.recordNavigationBar.ShowToolTips = recordNavigationControlShowToolTips;
            }
        }

        bool allowOptimizeLoadTime = true;

        /// <summary>
        /// Gets or sets whether the the grid should try and reduce flickering at startup. If
        /// set to true the grid will be rendered once into a offline bitmap before the form
        /// is shown for the first time. This offline rendering of the grid ensures that all required code is
        /// loaded into memory (jitted) and all grid data are initialized. Default is true.
        /// </summary>
        [Category(@"Optimization"),
        DefaultValue(true),
        Description(@"Reduce flickering of grid at startup by rendering grid into a bitmap before the parent form is shown for the first time.")]
        public bool AllowOptimizeLoadTime
        {
            get
            {
                return allowOptimizeLoadTime;
            }

            set
            {
                if (this.allowOptimizeLoadTime != value)
                {
                    OnPropertyChanging(new DescriptorPropertyChangedEventArgs("AllowOptimizeLoadTime"));
                    this.allowOptimizeLoadTime = value;
                    OnPropertyChanged(new DescriptorPropertyChangedEventArgs("AllowOptimizeLoadTime"));
                }
            }
        }

        private void OptimizeLoadTime()
        {
            if (!AllowOptimizeLoadTime)
            {
                return;
            }

            //// Reduce flickering of grid at startup.

            Syncfusion.Windows.Forms.Grid.Grouping.GridTableControl grid = TableControl;
            grid.Initialize();
            if (grid.Model.HasTable)
            {
                grid.Update();
                using (Bitmap bm = new Bitmap(Math.Max(100, grid.Width), Math.Max(100, grid.Height)))
                {
                    Graphics g = Graphics.FromImage(bm);
                    grid.DrawGrid(g, new Rectangle(Point.Empty, bm.Size), true, false);
                    g.Dispose();
                }
            }

            grid.synchronizeGridShouldScrollCurrentCell = true;
            grid.synchronizeGridShouldRestoreCurrentCell = true;
            ////    grid.synchronizeGridShouldnextUpdateScrollBars = true;
            grid.SynchronizeGridWithEngine();
        }

        Engine IEngineSource.GetEngine()
        {
            return Engine;
        }

        /// <override/>
        protected override void OnBindingContextChanged(EventArgs e)
        {
            if (engine != null)
            {
                engine.BindingContext = this.BindingContext;
            }

            base.OnBindingContextChanged(e);
        }

        void WireEngine()
        {
            if (engine != null)
            {
                engine.TableCreated += new EventHandler(engine_TableCreated);
                engine.TableDescriptorCreated += new EventHandler(engine_TableDescriptorCreated);
                engine.DataMemberChanged += new EventHandler(engine_DataMemberChanged);
                engine.DataSourceChanged += new EventHandler(engine_DataSourceChanged);

                engine.SourceListChanged += new EventHandler(engine_SourceListChanged);
                engine.PropertyChanging += new DescriptorPropertyChangedEventHandler(engine_PropertyChanging);
                engine.PropertyChanged += new DescriptorPropertyChangedEventHandler(engine_PropertyChanged);
                engine.QueryCustomSummary += new GridQueryCustomSummaryEventHandler(engine_QueryCustomSummary);
                engine.QueryRecordMeetsFilterCriteria += new QueryRecordMeetsFilterCriteriaEventHandler(engine_QueryRecordMeetsFilterCriteria);
                engine.QueryCellStyleInfo += new GridTableCellStyleInfoEventHandler(engine_QueryCellStyleInfo);
                engine.MarkResyncEvent += new CancelEventHandler(engine_MarkResyncEvent);

                engine.Appearance.Changed += new GridTableCellStyleInfoChangedEventHandler(Appearance_Changed);
                engine.Appearance.Changing += new GridTableCellStyleInfoChangedEventHandler(Appearance_Changing);
                engine.NestedTableGroupOptions.Changed += new Syncfusion.Styles.StyleChangedEventHandler(NestedTableGroupOptions_Changed);
                engine.NestedTableGroupOptions.Changing += new Syncfusion.Styles.StyleChangedEventHandler(NestedTableGroupOptions_Changing);
                engine.ChildGroupOptions.Changed += new Syncfusion.Styles.StyleChangedEventHandler(GroupOptions_Changed);
                engine.ChildGroupOptions.Changing += new Syncfusion.Styles.StyleChangedEventHandler(ChildGroupOptions_Changing);
                engine.PropertyTypeDefaultStyles.Changed += new Syncfusion.Collections.ListPropertyChangedEventHandler(PropertyTypeDefaultStyles_Changed);
                engine.PropertyTypeDefaultStyles.Changing += new Syncfusion.Collections.ListPropertyChangedEventHandler(PropertyTypeDefaultStyles_Changing);
                engine.TopLevelGroupOptions.Changed += new Syncfusion.Styles.StyleChangedEventHandler(TopLevelGroupOptions_Changed);
                engine.TopLevelGroupOptions.Changing += new Syncfusion.Styles.StyleChangedEventHandler(TopLevelGroupOptions_Changing);
                engine.Table.GroupExpanded += new GroupEventHandler(Table_GroupExpanded);
            }
        }

        void UnwireEngine()
        {
            if (engine != null)
            {
                engine.TableCreated -= new EventHandler(engine_TableCreated);
                engine.TableDescriptorCreated -= new EventHandler(engine_TableDescriptorCreated);
                engine.DataMemberChanged -= new EventHandler(engine_DataMemberChanged);
                engine.DataSourceChanged -= new EventHandler(engine_DataSourceChanged);

                engine.SourceListChanged -= new EventHandler(engine_SourceListChanged);
                engine.PropertyChanging -= new DescriptorPropertyChangedEventHandler(engine_PropertyChanging);
                engine.PropertyChanged -= new DescriptorPropertyChangedEventHandler(engine_PropertyChanged);
                engine.QueryCustomSummary -= new GridQueryCustomSummaryEventHandler(engine_QueryCustomSummary);
                engine.QueryRecordMeetsFilterCriteria -= new QueryRecordMeetsFilterCriteriaEventHandler(engine_QueryRecordMeetsFilterCriteria);
                engine.QueryCellStyleInfo -= new GridTableCellStyleInfoEventHandler(engine_QueryCellStyleInfo);
                engine.MarkResyncEvent -= new CancelEventHandler(engine_MarkResyncEvent);

                engine.Appearance.Changed -= new GridTableCellStyleInfoChangedEventHandler(Appearance_Changed);
                engine.Appearance.Changing -= new GridTableCellStyleInfoChangedEventHandler(Appearance_Changing);
                engine.NestedTableGroupOptions.Changed -= new Syncfusion.Styles.StyleChangedEventHandler(NestedTableGroupOptions_Changed);
                engine.NestedTableGroupOptions.Changing -= new Syncfusion.Styles.StyleChangedEventHandler(NestedTableGroupOptions_Changing);
                engine.ChildGroupOptions.Changed -= new Syncfusion.Styles.StyleChangedEventHandler(GroupOptions_Changed);
                engine.ChildGroupOptions.Changing -= new Syncfusion.Styles.StyleChangedEventHandler(ChildGroupOptions_Changing);
                engine.PropertyTypeDefaultStyles.Changed -= new Syncfusion.Collections.ListPropertyChangedEventHandler(PropertyTypeDefaultStyles_Changed);
                engine.PropertyTypeDefaultStyles.Changing -= new Syncfusion.Collections.ListPropertyChangedEventHandler(PropertyTypeDefaultStyles_Changing);
                engine.TopLevelGroupOptions.Changed -= new Syncfusion.Styles.StyleChangedEventHandler(TopLevelGroupOptions_Changed);
                engine.TopLevelGroupOptions.Changing -= new Syncfusion.Styles.StyleChangedEventHandler(TopLevelGroupOptions_Changing);
                engine.Table.GroupExpanded -= new GroupEventHandler(Table_GroupExpanded);
            }
        }

        private void engine_MarkResyncEvent(object sender, CancelEventArgs e)
        {
            if (TableControl != null)
            {
                TableControl.MarkResync(e.Cancel);
            }
        }

        private void engine_TableDescriptorCreated(object sender, EventArgs e)
        {
            WireTableDescriptor();
        }

        void WireTableDescriptor()
        {
            TableDescriptor.PropertyChanging += new DescriptorPropertyChangedEventHandler(TableDescriptor_PropertyChanging);
            TableDescriptor.PropertyChanged += new DescriptorPropertyChangedEventHandler(TableDescriptor_PropertyChanged);
            TableDescriptor.SortedColumns.Changed += new ListPropertyChangedEventHandler(SortedColumns_Changed);
        }

        void UnwireTableDescriptor()
        {
            TableDescriptor.PropertyChanging -= new DescriptorPropertyChangedEventHandler(TableDescriptor_PropertyChanging);
            TableDescriptor.PropertyChanged -= new DescriptorPropertyChangedEventHandler(TableDescriptor_PropertyChanged);
            TableDescriptor.SortedColumns.Changed -= new ListPropertyChangedEventHandler(SortedColumns_Changed);
        }

        void WireTable()
        {
            Table.CurrentRecordContextChange += new CurrentRecordContextChangeEventHandler(Table_CurrentRecordContextChange);
        }

        void UnwireTable()
        {
            if (this.engine != null && this.engine.ShouldSerializeTable())
            {
                Table.CurrentRecordContextChange -= new CurrentRecordContextChangeEventHandler(Table_CurrentRecordContextChange);
            }
        }

        private void engine_SourceListChanged(object sender, EventArgs e)
        {
            if (this.engine.HasSourceList())
            {
                this.TableControl.Table = Engine.Table;
                this.groupDropArea1.Model.Table = Engine.Table;
            }
            else
            {
                this.TableDescriptor.ResetItemProperties();
            }

            this.Invalidate();
            TableControl.synchronizeGridShouldInvalidate = true;
        }

        /// <override/>
        /// <summary>
        /// Returns a string containing the current object.
        /// </summary>
        /// <returns>
        /// A string containing the current object.
        /// </returns>
        public override string ToString()
        {
            IList list = engine != null ? engine.GetSourceList() as IList : null;
            int count = (list == null) ? -1 : list.Count;
            return String.Format("Engine: {0} Records.", count);
        }

        /// <summary>
        /// This property specifies if the engine should reset the TableDescriptor, Relations
        /// and clear out the SourceListSet when you set the Engine.DataSource = null. 
        /// </summary>
        [Category("Data"),
        DefaultValue(true),
        Description("This property specifies if the engine should reset the TableDescriptor and Relations when you set the Engine.DataSource = null. ")]
        public bool AllowResetTableDescriptorWhenDataSourceSetNull
        {
            get { return Engine.AllowResetTableDescriptorWhenDataSourceSetNull; }
            set { Engine.AllowResetTableDescriptorWhenDataSourceSetNull = value; }
        }

        /// <summary>
        ///   <para>Gets / sets the data source that the control is displaying data for.</para>
        /// </summary>
        [Description(@"Indicates the source of data for the DataGrid."),
        DefaultValue(null),
        Category(@"Data"),
        RefreshProperties(RefreshProperties.All),
        TypeConverter("System.Windows.Forms.Design.DataSourceConverter, System.Design")]
#if SyncfusionFramework2_0
        [AttributeProvider(typeof(IListSource))]
#endif
        public object DataSource
        {
            get
            {
                return Engine.DataSource;
            }

            set
            {
                if (initEngine != null)
                {
                    dataSource = value;
                }
                else
                {
                    DataSourceChangingEventArgs e = new DataSourceChangingEventArgs(value);
                    this.OnDataSourceChanging(e);
                    if (!e.Cancel)
                    {
                        Engine.DataSource = value;

                        this.OnDataSourceChanged();
                    }
                }
            }
        }

#if SyncfusionFramework4_0
        /// <summary>
        /// Gets or sets whether the bounded datasource is composed of dynamic objects or not. Applicable only for .NET Framework 4.0 and later.
        /// </summary>
        [DesignerSerializationVisibilityAttribute(DesignerSerializationVisibility.Hidden)]
        [Browsable(true)]
        [Description("Gets or sets whether the bounded datasource is composed of dynamic objects or not. Applicable only for .NET Framework 4.0 and later")]
        [Category("Grouping Control")]
        public bool IsDynamicData
        {
            get
            {
                return Engine.IsDynamicData;
            }
            set
            {
                Engine.IsDynamicData = value;
            }
        }
#endif
        /// <summary>
        /// Occurs immediately before the FieldChooser dialog is displayed.
        /// </summary>
        [Description("Occurs when the FieldChooser dialog is being shown"), Category("Field Chooser")]
        public event FieldChooserShowingEventHandler FieldChooserShowing;
        /// <summary>
        /// Raises the FieldChooserShowing event.
        /// </summary>
        /// <param name="e"> The event data </param>
        public virtual void OnFieldChooserShowing(FieldChooserShowingEventArgs e)
        {
            if (this.FieldChooserShowing != null)
            {
                this.FieldChooserShowing(this, e);
            }
        }
        /// <summary>
        /// Occurs after the FieldChooser dialog is displayed.
        /// </summary>
        [Description("Occurs after the FieldChooser dialog is displayed"), Category("Field Chooser")]
        public event FieldChooserShownEventHandler FieldChooserShown;
        /// <summary>
        /// Raises the FieldChooserShown event.
        /// </summary>
        /// <param name="e"> The event data. </param>
        public virtual void OnFieldChooserShown(FieldChooserShownEventArgs e)
        {
            if (this.FieldChooserShown != null)
            {
                this.FieldChooserShown(this, e);
            }
        }
        /// <summary>
        /// Occurs immediately before the FieldChooser dialog is closed.
        /// </summary>
        [Description("Occurs before the FieldChooser dialog is about to close"), Category("Field Chooser")]
        public event FieldChooserClosingEventHandler FieldChooserClosing;

        /// <summary>
        /// Raises the FieldChooserClosing event.
        /// </summary>
        /// <param name="e"> The event data. </param>
        public virtual void OnFieldChooserClosing(FieldChooserClosingEventArgs e)
        {
            if (this.FieldChooserClosing != null)
            {
                this.FieldChooserClosing(this, e);
            }
        }
        /// <summary>
        /// Occurs immediately after the FieldChooser dialog is closed.
        /// </summary>
        [Description("Occurs immediately after the FieldChooser dialog is closed"), Category("Field Chooser")]
        public event FieldChooserClosedEventHandler FieldChooserClosed;

        /// <summary>
        /// Raises the FieldChooserClosed event.
        /// </summary>
        /// <param name="e"> The event data. </param>
        public virtual void OnFieldChooserClosed(FieldChooserClosedEventArgs e)
        {
            if (this.FieldChooserClosed != null)
            {
                this.FieldChooserClosed(this, e);
            }
        }

        /// <summary>
        /// Provides Data for Cancellable Event
        /// </summary>
        public class DataSourceChangingEventArgs : CancelEventArgs
        {
            #region Fields
            private object _dataSource;
            #endregion

            #region Constructor
            /// <summary>
            /// Determine the datasource.
            /// </summary>
            public DataSourceChangingEventArgs()
                :base()
            {
            }
            /// <summary>
            /// Determine the datasource.
            /// </summary>
            /// <param name="bCancel"></param>
            public DataSourceChangingEventArgs(bool bCancel)
                : base(bCancel)
            {
            }
            /// <summary>
            /// 
            /// </summary>
            /// <param name="dataSource"></param>
            public DataSourceChangingEventArgs(object dataSource)
                : base()
            {
                this._dataSource = dataSource;
            }
            /// <summary>
            /// Determine the datasource events changing event.
            /// </summary>
            /// <param name="bCancel">Indicate the Cancel </param>
            /// <param name="dataSource">Datasource object</param>
            public DataSourceChangingEventArgs(bool bCancel, object dataSource)
                : base(bCancel)
            {
                this._dataSource = dataSource;
            }

            #endregion

            #region Public Properties

            /// <summary>
            /// Gets the DataSource
            /// </summary>
            public object DataSource
            {
                get
                {
                    return this._dataSource;
                }
            }

            #endregion
        }

        /// <summary>
        /// DataSourceChanging delegate
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        public delegate void DataSourceChangingEventHandler(object sender, DataSourceChangingEventArgs e);
        /// <summary>
        /// Occurs When the DataSource is about to Change.
        /// </summary>
        [Description("Occurs when the DataSource is about to Change"), Category("Data")]
        public event DataSourceChangingEventHandler DataSourceChanging;

        /// <summary>
        /// Occurs When the DataSource is to Changed. 
        /// </summary>
        [Description("Occurs when the DataSource is to Changed"), Category("Data")]
        public event EventHandler DataSourceChanged;

        /// <summary>
        /// DataSourceChanging Handler.
        /// </summary>
        /// <param name="e"></param>
        protected virtual void OnDataSourceChanging(DataSourceChangingEventArgs e)
        {
            if (this.DataSourceChanging != null)
            {
                this.DataSourceChanging(this, e);
            }
        }

        /// <summary>
        /// DataSourceChanged Handler.
        /// </summary>
        protected virtual void OnDataSourceChanged()
        {
            intialized = false;
            if (this.DataSourceChanged != null)
            {
                this.DataSourceChanged(this, EventArgs.Empty);
            }
        }

        /// <summary>
        /// Forces the Engine to Dispose.
        /// </summary>
        public void ForceEngineDispose()
        {
            this.Engine.Dispose();
            this.engine = null;
            GC.WaitForPendingFinalizers();
            GC.Collect();
        }

        object dataSource = null;
        string dataMember = string.Empty;

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
                return Engine.DataMember;
            }

            set
            {
                if (initEngine != null)
                {
                    dataMember = value;
                }
                else
                {
                    Engine.DataMember = value;
                }
            }
        }

        /// <override/>
        protected override void OnPaint(PaintEventArgs e)
        {
            this.engine.GetSourceList();
            this.TableControl.Table = Engine.Table;
            if (this.DesignMode)
            {
                this.PerformLayout();
                ////    Console.WriteLine(tableControl1.Bounds);
                ////    Console.WriteLine(tableControl1.Anchor);
                ////    Console.WriteLine(tableControl1.Dock);
                ////    Console.WriteLine(tableControl1.Parent.Bounds);
            }

            base.OnPaint(e);
        }
        /// <summary>
        /// Determine the changing uicues event.
        /// </summary>
        /// <param name="e"></param>
        protected override void OnChangeUICues(UICuesEventArgs e)
        {
            base.OnChangeUICues(e);
            if (gridHelper != null)
            {
                gridHelper.OnGridChangeUICues(this, e);
            }
        }
        /// <summary>
        /// Determine the lost focus.
        /// </summary>
        /// <param name="e"></param>
        protected override void OnLostFocus(EventArgs e)
        {
            base.OnLostFocus(e);
            if (gridHelper != null)
            {
                gridHelper.OnGridLostFocus(this, e);
            }
        }

        /// <summary>
        /// Gets / sets whether the <see cref="GridGroupDropArea"/> should be visible.
        /// </summary>
        [Description("Specifies whether the GridGroupDropArea should be visible.")]
        [Category("Grouping Control")]
        [DefaultValue(false)]
        public bool ShowGroupDropArea
        {
            get
            {
                if (this.Engine != null)
                    this.Engine.GroupDropArea = this.showGroupDropArea;
                return this.showGroupDropArea;
            }

            set
            {
                if (showGroupDropArea != value)
                {
                    this.OnPropertyChanging(new DescriptorPropertyChangedEventArgs("ShowGroupDropArea"));
                    showGroupDropArea = value;
                    if (this.Engine != null)
                        this.Engine.InternalSetGroupDropArea(value);
                    if (!stateOnly)
                    {
                        if (showGroupDropArea)
                        {
                            //// Show navigation
                            this.SuspendLayout();
                            this.groupDropPanel.Visible = true;
                            this.splitter1.Visible = true;
                            this.TableDescriptor.GroupedColumns.Changed += new ListPropertyChangedEventHandler(GroupedColumns_Changed);
                            this.Controls.Add(this.splitter1);
                            this.Controls.Add(this.groupDropPanel);
                            this.ResumeLayout();
                        }
                        else
                        {
                            //// Hide navigation
                            this.SuspendLayout();
                            this.groupDropPanel.Visible = false;
                            this.splitter1.Visible = false;
                            this.TableDescriptor.GroupedColumns.Changed -= new ListPropertyChangedEventHandler(GroupedColumns_Changed);
                            this.Controls.Remove(this.splitter1);
                            this.Controls.Remove(this.groupDropPanel);
                            this.ResumeLayout();
                        }
                    }

                    this.OnPropertyChanged(new DescriptorPropertyChangedEventArgs("ShowGroupDropArea"));
                }
            }
        }
        private GridGroupDropAreaAlignment gridGroupDropAreaAlignment;
        /// <summary>
        /// Gets / sets whether the <see cref="GridGroupDropAreaAlignment"/> should be top,right,left or bottom.
        /// </summary>
        [Browsable(true), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public GridGroupDropAreaAlignment GroupDropAreaAlignment
        {
            get { return gridGroupDropAreaAlignment; }
            set
            {
                if (!this.HierarchicalGroupDropArea)
                {
                    this.OnPropertyChanging(new DescriptorPropertyChangedEventArgs("GroupDropAreaAlignment"));
                    gridGroupDropAreaAlignment = value;
                    GroupAlign(value);
                }
                else
                {
                    GroupAlign(value);
                }
            }
        }
        private void GroupAlign(GridGroupDropAreaAlignment gridGroupDropAreaAlignment)
        {
            switch (gridGroupDropAreaAlignment)
            {
                case GridGroupDropAreaAlignment.Top:
                    {
                        this.groupDropArea1.Model.RowCount = 2;
                        this.groupDropArea1.Model.ColCount = 100;
                        this.groupDropPanel.Dock = System.Windows.Forms.DockStyle.Top;
                        this.splitter1.Dock = System.Windows.Forms.DockStyle.Top;
                        this.TableDescriptor.GroupedColumns.Changed += new ListPropertyChangedEventHandler(GroupedColumns_Changed);
                        this.BeginUpdate();
                        if (this.groupDropArea1.Dock == DockStyle.Fill)
                            this.groupDropArea1.Dock = DockStyle.None;
                        this.groupDropArea1.VScroll = false;
                        this.groupDropArea1.HScrollBehavior = GridScrollbarMode.Disabled;
                        this.groupDropArea1.VScrollBehavior = GridScrollbarMode.Disabled;
                        this.groupDropArea1.Location = new System.Drawing.Point(0, 0);
                        this.splitter1.Location = new System.Drawing.Point(0, 30);
                        this.splitter1.Size = new System.Drawing.Size(688, 3);
                        this.groupDropPanel.Location = new System.Drawing.Point(0, 0);
                        if (!EnableTouchMode)
                        {
                            this.groupDropArea1.Size = new System.Drawing.Size(tableControl1.Width, groupDropArea1.Model.RowHeights.GetTotal(1, 2));
                            this.groupDropPanel.Size = new System.Drawing.Size(tableControl1.Width, this.groupDropArea1.Height + groupDropArea1.Model.RowHeights[1]);
                        }
                        else
                        {
                            this.groupDropArea1.Size = new System.Drawing.Size(tableControl1.Width + 5, groupDropArea1.Model.RowHeights.GetTotal(1, 2) + 1);
                            this.groupDropPanel.Size = new System.Drawing.Size(tableControl1.Width + 5, this.groupDropArea1.Height + groupDropArea1.Model.RowHeights[1] + 1);
                        }
                        this.splitter1.Location = new System.Drawing.Point(0, 32);
                        if (this.GridVisualStyles == GridVisualStyles.Metro)
                            this.splitter1.BackColor = Color.FromArgb(94, 171, 222);
                        this.splitter1.MinSize = 25;
                        groupDropArea1.Model.changed = true;
                        this.groupDropArea1.Model.Refresh();
                        this.groupDropPanel.Refresh();
                        this.Refresh();
                        this.EndUpdate(true);
                        break;
                    }
                case GridGroupDropAreaAlignment.Bottom:
                    {
                        this.groupDropArea1.Model.RowCount = 2;
                        this.groupDropArea1.Model.ColCount = 100;
                        this.groupDropPanel.Dock = System.Windows.Forms.DockStyle.Bottom;
                        this.splitter1.Dock = System.Windows.Forms.DockStyle.Bottom;
                        this.BeginUpdate();
                        if (this.groupDropArea1.Dock == DockStyle.Fill)
                            this.groupDropArea1.Dock = DockStyle.None;
                        this.groupDropArea1.VScroll = false;
                        this.groupDropArea1.HScrollBehavior = GridScrollbarMode.Disabled;
                        this.groupDropArea1.VScrollBehavior = GridScrollbarMode.Disabled;
                        this.groupDropArea1.VScrollBar.Enabled = false;
                        if (!EnableTouchMode)
                        {
                            this.groupDropArea1.Size = new System.Drawing.Size(tableControl1.Width, groupDropArea1.Model.RowHeights.GetTotal(1, 2));
                            this.groupDropPanel.Size = new System.Drawing.Size(tableControl1.Width, this.groupDropArea1.Height + groupDropArea1.Model.RowHeights[1]);
                        }
                        else
                        {
                            this.groupDropArea1.Size = new System.Drawing.Size(tableControl1.Width + 5, groupDropArea1.Model.RowHeights.GetTotal(1, 2) + 1);
                            this.groupDropPanel.Size = new System.Drawing.Size(tableControl1.Width + 5, this.groupDropArea1.Height + groupDropArea1.Model.RowHeights[1] + 1);
                        }
                        this.splitter1.Location = new System.Drawing.Point(0, 30);
                        if (this.GridVisualStyles == GridVisualStyles.Metro)
                            this.splitter1.BackColor = Color.FromArgb(94, 171, 222);
                        this.splitter1.MinSize = 25;
                        this.groupDropArea1.Model.Refresh();
                        groupDropArea1.Model.changed = true;
                        this.groupDropPanel.Refresh();
                        this.EndUpdate(true);
                        this.Refresh();
                        break;
                    }
                case GridGroupDropAreaAlignment.Left:
                    {
                        this.groupDropArea1.Model.RowCount = 100;
                        this.groupDropArea1.Model.ColCount = 2;
                        this.groupDropPanel.Dock = System.Windows.Forms.DockStyle.Left;
                        this.TableDescriptor.GroupedColumns.Changed += new ListPropertyChangedEventHandler(GroupedColumns_Changed);
                        this.splitter1.Dock = System.Windows.Forms.DockStyle.Left;
                        this.BeginUpdate();
                        if (this.groupDropArea1.Dock == DockStyle.Fill)
                            this.groupDropArea1.Dock = DockStyle.None;
                        if (this.TableDescriptor.GroupedColumns.Count == 0)
                        {
                            this.groupDropArea1.Size = new System.Drawing.Size(117, tableControl1.Height);
                            this.groupDropPanel.Size = new System.Drawing.Size(122, tableControl1.Height);
                        }
                        else
                        {
                            if (!EnableTouchMode)
                            {
                                this.groupDropArea1.Size = new System.Drawing.Size(groupDropArea1.Model.ColWidths.GetTotal(1, 2), tableControl1.Height);
                                this.groupDropPanel.Size = new System.Drawing.Size(this.groupDropArea1.Width + groupDropArea1.Model.ColWidths[1], tableControl1.Height);
                            }
                            else
                            {
                                this.groupDropArea1.Size = new System.Drawing.Size(groupDropArea1.Model.ColWidths.GetTotal(1, 2), tableControl1.Height + 1);
                                this.groupDropPanel.Size = new System.Drawing.Size(this.groupDropArea1.Width + groupDropArea1.Model.ColWidths[1], tableControl1.Height + 1);
                            }
                        }
                        this.splitter1.MinSize = 100;
                        this.groupDropPanel.ForeColor = Color.Black;
                        if (this.GridVisualStyles == GridVisualStyles.Metro)
                            this.splitter1.BackColor = Color.FromArgb(208, 208, 208);
                        this.groupDropArea1.VScroll = false;
                        this.groupDropArea1.HScrollBehavior = GridScrollbarMode.Disabled;
                        this.groupDropArea1.VScrollBehavior = GridScrollbarMode.Disabled;
                        this.groupDropPanel.Refresh();
                        this.groupDropArea1.Refresh();
                        this.groupDropArea1.Model.Refresh();
                        this.TableModel.Refresh();
                        this.EndUpdate(true);
                        this.Refresh();
                        break;
                    }
                case GridGroupDropAreaAlignment.Right:
                    {
                        this.groupDropArea1.Model.RowCount = 100;
                        this.groupDropArea1.Model.ColCount = 2;
                        this.groupDropPanel.Dock = System.Windows.Forms.DockStyle.Right;
                        this.splitter1.Dock = System.Windows.Forms.DockStyle.Right;
                        this.BeginUpdate();
                        if (this.groupDropArea1.Dock == DockStyle.Fill)
                            this.groupDropArea1.Dock = DockStyle.None;
                        if (this.TableDescriptor.GroupedColumns.Count == 0)
                        {
                            this.groupDropArea1.Size = new System.Drawing.Size(117, tableControl1.Height);
                            this.groupDropPanel.Size = new System.Drawing.Size(122, tableControl1.Height);
                        }
                        else
                        {
                            this.groupDropArea1.Size = new System.Drawing.Size(groupDropArea1.Model.ColWidths.GetTotal(1, 2), tableControl1.Height);
                            this.groupDropPanel.Size = new System.Drawing.Size(this.groupDropArea1.Width + groupDropArea1.Model.ColWidths[1], tableControl1.Height);
                        }
                        this.groupDropArea1.VScroll = false;
                        this.groupDropArea1.HScrollBehavior = GridScrollbarMode.Disabled;
                        this.groupDropArea1.VScrollBehavior = GridScrollbarMode.Disabled;
                        this.splitter1.MinSize = 100;
                        if (this.GridVisualStyles == GridVisualStyles.Metro)
                            this.splitter1.BackColor = Color.FromArgb(208, 208, 208);
                        this.groupDropPanel.Refresh();
                        this.EndUpdate(true);
                        this.groupDropArea1.Refresh();
                        this.TableModel.Refresh();
                        this.Refresh();
                        break;
                    }
            }
        }
        internal bool hGroupDropArea = false;
         /// <summary>
        /// Gets / sets whether the <see cref="GridGroupDropArea"/> should be in Hierarchy.
        /// </summary>
        [Category("Grouping Control")]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        [DefaultValue(false),Description("Specifies whether the GridGroupDropArea should be in Hierarchical structure")]
        public bool HierarchicalGroupDropArea
        {
            get
            {
                return hGroupDropArea;
            }
            set
            {
                if (hGroupDropArea != value)
                {
                    this.GroupDropAreaAlignment = GridGroupDropAreaAlignment.Top;
                    this.OnPropertyChanging(new DescriptorPropertyChangedEventArgs("HierarchicalGroupDropArea"));
                    this.TableModel.HierarchicalGroupDropArea = value;
                    hGroupDropArea = value;
                    List<string> groupedColList = new List<string>();
                    foreach (SortColumnDescriptor item in this.TableDescriptor.GroupedColumns)
                    {
                        groupedColList.Add(item.Name);
                    }
                    if (hGroupDropArea)
                    {
                        this.groupDropPanel.Dock = System.Windows.Forms.DockStyle.Top;
                        this.splitter1.Dock = System.Windows.Forms.DockStyle.Top;
                        this.TableDescriptor.GroupedColumns.Changed += new ListPropertyChangedEventHandler(GroupedColumns_Changed);
                        if (this.TableDescriptor.GroupedColumns.Count > 0)
                        {
                            this.TableDescriptor.ResetGroupedColumns();
                            this.BeginUpdate();
                            foreach (string colName in groupedColList)
                            {
                                this.TableDescriptor.GroupedColumns.Add(colName);
                            }
                            this.EndUpdate(true);
                        }
                        this.Refresh();
                    }
                    else
                    {
                        if (this.TableDescriptor.GroupedColumns.Count > 0)
                        {
                            this.TableDescriptor.ResetGroupedColumns();
                            this.BeginUpdate();
                            foreach (string colName in groupedColList)
                            {
                                this.TableDescriptor.GroupedColumns.Add(colName);
                            }
                            this.EndUpdate(true);
                        }
                        this.TableDescriptor.GroupedColumns.Changed -= new ListPropertyChangedEventHandler(GroupedColumns_Changed);
                    }
                    this.OnPropertyChanged(new DescriptorPropertyChangedEventArgs("HierarchicalGroupDropArea"));
                }
            }
        }

        /// <summary>
        /// Handler to rearrange the controls to support Dynamic Resizing
        /// </summary>
        void GroupedColumns_Changed(object sender, ListPropertyChangedEventArgs e)
        {
            int noOfDropRows, n1, n2, n3, n4;
            if (EnableTouchMode)
            {
                this.Table.DefaultCaptionRowHeight = 27;
            }
            if (EnableTouchMode && this.TableDescriptor.GroupedColumns.Count == 0)
            {
                this.Table.DefaultRecordRowHeight = 30;
            }
            if (hGroupDropArea)
            {
                int oldheight = this.groupDropPanel.Size.Height;
                if (this.GroupDropPanel.Controls.Count > 1)
                {
                    GridGroupDropArea[] groupDropArea = new GridGroupDropArea[this.GroupDropPanel.Controls.Count];
                    this.BeginUpdate();
                    for (int i = 0; i < this.GroupDropPanel.Controls.Count; i++)
                    {
                        groupDropArea[i] = (GridGroupDropArea)this.GroupDropPanel.Controls[i];
                    }
                    for (int i = 0; i < this.GroupDropPanel.Controls.Count; i++)
                    {
                        noOfDropRows = groupDropArea[i].Model.RowCount > 1 ? groupDropArea[i].Model.RowCount - 1 : 1;
                        n1 = groupDropArea[i].Model.RowCount <= 2 ? noOfDropRows * groupDropArea[i].Model.globalWidth : noOfDropRows * (groupDropArea[i].Model.globalWidth / 2);
                        n2 = 5;
                        groupDropArea[i].Size = new Size(this.tableControl1.Width, (n1 + n2));
                        oldheight = groupDropArea[i].Size.Height + groupDropArea[i].Location.Y;
                        if (i < (this.GroupDropPanel.Controls.Count - 1))
                        {
                            groupDropArea[i + 1].Location = new Point(groupDropArea[i + 1].Location.X, groupDropArea[i].Size.Height + groupDropArea[i].Location.Y);
                        }
                    }
                    this.splitter1.Location = new Point(this.splitter1.Location.X, oldheight);
                    this.groupDropPanel.Size = new Size(this.groupDropPanel.Size.Width, this.splitter1.Location.Y);
                    this.groupDropPanel.Refresh();
                    this.EndUpdate(true);
                    this.Refresh();
                }
                else
                {
                    if (this.groupDropArea1.DynamicResizing || this.TableDescriptor.GroupedColumns.Count <= 4)
                    {
                        noOfDropRows = this.groupDropArea1.Model.RowCount - 1;
                        n1 = this.TableDescriptor.GroupedColumns.Count > 1 ? noOfDropRows * (groupDropArea1.Model.globalWidth / 2) : noOfDropRows * groupDropArea1.Model.globalWidth;
                        n2 = 5;
                        //For Touch Mode
                        n3 = this.TableDescriptor.GroupedColumns.Count > 1 ? noOfDropRows * (groupDropArea1.Model.globalWidth / 2) + 15 : noOfDropRows * groupDropArea1.Model.globalWidth;
                        n4 = 15;
                        this.BeginUpdate();
                        if (this.groupDropArea1.Dock == DockStyle.Fill)
                            this.groupDropArea1.Dock = DockStyle.None;
                        this.groupDropArea1.HScrollBehavior = GridScrollbarMode.Disabled;
                        this.groupDropArea1.VScrollBehavior = GridScrollbarMode.Disabled;
                        this.splitter1.Location = new Point(this.splitter1.Location.X, this.groupDropArea1.Size.Height + n2);
                        if (EnableTouchMode)
                        {
                            this.groupDropArea1.Size = new Size(this.tableControl1.Width, (n3 + n4));
                            this.groupDropPanel.Size = new Size(this.groupDropPanel.Size.Width, this.splitter1.Location.Y);
                        }
                        else
                        {
                            this.groupDropArea1.Size = new Size(this.tableControl1.Width, (n1 + n2));
                            this.groupDropPanel.Size = new Size(this.groupDropPanel.Size.Width, this.splitter1.Location.Y);
                        }
                        this.groupDropPanel.Refresh();
                        this.EndUpdate(true);
                        this.Refresh();
                    }
                    else
                    {
                        this.groupDropArea1.Dock = DockStyle.Fill;
                        this.groupDropArea1.VScrollBehavior = GridScrollbarMode.Automatic;
                        this.groupDropArea1.HScrollBehavior = GridScrollbarMode.Automatic;
                    }
                    if (this.TableDescriptor.GroupedColumns.Count > 4)
                    {
                        this.BeginUpdate();
                        this.groupDropArea1.Dock = DockStyle.Fill;
                        this.groupDropArea1.VScrollBehavior = GridScrollbarMode.Automatic;
                        this.groupDropArea1.HScrollBehavior = GridScrollbarMode.Automatic;
                        this.EndUpdate(true);
                        this.Refresh();
                    }
                }
            }
            else
            {
                if ((this.GroupDropPanel.Controls.Count <= 1) && !(this.GroupDropAreaAlignment == GridGroupDropAreaAlignment.Left || this.GroupDropAreaAlignment == GridGroupDropAreaAlignment.Right))
                {
                    n1 = 1 * groupDropArea1.Model.globalWidth;
                    n2 = 5;
                    this.BeginUpdate();
                    this.splitter1.Location = new Point(this.splitter1.Location.X, this.groupDropArea1.Size.Height + n2);
                       // Changing the size of the dropDrop while adding or removing Group in GroupDropArea
                    if (this.EnableTouchMode)
                    {
                        this.groupDropArea1.Size = new System.Drawing.Size(tableControl1.Width + 5, groupDropArea1.Model.RowHeights.GetTotal(1, 2) + 1);
                        this.groupDropPanel.Size = new System.Drawing.Size(tableControl1.Width + 5, this.groupDropArea1.Height + groupDropArea1.Model.RowHeights[1] + 1);
                    }
                    else
                    {
                        this.groupDropArea1.Size = new System.Drawing.Size(tableControl1.Width, groupDropArea1.Model.RowHeights.GetTotal(1, 2));
                        this.groupDropPanel.Size = new System.Drawing.Size(tableControl1.Width, this.groupDropArea1.Height + groupDropArea1.Model.RowHeights[1]);
                    }
                    this.groupDropPanel.Refresh();
                    this.EndUpdate(true);
                    this.Refresh();
                }
            }
            if (e.Action == ListPropertyChangedType.Remove)
            {
                ReinitializeMerging();
            }
        }
        
        bool wantTabKey = true;

        /// <summary>
        /// Gets / sets whether the Tab key should be used to move from cell to cell (= true) or
        /// if it should select the next control in the parent form (= false). Default is true.
        /// </summary>
        [Category("Grouping Control")]
        [DefaultValue(true)]
        [Description("the Tab key should be used to move from cell to cell (= true) or select the next control in the parent form (= false).")]
        public bool WantTabKey
        {
            get
            {
                return this.wantTabKey;
            }

            set
            {
                if (wantTabKey != value)
                {
                    this.OnPropertyChanging(new DescriptorPropertyChangedEventArgs("WantTabKey"));
                    wantTabKey = value;
                    if (!stateOnly)
                    {
                        this.TableControl.WantTabKey = false;
                    }

                    this.OnPropertyChanged(new DescriptorPropertyChangedEventArgs("WantTabKey"));
                }
            }
        }

        /// <override/>
        protected override void OnLeave(EventArgs e)
        {
            if (!this.WantTabKey)
            {
                Table.CurrentElement = null;
            }

            base.OnLeave(e);
            if (gridHelper != null)
            {
                gridHelper.OnGridLeave(this, e);
            }
        }

        Control GetParentGroupingControl(Control c)
        {
            while (c != null && !(c is GridGroupingControl))
            {
                c = c.Parent;
            }

            return c;
        }

        /// <summary>
        /// Processes a dialog key.
        /// </summary>
        /// <param name="keyData">One of the <see cref="T:System.Windows.Forms.Keys"/> values that represents the key to process.</param>
        /// <returns>
        /// true if the key was processed by the control; otherwise, false.
        /// </returns>
        /// <override/>
        protected override bool ProcessDialogKey(Keys keyData)
        {
            if (!this.WantTabKey)
            {
                Form f = this.FindForm();
                if (f != null)
                {
                    if (keyData == (Keys.Tab | Keys.Shift))
                    {
                        f.SelectNextControl(this, false, true, false, true);
                        return true;
                    }
                    else if (keyData == Keys.Tab)
                    {
                        f.SelectNextControl(this, true, true, false, true);
                        return true;
                    }
                }
            }

            return base.ProcessDialogKey(keyData);
        }

        private Office2007ColorScheme office2007ScrollBarsColorScheme = Office2007ColorScheme.Blue;
        bool office2007ScrollBars = false;

        /// <summary>
        /// Toggles between standard and Office2007 scrollbars.
        /// </summary>
        [Category("Look and Feel")]
        [Description("Toggle between standard and Office2007 scrollbars.")]
        [DefaultValue(false)]
        [NotifyParentProperty(true)]
        public bool Office2007ScrollBars
        {
            get
            {
                return office2007ScrollBars;
            }

            set
            {
                if (office2007ScrollBars != value)
                {
                    office2007ScrollBars = value;
                    if (this.ShowNavigationBar)
                    {
                        this.RecordNavigationControl.Office2007ScrollBars = value;
                        if (value) this.RecordNavigationControl.GridOfficeScrollBars = OfficeScrollBars.Office2007;
                    }

                    this.TableControl.Office2007ScrollBars = value;
                    if (value)
                    {
                        this.GridOfficeScrollBars = OfficeScrollBars.Office2007;
                        this.TableControl.GridOfficeScrollBars = OfficeScrollBars.Office2007;
                    }
                }

                OnOffice2007ScrollBarsChanged(EventArgs.Empty);
                OfficeScrollBarsEventArgs eventArgs = new OfficeScrollBarsEventArgs(OfficeScrollBars.Office2007);
                OnOfficeScrollBarsChanged(eventArgs);
            }
        }
        # region Office2010ScrollBarsColorSchemeChanged
        /// <summary>
        /// Occurs when the <see cref="Office2010ScrollBarsColorScheme"/> property has changed.
        /// </summary>
        [Category("Appearance"), Description("Occurs when the Office2010ScrollBarsColorScheme property has changed.")]
        public event EventHandler Office2010ScrollBarsColorSchemeChanged;

        /// <summary>
        /// Raises the <see cref="OnOffice2010ScrollBarsColorSchemeChanged"/> event.
        /// </summary>
        /// <param name="e">Event data.</param>
        protected virtual void OnOffice2010ScrollBarsColorSchemeChanged(EventArgs e)
        {
            if (Office2010ScrollBarsColorSchemeChanged != null)
                Office2010ScrollBarsColorSchemeChanged(this, e);
        }
        # endregion

        # region  OfficeScrollBarsChanged
        /// <summary>
        /// Occurs when the <see cref="Office2007ScrollBars"/> property has changed.
        /// </summary>
        [Category("Appearance"), Description("Occurs when the SupportsOfficeFlatScrollBars property has changed.")]
        public event OfficeScrollBarsEventHandler OfficeScrollBarsChanged;
        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        public delegate void OfficeScrollBarsEventHandler(object sender, OfficeScrollBarsEventArgs e);
        /// <summary>
        /// Raises the <see cref='OfficeScrollBarsChanged'/> event
        /// </summary>
        /// <param name="e"> Office scrollbar type </param>
        protected virtual void OnOfficeScrollBarsChanged(OfficeScrollBarsEventArgs e)
        {
            if (OfficeScrollBarsChanged != null)
            {
                OfficeScrollBarsChanged(this, e);
            }
        }
        /// <summary>
        /// Provides the data about <see cref="OfficeScrollBarsChanged"/> event of a <see cref="ScrollControl"/>.
        /// </summary>
        public class OfficeScrollBarsEventArgs : EventArgs
        {
            /// <summary>
            /// Initializes a new <see cref="CurrentRecordEventArgs"/>.
            /// </summary>
            /// <param name="gridOfficeScrollBars">The record index.</param>
            public OfficeScrollBarsEventArgs(OfficeScrollBars gridOfficeScrollBars)
            {
                this.gridOfficeScrollBars = gridOfficeScrollBars;
            }

            /// <summary>
            /// Gets or sets the Office scroll bars
            /// </summary>
            public OfficeScrollBars GridOfficeScrollBars
            {
                get
                {
                    return gridOfficeScrollBars;
                }
                set
                {
                    if (gridOfficeScrollBars != value)
                    {
                        gridOfficeScrollBars = value;
                    }
                }
            }
            private OfficeScrollBars gridOfficeScrollBars;
        }

        # endregion

        /// <summary>
        /// Raises the <see cref="SplitterControl.Office2007ScrollBarsChanged"/> event.
        /// </summary>
        /// <param name="e">A <see cref="System.EventArgs"/> that contains the event data.</param>
        protected virtual void OnOffice2007ScrollBarsChanged(EventArgs e)
        {
            if (Office2007ScrollBarsChanged != null)
            {
                Office2007ScrollBarsChanged(this, e);
            }
        }

        /// <summary>
        /// Gets / sets the style of Office2007 scroll bars
        /// </summary>
        [Category("Look and Feel")]
        [Description("Office 2007 style scrollbars.")]
        [DefaultValue(Office2007ColorScheme.Blue)]
        [NotifyParentProperty(true)]
        public Office2007ColorScheme Office2007ScrollBarsColorScheme
        {
            get
            {
                return office2007ScrollBarsColorScheme;
            }

            set
            {
                if (this.office2007ScrollBarsColorScheme != value)
                {
                    office2007ScrollBarsColorScheme = value;
                    if (this.ShowNavigationBar)
                    {
                        this.RecordNavigationControl.Office2007ScrollBarsColorScheme = value;
                    }

                    this.TableControl.Office2007ScrollBarsColorScheme = value;
                    if (this.DesignMode)
                    {
                        this.Office2007ScrollBars = true;
                        this.GridOfficeScrollBars = OfficeScrollBars.Office2007;
                    }
                }
                OnOffice2007ScrollBarsColorSchemeChanged(EventArgs.Empty);
            }
        }

        /// <summary>
        /// Raises the <see cref="OnOffice2007ScrollBarsColorSchemeChanged"/> event.
        /// </summary>
        /// <param name="e">Event data.</param>
        protected virtual void OnOffice2007ScrollBarsColorSchemeChanged(EventArgs e)
        {
            if (Office2007ScrollBarsColorSchemeChanged != null)
            {
                Office2007ScrollBarsColorSchemeChanged(this, e);
            }
        }

        /// <summary>
        /// Occurs when the <see cref="Office2007ColorScheme"/> property has changed.
        /// </summary>
        [Description("Occurs when the Office2007ColorScheme property has changed."), Category("Appearance")]
        public event EventHandler Office2007ScrollBarsColorSchemeChanged;

        /// <summary>
        /// Occurs when the <see cref="Office2007ScrollBars"/> property has changed.
        /// </summary>
        [Description("Occurs when the SupportsOffice2007FlatScrollBars property has changed."), Category("Appearance")]
        public event EventHandler Office2007ScrollBarsChanged;

        #region Office2010 Scrollbars
        private Office2010ColorScheme office2010ScrollBarsColorScheme = Office2010ColorScheme.Blue;
        OfficeScrollBars gridOfficeScrollBars = OfficeScrollBars.None;

        /// <summary>
        /// Gets or sets the Office like scrollbars.
        /// </summary>
        [
        SRCategory(@"Appearance"),
        Description("Gets or sets the Office like scrollbars"),
        DefaultValue(OfficeScrollBars.None),
        NotifyParentProperty(true)
        ]
        public OfficeScrollBars GridOfficeScrollBars
        {
            get
            {
                return gridOfficeScrollBars;
            }
            set
            {
                if (gridOfficeScrollBars != value)
                {
                    gridOfficeScrollBars = value;

                    if (this.ShowNavigationBar)
                    {
                        this.RecordNavigationControl.GridOfficeScrollBars = gridOfficeScrollBars;
                    }
                    this.TableControl.GridOfficeScrollBars = gridOfficeScrollBars;
                    if (GridOfficeScrollBars == OfficeScrollBars.Office2007)
                        this.Office2007ScrollBars = (gridOfficeScrollBars == OfficeScrollBars.Office2007);
                    this.tableControl1.MetroScrollBars = false;
                    if (this.GridOfficeScrollBars == OfficeScrollBars.Office2010)
                    {
                        if (this.tableControl1.HScrollBar.InnerScrollBar is ScrollBarCustomDraw)
                        {
                            ScrollBarCustomDraw sbar = (ScrollBarCustomDraw)this.tableControl1.HScrollBar.InnerScrollBar;
                            sbar.VisualStyle = ScrollBarCustomDrawStyles.Office2010;
                            sbar.Office2010ColorScheme = Office2010ScrollBarsColorScheme;
                        }
                        if (this.tableControl1.VScrollBar.InnerScrollBar is ScrollBarCustomDraw)
                        {
                            ScrollBarCustomDraw sbar = (ScrollBarCustomDraw)this.tableControl1.VScrollBar.InnerScrollBar;
                            sbar.VisualStyle = ScrollBarCustomDrawStyles.Office2010;
                            sbar.Office2010ColorScheme = Office2010ScrollBarsColorScheme;
                        }
                    }
                    else if (this.GridOfficeScrollBars == OfficeScrollBars.Office2007)
                    {
                        if (this.tableControl1.HScrollBar.InnerScrollBar is ScrollBarCustomDraw)
                        {
                            ScrollBarCustomDraw sbar = (ScrollBarCustomDraw)this.tableControl1.HScrollBar.InnerScrollBar;
                            sbar.VisualStyle = ScrollBarCustomDrawStyles.Office2007;
                            sbar.OfficeColorScheme = Office2007ScrollBarsColorScheme;
                        }
                        if (this.tableControl1.VScrollBar.InnerScrollBar is ScrollBarCustomDraw)
                        {
                            ScrollBarCustomDraw sbar = (ScrollBarCustomDraw)this.tableControl1.VScrollBar.InnerScrollBar;
                            sbar.VisualStyle = ScrollBarCustomDrawStyles.Office2007;
                            sbar.OfficeColorScheme = Office2007ScrollBarsColorScheme;
                        }
                    }
                    else if (this.GridOfficeScrollBars == OfficeScrollBars.Metro)
                        this.tableControl1.MetroScrollBars = true;
                }

                OfficeScrollBarsEventArgs eventArgs = new OfficeScrollBarsEventArgs(gridOfficeScrollBars);
                OnOfficeScrollBarsChanged(eventArgs);
            }
        }


        /// <summary>
        /// Gets or sets the style of Office2007 scroll bars.
        /// </summary>
        [Category("Look and Feel")]
        [Description("Office 2010 style scrollbars.")]
        [DefaultValue(Office2010ColorScheme.Blue)]
        [NotifyParentProperty(true)]
        public Office2010ColorScheme Office2010ScrollBarsColorScheme
        {
            get
            {
                return office2010ScrollBarsColorScheme;
            }
            set
            {
                if (this.office2010ScrollBarsColorScheme != value)
                {
                    office2010ScrollBarsColorScheme = value;
                    if (this.ShowNavigationBar)
                    {
                        this.RecordNavigationControl.Office2010ScrollBarsColorScheme = value;
                    }

                    this.TableControl.Office2010ScrollBarsColorScheme = value;
                    if (this.DesignMode)
                    {
                        this.GridOfficeScrollBars = OfficeScrollBars.Office2010;
                        this.Office2007ScrollBars = false;
                    }
                }
                OnOffice2010ScrollBarsColorSchemeChanged(EventArgs.Empty);
            }
        }
        # endregion
        bool showNavigationBar = false;

        /// <summary>
        /// Gets / sets whether the <see cref="RecordNavigationBar"/> should be visible.
        /// </summary>
        [Description("Specifies whether the NavigationBar should be visible.")]
        [Category("Grouping Control")]
        [DefaultValue(false)]
        public bool ShowNavigationBar
        {
            get
            {
                return showNavigationBar;
            }

            set
            {
                if (showNavigationBar != value)
                {
                    this.OnPropertyChanging(new DescriptorPropertyChangedEventArgs("ShowNavigationBar"));
                    showNavigationBar = value;
                    if (!stateOnly)
                    {
                        if (showNavigationBar)
                        {
                            //// Show navigation.
                            this.SuspendLayout();
                            this.TableControl.Dock = DockStyle.None;
                            this.TableControl.Size = new Size(100, 40);
                            this.recordNavigationControl1.Visible = true;
                            this.recordNavigationControl1.Bounds = this.gridTablePanel.ClientRectangle;
                            this.recordNavigationControl1.GridOfficeScrollBars = this.GridOfficeScrollBars;
                            this.recordNavigationControl1.Office2007ScrollBars = this.Office2007ScrollBars;
                            this.recordNavigationControl1.Office2007ScrollBarsColorScheme = this.Office2007ScrollBarsColorScheme;
                            this.recordNavigationControl1.Office2010ScrollBarsColorScheme = this.Office2010ScrollBarsColorScheme;
                            this.TableControl.Parent = recordNavigationControl1;
                            this.TableControl.FillSplitterPane = true;
                            this.TableControl.InitSplitterControl();
                            this.ResumeLayout();
                        }
                        else
                        {
                            //// Hide navigation.
                            this.SuspendLayout();
                            this.recordNavigationControl1.Visible = false;
                            this.gridTablePanel.Controls.Add(this.tableControl1);
                            this.tableControl1 = this.tableControl1.Model == null ? CreateTableControl(savedModel != null ? savedModel : this.TableModel) : this.tableControl1;
                            this.tableControl1.FillSplitterPane = false;
                            this.tableControl1.InitSplitterControl();
                            this.tableControl1.Dock = DockStyle.Fill;
                            this.tableControl1.VScrollBehavior = GridScrollbarMode.Automatic | GridScrollbarMode.AutoScroll;
                            this.tableControl1.HScrollBehavior = GridScrollbarMode.Automatic | GridScrollbarMode.AutoScroll;
                            this.tableControl1.GridOfficeScrollBars = this.GridOfficeScrollBars;
                            this.tableControl1.Office2007ScrollBars = this.Office2007ScrollBars;
                            this.tableControl1.Office2007ScrollBarsColorScheme = this.Office2007ScrollBarsColorScheme;
                            this.tableControl1.Office2010ScrollBarsColorScheme = this.Office2010ScrollBarsColorScheme;
                            this.tableControl1.ThemesEnabled = this.ThemesEnabled;
                            this.ResumeLayout();
                            this.tableControl1.UpdateScrollBars();
                        }
                    }

                    this.OnPropertyChanged(new DescriptorPropertyChangedEventArgs("ShowNavigationBar"));
                }
            }
        }

        /// <summary>
        /// Toggles support for Intelli-Mouse panning. When the user presses the middle mouse button and drags the mouse,
        /// the window will scroll.
        /// </summary>
        [DefaultValue(false),
        Description("Toggles support for IntelliMouse panning."),
        Category("Grouping Control")]
        public bool IntelliMousePanning
        {
            get
            {
                return TableControl.EnableIntelliMouse;
            }

            set
            {
                if (value != IntelliMousePanning)
                {
                    this.OnPropertyChanging(new DescriptorPropertyChangedEventArgs("EnableIntelliMouse"));
                    TableControl.EnableIntelliMouse = value;
                    this.OnPropertyChanged(new DescriptorPropertyChangedEventArgs("EnableIntelliMouse"));
                }
            }
        }

        bool isThemed = true;

        /// <summary>
        /// Gets / sets whether the control should be drawn using Windows XP Themes if available.
        /// </summary>
        [Category("Grouping Control")]
        [DefaultValue(true), Description("Specifies whether the control should be drawn using Windows XP Themes if available")]
        public bool ThemesEnabled
        {
            get
            {
                return isThemed;
            }

            set
            {
                if (isThemed != value)
                {
                    this.OnPropertyChanging(new DescriptorPropertyChangedEventArgs("ThemesEnabled"));
                    isThemed = value;
                    if (!stateOnly)
                    {
                        this.recordNavigationControl1.ThemesEnabled = ThemesEnabled;
                        this.TableControl.ThemesEnabled = ThemesEnabled;
                        foreach (Control control in this.groupDropPanel.Controls)
                        {
                            IThemedControl themedControl = control as IThemedControl;
                            if (themedControl != null)
                            {
                                themedControl.ThemesEnabled = this.ThemesEnabled;
                            }
                        }
                    }

                    this.OnPropertyChanged(new DescriptorPropertyChangedEventArgs("ThemesEnabled"));
                }
            }
        }

        /// <summary>
        /// Maintains a collection of base styles. Users can add BaseStyles to the engine (also in design-time) and
        /// then inherit style settings through the GridStyleInfo.BaseStyle property in <see cref="GridTableCellStyleInfo"/>
        /// property of <see cref="GridTableCellAppearance"/>.
        /// </summary>
        [Category("Look and Feel")]
        [Browsable(true), DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
        [Description("The collection of base styles used in this grid.")]
        [RefreshProperties(RefreshProperties.Repaint)]
        public GridTableBaseStyleCollection BaseStyles
        {
            get
            {
                return Engine.BaseStyles;
            }

            set
            {
                Engine.BaseStyles = value;
            }
        }

        /// <summary>
        /// Determines whether the <see cref="BaseStyles"/> collection was modified.
        /// </summary>
        /// <returns>True if the collection was modified.</returns>
        public bool ShouldSerializeBaseStyles()
        {
            return Engine.ShouldSerializeBaseStyles();
        }

        /// <summary>
        /// Resets the <see cref="BaseStyles"/> property.
        /// </summary>
        public void ResetBaseStyles()
        {
            Engine.ResetBaseStyles();
        }

        /// <summary>
        /// Maintains the table schema information of the root table in the datasource.
        /// </summary>
        [Browsable(true), DesignerSerializationVisibility(DesignerSerializationVisibility.Content), Description("Maintains the table schema information of the root table in the datasource.")]
        [Category("TableDescriptors")]
        [RefreshPropertiesAttribute(RefreshProperties.All)]
        public GridTableDescriptor TableDescriptor
        {
            get
            {
                return Engine.TableDescriptor;
            }

            set
            {
                Engine.TableDescriptor = value;
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
            return TableDescriptor.GetModified();
        }

        /// <summary>
        /// Resets the <see cref="TableDescriptor"/> back to its
        /// default state and autopopulates schema information on demand.
        /// </summary>
        public void ResetTableDescriptor()
        {
            if (this.ShouldSerializeTableDescriptor())
            {
                System.ComponentModel.Design.IComponentChangeService changeService = null;
                if (!stateOnly && Site != null)
                {
                    changeService = Site.GetService(typeof(System.ComponentModel.Design.IComponentChangeService)) as System.ComponentModel.Design.IComponentChangeService;
                }

                if (changeService != null)
                {
                    MemberDescriptor md = TypeDescriptor.GetProperties(this)["TableDescriptor"];
                    changeService.OnComponentChanging(this, md);

                    TableDescriptor.ResetTableDescriptor();
                    Engine.ResetTable();

                    changeService.OnComponentChanged(this, md, null, null);
                    TableControl.Refresh();
                }
                else
                {
                    TableDescriptor.ResetTableDescriptor();
                    Engine.ResetTable();
                    Repaint();
                }
            }
        }

        /// <summary>
        /// Discards current cells and current record changes.
        /// </summary>
        public void CancelEdit()
        {
            this.TableControl.CurrentCell.Deactivate(true);
            this.engine.Table.CurrentRecordManager.CancelEdit();
        }

        private void engine_TableCreated(object sender, EventArgs e)
        {
            if (this.tableControl1 != null)
            {
                this.tableControl1.Table = Engine.Table;
            }

            if (this.groupDropArea1 != null)
            {
                this.groupDropArea1.Model.Table = Engine.Table;
            }

            WireTable();
        }

        private void Table_CurrentRecordContextChange(object sender, CurrentRecordContextChangeEventArgs e)
        {
            if (e.Action == CurrentRecordAction.EnterRecordComplete)
            {
                UpdateNavigationBar();
            }
        }

        /// <summary>
        /// The table object manages the records from the engine's DataSource and
        /// provides access to records and grouped elements through several
        /// collection classes, most prominent the <see cref="DisplayElementsInTableCollection"/>.
        /// </summary>
        [System.Xml.Serialization.XmlIgnore]
        [DesignerSerializationVisibilityAttribute(DesignerSerializationVisibility.Hidden)]
        [Browsable(false)]
        public GridTable Table
        {
            get
            {
                return Engine.Table;
            }
        }

        /// <summary>
        /// The record navigation control which hosts the record navigation bar
        /// and also the <see cref="TableControl"/>.
        /// </summary>
        /// <remarks>The control is only visible
        /// if <see cref="GridGroupingControl.ShowNavigationBar"/> is set.
        /// It is a child control of the <see cref="GridTablePanel"/>.
        /// </remarks>
        [System.Xml.Serialization.XmlIgnore]
        [DesignerSerializationVisibilityAttribute(DesignerSerializationVisibility.Hidden)]
        [Browsable(false)]
        public RecordNavigationControl RecordNavigationControl
        {
            get
            {
                return recordNavigationControl1;
            }
        }

        /// <summary>
        /// Gets the hosted <see cref="GridTableControl"/>. The GridTableControl is a grid derived from
        /// GridControlBase and displays and allows user interaction and modification of data.
        /// </summary>
        /// <remarks>
        /// This control is either a child control of the <see cref="GridTablePanel"/>
        /// or <see cref="RecordNavigationControl"/> depending on whether <see cref="GridGroupingControl.ShowNavigationBar"/> is set.
        /// </remarks>
        [System.Xml.Serialization.XmlIgnore]
        [DesignerSerializationVisibilityAttribute(DesignerSerializationVisibility.Hidden)]
        [Browsable(false)]
        public GridTableControl TableControl
        {
            get
            {
                if (tableControl1 == null && this.recordNavigationControl1 != null)
                {
                    this.tableControl1 = (GridTableControl)this.recordNavigationControl1.GetPane(0, 0);
                }

                return tableControl1;
            }
        }

        /// <summary>
        /// Gets the <see cref="GridTableModel"/> of the <see cref="TableControl"/>.
        /// The GridTableModel is derived from the GridModel and adds support for retrieving
        /// data from the datasource for a <see cref="GridTableControl"/>.
        /// </summary>
        [System.Xml.Serialization.XmlIgnore]
        [DesignerSerializationVisibilityAttribute(DesignerSerializationVisibility.Hidden)]
        [Browsable(false)]
        public GridTableModel TableModel
        {
            get
            {
                if (TableControl != null)
                    return TableControl.Model;
                return null;
            }
        }

        /// <summary>
        /// Gets the GridGroupDropArea control. Its purpose is to allow dragging header columns
        /// from the GridTableControl for user-interactive grouping of records.
        /// </summary>
        /// <remarks>
        /// The control is a child control of the <see cref="GroupDropPanel"/>.
        /// </remarks>
        [System.Xml.Serialization.XmlIgnore]
        [DesignerSerializationVisibilityAttribute(DesignerSerializationVisibility.Hidden)]
        [Browsable(false)]
        public GridGroupDropArea GridGroupDropArea
        {
            get
            {
                if (this.TableControl != null)
                    groupDropArea1.SortIconPlacement = this.TableControl.SortIconPlacement;
                return groupDropArea1;
            }
        }

        /// <summary>
        /// Get or Set of Skin Manager Interface
        /// </summary>
        private string style;
        string IVisualStyle.VisualTheme
        {
            get
            {
                return style;
            }
            set
            {
                style = value;
                this.TableModel.EnableLegacyStyle = false;
                switch (style)
                {
                    case "Office2007Blue":
                        GridVisualStyles = GridVisualStyles.Office2007Blue;
                        break;
                    case "Office2007Black":
                        GridVisualStyles = GridVisualStyles.Office2007Black;
                        break;
                    case "Office2007Silver":
                        GridVisualStyles = GridVisualStyles.Office2007Silver;
                        break;
                    case "Office2010Blue":
                        GridVisualStyles = GridVisualStyles.Office2010Blue;
                        break;
                    case "Office2010Black":
                        GridVisualStyles = GridVisualStyles.Office2010Black;
                        break;
                    case "Office2010Silver":
                        GridVisualStyles = GridVisualStyles.Office2010Silver;
                        break;
                }
            }
        }

        /// <summary>
        /// Gets or sets the VisualStyles (skins) like Office2007, Office2003
        /// </summary>
        [Category("Look and Feel")]
        [Description(@"Specifies the skin for the Grid")]
        [DefaultValue(GridVisualStyles.SystemTheme)]
        [NotifyParentProperty(true)]
        public GridVisualStyles GridVisualStyles
        {
            get
            {
                return TableOptions.GridVisualStyles;
                ////return TableModel.Options.GridVisualStyles;
            }

            set
            {
                this.BeginUpdate();
                GridVisualStyles visualStyles = this.TableModel.Options.GridVisualStyles;
                this.TableOptions.GridVisualStyles = value;
                if (value == GridVisualStyles.Metro)
                {
                    if (!this.TableControl.PersistAppearanceSettings)
                    {
                        isMetroSettingsApplied = true;
                        this.Table.DefaultRecordRowHeight = 25;
                        this.Table.DefaultColumnHeaderRowHeight = 25;
                        this.TableDescriptor.AllowNew = false;
                        engine.DefaultAppearance.AnyCell.TextColor = Color.FromArgb(91, 91, 91);
                        engine.DefaultAppearance.AnyCell.Font.Facename = "Segoe UI";
                        engine.DefaultAppearance.AnyCell.Font.Size = 9f;
                        engine.DefaultAppearance.ColumnHeaderCell.Font.Bold = true;
                        engine.DefaultAppearance.AnyRecordFieldCell.Borders.Bottom = new GridBorder(GridBorderStyle.Solid, Color.FromArgb(212, 212, 212), GridBorderWeight.ExtraThin);
                        engine.DefaultAppearance.AnyRecordFieldCell.Borders.Right = new GridBorder(GridBorderStyle.Solid, Color.FromArgb(212, 212, 212), GridBorderWeight.ExtraThin);
                        engine.DefaultAppearance.AnyRecordFieldCell.Font.Facename = "Segoe UI";
                        engine.DefaultAppearance.AnyRecordFieldCell.Font.Size = 9f;
                        engine.DefaultAppearance.AnyRecordFieldCell.TextColor = Color.FromArgb(91, 91, 91);
                        engine.DefaultAppearance.AnyGroupCell.Borders.Bottom = new GridBorder(GridBorderStyle.Solid, Color.FromArgb(212, 212, 212), GridBorderWeight.ExtraThin);
                        engine.DefaultAppearance.AnyGroupCell.Borders.Right = new GridBorder(GridBorderStyle.Solid, Color.FromArgb(212, 212, 212), GridBorderWeight.ExtraThin);
                        engine.DefaultAppearance.AnyGroupCell.BackColor = Color.FromArgb(235, 235, 235);
                        engine.DefaultAppearance.AnyGroupCell.TextColor = Color.FromArgb(138, 138, 138);
                        engine.DefaultAppearance.AnySummaryCell.BackColor = Color.FromArgb(208, 208, 208);
                        engine.DefaultAppearance.AnySummaryCell.Borders.Top = new GridBorder(GridBorderStyle.Solid, Color.FromArgb(35, 130, 195), GridBorderWeight.ExtraThick);
                        engine.DefaultAppearance.AnySummaryCell.Borders.Right = new GridBorder(GridBorderStyle.Solid, Color.FromArgb(212, 212, 212), GridBorderWeight.ExtraThin);
                        this.recordNavigationControl1.Font = new Font("Segoe UI", 9f, FontStyle.Bold);
                        this.recordNavigationControl1.ForeColor = Color.FromArgb(138, 138, 138);
                        this.TableModel.RowHeights[1] = 29;
                        if (this.splitter1 != null)
                            this.splitter1.BackColor = Color.FromArgb(94, 171, 222);
                    }
                    this.GridOfficeScrollBars = OfficeScrollBars.Metro;                    
                }

                foreach (Control control in this.groupDropPanel.Controls)
                {
                    if (control is GridGroupDropArea)
                    {
                        GridGroupDropArea ggd = control as GridGroupDropArea;
                        if (ggd.ThemesEnabled)
                            ggd.Model.Options.GridVisualStyles = value;
                        else
                            ggd.Model.Options.GridVisualStyles = GridVisualStyles.SystemTheme;
                        if (!this.TableModel.EnableLegacyStyle)
                            ggd.Model.TableStyle.Font.Facename = "Segoe UI";
                    }
                }

                if (captionGrid.ThemesEnabled)
                    captionGrid.Model.Options.GridVisualStyles = value;
                else
                    captionGrid.Model.Options.GridVisualStyles = GridVisualStyles.SystemTheme;

                if (!this.TableModel.EnableLegacyStyle && this.ThemesEnabled && value != GridVisualStyles.Metro)
                {
                    switch (value)
                    {
                        case GridVisualStyles.Office2007Blue:
                            this.GridOfficeScrollBars = OfficeScrollBars.Office2007;
                            this.Office2007ScrollBarsColorScheme = Office2007ColorScheme.Blue;
                            engine.DefaultAppearance.AnyRecordFieldCell.BackColor = Color.White;
                            engine.DefaultAppearance.AnyRecordFieldCell.TextColor = SystemColors.InactiveCaptionText;
                            this.TableOptions.SelectionBackColor = this.AlphaBlendSelectionColor = Color.FromArgb(153, 204, 255);
                            engine.DefaultAppearance.AnyCell.Font = new GridFontInfo(new System.Drawing.Font("Segoe UI", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0))));
                            break;
                        case GridVisualStyles.Office2007Black:
                            this.GridOfficeScrollBars = OfficeScrollBars.Office2007;
                            this.Office2007ScrollBarsColorScheme = Office2007ColorScheme.Black;
                            engine.DefaultAppearance.AnyRecordFieldCell.TextColor = SystemColors.InactiveCaptionText;
                            this.TableOptions.SelectionBackColor = this.AlphaBlendSelectionColor = Color.FromArgb(180, 180, 180);
                            engine.DefaultAppearance.AnyCell.Font = new GridFontInfo(new System.Drawing.Font("Segoe UI", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0))));
                            break;
                        case GridVisualStyles.Office2007Silver:
                            this.GridOfficeScrollBars = OfficeScrollBars.Office2007;
                            this.Office2007ScrollBarsColorScheme = Office2007ColorScheme.Silver;
                            engine.DefaultAppearance.AnyRecordFieldCell.TextColor = SystemColors.InactiveCaptionText;
                            this.TableOptions.SelectionBackColor = this.AlphaBlendSelectionColor = Color.FromArgb(180, 180, 180);
                            engine.DefaultAppearance.AnyCell.Font = new GridFontInfo(new System.Drawing.Font("Segoe UI", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0))));
                            break;
                        case GridVisualStyles.Office2010Blue:
                            this.GridOfficeScrollBars = OfficeScrollBars.Office2010;
                            this.Office2010ScrollBarsColorScheme = Office2010ColorScheme.Blue;
                            engine.DefaultAppearance.AnyRecordFieldCell.BackColor = Color.White;
                            engine.DefaultAppearance.AnyRecordFieldCell.TextColor = SystemColors.InactiveCaptionText;
                            this.TableOptions.SelectionBackColor = this.AlphaBlendSelectionColor = Color.FromArgb(153, 204, 255);
                            engine.DefaultAppearance.AnyCell.Font = new GridFontInfo(new System.Drawing.Font("Segoe UI", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0))));
                            break;
                        case GridVisualStyles.Office2010Black:
                            this.GridOfficeScrollBars = OfficeScrollBars.Office2010;
                            this.Office2010ScrollBarsColorScheme = Office2010ColorScheme.Black;
                            this.TableModel.BaseStylesMap.ColumnHeader.StyleInfo.TextColor = Color.White;
                            engine.DefaultAppearance.AnyRecordFieldCell.TextColor = SystemColors.InactiveCaptionText;
                            this.TableOptions.SelectionBackColor = this.AlphaBlendSelectionColor = Color.FromArgb(180, 180, 180);
                            engine.DefaultAppearance.AnyCell.Font = new GridFontInfo(new System.Drawing.Font("Segoe UI", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0))));
                            break;
                        case GridVisualStyles.Office2010Silver:
                            this.GridOfficeScrollBars = OfficeScrollBars.Office2010;
                            this.Office2010ScrollBarsColorScheme = Office2010ColorScheme.Silver;
                            engine.DefaultAppearance.AnyRecordFieldCell.TextColor = SystemColors.InactiveCaptionText;
                            this.TableOptions.SelectionBackColor = this.AlphaBlendSelectionColor = Color.FromArgb(180, 180, 180);
                            engine.DefaultAppearance.AnyCell.Font = new GridFontInfo(new System.Drawing.Font("Segoe UI", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0))));
                            break;

                        case GridVisualStyles.SystemTheme:
                            this.GridOfficeScrollBars = OfficeScrollBars.None;
                            engine.DefaultAppearance.AnyCell.Font = new GridFontInfo(new System.Drawing.Font("Segoe UI", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0))));
                            break;
                    }
                    if (isMetroSettingsApplied)
                    {
                        GridStyleInfo.Default.Font.Size = 8.25f;
                        engine.DefaultAppearance.ColumnHeaderCell.Font.Bold = false;
                        engine.DefaultAppearance.AnyHeaderCell.TextColor = (value == GridVisualStyles.Office2010Black) ? Color.White : SystemColors.WindowText;
                        this.TableModel.RowHeights[1] = 22;
                        engine.DefaultAppearance.ColumnHeaderCell.HorizontalAlignment = GridHorizontalAlignment.Center;
                        this.Table.DefaultColumnHeaderRowHeight = 25;
                        this.Table.DefaultRecordRowHeight = 22;
                        isMetroSettingsApplied = false;
                    }
                }
                else if (isMetroSettingsApplied && value != GridVisualStyles.Metro)
                {
					GridStyleInfo.Default.Font.Size = 8.25f;
                    engine.DefaultAppearance.ColumnHeaderCell.Font.Bold = false;
                    engine.DefaultAppearance.AnyHeaderCell.TextColor = (value == GridVisualStyles.Office2010Black) ? Color.White : SystemColors.WindowText;
                    this.TableModel.RowHeights[1] = 22;
                    isMetroSettingsApplied = false;
                }
                if (this.TableControl.DpiAware)
                {
                    this.Table.DefaultRecordRowHeight = 18;
                    this.Table.DefaultRecordRowHeight = this.RowHeightOnScaling();
                    this.Table.DefaultColumnHeaderRowHeight = this.ColumnHeaderRowHeightOnScaling();
                    this.Table.DefaultCaptionRowHeight = this.Table.DefaultRecordRowHeight;
                }
                this.EndUpdate(true);
            }
        }
        private int padding = 5;
        /// <summary>
        /// When DPI is greater than 100 then the DefaultRecordRowHeight will be set based on the font size.
        /// </summary>
        /// <returns>The Height Value</returns>
        private int RowHeightOnScaling()
        {
            if (this.TableModel.ActiveGridView != null)
                using (Graphics graph = this.TableModel.ActiveGridView.CreateGraphics())
                {
                    Font s = new Font(this.TableDescriptor.Appearance.AnyCell.Font.Facename, this.TableDescriptor.Appearance.AnyCell.Font.Size);
                    float heights = padding + s.GetHeight(graph.DpiY);
                    if (this.Table.DefaultRecordRowHeight > (int)heights)
                        return this.Table.DefaultRecordRowHeight;
                    return (int)Math.Round(heights, 0);
                }
            return this.Table.DefaultRecordRowHeight;
        }
        /// <summary>
        /// When DPI is greater than 100 then the DefaultColumnHeaderRowHeight will be set based on the font size.
        /// </summary>
        /// <returns>The Height Value</returns>
        private int ColumnHeaderRowHeightOnScaling()
        {
            if (this.TableModel.ActiveGridView != null)
                using (Graphics graph = this.TableModel.ActiveGridView.CreateGraphics())
                {
                    Font s = new Font(this.TableDescriptor.Appearance.AnyCell.Font.Facename, this.TableDescriptor.Appearance.AnyCell.Font.Size);
                    float heights = s.GetHeight(graph.DpiY) + padding + 8; //Header padding value is increased by 8 regardless of normal rows.
                    if (this.Table.DefaultColumnHeaderRowHeight > (int)heights)
                        return this.Table.DefaultColumnHeaderRowHeight;
                    return (int)Math.Round(heights, 0);
                }
            return this.Table.DefaultColumnHeaderRowHeight;
        }
        /// <summary>
        /// Sets the custom Metro Colors to the Grid.
        /// </summary>
        /// <param name="metroColor">Custom Metro Color</param>
        /// <param name="metroHoverColor">Custom MouseHover Color</param>
        /// <param name="metroColorPressed">Custom PushButtonPress Color</param>
        /// <param name="metroGroupBarColor">Custom GroupBar Color</param>
        public void SetMetroStyle(object metroColor, object metroHoverColor, object metroColorPressed, object metroGroupBarColor)
        {
            this.TableModel.Options.SetMetroStyles(metroColor, metroHoverColor, metroColorPressed, metroGroupBarColor);
            this.GridOfficeScrollBars = OfficeScrollBars.Metro;
            this.GridVisualStyles = GridVisualStyles.Metro;
        }

        /// <summary>
        /// set the color for grid when metro theme applied
        /// </summary>
        /// <param name="metroColor">Collection of metro colors</param>
        public void SetMetroStyle(GridMetroColors metroColor)
        {
            this.TableModel.Options.SetMetroStyles(metroColor);
            this.GridOfficeScrollBars = OfficeScrollBars.Metro;
            this.GridVisualStyles = GridVisualStyles.Metro;
        }
        /// <summary>
        /// [Deprecated] Gets or sets the enhanced VisualStyles (skins) like Office2010, Office2007, Office2003
        /// </summary>
        [Browsable(true),
        DefaultValue(ColorStyles.SystemTheme)]
        [Description("[Deprecated] Specifies look and feel skins for the Grid")]
        [Category("Look and Feel")]
        public ColorStyles ColorStyles
        {
            get
            {
                return colorStyles;
            }

            set
            {
                colorStyles = value;
                switch (value)
                {
                    case ColorStyles.Office2003:
                        this.GridVisualStyles = GridVisualStyles.Office2003;
                        break;
                    case ColorStyles.Office2007Blue:
                        this.GridVisualStyles = GridVisualStyles.Office2007Blue;
                        break;
                    case ColorStyles.Office2007Black:
                        this.GridVisualStyles = GridVisualStyles.Office2007Black;
                        break;
                    case ColorStyles.Office2007Silver:
                        this.GridVisualStyles = GridVisualStyles.Office2007Silver;
                        break;
                    case ColorStyles.Office2010Blue:
                        this.GridVisualStyles = GridVisualStyles.Office2010Blue;
                        break;
                    case ColorStyles.Office2010Black:
                        this.GridVisualStyles = GridVisualStyles.Office2010Black;
                        break;
                    case ColorStyles.Office2010Silver:
                        this.GridVisualStyles = GridVisualStyles.Office2010Silver;
                        break;
                }
            }
        }
        /// <summary>
        /// Updates the display after changes were made to schema or data in
        /// the engine or datasource.
        /// </summary>
        public new void Update()
        {
            this.Engine.GetSourceList();
            if (TableControl != null)
            {
                this.TableControl.Update();
                base.Update();
                this.PaintUpdatedRecordFields();
            }
        }

        /// <summary>
        /// Occurs for each cell before a <see cref="GridTableControl"/>
        /// starts painting and lets users customize the display of cells.
        /// </summary>
        [Description("Occurs for each cell before a GridTableControl starts painting."), Category("Appearance")]
        public event GridTableCellStyleInfoEventHandler QueryCellStyleInfo;

        /// <summary>
        /// Raises the <see cref="QueryCellStyleInfo"/> event.
        /// </summary>
        /// <param name="e">A <see cref="GridTableCellStyleInfoEventArgs" /> that contains the event data.</param>
        protected virtual void OnQueryCellStyleInfo(GridTableCellStyleInfoEventArgs e)
        {
            if (QueryCellStyleInfo != null)
            {
                QueryCellStyleInfo(this, e);
            }
        }

        ////        /// <summary>
        ////        /// Raises the <see cref="QueryCellStyleInfo"/> event.
        ////        /// </summary>
        ////        /// <param name="e">A <see cref="GridTableCellStyleInfoEventArgs" /> that contains the event data.</param>
        ////        internal void RaiseQueryCellStyleInfo(GridTableCellStyleInfoEventArgs e)
        ////        {
        ////         OnQueryCellStyleInfo(e);
        ////        }

        private void engine_QueryCellStyleInfo(object sender, GridTableCellStyleInfoEventArgs e)
        {
            OnQueryCellStyleInfo(e);
        }

        /// <summary>
        /// Overriden. Changes <see cref="System.Windows.Forms.CreateParams.Style"/> to show or hide scrollbars and also consider the control's
        /// <see cref="ScrollControl.BorderStyle"/> setting.
        /// </summary>
        protected override/*Control*/ CreateParams CreateParams
        {
            ////[SecurityPermission(SecurityAction.LinkDemand, UnmanagedCode=true)]
            get
            {
                System.Windows.Forms.CreateParams cp = base.CreateParams;

                switch (this.borderStyle)
                {
                    case BorderStyle.Fixed3D:
                        cp.ExStyle |= 0x200; // WS_EX_DLGFRAME
                        break;
                    case BorderStyle.FixedSingle:
                        cp.Style |= 0x800000; // WS_BORDER
                        break;
                }

                return cp;
            }
        }

        /// <summary>
        ///   <para>Gets / sets the border style of the control.</para>
        /// </summary>
        [Category(@"Grouping Control"),
        DefaultValue(BorderStyle.FixedSingle),
        Description(@"The border style of the control.")]
        public BorderStyle BorderStyle
        {
            get
            {
                return this.borderStyle;
            }

            set
            {
                if (this.borderStyle != value)
                {
                    OnPropertyChanging(new DescriptorPropertyChangedEventArgs("BorderStyle"));
                    if (!Enum.IsDefined(typeof(System.Windows.Forms.BorderStyle), value))
                    {
                        throw new InvalidEnumArgumentException("value", (int)value, typeof(BorderStyle));
                    }

                    this.borderStyle = value;
                    if (!stateOnly)
                    {
                        UpdateStyles();
                    }

                    OnPropertyChanged(new DescriptorPropertyChangedEventArgs("BorderStyle"));
                }
            }
        }

        private void WmNcPaint(ref Message msg)
        {
            bool themed = this is IThemedControl && XPThemes.IsThemedOS && ((IThemedControl)this).ThemesEnabled;
            if (themed && themedDrawing != null)
            {
                themedDrawing.DrawThemedBorderColor(this, ref msg);
            }

            base.WndProc(ref msg);
        }

        const int WM_NCPAINT = 133; // 0x0085
        const int WM_MOUSEWHEEL = 522; // 0x020a
        /// <override/>
        ////[SecurityPermission(SecurityAction.LinkDemand, UnmanagedCode=true)]
        protected override void WndProc(ref Message msg)
        {
            switch (msg.Msg)
            {
                case WM_NCPAINT:
                    this.WmNcPaint(ref msg);
                    break;
                case WM_MOUSEWHEEL:
                    if (this.TableControl != null && this.TableControl.IsActiveControl && this.TableControl.VScrollBar.Enabled && ((this.TableControl.VScrollBehavior != GridScrollbarMode.Disabled) || (this.TableControl.UseSharedScrollBars && this.TableControl.VScrollBehavior == GridScrollbarMode.Shared)))
                        return;
                    goto default;
                default:
                    base.WndProc(ref msg);
                    break;
            }
        }

        private ThemedWindowDrawing _themedDrawing = null;

        private ThemedWindowDrawing themedDrawing
        {
            get
            {
                if (XPThemes.IsThemedOS && XPThemes.IsAppThemed)
                {
                    if (_themedDrawing == null)
                    {
                        _themedDrawing = new ThemedWindowDrawing();
                    }
                }

                return this._themedDrawing;
            }
        }

        /// <summary>
        /// Gets / sets whether ScrollTips should be displayed when
        /// the user drags the horizontal scrollbar's thumb bar.
        /// </summary>
        [Category("Grouping Control")]
        [DefaultValue(false), Description("Specifies whether ScrollTips should be displayed when dragging the horizontal scrollbar's thumb bar")]
        public bool HorizontalScrollTips
        {
            get
            {
                return this.TableControl.HorizontalScrollTips;
            }

            set
            {
                if (this.TableControl.HorizontalScrollTips != value)
                {
                    this.OnPropertyChanging(new DescriptorPropertyChangedEventArgs("HorizontalScrollTips"));
                    this.TableControl.HorizontalScrollTips = value;
                    this.OnPropertyChanged(new DescriptorPropertyChangedEventArgs("HorizontalScrollTips"));
                }
            }
        }

        /// <summary>
        /// Gets / sets whether the grid should scroll and update its contents
        /// while the user drags the horizontal scrollbar's thumb bar.
        /// </summary>
        [Category("Grouping Control")]
        [DefaultValue(true), Description("Specifies whether the grid should scroll and update its contents while dragging the horizontal scrollbar's thumb bar")]
        public bool HorizontalThumbTrack
        {
            get
            {
                return this.TableControl.HorizontalThumbTrack;
            }

            set
            {
                if (this.TableControl.HorizontalThumbTrack != value)
                {
                    this.OnPropertyChanging(new DescriptorPropertyChangedEventArgs("HorizontalThumbTrack"));
                    this.TableControl.HorizontalThumbTrack = value;
                    this.OnPropertyChanged(new DescriptorPropertyChangedEventArgs("HorizontalThumbTrack"));
                }
            }
        }

        /// <summary>
        /// Gets / sets whether ScrollTips should be displayed when
        /// the user drags the vertical scrollbar's thumb bar.
        /// </summary>
        [Category("Grouping Control")]
        [DefaultValue(false), Description("Specifies whether ScrollTips should be displayed when dragging the vertical scrollbar's thumb bar")]
        public bool VerticalScrollTips
        {
            get
            {
                return this.TableControl.VerticalScrollTips;
            }

            set
            {
                if (this.TableControl.VerticalScrollTips != value)
                {
                    this.OnPropertyChanging(new DescriptorPropertyChangedEventArgs("VerticalScrollTips"));
                    this.TableControl.VerticalScrollTips = value;
                    this.OnPropertyChanged(new DescriptorPropertyChangedEventArgs("VerticalScrollTips"));
                }
            }
        }

        /// <summary>
        /// Gets / sets whether the grid should scroll and update its contents
        /// while the user drags the vertical scrollbar's thumb bar.
        /// </summary>
        [Category("Grouping Control")]
        [DefaultValue(true), Description("Specifies whether the grid should scroll and update its contents while dragging the vertical scrollbar's thumb bar")]
        public bool VerticalThumbTrack
        {
            get
            {
                return this.TableControl.VerticalThumbTrack;
            }

            set
            {
                if (this.TableControl.VerticalThumbTrack != value)
                {
                    this.OnPropertyChanging(new DescriptorPropertyChangedEventArgs("VerticalThumbTrack"));
                    this.TableControl.VerticalThumbTrack = value;
                    this.OnPropertyChanged(new DescriptorPropertyChangedEventArgs("VerticalThumbTrack"));
                }
            }
        }


        bool accessibilityEnabled = false;

        /// <summary>
        /// Gets or sets a value indicating whether the control should enable its Accessibility support.
        /// </summary>
        [Browsable(true),
        Category("Behavior"),
        Description("Specifies if the control should enable its Accessibility support."),
        DefaultValue(false)]
        public bool AccessibilityEnabled
        {
            get
            {
                return accessibilityEnabled;
            }

            set
            {
                accessibilityEnabled = value;
            }
        }


        /// <summary>
        /// Gets / sets whether ToolTips should be shown when the user
        /// hovers the mouse over elements of the <see cref="RecordNavigationBar"/>.
        /// </summary>
        [Description("Specifies whether ToolTips should be shown when the user hovers the mouse over elements of the RecordNavigationBar")]
        [Category("Grouping Control")]
        [DefaultValue(false)]
        public bool ShowNavigationBarToolTips
        {
            get
            {
                if (!this.IsHandleCreated)
                {
                    return recordNavigationControlShowToolTips;
                }

                return this.RecordNavigationControl.ShowToolTips;
            }

            set
            {
                if (ShowNavigationBarToolTips != value)
                {
                    this.OnPropertyChanging(new DescriptorPropertyChangedEventArgs("ShowNavigationBarToolTips"));
                    recordNavigationControlShowToolTips = value;
                    if (this.IsHandleCreated)
                    {
                        this.RecordNavigationControl.ShowToolTips = value;
                    }

                    this.OnPropertyChanged(new DescriptorPropertyChangedEventArgs("ShowNavigationBarToolTips"));
                }
            }
        }

        bool recordNavigationControlShowToolTips = false;

        [Category("Grouping Control")]
        [DefaultValue(DynamicSplitBars.None)]
        internal DynamicSplitBars SplitBars
        {
            get
            {
                return this.RecordNavigationControl.SplitBars;
            }

            set
            {
                this.RecordNavigationControl.SplitBars = value;
            }
        }

        GridTable _FindTable(GridTable table, string name)
        {
            int n = table.RelatedTables.IndexOf(name);
            if (n != -1)
            {
                return (GridTable)table.RelatedTables[n];
            }

            foreach (GridTable relatedTable in table.RelatedTables)
            {
                GridTable found = _FindTable(relatedTable, name);
                if (found != null)
                {
                    return found;
                }
            }

            return null;
        }

        /// <summary>
        /// Returns the main <see cref="Table"/> or a <see cref="GridTable"/> of any nested relation that matches the
        /// specified name.
        /// </summary>
        /// <param name="name">The name of the table to search.</param>
        /// <returns>The table or NULL if not found.</returns>
        public GridTable GetTable(string name)
        {
            if (name == null || name == string.Empty || TableDescriptor.Name == name)
            {
                return Table;
            }

            return _FindTable(this.Table, name);
        }

        GridTableDescriptor _FindTableDescriptor(GridTableDescriptor tableDescriptor, string name)
        {
            int n = tableDescriptor.Relations.IndexOf(name);
            if (n != -1)
            {
                return tableDescriptor.Relations[n].ChildTableDescriptor;
            }

            foreach (GridRelationDescriptor relatedTableDescriptor in tableDescriptor.Relations)
            {
                GridTableDescriptor found = _FindTableDescriptor(relatedTableDescriptor.ChildTableDescriptor, name);
                if (found != null)
                {
                    return found;
                }
            }

            return null;
        }

        /// <summary>
        /// Returns the main <see cref="TableDescriptor"/> or a <see cref="GridTableDescriptor"/> of any nested relation that matches the
        /// specified name.
        /// </summary>
        /// <param name="name">The name of the table descriptor to search.</param>
        /// <returns>The table descriptor or NULL if not found.</returns>
        public GridTableDescriptor GetTableDescriptor(string name)
        {
            if (TableDescriptor.Name == name)
            {
                return TableDescriptor;
            }

            return _FindTableDescriptor(this.TableDescriptor, name);
        }

        /// <summary>
        /// Returns the main <see cref="TableControl"/> or a <see cref="GridNestedTableControl"/> of any nested relation that matches the
        /// specified name.
        /// </summary>
        /// <param name="name">The name of the table control to search.</param>
        /// <returns>The table control or NULL if not found.</returns>
        public GridTableControl GetTableControl(string name)
        {
            if (name == null || name == string.Empty || TableDescriptor.Name == name)
            {
                return TableControl;
            }

            GridTable relatedTable = GetTable(name);

            if (relatedTable != null)
            {
                Stack stack = new Stack();
                do
                {
                    stack.Push(relatedTable);
                    if (Object.ReferenceEquals(Table, relatedTable.RelationParentTable))
                    {
                        break;
                    }

                    relatedTable = relatedTable.RelationParentTable;
                }
                while (relatedTable != null);

                GridTableControl tableControl = this.TableControl;

                while (stack.Count > 0)
                {
                    relatedTable = (GridTable)stack.Pop();
                    string cellType = "RT" + relatedTable.TableDescriptor.Name;
                    GridNestedTableControlCellRenderer renderer = tableControl.CellRenderers[cellType] as GridNestedTableControlCellRenderer;
                    if (renderer != null)
                        tableControl = renderer.Control;
                }

                return tableControl;
            }

            return null;
        }

        /// <summary>
        /// Returns the main <see cref="TableModel"/> or a <see cref="GridTableModel"/>
        /// of any nested relation that matches the specified name.
        /// </summary>
        /// <param name="name">The name of the table model to search.</param>
        /// <returns>The table model or null if not found.</returns>
        public GridTableModel GetTableModel(string name)
        {
            if (name == null || name == string.Empty || TableDescriptor.Name == name)
            {
                return TableModel;
            }

            GridTableControl tableControl = GetTableControl(name);
            if (tableControl != null)
            {
                return tableControl.Model;
            }

            return null;
        }

        /// <overload>
        /// Adds a row of columns to the <see cref="GridGroupDropArea"/>.
        /// </overload>
        /// <summary>
        /// Adds a row of columns to the <see cref="GridGroupDropArea"/>. Users have
        /// to manually add rows for nested relations to allow end-users grouping
        /// nested relations.
        /// </summary>
        /// <param name="name">The name of the relation for which to add support for
        /// grouping.</param>
        public void AddGroupDropArea(string name)
        {
            GridTable relatedTable = GetTable(name);
            if (relatedTable != null)
            {
                AddGroupDropArea(relatedTable);
            }
        }

        private GridGroupDropArea groupDropArea2;
        /// <summary>
        /// Adds a row of columns to the <see cref="GridGroupDropArea"/>. Users have
        /// to manually add rows for nested relations to allow end-users grouping
        /// of nested relations.
        /// </summary>
        /// <param name="relatedTable">The nested table for which to add support for
        /// grouping.</param>
        public void AddGroupDropArea(GridTable relatedTable)
        {
            if (relatedTable.RelationParentTable == null
                || relatedTable.RelationParentTable.TableModel == null
                || !relatedTable.RelationParentTable.TableModel.CellModels.ContainsKey("RT" + relatedTable.TableDescriptor.Name))
            {
                return;
            }

            GridNestedTableControlCellModel rt0 = (GridNestedTableControlCellModel)relatedTable.RelationParentTable.TableModel.CellModels["RT" + relatedTable.TableDescriptor.Name];
            groupDropArea2 = this.CreateGroupDropArea(null, rt0.RelatedTableModel.GroupDropAreaModel);

            int oldHeight = this.GroupDropPanel.Height;

            ////
            //// groupDropArea2
            ////
            ////groupDropArea2.Anchor = System.Windows.Forms.AnchorStyles.Bottom;
            groupDropArea2.Model.GridTableModel.HierarchicalGroupDropArea = groupDropArea1.Model.GridTableModel.HierarchicalGroupDropArea;
            groupDropArea2.AllowRemove = groupDropArea1.AllowRemove;
            groupDropArea2.Location = new System.Drawing.Point(0, oldHeight - groupDropArea1.Model.RowHeights[1]);
            groupDropArea2.Name = "groupDropArea2";
            groupDropArea2.TabIndex = this.GridGroupDropArea.TabIndex + 1;
            groupDropArea2.ThemesEnabled = this.GridGroupDropArea.ThemesEnabled;
                ////Update GridVisualStyles
                groupDropArea2.Model.Options.GridVisualStyles = this.GridGroupDropArea.Model.Options.GridVisualStyles;
                groupDropArea2.Model.Options.GridVisualStylesDrawing = this.GridGroupDropArea.Model.Options.GridVisualStylesDrawing;

            GridStyleInfo standard = groupDropArea2.Model.BaseStylesMap["Standard"].StyleInfo;
            Color clrBack = Color.Empty;
            Color headerBorderTop = Color.Empty;
            Color headerBorderLeft = Color.Empty;
                if (groupDropArea2.Model.Options.GridVisualStylesDrawing.GetGroupDropAreaColors(out clrBack, out headerBorderTop, out headerBorderLeft))
                {
                    standard.BackColor = clrBack;
                    groupDropArea2.Model.Properties.BackgroundColor = clrBack;
                }
            groupDropArea2.TabStop = false;
            ////groupDropArea2.Model.RowHeights[1] = 1;
            ////groupDropArea2.Model.RowHeights[3] = 1;
            groupDropArea2.Size = new Size(this.tableControl1.Width, this.GridGroupDropArea.Height);
            GroupDropPanel.Controls.Add(groupDropArea2);

            this.GroupDropPanel.Height += groupDropArea2.Height;

            groupDropArea2.Model.Table = relatedTable;

            groupDropArea2.Initialize();
            ////groupDropArea2.Model.Rows.DefaultSize = 0; //RowHeights[1] = 0;
            groupDropArea2.Visible = true;
            relatedTable.TableDescriptor.GroupedColumns.Changed += new ListPropertyChangedEventHandler(GroupedColumns_Changed);
            relatedTable.Disposed += new EventHandler(groupDropArea_relatedTable_Disposed);
            groupDropArea2.TreeLineColor = groupDropArea1.TreeLineColor;
            groupDropArea2.TreeLinePlacement = groupDropArea1.TreeLinePlacement;
            //// resize to fit table name
            groupDropArea2.Model.ColWidths.ResizeToFit(GridRangeInfo.Cols(1, 2));
        }

        private void groupDropArea_relatedTable_Disposed(object sender, EventArgs e)
        {
            Table table = (Table)sender;
            table.Disposed -= new EventHandler(groupDropArea_relatedTable_Disposed);

            if (GroupDropPanel == null)
            {
                return;
            }

            if (Syncfusion.Grouping.Engine.VerboseEnsureObjectLifeTime)
            {
                TraceUtil.TraceCurrentMethodInfo(table);
            }

            int height = 0;

            //// dispose old group drop area that have references to old data
            //// can't use foreach here, the iterator gets lost when elements are removed.
            for (int idx = GroupDropPanel.Controls.Count; idx > 0; idx--)
            {
                Control c = GroupDropPanel.Controls[idx - 1];
                if (c != null && c != GridGroupDropArea)
                {
                    GridGroupDropArea groupDropArea2 = (GridGroupDropArea)c;
                    if (c == this.GridGroupDropArea)
                    {
                        groupDropArea2.Model.Table = null;
                    }
                    else if (groupDropArea2.Model.Table == table)
                    {
                        height += c.Height;
                        ////groupDropArea2.Model.Dispose();
                        groupDropArea2.Dispose();
                    }
                    else
                    {
                        ////Console.WriteLine(c.Name + " still alive ");
                    }
                }
            }

            this.GroupDropPanel.Height -= height;
        }

        /// <summary>
        /// The default <see cref="GridTableCellAppearance"/> with default <see cref="GridTableCellStyleInfo"/>
        /// information for all cell elements in the control. This property lets you control almost any aspect of
        /// the appearance of the grouping grid like cell backcolor, font, or the cell type.
        /// </summary>
        [Category("Look and Feel")]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
        [Description("A gateway to control almost any aspect Grouping Grid's appearance")]
        public GridTableCellAppearance Appearance
        {
            get
            {
                return Engine.Appearance;
            }

            set
            {
                Engine.Appearance = value;
            }
        }

        /// <summary>
        /// Determines whether <see cref="Appearance"/> has been modified
        /// and its contents should be serialized at design-time.
        /// </summary>
        /// <returns>True if contents were changed; False otherwise.</returns>
        public bool ShouldSerializeAppearance()
        {
            return Engine.ShouldSerializeAppearance();
        }

        /// <summary>
        /// Discards any changes for the <see cref="Appearance"/> object.
        /// </summary>
        public void ResetAppearance()
        {
            Engine.ResetAppearance();
            Repaint();
        }

        /// <summary>
        /// Defines default appearance settings for the grid at runtime. These settings will
        /// not be serialized or written to good and can be used if you want to specify default
        /// settings for a derived GridGroupingControl. Any appearance element 
        /// in the engine will inherit these settings.
        /// </summary>
        [System.Xml.Serialization.XmlIgnore]
        [DesignerSerializationVisibilityAttribute(DesignerSerializationVisibility.Hidden)]
        [Browsable(false)]
        public GridTableCellAppearance DefaultAppearance
        {
            get
            {
                return Engine.DefaultAppearance;
            }
        }

        /// <summary>
        /// A collection of <see cref="GridPropertyTypeDefaultStyle"/> with default
        /// <see cref="GridTableCellStyleInfo"/> information for RecordFieldCell elements
        /// based on the columns System.Type. Each basic type has default style information
        /// registered with this collection.
        /// </summary>
        /// <remarks>
        /// The collection contains pre-defined settings such as HorizontalAlignment for numbers and
        /// cell type (e.g. check box for boolean).<para/>
        /// GridPropertyTypeDefaultStyle settings have less precedence in styles inheritance than
        /// <see cref="Appearance"/> styles. <para/>
        /// Note: Changes you make to this collection do not get serialized; you will need
        /// to reapply any changes even if you read back the schema from an XML file.
        /// </remarks>
        [System.Xml.Serialization.XmlIgnore]
        [DesignerSerializationVisibilityAttribute(DesignerSerializationVisibility.Hidden)]
        [Browsable(false)]
        public GridPropertyTypeDefaultStyleCollection PropertyTypeDefaultStyles
        {
            get
            {
                return Engine.PropertyTypeDefaultStyles;
            }
        }

        /// <summary>
        /// Lets you control the look of inner groups like whether the Caption Row is visible, or what CaptionText is.
        /// </summary>
        [Category("Look and Feel")]
        [DesignerSerializationVisibilityAttribute(DesignerSerializationVisibility.Content)]
        [Browsable(true)]
        public GridGroupOptionsStyleInfo ChildGroupOptions
        {
            get
            {
                return Engine.ChildGroupOptions;
            }

            set
            {
                ChildGroupOptions.CopyFrom(value);
            }
        }

        /// <summary>
        /// Determines whether <see cref="ChildGroupOptions"/> were modified
        /// and whether their contents should be serialized at design-time.
        /// </summary>
        /// <returns>True if contents are changed; False otherwise.</returns>
        public bool ShouldSerializeChildGroupOptions()
        {
            return Engine.ShouldSerializeChildGroupOptions();
        }

        /// <summary>
        /// Discards any changes for the <see cref="ChildGroupOptions"/> object.
        /// </summary>
        public void ResetChildGroupOptions()
        {
            Engine.ResetChildGroupOptions();
            Repaint();
        }

        /// <summary>
        /// Lets you set table-wide properties like the width of the indent column, or whether header rows should be visible.
        /// </summary>
        [Category("Look and Feel")]
        [DesignerSerializationVisibilityAttribute(DesignerSerializationVisibility.Content)]
        [Browsable(true)]
        public GridTableOptionsStyleInfo TableOptions
        {
            get
            {
                return Engine.TableOptions;
            }

            set
            {
                TableOptions.CopyFrom(value);
            }
        }

        /// <summary>
        /// Determines whether <see cref="TableOptions"/> were modified
        /// and whether their contents should be serialized at design-time.
        /// </summary>
        /// <returns>True if contents were changed; False otherwise.</returns>
        public bool ShouldSerializeTableOptions()
        {
            return Engine.ShouldSerializeTableOptions();
        }

        /// <summary>
        /// Discards any changes for the <see cref="TableOptions"/> object.
        /// </summary>
        public void ResetTableOptions()
        {
            Engine.ResetTableOptions();
            Repaint();
        }

        /// <summary>
        /// Lets you control the look of the top most group such as whether the Caption Row is visible, or what CaptionText is.
        /// </summary>
        [Category("Look and Feel")]
        [DesignerSerializationVisibilityAttribute(DesignerSerializationVisibility.Content)]
        [Browsable(true)]
        public GridGroupOptionsStyleInfo TopLevelGroupOptions
        {
            get
            {
                return Engine.TopLevelGroupOptions;
            }

            set
            {
                TopLevelGroupOptions.CopyFrom(value);
            }
        }

        /// <summary>
        /// Determines whether <see cref="TopLevelGroupOptions"/> were modified
        /// and whether their contents should be serialized at design-time.
        /// </summary>
        /// <returns>True if contents were changed; False otherwise.</returns>
        public bool ShouldSerializeTopLevelGroupOptions()
        {
            return Engine.ShouldSerializeTopLevelGroupOptions();
        }

        /// <summary>
        /// Discards any changes for the <see cref="TopLevelGroupOptions"/> object.
        /// </summary>
        public void ResetTopLevelGroupOptions()
        {
            Engine.ResetTopLevelGroupOptions();
            Repaint();
        }

        /// <summary>
        /// Lets you control the look of the topmost group of nested tables such as whether the Caption Row is visible, or what CaptionText is.
        /// </summary>
        [Category("Look and Feel")]
        [DesignerSerializationVisibilityAttribute(DesignerSerializationVisibility.Content)]
        [Browsable(true)]
        public GridGroupOptionsStyleInfo NestedTableGroupOptions
        {
            get
            {
                return Engine.NestedTableGroupOptions;
            }

            set
            {
                NestedTableGroupOptions.CopyFrom(value);
            }
        }

        /// <summary>
        /// Determines whether <see cref="NestedTableGroupOptions"/> were modified
        /// and whether their contents should be serialized at design-time.
        /// </summary>
        /// <returns>True if contents were changed; False otherwise.</returns>
        public bool ShouldSerializeNestedTableGroupOptions()
        {
            return Engine.ShouldSerializeNestedTableGroupOptions();
        }

        /// <summary>
        /// Discards any changes for the <see cref="NestedTableGroupOptions"/> object.
        /// </summary>
        public void ResetNestedTableGroupOptions()
        {
            Engine.ResetNestedTableGroupOptions();
            Repaint();
        }

        #region ISupportInitialize Members

        GridEngine initEngine;

        /// <summary>
        /// Begins the initialization of the control that is used on a form or used by another component. The initialization occurs at run-time.
        /// </summary>
        public void BeginInit()
        {
            //// Create a new engine. get_Engine will return this temporary engine instead. For initEngine
            //// no DataSource and DataMember will be set. It will simply be initialized offline.
            //// That way we can avoid the engine being reinitialized multiple times when the inner
            //// collections are changed and various version counters keep increasing.
            initEngine = CreateEngineForBeginInit();
            initEngine.InitializeFrom(this.engine);
            initEngine.DefaultAppearance.InitializeFrom(this.engine.DefaultAppearance);

            Engine.BeginInit();
        }

        /// <summary>
        /// Creates an engine object that is used temporarily only after BeginInit was called from
        /// within the parent forms InitializeComponent method.
        /// </summary>
        /// <returns>Returns the GridEngine</returns>
        protected virtual GridEngine CreateEngineForBeginInit()
        {
            return new GridEngine();
        }

        /// <override/>
        protected override void OnParentChanged(EventArgs e)
        {
            if (this.InDesigner)
            {
                //// be verbose during design-time only. (only works with debug builds of library).
                Syncfusion.Grouping.Engine.VerboseEnsureObjectLifeTime = true;
                Syncfusion.Grouping.RelationDescriptorCollection.VerboseEnsureInitialized = true;
            }

            base.OnParentChanged(e);
        }

        /// <summary>
        /// Ends the initialization of the control that is used on a form or used by another component. The initialization occurs at run-time.
        /// </summary>
        public void EndInit()
        {
            Engine.EndInit();

            //// Save offline engine and reset initEngine.
            GridEngine offLineEngine = initEngine;
            initEngine = null;  //// get_Engine will now return the correct Engine object.

            //// Now set the DataSource and DataMember.
            Engine.DataMember = dataMember;
            Engine.DataSource = dataSource;

            dataMember = string.Empty;
            dataSource = null;

            //// This will initialize all schema settings that were defined in InitializeComponent.
            Engine.InitializeFrom(offLineEngine);
            Engine.DefaultAppearance.InitializeFrom(offLineEngine.DefaultAppearance);
            Engine.engineDefaultStyle.CopyFrom(offLineEngine.engineDefaultStyle);
            offLineEngine.Dispose();

            //// Delay certain settings - either in HandleCreated event or shortly after
            if (this.IsHandleCreated)
            {
                ApplyCachedSettings();
            }
        }

        #endregion

        ////        /// <summary>
        ////        /// Maintains performance while changes are made to the engine or datasource
        ////        /// one at a time by preventing the control from drawing until the EndUpdate
        ////        /// method is called.
        ////        /// </summary>
        ////        public virtual void BeginUpdate()
        ////        {
        ////            if (this.updateCount++ == 0)
        ////            {
        ////    this.paintPending = false;
        ////            }
        ////        }
        ////
        ////        ///// <summary>
        ////        ///// Resumes painting the control after painting is suspended by the BeginUpdate method.
        ////        ///// </summary>
        ////        public virtual void EndUpdate()
        ////        {
        ////            if (this.updateCount > 0)
        ////            {
        ////    this.updateCount--;
        ////    if (this.updateCount == 0)
        ////    {
        ////        if (paintPending)
        ////            Refresh();
        ////        else
        ////            Update();
        ////    }
        ////            }
        ////        }
        ////
        ////        bool Updating
        ////        {
        ////            get
        ////            {
        ////    return updateCount > 0;
        ////            }
        ////        }

        ////int updateCount = 0;
        ////bool paintPending = false;

        void Repaint()
        {
            if (!stateOnly && this.tableControl1 != null)
             { 
                ////if (!inInit && DesignMode)
          
                if(this.engine != null)
                    this.engine.BumpVersion();
                ////this.Table.CountersDirty = true;

                ////this.TableControl.synchronizeGridShouldInvalidate = true;
                this.TableControl.ViewLayout.Reset();
                this.TableControl.synchronizeGridShouldUpdateColumnWidths = true;
                this.TableControl.synchronizeGridShouldnextUpdateScrollBars = true;
                this.GridGroupDropArea.Invalidate();
                this.TableControl.Invalidate();
            }
        }

        private void Appearance_Changing(object sender, GridTableCellStyleInfoChangedEventArgs e)
        {
            ////this.OnPropertyChanging(new DescriptorPropertyChangedEventArgs("Appearance", e));
        }

        private void NestedTableGroupOptions_Changing(object sender, Syncfusion.Styles.StyleChangedEventArgs e)
        {
            ////this.OnPropertyChanging(new DescriptorPropertyChangedEventArgs("NestedTableGroupOptions", e));
        }

        private void ChildGroupOptions_Changing(object sender, Syncfusion.Styles.StyleChangedEventArgs e)
        {
            ////this.OnPropertyChanging(new DescriptorPropertyChangedEventArgs("ChildGroupOptions", e));
        }

        private void PropertyTypeDefaultStyles_Changing(object sender, Syncfusion.Collections.ListPropertyChangedEventArgs e)
        {
            ////this.OnPropertyChanging(new DescriptorPropertyChangedEventArgs("PropertyTypeDefaultStyles", e));
        }

        private void TopLevelGroupOptions_Changing(object sender, Syncfusion.Styles.StyleChangedEventArgs e)
        {
            ////this.OnPropertyChanging(new DescriptorPropertyChangedEventArgs("TopLevelGroupOptions", e));
        }

        private void Appearance_Changed(object sender, GridTableCellStyleInfoChangedEventArgs e)
        {
            ////this.OnPropertyChanged(new DescriptorPropertyChangedEventArgs("Appearance", e));
        }

        private void NestedTableGroupOptions_Changed(object sender, Syncfusion.Styles.StyleChangedEventArgs e)
        {
            ////this.OnPropertyChanged(new DescriptorPropertyChangedEventArgs("NestedTableGroupOptions", e));
        }

        private void GroupOptions_Changed(object sender, Syncfusion.Styles.StyleChangedEventArgs e)
        {
            ////this.OnPropertyChanged(new DescriptorPropertyChangedEventArgs("GroupOptions", e));
        }

        private void PropertyTypeDefaultStyles_Changed(object sender, Syncfusion.Collections.ListPropertyChangedEventArgs e)
        {
            ////this.OnPropertyChanged(new DescriptorPropertyChangedEventArgs("PropertyTypeDefaultStyles", e));
        }

        private void TopLevelGroupOptions_Changed(object sender, Syncfusion.Styles.StyleChangedEventArgs e)
        {
            ////this.OnPropertyChanged(new DescriptorPropertyChangedEventArgs("TopLevelGroupOptions", e));
        }

        private void TableDescriptor_PropertyChanging(object sender, DescriptorPropertyChangedEventArgs e)
        {
            ////this.OnPropertyChanging(new DescriptorPropertyChangedEventArgs("TableDescriptor", e));
        }

        private void TableDescriptor_PropertyChanged(object sender, DescriptorPropertyChangedEventArgs e)
        {
            ////            if (/*e.PropertyName == "SummaryRows" &&*/ e.Inner is ListPropertyChangedEventArgs)
            ////            {
            ////    ListPropertyChangedEventArgs inner = (ListPropertyChangedEventArgs) e.Inner;
            ////    switch (inner.Action)
            ////    {
            ////        case ListPropertyChangedType.Add:
            ////        case ListPropertyChangedType.Insert:
            ////        case ListPropertyChangedType.Remove:
            ////            this.Table.CountersDirty = true;
            ////            break;
            ////    }
            ////            } //// TableDescriptor already takes care on above ...
            Repaint();
            ////this.OnPropertyChanged(new DescriptorPropertyChangedEventArgs("TableDescriptor", e));
        }

        private void engine_PropertyChanging(object sender, DescriptorPropertyChangedEventArgs e)
        {
            this.OnPropertyChanging(e);
        }

        private void engine_PropertyChanged(object sender, DescriptorPropertyChangedEventArgs e)
        {
            Repaint();
            this.OnPropertyChanged(e);
        }

        private void engine_DataMemberChanged(object sender, EventArgs e)
        {
            Repaint();
        }

        private void engine_DataSourceChanged(object sender, EventArgs e)
        {
            if (this.forceDisposeOnResetDataSource && this.engine.DataSource == null)
            {   
                this.engine.Reset();
                GC.WaitForPendingFinalizers();
#if SyncfusionFramework4_0
                GC.Collect(GC.MaxGeneration, GCCollectionMode.Optimized);
#elif SyncfusionFramework3_5
                GC.Collect(GC.MaxGeneration, GCCollectionMode.Optimized);
#elif SyncfusionFramework2_0
                GC.Collect();
#elif SyncfusionFramework1_1
                GC.Collect();
#elif SyncfusionFramework1_0
                GC.Collect();
#else
                GC.Collect();
#endif
            }
            
            Repaint();
        }

        void SortedColumns_Changed(object sender, ListPropertyChangedEventArgs e)
        {
            if (e.Item != null)
                ReinitializeMerging();
        }

        void Table_GroupExpanded(object sender, GroupEventArgs e)
        {
            ReinitializeMerging();
        }

        GridMergeCellDirection initialDir; GridMergeCellsMode initialMode;
        /// <summary>
        /// Reinitializes merging cells if the view layout is changed
        /// </summary>
        private void ReinitializeMerging()
        {
            List<GridColumnDescriptor> colCollection = new List<GridColumnDescriptor>();
            if (this.tableControl1 != null && this.TableModel != null)
            {
                this.TableControl.BeginUpdate();
                foreach (GridColumnDescriptor col in this.TableDescriptor.Columns)
                {
                    if (col.Appearance.AnyRecordFieldCell.MergeCell != GridMergeCellDirection.None)//|| this.TableModel.Options.MergeCellsMode != GridMergeCellsMode.None)
                    {
                        colCollection.Add(col);
                        initialDir = this.TableDescriptor.Columns[col.MappingName].Appearance.AnyRecordFieldCell.MergeCell;
                        initialMode = this.TableModel.Options.MergeCellsMode;
                        this.TableDescriptor.Columns[col.MappingName].Appearance.AnyRecordFieldCell.MergeCell = GridMergeCellDirection.None;
                        this.TableDescriptor.Columns[col.MappingName].Appearance.AnyRecordFieldCell.ResetMergeCell();
                        this.TableModel.Options.MergeCellsMode = GridMergeCellsMode.None;

                        this.TableDescriptor.Columns[col.MappingName].Appearance.AnyRecordFieldCell.MergeCell = initialDir;
                        this.TableModel.Options.MergeCellsMode = initialMode;
                        GridRangeInfo mergeRange = this.TableModel.Options.MergeCellsLayout == GridMergeCellsLayout.VisibleRange ?
                            this.TableControl.ViewLayout.VisibleCellsRange : this.TableControl.GridCellsRange;
                        this.TableModel.MergeCells.EvaluateMergeCells(mergeRange);
                    }
                }
                this.TableControl.EndUpdate(true);
            }
        }

        private bool optimizeFilterPerformance = false;

        /// <summary>
        /// Gets or sets a value indicating whether use a optimization filter in GridGroupingControl
        /// </summary>
        [
        Browsable(false),
        Description("To optimize the performance in GridGroupingControl while using Filters"),
        DefaultValue(false)
        ]
        public bool OptimizeFilterPerformance
        {
            get
            {
                return this.optimizeFilterPerformance;
            }
            set
            {
                if (this.optimizeFilterPerformance != value)
                {                    
                    this.optimizeFilterPerformance = value;         
                    if(this.TableDescriptor.summaries_savedColumnsVersion!=-1)
                        this.TableDescriptor.summaries_savedColumnsVersion += 1;
                }
            }
        }

        private bool forceDisposeOnResetDataSource = false;

        /// <summary>
        /// Indicates whether to force the GC forcibly when Datasource is changed.
        /// </summary>
        [
        Browsable(false),
        Description("Indicates whether to force the GC forcibly when Datasource is changed."),
        DefaultValue(false)
        ]
        public bool ForceDisposeOnResetDataSource
        {
            get
            {
                return this.forceDisposeOnResetDataSource;
            }
            set
            {
                if (this.forceDisposeOnResetDataSource != value)
                {
                    this.forceDisposeOnResetDataSource = value;
                }
            }
        }
        
        ////        private void engine_SourceListChanging(object sender, CancelEventArgs e)
        ////        {
        ////            this.OnPropertyChanging(new DescriptorPropertyChangedEventArgs("SourceList", e));
        ////        }

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

        private bool filterRuntimeProperties = false;

        /// <summary>
        /// Property FilterRuntimeProperties (bool)
        /// </summary>
        [System.Xml.Serialization.XmlIgnore]
        [DesignerSerializationVisibilityAttribute(DesignerSerializationVisibility.Hidden)]
        [Browsable(false)]
        [EditorBrowsable(EditorBrowsableState.Advanced)]
        public bool FilterRuntimeProperties
        {
            get
            {
                return this.filterRuntimeProperties;
            }

            set
            {
                this.filterRuntimeProperties = value;
            }
        }

        int typePropertiesCount = -1;
        internal ArrayList tdPropertiesCache = new ArrayList();
        internal PropertyDescriptorCollection pdcCache = null;
        PropertyDescriptorCollection typeProperties = null;
        int pdcPropertiesCount = -1;

        PropertyDescriptorCollection ICustomTypeDescriptor.GetProperties(Attribute[] attributes)
        {
            PropertyDescriptorCollection pdc = TypeDescriptor.GetProperties(this, new Attribute[0], true);

            //// Do this every time to make sure to include extended properties.
            if (typeProperties == null || pdc.Count != pdcPropertiesCount)
            {
                pdcPropertiesCount = pdc.Count;
                if (this.FilterRuntimeProperties)
                {
                    ArrayList al = new ArrayList();
                    foreach (PropertyDescriptor pd in pdc)
                    {
                        if ((pd.ComponentType == typeof(GridGroupingControl)
                            && pd.Category != "Data")
                            || this.GetType().IsAssignableFrom(pd.ComponentType))
                        {
                            //// custom properties of a derived control ...
                            al.Add(pd);
                        }
                    }

                    typeProperties = new PropertyDescriptorCollection((PropertyDescriptor[])al.ToArray(typeof(PropertyDescriptor)));
                }
                else
                {
                    typeProperties = pdc;
                }
            }

            //// Relation TableDescriptors
            ArrayList tdProperties = new ArrayList();
            Attribute[] att = new Attribute[] 
            {
              new BrowsableAttribute(true),
              new DesignerSerializationVisibilityAttribute(DesignerSerializationVisibility.Hidden),
              new System.Xml.Serialization.XmlIgnoreAttribute(),
              new RefreshPropertiesAttribute(RefreshProperties.All),
              new CategoryAttribute("TableDescriptors")
                      };

            string name = TableDescriptor.Name;
            ////            if (name == string.Empty)
            ////            {
            ////    if (this.engine.Table.HasSourceList)
            ////        name = ListUtil.GetListName(engine.Table.SourceList);
            ////    if (name == string.Empty)
            ////        name = "TableDescriptor";
            ////            }
            ////            if (typeProperties.Find(name, true) != null)
            ////    name = "Schema";
            ////AddRelations(tdProperties, this.TableDescriptor, name, att);
            foreach (RelationDescriptor rd in TableDescriptor.Relations)
            {
                AddRelations(tdProperties, rd.ChildTableDescriptor, name + rd.Name, att);
            }

            //// Compare PropertyDescriptors
            bool isEqual = true;
            if (tdPropertiesCache.Count != tdProperties.Count || typePropertiesCount != typeProperties.Count)
            {
                isEqual = false;
            }
            else
            {
                for (int n = 0; n < tdPropertiesCache.Count; n++)
                {
                    if (((PropertyDescriptor)tdPropertiesCache[n]).Name != ((PropertyDescriptor)tdProperties[n]).Name)
                    {
                        isEqual = false;
                        break;
                    }
                }
            }

            typePropertiesCount = typeProperties.Count;
            tdPropertiesCache = tdProperties;

            if (pdcCache == null || !isEqual)
            {
                //// Type
                PropertyDescriptor[] pdArray = new PropertyDescriptor[typeProperties.Count + tdProperties.Count];
                typeProperties.CopyTo(pdArray, 0);
                tdProperties.CopyTo(pdArray, typeProperties.Count);
                pdcCache = new PropertyDescriptorCollection(pdArray);
            }

            return pdcCache;
        }

        void AddRelations(ArrayList pds, TableDescriptor tableDescriptor, string name, Attribute[] att)
        {
            pds.Add(new GridTableDescriptorPropertyDescriptor(name, tableDescriptor, att));
            foreach (RelationDescriptor rd in tableDescriptor.Relations)
            {
                AddRelations(pds, rd.ChildTableDescriptor, name + rd.Name, att);
            }
        }

        object ICustomTypeDescriptor.GetPropertyOwner(PropertyDescriptor pd)
        {
            return this;
        }

        #endregion

        /// <summary>
        /// Gets the default size of the control.
        /// </summary>
        protected override Size DefaultSize
        {
            get
            {
                return new Size(130, 80);
            }
        }

        /// <summary>
        /// Loads engine settings from an XML stream.
        /// </summary>
        /// <param name="xr">The XML reader stream.</param>
        public void ApplyXmlSchema(XmlReader xr)
        {
            Engine.InitializeFrom(GridEngine.CreateFromXml(xr));
        }

        /// <summary>
        /// Writes engines settings to an XML stream.
        /// </summary>
        /// <param name="xw">The XML writer stream.</param>
        public void WriteXmlSchema(XmlWriter xw)
        {
            Engine.WriteXml(xw);
        }

        /// <summary>
        /// Writes <see cref="GridGroupingLookAndFeel"/> state to an XML stream.
        /// </summary>
        /// <param name="xw">The XML writer stream.</param>
        public void WriteXmlLookAndFeel(XmlWriter xw)
        {
            GridGroupingLookAndFeel lookAndFeel = new GridGroupingLookAndFeel();
            lookAndFeel.InitializeFrom(this);
            lookAndFeel.WriteXml(xw);
        }

        /// <summary>
        /// Loads <see cref="GridGroupingLookAndFeel"/> state from an XML stream.
        /// </summary>
        /// <param name="xr">The XML reader stream.</param>
        public void ApplyXmlLookAndFeel(XmlReader xr)
        {
            GridGroupingLookAndFeel lookAndFeel = GridGroupingLookAndFeel.CreateFromXml(xr);
            lookAndFeel.ApplyTo(this);
        }

        /// <summary>
        /// Discards changes, refreshes, counters, and summaries and redraws the grid.
        /// </summary>
        public void Reinitialize()
        {
            TableControl.Reinitialize();
            Repaint();
        }

        /// <summary>
        /// Determines whether the specified key is a regular input key or a special key that requires preprocessing.
        /// </summary>
        /// <param name="keyData">One of the <see cref="T:System.Windows.Forms.Keys"/> values.</param>
        /// <returns>
        /// true if the specified key is a regular input key; otherwise, false.
        /// </returns>
        /// <override/>
        protected override bool IsInputKey(Keys keyData)
        {
            return true;
        }

        /// <override/>
        protected override void OnKeyDown(KeyEventArgs e)
        {
            base.OnKeyDown(e);
        }

        //// event GridRaiseQueryCustomSummary QueryCustomSummary

        /// <summary>
        /// Occurs for each GridSummaryColumnDescriptor before the <see cref="SummaryDescriptor"/> is determined. You must handle this event if you specified <see cref="SummaryType.Custom"/> as <see cref="GridSummaryColumnDescriptor.SummaryType"/>.
        /// </summary>
        [Description("Occurs for each GridSummaryColumnDescriptor before the SummaryDescriptor is determined. You must handle this event if you specified SummaryType.Custom as GridSummaryColumnDescriptor.SummaryType"), Category("Data")]
        public event GridQueryCustomSummaryEventHandler QueryCustomSummary;

        /// <summary>
        /// Raises the <see cref="QueryCustomSummary"/> event.
        /// </summary>
        /// <param name="e">A <see cref="GridQueryCustomSummaryEventArgs" /> that contains the event data.</param>
        protected virtual void OnQueryCustomSummary(GridQueryCustomSummaryEventArgs e)
        {
            if (QueryCustomSummary != null)
            {
                QueryCustomSummary(this, e);
            }
        }

        ////        internal void RaiseQueryCustomSummary(GridQueryCustomSummaryEventArgs e)
        ////        {
        ////            OnQueryCustomSummary(e);
        ////        }

        private void engine_QueryCustomSummary(object sender, GridQueryCustomSummaryEventArgs e)
        {
            OnQueryCustomSummary(e);
        }

        /// <summary>
        /// Occurs when a record is checked whether it meets filter criteria and should appear visible in the table's DisplayElements.
        /// </summary>
        [Description("Occurs when a record is checked whether it meets filter criteria and should appear visible in the table's DisplayElements."), Category("Filter")]
        public event QueryRecordMeetsFilterCriteriaEventHandler QueryRecordMeetsFilterCriteria;

        /// <summary>
        /// Raises the <see cref="QueryRecordMeetsFilterCriteria"/> event.
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

        ////        internal void RaiseQueryRecordMeetsFilterCriteria(QueryRecordMeetsFilterCriteriaEventArgs e)
        ////        {
        ////            OnQueryRecordMeetsFilterCriteria(e);
        ////        }

        private void engine_QueryRecordMeetsFilterCriteria(object sender, QueryRecordMeetsFilterCriteriaEventArgs e)
        {
            OnQueryRecordMeetsFilterCriteria(e);
        }

        /// <override/>
        /// <summary>Specifies the font used to display text in the grid.</summary>
        [Category(@"Grouping Control")]
        public override Font Font
        {
            get
            {
                return base.Font;
            }

            set
            {
                if (Font != value)
                {
                    this.OnPropertyChanging(new DescriptorPropertyChangedEventArgs("Font"));
                    base.Font = value;
                    this.OnPropertyChanged(new DescriptorPropertyChangedEventArgs("Font"));
                    Repaint();
                }
            }
        }

        /// <override/>
        /// <summary>Specifies the background color for the grid.</summary>
        [Category(@"Grouping Control")]
        public override Color BackColor
        {
            get
            {
                return base.BackColor;
            }

            set
            {
                if (BackColor != value)
                {
                    this.OnPropertyChanging(new DescriptorPropertyChangedEventArgs("BackColor"));
                    base.BackColor = value;
                    this.OnPropertyChanged(new DescriptorPropertyChangedEventArgs("BackColor"));
                    TypeDescriptor.Refresh(this);
                    Repaint();
                }
            }
        }

        /// <override/>
        /// <summary>Specifies whether the text should be displayed from right to left.</summary>
        [Category(@"Grouping Control")]
        public override RightToLeft RightToLeft
        {
            get
            {
                return base.RightToLeft;
            }

            set
            {
                if (RightToLeft != value)
                {
                    this.OnPropertyChanging(new DescriptorPropertyChangedEventArgs("RightToLeft"));
                    base.RightToLeft = value;
                    this.OnPropertyChanged(new DescriptorPropertyChangedEventArgs("RightToLeft"));
                    TypeDescriptor.Refresh(this);
                    Repaint();
                }
            }
        }

        /// <override/>
        /// <summary>Specifies text color.</summary>
        [Category(@"Grouping Control")]
        public override Color ForeColor
        {
            get
            {
                return base.ForeColor;
            }

            set
            {
                if (ForeColor != value)
                {
                    this.OnPropertyChanging(new DescriptorPropertyChangedEventArgs("ForeColor"));
                    base.ForeColor = value;
                    this.OnPropertyChanged(new DescriptorPropertyChangedEventArgs("ForeColor"));
                    Repaint();
                }
            }
        }

        /// <override/>
        protected override void OnParentFontChanged(EventArgs e)
        {
            base.OnParentFontChanged(e);

            GridStyleInfo s = new GridStyleInfo();
            s.Font = new GridFontInfo(Font);
            Engine.UpdateTableOptionsDefault(Font);
            TypeDescriptor.Refresh(this);
            Repaint();
        }

        /// <summary>
        /// Font changed event
        /// </summary>
        /// <param name="e"></param>
        protected override void OnFontChanged(EventArgs e)
        {
            GridStyleInfo s = new GridStyleInfo();
            s.Font = new GridFontInfo(Font);
            Engine.DefaultAppearance.AnyCell.Font = new GridFontInfo(Font);
            Engine.UpdateTableOptionsDefault(Font);
            base.OnFontChanged(e);
        }

        /// <override/>
        protected override void OnForeColorChanged(EventArgs e)
        {
            Engine.DefaultAppearance.AnyCell.TextColor = ForeColor;
            base.OnForeColorChanged(e);
        }

        /// <override/>
        protected override void OnBackColorChanged(EventArgs e)
        {
            Engine.DefaultAppearance.AnyRecordFieldCell.BackColor = this.BackColor;
            Engine.DefaultAppearance.AnySummaryCell.BackColor = this.BackColor;
            Engine.DefaultAppearance.AnyIndentCell.BackColor = this.BackColor;
            Engine.DefaultAppearance.AnyPreviewCell.BackColor = this.BackColor;
            Engine.DefaultAppearance.EmptyCell.BackColor = this.BackColor;

            base.OnBackColorChanged(e);
        }

        private void recordNavigationControl1_PaneClosing(object sender, SplitterPaneEventArgs e)
        {
            if (Object.ReferenceEquals(e.Control, this.tableControl1))
            {
                this.tableControl1 = null;
                //// Calling TableControl will reinitialize reference later.
            }

            GridTableControl tableControl = (GridTableControl)e.Control;
            tableControl.GridControlBaseEventsTarget.Dispose();
            tableControl.GridControlBaseEventsTarget = null;
        }

        internal class GridControlBaseEventsTarget : IGridControlBaseEventsTarget
        {
            GridTableControl tableControl;
            GridGroupingControl owner;

            public GridControlBaseEventsTarget(GridTableControl tableControl, GridGroupingControl owner)
            {
                this.tableControl = tableControl;
                this.owner = owner;
            }

            /// <summary>
            /// Disposes of the object.
            /// </summary>
            public void Dispose()
            {
                this.tableControl = null;
                this.owner = null;
                GC.SuppressFinalize(this);
            }

            #region IGridControlBaseEventsTarget Members

            public void OnModelChanged(EventArgs e)
            {
                //// owner.OnTableControlModelChanged(new GridTableControlEventArgs(tableControl, e));
            }

            public void OnVScrollPixelPosChanged(GridScrollPositionChangedEventArgs e)
            {
                owner.OnTableControlVScrollPixelPosChanged(new GridTableControlScrollPositionChangedEventArgs(tableControl, e));
            }

            public void OnGridControlMouseMove(CancelMouseEventArgs e)
            {
                //// owner.OnTableControlControlMouseMove(new GridTableControlCancelMouseEventArgs(tableControl, e));
            }

            public void OnSplitterPaneClosing(EventArgs e)
            {
                //// owner.OnTableControlSplitterPaneClosing(new GridTableControlEventArgs(tableControl, e));
            }

            public void OnCurrentCellKeyDown(KeyEventArgs e)
            {
                owner.OnTableControlCurrentCellKeyDown(new GridTableControlKeyEventArgs(tableControl, e));
            }

            public void OnCellMouseUp(GridCellMouseEventArgs e)
            {
                owner.OnTableControlCellMouseUp(new GridTableControlCellMouseEventArgs(tableControl, e));
            }

            public void OnCellMouseMove(GridCellMouseEventArgs e)
            {
                owner.OnTableControlCellMouseMove(new GridTableControlCellMouseEventArgs(tableControl, e));
            }

            public void OnMouseUp(MouseEventArgs e)
            {
                //// owner.OnTableControlMouseUp(new GridTableControlMouseEventArgs(tableControl, e));
            }

            public void OnScrollControlMouseDown(CancelMouseEventArgs e)
            {
                // owner.OnTableControlScrollControlMouseDown(new GridTableControlCancelMouseEventArgs(tableControl, e));
            }

            public void OnInvalidated(InvalidateEventArgs e)
            {
                // owner.OnTableControlInvalidated(new GridTableControlInvalidateEventArgs(tableControl, e));
            }

            public void OnCurrentCellValidated(EventArgs e)
            {
                owner.OnTableControlCurrentCellValidated(new GridTableControlEventArgs(tableControl, e));
            }

            public void OnCurrentCellChanged(EventArgs e)
            {
                owner.OnTableControlCurrentCellChanged(new GridTableControlEventArgs(tableControl, e));
            }

            public void OnWindowScrolled(ScrollWindowEventArgs e)
            {
                //// TODO:  Add GridControlBaseEventsTarget.OnWindowScrolled implementation
            }

            public void OnQueryCanOleDragRange(GridQueryCanOleDragRangeEventArgs e)
            {
                ////owner.OnTableControlQueryCanOleDragRange(new GridTableControlQueryCanOleDragRangeEventArgs(tableControl, e));
            }

            public void OnCurrentCellDeactivateFailed(EventArgs e)
            {
                owner.OnTableControlCurrentCellDeactivateFailed(new GridTableControlEventArgs(tableControl, e));
            }

            public void OnGridControlMouseUp(CancelMouseEventArgs e)
            {
                //// owner.OnTableControlControlMouseUp(new GridTableControlCancelMouseEventArgs(tableControl, e));
            }

            public void OnKeyDown(KeyEventArgs e)
            {
                owner.OnTableControlKeyDown(new GridTableControlKeyEventArgs(tableControl, e));
            }

            public void OnLeftColChanged(GridRowColIndexChangedEventArgs e)
            {
                owner.OnTableControlLeftColChanged(new GridTableControlRowColIndexChangedEventArgs(tableControl, e));
            }

            public void OnCurrentCellDeleting(CancelEventArgs e)
            {
                owner.OnTableControlCurrentCellDeleting(new GridTableControlCancelEventArgs(tableControl, e));
            }

            public void OnPaint(PaintEventArgs pe)
            {
                ////owner.OnTableControlPaint(new GridTableControlPaintEventArgs(tableControl, e));
            }

            public void OnCurrentCellValidating(CancelEventArgs e)
            {
                owner.OnTableControlCurrentCellValidating(new GridTableControlCancelEventArgs(tableControl, e));
            }

            public void OnCurrentCellControlKeyMessage(GridCurrentCellControlKeyMessageEventArgs e)
            {
                owner.OnTableControlCurrentCellControlKeyMessage(new GridTableControlCurrentCellControlKeyMessageEventArgs(tableControl, e));
            }

            public void OnCurrentCellActivated(EventArgs e)
            {
                owner.OnTableControlCurrentCellActivated(new GridTableControlEventArgs(tableControl, e));
            }

            public void OnSelectionFrameChanging(GraphicsEventArgs e)
            {
                ////owner.OnTableControlSelectionFrameChanging(new GridTableControlGraphicsEventArgs(tableControl, e));
            }

            public void OnCurrentCellInitializeControlText(GridCurrentCellInitializeControlTextEventArgs e)
            {
                owner.OnTableControlCurrentCellInitializeControlText(new GridTableControlCurrentCellInitializeControlTextEventArgs(tableControl, e));
            }

            public void OnCurrentCellErrorMessage(GridCurrentCellErrorMessageEventArgs e)
            {
                owner.OnTableControlCurrentCellErrorMessage(new GridTableControlCurrentCellErrorMessageEventArgs(tableControl, e));
            }

            public void OnKeyUp(KeyEventArgs e)
            {
                owner.OnTableControlKeyUp(new GridTableControlKeyEventArgs(tableControl, e));
            }

            public void OnCurrentCellRejectedChanges(EventArgs e)
            {
                owner.OnTableControlCurrentCellRejectedChanges(new GridTableControlEventArgs(tableControl, e));
            }

            public void OnCurrentCellChanging(CancelEventArgs e)
            {
                owner.OnTableControlCurrentCellChanging(new GridTableControlCancelEventArgs(tableControl, e));
            }

            public void OnDrawCellButtonBackground(GridDrawCellButtonBackgroundEventArgs e)
            {
                owner.OnTableControlDrawCellButtonBackground(new GridTableControlDrawCellButtonBackgroundEventArgs(tableControl, e));
            }

            public void OnCurrentCellControlDoubleClick(ControlEventArgs e)
            {
                owner.OnTableControlCurrentCellControlDoubleClick(new GridTableControlControlEventArgs(tableControl, e));
            }

            public void OnScrollInfoChanged(EventArgs e)
            {
                //// owner.OnTableControlScrollInfoChanged(new GridTableControlEventArgs(tableControl, e));
            }

            public void OnPrepareViewStyleInfo(GridPrepareViewStyleInfoEventArgs e)
            {
                owner.OnTableControlPrepareViewStyleInfo(new GridTableControlPrepareViewStyleInfoEventArgs(tableControl, e));
            }

            public void OnDrawCellButton(GridDrawCellButtonEventArgs e)
            {
                owner.OnTableControlDrawCellButton(new GridTableControlDrawCellButtonEventArgs(tableControl, e));
            }

            public void OnDrawCellBackground(GridDrawCellBackgroundEventArgs e)
            {
                owner.OnTableControlDrawCellBackground(new GridTableControlDrawCellBackgroundEventArgs(tableControl, e));
            }

            public void OnDrawCellDisplayText(GridDrawCellDisplayTextEventArgs e)
            {
                owner.OnTableControlDrawCellDisplayText(new GridTableControlDrawCellDisplayTextEventArgs(tableControl, e));
            }

            public void OnCurrentCellDeactivating(CancelEventArgs e)
            {
                owner.OnTableControlCurrentCellDeactivating(new GridTableControlCancelEventArgs(tableControl, e));
            }

            public void OnCellMouseHoverEnter(GridCellMouseEventArgs e)
            {
                owner.OnTableControlCellMouseHoverEnter(new GridTableControlCellMouseEventArgs(tableControl, e));
            }

            public void OnMoveCurrentCellDirection(GridMoveCurrentCellDirectionEventArgs e)
            {
                owner.OnTableControlMoveCurrentCellDirection(new GridTableControlMoveCurrentCellDirectionEventArgs(tableControl, e));
            }

            public void OnQueryNextCurrentCellPosition(GridQueryNextCurrentCellPositionEventArgs e)
            {
                owner.OnTableControlQueryNextCurrentCellPosition(new GridTableControlQueryNextCurrentCellPositionEventArgs(tableControl, e));
            }

            public void OnTopRowChanged(GridRowColIndexChangedEventArgs e)
            {
                owner.OnTableControlTopRowChanged(new GridTableControlRowColIndexChangedEventArgs(tableControl, e));
            }

            public void OnCurrentCellKeyPress(KeyPressEventArgs e)
            {
                owner.OnTableControlCurrentCellKeyPress(new GridTableControlKeyPressEventArgs(tableControl, e));
            }

            public void OnResizingColumns(GridResizingColumnsEventArgs e)
            {
                owner.OnTableControlResizingColumns(new GridTableControlResizingColumnsEventArgs(tableControl, e));
            }

            public void OnCellCursor(GridCellCursorEventArgs e)
            {
                owner.OnTableControlCellCursor(new GridTableControlCellCursorEventArgs(tableControl, e));
            }

            public void OnQueryScrollCellInView(GridQueryScrollCellInViewEventArgs e)
            {
                owner.OnTableControlQueryScrollCellInView(new GridTableControlQueryScrollCellInViewEventArgs(tableControl, e));
            }

            public void OnGridValidating(CancelEventArgs e)
            {
                //// owner.OnTableControlValidating(new GridTableControlCancelEventArgs(tableControl, e));
            }

            public void OnSelectionFrameChanged(GraphicsEventArgs e)
            {
                //// owner.OnTableControlSelectionFrameChanged(new GridTableControlGraphicsEventArgs(tableControl, e));
            }

            public void OnLayout(LayoutEventArgs le)
            {
                //// owner.OnTableControlLayout(new GridTableControlLayoutEventArgs(tableControl, e));
            }

            public void OnPushButtonClick(GridCellPushButtonClickEventArgs e)
            {
                owner.OnTableControlPushButtonClick(new GridTableControlCellPushButtonClickEventArgs(tableControl, e));
            }

            public void OnCurrentCellActivating(GridCurrentCellActivatingEventArgs e)
            {
                owner.OnTableControlCurrentCellActivating(new GridTableControlCurrentCellActivatingEventArgs(tableControl, e));
            }

            public void OnResizingRows(GridResizingRowsEventArgs e)
            {
                owner.OnTableControlResizingRows(new GridTableControlResizingRowsEventArgs(tableControl, e));
            }

            public void OnCurrentCellAcceptedChanges(CancelEventArgs e)
            {
                owner.OnTableControlCurrentCellAcceptedChanges(new GridTableControlCancelEventArgs(tableControl, e));
            }

            public void OnCurrentCellValidateString(GridCurrentCellValidateStringEventArgs e)
            {
                owner.OnTableControlCurrentCellValidateString(new GridTableControlCurrentCellValidateStringEventArgs(tableControl, e));
            }

            public void OnCurrentCellMoved(GridCurrentCellMovedEventArgs e)
            {
                owner.OnTableControlCurrentCellMoved(new GridTableControlCurrentCellMovedEventArgs(tableControl, e));
            }

            public void OnCurrentCellActivateFailed(GridCurrentCellActivateFailedEventArgs e)
            {
                owner.OnTableControlCurrentCellActivateFailed(new GridTableControlCurrentCellActivateFailedEventArgs(tableControl, e));
            }

            public void OnCurrentCellKeyUp(KeyEventArgs e)
            {
                owner.OnTableControlCurrentCellKeyUp(new GridTableControlKeyEventArgs(tableControl, e));
            }

            public void OnCellHitTest(GridCellHitTestEventArgs e)
            {
                owner.OnTableControlCellHitTest(new GridTableControlCellHitTestEventArgs(tableControl, e));
            }

            public void OnMouseDown(MouseEventArgs e)
            {
                owner.OnTableControlMouseDown(new GridTableControlMouseEventArgs(tableControl, e));
            }

            public void OnCellMouseHover(GridCellMouseEventArgs e)
            {
                owner.OnTableControlCellMouseHover(new GridTableControlCellMouseEventArgs(tableControl, e));
            }

            public void OnVScrollPixelPosChanging(GridScrollPositionChangingEventArgs e)
            {
                owner.OnTableControlVScrollPixelPosChanging(new GridTableControlScrollPositionChangingEventArgs(tableControl, e));
            }

            public void OnCellDrawn(GridDrawCellEventArgs e)
            {
                owner.OnTableControlCellDrawn(new GridTableControlDrawCellEventArgs(tableControl, e));
            }

            public void OnCurrentCellMoving(GridCurrentCellMovingEventArgs e)
            {
                owner.OnTableControlCurrentCellMoving(new GridTableControlCurrentCellMovingEventArgs(tableControl, e));
            }

            public void OnCurrentCellShowedDropDown(EventArgs e)
            {
                owner.OnTableControlCurrentCellShowedDropDown(new GridTableControlEventArgs(tableControl, e));
            }

            public void OnSelectionDragging(GridSelectionDragEventArgs e)
            {
                //// owner.OnTableControlSelectionDragging(new GridTableControlSelectionDragEventArgs(tableControl, e));
            }

            public void OnCurrentCellShowingDropDown(GridCurrentCellShowingDropDownEventArgs e)
            {
                owner.OnTableControlCurrentCellShowingDropDown(new GridTableControlCurrentCellShowingDropDownEventArgs(tableControl, e));
            }

            public void OnSizeChanged(EventArgs e)
            {
                //// owner.OnTableControlSizeChanged(new GridTableControlEventArgs(tableControl, e));
            }

            public void OnCellClick(GridCellClickEventArgs e)
            {
                owner.OnTableControlCellClick(new GridTableControlCellClickEventArgs(tableControl, e));
            }

            public void OnMouseActivating(CancelEventArgs e)
            {
                //// owner.OnTableControlMouseActivating(new GridTableControlCancelEventArgs(tableControl, e));
            }

            public void OnCellButtonClicked(GridCellButtonClickedEventArgs e)
            {
                owner.OnTableControlCellButtonClicked(new GridTableControlCellButtonClickedEventArgs(tableControl, e));
            }

            public void OnDrawCell(GridDrawCellEventArgs e)
            {
                owner.OnTableControlDrawCell(new GridTableControlDrawCellEventArgs(tableControl, e));
            }

            public void OnVisibleChanged(EventArgs e)
            {
                //// owner.OnTableControlVisibleChanged(new GridTableControlEventArgs(tableControl, e));
            }

            public void OnCheckBoxClick(GridCellClickEventArgs e)
            {
                owner.OnTableControlCheckBoxClick(new GridTableControlCellClickEventArgs(tableControl, e));
            }

            public void OnLeftColChanging(GridRowColIndexChangingEventArgs e)
            {
                owner.OnTableControlLeftColChanging(new GridTableControlRowColIndexChangingEventArgs(tableControl, e));
            }

            public void OnSupportsTransparentBackColorChanged(EventArgs e)
            {
                //// owner.OnTableControlSupportsTransparentBackColorChanged(new GridTableControlEventArgs(tableControl, e));
            }

            public void OnCellDoubleClick(GridCellClickEventArgs e)
            {
                owner.OnTableControlCellDoubleClick(new GridTableControlCellClickEventArgs(tableControl, e));
            }

            public void OnPrintingModeChanged(EventArgs e)
            {
                // owner.OnTableControlPrintingModeChanged(new GridTableControlEventArgs(tableControl, e));
            }

            public void OnGridBoundsChanged(EventArgs e)
            {
                //// owner.OnTableControlBoundsChanged(new GridTableControlEventArgs(tableControl, e));
            }

            public void OnEnter(EventArgs e)
            {
                //// owner.OnTableControlEnter(new GridTableControlEventArgs(tableControl, e));
            }

            public void OnCurrentCellControlLostFocus(ControlEventArgs e)
            {
                owner.OnTableControlCurrentCellControlLostFocus(new GridTableControlControlEventArgs(tableControl, e));
            }

            public void OnWrapCellNextControlInForm(GridWrapCellNextControlInFormEventArgs e)
            {
                owner.OnTableControlWrapCellNextControlInForm(new GridTableControlWrapCellNextControlInFormEventArgs(tableControl, e));
            }

            public void OnValidating(CancelEventArgs e)
            {
                //// owner.OnTableControlValidating(new GridTableControlCancelEventArgs(tableControl, e));
            }

            public void OnMouseMove(MouseEventArgs e)
            {
                //// owner.OnTableControlMouseMove(new GridTableControlMouseEventArgs(tableControl, e));
            }

            public void OnDrawCurrentCellBorder(GridDrawCurrentCellBorderEventArgs e)
            {
                owner.OnTableControlDrawCurrentCellBorder(new GridTableControlDrawCurrentCellBorderEventArgs(tableControl, e));
            }

            public void OnGridControlMouseDown(CancelMouseEventArgs e)
            {
                //// owner.OnTableControlControlMouseDown(new GridTableControlCancelMouseEventArgs(tableControl, e));
            }

            public void OnCurrentCellEditingComplete(EventArgs e)
            {
                owner.OnTableControlCurrentCellEditingComplete(new GridTableControlEventArgs(tableControl, e));
            }

            public void OnThemeChanged(EventArgs e)
            {
                //// owner.OnTableControlThemeChanged(new GridTableControlEventArgs(tableControl, e));
            }

            public void OnTopRowChanging(GridRowColIndexChangingEventArgs e)
            {
                owner.OnTableControlTopRowChanging(new GridTableControlRowColIndexChangingEventArgs(tableControl, e));
            }

            public void OnCellCancelMode(GridCellMouseEventArgs e)
            {
                owner.OnTableControlCellCancelMode(new GridTableControlCellMouseEventArgs(tableControl, e));
            }

            public void OnHScrollPixelPosChanging(GridScrollPositionChangingEventArgs e)
            {
                owner.OnTableControlHScrollPixelPosChanging(new GridTableControlScrollPositionChangingEventArgs(tableControl, e));
            }

            public void OnScrollTipFeedback(ScrollTipFeedbackEventArgs e)
            {
                owner.OnTableControlScrollTipFeedback(new GridTableControlScrollTipFeedbackEventArgs(tableControl, e));
            }

            public void OnSelectionDragged(GridSelectionDragEventArgs e)
            {
                //// owner.OnTableControlSelectionDragged(new GridTableControlSelectionDragEventArgs(tableControl, e));
            }

            public void OnCurrentCellControlGotFocus(ControlEventArgs e)
            {
                owner.OnTableControlCurrentCellControlGotFocus(new GridTableControlControlEventArgs(tableControl, e));
            }

            public void OnCellMouseDown(GridCellMouseEventArgs e)
            {
                owner.OnTableControlCellMouseDown(new GridTableControlCellMouseEventArgs(tableControl, e));
            }

            public void OnCurrentCellCloseDropDown(PopupClosedEventArgs e)
            {
                owner.OnTableControlCurrentCellCloseDropDown(new GridTableControlPopupClosedEventArgs(tableControl, e));
            }

            public void OnCurrentCellDeactivated(GridCurrentCellDeactivatedEventArgs e)
            {
                owner.OnTableControlCurrentCellDeactivated(new GridTableControlCurrentCellDeactivatedEventArgs(tableControl, e));
            }

            public void OnCurrentCellStartEditing(CancelEventArgs e)
            {
                owner.OnTableControlCurrentCellStartEditing(new GridTableControlCancelEventArgs(tableControl, e));
            }

            public void OnCurrentCellConfirmChangesFailed(EventArgs e)
            {
                owner.OnTableControlCurrentCellConfirmChangesFailed(new GridTableControlEventArgs(tableControl, e));
            }

            public void OnHScrollPixelPosChanged(GridScrollPositionChangedEventArgs e)
            {
                owner.OnTableControlHScrollPixelPosChanged(new GridTableControlScrollPositionChangedEventArgs(tableControl, e));
            }

            public void OnDeactivated(EventArgs e)
            {
                //// owner.OnTableControlDeactivated(new GridTableControlEventArgs(tableControl, e));
            }

            public void OnCellMouseHoverLeave(GridCellMouseEventArgs e)
            {
                owner.OnTableControlCellMouseHoverLeave(new GridTableControlCellMouseEventArgs(tableControl, e));
            }

            public void OnCurrentCellMoveFailed(GridCurrentCellMoveFailedEventArgs e)
            {
                owner.OnTableControlCurrentCellMoveFailed(new GridTableControlCurrentCellMoveFailedEventArgs(tableControl, e));
            }

            public void OnDrawCellFrameAppearance(GridDrawCellBackgroundEventArgs e)
            {
                owner.OnTableControlDrawCellFrameAppearance(new GridTableControlDrawCellBackgroundEventArgs(tableControl, e));
            }

            public void OnMouseWheel(MouseEventArgs e)
            {
                owner.OnTableControlMouseWheel(new GridTableControlMouseEventArgs(tableControl, e));
            }

            public void OnKeyPress(KeyPressEventArgs e)
            {
                owner.OnTableControlKeyPress(new GridTableControlKeyPressEventArgs(tableControl, e));
            }

            #endregion
        }

        //// evtable TableControlCurrentCellKeyDown GridTableControlKey

        /// <summary>
        /// Occurs when the <see cref="GridControlBase.CurrentCellKeyDown"/> event of the underlying <see cref="GridTableControl"/> is raised.
        /// The original event arguments are provided with the <see cref="GridTableControlKeyEventArgs.Inner"/> property of the
        /// <see cref="GridTableControlKeyEventArgs"/> class.
        /// </summary>
        [Description("Occurs when the CurrentCellKeyDown event of the underlying GridTableControl is raised")]
        [Category("TableControl")]
        public event GridTableControlKeyEventHandler TableControlCurrentCellKeyDown;

        /// <summary>
        /// Raises the <see cref="TableControlCurrentCellKeyDown"/> event.
        /// </summary>
        // <param name="e">A <see cref="GridTableControlKeyEventArgs" /> that contains the event data.</param>
        protected virtual void OnTableControlCurrentCellKeyDown(GridTableControlKeyEventArgs e)
        {
            ////TraceUtil.TraceCurrentMethodInfo(e);

            if (TableControlCurrentCellKeyDown != null)
            {
                TableControlCurrentCellKeyDown(this, e);
            }
            if (e.Inner.KeyCode == Keys.Enter && this.TableControl.WantEnterKey
                && e.TableControl.CurrentCell.Renderer != null && this.TableDescriptor != null && !e.TableControl.CurrentCell.IsLocked)
            {
                int lastRowCount = this.TableModel.RowCount;
                int lastColCount = this.TableDescriptor.GroupedColumns.Count == 0 ? this.TableDescriptor.Columns.Count : this.TableDescriptor.Columns.Count + this.TableDescriptor.GroupedColumns.Count;
                int visibleStartCol = 1;
                int rowHeaderCount = this.TableModel.Rows.HeaderCount;
                int colHeaderCount = this.TableModel.Cols.HeaderCount;
                int CurRowIndex = e.TableControl.CurrentCell.RowIndex;
                int CurColIndex = e.TableControl.CurrentCell.ColIndex;
                int groupedColumns = this.TableDescriptor.GroupedColumns.Count;
                int stackedHeaders = this.TableDescriptor.StackedHeaderRows.Count;
                int topRow = e.TableControl.Table.Records[0].GetRowIndex() + rowHeaderCount;
                int topRowIndex = e.TableControl.Table.Records[0].GetRowIndex();
                int firstCol = this.TableDescriptor.FieldToColIndex(0) + colHeaderCount;
                int firstColIndex = this.TableDescriptor.FieldToColIndex(0);
                switch (this.TableModel.Options.EnterKeyBehavior)
                {
                    case GridDirectionType.Left:
                        if (CurRowIndex <= lastRowCount && CurRowIndex >= topRow && CurColIndex >= firstCol)
                        {
                            if (CurColIndex > firstCol)
                            {
                                int inc = 1;
                                GridStyleInfo style = e.TableControl.GetViewStyleInfo(CurRowIndex, CurColIndex + inc);
                                if (!style.Enabled)
                                {
                                    inc++;
                                }
                                e.TableControl.CurrentCell.MoveLeft();
                            }

                            else
                            {
                                int scopeCurRowIndex = --CurRowIndex;
                                switch (this.TableModel.Options.WrapCellBehavior)
                                {
                                    case GridWrapCellBehavior.None:
                                        return;
                                    case GridWrapCellBehavior.WrapGrid:
                                        if (CurRowIndex == topRow - 1)
                                        {
                                            e.TableControl.CurrentCell.MoveTo(lastRowCount, lastColCount);
                                            e.TableControl.ScrollCellInView(lastRowCount, lastColCount, GridScrollCurrentCellReason.MoveTo);
                                        }
                                        else
                                        {
                                            e.TableControl.CurrentCell.MoveTo(scopeCurRowIndex, lastColCount);
                                            e.TableControl.ScrollCellInView(scopeCurRowIndex, lastColCount, GridScrollCurrentCellReason.MoveTo);
                                        }
                                        break;
                                    default:
                                        e.TableControl.CurrentCell.MoveTo(scopeCurRowIndex, lastColCount);
                                        break;
                                }
                            }
                        }
                        break;
                    case GridDirectionType.Right:
                        if (CurColIndex <= lastColCount && CurRowIndex >= topRow && CurColIndex >= firstCol)
                        {
                            if (e.TableControl.CurrentCell.ColIndex < lastColCount)
                            {
                                int inc = 1;
                                GridStyleInfo style = e.TableControl.GetViewStyleInfo(CurRowIndex, CurColIndex + inc);
                                if (!style.Enabled)
                                {
                                    inc++;
                                }
                                e.TableControl.CurrentCell.MoveRight();
                            }
                            else
                            {
                                int scopeCurRowIndex = ++CurRowIndex;
                                switch (this.TableModel.Options.WrapCellBehavior)
                                {
                                    case GridWrapCellBehavior.None:
                                        return;
                                    case GridWrapCellBehavior.WrapGrid:
                                        if (scopeCurRowIndex > lastRowCount)
                                        {
                                            e.TableControl.CurrentCell.MoveTo(topRow, firstCol);
                                            e.TableControl.ScrollCellInView(topRow, firstCol, GridScrollCurrentCellReason.MoveTo);
                                        }
                                        else
                                        {
                                            e.TableControl.CurrentCell.MoveTo(scopeCurRowIndex, firstCol);
                                            e.TableControl.ScrollCellInView(scopeCurRowIndex, firstCol, GridScrollCurrentCellReason.MoveTo);
                                        }
                                        break;
                                    default:
                                        e.TableControl.CurrentCell.MoveTo(scopeCurRowIndex, firstCol);
                                        break;
                                }
                            }
                        }
                        break;
                    case GridDirectionType.Up:
                        if (CurRowIndex <= lastRowCount && CurRowIndex > rowHeaderCount + stackedHeaders && CurColIndex > colHeaderCount)
                        {
                            if (e.TableControl.CurrentCell.Renderer.RowIndex > topRow)
                            {
                                int inc = 1;
                                GridStyleInfo style = e.TableControl.GetViewStyleInfo(CurRowIndex, CurColIndex + inc);
                                if (!style.Enabled)
                                {
                                    inc++;
                                }
                                e.TableControl.CurrentCell.MoveUp();
                            }
                            else
                            {
                                switch (this.TableModel.Options.WrapCellBehavior)
                                {
                                    case GridWrapCellBehavior.None:
                                        return;
                                    case GridWrapCellBehavior.WrapGrid:
                                        if (CurColIndex <= firstCol )
                                        {
                                            e.TableControl.CurrentCell.MoveTo(lastRowCount, lastColCount);
                                            e.TableControl.ScrollCellInView(lastRowCount, lastColCount, GridScrollCurrentCellReason.MoveTo);
                                        }
                                        else
                                        {
                                            e.TableControl.CurrentCell.MoveTo(lastRowCount, CurColIndex - 1);
                                            e.TableControl.ScrollCellInView(lastRowCount, CurColIndex - 1, GridScrollCurrentCellReason.MoveTo);
                                        }
                                            break;
                                    default:
                                        e.TableControl.CurrentCell.MoveTo(CurRowIndex, firstCol);
                                        break;
                                }
                            }
                        }
                        break;
                    case GridDirectionType.Down:
                        if (CurRowIndex <= lastRowCount && CurRowIndex >= topRow && CurColIndex > colHeaderCount)
                        {
                            if (CurRowIndex < lastRowCount)
                            {
                                int inc = 1;
                                GridStyleInfo style = e.TableControl.GetViewStyleInfo(CurRowIndex, CurColIndex + inc);
                                if (!style.Enabled)
                                {
                                    inc++;
                                }
                                e.TableControl.CurrentCell.MoveDown();
                            }
                            else
                            {
                                switch (this.TableModel.Options.WrapCellBehavior)
                                {
                                    case GridWrapCellBehavior.None:
                                        return;
                                    case GridWrapCellBehavior.WrapGrid:
                                        if (CurColIndex >= lastColCount)
                                        {
                                            e.TableControl.CurrentCell.MoveTo(topRow, firstCol);
                                            e.TableControl.ScrollCellInView(topRow, firstCol, GridScrollCurrentCellReason.MoveTo);
                                        }
                                        else
                                        {
                                            e.TableControl.CurrentCell.MoveTo(topRow, CurColIndex + 1);
                                            e.TableControl.ScrollCellInView(topRow, CurColIndex + 1, GridScrollCurrentCellReason.MoveTo);
                                        }
                                        break;
                                    default:
                                        e.TableControl.CurrentCell.MoveTo(CurRowIndex, firstCol);
                                        break;
                                }
                            }
                        }
                        break;
                    case GridDirectionType.Bottom:
                        if (CurRowIndex <= lastRowCount && CurRowIndex >= topRow && CurColIndex > colHeaderCount)
                        {
                            e.TableControl.CurrentCell.MoveTo(lastRowCount, CurColIndex);
                            e.TableControl.ScrollCellInView(lastRowCount, CurColIndex, GridScrollCurrentCellReason.MoveTo);
                        }
                        break;
                    case GridDirectionType.Top:
                        if (CurRowIndex <= lastRowCount && CurRowIndex > rowHeaderCount+stackedHeaders && CurColIndex > colHeaderCount)
                        {
                            e.TableControl.CurrentCell.MoveTo(topRow , CurColIndex);
                            e.TableControl.ScrollCellInView(topRowIndex , CurColIndex, GridScrollCurrentCellReason.MoveTo);
                        }
                        break;
                    case GridDirectionType.TopLeft:
                        if (CurRowIndex <= lastRowCount && CurRowIndex >= topRow && CurColIndex >= firstCol)
                        {
                            e.TableControl.CurrentCell.MoveTo(topRow, firstCol);
                            e.TableControl.ScrollCellInView(topRowIndex, firstCol, GridScrollCurrentCellReason.MoveTo);
                        }
                        break;
                    case GridDirectionType.BottomRight:
                        if (CurRowIndex <= lastRowCount && CurRowIndex >= topRow && CurColIndex >= firstCol)
                        {
                            e.TableControl.CurrentCell.MoveTo(lastRowCount, lastColCount);
                            e.TableControl.ScrollCellInView(lastRowCount, lastColCount, GridScrollCurrentCellReason.MoveTo);
                        }
                        break;
                    case GridDirectionType.MostLeft:
                        if (CurColIndex <= lastColCount && CurRowIndex >= topRow && CurColIndex >= firstCol)
                        {
                            e.TableControl.CurrentCell.MoveTo(CurRowIndex, firstCol );
                            e.TableControl.ScrollCellInView(CurRowIndex, firstColIndex, GridScrollCurrentCellReason.MoveTo);
                        }
                        break;
                    case GridDirectionType.MostRight:
                        if (CurColIndex <= lastColCount && CurRowIndex >= topRow && CurColIndex >= firstCol)
                        {
                            e.TableControl.CurrentCell.MoveTo(CurRowIndex, lastColCount);
                            e.TableControl.ScrollCellInView(CurRowIndex, lastColCount, GridScrollCurrentCellReason.MoveTo);
                        }
                        break;
                    case GridDirectionType.PageDown:
                        if (CurRowIndex <= lastRowCount && CurRowIndex >= topRow && CurColIndex > colHeaderCount)
                        {
                            if (CurRowIndex < lastRowCount)
                            {
                                int visibleRow = e.TableControl.ViewLayout.VisibleRows;
                                int colHeader = e.TableControl.Model.Cols.HeaderCount;
                                if (e.TableControl.CurrentCell.RowIndex + visibleRow > lastRowCount)
                                {
                                    e.TableControl.CurrentCell.MoveTo(lastRowCount, CurColIndex);
                                    e.TableControl.ScrollCellInView(lastRowCount, CurColIndex, GridScrollCurrentCellReason.MoveTo);
                                }
                                else
                                {
                                    if (rowHeaderCount >= 1)
                                    {
                                        e.TableControl.CurrentCell.MoveTo((CurRowIndex + visibleRow) - (topRowIndex + 1), CurColIndex);
                                        e.TableControl.ScrollCellInView((CurRowIndex + visibleRow) - (topRowIndex + 1), CurColIndex, GridScrollCurrentCellReason.MoveTo);
                                    }
                                    else
                                    {
                                        e.TableControl.CurrentCell.MoveTo((CurRowIndex + visibleRow) - (topRow + 1), CurColIndex);
                                        e.TableControl.ScrollCellInView((CurRowIndex + visibleRow) - (topRow + 1), CurColIndex, GridScrollCurrentCellReason.MoveTo);
                                    }
                                }
                            }
                            else
                            {
                                switch (this.TableModel.Options.WrapCellBehavior)
                                {
                                    case GridWrapCellBehavior.None:
                                        return;
                                    case GridWrapCellBehavior.WrapGrid:
                                        if (CurColIndex >= lastColCount)
                                        {
                                            e.TableControl.CurrentCell.MoveTo(topRow, firstCol);
                                            e.TableControl.ScrollCellInView(topRow, firstCol, GridScrollCurrentCellReason.MoveTo);
                                        }
                                        else
                                        {
                                            e.TableControl.CurrentCell.MoveTo(topRow, CurColIndex + 1);
                                            e.TableControl.ScrollCellInView(topRow, CurColIndex + 1, GridScrollCurrentCellReason.MoveTo);
                                        }
                                        break;
                                    default:
                                        e.TableControl.CurrentCell.MoveTo(CurRowIndex, visibleStartCol);
                                        break;
                                }
                            }
                        }
                            break;
                        
                    case GridDirectionType.PageUp:
                            if (CurRowIndex <= lastRowCount && CurRowIndex > rowHeaderCount + stackedHeaders && CurColIndex > colHeaderCount)
                            {
                                if (CurRowIndex > topRow)
                                {
                                    int visibleRow = e.TableControl.ViewLayout.VisibleRows;
                                    int colHeader = e.TableControl.Model.Cols.HeaderCount;
                                    if (CurRowIndex - visibleRow < topRow)
                                    {
                                        e.TableControl.CurrentCell.MoveTo(topRow, CurColIndex);
                                        e.TableControl.ScrollCellInView(topRow, CurColIndex, GridScrollCurrentCellReason.MoveTo);
                                    }
                                    else
                                    {
                                        if (rowHeaderCount >= 1)
                                        {
                                            e.TableControl.CurrentCell.MoveTo((CurRowIndex - visibleRow) + (topRowIndex + 1), CurColIndex);
                                            e.TableControl.ScrollCellInView((CurRowIndex - visibleRow) + (topRowIndex + 1), CurColIndex, GridScrollCurrentCellReason.MoveTo);
                                        }
                                        else
                                        {
                                            e.TableControl.CurrentCell.MoveTo((CurRowIndex - visibleRow) + (topRow + 1), CurColIndex);
                                            e.TableControl.ScrollCellInView((CurRowIndex - visibleRow) + (topRow + 1), CurColIndex, GridScrollCurrentCellReason.MoveTo);
                                        }
                                    }
                                }
                                else
                                {
                                    switch (this.TableModel.Options.WrapCellBehavior)
                                    {
                                        case GridWrapCellBehavior.None:
                                            return;
                                        case GridWrapCellBehavior.WrapGrid:
                                            if (CurColIndex <= firstCol)
                                            {
                                                e.TableControl.CurrentCell.MoveTo(lastRowCount, lastColCount);
                                                e.TableControl.ScrollCellInView(lastRowCount, lastColCount, GridScrollCurrentCellReason.MoveTo);
                                            }
                                            else
                                            {
                                                e.TableControl.CurrentCell.MoveTo(lastRowCount, CurColIndex - 1);
                                                e.TableControl.ScrollCellInView(lastRowCount, CurColIndex - 1, GridScrollCurrentCellReason.MoveTo);
                                            }
                                            break;
                                        default:
                                            e.TableControl.CurrentCell.MoveTo(CurRowIndex, visibleStartCol);
                                            break;
                                    }
                                }
                            }
                            break;
                }
            }
        }       

        //// evtable TableControlCurrentCellValidated GridTableControl

        /// <summary>
        /// Occurs when the <see cref="GridControlBase.CurrentCellValidated"/> event of the underlying <see cref="GridTableControl"/> is raised.
        /// The original event arguments are provided with the <see cref="GridTableControlEventArgs.Inner"/> property of the
        /// <see cref="GridTableControlEventArgs"/> class.
        /// </summary>
        [Description("Occurs when the CurrentCellValidated event of the underlying GridTableControl is raised")]
        [Category("TableControl")]
        public event GridTableControlEventHandler TableControlCurrentCellValidated;

        /// <summary>
        /// Raises the <see cref="TableControlCurrentCellValidated"/> event.
        /// </summary>
        // <param name="e">A <see cref="GridTableControlEventArgs" /> that contains the event data.</param>
        protected virtual void OnTableControlCurrentCellValidated(GridTableControlEventArgs e)
        {
            ////TraceUtil.TraceCurrentMethodInfo(e);

            if (TableControlCurrentCellValidated != null)
            {
                TableControlCurrentCellValidated(this, e);
            }
        }

        //// evtable TableControlCurrentCellChanged GridTableControl

        /// <summary>
        /// Occurs when the <see cref="GridControlBase.CurrentCellChanged"/> event of the underlying <see cref="GridTableControl"/> is raised.
        /// The original event arguments are provided with the <see cref="GridTableControlEventArgs.Inner"/> property of the
        /// <see cref="GridTableControlEventArgs"/> class.
        /// </summary>
        [Description("Occurs when the CurrentCellChanged event of the underlying GridTableControl is raised")]
        [Category("TableControl")]
        public event GridTableControlEventHandler TableControlCurrentCellChanged;

        /// <summary>
        /// Raises the <see cref="TableControlCurrentCellChanged"/> event.
        /// </summary>
        // <param name="e">A <see cref="GridTableControlEventArgs" /> that contains the event data.</param>
        protected virtual void OnTableControlCurrentCellChanged(GridTableControlEventArgs e)
        {
            ////TraceUtil.TraceCurrentMethodInfo(e);

            if (TableControlCurrentCellChanged != null)
            {
                TableControlCurrentCellChanged(this, e);
            }
        }

        //// evtable TableControlCurrentCellDeactivateFailed GridTableControl

        /// <summary>
        /// Occurs when the <see cref="GridControlBase.CurrentCellDeactivateFailed"/> event of the underlying <see cref="GridTableControl"/> is raised.
        /// The original event arguments are provided with the <see cref="GridTableControlEventArgs.Inner"/> property of the
        /// <see cref="GridTableControlEventArgs"/> class.
        /// </summary>
        [Description("Occurs when the CurrentCellDeactivateFailed event of the underlying GridTableControl is raised")]
        [Category("TableControl")]
        public event GridTableControlEventHandler TableControlCurrentCellDeactivateFailed;

        /// <summary>
        /// Raises the <see cref="TableControlCurrentCellDeactivateFailed"/> event.
        /// </summary>
        // <param name="e">A <see cref="GridTableControlEventArgs" /> that contains the event data.</param>
        protected virtual void OnTableControlCurrentCellDeactivateFailed(GridTableControlEventArgs e)
        {
            ////TraceUtil.TraceCurrentMethodInfo(e);
            if (TableControlCurrentCellDeactivateFailed != null)
            {
                TableControlCurrentCellDeactivateFailed(this, e);
            }
        }

        //// evtable TableControlKeyDown GridTableControlKey

        /// <summary>
        /// Occurs when the <see cref="Control.KeyDown"/> event of the underlying <see cref="GridTableControl"/> is raised.
        /// The original event arguments are provided with the <see cref="GridTableControlKeyEventArgs.Inner"/> property of the
        /// <see cref="GridTableControlKeyEventArgs"/> class.
        /// </summary>
        [Description("Occurs when the KeyDown event of the underlying GridTableControl is raised")]
        [Category("TableControl")]
        public event GridTableControlKeyEventHandler TableControlKeyDown;

        /// <summary>
        /// Raises the <see cref="TableControlKeyDown"/> event.
        /// </summary>
        // <param name="e">A <see cref="GridTableControlKeyEventArgs" /> that contains the event data.</param>
        protected virtual void OnTableControlKeyDown(GridTableControlKeyEventArgs e)
        {
            ////TraceUtil.TraceCurrentMethodInfo(e);
            if (TableControlKeyDown != null)
            {
                TableControlKeyDown(this, e);
            }
        }

        //// evtable TableControlCurrentCellDeleting GridTableControlCancel

        /// <summary>
        /// Occurs when the <see cref="GridControlBase.CurrentCellDeleting"/> event of the underlying <see cref="GridTableControl"/> is raised.
        /// The original event arguments are provided with the <see cref="GridTableControlCancelEventArgs.Inner"/> property of the
        /// <see cref="GridTableControlCancelEventArgs"/> class.
        /// </summary>
        [Description("Occurs when the CurrentCellDeleting event of the underlying GridTableControl is raised")]
        [Category("TableControl")]
        public event GridTableControlCancelEventHandler TableControlCurrentCellDeleting;

        /// <summary>
        /// Raises the <see cref="TableControlCurrentCellDeleting"/> event.
        /// </summary>
        // <param name="e">A <see cref="GridTableControlCancelEventArgs" /> that contains the event data.</param>
        protected virtual void OnTableControlCurrentCellDeleting(GridTableControlCancelEventArgs e)
        {
            ////TraceUtil.TraceCurrentMethodInfo(e);
            if (TableControlCurrentCellDeleting != null)
            {
                TableControlCurrentCellDeleting(this, e);
            }
        }

        //// evtable TableControlCurrentCellValidating GridTableControlCancel

        /// <summary>
        /// Occurs when the <see cref="GridControlBase.CurrentCellValidating"/> event of the underlying <see cref="GridTableControl"/> is raised.
        /// The original event arguments are provided with the <see cref="GridTableControlCancelEventArgs.Inner"/> property of the
        /// <see cref="GridTableControlCancelEventArgs"/> class.
        /// </summary>
        [Description("Occurs when the CurrentCellValidating event of the underlying GridTableControl is raised")]
        [Category("TableControl")]
        public event GridTableControlCancelEventHandler TableControlCurrentCellValidating;

        /// <summary>
        /// Raises the <see cref="TableControlCurrentCellValidating"/> event.
        /// </summary>
        // <param name="e">A <see cref="GridTableControlCancelEventArgs" /> that contains the event data.</param>
        protected virtual void OnTableControlCurrentCellValidating(GridTableControlCancelEventArgs e)
        {
            ////TraceUtil.TraceCurrentMethodInfo(e);

            if (TableControlCurrentCellValidating != null)
            {
                TableControlCurrentCellValidating(this, e);
            }
        }

        //// evtable TableControlCurrentCellActivated GridTableControl

        /// <summary>
        /// Occurs when the <see cref="GridControlBase.CurrentCellActivated"/> event of the underlying <see cref="GridTableControl"/> is raised.
        /// The original event arguments are provided with the <see cref="GridTableControlEventArgs.Inner"/> property of the
        /// <see cref="GridTableControlEventArgs"/> class.
        /// </summary>
        [Description("Occurs when the CurrentCellActivated event of the underlying GridTableControl is raised")]
        [Category("TableControl")]
        public event GridTableControlEventHandler TableControlCurrentCellActivated;

        /// <summary>
        /// Raises the <see cref="TableControlCurrentCellActivated"/> event.
        /// </summary>
        // <param name="e">A <see cref="GridTableControlEventArgs" /> that contains the event data.</param>
        protected virtual void OnTableControlCurrentCellActivated(GridTableControlEventArgs e)
        {
            ////TraceUtil.TraceCurrentMethodInfo(e);

            if (TableControlCurrentCellActivated != null)
            {
                TableControlCurrentCellActivated(this, e);
            }
        }

        //// evtable TableControlKeyUp GridTableControlKey

        /// <summary>
        /// Occurs when the <see cref="Control.KeyUp"/> event of the underlying <see cref="GridTableControl"/> is raised.
        /// The original event arguments are provided with the <see cref="GridTableControlKeyEventArgs.Inner"/> property of the
        /// <see cref="GridTableControlKeyEventArgs"/> class.
        /// </summary>
        [Description("Occurs when the KeyUp event of the underlying GridTableControl is raised")]
        [Category("TableControl")]
        public event GridTableControlKeyEventHandler TableControlKeyUp;

        /// <summary>
        /// Raises the <see cref="TableControlKeyUp"/> event.
        /// </summary>
        // <param name="e">A <see cref="GridTableControlKeyEventArgs" /> that contains the event data.</param>
        protected virtual void OnTableControlKeyUp(GridTableControlKeyEventArgs e)
        {
            ////TraceUtil.TraceCurrentMethodInfo(e);

            if (TableControlKeyUp != null)
            {
                TableControlKeyUp(this, e);
            }
        }

        // evtable TableControlCurrentCellControlDoubleClick GridTableControlControl

        /// <summary>
        /// Occurs when the <see cref="GridControlBase.CurrentCellControlDoubleClick"/> event of the underlying <see cref="GridTableControl"/> is raised.
        /// The original event arguments are provided with the <see cref="GridTableControlControlEventArgs.Inner"/> property of the
        /// <see cref="GridTableControlControlEventArgs"/> class.
        /// </summary>
        [Description("Occurs when the CurrentCellControlDoubleClick event of the underlying GridTableControl is raised")]
        [Category("TableControl")]
        public event GridTableControlControlEventHandler TableControlCurrentCellControlDoubleClick;

        /// <summary>
        /// Raises the <see cref="TableControlCurrentCellControlDoubleClick"/> event.
        /// </summary>
        // <param name="e">A <see cref="GridTableControlControlEventArgs" /> that contains the event data.</param>
        protected virtual void OnTableControlCurrentCellControlDoubleClick(GridTableControlControlEventArgs e)
        {
            ////TraceUtil.TraceCurrentMethodInfo(e);

            if (TableControlCurrentCellControlDoubleClick != null)
            {
                TableControlCurrentCellControlDoubleClick(this, e);
            }
        }

        //// evtable TableControlCurrentCellDeactivating GridTableControlCancel

        /// <summary>
        /// Occurs when the <see cref="GridControlBase.CurrentCellDeactivating"/> event of the underlying <see cref="GridTableControl"/> is raised.
        /// The original event arguments are provided with the <see cref="GridTableControlCancelEventArgs.Inner"/> property of the
        /// <see cref="GridTableControlCancelEventArgs"/> class.
        /// </summary>
        [Description("Occurs when the CurrentCellDeactivating event of the underlying GridTableControl is raised")]
        [Category("TableControl")]
        public event GridTableControlCancelEventHandler TableControlCurrentCellDeactivating;

        /// <summary>
        /// Raises the <see cref="TableControlCurrentCellDeactivating"/> event.
        /// </summary>
        // <param name="e">A <see cref="GridTableControlCancelEventArgs" /> that contains the event data.</param>
        protected virtual void OnTableControlCurrentCellDeactivating(GridTableControlCancelEventArgs e)
        {
            ////TraceUtil.TraceCurrentMethodInfo(e);

            if (TableControlCurrentCellDeactivating != null)
            {
                TableControlCurrentCellDeactivating(this, e);
            }
        }

        //// evtable TableControlCurrentCellKeyPress GridTableControlKeyPress

        /// <summary>
        /// Occurs when the <see cref="GridControlBase.CurrentCellKeyPress"/> event of the underlying <see cref="GridTableControl"/> is raised.
        /// The original event arguments are provided with the <see cref="GridTableControlKeyPressEventArgs.Inner"/> property of the
        /// <see cref="GridTableControlKeyPressEventArgs"/> class.
        /// </summary>
        [Description("Occurs when the CurrentCellKeyPress event of the underlying GridTableControl is raised")]
        [Category("TableControl")]
        public event GridTableControlKeyPressEventHandler TableControlCurrentCellKeyPress;

        /// <summary>
        /// Raises the <see cref="TableControlCurrentCellKeyPress"/> event.
        /// </summary>
        // <param name="e">A <see cref="GridTableControlKeyPressEventArgs" /> that contains the event data.</param>
        protected virtual void OnTableControlCurrentCellKeyPress(GridTableControlKeyPressEventArgs e)
        {
            ////TraceUtil.TraceCurrentMethodInfo(e);

            if (TableControlCurrentCellKeyPress != null)
            {
                TableControlCurrentCellKeyPress(this, e);
            }
        }

        //// evtable TableControlCurrentCellAcceptedChanges GridTableControlCancel

        /// <summary>
        /// Occurs when the <see cref="GridControlBase.CurrentCellAcceptedChanges"/> event of the underlying <see cref="GridTableControl"/> is raised.
        /// The original event arguments are provided with the <see cref="GridTableControlCancelEventArgs.Inner"/> property of the
        /// <see cref="GridTableControlCancelEventArgs"/> class.
        /// </summary>
        [Description("Occurs when the CurrentCellAcceptedChanges event of the underlying GridTableControl is raised")]
        [Category("TableControl")]
        public event GridTableControlCancelEventHandler TableControlCurrentCellAcceptedChanges;

        /// <summary>
        /// Raises the <see cref="TableControlCurrentCellAcceptedChanges"/> event.
        /// </summary>
        // <param name="e">A <see cref="GridTableControlCancelEventArgs" /> that contains the event data.</param>
        protected virtual void OnTableControlCurrentCellAcceptedChanges(GridTableControlCancelEventArgs e)
        {
            ////TraceUtil.TraceCurrentMethodInfo(e);

            if (TableControlCurrentCellAcceptedChanges != null)
            {
                TableControlCurrentCellAcceptedChanges(this, e);
            }
        }

        //// event GridQueryAllowDragColumnEventHandler QueryAllowDragColumn;

        /// <summary>
        /// Occurs when the user hovers the mouse over a column header or clicks on it.
        /// In your event handler, you can determine if the selected column can be dragged.
        /// </summary>
        /// <remarks>
        /// You can disallow dragging the column when
        /// you assign False to <see cref="GridQueryAllowDragColumnEventArgs.AllowDrag"/>.
        /// </remarks>
        /// <seealso cref="GridQueryAllowDragColumnEventArgs"/>
        /// <seealso cref="GridTableOptionsStyleInfo"/>
        /// <seealso cref="GridTableOptionsStyleInfo.AllowDragColumns"/>
        [Description("Occurs when the user hovers the mouse over a column header or clicks on it"), Category("TableControl")]
        public event GridQueryAllowDragColumnEventHandler TableControlQueryAllowDragColumn;

        /// <summary>
        /// Raises the <see cref="TableControlQueryAllowDragColumn"/> event.
        /// </summary>
        /// <param name="e">A <see cref="GridQueryAllowDragColumnEventArgs" /> that contains the event data.</param>
        protected virtual void OnTableControlQueryAllowDragColumn(GridQueryAllowDragColumnEventArgs e)
        {
            if (TableControlQueryAllowDragColumn != null)
            {
                TableControlQueryAllowDragColumn(this, e);
            }
        }

        internal void RaiseTableControlQueryAllowDragColumn(GridQueryAllowDragColumnEventArgs e)
        {
            OnTableControlQueryAllowDragColumn(e);
        }

        //// event GridQueryAllowGroupByColumnEventHandler QueryAllowGroupByColumn;

        /// <summary>
        /// Occurs when the user drags a column header over the GroupDropArea.
        /// In your event handler, you can determine if the grid can be grouped by the selected column.
        /// </summary>
        /// <remarks>
        /// You can disallow grouping by the column when
        /// you assign False to <see cref="GridQueryAllowGroupByColumnEventArgs.AllowGroupByColumn"/>.
        /// </remarks>
        /// <seealso cref="GridQueryAllowGroupByColumnEventArgs"/>
        /// <seealso cref="GridTableOptionsStyleInfo"/>
        /// <seealso cref="GridColumnDescriptor.AllowGroupByColumn"/>
        [Description("Occurs when the user drags a column header over the GroupDropArea"), Category("TableControl")]
        public event GridQueryAllowGroupByColumnEventHandler TableControlQueryAllowGroupByColumn;

        /// <summary>
        /// Raises the <see cref="TableControlQueryAllowGroupByColumn"/> event.
        /// </summary>
        /// <param name="e">A <see cref="GridQueryAllowGroupByColumnEventArgs" /> that contains the event data.</param>
        protected virtual void OnTableControlQueryAllowGroupByColumn(GridQueryAllowGroupByColumnEventArgs e)
        {
            if (TableControlQueryAllowGroupByColumn != null)
            {
                TableControlQueryAllowGroupByColumn(this, e);
            }
        }

        internal void RaiseTableControlQueryAllowGroupByColumn(GridQueryAllowGroupByColumnEventArgs e)
        {
            OnTableControlQueryAllowGroupByColumn(e);
        }

        //// event GridQueryAllowSortColumnEventHandler QueryAllowSortColumn;

        /// <summary>
        /// Occurs when the user hovers the mouse over a column header or clicks on it.
        /// In your event handler, you can determine if the selected column can be sorted.
        /// </summary>
        /// <remarks>
        /// You can disallow sorting by the column when
        /// you assign False to <see cref="GridQueryAllowSortColumnEventArgs.AllowSort"/>.
        /// </remarks>
        /// <seealso cref="GridQueryAllowDragColumnEventArgs"/>
        /// <seealso cref="GridTableOptionsStyleInfo"/>
        [Description("Occurs when the user hovers the mouse over a column header or clicks on it"), Category("TableControl")]
        public event GridQueryAllowSortColumnEventHandler TableControlQueryAllowSortColumn;

        /// <summary>
        /// Raises the <see cref="TableControlQueryAllowSortColumn"/> event.
        /// </summary>
        /// <param name="e">A <see cref="GridQueryAllowSortColumnEventArgs" /> that contains the event data.</param>
        protected virtual void OnTableControlQueryAllowSortColumn(GridQueryAllowSortColumnEventArgs e)
        {
            if (TableControlQueryAllowSortColumn != null)
            {
                TableControlQueryAllowSortColumn(this, e);
            }
        }

        internal void RaiseTableControlQueryAllowSortColumn(GridQueryAllowSortColumnEventArgs e)
        {
            OnTableControlQueryAllowSortColumn(e);
        }

        //// event GridQueryAllowArrowKeyNavigateToEventHandler QueryAllowArrowKeyNavigateTo;

        /// <summary>
        /// Occurs when the user navigates though display elements with arrow keys.
        /// In your event handler you can determine if the specified display element (e.g. a CaptionRow) can be stepped
        /// on or if it should be skipped.
        /// </summary>
        /// <remarks>
        /// You can set AllowNavigateTo if you
        /// want arrow keys to skip over specific display elements (e.g. skip caption rows).
        /// </remarks>
        /// <seealso cref="GridQueryAllowArrowKeyNavigateToEventArgs"/>
        [Description("Occurs when the user navigates though display elements with arrow keys"), Category("TableControl")]
        public event GridQueryAllowArrowKeyNavigateToEventHandler TableControlQueryAllowArrowKeyNavigateTo;

        /// <summary>
        /// Raises the <see cref="TableControlQueryAllowArrowKeyNavigateTo"/> event.
        /// </summary>
        /// <param name="e">A <see cref="GridQueryAllowArrowKeyNavigateToEventArgs" /> that contains the event data.</param>
        protected virtual void OnTableControlQueryAllowArrowKeyNavigateTo(GridQueryAllowArrowKeyNavigateToEventArgs e)
        {
            if (TableControlQueryAllowArrowKeyNavigateTo != null)
            {
                TableControlQueryAllowArrowKeyNavigateTo(this, e);
            }
        }

        internal void RaiseTableControlQueryAllowArrowKeyNavigateTo(GridQueryAllowArrowKeyNavigateToEventArgs e)
        {
            OnTableControlQueryAllowArrowKeyNavigateTo(e);
        }

        //// evtable TableControlCurrentCellKeyUp GridTableControlKey

        /// <summary>
        /// Occurs when the <see cref="GridControlBase.CurrentCellKeyUp"/> event of the underlying <see cref="GridTableControl"/> is raised.
        /// The original event arguments are provided with the <see cref="GridTableControlKeyEventArgs.Inner"/> property of the
        /// <see cref="GridTableControlKeyEventArgs"/> class.
        /// </summary>
        [Description("Occurs when the CurrentCellKeyUp event of the underlying GridTableControl is raised")]
        [Category("TableControl")]
        public event GridTableControlKeyEventHandler TableControlCurrentCellKeyUp;

        /// <summary>
        /// Raises the <see cref="TableControlCurrentCellKeyUp"/> event.
        /// </summary>
        // <param name="e">A <see cref="GridTableControlKeyEventArgs" /> that contains the event data.</param>
        protected virtual void OnTableControlCurrentCellKeyUp(GridTableControlKeyEventArgs e)
        {
            ////TraceUtil.TraceCurrentMethodInfo(e);

            if (TableControlCurrentCellKeyUp != null)
            {
                TableControlCurrentCellKeyUp(this, e);
            }
        }

        //// evtable TableControlMouseDown GridTableControlMouse

        /// <summary>
        /// Occurs when the <see cref="Control.MouseDown"/> event of the underlying <see cref="GridTableControl"/> is raised.
        /// The original event arguments are provided with the <see cref="GridTableControlMouseEventArgs.Inner"/> property of the
        /// <see cref="GridTableControlMouseEventArgs"/> class.
        /// </summary>
        [Description("Occurs when the MouseDown event of the underlying GridTableControl is raised")]
        [Category("TableControl")]
        public event GridTableControlMouseEventHandler TableControlMouseDown;

        /// <summary>
        /// Raises the <see cref="TableControlMouseDown"/> event.
        /// </summary>
        // <param name="e">A <see cref="GridTableControlMouseEventArgs" /> that contains the event data.</param>
        protected virtual void OnTableControlMouseDown(GridTableControlMouseEventArgs e)
        {
            ////TraceUtil.TraceCurrentMethodInfo(e);

            if (TableControlMouseDown != null)
            {
                TableControlMouseDown(this, e);
            }
        }

        //// evtable TableControlCurrentCellShowedDropDown GridTableControl

        /// <summary>
        /// Occurs when the <see cref="GridControlBase.CurrentCellShowedDropDown"/> event of the underlying <see cref="GridTableControl"/> is raised.
        /// The original event arguments are provided with the <see cref="GridTableControlEventArgs.Inner"/> property of the
        /// <see cref="GridTableControlEventArgs"/> class.
        /// </summary>
        [Description("Occurs when the CurrentCellShowedDropDown event of the underlying GridTableControl is raised")]
        [Category("TableControl")]
        public event GridTableControlEventHandler TableControlCurrentCellShowedDropDown;

        /// <summary>
        /// Raises the <see cref="TableControlCurrentCellShowedDropDown"/> event.
        /// </summary>
        // <param name="e">A <see cref="GridTableControlEventArgs" /> that contains the event data.</param>
        protected virtual void OnTableControlCurrentCellShowedDropDown(GridTableControlEventArgs e)
        {
            ////TraceUtil.TraceCurrentMethodInfo(e);

            if (TableControlCurrentCellShowedDropDown != null)
            {
                TableControlCurrentCellShowedDropDown(this, e);
            }
        }

        //// evtable TableControlCurrentCellControlLostFocus GridTableControlControl

        /// <summary>
        /// Occurs when the <see cref="GridControlBase.CurrentCellControlLostFocus"/> event of the underlying <see cref="GridTableControl"/> is raised.
        /// The original event arguments are provided with the <see cref="GridTableControlControlEventArgs.Inner"/> property of the
        /// <see cref="GridTableControlControlEventArgs"/> class.
        /// </summary>
        [Description("Occurs when the CurrentCellControlLostFocus event of the underlying GridTableControl is raised")]
        [Category("TableControl")]
        public event GridTableControlControlEventHandler TableControlCurrentCellControlLostFocus;

        /// <summary>
        /// Raises the <see cref="TableControlCurrentCellControlLostFocus"/> event.
        /// </summary>
        // <param name="e">A <see cref="GridTableControlControlEventArgs" /> that contains the event data.</param>
        protected virtual void OnTableControlCurrentCellControlLostFocus(GridTableControlControlEventArgs e)
        {
            ////TraceUtil.TraceCurrentMethodInfo(e);

            if (TableControlCurrentCellControlLostFocus != null)
            {
                TableControlCurrentCellControlLostFocus(this, e);
            }
        }

        //// evtable TableControlCurrentCellEditingComplete GridTableControl

        /// <summary>
        /// Occurs when the <see cref="GridControlBase.CurrentCellEditingComplete"/> event of the underlying <see cref="GridTableControl"/> is raised.
        /// The original event arguments are provided with the <see cref="GridTableControlEventArgs.Inner"/> property of the
        /// <see cref="GridTableControlEventArgs"/> class.
        /// </summary>
        [Description("Occurs when the CurrentCellEditingComplete event of the underlying GridTableControl is raised")]
        [Category("TableControl")]
        public event GridTableControlEventHandler TableControlCurrentCellEditingComplete;

        /// <summary>
        /// Raises the <see cref="TableControlCurrentCellEditingComplete"/> event.
        /// </summary>
        // <param name="e">A <see cref="GridTableControlEventArgs" /> that contains the event data.</param>
        protected virtual void OnTableControlCurrentCellEditingComplete(GridTableControlEventArgs e)
        {
            ////TraceUtil.TraceCurrentMethodInfo(e);

            if (TableControlCurrentCellEditingComplete != null)
            {
                TableControlCurrentCellEditingComplete(this, e);
            }
        }

        //// evtable TableControlCurrentCellCloseDropDown GridTableControlPopupClosed

        /// <summary>
        /// Occurs when the <see cref="GridControlBase.CurrentCellCloseDropDown"/> event of the underlying <see cref="GridTableControl"/> is raised.
        /// The original event arguments are provided with the <see cref="GridTableControlPopupClosedEventArgs.Inner"/> property of the
        /// <see cref="GridTableControlPopupClosedEventArgs"/> class.
        /// </summary>
        [Description("Occurs when the CurrentCellCloseDropDown event of the underlying GridTableControl is raised")]
        [Category("TableControl")]
        public event GridTableControlPopupClosedEventHandler TableControlCurrentCellCloseDropDown;

        /// <summary>
        /// Raises the <see cref="TableControlCurrentCellCloseDropDown"/> event.
        /// </summary>
        // <param name="e">A <see cref="GridTableControlPopupClosedEventArgs" /> that contains the event data.</param>
        protected virtual void OnTableControlCurrentCellCloseDropDown(GridTableControlPopupClosedEventArgs e)
        {
            ////TraceUtil.TraceCurrentMethodInfo(e);

            if (TableControlCurrentCellCloseDropDown != null)
            {
                TableControlCurrentCellCloseDropDown(this, e);
            }
        }

        //// evtable TableControlScrollTipFeedback GridTableControlScrollTipFeedback

        /// <summary>
        /// Occurs when the <see cref="ScrollControl.ScrollTipFeedback"/> event of the underlying <see cref="GridTableControl"/> is raised.
        /// The original event arguments are provided with the <see cref="GridTableControlScrollTipFeedbackEventArgs.Inner"/> property of the
        /// <see cref="GridTableControlScrollTipFeedbackEventArgs"/> class.
        /// </summary>
        [Description("Occurs when the ScrollTipFeedback event of the underlying GridTableControl is raised")]
        [Category("TableControl")]
        public event GridTableControlScrollTipFeedbackEventHandler TableControlScrollTipFeedback;

        /// <summary>
        /// Raises the <see cref="TableControlScrollTipFeedback"/> event.
        /// </summary>
        // <param name="e">A <see cref="GridTableControlScrollTipFeedbackEventArgs" /> that contains the event data.</param>
        protected virtual void OnTableControlScrollTipFeedback(GridTableControlScrollTipFeedbackEventArgs e)
        {
            ////TraceUtil.TraceCurrentMethodInfo(e);

            if (TableControlScrollTipFeedback != null)
            {
                TableControlScrollTipFeedback(this, e);
            }
        }

        //// evtable TableControlCurrentCellControlGotFocus GridTableControlControl

        /// <summary>
        /// Occurs when the <see cref="GridControlBase.CurrentCellControlGotFocus"/> event of the underlying <see cref="GridTableControl"/> is raised.
        /// The original event arguments are provided with the <see cref="GridTableControlControlEventArgs.Inner"/> property of the
        /// <see cref="GridTableControlControlEventArgs"/> class.
        /// </summary>
        [Description("Occurs when the CurrentCellControlGotFocus event of the underlying GridTableControl is raised")]
        [Category("TableControl")]
        public event GridTableControlControlEventHandler TableControlCurrentCellControlGotFocus;

        /// <summary>
        /// Raises the <see cref="TableControlCurrentCellControlGotFocus"/> event.
        /// </summary>
        // <param name="e">A <see cref="GridTableControlControlEventArgs" /> that contains the event data.</param>
        protected virtual void OnTableControlCurrentCellControlGotFocus(GridTableControlControlEventArgs e)
        {
            ////TraceUtil.TraceCurrentMethodInfo(e);

            if (TableControlCurrentCellControlGotFocus != null)
            {
                TableControlCurrentCellControlGotFocus(this, e);
            }
        }

        //// evtable TableControlCurrentCellStartEditing GridTableControlCancel

        /// <summary>
        /// Occurs when the <see cref="GridControlBase.CurrentCellStartEditing"/> event of the underlying <see cref="GridTableControl"/> is raised.
        /// The original event arguments are provided with the <see cref="GridTableControlCancelEventArgs.Inner"/> property of the
        /// <see cref="GridTableControlCancelEventArgs"/> class.
        /// </summary>
        [Description("Occurs when the CurrentCellStartEditing event of the underlying GridTableControl is raised")]
        [Category("TableControl")]
        public event GridTableControlCancelEventHandler TableControlCurrentCellStartEditing;

        /// <summary>
        /// Raises the <see cref="TableControlCurrentCellStartEditing"/> event.
        /// </summary>
        // <param name="e">A <see cref="GridTableControlCancelEventArgs" /> that contains the event data.</param>
        protected virtual void OnTableControlCurrentCellStartEditing(GridTableControlCancelEventArgs e)
        {
            ////TraceUtil.TraceCurrentMethodInfo(e);

            if (TableControlCurrentCellStartEditing != null)
            {
                TableControlCurrentCellStartEditing(this, e);
            }
        }

        //// evtable TableControlCurrentCellConfirmChangesFailed GridTableControl

        /// <summary>
        /// Occurs when the <see cref="GridControlBase.CurrentCellConfirmChangesFailed"/> event of the underlying <see cref="GridTableControl"/> is raised.
        /// The original event arguments are provided with the <see cref="GridTableControlEventArgs.Inner"/> property of the
        /// <see cref="GridTableControlEventArgs"/> class.
        /// </summary>
        [Description("Occurs when the CurrentCellConfirmChangesFailed event of the underlying GridTableControl is raised")]
        [Category("TableControl")]
        public event GridTableControlEventHandler TableControlCurrentCellConfirmChangesFailed;

        /// <summary>
        /// Raises the <see cref="TableControlCurrentCellConfirmChangesFailed"/> event.
        /// </summary>
        // <param name="e">A <see cref="GridTableControlEventArgs" /> that contains the event data.</param>
        protected virtual void OnTableControlCurrentCellConfirmChangesFailed(GridTableControlEventArgs e)
        {
            ////TraceUtil.TraceCurrentMethodInfo(e);

            if (TableControlCurrentCellConfirmChangesFailed != null)
            {
                TableControlCurrentCellConfirmChangesFailed(this, e);
            }
        }

        //// evtable TableControlKeyPress GridTableControlKeyPress

        /// <summary>
        /// Occurs when the <see cref="Control.KeyPress"/> event of the underlying <see cref="GridTableControl"/> is raised.
        /// The original event arguments are provided with the <see cref="GridTableControlKeyPressEventArgs.Inner"/> property of the
        /// <see cref="GridTableControlKeyPressEventArgs"/> class.
        /// </summary>
        [Description("Occurs when the KeyPress event of the underlying GridTableControl is raised")]
        [Category("TableControl")]
        public event GridTableControlKeyPressEventHandler TableControlKeyPress;

        /// <summary>
        /// Raises the <see cref="TableControlKeyPress"/> event.
        /// </summary>
        // <param name="e">A <see cref="GridTableControlKeyPressEventArgs" /> that contains the event data.</param>
        protected virtual void OnTableControlKeyPress(GridTableControlKeyPressEventArgs e)
        {
            ////TraceUtil.TraceCurrentMethodInfo(e);

            if (TableControlKeyPress != null)
            {
                TableControlKeyPress(this, e);
            }
        }

        //// evtable TableControlCurrentCellRejectedChanges GridTableControl

        /// <summary>
        /// Occurs when the <see cref="GridControlBase.CurrentCellRejectedChanges"/> event of the underlying <see cref="GridTableControl"/> is raised.
        /// The original event arguments are provided with the <see cref="GridTableControlEventArgs.Inner"/> property of the
        /// <see cref="GridTableControlEventArgs"/> class.
        /// </summary>
        [Description("Occurs when the CurrentCellRejectedChanges event of the underlying GridTableControl is raised")]
        [Category("TableControl")]
        public event GridTableControlEventHandler TableControlCurrentCellRejectedChanges;

        /// <summary>
        /// Raises the <see cref="TableControlCurrentCellRejectedChanges"/> event.
        /// </summary>
        // <param name="e">A <see cref="GridTableControlEventArgs" /> that contains the event data.</param>
        protected virtual void OnTableControlCurrentCellRejectedChanges(GridTableControlEventArgs e)
        {
            ////TraceUtil.TraceCurrentMethodInfo(e);

            if (TableControlCurrentCellRejectedChanges != null)
            {
                TableControlCurrentCellRejectedChanges(this, e);
            }
        }

        //// evtable TableControlCurrentCellChanging GridTableControlCancel

        /// <summary>
        /// Occurs when the <see cref="GridControlBase.CurrentCellChanging"/> event of the underlying <see cref="GridTableControl"/> is raised.
        /// The original event arguments are provided with the <see cref="GridTableControlCancelEventArgs.Inner"/> property of the
        /// <see cref="GridTableControlCancelEventArgs"/> class.
        /// </summary>
        [Description("Occurs when the CurrentCellChanging event of the underlying GridTableControl is raised")]
        [Category("TableControl")]
        public event GridTableControlCancelEventHandler TableControlCurrentCellChanging;

        /// <summary>
        /// Raises the <see cref="TableControlCurrentCellChanging"/> event.
        /// </summary>
        // <param name="e">A <see cref="GridTableControlCancelEventArgs" /> that contains the event data.</param>
        protected virtual void OnTableControlCurrentCellChanging(GridTableControlCancelEventArgs e)
        {
            ////TraceUtil.TraceCurrentMethodInfo(e);

            if (TableControlCurrentCellChanging != null)
            {
                TableControlCurrentCellChanging(this, e);
            }
        }

        //// evtable TableControlVScrollPixelPosChanged GridTableControlScrollPositionChanged

        /// <summary>
        /// Occurs when the <see cref="GridControlBase.VScrollPixelPosChanged"/> event of the underlying <see cref="GridTableControl"/> is raised.
        /// The original event arguments are provided with the <see cref="GridTableControlScrollPositionChangedEventArgs.Inner"/> property of the
        /// <see cref="GridTableControlScrollPositionChangedEventArgs"/> class.
        /// </summary>
        [Description("Occurs when the VScrollPixelPosChanged event of the underlying GridTableControl is raised")]
        [Category("TableControl")]
        public event GridTableControlScrollPositionChangedEventHandler TableControlVScrollPixelPosChanged;

        /// <summary>
        /// Raises the <see cref="TableControlVScrollPixelPosChanged"/> event.
        /// </summary>
        // <param name="e">A <see cref="GridTableControlScrollPositionChangedEventArgs" /> that contains the event data.</param>
        protected virtual void OnTableControlVScrollPixelPosChanged(GridTableControlScrollPositionChangedEventArgs e)
        {
            ////TraceUtil.TraceCurrentMethodInfo(e);

            if (TableControlVScrollPixelPosChanged != null)
            {
                TableControlVScrollPixelPosChanged(this, e);
            }
        }

        //// evtable TableControlCellMouseUp GridTableControlCellMouse

        /// <summary>
        /// Occurs when the <see cref="GridControlBase.CellMouseUp"/> event of the underlying <see cref="GridTableControl"/> is raised.
        /// The original event arguments are provided with the <see cref="GridTableControlCellMouseEventArgs.Inner"/> property of the
        /// <see cref="GridTableControlCellMouseEventArgs"/> class.
        /// </summary>
        [Description("Occurs when the CellMouseUp event of the underlying GridTableControl is raised")]
        [Category("TableControl")]
        public event GridTableControlCellMouseEventHandler TableControlCellMouseUp;

        /// <summary>
        /// Raises the <see cref="TableControlCellMouseUp"/> event.
        /// </summary>
        // <param name="e">A <see cref="GridTableControlCellMouseEventArgs" /> that contains the event data.</param>
        protected virtual void OnTableControlCellMouseUp(GridTableControlCellMouseEventArgs e)
        {
            ////TraceUtil.TraceCurrentMethodInfo(e);

            if (TableControlCellMouseUp != null)
            {
                TableControlCellMouseUp(this, e);
            }
        }

        //// evtable TableControlCellMouseMove GridTableControlCellMouse

        /// <summary>
        /// Occurs when the <see cref="GridControlBase.CellMouseMove"/> event of the underlying <see cref="GridTableControl"/> is raised.
        /// The original event arguments are provided with the <see cref="GridTableControlCellMouseEventArgs.Inner"/> property of the
        /// <see cref="GridTableControlCellMouseEventArgs"/> class.
        /// </summary>
        [Description("Occurs when the CellMouseMove event of the underlying GridTableControl is raised")]
        [Category("TableControl")]
        public event GridTableControlCellMouseEventHandler TableControlCellMouseMove;

        /// <summary>
        /// Raises the <see cref="TableControlCellMouseMove"/> event.
        /// </summary>
        // <param name="e">A <see cref="GridTableControlCellMouseEventArgs" /> that contains the event data.</param>
        protected virtual void OnTableControlCellMouseMove(GridTableControlCellMouseEventArgs e)
        {
            if (TableControlCellMouseMove != null)
            {
                TableControlCellMouseMove(this, e);
            }
        }

        //// evtable TableControlLeftColChanged GridTableControlRowColIndexChanged

        /// <summary>
        /// Occurs when the <see cref="GridControlBase.LeftColChanged"/> event of the underlying <see cref="GridTableControl"/> is raised.
        /// The original event arguments are provided with the <see cref="GridTableControlRowColIndexChangedEventArgs.Inner"/> property of the
        /// <see cref="GridTableControlRowColIndexChangedEventArgs"/> class.
        /// </summary>
        [Description("Occurs when the LeftColChanged event of the underlying GridTableControl is raised")]
        [Category("TableControl")]
        public event GridTableControlRowColIndexChangedEventHandler TableControlLeftColChanged;

        /// <summary>
        /// Raises the <see cref="TableControlLeftColChanged"/> event.
        /// </summary>
        // <param name="e">A <see cref="GridTableControlRowColIndexChangedEventArgs" /> that contains the event data.</param>
        protected virtual void OnTableControlLeftColChanged(GridTableControlRowColIndexChangedEventArgs e)
        {
            ////TraceUtil.TraceCurrentMethodInfo(e);

            if (TableControlLeftColChanged != null)
            {
                TableControlLeftColChanged(this, e);
            }
        }

        //// evtable TableControlCurrentCellControlKeyMessage GridTableControlCurrentCellControlKeyMessage

        /// <summary>
        /// Occurs when the <see cref="GridControlBase.CurrentCellControlKeyMessage"/> event of the underlying <see cref="GridTableControl"/> is raised.
        /// The original event arguments are provided with the <see cref="GridTableControlCurrentCellControlKeyMessageEventArgs.Inner"/> property of the
        /// <see cref="GridTableControlCurrentCellControlKeyMessageEventArgs"/> class.
        /// </summary>
        [Description("Occurs when the CurrentCellControlKeyMessage event of the underlying GridTableControl is raised")]
        [Category("TableControl")]
        public event GridTableControlCurrentCellControlKeyMessageEventHandler TableControlCurrentCellControlKeyMessage;

        /// <summary>
        /// Raises the <see cref="TableControlCurrentCellControlKeyMessage"/> event.
        /// </summary>
        // <param name="e">A <see cref="GridTableControlCurrentCellControlKeyMessageEventArgs" /> that contains the event data.</param>
        protected virtual void OnTableControlCurrentCellControlKeyMessage(GridTableControlCurrentCellControlKeyMessageEventArgs e)
        {
            if (TableControlCurrentCellControlKeyMessage != null)
            {
                TableControlCurrentCellControlKeyMessage(this, e);
            }
        }

        //// evtable TableControlCurrentCellInitializeControlText GridTableControlCurrentCellInitializeControlText

        /// <summary>
        /// Occurs when the <see cref="GridControlBase.CurrentCellInitializeControlText"/> event of the underlying <see cref="GridTableControl"/> is raised.
        /// The original event arguments are provided with the <see cref="GridTableControlCurrentCellInitializeControlTextEventArgs.Inner"/> property of the
        /// <see cref="GridTableControlCurrentCellInitializeControlTextEventArgs"/> class.
        /// </summary>
        [Description("Occurs when the CurrentCellInitializeControlText event of the underlying GridTableControl is raised")]
        [Category("TableControl")]
        public event GridTableControlCurrentCellInitializeControlTextEventHandler TableControlCurrentCellInitializeControlText;

        /// <summary>
        /// Raises the <see cref="TableControlCurrentCellInitializeControlText"/> event.
        /// </summary>
        // <param name="e">A <see cref="GridTableControlCurrentCellInitializeControlTextEventArgs" /> that contains the event data.</param>
        protected virtual void OnTableControlCurrentCellInitializeControlText(GridTableControlCurrentCellInitializeControlTextEventArgs e)
        {
            ////TraceUtil.TraceCurrentMethodInfo(e);

            if (TableControlCurrentCellInitializeControlText != null)
            {
                TableControlCurrentCellInitializeControlText(this, e);
            }
        }

        //// evtable TableControlCurrentCellErrorMessage GridTableControlCurrentCellErrorMessage

        /// <summary>
        /// Occurs when the <see cref="GridControlBase.CurrentCellErrorMessage"/> event of the underlying <see cref="GridTableControl"/> is raised.
        /// The original event arguments are provided with the <see cref="GridTableControlCurrentCellErrorMessageEventArgs.Inner"/> property of the
        /// <see cref="GridTableControlCurrentCellErrorMessageEventArgs"/> class.
        /// </summary>
        [Description("Occurs when the CurrentCellErrorMessage event of the underlying GridTableControl is raised")]
        [Category("TableControl")]
        public event GridTableControlCurrentCellErrorMessageEventHandler TableControlCurrentCellErrorMessage;

        /// <summary>
        /// Raises the <see cref="TableControlCurrentCellErrorMessage"/> event.
        /// </summary>
        // <param name="e">A <see cref="GridTableControlCurrentCellErrorMessageEventArgs" /> that contains the event data.</param>
        protected virtual void OnTableControlCurrentCellErrorMessage(GridTableControlCurrentCellErrorMessageEventArgs e)
        {
            ////TraceUtil.TraceCurrentMethodInfo(e);

            if (TableControlCurrentCellErrorMessage != null)
            {
                TableControlCurrentCellErrorMessage(this, e);
            }
        }

        //// evtable TableControlDrawCellButtonBackground GridTableControlDrawCellButtonBackground

        /// <summary>
        /// Occurs when the <see cref="GridControlBase.DrawCellButtonBackground"/> event of the underlying <see cref="GridTableControl"/> is raised.
        /// The original event arguments are provided with the <see cref="GridTableControlDrawCellButtonBackgroundEventArgs.Inner"/> property of the
        /// <see cref="GridTableControlDrawCellButtonBackgroundEventArgs"/> class.
        /// </summary>
        [Description("Occurs when the DrawCellButtonBackground event of the underlying GridTableControl is raised")]
        [Category("TableControl")]
        public event GridTableControlDrawCellButtonBackgroundEventHandler TableControlDrawCellButtonBackground;

        /// <summary>
        /// Raises the <see cref="TableControlDrawCellButtonBackground"/> event.
        /// </summary>
        // <param name="e">A <see cref="GridTableControlDrawCellButtonBackgroundEventArgs" /> that contains the event data.</param>
        protected virtual void OnTableControlDrawCellButtonBackground(GridTableControlDrawCellButtonBackgroundEventArgs e)
        {
            if (TableControlDrawCellButtonBackground != null)
            {
                TableControlDrawCellButtonBackground(this, e);
            }
        }

        //// evtable TableControlPrepareViewStyleInfo GridTableControlPrepareViewStyleInfo

        /// <summary>
        /// Occurs when the <see cref="GridControlBase.PrepareViewStyleInfo"/> event of the underlying <see cref="GridTableControl"/> is raised.
        /// The original event arguments are provided with the <see cref="GridTableControlPrepareViewStyleInfoEventArgs.Inner"/> property of the
        /// <see cref="GridTableControlPrepareViewStyleInfoEventArgs"/> class.
        /// </summary>
        [Description("Occurs when the PrepareViewStyleInfo event of the underlying GridTableControl is raised")]
        [Category("TableControl")]
        public event GridTableControlPrepareViewStyleInfoEventHandler TableControlPrepareViewStyleInfo;

        /// <summary>
        /// Raises the <see cref="TableControlPrepareViewStyleInfo"/> event.
        /// </summary>
        // <param name="e">A <see cref="GridTableControlPrepareViewStyleInfoEventArgs" /> that contains the event data.</param>
        protected virtual void OnTableControlPrepareViewStyleInfo(GridTableControlPrepareViewStyleInfoEventArgs e)
        {
            if (TableControlPrepareViewStyleInfo != null)
            {
                TableControlPrepareViewStyleInfo(this, e);
            }

            if (!e.Inner.Cancel)
            {
                if (gridHelper != null)
                {
                    gridHelper.OnGridTableControlPrepareViewStyleInfo(this, e);
                }
            }
            if (FreezeCaption && this.TopLevelGroupOptions.ShowCaption)
            {
                captionGrid.Size = new System.Drawing.Size(this.Width - SystemInformation.VerticalScrollBarWidth, this.tableControl1.Model.RowHeights[1]);
                captionGrid.RowHeights[1] = this.tableControl1.Model.RowHeights[1];
                captionGrid.ColWidths[1] = this.Width - SystemInformation.VerticalScrollBarWidth;
            }
            captionGrid.Visible = this.FreezeCaption && this.TopLevelGroupOptions.ShowCaption;
             
        }

        //// evtable TableControlDrawCellButton GridTableControlDrawCellButton

        /// <summary>
        /// Occurs when the <see cref="GridControlBase.DrawCellButton"/> event of the underlying <see cref="GridTableControl"/> is raised.
        /// The original event arguments are provided with the <see cref="GridTableControlDrawCellButtonEventArgs.Inner"/> property of the
        /// <see cref="GridTableControlDrawCellButtonEventArgs"/> class.
        /// </summary>
        [Description("Occurs when the DrawCellButton event of the underlying GridTableControl is raised")]
        [Category("TableControl")]
        public event GridTableControlDrawCellButtonEventHandler TableControlDrawCellButton;

        /// <summary>
        /// Raises the <see cref="TableControlDrawCellButton"/> event.
        /// </summary>
        // <param name="e">A <see cref="GridTableControlDrawCellButtonEventArgs" /> that contains the event data.</param>
        protected virtual void OnTableControlDrawCellButton(GridTableControlDrawCellButtonEventArgs e)
        {
            if (TableControlDrawCellButton != null)
            {
                TableControlDrawCellButton(this, e);
            }
        }

        //// evtable TableControlDrawCellBackground GridTableControlDrawCellBackground

        /// <summary>
        /// Occurs when the <see cref="GridControlBase.DrawCellBackground"/> event of the underlying <see cref="GridTableControl"/> is raised.
        /// The original event arguments are provided with the <see cref="GridTableControlDrawCellBackgroundEventArgs.Inner"/> property of the
        /// <see cref="GridTableControlDrawCellBackgroundEventArgs"/> class.
        /// </summary>
        [Description("Occurs when the DrawCellBackground event of the underlying GridTableControl is raised")]
        [Category("TableControl")]
        public event GridTableControlDrawCellBackgroundEventHandler TableControlDrawCellBackground;

        /// <summary>
        /// Raises the <see cref="TableControlDrawCellBackground"/> event.
        /// </summary>
        // <param name="e">A <see cref="GridTableControlDrawCellBackgroundEventArgs" /> that contains the event data.</param>
        protected virtual void OnTableControlDrawCellBackground(GridTableControlDrawCellBackgroundEventArgs e)
        {
            if (TableControlDrawCellBackground != null)
            {
                TableControlDrawCellBackground(this, e);
            }
        }

        //// evtable TableControlDrawCellDisplayText GridTableControlDrawCellDisplayText

        /// <summary>
        /// Occurs when the <see cref="GridControlBase.DrawCellDisplayText"/> event of the underlying <see cref="GridTableControl"/> is raised.
        /// The original event arguments are provided with the <see cref="GridTableControlDrawCellDisplayTextEventArgs.Inner"/> property of the
        /// <see cref="GridTableControlDrawCellDisplayTextEventArgs"/> class.
        /// </summary>
        [Description("Occurs when the DrawCellDisplayText event of the underlying GridTableControl is raised")]
        [Category("TableControl")]
        public event GridTableControlDrawCellDisplayTextEventHandler TableControlDrawCellDisplayText;

        /// <summary>
        /// Raises the <see cref="TableControlDrawCellDisplayText"/> event.
        /// </summary>
        // <param name="e">A <see cref="GridTableControlDrawCellDisplayTextEventArgs" /> that contains the event data.</param>
        private double value;
        /// <summary>
        /// Determine the display text.
        /// </summary>
        /// <param name="e"></param>
        protected virtual void OnTableControlDrawCellDisplayText(GridTableControlDrawCellDisplayTextEventArgs e)
        {
            if (TableControlDrawCellDisplayText != null)
            {
                TableControlDrawCellDisplayText(this, e);
            }
            GridTableCellStyleInfo style =(GridTableCellStyleInfo)e.Inner.Style;
            if ((style.TableCellIdentity.TableCellType == GridTableCellType.SummaryFieldCell || style.TableCellIdentity.TableCellType == GridTableCellType.GroupCaptionSummaryCell)&& !string.IsNullOrEmpty(e.Inner.Style.Format)
                && e.Inner.Style.Text != null && double.TryParse(e.Inner.Style.Text, out value))
            {
                e.Inner.DisplayText = string.Format("{0:" + e.Inner.Style.Format + "}", value);
            }

            if (style.TableCellIdentity.TableCellType == GridTableCellType.GroupCaptionCell && this.FreezeCaption)
            {
                capRow = style.TableCellIdentity.DisplayElement as GridCaptionRow;
                if (capRow != null)
                {
                    g = capRow.ParentGroup;
                    if (g != null && g.IsTopLevelGroup)
                        captionGrid[1, 1].Text = e.Inner.Style.Text;
                }
            }

            if (e.Inner.Cancel || !e.TableControl.Table.TableOptions.DrawTextWithGdiInterop)
            {
                return;
            }

            ////using (MeasureTime.Measure("Form1.gridGroupingControl_TableControlDrawCellDisplayText"))
            {
                if (style.TableCellIdentity.TableCellType == GridTableCellType.ColumnHeaderCell)
                {
                    return;
                }

                bool useGDIExtTextOutInsteadOfDrawText = style.Trimming == StringTrimming.None;

                // check if we need clipping
                if (useGDIExtTextOutInsteadOfDrawText)
                {
                    GridColumnDescriptor column = style.TableCellIdentity.Column;
                    GridTable tb = style.TableCellIdentity.Table;

                    if (column != null)
                    {
                        float wgWidth = column.AvgCharWidth;
                        if (e.Inner.DisplayText.Length * wgWidth > e.Inner.TextRectangle.Width)
                        {
                            useGDIExtTextOutInsteadOfDrawText = false;
                        }

                        if (!useGDIExtTextOutInsteadOfDrawText)
                        {
                            e.Inner.Style.Trimming = StringTrimming.Character;
                        }
                    }
                }

                bool isRightToLeft = (style.RightToLeft == RightToLeft.Inherit && RightToLeft == RightToLeft.Yes) || style.RightToLeft == RightToLeft.Yes;

                e.Inner.Cancel = GridGdiPaint.Instance.DrawText(e.Inner.Graphics, e.Inner.DisplayText, e.Inner.TextRectangle, e.Inner.Style, e.Inner.ClipBounds, isRightToLeft);
            }
        }

        //// evtable TableControlCellMouseHoverEnter GridTableControlCellMouse

        /// <summary>
        /// Occurs when the <see cref="GridControlBase.CellMouseHoverEnter"/> event of the underlying <see cref="GridTableControl"/> is raised.
        /// The original event arguments are provided with the <see cref="GridTableControlCellMouseEventArgs.Inner"/> property of the
        /// <see cref="GridTableControlCellMouseEventArgs"/> class.
        /// </summary>
        [Description("Occurs when the CellMouseHoverEnter event of the underlying GridTableControl is raised")]
        [Category("TableControl")]
        public event GridTableControlCellMouseEventHandler TableControlCellMouseHoverEnter;

        /// <summary>
        /// Raises the <see cref="TableControlCellMouseHoverEnter"/> event.
        /// </summary>
        // <param name="e">A <see cref="GridTableControlCellMouseEventArgs" /> that contains the event data.</param>
        protected virtual void OnTableControlCellMouseHoverEnter(GridTableControlCellMouseEventArgs e)
        {
            ////    TraceUtil.TraceCurrentMethodInfo(e);

            if (TableControlCellMouseHoverEnter != null)
            {
                TableControlCellMouseHoverEnter(this, e);
            }
        }

        //// evtable TableControlMoveCurrentCellDirection GridTableControlMoveCurrentCellDirection

        /// <summary>
        /// Occurs when the <see cref="GridControlBase.MoveCurrentCellDirection"/> event of the underlying <see cref="GridTableControl"/> is raised.
        /// The original event arguments are provided with the <see cref="GridTableControlMoveCurrentCellDirectionEventArgs.Inner"/> property of the
        /// <see cref="GridTableControlMoveCurrentCellDirectionEventArgs"/> class.
        /// </summary>
        [Description("Occurs when the MoveCurrentCellDirection event of the underlying GridTableControl is raised")]
        [Category("TableControl")]
        public event GridTableControlMoveCurrentCellDirectionEventHandler TableControlMoveCurrentCellDirection;

        /// <summary>
        /// Raises the <see cref="TableControlMoveCurrentCellDirection"/> event.
        /// </summary>
        // <param name="e">A <see cref="GridTableControlMoveCurrentCellDirectionEventArgs" /> that contains the event data.</param>
        protected virtual void OnTableControlMoveCurrentCellDirection(GridTableControlMoveCurrentCellDirectionEventArgs e)
        {
            if (TableControlMoveCurrentCellDirection != null)
            {
                TableControlMoveCurrentCellDirection(this, e);
            }

            if (!e.Inner.Handled)
            {
                TableControl.ProcessTableControlMoveCurrentCellDirection(e);
            }
        }

        //// evtable TableControlQueryNextCurrentCellPosition GridTableControlQueryNextCurrentCellPosition

        /// <summary>
        /// Occurs when the <see cref="GridControlBase.QueryNextCurrentCellPosition"/> event of the underlying <see cref="GridTableControl"/> is raised.
        /// The original event arguments are provided with the <see cref="GridTableControlQueryNextCurrentCellPositionEventArgs.Inner"/> property of the
        /// <see cref="GridTableControlQueryNextCurrentCellPositionEventArgs"/> class.
        /// </summary>
        [Description("Occurs when the QueryNextCurrentCellPosition event of the underlying GridTableControl is raised")]
        [Category("TableControl")]
        public event GridTableControlQueryNextCurrentCellPositionEventHandler TableControlQueryNextCurrentCellPosition;

        /// <summary>
        /// Raises the <see cref="TableControlQueryNextCurrentCellPosition"/> event.
        /// </summary>
        // <param name="e">A <see cref="GridTableControlQueryNextCurrentCellPositionEventArgs" /> that contains the event data.</param>
        protected virtual void OnTableControlQueryNextCurrentCellPosition(GridTableControlQueryNextCurrentCellPositionEventArgs e)
        {
            if (TableControlQueryNextCurrentCellPosition != null)
            {
                TableControlQueryNextCurrentCellPosition(this, e);
            }
        }

        //// evtable TableControlTopRowChanged GridTableControlRowColIndexChanged

        /// <summary>
        /// Occurs when the <see cref="GridControlBase.TopRowChanged"/> event of the underlying <see cref="GridTableControl"/> is raised.
        /// The original event arguments are provided with the <see cref="GridTableControlRowColIndexChangedEventArgs.Inner"/> property of the
        /// <see cref="GridTableControlRowColIndexChangedEventArgs"/> class.
        /// </summary>
        [Description("Occurs when the TopRowChanged event of the underlying GridTableControl is raised")]
        [Category("TableControl")]
        public event GridTableControlRowColIndexChangedEventHandler TableControlTopRowChanged;

        /// <summary>
        /// Raises the <see cref="TableControlTopRowChanged"/> event.
        /// </summary>
        // <param name="e">A <see cref="GridTableControlRowColIndexChangedEventArgs" /> that contains the event data.</param>
        protected virtual void OnTableControlTopRowChanged(GridTableControlRowColIndexChangedEventArgs e)
        {
            if (TableControlTopRowChanged != null)
            {
                TableControlTopRowChanged(this, e);
            }

            if (gridHelper != null)
            {
                gridHelper.OnGridTableControlTopRowChanged(this, e);
            }
        }

        //// evtable TableControlResizingColumns GridTableControlResizingColumns

        /// <summary>
        /// Occurs when the <see cref="GridControlBase.ResizingColumns"/> event of the underlying <see cref="GridTableControl"/> is raised.
        /// The original event arguments are provided with the <see cref="GridTableControlResizingColumnsEventArgs.Inner"/> property of the
        /// <see cref="GridTableControlResizingColumnsEventArgs"/> class.
        /// </summary>
        [Description("Occurs when the ResizingColumns event of the underlying GridTableControl is raised")]
        [Category("TableControl")]
        public event GridTableControlResizingColumnsEventHandler TableControlResizingColumns;

        /// <summary>
        /// Raises the <see cref="TableControlResizingColumns"/> event.
        /// </summary>
        // <param name="e">A <see cref="GridTableControlResizingColumnsEventArgs" /> that contains the event data.</param>
        protected virtual void OnTableControlResizingColumns(GridTableControlResizingColumnsEventArgs e)
        {
            if (AllowProportionalColumnSizing)
            {   
                if (e.Inner.Reason == Syncfusion.Windows.Forms.Grid.GridResizeCellsReason.MouseUp)
                {
                    string name = this.TableDescriptor.VisibleColumns[e.Inner.Columns.Left - 1].Name;
                    if (!sizedColumns.ContainsKey(name))
                    {
                        if (comparingClientSize(name, e.Inner.Width, this.TableDescriptor.Columns[name].Width))
                            e.Inner.Cancel = true;                        
                    }
                    else
                    {
                        if (comparingClientSize(name, e.Inner.Width, this.TableDescriptor.Columns[name].Width))
                            e.Inner.Cancel = true;
                    }
                  
                }
                else if (e.Inner.Reason == Syncfusion.Windows.Forms.Grid.GridResizeCellsReason.DoubleClick)
                {
                    e.TableControl.Model.ColWidths[e.Inner.Columns.Left] = 5; //set it to some minimun and then size up to optimal size
                    e.TableControl.Model.ColWidths.ResizeToFit(GridRangeInfo.Col(e.Inner.Columns.Left));
                    int size = e.TableControl.Model.ColWidths[e.Inner.Columns.Left];
                    e.Inner.Cancel = true; //we handled it...
                    string name = this.TableDescriptor.VisibleColumns[e.Inner.Columns.Left - 1].Name;
                    if (!sizedColumns.ContainsKey(name))
                    {
                        if (comparingClientSize(name, size, this.TableDescriptor.Columns[name].Width))
                            e.Inner.Cancel = true;
                    }
                    else
                    {
                        if (comparingClientSize(name, size, this.TableDescriptor.Columns[name].Width))
                            e.Inner.Cancel = true;
                    }                   
                }
            }
            else
            {
                if (e.Inner.Reason == GridResizeCellsReason.DoubleClick)
                {
                    if (this.TableModel.HideCols[e.Inner.Columns.Left + 1] || this.TableModel.ColWidths[e.Inner.Columns.Left + 1] == 0)
                    {
                        e.Inner.Cancel = true;
                        return;
                    }
                }
            }
            if (e.Inner.Reason == GridResizeCellsReason.HitTest && e.Inner.Columns.Right < this.TableDescriptor.GetColumnIndentCount())
            {
                e.Inner.Cancel = true;
            }

            ////TraceUtil.TraceCurrentMethodInfo(e);

            if (TableControlResizingColumns != null)
            {
                TableControlResizingColumns(this, e);
            }
        }
        /// <summary>
        /// compare the grid client size to change the column width with new column width allow resizing to fit.
        /// </summary>
        /// <param name="colName">Column Index </param>
        /// <param name="newColWidth"> new column with</param>
        /// <param name="oldColWidth">column width before change the size of column</param>
        /// <returns>returns true value.</returns>
        private bool comparingClientSize(string colName, int newColWidth,int oldColWidth)
        {
            int colIndex = this.TableDescriptor.VisibleColumns.IndexOf(colName);
            int clientWidth = this.TableControl.ClientSize.Width;
            int vColCount = this.TableDescriptor.VisibleColumns.Count;
            int frozenCount = TableControl.InternalGetFrozenCols();
            int frozen = TableControl.Model.ColWidths.GetTotal(0, frozenCount);
            int resizedRightColWidth = 0;
            string name = string.Empty;
            int rightColWidth=0;
            int resizedLeftColWidth = 0;            
            int leftColWidth=0;
            int rightColCount = 0;
            int rightResizedColCount = 0;
            int leftColCount = 0;
            int hiddenCol = 0;
            for (int i = colIndex + 1; i < vColCount; i++)
            {
                name = this.TableDescriptor.VisibleColumns[i].Name;
                if (sizedColumns.ContainsKey(name))
                {
                    resizedRightColWidth += sizedColumns[name];
                    rightResizedColCount++;
                }
                else
                {
                    if (this.TableDescriptor.Columns[name].Width == 0)
                        hiddenCol++;
                    rightColWidth += this.TableDescriptor.Columns[name].Width;
                }
                rightColCount++;
            }
            for (int i = 0; i < colIndex; i++)
            {
                name = this.TableDescriptor.VisibleColumns[i].Name;
                if (sizedColumns.ContainsKey(name))
                {
                    resizedLeftColWidth += sizedColumns[name];
                }
                else
                {
                    if (this.TableDescriptor.Columns[name].Width == 0)
                        hiddenCol++;
                    leftColWidth += this.TableDescriptor.Columns[name].Width;
                }
                leftColCount++;
            }            
            
            int tot = clientWidth - leftColWidth - resizedLeftColWidth - newColWidth - frozen - resizedRightColWidth;
            if (tot > 0 && rightColCount > 0)
            {
                if (sizedColumns.ContainsKey(colName))
                {
                    sizedColumns.Remove(colName);
                }
                sizedColumns.Add(colName, newColWidth);
                int resizeColCount = rightColCount - rightResizedColCount - hiddenCol;
                int dx;
                if (resizeColCount == 0)
                    dx = tot;
                else
                    dx = tot / (resizeColCount);
                for (int i = colIndex; i < vColCount; i++)
                {
                    name = this.TableDescriptor.VisibleColumns[i].Name;
                    if (!(this.TableDescriptor.Columns[name].Width == 0))
                    {
                        if (!sizedColumns.ContainsKey(name))
                            this.TableDescriptor.Columns[name].Width = dx;
                        else
                            this.TableDescriptor.Columns[name].Width = sizedColumns[name];
                    }
                }
                return false;
            }
            else
                return true;
        }
        /// <summary>
        /// set the column width when AllowProportionalColumnSizing  is enable
        /// </summary>
        private void SetColumnWidths()
        {
            int width = this.TableControl.ClientSize.Width;
            int count = this.TableDescriptor.VisibleColumns.Count;
            if (count > 0 && count > sizedColumns.Count)
            {
                int frozenCount = TableControl.InternalGetFrozenCols();
                int frozen = TableControl.Model.ColWidths.GetTotal(0, frozenCount);
                int fixedSize = 0;
                string name = string.Empty;
                int hiddencolCount = 0;
                foreach (string name1 in sizedColumns.Keys)
                {
                    fixedSize += sizedColumns[name1];
                }
                for (int i = 0; i < count; ++i)
                {
                    name = this.TableDescriptor.VisibleColumns[i].Name;
                    if (this.TableDescriptor.Columns[name].Width == 0)
                        hiddencolCount++;
                }
                int dx = (width - frozen - fixedSize) / (count - sizedColumns.Count - hiddencolCount);
                name = string.Empty;
                int addedWidth = 0;
                for (int i = 0; i < count - 1; ++i)
                {
                    name = this.TableDescriptor.VisibleColumns[i].Name;
                    if (this.TableDescriptor.Columns[name].Width !=0 && !sizedColumns.ContainsKey(name))
                    {
                        this.TableDescriptor.Columns[name].Width = dx;
                        addedWidth += dx;
                    }
                }
                name = this.TableDescriptor.VisibleColumns[count - 1].Name;
                this.TableDescriptor.Columns[name].Width = width - frozen - fixedSize - addedWidth; //add all the roundoff pixels to the last column
            }
        }

        //// evtable TableControlCellCursor GridTableControlCellCursor

        /// <summary>
        /// Occurs when the <see cref="GridControlBase.CellCursor"/> event of the underlying <see cref="GridTableControl"/> is raised.
        /// The original event arguments are provided with the <see cref="GridTableControlCellCursorEventArgs.Inner"/> property of the
        /// <see cref="GridTableControlCellCursorEventArgs"/> class.
        /// </summary>
        [Description("Occurs when the CellCursor event of the underlying GridTableControl is raised")]
        [Category("TableControl")]
        public event GridTableControlCellCursorEventHandler TableControlCellCursor;

        /// <summary>
        /// Raises the <see cref="TableControlCellCursor"/> event.
        /// </summary>
        // <param name="e">A <see cref="GridTableControlCellCursorEventArgs" /> that contains the event data.</param>
        protected virtual void OnTableControlCellCursor(GridTableControlCellCursorEventArgs e)
        {
            if (TableControlCellCursor != null)
            {
                TableControlCellCursor(this, e);
            }
        }

        //// evtable TableControlQueryScrollCellInView GridTableControlQueryScrollCellInView

        /// <summary>
        /// Occurs when the <see cref="GridControlBase.QueryScrollCellInView"/> event of the underlying <see cref="GridTableControl"/> is raised.
        /// The original event arguments are provided with the <see cref="GridTableControlQueryScrollCellInViewEventArgs.Inner"/> property of the
        /// <see cref="GridTableControlQueryScrollCellInViewEventArgs"/> class.
        /// </summary>
        [Description("Occurs when the QueryScrollCellInView event of the underlying GridTableControl is raised")]
        [Category("TableControl")]
        public event GridTableControlQueryScrollCellInViewEventHandler TableControlQueryScrollCellInView;

        /// <summary>
        /// Raises the <see cref="TableControlQueryScrollCellInView"/> event.
        /// </summary>
        // <param name="e">A <see cref="GridTableControlQueryScrollCellInViewEventArgs" /> that contains the event data.</param>
        protected virtual void OnTableControlQueryScrollCellInView(GridTableControlQueryScrollCellInViewEventArgs e)
        {
            if (TableControlQueryScrollCellInView != null)
            {
                TableControlQueryScrollCellInView(this, e);
            }
        }

        //// evtable TableControlPushButtonClick GridTableControlCellPushButtonClick

        /// <summary>
        /// Occurs when the <see cref="GridControlBase.PushButtonClick"/> event of the underlying <see cref="GridTableControl"/> is raised.
        /// The original event arguments are provided with the <see cref="GridTableControlCellPushButtonClickEventArgs.Inner"/> property of the
        /// <see cref="GridTableControlCellPushButtonClickEventArgs"/> class.
        /// </summary>
        [Description("Occurs when the PushButtonClick event of the underlying GridTableControl is raised")]
        [Category("TableControl")]
        public event GridTableControlCellPushButtonClickEventHandler TableControlPushButtonClick;

        /// <summary>
        /// Raises the <see cref="TableControlPushButtonClick"/> event.
        /// </summary>
        // <param name="e">A <see cref="GridTableControlCellPushButtonClickEventArgs" /> that contains the event data.</param>
        protected virtual void OnTableControlPushButtonClick(GridTableControlCellPushButtonClickEventArgs e)
        {
            ////TraceUtil.TraceCurrentMethodInfo(e);

            if (TableControlPushButtonClick != null)
            {
                TableControlPushButtonClick(this, e);
            }
        }

        //// evtable TableControlCurrentCellActivating GridTableControlCurrentCellActivating

        /// <summary>
        /// Occurs when the <see cref="GridControlBase.CurrentCellActivating"/> event of the underlying <see cref="GridTableControl"/> is raised.
        /// The original event arguments are provided with the <see cref="GridTableControlCurrentCellActivatingEventArgs.Inner"/> property of the
        /// <see cref="GridTableControlCurrentCellActivatingEventArgs"/> class.
        /// </summary>
        [Description("Occurs when the CurrentCellActivating event of the underlying GridTableControl is raised")]
        [Category("TableControl")]
        public event GridTableControlCurrentCellActivatingEventHandler TableControlCurrentCellActivating;

        /// <summary>
        /// Raises the <see cref="TableControlCurrentCellActivating"/> event.
        /// </summary>
        /// <param name="e">A <see cref="GridTableControlCurrentCellActivatingEventArgs" /> that contains the event data.</param>
        protected virtual void OnTableControlCurrentCellActivating(GridTableControlCurrentCellActivatingEventArgs e)
        {
            ////TraceUtil.TraceCurrentMethodInfo(e);

            if (TableControlCurrentCellActivating != null)
            {
                TableControlCurrentCellActivating(this, e);
            }
        }

        //// evtable TableControlResizingRows GridTableControlResizingRows

        /// <summary>
        /// Occurs when the <see cref="GridControlBase.ResizingRows"/> event of the underlying <see cref="GridTableControl"/> is raised.
        /// The original event arguments are provided with the <see cref="GridTableControlResizingRowsEventArgs.Inner"/> property of the
        /// <see cref="GridTableControlResizingRowsEventArgs"/> class.
        /// </summary>
        [Description("Occurs when the ResizingRows event of the underlying GridTableControl is raised")]
        [Category("TableControl")]
        public event GridTableControlResizingRowsEventHandler TableControlResizingRows;

        /// <summary>
        /// Raises the <see cref="TableControlResizingRows"/> event.
        /// </summary>
        // <param name="e">A <see cref="GridTableControlResizingRowsEventArgs" /> that contains the event data.</param>
        protected virtual void OnTableControlResizingRows(GridTableControlResizingRowsEventArgs e)
        {
            ////TraceUtil.TraceCurrentMethodInfo(e);

            if (TableControlResizingRows != null)
            {
                TableControlResizingRows(this, e);
            }
        }

        //// evtable TableControlCurrentCellValidateString GridTableControlCurrentCellValidateString

        /// <summary>
        /// Occurs when the <see cref="GridControlBase.CurrentCellValidateString"/> event of the underlying <see cref="GridTableControl"/> is raised.
        /// The original event arguments are provided with the <see cref="GridTableControlCurrentCellValidateStringEventArgs.Inner"/> property of the
        /// <see cref="GridTableControlCurrentCellValidateStringEventArgs"/> class.
        /// </summary>
        [Description("Occurs when the CurrentCellValidateString event of the underlying GridTableControl is raised")]
        [Category("TableControl")]
        public event GridTableControlCurrentCellValidateStringEventHandler TableControlCurrentCellValidateString;

        /// <summary>
        /// Raises the <see cref="TableControlCurrentCellValidateString"/> event.
        /// </summary>
        // <param name="e">A <see cref="GridTableControlCurrentCellValidateStringEventArgs" /> that contains the event data.</param>
        protected virtual void OnTableControlCurrentCellValidateString(GridTableControlCurrentCellValidateStringEventArgs e)
        {
            ////TraceUtil.TraceCurrentMethodInfo(e);

            if (TableControlCurrentCellValidateString != null)
            {
                TableControlCurrentCellValidateString(this, e);
            }
        }

        //// evtable TableControlCurrentCellMoved GridTableControlCurrentCellMoved

        /// <summary>
        /// Occurs when the <see cref="GridControlBase.CurrentCellMoved"/> event of the underlying <see cref="GridTableControl"/> is raised.
        /// The original event arguments are provided with the <see cref="GridTableControlCurrentCellMovedEventArgs.Inner"/> property of the
        /// <see cref="GridTableControlCurrentCellMovedEventArgs"/> class.
        /// </summary>
        [Description("Occurs when the CurrentCellMoved event of the underlying GridTableControl is raised")]
        [Category("TableControl")]
        public event GridTableControlCurrentCellMovedEventHandler TableControlCurrentCellMoved;

        /// <summary>
        /// Raises the <see cref="TableControlCurrentCellMoved"/> event.
        /// </summary>
        // <param name="e">A <see cref="GridTableControlCurrentCellMovedEventArgs" /> that contains the event data.</param>
        protected virtual void OnTableControlCurrentCellMoved(GridTableControlCurrentCellMovedEventArgs e)
        {
            ////TraceUtil.TraceCurrentMethodInfo(e);

            if (TableControlCurrentCellMoved != null)
            {
                TableControlCurrentCellMoved(this, e);
            }
        }

        //// evtable TableControlCurrentCellActivateFailed GridTableControlCurrentCellActivateFailed

        /// <summary>
        /// Occurs when the <see cref="GridControlBase.CurrentCellActivateFailed"/> event of the underlying <see cref="GridTableControl"/> is raised.
        /// The original event arguments are provided with the <see cref="GridTableControlCurrentCellActivateFailedEventArgs.Inner"/> property of the
        /// <see cref="GridTableControlCurrentCellActivateFailedEventArgs"/> class.
        /// </summary>
        [Description("Occurs when the CurrentCellActivateFailed event of the underlying GridTableControl is raised")]
        [Category("TableControl")]
        public event GridTableControlCurrentCellActivateFailedEventHandler TableControlCurrentCellActivateFailed;

        /// <summary>
        /// Raises the <see cref="TableControlCurrentCellActivateFailed"/> event.
        /// </summary>
        // <param name="e">A <see cref="GridTableControlCurrentCellActivateFailedEventArgs" /> that contains the event data.</param>
        protected virtual void OnTableControlCurrentCellActivateFailed(GridTableControlCurrentCellActivateFailedEventArgs e)
        {
            ////TraceUtil.TraceCurrentMethodInfo(e);

            if (TableControlCurrentCellActivateFailed != null)
            {
                TableControlCurrentCellActivateFailed(this, e);
            }
        }

        //// evtable TableControlCellHitTest GridTableControlCellHitTest

        /// <summary>
        /// Occurs when the <see cref="GridControlBase.CellHitTest"/> event of the underlying <see cref="GridTableControl"/> is raised.
        /// The original event arguments are provided with the <see cref="GridTableControlCellHitTestEventArgs.Inner"/> property of the
        /// <see cref="GridTableControlCellHitTestEventArgs"/> class.
        /// </summary>
        [Description("Occurs when the CellHitTest event of the underlying GridTableControl is raised")]
        [Category("TableControl")]
        public event GridTableControlCellHitTestEventHandler TableControlCellHitTest;

        /// <summary>
        /// Raises the <see cref="TableControlCellHitTest"/> event.
        /// </summary>
        // <param name="e">A <see cref="GridTableControlCellHitTestEventArgs" /> that contains the event data.</param>
        protected virtual void OnTableControlCellHitTest(GridTableControlCellHitTestEventArgs e)
        {
            if (TableControlCellHitTest != null)
            {
                TableControlCellHitTest(this, e);
            }
        }

        //// evtable TableControlCellMouseHover GridTableControlCellMouse

        /// <summary>
        /// Occurs when the <see cref="GridControlBase.CellMouseHover"/> event of the underlying <see cref="GridTableControl"/> is raised.
        /// The original event arguments are provided with the <see cref="GridTableControlCellMouseEventArgs.Inner"/> property of the
        /// <see cref="GridTableControlCellMouseEventArgs"/> class.
        /// </summary>
        [Description("Occurs when the CellMouseHover event of the underlying GridTableControl is raised")]
        [Category("TableControl")]
        public event GridTableControlCellMouseEventHandler TableControlCellMouseHover;

        /// <summary>
        /// Raises the <see cref="TableControlCellMouseHover"/> event.
        /// </summary>
        // <param name="e">A <see cref="GridTableControlCellMouseEventArgs" /> that contains the event data.</param>
        protected virtual void OnTableControlCellMouseHover(GridTableControlCellMouseEventArgs e)
        {
            if (TableControlCellMouseHover != null)
            {
                TableControlCellMouseHover(this, e);
            }
        }

        //// evtable TableControlVScrollPixelPosChanging GridTableControlScrollPositionChanging

        /// <summary>
        /// Occurs when the <see cref="GridControlBase.VScrollPixelPosChanging"/> event of the underlying <see cref="GridTableControl"/> is raised.
        /// The original event arguments are provided with the <see cref="GridTableControlScrollPositionChangingEventArgs.Inner"/> property of the
        /// <see cref="GridTableControlScrollPositionChangingEventArgs"/> class.
        /// </summary>
        [Description("Occurs when the VScrollPixelPosChanging event of the underlying GridTableControl is raised")]
        [Category("TableControl")]
        public event GridTableControlScrollPositionChangingEventHandler TableControlVScrollPixelPosChanging;

        /// <summary>
        /// Raises the <see cref="TableControlVScrollPixelPosChanging"/> event.
        /// </summary>
        // <param name="e">A <see cref="GridTableControlScrollPositionChangingEventArgs" /> that contains the event data.</param>
        protected virtual void OnTableControlVScrollPixelPosChanging(GridTableControlScrollPositionChangingEventArgs e)
        {
            if (TableControlVScrollPixelPosChanging != null)
            {
                TableControlVScrollPixelPosChanging(this, e);
            }
        }

        //// evtable TableControlCellDrawn GridTableControlDrawCell

        /// <summary>
        /// Occurs when the <see cref="GridControlBase.CellDrawn"/> event of the underlying <see cref="GridTableControl"/> is raised.
        /// The original event arguments are provided with the <see cref="GridTableControlDrawCellEventArgs.Inner"/> property of the
        /// <see cref="GridTableControlDrawCellEventArgs"/> class.
        /// </summary>
        [Description("Occurs when the CellDrawn event of the underlying GridTableControl is raised")]
        [Category("TableControl")]
        public event GridTableControlDrawCellEventHandler TableControlCellDrawn;
        private bool intialized = false;
        /// <summary>
        /// Raises the <see cref="TableControlCellDrawn"/> event.
        /// </summary>
        // <param name="e">A <see cref="GridTableControlDrawCellEventArgs" /> that contains the event data.</param>
        protected virtual void OnTableControlCellDrawn(GridTableControlDrawCellEventArgs e)
        {
            if (AllowProportionalColumnSizing && !intialized)
            {
                SetColumnWidths();
                intialized = true ;
            }

            if (TableControlCellDrawn != null)
            {
                TableControlCellDrawn(this, e);
            }

            if (!e.Inner.Cancel)
            {
                if (gridHelper != null)
                {
                    gridHelper.OnGridTableControlCellDrawn(this, e);
                }
            }
        }

        //// evtable TableControlCurrentCellMoving GridTableControlCurrentCellMoving

        /// <summary>
        /// Occurs when the <see cref="GridControlBase.CurrentCellMoving"/> event of the underlying <see cref="GridTableControl"/> is raised.
        /// The original event arguments are provided with the <see cref="GridTableControlCurrentCellMovingEventArgs.Inner"/> property of the
        /// <see cref="GridTableControlCurrentCellMovingEventArgs"/> class.
        /// </summary>
        [Description("Occurs when the CurrentCellMoving event of the underlying GridTableControl is raised")]
        [Category("TableControl")]
        public event GridTableControlCurrentCellMovingEventHandler TableControlCurrentCellMoving;

        /// <summary>
        /// Raises the <see cref="TableControlCurrentCellMoving"/> event.
        /// </summary>
        // <param name="e">A <see cref="GridTableControlCurrentCellMovingEventArgs" /> that contains the event data.</param>
        protected virtual void OnTableControlCurrentCellMoving(GridTableControlCurrentCellMovingEventArgs e)
        {
            ////TraceUtil.TraceCurrentMethodInfo(e);

            if (TableControlCurrentCellMoving != null)
            {
                TableControlCurrentCellMoving(this, e);
            }
        }

        //// evtable TableControlCurrentCellShowingDropDown GridTableControlCurrentCellShowingDropDown

        /// <summary>
        /// Occurs when the <see cref="GridControlBase.CurrentCellShowingDropDown"/> event of the underlying <see cref="GridTableControl"/> is raised.
        /// The original event arguments of the event are provided with the <see cref="GridTableControlCurrentCellShowingDropDownEventArgs.Inner"/> property of the
        /// <see cref="GridTableControlCurrentCellShowingDropDownEventArgs"/> class.
        /// </summary>
        [Description("Occurs when the CurrentCellShowingDropDown event of the underlying GridTableControl is raised")]
        [Category("TableControl")]
        public event GridTableControlCurrentCellShowingDropDownEventHandler TableControlCurrentCellShowingDropDown;

        /// <summary>
        /// Raises the <see cref="TableControlCurrentCellShowingDropDown"/> event.
        /// </summary>
        // <param name="e">A <see cref="GridTableControlCurrentCellShowingDropDownEventArgs" /> that contains the event data.</param>
        protected virtual void OnTableControlCurrentCellShowingDropDown(GridTableControlCurrentCellShowingDropDownEventArgs e)
        {
            ////TraceUtil.TraceCurrentMethodInfo(e);

            if (TableControlCurrentCellShowingDropDown != null)
            {
                TableControlCurrentCellShowingDropDown(this, e);
            }
            if (this.BrowseOnly)
                e.Inner.Cancel = true;
        }

        //// evtable TableControlCellClick GridTableControlCellClick

        /// <summary>
        /// Occurs when the <see cref="GridControlBase.CellClick"/> event of the underlying <see cref="GridTableControl"/> is raised.
        /// The original event arguments of the event are provided with the <see cref="GridTableControlCellClickEventArgs.Inner"/> property of the
        /// <see cref="GridTableControlCellClickEventArgs"/> class.
        /// </summary>
        [Description("Occurs when the CellClick event of the underlying GridTableControl is raised")]
        [Category("TableControl")]
        public event GridTableControlCellClickEventHandler TableControlCellClick;

        /// <summary>
        /// Raises the <see cref="TableControlCellClick"/> event.
        /// </summary>
        // <param name="e">A <see cref="GridTableControlCellClickEventArgs" /> that contains the event data.</param>
        protected virtual void OnTableControlCellClick(GridTableControlCellClickEventArgs e)
        {
            ////TraceUtil.TraceCurrentMethodInfo(e);

            if (TableControlCellClick != null)
            {
                TableControlCellClick(this, e);
            }
        }

        //// evtable TableControlCellButtonClicked GridTableControlCellButtonClicked

        /// <summary>
        /// Occurs when the <see cref="GridControlBase.CellButtonClicked"/> event of the underlying <see cref="GridTableControl"/> is raised.
        /// The original event arguments are provided with the <see cref="GridTableControlCellButtonClickedEventArgs.Inner"/> property of the
        /// <see cref="GridTableControlCellButtonClickedEventArgs"/> class.
        /// </summary>
        [Description("Occurs when the CellButtonClicked event of the underlying GridTableControl is raised")]
        [Category("TableControl")]
        public event GridTableControlCellButtonClickedEventHandler TableControlCellButtonClicked;

        /// <summary>
        /// Raises the <see cref="TableControlCellButtonClicked"/> event.
        /// </summary>
        // <param name="e">A <see cref="GridTableControlCellButtonClickedEventArgs" /> that contains the event data.</param>
        protected virtual void OnTableControlCellButtonClicked(GridTableControlCellButtonClickedEventArgs e)
        {
            ////TraceUtil.TraceCurrentMethodInfo(e);

            if (TableControlCellButtonClicked != null)
            {
                TableControlCellButtonClicked(this, e);
            }
        }

        //// evtable TableControlDrawCell GridTableControlDrawCell

        /// <summary>
        /// Occurs when the <see cref="GridControlBase.DrawCell"/> event of the underlying <see cref="GridTableControl"/> is raised.
        /// The original event arguments are provided with the <see cref="GridTableControlDrawCellEventArgs.Inner"/> property of the
        /// <see cref="GridTableControlDrawCellEventArgs"/> class.
        /// </summary>
        [Description("Occurs when the DrawCell event of the underlying GridTableControl is raised")]
        [Category("TableControl")]
        public event GridTableControlDrawCellEventHandler TableControlDrawCell;

        /// <summary>
        /// Raises the <see cref="TableControlDrawCell"/> event.
        /// </summary>
        // <param name="e">A <see cref="GridTableControlDrawCellEventArgs" /> that contains the event data.</param>
        protected virtual void OnTableControlDrawCell(GridTableControlDrawCellEventArgs e)
        {
            if (TableControlDrawCell != null)
            {
                TableControlDrawCell(this, e);
            }
        }

        //// evtable TableControlCheckBoxClick GridTableControlCellClick

        /// <summary>
        /// Occurs when the <see cref="GridControlBase.CheckBoxClick"/> event of the underlying <see cref="GridTableControl"/> is raised.
        /// The original event arguments are provided with the <see cref="GridTableControlCellClickEventArgs.Inner"/> property of the
        /// <see cref="GridTableControlCellClickEventArgs"/> class.
        /// </summary>
        [Description("Occurs when the CheckBoxClick event of the underlying GridTableControl is raised")]
        [Category("TableControl")]
        public event GridTableControlCellClickEventHandler TableControlCheckBoxClick;

        /// <summary>
        /// Raises the <see cref="TableControlCheckBoxClick"/> event.
        /// </summary>
        // <param name="e">A <see cref="GridTableControlCellClickEventArgs" /> that contains the event data.</param>
        protected virtual void OnTableControlCheckBoxClick(GridTableControlCellClickEventArgs e)
        {
            ////TraceUtil.TraceCurrentMethodInfo(e);

            if (TableControlCheckBoxClick != null)
            {
                TableControlCheckBoxClick(this, e);
            }
        }

        //// evtable TableControlLeftColChanging GridTableControlRowColIndexChanging

        /// <summary>
        /// Occurs when the <see cref="GridControlBase.LeftColChanging"/> event of the underlying <see cref="GridTableControl"/> is raised.
        /// The original event arguments are provided with the <see cref="GridTableControlRowColIndexChangingEventArgs.Inner"/> property of the
        /// <see cref="GridTableControlRowColIndexChangingEventArgs"/> class.
        /// </summary>
        [Description("Occurs when the LeftColChanging event of the underlying GridTableControl is raised")]
        [Category("TableControl")]
        public event GridTableControlRowColIndexChangingEventHandler TableControlLeftColChanging;

        /// <summary>
        /// Raises the <see cref="TableControlLeftColChanging"/> event.
        /// </summary>
        // <param name="e">A <see cref="GridTableControlRowColIndexChangingEventArgs" /> that contains the event data.</param>
        protected virtual void OnTableControlLeftColChanging(GridTableControlRowColIndexChangingEventArgs e)
        {
            if (TableControlLeftColChanging != null)
            {
                TableControlLeftColChanging(this, e);
            }
        }

        //// evtable TableControlCellDoubleClick GridTableControlCellClick

        /// <summary>
        /// Occurs when the <see cref="GridControlBase.CellDoubleClick"/> event of the underlying <see cref="GridTableControl"/> is raised.
        /// The original event arguments are provided with the <see cref="GridTableControlCellClickEventArgs.Inner"/> property of the
        /// <see cref="GridTableControlCellClickEventArgs"/> class.
        /// </summary>
        [Description("Occurs when the CellDoubleClick event of the underlying GridTableControl is raised")]
        [Category("TableControl")]
        public event GridTableControlCellClickEventHandler TableControlCellDoubleClick;

        /// <summary>
        /// Raises the <see cref="TableControlCellDoubleClick"/> event.
        /// </summary>
        // <param name="e">A <see cref="GridTableControlCellClickEventArgs" /> that contains the event data.</param>
        protected virtual void OnTableControlCellDoubleClick(GridTableControlCellClickEventArgs e)
        {
            ////TraceUtil.TraceCurrentMethodInfo(e);

            if (TableControlCellDoubleClick != null)
            {
                TableControlCellDoubleClick(this, e);
            }
        }

        //// evtable TableControlWrapCellNextControlInForm GridTableControlWrapCellNextControlInForm

        /// <summary>
        /// Occurs when the <see cref="GridControlBase.WrapCellNextControlInForm"/> event of the underlying <see cref="GridTableControl"/> is raised.
        /// The original event arguments are provided with the <see cref="GridTableControlWrapCellNextControlInFormEventArgs.Inner"/> property of the
        /// <see cref="GridTableControlWrapCellNextControlInFormEventArgs"/> class.
        /// </summary>
        [Description("Occurs when the WrapCellNextControlInForm event of the underlying GridTableControl is raised")]
        [Category("TableControl")]
        public event GridTableControlWrapCellNextControlInFormEventHandler TableControlWrapCellNextControlInForm;

        /// <summary>
        /// Raises the <see cref="TableControlWrapCellNextControlInForm"/> event.
        /// </summary>
        // <param name="e">A <see cref="GridTableControlWrapCellNextControlInFormEventArgs" /> that contains the event data.</param>
        protected virtual void OnTableControlWrapCellNextControlInForm(GridTableControlWrapCellNextControlInFormEventArgs e)
        {
            if (TableControlWrapCellNextControlInForm != null)
            {
                TableControlWrapCellNextControlInForm(this, e);
            }
        }

        //// evtable TableControlDrawCurrentCellBorder GridTableControlDrawCurrentCellBorder

        /// <summary>
        /// Occurs when the <see cref="GridControlBase.DrawCurrentCellBorder"/> event of the underlying <see cref="GridTableControl"/> is raised.
        /// The original event arguments are provided with the <see cref="GridTableControlDrawCurrentCellBorderEventArgs.Inner"/> property of the
        /// <see cref="GridTableControlDrawCurrentCellBorderEventArgs"/> class.
        /// </summary>
        [Description("Occurs when the DrawCurrentCellBorder event of the underlying GridTableControl is raised")]
        [Category("TableControl")]
        public event GridTableControlDrawCurrentCellBorderEventHandler TableControlDrawCurrentCellBorder;

        /// <summary>
        /// Raises the <see cref="TableControlDrawCurrentCellBorder"/> event.
        /// </summary>
        // <param name="e">A <see cref="GridTableControlDrawCurrentCellBorderEventArgs" /> that contains the event data.</param>
        protected virtual void OnTableControlDrawCurrentCellBorder(GridTableControlDrawCurrentCellBorderEventArgs e)
        {
            if (TableControlDrawCurrentCellBorder != null)
            {
                TableControlDrawCurrentCellBorder(this, e);
            }
        }

        //// evtable TableControlTopRowChanging GridTableControlRowColIndexChanging

        /// <summary>
        /// Occurs when the <see cref="GridControlBase.TopRowChanging"/> event of the underlying <see cref="GridTableControl"/> is raised.
        /// The original event arguments are provided with the <see cref="GridTableControlRowColIndexChangingEventArgs.Inner"/> property of the
        /// <see cref="GridTableControlRowColIndexChangingEventArgs"/> class.
        /// </summary>
        [Description("Occurs when the TopRowChanging event of the underlying GridTableControl is raised")]
        [Category("TableControl")]
        public event GridTableControlRowColIndexChangingEventHandler TableControlTopRowChanging;

        /// <summary>
        /// Raises the <see cref="TableControlTopRowChanging"/> event.
        /// </summary>
        // <param name="e">A <see cref="GridTableControlRowColIndexChangingEventArgs" /> that contains the event data.</param>
        protected virtual void OnTableControlTopRowChanging(GridTableControlRowColIndexChangingEventArgs e)
        {
            if (TableControlTopRowChanging != null)
            {
                TableControlTopRowChanging(this, e);
            }
        }

        // evtable TableControlCellCancelMode GridTableControlCellMouse

        /// <summary>
        /// Occurs when the <see cref="GridControlBase.CellCancelMode"/> event of the underlying <see cref="GridTableControl"/> is raised.
        /// The original event arguments are provided with the <see cref="GridTableControlCellMouseEventArgs.Inner"/> property of the
        /// <see cref="GridTableControlCellMouseEventArgs"/> class.
        /// </summary>
        [Description("Occurs when the CellCancelMode event of the underlying GridTableControl is raised")]
        [Category("TableControl")]
        public event GridTableControlCellMouseEventHandler TableControlCellCancelMode;

        /// <summary>
        /// Raises the <see cref="TableControlCellCancelMode"/> event.
        /// </summary>
        // <param name="e">A <see cref="GridTableControlCellMouseEventArgs" /> that contains the event data.</param>
        protected virtual void OnTableControlCellCancelMode(GridTableControlCellMouseEventArgs e)
        {
            if (TableControlCellCancelMode != null)
            {
                TableControlCellCancelMode(this, e);
            }
        }

        //// evtable TableControlHScrollPixelPosChanging GridTableControlScrollPositionChanging

        /// <summary>
        /// Occurs when the <see cref="GridControlBase.HScrollPixelPosChanging"/> event of the underlying <see cref="GridTableControl"/> is raised.
        /// The original event arguments are provided with the <see cref="GridTableControlScrollPositionChangingEventArgs.Inner"/> property of the
        /// <see cref="GridTableControlScrollPositionChangingEventArgs"/> class.
        /// </summary>
        [Description("Occurs when the HScrollPixelPosChanging event of the underlying GridTableControl is raised")]
        [Category("TableControl")]
        public event GridTableControlScrollPositionChangingEventHandler TableControlHScrollPixelPosChanging;

        /// <summary>
        /// Raises the <see cref="TableControlHScrollPixelPosChanging"/> event.
        /// </summary>
        // <param name="e">A <see cref="GridTableControlScrollPositionChangingEventArgs" /> that contains the event data.</param>
        protected virtual void OnTableControlHScrollPixelPosChanging(GridTableControlScrollPositionChangingEventArgs e)
        {
            if (TableControlHScrollPixelPosChanging != null)
            {
                TableControlHScrollPixelPosChanging(this, e);
            }
        }

        //// evtable TableControlCellMouseDown GridTableControlCellMouse

        /// <summary>
        /// Occurs when the <see cref="GridControlBase.CellMouseDown"/> event of the underlying <see cref="GridTableControl"/> is raised.
        /// The original event arguments are provided with the <see cref="GridTableControlCellMouseEventArgs.Inner"/> property of the
        /// <see cref="GridTableControlCellMouseEventArgs"/> class.
        /// </summary>
        [Description("Occurs when the CellMouseDown event of the underlying GridTableControl is raised")]
        [Category("TableControl")]
        public event GridTableControlCellMouseEventHandler TableControlCellMouseDown;

        /// <summary>
        /// Raises the <see cref="TableControlCellMouseDown"/> event.
        /// </summary>
        // <param name="e">A <see cref="GridTableControlCellMouseEventArgs" /> that contains the event data.</param>
        protected virtual void OnTableControlCellMouseDown(GridTableControlCellMouseEventArgs e)
        {
            ////TraceUtil.TraceCurrentMethodInfo(e);

            if (TableControlCellMouseDown != null)
            {
                TableControlCellMouseDown(this, e);
            }
        }

        //// evtable TableControlCurrentCellDeactivated GridTableControlCurrentCellDeactivated

        /// <summary>
        /// Occurs when the <see cref="GridControlBase.CurrentCellDeactivated"/> event of the underlying <see cref="GridTableControl"/> is raised.
        /// The original event arguments are provided with the <see cref="GridTableControlCurrentCellDeactivatedEventArgs.Inner"/> property of the
        /// <see cref="GridTableControlCurrentCellDeactivatedEventArgs"/> class.
        /// </summary>
        [Description("Occurs when the CurrentCellDeactivated event of the underlying GridTableControl is raised")]
        [Category("TableControl")]
        public event GridTableControlCurrentCellDeactivatedEventHandler TableControlCurrentCellDeactivated;

        /// <summary>
        /// Raises the <see cref="TableControlCurrentCellDeactivated"/> event.
        /// </summary>
        // <param name="e">A <see cref="GridTableControlCurrentCellDeactivatedEventArgs" /> that contains the event data.</param>
        protected virtual void OnTableControlCurrentCellDeactivated(GridTableControlCurrentCellDeactivatedEventArgs e)
        {
            ////TraceUtil.TraceCurrentMethodInfo(e);

            if (TableControlCurrentCellDeactivated != null)
            {
                TableControlCurrentCellDeactivated(this, e);
            }
        }

        //// evtable TableControlHScrollPixelPosChanged GridTableControlScrollPositionChanged

        /// <summary>
        /// Occurs when the <see cref="GridControlBase.HScrollPixelPosChanged"/> event of the underlying <see cref="GridTableControl"/> is raised.
        /// The original event arguments are provided with the <see cref="GridTableControlScrollPositionChangedEventArgs.Inner"/> property of the
        /// <see cref="GridTableControlScrollPositionChangedEventArgs"/> class.
        /// </summary>
        [Description("Occurs when the HScrollPixelPosChanged event of the underlying GridTableControl is raised")]
        [Category("TableControl")]
        public event GridTableControlScrollPositionChangedEventHandler TableControlHScrollPixelPosChanged;

        /// <summary>
        /// Raises the <see cref="TableControlHScrollPixelPosChanged"/> event.
        /// </summary>
        // <param name="e">A <see cref="GridTableControlScrollPositionChangedEventArgs" /> that contains the event data.</param>
        protected virtual void OnTableControlHScrollPixelPosChanged(GridTableControlScrollPositionChangedEventArgs e)
        {
            if (TableControlHScrollPixelPosChanged != null)
            {
                TableControlHScrollPixelPosChanged(this, e);
            }
        }

        //// evtable TableControlCellMouseHoverLeave GridTableControlCellMouse

        /// <summary>
        /// Occurs when the <see cref="GridControlBase.CellMouseHoverLeave"/> event of the underlying <see cref="GridTableControl"/> is raised.
        /// The original event arguments are provided with the <see cref="GridTableControlCellMouseEventArgs.Inner"/> property of the
        /// <see cref="GridTableControlCellMouseEventArgs"/> class.
        /// </summary>
        [Description("Occurs when the CellMouseHoverLeave event of the underlying GridTableControl is raised")]
        [Category("TableControl")]
        public event GridTableControlCellMouseEventHandler TableControlCellMouseHoverLeave;

        /// <summary>
        /// Raises the <see cref="TableControlCellMouseHoverLeave"/> event.
        /// </summary>
        // <param name="e">A <see cref="GridTableControlCellMouseEventArgs" /> that contains the event data.</param>
        protected virtual void OnTableControlCellMouseHoverLeave(GridTableControlCellMouseEventArgs e)
        {
            if (TableControlCellMouseHoverLeave != null)
            {
                TableControlCellMouseHoverLeave(this, e);
            }
        }

        //// evtable TableControlCurrentCellMoveFailed GridTableControlCurrentCellMoveFailed

        /// <summary>
        /// Occurs when the <see cref="GridControlBase.CurrentCellMoveFailed"/> event of the underlying <see cref="GridTableControl"/> is raised.
        /// The original event arguments are provided with the <see cref="GridTableControlCurrentCellMoveFailedEventArgs.Inner"/> property of the
        /// <see cref="GridTableControlCurrentCellMoveFailedEventArgs"/> class.
        /// </summary>
        [Description("Occurs when the CurrentCellMoveFailed event of the underlying GridTableControl is raised")]
        [Category("TableControl")]
        public event GridTableControlCurrentCellMoveFailedEventHandler TableControlCurrentCellMoveFailed;

        /// <summary>
        /// Raises the <see cref="TableControlCurrentCellMoveFailed"/> event.
        /// </summary>
        // <param name="e">A <see cref="GridTableControlCurrentCellMoveFailedEventArgs" /> that contains the event data.</param>
        protected virtual void OnTableControlCurrentCellMoveFailed(GridTableControlCurrentCellMoveFailedEventArgs e)
        {
            if (TableControlCurrentCellMoveFailed != null)
            {
                TableControlCurrentCellMoveFailed(this, e);
            }
        }

        // evtable TableControlDrawCellFrameAppearance GridTableControlDrawCellBackground

        /// <summary>
        /// Occurs when the <see cref="GridControlBase.DrawCellFrameAppearance"/> event of the underlying <see cref="GridTableControl"/> is raised.
        /// The original event arguments are provided with the <see cref="GridTableControlDrawCellBackgroundEventArgs.Inner"/> property of the
        /// <see cref="GridTableControlDrawCellBackgroundEventArgs"/> class.
        /// </summary>
        [Description("Occurs when the DrawCellFrameAppearance event of the underlying GridTableControl is raised")]
        [Category("TableControl")]
        public event GridTableControlDrawCellBackgroundEventHandler TableControlDrawCellFrameAppearance;

        /// <summary>
        /// Raises the <see cref="TableControlDrawCellFrameAppearance"/> event.
        /// </summary>
        // <param name="e">A <see cref="GridTableControlDrawCellBackgroundEventArgs" /> that contains the event data.</param>
        protected virtual void OnTableControlDrawCellFrameAppearance(GridTableControlDrawCellBackgroundEventArgs e)
        {
            if (TableControlDrawCellFrameAppearance != null)
            {
                TableControlDrawCellFrameAppearance(this, e);
            }
        }

        //// evtable TableControlMouseWheel GridTableControlDrawCellBackground

        /// <summary>
        /// Occurs when the <see cref="Control.MouseWheel"/> event of the underlying <see cref="GridTableControl"/> is raised.
        /// The original event arguments are provided with the <see cref="GridTableControlDrawCellBackgroundEventArgs.Inner"/> property of the
        /// <see cref="GridTableControlDrawCellBackgroundEventArgs"/> class.
        /// </summary>
        [Description("Occurs when the MouseWheel event of the underlying GridTableControl is raised")]
        [Category("TableControl")]
        public event GridTableControlMouseEventHandler TableControlMouseWheel;

        /// <summary>
        /// Raises the <see cref="TableControlMouseWheel"/> event.
        /// </summary>
        // <param name="e">A <see cref="GridTableControlDrawCellBackgroundEventArgs" /> that contains the event data.</param>
        protected virtual void OnTableControlMouseWheel(GridTableControlMouseEventArgs e)
        {
            if (TableControlMouseWheel != null)
            {
                TableControlMouseWheel(this, e);
            }
        }

        private void recordNavigationControl1_PaneCreated(object sender, SplitterPaneEventArgs e)
        {
            GridTableControl tableControl = (GridTableControl)e.Control;
            tableControl.GridControlBaseEventsTarget = new GridControlBaseEventsTarget(tableControl, this);
        }

        //// tevent ExceptionRaised ExceptionRaisedEventArgs

        /// <summary>
        /// Occurs when a unknown exception has been cached while modifying underlying data in the datasource.
        /// </summary>
        /// <remarks>
        /// If necessary, you can rethrow the exception in your event handler.
        /// </remarks>
        [Category("Table")]
        [Description("Occurs when an unknown exception has been cached while modifying underlying data in the datasource.")]
        public event ExceptionRaisedEventHandler ExceptionRaised;

        /// <summary>
        /// Raises the <see cref="ExceptionRaised"/> event.
        /// </summary>
        /// <param name="e">An <see cref="ExceptionRaisedEventArgs" /> that contains the event data.</param>
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

            if (!e.Cancel)
            {
                this.TableControl.Invalidate();

                string msg = e.Method + ": " + e.Exception.Message;

                MessageBoxAdv.Show(msg);

                TableControl.synchronizeGridShouldRestoreCurrentCell = true;
            }
        }

        //// tevent GroupCollapsing GroupEventArgs

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

        //// tevent GroupCollapsed GroupEventArgs

        /// <summary>
        /// Occurs before a group is collapsed.
        /// </summary>
        [Description("Occurs before a group is collapsed.")]
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

            if (gridHelper != null)
            {
                gridHelper.OnGridGroupCollapsed(this, e);
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

        //// tevent GroupExpanded GroupEventArgs

        /// <summary>
        /// Occurs after a group was expanded.
        /// </summary>
        [Description("Occurs after a group was expanded.")]
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

            if (gridHelper != null)
            {
                gridHelper.OnGridGroupExpanded(this, e);
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

        //// tevent RecordCollapsing RecordEventArgs

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

        //// tevent RecordCollapsed RecordEventArgs

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

            if (gridHelper != null)
            {
                gridHelper.OnGridRecordCollapsed(this, e);
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

        //// tevent RecordExpanding RecordEventArgs

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

        //// tevent RecordExpanded RecordEventArgs

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

            if (gridHelper != null)
            {
                gridHelper.OnGridRecordExpanded(this, e);
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

        //// tevent RecordDeleting RecordEventArgs

        /// <summary>
        /// Occurs before a record is deleted.
        /// </summary>
        /// <remarks>
        /// This event is raised only when the <see cref="Table"/> or <see cref="Record"/> triggers the deletion. If
        /// the underlying source list deletes the record, a <see cref="Syncfusion.Grouping.Table.SourceListListChanged"/> event is raised instead.
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

        //// tevent RecordDeleted RecordEventArgs

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

        //// tevent CurrentRecordContextChange CurrentRecordContextChangeEventArgs

        /// <summary>
        /// Occurs before and after the status of the current record is changed. Check the <see cref="CurrentRecordContextChangeEventArgs.Action"/>
        /// of the <see cref="CurrentRecordContextChangeEventArgs"/> to get information on which current record state was changed.
        /// </summary>
        [Description("Occurs before and after the status of the current record is changed.")]
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

        //// tevent CurrentRecordManagerReset tableEventsTarget

        /// <summary>
        /// Occurs when the <see cref="CurrentRecordManager.Reset"/> method of the <see cref="CurrentRecordManager"/> was called.
        /// </summary>
        /// <remarks>
        /// The GridGroupingControl listens to this events and resets any "Current Cell" state when this
        /// event is raised.
        /// </remarks>
        [Description("Occurs when the CurrentRecordManager.Reset method of the CurrentRecordManager is called.")]
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

        //// tevent GroupSummaryInvalidated GroupEventArgs

        /// <summary>
        /// Occurs when a summary has been marked dirty.
        /// </summary>
        /// <remarks>
        /// The GridGroupingControl listens to this event and will force a repaint of the specified summary if it is visible
        /// when this event was raised.
        /// </remarks>
        [Description("Occurs when a summary has been marked dirty.")]
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

            if (!e.Cancel)
            {
                if (gridHelper != null)
                {
                    gridHelper.OnGridGroupSummaryInvalidated(this, e);
                }
            }
        }

        //// tevent SourceListListChanged TableListChangedEventArgs

        /// <summary>
        /// Occurs before the <see cref="Table"/> processes the <see cref="IBindingList.ListChanged"/> event
        /// of an attached source list. More detailed <see cref="SourceListRecordChanged"/> events will be
        /// raised after this event.
        /// </summary>
        /// <remarks>
        /// The reason for firing this event is to give a programmer the chance to react to an <see cref="IBindingList.ListChanged"/>
        /// event before the engine since there is otherwise no order guaranteed when an IBindingList raises a ListChanged
        /// event.
        /// </remarks>
        [Description("Occurs before the table processes the IBindingList.ListChanged event.")]
        [Category("Table")]
        public event TableListChangedEventHandler SourceListListChanged;
        int saveBlinkTime = 0;
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

        //// tevent SourceListListChangedCompleted TableListChangedEventArgs

        /// <summary>
        /// Occurs after the <see cref="Table"/> processes the <see cref="IBindingList.ListChanged"/> event
        /// of an attached source list.
        /// </summary>
        /// <remarks>
        /// The reason for firing this event is to give a programmer the chance to react to an <see cref="IBindingList.ListChanged"/>
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
                if (saveBlinkTime > 0)
                {
                    this.BlinkTime = saveBlinkTime;
                    saveBlinkTime = 0;
                }
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

        //// tevent SourceListRecordChanged RecordChangedEventArgs

        /// <summary>
        /// Occurs when a record in the underlying data source is added, removed, or changed and after
        /// the <see cref="Table"/> was updated with that change.
        /// </summary>
        [Description("Occurs when a record in the underlying datasource is added, removed, or changed and the table was updated.")]
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

            if (!e.Cancel)
            {
                if (gridHelper != null)
                {
                    gridHelper.OnGridSourceListRecordChanged(this, e);
                }
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

        //// tevent SourceListRecordChanging RecordChangedEventArgs

        /// <summary>
        /// Occurs when a record in the underlying data source is added, removed or changed and before
        /// the <see cref="Table"/> is updated with that change.
        /// </summary>
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

            if (!e.Cancel)
            {
                if (gridHelper != null)
                {
                    gridHelper.OnGridSourceListRecordChanging(this, e);
                }
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

        bool useCustomUpdateOnListChanged = false;

        /// <summary>
        /// Specify this if you do not want TableControl to use built-in paint mechanism
        /// when ListChanged events are handled. (Makes TableControl.Table_SourceListRecordChanging 
        /// and TableControl.Table_SourceListRecordChanged return immediately)
        /// </summary>
        [System.Xml.Serialization.XmlIgnore]
        [DesignerSerializationVisibilityAttribute(DesignerSerializationVisibility.Hidden)]
        [Browsable(false)]
        [EditorBrowsable(EditorBrowsableState.Never)]
        [DefaultValue(false)]
        [Category("Optimization")]
        public bool UseCustomUpdateOnListChanged
        {
            get
            {
                return useCustomUpdateOnListChanged;
            }

            set
            {
                useCustomUpdateOnListChanged = value;
            }
        }

        /// <summary>
        /// Lets you specify whether the grid should simply call Invalidate
        /// when a ListChanged event is handled or if it should determine the area
        /// that is affected by the change and call InvalidateRange. With version 4.4
        /// check also the <see cref="InsertRemoveBehavior"/> and <see cref="SortPositionChangedBehavior"/>
        /// properties.
        /// </summary>
        /// <remarks>
        /// On first sight, you might think it better to determine the area
        /// that is affected by a change and call InvalidateRange.
        /// But when calling InvalidateRange, the grid needs to know the
        /// exact position of the record in the table before it can mark that area dirty.
        /// In order to determine the record position (and y-position of the row in the display),
        /// counters need to be evaluated. This operation can cost more time than simply
        /// calling Invalidate in high-frequency update scenarios. <para/>
        /// Also, be aware that the group caption bar needs to be updated when a
        /// record changes. <para/>
        /// With version 4.4 check out the new InsertRemoveBehavior and SortPositionChangedBehavior
        /// properties and the UpdateDisplayFrequency properties that will speed up things a lot
        /// if InvalidateAllWhenListChanged = false. <para/>
        /// </remarks>
        [Category("Optimization")]
        [Description("Lets you specify whether the grid should simply call Invalidate when a ListChanged event is handled or only paint the affected area.")]
        public bool InvalidateAllWhenListChanged
        {
            get
            {
                return Engine.InvalidateAllWhenListChanged;
            }

            set
            {
                Engine.InvalidateAllWhenListChanged = value;
            }
        }

        /// <summary>
        /// Determines whether <see cref="InvalidateAllWhenListChanged"/> was modified.
        /// </summary>
        /// <returns>True if content is changed;False, otherwise.</returns>
        public bool ShouldSerializeInvalidateAllWhenListChanged()
        {
            return Engine.ShouldSerializeInvalidateAllWhenListChanged();
        }

        /// <summary>
        /// Discards any changes for the <see cref="InvalidateAllWhenListChanged"/> object.
        /// </summary>
        public void ResetInvalidateAllWhenListChanged()
        {
            Engine.ResetInvalidateAllWhenListChanged();
        }

        //// tevent GroupAdded GroupEventArgs

        /// <summary>
        /// Occurs when a new group is added in a table after the table was categorized and when a record is changed. The event does not
        /// occur during categorization of the table. See the <see cref="CategorizedRecords"/> elements to when categorization
        /// finished.
        /// </summary>
        [Description("Occurs when a new group is added in a table after the table was categorized and when a record is changed.")]
        [Category("Table")]
        public event GroupEventHandler GroupAdded;

        /// <summary>
        /// Raises the  <see cref="GroupAdded"/> event.
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

        //// tevent GroupRemoving GroupEventArgs

        /// <summary>
        /// Occurs when a group is removed from a table after the table was categorized and when a record is changed. The event does not
        /// occur during categorization of the table. See the <see cref="CategorizedRecords"/> elements to when categorization
        /// finished.
        /// </summary>
        [Description("Occurs when a group is removed from a table after the table was categorized and when a record is changed.")]
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

        //// tevent SortingItemsInGroup GroupEventArgs

        /// <summary>
        /// Occurs before the records for a group are sorted.
        /// </summary>
        /// <remarks>
        /// The engine has a built-in optimization for sorting columns that allows it to perform the sorting
        /// on an on-demand basis group-by-group. Suppose you have a table with 200 different countries and
        /// you change the sort order of the cities. It is not necessary to sort the whole table. Instead,
        /// the individual groups can be sorted when they are scrolled into view. SortingItemsInGroup and
        /// SortedItemsInGroup events are fired in such case when a specific group was sorted on demand.
        /// <para/>
        /// If the whole table was set to dirty (see <see cref="Syncfusion.Grouping.Table.TableDirty"/>), then the whole table
        /// is simply recategorized. In that case, only a CategorizedElements event is raised but no
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

        //// tevent SortedItemsInGroup GroupEventArgs

        /// <summary>
        /// Occurs after the records for a group are sorted.
        /// </summary>
        /// <remarks>
        /// The engine has a built-in optimization for sorting columns that allows it to perform the sorting
        /// on an on-demand basis group-by-group. Suppose you have a table with 200 different countries and
        /// you change the sort order of the cities. It is not necessary to sort the whole table. Instead,
        /// the individual groups can be sorted when they are scrolled into view. SortingItemsInGroup and
        /// SortedItemsInGroup events are fired in such case when a specific group was sorted on demand.
        /// <para/>
        /// If the whole table was set to dirty (see <see cref="Syncfusion.Grouping.Table.TableDirty"/>), then the whole table
        /// is simply recategorized. In that case, only a CategorizedElements event is raised but no
        /// SortingItemsInGroup event.
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

        //// tevent InvalidatingCounters tableEventsTarget

        /// <summary>
        /// Occurs when the <see cref="Syncfusion.Grouping.Table.InvalidateCounterTopDown"/> of a <see cref="Table"/> was called
        /// and before all counters are marked dirty.
        /// </summary>
        [Category("Table")]
        [Description("Occurs when the Table.InvalidateCounterTopDown method of a table was called.")]
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
        [Description("Occurs when the Table.InvalidateSummariesTopDown of a Table is called.")]
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

        //// tevent CategorizingRecords tableEventsTarget

        /// <summary>
        /// Occurs before records are categorized after a table is marked dirty (<see cref="Syncfusion.Grouping.Table.TableDirty"/>).
        /// </summary>
        /// <remarks>
        /// When <see cref="Syncfusion.Grouping.Table.TableDirty"/> is set to True, e.g. because schema information for a table was changed
        /// or because the grouped columns were changed, the table will categorize records on demand the first time
        /// information about a record is accessed. At that time, the <see cref="Syncfusion.Grouping.Element"/> of the <see cref="Table"/> will start
        /// categorization.
        /// </remarks>
        [Category("Table")]
        [Description("Occurs before records are categorized after a table is marked dirty.")]
        public event TableEventHandler CategorizingRecords;

        /// <summary>
        /// Raises the  <see cref="CategorizingRecords"/> event.
        /// </summary>
        /// <param name="e">A <see cref="TableEventArgs" /> that contains the event data.</param>
        protected virtual void OnCategorizingRecords(TableEventArgs e)
        {
            if (CategorizingRecords != null)
            {
                CategorizingRecords(this, e);
            }

            if (gridHelper != null)
            {
                gridHelper.OnGridCategorizingRecords();
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

        //// tevent CategorizedRecords tableEventsTarget

        /// <summary>
        /// Occurs after records are categorized after a table is marked dirty (<see cref="Syncfusion.Grouping.Table.TableDirty"/>)
        /// </summary>
        /// <remarks>
        /// When <see cref="Syncfusion.Grouping.Table.TableDirty"/> is set to True, e.g. because schema information for a table was changed
        /// or because the grouped columns were changed, the table will categorize records on demand the first time
        /// information about a record is accessed. At that time, the <see cref="Element"/> of the <see cref="Table"/>
        /// will start
        /// categorization.
        /// </remarks>
        [Category("Table")]
        [Description("Occurs after records are categorized after a table was marked dirty.")]
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

        //// tevent TableSourceListChanged Table

        /// <summary>
        /// Occurs after the datasource was replaced.
        /// </summary>
        [Category("Table")]
        [Description("Occurs after the datasource was replaced.")]
        public event TableEventHandler TableSourceListChanged;

        /// <summary>
        /// Raises the <see cref="TableSourceListChanged"/> event.
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

        //// tevent RecordValueChanging RecordValueChanging

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

        //// tevent RecordValueChanged RecordValueChanged

        /// <summary>
        /// Occurs when a RecordFieldCell cell's value is changed and after Record.SetValue returned.
        /// </summary>
        [Description("Occurs when a RecordFieldCell cell's value is changed and after Record.SetValue returned"), Category("Table")]
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

        //// tevent DisplayElementChanging DisplayElementChanging

        /// <summary>
        /// When number of visible elements were changed.
        /// </summary>
        [Description("Occurs when number of visible elements were about to change"), Category("Table")]
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

        //// tevent DisplayElementChanged DisplayElementChanged

        /// <summary>
        /// When number of visible elements were changed.
        /// </summary>
        [Description("Occurs when number of visible elements were changed"), Category("Table")]
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

        //// tevent SelectedRecordsChanging SelectedRecordsChanging

        /// <summary>
        /// Occurs before the <see cref="Syncfusion.Grouping.Table.SelectedRecords"/> collection is modified.
        /// </summary>
        [Description("Occurs before the SelectedRecords collection is modified"), Category("Table")]
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

        //// tevent SelectedRecordsChanged SelectedRecordsChanged

        /// <summary>
        /// Occurs after the <see cref="Syncfusion.Grouping.Table.SelectedRecords"/> collection was modified.
        /// </summary>
        [Description("Occurs after the SelectedRecords collection is modified"), Category("Table")]
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

        ITableEventsTarget tableEventsTarget;

        /// <summary>
        /// Gets / sets an object that handles events raised by the <see cref="Table"/> object.
        /// </summary>
        [System.Xml.Serialization.XmlIgnore]
        [DesignerSerializationVisibilityAttribute(DesignerSerializationVisibility.Hidden)]
        [Browsable(false)]
        [EditorBrowsable(EditorBrowsableState.Never)]
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

        /// <overload>
        /// Suspends the painting of the control until the <see cref="EndUpdate"/> method is called.
        /// </overload>
        /// <summary>
        /// Suspends the painting of the control until the <see cref="EndUpdate"/> method is called.
        /// </summary>
        /// <remarks>
        /// <para>When many paints are made to the appearance of a control, you should invoke the
        /// BeginUpdate method to temporarily freeze the drawing of the control. This results
        /// in less distraction to the user, and a performance gain. After all updates have
        /// been made, invoke the EndUpdate method to resume drawing of the control.</para>
        /// <para/>
        /// The BeginUpdate method will call both the <see cref="BeginUpdate"/> method
        /// for the <see cref="TableControl"/> and <see cref="GridGroupDropArea"/>. This
        /// suspends drawing for both child controls.
        /// </remarks>
        /// <seealso cref="BeginUpdate"/>
        /// <seealso cref="EndUpdate"/>
        public void BeginUpdate()
        {
            SuspendLayout();
            GridGroupDropArea.BeginUpdate();
            TableControl.BeginUpdate();
        }

        /// <overload>
        /// Resumes the painting of the control suspended by calling the BeginUpdate method.
        /// </overload>
        /// <summary>
        /// Resumes the painting of the control suspended by calling the BeginUpdate method.
        /// </summary>
        /// <param name="update">True if pending paint operations should be executed immediately; False if they should be discarded.</param>
        /// <remarks>
        /// When many paints are made to the appearance of a control, you should invoke the
        /// BeginUpdate method to temporarily freeze the drawing of the control. This results
        /// in less distraction to the user, and a performance gain. After all updates have
        /// been made, invoke the EndUpdate method to resume drawing of the control.
        /// <para/>
        /// The EndUpdate method will call both the <see cref="EndUpdate "/> method
        /// for the <see cref="TableControl"/> and <see cref="GridGroupDropArea"/>. This
        /// resumes drawing for both child controls.
        /// </remarks>
        /// <seealso cref="BeginUpdate"/>
        public void EndUpdate(bool update)
        {
            GridGroupDropArea.EndUpdate(update);
            TableControl.EndUpdate(update);
            ResumeLayout();
        }

        #region RecordNavigationBar

        bool inUpdateNavigationBar;
        RecordNavigationBar recordNavigationBar;
        bool inNavigationBar_CurrentRecordChanging = false;
        bool inNavigationBar_CurrentRecordContextChange = false;

        void WireRecordNavigationBar()
        {
            if (recordNavigationBar != null)
            {
                recordNavigationBar.CurrentRecordChanging += new CurrentRecordChangedEventHandler(NavigationBar_CurrentRecordChanging);
                recordNavigationBar.CurrentRecordChanged += new CurrentRecordChangedEventHandler(NavigationBar_CurrentRecordChanged);
                recordNavigationBar.AllowStepIncrease = false;
            }
        }

        void UnwireRecordNavigationBar()
        {
            if (recordNavigationBar != null)
            {
                recordNavigationBar.CurrentRecordChanging -= new CurrentRecordChangedEventHandler(NavigationBar_CurrentRecordChanging);
                recordNavigationBar.CurrentRecordChanged -= new CurrentRecordChangedEventHandler(NavigationBar_CurrentRecordChanged);
            }
        }

        /// <summary>
        /// Gets / sets the navigation bar.
        /// </summary>
        [System.Xml.Serialization.XmlIgnore]
        [DesignerSerializationVisibilityAttribute(DesignerSerializationVisibility.Hidden)]
        [Browsable(false)]
        public RecordNavigationBar RecordNavigationBar
        {
            get
            {
                if (recordNavigationBar == null)
                {
                    RecordNavigationBar = this.recordNavigationControl1.NavigationBar;
                }

                return recordNavigationBar;
            }

            set
            {
                if (recordNavigationBar != value)
                {
                    UnwireRecordNavigationBar();
                    recordNavigationBar = value;
                    WireRecordNavigationBar();
                }
            }
        }

        /// <summary>
        /// Updates the record navigation bar properties such as maximum count, current record, etc.
        /// </summary>
        public void UpdateNavigationBar()
        {
            if (/*!HasTable
    || */
                     inNavigationBar_CurrentRecordChanging
                || inNavigationBar_CurrentRecordContextChange
                || inUpdateNavigationBar
                || this.recordNavigationBar == null)
            {
                return;
            }

            try
            {
                //// Note: NavigationBar has one-based record index. GridTable has zero-based record index.
                inUpdateNavigationBar = true;
                navigationBarWidth = this.RecordNavigationControl.NavigationBarWidth;
                string maxLabel = SR.GetString(SR.RecordNavigatorOF) + " " + Table.FilteredRecords.Count.ToString();
                bool allowAddNew = Table.TableDescriptor.AllowNew && Table.SourceListAllowNew;
                Record r = Table.CurrentRecord;
                if (Table.CurrentElement is GridNestedTable)
                {
                    r = Table.CurrentElement.ParentRecord;
                }

                int current = Table.FilteredRecords.IndexOf(r) + 1;
                int max = Table.FilteredRecords.Count;

                recordNavigationBar.SetValues(1, max, maxLabel, allowAddNew, current);

                ////Dynamically adjust the NavigatorWidth
                int maxLabelButtonPosition = recordNavigationBar.ButtonBarChild.Buttons[4].Bounds.X;

                recordNavigationBar.PerformLayout();

                int textAreaHeight = recordNavigationBar.TextBox.TextLength + recordNavigationBar.MaxLabel.Length + recordNavigationBar.Label.Length;
                if (this.RightToLeft == RightToLeft.Yes)
                {
                    this.RecordNavigationControl.NavigationBarWidth += maxLabelButtonPosition - recordNavigationBar.ButtonBarChild.Buttons[4].Bounds.X;
                }
                else
                {
                    if (textAreaHeight > recordNavigationBar.Height + 1)
                        this.RecordNavigationControl.NavigationBarWidth += recordNavigationBar.ButtonBarChild.Buttons[4].Bounds.X - maxLabelButtonPosition;
                    else
                        this.RecordNavigationControl.NavigationBarWidth = navigationBarWidth;
                }
            }
            finally
            {
                inUpdateNavigationBar = false;
            }
        }

        private void NavigationBar_CurrentRecordChanging(object sender, CurrentRecordEventArgs e)
        {
            if (inUpdateNavigationBar || inNavigationBar_CurrentRecordChanging)
            {
                return;
            }

            inNavigationBar_CurrentRecordChanging = true;

            try
            {
                OnNavigationBarRecordChanging(e);
                //// || !HasTable)
                if (e.Cancel)
                {
                    return;
                }

                //// Note: NavigationBar has one-based record index. GridTable has zero-based record index.
                if (e.Record >= 1 && e.Record <= Table.FilteredRecords.Count)
                {
                    Record r = Table.FilteredRecords[e.Record - 1];
                    ////        if (r.IsEditing)
                    ////        {
                    ////            if (!r.EndEdit())
                    ////            {
                    ////                e.Cancel = true;
                    ////                return;
                    ////            }
                    ////        }
                    ////Table.ShowRecord(r, true);
                    ////Table.CurrentRecordManager.NavigateTo(r);

                    if (Table.TableOptions.AllowSelection == GridSelectionFlags.None
                        && Table.TableOptions.ListBoxSelectionMode != SelectionMode.None)
                    {
                        TableControl.BeginUpdate(BeginUpdateOptions.InvalidateAndScroll);
                        TableControl.SelectRecords.Clear();
                        Table.ShowRecord(r, true);
                        Table.CurrentRecordManager.LeaveRecord(false);
                        if (!Table.CurrentRecordManager.HasCurrentElement)
                        {
                            Table.SelectedRecords.Add(r);
                            Table.CurrentRecordManager.NavigateTo(r);
                        }

                        TableControl.EndUpdate(true);
                    }
                    else
                    {
                        Table.ShowRecord(r, true);
                        Table.CurrentRecordManager.NavigateTo(r);
                    }
                    TableControl.ScrollCellInView(GridRangeInfo.Row(r.GetRowIndex()));
                }
                else if (e.Record == Table.FilteredRecords.Count + 1)
                {
                    Record r = Table.CurrentRecord;
                    Record addNewRecord = null;
                    if (r != null)
                    {
                        addNewRecord = r.ParentGroup.FindAddNewRecord();
                    }

                    if (addNewRecord == null)
                    {
                        addNewRecord = Table.AddNewRecord;
                    }

                    Table.ShowRecord(addNewRecord, true);
                    Table.CurrentRecordManager.NavigateTo(addNewRecord);
                }
            }
            finally
            {
                inNavigationBar_CurrentRecordChanging = false;
            }
        }

        /// <summary>
        /// Occurs after the current record of the navigation bar is changed.
        /// </summary>
        [Description("Occurs after the current record of the navigation bar is changed."), Category("Table")]
        public event CurrentRecordChangedEventHandler NavigationBarRecordChanged;

        /// <summary>
        /// Raises the <see cref="NavigationBarRecordChanged"/> event.
        /// </summary>
        /// <param name="e">A <see cref="CurrentRecordEventArgs" /> that contains the event data.</param>
        protected virtual void OnNavigationBarRecordChanged(CurrentRecordEventArgs e)
        {
            if (NavigationBarRecordChanged != null)
            {
                NavigationBarRecordChanged(this, e);
            }
        }

        /// <summary>
        /// Occurs before the current record of the navigation bar is changed.
        /// </summary>
        [Description("Occurs before the current record of the navigation bar is changed."), Category("Table")]
        public event CurrentRecordChangedEventHandler NavigationBarRecordChanging;

        /// <summary>
        /// Raises the <see cref="NavigationBarRecordChanging"/> event.
        /// </summary>
        /// <param name="e">A <see cref="CurrentRecordEventArgs" /> that contains the event data.</param>
        protected virtual void OnNavigationBarRecordChanging(CurrentRecordEventArgs e)
        {
            if (NavigationBarRecordChanging != null)
            {
                NavigationBarRecordChanging(this, e);
            }
        }

        private void NavigationBar_CurrentRecordChanged(object sender, CurrentRecordEventArgs e)
        {
            if (inUpdateNavigationBar || inNavigationBar_CurrentRecordContextChange)
            {
                return;
            }

            inNavigationBar_CurrentRecordContextChange = true;

            try
            {
                OnNavigationBarRecordChanged(e);
                //// || !HasTable)
                if (e.Cancel)
                {
                    return;
                }
            }
            finally
            {
                inNavigationBar_CurrentRecordContextChange = false;
            }
        }

        #endregion
        #region FieldValue events
        // event FieldValueEventHandler QueryValue

        /// <summary>
        /// Occurs when a value for a field descriptor and record is returned. See the Grid\Grouping\Samples\CustomSummary
        /// sample how to use this event with unbound field descriptors.
        /// </summary>
        [Description("Occurs when a value for a field descriptor and record is returned"), Category("Data")]
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
        /// Raises the <see cref="QueryValue"/> event
        /// </summary>
        /// <param name="e">The <see cref="FieldValueEventArgs"/> with event data.</param>
        public void RaiseQueryValue(FieldValueEventArgs e)
        {
            OnQueryValue(e);
        }

        //// event FieldValueEventHandler SaveValue

        /// <summary>
        /// Occurs when a value for a field descriptor and record is saved. See the Grid\Grouping\Samples\CustomSummary
        /// sample how to use this event with unbound field descriptors.
        /// </summary>
        [Description("Occurs when a value for a field descriptor and record is saved"), Category("Data")]
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

        GridDelayUpdateBehavior delayUpdateBehavior = GridDelayUpdateBehavior.PostCustomMessage | GridDelayUpdateBehavior.Timer;

        /// <summary>
        /// Specifies delayed update behavior when changes are made to the underlying engine from outside of WndProc of this
        /// control, for example when changes made through an external PropertyGrid attached to the engine.
        /// </summary>
        /// <remarks>
        /// The grid does not to call update after each and every operation. Instead it leaves the
        /// update to the end user or to user interaction. Normally, operations on the engine are
        /// triggered from within a WndProc call. In the grids WndProc routine the grid will call its base
        /// class version and then after the WndProc returns it will call synchronize any changes to the underlying
        /// engine with the display if the message was a mouse operation.
        /// <para/>
        /// This has the big advantage that users don't have to worry about calling BeginUpdate / EndUpdate in
        /// mouse handling code. You can just batch operations and then manually call Update(). The programmer
        /// does not have to worry about calling Update() since that will be done once the Mouse event returns
        /// and the GridTableControl.WndProc is executed. The idea is to have good performance for the most typical case.
        /// Updating the grid after every operation would be too expensive (e.g. if you loop through the records and set
        /// IsExpanded = true for each record).
        /// </remarks>
        [Browsable(false)]
        [DefaultValue(GridDelayUpdateBehavior.PostCustomMessageAndTimer)]
        [System.Xml.Serialization.XmlIgnore]
        public GridDelayUpdateBehavior DelayUpdateBehavior
        {
            get
            {
                return delayUpdateBehavior;
            }

            set
            {
                delayUpdateBehavior = value;
            }
        }

        /// <summary>
        /// Occurs each time the <see cref="GridStyleInfo.Text"/> is called to get the raw string that represents the underlying cell's value.
        /// </summary>
        /// <remarks>
        /// <para/>
        /// This event allows you to customize how to represent a cell's value as string at run-time on demand.
        /// <para/>
        /// If you do want to customize the grid's default conversion, you should assign the result string
        /// to <see cref="GridCellTextEventArgs.Text"/> and set <see cref="Syncfusion.ComponentModel.SyncfusionHandledEventArgs.Handled"/>
        /// to True. The grid will check this flag to see whether it should return <see cref="GridCellTextEventArgs.Text"/>
        /// or use a default conversion.
        /// <para/>
        /// If you need identity information about the cell such as row and column index, you can get that
        /// information by querying <see cref="GridStyleInfo.CellIdentity"/> of the <see cref="GridCellTextEventArgs.Style"/>
        /// object.
        /// </remarks>
        /// <seealso cref="GridCellTextEventHandler"/>
        /// <seealso cref="GridCellTextEventArgs"/>
        /// <seealso cref="GridModel.SaveCellText"/>
        /// <seealso cref="GridModel.QueryCellText"/>
        /// <seealso cref="GridCellModelBase.GetText"/>
        /// <seealso cref="GridStyleInfo.Text"/>
        [Description("Occurs each time the GridStyleInfo.Text is called to get the string that represents the underlying cell's value"),
        Category("Data")]
        public event GridCellTextEventHandler QueryCellText;

        /// <summary>
        /// Occurs each time the <see cref="GridStyleInfo.Text"/> is called to set the unformatted string that represents the underlying cell's value.
        /// </summary>
        /// <remarks>
        /// <para/>
        /// This event allows you to customize how to parse the unformatted text into a cell value at run-time on demand.
        /// <para/>
        /// If you do want to customize the grid's default parsing behavior, you should assign the resulting value
        /// to the <see cref="GridStyleInfo.CellValue"/> of the <see cref="GridStyleInfo"/> object
        /// and set <see cref="Syncfusion.ComponentModel.SyncfusionHandledEventArgs.Handled"/>
        /// to True. The grid will check this flag to see whether it should accept your modification
        /// or use a default parsing routine.
        /// <para/>
        /// If you need identity information about the cell such as row and column index, query the <see cref="GridStyleInfo.CellIdentity"/> of the <see cref="GridCellTextEventArgs.Style"/>
        /// object.
        /// <para/>
        ///    See the <see cref="SaveCellFormattedText"/> event for further discussion since these two events
        ///    are very similar. Often you will need to handle both events in your code in the same way.
        /// </remarks>
        /// <seealso cref="GridCellTextEventHandler"/>
        /// <seealso cref="GridCellTextEventArgs"/>
        /// <seealso cref="GridModel.SaveCellText"/>
        /// <seealso cref="GridModel.QueryCellText"/>
        /// <seealso cref="GridCellModelBase.ApplyText"/>
        /// <seealso cref="GridStyleInfo.Text"/>
        [Description("Occurs each time the GridStyleInfo.Text is called to set the raw string that represents the underlying cell's value."),
        Category("Data")]
        public event GridCellTextEventHandler SaveCellText;

        /// <summary>
        /// Occurs each time the <see cref="GridStyleInfo.FormattedText"/> is called to get the formatted string that represents the underlying cell's value
        /// considering <see cref="GridStyleInfo.Format"/>.
        /// </summary>
        /// <remarks>
        /// <para/>
        /// This event allows you to customize how to format a cell's value as string at run-time on demand based on <see cref="GridStyleInfo.Format"/>.
        /// <para/>
        /// If you do want to customize the grid's default formatting, you should assign the resulting string
        /// to <see cref="GridCellTextEventArgs.Text"/> and set <see cref="Syncfusion.ComponentModel.SyncfusionHandledEventArgs.Handled"/>
        /// to True. The grid will check this flag to see whether it should return <see cref="GridCellTextEventArgs.Text"/>
        /// or use a default formatting routine.
        /// <para/>
        /// If you need identity information about the cell such as row and column index, you can get that
        /// information by querying the <see cref="GridStyleInfo.CellIdentity"/> of the <see cref="GridCellTextEventArgs.Style"/>
        /// object.
        /// </remarks>
        /// <seealso cref="GridCellTextEventHandler"/>
        /// <seealso cref="GridCellTextEventArgs"/>
        /// <seealso cref="GridModel.SaveCellFormattedText"/>
        /// <seealso cref="GridModel.QueryCellFormattedText"/>
        /// <seealso cref="GridCellModelBase.ApplyFormattedText"/>
        /// <seealso cref="GridStyleInfo.FormattedText"/>
        [Description("Occurs each time the GridStyleInfo.Text is called to get the string that represents the underlying cell's value"),
        Category("Data")]
        public event GridCellTextEventHandler QueryCellFormattedText;

        /// <summary>
        /// Occurs each time the <see cref="GridStyleInfo.FormattedText"/> is called to parse the formatted string that represents the underlying cell's value
        /// considering <see cref="GridStyleInfo.Format"/> and <see cref="GridStyleInfo.CellValueType"/>.
        /// </summary>
        /// <remarks>
        /// <para/>
        /// This event allows you to customize how to parse the formatted text into a cell value at run-time on demand.
        /// <para/>
        /// If you do want to customize the grid's default parsing behavior, you should assign the resulting value
        /// to the <see cref="GridStyleInfo.CellValue"/> of the <see cref="GridStyleInfo"/> object
        /// and set <see cref="SyncfusionHandledEventArgs.Handled"/>
        /// to True. The grid will check this flag to see whether it should accept your modification
        /// or use a default parsing routine.
        /// <para/>
        /// If you need identity information about the cell such as row and column index, you can get that
        /// information by querying the <see cref="GridStyleInfo.CellIdentity"/> of the <see cref="GridCellTextEventArgs.Style"/>
        /// object.
        /// <para/>
        /// This event is normally called from within <see cref="GridCellModelBase.ApplyFormattedText"/>, which is called
        /// when the user enters text into a text box or when text is assigned to <see cref="GridStyleInfo.FormattedText"/>.
        /// ApplyFormattedText method checks if there are event handlers for <see cref="GridModel.SaveCellFormattedText"/> and
        /// if the <see cref="SyncfusionHandledEventArgs.Handled"/> is not set, they try to convert the input text into
        /// the type specified with <see cref="GridStyleInfo.CellValueType"/>.
        /// <para/>
        /// If this conversion fails, <see cref="GridCellModelBase.ApplyFormattedText"/> will check <see cref="GridStyleInfo.StrictValueType"/>. If it
        /// is True, an exception is thrown which itself results in a warning message displayed to the user at the
        /// time from <see cref="GridControlBase.CurrentCellValidating"/>.
        /// <para/>
        /// If you set <see cref="GridStyleInfo.StrictValueType"/> to False, <see cref="GridCellModelBase.ApplyFormattedText"/> will not throw
        /// an exception and simply store the text as <see cref="GridStyleInfo.CellValue"/>.
        /// <para/>
        /// If you need a more specialized customization of this behavior, you should handle the
        /// <see cref="GridModel.SaveCellFormattedText"/> event. This lets you parse the text input
        /// and change the cells <see cref="GridStyleInfo.CellValueType"/> at run-time. See the attached example.
        /// </remarks>
        /// <seealso cref="GridCellTextEventHandler"/>
        /// <seealso cref="GridCellTextEventArgs"/>
        /// <seealso cref="GridModel.SaveCellFormattedText"/>
        /// <seealso cref="GridModel.QueryCellFormattedText"/>
        /// <seealso cref="GridCellModelBase.ApplyFormattedText"/>
        /// <seealso cref="GridStyleInfo.FormattedText"/>
        /// <seealso cref="GridStyleInfo.CellValueType"/>
        /// <seealso cref="GridStyleInfo.StrictValueType"/>
        /// <example>
        /// This example parses the text input and changes the cell's CellValueType at run-time if the input does not match the current CellValueType.
        /// <code lang="C#">
        /// void InitializeComponent()
        /// {
        ///     // initialize code
        ///     // ...
        ///     this.gridControl1.SaveCellText += new Syncfusion.Windows.Forms.Grid.GridCellTextEventHandler(this.gridControl1_SaveCellText);
        ///     this.gridControl1.QueryCellFormattedText += new Syncfusion.Windows.Forms.Grid.GridCellTextEventHandler(this.gridControl1_QueryCellFormattedText);
        ///     this.gridControl1.SaveCellFormattedText += new Syncfusion.Windows.Forms.Grid.GridCellTextEventHandler(this.gridControl1_SaveCellFormattedText);
        ///     this.gridControl1.QueryCellText += new Syncfusion.Windows.Forms.Grid.GridCellTextEventHandler(this.gridControl1_QueryCellText);
        /// }
        /// <para/>
        /// private void gridControl1_QueryCellFormattedText(object sender, Syncfusion.Windows.Forms.Grid.GridCellTextEventArgs e)
        /// {
        /// <para/>
        /// }
        /// <para/>
        /// private void gridControl1_QueryCellText(object sender, Syncfusion.Windows.Forms.Grid.GridCellTextEventArgs e)
        /// {
        /// <para/>
        /// }
        /// <para/>
        /// private void gridControl1_SaveCellText(object sender, Syncfusion.Windows.Forms.Grid.GridCellTextEventArgs e)
        /// {
        ///     ParseText(e);
        /// }
        /// <para/>
        /// private void gridControl1_SaveCellFormattedText(object sender, GridCellTextEventArgs e)
        /// {
        ///     ParseText(e);
        /// }
        /// <para/>
        /// void ParseText(GridCellTextEventArgs e)
        /// {
        ///     // By default, the grid will display a warning message box informing the user
        ///     // the entered value is not valid and the user will have to change the value.
        ///     //
        ///     // In this event handler, we change the grid default's behavior such that
        ///     // when the user enters a value that does not fit the cell's CellValueType,
        ///     // the input text is accepted and no warning message is shown.
        ///     if (e.Handled)
        ///         return;
        /// <para/>
        ///     System.Globalization.CultureInfo ci = e.Style.CultureInfo;
        ///     System.Globalization.NumberFormatInfo nfi = ci != null ? ci.NumberFormat : null;
        ///     try
        ///     {
        ///         e.Style.CellValue = GridCellValueConvert.Parse(e.Text, e.Style.CellValueType, nfi, e.Style.Format);
        ///     }
        ///     catch (Exception ex)
        ///     {
        ///         if (ex is FormatException || ex.InnerException is FormatException)
        ///         {
        ///             e.Style.CellValue = e.Text;
        ///             // possibly could also change CellValueType here
        ///             e.Style.CellValueType = typeof(string);
        ///             // - or -
        ///             // you could also further analyze the input text and assign a type
        ///             // that fits the input text, e.g.
        ///             // e.Style.CellValueType = typeof(datetime);
        ///             // - or -
        ///             // e.Style.CellValueType = typeof(decimal);
        ///             // etc.
        ///         }
        ///         else
        ///             throw;
        ///     }
        ///     e.Handled = true;
        /// }
        /// </code>
        /// <code lang="VB">
        /// Private Sub InitializeComponent()
        ///     ' Initalize code
        ///     ' ...
        ///     AddHandler Me.gridControl1.SaveCellText, AddressOf Me.gridControl1_SaveCellText
        ///     AddHandler Me.gridControl1.QueryCellFormattedText, AddressOf Me.gridControl1_QueryCellFormattedText
        ///     AddHandler Me.gridControl1.SaveCellFormattedText, AddressOf Me.gridControl1_SaveCellFormattedText
        ///     AddHandler Me.gridControl1.QueryCellText, AddressOf Me.gridControl1_QueryCellText
        /// End Sub 'InitializeComponent
        /// <para/>
        /// Private Sub gridControl1_QueryCellFormattedText(sender As Object, e As Syncfusion.Windows.Forms.Grid.GridCellTextEventArgs)
        /// End Sub 'gridControl1_QueryCellFormattedText
        /// <para/>
        /// Private Sub gridControl1_QueryCellText(sender As Object, e As Syncfusion.Windows.Forms.Grid.GridCellTextEventArgs)
        /// End Sub 'gridControl1_QueryCellText
        /// <para/>
        /// Private Sub gridControl1_SaveCellText(sender As Object, e As Syncfusion.Windows.Forms.Grid.GridCellTextEventArgs)
        ///     ParseText(e)
        /// End Sub 'gridControl1_SaveCellText
        /// <para/>
        /// Private Sub gridControl1_SaveCellFormattedText(sender As Object, e As GridCellTextEventArgs)
        ///     ParseText(e)
        /// End Sub 'gridControl1_SaveCellFormattedText
        /// <para/>
        /// Sub ParseText(e As GridCellTextEventArgs)
        ///     ' By default, the grid will display a warning message box informing the user
        ///     ' the entered value is not valid and the user will have to change the value.
        ///     '
        ///     ' In this event handler we change the grid default's behavior such that
        ///     ' when the user enters a value that does not fit the cell's CellValueType,
        ///     ' the input text is accepted and no warning message is shown.
        ///     If e.Handled Then
        ///         Return
        ///     End If
        ///     Dim ci As System.Globalization.CultureInfo = e.Style.CultureInfo
        ///     Dim nfi As System.Globalization.NumberFormatInfo = Nothing
        ///     If (Not (ci Is Nothing)) Then nfi = ci.NumberFormat
        ///     Try
        ///         e.Style.CellValue = GridCellValueConvert.Parse(e.Text, e.Style.CellValueType, nfi, e.Style.Format)
        ///     Catch ex As Exception
        ///         If TypeOf ex Is FormatException OrElse TypeOf ex.InnerException Is FormatException Then
        ///             e.Style.CellValue = e.Text
        ///         ' possibly could also change CellValueType here
        ///         ' e.Style.CellValueType = typeof(string);
        ///         ' - or -
        ///         ' you could also further analyze the input text and assign a type
        ///         ' that fits the input text, e.g.
        ///         ' e.Style.CellValueType = typeof(datetime);
        ///         ' - or -
        ///         ' e.Style.CellValueType = typeof(decimal);
        ///         ' etc.
        ///         Else
        ///             Throw
        ///         End If
        ///     End Try
        ///     e.Handled = True
        /// End Sub 'ParseText
        /// </code>
        /// </example>
        [Description("Occurs each time the GridStyleInfo.FormattedText is called to set the raw string that represents the underlying cell's value."),
        Category("Data")]
        public event GridCellTextEventHandler SaveCellFormattedText;

        /// <summary>
        /// Use this event to provide support for parsing the formatted string and convert
        /// it into the the underlying cell's value
        /// considering <see cref="GridStyleInfo.Format"/> and <see cref="GridStyleInfo.CellValueType"/>.
        /// <para/>
        /// This event is raised from GridCellModelBase.ApplyFormattedText after 
        /// <see cref="SaveCellFormattedText"/> was raised. The event is raised only 
        /// if the SaveCellFormattedText did not set e.Handled. 
        /// </summary>
        /// <remarks>
        /// The grid has built-in support for parsing the Percent format (Format = "P") and Hexadecimal
        /// format (Format = "X"). You should handle this event if you want to add support
        /// for other formats. <para/>
        /// GridCellTextEventArgs has information about the style settings of the cell. You can
        /// inspect that style to get information about Format and CellValueType of the cell.
        /// </remarks>
        [Description("Handle this event to provide support for parsing the formatted string and convert it into the the underlying cell's value."),
        Category("Data")]
        public event GridCellTextEventHandler ParseCommonFormats;

        /// <summary>
        /// Raises the <see cref="QueryCellText"/> event.
        /// </summary>
        /// <param name="e">A <see cref="GridCellTextEventArgs" /> that contains the event data.</param>
        protected virtual void OnQueryCellText(GridCellTextEventArgs e)
        {
            ////            if (this.GroupingControl != null)
            ////    this.GroupingControl.RaiseQueryCellText(e);

            if (QueryCellText != null)
            {
                QueryCellText(this, e);
            }
        }

        /// <internalonly/>
        /// <summary>Used internally.</summary>
        [Syncfusion.Documentation.DocumentationExclude()]
        public void RaiseQueryCellText(GridCellTextEventArgs e)
        {
            OnQueryCellText(e);
        }

        /// <summary>
        /// Raises the <see cref="SaveCellText"/> event.
        /// </summary>
        /// <param name="e">A <see cref="GridCellTextEventArgs" /> that contains the event data.</param>
        protected virtual void OnSaveCellText(GridCellTextEventArgs e)
        {
            ////            if (this.GroupingControl != null)
            ////    this.GroupingControl.RaiseSaveCellText(e);

            if (SaveCellText != null)
            {
                SaveCellText(this, e);
            }
        }

        /// <internalonly/>
        /// <summary>Used internally.</summary>
        [Syncfusion.Documentation.DocumentationExclude()]
        public void RaiseSaveCellText(GridCellTextEventArgs e)
        {
            OnSaveCellText(e);
        }

        /// <summary>
        /// Raises the <see cref="QueryCellFormattedText"/> event.
        /// </summary>
        /// <param name="e">A <see cref="GridCellTextEventArgs" /> that contains the event data.</param>
        protected virtual void OnQueryCellFormattedText(GridCellTextEventArgs e)
        {
            ////            if (this.GroupingControl != null)
            ////    this.GroupingControl.RaiseQueryCellFormattedText(e);

            if (QueryCellFormattedText != null)
            {
                QueryCellFormattedText(this, e);
            }
        }

        /// <internalonly/>
        /// <summary>Used internally.</summary>
        [Syncfusion.Documentation.DocumentationExclude()]
        public void RaiseQueryCellFormattedText(GridCellTextEventArgs e)
        {
            OnQueryCellFormattedText(e);
        }

        /// <summary>
        /// Raises the <see cref="ParseCommonFormats"/> event. 
        /// </summary>
        /// <param name="e">A <see cref="GridCellTextEventArgs" /> that contains the event data.</param>
        protected virtual void OnParseCommonFormats(GridCellTextEventArgs e)
        {
            ////if (this.eventsTarget != null)
            ////    this.eventsTarget.OnParseCommonFormats(e);

            if (ParseCommonFormats != null)
            {
                ParseCommonFormats(this, e);
            }
        }

        /// <internalonly/>
        /// <summary>Used internally.</summary>
        [Syncfusion.Documentation.DocumentationExclude()]
        public void RaiseParseCommonFormats(GridCellTextEventArgs e)
        {
            OnParseCommonFormats(e);
        }

        /// <summary>
        /// Raises the <see cref="SaveCellFormattedText"/> event.
        /// </summary>
        /// <param name="e">A <see cref="GridCellTextEventArgs" /> that contains the event data.</param>
        protected virtual void OnSaveCellFormattedText(GridCellTextEventArgs e)
        {
            ////            if (this.GroupingControl != null)
            ////    this.GroupingControl.RaiseSaveCellFormattedText(e);

            if (SaveCellFormattedText != null)
            {
                SaveCellFormattedText(this, e);
            }
        }

        /// <internalonly/>
        /// <summary>Used internally.</summary>
        [Syncfusion.Documentation.DocumentationExclude()]
        public void RaiseSaveCellFormattedText(GridCellTextEventArgs e)
        {
            OnSaveCellFormattedText(e);
        }

        /// <summary>
        /// Occurs to determine if the cell belongs to a covered range and returns the covered range of the cell or
        /// the cell itself as <see cref="GridRangeInfo"/> if it is not a covered range.
        /// </summary>
        [Description("Occurs to determine if the cell belongs to a covered range and returns the covered range of the cell or the cell itself as GridRangeInfo if it is not a covered range"), Category("Appearance")]
        public event GridTableQueryCoveredRangeEventHandler QueryCoveredRange;

        /// <summary>
        /// Raises the <see cref="QueryCoveredRange"/> event.
        /// </summary>
        /// <param name="e">A <see cref="GridTableCellStyleInfoEventArgs" /> that contains the event data.</param>
        protected virtual void OnQueryCoveredRange(GridTableQueryCoveredRangeEventArgs e)
        {
           ////            if (this.GroupingControl != null)
            ////    this.GroupingControl.RaiseQueryCoveredRange(e);

            if (QueryCoveredRange != null)
            {
                QueryCoveredRange(this, e);
            }
        }

        /// <summary>
        /// Raises the <see cref="QueryCoveredRange"/> event.
        /// </summary>
        /// <param name="e">A <see cref="GridTableCellStyleInfoEventArgs" /> that contains the event data.</param>
        internal void RaiseQueryCoveredRange(GridTableQueryCoveredRangeEventArgs e)
        {
            OnQueryCoveredRange(e);
        }

        GridGroupingControlOptimizeListChanged gridHelper;

        /// <exclude/>
        /// <summary>Used internally.</summary>
        [System.Xml.Serialization.XmlIgnore]
        [DesignerSerializationVisibilityAttribute(DesignerSerializationVisibility.Hidden)]
        [Browsable(false)]
        public GridGroupingControlOptimizeListChanged GridHelper
        {
            get
            {
                return gridHelper;
            }
        }

        /// <exclude/>
        /// <summary>Used internally.</summary>
        public void AddHighlightedElement(Element displayElement)
        {
            if (gridHelper != null)
            {
                gridHelper.AddHighlightedElement(displayElement);
            }
        }

        /// <exclude/>
        /// <summary>Used internally.</summary>
        public void PaintUpdatedRecordFields()
        {
            if (gridHelper != null)
            {
                gridHelper.PaintUpdatedRecordFields();
            }
        }

        /// <summary>
        /// Specifies the number of milliseconds to wait between display updates when new ListChanged event handler logic
        /// is used. This property does have no any effect if UseOldListChangedHandler = true. Special values are
        /// 0 - only manually update display by calling grid.Update() and 1 - update display immediately after each
        /// change.
        /// </summary>
        [DefaultValue(100)]
        [Category("Optimization")]
        [Description("Specifies the number of milliseconds to wait between display updates when new ListChanged event handler is used. Special values:  0 - only manually update display, 1 - immediate update after each change.")]
        public int UpdateDisplayFrequency
        {
            get
            {
                return Engine.UpdateDisplayFrequency;
            }

            set
            {
                Engine.UpdateDisplayFrequency = value;
            }
        }

        /// <summary>
        /// Specifies how the grid should react if records are inserted or deleted.
        /// </summary>
        [DefaultValue(GridListChangedInsertRemoveBehavior.InvalidateVisible)]
        [Category("Optimization")]
        [Description("Specifies how the grid should react if records are inserted or deleted.")]
        public GridListChangedInsertRemoveBehavior InsertRemoveBehavior
        {
            get { return Engine.InsertRemoveBehavior; }
            set { Engine.InsertRemoveBehavior = value; }
        }

        /// <summary>
        /// Specifies how the grid should react if records are inserted or deleted through direct
        /// user interaction with AddNew record.
        /// </summary>
        [DefaultValue(GridListChangedInsertRemoveBehavior.InvalidateAll)]
        [Category("Optimization")]
        [Description("Specifies how the grid should react if records are inserted or deleted through direct user interaction with AddNew record.")]
        public GridListChangedInsertRemoveBehavior InsertRemoveBehaviorWithEndEdit
        {
            get { return Engine.InsertRemoveBehaviorWithEndEdit; }
            set { Engine.InsertRemoveBehaviorWithEndEdit = value; }
        }

        /// <summary>
        /// Specifies how the grid should react if the sort position of a record records changes.
        /// </summary>
        [DefaultValue(GridListChangedInsertRemoveBehavior.InvalidateVisible)]
        [Category("Optimization")]
        [Description("Specifies how the grid should react if the sort position of a record records changes.")]
        public GridListChangedInsertRemoveBehavior SortPositionChangedBehavior
        {
            get { return Engine.SortPositionChangedBehavior; }
            set { Engine.SortPositionChangedBehavior = value; }
        }

        /// <summary>
        /// Specifies how the grid should react if the sort position of a record records changes when the current record 
        /// is edited interactively by user.
        /// </summary>
        [DefaultValue(GridListChangedInsertRemoveBehavior.InvalidateAll)]
        [Category("Optimization")]
        [Description("Specifies how the grid should react if the sort position of a record records changes when the current record is edited interactively by user..")]
        public GridListChangedInsertRemoveBehavior SortPositionChangedBehaviorWithEndEdit
        {
            get { return Engine.SortPositionChangedBehaviorWithEndEdit; }
            set { Engine.SortPositionChangedBehaviorWithEndEdit = value; }
        }

        /// <summary>
        /// Gets or sets the time in milliseconds how long to highlight values in a record after a change 
        /// was detected. The engine will highlight a cell for the specified period in milliseconds if
        /// the value was increased or decreased. If set to 0 the feature is disabled.
        /// </summary>
        [DefaultValue(0)]
        [Category("Grouping Control")]
        [Description("Time in milliseconds how long to highlight values in a record after a change was detected. Specify 0 to disable feature.")]
        public int BlinkTime
        {
            get
            {
                return Engine.BlinkTime;
            }

            set
            {
                Engine.BlinkTime = value;
            }
        }

        /// <summary>
        /// Gets the current <see cref="BlinkState"/> for a cell indicating whether the cells
        /// value was increased or decreased or if the record has been recently added. The
        /// BlinkState will be reset to BlinkState.None after the interval specified in <see cref="BlinkTime"/> 
        /// elapsed.
        /// </summary>
        /// <param name="tableCellIdentity">The identity for the table cell.</param>
        /// <returns>Blink state.</returns>
        public BlinkState GetBlinkState(GridTableCellStyleInfoIdentity tableCellIdentity)
        {
            if (gridHelper != null)
            {
                return gridHelper.GetBlinkState(tableCellIdentity);
            }

            return BlinkState.None;
        }

        private BlinkUpdateTriggerModeStyle blinkUpdateMode = BlinkUpdateTriggerModeStyle.Idle;

        /// <summary>
        /// Gets or sets when checks for updates will happen while blinking support is enabled.
        /// </summary>
        /// <remarks>
        /// When blinking support is enabled, the default behavior is to use the Application.Idle event to test
        /// whether pending cell refreshes are required. If the Idle event is not being reliably raised in a timely fashion in your
        /// application, you can use this property to tell the grid to use either a System.Threading.Timer or a
        /// System.Windows.Forms.Timer to facilitate the grid testing for pending cell updates.
        /// </remarks>
        [
        DefaultValue(BlinkUpdateTriggerModeStyle.Idle),
        Browsable(false)
        ]
        public BlinkUpdateTriggerModeStyle BlinkUpdateMode
        {
            get
            {
                return this.blinkUpdateMode;
            }
            set
            {
                if (this.blinkUpdateMode != value)
                {
                    this.blinkUpdateMode = value;
                    BlinkUpdateModeChanged(blinkUpdateMode);
                }
            }
        }

        private bool idleHooked = true;
        private System.Threading.Timer Sys_timer = null;
        private Timer WF_timer = null;
        private int blinkTimerRefreshInterval = 30;

        /// <summary>
        /// Gets or sets the timer interval (in milliseconds) used with <see cref="BlinkUpdateTriggerModeStyle"/> is set to use one of
        /// its timer methods to determine when the grid is refreshed for blinking.
        /// </summary>
        [
        DefaultValue(30),
        Browsable(false)
        ]
        public int BlinkTimerRefreshInterval
        {
            get { return blinkTimerRefreshInterval; }
            set 
            {
                if (blinkTimerRefreshInterval != value)
                {
                    blinkTimerRefreshInterval = value;
                    //force timers to reset
                    BlinkUpdateModeChanged(this.blinkUpdateMode);
                }
            }
        }


        private void BlinkUpdateModeChanged(BlinkUpdateTriggerModeStyle blinkUpdateMode)
        {
            ResetWF_Timer();
            ResetSys_Timer();
            ResetIdle();

            if (blinkUpdateMode == BlinkUpdateTriggerModeStyle.Idle)
            {
                Application.Idle += new EventHandler(Application_Idle);
            }
            else if (blinkUpdateMode == BlinkUpdateTriggerModeStyle.WF_Timer)
            {
                WF_timer = new Timer();
                WF_timer.Interval = BlinkTimerRefreshInterval;
                WF_timer.Tick += new EventHandler(WF_timer_Tick);
                WF_timer.Start();
            }
            else if (blinkUpdateMode == BlinkUpdateTriggerModeStyle.System_Timer)
            {
                if (this.Handle == null)
                {
                    //postpone creating timer until window created.
                    this.HandleCreated += new EventHandler(GridGroupingControl_HandleCreated);
                }
                else
                {
                    Sys_timer = new System.Threading.Timer(InvokeThrottledUpdate, null, 0, BlinkTimerRefreshInterval);
                }
            }
        }

        void GridGroupingControl_HandleCreated(object sender, EventArgs e)
        {
            this.HandleCreated -= new EventHandler(GridGroupingControl_HandleCreated);
            Sys_timer = new System.Threading.Timer(InvokeThrottledUpdate, null, 0, BlinkTimerRefreshInterval);
        }

        private void ResetIdle()
        {
            if (idleHooked)
            {
                Application.Idle -= new EventHandler(Application_Idle);
                idleHooked = false;
            }
        }

        private void ResetSys_Timer()
        {
            if (Sys_timer != null)
            {
                Sys_timer.Change(System.Threading.Timeout.Infinite, System.Threading.Timeout.Infinite);
                Sys_timer.Dispose();
                Sys_timer = null;
            }
        }

        private void InvokeThrottledUpdate(object o)
        {
            this.Invoke(new MethodInvoker(this.UpdateThrottled));
        }

        private void ResetWF_Timer()
        {
            if (WF_timer != null)
            {
                if (WF_timer.Enabled)
                    WF_timer.Stop();
                WF_timer.Tick -= new EventHandler(WF_timer_Tick);
                WF_timer.Dispose();
                WF_timer = null;
            }
        }
  
        private void WF_timer_Tick(object sender, EventArgs e)
        {
            this.UpdateThrottled();
        }
        /// <summary>
        /// Gets the current <see cref="BlinkState"/> for a cell indicating whether the cells
        /// value was increased or decreased or if the record has been recently added. The
        /// BlinkState will be reset to BlinkState.None after the interval specified in <see cref="BlinkTime"/> 
        /// elapsed.
        /// </summary>       
        /// <param name="r">The descriptor of the field to which the cell belongs to.</param>
        /// <param name="fd">The record object.</param>
        /// <returns>Returns the current <see cref="BlinkState"/> for the given cell.</returns>
        public BlinkState GetBlinkState(Record r, FieldDescriptor fd)
        {
            if (gridHelper != null)
            {
                return gridHelper.GetBlinkState(r, fd);
            }

            return BlinkState.None;
        }

        /// <summary>
        /// Initializes recommended settings to improve handling of ListChanged events
        /// and scrolling through grid. Affected settings are: TableOptions.ColumnsMaxLengthStrategy,
        /// TableOptions.GridLineBorder, TableOptions.DrawTextWithGdiInterop, TableOptions.VerticalPixelScroll,
        /// Appearance.AnyRecordFieldCell.WrapText and  Appearance.AnyRecordFieldCell.Trimming.
        /// </summary>
        [DefaultValue(false)]
        [Category("Optimization")]
        [Description("Specifies whether to use default settings that improve handling of ListChanged events and scrolling through grid.")]
        public bool UseDefaultsForFasterDrawing
        {
            get
            {
                return Engine.UseDefaultsForFasterDrawing;
            }

            set
            {
                Engine.UseDefaultsForFasterDrawing = value;
            }
        }

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
                return Engine.AllowedOptimizations;
            }

            set
            {
                Engine.AllowedOptimizations = value;
            }
        }

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
            get { return Engine.UseOldListChangedHandler; }
            set { Engine.UseOldListChangedHandler = value; }
        }

        /// <summary>
        /// Determines whether the UseOldListChangedHandler property was modified.
        /// </summary>
        /// <returns>True if it was modified.</returns>
        public bool ShouldSerializeUseOldListChangedHandler()
        {
            return Engine.ShouldSerializeUseOldListChangedHandler();
        }

        /// <summary>
        /// Discards any changes for the UseOldListChangedHandler property.
        /// </summary>
        public void ResetUseOldListChangedHandler()
        {
            Engine.ResetUseOldListChangedHandler();
        }

        /// <summary>
        /// When you use the new UniformChildList behavior (i.e. when Engine.UseOldUniformChildListRelation = false) 
        /// you can specify with this UseLazyUniformChildListRelation property whether the engine should 
        /// access and enumerate the child collections only once the user expands a record.
        /// This will speed up load time of the grid and reduce memory usage when not all records 
        /// get expanded.
        /// </summary>
        [Description("Specifies whether the engine should access and enumerate child collections only once the user expands a record.")]
        [Category("Optimization")]
        public bool UseLazyUniformChildListRelation
        {
            get { return Engine.UseLazyUniformChildListRelation; }
            set { Engine.UseLazyUniformChildListRelation = value; }
        }

        /// <summary>
        /// Determines whether the content of UseLazyUniformChildListRelation property was modified.
        /// </summary>
        /// <returns>True if it was modified.</returns>
        public bool ShouldSerializeUseLazyUniformChildListRelation()
        {
            return Engine.ShouldSerializeUseLazyUniformChildListRelation();
        }

        /// <summary>
        /// Discards any changes for the UseLazyUniformChildListRelation property.
        /// </summary>
        public void ResetUseLazyUniformChildListRelation()
        {
            Engine.ResetUseLazyUniformChildListRelation();
        }

        /// <summary>
        /// When the engine handles the ListChanged event it will itself raise numerous events. When set
        /// to true this the events will only be raised on the Engine object. If set to false
        /// then events will also be raised on inner objects (will bubble up on nested tables which
        /// caused some performance overhead). Property will only have effect if UseOldListChangedHandler = false.
        /// </summary>
        [DefaultValue(true)]
        [Description("Raise events that are triggerer by the tables ListChanged handler only on the engine object or also on nested inner objects.")]
        [Category("Optimization")]
        public bool RaiseSourceListChangedEventsOnEngineOnly
        {
            get { return Engine.RaiseSourceListChangedEventsOnEngineOnly; }
            set { Engine.RaiseSourceListChangedEventsOnEngineOnly = value; }
        }

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
                return Engine.CounterLogic;
            }

            set
            {
                Engine.CounterLogic = value;
            }
        }

        /// <summary>
        /// Marks the field in the record to be repainted later when 
        /// the grid paints pending changes from ListChanged events as 
        /// specified with <see cref="UpdateDisplayFrequency"/>.
        /// </summary>
        /// <param name="record">The record.</param>
        /// <param name="fieldName">The name of the file.</param>
        public void AddPendingUpdate(Record record, string fieldName)
        {
            if (this.gridHelper != null)
            {
                gridHelper.AddPendingUpdate(record, fieldName);
            }
        }

        /// <summary>
        /// Marks the record to be repainted later when 
        /// the grid paints pending changes from ListChanged events as 
        /// specified with <see cref="UpdateDisplayFrequency"/>.
        /// </summary>
        /// <param name="record">The record.</param>
        public void AddPendingUpdate(Record record)
        {
            if (this.gridHelper != null)
            {
                gridHelper.AddPendingUpdate(record, null);
            }
        }

        /// <summary>
        /// Marks the whole grid to be repainted later when 
        /// the grid paints pending changes from ListChanged events as 
        /// specified with <see cref="UpdateDisplayFrequency"/>.
        /// </summary>
        public void InvalidateDisplay()
        {
            if (this.gridHelper != null)
            {
                this.gridHelper.isInvalidated = true;
                this.gridHelper.UpdateThrottled();
            }
        }

        internal void SetInvalidated()
        {
            //// Called from GridTableControl.Invalidate
            if (this.gridHelper != null)
            {
                this.gridHelper.isInvalidated = true;
            }
        }

        /// <summary>
        /// Checks the grid if there are pending changes from ListChanged events
        /// and enough time has elapsed since the last update as specified with 
        /// <see cref="UpdateDisplayFrequency"/>.
        /// </summary>
        public void UpdateThrottled()
        {
            if (this.gridHelper != null)
            {
                this.gridHelper.UpdateThrottled();
            }
        }

        /// <summary>
        /// Marks a summary field to be updated next time when 
        /// the grid paints pending changes from ListChanged events as 
        /// specified with <see cref="UpdateDisplayFrequency"/>.
        /// </summary>
        /// <param name="group">The group  that defines a set to record that belongs to a category</param>
        /// <param name="fieldName">The field the summary is dependent on.</param>
        public void AddPendingSummaryUpdate(Group group, string fieldName)
        {
            if (this.gridHelper != null)
            {
                this.gridHelper.AddPendingSummaryUpdate(group, fieldName);
            }
        }

        /// <summary>
        /// Raises the <see cref="PaintingFields"/> event.
        /// </summary>
        /// <param name="e">Event data.</param>
        public virtual void OnPaintingFields(EventArgs e)
        {
            if (PaintingFields != null)
            {
                PaintingFields(this, e);
            }
        }

        /// <summary>
        /// Occurs when grid paints pending changes from ListChanged events as 
        /// specified with <see cref="UpdateDisplayFrequency"/>.
        /// </summary>
        [Description("Occurs when grid paints pending changes from ListChanged events"), Category("Layout")]
        public event EventHandler PaintingFields;

        internal bool OnTableControlPaint(GridTableControl sender, PaintEventArgs pe)
        {
            if (gridHelper != null)
            {
                return gridHelper.OnGridPaint(sender, pe);
            }

            return true;
        }

        /// <summary>
        /// Gets or sets whether the engine should sort mapping names alphabetically
        /// in the dropdown editors of the property grid. Default is false.
        /// </summary>
        [DefaultValue(false)]
        [Description("Specifies whether the engine should sort mapping names alphabetically in the dropdown editors of the property grid")]
        [Category("DesignTime")]
        public bool SortMappingNames
        {
            get { return Engine.SortMappingNames; }
            set { Engine.SortMappingNames = value; }
        }

        /// <summary>
        /// This event occurs when GridTableDescriptor is initializing columns with .AllowFilter set and gives
        /// you the option to handle filterbarchoices through custom code. In such case the event is also raised
        /// when the user clicks on on dropdown button of a GridTableFilterBarCell.
        /// </summary>
        [Description("Occurs when GridTableDescriptor is initializing columns with Filter enabled and gives option to handle FilterBarChoices through custom code"), Category("Filter")]
        public event GridQueryFilterBarChoicesEventHandler QueryFilterBarChoices;

        /// <summary>
        /// Raises the <see cref="QueryFilterBarChoices"/> event.
        /// </summary>
        /// <param name="e">Event data.</param>
        public virtual void OnQueryFilterBarChoices(GridQueryFilterBarChoicesEventArgs e)
        {
            if (QueryFilterBarChoices != null)
            {
                QueryFilterBarChoices(this, e);
            }
        }
        
        /// <summary>
        ///This event occurs when an item selected through the filtered dropdown.
        /// </summary>
        [Description("Occurs when an item is being selected through the filtered dropdown"), Category("Filter")]
        public event FilterBarSelectedItemChangingEventHandler FilterBarSelectedItemChanging;
        
        /// <summary>
        ///This event occurs after an item selected through the filtered dropdown.
        /// </summary>
        [Description("Occurs after an item selected through the filtered dropdown"), Category("Filter")]
        public event FilterBarSelectedItemChangedEventHandler FilterBarSelectedItemChanged;
        
        /// <summary>
        /// Raises the <see cref="FilterBarSelectedItemChanging"/> event.
        /// </summary>
        /// <param name="e">Event data.</param>
        public virtual void OnFilterBarSelectedItemChanging(FilterBarSelectedItemChangingEventArgs e)
        {
            if (FilterBarSelectedItemChanging != null)
            {
               FilterBarSelectedItemChanging(this,e);
            }
        }
        
        /// <summary>
        /// Raises the <see cref="FilterBarSelectedItemChanged"/> event.
        /// </summary>
        /// <param name="e">Event data.</param>
        public virtual void OnFilterBarSelectedItemChanged(FilterBarSelectedItemChangedEventArgs e)
        {
            if (FilterBarSelectedItemChanged != null)
            {
                FilterBarSelectedItemChanged(this, e);
            }
        }
    }

    /// <summary>
    /// Specifies delayed update behavior when changes are made to the underlying engine from outside of WndProc of this
    /// control, for example when changes made through an external PropertyGrid attached to the engine.
    /// </summary>
    /// <remarks>
    /// The grid does not to call update after each and every operation. Instead it leaves the
    /// update to the end user or to user interaction. Normally, operations on the engine are
    /// triggered from within a WndProc call. In the grids WndProc routine the grid will call its base
    /// class version and then after the WndProc returns it will call synchronize any changes to the underlying
    /// engine with the display if the message was a mouse operation.
    /// <para/>
    /// This has the big advantage that users don't have to worry about calling BeginUpdate / EndUpdate in
    /// mouse handling code. You can just batch operations and then manually call Update(). The programmer
    /// does not have to worry about calling Update() since that will be done once the Mouse event returns
    /// and the GridTableControl.WndProc is executed. The idea is to have good performance for the most typical case.
    /// Updating the grid after every operation would be too expensive (e.g. if you loop through the records and set
    /// IsExpanded = true for each record).
    /// </remarks>
    public enum GridDelayUpdateBehavior
    {
        /// <summary>
        /// No special delay update behavior. Programmer has to manually call Update. Only when a mouse message is handled
        /// WndProc will automatically synchronize display with engine.
        /// </summary>
        Disabled = 0,

        /// <summary>
        /// Use a timer do delay synchronization whenever changes in engine are detected that need the screen to be updated.
        /// PostCustomMessage and Timer can be combined.
        /// </summary>
        Timer = 1,

        /// <summary>
        /// Post a custom message to the message loop whenever changes in engine are detected that need the screen to be updated
        /// </summary>
        PostCustomMessage = 2,

        /// <summary>
        /// Post a custom message to the message loop and do also trigger a timer whenever changes in engine are detected that need the screen to be updated
        /// </summary>
        PostCustomMessageAndTimer = 3
    }

    /// <summary>
    /// Specifies the blink state for a cell indicating whether the cells
    /// value was increased or decreased or if the record has been recently added. The
    /// BlinkState will be reset to BlinkState.None after the interval specified in 
    /// <see cref="GridGroupingControl.BlinkTime"/> elapsed.
    /// </summary>
    public enum BlinkState
    {
        /// <summary>
        /// No change was detected recently.
        /// </summary>
        None,

        /// <summary>
        /// Value of cell was increased.
        /// </summary>
        Increased,

        /// <summary>
        /// Value of cell was reduced.
        /// </summary>
        Reduced,

        /// <summary>
        /// Cell belongs to a record that 
        /// has been recently added or cell was null before.
        /// </summary>
        NewRecord,

        /// <summary>
        /// A value has been applied to a cell that was null before.
        /// </summary>
        NewValue,

        /// <summary>
        /// Null has been applied to a cell that was a valid value before.
        /// </summary>
        NullValue
    }
    /// <summary>
    /// Determine the blink update modes
    /// </summary>
    public enum BlinkUpdateTriggerModeStyle
    {
        /// <summary>
        /// The default blink mode of blinking only on grid redraws.
        /// </summary>
        Idle,
        /// <summary>
        /// The enabled timer to redraw on each blink.
        /// </summary>
        WF_Timer,
        /// <summary>
        /// Update the grid if it runs on separte UI thread.
        /// </summary>
        System_Timer
    }
    /// <summary>
    /// Determine the GridGroupDropAreaAlignment
    /// </summary>
    public enum GridGroupDropAreaAlignment
    {
        Top,
        Left,
        Right,
        Bottom,
    }
    /// <exclude/>
    /// <summary>Used internally.</summary>
    public class GridGroupingControlOptimizeListChanged
    {
        GridGroupingControl gridGroupingControl1;

        /// <summary>
        /// Allows you to specify how often the display should be updated
        /// </summary>
        int UpdateDisplayFrequency
        {
            get
            {
                return gridGroupingControl1.UpdateDisplayFrequency;
            }

            set
            {
                gridGroupingControl1.UpdateDisplayFrequency = value;
            }
        }

        GridListChangedInsertRemoveBehavior InsertRemoveBehavior
        {
            get { return gridGroupingControl1.InsertRemoveBehavior; }
            set { gridGroupingControl1.InsertRemoveBehavior = value; }
        }

        /// <summary>
        /// Specifies how the grid should react if the records are inserted or deleted through
        /// direct user interaction with AddNew record.
        /// </summary>
        public GridListChangedInsertRemoveBehavior InsertRemoveBehaviorWithEndEdit
        {
            get { return gridGroupingControl1.InsertRemoveBehaviorWithEndEdit; }
            set { gridGroupingControl1.InsertRemoveBehaviorWithEndEdit = value; }
        }

        GridListChangedInsertRemoveBehavior SortPositionChangedBehavior
        {
            get { return gridGroupingControl1.SortPositionChangedBehavior; }
            set { gridGroupingControl1.SortPositionChangedBehavior = value; }
        }

        /// <summary>
        /// Specifies how the grid should react if the sort position of the record changes.
        /// </summary>
        public GridListChangedInsertRemoveBehavior SortPositionChangedBehaviorWithEndEdit
        {
            get { return gridGroupingControl1.SortPositionChangedBehaviorWithEndEdit; }
            set { gridGroupingControl1.SortPositionChangedBehaviorWithEndEdit = value; }
        }

        /// <summary>
        /// Specifies whether the update of display should be disabled.
        /// </summary>
        public bool DisableAutomaticUpdates
        {
            get
            {
                return UpdateDisplayFrequency == 0 || gridGroupingControl1.Engine.InvalidateAllWhenListChanged
                    || gridGroupingControl1.Engine.UseOldListChangedHandler
                    || gridGroupingControl1.UseCustomUpdateOnListChanged;
            }
        }

        /// <summary>
        /// Constructor for GridGroupingControlOptimizeListChanged.
        /// </summary>
        /// <param name="gridGroupingControl">The grouping grid.</param>
        public GridGroupingControlOptimizeListChanged(GridGroupingControl gridGroupingControl)
        {
            this.gridGroupingControl1 = gridGroupingControl;
        }

        ////bool isNavigateCalled = false;

        ////void gridGroupingControl_CurrentRecordContextChange(object sender, CurrentRecordContextChangeEventArgs e)
        ////{
        ////    return;
        ////    if (e.Record is NestedTable)
        ////        return;

        ////    GridGroupingControl gridGroupingControl = ((GridGroupingControl) sender);
        ////    GridTableControl gridTableControl = gridGroupingControl.TableControl;

        ////    switch (e.Action)
        ////    {
        ////        case CurrentRecordAction.NavigateCalled:
        ////            if (e.Table.CurrentRecord != null)
        ////            {
        ////                Record next = e.Table.CurrentRecord.GetNextRecord();
        ////                Record prev = e.Table.CurrentRecord.GetPreviousRecord();

        ////                //// Only optimize if next record is not sibling record.
        ////                isNavigateCalled = e.Record != next && e.Record != prev;
        ////                if (isNavigateCalled)
        ////                    gridTableControl.SuspendInvalidate();
        ////            }
        ////            break;

        ////        case CurrentRecordAction.NavigateComplete:
        ////            if (isNavigateCalled)
        ////                gridTableControl.ResumeInvalidate();
        ////            isNavigateCalled = false;
        ////            break;

        ////        case CurrentRecordAction.LeaveRecordComplete:
        ////        case CurrentRecordAction.EnterRecordComplete:
        ////            ////if (isNavigateCalled)
        ////              ////  PaintElement(gridTableControl, e.Record, string.Empty); ////Supported in 4.X
        ////            break;
        ////    }
        ////}
        #region Blinking
        
        int BlinkTime
        {
            get
            {
                return gridGroupingControl1.BlinkTime;
            }

            set
            {
                gridGroupingControl1.BlinkTime = value;
            }
        }
        
        void Blink(object sender, RecordChangedEventArgs rce)
        {
            if (BlinkTime <= 0)
            {
                return;
            }

            TableListChangedEventArgs e = rce.TableListChangedEventArgs;
            GridTable table = e.Table as GridTable;
            Record record = (Record)e.Table.UnsortedRecords[e.NewIndex];
            if (!record.GetVisibleInHierarchy())
            {
                return;
            }

            ArrayList changeFields = table.GetChangedFields();
            if (changeFields != null)
            {
                foreach (ChangedFieldInfo ci in changeFields)
                {
                    FieldDescriptor fd = table.TableDescriptor.Fields[ci.FieldIndex];

                    GridColumnDescriptor cd = table.TableDescriptor.Columns.FindByField(fd);
                    if (cd == null || !cd.AllowBlink)
                    {
                        continue;
                    }

                    BlinkState bs = BlinkState.None;

                    object value = record.GetValue(fd);
                    object oldValue = record.GetOldValue(ci.FieldIndex);

                    if (rce.Action == RecordChangedType.Added ||
                        // A BindingList will add a record and then send out subsequent ItemChanged notifications
                        // on that same record.
                        (rce.Action == RecordChangedType.Changed && oldValue == value) && rce.TableListChangedEventArgs.Table.LastAddNewIndex == rce.TableListChangedEventArgs.NewIndex)
                    {
                        bs = BlinkState.NewRecord;
                    }
                    else
                    {
                       
                        int cmp = CompareColumns.CompareNullableObjects(value, oldValue);
                        if (cmp > 0)
                        {
                            bs = BlinkState.Increased;
                            if (oldValue == null || oldValue is DBNull)
                            {
                                bs = BlinkState.NewValue;
                            }
                        }
                        else if (cmp < 0)
                        {
                            bs = BlinkState.Reduced;
                            if (value == null || value is DBNull)
                            {
                                bs = BlinkState.NullValue;
                            }
                        }
                    }

                    if (bs != BlinkState.None)
                    {
                        object key = BlinkInfo.GetKey(record, ci.FieldIndex);
                        if (blinkTable.Contains(key))
                        {
                            BlinkInfo biOld = blinkTable[key] as BlinkInfo;
                            int /*n = blinkQueue.BinarySearch(biOld);
                            if (n == -1)*/
                                n = blinkQueue.IndexOf(biOld);
                            if (n != -1)
                            {
                                blinkQueue.RemoveAt(n);
                            }
                        }

                        BlinkInfo bi = new BlinkInfo(bs, record, ci.FieldIndex, Environment.TickCount, ci);
                        blinkQueue.Add(bi);
                        blinkTable[key] = bi;

                        AddPendingUpdateInfo(record, ci);
                    }
                }
            }
        }

        ////        public int IndexOfBlink(Record r, int fieldIndex)
        ////        {
        ////            if (r.Tag == null)
        ////    return -1;
        ////
        ////            BlinkInfo[] bis = (BlinkInfo[]) Tag;
        ////
        ////            for (int n = 0; n < bis.Length; n++)
        ////    if (bis[n].fieldIndex == fieldIndex)
        ////        return n;
        ////
        ////            return -1;
        ////        }
        ////
        ////        public void AddBlink(Record r, BlinkInfo bi)
        ////        {
        ////            if (r.Tag != null)
        ////            {
        ////    r.Tag = new BlinkInfo[.Length + 1];
        ////    ((BlinkInfo[]) r.Tag).CopyTo
        ////        }

        /// <summary>
        /// Allows the custom formatting of a cell by changing its style object.
        /// </summary>
        /// <param name="sender">Event source.</param>
        /// <param name="e">Event data.</param>
        public virtual void OnGridTableControlPrepareViewStyleInfo(object sender, GridTableControlPrepareViewStyleInfoEventArgs e)
        {
            if (this.gridGroupingControl1.UseOldListChangedHandler
                || this.gridGroupingControl1.UseCustomUpdateOnListChanged
                || this.gridGroupingControl1.InvalidateAllWhenListChanged
                || e.Inner.Cancel)
            {
                return;
            }

            GridTableCellStyleInfo style = (GridTableCellStyleInfo)e.Inner.Style;
            BlinkState bs = GetBlinkState(style.TableCellIdentity);

            if (bs != BlinkState.None)
            {
                GridTableCellViewStyleInfoIdentity id = e.Inner.Style.Identity as GridTableCellViewStyleInfoIdentity;
                if (!id.ParentStyle.HasBaseStyle && !style.HasBaseStyle)
                {
                    e.Inner.Style.BaseStyle = "Blink" + bs.ToString();
                }
            }
        }

        /// <summary>
        /// Gets the current <see cref="BlinkState"/> for a cell indicating whether the cells
        /// value was increased or decreased or if the record has been recently added.
        /// </summary>
        /// <param name="tableCellIdentity">The identity for the table cell.</param>
        /// <returns>Blink state.</returns>
        public BlinkState GetBlinkState(GridTableCellStyleInfoIdentity tableCellIdentity)
        {
            if (blinkQueue.Count > 0)
            {
                if (tableCellIdentity.TableCellType == GridTableCellType.RecordFieldCell
                    || tableCellIdentity.TableCellType == GridTableCellType.AlternateRecordFieldCell)
                {
                    Record r = Record.GetRecord(tableCellIdentity.DisplayElement);
                    return GetBlinkState(r, tableCellIdentity.Column.FieldDescriptor);
                }
            }

            return BlinkState.None;
        }

        /// <summary>
        /// Gets the current <see cref="BlinkState"/> for a cell indicating whether the cells
        /// value was increased or decreased or if the record has been recently added.
        /// </summary>
        /// <param name="r">The record.</param>
        /// <param name="fd">The field descriptor.</param>
        /// <returns>Blink state.</returns>
        public BlinkState GetBlinkState(Record r, FieldDescriptor fd)
        {
            object key = BlinkInfo.GetKey(r, r.ParentTableDescriptor.Fields.IndexOf(fd));
            if (blinkTable.Contains(key))
            {
                BlinkInfo bi = (BlinkInfo)blinkTable[key];
                return bi.BlinkState;
            }

            return BlinkState.None;
        }

        Hashtable blinkTable = new Hashtable();
        ArrayList blinkQueue = new ArrayList();

        internal class BlinkInfo
        {
            public BlinkState BlinkState;
            public int tickCount;
            public Record record;
            public int fieldIndex;
            public ChangedFieldInfo ci;

            public BlinkInfo(BlinkState state, Record r, int fieldIndex, int tickCount, ChangedFieldInfo ci)
            {
                this.BlinkState = state;
                this.record = r;
                this.fieldIndex = fieldIndex;
                this.tickCount = tickCount;
                this.ci = ci;
            }

            public UInt64 GetKey()
            {
                return GetKey(record, fieldIndex);
            }

            public static UInt64 GetKey(Record record, int fieldIndex)
            {
                UInt64 u = ((UInt64)record.Id) << 32;
                return u + (UInt64)fieldIndex;
            }
        }
        #endregion

        #region Insert Remove and Changes
        class DeleteRecordInfo
        {
            public int yPos;
            public int yAmount;
            public int visibleCount;
            public int lastVisibleRow;
            public int topRowIndex;
            public bool meetsFilterCriteria;
            public bool visibleInHierarchy;
            public Element element;
            public GridRecord record;
            public int pos;
            public bool hasPartialVisibleRows;

            public override string ToString()
            {
                StringBuilder sb = new StringBuilder();
                sb.Append("DeleteRecordInfo:");
                sb.AppendFormat("yPos = {0}\n", yPos);
                sb.AppendFormat("yAmount = {0}\n", yAmount);
                sb.AppendFormat("visibleCount = {0}\n", visibleCount);
                sb.AppendFormat("topRowIndex = {0}\n", topRowIndex);
                sb.AppendFormat("meetsFilterCriteria = {0}\n", meetsFilterCriteria);
                sb.AppendFormat("visibleInHierarchy = {0}\n", visibleInHierarchy);
                sb.AppendFormat("element = {0}\n", element != null ? element.GetType().Name : "null");
                if (element != null)
                {
                    sb.AppendFormat("Table = {0}\n", element.ParentTable.ToString());
                }

                sb.AppendFormat("record = {0}\n", record != null ? record.ToString() : "null");
                sb.AppendFormat("pos = {0}\n", pos);
                sb.AppendFormat("hasPartialVisibleRows = {0}\n", hasPartialVisibleRows);
                return sb.ToString();
            }
        }

        DeleteRecordInfo deleteRecordInfo = null;

        private Rectangle HandleInsertedElement(Element element, bool allowUpdate, bool invalidateIfVisible)
        {
            ////using (Syncfusion.Diagnostics.MeasureTime.Measure("HanldeInserted"))
            {
                GridRecord rec = element as GridRecord;

                //// Does record affect display, current cell or scrollbars at all?
                if (rec != null && !rec.MeetsFilterCriteria())
                {
                    return Rectangle.Empty;
                }

                if (!element.GetVisibleInHierarchy())
                {
                    return Rectangle.Empty;
                }

                GridGroupingControl gridGroupingControl = ((GridEngine)element.Engine).ParentControl;
                GridTableControl gridTableControl = gridGroupingControl.TableControl;

                //// Get position of new element
                int pos = gridTableControl.Table.NestedDisplayElements.IndexOf(element);

                //// When element is ChildTable, ParentNestedTable could be null and in that
                //// case the record is not visible
                if (pos == -1)
                {
                    return Rectangle.Empty;
                }

                //// Adjust current cell without raising any events.
                //// TODO: Ignore current cell if it belongs to a different child table.
                AdjustCurrentCell(element, pos, element.GetVisibleCount());

                //// Optimzations when inserting rows above or below current view

                //// removeElementYAmount != 0 if sorted position is changed and previous
                //// HandleRemovedElement removed element from viewable area. At that moment
                //// the scroll rectangle and yamount was saved but no ScrollWindow done.
                if (removeElementYAmount == 0 || lastRemovedRect.IsEmpty)
                {
                    //// If element is inserted below current view
                    if (pos > LastVisibleRow)
                    {
                        if (gridTableControl.ViewLayout.HasPartialVisibleRows)
                        {
                            //// there is not much to do other than update VScrollBar.Maximum
                            if (allowUpdate)
                            {
                                gridTableControl.UpdateScrollBars();
                            }

                            return Rectangle.Empty;
                        }
                    }                   
                    else if (pos + element.GetVisibleCount() <= gridTableControl.TopRowIndex)
                    { 
                        //// If element is above current view 
                        //// we can adjust scroll position and update VScrollBar.Maximum. There
                        //// is no flicker/update for cells on the display. Only scrollbar thumb
                        //// will be reduced and moved down.
                        AdjustTopRowIndex(gridTableControl, gridTableControl.TopRowIndex + element.GetVisibleCount(), allowUpdate);
                        return Rectangle.Empty;
                    }
                }

                //// Get visible screen position and height of new element.
                int yPos, yAmount;

                if (pos > LastVisibleRow && gridTableControl.ViewLayout.HasPartialVisibleRows)
                {
                    yPos = removeElementScrollRect.Y;
                    yAmount = 0;
                }
                else if (pos + element.GetVisibleCount() <= gridTableControl.TopRowIndex)
                {
                    yPos = removeElementScrollRect.Y;
                    yAmount = 0;
                    AdjustTopRowIndex(gridTableControl, gridTableControl.TopRowIndex + element.GetVisibleCount(), false);
                }
                else
                {
                    yPos = gridTableControl.ViewLayout.RowColToPoint(Math.Max(gridTableControl.TopRowIndex, pos), 0, GridCellSizeKind.VisibleSize).Y;
                    yAmount = (int)element.GetYAmountCount();
                }

                Rectangle gridRect = gridTableControl.ClientRectangle;
                Rectangle scrollRect = Rectangle.FromLTRB(gridRect.Left, yPos, gridRect.Right, gridRect.Bottom);
                ////Rectangle elementRect = new Rectangle(gridRect.Left, yPos, gridRect.Width, yAmount);

                //// Scroll area
                ResetViewLayout((GridTable)element.ParentTable);
#if TRACELOG
            Trace.WriteLine(String.Format("{0:hh:mm:ss:ff}: HandleInsertedElement: ScrollWindow(0, {1}, {2}, {3})", DateTime.Now, yAmount, scrollRect, scrollRect));
#endif
                Rectangle updateRect;

                if (invalidateIfVisible)
                {
                    gridTableControl.Invalidate(scrollRect);
                    updateRect = scrollRect;

                    if (allowUpdate)
                    {
                        gridTableControl.UpdateScrollBars();
                    }
                }
                else
                {
                    //// Combine previous HandleRemovedElement operation with this insert
                    //// operation to reduce flickering.
                    if (this.removeElementYAmount > 0 && !lastRemovedRect.IsEmpty)
                    {
                        if (this.removeElementScrollRect.Y == scrollRect.Y)
                        {
                            int combYAmount = yAmount - removeElementYAmount;

                            if (combYAmount != 0)
                            {
                                updateRect = ScrollWindow(gridTableControl, 0, combYAmount, scrollRect, scrollRect, true);
                                if (allowUpdate)
                                {
                                    this.ScrolledWithImmediateUpdate();
                                }

                                lastRemovedRect = new Rectangle(scrollRect.X, scrollRect.Top, scrollRect.Width, yAmount);
                            }
                            else
                            {
                                lastRemovedRect = updateRect = new Rectangle(removeElementScrollRect.X, removeElementScrollRect.Top, removeElementScrollRect.Width, this.removeElementYAmount);
                            }
                        }
                        else if (yAmount == removeElementYAmount)
                        {
                            Rectangle intersectScrollRect;
                            if (removeElementScrollRect.Y < scrollRect.Y)
                            {
                                intersectScrollRect = Rectangle.FromLTRB(removeElementScrollRect.X, removeElementScrollRect.Top, removeElementScrollRect.Right, scrollRect.Y + yAmount);
                                yAmount = -yAmount;
                            }
                            else
                            {
                                intersectScrollRect = Rectangle.FromLTRB(scrollRect.X, scrollRect.Top, scrollRect.Right, removeElementScrollRect.Y + yAmount);
                            }

                            updateRect = ScrollWindow(gridTableControl, 0, yAmount, intersectScrollRect, intersectScrollRect, true);
                            if (allowUpdate)
                            {
                                this.ScrolledWithImmediateUpdate();
                            }

                            lastRemovedRect = Rectangle.Empty;
                            return updateRect;
                        }
                        else
                        {
                            if (scrollRect.Y < removeElementScrollRect.Y)
                            {
                                Rectangle r1 = Rectangle.FromLTRB(scrollRect.Left, scrollRect.Y, scrollRect.Right, removeElementScrollRect.Y + yAmount);
                                int yAmount1 = yAmount;

                                Rectangle r2 = Rectangle.FromLTRB(scrollRect.Left, removeElementScrollRect.Y + yAmount, scrollRect.Right, removeElementScrollRect.Bottom);
                                int yAmount2 = yAmount - removeElementYAmount;

                                updateRect = ScrollWindow(gridTableControl, 0, yAmount1, r1, r1, true);

                                if (r2.Height > 0)
                                {
                                    ScrollWindow(gridTableControl, 0, yAmount2, r2, r2, true);
                                }
                            }
                            else
                            {
                                int yAmount1 = -removeElementYAmount;
                                Rectangle r1 = Rectangle.FromLTRB(scrollRect.Left, removeElementScrollRect.Y, scrollRect.Right, scrollRect.Y + removeElementYAmount);

                                int yAmount2 = -(removeElementYAmount - yAmount);
                                Rectangle r2 = Rectangle.FromLTRB(scrollRect.Left, scrollRect.Y + removeElementYAmount, scrollRect.Right, scrollRect.Bottom);

                                Rectangle updateRect1 = ScrollWindow(gridTableControl, 0, yAmount1, r1, r1, true);

                                if (r2.Height > 0)
                                {
                                    ScrollWindow(gridTableControl, 0, yAmount2, r2, r2, true);
                                }

                                updateRect = updateRect1;
                            }

                            if (allowUpdate)
                            {
                                this.ScrolledWithImmediateUpdate();
                            }

                            lastRemovedRect = Rectangle.Empty;
                        }
                    }
                    else
                    {
                        updateRect = ScrollWindow(gridTableControl, 0, yAmount, scrollRect, scrollRect, true);
                        if (allowUpdate)
                        {
                            lastRemovedRect = Rectangle.Empty;
                            ScrolledWithImmediateUpdate();
                        }
                    }
                }

                return updateRect;
            }
        }

        /// <summary>
        /// Scrolls the contents of the control.
        /// </summary>
        /// <param name="tc">The table control.</param>
        /// <param name="xAmount">Horizontal scroll offset.</param>
        /// <param name="yAmount">Vertical scroll offset.</param>
        /// <param name="rect">Scroll bounds.</param>
        /// <param name="clipRect">Clipping rectangle.</param>
        /// <param name="allowUpdate">If true, redraws the invalidated regions within its client area. </param>
        /// <returns>Resultant rectangle that was scrolled into view.</returns>
        public Rectangle ScrollWindow(GridTableControl tc, int xAmount, int yAmount, Rectangle rect, Rectangle clipRect, bool allowUpdate)
        {
            ////Console.WriteLine(String.Format("{0:hh:mm:ss:ff}: ScrollWindow(0, {1}, {2}, {3})", DateTime.Now, yAmount, rect, tc.TopRowIndex));
            tc.ScrollWindow(xAmount, yAmount, rect, clipRect, allowUpdate && !tc.HasDoubleBufferSurface);

            Rectangle updateRect = clipRect;
            if (xAmount < 0)
            {
                updateRect.Width = -xAmount;
                updateRect.X = clipRect.Right + xAmount;
            }
            else if (xAmount > 0)
            {
                updateRect.Width = xAmount;
            }

            if (yAmount < 0)
            {
                updateRect.Height = -yAmount;
                ////updateRect.Y += clipRect.Height + yAmount;
                updateRect.Y = clipRect.Bottom + yAmount;
            }
            else if (yAmount > 0)
            {
                updateRect.Height = yAmount;
            }

            if (allowUpdate)
            {
                DrawGrid(updateRect);
            }

            return updateRect;
        }

        bool HasPartialVisibleRows
        {
            get
            {
                return this.gridGroupingControl1.TableControl.ViewLayout.HasPartialVisibleRows;
                ////return this.gridGroupingControl1.TableControl.VScrollBar.Value < this.gridGroupingControl1.TableControl.VScrollBar.Maximum
                ////    || this.gridGroupingControl1.TableControl.VScrollBar.Minimum == this.gridGroupingControl1.TableControl.VScrollBar.Maximum;
            }
        }

        int LastVisibleRow
        {
            get
            {
                ////GridTableControl gridTableControl = this.gridGroupingControl1.TableControl;
                ////Rectangle vScrollBounds 
                ////int y = gridTableControl.GridBounds.Height - ;
                return this.gridGroupingControl1.TableControl.ViewLayout.LastVisibleRow;
            }
        }

        void AdjustTopRowIndex(GridTableControl gridTableControl, int topRow, bool updateScrollbars)
        {
            ////Trace.WriteLine(String.Format("InternalSetTopRow({0})", gridTableControl.TopRowIndex));

            //// Reset ViewLayout since mapping from client to absolut row indexes needs to be recalculated.
            gridTableControl.ViewLayout.Reset();

            //// Support for TableOptions.VerticalPixelScroll = true
            //// gridTableControl.InternalSetTopRow(topRow);
            gridTableControl.InternalSetTopRow(topRow, false);

            if (updateScrollbars)
            {
                gridTableControl.UpdateScrollBars();
            }
        }

        void AdjustCurrentCell(Element element, int pos, int visibleCount)
        {
            GridTableControl gridTableControl = gridGroupingControl1.TableControl;

            if (gridTableControl.CurrentCell.HasCurrentCell)
            {
                int r = gridTableControl.CurrentCell.RowIndex;
                if (pos <= r)
                {
                    r += visibleCount;
                    gridTableControl.CurrentCell.SetPositionNoActivate(r, gridTableControl.CurrentCell.ColIndex);
                    gridTableControl.CurrentCell.Renderer.RowIndex = r;
                }
            }

            Element el2 = element;
            if (el2 != null)
            {
                GridTable tbe = (GridTable)el2.ParentTable;

                while (tbe != null && tbe.RelationParentTable != null)
                {
                    GridChildTable ct = (GridChildTable)el2.ParentChildTable;
                    if (ct != null && ct.CurrentCell != null && ct.CurrentCell.HasCurrentCell)
                    {
                        int r = ct.CurrentCell.RowIndex;
                        int p = ct.DisplayElements.IndexOf(el2);
                        if (p <= r)
                        {
                            r += element.GetVisibleCount();
                            ct.CurrentCell.SetPositionNoActivate(r, gridTableControl.CurrentCell.ColIndex);
                            ct.CurrentCell.Renderer.RowIndex = r;
                        }
                    }

                    el2 = null;
                    tbe = null;
                    NestedTable nt = ct.ParentNestedTable;
                    if (nt != null)
                    {
                        el2 = nt.ParentRecord;
                        if (el2 != null)
                        {
                            tbe = (GridTable)el2.ParentTable;
                        }
                    }
                }
            }
        }
        /// <summary>
        /// Determine the imidiate updates.
        /// </summary>
        protected virtual void ScrolledWithImmediateUpdate()
        {
            GridTableControl gridTableControl = gridGroupingControl1.TableControl;
            if (!gridTableControl.HasDoubleBufferSurface)
            {
                gridTableControl.UpdateWithDrawClippedGrid(gridTableControl.InvalidBounds);
            }

            gridTableControl.UpdateScrollBars();
        }

        private DeleteRecordInfo PrepareRemoveElement(Element element, Record record)
        {
            DeleteRecordInfo dr = new DeleteRecordInfo();

            dr.element = element;
            dr.record = record as GridRecord;

            //// Does record affect display, current cell or scrollbars at all?
            dr.meetsFilterCriteria = dr.record == null || dr.record.GetSavedMeetsFilterCriteria();
            if (!dr.meetsFilterCriteria)
            {
                return dr;
            }

            dr.visibleInHierarchy = element.GetVisibleInHierarchy();
            if (!dr.visibleInHierarchy)
            {
                return dr;
            }

            GridGroupingControl gridGroupingControl = ((GridEngine)element.Engine).ParentControl;
            GridTableControl gridTableControl = gridGroupingControl.TableControl;

            //// Get position of element
            dr.pos = gridTableControl.Table.NestedDisplayElements.IndexOf(element);

            //// When element is ChildTable, ParentNestedTable could be null and in that
            //// case the record is not visible
            if (dr.pos == -1)
            {
                dr.visibleInHierarchy = false;
                return dr;
            }

            dr.hasPartialVisibleRows = HasPartialVisibleRows;
            dr.lastVisibleRow = LastVisibleRow;
            dr.topRowIndex = gridTableControl.TopRowIndex;

            if (dr.pos > dr.lastVisibleRow)
            {
                return dr;
            }

            //// Get visible screen position and height of new element.
            try
            {
                dr.yPos = gridTableControl.ViewLayout.RowColToPoint(Math.Max(dr.topRowIndex, dr.pos), 0, GridCellSizeKind.VisibleSize).Y;
            }
            catch (Exception ex)
            {
                dr.yPos = -1;
                Trace.WriteLine(ex.ToString());
                Trace.WriteLine(dr.ToString());
            }

            if (element is Record)
            {
                dr.yAmount = (int)((Record)element).GetInternalYAmountCount();
            }
            else
            {
                dr.yAmount = (int)element.GetYAmountCount();
            }

            if (element is Record)
            {
                dr.visibleCount = (int)((Record)element).GetInternalVisibleCount();
            }
            else
            {
                dr.visibleCount = element.GetVisibleCount();
            }

            return dr;
        }

        Rectangle lastRemovedRect = Rectangle.Empty;

        Rectangle HandleRemovedElement(DeleteRecordInfo dr, bool allowUpdate, bool invalidateIfVisible)
        {
            ////using (Syncfusion.Diagnostics.MeasureTime.Measure("HanldeRemoved"))
            {
                GridRecord record = dr.record;
                Element element = dr.element;

                if (!dr.visibleInHierarchy || !dr.meetsFilterCriteria)
                {
                    return Rectangle.Empty;
                }

                GridTableControl gridTableControl = gridGroupingControl1.TableControl;

                //// Get position of deleted element
                int pos = dr.pos;

                //// Adjust current cell without raising any events.
                AdjustCurrentCell(element, pos, -dr.visibleCount);

                //// Optimzations when removing rows above or below current view

                //// If element is inserted below current view
                if (dr.hasPartialVisibleRows && pos > dr.lastVisibleRow)
                {
                    //// there is not much to do other than update VScrollBar.Maximum
                    if (allowUpdate)
                    {
                        gridTableControl.UpdateScrollBars();
                    }

                    return Rectangle.Empty;
                }                
                else if (pos + dr.visibleCount <= dr.topRowIndex)
                {
                    //// If element is above current view 
                    //// we can adjust scroll position and update VScrollBar.Maximum. There
                    //// is no flicker/update for cells on the display. Only scrollbar thumb
                    //// will be reduced and moved down.
                    AdjustTopRowIndex(gridTableControl, dr.topRowIndex - dr.visibleCount, allowUpdate);
                    return Rectangle.Empty;
                }                
                else if (pos < dr.topRowIndex && pos + dr.visibleCount > dr.topRowIndex)
                {
                    // Handle special cases that affects both visible records in the view
                    // and also scrolling the view

                    // This should happen only rarely, therefore it is not worth
                    // saving extra values for each individual row height in order
                    // to only scroll parts of the rows, better is to invalidate then.
                    AdjustTopRowIndex(gridTableControl, pos + dr.visibleCount, true);
                    this.isInvalidated = true;
                    return Rectangle.Empty;
                }

                //// Get visible screen position and height of new element.
                int yPos = dr.yPos;
                int yAmount = dr.yAmount;

                Rectangle gridRect = gridTableControl.ClientRectangle;
                Rectangle scrollRect = Rectangle.FromLTRB(gridRect.Left, yPos, gridRect.Right, gridRect.Bottom);

                //// Scroll area
                ResetViewLayout((GridTable)dr.element.ParentTable);
#if TRACELOG
            Trace.WriteLine(String.Format("{0:hh:mm:ss:ff}: HandleRemovedElement: ScrollWindow(0, {1}, {2}, {3})", DateTime.Now, -yAmount, scrollRect, scrollRect));
#endif
                Rectangle updateRect;

                if (invalidateIfVisible)
                {
                    gridTableControl.Invalidate(scrollRect);
                    updateRect = scrollRect;

                    if (allowUpdate)
                    {
                        gridTableControl.UpdateScrollBars();
                    }
                }
                else
                {
                    this.removeElementScrollRect = Rectangle.Empty;
                    this.removeElementYAmount = 0;

                    if (!allowUpdate)
                    {
                        Rectangle srcRect = scrollRect;
                        if (srcRect.Height > yAmount)
                        {
                            srcRect.Height -= yAmount;
                        }

                        Rectangle destRect = srcRect;
                        srcRect.Y += yAmount;
                        updateRect = Rectangle.FromLTRB(srcRect.Left, scrollRect.Bottom - yAmount, srcRect.Right, srcRect.Bottom);
                        this.removeElementScrollRect = scrollRect;
                        this.removeElementYAmount = yAmount;
                    }
                    else
                    {
                        updateRect = ScrollWindow(gridTableControl, 0, -yAmount, scrollRect, scrollRect, true);
                        if (allowUpdate)
                        {
                            ScrolledWithImmediateUpdate();
                        }
                    }
                }

                return updateRect;
            }
        }

        Rectangle removeElementScrollRect;
        int removeElementYAmount;
        bool ignoreSummaryChanges = false;

        bool MarkSortedColumnsDirtyWhenSortedPositionChanged
        {
            get
            {
                return gridGroupingControl1.Engine.MarkSortedColumnsDirtyWhenSortedPositionChanged;
            }
        }

        /// <summary>
        /// Occurs when a record in the underlying datasource is added, changed or removed and before the Table was updated with this change.
        /// </summary>
        /// <param name="sender">Event source.</param>
        /// <param name="rce">Event data.</param>
        public virtual void OnGridSourceListRecordChanging(object sender, RecordChangedEventArgs rce)
        {
            ////using (Syncfusion.Diagnostics.MeasureTime.Measure("RecordChangng"))
            {
                lastRemovedRect = Rectangle.Empty;

                if (this.gridGroupingControl1.UseOldListChangedHandler
                    || this.gridGroupingControl1.UseCustomUpdateOnListChanged
                    || this.gridGroupingControl1.InvalidateAllWhenListChanged
                    || !rce.TableListChangedEventArgs.ShouldInvalidateScreen
                    || isInvalidated
                    || showEmptyGroups)
                {
                    return;
                }

                ignoreSummaryChanges = !rce.TableListChangedEventArgs.ShouldInvalidateScreen;

                deleteRecordInfo = null;
                switch (rce.Action)
                {
                    case RecordChangedType.Removed:
                        {
                            if (this.InsertRemoveBehavior != GridListChangedInsertRemoveBehavior.InvalidateAll)
                            {
                                Element el = rce.Group != null ? (Element)rce.Group : rce.Record;
                                deleteRecordInfo = PrepareRemoveElement(el, rce.Record);
                            }

                            break;
                        }

                    case RecordChangedType.Changed:
                        {
                            if (!rce.IsNestedRelationParentKeyFieldAffected && this.SortPositionChangedBehavior != GridListChangedInsertRemoveBehavior.InvalidateAll)
                            {
                                if (rce.Group != null)
                                {
                                    Element group = rce.Group;
                                    ////ShowEmptyGroups support not implemented: if (group is GridGroup && ((GridGroup) group).ReadGroupOptions.ShowEmptyGroups)
                                    ////    group = rce.Record;
                                    deleteRecordInfo = PrepareRemoveElement(group, rce.Record);

                                    //// Prevent PaintSummaryFields from later trying to paint the deleted group
                                    RemoveDirtyGroupSummaryForParentGroups(rce.Record, rce.Group);
                                }
                                else if (rce.SortedPositionChanged || (rce.VisibilityChanged && rce.Record.GetSavedMeetsFilterCriteria()))
                                {
                                    deleteRecordInfo = PrepareRemoveElement(rce.Record, rce.Record);
                                }
                                ////else
                                ////    EnsureVisibleRecords();
                            }
                            else
                            {
                                if (rce.SortedPositionChanged)
                                {
                                    isInvalidated = true;

                                    if (MarkSortedColumnsDirtyWhenSortedPositionChanged)
                                    {
                                        //// Bumping the Version of SortedColumns collection prevents
                                        //// further sort order reevaluation in Table.OnSourceListItemChanged
                                        rce.Record.ParentTableDescriptor.SortedColumns.BumpVersion();
                                    }

                                    break;
                                }

                                if (rce.IsNestedRelationParentKeyFieldAffected)
                                {
                                    isInvalidated = true;
                                }
                                ////EnsureVisibleRecords();
                            }

                            break;
                        }
                }
            }

            lastRemovedRect = Rectangle.Empty;
        }

        void RemoveDirtyGroupSummaryForParentGroups(Record rceRecord, Group rceGroup)
        {
            if (dirtyGroupSummary != null && rceRecord != null && rceGroup != null)
            {
                Group g = rceRecord.ParentGroup;
                while (g != null)
                {
                    if (dirtyGroupSummary.ContainsKey(g.Id))
                    {
                        dirtyGroupSummary.Remove(g.Id);
                    }

                    if (g == rceGroup)
                    {
                        break;
                    }

                    g = g.ParentGroup;
                }
            }
        }

        /// <summary>
        /// Occurs when a record in the underlying datasource is added, changed or removed and after the Table was updated with this change.
        /// </summary>
        /// <param name="sender">Event source.</param>
        /// <param name="rce">Event data.</param>
        public virtual void OnGridSourceListRecordChanged(object sender, RecordChangedEventArgs rce)
        {
            ////using (Syncfusion.Diagnostics.MeasureTime.Measure("RecordChanged"))
            {
#if TRACELOG
            Trace.WriteLine(String.Format("{0:hh:mm:ss:ff}: {1} gridGroupingControl1_SourceListRecordChanged: {2})", DateTime.Now, rce.Record.ParentTable.ToString(), rce.ToString()));
            ArrayList cf = rce.Record.ParentTable.GetChangedFields();
            if (cf != null)
            {
                StringBuilder changeFieldsString = new StringBuilder();
                foreach (ChangedFieldInfo ci in cf)
                    changeFieldsString.AppendFormat("{0}: {1} -> {2}; ", ci.Name, ci.OldValue, ci.NewValue);

                Trace.WriteLine(String.Format("{0:hh:mm:ss:ff}: {1} {2}", DateTime.Now, rce.TableListChangedEventArgs.ListChangedType, changeFieldsString.ToString()));
            }
#endif
                if (this.gridGroupingControl1.UseOldListChangedHandler
                    || this.gridGroupingControl1.UseCustomUpdateOnListChanged)
                {
                    return;
                }

                if (this.gridGroupingControl1.InvalidateAllWhenListChanged || rce.TableListChangedEventArgs.ShouldInvalidateGroupSortOrder)
                {
                    this.gridGroupingControl1.TableControl.synchronizeGridShouldInvalidate = true;
                    return;
                }

                if (showEmptyGroups)
                {
                    isInvalidated = true;
                }

                try
                {
                    if (!rce.TableListChangedEventArgs.ShouldInvalidateScreen || isInvalidated)
                    {
                        return;
                    }

                    TableListChangedEventArgs e = rce.TableListChangedEventArgs;
                    GridGroupingControl gridGroupingControl = (GridGroupingControl)sender;
                    GridTableControl gridTableControl = gridGroupingControl.TableControl;

                    if (rce.SortedPositionChanged || rce.GroupsChanged || rce.VisibilityChanged)
                    {
                        gridTableControl.Model.CoveredRanges.ResetCache();

                        if (!this.isInvalidated)
                        {
                            switch (rce.Action)
                            {
                                case RecordChangedType.Added:
                                    ////using (Syncfusion.Diagnostics.MeasureTime.Measure("Added"))
                                    {
                                        if (this.InsertRemoveBehavior == GridListChangedInsertRemoveBehavior.InvalidateAll)
                                        {
                                            this.isInvalidated = true;
                                        }

                                        if (e.Table.CurrentRecordManager.InEndEdit)
                                        {
                                            currentRecordEndEditCalled = true;
                                            if (this.InsertRemoveBehaviorWithEndEdit == GridListChangedInsertRemoveBehavior.InvalidateAll)
                                            {
                                                this.isInvalidated = true;
                                            }
                                        }

                                        if (!isInvalidated && this.InsertRemoveBehavior != GridListChangedInsertRemoveBehavior.InvalidateAll)
                                        {
                                            Element el = rce.Group != null ? (Element)rce.Group : rce.Record;
                                            Blink(sender, rce);
                                            HandleInsertedElement(el, true, InsertRemoveBehavior == GridListChangedInsertRemoveBehavior.InvalidateVisible);
                                        }
                                        else
                                        {
                                            this.isInvalidated = true;
                                        }

                                        break;
                                    }

                                case RecordChangedType.Removed:
                                    ////using (Syncfusion.Diagnostics.MeasureTime.Measure("Removed"))
                                    {
                                        if (this.InsertRemoveBehavior == GridListChangedInsertRemoveBehavior.InvalidateAll)
                                        {
                                            this.isInvalidated = true;
                                        }

                                        if (e.Table.CurrentRecordManager.InEndEdit)
                                        {
                                            currentRecordEndEditCalled = true;
                                            if (this.InsertRemoveBehaviorWithEndEdit == GridListChangedInsertRemoveBehavior.InvalidateAll)
                                            {
                                                this.isInvalidated = true;
                                            }
                                        }

                                        if (!isInvalidated && this.InsertRemoveBehavior != GridListChangedInsertRemoveBehavior.InvalidateAll)
                                        {
                                            // Make sure PaintPendingUpdates does not try to paint cells from this record later.
                                            if (rce.Record != null && pendingUpdates.ContainsKey(rce.Record.Id))
                                            {
                                                pendingUpdates.Remove(rce.Record.Id);
                                            }

                                            //// Prevent PaintSummaryFields from later trying to paint the deleted group
                                            RemoveDirtyGroupSummaryForParentGroups(rce.Record, rce.Group);

                                            HandleRemovedElement(deleteRecordInfo, true, InsertRemoveBehavior == GridListChangedInsertRemoveBehavior.InvalidateVisible);
                                        }
                                        else
                                        {
                                            this.isInvalidated = true;
                                        }

                                        break;
                                    }

                                case RecordChangedType.Changed:
                                    ////using (Syncfusion.Diagnostics.MeasureTime.Measure("Changed"))
                                    {
                                        if (this.SortPositionChangedBehavior == GridListChangedInsertRemoveBehavior.InvalidateAll)
                                        {
                                            this.isInvalidated = true;
                                        }

                                        if (e.Table.CurrentRecordManager.InEndEdit)
                                        {
                                            currentRecordEndEditCalled = true;
                                            if (this.SortPositionChangedBehaviorWithEndEdit == GridListChangedInsertRemoveBehavior.InvalidateAll)
                                            {
                                                this.isInvalidated = true;
                                            }
                                        }

                                        if (!isInvalidated)
                                        {
                                            Element el = rce.Group != null ? (Element)rce.Group : rce.Record;

                                            // RecordChangedType.Changed is raised twice if sort order is affected.
                                            // Exception: RecordChangedType.Changed is raised only once if visibility was changed
                                            if (deleteRecordInfo != null)
                                            {
                                                if (rce.SortedPositionChanged)
                                                {
                                                    lastRemovedRect = HandleRemovedElement(deleteRecordInfo, false, SortPositionChangedBehavior == GridListChangedInsertRemoveBehavior.InvalidateVisible);
                                                }
                                                else
                                                {
                                                    //// meet filter criteria status was changed (VisibilityChanged).
                                                    //// in this case there is no second call to HandleInsertedElement.
                                                    lastRemovedRect = HandleRemovedElement(deleteRecordInfo, true, SortPositionChangedBehavior == GridListChangedInsertRemoveBehavior.InvalidateVisible);

                                                    ////using (Syncfusion.Diagnostics.MeasureTime.Measure("ImmediateUpdate"))
                                                    {
                                                        //// RecordChangedType.Changed is raised only once if visibility was changed
                                                        Blink(sender, rce);

                                                        if (SortPositionChangedBehavior == GridListChangedInsertRemoveBehavior.ScrollWithImmediateUpdate)
                                                        {
                                                            ScrolledWithImmediateUpdate();
                                                        }
                                                    }
                                                }

                                                deleteRecordInfo = null;
                                                return;
                                            }

                                            if (rce.AddedGroup != null && rce.Group == null && rce.AddedGroup.GetVisibleCount() > 0)
                                            {
                                                el = rce.AddedGroup;
                                            }

                                            ////ShowEmptyGroups support not implemented: else if (el is GridGroup && ((GridGroup)el).ReadGroupOptions.ShowEmptyGroups)
                                            ////    el = rce.Record;

                                            Rectangle rcInsert = HandleInsertedElement(el, true, SortPositionChangedBehavior == GridListChangedInsertRemoveBehavior.InvalidateVisible);

                                            ////using (Syncfusion.Diagnostics.MeasureTime.Measure("AfterInsert"))
                                            {
                                                if (SortPositionChangedBehavior != GridListChangedInsertRemoveBehavior.InvalidateVisible
                                                    && SortPositionChangedBehavior != GridListChangedInsertRemoveBehavior.InvalidateAll)
                                                {
                                                    ////Trace.WriteLine(String.Format("lastRemovedRect = {0}", lastRemovedRect));
                                                    if (!lastRemovedRect.IsEmpty)
                                                    {
                                                        if (rcInsert.IsEmpty)
                                                        {
                                                            ScrollWindow(gridTableControl, 0, -removeElementYAmount, removeElementScrollRect, removeElementScrollRect, true);
                                                        }
                                                        else
                                                        {
                                                            if (lastRemovedRect.Y > rcInsert.Y)
                                                            {
                                                                lastRemovedRect.Offset(0, rcInsert.Height);
                                                            }

                                                            if (lastRemovedRect.IntersectsWith(gridTableControl.GridBounds))
                                                            {
                                                                lastRemovedRect.Intersect(gridTableControl.GridBounds);

                                                                DrawGrid(lastRemovedRect);
                                                            }
                                                        }
                                                    }

                                                    ////using (Syncfusion.Diagnostics.MeasureTime.Measure("ImmediateUpdate"))
                                                    {
                                                        Blink(sender, rce);

                                                        ////gridTableControl.Update();

                                                        if (SortPositionChangedBehavior == GridListChangedInsertRemoveBehavior.ScrollWithImmediateUpdate)
                                                        {
                                                            ScrolledWithImmediateUpdate();
                                                        }
                                                    }
                                                }

                                                lastRemovedRect = Rectangle.Empty;
                                            }
                                        }
                                        else
                                        {
                                            this.isInvalidated = true;
                                        }
                                        ////Thread.Sleep(300);   

                                        break;
                                    }
                            }
                        }

                        visibleRecords.Clear();
                    }
                    else
                    {
                        if (e.Table.CurrentRecordManager.InEndEdit)
                        {
                            currentRecordEndEditCalled = true;
                        }

                        Blink(sender, rce);

                        if (isInvalidated)
                        {
                            visibleRecords.Clear();
                            return;
                        }

                        ////using (MeasureTime.Measure("Form1.gridGroupingControl_SourceListListChangedCompleted"))
                        {
                            ////GridGroupingControl gridGroupingControl = (GridGroupingControl) sender;
                            ////GroupingEngine engine = (GroupingEngine) gridGroupingControl.Engine;
                            GridTable table = e.Table as GridTable;
                            ////GridTableControl gridTableControl = gridGroupingControl.TableControl;

                            EnsureVisibleRecords();

                            GridRecord record = (GridRecord)e.Table.UnsortedRecords[e.NewIndex];
                            int id = record.Id;
                            if (!visibleRecords.ContainsKey(id))
                            {
                                return;
                            }

                            ArrayList changeFields = table.GetChangedFields();
                            if (changeFields == null)
                            {
                                AddPendingUpdateInfo(record);
                            }
                            else if (changeFields.Count > 0)
                            {
                                AddPendingUpdateInfo(record, table.GetChangedFields());
                            }
                            ////else
                            ////    this.gridGroupingControl1.TableControl.Update();
                        }
                    }
                }
                finally
                {
                    ProcessGroupSummaryInvalidated();

                    ////   if (checkThrottlePaint)
                    {
                        if (lastUpdate == int.MinValue)
                        {
                            lastUpdate = Environment.TickCount;
                        }

                        this.UpdateThrottled();
                    }

                    deleteRecordInfo = null;
                }
            }
        }

        void ResetViewLayout(GridTable table)
        {
            GridTableModel tm = table.TableModel;
            GridControlBase grid = tm.ActiveGridView != null ? tm.ActiveGridView : null;
            if (grid != null)
            {
                grid.ViewLayout.Reset();
                if (table.RelationParentTable != null)
                {
                    ResetViewLayout(table.RelationParentTable);
                }
            }
        }

        #endregion
        #region VisibleRecords
        Hashtable visibleRecords = new Hashtable();

        /// <summary>
        /// Gets a collection of visible records.
        /// </summary>
        public Hashtable VisibleRecords
        {
            get
            {
                return visibleRecords;
            }
        }

        /// <summary>
        /// Occurs when a property is changed.
        /// </summary>
        /// <param name="sender">Event source.</param>
        /// <param name="e">Event data.</param>
        public virtual void OnGridPropertyChanged(object sender, DescriptorPropertyChangedEventArgs e)
        {
            showEmptyGroups = false;
            ClearVisibleRecord();
        }

        /// <summary>
        /// Occurs when the grid size is changed.
        /// </summary>
        /// <param name="sender">Event source.</param>
        /// <param name="e">Event data.</param>
        public virtual void OnGridSizeChanged(object sender, EventArgs e)
        {
            visibleRecords.Clear();
        }

        /// <summary>
        /// Occurs when a group is expanded.
        /// </summary>
        /// <param name="sender">Event source.</param>
        /// <param name="e">Event data.</param>
        public virtual void OnGridGroupExpanded(object sender, GroupEventArgs e)
        {
            visibleRecords.Clear();
        }

        /// <summary>
        /// Occurs when a group is collapsed.
        /// </summary>
        /// <param name="sender">Event source.</param>
        /// <param name="e">Event data.</param>
        public virtual void OnGridGroupCollapsed(object sender, GroupEventArgs e)
        {
            visibleRecords.Clear();
        }

        /// <summary>
        /// Occurs when a record is expanded.
        /// </summary>
        /// <param name="sender">Event source.</param>
        /// <param name="e">Event data.</param>
        public virtual void OnGridRecordExpanded(object sender, RecordEventArgs e)
        {
            visibleRecords.Clear();
        }

        /// <summary>
        /// Occurs when a record is collapsed.
        /// </summary>
        /// <param name="sender">Event source.</param>
        /// <param name="e">Event data.</param>
        public virtual void OnGridRecordCollapsed(object sender, RecordEventArgs e)
        {
            visibleRecords.Clear();
        }

        /// <summary>
        /// Occurs when the <see cref="GridControlBase.TopRowChanged"/> event of the underlying <see cref="GridTableControl"/> is raised.
        /// </summary>
        /// <param name="sender">Event source.</param>
        /// <param name="e">Event data.</param>
        public virtual void OnGridTableControlTopRowChanged(object sender, GridTableControlRowColIndexChangedEventArgs e)
        {
            visibleRecords.Clear();
        }

        void ClearVisibleRecord()
        {
            visibleRecords.Clear();
            this.blinkQueue.Clear();
            this.blinkTable.Clear();
        }

        void EnsureVisibleRecords()
        {
            GridTableControl gridTableControl = gridGroupingControl1.TableControl;
            if (this.visibleRecords.Count == 0)
            {
                int count = gridTableControl.ViewLayout.VisibleRows;
                int elementCount = gridTableControl.Table.NestedDisplayElements.Count;
                for (int n = 0; n <= count; n++)
                {
                    int rowIndex = gridTableControl.GetRow(n);
                    if (rowIndex >= elementCount)
                    {
                        continue;
                    }

                    Element el = gridTableControl.Table.NestedDisplayElements[rowIndex];
                    Record r = Element.GetRecord(el);
                    if (r != null)
                    {
                        int id = r.Id;
                        visibleRecords[id] = r;
                    }
                    else
                    {
                        Group g = el.ParentGroup;
                        if (g != null && !g.IsTopLevelGroup)
                        {
                            visibleRecords[g.Id] = g;
                        }
                    }
                }
            }
        }
        #endregion
        #region PendingUpdateInfo

        internal Hashtable dirtyGroupSummary = new Hashtable();

        ArrayList invalidateGroupsToBeProcessed = new ArrayList();

        /// <summary>
        /// Occurs when a summary has been marked dirty.
        /// </summary>
        /// <param name="sender">Event source.</param>
        /// <param name="e">Event data.</param>
        public virtual void OnGridGroupSummaryInvalidated(object sender, GroupEventArgs e)
        {
            // Do not process this at this time. Wait until SourceListRecordChanged event.
            // Then it is safe to access DisplayElements since Engine.Version will not be
            // increased any more. Otherwise we waste recreating the display elements cache.
            invalidateGroupsToBeProcessed.Add(e);
        }

        void ProcessGroupSummaryInvalidated()
        {
            if (this.gridGroupingControl1.UseOldListChangedHandler
                || this.gridGroupingControl1.UseCustomUpdateOnListChanged
                || this.gridGroupingControl1.InvalidateAllWhenListChanged
                || ignoreSummaryChanges
                || isInvalidated)
            {
            }
            else
            {
                GroupEventArgs[] groups = new GroupEventArgs[invalidateGroupsToBeProcessed.Count];
                invalidateGroupsToBeProcessed.CopyTo(groups, 0);
                foreach (GroupEventArgs e in groups)
                {
                    ProcessGroupSummaryInvalidated(e);
                }
            }

            invalidateGroupsToBeProcessed.Clear();
        }

        void ProcessGroupSummaryInvalidated(GroupEventArgs e)
        {
            ////EnsureVisibleRecords();
            if (e.Group.IsTopLevelGroup || e.Group.GetVisibleInHierarchy()) 
            {
                ////this.visibleRecords.ContainsKey(e.Group.Id))

                GroupSummaryDirtyInfo groupSummaryDirtyInfo;
                Group g = e.Group;
                object id = g.Id;
                if (dirtyGroupSummary.ContainsKey(id))
                {
                    groupSummaryDirtyInfo = (GroupSummaryDirtyInfo)this.dirtyGroupSummary[id];
                }
                else
                {
                    groupSummaryDirtyInfo = new GroupSummaryDirtyInfo(g);
                    dirtyGroupSummary.Add(id, groupSummaryDirtyInfo);
                }

                foreach (ChangedFieldInfo ci in g.ParentTable.ChangedFieldsArray)
                {
                    groupSummaryDirtyInfo.Fields[ci.FieldIndex] = ci.Name;
                }
            }
        }

        Hashtable highlightedElements = new Hashtable();

        internal void AddHighlightedElement(Element element)
        {
            highlightedElements[element] = element;
        }

        /// <summary>
        /// Mark a summary field to be updated next time the grid display is updated.
        /// </summary>
        /// <param name="group">The group that defines a set to record that belongs to a category.</param>
        /// <param name="fieldName">The field the summary is dependent on.</param>
        public void AddPendingSummaryUpdate(Group group, string fieldName)
        {
            if (isInvalidated || group == null)
            {
                return;
            }

            EnsureVisibleRecords();
            if (group.IsTopLevelGroup || this.visibleRecords.ContainsKey(group.Id))
            {
                GroupSummaryDirtyInfo groupSummaryDirtyInfo;
                Group g = group;
                object id = g.Id;
                if (dirtyGroupSummary.ContainsKey(id))
                {
                    groupSummaryDirtyInfo = (GroupSummaryDirtyInfo)this.dirtyGroupSummary[id];
                }
                else
                {
                    groupSummaryDirtyInfo = new GroupSummaryDirtyInfo(g);
                    dirtyGroupSummary.Add(id, groupSummaryDirtyInfo);
                }

                if (fieldName != null && fieldName != string.Empty)
                {
                    ChangedFieldInfo ci = new ChangedFieldInfo(group.ParentTableDescriptor, fieldName);
                    groupSummaryDirtyInfo.Fields[ci.FieldIndex] = ci.Name;
                }
            }
        }

        /// <summary>
        /// Holds information about summary fields in a group that need to be repainted
        /// </summary>
        internal class GroupSummaryDirtyInfo
        {
            Group group;
            Int32ToStringDictionary fields = new Int32ToStringDictionary();

            public GroupSummaryDirtyInfo(Group group)
            {
                this.group = group;
            }

            public Group Group
            {
                get
                {
                    return group;
                }
            }

            public Int32ToStringDictionary Fields
            {
                get
                {
                    return fields;
                }
            }
        }

#if SyncfusionFramework2_0
        class Int32ToChangedFieldInfoDictionary : System.Collections.Generic.Dictionary<int, ChangedFieldInfo>
        {
        }

        class Int32ToPendingUpdateInfoDictionary : System.Collections.Generic.Dictionary<int, PendingUpdateInfo>
        {
        }
#else
        class Int32ToChangedFieldInfoDictionary : Hashtable
        {
            public ChangedFieldInfo this[int index]
            {
    get
    {
        return (ChangedFieldInfo) base[index];
    }
    set
    {
        base[index] = value;
    }
            }
        }

        class Int32ToPendingUpdateInfoDictionary : Hashtable
        {
            public PendingUpdateInfo this[int index]
            {
    get
    {
        return (PendingUpdateInfo) base[index];
    }
    set
    {
        base[index] = value;
    }
            }
        }
#endif

        /// <summary>
        /// Marks the field in the record to be repainted later when 
        /// the grid paints pending changes from ListChanged events as 
        /// specified with <see cref="UpdateDisplayFrequency"/>.
        /// </summary>
        /// <param name="record">The record.</param>
        /// <param name="fieldName">The name of the file.</param>
        public void AddPendingUpdate(Record record, string fieldName)
        {
            if (this.isInvalidated)
            {
                return;
            }

            PendingUpdateInfo ef = getPendingUpdateInfo(record);

            if (fieldName != string.Empty && fieldName != null)
            {
                ChangedFieldInfo ci = new ChangedFieldInfo(record.ParentTableDescriptor, fieldName);
                if (!ef.fields.ContainsKey(ci.FieldIndex))
                {
                    ef.fields[ci.FieldIndex] = ci;
                }
            }
            else
            {
                ef.fields.Clear();
            }
        }

        void AddPendingUpdateInfo(Record record, ChangedFieldInfoCollection cc)
        {
            PendingUpdateInfo ef = getPendingUpdateInfo(record);
            if (cc != null)
            {
                foreach (ChangedFieldInfo ci in cc)
                {
                    ef.fields[ci.FieldIndex] = ci;
                }
            }
        }

        void AddPendingUpdateInfo(Record record)
        {
            getPendingUpdateInfo(record);
        }

        void AddPendingUpdateInfo(Record record, ChangedFieldInfo ci)
        {
            PendingUpdateInfo ef = getPendingUpdateInfo(record);
            if (ci != null)
            {
                ef.fields[ci.FieldIndex] = ci;
            }
        }

        PendingUpdateInfo getPendingUpdateInfo(Record record)
        {
            PendingUpdateInfo ef;
            int id = record.Id;
            if (this.pendingUpdates.ContainsKey(id))
            {
                ef = this.pendingUpdates[id];
            }
            else
            {
                ef = new PendingUpdateInfo();
                ef.r = record;
                pendingUpdates.Add(id, ef);
            }

            return ef;
        }

        Int32ToPendingUpdateInfoDictionary pendingUpdates = new Int32ToPendingUpdateInfoDictionary();

        class PendingUpdateInfo
        {
            public Record r;
            public Int32ToChangedFieldInfoDictionary fields = new Int32ToChangedFieldInfoDictionary();
        }
        #endregion
        #region PaintUpdatedRecordFields
        internal int lastUpdate = int.MinValue;
        internal bool isInvalidated = false;

        /// <summary>
        /// Paint fields in records that were changed since last call
        /// to PaintUpdatedRecordFields.
        /// </summary>
        public void PaintUpdatedRecordFields()
        {
            GridTableControl gridTableControl = gridGroupingControl1.TableControl;
            ////using (Syncfusion.Diagnostics.MeasureTime.Measure("PaintUpdatedRecordFields"))
            {
                inPaintUpdatedRecordFields = true;
                ProcessBlinkQueue();

                if (isInvalidated)
                {
                    ClearPending();
                    gridTableControl.Invalidate();  // beware, this sets isInvalidated ...
                    gridTableControl.MarkResync(true);
                    gridTableControl.SynchronizeGridWithEngine(); // get this out of the way before Update calls it
                    //// since it also possibly sets isInvalidated again.
                    isInvalidated = false;
                    gridTableControl.Update();
                    isInvalidated = false;
                }
                else
                {
                    IntPaintUpdatedRecordFields();
                    PaintUpdatedSummaryFields();
                    PaintHighlightedElements();
                    if (gridTableControl.HasDoubleBufferSurface)
                    {
                        gridTableControl.DoubleBufferSurface.Render();
                    }
                }

                inPaintUpdatedRecordFields = false;
            }
        }

        /// <summary>
        /// Draws the grid.
        /// </summary>
        /// <param name="clipBounds">Clipping bounds.</param>
        public virtual void DrawGrid(Rectangle clipBounds)
        {
            ////if (clipBounds.IsEmpty)
            ////    return;

            gridGroupingControl1.TableControl.UpdateWithDrawClippedGrid(clipBounds);
        }

        bool inPaintUpdatedRecordFields = false;

        /// <summary>
        /// Occurs when the <see cref="GridControlBase.CellDrawn"/> event of the underlying <see cref="GridTableControl"/> is raised.
        /// </summary>
        /// <param name="sender">Event source.</param>
        /// <param name="e">Event data.</param>
        public virtual void OnGridTableControlCellDrawn(object sender, GridTableControlDrawCellEventArgs e)
        {
            if (inPaintUpdatedRecordFields || pendingUpdates.Count == 0)
            {
                return;
            }

            if (!e.TableControl.GridBounds.Contains(e.Inner.Bounds))
            {
                return;
            }

            GridTableCellStyleInfo style = (GridTableCellStyleInfo)e.Inner.Style;

            if (style.TableCellIdentity.Column != null)
            {
                Record record = style.TableCellIdentity.DisplayElement.GetRecord();
                if (record != null)
                {
                    int id = record.Id;
                    if (this.pendingUpdates.ContainsKey(id))
                    {
                        PendingUpdateInfo ef;
                        ef = this.pendingUpdates[id];
                        int field = record.ParentTableDescriptor.Fields.IndexOf(style.TableCellIdentity.Column.FieldDescriptor);
                        if (ef.fields.ContainsKey(field))
                        {
                            ef.fields.Remove(field);
                            if (ef.fields.Count == 0)
                            {
                                this.pendingUpdates.Remove(id);
                            }
                        }
                    }
                }
            }
        }

        void ProcessBlinkQueue()
        {
            int lastUpdate = Environment.TickCount;

            while (blinkQueue.Count > 0 && lastUpdate - ((BlinkInfo)blinkQueue[0]).tickCount > BlinkTime)
            {
                BlinkInfo bi = (BlinkInfo)blinkQueue[0];
                blinkQueue.RemoveAt(0);

                BlinkInfo bi2 = (BlinkInfo)blinkTable[bi.GetKey()];
                if (bi2 != null && bi2.tickCount == bi.tickCount)
                {
                    blinkTable.Remove(bi.GetKey());
                }

                if (!isInvalidated)
                {
                    ////  Graphics cachedGraphics = gridTableControl.GetCachedGraphics();//Suported in 4.X
                    Record record = bi.record;
                    int id = record.Id;
                    EnsureVisibleRecords();
                    if (!visibleRecords.ContainsKey(id))
                    {
                        continue;
                    }

                    AddPendingUpdateInfo(record, bi.ci);
                }
            }
        }

        void ClearPending()
        {
            pendingUpdates.Clear();
            dirtyGroupSummary.Clear();
            invalidateGroupsToBeProcessed.Clear();
            highlightedElements.Clear();
        }

        void IntPaintUpdatedRecordFields()
        {
            lastUpdate = Environment.TickCount;
            GridTableControl gridTableControl = gridGroupingControl1.TableControl;

            this.gridGroupingControl1.OnPaintingFields(EventArgs.Empty);

            if (pendingUpdates.Count == 0)
            {
                return;
            }

            //// Review line
            this.gridGroupingControl1.TableControl.Update();
            ////foreach (GroupingTable table in gridGroupingControl1.Engine.EnumerateTables())
            ////{
            ////    this.ResetViewLayout(table);
            ////}
            ////this.gridGroupingControl1.Engine.BumpVersion();
            ////gridTableControl.Update();
            ////pendingUpdates.Clear();
            ////PaintUpdatedSummaryFields();
            ////return;

            ////using (MeasureTime.Measure("GroupingTable.PaintUpdatedRecordFields"))
            {
                PendingUpdateInfo[] pendingUpdatesValues = new PendingUpdateInfo[pendingUpdates.Count];
                pendingUpdates.Values.CopyTo(pendingUpdatesValues, 0);
                foreach (PendingUpdateInfo pendingUpdateInfo in pendingUpdatesValues)
                {
                    if (pendingUpdateInfo.r.IsDisposed)
                    {
                        continue;
                    }

                    if (pendingUpdateInfo.fields.Count == 0)
                    {
                        //// no info about individual fields - whole record needs to be repainted.
                        Record record = (Record)pendingUpdateInfo.r;
                        PaintElement(gridTableControl, record, null);
                    }
                    else
                    {
                        foreach (ChangedFieldInfo ci in pendingUpdateInfo.fields.Values)
                        {
                            string field = ci.Name;
                            Record record = (Record)pendingUpdateInfo.r;

                            PaintElement(gridTableControl, record, field);
                        }
                    }
                }

                pendingUpdates.Clear();
            }
        }

        int lastPaintHighlightedElements = int.MinValue;

        void PaintHighlightedElements()
        {
            if (lastPaintHighlightedElements != int.MinValue && Environment.TickCount - lastPaintHighlightedElements < 200)
            {
                return;
            }

            if (highlightedElements.Count > 0)
            {
                ////Console.WriteLine("highlightedElements {0}", highlightedElements.Count);
                GridTableControl gridTableControl = this.gridGroupingControl1.TableControl;
                if (gridTableControl == null)
                {
                    return;
                }

                Hashtable copy = highlightedElements;
                highlightedElements = new Hashtable();
                foreach (Element el in copy.Keys)
                {
                    ////Console.WriteLine(el.ToString());
                    PaintElement(gridTableControl, el, string.Empty);
                }
            }

            lastPaintHighlightedElements = Environment.TickCount;
        }

        /// <summary>
        /// Paint summary fields in group captions and summary rows that were changed since last call
        /// to PaintUpdatedSummaryFields.
        /// </summary>
        public void PaintUpdatedSummaryFields()
        {
            ////using (MeasureTime.Measure("PaintUpdatedSummaryFields"))
            {
                GridTableControl gridTableControl = this.gridGroupingControl1.TableControl;
                if (gridTableControl == null)
                {
                    return;
                }

                if (dirtyGroupSummary.Count == 0)
                {
                    return;
                }

                ////Review
                EnsureVisibleRecords();

                //// TODO: GridSummaryColumnDescripor ....
                GroupSummaryDirtyInfo[] dirtySummaries = new GroupSummaryDirtyInfo[dirtyGroupSummary.Count];
                dirtyGroupSummary.Values.CopyTo(dirtySummaries, 0);
                foreach (GroupSummaryDirtyInfo ef in dirtySummaries)
                {
                    if (ef.Group.IsDisposed)
                    {
                        continue;
                    }

                    GridTable table = (GridTable)ef.Group.ParentTable;
                    GridTableDescriptor td = table.TableDescriptor;
                    Hashtable fieldNames = new Hashtable();

                    foreach (string field in ef.Fields.Values)
                    {
                        fieldNames.Add(field, field);
                        foreach (GridSummaryColumnDescriptor scd in table.GetSummaryColumnCollection(td.Fields.IndexOf(field)))
                        {
                            GridColumnDescriptor cd = td.Columns[scd.DisplayColumn];
                            fieldNames[cd.MappingName] = scd;
                        }
                    }

                    GridGroupOptionsStyleInfo go = null;
                    if (ef.Group is GridGroup)
                    {
                        go = ((GridGroup)ef.Group).ReadGroupOptions;
                    }
                    else if (ef.Group is GridChildTable)
                    {
                        go = ((GridChildTable)ef.Group).ReadGroupOptions;
                    }

                    if (ef.Group.IsChildVisible(ef.Group.Caption))
                    {
                        // Repaint whole caption with all cells in it if RepaintCaptionWhenItemsChanged = true.
                        // That takes care of issue when caption and summary cells are combined.
                        bool captionDone = false;
                        if (go == null || go.RepaintCaptionWhenItemsChanged)
                        {
                            string text = GridEngine.GetGroupCaptionText(ef.Group);
                            string oldCaption = ef.Group.OldCaptionText;

                            if (oldCaption != text && ef.Group.Caption.GetVisibleInParent())
                            {
                                PaintElement(gridTableControl, ef.Group.Caption, string.Empty);
                                ef.Group.OldCaptionText = text;
                                captionDone = true;
                            }

                            if (!captionDone && go != null && go.ShowCaptionSummaryCells && ef.Group.Caption.GetVisibleInParent())
                            {
                                foreach (string f in fieldNames.Keys)
                                {
                                    PaintElement(gridTableControl, ef.Group.Caption, f);
                                }
                            }
                        }
                    }

                    if (table.TableDescriptor.SummaryRows.VisibleRowCount > 0 && ef.Group.IsChildVisible(ef.Group.Summary as Element) && go.ShowSummaries)
                    {
                        ////foreach (string f in fieldNames.Keys)
                        Element el = ef.Group.Summary as Element;
                        if (el != null && el.GetVisibleInParent())
                            PaintElement(gridTableControl, el, string.Empty);
                    }
                    ef.Group.Reserved2 = false;
                }

                dirtyGroupSummary.Clear();
            }
        }

        void PaintElement(GridTableControl gridTableControl, Element el, string field)
        {
            gridTableControl.PaintElement(el, field);
        }

        #endregion

        /// <summary>
        /// Occurs when the input focus leaves the control.
        /// </summary>
        /// <param name="sender">Event source.</param>
        /// <param name="e">Event data.</param>
        public virtual void OnGridLeave(object sender, EventArgs e)
        {
            if (!DisableAutomaticUpdates && this.UpdateDisplayFrequency == 100)
            {
                PaintUpdatedRecordFields();
            }
        }

        internal bool currentRecordEndEditCalled = false;

        /// <summary>
        /// Occurs when the grid application finishes its processing and is about to enter idle state.
        /// </summary>
        /// <param name="sender">Event source.</param>
        /// <param name="e">Event data.</param>
        public virtual void OnGridApplicationIdle(object sender, EventArgs e)
        {
            ////using (Syncfusion.Diagnostics.MeasureTime.Measure("OnGridApplicationIdle"))
            {
                UpdateThrottled();

                if (BlinkTime > 0 || currentRecordEndEditCalled)
                {
                    currentRecordEndEditCalled = false;
                    if (lastRemovedRect.IsEmpty && lastUpdate > int.MinValue && Environment.TickCount - lastUpdate > this.BlinkTime)
                    {
                        this.PaintUpdatedRecordFields();
                    }
                }
            }
        }

        /// <summary>
        /// Checks the grid if there are pending changes from ListChanged events
        /// and enough time has elapsed since the last update as specified with 
        /// <see cref="UpdateDisplayFrequency"/>.
        /// </summary>
        public void UpdateThrottled()
        {
            if (!DisableAutomaticUpdates && lastRemovedRect.IsEmpty)
            {
                if (lastUpdate == int.MinValue)
                {
                    return;
                }

                ////this.gridGroupingControl1.TableControl.Update();

                if (UpdateDisplayFrequency == 1 || Environment.TickCount - lastUpdate > this.UpdateDisplayFrequency)
                {
                    this.PaintUpdatedRecordFields();
                }
            }
        }

        /// <summary>
        /// Occurs before the records are categorized after the table is marked dirty.
        /// </summary>
        public virtual void OnGridCategorizingRecords()
        {
            isInvalidated = true;
            this.ClearPending();
        }

        bool showEmptyGroups;

        /// <summary>
        /// Occurs when grid paints pending changes from ListChanged events.
        /// </summary>
        /// <param name="sender">Event source.</param>
        /// <param name="pe">A <see cref="PaintEventArgs"/> holding the event data.</param>
        /// <returns>True if painting is successfully completed.</returns>
        public virtual bool OnGridPaint(GridTableControl sender, PaintEventArgs pe)
        {
            if (sender.Table.TableDescriptor.ChildGroupOptions.ShowEmptyGroups)
            {
                showEmptyGroups = true;
            }

            if (pe.ClipRectangle.Contains(sender.ClientRectangle))
            {
                // the whole grid is repainted, no need for manually painting individual
                // cells in PaintUpdatedRecordFields.
                isInvalidated = false;
                ProcessBlinkQueue();
                this.ClearPending();
            }
            else if (isInvalidated && this.UpdateDisplayFrequency == 100)
            {
                // Trigger call to PaintUpdatedRecordFields after this paint operation is finished.
                sender.BeginInvoke(new MethodInvoker(PaintUpdatedRecordFields));
            }

            return true;
        }

        /// <summary>
        /// Occurs when the focus leaves the control.
        /// </summary>
        /// <param name="sender">Event source.</param>
        /// <param name="e">Event data.</param>
        public virtual void OnGridLostFocus(object sender, EventArgs e)
        {
            if (!DisableAutomaticUpdates && this.UpdateDisplayFrequency == 100)
            {
                PaintUpdatedRecordFields();
            }
        }

        /// <summary>
        /// Occurs before a <see cref="System.Windows.Forms.Control.MouseDown"/> event and allows you to cancel the mouse event.
        /// </summary>
        /// <param name="sender">Event source.</param>
        /// <param name="e">Event data.</param>
        public virtual void OnGridTableControlScrollControlMouseDown(object sender, Syncfusion.Windows.Forms.CancelMouseEventArgs e)
        {
            if (!DisableAutomaticUpdates && this.UpdateDisplayFrequency == 100)
            {
                PaintUpdatedRecordFields();
            }
        }

        /// <summary>
        /// Occurs when a key is pressed when the control has focus.
        /// </summary>
        /// <param name="sender">Event source.</param>
        /// <param name="e">Event data.</param>
        public virtual void OnGridTableControlTableControlKeyDown(object sender, KeyEventArgs e)
        {
            if (!DisableAutomaticUpdates && this.UpdateDisplayFrequency == 100)
            {
                PaintUpdatedRecordFields();
            }
        }

        /// <summary>
        /// Occurs when the focus or keyboard user interface cues change.
        /// </summary>
        /// <param name="sender">Event source.</param>
        /// <param name="e">Event data.</param>
        public virtual void OnGridChangeUICues(object sender, UICuesEventArgs e)
        {
            if (!DisableAutomaticUpdates && this.UpdateDisplayFrequency == 100)
            {
                PaintUpdatedRecordFields();
            }
        }

        private void f_Deactivate(object sender, EventArgs e)
        {
            if (!DisableAutomaticUpdates && this.UpdateDisplayFrequency == 100)
            {
                PaintUpdatedRecordFields();
            }
        }
    }

    /// <summary>
    /// Used internally.
    /// </summary>
    [Documentation.DocumentationExclude()]
    public class GridTableCellTypeNameConverter : GridCellTypeNameConverter
    {
        /// <summary>
        /// Determine the basestyles.
        /// </summary>
        /// <param name="identity"></param>
        /// <returns></returns>
        protected override GridBaseStylesMap GetStylesMap(StyleInfoIdentityBase identity)
        {
            GridBaseStylesMap map = base.GetStylesMap(identity);

            if (map == null)
            {
                GridTableCellAppearanceStyleInfoIdentity caid = identity as GridTableCellAppearanceStyleInfoIdentity;
                if (caid.Engine != null)
                {
                    map = caid.Engine.TableModel.BaseStylesMap;
                }
            }

            return map;
        }
    }
}