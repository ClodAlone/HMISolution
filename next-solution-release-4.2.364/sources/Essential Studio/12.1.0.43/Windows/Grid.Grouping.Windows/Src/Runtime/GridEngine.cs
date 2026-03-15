//-------------------------------------------------------------------------------------------------
// <copyright file="GridEngine.cs" company="syncfusion">
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
using System.Xml.Serialization;
using System.Xml;
using System.IO;

using Syncfusion.Collections;
using Syncfusion.ComponentModel;
using Syncfusion.Diagnostics;
using Syncfusion.Drawing;
using Syncfusion.Grouping;
using Syncfusion.Windows.Forms.Grid;

using ChildTable = Syncfusion.Grouping.ChildTable;

using ISummary = Syncfusion.Collections.BinaryTree.ITreeTableSummary;

using Table = Syncfusion.Grouping.Table;

#if ASPNET
using System.Web.UI.WebControls;
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
    /// The TableDescriptor is browsable. You can modify its collections and properties in the
    /// designer. <para/>
    /// By default, TableDescriptor is auto-populated. If you do not modify its settings and
    /// later change the datasource, it will be automatically reinitialized. If you have
    /// made modifications to the TableDescriptor and changed the SourceList, the modifications
    /// will be kept. To discard modifications of a TableDescriptor, you need to explicitly
    /// call ResetTableDescriptor.<para/>
    /// The Table is dependent on information provided by the TableDescriptor and the records from
    /// SourceList. It is created on the fly and can not be designed with designer.
    /// <para/>
    /// The GridEngine class adds the plumbing for displaying the data in a <see cref="GridGroupingControl"/>.
    /// You can specify the datasource using the DataSource and DataMember
    /// properties through the designer.
    /// </remarks>
    [ToolboxItem(false)]
    public class GridEngine : GridEngineBase, IGridTableCellAppearanceSource, IGridGroupOptionsSource, IGridTableOptionsSource, ISupportInitialize
    {
        /// <summary>
        /// Use the following code instead:
        /// bool displayCheckBoxForBooleanFields = false;
        ///    if (displayCheckBoxForBooleanFields)
        ///    {
        ///        GridPropertyTypeDefaultStyle booleanDefault = this.gridGroupingControl1.Engine.PropertyTypeDefaultStyles["System.Boolean"];
        ///        booleanDefault.Style.CellType = "CheckBox";
        ///        booleanDefault.Style.HorizontalAlignment = GridHorizontalAlignment.Center;
        ///        booleanDefault.AllowDropDown = false;
        ///    }
        /// </summary>
        internal static bool DisplayCheckBoxForBooleanFields = true; ////false;

        private bool allowCacheStyles = false;

        bool invalidateAllWhenListChanged = true;

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
                if (!invalidateAllWhenListChangedModified && this.UseDefaultsForFasterDrawing)
                {
                    return false;
                }

                return invalidateAllWhenListChanged;
            }

            set
            {
                invalidateAllWhenListChanged = value;
                invalidateAllWhenListChangedModified = true;
            }
        }

        /// <summary>
        /// Determines whether the value of the InvalidateAllWhenListChanged property was modified.
        /// </summary>
        /// <returns>True if it is modified.</returns>
        public bool ShouldSerializeInvalidateAllWhenListChanged()
        {
            return invalidateAllWhenListChangedModified;
        }

        /// <summary>
        /// Discards any changes for the InvalidateAllWhenListChanged property.
        /// </summary>
        public void ResetInvalidateAllWhenListChanged()
        {
            invalidateAllWhenListChangedModified = false;
        }

        bool invalidateAllWhenListChangedModified = false;

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
#if ASPNET
                return this.Table.TableModel;
#else
                if (ParentControl != null && ParentControl.TableControl!=null)
                {
                    return ParentControl.TableControl.Model;
                }

                return null;
#endif
            }
        }

        [NonSerialized]
        private bool groupDropArea = false;

        /// <summary>
        /// Used internally. Gets/sets the visibility of GroupDropArea from ParentControl.
        /// </summary>
        [Browsable(false)]
        [Bindable(false)]
        public bool GroupDropArea
        {
            get
            {
                return this.groupDropArea;
            }
            set
            {
                if (this.groupDropArea != value)
                {
                    this.groupDropArea = value;
                    if (this.GroupingControl != null)
                        this.GroupingControl.ShowGroupDropArea = this.groupDropArea;
                }
            }
        }

        /// <summary>
        /// Used internally.
        /// </summary>
        /// <param name="value"></param>
        internal void InternalSetGroupDropArea(bool value)
        {
            if (this.groupDropArea != value)
                this.groupDropArea = value;
        }


        #region Construct
        /// <summary>
        /// Initializes a new engine.
        /// </summary>
        public GridEngine()
        {
            ////            TraceUtil.TraceCurrentMethodInfo();
            ////            TraceUtil.TraceCalledFrom(10);

            Appearance.Changed += new GridTableCellStyleInfoChangedEventHandler(Appearance_Changed);
            Appearance.Changing += new GridTableCellStyleInfoChangedEventHandler(Appearance_Changing);
            NestedTableGroupOptions.Changed += new Syncfusion.Styles.StyleChangedEventHandler(NestedTableGroupOptions_Changed);
            NestedTableGroupOptions.Changing += new Syncfusion.Styles.StyleChangedEventHandler(NestedTableGroupOptions_Changing);
            ChildGroupOptions.Changed += new Syncfusion.Styles.StyleChangedEventHandler(GroupOptions_Changed);
            ChildGroupOptions.Changing += new Syncfusion.Styles.StyleChangedEventHandler(ChildGroupOptions_Changing);
            PropertyTypeDefaultStyles.Changed += new Syncfusion.Collections.ListPropertyChangedEventHandler(PropertyTypeDefaultStyles_Changed);
            PropertyTypeDefaultStyles.Changing += new Syncfusion.Collections.ListPropertyChangedEventHandler(PropertyTypeDefaultStyles_Changing);
            TopLevelGroupOptions.Changed += new Syncfusion.Styles.StyleChangedEventHandler(TopLevelGroupOptions_Changed);
            TopLevelGroupOptions.Changing += new Syncfusion.Styles.StyleChangedEventHandler(TopLevelGroupOptions_Changing);
            this.engineDefaultTableOptions.Changing += new Syncfusion.Styles.StyleChangedEventHandler(engineDefaultTableOptions_Changing);
            this.engineDefaultTableOptions.Changed += new Syncfusion.Styles.StyleChangedEventHandler(engineDefaultTableOptions_Changed);

            //// DefaultBaseStyles don't get serialized neither to Xml nor Code.
            DefaultBaseStyles.Add(BlinkIncreased);
            DefaultBaseStyles.Add(BlinkReduced);
            DefaultBaseStyles.Add(BlinkNewValue);
            DefaultBaseStyles.Add(BlinkNullValue);
            DefaultBaseStyles.Add(BlinkNewRecord);

            DefaultBaseStyles[BlinkIncreased].StyleInfo.BackColor = Color.FromArgb(238, 122, 3); //// dark orange
            DefaultBaseStyles[BlinkIncreased].StyleInfo.TextColor = Color.White; //// dark orange
            DefaultBaseStyles[BlinkReduced].StyleInfo.BackColor = Color.FromArgb(102, 110, 152); //// light blue
            DefaultBaseStyles[BlinkReduced].StyleInfo.TextColor = Color.White;
            DefaultBaseStyles[BlinkNewValue].StyleInfo.BackColor = Color.White;
            DefaultBaseStyles[BlinkNewValue].StyleInfo.TextColor = Color.Black;
            DefaultBaseStyles[BlinkNullValue].StyleInfo.BackColor = Color.White;
            DefaultBaseStyles[BlinkNullValue].StyleInfo.TextColor = Color.FromArgb(238, 122, 3); //// dark orange
            DefaultBaseStyles[BlinkNewRecord].StyleInfo.BackColor = Color.White;
            DefaultBaseStyles[BlinkNewRecord].StyleInfo.TextColor = Color.FromArgb(102, 110, 152); //// light blue
        }

        /// <summary>
        /// Adds base styles that to the <see cref="BaseStyles"/> collection that allow customization
        /// of appearance of blinking cells.
        /// </summary>
        public void AddBaseStylesForBlinking()
        {
            if (!BaseStyles.Contains(BlinkIncreased))
            {
                BaseStyles.Add(BlinkIncreased);
            }

            if (!BaseStyles.Contains(BlinkReduced))
            {
                BaseStyles.Add(BlinkReduced);
            }

            if (!BaseStyles.Contains(BlinkNewValue))
            {
                BaseStyles.Add(BlinkNewValue);
            }

            if (!BaseStyles.Contains(BlinkNullValue))
            {
                BaseStyles.Add(BlinkNullValue);
            }

            if (!BaseStyles.Contains(BlinkNewRecord))
            {
                BaseStyles.Add(BlinkNewRecord);
            }
        }

        /// <summary>
        /// Default value for BlinkIncreased state.
        /// </summary>
        public static readonly string BlinkIncreased = "BlinkIncreased";
        /// <summary>
        /// Default value for BlinkReduced state.
        /// </summary>
        public static readonly string BlinkReduced = "BlinkReduced";
        /// <summary>
        /// Default value for BlinkNewValue state.
        /// </summary>
        public static readonly string BlinkNewValue = "BlinkNewValue";
        /// <summary>
        /// Default value for BlinkNullValue state.
        /// </summary>
        public static readonly string BlinkNullValue = "BlinkNullValue";
        /// <summary>
        /// Default value for BlinkNewRecord state.
        /// </summary>
        public static readonly string BlinkNewRecord = "BlinkNewRecord";
        /// <summary>
        /// 
        /// </summary>
        public override void Reset()
        {
            ResetAllowResetSourceListWhenDataSourceChanged();
            ResetAllowResetTableDescriptorWhenDataSourceSetNull();
            ResetAppearance();
            ResetBaseStyles();
            ResetCacheRecordValues();
            ResetChildGroupOptions();
            ResetCultureInfo();
            ResetDefaultAppearance();
            ResetInvalidateAllWhenListChanged();
            ResetMaxNestedCollectionRecurseLevel();
            ResetMaxNestedFieldRecurseLevel();
            ResetNestedTableGroupOptions();
            ResetNewTableOptionsDefaultSettings();
            ResetTable();
            ResetTableDescriptor();
            ResetTableDirtyOnSourceListReset();
            ResetTopLevelGroupOptions();
            ResetUseLazyUniformChildListRelation();
            ResetUseOldListChangedHandler();
            ResetUseOldUniformChildListRelation();
            base.Reset();
        }
        /// <override/>
        protected override void Dispose(bool disposing)
        {
            ////TraceUtil.TraceCurrentMethodInfo();
            ////TraceUtil.TraceCalledFrom(10);

            if (disposing)
            {
                BaseStyles.Changing -= new ListPropertyChangedEventHandler(baseStyles_Changing);
                BaseStyles.Changed -= new ListPropertyChangedEventHandler(baseStyles_Changed);
                Appearance.Changed -= new GridTableCellStyleInfoChangedEventHandler(Appearance_Changed);
                Appearance.Changing -= new GridTableCellStyleInfoChangedEventHandler(Appearance_Changing);
                NestedTableGroupOptions.Changed -= new Syncfusion.Styles.StyleChangedEventHandler(NestedTableGroupOptions_Changed);
                NestedTableGroupOptions.Changing -= new Syncfusion.Styles.StyleChangedEventHandler(NestedTableGroupOptions_Changing);
                ChildGroupOptions.Changed -= new Syncfusion.Styles.StyleChangedEventHandler(GroupOptions_Changed);
                ChildGroupOptions.Changing -= new Syncfusion.Styles.StyleChangedEventHandler(ChildGroupOptions_Changing);
                PropertyTypeDefaultStyles.Changed -= new Syncfusion.Collections.ListPropertyChangedEventHandler(PropertyTypeDefaultStyles_Changed);
                PropertyTypeDefaultStyles.Changing -= new Syncfusion.Collections.ListPropertyChangedEventHandler(PropertyTypeDefaultStyles_Changing);
                TopLevelGroupOptions.Changed -= new Syncfusion.Styles.StyleChangedEventHandler(TopLevelGroupOptions_Changed);
                TopLevelGroupOptions.Changing -= new Syncfusion.Styles.StyleChangedEventHandler(TopLevelGroupOptions_Changing);
                this.engineDefaultTableOptions.Changing -= new Syncfusion.Styles.StyleChangedEventHandler(engineDefaultTableOptions_Changing);
                this.engineDefaultTableOptions.Changed -= new Syncfusion.Styles.StyleChangedEventHandler(engineDefaultTableOptions_Changed);
                UnwireTableDescriptor();
                ////                this.parentControl = null;
                this.engineDefaultStyle.Dispose();
                ////                engineDefaultStyle = null;
                this.engineDefaultTableOptions.Dispose();
                ////                engineDefaultTableOptions = null;
                this.topLevelGroupOptions.Dispose();
                ////                topLevelGroupOptions = null;
                if (tableOptions != null)
                {
                    this.tableOptions.Dispose();
                }

                ////                tableOptions = null;
                if (appearance != null)
                {
                    this.appearance.Dispose();
                }

                ////                appearance = null;
                if (baseStyles != null)
                {
                    this.baseStyles.Dispose();
                }

                ////                baseStyles = null;
                if (childTableGroupOptions != null)
                {
                    this.childTableGroupOptions.Dispose();
                }

                if (propertyTypeDefaultStyles != null)
                {
                    this.propertyTypeDefaultStyles.Dispose();
                }

                if (groupOptions != null)
                {
                    this.groupOptions.Dispose();
                }
                ////                groupOptions = null;
            }

            base.Dispose(disposing);
        }

        #endregion

        #region Initialize
        /// <override/>
        /// <summary>Initializes this object and copies properties from another object.</summary>
        /// <param name="source">The source object.</param>
        public override void InitializeFrom(Engine source)
        {
            ////Table.LockOutEnsureInitialized = true;

            base.InitializeFrom(source);

            if (source is GridEngine)
            {
                GridEngine other = (GridEngine)source;

                if (other.ShouldSerializeAppearance())
                {
                    Appearance.InitializeFrom(other.Appearance);
                }
                else
                {
                    ResetAppearance();
                }

                if (other.ShouldSerializeBaseStyles())
                {
                    BaseStyles.InitializeFrom(other.BaseStyles);
                }
                else
                {
                    ResetBaseStyles();
                }

                if (other.ShouldSerializeChildGroupOptions())
                {
                    ChildGroupOptions.CopyFrom(other.ChildGroupOptions);
                }
                else
                {
                    ResetChildGroupOptions();
                }

                if (other.ShouldSerializeNestedTableGroupOptions())
                {
                    NestedTableGroupOptions.CopyFrom(other.NestedTableGroupOptions);
                }
                else
                {
                    ResetNestedTableGroupOptions();
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

                if (other.ShouldSerializeInvalidateAllWhenListChanged())
                {
                    this.InvalidateAllWhenListChanged = other.InvalidateAllWhenListChanged;
                }
                else
                {
                    ResetInvalidateAllWhenListChanged();
                }

                this.AllowCacheStyles = other.AllowCacheStyles;
                this.GroupDropArea = other.GroupDropArea;
                this.UseDefaultsForFasterDrawing = other.UseDefaultsForFasterDrawing;
                this.UpdateDisplayFrequency = other.UpdateDisplayFrequency;
                this.InsertRemoveBehavior = other.InsertRemoveBehavior;
                this.InsertRemoveBehaviorWithEndEdit = other.InsertRemoveBehaviorWithEndEdit;
                this.SortPositionChangedBehavior = other.SortPositionChangedBehavior;
                this.SortPositionChangedBehaviorWithEndEdit = other.SortPositionChangedBehaviorWithEndEdit;
                this.BlinkTime = other.BlinkTime;
                this.MarkSortedColumnsDirtyWhenSortedPositionChanged = other.MarkSortedColumnsDirtyWhenSortedPositionChanged;
                this.AllowResetTableDescriptorWhenDataSourceSetNull = other.AllowResetTableDescriptorWhenDataSourceSetNull;
                this.UseLazyUniformChildListRelation = other.UseLazyUniformChildListRelation;

                ////Table.LockOutEnsureInitialized = false;
            }
        }

        #endregion

        #region Design support
        /// <override/>
        /// <summary>
        /// Determines whether the engine is attached to the control that is currently being designed in VisualStudio designer.
        /// </summary>
        /// <returns>True if it is in design mode.</returns>
        public override bool GetDesignMode()
        {
#if EMUDESIGN
                return true;
#endif

            if (this.DesignMode)
            {
                return true;
            }

            if (parentControl != null)
            {
                if (parentControl.Site != null)
                {
                    return parentControl.Site.DesignMode;
                }
            }

            return false;
        }

        /// <override/>
        /// <summary>Gets the service object of the specified type.</summary>
        /// <param name="service">The type of service object to get.</param>
        /// <returns>Service object.</returns>
        public override object GetService(Type service)
        {
            object obj = base.GetService(service);
            if (obj == null && ParentControl != null)
            {
                obj = ((IComponent)ParentControl).Site.GetService(service);
            }

            return obj;
        }

        #endregion

        #region Parent Control
        internal GridGroupingControl parentControl;

        /// <summary>
        /// The <see cref="GridGroupingControl"/> that hosts this engine.
        /// </summary>
        [System.Xml.Serialization.XmlIgnore]
        [DesignerSerializationVisibilityAttribute(DesignerSerializationVisibility.Hidden)]
        [Browsable(false)]
        public GridGroupingControl ParentControl
        {
            get
            {
                return parentControl;
            }
        }

        internal GridTableCellStyleInfo engineDefaultStyle = new GridTableCellStyleInfo();

        internal GridTableOptionsStyleInfo engineDefaultTableOptions = new GridTableOptionsStyleInfo();

        /// <summary>
        /// Internal helper routine that helps attaching the engine to a parent GridGroupingControl
        /// when multiple GridGroupingControls share the same engine object. This event lets you
        /// choose the one GridGroupingControl that should be treated as the main control for this engine.
        /// </summary>
        /// <param name="parentControl">The parent control.</param>
        public void SetParentControl(GridGroupingControl parentControl)
        {
            this.parentControl = parentControl;
            if (parentControl != null)
            {
#if ASPNET
                DefaultAppearance.AnyCell.Font.Facename = parentControl.Font.Name;
                DefaultAppearance.AnyCell.Font.FontStyle = this.GetFontStyleFromFontInfo(parentControl.Font);

                engineDefaultStyle.BackColor = Color.Empty;
#else
                DefaultAppearance.AnyCell.Font.Facename = parentControl.Font.FontFamily.Name;
                DefaultAppearance.AnyCell.Font.FontStyle = parentControl.Font.Style;
                DefaultAppearance.AnyCell.Font.Size = parentControl.Font.Size;
                DefaultAppearance.AnyCell.Font.Unit = parentControl.Font.Unit;
#endif

                DefaultAppearance.AnyCell.TextColor = parentControl.ForeColor;
#if ASPNET
#else
                DefaultAppearance.AnyRecordFieldCell.BackColor = parentControl.BackColor;
                DefaultAppearance.AnySummaryCell.BackColor = parentControl.BackColor;
                DefaultAppearance.AnyIndentCell.BackColor = parentControl.BackColor;
                DefaultAppearance.AnyPreviewCell.BackColor = parentControl.BackColor;
                DefaultAppearance.EmptyCell.BackColor = parentControl.BackColor;
                UpdateTableOptionsDefault(parentControl.Font);
#endif
            }

            ForwardTableEvents = parentControl;
        }

#if ASPNET
        private FontStyle GetFontStyleFromFontInfo(System.Web.UI.WebControls.FontInfo fi)
        {
            FontStyle style = FontStyle.Regular;
            if(fi.Strikeout)
                style |= FontStyle.Strikeout;
            if(fi.Underline)
                style |= FontStyle.Underline;
            if(fi.Bold)
                style |= FontStyle.Bold;
            if(fi.Italic)
                style |= FontStyle.Italic;

            return style;
        }
#endif

#if ASPNET
        // In ASPNET the font size is not known - it is interpretted at the client side.
#else
        internal void UpdateTableOptionsDefault(Font font)
        {
            float fontHeight = font.GetHeight();
            float cmpHeight = 12.5f;
            int diff = (int)Math.Ceiling(fontHeight - cmpHeight);

            engineDefaultTableOptions.RecordPreviewRowHeight = (18 + diff) * 2;
            engineDefaultTableOptions.RecordRowHeight = 22 + diff;
            engineDefaultTableOptions.CaptionRowHeight = 22 + diff;
            engineDefaultTableOptions.ColumnHeaderRowHeight = 25 + diff;
            engineDefaultTableOptions.EmptySectionHeight = 10;
            engineDefaultTableOptions.GroupFooterSectionHeight = 10;
            engineDefaultTableOptions.GroupPreviewSectionHeight = (18 + diff) * 2;
            engineDefaultTableOptions.GroupHeaderSectionHeight = 10;
            engineDefaultTableOptions.FilterBarRowHeight = -1;
            engineDefaultTableOptions.SummaryRowHeight = -1;
            engineDefaultTableOptions.IndentWidth = 18 + diff;
            engineDefaultTableOptions.RowHeaderWidth = 18 + diff;
        }
#endif
        #endregion

        #region Table and TableDescriptor
        /// <override/>
        /// <summary>Creates a <see cref="Table"/>.</summary>
        /// <param name="tableDescriptor">The table descriptor.</param>
        /// <param name="parentRelationTable">Related parent table.</param>
        /// <returns>The new table.</returns>
        public override Table CreateTable(TableDescriptor tableDescriptor, Table parentRelationTable)
        {
            if (Syncfusion.Grouping.Engine.HelpTracing)
            {
                TraceUtil.TraceCurrentMethodInfo(tableDescriptor, parentRelationTable);
                TraceUtil.TraceCalledFrom();
            }

            return new GridTable((GridTableDescriptor)tableDescriptor, (GridTable)parentRelationTable);
        }

        /// <override/>
        /// <summary>Creates a <see cref="TableDescriptor"/>.</summary>
        /// <param name="parentRelation">Parent relation descriptor.</param>
        /// <returns>The new table descriptor.</returns>
        public override TableDescriptor CreateTableDescriptor(RelationDescriptor parentRelation)
        {
            if (Syncfusion.Grouping.Engine.HelpTracing)
            {
                TraceUtil.TraceCurrentMethodInfo(parentRelation);
                TraceUtil.TraceCalledFrom();
            }

            return new GridTableDescriptor(this, parentRelation);
        }

        /// <override/>
        /// <summary>Returns the main table descriptor or a table descriptor that matches with the given name.</summary>
        /// <param name="name">Descriptor name.</param>
        /// <returns>Table descriptor.</returns>
        public new GridTableDescriptor GetTableDescriptor(string name)
        {
            return (GridTableDescriptor)base.GetTableDescriptor(name);
        }

        /// <summary>
        /// Maintains the table schema information of the root table in the datasource.
        /// </summary>
        [Browsable(true), DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
        [Category("TableDescriptors")]
        [RefreshPropertiesAttribute(RefreshProperties.All)]
        public new GridTableDescriptor TableDescriptor
        {
            get
            {
                return (GridTableDescriptor)base.TableDescriptor;
            }

            set
            {
                TableDescriptor.InitializeFrom(value);
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
        public new GridTable Table
        {
            get
            {
                return (GridTable)base.Table;
            }
        }
#if ASPNET
#else
        /// <summary>
        /// Gets the main <see cref="GridTableControl"/>. The GridTableControl is a grid derived from
        /// GridControlBase and displays and allows user interaction and modification of data.
        /// </summary>
        /// <remarks>
        /// This control is either a child control of the <see cref="GridGroupingControl.GridTablePanel"/>
        /// or <see cref="RecordNavigationControl"/> depending on whether the <see cref="GridGroupingControl.ShowNavigationBar"/> is set.
        /// </remarks>
        [System.Xml.Serialization.XmlIgnore]
        [DesignerSerializationVisibilityAttribute(DesignerSerializationVisibility.Hidden)]
        [Browsable(false)]
        public GridTableControl TableControl
        {
            get
            {
                ////is GridGroupingControl)
                if (ParentControl != null)
                {
                    return ((GridGroupingControl)ParentControl).TableControl;
                }

                return null;
            }
        }

        /// <override/>
        protected override void OnBindingContextChanged(EventArgs e)
        {
            if (this.ShouldSerializeTable())
            {
                Table.BindingContext = BindingContext;
            }

            base.OnBindingContextChanged(e);
        }
#endif
        /// <summary>
        /// Gets the <see cref="GridGroupingControl"/> that hosts this engine.
        /// </summary>
        [System.Xml.Serialization.XmlIgnore]
        [DesignerSerializationVisibilityAttribute(DesignerSerializationVisibility.Hidden)]
        [Browsable(false)]
        public GridGroupingControl GroupingControl
        {
            get
            {
                return ParentControl as GridGroupingControl;
            }
        }

        /// <override/>
        protected override void OnTableDescriptorCreated(EventArgs e)
        {
            base.OnTableDescriptorCreated(e);
            WireTableDescriptor();
        }

        void WireTableDescriptor()
        {
            TableDescriptor.PropertyChanging += new DescriptorPropertyChangedEventHandler(TableDescriptor_PropertyChanging);
            TableDescriptor.PropertyChanged += new DescriptorPropertyChangedEventHandler(TableDescriptor_PropertyChanged);
        }

        void UnwireTableDescriptor()
        {
            TableDescriptor.PropertyChanging -= new DescriptorPropertyChangedEventHandler(TableDescriptor_PropertyChanging);
            TableDescriptor.PropertyChanged -= new DescriptorPropertyChangedEventHandler(TableDescriptor_PropertyChanged);
        }
        #endregion

        #region BaseStylesMap

        GridTableBaseStyleCollection baseStyles;

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
                if (baseStyles == null)
                {
                    baseStyles = new GridTableBaseStyleCollection(this);
                    baseStyles.Changing += new ListPropertyChangedEventHandler(baseStyles_Changing);
                    baseStyles.Changed += new ListPropertyChangedEventHandler(baseStyles_Changed);
                }

                return baseStyles;
            }

            set
            {
                if (value != null)
                {
                    BaseStyles.InitializeFrom(value);
                }
                else
                {
                    ResetBaseStyles();
                }
            }
        }

        /// <summary>
        /// Determines whether the <see cref="BaseStyles"/> collection was modified.
        /// </summary>
        /// <returns>True if the collection was modified.</returns>
        public bool ShouldSerializeBaseStyles()
        {
            return baseStyles != null && baseStyles.Count > 0;
        }

        /// <summary>
        /// Resets the <see cref="BaseStyles"/> property.
        /// </summary>
        public void ResetBaseStyles()
        {
            if (baseStyles != null && baseStyles.Count > 0)
            {
                BaseStyles.Clear();
            }
        }

        private void baseStyles_Changing(object sender, ListPropertyChangedEventArgs e)
        {
            this.OnPropertyChanging(new DescriptorPropertyChangedEventArgs("BaseStyles", e));
        }

        private void baseStyles_Changed(object sender, ListPropertyChangedEventArgs e)
        {
            this.OnPropertyChanged(new DescriptorPropertyChangedEventArgs("BaseStyles", e));
        }

        GridTableBaseStyleCollection defaultBaseStyles;

        /// <summary>
        /// Maintains a collection of default base styles which are not serialized to xml or code. End users
        /// can override the base styles by manually adding a base style with the same name to the BaseStyles 
        /// collection.
        /// </summary>
        [Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        [XmlIgnore]
        public GridTableBaseStyleCollection DefaultBaseStyles
        {
            get
            {
                if (defaultBaseStyles == null)
                {
                    defaultBaseStyles = new GridTableBaseStyleCollection(this);
                }

                return defaultBaseStyles;
            }
        }

        #endregion

        #region Appearance
        GridTableCellAppearance appearance;

        /// <summary>
        /// The default <see cref="GridTableCellAppearance"/> with default <see cref="GridTableCellStyleInfo"/>
        /// information for all cell elements in the control. This property lets you control almost any aspect of
        /// the appearance of the grouping grid like cell backcolor, font, or the cell type.
        /// </summary>
        [Category("Look and Feel")]
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
                ((IGridTableCellAppearanceSource)this).RaiseAppearanceChanging(null);
                appearance.Reset();
                ((IGridTableCellAppearanceSource)this).RaiseAppearanceChanged(null);
            }
        }

        #endregion

        #region IGridTableCellAppearanceSource Members

        GridTableCellAppearance IGridTableCellAppearanceSource.GetAppearance()
        {
            return Appearance;
        }

        [System.Xml.Serialization.XmlIgnore]
        [DesignerSerializationVisibilityAttribute(DesignerSerializationVisibility.Hidden)]
        [Browsable(false)]
        internal bool HasAppearance
        {
            get { return ShouldSerializeAppearance(); }
        }

        void IGridTableCellAppearanceSource.RaiseAppearanceChanged(GridTableCellStyleInfoChangedEventArgs e)
        {
            ////            if (this.ShouldSerializeTable())
            ////                this.Table.TableDirty = true;
            this.OnPropertyChanged(new DescriptorPropertyChangedEventArgs("Appearance", e));
        }

        void IGridTableCellAppearanceSource.RaiseAppearanceChanging(GridTableCellStyleInfoChangedEventArgs e)
        {
            this.OnPropertyChanging(new DescriptorPropertyChangedEventArgs("Appearance", e));
        }

        GridTableCellAppearance IGridTableCellAppearanceSource.GetBaseAppearance()
        {
            return null; //// defaultAppearanceSource != null ? defaultAppearanceSource.Appearance : null; 
        }

        GridEngine IGridTableCellAppearanceSource.Engine
        {
            get { return null; }
        }

        GridDefaultAppearanceSource defaultAppearanceSource = null;

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
                if (defaultAppearanceSource == null)
                {
                    defaultAppearanceSource = new GridDefaultAppearanceSource();
                }

                return defaultAppearanceSource.Appearance;
            }
        }

        /// <summary>
        /// Discards any changes for the <see cref="Appearance"/> object.
        /// </summary>    
        public void ResetDefaultAppearance()
        {
            this.defaultAppearanceSource = null;
        }

        /// <exclude/>
        /// <summary>Used internally.</summary>
        [System.Xml.Serialization.XmlIgnore]
        [DesignerSerializationVisibilityAttribute(DesignerSerializationVisibility.Hidden)]
        [Browsable(false)]
        public IGridTableCellAppearanceSource InternalDefaultAppearanceSource
        {
            get
            {
                return defaultAppearanceSource;
            }
        }
        #endregion

        #region Factory Methods

#if ASPNET
#else
        /// <summary>
        /// Virtual method to create a <see cref="TableControl"/>.
        /// </summary>
        /// <param name="model">The <see cref="GridTableModel"/>.</param>
        /// <returns>The table control.</returns>
        public virtual GridTableControl CreateTableControl(GridTableModel model)
        {
            return new GridTableControl(model);
        }

        /// <summary>
        /// Virtual method to create a <see cref="GridNestedTableControl"/>.
        /// </summary>
        /// <param name="relatedTableModel">the table model.</param>
        /// <param name="parentGrid">The parent control.</param>
        /// <param name="parentRenderer">The GridNestedTableControlCellRenderer which hosts this control.</param>
        /// <returns>The table control.</returns>
        public virtual GridNestedTableControl CreateNestedTableControl(GridTableModel relatedTableModel, GridTableControl parentGrid, GridNestedTableControlCellRenderer parentRenderer)
        {
            return new GridNestedTableControl(relatedTableModel, parentGrid, parentRenderer);
        }

        /// <summary>
        /// Virtual method to create the <see cref="RecordNavigationControl"/>.
        /// </summary>
        /// <returns>The RecordNavigationControl.</returns>
        public virtual RecordNavigationControl CreateRecordNavigationControl()
        {
            return new RecordNavigationControl();
        }

        /// <summary>
        /// Virtual method to create the <see cref="GridGroupDropArea"/>.
        /// </summary>
        /// <param name="tableControl">The table control.</param>
        /// <param name="groupDropAreaModel">The GroupDropArea model.</param>
        /// <returns>The new group drop area.</returns>
        public virtual GridGroupDropArea CreateGroupDropArea(GridTableControl tableControl, GridGroupDropAreaModel groupDropAreaModel)
        {
            return new GridGroupDropArea(tableControl, groupDropAreaModel);
        }
#endif
        /// <summary>
        /// Virtual method to create a <see cref="GridNestedTableControlCellModel"/>.
        /// </summary>
        /// <param name="model">The table model.</param>
        /// <param name="relatedTable">The related table.</param>
        /// <returns>The GridNestedTableControlCellModel.</returns>
        public virtual GridNestedTableControlCellModel CreateNestedTableControlCellModel(GridTableModel model, GridTable relatedTable)
        {
            return new GridNestedTableControlCellModel(model, relatedTable);
        }

        /// <summary>
        /// Virtual method to create the <see cref="GridTableModel"/>.
        /// </summary>
        /// <returns>The table model used by the <see cref="TableControl"/>.</returns>
        public virtual GridTableModel CreateTableModel()
        {
            return new GridTableModel();
        }

        /// <override/>
        /// <summary>Creates a <see cref="RelationDescriptor"/>.</summary>
        /// <returns>The new relation descriptor.</returns>
        public override RelationDescriptor CreateRelationDescriptor()
        {
            return new GridRelationDescriptor();
        }

        /// <override/>
        /// <summary>
        /// Creates an AddNewRecord for the given parent.
        /// </summary>
        /// <param name="parent">Parent section.</param>
        /// <returns>The new add new record.</returns>
        public override AddNewRecord CreateAddNewRecord(AddNewRecordSection parent)
        {
            return new GridAddNewRecord(parent);
        }

        /// <override/>
        /// <summary>
        /// Creates a CaptionSection for the given element.
        /// </summary>
        /// <param name="parent">Parent group.</param>
        /// <returns>The new caption section.</returns>
        public override CaptionSection CreateCaptionSection(Group parent)
        {
            return new GridCaptionSection(parent);
        }

        /// <override/>
        /// <summary>
        /// Creates a <see cref="ChildTable"/>.
        /// </summary>
        /// <param name="parent">The parent element.</param>
        /// <returns>The new child table.</returns>
        public override ChildTable CreateChildTable(Element parent)
        {
            return new GridChildTable(parent);
        }

        /// <override/>
        /// <summary>
        /// Creates a <see cref="ColumnHeaderRow"/>.
        /// </summary>
        /// <param name="parent">The parent section.</param>
        /// <returns>The new column header row.</returns>
        public override ColumnHeaderRow CreateColumnHeaderRow(ColumnHeaderSection parent)
        {
            return new GridColumnHeaderRow(parent);
        }

        /// <override/>
        /// <summary>
        /// Creates a CaptionRow for the given element.
        /// </summary>
        /// <param name="parent">Parent caption section.</param>
        /// <returns>The new caption row.</returns>
        public override CaptionRow CreateCaptionRow(CaptionSection parent)
        {
            if (allowCacheStyles)
            {
                return new GridCaptionRowWithCache(parent);
            }

            return new GridCaptionRow(parent);
        }

        /// <override/>
        /// <summary>
        /// Creates a <see cref="FilterBarRow"/>.
        /// </summary>
        /// <param name="parent">The parent section.</param>
        /// <returns>The new filter bar row.</returns>
        public override FilterBarRow CreateFilterBarRow(FilterBarSection parent)
        {
            return new GridFilterBarRow(parent);
        }

        /// <override/>
        /// <summary>
        /// Creates an AddNewRecordSection for the given parent.
        /// </summary>
        /// <param name="parent">Parent group.</param>
        /// <returns>The new add new record section.</returns>
        public override AddNewRecordSection CreateAddNewRecordSection(Group parent)
        {
            return new GridAddNewRecordSection(parent);
        }

        /// <summary>
        /// Creates a <see cref="GridStackedHeaderSection"/> element.
        /// </summary>
        /// <param name="parent">The parent group.</param>
        /// <returns>The new element.</returns>
        public virtual Section CreateStackedHeaderSection(Group parent)
        {
            return new GridStackedHeaderSection(parent);
        }

        /// <override/>
        /// <summary>
        /// Creates a <see cref="ColumnHeaderSection"/>.
        /// </summary>
        /// <param name="parent">The parent group.</param>
        /// <returns>The new column header section.</returns>
        public override ColumnHeaderSection CreateColumnHeaderSection(Group parent)
        {
            return new GridColumnHeaderSection(parent);
        }

        /// <override/>
        /// <summary>
        /// Creates an <see cref="EmptySection"/>.
        /// </summary>
        /// <param name="parent">The parent group.</param>
        /// <returns>The new empty section.</returns>
        public override EmptySection CreateEmptySection(Group parent)
        {
            return new GridEmptySection(parent);
        }

        /// <override/>
        /// <summary>
        /// Creates a <see cref="FilterBarSection"/>.
        /// </summary>
        /// <param name="parent">The parent group.</param>
        /// <returns>The new filter bar section.</returns>
        public override FilterBarSection CreateFilterBarSection(Group parent)
        {
            return new GridFilterBarSection(parent);
        }

        /// <override/>
        /// <summary>
        /// Creates a <see cref="Group"/>.
        /// </summary>
        /// <param name="parent">The parent section.</param>
        /// <returns>The new group.</returns>
        public override Group CreateGroup(Section parent)
        {
            return new GridGroup(parent);
        }

        /// <override/>
        /// <summary>
        /// Creates a <see cref="GroupsDetails"/>.
        /// </summary>
        /// <param name="parent">The parent group.</param>
        /// <returns>The new details section.</returns>
        public override GroupsDetails CreateGroupsDetails(Group parent)
        {
            return new GridGroupsDetails(parent);
        }

        /// <override/>
        /// <summary>
        /// Creates a <see cref="NestedTable"/>.
        /// </summary>
        /// <param name="parent">The parent element.</param>
        /// <returns>the new nested table.</returns>
        public override NestedTable CreateNestedTable(RecordNestedTablesPart parent)
        {
            return new GridNestedTable(parent);
        }
        
        /// <summary>
        /// Creates a <see cref="Record"/>.
        /// </summary>
        /// <param name="parentTable">The parent table.</param>
        /// <returns>The new record.</returns>
        /// <override/>
        public override Record CreateRecord(Table parentTable)
        {
            if (ShouldCreateRecordWithCache)
            {
                return new GridRecordWithValueCache(parentTable);
            }

            return new GridRecord(parentTable);
        }

        /// <override/>
        /// <summary>
        /// Creates a <see cref="RecordNestedTablesPart"/>.
        /// </summary>
        /// <param name="parent">The parent record.</param>
        /// <returns>The new RecordNestedTablesPart.</returns>
        public override RecordNestedTablesPart CreateRecordNestedTablesPart(Record parent)
        {
            return new GridRecordNestedTablesPart(parent);
        }

        /// <override/>
        /// <summary>
        /// Creates a <see cref="RecordRow"/>.
        /// </summary>
        /// <param name="parent">The parent element.</param>
        /// <returns>The new record row.</returns>
        public override RecordRow CreateRecordRow(RecordRowsPart parent)
        {
            if (allowCacheStyles)
            {
                return new GridRecordRowWithCache(parent);
            }

            return new GridRecordRow(parent);
        }

        /// <override/>
        /// <summary>
        /// Creates a <see cref="RecordRowsPart"/>.
        /// </summary>
        /// <param name="parent">The parent record.</param>
        /// <returns>The new RecordRowsPart.</returns>
        public override RecordRowsPart CreateRecordRowsPart(Record parent)
        {
            return new GridRecordRowsPart(parent);
        }

        /// <override/>
        /// <summary>
        /// Creates a <see cref="CreateRecordsDetails"/>.
        /// </summary>
        /// <param name="parent">The parent group.</param>
        /// <returns>The new details section.</returns>
        public override RecordsDetails CreateRecordsDetails(Group parent)
        {
            return new GridRecordsDetails(parent);
        }

        /// <override/>
        /// <summary>
        /// Creates a <see cref="RowElementsSection"/>.
        /// </summary>
        /// <param name="parent">The parent group.</param>
        /// <returns>The new row elements section.</returns>
        public override RowElementsSection CreateRowElementsSection(Group parent)
        {
            return new GridRowElementsSection(parent);
        }

        /// <override/>
        /// <summary>
        /// Creates a <see cref="GridSummarySection"/>.
        /// </summary>
        /// <param name="parent">The parent group.</param>
        /// <returns>The new summary section.</returns>
        public override Section CreateSummarySection(Group parent)
        {
            return new GridSummarySection(parent);
        }

        /// <override/>
        /// <summary>
        /// Creates a <see cref="GroupFooterSection"/>.
        /// </summary>
        /// <param name="parent">The parent group.</param>
        /// <returns>The new group footer section.</returns>
        public override GroupFooterSection CreateGroupFooterSection(Group parent)
        {
            return new GridGroupFooterSection(parent);
        }

        /// <override/>
        /// <summary>
        /// Creates a <see cref="GroupHeaderSection"/>.
        /// </summary>
        /// <param name="parent">The parent group.</param>
        /// <returns>The new group header section.</returns>
        public override GroupHeaderSection CreateGroupHeaderSection(Group parent)
        {
            return new GridGroupHeaderSection(parent);
        }

        /// <override/>
        /// <summary>
        /// Creates a <see cref="RecordPreviewRow"/>.
        /// </summary>
        /// <param name="parent">The parent element.</param>
        /// <returns>The new record preview row.</returns>
        public override RecordPreviewRow CreateRecordPreviewRow(RecordPreviewRowsPart parent)
        {
            return new GridRecordPreviewRow(parent);
        }

        /// <override/>
        /// <summary>
        /// Creates a <see cref="GroupPreviewSection"/>.
        /// </summary>
        /// <param name="parent">Parent group.</param>
        /// <returns>The new group preview section.</returns>
        public override GroupPreviewSection CreateGroupPreviewSection(Group parent)
        {
            return new GridGroupPreviewSection(parent);
        }
        #endregion

        #region QueryCellStyleInfo event

        /// <summary>
        /// Occurs for each cell before a <see cref="GridTableControl"/>
        /// starts painting and lets users customize the display of cells.
        /// </summary>
        [Description("Occurs for each cell before a GridTableControl starts painting.")]
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
        }
        #endregion

        #region PropertyTypeDefaultStyles
        private GridPropertyTypeDefaultStyleCollection propertyTypeDefaultStyles;

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
                if (propertyTypeDefaultStyles == null)
                {
                    propertyTypeDefaultStyles = new GridPropertyTypeDefaultStyleCollection();
                    propertyTypeDefaultStyles.engine = this;
                }

                return this.propertyTypeDefaultStyles;
            }
        }
        #endregion

        #region IGridGroupOptionsSource Members
        GridGroupOptionsStyleInfo groupOptions;

        ////        [System.Xml.Serialization.XmlIgnore]
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
        /// Lets you control the look of inner groups like whether the Caption Row is visible, or what CaptionText is.
        /// </summary>
        [Category("Look and Feel")]
        [DesignerSerializationVisibilityAttribute(DesignerSerializationVisibility.Content)]
        [Browsable(true)]
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
        /// <returns>True if contents were changed; False otherwise.</returns>
        public bool ShouldSerializeChildGroupOptions()
        {
            return this.groupOptions != null && !groupOptions.IsEmpty;
        }

        /// <summary>
        /// Discards any changes for the <see cref="ChildGroupOptions"/> object.
        /// </summary>
        public void ResetChildGroupOptions()
        {
            if (ShouldSerializeChildGroupOptions())
            {
                ((IGridGroupOptionsSource)this).RaiseGroupOptionsChanging(null);
                groupOptions = null;
                ((IGridGroupOptionsSource)this).RaiseGroupOptionsChanged(null);
            }
        }

        void IGridGroupOptionsSource.RaiseGroupOptionsChanged(GridGroupOptionsChangedEventArgs e)
        {
            ////            this.BumpVersion();
            ////            if (hasTable)
            ////                Table.CountersDirty = true;
            ////            if (hasTable)
            ////                Table.RaiseDisplayElementChanged(Table, -1, -1, true, true, true);
            this.OnPropertyChanged(new DescriptorPropertyChangedEventArgs("GroupOptions", e));
        }

        void IGridGroupOptionsSource.RaiseGroupOptionsChanging(GridGroupOptionsChangedEventArgs e)
        {
            ////            if (hasTable)
            ////                Table.RaiseDisplayElementChanging(Table, -1, -1, true, true, true);
            this.OnPropertyChanging(new DescriptorPropertyChangedEventArgs("GroupOptions", e));
        }

        IGridGroupOptionsSource IGridGroupOptionsSource.GetParentGroupOptionsSource()
        {
            return null;
        }

        GridGroupOptionsStyleInfo topLevelGroupOptions;

        [System.Xml.Serialization.XmlIgnore]
        [DesignerSerializationVisibilityAttribute(DesignerSerializationVisibility.Hidden)]
        [Browsable(false)]
        internal bool HasTopLevelGroupOptionsGroupOptions
        {
            get
            {
                return topLevelGroupOptions != null;
            }
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
            if (ShouldSerializeTopLevelGroupOptions())
            {
                ((IGridGroupOptionsSource)this).RaiseGroupOptionsChanging(null);
                this.topLevelGroupOptions = null;
                ((IGridGroupOptionsSource)this).RaiseGroupOptionsChanged(null);
            }
        }

        GridGroupOptionsStyleInfo childTableGroupOptions;

        [System.Xml.Serialization.XmlIgnore]
        [DesignerSerializationVisibilityAttribute(DesignerSerializationVisibility.Hidden)]
        [Browsable(false)]
        internal bool HasChildTableGroupOptionsOptions
        {
            get
            {
                return childTableGroupOptions != null;
            }
        }

        /// <summary>
        /// Lets you control the look of the topmost group of nested tables such as whether the Caption Row is visible or what CaptionText is.
        /// </summary>
        [DesignerSerializationVisibilityAttribute(DesignerSerializationVisibility.Content)]
        [Browsable(true)]
        public GridGroupOptionsStyleInfo NestedTableGroupOptions
        {
            get
            {
                if (childTableGroupOptions == null)
                {
                    childTableGroupOptions = new GridGroupOptionsStyleInfo(new GridGroupOptionsStyleInfoIdentity(this, GridGroupOptionsType.ChildTable));
                }

                return childTableGroupOptions;
            }

            set
            {
                NestedTableGroupOptions.CopyFrom(value);
            }
        }

        /// <summary>
        /// Determines whether <see cref="TopLevelGroupOptions"/> were modified
        /// and contents should be serialized at design-time.
        /// </summary>
        /// <returns>True if contents were changed; False otherwise.</returns>
        public bool ShouldSerializeNestedTableGroupOptions()
        {
            return this.childTableGroupOptions != null && !childTableGroupOptions.IsEmpty;
        }

        /// <summary>
        /// Discards any changes for the <see cref="NestedTableGroupOptions"/> object.
        /// </summary>
        public void ResetNestedTableGroupOptions()
        {
            if (ShouldSerializeNestedTableGroupOptions())
            {
                ((IGridGroupOptionsSource)this).RaiseGroupOptionsChanging(null);
                this.childTableGroupOptions = null;
                ((IGridGroupOptionsSource)this).RaiseGroupOptionsChanged(null);
            }
        }

        GridEngine IGridGroupOptionsSource.Engine
        {
            get
            {
                return this;
            }
        }

        #endregion

        #region IGridTableOptionsSource Members
        GridTableOptionsStyleInfo tableOptions;

        ////        [System.Xml.Serialization.XmlIgnore]
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
        /// Lets you set table-wide properties like the width of the indent column, or whether header rows should be visible.
        /// </summary>
        [Category("Look and Feel")]
        [DesignerSerializationVisibilityAttribute(DesignerSerializationVisibility.Content)]
        [Browsable(true)]
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
        /// Determines whether <see cref="TopLevelGroupOptions"/> were modified
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
            if (ShouldSerializeTableOptions())
            {
                ((IGridTableOptionsSource)this).RaiseTableOptionsChanging(null);
                this.tableOptions = null;
                ((IGridTableOptionsSource)this).RaiseTableOptionsChanged(null);
            }
        }

        void IGridTableOptionsSource.RaiseTableOptionsChanged(GridTableOptionsChangedEventArgs e)
        {
            ////            if (hasTable)
            ////                Table.CountersDirty = true;
            ////            this.BumpVersion();
            ////            if (hasTable)
            ////                Table.RaiseDisplayElementChanged(Table, -1, -1, true, true, true);
            this.OnPropertyChanged(new DescriptorPropertyChangedEventArgs("TableOptions", e));
        }

        void IGridTableOptionsSource.RaiseTableOptionsChanging(GridTableOptionsChangedEventArgs e)
        {
            /////            if (hasTable)
            ////                Table.RaiseDisplayElementChanging(Table, -1, -1, true, true, true);
            this.OnPropertyChanging(new DescriptorPropertyChangedEventArgs("TableOptions", e));
        }

        IGridTableOptionsSource IGridTableOptionsSource.GetParentTableOptionsSource()
        {
            return null;
        }

        GridEngine IGridTableOptionsSource.Engine
        {
            get
            {
                return this;
            }
        }
        #endregion

        #region ISupportInitialize

        /// <summary>
        /// Implements <see cref="ISupportInitialize.BeginInit"/> of the <see cref="ISupportInitialize"/> interface.
        /// </summary>
        public void BeginInit()
        {
            ////TraceUtil.TraceCurrentMethodInfoIf(Switches.BeginEndUpdate.TraceVerbose);
            if (inInit)
            {
                throw new System.Exception("BeginInit called twice.");
            }

            inInit = true;
        }

        bool inInit;

        /// <summary>
        /// Implements <see cref="ISupportInitialize.EndInit"/> of the <see cref="ISupportInitialize"/> interface.
        /// </summary>
        public void EndInit()
        {
            ////TraceUtil.TraceCurrentMethodInfoIf(Switches.BeginEndUpdate.TraceVerbose);
            inInit = false;
        }

        /// <summary>
        /// Determines if <see cref="GridModel.BeginInit"/> was called.
        /// </summary>
        [Browsable(false)]
        [System.Xml.Serialization.XmlIgnore]
        [DesignerSerializationVisibilityAttribute(DesignerSerializationVisibility.Hidden)]
        public bool Initializing
        {
            [DebuggerStepThrough()]
            get
            {
                return this.inInit;
            }
        }
        #endregion

        #region XmlSerializer
        [ThreadStatic]
        private static XmlSerializer xmlSerializer;

        /// <summary>
        /// Returns the <see cref="XmlSerializer"/> that can be used to
        /// serialize and deserialize this object to XML.
        /// </summary>
        /// <returns>The xml serializer.</returns>
        public static XmlSerializer GetXmlSerializer()
        {
            if (xmlSerializer == null)
            {
                xmlSerializer = new XmlSerializer(typeof(GridEngine));
            }

            return xmlSerializer;
        }

        /// <overload>
        /// Creates a <see cref="GridEngine"/> object from a valid XML stream.
        /// </overload>
        /// <summary>
        /// Creates a <see cref="GridEngine"/> object from a valid XML stream.
        /// </summary>
        /// <param name="xr">The XML stream.</param>
        /// <returns>A <see cref="GridEngine"/> object.</returns>
        public static GridEngine CreateFromXml(XmlReader xr)
        {
            XmlSerializer serializer = GetXmlSerializer();
            object obj = serializer.Deserialize(xr);
            xr.Close();
            return obj as GridEngine;
        }

        /// <summary>
        /// Creates a <see cref="GridEngine"/> object from a valid XML stream.
        /// </summary>
        /// <param name="r">The TextReader with XML stream.</param>
        /// <returns>A <see cref="GridEngine"/> object.</returns>
        public static GridEngine CreateFromXml(TextReader r)
        {
            System.Xml.XmlReader xr = new System.Xml.XmlTextReader(r);
            return CreateFromXml(xr);
        }

        /// <overload>
        /// Saves the engine changes to an XML stream.
        /// </overload>
        /// <summary>
        /// Saves the engine changes to an XML stream.
        /// </summary>
        /// <param name="xw">The XMLWriter.</param>
        public void WriteXml(XmlWriter xw)
        {
            XmlSerializer serializer = GetXmlSerializer();
            serializer.Serialize(xw, this);
        }

        /// <summary>
        /// Saves the engine changes to an XML stream.
        /// </summary>
        /// <param name="w">The TextWriter.</param>
        public void WriteXml(TextWriter w)
        {
            System.Xml.XmlTextWriter xw = new System.Xml.XmlTextWriter(w);
            xw.Formatting = Formatting.Indented;
            WriteXml(xw);
        }
        #endregion

        #region Custom Summaries

        // event GridRaiseQueryCustomSummary QueryCustomSummary

        /// <summary>
        /// Occurs for each GridSummaryColumnDescriptor before the <see cref="SummaryDescriptor"/> is determined. You must handle this event if you specified <see cref="SummaryType.Custom"/> as <see cref="GridSummaryColumnDescriptor.SummaryType"/>.
        /// </summary>
        [Description("Occurs for each GridSummaryColumnDescriptor before the SummaryDescriptor is determined. You must handle this event if you specified SummaryType.Custom as GridSummaryColumnDescriptor.SummaryType")]
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
        }

        #endregion

        private void Appearance_Changing(object sender, GridTableCellStyleInfoChangedEventArgs e)
        {
            this.OnPropertyChanging(new DescriptorPropertyChangedEventArgs("Appearance", e));
        }

        private void NestedTableGroupOptions_Changing(object sender, Syncfusion.Styles.StyleChangedEventArgs e)
        {
            this.OnPropertyChanging(new DescriptorPropertyChangedEventArgs("NestedTableGroupOptions", e));
        }

        private void ChildGroupOptions_Changing(object sender, Syncfusion.Styles.StyleChangedEventArgs e)
        {
            this.OnPropertyChanging(new DescriptorPropertyChangedEventArgs("ChildGroupOptions", e));
        }

        private void PropertyTypeDefaultStyles_Changing(object sender, Syncfusion.Collections.ListPropertyChangedEventArgs e)
        {
            this.OnPropertyChanging(new DescriptorPropertyChangedEventArgs("PropertyTypeDefaultStyles", e));
        }

        private void TopLevelGroupOptions_Changing(object sender, Syncfusion.Styles.StyleChangedEventArgs e)
        {
            this.OnPropertyChanging(new DescriptorPropertyChangedEventArgs("TopLevelGroupOptions", e));
        }

        private void Appearance_Changed(object sender, GridTableCellStyleInfoChangedEventArgs e)
        {
            this.OnPropertyChanged(new DescriptorPropertyChangedEventArgs("Appearance", e));
            this.appearanceVersion++;
        }

        private void NestedTableGroupOptions_Changed(object sender, Syncfusion.Styles.StyleChangedEventArgs e)
        {
            this.OnPropertyChanged(new DescriptorPropertyChangedEventArgs("NestedTableGroupOptions", e));
        }

        private void GroupOptions_Changed(object sender, Syncfusion.Styles.StyleChangedEventArgs e)
        {
            this.OnPropertyChanged(new DescriptorPropertyChangedEventArgs("GroupOptions", e));
        }

        private void PropertyTypeDefaultStyles_Changed(object sender, Syncfusion.Collections.ListPropertyChangedEventArgs e)
        {
            this.OnPropertyChanged(new DescriptorPropertyChangedEventArgs("PropertyTypeDefaultStyles", e));
            this.appearanceVersion++;
        }

        private void TopLevelGroupOptions_Changed(object sender, Syncfusion.Styles.StyleChangedEventArgs e)
        {
            this.OnPropertyChanged(new DescriptorPropertyChangedEventArgs("TopLevelGroupOptions", e));
        }

        private void TableDescriptor_PropertyChanging(object sender, DescriptorPropertyChangedEventArgs e)
        {
            this.OnPropertyChanging(new DescriptorPropertyChangedEventArgs("TableDescriptor", e));
        }

        private void TableDescriptor_PropertyChanged(object sender, DescriptorPropertyChangedEventArgs e)
        {
            this.OnPropertyChanged(new DescriptorPropertyChangedEventArgs("TableDescriptor", e));
        }

        private void engineDefaultTableOptions_Changing(object sender, Syncfusion.Styles.StyleChangedEventArgs e)
        {
        }

        private void engineDefaultTableOptions_Changed(object sender, Syncfusion.Styles.StyleChangedEventArgs e)
        {
            if (this.ShouldSerializeTable())
            {
                Table.CountersDirty = true;
                Table.SummariesDirty = true;
            }

            ////if (e.Sip != null && e.Sip.PropertyName == "Font")
            this.appearanceVersion++;
        }

        private int appearanceVersion;

        /// <summary>
        /// The version of the <see cref="Appearance"/> object. This number is increased
        /// each time a property in the <see cref="Appearance"/> object is changed.
        /// </summary>
        [Browsable(false)]
        [System.Xml.Serialization.XmlIgnore]
        [DesignerSerializationVisibilityAttribute(DesignerSerializationVisibility.Hidden)]
        public int AppearanceVersion
        {
            get
            {
                return this.appearanceVersion;
            }

            set
            {
                this.appearanceVersion = value;
            }
        }

        /// <summary>
        /// Enable this flag if you want GridRecordRow and GridCaptionRow elements to keep
        /// a cache with style information for individual cells and reduce the number of
        /// QueryCellStyleInfo calls being raised for this cell.
        /// </summary>
        /// <remarks>
        /// When you enable this property, the <see cref="CreateRecordRow"/> and <see cref="CreateCaptionRow"/>
        /// methods will instantiate derived <see cref="GridRecordRowWithCache"/> and <see cref="GridCaptionRowWithCache"/>
        /// elements that implement the IGridTableCellStyleCache interface.
        /// <para/>
        /// If other elements should support caching, you can add support for caching by deriving a class
        /// from the elements base class and implement the IGridTableCellStyleCache interface. See the
        /// GridRecordRowWithCache implementation how to do this.
        /// </remarks>
        [DefaultValue(false)]
        public bool AllowCacheStyles
        {
            get
            {
                return allowCacheStyles;
            }

            set
            {
                allowCacheStyles = value;
            }
        }

        void EnableNewTableOptionsDefaultSettings()
        {
            GridEngine engine = this;

            engine.engineDefaultTableOptions.ColumnsMaxLengthStrategy = GridColumnsMaxLengthStrategy.FirstNRecords;
            engine.engineDefaultTableOptions.ColumnsMaxLengthFirstNRecords = 100;

            //// Do not use dotted grid lines - use solid lines instead.
            engine.engineDefaultTableOptions.GridLineBorder = new GridBorder(GridBorderStyle.Solid, SystemColors.Control);

            //// Enable GDI drawing instead of GDI+
            engine.engineDefaultTableOptions.DrawTextWithGdiInterop = true;

            engine.DefaultAppearance.AnySummaryCell.WrapText = false;
            engine.DefaultAppearance.AnyRecordFieldCell.WrapText = false;
            engine.DefaultAppearance.AnySummaryCell.Trimming = StringTrimming.None;
            engine.DefaultAppearance.AnyRecordFieldCell.Trimming = StringTrimming.None;

            //// Need to disable for VScrollPixel for Insert/Remove rows case because
            //// it does not work correct when a top row is only partial visible. Will be fixed
            //// later in 4.4
            engine.engineDefaultTableOptions.VerticalPixelScroll = false;
        }

        void ResetNewTableOptionsDefaultSettings()
        {
            GridEngine engine = this;

            engine.engineDefaultTableOptions.ResetColumnsMaxLengthStrategy();
            engine.engineDefaultTableOptions.ResetColumnsMaxLengthFirstNRecords();

            //// Do not use dotted grid lines - use solid lines instead.
            engine.engineDefaultTableOptions.ResetGridLineBorder();

            //// Enable GDI drawing instead of GDI+
            engine.engineDefaultTableOptions.ResetDrawTextWithGdiInterop();

            engine.DefaultAppearance.AnySummaryCell.ResetWrapText();
            engine.DefaultAppearance.AnyRecordFieldCell.ResetWrapText();
            engine.DefaultAppearance.AnySummaryCell.ResetTrimming();
            engine.DefaultAppearance.AnyRecordFieldCell.ResetTrimming();

            //// Need to disable for VScrollPixel for Insert/Remove rows case because
            //// it does not work correct when a top row is only partial visible. Will be fixed
            //// later in 4.4
            engine.engineDefaultTableOptions.ResetVerticalPixelScroll();
        }

        bool useDefaultsForFasterDrawing = false;

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
                return useDefaultsForFasterDrawing;
            }

            set
            {
                if (useDefaultsForFasterDrawing != value)
                {
                    this.OnPropertyChanging(new DescriptorPropertyChangedEventArgs("UseDefaultsForFasterDrawing"));
                    if (value)
                    {
                        EnableNewTableOptionsDefaultSettings();
                    }
                    else
                    {
                        ResetNewTableOptionsDefaultSettings();
                    }

                    useDefaultsForFasterDrawing = value;
                    this.OnPropertyChanged(new DescriptorPropertyChangedEventArgs("UseDefaultsForFasterDrawing"));
                }
            }
        }

        int blinkTime = 0;
        int updateDisplayFrequency = 100;
        GridListChangedInsertRemoveBehavior insertRemoveBehavior = GridListChangedInsertRemoveBehavior.InvalidateVisible;
        GridListChangedInsertRemoveBehavior insertRemoveBehaviorWithEndEdit = GridListChangedInsertRemoveBehavior.InvalidateAll;
        GridListChangedInsertRemoveBehavior sortPositionChangedBehavior = GridListChangedInsertRemoveBehavior.InvalidateVisible;
        GridListChangedInsertRemoveBehavior sortPositionChangedBehaviorWithEndEdit = GridListChangedInsertRemoveBehavior.InvalidateAll;

        /// <summary>
        /// Specifies how the grid should react if the sort position of a record records changes.
        /// </summary>
        [DefaultValue(GridListChangedInsertRemoveBehavior.InvalidateVisible)]
        [Category("Optimization")]
        [Description("Specifies how the grid should react if the sort position of a record records changes.")]
        public GridListChangedInsertRemoveBehavior SortPositionChangedBehavior
        {
            get { return sortPositionChangedBehavior; }
            set { sortPositionChangedBehavior = value; }
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
            get { return sortPositionChangedBehaviorWithEndEdit; }
            set { sortPositionChangedBehaviorWithEndEdit = value; }
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
                return this.updateDisplayFrequency;
            }

            set
            {
                if (this.updateDisplayFrequency != value)
                {
                    this.OnPropertyChanging(new DescriptorPropertyChangedEventArgs("UpdateDisplayFrequency"));
                    this.updateDisplayFrequency = value;
                    this.OnPropertyChanging(new DescriptorPropertyChangedEventArgs("UpdateDisplayFrequency"));
                }
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
            get
            {
                return insertRemoveBehavior;
            }

            set
            {
                if (this.insertRemoveBehavior != value)
                {
                    this.OnPropertyChanging(new DescriptorPropertyChangedEventArgs("InsertRemoveBehavior"));
                    this.insertRemoveBehavior = value;
                    this.OnPropertyChanging(new DescriptorPropertyChangedEventArgs("InsertRemoveBehavior"));
                }
            }
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
            get
            {
                return insertRemoveBehaviorWithEndEdit;
            }

            set
            {
                if (this.insertRemoveBehaviorWithEndEdit != value)
                {
                    this.OnPropertyChanging(new DescriptorPropertyChangedEventArgs("InsertRemoveBehaviorWithEndEdit"));
                    this.insertRemoveBehaviorWithEndEdit = value;
                    this.OnPropertyChanging(new DescriptorPropertyChangedEventArgs("InsertRemoveBehaviorWithEndEdit"));
                }
            }
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
                return this.blinkTime;
            }

            set
            {
                if (this.blinkTime != value)
                {
                    this.OnPropertyChanging(new DescriptorPropertyChangedEventArgs("BlinkTime"));
                    this.blinkTime = value;
                    this.OnPropertyChanging(new DescriptorPropertyChangedEventArgs("BlinkTime"));
                }
            }
        }

        /// <summary>
        /// Checks if the DescriptorPropertyChangedEventArgs has nested event data and if the innermost event is the
        /// result of a change to an Appearance object. If so, the method will return the GridTableCellStyleInfoChangedEventArgs
        /// and the object whose appearance was modified.<para/>
        /// Example: object app = tableDescriptor;<para/>
        /// GridTableCellStyleInfoChangedEventArgs tableCellStyleInfoChangedEventArgs = GetNestedAppearanceEvent(e, ref app);<para/>
        /// IGridTableCellAppearanceSource appearanceHolder = app as IGridTableCellAppearanceSource;
        /// </summary>
        /// <param name="e">Event data.</param>
        /// <param name="appearanceHolder">The object holding the appearance settings.</param>
        /// <returns>The inner event args that holds more information about the nested event.</returns>
        public static GridTableCellStyleInfoChangedEventArgs GetNestedAppearanceEvent(DescriptorPropertyChangedEventArgs e, ref object appearanceHolder)
        {
            TableDescriptor td = appearanceHolder as TableDescriptor;
            if (td != null)
            {
                e = e.GetNestedChildTableDescriptorEvent(ref td);
                appearanceHolder = td;
            }

            //// Columns, ConditionalFormats, SummaryRows
            ListPropertyChangedEventArgs rla = e.Inner as ListPropertyChangedEventArgs;
            if (rla != null)
            {
                //// relationListArgs.Item: GridColumnDescriptor, GridConditionalFormatsDescriptor, GridSummaryRowDescriptor
                //// relationListArgs.Inner: DescriptorPropertyChangedEventArgs (reason why GridColumnDescriptor was changed)

                if (rla.Action == ListPropertyChangedType.ItemPropertyChanged)
                {
                    DescriptorPropertyChangedEventArgs rlai = rla.Inner as DescriptorPropertyChangedEventArgs;
                    if (rlai != null)
                    {
                        if (rlai.PropertyName == "Appearance")
                        {
                            appearanceHolder = rla.Item;
                            return rlai.Inner as GridTableCellStyleInfoChangedEventArgs;
                        }
                        else if (rlai.PropertyName == "SummaryColumns")
                        {
                            rla = rlai.Inner as ListPropertyChangedEventArgs;
                            //// could do a while loop here - but I think with a nested if it's easier to understand the code ...
                            if (rla != null)
                            {
                                //// relationListArgs.Item: GridSummaryColumnDescriptor
                                //// relationListArgs.Inner: DescriptorPropertyChangedEventArgs (reason why GridColumnDescriptor was changed)

                                if (rla.Action == ListPropertyChangedType.ItemPropertyChanged)
                                {
                                    rlai = rla.Inner as DescriptorPropertyChangedEventArgs;
                                    if (rlai != null)
                                    {
                                        if (rlai.PropertyName == "Appearance")
                                        {
                                            appearanceHolder = rla.Item;
                                            return rlai.Inner as GridTableCellStyleInfoChangedEventArgs;
                                        }
                                    }
                                }
                            }
                        }
                    }
                }
            }

            return null;
        }

        /// <override/>
        protected override void OnQueryValue(FieldValueEventArgs e)
        {
            if (this.GroupingControl != null)
            {
                this.GroupingControl.RaiseQueryValue(e);
            }

            base.OnQueryValue(e);
        }

        /// <override/>
        protected override void OnSaveValue(FieldValueEventArgs e)
        {
            if (this.GroupingControl != null)
            {
                this.GroupingControl.RaiseSaveValue(e);
            }

            base.OnSaveValue(e);
        }

        //// event QueryAddColumn QueryAddColumn

        /// <summary>
        /// The GridEngine.QueryAddColumn event affects the auto-population of the GridColumnDescriptorCollection. <para/>
        /// It is called for each column and lets you control at run-time if the column should be added to the
        /// GridColumnDescriptorCollection. You can set e.Cancel = True to avoid specific columns
        /// being added.
        /// </summary>
        public event GridQueryAddColumnEventHandler QueryAddColumn;

        /// <summary>
        /// Raises the <see cref="QueryAddColumn"/> event.
        /// </summary>
        /// <param name="e">A <see cref="GridQueryAddColumnEventArgs" /> that contains the event data.</param>
        protected virtual void OnQueryAddColumn(GridQueryAddColumnEventArgs e)
        {
            if (QueryAddColumn != null)
            {
                QueryAddColumn(this, e);
            }
        }

        /// <summary>
        /// Raises the <see cref="QueryAddColumn"/> event.
        /// </summary>
        /// <param name="e">A <see cref="GridQueryAddColumnEventArgs" /> that contains the event data.</param>
        public void RaiseQueryAddColumn(GridQueryAddColumnEventArgs e)
        {
            OnQueryAddColumn(e);
        }

        //// event QueryAddVisibleColumn QueryAddVisibleColumn

        /// <summary>
        /// The GridEngine.QueryAddVisibleColumn event affects the auto-population of the GridVisibleColumnDescriptorCollection. <para/>
        /// It is called for each column and lets you control at run-time if the column should be added to the
        /// GridVisibleColumnDescriptorCollection. You can set e.Cancel = True to avoid specific columns
        /// being added.
        /// </summary>
        public event GridQueryAddVisibleColumnEventHandler QueryAddVisibleColumn;

        /// <summary>
        /// Raises the <see cref="QueryAddVisibleColumn"/> event.
        /// </summary>
        /// <param name="e">A <see cref="GridQueryAddVisibleColumnEventArgs" /> that contains the event data.</param>
        protected virtual void OnQueryAddVisibleColumn(GridQueryAddVisibleColumnEventArgs e)
        {
            if (QueryAddVisibleColumn != null)
            {
                QueryAddVisibleColumn(this, e);
            }
        }

        /// <summary>
        /// Raises the <see cref="QueryAddVisibleColumn"/> event.
        /// </summary>
        /// <param name="e">A <see cref="GridQueryAddVisibleColumnEventArgs" /> that contains the event data.</param>
        public void RaiseQueryAddVisibleColumn(GridQueryAddVisibleColumnEventArgs e)
        {
            OnQueryAddVisibleColumn(e);
        }

        /// <summary>
        /// The assembly version of the GridGroupingControl at the time
        /// it was dropped onto a form with designer.
        /// </summary>
        [Browsable(false)]
        public override string VersionInfo
        {
            get
            {
                return this.ParentControl != null ? ParentControl.VersionInfo : string.Empty;
            }
        }

        internal event CancelEventHandler MarkResyncEvent;

        internal void RaiseMarkResync(bool resetWidth)
        {
            if (MarkResyncEvent != null)
            {
                MarkResyncEvent(this, new CancelEventArgs(resetWidth));
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
        /// to <see cref="GridCellTextEventArgs.Text"/> and set <see cref="SyncfusionHandledEventArgs.Handled"/>
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
        /// and set <see cref="SyncfusionHandledEventArgs.Handled"/>
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
        /// to <see cref="GridCellTextEventArgs.Text"/> and set <see cref="SyncfusionHandledEventArgs.Handled"/>
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
        /// This event is normally called from within <see cref="GridStyleInfo"/>, which is called
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
        /// Raises the <see cref="QueryCellText"/> event.
        /// </summary>
        /// <param name="e">A <see cref="GridCellTextEventArgs" /> that contains the event data.</param>
        protected virtual void OnQueryCellText(GridCellTextEventArgs e)
        {
            if (this.GroupingControl != null)
            {
                this.GroupingControl.RaiseQueryCellText(e);
            }

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
            if (this.GroupingControl != null)
            {
                this.GroupingControl.RaiseSaveCellText(e);
            }

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
            if (this.GroupingControl != null)
            {
                this.GroupingControl.RaiseQueryCellFormattedText(e);
            }

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
        /// Raises the <see cref="SaveCellFormattedText"/> event.
        /// </summary>
        /// <param name="e">A <see cref="GridCellTextEventArgs" /> that contains the event data.</param>
        protected virtual void OnSaveCellFormattedText(GridCellTextEventArgs e)
        {
            if (this.GroupingControl != null)
            {
                this.GroupingControl.RaiseSaveCellFormattedText(e);
            }

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
        /// Raises the <see cref="ParseCommonFormats"/> event.
        /// </summary>
        /// <param name="e">A <see cref="GridCellTextEventArgs" /> that contains the event data.</param>
        protected virtual void OnParseCommonFormats(GridCellTextEventArgs e)
        {
            if (this.GroupingControl != null)
            {
                this.GroupingControl.RaiseParseCommonFormats(e);
            }

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
        /// Occurs to determine if the cell belongs to a covered range and returns the covered range of the cell or
        /// the cell itself as <see cref="GridRangeInfo"/> if it is not a covered range.
        /// </summary>
        public event GridTableQueryCoveredRangeEventHandler QueryCoveredRange;

        /// <summary>
        /// Raises the <see cref="QueryCoveredRange"/> event.
        /// </summary>
        /// <param name="e">A <see cref="GridTableCellStyleInfoEventArgs" /> that contains the event data.</param>
        protected virtual void OnQueryCoveredRange(GridTableQueryCoveredRangeEventArgs e)
        {
            if (this.GroupingControl != null)
            {
                this.GroupingControl.RaiseQueryCoveredRange(e);
            }

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

        /// <overload>
        /// Returns the formatted summary text for the given group, summary row and column.
        /// </overload>
        /// <summary>
        /// Returns the formatted summary text for the given group, summary row and column.
        /// </summary>
        /// <param name="group">The group that defines a set of records that belong to a category.</param>
        /// <param name="summaryRowName">The name of the GridSummaryRowDescriptor in the GridTableDescriptor.SummaryRows collection.</param>
        /// <param name="summaryColumnName">The name of the GridSummaryColumnDescriptor in the GridSummaryRowDescriptor.Summaries collection.</param>
        /// <returns>The summary formatted text as specified with GridSummaryColumnDescriptor.Format</returns>
        /// <remarks>
        /// See <see cref="GridSummaryColumnDescriptor"/> for an example.
        /// </remarks>
        public static string GetSummaryText(Group group, string summaryRowName, string summaryColumnName)
        {
            GridTable table = (GridTable)group.ParentTable;
            GridTableDescriptor td = table.TableDescriptor;
            GridSummaryRowDescriptor srd = td.SummaryRows[summaryRowName];
            if (srd != null)
            {
                GridSummaryColumnDescriptor scd = srd.SummaryColumns[summaryColumnName];
                return GetSummaryText(group, scd);
            }

            return string.Empty;
        }

        /// <summary>
        /// Returns the formatted summary text for the given group, and summary column.
        /// </summary>
        /// <param name="group">The group that defines a set to records that belongs to a category.</param>
        /// <param name="scd">The GridSummaryColumnDescriptor.</param>
        /// <returns>The summary formatted text as specified with GridSummaryColumnDescriptor.Format</returns>
        /// <genoverload/>
        public static string GetSummaryText(Group group, GridSummaryColumnDescriptor scd)
        {
            if (scd != null)
            {
                return scd.GetDisplayText(group);
            }

            return string.Empty;
        }

        /// <summary>
        /// Returns the caption text for a group.
        /// </summary>
        /// <param name="group">The group that defines a set to records that belongs to a category.</param>
        /// <returns>Caption text for the group.</returns>
        public static string GetGroupCaptionText(Group group)
        {
            IGridGroupOptionsSource g = group as IGridGroupOptionsSource;
            string captionText;
            if (g != null)
            {
                captionText = g.GroupOptions.CaptionText;
            }
            else
            {
                captionText = "{CategoryCaption}: {Category} - {RecordCount} Items";
            }

            return GetGroupCaptionDisplayText(group, captionText);
        }

        /// <summary>
        /// Returns the caption text for a group using a specified format.
        /// </summary>
        /// <param name="group">The group that defines a set to records that belongs to a category.</param>
        /// <param name="format">See GroupOptions.CaptionText, e.g. "{CategoryCaption}: {Category} - {RecordCount} Items";</param>
        /// <returns>Caption text for the group.</returns>
        public static string GetGroupCaptionDisplayText(Group group, string format)
        {
            GridTableDescriptor tableDescriptor = (GridTableDescriptor)group.ParentTableDescriptor;
            bool raiseException = false;
            ArrayList al = new ArrayList();
            int n1 = format.IndexOf("{");
            StringBuilder sb = new StringBuilder();
            int n2 = 0;

            if (n1 == -1)
            {
                sb.Append(format);
            }
            else
            {
                sb.Append(format.Substring(0, n1 + 1));
            }

            while (n1 != -1)
            {
                n2 = format.IndexOf("}", n1);
                if (n1 != -1 && n2 != -1 && n2 > n1)
                {
                    int n3 = format.IndexOfAny(new char[] { '}', ':' }, n1);
                    string name = format.Substring(n1 + 1, n3 - n1 - 1);
                    sb.Append(al.Count.ToString());
                    object obj = string.Empty;
                    if (name == "TableName" && group.ParentTableDescriptor !=null)
                    {
                        obj = group.ParentTableDescriptor.Name;
                    }
                    else if (name == "CategoryName")
                    {
                        obj = group.Name;
                    }
                    else if (name == "CategoryCaption")
                    {
                        string fieldName = group.Name;
                        GridColumnDescriptor cd = group.GroupLevel >= 0 ? tableDescriptor.Columns.FindByMappingName(fieldName) : null;
                        if (cd != null)
                        {
                            obj = cd.HeaderText;
                        }
                        else
                        {
                            obj = fieldName;
                        }
                    }
                    else if (name == "Category")
                    {
                        obj = group.Category;
                    }
                    else if (name == "RecordCount")
                    {
                        //// IPassThroughGroupingResult
                        if (group.ParentTable !=null && group.ParentTable.PassThroughGroupingResult != null)
                        {
                            obj = group.GetChildCount();
                        }
                        else
                        {
                            obj = group.GetFilteredRecordCount();
                        }
                    }
#if ASPNET
                    else if(name == "PagingInfo")
                    {
                        obj = ((GridCaptionSection)group.Caption).PagingCaptionText;
                    }
                    else if (name == "CurrentPage")
                    {
                        obj = ((GridCaptionSection)group.Caption).CurrentPage;
                    }
                    else if (name == "PageCount")
                    {
                        obj = ((GridCaptionSection)group.Caption).PageCount;
                    }
#endif
                    else
                    {
                        GridTableDescriptor td = (GridTableDescriptor)group.ParentTableDescriptor;
                        int summaryRowNum = -1;
                        int dot = name.IndexOf('.');
                        if (td != null)
                        {
                            if (dot != -1)
                            {
                                string rowName = name.Substring(0, dot);
                                name = name.Substring(dot + 1);
                                summaryRowNum = td.SummaryRows.IndexOf(rowName);
                            }
                            else
                            {
                                summaryRowNum = td.SummaryRows.IndexOf("GroupCaption");
                            }
                        }

                        if (summaryRowNum != -1)
                        {
                            GridSummaryColumnDescriptor scd = td.SummaryRows[summaryRowNum].SummaryColumns[name];
                            if (scd != null)
                            {
                                Group g = group;
                                Syncfusion.Collections.BinaryTree.ITreeTableSummary[] sums = g.GetSummaries(group.ParentTable);
                                int ndx = scd.GetSummaryIndex();
                                ISummary summary = null;
                                if (sums != null && ndx != -1)
                                {
                                    summary = sums[ndx];
                                }

                                obj = scd.GetDisplayText(summary, g.PassThroughItem);
                            }
                            else
                            {
                            }
                        }
                    }

                    al.Add(obj);
                    n1 = format.IndexOf("{", n2);
                    if (n1 == -1)
                    {
                        sb.Append(format.Substring(n3));
                    }
                    else
                    {
                        sb.Append(format.Substring(n3, n1 - n3 + 1));
                    }
                }
                else
                {
                    if (raiseException)
                    {
                        throw new FormatException("No closing char found: " + sb.ToString());
                    }
                    else
                    {
                        break;
                    }
                }
            }

            string formatString = sb.ToString();
            try
            {
                string captionDispText = String.Format(formatString, al.ToArray());
#if ASPNET
                // With the new default CaptionText value (which includes a "({PagingInfo})" string)
                // it's possible that the resultant text contains a "()" (when CurrentPage == -1 for example).
                // We will remove such stray brackets here.
                captionDispText = captionDispText.Replace("()", String.Empty);
#endif
                return captionDispText;
            }
            catch (Exception ex)
            {
                return formatString + ": " + ex.Message;
            }
        }

        /// <summary>
        /// Determines if GroupOptions were specified for a group.
        /// </summary>
        /// <param name="group">The <see cref="GridGroup"/> or <see cref="GridChildTable"/></param>
        /// <returns>True if GroupOptions were specified.</returns>
        public static bool HasGroupOptions(Group group)
        {
            IGridGroupOptionsSource gos = group as IGridGroupOptionsSource;
            if (gos != null)
            {
                return gos.HasGroupOptions;
            }

            return false;
        }

        /// <summary>
        /// Gets the GroupOptions of a GridGroup or GridChildTable
        /// </summary>
        /// <param name="group">The <see cref="GridGroup"/> or <see cref="GridChildTable"/></param>
        /// <returns>GroupOptions of the group.</returns>
        public static GridGroupOptionsStyleInfo GetGroupOptions(Group group)
        {
            IGridGroupOptionsSource gos = group as IGridGroupOptionsSource;
            if (gos != null)
            {
                return gos.GroupOptions;
            }

            return null;
        }

        /// <summary>
        /// If the groups GroupOptions were modified, the GroupOptions returns group object's GroupOptions; otherwise it
        /// returns a <see cref="GridGroupOptionsStyleInfo"/> of the first parent element with GroupOptions
        /// in the hierarchy.
        /// </summary>
        /// <param name="group">The <see cref="GridGroup"/> or <see cref="GridChildTable"/></param>
        /// <returns>GroupOptions of the group.</returns>
        public static GridGroupOptionsStyleInfo GetReadGroupOptions(Group group)
        {
            GridGroup gg = group as GridGroup;
            if (gg != null)
            {
                return gg.ReadGroupOptions;
            }

            GridChildTable ct = group as GridChildTable;
            if (ct != null)
            {
                return ct.ReadGroupOptions;
            }

            IGridGroupOptionsSource gos = group as IGridGroupOptionsSource;
            if (gos != null)
            {
                return gos.GroupOptions;
            }

            return null;
        }

        /// <summary>
        /// Returns a <see cref="GridTableCellAppearance"/> of the first parent element with appearance in the hierarchy.
        /// </summary>
        /// <param name="el">The element to be queried.</param>
        /// <returns>A <see cref="GridTableCellAppearance"/>.</returns>
        public static GridTableCellAppearance GetBaseAppearance(Element el)
        {
            Element parent = el.ParentElement;
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
        /// <param name="el">The element.</param>
        /// <returns>Appearance of this element.</returns>
        public static GridTableCellAppearance GetReadOnlyAppearance(Element el)
        {
            IGridTableCellAppearanceSource tcas = el as IGridTableCellAppearanceSource;
            if (tcas != null)
            {
                if (!tcas.ShouldSerializeAppearance())
                {
                    return tcas.GetBaseAppearance();
                }

                return tcas.GetAppearance();
            }

            return null;
        }

        /// <summary>
        /// The default <see cref="GridTableCellAppearance"/> with <see cref="GridTableCellStyleInfo"/>
        /// information for cells of this element.
        /// </summary>
        /// <param name="el">The element.</param>
        /// <returns>A <see cref="GridTableCellAppearance"/>.</returns>
        public static GridTableCellAppearance GetAppearance(Element el)
        {
            IGridTableCellAppearanceSource tcas = el as IGridTableCellAppearanceSource;
            if (tcas != null)
            {
                return tcas.GetAppearance();
            }

            return null;
        }

        /// <summary>
        /// Determines whether <see cref="Appearance"/> has been modified
        /// and contents should be serialized at design-time.
        /// </summary>
        /// <param name="el">The element.</param>
        /// <returns>True if contents were changed; False otherwise.</returns>
        public static bool ShouldSerializeAppearance(Element el)
        {
            IGridTableCellAppearanceSource tcas = el as IGridTableCellAppearanceSource;
            if (tcas != null)
            {
                return tcas.ShouldSerializeAppearance();
            }

            return false;
        }

        /// <summary>
        /// Discards any changes for the <see cref="Appearance"/> object.
        /// </summary>
        /// <param name="el">An element that implements the <see cref="IGridTableCellAppearanceSource"/> interface.</param>
        public static void ResetAppearance(Element el)
        {
            IGridTableCellAppearanceSource tcas = el as IGridTableCellAppearanceSource;
            if (tcas != null && tcas.ShouldSerializeAppearance())
            {
                tcas.GetAppearance().Reset();
            }
        }

        bool markSortedColumnsDirtyWhenSortedPositionChanged = false;

        /// <exclude/>
        /// <summary>Used internally.</summary>
        [Browsable(false)]
        [DefaultValue(false)]
        public bool MarkSortedColumnsDirtyWhenSortedPositionChanged
        {
            get
            {
                return markSortedColumnsDirtyWhenSortedPositionChanged;
            }

            set
            {
                markSortedColumnsDirtyWhenSortedPositionChanged = value;
            }
        }
    }

    internal class GridDefaultAppearanceSource : IGridTableCellAppearanceSource
    {
        GridTableCellAppearance appearance;

        /// <summary>
        /// The default <see cref="GridTableCellAppearance"/> with default <see cref="GridTableCellStyleInfo"/>
        /// information for all cell elements in the control. This property lets you control almost every aspect of
        /// the appearance of the grouping grid like cell backcolor, font, or the cell type.
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
                ((IGridTableCellAppearanceSource)this).RaiseAppearanceChanging(null);
                appearance.Reset();
                ((IGridTableCellAppearanceSource)this).RaiseAppearanceChanged(null);
            }
        }

        #region IGridTableCellAppearanceSource Members

        GridTableCellAppearance IGridTableCellAppearanceSource.GetAppearance()
        {
            return Appearance;
        }

        void IGridTableCellAppearanceSource.RaiseAppearanceChanged(GridTableCellStyleInfoChangedEventArgs e)
        {
            //// not meant to be changed at runtime - only at initialization time - therefore no need to raise event.
        }

        void IGridTableCellAppearanceSource.RaiseAppearanceChanging(GridTableCellStyleInfoChangedEventArgs e)
        {
        }

        GridTableCellAppearance IGridTableCellAppearanceSource.GetBaseAppearance()
        {
            return null;
        }

        GridEngine IGridTableCellAppearanceSource.Engine 
        { 
            get 
            {
                return null;
            } 
        }

        #endregion
    }

    /// <summary>
    /// Allows you to specify how often the grid should rect to changes in the sort position of the current 
    /// record when edited interactively by user.
    /// </summary>
    public enum GridListChangedInsertRemoveBehavior
    {
        /// <summary>
        /// Invalidate display, do not check position of record
        /// </summary>
        InvalidateAll,

        /// <summary>
        /// Invalidate only the visible portion of display that is affected by change. If record is above 
        /// current view change the top row to mininmize scrolling.
        /// </summary>
        InvalidateVisible,

        ///// <summary>
        ///// Scroll contents. If record is above 
        ///// current view change the top row to mininmize scrolling. If DoubleBufferSurface is enabled
        ///// the display will be updated later depending on UpdateDisplayFrequency setting. If DoubleBufferSurface
        ///// changes will be visible immediately.
        ///// </summary>
        ////ScrollAndDelayPaint,

        /// <summary>
        /// Scroll contents. If record is above 
        /// current view change the top row to mininmize scrolling. Changes will be visible immediately.
        /// </summary>
        ScrollWithImmediateUpdate
    }
}