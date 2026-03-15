#region Copyright Syncfusion Inc. 2001 - 2014
// Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Any infringement will be prosecuted under
// applicable laws. 
#endregion
namespace Syncfusion.Windows.Controls.Grid
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Net;
    using System.Windows;
    using System.Windows.Controls;
    using System.Windows.Documents;
    using System.Windows.Input;
    using System.Windows.Media;
    using System.Windows.Media.Animation;
    using System.Windows.Shapes;
    using System.Collections.ObjectModel;
    using Syncfusion.Windows.ComponentModel;
    using System.Windows.Data;
    using Syncfusion.Windows.Data;
#if SILVERLIGHT
    using System.Windows.Media.Imaging;
    using System.ComponentModel;
    using Syncfusion.Windows.Controls.Theming;
    using Syncfusion.Linq;
    using Syncfusion.Windows.Controls.Scroll;
#endif

#if SyncfusionFramework4_0
    [Bindable(true), DefaultBindingProperty("ItemsSource")]
    [StyleTypedProperty(Property = "HeaderStyle", StyleTargetType = typeof(GridDataHeaderCellControl))]
    [StyleTypedProperty(Property = "ScrollViewerStyle", StyleTargetType = typeof(ScrollableContentViewer))]
#endif

    public class GridDataControl : ContentControl, ISkinStylePropagator
    {
        #region Constants

        private const double StatusBarHeight = 30.00;
        internal const string GroupCaptionConstant = "{ColumnName} : {Key} - {ItemsCount} Items";
        public const string TemplateGroupDropAreaGrid = "PART_GroupDropAreaGrid";
        public const string TemplateGrid = "PART_GridControl";

        #endregion

        #region Fields

        private bool isGridLoaded = false;
        private GridDataTableModel ctorModel = null;
        private bool ensuredProperties = false;
        private Border PART_BorderStatusBar;
        private Button PART_CloseButton;

        #endregion

        #region ctor

        public GridDataControl()
        {
            //GridDataResourceWrapper wr = new GridDataResourceWrapper();
            this.DefaultStyleKey = typeof(GridDataControl);
            this.TableProperties = this.GetTableProperties();
            this.CopyInitializePropertyCollections();
            this.SelectedItems = new ObservableCollection<object>();
            this.ConditionalFormats.CollectionChanged += new System.Collections.Specialized.NotifyCollectionChangedEventHandler(this.OnConditionalFormatsChanged);
            this.Model.SelectionChanged -= new GridSelectionChangedEventHandler(Model_SelectionChanged);
            this.Model.SelectionChanged += new GridSelectionChangedEventHandler(Model_SelectionChanged);
            this.Loaded += new RoutedEventHandler(GridDataControl_Loaded);
            this.SelectedItems.CollectionChanged += new System.Collections.Specialized.NotifyCollectionChangedEventHandler(SelectedItems_CollectionChanged);
          
        }

        void SelectedItems_CollectionChanged(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
        {
            if (resetSelectedItems)
            {
                return;
            }
            switch (e.Action)
            {
                case System.Collections.Specialized.NotifyCollectionChangedAction.Add:
                    int recordIndex = this.Model.View.Records.IndexOfRecord(e.NewItems[0]);
                    int rowIndex;
                    if (!Model.Table.HasGroups)
                    {
                        rowIndex = this.Model.ResolvePositionToIndex(recordIndex);
                    }
                    else
                    {
                        rowIndex = this.Model.ResolveGroupRecordPositionToIndex(recordIndex);//Get the rowIndex when the DataGrid is Grouping
                        if (rowIndex < 0)
                        {
                            return;
                        }
                    }
                    int x = this.ShowRowHeader == true ? 1 : 0;
                    GridRangeInfo range = new GridRangeInfo(rowIndex, x, rowIndex, this.Model.ColumnCount - 1);
                    if (!this.Model.SelectedRanges.Contains(range))
                    {
                        this.Model.SelectedRanges.Add(range);
                        this.Model.InvalidateCell(range);
                    }
                    break;
                case System.Collections.Specialized.NotifyCollectionChangedAction.Remove:
                    if (!this.Model.Table.HasGroups)
                        recordIndex = this.Model.View.Records.IndexOfRecord(e.OldItems[0]);
                    else
                        recordIndex = this.Model.View.TopLevelGroup.IndexOf(e.OldItems[0]);
                    rowIndex = this.Model.ResolvePositionToIndex(recordIndex);
                    if (rowIndex > -1)
                    {
                        range = GridRangeInfo.Row(rowIndex);//new GridRangeInfo(GridRangeInfoType.Rows,rowIndex, x, rowIndex, this.Model.ColumnCount - 1);
                        bool valid = false;
                        for (int i = this.Model.SelectedRanges.ActiveRange.Top; i <= this.Model.SelectedRanges.ActiveRange.Bottom; i++)
                        {
                            if (i == range.Top)
                                valid = true;
                        }
                        if (valid)
                        //if (this.Model.Selections.Contains(range))
                        {
                            this.Model.Selections.Remove(range);
                            this.Model.InvalidateCell(range);
                            valid = false;
                        }
                    }
                    break;
                case System.Collections.Specialized.NotifyCollectionChangedAction.Reset:
                    GridRangeInfoList rangeList = this.Model.SelectedRanges.Clone();
                    this.Model.SelectedRanges.Clear();
                    foreach (GridRangeInfo r in rangeList)
                    {
                        this.Model.InvalidateCell(r);
                    }
                    break;

            }
            this.Model.InvalidateVisual(true);
        }

        void Model_SelectionChanged(object sender, GridSelectionChangedEventArgs e)
        {
            if (SelectedChildModel != null)
            {
                if ((e.Reason == GridSelectionReason.MouseDown || e.Reason == GridSelectionReason.Clear) && SelectedChildModel.Grid.CurrentCell.HasCurrentCell)
                {
                    this.InternalGrid.Focus();
                }
                if (SelectedChildModel.Grid != null)
                {
                    var nestedGrid = SelectedChildModel.Grid as GridDataCellNestedGridEditor;
                    if (nestedGrid != null)
                        nestedGrid.ClearChildGridSelections(SelectedChildModel);
                }
                SelectedChildModel = null;
            }
            if (this.selectedItemChangedFlag)
            {
                //If Model.IsInSort then we dont reset the selected Items, bcoz based on the selected Items we update the selected ranges while sorting.
                if ((e.Reason == GridSelectionReason.MouseUp || (e.Reason == GridSelectionReason.ArrowKey) && !this.Model.IsInSort&&!this.Model.IsInGroup))
                {
                    this.ResetSelectedItems();
                    this.selectedItemChangedFlag = false;
                }
            }
            else if (e.Reason == GridSelectionReason.MouseUp && e.Range.Height > 0 && e.Range.Width > 0)
            {
                // bool flag = false;
                for (int index = e.Range.Top; index <= e.Range.Bottom; index++)
                {
                    int recordIndex = this.Model.ResolveIndexToRecordPosition(index);
                    object record = null;
                    if (!this.Model.Table.HasGroups)
                    {
                        if (recordIndex < this.Model.View.Records.Count && recordIndex > -1)
                        {
                            record = this.Model.View.Records.GetItemAt(recordIndex);
                        }
                    }
                    else
                    {
                        if (this.Model.View.TopLevelGroup.DisplayElements.Count >= recordIndex)
                        {
                            var recordEntry = this.Model.View.TopLevelGroup.DisplayElements[recordIndex] as RecordEntry;
                            if (recordEntry != null)
                            {
                                record = recordEntry.Data;
                            }
                        }
                    }                    
                }
                this.ResetSelectedItems();                
            }       
     
            //We do not reset the selected items while performing sorting, grouping, filtering.
            if ((e.Reason == GridSelectionReason.SelectRange || e.Reason == GridSelectionReason.Clear||e.Reason==GridSelectionReason.ArrowKey) && !this.Model.IsInSort && !this.Model.IsInFilter&&!this.Model.IsInGroup)                        
            {
                ResetSelectedItems();
            }

            //Here we update the selection based on SelectedItems while Filtering, Grouping.
            if (this.Model.IsInFilter)
                this.Model.UpdateSelectedRanges();

            //While deleting the row following operation should needed to maintain the selection.
            if (e.Reason == GridSelectionReason.DeleteRow)
            {
                //Here we Update the selected ranges based on Selected Items and Move the Current Cell.
                this.Model.UpdateSelectedRanges();
                if (this.Model.SelectedRanges.Count == 0)
                {
                    this.Model.SelectedRanges.Add(e.Range);
                    if (this.InternalGrid.CurrentCell.RowIndex == -1 && !this.Model.SelectedRanges.ActiveRange.IsEmpty)
                        this.InternalGrid.CurrentCell.MoveTo(this.Model.SelectedRanges.ActiveRange.Top, this.InternalGrid.CurrentCell.ColumnIndex);
                }
                else if (this.InternalGrid.CurrentCell.RowIndex == -1)
                {
                    this.InternalGrid.CurrentCell.MoveTo(this.Model.SelectedRanges.ActiveRange.Top, this.InternalGrid.CurrentCell.ColumnIndex);
                }
            }
            this.Model.Grid.InvalidateVisual();
        }

        #endregion

        #region Events

        /// <summary>
        /// Occurs when the Model is loaded. This is useful to listen to Model events when
        /// the control is initialized. 
        /// <para></para>
        /// <code lang="C#">            
        ///             this.dataGrid.ModelLoaded += (sender, args) =&gt;
        ///             {
        ///                 this.dataGrid.Model.QueryCellInfo += new
        ///                 Syncfusion.Windows.Controls.Grid.GridQueryCellInfoEventHandler(Model_QueryCellInfo);
        ///             };
        ///             </code>
        /// </summary>
        public event EventHandler ModelLoaded;

        /// <summary>
        /// Occurs when ItemsSource for GridDataControl is Changed.
        /// </summary>
        public event GridRoutedEventHandler ItemsSourceChanged;


        #region RowValueCommitting

        /// <summary>
        /// This event only for UpdateMode = RowCachedMode. Occurs Row focus moved to another row.
        /// </summary>
        public event GridDataRowValueCommittingEventHandler RowValueCommitting;

        internal bool RaiseRowValueCommittingEvent(GridDataRecord record, Dictionary<string, object> value, int rowIndex)
        {
            GridDataRowValueCommittingEventArgs args = new GridDataRowValueCommittingEventArgs()
            {
                Record = record,
                EditedValues = value,
                RowIndex = rowIndex
            };

            if (this.RowValueCommitting != null)
            {
                this.RowValueCommitting(this, args);
            }

            return args.Cancel;
        }

        #endregion

        #region RowValueCommitted

        /// <summary>
        /// This event only for UpdateMode = RowCachedMode. If committing row values success means this event will fires
        /// </summary>
        public event GridDataRowValueCommittedEventHandler RowValueCommitted;

        /// <summary>
        /// Raises the row value committed event.
        /// </summary>
        /// <param name="record">The record.</param>
        /// <param name="value">The value.</param>
        /// <param name="rowIndex">Index of the row.</param>
        /// <returns></returns>
        internal bool RaiseRowValueCommittedEvent(GridDataRecord record, Dictionary<string, object> value, int rowIndex)
        {
            GridDataRowValueCommittedEventArgs args = new GridDataRowValueCommittedEventArgs()
            {
                Record = record,
                NewValues = value,
                RowIndex = rowIndex
            };

            if (this.RowValueCommitted != null)
            {
                this.RowValueCommitted(this, args);
            }

            return args.Cancel;
        }

        #endregion

        #region RowValueCommittingCancelled

        /// <summary>
        /// This event only for UpdateMode = RowCachedMode. If committing row values cancelled means this event will fires
        public event GridDataRowValueCommittingCancelledEventHandler RowValueCommittingCancelled;

        /// <summary>
        /// Raises the row value committing cancelled event.
        /// </summary>
        /// <param name="record">The record.</param>
        /// <param name="rowIndex">Index of the row.</param>
        internal void RaiseRowValueCommittingCancelledEvent(GridDataRecord record, int rowIndex)
        {
            GridDataRowValueCommittingCancelledEventArgs args = new GridDataRowValueCommittingCancelledEventArgs()
            {
                Record = record,
                RowIndex = rowIndex
            };

            if (this.RowValueCommittingCancelled != null)
            {
                this.RowValueCommittingCancelled(this, args);
            }
        }

        #endregion

        #region RowValidating

        /// <summary>
        /// Occurs Row focus moved to another row.
        /// </summary>
        public event GridDataRowValidatingEventHandler RowValidating;

        /// <summary>
        /// Raises the row validating event.
        /// </summary>
        /// <param name="currentRowIndex">Index of the current row.</param>
        /// <param name="record">The record.</param>
        /// <param name="newValues">The new values.</param>
        /// <returns></returns>
        internal bool RaiseRowValidatingEvent(int currentRowIndex, object record, Dictionary<string, object> newValues)
        {
            GridDataRowValidatingEventArgs args = new GridDataRowValidatingEventArgs()
            {
                IsValid = true,
                RowIndex = currentRowIndex,
                Record = record,
                NewValues = newValues
            };

            if (this.RowValidating != null)
            {
                this.RowValidating(this, args);
            }
            return args.IsValid;
        }

        #endregion

        #endregion

        #region CellRequestNavigateCommand

        public static readonly DependencyProperty CellRequestNavigateCommandProperty = DependencyProperty.Register(
           "CellRequestNavigateCommand",
           typeof(ICommand),
           typeof(GridDataControl),
           new PropertyMetadata(null));

        public ICommand CellRequestNavigateCommand
        {
            get
            {
                return (ICommand)this.GetValue(GridDataControl.CellRequestNavigateCommandProperty);
            }
            set
            {
                this.SetValue(GridDataControl.CellRequestNavigateCommandProperty, value);
            }
        }

        #endregion

        #region PublicProperties

        /// <summary>
        /// 
        /// </summary>
        public GridDataControlBaseImpl InternalGrid
        {
            get;
            private set;
        }


        /// <summary>
        /// Gets or sets the group drop area grid.
        /// </summary>
        /// <value>The group drop area grid.</value>
        public GridDataGroupDropAreaGridImpl GroupDropAreaGrid
        {
            get;
            internal set;
        }

        #endregion

        #region InternalProperties

        public static readonly DependencyProperty TablePropertiesProperty = DependencyProperty.Register("TableProperties", typeof(GridDataTableProperties), typeof(GridDataControl), new PropertyMetadata(null));

        /// <summary>
        /// 
        /// </summary>
        internal GridDataTableProperties TableProperties
        {
            get
            {
                return (GridDataTableProperties)this.GetValue(GridDataControl.TablePropertiesProperty);
            }
            set
            {
                this.SetValue(GridDataControl.TablePropertiesProperty, value);
            }
        }

        internal static VisualStyle ResolveSkinStorageToVisualStyle(Theming.VisualStyle visualStyle)
        {
            switch (visualStyle)
            {
                case Theming.VisualStyle.Office2007Blue:
                    return VisualStyle.Office2007Blue;
                case Theming.VisualStyle.Office2007Silver:
                    return VisualStyle.Office2007Silver;
                case Theming.VisualStyle.Office2007Black:
                    return VisualStyle.Office2007Black;
                case Theming.VisualStyle.Blend:
                    return VisualStyle.Blend;
                case Theming.VisualStyle.Office2003:
                    return VisualStyle.Office2003;
                case Theming.VisualStyle.VS2010:
                    return VisualStyle.VS2010;
                default:
                    return VisualStyle.Default;
            }
        }

        internal static Theming.VisualStyle ResolveVisualStyleToSkinStorage(VisualStyle visualStyle)
        {
            var result = default(Theming.VisualStyle);

            switch (visualStyle)
            {
                case VisualStyle.Office2007Blue:
                case VisualStyle.Office14Blue:
                case VisualStyle.DefaultOffice2007Blue:
                case VisualStyle.BureauBlue:
                case VisualStyle.ShinyBlue:
                    result = Theming.VisualStyle.Office2007Blue;
                    break;
                case VisualStyle.DefaultOffice2007Silver:
                case VisualStyle.Office2007Silver:
                case VisualStyle.Office14Silver:
                    result = Theming.VisualStyle.Office2007Silver;
                    break;
                case VisualStyle.DefaultOffice2007Black:
                case VisualStyle.Office2007Black:
                case VisualStyle.Office14Black:
                case VisualStyle.SunBlack:
                case VisualStyle.ShinyRed:
                case VisualStyle.GlassyGreen:
                    result = Theming.VisualStyle.Office2007Black;
                    break;
                case VisualStyle.Blend:
                case VisualStyle.BureauBlack:
                    result = Theming.VisualStyle.Blend;
                    break;
                case VisualStyle.Office2003:
                    result = Theming.VisualStyle.Office2003;
                    break;
                case VisualStyle.VS2010:
                    result = Theming.VisualStyle.VS2010;
                    break;
                case VisualStyle.Default:
                    result = Theming.VisualStyle.Default;
                    break;
            }
            return result;
        }


        /// <summary>
        /// Gets or sets the current scroll child. This is used to close the filter popup in various nested levels.
        /// </summary>
        /// <value>The current scroll child.</value>
        internal ScrollControlChildFrame CurrentScrollChild
        {
            get;
            set;
        }

        #endregion

        #region DependencyProperties

        #region ActivateCurrentCellBehavior

        public static readonly DependencyProperty ActivateCurrentCellBehaviorProperty = DependencyProperty.Register("ActivateCurrentCellBehavior", typeof(GridCellActivateAction), typeof(GridDataControl), new PropertyMetadata(GridCellActivateAction.DblClickOnCell, OnActivateCurrentCellBehaviorChanged));

        public GridCellActivateAction ActivateCurrentCellBehavior
        {
            get
            {
                return (GridCellActivateAction)GetValue(ActivateCurrentCellBehaviorProperty);
            }

            set
            {
                SetValue(ActivateCurrentCellBehaviorProperty, value);
            }
        }

        private static void OnActivateCurrentCellBehaviorChanged(DependencyObject d, DependencyPropertyChangedEventArgs args)
        {
            var gridDataControl = d as GridDataControl;
            gridDataControl.Model.Options.ActivateCurrentCellBehavior = (GridCellActivateAction)args.NewValue;
        }

        #endregion

        #region AllowDelete

        public static readonly DependencyProperty AllowDeleteProperty =
            DependencyProperty.Register("AllowDelete", typeof(bool), typeof(GridDataControl),
                new PropertyMetadata(true, OnAllowDeleteChanged));

        /// <summary>
        /// Gets / Sets if delete is allowed.
        /// </summary>
        public bool AllowDelete
        {
            get { return (bool)GetValue(AllowDeleteProperty); }
            set { SetValue(AllowDeleteProperty, value); }
        }

        private static void OnAllowDeleteChanged(DependencyObject dpo, DependencyPropertyChangedEventArgs args)
        {
            var grid = dpo as GridDataControl;
            grid.TableProperties.AllowDelete = (bool)args.NewValue;
        }

        public static readonly DependencyProperty AllowMultipleRecordDeletionProperty = DependencyProperty.Register(
            "AllowMultipleRecordDeletion",
            typeof(bool),
            typeof(GridDataControl),
            new PropertyMetadata(false, OnAllowMultipleRecordDeletionChanged));

        #endregion

        #region AllowMultipleRecordDeletion

        public bool AllowMultipleRecordDeletion
        {
            get
            {
                return (bool)GetValue(AllowMultipleRecordDeletionProperty);
            }
            set
            {
                SetValue(AllowMultipleRecordDeletionProperty, value);
            }
        }

        private static void OnAllowMultipleRecordDeletionChanged(DependencyObject d, DependencyPropertyChangedEventArgs args)
        {
            GridDataControl grid = d as GridDataControl;
            grid.TableProperties.AllowMultipleRecordDeletion = (bool)args.NewValue;
        }

        #endregion

        #region AllowDragColumns

        /// <summary>
        /// DependencyProperty for <see cref = "GridDataControl.AllowDragColumns"/>.
        /// </summary>
        public static readonly DependencyProperty AllowDragColumnsProperty = DependencyProperty.Register(
            "AllowDragColumns",
            typeof(bool),
            typeof(GridDataControl),
            new PropertyMetadata(false, OnAllowDragColumnsChanged));

        private bool isAllowDragChangedBeforeGridLoaded = false;

        private static void OnAllowDragColumnsChanged(DependencyObject d, DependencyPropertyChangedEventArgs args)
        {
            var grid = d as GridDataControl;
            if (grid.isGridLoaded)
            {
                grid.TableProperties.AllowDragColumns = (bool)args.NewValue;
            }
            else
            {
                grid.isAllowDragChangedBeforeGridLoaded = true;
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether [allow drag columns].
        /// </summary>
        /// <value><c>true</c> if [allow drag columns]; otherwise, <c>false</c>.</value>
        public bool AllowDragColumns
        {
            get
            {
                return (bool)this.GetValue(GridDataControl.AllowDragColumnsProperty);
            }

            set
            {
                this.SetValue(GridDataControl.AllowDragColumnsProperty, value);
            }
        }

        #endregion

        #region AllowEdit

        /// <summary>
        /// Gets / Sets if the grid is editable
        /// </summary>
        public bool AllowEdit
        {
            get { return (bool)GetValue(AllowEditProperty); }
            set { SetValue(AllowEditProperty, value); }
        }

        public static readonly DependencyProperty AllowEditProperty = DependencyProperty.Register("AllowEdit", typeof(bool), typeof(GridDataControl), new PropertyMetadata(true, OnAllowEditChanged));

        private static void OnAllowEditChanged(DependencyObject dpo, DependencyPropertyChangedEventArgs args)
        {
            var grid = dpo as GridDataControl;
            grid.TableProperties.AllowEdit = (bool)args.NewValue;
        }

        #endregion

        #region AllowExcelLikeResizing

        /// <summary>
        /// AllowExcelLikeResizing Dependency Property
        /// </summary>
        public static readonly DependencyProperty AllowExcelLikeResizingProperty =
            DependencyProperty.Register("AllowExcelLikeResizing", typeof(bool), typeof(GridDataControl),
                new PropertyMetadata((bool)true,
                    new PropertyChangedCallback(OnAllowExcelLikeResizingChanged)));

        /// <summary>
        /// Gets or sets the AllowExcelLikeResizing property. This dependency property 
        /// indicates whether hidden column/row resizing is allowed like in Excel.
        /// </summary>
        public bool AllowExcelLikeResizing
        {
            get { return (bool)GetValue(AllowExcelLikeResizingProperty); }
            set { SetValue(AllowExcelLikeResizingProperty, value); }
        }

        /// <summary>
        /// Handles changes to the AllowExcelLikeResizing property.
        /// </summary>
        private static void OnAllowExcelLikeResizingChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var gridDataControl = d as GridDataControl;
            gridDataControl.Model.Options.AllowExcelLikeResizing = (bool)e.NewValue;
        }

        #endregion

        #region ShowErrorIconOnEditing
        /// <summary>
        /// Gets / sets ErrorIcon at Editing time.
        /// </summary>
        public bool ShowErrorIconOnEditing
        {
            get { return (bool)GetValue(ShowErrorIconOnEditingProperty); }
            set { SetValue(ShowErrorIconOnEditingProperty, value); }
        }

        public static readonly DependencyProperty ShowErrorIconOnEditingProperty =
            DependencyProperty.Register("ShowErrorIconOnEditing", typeof(bool), typeof(GridDataControl),
                new PropertyMetadata((bool)true,
                    new PropertyChangedCallback(OnShowErrorIconOnEditingChanged)));

        private static void OnShowErrorIconOnEditingChanged(DependencyObject dpo, DependencyPropertyChangedEventArgs args)
        {
            var grid = dpo as GridDataControl;
            grid.Model.Options.ShowErrorIconOnEditing = (bool)args.NewValue;
        }

        #endregion

        /// <summary>
        /// Gets or sets a value for EnableRenderOptimization
        /// Setting true will optimize the scrolling performance of the GridDataControl
        /// </summary>

        public bool EnableRenderOptimization
        {
            get
            {
                return (bool)this.GetValue(GridDataControl.EnableRenderOptimizationProperty);
            }
            set
            {
                this.SetValue(GridDataControl.EnableRenderOptimizationProperty, value);
            }
        }


        /// <summary>
        /// DependencyPropery for <see cref = "GridDataControl.EnableRenderOptimizationProperty" />.
        /// </summary>
        public static readonly DependencyProperty EnableRenderOptimizationProperty = DependencyProperty.Register(
            "EnableRenderOptimization",
            typeof(bool),
            typeof(GridDataControl),
            new PropertyMetadata(false, OnEnableRenderOptimizationChanged));

        private bool isEnableRenderOptimizationSetBeforeLoaded = false;

        private static void OnEnableRenderOptimizationChanged(DependencyObject dpo, DependencyPropertyChangedEventArgs args)
        {
            var grid = dpo as GridDataControl;
            if (grid.isGridLoaded)
            {
                grid.InternalGrid.EnableRenderOptimization = (bool)args.NewValue;
            }
            else
            {
                grid.isEnableRenderOptimizationSetBeforeLoaded = true;
            }
        }

        public static readonly DependencyProperty IsViewLevelPagingProperty = DependencyProperty.Register(
    "IsViewLevelPaging",
    typeof(bool),
    typeof(GridDataControl),
    new PropertyMetadata(false, IsViewLevelPagingPropertyChanged));

        public bool IsViewLevelPaging
        {
            get
            {
                return (bool)this.GetValue(GridDataControl.IsViewLevelPagingProperty);
            }

            set
            {
                this.SetValue(GridDataControl.IsViewLevelPagingProperty, value);
            }
        }

        private static void IsViewLevelPagingPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs args)
        {
            var grid = d as GridDataControl;
            grid.TableProperties.IsViewLevelPaging = (bool)args.NewValue;

        }
        #region ClearMultiSelectionInNestedGrid Property
        
        public static readonly DependencyProperty ClearMultiSelectionInNestedGridProperty = 
            DependencyProperty.Register("ClearMultiSelectionInNestedGrid", typeof(bool), typeof(GridDataControl), new PropertyMetadata(false, ClearMultiSelectionInNestedGridChanged));

        public bool ClearMultiSelectionInNestedGrid
        {
            get
            {
                return (bool)this.GetValue(GridDataControl.ClearMultiSelectionInNestedGridProperty);
            }

            set
            {
                this.SetValue(GridDataControl.ClearMultiSelectionInNestedGridProperty, value);
            }
        }

        private static void ClearMultiSelectionInNestedGridChanged(DependencyObject d, DependencyPropertyChangedEventArgs args)
        {
            var grid = d as GridDataControl;
            grid.TableProperties.ClearMultiSelectionInNestedGrid = (bool)args.NewValue;

        }

        #endregion
        #region Seleced child model
        /// <summary>
        /// Gets / sets the selected child model for nestedGrid.
        /// </summary>
        public static readonly DependencyProperty SelectedChildModelProperty = DependencyProperty.Register(
          "SelectedChildModel", typeof(GridDataChildTableModel), typeof(GridDataControl), new PropertyMetadata(null, SelectedChildModelChanged));

        /// <summary>
        /// Gets or sets the selected child model.
        /// </summary>
        /// <value>The selected child model.</value>
        public GridDataChildTableModel SelectedChildModel
        {
            get
            {
                return (GridDataChildTableModel)GetValue(SelectedChildModelProperty);
            }
            internal set
            {
                SetValue(SelectedChildModelProperty, value);
            }
        }
        private static void SelectedChildModelChanged(DependencyObject dpo, DependencyPropertyChangedEventArgs args)
        {
            var grid = dpo as GridDataControl;
            if (args.OldValue != null)
            {
                GridDataChildTableModel oldChildGridModel = args.OldValue as GridDataChildTableModel;
                if (oldChildGridModel != null && oldChildGridModel.Grid != null)
                {
                    var nestedGrid = oldChildGridModel.Grid as GridDataCellNestedGridEditor;
                    if (nestedGrid != null)
                    {
                        nestedGrid.ClearChildGridSelections(oldChildGridModel);

                    }
                }
            }
            else if (args.NewValue != null)
            {
                if (grid.SelectedItems != null)
                    grid.SelectedItems.Clear();
                if (grid.SelectedItem != null)
                    grid.SelectedItem = null;
            }
        }
        #endregion

        #region HideEmptyChildGrid Property

        public static readonly DependencyProperty HideEmptyChildGridProperty =
            DependencyProperty.Register("HideEmptyChildGrid", typeof(bool), typeof(GridDataControl), new PropertyMetadata(false, HideEmptyChildGridChanged));

        public bool HideEmptyChildGrid
        {
            get
            {
                return (bool)this.GetValue(GridDataControl.HideEmptyChildGridProperty);
            }

            set
            {
                this.SetValue(GridDataControl.HideEmptyChildGridProperty, value);
            }
        }

        private static void HideEmptyChildGridChanged(DependencyObject d, DependencyPropertyChangedEventArgs args)
        {
            var grid = d as GridDataControl;
            grid.TableProperties.HideEmptyChildGrid = (bool)args.NewValue;
        }

        #endregion




        public static readonly DependencyProperty EnablePagingProperty = DependencyProperty.Register(
           "EnablePaging",
           typeof(bool),
           typeof(GridDataControl),
           new PropertyMetadata(false, EnablePagingPropertyChanged));

        public bool EnablePaging
        {
            get
            {
                return (bool)this.GetValue(GridDataControl.EnablePagingProperty);
            }

            set
            {
                this.SetValue(GridDataControl.EnablePagingProperty, value);
            }
        }

        private static void EnablePagingPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs args)
        {
            var grid = d as GridDataControl;
            grid.TableProperties.EnablePaging = (bool)args.NewValue;

        }


        #region AllowGroup

        /// <summary>
        /// DependencyProperty for <see cref = "GridDataControl.AllowGroup" />.
        /// </summary>
        public static readonly DependencyProperty AllowGroupProperty = DependencyProperty.Register(
            "AllowGroup",
            typeof(bool),
            typeof(GridDataControl),
            new PropertyMetadata(false, OnAllowGroupPropertyChanged));

        /// <summary>
        /// Gets or sets a value indicating whether [allow group].
        /// </summary>
        /// <value><c>true</c> if [allow group]; otherwise, <c>false</c>.</value>
        public bool AllowGroup
        {
            get
            {
                return (bool)this.GetValue(GridDataControl.AllowGroupProperty);
            }

            set
            {
                this.SetValue(GridDataControl.AllowGroupProperty, value);
            }
        }

        private bool isAllowGroupChangedBeforeGridLoaded = false;

        private static void OnAllowGroupPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs args)
        {
            var grid = d as GridDataControl;
            if (grid.isGridLoaded)
            {
                grid.TableProperties.AllowGroup = (bool)args.NewValue;
            }
            else
            {
                grid.isAllowGroupChangedBeforeGridLoaded = true;
            }
        }

        #endregion

        #region AllowNestedGridPadding

        /// <summary>
        /// DependencyProperty for <see cref = "GridDataControl.AllowSort" />.
        /// </summary>
        public static readonly DependencyProperty AllowNestedGridPaddingProperty = DependencyProperty.Register(
            "AllowNestedGridPadding",
            typeof(bool),
            typeof(GridDataControl),
            new PropertyMetadata(true, OnAllowNestedGridPaddingPropertyChanged));

        private bool _isAllowNestedGridPaddingChangedBeforeGridLoaded = false;

        private static void OnAllowNestedGridPaddingPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs args)
        {
            var grid = d as GridDataControl;
            if (grid.isGridLoaded)
            {
                grid.TableProperties.AllowNestedGridPadding = (bool)args.NewValue;
            }
            else
            {
                grid._isAllowNestedGridPaddingChangedBeforeGridLoaded = true;
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether [allow sort].
        /// </summary>
        /// <value><c>true</c> if [allow sort]; otherwise, <c>false</c>.</value>
        public bool AllowNestedGridPadding
        {
            get
            {
                return (bool)this.GetValue(GridDataControl.AllowNestedGridPaddingProperty);
            }

            set
            {
                this.SetValue(GridDataControl.AllowNestedGridPaddingProperty, value);
            }
        }

        #endregion

        #region AllowResizeColumns

        public static readonly DependencyProperty AllowResizeColumnsProperty = DependencyProperty.Register(
            "AllowResizeColumns",
            typeof(bool),
            typeof(GridDataControl),
            new PropertyMetadata(true, OnAllowResizeColumnsChanged));

        private bool isAllowResizeColumnsChangedBeforeLoaded = false;

        private static void OnAllowResizeColumnsChanged(DependencyObject d, DependencyPropertyChangedEventArgs args)
        {
            var grid = d as GridDataControl;
            if (grid.isGridLoaded)
            {
                grid.TableProperties.AllowResizeColumns = (bool)args.NewValue;
            }
            else
            {
                grid.isAllowResizeColumnsChangedBeforeLoaded = true;
            }
        }

        public bool AllowResizeColumns
        {
            get
            {
                return (bool)this.GetValue(GridDataControl.AllowResizeColumnsProperty);
            }

            set
            {
                this.SetValue(GridDataControl.AllowResizeColumnsProperty, value);
            }
        }

        #endregion

        #region AllowResizeRows

        public static readonly DependencyProperty AllowResizeRowsProperty = DependencyProperty.Register(
            "AllowResizeRows",
            typeof(bool),
            typeof(GridDataControl),
            new PropertyMetadata(true, OnAllowResizeRowsChanged));

        private bool isAllowResizeRowsChangedBeforeLoaded = false;

        private static void OnAllowResizeRowsChanged(DependencyObject d, DependencyPropertyChangedEventArgs args)
        {
            var grid = d as GridDataControl;
            if (grid.isGridLoaded)
            {
                grid.TableProperties.AllowResizeRows = (bool)args.NewValue;
            }
            else
            {
                grid.isAllowResizeRowsChangedBeforeLoaded = true;
            }
        }

        public bool AllowResizeRows
        {
            get
            {
                return (bool)this.GetValue(GridDataControl.AllowResizeRowsProperty);
            }

            set
            {
                this.SetValue(GridDataControl.AllowResizeRowsProperty, value);
            }
        }

        #endregion

        #region AllowSelection

        public static readonly DependencyProperty AllowSelectionProperty = DependencyProperty.Register("AllowSelection", typeof(GridSelectionFlags), typeof(GridDataControl), new PropertyMetadata(GridSelectionFlags.Any, OnAllowSelectionChanged));

        public GridSelectionFlags AllowSelection
        {
            get
            {
                return (GridSelectionFlags)GetValue(AllowSelectionProperty);
            }

            set
            {
                SetValue(AllowSelectionProperty, value);
            }
        }

        private static void OnAllowSelectionChanged(DependencyObject d, DependencyPropertyChangedEventArgs args)
        {
            var gridDataControl = d as GridDataControl;
            gridDataControl.Model.Options.AllowSelection = (GridSelectionFlags)args.NewValue;
        }

        #endregion

        #region AllowSort

        /// <summary>
        /// DependencyProperty for <see cref = "GridDataControl.AllowSort" />.
        /// </summary>
        public static readonly DependencyProperty AllowSortProperty = DependencyProperty.Register(
            "AllowSort",
            typeof(bool),
            typeof(GridDataControl),
            new PropertyMetadata(true, OnAllowSortPropertyChanged));

        private bool isAllowSortChangedBeforeGridLoaded = false;

        private static void OnAllowSortPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs args)
        {
            var grid = d as GridDataControl;
            if (grid.isGridLoaded)
            {
                grid.TableProperties.AllowSort = (bool)args.NewValue;
            }
            else
            {
                grid.isAllowSortChangedBeforeGridLoaded = true;
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
                return (bool)this.GetValue(GridDataControl.AllowSortProperty);
            }

            set
            {
                this.SetValue(GridDataControl.AllowSortProperty, value);
            }
        }

        #endregion

        #region AlternatingRowBackground

        /// <summary>
        /// DependencyProperty for <see cref = "GridDataControl.AlternatingRowBackground"/>.
        /// </summary>
        public static readonly DependencyProperty AlternatingRowBackgroundProperty = DependencyProperty.Register(
            "AlternatingRowBackground",
            typeof(Brush),
            typeof(GridDataControl),
            new PropertyMetadata(OnAlternatingRowBackgroundChanged));

        private static void OnAlternatingRowBackgroundChanged(DependencyObject d, DependencyPropertyChangedEventArgs args)
        {
            GridDataControl grid = d as GridDataControl;
            grid.TableProperties.AlternatingRowBackground = (Brush)args.NewValue;
        }

        /// <summary>
        /// Gets or sets the alternating row background.
        /// </summary>
        /// <value>The alternating row background.</value>
        public Brush AlternatingRowBackground
        {
            get
            {
                return this.GetValue(GridDataControl.AlternatingRowBackgroundProperty) as Brush;
            }

            set
            {
                this.SetValue(GridDataControl.AlternatingRowBackgroundProperty, value);
            }
        }

        #endregion

        #region AlternatingRowCount

        /// <summary>
        /// DependencyProperty for <see cref = "GridDataControl.AlternatingRowCount"/>.
        /// </summary>
        public static readonly DependencyProperty AlternatingRowCountProperty = DependencyProperty.Register(
            "AlternatingRowCount",
            typeof(int),
            typeof(GridDataControl),
            new PropertyMetadata(2, OnAlternatingRowCountPropertyChanged));

        private static void OnAlternatingRowCountPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs args)
        {
            GridDataControl grid = d as GridDataControl;
            grid.TableProperties.AlternatingRowCount = (int)args.NewValue;
        }

        /// <summary>
        /// Gets or sets the alternating row count.
        /// </summary>
        /// <value>The alternating row count.</value>
        public int AlternatingRowCount
        {
            get
            {
                return (int)this.GetValue(GridDataControl.AlternatingRowCountProperty);
            }

            set
            {
                this.SetValue(GridDataControl.AlternatingRowCountProperty, value);
            }
        }

        #endregion

        #region AutoPopulateRelations

        /// <summary>
        /// DependencyProperty for <see cref = "GridDataControl.AutoPopulateRelations"/>.
        /// </summary>
        public static readonly DependencyProperty AutoPopulateRelationsProperty = DependencyProperty.Register(
            "AutoPopulateRelations",
            typeof(bool),
            typeof(GridDataControl),
            new PropertyMetadata(true, OnAutoPopulateRelationsChanged));


        private static void OnAutoPopulateRelationsChanged(DependencyObject d, DependencyPropertyChangedEventArgs args)
        {
            GridDataControl grid = d as GridDataControl;
            grid.TableProperties.AutoPopulateRelations = (bool)args.NewValue;
        }

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
                return (bool)this.GetValue(GridDataControl.AutoPopulateRelationsProperty);
            }

            set
            {
                this.SetValue(GridDataControl.AutoPopulateRelationsProperty, value);
            }
        }

        #endregion AutoPopulateRelations

        #region AutoFocusCurrentItem

        /// <summary>
        /// Gets / Sets the AutoFocusCurrentItem property.
        /// </summary>
        public bool AutoFocusCurrentItem
        {
            get { return (bool)GetValue(AutoFocusCurrentItemProperty); }
            set { SetValue(AutoFocusCurrentItemProperty, value); }
        }

        public static readonly DependencyProperty AutoFocusCurrentItemProperty = DependencyProperty.Register("AutoFocusCurrentItem", typeof(bool), typeof(GridDataControl), new PropertyMetadata(true, OnAutoFocusCurrentItemChanged));

        private static void OnAutoFocusCurrentItemChanged(DependencyObject dpo, DependencyPropertyChangedEventArgs args)
        {
            var grid = dpo as GridDataControl;
            grid.TableProperties.AutoFocusCurrentItem = (bool)args.NewValue;
        }

        #endregion

        #region AutoPopulateColumns

        /// <summary>
        /// DependencyProperty for <see cref = "GridDataControl.AutoPopulateColumns"/>.
        /// </summary>
        public static readonly DependencyProperty AutoPopulateColumnsProperty = DependencyProperty.Register(
            "AutoPopulateColumns",
            typeof(bool),
            typeof(GridDataControl),
            new PropertyMetadata(true, OnAutoPopulateColumnsChanged));

        private static void OnAutoPopulateColumnsChanged(DependencyObject d, DependencyPropertyChangedEventArgs args)
        {
            GridDataControl grid = d as GridDataControl;
            grid.TableProperties.AutoPopulateColumns = (bool)args.NewValue;
        }

        /// <summary>
        /// Gets or sets a value indicating whether [auto populate columns].
        /// </summary>
        /// <value><c>true</c> if [auto populate columns]; otherwise, <c>false</c>.</value>
        public bool AutoPopulateColumns
        {
            get
            {
                return (bool)this.GetValue(GridDataControl.AutoPopulateColumnsProperty);
            }

            set
            {
                this.SetValue(GridDataControl.AutoPopulateColumnsProperty, value);
            }
        }

        #endregion

        #region AutoGenerateColumnsInfo
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
            DependencyProperty.Register("AutoGenerateColumnsInfo", typeof(bool), typeof(GridDataControl), new PropertyMetadata(false, OnAutoGenerateColumnsInfoChanged));

        private static void OnAutoGenerateColumnsInfoChanged(DependencyObject d, DependencyPropertyChangedEventArgs args)
        {
            GridDataControl grid = d as GridDataControl;
            grid.TableProperties.AutoGenerateColumnsInfo = (bool)args.NewValue;
        }
        #endregion

        #region CaptionSummaryRow

        private bool isCaptionSummaryRowChanged = false;

        private static void OnCaptionSummaryRowChanged(DependencyObject d, DependencyPropertyChangedEventArgs args)
        {
            var grid = d as GridDataControl;
            if (grid.isGridLoaded)
            {
                grid.TableProperties.CaptionSummaryRow = (GridDataSummaryRow)args.NewValue;
            }
            else
            {
                grid.isCaptionSummaryRowChanged = true;
            }
        }

        public GridDataSummaryRow CaptionSummaryRow
        {
            get
            {
                return (GridDataSummaryRow)this.GetValue(GridDataControl.CaptionSummaryRowProperty);
            }

            set
            {
                this.SetValue(GridDataControl.CaptionSummaryRowProperty, value);
            }
        }

        #endregion

        #region ClearAllOnItemSourceChange
        /// DependencyProperty for <see cref="GridDataControl.ClearAllOnItemSourceChange"/> property.
        /// </summary>
        public static readonly DependencyProperty ClearAllOnItemSourceChangeProperty = DependencyProperty.Register(
            "ClearAllOnItemSourceChange",
            typeof(bool),
            typeof(GridDataControl),
            new PropertyMetadata(false, OnClearAllOnItemsSourceChangeChanged));

        private static void OnClearAllOnItemsSourceChangeChanged(DependencyObject dpo, DependencyPropertyChangedEventArgs args)
        {
            var grid = dpo as GridDataControl;
            grid.TableProperties.ClearAllOnItemSourceChange = (bool)args.NewValue;
        }

        /// <summary>
        /// Gets or sets a value indicating whether ClearAllOnItemSource is true / false.
        /// </summary>
        /// <value>
        /// 	<c>true</c> if [clear all on item source change]; otherwise, <c>false</c>.
        /// </value>
        public bool ClearAllOnItemSourceChange
        {
            get
            {
                return (bool)this.GetValue(GridDataControl.ClearAllOnItemSourceChangeProperty);
            }
            set
            {
                this.SetValue(GridDataControl.ClearAllOnItemSourceChangeProperty, value);
            }
        }

        #endregion

        #region ColumnSizer
        public static readonly DependencyProperty ColumnSizerProperty = DependencyProperty.Register(
            "ColumnSizer",
            typeof(GridControlLengthUnitType),
            typeof(GridDataControl),
            new PropertyMetadata(OnColumnSizerChanged));

        private bool isColumnSizerChangedBeforeGridLoaded = false;

        private static void OnColumnSizerChanged(DependencyObject d, DependencyPropertyChangedEventArgs args)
        {
            var grid = d as GridDataControl;
            if (grid.isGridLoaded)
            {
                grid.TableProperties.ColumnSizer = (GridControlLengthUnitType)args.NewValue;
            }
            else
            {
                grid.isColumnSizerChangedBeforeGridLoaded = true;
            }
        }

        /// <summary>
        /// Gets or Sets the ColumnSizer property for GridDataControl
        /// </summary>
        public GridControlLengthUnitType ColumnSizer
        {
            get
            {
                return (GridControlLengthUnitType)this.GetValue(GridDataControl.ColumnSizerProperty);
            }
            set
            {
                this.SetValue(GridDataControl.ColumnSizerProperty, value);
            }
        }
        #endregion

        #region ConditionalFormats

        /// <summary>
        /// ConditionalFormats Dependency Property
        /// </summary>
        public static readonly DependencyProperty ConditionalFormatsProperty =
            DependencyProperty.Register("ConditionalFormats", typeof(ObservableCollection<GridDataConditionalFormat>), typeof(GridDataControl),
                new PropertyMetadata(new PropertyChangedCallback(OnConditionalFormatsChanged)));

        /// <summary>
        /// Gets or sets the ConditionalFormats property. This dependency property 
        /// indicates ConditionalFormats present in GridDataControl
        /// </summary>
        public ObservableCollection<GridDataConditionalFormat> ConditionalFormats
        {
            get { return (ObservableCollection<GridDataConditionalFormat>)GetValue(ConditionalFormatsProperty); }
            set { SetValue(ConditionalFormatsProperty, value); }
        }

        /// <summary>
        /// Handles changes to the ConditionalFormats property.
        /// </summary>
        private static void OnConditionalFormatsChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var grid = d as GridDataControl;
            grid.TableProperties.ConditionalFormats = (ObservableCollection<GridDataConditionalFormat>)e.NewValue;
        }

        //public ObservableCollection<GridDataConditionalFormat> ConditionalFormats
        //{
        //    get
        //    {
        //        return this.TableProperties.ConditionalFormats;
        //    }

        //    set
        //    {
        //        this.TableProperties.ConditionalFormats = value;
        //    }
        //}

        #endregion

        #region CustomGroupComparer (DependencyProperty)

        /// <summary>
        /// Defines the custom group comparer. This will work only when the grouped column is also sorted.
        /// </summary>
        public IComparer<Group> CustomGroupComparer
        {
            get { return (IComparer<Group>)GetValue(CustomGroupComparerProperty); }
            set { SetValue(CustomGroupComparerProperty, value); }
        }

        public static readonly DependencyProperty CustomGroupComparerProperty = DependencyProperty.Register("CustomGroupComparer", typeof(IComparer<Group>), typeof(GridDataControl), new PropertyMetadata(OnCustomGroupComparerPropertyChanged));

        private static void OnCustomGroupComparerPropertyChanged(DependencyObject dpo, DependencyPropertyChangedEventArgs args)
        {
            var grid = dpo as GridDataControl;
            grid.TableProperties.CustomGroupComparer = (IComparer<Group>)args.NewValue;
        }

        #endregion

        #region DefaultColumnWidth

        /// <summary>
        /// DependencyProperty for <see cref = "GridDataControl.DefaultColumnWidth"/>.
        /// </summary>
        public static readonly DependencyProperty DefaultColumnWidthProperty = DependencyProperty.Register(
            "DefaultColumnWidth",
            typeof(double),
            typeof(GridDataControl),
            new PropertyMetadata(150d, OnDefaultColumnWidthChanged));

        private static void OnDefaultColumnWidthChanged(DependencyObject d, DependencyPropertyChangedEventArgs args)
        {
            GridDataControl grid = d as GridDataControl;
            grid.TableProperties.DefaultColumnWidth = (double)args.NewValue;
        }

        /// <summary>
        /// Gets or sets the default width of the column.
        /// </summary>
        /// <value>The default width of the column.</value>
        public double DefaultColumnWidth
        {
            get
            {
                return (double)this.GetValue(GridDataControl.DefaultColumnWidthProperty);
            }

            set
            {
                this.SetValue(GridDataControl.DefaultColumnWidthProperty, value);
            }
        }

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

        public static readonly DependencyProperty DefaultFilterOperatorProperty = DependencyProperty.Register("DefaultFilterOperator", typeof(FilterOperatorType), typeof(GridDataControl), new PropertyMetadata(FilterOperatorType.StartsWith, OnDefaultFilterOperatorPropertyChanged));

        private static void OnDefaultFilterOperatorPropertyChanged(DependencyObject dpo, DependencyPropertyChangedEventArgs args)
            {
            var grid = dpo as GridDataControl;
            grid.TableProperties.DefaultFilterOperator = (FilterOperatorType)args.NewValue;
            }

        #endregion

        #region DefaultHeaderRowHeight

        /// <summary>
        /// DefaultHeaderRowHeight Dependency Property
        /// </summary>
        public static readonly DependencyProperty DefaultHeaderRowHeightProperty =
            DependencyProperty.Register("DefaultHeaderRowHeight", typeof(Double),
            typeof(GridDataControl),
            new PropertyMetadata(GridDataTableModel.HeaderRowHeight, OnDefaultHeaderRowHeightChanged));
        /// <summary>
        /// Gets or sets the DefaultHeaderRowHeight property. This dependency property 
        /// indicates DefaultHeaderRowHeight of HeaderCells.
        /// </summary>
        public Double DefaultHeaderRowHeight
        {
            get { return (Double)GetValue(DefaultHeaderRowHeightProperty); }
            set { SetValue(DefaultHeaderRowHeightProperty, value); }
        }

        /// <summary>
        /// Handles changes to the DefaultHeaderRowHeight property.
        /// </summary>
        private static void OnDefaultHeaderRowHeightChanged(DependencyObject d, DependencyPropertyChangedEventArgs args)
        {
            var grid = d as GridDataControl;
            grid.TableProperties.DefaultHeaderRowHeight = (double)args.NewValue;
        }

        #endregion

        #region DragIndicatorInnerBrush
        /// <summary>
        /// DependencyProperty for <see cref="GridDataControl.DragIndicatorInnerBrush"/>.
        /// </summary>
        public static readonly DependencyProperty DragIndicatorInnerBrushProperty = DependencyProperty.Register("DragIndicatorInnerBrush", typeof(Brush), typeof(GridDataControl), new PropertyMetadata(OnDragIndicatorInnerBrushChanged));

        private bool isDragIndicatorInnerBrushChangedBeforeGridLoaded = false;

        private static void OnDragIndicatorInnerBrushChanged(DependencyObject d, DependencyPropertyChangedEventArgs args)
        {
            var grid = d as GridDataControl;
            if (grid.isGridLoaded)
            {
                grid.TableProperties.DragIndicatorInnerBrush = (Brush)args.NewValue;
            }
            else
            {
                grid.isDragIndicatorInnerBrushChangedBeforeGridLoaded = true;
            }
        }

        /// <summary>
        /// Gets or sets the drag indicator inner brush.
        /// </summary>
        /// <value>The drag indicator inner brush.</value>
        public Brush DragIndicatorInnerBrush
        {
            get
            {
                return (Brush)this.GetValue(GridDataControl.DragIndicatorInnerBrushProperty);
            }

            set
            {
                this.SetValue(GridDataControl.DragIndicatorInnerBrushProperty, value);
            }
        }

        #endregion

        #region DragIndicatorOuterBrush
        /// <summary>
        /// DependencyProperty for <see cref="GridDataControl.DragIndicatorOuterBrush"/>.
        /// </summary>
        public static readonly DependencyProperty DragIndicatorOuterBrushProperty = DependencyProperty.Register("DragIndicatorOuterBrush", typeof(Brush), typeof(GridDataControl), new PropertyMetadata(OnDragIndicatorOuterBrushChanged));

        private bool isDragIndicatorOuterBrushChangedBeforeGridLoaded = false;

        private static void OnDragIndicatorOuterBrushChanged(DependencyObject d, DependencyPropertyChangedEventArgs args)
        {
            var grid = d as GridDataControl;
            if (grid.isGridLoaded)
            {
                grid.TableProperties.DragIndicatorOuterBrush = (Brush)args.NewValue;
            }
            else
            {
                grid.isDragIndicatorOuterBrushChangedBeforeGridLoaded = true;
            }
        }

        /// <summary>
        /// Gets or sets the drag indicator outer brush.
        /// </summary>
        /// <value>The drag indicator outer brush.</value>
        public Brush DragIndicatorOuterBrush
        {
            get
            {
                return (Brush)this.GetValue(GridDataControl.DragIndicatorOuterBrushProperty);
            }

            set
            {
                this.SetValue(GridDataControl.DragIndicatorOuterBrushProperty, value);
            }
        }

        #endregion

        #region DrawSelectionOptions

        public static readonly DependencyProperty DrawSelectionOptionsProperty = DependencyProperty.Register("DrawSelectionOptions", typeof(GridDrawSelectionOptions), typeof(GridDataControl), new PropertyMetadata(OnDrawSelectionOptionsChanged));

        public GridDrawSelectionOptions DrawSelectionOptions
        {
            get
            {
                return (GridDrawSelectionOptions)GetValue(DrawSelectionOptionsProperty);
            }

            set
            {
                SetValue(DrawSelectionOptionsProperty, value);
            }
        }

        private static void OnDrawSelectionOptionsChanged(DependencyObject d, DependencyPropertyChangedEventArgs args)
        {
            var gridDataControl = d as GridDataControl;
            gridDataControl.Model.Options.DrawSelectionOptions = (GridDrawSelectionOptions)args.NewValue;
        }

        #endregion

        #region ExcelLikeCurrentCell

        public static readonly DependencyProperty ExcelLikeCurrentCellProperty = DependencyProperty.Register("ExcelLikeCurrentCell", typeof(bool), typeof(GridDataControl), new PropertyMetadata(OnExcelLikeCurrentCellChanged));

        public bool ExcelLikeCurrentCell
        {
            get
            {
                return (bool)GetValue(ExcelLikeCurrentCellProperty);
            }

            set
            {
                SetValue(ExcelLikeCurrentCellProperty, value);
            }
        }

        private static void OnExcelLikeCurrentCellChanged(DependencyObject d, DependencyPropertyChangedEventArgs args)
        {
            var gridDataControl = d as GridDataControl;
            gridDataControl.Model.Options.ExcelLikeCurrentCell = (bool)args.NewValue;
        }

        #endregion

        #region StyleManager

        public GridDataStyleManager StyleManager
        {
            get { return (GridDataStyleManager)GetValue(StyleManagerProperty); }
            set { SetValue(StyleManagerProperty, value); }
        }

        // Using a DependencyProperty as the backing store for StyleManager.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty StyleManagerProperty =
            DependencyProperty.Register("StyleManager", typeof(GridDataStyleManager), typeof(GridDataControl), new PropertyMetadata(OnStyleManagerChanged));

        private bool isStyleManagerChangedBeforeLoaded = false;

        private static void OnStyleManagerChanged(DependencyObject d, DependencyPropertyChangedEventArgs args)
        {
            GridDataControl dataControl = d as GridDataControl;
            GridDataStyleManager styleManager = args.NewValue as GridDataStyleManager;
            if (styleManager != null)
            {
                styleManager.gridDataControl = dataControl;
                if (dataControl.isGridLoaded)
                {
                    dataControl.Model.TableProperties.StyleManager = styleManager;
                    if (dataControl.InternalGrid != null)
                        dataControl.InternalGrid.StyleManager = styleManager;
                }
                else
                {
                    dataControl.isStyleManagerChangedBeforeLoaded = true;
                }
            }
        }

        #endregion

        #region ExcelLikeSelectionFrame
        public static readonly DependencyProperty ExcelLikeSelectionFrameProperty = DependencyProperty.Register("ExcelLikeSelectionFrame", typeof(bool), typeof(GridDataControl), new PropertyMetadata(false, OnExcelLikeSelectionFrameChanged));

        public bool ExcelLikeSelectionFrame
        {
            get
            {
                return (bool)GetValue(ExcelLikeSelectionFrameProperty);
            }

            set
            {
                SetValue(ExcelLikeSelectionFrameProperty, value);
            }
        }

        private static void OnExcelLikeSelectionFrameChanged(DependencyObject d, DependencyPropertyChangedEventArgs args)
        {
            var gridDataControl = d as GridDataControl;
            gridDataControl.Model.Options.ExcelLikeSelectionFrame = (bool)args.NewValue;
        }

        #endregion

        #region EnableAdvanceExcelLikeFiltering

        public static readonly DependencyProperty EnableLegacyFilteringProperty =
           DependencyProperty.Register("EnableLegacyFiltering", typeof(bool), typeof(GridDataControl), new PropertyMetadata(false, OnEnableAdvanceExcelLikeFilteringChanged));

        public bool EnableLegacyFiltering
        {
            get
            {
                return (bool)this.GetValue(GridDataControl.EnableLegacyFilteringProperty);
            }
            set
            {
                this.SetValue(GridDataControl.EnableLegacyFilteringProperty, value);
            }
        }

        private static void OnEnableAdvanceExcelLikeFilteringChanged(DependencyObject d, DependencyPropertyChangedEventArgs args)
        {
            GridDataControl grid = d as GridDataControl;
            grid.TableProperties.EnableLegacyFiltering = (bool)args.NewValue;

        }

        #endregion

        #region EnableBlendStyling



        public bool EnableBlendStyling
        {
            get { return (bool)GetValue(EnableBlendStylingProperty); }
            set { SetValue(EnableBlendStylingProperty, value); }
        }

        // Using a DependencyProperty as the backing store for EnableBlendStyling.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty EnableBlendStylingProperty =
            DependencyProperty.Register("EnableBlendStyling", typeof(bool), typeof(GridDataControl), new PropertyMetadata(false, OnEnableBlendStylingChanged));

        private bool isEnableBlendStyleSetBeforeLoaded = false;

        private static void OnEnableBlendStylingChanged(DependencyObject dpo, DependencyPropertyChangedEventArgs args)
        {
            var grid = dpo as GridDataControl;
            if (grid.isGridLoaded)
            {
                grid.InternalGrid.EnableBlendStyling = (bool)args.NewValue;
            }
            else
            {
                grid.isEnableBlendStyleSetBeforeLoaded = true;
            }
        }

        #endregion

        #region FilterBarMode

        /// <summary>
        /// Dependency property for <see cref = "GridDataControl.FilterBarMode"/>.
        /// </summary>
        /// <value><c>Immediate</c> FilterBarMode; otherwise, <c>OnEnter</c>.</value>
        public static readonly DependencyProperty FilterBarModeProperty =
            DependencyProperty.Register("FilterBarMode", typeof(GridDataFilterBarMode), typeof(GridDataControl), new PropertyMetadata(GridDataFilterBarMode.Immediate, OnFilterBarModeChanged));

        private static void OnFilterBarModeChanged(DependencyObject d, DependencyPropertyChangedEventArgs args)
        {
            GridDataControl grid = d as GridDataControl;
            grid.TableProperties.FilterBarMode = (GridDataFilterBarMode)args.NewValue;
        }

        public GridDataFilterBarMode FilterBarMode
        {
            get { return (GridDataFilterBarMode)GetValue(FilterBarModeProperty); }
            set { SetValue(FilterBarModeProperty, value); }
        }

        #endregion

        #region FilterBehavior

        /// <summary>
        /// FilterBehavior Dependency Property
        /// </summary>
        public static readonly DependencyProperty FilterBehaviorProperty =
            DependencyProperty.Register("FilterBehavior", typeof(FilterBehavior), typeof(GridDataControl),
                new PropertyMetadata(FilterBehavior.StronglyTyped,
                    new PropertyChangedCallback(OnFilterBehaviorChanged)));

        /// <summary>
        /// Gets or sets the FilterBehavior property. This dependency property 
        /// indicates ....
        /// </summary>
        public FilterBehavior FilterBehavior
        {
            get { return (FilterBehavior)GetValue(FilterBehaviorProperty); }
            set { SetValue(FilterBehaviorProperty, value); }
        }

        /// <summary>
        /// Handles changes to the FilterBehavior property.
        /// </summary>
        private static void OnFilterBehaviorChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            GridDataControl grid = d as GridDataControl;
            grid.TableProperties.FilterBehavior = (FilterBehavior)e.NewValue;
        }

        #endregion

        #region FilterBarPredicateType (Dependency Property)

        public PredicateType FilterBarPredicateType
        {
            get { return (PredicateType)GetValue(FilterBarPredicateTypeProperty); }
            set { SetValue(FilterBarPredicateTypeProperty, value); }
        }

        public static readonly DependencyProperty FilterBarPredicateTypeProperty = DependencyProperty.Register("FilterBarPredicateType", typeof(PredicateType), typeof(GridDataControl), new PropertyMetadata(PredicateType.And, OnFilterBarPredicateTypePropertyChanged));

        private static void OnFilterBarPredicateTypePropertyChanged(DependencyObject dpo, DependencyPropertyChangedEventArgs args)
        {
            var grid = dpo as GridDataControl;
            grid.TableProperties.FilterBarPredicateType = (PredicateType)args.NewValue;
        }

        #endregion

        #region
        public bool EnableTriStateSorting
        {
            get { return (bool)GetValue(EnableTriStateSortingProperty); }
            set { SetValue(EnableTriStateSortingProperty, value); }
        }

        public static readonly DependencyProperty EnableTriStateSortingProperty = DependencyProperty.Register("EnableTriStateSorting", typeof(bool), typeof(GridDataControl), new PropertyMetadata(false, OnEnableTriStateSortingPropertyChanged));

        private static void OnEnableTriStateSortingPropertyChanged(DependencyObject dpo, DependencyPropertyChangedEventArgs args)
        {
            var grid = dpo as GridDataControl;
            grid.TableProperties.EnableTriStateSorting = (bool)args.NewValue;
        }
        #endregion

        #region FooterColumns

        public static readonly DependencyProperty FooterColumnsProperty = DependencyProperty.Register("FooterColumns", typeof(int), typeof(GridDataControl), new PropertyMetadata(0, OnFooterColumnsChanged));

        private static void OnFooterColumnsChanged(DependencyObject d, DependencyPropertyChangedEventArgs args)
        {
            var grid = d as GridDataControl;
            grid.TableProperties.FooterColumns = (int)args.NewValue;
        }

        public int FooterColumns
        {
            get
            {
                return (int)this.GetValue(GridDataControl.FooterColumnsProperty);
            }
            set
            {
                this.SetValue(GridDataControl.FooterColumnsProperty, value);
            }
        }

        #endregion

        #region FooterRows

        public static readonly DependencyProperty FooterRowsProperty = DependencyProperty.Register("FooterRows", typeof(int), typeof(GridDataControl), new PropertyMetadata(0, OnFooterRowsChanged));

        private static void OnFooterRowsChanged(DependencyObject d, DependencyPropertyChangedEventArgs args)
        {
            var grid = d as GridDataControl;
            grid.TableProperties.FooterRows = (int)args.NewValue;
        }

        public int FooterRows
        {
            get
            {
                return (int)this.GetValue(GridDataControl.FooterRowsProperty);
            }
            set
            {
                this.SetValue(GridDataControl.FooterRowsProperty, value);
            }
        }

        #endregion

        #region FrozenColumns

        /// <summary>
        /// Gets / Sets the FrozenColumns
        /// </summary>
        public int FrozenColumns
        {
            get { return (int)GetValue(FrozenColumnsProperty); }
            set { SetValue(FrozenColumnsProperty, value); }
        }

        public static readonly DependencyProperty FrozenColumnsProperty = DependencyProperty.Register("FrozenColumns", typeof(int), typeof(GridDataControl), new PropertyMetadata(0, OnFrozenColumnsChanged));

        private static void OnFrozenColumnsChanged(DependencyObject dpo, DependencyPropertyChangedEventArgs args)
        {
            var grid = dpo as GridDataControl;
            grid.TableProperties.FrozenColumns = (int)args.NewValue;
        }

        #endregion

        #region FrozenRows

        /// <summary>
        /// Gets / Sets FrozenRows.
        /// </summary>
        public int FrozenRows
        {
            get { return (int)GetValue(FrozenRowsProperty); }
            set { SetValue(FrozenRowsProperty, value); }
        }

        public static readonly DependencyProperty FrozenRowsProperty = DependencyProperty.Register("FrozenRows", typeof(int), typeof(GridDataControl), new PropertyMetadata(1, OnFrozenRowsChanged));

        private static void OnFrozenRowsChanged(DependencyObject dpo, DependencyPropertyChangedEventArgs args)
        {
            var grid = dpo as GridDataControl;
            grid.TableProperties.FrozenRows = (int)args.NewValue;
        }

        #endregion

        #region GroupDropAreaText

        public static readonly DependencyProperty GroupDropAreaTextProperty = DependencyProperty.Register(
            "GroupDropAreaText",
            typeof(string),
            typeof(GridDataControl),
            new PropertyMetadata(OnGroupDropAreaTextChanged));

        private bool isGroupDropAreaTextChanged = false;

        private static void OnGroupDropAreaTextChanged(DependencyObject d, DependencyPropertyChangedEventArgs args)
        {
            var grid = d as GridDataControl;
            if (grid.isGridLoaded)
            {
                if (grid.GroupDropAreaGrid != null && grid.ShowGroupDropArea)
                {
                    grid.GroupDropAreaGrid.Model.GroupDropAreaText = (string)args.NewValue;
                    grid.GroupDropAreaGrid.InvalidateCells();
                }
            }
            else
            {
                grid.isGroupDropAreaTextChanged = true;
            }
        }

        public string GroupDropAreaText
        {
            get
            {
                return (string)this.GetValue(GridDataControl.GroupDropAreaTextProperty);
            }

            set
            {
                this.SetValue(GridDataControl.GroupDropAreaTextProperty, value);
            }
        }

        #endregion

        #region GroupDropAreaHeight

        public static readonly DependencyProperty GroupDropAreaHeightProperty = DependencyProperty.Register(
            "GroupDropAreaHeight",
            typeof(double),
            typeof(GridDataControl),
            new PropertyMetadata((double)50, OnGroupDropAreaHeightChanged));

        private bool isGroupDropAreaHeightChanged = false;

        private static void OnGroupDropAreaHeightChanged(DependencyObject d, DependencyPropertyChangedEventArgs args)
        {
            var grid = d as GridDataControl;
            if (grid.isGridLoaded)
            {
                grid.TableProperties.GroupDropAreaHeight = (double)args.NewValue;
                grid.GroupDropAreaGrid.InvalidateCells();

            }
            else
                grid.isGroupDropAreaHeightChanged = true;

        }

        public double GroupDropAreaHeight
        {
            get
            {
                return (double)this.GetValue(GridDataControl.GroupDropAreaHeightProperty);
            }

            set
            {
                this.SetValue(GridDataControl.GroupDropAreaHeightProperty, value);
            }
        }

        #endregion

        #region GroupCaptionText

        public static readonly DependencyProperty GroupCaptionTextProperty = DependencyProperty.Register(
            "GroupCaptionText",
            typeof(string),
            typeof(GridDataControl),
            new PropertyMetadata(OnGroupCaptionTextChanged));

        private bool isGroupCaptionTextChanged = false;

        private static void OnGroupCaptionTextChanged(DependencyObject d, DependencyPropertyChangedEventArgs args)
        {
            var grid = d as GridDataControl;
            var value = args.NewValue.ToString();
            if (grid.isGridLoaded)
            {
                grid.TableProperties.GroupCaptionText = value != string.Empty ? value : GridDataControl.GroupCaptionConstant;
            }
            else
            {
                grid.isGroupCaptionTextChanged = true;
            }
        }

        /// <summary>
        /// Gets or sets the Group Caption Text. There are three properties that are
        /// necessary for the caption to display, 
        /// <para></para>
        /// <list type="bullet">
        /// <item>
        /// <description>Name - Defines the Name of the group.</description></item>
        /// <item>
        /// <description>Count - Defines the group count.</description></item>
        /// <item>
        /// <description>ColumnName - Defines the name of the column that is
        /// grouped.</description></item></list>
        /// <para></para>
        /// <para>Default value of the GroupCaptionText is, {ColumnName} : {Name} - {Count}
        /// Items. If the ShowGroupSummaryInCaption is set, this value would be overridden by 
        /// the CaptionSummaryRow.Title.</para>
        /// </summary>
        [System.ComponentModel.TypeConverter(typeof(GridDataFormatConverter))]
        public string GroupCaptionText
        {
            get
            {
                return (string)this.GetValue(GridDataControl.GroupCaptionTextProperty);
            }

            set
            {
                this.SetValue(GridDataControl.GroupCaptionTextProperty, value);
            }
        }

        #endregion

        #region GroupedColumns

        /// <summary>
        /// GroupedColumns Dependency Property
        /// </summary>
        public static readonly DependencyProperty GroupedColumnsProperty =
            DependencyProperty.Register("GroupedColumns", typeof(ObservableCollection<GridDataGroupColumn>), typeof(GridDataControl),
                new PropertyMetadata(new PropertyChangedCallback(OnGroupedColumnsChanged)));

        /// <summary>
        /// Gets or sets the GroupedColumns property. This dependency property 
        /// indicates Grouped Columns present in GridDataControl.
        /// </summary>
        public ObservableCollection<GridDataGroupColumn> GroupedColumns
        {
            get { return (ObservableCollection<GridDataGroupColumn>)GetValue(GroupedColumnsProperty); }
            set { SetValue(GroupedColumnsProperty, value); }
        }

        /// <summary>
        /// Handles changes to the GroupedColumns property.
        /// </summary>
        private static void OnGroupedColumnsChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var grid = d as GridDataControl;
            grid.TableProperties.GroupedColumns = (ObservableCollection<GridDataGroupColumn>)e.NewValue;
        }

        //public ObservableCollection<GridDataGroupColumn> GroupedColumns
        //{
        //    get
        //    {
        //        return this.TableProperties.GroupedColumns;
        //    }
        //    set
        //    {
        //        this.TableProperties.GroupedColumns = value;
        //    }
        //}

        #endregion

        #region HeaderColumns

        public static readonly DependencyProperty HeaderColumnsProperty = DependencyProperty.Register("HeaderColumns", typeof(int), typeof(GridDataControl), new PropertyMetadata(0, OnHeaderColumnsChanged));

        private static void OnHeaderColumnsChanged(DependencyObject d, DependencyPropertyChangedEventArgs args)
        {
            var grid = d as GridDataControl;
            grid.TableProperties.HeaderColumns = (int)args.NewValue;
        }

        public int HeaderColumns
        {
            get
            {
                return (int)this.GetValue(GridDataControl.HeaderColumnsProperty);
            }
            set
            {
                this.SetValue(GridDataControl.HeaderColumnsProperty, value);
            }
        }

        #endregion

        #region HeaderRows
        public static readonly DependencyProperty HeaderRowsProperty = DependencyProperty.Register("HeaderRows", typeof(int), typeof(GridDataControl), new PropertyMetadata(1, OnHeaderRowsChanged));

        private static void OnHeaderRowsChanged(DependencyObject d, DependencyPropertyChangedEventArgs args)
        {
            var grid = d as GridDataControl;
            grid.TableProperties.HeaderRows = (int)args.NewValue;
        }

        public int HeaderRows
        {
            get
            {
                return (int)this.GetValue(GridDataControl.HeaderRowsProperty);
            }
            set
            {
                this.SetValue(GridDataControl.HeaderRowsProperty, value);
            }
        }

        #endregion

        #region HideColumnsWhenGrouped

        /// <summary>
        /// Gets / Sets HideColumnsWhenGrouped property.
        /// </summary>
        public bool HideColumnsWhenGrouped
        {
            get { return (bool)GetValue(HideColumnsWhenGroupedProperty); }
            set { SetValue(HideColumnsWhenGroupedProperty, value); }
        }

        public static readonly DependencyProperty HideColumnsWhenGroupedProperty = DependencyProperty.Register("HideColumnsWhenGrouped", typeof(bool), typeof(GridDataControl), new PropertyMetadata(false, OnHideColumnsWhenGroupedChanged));

        private static void OnHideColumnsWhenGroupedChanged(DependencyObject dpo, DependencyPropertyChangedEventArgs args)
        {
            var grid = dpo as GridDataControl;
            grid.TableProperties.HideColumnsWhenGrouped = (bool)args.NewValue;
        }

        #endregion

        #region HighlightSelectionAlphaBlend

        public static readonly DependencyProperty HighlightSelectionAlphaBlendProperty = DependencyProperty.Register("HighlightSelectionAlphaBlend", typeof(Brush), typeof(GridDataControl), new PropertyMetadata(OnHighlightSelectionAlphaBlendChanged));

        private static void OnHighlightSelectionAlphaBlendChanged(DependencyObject d, DependencyPropertyChangedEventArgs args)
        {
            var gridDataControl = d as GridDataControl;
            gridDataControl.Model.Options.HighlightSelectionAlphaBlend = (Brush)args.NewValue;
        }

        public Brush HighlightSelectionAlphaBlend
        {
            get
            {
                return (Brush)GetValue(HighlightSelectionAlphaBlendProperty);
            }

            set
            {
                SetValue(HighlightSelectionAlphaBlendProperty, value);
            }
        }

        #endregion

        #region HighlightSelectionBackground

        public static readonly DependencyProperty HighlightSelectionBackgroundProperty = DependencyProperty.Register("HighlightSelectionBackground", typeof(Brush), typeof(GridDataControl), new PropertyMetadata(OnHighlightSelectionBackgroundChanged));

        public Brush HighlightSelectionBackground
        {
            get
            {
                return (Brush)GetValue(HighlightSelectionBackgroundProperty);
            }

            set
            {
                SetValue(HighlightSelectionBackgroundProperty, value);
            }
        }

        private static void OnHighlightSelectionBackgroundChanged(DependencyObject d, DependencyPropertyChangedEventArgs args)
        {
            var gridDataControl = d as GridDataControl;
            gridDataControl.Model.Options.HighlightSelectionBackground = (Brush)args.NewValue;
        }

        #endregion

        #region HighlightSelectionBorder

        public static readonly DependencyProperty HighlightSelectionBorderProperty = DependencyProperty.Register("HighlightSelectionBorder", typeof(Brush), typeof(GridDataControl), new PropertyMetadata(OnHighlightSelectionBorderChanged));

        public Brush HighlightSelectionBorder
        {
            get
            {
                return (Brush)GetValue(HighlightSelectionBorderProperty);
            }

            set
            {
                SetValue(HighlightSelectionBorderProperty, value);
            }
        }

        private static void OnHighlightSelectionBorderChanged(DependencyObject d, DependencyPropertyChangedEventArgs args)
        {
            var gridDataControl = d as GridDataControl;
            gridDataControl.Model.Options.HighlightSelectionBorder = (Brush)args.NewValue;
        }

        #endregion

        #region HighlightSelectionBorderWidth

        public static readonly DependencyProperty HighlightSelectionBorderWidthProperty = DependencyProperty.Register("HighlightSelectionBorderWidth", typeof(double), typeof(GridDataControl), new PropertyMetadata(OnHighlightSelectionBorderWidthChanged));

        public double HighlightSelectionBorderWidth
        {
            get
            {
                return (double)GetValue(HighlightSelectionBorderWidthProperty);
            }

            set
            {
                SetValue(HighlightSelectionBorderWidthProperty, value);
            }
        }

        private static void OnHighlightSelectionBorderWidthChanged(DependencyObject d, DependencyPropertyChangedEventArgs args)
        {
            var gridDataControl = d as GridDataControl;
            gridDataControl.Model.Options.HighlightSelectionBorderWidth = (double)args.NewValue;
        }

        #endregion

        #region HighlightSelectionForeground
        public static readonly DependencyProperty HighlightSelectionForegroundProperty = DependencyProperty.Register("HighlightSelectionForeground", typeof(Brush), typeof(GridDataControl), new PropertyMetadata(OnHighlightSelectionForegroundChanged));

        public Brush HighlightSelectionForeground
        {
            get
            {
                return (Brush)GetValue(HighlightSelectionForegroundProperty);
            }

            set
            {
                SetValue(HighlightSelectionForegroundProperty, value);
            }
        }

        private static void OnHighlightSelectionForegroundChanged(DependencyObject d, DependencyPropertyChangedEventArgs args)
        {
            var gridDataControl = d as GridDataControl;
            gridDataControl.Model.Options.HighlightSelectionForeground = (Brush)args.NewValue;
        }

        #endregion

        #region HeaderStyle


        public Style HeaderStyle
        {
            get { return (Style)GetValue(HeaderStyleProperty); }
            set { SetValue(HeaderStyleProperty, value); }
        }

        // Using a DependencyProperty as the backing store for HeaderStyle.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty HeaderStyleProperty =
            DependencyProperty.Register("HeaderStyle", typeof(Style), typeof(GridDataControl), new PropertyMetadata(OnHeaderStyleChanged));

        private bool isHeaderStyleSetBeforeGridLoaded = false;
        private static void OnHeaderStyleChanged(DependencyObject d, DependencyPropertyChangedEventArgs args)
        {
            var gridDataControl = d as GridDataControl;
            if (gridDataControl.isGridLoaded)
            {
                gridDataControl.InternalGrid.HeaderStyle = args.NewValue != null ? (Style)args.NewValue : null;
                gridDataControl.Model.TableProperties.HeaderStyle = args.NewValue != null ? (Style)args.NewValue : null;
            }
            else
            {
                gridDataControl.isHeaderStyleSetBeforeGridLoaded = true;
            }
        }



        #endregion

        #region ItemsSource

        private bool isItemsSourceLoadedBeforeGridLoaded = false;

        /// <summary>
        /// DependencyProperty for <see cref = "GridDataControl.ItemsSource"/>.
        /// </summary>
        public static readonly DependencyProperty ItemsSourceProperty = DependencyProperty.Register(
            "ItemsSource",
            typeof(object),
            typeof(GridDataControl),
            new PropertyMetadata(OnItemsSourceChanged));

        private static void OnItemsSourceChanged(DependencyObject d, DependencyPropertyChangedEventArgs args)
        {
            GridDataControl grid = d as GridDataControl;
            if (grid.isGridLoaded)
            {
                grid.TableProperties.ItemsSource = args.NewValue;
                grid.OnItemsSourceChanged(new SyncfusionRoutedEventArgs());
            }
            else
            {
                grid.isItemsSourceLoadedBeforeGridLoaded = true;
            }
        }

        /// <summary>
        /// Gets or sets the items source.
        /// </summary>
        /// <value>The items source.</value>
        public object ItemsSource
        {
            get
            {
                return this.GetValue(GridDataControl.ItemsSourceProperty);
            }

            set
            {
                this.SetValue(GridDataControl.ItemsSourceProperty, value);
            }
        }

#if SyncfusionFramework4_0

        #region IsDynamicItemsSource

        /// <summary>
        /// Gets / Sets the IsDynamicItemsSource property. Set this to true if ItemsSource is a collection of 'dynamic' objects. Default value is false.
        /// </summary>
        public bool IsDynamicItemsSource
        {
            get { return (bool)GetValue(IsDynamicItemsSourceProperty); }
            set { SetValue(IsDynamicItemsSourceProperty, value); }
        }

        public static readonly DependencyProperty IsDynamicItemsSourceProperty = DependencyProperty.Register("IsDynamicItemsSource", typeof(bool), typeof(GridDataControl), new PropertyMetadata(false, OnIsDynamicItemsSourceChanged));

        private static void OnIsDynamicItemsSourceChanged(DependencyObject dpo, DependencyPropertyChangedEventArgs args)
        {
            var grid = dpo as GridDataControl;
            grid.TableProperties.IsDynamicItemsSource = (bool)args.NewValue;
        }

        #endregion

#endif


        #endregion

        #region IsGroupsExpanded

        public static readonly DependencyProperty IsGroupsExpandedProperty = DependencyProperty.Register(
            "IsGroupsExpanded",
            typeof(bool),
            typeof(GridDataControl),
            new PropertyMetadata(false, OnIsGroupsExpandedChanged));

        private bool isGroupsExpandedSetBeforeLoad = false;

        private static void OnIsGroupsExpandedChanged(DependencyObject d, DependencyPropertyChangedEventArgs args)
        {
            var grid = d as GridDataControl;
            if (grid.isGridLoaded)
            {
                var value = (bool)args.NewValue;
                grid.TableProperties.IsGroupsExpanded = value;
            }
            else
            {
                grid.isGroupsExpandedSetBeforeLoad = true;
            }
        }

        public bool IsGroupsExpanded
        {
            get
            {
                return (bool)this.GetValue(GridDataControl.IsGroupsExpandedProperty);
            }

            set
            {
                this.SetValue(GridDataControl.IsGroupsExpandedProperty, value);
            }
        }

        #endregion

        #region ListboxSelectionMode

        public static readonly DependencyProperty ListBoxSelectionModeProperty = DependencyProperty.Register("ListBoxSelectionMode", typeof(GridSelectionMode), typeof(GridDataControl), new PropertyMetadata(GridSelectionMode.One, OnListBoxSelectionModeChanged));

        public GridSelectionMode ListBoxSelectionMode
        {
            get
            {
                return (GridSelectionMode)GetValue(ListBoxSelectionModeProperty);
            }

            set
            {
                SetValue(ListBoxSelectionModeProperty, value);
            }
        }

        private static void OnListBoxSelectionModeChanged(DependencyObject d, DependencyPropertyChangedEventArgs args)
        {
            var gridDataControl = d as GridDataControl;
            gridDataControl.Model.Options.ListBoxSelectionMode = (GridSelectionMode)args.NewValue;
        }

        #endregion

        #region NotifyPropertyChanges

        public static readonly DependencyProperty NotifyPropertyChangesProperty = DependencyProperty.Register(
            "NotifyPropertyChanges",
            typeof(bool),
            typeof(GridDataControl),
            new PropertyMetadata(false, OnNotifyPropertyChanged));

        private bool isNotifyPropertyChangedSetBeforeGridLoaded = false;
        private static void OnNotifyPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs args)
        {
            var grid = d as GridDataControl;
            if (grid.isGridLoaded)
            {
                grid.TableProperties.NotifyPropertyChanges = (bool)args.NewValue;
            }
            else
            {
                grid.isNotifyPropertyChangedSetBeforeGridLoaded = true;
            }
        }

        public bool NotifyPropertyChanges
        {
            get
            {
                return (bool)this.GetValue(GridDataControl.NotifyPropertyChangesProperty);
            }

            set
            {
                this.SetValue(GridDataControl.NotifyPropertyChangesProperty, value);
            }
        }

        #endregion

        #region Relations

        /// <summary>
        /// Relations Dependency Property
        /// </summary>
        public static readonly DependencyProperty RelationsProperty =
            DependencyProperty.Register("Relations", typeof(ObservableCollection<GridDataRelation>), typeof(GridDataControl),
                new PropertyMetadata(new PropertyChangedCallback(OnRelationsChanged)));

        /// <summary>
        /// Gets or sets the Relations property. This dependency property 
        /// indicates Relations present in GridDataControl.
        /// </summary>
        public ObservableCollection<GridDataRelation> Relations
        {
            get { return (ObservableCollection<GridDataRelation>)GetValue(RelationsProperty); }
            set { SetValue(RelationsProperty, value); }
        }

        /// <summary>
        /// Handles changes to the Relations property.
        /// </summary>
        private static void OnRelationsChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var grid = d as GridDataControl;
            grid.TableProperties.Relations = (ObservableCollection<GridDataRelation>)e.NewValue;
        }

        ///// <summary>
        ///// Gets or sets the relations.
        ///// </summary>
        ///// <value>The relations.</value>
        //public ObservableCollection<GridDataRelation> Relations
        //{
        //    get
        //    {
        //        return this.TableProperties.Relations;
        //    }

        //    set
        //    {
        //        this.TableProperties.Relations = value;
        //    }
        //}

        #endregion

        #region ReserveSpaceForIcons

        /// <summary>
        /// ReserveSpaceForIcons Dependency Property
        /// </summary>
        public static readonly DependencyProperty ReserveSpaceForIconsProperty =
            DependencyProperty.Register("ReserveSpaceForIcons", typeof(bool), typeof(GridDataControl),
                new PropertyMetadata(false, OnReserveSpaceForIconsChanged));

        //private bool isReserveSpaceForIconsChangedBeforeGridLoaded = false;
        private static void OnReserveSpaceForIconsChanged(DependencyObject d, DependencyPropertyChangedEventArgs args)
        {
            var grid = d as GridDataControl;
            grid.TableProperties.ReserveSpaceForIcons = (bool)args.NewValue;
        }

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

        #region RowHeaderWidth

        /// <summary>
        /// Gets / sets row header width
        /// </summary>
        public double RowHeaderWidth
        {
            get { return (double)GetValue(RowHeaderWidthProperty); }
            set { SetValue(RowHeaderWidthProperty, value); }
        }

        public static readonly DependencyProperty RowHeaderWidthProperty = DependencyProperty.Register("RowHeaderWidth", typeof(double), typeof(GridDataControl), new PropertyMetadata(GridDataTableModel.ExpandCollapseCellWidth, OnRowHeaderWidthChanged));

        private static void OnRowHeaderWidthChanged(DependencyObject dpo, DependencyPropertyChangedEventArgs args)
        {
            var grid = dpo as GridDataControl;
            grid.TableProperties.RowHeaderWidth = (double)args.NewValue;
        }

        #endregion

        #region RowBackground
        /// <summary>
        /// DependencyProperty for <see cref = "GridDataControl.RowBackground"/>.
        /// </summary>
        public static readonly DependencyProperty RowBackgroundProperty = DependencyProperty.Register(
            "RowBackground",
            typeof(Brush),
            typeof(GridDataControl),
            new PropertyMetadata(OnRowBackgroundChanged));

        private static void OnRowBackgroundChanged(DependencyObject d, DependencyPropertyChangedEventArgs args)
        {
            GridDataControl grid = d as GridDataControl;
            grid.TableProperties.RowBackground = (Brush)args.NewValue;
        }

        /// <summary>
        /// Gets or sets the row background.
        /// </summary>
        /// <value>The row background.</value>
        public Brush RowBackground
        {
            get
            {
                return this.GetValue(GridDataControl.RowBackgroundProperty) as Brush;
            }

            set
            {
                this.SetValue(GridDataControl.RowBackgroundProperty, value);
            }
        }

        #endregion

        #region SelectedItem

        private bool innerSelectionChange = false;

        private void OnCurrentRecordSelectionChanged(object sender, GridDataCurrentRecordSelectionChangedEventArgs args)
        {
            this.innerSelectionChange = true;
            var view = this.Model.View;
            var record = args.NewIndex > -1 && args.NewIndex < this.Model.SourceListCount ? view.Records[args.NewIndex] : null;

            if (this.Model.Table.HasGroups)
            {
                if (args.Record != null)
                {
                    record = args.Record as RecordEntry;
                }
            }

            if (record != null)
            {
                this.SelectedItem = ((RecordEntry)record).Data;
            }
        
            else
            {
                this.SelectedItem = null;
            }
            this.innerSelectionChange = false;
        }

        public static readonly DependencyProperty SelectedItemProperty = DependencyProperty.Register(
            "SelectedItem",
            typeof(object),
            typeof(GridDataControl),
            new PropertyMetadata(null, OnSelectedItemChanged));

        private static void OnSelectedItemChanged(DependencyObject d, DependencyPropertyChangedEventArgs args)
        {
            var grid = d as GridDataControl;
            grid.OnSelectedItemChanged(args.NewValue);
            grid.oldSelectedItem = args.NewValue;
        }
        private bool selectedItemChangedFlag = false;
        private bool isSelectedItemSetBeforeLoaded = false;
        private void OnSelectedItemChanged(object record)
        {
            if (this.Model.View == null)
            {
                this.isSelectedItemSetBeforeLoaded = true;
                return;
            }

            if (!this.innerSelectionChange)
            {
                this.Model.View.MoveCurrentTo(record);
            }
            if (!this.Model.IsInSort && !this.Model.IsInGroup && !this.Model.IsInFilter)
                ResetSelectedItems();
            this.selectedItemChangedFlag = true;
        }

        public object SelectedItem
        {
            get
            {
                return (object)this.GetValue(GridDataControl.SelectedItemProperty);
            }

            set
            {
                this.SetValue(GridDataControl.SelectedItemProperty, value);
            }
        }

        #region SelectedItems
        public ObservableCollection<object> SelectedItems { get; private set; }
        private bool resetSelectedItems = false;
        //private void ResetSelectedItems()
        //{
        //    this.resetSelectedItems = true;
        //    this.SelectedItems.Clear();
        //    Dictionary<int, object> dictionary = new Dictionary<int, object>();
        //    GridDataTableModel tableModel = this.Model as GridDataTableModel;
        //    GridRangeInfoList rangeList = this.Model.SelectedRanges;
        //    foreach (GridRangeInfo range in this.Model.SelectedRanges)
        //    {
        //        for (int index = range.Top; index <= range.Bottom; index++)
        //        {
        //            int recordIndex = tableModel.ResolveIndexToRecordPosition(index);
        //            object record = null;
        //            if (!this.Model.Table.HasGroups)
        //            {
        //                if (this.Model.View != null && this.Model.View.Records != null)
        //                {
        //                    if (recordIndex < this.Model.View.Records.Count)
        //                    {
        //                        if (recordIndex > -1)
        //                            record = this.Model.View.Records.GetItemAt(recordIndex);
        //                    }
        //                }
        //            }
        //            else
        //            {
        //                if (this.Model.View.TopLevelGroup.DisplayElements.Count >= recordIndex)
        //                {
        //                    var recordEntry = this.Model.View.TopLevelGroup.DisplayElements[recordIndex] as RecordEntry;
        //                    if (recordEntry != null)
        //                        record = recordEntry.Data;
        //                }
        //            }
        //            if (record != null)
        //            {
        //                if (!dictionary.ContainsKey(recordIndex))
        //                    dictionary.Add(recordIndex, record);
        //            }
        //        }
        //    }

        //    foreach (var record in dictionary.Values)
        //    {
        //        SelectedItems.Add(record);
        //    }
        //    dictionary.Clear();
        //    this.resetSelectedItems = false;
        //}

        private object oldSelectedItem;
        

        private void ResetSelectedItems()
        {
            this.resetSelectedItems = true;
            this.innerSelectionChange = true;

            var oldSelectedItem = this.SelectedItem != null ? this.SelectedItem : null;
            List<object> removedItems = new List<object>();

            foreach (var rec in this.SelectedItems)
            {
                removedItems.Add(rec);
            }
           
            this.SelectedItems.Clear();
            Dictionary<int, object> dictionary = new Dictionary<int, object>();
            GridDataTableModel tableModel = this.Model as GridDataTableModel;
            
            foreach (GridRangeInfo range in this.Model.SelectedRanges)
            {
                GridRangeInfo internalrange = range;
                if (internalrange.RangeType == GridRangeInfoType.Table)
                    internalrange = range.ExpandRange(range.Top, range.Left, Model.RowCount, Model.ColumnCount);
                for (int index = internalrange.Top; index <= internalrange.Bottom; index++)
                {
                    int recordIndex = tableModel.ResolveIndexToRecordPosition(index);
                    object record = null;
                    if (!this.Model.Table.HasGroups)
                    {
                        if (this.Model.View != null && this.Model.View.Records != null)
                        {
                            if (recordIndex < this.Model.View.Records.Count)
                            {
                                if (recordIndex > -1)
                                    record = this.Model.View.Records.GetItemAt(recordIndex);
                            }
                        }
                    }
                    else
                    {
                        if (this.Model.View.TopLevelGroup.DisplayElements.Count >= recordIndex)
                        {
                            var recordEntry = this.Model.View.TopLevelGroup.DisplayElements[recordIndex] as RecordEntry;
                            if (recordEntry != null)
                                record = recordEntry.Data;
                        }
                    }
                    if (record != null)
                    {
                        if (!dictionary.ContainsKey(recordIndex))
                            dictionary.Add(recordIndex, record);
                    }
                }
            }
            if (!this.Model.IsInFilter)
            {
                foreach (var record in dictionary.Values)
                {                    
                    SelectedItems.Add(record);                   
                }
                
                dictionary.Clear();
            }
            resetSelectedItems = false;            

            #region  Raise the RaiseRecordsSelectionChanged event.

            List<object> addedItems = new List<object>();
            if (!this.Model.IsInFilter)
            {
                foreach (var rec in this.SelectedItems)
                {
                    addedItems.Add(rec);
                }

                foreach (var rec in this.SelectedItems)
                {
                    if (removedItems.Contains(rec))
                    {
                        removedItems.Remove(rec);
                    }
                }

                if (this.SelectedItems.Count == 0 && this.Model.Grid != null && this.Model.CurrencyManager.IsGroupCaptionCell)
                {
                    this.SelectedItem = null;
                }
            }
            var r = removedItems.ToList();

            if (this.Model.IsInFilter && !this.Model.CurrencyManager.IsInFilterBarRow)
            {
                this.Model.SelectedRanges.Clear();
                if(this.SelectedItem!=null)
                    this.SelectedItem = null;
                this.SelectedItems.Clear();
                addedItems.Clear();
                removedItems.Clear();
                foreach (var record in r)
                {
                    if (this.Model.View.Contains(record))
                    {
                        this.SelectedItems.Add(record);
                        addedItems.Add(record);
                    }
                    else
                    {
                        removedItems.Add(record);
                    }
                }

                int itemCount = this.SelectedItems.Count;
                if (itemCount > 0)
                {
                    this.SelectedItem = this.SelectedItems[itemCount - 1];
                }
                else
                {
                    if (this.SelectedItem != null)
                        this.SelectedItem = null;
                }
            }

            this.innerSelectionChange = false;

            if (this.ListBoxSelectionMode != GridSelectionMode.None && (removedItems.Count > 0 || addedItems.Count > 0))
                this.Model.Table.RaiseRecordsSelectionChanged(new GridDataRecordsSelectionChangedEventArgs(removedItems, addedItems));

            #endregion
        }
        #endregion

        #endregion

        #region ScrollViewerStyle


        public Style ScrollViewerStyle
        {
            get { return (Style)GetValue(ScrollViewerStyleProperty); }
            set { SetValue(ScrollViewerStyleProperty, value); }
        }

        // Using a DependencyProperty as the backing store for ScrollViewerStyle.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty ScrollViewerStyleProperty =
            DependencyProperty.Register("ScrollViewerStyle", typeof(Style), typeof(GridDataControl), new PropertyMetadata(null));



        #endregion

        #region ShowAddNewRow

        public static readonly DependencyProperty ShowAddNewRowProperty = DependencyProperty.Register("ShowAddNewRow", typeof(bool), typeof(GridDataControl), new PropertyMetadata(true, OnShowAddNewRowChanged));

        private static void OnShowAddNewRowChanged(DependencyObject d, DependencyPropertyChangedEventArgs args)
        {
            var grid = d as GridDataControl;
            grid.TableProperties.ShowAddNewRow = (bool)args.NewValue;
        }

        public bool ShowAddNewRow
        {
            get
            {
                return (bool)this.GetValue(GridDataControl.ShowAddNewRowProperty);
            }

            set
            {
                this.SetValue(GridDataControl.ShowAddNewRowProperty, value);
            }
        }

        #endregion

        #region ShowColumnOptionsProperty
        /* public static readonly DependencyProperty ShowColumnOptionsProperty = DependencyProperty.Register(
            "ShowColumnOptions",
            typeof(bool),
            typeof(GridDataControl),
            new PropertyMetadata(OnShowColumnOptionsPropertyChanged));

         private bool isShowColumnOptionsPropertyChangedBeforeGridLoaded = false;
         private static void OnShowColumnOptionsPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs args)
         {
             var grid = d as GridDataControl;
             if (grid.isGridLoaded)
             {
                 grid.TableProperties.ShowColumnOptions = (bool)args.NewValue;
             }
             else
             {
                 grid.isShowColumnOptionsPropertyChangedBeforeGridLoaded = true;
             }
         }

         public bool ShowColumnOptions
         {
             get
             {
                 return (bool)this.GetValue(GridDataControl.ShowColumnOptionsProperty);
             }

             set
             {
                 this.SetValue(GridDataControl.ShowColumnOptionsProperty, value);
             }
         }*/
        #endregion

        #region ShowCurrentCell

        public static readonly DependencyProperty ShowCurrentCellProperty = DependencyProperty.Register("ShowCurrentCell", typeof(bool), typeof(GridDataControl), new PropertyMetadata(true, OnShowCurrentCellChanged));

        public bool ShowCurrentCell
        {
            get
            {
                return (bool)GetValue(ShowCurrentCellProperty);
            }

            set
            {
                SetValue(ShowCurrentCellProperty, value);
            }
        }

        private static void OnShowCurrentCellChanged(DependencyObject d, DependencyPropertyChangedEventArgs args)
        {
            var gridDataControl = d as GridDataControl;
            gridDataControl.Model.Options.ShowCurrentCell = (bool)args.NewValue;
        }

        #endregion

        #region ShowErrorToolTips
        /// <summary>
        /// DependencyProperty for <see cref="GridDataControl.ShowErrorTooltips"/> property.
        /// </summary>
        public static readonly DependencyProperty ShowErrorTooltipsProperty = DependencyProperty.Register("ShowErrorTooltips", typeof(bool), typeof(GridDataControl), new PropertyMetadata(true, OnShowErrorTooltipsPropertyChanged));

        private bool isShowErrorTooltipLoadedBeforeGrid = false;

        private static void OnShowErrorTooltipsPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs args)
        {
            var grid = d as GridDataControl;
            if (grid.isGridLoaded)
            {
                var value = (bool)args.NewValue;
                GridTooltipService.SetShowErrorTooltips(grid.InternalGrid, value);
            }
            else
            {
                grid.isShowErrorTooltipLoadedBeforeGrid = true;
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether [show error tooltips].
        /// </summary>
        /// <value><c>true</c> if [show error tooltips]; otherwise, <c>false</c>.</value>
        public bool ShowErrorTooltips
        {
            get
            {
                return (bool)this.GetValue(GridDataControl.ShowErrorTooltipsProperty);
            }

            set
            {
                this.SetValue(GridDataControl.ShowErrorTooltipsProperty, value);
            }
        }
        #endregion

        #region ShowFilterBar

        /// <summary>
        /// Dependency property for <see cref = "GridDataControl.ShowFilterBar"/>.
        /// </summary>
        /// <value><c>true</c> if [show Filterbar]; otherwise, <c>false</c>.</value>
        public static readonly DependencyProperty ShowFilterBarProperty =
            DependencyProperty.Register("ShowFilterBar", typeof(bool), typeof(GridDataControl), new PropertyMetadata(false, OnShowFilterBarChanged));

        private static void OnShowFilterBarChanged(DependencyObject d, DependencyPropertyChangedEventArgs args)
        {
            GridDataControl grid = d as GridDataControl;
            grid.TableProperties.ShowFilterBar = (bool)args.NewValue;
        }

        public bool ShowFilterBar
        {
            get { return (bool)GetValue(ShowFilterBarProperty); }
            set { SetValue(ShowFilterBarProperty, value); }
        }

        #endregion

        #region ShowFilters

        public static readonly DependencyProperty ShowFiltersProperty = DependencyProperty.Register(
            "ShowFilters",
            typeof(bool),
            typeof(GridDataControl),
            new PropertyMetadata(OnShowFiltersPropertyChanged));

        private bool isShowFiltersChangedBeforeGridLoaded = false;

        private static void OnShowFiltersPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs args)
        {
            var grid = d as GridDataControl;
            if (grid.isGridLoaded)
            {
                grid.TableProperties.ShowFilters = (bool)args.NewValue;
            }
            else
            {
                grid.isShowFiltersChangedBeforeGridLoaded = true;
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether [show filters].
        /// </summary>
        /// <value><c>true</c> if [show filters]; otherwise, <c>false</c>.</value>
        public bool ShowFilters
        {
            get
            {
                return (bool)this.GetValue(GridDataControl.ShowFiltersProperty);
            }

            set
            {
                this.SetValue(GridDataControl.ShowFiltersProperty, value);
            }
        }

        #endregion

        #region ShowGroupCaptionPlusMinus

        public static readonly DependencyProperty ShowGroupCaptionPlusMinusProperty = DependencyProperty.Register(
            "ShowGroupCaptionPlusMinus",
            typeof(bool),
            typeof(GridDataControl),
            new PropertyMetadata(true, OnShowGroupCaptionPlusMinus));

        private static void OnShowGroupCaptionPlusMinus(DependencyObject d, DependencyPropertyChangedEventArgs args)
        {
            var grid = d as GridDataControl;
            grid.TableProperties.ShowGroupCaptionPlusMinus = (bool)args.NewValue;
        }

        public bool ShowGroupCaptionPlusMinus
        {
            get
            {
                return (bool)this.GetValue(GridDataControl.ShowGroupCaptionPlusMinusProperty);
            }

            set
            {
                this.SetValue(GridDataControl.ShowGroupCaptionPlusMinusProperty, value);
            }
        }

        #endregion

        #region ShowGroupDropArea

        public static readonly DependencyProperty ShowGroupDropAreaProperty = DependencyProperty.Register(
            "ShowGroupDropArea",
            typeof(bool),
            typeof(GridDataControl),
            new PropertyMetadata(false, OnShowGroupDropAreaChanged));

        private bool isShowGroupDropAreaChangedBeforeGridLoaded = false;

        private static void OnShowGroupDropAreaChanged(DependencyObject d, DependencyPropertyChangedEventArgs args)
        {
            var grid = d as GridDataControl;

            if (grid.isGridLoaded)
            {
                grid.TableProperties.ShowGroupDropArea = (bool)args.NewValue;
            }
            else
            {
                grid.isShowGroupDropAreaChangedBeforeGridLoaded = true;
            }
        }

        public bool ShowGroupDropArea
        {
            get
            {
                return (bool)this.GetValue(GridDataControl.ShowGroupDropAreaProperty);
            }

            set
            {
                this.SetValue(GridDataControl.ShowGroupDropAreaProperty, value);
            }
        }

        #endregion

        #region ShowGroupIndicators

        public static readonly DependencyProperty ShowGroupIndicatorsProperty = DependencyProperty.Register("ShowGroupIndicators", typeof(bool), typeof(GridDataControl), new PropertyMetadata(false, OnShowGroupIndicatorsChanged));

        private bool isShowGroupIndicatorsChangedBeforeLoaded = false;

        private static void OnShowGroupIndicatorsChanged(DependencyObject dpo, DependencyPropertyChangedEventArgs args)
        {
            var grid = dpo as GridDataControl;
            if (!grid.isGridLoaded)
            {
                grid.isShowGroupIndicatorsChangedBeforeLoaded = true;
            }
            else
            {
                grid.TableProperties.ShowGroupIndicators = (bool)args.NewValue;
            }
        }

        public bool ShowGroupIndicators
        {
            get
            {
                return (bool)this.GetValue(GridDataControl.ShowGroupIndicatorsProperty);
            }

            set
            {
                this.SetValue(GridDataControl.ShowGroupIndicatorsProperty, value);
            }
        }

        #endregion

        #region ShowGroupSummaries

        public static readonly DependencyProperty ShowGroupSummariesProperty = DependencyProperty.Register(
            "ShowGroupSummaries",
            typeof(bool),
            typeof(GridDataControl),
            new PropertyMetadata(true, OnShowGroupSummariesChanged));

        private bool isShowGroupSummariesChanged = false;

        private static void OnShowGroupSummariesChanged(DependencyObject d, DependencyPropertyChangedEventArgs args)
        {
            var grid = d as GridDataControl;
            if (grid.isGridLoaded)
            {
                grid.TableProperties.ShowGroupSummaries = (bool)args.NewValue;
            }
            else
            {
                grid.isShowGroupSummariesChanged = true;
            }
        }

        public bool ShowGroupSummaries
        {
            get
            {
                return (bool)this.GetValue(GridDataControl.ShowGroupSummariesProperty);
            }

            set
            {
                this.SetValue(GridDataControl.ShowGroupSummariesProperty, value);
            }
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
            DependencyProperty.Register("EnableLegacyStyle", typeof(bool), typeof(GridDataControl), new PropertyMetadata(false, OnEnableLegacyStylechanged));


        private bool isEnableLegacyStyleChanged = false;

        private static void OnEnableLegacyStylechanged(DependencyObject d, DependencyPropertyChangedEventArgs args)
        {
            var grid = d as GridDataControl;
            if (grid.isGridLoaded)
            {
                grid.TableProperties.EnableLegacyStyle = (bool)args.NewValue;
            }
            else
            {
                grid.isEnableLegacyStyleChanged = true;
            }
        }



        #endregion

        #region ShowGroupSummaryInCaption
        public static readonly DependencyProperty ShowGroupSummaryInCaptionProperty = DependencyProperty.Register(
            "ShowGroupSummaryInCaption",
            typeof(bool),
            typeof(GridDataControl),
            new PropertyMetadata(OnShowGroupSummaryInCaptionChanged));

        private bool isShowGroupSummaryInCaptionLoaded = false;

        private static void OnShowGroupSummaryInCaptionChanged(DependencyObject d, DependencyPropertyChangedEventArgs args)
        {
            var grid = d as GridDataControl;
            if (grid.isGridLoaded)
            {
                grid.TableProperties.ShowGroupSummaryInCaption = (bool)args.NewValue;
            }
            else
            {
                grid.isShowGroupSummaryInCaptionLoaded = true;
            }
        }

        public bool ShowGroupSummaryInCaption
        {
            get
            {
                return (bool)this.GetValue(GridDataControl.ShowGroupSummaryInCaptionProperty);
            }

            set
            {
                this.SetValue(GridDataControl.ShowGroupSummaryInCaptionProperty, value);
            }
        }
        #endregion

        #region ShowRowHeader
        /// <summary>
        /// DependencyProperty for <see cref = "GridDataControl.ShowRowHeader"/>.
        /// </summary>
        public static readonly DependencyProperty ShowRowHeaderProperty = DependencyProperty.Register(
            "ShowRowHeader",
            typeof(bool),
            typeof(GridDataControl),
            new PropertyMetadata(false, OnShowRowHeaderChanged));

        private bool isShowRowHeaderChangedBeforeGridLoaded = false;

        private static void OnShowRowHeaderChanged(DependencyObject d, DependencyPropertyChangedEventArgs args)
        {
            GridDataControl grid = d as GridDataControl;
            if (grid.isGridLoaded)
            {
                grid.TableProperties.ShowRowHeader = (bool)args.NewValue;
            }
            else
            {
                grid.isShowRowHeaderChangedBeforeGridLoaded = true;
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
                return (bool)this.GetValue(GridDataControl.ShowRowHeaderProperty);
            }

            set
            {
                this.SetValue(GridDataControl.ShowRowHeaderProperty, value);
            }
        }

        #endregion

        #region ShowRowHeaderArrow

        /// <summary>
        /// Gets / Sets the ShowRowHeaderArrow property
        /// </summary>
        public bool ShowRowHeaderArrow
        {
            get { return (bool)GetValue(ShowRowHeaderArrowProperty); }
            set { SetValue(ShowRowHeaderArrowProperty, value); }
        }

        public static readonly DependencyProperty ShowRowHeaderArrowProperty = DependencyProperty.Register("ShowRowHeaderArrow", typeof(bool), typeof(GridDataControl), new PropertyMetadata(true, OnShowRowHeaderArrowChanged));

        private static void OnShowRowHeaderArrowChanged(DependencyObject dpo, DependencyPropertyChangedEventArgs args)
        {
            var grid = dpo as GridDataControl;
            grid.TableProperties.ShowRowHeaderArrow = (bool)args.NewValue;
        }

        #endregion      

        #endregion           

        #region ShowToolTips
        /// <summary>
        /// DependencyProperty for <see cref="GridDataControl.ShowTooltips"/> property.
        /// </summary>
        public static readonly DependencyProperty ShowTooltipsProperty = DependencyProperty.Register("ShowTooltips", typeof(bool), typeof(GridDataControl), new PropertyMetadata(OnShowTooltipsChanged));

        private bool isShowTooltipsChangedBeforeGridLoaded = false;

        private static void OnShowTooltipsChanged(DependencyObject d, DependencyPropertyChangedEventArgs args)
        {
            var grid = d as GridDataControl;
            if (grid.isGridLoaded)
            {
                var value = (bool)args.NewValue;
                //grid.TableProperties.ShowTooltips = value;
                GridTooltipService.SetShowTooltips(grid.InternalGrid, value);
            }
            else
            {
                grid.isShowTooltipsChangedBeforeGridLoaded = true;
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether ShowTooltips is true / false.
        /// </summary>
        /// <value><c>true</c> if [show tooltips]; otherwise, <c>false</c>.</value>
        public bool ShowTooltips
        {
            get
            {
                return (bool)this.GetValue(GridDataControl.ShowTooltipsProperty);
            }

            set
            {
                this.SetValue(GridDataControl.ShowTooltipsProperty, value);
            }
        }


        public static readonly DependencyProperty CaptionSummaryRowProperty = DependencyProperty.Register(
            "CaptionSummaryRow",
            typeof(GridDataSummaryRow),
            typeof(GridDataControl),
            new PropertyMetadata(OnCaptionSummaryRowChanged));
        #endregion

        #region ShowTableSummaries

        public static readonly DependencyProperty ShowTableSummariesProperty = DependencyProperty.Register(
            "ShowTableSummaries",
            typeof(bool),
            typeof(GridDataControl),
            new PropertyMetadata(true, OnShowTableSummariesChanged));

        private static void OnShowTableSummariesChanged(DependencyObject d, DependencyPropertyChangedEventArgs args)
        {
            var grid = d as GridDataControl;
            grid.TableProperties.ShowTableSummaries = (bool)args.NewValue;
        }

        public bool ShowTableSummaries
        {
            get
            {
                return (bool)this.GetValue(GridDataControl.ShowTableSummariesProperty);
            }

            set
            {
                this.SetValue(GridDataControl.ShowTableSummariesProperty, value);
            }
        }

        #endregion

        #region SortColumns

        /// <summary>
        /// SortColumns Dependency Property
        /// </summary>
        public static readonly DependencyProperty SortColumnsProperty =
            DependencyProperty.Register("SortColumns", typeof(ObservableCollection<GridDataSortColumn>), typeof(GridDataControl),
                new PropertyMetadata(new PropertyChangedCallback(OnSortColumnsChanged)));

        /// <summary>
        /// Gets or sets the SortColumns property. This dependency property 
        /// indicates SortColumns Present in GridDataControl
        /// </summary>
        public ObservableCollection<GridDataSortColumn> SortColumns
        {
            get { return (ObservableCollection<GridDataSortColumn>)GetValue(SortColumnsProperty); }
            set { SetValue(SortColumnsProperty, value); }
        }

        /// <summary>
        /// Handles changes to the SortColumns property.
        /// </summary>
        private static void OnSortColumnsChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var grid = d as GridDataControl;
            grid.TableProperties.SortColumns = (ObservableCollection<GridDataSortColumn>)e.NewValue;
        }

        //public ObservableCollection<GridDataSortColumn> SortColumns
        //{
        //    get
        //    {
        //        return this.TableProperties.SortColumns;
        //    }
        //    set
        //    {
        //        this.TableProperties.SortColumns = value;
        //    }
        //}

        #endregion

        #region SortWhenGrouped

        /// <summary>
        /// SortWhenGrouped Dependency Property
        /// </summary>
        public static readonly DependencyProperty SortWhenGroupedProperty =
            DependencyProperty.Register("SortWhenGrouped", typeof(bool), typeof(GridDataControl),
                new PropertyMetadata((bool)true,
                    new PropertyChangedCallback(OnSortWhenGroupedChanged)));

        /// <summary>
        /// Gets or sets the SortWhenGrouped property. This dependency property 
        /// indicates ....
        /// </summary>
        public bool SortWhenGrouped
        {
            get { return (bool)GetValue(SortWhenGroupedProperty); }
            set { SetValue(SortWhenGroupedProperty, value); }
        }

        /// Handles changes to the SortWhenGrouped property.
        /// </summary>
        private static void OnSortWhenGroupedChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var grid = d as GridDataControl;
            grid.TableProperties.SortWhenGrouped = (bool)e.NewValue;
        }

        #endregion

        #region StackedHeaderRows

        /// <summary>
        /// GridDataStackedHeaderRow Dependency Property
        /// </summary>
        public static readonly DependencyProperty GridDataStackedHeaderRowProperty =
            DependencyProperty.Register("StackedHeaderRows", typeof(ObservableCollection<GridDataStackedHeaderRow>), typeof(GridDataControl),
                new PropertyMetadata(new PropertyChangedCallback(OnGridDataStackedHeaderRowChanged)));

        /// <summary>
        /// Gets or sets the GridDataStackedHeaderRow property. This dependency property 
        /// indicates StackedHeaderRows present in GridDataControl.
        /// </summary>
        public ObservableCollection<GridDataStackedHeaderRow> StackedHeaderRows
        {
            get { return (ObservableCollection<GridDataStackedHeaderRow>)GetValue(GridDataStackedHeaderRowProperty); }
            set { SetValue(GridDataStackedHeaderRowProperty, value); }
        }

        /// <summary>
        /// Handles changes to the GridDataStackedHeaderRow property.
        /// </summary>
        private static void OnGridDataStackedHeaderRowChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var grid = d as GridDataControl;
            grid.TableProperties.StackedHeaderRows = (ObservableCollection<GridDataStackedHeaderRow>)e.NewValue;
        }

        //public ObservableCollection<GridDataStackedHeaderRow> StackedHeaderRows
        //{
        //    get
        //    {
        //        return this.TableProperties.StackedHeaderRows;
        //    }

        //    set
        //    {
        //        this.TableProperties.StackedHeaderRows = value;
        //    }
        //}

        #endregion

        #region ShowFilterStatusMessage

        public bool ShowFilterStatusMessage
        {
            get { return (bool)GetValue(ShowFilterStatusMessageProperty); }
            set { SetValue(ShowFilterStatusMessageProperty, value); }
        }

        // Using a DependencyProperty as the backing store for ShowFilterStatusMessage.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty ShowFilterStatusMessageProperty =
            DependencyProperty.Register("ShowFilterStatusMessage", typeof(bool), typeof(GridDataControl), new PropertyMetadata(true, OnStatusBarMessageChanged));

        public string StatusBarMessage
        {
            get { return (string)GetValue(GridDataControl.StatusBarMessageProperty); }
            set { SetValue(GridDataControl.StatusBarMessageProperty, value); }
        }

        // Using a DependencyProperty as the backing store for StatusBarMessage.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty StatusBarMessageProperty =
            DependencyProperty.Register("StatusBarMessage", typeof(string), typeof(GridDataControl), new PropertyMetadata("", OnStatusBarMessageChanged));

        private static void OnStatusBarMessageChanged(DependencyObject sender, DependencyPropertyChangedEventArgs args)
        {
            var gdc = (GridDataControl)sender;
            gdc.ShowHideStatusBar();
        }

        private void ShowHideStatusBar()
        {
            if (PART_BorderStatusBar != null)
            {
                //var value = GridDataControl.ResolveVisualStyleToSkinStorage(this.VisualStyle);
                //SkinStorage.SetVisualStyle(PART_BorderStatusBar, value);

                if (StatusBarMessage != null && StatusBarMessage != string.Empty && this.ShowFilterStatusMessage == true) // && this.ShowFilterStatusMessage != null Since expression is always true
                    PART_BorderStatusBar.Height = StatusBarHeight;
                else
                    PART_BorderStatusBar.Height = 0;
            }
        }

        #endregion

        #region SummaryRows

        /// <summary>
        /// SummaryRows Dependency Property
        /// </summary>
        public static readonly DependencyProperty SummaryRowsProperty =
            DependencyProperty.Register("SummaryRows", typeof(ObservableCollection<GridDataSummaryRow>), typeof(GridDataControl),
                new PropertyMetadata(new PropertyChangedCallback(OnSummaryRowsChanged)));

        /// <summary>
        /// Gets or sets the SummaryRows property. This dependency property 
        /// indicates SummaryRows present in GridDataControl.
        /// </summary>
        public ObservableCollection<GridDataSummaryRow> SummaryRows
        {
            get { return (ObservableCollection<GridDataSummaryRow>)GetValue(SummaryRowsProperty); }
            set { SetValue(SummaryRowsProperty, value); }
        }

        /// <summary>
        /// Handles changes to the SummaryRows property.
        /// </summary>
        private static void OnSummaryRowsChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var grid = d as GridDataControl;
            grid.TableProperties.SummaryRows = (ObservableCollection<GridDataSummaryRow>)e.NewValue;
        }

        //public ObservableCollection<GridDataSummaryRow> SummaryRows
        //{
        //    get
        //    {
        //        return this.TableProperties.SummaryRows;
        //    }

        //    set
        //    {
        //        this.TableProperties.SummaryRows = value;
        //    }
        //}

        #endregion

        #region TableSummaryRows

        /// <summary>
        /// TableSummaryRows Dependency Property
        /// </summary>
        public static readonly DependencyProperty TableSummaryRowsProperty =
            DependencyProperty.Register("TableSummaryRows", typeof(ObservableCollection<GridDataSummaryRow>), typeof(GridDataControl),
                new PropertyMetadata(new PropertyChangedCallback(OnTableSummaryRowsChanged)));

        /// <summary>
        /// Gets or sets the TableSummaryRows property. This dependency property 
        /// indicates TableSummaryRows present in GridDataControl.
        /// </summary>
        public ObservableCollection<GridDataSummaryRow> TableSummaryRows
        {
            get { return (ObservableCollection<GridDataSummaryRow>)GetValue(TableSummaryRowsProperty); }
            set { SetValue(TableSummaryRowsProperty, value); }
        }

        /// <summary>
        /// Handles changes to the TableSummaryRows property.
        /// </summary>
        private static void OnTableSummaryRowsChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var grid = d as GridDataControl;
            grid.TableProperties.TableSummaryRows = (ObservableCollection<GridDataSummaryRow>)e.NewValue;
        }

        //public ObservableCollection<GridDataSummaryRow> TableSummaryRows
        //{
        //    get
        //    {
        //        return this.TableProperties.TableSummaryRows;
        //    }

        //    set
        //    {
        //        this.TableProperties.TableSummaryRows = value;
        //    }
        //}

        #endregion

        #region UpdateMode

        public static readonly DependencyProperty UpdateModeProperty = DependencyProperty.Register(
            "UpdateMode",
            typeof(UpdateMode),
            typeof(GridDataControl),
            new PropertyMetadata(UpdateMode.LostFocus, OnUpdateModeChanged));

        private bool isUpdateModeChangedBeforeGridLoaded = false;
        private static void OnUpdateModeChanged(DependencyObject d, DependencyPropertyChangedEventArgs args)
        {
            var grid = d as GridDataControl;
            if (grid.isGridLoaded)
            {
                grid.TableProperties.UpdateMode = (UpdateMode)args.NewValue;
            }
            else
            {
                grid.isUpdateModeChangedBeforeGridLoaded = true;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        public UpdateMode UpdateMode
        {
            get
            {
                return (UpdateMode)this.GetValue(GridDataControl.UpdateModeProperty);
            }

            set
            {
                this.SetValue(GridDataControl.UpdateModeProperty, value);
            }
        }

        #endregion

        #region VisualStyle
        /// <summary>
        /// DependencyProperty for <see cref = "GridDataControl.VisualStyle"/>.
        /// </summary>
        public static readonly DependencyProperty VisualStyleProperty = DependencyProperty.Register(
            "VisualStyle",
            typeof(VisualStyle),
            typeof(GridDataControl),
            new PropertyMetadata(VisualStyle.Default, OnVisualStyleChanged));

        private bool isVisualStyleChangedBeforeGridLoaded = false;

        private static void OnVisualStyleChanged(DependencyObject d, DependencyPropertyChangedEventArgs args)
        {
            GridDataControl grid = d as GridDataControl;

#if !SILVERLIGHT
            var visualStyle = SkinStorage.GetVisualStyle(d);
            var value = ((VisualStyle)args.NewValue).ToString();
            if (visualStyle != value)
            {
                // we just ensure that the DO has the same visual style with SkinStorage
                SkinStorage.SetVisualStyle(d, value);
            }
#endif

            if (grid.isGridLoaded)
            {
                grid.TableProperties.CustomVisualStyle = grid.CustomVisualStyle;
                grid.TableProperties.VisualStyle = (VisualStyle)args.NewValue;
            }
            else
            {
                grid.isVisualStyleChangedBeforeGridLoaded = true;
            }
        }

        /// <summary>
        /// Gets or sets the visual style.
        /// </summary>
        /// <value>The visual style.</value>
        public VisualStyle VisualStyle
        {
            get
            {
                return (VisualStyle)this.GetValue(GridDataControl.VisualStyleProperty);
            }

            set
            {
                this.SetValue(GridDataControl.VisualStyleProperty, value);
            }
        }

        #region Custom Visual Style

        /// <summary>
        /// DependencyProperty for <see cref = "GridDataControl.CustomVisualStyle"/>.
        /// </summary>
        public static readonly DependencyProperty CustomVisualStyleProperty = DependencyProperty.Register(
            "CustomVisualStyle",
            typeof(IGridDataVisualStyle),
            typeof(GridDataControl),
            new PropertyMetadata(null));

        /// <summary>
        /// Gets or sets the custom visual style.
        /// </summary>
        /// <value>The custom visual style.</value>
        public IGridDataVisualStyle CustomVisualStyle
        {
            get
            {
                return (IGridDataVisualStyle)this.GetValue(GridDataControl.CustomVisualStyleProperty);
            }

            set
            {
                this.SetValue(GridDataControl.CustomVisualStyleProperty, value);
            }
        }

        #endregion

        #endregion

        #region VisibleColumns

        ///// <summary>
        ///// VisibleColumns Dependency Property
        ///// </summary>
        //public static readonly DependencyProperty VisibleColumnsProperty =
        //    DependencyProperty.Register("VisibleColumns", typeof(GridDataVisibleColumns), typeof(GridDataControl),
        //        new PropertyMetadata(new PropertyChangedCallback(OnVisibleColumnsChanged)));

        ///// <summary>
        ///// Gets or sets the VisibleColumns property. This dependency property 
        ///// indicates VisibleColumns present in GridDataControl.
        ///// </summary>
        //public GridDataVisibleColumns VisibleColumns
        //{
        //    get { return (GridDataVisibleColumns)GetValue(VisibleColumnsProperty); }
        //    set { SetValue(VisibleColumnsProperty, value); }
        //}

        ///// <summary>
        ///// Handles changes to the VisibleColumns property.
        ///// </summary>
        //private static void OnVisibleColumnsChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        //{
        //    var grid = d as GridDataControl;
        //    grid.TableProperties.VisibleColumns = (GridDataVisibleColumns)e.NewValue;
        //}


        /// <summary>
        /// Gets or sets the visible columns.
        /// </summary>
        /// <value>The visible columns.</value>
        public GridDataVisibleColumns VisibleColumns
        {
            get
            {
                return this.TableProperties.VisibleColumns;
            }

            set
            {
                this.TableProperties.VisibleColumns = value;
            }
        }

        #endregion

        #region WrapCell

        public static readonly DependencyProperty WrapCellProperty = DependencyProperty.Register("WrapCell", typeof(bool), typeof(GridDataControl), new PropertyMetadata(true, OnWrapCellChanged));

        /// <summary>
        /// 
        /// </summary>
        public bool WrapCell
        {
            get
            {
                return (bool)GetValue(WrapCellProperty);
            }

            set
            {
                SetValue(WrapCellProperty, value);
            }
        }

        private static void OnWrapCellChanged(DependencyObject d, DependencyPropertyChangedEventArgs args)
        {
            var gridDataControl = d as GridDataControl;
            gridDataControl.Model.Options.WrapCell = (bool)args.NewValue;
        }

        #endregion

        #region WrapCellBehavior

        public static readonly DependencyProperty WrapCellBehaviorProperty = DependencyProperty.Register("WrapCellBehavior", typeof(GridWrapCellBehavior), typeof(GridDataControl), new PropertyMetadata(OnWrapCellBehaviorBehaviorChanged));

        /// <summary>
        /// 
        /// </summary>
        public GridWrapCellBehavior WrapCellBehavior
        {
            get
            {
                return (GridWrapCellBehavior)GetValue(WrapCellBehaviorProperty);
            }

            set
            {
                SetValue(WrapCellBehaviorProperty, value);
            }
        }

        private static void OnWrapCellBehaviorBehaviorChanged(DependencyObject d, DependencyPropertyChangedEventArgs args)
        {
            var gridDataControl = d as GridDataControl;
            gridDataControl.Model.Options.WrapCellBehavior = (GridWrapCellBehavior)args.NewValue;
        }

        #endregion

        #region SourceType (DependencyProperty)

        /// <summary>
        /// gets / sets the source type for the underlying source.
        /// </summary>
        public Type SourceType
        {
            get { return (Type)GetValue(SourceTypeProperty); }
            set { SetValue(SourceTypeProperty, value); }
        }

        public static readonly DependencyProperty SourceTypeProperty = DependencyProperty.Register("SourceType", typeof(Type), typeof(GridDataControl), new PropertyMetadata(null, OnSourceTypeChanged));

        private static void OnSourceTypeChanged(DependencyObject dpo, DependencyPropertyChangedEventArgs args)
        {
            var grid = dpo as GridDataControl;
            if (args.NewValue != null)
            {
                grid.TableProperties.SourceType = (Type)args.NewValue;
            }
        }

        #endregion

        #region ExpressionFunc (DependencyProperty)

        /// <summary>
        /// Gets / sets the custom expression func.
        /// </summary>
        public IUnboundExpressionFunc ExpressionFunc
        {
            get { return (IUnboundExpressionFunc)GetValue(ExpressionFuncProperty); }
            set { SetValue(ExpressionFuncProperty, value); }
        }

        public static readonly DependencyProperty ExpressionFuncProperty = DependencyProperty.Register("ExpressionFunc", typeof(IUnboundExpressionFunc), typeof(GridDataControl), new PropertyMetadata(OnExpressionFuncChanged));

        private static void OnExpressionFuncChanged(DependencyObject dpo, DependencyPropertyChangedEventArgs args)
        {
            var grid = dpo as GridDataControl;
            if (args.NewValue != null)
            {
                grid.TableProperties.ExpressionFunc = (IUnboundExpressionFunc)args.NewValue;
            }
            else
            {
                grid.TableProperties.ExpressionFunc = null;
            }
        }

        #endregion

        #region PersistGroupsExpandedState (DependencyProperty)

        /// <summary>
        /// Gets / sets the group expand state.
        /// </summary>
        public bool PersistGroupsExpandedState
        {
            get { return (bool)GetValue(PersistGroupsExpandedStateProperty); }
            set { SetValue(PersistGroupsExpandedStateProperty, value); }
        }

        public static readonly DependencyProperty PersistGroupsExpandedStateProperty = DependencyProperty.Register("PersistGroupsExpandedState", typeof(bool), typeof(GridDataControl), new PropertyMetadata(false, OnPersistGroupsExpandedStateChanged));

        private static void OnPersistGroupsExpandedStateChanged(DependencyObject dpo, DependencyPropertyChangedEventArgs args)
        {
            var grid = dpo as GridDataControl;
            grid.TableProperties.PersistGroupsExpandState = (bool)args.NewValue;
        }

        #endregion

        #region DetailsView Template

        // Using a DependencyProperty as the backing store for DetailsViewTemplate.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty DetailsViewTemplateProperty =
            DependencyProperty.Register("DetailsViewTemplate", typeof(DataTemplate), typeof(GridDataControl), new PropertyMetadata(OnDetailsViewTemplateChanged));

        /// <summary>
        /// Gets or sets the details view template.
        /// </summary>
        /// <value>The details view template.</value>
        public DataTemplate DetailsViewTemplate
        {
            get { return (DataTemplate)GetValue(DetailsViewTemplateProperty); }
            set { SetValue(DetailsViewTemplateProperty, value); }
        }

        /// <summary>
        /// Handles changes to the DetailsView Template property.
        /// </summary>
        private static void OnDetailsViewTemplateChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var grid = d as GridDataControl;
            grid.TableProperties.DetailsViewTemplate = (DataTemplate)e.NewValue;
        }

        #endregion

      

        #region PrivateMethods

        /// <summary>
        /// 
        /// </summary>
        /// <param name="skinStyle"></param>
        void ISkinStylePropagator.OnStyleChanged(Theming.VisualStyle visualStyle)
        {
            var visualStyleValue = GridDataControl.ResolveVisualStyleToSkinStorage(this.VisualStyle);
            if (visualStyle != visualStyleValue)
            {
                // we just ensure that the DO has the same visual style with SkinStorage
                this.VisualStyle = ResolveSkinStorageToVisualStyle(visualStyle);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void GridDataControl_Loaded(object sender, RoutedEventArgs e)
        {
            this.Unloaded += new RoutedEventHandler(GridDataControl_Unloaded);
            this.Dispatcher.BeginInvoke(new Action(() =>
            {
                if (this.Model == null)
                    return;

                this.Model.ColumnAutoSizer.IsGridDataControlLoaded = true;
                this.Model.ColumnAutoSizer.RefreshAll();
                this.Model.TableStyle.AllowRowResize = this.AllowResizeRows;
                if (this.ColumnSizer == GridControlLengthUnitType.AutoOnLoad || this.ColumnSizer == GridControlLengthUnitType.AutoOnLoadWithLastColumnFill)
                {
                    this.Model.ColumnAutoSizer.IsAutoOnLoad = true;
                }
            }));
        }

        void GridDataControl_Unloaded(object sender, RoutedEventArgs e)
        {
            UnWireGrid();
        }

        private void PART_CloseButton_Click(object sender, RoutedEventArgs e)
        {
            this.StatusBarMessage = "";
            //this.Model.View.BeginInit();
            this.Model.TableProperties.SuspendEvents();
            foreach (var item in this.Model.TableProperties.VisibleColumns)
            {
                if (item != null && item.Filters.Count > 0)
                {
                    this.Model.FilterColumn(item, null, Syncfusion.Linq.FilterType.Undefined, this.Model.TableProperties.FilterBarPredicateType, false, true);
                    item.Filters.Clear();
                }
            }
            this.Model.TableProperties.ResumeEvents();
            this.Model.View.FilterPredicates.Clear();
            this.Model.View.Filter = null;
            //this.Model.View.EndInit();
            this.Model.InvalidateDisplay();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="args"></param>
        private void OnConditionalFormatsChanged(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs args)
        {
            if (args.Action == System.Collections.Specialized.NotifyCollectionChangedAction.Add)
            {
                var cf = args.NewItems[0] as GridDataConditionalFormat;
                cf.SetTableModel(this.Model);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        private void HookGrid()
        {
            if (this.ctorModel == null)
            {
                this.ctorModel = this.OnModelCreated();
                this.ctorModel.CurrencyManager.CurrentRecordSelectionChanged += this.OnCurrentRecordSelectionChanged;
            }
            //// Hook the internal dependency
            GridDataTableModel model = this.ctorModel;
            this.ctorModel = null;

            //// set the table properties before the grid is set
            model.TableProperties = this.TableProperties;
            this.InternalGrid.Model = model;
            model.Grid = this.InternalGrid;

            if (this.isCaptionSummaryRowChanged)
            {
                model.TableProperties.CaptionSummaryRow = this.CaptionSummaryRow;
            }

            if (this.isShowGroupSummaryInCaptionLoaded)
            {
                model.TableProperties.ShowGroupSummaryInCaption = this.ShowGroupSummaryInCaption;
            }

            if (this.isEnableBlendStyleSetBeforeLoaded)
            {
                this.InternalGrid.EnableBlendStyling = this.EnableBlendStyling;
            }


            if (this.isItemsSourceLoadedBeforeGridLoaded)
            {
                model.TableProperties.ItemsSource = this.ItemsSource;
                this.OnItemsSourceChanged(new SyncfusionRoutedEventArgs());
            }

            if (this.isVisualStyleChangedBeforeGridLoaded)
            {
                model.TableProperties.CustomVisualStyle = this.CustomVisualStyle;
                model.TableProperties.VisualStyle = this.VisualStyle;
            }

            if (!this.ensuredProperties)
            {
                this.EnsureProperties();
            }

            #region EnableRenderOptimization code

            if (this.isEnableRenderOptimizationSetBeforeLoaded)
            {
                this.InternalGrid.EnableRenderOptimization = this.EnableRenderOptimization;
            }
            #endregion

            this.OnModelLoaded();
        }

        /// <summary>
        /// 
        /// </summary>
        private void CopyInitializePropertyCollections()
        {
            this.SortColumns = this.TableProperties.SortColumns;
            this.GroupedColumns = this.TableProperties.GroupedColumns;
            this.Relations = this.TableProperties.Relations;
            this.SummaryRows = this.TableProperties.SummaryRows;
            this.TableSummaryRows = this.TableProperties.SummaryRows;
            this.StackedHeaderRows = this.TableProperties.StackedHeaderRows;
            this.ConditionalFormats = this.TableProperties.ConditionalFormats;
            this.VisibleColumns = this.TableProperties.VisibleColumns;
        }

        private void UnWireGrid()
        {
            //if (this.ConditionalFormats != null)
            //    this.ConditionalFormats.CollectionChanged -= new System.Collections.Specialized.NotifyCollectionChangedEventHandler(this.OnConditionalFormatsChanged);
            //if (this.Model != null)
            //    this.Model.SelectionChanged -= new GridSelectionChangedEventHandler(Model_SelectionChanged);
            this.Unloaded -= new RoutedEventHandler(GridDataControl_Unloaded);
        }

        #endregion

        #region ProtectedMethods

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        protected virtual GridDataTableProperties GetTableProperties()
        {
            return new GridDataTableProperties();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        protected virtual GridDataTableModel OnModelCreated()
        {
            return new GridDataTableModel();
        }


        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        protected virtual GridDataControlBaseImpl GetGridHost()
        {
            return new GridDataControlBaseImpl();
        }

        /// <summary>
        /// 
        /// </summary>
        protected virtual void EnsureProperties()
        {
            GridDataTableModel model = this.Model;
            if (this.isEnableLegacyStyleChanged)
            {
                model.TableProperties.EnableLegacyStyle = this.EnableLegacyStyle;
            }
            if (this.ShowGroupDropArea)
            {
                //this.AdjustMainGrid();
                this.GroupDropAreaGrid.AttachParentGrid(model, this.InternalGrid);
            }

            if (this.isStyleManagerChangedBeforeLoaded)
            {
                this.InternalGrid.StyleManager = this.StyleManager;
                model.TableProperties.StyleManager = this.StyleManager;
            }

            if (this.isHeaderStyleSetBeforeGridLoaded)
            {
                this.InternalGrid.HeaderStyle = this.HeaderStyle;
                this.Model.TableProperties.HeaderStyle = this.HeaderStyle;
            }

            if (this.isAllowDragChangedBeforeGridLoaded)
            {
                model.TableProperties.AllowDragColumns = this.AllowDragColumns;
            }

            if (this.isAllowGroupChangedBeforeGridLoaded)
            {
                model.TableProperties.AllowGroup = this.AllowGroup;
            }
            if (this._isAllowNestedGridPaddingChangedBeforeGridLoaded)
            {
                model.TableProperties.AllowNestedGridPadding = this.AllowNestedGridPadding;
            }

            if (this.isShowRowHeaderChangedBeforeGridLoaded)
            {
                model.TableProperties.ShowRowHeader = this.ShowRowHeader;
            }

            if (this.isShowGroupDropAreaChangedBeforeGridLoaded)
            {
                model.TableProperties.ShowGroupDropArea = this.ShowGroupDropArea;
            }

            if (this.isAllowSortChangedBeforeGridLoaded)
            {
                model.TableProperties.AllowSort = this.AllowSort;
            }

            if (this.isAllowResizeRowsChangedBeforeLoaded || this.AllowResizeRows)
            {
                model.TableProperties.AllowResizeRows = this.AllowResizeRows;
            }

            if (this.isAllowResizeColumnsChangedBeforeLoaded)
            {
                model.TableProperties.AllowResizeColumns = this.AllowResizeColumns;
            }

            if (this.isShowFiltersChangedBeforeGridLoaded)
            {
                model.TableProperties.ShowFilters = this.ShowFilters;
            }

            if (this.isUpdateModeChangedBeforeGridLoaded)
            {
                model.TableProperties.UpdateMode = this.UpdateMode;
            }

            if (this.isGroupDropAreaTextChanged && this.ShowGroupDropArea && this.GroupDropAreaGrid != null)
            {
                this.GroupDropAreaGrid.Model.GroupDropAreaText = this.GroupDropAreaText;
            }

            if (this.isGroupDropAreaHeightChanged && this.ShowGroupDropArea && this.GroupDropAreaGrid != null)
            {
                this.GroupDropAreaGrid.Model.GroupDropAreaHeight = this.GroupDropAreaHeight;
            }

            if (this.isGroupCaptionTextChanged)
            {
                model.TableProperties.GroupCaptionText = this.GroupCaptionText != string.Empty ? this.GroupCaptionText : GridDataControl.GroupCaptionConstant;
            }

            if (this.isShowGroupSummariesChanged)
            {
                model.TableProperties.ShowGroupSummaries = this.ShowGroupSummaries;
            }

            if (this.isShowErrorTooltipLoadedBeforeGrid || this.ShowErrorTooltips)
            {
                GridTooltipService.SetShowErrorTooltips(this.InternalGrid, this.ShowErrorTooltips);
            }

            if (this.isShowTooltipsChangedBeforeGridLoaded || this.ShowTooltips)
            {
                GridTooltipService.SetShowTooltips(this.InternalGrid, this.ShowTooltips);
                //model.TableProperties.ShowTooltips = this.ShowTooltips;
            }


            if (this.isShowGroupSummaryInCaptionLoaded)
            {
                model.TableProperties.ShowGroupSummaryInCaption = this.ShowGroupSummaryInCaption;
            }

            if (this.isGroupsExpandedSetBeforeLoad)
            {
                model.TableProperties.IsGroupsExpanded = this.IsGroupsExpanded;
            }

            if (this.isNotifyPropertyChangedSetBeforeGridLoaded)
            {
                model.TableProperties.NotifyPropertyChanges = this.NotifyPropertyChanges;
            }

            if (this.isColumnSizerChangedBeforeGridLoaded)
            {
                model.TableProperties.ColumnSizer = this.ColumnSizer;
            }

            if (this.isDragIndicatorInnerBrushChangedBeforeGridLoaded)
            {
                model.TableProperties.DragIndicatorInnerBrush = this.DragIndicatorInnerBrush;
            }

            if (this.isDragIndicatorOuterBrushChangedBeforeGridLoaded)
            {
                model.TableProperties.DragIndicatorOuterBrush = this.DragIndicatorOuterBrush;
            }

#if !SILVERLIGHT
            if (this.isShowGroupIndicatorsChangedBeforeLoaded)
            {
                model.TableProperties.ShowGroupIndicators = this.ShowGroupIndicators;
            }            
#endif
            if (this.AutoPopulateRelations)
            {
                foreach (var rd in this.Relations)
                {
                    rd.TableProperties.InitializeFromInternal(this.Model.TableProperties, false);
                };
            }

            this.ensuredProperties = true;
        }

        /// <summary>
        /// 
        /// </summary>
        protected virtual void OnModelLoaded()
        {
            // raise the Model loaded event after everything is set
            if (this.ModelLoaded != null)
            {
                this.ModelLoaded(this, EventArgs.Empty);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="e"></param>
        protected virtual void OnItemsSourceChanged(SyncfusionRoutedEventArgs e)
        {
            if (!this.ensuredProperties)
            {
                this.EnsureProperties();
            }

            if (this.isSelectedItemSetBeforeLoaded)
            {
                this.Model.View.MoveCurrentTo(this.SelectedItem);
            }

            var handler = this.ItemsSourceChanged;
            if (handler != null)
            {
                handler(this, e);
            }
        }

        #endregion

        #region InternalMethods
        #endregion

        #region PublicMethods

        /// <summary>
        /// Gets the model. Once the template gets applied, the Model object will return. Otherwise, it would be null. So use this after the AppliedTemplate flow is called.
        /// </summary>
        /// <value>The model.</value>
        public GridDataTableModel Model
        {
            get
            {
                if (this.InternalGrid != null)
                {
                    return this.InternalGrid.Model as GridDataTableModel;
                }

                if (this.ctorModel == null)
                {
                    this.ctorModel = this.OnModelCreated();
                    this.ctorModel.CurrencyManager.CurrentRecordSelectionChanged += new GridDataCurrentRecordSelectionChangedEventHandler(this.OnCurrentRecordSelectionChanged);
                }

                return this.ctorModel;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();
            var scrollViewer = this.GetTemplateChild("PART_ScrollViewer") as ScrollableContentViewer;
            this.PART_BorderStatusBar = this.GetTemplateChild("PART_BorderStatusBar") as Border;
            this.PART_CloseButton = this.GetTemplateChild("PART_CloseButton") as Button;
            SetGroupDropAreaGrid();
            if (this.PART_CloseButton != null)
            {
                this.PART_CloseButton.Click += new RoutedEventHandler(PART_CloseButton_Click);
            }

            this.InternalGrid = this.GetGridHost();
            if (this.InternalGrid != null)
            {
                scrollViewer.Content = this.InternalGrid;
            }

            if (this.InternalGrid != null)
            {
                this.isGridLoaded = true;
                this.HookGrid();               
            }

            ShowHideStatusBar();
        }

        internal void SetGroupDropAreaGrid()
        {
            this.GroupDropAreaGrid = this.GetTemplateChild(GridDataControl.TemplateGroupDropAreaGrid) as GridDataGroupDropAreaGridImpl;
        }


        #endregion
    }

    public enum VisualStyle
    {
        Default = 0,
        Office2007Blue = 1,
        Office2007Silver = 2,
        Office2007Black = 3,
        Office2003 = 4,
        Blend = 5,
        Custom = 6,
        GlassyGreen = 7,
        SunBlack = 8,
        ShinyRed = 9,
        ShinyBlue = 10,
        BureauBlue = 11,
        BureauBlack = 12,
        TwilightBlue = 13,
        DefaultOffice2007Blue = 14,
        DefaultOffice2007Silver = 15,
        DefaultOffice2007Black = 16,
        Office14Blue = 17,
        Office14Black = 18,
        Office14Silver = 19,
        VS2010 = 20,
        Windows7,
        SyncfusionTheme,
        Metro
    }

    public class GridDataHeightConverter : IValueConverter
    {
        #region IValueConverter Members

        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            var result = (bool)value;
            if (result)
            {
                return Visibility.Visible;
            }

            return Visibility.Collapsed;
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            throw new NotImplementedException();
        }

        #endregion
    }
}
