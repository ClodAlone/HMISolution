#region Copyright
// Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Any infringement will be prosecuted under
// applicable laws. 
#endregion

namespace Syncfusion.Windows.Controls.Grid
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using System.ComponentModel;
    using System.Linq;
    using System.Text;
    using System.Windows;
    using System.Windows.Media;
    using Syncfusion.Linq;
    using Syncfusion.Windows.Controls.Scroll;
    using System.Windows.Data;
    using Syncfusion.Windows.Data;
    using System.Collections.Specialized;
    using System.Xml.Serialization;
    using Syncfusion.Windows.Shared;
    using System.Windows.Input;
#if SILVERLIGHT

    using System.Windows.Controls;
#endif
    /// <summary>
    /// Defines the Property values for the GridTable that gets generated at runtime.
    /// </summary>
#if SyncfusionFramework4_0&&!SILVERLIGHT
    [System.ComponentModel.DesignTimeVisible(false)]
#endif
#if !SILVERLIGHT
    [Serializable]
#endif
    public class GridDataTableProperties
#if !SILVERLIGHT
 : Freezable, IDisposable
#else
 : DependencyObject, IDisposable
#endif
    {
        public static readonly DependencyProperty AddNewRowPositionProperty = DependencyProperty.Register(
            "AddNewRowPosition",
            typeof(Position),
            typeof(GridDataTableProperties),
            new PropertyMetadata(Position.Top, OnAddNewRowPositionChanged));
        public static readonly DependencyProperty UnboundRowPositionProperty = DependencyProperty.Register(
            "UnboundRowPosition",
            typeof(Position),
            typeof(GridDataTableProperties),
            new PropertyMetadata(Position.Top, OnUnboundRowPositionChanged));

        public static readonly DependencyProperty TableSummaryPositionProperty = DependencyProperty.Register(
          "TableSummaryPosition",
          typeof(Position),
          typeof(GridDataTableProperties),
          new PropertyMetadata(Position.Bottom, OnTableSummaryPositionChanged));

        public static readonly DependencyProperty AllowDeleteProperty = DependencyProperty.Register(
            "AllowDelete",
            typeof(bool),
            typeof(GridDataTableProperties),
            new PropertyMetadata(true));

        public static readonly DependencyProperty AllowMultipleRecordDeletionProperty = DependencyProperty.Register(
            "AllowMultipleRecordDeletion",
            typeof(bool),
            typeof(GridDataTableProperties),
            new PropertyMetadata(false));

        public static readonly DependencyProperty AllowDragColumnsProperty = DependencyProperty.Register(
            "AllowDragColumns",
            typeof(bool),
            typeof(GridDataTableProperties),
            new PropertyMetadata(false, OnAllowDragColumnsChanged));

        public static readonly DependencyProperty AllowEditProperty = DependencyProperty.Register(
            "AllowEdit",
            typeof(bool),
            typeof(GridDataTableProperties),
            new PropertyMetadata(true, OnAllowEditPropertyChanged));

        public static readonly DependencyProperty AllowGroupProperty = DependencyProperty.Register(
            "AllowGroup",
            typeof(bool),
            typeof(GridDataTableProperties),
            new PropertyMetadata(true, OnAllowGroupChanged));

        public static readonly DependencyProperty AllowSortProperty = DependencyProperty.Register(
            "AllowSort",
            typeof(bool),
            typeof(GridDataTableProperties),
            new PropertyMetadata(true, OnAllowSortChanged));

        public static readonly DependencyProperty SortingOptionsProperty = DependencyProperty.Register(
            "SortingOptions",
            typeof(SortingOptions),
            typeof(GridDataTableProperties),
            new PropertyMetadata(SortingOptions.Default, OnSortingOptionsChanged));

        public static readonly DependencyProperty AlternatingRowBackgroundProperty = DependencyProperty.Register(
            "AlternatingRowBackground",
            typeof(Brush),
            typeof(GridDataTableProperties),
            new PropertyMetadata(OnAlternatingRowBackgroundChanged));

        public static readonly DependencyProperty AlternatingRowCountProperty = DependencyProperty.Register(
            "AlternatingRowCount",
            typeof(int),
            typeof(GridDataTableProperties),
            new PropertyMetadata(2));

        public static readonly DependencyProperty AutoPopulateColumnsProperty = DependencyProperty.Register(
            "AutoPopulateColumns",
            typeof(bool),
            typeof(GridDataTableProperties),
            new PropertyMetadata(true, OnAutoPopulateColumnsChanged));

        public static readonly DependencyProperty AutoPopulateRelationsProperty = DependencyProperty.Register(
            "AutoPopulateRelations",
            typeof(bool),
            typeof(GridDataTableProperties),
            new PropertyMetadata(true));

        public static readonly DependencyProperty ColumnSizerProperty = DependencyProperty.Register(
            "ColumnSizer",
            typeof(GridControlLengthUnitType),
            typeof(GridDataTableProperties),
            new PropertyMetadata(GridControlLengthUnitType.None, OnColumnSizerChanged));

        public static readonly DependencyProperty DefaultColumnWidthProperty = DependencyProperty.Register(
            "DefaultColumnWidth",
            typeof(double),
            typeof(GridDataTableProperties),
            new PropertyMetadata(150d, OnDefaultColumnWidthChanged));

        public static readonly DependencyProperty DragIndicatorInnerBrushProperty = DependencyProperty.Register(
            "DragIndicatorInnerBrush",
            typeof(Brush),
            typeof(GridDataTableProperties),
            new PropertyMetadata(OnDragIndicatorInnerBrushChanged));


        public static readonly DependencyProperty DragIndicatorOuterBrushProperty = DependencyProperty.Register(
    "DragIndicatorOuterBrush",
    typeof(Brush),
    typeof(GridDataTableProperties),
    new PropertyMetadata(OnDragIndicatorOuterBrushChanged));


        public static readonly DependencyProperty ItemsSourceProperty = DependencyProperty.Register(
            "ItemsSource",
            typeof(object),
            typeof(GridDataTableProperties),
            new PropertyMetadata(OnItemsSourcePropertyChanged));

        public static readonly DependencyProperty NullFilterTextProperty = DependencyProperty.Register(
            "NullFilterText",
            typeof(string),
            typeof(GridDataTableProperties),
            new PropertyMetadata("None"));

        #region ReserveSpaceForIcons

        /// <summary>
        /// ReserveSpaceForIcons Dependency Property
        /// </summary>
        public static readonly DependencyProperty ReserveSpaceForIconsProperty =
            DependencyProperty.Register("ReserveSpaceForIcons", typeof(bool), typeof(GridDataTableProperties),
                new PropertyMetadata(false));

        /// <summary>
        /// Gets or sets the ReserveSpaceForIcons property. This dependency property 
        /// reserves space for grid data control header icons.
        /// </summary>
        public bool ReserveSpaceForIcons
        {
            get { return (bool)GetValue(ReserveSpaceForIconsProperty); }
            set { SetValue(ReserveSpaceForIconsProperty, value); }
        }

        #endregion

        #region HideEmptyChildGrid

        /// <summary>
        /// HideEmptyChildGrid Dependency Property
        /// </summary>
        public static readonly DependencyProperty HideEmptyChildGridProperty =
            DependencyProperty.Register("HideEmptyChildGrid", typeof(bool), typeof(GridDataTableProperties),
                new PropertyMetadata((bool)false, null));


        /// <summary>
        /// Gets or sets the HideEmptyChildGrid property. This dependency property 
        /// indicates whether empty child grid should be hidded or not.
        /// </summary>
        public bool HideEmptyChildGrid
        {
            get { return (bool)GetValue(HideEmptyChildGridProperty); }
            set { SetValue(HideEmptyChildGridProperty, value); }
        }

        #endregion

        public static readonly DependencyProperty RowBackgroundProperty = DependencyProperty.Register(
            "RowBackground",
            typeof(Brush),
            typeof(GridDataTableProperties),
            new PropertyMetadata(Brushes.Transparent, OnRowBackgroundChanged));

        #region SortWhenGrouped

        /// <summary>
        /// SortWhenGrouped Dependency Property
        /// </summary>
        public static readonly DependencyProperty SortWhenGroupedProperty =
            DependencyProperty.Register("SortWhenGrouped", typeof(bool), typeof(GridDataTableProperties),
                new PropertyMetadata(true,
                    null));

        /// <summary>
        /// Gets or sets the SortWhenGrouped property. This dependency property 
        /// indicates whether sorting occurs while in group
        /// </summary>
        public bool SortWhenGrouped
        {
            get { return (bool)GetValue(SortWhenGroupedProperty); }
            set { SetValue(SortWhenGroupedProperty, value); }
        }

        #endregion

        #region DefaultHeaderRowHeight

        /// <summary>
        /// DefaultHeaderRowHeight Dependency Property
        /// </summary>
        public static readonly DependencyProperty DefaultHeaderRowHeightProperty =
            DependencyProperty.Register("DefaultHeaderRowHeight", typeof(double), typeof(GridDataTableProperties),
                new PropertyMetadata((double)GridDataTableModel.HeaderRowHeight, OnDefaultHeaderRowHeightChanged));

        private static void OnDefaultHeaderRowHeightChanged(DependencyObject obj, DependencyPropertyChangedEventArgs args)
        {
            var tableProperties = obj as GridDataTableProperties;
            if (tableProperties.Model == null)// || tableProperties.IsInSuspend)
            {
                return;
            }

            if (tableProperties.Model.HeaderRows == 1)
                tableProperties.model.RowHeights[0] = tableProperties.DefaultHeaderRowHeight;

            if (tableProperties.Model.HeaderRows > 1)
            {
                for (int i = 0; i < tableProperties.Model.HeaderRows; i++)
                {
                    tableProperties.Model.RowHeights[i] = tableProperties.Model.TableProperties.DefaultHeaderRowHeight;
                }
            }
        }

        /// <summary>
        /// Gets or sets the DefaultHeaderRowHeight property. This dependency property 
        /// indicates default header row height.
        /// </summary>
        public double DefaultHeaderRowHeight
        {
            get { return (double)GetValue(DefaultHeaderRowHeightProperty); }
            set { SetValue(DefaultHeaderRowHeightProperty, value); }
        }

        #endregion

        #region StyleManager

        /// <summary>
        /// Gets or sets the style manager.
        /// </summary>
        /// <value>The style manager.</value>
        [XmlIgnore]
        public GridDataStyleManager StyleManager
        {
            get { return (GridDataStyleManager)GetValue(StyleManagerProperty); }
            set { SetValue(StyleManagerProperty, value); }
        }

        // Using a DependencyProperty as the backing store for StyleManager.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty StyleManagerProperty =
            DependencyProperty.Register("StyleManager", typeof(GridDataStyleManager), typeof(GridDataTableProperties), new PropertyMetadata(null, OnStyleManagerChanged));

        private static void OnStyleManagerChanged(DependencyObject d, DependencyPropertyChangedEventArgs args)
        {
            GridDataTableProperties tableProperties = d as GridDataTableProperties;
            tableProperties.Model.ApplyGridVisualStyle(); 
        }

        

        #endregion

#if !SILVERLIGHT
        #region SortClickAction

        /// <summary>
        /// SortClickAction Dependency Property
        /// </summary>
        public static readonly DependencyProperty SortClickActionProperty = DependencyProperty.Register("SortClickAction", typeof(SortClickAction), typeof(GridDataTableProperties), new PropertyMetadata(SortClickAction.SingleClick));

        /// <summary>
        /// Gets or sets the SortClickAction property. This dependency property 
        /// indicates ....
        /// </summary>
        public SortClickAction SortClickAction
        {
            get { return (SortClickAction)GetValue(SortClickActionProperty); }
            set { SetValue(SortClickActionProperty, value); }
        }

        #endregion
#endif



        //This properties used for paging support.
        public static readonly DependencyProperty EnablePagingProperty = DependencyProperty.Register(
          "EnablePaging",
          typeof(bool),
          typeof(GridDataTableProperties),
          new PropertyMetadata(false, OnEnablePagingPropertyChanged));

        public bool EnablePaging
        {
            get
            {
                return (bool)this.GetValue(GridDataTableProperties.EnablePagingProperty);
            }

            set
            {
                this.SetValue(GridDataTableProperties.EnablePagingProperty, value);
            }
        }

        private static void OnEnablePagingPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs args)
        {
            GridDataTableProperties tableProperties = d as GridDataTableProperties;
            if (tableProperties.Model != null && tableProperties.Model.View != null)
            {
                tableProperties.Model.View.EnablePaging = (bool)args.NewValue;
            }
        }

        public static readonly DependencyProperty IsViewLevelPagingProperty = DependencyProperty.Register(
         "IsViewLevelPaging",
         typeof(bool),
         typeof(GridDataTableProperties),
         new PropertyMetadata(false, OnIsViewLevelPagingPropertyChanged));

        public bool IsViewLevelPaging
        {
            get
            {
                return (bool)this.GetValue(GridDataTableProperties.IsViewLevelPagingProperty);
            }

            set
            {
                this.SetValue(GridDataTableProperties.IsViewLevelPagingProperty, value);
            }
        }

        private static void OnIsViewLevelPagingPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs args)
        {
            GridDataTableProperties tableProperties = d as GridDataTableProperties;
            if (tableProperties.Model != null && tableProperties.Model.View!=null)
            {
                tableProperties.Model.View.IsViewLevelPaging =(bool)args.NewValue;
            }
        }



        public static readonly DependencyProperty ShowFiltersProperty = DependencyProperty.Register(
            "ShowFilters",
            typeof(bool),
            typeof(GridDataTableProperties),
            new PropertyMetadata(OnShowFiltersChanged));

        public bool ShowFilters
        {
            get
            {
                return (bool)this.GetValue(GridDataTableProperties.ShowFiltersProperty);
            }

            set
            {
                this.SetValue(GridDataTableProperties.ShowFiltersProperty, value);
            }
        }

        public static readonly DependencyProperty ShowColumnOptionsProperty = DependencyProperty.Register(
            "ShowColumnOptions",
            typeof(bool),
            typeof(GridDataTableProperties),
            new PropertyMetadata(false, OnShowColumnOptionsChanged));

        private static void OnShowColumnOptionsChanged(DependencyObject d, DependencyPropertyChangedEventArgs args)
        {
            var tableProperties = d as GridDataTableProperties;
            if (tableProperties.Model == null || tableProperties.IsInSuspend)
            {
                return;
            }

            tableProperties.VisibleColumns.ForEach<GridDataVisibleColumn>(v =>
            {
                v.ShowColumnOptions = (bool)args.NewValue;
            });
        }

        public bool ShowColumnOptions
        {
            get
            {
                return (bool)this.GetValue(GridDataTableProperties.ShowColumnOptionsProperty);
            }

            set
            {
                this.SetValue(GridDataTableProperties.ShowColumnOptionsProperty, value);
            }
        }

        public static readonly DependencyProperty ExpandGroupsWhenGroupedProperty = DependencyProperty.Register(
            "ExpandGroupsWhenGrouped",
            typeof(bool),
            typeof(GridDataTableProperties),
            new PropertyMetadata(false, OnExpandGroupsWhenGroupedChanged));

        private static void OnExpandGroupsWhenGroupedChanged(DependencyObject d, DependencyPropertyChangedEventArgs args)
        {
            var tableProperties = d as GridDataTableProperties;
            if (tableProperties.Model == null || tableProperties.IsInSuspend)
            {
                return;
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether ExpandGroupsWhenGrouped is true/false.
        /// </summary>
        /// <value><c>true</c> if [Expands all groups when the column is grouped]; otherwise, <c>false</c>.</value>
        public bool ExpandGroupsWhenGrouped
        {
            get
            {
                return (bool)this.GetValue(GridDataTableProperties.ExpandGroupsWhenGroupedProperty);
            }

            set
            {
                this.SetValue(GridDataTableProperties.ExpandGroupsWhenGroupedProperty, value);
            }
        }

        internal bool SelectFirstRowOnLoad
        {
            get;
            set;
        }

        #region GroupDropAreaHeight

        /// <summary>
        /// GroupDropAreaHeight Dependency Property
        /// </summary>
        public static readonly DependencyProperty GroupDropAreaHeightProperty =
            DependencyProperty.Register("GroupDropAreaHeight", typeof(double), typeof(GridDataTableProperties),
                new PropertyMetadata((double)50, OnGroupDropAreaHeightChanged));

        /// <summary>
        /// Gets or sets the GroupDropAreaHeight property. This dependency property 
        /// indicates height specified for group drop area.
        /// </summary>
        public double GroupDropAreaHeight
        {
            get { return (double)GetValue(GroupDropAreaHeightProperty); }
            set { SetValue(GroupDropAreaHeightProperty, value); }
        }

        private static void OnGroupDropAreaHeightChanged(DependencyObject d, DependencyPropertyChangedEventArgs args)
        {
            GridDataTableProperties tableProperties = d as GridDataTableProperties;
            GridDataControl dataGrid = null;
            if (tableProperties.Model != null && tableProperties.Model.Grid != null)
                dataGrid = tableProperties.Model.Grid.FindParentElementOfType<GridDataControl>();
            else
                return;
            if (dataGrid != null && dataGrid.GroupDropAreaGrid != null)
            {
                dataGrid.GroupDropAreaGrid.AttachParentGrid((GridDataTableModel)dataGrid.InternalGrid.Model, dataGrid.InternalGrid);
            }
        }

        #endregion

        public static readonly DependencyProperty ClearAllOnItemSourceChangeProperty = DependencyProperty.Register(
            "ClearAllOnItemSourceChange",
            typeof(bool),
            typeof(GridDataTableProperties),
            new PropertyMetadata(false));


        public bool ClearAllOnItemSourceChange
        {
            get
            {
                return (bool)this.GetValue(GridDataTableProperties.ClearAllOnItemSourceChangeProperty);
            }
            set
            {
                this.SetValue(GridDataTableProperties.ClearAllOnItemSourceChangeProperty, value);
            }
        }

        #region QueryCellInfoCommand
        
        /// <summary>
        /// Gets or sets the query cell info command.
        /// This command is binded to QueryCellInfo event, so that to use the QueryCellInfoEvent in ViewModel instead of code behind
        /// </summary>
        /// <value>The query cell info command.</value>
        [XmlIgnore]
        internal ICommand QueryCellInfoCommand
        {
            get { return (ICommand)GetValue(QueryCellInfoCommandProperty); }
            set { SetValue(QueryCellInfoCommandProperty, value); }
        }

        // Using a DependencyProperty as the backing store for QueryCellInfoCommand.  This enables to handle the QueryCellInfo event in ViewModel.
        public static readonly DependencyProperty QueryCellInfoCommandProperty =
            DependencyProperty.Register("QueryCellInfoCommand", typeof(ICommand), typeof(GridDataTableProperties), new PropertyMetadata(null));
        #endregion

        #region SortColumnChanging Command
        /// <summary>
        /// Gets or sets the command to invoke when SortColumnChanging event is triggered.
        /// </summary>
        [XmlIgnore]
        public ICommand SortColumnChangingCommand
        {
            get { return (ICommand)GetValue(SortColumnChangingCommandProperty); }
            set { SetValue(SortColumnChangingCommandProperty, value); }
        }

        // Using a DependencyProperty as the backing store for SortColumnChangingCommand.  This enables to handle the SortColumnChanging event in ViewModel.
        public static readonly DependencyProperty SortColumnChangingCommandProperty =
            DependencyProperty.Register("SortColumnChangingCommand", typeof(ICommand), typeof(GridDataTableProperties), new PropertyMetadata(null));

        #endregion
        
        #region HeaderTemplate

        /// <summary>
        /// HeaderTemplate Dependency Property
        /// </summary>
        public static readonly DependencyProperty HeaderTemplateProperty = DependencyProperty.Register("HeaderCellTemplate", typeof(DataTemplate), typeof(GridDataTableProperties), new PropertyMetadata(new PropertyChangedCallback(OnHeaderTemplateChanged)));

        [XmlIgnore]
        public DataTemplate HeaderCellTemplate
        {
            get { return (DataTemplate)GetValue(HeaderTemplateProperty); }
            set { SetValue(HeaderTemplateProperty, value); }
        }

        private bool isHeaderCellTemplateSetBeforeModelLoaded = false;

        /// <summary>
        /// Handles changes to the HeaderTemplate property.
        /// </summary>
        private static void OnHeaderTemplateChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var tableProperties = d as GridDataTableProperties;
            if (tableProperties.Model != null)
            {
                tableProperties.VisibleColumns.ForEach(v =>
                {
                    if (v.HeaderCellTemplate == null)
                    {
                        v.HeaderCellTemplate = (DataTemplate)e.NewValue;
                    }
                });

                tableProperties.Model.InvalidateCell(GridRangeInfo.Row(0));
                tableProperties.Model.InvalidateVisual(true);
            }
            else
            {
                tableProperties.isHeaderCellTemplateSetBeforeModelLoaded = true;
            }
        }

        #endregion

        #region HeaderStyle

        /// <summary>
        /// Gets or sets the header style.
        /// </summary>
        /// <value>The header style.</value>
        [XmlIgnore]
        public Style HeaderStyle
        {
            get { return (Style)GetValue(HeaderStyleProperty); }
            set { SetValue(HeaderStyleProperty, value); }
        }

        public static readonly DependencyProperty HeaderStyleProperty = DependencyProperty.Register("HeaderStyle", typeof(Style), typeof(GridDataTableProperties), null);

        #endregion

        #region AllowNestedGridPadding

        /// <summary>
        /// AllowNestedGridPadding Dependency Property
        /// </summary>
        public static readonly DependencyProperty AllowNestedGridPaddingProperty =
            DependencyProperty.Register("AllowNestedGridPadding", typeof(bool), typeof(GridDataTableProperties),
                new PropertyMetadata((bool)true, null));


        /// <summary>
        /// Gets or sets the AllowNestedGridPadding property. This dependency property 
        /// indicates whether padding value is set for nested grid or not.        
        /// </summary>
        public bool AllowNestedGridPadding
        {
            get { return (bool)GetValue(AllowNestedGridPaddingProperty); }
            set { SetValue(AllowNestedGridPaddingProperty, value); }
        }

        #endregion

        #region IncludeHeaderTextOnCopy

        /// <summary>
        /// IncludeHeaderTextOnCopy Dependency Property
        /// </summary>
        public static readonly DependencyProperty IncludeHeaderTextOnCopyProperty =
            DependencyProperty.Register("IncludeHeaderTextOnCopy", typeof(bool), typeof(GridDataTableProperties),
                new PropertyMetadata(true));

        /// <summary>
        /// Gets or sets the IncludeHeaderTextOnCopy property. This dependency property 
        /// indicates whether header text has to be added while copying.
        /// </summary>
        public bool IncludeHeaderTextOnCopy
        {
            get { return (bool)GetValue(IncludeHeaderTextOnCopyProperty); }
            set { SetValue(IncludeHeaderTextOnCopyProperty, value); }
        }

        #endregion

        #region EnableLegacyStyle

        public bool EnableLegacyStyle
        {
            get { return (bool)GetValue(EnableLegacyStyleProperty); }
            set { SetValue(EnableLegacyStyleProperty, value); }
        }

        // Using a DependencyProperty as the backing store for EnableLegacyStyle.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty EnableLegacyStyleProperty =
            DependencyProperty.Register("EnableLegacyStyle", typeof(bool), typeof(GridDataTableProperties), new PropertyMetadata(OnEnableLegacyStyleChanged));

        private static void OnEnableLegacyStyleChanged(DependencyObject d, DependencyPropertyChangedEventArgs args)
        {
            GridDataTableProperties table = d as GridDataTableProperties;
            table.OnEnableLegacyStyleChanged(args);
        }

        private void OnEnableLegacyStyleChanged(DependencyPropertyChangedEventArgs args)
        {
            if (Model != null)
            {
                Model.GridVisualStyle = GetVisualStyle(this.VisualStyle);

                if (Model.Table != null)
                {
                    Model.Table.RefreshChildVisualStyles(this.VisualStyle);
                }
            }
        }

        #endregion

        #region EnableParentTableStyleToChildTable

        public bool EnableParentStyleToChildTable
        {
            get { return (bool)GetValue(EnableParentStyleToChildTableProperty); }
            set { SetValue(EnableParentStyleToChildTableProperty, value); }
        }
        
        public static readonly DependencyProperty EnableParentStyleToChildTableProperty =
            DependencyProperty.Register("EnableParentStyleToChildTable", typeof(bool), typeof(GridDataTableProperties), new PropertyMetadata(false));

        #endregion
#if !SILVERLIGHT
        #region EnableVisualStyleForEditors
        private bool isEnableVisualStyleForEditorsChangedBeforeModelLoaded = false;
        /// <summary>
        /// Gets / sets VisualStyle for Editor Cell Types.
        /// </summary>
        public bool EnableVisualStyleForEditors
        {
            get
            {
                return (bool)this.GetValue(GridDataTableProperties.EnableVisualStyleForEditorsProperty);
            }

            set
            {
                this.SetValue(GridDataTableProperties.EnableVisualStyleForEditorsProperty, value);
            }
        }

        public static readonly DependencyProperty EnableVisualStyleForEditorsProperty = DependencyProperty.Register(
            "EnableVisualStyleForEditors",
            typeof(bool),
            typeof(GridDataTableProperties),
            new PropertyMetadata(false,OnEnableVisualStyleForEditorsChanged));

        private static void OnEnableVisualStyleForEditorsChanged(DependencyObject obj,DependencyPropertyChangedEventArgs args)
        {
            var tableProperties = obj as GridDataTableProperties;
            if (tableProperties.Model != null)
                tableProperties.Model.TableStyle.IsThemed = true;
            else
                tableProperties.isEnableVisualStyleForEditorsChangedBeforeModelLoaded = true;
        }

        #endregion
#endif

        #region IsLegacyStyleEnabled

        public bool IsLegacyStyleEnabled
        {
            get
            {
                return (EnableLegacyStyle || VisualStyle == VisualStyle.BureauBlack || VisualStyle == VisualStyle.DefaultOffice2007Black || VisualStyle == VisualStyle.DefaultOffice2007Blue
                    || VisualStyle == VisualStyle.DefaultOffice2007Silver || VisualStyle == VisualStyle.Office2003) && (VisualStyle != VisualStyle.SyncfusionTheme && VisualStyle != VisualStyle.Windows7 && VisualStyle!= VisualStyle.Metro);
            }
        }

        #endregion

        #region FilterBehavior

        /// <summary>
        /// FilterBehavior Dependency Property
        /// </summary>
        public static readonly DependencyProperty FilterBehaviorProperty =
            DependencyProperty.Register("FilterBehavior", typeof(FilterBehavior), typeof(GridDataTableProperties),
                new PropertyMetadata(FilterBehavior.StronglyTyped, OnFilterBehaviorPropertyChanged));


        private static void OnFilterBehaviorPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs args)
        {
            GridDataTableProperties tableProperties = d as GridDataTableProperties;
            if (tableProperties.Model != null && tableProperties.VisibleColumns.Count > 0)
            {
                FilterBehavior behavior = (FilterBehavior)args.NewValue;

                foreach (var column in tableProperties.VisibleColumns)
                {
                    column.FilterBehavior = behavior;
                }
            }
        }

        /// <summary>
        /// Gets or sets the FilterBehavior property. This dependency property 
        /// indicates ....
        /// </summary>
        public FilterBehavior FilterBehavior
        {
            get { return (FilterBehavior)GetValue(FilterBehaviorProperty); }
            set { SetValue(FilterBehaviorProperty, value); }
        }


        #endregion


        /// <summary>
        /// Dependency property for ShowRecordPlusMinus
        /// </summary>
        /// <value>
        /// <c>true</c> if [show record plus minus]; otherwise, <c>false</c>.
        /// </value>
        public static readonly DependencyProperty ShowRecordPlusMinusProperty = DependencyProperty.Register(
            "ShowRecordPlusMinus",
            typeof(bool),
            typeof(GridDataTableProperties),
            new PropertyMetadata(true, OnShowRecordPlusMinusChanged));

        public static readonly DependencyProperty ShowRowHeaderProperty = DependencyProperty.Register(
            "ShowRowHeader",
            typeof(bool),
            typeof(GridDataTableProperties),
            new PropertyMetadata(false, OnShowRowHeaderChanged));

        public static readonly DependencyProperty VisualStyleProperty = DependencyProperty.Register(
            "VisualStyle",
            typeof(VisualStyle),
            typeof(GridDataTableProperties),
            new PropertyMetadata(VisualStyle.Default, OnVisualStyleChanged));

        public static readonly DependencyProperty CustomVisualStyleProperty = DependencyProperty.Register(
            "CustomVisualStyle",
            typeof(IGridDataVisualStyle),
            typeof(GridDataTableProperties),
            new PropertyMetadata(null));

        #region DetailsView Template

        /// <summary>
        /// Gets or sets the details view template.
        /// </summary>
        /// <value>The details view template.</value>
        [XmlIgnore]
        public DataTemplate DetailsViewTemplate
        {
            get { return (DataTemplate)GetValue(DetailsViewTemplateProperty); }
            set { SetValue(DetailsViewTemplateProperty, value); }
        }

        // Using a DependencyProperty as the backing store for RowDetailsTemplate.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty DetailsViewTemplateProperty =
            DependencyProperty.Register("DetailsViewTemplate", typeof(DataTemplate), typeof(GridDataTableProperties), new PropertyMetadata(null));

        #endregion

        /// <summary>
        /// Initializes a new instance of the <see cref="GridtableProperties"/> class.
        /// </summary>
        public GridDataTableProperties()
        {
            this.InitializePropertyCollections();
        }

        public void InitializeFrom(GridDataTableProperties other, bool syncModelView)
        {
            this.InitializeFrom(other);
            if (syncModelView)
            {
                var view = this.Model.View;
                if (view == null)
                    return;
                var VisibleColumns = this.Model.GetVisibleColumns();
                using (view.DeferRefresh())
                {
                    view.GroupDescriptions.Clear();
                    foreach (var groupedCol in this.GroupedColumns)
                    {
                        var column = VisibleColumns.FirstOrDefault(C => C.MappingName == groupedCol.ColumnName);
                        if (column != null)
                            view.GroupDescriptions.Add(new PropertyGroupDescription() { PropertyName = groupedCol.ColumnName });
                    }

                    view.SortDescriptions.Clear();
                    foreach (var sortCol in this.SortColumns)
                    {
                        var column = VisibleColumns.FirstOrDefault(C => C.MappingName == sortCol.ColumnName);
                        if (column != null)
                            view.SortDescriptions.Add(new SortDescription(sortCol.ColumnName, sortCol.SortDirection));
                    }

                    var filters = this.VisibleColumns.OfType<IFilterDefinition, GridDataVisibleColumn>();
                    if (filters.Count > 0)
                    {
                        view.FilterPredicates = filters;
                    }

                    view.TableSummaryRows.Clear();
                    foreach (var tSummaryRow in this.TableSummaryRows)
                    {
                        view.TableSummaryRows.Add(tSummaryRow);
                    }

                    // SummaryRows will be added when the groups are created
                }
                var dataGrid = this.model.Grid.FindParentElementOfType<GridDataControl>();
#if !SILVERLIGHT
                //assigning the context menu items.
                if (dataGrid.HeaderContextMenuItems != null)
                    this.Model.TableProperties.HeaderContextMenuItems = dataGrid.HeaderContextMenuItems;
                if (dataGrid.RecordContextMenuItems != null)
                    this.model.TableProperties.RecordContextMenuItems = dataGrid.RecordContextMenuItems;
                if (dataGrid.GroupHeaderContextMenuItems != null)
                    this.model.TableProperties.GroupHeaderContextMenuItems = dataGrid.GroupHeaderContextMenuItems;
#endif
                this.WireEvents();
            }
        }

        public void InitializeFrom(GridDataTableProperties other)
        {
            this.InitializeFromInternal(other, true);
        }

        internal void InitializeFromInternal(GridDataTableProperties other, bool shouldInitializeCollections)
        {
            //// reset sort columns since we do not want it to be populated
            if (shouldInitializeCollections)
            {
                this.InitializePropertyCollections();
            }

            //this.CustomGroupComparer = other.CustomGroupComparer;
            //this.ExpressionFunc = other.ExpressionFunc;
            this.AddNewRowBehaviour = other.AddNewRowBehaviour;
            this.AddNewRowPosition = other.AddNewRowPosition;
            this.AllowDelete = other.AllowDelete;
            this.AllowMultipleRecordDeletion = other.AllowMultipleRecordDeletion;
            this.AllowEdit = other.AllowEdit;
            this.AllowResizeRows = other.AllowResizeRows;
            this.AllowNestedGridPadding = other.AllowNestedGridPadding;
            this.AlphaNumericFilterType = other.AlphaNumericFilterType;
            //this.IsInternalChange = true;
            if (other.AlternatingRowBackground != null)
                this.AlternatingRowBackground = other.AlternatingRowBackground;
            if (other.RowBackground != null)
                this.RowBackground = other.RowBackground;
            //this.IsInternalChange = false;
            this.AlternatingRowCount = other.AlternatingRowCount;
            this.AlternatingRowForeground = other.AlternatingRowForeground;
            this.AutoFocusCurrentItem = other.AutoFocusCurrentItem;
            this.AutoPopulateColumns = other.AutoPopulateColumns;
            this.AutoGenerateColumnsInfo = other.AutoGenerateColumnsInfo;
            this.AutoPopulateRelations = other.AutoPopulateRelations;
            this.EnableLegacyStyle = other.EnableLegacyStyle;
            this.EnableLegacyFiltering = other.EnableLegacyFiltering;
            if (shouldInitializeCollections && other.CaptionSummaryRow != null)
            {
                this.CaptionSummaryRow = other.CaptionSummaryRow;
                this.CaptionSummaryRow = new GridDataSummaryRow();
                this.CaptionSummaryRow.InitializeForm(other.CaptionSummaryRow);
            }

            this.ClearAllOnItemSourceChange = other.ClearAllOnItemSourceChange;
            if (shouldInitializeCollections)
            {
                other.ConditionalFormats.ForEach<GridDataConditionalFormat>(cf =>
                {
                    var conditionalFormat = new GridDataConditionalFormat();
                    conditionalFormat.SetTableModel(this.Model);
                    conditionalFormat.InitializeFrom(cf);
                    this.ConditionalFormats.Add(conditionalFormat);
                });
            }
            this.CustomVisualStyle = other.CustomVisualStyle;
            this.DefaultColumnWidth = other.DefaultColumnWidth;
            this.DefaultHeaderRowHeight = other.DefaultHeaderRowHeight;
            this.DragIndicatorInnerBrush = other.DragIndicatorInnerBrush;
#if !SILVERLIGHT
            this.DragIndicatorOuterBrush = other.DragIndicatorOuterBrush;
            this.EnableOptimizations = other.EnableOptimizations;
#endif
            this.EnableTriStateSorting = other.EnableTriStateSorting;
            this.FilterBehavior = other.FilterBehavior;
            this.FilterBarMode = other.FilterBarMode;
            this.FilterBarPredicateType = other.FilterBarPredicateType;
            this.FooterColumns = other.FooterColumns;
            this.FooterRows = other.FooterRows;
            this.FrozenColumns = other.FrozenColumns;
            this.FrozenRows = other.FrozenRows;
            this.HideEmptyChildGrid = other.HideEmptyChildGrid;
            this.GroupCaptionText = other.GroupCaptionText;
            this.GroupDropAreaHeight = other.GroupDropAreaHeight;
            if (shouldInitializeCollections)
            {
                other.GroupedColumns.ForEach<GridDataGroupColumn>(g =>
                {
                    var column = new GridDataGroupColumn();
                    column.InitializeFrom(g);
                    this.GroupedColumns.Add(column);
                });
            }
            this.HeaderColumns = other.HeaderColumns;
            this.HeaderRows = other.HeaderRows;
            this.IsGroupsExpanded = other.IsGroupsExpanded;
#if !SILVERLIGHT
            this.IsSynchronizedWithCurrentItem = other.IsSynchronizedWithCurrentItem;
#endif
#if SyncfusionFramework4_0
            this.IsDynamicItemsSource = other.IsDynamicItemsSource;
#endif
            this.IncludeHeaderTextOnCopy = other.IncludeHeaderTextOnCopy;
            this.NotifyPropertyChanges = other.NotifyPropertyChanges;
            // this.IsSynchronizedWithCurrentItem = other.IsSynchronizedWithCurrentItem; - Dont set this property
            // this.ItemsSource = other.ItemsSource; - Don't set this property
            this.NullFilterText = other.NullFilterText;
            this.OneTimePopulateRelations = other.OneTimePopulateRelations;
            if (shouldInitializeCollections)
            {
                other.Relations.ForEach<GridDataRelation>(r =>
                {
                    var relation = new GridDataRelation();
                    relation.InitializeFrom(r);
                    this.Relations.Add(relation);
                });
            }
            this.DetailsViewTemplate = other.DetailsViewTemplate;
            this.RetainSortWhenUnGrouped = other.RetainSortWhenUnGrouped;
            this.ReserveSpaceForIcons = other.ReserveSpaceForIcons;
            this.RowForeground = other.RowForeground;
            this.ShowAddNewRow = other.ShowAddNewRow;
            this.ShowFilterBar = other.ShowFilterBar;
            this.ShowGroupCaptionPlusMinus = other.ShowGroupCaptionPlusMinus;
            this.ShowGroupDropArea = other.ShowGroupDropArea;
            this.ShowGroupSummaries = other.ShowGroupSummaries;
            this.ShowGroupSummaryInCaption = other.ShowGroupSummaryInCaption;
#if !SILVERLIGHT
            this.ShowHoveringBackground = other.ShowHoveringBackground;
#endif            
            this.ShowRecordPlusMinus = other.ShowRecordPlusMinus;
            this.ShowRowHeader = other.ShowRowHeader;
            this.ShowRowHeaderArrow = other.ShowRowHeaderArrow;
            this.ShowTableSummaries = other.ShowTableSummaries;
            this.SelectFirstRowOnLoad = other.SelectFirstRowOnLoad;
            //this.ShowTooltips = other.ShowTooltips;
            this.SortWhenGrouped = other.SortWhenGrouped;
#if !SILVERLIGHT
            this.SortClickAction = other.SortClickAction;
#endif
            if (shouldInitializeCollections)
            {
                other.SortColumns.ForEach<GridDataSortColumn>(sortColumn =>
                {
                    var sortCol = new GridDataSortColumn();
                    sortCol.InitializeFrom(sortColumn);
                    this.SortColumns.Add(sortCol);
                });
                other.StackedHeaderRows.ForEach<GridDataStackedHeaderRow>(stackedHeaderRow =>
                {
                    var headerRow = new GridDataStackedHeaderRow();
                    headerRow.InitializeFrom(stackedHeaderRow);
                    this.StackedHeaderRows.Add(headerRow);
                });
                other.SummaryRows.ForEach<GridDataSummaryRow>(summary =>
                {
                    var summaryRow = new GridDataSummaryRow();
                    summaryRow.SetTableModel(this.Model);
                    summaryRow.InitializeForm(summary);
                    this.SummaryRows.Add(summaryRow);
                });
            }
            this.TableSummaryPosition = other.TableSummaryPosition;
            if (shouldInitializeCollections)
            {
                other.TableSummaryRows.ForEach<GridDataSummaryRow>(s =>
                {
                    var summary = new GridDataSummaryRow();
                    summary.SetTableModel(this.Model);
                    summary.InitializeForm(s);
                    this.TableSummaryRows.Add(summary);
                });
            }
            this.VisibleColumns.SuspendEvents(); // ensure we are suspending events here
   
            if (shouldInitializeCollections)
            {
                // var allVisibleColumns = Model.View != null ? this.Model.GetVisibleColumns() : null; Unused local variable
                other.VisibleColumns.ForEach<GridDataVisibleColumn>(v =>
                {
                    var col = !v.IsUnbound ? new GridDataVisibleColumn() : new GridDataUnboundVisibleColumn();
                    col.InitializeFrom(v);
                    col.SetTableModel(this.Model);
                    //var column = allVisibleColumns != null ? allVisibleColumns.FirstOrDefault(C => C.MappingName == col.MappingName) : null;
                    if (this.VisibleColumns.ValidateVisibleColumn(col))//column != null || allVisibleColumns == null || col.IsUnbound)
                    {
                        this.VisibleColumns.Add(col);
                        this.VisibleColumns.WireVisibleColumnDescriptor(col);
                    }
                });
            }
            
            this.UpdateMode = other.UpdateMode;
            this.VisibleColumns.ResumeEvents();
            this.AllowDragColumns = other.AllowDragColumns;
            this.AllowGroup = other.AllowGroup;
            this.AllowSort = other.AllowSort;
            this.AllowResizeColumns = other.AllowResizeColumns;
            this.ShowColumnOptions = other.ShowColumnOptions;
            this.ShowFilters = other.ShowFilters;
            this.ColumnSizer = other.ColumnSizer;
            this.HeaderCellTemplate = other.HeaderCellTemplate;
            this.CustomVisualStyle = other.CustomVisualStyle;
            this.VisualStyle = other.VisualStyle;
        }

        private void InitializePropertyCollections()
        {
            this.UnwireEvents();
#if !SILVERLIGHT
            this.HeaderContextMenuItems = new ObservableCollection<object>();
            this.GroupHeaderContextMenuItems = new ObservableCollection<object>();
            this.RecordContextMenuItems = new ObservableCollection<object>();
            this.SortColumns = new FreezableCollection<GridDataSortColumn>();
            this.GroupedColumns = new FreezableCollection<GridDataGroupColumn>();
            this.Relations = new FreezableCollection<GridDataRelation>();
            this.SummaryRows = new FreezableCollection<GridDataSummaryRow>();
            this.TableSummaryRows = new FreezableCollection<GridDataSummaryRow>();
            this.StackedHeaderRows = new FreezableCollection<GridDataStackedHeaderRow>();
            this.ConditionalFormats = new FreezableCollection<GridDataConditionalFormat>();
#else
            this.SortColumns = new ObservableCollection<GridDataSortColumn>();
            this.GroupedColumns = new ObservableCollection<GridDataGroupColumn>();
            this.Relations = new ObservableCollection<GridDataRelation>();
            this.SummaryRows = new ObservableCollection<GridDataSummaryRow>();
            this.TableSummaryRows = new ObservableCollection<GridDataSummaryRow>();
            this.StackedHeaderRows = new ObservableCollection<GridDataStackedHeaderRow>();
            this.ConditionalFormats = new ObservableCollection<GridDataConditionalFormat>();
#endif
            this.VisibleColumns = new GridDataVisibleColumns();
            this.VisibleColumns.SetTableModel(this.Model);
        }

        public Position AddNewRowPosition
        {
            get
            {
                return (Position)this.GetValue(GridDataTableProperties.AddNewRowPositionProperty);
            }

            set
            {
                this.SetValue(GridDataTableProperties.AddNewRowPositionProperty, value);
            }
        }
        /// <summary>
        /// Gets or sets  the UnboundRowPosition. 
        /// </summary>
        /// <value></value>
        /// <remarks></remarks>
        public Position UnboundRowPosition
            {
            get
                {
                return (Position)this.GetValue(GridDataTableProperties.UnboundRowPositionProperty);
                }

            set
                {
                this.SetValue(GridDataTableProperties.UnboundRowPositionProperty, value);
                }
            }
#if !SILVERLIGHT      
        // The three properties are used to maintain the value of Column chooser MouseHoverEvent in GridDataColumn Chooser Mouse Controller.                             
        internal bool CanDropOnColumnChooser = false;

        internal string DraggingColumnName = "";

        internal int DragColumnIndex = -1;
#endif
        public bool AllowDelete
        {
            get
            {
                return (bool)this.GetValue(GridDataTableProperties.AllowDeleteProperty);
            }

            set
            {
                this.SetValue(GridDataTableProperties.AllowDeleteProperty, value);
            }
        }

        public bool AllowMultipleRecordDeletion
        {
            get
            {
                return (bool)GetValue(GridDataTableProperties.AllowMultipleRecordDeletionProperty);
            }
            set
            {
                SetValue(GridDataTableProperties.AllowMultipleRecordDeletionProperty, value);
            }
        }



        public bool AllowDragColumns
        {
            get
            {
                return (bool)this.GetValue(GridDataTableProperties.AllowDragColumnsProperty);
            }

            set
            {
                this.SetValue(GridDataTableProperties.AllowDragColumnsProperty, value);
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether [allow edit].
        /// </summary>
        /// <value><c>true</c> if [allow edit]; otherwise, <c>false</c>.</value>
        public bool AllowEdit
        {
            get
            {
                return (bool)this.GetValue(GridDataTableProperties.AllowEditProperty);
            }

            set
            {
                this.SetValue(GridDataTableProperties.AllowEditProperty, value);
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether [allow group].
        /// </summary>
        /// <value><c>true</c> if [allow group]; otherwise, <c>false</c>.</value>
        public bool AllowGroup
        {
            get
            {
                return (bool)this.GetValue(GridDataTableProperties.AllowGroupProperty);
            }

            set
            {
                this.SetValue(GridDataTableProperties.AllowGroupProperty, value);
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether [allow sort].
        /// </summary>
        /// <value><c>true</c> if [allow sort]; otherwise, <c>false</c>.</value>
        public bool AllowSort
        {
            get
            {
                return (bool)this.GetValue(GridDataTableProperties.AllowSortProperty);
            }

            set
            {
                this.SetValue(GridDataTableProperties.AllowSortProperty, value);
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether [allow sort while editing].
        /// </summary>
        public SortingOptions SortingOptions
        {
            get
            {
                return (SortingOptions)this.GetValue(GridDataTableProperties.SortingOptionsProperty);
            }

            set
            {
                this.SetValue(GridDataTableProperties.SortingOptionsProperty, value);
            }
        }

        internal bool IsAlternatingRowBackgroundChangedExternally = false;
        internal bool IsInternalChange = false;
        /// <summary>
        /// Gets or sets the alternating row background.
        /// </summary>
        /// <value>The alternating row background.</value>
        [XmlIgnore]
        public Brush AlternatingRowBackground
        {
            get
            {
                return this.GetValue(GridDataTableProperties.AlternatingRowBackgroundProperty) as Brush;
            }

            set
            {
                this.SetValue(GridDataTableProperties.AlternatingRowBackgroundProperty, value);
            }
        }

#if !SILVERLIGHT
        [XmlElement("AlternatingRowBackground")]
        public string SerializableAlternatingRowBackground
        {
            get
            {
                if (AlternatingRowBackground == null)
                    AlternatingRowBackground = Brushes.Transparent;
                return this.GetValue(GridDataTableProperties.AlternatingRowBackgroundProperty).ToString();
            }


            set
            {
                this.SetValue(GridDataTableProperties.AlternatingRowBackgroundProperty, (SolidColorBrush)new BrushConverter().ConvertFromString(value));

            }
        }
#endif
        #region AlternatingRowForeGround
        /// <summary>
        /// Gets or sets the alternating row background.
        /// </summary>
        /// <value>The alternating row background.</value>
        [XmlIgnore]
        public Brush AlternatingRowForeground
        {
            get
            {
                return this.GetValue(GridDataTableProperties.AlternatingRowForegroundProperty) as Brush;
            }

            set
            {
                this.SetValue(GridDataTableProperties.AlternatingRowForegroundProperty, value);
            }
        }

        public static readonly DependencyProperty AlternatingRowForegroundProperty = DependencyProperty.Register(
            "AlternatingRowForeground",
            typeof(Brush),
            typeof(GridDataTableProperties),
            new PropertyMetadata(OnAlternatingRowForegroundChanged));

        private static void OnAlternatingRowForegroundChanged(DependencyObject d, DependencyPropertyChangedEventArgs args)
        {
            GridDataTableProperties tableProperties = d as GridDataTableProperties;
            if (tableProperties.Model != null)
            {
                tableProperties.AlternatingRowForeground = (Brush)args.NewValue;
                tableProperties.Model.InvalidateDisplay();
            }
        }
        #endregion

        public int AlternatingRowCount
        {
            get
            {
                return (int)this.GetValue(GridDataTableProperties.AlternatingRowCountProperty);
            }

            set
            {
                this.SetValue(GridDataTableProperties.AlternatingRowCountProperty, value);
            }
        }

        /// <summary>
        /// The property determines whether the VisibleColumns are to be auto populated or not
        /// </summary>
        /// <value><c>true</c> if [auto populate columns]; otherwise, <c>false</c>.</value>
        public bool AutoPopulateColumns
        {
            get
            {
                return (bool)this.GetValue(GridDataTableProperties.AutoPopulateColumnsProperty);
            }

            set
            {
                this.SetValue(GridDataTableProperties.AutoPopulateColumnsProperty, value);
            }
        }

        /// <summary>
        ///  The property determines whether the Column Type are to be auto generated or not
        ///  Setting of this property actually set the column type to VisibleColumn based on property type bound to columns
        /// </summary>
        public bool AutoGenerateColumnsInfo
        {
            get { return (bool)GetValue(AutoGenerateColumnsInfoProperty); }
            set { SetValue(AutoGenerateColumnsInfoProperty, value); }
        }

        public static readonly DependencyProperty AutoGenerateColumnsInfoProperty =
            DependencyProperty.Register("AutoGenerateColumnsInfo", typeof(bool), typeof(GridDataTableProperties), new PropertyMetadata(false));

        /// <summary>
        /// Gets or sets a value indicating whether [auto populate relations].
        /// </summary>
        /// <value>
        /// <c>true</c> if [auto populate relations]; otherwise, <c>false</c>.
        /// </value>
        public bool AutoPopulateRelations
        {
            get
            {
                return (bool)this.GetValue(GridDataTableProperties.AutoPopulateRelationsProperty);
            }

            set
            {
                this.SetValue(GridDataTableProperties.AutoPopulateRelationsProperty, value);
            }
        }


        #region AutoFocusCurrentItem (DependencyProperty)

        /// <summary>
        /// Gets / Sets the AutoFocusCurrentItem property.
        /// </summary>
        public bool AutoFocusCurrentItem
        {
            get { return (bool)GetValue(AutoFocusCurrentItemProperty); }
            set { SetValue(AutoFocusCurrentItemProperty, value); }
        }

        public static readonly DependencyProperty AutoFocusCurrentItemProperty = DependencyProperty.Register("AutoFocusCurrentItem", typeof(bool), typeof(GridDataTableProperties), new PropertyMetadata(true, OnAutoFocusCurrentItemChanged));

        private static void OnAutoFocusCurrentItemChanged(DependencyObject dpo, DependencyPropertyChangedEventArgs args)
        {
            var tableProperties = dpo as GridDataTableProperties;
            if (tableProperties.Model == null || tableProperties.IsInSuspend)
            {
                return;
            }

            if (tableProperties.Model != null && tableProperties.Model.View == null)
            {
                return;
            }

            var value = (bool)args.NewValue;
            if (value)
            {
                var selectedRecordIndex = tableProperties.Model.CurrencyManager.CurrentRecordIndex;
                var view = tableProperties.Model.View;
                if (selectedRecordIndex > -1 && view.CurrentPosition != selectedRecordIndex)
                {
                    view.MoveCurrentToPosition(selectedRecordIndex);
                }
            }
        }

        #endregion


        public GridControlLengthUnitType ColumnSizer
        {
            get
            {
                return (GridControlLengthUnitType)this.GetValue(GridDataTableProperties.ColumnSizerProperty);
            }
            set
            {
                this.SetValue(GridDataTableProperties.ColumnSizerProperty, value);
            }
        }


        #region FooterRows
        private bool isFooterRowsSetBeforeModelLoaded = false;
        public static readonly DependencyProperty FooterRowsProperty = DependencyProperty.Register("FooterRows", typeof(int), typeof(GridDataTableProperties), new PropertyMetadata(0, OnFooterRowsChanged));

        private static void OnFooterRowsChanged(DependencyObject d, DependencyPropertyChangedEventArgs args)
        {
            GridDataTableProperties tableProperties = d as GridDataTableProperties;
            if (tableProperties.Model != null)
            {
                tableProperties.Model.Table.SetFooterRows();
                //if (tableProperties.TableSummaryPosition == Position.Bottom)
                //{
                //    tableProperties.Model.FooterRows = (int)args.NewValue + tableProperties.TableSummaryRows.Count;
                //}
            }
            else
            {
               tableProperties.isFooterRowsSetBeforeModelLoaded = true;
            }
        }

        public int FooterRows
        {
            get
            {
                return (int)this.GetValue(GridDataTableProperties.FooterRowsProperty);
            }
            set
            {
                this.SetValue(GridDataTableProperties.FooterRowsProperty, value);
            }
        }

        #endregion

        #region HeaderRows

        public static readonly DependencyProperty HeaderRowsProperty = DependencyProperty.Register("HeaderRows", typeof(int), typeof(GridDataTableProperties), new PropertyMetadata(1, OnHeaderRowsChanged));


        bool isHeaderRowsChangedBeforeModelLoaded = false;

        private static void OnHeaderRowsChanged(DependencyObject d, DependencyPropertyChangedEventArgs args)
        {
            GridDataTableProperties tableProperties = d as GridDataTableProperties;
            if (tableProperties.Model != null)
            {
                if (tableProperties.TableSummaryPosition == Position.Bottom)
                {
                    tableProperties.Model.HeaderRows = (int)args.NewValue + tableProperties.StackedHeaderRows.Count;
                    tableProperties.Model.FrozenRows = tableProperties.Model.HeaderRows;
                }
                else
                {
                    tableProperties.Model.HeaderRows = (int)args.NewValue + tableProperties.StackedHeaderRows.Count + tableProperties.TableSummaryRows.Count;
                    tableProperties.Model.FrozenRows = tableProperties.Model.HeaderRows;
                }
            }
            else
            {
                tableProperties.isHeaderRowsChangedBeforeModelLoaded = true;
            }

        }

        public int HeaderRows
        {
            get
            {
                return (int)this.GetValue(GridDataTableProperties.HeaderRowsProperty);
            }
            set
            {
                this.SetValue(GridDataTableProperties.HeaderRowsProperty, value);
            }
        }

        #endregion

        #region FooterColumns

        private bool isFooterColumnsChangedBeforeModelLoaded = false;
        public static readonly DependencyProperty FooterColumnsProperty = DependencyProperty.Register("FooterColumns", typeof(int), typeof(GridDataTableProperties), new PropertyMetadata(0, OnFooterColumnsChanged));

        private static void OnFooterColumnsChanged(DependencyObject d, DependencyPropertyChangedEventArgs args)
        {
            GridDataTableProperties tableProperties = d as GridDataTableProperties;
            if (tableProperties.Model != null)
            {
                tableProperties.Model.FooterColumns = (int)args.NewValue;
            }
            else
            {
                tableProperties.isFooterColumnsChangedBeforeModelLoaded = true;
            }
        }

        public int FooterColumns
        {
            get
            {
                return (int)this.GetValue(GridDataTableProperties.FooterColumnsProperty);
            }
            set
            {
                this.SetValue(GridDataTableProperties.FooterColumnsProperty, value);
            }
        }

        #endregion

        #region HeaderColumns

        public static readonly DependencyProperty HeaderColumnsProperty = DependencyProperty.Register("HeaderColumns", typeof(int), typeof(GridDataTableProperties), new PropertyMetadata(0, OnHeaderColumnsChanged));

        private static void OnHeaderColumnsChanged(DependencyObject d, DependencyPropertyChangedEventArgs args)
        {
            GridDataTableProperties tableProperties = d as GridDataTableProperties;
            if (tableProperties.Model != null)
            {
                tableProperties.Model.HeaderColumns = (int)args.NewValue + (tableProperties.ShowRowHeader == true ? 1 : 0);
            }
            else
            {
                tableProperties.isHeaderColumnsSetBeforeModelLoaded = true;
            }
        }
        private bool isHeaderColumnsSetBeforeModelLoaded = false;
        public int HeaderColumns
        {
            get
            {
                return (int)this.GetValue(GridDataTableProperties.HeaderColumnsProperty);
            }
            set
            {
                this.SetValue(GridDataTableProperties.HeaderColumnsProperty, value);
            }
        }

        #endregion

        #region FrozenRows (DependencyProperty)

        /// <summary>
        /// Gets / Sets frozen rows
        /// </summary>
        public int FrozenRows
        {
            get { return (int)GetValue(FrozenRowsProperty); }
            set { SetValue(FrozenRowsProperty, value); }
        }

        public static readonly DependencyProperty FrozenRowsProperty = DependencyProperty.Register("FrozenRows", typeof(int), typeof(GridDataTableProperties), new PropertyMetadata(1, OnFrozenRowsChanged));

        private static void OnFrozenRowsChanged(DependencyObject dpo, DependencyPropertyChangedEventArgs args)
        {
            var tableProperties = dpo as GridDataTableProperties;
            if (tableProperties.Model != null)
            {
                tableProperties.Model.Table.SetFrozenRows();
            }
        }

        #endregion

        #region FrozenColumns (DependencyProperty)

        /// <summary>
        /// Gets / sets the FrozenColumns
        /// </summary>
        public int FrozenColumns
        {
            get { return (int)GetValue(FrozenColumnsProperty); }
            set { SetValue(FrozenColumnsProperty, value); }
        }

        public static readonly DependencyProperty FrozenColumnsProperty = DependencyProperty.Register("FrozenColumns", typeof(int), typeof(GridDataTableProperties), new PropertyMetadata(0, OnFrozenColumnsChanged));

        private bool isFrozenColumnsSetBeforeModelLoaded = false;
        private static void OnFrozenColumnsChanged(DependencyObject dpo, DependencyPropertyChangedEventArgs args)
        {
            var tableProperties = dpo as GridDataTableProperties;
            if (tableProperties.Model != null)
            {
                tableProperties.Model.Table.SetFrozenColumns();
                //tableProperties.Model.FrozenColumns = (int)args.NewValue;
            }
            else
            {
                tableProperties.isFrozenColumnsSetBeforeModelLoaded = true;
            }
        }

        #endregion

        /// <summary>
        /// Gets or sets the default width of the column.
        /// </summary>
        /// <value>The default width of the column.</value>
        public double DefaultColumnWidth
        {
            get
            {
                return (double)this.GetValue(GridDataTableProperties.DefaultColumnWidthProperty);
            }

            set
            {
                this.SetValue(GridDataTableProperties.DefaultColumnWidthProperty, value);
            }
        }

        [XmlIgnore]
        public Brush DragIndicatorInnerBrush
        {
            get
            {
                return (Brush)this.GetValue(GridDataTableProperties.DragIndicatorInnerBrushProperty);
            }

            set
            {
                this.SetValue(GridDataTableProperties.DragIndicatorInnerBrushProperty, value);
            }
        }


        [XmlIgnore]
        public Brush DragIndicatorOuterBrush
        {
            get
            {
                return (Brush)this.GetValue(GridDataTableProperties.DragIndicatorOuterBrushProperty);
            }

            set
            {
                this.SetValue(GridDataTableProperties.DragIndicatorOuterBrushProperty, value);
            }
        }


        [XmlIgnore]
        public object ItemsSource
        {
            get
            {
                return this.GetValue(GridDataTableProperties.ItemsSourceProperty) as IEnumerable;
            }

            set
            {
                this.SetValue(GridDataTableProperties.ItemsSourceProperty, value);
            }
        }

#if SyncfusionFramework4_0
        #region IsDynamicItemsSource (DependencyProperty)

        /// <summary>
        /// Gets / Sets the IsDynamicItemsSource property. Set this to true if ItemsSource is a collection of 'dynamic' objects.
        /// </summary>
        public bool IsDynamicItemsSource
        {
            get { return (bool)GetValue(GridDataTableProperties.IsDynamicItemsSourceProperty); }
            set { SetValue(GridDataTableProperties.IsDynamicItemsSourceProperty, value); }
        }

        public static readonly DependencyProperty IsDynamicItemsSourceProperty = DependencyProperty.Register("IsDynamicItemsSource", typeof(bool), typeof(GridDataTableProperties), new PropertyMetadata(false));

        #endregion
#endif

#if !SILVERLIGHT
        public static readonly DependencyProperty IsSynchronizedWithCurrentItemProperty = DependencyProperty.Register("IsSynchronizedWithCurrentItem", typeof(bool), typeof(GridDataTableProperties), new PropertyMetadata(false));

        /// <summary>
        /// Gets or sets a value indicating whether this instance is synchronized with current item.
        /// </summary>
        /// <value>
        /// 	<c>true</c> if this instance is synchronized with current item; otherwise, <c>false</c>.
        /// </value>
        public bool IsSynchronizedWithCurrentItem
        {
            get
            {
                return (bool)this.GetValue(GridDataTableProperties.IsSynchronizedWithCurrentItemProperty);
            }

            set
            {
                this.SetValue(GridDataTableProperties.IsSynchronizedWithCurrentItemProperty, value);
            }
        }
#endif

        private GridDataTableModel model;

        internal void SetTableModel(GridDataTableModel model)
        {
            this.model = model;
            this.VisibleColumns.SetTableModel(model);
            if (this.isHeaderColumnsSetBeforeModelLoaded)
            {
                this.Model.HeaderColumns = this.HeaderColumns + (this.ShowRowHeader == true ? 1 : 0);
            }
            if (this.isFrozenColumnsSetBeforeModelLoaded)
            {
                model.Table.SetFrozenColumns();
                //model.FrozenColumns = this.FrozenColumns;
            }
#if !SILVERLIGHT
            if (isEnableVisualStyleForEditorsChangedBeforeModelLoaded)
            {
                model.TableStyle.IsThemed = true;
            }
#endif
            if (this.isFooterRowsSetBeforeModelLoaded)
            {
                model.Table.SetFooterRows();
            }
            if (this.isFooterColumnsChangedBeforeModelLoaded)
            {
                Model.FooterColumns = this.FooterColumns;
            }
            if (this.FilterBehavior != Linq.FilterBehavior.StronglyTyped)
            {
                this.VisibleColumns.ForEach(v =>
                {
                    v.FilterBehavior = this.FilterBehavior;
                });
            }
            if (this.isHeaderCellTemplateSetBeforeModelLoaded)
            {
                this.VisibleColumns.ForEach(v =>
                {
                    if (v.HeaderCellTemplate == null)
                    {
                        v.HeaderCellTemplate = this.HeaderCellTemplate;
                    }
                });

                this.Model.InvalidateCell(GridRangeInfo.Row(0));
                this.Model.InvalidateVisual(true);
            }

            this.model.RowHeights[0] = this.DefaultHeaderRowHeight;

            if (this.isHeaderRowsChangedBeforeModelLoaded)
            {
                if (this.TableSummaryPosition == Position.Bottom)
                {
                    this.Model.HeaderRows = this.HeaderRows + this.StackedHeaderRows.Count;
                    this.Model.FrozenRows = this.Model.HeaderRows;
                }
                else
                {
                    this.Model.HeaderRows = this.HeaderRows + this.StackedHeaderRows.Count + this.TableSummaryRows.Count;
                    this.Model.FrozenRows = this.Model.HeaderRows;
                }
            }

        }

        [XmlIgnore]
        internal GridDataTableModel Model
        {
            get
            {
                return this.model;
            }
        }

        public string NullFilterText
        {
            get
            {
                return (string)this.GetValue(GridDataTableProperties.NullFilterTextProperty);
            }

            set
            {
                this.SetValue(GridDataTableProperties.NullFilterTextProperty, value);
            }
        }
#if !SILVERLIGHT
        public static readonly DependencyProperty RelationsProperty = DependencyProperty.Register(
       "Relations",
       typeof(FreezableCollection<GridDataRelation>),
       typeof(GridDataTableProperties), new PropertyMetadata(OnRelationPropertyChanged));


        private static void OnRelationPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs args)
        {
            GridDataTableProperties tableProperties = d as GridDataTableProperties;
            if (tableProperties.Model != null && tableProperties.Model.Grid != null && tableProperties.Model.Grid.IsLoaded && tableProperties.Model.Table != null)
            {
                if (tableProperties.NeedToRefresh)
                    tableProperties.Model.Table.Refresh();
            }
        }

        public bool NeedToRefresh = false;

        /// <summary>
        /// Gets or sets the relational columns.
        /// </summary>
        /// <value>The relational columns.</value>
        [XmlArray("Relations")]
        [XmlArrayItem("GridDataRelation", typeof(GridDataRelation))]
        public FreezableCollection<GridDataRelation> Relations
        {
            get
            {
                return (FreezableCollection<GridDataRelation>)GetValue(GridDataTableProperties.RelationsProperty);
            }
            set
            {
                SetValue(GridDataTableProperties.RelationsProperty, value);
            }
        }
#else
        /// <summary>
        /// Gets or sets the relational columns.
        /// </summary>
        /// <value>The relational columns.</value>
        [XmlArray("Relations")]
        [XmlArrayItem("GridDataRelation", typeof(GridDataRelation))]
        public ObservableCollection<GridDataRelation> Relations
        {
            get;
            set;
        }

#endif

        public static readonly DependencyProperty OneTimePopulateRelationsProperty = DependencyProperty.Register(
            "OneTimePopulateRelations",
            typeof(bool),
            typeof(GridDataTableProperties),
            new PropertyMetadata(false));

        public bool OneTimePopulateRelations
        {
            get
            {
                return (bool)this.GetValue(GridDataTableProperties.OneTimePopulateRelationsProperty);
            }

            set
            {
                this.SetValue(GridDataTableProperties.OneTimePopulateRelationsProperty, value);
            }
        }

        internal bool IsRowBackgroundChangedExternally = false;
        /// <summary>
        /// Gets or sets the row background.
        /// </summary>
        /// <value>The row background.</value>
        [XmlIgnore]
        public Brush RowBackground
        {
            get
            {
                return this.GetValue(GridDataTableProperties.RowBackgroundProperty) as Brush;
            }

            set
            {
                this.SetValue(GridDataTableProperties.RowBackgroundProperty, value);
            }
        }
#if !SILVERLIGHT

        [XmlElement("RowBackground")]
        public string SerializableRowBackground
        {

            get
            {
                return this.GetValue(GridDataTableProperties.RowBackgroundProperty).ToString();

            }


            set
            {
                this.SetValue(GridDataTableProperties.RowBackgroundProperty, (SolidColorBrush)new BrushConverter().ConvertFromString(value));

            }
        }
#endif
        #region RowForeground

        /// <summary>
        /// Gets or sets the row background.
        /// </summary>
        /// <value>The row background.</value>
        [XmlIgnore]
        public Brush RowForeground
        {
            get
            {
                return this.GetValue(GridDataTableProperties.RowForegroundProperty) as Brush;
            }

            set
            {
                this.SetValue(GridDataTableProperties.RowForegroundProperty, value);
            }
        }

        public static readonly DependencyProperty RowForegroundProperty = DependencyProperty.Register(
            "RowForeground",
            typeof(Brush),
            typeof(GridDataTableProperties),
            new PropertyMetadata(OnRowForegroundChanged));

        private static void OnRowForegroundChanged(DependencyObject d, DependencyPropertyChangedEventArgs args)
        {
            GridDataTableProperties tableProperties = d as GridDataTableProperties;
            if (tableProperties.Model != null)
            {
                tableProperties.RowForeground = (Brush)args.NewValue;
                tableProperties.Model.InvalidateDisplay();
            }
        }

        #endregion

        public GridDataFilterBarMode FilterBarMode
        {
            get { return (GridDataFilterBarMode)GetValue(FilterBarModeProperty); }
            set { SetValue(FilterBarModeProperty, value); }
        }

        public bool ShowFilterBar
        {
            get { return (bool)GetValue(ShowFilterBarProperty); }
            set { SetValue(ShowFilterBarProperty, value); }
        }

        public AlphaNumericFilterType AlphaNumericFilterType
        {
            get { return (AlphaNumericFilterType)GetValue(AlphaNumericFilterTypeProperty); }
            set { SetValue(AlphaNumericFilterTypeProperty, value); }
        }
#if !SILVERLIGHT
        [XmlIgnore]
        public FilterPanePosition FilterPanePosition
        {
            get { return (FilterPanePosition)GetValue(FilterPanePositionProperty); }
            set { SetValue(FilterPanePositionProperty, value); }
        }
#endif
        public bool ShowAddNewRow
        {
            get
            {
                return (bool)this.GetValue(GridDataTableProperties.ShowAddNewRowProperty);
            }

            set
            {
                this.SetValue(GridDataTableProperties.ShowAddNewRowProperty, value);
            }
        }

        public bool ShowRecordPlusMinus
        {
            get
            {
                return (bool)this.GetValue(GridDataTableProperties.ShowRecordPlusMinusProperty);
            }

            set
            {
                this.SetValue(GridDataTableProperties.ShowRecordPlusMinusProperty, value);
            }
        }

        public static readonly DependencyProperty ShowGroupCaptionPlusMinusProperty = DependencyProperty.Register(
            "ShowGroupCaptionPlusMinus",
            typeof(bool),
            typeof(GridDataTableProperties),
            new PropertyMetadata(true));

        /// <summary>
        /// Gets or sets a value indicating whether [show error tooltips].
        /// </summary>
        /// <value><c>true</c> if [show error tooltips]; otherwise, <c>false</c>.</value>
        public bool ShowErrorTooltips
        {
            get
            {
                return (bool)this.GetValue(GridDataTableProperties.ShowErrorTooltipsProperty);
            }

            set
            {
                this.SetValue(GridDataTableProperties.ShowErrorTooltipsProperty, value);
            }
        }

        /// <summary>
        /// DependencyProperty for <see cref="GridDataControl.ShowErrorTooltips"/> property.
        /// </summary>
        public static readonly DependencyProperty ShowErrorTooltipsProperty = DependencyProperty.Register(
            "ShowErrorTooltips", 
            typeof(bool),
            typeof(GridDataTableProperties),
            new PropertyMetadata(true));        
        
        /// <summary>
        /// Gets or sets a value indicating whether ShowTooltips is true / false.
        /// </summary>
        /// <value><c>true</c> if [show tooltips]; otherwise, <c>false</c>.</value>
        public bool ShowTooltips
        {
            get
            {
                return (bool)this.GetValue(GridDataTableProperties.ShowTooltipsProperty);
            }

            set
            {
                this.SetValue(GridDataTableProperties.ShowTooltipsProperty, value);
            }
        }

        /// <summary>
        /// DependencyProperty for <see cref="GridDataControl.ShowTooltips"/> property.
        /// </summary>
        public static readonly DependencyProperty ShowTooltipsProperty = DependencyProperty.Register(
            "ShowTooltips", 
            typeof(bool),
            typeof(GridDataTableProperties),
            new PropertyMetadata(false));

        public bool ShowGroupCaptionPlusMinus
        {
            get
            {
                return (bool)this.GetValue(GridDataTableProperties.ShowGroupCaptionPlusMinusProperty);
            }

            set
            {
                this.SetValue(GridDataTableProperties.ShowGroupCaptionPlusMinusProperty, value);
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether [show row header].
        /// </summary>
        /// <value><c>true</c> if [show row header]; otherwise, <c>false</c>.</value>
        public bool ShowRowHeader
        {
            get
            {
                return (bool)this.GetValue(GridDataTableProperties.ShowRowHeaderProperty);
            }

            set
            {
                this.SetValue(GridDataTableProperties.ShowRowHeaderProperty, value);
            }
        }

        #region RowHeaderWidth (DependencyProperty)

        /// <summary>
        /// Gets / sets row header width
        /// </summary>
        public double RowHeaderWidth
        {
            get { return (double)GetValue(RowHeaderWidthProperty); }
            set { SetValue(RowHeaderWidthProperty, value); }
        }

        public static readonly DependencyProperty RowHeaderWidthProperty = DependencyProperty.Register("RowHeaderWidth", typeof(double), typeof(GridDataTableProperties), new PropertyMetadata(GridDataTableModel.ExpandCollapseCellWidth, OnRowHeaderWidthChanged));

        private static void OnRowHeaderWidthChanged(DependencyObject dpo, DependencyPropertyChangedEventArgs args)
        {
            var tableProperties = dpo as GridDataTableProperties;
            if (tableProperties.Model == null)
            {
                return;
            }

            if (tableProperties.ShowRowHeader)
            {
                tableProperties.model.ColumnWidths[0] = (double)args.NewValue;
            }
        }

        #endregion


        #region ShowRowHeaderArrow (DependencyProperty)

        /// <summary>
        /// Gets / Sets to show the row header arrow.
        /// </summary>
        public bool ShowRowHeaderArrow
        {
            get { return (bool)GetValue(ShowRowHeaderArrowProperty); }
            set { SetValue(ShowRowHeaderArrowProperty, value); }
        }

        public static readonly DependencyProperty ShowRowHeaderArrowProperty = DependencyProperty.Register("ShowRowHeaderArrow", typeof(bool), typeof(GridDataTableProperties), new PropertyMetadata(true, OnShowRowHeaderArrowChanged));

        private static void OnShowRowHeaderArrowChanged(DependencyObject d, DependencyPropertyChangedEventArgs args)
        {
            GridDataTableProperties tableProperties = d as GridDataTableProperties;
            if (tableProperties.Model != null)
            {
                tableProperties.ShowRowHeaderArrow = (bool)args.NewValue;
                tableProperties.Model.InvalidateDisplay();
            }
        }

        #endregion

#if !SILVERLIGHT
        #region HeaderContextMenuItems (Dependency Property)
        public static readonly DependencyProperty HeaderContextMenuItemsProperty = DependencyProperty.Register("HeaderContextMenuItems", typeof(IEnumerable), typeof(GridDataTableProperties));

        [XmlIgnore]
        public IEnumerable HeaderContextMenuItems
        {
            get { return (IEnumerable)GetValue(GridDataTableProperties.HeaderContextMenuItemsProperty); }
            set { SetValue(GridDataTableProperties.HeaderContextMenuItemsProperty, value); }
        }
        #endregion

        #region GroupHeaderContextMenuItems (Dependency Property)
        public static readonly DependencyProperty GroupHeaderContextMenuItemsProperty = DependencyProperty.Register("GroupHeaderContextMenuItems", typeof(IEnumerable), typeof(GridDataTableProperties));
        [XmlIgnore]
        public IEnumerable GroupHeaderContextMenuItems
        {
            get { return (IEnumerable)GetValue(GridDataTableProperties.GroupHeaderContextMenuItemsProperty); }
            set { SetValue(GridDataTableProperties.GroupHeaderContextMenuItemsProperty, value); }
        }

        #endregion RecordContextMenuItems(Dependency Property)
        public static readonly DependencyProperty RecordContextMenuItemsProperty = DependencyProperty.Register("RecordContextMenuItems", typeof(IEnumerable), typeof(GridDataTableProperties));
        [XmlIgnore]
        public IEnumerable RecordContextMenuItems
        {
            get { return (IEnumerable)GetValue(GridDataTableProperties.RecordContextMenuItemsProperty); }
            set { SetValue(GridDataTableProperties.RecordContextMenuItemsProperty, value); }
        }
        #region

        #endregion
        public static readonly DependencyProperty SortColumnsProperty = DependencyProperty.Register(
        "SortColumns",
        typeof(FreezableCollection<GridDataSortColumn>),
        typeof(GridDataTableProperties));

        /// <summary>
        /// Gets or sets the sorted columns.
        /// </summary>
        /// <value>The sorted columns.</value>
        [XmlArray("SortColumns")]
        [XmlArrayItem("GridDataSortColumn", typeof(GridDataSortColumn))]
        public FreezableCollection<GridDataSortColumn> SortColumns
        {
            get
            {
                return (FreezableCollection<GridDataSortColumn>)GetValue(GridDataTableProperties.SortColumnsProperty);
            }
            set
            {
                SetValue(GridDataTableProperties.SortColumnsProperty, value);
            }
        }

        public static readonly DependencyProperty GroupedColumnsProperty = DependencyProperty.Register("GroupedColumns", typeof(FreezableCollection<GridDataGroupColumn>), typeof(GridDataTableProperties));

        [XmlArray("GroupedColumns")]
        [XmlArrayItem("GridDataGroupColumn", typeof(GridDataGroupColumn))]
        public FreezableCollection<GridDataGroupColumn> GroupedColumns
        {
            get
            {
                return (FreezableCollection<GridDataGroupColumn>)GetValue(GridDataTableProperties.GroupedColumnsProperty);
            }
            set
            {
                SetValue(GridDataTableProperties.GroupedColumnsProperty, value);
            }
        }

        public static readonly DependencyProperty SummaryRowsProperty = DependencyProperty.Register("SummaryRows", typeof(FreezableCollection<GridDataSummaryRow>), typeof(GridDataTableProperties));

        [XmlArray("SummaryRows")]
        [XmlArrayItem("GridDataSummaryRow", typeof(GridDataSummaryRow))]
        public FreezableCollection<GridDataSummaryRow> SummaryRows
        {
            get
            {
                return (FreezableCollection<GridDataSummaryRow>)GetValue(GridDataTableProperties.SummaryRowsProperty);
            }
            set
            {
                SetValue(GridDataTableProperties.SummaryRowsProperty, value);
            }
        }

        public static readonly DependencyProperty TableSummaryRowsProperty = DependencyProperty.Register("TableSummaryRows", typeof(FreezableCollection<GridDataSummaryRow>), typeof(GridDataTableProperties));

        [XmlArray("TableSummaryRows")]
        [XmlArrayItem("GridDataTableSummaryRows", typeof(GridDataSummaryRow))]
        public FreezableCollection<GridDataSummaryRow> TableSummaryRows
        {
            get
            {
                return (FreezableCollection<GridDataSummaryRow>)GetValue(GridDataTableProperties.TableSummaryRowsProperty);
            }
            set
            {
                SetValue(GridDataTableProperties.TableSummaryRowsProperty, value);
            }
        }


        public static readonly DependencyProperty StackedHeaderRowsProperty = DependencyProperty.Register("StackedHeaderRows", typeof(FreezableCollection<GridDataStackedHeaderRow>), typeof(GridDataTableProperties), new FrameworkPropertyMetadata(OnStackedHeaderRowsChanged));

        private static void OnStackedHeaderRowsChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            GridDataTableProperties tp = d as GridDataTableProperties;
            tp.OnStackedHeaderColumnTextChanged();            
        }

        private void OnStackedHeaderColumnTextChanged()
        {
            if (this.Model != null)
            {
                if (this.StackedHeaderRows != null && this.StackedHeaderRows.Count > 0)
                    this.Model.InvalidateCell(GridRangeInfo.Rows(0, this.StackedHeaderRows.Count));
            }
        }     

        [XmlArray("StackedHeaderRows")]
        [XmlArrayItem("GridDataStackedHeaderRows", typeof(GridDataStackedHeaderRow))]
        public FreezableCollection<GridDataStackedHeaderRow> StackedHeaderRows
        {
            get
            {
                return (FreezableCollection<GridDataStackedHeaderRow>)GetValue(GridDataTableProperties.StackedHeaderRowsProperty);
            }
            set
            {
                SetValue(GridDataTableProperties.StackedHeaderRowsProperty, value);
            }
        }

        public static readonly DependencyProperty ConditionalFormatsProperty = DependencyProperty.Register("ConditionalFormats", typeof(FreezableCollection<GridDataConditionalFormat>), typeof(GridDataTableProperties));

        [XmlArray("ConditionalFormats")]
        [XmlArrayItem("GridDataConditionalFormat", typeof(GridDataConditionalFormat))]
        public FreezableCollection<GridDataConditionalFormat> ConditionalFormats
        {
            get
            {
                return (FreezableCollection<GridDataConditionalFormat>)GetValue(GridDataTableProperties.ConditionalFormatsProperty);
            }
            set
            {
                SetValue(GridDataTableProperties.ConditionalFormatsProperty, value);
            }
        }

#else

        /// <summary>
        /// Gets or sets the sorted columns.
        /// </summary>
        /// <value>The sorted columns.</value>
        [XmlArray("SortColumns")]
        [XmlArrayItem("GridDataSortColumn", typeof(GridDataSortColumn))]
        public ObservableCollection<GridDataSortColumn> SortColumns
        {
            get;
            set;
        }



        [XmlArray("GroupedColumns")]
        [XmlArrayItem("GridDataGroupColumn", typeof(GridDataGroupColumn))]
        public ObservableCollection<GridDataGroupColumn> GroupedColumns
        {
            get;
            set;
        }

        [XmlArray("SummaryRows")]
        [XmlArrayItem("GridDataSummaryRow", typeof(GridDataSummaryRow))]
        public ObservableCollection<GridDataSummaryRow> SummaryRows
        {
            get;
            set;
        }

        [XmlArray("TableSummaryRows")]
        [XmlArrayItem("GridDataTableSummaryRows", typeof(GridDataSummaryRow))]
        public ObservableCollection<GridDataSummaryRow> TableSummaryRows
        {
            get;
            set;
        }

        [XmlArray("StackedHeaderRows")]
        [XmlArrayItem("GridDataStackedHeaderRows", typeof(GridDataStackedHeaderRow))]
        public ObservableCollection<GridDataStackedHeaderRow> StackedHeaderRows
        {
            get;
            set;
        }

        [XmlArray("ConditionalFormats")]
        [XmlArrayItem("GridDataConditionalFormat", typeof(GridDataConditionalFormat))]
        public ObservableCollection<GridDataConditionalFormat> ConditionalFormats
        {
            get;
            set;
        }

#endif

        #region EnableAdvanceExcelLikeFiltering

        public static readonly DependencyProperty EnableLegacyFilteringProperty =
           DependencyProperty.Register("EnableLegacyFiltering", typeof(bool), typeof(GridDataTableProperties), new PropertyMetadata(false, OnEnableLegacyFilteringPropertyChanged));

        public bool EnableLegacyFiltering
        {
            get
            {
                return (bool)this.GetValue(GridDataTableProperties.EnableLegacyFilteringProperty);
            }
            set
            {
                this.SetValue(GridDataTableProperties.EnableLegacyFilteringProperty, value);
            }
        }

        private static void OnEnableLegacyFilteringPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs args)
        {
            var grid = d as GridDataTableProperties;
            if (grid.Model != null)
            {
                var headercontrol = grid.Model.Grid.FindElementsOfType<GridDataHeaderCellControl>();
                var headers = headercontrol.Where(o => o != null).ToList();
                foreach (var item in headers)
                {
                    item.ExcelLikeFilterAdvVisibility = (bool)args.NewValue;
                }
            }

        }

        #endregion    


        public static readonly DependencyProperty ShowGroupDropAreaProperty = DependencyProperty.Register(
            "ShowGroupDropArea",
            typeof(bool),
            typeof(GridDataTableProperties),
#if !SILVERLIGHT
 new FrameworkPropertyMetadata(false, OnShowGroupDropAreaChanged));
#else
 new PropertyMetadata(false, OnShowGroupDropAreaChanged));
#endif
        double previousGroupDropAreaHeight;
        private static void OnShowGroupDropAreaChanged(DependencyObject d, DependencyPropertyChangedEventArgs args)
        {
            GridDataTableProperties tableProperties = d as GridDataTableProperties;
            GridDataControl dataGrid = null;
            if (tableProperties.Model != null && tableProperties.Model.Grid != null)
                dataGrid = tableProperties.Model.Grid.FindParentElementOfType<GridDataControl>();
            else
                return;
            if (dataGrid != null)
            {
                if ((bool)args.NewValue)
                {
                    if (dataGrid.GroupDropAreaGrid == null)
                        dataGrid.SetGroupDropAreaGrid();
                    dataGrid.GroupDropAreaGrid.AttachParentGrid((GridDataTableModel)dataGrid.InternalGrid.Model, dataGrid.InternalGrid);
                    if (dataGrid.GroupDropAreaText != null && dataGrid.GroupDropAreaText != string.Empty)
                        dataGrid.GroupDropAreaGrid.Model.GroupDropAreaText = dataGrid.GroupDropAreaText;
                    if (!dataGrid.AllowDragColumns)
                        dataGrid.AllowDragColumns = true;
                    dataGrid.GroupDropAreaGrid.InvalidateCells();
                }
                else
                {
                    tableProperties.previousGroupDropAreaHeight = tableProperties.GroupDropAreaHeight;
                }
            }
        }

        public bool ShowGroupDropArea
        {
            get
            {
                return (bool)this.GetValue(GridDataTableProperties.ShowGroupDropAreaProperty);
            }

            set
            {
                this.SetValue(GridDataTableProperties.ShowGroupDropAreaProperty, value);
            }
        }

        public static readonly DependencyProperty GroupCaptionTextProperty = DependencyProperty.Register(
            "GroupCaptionText",
            typeof(string),
            typeof(GridDataTableProperties),
            new PropertyMetadata(OnGroupCaptionTextPropertyChanged));

        private static void OnGroupCaptionTextPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs args)
        {
            var tableProperties = d as GridDataTableProperties;
            if (tableProperties.Model == null)
            {
                return;
            }

            if (tableProperties.model.Table != null && tableProperties.Model.Table.GroupModel != null)
            {
                // TODO - CHECK HERE
                // tableProperties.Model.Table.GroupModel.GroupCaptionText = (string)args.NewValue;
                if (tableProperties.Model.IsLoaded)
                {
                    tableProperties.Model.InvalidateDisplay();
                }
            }
        }

#if SILVERLIGHT
        #region ShowGroupIndicators

        public static readonly DependencyProperty ShowGroupIndicatorsProperty = DependencyProperty.Register(
            "ShowGroupIndicators",
            typeof(bool),
            typeof(GridDataTableProperties),
            new PropertyMetadata(false, OnShowGroupIndicatorsChanged));

        private static void OnShowGroupIndicatorsChanged(DependencyObject dpo, DependencyPropertyChangedEventArgs args)
        {
            var tableProperties = dpo as GridDataTableProperties;
            if (tableProperties.Model != null && !DesignerProperties.GetIsInDesignMode(dpo))
            {
                foreach (var column in tableProperties.VisibleColumns)
                {
                    column.ShowGroupIndicator = (bool)args.NewValue;
                }
            }
        }

        public bool ShowGroupIndicators
        {
            get
            {
                return (bool)this.GetValue(GridDataTableProperties.ShowGroupIndicatorsProperty);
            }

            set
            {
                this.SetValue(GridDataTableProperties.ShowGroupIndicatorsProperty, value);
            }
        }

        #endregion
#endif

        /// <summary>
        /// Gets or sets the group caption text.
        /// </summary>
        /// <value>The group caption text.</value>
        public string GroupCaptionText
        {
            get
            {
                return (string)this.GetValue(GridDataTableProperties.GroupCaptionTextProperty);
            }

            set
            {
                this.SetValue(GridDataTableProperties.GroupCaptionTextProperty, value);
            }
        }

        public static readonly DependencyProperty CaptionSummaryRowProperty = DependencyProperty.Register(
            "CaptionSummaryRow",
            typeof(GridDataSummaryRow),
            typeof(GridDataTableProperties),
            new PropertyMetadata(OnCaptionSummaryRowChanged));

        private static void OnCaptionSummaryRowChanged(DependencyObject d, DependencyPropertyChangedEventArgs args)
        {
            var tableProperties = d as GridDataTableProperties;
            if (tableProperties.IsInSuspend || tableProperties.Model == null)
            {
                return;
            }

            if (tableProperties.CaptionSummaryRow != null && tableProperties.CaptionSummaryRow.SummaryColumns != null && tableProperties.CaptionSummaryRow.SummaryColumns.Count > 0)
            {
                if (tableProperties.summaryColumns != null)
                    tableProperties.summaryColumns.Clear();
                else
                    tableProperties.summaryColumns = new ObservableCollection<ISummaryColumn>();
                foreach (ISummaryColumn col in tableProperties.CaptionSummaryRow.SummaryColumns)
                {                    
                    tableProperties.summaryColumns.Add(col);
                }
            }

            if (tableProperties.Model != null && tableProperties.Model.IsLoaded)
            {
                var model = tableProperties.Model;
                if (model.Table != null && model.Table.HasGroups)
                {
                    model.Table.GroupModel.UpdateCaptionSummaries();
                }
                tableProperties.Model.InvalidateDisplay();
            }
        }
        private ObservableCollection<ISummaryColumn> summaryColumns;
        /// <summary>
        /// Inrternally maintains the SummaryColumns for CaptionSummaryRow.
        /// </summary>
        internal ObservableCollection<ISummaryColumn> SummaryColumns
        {
            get
            {
                return this.summaryColumns;
            }
        }

        public GridDataSummaryRow CaptionSummaryRow
        {
            get
            {
                return (GridDataSummaryRow)this.GetValue(GridDataTableProperties.CaptionSummaryRowProperty);
            }

            set
            {
                this.SetValue(GridDataTableProperties.CaptionSummaryRowProperty, value);
            }
        }

        public static readonly DependencyProperty ShowGroupSummariesProperty = DependencyProperty.Register(
            "ShowGroupSummaries",
            typeof(bool),
            typeof(GridDataTableProperties),
            new PropertyMetadata(true, OnShowGroupSummariesChanged));

        private static void OnShowGroupSummariesChanged(DependencyObject d, DependencyPropertyChangedEventArgs args)
        {
            var tableProperties = d as GridDataTableProperties;
            if (tableProperties.IsInSuspend || tableProperties.Model == null)
            {
                return;
            }

            if (tableProperties.Model.Table.HasGroups)
            {
                if (tableProperties.Model.IsLoaded)
                {
                    tableProperties.Model.InvalidateVisual(true);
                    tableProperties.Model.InvalidateDisplay();
                }
            }
        }

        public bool ShowGroupSummaries
        {
            get
            {
                return (bool)this.GetValue(GridDataTableProperties.ShowGroupSummariesProperty);
            }

            set
            {
                this.SetValue(GridDataTableProperties.ShowGroupSummariesProperty, value);
            }
        }

        public static readonly DependencyProperty ShowGroupSummaryInCaptionProperty = DependencyProperty.Register(
            "ShowGroupSummaryInCaption",
            typeof(bool),
            typeof(GridDataTableProperties),
            new PropertyMetadata(OnShowGroupSummaryInCaptionChanged));

        private static void OnShowGroupSummaryInCaptionChanged(DependencyObject d, DependencyPropertyChangedEventArgs args)
        {
            var tableProperties = d as GridDataTableProperties;
            if (tableProperties.IsInSuspend || tableProperties.Model == null)
            {
                return;
            }

            if (tableProperties.Model.Table.HasGroups)
            {
                if (tableProperties.Model.IsLoaded)
                {
                    tableProperties.Model.InvalidateDisplay();
                }
            }
        }

        public bool ShowGroupSummaryInCaption
        {
            get
            {
                return (bool)this.GetValue(GridDataTableProperties.ShowGroupSummaryInCaptionProperty);
            }

            set
            {
                this.SetValue(GridDataTableProperties.ShowGroupSummaryInCaptionProperty, value);
            }
        }

        public static readonly DependencyProperty ShowTableSummariesProperty = DependencyProperty.Register(
            "ShowTableSummaries",
            typeof(bool),
            typeof(GridDataTableProperties),
            new PropertyMetadata(true, OnShowTableSummariesChanged));

        private static void OnShowTableSummariesChanged(DependencyObject d, DependencyPropertyChangedEventArgs args)
        {
            var tableProperties = d as GridDataTableProperties;
            if (tableProperties.IsInSuspend || tableProperties.Model == null)
            {
                return;
            }

            tableProperties.Model.Table.Refresh();
            tableProperties.Model.InvalidateVisual(true);
        }

        public bool ShowTableSummaries
        {
            get
            {
                return (bool)this.GetValue(GridDataTableProperties.ShowTableSummariesProperty);
            }

            set
            {
                this.SetValue(GridDataTableProperties.ShowTableSummariesProperty, value);
            }
        }

        //#region ShowTooltips

        ///// <summary>
        ///// ShowTooltips Dependency Property
        ///// </summary>
        //public static readonly DependencyProperty ShowTooltipsProperty =
        //    DependencyProperty.Register("ShowTooltips", typeof(bool), typeof(GridDataTableProperties),
        //        new PropertyMetadata((bool)false,
        //            new PropertyChangedCallback(OnShowTooltipsChanged)));

        ///// <summary>
        ///// Gets or sets the ShowTooltips property. This dependency property 
        ///// indicates Tooltips set for GridDataControl RecordCells.
        ///// </summary>
        //public bool ShowTooltips
        //{
        //    get { return (bool)GetValue(ShowTooltipsProperty); }
        //    set { SetValue(ShowTooltipsProperty, value); }
        //}

        ///// <summary>
        ///// Handles changes to the ShowTooltips property.
        ///// </summary>
        //private static void OnShowTooltipsChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        //{
        //    GridDataTableProperties target = (GridDataTableProperties)d;
        //    GridTooltipService.SetShowTooltips(target.Model.Grid, (bool)e.NewValue);
        //}

        //#endregion




#if !SILVERLIGHT
        /// <summary>
        /// Gets or sets the columns.
        /// </summary>
        /// <value>The columns.</value>
        [XmlArray("VisibleColumns")]
        [XmlArrayItem("GridDataVisibleColumn", typeof(GridDataVisibleColumn))]


        public static readonly DependencyProperty GridDataVisibleColumnsProperty = DependencyProperty.Register(
         "VisibleColumns",
         typeof(GridDataVisibleColumns),
         typeof(GridDataTableProperties));


        public GridDataVisibleColumns VisibleColumns
        {
            get
            {
                return (GridDataVisibleColumns)this.GetValue(GridDataTableProperties.GridDataVisibleColumnsProperty);
            }

            set
            {
                this.SetValue(GridDataTableProperties.GridDataVisibleColumnsProperty, value);
            }
        }

#else
        /// <summary>
        /// Gets or sets the columns.
        /// </summary>
        /// <value>The columns.</value>
        [XmlArray("VisibleColumns")]
        [XmlArrayItem("GridDataVisibleColumn", typeof(GridDataVisibleColumn))]
        public GridDataVisibleColumns VisibleColumns
        {
            get;
            set;
        }
#endif

        public VisualStyle VisualStyle
        {
            get
            {
                return (VisualStyle)this.GetValue(GridDataTableProperties.VisualStyleProperty);
            }

            set
            {
                this.SetValue(GridDataTableProperties.VisualStyleProperty, value);
            }
        }

        [XmlIgnore]
        public IGridDataVisualStyle CustomVisualStyle
        {
            get
            {
                return (IGridDataVisualStyle)this.GetValue(GridDataTableProperties.CustomVisualStyleProperty);
            }

            set
            {
                this.SetValue(GridDataTableProperties.CustomVisualStyleProperty, value);
            }
        }

#if !SILVERLIGHT

        internal void UpdateVisualStyle()
        {

            if(this.VisualStyle != VisualStyle.Metro)
                return;

            var tableProperties = this;
            var ivisualstyle = tableProperties.GetVisualStyle(this.VisualStyle);

            var dataGrid = tableProperties.Model.Grid.FindParentElementOfType<GridDataControl>();

            if (tableProperties.Model is GridDataChildTableModel)
                dataGrid = (tableProperties.Model as GridDataChildTableModel).ParentRecord.Model.Grid.FindParentElementOfType<GridDataControl>();

            if (GridDataControl.GetOverrideVisualStyle(dataGrid))
            {
                if (dataGrid != null)
                {
                    if (tableProperties.Model.GridVisualStyle is GridDataAccentMetroVisualStyle)
                    {
                        var visualstyle = ivisualstyle as GridDataAccentMetroVisualStyle;
                        visualstyle.MetroBorderBrush = SkinStorage.GetMetroBorderBrush(dataGrid) ?? visualstyle.MetroBorderBrush;
                        visualstyle.MetroBrush = SkinStorage.GetMetroBrush(dataGrid) ?? visualstyle.MetroBrush;
                        visualstyle.MetroFocusedBorderBrush = SkinStorage.GetMetroFocusedBorderBrush(dataGrid) ?? visualstyle.MetroFocusedBorderBrush;
                        visualstyle.MetroFontFamily = SkinStorage.GetMetroFontFamily(dataGrid) ?? visualstyle.MetroFontFamily;
                        visualstyle.MetroForegroundBrush = SkinStorage.GetMetroForegroundBrush(dataGrid) ?? visualstyle.MetroForegroundBrush;
                        visualstyle.MetroHighlightedForegroundBrush = SkinStorage.GetMetroHighlightedForegroundBrush(dataGrid) ?? visualstyle.MetroHighlightedForegroundBrush;
                        visualstyle.MetroHoverBrush = SkinStorage.GetMetroHoverBrush(dataGrid) ?? visualstyle.MetroHoverBrush;
                        visualstyle.MetroPanelBackgroundBrush = SkinStorage.GetMetroPanelBackgroundBrush(dataGrid) ?? visualstyle.MetroPanelBackgroundBrush;
                        
                    }
                }

                tableProperties.Model.GridVisualStyle = ivisualstyle;

                if (tableProperties.Model.Table != null)
                {
                    tableProperties.Model.Table.RefreshChildVisualStyles(this.VisualStyle, true);
                }

                if (tableProperties.Model.Grid != null)
                {
                    if (dataGrid != null && dataGrid.ShowGroupDropArea && dataGrid.GroupDropAreaGrid != null &&
                        dataGrid.GroupDropAreaGrid.Model != null)
                    {
                        dataGrid.GroupDropAreaGrid.Model.RefreshVisualStyles();
                        dataGrid.GroupDropAreaGrid.InvalidateCells();
                    }
                }
            }
        }

#endif
        internal IGridDataVisualStyle GetVisualStyle(VisualStyle style)
        {
            switch (style)
            {
                case VisualStyle.Default:
                    if (model.TableProperties.IsLegacyStyleEnabled)
                    {
                        return new GridDataLegacyDefaultGridVisualStyle();
                    }
                    else
                    {
                        return new GridDataWindows7VisualStyle();
                    }
                case VisualStyle.Office2007Blue:
                    if (model.TableProperties.EnableLegacyStyle)
                    {
                        return new GridDataLegacyBlueVisualStyle();
                    }
                    else
                    {
                        return new GridDataOffice2007BlueVisualStyle();
                    }
                case VisualStyle.Office2007Silver:
                    if (model.TableProperties.EnableLegacyStyle)
                    {
                        return new GridDataLegacySilverVisualStyle();
                    }
                    else
                    {
                        return new GridDataOffice2007SilverVisualStyle();
                    }
                case VisualStyle.Office2007Black:
                    if (model.TableProperties.EnableLegacyStyle)
                    {
                        return new GridDataLegacyBlackVisualStyle();
                    }
                    else
                    {
                        return new GridDataOffice2007BlackVisualStyle();
                    }
                //case VisualStyle.Office2003:
                //    return new GridDataSyncBlueVisualStyle();
                case VisualStyle.Blend:
                    if (model.TableProperties.EnableLegacyStyle)
                    {
                        return new GridDataLegacyBlendVisualStyle();
                    }
                    else
                    {
                        return new GridDataBlendVisualStyle();
                    }
                case VisualStyle.GlassyGreen:
                    if (model.TableProperties.EnableLegacyStyle)
                    {
                        return new GridDataLegacyGlassyGreenVisualStyle();
                    }
                    else
                    {
                        return new GridDataGlassyGreenVisualStyle();
                    }
                case VisualStyle.SunBlack:
                    if (model.TableProperties.EnableLegacyStyle)
                    {
                        return new GridDataLegacySunBlackVisualStyle();
                    }
                    else
                    {
                        return new GridDataSunBlackVisualStyle();
                    }
                case VisualStyle.ShinyRed:
                    if (model.TableProperties.EnableLegacyStyle)
                    {
                        return new GridDataLegacyShinyRedVisualStyle();
                    }
                    else
                    {
                        return new GridDataShinyRedVisualStyle();
                    }
                case VisualStyle.ShinyBlue:
                    if (model.TableProperties.EnableLegacyStyle)
                    {
                        return new GridDataLegacyShinyBlueVisualStyle();
                    }
                    else
                    {
                        return new GridDataShinyBlueVisualStyle();
                    }
                case VisualStyle.BureauBlue:
                    if (model.TableProperties.EnableLegacyStyle)
                    {
                        return new GridDataLegacyBureauBlueVisualStyle();
                    }
                    else
                    {
                        return new GridDataBureauBlueVisualStyle();
                    }
                case VisualStyle.BureauBlack:
                    if (model.TableProperties.EnableLegacyStyle)
                    {
                        return new GridDataLegacyBureauBlackVisualStyle();
                    }
                    else
                    {
                        return new GridDataBureauBlackVisualStyle();
                    }

                case VisualStyle.TwilightBlue:
#if!SILVERLIGHT

                        if (model.TableProperties.EnableLegacyStyle)
                        {
                            return new GridDataLegacyTwilightBlueVisualStyle();
                        }
                        else
                        {
#endif

                    return new GridDataTwilightBlueVisualStyle();
#if!SILVERLIGHT
                        }
#endif
                case VisualStyle.Office2003:
                        return new GridDataSyncBlueVisualStyle();

                case VisualStyle.DefaultOffice2007Blue:
                        return new GridDataLegacyOffice2007BlueVisualStyle();
                case VisualStyle.DefaultOffice2007Black:
                        return new GridDataLegacyOffice2007BlackVisualStyle();
                case VisualStyle.DefaultOffice2007Silver:
                        return new GridDataLegacyOffice2007SilverVisualStyle();
                case VisualStyle.Office14Blue:
                    if (model.TableProperties.EnableLegacyStyle)
                    {
                        return new GridDataLegacyOffice14BlueVisualStyle();
                    }
                    else
                    {
                        return new GridDataOffice14BlueVisualStyle();
                    }
                case VisualStyle.Office14Black:
                    if (model.TableProperties.EnableLegacyStyle)
                    {
                        return new GridDataLegacyOffice14BlackVisualStyle();
                    }
                    else
                    {
                        return new GridDataOffice14BlackVisualStyle();
                    }
                case VisualStyle.Office14Silver:
                    if (model.TableProperties.EnableLegacyStyle)
                    {
                        return new GridDataLegacyOffice14SilverVisualStyle();
                    }
                    else
                    {
                        return new GridDataOffice14SilverVisualStyle();
                    }
                case VisualStyle.VS2010:
                    if (model.TableProperties.EnableLegacyStyle)
                    {
                        return new GridDataLegacyVS2010VisualStyle();
                    }
                    else
                    {
                        return new GridDataVS2010VisualStyle();
                    }
                case VisualStyle.Windows7:
                    if (model.TableProperties.EnableLegacyStyle)
                    {
                        return new GridDataLegacyDefaultGridVisualStyle();
                    }
                    else
                    {
                        return new GridDataWindows7VisualStyle();
                    }
                case VisualStyle.SyncfusionTheme:
                    return new GridDataSyncfusionVisualStyle();
                case VisualStyle.Metro:
#if!SILVERLIGHT
                   var dataGrid = this.Model.Grid.FindParentElementOfType<GridDataControl>();
                   if (GridDataControl.GetOverrideVisualStyle(dataGrid))
                        return new GridDataAccentMetroVisualStyle();
                    else
#endif
                        return new GridDataMetroVisualStyle(); 
                case VisualStyle.Custom:
                    return this.CustomVisualStyle;
            }

            return null;
        }

        private static void OnAddNewRowPositionChanged(DependencyObject d, DependencyPropertyChangedEventArgs args)
        {
            var tableProperties = d as GridDataTableProperties;
            if (tableProperties.IsInSuspend || tableProperties.Model != null)
            {
                tableProperties.Model.InvalidateDisplay();
            }
        }
        private static void OnUnboundRowPositionChanged(DependencyObject d, DependencyPropertyChangedEventArgs args)
            {
            var tableProperties = d as GridDataTableProperties;
            if (tableProperties.IsInSuspend || tableProperties.Model != null)
                {
                tableProperties.Model.InvalidateDisplay();
                }
            }

        private static void OnAllowGroupChanged(DependencyObject d, DependencyPropertyChangedEventArgs args)
        {
            var tableProperties = d as GridDataTableProperties;
            if (tableProperties.Model != null)
            {
                var value = (bool)args.NewValue;
                if (value)
                {
                    tableProperties.VisibleColumns.ForEach(v =>
                    {
                        v.AllowGroup = true;
                    });
                }
                else
                {
                    tableProperties.VisibleColumns.Where(v => (bool)v.AllowGroup).ForEach(v =>
                    {
                        v.AllowGroup = false;
                    });
                }

                tableProperties.Model.InvalidateCell(GridRangeInfo.Row(0));
            }
        }

        private static void OnAllowSortChanged(DependencyObject d, DependencyPropertyChangedEventArgs args)
        {
            var tableProperties = d as GridDataTableProperties;
            if (tableProperties.Model != null)
            {
                var value = (bool)args.NewValue;
                if (value)
                {
                    tableProperties.VisibleColumns.Where(v => !v.AllowSort).ForEach(v =>
                    {
                        v.AllowSort = true;
                    });
                }
                else
                {
                    tableProperties.VisibleColumns.Where(v => v.AllowSort).ForEach(v =>
                    {
                        v.AllowSort = false;
                    });
                }
            }
        }

        private static void OnSortingOptionsChanged(DependencyObject d, DependencyPropertyChangedEventArgs args)
        {
            var tableProperties = d as GridDataTableProperties;
            CollectionViewAdv view = null;
            if (tableProperties.Model != null && tableProperties.Model.View !=null)
            {
                view = tableProperties.Model.View as CollectionViewAdv;
                view.SortingOptions = (SortingOptions)args.NewValue;
            }
        }

        private static void OnAllowEditPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs args)
        {
            var tableProperties = d as GridDataTableProperties;        
            if (tableProperties.Model != null && tableProperties.Model.Grid != null)
            {
                if (!(bool)args.NewValue)
                {
                    if (tableProperties.Model.Grid.CurrentCell != null && tableProperties.Model.Grid.CurrentCell.IsEditing)
                    {
                        tableProperties.Model.Grid.CurrentCell.Deactivate();
                    }
                }
            }
        }


        private static void OnAllowDragColumnsChanged(DependencyObject d, DependencyPropertyChangedEventArgs args)
        {
            var tableProperties = d as GridDataTableProperties;
            if (tableProperties.Model != null && tableProperties.Model.Grid != null && !DesignerProperties.GetIsInDesignMode(d))
            {
                var value = (bool)args.NewValue;
                var grid = tableProperties.Model.Grid;
                var dragController = grid.MouseControllerDispatcher.Find(GridDataGroupDragMouseController.MouseControllerName);
                if (value)
                {
                    if (dragController == null)
                    {
                        grid.MouseControllerDispatcher.Add(new GridDataGroupDragMouseController(grid));
                    }

                    tableProperties.VisibleColumns.Where(v => !v.AllowDrag).ForEach(v =>
                    {
                        v.AllowDrag = true;
                    });
                }
                else
                {
                    if (dragController != null)
                    {
                        grid.MouseControllerDispatcher.Remove(dragController);
                    }

                    tableProperties.VisibleColumns.Where(v => v.AllowDrag).ForEach(v =>
                    {
                        v.AllowDrag = false;
                    });
                }
            }
        }

        public static readonly DependencyProperty AllowResizeRowsProperty = DependencyProperty.Register(
            "AllowResizeRows",
            typeof(bool),
            typeof(GridDataTableProperties),
            new PropertyMetadata(true, OnAllowResizeRowsChanged));

        private static void OnAllowResizeRowsChanged(DependencyObject d, DependencyPropertyChangedEventArgs args)
        {
            var tableProperties = d as GridDataTableProperties;
            if (tableProperties.Model != null && tableProperties.Model.Grid != null && !DesignerProperties.GetIsInDesignMode(d))
            {
                var value = (bool)args.NewValue;
                var grid = tableProperties.Model.Grid;
#if !SILVERLIGHT
                var resizeRowsController = grid.MouseControllerDispatcher.Find("ResizeRowsMouseController");
#else
                var resizeRowsController = grid.MouseControllerDispatcher.Find("GridResizeRowsMouseController");
#endif
                if (value)
                {
                    if (resizeRowsController == null)
                    {
                        grid.MouseControllerDispatcher.Add(new GridDataNestedResizeRowsMouseController(grid));
                    }
                }
                else
                {
                    if (resizeRowsController != null)
                    {
                        grid.MouseControllerDispatcher.Remove(resizeRowsController);
                    }
                }
            }
        }

        public bool AllowResizeRows
        {
            get
            {
                return (bool)this.GetValue(GridDataTableProperties.AllowResizeRowsProperty);
            }

            set
            {
                this.SetValue(GridDataTableProperties.AllowResizeRowsProperty, value);
            }
        }

        public static readonly DependencyProperty AllowResizeColumnsProperty = DependencyProperty.Register(
            "AllowResizeColumns",
            typeof(bool),
            typeof(GridDataTableProperties),
            new PropertyMetadata(true, OnAllowResizeColumnsChanged));

        private static void OnAllowResizeColumnsChanged(DependencyObject d, DependencyPropertyChangedEventArgs args)
        {
            var tableProperties = d as GridDataTableProperties;
            if (tableProperties.Model != null && tableProperties.Model.Grid != null && !DesignerProperties.GetIsInDesignMode(d))
            {
                var value = (bool)args.NewValue;
                var grid = tableProperties.Model.Grid;
#if !SILVERLIGHT
                var resizeColsController = grid.MouseControllerDispatcher.Find("ResizeColumnsMouseController");
#else
                var resizeColsController = grid.MouseControllerDispatcher.Find("GridResizeColumnsMouseController");
#endif
                if (value)
                {
                    if (resizeColsController == null)
                    {
                        grid.MouseControllerDispatcher.Add(new GridResizeColumnsMouseController(grid));
                    }

                    tableProperties.VisibleColumns.Where(v => !v.AllowResize).ForEach(v =>
                    {
                        v.AllowResize = true;
                    });
                }
                else
                {
                    if (resizeColsController != null)
                    {
                        grid.MouseControllerDispatcher.Remove(resizeColsController);
                    }

                    tableProperties.VisibleColumns.Where(v => v.AllowResize).ForEach(v =>
                    {
                        v.AllowResize = false;
                    });
                }
            }
        }

        public bool AllowResizeColumns
        {
            get
            {
                return (bool)this.GetValue(GridDataTableProperties.AllowResizeColumnsProperty);
            }

            set
            {
                this.SetValue(GridDataTableProperties.AllowResizeColumnsProperty, value);
            }
        }

        public AddNewRowBehaviour AddNewRowBehaviour
        {
            get { return (AddNewRowBehaviour)GetValue(GridDataTableProperties.AddNewRowBehaviourProperty); }
            set { SetValue(GridDataTableProperties.AddNewRowBehaviourProperty, value); }
        }

        public static readonly DependencyProperty AddNewRowBehaviourProperty =
            DependencyProperty.Register("AddNewRowBehaviour", typeof(AddNewRowBehaviour), typeof(GridDataTableProperties), new PropertyMetadata(AddNewRowBehaviour.Default, OnAddnewRowBehaviourChanged));

        private static void OnAddnewRowBehaviourChanged(DependencyObject obj, DependencyPropertyChangedEventArgs args)
        {
        }
        //private void UnwireGrid(GridControlBase grid)
        //{
        //    grid.QueryAllowDragColumn += new GridQueryDragColumnHeaderEventHandler(this.OnGridQueryAllowDragColumn);
        //}

        //private void WireGrid(GridControlBase grid)
        //{
        //    grid.QueryAllowDragColumn += new GridQueryDragColumnHeaderEventHandler(this.OnGridQueryAllowDragColumn);
        //}

        private void OnGridQueryAllowDragColumn(object sender, GridQueryDragColumnHeaderEventArgs e)
        {
            if (e.Reason != GridQueryDragColumnHeaderReason.MouseMove)
            {
                var colIdx = this.Model.ResolvePositionToVisibleColumnIndex(e.Column);
                if (colIdx < 0)
                {
                    e.AllowDrag = false;
                }
            }
            else if (e.Reason == GridQueryDragColumnHeaderReason.MouseMove)
            {
                var colIdx = this.Model.ResolvePositionToVisibleColumnIndex(e.InsertBeforeColumn);
                if (colIdx < 0)
                {
                    e.AllowDrag = false;
                }
            }
        }

        private static void OnAlternatingRowBackgroundChanged(DependencyObject d, DependencyPropertyChangedEventArgs args)
        {
            GridDataTableProperties tableProperties = d as GridDataTableProperties;
            if (!tableProperties.IsInternalChange)
                tableProperties.IsAlternatingRowBackgroundChangedExternally = true;

            if (tableProperties.Model != null)
            {
                if (!tableProperties.Model.inVisualstylechange)
                    tableProperties.Model.InvalidateDisplay();
            }
        }

        private static void OnAutoPopulateColumnsChanged(DependencyObject d, DependencyPropertyChangedEventArgs args)
        {
            GridDataTableProperties tableProperties = d as GridDataTableProperties;
            if (tableProperties.Model != null)
            {
                tableProperties.Model.SetDirty();
            }
        }


        private static void OnDefaultColumnWidthChanged(DependencyObject d, DependencyPropertyChangedEventArgs args)
        {
            GridDataTableProperties tableProperties = d as GridDataTableProperties;
            if (tableProperties != null)
            {
                tableProperties.VisibleColumns.ForEach<GridDataVisibleColumn>(v =>
                {
                    v.ActualWidth = (double)args.NewValue;
                });
            }
        }

        private void SetDragIndicatorOuterBrush()
        {
            var dragController = this.Model.Grid.MouseControllerDispatcher.Find(GridDataGroupDragMouseController.MouseControllerName) as GridDataGroupDragMouseController;
            if (dragController != null)
            {
                dragController.UpIndicator.OuterBrush = (Brush)this.DragIndicatorOuterBrush;
                dragController.DownIndicator.OuterBrush = (Brush)this.DragIndicatorOuterBrush;
            }
            var dataGrid = this.Model.Grid.FindParentElementOfType<GridDataControl>();
            if (dataGrid != null && dataGrid.GroupDropAreaGrid != null)
            {
                var dropController = dataGrid.GroupDropAreaGrid.MouseControllerDispatcher.Find("GridDataGroupDropMouseController") as GridDataGroupDropMouseController;
                if (dropController != null)
                {
                    dropController.UpIndicator.OuterBrush = (Brush)this.DragIndicatorOuterBrush;
                    dropController.DownIndicator.OuterBrush = (Brush)this.DragIndicatorOuterBrush;
                }
            }
        }

        internal void SetDragIndicatorInnerBrush()
        {
            var dragController = this.Model.Grid.MouseControllerDispatcher.Find(GridDataGroupDragMouseController.MouseControllerName) as GridDataGroupDragMouseController;
            if (dragController != null)
            {
                dragController.UpIndicator.InnerBrush = (Brush)this.DragIndicatorInnerBrush;
                dragController.DownIndicator.InnerBrush = (Brush)this.DragIndicatorInnerBrush;
            }
            var dataGrid = this.Model.Grid.FindParentElementOfType<GridDataControl>();
            if (dataGrid != null && dataGrid.GroupDropAreaGrid != null)
            {
                var dropController = dataGrid.GroupDropAreaGrid.MouseControllerDispatcher.Find("GridDataGroupDropMouseController") as GridDataGroupDropMouseController;
                if (dropController != null)
                {
                    dropController.UpIndicator.InnerBrush = (Brush)this.DragIndicatorInnerBrush;
                    dropController.DownIndicator.InnerBrush = (Brush)this.DragIndicatorInnerBrush;
                }
            }
        }

        private static void OnDragIndicatorInnerBrushChanged(DependencyObject d, DependencyPropertyChangedEventArgs args)
        {
            var tableProperties = d as GridDataTableProperties;
            if (tableProperties.AllowDragColumns && tableProperties.Model != null && tableProperties.Model.Grid != null)
            {
                tableProperties.SetDragIndicatorInnerBrush();
            }
        }

        private static void OnDragIndicatorOuterBrushChanged(DependencyObject d, DependencyPropertyChangedEventArgs args)
        {
            var tableProperties = d as GridDataTableProperties;
            if (tableProperties.AllowDragColumns && tableProperties.Model != null && tableProperties.Model.Grid != null)
            {
                tableProperties.SetDragIndicatorOuterBrush();
            }
        }

        private static void OnItemsSourcePropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs args)
        {
            GridDataTableProperties tableProperties = d as GridDataTableProperties;
            if (tableProperties.Model != null)
            {
                var isAlreadyInitialized = tableProperties.Model.SourceList != null;
                if (isAlreadyInitialized)
                {
                    tableProperties.Model.Table.Dispose();
                    if (tableProperties.ClearAllOnItemSourceChange)
                    {
                        tableProperties.Model.SuspendEvents();
                        tableProperties.SummaryRows.Clear();
                        tableProperties.StackedHeaderRows.Clear();
                        tableProperties.SortColumns.Clear();
                        tableProperties.TableSummaryRows.Clear();
                        tableProperties.GroupedColumns.Clear();
                        tableProperties.ConditionalFormats.Clear();
                        tableProperties.UnwireEvents();
                        tableProperties.VisibleColumns.Clear();
                        if (tableProperties.summaryColumns != null)
                        {
                            tableProperties.summaryColumns.Clear();
                            tableProperties.summaryColumns = null;
                        }
                        tableProperties.CaptionSummaryRow = null;
                        tableProperties.Model.ResumeEvents();
                    }
                    tableProperties.Model.SelectedRanges.Clear();
                    tableProperties.Model.CurrencyManager.Reset();

#if SyncfusionFramework4_0
                    tableProperties.Model.Table.isDynamicSourceEvaluated = false;
#endif
                }
                if (tableProperties.VisibleColumns != null && tableProperties.VisibleColumns.Count > 0)
                {
                    tableProperties.VisibleColumns.ForEach(x =>
                        {
                            if (x.Filters.Count > 0)
                                x.Filters.Clear();
                        });
                }
                tableProperties.Model.SetSourceList(args.NewValue, true, true, null);
                tableProperties.Model.Selections.Clear();
                var grid = tableProperties.Model.Grid;
                if (grid != null)
                {
                    if (grid.CurrentCell != null)
                    {
                        grid.CurrentCell.Deactivate();
                        grid.CurrentCell.ResetCache();
                        tableProperties.Model.CurrencyManager.ResetCache();
                    }
                    grid.InvalidateCells();
                }
            }
        }

        private static void OnRowBackgroundChanged(DependencyObject d, DependencyPropertyChangedEventArgs args)
        {
            GridDataTableProperties tableProperties = d as GridDataTableProperties;
            if (!tableProperties.IsInternalChange)
                tableProperties.IsRowBackgroundChangedExternally = true;
            if (tableProperties.Model != null)
            {
                if (!tableProperties.Model.inVisualstylechange)
                    tableProperties.Model.InvalidateDisplay();
            }
        }

        // Using a DependencyProperty as the backing store for FilterBarMode.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty FilterBarModeProperty =
            DependencyProperty.Register("FilterBarMode", typeof(GridDataFilterBarMode), typeof(GridDataTableProperties), new PropertyMetadata(GridDataFilterBarMode.Immediate));

        // Using a DependencyProperty as the backing store for AlphaNumericFilterType.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty AlphaNumericFilterTypeProperty =
           DependencyProperty.Register("AlphaNumericFilterType", typeof(AlphaNumericFilterType), typeof(GridDataTableProperties), new PropertyMetadata(AlphaNumericFilterType.WithWildcard));

#if !SILVERLIGHT
        // Using a DependencyProperty as the backing store for FilterPanePosition.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty FilterPanePositionProperty =
            DependencyProperty.Register("FilterPanePosition", typeof(FilterPanePosition), typeof(GridDataTableProperties), new PropertyMetadata(null));
#endif
        // Using a DependencyProperty as the backing store for ShowFilterBar.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty ShowFilterBarProperty =
            DependencyProperty.Register("ShowFilterBar", typeof(bool), typeof(GridDataTableProperties), new PropertyMetadata(false, OnShowFilterBarChanged));

        /// <summary>
        /// Dependency property for ShowAddNewRow
        /// </summary>
        /// <value><c>true</c> if [show add new row]; otherwise, <c>false</c>.</value>
        public static readonly DependencyProperty ShowAddNewRowProperty = DependencyProperty.Register(
            "ShowAddNewRow",
            typeof(bool),
            typeof(GridDataTableProperties),
#if !SILVERLIGHT
 new FrameworkPropertyMetadata(true, OnShowAddNewRowChanged, OnShowAddNewRowCoerceValue));
#else
 new PropertyMetadata(true, OnShowAddNewRowChanged));
#endif

#if !SILVERLIGHT
        private static object OnShowAddNewRowCoerceValue(DependencyObject d, object value)
        {
            var tableProperties = d as GridDataTableProperties;
            if (tableProperties.Model != null && tableProperties.Model.SourceListCount > 0)
            {
                if (tableProperties.Model.View != null && !tableProperties.Model.View.CanAddNew && (bool)value)
                {
                    return false;
                }
            }

            return value;
        }
#endif

        private static void OnShowAddNewRowChanged(DependencyObject d, DependencyPropertyChangedEventArgs args)
        {
            GridDataTableProperties tableProperties = d as GridDataTableProperties;
            if (tableProperties.Model != null && tableProperties.Model.SourceListCount > 0)
            {
                //If values entered in the addnew row then while changing the visibilty it should be clear
                if (tableProperties.Model.Grid != null)
                    tableProperties.Model.CurrencyManager.CurrentCell.CancelEdit();
                ///The below code will Reset the cache when you call the CancelEdit. Otherwise cache will not be cleared.
                tableProperties.Model.CurrencyManager.ResetCache();
                if (tableProperties.Model.Table.HasNestedTables || tableProperties.model.Table.HasDetailsView)
                {
                    var lineSizeCollection = tableProperties.Model.RowHeights as LineSizeCollection;
                    lineSizeCollection.SuspendUpdates();
                    tableProperties.Model.Table.HideAllUIRows();

                    tableProperties.Model.UpdateSelectedRanges();
                    lineSizeCollection.ResetNestedLines();

                    lineSizeCollection.ResumeUpdates();
                }

                tableProperties.Model.Table.Refresh();
                tableProperties.Model.InvalidateDisplay();
            }
        }

        private static void OnShowFilterBarChanged(DependencyObject d, DependencyPropertyChangedEventArgs args)
        {
            GridDataTableProperties tableProperties = d as GridDataTableProperties;

            if (tableProperties.Model != null && tableProperties.Model.SourceListCount > 0)
            {
                if (tableProperties.Model.Table.HasNestedTables || tableProperties.model.Table.HasDetailsView)
                {
                    var lineSizeCollection = tableProperties.Model.RowHeights as LineSizeCollection;
                    lineSizeCollection.SuspendUpdates();
                    tableProperties.Model.Table.HideAllUIRows();
                    lineSizeCollection.ResumeUpdates();
                }

                // When the filter bar is enabled dynamically IsExcelLikeFilter should be reset: Fix for SD16816
                if (args.NewValue is bool && (bool)args.NewValue)
                    (tableProperties.model.View as IExcelLikeFilterExt).IsExcelLikeFilter = false;

                tableProperties.Model.Table.Refresh();
                tableProperties.Model.InvalidateDisplay();
            }
        }

        private static void OnShowFiltersChanged(DependencyObject d, DependencyPropertyChangedEventArgs args)
        {
            var tableProperties = d as GridDataTableProperties;
            if (tableProperties.Model != null)
            {
                var value = (bool)args.NewValue;
                if (value)
                {
                    tableProperties.VisibleColumns.Where(v => !v.AllowFilter).ForEach(v =>
                    {
                        v.AllowFilter = true;
                    });
                }
                else
                {
                    tableProperties.VisibleColumns.Where(v => v.AllowFilter).ForEach(v =>
                    {
                        v.AllowFilter = false;
                    });
                }

                
            }
            if (tableProperties != null && tableProperties.Model != null && tableProperties.Model.Grid != null) // && (bool)args.NewValue != null) bool is never equal to null
            {
                var headercontrol = tableProperties.Model.Grid.FindElementsOfType<GridDataHeaderCellControl>();
                var headers = headercontrol.Where(o => o != null).ToList();
                if ((bool)args.NewValue == false)
                {
                    foreach (var item in headers)
                    {
                        //The below lines of code will not hit at any situations. So, I am commenting these lines.
                        //The line of block will get hooked only when the ShowFilters is set to false. In False condition we have to set the Visibility to Collapsed only.
//                        if (item != null && item.VisibleColumn != null && item.VisibleColumn.AllowFilter == true && (item.FilterButtonVisibility == Visibility.Collapsed
//#if !SILVERLIGHT
// || item.FilterButtonVisibility == Visibility.Hidden
//#endif
//))
//                        {
//                            item.FilterButtonVisibility = Visibility.Visible;
//                        }
//                        else 
                        if (item != null && item.VisibleColumn != null && item.VisibleColumn.AllowFilter == false && item.FilterButtonVisibility == Visibility.Visible)
                        {
                            item.FilterButtonVisibility = Visibility.Collapsed;
                        }
                    }
                }
                else
                {
                    foreach (var item in headers)
                    {
                        if (item != null && item.FilterButtonVisibility == Visibility.Collapsed
                            #if !SILVERLIGHT
 || item.FilterButtonVisibility == Visibility.Hidden
#endif
)
                        {
                            item.FilterButtonVisibility = Visibility.Visible;
                        }
                    }
                }
                tableProperties.Model.Grid.InvalidateCell(GridRangeInfo.Row(0));
                tableProperties.Model.Grid.InvalidateVisual(true);
            }
            
        }


        internal void SetColumnSizer(GridDataTableProperties tableProperties, GridControlLengthUnitType value)
        {
            if (tableProperties.AutoPopulateRelations)
            {
                tableProperties.Relations.ForEach(rd =>
                {
                    rd.TableProperties.ColumnSizer = tableProperties.ColumnSizer;
                });
            }

            tableProperties.VisibleColumns.SuspendEvents();

            switch (value)
            {
                case GridControlLengthUnitType.Auto:
                case GridControlLengthUnitType.SizeToCells:
                case GridControlLengthUnitType.SizeToHeader:
                case GridControlLengthUnitType.Star:
                    tableProperties.VisibleColumns.ForEach(v =>
                    {
                        v.Width = new GridDataControlLength(1d, value);
                    });
                    break;
                case GridControlLengthUnitType.AutoOnLoad:
                    tableProperties.VisibleColumns.ForEach(v =>
                    {
                        v.Width = new GridDataControlLength(1d, GridControlLengthUnitType.Auto);
                    });
                    break;
                case GridControlLengthUnitType.AutoWithLastColumnFill:
                case GridControlLengthUnitType.AutoOnLoadWithLastColumnFill:
                    var count = tableProperties.VisibleColumns.Count;
                    tableProperties.VisibleColumns.Take(count - 1).ForEach(v =>
                    {
                        v.Width = new GridDataControlLength(1d, GridControlLengthUnitType.Auto);
                    });
                    //SD15825 - when last coulumn is Hidden need to set star for before column.
                    for (int colIdx = count - 1; colIdx >= 0; colIdx--)
                    {
                        if (!tableProperties.VisibleColumns[colIdx].IsHidden)
                        {
                            tableProperties.VisibleColumns[colIdx].Width = new GridDataControlLength(1d, GridControlLengthUnitType.Star);
                            break;
                        }
                    }
                    break;

                case GridControlLengthUnitType.None:
                    tableProperties.VisibleColumns.ForEach(v =>
                    {
                        v.Width = new GridDataControlLength(150d, value);
                    });
                    break;
            }
            tableProperties.VisibleColumns.ResumeEvents();
            tableProperties.Model.ColumnAutoSizer.RefreshAll();

            if ((tableProperties.ColumnSizer == GridControlLengthUnitType.AutoOnLoad ||
                tableProperties.ColumnSizer == GridControlLengthUnitType.AutoOnLoadWithLastColumnFill) &&
                tableProperties.Model.ColumnAutoSizer.IsGridDataControlLoaded)
            {
                tableProperties.Model.ColumnAutoSizer.IsAutoOnLoad = true;
            }
        }

        private static void OnColumnSizerChanged(DependencyObject d, DependencyPropertyChangedEventArgs args)
        {
            var tableProperties = d as GridDataTableProperties;
            if (tableProperties.Model != null && tableProperties.VisibleColumns.Count > 0)
            {
                tableProperties.Model.ColumnAutoSizer.IsAutoOnLoad = false;
                tableProperties.SetColumnSizer(tableProperties, (GridControlLengthUnitType)args.NewValue);
            }
        }

        private static void OnShowRecordPlusMinusChanged(DependencyObject d, DependencyPropertyChangedEventArgs args)
        {
            GridDataTableProperties tableProperties = d as GridDataTableProperties;
            if (tableProperties.Model != null && tableProperties.VisibleColumns.Count > 0)
            {
                tableProperties.Model.Table.HideAllUIRows();
                tableProperties.Model.Table.Refresh();
                //the below code is added for selection process when the ShowRecordPlusMinus property changed at runtime.
                var count = tableProperties.Model.ResolveDefaultColumnOffset();
                count -= tableProperties.Model.Table.HasDetailsView ? 1 : 0;
                if ((bool)args.NewValue)
                    tableProperties.Model.InsertColumns(count-1, 1);
                else
                    tableProperties.Model.RemoveColumns(count, 1);
                tableProperties.Model.RefreshColumns(true, false);
                tableProperties.Model.InvalidateDisplay();
            }
        }

        private static void OnShowRowHeaderChanged(DependencyObject d, DependencyPropertyChangedEventArgs args)
        {
            GridDataTableProperties tableProperties = d as GridDataTableProperties;
            if (tableProperties.Model != null && tableProperties.VisibleColumns.Count > 0)
            {
                var value = (bool)args.NewValue;
                tableProperties.Model.Table.SetFrozenColumns();
                //the below code is added for selection process when the ShowRowHeader property changed at runtime.
#if !SILVERLIGHT
                if (value)
                    tableProperties.Model.InsertColumns(0, tableProperties.Model.HeaderRows);
                else
                    tableProperties.Model.RemoveColumns(0, tableProperties.Model.HeaderRows);
#endif
                tableProperties.Model.RefreshColumns(true, false);
                tableProperties.Model.InvalidateDisplay();
            }
        }

        private static void OnVisualStyleChanged(DependencyObject d, DependencyPropertyChangedEventArgs args)
        {
            var tableProperties = d as GridDataTableProperties;

            if (tableProperties.Model != null)
            {
                if (tableProperties.Model.IsEditing && tableProperties.Model.CurrencyManager != null)
                {
                    tableProperties.Model.CurrencyManager.CancelEdit();
                }

                // var style = (VisualStyle)args.NewValue; Unused local variable

                if (tableProperties.AutoPopulateRelations)
                {
                    tableProperties.Relations.ForEach(rd =>
                    {
                        rd.TableProperties.CustomVisualStyle = tableProperties.CustomVisualStyle;
                        rd.TableProperties.VisualStyle = tableProperties.VisualStyle;
                        rd.TableProperties.EnableLegacyStyle = tableProperties.EnableLegacyStyle;
                    });
                }

#if SyncfusionFramework3_5&&!SILVERLIGHT
                if (!GridDataTableModelHelper.IsInDesignMode)
#endif
                    if (tableProperties.Model is GridDataChildTableModel)
                    {
                        var nestedModel = (tableProperties.Model as GridDataChildTableModel);
                        if(nestedModel.ParentRecord != null && nestedModel.ParentRecord.Model != null && nestedModel.ParentRecord.Model.TableProperties.VisualStyle == (VisualStyle)args.NewValue)
                        {
                              tableProperties.Model.GridVisualStyle = (tableProperties.Model as GridDataChildTableModel).ParentRecord.Model.GridVisualStyle;
                        }
                        else
                        {
                             tableProperties.Model.GridVisualStyle =
                            tableProperties.GetVisualStyle((VisualStyle)args.NewValue);
                        }
                    }
                    else if (tableProperties.Model is GridDataTableModel)
                    {
                        tableProperties.Model.GridVisualStyle =
                            tableProperties.GetVisualStyle((VisualStyle)args.NewValue);
                    }
                    
                if (tableProperties.Model.Table != null)
                {
                    tableProperties.Model.Table.RefreshChildVisualStyles((VisualStyle)args.NewValue);
                }

                if (tableProperties.Model.Grid != null)
                {
                    var dataGrid = tableProperties.Model.Grid.FindParentElementOfType<GridDataControl>();
                    if (dataGrid != null && dataGrid.ShowGroupDropArea && dataGrid.GroupDropAreaGrid != null && dataGrid.GroupDropAreaGrid.Model != null)
                    {
                        dataGrid.GroupDropAreaGrid.Model.RefreshVisualStyles();
                        dataGrid.GroupDropAreaGrid.InvalidateCells();
                    }
                }

                //if (tableProperties.Model.ColumnAutoSizer != null)
                //{
                //    tableProperties.Model.ColumnAutoSizer.RefreshAll();
                //}
            }
        }

        public static readonly DependencyProperty NotifyPropertyChangesProperty = DependencyProperty.Register(
            "NotifyPropertyChanges",
            typeof(bool),
            typeof(GridDataTableProperties),
            new PropertyMetadata(false, OnNotifyPropertyChanged));

        private static void OnNotifyPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs args)
        {
            var tableProperties = d as GridDataTableProperties;
            if (tableProperties.Model == null || tableProperties.IsInSuspend)
            {
                return;
            }
            var value = (bool)args.NewValue;
            tableProperties.Model.SetNotifyPropertyChanged(value);
        }

        public bool NotifyPropertyChanges
        {
            get
            {
                return (bool)this.GetValue(GridDataTableProperties.NotifyPropertyChangesProperty);
            }

            set
            {
                this.SetValue(GridDataTableProperties.NotifyPropertyChangesProperty, value);
            }
        }

        public bool NotifyComplexPropertyChanges
        {
            get { return (bool)GetValue(NotifyComplexPropertyChangesProperty); }
            set { SetValue(NotifyComplexPropertyChangesProperty, value); }
        }

        public static readonly DependencyProperty NotifyComplexPropertyChangesProperty =
            DependencyProperty.Register("NotifyComplexPropertyChanges", typeof(bool), typeof(GridDataTableProperties), new PropertyMetadata(true, OnNotifyComplexPropertyChanged));

        private static void OnNotifyComplexPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs args)
        {
            //var grid = d as GridDataControl;
            //if (grid.isGridLoaded)
            //{
            //    //grid.TableProperties.NotifyComplexPropertyChanges = (bool)args.NewValue;
            //}
            //else
            //{
            //    grid.isNotifyComplexPropertyChangedSetBeforeGridLoaded = true;
            //}
        }

        #region IsGroupsExpanded (DependencyProperty)

        /// <summary>
        /// Gets / Sets the IsGroupsExpanded property.
        /// </summary>
        public bool IsGroupsExpanded
        {
            get { return (bool)GetValue(IsGroupsExpandedProperty); }
            set { SetValue(IsGroupsExpandedProperty, value); }
        }

        public static readonly DependencyProperty IsGroupsExpandedProperty = DependencyProperty.Register("IsGroupsExpanded", typeof(bool), typeof(GridDataTableProperties), new PropertyMetadata(false, OnIsGroupsExpandedChanged));

        private static void OnIsGroupsExpandedChanged(DependencyObject dpo, DependencyPropertyChangedEventArgs args)
        {
            var tableProperties = dpo as GridDataTableProperties;
            if (tableProperties.Model == null)
            {
                return;
            }
            else if (tableProperties.model.View == null)
            {
                return;
            }

            var value = (bool)args.NewValue;
            if (value)
            {
                tableProperties.Model.View.IsGroupsExpanded = value;
                tableProperties.Model.Table.ExpandAllGroups();
            }
            else
            {
                tableProperties.Model.Table.CollapseAllGroups();
            }
        }

        #endregion

        #region UpdateMode

        public UpdateMode UpdateMode
        {
            get { return (UpdateMode)GetValue(UpdateModeProperty); }
            set { SetValue(UpdateModeProperty, value); }
        }

        public static readonly DependencyProperty UpdateModeProperty = DependencyProperty.Register("UpdateMode", typeof(UpdateMode), typeof(GridDataTableProperties), new PropertyMetadata(UpdateMode.LostFocus, OnUpdateModeChanged));

        private static void OnUpdateModeChanged(DependencyObject dpo, DependencyPropertyChangedEventArgs args)
        {
            var tableProperties = dpo as GridDataTableProperties;
            if (tableProperties.Model != null)
            {
                foreach (var visibleColumn in tableProperties.VisibleColumns)
                {
                    visibleColumn.UpdateMode = (UpdateMode)args.NewValue;
                }
            }
        }

        #endregion

        #region TableSummaryPosition

        private static void OnTableSummaryPositionChanged(DependencyObject d, DependencyPropertyChangedEventArgs args)
        {
            GridDataTableProperties tableProperties = d as GridDataTableProperties;
            if (tableProperties.Model != null && tableProperties.Model.SourceListCount > 0)
            {
                tableProperties.Model.Table.Refresh();
                tableProperties.Model.InvalidateDisplay();
            }
        }

        /// <summary>
        /// Gets or sets the TableSummaryPosition.
        /// </summary>
        /// <value>TableSummaryPosition.</value>
        public Position TableSummaryPosition
        {
            get
            {
                return (Position)this.GetValue(GridDataTableProperties.TableSummaryPositionProperty);
            }

            set
            {
                this.SetValue(GridDataTableProperties.TableSummaryPositionProperty, value);
            }
        }

        #endregion

        #region HideColumnsWhenGrouped (DependencyProperty)

        /// <summary>
        /// Gets / Sets if columns have to be hidden when grouped
        /// </summary>
        public bool HideColumnsWhenGrouped
        {
            get { return (bool)GetValue(HideColumnsWhenGroupedProperty); }
            set { SetValue(HideColumnsWhenGroupedProperty, value); }
        }

        public static readonly DependencyProperty HideColumnsWhenGroupedProperty = DependencyProperty.Register("HideColumnsWhenGrouped", typeof(bool), typeof(GridDataTableProperties), new PropertyMetadata(false));

        #endregion

#if !SILVERLIGHT
        #region EnableOptimizations (DependencyProperty)

        /// <summary>
        /// Gets / sets to enable optimizations on the grid. When set to true, it will exclude Covered ranges and other factors that affect performance. This property has to be used when there are fast updates, and no covered cells being used.
        /// </summary>
        public bool EnableOptimizations
        {
            get { return (bool)GetValue(EnableOptimizationsProperty); }
            set { SetValue(EnableOptimizationsProperty, value); }
        }

        public static readonly DependencyProperty EnableOptimizationsProperty = DependencyProperty.Register("EnableOptimizations", typeof(bool), typeof(GridDataTableProperties), new PropertyMetadata(false, OnEnableOptimizationsChanged));

        private static void OnEnableOptimizationsChanged(DependencyObject dpo, DependencyPropertyChangedEventArgs args)
        {
            var tableProperties = dpo as GridDataTableProperties;
            if (tableProperties.Model != null)
            {
                tableProperties.Model.SupportsQueryCoveredCellCallback = false;
            }
        }

        #endregion
#endif

#if !SILVERLIGHT && SyncfusionFramework4_0
        #region UsePLINQ (DependencyProperty)

        /// <summary>
        /// Gets / sets to use PLINQ
        /// </summary>
        public bool UsePLINQ
        {
            get { return (bool)GetValue(UsePLINQProperty); }
            set { SetValue(UsePLINQProperty, value); }
        }

        public static readonly DependencyProperty UsePLINQProperty = DependencyProperty.Register("UsePLINQ", typeof(bool), typeof(GridDataTableProperties), new PropertyMetadata(false, OnUsePlinqChanged));

        private static void OnUsePlinqChanged(DependencyObject dpo, DependencyPropertyChangedEventArgs args)
        {
            var tableProperties = dpo as GridDataTableProperties;
            if (tableProperties.Model == null || tableProperties.Model != null && tableProperties.Model.View == null)
            {
                return;
            }

            var queryableCollectionView = tableProperties.Model.View as QueryableCollectionView;
            if (queryableCollectionView != null)
            {
                queryableCollectionView.UsePLINQ = (bool)args.NewValue;
            }
        }

        #endregion
#endif

        #region ClearMultiSelectionInNestedGrid
        /// <summary>
        /// Gets / sets the persistance for group expand states when refreshed.
        /// </summary>
        [Obsolete]
        public bool ClearMultiSelectionInNestedGrid
        {
            get { return (bool)GetValue(ClearMultiSelectionInNestedGridProperty); }
            set { SetValue(ClearMultiSelectionInNestedGridProperty, value); }
        }

        public static readonly DependencyProperty ClearMultiSelectionInNestedGridProperty = DependencyProperty.Register("ClearMultiSelectionInNestedGrid", typeof(bool), typeof(GridDataTableProperties), new PropertyMetadata(false));

        #endregion

        #region PersistGroupExpandStates
        /// <summary>
        /// Gets / sets the persistance for group expand states when refreshed.
        /// </summary>
        public bool PersistGroupsExpandState
        {
            get { return (bool)GetValue(PersistGroupsExpandStateProperty); }
            set { SetValue(PersistGroupsExpandStateProperty, value); }
        }

        public static readonly DependencyProperty PersistGroupsExpandStateProperty = DependencyProperty.Register("PersistGroupExpandStates", typeof(bool), typeof(GridDataTableProperties), new PropertyMetadata(false));

        #endregion

        #region RetainSortWhenUnGrouped (Dependency Property)

        public bool RetainSortWhenUnGrouped
        {
            get { return (bool)GetValue(RetainSortWhenUnGroupedProperty); }
            set { SetValue(RetainSortWhenUnGroupedProperty, value); }
        }

        public static readonly DependencyProperty RetainSortWhenUnGroupedProperty = DependencyProperty.Register("RetainSortWhenUnGrouped", typeof(bool), typeof(GridDataTableProperties), new PropertyMetadata(false));

        #endregion

#if !SILVERLIGHT

        #region Hovering Background (Dependency Property)

        /// <summary>
        /// Gets or sets a value indicating whether [show hovering background].
        /// </summary>
        /// <value>
        /// 	<c>true</c> if [show hovering background]; otherwise, <c>false</c>.
        /// </value>
        public bool ShowHoveringBackground
        {
            get { return (bool)GetValue(ShowHoveringBackgroundProperty); }
            set { SetValue(ShowHoveringBackgroundProperty, value); }
        }

        public static readonly DependencyProperty ShowHoveringBackgroundProperty = DependencyProperty.Register("ShowHoveringBackground", typeof(bool), typeof(GridDataTableProperties), new PropertyMetadata(false));

        #endregion

        #region ContextMenuOptions (Dependency Property)
        public ContextMenuOptions ContextMenuOptions
        {
            get { return (ContextMenuOptions)GetValue(ContextMenuOptionsProperty); }
            set { SetValue(ContextMenuOptionsProperty, value); }
        }

        public static readonly DependencyProperty ContextMenuOptionsProperty =
            DependencyProperty.Register("ContextMenuOptions", typeof(ContextMenuOptions), typeof(GridDataTableProperties), new PropertyMetadata(ContextMenuOptions.Default));

        #endregion
#endif

        #region ShowSortNumber (Dependency Property)


        /// <summary>
        /// Gets or sets a value indicating whether [show sort number].
        /// </summary>
        /// <value><c>true</c> if [show sort number]; otherwise, <c>false</c>.</value>
        public bool ShowSortNumber
        {
            get { return (bool)GetValue(ShowSortNumberProperty); }
            set { SetValue(ShowSortNumberProperty, value); }
        }

        public static readonly DependencyProperty ShowSortNumberProperty = DependencyProperty.Register("ShowSortNumber", typeof(bool), typeof(GridDataTableProperties), new PropertyMetadata(false));

        #endregion


        #region EnableTriStateSorting (Dependency Property)

        public bool EnableTriStateSorting
        {
            get { return (bool)GetValue(EnableTriStateSortingProperty); }
            set { SetValue(EnableTriStateSortingProperty, value); }
        }

        public static readonly DependencyProperty EnableTriStateSortingProperty = DependencyProperty.Register("EnableTriStateSorting", typeof(bool), typeof(GridDataTableProperties), new PropertyMetadata(false));

        #endregion

        #region FilterBarPredicateType (Dependency Property)

        public PredicateType FilterBarPredicateType
        {
            get { return (PredicateType)GetValue(FilterBarPredicateTypeProperty); }
            set { SetValue(FilterBarPredicateTypeProperty, value); }
        }

        public static readonly DependencyProperty FilterBarPredicateTypeProperty = DependencyProperty.Register("FilterBarPredicateType", typeof(PredicateType), typeof(GridDataTableProperties), new PropertyMetadata(PredicateType.And));
        #endregion

        #region DefaultFilterOperator (Dependency Property)
        /// <summary>
        /// Gets or sets DefaultFilterOperator value for Filter bar.
        /// </summary>
        /// <value></value>
        /// <remarks>DefaultFilterOperator contains StartsWith, Contains and Equals property in Enum value. This is set for Default filter value where we can use the filter value with out wildcard.</remarks>
       
        public FilterOperatorType DefaultFilterOperator
            {
            get { return (FilterOperatorType)GetValue(DefaultFilterOperatorProperty); }
            set { SetValue(DefaultFilterOperatorProperty, value); }
            }

        public static readonly DependencyProperty DefaultFilterOperatorProperty = DependencyProperty.Register("DefaultFilterOperator", typeof(FilterOperatorType), typeof(GridDataTableProperties), new PropertyMetadata(FilterOperatorType.StartsWith));
        #endregion

        #region SourceType (DependencyProperty)

        /// <summary>
        /// Gets / sets source type
        /// </summary>
        [XmlIgnore]
        public Type SourceType
        {
            get { return (Type)GetValue(SourceTypeProperty); }
            set { SetValue(SourceTypeProperty, value); }
        }

        public static readonly DependencyProperty SourceTypeProperty = DependencyProperty.Register("SourceType", typeof(Type), typeof(GridDataTableProperties), new PropertyMetadata(null));

        #endregion

        #region CustomGroupComparer (DependencyProperty)

        /// <summary>
        /// Gets / sets the custom group comparer;
        /// </summary>
        [XmlIgnore]
        public IComparer<Group> CustomGroupComparer
        {
            get { return (IComparer<Group>)GetValue(CustomGroupComparerProperty); }
            set { SetValue(CustomGroupComparerProperty, value); }
        }

        public static readonly DependencyProperty CustomGroupComparerProperty = DependencyProperty.Register("CustomGroupComparer", typeof(IComparer<Group>), typeof(GridDataTableProperties), new PropertyMetadata(null));

        #endregion

        #region ExpressionFunc (DependencyProperty)

        /// <summary>
        /// Gets / sets the custom expression functor.
        /// </summary>
        [XmlIgnore]
        public IUnboundExpressionFunc ExpressionFunc
        {
            get { return (IUnboundExpressionFunc)GetValue(ExpressionFuncProperty); }
            set { SetValue(ExpressionFuncProperty, value); }
        }

        public static readonly DependencyProperty ExpressionFuncProperty = DependencyProperty.Register("ExpressionFunc", typeof(IUnboundExpressionFunc), typeof(GridDataTableProperties), new PropertyMetadata(OnExpressionFuncChanged));

        private static void OnExpressionFuncChanged(DependencyObject dpo, DependencyPropertyChangedEventArgs args)
        {
            var tableProperties = dpo as GridDataTableProperties;
            // the underlying view should be updated only when it is initialized, before initialization it will be taken care of by the model.
            if (tableProperties.Model != null && tableProperties.Model.View != null)
            {
                if (args.NewValue != null)
                {
                    var expressionFunc = args.NewValue as IUnboundExpressionFunc;
                    tableProperties.Model.View.SetCustomExpressionFunc(expressionFunc);
                }
                else
                {
                    tableProperties.Model.View.SetCustomExpressionFunc(null);
                }
            }
        }

        #endregion

        #region IDisposable Members

        public void Dispose()
        {
            this.UnwireEvents();
            this.AlternatingRowBackground = null;
            this.AlternatingRowForeground = null;  
            if(this.ConditionalFormats!=null)
                this.ConditionalFormats.Clear();  
            this.CustomVisualStyle = null;
            this.DragIndicatorInnerBrush = null;
#if !SILVERLIGHT
            this.DragIndicatorOuterBrush = null;
            this.GroupHeaderContextMenuItems = null;
            this.HeaderContextMenuItems = null;
            this.RecordContextMenuItems = null;
#endif
            this.ExpressionFunc = null;            
            this.GroupCaptionText = null;
            if(this.GroupedColumns!=null)
                this.GroupedColumns.Clear();      
            this.HeaderCellTemplate = null; 
            if(this.Relations!=null)
                this.Relations.Clear();
            this.RowBackground = null;
            this.RowForeground = null;
            if (this.SortColumns != null)
                this.SortColumns.Clear();
            this.SourceType = null;
            if(this.StackedHeaderRows!=null)
                this.StackedHeaderRows.Clear();
            if(this.SummaryRows!=null)
                this.SummaryRows.Clear();
            if(this.TableSummaryRows!=null)
                this.TableSummaryRows.Clear();
            if(this.VisibleColumns!=null)
                this.VisibleColumns.Dispose();
            if (this.Relations != null)
            {
                this.Relations.Clear();
                this.Relations = null;
            }
            this.VisibleColumns = null;
            this.SummaryRows = null;
            this.StackedHeaderRows = null;
            this.SortColumns = null;
            this.TableSummaryRows = null;
            this.GroupedColumns = null;
            this.ConditionalFormats = null;
            if (this.summaryColumns != null)
            {
                this.summaryColumns.Clear();
                this.summaryColumns = null;
            }
            this.CaptionSummaryRow = null;           
        }

        #endregion

        internal void UnwireEvents()
        {
            if (this.TableSummaryRows != null)
            {
                ((INotifyCollectionChanged)this.TableSummaryRows).CollectionChanged -= this.TableSummaryRows_CollectionChanged;
            }

            if (this.SummaryRows != null)
            {
                ((INotifyCollectionChanged)this.SummaryRows).CollectionChanged -= this.OnSummaryRowsChanged;
            }

            if (this.SortColumns != null)
            {
                ((INotifyCollectionChanged)this.SortColumns).CollectionChanged -= this.OnSortColumnsCollectionChanged;
            }

            if (this.GroupedColumns != null)
            {
                ((INotifyCollectionChanged)this.GroupedColumns).CollectionChanged -= this.OnGroupedColumnsChanged;
            }
        }

        public void SuspendEvents()
        {
            if (!this.IsInSuspend)
            {
                this.IsInSuspend = true;
                if(this.VisibleColumns !=null)
                this.VisibleColumns.SuspendEvents();
            }
        }

        [XmlIgnore]
        public bool IsInSuspend
        {
            get;
            private set;
        }

        public void ResumeEvents()
        {
            if (this.IsInSuspend)
            {
                this.IsInSuspend = false;
                this.VisibleColumns.ResumeEvents();
            }
        }

        internal void WireEvents()
        {
            this.VisibleColumns.SetTableModel(this.Model);
            ((INotifyCollectionChanged)this.TableSummaryRows).CollectionChanged += this.TableSummaryRows_CollectionChanged;
            ((INotifyCollectionChanged)this.SummaryRows).CollectionChanged += this.OnSummaryRowsChanged;
            ((INotifyCollectionChanged)this.SortColumns).CollectionChanged += this.OnSortColumnsCollectionChanged;
            ((INotifyCollectionChanged)this.GroupedColumns).CollectionChanged += this.OnGroupedColumnsChanged;
            //this.Relations.CollectionChanged += this.Relations_CollectionChanged;
        }

        void TableSummaryRows_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            //var tableSummaryRows = this.Model.View.TableSummaryRows;
            if (e.Action == NotifyCollectionChangedAction.Add)
            {
                //tableSummaryRows.Add(e.NewItems[0] as ISummaryRow);
                if (this.Model.View != null)
                {
                    var view = this.Model.View;
                    view.TableSummaryRows.Add(e.NewItems[0] as ISummaryRow);
                }

                this.Model.Table.SetFooterRows();
            }
            else if (e.Action == NotifyCollectionChangedAction.Remove)
            {
                if (this.Model.View != null)
                {
                    foreach (var row in e.OldItems)
                    {
                        var view = this.Model.View;
                        view.TableSummaryRows.Remove(row as ISummaryRow);
                        SummaryRecordEntry needRemoveRecord = this.Model.View.Records.TableSummaries.FirstOrDefault(record => record.SummaryRow == row);
                        view.Records.TableSummaries.Remove(needRemoveRecord);
                    }
                }
            }
            else if (e.Action == NotifyCollectionChangedAction.Reset)
            {
                //tableSummaryRows.Clear();
                if (this.Model.View != null)
                {
                    var view = this.Model.View;
                    view.TableSummaryRows.Clear();
                    view.Records.TableSummaries.Clear();
                }
            }
            this.Model.View.OnCollectionChanged(e);
            this.Model.InvalidateDisplay();
        }

        internal bool isGroupApplied = false;
        internal bool inGroupColumnsChanged = false;
        private void OnGroupedColumnsChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            if (this.IsInSuspend)
            {
                return;
            }

            if (this.Model.View == null)
                return;

            this.inGroupColumnsChanged = true;
            if (e.Action == NotifyCollectionChangedAction.Add)
            {
                var groupColumn = e.NewItems[0] as GridDataGroupColumn;
                this.Model.View.GroupDescriptions.Insert(e.NewStartingIndex, new PropertyGroupDescription() { PropertyName = groupColumn.ColumnName, Converter = groupColumn.Converter });
                if (this.HideColumnsWhenGrouped)
                {
                    var visibleColumn = this.VisibleColumns.FirstOrDefault(v => v.MappingName == groupColumn.ColumnName);
                    if (visibleColumn != null)
                    {
                        visibleColumn.IsInSuspend = true;
                        visibleColumn.IsHidden = true;
                        visibleColumn.IsInSuspend = false;
                    }
                }

                if (this.ExpandGroupsWhenGrouped)
                    this.Model.Table.ExpandAllGroups();

                this.Model.Table.RaiseGroupedColumnsChanged(new List<GridDataGroupColumn>() { groupColumn }, null, NotifyCollectionChangedAction.Add);
            }
            else if (e.Action == NotifyCollectionChangedAction.Remove)
            {
                var groupColumn = e.OldItems[0] as GridDataGroupColumn;
                var groupDesc = this.Model.View.GroupDescriptions.OfType<PropertyGroupDescription>().FirstOrDefault(g => g.PropertyName == groupColumn.ColumnName);
                if (groupDesc != null)
                {
                    this.Model.View.GroupDescriptions.Remove(groupDesc);
                }
                if (this.HideColumnsWhenGrouped)
                {
                    var visibleColumn = this.VisibleColumns.FirstOrDefault(v => v.MappingName == groupColumn.ColumnName);
                    if (visibleColumn != null)
                    {
                        visibleColumn.IsInSuspend = true;
                        visibleColumn.IsHidden = false;
                        visibleColumn.IsInSuspend = false;
                    }
                }

                this.Model.Table.RaiseGroupedColumnsChanged(null, new List<GridDataGroupColumn>() { groupColumn }, NotifyCollectionChangedAction.Remove);
            }
            else if (e.Action == NotifyCollectionChangedAction.Reset)
            {
                this.Model.View.GroupDescriptions.Clear();
            }

            //The below condition was added to skip the Update SelectedRanges if we group\ungroup by dragging the Column header,
            //In this case this will be invoked by GridDataGroupDropAreaMouseController.
            if (!this.Model.IsInGroup &&e.Action!=NotifyCollectionChangedAction.Reset)
            {
                this.Model.IsInGroup = true;
                this.Model.UpdateSelectedRanges();
                this.Model.IsInGroup = false;
            }

            var dataGrid = this.Model.Grid.FindParentElementOfType<GridDataControl>();
            if (dataGrid != null && dataGrid.GroupDropAreaGrid != null && dataGrid.GroupDropAreaGrid.Model !=null)
            {                
                dataGrid.GroupDropAreaGrid.InvalidateCells();                
            }

            if (this.GroupedColumns.Count == 0)
            {
                if (e.Action == NotifyCollectionChangedAction.Remove||e.Action==NotifyCollectionChangedAction.Reset)
                {
                    this.Model.RefreshColumns(true, false);
                }
                else
                {
                    this.Model.RefreshColumns(false, false);
                }
            }
                        
            //The below lines of code was added to refresh the header Column.
            var index = this.StackedHeaderRows.Count;
            this.Model.Grid.InvalidateCell(GridRangeInfo.Row(index));
            
            this.isGroupApplied = true;
            this.inGroupColumnsChanged = false;
        }

        internal bool inSortColumnsChanged = false;

        protected virtual void OnSortColumnsCollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            if (this.IsInSuspend || this.Model.View == null)
            {
                return;
            }

            this.inSortColumnsChanged = true;
            if (e.Action == NotifyCollectionChangedAction.Add)
            {
                var sortColumn = e.NewItems[0] as GridDataSortColumn;
                if (!this.IsViewLevelPaging && this.Model.View.PagedSource != null && this.EnablePaging)
                {
                    if (this.Model.View.PagedSource.SortDescriptions.Count < e.NewStartingIndex)
                        this.Model.View.PagedSource.SortDescriptions.Insert(this.Model.View.PagedSource.SortDescriptions.Count, new SortDescription() { PropertyName = sortColumn.ColumnName, Direction = sortColumn.SortDirection });
                    else
                        this.Model.View.PagedSource.SortDescriptions.Insert(e.NewStartingIndex, new SortDescription() { PropertyName = sortColumn.ColumnName, Direction = sortColumn.SortDirection });
                    this.Model.View.SortDescriptions.Insert(e.NewStartingIndex, new SortDescription() { PropertyName = sortColumn.ColumnName, Direction = sortColumn.SortDirection });
                }
                else
                    this.Model.View.SortDescriptions.Insert(e.NewStartingIndex, new SortDescription() { PropertyName = sortColumn.ColumnName, Direction = sortColumn.SortDirection });


                //Till This
                // add the custom comparer
                if (sortColumn.CustomComparer != null)
                {
                    if (this.Model.View.SortComparers.ContainsKey(sortColumn.ColumnName))
                    {
                        this.Model.View.SortComparers.Remove(sortColumn.ColumnName);
                    }
                    this.Model.View.SortComparers.Add(sortColumn.ColumnName, sortColumn.CustomComparer);
                }

            }
            else if (e.Action == NotifyCollectionChangedAction.Remove)
            {
                var sortColumn = e.OldItems[0] as GridDataSortColumn;
                var sortDesc = this.Model.View.SortDescriptions.FirstOrDefault(s => s.PropertyName == sortColumn.ColumnName);
                if (sortDesc != null)
                {
                    this.Model.View.SortDescriptions.Remove(sortDesc);

                    if (this.Model.View.SortComparers.ContainsKey(sortColumn.ColumnName))
                    {
                        this.Model.View.SortComparers.Remove(sortColumn.ColumnName);
                    }

                }
            }
            else if (e.Action == NotifyCollectionChangedAction.Replace)
            {
                var sortColumn = e.NewItems[0] as GridDataSortColumn;
                var sortDesc = this.Model.View.SortDescriptions.FirstOrDefault(s => s.PropertyName == sortColumn.ColumnName);
                if (sortDesc != null)
                {
                    this.Model.View.SortDescriptions.Remove(sortDesc);

                    if (this.Model.View.SortComparers.ContainsKey(sortColumn.ColumnName))
                    {
                        this.Model.View.SortComparers.Remove(sortColumn.ColumnName);
                    }

                    this.Model.View.SortDescriptions.Insert(e.NewStartingIndex, new SortDescription() { PropertyName = sortColumn.ColumnName, Direction = sortColumn.SortDirection });


                    if (sortColumn.CustomComparer != null)
                    {
                        this.Model.View.SortComparers.Add(sortColumn.ColumnName, sortColumn.CustomComparer);
                    }

                }
            }
            else if (e.Action == NotifyCollectionChangedAction.Reset)
            {
                using (this.Model.View.DeferRefresh())
                {
                    this.Model.View.SortDescriptions.Clear();
#if SyncfusionFramework4_0
                    this.Model.View.SortComparers.Clear();
#endif
                }
            }

            //the below method was invoked multiple times while grouping, so i have skip this method while grouping.
            //When grouping this method will be invoked from OnGroupedColumnsChanged or GroupDragMouseController.
            if(!this.Model.IsInGroup)
                this.Model.UpdateSelectedRanges();

            var dataGrid = this.Model.Grid.FindParentElementOfType<GridDataControl>();
            
            if (dataGrid != null && dataGrid.ShowGroupDropArea && dataGrid.GroupDropAreaGrid.Model != null)
            {
                dataGrid.GroupDropAreaGrid.InvalidateCells();
            }

            if (this.GroupedColumns.Count > 0 || !this.Model.IsInSort)
            {
                var index = 0;
                if (this.StackedHeaderRows.Count > 0)
                {
                    index = this.StackedHeaderRows.Count;
                }

                this.Model.Grid.InvalidateCell(GridRangeInfo.Row(index));
            }

            this.inSortColumnsChanged = false;
        }

        private void OnSummaryRowsChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            if (e.Action == NotifyCollectionChangedAction.Add)
            {
                var summaryRow = e.NewItems[0] as GridDataSummaryRow;
                if (this.Model.View != null)
                {
                    var view = this.Model.View;
                    view.SummaryRows.Add(e.NewItems[0] as ISummaryRow);
                }
                summaryRow.SetTableModel(this.Model);
            }
            else if (e.Action == NotifyCollectionChangedAction.Remove)
            {
                // var summaryRow = e.NewItems[0] as GridDataSummaryColumn; Unused local variable
                if (this.Model.View != null)
                {
                    var view = this.Model.View;
                    view.SummaryRows.Remove(e.NewItems[0] as ISummaryRow);
                }
            }
            else if (e.Action == NotifyCollectionChangedAction.Reset)
            {
                if (this.Model.View != null)
                {
                    this.Model.View.SummaryRows.Clear();
                }
            }
            this.Model.ForceFilterRefresh();
        }

        internal GridDataSortColumn GetSortColumnForGroup(GridDataGroupColumn groupColumn)
        {
            return this.SortColumns.FirstOrDefault(s => s.ColumnName == groupColumn.ColumnName);
        }

        internal GridDataSortColumn GetSortColumnForGroup(PropertyGroupDescription groupDesc)
        {
            return this.SortColumns.FirstOrDefault(s => s.ColumnName == groupDesc.PropertyName);
        }

#if !SILVERLIGHT
        protected override Freezable CreateInstanceCore()
        {
            throw new NotImplementedException();
        }
#endif
    }

    public enum Position
    {
        Top,
        Bottom
    }
}