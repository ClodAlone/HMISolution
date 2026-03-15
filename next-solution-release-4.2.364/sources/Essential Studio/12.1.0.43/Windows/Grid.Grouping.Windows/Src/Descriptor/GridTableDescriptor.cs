//-------------------------------------------------------------------------------------------------
// <copyright file="GridTableDescriptor.cs" company="syncfusion">
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
using System.Drawing;
using System.Windows.Forms;
using System.Xml.Serialization;

using Syncfusion.Design;
using Syncfusion.Collections;
using Syncfusion.ComponentModel;
using Syncfusion.Diagnostics;
using Syncfusion.Drawing;
using Syncfusion.Grouping;
using Syncfusion.Windows.Forms;
using Syncfusion.Windows.Forms.Grid;

using Table = Syncfusion.Grouping.Table;

#if ASPNET
using System.Web.UI;
using Syncfusion.Web.Design.UI;
using Syncfusion.Windows.Forms.Design;
using Syncfusion.Web.UI.WebControls.Tools;
using System.Drawing.Design;
namespace Syncfusion.Web.UI.WebControls.Grid.Grouping
#else
using Syncfusion.Windows.Forms.Grid.Grouping.Design;
using System.Collections.Generic;
namespace Syncfusion.Windows.Forms.Grid.Grouping
#endif
{
    
   /* /// <remarks>
    /// TableDescriptor has several collections that can either be auto-populated from the underlying datasource
    /// or manually modified.
    ///
    /// <list type="table">
    /// <listheader><term>Collection</term><description>Descriptions</description></listheader>
    ///
    /// <item><term>Fields</term><description>
    /// The Fields collection is auto-populated from the underlying sourcelist's columns. Normally you will not
    /// modify this collection; just use it in its default auto-populated state. A fieldDescriptor holds a
    /// name, mapping name, and type of the column in the list.
    /// </description></item>
    ///
    /// <item><term>Columns</term><description>
    /// The Columns collection lets you specify the fields that should be displayed in the GridTableControl.
    /// By default the Columns collection is auto-populated from the underlying Fields and ExpressionFields
    /// collections and will be a combination of these two collections. When the Columns collection is
    /// auto-populated and you make changes to the Fields or ExpressionFields collection, the changes will
    /// automatically be reflected in this collection. <para/>
    /// GridColumnDescriptors in the Columns collection have a reference to a FieldDescriptor
    /// (or ExpressionFieldDescriptor). <para/>
    /// Additionally GridColumnDescriptors contain grid-specific information about a column such as the column width.
    /// You can manually set the column width or have it be automatically initialized by the grid to fit the string
    /// with the maximum length in the columns data. GridColumnDescriptor also has an Appearance property. This is
    /// where cell type and formatting of the column can be specified.
    /// </description></item>
    ///
    /// <item><term>ColumnSets</term><description>
    /// The ColumnSet collection lets you specify a multi-row per record layout in a table. A ColumnSetDescriptor
    /// holds one or multiple ColumnSpans. In a GridColumnSpan, you can specify layout information of a column.
    /// You can, for example, specify that the Address column should be displayed in the grid above City and Region
    /// and span these two columns.
    /// </description></item>
    ///
    /// <item><term>VisibleColumns</term><description>
    /// The VisibleColumns collection is auto-populated from the Columns collection and the ColumnSets collection.
    ///  When auto-initialized, the VisibleColumns collection adds all GridColumnSetDescriptors from the ColumnSets
    ///  collection and also all ColumnDescriptors from the Columns collection that have not been referenced by a
    ///  ColumnSpan. So, if you do not specify any column sets, the VisibleColumns collection will have a
    ///  GridVisibleColumnDescriptor for each Column with the column's name.<para/>
    ///  You can also manually initialize the VisibleColumns collection. The name of the GridColumnSetDescriptor
    ///  identifies the column set descriptor or column in the VisibleColumns collection.
    /// </description></item>
    ///
    /// <item><term>ConditionalFormats</term><description>
    /// The ConditionalFormats collection has GridConditionalFormatDescriptor objects. The GridConditionalFormatDescriptor
    /// defines filter criteria for displaying a
    /// subset of records from the underlying datasource with conditional cell formatting.
    /// </description></item>
    ///*/
#if ASPNET
    /// <item><term>GridExpressionFields</term><description>
    /// The GridExpressionFields collection has Expression fields. You need to manually add entries to this collection. This
    ///  collection is not auto-populated from the datasource. A <see cref="GridExpressionFieldDescriptor"/> holds a name and the formula
    ///  expression of the column.
    /// </description></item>
    /// <item><term>GridGroupedColumns</term><description>
    /// The GridGroupedColumns collections contains SortColumnDescriptor objects. It defines the grouping of the
    ///  table. Each SortColumnDescriptor has a name that identifies a field in the Fields or ExpressionFields
    ///  collection, a SortDirection property, and a FieldDescriptor property. The FieldDescriptor is Read-only
    ///  and looked up in the Fields or ExpressionFields collection using the name of the SortColumnDescriptor.
    ///  A custom categorizer can be specified that allows you to group records into ranges of data, e.g. if
    ///  you want to group by month.
    /// </description></item>
      /// <item><term>GridSortedColumns</term><description>
    /// The GridSortedColumns collection contains GridSortColumnDescriptor objects. It specifies the sort order of
    /// records within a group.
    /// </description></item>
    /// <item><term>GridRecordFilters</term><description>
    /// The GridRecordFilters collection has GridRecordFilterDescriptor objects. GridRecordFilters define selection
    /// criteria to hide or show records based on criteria.
    /// </description></item>
    /// <item><term>GridRelations</term><description>
    /// The GridRelations collection is auto-populated from the underlying sourcelist's relations. It will extract
    /// its information from an ADO.NET datasource. If you have other IList collections that are related, you can
    /// add GridRelationDescriptor manually to this collection and specify the primary and foreign key between the
    /// two lists.
    /// </description></item>
    ///
#else
    /*/// <item><term>ExpressionFields</term><description>
    /// The ExpressionFields collection has ExpressionFields. You need to manually add ExpressionFields. This
    ///  collection is not auto-populated from the datasource. A fieldDescriptor holds a name and the formula
    ///  expression of the column.
    /// </description></item>
    /// <item><term>GroupedColumns</term><description>
    /// The GroupedColumns collections contains SortColumnDescriptor objects. It defines the grouping of the
    ///  table. Each SortColumnDescriptor has a name that identifies a field in the Fields or ExpressionFields
    ///  collection, a SortDirection property, and a FieldDescriptor property. The FieldDescriptor is Read-only
    ///  and looked up in the Fields or ExpressionFields collection using the name of the SortColumnDescriptor.
    ///  A custom categorizer can be specified that allows you to group records into ranges of data, e.g. if
    ///  you want to group by month.
    /// </description></item>
    /// <item><term>SortedColumns</term><description>
    /// The SortedColumns collection contains SortColumnDescriptor objects. It specifies the sort order of
    /// records within a group.
    /// </description></item>
    /// <item><term>RecordFilters</term><description>
    /// The RecordFilters collection has RecordFilterDescriptor objects. RecordFilters define selection
    /// criteria to hide or show records based on criteria.
    /// </description></item>
    /// <item><term>Relations</term><description>
    /// The Relations collection is auto-populated from the underlying sourcelist's relations. It will extract
    /// its information from an ADO.NET datasource. If you have other IList collections that are related, you can
    /// add RelationDescriptor manually to this collection and specify the primary and foreign key between the
    /// two lists.
    /// </description></item>
    /// <para/>*/
#endif
    /// <item><term>SummaryRows</term><description>
    /// The SummaryRows collection contains GridSummaryRowDescriptors. GridSummaryRowDescriptors have a name,
    /// title, and collection of summary columns. A GridSummaryRowDescriptor also has a Read-only
    /// IsFillRow property. If this property is True, the summary should fill the
    /// whole row and not be displayed below individual columns. IsFillRow will return True if
    /// any of the GridSummaryColummDescriptors in the SummaryColumns collection are set to
    /// GridSummaryStyle.FillRow.
    /// <para/>
    /// The GridSummaryColummDescriptor defines where to display the column in the row.
    /// Essential properties are the name, format, DisplayColumn, DataMember, and
    /// SummaryType. The multiple GridSummaryColumnDescriptor objects
    /// have a name and mapping name that identify the column for which a summary should be
    /// calculated for and a SummaryType property that defines the type of calculations to be performed.
    /// <para/>
    /// Possible SummaryTypes are: Count, BooleanAggregate, ByteAggregate, CharAggregate, DistinctCount,
    /// DoubleAggregate, Int32Aggregate, MaxLength, StringAggregate, Vector, DoubleVector, and Custom.
    /// <para/>
    /// When you specify the SummaryType.Custom type, you need to set the custom method through the
    /// CreateSummaryMethod property of the SummaryDescriptor. It is CreateSummaryDelegate and
    /// is called to create an instance of a summary object. You also need to handle the
    /// GridGroupingControl.QueryCustomSummary as demonstrated in the Grid/Grouping/CustomSummaries example.
    /// </description></item>
    /// <para/>
    /// <list/>
    /// <para/>
    /// The Field, Relations, and Columns collections feature auto-populating on demand
    /// and reflect changes from the collection they depend on. The auto-population will happen when
    /// you access the contents of the collection, e.g. if you query its Count.
    /// <para/>
    /// GroupedColumns, SortedColumns, and RecordFilters need to be manually initialized. They are not auto-populated.
    /// <para/>
    /// SortedColumnDescriptor also lets you specify a custom comparer that implements the IComparer
    /// interface.
    /// <para/>
    /// The AllowNew, AllowEdit, and AllowRemove properties let you specify whether the user should be allowed to modify the underlying table.
    /// <remarks/>
    /// <example>
    /// The columns collections feature auto-populating on demand and reflect changes from the collection they depend on. The auto-population will happen when you access the contents of the collection, e.g. if you query its Count.
    /// <code lang="C#">
    /// GridTableDescriptor orderDetailsTableDescriptor;
    /// <para/>
    /// // Lets check the count of each collection:
    /// Trace.WriteLine(orderDetailsTableDescriptor.Fields.Count); // returns 4
    /// Trace.WriteLine(orderDetailsTableDescriptor.ExpressionFields.Count); // returns 0
    /// Trace.WriteLine(orderDetailsTableDescriptor.Columns.Count); // returns 4 - will hold a columndescriptor for each field in the fields collection
    /// <para/>
    /// // Now, add a Expression Field.
    /// ExpressionFieldDescriptor ed = new ExpressionFieldDescriptor("Total", "Total", typeof(double), "[UnitPrice]*[Quantity]");
    ///             ed.DefaultValue = string.Empty;
    ///             orderDetailsTableDescriptor.ExpressionFields.Add(ed);
    /// <para/>
    /// <para/>
    /// // Lets check again the count of each collection:
    /// Trace.WriteLine(orderDetailsTableDescriptor.Fields.Count); // returns 4
    /// Trace.WriteLine(orderDetailsTableDescriptor.ExpressionFields.Count); // returns 1
    /// Trace.WriteLine(orderDetailsTableDescriptor.Columns.Count); // returns 5 - will hold a columndescriptor for each field in the fields collection and also a columndescriptor that references the expression field we just added.
    /// </code>
    /// <para/>
    /// Of course, you can also manually initialize the Columns collection. Once you modify a collection, it will not be auto re-initialized anymore.
    /// <para/>
    /// The following example shows how to add columns that should be displayed in the grid and initializes the width of one column:
    /// </example>
    /// <example>
    /// <code lang="C#">
    ///             GridTableDescriptor categoriesTableDescriptor = (GridTableDescriptor) engine.TableDescriptor;
    /// <para/>
    ///             categoriesTableDescriptor.Columns.Add("CategoryID");
    ///             categoriesTableDescriptor.Columns.Add("CategoryName");
    ///             categoriesTableDescriptor.Columns.Add("Description");
    /// <para/>
    ///             categoriesTableDescriptor.Columns["CategoryName"].Width = 200;
    /// </code>
    /// <para/>
    /// Now that the Columns collection has been initialized manually, changes in the underlying Fields or ExpressionFields collection will not be reflected. If you now add an ExpressionField to the ExpressionFields collection, you will also need to manually add it to the Columns collection in order to display it in the grid. Suppose you added a "Total" expression to the ExpressionFields collection. You can now add this expression field to the columns collection with:
    /// <para/>
    /// <code lang="C#">
    ///             categoriesTableDescriptor.Columns.Add("Total");
    /// </code>
    /// <para/>
    /// Only then new expression fields will be displayed.
    /// <para/>
    /// If you want to force re-initialization of a modified collection, you can call the ColumnDescriptorCollection.Reset() method. Once you call Columns.Reset, the columns collection will again auto-populate all fields from the Fields and ExpressionFields collections.
    /// <para/>
    /// The grid also supports displaying multiple rows per record.
    /// </example>
    /// <example>
    /// The ColumnSet collection lets you specify a multi-row per record layout in a table. A ColumnSetDescriptor holds one or multiple ColumnSpans. In a GridColumnSpan, you can specify layout information of a column. You ca,n for example, specify that the Address column should be displayed in the grid above City and Region and span these two columns:
    /// <code lang="C#">
    ///             GridColumnSpanDescriptor csd1 = new GridColumnSpanDescriptor("Address");
    ///             csd1.Range = GridRangeInfo.Cells(0,0,0,1);
    ///             GridColumnSpanDescriptor csd2 = new GridColumnSpanDescriptor("City");
    ///             csd2.Range = GridRangeInfo.Cells(1,0,1,0);
    ///             GridColumnSpanDescriptor csd3 = new GridColumnSpanDescriptor("Region");
    ///             csd3.Range = GridRangeInfo.Cells(1,1,1,1);
    ///             GridColumnSetDescriptor csd = new GridColumnSetDescriptor("Address_Set");
    ///             csd.ColumnSpans.Add(csd1);
    ///             csd.ColumnSpans.Add(csd2);
    ///             csd.ColumnSpans.Add(csd3);
    ///             this.groupingGrid1.TableDescriptor.ColumnSets.Add(csd);
    /// </code>
    /// </example>
    /// <example>You can also manually initialize the VisibleColumns collection. The name of the GridColumnSetDescriptor identifies the column set descriptor or column in the VisibleColumns collection:
    /// <code lang="C#">
    ///             this.groupingGrid1.TableDescriptor.VisibleColumns.Add("Address_Set");
    ///             this.groupingGrid1.TableDescriptor.VisibleColumns.Add("Phone");
    ///             this.groupingGrid1.TableDescriptor.VisibleColumns.Add("Fax");
    /// </code>
    /// A GridVisibleColumnDescriptor only has a Name property. The Name property identifies a ColumnSet or Column with the same name.
    /// </example>
    /// <summary>
    /// Maintains schema information for a table. Collections define columns, columnsets, fields, expressions,
    /// sorted, grouped columns, and related tables.
    /// </summary>
    [TypeConverter(typeof(DescriptorBaseConverter))]
    public class GridTableDescriptor : TableDescriptor, IGridTableCellAppearanceSource, IGridGroupOptionsSource, IGridTableOptionsSource
#if ASPNET
        //, IParserAccessor
#else
, IStandardValuesProvider
#endif
    {
        #region Fields
#if ASPNET
        bool allowColResize = true;
#endif
        GridColumnDescriptorCollection _columns;
        GridSummaryRowDescriptorCollection summaryRows;
        GridStackedHeaderRowDescriptorCollection stackedHeaderRows;
        GridColumnSetDescriptorCollection _columnSets;
        GridConditionalFormatDescriptorCollection _conditionalFormats;
        bool inheritAppearanceFomParent = true;
        GridRelationDescriptorCollection _relations;
        internal int summaries_savedColumnsVersion = -1;
        int summaries_savedSummarieRowsVersion = -1;
        int rowsPerRecord = -1;
        int rowsPerRecordColumnSetsVersion = -1;
        GridVisibleColumnDescriptorCollection _visibleColumns;
        GridColumnDescriptor[,] _recordRowColumns = null;
        GridRangeInfo[,] _recordRowCoveredRanges = null;
        ////int cachedMaxColumnSetCols = -1;
        int summaries_savedBaseSummariesVersion = -1;
        private bool isExcelFilterWired = false;
        internal Dictionary<string, string> columnImageCollection = new Dictionary<string, string>();
#if ASPNET
        EditFormSettings m_efsDefaultSettings = new EditFormSettings();
        EditFormSettings m_efsSettings = new EditFormSettings();
#endif
        bool hasSummaryFilterBarChoices = false;
        bool hasCustomSummaryFilterBarChoices = false;
#if !ASPNET
        int frozenCols = -1;
        string frozenColumn = string.Empty;
#endif
        #endregion
        #region Construct
        /// <summary>
        /// Initializes a new object.
        /// </summary>
        public GridTableDescriptor()
        {
            Construct();
        }

        /// <summary>
        /// Initializes a new <see cref="GridTableDescriptor"/> that is a child of
        /// a <see cref="GridRelationDescriptor"/>.
        /// </summary>
        /// <param name="parentRelation">Parent relation.</param>
        public GridTableDescriptor(RelationDescriptor parentRelation)
            : base(parentRelation)
        {
            Construct();
        }

        /// <summary>
        /// Initializes a new <see cref="GridTableDescriptor"/> that is a child of
        /// a <see cref="GridRelationDescriptor"/>.
        /// </summary>
        /// <param name="engine">Grouping engine</param>
        /// <param name="parentRelation">Parent relation.</param>
        public GridTableDescriptor(GridEngine engine, RelationDescriptor parentRelation)
            : base(engine, parentRelation)
        {
            Construct();
        }

        void Construct()
        {
            ////TraceUtil.TraceCurrentMethodInfo();
            ////TraceUtil.TraceCalledFrom(10);

            this.Fields.Changed += new ListPropertyChangedEventHandler(Fields_Changed);
            this.RecordFilters.Changing += new ListPropertyChangedEventHandler(RecordFilters_Changing);
            if (Engine != null)
            {
                this.Engine.DataMemberChanged += new EventHandler(Engine_DataMemberChanged);
                this.Engine.DataSourceChanged += new EventHandler(Engine_DataSourceChanged);
            }
        }
        #endregion
#if ASPNET
        #region Templates
        private ITemplate itemTemplate;
        private ITemplate groupCaptionTemplate;
        private ITemplate editItemTemplate;
        private ITemplate groupItemTemplate;
        private ITemplate headerTemplate;
        private ITemplate footerTemplate;
        private ITemplate rowBtnTemplate;
        private ITemplate expandBtnTemplate;
        private ITemplate statusBarSectionTemplate;
        private ITemplate previewTemplate;
        private ITemplate formModeTemplate;
        private ITemplate inlineFormModeTemplate;
        //private TableEditMode formEditMode = TableEditMode.Normal;
        //Move this above somewhere later
        private string groupDropAreaCssClass=string.Empty;
        
        internal bool AreTemplatesAvailable()
        {
            if(this.itemTemplate != null
                || this.groupCaptionTemplate != null
                || this.editItemTemplate != null
                || this.groupItemTemplate != null
                || this.headerTemplate != null
                || this.footerTemplate != null
                || this.rowBtnTemplate != null
                || this.expandBtnTemplate != null
                || this.statusBarSectionTemplate != null
                || this.previewTemplate != null
                || this.formModeTemplate != null
                || this.inlineFormModeTemplate != null
                )
                return true;
            foreach(GridColumnDescriptor gcd in this.Columns)
                if(gcd.AreTemplatesAvailable())
                    return true;
            foreach(GridSummaryRowDescriptor gsrd in this.SummaryRows)
                if(gsrd.AreTemplatesAvailable())
                    return true;
            return false;
        }
        /// <summary>
        /// Specifies the mode in which records should be edited. Set this to TableEditMode.UseTemplateForm when specifying a FormModeTemplate.
        /// </summary>
        /// <value>Default value is TableEditMode.Normal.</value>
        [
            Description( "Specifies whether records should be edited using a built in form." ),
        NotifyParentProperty(true),
            DefaultValue( TableEditMode.Normal ),
            Obsolete
        ]
        public TableEditMode FormEditMode
        {
            get
            {
                return this.EditFormSettings.FormEditMode;
            }
            set
            {
                this.EditFormSettings.FormEditMode = value;
            }
        }
        /// <summary>
        /// Specifies the template used for displaying a data row within the GroupingGridWebControl control.
        /// The ContainerType of this template is GridCellTemplated.
        /// </summary>
        [Description("Gets / sets the template used for displaying a data row within the GroupingGridWebControl control."),
        Browsable(false),
        DefaultValue(null),
        PersistenceMode(PersistenceMode.InnerProperty),
        TemplateContainer(typeof(GridCellTemplated)),
        XmlIgnore()
        ]
        public virtual ITemplate ItemTemplate
        {
            get { return itemTemplate; }
            set { itemTemplate = value; }
        }

        /// <summary>
        /// Specifies the template used for displaying a group caption.
        /// The ContainerType of this template is GridCellTemplated.
        /// </summary>
        [Description("Gets / sets the template used for displaying a group caption."),
        Browsable(false),
        DefaultValue(null),
        PersistenceMode(PersistenceMode.InnerProperty),
        TemplateContainer(typeof(GridCellTemplated)),
        XmlIgnore()
        ]
        public virtual ITemplate GroupCaptionTemplate
        {
            get { return this.groupCaptionTemplate; }
            set { this.groupCaptionTemplate = value; }
        }

        /// <summary>
        /// Specifies the template used for displaying a data row when in form edit mode. Set FormEditMode to TableEditMode.UseTemplateForm while setting this template.
        /// The ContainerType for this template is GridFormEditCell.
        /// </summary>
        /// <remarks>
        /// This template lets you customize the display of data row when in form edit mode.Set the FormEditMode to TableEditMode and UseTemplateForm while setting this template.
        /// The ContainerType for this template is GridFormEditCell.
        /// </remarks>
        [Description("Gets / sets the template used for displaying a data row when in form edit mode. Set FormEditMode to TableEditMode.UseTemplateForm while setting this template."),
        Browsable(false),
        DefaultValue(null),
        PersistenceMode(PersistenceMode.InnerProperty),
        TemplateContainer(typeof(GridFormEditCell)),
        XmlIgnore()
        ]
            
        public virtual ITemplate FormModeTemplate {get {return formModeTemplate;}set {formModeTemplate = value;}}

        /// <summary>
        /// Specifies the template used for displaying a data row when in form edit mode. Set FormEditMode to TableEditMode.UseInlineTemplateForm while setting this template.
        /// The ContainerType for this template is GridFormEditCell.
        /// </summary>
        /// <remarks>
        /// This template lets you customize the display of data row when in form edit mode. Set the InlineFormEditMode to TableEditMode and UseInlineTemplateForm while setting this template.
        /// The ContainerType for this template is GridFormEditCell.
        /// </remarks>
        [Description( "Gets / sets the template used for displaying a data row when in inline form edit mode. Set FormEditMode to TableEditMode.UseInlineTemplateForm while setting this template." ),
        Browsable( false ),
        DefaultValue( null ),
        PersistenceMode( PersistenceMode.InnerProperty ),
        TemplateContainer( typeof( GridFormEditCell ) ),
        XmlIgnore()
        ]
        public virtual ITemplate InlineFormModeTemplate
        {
            get
            {
                return inlineFormModeTemplate;
            }
            set
            {
                inlineFormModeTemplate = value;
            }
        }

        /// <summary>
        /// Specifies the template used to display the edited item.
        /// The ContainerType of this template is GridCellTemplated.
        /// </summary>
        [Description("Gets / sets the template used to display the edited item."),
        Browsable(false),
        DefaultValue(null),
        PersistenceMode(PersistenceMode.InnerProperty),
        TemplateContainer(typeof(GridCellTemplated)),
        XmlIgnore()
        ]
        public virtual ITemplate EditItemTemplate
        {
            get { return editItemTemplate; }
            set { editItemTemplate = value; }
        }
        /// <summary>
        /// Specifies the template used to display grouped rows of the GroupingGridWebControl control.
        /// The ContainerType of this template is GridCellTemplated.
        /// </summary>
        [Description("Gets / sets the template used to display grouped rows of the GroupingGridWebControl control."),
        Browsable(false),
        DefaultValue(null),
        PersistenceMode(PersistenceMode.InnerProperty),
        TemplateContainer(typeof(GridCellTemplated)),
        XmlIgnore()
        ]
        public virtual ITemplate GroupItemTemplate
        {
            get { return groupItemTemplate; }
            set { groupItemTemplate = value; }
        }
        /// <summary>
        /// Specifies the template used to display the header section of the GroupingGridWebControl contol.
        /// The ContainerType of this template is GridCellTemplated.
        /// </summary>
        [Description("Gets / sets the template used to display the header section of the GroupingGridWebControl contol."),
        Browsable(false),
        DefaultValue(null),
        PersistenceMode(PersistenceMode.InnerProperty),
        TemplateContainer(typeof(GridCellTemplated)),
        XmlIgnore()
        ]
        public virtual ITemplate HeaderTemplate
        {
            get { return headerTemplate; }
            set { headerTemplate = value; }
        }
        /// <summary>
        /// Specifies the template used for displaying the footer section of the GroupingGridWebControl control.
        /// The ContainerType of this template is GridCellTemplated.
        /// </summary>
        [Description("Gets / sets the template used for displaying the footer section of the GroupingGridWebControl control."),
        Browsable(false),
        DefaultValue(null),
        PersistenceMode(PersistenceMode.InnerProperty),
        TemplateContainer(typeof(GridCellTemplated)),
        XmlIgnore()
        ]
        public virtual ITemplate FooterTemplate
        {
            get { return footerTemplate; }
            set { footerTemplate = value; }
        }
        /// <summary>
        /// Specifies the template used for rendering buttons displayed within RowBtnColumn columns and the Indicator panel.
        /// The ContainerType of this template is GridCellTemplated.
        /// </summary>
        [Description("Gets / sets the template used for rendering buttons displayed within RowBtnColumn columns and the Indicator panel."),
        Browsable(false),
        DefaultValue(null),
        PersistenceMode(PersistenceMode.InnerProperty),
        TemplateContainer(typeof(GridCellTemplated)),
        XmlIgnore()
        ]
        public virtual ITemplate RowBtnTemplate
        {
            get { return rowBtnTemplate; }
            set { rowBtnTemplate = value; }
        }
        /// <summary>
        /// Specifies the template used for rendering buttons that expand / collapse group rows within the GroupingGridWebControl control.
        /// The ContainerType of this template is GridCellTemplated.
        /// </summary>
        [Description("Gets / sets the template used for rendering buttons that expand / collapse group rows within the GroupingGridWebControl control."),
        Browsable(false),
        DefaultValue(null),
        PersistenceMode(PersistenceMode.InnerProperty),
        TemplateContainer(typeof(GridCellTemplated)),
        XmlIgnore()
        ]
        public virtual ITemplate ExpandBtnTemplate
        {
            get { return expandBtnTemplate; }
            set { expandBtnTemplate = value; }
        }
        /// <summary>
        /// Specifies the template used for rendering the Status Bar section within the GroupingGridWebControl control.
        /// The ContainerType of this template is GridCellTemplated.
        /// </summary>
        [Description("Gets / sets the template used for rendering the Status Bar section within the GroupingGridWebControl control."),
        Browsable(false),
        DefaultValue(null),
        PersistenceMode(PersistenceMode.InnerProperty),
        TemplateContainer(typeof(GridCellTemplated)),
        XmlIgnore()
        ]
        public virtual ITemplate StatusBarSectionTemplate
        {
            get { return statusBarSectionTemplate; }
            set { statusBarSectionTemplate = value; }
        }
        /// <summary>
        /// Determines the template used to display Preview sections in the GroupingGridWebControl control.
        /// The ContainerType of this template is GridCellTemplated.
        /// </summary>
        [Description("Determines the template used to display Preview sections in the GroupingGridWebControl control."),
        Browsable(false),
        DefaultValue(null),
        PersistenceMode(PersistenceMode.InnerProperty),
        TemplateContainer(typeof(GridCellTemplated)),
        XmlIgnore()
        ]
        public virtual ITemplate PreviewTemplate
        {
            get { return previewTemplate; }
            set { previewTemplate = value; }
        }

        #endregion
#endif
        /// <override/>
        protected override void Dispose(bool disposing)
        {
            ////TraceUtil.TraceCurrentMethodInfo();
            ////TraceUtil.TraceCalledFrom(10);

            if (disposing)
            {
                if (summaryRows != null)
                {
                    summaryRows.Changed -= new ListPropertyChangedEventHandler(summaryRows_Changed);
                    summaryRows.Changing -= new ListPropertyChangedEventHandler(summaryRows_Changing);
                    summaryRows.Dispose();
                    ////summaryRows = null;
                }

                if (stackedHeaderRows != null)
                {
                    stackedHeaderRows.Changed -= new ListPropertyChangedEventHandler(stackedHeaderRows_Changed);
                    stackedHeaderRows.Changing -= new ListPropertyChangedEventHandler(stackedHeaderRows_Changing);
                    stackedHeaderRows.Dispose();
                    ////stackedHeaderRows = null;
                }

                if (_visibleColumns != null)
                {
                    _visibleColumns.Changed -= new Syncfusion.Collections.ListPropertyChangedEventHandler(_visibleColumns_Changed);
                    _visibleColumns.Changing -= new Syncfusion.Collections.ListPropertyChangedEventHandler(_visibleColumns_Changing);
                    _visibleColumns.Dispose();
                    ////_visibleColumns = null;
                }

                if (_columns != null)
                {
                    _columns.Changed -= new ListPropertyChangedEventHandler(_columns_Changed);
                    _columns.Changing -= new ListPropertyChangedEventHandler(_columns_Changing);
                    _columns.Dispose();
                    ////_columns = null;
                }

                if (_columnSets != null)
                {
                    _columnSets.Changed -= new Syncfusion.Collections.ListPropertyChangedEventHandler(_columnSets_Changed);
                    _columnSets.Changing -= new ListPropertyChangedEventHandler(_columnSets_Changing);
                    _columnSets.Dispose();
                    ////_columnSets = null;
                }

                if (_conditionalFormats != null)
                {
                    _conditionalFormats.Changed -= new Syncfusion.Collections.ListPropertyChangedEventHandler(_conditionalFormats_Changed);
                    _conditionalFormats.Changing -= new ListPropertyChangedEventHandler(_conditionalFormats_Changing);
                    _conditionalFormats.Dispose();
                    ////_conditionalFormats = null;
                }

                if (appearance != null)
                {
                    this.appearance.Dispose();
                }
                ////appearance = null;

                if (groupOptions != null)
                {
                    this.groupOptions.Dispose();
                }
                ////groupOptions = null;

                if (topLevelGroupOptions != null)
                {
                    this.topLevelGroupOptions.Dispose();
                }
                ////topLevelGroupOptions = null;

                if (tableOptions != null)
                {
                    this.tableOptions.Dispose();
                }
                ////tableOptions = null;

                if (groupOptions != null)
                {               
                    this.groupOptions.Dispose();
                }
                ////groupOptions = null;

                this.Fields.Changed -= new ListPropertyChangedEventHandler(Fields_Changed);
                this.RecordFilters.Changing -= new ListPropertyChangedEventHandler(RecordFilters_Changing);
                if (Engine != null)
                {
                    this.Engine.DataMemberChanged -= new EventHandler(Engine_DataMemberChanged);
                    this.Engine.DataSourceChanged -= new EventHandler(Engine_DataSourceChanged);
                }

                if (_relations != null)
                {
                    this._relations.Dispose();
                }
                ////this._relations = null;

                this._recordRowColumns = null;
                this._recordRowCoveredRanges = null;

                if (sortByDisplayMemberCols != null)
                {
                    sortByDisplayMemberCols.Clear();
                    sortByDisplayMemberCols = null;
                }
            }

            base.Dispose(disposing);
        }

        /// <summary>
        /// Performs searching in all the columns.
        /// </summary>
        /// <param name="text">Text to be searched</param>
        public void Search(string text)
        {
            List<GridColumnDescriptor> list = new List<GridColumnDescriptor>();
            foreach (GridColumnDescriptor cols in this.Columns)
                list.Add(cols);
            Search(text, list);
        }

        /// <summary>
        /// Performs searching in User-Specified columns.
        /// </summary>
        /// <param name="list">Columns where the text has to be searched</param>
        /// <param name="text">Text to be searched</param>
        public void Search(string text, List<GridColumnDescriptor> list)
        {
            string filterString = null;
            this.RecordFilters.Clear();

            foreach (GridColumnDescriptor visible_col in list)
                filterString += string.Format("[{0}] match '{1}' or ", visible_col.Name, text);

            filterString = filterString.Substring(0, filterString.Length - 3);
            RecordFilterDescriptor rfd = new RecordFilterDescriptor(filterString);
            this.RecordFilters.Add(rfd);             
        }

       
        #region Parents
        /// <summary>
        /// Gets the <see cref="GridEngine"/> that this table descriptor belongs to.
        /// </summary>
        [XmlIgnore]
        [DesignerSerializationVisibilityAttribute(DesignerSerializationVisibility.Hidden)]
        [Browsable(false)]
        public new GridEngine Engine
        {
            get
            {
                return (GridEngine)base.Engine;
            }
        }

        /// <summary>
        /// Gets the <see cref="GridRelationDescriptor"/> that this table descriptor belongs to
        /// if it is a child table of a relation; returns NULL if it is the main
        /// table descriptor.
        /// </summary>
        [XmlIgnore]
        [DesignerSerializationVisibilityAttribute(DesignerSerializationVisibility.Hidden)]
        [Browsable(false)]
        public new GridRelationDescriptor ParentRelation
        {
            get
            {
                return (GridRelationDescriptor)base.ParentRelation;
            }
        }

        /// <summary>
        /// Gets the <see cref="GridTableDescriptor"/> that the <see cref="ParentRelation"/> belongs to
        /// if this table descriptor is a child table of a relation; returns NULL if it is the main
        /// table descriptor.
        /// </summary>
        [XmlIgnore]
        [DesignerSerializationVisibilityAttribute(DesignerSerializationVisibility.Hidden)]
        [Browsable(false)]
        public new GridTableDescriptor ParentTableDescriptor
        {
            get
            {
                return (GridTableDescriptor)base.ParentTableDescriptor;
            }
        }

        #endregion
        #region InitializeFrom, Modified, Reset methods
        /// <override/>
        /// <summary>Initializes this object and copies properties from another object.</summary>
        /// <param name="tableDescriptor">The source object.</param>
        public override void InitializeFrom(TableDescriptor tableDescriptor)
        {
            if (tableDescriptor is GridTableDescriptor)
            {
                GridTableDescriptor other = (GridTableDescriptor)tableDescriptor;

                this.AllowEdit = other.AllowEdit;
                this.AllowNew = other.AllowNew;
                this.AllowRemove = other.AllowRemove;

                if (other.ShouldSerializeRelations())
                {
                    base.Relations.InitializeFrom(((TableDescriptor)other).Relations);
                }
                else
                {
                    ResetRelations();
                }

                if (other.ForceEmptyRelations)
                {
                    Relations.Clear();
                }

                if (other.ShouldSerializeAppearance())
                {
                    Appearance.InitializeFrom(other.Appearance);
                }
                else
                {
                    ResetAppearance();
                }

                if (other.ShouldSerializeUnboundFields())
                {
                    UnboundFields.InitializeFrom(other.UnboundFields);
                }
                else
                {
                    ResetUnboundFields();
                }

                if (other.ShouldSerializeFields())
                {
                    Fields.InitializeFrom(other.Fields);
                }
                else
                {
                    ResetFields();
                }

                if (other.ShouldSerializeExpressionFields())
                {
                    ExpressionFields.InitializeFrom(other.ExpressionFields);
                }
                else
                {
                    ResetExpressionFields();
                }

                if (other.ShouldSerializeColumns())
                {
                    Columns.InitializeFrom(other.Columns);
                }
                else
                {
                    ResetColumns();
                }

                if (other.ForceEmptyColumns)
                {
                    Columns.Clear();
                }

                if (other.ShouldSerializeColumnSets())
                {
                    ColumnSets.InitializeFrom(other.ColumnSets);
                }
                else
                {
                    ResetColumnSets();
                }

                if (other.ShouldSerializeVisibleColumns())
                {
                    VisibleColumns.InitializeFrom(other.VisibleColumns);
                }
                else
                {
                    ResetVisibleColumns();
                }

                if (other.ForceEmptyVisibleColumns)
                {
                    VisibleColumns.Clear();
                }

                if (other.ShouldSerializeConditionalFormats())
                {
                    ConditionalFormats.InitializeFrom(other.ConditionalFormats);
                }
                else
                {
                    ResetConditionalFormats();
                }

                if (other.ShouldSerializeGroupedColumns())
                {
                    GroupedColumns.InitializeFrom(other.GroupedColumns);
                }
                else
                {
                    ResetGroupedColumns();
                }

                if (other.ShouldSerializeChildGroupOptions())
                {
                    ChildGroupOptions.CopyFrom(other.ChildGroupOptions);
                }
                else
                {
                    ResetChildGroupOptions();
                }

                if (other.ShouldSerializeTopLevelGroupOptions())
                {
                    TopLevelGroupOptions.CopyFrom(other.TopLevelGroupOptions);
                }
                else
                {
                    ResetTopLevelGroupOptions();
                }

                if (other.ShouldSerializeTableOptions())
                {
                    TableOptions.CopyFrom(other.TableOptions);
                }
                else
                {                
                    ResetTableOptions();
                }

                if (other.ShouldSerializeName())
                {
                    Name = other.Name;
                }
                else
                {
                    ResetName();
                }

                if (other.ShouldSerializeRecordFilters())
                {
                    RecordFilters.InitializeFrom(other.RecordFilters);
                }
                else
                {
                    ResetRecordFilters();
                }

                if (other.ShouldSerializeSortedColumns())
                {
                    SortedColumns.InitializeFrom(other.SortedColumns);
                }
                else
                {
                    ResetSortedColumns();
                }

                if (other.ShouldSerializeSummaryRows())
                {
                    SummaryRows.InitializeFrom(other.SummaryRows);
                }
                else
                {
                    ResetSummaryRows();
                }

                if (other.ShouldSerializeStackedHeaderRows())
                {
                    StackedHeaderRows.InitializeFrom(other.StackedHeaderRows);
                }
                else
                {
                    ResetStackedHeaderRows();
                }

                if (other.ShouldSerializeGroupedColumns())
                {
                    GroupedColumns.InitializeFrom(other.GroupedColumns);
                }
                else
                {
                    ResetGroupedColumns();
                }

                if (other.ShouldSerializePrimaryKeyColumns())
                {
                    PrimaryKeyColumns.InitializeFrom(other.PrimaryKeyColumns);
                }
                else
                {
                    ResetPrimaryKeyColumns();
                }

                if (other.ShouldSerializeRelationChildColumns())
                {
                    RelationChildColumns.InitializeFrom(other.RelationChildColumns);
                }
                else
                {
                    ResetRelationChildColumns();
                }
#if ASPNET
                if( other.ShouldSerializeEditFormSettings() )
                {
                    EditFormSettings.FormEditMode = other.EditFormSettings.FormEditMode;
                    EditFormSettings.CaptionDataField = other.EditFormSettings.CaptionDataField;
                    EditFormSettings.CaptionFormatString = other.EditFormSettings.CaptionFormatString;
                    EditFormSettings.ColumnNumber = other.EditFormSettings.ColumnNumber;
                }
                else
                {
                    ResetEditFormSettings();
                }

                this.allowColResize = other.AllowColumnResize;
                this.GroupDropAreaCssClass=other.GroupDropAreaCssClass;
                this.EditFormSettings.FormEditMode = other.EditFormSettings.FormEditMode;
                this.FormEditMode = other.FormEditMode;
                if(other.ItemTemplate!=null)
                {
                     this.ItemTemplate =other.ItemTemplate;
                }

                if(other.GroupCaptionTemplate!=null)
                {
                    this.GroupCaptionTemplate=other.GroupCaptionTemplate;
                }

                if(other.EditItemTemplate!=null)
                {
                     this.EditItemTemplate=other.EditItemTemplate;
                }

                if(other.HeaderTemplate!=null)
                {
                    this.HeaderTemplate=other.HeaderTemplate;
                }

                if(other.FooterTemplate!=null)
               {
                    this.FooterTemplate=other.FooterTemplate;
                }

                if(other.RowBtnTemplate!=null)
                {
                    this.RowBtnTemplate=other.RowBtnTemplate;
                }

                if(other.ExpandBtnTemplate!=null)
                {
                    this.ExpandBtnTemplate=other.ExpandBtnTemplate;
                }

                if(other.StatusBarSectionTemplate!=null)
                {
                    this.StatusBarSectionTemplate=other.StatusBarSectionTemplate;
                }

                if(other.PreviewTemplate!=null)
                {
                    this.PreviewTemplate=other.PreviewTemplate;
                }
                   
#else
                this.FrozenColumn = other.FrozenColumn;
#endif
            }
            else
            {
                base.InitializeFrom(tableDescriptor);
            }
            //// Summaries, RelationChildColumns are not initialized automatically on demand.

            InitializePropertyDescriptors();
        }

        /// <override/>
        /// <summary>Determines if the summaries were modified.</summary>
        /// <returns>returns False.</returns>
        public override bool ShouldSerializeSummaries()
        {
            return false;
        }

        /// <override/>
        /// <summary>Discards any changes for the <see cref="TableDescriptor"/> object.</summary>
        public override void ResetTableDescriptor()
        {
            this.OnPropertyChanging(new DescriptorPropertyChangedEventArgs("ResetTableDescriptor"));
#if ASPNET
            AllowColumnResize = true;
            //EditFormSettings.FormEditMode = TableEditMode.Normal;
            FormEditMode = TableEditMode.Normal;
#endif
            AllowEdit = true;
            AllowNew = true;
            AllowRemove = true;
            ResetAppearance();
#if ASPNET
            ResetEditFormSettings();
#endif
            ResetColumnSets();
            ResetConditionalFormats();
            ResetExpressionFields();
            ////ResetFields();
            ResetGroupedColumns();
            ResetPrimaryKeyColumns();
            ResetRelationChildColumns();
            ResetTableOptions();
            ResetTopLevelGroupOptions();
            ResetChildGroupOptions();
            ResetName();
            ResetRecordFilters();
            ResetRelations();
            ResetSortedColumns();
            ResetSummaries();
            ResetSummaryRows();
            ResetStackedHeaderRows();
            ResetVisibleColumns();
            ResetUnboundFields();
            ResetColumns();
            InitializePropertyDescriptors();
            this.OnPropertyChanged(new DescriptorPropertyChangedEventArgs("ResetTableDescriptor"));
        }

        /// <override/>
        /// <summary>Determines if any property has been modified.</summary>
        /// <returns>True if modified.</returns>
        public override bool GetModified()
        {
            return
#if ASPNET
                !AllowColumnResize ||
                this.FormEditMode != TableEditMode.Normal ||
                this.EditFormSettings.FormEditMode != TableEditMode.Normal ||
                this.ItemTemplate != null ||
                this.GroupCaptionTemplate != null ||
                this.FormModeTemplate != null ||
                this.inlineFormModeTemplate != null ||
                this.EditItemTemplate != null ||
                this.GroupItemTemplate != null ||
                this.HeaderTemplate != null ||
                this.FooterTemplate != null ||
                this.RowBtnTemplate != null ||
                this.ExpandBtnTemplate != null ||
                this.StatusBarSectionTemplate != null ||
                this.PreviewTemplate != null ||
#endif
 !AllowEdit ||
                !AllowNew ||
                !AllowRemove ||
                ShouldSerializeAppearance() ||
                ShouldSerializeColumns() ||
                ShouldSerializePrimaryKeyColumns() ||
                ////This will always return true - even if it was auto populated, so we don't check it now.
                ////ShouldSerializeRelationChildColumns() ||
                ShouldSerializeConditionalFormats() ||
                ShouldSerializeColumnSets() ||
                ShouldSerializeExpressionFields() ||
                ////ShouldSerializeFields() ||
                ShouldSerializeGroupedColumns() ||
                ShouldSerializeTableOptions() ||
                ShouldSerializeChildGroupOptions() ||
                ShouldSerializeTopLevelGroupOptions() ||
                ShouldSerializeName() ||
                ShouldSerializeRecordFilters() ||
                ShouldSerializeRelations() ||
                ShouldSerializeSortedColumns() ||
                ////ShouldSerializeSummaries() ||
                ShouldSerializeSummaryRows() ||
                ShouldSerializeStackedHeaderRows() ||
                ShouldSerializeUnboundFields() ||
#if ASPNET
                ShouldSerializeEditFormSettings() ||
#endif
 ShouldSerializeVisibleColumns();
        }

        ////        /// <override/>
        ////        public override string ToString()
        ////        {
        ////            return Name; // only show Name in PropertyGrid
        ////        }

        #endregion
        #region Strong Typed Relations collection
#if ASPNET
        
#if SyncfusionFramework2_0
        public override bool CheckAllProperties
        {
            get { return false; }
        }
#endif
        /// <summary>
        /// Specifies the CSS class of grid grouping control's GroupDropArea.
        /// </summary>
        /// <example>
        /// [c#]
        ///this.GridGroupingControl1.TableDescriptor.GroupDropAreaCssClass="GroupDropAreaCss";
        ///this.GridGroupingControl1.TableDescriptor.GridRelations[0].GridChildTableDescriptor.GroupDropAreaCssClass="ChildGroupDropAreaCss";
        /// //For setting the GroupDropAreaCss class for the GrandChildTableDescriptor.
        /// this.GridGroupingControl1.TableDescriptor.GridRelations[0].GridChildTableDescriptor.GridRelations[0].GridChildTableDescriptor.GroupDropAreaCssClass="GrandChildGroupDropAreaCss";
        ///[vb]
        ///Me.GridGroupingControl1.TableDescriptor.GroupDropAreaCssClass="GroupDropAreaCss";
        ///Me.GridGroupingControl1.TableDescriptor.GridRelations[0].GridChildTableDescriptor.GroupDropAreaCssClass="ChildGroupDropAreaCss";
        /// 'For setting the GroupDropAreaCss class for the GrandChildTableDescriptor.
        ///Me.GridGroupingControl1.TableDescriptor.GridRelations[0].GridChildTableDescriptor.GridRelations[0].GridChildTableDescriptor.GroupDropAreaCssClass="GrandChildGroupDropAreaCss";
        /// </example>
        [DefaultValue(string.Empty),
        Description("Specifies the CSS class of grid grouping control's GroupDropArea."),
        NotifyParentProperty(true)
        ]
        public string GroupDropAreaCssClass
        {
            get
            {
                return this.groupDropAreaCssClass;                
            }

            set
            {
                this.groupDropAreaCssClass=value;
            }
        }

        [DefaultValue(true),
        Description("Specifies whether the end user is allowed to resize the column width."),
        NotifyParentProperty(true)]
        public bool AllowColumnResize
        {
            get{return this.allowColResize;}
            set{this.allowColResize = value;}
        }

        #region RecordFilters
        // Will hide the base property in the designer and also xml serialize here.
        // Need to xml-ser here since the base class prop. will be serialized anyway.
        [Browsable(false)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        [XmlArrayItem(typeof(RecordFilterDescriptor)), XmlArrayItem(typeof(GridRecordFilterDescriptor))]
        [NotifyParentProperty(true)]
        public new RecordFilterDescriptorCollection RecordFilters
        {
            get    {return base.RecordFilters;}
            set    {base.RecordFilters = value;}
        }

        private GridRecordFilterDescriptorCollection _recordFilters;

        /// <summary>
        /// Gets the collection of <see cref="RecordFilterDescriptor"/> objects defining filter criteria
        /// for records in the table. Each <see cref="RecordFilterDescriptor"/> in the collection references
        /// one or multiple <see cref="FieldDescriptor"/> of the <see cref="Fields"/> collection. Multiple
        /// criteria can be combined with logical "And" or "Or" operations.
        /// </summary>
        [RefreshProperties(RefreshProperties.All)]
        [DesignerSerializationVisibilityAttribute(DesignerSerializationVisibility.Content)]
        [ListBindableAttribute(false)]
        [EditorAttribute(typeof(GroupingCollectionEditor), typeof(System.Drawing.Design.UITypeEditor))]
        [PersistenceMode(PersistenceMode.InnerProperty)]
        [XmlIgnore()]
        [NotifyParentProperty(true)]
        [Description("Gets the collection of RecordFilterDescriptor objects defining filter criteria for records in the table."),
        Category("TableDescriptor")]
        public GridRecordFilterDescriptorCollection GridRecordFilters
        {
            get
            {
                if (_recordFilters == null)
                {
                    _recordFilters = new GridRecordFilterDescriptorCollection(base.RecordFilters);
                }

                return _recordFilters;
            }
        }

        /// <summary>
        /// Determines if the <see cref="Relations"/> collection or child objects have been modified from its
        /// default state.
        /// </summary>
        /// <returns></returns>
        public bool ShouldSerializeGridRecordFilters()
        {
            return base.ShouldSerializeRecordFilters();
        }

        /// <summary>
        /// Resets the <see cref="ExpressionFields"/> collection back to its
        /// default state.
        /// </summary>
        public void ResetGridRecordFilters()
        {
            if (this.ShouldSerializeGridRecordFilters())
            {
                if (Engine != null)
                {
                    IComponentChangeService changeService = null;
                    if (this.Engine.ParentControl != null && this.Engine.ParentControl.Site != null)
                      {
                        changeService  = this.Engine.ParentControl.Site.GetService(typeof(IComponentChangeService)) as IComponentChangeService;
                      }

                    if(changeService != null)
                    {
                        MemberDescriptor md = TypeDescriptor.GetProperties(this)["GridRecordFilters"];
                        changeService.OnComponentChanging(this, md);
                        base.ResetRecordFilters();
                        Engine.Table.TableDirty = true;
                        changeService.OnComponentChanged(this, md, null, null);
                        return;
                    }
                }
            }

            base.ResetRecordFilters();
            if (Engine != null)
            {
                if (Engine.ParentControl != null)
                {
                    Engine.ParentControl.SuspendLayout();
                    Engine.TableDescriptor.ResetItemProperties();
                    Engine.Table.TableDirty = true;
                    Engine.ParentControl.ResumeLayout();
                }
            }
        }

        #endregion

        #region UnboundFields
        // This property will be used for XML Serialization and the one below user-browsable.
        [Browsable(false)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        [XmlArrayItem(typeof(FieldDescriptor)), XmlArrayItem(typeof(GridUnboundFieldDescriptor))]
        [NotifyParentProperty(true)]
        public new UnboundFieldDescriptorCollection UnboundFields
        {
            get{return base.UnboundFields;}
            set{base.UnboundFields = value;}
        }

        private GridUnboundFieldDescriptorCollection _unboundFields;
        /// <summary>
        /// Gets the collection of <see cref="GridExpressionFieldDescriptorCollection"/> objects defining expression fields that
        /// represent values for each row in the table. Expression fields can reference other fields and
        /// support arithmetic calculatations and boolean expressions.
        /// </summary>
        [RefreshProperties(RefreshProperties.All)]
        [DesignerSerializationVisibilityAttribute(DesignerSerializationVisibility.Content)]
        [ListBindableAttribute(false)]
        [EditorAttribute(typeof(GroupingCollectionEditor), typeof(System.Drawing.Design.UITypeEditor))]
        [XmlIgnore()]
        [PersistenceMode(PersistenceMode.InnerProperty)]
        [NotifyParentProperty(true)]
        [Description("Gets the collection of GridExpressionFieldDescriptorCollection objects defining expression fields that represent values for each row in the table."),
        Category("TableDescriptors")]
        public GridUnboundFieldDescriptorCollection GridUnboundFields
        {
            get
            {
                if (_unboundFields == null)
                   {
                    _unboundFields = new GridUnboundFieldDescriptorCollection(base.UnboundFields);
                   }

                return _unboundFields;
            }
        }

        /// <summary>
        /// Determines if the <see cref="UnboundFields"/> collection or child objects have been modified from its
        /// default state.
        /// </summary>
        /// <returns></returns>
        public bool ShouldSerializeGridUnboundFields()
        {
            return base.ShouldSerializeUnboundFields();
        }

        /// <summary>
        /// Resets the <see cref="UnboundFields"/> collection back to its
        /// default state.
        /// </summary>
        public void ResetUnboundGridFields()
        {
            if (this.ShouldSerializeGridUnboundFields())
            {
                if (Engine != null)
                {
                    IComponentChangeService changeService = null;
                    if (this.Engine.ParentControl != null && this.Engine.ParentControl.Site != null)
                     {
                       changeService  = this.Engine.ParentControl.Site.GetService(typeof(IComponentChangeService)) as IComponentChangeService;
                     }

                    if(changeService != null)
                    {
                        MemberDescriptor md = TypeDescriptor.GetProperties(this)["UnboundFields"];
                        changeService.OnComponentChanging(this, md);
                        base.ResetUnboundFields();
                        Engine.Table.TableDirty = true;
                        changeService.OnComponentChanged(this, md, null, null);
                        return;
                    }
                }
            }

            base.ResetUnboundFields();
            if (Engine != null)
            {
                if (Engine.ParentControl != null)
                {
                    Engine.ParentControl.SuspendLayout();
                    Engine.TableDescriptor.ResetItemProperties();
                    Engine.Table.TableDirty = true;
                    Engine.ParentControl.ResumeLayout();
                }
            }
        }
        #endregion

        #region GroupedColumns
        // Here we hide the base class property
        // and also use this for xml serialization.
        [Browsable(false)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        [XmlArrayItem(typeof(SortColumnDescriptor)), XmlArrayItem(typeof(GridSortColumnDescriptor))]
        [NotifyParentProperty(true)]
        public new SortColumnDescriptorCollection GroupedColumns
        {
            get{return base.GroupedColumns;}
            set{base.GroupedColumns = value;}
        }

        private GridSortColumnDescriptorCollection _groupedColumns;

        /// <summary>
        /// Gets the collection of <see cref="GridSortColumnDescriptorCollection"/> objects defining group by
        /// state of the table. Each <see cref="GridSortColumnDescriptor"/> in the collection references
        /// a <see cref="FieldDescriptor"/> of the <see cref="Fields"/> collection.
        /// </summary>
        [RefreshProperties(RefreshProperties.All)]
        [DesignerSerializationVisibilityAttribute(DesignerSerializationVisibility.Content)]
        [ListBindableAttribute(false)]
        [EditorAttribute(typeof(GroupingCollectionEditor), typeof(System.Drawing.Design.UITypeEditor))]
        [XmlIgnore()]
        [PersistenceMode(PersistenceMode.InnerProperty)]
        [NotifyParentProperty(true)]
        [Description("Gets the collection of GridSortColumnDescriptorCollection objects defining group by state of the table."),
        Category("TableDescriptors")]
        public GridSortColumnDescriptorCollection GridGroupedColumns
        {
            get
            {
                if (_groupedColumns == null)
                  {
                    _groupedColumns = new GridSortColumnDescriptorCollection(base.GroupedColumns);
                   }
        
                return _groupedColumns;
            }
        }

        /// <summary>
        /// Determines if the <see cref="GroupedColumns"/> collection contains values.
        /// </summary>
        /// <returns></returns>
        public bool ShouldSerializeGridGroupedColumns()
        {
            return base.ShouldSerializeGroupedColumns();
        }

        /// <summary>
        /// Resets the <see cref="GroupedColumns"/> collection back to its
        /// default state.
        /// </summary>
        public void ResetGroupedGridColumns()
        {
            if (this.ShouldSerializeGridGroupedColumns())
            {
                if (Engine != null)
                {
                    IComponentChangeService changeService = null;
                    if (this.Engine.ParentControl != null && this.Engine.ParentControl.Site != null)
                      {
                        changeService  = this.Engine.ParentControl.Site.GetService(typeof(IComponentChangeService)) as IComponentChangeService;
                      }

                    if(changeService != null)
                    {
                        MemberDescriptor md = TypeDescriptor.GetProperties(this)["GridGroupedColumns"];
                        changeService.OnComponentChanging(this, md);
                        base.ResetGroupedColumns();
                        Engine.Table.TableDirty = true;
                        changeService.OnComponentChanged(this, md, null, null);
                        return;
                    }
                }
            }

            base.ResetGroupedColumns();
            if (Engine != null)
            {
                if (Engine.ParentControl != null)
                {
                    Engine.ParentControl.SuspendLayout();
                    Engine.TableDescriptor.ResetItemProperties();
                    Engine.Table.TableDirty = true;
                    Engine.ParentControl.ResumeLayout();
                }
            }
        }
        #endregion

        #region SortedColumns
        // This property will be used for XML Serialization and the one below user-browsable.
        [Browsable(false)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        [XmlArrayItem(typeof(SortColumnDescriptor)), XmlArrayItem(typeof(GridSortColumnDescriptor))]
        [NotifyParentProperty(true)]
        public new SortColumnDescriptorCollection SortedColumns
        {
            get{return base.SortedColumns;}
            set{base.SortedColumns = value;}
        }

        private GridSortColumnDescriptorCollection _sortedColumns;
        /// <summary>
        /// Gets the collection of <see cref="GridExpressionFieldDescriptorCollection"/> objects defining expression fields that
        /// represent values for each row in the table. Expression fields can reference other fields and
        /// support arithmetic calculatations and boolean expressions.
        /// </summary>
        [RefreshProperties(RefreshProperties.All)]
        [DesignerSerializationVisibilityAttribute(DesignerSerializationVisibility.Content)]
        [ListBindableAttribute(false)]
        [EditorAttribute(typeof(GroupingCollectionEditor), typeof(System.Drawing.Design.UITypeEditor))]
        [XmlIgnore()]
        [PersistenceMode(PersistenceMode.InnerProperty)]
        [NotifyParentProperty(true)]
        [Description("Gets the collection of GridExpressionFieldDescriptorCollection objects defining expression fields that represent values for each row in the table."),
        Category("TableDescriptors")]
        public GridSortColumnDescriptorCollection GridSortedColumns
        {
            get
            {
                if (_sortedColumns == null)
                    {
                     _sortedColumns = new GridSortColumnDescriptorCollection(base.SortedColumns);
                    }

                return _sortedColumns;
            }
        }

        /// <summary>
        /// Determines if the <see cref="Relations"/> collection or child objects have been modified from its
        /// default state.
        /// </summary>
        /// <returns></returns>
        public bool ShouldSerializeGridSortedColumns()
        {
            return base.ShouldSerializeSortedColumns();
        }

        /// <summary>
        /// Resets the <see cref="SortedColumns"/> collection back to its
        /// default state.
        /// </summary>
        public void ResetGridSortedColumns()
        {
            if (this.ShouldSerializeGridSortedColumns())
            {
                if (Engine != null)
                {
                    IComponentChangeService changeService = null;
                    if (this.Engine.ParentControl != null && this.Engine.ParentControl.Site != null)
                       {
                         changeService  = this.Engine.ParentControl.Site.GetService(typeof(IComponentChangeService)) as IComponentChangeService;
                       }

                    if(changeService != null)
                    {
                        MemberDescriptor md = TypeDescriptor.GetProperties(this)["GridSortedColumns"];
                        changeService.OnComponentChanging(this, md);
                        base.ResetSortedColumns();
                        Engine.Table.TableDirty = true;
                        changeService.OnComponentChanged(this, md, null, null);
                        return;
                    }
                }
            }

            base.ResetSortedColumns();
            if (Engine != null)
            {
                if (Engine.ParentControl != null)
                {
                    Engine.ParentControl.SuspendLayout();
                    Engine.TableDescriptor.ResetItemProperties();
                    Engine.Table.TableDirty = true;
                    Engine.ParentControl.ResumeLayout();
                }
            }
        }
        #endregion

        #region ExpressionFields
        // This property will be used for XML Serialization and the one below user-browsable.
        /// <summary>
        /// Use <see cref="GridExpressionFields"/> instead.
        /// </summary>
        [Browsable(false)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        [XmlArrayItem(typeof(ExpressionFieldDescriptor)), XmlArrayItem(typeof(GridExpressionFieldDescriptor))]
        public new ExpressionFieldDescriptorCollection ExpressionFields
        {
            get{return base.ExpressionFields;}
            set{base.ExpressionFields = value;}
        }

        private GridExpressionFieldDescriptorCollection _expressions;
        /// <summary>
        /// Gets the collection of <see cref="GridExpressionFieldDescriptorCollection"/> objects defining expression fields that
        /// represent values for each row in the table. Expression fields can reference other fields and
        /// support arithmetic calculatations and boolean expressions.
        /// </summary>
        /// <remarks>
        /// <example>
        /// This sample shows how to create a new GridExpressionFieldDescriptor for a new field based on 2 other field values:
        /// <code lang="C#">
        /// this.GridGroupingControl1.TableDescriptor.GridExpressionFields.Add(new GridExpressionFieldDescriptor("NewField", "[Field1] - [Field2]", typeof(float)));
        /// </code>
        /// <code lang="VB">
        /// Me.GridGroupingControl1.TableDescriptor.GridExpressionFields.Add(New GridExpressionFieldDescriptor("NewField", "[Field1] - [Field2]", GetType(Single)))
        /// </code>
        /// The above can also be declared in aspx as follows:
        /// <code>
        /// <TableDescriptor>
        ///     .....
        ///     <GridExpressionFields>
        ///           <sfwg:GridExpressionFieldDescriptor Name="NewField" Expression="[Field1] - [Field2]" />
        ///     </GridExpressionFields>
        /// </TableDescriptor>
        /// </code>
        /// </example>
        /// </remarks>
        [RefreshProperties(RefreshProperties.All)]
        [DesignerSerializationVisibilityAttribute(DesignerSerializationVisibility.Content)]
        [ListBindableAttribute(false)]
        [EditorAttribute(typeof(GroupingCollectionEditor), typeof(System.Drawing.Design.UITypeEditor))]
        [XmlIgnore()]
        [PersistenceMode(PersistenceMode.InnerProperty)]
        [NotifyParentProperty(true)]
        [Description("Gets the collection of GridExpressionFieldDescriptor objects defining expression fields that represent values for each row in the table."),
        Category("TableDescriptors")]
        public GridExpressionFieldDescriptorCollection GridExpressionFields
        {
            get
            {
                if (_expressions == null)
                   {
                       _expressions = new GridExpressionFieldDescriptorCollection(base.ExpressionFields);
                   }

                return _expressions;
            }

//            set
//            {
//                if (value != null)
//                    ExpressionFields.InitializeFrom(value);
//                else
//                    ResetExpressionFields();
//            }
        }

        /// <summary>
        /// Determines if the <see cref="Relations"/> collection or child objects have been modified from its
        /// default state.
        /// </summary>
        /// <returns></returns>
        public bool ShouldSerializeGridExpressionFields()
        {
            return base.ShouldSerializeExpressionFields();
        }

        /// <summary>
        /// Resets the <see cref="GridExpressionFields"/> collection back to its
        /// default state.
        /// </summary>
        public void ResetGridExpressionFields()
        {
            if (this.ShouldSerializeGridExpressionFields())
            {
                if (Engine != null)
                {
                    IComponentChangeService changeService = null;
                    if (this.Engine.ParentControl != null && this.Engine.ParentControl.Site != null)
                       { 
                           changeService  = this.Engine.ParentControl.Site.GetService(typeof(IComponentChangeService)) as IComponentChangeService;
                       }

                    if(changeService != null)
                    {
                        MemberDescriptor md = TypeDescriptor.GetProperties(this)["GridExpressionFields"];
                        changeService.OnComponentChanging(this, md);
                        base.ResetExpressionFields();
                        Engine.Table.TableDirty = true;
                        changeService.OnComponentChanged(this, md, null, null);
                        return;
                    }
                }
            }

            base.ResetExpressionFields();
            if (Engine != null)
            {
                if (Engine.ParentControl != null)
                {
                    Engine.ParentControl.SuspendLayout();
                    Engine.TableDescriptor.ResetItemProperties();
                    Engine.Table.TableDirty = true;
                    Engine.ParentControl.ResumeLayout();
                }
            }
        }
        #endregion

#endif

#if ASPNET
        [PersistenceMode(PersistenceMode.InnerProperty)]
        [NotifyParentProperty(true)]
        [Description("Specifies the collection of PrimaryKeyColumns for the table."),
        Category("TableDescriptors")]
        public override SortColumnDescriptorCollection PrimaryKeyColumns
        {
            get { return base.PrimaryKeyColumns; }
            //set { base.PrimaryKeyColumns = value; } designer ser. expects u to not have a set prop.
        }
        #region Relations
        // Here we hide the base class implementation.
        [Browsable(false)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        [XmlArrayItem(typeof(RelationDescriptor)), XmlArrayItem(typeof(GridRelationDescriptor))]
        public new RelationDescriptorCollection Relations
        {
            get{return base.Relations;}
            set
            {
                if (value != null)
                    Relations.InitializeFrom(value);
                else
                    ResetRelations();
            }
        }

        /// <summary>
        /// Gets the collection of <see cref="GridRelationDescriptor"/> objects defining relations
        /// to other tables.
        /// </summary>
        /// <remarks>
        /// The default state of this collection and child objects is auto-populated from relation descriptors
        /// found in the underlying source list for this table. <para/>
        /// If you assign a <see cref="System.Data.DataView"/> or <see cref="System.Data.DataSet"/> to <see cref="Syncfusion.Grouping.Engine.SetSourceList"/>,
        /// the <see cref="Relations"/> collection is auto-populated from <see cref="System.Data.DataRelation"/> objects
        /// found in the <see cref="System.Data.DataSet"/>.
        /// </remarks>
        [RefreshProperties(RefreshProperties.All)]
        [DesignerSerializationVisibilityAttribute(DesignerSerializationVisibility.Content)]
        [EditorAttribute(typeof(GroupingCollectionEditor), typeof(System.Drawing.Design.UITypeEditor))]
        [Browsable(true)]
        [NotifyParentProperty(true)]
        [PersistenceMode(PersistenceMode.InnerProperty)]
        [XmlIgnore()]
#if ASPNET
        [UseShouldSerialize()]
#endif
        [Description("Gets the collection of GridRelationDescriptor objects defining relations to other tables."),
        Category("TableDescriptors")]
        public GridRelationDescriptorCollection GridRelations
        {
            get
            {
                if (_relations == null)
                    _relations = new GridRelationDescriptorCollection(base.Relations);
                return _relations;
            }
        }

        /// <summary>
        /// Determines if the <see cref="Relations"/> collection or child objects have been modified from its
        /// default state.
        /// </summary>
        /// <returns></returns>
        public bool ShouldSerializeGridRelations()
        {
            return base.ShouldSerializeRelations();
        }

        /// <summary>
        /// Resets the <see cref="SortedColumns"/> collection back to its
        /// default state.
        /// </summary>
        public void ResetGridRelations()
        {
            if (this.ShouldSerializeGridRelations())
            {
                if (Engine != null)
                {
                    IComponentChangeService changeService = null;
                    if (this.Engine.ParentControl != null && this.Engine.ParentControl.Site != null)
                        changeService  = this.Engine.ParentControl.Site.GetService(typeof(IComponentChangeService)) as IComponentChangeService;

                    if(changeService != null)
                    {
                        MemberDescriptor md = TypeDescriptor.GetProperties(this)["GridRelations"];
                        changeService.OnComponentChanging(this, md);
                        base.ResetRelations();
                        Engine.Table.TableDirty = true;
                        changeService.OnComponentChanged(this, md, null, null);
                        return;
                    }
                }
            }
            base.ResetRelations();
            if (Engine != null)
            {
                if (Engine.ParentControl != null)
                {
                    Engine.ParentControl.SuspendLayout();
                    Engine.TableDescriptor.ResetItemProperties();
                    Engine.Table.TableDirty = true;
                    Engine.ParentControl.ResumeLayout();
                }
            }
        }
        #endregion
#else
        /// <summary>
        /// Gets the collection of <see cref="GridRelationDescriptor"/> objects defining relations
        /// to other tables.
        /// </summary>
        /// <remarks>
        /// The default state of this collection and child objects is auto-populated from relation descriptors
        /// found in the underlying source list for this table. <para/>
        /// If you assign a <see cref="System.Data.DataView"/> or <see cref="System.Data.DataSet"/> to <see cref="Syncfusion.Grouping.Engine.SetSourceList"/>,
        /// the <see cref="Relations"/> collection is auto-populated from <see cref="System.Data.DataRelation"/> objects
        /// found in the <see cref="System.Data.DataSet"/>.
        /// </remarks>
        [RefreshProperties(RefreshProperties.All)]
        [DesignerSerializationVisibilityAttribute(DesignerSerializationVisibility.Content)]
        [EditorAttribute(typeof(GroupingCollectionEditor), typeof(System.Drawing.Design.UITypeEditor))]
        [Browsable(true)]
        [NotifyParentProperty(true)]
        public new GridRelationDescriptorCollection Relations
        {
            get
            {
                if (_relations == null)
                {
                    _relations = new GridRelationDescriptorCollection(base.Relations);
                }

                return _relations;
            }

            set
            {
                if (value != null)
                {
                    Relations.InitializeFrom(value);
                }
                else
                {
                    ResetRelations();
                }
            }
        }
#endif
        /// <summary>
        /// Determines if the <see cref="Relations"/> collection or child objects have been modified from its
        /// default state.
        /// </summary>
        /// <returns>True if the collection was modifed.</returns>
        public new bool ShouldSerializeRelations()
        {
            return base.ShouldSerializeRelations();
        }

        /// <summary>
        /// Resets the <see cref="Relations"/> collection back to its
        /// default state.
        /// </summary>
        public new void ResetRelations()
        {
            if (this.ShouldSerializeRelations())
            {
                if (Engine != null)
                {
                    IComponentChangeService changeService = null;
                    if (this.Engine.ParentControl != null && this.Engine.ParentControl.Site != null)
                    {
                        changeService = this.Engine.ParentControl.Site.GetService(typeof(IComponentChangeService)) as IComponentChangeService;
                    }

                    if (changeService != null)
                    {
                        MemberDescriptor md = TypeDescriptor.GetProperties(this)["Relations"];
                        changeService.OnComponentChanging(this, md);
                        base.ResetRelations();
                        Engine.Table.TableDirty = true;
                        changeService.OnComponentChanged(this, md, null, null);
#if ASPNET
#else
                        this.Engine.ParentControl.Refresh();
#endif
                        return;
                    }
                }
            }

            base.ResetRelations();
            if (Engine != null)
            {
                if (Engine.ParentControl != null)
                {
#if ASPNET
#else
                    Engine.ParentControl.Invalidate();
#endif
                    Engine.Table.TableDirty = true;
                    Engine.TableDescriptor.ResetItemProperties();
                    ////                    Engine.ParentControl.SuspendLayout();
                    ////                    int count = Engine.Table.RelatedTables.Count;
                    ////                    Engine.ParentControl.ResumeLayout();
                }
            }
        }

        #endregion
        #region SummaryRows collection
        /// <summary>
        /// A collection from <see cref="GridSummaryRowDescriptor"/> that declares
        /// summary rows each with one or multiple GridSummaryColumnDescriptor elements.
        /// </summary>
        /// <remarks>
        /// When you assign a GridSummaryRowDescriptorCollection object using this property, the existing collection
        /// object is not replaced. Instead, all properties and elements are copied
        /// from the assigned GridSummaryRowDescriptorCollection object using the <see cref="GridSummaryRowDescriptorCollection.InitializeFrom"/> method.
        /// <para/>
        /// The SummaryRows collection contains GridSummaryRowDescriptors. GridSummaryRowDescriptors have a name,
        /// title, and collection of summary columns. A GridSummaryRowDescriptor also has a Read-only
        /// IsFillRow property. If this property is True, the summary should fill the
        /// whole row and not be displayed below individual columns. IsFillRow will return True if
        /// any of the GridSummaryColummDescriptors in the SummaryColumns collection is set to
        /// GridSummaryStyle.FillRow.
        /// <para/>
        /// The GridSummaryColummDescriptor defines where to display the column in the row.
        /// Essential properties are the name, format, DisplayColumn, DataMember, and
        /// SummaryType. The multiple GridSummaryColumnDescriptor objects
        /// have a name and mapping name that identify the column for which a summary should be
        /// calculated for and a SummaryType property that defines the type of calculations to be performed.
        /// <para/>
        /// Possible SummaryTypes are: Count, BooleanAggregate, ByteAggregate, CharAggregate, DistinctCount,
        /// DoubleAggregate, Int32Aggregate, MaxLength, StringAggregate, Vector, DoubleVector, and Custom.
        /// <para/>
        /// When you specify the SummaryType.Custom type, you need to set the custom method through the
        /// CreateSummaryMethod property of the SummaryDescriptor. It is CreateSummaryDelegate and
        /// is called to create an instance of a summary object. You also need to handle the
        /// GridGroupingControl.QueryCustomSummary as demonstrated in the Grid/Grouping/CustomSummaries example.
        /// </remarks>
        [RefreshProperties(RefreshProperties.All)]
        [DesignerSerializationVisibilityAttribute(DesignerSerializationVisibility.Content)]
        [NotifyParentProperty(true)]
#if ASPNET
        [PersistenceMode(PersistenceMode.InnerProperty)]
#endif
        [Description("Declares summary rows each with one or multiple GridSummaryColumnDescriptor elements."),
        Category("TableDescriptors")]
        public GridSummaryRowDescriptorCollection SummaryRows
        {
            get
            {
                if (summaryRows == null)
                {
                    summaryRows = new GridSummaryRowDescriptorCollection(this);
                    summaryRows.Changed += new ListPropertyChangedEventHandler(summaryRows_Changed);
                    summaryRows.Changing += new ListPropertyChangedEventHandler(summaryRows_Changing);
                }

                return summaryRows;
            }
#if ASPNET
            // Designer ser. requires you not to have a set property!
#else
            set
            {
                if (value != null)
                {
                    SummaryRows.InitializeFrom(value);
                }
                else
                {
                    ResetSummaryRows();
                }
            }
#endif
        }

        /// <summary>
        /// Determines if the <see cref="SummaryRows"/> collection contains values.
        /// </summary>
        /// <returns>True if not empty; False otherwise.</returns>
        public bool ShouldSerializeSummaryRows()
        {
            return SummaryRows.Count > 0;
        }

        /// <summary>
        /// Clears the <see cref="SummaryRows"/> collection.
        /// </summary>
        public void ResetSummaryRows()
        {
            SummaryRows.Clear();
        }
        #endregion
        #region StackedHeaderRows collection

        /// <summary>
        /// A collection of <see cref="GridStackedHeaderRowDescriptor"/> objects that declares
        /// StackedHeader rows each with one or multiple GridStackedHeaderDescriptor elements.
        /// An instance of this collection is returned by the <see cref="GridTableDescriptor.StackedHeaderRows"/> property
        /// of a <see cref="GridTableDescriptor"/>. StackedHeaders allow you to display headers that spread multiple columns
        /// before the regular column headers.
        /// </summary>
        /// <remarks>
        /// Each group in the GridTable has a GridStackedHeaderSection. The StackedHeader section has as many
        /// rows as there are GridStackedHeaderRowDescriptors. Each GridStackedHeaderRowDescriptor
        /// has a collection of GridStackedHeaderDescriptor columns. The GridStackedHeaderDescriptor
        /// defines the VisibleColumns or ColumnSets for which a combined header should be displayed 
        /// before the normal column headers.
        /// <para/>
        /// If you leave the VisibleColumns collection empty than this header will be used as default
        /// header for all columns that were not explicitly associated with another header.
        /// <para/>
        /// So, if you want to just add an extra Caption then you could add a StackedHeaderRow
        /// with only one StackedHeader that has an empty VisibleColumns collection.
        /// </remarks>
        [RefreshProperties(RefreshProperties.All)]
        [DesignerSerializationVisibilityAttribute(DesignerSerializationVisibility.Content)]
        [NotifyParentProperty(true)]
#if ASPNET
        [PersistenceMode(PersistenceMode.InnerProperty)]
#endif
        [Description("Declares StackedHeader rows each with GridStackedHeaderDescriptor elements that allow you to display headers that spread multiple columns."),
        Category("TableDescriptors")]
        public GridStackedHeaderRowDescriptorCollection StackedHeaderRows
        {
            get
            {
                if (stackedHeaderRows == null)
                {
                    stackedHeaderRows = new GridStackedHeaderRowDescriptorCollection(this);
                    stackedHeaderRows.Changed += new ListPropertyChangedEventHandler(stackedHeaderRows_Changed);
                    stackedHeaderRows.Changing += new ListPropertyChangedEventHandler(stackedHeaderRows_Changing);
                }

                return stackedHeaderRows;
            }
#if ASPNET
            // Designer ser. requires you not to have a set property!
#else
            set
            {
                if (value != null)
                {
                    StackedHeaderRows.InitializeFrom(value);
                }
                else
                {
                    ResetStackedHeaderRows();
                }
            }
#endif
        }

        /// <summary>
        /// Determines if the <see cref="StackedHeaderRows"/> collection contains values.
        /// </summary>
        /// <returns>True if not empty; False otherwise.</returns>
        public bool ShouldSerializeStackedHeaderRows()
        {
            return StackedHeaderRows.Count > 0;
        }

        /// <summary>
        /// Clears the <see cref="StackedHeaderRows"/> collection.
        /// </summary>
        public void ResetStackedHeaderRows()
        {
            StackedHeaderRows.Clear();
        }
        #endregion
        #region VisibleColumns collection
        /// <summary>
        /// A collection of <see cref="GridVisibleColumnDescriptor"/> columns each referencing
        /// a <see cref="GridColumnDescriptor"/> or <see cref="GridColumnSetDescriptor"/>.
        /// The order of GridVisibleColumnDescriptors in the <see cref="GridTableDescriptor.VisibleColumns"/> collection defines
        /// the left to right order of columns shown in the grid.
        /// </summary>
        /// <remarks>
        /// When you assign a GridVisibleColumnDescriptorCollection object using this property, the existing collection
        /// object is not replaced. Instead, all properties and elements are copied
        /// from the assigned GridVisibleColumnDescriptorCollection object using the <see cref="GridVisibleColumnDescriptorCollection.InitializeFrom"/> method.
        /// <para/>
        /// The VisibleColumns collection is auto-populated from the Columns collection and the ColumnSets collection.
        /// When auto-initialized, the VisibleColumns collection adds all GridColumnSetDescriptors from the ColumnSets
        /// collection and also all ColumnDescriptors from the Columns collection that have not been referenced by a
        /// ColumnSpan. So, if you do not specify any column sets, the VisibleColumns collection will have a
        /// GridVisibleColumnDescriptor for each Column with the column's name.<para/>
        /// You can also manually initialize the VisibleColumns collection. The name of the GridColumnSetDescriptor
        /// identifies the column set descriptor or column in the VisibleColumns collection.
        /// </remarks>
        [RefreshProperties(RefreshProperties.All)]
        [DesignerSerializationVisibilityAttribute(DesignerSerializationVisibility.Content)]
        [NotifyParentProperty(true)]
#if ASPNET
        [PersistenceMode(PersistenceMode.InnerProperty)]
        [UseShouldSerialize()]
#endif
        [Description("A collection of GridVisibleColumnDescriptor columns each referencing a GridColumnDescriptor or GridColumnSetDescriptor.  The order of GridVisibleColumnDescriptors in the GridTableDescriptor.VisibleColumns collection defines the left to right order of columns shown in the grid."),
            Category("TableDescriptors")]
        public GridVisibleColumnDescriptorCollection VisibleColumns
        {
            get
            {
                if (_visibleColumns == null)
                {
                    _visibleColumns = new GridVisibleColumnDescriptorCollection(this);
                    _visibleColumns.shouldPopulate = true;
                    _visibleColumns.Changed += new Syncfusion.Collections.ListPropertyChangedEventHandler(_visibleColumns_Changed);
                    _visibleColumns.Changing += new Syncfusion.Collections.ListPropertyChangedEventHandler(_visibleColumns_Changing);
                }

                return _visibleColumns;
            }
#if ASPNET
            // Designer ser. requires you not to have a set property!
#else
            set
            {
                if (value != null)
                {
                    VisibleColumns.InitializeFrom(value);
                }
                else
                {
                    ResetVisibleColumns();
                }
            }
#endif
        }

        /// <summary>
        /// Determines if the <see cref="VisibleColumns"/> collection has been modified from its
        /// default state.
        /// </summary>
        /// <returns>True if it is modified; False otherwise.</returns>
        public bool ShouldSerializeVisibleColumns()
        {
            return _visibleColumns != null && _visibleColumns.IsModified;
        }

        /// <summary>
        /// Resets the <see cref="VisibleColumns"/> collection back to its
        /// default state.
        /// </summary>
        public void ResetVisibleColumns()
        {
            if (_visibleColumns != null)
            {
                _visibleColumns.Reset();
            }
        }

        /// <summary>
        /// Gets or sets whether the <see cref="VisibleColumns"/> collection should not be autopopulated. When you set this property true
        /// <see cref="VisibleColumns"/>.<see cref="VisibleColumns"/> will be called. When you set this property false, <see cref="ResetVisibleColumns"/>
        /// will be called.
        /// </summary>
        [Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        [DefaultValue(false)]
        public bool ForceEmptyVisibleColumns
        {
            get
            {
                return ShouldSerializeVisibleColumns() && VisibleColumns.Count == 0;
            }

            set
            {
                if (value != ForceEmptyVisibleColumns)
                {
                    if (value)
                    {
                        VisibleColumns.Clear();
                    }
                    else
                    {
                        ResetVisibleColumns();
                    }
                }
            }
        }

        #endregion
        #region Columns collection
        /// <summary>
        /// A collection of <see cref="GridColumnDescriptor"/> columns with mapping information to columns of the underlying datasource.
        /// </summary>
        /// <remarks>
        /// When you assign a GridColumnDescriptorCollection object using this property, the existing collection
        /// object is not replaced. Instead, all properties and elements are copied
        /// from the assigned GridColumnDescriptorCollection object using the <see cref="GridColumnDescriptorCollection.InitializeFrom"/> method.
        /// <para/>
        /// The Columns collection lets you specify the fields that should be displayed in the GridTableControl.
        /// By default, the Columns collection is auto-populated from the underlying Fields, ExpressionFields and UnboundFields
        /// collections and will be a combination of these three collections. When the Columns collection is
        /// auto-populated and you make changes to the above collections, the changes will
        /// automatically be reflected in this collection. <para/>
        /// GridColumnDescriptors in the Columns collection have a reference to a FieldDescriptor
        /// (or ExpressionFieldDescriptor). <para/>
        /// Additionally, GridColumnDescriptors contain grid-specific information about a column such as the column width.
        /// You can manually set the column width or have it be automatically initialized by the grid to fit the string
        /// with the maximum length in the column's data. GridColumnDescriptor also has an Appearance property. This is
        /// where cell type and formatting of the column can be specified.
        /// </remarks>
        [RefreshProperties(RefreshProperties.All)]
        [DesignerSerializationVisibilityAttribute(DesignerSerializationVisibility.Content)]
        [NotifyParentProperty(true)]
#if ASPNET
        [PersistenceMode(PersistenceMode.InnerProperty)]
        [UseShouldSerialize()]
#endif
        [Description("A collection of GridColumnDescriptor columns with mapping information to columns of the underlying datasource."),
        Category("TableDescriptors")]
        public GridColumnDescriptorCollection Columns
        {
            get
            {
                if (_columns == null)
                {
                    _columns = this.CreateColumnDescriptorCollection();
                    _columns.shouldPopulate = true;
                    _columns.Changed += new ListPropertyChangedEventHandler(_columns_Changed);
                    _columns.Changing += new ListPropertyChangedEventHandler(_columns_Changing);
                }

                return _columns;
            }
#if ASPNET
            // designer ser. expects u to not have a set prop.
#else
            
            set
            {
                if (value != null)
                {
                    Columns.InitializeFrom(value);
                }
                else
                {
                    ResetColumns();
                }
            }
#endif
        }
    
        [NonSerialized]
        bool supportAppearanceDeserialization = true;

        /// <summary>
        /// To enable/disable column appearance deserialization.
        /// </summary>
        [Description("Enables/disables column appearance deserialization")]
        [DefaultValue(true)]
        [XmlIgnore()]
        public bool SupportColumnAppearanceDeserialization
        {
            get
            {
                return this.supportAppearanceDeserialization;
            }
            set
            {
                if (this.supportAppearanceDeserialization != value)
                    this.supportAppearanceDeserialization = value;
            }
        }

        /// <summary>
        /// Determines if the <see cref="Columns"/> collection has been modified from its
        /// default state.
        /// </summary>
        /// <returns>true if it is modified; false otherwise.</returns>
        public bool ShouldSerializeColumns()
        {
            return _columns != null && _columns.IsModified;
        }

        /// <summary>
        /// Resets the <see cref="Columns"/> collection back to its
        /// default state.
        /// </summary>
        public void ResetColumns()
        {
            if (ShouldSerializeColumns())
            {
                _columns.Reset();
            }
        }

        /// <summary>
        /// Gets or sets whether the <see cref="Columns"/> collection should not be autopopulated. When you set this property true
        /// <see cref="Columns"/>.<see cref="Columns"/> will be called. When you set this property false, <see cref="ResetColumns"/>
        /// will be called.
        /// </summary>
        [Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        [DefaultValue(false)]
        public bool ForceEmptyColumns
        {
            get
            {
                return ShouldSerializeColumns() && Columns.Count == 0;
            }

            set
            {
                if (value != ForceEmptyColumns)
                {
                    if (value)
                    {
                        Columns.Clear();
                    }
                    else
                    {
                        ResetColumns();
                    }
                }
            }
        }
        
        /// <override/>
        /// <summary>Determines if this object is used by the parent control in design-time.</summary>
        /// <returns>True if it is being used in design-time.</returns>
        public override bool IsDesignTime()
        {
            return Engine != null && Engine.ParentControl != null && Engine.ParentControl.InDesigner;
        }

        /// <summary>
        /// Override this factory method if custom properties should be added to the column descriptor.
        /// You also have to derive GridTableDescriptor and provide a strong typed collection
        /// property.
        /// </summary>
        /// <returns>returns GridColumnDescriptorCollection</returns>
        protected virtual GridColumnDescriptorCollection CreateColumnDescriptorCollection()
        {
            return new GridColumnDescriptorCollection(this);
        }
        #endregion
        #region ColumnSets collection

        /// <summary>
        /// A collection from <see cref="GridColumnSetDescriptor"/> with <see cref="GridColumnSpanDescriptor"/> information
        /// about columns that can spread multiple grid rows or columns. <para/>
        /// </summary>
        /// <remarks>
        /// When you assign a GridColumnSetDescriptorCollection object using this property, the existing collection
        /// object is not replaced. Instead, all properties and elements are copied
        /// from the assigned GridColumnSetDescriptorCollection object using the <see cref="GridColumnSetDescriptorCollection.InitializeFrom"/> method.
        /// <para/>
        /// The ColumnSet collection lets you specify a multi-row per record layout in a table. A ColumnSetDescriptor
        /// holds one or multiple ColumnSpans. In a GridColumnSpan, you can specify layout information of a column.
        /// You can, for example, specify that the Address column should be displayed in the grid above City and Region
        /// and span these two columns.
        /// </remarks>
        [DesignerSerializationVisibilityAttribute(DesignerSerializationVisibility.Content)]
        [RefreshProperties(RefreshProperties.All)]
        [NotifyParentProperty(true)]
#if ASPNET
        [PersistenceMode(PersistenceMode.InnerProperty)]
#endif
        [Description("A collection from GridColumnSetDescriptor with GridColumnSpanDescriptor information about columns that can spread multiple grid rows or columns"),
            Category("TableDescriptors")]
        public GridColumnSetDescriptorCollection ColumnSets
        {
            get
            {
                if (_columnSets == null)
                {
                    _columnSets = new GridColumnSetDescriptorCollection(this);
                    _columnSets.Changed += new Syncfusion.Collections.ListPropertyChangedEventHandler(_columnSets_Changed);
                    _columnSets.Changing += new ListPropertyChangedEventHandler(_columnSets_Changing);
                }

                return _columnSets;
            }
#if ASPNET
            // Designer serialization requires you to not have a set property.
#else
            set
            {
                if (value != null)
                {
                    ColumnSets.InitializeFrom(value);
                }
                else
                {
                    ResetColumnSets();
                }
            }
#endif
        }

        /// <summary>
        /// Determines if the <see cref="ColumnSets"/> collection contains values.
        /// </summary>
        /// <returns>True if not empty; False otherwise.</returns>
        public bool ShouldSerializeColumnSets()
        {
            return _columnSets != null && _columnSets.IsModified;
        }

        /// <summary>
        /// Clears the <see cref="ColumnSets"/> collection.
        /// </summary>
        public void ResetColumnSets()
        {
            if (ShouldSerializeColumnSets())
            {
                ColumnSets.Reset();
            }
        }

        #endregion
        #region ConditionalFormats collection
        /// <summary>
        /// A collection from <see cref="GridConditionalFormatDescriptor"/> which provides filter criteria for displaying a
        /// subset of records from the underlying datasource with conditional cell formatting.<para/>
        /// </summary>
        /// <remarks>
        /// When you assign a GridConditionalFormatDescriptorCollection object using this property, the existing collection
        /// object is not replaced. Instead, all properties and elements are copied
        /// from the assigned GridConditionalFormatDescriptorCollection object using the <see cref="GridConditionalFormatDescriptorCollection.InitializeFrom"/> method.
        /// <para/>
        /// The ConditionalFormats collection has GridConditionalFormatDescriptor objects. The GridConditionalFormatDescriptor
        /// defines filter criteria for displaying a
        /// subset of records from the underlying datasource with conditional cell formatting.
        /// </remarks>
        [DesignerSerializationVisibilityAttribute(DesignerSerializationVisibility.Content)]
        [RefreshProperties(RefreshProperties.All)]
        [NotifyParentProperty(true)]
#if ASPNET
        [PersistenceMode(PersistenceMode.InnerProperty)]
#endif
        [Description("A collection from GridConditionalFormatDescriptor which provides filter criteria for displaying a subset of records from the underlying datasource with conditional cell formatting."),
            Category("TableDescriptors")]
        public GridConditionalFormatDescriptorCollection ConditionalFormats
        {
            get
            {
                if (_conditionalFormats == null)
                {
                    _conditionalFormats = new GridConditionalFormatDescriptorCollection(this);
                    _conditionalFormats.Changed += new Syncfusion.Collections.ListPropertyChangedEventHandler(_conditionalFormats_Changed);
                    _conditionalFormats.Changing += new ListPropertyChangedEventHandler(_conditionalFormats_Changing);
                }

                return _conditionalFormats;
            }
#if ASPNET
            // Designer serialization requires you to not have a set property.
#else

            set
            {
                if (value != null)
                {
                    ConditionalFormats.InitializeFrom(value);
                }
                else
                {
                    ResetConditionalFormats();
                }
            }
#endif
        }

        /// <summary>
        /// Determines if the <see cref="ConditionalFormats"/> collection contains values.
        /// </summary>
        /// <returns>True if not empty; False otherwise.</returns>
        public bool ShouldSerializeConditionalFormats()
        {
            return _conditionalFormats != null && _conditionalFormats.Count > 0;
        }

        /// <summary>
        /// Clears the <see cref="ConditionalFormats"/> collection.
        /// </summary>
        public void ResetConditionalFormats()
        {
            if (ShouldSerializeConditionalFormats())
            {
                ConditionalFormats.Clear();
            }
        }

        #endregion
        #region Hide Base Summaries
        /// <summary>
        /// Gets the collection of <see cref="SummaryDescriptor"/> objects defining summaries
        /// of the table. This collection is maintained automatically by the <see cref="GridTableDescriptor"/>
        /// and is filled with summaries from the <see cref="SummaryRows"/> collection. You should not
        /// directly modify this collection. Instead, you should modify <see cref="SummaryRows"/>.
        /// </summary>
        /// <remarks>
        /// Each <see cref="SummaryDescriptor"/> in the collection references
        /// a <see cref="FieldDescriptor"/> of the <see cref="Syncfusion.Grouping.TableDescriptor.Fields"/> collection. Based
        /// on the summaries defined in this collection, each group in the table will have
        /// summaries calculated.
        /// </remarks>
        [EditorBrowsable(EditorBrowsableState.Never)]
        [XmlIgnore]
        [RefreshProperties(RefreshProperties.All)]
        [DesignerSerializationVisibilityAttribute(DesignerSerializationVisibility.Hidden)]
        [Browsable(false)]
        [NotifyParentProperty(true)]
        public new SummaryDescriptorCollection Summaries
        {
            get
            {
                return base.Summaries;
            }

            set
            {
                base.Summaries = value;
            }
        }
        #endregion
        #region FrozenColumn
#if !ASPNET
        /// <summary>
        /// Freeze columns. The name of a <see cref="GridColumnDescriptor"/> which defines the columns to prevent scrolling.
        /// </summary>
        /// <remarks>
        /// All columns left of the specified including the column will not be scrollable. 
        /// <para/>
        /// Note: If you set this property for a child table you have to make sure that the column is properly
        /// aligned with the frozen column of the parent table. It is not supported to have a different scroll position
        /// for a nested table. If it is not possible to correctly align the right border of the column in the child
        /// table with the right border of the frozen column of the parent table then you should leave this field blank.
        /// </remarks>
        [TypeConverter(typeof(StandardValuesCollectionConverter))]
        [DefaultValue("")]
        [Description("The name of a GridColumnDescriptor which defines the columns to prevent scrolling."),
        Category("TableDescriptors")]
        [RefreshProperties(RefreshProperties.All)]
        public string FrozenColumn
        {
            get
            {
                return frozenColumn;
            }
          
            set
            {
                if (this.frozenColumn != value)
                {
                    this.OnPropertyChanging(new DescriptorPropertyChangedEventArgs("FrozenColumn"));
                    frozenColumn = value;
                    this.OnPropertyChanged(new DescriptorPropertyChangedEventArgs("FrozenColumn"));
                }
            }
        }

        /// <summary>
        /// Returns the value of <see cref="FrozenColumn"/> if valid. An empty string
        /// if the parent table has no frozen column specified.
        /// </summary>
        /// <returns>The value of <see cref="FrozenColumn"/>.</returns>
        public string GetFrozenColumn()
        {
            if (this.ParentRelation != null)
            {
                GridTableDescriptor ptd = this.ParentRelation.ParentTableDescriptor;
                string parfc = ptd.GetFrozenColumn();
                if (parfc == string.Empty)
                {
                    return string.Empty;
                }

                ////Possibly automate detection of setting frozen column in nested table.
                ////if (this.FrozenColumn == string.Empty && ptd.RecordRowColumns[0,0] != null && ptd.RecordRowColumns[0,0].Name == parfc)
                ////{
                ////    GridColumnDescriptor cd = RecordRowColumns[0, 0];
                ////    if (cd != null)
                ////        return cd.Name;
                ////}
            }

            return FrozenColumn;
        }

        /// <summary>
        /// Returns width specified in TableOptions.RowHeaderWidth if TableOptions.ShowRowHeader is true; 0 otherwise.
        /// </summary>
        /// <returns>Row header width.</returns>
        public int GetRowHeaderWidth()
        {
            if (this.TableOptions.ShowRowHeader
                && this.RowsPerRecord > 0)
            {
                return TableOptions.RowHeaderWidth;
            }

            return 0;
        }

        /// <summary>
        /// Returns width specified in TableOptions.IndentWidth if TableOptions.ShowTableIndent is true; 0 otherwise.
        /// </summary>
        /// <returns>Width of indentation of each group.</returns>
        public int GetTableIndentWidth()
        {
            if (this.TableOptions.ShowTableIndent
                && this.RowsPerRecord > 0)
            {
                return TableOptions.IndentWidth;
            }

            return 0;
        }

        /// <summary>
        /// Calculates the width of row headers and all indent columns before
        /// the first record column.
        /// </summary>
        /// <returns>Width of row headers and all indent columns</returns>
        public int GetTotalWidthOfRowHeadersAndIndent()
        {
            int columnIndentCount = GetColumnIndentCount();

            int width = GetRowHeaderWidth();
            for (int n = 0; n < GroupedColumns.Count; n++)
            {
                width += GetTableIndentWidth();
            }

            if (Relations.NestedCount > 0)
            {
                width += GetTableIndentWidth();
            }

            return width;
        }

        /// <summary>
        /// Calculates the width of row headers and all indent columns before
        /// the first record column.
        /// </summary>
        /// <param name="includeWidthOfParentTableIndent">Specifies if width of nested table indents and parent table row headers should be added.</param>
        /// <returns>Width of row headers and all indent columns</returns>
        public int GetTotalWidthOfRowHeadersAndIndent(bool includeWidthOfParentTableIndent)
        {
            if (!includeWidthOfParentTableIndent)
            {
                return GetTotalWidthOfRowHeadersAndIndent();
            }

            int indentWidth = GetTotalWidthOfRowHeadersAndIndent();
            GridTableDescriptor parentTable = this.ParentTableDescriptor;
            while (parentTable != null)
            {
                indentWidth += parentTable.GetTotalWidthOfRowHeadersAndIndent();
                parentTable = parentTable.ParentTableDescriptor;
            }

            return indentWidth;
        }

        ICollection IStandardValuesProvider.GetStandardValues(PropertyDescriptor pd)
        {
            ArrayList al = new ArrayList();
            foreach (GridColumnDescriptor cd in Columns)
            {
                al.Add(cd.Name);
            }

            return al;
        }

        /// <summary>
        /// Returns the number of frozen columns based on the <see cref="FrozenColumn"/>
        /// property.
        /// </summary>
        /// <returns>Number of frozen columns.</returns>
        public int GetFrozenColumnCount()
        {
            if (frozenCols == -1)
            {
                frozenCols = 0;

                string frozenFieldName = string.Empty;
                int c = Columns.IndexOf(GetFrozenColumn());
                if (c != -1)
                {
                    frozenFieldName = Columns[c].MappingName;
                }

                if (frozenFieldName != string.Empty)
                {
                    int resultRow, resultCol;
                    ColumnToRowColIndex(frozenFieldName, out resultRow, out resultCol);
                    if (resultCol >= 0)
                    {
                        frozenCols = Math.Max(0, resultCol + 1);
                    }
                }
            }

            return frozenCols;
        }
#endif
        #endregion
        #region MaxLength Summaries

        ////bool allowCalculateMaxColumnWidth = true;

        /// <summary>
        /// Gets / sets whether the maximum number of characters found in record field cells
        /// should be calculated for columns. See also TableOptions.ColumnsMaxLengthStrategy.
        /// </summary>
        [XmlIgnore]
        [DesignerSerializationVisibilityAttribute(DesignerSerializationVisibility.Hidden)]
        [Browsable(false)]
        public bool AllowCalculateMaxColumnWidth
        {
            get
            {
                return (TableOptions.ColumnsMaxLengthStrategy & GridColumnsMaxLengthStrategy.MaxLengthSummary) != 0;
            }

            set
            {
                if (value)
                {
                    TableOptions.ColumnsMaxLengthStrategy = GridColumnsMaxLengthStrategy.MaxLengthSummary;
                }
                else
                { 
                    TableOptions.ColumnsMaxLengthStrategy = GridColumnsMaxLengthStrategy.None;
                }
            }
        }

        private SummaryDescriptorCollection summaryDescriptorsFilterBarChoices = null;
        /// <summary>
        /// Recreates the Summaries collection if fields, columns, or SummaryRows
        /// have changed. Add summaries for calculating the maximum length
        /// of columns in the table.
        /// </summary>
        public override void EnsureSummaryDescriptors()
        {
            if (summaries_savedColumnsVersion == this.Columns.Version
                && summaries_savedSummarieRowsVersion == this.SummaryRows.Version)
            {
                return;
            }

            TraceUtil.TraceCalledFromIf(Switches.AutoPopulate.TraceVerbose, 10, summaries_savedSummarieRowsVersion);

            SummaryDescriptorCollection summaryDescriptors = new SummaryDescriptorCollection();
            summaryDescriptors.InsideCollectionEditor = true;
            foreach (GridSummaryRowDescriptor sr in this.SummaryRows)
            {
                sr.SetTableDescriptor(this);
                foreach (GridSummaryColumnDescriptor sc in sr.SummaryColumns)
                {
                    string dm = sc.GetDataMember();
                    SummaryDescriptor sd;
                    if (sc.SummaryType != SummaryType.Custom)
                    {
                        sd = new SummaryDescriptor(sc.GetSummaryDescriptorName(), dm, sc.SummaryType);
                    }
                    else
                    {
                        sd = sc.SummaryDescriptor;
                    }

                    if (sd != null)
                    {
                        sd.IgnoreRecordFilterCriteria = sc.IgnoreRecordFilterCriteria;
                    }

                    if (sd != null && (sd.CreateSummaryFromElementMethod != null || sd.CreateSummaryMethod != null))
                    {
                        summaryDescriptors.Add(sd);
#if DEBUG
                        if (Switches.AutoPopulate.TraceVerbose)
                        {
                            TraceUtil.TraceCurrentMethodInfo("Add", sd);
                        }
#else
                        ;
#endif
                    }
                }
            }

            if ((this.TableOptions.ColumnsMaxLengthStrategy & GridColumnsMaxLengthStrategy.MaxLengthSummary) != 0)
            {
                foreach (GridColumnDescriptor column in this.Columns)
                {
                    if (column.MaxLength == -1
                        && column.FieldDescriptor != null
                        && (!column.FieldDescriptor.IsRelatedField() || !column.AllowDropDownCell))
                    {
                        SummaryDescriptor sd = new SummaryDescriptor(
                            column.FieldDescriptor.Name + "AutoSizeMaxLength",
                            column.FieldDescriptor.Name,
                            new CreateSummaryFromElementDelegate(GridMaxLengthSummary.CreateSummaryFromElementMethod));
                        summaryDescriptors.Add(sd);
#if DEBUG
                        if (Switches.AutoPopulate.TraceVerbose)
                        {
                            TraceUtil.TraceCurrentMethodInfo("Add", sd);
                        }
#else
                        ;
#endif
                    }
                }

                ////                RelationDescriptor parentRelation = this.ParentRelation;
                ////                if (parentRelation != null && (parentRelation.RelationKind == RelationKind.ForeignKeyReference
                ////                    || parentRelation.RelationKind == RelationKind.ListItemReference
                ////                    || parentRelation.RelationKind == RelationKind.ForeignKeyKeyWords))        // ForeignListItems
                ////                {
                ////                    foreach (FieldDescriptor field in this.Fields)
                ////                    {
                ////                        if (!field.Hide && summaryDescriptors[field.Name + "AutoSizeMaxLength"] == null)
                ////                        {
                ////                            SummaryDescriptor sd = new SummaryDescriptor(field.Name + "AutoSizeMaxLength", field.Name,
                ////                                new CreateSummaryFromElementDelegate(GridMaxLengthSummary.CreateSummaryFromElementMethod));
                ////                            summaryDescriptors.Add(sd);
                ////                            TraceUtil.TraceCurrentMethodInfoIf(Switches.AutoPopulate.TraceVerbose, "Add", sd);
                ////                        }
                ////                    }
                ////                }

                //// TODO: RelationKind.ForeignKeyKeyWords - display could be mutiple entries separated with comma.
            }

            if (summaryDescriptorsFilterBarChoices == null)
                summaryDescriptorsFilterBarChoices = new SummaryDescriptorCollection();
            EnsureSummaryFilterBarChoices(summaryDescriptorsFilterBarChoices);
            hasSummaryFilterBarChoices = summaryDescriptorsFilterBarChoices.Count > 0;

            int sdfcount = summaryDescriptorsFilterBarChoices.Count;
            for (int n = 0; n < sdfcount; n++)
            {
                summaryDescriptors.Add(summaryDescriptorsFilterBarChoices[n]);
            }

            //// ForeignListItems
            if (this.ParentRelation != null && this.ParentRelation.RelationKind == RelationKind.ForeignKeyKeyWords)
            {
                foreach (FieldDescriptor field in this.Fields)
                {
                    if (!(field.GetPropertyType() == typeof(byte[])))
                    {
                        SummaryDescriptor sd = new SummaryDescriptor(
                            field.Name, 
                            field.Name,
                            new CreateSummaryDelegate(DistinctCountSummary.CreateSummaryMethod));
                        summaryDescriptors.Add(sd);
#if DEBUG
                        if (Switches.AutoPopulate.TraceVerbose)
                        {
                            TraceUtil.TraceCurrentMethodInfo("Add", sd);
                        }
#else
                        ;
#endif
                    }
                }
            }

            summaryDescriptors.InsideCollectionEditor = false;
            summaries_savedBaseSummariesVersion = this.Summaries.Version;

            base.Summaries.InitializeFrom(summaryDescriptors);

            if (summaries_savedBaseSummariesVersion != this.Summaries.Version)
            {
                foreach (GridSummaryRowDescriptor srd in this.SummaryRows)
                {                
                    srd.ResetDisplayColumns();
                }
            }

            summaries_savedBaseSummariesVersion = -1;

            summaries_savedColumnsVersion = this.Columns.Version;
            summaries_savedSummarieRowsVersion = this.SummaryRows.Version;
        }

        /// <summary>
        /// This method is called to add summaries for those columns where GridColumnDescriptor.AllowFilter 
        /// is true. The base class implementation adds a FilterBarChoicesSummary for each column with .AllowFilter set to true.
        /// </summary>
        /// <param name="summaryDescriptors">The collection where new summaries should be added.</param>
        protected virtual void EnsureSummaryFilterBarChoices(SummaryDescriptorCollection summaryDescriptors)
        {
            GridGroupingControl gc = Engine != null ? Engine.ParentControl : null;

            foreach (GridColumnDescriptor column in this.Columns)
            {
                if (column.AllowFilter)
                {
                    //// Raise event to give user a chance to implement their own filter bar choice list logic.
                    if (gc != null)
                    {
                        gc.TableDescriptor.IsExcelFilterWired = true;
                        GridQueryFilterBarChoicesEventArgs qe = new GridQueryFilterBarChoicesEventArgs(column, true, null);
#if ASPNET
#else
                        gc.OnQueryFilterBarChoices(qe);
#endif
                        if (qe.Cancel)
                        {
                            continue;
                        }
                        else if (!qe.ShouldCreateSummaryDescriptor)
                        {
                            this.hasCustomSummaryFilterBarChoices = true;
                            continue;
                        }
                    }

                    if (column.FieldDescriptor != null)
                    {
                        Type type = column.FieldDescriptor.GetPropertyType();
#if !(SyncfusionFramework1_0 || SyncfusionFramework1_1)
                        Type nullableUnderlyingType = Nullable.GetUnderlyingType(type);
                        if (nullableUnderlyingType != null)
                        {
                            type = nullableUnderlyingType;
                        }
#endif
                        if (!typeof(IComparable).IsAssignableFrom(type))
                        {
                            continue;
                        }

                        SummaryDescriptor sd = new SummaryDescriptor(
                            column.Name + "FilterBarChoices",
                            column.MappingName,
                            new CreateSummaryDelegate(FilterBarChoicesSummary.CreateSummaryMethod));
                        sd.IgnoreRecordFilterCriteria = true;
                        if (!gc.OptimizeFilterPerformance && !summaryDescriptors.Contains(sd))
                            summaryDescriptors.Add(sd);                 
#if DEBUG
                        if (Switches.AutoPopulate.TraceVerbose)
                        {
                            TraceUtil.TraceCurrentMethodInfo("Add", sd);
                        }
#else
                ;
#endif
                    }
                }
            }
        }

        /// <summary>
        /// Returns true if any summaries were implicitly created for FilterBar
        /// </summary>
        [Browsable(false), XmlIgnore, DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public bool HasSummaryFilterBarChoices
        {
            get
            {
                return hasSummaryFilterBarChoices;
            }
        }

        /// <summary>
        /// Returns true if there are columns with .AllowFilter = true and GridFilterBarChoicesEventArgs.ShouldCreateSummaryDescriptor
        /// set to false.
        /// </summary>
        [Browsable(false), XmlIgnore, DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public bool HasCustomSummaryFilterBarChoices
        {
            get
            {
                return hasCustomSummaryFilterBarChoices;
            }
        }

        #endregion
        #region Multiple Rows per Record and Column To Field Mapping
        /// <summary>
        /// An internal array used for multiple rows per Record and Column to Field Mapping.
        /// </summary>
        [XmlIgnore]
        [DesignerSerializationVisibilityAttribute(DesignerSerializationVisibility.Hidden)]
        [Browsable(false)]
        public GridColumnDescriptor[,] RecordRowColumns
        {
            get
            {
                EnsureRecordRowColumns();
                return _recordRowColumns;
            }
        }

        /// <summary>
        /// An internal array used for multiple rows per Record and Column to Field Mapping.
        /// </summary>
        [XmlIgnore]
        [DesignerSerializationVisibilityAttribute(DesignerSerializationVisibility.Hidden)]
        [Browsable(false)]
        public GridRangeInfo[,] RecordRowCoveredRanges
        {
            get
            {
                EnsureRecordRowColumns();
                return _recordRowCoveredRanges;
            }
        }

        /// <summary>
        /// Returns the number of indent columns.
        /// </summary>
        /// <returns>If records don't have nested tables, the method returns GroupedColumns.Count+1; otherwise GroupedColumns.Count+2.</returns>
        public int GetColumnIndentCount()
        {
            return GroupedColumns.Count + 1 + (Relations.NestedCount > 0 ? 1 : 0);
        }

        /// <summary>
        /// Returns the number of grid columns to display in grid.
        /// </summary>
        /// <returns>Number of columns.</returns>
        public int GetColCount()
        {
            return GetColumnIndentCount() + GetColumnSetColCount();
        }

        /// <summary>
        /// Returns the last column index where a record field is displayed.
        /// </summary>
        /// <returns>Last column index.</returns>
        public int GetLastColumnIndex()
        {
            return GetColCount() - 1;
        }

        /// <summary>
        /// Converts a column index in a grid to a zero-based number adjusted for column headers (subtracting <see cref="GetColumnIndentCount"/>)
        /// collection.
        /// </summary>
        /// <param name="colIndex">The column index in the grid.</param>
        /// <returns>A zero-based number.</returns>
        public int ColIndexToField(int colIndex)
        {
            int fieldNum;
            if (colIndex <= GetColumnIndentCount())
            {
                fieldNum = 0;
            }
            else
            {
                fieldNum = colIndex - GetColumnIndentCount();
            }

            return fieldNum;
        }

        /// <summary>
        /// Converts a zero-based number to a column index in a grid adjusted for column headers (adding <see cref="GetColumnIndentCount"/>).
        /// </summary>
        /// <param name="fieldNum">A zero-based number.</param>
        /// <returns>The column index in the grid.</returns>
        public int FieldToColIndex(int fieldNum)
        {
            if (fieldNum < 0)
            {
                return -1;
            }

            return fieldNum + GetColumnIndentCount();
        }

        /// <summary>
        /// Returns the zero-based index for a column. The resulting
        /// number can be used as an index to look up a <see cref="GridColumnDescriptor"/> in the <see cref="Columns"/>
        /// collection.
        /// </summary>
        /// <param name="name">The name of the column to be matched.</param>
        /// <returns>A zero-based field number in the <see cref="Columns"/> collection; -1 if not found.</returns>
        public int NameToField(string name)
        {
            return Columns.IndexOf(name);
        }

        /// <summary>
        /// Searches for the column descriptor with the specified name.
        /// </summary>
        /// <param name="name">The name of the element to locate in the collection. </param>
        /// <returns>The GridColumnDescriptor that matches the name, if found; otherwise, NULL.</returns>
        public GridColumnDescriptor GetColumnDescriptor(string name)
        {
            int indexOf = this.Columns.IndexOf(name);
            if (indexOf != -1)
            {
                return this.Columns[indexOf];
            }

            return null;
        }

        int recordRowVisibleColumnsVersion = -1;

        /// <exclude/>
        protected override void OnEnableOneTimePopulate()
        {
            this.Columns.EnableOneTimePopulate();
            this.VisibleColumns.EnableOneTimePopulate();
        }

        /// <summary>
        /// Recreates the internal <see cref="RecordRowCoveredRanges"/> and <see cref="RecordRowColumns"/>
        /// if changes were detected in the <see cref="ColumnSets"/>, <see cref="VisibleColumns"/>,
        /// or <see cref="Columns"/> collections.
        /// </summary>
        public void EnsureRecordRowColumns()
        {
            if (_recordRowColumns == null || recordRowVisibleColumnsVersion != this.VisibleColumns.Version)
            {
                recordRowVisibleColumnsVersion = this.VisibleColumns.Version;
                rowsPerRecord = -1;
                int colCount = CalculateColumnSetCols();
                int rowCount = RowsPerRecord;

                GridColumnDescriptor[,] recordRowColumns = new GridColumnDescriptor[rowCount, colCount];
                GridRangeInfo[,] recordRowCoveredRanges = new GridRangeInfo[rowCount, colCount];

                if (rowCount > 0)
                {
                    int fieldNum = 0;
                    int count;
                    foreach (GridVisibleColumnDescriptor visibleColumn in VisibleColumns)
                    {
                        count = 1;
                        string name = visibleColumn.Name;
                        int n = this.ColumnSets.IndexOf(name);
                        if (n != -1)
                        {
                            GridColumnSetDescriptor columnSet = this.ColumnSets[n];
                            if (columnSet != null)
                            {
                                foreach (GridColumnSpanDescriptor columnSpan in columnSet.ColumnSpans)
                                {
                                    if (columnSpan.Range.IsCells)
                                    {
                                        GridRangeInfo range = GridRangeInfo.Cells(columnSpan.Range.Top, fieldNum + columnSpan.Range.Left, columnSpan.Range.Bottom, fieldNum + columnSpan.Range.Right);
                                        for (int r = columnSpan.Range.Top; r <= columnSpan.Range.Bottom; r++)
                                        {
                                            for (int c = columnSpan.Range.Left; c <= columnSpan.Range.Right; c++)
                                            {
                                                recordRowColumns[r, c + fieldNum] = this.Columns[columnSpan.Name];
                                                recordRowCoveredRanges[r, c + fieldNum] = range;
                                            }
                                        }
                                    }

                                    count = Math.Max(count, columnSpan.Range.Right + 1);
                                }
                            }
                        }
                        else
                        {
                            recordRowColumns[0, fieldNum] = this.Columns[name];
                            recordRowCoveredRanges[0, fieldNum] = GridRangeInfo.Cell(0, fieldNum);
                        }

                        fieldNum += count;
                    }
                }

                int cou = this.SummaryRows.Count; //// just in case it needs on demand init...
                _recordRowColumns = recordRowColumns;
                _recordRowCoveredRanges = recordRowCoveredRanges;
                foreach (GridSummaryRowDescriptor srd in this.SummaryRows)
                {
                    srd.ResetDisplayColumns();
                }
            }

            foreach (GridSummaryRowDescriptor srd in this.SummaryRows)
            {
                srd.EnsureDisplayColumns();
            }
        }

        /// <summary>
        /// Returns the relative row and column index in the grid of a column descriptor.
        /// </summary>
        /// <param name="fieldDescriptorName">The name of the field descriptor (which is GridColumnDescriptor.MappingName).</param>
        /// <param name="resultRow">Returns the relative zero-based row index; -1 if column was not found.</param>
        /// <param name="resultCol">Returns the relative zero-based column index; -1 if column was not found.</param>
        /// <returns>True if column was found; False otherwise.</returns>
        public bool ColumnToRowColIndex(string fieldDescriptorName, out int resultRow, out int resultCol)
        {
            GridTableDescriptor td = this; ////.Table.TableDescriptor;
            resultRow = -1;
            resultCol = -1;

            bool found = false;
            for (int row = 0; !found && row < td.RowsPerRecord; row++)
            {
                int colCount = td.GetColumnSetColCount();
                for (int col = 0; !found && col < colCount; col++)
                {
                    if (td.RecordRowColumns[row, col] != null && td.RecordRowColumns[row, col].MappingName == fieldDescriptorName)
                    {
                        found = true;
                        resultRow = row;
                        resultCol = col;
                    }
                }
            }

            if (!found && fieldDescriptorName.IndexOf("_") != -1)
            {
                return ColumnToRowColIndex(fieldDescriptorName.Replace("_", "."), out resultRow, out resultCol);
            }

            return found;
        }

        /// <summary>
        /// Used internally.
        /// </summary>
        /// <returns>returns the count of columnset</returns>
        /// <internalonly/>
        [Syncfusion.Documentation.DocumentationExclude()]
        public int GetColumnSetColCount()
        {
            return RecordRowColumns.GetLength(1);
        }

        int CalculateRowsPerRecord()
        {
            if (Columns.Count == 0)
            {
                return 0;
            }

            int count = 1;
            foreach (GridColumnSetDescriptor columnSet in ColumnSets)
            {
                foreach (GridColumnSpanDescriptor columnSpan in columnSet.ColumnSpans)
                {
                    if (columnSpan.Range.IsCells)
                    {
                        count = Math.Max(count, columnSpan.Range.Bottom + 1);
                    }
                }
            }

            return count;
        }

        internal int CalculateColumnSetCols()
        {
            int count = 1;
            int colCount = 0;
            foreach (GridVisibleColumnDescriptor visibleColumn in VisibleColumns)
            {
                count = 1;
                string name = visibleColumn.Name;
                int n = this.ColumnSets.IndexOf(name);
                if (n != -1)
                {
                    GridColumnSetDescriptor columnSet = this.ColumnSets[n];
                    if (columnSet != null)
                    {
                        foreach (GridColumnSpanDescriptor columnSpan in columnSet.ColumnSpans)
                        {
                            if (columnSpan.Range.IsCells)
                            {
                                count = Math.Max(count, columnSpan.Range.Right + 1);
                            }
                        }
                    }
                }

                colCount += count;
            }

            return colCount;
        }

        ////        internal int GetCachedMaxColumnSetCols()
        ////        {
        ////            if (cachedMaxColumnSetCols == -1)
        ////                cachedMaxColumnSetCols = this.CalculateColumnSetCols();
        ////            return cachedMaxColumnSetCols;
        ////        }

        #endregion
        #region Collection Changed handlers

        private void _visibleColumns_Changing(object sender, ListPropertyChangedEventArgs e)
        {
            this.OnPropertyChanging(new DescriptorPropertyChangedEventArgs("VisibleColumns", e));
        }

        private void summaryRows_Changing(object sender, ListPropertyChangedEventArgs e)
        {
            this.OnPropertyChanging(new DescriptorPropertyChangedEventArgs("SummaryRows", e));
        }

        private void stackedHeaderRows_Changing(object sender, ListPropertyChangedEventArgs e)
        {
            this.OnPropertyChanging(new DescriptorPropertyChangedEventArgs("StackedHeader", e));
        }

        private void _columns_Changing(object sender, ListPropertyChangedEventArgs e)
        {
            this.OnPropertyChanging(new DescriptorPropertyChangedEventArgs("Columns", e));
        }

        private void _columnSets_Changing(object sender, ListPropertyChangedEventArgs e)
        {
            this.OnPropertyChanging(new DescriptorPropertyChangedEventArgs("ColumnSets", e));
        }

        private void _conditionalFormats_Changing(object sender, ListPropertyChangedEventArgs e)
        {
            this.OnPropertyChanging(new DescriptorPropertyChangedEventArgs("ColumnSets", e));
        }

        private void summaryRows_Changed(object sender, ListPropertyChangedEventArgs e)
        {
            this.InitializePropertyDescriptors();
            this.OnPropertyChanged(new DescriptorPropertyChangedEventArgs("SummaryRows", e));
        }

        private void stackedHeaderRows_Changed(object sender, ListPropertyChangedEventArgs e)
        {
            this.InitializePropertyDescriptors();
            this.OnPropertyChanged(new DescriptorPropertyChangedEventArgs("StackedHeaderRows", e));
        }

        private void Fields_Changed(object sender, ListPropertyChangedEventArgs e)
        {
            if (!this.ShouldSerializeVisibleColumns() && !this.ShouldSerializeColumns())
            {
                ResetRowLayout();
            }
        }

        private void _columns_Changed(object sender, Syncfusion.Collections.ListPropertyChangedEventArgs e)
        {
            this.InitializePropertyDescriptors();
            if (!this.ShouldSerializeVisibleColumns())
            {
                ResetRowLayout();
            }
#if ASPNET
#else
            GridEngine engine = Engine;
            if (engine != null)
            {            
                engine.RaiseMarkResync(true);
            }
#endif
            this.OnPropertyChanged(new DescriptorPropertyChangedEventArgs("Columns", e));
        }

        private void _columnSets_Changed(object sender, ListPropertyChangedEventArgs e)
        {
            this.InitializePropertyDescriptors();
            ResetRowLayout();
            this.OnPropertyChanged(new DescriptorPropertyChangedEventArgs("ColumnSets", e));
        }

        private void _conditionalFormats_Changed(object sender, ListPropertyChangedEventArgs e)
        {
            this.InitializePropertyDescriptors();
            this.OnPropertyChanged(new DescriptorPropertyChangedEventArgs("ColumnSets", e));
        }

        private void _visibleColumns_Changed(object sender, ListPropertyChangedEventArgs e)
        {
            this.InitializePropertyDescriptors();
            ResetRowLayout();
            this.OnPropertyChanged(new DescriptorPropertyChangedEventArgs("VisibleColumns", e));
        }

        void ResetRowLayout()
        {
            this._recordRowColumns = null;
            this._recordRowCoveredRanges = null;
            ////cachedMaxColumnSetCols = -1;
        }
        #endregion
        #region QueryCustomSummary
        //// event GridRaiseQueryCustomSummary QueryCustomSummary

        /// <summary>
        /// Occurs for each GridSummaryColumnDescriptor before the <see cref="SummaryDescriptor"/> is determined. You must handle this event if you specified <see cref="SummaryType.Custom"/> as <see cref="GridSummaryColumnDescriptor.SummaryType"/>.
        /// </summary>
        [Description("Occurs for each GridSummaryColumnDescriptor before the SummaryDescriptor is determined. You must handle this event if you specified SummaryType.Custom as GridSummaryColumnDescriptor.SummaryType.")]
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

        internal void RaiseQueryCustomSummary(GridQueryCustomSummaryEventArgs e)
        {
            OnQueryCustomSummary(e);

            if (!e.Handled && Engine != null)
            {
                Engine.RaiseQueryCustomSummary(e);
            }
        }
        #endregion
        #region Appearance
        GridTableCellAppearance appearance;

        /// <summary>
        /// The default <see cref="GridTableCellAppearance"/> with default <see cref="GridTableCellStyleInfo"/>
        /// information for all cell elements in the table. This property lets you control almost every aspect of
        /// the appearance of the grouping grid like cell backcolor, font, or the cell type.
        /// </summary>
        [RefreshProperties(RefreshProperties.All)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
        [NotifyParentProperty(true)]
#if ASPNET
        [PersistenceMode(PersistenceMode.InnerProperty)]
#endif
        [Description("Specifies almost every aspect of the appearance of the grouping grid like cell backcolor,font,or the cell type."),
        Category("TableDescriptors")]
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
        /// Indicates the grouping grid is wired with excel style filter.
        /// This is specifically used to apply the filter in excel sheet while exporting.
        /// </summary>
        [XmlIgnore]
        [DesignerSerializationVisibilityAttribute(DesignerSerializationVisibility.Hidden)]
        [Browsable(false)]
        public bool IsExcelFilterWired
        {
            get
            {
                return isExcelFilterWired;
            }
            set
            {
                if (isExcelFilterWired != value)
                    isExcelFilterWired = value;
            }
        }

        /// <summary>
        /// Determines whether <see cref="Appearance"/> has been modified
        /// and contents should be serialized at design-time.
        /// </summary>
        /// <returns>true if contents were changed; false otherwise.</returns>
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

        #endregion
        #region IGridTableCellAppearanceSource Members

        GridTableCellAppearance IGridTableCellAppearanceSource.GetAppearance()
        {
            return Appearance;
        }

        [XmlIgnore]
        [DesignerSerializationVisibilityAttribute(DesignerSerializationVisibility.Hidden)]
        [Browsable(false)]
        internal bool HasAppearance
        {
            get { return ShouldSerializeAppearance(); }
        }

        GridTableCellAppearance IGridTableCellAppearanceSource.GetBaseAppearance()
        {
            if (InheritAppearanceFomParent && this.ParentTableDescriptor != null)
            {
                return ((GridTableDescriptor)this.ParentTableDescriptor).Appearance;
            }

            if (Engine != null)
            {
                return Engine.Appearance;
            }

            return null;
        }

        void IGridTableCellAppearanceSource.RaiseAppearanceChanged(GridTableCellStyleInfoChangedEventArgs e)
        {
            this.OnPropertyChanged(new DescriptorPropertyChangedEventArgs("Appearance", e));
        }

        void IGridTableCellAppearanceSource.RaiseAppearanceChanging(GridTableCellStyleInfoChangedEventArgs e)
        {
            this.OnPropertyChanging(new DescriptorPropertyChangedEventArgs("Appearance", e));
        }

        #endregion
        #region InheritAppearanceFomParent
        /// <summary>
        /// Gets / sets whether the <see cref="Appearance"/> of the table descriptor should
        /// inherit properties of a <see cref="ParentTableDescriptor"/> if this object is the child table
        /// descriptor in a relation.
        /// </summary>
        [RefreshProperties(RefreshProperties.All)]
        [XmlIgnore]
        [DesignerSerializationVisibilityAttribute(DesignerSerializationVisibility.Hidden)]
        [Browsable(false)]
        public bool InheritAppearanceFomParent
        {
            get
            {
                return inheritAppearanceFomParent;
            }

            set
            {
                inheritAppearanceFomParent = value;
            }
        }
        #endregion
        #region QueryCellStyleInfo
        /// <summary>
        /// Occurs for each cell before a <see cref="GridTableModel"/>
        /// starts painting and lets users customize the display of cells.
        /// </summary>
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

        /// <summary>
        /// Raises the <see cref="QueryCellStyleInfo"/> event.
        /// </summary>
        /// <param name="e">A <see cref="GridTableCellStyleInfoEventArgs" /> that contains the event data.</param>
        internal void RaiseQueryCellStyleInfo(GridTableCellStyleInfoEventArgs e)
        {
            OnQueryCellStyleInfo(e);

            if (this.Engine != null)
            {
                Engine.RaiseQueryCellStyleInfo(e);
            }
        }
        #endregion
#if ASPNET
        #region EditFormSettings
        [RefreshProperties( RefreshProperties.All )]
        [DesignerSerializationVisibility( DesignerSerializationVisibility.Content )]
        [NotifyParentProperty( true )]
        [UseShouldSerialize()]
        [PersistenceMode( PersistenceMode.InnerProperty )]
        [Category( "TableDescriptors" )]
        [Description( "Settings for form edit mode." )]
        public EditFormSettings EditFormSettings
        {
            get
            {
                return m_efsSettings;
            }

            set
            {
                if( value == null )
                {
                    m_efsSettings = m_efsDefaultSettings;
                }
                else
                {
                    m_efsSettings = value;
                }
            }
        }

        public void ResetEditFormSettings()
        {
            this.EditFormSettings = new EditFormSettings();
        }

        public bool ShouldSerializeEditFormSettings()
        {
            return true;
        }
        #endregion
#endif
        #region IGridGroupOptionsSource Members
        GridGroupOptionsStyleInfo groupOptions;

        ////        [XmlIgnore]
        ////        [DesignerSerializationVisibilityAttribute(DesignerSerializationVisibility.Hidden)]
        ////        [Browsable(false)]
        bool IGridGroupOptionsSource.HasGroupOptions
        {
            get
            {
                return groupOptions != null;
            }
        }

        GridGroupOptionsStyleInfo IGridGroupOptionsSource.GroupOptions
        {
            get
            {
                return ChildGroupOptions;
            }
        }

        /// <summary>
        /// Lets you control the look of inner groups like whether the Caption Row is visible or what CaptionText is.
        /// </summary>
        [RefreshProperties(RefreshProperties.All)]
        [DesignerSerializationVisibilityAttribute(DesignerSerializationVisibility.Content)]
        [Browsable(true)]
        [NotifyParentProperty(true)]
#if ASPNET
        [PersistenceMode(PersistenceMode.InnerProperty)]
#endif
        [Description("Specifies the look of inner groups like whether the Caption Row is visible or what CaptionText is."),
            Category("TableDescriptors")]
        public GridGroupOptionsStyleInfo ChildGroupOptions
        {
            get
            {
                if (groupOptions == null)
                {
                    groupOptions = new GridGroupOptionsStyleInfo(new GridGroupOptionsStyleInfoIdentity(this));
                }

                return groupOptions;
            }

            set
            {
                ChildGroupOptions.CopyFrom(value);
            }
        }

        /// <summary>
        /// Determines whether <see cref="ChildGroupOptions"/> were modified
        /// and contents should be serialized at design-time.
        /// </summary>
        /// <returns>true if contents were changed; false otherwise.</returns>
        public bool ShouldSerializeChildGroupOptions()
        {
            return this.groupOptions != null && !groupOptions.IsEmpty;
        }

        /// <summary>
        /// Discards any changes for the <see cref="ChildGroupOptions"/> object.
        /// </summary>
        public void ResetChildGroupOptions()
        {
            if (this.ShouldSerializeChildGroupOptions())
            {
                this.OnPropertyChanging(new DescriptorPropertyChangedEventArgs("ChildGroupOptions"));
                this.groupOptions = null;
                this.OnPropertyChanged(new DescriptorPropertyChangedEventArgs("ChildGroupOptions"));
            }
        }

        void IGridGroupOptionsSource.RaiseGroupOptionsChanged(GridGroupOptionsChangedEventArgs e)
        {
            ////            if (this.Engine != null)
            ////                Engine.RaiseGroupOptionsChanged(e);
            this.OnPropertyChanged(new DescriptorPropertyChangedEventArgs("GroupOptions", e));
        }

        void IGridGroupOptionsSource.RaiseGroupOptionsChanging(GridGroupOptionsChangedEventArgs e)
        {
            ////            if (this.Engine != null)
            ////                Engine.RaiseGroupOptionsChanging(e);
            this.OnPropertyChanging(new DescriptorPropertyChangedEventArgs("GroupOptions", e));
        }

        IGridGroupOptionsSource IGridGroupOptionsSource.GetParentGroupOptionsSource()
        {
            return this.Engine;
        }

        GridGroupOptionsStyleInfo topLevelGroupOptions;

        [XmlIgnore]
        [DesignerSerializationVisibilityAttribute(DesignerSerializationVisibility.Hidden)]
        [Browsable(false)]
        internal bool HasTopLevelGroupOptions
        {
            get
            {
                return topLevelGroupOptions != null;
            }
        }

        /// <summary>
        /// Lets you control the look of the top most group like whether the Caption Row is visible or what CaptionText is.
        /// </summary>
        [RefreshProperties(RefreshProperties.All)]
        [DesignerSerializationVisibilityAttribute(DesignerSerializationVisibility.Content)]
        [Browsable(true)]
        [NotifyParentProperty(true)]
#if ASPNET
        [PersistenceMode(PersistenceMode.InnerProperty)]
#endif
        [Description("Specifies the look of the top most group like whether the Caption Row is visible of what CaptionText is."),
            Category("TableDescriptors")]
        public GridGroupOptionsStyleInfo TopLevelGroupOptions
        {
            get
            {
                if (topLevelGroupOptions == null)
                {
                    topLevelGroupOptions = new GridGroupOptionsStyleInfo(new GridGroupOptionsStyleInfoIdentity(this, GridGroupOptionsType.TopLevelGroup));
                }

                return topLevelGroupOptions;
            }

            set
            {
                TopLevelGroupOptions.CopyFrom(value);
            }
        }

        /// <summary>
        /// Determines whether <see cref="TopLevelGroupOptions"/> were modified
        /// and contents should be serialized at design-time.
        /// </summary>
        /// <returns>True if contents were changed; False otherwise.</returns>
        public bool ShouldSerializeTopLevelGroupOptions()
        {
            return this.topLevelGroupOptions != null && !topLevelGroupOptions.IsEmpty;
        }

        /// <summary>
        /// Discards any changes for the <see cref="TopLevelGroupOptions"/> object.
        /// </summary>
        public void ResetTopLevelGroupOptions()
        {
            if (this.ShouldSerializeTopLevelGroupOptions())
            {
                this.OnPropertyChanging(new DescriptorPropertyChangedEventArgs("TopLevelGroupOptions"));
                this.topLevelGroupOptions = null;
                this.OnPropertyChanged(new DescriptorPropertyChangedEventArgs("TopLevelGroupOptions"));
            }
        }

        #endregion
        #region IGridTableOptionsSource Members
        GridTableOptionsStyleInfo tableOptions;

        ////        [XmlIgnore]
        ////        [DesignerSerializationVisibilityAttribute(DesignerSerializationVisibility.Hidden)]
        ////        [Browsable(false)]
        bool IGridTableOptionsSource.HasTableOptions
        {
            get
            {
                return tableOptions != null;
            }
        }

        /// <summary>
        /// Lets you set table-wide properties like the width of the indent column or whether header rows should be visible.
        /// </summary>
        [RefreshProperties(RefreshProperties.All)]
        [DesignerSerializationVisibilityAttribute(DesignerSerializationVisibility.Content)]
        [Browsable(true)]
        [NotifyParentProperty(true)]
#if ASPNET
        [PersistenceMode(PersistenceMode.InnerProperty)]
#endif
        [Description("Specifies table-wide properties like width of indent column, etc."),
        Category("TableDescriptors")]
        public GridTableOptionsStyleInfo TableOptions
        {
            get
            {
                if (tableOptions == null)
                {
                    tableOptions = new GridTableOptionsStyleInfo(new GridTableOptionsStyleInfoIdentity(this));
                }

                return tableOptions;
            }

            set
            {
                TableOptions.CopyFrom(value);
            }
        }

        /// <summary>
        /// Determines whether <see cref="TableOptions"/> were modified
        /// and contents should be serialized at design-time.
        /// </summary>
        /// <returns>True if contents were changed; False otherwise.</returns>
        public bool ShouldSerializeTableOptions()
        {
            return this.tableOptions != null && !tableOptions.IsEmpty;
        }

        /// <summary>
        /// Discards any changes for the <see cref="TableOptions"/> object.
        /// </summary>
        public void ResetTableOptions()
        {
            if (this.ShouldSerializeTableOptions())
            {
                this.tableOptions = null;
                this.OnPropertyChanged(new DescriptorPropertyChangedEventArgs("TableOptions"));
            }
        }

        void IGridTableOptionsSource.RaiseTableOptionsChanged(GridTableOptionsChangedEventArgs e)
        {
            ////            if (this.Engine != null)
            ////                Engine.RaiseTableOptionsChanged(e);
            this.OnPropertyChanged(new DescriptorPropertyChangedEventArgs("TableOptions", e));
        }

        void IGridTableOptionsSource.RaiseTableOptionsChanging(GridTableOptionsChangedEventArgs e)
        {
            ////            if (this.Engine != null)
            ////                Engine.RaiseTableOptionsChanging(e);
            this.OnPropertyChanging(new DescriptorPropertyChangedEventArgs("TableOptions", e));
        }

        IGridTableOptionsSource IGridTableOptionsSource.GetParentTableOptionsSource()
        {
            return this.Engine;
        }

        #endregion
        #region TableDescriptor overrides
        /// <override/>
        /// <summary>Gets the number of rows that should be added to each record.</summary>
        [DesignerSerializationVisibilityAttribute(DesignerSerializationVisibility.Hidden)]
        [Browsable(false)]
        public override int RowsPerRecord
        {
            get
            {
                if (rowsPerRecord == -1 || rowsPerRecordColumnSetsVersion != ColumnSets.Version)
                {
                    rowsPerRecord = CalculateRowsPerRecord();
                    //// Note: When there are no columns at the time this    property is called rowsPerRecord = 0 is a valid value.
                    //// For example, a DataSet has no column (only tables). Then no record rows are
                    //// displayed.
                    rowsPerRecordColumnSetsVersion = ColumnSets.Version;
                    ////TraceUtil.TraceCurrentMethodInfo(this, rowsPerRecord);
                }

                return rowsPerRecord;
            }
        }

        /// <override/>
        protected override void OnTableSourceListChanged(TableEventArgs e)
        {
            rowsPerRecord = -1;
            base.OnTableSourceListChanged(e);
        }
        
        /// <override/>
        protected override void OnItemPropertiesChanged(EventArgs e)
        {
            rowsPerRecord = -1;
            base.OnItemPropertiesChanged(e);
        }

        /// <override/>
        /// <summary>Gets the number of preview rows that should be added to each record.</summary>
        [DesignerSerializationVisibilityAttribute(DesignerSerializationVisibility.Hidden)]
        [Browsable(false)]
        public override int PreviewRowsPerRecord
        {
            get
            {
                return TableOptions.ShowRecordPreviewRow ? 1 : 0;
            }
        }
        #endregion

        private void RecordFilters_Changing(object sender, ListPropertyChangedEventArgs e)
        {
#if ASPNET
#else
            GridEngine engine = Engine;
            if (engine != null)
            {
                GridTableControl control = engine.TableControl;
                if (control != null)
                {
                    control.VScrollBar.Value = control.VScrollBar.Minimum;
                }
            }
#endif
        }

        void RecordFilters_Changed(object sender, ListPropertyChangedEventArgs e)
        {

        }

        /// <override/>
        protected override void OnPropertyChanged(DescriptorPropertyChangedEventArgs e)
        {
            if (e.PropertyName != "Summaries")
            {
                base.OnPropertyChanged(e);
            }

#if ASPNET
#else
            frozenCols = -1;

            GridEngine engine = Engine;
            if (engine != null)
            {
                engine.RaiseMarkResync(false);
            }
#endif
        }

        private void Engine_DataMemberChanged(object sender, EventArgs e)
        {
            ResetItemProperties();
        }

        private void Engine_DataSourceChanged(object sender, EventArgs e)
        {
            ResetItemProperties();
        }

        internal System.Collections.Generic.Dictionary<string, object[]> sortByDisplayMemberCols = null;

        /// <summary>
        /// Clears the sortByDisplayMemberCols collection. 
        /// </summary>
        public void ResetSortByDisplayMemberCols()
        {
            if (sortByDisplayMemberCols != null)
            {
                sortByDisplayMemberCols.Clear();
                sortByDisplayMemberCols = null;
            }
        }

        /// <summary>
        /// Determines if the specified column should be sorted by the DisplayMember.
        /// Default behavior of the method is to return false.
        /// GridGroupingControl overrides this method and checks whether the GridColumnDescriptor
        /// associated with column has its GridColumnDescriptor.SortByDisplayMember 
        /// property set to true.
        /// </summary>
        /// <param name="cd">The SortColumnDescriptor.</param>
        /// <returns>True if the column should be sorted by the DisplayMember.</returns>
        public override bool ShouldSortByDisplayMember(SortColumnDescriptor cd)
        {
            ////cd.Name should match FieldDescriptor.Name which matches
            ////GridColumnDescriptor.MappingName.Keys for each entry in sortByDisplayMemberCols
            ////is GridColumnDescriptor.MappingName
            return sortByDisplayMemberCols != null && sortByDisplayMemberCols.ContainsKey(cd.Name);
        }
        /// <summary>
        /// Initiate sort display member columns.
        /// </summary>
        protected override void InitSortByDisplayMemberCols()
        {
            if (sortByDisplayMemberCols == null)
            {
                sortByDisplayMemberCols = new System.Collections.Generic.Dictionary<string, object[]>();
            }

            foreach (GridColumnDescriptor column in Columns)
            {
                if (column.SortByDisplayMember)
                {
                    if (sortByDisplayMemberCols.ContainsKey(column.MappingName))
                    {
                        continue;
                    }

                    GridTableCellStyleInfo style = column.Appearance.AnyRecordFieldCell;
                    if ((style.DataSource != null || style.ChoiceList != null) &&
                        !string.IsNullOrEmpty(style.DisplayMember) && !string.IsNullOrEmpty(style.ValueMember))
                    {
                        sortByDisplayMemberCols.Add(column.MappingName, new object[] { new GridComboBoxListBoxHelper(), style });
                    }
                }
                else if (sortByDisplayMemberCols.ContainsKey(column.MappingName))
                {
                    sortByDisplayMemberCols[column.MappingName] = null;
                    sortByDisplayMemberCols.Remove(column.MappingName);
                }
            }

            if (sortByDisplayMemberCols.Count < 1)
            {
                sortByDisplayMemberCols = null;
            }
        }

#if ASPNET
//        void IParserAccessor.AddParsedSubObject(object obj)
//        {
//            if(obj is GridExpressionFieldDescriptorCollection)
//                this.GridExpressionFields.InitializeFrom(obj as GridExpressionFieldDescriptorCollection);
//        }
        #region SortUpImage
        /// <summary>
        /// Specifies the image to column header when it's sort ascending.
        /// </summary>
        [
            Description( "Specifies the image to column header when it's sort ascending." ),
            Bindable( true ),
#if SyncfusionFramework2_0
            UrlProperty,
#endif //SyncfusionFramework2_0
            DefaultValue( string.Empty ),
            NotifyParentProperty( true ),
            Editor( typeof( Syncfusion.Web.UI.WebControls.Grid.Grouping.Design.ImageUrlEditor ), typeof( UITypeEditor ) ),
            Category( "TableDescriptors" )
        ]
        public string SortUpImage
        {
            get
            {
                string sortAsc = Engine.ParentControl.Images.SortAsc;
                return ShouldSerializeSortUpImage() ? m_sSortUpImage : Utilities.IsDesignTime() ? string.Empty : sortAsc;
            }

            set
            {
                if( Engine != null && SortUpImage != value && value != Engine.ParentControl.Images.SortAsc )
                {
#if SyncfusionFramework2_0
                    if( !string.IsNullOrEmpty( value ) )
#else        
                    if( value != null && value.Length > 0 )
#endif //SyncfusionFramework2_0
                    {
                        m_sSortUpImage = value.Replace( "~/", string.Empty );
                    }
                    else
                    {
                        ResetSortUpImage();
                    }
                }
            }
        }

        private bool ShouldSerializeSortUpImage()
        {
#if SyncfusionFramework2_0
            return !string.IsNullOrEmpty( m_sSortUpImage );
#else        
            return  m_sSortUpImage != null && m_sSortUpImage.Length > 0;
#endif //SyncfusionFramework2_0
        }

        private void ResetSortUpImage()
        {
            m_sSortUpImage = string.Empty;
        }

        private string m_sSortUpImage = string.Empty;
        #endregion
        #region SortDownImage
        /// <summary>
        /// Specifies the image to column header when it's sort descending.
        /// </summary>
        [Description( "Specifies the image to column header when it's sort descending." ),
            Bindable( true ),
#if SyncfusionFramework2_0
            UrlProperty,
#endif //SyncfusionFramework2_0
            DefaultValue( string.Empty ),
            NotifyParentProperty( true ),
            Editor( typeof( Syncfusion.Web.UI.WebControls.Grid.Grouping.Design.ImageUrlEditor ), typeof( UITypeEditor ) ),
            Category( "TableDescriptors" )]
        public string SortDownImage
        {
            get
            {
                string sortDesc = Engine.ParentControl.Images.SortDesc;
                return ShouldSerializeSortDownImage() ? m_sSortDownImage : Utilities.IsDesignTime() ? string.Empty : sortDesc;
            }

            set
            {
                if( Engine != null && SortDownImage != value && value != Engine.ParentControl.Images.SortAsc )
                {
#if SyncfusionFramework2_0
                    if( !string.IsNullOrEmpty( value ) )
#else        
                    if( value != null && value.Length > 0 )
#endif //SyncfusionFramework2_0
                    {
                        m_sSortDownImage = value.Replace( "~/", string.Empty );
                    }
                    else
                    {
                        ResetSortDownImage();
                    }
                }
            }
        }
       
        private bool ShouldSerializeSortDownImage()
        {
#if SyncfusionFramework2_0
            return !string.IsNullOrEmpty( m_sSortDownImage );
#else        
            return  m_sSortDownImage != null && m_sSortDownImage.Length > 0;
#endif //SyncfusionFramework2_0
        }
       
        private void ResetSortDownImage()
        {
            m_sSortDownImage = string.Empty;
        }
        private string m_sSortDownImage = string.Empty;
        #endregion
#endif
    }
#if ASPNET
    // Not using this because this will necessitate custom ControlBuilders for all the collection-properties (and the nested collections) in the TD. Very cumbersome.
    class TDBuilder : ControlBuilder
    {
        public TDBuilder()
        {
        }

        public override void Init(TemplateParser parser, ControlBuilder parentBuilder, Type type, string tagName, string id, IDictionary attribs)
        {
            object[] atts = typeof(GridTableDescriptor).GetCustomAttributes(typeof(ParseChildrenAttribute), true);
            atts = typeof(GridGroupingControl).GetCustomAttributes(typeof(ParseChildrenAttribute), true);
            bool parserType = type is IParserAccessor;
            bool pparserType = parentBuilder.ControlType is IParserAccessor;
            base.Init (parser, parentBuilder, type, tagName, id, attribs);
        }

        public override void AppendSubBuilder(ControlBuilder subBuilder)
        {
            base.AppendSubBuilder (subBuilder);
        }

        public override Type GetChildControlType(string tagName, IDictionary attribs)
        {
            if(tagName == "ExpressionFields")
                return typeof(GridExpressionFieldDescriptorCollection);

            //// This will be just null.
            return base.GetChildControlType(tagName, attribs);
        }

        public override bool AllowWhitespaceLiterals()
        {
            return false;
        }
    }

    [TypeConverter( typeof( DescriptorBaseConverter ) )]
    public class EditFormSettings
    {
        int m_nColumnNumber = 1;
        string m_sCaptionFormatString = "Edit details for {0}";
        string m_sCaptionDataField = string.Empty;
        TableEditMode m_temFormEditMode = TableEditMode.Normal;

        [ DefaultValue( 1 ) ]
        [NotifyParentProperty(true)]
        [Description( "Number of columns in form." )]
        public int ColumnNumber
        {
            get
            {
                return m_nColumnNumber;
            }
            
            set
            {
                m_nColumnNumber = value;
            }
        }

        [ DefaultValue( "Edit details for {0}" ) ]
        [NotifyParentProperty( true )]
        [Description( "The format string for the form caption." )]
        public string CaptionFormatString
        {
            get
            {
                return m_sCaptionFormatString;
            }
            
            set
            {
                m_sCaptionFormatString = value;
            }
        }

        [ DefaultValue( string.Empty ) ]
        [NotifyParentProperty( true )]
        [Description( "Name of column which value is used in CaptionFormatString property." )]
        public string CaptionDataField
        {
            get
            {
                return m_sCaptionDataField;
            }

            set
            {
                m_sCaptionDataField = value;
            }
        }

        [ DefaultValue( TableEditMode.Normal ) ]
        [NotifyParentProperty( true )]
        [Description( "Specifies record edited mode." )]
        public TableEditMode FormEditMode
        {
            get
            {
                return m_temFormEditMode;
            }

            set
            {
                m_temFormEditMode = value;
            }
        }
    }
#endif
}
