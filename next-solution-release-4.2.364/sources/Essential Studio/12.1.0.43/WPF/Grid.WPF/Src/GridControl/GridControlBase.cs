#region Copyright Syncfusion Inc. 2001 - 2014
// Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Any infringement will be prosecuted under
// applicable laws. 
#endregion
//#define TestDrawTextPerformance

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Windows;
using System.Linq;
using System.Windows.Controls.Primitives;
using System.Windows.Input;
using System.Windows.Media;
using System.Text;
using Syncfusion.Windows.ComponentModel;
using Syncfusion.Windows.Controls.Cells;
using Syncfusion.Windows.Controls.Scroll;
using Syncfusion.Windows.Diagnostics;
using Syncfusion.Windows.GridCommon;
using Syncfusion.Windows.Controls.Grid.Automation.Peers;
using System.Globalization;

#if ENABLE_PARTIAL_TRUST
using System.Security;
#endif

namespace Syncfusion.Windows.Controls.Grid
{
    /// <summary>
    /// Implements a grid control that displays a grid model.
    /// </summary>
    /// <remarks>
    /// <see cref="GridControlBase"/> offers many events that you can subscribe to and modify the default behavior of the grid. Typically, methods
    /// that are sent before the action is carried out allows you to adjust certain parameters or cancel the operation. These events usually
    /// end with an "ing" suffix, like "Changing". Events that are raised after the data in the model have been changed inform associated controls
    /// about the success of the operation.
    /// </remarks>
#if SyncfusionFramework4_0
    [System.ComponentModel.DesignTimeVisible(false)]
#endif   

#if ENABLE_PARTIAL_TRUST
    [SecuritySafeCritical]
#endif
    public partial class GridControlBase : VirtualizingCellsControl
    {
        #region Fields
        DrawingVisual dvSelectBackground = new NoHitTestDrawingVisual();
        DrawingVisual dvSelectBorder = new NoHitTestDrawingVisual();
        DrawingVisual dvHiddenBorder = new NoHitTestDrawingVisual();
        DrawingVisual dvCurrentCellBorder = new NoHitTestDrawingVisual();
        GridModel model = null;
        GridCellRendererCollection cellRenderers = null;
        GridCurrentCell currentCell;
        GridControlRenderStyles renderStyles;
        public GridRangeInfo previous = null;
        bool resethiddenwhenfreezing = true;
        #endregion

        #region Dependency Properties

        #region RenderCellInfo
        /// <summary>
        /// Gets the <see cref="RenderCellInfoProperty"/> attached dependency property value. 
        /// </summary>
        /// <param name="dpo">The instance to be queried for the effective value of the dependency property.</param>
        /// <returns>
        /// Returns the effective value for the given instance.
        /// </returns>
        public static GridRenderStyleInfo GetRenderStyleInfo(DependencyObject dpo)
        {
            return GetRenderCellInfo(dpo) as GridRenderStyleInfo;
        }

        /// <summary>
        /// Sets the <see cref="RenderCellInfoProperty"/> attached dependency property value. 
        /// </summary>
        /// <param name="dpo">The instance to be assigned the value of the dependency property.</param>
        /// <param name="value">The value.</param>
        public static void SetRenderStyleInfo(DependencyObject dpo, GridRenderStyleInfo value)
        {
            SetRenderCellInfo(dpo, value);
        }
        #endregion

        #region CellRendererProperty
        /// <summary>
        /// Gets the <see cref="CellRendererProperty"/> attached dependency property value. 
        /// </summary>
        /// <param name="dpo">The instance to be queried for the effective value of the dependency property.</param>
        /// <returns>
        /// Returns the effective value for the given instance.
        /// </returns>
        public static new IGridCellRenderer GetCellRenderer(DependencyObject dpo)
        {
            return VirtualizingCellsControl.GetCellRenderer(dpo) as IGridCellRenderer;
        }

        /// <summary>
        /// Sets the <see cref="CellRendererProperty"/> attached dependency property value. 
        /// </summary>
        /// <param name="dpo">The instance to be assigned the value of the dependency property.</param>
        /// <param name="value">The value.</param>
        public static void SetCellRenderer(DependencyObject dpo, IGridCellRenderer value)
        {
            VirtualizingCellsControl.SetCellRenderer(dpo, value);
        }
        #endregion

        #region IgnoreChangedEventProperty
        /// <summary>
        /// Returns the cell renderer of an UIElement inside a cell. When the editor
        /// inside a cell has children and you query this attached property for a child it will query the top-most 
        /// parent element of the cell renderer for the value of the property.
        /// </summary>
        public static readonly DependencyProperty IgnoreChangedEventProperty = DependencyProperty.Register(
            "IgnoreChangedEvent", typeof(bool?), typeof(GridControlBase), new FrameworkPropertyMetadata(null));

        /// <summary>
        /// Gets the <see cref="IgnoreChangeProperty"/> attached dependency property value. 
        /// </summary>
        /// <param name="dpo">The instance to be queried for the effective value of the dependency property.</param>
        /// <returns>
        /// Returns the effective value for the given instance.
        /// </returns>
        public static bool? GetIgnoreChangedEvent(DependencyObject dpo)
        {
            return (bool?)dpo.GetValue(IgnoreChangedEventProperty);
        }

        /// <summary>
        /// Sets the <see cref="IgnoreChangeProperty"/> attached dependency property value. 
        /// </summary>
        /// <param name="dpo">The instance to be assigned the value of the dependency property.</param>
        /// <param name="value">The value.</param>
        public static void SetIgnoreChangedEvent(DependencyObject dpo, bool value)
        {
            dpo.SetValue(IgnoreChangedEventProperty, value);
        }
        #endregion
        #endregion

        #region DelayLoad property

        /// <summary>
        /// Dependency attached property for GetDelayLoad/SetDelayLoad.
        /// </summary>
        private static readonly DependencyProperty DelayLoadProperty = DependencyProperty.RegisterAttached("DelayLoad", typeof(bool), typeof(GridControlBase), new PropertyMetadata(null));

        /// <summary>
        /// Gets a boolean value if the DependencyObject should be loaded with a delay when it is created.
        /// </summary>
        /// <param name="dpo">The dpo.</param>
        /// <returns></returns>
        public static bool GetDelayLoad(DependencyObject dpo)
        {
            return (bool)dpo.GetValue(GridControlBase.DelayLoadProperty);
        }

        /// <summary>
        /// Sets a boolean value if the DependencyObject should be loaded with a delay when it is created. Set this for templated controls.
        /// </summary>
        /// <param name="dpo"></param>
        /// <param name="value"></param>
        public static void SetDelayLoad(DependencyObject dpo, bool value)
        {
            dpo.SetValue(GridControlBase.DelayLoadProperty, value);
        }

        #endregion

        #region AutomationTemplateElement

        private static readonly DependencyProperty AutomationTemplateElement = DependencyProperty.Register("AutomationTemplateElement", typeof(bool), typeof(GridControlBase), new PropertyMetadata(null));

        /// <summary>
        /// Gets value true / false if this is a templated element, Set this to true if the AutomationElement returning from a grid cell has to be from the underlying
        /// control.
        /// </summary>
        /// <param name="dpo">Dependency Object.</param>
        /// <returns>true / false</returns>
        public static bool GetAutomationTemplateElement(DependencyObject dpo)
        {
            return (bool)dpo.GetValue(GridControlBase.AutomationTemplateElement);
        }

        /// <summary>
        /// Sets the automation template element.
        /// </summary>
        /// <param name="dpo">The dpo.</param>
        /// <param name="value">if set to <c>true</c> [value].</param>
        public static void SetAutomationTemplateElement(DependencyObject dpo, bool value)
        {
            dpo.SetValue(GridControlBase.AutomationTemplateElement, value);
        }

        #endregion

        #region AllowComboBoxDropDownKeyHandling

        /// <summary>
        /// AllowComboBoxDropDownKeyHandling Attached Dependency Property
        /// </summary>
        public static readonly DependencyProperty AllowDropDownKeyHandlingProperty = DependencyProperty.RegisterAttached("AllowDropDownKeyHandling", typeof(bool), typeof(GridControlBase), new PropertyMetadata((bool)false));

        /// <summary>
        /// Gets the AllowComboBoxDropDownKeyHandling property. This dependency property 
        /// indicates whether ComboBox DropDown should handle up down key.
        /// </summary>
        public static bool GetAllowDropDownKeyHandling(DependencyObject d)
        {
            return (bool)d.GetValue(AllowDropDownKeyHandlingProperty);
        }

        /// <summary>
        /// Sets the AllowComboBoxDropDownKeyHandling property. This dependency property 
        /// indicates whether ComboBox DropDown should handle up down key.
        /// </summary>
        public static void SetAllowDropDownKeyHandling(DependencyObject d, bool value)
        {
            d.SetValue(AllowDropDownKeyHandlingProperty, value);
        }

        #endregion


        #region AllowSelectionInDataTemplate (Attached DependencyProperty)

        public static readonly DependencyProperty AllowSelectionInDataTemplateProperty =
            DependencyProperty.RegisterAttached("AllowSelectionInDataTemplate", typeof(bool), typeof(GridControlBase), new PropertyMetadata((bool)false));

        public static void SetAllowSelectionInDataTemplate(DependencyObject o, bool value)
        {
            o.SetValue(AllowSelectionInDataTemplateProperty, value);
        }

        public static bool GetAllowSelectionInDataTemplate(DependencyObject o)
        {
            return (bool)o.GetValue(AllowSelectionInDataTemplateProperty);
        }

        #endregion


        #region AddSkinResourceForDataTemplate

        /// <summary>
        /// AddSkinResourceForDataTemplate Attached Dependency Property
        /// </summary>
        public static readonly DependencyProperty AddSkinResourceForDataTemplateProperty = DependencyProperty.RegisterAttached("AddSkinResourceForDataTemplate", typeof(bool), typeof(GridControlBase), new PropertyMetadata((bool)false));

        /// <summary>
        /// Gets the AddSkinResourceForDataTemplate property. This dependency property 
        /// indicates whether skins have to be added for DataTemplate as MergedDictionaries.
        /// </summary>
        public static bool GetAddSkinResourceForDataTemplate(DependencyObject d)
        {
            return (bool)d.GetValue(AddSkinResourceForDataTemplateProperty);
        }

        /// <summary>
        /// Sets the AddSkinResourceForDataTemplate property. This dependency property 
        /// indicates whether skins have to be added for DataTemplate as MergedDictionaries.
        /// </summary>
        public static void SetAddSkinResourceForDataTemplate(DependencyObject d, bool value)
        {
            d.SetValue(AddSkinResourceForDataTemplateProperty, value);
        }

        #endregion

        /// <summary>
        /// Obsolete. Use Model.Options.HighlightSelectionAlphaBlend.
        /// </summary>
        [Obsolete("Use Model.Options.HighlightSelectionAlphaBlend")]
        public Brush HighlightBrush
        {
            get { return Model.Options.HighlightSelectionAlphaBlend; }
            set { Model.Options.HighlightSelectionAlphaBlend = value; }
        }

        /// <summary>
        /// Obsolete. Use Model.Options.HighlightSelectionBorder.
        /// </summary>
        [Obsolete("Use Model.Options.HighlightSelectionBorder")]
        public Brush HighlightBorder
        {
            get { return Model.Options.HighlightSelectionBorder; }
            set { Model.Options.HighlightSelectionBorder = value; }
        }

        /// <summary>
        /// Obsolete. Use Model.Options.CurrentCellBorderWitdh.
        /// </summary>
        [Obsolete("Use Model.Options.CurrentCellBorderWitdh")]
        public double CurrentCellBorderWeight
        {
            get { return Model.Options.CurrentCellBorderWidth; }
            set { Model.Options.CurrentCellBorderWidth = value; }
        }

        #region EnableRenderOptimization (DependencyProperty)

        internal EnableRenderOptimization EnableRenderOptimization
        {
            get { return (EnableRenderOptimization)GetValue(EnableRenderOptimizationProperty); }
            set { SetValue(EnableRenderOptimizationProperty, value); }
        }

        internal static readonly DependencyProperty EnableRenderOptimizationProperty = DependencyProperty.Register("EnableRenderOptimization", typeof(EnableRenderOptimization), typeof(GridControlBase), new PropertyMetadata(EnableRenderOptimization.None));

        internal void MarkIndividualCellBackgroundDirty(int rowIndex, int colIndex)
        {
            if (this.EnableRenderOptimization == EnableRenderOptimization.DisableBackgroundFrameRendering)
            {
                this.InvalidateCellBackground(rowIndex, colIndex, true);
            };
        }

        internal void MarkIndividualRowBackgroundDirty(int rowIndex)
        {
            if (this.EnableRenderOptimization == EnableRenderOptimization.DisableBackgroundFrameRendering)
            {
                for (int col = this.ScrollColumns.ScrollLineIndex; col <= this.ScrollColumns.LastBodyVisibleLineIndex; ++col)
                {
                    this.InvalidateCellBackground(rowIndex, col, true);
                }
            };
        }
        #endregion

        protected override void OnPreviewMouseDown(MouseButtonEventArgs e)
        {
            base.OnPreviewMouseDown(e);
            if (Model != null)
            {
                Model.ActiveGridView = this;
            }
        }

        #region Ctor
        /// <summary>
        /// Initializes a new <see cref="GridControlBase"/>.
        /// </summary>
        public GridControlBase()
        {
            renderStyles = new GridControlRenderStyles(this);
            Focusable = true;

            MouseControllerDispatcher.Add(new GridResizeColumnsMouseController(this));
            MouseControllerDispatcher.Add(new GridResizeRowsMouseController(this));
            MouseControllerDispatcher.Add(new GridSelectCellsMouseController(this));
            //MouseControllerDispatcher.Add(new GridClickCellsMouseController(this));
            //MouseControllerDispatcher.Add(this.ExcelLikeDragDrop);

            BackgroundFrame.Children.Add(dvSelectBackground);
            ForegroundFrame.Children.Add(dvSelectBorder);
            ForegroundFrame.Children.Add(dvCurrentCellBorder);
            ForegroundFrame.Children.Add(dvHiddenBorder);

            currentCell = new GridCurrentCell(this);

            // CoveredCellsProvider, CellSpanBackgroundsProvider, 
            // RowHeightsProvider and ColumnWidthsProvider are set
            // in Model property setter below.

            GridControlBase.SetCellsControl(this, this); // TODO: Debug MoveCurrentHelper with nested grids.
            GridControlBase.SetWantsMouseInput(this, true);

            //GridCommentService.SetShowComment(this,true);
            CommandManager.RegisterClassCommandBinding(typeof(GridControlBase), new CommandBinding(ApplicationCommands.Copy, new ExecutedRoutedEventHandler(OnExecutedCopy), new CanExecuteRoutedEventHandler(OnCanExecuteCopy)));
            CommandManager.RegisterClassCommandBinding(typeof(GridControlBase), new CommandBinding(ApplicationCommands.Cut, new ExecutedRoutedEventHandler(OnExecutedCut), new CanExecuteRoutedEventHandler(OnCanExecuteCut)));
            CommandManager.RegisterClassCommandBinding(typeof(GridControlBase), new CommandBinding(ApplicationCommands.Paste, new ExecutedRoutedEventHandler(OnExecutedPaste), new CanExecuteRoutedEventHandler(OnCanExecutePaste)));
            CommandManager.RegisterClassCommandBinding(typeof(GridControlBase), new CommandBinding(ApplicationCommands.Undo, new ExecutedRoutedEventHandler(OnExecuteUndo), new CanExecuteRoutedEventHandler(OnCanExecuteUndo)));
            CommandManager.RegisterClassCommandBinding(typeof(GridControlBase), new CommandBinding(ApplicationCommands.Redo, new ExecutedRoutedEventHandler(OnExecuteRedo), new CanExecuteRoutedEventHandler(OnCanExecuteRedo)));
        }
        #endregion

        #region Copy Paste Routed Commands

        protected virtual void OnCanExecuteCopy(CanExecuteRoutedEventArgs args)
        {
            if (this.Model.CutPaste.CanCopy())
            {
                args.CanExecute = true;
                args.Handled = true;
            }
        }

        private static void OnCanExecuteCopy(object sender, CanExecuteRoutedEventArgs args)
        {
            ((GridControlBase)sender).OnCanExecuteCopy(args);
        }

        protected virtual void OnExecutedCopy(ExecutedRoutedEventArgs args)
        {

            this.Model.CutPaste.Copy();
            args.Handled = true;
        }

        private static void OnExecutedCopy(object sender, ExecutedRoutedEventArgs args)
        {
            ((GridControlBase)sender).OnExecutedCopy(args);
        }

        protected virtual void OnCanExecuteCut(CanExecuteRoutedEventArgs args)
        {
            if (this.Model.CutPaste.CanCut())
            {
                args.CanExecute = true;
                args.Handled = true;
            }
        }

        private static void OnCanExecuteCut(object sender, CanExecuteRoutedEventArgs args)
        {
            ((GridControlBase)sender).OnCanExecuteCut(args);
        }

        protected virtual void OnExecutedCut(ExecutedRoutedEventArgs args)
        {
            if (this.Model.TableStyle.ReadOnly || (this is GridTreeControlImpl && (this as GridTreeControlImpl).ReadOnly))
                return;            
            this.Model.CutPaste.Cut();
            args.Handled = true;
        }

        private static void OnExecutedCut(object sender, ExecutedRoutedEventArgs args)
        {
            ((GridControlBase)sender).OnExecutedCut(args);
        }

        protected virtual void OnCanExecutePaste(CanExecuteRoutedEventArgs args)
        {
            if (this.Model.CutPaste.CanPaste())
            {
                args.CanExecute = true;
                args.Handled = true;
            }
        }

        private static void OnCanExecutePaste(object sender, CanExecuteRoutedEventArgs args)
        {
            ((GridControlBase)sender).OnCanExecutePaste(args);
        }

        protected virtual void OnExecutedPaste(ExecutedRoutedEventArgs args)
        {
            this.Model.CutPaste.Paste();
            args.Handled = true;
        }

        private static void OnExecutedPaste(object sender, ExecutedRoutedEventArgs args)
        {
            ((GridControlBase)sender).OnExecutedPaste(args);
        }

        #endregion

        #region Undo Redo Commands

        protected virtual void OnCanExecuteUndo(CanExecuteRoutedEventArgs args)
        {
            if (this.Model != null && this.Model.ShouldRecordUndo && this.CurrentCell != null && !this.CurrentCell.IsEditing)
            {
                args.CanExecute = true;
                args.Handled = true;
            }
        }

        protected static void OnCanExecuteUndo(object sender, CanExecuteRoutedEventArgs args)
        {
            ((GridControlBase)sender).OnCanExecuteUndo(args);
        }

        private static void OnExecuteUndo(object sender, ExecutedRoutedEventArgs args)
        {
            ((GridControlBase)sender).OnExecuteUndo(args);
        }

        protected virtual void OnExecuteUndo(ExecutedRoutedEventArgs args)
        {
            this.Model.CommandStack.Undo();
        }

        protected virtual void OnCanExecuteRedo(CanExecuteRoutedEventArgs args)
        {
            if (this.Model.ShouldRecordUndo && !this.CurrentCell.IsEditing)
            {
                args.CanExecute = true;
                args.Handled = true;
            }
        }

        protected static void OnCanExecuteRedo(object sender, CanExecuteRoutedEventArgs args)
        {
            ((GridControlBase)sender).OnCanExecuteRedo(args);
        }

        private static void OnExecuteRedo(object sender,ExecutedRoutedEventArgs args)
        {
            ((GridControlBase)sender).OnExecuteRedo(args);
        }

        protected virtual void OnExecuteRedo(ExecutedRoutedEventArgs args)
        {
            this.Model.CommandStack.Redo();
        }

        #endregion

        #region Properties: CurrentCell, Model, SelectedCells, CoveredCells, CellSpanBackgrounds, RowHeights, ColumnWidths, Header and Footer

        /// <summary>
        /// Gets or sets the model.
        /// </summary>
        /// <value>The model.</value>
        public GridModel Model
        {
            get
            {
                if (model == null)
                    Model = OnModelCreated();
                return model;
            }
            set
            {
                if (model != value)
                {
                    if (model != null)
                    {
                        UnwireModel();
                        CoveredCells.Clear();
                        CoveredCellsProvider = null;
                        OverlappingCellsProvider = null;
                        CellSpanBackgrounds.Clear();
                        CellSpanBackgroundsProvider = null;
                        RowHeightsProvider = null;
                        ColumnWidthsProvider = null;
                        model.CommitCellInfo -= new GridCommitCellInfoEventHandler(modelCommitCellInfo);
                        model.Disposing -= new EventHandler(modelDisposing);
                        if(model.Views != null)
                        model.RemoveView(this);
                        if (model.VolatileCellStyles != null)
                        {
                            model.VolatileCellStyles.BaseStylesMap.Dispose();
                            model.VolatileCellStyles.visibleRowIndexes.Remove(this);
                            model.VolatileCellStyles.visibleColumnIndexes.Remove(this);
                        }
                        if (cellRenderers != null)
                        {
                            cellRenderers.Dispose();
                            cellRenderers = null;
                        }
                        if (model.GraphicModel != null)
                        {
                            model.GraphicModel.Dispose();
                            model.GraphicModel = null;
                        }
                        model.ActiveGridView = null;
                    }
                    model = value;
                    if (model != null)
                    {
                        CoveredCellsProvider = Model.CoveredCells;
                        OverlappingCellsProvider = Model.OverlappingCells;
                        CellSpanBackgroundsProvider = Model.CellSpanBackgrounds;
                        RowHeightsProvider = Model.RowHeights;
                        ColumnWidthsProvider = Model.ColumnWidths;
                        model.CommitCellInfo += new GridCommitCellInfoEventHandler(modelCommitCellInfo);
                        model.Disposing += new EventHandler(modelDisposing);
                        model.AddView(this);
                        model.Options.PropertyChanged += new PropertyChangedEventHandler(Options_PropertyChanged);
                        WireModel();
                        if (model.Views != null)
                        {
                            foreach (GridControlBase grid in model.Views)
                            {
                                if (grid != null)
                                {
                                    model.ActiveGridView = grid;
                                    break;
                                }
                            }
                        }
                    }
                }
            }
        }

        protected virtual GridModel OnModelCreated()
        {
            return new GridModel();
        }

        void Options_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == "ColumnSizer")
            {
                this.Model.UpdateAutoSizer(true, true);
            }
            else if (e.PropertyName == "MaxLength")
            {
                if (this.Model.Sizer != null)
                {
                    this.Model.Sizer.ApplySizes();
                }
            }
            else if (e.PropertyName == "DragDropDropTargetFlags")
            {
                this.ExcelLikeDragDrop.EnableExcelLikeDragDrop(this.Model.Options.DragDropDropTargetFlags);
            }
            else if (e.PropertyName == "DataObjectConsumerOptions")
            {
                this.ChangeDataObjectConsumer();
            }
        }

        void modelDisposing(object sender, EventArgs e)
        {
            autoScroller.Dispose();
            Model = null;
        }

        internal void UnWireModelWhieDispose()
        {
            autoScroller.Dispose();
            this.UnwireModelEvents();
        }


        protected virtual void WireModel()
        {
            this.WireModelEvents();
        }

        protected virtual void UnwireModel()
        {
            this.UnwireModelEvents();
        }


        #region ModelEvents

        private void UnwireModelEvents()
        {
            this.Model.BaseStylesMapChanged -= Model_BaseStylesMapChanged;
            this.Model.CellModelsChanged -= Model_CellModelsChanged;
            this.Model.ColumnsInserted -= Model_ColumnsInserted;
            this.Model.ColumnsMoved -= Model_ColumnsMoved;
            this.Model.ColumnsRemoved -= Model_ColumnsRemoved;
            this.Model.CommitCellInfo -= Model_CommitCellInfo;
            this.Model.CommittedCellInfo -= Model_CommittedCellInfo;
            this.Model.ParseCommonFormats -= Model_ParseCommonFormats;
            this.Model.QueryBaseStyles -= Model_QueryBaseStyles;
            this.Model.QueryCellFormattedText -= Model_QueryCellFormattedText;
            this.Model.QueryCellInfo -= Model_QueryCellInfo;
            this.Model.QueryCellModel -= Model_QueryCellModel;
            this.Model.QueryCellSpanBackgrounds -= Model_QueryCellSpanBackgrounds;
            this.Model.QueryCellText -= Model_QueryCellText;
            this.Model.SaveCellText -= Model_SaveCellText;
            this.Model.QueryCoveredRange -= Model_QueryCoveredRange;
            this.Model.RowsInserted -= Model_RowsInserted;
            this.Model.RowsMoved -= Model_RowsMoved;
            this.Model.RowsRemoved -= Model_RowsRemoved;
            this.Model.SaveCellFormattedText -= Model_SaveCellFormattedText;
            this.Model.SelectionChanged -= Model_SelectionChanged;
            this.Model.SelectionChanging -= Model_SelectionChanging;
            this.Model.Disposing -= new EventHandler(modelDisposing);
        }

        private void WireModelEvents()
        {
            this.Model.BaseStylesMapChanged += Model_BaseStylesMapChanged;
            this.Model.CellModelsChanged += Model_CellModelsChanged;
            this.Model.ColumnsInserted += Model_ColumnsInserted;
            this.Model.ColumnsMoved += Model_ColumnsMoved;
            this.Model.ColumnsRemoved += Model_ColumnsRemoved;
            this.Model.CommitCellInfo += Model_CommitCellInfo;
            this.Model.CommittedCellInfo += Model_CommittedCellInfo;
            this.Model.ParseCommonFormats += Model_ParseCommonFormats;
            this.Model.QueryBaseStyles += Model_QueryBaseStyles;
            this.Model.QueryCellFormattedText += Model_QueryCellFormattedText;
            this.Model.QueryCellInfo += Model_QueryCellInfo;
            this.Model.QueryCellModel += Model_QueryCellModel;
            this.Model.QueryCellSpanBackgrounds += Model_QueryCellSpanBackgrounds;
            this.Model.QueryCellText += Model_QueryCellText;
            this.Model.SaveCellText += Model_SaveCellText;
            this.Model.QueryCoveredRange += Model_QueryCoveredRange;
            this.Model.RowsInserted += Model_RowsInserted;
            this.Model.RowsMoved += Model_RowsMoved;
            this.Model.RowsRemoved += Model_RowsRemoved;
            this.Model.SaveCellFormattedText += Model_SaveCellFormattedText;
            this.Model.SelectionChanged += Model_SelectionChanged;
            this.Model.SelectionChanging += Model_SelectionChanging;
        }

        #region SelectionChanging
        void Model_SelectionChanging(object sender, GridSelectionChangingEventArgs e)
        {
            if ((Model.Options.DrawSelectionOptions & (GridDrawSelectionOptions.ReplaceBackground | GridDrawSelectionOptions.ReplaceTextColor)) != 0)
            {
                foreach (GridRangeInfo range in Model.SelectedRanges)
                    InvalidateRenderCell(range);
            }

            this.OnSelectionChanging(e);
        }
        /// <summary>
        /// Occurs before the model updates internal data structures when the model is in the process of selecting
        /// a range of cells.
        /// </summary>
        public event GridSelectionChangingEventHandler SelectionChanging;

        protected virtual void OnSelectionChanging(GridSelectionChangingEventArgs e)
        {
            if (SelectionChanging != null)
                SelectionChanging(this, e);
        }

        #endregion
        #region SelectionChanged

        void Model_SelectionChanged(object sender, GridSelectionChangedEventArgs e)
        {
            if ((Model.Options.DrawSelectionOptions & (GridDrawSelectionOptions.ReplaceBackground | GridDrawSelectionOptions.ReplaceTextColor)) != 0)
            {
                foreach (GridRangeInfo range in Model.SelectedRanges)
                    InvalidateRenderCell(range);

                // just invalidate the backgrounds to mark it dirty, when grid is navigated horizontally
                // it takes the last column background and does a combine, this is because the CombinedCellBackground
                // collection is not refreshed with the new view

                //if (this.EnableRenderOptimization == Grid.EnableRenderOptimization.EnableOptimizations || EnableRenderOptimization == Grid.EnableRenderOptimization.None)
                {
                    var currentCellRange = this.CurrentCell.RangeInfo;
                    this.InvalidateCellBackground(currentCellRange.Top, currentCellRange.Left);
                }
            }
            RenderSelectedCells();

            this.OnSelectionChanged(e);
        }

        /// <summary>
        /// Invalidate cell so that PrepareRenderCell will get called again but not QueryCellStyle.
        /// </summary>
        /// <param name="range">The range of cells to invalidate.</param>
        public void InvalidateRenderCell(GridRangeInfo range)
        {
            CellSpanInfoBase span = range.ToCellSpan(Model);
            RenderStyles.Clear(span);
            base.InvalidateCell(span);
        }

        /// <summary>
        /// Occurs after the model updates its internal data structures when the model is in the process of selecting
        /// a range of cells.
        /// </summary>
        //public event GridSelectionChangedEventHandler SelectionChanged;
        public static RoutedEvent SelectionChangedEvent = EventManager.RegisterRoutedEvent("SelectionChanged", RoutingStrategy.Direct, typeof(GridSelectionChangedEventHandler), typeof(GridControlBase));

        /// <summary>
        /// Occurs after the model updates its internal data structures when the model is in the process of selecting
        /// a range of cells.
        /// </summary>
        public event GridSelectionChangedEventHandler SelectionChanged
        {
            add { this.AddHandler(GridControlBase.SelectionChangedEvent, value); }
            remove { this.RemoveHandler(GridControlBase.SelectionChangedEvent, value); }
        }

        protected virtual void OnSelectionChanged(GridSelectionChangedEventArgs e)
        {
            e.RoutedEvent = GridControlBase.SelectionChangedEvent;
            e.Source = this;
            base.RaiseEvent(e);

            //if (SelectionChanged != null)
            //{
            //    //e.OriginalSource = this;
            //    SelectionChanged(this, e);
            //}
        }

        #endregion
        #region SaveCellFormattedText

        void Model_SaveCellFormattedText(object sender, GridCellTextEventArgs e)
        {
            this.OnSaveCellFormattedText(e);
        }

        /// <summary>
        /// Occurs each time the <see cref="GridStyleInfo.FormattedText"/> is called to parse the formatted string that represents the underlying cell's value
        /// considering <see cref="GridStyleInfo.Format"/> and <see cref="GridStyleInfo.CellValueType"/>.
        /// </summary>
        public event GridCellTextEventHandler SaveCellFormattedText;
        protected virtual void OnSaveCellFormattedText(GridCellTextEventArgs e)
        {
            if (SaveCellFormattedText != null)
                SaveCellFormattedText(this, e);
        }

        #endregion
        #region RowsRemoved

        void Model_RowsRemoved(object sender, GridRangeRemovedEventArgs e)
        {
            this.OnRowsRemoved(e);
        }

        /// <summary>
        /// Occurs after a range of rows has been removed.
        /// See <see cref="GridRangeRemovedEventArgs"/> for more details.
        /// </summary>
        public event GridRangeRemovedEventHandler RowsRemoved;
        protected virtual void OnRowsRemoved(GridRangeRemovedEventArgs e)
        {
            if (RowsRemoved != null)
                RowsRemoved(this, e);
        }

        #endregion
        #region RowsMoved

        void Model_RowsMoved(object sender, GridRangeMovedEventArgs e)
        {
            this.OnRowsMoved(e);
        }

        /// <summary>
        /// Occurs after a range of rows is moved.
        /// See <see cref="GridRangeMovedEventArgs"/> for more details.
        /// </summary>
        public event GridRangeMovedEventHandler RowsMoved;
        protected virtual void OnRowsMoved(GridRangeMovedEventArgs e)
        {
            if (RowsMoved != null)
                RowsMoved(this, e);
        }

        #endregion
        #region RowsInserted

        void Model_RowsInserted(object sender, GridRangeInsertedEventArgs e)
        {
            this.OnRowsInserted(e);
        }

        /// <summary>
        /// Occurs after a range of rows has been inserted.
        /// See <see cref="GridRangeInsertedEventArgs"/> for more details.
        /// </summary>
        public event GridRangeInsertedEventHandler RowsInserted;
        protected virtual void OnRowsInserted(GridRangeInsertedEventArgs e)
        {
            if (RowsInserted != null)
                RowsInserted(this, e);
        }

        #endregion
        #region QueryCoveredRange

        void Model_QueryCoveredRange(object sender, GridQueryCoveredRangeEventArgs e)
        {
            this.OnQueryCoveredRange(e);
        }

        /// <summary>
        /// Occurs when the model queries information about covered cells at a specific cell.
        /// </summary>
        public event GridQueryCoveredRangeEventHandler QueryCoveredRange;
        protected virtual void OnQueryCoveredRange(GridQueryCoveredRangeEventArgs e)
        {
            if (QueryCoveredRange != null)
                QueryCoveredRange(this, e);
        }

        #endregion
        #region SaveCellText

        void Model_SaveCellText(object sender, GridCellTextEventArgs e)
        {
            this.OnSaveCellText(e);
        }

        /// <summary>
        /// Occurs each time the <see cref="GridStyleInfo.Text"/> is called to set the unformatted string that represents the underlying cell's value.
        /// </summary>
        public event GridCellTextEventHandler SaveCellText;
        protected virtual void OnSaveCellText(GridCellTextEventArgs e)
        {
            if (SaveCellText != null)
                SaveCellText(this, e);
        }

        #endregion
        #region QueryCellText

        void Model_QueryCellText(object sender, GridCellTextEventArgs e)
        {
            this.OnQueryCellText(e);
        }

        /// <summary>
        /// Occurs each time the <see cref="GridStyleInfo.Text"/> is called to get the raw string that represents the underlying cell's value.
        /// </summary>
        public event GridCellTextEventHandler QueryCellText;
        protected virtual void OnQueryCellText(GridCellTextEventArgs e)
        {
            if (QueryCellText != null)
                QueryCellText(this, e);
        }

        #endregion
        #region QueryCellSpanBackgrounds

        void Model_QueryCellSpanBackgrounds(object sender, GridQueryCellSpanBackgroundsEventArgs e)
        {
            this.OnQueryCellSpanBackgrounds(e);
        }

        /// <summary>
        /// Occurs when the model queries information about a cell spanned range at a specific cell.
        /// </summary>
        public event GridQueryCellSpanBackgroundsEventHandler QueryCellSpanBackgrounds;
        protected virtual void OnQueryCellSpanBackgrounds(GridQueryCellSpanBackgroundsEventArgs e)
        {
            if (QueryCellSpanBackgrounds != null)
                QueryCellSpanBackgrounds(this, e);
        }

        #endregion
        #region QueryCellModel

        void Model_QueryCellModel(object sender, GridQueryCellModelEventArgs e)
        {
            this.OnQueryCellModel(e);
        }

        /// <summary>
        /// Occurs when querying for a cell type.
        /// </summary>
        public event GridQueryCellModelEventHandler QueryCellModel;
        protected virtual void OnQueryCellModel(GridQueryCellModelEventArgs e)
        {
            if (QueryCellModel != null)
                QueryCellModel(this, e);
        }

        #endregion
        #region QueryCellInfo

        void Model_QueryCellInfo(object sender, GridQueryCellInfoEventArgs e)
        {
            this.OnQueryCellInfo(e);
        }

        /// <summary>
        /// Occurs when the model queries for style information about a specific cell.
        /// </summary>
        public event GridQueryCellInfoEventHandler QueryCellInfo;
        protected virtual void OnQueryCellInfo(GridQueryCellInfoEventArgs e)
        {
            if (QueryCellInfo != null)
                QueryCellInfo(this, e);
        }
        #endregion
        #region QueryCellFormattedText

        void Model_QueryCellFormattedText(object sender, GridCellTextEventArgs e)
        {
            this.OnQueryCellFormattedText(e);
        }

        /// <summary>
        /// Occurs each time the <see cref="GridStyleInfo.FormattedText"/> is called to get the formatted string that represents the underlying cell's value
        /// considering <see cref="GridStyleInfo.Format"/>.
        /// </summary>
        public event GridCellTextEventHandler QueryCellFormattedText;
        protected virtual void OnQueryCellFormattedText(GridCellTextEventArgs e)
        {
            if (QueryCellFormattedText != null)
                QueryCellFormattedText(this, e);
        }

        #endregion
        #region QueryBaseStyles

        void Model_QueryBaseStyles(object sender, GridQueryBaseStylesEventArgs e)
        {
            this.OnQueryBaseStyles(e);
        }

        /// <summary>
        /// Occurs when the model queries information about base styles at a specific cell.
        /// </summary>
        public event GridQueryBaseStylesEventHandler QueryBaseStyles;
        protected virtual void OnQueryBaseStyles(GridQueryBaseStylesEventArgs e)
        {
            if (QueryBaseStyles != null)
                QueryBaseStyles(this, e);
        }

        #endregion
        #region ParseCommonFormats

        void Model_ParseCommonFormats(object sender, GridCellTextEventArgs e)
        {
            this.OnParseCommonFormats(e);
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
        public event GridCellTextEventHandler ParseCommonFormats;
        protected virtual void OnParseCommonFormats(GridCellTextEventArgs e)
        {
            if (ParseCommonFormats != null)
                ParseCommonFormats(this, e);

        }

        #endregion
        #region CommittedCellInfo

        void Model_CommittedCellInfo(object sender, GridCommitCellInfoEventArgs e)
        {
            this.OnCommittedCellInfo(e);
        }

        /// <summary>
        /// Occurs when the model has saved style information about a specific cell.        
        /// </summary>
        public event GridCommitCellInfoEventHandler CommittedCellInfo;
        protected virtual void OnCommittedCellInfo(GridCommitCellInfoEventArgs e)
        {
            if (CommittedCellInfo != null)
                CommittedCellInfo(this, e);
        }

        #endregion
        #region CommitCellInfo

        void Model_CommitCellInfo(object sender, GridCommitCellInfoEventArgs e)
        {
            this.OnCommitCellInfo(e);
        }

        /// <summary>
        /// Occurs when the model is about to save style information about a specific cell.
        /// </summary>
        public event GridCommitCellInfoEventHandler CommitCellInfo;
        protected virtual void OnCommitCellInfo(GridCommitCellInfoEventArgs e)
        {
            if (CommitCellInfo != null)
                CommitCellInfo(this, e);
        }

        #endregion
        #region ColumnsRemoved

        void Model_ColumnsRemoved(object sender, GridRangeRemovedEventArgs e)
        {
            this.OnColumnsRemoved(e);
        }

        /// <summary>
        /// Occurs after a range of columns has been removed.
        /// See <see cref="GridRangeRemovedEventArgs"/> for more details.
        /// </summary>
        public event GridRangeRemovedEventHandler ColumnsRemoved;
        protected virtual void OnColumnsRemoved(GridRangeRemovedEventArgs e)
        {
            if (ColumnsRemoved != null)
                ColumnsRemoved(this, e);
        }

        #endregion
        #region ColumnsMoved

        void Model_ColumnsMoved(object sender, GridRangeMovedEventArgs e)
        {
            this.OnColumnsMoved(e);
        }

        /// <summary>
        /// Occurs after a range of columns is moved.
        /// See <see cref="GridRangeMovedEventArgs"/> for more details.
        /// </summary>
        public event GridRangeMovedEventHandler ColumnsMoved;
        protected virtual void OnColumnsMoved(GridRangeMovedEventArgs e)
        {
            if (ColumnsMoved != null)
                ColumnsMoved(this, e);
        }

        #endregion
        #region ColumnsInserted

        void Model_ColumnsInserted(object sender, GridRangeInsertedEventArgs e)
        {
            this.OnColumnsInserted(e);
        }

        /// <summary>
        /// Occurs after a range of columns has been inserted.
        /// See <see cref="GridRangeInsertedEventArgs"/> for more details.
        /// </summary>
        public event GridRangeInsertedEventHandler ColumnsInserted;
        protected virtual void OnColumnsInserted(GridRangeInsertedEventArgs e)
        {
            if (ColumnsInserted != null)
                ColumnsInserted(this, e);
        }

        #endregion
        #region CellModelsChanged

        void Model_CellModelsChanged(object sender, CollectionChangeEventArgs e)
        {
            this.OnCellModelsChanged(e);
        }

        /// <summary>
        /// Occurs when the CellModels collection is changed.
        /// </summary>
        public event CollectionChangeEventHandler CellModelsChanged;
        protected virtual void OnCellModelsChanged(CollectionChangeEventArgs e)
        {
            if (CellModelsChanged != null)
                CellModelsChanged(this, e);
        }

        #endregion
        #region BaseStylesMapChanged

        void Model_BaseStylesMapChanged(object sender, EventArgs e)
        {
            this.OnBaseStylesMapChanged(e);
        }

        /// <summary>
        /// Occurs when the GridModel.BaseStylesMap is changed.
        /// </summary>
        public event EventHandler BaseStylesMapChanged;
        protected virtual void OnBaseStylesMapChanged(EventArgs e)
        {
            if (BaseStylesMapChanged != null)
                BaseStylesMapChanged(this, e);
        }

        #endregion
        #endregion

        protected override Size MeasureOverride(Size constraint)
        {
            // GridModel model = Model; // possibly creates Model on demand and assign linesizes. Unused local variable
            return base.MeasureOverride(constraint);
        }

        void modelCommitCellInfo(object sender, GridCommitCellInfoEventArgs e)
        {
            if (e.Style.Store.IsValueModified(GridStyleInfoStore.BackgroundProperty))
                this.InvalidateCellBackground(e.Cell);
            if (e.Style.Store.IsValueModified(GridStyleInfoStore.BordersProperty))
                this.InvalidateCellBorder(e.Cell);
        }

        /// <summary>
        /// Gets the current cell.
        /// </summary>
        public GridCurrentCell CurrentCell
        {
            get { return currentCell; }
            // set { currentCell = value; }
        }

        /// <summary>
        /// Manages the collection of <see cref="GridCellRendererBase"/> objects for the current grid view
        /// method.
        /// </summary>
        /// <remarks>
        /// Cell renderers will be created on demand by calling the <see cref="GridCellModelBase.CreateRenderer"/>.
        /// Each renderer is associated with a <see cref="GridCellModelBase"/> object that holds its data and has
        /// knowledge how to instantiate a renderer and associates it with a grid view.
        /// <para/>
        /// A renderer is created for each grid view but
        /// renderers (of the same cell type) share the same <see cref="GridCellModelBase"/> instance even though they belong
        /// to different grid views.
        /// </remarks>
        /// <example>
        /// The following examples show how to get a reference to the renderer for a specific cell.
        /// <code lang="C#">
        ///             GridStyleInfo style = Model[rowIndex, colIndex];
        ///             GridCellRendererBase renderer = CellRenderers[style.CellType];
        /// </code>
        /// </example>
        public GridCellRendererCollection CellRenderers
        {
            get
            {
                if (cellRenderers == null)
                    cellRenderers = new GridCellRendererCollection(this);
                return cellRenderers;
            }
        }

        //public GridRangeInfo SelectedCells
        //{
        //    get
        //    {
        //        if (model == null)
        //            return GridRangeInfo.Empty;

        //        return Model.SelectedCells;
        //    }
        //    set
        //    {
        //        if (!CoveredCells.IsEmpty)
        //        {
        //            List<CoveredCellInfo> list = CoveredCells.SearchCellSpan(new CoveredCellInfo(value.Top, value.Left, value.Bottom, value.Right));
        //            foreach (CoveredCellInfo cc in list)
        //                value = GridRangeInfo.UnionRange(value, GridRangeInfo.Cells(cc.Top, cc.Left, cc.Bottom, cc.Right));
        //        }

        //        if (Model.SelectedCells != value)
        //        {
        //            Model.SelectedCells = value;

        //            RenderSelectedCells();
        //        }
        //    }
        //}

        /// <summary>
        /// Returns the covered cells.
        /// </summary>
        public GridCoveredCellInfoCollection CoveredCells
        {
            get
            {
                if (CoveredCellsProvider == null)
                    Model = new GridModel();

                return (GridCoveredCellInfoCollection)CoveredCellsProvider;
            }
        }

        
        public GridOverlappingCellInfoCollection ImageCells
        {
            get
            {
                if (OverlappingCellsProvider == null)
                    Model = new GridModel();

                return (GridOverlappingCellInfoCollection)OverlappingCellsProvider;
            }
        }

        /// <summary>
        /// Returns the cell spanned backgrounds.
        /// </summary>
        public GridCellSpanBackgroundInfoCollection CellSpanBackgrounds
        {
            get
            {
                if (CellSpanBackgroundsProvider == null)
                    Model = new GridModel();

                return (GridCellSpanBackgroundInfoCollection)CellSpanBackgroundsProvider;
            }
        }

        /// <summary>
        /// Returns the row heights for the grid.
        /// </summary>
        public IEditableLineSizeHost RowHeights
        {
            get
            {
                if (RowHeightsProvider == null || RowHeightsProvider is EmptyLineSizeHost)
                    Model = new GridModel();

                return (IEditableLineSizeHost)RowHeightsProvider;
            }
        }

        /// <summary>
        /// Returns the column widths for the grid.
        /// </summary>
        public IEditableLineSizeHost ColumnWidths
        {
            get
            {
                if (ColumnWidthsProvider == null || ColumnWidthsProvider is EmptyLineSizeHost)
                    Model = new GridModel();

                return (IEditableLineSizeHost)ColumnWidthsProvider;
            }
        }

        /// <summary>
        /// Get or set a value that is used to check whether the frozen rows needs to reset the hidden rows/columns to avoid resetting the previous hidden rows/columns
        /// </summary>
        public bool ResetHiddenWhenFreezing
        {
            get
            {
                return resethiddenwhenfreezing;
            }
            set
            {
                resethiddenwhenfreezing = value;
            }
        }

        /// <summary>
        /// Gets or sets the frozen row count.
        /// </summary>
        public int FrozenRows
        {
            get
            {
                return RowHeights.HeaderLineCount;
            }
            set
            {
                if (resethiddenwhenfreezing)
                {
                    RowHeights.SetHidden(1, Model.RowCount - 1, false);
                    if (value > TopRowIndex && TopRowIndex > 0)
                        RowHeights.SetHidden(1, TopRowIndex - 1, true);
                }
                RowHeights.HeaderLineCount = value;
            }
        }

        /// <summary>
        /// Gets or sets the frozen column count.
        /// </summary>
        public int FrozenColumns
        {
            get
            {
                return ColumnWidths.HeaderLineCount;
            }
            set
            { 
                if (resethiddenwhenfreezing)
                {
                    if(value != ScrollColumns.HeaderLineCount)
                    ColumnWidths.SetHidden(ScrollColumns.HeaderLineCount, Model.ColumnCount - 1, false);
                    if (value > LeftColumnIndex && LeftColumnIndex > ScrollColumns.HeaderLineCount)
                        ColumnWidths.SetHidden(ScrollColumns.HeaderLineCount, LeftColumnIndex - ScrollColumns.HeaderLineCount, true);
                }
                ColumnWidths.HeaderLineCount = value;
            }
        }

        /// <summary>
        /// Gets or sets the footer row count.
        /// </summary>
        public int FooterRows
        {
            get
            {
                return RowHeights.FooterLineCount;
            }
            set
            {
                RowHeights.FooterLineCount = value;
            }
        }

        /// <summary>
        /// Gets or sets the footer columns count.
        /// </summary>
        public int FooterColumns
        {
            get
            {
                return ColumnWidths.FooterLineCount;
            }
            set
            {
                ColumnWidths.FooterLineCount = value;
            }
        }

        #endregion

        #region RenderSelectedCells, RenderCurrentCellBorder

#if !TestDrawTextPerformance
        protected override void OnRender(DrawingContext dc)
        {
            RenderCurrentCellBorder();
            RenderHiddenCellBorder();
            base.OnRender(dc);
        }
#else
        int n=0;
        GridRenderStyleInfo style;

        protected override void OnRender(DrawingContext dc)
        {
            base.OnRender(dc);
            n++;
        }
        
        protected override void OnRenderCell(DrawingContext dc, RenderCellArgs rca)
        {
            if (rca.CellRect.IsEmpty)
                return;

            GridTextBoxPaint.DrawText(dc, rca.CellRect, n.ToString(), style);
        }
#endif

        protected override void OnArrangeContent(Size arrangeSize)
        {
            if (CurrentCell.IsEditing)
            {
                CurrentCell.CreateCurrentCellUIElements();
            }
            if (IndividualCellBackgroundsToDraw != null)
            {
                IndividualCellBackgroundsToDraw.Clear();
            }

            base.OnArrangeContent(arrangeSize);
            if (this.Model.GraphicModel != null && this.Model.GraphicModel.GraphicCells.Count > 0)
            {
                this.Model.GraphicModel.ArrangeGraphicCells();
            }
        }
      
        /// <summary>
        /// Gets the cell background for a cell from the <see cref="IRenderCellInfo"/> cell style.
        /// </summary>
        /// <param name="ci">The cell style.</param>
        /// <returns></returns>
        protected override Brush GetCellBackground(IRenderCellInfo ci, bool combineBackgrounds)
        {
            GridRenderStyleInfo style = (GridRenderStyleInfo)ci;
            // If background stored in CellInfo is not a brush and instead a description of a brush (e.g. GridBrushInfo)
            // this method is a good place to convert that to a brush and cache it for reuse in other cells.
            if (combineBackgrounds && this is GridControl && style.HasConditionalFormat)
            {
                return Brushes.Transparent;
            }

            if (combineBackgrounds)
                return style.ModelStyle.Background;

            return style.Background;
        }

        //protected override void OnRenderCell(DrawingContext dc, RenderCellArgs rca)
        //{
        //    if (rca.CellRect.IsEmpty)
        //        return;

        //    GridRenderStyleInfo style = (GridRenderStyleInfo)rca.CellInfo;
        //    bool backgroundDrawn = GetCellBackground(style) == style.Background;
        //    if (!backgroundDrawn)
        //        dc.DrawRectangle(style.Background, null, rca.CellRect);

        //    base.OnRenderCell(dc, rca);
        //}

        /// <summary>
        /// Expands the range of selected cells.
        /// </summary>
        /// <param name="selectedCells">The selected cells range.</param>
        /// <returns>Expanded range of selected cells.</returns>
        public virtual GridRangeInfo ExpandSelectedCellsRange(GridRangeInfo selectedCells)
        {
            return selectedCells.ExpandRange(Model.HeaderRows, Model.HeaderColumns, Model.RowCount, Model.ColumnCount);
        }

        protected virtual void RenderSelectedCells()
        {
            DrawingContext dccBackground = dvSelectBackground.RenderOpen();

            if ((Model.Options.DrawSelectionOptions & GridDrawSelectionOptions.AlphaBlend) != 0)
            {
                GridRangeInfoList savedRanges = new GridRangeInfoList();

                // Rect gridRect = new Rect(0, 0, RenderSize.Width, RenderSize.Height); Unused local variable
                for (int n = 0; n < Model.SelectedRanges.Count; n++)
                {
                    GridRangeInfo selectedCells = Model.SelectedRanges[n];

                    if (!selectedCells.IsEmpty)
                    {
                        selectedCells = this.ExpandSelectedCellsRange(selectedCells);

                        // Check for overlapping ranges, avoid them being drawn darker.
                        bool drawCellsIndividually = (Model.Options.DrawSelectionOptions & GridDrawSelectionOptions.OverlapWithDarkenedAlphaBlend) == 0
                            && savedRanges.AnyRangeIntersects(selectedCells);

                        DoubleSpan[] yPos = ScrollRows.RangeToRegionPoints(selectedCells.Top, selectedCells.Bottom, true);
                        DoubleSpan[] xPos = ScrollColumns.RangeToRegionPoints(selectedCells.Left, selectedCells.Right, true);

                        GridRangeInfo currentCellRange = Model.Options.ShowCurrentCell ? CurrentCell.RangeInfo : GridRangeInfo.Empty;
                        CoveredCellInfo cc = CoveredCells.GetCellSpan(CurrentCell.RowIndex, CurrentCell.ColumnIndex);
                        if (cc != null)
                            currentCellRange = GridRangeInfo.Cells(cc.Top, cc.Left, cc.Bottom, cc.Right);

                        DoubleSpan[] yCurrentCellPos = ScrollRows.RangeToRegionPoints(currentCellRange.Top, currentCellRange.Bottom, true);
                        DoubleSpan[] xCurrentCellPos = ScrollColumns.RangeToRegionPoints(currentCellRange.Left, currentCellRange.Right, true);

                        for (int rowRegion = 0; rowRegion < 3; rowRegion++)
                        {
                            bool selectionMade = false;

                            if (yPos[rowRegion].IsEmpty)
                                continue;

                            for (int columnRegion = 0; columnRegion < 3; columnRegion++)
                            {
                                if (xPos[columnRegion].IsEmpty)
                                    continue;

                                Rect clipRect = GetClipRect((ScrollAxisRegion)rowRegion, (ScrollAxisRegion)columnRegion);
                                Rect r = new Rect(xPos[columnRegion].Start, yPos[rowRegion].Start, xPos[columnRegion].Length, yPos[rowRegion].Length);

                                if (!clipRect.IntersectsWith(r))
                                    continue;

                                // Background
                                Rect currentCellRect = Rect.Empty;
                                if (!xCurrentCellPos[columnRegion].IsEmpty && !yCurrentCellPos[rowRegion].IsEmpty)
                                {
                                    // Exclude current cell
                                    if (!ShouldPushClip())
                                    {
                                        clipRect = this.RangeToClippedVisibleRect(selectedCells);
                                        currentCellRect = this.RangeToClippedVisibleRect(currentCellRange);

                                        if(currentCellRect.IsEmpty)
                                            currentCellRect = new Rect(xCurrentCellPos[columnRegion].Start, yCurrentCellPos[rowRegion].Start, xCurrentCellPos[columnRegion].Length, yCurrentCellPos[rowRegion].Length);

                                        //if (this.InternalGetHeaderCols() == 0 && this.InternalGetHeaderRows() == 0)
                                        //{
                                        //    clipRect = this.RangeToClippedVisibleRect(GridRangeInfo.Table());
                                        //}

                                        //if (this.InternalGetHeaderCols() == 0 && this.InternalGetHeaderRows() > 0)
                                        //{
                                        //    clipRect = this.RangeToClippedVisibleRect(GridRangeInfo.Cells(this.InternalGetHeaderRows() - 1, 0, this.Model.RowCount, this.Model.ColumnCount));
                                        //}

                                        //if (this.InternalGetHeaderCols() > 0 && this.InternalGetHeaderRows() == 0)
                                        //{
                                        //    clipRect = this.RangeToClippedVisibleRect(GridRangeInfo.Cells(0, this.InternalGetHeaderCols() - 1, this.Model.RowCount, this.Model.ColumnCount));
                                        //}
                                    }
                                    else
                                    {
                                        currentCellRect = new Rect(xCurrentCellPos[columnRegion].Start, yCurrentCellPos[rowRegion].Start, xCurrentCellPos[columnRegion].Length, yCurrentCellPos[rowRegion].Length);
                                    }

                                    PushClip(dccBackground, clipRect, currentCellRect);
                                }
                                else
                                {
                                    clipRect = this.RangeToClippedVisibleRect(selectedCells);
                                    currentCellRect = this.RangeToClippedVisibleRect(currentCellRange);
                                    PushClip(dccBackground, clipRect, currentCellRect);
                                }

                                if (!drawCellsIndividually)
                                {
                                    DrawSelectedCellsRectangle(dccBackground, r);
                                    if (!ShouldPushClip())
                                    {
                                        selectionMade = true;
                                    }
                                }
                                else
                                {
                                    Int32Span regionRows = ScrollRows.GetVisibleLinesRange(rowRegion);
                                    Int32Span regionColumns = ScrollColumns.GetVisibleLinesRange(columnRegion);
                                    GridRangeInfo regionRange = GridRangeInfo.Cells(regionRows.Start, regionColumns.Start, regionRows.End, regionColumns.End);

                                    GridRangeInfo range = selectedCells.IntersectRange(regionRange);
                                    if (range.IsEmpty)
                                        continue;

                                    for (int rowIndex = range.Top; rowIndex <= range.Bottom; rowIndex++)
                                    {
                                        for (int columnIndex = range.Left; columnIndex <= range.Right; columnIndex++)
                                        {
                                            GridRangeInfo cell = GridRangeInfo.Cell(rowIndex, columnIndex);
                                            if (savedRanges.AnyRangeContains(cell))
                                                continue;

                                            r = RangeToClippedVisibleRect(cell);
                                            DrawSelectedCellsRectangle(dccBackground, r);
                                        }
                                    }
                                }

                                if (!currentCellRect.IsEmpty)
                                    dccBackground.Pop();

                                if (!ShouldPushClip() && selectionMade)
                                {
                                    break;
                                }
                            }

                            if (!ShouldPushClip() && selectionMade)
                            {
                                break;
                            }
                        }

                        savedRanges.Add(selectedCells);
                    }
                }                
            }
            dccBackground.Close();

            RenderActiveRangeBorder();
        }

        protected virtual void DrawSelectedCellsRectangle(DrawingContext dccBackground, Rect r)
        {
            dccBackground.DrawRectangle(Model.Options.HighlightSelectionAlphaBlend, null, r);
        }

        internal enum GridSelectionMarkerLocation
        {
            /// <summary>
            /// Represents None
            /// </summary>
            None = 0,

            /// <summary>
            /// Represents Default
            /// </summary>
            Default = 1,

            /// <summary>
            /// Represents Top
            /// </summary>
            Top = 2,

            /// <summary>
            /// Represents Left
            /// </summary>
            Left = 3
        }

        protected override void RenderCellBorders(DrawingContext dc)
        {
            base.RenderCellBorders(dc);
            if (Model.Options.ExcelLikeFreezePane)
            {
                int headerColumn = (ScrollColumns.HeaderLineCount - 1);
                int headerRow = (ScrollRows.HeaderLineCount - 1);
                VisibleLinesCollection visiblelinesInColumn = ScrollColumns.GetVisibleLines();
                VisibleLinesCollection visiblelinesInRow = ScrollRows.GetVisibleLines();
                Pen frozenBorder = new Pen(Brushes.Black, 1.0);
                if (headerColumn > 0 && headerColumn < visiblelinesInColumn.Count)
                {
                    VisibleLineInfo visibleHeader = visiblelinesInColumn[headerColumn];
                    while (!visibleHeader.IsHeader)
                    {
                        headerColumn--;
                        if (headerColumn > 0)
                            visibleHeader = visiblelinesInColumn[headerColumn];
                    }
                    VisibleLineInfo firstHeader = visiblelinesInRow[0];
                    VisibleLineInfo lastHeader = visiblelinesInRow[visiblelinesInRow.LastBodyVisibleIndex];
                    Rect r1 = GridUtil.FromLTRB(Math.Floor(visibleHeader.ClippedOrigin), Math.Floor(firstHeader.ClippedOrigin), Math.Floor(visibleHeader.ClippedCorner), Math.Floor(lastHeader.ClippedCorner));
                    OnRenderBorder(dc, r1, r1, CellBorderSide.Right, frozenBorder);
                }
                if (headerRow > 0 && headerRow < visiblelinesInRow.Count)
                {
                    VisibleLineInfo visibleHeader = visiblelinesInRow[headerRow];
                    while (!visibleHeader.IsHeader)
                    {
                        headerRow--;
                        if (headerRow > 0)
                            visibleHeader = visiblelinesInRow[headerRow];
                    }
                    VisibleLineInfo firstHeader = visiblelinesInColumn[0];
                    VisibleLineInfo lastHeader = visiblelinesInColumn[visiblelinesInColumn.LastBodyVisibleIndex];
                    Rect r2 = GridUtil.FromLTRB(Math.Floor(firstHeader.ClippedOrigin), Math.Floor(visibleHeader.ClippedOrigin), Math.Floor(lastHeader.ClippedCorner), Math.Floor(visibleHeader.ClippedCorner));
                    OnRenderBorder(dc, r2, r2, CellBorderSide.Bottom, frozenBorder);
                }
            }
        }

        #region FloatingCellImplementation

        /// <summary>
        /// Gets the required range for Overlapping cell support
        /// </summary>
        /// <param name="cell">The cell.</param>
        /// <returns></returns>
        private GridRangeInfo GetRequiredRange(RowColumnIndex cell)
        {
            double threshHold = 7;
            GridRangeInfo range = GridRangeInfo.Cell(cell.RowIndex, cell.ColumnIndex);
            GridStyleInfo style = RenderStyles.GetRenderStyleInfo(cell.RowIndex, cell.ColumnIndex);
            string text = style.FormattedText;
            Rect cellRect = this.RangeToRect(ScrollAxisRegion.Body, ScrollAxisRegion.Body, range, false, false);
            Rect textRect = SubtractBorderMargins(cellRect, style.TextMargins.ToThickness());
            CultureInfo cultureInfo = style.GetCulture(true);
            FlowDirection flowDirection = style.FlowDirection;
            Typeface typeface = style.Typeface;
            double emSize = style.ReadOnlyFont.FontSize;
            // double lineHeight = style.ReadOnlyFont.GetLineHeightValue(); Unused local variable
            Brush foreground = style.Foreground;
            FormattedText formattedText = new FormattedText(text, cultureInfo, flowDirection, typeface, emSize, foreground);
            int colIndex = cell.ColumnIndex;
            if (style.HorizontalAlignment == System.Windows.HorizontalAlignment.Right)
                colIndex = colIndex - 1;
            else
                colIndex = colIndex + 1;
            while (formattedText.Width + threshHold > textRect.Width && colIndex < this.Model.ColumnCount&&colIndex > 0)
            {
                //Console.WriteLine("Column Index : ", colIndex);
                if (this.Model[cell.RowIndex, colIndex].FormattedText == ""
                    && (this.Model.Options.FloodCell
                    || this.Model[cell.RowIndex, colIndex].FloodCell))
                {
                    string internalcelltype = this.Model[cell.RowIndex, cell.ColumnIndex].CellType;
                    string nextcelltype = this.Model[cell.RowIndex, colIndex].CellType;
                    if ((internalcelltype == "TextBox" || internalcelltype == "TextBlock" || internalcelltype == "FormulaCell"
                        || internalcelltype == "Static" || nextcelltype == "CurrencyEdit" || nextcelltype == "MaskEdit" 
                        || internalcelltype == "PercentEdit" || internalcelltype == "DoubleEdit" || internalcelltype == "Hyperlink")
                        && (nextcelltype == "TextBox" || nextcelltype == "TextBlock" || nextcelltype == "FormulaCell" 
                        || nextcelltype == "Static" || nextcelltype == "CurrencyEdit" || nextcelltype == "MaskEdit" 
                        || nextcelltype == "PercentEdit" || nextcelltype == "DoubleEdit"|| internalcelltype == "Hyperlink"))
                    {
                        if (style.HorizontalAlignment == System.Windows.HorizontalAlignment.Right)
                            range = GridRangeInfo.Cells(cell.RowIndex, colIndex, cell.RowIndex, cell.ColumnIndex);
                        else
                            range = GridRangeInfo.Cells(cell.RowIndex, cell.ColumnIndex, cell.RowIndex, colIndex);
                        cellRect = this.RangeToRect(ScrollAxisRegion.Body, ScrollAxisRegion.Body, range, false, false);
                        textRect = SubtractBorderMargins(cellRect, style.TextMargins.ToThickness());
                    }
                    if (style.HorizontalAlignment == System.Windows.HorizontalAlignment.Right)
                        colIndex--;
                    else
                        colIndex++;
                }
                else
                    break;
            }
            return range;
        }

        /// <summary>
        /// Subtracts the border margins for Overlapping cells will be called from GetRequiredRange
        /// </summary>
        /// <param name="cellRect">The cell rect.</param>
        /// <param name="mi">The mi.</param>
        /// <returns></returns>
        private Rect SubtractBorderMargins(Rect cellRect, Thickness mi)
        {
            if (cellRect.IsEmpty || cellRect.Width <= mi.Left + mi.Right || cellRect.Height <= mi.Top + mi.Bottom)
                return new Rect(0, 0, 0, 0);

            cellRect.Height -= mi.Bottom + mi.Top;
            cellRect.Y += mi.Top;
            cellRect.Width -= mi.Right + mi.Left;
            cellRect.X += mi.Left;

            return cellRect;
        }

        /// <summary>
        /// Renders a row of cells. The method calls <see cref="OnRenderCell"/> for each cell. OnRenderCell gets
        /// the <see cref="ICellRenderer"/> for a cell and calls its <see cref="ICellRenderer.Render"/>
        /// method.
        /// </summary>
        /// <param name="dc">The drawing context.</param>
        /// <param name="visibleRow">The visible row.</param>
        /// <param name="corner">The corner which is the point after the last visible row and column
        /// of the body region.</param>
        protected override void RenderRow(DrawingContext dc, VisibleLineInfo visibleRow, Point corner)
        {
            if (this.Model.Options.FloatCellMode != GridFloatCellsMode.OnDemandCalculation)
            {
                base.RenderRow(dc, visibleRow, corner);
                return;
            }

            ///Below code will run only when Overlapping cell enabled.
            int rowIndex = visibleRow.LineIndex;
            Rect cellRect = new Rect(0, visibleRow.Origin, ScrollColumns.ViewSize, visibleRow.Size);

            VisibleLinesCollection visibleColumns = ScrollColumns.GetVisibleLines();
            foreach (VisibleLineInfo visibleColumn in visibleColumns)
            {
                if (GetVisibleCoveredCell(visibleRow, visibleColumn) != null)
                    continue;

                if (GetVisibleOverlappingCell(visibleRow, visibleColumn) != null)
                    continue;
                int columnIndex = visibleColumn.LineIndex;
                var style = RenderStyles.GetRenderStyleInfo(rowIndex, columnIndex);
                if (((this.Model.Options.FloatCellMode == GridFloatCellsMode.OnDemandCalculation && style.TextWrapping == TextWrapping.NoWrap)) && (columnIndex != 0 || Model.HeaderColumns == 0))
                {
                    if (floatcellran.ContainsKey(new RowColumnIndex(rowIndex, columnIndex)))
                    {
                        floatcellran.Remove(new RowColumnIndex(rowIndex, columnIndex));
                        shouldrefresh = true;
                    }
                    GridRangeInfo range = GetRequiredRange(new RowColumnIndex(rowIndex, columnIndex));
                    cellRect.X = visibleColumn.Origin;
                    cellRect.Width = this.RangeToRect(ScrollAxisRegion.Body, ScrollAxisRegion.Body, range, true, true).Width;
                    if (range.Left != range.Right)
                    {
                        VisibleLineInfo OriginvisibleColumn = visibleColumns.GetVisibleLineAtLineIndex(range.Left);
                        if (OriginvisibleColumn != null)
                            cellRect.X = OriginvisibleColumn.Origin;
                        if (floatcellran.ContainsKey(new RowColumnIndex(rowIndex, columnIndex)))
                            floatcellran[new RowColumnIndex(rowIndex, columnIndex)] = range.Right;
                        else
                            floatcellran.Add(new RowColumnIndex(rowIndex, columnIndex), range.Right);
                        shouldrefresh = true;
                    }
                }
                else if (floatcellran.ContainsKey(new RowColumnIndex(rowIndex, columnIndex)))
                {
                    floatcellran.Remove(new RowColumnIndex(rowIndex, columnIndex));
                    cellRect.X = visibleColumn.Origin;
                    cellRect.Width = visibleColumn.Size;
                }
                else
                {
                    cellRect.X = visibleColumn.Origin;
                    cellRect.Width = visibleColumn.Size;
                }
                IRenderCellInfo renderCellInfo = InternalGetCellInfo(visibleRow, visibleColumn);
                RenderCellArgs rca = new RenderCellArgs(this, visibleRow, visibleColumn, cellRect, renderCellInfo);
                RenderCell(dc, rca);
            }
        }

        bool shouldrefresh;
        protected override void RenderCells(DrawingContext dc)
        {
            base.RenderCells(dc);
            if (shouldrefresh)
            {
                ArrangeCellBorders(new Size());
                shouldrefresh = false;
            }
        }
        #endregion

        protected override void OnRenderCell(DrawingContext dc, RenderCellArgs rca)
        {
            var style = (GridStyleInfo)rca.CellInfo;
            
            if (!this.IsInArrangeContent && this.IndividualCellBackgroundsToDraw.Contains(rca.CellRowColumnIndex))
            {
                Thickness margins = style.BorderMargins.ToThickness();

                Rect rect = rca.SubtractBorderMargins(rca.CellRect, margins);

                dc.DrawRectangle(rca.CellInfo.GetCellBackground() as Brush, null, rect);

                OnRenderBorder(dc, rca.CellRect, rca.CellClipRect, CellBorderSide.Top, style.Borders.Top);
                OnRenderBorder(dc, rca.CellRect, rca.CellClipRect, CellBorderSide.Right, style.Borders.Right);
                OnRenderBorder(dc, rca.CellRect, rca.CellClipRect, CellBorderSide.Bottom, style.Borders.Bottom);
                OnRenderBorder(dc, rca.CellRect, rca.CellClipRect, CellBorderSide.Left, style.Borders.Left);
                //this.IndividualCellBackgroundsToDraw.Remove(rca.CellRowColumnIndex);
            }
            base.OnRenderCell(dc, rca);
            ICellRenderer renderer = GetCellRenderer(rca.CellInfo);
            //var style = (GridStyleInfo)rca.CellInfo;
            if (renderer != null)
            {
                IAllowInvalidateCell aic = style as IAllowInvalidateCell;
                if (aic != null)
                {
                    bool b = aic.AllowInvalidateCell;
                    aic.AllowInvalidateCell = false;
                    var args = new GridCellRenderEventArgs(style, rca, dc, GridControlBase.CellRenderedEvent, this);
                    this.RaiseGridCellRenderEvent(args);
                    if (!args.Handled)
                    {
                        if (style.HasImageIndex)
                        {
                            this.RenderCellImage(dc, args.Style, rca.CellRect);
                        }
                        else
                        {
                            this.RenderCellErrorInfo(dc, args.Style, rca.CellRect);
                        }

                        if (style.Comment != null || style.CommentTemplateKey != null || style.HasGridCommentStyleInfo)
                        {
                            this.RenderCellComment(dc, args.Style, rca.OriginalCellRect);
                        }
                    }
                    aic.AllowInvalidateCell = b;
                }

                this.RaiseGridCellRenderEvent(new GridCellRenderEventArgs(style, rca, dc, GridControlBase.CellRenderedEvent, this));
            }
        }

        protected virtual void RenderCellComment(DrawingContext dc, GridStyleInfo style, Rect cellRect)
        {
            // Will only get hit if SupportsRenderOptimization is true, otherwise rca.CellUIElements is never null.
            // Thickness margins = style.TextMargins.ToThickness(); Unused local variable

            Rect textRectangle = cellRect;

            if (textRectangle.IsEmpty)
            {
                return;
            }

            double top = textRectangle.Top;
            double right = textRectangle.Right;
            double bottom = textRectangle.Bottom;
            double left = textRectangle.Left;

            if (!style.HasGridCommentStyleInfo)
            {
                if (style.CommentAlignment == CommentAlignment.TopRight)
                { 
                    dc.DrawLine(new Pen(style.CommentBrush, 2), new Point(right, top + 5), new Point(right - 5, top));
                    dc.DrawLine(new Pen(style.CommentBrush, 2), new Point(right, top + 3), new Point(right - 3, top));
                    dc.DrawLine(new Pen(style.CommentBrush, 2), new Point(right, top + 1), new Point(right - 1, top));
                }
                if (style.CommentAlignment == CommentAlignment.TopLeft)
                {
                    dc.DrawLine(new Pen(style.CommentBrush, 2), new Point(left + 5, top), new Point(left, top + 5));
                    dc.DrawLine(new Pen(style.CommentBrush, 2), new Point(left + 3, top), new Point(left, top + 3));
                    dc.DrawLine(new Pen(style.CommentBrush, 2), new Point(left + 1, top), new Point(left, top + 1));
                }
                if (style.CommentAlignment == CommentAlignment.BottomLeft)
                {
                    dc.DrawLine(new Pen(style.CommentBrush, 2), new Point(left + 5, bottom), new Point(left, bottom - 5));
                    dc.DrawLine(new Pen(style.CommentBrush, 2), new Point(left + 3, bottom), new Point(left, bottom - 3));
                    dc.DrawLine(new Pen(style.CommentBrush, 2), new Point(left + 1, bottom), new Point(left, bottom - 1));
                }
                if (style.CommentAlignment == CommentAlignment.BottomRight)
                {
                    dc.DrawLine(new Pen(style.CommentBrush, 2), new Point(right, bottom - 5), new Point(right - 5, bottom));
                    dc.DrawLine(new Pen(style.CommentBrush, 2), new Point(right, bottom - 3), new Point(right - 3, bottom));
                    dc.DrawLine(new Pen(style.CommentBrush, 2), new Point(right, bottom - 1), new Point(right - 1, bottom));
                }
            }
            else
            {
                if (style.GridCommentStyleInfo.TopRightComment != null || style.GridCommentStyleInfo.TopRightCommentTemplateKey != null)
                {
                    dc.DrawLine(new Pen(style.GridCommentStyleInfo.TopRightCommentBrush, 2), new Point(right, top + 5), new Point(right - 5, top));
                    dc.DrawLine(new Pen(style.GridCommentStyleInfo.TopRightCommentBrush, 2), new Point(right, top + 3), new Point(right - 3, top));
                    dc.DrawLine(new Pen(style.GridCommentStyleInfo.TopRightCommentBrush, 2), new Point(right, top + 1), new Point(right - 1, top));
                }
                if (style.GridCommentStyleInfo.TopLeftComment != null || style.GridCommentStyleInfo.TopLeftCommentTemplateKey != null)
                {
                    dc.DrawLine(new Pen(style.GridCommentStyleInfo.TopLeftCommentBrush, 2), new Point(left + 5, top), new Point(left, top + 5));
                    dc.DrawLine(new Pen(style.GridCommentStyleInfo.TopLeftCommentBrush, 2), new Point(left + 3, top), new Point(left, top + 3));
                    dc.DrawLine(new Pen(style.GridCommentStyleInfo.TopLeftCommentBrush, 2), new Point(left + 1, top), new Point(left, top + 1));
                }
                if (style.GridCommentStyleInfo.BottomLeftComment != null || style.GridCommentStyleInfo.BottomLeftCommentTemplateKey != null)
                {
                    dc.DrawLine(new Pen(style.GridCommentStyleInfo.BottomLeftCommentBrush, 2), new Point(left + 5, bottom), new Point(left, bottom - 5));
                    dc.DrawLine(new Pen(style.GridCommentStyleInfo.BottomLeftCommentBrush, 2), new Point(left + 3, bottom), new Point(left, bottom - 3));
                    dc.DrawLine(new Pen(style.GridCommentStyleInfo.BottomLeftCommentBrush, 2), new Point(left + 1, bottom), new Point(left, bottom - 1));
                }
                if (style.GridCommentStyleInfo.BottomRightComment != null || style.GridCommentStyleInfo.BottomRightCommentTemplateKey != null)
                {
                    dc.DrawLine(new Pen(style.GridCommentStyleInfo.BottomRightCommentBrush, 2), new Point(right, bottom - 5), new Point(right - 5, bottom));
                    dc.DrawLine(new Pen(style.GridCommentStyleInfo.BottomRightCommentBrush, 2), new Point(right, bottom - 3), new Point(right - 3, bottom));
                    dc.DrawLine(new Pen(style.GridCommentStyleInfo.BottomRightCommentBrush, 2), new Point(right, bottom - 1), new Point(right - 1, bottom));
                }
            }
        }

        protected virtual void RenderCellImage(DrawingContext dc, GridStyleInfo style, Rect cellRect)
        {
            if (!style.HasImageIndex && (!style.HasImageList || Model.ColStyles[style.ColumnIndex].ImageList.Count == 0))
            {
                return;
            }
            if (style.ImageList == null || style.ImageIndex >= style.ImageList.Count)
                return;

            var image = style.ImageList[style.ImageIndex];
            var imgRect = Rect.Empty;
            var imageContentAlignment = style.ReadOnlyImageContentAlignment;
            var imageContentStretch = style.ReadOnlyImageContentStretch;
            var width =  0.0; 
            var height = 0.0; 

            switch (imageContentStretch)
            {
                case ImageContentStretch.Fill:
                    width = style.GetImageWidth() * cellRect.Width;
                    height = style.GetImageHeight() * cellRect.Height;
                    break;
                case ImageContentStretch.Uniform:
                    width = style.GetImageWidth() * this.Model.ColumnWidths.GetDefaultLineSize();
                    height = style.GetImageHeight() * this.Model.RowHeights.GetDefaultLineSize();
                    break;
                case ImageContentStretch.Absolute:
                    width = style.GetImageWidth();
                    height = style.GetImageHeight();
                    break;
            }

            if (imageContentAlignment == ImageContentAlignment.Left)
            {
                imgRect = new Rect(cellRect.X, cellRect.Y, width, height);
            }
            else if (imageContentAlignment == ImageContentAlignment.Right)
            {
                imgRect = new Rect(cellRect.X + (cellRect.Width - width), cellRect.Y, width, height);
            }

            if (imgRect != Rect.Empty)
            {
                imgRect = style.AdjustImageMargins(imgRect);
                dc.DrawImage(image.Source, imgRect);
            }
        }

        protected virtual void RenderCellErrorInfo(DrawingContext dc, GridStyleInfo style, Rect cellRect)
        {
            if (style.HasErrorInfo && style.ErrorInfo.HasErrorMessage)
            {
                var contentAlignment = style.ErrorInfo.ErrorContentAlignment;
                Rect imgRect = Rect.Empty;
                // var width = style.ErrorInfo.GetImageWidth() * cellRect.Width; Unused local variable
                var height = style.ErrorInfo.GetImageHeight() * cellRect.Height;
                if (contentAlignment == ImageContentAlignment.Left)
                {
                    //imgRect = new Rect(cellRect.X, cellRect.Y, GridTooltipService.ErrorImageBounds, cellRect.Height);
                    var xWidth = (cellRect.Width < 20 ? cellRect.Width : 20); // Previous code <==  var xWidth = (width < 30 ? width : 30); ==> this is changed to avoid the error icon to shrink before it reaches the left end and allow to overlap on text.
                    if (!(this.CurrentCell.IsEditing && this.CurrentCell.CellRowColumnIndex == style.CellRowColumnIndex && (style.CellType == "ComboBox" || style.CellType == "DropDownList")))
                    {
                        imgRect = new Rect(cellRect.X, cellRect.Y, xWidth, height);
                    }
                }
                else if (contentAlignment == ImageContentAlignment.Right)
                {
                    //imgRect = new Rect(cellRect.X + (cellRect.Width - (GridTooltipService.ErrorImageBounds + 2)), cellRect.Y, GridTooltipService.ErrorImageBounds, cellRect.Height);
                    // we fix the width to 30d if the columns are resized
                    //var xWidth = (cellRect.Width < 20 ? cellRect.Width : 20); // Previous code <==  var xWidth = (width < 30 ? width : 30); ==> this is changed to avoid the error icon to shrink before it reaches the left end and allow to overlap on text.
                    double xWidth = 0d;
                    double combowidth = 0d;

                    if (style.CellType == "ComboBox" || style.CellType == "DropDownList")
                    {
                        combowidth = (cellRect.Width < 5 ? cellRect.Width : 5);
                        xWidth = (cellRect.Width < 30 ? cellRect.Width : 30);
                    }
                    else
                    {
                        xWidth = (cellRect.Width < 20 ? cellRect.Width : 20); // Previous code <==  var xWidth = (width < 30 ? width : 30); ==> this is changed to avoid the error icon to shrink before it reaches the left end and allow to overlap on text.
                    }       
                    //var xWidth = cellRect.Width - (width < 30 ? width : 30);
                    if (!(this.CurrentCell.IsEditing && this.CurrentCell.CellRowColumnIndex == style.CellRowColumnIndex && (style.CellType == "ComboBox" || style.CellType == "DropDownList")))
                    {
                        imgRect = new Rect(cellRect.X + (cellRect.Width - xWidth - combowidth), cellRect.Y, xWidth, height);
                    }
                }
                if (imgRect != Rect.Empty)
                {
                    imgRect = style.ErrorInfo.AdjustImageMargins(imgRect);
                    switch (style.ErrorInfo.ReadOnlyErrorType)
                    {
                        case ErrorType.ErrorMessage:
                            var errorMessage = this.GetErrorMessageBrush();
                            dc.DrawRectangle(errorMessage, null, imgRect);
                            break;
                        case ErrorType.Information:
                            var errorInfoBrush = this.GetErrorInformationBrush();
                            dc.DrawRectangle(errorInfoBrush, null, imgRect);
                            break;
                        case ErrorType.Custom:
                            if (style.ErrorInfo.HasCustomImage)
                            {
                                dc.DrawImage(style.ErrorInfo.CustomImage, imgRect);
                            }
                            break;
                    }
                }
            }
        }

        #region GridCellRender

        public static readonly RoutedEvent CellRenderedEvent = EventManager.RegisterRoutedEvent(
            "CellRendered",
            RoutingStrategy.Direct,
            typeof(GridCellRenderEventHandler),
            typeof(GridControlBase));

        /// <summary>
        /// Occurs after the grid cells have been rendered.
        /// </summary>
        public event GridCellRenderEventHandler CellRendered
        {
            add
            {
                this.AddHandler(GridControlBase.CellRenderedEvent, value);
            }

            remove
            {
                this.RemoveHandler(GridControlBase.CellRenderedEvent, value);
            }
        }

        internal void RaiseGridCellRenderEvent(GridCellRenderEventArgs args)
        {
            this.OnGridCellRenderEvent(args);
        }

        protected virtual void OnGridCellRenderEvent(GridCellRenderEventArgs args)
        {
            this.RaiseEvent(args);
        }

        #endregion

        private bool ShouldPushClip()
        {
            int nhRow = this.InternalGetHeaderRows();
            int nhCol = this.InternalGetHeaderCols();
            int nfRow = this.InternalGetFrozenRows();
            int nfCol = this.InternalGetFrozenCols();

            if (nhCol == 0 && nfCol > 0)
            {
                return false;
            }

            if (nhRow == 0 && nfCol > 0)
            {
                return false;
            }

            return true;
        }

        protected virtual void RenderActiveRangeBorder()
        {
            DrawingContext dccBorder = dvSelectBorder.RenderOpen();

            if (Model.Options.ExcelLikeSelectionFrame
                && Model.SelectedRanges.Count > 0)
            {
                // Rect gridRect = new Rect(0, 0, RenderSize.Width, RenderSize.Height); Unused local variable
                GridRangeInfo selectedCells = Model.SelectedRanges[Model.SelectedRanges.Count - 1];

                if (!selectedCells.IsEmpty)
                {
                    // When rows are selected, draw marker at bottom-left.
                    // When columns are selected, draw marker at top-right.
                    GridSelectionMarkerLocation nMarker;
                    if (selectedCells.IsCols)
                    {
                        nMarker = GridSelectionMarkerLocation.Top;
                    }
                    else if (selectedCells.IsRows)
                    {
                        nMarker = GridSelectionMarkerLocation.Left;
                    }
                    else
                    {
                        nMarker = GridSelectionMarkerLocation.Default;
                    }

                    selectedCells = this.ExpandSelectedCellsRange(selectedCells);

                    DoubleSpan[] yPos = ScrollRows.RangeToRegionPoints(selectedCells.Top, selectedCells.Bottom, true);
                    DoubleSpan[] xPos = ScrollColumns.RangeToRegionPoints(selectedCells.Left, selectedCells.Right, true);

                    for (int rowRegion = 0; rowRegion < 3; rowRegion++)
                    {
                        if (yPos[rowRegion].IsEmpty)
                            continue;

                        for (int columnRegion = 0; columnRegion < 3; columnRegion++)
                        {
                            if (xPos[columnRegion].IsEmpty)
                                continue;

                            Rect clipRect = GetClipRect((ScrollAxisRegion)rowRegion, (ScrollAxisRegion)columnRegion);
                            Rect r = new Rect(xPos[columnRegion].Start, yPos[rowRegion].Start, xPos[columnRegion].Length, yPos[rowRegion].Length);

                            if (!clipRect.IntersectsWith(r) && (this.model.HeaderRows != 0 || this.model.HeaderColumns != 0))
                                continue;
                            if (!clipRect.IntersectsWith(r) && (this.model.HeaderRows == 0 && this.model.HeaderColumns == 0))
                            {
                                if ((this.model.FrozenColumns != 0 && this.model.FrozenRows != 0))
                                    continue;
                            }
                            // Border
                            //if (this.ShouldPushClip())
                                PushClip(dccBorder, clipRect);

                            // adjusting values if there are frozen columns / rows
                            //int nhRow = this.InternalGetHeaderRows();
                                //int nhCol = this.InternalGetHeaderCols(); Unused local variable
                            int nfRow = this.InternalGetFrozenRows();
                            int nfCol = this.InternalGetFrozenCols();
                            //int nfooterrow = this.InternalGetFooterRows();
                            //int nfootercol = this.InternalGetFooterColumns(); Unused local variable
                            bool CanDrawMarkerRect = false;

                            //if(nfootercol > 0)
                            //    r = new Rect(0, r.Y, r.Width + r.X, r.Height);
                            //else if(nfooterrow > 0)
                            //    r = new Rect(0, r.Y, r.Width + r.X, r.Height);
                            //else if (nhCol == 0 && nfCol > 0)
                            //{
                            //    r = new Rect(0, r.Y, r.Width + r.X, r.Height);
                            //}
                            //else if (nhRow == 0 && nfRow > 0)
                            //{
                            //    r = new Rect(r.X, 0, r.Width, r.Height + r.Y);
                            //}

                            //r.Intersect(clipRect);

                            //if (!r.IsEmpty)
                            //{
                            //    r = new Rect(0, r.Y, r.Width + r.X, r.Height);
                            //}

                            if (nfCol == 0 && nfRow == 0)
                            {
                                CanDrawMarkerRect = true;
                            }
                            if (nfCol > 0 || nfRow > 0)
                            {
                               Rect  tRect = this.RangeToClippedVisibleRect(selectedCells);
                               if (!tRect.IsEmpty)
                               {
                                   r.Intersect(tRect);
                                   CanDrawMarkerRect = true;
                               }
                            }

                            double w = Model.Options.HighlightSelectionBorderWidth;
                            double w2 = w + 6;
                            Pen pen = new Pen(Model.Options.HighlightSelectionBorder, w);
                            pen.EndLineCap = PenLineCap.Round;
                            pen.StartLineCap = PenLineCap.Round;

                            Rect markerRect = Rect.Empty;
                            if ((Model.Options.DrawSelectionOptions & GridDrawSelectionOptions.ExcelLikeSelectionMarker) != 0)
                            {
                                if (CanDrawMarkerRect)
                                {
                                    switch (nMarker)
                                    {
                                        case GridSelectionMarkerLocation.Left:
                                            markerRect = new Rect(r.Left - w2 / 2, r.Bottom - w2 / 2, w2, w2);
                                            break;

                                        case GridSelectionMarkerLocation.Top:
                                            markerRect = new Rect(r.Right - w2 / 2, r.Top - w2 / 2, w2, w2);
                                            break;

                                        default:
                                            markerRect = new Rect(r.Right - w2 / 2, r.Bottom - w2 / 2, w2, w2);
                                            break;
                                    }
                                }

                                PushClip(dccBorder, clipRect, markerRect);
                            }

                            OnRenderSelectionBorder(dccBorder, r, CellBorderSide.Left, pen);
                            OnRenderSelectionBorder(dccBorder, r, CellBorderSide.Top, pen);
                            OnRenderSelectionBorder(dccBorder, r, CellBorderSide.Right, pen);
                            OnRenderSelectionBorder(dccBorder, r, CellBorderSide.Bottom, pen);

                            //if (ShouldPushClip())
                                dccBorder.Pop();

                            if (!markerRect.IsEmpty)
                            {
                                dccBorder.Pop();

                                markerRect.Inflate(-2, -2);

                                dccBorder.DrawRectangle(pen.Brush, null, markerRect);
                            }
                        }
                    }
                }
            }
            dccBorder.Close();
        }

        /// <summary>
        /// Render current cell border into the dvCurrentCellBorder DrawingVisual of the ScrollControl.ForegroundFrame. This will not trigger any InvalidateVisual or InvalidateArrange calls.
        /// </summary>
        protected internal virtual void RenderCurrentCellBorder()
        {
            DrawingContext dccBorder = dvCurrentCellBorder.RenderOpen();

            if (ShouldRenderCurrentCellBorder() && CurrentCell.HasCurrentCell && Model.Options.ShowCurrentCell && !Model.Options.ExcelLikeSelectionFrame)
            {
                GridRangeInfo currentCellRange = CurrentCell.RangeInfo;
                CoveredCellInfo cc = CoveredCells.GetCellSpan(CurrentCell.RowIndex, CurrentCell.ColumnIndex);
                if (cc != null)
                    currentCellRange = GridRangeInfo.Cells(cc.Top, cc.Left, cc.Bottom, cc.Right);

                DoubleSpan[] yCurrentCellPos = ScrollRows.RangeToRegionPoints(currentCellRange.Top, currentCellRange.Bottom, true);
                DoubleSpan[] xCurrentCellPos = ScrollColumns.RangeToRegionPoints(currentCellRange.Left, currentCellRange.Right, true);

                for (int rowRegion = 0; rowRegion < 3; rowRegion++)
                {
                    if (yCurrentCellPos[rowRegion].IsEmpty)
                        continue;

                    for (int columnRegion = 0; columnRegion < 3; columnRegion++)
                    {
                        if (xCurrentCellPos[columnRegion].IsEmpty)
                            continue;

                        Rect clipRect = GetClipRect((ScrollAxisRegion)rowRegion, (ScrollAxisRegion)columnRegion);
                        Rect r = new Rect(xCurrentCellPos[columnRegion].Start, yCurrentCellPos[rowRegion].Start, xCurrentCellPos[columnRegion].Length, yCurrentCellPos[rowRegion].Length);

                        if (!clipRect.IntersectsWith(r) && !this.ShouldPushClip())
                            continue;

                        if (this.ShouldPushClip())
                        {
                            PushClip(dccBorder, clipRect);
                        }
                        else
                        {
                            Rect tRect = this.RangeToClippedVisibleRect(currentCellRange);
                            r.Intersect(tRect);
                        }

                        if (!r.IsEmpty)
                        {
                            RenderCurrentCellBorder(dccBorder, r);
                        }

                        if (this.CurrentCell.RowIndex + 1 > this.InternalGetHeaderRows() && this.CurrentCell.ColumnIndex + 1 > this.InternalGetHeaderCols())
                        {
                           if (ShouldPushClip())
                            {
                                dccBorder.Pop();
                            }
                        }
                    }
                }
            }
            dccBorder.Close();

            RenderSelectedCells();
        }

        protected virtual void RenderCurrentCellBorder(DrawingContext dccBorder, Rect r)
        {
            Pen pen = new Pen(Model.Options.CurrentCellBorder, Model.Options.CurrentCellBorderWidth);
            pen.EndLineCap = PenLineCap.Round;
            pen.StartLineCap = PenLineCap.Round;

            if (CurrentCell.RowIndex == Model.RowCount - 1 && Model.RowHeights.PaddingDistance > 0)
                r.Height -= Model.RowHeights.PaddingDistance;

            OnRenderSelectionBorder(dccBorder, r, CellBorderSide.Left, pen);
            OnRenderSelectionBorder(dccBorder, r, CellBorderSide.Top, pen);
            OnRenderSelectionBorder(dccBorder, r, CellBorderSide.Right, pen);
            OnRenderSelectionBorder(dccBorder, r, CellBorderSide.Bottom, pen);
        }

        protected virtual bool ShouldRenderCurrentCellBorder()
        {
            return true;
        }

        internal bool needRenderStyleBackgrounds = true;
        protected override bool ShouldRenderStyleBackgrounds()
        {
            if (this.EnableRenderOptimization == Grid.EnableRenderOptimization.DisableBackgroundFrameRendering && !needRenderStyleBackgrounds)
            {
                needRenderStyleBackgrounds = true;
                return false;
            }
            else
                return base.ShouldRenderStyleBackgrounds();
        }
        //Below method is to provide for calculating Grid width.
        internal bool canAutoCalculateWidth = false;
        protected override bool CanAutoCalculateWidth()
        {
            return canAutoCalculateWidth;
        }
        protected virtual void OnRenderSelectionBorder(DrawingContext dc, Rect cellRect, CellBorderSide borderSide, Pen pen)
        {
            if (cellRect.Width == 0 || cellRect.Height == 0)
                return;

            double halfPenWidth = pen.Thickness / 2;
            // Create a guidelines set
            GuidelineSet guidelines = new GuidelineSet();
            guidelines.GuidelinesX.Add(cellRect.Left + halfPenWidth);
            guidelines.GuidelinesX.Add(cellRect.Right + halfPenWidth);
            guidelines.GuidelinesY.Add(cellRect.Top + halfPenWidth);
            guidelines.GuidelinesY.Add(cellRect.Bottom + halfPenWidth);
            dc.PushGuidelineSet(guidelines);

            var left = Model.TableStyle.Borders.Left != null ? Model.TableStyle.Borders.Left.Thickness : 0;
            var right = Model.TableStyle.Borders.Right != null ? Model.TableStyle.Borders.Right.Thickness : 0;
            var top = Model.TableStyle.Borders.Top != null ? Model.TableStyle.Borders.Top.Thickness : 0;
            var bottom = Model.TableStyle.Borders.Bottom != null ? Model.TableStyle.Borders.Bottom.Thickness : 0;
            if (cellRect.Width > (left + right) && cellRect.Height > (top + bottom))
                //X, Y position is adjusted due to the SnapsToDevicePixels clips the blured borders.
                cellRect = new Rect(cellRect.X + left, cellRect.Y + top, cellRect.Width - (left + right), cellRect.Height - (top + bottom));
            
            switch (borderSide)
            {
                case CellBorderSide.Top:
                    dc.DrawLine(pen, cellRect.TopLeft, cellRect.TopRight);
                    break;
                case CellBorderSide.Bottom:
                    dc.DrawLine(pen, cellRect.BottomLeft, cellRect.BottomRight);
                    break;
                case CellBorderSide.Left:
                    dc.DrawLine(pen, cellRect.TopLeft, cellRect.BottomLeft);
                    break;
                case CellBorderSide.Right:
                    dc.DrawLine(pen, cellRect.TopRight, cellRect.BottomRight);
                    break;
            }
            dc.Pop();
        }

        #endregion

        #region RenderHiddenCellBorder

        protected virtual void RenderHiddenCellBorder()
        {
            if (!this.Model.Options.AllowExcelLikeResizing)
            {
                return;
            }

            using (var dchBorder = dvHiddenBorder.RenderOpen())
            {
                //Draw Hidden Cols border
                if (Model.HiddenColRanges != null && Model.HiddenColRanges.Count > 0)
                {
                    // Optimized the looping for faster rendering
                    foreach (GridRangeInfo colInfo in Model.HiddenColRanges)
                    {
                        VisibleLineInfo visibleColumn = null;

                        bool preceding = true;

                        if (colInfo.Right == this.Model.ColumnCount - 1) // Special case for last column
                        {
                            visibleColumn = ScrollColumns.GetVisibleLines()[ScrollColumns.GetVisibleLines().Count - 1];
                            preceding = false;
                        }
                        else if (colInfo.Right > 0 && colInfo.Right < this.Model.ColumnCount)
                        {
                            visibleColumn = ScrollColumns.GetVisibleLineAtLineIndex(colInfo.Right + 1, false);
                        }
                        // if visible Column is null then skip drawing as the next visible column appearing
                        // immediately after the hidden column will take care of it
                        if (visibleColumn != null)
                        {
                            int columnIndex = visibleColumn.LineIndex;
                            var visibleRows = ScrollRows.GetVisibleLines();
                            foreach (var visibleRow in visibleRows)
                            {
                                var rowIndex = visibleRow.LineIndex;
                                var cellRect = new Rect(0, visibleRow.Origin, ScrollColumns.ViewSize, visibleRow.Size);
                                if (visibleRow.IsHeader)
                                {
                                    cellRect.X = visibleColumn.Origin;
                                    cellRect.Width = visibleColumn.Size;

                                    IRenderCellInfo renderCellInfo = GetRenderCellInfo(rowIndex, columnIndex);
                                    var rca = new RenderCellArgs(this, visibleRow, visibleColumn, cellRect, renderCellInfo);
                                    OnRenderHiddenColBorder(dchBorder, rca, preceding);
                                }
                            }
                        }
                    }
                }

                //Draw Hidden Rows Border
                if (Model.HiddenRowRanges != null && Model.HiddenRowRanges.Count > 0)
                {
                    // Optimized the looping for faster rendering                    
                    foreach (GridRangeInfo rowInfo in Model.HiddenRowRanges)
                    {
                        VisibleLineInfo visibleRow = null;
                        bool preceding = true;

                        if (rowInfo.Bottom == this.Model.RowCount - 1) // Sepcial case for last row
                        {
                            visibleRow = ScrollRows.GetVisibleLines()[ScrollRows.GetVisibleLines().Count - 1];
                            preceding = false;
                        }
                        else
                            visibleRow = ScrollRows.GetVisibleLineAtLineIndex(rowInfo.Bottom + 1, false);

                        // if visible row is null then skip drawing as the next visible row appearing
                        // immediately after the hidden row will take care of it
                        if (visibleRow != null)
                        {
                            var visibleCols = ScrollColumns.GetVisibleLines();
                            foreach (var visibleCol in visibleCols)
                            {
                                if (visibleCol.IsHeader)
                                {
                                    int rowIndex = visibleRow.LineIndex;
                                    var colIndex = visibleCol.LineIndex;
                                    var cellRect = new Rect(0, visibleCol.Origin, ScrollColumns.ViewSize, visibleCol.Size);
                                    cellRect.X = visibleRow.Origin;
                                    cellRect.Width = visibleRow.Size;

                                    IRenderCellInfo renderCellInfo = GetRenderCellInfo(rowIndex, colIndex);
                                    var rca = new RenderCellArgs(this, visibleRow, visibleCol, cellRect, renderCellInfo);
                                    OnRenderHiddenRowBorder(dchBorder, rca, preceding);
                                }
                            }
                        }
                    }
                }
            }
        }
        #endregion

        #region RenderHiddenColBorder

        protected virtual void OnRenderHiddenColBorder(DrawingContext dc, RenderCellArgs rca, bool preceding)
        {
            if (rca.CellsControl is GridDataControlBaseImpl)
            {
                int stackedheaderindex = (rca.CellsControl as GridDataControlBaseImpl).TableModel.TableProperties.StackedHeaderRows.Count;
                if (stackedheaderindex >rca.RowIndex)
                    return;
            }
            if (rca.ColumnIndex > 0)
            {
                var coveredRange = this.Model.CoveredCells.GetCoveredCell(rca.RowIndex, rca.ColumnIndex);
                var isCovered = coveredRange != null;

                int rc, rh;
                var hidden = this.ColumnWidths.GetHidden(rca.ColumnIndex, out rc);
                var drawHidden = this.ColumnWidths.GetHidden(rca.ColumnIndex + (preceding ? -1 : 1), out rh);
                var startPoint = preceding ? rca.CellClipRect.TopLeft : rca.CellClipRect.TopRight;
                var endPoint = preceding ? rca.CellClipRect.BottomLeft : rca.CellClipRect.BottomRight;

                var hiddenCoveredRange = this.Model.CoveredCells.GetCoveredCell(rca.RowIndex, rca.ColumnIndex + (preceding ? -1 : 1));
                var containsHiddenCoveredRange = hiddenCoveredRange != null;

                bool skip = false;
                // Skip drawing hidden border for inner columns of stacked headers except for the edges.
                if (isCovered)
                {
                    skip = true;
                    if (rca.ColumnIndex == coveredRange.Right - rh || (containsHiddenCoveredRange && coveredRange != hiddenCoveredRange && hiddenCoveredRange.Width == rh))
                    {
                        skip = false;
                    }
                }

                if (!hidden && drawHidden && !skip) // Current is not hidden and the previous or next is hidden
                {
                    dc.DrawLine(new Pen(this.Model.Options.HiddenBorderBrush, this.Model.Options.HiddenBorderThickness), startPoint, endPoint);
                }
            }
        }

        #endregion


        #region RenderHiddenRowBorder

        protected virtual void OnRenderHiddenRowBorder(DrawingContext dc, RenderCellArgs rca, bool preceding)
        {
            if (rca.RowIndex > 0)
            {
                var coveredRange = this.Model.CoveredCells.GetCoveredCell(rca.RowIndex, rca.ColumnIndex);
                var isCovered = coveredRange != null;

                int rc, rh;
                var hidden = this.RowHeights.GetHidden(rca.RowIndex, out rc);
                var drawHidden = this.RowHeights.GetHidden(rca.RowIndex + (preceding ? -1 : 1), out rh);
                var startPoint = preceding ? rca.CellClipRect.TopLeft : rca.CellClipRect.BottomLeft;
                var endPoint = preceding ? rca.CellClipRect.TopRight : rca.CellClipRect.BottomRight;

                bool skip = false;
                // Skip drawing hidden border for inner rows of covered headers except for the edges.
                if (isCovered)
                {
                    skip = true;
                    if (rca.ColumnIndex == coveredRange.Right - rh)
                    {
                        skip = false;
                    }
                }

                if (!hidden && drawHidden && !skip) // Current is not hidden and the previous or next is hidden
                {
                    dc.DrawLine(new Pen(this.Model.Options.HiddenBorderBrush, this.Model.Options.HiddenBorderThickness), startPoint, endPoint);
                }
            }
        }

        #endregion

        #region Resize Rows and Columns Controller Hooks

        /// <summary>
        /// Sets the height for the given row.
        /// </summary>
        /// <param name="rowIndex">The row index.</param>
        /// <param name="size">Row Height to be set.</param>
        public virtual void SetRowHeight(int rowIndex, double size)
        {
            if (!RowHeights.SupportsNestedLines || RowHeights.GetNestedLines(rowIndex) == null)
                RowHeights[rowIndex] = size;
            InvalidateVisual();
        }

        /// <summary>
        /// Sets the width for the given column.
        /// </summary>
        /// <param name="columnIndex">The column index.</param>
        /// <param name="size">Column Width to be set.</param>
        public virtual void SetColumnWidth(int columnIndex, double size)
        {
            if (!ColumnWidths.SupportsNestedLines || ColumnWidths.GetNestedLines(columnIndex) == null)
            {
                //this.CurrentCell.CellRowColumnIndex.
                //for (int i = 0; i < this.RowHeights.LineCount; i++)
                //{
                //    var uielement = this.GetCellUIElements(i, columnIndex);
                //    if(uielement!=null)
                //        uielement.IsDirty = true; 
                //}
                if (columnIndex < this.Model.ColumnCount)
                {
                    this.ArrangedCellUIElements.Invalidate(new CellSpanInfoBase(0, columnIndex, this.RowHeights.LineCount - 1, columnIndex + 1));
                    //for (int i = 0; i < this.RowHeights.LineCount; i++)
                    //{
                    //    var uielement = this.GetCellUIElements(i, columnIndex + 1);
                    //    if (uielement != null)
                    //        uielement.IsDirty = true;
                    //}
                }
                ColumnWidths[columnIndex] = size;
            }
            InvalidateVisual();
        }

        #endregion

        #region Virtualized GridRenderStyleInfo

        /// <summary>
        /// Gets a collection of rendering styles for grid cells.
        /// </summary>
        public GridControlRenderStyles RenderStyles
        {
            get { return renderStyles; }
        }

        protected override void ArrangeCellUIElements(Size arrangeSize)
        {
            Model.VolatileCellStyles.visibleRowIndexes[this] = ScrollRows.GetVisibleLines().VisibleLineIndexes;
            Model.VolatileCellStyles.visibleColumnIndexes[this] = ScrollColumns.GetVisibleLines().VisibleLineIndexes;

            RenderStyles.PrepareArrange();

            // unloadCellStylesDictionary will get updated inside next method call
            // when it calls GetRenderCellInfo for individual cells.
            base.ArrangeCellUIElements(arrangeSize);

            RenderStyles.ConcludeArrange();
        }

        protected override void OnUnloaded(RoutedEventArgs e)
        {
            if (ClearVisualsCacheWhenUnloaded)
            {
                if (model != null && model.VolatileCellStyles != null)
                {
                    model.VolatileCellStyles.visibleRowIndexes.Remove(this);
                    model.VolatileCellStyles.visibleColumnIndexes.Remove(this);
                }

                RenderStyles.Clear();
            }
            base.OnUnloaded(e);
        }

        protected sealed override IRenderCellInfo GetRenderCellInfo(int rowIndex, int columnIndex)
        {
#if TestDrawTextPerformance
			if (style == null) style = new GridRenderStyleInfo(
				this, new GridStyleInfo(
					new GridStyleInfoIdentity(Model.VolatileCellStyles, new RowColumnIndex(0, 0))
					));
			return style;
#endif
            return RenderStyles.GetRenderStyleInfo(rowIndex, columnIndex);
        }

        protected override ICellRenderer GetCellRenderer(IRenderCellInfo cellInfo)
        {
            return ((GridRenderStyleInfo)cellInfo).CellRenderer;
        }

        /// <summary>
        /// Gets the rendering style for a cell given its row and column indices.
        /// </summary>
        /// <param name="rowIndex">Row index.</param>
        /// <param name="columnIndex">Column index.</param>
        /// <returns>Rendering cell style.</returns>
        public GridRenderStyleInfo GetRenderStyleInfo(int rowIndex, int columnIndex)
        {
            return RenderStyles.GetRenderStyleInfo(rowIndex, columnIndex);
        }

        /// <summary>
        /// Gets the rendering style for a cell given its row and column indices.
        /// </summary>
        /// <param name="cellRowColumnIndex">Row and column indices as <see cref="RowColumnIndex"/>.</param>
        /// <returns>Rendering cell style.</returns>
        public GridRenderStyleInfo GetRenderStyleInfo(RowColumnIndex cellRowColumnIndex)
        {
            return RenderStyles.GetRenderStyleInfo(cellRowColumnIndex.RowIndex, cellRowColumnIndex.ColumnIndex);
        }

        /// <summary>
        /// Gets the rendering style for a cell given its row and column indices.
        /// </summary>
        /// <param name="rowIndex">Row index.</param>
        /// <param name="columnIndex">Column index.</param>
        /// <param name="createDisposableObject">When true, returns a render style object that can be disposed.</param>
        /// <returns>Rendering cell style.</returns>
        public GridRenderStyleInfo GetRenderStyleInfo(int rowIndex, int columnIndex, bool createDisposableObject)
        {
            return RenderStyles.GetRenderStyleInfo(rowIndex, columnIndex, createDisposableObject);
        }

        /// <summary>
        /// Gets the rendering style for a cell given its row and column indices.
        /// </summary>
        /// <param name="cellRowColumnIndex">Row and column indices as <see cref="RowColumnIndex"/>.</param>
        /// <param name="createDisposableObject">When true, returns a render style object that can be disposed.</param>
        /// <returns>Rendering cell style.</returns>
        public GridRenderStyleInfo GetRenderStyleInfo(RowColumnIndex cellRowColumnIndex, bool createDisposableObject)
        {
            return RenderStyles.GetRenderStyleInfo(cellRowColumnIndex.RowIndex, cellRowColumnIndex.ColumnIndex, createDisposableObject);
        }

        #endregion

        #region InvalidateCell

        /// <summary>
        /// Reset cached values for cell and reset any visuals associated with the cell. You need
        /// to call InvalidateVisual or RenderNow after this method.
        /// </summary>
        /// <param name="cellRowColumnIndex">Row and column indices as <see cref="RowColumnIndex"/>.</param>
        public override void InvalidateCell(RowColumnIndex cellRowColumnIndex)
        {
            RenderStyles.Clear(cellRowColumnIndex);
            Model.VolatileCellStyles.Clear(cellRowColumnIndex);
            base.InvalidateCell(cellRowColumnIndex);
        }

        /// <summary>
        /// Reset cached values for the cell span and reset any visuals associated with the cell. You need
        /// to call InvalidateVisual or RenderNow after this method.
        /// </summary>
        /// <param name="span">Cell spanned range to be invalidated.</param>
        public override void InvalidateCell(CellSpanInfoBase span)
        {
            // SH 4/6/10: With ClearStylesOnInsertRemoveRows set to true RenderStyles will be 
            // empty after inser remove operations and so this call should return quickly.
            RenderStyles.Clear(span);

            // I optimized this methos for Rows ranges.
            Model.VolatileCellStyles.Clear(span);

            // ArrangedCellUIElements and RenderedCellVisuals alwas must be updated... If cells
            // are not visible this should however return real quick since row lookup will return
            // false inside Invalidate calls.

            base.InvalidateCell(span);
        }

        /// <summary>
        /// Reset cached values for a range of cells and reset any visuals associated with the cell. You need
        /// to call InvalidateVisual or RenderNow after this method.
        /// </summary>
        /// <param name="gridRangeInfo">A range of cells to invalidate.</param>
        public void InvalidateCell(GridRangeInfo gridRangeInfo)
        {
            CellSpanInfoBase span = gridRangeInfo.ToCellSpan(Model);
            InvalidateCell(span);
        }

        /// <summary>
        /// Recycles the UI element objects of the cells.
        /// </summary>
        public override void UnloadArrangedCells()
        {
            RenderStyles.Clear();
            Model.VolatileCellStyles.Clear();
            base.UnloadArrangedCells();
            CellRenderers.EmptyRecycleBin();
        }

        #endregion

        #region PrepareRenderCell Event
        public static readonly RoutedEvent PrepareRenderCellEvent = EventManager.RegisterRoutedEvent(
            "PrepareRenderCell",
            RoutingStrategy.Direct,
            typeof(GridPrepareRenderCellEventHandler),
            typeof(GridControlBase));

        internal virtual void RaisePrepareRenderCell(GridPrepareRenderCellEventArgs e)
        {
            if (Model.Options != null && ((Model.Options.DrawSelectionOptions & (GridDrawSelectionOptions.ReplaceBackground | GridDrawSelectionOptions.ReplaceTextColor)) != 0))
            {
                if (e.Cell.RowIndex >= Model.HeaderRows && e.Cell.ColumnIndex >= Model.HeaderColumns)
                {
                    if (Model.SelectedRanges.AnyRangeContains(GridRangeInfo.Cell(e.Cell.RowIndex, e.Cell.ColumnIndex)))
                    {
                        if (Model is GridDataTableModel)
                        {
                            var currentStyle = (e.Style as GridRenderStyleInfo).ModelStyle;
                            var tableStyleIdentity = (currentStyle as GridDataStyleInfo).CellIdentity;

                            if (tableStyleIdentity.TableCellType == GridDataTableCellType.RecordPlusMinusCell
                                || tableStyleIdentity.TableCellType == GridDataTableCellType.GroupCaptionPlusMinusCell                                
                                ||tableStyleIdentity.TableCellType==GridDataTableCellType.EmptyCell
                                ||tableStyleIdentity.TableCellType==GridDataTableCellType.NestedTableEmptyCell)
                                return;
                        }
                        if ((Model.Options.DrawSelectionOptions & GridDrawSelectionOptions.ReplaceBackground) != 0)
                            e.Style.Background = Model.Options.HighlightSelectionBackground;
                        if ((Model.Options.DrawSelectionOptions & GridDrawSelectionOptions.ReplaceTextColor) != 0)
                            e.Style.Foreground = Model.Options.HighlightSelectionForeground;
                    }
                }
            }

            OnPrepareRenderCell(e);
        }

        protected virtual void OnPrepareRenderCell(GridPrepareRenderCellEventArgs e)
        {
            base.RaiseEvent(e);
        }

        /// <summary>
        /// Occurs before a cell gets rendered.
        /// </summary>
        public event GridPrepareRenderCellEventHandler PrepareRenderCell
        {
            add
            {
                this.AddHandler(GridControlBase.PrepareRenderCellEvent, value);
            }
            remove
            {
                this.RemoveHandler(GridControlBase.PrepareRenderCellEvent, value);
            }
        }

        #endregion

        #region Insert and Remove Rows

        /*
		/// <summary>
		/// Enable adjusting scrollbar without setting cell layout dirty when removing
		/// or inserting rows outside visible range. When you set this true you must not
		/// use individual row heights. All rows in scrollable area must have the same height.
		/// Headers and footers can have different row heights.
		/// </summary>
		public bool AllowOptimizedInsertRemoveRows = false;
		*/

        protected internal virtual void ModelInsertRows(int insertAtRowIndex, int count, GridViewMoveCellsState moveCellsState)
        {
            /*if (Model.ClearStylesOnInsertRemoveRows)
            {
                // SH 4/6/10: Do not bother about looping through rows and updating cell styles rowcolumn index
                // position. Instead when a style is needed requery that cell information. 
                RenderStyles.Clear();
                // RenderStyles.RemoveRows call below will have no cost since list is empty.

                // There is however a bit an issue that now the ArrangedUIElements and CellVisuals that
                // reference style object will now have dangling style objects with invalid row position
                // and this could become an issue. We should only use this optimization and do further
                // checks if it works only if it really makes a difference.
            }*/

            if (moveCellsState == null) moveCellsState = GridViewMoveCellsState.Empty;

            RenderStyles.InsertRows(insertAtRowIndex, count, moveCellsState.RenderStyles);
            ArrangedCellUIElements.InsertRows(insertAtRowIndex, count, moveCellsState.CellUIElements);
            CurrentCell.InsertRows(insertAtRowIndex, count, moveCellsState.CurrentCell);
            RenderedCellVisuals.InsertRows(insertAtRowIndex, count, moveCellsState.DrawingVisuals);

            // TODO: Instead of only relying on AllowOptimizedInsertRemoveRows I could also
            // double check Distances collection whether there were any individual row heights
            // set to avoid drawing glitches.
            //if (AllowOptimizedInsertRemoveRows)
            {
                bool visibleRowsAffected = ScrollRows.IsLineVisible(insertAtRowIndex);

                forceArrangeDirty = visibleRowsAffected || insertAtRowIndex < TopRowIndex; // force ArrangeContent to be called next time ArrangeOverride is called.
                if(forceArrangeDirty)
                    InvalidateVisual(visibleRowsAffected);
            }
            /*else
            {
                InvalidateVisual(true); // calls ScrollRows.MarkDirty();
            }*/
        }

        protected internal virtual void ModelRemoveRows(int removeAtRowIndex, int count, GridViewMoveCellsState moveCellsState)
        {
            /*if (Model.ClearStylesOnInsertRemoveRows)
            {
                // SH 4/6/10: Do not bother about looping through rows and updating cell styles rowcolumn index
                // position. Instead when a style is needed requery that cell information. 
                RenderStyles.Clear();
                // RenderStyles.RemoveRows call below will have no cost since list is empty.

                // There is however a bit an issue that now the ArrangedUIElements and CellVisuals that
                // reference style object will now have dangling style objects with invalid row position
                // and this could become an issue. We should only use this optimization and do further
                // checks if it works only if it really makes a difference.
            } */

            if (moveCellsState == null) moveCellsState = GridViewMoveCellsState.Empty;

            RenderStyles.RemoveRows(removeAtRowIndex, count, moveCellsState.RenderStyles);
            ArrangedCellUIElements.RemoveRows(removeAtRowIndex, count, moveCellsState.CellUIElements);
            CurrentCell.RemoveRows(removeAtRowIndex, count, moveCellsState.CurrentCell);
            RenderedCellVisuals.RemoveRows(removeAtRowIndex, count, moveCellsState.DrawingVisuals);

            //if (AllowOptimizedInsertRemoveRows)
            {
                bool visibleRowsAffected = ScrollRows.AnyVisibleLines(removeAtRowIndex, removeAtRowIndex + count - 1);

                /* Two issues to consider are: cell borders and cell backgrounds.
                 *
                 * In VirtualizingCellsControl.OnRender I check needRenderStyleBackgrounds. If this is true 
                 * and cell backgrounds are specified in PrepareRenderCell (and not in QueryCellInfo) I could skip arrange cell backgounds.
                 * 
                 * ArrangeCellBorders is not called because ArrangeOverride does return immediately when (!isArrangeDirty && arrangeSize == lastArrangeSize) 
                 * condition is met. Therefore borders will not get updated even if SetCellLayoutDirty is called.
                 * forceArrangeDirty property makes sure cell borders get redrawn.
                 */
                forceArrangeDirty = removeAtRowIndex < TopRowIndex || visibleRowsAffected; // force ArrangeContent to be called next time ArrangeOverride is called.
                if(forceArrangeDirty)
                    InvalidateVisual(visibleRowsAffected);
            }
            /*else
            {
                InvalidateVisual(true);  // calls ScrollRows.MarkDirty();
            }*/
        }

        bool forceArrangeDirty = false;

        protected override Size OnArrangeOverride(Size arrangeSize, ref bool isArrangeDirty)
        {
            isArrangeDirty |= forceArrangeDirty;
            forceArrangeDirty = false;
            ScrollRows.UpdateScrollBar(true);

            return base.OnArrangeOverride(arrangeSize, ref isArrangeDirty);
        }

        #endregion

        #region Insert and Remove Columns

        protected internal virtual void ModelInsertColumns(int insertAtColumnIndex, int count, GridViewMoveCellsState moveCellsState)
        {
            if (moveCellsState == null) moveCellsState = GridViewMoveCellsState.Empty;

            RenderStyles.InsertColumns(insertAtColumnIndex, count, moveCellsState.RenderStyles);
            ArrangedCellUIElements.InsertColumns(insertAtColumnIndex, count, moveCellsState.CellUIElements);
            CurrentCell.InsertColumns(insertAtColumnIndex, count, moveCellsState.CurrentCell);
            RenderedCellVisuals.InsertColumns(insertAtColumnIndex, count, moveCellsState.DrawingVisuals);

            ScrollColumns.MarkDirty();
            InvalidateVisual(true);
        }

        protected internal virtual void ModelRemoveColumns(int removeAtColumnIndex, int count, GridViewMoveCellsState moveCellsState)
        {
            if (moveCellsState == null) moveCellsState = GridViewMoveCellsState.Empty;

            RenderStyles.RemoveColumns(removeAtColumnIndex, count, moveCellsState.RenderStyles);
            ArrangedCellUIElements.RemoveColumns(removeAtColumnIndex, count, moveCellsState.CellUIElements);
            CurrentCell.RemoveColumns(removeAtColumnIndex, count, moveCellsState.CurrentCell);
            RenderedCellVisuals.RemoveColumns(removeAtColumnIndex, count, moveCellsState.DrawingVisuals);

            ScrollColumns.MarkDirty();
            InvalidateVisual(true);
        }

        #endregion

        #region RangeToRect
        /// <summary>
        /// Returns the visible client rectangle for the given cell range.
        /// </summary>
        /// <param name="cellRowColumnIndex">Row and column indices as <see cref="RowColumnIndex"/>.</param>
        /// <returns>Visible client rectangle for the given cell range.</returns>
        public Rect RangeToClippedVisibleRect(RowColumnIndex cellRowColumnIndex)
        {
            return RangeToClippedVisibleRect(cellRowColumnIndex, true);
        }

        /// <summary>
        /// Returns the visible client rectangle for the given cell range.
        /// </summary>
        /// <param name="cellRowColumnIndex">Row and column indices as <see cref="RowColumnIndex"/>.</param>
        /// <param name="expandCoveredCell">When true, any covered cell will be expanded before calculating the visible rectangle.</param>
        /// <returns>Visible client rectangle for the given cell range.</returns>
        public Rect RangeToClippedVisibleRect(RowColumnIndex cellRowColumnIndex, bool expandCoveredCell)
        {
            if (cellRowColumnIndex.IsEmpty)
                return Rect.Empty;

            Rect rect;
            CoveredCellInfo cc = null;
            if (expandCoveredCell)
                cc = GetCoveredCell(cellRowColumnIndex);
            if (cc == null)
                rect = RangeToClippedVisibleRect(GridRangeInfo.Cell(cellRowColumnIndex.RowIndex, cellRowColumnIndex.ColumnIndex));
            else
                rect = RangeToClippedVisibleRect(GridRangeInfo.Cells(cc.Top, cc.Left, cc.Bottom, cc.Right));
            return rect;
        }

        /// <summary>
        /// Returns the visible client rectangle for the given cell range.
        /// </summary>
        /// <param name="range">Cell range.</param>
        /// <returns>Visible client rectangle for the given cell range.</returns>
        public Rect RangeToClippedVisibleRect(GridRangeInfo range)
        {
            if (range.IsEmpty)
                return Rect.Empty;

            GridRangeInfo rg = range.ExpandRange(0, 0, RowHeights.LineCount, ColumnWidths.LineCount);
            DoubleSpan ySpan = ScrollRows.GetVisibleLinesClipPoints(rg.Top, rg.Bottom);
            DoubleSpan xSpan = ScrollColumns.GetVisibleLinesClipPoints(rg.Left, rg.Right);

            if (ySpan.IsEmpty || xSpan.IsEmpty)
                return Rect.Empty;

            return new Rect(xSpan.Start, ySpan.Start, xSpan.Length, ySpan.Length);
        }

        /// <summary>
        /// For internal use.
        /// </summary>
        /// <param name="rowRegion">Scroll axis region for row.</param>
        /// <param name="columnRegion">Scroll axis region for column.</param>
        /// <param name="range">Cell range.</param>
        /// <param name="allowEstimatesForOutOfViewRows">If set to true, allows estimate for out of view rows.</param>
        /// <param name="allowEstimatesForOutOfViewColumns">If set to true, allows estimate for out of view columns.</param>
        /// <returns>Visible rectangle for the given range.</returns>
        public Rect RangeToRect(ScrollAxisRegion rowRegion, ScrollAxisRegion columnRegion, GridRangeInfo range, bool allowEstimatesForOutOfViewRows, bool allowEstimatesForOutOfViewColumns)
        {
            if (range.IsEmpty)
                return Rect.Empty;

            GridRangeInfo rg = range.ExpandRange(0, 0, RowHeights.LineCount, ColumnWidths.LineCount);
            DoubleSpan ySpan = ScrollRows.RangeToPoints(rowRegion, rg.Top, rg.Bottom, allowEstimatesForOutOfViewRows);
            DoubleSpan xSpan = ScrollColumns.RangeToPoints(columnRegion, rg.Left, rg.Right, allowEstimatesForOutOfViewColumns);

            if (ySpan.IsEmpty || xSpan.IsEmpty)
                return Rect.Empty;

            return new Rect(xSpan.Start, ySpan.Start, xSpan.Length, ySpan.Length);
        }

        #endregion

        #region CancelModel Event

        public static readonly RoutedEvent CancelModeEvent = EventManager.RegisterRoutedEvent("CancelMode",
            RoutingStrategy.Direct,
            typeof(GridRoutedEventHandler),
            typeof(GridControlBase));

        /// <summary>
        /// Raises the <see cref="CancelMode"/> event.
        /// </summary>
        /// <param name="e">An <see cref="EventArgs"/> that contains the event data.</param>
        protected virtual void OnCancelMode(SyncfusionRoutedEventArgs e)
        {
            base.RaiseEvent(e);
            //if (CancelMode != null)
            //{
            //    try
            //    {
            //        base.RaiseEvent(e)
            //    }
            //    catch (Exception ex)
            //    {
            //        TraceUtil.TraceExceptionCatched(ex);
            //        //if (!ExceptionManager.RaiseExceptionCatched(this, ex))
            //        {
            //            throw;
            //        }
            //    }
            //}
        }


        /// <summary>
        /// Occurs any current user interaction should be cancelled, e.g. cancel select cells when escape key is pressed.
        /// </summary>
        /// <remarks>
        /// WM_CANCELMODE is sent to cancel certain modes, such as mouse capture.
        /// For example, the system sends this message to the active window when a
        /// dialog box or message box is displayed. Certain functions also send this
        /// message explicitly to the specified window regardless of whether it is the
        /// active window. For example, the EnableWindow function sends this message
        /// when disabling the specified window.
        /// </remarks>
        [
        Description("Occurs when the window receives a WM_CANCELMODE message."),
        Category("Behavior")
        ]
        public event GridRoutedEventHandler CancelMode
        {
            add
            {
                this.AddHandler(GridControlBase.CancelModeEvent, value);
            }
            remove
            {
                this.RemoveHandler(GridControlBase.CancelModeEvent, value);
            }
        }

        #endregion

        #region CurrentCell Navigation

        internal GridCurrentCellMoveDelegateHandler externalMove;

        /// <summary>
        /// Used by GridSelectCellsMouseController.
        /// </summary>
        [Browsable(false)]
        [DesignerSerializationVisibilityAttribute(DesignerSerializationVisibility.Hidden)]
        [System.Xml.Serialization.XmlIgnore]
        public GridCurrentCellMoveDelegateHandler ExternalMove
        {
            get
            {
                return externalMove;
            }
            set
            {
                externalMove = value;
            }
        }

        /// <summary>
        /// The range of cells in the grid that can be navigated to using arrow keys.
        /// This range can but does not need to include header and footer rows and columns.
        /// The default scenario is that header and footer rows and columns are excluded 
        /// <para/>
        /// Cells outside the range can still be clicked on and be made the current cell but they
        /// will be skipped when the user navigate with arrow keys.
        /// </summary>
        /// <value>The grid cells range.</value>
        /// <remarks>
        /// When the user clicks on a cell it will be made the current cell through
        /// a <see cref="GridCurrentCell.MoveTo"/> call when the cell is enabled.
        /// When the user navigates with arrow keys the next current cell is determined
        /// by looping in the direction and skipping disabled cells with the <see cref="GridCurrentCell.Move"/>
        /// method. The Move method checks this range to determine when to stop searching
        /// in a given direction.
        /// </remarks>
        public virtual GridRangeInfo NavigateWithArrowKeysCellsRange
        {
            get
            {
                // TODO: Support non-default scenario. 
                // Default Range.
                return GridRangeInfo.Cells(
                    Model.HeaderRows,
                    Model.HeaderColumns,
                    RowHeights.LineCount - (RowHeights.FooterLineCount + 1),
                    ColumnWidths.LineCount - (ColumnWidths.FooterLineCount + 1)
                    );
            }
        }

        /// <summary>
        /// The range of cells in the grid that can be scrolled. This excludes
        /// all frozen rows and columns.
        /// </summary>
        public GridRangeInfo ScrollCellsRange
        {
            get
            {
                // TODO: Support non-default scenario. 
                // Default Range.
                return GridRangeInfo.Cells(
                    RowHeights.HeaderLineCount,
                    ColumnWidths.HeaderLineCount,
                    RowHeights.LineCount - (RowHeights.FooterLineCount + 1),
                    ColumnWidths.LineCount - (ColumnWidths.FooterLineCount + 1)
                    );
            }
        }


        #region WrapCellNextControlInForm
        public static readonly RoutedEvent WrapCellNextControlInFormEvent = EventManager.RegisterRoutedEvent(
            "WrapCellNextControlInForm",
            RoutingStrategy.Direct,
            typeof(GridWrapCellNextControlInFormEventHandler),
            typeof(GridControlBase));

        /// <summary>
        /// Occurs before the grid is about to be left because the user is at the top-left or bottom-right
        /// cell and about to tab out of the grid.
        /// This event is only raised if the <see cref="GridWrapCellBehavior.NextControlInForm"/>
        /// has been specified for <see cref="GridModelOptions.WrapCell"/>.
        /// </summary>
        /// <seealso cref="GridWrapCellNextControlInFormEventHandler"/>
        public event GridWrapCellNextControlInFormEventHandler WrapCellNextControlInForm
        {
            add
            {
                this.AddHandler(GridControlBase.WrapCellNextControlInFormEvent, value);
            }
            remove
            {
                this.RemoveHandler(GridControlBase.WrapCellNextControlInFormEvent, value);
            }
        }

        /// <summary>
        /// Raises the <see cref="WrapCellNextControlInForm"/> event.
        /// </summary>
        /// <param name="e">A <see cref="GridWrapCellNextControlInFormEventArgs" /> that contains the event data.</param>
        protected virtual void OnWrapCellNextControlInForm(GridWrapCellNextControlInFormEventArgs e)
        {
            base.RaiseEvent(e);
        }

        /// <summary>
        /// Initiates call to <see cref="OnWrapCellNextControlInForm"/>.
        /// </summary>
        /// <param name="e">The event arguments.</param>
        public void RaiseWrapCellNextControlInForm(GridWrapCellNextControlInFormEventArgs e)
        {
            OnWrapCellNextControlInForm(e);
        }

        #endregion
        #region MoveCurrentCellDirection

        public static readonly RoutedEvent MoveCurrentCellDirectionEvent = EventManager.RegisterRoutedEvent(
            "MoveCurrentCellDirection",
            RoutingStrategy.Direct,
            typeof(GridMoveCurrentCellDirectionEventHandler),
            typeof(GridControlBase));

        /// <summary>
        /// Occurs when the direction for current cell movement is changed.
        /// </summary>
        public event GridMoveCurrentCellDirectionEventHandler MoveCurrentCellDirection
        {
            add
            {
                this.AddHandler(GridControlBase.MoveCurrentCellDirectionEvent, value);
            }
            remove
            {
                this.RemoveHandler(GridControlBase.MoveCurrentCellDirectionEvent, value);
            }
        }

        /// <summary>
        /// Raises the  <see cref="MoveCurrentCellDirection"/> event.
        /// </summary>
        /// <param name="e">A <see cref="GridMoveCurrentCellDirectionEventArgs" /> that contains the event data.</param>
        protected virtual void OnMoveCurrentCellDirection(GridMoveCurrentCellDirectionEventArgs e)
        {
            base.RaiseEvent(e);
        }

        internal void RaiseMoveCurrentCellDirection(GridMoveCurrentCellDirectionEventArgs e)
        {
            OnMoveCurrentCellDirection(e);
        }

        #endregion

        #region QueryNextCurrentCellPosition
        public static readonly RoutedEvent QueryNextCurrentCellPositionEvent = EventManager.RegisterRoutedEvent(
            "QueryNextCurrentCellPosition",
            RoutingStrategy.Direct,
            typeof(GridQueryNextCurrentCellPositionEventHandler),
            typeof(GridControlBase));

        /// <summary>
        /// Occurs when the model queries the next position to move the current cell.
        /// </summary>
        public event GridQueryNextCurrentCellPositionEventHandler QueryNextCurrentCellPosition
        {
            add
            {
                this.AddHandler(GridControlBase.QueryNextCurrentCellPositionEvent, value);
            }
            remove
            {
                this.RemoveHandler(GridControlBase.QueryNextCurrentCellPositionEvent, value);
            }
        }

        /// <summary>
        /// Raises the <see cref="QueryNextCurrentCellPosition"/> event.
        /// </summary>
        /// <param name="e">A <see cref="GridQueryNextCurrentCellPositionEventArgs" /> that contains the event data.</param>
        protected virtual void OnQueryNextCurrentCellPosition(GridQueryNextCurrentCellPositionEventArgs e)
        {
            base.RaiseEvent(e);
        }

        /// <summary>
        /// Determines the next position for the current cell for a given direction. Normally, cells that are not
        /// marked as enabled with <see cref="GridStyleInfo.Enabled"/> will be skipped but you can hook into this
        /// mechanism by implementing an event handler for <see cref="QueryNextCurrentCellPosition"/>.
        /// </summary>
        /// <param name="direction">The <see cref="GridDirectionType"/> that specifies the direction of the current cell movement.</param>
        /// <param name="rowIndex">The row index.</param>
        /// <param name="colIndex">The column index.</param>
        /// <returns>True if an enabled cell was found; False otherwise.</returns>
        /// <remarks>
        /// This method will raise the <see cref="GridControlBase.QueryNextCurrentCellPosition"/> event.
        /// In your event handler, you can call <see cref="GridCurrentCell.QueryNextEnabledCell"/> from your QueryNextMoveCell
        /// event handler to find out about the next enabled cell and then decide on further criteria
        /// if the suggested cell is good.
        /// <para/>
        /// See the SampleGrid class in the gridpad sample for an example.
        /// </remarks>
        public bool GetNextCurrentCellPosition(GridDirectionType direction, ref int rowIndex, ref int colIndex)
        {
            GridQueryNextCurrentCellPositionEventArgs e = new GridQueryNextCurrentCellPositionEventArgs(direction, rowIndex, colIndex, GridControlBase.QueryNextCurrentCellPositionEvent, this);
            OnQueryNextCurrentCellPosition(e);
            if (e.Handled)
            {
                rowIndex = e.RowIndex;
                colIndex = e.ColIndex;
                return e.Result;
            }
            else
                return CurrentCell.QueryNextEnabledCell(direction, ref rowIndex, ref colIndex);
        }

        #endregion
        #endregion

        #region ExcelLikeDragDrop

        #region OleDragDrop
        private GridExcelLikeDragDropMouseController oleDataSource = null;

        /// <summary>
        /// Enables OLE Data Source support for this control with default support
        /// for Text and Styles format.
        /// </summary>
        /// <returns>True if support was enabled successfully; False otherwise.</returns>
        public bool EnableOleDataSource()
        {
            return EnableOleDataSource(GridDragDropFlags.Text | GridDragDropFlags.Styles);
        }

        /// <summary>
        /// Enables OLE Data Source support for this control with default support
        /// for Text and Styles format.
        /// </summary>
        /// <param name="flags">See <see cref="GridDragDropFlags"/> for various flags that customize
        /// the OLE Data Source behavior of the grid.</param>
        /// <returns>True if support was enabled successfully; False otherwise.</returns>
        public bool EnableOleDataSource(GridDragDropFlags flags)
        {
            if (oleDataSource != null)
            {
                this.MouseControllerDispatcher.Remove(oleDataSource);
            }

            oleDataSource = new GridExcelLikeDragDropMouseController(this);
            if (oleDataSource != null)
            {
                this.MouseControllerDispatcher.Add(oleDataSource);
                return oleDataSource.EnableExcelLikeDragDrop(flags);
            }

            return false;
        }

        private GridOleDropTarget oleDropTarget = null;

        /// <summary>
        /// Enables OLE Drop Target support for this control with default support
        /// for Text and Styles format.
        /// </summary>
        //// <returns>True if support was enabled successfully; False otherwise.</returns>
        public void EnableOleDropTarget()
        {
            GridDragDropFlags flags = this.Model.Options.DragDropDropTargetFlags;
            if (flags == 0)
            {
                flags = GridDragDropFlags.AutoScroll | GridDragDropFlags.EdgeScroll | GridDragDropFlags.Text | GridDragDropFlags.Styles;
            }

            EnableOleDropTarget(flags);
        }

        /// <summary>
        /// Enables OLE Drop Target support for this control with <see cref="GridDragDropFlags"/>
        /// options specified.
        /// </summary>
        /// <param name="flags">See <see cref="GridDragDropFlags"/> for various flags that customize
        /// the OLE Drop Target behavior of the grid.</param>
        //// <returns>True if support was enabled successfully; False otherwise.</returns>
        public void EnableOleDropTarget(GridDragDropFlags flags)
        {
            if (oleDropTarget == null)
            {
                oleDropTarget = CreateOleDropTarget(this, flags);
            }
        }

        /// <summary>
        /// Creates a GridOleDropTarget object and calls GridOleDropTarget.Register. Override this
        /// method if you want to customize behavior of the GridOleDropTarget object.
        /// </summary>
        /// <param name="grid">The grid control</param>
        /// <param name="flags">Value for DragDropDropTargetFlags</param>
        /// <returns>returns GridOleDropTarget</returns>
        protected virtual GridOleDropTarget CreateOleDropTarget(GridControlBase grid, GridDragDropFlags flags)
        {
            GridOleDropTarget oleDropTarget1 = new GridOleDropTarget(this);
            oleDropTarget1.Register(flags);
            return oleDropTarget1;
        }

        /// <summary>
        /// Registers a <see cref="IGridDataObjectConsumer"/> with the grid that can participate
        /// in an OLE Drop Target operation. If you want to add support for custom clipboard formats,
        /// you should create a class that implements IGridDataObjectConsumer and register it with 
        /// <see cref="RegisterDataObjectConsumer"/>.
        /// </summary>
        /// <param name="consumer">An <see cref="IGridDataObjectConsumer"/> to be added to 
        /// the internal collection of OLE Drop Target consumers.</param>
        public void RegisterDataObjectConsumer(IGridDataObjectConsumer consumer)
        {
            if (oleDropTarget == null)
            {
                EnableOleDropTarget();
            }

            oleDropTarget.RegisterConsumer(consumer);
        }

        //// Events in GridControlBase ...

        #endregion

        #region OleDropTarget

        private GridExcelLikeDragDropMouseController excelLikeDragDrop;

        internal GridExcelLikeDragDropMouseController ExcelLikeDragDrop
        {
            get
            {
                if (this.excelLikeDragDrop == null)
                {
                    this.excelLikeDragDrop = new GridExcelLikeDragDropMouseController(this);
                }

                return excelLikeDragDrop;
            }
        }

        private GridTextDataObjectConsumer textDataObjectConsumer;
        private GridDataDataObjectConsumer dataDataObjectConsumer;


        private void ChangeDataObjectConsumer()
        {
            if ((Model.Options.DataObjectConsumerOptions & GridDataObjectConsumerOptions.None) == 0)//(Model.Options.ControllerOptions & GridControllerOptions.OleDropTarget) != GridControllerOptions.None)
            {
                if ((Model.Options.DataObjectConsumerOptions & GridDataObjectConsumerOptions.Styles) != GridDataObjectConsumerOptions.None)
                {
                    RegisterDataObjectConsumer(dataDataObjectConsumer = new GridDataDataObjectConsumer(this));
                }
                else if (oleDropTarget != null)
                {
                    oleDropTarget.UnregisterConsumer(dataDataObjectConsumer);
                    dataDataObjectConsumer = null;
                }

                if ((Model.Options.DataObjectConsumerOptions & GridDataObjectConsumerOptions.Text) != GridDataObjectConsumerOptions.None)
                {
                    RegisterDataObjectConsumer(textDataObjectConsumer = new GridTextDataObjectConsumer(this));
                }
                else if (textDataObjectConsumer != null)
                {
                    oleDropTarget.UnregisterConsumer(textDataObjectConsumer);
                    textDataObjectConsumer = null;
                }
            }
            else
            {
                if (oleDropTarget != null)
                {
                    if (dataDataObjectConsumer != null)
                    {
                        oleDropTarget.UnregisterConsumer(dataDataObjectConsumer);
                        dataDataObjectConsumer = null;
                    }

                    if (textDataObjectConsumer != null)
                    {
                        oleDropTarget.UnregisterConsumer(textDataObjectConsumer);
                        textDataObjectConsumer = null;
                    }

                    oleDropTarget.Dispose();
                    oleDropTarget = null;
                }
            }
        }


        IGridOleDragDropEventsTarget oleDragDropEventsTarget;

        internal bool IsInDrag
        {
            get;
            private set;
        }

        internal Point DragPoint
        {
            get;
            private set;
        }

        /// <summary>
        /// Gets or sets the OleDragDropEventsTarget. Redirects events defined in <see cref="IGridOleDragDropEventsTarget"/> to the specified object.
        /// Each event will first be called on <see cref="IGridOleDragDropEventsTarget"/> before the actual
        /// event handler in this control is called.
        /// </summary>
        [DesignerSerializationVisibilityAttribute(DesignerSerializationVisibility.Hidden)]
        [Browsable(false)]
        public IGridOleDragDropEventsTarget OleDragDropEventsTarget
        {
            get
            {
                return oleDragDropEventsTarget;
            }

            set
            {
                oleDragDropEventsTarget = value;
            }
        }



        /// <override/>
        protected override void OnDrop(DragEventArgs e)
        {
            if (oleDragDropEventsTarget != null)
            {
                oleDragDropEventsTarget.OnDragDrop(e);
            }

            base.OnDrop(e);
        }

        /// <override/>
        protected override void OnDragEnter(DragEventArgs e)
        {
            if (oleDragDropEventsTarget != null)
            {
                this.DragPoint = e.GetPosition(this);
                this.IsInDrag = true;
                oleDragDropEventsTarget.OnDragEnter(e);
            }

            base.OnDragEnter(e);
        }

        /// <override/>
        protected override void OnDragLeave(DragEventArgs e)
        {
            if (oleDragDropEventsTarget != null)
            {
                oleDragDropEventsTarget.OnDragLeave(e);
            }

            base.OnDragLeave(e);
            this.IsInDrag = false;
        }

        /// <override/>
        protected override void OnDragOver(DragEventArgs e)
        {
            if (oleDragDropEventsTarget != null)
            {
                if (this.IsInDrag)
                {
                    this.DragPoint = e.GetPosition(this);
                }

                oleDragDropEventsTarget.OnDragOver(e);
            }

            base.OnDragOver(e);
        }

        #endregion


        #region ExcelLikeDragDropSource


        public event GridExcelLikeDragRangeEventHandler QueryCanDragRange;

        /// <summary>
        /// Initiates call to <see cref="OnQueryCanOleDragRange"/>.
        /// </summary>
        /// <param name="e">The event arguments.</param>
        public void RaiseQueryCanOleDragRange(GridQueryCanDragRangeEventArgs e)
        {
            OnQueryCanDragRange(e);
        }

        /// <summary>
        /// Raises the <see cref="GridControlBase.QueryCanOleDragRange" /> event.
        /// </summary>
        /// <param name="e">An <see cref="GridQueryCanOleDragRangeEventArgs" /> that contains the event data.</param>
        protected virtual void OnQueryCanDragRange(GridQueryCanDragRangeEventArgs e)
        {
            //if (this.eventsTarget != null)
            //{
            //    this.eventsTarget.OnQueryCanOleDragRange(e);
            //}


            if (QueryCanDragRange != null)
            {
                try
                {
                    QueryCanDragRange(this, e);
                }
                catch (Exception ex)
                {
                    TraceUtil.TraceExceptionCatched(ex);
                    if (!ExceptionManager.RaiseExceptionCatched(this, ex))
                    {
                        throw;
                    }

                    e.Cancel = true;
                }
            }
        }

        #endregion

        private GridControlDragAutoScroller autoScroller;
        /// <summary>
        /// Gets the auto scroller which provides automatic scrolling of content when the user drags the pressed
        /// mouse to an edge of the control.
        /// </summary>
        /// <value>The auto scroller.</value>
        public new GridControlDragAutoScroller AutoScroller
        {
            get
            {
                if (this.autoScroller == null)
                {
                    this.autoScroller = new GridControlDragAutoScroller(this);
                }

                return this.autoScroller;
            }
        }

        #endregion

        internal void RenderExcelLikeBorder(Rect r)
        {
            var coll = new DoubleCollection();
            coll.Add(4);
            coll.Add(4);
            DrawingContext dccBorder = dvCurrentCellBorder.RenderOpen();
            Pen pen = new Pen()
            {
                Brush = new SolidColorBrush(Colors.Black),
                DashStyle = new DashStyle()
                {
                    Dashes = coll
                },
                Thickness = 2
            };

            OnRenderSelectionBorder(dccBorder, r, CellBorderSide.Left, pen);
            OnRenderSelectionBorder(dccBorder, r, CellBorderSide.Top, pen);
            OnRenderSelectionBorder(dccBorder, r, CellBorderSide.Right, pen);
            OnRenderSelectionBorder(dccBorder, r, CellBorderSide.Bottom, pen);
            dccBorder.Close();
        }

        public override void Dispose(bool disposing)
        {
            if (this.model != null)
            {
                this.UnwireModelEvents();
                model.CommitCellInfo -= new GridCommitCellInfoEventHandler(modelCommitCellInfo);
            }
            this.dvSelectBackground = null;
            this.dvSelectBorder = null;
            this.dvHiddenBorder = null;
            this.dvCurrentCellBorder = null;
            if (this.cellRenderers != null)
            {
                this.cellRenderers.Dispose();
                this.cellRenderers = null;
            }
            if(this.RenderStyles!=null)
                this.RenderStyles.Clear();
            if (this.currentCell != null)
            {
                this.currentCell.Dispose();
                this.currentCell = null;
            }
            if (this.autoScroller != null)
                this.autoScroller = null;
            base.Dispose(disposing);
        }
        internal virtual Rect RenderExcelRangeBorder(GridRangeInfo range)
        {
            if (Model.Options.ExcelLikeSelectionFrame)
            {
                // Rect gridRect = new Rect(0, 0, RenderSize.Width, RenderSize.Height); Unused local variable
                GridRangeInfo selectedCells = range;

                if (!selectedCells.IsEmpty)
                {
                    selectedCells = this.ExpandSelectedCellsRange(selectedCells);
                    DoubleSpan[] yPos = ScrollRows.RangeToRegionPoints(selectedCells.Top, selectedCells.Bottom, true);
                    DoubleSpan[] xPos = ScrollColumns.RangeToRegionPoints(selectedCells.Left, selectedCells.Right, true);

                    for (int rowRegion = 0; rowRegion < 3; rowRegion++)
                    {
                        if (yPos[rowRegion].IsEmpty)
                            continue;

                        for (int columnRegion = 0; columnRegion < 3; columnRegion++)
                        {
                            if (xPos[columnRegion].IsEmpty)
                                continue;

                            Rect clipRect = GetClipRect((ScrollAxisRegion)rowRegion, (ScrollAxisRegion)columnRegion);
                            Rect r = new Rect(xPos[columnRegion].Start, yPos[rowRegion].Start, xPos[columnRegion].Length, yPos[rowRegion].Length);

                            if (!clipRect.IntersectsWith(r))
                                continue;

                            return r;
                        }
                    }
                }
            }

            return new Rect(0, 0, 0, 0);
        }


        #region CurrentCell Key Input

        protected override void OnPreviewTextInput(TextCompositionEventArgs e)
        {
            base.OnPreviewTextInput(e);

            //if (!e.Handled)
            if (!e.Handled && e.ControlText == string.Empty)
            {
                IGraphicCellRenderer graphicRenderer = this.model.GraphicModel.CurrentCellRenderers;
                if (graphicRenderer != null)
                    return;
                IGridCellRenderer renderer = CurrentCell.Renderer;
                if (renderer != null)
                    renderer.RaiseGridPreviewTextInput(e);
            }
        }

        protected internal virtual bool ShouldGridTryToHandlePreviewKeyDown(KeyEventArgs e)
        {
            IGridCellRenderer renderer = CurrentCell.Renderer;
            if (renderer != null)
                return renderer.ShouldGridTryToHandlePreviewKeyDown(e);

            return true;
        }

        #region CurrentCellPreviewKeyDown

        protected virtual bool OnCurrentCellPreviewKeyDown(GridCellKeyEventArgs e)
        {
            base.RaiseEvent(e);
            return e.Cancel;
        }

        public static readonly RoutedEvent CurrentCellPreviewKeyDownEvent = EventManager.RegisterRoutedEvent(
            "CurrentCellPreviewKeyDown",
            RoutingStrategy.Tunnel,
            typeof(GridCellKeyEventHandler),
            typeof(GridControlBase));

        /// <summary>
        /// Occurs during the PreviewKeyDown event for the current cell.
        /// </summary>
        public event GridCellKeyEventHandler CurrentCellPreviewKeyDown
        {
            add
            {
                this.AddHandler(GridControlBase.CurrentCellPreviewKeyDownEvent, value);
            }
            remove
            {
                this.RemoveHandler(GridControlBase.CurrentCellPreviewKeyDownEvent, value);
            }
        }

        #endregion

        internal bool cellPreviewHandled;
        /// <summary>
        /// Flag to Handle the PreviewKeyDown event in CurrentRecordManager
        /// </summary>
        internal bool shouldGridTryToHandlePreviewKeyDown;

        protected override void OnPreviewKeyDown(KeyEventArgs e)
        {
            base.OnPreviewKeyDown(e);

            this.IsInShiftTab = false;
            this.CheckShiftTab = false;
            bool isShiftKey = (e.KeyboardDevice.Modifiers & ModifierKeys.Shift) != ModifierKeys.None;
            //this.IsInShiftTab = isShiftKey ? true : false;
            this.IsInShiftTab = isShiftKey && e.Key == Key.Tab;
            this.CheckShiftTab = IsInShiftTab;
            if (!e.Handled)
            {
                 graphicCellPreviewHandled = ShouldGraphicCellTryToHandlePreviewKeyDown(e);
                 if (!graphicCellPreviewHandled)
                 {
                     cellPreviewHandled = this.OnCurrentCellPreviewKeyDown(new GridCellKeyEventArgs(GridControlBase.CurrentCellPreviewKeyDownEvent, this, e));
                     e.Handled = cellPreviewHandled;
                     if (!e.Handled)
                     {
                         shouldGridTryToHandlePreviewKeyDown = ShouldGridTryToHandlePreviewKeyDown(e);
                         if (!cellPreviewHandled && shouldGridTryToHandlePreviewKeyDown)
                         {
                             MoveCurrentCellWithArrowKey(e);
                         }
                     }
                 }
            }
        }

        #region CurrentCellKeyDown

        protected virtual bool OnCurrentCellKeyDown(GridCellKeyEventArgs e)
        {
            if (e.Source is GridDataControlBaseImpl)
            {
                //While deleting nested child record and parent record Message box will shown more than one times. To avoid this we have use the flag IsDeleteMessageBoxShown. This should reset after multi records deleted. 
                var grid = e.Source as GridDataControlBaseImpl;
                grid.TableModel.CurrencyManager.IsDeleteMessageBoxShown = false;
            }
            
            base.RaiseEvent(e);
            return e.Handled;
        }

        public static readonly RoutedEvent CurrentCellKeyDownEvent = EventManager.RegisterRoutedEvent(
            "CurrentCellKeyDown",
            RoutingStrategy.Tunnel,
            typeof(GridCellKeyEventHandler),
            typeof(GridControlBase));

        /// <summary>
        /// Occurs during a key press in the current cell.
        /// </summary>
        public event GridCellKeyEventHandler CurrentCellKeyDown
        {
            add
            {
                this.AddHandler(GridControlBase.CurrentCellKeyDownEvent, value);
            }
            remove
            {
                this.RemoveHandler(GridControlBase.CurrentCellKeyDownEvent, value);
            }
        }

        #endregion

        protected override void OnKeyDown(KeyEventArgs e)
        {
            base.OnKeyDown(e);
            if (!e.Handled) 
            {
                graphicCellPreviewHandled = ShouldGraphicCellTryToHandlePreviewKeyDown(e);
                if (!graphicCellPreviewHandled)
                {
                    cellPreviewHandled = this.OnCurrentCellKeyDown(new GridCellKeyEventArgs(GridControlBase.CurrentCellKeyDownEvent, this, e));
                    e.Handled = cellPreviewHandled;
                    if (!e.Handled)
                    {
                        shouldGridTryToHandlePreviewKeyDown = ShouldGridTryToHandlePreviewKeyDown(e);
                        if (!cellPreviewHandled && shouldGridTryToHandlePreviewKeyDown)
                        {
                            MoveCurrentCellWithArrowKey(e);
                        }
                    }
                }
            }
        }

        internal bool graphicCellPreviewHandled;
        protected internal virtual bool ShouldGraphicCellTryToHandlePreviewKeyDown(KeyEventArgs e)
        {
            if (this.Model.GraphicModel != null && this.Model.GraphicModel.CurrentCellRenderers != null)
            {
                return this.Model.GraphicModel.CurrentCellRenderers.ShouldTryToHandlePreviewKeyDown(e);
            }
            return false;
        }

        internal bool IsInShiftTab
        {
            get;
            set;
        }
        //below flag is to check tab navigation between controls.
        internal bool CheckShiftTab
        {
            get;
            set;
        }

        /// <summary>
        /// Moves the current cell in the direction indicated by the arrow keys.
        /// </summary>
        /// <param name="e">The <see cref="KeyEventArgs"/> object.</param>
        /// <returns>True if the operation was successful; False otherwise.</returns>
        public virtual bool MoveCurrentCellWithArrowKey(KeyEventArgs e)
        {
            bool isControlKey = (e.KeyboardDevice.Modifiers & ModifierKeys.Control) != ModifierKeys.None;
            bool isShiftKey = (e.KeyboardDevice.Modifiers & ModifierKeys.Shift) != ModifierKeys.None;
            RowColumnIndex rc = CurrentCell.CellRowColumnIndex;

            if (rc.RowIndex < 0 && rc.ColumnIndex < 0)
            {
                return false;
            }

            if ((e.KeyboardDevice.Modifiers & ModifierKeys.Alt) == ModifierKeys.None)
            {
                bool isRightToLeft = base.FlowDirection == FlowDirection.RightToLeft;
                switch (e.Key)
                {
                    case Key.Escape:
                        if (CurrentCell.IsEditing)
                            CurrentCell.CancelEdit();
                        ///The below code will Reset the cache when you call the CancelEdit. Otherwise cache will not be cleared.
                        if((this is GridDataControlBaseImpl &&((GridDataControlBaseImpl)this) != null))
                            (((GridDataControlBaseImpl)this).Model as GridDataTableModel).CurrencyManager.ResetCache();
                        e.Handled = true;
                        break;

                    case Key.F2:
                        if (CurrentCell.IsEditing)
                        {
                            CurrentCell.EndEdit();                            
                        }
                        else
                            CurrentCell.BeginEdit();
                        e.Handled = true;
                        break;

                    case Key.Enter:
                        if (isShiftKey)
                        {
                            break;
                        }
                        else
                        {
                            if (CurrentCell.IsEditing)
                                CurrentCell.EndEdit();
                            CurrentCell.ScrollInView();
                            e.Handled = true;
                            break;                       
                        }
                        
                        
                       
                    case Key.Tab:
                        if (!isShiftKey)
                        {
                            if (this.Model is GridDataTableModel)
                            {
                                var columnCollection = (this.Model as GridDataTableModel).TableProperties.VisibleColumns.Where(c => c.IsHidden == false);
                                var lastColumn = columnCollection.Last();

                                if (this.Model.Options.WrapCellBehavior == GridWrapCellBehavior.NextControlInForm
                                   && CurrentCell.RowIndex == this.Model.RowCount - 1
                                   && CurrentCell.ColumnIndex == (this.model as GridDataTableModel).TableProperties.VisibleColumns.IndexOf(lastColumn))
                                {
                                    if (CurrentCell.IsEditing)
                                        CurrentCell.EndEdit();
                                    return false;
                                }
                            }
                            else
                            {
                                if (this.Model.Options.WrapCellBehavior == GridWrapCellBehavior.NextControlInForm
                               && CurrentCell.RowIndex == this.Model.RowCount - 1
                               && CurrentCell.ColumnIndex == this.Model.ColumnCount - 1)
                                {
                                    if (CurrentCell.IsEditing)
                                        CurrentCell.EndEdit();
                                    return false;
                                }
                            }
                        }
                        else
                        {
                            if (this.model.Options.WrapCellBehavior == GridWrapCellBehavior.NextControlInForm 
                                && CurrentCell.RowIndex == 1 
                                && CurrentCell.ColumnIndex == 0)
                            {
                                if (CurrentCell.IsEditing)
                                    CurrentCell.EndEdit();
                                return false;
                            }
                        }

                        if (!isRightToLeft)
                        {
                            //Excel like tab navigation
                            if (Model.Options.ExcelLikeTabNavigation)
                            {
                                MoveNextNotReadOnlyCell(isShiftKey);
                                currentCell.ScrollInView();
                                e.Handled = true;
                                return rc != CurrentCell.CellRowColumnIndex;
                            }
                            if (isShiftKey)
                            {
                                if (this.Model.Options.AllowSelectionOnShiftTab)
                                {
                                    CurrentCell.MoveLeft();
                                }
                                else
                                {
                                    CurrentCell.MoveLeft(1, false);
                                }
                            }
                            else
                                CurrentCell.MoveRight();
                        }
                        else
                        {
                            if (isShiftKey)
                                CurrentCell.MoveRight();
                            else
                                CurrentCell.MoveLeft();
                        }
                        e.Handled = true;
                        return rc != CurrentCell.CellRowColumnIndex;

                    case Key.Prior:
                        if (isControlKey)
                            CurrentCell.PageLeft();
                        else
                            CurrentCell.PageUp();
                        e.Handled = true;
                        return rc != CurrentCell.CellRowColumnIndex;

                    case Key.Next:
                        if (isControlKey)
                            CurrentCell.PageRight();
                        else
                            CurrentCell.PageDown();
                        e.Handled = true;
                        return rc != CurrentCell.CellRowColumnIndex;

                    case Key.End:
                        if (isControlKey && !this.CurrentCell.IsEditing)
                            CurrentCell.MoveToBottomRight();
                        else
                            CurrentCell.MoveToRightEnd();
                        e.Handled = true;
                        return rc != CurrentCell.CellRowColumnIndex;

                    case Key.Home:
                        if (isControlKey && !this.CurrentCell.IsEditing)
                            CurrentCell.MoveToTopLeft();
                        else
                            CurrentCell.MoveToLeftEnd();
                        e.Handled = true;
                        return rc != CurrentCell.CellRowColumnIndex;

                    case Key.Left:
                        if (!isRightToLeft)
                        {
                            if (isControlKey && !this.CurrentCell.IsEditing)
                                CurrentCell.MoveToLeftEnd();
                            else
                                CurrentCell.MoveLeft();
                        }
                        else
                        {
                            if (isControlKey)
                                CurrentCell.MoveToLeftEnd();
                            else if(!this.CurrentCell.IsEditing)
                                CurrentCell.MoveRight();
                        }
                        e.Handled = true;
                        return rc != CurrentCell.CellRowColumnIndex;

                    case Key.Up:
                        if (isControlKey && !this.CurrentCell.IsEditing)
                            CurrentCell.MoveToTop();
                        else 
                            CurrentCell.MoveUp();
                        e.Handled = true;
                        return rc != CurrentCell.CellRowColumnIndex;

                    case Key.Right:
                        if (!isRightToLeft)
                        {
                            if (isControlKey && !this.CurrentCell.IsEditing)
                                CurrentCell.MoveToRightEnd();
                            else 
                                CurrentCell.MoveRight();
                        }
                        else
                        {
                            if (isControlKey)
                                CurrentCell.MoveToLeftEnd();
                            else if(!this.CurrentCell.IsEditing)
                                CurrentCell.MoveLeft();
                        }
                        e.Handled = true;
                        return rc != CurrentCell.CellRowColumnIndex;

                    case Key.Down:
                        if (isControlKey && !this.CurrentCell.IsEditing)
                            CurrentCell.MoveToBottom();
                        else 
                            CurrentCell.MoveDown();
                        e.Handled = true;
                        return rc != CurrentCell.CellRowColumnIndex;

                    case Key.A:
                        if (isControlKey && !this.CurrentCell.IsEditing && this.Model.Options.AllowSelection != GridSelectionFlags.None)
                        {
                            if (!this.model.Options.ExcelLikeCurrentCell)
                                CurrentCell.MoveToTopLeft();
                            this.model.Selections.Clear();
                            this.model.Selections.Add(GridRangeInfo.Table());
                            e.Handled = true;
                        }
                        return rc != CurrentCell.CellRowColumnIndex;
                }
            }
            return e.Handled;
        }

        /// <summary>
        /// Find the next editable cell
        /// </summary>
        /// <param name="isShift">If set to <see langword="true"/>, then move right to left; otherwise, Move left to right.</param>
        /// <remarks></remarks>
        private void MoveNextNotReadOnlyCell(bool isShift)
        {
            GridRangeInfo gridCells = NavigateWithArrowKeysCellsRange;
            int targetRow;
            int targetCol;
            GridRangeInfo coveredRange;
            GridRenderStyleInfo style;
            bool readOnly = true;
            int loopCount = 0;
            if (!isShift)
            {
                targetRow = CurrentCell.RowIndex;
                targetCol = CurrentCell.ColumnIndex + 1;
                // var moveNextRow = false; Unused local variable
                while (readOnly)
                {
                    // Skip invisible and covered cells.
                    while (targetCol <= gridCells.Right && (GetColWidth(targetCol) == 0
                        || Model.CoveredRanges.Find(targetRow, targetCol, out coveredRange)
                        && coveredRange.Left != targetCol))
                    {
                        targetCol = ScrollColumns.GetNextScrollLineIndex(targetCol);
                        if (targetCol == -1)
                        {
                            // we reached the end move to next row
                            targetRow = ScrollRows.GetNextScrollLineIndex(targetRow);
                            targetCol = gridCells.Left;
                        }
                    }

                    if (targetCol <= gridCells.Right)
                    {
                        Model.CoveredRanges.Find(targetRow, targetCol, out coveredRange);
                        style = GetRenderStyleInfo(coveredRange.Top, coveredRange.Left, true);
                        readOnly = style.ReadOnly;
                        style.Dispose();

                        //If Read Only, continue search.
                        if (readOnly)
                            targetCol = ScrollColumns.GetNextScrollLineIndex(targetCol);
                    }

                    if (targetCol > gridCells.Right || targetCol == -1)
                    {
                        targetRow = ScrollRows.GetNextScrollLineIndex(targetRow);
                        targetCol = gridCells.Left;
                        if (targetRow > gridCells.Bottom)
                        {
                            targetRow = gridCells.Top;
                            loopCount += 1;
                        }
                        if (loopCount > 1)
                            break;
                    }
                }

            }
            else
            {
                targetRow = CurrentCell.RowIndex;
                targetCol = CurrentCell.ColumnIndex - 1;
                while (readOnly)
                {
                    // Skip invisible and covered cells.
                    while (targetCol >= gridCells.Left && (GetColWidth(targetCol) == 0
                        || Model.CoveredRanges.Find(targetRow, targetCol, out coveredRange)
                        && coveredRange.Left != targetCol))
                        targetCol = ScrollColumns.GetPreviousScrollLineIndex(targetCol);

                    if (targetCol >= gridCells.Left)
                    {
                        Model.CoveredRanges.Find(targetRow, targetCol, out coveredRange);
                        style = GetRenderStyleInfo(coveredRange.Top, coveredRange.Left, true);
                        readOnly = style.ReadOnly;
                        style.Dispose();

                        //If Read only, continue search.
                        if (readOnly)
                            targetCol = ScrollColumns.GetPreviousScrollLineIndex(targetCol);
                    }

                    if (targetCol < gridCells.Left || targetCol == -1)
                    {
                        targetRow = ScrollRows.GetPreviousScrollLineIndex(targetRow);
                        targetCol = gridCells.Right;
                        if (targetRow < gridCells.Top)
                        {
                            targetRow = gridCells.Bottom;
                            loopCount += 1;
                        }
                        if (loopCount > 1)
                            break;
                    }
                }
            }
            GridActivateCurrentCellOptions options = new GridActivateCurrentCellOptions(GridSetCurrentCellOptions.ScrollInView);
            if (!Model.Options.ExcelLikeCurrentCell)
                options.SetCurrentCellOptions |= GridSetCurrentCellOptions.NoSelectRange;
            if (!readOnly)
                CurrentCell.MoveTo(targetRow, targetCol, options);
        }


        #endregion

        private bool allowDragColumns = false;
        /// <summary>
        /// Gets or sets a value indicating whether the grid columns can be dragged.
        /// </summary>
        public bool AllowDragColumns
        {
            get
            {
                return this.allowDragColumns;
            }

            set
            {
                var controller = this.MouseControllerDispatcher.Find(GridDragColumnHeaderMouseController.GridDragColumnHeaderMouseControllerName);
                if (value)
                {
                    if (controller == null)
                    {
                        this.MouseControllerDispatcher.Add(new GridDragColumnHeaderMouseController(this));
                    }
                }
                else
                {
                    // if false and we found a controller then remove it
                    if (controller != null)
                    {
                        this.MouseControllerDispatcher.Remove(controller);
                    }
                }
                this.allowDragColumns = value;
            }
        }


        private bool allowDragDrop = false;
        /// <summary>
        /// Gets or sets a value indicating whether the grid columns can be dragged.
        /// </summary>
        public bool AllowDragDrop
        {
            get
            {
                return this.allowDragDrop;
            }

            set
            {
                var controller = this.MouseControllerDispatcher.Find("ExcelLikeDragDrop");
                if (value)
                {
                    if (controller == null)
                    {
                        this.MouseControllerDispatcher.Add(this.ExcelLikeDragDrop);
                    }
                }
                else
                {
                    // if false and we found a controller then remove it
                    if (controller != null)
                    {
                        this.MouseControllerDispatcher.Remove(controller);
                    }
                }

                this.allowDragDrop = value;
            }
        }
        //internal GridStyleInfo GetRenderStyleInfo(int rowIndex, int columnIndex)
        //{
        //    return GetRenderStyleInfo(rowIndex, columnIndex);
        //}

        /// <summary>
        /// Suspend formula calculation while rendering the grid
        /// </summary>
        public void SuspendFormulaCalculation()
        {
            if (this.Model != null)
            {
                this.Model.EnableFormulaCalculations = false;
            }
        }

        /// <summary>
        /// Resume the formula calculation while rendering the grid
        /// </summary>
        /// <param name="Range"></param>
        public void ResumeFormulaCalculation(GridRangeInfo Range)
        {
            if (this.Model != null)
            {
                this.Model.EnableFormulaCalculations = true;
                this.Model.FormulaEngine.RecalculateRange(Range, this.Model, false, true);
            }
        }

        internal bool IsDesignMode()
        {
            return false;
        }

        internal double GetRowHeight(int rowIndex)
        {
            return RowHeights[rowIndex];
        }

        internal double GetColWidth(int columnIndex)
        {
            return ColumnWidths[columnIndex];
        }

        internal int InternalGetFrozenRows()
        {
            return RowHeights.HeaderLineCount;
        }

        internal int InternalGetFrozenCols()
        {
            return ColumnWidths.HeaderLineCount;
        }

        internal int InternalGetFooterRows()
        {
            return RowHeights.FooterLineCount;
        }

        internal int InternalGetFooterColumns()
        {
            return ColumnWidths.FooterLineCount;
        }

        internal int InternalGetHeaderRows()
        {
            return Model.HeaderRows;
        }

        internal int InternalGetHeaderCols()
        {
            return Model.HeaderColumns;
        }

        internal int GetClientRow(int currentRow)
        {
            VisibleLineInfo line = ScrollRows.GetVisibleLineAtLineIndex(currentRow);
            return line.VisibleIndex;
        }

        internal int GetClientCol(int currentCol)
        {
            VisibleLineInfo line = ScrollColumns.GetVisibleLineAtLineIndex(currentCol);
            if (line != null)
            {
                return line.VisibleIndex;
            }

            return -1;
        }

        internal int GetRow(int currentRow)
        {
            if (ScrollRows.GetVisibleLines().Count > currentRow)
            {
                VisibleLineInfo line = ScrollRows.GetVisibleLines()[currentRow];
                return line.LineIndex;
            }
            else
            {
                return -1;
            }
        }

        internal int GetCol(int currentCol)
        {
            if (currentCol > -1)
            {
                VisibleLineInfo line = ScrollColumns.GetVisibleLines()[currentCol];
                if (line != null)
                {
                    return line.LineIndex;
                }
            }

            return -1;
        }

        internal bool ScrollGridGetPrevRowIndex(ref int newRowIndex)
        {
            int n = ScrollRows.GetPreviousScrollLineIndex(newRowIndex);
            if (n == newRowIndex)
                return false;
            newRowIndex = n;
            return true;
        }

        internal bool ScrollGridGetPrevColIndex(ref int newRowIndex)
        {
            int n = ScrollColumns.GetPreviousScrollLineIndex(newRowIndex);
            if (n == newRowIndex)
                return false;
            newRowIndex = n;
            return true;
        }

        internal void ScrollGridGetNextRowIndex(ref int newRowIndex, bool p)
        {
            newRowIndex = ScrollRows.GetNextScrollLineIndex(newRowIndex);
        }

        internal void ScrollGridGetNextColIndex(ref int newRowIndex, bool p)
        {
            newRowIndex = ScrollColumns.GetNextScrollLineIndex(newRowIndex);
        }

        [Obsolete("Use LeftColumnIndex")]
        internal int LeftColIndex
        {
            get
            {
                return LeftColumnIndex;
            }
            set
            {
                LeftColumnIndex = value;
            }
        }

        internal int GetFirstScrollableRow()
        {
            return NavigateWithArrowKeysCellsRange.Top;
        }


        internal int GetFirstScrollableCol()
        {
            return NavigateWithArrowKeysCellsRange.Left;
        }

        private DataTemplate defaultTooltipTemplate = null;
        internal DataTemplate GetDefaultTooltipTemplate()
        {
            this.defaultTooltipTemplate = null; 
            if (this.defaultTooltipTemplate == null)
            {
                var rd = new ResourceDictionary()
                {
                    Source = new Uri("/Syncfusion.Grid.Wpf;component/GridControl/Themes/Generic.xaml", UriKind.RelativeOrAbsolute)
                };
                //rd.Source = GridUtil.GetXamlConvertedValue<Uri>("Syncfusion.Grid.Wpf;component/GridControl/Themes/Generic.xaml");
                this.defaultTooltipTemplate = rd["tooltipTemplate"] as DataTemplate;
            }

            return this.defaultTooltipTemplate;
        }

        private DataTemplate validationTemplate = null;
        internal DataTemplate GetDataValidationTemplate()
        {
            if (this.validationTemplate == null)
            {
                var rd = new ResourceDictionary();
                rd.Source = GridUtil.GetXamlConvertedValue<Uri>("Syncfusion.Grid.Wpf;component/GridControl/Themes/Generic.xaml");
                this.validationTemplate = rd["datavalidationtooltipTemplate"] as DataTemplate;
            }
            return this.validationTemplate;
        }

        internal DataTemplate GetCustomTooltipTemplate()
        {
            var rd = new ResourceDictionary()
            {
                Source = new Uri("/Syncfusion.Grid.Wpf;component/GridControl/Themes/Generic.xaml", UriKind.RelativeOrAbsolute)
            };
            //rd.Source = GridUtil.GetXamlConvertedValue<Uri>("Syncfusion.Grid.Wpf;component/GridControl/Themes/Generic.xaml");
            this.defaultTooltipTemplate = rd["customTooltipTemplate"] as DataTemplate;

            return this.defaultTooltipTemplate;
        }

        private DataTemplate defaultCommentTemplate = null;
        internal DataTemplate GetDefaultCommentTemplate()
        {
            if (this.defaultCommentTemplate == null)
            {
                var rd = new ResourceDictionary()
                {
                    Source = new Uri("/Syncfusion.Grid.Wpf;component/GridControl/Themes/Generic.xaml", UriKind.RelativeOrAbsolute)
                };
                //rd.Source = GridUtil.GetXamlConvertedValue<Uri>("Syncfusion.Grid.Wpf;component/GridControl/Themes/Generic.xaml");
                this.defaultCommentTemplate = rd["commentTemplate"] as DataTemplate;
            }

            return this.defaultCommentTemplate;
        }



        private DataTemplate defaultErrorTooltipTemplate = null;
        internal DataTemplate GetDefaultErrorTemplate()
        {
            if (this.defaultErrorTooltipTemplate == null)
            {
                //errorTemplate
                var rd = new ResourceDictionary()
                {
                    Source = new Uri("/Syncfusion.Grid.Wpf;component/GridControl/Themes/Generic.xaml", UriKind.RelativeOrAbsolute)
                };
                //rd.Source = GridUtil.GetXamlConvertedValue<Uri>("Syncfusion.Grid.Wpf;component/GridControl/Themes/Generic.xaml");
                this.defaultErrorTooltipTemplate = rd["errorTemplate"] as DataTemplate;
            }

            return this.defaultErrorTooltipTemplate;
        }

        private Brush errorMessageBrush = null;
        internal Brush GetErrorMessageBrush()
        {
            if (this.errorMessageBrush == null)
            {
                var rd = new ResourceDictionary()
                {
                    Source = new Uri("/Syncfusion.Grid.Wpf;component/GridControl/Themes/Generic.xaml", UriKind.RelativeOrAbsolute)
                };
                //rd.Source = GridUtil.GetXamlConvertedValue<Uri>("Syncfusion.Grid.Wpf;component/GridControl/Themes/Generic.xaml");
                this.errorMessageBrush = rd["errorMessage"] as Brush;
            }

            return this.errorMessageBrush;
        }

        private Brush errorInformationBrush = null;
        internal Brush GetErrorInformationBrush()
        {
            if (this.errorInformationBrush == null)
            {
                var rd = new ResourceDictionary()
                {
                    Source = new Uri("/Syncfusion.Grid.Wpf;component/GridControl/Themes/Generic.xaml", UriKind.RelativeOrAbsolute)
                };
                //rd.Source = GridUtil.GetXamlConvertedValue<Uri>("Syncfusion.Grid.Wpf;component/GridControl/Themes/Generic.xaml");
                this.errorInformationBrush = rd["errorInformation"] as Brush;
            }

            return this.errorInformationBrush;
        }

        /// <summary>
        /// Brings the given cell into view.
        /// </summary>
        /// <param name="cellRowColumnIndex">The row and column indices as <see cref="RowColumnIndex"/>.</param>
        public virtual void ScrollInView(RowColumnIndex cellRowColumnIndex)
        {
            if (this.ScrollOwner == null || !ScrollOwner.CanContentScroll)
            {
                Rect rect = RangeToClippedVisibleRect(cellRowColumnIndex, true);
                IScrollInfo scp = GetScrollContentPresenter();
                if (scp != null)
                    scp.MakeVisible(this, rect);
            }
            else
            {
                ScrollRows.ScrollInView(cellRowColumnIndex.RowIndex);
                ScrollColumns.ScrollInView(cellRowColumnIndex.ColumnIndex);
            }
        }

        private IScrollInfo GetScrollContentPresenter()
        {
            IScrollInfo scp = (IScrollInfo)GetParentTypeOf(this, typeof(IScrollInfo));
            while (scp != null && !(scp.CanHorizontallyScroll || scp.CanVerticallyScroll))
            {
                DependencyObject dpo = (DependencyObject)scp;
                scp = (IScrollInfo)GetParentTypeOf(VisualTreeHelper.GetParent(dpo), typeof(IScrollInfo));
            }
            return scp;
        }

        DependencyObject GetParentTypeOf(DependencyObject dpo, Type type)
        {
            while (dpo != null && !type.IsAssignableFrom(dpo.GetType()))
            {
                if (dpo is Visual)
                    dpo = VisualTreeHelper.GetParent(dpo);
                else
                    dpo = LogicalTreeHelper.GetParent(dpo);
            }
            return dpo;
        }

        /// <summary>
        /// Returns a System.String that represents the current object.
        /// </summary>
        public string PaneDesc
        {
            get { return ToString(); }
        }

        #region CurrentCell Events

        internal RowColumnIndex rowColumnUnderMouse = RowColumnIndex.Empty;
        internal CellSpanBackgroundInfo rowSpanUnderMouse = null;
        internal CellSpanBackgroundInfo oldrowSpanUnderMouse = null;

        /// <summary>
        /// Implements handling for the PreviewMouseMove�event. When
        /// no mouse button is pressed and the mouse is over a cell it calls <see cref="DelayedCreateCellUIElements"/>.
        /// </summary>
        /// <param name="e">The <see cref="System.Windows.Input.MouseEventArgs"/> that contains the event data.</param>
        protected override void OnPreviewMouseMove(MouseEventArgs e)
        {
            RowColumnIndex rci = PointToCellRowColumnIndex(e);
            if (!rci.IsEmpty)
            {
                IGridCellRenderer renderer = RenderStyles[rci].CellRenderer;
                if (renderer != null)
                    renderer.RaiseGridPreviewMouseMove(rci, e);

                if ((this.Model as GridDataTableModel) != null)
                {
                    if ((this.Model as GridDataTableModel).TableProperties.ShowHoveringBackground)
                    {
                        if (rci.RowIndex != rowColumnUnderMouse.RowIndex
                            || (rci.RowIndex == rowColumnUnderMouse.RowIndex && rci.ColumnIndex != rowColumnUnderMouse.ColumnIndex))
                        {
                            rowColumnUnderMouse = rci;
                            if (rowSpanUnderMouse != null)
                            {
                                oldrowSpanUnderMouse = rowSpanUnderMouse;
                                this.InvalidateCell(rowSpanUnderMouse);
                                rowSpanUnderMouse = null;
                            }
                            GridDataControl grid = this.FindParentElementOfType<GridDataControl>();
                            if (grid != null)
                            {
                                if (grid.ShowAddNewRow && grid.AddNewRowPosition == Position.Top)
                                {
                                    if (rci.RowIndex > 1)
                                    {
                                        rowSpanUnderMouse = new CellSpanBackgroundInfo(rci.RowIndex, 0, rci.RowIndex, this.Model.ColumnCount);
                                        if (!this.Model.CellSpanBackgrounds.Contains(rowSpanUnderMouse))
                                        {
                                            this.InvalidateCell(rowSpanUnderMouse);
                                        }
                                    }
                                }
                                else
                                {
                                    if (rci.RowIndex > 0)
                                    {
                                        rowSpanUnderMouse = new CellSpanBackgroundInfo(rci.RowIndex, 0, rci.RowIndex, this.Model.ColumnCount);
                                        if (!this.Model.CellSpanBackgrounds.Contains(rowSpanUnderMouse))
                                        {
                                            this.InvalidateCell(rowSpanUnderMouse);
                                        }
                                    }
                                }
                            }
                            else
                            {
                                if (rci.RowIndex > 0)
                                {
                                    rowSpanUnderMouse = new CellSpanBackgroundInfo(rci.RowIndex, 0, rci.RowIndex, this.Model.ColumnCount);
                                    if (!this.Model.CellSpanBackgrounds.Contains(rowSpanUnderMouse))
                                    {
                                        this.InvalidateCell(rowSpanUnderMouse);
                                    }
                                }

                            }
                        }
                    }
                }
            }

            base.OnPreviewMouseMove(e);
        }

        protected override void OnMouseLeave(MouseEventArgs e)
        {
            if (rowSpanUnderMouse != null)
            {
                this.InvalidateCell(rowSpanUnderMouse);
                rowSpanUnderMouse = null;
#if !SILVERLIGHT
                this.InvalidateVisual(false);
#else
                this.InvalidateVisual();
#endif
            }
            base.OnMouseLeave(e);
        }

        #region CurrentCellActivating
        public static readonly RoutedEvent CurrentCellActivatingEvent = EventManager.RegisterRoutedEvent(
            "CurrentCellActivating",
            RoutingStrategy.Direct,
            typeof(GridCurrentCellActivatingEventHandler),
            typeof(GridControlBase));

        /// <summary>
        /// Occurs before the grid activates the specified cell as current cell.
        /// </summary>
        /// <remarks>
        /// You can disallow the activation of specific cells at run-time when
        /// you assign True to <see cref="CancelEventArgs.Cancel"/>.<para/>
        /// You can modify the <see cref="GridCurrentCellActivatingEventArgs.CellRowColumnIndex"/>
        /// to activate a different cell.
        /// <para/>
        /// You can determine if <see cref="GridCurrentCell.Activate"/>
        /// was called stand-alone or as result of a <see cref="GridCurrentCell.MoveTo"/>
        /// call by checking the <see cref="GridCurrentCell.IsInMoveTo"/> property.
        /// <para/>
        /// Once the current cell has been activated, a <see cref="GridControlBase.CurrentCellActivated"/> event
        /// is raised or a <see cref="GridControlBase.CurrentCellActivateFailed"/> if activating the specified
        /// cell failed.
        /// <para/>
        /// See <see cref="GridCurrentCell.MoveTo"/> for a discussion about the
        /// order of events that you receive when the current cell is moved.
        /// </remarks>
        /// <seealso cref="GridCurrentCellActivatingEventArgs"/>
        /// <seealso cref="GridCurrentCell.Activate"/>
        /// <seealso cref="GridCurrentCell.IsInMoveTo"/>
        [
        Description("Occurs before the grid activates the specified cell as current cell."),
        Category("Behavior")
        ]
        public event GridCurrentCellActivatingEventHandler CurrentCellActivating
        {
            add
            {
                AddHandler(GridControl.CurrentCellActivatingEvent, value, false);
            }
            remove
            {
                RemoveHandler(GridControl.CurrentCellActivatingEvent, value);
            }
        }

        internal bool RaiseCurrentCellActivating(ref RowColumnIndex cellRowColumnIndex, GridActivateCurrentCellOptions activateOptions)
        {
            if (CurrentCell.IsSuspendEvents) return true;
            GridCurrentCellActivatingEventArgs e = new GridCurrentCellActivatingEventArgs(cellRowColumnIndex, activateOptions, GridControlBase.CurrentCellActivatingEvent, this);
            OnCurrentCellActivating(e);
            cellRowColumnIndex = e.CellRowColumnIndex;
            return !e.Cancel;
        }

        /// <summary>
        /// Raises the <see cref="GridControlBase.CurrentCellActivating"/> event.
        /// </summary>
        /// <param name="e">The <see cref="GridCurrentCellActivatingEventArgs"/> instance containing the event data.</param>
        protected virtual void OnCurrentCellActivating(GridCurrentCellActivatingEventArgs e)
        {
            base.RaiseEvent(e);
        }

        #endregion
        #region CurrentCellActivated
        public static readonly RoutedEvent CurrentCellActivatedEvent = EventManager.RegisterRoutedEvent(
            "CurrentCellActivated",
            RoutingStrategy.Direct,
            typeof(GridRoutedEventHandler),
            typeof(GridControlBase));

        internal void RaiseCurrentCellActivated()
        {
            if (CurrentCell.IsSuspendEvents) return;
            OnCurrentCellActivated(new SyncfusionRoutedEventArgs(GridControlBase.CurrentCellActivatedEvent, this));
        }

        /// <summary>
        /// Raises the <see cref="GridControlBase.CurrentCellActivated"/> event.
        /// </summary>
        /// <param name="e">The <see cref="EventArgs"/> instance containing the event data.</param>
        private void OnCurrentCellActivated(SyncfusionRoutedEventArgs e)
        {
            base.RaiseEvent(e);
        }

        /// <summary>
        /// Occurs after the grid activates the specified cell as current cell.
        /// </summary>
        /// <remarks>
        /// You can determine if <see cref="GridCurrentCell.Activate"/>
        /// was called stand-alone or as result of a <see cref="GridCurrentCell.MoveTo"/>
        /// call by checking the <see cref="GridCurrentCell.IsInMoveTo"/> property.
        /// <para/>
        /// See <see cref="GridCurrentCell.MoveTo"/> for a discussion about the
        /// order of events that you receive when the current cell is moved.
        /// <para/>
        /// You can find out about the current cell's position by querying the <see cref="GridCurrentCell.CellRowColumnIndex"/>
        /// property of the <see cref="GridControlBase.CurrentCell"/> object
        /// in <see cref="GridControlBase"/>.
        /// </remarks>
        /// <seealso cref="GridCurrentCellActivatingEventArgs"/>
        /// <seealso cref="GridCurrentCell.Activate"/>
        /// <seealso cref="GridCurrentCell.IsInMoveTo"/>
        [
        Description("Occurs after the grid activates the specified cell as current cell."),
        Category("Behavior")
        ]
        public event GridRoutedEventHandler CurrentCellActivated
        {
            add
            {
                this.AddHandler(GridControlBase.CurrentCellActivatedEvent, value, false);
            }
            remove
            {
                this.RemoveHandler(GridControlBase.CurrentCellActivatedEvent, value);
            }
        }
        #endregion
        #region CurrentCellActivateFailed
        public static readonly RoutedEvent CurrentCellActivateFailedEvent = EventManager.RegisterRoutedEvent(
            "CurrentCellActivateFailed",
            RoutingStrategy.Direct,
            typeof(GridCurrentCellActivateFailedEventHandler),
            typeof(GridControlBase));

        internal void RaiseCurrentCellActivateFailed(RowColumnIndex cellRowColumnIndex)
        {
            if (CurrentCell.IsSuspendEvents) return;

            GridCurrentCellActivateFailedEventArgs e = new GridCurrentCellActivateFailedEventArgs(cellRowColumnIndex,
                GridControlBase.CurrentCellActivateFailedEvent,
                this);
            OnCurrentCellActivateFailed(e);
        }

        /// <summary>
        /// Raises the <see cref="GridControlBase.CurrentCellActivateFailed"/> event.
        /// </summary>
        /// <param name="e">The <see cref="GridCurrentCellActivateFailedEventArgs"/> instance containing the event data.</param>
        protected virtual void OnCurrentCellActivateFailed(GridCurrentCellActivateFailedEventArgs e)
        {
            base.RaiseEvent(e);
        }

        /// <summary>
        /// Occurs after the grid fails to activate a specific cell as current cell.
        /// </summary>
        /// <remarks>
        /// You can determine if <see cref="GridCurrentCell.Activate"/>
        /// was called stand-alone or as result of a <see cref="GridCurrentCell.MoveTo"/>
        /// call by checking the <see cref="GridCurrentCell.IsInMoveTo"/> property.
        /// <para/>
        /// See <see cref="GridCurrentCell.MoveTo"/> for a discussion about the
        /// order of events that you receive when the current cell is moved.
        /// </remarks>
        /// <seealso cref="GridCurrentCellActivateFailedEventArgs"/>
        /// <seealso cref="GridCurrentCell.Activate"/>
        /// <seealso cref="GridCurrentCell.IsInMoveTo"/>
        [
        Description("Occurs after the grid fails to activate a specific cell as current cell."),
        Category("Behavior")
        ]
        public event GridCurrentCellActivateFailedEventHandler CurrentCellActivateFailed
        {
            add
            {
                this.AddHandler(GridControlBase.CurrentCellActivateFailedEvent, value, false);
            }
            remove
            {
                this.RemoveHandler(GridControlBase.CurrentCellActivateFailedEvent, value);
            }
        }
        #endregion
        #region CurrentCellDeactivating
        public static readonly RoutedEvent CurrentCellDeactivatingEvent = EventManager.RegisterRoutedEvent(
            "CurrentCellDeactivating",
            RoutingStrategy.Direct,
            typeof(GridCancelRoutedEventHandler),
            typeof(GridControlBase));
        internal bool RaiseCurrentCellDeactivating()
        {
            if (CurrentCell.IsSuspendEvents) return true;

            SyncfusionCancelRoutedEventArgs e = new SyncfusionCancelRoutedEventArgs(GridControlBase.CurrentCellDeactivatingEvent, this);
            this.OnCurrentCellDeactivating(e);
            return !e.Cancel;
        }

        /// <summary>
        /// Raises the <see cref="GridControlBase.CurrentCellDeactivating"/> event.
        /// </summary>
        /// <param name="e">An <see cref="CancelEventArgs"/> that contains the event data.</param>
        protected virtual void OnCurrentCellDeactivating(SyncfusionCancelRoutedEventArgs e)
        {
            base.RaiseEvent(e);
        }

        /// <summary>
        /// Occurs before the grid the deactivates the current cell.
        /// </summary>
        /// <remarks>
        /// You can cancel the operation
        /// by setting <see cref="CancelEventArgs.Cancel"/> to True.
        /// <para/>
        /// You can determine if <see cref="GridCurrentCell.Deactivate"/>
        /// was called stand-alone or as result of a <see cref="GridCurrentCell.MoveTo"/>
        /// call by checking the <see cref="GridCurrentCell.IsInMoveTo"/> property.
        /// <para/>
        /// You can find out about the current cell's position by querying the <see cref="GridCurrentCell.CellRowColumnIndex"/>
        /// property of the <see cref="GridControlBase.CurrentCell"/> object
        /// in <see cref="GridControlBase"/>.
        /// <para/>
        /// See <see cref="GridCurrentCell.MoveTo"/> for a discussion about the
        /// order of events that you receive when the current cell is moved.
        /// </remarks>
        /// <seealso cref="GridCurrentCell.Deactivate"/>
        /// <seealso cref="GridCurrentCell.IsInMoveTo"/>
        [
        Description("Occurs before the grid the deactivates the current cell."),
        Category("Behavior")
        ]
        public event GridCancelRoutedEventHandler CurrentCellDeactivating
        {
            add
            {
                this.AddHandler(GridControlBase.CurrentCellDeactivatingEvent, value, false);
            }
            remove
            {
                this.RemoveHandler(GridControlBase.CurrentCellDeactivatingEvent, value);
            }
        }
        #endregion
        #region CurrentCellDeactivated
        public static readonly RoutedEvent CurrentCellDeactivatedEvent = EventManager.RegisterRoutedEvent(
            "CurrentCellDeactivated",
            RoutingStrategy.Direct,
            typeof(GridCurrentCellDeactivatedEventHandler),
            typeof(GridControlBase));

        internal void RaiseCurrentCellDeactivated(RowColumnIndex cellRowColumnIndex)
        {
            if (CurrentCell.IsSuspendEvents) return;
            GridCurrentCellDeactivatedEventArgs e = new GridCurrentCellDeactivatedEventArgs(cellRowColumnIndex, GridControlBase.CurrentCellDeactivatedEvent, this);
            this.OnCurrentCellDeactivated(e);
        }

        /// <summary>
        /// Raises the <see cref="GridControlBase.CurrentCellDeactivated"/> event.
        /// </summary>
        /// <param name="e">A <see cref="GridCurrentCellDeactivatedEventArgs"/> that contains the event data.</param>
        protected virtual void OnCurrentCellDeactivated(GridCurrentCellDeactivatedEventArgs e)
        {
            base.RaiseEvent(e);
        }

        /// <summary>
        /// Occurs after the grid deactivates current cell.
        /// </summary>
        /// <remarks>
        /// The grid raises this event when the <see cref="GridControlBase.CurrentCell"/> object's <see cref="GridCurrentCell.Deactivate"/>
        /// method is called. The event occurs after any <see cref="GridControlBase.CurrentCellRejectedChanges"/>,
        ///  <see cref="GridControlBase.CurrentCellAcceptedChanges"/>, <see cref="GridControlBase.CurrentCellRejectedChanges"/>, or
        /// <see cref="GridControlBase.CurrentCellAcceptedChanges"/> are raised.
        /// <para/>
        /// You can determine if <see cref="GridCurrentCell.Deactivate"/>
        /// was called stand-alone or as result of a <see cref="GridCurrentCell.MoveTo"/>
        /// call by checking the <see cref="GridCurrentCell.IsInMoveTo"/> property.
        /// <para/>
        /// See <see cref="GridCurrentCell.MoveTo"/> for a discussion about the
        /// order of events that you receive when the current cell is moved.
        /// </remarks>
        [
        Description("Occurs after the grid deactivates current cell."),
        Category("Behavior")
        ]
        public event GridCurrentCellDeactivatedEventHandler CurrentCellDeactivated
        {
            add
            {
                this.AddHandler(GridControlBase.CurrentCellDeactivatedEvent, value, false);
            }
            remove
            {
                this.RemoveHandler(GridControlBase.CurrentCellDeactivatedEvent, value);
            }
        }
        #endregion
        #region CurrentCellDeactivateFailed
        public static readonly RoutedEvent CurrentCellDeactivateFailedEvent = EventManager.RegisterRoutedEvent(
            "CurrentCellDeactivateFailed",
            RoutingStrategy.Direct,
            typeof(GridRoutedEventHandler),
            typeof(GridControlBase));
        internal void RaiseCurrentCellDeactivateFailed()
        {
            if (CurrentCell.IsSuspendEvents) return;
            this.OnCurrentCellDeactivateFailed(new SyncfusionRoutedEventArgs(GridControlBase.CurrentCellDeactivateFailedEvent, this));
        }

        /// <summary>
        /// Raises the <see cref="GridControlBase.CurrentCellDeactivateFailed"/> event.
        /// </summary>
        /// <param name="e">An <see cref="EventArgs"/> that contains the event data.</param>
        protected virtual void OnCurrentCellDeactivateFailed(SyncfusionRoutedEventArgs e)
        {
            base.RaiseEvent(e);
        }

        /// <summary>
        /// Occurs after the grid fails to deactivate the current cell.
        /// </summary>
        /// <remarks>
        /// The grid raises this event when the <see cref="GridControlBase.CurrentCell"/> object's <see cref="GridCurrentCell.Deactivate"/>
        /// method is called and can not deactivate the current cell. The reason deactivation may fail could be
        /// that the cell's contents were invalid or any of the event handlers associated with deactivating the current cell
        /// signaled to abort this operation.
        /// <para/>
        /// You can determine if <see cref="GridCurrentCell.Deactivate"/>
        /// was called stand-alone or as result of a <see cref="GridCurrentCell.MoveTo"/>
        /// call by checking the <see cref="GridCurrentCell.IsInMoveTo"/> property.
        /// <para/>
        /// See <see cref="GridCurrentCell.MoveTo"/> for a discussion about the
        /// order of events that you receive when the current cell is moved.
        /// </remarks>
        [
        Description("Occurs after the grid fails to deactivate the current cell."),
        Category("Behavior")
        ]
        public event GridRoutedEventHandler CurrentCellDeactivateFailed
        {
            add
            {
                this.AddHandler(GridControlBase.CurrentCellDeactivateFailedEvent, value);
            }
            remove
            {
                this.RemoveHandler(GridControlBase.CurrentCellDeactivateFailedEvent, value);
            }
        }
        #endregion
        #region CurrentCellConfirmChangesFailed
        public static readonly RoutedEvent CurrentCellConfirmChangesFailedEvent = EventManager.RegisterRoutedEvent(
            "CurrentCellConfirmChangesFailed",
            RoutingStrategy.Direct,
            typeof(GridRoutedEventHandler),
            typeof(GridControlBase));

        internal void RaiseCurrentCellConfirmChangesFailed()
        {
            if (CurrentCell.IsSuspendEvents) return;
            this.OnCurrentCellConfirmChangesFailed(new SyncfusionRoutedEventArgs(GridControlBase.CurrentCellConfirmChangesFailedEvent, this));
        }

        /// <summary>
        /// Raises the <see cref="CurrentCellConfirmChangesFailed"/> event.
        /// </summary>
        /// <param name="e">An <see cref="EventArgs" /> that contains the event data.</param>
        protected virtual void OnCurrentCellConfirmChangesFailed(SyncfusionRoutedEventArgs e)
        {
            base.RaiseEvent(e);
        }

        /// <summary>
        /// Occurs when the grid could not save changes made to the active current cell.
        /// </summary>
        /// <remarks>
        /// The grid raises this event when the <see cref="GridControlBase.CurrentCell"/> object's <see cref="GridCurrentCell.ConfirmChanges"/>
        /// method is called and its contents were modified and could not be succesfully validated
        /// or saved back to the data source.
        /// <para/>
        /// The <see cref="GridCurrentCell.Exception"/> and <see cref="GridCurrentCell.ErrorMessage"/>
        /// properties provide details why the operation failed. If you want to display a message box
        /// be sure to reset the the error state with <see cref="GridCurrentCell.ResetError"/>.
        /// <para/>
        /// You can determine if <see cref="GridCurrentCell.Deactivate"/>
        /// was called stand-alone or as result of a <see cref="GridCurrentCell.MoveTo"/>
        /// call by checking the <see cref="GridCurrentCell.IsInMoveTo"/> property.
        /// <para/>
        /// You can find out about the current cell's position by querying the <see cref="GridCurrentCell.CellRowColumnIndex"/>
        /// property of the <see cref="GridControlBase.CurrentCell"/> object
        /// in <see cref="GridControlBase"/>
        /// <para/>
        /// See <see cref="GridCurrentCell.MoveTo"/> for a discussion about the
        /// order of events that you receive when the current cell is moved.
        /// <para/>
        /// </remarks>
        [
        Description("Occurs when the grid accepted changes made to the active current cell."),
        Category("Behavior")
        ]
        public event GridRoutedEventHandler CurrentCellConfirmChangesFailed
        {
            add
            {
                this.AddHandler(GridControlBase.CurrentCellConfirmChangesFailedEvent, value);
            }
            remove
            {
                this.RemoveHandler(GridControlBase.CurrentCellConfirmChangesFailedEvent, value);
            }
        }
        #endregion
        #region CurrentCellAcceptedChanges
        public static readonly RoutedEvent CurrentCellAcceptedChangesEvent = EventManager.RegisterRoutedEvent(
            "CurrentCellAcceptedChanges",
            RoutingStrategy.Direct,
            typeof(GridRoutedEventHandler),
            typeof(GridControlBase));

        internal bool RaiseCurrentCellAcceptedChanges()
        {
            if (CurrentCell.IsSuspendEvents) return true;
            SyncfusionRoutedEventArgs e = new SyncfusionRoutedEventArgs(GridControlBase.CurrentCellAcceptedChangesEvent, this);
            this.OnCurrentCellAcceptedChanges(e);
            return !e.Handled;
        }

        /// <summary>
        /// Raises the cancelable <see cref="GridControlBase.CurrentCellAcceptedChanges"/> event.
        /// </summary>
        /// <param name="e">An <see cref="EventArgs"/> that contains the event data.</param>
        protected virtual void OnCurrentCellAcceptedChanges(SyncfusionRoutedEventArgs e)
        {
            base.RaiseEvent(e);
        }

        /// <summary>
        /// Occurs when the grid accepts changes made to the active current cell.
        /// </summary>
        /// <remarks>
        /// The grid raises this cancelable event when the <see cref="GridControlBase.CurrentCell"/> object's <see cref="GridCurrentCell.ConfirmChanges"/>
        /// method is called. <see cref="GridCurrentCell.Deactivate"/> and <see cref="GridCurrentCell.EndEdit"/> call this method when the current cell was in editing mode
        /// and its contents were modified and validated.
        /// <para/>
        /// You can determine if <see cref="GridCurrentCell.Deactivate"/>
        /// was called stand-alone or as result of a <see cref="GridCurrentCell.MoveTo"/>
        /// call by checking the <see cref="GridCurrentCell.IsInMoveTo"/> property.
        /// <para/>
        /// If you assign true to <see cref="CancelEventArgs.Cancel"/>, the grid will not deactivate the current
        /// cell.
        /// <para/>
        /// You can find out about the current cell's position by querying the <see cref="GridCurrentCell.CellRowColumnIndex"/>
        /// property of the <see cref="GridControlBase.CurrentCell"/> object
        /// in <see cref="GridControlBase"/>.
        /// <para/>
        /// See <see cref="GridCurrentCell.MoveTo"/> for a discussion about the
        /// order of events that you receive when the current cell is moved.
        /// <para/>
        /// </remarks>
        [
        Description("Occurs when the grid accepted changes made to the active current cell."),
        Category("Behavior")
        ]
        public event GridRoutedEventHandler CurrentCellAcceptedChanges
        {
            add
            {
                this.AddHandler(GridControlBase.CurrentCellAcceptedChangesEvent, value);
            }
            remove
            {
                this.RemoveHandler(GridControlBase.CurrentCellAcceptedChangesEvent, value);
            }
        }
        #endregion
        #region CurrentCellChanging
        public static readonly RoutedEvent CurrentCellChangingEvent = EventManager.RegisterRoutedEvent(
            "CurrentCellChanging",
            RoutingStrategy.Direct,
            typeof(GridCancelRoutedEventHandler),
            typeof(GridControlBase));

        internal bool RaiseCurrentCellChanging()
        {
            if (CurrentCell.IsSuspendEvents) return true;

            SyncfusionCancelRoutedEventArgs e = new SyncfusionCancelRoutedEventArgs(GridControlBase.CurrentCellChangingEvent, this);
            this.OnCurrentCellChanging(e);
            return !e.Cancel;
        }

        /// <summary>
        /// Raises the <see cref="GridControlBase.CurrentCellChanging"/> event.
        /// </summary>
        /// <param name="e">A <see cref="CancelEventArgs" /> that contains the event data.</param>
        protected virtual void OnCurrentCellChanging(SyncfusionCancelRoutedEventArgs e)
        {
            base.RaiseEvent(e);
        }


        /// <summary>
        /// Occurs when the user wants to modify contents of the current cell.
        /// </summary>
        /// <remarks>
        /// The grid sends this event before the changes are applied to the active cell. You can cancel the operation
        /// by setting <see cref="CancelEventArgs.Cancel"/> to True.
        /// <para/>
        /// You can find out about the current cell's position by querying the <see cref="GridCurrentCell.CellRowColumnIndex"/>
        /// property of the <see cref="GridControlBase.CurrentCell"/> object
        /// in <see cref="GridControlBase"/>.
        /// <para/>
        /// See <see cref="GridCurrentCell.MoveTo"/> for a discussion about the
        /// order of events that you receive when the current cell is moved.
        /// </remarks>
        [
        Description("Occurs when the user wants to modify contents of the current cell."),
        Category("Behavior")
        ]
        public event GridCancelRoutedEventHandler CurrentCellChanging
        {
            add
            {
                this.AddHandler(GridControlBase.CurrentCellChangingEvent, value);
            }
            remove
            {
                this.RemoveHandler(GridControlBase.CurrentCellChangingEvent, value);
            }
        }

        #endregion
        #region CurrentCellStartEditing
        public static readonly RoutedEvent CurrentCellStartEditingEvent = EventManager.RegisterRoutedEvent(
            "CurrentCellStartEditing",
            RoutingStrategy.Direct,
            typeof(GridCancelRoutedEventHandler),
            typeof(GridControlBase));

        internal bool RaiseCurrentCellStartEditing()
        {
            if (CurrentCell.IsSuspendEvents) return true;

            SyncfusionCancelRoutedEventArgs e = new SyncfusionCancelRoutedEventArgs(GridControlBase.CurrentCellStartEditingEvent, this);
            this.OnCurrentCellStartEditing(e);
            return !e.Cancel;
        }

        /// <summary>
        /// Raises the <see cref="GridControlBase.CurrentCellStartEditing"/> event.
        /// </summary>
        /// <param name="e">A <see cref="CancelEventArgs"/> that contains the event data.</param>
        protected virtual void OnCurrentCellStartEditing(SyncfusionCancelRoutedEventArgs e)
        {
            base.RaiseEvent(e);
        }

        /// <summary>
        /// Occurs before the current cell switches into editing mode.
        /// </summary>
        /// <remarks>
        /// The grid will switch into editing mode when the user presses a key while the cell
        /// is not in editing mode or when you call <see cref="GridCurrentCell.BeginEdit"/>.
        /// You can cancel the operation
        /// by setting <see cref="CancelEventArgs.Cancel"/> to True.
        /// <para/>
        /// You can find out about the current cell's position by querying the <see cref="GridCurrentCell.CellRowColumnIndex"/>
        /// property of the <see cref="GridControlBase.CurrentCell"/> object
        /// in <see cref="GridControlBase"/>.
        /// <para/>
        /// See <see cref="GridCurrentCell.MoveTo"/> for a discussion about the
        /// order of events that you receive when the current cell is moved.
        /// </remarks>
        [
        Description("Occurs before the current cell switches into editing mode."),
        Category("Behavior")
        ]
        public event GridCancelRoutedEventHandler CurrentCellStartEditing
        {
            add
            {
                this.AddHandler(GridControlBase.CurrentCellStartEditingEvent, value);
            }
            remove
            {
                this.RemoveHandler(GridControlBase.CurrentCellStartEditingEvent, value);
            }
        }
        #endregion

        //#region CurrentCellLoaded
        //public static readonly RoutedEvent CurrentCellLoadedEvent = EventManager.RegisterRoutedEvent(
        //    "CurrentCellLoaded",
        //    RoutingStrategy.Direct,
        //    typeof(GridCancelRoutedEventHandler),
        //    typeof(GridControlBase));

        //internal bool RaiseCurrentCellLoaded()
        //{
        //    if (CurrentCell.IsSuspendEvents) return true;

        //    SyncfusionCancelRoutedEventArgs e = new SyncfusionCancelRoutedEventArgs(GridControlBase.CurrentCellLoadedEvent, this);
        //    this.OnCurrentCellLoaded(e);
        //    return !e.Cancel;
        //}

        ///// <summary>
        ///// Raises the <see cref="GridControlBase.CurrentCellLoaded"/> event.
        ///// </summary>
        ///// <param name="e">A <see cref="CancelEventArgs"/> that contains the event data.</param>
        //protected virtual void OnCurrentCellLoaded(SyncfusionCancelRoutedEventArgs e)
        //{
        //    base.RaiseEvent(e);
        //}

        ///// <summary>
        ///// Occurs after the current cell is loaded.
        ///// </summary>
        ///// <remarks>      
        //[
        //Description("Occurs after the current cell is loaded."),
        //Category("Behavior")
        //]
        //public event GridCancelRoutedEventHandler CurrentCellLoaded
        //{
        //    add
        //    {
        //        this.AddHandler(GridControlBase.CurrentCellLoadedEvent, value);
        //    }
        //    remove
        //    {
        //        this.RemoveHandler(GridControlBase.CurrentCellLoadedEvent, value);
        //    }
        //}
        //#endregion




        #region CurrentCellEditingComplete
        public static readonly RoutedEvent CurrentCellEditingCompleteEvent = EventManager.RegisterRoutedEvent(
            "CurrentCellEditingComplete",
            RoutingStrategy.Direct,
            typeof(GridRoutedEventHandler),
            typeof(GridControlBase));

        internal void RaiseCurrentCellEditingComplete()
        {
            if (CurrentCell.IsSuspendEvents) return;
            string currentcelltype = string.Empty;
            if (CurrentCell.Renderer != null && CurrentCell.Renderer.CurrentStyle != null)
                currentcelltype = CurrentCell.Renderer.CurrentStyle.CellType;

            if (currentcelltype == "TextBox" || currentcelltype == "TextBlock" || currentcelltype == "FormulaCell"
                || currentcelltype == "Static" || currentcelltype == "CurrencyEdit" || currentcelltype == "MaskEdit"
                || currentcelltype == "PercentEdit" || currentcelltype == "DoubleEdit")
            {
                if (this.Model is GridDataTableModel && (this.Model as GridDataTableModel).TableProperties.ShowRowHeader)
                    InvalidateCell(GridRangeInfo.Cells(currentCell.RowIndex, 1, currentCell.RowIndex, model.ColumnCount));
                else
                    InvalidateCell(GridRangeInfo.Row(currentCell.RowIndex));
            }
            this.OnCurrentCellEditingComplete(new SyncfusionRoutedEventArgs(GridControlBase.CurrentCellEditingCompleteEvent, this));
        }

        /// <summary>
        /// Raises the <see cref="GridControlBase.CurrentCellEditingComplete"/> event.
        /// </summary>
        /// <param name="e">An <see cref="EventArgs"/> that contains the event data.</param>
        protected virtual void OnCurrentCellEditingComplete(SyncfusionRoutedEventArgs e)
        {
            base.RaiseEvent(e);
        }

        /// <summary>
        /// Occurs when the grid completes editing mode for the active current cell.
        /// </summary>
        /// <remarks>
        /// The grid raises this event when the <see cref="GridControlBase.CurrentCell"/> object's <see cref="GridCurrentCell.EndEdit"/>
        /// or <see cref="GridCurrentCell.CancelEdit"/> method is called. The event occurs after <see cref="GridControlBase.CurrentCellRejectedChanges"/>
        /// or <see cref="GridControlBase.CurrentCellAcceptedChanges"/> were raised.
        /// <para/>
        /// You can determine if <see cref="GridCurrentCell.Deactivate"/>
        /// was called stand-alone or as result of a <see cref="GridCurrentCell.MoveTo"/>
        /// call by checking the <see cref="GridCurrentCell.IsInMoveTo"/> property.
        /// <para/>
        /// You can find out about the current cell's position by querying the <see cref="GridCurrentCell.CellRowColumnIndex"/>
        /// property of the <see cref="GridControlBase.CurrentCell"/> object
        /// in <see cref="GridControlBase"/>.
        /// <para/>
        /// See <see cref="GridCurrentCell.MoveTo"/> for a discussion about the
        /// order of events that you receive when the current cell is moved.
        /// </remarks>
        [
        Description("Occurs when the grid completes editing mode for the active current cell."),
        Category("Behavior")
        ]
        public event GridRoutedEventHandler CurrentCellEditingComplete
        {
            add
            {
                this.AddHandler(GridControlBase.CurrentCellEditingCompleteEvent, value);
            }
            remove
            {
                this.RemoveHandler(GridControlBase.CurrentCellEditingCompleteEvent, value);
            }
        }
        #endregion
        #region CurrentCellRejectedChanges
        public static readonly RoutedEvent CurrentCellRejectedChangesEvent = EventManager.RegisterRoutedEvent(
            "CurrentCellRejectedChanges",
            RoutingStrategy.Direct,
            typeof(GridRoutedEventHandler),
            typeof(GridControlBase));

        internal void RaiseCurrentCellRejectedChanges()
        {
            if (CurrentCell.IsSuspendEvents) return;
            this.OnCurrentCellRejectedChanges(new SyncfusionRoutedEventArgs(GridControlBase.CurrentCellRejectedChangesEvent, this));
        }

        /// <summary>
        /// Raises the <see cref="GridControlBase.CurrentCellRejectedChanges"/> event.
        /// </summary>
        /// <param name="e">An <see cref="EventArgs"/> that contains the event data.</param>
        protected virtual void OnCurrentCellRejectedChanges(SyncfusionRoutedEventArgs e)
        {
            base.RaiseEvent(e);
        }

        /// <summary>
        /// Occurs when the grid rejects changes made to the active current cell.
        /// </summary>
        /// <remarks>
        /// The grid raises this event when the <see cref="GridControlBase.CurrentCell"/> object's <see cref="GridCurrentCell.RejectChanges"/>
        /// method is called. <see cref="GridCurrentCell.Deactivate"/> and <see cref="GridCurrentCell.CancelEdit"/> call this method when the current cell was in editing mode
        /// and its contents were modified.
        /// <para/>
        /// You can determine if <see cref="GridCurrentCell.Deactivate"/>
        /// was called stand-alone or as result of a <see cref="GridCurrentCell.MoveTo"/>
        /// call by checking the <see cref="GridCurrentCell.IsInMoveTo"/> property.
        /// <para/>
        /// You can find out about the current cell's position by querying the <see cref="GridCurrentCell.CellRowColumnIndex"/>
        /// property of the <see cref="GridControlBase.CurrentCell"/> object
        /// in <see cref="GridControlBase"/>.
        /// <para/>
        /// See <see cref="GridCurrentCell.MoveTo"/> for a discussion about the
        /// order of events that you receive when the current cell is moved.
        /// <para/>
        /// </remarks>
        [
        Description("Occurs when the grid rejects changes made to the active current cell."),
        Category("Behavior")
        ]
        public event GridRoutedEventHandler CurrentCellRejectedChanges
        {
            add
            {
                this.AddHandler(GridControlBase.CurrentCellRejectedChangesEvent, value);
            }
            remove
            {
                this.RemoveHandler(GridControlBase.CurrentCellRejectedChangesEvent, value);
            }
        }
        #endregion
        #region CurrentCellChanged
        public static readonly RoutedEvent CurrentCellChangedEvent = EventManager.RegisterRoutedEvent(
            "CurrentCellChanged",
            RoutingStrategy.Direct,
            typeof(GridRoutedEventHandler),
            typeof(GridControlBase));

        internal void RaiseCurrentCellChanged()
        {
            if (CurrentCell.IsSuspendEvents) return;
            this.OnCurrentCellChanged(new SyncfusionRoutedEventArgs(GridControlBase.CurrentCellChangedEvent, this));
        }

        /// <summary>
        /// Raises the <see cref="GridControlBase.CurrentCellChanged"/> event.
        /// </summary>
        /// <param name="e">A <see cref="EventArgs"/> that contains the event data.</param>
        protected virtual void OnCurrentCellChanged(SyncfusionRoutedEventArgs e)
        {
            base.RaiseEvent(e);
        }

        /// <summary>
        /// Occurs when the user changes contents of the current cell.
        /// </summary>
        /// <remarks>
        /// The grid sends this event whenever changes occur, similar to a <see cref="TextBoxBase.ModifiedChanged"/> event.
        /// <para/>
        /// You can find out about the current cell's position by querying the <see cref="GridCurrentCell.CellRowColumnIndex"/>
        /// property of the <see cref="GridControlBase.CurrentCell"/> object
        /// in <see cref="GridControlBase"/>
        /// <para/>
        /// See <see cref="GridCurrentCell.MoveTo"/> for a discussion about the
        /// order of events that you receive when the current cell is moved.
        /// </remarks>
        [
        Description("Occurs when the user changes contents of the current cell."),
        Category("Behavior")
        ]
        public event GridRoutedEventHandler CurrentCellChanged
        {
            add
            {
                this.AddHandler(GridControlBase.CurrentCellChangedEvent, value);
            }
            remove
            {
                this.RemoveHandler(GridControlBase.CurrentCellChangedEvent, value);
            }
        }
        #endregion
        #region CurrentCellMoved
        public static readonly RoutedEvent CurrentCellMovedEvent = EventManager.RegisterRoutedEvent(
            "CurrentCellMoved",
            RoutingStrategy.Direct,
            typeof(GridCurrentCellMovedEventHandler),
            typeof(GridControlBase));

        internal void RaiseCurrentCellMoved(GridActivateCurrentCellOptions activateOptions)
        {
            if (CurrentCell.IsSuspendEvents) return;

            GridCurrentCellMovedEventArgs e = new GridCurrentCellMovedEventArgs(activateOptions, GridControlBase.CurrentCellMovedEvent, this);
            this.OnCurrentCellMoved(e);
        }

        /// <summary>
        /// Raises the <see cref="GridControlBase.CurrentCellMoved"/> event.
        /// </summary>
        /// <param name="e">A <see cref="GridCurrentCellMovedEventArgs"/> that contains the event data.</param>
        protected virtual void OnCurrentCellMoved(GridCurrentCellMovedEventArgs e)
        {
            base.RaiseEvent(e);
        }

        /// <summary>
        /// Occurs when the current cell has been successfully moved to a new position.
        /// </summary>
        /// <remarks>
        /// See <see cref="GridCurrentCell.MoveTo"/> for a discussion about the
        /// order of events that you receive when the current cell is moved.
        /// </remarks>
        /// <seealso cref="GridCurrentCellMovedEventArgs"/>
        /// <seealso cref="GridCurrentCell.MoveTo"/>
        [
        Description("Occurs when the current cell has been successfully moved to a new position."),
        Category("Behavior")
        ]
        public event GridCurrentCellMovedEventHandler CurrentCellMoved
        {
            add
            {
                this.AddHandler(GridControlBase.CurrentCellMovedEvent, value);
            }
            remove
            {
                this.RemoveHandler(GridControlBase.CurrentCellMovedEvent, value);
            }
        }
        #endregion
        #region CurrentCellMoveFailed
        public static readonly RoutedEvent CurrentCellMoveFailedEvent = EventManager.RegisterRoutedEvent(
            "CurrentCellMoveFailed",
            RoutingStrategy.Direct,
            typeof(GridCurrentCellMoveFailedEventHandler),
            typeof(GridControlBase));

        internal void RaiseCurrentCellMoveFailed(RowColumnIndex rowColumnIndex, GridActivateCurrentCellOptions activateOptions)
        {
            if (CurrentCell.IsSuspendEvents) return;

            GridCurrentCellMoveFailedEventArgs e = new GridCurrentCellMoveFailedEventArgs(rowColumnIndex, activateOptions, GridControlBase.CurrentCellMoveFailedEvent, this);
            this.OnCurrentCellMoveFailed(e);
        }

        /// <summary>
        /// Raises the <see cref="GridControlBase.CurrentCellMoveFailed"/> event.
        /// </summary>
        /// <param name="e">A <see cref="GridCurrentCellMoveFailedEventArgs"/> that contains the event data.</param>
        protected virtual void OnCurrentCellMoveFailed(GridCurrentCellMoveFailedEventArgs e)
        {
            base.RaiseEvent(e);
        }

        /// <summary>
        /// Occurs when the current cell fails to be moved to a new position.
        /// </summary>
        /// <remarks>
        /// See <see cref="GridCurrentCell.MoveTo"/> for a discussion about the
        /// order of events that you receive when the current cell is moved.
        /// <para/>
        /// <see cref="GridCurrentCell.ErrorMessage"/> might hold an error message
        /// why the operation failed.
        /// </remarks>
        /// <seealso cref="GridCurrentCellMoveFailedEventArgs"/>
        /// <seealso cref="GridCurrentCell.MoveTo"/>
        [
        Description("Occurs when the current cell fails to be moved to a new position."),
        Category("Behavior")
        ]
        public event GridCurrentCellMoveFailedEventHandler CurrentCellMoveFailed
        {
            add
            {
                this.AddHandler(GridControlBase.CurrentCellMoveFailedEvent, value);
            }
            remove
            {
                this.RemoveHandler(GridControlBase.CurrentCellMoveFailedEvent, value);
            }
        }
        #endregion
        #region CurrentCellMoving
        public static readonly RoutedEvent CurrentCellMovingEvent = EventManager.RegisterRoutedEvent(
            "CurrentCellMoving",
            RoutingStrategy.Direct,
            typeof(GridCurrentCellMovingEventHandler),
            typeof(GridControlBase));

        internal bool RaiseCurrentCellMoving(ref RowColumnIndex cellRowColumnIndex, GridActivateCurrentCellOptions activateOptions)
        {
            if (CurrentCell.IsSuspendEvents) return true;

            GridCurrentCellMovingEventArgs e = new GridCurrentCellMovingEventArgs(cellRowColumnIndex, activateOptions, GridControlBase.CurrentCellMovingEvent, this);
            this.OnCurrentCellMoving(e);
            cellRowColumnIndex = e.CellRowColumnIndex;
            return !e.Cancel;
        }

        /// <summary>
        /// Raises the <see cref="GridControlBase.CurrentCellMoving"/> event.
        /// </summary>
        /// <param name="e">A <see cref="GridCurrentCellMovingEventArgs"/> that contains the event data.</param>
        protected virtual void OnCurrentCellMoving(GridCurrentCellMovingEventArgs e)
        {
            base.RaiseEvent(e);
        }

        /// <summary>
        /// Occurs when the current cell is about to be moved to a new position.
        /// </summary>
        /// <remarks>
        /// You can disallow the activataion of specific cells at run-time when
        /// you assign True to <see cref="CancelEventArgs.Cancel"/>.
        /// <para/>
        /// You can modify the <see cref="GridCurrentCellActivatingEventArgs.CellRowColumnIndex"/>
        /// to activate a different cell.
        /// <para/>
        /// You can also modify the <see cref="GridCurrentCellActivatingEventArgs.Options"/>.
        /// <para/>
        /// Once the current cell has been moved, a <see cref="GridControlBase.CurrentCellMoved"/> event
        /// is raised or a <see cref="GridControlBase.CurrentCellMoveFailed"/> if moving to the specified
        /// target cell failed.
        /// <para/>
        /// See <see cref="GridCurrentCell.MoveTo"/> for a discussion about the
        /// order of events that you receive when the current cell is moved.
        /// </remarks>
        /// <seealso cref="GridCurrentCellMovingEventArgs"/>
        /// <seealso cref="GridCurrentCell.MoveTo"/>
        [
        Description("Occurs when the current cell is about to be moved to a new position."),
        Category("Behavior")
        ]
        public event GridCurrentCellMovingEventHandler CurrentCellMoving
        {
            add
            {
                this.AddHandler(GridControlBase.CurrentCellMovingEvent, value);
            }
            remove
            {
                this.RemoveHandler(GridControlBase.CurrentCellMovingEvent, value);
            }
        }
        #endregion
        #region CurrentCellValidating
        public static readonly RoutedEvent CurrentCellValidatingEvent = EventManager.RegisterRoutedEvent(
            "CurrentCellValidating",
            RoutingStrategy.Direct,
            typeof(CurrentCellValidatingEventHandler),
            typeof(GridControlBase));

        internal bool RaiseCurrentCellValidating(GridStyleInfo style,object oldValue,object newValue,out object modifiedValue)
        {
            modifiedValue = null;
            if (CurrentCell.IsSuspendEvents) return true;

            CurrentCellValidatingEventArgs e = new CurrentCellValidatingEventArgs(style, oldValue, newValue, GridControlBase.CurrentCellValidatingEvent, this);
            this.OnCurrentCellValidating(e);
            modifiedValue = e.NewValue;
            return !e.Cancel;
        }

        /// <summary>
        /// Raises the cancelable <see cref="GridControlBase.CurrentCellValidating"/> event.
        /// </summary>
        /// <param name="e">An <see cref="CancelEventArgs"/> that contains the event data.</param>
        protected virtual void OnCurrentCellValidating(CurrentCellValidatingEventArgs e)
        {
            base.RaiseEvent(e);
        }

        /// <summary>
        /// Occurs when the grid validates contents of the active current cell.
        /// </summary>
        /// <remarks>
        /// You can mark the contents as invalid by by setting <see cref="CancelEventArgs.Cancel"/> to True.<para/>
        /// The grid raises this event when the <see cref="GridControlBase.CurrentCell"/> object's <see cref="GridCurrentCell.Validate"/>
        /// method is called. <see cref="GridCurrentCell.Deactivate"/> calls this method when the current cell was in editing mode
        /// and its contents were modified.
        /// <para/>
        /// You can determine if <see cref="GridCurrentCell.Deactivate"/>
        /// was called stand-alone or as result of a <see cref="GridCurrentCell.MoveTo"/>
        /// call by checking the <see cref="GridCurrentCell.IsInMoveTo"/> property.
        /// <para/>
        /// You can find out about the current cell's position by querying the <see cref="GridCurrentCell.CellRowColumnIndex"/>
        /// property of the <see cref="GridControlBase.CurrentCell"/> object
        /// in <see cref="GridControlBase"/>.
        /// <para/>
        /// See <see cref="GridCurrentCell.MoveTo"/> for a discussion about the
        /// order of events that you receive when the current cell is moved.
        /// </remarks>
        [
        Description("Occurs when the grid validates contents of the active current cell."),
        Category("Behavior")
        ]
        public event CurrentCellValidatingEventHandler CurrentCellValidating
        {
            add
            {
                this.AddHandler(GridControlBase.CurrentCellValidatingEvent, value);
            }
            remove
            {
                this.RemoveHandler(GridControlBase.CurrentCellValidatingEvent, value);
            }
        }
        #endregion
        #region CurrentCellValidated
        public static readonly RoutedEvent CurrentCellValidatedEvent = EventManager.RegisterRoutedEvent(
            "CurrentCellValidated",
            RoutingStrategy.Direct,
            typeof(GridRoutedEventHandler),
            typeof(GridControlBase));

        internal void RaiseCurrentCellValidated()
        {
            if (CurrentCell.IsSuspendEvents) return;
            this.OnCurrentCellValidated(new SyncfusionRoutedEventArgs(GridControlBase.CurrentCellValidatedEvent, this));
        }

        /// <summary>
        /// Raises the <see cref="GridControlBase.CurrentCellValidated"/> event.
        /// </summary>
        /// <param name="e">An <see cref="EventArgs"/> that contains the event data.</param>
        protected virtual void OnCurrentCellValidated(SyncfusionRoutedEventArgs e)
        {
            base.RaiseEvent(e);
        }

        /// <summary>
        /// Occurs when the grid has successfully validated the contents of the active current cell.
        /// </summary>
        /// <remarks>
        /// You can determine if <see cref="GridCurrentCell.Deactivate"/>
        /// was called stand-alone or as result of a <see cref="GridCurrentCell.MoveTo"/>
        /// call by checking the <see cref="GridCurrentCell.IsInMoveTo"/> property.
        /// <para/>
        /// You can find out about the current cell's position by querying the <see cref="GridCurrentCell.CellRowColumnIndex"/>
        /// property of the <see cref="GridControlBase.CurrentCell"/> object
        /// in <see cref="GridControlBase"/>.
        /// <para/>
        /// See <see cref="GridCurrentCell.MoveTo"/> for a discussion about the
        /// order of events that you receive when the current cell is moved.
        /// </remarks>
        [
        Description("Occurs when the grid validates contents of the active current cell."),
        Category("Behavior")
        ]
        public event GridRoutedEventHandler CurrentCellValidated
        {
            add
            {
                this.AddHandler(GridControlBase.CurrentCellValidatedEvent, value);
            }
            remove
            {
                this.RemoveHandler(GridControlBase.CurrentCellValidatedEvent, value);
            }
        }
        #endregion
        #region CurrentCellClosedDropDown

        public static readonly RoutedEvent CurrentCellClosedDropDownEvent = EventManager.RegisterRoutedEvent("CurrentCellClosedDropDown", RoutingStrategy.Direct, typeof(GridRoutedEventHandler), typeof(GridControlBase));

        internal void RaiseCurrentCellClosedDropDown()
        {
            if (this.CurrentCell.IsSuspendEvents) return;

            var e = new SyncfusionRoutedEventArgs(GridControlBase.CurrentCellClosedDropDownEvent, this);
            this.OnCurrentCellClosedDropDown(e);
        }

        protected virtual void OnCurrentCellClosedDropDown(SyncfusionRoutedEventArgs e)
        {
            this.RaiseEvent(e);
        }

        public event GridRoutedEventHandler CurrentCellClosedDropDown
        {
            add
            {
                this.AddHandler(GridControlBase.CurrentCellClosedDropDownEvent, value);
            }

            remove
            {
                this.RemoveHandler(GridControlBase.CurrentCellClosedDropDownEvent, value);
            }
        }

        #endregion
        #region CurrentCellShowedDropDown

        public static readonly RoutedEvent CurrentCellShowedDropDownEvent = EventManager.RegisterRoutedEvent("CurrentCellShowedDropDown", RoutingStrategy.Direct, typeof(GridRoutedEventHandler), typeof(GridControlBase));

        internal void RaiseCurrentCellShowedDropDown()
        {
            if (this.CurrentCell.IsSuspendEvents) return;

            var e = new SyncfusionRoutedEventArgs(GridControlBase.CurrentCellShowedDropDownEvent, this);
            this.OnCurrentCellShowedDropDown(e);
        }

        /// <summary>
        /// Occurs after the drop-down part has been dropped-down and made visible.
        /// </summary>
        protected virtual void OnCurrentCellShowedDropDown(SyncfusionRoutedEventArgs e)
        {
            this.RaiseEvent(e);
        }

        /// <summary>
        /// Occurs after the drop-down part has been dropped-down and made visible.
        /// </summary>
        [Description("Occurs after the drop-down part has been dropped-down and made visible."),
        Category("Behavior")]
        public event GridRoutedEventHandler CurrentCellShowedDropDown
        {
            add
            {
                this.AddHandler(GridControlBase.CurrentCellShowedDropDownEvent, value);
            }

            remove
            {
                this.RemoveHandler(GridControlBase.CurrentCellShowedDropDownEvent, value);
            }
        }

        #endregion
        #region CurrentCellShowingDropDown

        public static readonly RoutedEvent CurrentCellShowingDropDownEvent = EventManager.RegisterRoutedEvent("CurrentCellShowingDropDown", RoutingStrategy.Direct, typeof(GridCurrentCellShowingDropDownEventHandler), typeof(GridControlBase));

        internal bool RaiseCurrentCellShowingDropDown(bool isDropDownOpen)
        {
            if (this.CurrentCell.IsSuspendEvents) return true;

            var e = new GridCurrentCellShowingDropDownEventArgs(isDropDownOpen, GridControlBase.CurrentCellShowingDropDownEvent, this);
            this.OnCurrentCellShowingDropDown(e);
            return !e.Cancel;
        }

        protected virtual void OnCurrentCellShowingDropDown(GridCurrentCellShowingDropDownEventArgs e)
        {
            this.RaiseEvent(e);
        }

        /// <summary>
        /// Occurs when the drop-down part is about to be shown.
        /// </summary>
        /// <para/>
        /// To abort the drop-down operation, you should set <see cref="CancelEventArgs.Cancel"/> to True.
        /// <para/>
        /// If you need to get access to the cell renderer, you can use the <see cref="GridCurrentCell.Renderer"/>
        /// property of the <see cref="GridControlBase.CurrentCell"/> object. The <see cref="GridControlBase.CurrentCell"/> object
        /// also holds style information and row and column index. See the cell renderer for properties to access
        /// the drop-down container and drop-down part.
        /// </remarks>
        [Description("Occurs when the drop-down part is about to be shown."),
        Category("Behavior")]
        public event GridCurrentCellShowingDropDownEventHandler CurrentCellShowingDropDown
        {
            add
            {
                this.AddHandler(GridControlBase.CurrentCellShowingDropDownEvent, value);
            }

            remove
            {
                this.RemoveHandler(GridControlBase.CurrentCellShowingDropDownEvent, value);
            }
        }

        #endregion
        #endregion

        #region CellEvents

        #region CellButtonClick

        public static readonly RoutedEvent CellButtonClickEvent = EventManager.RegisterRoutedEvent(
            "CellButtonClick",
            RoutingStrategy.Direct,
            typeof(GridCellButtonClickEventHandler),
            typeof(GridControlBase));

        public event GridCellButtonClickEventHandler CellButtonClick
        {
            add
            {
                this.AddHandler(GridControlBase.CellButtonClickEvent, value);
            }
            remove
            {
                this.RemoveHandler(GridControlBase.CellButtonClickEvent, value);
            }
        }

        internal void RaiseGridCellButtonClick(int rowIndex, int colIndex)
        {
            GridCellButtonClickEventArgs e = new GridCellButtonClickEventArgs(rowIndex, colIndex, GridControlBase.CellButtonClickEvent, this);
            this.OnCellButtonClick(e);
        }

        protected virtual void OnCellButtonClick(GridCellButtonClickEventArgs e)
        {
            base.RaiseEvent(e);
        }

        #endregion
        #region DropDownSelectionChanged
        public static readonly RoutedEvent DropDownSelectionChangedEvent = EventManager.RegisterRoutedEvent(
            "DropDownSelectionChanged",
            RoutingStrategy.Direct,
            typeof(GridCellComboValueChangedEventHandler),
            typeof(GridControlBase));

        public event GridCellComboValueChangedEventHandler DropDownSelectionChanged
        {
            add
            {
                this.AddHandler(GridControlBase.DropDownSelectionChangedEvent, value);
            }

            remove
            {
                this.RemoveHandler(GridControlBase.DropDownSelectionChangedEvent, value);
            }
        }

        internal void RaiseGridDropDownSelectionChanged(RowColumnIndex cellRowColumnIndex, object selectedItem)
        {
            GridCellComboValueChangedEventArgs args = new GridCellComboValueChangedEventArgs(GridControlBase.DropDownSelectionChangedEvent, this, cellRowColumnIndex, selectedItem);
            this.OnDropDownSelectionChanged(args);
        }

        protected virtual void OnDropDownSelectionChanged(GridCellComboValueChangedEventArgs args)
        {
            this.RaiseEvent(args);
        }

        #endregion

        #region CellClick

        public static readonly RoutedEvent CellClickEvent = EventManager.RegisterRoutedEvent(
            "CellClick",
            RoutingStrategy.Direct,
            typeof(GridCellClickEventHandler),
            typeof(GridControlBase));

        /// <summary>
        /// Occurs when the user clicks inside a cell.
        /// </summary>
        public event GridCellClickEventHandler CellClick
        {
            add
            {
                this.AddHandler(GridControlBase.CellClickEvent, value);
            }
            remove
            {
                this.RemoveHandler(GridControlBase.CellClickEvent, value);
            }
        }

        internal bool RaiseGridCellClick(int rowIndex, int colIndex)
        {
            return RaiseGridCellClick(rowIndex, colIndex, 1);
        }

        internal bool RaiseGridCellClick(int rowIndex, int colIndex, int clicks)
        {
            GridCellClickEventArgs e = new GridCellClickEventArgs(rowIndex, colIndex, clicks, GridControlBase.CellClickEvent, this);
            this.OnCellClick(e);
            return !e.Handled;
        }

        protected virtual void OnCellClick(GridCellClickEventArgs e)
        {
            base.RaiseEvent(e);
        }

        #endregion
        #region CellCursor
        public static readonly RoutedEvent CellCursorEvent = EventManager.RegisterRoutedEvent("CellCursor",
            RoutingStrategy.Direct,
            typeof(GridCellCursorEventHandler),
            typeof(GridControlBase));

        /// <summary>
        /// Grid queries for the cursor to display for a specific cell when the cell indicated
        /// previously with a non-zero hit-test value that it wants the mouse operation
        /// </summary>
        public event GridCellCursorEventHandler CellCursor
        {
            add
            {
                this.AddHandler(GridControlBase.CellCursorEvent, value);
            }
            remove
            {
                this.RemoveHandler(GridControlBase.CellCursorEvent, value);
            }
        }

        internal Cursor RaiseGridCellCursor()
        {
            GridCellCursorEventArgs args = new GridCellCursorEventArgs(GridControlBase.CellCursorEvent, this);
            OnGridCellCursor(args);
            if (args.Handled)
                return args.Cursor;
            return Cursors.Arrow;
        }

        protected virtual void OnGridCellCursor(GridCellCursorEventArgs args)
        {
            base.RaiseEvent(args);
        }

        #endregion
        #region CellMouseHoverEnter
        public static readonly RoutedEvent CellMouseHoverEnterEvent = EventManager.RegisterRoutedEvent(
            "CellMouseHoverEnter",
            RoutingStrategy.Direct,
            typeof(GridCellMouseEventHandler),
            typeof(GridControlBase));

        /// <summary>
        /// Occurs when the cell's HitTest method indicated previously with a non-zero hit-test value
        /// that it wants the mouse operation or when the user is hovering the mouse over cells
        /// and the "SelectCells" mouse controller is about to handle the mouse operation.
        /// </summary>
        public event GridCellMouseEventHandler CellMouseHoverEnter
        {
            add
            {
                this.AddHandler(GridControlBase.CellMouseHoverEnterEvent, value);
            }
            remove
            {
                this.RemoveHandler(GridControlBase.CellMouseHoverEnterEvent, value);
            }
        }

        internal void RaiseCellMouseHoverEnter(MouseEventArgs e)
        {
            this.OnCellMouseHoverEnter(new GridCellMouseEventArgs(GridControlBase.CellMouseHoverEnterEvent, this)
            {
                MouseEventArgs = e
            });
        }

        protected virtual void OnCellMouseHoverEnter(GridCellMouseEventArgs args)
        {
            base.RaiseEvent(args);
        }

        #endregion
        #region CellMouseHover
        public static readonly RoutedEvent CellMouseHoverEvent = EventManager.RegisterRoutedEvent(
            "CellMouseHover",
            RoutingStrategy.Direct,
            typeof(GridCellMouseControllerEventHandler),
            typeof(GridControlBase));

        /// <summary>
        /// Occurs when the cell's HitTest method indicated previously with a non-zero hit-test value
        /// that it wants the mouse operation or when the user is hovering the mouse over cells and
        /// the "SelectCells" mouse controller is about to handle the mouse operation.
        /// </summary>
        public event GridCellMouseControllerEventHandler CellMouseHover
        {
            add
            {
                this.AddHandler(GridControlBase.CellMouseHoverEvent, value);
            }
            remove
            {
                this.RemoveHandler(GridControlBase.CellMouseHoverEvent, value);
            }
        }

        internal void RaiseCellMouseHover(MouseControllerEventArgs args)
        {
            this.OnCellMouseHover(new GridCellMouseControllerEventArgs(GridControlBase.CellMouseHoverEvent, this)
            {
                MouseControllerEventArgs = args
            });
        }

        protected virtual void OnCellMouseHover(GridCellMouseControllerEventArgs args)
        {
            base.RaiseEvent(args);
        }

        #endregion
        #region CellMouseHoverLeave
        public static readonly RoutedEvent CellMouseHoverLeaveEvent = EventManager.RegisterRoutedEvent(
            "CellMouseHoverLeave",
            RoutingStrategy.Direct,
            typeof(GridCellMouseEventHandler),
            typeof(GridControlBase));

        /// <summary>
        /// Occurs when the cell's HitTest method indicated previously with a non-zero hit-test value
        /// that it wants the mouse operation or when the user is hovering the mouse over cells and
        /// the "SelectCells" mouse controller is about to handle the mouse operation.
        /// </summary>
        public event GridCellMouseEventHandler CellMouseHoverLeave
        {
            add
            {
                this.AddHandler(GridControlBase.CellMouseHoverLeaveEvent, value);
            }
            remove
            {
                this.RemoveHandler(GridControlBase.CellMouseHoverLeaveEvent, value);
            }
        }

        internal void RaiseCellMouseHoverLeave(MouseEventArgs e)
        {
            this.OnCellMouseHoverLeave(new GridCellMouseEventArgs(GridControlBase.CellMouseHoverLeaveEvent, this)
            {
                MouseEventArgs = e
            });
        }

        protected virtual void OnCellMouseHoverLeave(GridCellMouseEventArgs args)
        {
            base.RaiseEvent(args);
        }

        #endregion
        #region CellMouseDown
        public static readonly RoutedEvent CellMouseDownEvent = EventManager.RegisterRoutedEvent(
            "CellMouseDown",
            RoutingStrategy.Direct,
            typeof(GridCellMouseControllerEventHandler),
            typeof(GridControlBase));

        /// <summary>
        /// Occurs when your cell renderer has indicated in its OnHitTest override that
        /// it wants to receive mouse events and the user has pressed a mouse button.
        /// </summary>
        public event GridCellMouseControllerEventHandler CellMouseDown
        {
            add
            {
                this.AddHandler(GridControlBase.CellMouseDownEvent, value);
            }
            remove
            {
                this.RemoveHandler(GridControlBase.CellMouseDownEvent, value);
            }
        }

        internal void RaiseCellMouseDown(MouseControllerEventArgs e)
        {
            this.OnCellMouseDown(new GridCellMouseControllerEventArgs(GridControlBase.CellMouseDownEvent, this)
            {
                MouseControllerEventArgs = e
            });
        }

        protected virtual void OnCellMouseDown(GridCellMouseControllerEventArgs args)
        {
            base.RaiseEvent(args);
        }

        #endregion
        #region CellMouseMove
        public static readonly RoutedEvent CellMouseMoveEvent = EventManager.RegisterRoutedEvent(
            "CellMouseMove",
            RoutingStrategy.Direct,
            typeof(GridCellMouseControllerEventHandler),
            typeof(GridControlBase));

        /// <summary>
        /// Occurs when your cell renderer has indicated in its OnHitTest override that
        /// it wants to receive mouse events and the user has pressed a mouse button
        /// and is moving the mouse pointer.
        /// </summary>
        public event GridCellMouseControllerEventHandler CellMouseMove
        {
            add
            {
                this.AddHandler(GridControlBase.CellMouseMoveEvent, value);
            }
            remove
            {
                this.RemoveHandler(GridControlBase.CellMouseMoveEvent, value);
            }
        }

        internal void RaiseCellMouseMove(MouseControllerEventArgs e)
        {
            this.OnCellMouseMove(new GridCellMouseControllerEventArgs(GridControlBase.CellMouseMoveEvent, this)
            {
                MouseControllerEventArgs = e
            });
        }

        protected virtual void OnCellMouseMove(GridCellMouseControllerEventArgs args)
        {
            base.RaiseEvent(args);
        }

        #endregion
        #region CellMouseUp
        public static readonly RoutedEvent CellMouseUpEvent = EventManager.RegisterRoutedEvent(
            "CellMouseUp",
            RoutingStrategy.Direct,
            typeof(GridCellMouseControllerEventHandler),
            typeof(GridControlBase));

        /// <summary>
        /// Occurs when your cell renderer has indicated in its OnHitTest override that
        /// it wants to receive mouse events and the user has pressed a mouse button
        /// and is releasing the button.
        /// </summary>
        public event GridCellMouseControllerEventHandler CellMouseUp
        {
            add
            {
                this.AddHandler(GridControlBase.CellMouseUpEvent, value);
            }
            remove
            {
                this.RemoveHandler(GridControlBase.CellMouseUpEvent, value);
            }
        }

        internal void RaiseCellMouseUp(MouseControllerEventArgs e)
        {
            this.OnCellMouseUp(new GridCellMouseControllerEventArgs(GridControlBase.CellMouseUpEvent, this)
            {
                MouseControllerEventArgs = e
            });
        }

        protected virtual void OnCellMouseUp(GridCellMouseControllerEventArgs args)
        {
            base.RaiseEvent(args);
        }

        #endregion
        #region CellCancelMode
        public static readonly RoutedEvent CellCancelModeEvent = EventManager.RegisterRoutedEvent(
            "CellCancelMode",
            RoutingStrategy.Direct,
            typeof(GridRoutedEventHandler),
            typeof(GridControlBase));

        /// <summary>
        /// Occurs when your cell renderer has indicated in its OnHitTest override that
        /// it wants to receive mouse events and the mouse operation is canceled.
        /// </summary>
        public event GridRoutedEventHandler CellCancelMode
        {
            add
            {
                this.AddHandler(GridControlBase.CellCancelModeEvent, value);
            }
            remove
            {
                this.RemoveHandler(GridControlBase.CellCancelModeEvent, value);
            }
        }

        internal void RaiseCellCancelMode()
        {
            this.OnCellCancelMode(new SyncfusionRoutedEventArgs(GridControlBase.CellCancelModeEvent, this));
        }

        protected virtual void OnCellCancelMode(SyncfusionRoutedEventArgs args)
        {
            base.RaiseEvent(args);
        }

        #endregion
        #region CellRestoreMode
        public static readonly RoutedEvent CellRestoreModeEvent = EventManager.RegisterRoutedEvent(
            "CellRestoreMode",
            RoutingStrategy.Direct,
            typeof(GridRoutedEventHandler),
            typeof(GridControlBase));

        /// <summary>
        /// Occurs after the CellCancelMode event and is used to restore the cell state.
        /// </summary>
        public event GridRoutedEventHandler CellRestoreMode
        {
            add
            {
                this.AddHandler(GridControlBase.CellRestoreModeEvent, value);
            }
            remove
            {
                this.RemoveHandler(GridControlBase.CellRestoreModeEvent, value);
            }
        }

        internal void RaiseCellRestoreMode()
        {
            this.OnCellCancelMode(new SyncfusionRoutedEventArgs(GridControlBase.CellRestoreModeEvent, this));
        }

        protected virtual void OnCellRestoreMode(SyncfusionRoutedEventArgs args)
        {
            base.RaiseEvent(args);
        }

        #endregion
        #region ResizingColumns
        public static readonly RoutedEvent ResizingColumnsEvent = EventManager.RegisterRoutedEvent(
            "ResizingColumns",
            RoutingStrategy.Direct,
            typeof(GridResizingColumnsEventHandler),
            typeof(GridControlBase));

        /// <summary>
        /// Occurs when the user is resizing a selected range of columns.
        /// </summary>
        public event GridResizingColumnsEventHandler ResizingColumns
        {
            add
            {
                this.AddHandler(GridControlBase.ResizingColumnsEvent, value);
            }
            remove
            {
                this.RemoveHandler(GridControlBase.ResizingColumnsEvent, value);
            }
        }

        internal bool RaiseResizingColumnsEvent(GridRangeInfo columns, ref double width, GridResizeCellsReason reason, Point point)
        {
            return RaiseResizingColumnsEvent(columns, ref width, reason, point, true);
        }

        internal bool RaiseResizingColumnsEvent(GridRangeInfo columns, ref double width, GridResizeCellsReason reason, Point point, bool allowResize)
        {
            return RaiseResizingColumnsEvent(columns, ref width, reason, point, allowResize, false);
        }

        internal bool RaiseResizingColumnsEvent(GridRangeInfo columns, ref double width, GridResizeCellsReason reason, Point point, bool allowResize, bool inHiddenColResize)
        {
            GridResizingColumnsEventArgs e = new GridResizingColumnsEventArgs(GridControlBase.ResizingColumnsEvent, this)
            {
                Columns = columns,
                Width = width,
                Reason = reason,
                Point = point,
                AllowResize = allowResize,
                InHiddenColResize = inHiddenColResize
            };
            try
            {
                this.OnResizingColumns(e);
                width = e.Width;
            }
            catch
            {

            }
            return e.AllowResize;
        }

        protected virtual void OnResizingColumns(GridResizingColumnsEventArgs args)
        {
            base.RaiseEvent(args);
        }

        #endregion

        #region ColumnDrag
        public static readonly RoutedEvent QueryAllowDragColumnEvent = EventManager.RegisterRoutedEvent(
            "QueryAllowDragColumn",
            RoutingStrategy.Direct,
            typeof(GridQueryDragColumnHeaderEventHandler),
            typeof(GridControlBase));

        /// <summary>
        /// Occurs when the user hovers the mouse over the edge of a selected range.
        /// You can determine whether to allow the column drag action.
        /// </summary>
        public event GridQueryDragColumnHeaderEventHandler QueryAllowDragColumn
        {
            add
            {
                this.AddHandler(GridControlBase.QueryAllowDragColumnEvent, value);
            }

            remove
            {
                this.RemoveHandler(GridControlBase.QueryAllowDragColumnEvent, value);
            }
        }
        public virtual bool RaiseGridDataQueryAllowDragColumn(int colIndex, int insertBeforeColumn, GridQueryDragColumnHeaderReason reason, GridQueryDragColumnHeaderAction tag)
        {
            GridDataQueryDragColumnHeaderEventArgs ae = new GridDataQueryDragColumnHeaderEventArgs(colIndex, insertBeforeColumn, reason, tag, GridControlBase.QueryAllowDragColumnEvent, this);
            this.OnRaiseQueryAllowDragColumn(ae);
            return ae.AllowDrag;
        }
        public virtual bool RaiseQueryAllowDragColumn(int colIndex, int insertBeforeColumn, GridQueryDragColumnHeaderReason reason)
        {
            GridQueryDragColumnHeaderEventArgs ae = new GridQueryDragColumnHeaderEventArgs(colIndex, insertBeforeColumn, reason, GridControlBase.QueryAllowDragColumnEvent, this);
            this.OnRaiseQueryAllowDragColumn(ae);
            return ae.AllowDrag;
        }

        protected virtual void OnRaiseQueryAllowDragColumn(GridQueryDragColumnHeaderEventArgs ae)
        {
            this.RaiseEvent(ae);
        }

        #endregion

        #region ResizingRows
        public static readonly RoutedEvent ResizingRowsEvent = EventManager.RegisterRoutedEvent(
            "ResizingRows",
            RoutingStrategy.Direct,
            typeof(GridResizingRowsEventHandler),
            typeof(GridControlBase));

        /// <summary>
        /// Occurs when the user is resizing a selected range of rows.
        /// </summary>
        public event GridResizingRowsEventHandler ResizingRows
        {
            add
            {
                this.AddHandler(GridControlBase.ResizingRowsEvent, value);
            }
            remove
            {
                this.RemoveHandler(GridControlBase.ResizingRowsEvent, value);
            }
        }

        internal bool RaiseResizingRowsEvent(GridRangeInfo rows, ref double height, GridResizeCellsReason reason, Point point)
        {
            return RaiseResizingRowsEvent(rows, ref height, reason, point, true);
        }

        internal bool RaiseResizingRowsEvent(GridRangeInfo rows, ref double height, GridResizeCellsReason reason, Point point, bool allowResize)
        {
            return RaiseResizingRowsEvent(rows, ref height, reason, point, allowResize, false);
        }

        internal bool RaiseResizingRowsEvent(GridRangeInfo rows, ref double height, GridResizeCellsReason reason, Point point, bool allowResize, bool inHiddenRowResize)
        {
            GridResizingRowsEventArgs e = new GridResizingRowsEventArgs(GridControlBase.ResizingRowsEvent, this)
            {
                Rows = rows,
                Height = height,
                Reason = reason,
                Point = point,
                AllowResize = allowResize,
                InHiddenRowResize = inHiddenRowResize
            };
            this.OnResizingRows(e);
            height = e.Height;
            return e.AllowResize;
        }

        protected virtual void OnResizingRows(GridResizingRowsEventArgs args)
        {
            base.RaiseEvent(args);
        }

        #endregion

        #endregion

        bool IsCurrentCellKeyboardFocusWithin
        {
            get
            {
                if (CurrentCell.HasCurrentCell && CurrentCell.Renderer.CurrentCellUIElement != null)
                    return CurrentCell.Renderer.CurrentCellUIElement.IsKeyboardFocusWithin;
                return false;
            }
        }

        protected internal virtual GridViewMoveCellsState CreateGridViewMoveCellsState()
        {
            return new GridViewMoveCellsState();
        }

        internal void ScrollCellInView(int rowIndex, int colIndex, GridScrollCurrentCellReason gridScrollCurrentCellReason)
        {
            ScrollInView(new RowColumnIndex(rowIndex, colIndex));
        }

        protected override System.Windows.Automation.Peers.AutomationPeer OnCreateAutomationPeer()
        {
            //System.Diagnostics.Debug.WriteLine("GridControl CreateAutomationPeer called");
            return new GridControlAutomationPeer(this);
        }

        /// <summary>
        /// Gets called from ResizeRowsMouseController when user resizes a row. The method calls ScrollRows.SetLineResize followed by a call to InvalidateRowResize.
        /// </summary>
        /// <param name="index"></param>
        /// <param name="height"></param>
        public virtual void SetRowResize(int index, double height)
        {
            ScrollRows.SetLineResize(index, height);
            InvalidateRowResize();
        }

        /// <summary>
        /// Gets called from ResizeRowsMouseController when user ended resizing a row. The method calls ScrollRows.ResetLineResize followed by a call to InvalidateRowResize.
        /// </summary>
        public virtual void ResetRowResize()
        {
            ScrollRows.ResetLineResize();
            InvalidateRowResize();
        }
        
        /// <summary>
        /// Calls InvalidateVisual.
        /// </summary>
        public virtual void InvalidateRowResize()
        {
            InvalidateVisual(true);
        }

        public event GridCellCommentOpeningEvent CellCommentOpening;

        public void RaiseCellCommentOpening(RowColumnIndex cell, System.Windows.Controls.ContentControl popup, string corner)
        {
            if (CellCommentOpening != null)
            {
                CellCommentOpening(this, new GridCellCommentOpeningEventArgs() { Cell = cell, Popup = popup, Corner = corner });
            }
        }

        ///<summary>
        /// Occurs before the opening of Tooltip Popup
        /// </summary>
        public event GridCellToolTipOpeningEvent CellToolTipOpening;

        //Method for CellToolTipOpening event
        public void RaiseCellToolTipOpening(RowColumnIndex cell, System.Windows.Controls.ContentControl popup)
        {
            if (CellToolTipOpening != null)
            {
                CellToolTipOpening(this, new GridCellToolTipOpeningEventArgs() { Cell = cell, Popup = popup});
            }
        }

        public delegate void GridCellCommentOpeningEvent(object sender, GridCellCommentOpeningEventArgs e);

        //Delegate for GridCellToolTipOpening Event
        public delegate void GridCellToolTipOpeningEvent(object sender, GridCellToolTipOpeningEventArgs e);

        public delegate void GridCurrentCellLoadedEvent(object sender, GridCurrentCellLoadedEventArgs e);

        public event GridCurrentCellLoadedEvent CurrentCellLoaded;

        public void RaiseCurrenctCellLoaded(RowColumnIndex cell, UIElement element)
        {
            if (CurrentCellLoaded != null)
            {
                CurrentCellLoaded(this, new GridCurrentCellLoadedEventArgs() { Cell = cell, UIElement = element });
            }

        }
    }
    public class GridCellCommentOpeningEventArgs : EventArgs
    {
        public RowColumnIndex Cell { get; set; }
        public System.Windows.Controls.ContentControl Popup { get; set; }
        public string Corner { get; set; }
    }

    
    public class GridCellToolTipOpeningEventArgs : EventArgs
    {
        //Event Args for Tooltip Opening Event
        /// <summary>
        /// Get or Set Cell RowColumnIndex. Holds the Cell Rowindex and Columnindex
        /// </summary>
        public RowColumnIndex Cell { get; set; }
        /// <summary>
        /// Get or Set Popup. Holds the ToolTip Popup
        /// </summary>
        public System.Windows.Controls.ContentControl Popup { get; set; }
    }

    public class GridCurrentCellLoadedEventArgs : EventArgs
    {
        public RowColumnIndex Cell { get; set; }
        public UIElement UIElement { get; set; }
    }

}
